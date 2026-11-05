'use strict';
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const ROOT=path.resolve(__dirname,'../../../..'),sha=b=>crypto.createHash('sha256').update(b).digest('hex');
const check=(v,c)=>{if(!v)throw Error('DOC116_RUNTIME_'+c);};
function copyAcl(source,destination){
  if(process.platform!=='win32')return;
  const env=Object.fromEntries(Object.entries(process.env).filter(([k])=>['PATH','Path','SystemRoot','WINDIR','TEMP','TMP','ComSpec','PATHEXT'].includes(k)));
  Object.assign(env,{DOC116_ACL_SOURCE:source,DOC116_ACL_DESTINATION:destination});
  try{require('node:child_process').execFileSync('powershell.exe',['-NoProfile','-NonInteractive','-Command',"$ErrorActionPreference='Stop'; Set-Acl -LiteralPath $env:DOC116_ACL_DESTINATION -AclObject (Get-Acl -LiteralPath $env:DOC116_ACL_SOURCE); if((Get-Acl -LiteralPath $env:DOC116_ACL_DESTINATION).Sddl -ne (Get-Acl -LiteralPath $env:DOC116_ACL_SOURCE).Sddl){throw 'ACL_NOT_MATCHED'}"],{env,windowsHide:true,stdio:'pipe',timeout:15000});}catch{throw Error('DOC116_RUNTIME_ACL_NOT_PRESERVED');}
}
function validatePaths(m){
  check(m?.issueKey==='DOC-116'&&m.scenario==='EXP-02-INTERNAL','SCOPE_INVALID');
  check(m.runtimeRoot===ROOT&&[m.originalSha256,m.candidateSha256].every(x=>/^[a-f0-9]{64}$/.test(x))&&m.originalSha256!==m.candidateSha256,'SCOPE_INVALID');
  for(const field of ['candidatePath','backupPath','reserveBackupPath']){const relative=path.relative(path.join(ROOT,'.tmp'),path.resolve(m[field]||''));check(relative!==''&&!relative.startsWith('..')&&!path.isAbsolute(relative)&&path.isAbsolute(m[field]),'PATH_INVALID');}
  check(new Set([m.candidatePath,m.backupPath,m.reserveBackupPath]).size===3,'PATH_INVALID');
  check(m.leaseJournal===path.join(ROOT,'tools/e2e/artifacts/doc116-internal-runtime-lease.json')&&m.maximumRuntimeSeconds===600,'SCOPE_INVALID');return m;
}
function validateManifest(m){check(m?.authorization===true&&m.runtimeActivationAuthorized===true&&m.effectsAuthorized===true,'AUTHORIZATION_REQUIRED');return validatePaths(m);}
function createLease(input,io=fs,alive=pid=>{try{process.kill(pid,0);return true;}catch{return false;}},preserveAcl=copyAcl){
  const m=validateManifest(input),target=path.join(ROOT,'bin/GestionDocumental-Docuarchi.net.dll');
  const digest=file=>sha(io.readFileSync(file));
  function noLinks(file){let current=path.dirname(file);while(current!==ROOT){check(current.startsWith(ROOT+path.sep),'PATH_INVALID');if(io.existsSync(current))check(!io.lstatSync(current).isSymbolicLink(),'PATH_INVALID');current=path.dirname(current);}if(io.existsSync(file))check(!io.lstatSync(file).isSymbolicLink(),'PATH_INVALID');}
  function flush(file){const fd=io.openSync(file,'r+');try{io.fsyncSync(fd);}finally{io.closeSync(fd);}}
  function save(stage,extra={}){noLinks(m.leaseJournal);const temp=m.leaseJournal+'.'+process.pid+'.tmp';const data={version:1,stage,restored:stage==='RESTORED',originalSha256:m.originalSha256,candidateSha256:m.candidateSha256,backupPath:m.backupPath,reserveBackupPath:m.reserveBackupPath,runtimeRoot:ROOT,updatedUtc:new Date().toISOString(),...extra};io.writeFileSync(temp,JSON.stringify(data,null,2)+'\n',{flag:'wx'});flush(temp);io.renameSync(temp,m.leaseJournal);return data;}
  function replace(source,expected,tag){const temp=path.join(path.dirname(target),'.doc116-'+tag+'-'+process.pid+'.tmp');noLinks(temp);check(!io.existsSync(temp),'PREVIOUS_LEASE_REQUIRES_REVIEW');io.copyFileSync(source,temp,fs.constants.COPYFILE_EXCL);flush(temp);check(digest(temp)===expected,'SOURCE_CHANGED');preserveAcl(target,temp);io.renameSync(temp,target);}
  function verifyBackup(strict=true){const copies=[m.backupPath,m.reserveBackupPath];for(const file of copies)noLinks(file);const valid=copies.filter(file=>io.existsSync(file)&&digest(file)===m.originalSha256);check(strict?valid.length===2:valid.length>=1,'BACKUP_CHANGED');return valid[0];}
  function backup(){noLinks(target);noLinks(m.candidatePath);check(!io.existsSync(m.leaseJournal),'PREVIOUS_LEASE_REQUIRES_REVIEW');check(digest(target)===m.originalSha256&&digest(m.candidatePath)===m.candidateSha256,'SOURCE_CHANGED');for(const file of [m.backupPath,m.reserveBackupPath]){noLinks(file);if(!io.existsSync(file)){io.mkdirSync(path.dirname(file),{recursive:true});io.copyFileSync(target,file,fs.constants.COPYFILE_EXCL);flush(file);}}verifyBackup();return save('BACKED_UP');}
  function activate(){verifyBackup();noLinks(target);noLinks(m.candidatePath);const guard=JSON.parse(io.readFileSync(m.leaseJournal,'utf8'));check(guard.stage==='GUARD_READY'&&guard.ownerPid===process.pid&&Number.isSafeInteger(guard.guardPid)&&guard.guardPid>0&&guard.guardPid!==process.pid&&alive(guard.guardPid),'GUARD_NOT_READY');check(digest(target)===m.originalSha256&&digest(m.candidatePath)===m.candidateSha256,'SOURCE_CHANGED');save('ACTIVATION_REQUESTED');replace(m.candidatePath,m.candidateSha256,'activate');check(digest(target)===m.candidateSha256,'ACTIVATION_NOT_CONFIRMED');return save('ACTIVE');}
  function restore(){const copy=verifyBackup(false);noLinks(target);const current=digest(target);if(current===m.originalSha256)return save('RESTORED');if(current!==m.candidateSha256){save('RESTORE_BLOCKED_EXTERNAL_CHANGE');throw Error('DOC116_RUNTIME_EXTERNAL_CHANGE');}save('RESTORATION_REQUESTED');replace(copy,m.originalSha256,'restore');check(digest(target)===m.originalSha256,'RESTORATION_NOT_CONFIRMED');return save('RESTORED');}
  function current(){return digest(target);}
  return {backup,activate,restore,current,verifyBackup,arm:owner=>save('GUARD_READY',{guardPid:process.pid,ownerPid:Number(owner)})};
}
async function watch(file,owner){
  const m=validateManifest(JSON.parse(fs.readFileSync(file,'utf8'))),lease=createLease(m);check(/^[1-9][0-9]*$/.test(owner),'OWNER_INVALID');lease.verifyBackup();
  const start=Date.now();process.stdout.on('error',()=>{});lease.arm(owner);process.stdout.write('DOC116_RUNTIME_GUARD_READY\n');
  while(true){const journal=JSON.parse(fs.readFileSync(m.leaseJournal,'utf8'));if(journal.restored===true&&lease.current()===m.originalSha256)return;let alive=true;try{process.kill(Number(owner),0);}catch{alive=false;}
    if(!alive||Date.now()-start>=m.maximumRuntimeSeconds*1000){lease.restore();return;}
    await new Promise(resolve=>setTimeout(resolve,500));
  }
}
if(require.main===module){const [mode,file,owner]=process.argv.slice(2);Promise.resolve().then(()=>{if(mode==='--watch')return watch(file,owner);if(mode==='--restore')return createLease(JSON.parse(fs.readFileSync(file,'utf8'))).restore();throw Error('DOC116_RUNTIME_ARGUMENT_INVALID');}).catch(()=>{process.stderr.write('DOC116_RUNTIME_GUARD_REQUIRES_REVIEW\n');process.exitCode=2;});}
module.exports={ROOT,sha,validatePaths,validateManifest,createLease,copyAcl};
