'use strict';
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const check=(v,c)=>{if(!v)throw Error('DOC118_LIVE_'+c);};
const digest=file=>crypto.createHash('sha256').update(fs.readFileSync(file)).digest('hex');
const FILES=['tools/e2e/scripts/run-doc118-live.cjs','tools/e2e/scripts/support/doc118-live-plan.cjs','tools/e2e/scripts/support/Doc118Debugger.cs','tools/e2e/scripts/support/doc118-debugger.ps1','tools/e2e/scripts/support/doc118-debugger-protocol.ps1',...['doc118-session-bridge','doc118-active-call','doc118-case-preparation','doc118-case-completion','doc118-lost-response','doc118-debugger-observer','doc118-live-lifecycle','doc118-symbols','doc111-importacion-carpetas'].map(n=>'tools/e2e/tests/support/'+n+'.cjs')];
function implementationSeal(root){return crypto.createHash('sha256').update(JSON.stringify(FILES.map(file=>[file,digest(path.join(root,file))]))).digest('hex');}
function acquireLease(root,runId,pin){
 check(/^[a-f0-9]{32}$/.test(runId)&&/^[a-f0-9]{64}$/.test(pin),'LEASE_INVALID');
 const directory=path.join(root,'.tmp');fs.mkdirSync(directory,{recursive:true});const file=path.join(directory,'doc118-active-run.json'),owner={runId,pid:process.pid,authorizationScopeSha256:pin};
 try{fs.writeFileSync(file,JSON.stringify(owner)+'\n',{flag:'wx'});}catch{throw Error('DOC118_LIVE_PREVIOUS_RUN_REQUIRES_REVIEW');}
 return{release(){const current=JSON.parse(fs.readFileSync(file,'utf8'));check(JSON.stringify(current)===JSON.stringify(owner),'LEASE_OWNER_CHANGED');fs.unlinkSync(file);}};
}
function assertObserverReady(live,{isAlive=pid=>{try{process.kill(pid,0);return true;}catch{return false;}},now=Date.now()}={}){
 check(/^[a-f0-9]{32}$/.test(live.runId),'OBSERVER_RUN_INVALID');const candidates=[];
 for(let attempt=1;attempt<=3;attempt++){
  const directory=path.join(live.ROOT,'.tmp','doc118-debugger-'+live.runId+'-'+attempt);let state;
  try{state=JSON.parse(fs.readFileSync(path.join(directory,'status.json'),'utf8').replace(/^\uFEFF/,''));}catch{continue;}
  if(!['ATTACHED','INSPECT','RESUME'].includes(state.status)||state.failure||state.runId!==live.runId)continue;
  const v=state.observation,age=now-Date.parse(state.heartbeatUtc);
  if(!v||v.sourceBound!==true||v.otherThreadsRunning!==true||v.workerAlive!==true||v.released!==true||v.frozen||v.closed||v.fault||!Number.isInteger(v.processId)||v.processId<=0||!Number.isInteger(state.pid)||state.pid<=0||!isAlive(state.pid)||!isAlive(v.processId)||!Number.isFinite(age)||age<0||age>5000)continue;
  const expected={controller:digest(path.join(live.ROOT,'tools/e2e/scripts/support/Doc118Debugger.cs')),protocol:digest(path.join(live.ROOT,'tools/e2e/scripts/support/doc118-debugger.ps1')),commandProtocol:digest(path.join(live.ROOT,'tools/e2e/scripts/support/doc118-debugger-protocol.ps1')),source:digest(path.join(live.ROOT,'workflow/ClassAlmacenamiento.vb'))};
  if(!state.implementationHashes||Object.keys(state.implementationHashes).sort().join()!=='commandProtocol,controller,protocol,source'||!Object.entries(expected).every(([k,v])=>state.implementationHashes[k]===v))continue;
  candidates.push({directory,controllerPid:state.pid,workerPid:v.processId,createdUtc:state.createdUtc});
 }
 check(candidates.length===1,'OBSERVER_NOT_READY_BEFORE_CAPTURE');const binding=candidates[0];
 if(live.observerBinding)check(JSON.stringify(live.observerBinding)===JSON.stringify(binding),'OBSERVER_BINDING_CHANGED');
 live.observerBinding=binding;return binding;
}
module.exports={implementationSeal,acquireLease,assertObserverReady};
