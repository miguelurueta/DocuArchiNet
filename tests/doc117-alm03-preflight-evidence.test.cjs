'use strict';
const syntheticHistory=require('./support/doc117-history-fixture.cjs').install();
const {test}=require('node:test'),assert=require('node:assert/strict');
const {preflightFailure}=require('../tools/e2e/scripts/support/doc117-alm03-preflight-evidence.cjs');
test('preflight keeps only explicit login and level codes, never arbitrary error details',()=>{
 for(const code of ['DOC111_LOGIN_TRANSPORT_FAILED','DOC111_LOGIN_CREDENTIALS_REJECTED','DOC111_SESSION_NOT_ESTABLISHED','DOC111_PRODUCTION_OPEN_FAILED','DOC116_LEVEL_SELECTION_FAILED'])assert.equal(preflightFailure(Error(code)).code,code);
 const error=Object.assign(Error('PRIVATE_PASSWORD'),{code:'DOC111_LOGIN_PRIVATE_SECRET',stack:'PRIVATE_STACK',cause:'PRIVATE_COOKIE',body:'PRIVATE_BODY',operation:'PRIVATE_URL'});assert.equal(preflightFailure(error).code,'UNCLASSIFIED_FAILURE');assert.ok(!JSON.stringify(preflightFailure(error)).includes('PRIVATE'));
 assert.equal(preflightFailure(Object.defineProperty({},'code',{get(){throw Error('PRIVATE');}})).code,'UNCLASSIFIED_FAILURE');
});
test('preflight preserves sanitized login observations and only the read-only lot failure metadata',()=>{
 const observation={postbackStatus:200,productionStatus:401,probeStatus:null,finish:'COMPLETE',body:'AVAILABLE',unavailableAt:'NONE',category:'NONE',postedFields:'MATCHED',password:'PRIVATE_PASSWORD',cookie:'PRIVATE_COOKIE'},error=Object.assign(Error('DOC111_OPERATION_NOT_CONFIRMED'),{loginObservation:observation,operation:'ConsultarLote',httpStatus:403,serverCode:'ACCESO_DENEGADO'}),result=preflightFailure(error);
 assert.equal(result.loginObservation.postbackStatus,200);assert.equal(result.loginObservation.postedFields,'MATCHED');assert.equal(result.operation,'ConsultarLote');assert.equal(result.serverCode,'ACCESO_DENEGADO');assert.equal(result.httpStatus,403);assert.ok(!JSON.stringify(result).includes('PRIVATE'));
 const invalid=preflightFailure({loginObservation:{...observation,postedFields:'PRIVATE'},operation:'ConsultarLote',httpStatus:'PRIVATE',serverCode:'PRIVATE'});assert.equal(invalid.loginObservation,null);assert.equal(invalid.httpStatus,null);assert.equal(invalid.serverCode,null);
});
test('diagnostic code correction invalidates the previous real grant without changing native candidate or scope',()=>{
 const fs=require('node:fs'),path=require('node:path'),runner=require('../tools/e2e/scripts/run-doc117-alm03-guard-diagnostic.cjs'),prepared=runner.loadPrepared(),approval=JSON.parse(fs.readFileSync(path.join(__dirname,'../.tmp/doc117-alm03-guard-probe-authorization.json'),'utf8'));
 assert.notEqual(approval.codeSha256,runner.codeFingerprint());assert.equal(approval.candidateSha256,prepared.p.candidateSha256);assert.equal(approval.resourceScopeSha256,prepared.p.resourceScopeSha256);assert.doesNotThrow(()=>runner.validateGrant({...approval,codeSha256:runner.codeFingerprint()},prepared,runner.codeFingerprint()));assert.throws(()=>runner.validateGrant(approval,prepared,runner.codeFingerprint()),/SPECIFIC_AUTHORIZATION_REQUIRED/);
});
