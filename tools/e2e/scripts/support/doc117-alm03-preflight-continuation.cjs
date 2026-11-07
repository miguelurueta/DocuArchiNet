'use strict';
// This single known preflight failed before any storage dispatch or runtime
// activation. It may contribute its reads to the same authorized 320 ceiling.
// No storage result, uncertain request or activated runtime is resumable here.
const fs=require('node:fs'),path=require('node:path');
const {ROOT,sha}=require('./doc117-alm03-runtime-lease.cjs');
const PRIOR_ID='350ac90a29f77f9cc46b6b070f610680',PRIOR_CODE='3d35ef187201bfcd4d125844a8b240963266b2606cd5d74e238a24504241f01e';
const FILES={authorization:'.tmp/doc117-alm03-coexistence-authorization.json',priorArtifact:'tools/e2e/artifacts/doc117-alm03-coexistence-'+PRIOR_ID+'.json',diagnostic:'tools/e2e/artifacts/doc117-alm03-index-diagnostic-'+PRIOR_ID+'.json'};
const check=(v,c)=>{if(!v)throw Error('DOC117_ALM03_CONTINUATION_'+c);};
function validateRecords(prior,diagnostic,approval,prepared){
 check(approval?.authorizationId===PRIOR_ID&&approval.codeSha256===PRIOR_CODE&&approval.scope==='ALM-03-COEXISTENCE'&&['authorized','environmentAuthorized','account','select','storage','runtimeChanges','exclusive','discardable'].every(k=>approval[k]===true),'PRIOR_AUTHORIZATION_CHANGED');
 check(approval.proposalSha256===prepared.proposalSha256&&approval.candidateSha256===prepared.p.candidateSha256&&approval.resourceScopeSha256===prepared.p.resourceScopeSha256&&approval.environment===prepared.p.environment,'PRIOR_SCOPE_CHANGED');
 check(prior?.status==='STOPPED_REVIEW_REQUIRED'&&prior.scope==='ALM-03-COEXISTENCE'&&prior.code==='DOC117_ALM03_CONTROL_NOT_UNIQUE'&&prior.codeSha256===PRIOR_CODE&&prior.proposalSha256===approval.proposalSha256&&prior.candidateSha256===approval.candidateSha256&&prior.originalSha256===prepared.p.originalSha256&&prior.runtimeRestored===true&&prior.selectRequests===32&&Array.isArray(prior.history)&&prior.history.length===0&&!prior.result,'PRIOR_EFFECT_OR_STATE_CHANGED');
 check(diagnostic?.status==='DIAGNOSTIC_COMPLETE'&&diagnostic.authorizationId===PRIOR_ID&&diagnostic.scenario==='ALM-03-INDEX-DIAGNOSTIC'&&diagnostic.priorSelectRequests===32&&diagnostic.selectRequests===3&&diagnostic.cumulativeSelectRequests===35&&diagnostic.storageRequests===0&&diagnostic.applicationLogins===0&&diagnostic.runtimeUnchanged===true&&diagnostic.proof?.defaultRelationCount===0&&diagnostic.proof.relationCount===0&&diagnostic.proof.dateMappingCount===0,'DIAGNOSTIC_CHANGED');return 35;
}
function priorReads(continuation,prepared){
 if(continuation===undefined){check(!fs.existsSync(path.join(ROOT,FILES.priorArtifact)),'PROOF_REQUIRED');return 0;}
 check(continuation&&Object.keys(continuation).sort().join(',')==='authorizationSha256,diagnosticSha256,priorArtifactSha256','PROOF_REQUIRED');
 const records={},hashes={};for(const [key,file]of Object.entries(FILES)){const full=path.join(ROOT,file);let current=full;while(current!==ROOT){check(!fs.lstatSync(current).isSymbolicLink(),'PATH_CHANGED');current=path.dirname(current);}const bytes=fs.readFileSync(full);hashes[key]=sha(bytes);check(hashes[key]===continuation[key+'Sha256'],'PROOF_CHANGED');records[key]=JSON.parse(bytes.toString('utf8').replace(/^\uFEFF/,''));}
 check(records.diagnostic.priorArtifact===FILES.priorArtifact&&records.diagnostic.priorArtifactSha256===hashes.priorArtifact,'DIAGNOSTIC_BASE_CHANGED');
 return validateRecords(records.priorArtifact,records.diagnostic,records.authorization,prepared);
}
module.exports={FILES,PRIOR_ID,PRIOR_CODE,validateRecords,priorReads};
