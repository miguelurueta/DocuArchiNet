'use strict';
const {expect}=require('@playwright/test');

async function verifyMissingSchema({bridge,listProfiles,checkUnavailableUI,upload}){
  const before=await bridge.send('LEGACY_COUNT');
  expect(before.ok&&before.stage==='LEGACY_COUNT').toBe(true);
  let restore=false,result;
  try{
    restore=true;
    const hidden=await bridge.send('HIDE');expect(hidden.ok&&hidden.stage==='HIDDEN'&&hidden.missingOriginalTables===3).toBe(true);
    const response=await listProfiles();
    expect(response.status).toBe(503);expect(response.result?.ok).toBe(false);
    expect(response.result?.codigo).toBe('CAPACIDAD_NO_DISPONIBLE');expect(response.result?.datos==null).toBe(true);
    await checkUnavailableUI();
    console.log('DOC110_PASS: MISSING_PROFILE_TABLES_ADMIN_HTTP_503_AND_UI_NOTICE');
    result=await upload();
    expect(result.count===1&&result.codes?.storage==='CONFIRMED'&&result.codes?.projection==='CONFIRMED').toBe(true);
    const after=await bridge.send('LEGACY_COUNT');
    expect(after.ok&&after.count===before.count+1).toBe(true);
    console.log('DOC110_PASS: SINGLE_DOCUMENT_STORED_WITHOUT_PROFILE_SCHEMA_SELECT_CONFIRMED');
  }finally{
    if(restore){
      const restored=await bridge.send('RESTORE');
      expect(restored.ok&&restored.stage==='RESTORED'&&restored.profileDataAndSchemaUnchanged).toBe(true);
      console.log('DOC110_PASS: PROFILE_TABLE_NAMES_DATA_SCHEMA_AND_IDENTITIES_RESTORED');
    }
  }
  const available=await listProfiles();expect(available.status).toBe(200);expect(available.result?.ok).toBe(true);
  return {upload:result,documentDelta:1};
}

async function exerciseMissingSchema(auth,baseUrl,token){
  if(process.env.DOC110_E2E_USER?.toUpperCase()!=='LUZ.AGUILERA')throw new Error('DOC110_MISSING_SCHEMA_AUTHORIZED_ACCOUNT_REQUIRED');
  const fs=require('node:fs'),path=require('node:path');
  const {safePost}=require('./doc110-safe-request.cjs');
  const {startDatabaseBridge}=require('./doc110-permission-revocation.cjs');
  const {openImportAdministrator}=require('./doc110-admin-navigation.cjs');
  const {validateProfile}=require('../../scripts/support/workflow-e2e-platform-profile.cjs');
  const {resolveScenario}=require('../../scripts/support/workflow-e2e-platform-registry.cjs');
  const {inspectProductionDocumentUploadPreviewUi}=require('../../scripts/run-workflow-e2e-platform.cjs');
  const profile=validateProfile(JSON.parse(fs.readFileSync(path.resolve(__dirname,'../../profiles/doc110-production-upload-no-schema.local.runtime.json'),'utf8')));
  if(profile.expedientId!==467||profile.baseUrl!==baseUrl||profile.scenarioId!=='production-document-upload-execution')throw new Error('DOC110_MISSING_SCHEMA_FIXTURE_MISMATCH');
  const plan={profile,scenario:resolveScenario(profile.scenarioId)};
  const evidence={status:'RUNNING',startedUtc:new Date().toISOString(),fixture:profile.fixturePath,expedientId:467};
  const bridge=startDatabaseBridge('doc110-profile-schema-db.ps1');
  const send=bridge.send.bind(bridge);
  bridge.send=async operation=>{if(operation==='HIDE')evidence.renameAttempted=true;return send(operation);};
  try{
    const ready=await bridge.ready();
    if(!ready.ok||ready.stage!=='READY'){
      evidence.preflight={code:/^[A-Z_]+$/.test(ready.code||'')?ready.code:'UNKNOWN',
        queryKind:/^[A-Z_]+$/.test(ready.queryKind||'')?ready.queryKind:'UNKNOWN',
        errorType:/^[A-Za-z]+$/.test(ready.errorType||'')?ready.errorType:'UNKNOWN',
        mysqlNumber:Number.isInteger(ready.mysqlNumber)?ready.mysqlNumber:null};
      throw new Error('DOC110_MISSING_SCHEMA_PREFLIGHT_FAILED: '+evidence.preflight.code+'; '+evidence.preflight.queryKind);
    }
    evidence.preflight={status:'READY',slowestQuery:ready.slowestQuery};
    const result=await verifyMissingSchema({bridge,
      async listProfiles(){
        const response=await safePost(auth.context.request,new URL('webservice/WebServiceImportacionCarpetas.asmx/ListarPerfiles',baseUrl).href,
          {headers:{'X-Importacion-Perfil-CSRF':token},data:{solicitud:{pagina:1,tamanoPagina:1}}},'ListarPerfiles');
        let result;try{result=(await response.json()).d;}catch{throw new Error('DOC110_MISSING_SCHEMA_NON_JSON');}
        return {status:response.status(),result};
      },
      async checkUnavailableUI(){
        const response=await openImportAdministrator(auth.page,'abrir','ListarPerfiles');
        expect(response.status()).toBe(503);
        await expect(auth.page.locator('#doc110-perfiles [data-doc110=estado]')).toContainText('La configuración de perfiles no está disponible.');
      },
      async upload(){
        evidence.uploadEntered=true;
        // Reutiliza la carga DOC-88 oficial, sin reenviar ante fallo ni cambiar su contrato.
        return inspectProductionDocumentUploadPreviewUi({context:auth.context,plan});
      }
    });
    evidence.result=result;evidence.status='PASS';
  }catch(error){evidence.status='FAIL';throw error;}
  finally{
    try{await bridge.close();}
    catch{evidence.status='FAIL';throw new Error(evidence.renameAttempted?'DOC110_SCHEMA_RESTORATION_MUST_BE_VERIFIED':'DOC110_MISSING_SCHEMA_PREFLIGHT_OR_CONTROL_FAILED');}
    finally{
      evidence.finishedUtc=new Date().toISOString();
      const report=path.resolve(__dirname,'../../artifacts/doc110-missing-schema-result.json');
      fs.mkdirSync(path.dirname(report),{recursive:true});fs.writeFileSync(report,JSON.stringify(evidence,null,2));
    }
  }
}
module.exports={verifyMissingSchema,exerciseMissingSchema};
