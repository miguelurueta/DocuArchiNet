'use strict';
const syntheticHistory=require('./support/doc117-history-fixture.cjs').install();
const {test}=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs'),path=require('node:path');
const {loadPrepared,validateGrant,codeFingerprint,main}=require('../tools/e2e/scripts/run-doc117-alm03-guard-diagnostic.cjs');
test('guard probe checks sealed sources, prior 146 reads and original runtime without authorization or activation',async()=>{
 const p=loadPrepared();assert.equal(p.p.realExecutionAuthorized,false);assert.equal(p.p.runtimeActivationAuthorized,false);assert.equal(p.p.temporaryUploadAuthorized,false);assert.equal(p.p.documents,0);assert.equal(p.p.storageRequests,0);assert.equal(p.s.priorCaseSelectRequests,146);assert.equal(p.s.maximumAdditionalSelectRequests,160);assert.equal(p.s.maximumCumulativeSelectRequests,306);assert.match(p.p.candidateSha256,/^[a-f0-9]{64}$/);
 const digest=target=>{if(!fs.existsSync(target))return null;if(fs.statSync(target).isDirectory())return fs.readdirSync(target).sort().map(name=>[name,digest(path.join(target,name))]);return require('node:crypto').createHash('sha256').update(fs.readFileSync(target)).digest('hex');};
 const targets=[p.run,p.proofDirectory,p.reservationDirectory,p.temporaryFile],before=targets.map(digest);await main(['--check']);assert.deepEqual(targets.map(digest),before);
});
test('guard probe rejects historical coexistence grants and changed scope, code, proposal, binary or effect permission',()=>{
 const p=loadPrepared(),code=codeFingerprint(),a={scope:'ALM-03-GUARD-DIAGNOSTIC',environment:p.p.environment,authorized:true,environmentAuthorized:true,account:true,select:true,temporaryUpload:true,runtimeChanges:true,exclusive:true,discardable:true,storage:false,imports:false,proposalSha256:p.proposalSha256,resourceScopeSha256:p.p.resourceScopeSha256,candidateSha256:p.p.candidateSha256,codeSha256:code,authorizationId:'b'.repeat(32)};
 assert.doesNotThrow(()=>validateGrant(a,p,code));
 for(const key of ['authorized','environmentAuthorized','account','select','temporaryUpload','runtimeChanges','exclusive','discardable'])assert.throws(()=>validateGrant({...a,[key]:false},p,code),/SPECIFIC_AUTHORIZATION_REQUIRED/);
 for(const change of [{scope:'ALM-03-COEXISTENCE'},{scope:'ALM-03-SETUP'},{storage:true},{imports:true},{environment:'https://other.invalid/'},{authorizationId:'x'},{proposalSha256:'c'.repeat(64)},{resourceScopeSha256:'c'.repeat(64)},{candidateSha256:'142e0d0bba0b6bcfd66933158ac163623784a2d0556c1a0f244ec92396b32e12'},{codeSha256:'c'.repeat(64)}])assert.throws(()=>validateGrant({...a,...change},p,code),/SPECIFIC_AUTHORIZATION_REQUIRED/);
});
