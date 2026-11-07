'use strict';
// Orchestration shared by the console and its local lifecycle tests.
async function executeSession({lease,preflightOriginal,closeSession,startGuard,openCandidate,executeWindows,reopenOriginal,verifyAfterRestore,onProgress}){
 let armed=false;
 try{
  const before=await preflightOriginal();await closeSession();
  lease.backup();armed=true;onProgress({stage:'BACKUPS_VERIFIED'});
  await startGuard();onProgress({stage:'RUNTIME_ACTIVATION_REQUESTED'});lease.activate();onProgress({stage:'CANDIDATE_ACTIVE'});
  await openCandidate();const result=await executeWindows(before);await closeSession();
  lease.restore();lease.verifyBackup();onProgress({stage:'ORIGINAL_RUNTIME_RESTORED'});
  await reopenOriginal();const checked=await verifyAfterRestore(before,result);
  if(!checked||lease.current()!==before.originalSha256)throw Error('DOC117_INTERNAL_POST_RESTORE_NOT_VERIFIED');
  return{...result,runtimeRestored:true,bothBackupsVerified:true,reopenedWithOriginalRuntime:true,preservedResourcesUnchanged:true};
 }finally{
  try{await closeSession();}finally{
   if(armed){try{lease.restore();lease.verifyBackup();onProgress({stage:'RESTORATION_FINALLY_VERIFIED',restored:true});}catch{onProgress({stage:'RESTORATION_FAILED',restored:false});throw Error('DOC117_INTERNAL_RESTORATION_REQUIRES_REVIEW');}}
  }
 }
}
module.exports={executeSession};
