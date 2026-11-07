'use strict';
const {test,expect}=require('@playwright/test');
const {createWorkflowLoginPageSession}=require('./support/authenticated-workflow-session.cjs');
const {safePost}=require('./support/doc110-safe-request.cjs');
test.use({screenshot:'off',trace:'off',video:'off'});
test.describe.configure({retries:0});
test('DOC-110: sesión administrativa, apertura y listado real',async({browser})=>{
  test.setTimeout(180000);
  if(process.env.DOC110_E2E_SESSION_EXPIRY==='YES'){
    const minutes=Number(process.env.DOC110_E2E_SESSION_IDLE_MINUTES);
    if(!Number.isInteger(minutes)||minutes<1||minutes>60)throw new Error('DOC110_SESSION_TIMEOUT_CONFIGURATION_REQUIRED');
    // Solo este modo necesita esperar la inactividad configurada, sin cambiar el servidor.
    test.setTimeout(minutes*60000+255000);
  }
  if(!process.env.DOC110_E2E_USER || !process.env.DOC110_E2E_PASSWORD)
    throw new Error('DOC110_SESSION_BLOCKED: faltan credenciales efímeras.');
  const auth=await createWorkflowLoginPageSession(browser,{
    baseUrl:process.env.DOC110_E2E_BASE_URL,
    moduleEnvironmentVariable:'DOC110_E2E_MODULE',
    userEnvironmentVariable:'DOC110_E2E_USER',
    passwordEnvironmentVariable:'DOC110_E2E_PASSWORD',
    ignoreHTTPSErrors:process.env.DOC10_E2E_IGNORE_HTTPS_ERRORS==='true'
  });
  try{
    await auth.page.goto(new URL('Gestion/WebFormProducionDocumental.aspx',process.env.DOC110_E2E_BASE_URL).href);
    if(await auth.page.locator('#doc110-perfiles').count()===0){
      // Diagnóstico de solo lectura por el mismo contexto; sin tokens ni payloads en evidencia.
      const probe=await safePost(auth.context.request,new URL('webservice/WebServiceImportacionCarpetas.asmx/ListarPerfiles',process.env.DOC110_E2E_BASE_URL).href,{
        data:{solicitud:{pagina:1,tamanoPagina:1}},maxRedirects:0
      },'ListarPerfiles');
      let code;try{code=(await probe.json()).d?.codigo;}catch{}
      if(probe.status()===403 && code==='ACCESO_DENEGADO')
        throw new Error('DOC110_ACCESS_DENIED: verificar ADMINISTRAR_PERFILES_IMPORTACION y ámbito de la cuenta. El servidor no habilitó el editor.');
      if(probe.status()===401)
        throw new Error('DOC110_SESSION_REQUIRED: el login no dejó una sesión de Gestión vigente.');
      if(probe.status()===503)
        throw new Error('DOC110_UNAVAILABLE: no se pudo comprobar la capacidad en el servidor.');
      throw new Error('DOC110_EDITOR_HOST_MISSING: la página no emitió el contenedor del editor; requiere diagnóstico del servidor.');
    }
    await auth.page.locator('#doc110-navigation-toggle').click();
    await expect(auth.page.locator('[data-doc110=abrir]')).toBeVisible();
    const pending=auth.page.waitForResponse(r=>r.url().endsWith('/ListarPerfiles') && r.request().method()==='POST');
    await auth.page.locator('[data-doc110=abrir]').click();
    const response=await pending;
    expect(response.status()).toBe(200);
    const result=await response.json();
    expect(result.d?.ok).toBe(true);
    await expect(auth.page.locator('[data-doc110=nuevo]')).toBeVisible();
    // COR-03: consulta real con la sesión autorizada; no crea ni modifica perfiles.
    const sample=result.d.datos.Elementos[0];
    if(process.env.DOC110_E2E_PROVIDER_SCOPE==='YES'){
      const token=response.request().headers()['x-importacion-perfil-csrf'];
      if(!token)throw new Error('DOC110_CSRF_NOT_OBSERVED');
      const fs=require('node:fs'),path=require('node:path');
      const fixturePath=path.resolve(__dirname,'../artifacts/doc110-legacy-provider-fixture-result.json');
      const fixture=fs.existsSync(fixturePath)?JSON.parse(fs.readFileSync(fixturePath,'utf8').replace(/^\uFEFF/,'')):null;
      if(fixture && !(fixture.status==='CREATED' && fixture.synthetic===true && fixture.inactive===true))throw new Error('DOC110_LEGACY_FIXTURE_REPORT_INVALID');
      await require('./support/doc110-source-scope.cjs').exerciseSourceScope(auth,process.env.DOC110_E2E_BASE_URL,token);
      const scope=await require('./support/doc110-provider-scope.cjs').exerciseProviderScope(auth,process.env.DOC110_E2E_BASE_URL,token,fixture);
      test.skip(scope.status==='NOT_RUN','No existe proveedor previo fuera del prefijo; escenario no ejecutado.');
      fs.writeFileSync(path.resolve(__dirname,'../artifacts/doc110-provider-scope-result.json'),JSON.stringify({
        ...scope,syntheticFixture:!!fixture,fixtureProviderId:fixture?.providerId,fixtureVersionId:fixture?.versionId,
        prefixMetadataAndForeignTableDenial:true,documentUnchanged:true,finishedUtc:new Date().toISOString()
      },null,2));
    }
    if(process.env.DOC110_E2E_SOURCE_SCOPE==='YES'){
      const token=response.request().headers()['x-importacion-perfil-csrf'];
      if(!token)throw new Error('DOC110_CSRF_NOT_OBSERVED');
      await require('./support/doc110-source-scope.cjs').exerciseSourceScope(auth,process.env.DOC110_E2E_BASE_URL,token);
    }
    if(process.env.DOC110_E2E_ACCESS_ISOLATION==='YES'){
      const token=response.request().headers()['x-importacion-perfil-csrf'];
      if(!token)throw new Error('DOC110_CSRF_NOT_OBSERVED');
      const providersResponse=await safePost(auth.context.request,new URL('webservice/WebServiceImportacionCarpetas.asmx/ListarAdministracionProveedores',process.env.DOC110_E2E_BASE_URL).href,{
        headers:{'X-Importacion-Perfil-CSRF':token},data:{solicitud:{Pagina:1}}
      },'ListarAdministracionProveedores');
      expect(providersResponse.status()).toBe(200);
      let providers;try{providers=(await providersResponse.json()).d;}catch{throw new Error('DOC110_PROVIDER_ACCESS_NON_JSON');}
      expect(providers?.ok).toBe(true);
      if(!providers.datos?.length)throw new Error('DOC110_PROVIDER_ACCESS_REQUIRES_EXISTING_PROVIDER');
      await require('./support/doc110-access-isolation.cjs').exerciseAccessIsolation(browser,process.env.DOC110_E2E_BASE_URL,sample,providers.datos[0]);
    }
    if(sample && sample.Codigo.length>=2 && sample.Nombre.length>=2){
      const search=auth.page.locator('[data-doc110=buscar-texto]');
      for(const term of [sample.Codigo.slice(0,2),sample.Nombre.slice(0,2)]){
        const suggestions=auth.page.waitForResponse(r=>r.url().endsWith('/ListarPerfiles') && r.request().postDataJSON()?.solicitud?.termino===term);
        await search.fill(term);await search.press('End');
        expect((await suggestions).status()).toBe(200);
        await expect(auth.page.locator('#doc110-perfiles section[aria-labelledby=doc110-title] .ui-autocomplete li').filter({hasText:sample.Codigo+' — '+sample.Nombre}).first()).toBeVisible();
      }
      const filtered=auth.page.waitForResponse(r=>r.url().endsWith('/ListarPerfiles') && r.request().postDataJSON()?.solicitud?.codigo===sample.Codigo);
      await auth.page.locator('#doc110-perfiles section[aria-labelledby=doc110-title] .ui-autocomplete li').filter({hasText:sample.Codigo+' — '+sample.Nombre}).first().click();
      expect((await filtered).status()).toBe(200);
      const cleared=auth.page.waitForResponse(r=>r.url().endsWith('/ListarPerfiles') && r.request().postDataJSON()?.solicitud?.codigo===null);
      await auth.page.locator('[data-doc110=limpiar-busqueda]').click();
      expect((await cleared).status()).toBe(200);
      console.log('DOC110_PASS: AUTOCOMPLETE_CODE_NAME_SELECTION_CLEAR');
      await require('./support/doc110-profile-search.cjs').exerciseProfileSearch(auth.page,sample);
    }else{console.log('DOC110_NOT_RUN: AUTOCOMPLETE_REQUIRES_EXISTING_PROFILE');}
    if(process.env.DOC110_E2E_MISSING_SCHEMA==='YES'){
      const token=response.request().headers()['x-importacion-perfil-csrf'];
      if(!token)throw new Error('DOC110_CSRF_NOT_OBSERVED');
      await require('./support/doc110-missing-schema.cjs').exerciseMissingSchema(auth,process.env.DOC110_E2E_BASE_URL,token);
    }
    if(process.env.DOC110_E2E_ACCEPTANCE==='YES'){
      const token=response.request().headers()['x-importacion-perfil-csrf'];
      if(!token)throw new Error('DOC110_CSRF_NOT_OBSERVED');
      await require('./support/doc110-acceptance.cjs').exerciseAcceptance(auth,process.env.DOC110_E2E_BASE_URL,token);
    }
    if(process.env.DOC110_E2E_PERMISSION_REVOCATION==='YES'){
      const token=response.request().headers()['x-importacion-perfil-csrf'];
      if(!token)throw new Error('DOC110_CSRF_NOT_OBSERVED');
      await require('./support/doc110-permission-revocation.cjs').exercisePermissionRevocation(auth,process.env.DOC110_E2E_BASE_URL,token,sample);
    }
    if(process.env.DOC110_E2E_SESSION_EXPIRY==='YES'){
      const token=response.request().headers()['x-importacion-perfil-csrf'];
      if(!token)throw new Error('DOC110_CSRF_NOT_OBSERVED');
      await require('./support/doc110-session-expiry.cjs').exerciseSessionExpiry(auth,process.env.DOC110_E2E_BASE_URL,token,sample);
    }
    if(process.env.DOC110_E2E_SOURCE_TRANSACTIONS==='YES'){
      const token=response.request().headers()['x-importacion-perfil-csrf'];
      if(!token)throw new Error('DOC110_CSRF_NOT_OBSERVED');
      await require('./support/doc110-source-transactions.cjs').exerciseSourceTransactions(auth,process.env.DOC110_E2E_BASE_URL,token);
    }
    if(process.env.DOC110_E2E_SOURCE_GUARDS==='YES'){
      const token=response.request().headers()['x-importacion-perfil-csrf'];
      if(!token)throw new Error('DOC110_CSRF_NOT_OBSERVED');
      await require('./support/doc110-source-guards.cjs').exerciseSourceGuards(auth,process.env.DOC110_E2E_BASE_URL,token);
    }
    if(process.env.DOC110_E2E_PROVIDER_DIAGNOSTICS==='YES'){
      const token=response.request().headers()['x-importacion-perfil-csrf'];
      if(!token)throw new Error('DOC110_CSRF_NOT_OBSERVED');
      await require('./support/doc110-provider-source-diagnostics.cjs').exerciseProviderSourceDiagnostics(auth,process.env.DOC110_E2E_BASE_URL,token);
    }
    if(process.env.DOC110_E2E_SOURCE_PROFILE==='YES'){
      const token=response.request().headers()['x-importacion-perfil-csrf'];
      if(!token)throw new Error('DOC110_CSRF_NOT_OBSERVED');
      await require('./support/doc110-source-profile.cjs').exerciseSourceProfile(auth,process.env.DOC110_E2E_BASE_URL,token);
    }
    if(process.env.DOC110_E2E_PROVIDER_LIFECYCLE==='YES'){
      const token=response.request().headers()['x-importacion-perfil-csrf'];
      if(!token)throw new Error('DOC110_CSRF_NOT_OBSERVED');
      await require('./support/doc110-provider-lifecycle.cjs').exerciseProviderLifecycle(auth,process.env.DOC110_E2E_BASE_URL,token);
    }
    if(process.env.DOC110_E2E_UPDATEPANEL==='YES')
      await require('./support/doc110-updatepanel.cjs').exerciseUpdatePanel(auth.page);
    if(process.env.DOC110_E2E_DRAFT_LIFECYCLE==='YES' || process.env.DOC110_E2E_PUBLICATION_LIFECYCLE==='YES'){
      const token=response.request().headers()['x-importacion-perfil-csrf'];
      if(!token)throw new Error('DOC110_CSRF_NOT_OBSERVED');
      let contender;
      try{
        if(process.env.DOC110_E2E_INDEPENDENT_SESSIONS==='YES'){
          contender=await createWorkflowLoginPageSession(browser,{
            baseUrl:process.env.DOC110_E2E_BASE_URL,
            moduleEnvironmentVariable:'DOC110_E2E_MODULE',
            userEnvironmentVariable:'DOC110_E2E_USER',
            passwordEnvironmentVariable:'DOC110_E2E_PASSWORD',
            ignoreHTTPSErrors:process.env.DOC10_E2E_IGNORE_HTTPS_ERRORS==='true'
          });
          await contender.page.goto(new URL('Gestion/WebFormProducionDocumental.aspx',process.env.DOC110_E2E_BASE_URL).href);
          await contender.page.locator('#doc110-navigation-toggle').click();
          const listed=contender.page.waitForResponse(r=>r.url().endsWith('/ListarPerfiles') && r.request().method()==='POST');
          await contender.page.locator('[data-doc110=abrir]').click();
          const secondResponse=await listed;
          expect(secondResponse.status()).toBe(200);
          expect((await secondResponse.json()).d?.ok).toBe(true);
          contender.token=secondResponse.request().headers()['x-importacion-perfil-csrf'];
          if(!contender.token)throw new Error('DOC110_CSRF_NOT_OBSERVED');
          const sessionValue=async context=>(await context.cookies(process.env.DOC110_E2E_BASE_URL)).find(c=>c.name==='ASP.NET_SessionId')?.value;
          const firstSession=await sessionValue(auth.context),secondSession=await sessionValue(contender.context);
          // Comparar solo el booleano: nunca incluir cookies en un error de expect.
          expect(Boolean(firstSession && secondSession && firstSession!==secondSession)).toBe(true);
          console.log('DOC110_PASS: INDEPENDENT_ASPNET_SESSIONS');
        }
        if(process.env.DOC110_E2E_DRAFT_LIFECYCLE==='YES')
          await require('./support/doc110-draft-lifecycle.cjs').exerciseDraft(auth,process.env.DOC110_E2E_BASE_URL,token,contender);
        else
          await require('./support/doc110-publication-lifecycle.cjs').exercisePublication(auth,process.env.DOC110_E2E_BASE_URL,token,contender);
      }finally{if(contender)await contender.context.close();}
    }
  }finally{await auth.context.close();}
});
