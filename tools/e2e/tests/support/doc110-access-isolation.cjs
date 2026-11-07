'use strict';
const {expect}=require('@playwright/test');
const {createWorkflowLoginPageSession}=require('./authenticated-workflow-session.cjs');
const {safePost}=require('./doc110-safe-request.cjs');

// Solo lecturas: el usuario debe autorizar una segunda cuenta sin acceso al perfil.
async function exerciseAccessIsolation(browser,baseUrl,profile,provider){
  if(!profile)throw new Error('DOC110_ISOLATION_REQUIRES_EXISTING_PROFILE');
  if(!process.env.DOC110_E2E_SECOND_USER || !process.env.DOC110_E2E_SECOND_PASSWORD)
    throw new Error('DOC110_SECOND_ACCOUNT_REQUIRED');
  expect(process.env.DOC110_E2E_SECOND_USER.toUpperCase()!==process.env.DOC110_E2E_USER.toUpperCase()).toBe(true);
  const auth=await createWorkflowLoginPageSession(browser,{
    baseUrl,moduleEnvironmentVariable:'DOC110_E2E_MODULE',
    userEnvironmentVariable:'DOC110_E2E_SECOND_USER',passwordEnvironmentVariable:'DOC110_E2E_SECOND_PASSWORD',
    ignoreHTTPSErrors:process.env.DOC10_E2E_IGNORE_HTTPS_ERRORS==='true'
  });
  try{
    await auth.page.goto(new URL('Gestion/WebFormProducionDocumental.aspx',baseUrl).href);
    let token;
    if(await auth.page.locator('#doc110-perfiles').count()){
      await auth.page.locator('#doc110-navigation-toggle').click();
      const pending=auth.page.waitForResponse(r=>r.url().endsWith('/ListarPerfiles') && r.request().method()==='POST');
      await auth.page.locator('[data-doc110=abrir]').click();
      const response=await pending;
      expect(response.status()).toBe(200);
      token=response.request().headers()['x-importacion-perfil-csrf'];
      if(!token)throw new Error('DOC110_CSRF_NOT_OBSERVED');
    }
    for(const [method,payload] of [
      ['ListarPerfiles',{codigo:profile.Codigo,pagina:1,tamanoPagina:10}],
      ['ObtenerPerfil',{perfilId:profile.PerfilId}],
      ['ConsultarHistorial',{perfilId:profile.PerfilId,pagina:1,tamanoPagina:10}]
    ]){
      const response=await safePost(auth.context.request,new URL('webservice/WebServiceImportacionCarpetas.asmx/'+method,baseUrl).href,{
        headers:token?{'X-Importacion-Perfil-CSRF':token}:{},data:{solicitud:payload}
      },method);
      let result;try{result=(await response.json()).d;}catch{throw new Error('DOC110_NON_JSON: '+method);}
      if(method==='ListarPerfiles' && response.status()===200){
        expect(result?.ok).toBe(true);
        expect(result.datos.Elementos.length).toBe(0);
        expect(result.datos.Total).toBe(0);
      }else{
        expect(response.status()).toBe(403);
        expect(result?.ok).toBe(false);
        expect(result?.codigo).toBe('ACCESO_DENEGADO');
        expect(result?.datos==null).toBe(true);
      }
      console.log('DOC110_PASS: SECOND_ACCOUNT_DENIED_'+method.toUpperCase());
    }
    await require('./doc110-provider-access.cjs').exerciseProviderAccess(auth,baseUrl,token,provider);
  }finally{await auth.context.close();}
}
module.exports={exerciseAccessIsolation};
