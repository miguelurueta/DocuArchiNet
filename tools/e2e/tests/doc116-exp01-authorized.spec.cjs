'use strict';
const {test}=require('@playwright/test');
const fs=require('node:fs'),path=require('node:path');
const {openSession,createDriver,readFixture}=require('./support/doc111-importacion-carpetas.cjs');
const {runCreationScenario,selectProductionLevel,safeFailureCode}=require('./support/doc116-expedientes-importacion.cjs');
const {hash,validateSetup,preservation,prepareFreshLot,publicFailure}=require('./support/doc116-exp01-setup.cjs');
const {queryRows}=require('../scripts/support/doc32-e2e-odbc.cjs');
test.use({screenshot:'off',trace:'off',video:'off'});
for(const scenario of ['EXP-01','EXP-02','EXP-03'])test('DOC116 SETUP-'+scenario.replace('-',''),async({browser})=>{
  test.skip(process.env.DOC116_SETUP_AUTHORIZED!=='true'||(process.env.DOC116_SETUP_SCENARIO||'EXP-01')!==scenario,'Requiere autorización específica de los recursos del escenario seleccionado.');
  let auth,record;const file=process.env.DOC116_SETUP_CHECKPOINT;
  const save=()=>fs.writeFileSync(file,JSON.stringify(record,null,2)+'\n');
  try{
    const p=JSON.parse(process.env.DOC116_SETUP_PROPOSAL),bytes=readFixture(p.profile);validateSetup(p,bytes);if(p.scenario!==scenario)throw Error('DOC116_SETUP_SCOPE_INVALID');test.setTimeout(p.profile.timeoutMilliseconds);
    const backend=path.join(p.runtimeRoot,'bin/GestionDocumental-Docuarchi.net.dll');if(hash(fs.readFileSync(backend))!==process.env.DOC116_BACKEND_SHA256)throw Error('DOC116_OPERATION_NOT_CONFIRMED');
    record={scenario,status:'STARTING',history:[],references:[],backendSha256:process.env.DOC116_BACKEND_SHA256,codeSha256:process.env.DOC116_SETUP_CODE_SHA256,resourcesSha256:process.env.DOC116_SETUP_RESOURCES_SHA256};fs.writeFileSync(file,JSON.stringify(record,null,2)+'\n',{flag:'wx'});
    const a={environment:true,account:true,select:true,effects:true,discardable:true,tls:true,exclusive:true,levelPermission:process.env.DOC116_AUTH_LEVEL_PERMISSION==='true'};
    if(scenario==='EXP-03'&&!a.levelPermission)throw Error('DOC116_LEVEL_PERMISSION_AUTHORIZATION_REQUIRED');
    auth=await openSession(browser,p.profile,process.env,a);
    const query=(origin,sql,args)=>queryRows(sql,args,process.env,origin==='RA'?'DOC111_E2E':'DOC111_DA_E2E');
    const before=await preservation(query,p);record.preservedBefore=before;save();
    const checkpoint=v=>{record.status=v.stage;record.history.push(v);if(v.planId)record.planId=v.planId;if(v.loteId)record.loteId=v.loteId;save();if(v.planId||v.loteId)console.log('DOC116_SETUP_REFERENCE='+JSON.stringify({scenario,stage:v.stage,...(v.planId?{planId:v.planId}:{}),...(v.loteId?{loteId:v.loteId}:{})}));};
    const preflightOnly=process.env.DOC116_SETUP_PROBE_ONLY==='true';
    const session=await prepareFreshLot(createDriver(auth,p.profile,a),query,p,bytes,checkpoint,{preflightOnly});
    if(preflightOnly){const after=await preservation(query,p,before.scope);if(hash(before)!==hash(after))throw Error('DOC116_OPERATION_NOT_CONFIRMED');record.status='PASS_READONLY_PREFLIGHT';record.result={scenario,status:'PASS',preflightOnly:true,preparations:0,confirmations:0,uploads:0,lots:0,rootsCreated:0,creationRequests:0,documents:0,replayUnchanged:null,readOnlyControls:true,preservedResourcesUnchanged:true,backendSha256:record.backendSha256};save();console.log('DOC116_SETUP_RESULT='+JSON.stringify(record.result));return;}
    const createLevelBridge=()=>require('./support/doc116-level-bridge.cjs').startLevelBridge(session,path.join(path.dirname(file),path.basename(file,'.json')+'-level-resources.json'));
    const creation=await runCreationScenario(auth,session,a,query,v=>{record.references.push(v);save();console.log('DOC116_REFERENCE='+JSON.stringify({scenario,...v}));},{selectLevel:selectProductionLevel,createLevelBridge});
    const after=await preservation(query,p,before.scope);record.preservedAfter=after;save();
    if(hash(before)!==hash(after)||hash(fs.readFileSync(backend))!==process.env.DOC116_BACKEND_SHA256)throw Error('DOC116_OPERATION_NOT_CONFIRMED');
    record.result={scenario,status:'PASS',preparations:1,confirmations:1,uploads:2,lots:1,caseComplete:creation.caseComplete,internalFailure:creation.internalFailure,responseObservation:creation.responseObservation,rootsCreated:creation.rootsCreated,creationRequests:creation.counts.creationRequests,documents:0,replayUnchanged:creation.replayUnchanged,...(scenario==='EXP-03'?{denialUnchanged:creation.denialUnchanged,permissionRestored:creation.permissionRestored}:{}),readOnlyControls:true,preservedResourcesUnchanged:true,backendSha256:record.backendSha256};record.status='PASS';save();console.log('DOC116_SETUP_RESULT='+JSON.stringify(record.result));
  }catch(error){const failure=publicFailure(error);if(failure.code==='DOC116_OPERATION_NOT_CONFIRMED')failure.code=safeFailureCode(error);if(record){record.status='STOPPED_REVIEW_REQUIRED';record.failure={...failure,stage:record.statusBeforeFailure||record.history.at(-1)?.stage||'LOGIN_OR_CONTROLS'};try{save();}catch{}}console.log('DOC116_SETUP_FAILURE='+JSON.stringify({scenario,...failure,...(record?.failure?.stage?{stage:record.failure.stage}:{})}));throw Error(failure.code);
  }finally{if(auth)await auth.context.close().catch(()=>{});}
});
