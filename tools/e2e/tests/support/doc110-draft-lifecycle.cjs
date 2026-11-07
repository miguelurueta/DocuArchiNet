'use strict';
const {expect}=require('@playwright/test');
const crypto=require('node:crypto');
const fs=require('node:fs');
const path=require('node:path');
const {safePost}=require('./doc110-safe-request.cjs');

async function exerciseDraft(auth,baseUrl,token,contender){
  const evidence={checks:[],status:'RUNNING',startedUtc:new Date().toISOString(),code:require('./doc110-code-evidence.cjs').captureCodeEvidence(),sessionMode:contender?'INDEPENDENT_SESSIONS':'SINGLE_SESSION'};
  const report=path.resolve(__dirname,'../../artifacts/doc110-draft-result.json');
  fs.mkdirSync(path.dirname(report),{recursive:true});
  async function call(method,payload,expected=200){
    const response=await safePost(auth.context.request,new URL('webservice/WebServiceImportacionCarpetas.asmx/'+method,baseUrl).href,{
      headers:{'X-Importacion-Perfil-CSRF':token},data:{solicitud:payload}
    },method);
    let result;try{result=(await response.json()).d;}catch{throw new Error('DOC110_NON_JSON: '+method);}
    if(response.status()!==expected) {
      const safe=/^[A-Z_]+$/.test(result?.codigo||'')?result.codigo:'UNKNOWN';
      throw new Error('DOC110_HTTP: '+method+' status='+response.status()+' code='+safe);
    }
    expect(result?.ok).toBe(expected===200);
    return result;
  }
  function checked(name){evidence.checks.push(name);console.log('DOC110_PASS: '+name);}
  try{
    await auth.page.locator('[data-doc110=nuevo]').click();
    await auth.page.locator('[data-doc110=codigo]').fill('E2E-'+crypto.randomBytes(12).toString('hex'));
    await auth.page.locator('[data-doc110=nombre]').fill('Prueba descartable DOC-110');
    const response=auth.page.waitForResponse(r=>r.url().endsWith('/CrearPerfil'));
    await auth.page.locator('[data-doc110=crear]').click();
    const created=await response;
    expect(created.status()).toBe(200);
    const body=await created.json();expect(body.d?.ok).toBe(true);
    let p=body.d.datos;
    evidence.profileId=p.PerfilId;evidence.versionIds=[p.Version.VersionId];
    for(const id of [p.PerfilId,p.Version.VersionId]){
      expect(typeof id).toBe('string');
      expect(id).toMatch(/^[1-9][0-9]*$/);
      expect(Number(id)).toBeLessThanOrEqual(4294967295);
    }
    expect(p.Version.PerfilId).toBe(p.PerfilId);
    checked('INCREMENTAL_IDENTITIES');
    checked('CREATE_UI');
    const rule={DestinoTipo:'CAMPO_EXPEDIENTE',DestinoRef:'CODIGO',FuenteTipo:'CARPETA',
      EtapaResolucion:'PREFLIGHT',Obligatoria:true,Orden:0,Extraccion:'SEGMENTO',
      Separador:'-',Posicion:0,QuitarExtension:false,RecortarEspacios:false,
      TipoValor:'TEXTO',LongitudMaxima:45};
    function payload(profile,config,rules){return {perfilId:profile.PerfilId,versionId:profile.Version.VersionId,
      revisionVersionEsperada:profile.Version.Revision,configuracion:config,reglas:rules};}
    let request=payload(p,{...p.Version.Configuracion,NombreVisible:'Borrador sintético',
      ModoIntegridad:'COMPLETO_OBLIGATORIO',AmbitoIntegridad:'EXPEDIENTE'},[rule]);
    p=(await call('GuardarBorrador',request)).datos;
    for(const r of p.Version.Reglas){
      expect(r.ReglaId).toMatch(/^[1-9][0-9]*$/);
      expect(Number(r.ReglaId)).toBeLessThanOrEqual(4294967295);
    }
    checked('SAVE_DRAFT');
    let loaded=(await call('ObtenerPerfil',{perfilId:p.PerfilId,versionId:p.Version.VersionId})).datos;
    expect(loaded.Version.Configuracion.NombreVisible).toBe('Borrador sintético');
    expect(loaded.Version.Reglas[0].Separador).toBe('-');
    checked('RELOAD_PERSISTED');
    if(process.env.DOC110_E2E_TABS==='YES'){
      const secondTab=await auth.context.newPage();
      try{
        await secondTab.goto(new URL('Gestion/WebFormProducionDocumental.aspx',baseUrl).href);
        await secondTab.locator('#doc110-navigation-toggle').click();
        const pending=secondTab.waitForResponse(r=>r.url().endsWith('/ListarPerfiles') && r.request().method()==='POST');
        await secondTab.locator('[data-doc110=abrir]').click();
        const listed=await pending;expect(listed.status()).toBe(200);
        expect((await listed.json()).d?.ok).toBe(true);
        const tabToken=listed.request().headers()['x-importacion-perfil-csrf'];
        expect(Boolean(tabToken && tabToken===token)).toBe(true);
        checked('SECOND_TAB_PRESERVES_SESSION_CSRF');
        const previousRevision=p.Version.Revision;
        // El token de la primera pestaña debe seguir siendo válido después de abrir la segunda.
        p=(await call('GuardarBorrador',payload(p,{...p.Version.Configuracion,NombreVisible:'Dos pestañas DOC-110'},p.Version.Reglas))).datos;
        expect(p.Version.Revision).toBe(previousRevision+1);
        const read=await safePost(auth.context.request,new URL('webservice/WebServiceImportacionCarpetas.asmx/ObtenerPerfil',baseUrl).href,{
          headers:{'X-Importacion-Perfil-CSRF':tabToken},data:{solicitud:{perfilId:p.PerfilId,versionId:p.Version.VersionId}}
        },'ObtenerPerfil');
        expect(read.status()).toBe(200);
        const result=(await read.json()).d;expect(result?.ok).toBe(true);
        expect(JSON.stringify(result.datos)===JSON.stringify(p)).toBe(true);
        loaded=result.datos;
        checked('FIRST_TAB_TOKEN_WRITES_ONCE_SECOND_TAB_TOKEN_READS_RESULT');
        const close=secondTab.locator('#doc110-perfiles section[role=dialog]:not([hidden])');
        await expect(close).toBeVisible();
        await secondTab.keyboard.press('Escape');
        await expect(close).toHaveCount(0);
        await expect(secondTab.locator('#doc110-navigation-toggle')).toBeFocused();
        checked('SECOND_TAB_ESCAPE_RESTORES_MENU_FOCUS');
      }finally{await secondTab.close();}
    }
    if(process.env.DOC110_E2E_GUARDS==='YES'){
      if(!contender)throw new Error('DOC110_GUARDS_REQUIRE_SECOND_SESSION');
      expect(Boolean(token && contender.token && token!==contender.token)).toBe(true);
      const attempt=payload(p,{...p.Version.Configuracion,NombreVisible:'Cambio que debe rechazarse'},p.Version.Reglas);
      async function rejected(context,headers,status,code,name){
        const response=await safePost(context.request,new URL('webservice/WebServiceImportacionCarpetas.asmx/GuardarBorrador',baseUrl).href,
          {headers,data:{solicitud:attempt},maxRedirects:0},'GuardarBorrador');
        expect(response.status()).toBe(status);
        let result;try{result=(await response.json()).d;}catch{throw new Error('DOC110_GUARD_NON_JSON');}
        expect(result?.ok).toBe(false);expect(result?.codigo).toBe(code);expect(result?.datos==null).toBe(true);
        const after=(await call('ObtenerPerfil',{perfilId:p.PerfilId,versionId:p.Version.VersionId})).datos;
        expect(JSON.stringify(after)===JSON.stringify(loaded)).toBe(true);
        checked(name);
      }
      await rejected(auth.context,{},403,'SOLICITUD_NO_VERIFICADA','MISSING_CSRF_REJECTED_NO_CHANGE');
      await rejected(auth.context,{'X-Importacion-Perfil-CSRF':contender.token},403,'SOLICITUD_NO_VERIFICADA','OTHER_SESSION_CSRF_REJECTED_NO_CHANGE');
      const anonymous=await auth.context.browser().newContext({ignoreHTTPSErrors:process.env.DOC10_E2E_IGNORE_HTTPS_ERRORS==='true'});
      try{
        await rejected(anonymous,{'X-Importacion-Perfil-CSRF':token},401,'SESION_REQUERIDA','UNAUTHENTICATED_WRITE_REJECTED_NO_CHANGE');
      }finally{await anonymous.close();}
    }
    const stale=await call('GuardarBorrador',request,409);
    expect(stale.codigo).toBe('CONFLICTO_REVISION');
    checked('STALE_REVISION_REJECTED');
    for(const sample of [{separator:'-',name:'00125-Trabajo'},{separator:'_',name:'00125_Trabajo'}]){
      p=(await call('GuardarBorrador',payload(p,p.Version.Configuracion,[{...rule,Separador:sample.separator}]))).datos;
      const trial=await call('EnsayarReglas',{...payload(p,p.Version.Configuracion,p.Version.Reglas),
        ejemplos:[{NombreCarpeta:sample.name,NombreArchivo:'sintetico.pdf',Expediente:{}}]});
      expect(trial.datos[0].Incidencias.length).toBe(0);
      expect(trial.datos[0].Valores[0].Valor).toBe('00125');
    }
    checked('TWO_PATTERNS_KEEP_LEADING_ZEROES');
    const invalid=await call('PublicarVersion',{perfilId:p.PerfilId,versionId:p.Version.VersionId,
      revisionPerfilEsperada:p.Revision,revisionVersionEsperada:p.Version.Revision},400);
    expect(invalid.incidencias.length).toBeGreaterThan(0);
    loaded=(await call('ObtenerPerfil',{perfilId:p.PerfilId,versionId:p.Version.VersionId})).datos;
    expect(loaded.Version.Estado).toBe('BORRADOR');
    checked('INCOMPLETE_PUBLICATION_REJECTED');
    const base=payload(p,p.Version.Configuracion,p.Version.Reglas);
    const writers=[{context:auth.context,token},contender || {context:auth.context,token}];
    const outcomes=await Promise.all(['A','B'].map((name,index)=>safePost(writers[index].context.request,
      new URL('webservice/WebServiceImportacionCarpetas.asmx/GuardarBorrador',baseUrl).href,{
        headers:{'X-Importacion-Perfil-CSRF':writers[index].token},
        data:{solicitud:{...base,configuracion:{...base.configuracion,NombreVisible:'Carrera '+name}}}
      },'GuardarBorrador')));
    expect(outcomes.map(r=>r.status()).sort()).toEqual([200,409]);
    const winner=(await outcomes.find(r=>r.status()===200).json()).d.datos;
    loaded=(await call('ObtenerPerfil',{perfilId:p.PerfilId,versionId:p.Version.VersionId})).datos;
    expect(loaded.Version.Revision).toBe(p.Version.Revision+1);
    expect(loaded.Version.Configuracion.NombreVisible).toBe(winner.Version.Configuracion.NombreVisible);
    checked('CONCURRENT_REVISION_ONE_WINNER');
    if(contender){
      const observed=await safePost(contender.context.request,
        new URL('webservice/WebServiceImportacionCarpetas.asmx/ObtenerPerfil',baseUrl).href,{
          headers:{'X-Importacion-Perfil-CSRF':contender.token},
          data:{solicitud:{perfilId:p.PerfilId,versionId:p.Version.VersionId}}
        },'ObtenerPerfil');
      expect(observed.status()).toBe(200);
      const result=(await observed.json()).d;
      expect(result?.ok).toBe(true);
      expect(result.datos.Version.Revision).toBe(loaded.Version.Revision);
      expect(result.datos.Version.Configuracion.NombreVisible).toBe(winner.Version.Configuracion.NombreVisible);
      checked('INDEPENDENT_SESSIONS_ONE_WINNER_AND_SHARED_RESULT');
    }
    const history=await call('ConsultarHistorial',{perfilId:p.PerfilId,pagina:1,tamanoPagina:25});
    expect(history.datos.length).toBe(1);
    checked('HISTORY');
    const disabled=await call('CambiarDisponibilidad',{perfilId:p.PerfilId,revisionPerfilEsperada:p.Revision,activo:false});
    expect(disabled.datos.Activo).toBe(false);
    checked('DISABLE_DISPOSABLE_PROFILE');
    evidence.status='PASS';
    return {perfilId:p.PerfilId,versionIds:[p.Version.VersionId]};
  }catch(error){evidence.status='FAIL';throw error;}
  finally{
    evidence.finishedUtc=new Date().toISOString();
    fs.writeFileSync(report,JSON.stringify(evidence,null,2));
  }
}
module.exports={exerciseDraft};
