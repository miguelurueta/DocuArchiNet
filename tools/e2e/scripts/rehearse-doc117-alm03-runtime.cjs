'use strict';
// ALM-03 byte-copy rehearsal only: no live mode, account or effects grant.
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto'),{spawn}=require('node:child_process');
const {createRuntimeLease,watchRuntimeLease,sha}=require('./support/laboratory-runtime-lease.cjs');
const ROOT=path.resolve(__dirname,'../../..'),DLL='GestionDocumental-Docuarchi.net.dll';
const check=(v,c)=>{if(!v)throw Error('DOC117_ALM03_REHEARSAL_'+c);},delay=ms=>new Promise(r=>setTimeout(r,ms));
const env=()=>Object.fromEntries(Object.entries(process.env).filter(([k])=>['PATH','Path','SystemRoot','WINDIR','TEMP','TMP','ComSpec','PATHEXT'].includes(k)));
function lease(m){
 check(m?.issueKey==='DOC-117'&&m.scenario==='ALM-03'&&m.mode==='REHEARSAL'&&m.authorization===false&&m.effectsAuthorized===false&&m.runtimeActivationAuthorized===false&&m.maximumRuntimeSeconds===600&&/^[a-f0-9]{32}$/.test(m.runId)&&[m.originalSha256,m.candidateSha256].every(h=>/^[a-f0-9]{64}$/.test(h))&&m.originalSha256!==m.candidateSha256,'SCOPE_INVALID');
 const dir=path.join(ROOT,'.tmp','doc117-alm03-lease-'+m.runId);
 const expected={runtimeRoot:path.join(dir,'site'),candidatePath:path.join(dir,'candidate',DLL),backupPath:path.join(dir,'backup','original.dll'),reserveBackupPath:path.join(dir,'reserve','original.dll'),leaseJournal:path.join(dir,'lease.json')};
 check(Object.entries(expected).every(([k,v])=>m[k]===v),'PATH_INVALID');
 return createRuntimeLease(m,{root:ROOT,target:path.join(m.runtimeRoot,'bin',DLL),errorPrefix:'DOC117_ALM03_REHEARSAL_',tag:'doc117-alm03'});
}
async function owner(file){
 const m=JSON.parse(fs.readFileSync(file,'utf8')),l=lease(m);l.backup();
 const guard=spawn(process.execPath,[__filename,'--watch',file,String(process.pid)],{detached:true,windowsHide:true,stdio:'ignore',env:env()});guard.unref();
 let ready=false;for(let i=0;i<80;i++){const j=JSON.parse(fs.readFileSync(m.leaseJournal,'utf8'));if(j.stage==='GUARD_READY'&&j.ownerPid===process.pid&&j.guardPid===guard.pid){ready=true;break;}await delay(50);}check(ready,'GUARD_NOT_READY');
 l.activate();check(l.current()===m.candidateSha256,'CANDIDATE_NOT_OBSERVED');process.send?.({candidateObserved:true,ownerPid:process.pid,guardPid:guard.pid});process.exit(0);
}
async function rehearse(buildDirectory){
 const buildPath=path.resolve(buildDirectory),relative=path.relative(path.join(ROOT,'.tmp'),buildPath);check(relative&&!relative.startsWith('..')&&!path.isAbsolute(relative),'BUILD_PATH_INVALID');
 let cursor=buildPath;while(cursor!==ROOT){check(!fs.lstatSync(cursor).isSymbolicLink(),'PATH_INVALID');cursor=path.dirname(cursor);}
 const build=JSON.parse(fs.readFileSync(path.join(buildPath,'build-result.json'),'utf8')),original=fs.readFileSync(path.join(ROOT,'bin',DLL)),candidate=fs.readFileSync(path.join(buildPath,'obj',DLL));
 check(build.scenario==='ALM-03'&&['COMPILED_DORMANT_CANDIDATE','COMPILED_SCOPED_NOT_INSTALLED','COMPILED_NO_WRITER_DIAGNOSTIC'].includes(build.status)&&build.deployed===false&&build.executionAuthorized===false&&build.originalSha256===sha(original)&&build.candidateSha256===sha(candidate),'BUILD_CHANGED');
 const inputs=JSON.parse(fs.readFileSync(path.join(buildPath,'source-inputs.json'),'utf8'));for(const [file,h]of Object.entries(inputs)){const full=path.resolve(ROOT,file);check(!path.relative(ROOT,full).startsWith('..')&&sha(fs.readFileSync(full))===h,'PRODUCT_INPUT_CHANGED');}
 const runId=crypto.randomBytes(16).toString('hex'),dir=path.join(ROOT,'.tmp','doc117-alm03-lease-'+runId),m={issueKey:'DOC-117',scenario:'ALM-03',mode:'REHEARSAL',runId,authorization:false,effectsAuthorized:false,runtimeActivationAuthorized:false,maximumRuntimeSeconds:600,originalSha256:sha(original),candidateSha256:sha(candidate),runtimeRoot:path.join(dir,'site'),candidatePath:path.join(dir,'candidate',DLL),backupPath:path.join(dir,'backup','original.dll'),reserveBackupPath:path.join(dir,'reserve','original.dll'),leaseJournal:path.join(dir,'lease.json')};lease(m);
 fs.mkdirSync(path.join(m.runtimeRoot,'bin'),{recursive:true});fs.mkdirSync(path.dirname(m.candidatePath));
 const target=path.join(m.runtimeRoot,'bin',DLL),manifest=path.join(dir,'rehearsal-manifest.json');fs.writeFileSync(target,original,{flag:'wx'});fs.writeFileSync(m.candidatePath,candidate,{flag:'wx'});fs.writeFileSync(manifest,JSON.stringify(m),{flag:'wx'});
 let observed;const child=spawn(process.execPath,[__filename,'--owner',manifest],{windowsHide:true,stdio:['ignore','ignore','ignore','ipc'],env:env()});
 await new Promise((resolve,reject)=>{child.on('message',v=>{observed=v;});child.once('error',()=>reject(Error('DOC117_ALM03_REHEARSAL_OWNER_FAILED')));child.once('exit',code=>code===0?resolve():reject(Error('DOC117_ALM03_REHEARSAL_OWNER_FAILED')));});
 check(observed?.candidateObserved===true&&observed.ownerPid===child.pid&&observed.guardPid!==child.pid,'OWNER_NOT_OBSERVED');
 let restored=false;for(let i=0;i<80;i++){const j=JSON.parse(fs.readFileSync(m.leaseJournal,'utf8'));if(j.restored===true&&sha(fs.readFileSync(target))===m.originalSha256){restored=true;break;}await delay(100);}check(restored,'RESTORE_NOT_OBSERVED');
 check([target,m.backupPath,m.reserveBackupPath].every(f=>fs.readFileSync(f).equals(original))&&sha(fs.readFileSync(path.join(ROOT,'bin',DLL)))===m.originalSha256,'RESTORATION_CHANGED');
 const report={issueKey:'DOC-117',scenario:'ALM-03-RUNTIME-REHEARSAL',status:'PASS',runId,originalSha256:m.originalSha256,candidateSha256:m.candidateSha256,independentGuardian:true,ownerExitedWithoutRestore:true,temporaryRuntimeRestored:true,bothBackupsByteIdentical:true,windowsAclTransferVerified:process.platform==='win32',liveRuntimeActivation:false,authenticatedExecution:false,businessEffects:0,maximumRuntimeSeconds:600};
 fs.writeFileSync(path.join(dir,'rehearsal-result.json'),JSON.stringify(report,null,2)+'\n',{flag:'wx'});return report;
}
if(require.main===module){const args=process.argv.slice(2);Promise.resolve().then(async()=>{
 if(args.length===2&&args[0]==='--owner')return owner(args[1]);
 if(args.length===3&&args[0]==='--watch'){const m=JSON.parse(fs.readFileSync(args[1],'utf8'));return watchRuntimeLease(m,args[2],lease(m),{errorPrefix:'DOC117_ALM03_REHEARSAL_'});}
 check(args.length===2&&args[0]==='--candidate-build','ARGUMENT_INVALID');console.log(JSON.stringify(await rehearse(args[1])));
}).catch(()=>{console.error('DOC117_ALM03_REHEARSAL_REQUIRES_REVIEW');process.exitCode=2;});}
module.exports={lease,rehearse};
