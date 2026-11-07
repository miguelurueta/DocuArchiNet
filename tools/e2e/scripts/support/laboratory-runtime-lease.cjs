'use strict';
// Shared file mechanics only. Each consumer must validate its own authorization
// and exact paths before calling this engine. No CLI or environment switches.
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const sha=b=>crypto.createHash('sha256').update(b).digest('hex');
const isAlive=pid=>{try{process.kill(pid,0);return true;}catch{return false;}};
function copyWindowsAcl(source,destination){
 if(process.platform!=='win32')return;
 const env=Object.fromEntries(Object.entries(process.env).filter(([k])=>['PATH','Path','SystemRoot','WINDIR','TEMP','TMP','ComSpec','PATHEXT'].includes(k)));
 Object.assign(env,{LAB_ACL_SOURCE:source,LAB_ACL_DESTINATION:destination});
 require('node:child_process').execFileSync('powershell.exe',['-NoProfile','-NonInteractive','-Command',"$ErrorActionPreference='Stop'; Set-Acl -LiteralPath $env:LAB_ACL_DESTINATION -AclObject (Get-Acl -LiteralPath $env:LAB_ACL_SOURCE); if((Get-Acl -LiteralPath $env:LAB_ACL_DESTINATION).Sddl -ne (Get-Acl -LiteralPath $env:LAB_ACL_SOURCE).Sddl){throw 'ACL_NOT_MATCHED'}"],{env,windowsHide:true,stdio:'pipe',timeout:15000});
}
function createRuntimeLease(m,policy,io=fs,alive=isAlive,preserveAcl=copyWindowsAcl){
 const {root,target,errorPrefix,tag}=policy,check=(v,c)=>{if(!v)throw Error(errorPrefix+c);},digest=file=>sha(io.readFileSync(file));
 function noLinks(file){let current=path.dirname(file);while(current!==root){check(current.startsWith(root+path.sep),'PATH_INVALID');if(io.existsSync(current))check(!io.lstatSync(current).isSymbolicLink(),'PATH_INVALID');current=path.dirname(current);}if(io.existsSync(file))check(!io.lstatSync(file).isSymbolicLink(),'PATH_INVALID');}
 function flush(file){const fd=io.openSync(file,'r+');try{io.fsyncSync(fd);}finally{io.closeSync(fd);}}
 function save(stage,extra={}){
  noLinks(m.leaseJournal);const temp=m.leaseJournal+'.'+process.pid+'.tmp';
  const data={version:1,stage,restored:stage==='RESTORED',originalSha256:m.originalSha256,candidateSha256:m.candidateSha256,backupPath:m.backupPath,reserveBackupPath:m.reserveBackupPath,runtimeRoot:m.runtimeRoot,updatedUtc:new Date().toISOString(),...extra};
  io.writeFileSync(temp,JSON.stringify(data,null,2)+'\n',{flag:'wx'});flush(temp);io.renameSync(temp,m.leaseJournal);return data;
 }
 function replace(source,expected,label){
  const temp=path.join(path.dirname(target),'.'+tag+'-'+label+'-'+process.pid+'.tmp');noLinks(temp);check(!io.existsSync(temp),'PREVIOUS_LEASE_REQUIRES_REVIEW');
  io.copyFileSync(source,temp,fs.constants.COPYFILE_EXCL);flush(temp);check(digest(temp)===expected,'SOURCE_CHANGED');preserveAcl(target,temp);io.renameSync(temp,target);
 }
 function verifyBackup(strict=true){
  const copies=[m.backupPath,m.reserveBackupPath];for(const file of copies)noLinks(file);
  const valid=copies.filter(file=>io.existsSync(file)&&digest(file)===m.originalSha256);check(strict?valid.length===2:valid.length>=1,'BACKUP_CHANGED');return valid[0];
 }
 function backup(){
  noLinks(target);noLinks(m.candidatePath);check(!io.existsSync(m.leaseJournal),'PREVIOUS_LEASE_REQUIRES_REVIEW');check(digest(target)===m.originalSha256&&digest(m.candidatePath)===m.candidateSha256,'SOURCE_CHANGED');
  for(const file of [m.backupPath,m.reserveBackupPath]){noLinks(file);if(!io.existsSync(file)){io.mkdirSync(path.dirname(file),{recursive:true});io.copyFileSync(target,file,fs.constants.COPYFILE_EXCL);flush(file);}}
  verifyBackup();return save('BACKED_UP');
 }
 function activate(){
  verifyBackup();noLinks(target);noLinks(m.candidatePath);const guard=JSON.parse(io.readFileSync(m.leaseJournal,'utf8'));
  check(guard.stage==='GUARD_READY'&&guard.ownerPid===process.pid&&Number.isSafeInteger(guard.guardPid)&&guard.guardPid>0&&guard.guardPid!==process.pid&&alive(guard.guardPid),'GUARD_NOT_READY');
  check(digest(target)===m.originalSha256&&digest(m.candidatePath)===m.candidateSha256,'SOURCE_CHANGED');save('ACTIVATION_REQUESTED');replace(m.candidatePath,m.candidateSha256,'activate');check(digest(target)===m.candidateSha256,'ACTIVATION_NOT_CONFIRMED');return save('ACTIVE');
 }
 function restore(){
  const copy=verifyBackup(false);noLinks(target);const current=digest(target);if(current===m.originalSha256)return save('RESTORED');
  if(current!==m.candidateSha256){save('RESTORE_BLOCKED_EXTERNAL_CHANGE');throw Error(errorPrefix+'EXTERNAL_CHANGE');}
  save('RESTORATION_REQUESTED');replace(copy,m.originalSha256,'restore');check(digest(target)===m.originalSha256,'RESTORATION_NOT_CONFIRMED');return save('RESTORED');
 }
 return{backup,activate,restore,verifyBackup,current:()=>digest(target),arm:owner=>save('GUARD_READY',{guardPid:process.pid,ownerPid:Number(owner)})};
}
async function watchRuntimeLease(m,owner,lease,options={}){
 const {errorPrefix,readyMarker}=options,alive=options.alive||isAlive,now=options.now||Date.now,delay=options.delay||(ms=>new Promise(resolve=>setTimeout(resolve,ms))),readJournal=options.readJournal||(()=>JSON.parse(fs.readFileSync(m.leaseJournal,'utf8')));
 if(!/^[1-9][0-9]*$/.test(String(owner)))throw Error(errorPrefix+'OWNER_INVALID');
 lease.verifyBackup();const start=now();lease.arm(owner);if(readyMarker){process.stdout.on('error',()=>{});process.stdout.write(readyMarker+'\n');}
 for(;;){const journal=readJournal();if(journal.restored===true&&lease.current()===m.originalSha256)return;
  if(!alive(Number(owner))||now()-start>=m.maximumRuntimeSeconds*1000){lease.restore();return;}
  await delay(500);
 }
}
module.exports={sha,isAlive,copyWindowsAcl,createRuntimeLease,watchRuntimeLease};
