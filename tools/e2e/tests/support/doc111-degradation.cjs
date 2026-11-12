'use strict';
const fs=require('node:fs'),path=require('node:path');
const {expect}=require('@playwright/test');
const {safePost}=require('./doc110-safe-request.cjs');
const {verifyMissingSchema}=require('./doc110-missing-schema.cjs');
const {startDatabaseBridge}=require('./doc110-permission-revocation.cjs');
const {assertAuthorizations,createSelectControls}=require('./doc111-importacion-carpetas.cjs');
function check(value,code){if(!value)throw new Error('DOC111_'+code);}
async function checkUnavailableUI(page){
  if(!await page.locator('[data-doc111-open]').isVisible()&&await page.locator('#doc110-navigation-toggle').count())await page.locator('#doc110-navigation-toggle').click();
  const pending=page.waitForResponse(r=>r.url().endsWith('/AbrirImportacion')&&r.request().method()==='POST',{timeout:30000});
  pending.catch(()=>{});
  await page.locator('[data-doc111-open]').click();check((await pending).status()===503,'DEGRADATION_NOT_ENFORCED');
  await expect(page.locator('[data-doc111-status]')).toContainText('La importación no está disponible con la configuración actual.');
}
function validateDegradationProfile(p,profile){
  const validated=require('../../scripts/support/workflow-e2e-platform-profile.cjs').validateProfile(p);
  check(profile.mode==='LOTE'&&validated.baseUrl===profile.baseUrl&&validated.module===profile.module&&validated.scenarioId==='production-document-upload-execution'&&validated.expedientId===467&&validated.documentTypeId===50&&validated.fixturePath==='doc111-carga-individual.pdf'&&validated.odbcDsn==='MembershipUsers'&&profile.nivelRef==='7','DEGRADATION_SCOPE_INVALID');
  return validated;
}
async function runDegradation(auth,profile,uploadProfile,authorization,query){
  assertAuthorizations(authorization);validateDegradationProfile(uploadProfile,profile);
  const fixture=fs.readFileSync(path.resolve(__dirname,'../../fixtures',uploadProfile.fixturePath));
  check(fixture.length===677&&fixture.subarray(0,5).toString()==='%PDF-','DEGRADATION_SCOPE_INVALID');
  const controls=createSelectControls(query),lots=['2','3','4','5'];
  async function snapshots(){const result=[];for(const lot of lots)result.push(await controls.snapshot(lot));return JSON.stringify(result);}
  const unchanged=await snapshots();
  const inventorySql='SELECT ID_REGISTRO_PRODUCION_DOCUMENTAL,ID_DOCUMENTO_DOCUARCHI_ALMACEN,NOMBRE_GABINETE,ID_TIPO_DOCUMENTO,ID_EMPRESA_DOCUMENTO,ID_USUARIO_GESTION FROM registro_producion_documental WHERE EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE=? ORDER BY ID_REGISTRO_PRODUCION_DOCUMENTAL';
  const initial=await query('RA',inventorySql,[467]);
  // Selección real en el árbol existente antes de ocultar el esquema. Sin
  // manipular sesión ni fabricar un atributo data-nivel para habilitar el botón.
  const page=auth.page,production=new URL('Gestion/WebFormProducionDocumental.aspx',profile.baseUrl).href;
  await page.waitForFunction(level=>Array.isArray(window.ITEMS_DATOS)&&window.ITEMS_DATOS.some(item=>String(document.getElementById(item.id)?.title||'').split('\\')[0]===level),profile.nivelRef,{timeout:30000});
  const selected=page.waitForResponse(r=>r.request().method()==='POST'&&/\/Gestion\/WebFormProducionDocumental\.aspx(?:\?|$)/i.test(r.url()),{timeout:30000});
  check(await page.evaluate(level=>{
    const matches=window.ITEMS_DATOS.filter(item=>String(document.getElementById(item.id)?.title||'').split('\\')[0]===level);
    if(matches.length!==1||typeof window.OnSearchClick!=='function')return false;
    window.OnSearchClick(matches[0].id,'TreeViewArchivo');return true;
  },profile.nivelRef),'DEGRADATION_LEVEL_UNAVAILABLE');
  check((await selected).status()===200,'DEGRADATION_LEVEL_UNAVAILABLE');
  check((await page.goto(production,{waitUntil:'domcontentloaded'})).status()===200,'DEGRADATION_PRODUCTION_UNAVAILABLE');
  const host=page.locator('#doc111-importacion');
  check(await host.getAttribute('data-nivel')===profile.nivelRef,'DEGRADATION_LEVEL_UNAVAILABLE');
  const token=await host.getAttribute('data-token');check(Boolean(token),'TOKEN_UNAVAILABLE');
  async function opening(){
    const response=await safePost(auth.context.request,new URL('webservice/WebServiceImportacionCarpetas.asmx/AbrirImportacion',profile.baseUrl).href,{maxRedirects:0,headers:{'X-Importacion-Carpetas-CSRF':token,Origin:new URL(profile.baseUrl).origin},data:{solicitud:{nivelRef:profile.nivelRef}}},'AbrirImportacion');
    const raw=await response.json();return {status:response.status(),result:raw?.d??raw};
  }
  const available=await opening();check(available.status===200&&available.result.ok,'DEGRADATION_PRECHECK_FAILED');
  const bridge=startDatabaseBridge('doc110-profile-schema-db.ps1',{operationalSchema:true});
  const recordPath=path.resolve(__dirname,'../../artifacts/doc111-degradation-state.json');
  const state={status:'RUNNING',renameAttempted:false,restored:false,uploadEntered:false};
  const save=()=>fs.writeFileSync(recordPath,JSON.stringify(state,null,2)+'\n');save();
  let result;
  try{
    const ready=await bridge.ready();check(ready.ok&&ready.stage==='READY','DEGRADATION_BRIDGE_UNAVAILABLE');
    const guarded={send:async operation=>{
      if(operation==='LEGACY_COUNT'){
        if(state.renameAttempted&&!state.restored){
          const original=await query('RA','SELECT COUNT(*) AS total FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME=?',['ra_integracion_evento']);
          const hidden=await query('RA','SELECT COUNT(*) AS total FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME=?',['doc111_rec_evento_20261102']);
          check(original.length===1&&String(original[0].total)==='0'&&hidden.length===1&&String(hidden[0].total)==='1','DEGRADATION_NOT_ENFORCED');
        }
        return {ok:true,stage:'LEGACY_COUNT',count:(await query('RA',inventorySql,[467])).length};
      }
      if(operation==='HIDE'){state.renameAttempted=true;save();}
      const response=await bridge.send(operation);
      if(operation==='RESTORE'){state.restored=response.ok&&response.profileDataAndSchemaUnchanged===true;save();}
      return response;
    }};
    result=await verifyMissingSchema({bridge:guarded,expectedMissingTables:1,listProfiles:opening,
      async checkUnavailableUI(){
        state.stage='UNAVAILABLE_UI';save();
        await checkUnavailableUI(page);
      },
      async upload(){
        state.stage='INDIVIDUAL_UPLOAD';state.uploadEntered=true;save();
        return require('../../scripts/run-workflow-e2e-platform.cjs').inspectProductionDocumentUploadPreviewUi({context:auth.context,plan:{profile:uploadProfile,scenario:require('../../scripts/support/workflow-e2e-platform-registry.cjs').resolveScenario(uploadProfile.scenarioId)}});
      }
    });
  }catch{
    state.status='STOPPED_REVIEW_REQUIRED';save();throw new Error('DOC111_DEGRADATION_NOT_CONFIRMED');
  }finally{
    try{await bridge.close();}catch{state.status='RECOVERY_CHECK_REQUIRED';save();throw new Error('DOC111_DEGRADATION_RECOVERY_REQUIRED');}
  }
  check(state.restored&&result.documentDelta===1,'DEGRADATION_RECOVERY_REQUIRED');
  state.stage='FINAL_SELECT';save();
  check(await snapshots()===unchanged,'DEGRADATION_LOTS_CHANGED');
  const after=await query('RA',inventorySql,[467]);
  const ids=new Set(initial.map(r=>String(r.ID_REGISTRO_PRODUCION_DOCUMENTAL))),added=after.filter(r=>!ids.has(String(r.ID_REGISTRO_PRODUCION_DOCUMENTAL)));
  check(added.length===1&&after.length===initial.length+1,'DEGRADATION_DOCUMENT_INVALID');
  const row=added[0],inventoryId=String(row.ID_REGISTRO_PRODUCION_DOCUMENTAL),documentId=String(row.ID_DOCUMENTO_DOCUARCHI_ALMACEN);
  check(/^[1-9][0-9]*$/.test(documentId)&&String(row.ID_TIPO_DOCUMENTO)==='50'&&String(row.ID_EMPRESA_DOCUMENTO)==='2'&&String(row.ID_USUARIO_GESTION)==='141'&&/^[A-Za-z_][A-Za-z0-9_]{0,63}$/.test(row.NOMBRE_GABINETE),'DEGRADATION_DOCUMENT_INVALID');
  const physical=await query('DA','SELECT COUNT(*) AS total FROM `'+row.NOMBRE_GABINETE+'` WHERE ID=? AND ID_INVENTARIO_DOCUMENTAL=? AND ID_EXPEDIENTE=?',[documentId,inventoryId,467]);
  const incorporated=await query('RA','SELECT COUNT(*) AS total FROM ra_cert_indice_expediente WHERE registro_producion_documental_ID_REGISTRO_PRODUCION_DOCUMENTAL=? AND expediente_archivo_ID_EXPEDIENTE=?',[inventoryId,467]);
  check(physical.length===1&&String(physical[0].total)==='1'&&incorporated.length===1&&String(incorporated[0].total)==='1','DEGRADATION_DOCUMENT_INVALID');
  state.status='PASS';state.documentId=documentId;state.inventoryId=inventoryId;save();
  return {mode:'LOTE',expedienteId:'467',documentoId:documentId,inventarioId:inventoryId,unavailableVerified:true,individualUploadVerified:true,schemaRestored:true,lotsUnchanged:true,documents:1};
}
module.exports={validateDegradationProfile,runDegradation,checkUnavailableUI};
