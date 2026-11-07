'use strict';
const fs=require('node:fs'),path=require('node:path'),{spawn,execFileSync}=require('node:child_process');
const {ROOT,DLL,sha,validateManifest,createLease}=require('./support/doc117-runtime-lease.cjs');
const {scopeDigest}=require('./support/doc117-alm02-candidate.cjs');
const {executeSession}=require('./support/doc117-internal-session.cjs');
const {validatePrepared,assertInitial,assertCheckpoint,validateProof,createControls,runWindows}=require('../tests/support/doc117-alm02-internal.cjs');
const {validateProfile,openSession,createDriver}=require('../tests/support/doc111-importacion-carpetas.cjs');
const {selectProductionLevel}=require('../tests/support/doc116-expedientes-importacion.cjs');
const {safePost}=require('../tests/support/doc110-safe-request.cjs');
const {queryRows}=require('./support/doc32-e2e-odbc.cjs');
const {hashTool}=require('./run-doc117-alm02-setup.cjs');
const tty=require('./support/interactive-e2e-console.cjs');
const PROPOSAL='Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc117-alm02-interno-propuesto-20261106.json';
const check=(v,c)=>{if(!v)throw Error('DOC117_INTERNAL_'+c);};
const json=file=>JSON.parse(fs.readFileSync(file,'utf8').replace(/^\uFEFF/,''));
const minimumEnvironment=()=>Object.fromEntries(Object.entries(process.env).filter(([k])=>['PATH','Path','SystemRoot','WINDIR','TEMP','TMP','ComSpec','PATHEXT'].includes(k)));
const REVIEW_FILES=['tools/e2e/scripts/run-doc117-alm02-internal.cjs','tools/e2e/scripts/support/doc117-internal-session.cjs','tools/e2e/tests/support/doc117-alm02-internal.cjs','tools/e2e/scripts/support/doc117-runtime-lease.cjs','tools/e2e/scripts/support/laboratory-runtime-lease.cjs','tools/e2e/scripts/support/doc117-alm02-candidate.cjs','tools/e2e/fixtures/Doc117InternalDocumentCandidate.vb','tools/e2e/tests/support/doc111-importacion-carpetas.cjs','tools/e2e/tests/support/doc117-alm01-driver.cjs','tools/e2e/tests/support/doc116-exp01-setup.cjs','tools/e2e/tests/support/doc116-expedientes-importacion.cjs','tools/e2e/tests/support/doc110-safe-request.cjs','tools/e2e/tests/support/authenticated-workflow-session.cjs','tools/e2e/scripts/support/doc32-e2e-odbc.cjs','tools/e2e/scripts/query-doc32-odbc-fingerprint.ps1','tools/e2e/scripts/support/interactive-e2e-console.cjs','tools/e2e/scripts/run-doc117-alm02-setup.cjs'];
function codeFingerprint(){return sha(REVIEW_FILES.map(f=>sha(fs.readFileSync(path.join(ROOT,f)))).join('\n'));}
function closedPath(file){const full=path.resolve(ROOT,file),relative=path.relative(ROOT,full);check(relative&&!relative.startsWith('..')&&!path.isAbsolute(relative),'PATH_INVALID');let current=full;while(current!==ROOT){if(fs.existsSync(current))check(!fs.lstatSync(current).isSymbolicLink(),'PATH_INVALID');current=path.dirname(current);}return full;}
function loadPrepared(){
 const p=json(path.join(ROOT,PROPOSAL));
 check(p.issueKey==='DOC-117'&&p.scope==='ALM-02-INTERNAL'&&p.environment==='https://localhost/GestionDocumental-Docuarchi.net/'&&p.lotId==='21'&&p.actorRef==='141'&&p.ambitoRef==='2'&&p.levelRef==='7'&&p.versionRef==='38'&&p.cabinetRef==='10','PROPOSAL_CHANGED');
 check(JSON.stringify(p.budget)===JSON.stringify({storageRequests:3,documents:2,plans:0,lots:0,receipts:0,expedients:0,maximumSelectRequests:240,maximumRuntimeSeconds:600}),'BUDGET_CHANGED');
 const readyFile=closedPath(p.readyFile),ready=json(readyFile),scope=validatePrepared(ready.resourceScope,ready.commands);
 check(scope.lotId===p.lotId&&scope.runtimeRoot===ROOT&&scopeDigest(scope)===p.resourceScopeSha256&&sha(fs.readFileSync(readyFile))===p.readyFileSha256,'RESOURCE_SCOPE_CHANGED');
 check(scope.resources.map(r=>r.rootId+'/'+r.fileId+'/'+r.expedienteId+'/'+r.relacionNivelId).join(',')==='33/34/664/1089,34/35/665/1090','RESOURCES_CHANGED');
 const candidate=closedPath(p.candidateFile),report=json(closedPath(p.candidateBuildReport));
 check(sha(fs.readFileSync(candidate))===p.candidateSha256&&report.candidateSha256===p.candidateSha256&&report.resourceScopeSha256===p.resourceScopeSha256&&report.originalSha256===p.originalSha256&&report.status==='COMPILED_SCOPED_NOT_INSTALLED','CANDIDATE_CHANGED');
 const inputs=json(closedPath(p.sourceInputsFile));for(const [file,digest]of Object.entries(inputs))check(sha(fs.readFileSync(closedPath(file)))===digest,'PRODUCT_INPUT_CHANGED');
 const calculator=hashTool(closedPath(p.hashToolFile));check(sha(fs.readFileSync(calculator))===p.hashToolSha256,'HASH_TOOL_CHANGED');
 const run=path.join(ROOT,'.tmp','doc117-alm02-lease-'+scope.runId),m={issueKey:'DOC-117',scenario:'ALM-02-INTERNAL',mode:'LIVE',runId:scope.runId,runtimeRoot:ROOT,environment:p.environment,actorRef:'141',ambitoRef:'2',levelRef:'7',versionRef:'38',cabinetRef:'10',lotId:scope.lotId,resourceScope:scope,resourceScopeSha256:scopeDigest(scope),originalSha256:p.originalSha256,candidateSha256:p.candidateSha256,maximumRuntimeSeconds:600,candidatePath:path.join(run,'candidate',DLL),backupPath:path.join(run,'backup/original.dll'),reserveBackupPath:path.join(run,'reserve/original.dll'),leaseJournal:path.join(run,'lease.json')};
 const markers=path.join(ROOT,'App_Data','DOC117-ALM02-'+scope.runId);
 return{p,ready,scope,candidate,calculator,m,run,markers,proposalSha256:sha(fs.readFileSync(path.join(ROOT,PROPOSAL)))};
}
function validateGrant(a,s,codeSha256){
 check(a?.scope==='ALM-02-INTERNAL'&&a.environment===s.p.environment&&['authorized','environmentAuthorized','account','select','storage','faultInjection','runtimeChanges','exclusive','discardable'].every(k=>a[k]===true)&&a.proposalSha256===s.proposalSha256&&a.resourceScopeSha256===s.p.resourceScopeSha256&&a.candidateSha256===s.p.candidateSha256&&a.codeSha256===codeSha256&&/^[a-f0-9]{32}$/.test(a.authorizationId),'SPECIFIC_AUTHORIZATION_REQUIRED');
}
async function main(args=process.argv.slice(2)){
 const prepared=loadPrepared(),{p,ready,scope}=prepared,codeSha256=codeFingerprint();
 if(args.length===1&&args[0]==='--check'){check(sha(fs.readFileSync(path.join(ROOT,'bin',DLL)))===p.originalSha256,'ORIGINAL_RUNTIME_CHANGED');console.log('DOC117_INTERNAL_CHECK='+JSON.stringify({status:'PASS',proposalSha256:prepared.proposalSha256,codeSha256,candidateSha256:p.candidateSha256,resourceScopeSha256:p.resourceScopeSha256,budget:p.budget,realExecution:false}));return;}
 check(args.length===4&&args[0]==='--profile'&&args[2]==='--authorization','ARGUMENT_INVALID');
 const profile=validateProfile(json(path.resolve(args[1]))),approvalFile=path.resolve(args[3]),approval=json(approvalFile);validateGrant(approval,prepared,codeSha256);
 check(profile.baseUrl===p.environment&&profile.nivelRef==='7'&&(!profile.ignoreHttpsErrors||approval.tls===true),'ENVIRONMENT_CHANGED');
 tty.requireInteractiveConsole();
 const manifestFile=path.join(prepared.run,'authorized-runtime-manifest.json'),artifact=path.join(ROOT,'tools/e2e/artifacts','doc117-alm02-internal-'+approval.authorizationId+'.json');
 check(!fs.existsSync(artifact)&&!fs.existsSync(prepared.m.leaseJournal)&&!fs.existsSync(manifestFile)&&!fs.existsSync(prepared.markers),'PREVIOUS_RUN_REQUIRES_REVIEW');
 check(sha(fs.readFileSync(path.join(ROOT,'bin',DLL)))===p.originalSha256,'ORIGINAL_RUNTIME_CHANGED');
 fs.mkdirSync(path.dirname(artifact),{recursive:true});const record={issueKey:'DOC-117',scope:p.scope,status:'AWAITING_CREDENTIALS',proposalSha256:prepared.proposalSha256,codeSha256,selectRequests:0,history:[],originalSha256:p.originalSha256,candidateSha256:p.candidateSha256,createdUtc:new Date().toISOString()};fs.writeFileSync(artifact,JSON.stringify(record,null,2)+'\n',{flag:'wx'});const save=()=>fs.writeFileSync(artifact,JSON.stringify(record,null,2)+'\n');
 const step=value=>{record.history.push(value);record.status=value.stage;save();};
 const secrets={},environment=minimumEnvironment();let auth,browser,lease,readProof;
 try{
  console.log('DOC-117 ALM-02 INTERNO: lote 21, archivos 34/35, expedientes 664/665. Hasta dos documentos, tres solicitudes A/replay-A/B. B no se repite.');
  console.log('Instalacion temporal de la candidata revisada; respaldo doble y guardian independiente de 10 minutos. Ventana exclusiva; conservar todos los recursos.');
  await tty.collectValue(secrets,'user','Cuenta de aplicacion autorizada',{secret:true});await tty.collectValue(secrets,'password','Contrasena de aplicacion',{secret:true});await tty.collectValue(secrets,'dbUser','Cuenta MySQL exclusivamente SELECT',{secret:true});await tty.collectValue(secrets,'dbPassword','Contrasena MySQL de lectura',{secret:true});
  validateGrant(json(approvalFile),prepared,codeFingerprint());loadPrepared();check(!fs.existsSync(prepared.markers),'PREVIOUS_RUN_REQUIRES_REVIEW');
  Object.assign(environment,{DOC111_E2E_MODULE:profile.module,DOC111_E2E_USER:secrets.user,DOC111_E2E_PASSWORD:secrets.password});
  for(const [prefix,dsn]of [['DOC111_E2E',profile.odbcDsn],['DOC111_DA_E2E',profile.docuarchiOdbcDsn]])Object.assign(environment,{[prefix+'_ODBC_DSN']:dsn,[prefix+'_MYSQL_USER']:secrets.dbUser,[prefix+'_MYSQL_PASSWORD']:secrets.dbPassword});
  const query=async(origin,sql,params)=>{check(record.selectRequests<240&&['RA','DA'].includes(origin),'SELECT_BUDGET_EXHAUSTED');record.selectRequests++;save();return queryRows(sql,params,environment,origin==='RA'?'DOC111_E2E':'DOC111_DA_E2E');};
  const configured={...profile,nivelRef:'7',versionId:'38',mode:'LOTE',pdfFixture:'doc111-carga-individual.pdf',rootNames:p.rootNames,expectations:p.rootNames.map(n=>({codigo:n,nombre:n,tipologiaId:'79',indices:[{referencia:'14',tipo:'TEXTO',valor:'EEE'}]}))};
  const controls=createControls(query,scope,configured,ROOT),booleanGrants={environment:true,account:true,select:true,tls:approval.tls};
  const open=async()=>{auth=await openSession(browser,configured,environment,booleanGrants);await selectProductionLevel(auth.page,'7');const reader=createDriver(auth,configured,booleanGrants,{readOnly:true});await reader.refreshToken();const state=await reader.request('ConsultarLote',{loteId:'21'});check(state.loteId==='21'&&state.nivelRef==='7','ACCOUNT_PROOF_FAILED');};
  const close=async()=>{if(auth){const current=auth;auth=null;await current.context.close().catch(()=>{});}};
  const calculate=async view=>{
   const input={Contexto:JSON.parse(view.lot.contexto_doc),Solicitudes:ready.commands.map(c=>({LoteId:c.loteId,ArchivoId:c.recursoId,RevisionEsperada:c.revisionEsperada,ClaveOperacion:c.claveOperacion,Fase:c.fase}))};
   const hashes=JSON.parse(execFileSync(prepared.calculator,[],{input:JSON.stringify(input),encoding:'utf8',env:minimumEnvironment(),windowsHide:true,stdio:['pipe','pipe','pipe'],timeout:15000}));check(hashes.length===2&&hashes.every((h,i)=>h===scope.resources[i].requestSha256),'REQUEST_HASH_CHANGED');
  };
  readProof=async index=>{const file=scope.resources[index].fileId,read=name=>json(closedPath(path.join(prepared.markers,name+'-archivo-'+file+'.json')));return{pin:read('intento'),writer:read('escritor'),fault:json(closedPath(path.join(prepared.markers,'archivo-'+file+'.json')))};};
  const {chromium}=require('../node_modules/@playwright/test'),playwright=require('../playwright.config.cjs');browser=await chromium.launch({headless:true,channel:playwright.use.channel,executablePath:playwright.use.executablePath});
  const m=validateManifest({...prepared.m,authorization:true,runtimeActivationAuthorized:true,effectsAuthorized:true});lease=createLease(m);
  const result=await executeSession({lease,onProgress:step,closeSession:close,
   preflightOriginal:async()=>{check(lease.current()===p.originalSha256,'ORIGINAL_RUNTIME_CHANGED');await open();const view=await controls.snapshot('21');assertInitial(view,scope,ready.commands);await calculate(view);await controls.original(0);await controls.original(1);await controls.assertNoDocuments(view);const before=await controls.preservation({maxPlan:'36',maxLot:'20'});record.preservedBefore=before;save();
    fs.mkdirSync(path.dirname(m.candidatePath),{recursive:true});fs.copyFileSync(prepared.candidate,m.candidatePath,fs.constants.COPYFILE_EXCL);check(sha(fs.readFileSync(m.candidatePath))===p.candidateSha256,'CANDIDATE_CHANGED');fs.writeFileSync(manifestFile,JSON.stringify(m,null,2)+'\n',{flag:'wx'});return{originalSha256:p.originalSha256,preserved:before};},
   startGuard:async()=>{const guard=spawn(process.execPath,[path.join(__dirname,'support/doc117-runtime-lease.cjs'),'--watch',manifestFile,String(process.pid)],{detached:true,windowsHide:true,stdio:'ignore',env:minimumEnvironment()});let failed=false;guard.once('error',()=>{failed=true;});guard.unref();let armed=false;for(let i=0;i<80;i++){check(!failed,'GUARD_NOT_READY');const journal=json(m.leaseJournal);if(journal.stage==='GUARD_READY'&&journal.ownerPid===process.pid&&journal.guardPid===guard.pid){armed=true;break;}await new Promise(resolve=>setTimeout(resolve,50));}check(armed,'GUARD_NOT_READY');},
   openCandidate:async()=>{check(lease.current()===p.candidateSha256,'CANDIDATE_NOT_ACTIVE');await open();await calculate(await controls.snapshot('21'));},
   executeWindows:async()=>runWindows({scope,commands:ready.commands,authorization:approval,controls,readProof,onProgress:step,send:async command=>{validateGrant(json(approvalFile),prepared,codeFingerprint());check(lease.current()===p.candidateSha256,'RUNTIME_WINDOW_ENDED');const token=await auth.page.locator('#doc111-importacion').getAttribute('data-token');check(typeof token==='string'&&token.length>0,'TOKEN_REQUIRED');try{const response=await safePost(auth.context.request,new URL('webservice/WebServiceImportacionCarpetas.asmx/AvanzarLote',profile.baseUrl).href,{maxRedirects:0,headers:{'X-Importacion-Carpetas-CSRF':token,Origin:new URL(profile.baseUrl).origin},data:{solicitud:command}},'AvanzarLote');const raw=await response.json();return{status:response.status(),body:raw?.d??raw};}catch{throw Error('DOC117_INTERNAL_RESPONSE_UNCERTAIN');}}}),
   reopenOriginal:open,
   verifyAfterRestore:async(before,r)=>{check(lease.current()===p.originalSha256,'ORIGINAL_RUNTIME_CHANGED');const view=await controls.snapshot('21');assertCheckpoint(view,scope,0,r.references[0],true);assertCheckpoint(view,scope,1,r.references[1],false);await controls.physical(view,0,r.references[0]);await controls.physical(view,1,r.references[1]);await controls.original(0);await controls.original(1);check(await controls.inventoryCount()===r.inventoryAfter,'UNEXPECTED_DOCUMENT_COUNT');const after=await controls.preservation(before.preserved.scope);record.preservedAfter=after;save();return JSON.stringify(after)===JSON.stringify(before.preserved);}
  });
  record.result=result;record.status='PASS';record.finishedUtc=new Date().toISOString();save();console.log('DOC117_INTERNAL_RESULT='+JSON.stringify(result));
 }catch(error){
  record.status='STOPPED_REVIEW_REQUIRED';record.code=/^(?:DOC117_(?:INTERNAL|RUNTIME)|DOC111)_[A-Z0-9_]+$/.test(error?.message||'')?error.message:'DOC117_INTERNAL_STOPPED';
  if(readProof)for(let i=0;i<2;i++)try{const proof=validateProof(await readProof(i),scope,i,i===0?'AFTER_CHECKPOINT_COMMIT_BEFORE_RETURN':'AFTER_VERIFICATION_BEFORE_CHECKPOINT');record.history.push({stage:'KNOWN_FAULT_IDENTITIES_AFTER_STOP',...proof});}catch{}
  try{record.runtimeRestored=!!lease&&lease.current()===p.originalSha256;}catch{record.runtimeRestored=false;}record.finishedUtc=new Date().toISOString();save();console.log('DOC117_INTERNAL_STOPPED='+JSON.stringify({status:record.status,code:record.code,runtimeRestored:record.runtimeRestored}));process.exitCode=2;
 }finally{if(auth)await auth.context.close().catch(()=>{});if(browser)await browser.close().catch(()=>{});for(const k of Object.keys(secrets))delete secrets[k];for(const k of Object.keys(environment))delete environment[k];}
 console.log('DOC117_INTERNAL_FINISHED: no repetir; conservar marcadores, documentos y originales para revision.');
}
if(require.main===module)main().catch(()=>{console.error('DOC117_INTERNAL_NOT_STARTED: revisar autorizacion, fuentes y recursos.');process.exitCode=2;});
module.exports={main,PROPOSAL,REVIEW_FILES,codeFingerprint,loadPrepared,validateGrant};
