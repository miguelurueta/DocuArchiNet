'use strict';
const {expect}=require('@playwright/test');
async function verifyDraftConsumer(call,profile,draft){
  expect(draft.Estado).toBe('BORRADOR');expect(draft.Activo).toBe(true);
  const options=(await call('ConsultarProveedores',{pagina:1,tamanoPagina:25,proveedorId:draft.ProveedorId,versionId:draft.VersionId})).datos;
  expect(Array.isArray(options)).toBe(true);expect(options.length).toBe(0);
  console.log('DOC110_PASS: ACTIVE_PROVIDER_DRAFT_EXCLUDED_FROM_CONSUMER_CATALOG');
  const reglas=JSON.parse(JSON.stringify(profile.Version.Reglas));
  const rule=reglas.find(r=>r.FuenteTipo==='PROVEEDOR' && r.Proveedor.ProveedorId===draft.ProveedorId);
  expect(Boolean(rule)).toBe(true);rule.Proveedor.VersionId=draft.VersionId;
  const payload={perfilId:profile.PerfilId,versionId:profile.Version.VersionId,revisionVersionEsperada:profile.Version.Revision,
    configuracion:profile.Version.Configuracion,reglas};
  for(const method of ['GuardarBorrador','EnsayarReglas']){
    const request=method==='EnsayarReglas'?{...payload,ejemplos:[{NombreCarpeta:'DOC110',NombreArchivo:'DOC110.pdf',Expediente:{}}]}:payload;
    const rejected=await call(method,request,403);
    expect(rejected.codigo).toBe('ACCESO_DENEGADO');expect(rejected.datos==null).toBe(true);
    const after=(await call('ObtenerPerfil',{perfilId:profile.PerfilId,versionId:profile.Version.VersionId})).datos;
    expect(JSON.stringify(after)===JSON.stringify(profile)).toBe(true);
    console.log('DOC110_PASS: ACTIVE_PROVIDER_DRAFT_DENIED_'+method.toUpperCase()+'_PROFILE_UNCHANGED');
  }
  const after=(await call('ObtenerAdministracionProveedor',{ProveedorId:draft.ProveedorId,VersionId:draft.VersionId})).datos;
  expect(JSON.stringify(after)===JSON.stringify(draft)).toBe(true);
  console.log('DOC110_PASS: REJECTED_CONSUMER_REQUESTS_KEEP_PROVIDER_DRAFT_UNCHANGED');
}
module.exports={verifyDraftConsumer};
