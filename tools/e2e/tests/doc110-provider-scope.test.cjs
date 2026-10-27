'use strict';
const {test}=require('node:test');
const assert=require('node:assert/strict');
const {exerciseProviderScope}=require('./support/doc110-provider-scope.cjs');
function context(items,denied=true){
  const calls=[];
  const auth={context:{request:{async post(url){
    const method=url.split('/').pop();calls.push(method);
    if(!['ListarAdministracionProveedores','ObtenerAdministracionProveedor','ValidarProveedor'].includes(method))throw new Error('Unexpected operation');
    const blocked=method==='ValidarProveedor' && denied;
    return {status:()=>blocked?403:200,json:async()=>({d:{ok:!blocked,codigo:blocked?'ACCESO_DENEGADO':null,datos:blocked?null:method==='ListarAdministracionProveedores'?items:items[0]}})};
  }}}};return {auth,calls};
}
test('sin candidato no acredita rechazo de proveedor',async()=>{
  const c=context([]);
  assert.deepEqual(await exerciseProviderScope(c.auth,'http://localhost/','synthetic'),{status:'NOT_RUN',checked:0});
  assert.deepEqual(c.calls,['ListarAdministracionProveedores']);
});
const provider={ProveedorId:'1',VersionId:'1',Revision:1,RevisionProveedor:1,Contrato:{ConexionRef:'DA',Tabla:'legacy_source'}};
test('valida proveedor anterior sin ejecutar consulta ni mutar',async()=>{
  const c=context([provider]);
  assert.deepEqual(await exerciseProviderScope(c.auth,'http://localhost/','synthetic'),{status:'PASS',checked:1});
  assert.deepEqual(c.calls,['ListarAdministracionProveedores','ObtenerAdministracionProveedor','ValidarProveedor','ObtenerAdministracionProveedor']);
});
test('una validacion permitida fuera del prefijo falla',async()=>{
  const c=context([provider],false);
  await assert.rejects(exerciseProviderScope(c.auth,'http://localhost/','synthetic'));
});

test('fixture declarado debe encontrarse y permanecer inactivo',async()=>{
  const fixture={providerId:'1',versionId:'1',code:'SYNTHETIC',connectionRef:'DA',table:'legacy_source'};
  const c=context([{...provider,Codigo:'SYNTHETIC',Activo:false,Estado:'BORRADOR'}]);
  assert.deepEqual(await exerciseProviderScope(c.auth,'http://localhost/','synthetic',fixture),{status:'PASS',checked:1});
  await assert.rejects(exerciseProviderScope(context([]).auth,'http://localhost/','synthetic',fixture));
  await assert.rejects(exerciseProviderScope(context([{...provider,Codigo:'SYNTHETIC',Activo:true,Estado:'BORRADOR'}]).auth,'http://localhost/','synthetic',fixture));
});
