'use strict';
const {expect}=require('@playwright/test');
const {safePost}=require('./doc110-safe-request.cjs');

async function exerciseProviderGuards(auth,baseUrl,token,current){
  const refs={ProveedorId:current.ProveedorId,VersionId:current.VersionId,Revision:current.Revision,RevisionProveedor:current.RevisionProveedor};
  const contract=JSON.parse(JSON.stringify(current.Contrato));
  contract.Propiedades[0].Nombre='guard_probe';
  const payload={...refs,Contrato:contract};
  async function post(context,method,headers,data){
    const response=await safePost(context.request,new URL('webservice/WebServiceImportacionCarpetas.asmx/'+method,baseUrl).href,{headers,data:{solicitud:data}},method);
    let result;try{result=(await response.json()).d;}catch{throw new Error('DOC110_PROVIDER_GUARD_NON_JSON: '+method);}
    return {status:response.status(),result};
  }
  async function rejected(context,method,headers,status,code,label,data=payload){
    const response=await post(context,method,headers,data);
    expect(response.status).toBe(status);expect(response.result?.ok).toBe(false);
    expect(response.result?.codigo).toBe(code);expect(response.result?.datos==null).toBe(true);
    const after=await post(auth.context,'ObtenerAdministracionProveedor',{'X-Importacion-Perfil-CSRF':token},refs);
    expect(after.status).toBe(200);expect(after.result?.ok).toBe(true);
    expect(JSON.stringify(after.result.datos)===JSON.stringify(current)).toBe(true);
    console.log('DOC110_PASS: PROVIDER_'+label+'_'+method.toUpperCase()+'_NO_CHANGE');
  }
  const methods=['CrearProveedor','GuardarProveedor','PublicarProveedor','CrearVersionProveedor',
    'CambiarDisponibilidadProveedor','ValidarProveedor','EnsayarProveedor',
    'ListarAdministracionProveedores','ObtenerAdministracionProveedor'];
  const anonymous=await auth.context.browser().newContext({ignoreHTTPSErrors:process.env.DOC10_E2E_IGNORE_HTTPS_ERRORS==='true'});
  try{
    for(const method of methods){
      // CREAR carece deliberadamente de código/nombre válidos. Nunca se envía un alta válida.
      await rejected(auth.context,method,{},403,'SOLICITUD_NO_VERIFICADA','MISSING_CSRF');
      await rejected(auth.context,method,{'X-Importacion-Perfil-CSRF':'doc110-invalid-token'},403,'SOLICITUD_NO_VERIFICADA','INVALID_CSRF');
      await rejected(anonymous,method,{'X-Importacion-Perfil-CSRF':token},401,'SESION_REQUERIDA','NO_SESSION');
    }
  }finally{await anonymous.close();}
  expect(current.Revision).toBeGreaterThan(1);
  for(const method of methods.filter(m=>!['CrearProveedor','ListarAdministracionProveedores','ObtenerAdministracionProveedor'].includes(m))){
    await rejected(auth.context,method,{'X-Importacion-Perfil-CSRF':token},409,'CONFLICTO_REVISION','STALE_REVISION',{...payload,Revision:current.Revision-1});
  }
}
module.exports={exerciseProviderGuards};
