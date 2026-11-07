'use strict';
const {test} = require('@playwright/test');
const fs = require('node:fs');
const path = require('node:path');
const crypto = require('node:crypto');
const {MODES,validateProfile,readFixture,assertAuthorizations,loginObservation,openSession,createDriver,createSelectControls,runRec,runResume,runReconciliationResume,runCompletedVerification} = require('./support/doc111-importacion-carpetas.cjs');
const {captureCodeEvidence} = require('./support/doc110-code-evidence.cjs');
const {queryRows} = require('../scripts/support/doc32-e2e-odbc.cjs');

function scopeSha() {
  const root=path.resolve(__dirname,'../../..');
  const dirs=['Modelo/ProduccionDocumental/ImportacionCarpetas','DTOs/ProduccionDocumental/ImportacionCarpetas','Services/ProduccionDocumental/ImportacionCarpetas','Infrastructure/ProduccionDocumental/ImportacionCarpetas','Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas','js/gestion/ImportacionCarpetas'];
  const files=dirs.flatMap(d=>fs.readdirSync(path.join(root,d)).filter(n=>/\.(vb|js|css)$/.test(n)).map(n=>d+'/'+n));
  files.push('tools/e2e/tests/support/doc111-degradation.cjs','tools/e2e/tests/support/doc110-missing-schema.cjs','tools/e2e/tests/support/doc110-permission-revocation.cjs','tools/e2e/tests/support/doc110-admin-navigation.cjs','tools/e2e/scripts/support/doc110-profile-schema-db.ps1','tools/e2e/scripts/run-workflow-e2e-platform.cjs');
  files.push('js/gestion/WebFormProducionDocumental.js','webservice/WebServiceImportacionCarpetas.asmx.vb','webservice/WebServiceImportacionCarpetas.Composicion.vb','generic_control/ImportacionCarpetasUpload.ashx.vb','tools/e2e/tests/support/doc111-partial-profile.cjs','tools/e2e/tests/support/doc111-importacion-carpetas.cjs','tools/e2e/tests/doc111-importacion-carpetas.spec.cjs','tools/e2e/scripts/run-doc111-importacion-carpetas.cjs','tools/e2e/scripts/support/doc32-e2e-odbc.cjs','tools/e2e/scripts/query-doc32-odbc-fingerprint.ps1');
  return crypto.createHash('sha256').update(JSON.stringify(files.sort().map(file=>({file,sha:crypto.createHash('sha256').update(fs.readFileSync(path.join(root,file))).digest('hex')})))).digest('hex');
}

