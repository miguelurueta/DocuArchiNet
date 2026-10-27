'use strict';
const {expect}=require('@playwright/test');
const crypto=require('node:crypto');
const fs=require('node:fs');
const path=require('node:path');
const {safePost}=require('./doc110-safe-request.cjs');
const {discoverFixture}=require('./doc110-publication-fixture.cjs');

async function exercisePublication(auth,baseUrl,token,contender,providerBinding){
  const evidence={status:'RUNNING',checks:[],startedUtc:new Date().toISOString(),code:require('./doc110-code-evidence.cjs').captureCodeEvidence(),sessionMode:contender?'INDEPENDENT_SESSIONS':'SINGLE_SESSION'};
  const report=path.resolve(__dirname,'../../artifacts/doc110-publication-result.json');
  async function call(method,payload,expected=200){
    const response=await safePost(auth.context.request,new URL('webservice/WebServiceImportacionCarpetas.asmx/'+method,baseUrl).href,
      {headers:{'X-Importacion-Perfil-CSRF':token},data:{solicitud:payload}},method);
    let result;try{result=(await response.json()).d;}catch{throw new Error('DOC110_NON_JSON: '+method);}
    if(response.status()!==expected){
      const code=/^[A-Z_]+$/.test(result?.codigo||'')?result.codigo:'UNKNOWN';
      const fields=(result?.incidencias||[]).map(i=>/^[A-Z_]+$/.test(i.Codigo||'')?i.Codigo:'INVALID').join(',');
      throw new Error('DOC110_HTTP: '+method+' status='+response.status()+' code='+code+' validations='+fields);
    }
    expect(result?.ok).toBe(expected===200);return result;
  }
  function checked(name){evidence.checks.push(name);console.log('DOC110_PASS: '+name);}
  function payload(p){return {perfilId:p.PerfilId,versionId:p.Version.VersionId,revisionVersionEsperada:p.Version.Revision,
    configuracion:p.Version.Configuracion,reglas:p.Version.Reglas};}
  async function configureUI(f){
    const field=key=>auth.page.locator('[data-doc110-field="'+key+'"]');
    async function choose(key,value){
      if(value===null || value===undefined || value==='')return;
      for(let page=0;page<10;page++){
        await expect(auth.page.locator('[data-doc110-catalog="'+key+'"]')).toHaveAttribute('data-state','READY');
        const exists=await field(key).locator('option').evaluateAll((items,id)=>items.some(o=>o.value===id),String(value));
        if(exists){await field(key).selectOption(String(value));return;}
        const more=auth.page.locator('[data-doc110="mas-'+key+'"]');await expect(more).toBeVisible();await more.click();
      }
      throw new Error('DOC110_UI_CATALOG_PAGE_LIMIT');
    }
    for(const key of ['FondoRef','InstrumentoRef','AreaRef','SerieRef','SubserieRef','TipoExpedienteRef','UnidadDocumentalRef','GabineteId'])await choose(key,f.configuracion[key]);
    for(const key of ['ModoIntegridad','AmbitoIntegridad','EstrategiaCarpetas','PoliticaSubcarpetas','PoliticaCarpetasVacias','PoliticaArchivosSueltos','PoliticaCoincidencias'])await field(key).selectOption(f.configuracion[key]);
    await field('NombreVisible').fill(f.configuracion.NombreVisible);
    await field('Extensiones').fill(f.configuracion.Extensiones.join(','));
    await field('MaximoBytesArchivo').fill(String(f.configuracion.MaximoBytesArchivo));
    await expect(auth.page.locator('[data-doc110-catalog=INDICE]')).toHaveAttribute('data-state','READY');
    for(const rule of f.reglas){
      await auth.page.locator('[data-doc110=agregar-regla]').click();
      const row=auth.page.locator('tr').filter({has:auth.page.locator('[data-doc110-rule=destino]')}).last();
      const r=key=>row.locator('[data-doc110-rule='+key+']');
      await r('destino').selectOption(rule.DestinoTipo);await r('campo').selectOption(rule.DestinoRef);
      await r('fuente').selectOption(rule.FuenteTipo);await r('tipo').selectOption(rule.TipoValor);
      if(rule.FuenteTipo==='CONSTANTE')await r('constante').fill(rule.ValorConstante);
      if(rule.FuenteTipo==='EXPEDIENTE')await r('propiedad').selectOption(rule.PropiedadExpediente);
      if(rule.FuenteTipo==='PROVEEDOR'){
        const selection=rule.Proveedor.ProveedorId+':'+rule.Proveedor.VersionId;
        let found=false;
        for(let attempt=0;attempt<20;attempt++){
          await expect(row.getByText('Consultando proveedores…',{exact:true})).toHaveCount(0);
          if(await r('proveedor').locator('option').evaluateAll((items,value)=>items.some(x=>x.value===value),selection)){found=true;break;}
          const more=row.getByRole('button',{name:'Más proveedores',exact:true});
          if(!await more.isVisible())break;
          await more.click();
        }
        if(!found)throw new Error('DOC110_PUBLISHED_PROVIDER_NOT_IN_CATALOG');
        await r('proveedor').selectOption(selection);
        await r('proveedorPropiedad').selectOption(rule.Proveedor.Propiedad);
        await r('claveFuente').selectOption('CONSTANTE');
        await r('claveConstante').fill(rule.Proveedor.Clave.ValorConstante);
        await r('claveLongitud').fill('250');
      }
      if(rule.TipoValor==='FECHA')await r('formato').selectOption(rule.FormatoFecha);
      await r('longitud').fill(String(rule.LongitudMaxima));
    }
    await expect(auth.page.locator('[data-doc110=publicar]')).toBeDisabled();
    const saved=auth.page.waitForResponse(r=>r.url().endsWith('/GuardarBorrador'));
    await auth.page.locator('[data-doc110=guardar]').click();
    const response=await saved;expect(response.status()).toBe(200);
    const result=(await response.json()).d;expect(result.ok).toBe(true);
    return result.datos;
  }
  try{
    const f=await discoverFixture(call);
    let providerRule;
    if(providerBinding){
      providerRule=f.reglas.find(r=>r.DestinoTipo==='INDICE_GABINETE' && r.TipoValor==='TEXTO' && r.LongitudMaxima>=providerBinding.key.length);
      if(!providerRule)throw new Error('DOC110_FIXTURE_NO_TEXT_INDEX_FOR_PROVIDER');
      providerRule.FuenteTipo='PROVEEDOR';delete providerRule.ValorConstante;
      providerRule.Proveedor={ProveedorId:providerBinding.provider.ProveedorId,VersionId:providerBinding.provider.VersionId,
        Propiedad:'numero_orden',Clave:{FuenteTipo:'CONSTANTE',ValorConstante:providerBinding.key,Extraccion:'COMPLETO',Posicion:0,
          QuitarExtension:false,RecortarEspacios:false,TipoValor:'TEXTO',LongitudMaxima:250}};
    }
    let inherited;
    if(process.env.DOC110_E2E_INHERITANCE==='YES'){
      inherited=f.reglas.find(r=>r.DestinoTipo==='INDICE_GABINETE' && r.TipoValor==='TEXTO' && r.LongitudMaxima>=String(f.configuracion.SerieRef).length);
      if(!inherited)throw new Error('DOC110_FIXTURE_NO_TEXT_INDEX_FOR_INHERITANCE');
      inherited.FuenteTipo='EXPEDIENTE';inherited.EtapaResolucion='TRAS_EXPEDIENTE';inherited.PropiedadExpediente='SERIE_ID';
      delete inherited.ValorConstante;
    }
    checked('AUTHORIZED_RA_DA_CATALOGS');
    await auth.page.locator('[data-doc110=nuevo]').click();
    await auth.page.locator('[data-doc110=codigo]').fill('E2E-PUB-'+crypto.randomBytes(12).toString('hex'));
    await auth.page.locator('[data-doc110=nombre]').fill('Publicación descartable DOC-110');
    const pending=auth.page.waitForResponse(r=>r.url().endsWith('/CrearPerfil'));
    await auth.page.locator('[data-doc110=crear]').click();
    const response=await pending;expect(response.status()).toBe(200);
    const body=await response.json();expect(body.d?.ok).toBe(true);
    let p=body.d.datos;
    evidence.profileId=p.PerfilId;evidence.versionIds=[p.Version.VersionId];
    for(const id of [p.PerfilId,p.Version.VersionId]){
      expect(typeof id).toBe('string');
      expect(id).toMatch(/^[1-9][0-9]*$/);
      expect(Number(id)).toBeLessThanOrEqual(4294967295);
    }
    expect(p.Version.PerfilId).toBe(p.PerfilId);
    checked('INCREMENTAL_IDENTITIES');
    if(process.env.DOC110_E2E_MATCHING==='YES'){
      expect(p.Version.Configuracion.PoliticaCoincidencias).toBe('BLOQUEAR');
      await expect(auth.page.locator('[data-doc110-field=PoliticaCoincidencias]')).toHaveValue('BLOQUEAR');
      const reloaded=(await call('ObtenerPerfil',{perfilId:p.PerfilId,versionId:p.Version.VersionId})).datos;
      expect(reloaded.Version.Configuracion.PoliticaCoincidencias).toBe('BLOQUEAR');
      checked('MATCHING_DEFAULT_BLOCK_PERSISTED');
      // configureUI realiza la seleccion explicita; no se cambia el valor por defecto del producto.
      f.configuracion.PoliticaCoincidencias='REUTILIZAR_VALIDADO';
    }
    p=await configureUI(f);
    checked('GUIDED_CONFIGURATION_AND_SAVE_UI');
    if(process.env.DOC110_E2E_ACCEPTANCE==='YES'){
      const scope=auth.page.locator('[data-doc110-field=AmbitoIntegridad]');
      async function saveScope(value){
        await scope.selectOption(value);
        const pending=auth.page.waitForResponse(r=>r.url().endsWith('/GuardarBorrador')&&r.request().method()==='POST');
        await auth.page.locator('[data-doc110=guardar]').click();
        const response=await pending;expect(response.status()).toBe(200);
        const result=(await response.json()).d;expect(result?.ok).toBe(true);return result.datos;
      }
      p=await saveScope('');
      const missing=JSON.stringify(p);
      const denied=await call('PublicarVersion',{perfilId:p.PerfilId,versionId:p.Version.VersionId,
        revisionPerfilEsperada:p.Revision,revisionVersionEsperada:p.Version.Revision},400);
      expect(denied.codigo).toBe('CONFIGURACION_INVALIDA');
      expect(denied.incidencias.some(i=>i.Campo==='AmbitoIntegridad'&&i.Codigo==='AMBITO_REQUERIDO')).toBe(true);
      const after=(await call('ObtenerPerfil',{perfilId:p.PerfilId,versionId:p.Version.VersionId})).datos;
      expect(JSON.stringify(after)===missing).toBe(true);
      checked('PER01_COMPLETE_WITHOUT_SCOPE_REJECTED_PROFILE_UNCHANGED');
      p=await saveScope('EXPEDIENTE');
    }
    if(providerBinding?.draft)
      await require('./doc110-provider-draft-consumer.cjs').verifyDraftConsumer(call,p,providerBinding.draft);
    if(providerBinding)
      await require('./doc110-provider-revalidation.cjs').verifyProviderRevalidation(call,p,providerBinding.provider);
    if(process.env.DOC110_E2E_MATCHING==='YES'){
      const before=(await call('ObtenerPerfil',{perfilId:p.PerfilId,versionId:p.Version.VersionId})).datos;
      expect(before.Version.Configuracion.PoliticaCoincidencias).toBe('REUTILIZAR_VALIDADO');
      checked('MATCHING_EXPLICIT_REUSE_UI_PERSISTED');
      const invalid=await call('GuardarBorrador',{...payload(p),configuracion:{...p.Version.Configuracion,PoliticaCoincidencias:'DOC110_INVALID'}},400);
      expect(invalid.codigo).toBe('CONFIGURACION_INVALIDA');
      expect(invalid.incidencias.some(i=>i.Campo==='PoliticaCoincidencias' && i.Codigo==='POLITICA_INVALIDA')).toBe(true);
      // PublicarVersion solo admite referencias/revisiones; no acepta una configuracion inyectada.
      const publish=await call('PublicarVersion',{perfilId:p.PerfilId,versionId:p.Version.VersionId,
        revisionPerfilEsperada:p.Revision,revisionVersionEsperada:p.Version.Revision,
        configuracion:{PoliticaCoincidencias:'DOC110_INVALID'}},400);
      expect(publish.codigo).toBe('CONFIGURACION_INVALIDA');
      const after=(await call('ObtenerPerfil',{perfilId:p.PerfilId,versionId:p.Version.VersionId})).datos;
      expect(after.Version.Estado).toBe('BORRADOR');
      expect(JSON.stringify(before)===JSON.stringify(after)).toBe(true);
      checked('MATCHING_INVALID_REJECTED_WITHOUT_PERSISTENCE_OR_PUBLICATION');
    }
    if(inherited){
      const persisted=(await call('ObtenerPerfil',{perfilId:p.PerfilId,versionId:p.Version.VersionId})).datos;
      const saved=persisted.Version.Reglas.find(r=>r.DestinoTipo==='INDICE_GABINETE' && r.DestinoRef===inherited.DestinoRef);
      expect(saved.FuenteTipo).toBe('EXPEDIENTE');expect(saved.PropiedadExpediente).toBe('SERIE_ID');
      expect(saved.EtapaResolucion).toBe('TRAS_EXPEDIENTE');
      checked('INHERITED_SERIES_SAVED_UI_AND_RELOADED');
      const example={NombreCarpeta:'00125',NombreArchivo:'sintetico.pdf',Expediente:{}};
      // El contrato HTTP cerrado rechaza estas claves antes del servicio.
      // Su sustitución defensiva en el servicio se verifica en el arnés .NET.
      const forged=await call('EnsayarReglas',{...payload(p),reglas:[saved],ejemplos:[{
        ...example,Expediente:{SERIE_ID:'DOC110_FORGED',SUBSERIE_ID:'DOC110_FORGED'}
      }]},400);
      expect(forged.codigo).toBe('CONFIGURACION_INVALIDA');
      checked('INHERITED_CLIENT_CLASSIFICATION_REJECTED_BY_HTTP_CONTRACT');
      async function trial(config,rule){
        return (await call('EnsayarReglas',{...payload(p),configuracion:config,reglas:[rule],ejemplos:[example]})).datos[0];
      }
      const resolved=await trial(p.Version.Configuracion,saved);
      expect(resolved.Incidencias.length).toBe(0);
      const value=resolved.Valores.find(v=>v.DestinoTipo==='INDICE_GABINETE' && v.DestinoRef===saved.DestinoRef);
      expect(String(value.Valor)===String(p.Version.Configuracion.SerieRef)).toBe(true);
      expect(value.Diferido).toBe(false);expect(value.Procedencia).toBe('PROPUESTA_PERFIL');
      checked('INHERITED_SERIES_RESOLVED_FROM_SERVER');
      const pending=await trial({...p.Version.Configuracion,SerieRef:null,SubserieRef:null},saved);
      expect(pending.Incidencias.length).toBe(0);
      expect(pending.Valores.some(v=>v.DestinoRef===saved.DestinoRef && v.Diferido===true && v.Procedencia==='PROPUESTA_PERFIL')).toBe(true);
      checked('INHERITED_CLASSIFICATION_PENDING');
      const absent=await trial({...p.Version.Configuracion,SubserieRef:null},{...saved,PropiedadExpediente:'SUBSERIE_ID',Obligatoria:true});
      expect(absent.Incidencias.some(i=>i.Codigo==='VALOR_REQUERIDO')).toBe(true);
      checked('INHERITED_REQUIRED_SUBSERIES_ABSENT_REJECTED');
      const after=(await call('ObtenerPerfil',{perfilId:p.PerfilId,versionId:p.Version.VersionId})).datos;
      expect(JSON.stringify(after)===JSON.stringify(persisted)).toBe(true);
      checked('INHERITANCE_TRIALS_DO_NOT_CHANGE_SAVED_PROFILE');
    }
    expect(p.Version.Configuracion.OrigenGabinete).toBe('DA');
    expect(/^[a-f0-9]{64}$/.test(p.Version.Configuracion.HuellaOrigenGabinete)).toBe(true);
    const trial=await call('EnsayarReglas',{...payload(p),ejemplos:[{NombreCarpeta:'00125',NombreArchivo:'sintetico.pdf',Expediente:{}}]});
    expect(trial.datos[0].Incidencias.length).toBe(0);
    checked('VALID_CONFIGURATION_AND_INDEX_VALUES');
    if(providerBinding){
      const value=trial.datos[0].Valores.find(v=>v.DestinoTipo==='INDICE_GABINETE' && v.DestinoRef===providerRule.DestinoRef);
      expect(Boolean(value && value.Valor===providerBinding.key && !value.Diferido)).toBe(true);
      const reread=(await call('ObtenerPerfil',{perfilId:p.PerfilId,versionId:p.Version.VersionId})).datos;
      const rule=reread.Version.Reglas.find(r=>r.DestinoTipo==='INDICE_GABINETE' && r.DestinoRef===providerRule.DestinoRef);
      expect(Boolean(rule && rule.FuenteTipo==='PROVEEDOR' && rule.Proveedor.ProveedorId===providerBinding.provider.ProveedorId &&
        rule.Proveedor.VersionId===providerBinding.provider.VersionId && rule.Proveedor.Propiedad==='numero_orden')).toBe(true);
      checked('PUBLISHED_PROVIDER_SELECTED_UI_PERSISTED_AND_REAL_INDEX_RESOLVED');
    }

    const publication=auth.page.waitForResponse(r=>r.url().endsWith('/PublicarVersion'));
    await auth.page.locator('[data-doc110=publicar]').click();
    const published=await publication;expect(published.status()).toBe(200);
    const publishedBody=await published.json();expect(publishedBody.d?.ok).toBe(true);p=publishedBody.d.datos;
    expect(p.Version.Estado).toBe('PUBLICADA');
    expect(p.Version.Configuracion.AmbitoIntegridad).toBe('EXPEDIENTE');
    await expect(auth.page.locator('[data-doc110=guardar]')).toHaveCount(0);
    checked('PUBLISH_EXPEDIENTE_UI');
    const firstId=p.Version.VersionId;
    const original=(await call('ObtenerPerfil',{perfilId:p.PerfilId,versionId:firstId})).datos.Version;
    await call('GuardarBorrador',payload(p),409);
    checked('PUBLISHED_EDIT_REJECTED');
    const createVersion={perfilId:p.PerfilId,versionOrigenId:firstId,revisionPerfilEsperada:p.Revision,revisionOrigenEsperada:p.Version.Revision};
    let second;
    if(contender){
      const outcomes=await Promise.all([{context:auth.context,token},contender].map(session=>safePost(session.context.request,
        new URL('webservice/WebServiceImportacionCarpetas.asmx/CrearVersion',baseUrl).href,{
          headers:{'X-Importacion-Perfil-CSRF':session.token},data:{solicitud:createVersion}
        },'CrearVersion')));
      expect(outcomes.map(r=>r.status()).sort()).toEqual([200,409]);
      const winner=(await outcomes.find(r=>r.status()===200).json()).d;
      const loser=(await outcomes.find(r=>r.status()===409).json()).d;
      expect(winner?.ok).toBe(true);expect(loser?.ok).toBe(false);
      expect(loser.codigo).toBe('CONFLICTO_REVISION');
      second=winner.datos;
      expect(second.Revision).toBe(p.Revision+1);
      const history=(await call('ConsultarHistorial',{perfilId:p.PerfilId,pagina:1,tamanoPagina:25})).datos;
      expect(history.length).toBe(2);
      expect(new Set(history.map(v=>v.Numero)).size).toBe(2);
      expect(new Set(history.map(v=>v.VersionId)).size).toBe(2);
      const read=await safePost(contender.context.request,
        new URL('webservice/WebServiceImportacionCarpetas.asmx/ObtenerPerfil',baseUrl).href,{
          headers:{'X-Importacion-Perfil-CSRF':contender.token},
          data:{solicitud:{perfilId:p.PerfilId,versionId:second.Version.VersionId}}
        },'ObtenerPerfil');
      expect(read.status()).toBe(200);
      const shared=(await read.json()).d;expect(shared?.ok).toBe(true);
      expect(shared.datos.Version.VersionId===second.Version.VersionId && shared.datos.Version.Numero===second.Version.Numero).toBe(true);
      checked('CONCURRENT_VERSION_ONE_WINNER_NO_DUPLICATE_NUMBER');
    }else second=(await call('CrearVersion',createVersion)).datos;
    expect(second.Version.VersionId!==firstId).toBe(true);
    expect(second.Version.Numero).toBe(original.Numero+1);
    expect(second.Version.Estado).toBe('BORRADOR');
    second=(await call('GuardarBorrador',{...payload(second),configuracion:{...second.Version.Configuracion,AmbitoIntegridad:'LOTE',
      ...(process.env.DOC110_E2E_MATCHING==='YES'?{PoliticaCoincidencias:'BLOQUEAR'}:{})}})).datos;
    const publicationRequest={perfilId:second.PerfilId,versionId:second.Version.VersionId,
      revisionPerfilEsperada:second.Revision,revisionVersionEsperada:second.Version.Revision};
    if(contender){
      const outcomes=await Promise.all([{context:auth.context,token},contender].map(session=>safePost(session.context.request,
        new URL('webservice/WebServiceImportacionCarpetas.asmx/PublicarVersion',baseUrl).href,{
          headers:{'X-Importacion-Perfil-CSRF':session.token},data:{solicitud:publicationRequest}
        },'PublicarVersion')));
      expect(outcomes.map(r=>r.status()).sort()).toEqual([200,409]);
      const winner=(await outcomes.find(r=>r.status()===200).json()).d;
      const loser=(await outcomes.find(r=>r.status()===409).json()).d;
      expect(winner?.ok).toBe(true);expect(loser?.ok).toBe(false);
      expect(loser.codigo).toBe('CONFLICTO_REVISION');expect(loser.datos==null).toBe(true);
      expect(winner.datos.Revision).toBe(second.Revision+1);
      expect(winner.datos.Version.Revision).toBe(second.Version.Revision+1);
      expect(JSON.stringify(winner.datos.Version.Configuracion)===JSON.stringify(second.Version.Configuracion)).toBe(true);
      second=winner.datos;
      for(const session of [{context:auth.context,token},contender]){
        const response=await safePost(session.context.request,
          new URL('webservice/WebServiceImportacionCarpetas.asmx/ObtenerPerfil',baseUrl).href,{
            headers:{'X-Importacion-Perfil-CSRF':session.token},
            data:{solicitud:{perfilId:second.PerfilId,versionId:second.Version.VersionId}}
          },'ObtenerPerfil');
        expect(response.status()).toBe(200);
        const result=(await response.json()).d;expect(result?.ok).toBe(true);
        expect(JSON.stringify(result.datos)===JSON.stringify(second)).toBe(true);
      }
      checked('CONCURRENT_PUBLICATION_ONE_WINNER_SINGLE_REVISION_INCREMENT');
      checked('CONCURRENT_PUBLICATION_SHARED_PERSISTED_RESULT');
    }else second=(await call('PublicarVersion',publicationRequest)).datos;
    expect(second.Version.Estado).toBe('PUBLICADA');
    expect(second.VersionPublicadaId).toBe(second.Version.VersionId);
    expect(second.Version.Configuracion.AmbitoIntegridad).toBe('LOTE');
    checked('SECOND_VERSION_PUBLISHED_LOTE');
    evidence.versionIds=[firstId,second.Version.VersionId];
    const old=(await call('ObtenerPerfil',{perfilId:p.PerfilId,versionId:firstId})).datos.Version;
    // Boolean comparison keeps business references out of failed assertion output.
    expect(JSON.stringify(old)===JSON.stringify(original)).toBe(true);
    checked('FIRST_PUBLICATION_IMMUTABLE');
    if(process.env.DOC110_E2E_MATCHING==='YES'){
      expect(old.Configuracion.PoliticaCoincidencias).toBe('REUTILIZAR_VALIDADO');
      expect(second.Version.Configuracion.PoliticaCoincidencias).toBe('BLOQUEAR');
      checked('MATCHING_POLICY_VERSIONED_WITHOUT_CHANGING_PRIOR_PUBLICATION');
    }
    const history=(await call('ConsultarHistorial',{perfilId:p.PerfilId,pagina:1,tamanoPagina:25})).datos;
    expect(history.length).toBe(2);expect(history.every(v=>v.Estado==='PUBLICADA')).toBe(true);
    checked('TWO_PUBLISHED_VERSIONS_IN_HISTORY');
    const disabled=(await call('CambiarDisponibilidad',{perfilId:p.PerfilId,revisionPerfilEsperada:second.Revision,activo:false})).datos;
    expect(disabled.Activo).toBe(false);
    checked('DISABLE_DISPOSABLE_PROFILE');
    evidence.status='PASS';
    if(providerBinding){
      const documents=[];
      for(const versionId of [firstId,second.Version.VersionId])
        documents.push((await call('ObtenerPerfil',{perfilId:p.PerfilId,versionId})).datos);
      const retainedHistory=(await call('ConsultarHistorial',{perfilId:p.PerfilId,pagina:1,tamanoPagina:25})).datos;
      return {documents,history:retainedHistory};
    }
    return {perfilId:p.PerfilId,versionIds:evidence.versionIds};
  }catch(error){evidence.status='FAIL';throw error;}
  finally{
    evidence.finishedUtc=new Date().toISOString();fs.mkdirSync(path.dirname(report),{recursive:true});
    fs.writeFileSync(report,JSON.stringify(evidence,null,2));
  }
}
module.exports={exercisePublication};
