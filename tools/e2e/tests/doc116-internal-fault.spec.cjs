'use strict';
const {test}=require('@playwright/test'),fs=require('node:fs'),path=require('node:path'),{execFileSync}=require('node:child_process');
const {openSession,createDriver}=require('./support/doc111-importacion-carpetas.cjs');
const {assertInitialSnapshot,createCreationControls,createLevelControls}=require('./support/doc116-expedientes-importacion.cjs');
const {preservation,hash}=require('./support/doc116-exp01-setup.cjs');
const {runInternalFault,validateScope}=require('./support/doc116-internal-fault.cjs');
const {queryRows}=require('../scripts/support/doc32-e2e-odbc.cjs');
const {createLease,validateManifest}=require('../scripts/support/doc116-runtime-lease.cjs');
test.use({screenshot:'off',trace:'off',video:'off'});
test('DOC116 INTERNAL-EXP02',async({browser})=>{
  test.skip(process.env.DOC116_INTERNAL_AUTHORIZED!=='true','Requiere variante y activación específicamente autorizadas.');test.setTimeout(480000);
  let m,auth,lease,armed=false,record;const file=process.env.DOC116_INTERNAL_CHECKPOINT;const save=()=>fs.writeFileSync(file,JSON.stringify(record,null,2)+'\n');
  const authorization={environment:true,account:true,select:true,effects:true,discardable:true,tls:true,exclusive:true};
  try{
    const manifestFile=process.env.DOC116_INTERNAL_MANIFEST;m=validateScope(validateManifest(JSON.parse(fs.readFileSync(manifestFile,'utf8'))));
    lease=createLease(m);if(lease.current()!==m.originalSha256)throw Error('DOC116_INTERNAL_ORIGINAL_RUNTIME_CHANGED');
    for(const [source,digest]of Object.entries(m.reviewedFiles))if(hash(fs.readFileSync(path.join(m.runtimeRoot,source)))!==digest)throw Error('DOC116_INTERNAL_REVIEW_CHANGED');
    record={scenario:m.scenario,status:'STARTING',lotId:'18',history:[],originalSha256:m.originalSha256,candidateSha256:m.candidateSha256,codeSha256:hash(m.reviewedFiles)};fs.writeFileSync(file,JSON.stringify(record,null,2)+'\n',{flag:'wx'});
    const step=v=>{record.history.push(v);record.status=v.stage;save();console.log('DOC116_INTERNAL_REFERENCE='+JSON.stringify(v));};
    const query=(origin,sql,args)=>queryRows(sql,args,process.env,origin==='RA'?'DOC111_E2E':'DOC111_DA_E2E');
    auth=await openSession(browser,m.session.profile,process.env,authorization);
    const initial=await createCreationControls(query,m.session).snapshot();assertInitialSnapshot(initial,m.session);
    await createLevelControls(query,{...m.session,scenario:'EXP-03',levelPermission:m.levelPermission}).grant(true);
    const current=await query('RA','SELECT (SELECT MAX(plan_id) FROM ra_integracion_plan WHERE usuario_ref=? AND ambito_ref=?) AS maxPlan,(SELECT MAX(lote_id) FROM ra_integracion_lote WHERE solicitante_ref=? AND ambito_ref=?) AS maxLot',['141','2','141','2']);if(current.length!==1||String(current[0].maxPlan)!=='34'||String(current[0].maxLot)!=='18')throw Error('DOC116_INTERNAL_REFERENCES_CHANGED');
    const p={identity:m.session.identity,preservedLotIds:m.preservedLotIds},bounds={maxPlan:'34',maxLot:'17'},before=await preservation(query,p,bounds);record.preservedBefore=before;save();
    const reader=createDriver(auth,m.session.profile,{environment:true,account:true,select:true,tls:true},{readOnly:true});await reader.refreshToken();await reader.request('ConsultarLote',{loteId:'18'});
    for(const root of m.session.roots)if(fs.existsSync(path.join(m.markerDirectory,'raiz-'+root.rootId+'.json')))throw Error('DOC116_INTERNAL_EXISTING_MARKER_REQUIRES_REVIEW');
    await auth.context.close();auth=null;
    lease.backup();armed=true;
    const guardEnv=Object.fromEntries(Object.entries(process.env).filter(([k])=>['PATH','Path','SystemRoot','WINDIR','TEMP','TMP','ComSpec','PATHEXT'].includes(k)));
    Object.assign(guardEnv,{DOC116_GUARD_NODE:process.execPath,DOC116_GUARD_SCRIPT:path.resolve(__dirname,'../scripts/support/doc116-runtime-lease.cjs'),DOC116_GUARD_MANIFEST:manifestFile,DOC116_GUARD_OWNER:String(process.pid)});
    execFileSync('powershell.exe',['-NoProfile','-NonInteractive','-Command',"$ErrorActionPreference='Stop'; Start-Process -FilePath $env:DOC116_GUARD_NODE -ArgumentList @($env:DOC116_GUARD_SCRIPT,'--watch',$env:DOC116_GUARD_MANIFEST,$env:DOC116_GUARD_OWNER) -WindowStyle Hidden | Out-Null"],{env:guardEnv,windowsHide:true,stdio:'ignore',timeout:15000});
    let ready=false;for(let i=0;i<40;i++){const guard=JSON.parse(fs.readFileSync(m.leaseJournal,'utf8'));if(guard.stage==='GUARD_READY'&&guard.ownerPid===process.pid){ready=true;break;}await new Promise(resolve=>setTimeout(resolve,250));}if(!ready)throw Error('DOC116_INTERNAL_GUARD_NOT_READY');
    step({stage:'RUNTIME_ACTIVATION_REQUESTED'});lease.activate();step({stage:'RUNTIME_CANDIDATE_ACTIVE'});
    auth=await openSession(browser,m.session.profile,process.env,authorization);
    const readMarker=async root=>JSON.parse(fs.readFileSync(path.join(m.markerDirectory,'raiz-'+root+'.json'),'utf8'));
    const result=await runInternalFault(auth,m,query,readMarker,step);record.internalResult=result;save();
    await auth.context.close();auth=null;lease.restore();record.runtimeRestored=true;step({stage:'ORIGINAL_RUNTIME_RESTORED'});
    auth=await openSession(browser,m.session.profile,process.env,authorization);
    const reopened=createDriver(auth,m.session.profile,{environment:true,account:true,select:true,tls:true},{readOnly:true});await reopened.refreshToken();const state=await reopened.request('ConsultarLote',{loteId:'18'});if(state.loteId!=='18'||state.nivelRef!=='215')throw Error('DOC116_INTERNAL_REOPEN_FAILED');
    const after=await preservation(query,p,bounds);record.preservedAfter=after;if(hash(before)!==hash(after)||lease.current()!==m.originalSha256)throw Error('DOC116_INTERNAL_PRESERVATION_FAILED');
    record.result={...result,runtimeRestored:true,reopenedWithOriginalRuntime:true,preservedResourcesUnchanged:true,originalSha256:m.originalSha256,candidateSha256:m.candidateSha256};record.status='PASS';save();console.log('DOC116_INTERNAL_RESULT='+JSON.stringify(record.result));
  }catch(error){const code=/^DOC116_(?:INTERNAL|RUNTIME)_[A-Z_]+$/.test(error?.message||'')?error.message:'DOC116_INTERNAL_STOPPED_REVIEW_REQUIRED';if(record){record.failure={code,lastStage:record.status};record.status='STOPPED_REVIEW_REQUIRED';try{save();}catch{}}console.log('DOC116_INTERNAL_FAILURE='+JSON.stringify({scenario:'EXP-02-INTERNAL',code}));throw Error(code);
  }finally{
    if(auth)await auth.context.close().catch(()=>{});
    if(armed){try{lease.restore();if(record){record.runtimeRestored=true;save();}console.log('DOC116_RUNTIME_RESTORATION='+JSON.stringify({restored:true,originalSha256:m.originalSha256}));}catch{if(record){record.runtimeRestored=false;try{save();}catch{}}console.log('DOC116_RUNTIME_RESTORATION='+JSON.stringify({restored:false,code:'DOC116_RUNTIME_RESTORATION_REQUIRES_REVIEW'}));throw Error('DOC116_RUNTIME_RESTORATION_REQUIRES_REVIEW');}}
  }
});
