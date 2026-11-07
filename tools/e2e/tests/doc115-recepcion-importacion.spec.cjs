'use strict';
const {test}=require('@playwright/test');
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const {openSession}=require('./support/doc111-importacion-carpetas.cjs');
const {captureCodeEvidence}=require('./support/doc110-code-evidence.cjs');
const {queryRows}=require('../scripts/support/doc32-e2e-odbc.cjs');
const {SCENARIOS,validateSession,authorize,runReceiptSession,safeFailureCode}=require('./support/doc115-recepcion-importacion.cjs');
test.use({screenshot:'off',trace:'off',video:'off'});
test.describe.configure({mode:'serial',retries:0});
for(const scenario of SCENARIOS)test('DOC115 '+scenario,async({browser})=>{
  test.skip(process.env.DOC115_E2E_AUTHORIZED!=='true'||process.env.DOC115_SCENARIO!==scenario,'NO_EJECUTADO: exige autorización específica del ambiente, cuenta y recursos DOC-115.');
  let auth,checkpoint,file,stage='PRECHECK';const save=()=>fs.writeFileSync(file,JSON.stringify(checkpoint,null,2)+'\n');
  try{
    const session=validateSession(JSON.parse(process.env.DOC115_SESSION));if(session.scenario!==scenario)throw Error('DOC115_SCENARIO_INVALID');
    const a=Object.fromEntries(['environment','account','select','effects','discardable','tls','interruption'].map(k=>[k,process.env['DOC115_AUTH_'+k.toUpperCase()]==='true']));authorize(session,a);
    if(!/^[a-f0-9]{32}$/.test(process.env.DOC115_RUN_ID||''))throw Error('DOC115_RUN_ID_REQUIRED');
    const backend=path.join(session.runtimeRoot,'bin/GestionDocumental-Docuarchi.net.dll'),fingerprint=()=>crypto.createHash('sha256').update(fs.readFileSync(backend)).digest('hex'),backendSha256=fingerprint();if(backendSha256!==process.env.DOC115_BACKEND_SHA256)throw Error('DOC115_BACKEND_CHANGED');
    const dir=path.resolve(__dirname,'../artifacts');fs.mkdirSync(dir,{recursive:true});file=path.join(dir,'doc115-'+process.env.DOC115_RUN_ID+'.json');
    const initial={scenario,status:'STARTING',backendSha256,references:[]};fs.writeFileSync(file,JSON.stringify(initial,null,2)+'\n',{flag:'wx'});checkpoint=initial;
    test.setTimeout(session.profile.timeoutMilliseconds);
    stage='LOGIN';auth=await openSession(browser,session.profile,process.env,a);
    const code=captureCodeEvidence({files:['Services/ProduccionDocumental/ImportacionCarpetas/ServicioRecepcionImportacion.vb','Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.vb','Infrastructure/ProduccionDocumental/ImportacionCarpetas/AlmacenStagingImportacion.vb','Infrastructure/ProduccionDocumental/ImportacionCarpetas/ImportacionCarpetasRequestGuard.vb','generic_control/ImportacionCarpetasUpload.ashx.vb','tools/e2e/tests/doc115-recepcion-importacion.spec.cjs','tools/e2e/tests/support/doc115-recepcion-importacion.cjs','tools/e2e/scripts/run-doc115-recepcion-importacion.cjs','tools/e2e/fixtures/doc115-recepcion-cases.json','tools/e2e/tests/support/doc114-plan-importacion.cjs']});
    stage='RECIB';checkpoint.status='RUNNING';save();
    const query=(origin,sql,args)=>queryRows(sql,args,process.env,origin==='RA'?'DOC111_E2E':'DOC111_DA_E2E');
    const result=await runReceiptSession(auth,session,a,query,reference=>{checkpoint.references.push(reference);save();console.log('DOC115_REFERENCE='+JSON.stringify({scenario,...reference}));},{onProgress:progress=>{checkpoint.progress=progress;save();},onInterruptionVerified:observation=>{checkpoint.interruption=observation;save();}});
    if(fingerprint()!==backendSha256)throw Error('DOC115_BACKEND_CHANGED');checkpoint.status='PASS';checkpoint.result={...result,head:code.head,codeSha256:code.scopeSha256,backendSha256};save();console.log('DOC115_RESULT='+JSON.stringify(checkpoint.result));
  }catch(error){const code=safeFailureCode(error);if(checkpoint){checkpoint.status='STOPPED_REVIEW_REQUIRED';checkpoint.failure={stage,code};try{save();}catch{}}console.log('DOC115_FAILURE='+JSON.stringify({scenario,stage,code}));throw Error(code);
  }finally{if(auth)await auth.context.close().catch(()=>{});}
});
