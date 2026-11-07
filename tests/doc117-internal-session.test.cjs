'use strict';
const {test}=require('node:test'),assert=require('node:assert/strict');
const {executeSession}=require('../tools/e2e/scripts/support/doc117-internal-session.cjs');
function fixture(failure){
 const events=[];let current='original',restores=0;
 const lease={backup:()=>events.push('backup'),activate:()=>{events.push('activate');current='candidate';if(failure==='activation')throw Error('FAIL_ACTIVATE');},restore:()=>{restores++;events.push('restore');if(failure==='restore')throw Error('FAIL_RESTORE');current='original';},verifyBackup:()=>events.push('verifyBackups'),current:()=>current};
 const stage=(name,result)=>async()=>{events.push(name);if(failure===name)throw Error('FAIL_'+name);return result;};
 const options={lease,preflightOriginal:stage('preflight',{originalSha256:'original'}),closeSession:stage('close'),startGuard:stage('guard'),openCandidate:stage('candidateLogin'),executeWindows:stage('windows',{status:'PASS'}),reopenOriginal:stage('originalLogin'),verifyAfterRestore:async()=>{events.push('postChecks');assert.equal(current,'original');if(failure==='postChecks')throw Error('FAIL_POSTCHECKS');return true;},onProgress:v=>events.push(v.stage)};
 return{events,options,get current(){return current;},get restores(){return restores;}};
}
test('Internal session restores before re-login/post-checks and verifies restoration again in finally',async()=>{
 const f=fixture(),result=await executeSession(f.options);assert.equal(result.runtimeRestored,true);assert.equal(f.current,'original');assert.equal(f.restores,2);assert.ok(f.events.indexOf('restore')<f.events.indexOf('originalLogin'));
});
for(const failure of ['guard','activation','candidateLogin','windows','originalLogin','postChecks'])test('Internal session restores on '+failure,async()=>{
 const f=fixture(failure);await assert.rejects(executeSession(f.options));assert.equal(f.current,'original');assert.ok(f.restores>=1);if(['guard','activation','candidateLogin'].includes(failure))assert.ok(!f.events.includes('windows'));
});
test('Internal session never grants a PASS when restoration fails',async()=>{
 const f=fixture('restore');await assert.rejects(executeSession(f.options),/RESTORATION_REQUIRES_REVIEW/);assert.notEqual(f.current,'original');assert.ok(f.events.includes('RESTORATION_FAILED'));
});
test('Internal session restores even when closing the candidate session and final cleanup both fail',async()=>{
 const f=fixture();let closes=0;
 f.options.closeSession=async()=>{closes++;if(closes>=2)throw Error('FAIL_CLOSE');};
 await assert.rejects(executeSession(f.options),/FAIL_CLOSE/);
 assert.equal(f.current,'original');assert.equal(f.restores,1);
});
