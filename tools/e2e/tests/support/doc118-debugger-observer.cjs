'use strict';
const fs=require('node:fs'),path=require('node:path');
const {METHOD}=require('./doc118-active-call.cjs');
const check=(v,c)=>{if(!v)throw Error('DOC118_OBSERVER_'+c);};
const delay=ms=>new Promise(r=>setTimeout(r,ms));
function connectObserver({directory,runId,resourceSha256,peerProbe,maximumWaitMilliseconds=12000}){
 const statusFile=path.join(directory,'status.json'),requestFile=path.join(directory,'request.json');
 const read=()=>{try{return JSON.parse(fs.readFileSync(statusFile,'utf8').replace(/^\uFEFF/,''));}catch{return null;}};
 let qualified=false,threadId=null,processId=null;
 async function command(operation,args={}){
  const before=read();check(before&&before.runId===runId&&!before.failure&&!['CLOSED','UNCONFIRMED','RELEASE_UNCONFIRMED','REVIEW_REQUIRED'].includes(before.status),'CONTROLLER_NOT_AVAILABLE');
  check(!fs.existsSync(requestFile),'COMMAND_PENDING');const sequence=before.sequence+1,temporary=requestFile+'.new';
  fs.writeFileSync(temporary,JSON.stringify({sequence,operation,args}),{flag:'wx'});fs.renameSync(temporary,requestFile);
  const until=Date.now()+4000;
  while(Date.now()<until){const state=read();if(state?.sequence===sequence){check(!state.failure&&!['UNCONFIRMED','RELEASE_UNCONFIRMED','REVIEW_REQUIRED'].includes(state.status),'COMMAND_UNCONFIRMED');return state;}await delay(25);}
  throw Error('DOC118_OBSERVER_COMMAND_TIMEOUT');
 }
 function active(state){const v=state?.observation;check(v&&v.sourceBound===true&&v.frozen===true&&v.otherThreadsRunning===true&&v.released===false&&v.closed===false&&!v.fault&&v.resourceSha256===resourceSha256&&Number.isInteger(v.threadId)&&v.threadId>0&&v.entriesA===1&&v.entriesB===0,'ACTIVE_BODY_UNCONFIRMED');if(threadId!==null)check(v.threadId===threadId&&v.processId===processId,'THREAD_CHANGED');threadId=v.threadId;processId=v.processId;return v;}
 function proof(state){active(state);check(qualified,'PEER_NOT_QUALIFIED');return{runId,resourceSha256,processId,threadId,method:METHOD,insideBody:true,frozen:true,runtimeQualified:true};}
 return{
  async arm(targets){const s=await command('ARM',{...targets,resourceSha256});check(s.observation?.sourceBound===true,'SOURCE_NOT_BOUND');},
  async waitInside(scope){check(scope.runId===runId&&scope.resourceSha256===resourceSha256,'SCOPE_CHANGED');const until=Date.now()+maximumWaitMilliseconds;let state;
   while(Date.now()<until){state=read();check(state&&!state.failure&&!['CLOSED','UNCONFIRMED','RELEASE_UNCONFIRMED','REVIEW_REQUIRED'].includes(state.status),'CONTROLLER_NOT_AVAILABLE');if(state.status==='INSIDE_FROZEN')break;await delay(25);}
   active(state);await peerProbe();state=await command('INSPECT');active(state);qualified=true;return proof(state);
  },
  async inspect(){return proof(await command('INSPECT'));},
  async resume(){await command('RESUME');},
  async summary(){const s=await command('INSPECT');check(s.observation?.released===true&&!s.observation.frozen&&!s.observation.fault,'RELEASE_NOT_CONFIRMED');return s.observation;},
  async close(){const state=read();if(state?.status==='CLOSED')return;await command('CLOSE');const until=Date.now()+4000;while(Date.now()<until){const s=read();if(s?.status==='CLOSED'&&!s.failure)return;await delay(25);}throw Error('DOC118_OBSERVER_CLOSE_UNCONFIRMED');}
 };
}
module.exports={connectObserver};
