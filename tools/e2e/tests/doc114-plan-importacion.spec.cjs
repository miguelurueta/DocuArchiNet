'use strict';
const {test}=require('@playwright/test');
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const {openSession}=require('./support/doc111-importacion-carpetas.cjs');
const {captureCodeEvidence}=require('./support/doc110-code-evidence.cjs');
const {queryRows}=require('../scripts/support/doc32-e2e-odbc.cjs');
const {SCENARIOS,validateSession,runPlanSession,validateResumePlan,safePlanObservation}=require('./support/doc114-plan-importacion.cjs');
test.use({screenshot:'off',trace:'off',video:'off'});
test.describe.configure({mode:'serial',retries:0});
for(const scenario of SCENARIOS)test('DOC114 '+scenario,async({browser})=>{
  test.skip(process.env.DOC114_E2E_AUTHORIZED!=='true'||process.env.DOC114_SCENARIO!==scenario,'NO_EJECUTADO: exige autorización específica DOC-114 y recursos revisados.');
  let auth,stage='PRECHECK',checkpointPath,checkpoint,ownsCheckpoint=false;
  const save=()=>fs.writeFileSync(checkpointPath,JSON.stringify(checkpoint,null,2)+'\n');
  try{
    const session=validateSession(JSON.parse(process.env.DOC114_SESSION));
    const resumePlan=validateResumePlan(session,process.env.DOC114_RESUME_PLAN?JSON.parse(process.env.DOC114_RESUME_PLAN):undefined);
    if(session.scenario!==scenario)throw Error('DOC114_SCENARIO_INVALID');
    const authorization=Object.fromEntries(['environment','account','select','effects','discardable','tls','revocation'].map(k=>[k,process.env['DOC114_AUTH_'+k.toUpperCase()]==='true']));
    if(!['environment','account','select','effects','discardable'].every(k=>authorization[k])||session.profiles[0].ignoreHttpsErrors&&!authorization.tls||scenario==='PLAN-03'&&!authorization.revocation)throw Error('DOC114_AUTHORIZATION_REQUIRED');
    const backendPath=path.join(session.runtimeRoot,'bin/GestionDocumental-Docuarchi.net.dll');
    const backendSha256=crypto.createHash('sha256').update(fs.readFileSync(backendPath)).digest('hex');
    if(backendSha256!==process.env.DOC114_BACKEND_SHA256)throw Error('DOC114_BACKEND_CHANGED');
    if(!/^[a-f0-9]{32}$/.test(process.env.DOC114_RUN_ID||''))throw Error('DOC114_RUN_ID_REQUIRED');
    const artifactDirectory=path.resolve(__dirname,'../artifacts');fs.mkdirSync(artifactDirectory,{recursive:true});
    checkpointPath=path.join(artifactDirectory,'doc114-'+process.env.DOC114_RUN_ID+'.json');
    checkpoint={scenario,status:'STARTING',backendSha256,references:[],...(resumePlan?{resumePlan}: {})};
    fs.writeFileSync(checkpointPath,JSON.stringify(checkpoint,null,2)+'\n',{flag:'wx'});
    ownsCheckpoint=true;
    test.setTimeout(Math.max(...session.profiles.map(p=>p.timeoutMilliseconds)));
    stage='LOGIN';auth=await openSession(browser,session.profiles[0],process.env,authorization);
    const code=captureCodeEvidence({files:['Services/ProduccionDocumental/ImportacionCarpetas/ServicioPreparacionImportacion.vb','webservice/WebServiceImportacionCarpetas.asmx.vb','tools/e2e/tests/doc114-plan-importacion.spec.cjs','tools/e2e/tests/support/doc114-plan-importacion.cjs','tools/e2e/scripts/run-doc114-plan-importacion.cjs','tools/e2e/fixtures/doc114-plan-cases.json','tools/e2e/tests/support/doc110-permission-revocation.cjs','tools/e2e/scripts/support/doc110-permission-db.ps1']});
    stage='PLAN';
    const query=(origin,sql,args)=>queryRows(sql,args,process.env,origin==='RA'?'DOC111_E2E':'DOC111_DA_E2E');
    checkpoint.status='RUNNING';save();
    const result=await runPlanSession(auth,session,authorization,query,reference=>{checkpoint.references.push(reference);save();console.log('DOC114_PLAN_REFERENCE='+JSON.stringify({scenario,...reference}));},{resumePlan:resumePlan||undefined,onProgress:progress=>{checkpoint.progress=progress;save();},onPlanVerified:proof=>{(checkpoint.verifiedPlans??=[]).push(proof);save();}});
    if(crypto.createHash('sha256').update(fs.readFileSync(backendPath)).digest('hex')!==backendSha256)throw Error('DOC114_BACKEND_CHANGED');
    checkpoint.status='PASS';checkpoint.result={...result,head:code.head,codeSha256:code.scopeSha256,backendSha256};save();
    console.log('DOC114_PLAN_RESULT='+JSON.stringify(checkpoint.result));
  }catch(error){
    const code=/^DOC114_[A-Z0-9_]{3,80}$/.test(error?.message)?error.message:'DOC114_E2E_FAILED';
    const observation=safePlanObservation(error?.observation);
    if(ownsCheckpoint){checkpoint.status='STOPPED_REVIEW_REQUIRED';checkpoint.failure={stage,code,...(observation?{observation}: {})};try{save();}catch{}}
    console.log('DOC114_PLAN_FAILURE='+JSON.stringify({scenario,stage,code,...(observation?{observation}: {})}));throw Error(code);
  }finally{if(auth)await auth.context.close().catch(()=>{});}
});
