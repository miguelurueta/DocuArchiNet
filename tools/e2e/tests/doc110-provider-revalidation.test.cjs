'use strict';
const {test}=require('node:test');
const assert=require('node:assert/strict');
const {verifyProviderRevalidation}=require('./support/doc110-provider-revalidation.cjs');
function fixture(fault){
  let provider={ProveedorId:'1',VersionId:'2',Estado:'PUBLICADA',Activo:true,Revision:3,RevisionProveedor:4,Contrato:{Propiedades:[]}};
  const initial=structuredClone(provider);
  provider.RevisionProveedor++; // El ciclo UI crea después otra versión del proveedor.
  const profile={PerfilId:'3',Revision:1,Version:{VersionId:'4',Revision:2,Estado:'BORRADOR',Reglas:[{FuenteTipo:'PROVEEDOR',Proveedor:{ProveedorId:'1',VersionId:'2'}}]}};
  const history=[structuredClone(profile.Version)];
  const calls=[];let failed=false;
  const call=async(method,payload,status=200)=>{
    calls.push({method,payload:structuredClone(payload),status});
    if(method==='CambiarDisponibilidadProveedor'){
      assert.equal(payload.ProveedorId,'1');assert.equal(payload.VersionId,'2');
      assert.equal(payload.Revision,provider.Revision);
      assert.equal(payload.RevisionProveedor,provider.RevisionProveedor);
      provider={...provider,Activo:payload.Activo,RevisionProveedor:provider.RevisionProveedor+1};
      if(fault==='lost-disable' && !payload.Activo)throw new Error('simulated response loss');
      return {datos:structuredClone(provider)};
    }
    if(method==='ObtenerAdministracionProveedor')return {datos:structuredClone(provider)};
    if(method==='ConsultarHistorial')return {datos:failed && fault==='history'?[]:structuredClone(history)};
    if(method==='ObtenerPerfil')return {datos:fault==='mutation'?{...profile,Revision:99}:structuredClone(profile)};
    assert.equal(method,'PublicarVersion');assert.equal(status,403);assert.equal(provider.Activo,false);
    assert.deepEqual(payload,{perfilId:'3',versionId:'4',revisionPerfilEsperada:1,revisionVersionEsperada:2});
    failed=true;
    return {codigo:fault==='wrong-code'?'SOLICITUD_NO_VERIFICADA':'ACCESO_DENEGADO',datos:null};
  };
  return {call,profile,initial,calls,provider:()=>provider};
}
test('revalida proveedor al publicar y restaura solo su disponibilidad con revision vigente',async()=>{
  const f=fixture();await verifyProviderRevalidation(f.call,f.profile,f.initial);
  assert.deepEqual(f.calls.map(c=>c.method),['ObtenerAdministracionProveedor','ConsultarHistorial','CambiarDisponibilidadProveedor','PublicarVersion',
    'ObtenerPerfil','ConsultarHistorial','ObtenerAdministracionProveedor','CambiarDisponibilidadProveedor']);
  assert.deepEqual(f.provider(),{...f.initial,RevisionProveedor:7});
});
test('rechazo incorrecto, perfil o historial alterados fallan y restauran el proveedor',async()=>{
  for(const fault of ['wrong-code','mutation','history']){
    const f=fixture(fault);await assert.rejects(verifyProviderRevalidation(f.call,f.profile,f.initial));
    assert.equal(f.provider().Activo,true);assert.equal(f.provider().RevisionProveedor,7);
  }
});
test('respuesta de desactivacion perdida se recupera leyendo estado sin repetir escritura',async()=>{
  const f=fixture('lost-disable');await assert.rejects(verifyProviderRevalidation(f.call,f.profile,f.initial));
  assert.equal(f.provider().Activo,true);
  assert.deepEqual(f.calls.filter(c=>c.method==='CambiarDisponibilidadProveedor').map(c=>c.payload.Activo),[false,true]);
  assert.equal(f.calls.some(c=>c.method==='PublicarVersion'),false);
});
