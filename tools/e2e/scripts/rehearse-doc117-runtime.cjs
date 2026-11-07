'use strict';
// Byte-copy rehearsal under .tmp only. This does not load or deploy the DLL.
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto'),{spawn}=require('node:child_process');
const {ROOT,DLL,sha,validateRehearsal,createRehearsalLease}=require('./support/doc117-runtime-lease.cjs');
const check=(v,c)=>{if(!v)throw Error('DOC117_REHEARSAL_'+c);},delay=ms=>new Promise(resolve=>setTimeout(resolve,ms));
async function owner(file){
 const m=validateRehearsal(JSON.parse(fs.readFileSync(file,'utf8'))),lease=createRehearsalLease(m);lease.backup();
 const guard=spawn(process.execPath,[path.join(__dirname,'support/doc117-runtime-lease.cjs'),'--watch-rehearsal',file,String(process.pid)],{detached:true,windowsHide:true,stdio:'ignore',env:Object.fromEntries(Object.entries(process.env).filter(([k])=>['PATH','Path','SystemRoot','WINDIR','TEMP','TMP','ComSpec','PATHEXT'].includes(k)))});guard.unref();
 let ready=false;for(let i=0;i<80;i++){const state=JSON.parse(fs.readFileSync(m.leaseJournal,'utf8'));if(state.stage==='GUARD_READY'&&state.guardPid===guard.pid&&state.ownerPid===process.pid){ready=true;break;}await delay(50);}check(ready,'GUARD_NOT_READY');
 lease.activate();check(lease.current()===m.candidateSha256,'ACTIVATION_NOT_OBSERVED');
 process.send?.({candidateObserved:true,ownerPid:process.pid,guardPid:guard.pid});
 // Deliberately exit without finally/restore. The independent guardian owns recovery.
 process.exit(0);
}
async function rehearse(buildDirectory){
 const buildPath=path.resolve(buildDirectory),relative=path.relative(path.join(ROOT,'.tmp'),buildPath);check(relative&&!relative.startsWith('..')&&!path.isAbsolute(relative),'BUILD_PATH_INVALID');
 let cursor=buildPath;while(cursor!==ROOT){check(!fs.lstatSync(cursor).isSymbolicLink(),'BUILD_PATH_INVALID');cursor=path.dirname(cursor);}
 const build=JSON.parse(fs.readFileSync(path.join(buildPath,'build-result.json'),'utf8')),original=fs.readFileSync(path.join(ROOT,'bin',DLL)),candidate=fs.readFileSync(path.join(buildPath,'obj',DLL));
 check(['COMPILED_DORMANT_CANDIDATE','COMPILED_SCOPED_NOT_INSTALLED'].includes(build.status)&&build.deployed===false&&build.executionAuthorized===false&&build.originalSha256===sha(original)&&build.candidateSha256===sha(candidate),'BUILD_CHANGED');
 const runId=crypto.randomBytes(16).toString('hex'),dir=path.join(ROOT,'.tmp','doc117-alm02-lease-'+runId);
 const m={issueKey:'DOC-117',scenario:'ALM-02-INTERNAL',mode:'REHEARSAL',runId,authorization:false,runtimeActivationAuthorized:false,effectsAuthorized:false,maximumRuntimeSeconds:600,originalSha256:sha(original),candidateSha256:sha(candidate),runtimeRoot:path.join(dir,'site'),candidatePath:path.join(dir,'candidate',DLL),backupPath:path.join(dir,'backup','original.dll'),reserveBackupPath:path.join(dir,'reserve','original.dll'),leaseJournal:path.join(dir,'lease.json')};validateRehearsal(m);
 fs.mkdirSync(path.join(m.runtimeRoot,'bin'),{recursive:true});fs.mkdirSync(path.dirname(m.candidatePath));
 const target=path.join(m.runtimeRoot,'bin',DLL),manifest=path.join(dir,'rehearsal-manifest.json');
 fs.writeFileSync(target,original,{flag:'wx'});fs.writeFileSync(m.candidatePath,candidate,{flag:'wx'});fs.writeFileSync(manifest,JSON.stringify(m,null,2)+'\n',{flag:'wx'});
 let observation;
 const child=spawn(process.execPath,[__filename,'--owner',manifest],{cwd:ROOT,windowsHide:true,stdio:['ignore','ignore','ignore','ipc'],env:Object.fromEntries(Object.entries(process.env).filter(([k])=>['PATH','Path','SystemRoot','WINDIR','TEMP','TMP','ComSpec','PATHEXT'].includes(k)))});
 await new Promise((resolve,reject)=>{child.on('message',message=>{observation=message;});child.once('error',()=>reject(Error('DOC117_REHEARSAL_OWNER_NOT_STARTED')));child.once('exit',code=>code===0?resolve():reject(Error('DOC117_REHEARSAL_OWNER_FAILED')));});
 check(observation?.candidateObserved===true&&observation.ownerPid===child.pid&&observation.guardPid!==child.pid,'OWNER_OBSERVATION_MISSING');
 let restored=false;for(let i=0;i<80;i++){const state=JSON.parse(fs.readFileSync(m.leaseJournal,'utf8'));if(state.restored===true&&sha(fs.readFileSync(target))===m.originalSha256){restored=true;break;}await delay(100);}check(restored,'RESTORATION_NOT_OBSERVED');
 check([target,m.backupPath,m.reserveBackupPath].every(f=>fs.readFileSync(f).equals(original)),'RESTORATION_BYTES_CHANGED');
 check(sha(fs.readFileSync(path.join(ROOT,'bin',DLL)))===m.originalSha256,'LIVE_RUNTIME_CHANGED');
 const result={issueKey:'DOC-117',scenario:'ALM-02-RUNTIME-REHEARSAL',status:'PASS',runId,originalSha256:m.originalSha256,candidateSha256:m.candidateSha256,independentGuardian:true,ownerExitedWithoutRestore:true,candidateObservedInTemporarySite:true,temporaryRuntimeRestored:true,bothBackupsByteIdentical:true,windowsAclTransferVerified:process.platform==='win32',liveRuntimeUnchanged:true,liveRuntimeActivation:false,authenticatedExecution:false,effectsAuthorized:false,maximumRuntimeSeconds:600,scope:'Byte-copy rehearsal in a disposable .tmp site; no IIS/HTTP/database, DLL loading or activation in the live site'};
 fs.writeFileSync(path.join(dir,'rehearsal-result.json'),JSON.stringify(result,null,2)+'\n',{flag:'wx'});return result;
}
if(require.main===module){const args=process.argv.slice(2);Promise.resolve().then(async()=>{
 if(args.length===2&&args[0]==='--owner')return owner(args[1]);
 check(args.length===2&&args[0]==='--candidate-build','ARGUMENT_INVALID');console.log(JSON.stringify(await rehearse(args[1])));
}).catch(()=>{process.stderr.write('DOC117_RUNTIME_REHEARSAL_REQUIRES_REVIEW\n');process.exitCode=2;});}
module.exports={rehearse};
