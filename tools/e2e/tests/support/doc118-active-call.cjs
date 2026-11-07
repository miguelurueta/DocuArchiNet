'use strict';

// Bounded orchestration only: callers supply already-authorized sessions and a
// qualified external debugger. No login, attach, deployment, SQL or retry here.
const {performance}=require('node:perf_hooks');
const METHOD='ClassAlmacenamiento.AlmacenamientoDocumentoProduccionDocumental';
const CASES=Object.freeze({COORD02:['SAME_COMMAND','OTHER_FILE'],COORD03:['STOP']});
class CoordinationFault extends Error { constructor(code){super('DOC118_'+code);this.code='DOC118_'+code;} }
const fail=code=>{throw new CoordinationFault(code);};
const check=(value,code)=>{if(!value)fail(code);};
const positive=value=>Number.isSafeInteger(value)&&value>0;
async function limited(work,milliseconds,onTimeout){
  let timer;
  try{return await Promise.race([Promise.resolve().then(work),new Promise((resolve,reject)=>{timer=setTimeout(()=>{try{resolve(onTimeout());}catch(error){reject(error);}},milliseconds);})]);}
  finally{clearTimeout(timer);}
}
function validateObservation(value,scope,previous){
  const keys=['runId','resourceSha256','processId','threadId','method','insideBody','frozen','runtimeQualified'];
  check(value&&typeof value==='object'&&Object.keys(value).sort().join()===keys.slice().sort().join(),'OBSERVATION_INVALID');
  check(value.runId===scope.runId&&value.resourceSha256===scope.resourceSha256&&positive(value.processId)&&positive(value.threadId)&&value.method===METHOD&&value.insideBody===true&&value.frozen===true&&value.runtimeQualified===true,'ACTIVE_BODY_NOT_PROVEN');
  if(previous)check(value.processId===previous.processId&&value.threadId===previous.threadId,'ACTIVE_THREAD_CHANGED');
  return {processId:value.processId,threadId:value.threadId};
}
async function coordinateActiveCall({scenario,scope,first,actions,observer,maximumPauseMilliseconds=30000}){
  check(Object.hasOwn(CASES,scenario),'SCENARIO_INVALID');
  check(scope&&Object.keys(scope).sort().join()==='resourceSha256,runId'&&/^[a-f0-9]{32}$/.test(scope.runId)&&/^[a-f0-9]{64}$/.test(scope.resourceSha256),'SCOPE_INVALID');
  check(typeof first==='function'&&observer&&['waitInside','inspect','resume'].every(k=>typeof observer[k]==='function'),'DEPENDENCIES_INVALID');
  check(Array.isArray(actions)&&actions.length===CASES[scenario].length&&actions.every((a,i)=>a&&a.kind===CASES[scenario][i]&&typeof a.execute==='function'),'ACTIONS_INVALID');
  check(Number.isInteger(maximumPauseMilliseconds)&&maximumPauseMilliseconds>0&&maximumPauseMilliseconds<=30000,'PAUSE_LIMIT_INVALID');
  scope=Object.freeze({...scope});
  actions=actions.map(({kind,execute})=>Object.freeze({kind,execute}));
  const started=performance.now(),order=[],outcomes=[];let firstSettled=false,firstOutcome='UNKNOWN',errorCode=null;
  // Observe rejection immediately; the original error can contain headers or paths.
  const pending=Promise.resolve().then(first).then(()=>{firstSettled=true;firstOutcome='FULFILLED';},()=>{firstSettled=true;firstOutcome='REJECTED';});
  let release=true;
  const bounded=async(work)=>{
    const left=maximumPauseMilliseconds-(performance.now()-started);
    check(left>0,'PAUSE_LIMIT_REACHED');
    return limited(work,left,()=>fail('PAUSE_LIMIT_REACHED'));
  };
  try{
    const proof=validateObservation(await bounded(()=>observer.waitInside(scope)),scope);check(!firstSettled,'FIRST_ALREADY_SETTLED');order.push('INSIDE_FROZEN');
    for(const action of actions){
      validateObservation(await bounded(()=>observer.inspect(scope)),scope,proof);check(!firstSettled,'FIRST_ALREADY_SETTLED');
      order.push(action.kind+'_START');
      await bounded(()=>action.execute());outcomes.push({kind:action.kind,outcome:'FULFILLED'});order.push(action.kind+'_END');
      validateObservation(await bounded(()=>observer.inspect(scope)),scope,proof);check(!firstSettled,'FIRST_ALREADY_SETTLED');
    }
    order.push('ACTIVE_INTERVAL_VERIFIED');
  }catch(error){errorCode=error instanceof CoordinationFault?error.code:'DOC118_COORDINATION_UNCERTAIN';}
  finally{
    // Always attempt release once, even if observation failed after freezing.
    try{await limited(()=>observer.resume(scope),5000,()=>fail('RESUME_NOT_CONFIRMED'));order.push('RESUME_REQUESTED');release=false;}
    catch{errorCode='DOC118_RESUME_NOT_CONFIRMED';}
    // The caller must independently query the durable outcome. Never resend first.
    await limited(()=>pending,1000,()=>undefined);
  }
  const result={scenario,coordinationVerified:errorCode===null,firstOutcome,actions:outcomes,order,releaseUnconfirmed:release,businessEvidenceVerified:false,integrationPass:false};
  if(errorCode){const error=Object.assign(new Error(errorCode),{code:errorCode,coordination:result});throw error;}
  return result;
}
module.exports={METHOD,CASES,validateObservation,coordinateActiveCall};
