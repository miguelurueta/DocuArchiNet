'use strict';
const {test}=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const {ROOT,DLL,sha,validateManifest,validateRehearsal,createLease,createRehearsalLease}=require('../tools/e2e/scripts/support/doc117-runtime-lease.cjs');
const {watchRuntimeLease}=require('../tools/e2e/scripts/support/laboratory-runtime-lease.cjs');
const {PROTOCOL,scopeDigest}=require('../tools/e2e/scripts/support/doc117-alm02-candidate.cjs');
function fixture(){
 const runId=crypto.randomBytes(16).toString('hex'),dir=path.join(ROOT,'.tmp','doc117-alm02-lease-'+runId),target=path.join(dir,'site/bin',DLL);
 const m={issueKey:'DOC-117',scenario:'ALM-02-INTERNAL',mode:'REHEARSAL',runId,authorization:false,runtimeActivationAuthorized:false,effectsAuthorized:false,maximumRuntimeSeconds:600,originalSha256:sha('original'),candidateSha256:sha('candidate'),runtimeRoot:path.join(dir,'site'),candidatePath:path.join(dir,'candidate',DLL),backupPath:path.join(dir,'backup/original.dll'),reserveBackupPath:path.join(dir,'reserve/original.dll'),leaseJournal:path.join(dir,'lease.json')};
 fs.mkdirSync(path.dirname(target),{recursive:true});fs.mkdirSync(path.dirname(m.candidatePath));fs.writeFileSync(target,'original');fs.writeFileSync(m.candidatePath,'candidate');
 const lease=createRehearsalLease(m,fs,()=>true,()=>{}),ready=()=>fs.writeFileSync(m.leaseJournal,JSON.stringify({stage:'GUARD_READY',ownerPid:process.pid,guardPid:process.pid+1000}));
 return{m,target,lease,ready,journal:()=>JSON.parse(fs.readFileSync(m.leaseJournal,'utf8'))};
}
test('DOC117 rehearsal restores exact bytes from two backups and leaves the live DLL intact',()=>{
 const live=path.join(ROOT,'bin',DLL),before=sha(fs.readFileSync(live)),f=fixture();f.lease.backup();f.ready();f.lease.activate();assert.equal(fs.readFileSync(f.target,'utf8'),'candidate');f.lease.restore();assert.equal(f.journal().restored,true);
 for(const file of [f.target,f.m.backupPath,f.m.reserveBackupPath])assert.equal(fs.readFileSync(file,'utf8'),'original');assert.equal(sha(fs.readFileSync(live)),before);
});
test('DOC117 refuses missing independent guard and changed candidate or backup',()=>{
 const f=fixture();f.lease.backup();assert.throws(()=>f.lease.activate(),/GUARD_NOT_READY/);f.ready();fs.writeFileSync(f.m.candidatePath,'different');assert.throws(()=>f.lease.activate(),/SOURCE_CHANGED/);
 fs.writeFileSync(f.m.candidatePath,'candidate');fs.writeFileSync(f.m.reserveBackupPath,'different');assert.throws(()=>f.lease.activate(),/BACKUP_CHANGED/);assert.equal(fs.readFileSync(f.target,'utf8'),'original');
});
test('DOC117 guardian recovers from reserve and refuses a foreign runtime or two corrupted backups',()=>{
 const f=fixture();f.lease.backup();f.ready();f.lease.activate();fs.writeFileSync(f.m.backupPath,'corrupted');f.lease.restore();assert.equal(fs.readFileSync(f.target,'utf8'),'original');
 const g=fixture();g.lease.backup();g.ready();g.lease.activate();fs.writeFileSync(g.target,'external');assert.throws(()=>g.lease.restore(),/EXTERNAL_CHANGE/);assert.equal(fs.readFileSync(g.target,'utf8'),'external');
 fs.writeFileSync(g.m.backupPath,'corrupted');fs.writeFileSync(g.m.reserveBackupPath,'corrupted');assert.throws(()=>g.lease.restore(),/BACKUP_CHANGED/);
});
test('DOC117 guardian restores at 600 seconds even if its owner is alive (injected clock)',async()=>{
 const f=fixture();f.lease.backup();f.ready();f.lease.activate();let clock=0,aliveChecks=0;
 await watchRuntimeLease(f.m,String(process.pid),f.lease,{errorPrefix:'DOC117_RUNTIME_',now:()=>clock,alive:()=>{aliveChecks++;return true;},readJournal:f.journal,delay:async()=>{clock=600000;}});
 assert.equal(aliveChecks,2);assert.equal(clock,600000);assert.equal(f.journal().restored,true);assert.equal(fs.readFileSync(f.target,'utf8'),'original');
});
test('DOC117 guardian restores when its owner dies without waiting for the deadline',async()=>{
 const f=fixture();f.lease.backup();f.ready();f.lease.activate();await watchRuntimeLease(f.m,String(process.pid),f.lease,{errorPrefix:'DOC117_RUNTIME_',alive:()=>false,readJournal:f.journal,delay:async()=>assert.fail('No delay required')});assert.equal(f.journal().restored,true);
});
test('DOC117 rehearsal cannot target the live site or inherit DOC116 authorization',()=>{
 const f=fixture();assert.throws(()=>validateRehearsal({...f.m,runtimeRoot:ROOT}),/PATH_INVALID/);assert.throws(()=>createLease(f.m),/AUTHORIZATION_REQUIRED/);assert.throws(()=>validateRehearsal({...f.m,issueKey:'DOC-116'}),/SCOPE_INVALID/);assert.throws(()=>validateRehearsal({...f.m,authorization:true}),/REHEARSAL_SCOPE_INVALID/);
 for(const key of ['candidatePath','backupPath','reserveBackupPath','leaseJournal'])assert.throws(()=>validateRehearsal({...f.m,[key]:path.join(ROOT,'bin/foreign.dll')}),/PATH_INVALID/);
 assert.throws(()=>validateRehearsal({...f.m,maximumRuntimeSeconds:601}),/SCOPE_INVALID/);
});
test('DOC117 live gate requires its own closed resource scope and three separate grants',()=>{
 const f=fixture(),m={...f.m,mode:'LIVE',runtimeRoot:ROOT,authorization:true,runtimeActivationAuthorized:true,effectsAuthorized:true,environment:'https://localhost/GestionDocumental-Docuarchi.net/',actorRef:'141',ambitoRef:'2',levelRef:'7',versionRef:'38',cabinetRef:'10',lotId:'900'};
 m.resourceScope={issueKey:'DOC-117',protocol:PROTOCOL,runtimeRoot:ROOT,runId:m.runId,lotId:'900',snapshotSha256:'f'.repeat(64),resources:[{rootId:'901',fileId:'902',expedienteId:'910',relacionNivelId:'911',fileName:'a.pdf',bytes:'677',contentSha256:'d'.repeat(64),requestSha256:'a'.repeat(64)},{rootId:'904',fileId:'905',expedienteId:'912',relacionNivelId:'913',fileName:'b.pdf',bytes:'677',contentSha256:'d'.repeat(64),requestSha256:'b'.repeat(64)}]};
 m.resourceScopeSha256=scopeDigest(m.resourceScope);
 assert.doesNotThrow(()=>validateManifest(m));for(const key of ['authorization','runtimeActivationAuthorized','effectsAuthorized'])assert.throws(()=>validateManifest({...m,[key]:false}),/AUTHORIZATION_REQUIRED/);
 assert.throws(()=>validateManifest({...m,issueKey:'DOC-116'}),/SCOPE_INVALID/);assert.throws(()=>validateManifest({...m,resourceScopeSha256:'a'.repeat(64)}),/RESOURCES_CHANGED/);assert.throws(()=>validateManifest({...m,lotId:'18'}),/RESOURCES_REQUIRED/);
 const changed=structuredClone(m);changed.resourceScope.resources[0].requestSha256='c'.repeat(64);assert.throws(()=>validateManifest(changed),/RESOURCES_CHANGED/);
 const predicted=structuredClone(m);predicted.resourceScope.resources[0].attemptId='999';assert.throws(()=>validateManifest(predicted),/RESOURCES_REQUIRED/);
});
