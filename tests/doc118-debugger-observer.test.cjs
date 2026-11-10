'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs'),os=require('node:os'),path=require('node:path');
const {connectObserver}=require('../tools/e2e/tests/support/doc118-debugger-observer.cjs');
function fixture(){
 const directory=fs.mkdtempSync(path.join(os.tmpdir(),'doc118-observer-')),runId='e'.repeat(32),resourceSha256='a'.repeat(64),commands=[];
 let peerCalls=0;
 const observation={sourceBound:true,workerAlive:true,processId:26244,threadId:33,frozen:true,otherThreadsRunning:true,released:false,resourceSha256,entriesA:1,entriesB:0,fault:null,closed:false};
 const state={runId,pid:process.pid,sequence:0,status:'ATTACHED',observation,failure:null},save=()=>{state.heartbeatUtc=new Date().toISOString();fs.writeFileSync(path.join(directory,'status.json'),JSON.stringify(state));};save();
 const timer=setInterval(()=>{const request=path.join(directory,'request.json');if(!fs.existsSync(request))return;
  const command=JSON.parse(fs.readFileSync(request));fs.unlinkSync(request);commands.push(command);state.sequence=command.sequence;
  if(command.operation==='ARM')state.status='INSIDE_FROZEN';
  if(command.operation==='RESUME'){observation.frozen=false;observation.released=true;state.status='RESUME';}
  if(command.operation==='CLOSE'){state.status='CLOSED';state.cleanupConfirmed=true;}save();
 },5);
 const observer=connectObserver({directory,runId,resourceSha256,peerProbe:async()=>{peerCalls++;},maximumWaitMilliseconds:200});
 return{observer,state,observation,commands,scope:{runId,resourceSha256},save,peerCalls:()=>peerCalls,async close(){clearInterval(timer);const resolved=path.resolve(directory);assert.ok(resolved.startsWith(path.resolve(os.tmpdir())+path.sep));fs.rmSync(resolved,{recursive:true,force:true});}};
}
test('DOC118 external observer qualifies only after peer progress while the same internal thread remains frozen',async()=>{const f=fixture();try{
 await f.observer.arm({expA:1,nameA:'DOC118-C02-A-20261107.pdf',expB:2,nameB:'DOC118-C02-B-20261107.pdf'});
 const proof=await f.observer.waitInside(f.scope);assert.equal(f.peerCalls(),1);assert.equal(proof.runtimeQualified,true);assert.equal(proof.threadId,33);
 assert.deepEqual(await f.observer.inspect(),proof);await f.observer.resume();assert.equal((await f.observer.summary()).released,true);await f.observer.close();
 assert.ok(f.commands.every(c=>Object.keys(c).sort().join()==='args,operation,sequence'));
}finally{await f.close();}});
test('DOC118 external observer rejects another resource before a peer request',async()=>{const f=fixture();try{
 f.state.status='INSIDE_FROZEN';f.observation.resourceSha256='b'.repeat(64);f.save();await assert.rejects(f.observer.waitInside(f.scope),/ACTIVE_BODY_UNCONFIRMED/);assert.equal(f.peerCalls(),0);
}finally{await f.close();}});
test('DOC118 external observer refuses scope changes and a changed selected thread',async()=>{const f=fixture();try{
 await assert.rejects(f.observer.waitInside({...f.scope,runId:'f'.repeat(32)}),/SCOPE_CHANGED/);f.state.status='INSIDE_FROZEN';f.save();await f.observer.waitInside(f.scope);
 f.observation.threadId=34;f.save();await assert.rejects(f.observer.inspect(),/THREAD_CHANGED/);
}finally{await f.close();}});
test('DOC118 external observer stops on controller failure without claiming runtime qualification',async()=>{const f=fixture();try{
 f.state.failure={stage:'FIND_TARGET_PROCESS'};f.save();await assert.rejects(f.observer.waitInside(f.scope),/CONTROLLER_NOT_AVAILABLE/);assert.equal(f.peerCalls(),0);
}finally{await f.close();}});
test('DOC118 transient status sharing errors do not lose a live frozen observer or resend a command',async()=>{const f=fixture(),originalRead=fs.readFileSync;let remaining=1;try{
 f.state.status='INSIDE_FROZEN';f.save();fs.readFileSync=function(file,...args){if(String(file).endsWith(path.sep+'status.json')&&remaining-->0)throw Object.assign(Error('synthetic sharing collision'),{code:'EBUSY'});return originalRead.call(this,file,...args);};
 const proof=await f.observer.waitInside(f.scope);assert.equal(proof.runtimeQualified,true);assert.equal(f.peerCalls(),1);assert.deepEqual(f.commands.map(c=>c.operation),['INSPECT']);
}finally{fs.readFileSync=originalRead;await f.close();}});
test('DOC118 persistent inaccessible or invalid status cannot qualify the controller or send peer requests',async()=>{for(const failure of ['EBUSY','INVALID_JSON']){const f=fixture(),originalRead=fs.readFileSync;try{
 fs.readFileSync=function(file,...args){if(String(file).endsWith(path.sep+'status.json')){if(failure==='INVALID_JSON')return '{broken';throw Object.assign(Error('synthetic inaccessible status'),{code:failure});}return originalRead.call(this,file,...args);};
 await assert.rejects(f.observer.waitInside(f.scope),/CONTROLLER_NOT_AVAILABLE/);assert.equal(f.peerCalls(),0);assert.equal(f.commands.length,0);
}finally{fs.readFileSync=originalRead;await f.close();}}});
