'use strict';
const fs=require('node:fs'),path=require('node:path');
const {sha,copyWindowsAcl,createRuntimeLease,watchRuntimeLease}=require('./laboratory-runtime-lease.cjs');
const {validateScope,scopeDigest}=require('./doc117-alm02-candidate.cjs');
const ROOT=path.resolve(__dirname,'../../../..'),DLL='GestionDocumental-Docuarchi.net.dll';
const check=(ok,code)=>{if(!ok)throw Error('DOC117_RUNTIME_'+code);};
const id=x=>typeof x==='string'&&/^[1-9][0-9]{0,9}$/.test(x)&&BigInt(x)<=4294967295n;
function validatePaths(m,mode){
 check(m?.issueKey==='DOC-117'&&m.scenario==='ALM-02-INTERNAL'&&m.mode===mode&&/^[a-f0-9]{32}$/.test(m.runId),'SCOPE_INVALID');
 check([m.originalSha256,m.candidateSha256].every(h=>/^[a-f0-9]{64}$/.test(h))&&m.originalSha256!==m.candidateSha256&&m.maximumRuntimeSeconds===600,'SCOPE_INVALID');
 const run=path.join(ROOT,'.tmp','doc117-alm02-lease-'+m.runId);
 const wanted={candidatePath:path.join(run,'candidate',DLL),backupPath:path.join(run,'backup','original.dll'),reserveBackupPath:path.join(run,'reserve','original.dll'),leaseJournal:path.join(run,'lease.json'),runtimeRoot:mode==='LIVE'?ROOT:path.join(run,'site')};
 for(const [key,value]of Object.entries(wanted))check(m[key]===value,'PATH_INVALID');
 return m;
}
function validateManifest(m){
 check(m?.authorization===true&&m.runtimeActivationAuthorized===true&&m.effectsAuthorized===true,'AUTHORIZATION_REQUIRED');validatePaths(m,'LIVE');
 check(m.environment==='https://localhost/GestionDocumental-Docuarchi.net/'&&m.actorRef==='141'&&m.ambitoRef==='2'&&m.levelRef==='7'&&m.versionRef==='38'&&m.cabinetRef==='10','SCOPE_INVALID');
 let scope;try{scope=validateScope(m.resourceScope);}catch{throw Error('DOC117_RUNTIME_RESOURCES_REQUIRED');}
 check(scope.runtimeRoot===ROOT&&scope.runId===m.runId&&scope.lotId===m.lotId,'RESOURCES_REQUIRED');
 check(m.resourceScopeSha256===scopeDigest(scope),'RESOURCES_CHANGED');return m;
}
function validateRehearsal(m){
 validatePaths(m,'REHEARSAL');check(m.authorization===false&&m.runtimeActivationAuthorized===false&&m.effectsAuthorized===false,'REHEARSAL_SCOPE_INVALID');return m;
}
function copyAcl(source,destination){try{return copyWindowsAcl(source,destination);}catch{throw Error('DOC117_RUNTIME_ACL_NOT_PRESERVED');}}
function compose(m,io,alive,preserveAcl){
 return createRuntimeLease(m,{root:ROOT,target:path.join(m.runtimeRoot,'bin',DLL),errorPrefix:'DOC117_RUNTIME_',tag:'doc117'},io,alive,preserveAcl);
}
function createLease(input,io,alive,preserveAcl=copyAcl){return compose(validateManifest(input),io,alive,preserveAcl);}
function createRehearsalLease(input,io,alive,preserveAcl=copyAcl){return compose(validateRehearsal(input),io,alive,preserveAcl);}
async function watch(file,owner,rehearsal=false){
 const m=JSON.parse(fs.readFileSync(file,'utf8')),lease=rehearsal?createRehearsalLease(m):createLease(m);
 return watchRuntimeLease(m,owner,lease,{errorPrefix:'DOC117_RUNTIME_',readyMarker:'DOC117_RUNTIME_GUARD_READY'});
}
if(require.main===module){const [mode,file,owner,...extra]=process.argv.slice(2);Promise.resolve().then(()=>{
 check(extra.length===0&&typeof file==='string','ARGUMENT_INVALID');
 if(mode==='--watch')return watch(file,owner);
 if(mode==='--watch-rehearsal')return watch(file,owner,true);
 if(mode==='--restore'&&!owner)return createLease(JSON.parse(fs.readFileSync(file,'utf8'))).restore();
 if(mode==='--restore-rehearsal'&&!owner)return createRehearsalLease(JSON.parse(fs.readFileSync(file,'utf8'))).restore();
 throw Error('DOC117_RUNTIME_ARGUMENT_INVALID');
}).catch(()=>{process.stderr.write('DOC117_RUNTIME_GUARD_REQUIRES_REVIEW\n');process.exitCode=2;});}
module.exports={ROOT,DLL,sha,validateManifest,validateRehearsal,createLease,createRehearsalLease,copyAcl};
