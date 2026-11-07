'use strict';
const {test}=require('node:test'),assert=require('node:assert/strict'),path=require('node:path');
const {ROOT}=require('../tools/e2e/scripts/support/doc117-alm03-candidate.cjs');
const {lease}=require('../tools/e2e/scripts/rehearse-doc117-alm03-runtime.cjs');
function manifest(){const runId='f'.repeat(32),dir=path.join(ROOT,'.tmp','doc117-alm03-lease-'+runId);return{issueKey:'DOC-117',scenario:'ALM-03',mode:'REHEARSAL',runId,authorization:false,effectsAuthorized:false,runtimeActivationAuthorized:false,maximumRuntimeSeconds:600,originalSha256:'a'.repeat(64),candidateSha256:'b'.repeat(64),runtimeRoot:path.join(dir,'site'),candidatePath:path.join(dir,'candidate','GestionDocumental-Docuarchi.net.dll'),backupPath:path.join(dir,'backup','original.dll'),reserveBackupPath:path.join(dir,'reserve','original.dll'),leaseJournal:path.join(dir,'lease.json')};}
test('ALM03 rehearsal has its own scope and cannot accept a live grant or ALM02 identity',()=>{
 assert.doesNotThrow(()=>lease(manifest()));
 for(const changed of [{mode:'LIVE'},{scenario:'ALM-02-INTERNAL'},{authorization:true},{effectsAuthorized:true},{runtimeActivationAuthorized:true},{maximumRuntimeSeconds:601}])assert.throws(()=>lease({...manifest(),...changed}),/SCOPE_INVALID/);
});
test('ALM03 rehearsal refuses real bin, foreign backups and paths outside its own temporary site',()=>{
 for(const changed of [{runtimeRoot:ROOT},{candidatePath:path.join(ROOT,'bin','GestionDocumental-Docuarchi.net.dll')},{backupPath:path.join(ROOT,'.tmp','foreign.dll')},{leaseJournal:path.join(ROOT,'App_Data','lease.json')}])assert.throws(()=>lease({...manifest(),...changed}),/PATH_INVALID/);
});
