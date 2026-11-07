'use strict';
const fs=require('node:fs'),path=require('node:path');
const {ROOT,scopeDigest,validateScope}=require('./doc117-alm03-guard-diagnostic.cjs');
const {sha,copyWindowsAcl,createRuntimeLease,watchRuntimeLease}=require('./laboratory-runtime-lease.cjs');
const DLL='GestionDocumental-Docuarchi.net.dll',check=(v,c)=>{if(!v)throw Error('DOC117_ALM03_PROBE_RUNTIME_'+c);};
function validateManifest(m){
 check(m?.issueKey==='DOC-117'&&m.scenario==='ALM-03-GUARD-DIAGNOSTIC'&&m.mode==='LIVE'&&m.authorization===true&&m.runtimeActivationAuthorized===true&&m.temporaryUploadAuthorized===true&&m.effectsAuthorized===false&&m.maximumRuntimeSeconds===600,'AUTHORIZATION_REQUIRED');
 const s=validateScope(m.resourceScope);check(s.runId===m.runId&&scopeDigest(s)===m.resourceScopeSha256&&m.runtimeRoot===ROOT&&[m.originalSha256,m.candidateSha256].every(h=>/^[a-f0-9]{64}$/.test(h))&&m.originalSha256!==m.candidateSha256,'SCOPE_CHANGED');
 const dir=path.join(ROOT,'.tmp','doc117-alm03-guard-probe-lease-'+m.runId),expected={candidatePath:path.join(dir,'candidate',DLL),backupPath:path.join(dir,'backup','original.dll'),reserveBackupPath:path.join(dir,'reserve','original.dll'),leaseJournal:path.join(dir,'lease.json')};check(Object.entries(expected).every(([k,v])=>m[k]===v),'PATH_CHANGED');return m;
}
const createLease=(m,io,alive,acl=copyWindowsAcl)=>createRuntimeLease(validateManifest(m),{root:ROOT,target:path.join(ROOT,'bin',DLL),errorPrefix:'DOC117_ALM03_PROBE_RUNTIME_',tag:'doc117-guard-probe'},io,alive,acl);
if(require.main===module){const [mode,file,owner,...extra]=process.argv.slice(2);Promise.resolve().then(async()=>{check(extra.length===0&&typeof file==='string','ARGUMENT_INVALID');const m=validateManifest(JSON.parse(fs.readFileSync(file,'utf8'))),lease=createLease(m);if(mode==='--watch')return watchRuntimeLease(m,owner,lease,{errorPrefix:'DOC117_ALM03_PROBE_RUNTIME_'});if(mode==='--restore'&&!owner)return lease.restore();throw Error('DOC117_ALM03_PROBE_RUNTIME_ARGUMENT_INVALID');}).catch(()=>{console.error('DOC117_ALM03_PROBE_RUNTIME_REQUIRES_REVIEW');process.exitCode=2;});}
module.exports={ROOT,DLL,sha,validateManifest,createLease};
