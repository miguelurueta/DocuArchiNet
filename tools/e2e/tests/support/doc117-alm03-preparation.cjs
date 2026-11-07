'use strict';
const {preflightFailure}=require('../../scripts/support/doc117-alm03-preflight-evidence.cjs');
const {loginObservation}=require('./doc111-importacion-carpetas.cjs');
async function runPreparation({open,selectLevel,readLot,onProgress}){
 let stage='SESSION_OPEN_REQUESTED',auth,sessionEstablished=false,levelSelected=false,observation=null;
 const step=s=>{stage=s;const result=onProgress({stage:s});if(result&&typeof result.then==='function'){Promise.resolve(result).catch(()=>{});throw Error('DOC117_ALM03_PREPARATION_DURABLE_PROGRESS_REQUIRED');}};
 try{
  step('SESSION_OPEN_REQUESTED');auth=await open();sessionEstablished=true;observation=loginObservation(auth.loginObservation);step('SESSION_OPENED');
  step('LEVEL_SELECTION_REQUESTED');await selectLevel(auth,'7');levelSelected=true;step('LEVEL_SELECTED');
  step('LOT_READ_REQUESTED');const lot=await readLot(auth,'22');if(lot?.loteId!=='22'||lot?.nivelRef!=='7')throw Error('DOC117_ALM03_PREPARATION_LOT_MISMATCH');step('LOT_READ_VERIFIED');
  return{status:'PREPARATION_VERIFIED',sessionEstablished,levelSelected,lotAccessible:true,loginObservation:observation,controlSelectRequests:0,cumulativeSelectRequests:146,temporaryUploads:0,storageRequests:0,runtimeActivation:false,alm03ConcurrentPass:false};
 }catch(error){
  const failure=preflightFailure(error);if(error?.message==='DOC117_ALM03_PREPARATION_LOT_MISMATCH')failure.code=error.message;
  return{status:'PREPARATION_STOPPED',failedAt:stage,sessionEstablished,levelSelected,lotAccessible:false,failure:{...failure,loginObservation:failure.loginObservation||observation},controlSelectRequests:0,cumulativeSelectRequests:146,temporaryUploads:0,storageRequests:0,runtimeActivation:false,alm03ConcurrentPass:false};
 }
}
module.exports={runPreparation};
