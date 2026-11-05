'use strict';
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const {validateSession}=require('./doc116-expedientes-importacion.cjs');
function levelResources(session){
  validateSession(session);if(session.scenario!=='EXP-03'||!session.levelPermission)throw Error('DOC116_LEVEL_PERMISSION_RESOURCES_REQUIRED');
  return {userRef:session.identity.userRef,companyRef:session.identity.ambitoRef,levelRef:session.profile.nivelRef,lotId:session.lotId,permissionId:session.levelPermission.permissionId,environmentHash:session.levelPermission.environmentHash};
}
async function startLevelBridge(session,resourcesFile){
  const resources=levelResources(session),root=fs.realpathSync(path.resolve(__dirname,'../../../..'));
  if(process.env.DOC116_AUTH_LEVEL_PERMISSION!=='true')throw Error('DOC116_LEVEL_PERMISSION_AUTHORIZATION_REQUIRED');
  if(!session.runtimeRoot||fs.realpathSync(session.runtimeRoot)!==root||path.dirname(path.resolve(resourcesFile))!==path.join(root,'tools/e2e/artifacts'))throw Error('DOC116_LEVEL_BRIDGE_SCOPE_CHANGED');
  const bytes=Buffer.from(JSON.stringify(resources));fs.writeFileSync(resourcesFile,bytes,{flag:'wx'});
  const prior=process.env.DOC116_LEVEL_RESOURCES_SHA256;process.env.DOC116_LEVEL_RESOURCES_SHA256=crypto.createHash('sha256').update(bytes).digest('hex');
  const restore=()=>{if(prior===undefined)delete process.env.DOC116_LEVEL_RESOURCES_SHA256;else process.env.DOC116_LEVEL_RESOURCES_SHA256=prior;};
  try{
    const bridge=require('./doc110-permission-revocation.cjs').startDatabaseBridge('doc110-permission-db.ps1',{doc116LevelResourcesFile:resourcesFile});
    return {ready:()=>bridge.ready(),send:operation=>bridge.send(operation),close:async()=>{try{await bridge.close();}finally{restore();}}};
  }catch(error){restore();throw error;}
}
module.exports={levelResources,startLevelBridge};
