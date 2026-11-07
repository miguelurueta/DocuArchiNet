'use strict';
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const ROOT=path.resolve(__dirname,'../../../..'),sha=b=>crypto.createHash('sha256').update(b).digest('hex');
const check=(v,c)=>{if(!v)throw Error('DOC116_RUNTIME_'+c);};
const {copyWindowsAcl,createRuntimeLease,watchRuntimeLease}=require('./laboratory-runtime-lease.cjs');
function copyAcl(source,destination){try{return copyWindowsAcl(source,destination);}catch{throw Error('DOC116_RUNTIME_ACL_NOT_PRESERVED');}}
function validatePaths(m){
  check(m?.issueKey==='DOC-116'&&m.scenario==='EXP-02-INTERNAL','SCOPE_INVALID');
  check(m.runtimeRoot===ROOT&&[m.originalSha256,m.candidateSha256].every(x=>/^[a-f0-9]{64}$/.test(x))&&m.originalSha256!==m.candidateSha256,'SCOPE_INVALID');
  for(const field of ['candidatePath','backupPath','reserveBackupPath']){const relative=path.relative(path.join(ROOT,'.tmp'),path.resolve(m[field]||''));check(relative!==''&&!relative.startsWith('..')&&!path.isAbsolute(relative)&&path.isAbsolute(m[field]),'PATH_INVALID');}
  check(new Set([m.candidatePath,m.backupPath,m.reserveBackupPath]).size===3,'PATH_INVALID');
  check(m.leaseJournal===path.join(ROOT,'tools/e2e/artifacts/doc116-internal-runtime-lease.json')&&m.maximumRuntimeSeconds===600,'SCOPE_INVALID');return m;
}
function validateManifest(m){check(m?.authorization===true&&m.runtimeActivationAuthorized===true&&m.effectsAuthorized===true,'AUTHORIZATION_REQUIRED');return validatePaths(m);}
function createLease(input,io=fs,alive,preserveAcl=copyAcl){
  return createRuntimeLease(validateManifest(input),{root:ROOT,target:path.join(ROOT,'bin/GestionDocumental-Docuarchi.net.dll'),errorPrefix:'DOC116_RUNTIME_',tag:'doc116'},io,alive,preserveAcl);
}
async function watch(file,owner){
  const m=validateManifest(JSON.parse(fs.readFileSync(file,'utf8')));
  return watchRuntimeLease(m,owner,createLease(m),{errorPrefix:'DOC116_RUNTIME_',readyMarker:'DOC116_RUNTIME_GUARD_READY'});
}
if(require.main===module){const [mode,file,owner]=process.argv.slice(2);Promise.resolve().then(()=>{if(mode==='--watch')return watch(file,owner);if(mode==='--restore')return createLease(JSON.parse(fs.readFileSync(file,'utf8'))).restore();throw Error('DOC116_RUNTIME_ARGUMENT_INVALID');}).catch(()=>{process.stderr.write('DOC116_RUNTIME_GUARD_REQUIRES_REVIEW\n');process.exitCode=2;});}
module.exports={ROOT,sha,validatePaths,validateManifest,createLease,copyAcl};
