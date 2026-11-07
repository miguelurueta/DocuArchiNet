'use strict';
const syntheticHistory=require('./support/doc117-history-fixture.cjs').install();
const {test}=require('node:test'),assert=require('node:assert/strict');
const {loadPrepared,validateGrant}=require('../tools/e2e/scripts/run-doc117-alm02-internal.cjs');
test('Internal loader verifies a closed synthetic scope/candidate without activation and rejects setup permission',()=>{
 const s=loadPrepared(),code='a'.repeat(64),a={scope:'ALM-02-INTERNAL',environment:s.p.environment,authorized:true,environmentAuthorized:true,account:true,select:true,storage:true,faultInjection:true,runtimeChanges:true,exclusive:true,discardable:true,proposalSha256:s.proposalSha256,resourceScopeSha256:s.p.resourceScopeSha256,candidateSha256:s.p.candidateSha256,codeSha256:code,authorizationId:'b'.repeat(32)};
 assert.equal(s.p.lotId,'21');assert.doesNotThrow(()=>validateGrant(a,s,code));
 for(const key of ['authorized','environmentAuthorized','account','select','storage','faultInjection','runtimeChanges','exclusive','discardable'])assert.throws(()=>validateGrant({...a,[key]:false},s,code),/SPECIFIC_AUTHORIZATION_REQUIRED/);
 for(const changed of [{scope:'ALM-02-SETUP'},{proposalSha256:'c'.repeat(64)},{resourceScopeSha256:'c'.repeat(64)},{candidateSha256:'c'.repeat(64)},{codeSha256:'c'.repeat(64)}])assert.throws(()=>validateGrant({...a,...changed},s,code),/SPECIFIC_AUTHORIZATION_REQUIRED/);
});
