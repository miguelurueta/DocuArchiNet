'use strict';
const {test}=require('node:test');
const assert=require('node:assert/strict');
const {exerciseProviderAccess}=require('./support/doc110-provider-access.cjs');
const provider={ProveedorId:1,VersionId:2};
function context(status=403,code='ACCESO_DENEGADO',data=null){
  const calls=[];
  return {calls,auth:{context:{request:{async post(url){
    calls.push(url.split('/').pop());
    return {status:()=>status,json:async()=>({d:{ok:status===200,codigo:code,datos:data}})};
  }}}}};
}
test('rechazo autenticado sin datos en tres endpoints de lectura',async()=>{
  const c=context();await exerciseProviderAccess(c.auth,'http://localhost/',undefined,provider);
  assert.deepEqual(c.calls,['ListarAdministracionProveedores','ObtenerAdministracionProveedor','ConsultarMetadatosProveedor']);
});
test('no confunde CSRF ni sesion ausente con falta de permiso',async()=>{
  for(const [status,code] of [[403,'SOLICITUD_NO_VERIFICADA'],[401,'SESION_REQUERIDA'],[200,null]]){
    const c=context(status,code);await assert.rejects(exerciseProviderAccess(c.auth,'http://localhost/',undefined,provider));
  }
});
test('rechazo con datos filtrados y ausencia de candidato no acreditan seguridad',async()=>{
  const c=context(403,'ACCESO_DENEGADO',{});
  await assert.rejects(exerciseProviderAccess(c.auth,'http://localhost/',undefined,provider));
  await assert.rejects(exerciseProviderAccess(c.auth,'http://localhost/',undefined,null));
});
