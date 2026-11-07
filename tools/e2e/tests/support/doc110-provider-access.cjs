'use strict';
const {expect}=require('@playwright/test');
const {safePost}=require('./doc110-safe-request.cjs');

// Cuenta autenticada sin permiso: solo lecturas, sin ensayos ni cambios de disponibilidad.
async function exerciseProviderAccess(auth,baseUrl,token,provider){
  if(!provider?.ProveedorId || !provider?.VersionId)throw new Error('DOC110_PROVIDER_ACCESS_REQUIRES_EXISTING_PROVIDER');
  for(const [method,payload] of [
    ['ListarAdministracionProveedores',{Pagina:1,ProveedorId:provider.ProveedorId}],
    ['ObtenerAdministracionProveedor',{ProveedorId:provider.ProveedorId,VersionId:provider.VersionId}],
    ['ConsultarMetadatosProveedor',{tipo:'COLUMNAS',conexionRef:'DA',tabla:'imp_02_egresos',pagina:1}]
  ]){
    const response=await safePost(auth.context.request,new URL('webservice/WebServiceImportacionCarpetas.asmx/'+method,baseUrl).href,{
      headers:token?{'X-Importacion-Perfil-CSRF':token}:{},data:{solicitud:payload}
    },method);
    let result;try{result=(await response.json()).d;}catch{throw new Error('DOC110_PROVIDER_ACCESS_NON_JSON: '+method);}
    expect(response.status()).toBe(403);
    expect(result?.ok).toBe(false);
    expect(result?.codigo).toBe('ACCESO_DENEGADO');
    expect(result?.datos==null).toBe(true);
    console.log('DOC110_PASS: SECOND_ACCOUNT_DENIED_PROVIDER_'+method.toUpperCase());
  }
}
module.exports={exerciseProviderAccess};
