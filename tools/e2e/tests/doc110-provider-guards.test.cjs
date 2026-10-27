'use strict';
const {test}=require('node:test');
const assert=require('node:assert/strict');
const {exerciseProviderGuards}=require('./support/doc110-provider-guards.cjs');
function fixture(changed=false,wrongCode=false){
  const current={ProveedorId:'1',VersionId:'2',Revision:2,RevisionProveedor:2,Activo:false,Contrato:{Propiedades:[{Nombre:'consecutivo'}]}};
  let reads=0,closed=false;
  const calls=[];
  function context(anonymous){return {request:{async post(url,options){
    const method=url.split('/').pop();calls.push({method,options});
    let status=200,code=null,data=current;
    const valid=options.headers['X-Importacion-Perfil-CSRF']==='synthetic';
    const stale=options.data.solicitud.Revision===current.Revision-1;
    if(method!=='ObtenerAdministracionProveedor'||anonymous||!valid||stale){
      status=anonymous?401:valid?409:403;
      code=anonymous?'SESION_REQUERIDA':valid?'CONFLICTO_REVISION':'SOLICITUD_NO_VERIFICADA';
      if(wrongCode)code='ACCESO_DENEGADO';data=null;
    }else{
      assert.equal(method,'ObtenerAdministracionProveedor');reads++;
      if(changed)data={...current,Revision:3};
    }
    return {status:()=>status,json:async()=>({d:{ok:status===200,codigo:code,datos:data}})};
  }},close:async()=>{closed=true;}};}
  const authenticated=context(false);authenticated.browser=()=>({newContext:async()=>context(true)});
  return {current,auth:{context:authenticated},calls,reads:()=>reads,closed:()=>closed};
}
test('33 rechazos en nueve endpoints, documento persistido y cierre anonimo',async()=>{
  const f=fixture();await exerciseProviderGuards(f.auth,'http://localhost/','synthetic',f.current);
  assert.equal(f.reads(),33);assert.equal(f.closed(),true);assert.equal(f.calls.length,66);
  const probes=f.calls.filter((_,i)=>i%2===0);
  assert.equal(new Set(probes.map(c=>c.method)).size,9);
  assert.equal(probes.filter(c=>c.options.data.solicitud.Revision===1).length,6);
  for(const c of probes.filter(c=>c.method==='CrearProveedor')){
    assert.equal(c.options.data.solicitud.Codigo,undefined);
    assert.equal(c.options.data.solicitud.Nombre,undefined);
  }
});
test('rechazo no acredita proteccion si cambia el documento o el codigo es incorrecto',async()=>{
  for(const f of [fixture(true),fixture(false,true)])
    await assert.rejects(exerciseProviderGuards(f.auth,'http://localhost/','synthetic',f.current));
});
