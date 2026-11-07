'use strict';
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const ROOT=path.resolve(__dirname,'../../../..'),sha=v=>crypto.createHash('sha256').update(v).digest('hex'),check=(v,c)=>{if(!v)throw Error('DOC117_ALM03C_CONTINUATION_'+c);};
function validateRecords(previous,parent,prepared){
 check(previous?.status==='STOPPED_REVIEW_REQUIRED'&&previous.code==='DOC117_ALM03C_PRIOR_INVENTORY_CHANGED'&&previous.priorSelectRequests===250&&previous.selectRequests===3&&previous.cumulativeSelectRequests===253&&previous.failedAt==='SESSION_OPENED'&&!previous.result&&JSON.stringify(previous.references)==='[{"stage":"BROWSER_OPEN_REQUESTED"},{"stage":"SESSION_OPEN_REQUESTED"},{"stage":"SESSION_OPENED"}]','PREVIOUS_EFFECT_OR_COUNT_CHANGED');
 check(parent?.scope==='ALM-03C-SETUP'&&parent.authorized===true&&parent.budgetIncreaseAuthorized===true&&parent.priorSelectRequests===250&&parent.maximumAdditionalSelectRequests===160&&parent.maximumCumulativeSelectRequests===410&&parent.storage===false&&parent.runtimeChanges===false&&parent.individualUpload===false&&parent.proposalSha256===prepared.proposalSha256&&previous.proposalSha256===prepared.proposalSha256&&previous.codeSha256===parent.codeSha256&&parent.hashToolSha256===prepared.p.hashToolSha256&&/^[a-f0-9]{32}$/.test(parent.authorizationId),'PARENT_CHANGED');return 3;
}
function carriedReads(a,prepared){
 if(a.zeroEffectContinuation===undefined)return 0;const c=a.zeroEffectContinuation;
 check(c&&Object.keys(c).sort().join(',')==='parentAuthorizationSha256,previousArtifactSha256','PROOF_REQUIRED');
 const parentFile=path.join(ROOT,'.tmp/doc117-alm03c-setup-authorization.json'),parentBytes=fs.readFileSync(parentFile),parent=JSON.parse(parentBytes.toString('utf8'));check(/^[a-f0-9]{32}$/.test(parent.authorizationId),'PARENT_CHANGED');const previousFile=path.join(ROOT,'tools/e2e/artifacts/doc117-alm03c-setup-'+parent.authorizationId+'.json'),previousBytes=fs.readFileSync(previousFile),previous=JSON.parse(previousBytes.toString('utf8'));
 check(sha(parentBytes)===c.parentAuthorizationSha256&&sha(previousBytes)===c.previousArtifactSha256,'PROOF_CHANGED');
 for(const suffix of ['-ready.runtime.json','-scope.json'])check(!fs.existsSync(path.join(ROOT,'.tmp/doc117-alm03c-setup-'+parent.authorizationId+suffix)),'PREVIOUS_RESOURCE_SCOPE_EXISTS');return validateRecords(previous,parent,prepared);
}
module.exports={validateRecords,carriedReads};
