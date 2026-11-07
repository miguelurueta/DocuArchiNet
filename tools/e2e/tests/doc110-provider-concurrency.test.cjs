'use strict';
const {test}=require('node:test');
const assert=require('node:assert/strict');
const {runProviderRaces}=require('./support/doc110-provider-concurrency.cjs');
const copy=x=>JSON.parse(JSON.stringify(x));
function fixture(fault){
  const initial={ProveedorId:'1',VersionId:'1',Numero:1,Revision:2,RevisionProveedor:2,Activo:false,Estado:'BORRADOR',Contrato:{Propiedades:[{Nombre:'consecutivo'}]}};
  let header=2;const versions=[copy(initial)];const mutations=[];
  const peer=async(method,s)=>{
    let doc=versions.find(d=>d.VersionId===s.VersionId);
    const ok=data=>({status:200,result:{ok:true,datos:copy(data)}});
    if(method==='ObtenerAdministracionProveedor')return ok({...doc,RevisionProveedor:header});
    if(method==='ListarAdministracionProveedores')return ok(versions.map(d=>({...d,RevisionProveedor:header})));
    if(s.RevisionProveedor!==header || s.Revision!==doc.Revision){
      if(fault==='double-success')return ok({...doc,RevisionProveedor:header});
      return {status:409,result:{ok:false,codigo:'CONFLICTO_REVISION',datos:null}};
    }
    mutations.push(method);header++;
    if(method==='GuardarProveedor'){doc.Contrato=copy(s.Contrato);doc.Revision+=fault==='double-increment'?2:1;}
    else if(method==='PublicarProveedor'){doc.Estado='PUBLICADA';doc.Revision++;}
    else if(method==='CrearVersionProveedor'){
      doc={...copy(doc),VersionId:'2',Numero:2,Revision:1,Estado:'BORRADOR'};versions.push(doc);
      if(fault==='rewrite-old')versions[0].Contrato.Propiedades[0].Nombre='changed';
    }else if(method==='CambiarDisponibilidadProveedor'){
      for(const version of versions)version.Activo=s.Activo;
      if(s.Activo && fault==='activation-mutates-version')doc.Revision++;
      if(s.Activo && fault==='activation-transport')throw new Error('synthetic transport failure after commit');
    }else throw new Error('Unexpected mutation');
    return ok({...doc,RevisionProveedor:header});
  };
  return {initial,peers:[peer,peer],mutations,isActive:()=>versions.some(v=>v.Activo)};
}
test('tres carreras exigen ganador unico, revisiones y primera version intacta',async()=>{
  const f=fixture();await runProviderRaces(f.peers,f.initial);
  assert.deepEqual(f.mutations,['GuardarProveedor','PublicarProveedor','CrearVersionProveedor']);
});

test('verifica el contrato ganador tambien cuando vence la segunda sesion',async()=>{
  const f=fixture();
  const delayed=async(method,data)=>{
    await new Promise(resolve=>setImmediate(resolve));
    return f.peers[0](method,data);
  };
  await runProviderRaces([delayed,f.peers[1]],f.initial,true);
  assert.equal(f.isActive(),false);
  const published=await f.peers[1]('ObtenerAdministracionProveedor',{VersionId:'1'});
  assert.equal(published.result.datos.Contrato.Propiedades[0].Nombre,'concurrent_b');
  assert.deepEqual(f.mutations,['GuardarProveedor','PublicarProveedor','CrearVersionProveedor',
    'CambiarDisponibilidadProveedor','CambiarDisponibilidadProveedor']);
});
test('disponibilidad concurrente conserva historial y desactiva al terminar',async()=>{
  const f=fixture();await runProviderRaces(f.peers,f.initial,true);
  assert.equal(f.isActive(),false);
  assert.deepEqual(f.mutations.slice(-2),['CambiarDisponibilidadProveedor','CambiarDisponibilidadProveedor']);
});
test('desactiva tambien ante fallo de asercion o transporte despues de activar',async()=>{
  for(const fault of ['activation-mutates-version','activation-transport']){
    const f=fixture(fault);await assert.rejects(runProviderRaces(f.peers,f.initial,true));
    assert.equal(f.isActive(),false);
    assert.equal(f.mutations.at(-1),'CambiarDisponibilidadProveedor');
  }
});
test('no acredita doble exito, doble incremento ni reescritura de publicacion',async()=>{
  for(const fault of ['double-success','double-increment','rewrite-old']){
    const f=fixture(fault);await assert.rejects(runProviderRaces(f.peers,f.initial));
  }
});