// Configuracion/runner existentes. Ninguna repeticion automatica de mutaciones.
test.use({screenshot:'off',trace:'off',video:'off'});
test.describe.configure({mode:'serial',retries:0,timeout:900000});
for (const mode of MODES) test('DOC-111 REC-01/02 ' + mode, async ({browser}) => {
  test.skip(process.env.DOC111_E2E_AUTHORIZED !== 'true' || process.env.DOC111_E2E_SCENARIO !== mode,
    'NO_EJECUTADO: requiere autorizacion DOC-111 especifica de ambiente, cuenta, efectos, descartables y SELECT.');
  const authorization = Object.fromEntries(['environment','account','effects','discardable','select'].map(k => [k,process.env['DOC111_AUTH_' + k.toUpperCase()] === 'true']));
  let auth,stage='PRECHECK';
  try {
    let profile = validateProfile(JSON.parse(process.env.DOC111_E2E_PROFILE));
    const diagnosticOnly=process.env.DOC111_E2E_DIAGNOSE==='true';
    const verifyLot=process.env.DOC111_E2E_VERIFY_LOT;
    if(diagnosticOnly){if(!authorization.environment||!authorization.account)throw new Error('DOC111_AUTHORIZATION_REQUIRED');}
    else if(verifyLot){if(!authorization.environment||!authorization.account||!authorization.select||authorization.effects||authorization.discardable)throw new Error('DOC111_AUTHORIZATION_REQUIRED');}
    else assertAuthorizations(authorization);
    if(profile.ignoreHttpsErrors && process.env.DOC111_AUTH_TLS!=='true')throw new Error('DOC111_AUTHORIZATION_REQUIRED');
    if(profile.mode!==mode)throw new Error('DOC111_MODE_MISMATCH');
    const bytes = readFixture(profile), code = captureCodeEvidence();
    const driverSha256 = scopeSha();
    // Reuse the registered DSNs and SELECT bridge; never request connection URLs.
    const query=(origin,sql,args)=>queryRows(sql,args,process.env,origin==='RA'?'DOC111_E2E':'DOC111_DA_E2E');
    for(const origin of diagnosticOnly?[]:['RA','DA']){
      stage='READONLY_'+origin;
      await query(origin,origin==='RA'?'SELECT COUNT(*) AS total FROM ra_integracion_plan WHERE plan_id=?':'SELECT COUNT(*) AS total FROM system1 WHERE id_gabinete=?',[0]);
    }
    stage='LOGIN';
    auth = await openSession(browser,profile,process.env,authorization);
    if(diagnosticOnly){console.log('DOC111_DIAGNOSTIC_RESULT='+JSON.stringify({status:'PASS',mode,pageReady:true,loginObservation:auth.loginObservation}));return;}
    if(process.env.DOC111_E2E_DEGRADATION_PROFILE){
      stage='SCHEMA';
      if(mode!=='LOTE'||verifyLot||process.env.DOC111_E2E_RESUME_LOT||process.env.DOC111_E2E_PARTIAL_PROPOSAL)throw new Error('DOC111_DEGRADATION_SCOPE_INVALID');
      const result=await require('./support/doc111-degradation.cjs').runDegradation(auth,profile,JSON.parse(process.env.DOC111_E2E_DEGRADATION_PROFILE),authorization,query);
      if(captureCodeEvidence().scopeSha256!==code.scopeSha256||scopeSha()!==driverSha256)throw new Error('DOC111_CODE_CHANGED');
      console.log('DOC111_DEGRADATION_RESULT='+JSON.stringify({status:'PASS',...result,head:code.head,codeSha256:code.scopeSha256,driverSha256}));return;
    }
    if(process.env.DOC111_E2E_PARTIAL_PROPOSAL){
      stage='PROFILE';
      if(mode!=='PARCIAL'||verifyLot||process.env.DOC111_E2E_RESUME_LOT)throw new Error('DOC111_PARTIAL_PROPOSAL_INVALID');
      profile=await require('./support/doc111-partial-profile.cjs').preparePartialProfile(auth,profile,JSON.parse(process.env.DOC111_E2E_PARTIAL_PROPOSAL),authorization,reference=>console.log('DOC111_PROFILE_REFERENCE='+JSON.stringify({mode,...reference})));
    }
    const controls = createSelectControls(query);
    stage=verifyLot?'VERIFY':'REC';
    const resumeLot=process.env.DOC111_E2E_RESUME_LOT;
    const driver=createDriver(auth,profile,authorization,{readOnly:Boolean(verifyLot)});
    const reconcileAttempt=process.env.DOC111_E2E_RECONCILE_ATTEMPT;
    const result = verifyLot?await runCompletedVerification(driver,controls,profile,bytes,verifyLot):reconcileAttempt?await runReconciliationResume(driver,controls,profile,bytes,resumeLot,reconcileAttempt):resumeLot?await runResume(driver,controls,profile,bytes,resumeLot,process.env.DOC111_E2E_CLOSE_ATTEMPT):await runRec(driver,controls,profile,bytes,reference=>console.log('DOC111_REC_REFERENCE='+JSON.stringify({mode,...reference})));
    if(captureCodeEvidence().scopeSha256!==code.scopeSha256||scopeSha()!==driverSha256)throw new Error('DOC111_CODE_CHANGED');
    console.log((verifyLot?'DOC111_COMPLETED_VERIFICATION_RESULT=':reconcileAttempt?'DOC111_RECONCILE_RESUME_RESULT=':resumeLot?'DOC111_RESUME_RESULT=':'DOC111_REC_RESULT=') + JSON.stringify({status:'PASS',...result,head:code.head,codeSha256:code.scopeSha256,driverSha256}));
  }catch(error){
    const code = /^DOC111_[A-Z_]{3,70}$/.test(error?.message||'')?error.message:'DOC111_REC_FAILED';
    const observed=loginObservation(error?.loginObservation);
    console.error('DOC111_REC_FAILURE='+JSON.stringify({mode,stage,code,...(stage==='LOGIN'&&observed?{loginObservation:observed}:{}),...(code==='DOC111_OPERATION_NOT_CONFIRMED'?{operation:error.operation,httpStatus:error.httpStatus,serverCode:error.serverCode}:{})}));
    throw new Error(code); // Sin causa/stack de HTTP, SQL, login o payloads privados.
  }finally{
    if(auth)await auth.context.close().catch(()=>{});
  }
});
