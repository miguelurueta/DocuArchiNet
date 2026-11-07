'use strict';
// ALM-03 authorization policy; only byte/ACL/guardian mechanics are shared.
const fs=require('node:fs'),path=require('node:path');
const {ROOT,scopeDigest}=require('./doc117-alm03-scoped-candidate.cjs');
const {validateScope}=require('../../tests/support/doc117-alm03-setup-scope.cjs');
const {sha,copyWindowsAcl,createRuntimeLease,watchRuntimeLease}=require('./laboratory-runtime-lease.cjs');
const DLL='GestionDocumental-Docuarchi.net.dll',check=(v,c)=>{if(!v)throw Error('DOC117_ALM03_RUNTIME_'+c);};
function validateManifest(m){
 check(m?.issueKey==='DOC-117'&&m.scenario==='ALM-03-COEXISTENCE'&&m.mode==='LIVE'&&['authorization','runtimeActivationAuthorized','effectsAuthorized'].every(k=>m[k]===true),'SPECIFIC_AUTHORIZATION_REQUIRED');
 check(/^[a-f0-9]{32}$/.test(m.runId)&&[m.originalSha256,m.candidateSha256].every(h=>/^[a-f0-9]{64}$/.test(h))&&m.originalSha256!==m.candidateSha256&&m.maximumRuntimeSeconds===600,'SCOPE_INVALID');
 check(m.environment==='https://localhost/GestionDocumental-Docuarchi.net/'&&m.actorRef==='141'&&m.ambitoRef==='2'&&m.levelRef==='7'&&m.individualLevelRef==='8'&&m.versionRef==='38'&&m.cabinetRef==='10'&&m.lotId==='22','SCOPE_INVALID');
 const s=validateScope(m.resourceScope);check(s.runId===m.runId&&s.lotId===m.lotId&&scopeDigest(s)===m.resourceScopeSha256&&s.resources.map(r=>[r.rootId,r.fileId,r.expedienteId,r.relacionNivelId].join('/')).join(',')==='35/36/666/1091,36/37/667/1092','RESOURCES_CHANGED');
 const run=path.join(ROOT,'.tmp','doc117-alm03-live-lease-'+m.runId),expected={runtimeRoot:ROOT,candidatePath:path.join(run,'candidate',DLL),backupPath:path.join(run,'backup','original.dll'),reserveBackupPath:path.join(run,'reserve','original.dll'),leaseJournal:path.join(run,'lease.json')};
 check(Object.entries(expected).every(([k,v])=>m[k]===v),'PATH_INVALID');return m;
}
function createLease(m,io,alive,acl=copyWindowsAcl){return createRuntimeLease(validateManifest(m),{root:ROOT,target:path.join(ROOT,'bin',DLL),errorPrefix:'DOC117_ALM03_RUNTIME_',tag:'doc117-alm03'},io,alive,acl);}
if(require.main===module){const [mode,file,owner,...extra]=process.argv.slice(2);Promise.resolve().then(async()=>{
 check(extra.length===0&&typeof file==='string','ARGUMENT_INVALID');const m=validateManifest(JSON.parse(fs.readFileSync(file,'utf8'))),lease=createLease(m);
 if(mode==='--watch')return watchRuntimeLease(m,owner,lease,{errorPrefix:'DOC117_ALM03_RUNTIME_'});
 if(mode==='--restore'&&!owner)return lease.restore();throw Error('DOC117_ALM03_RUNTIME_ARGUMENT_INVALID');
}).catch(()=>{process.stderr.write('DOC117_ALM03_RUNTIME_GUARD_REQUIRES_REVIEW\n');process.exitCode=2;});}
module.exports={ROOT,DLL,sha,validateManifest,createLease};
