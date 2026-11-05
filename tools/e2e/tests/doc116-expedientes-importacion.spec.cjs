'use strict';
const {test}=require('@playwright/test');
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const {openSession}=require('./support/doc111-importacion-carpetas.cjs');
const {captureCodeEvidence}=require('./support/doc110-code-evidence.cjs');
const {queryRows}=require('../scripts/support/doc32-e2e-odbc.cjs');
const {validateSession,authorize,runCreationScenario,selectProductionLevel,safeFailureCode}=require('./support/doc116-expedientes-importacion.cjs');
test.use({screenshot:'off',trace:'off',video:'off'});
test.describe.configure({mode:'serial',retries:0});
for(const scenario of ['EXP-01','EXP-02','EXP-03'])test('DOC116 '+scenario,async({browser})=>{
  test.skip(process.env.DOC116_E2E_AUTHORIZED!=='true'||process.env.DOC116_SCENARIO!==scenario,'NO_EJECUTADO: exige autorización específica y lote ya confirmado con dos originales recibidos.');
  let auth,file,checkpoint,stage='PRECHECK';const save=()=>fs.writeFileSync(file,JSON.stringify(checkpoint,null,2)+'\n');
  try{
    const session=validateSession(JSON.parse(process.env.DOC116_SESSION));if(session.scenario!==scenario)throw Error('DOC116_SCENARIO_INVALID');
    const a=Object.fromEntries(['environment','account','select','effects','discardable','tls','exclusive','levelPermission'].map(k=>[k,process.env['DOC116_AUTH_'+k.toUpperCase()]==='true']));authorize(session,a);
    if(scenario==='EXP-03'&&path.relative(fs.realpathSync(path.resolve(__dirname,'../../..')),fs.realpathSync(session.runtimeRoot))!=='')throw Error('DOC116_LEVEL_BRIDGE_SCOPE_CHANGED');
    if(!/^[a-f0-9]{32}$/.test(process.env.DOC116_RUN_ID||''))throw Error('DOC116_OPERATION_NOT_CONFIRMED');
    const backend=path.join(session.runtimeRoot,'bin/GestionDocumental-Docuarchi.net.dll'),fingerprint=()=>crypto.createHash('sha256').update(fs.readFileSync(backend)).digest('hex');
    const backendSha256=fingerprint();if(backendSha256!==process.env.DOC116_BACKEND_SHA256)throw Error('DOC116_OPERATION_NOT_CONFIRMED');
    const code=captureCodeEvidence({files:['Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorExpedienteImportacion.vb','Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorExpedienteImportacion.Legacy.vb','Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Creacion.vb','Infrastructure/ProduccionDocumental/ImportacionCarpetas/ImportacionCarpetasContextAdapter.vb','Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.ResultadoCreacion.vb','tools/e2e/tests/doc116-expedientes-importacion.spec.cjs','tools/e2e/tests/support/doc116-expedientes-importacion.cjs','tools/e2e/scripts/run-doc116-expedientes-importacion.cjs','tools/e2e/fixtures/doc116-expedientes-cases.json','tools/e2e/scripts/support/doc116-level-permission.ps1','tools/e2e/scripts/support/doc110-permission-db.ps1','tools/e2e/tests/support/doc110-permission-revocation.cjs']});
    const dir=path.resolve(__dirname,'../artifacts');fs.mkdirSync(dir,{recursive:true});file=path.join(dir,'doc116-'+process.env.DOC116_RUN_ID+'.json');
    checkpoint={scenario,status:'STARTING',loteId:session.lotId,references:[],backendSha256,head:code.head,codeSha256:code.scopeSha256};fs.writeFileSync(file,JSON.stringify(checkpoint,null,2)+'\n',{flag:'wx'});
    test.setTimeout(session.profile.timeoutMilliseconds);stage='LOGIN';auth=await openSession(browser,session.profile,process.env,a);
    stage='CREATION';checkpoint.status='RUNNING';save();
    const query=(origin,sql,args)=>queryRows(sql,args,process.env,origin==='RA'?'DOC111_E2E':'DOC111_DA_E2E');
    const createLevelBridge=()=>require('./support/doc116-level-bridge.cjs').startLevelBridge(session,path.join(dir,'doc116-level-resources-'+process.env.DOC116_RUN_ID+'.json'));
    const result=await runCreationScenario(auth,session,a,query,reference=>{checkpoint.references.push(reference);save();console.log('DOC116_REFERENCE='+JSON.stringify({scenario,...reference}));},{selectLevel:selectProductionLevel,createLevelBridge});
    if(fingerprint()!==backendSha256)throw Error('DOC116_OPERATION_NOT_CONFIRMED');
    checkpoint.status='PASS';checkpoint.result={...result,head:code.head,codeSha256:code.scopeSha256,backendSha256};save();console.log('DOC116_RESULT='+JSON.stringify(checkpoint.result));
  }catch(error){const code=safeFailureCode(error);if(checkpoint){checkpoint.status='STOPPED_REVIEW_REQUIRED';checkpoint.failure={stage,code};try{save();}catch{}}console.log('DOC116_FAILURE='+JSON.stringify({scenario,stage,code}));throw Error(code);
  }finally{if(auth)await auth.context.close().catch(()=>{});}
});
test('DOC116 EXP-02 internal failure integration',async()=>{test.skip(true,'NO_EJECUTADO: no hay composición autenticada segura del fallo postlegacy/precheckpoint; el descarte de resultado en cliente no lo sustituye.');});
