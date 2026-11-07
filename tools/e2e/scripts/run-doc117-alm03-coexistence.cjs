'use strict';
const fs=require('node:fs'),path=require('node:path'),{spawn,execFileSync}=require('node:child_process');
const {ROOT,DLL,sha,validateManifest,createLease}=require('./support/doc117-alm03-runtime-lease.cjs');
const {scopeDigest,renderGuard}=require('./support/doc117-alm03-scoped-candidate.cjs');
const {executeSession}=require('./support/doc117-internal-session.cjs');
const {priorReads}=require('./support/doc117-alm03-preflight-continuation.cjs');
const {failureCode,validatePairOutcomes}=require('./support/doc117-alm03-failure-evidence.cjs');
const {validatePrepared,assertInitial,assertCheckpoint,validateAuthorization,runCoexistence,check}=require('../tests/support/doc117-alm03-coexistence.cjs');
const {createControls}=require('../tests/support/doc117-alm03-controls.cjs');
const {verifyIndividual}=require('../tests/support/doc117-alm03-setup-scope.cjs');
const {validateProfile,openSession,createDriver}=require('../tests/support/doc111-importacion-carpetas.cjs');
const {selectProductionLevel}=require('../tests/support/doc116-expedientes-importacion.cjs');
const {safePost}=require('../tests/support/doc110-safe-request.cjs');
const {inspectProductionDocumentUploadPreviewUi}=require('./run-workflow-e2e-platform.cjs');
const {queryRows}=require('./support/doc32-e2e-odbc.cjs');
const {hashTool}=require('./run-doc117-alm02-setup.cjs');
const tty=require('./support/interactive-e2e-console.cjs');
const PROPOSAL='Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc117-alm03-coexistencia-propuesto-20261106.json';
const BUDGET={storageRequests:3,documents:3,plans:0,lots:0,receipts:0,expedients:0,maximumSelectRequests:320,maximumRuntimeSeconds:600};
const REVIEW_FILES=[
 'tools/e2e/scripts/run-doc117-alm03-coexistence.cjs','tools/e2e/tests/support/doc117-alm03-coexistence.cjs','tools/e2e/tests/support/doc117-alm03-controls.cjs','tools/e2e/tests/support/doc117-alm03-setup-scope.cjs','tools/e2e/tests/support/doc117-alm03-method-observation.cjs','tools/e2e/scripts/support/doc117-alm03-runtime-lease.cjs','tools/e2e/scripts/support/doc117-alm03-scoped-candidate.cjs','tools/e2e/scripts/support/doc117-alm03-candidate.cjs','tools/e2e/fixtures/Doc117Alm03ClosedGuard.vb','tools/e2e/fixtures/Doc117Alm03MethodObserver.vb','tools/e2e/scripts/support/doc117-internal-session.cjs','tools/e2e/scripts/support/laboratory-runtime-lease.cjs','tools/e2e/scripts/run-workflow-e2e-platform.cjs','tools/e2e/tests/support/doc117-alm02-internal.cjs','tools/e2e/tests/support/doc111-importacion-carpetas.cjs','tools/e2e/tests/support/doc117-alm01-driver.cjs','tools/e2e/tests/support/doc116-exp01-setup.cjs','tools/e2e/tests/support/doc116-expedientes-importacion.cjs','tools/e2e/tests/support/doc110-safe-request.cjs','tools/e2e/tests/support/authenticated-workflow-session.cjs','tools/e2e/scripts/support/doc32-e2e-odbc.cjs','tools/e2e/scripts/query-doc32-odbc-fingerprint.ps1','tools/e2e/scripts/support/interactive-e2e-console.cjs','tools/e2e/scripts/run-doc117-alm02-setup.cjs'
 ,'tools/e2e/scripts/support/workflow-e2e-platform.cjs','tools/e2e/scripts/support/workflow-e2e-platform-profile.cjs','tools/e2e/scripts/support/workflow-e2e-platform-registry.cjs','tools/e2e/fixtures/Doc117SolicitudHash.vb','tools/e2e/fixtures/Doc117SolicitudHashConsole.vb','tools/e2e/scripts/build-doc117-alm03-scoped.cjs','tools/e2e/scripts/rehearse-doc117-alm03-runtime.cjs'
 ,'tools/e2e/scripts/support/doc117-alm03-preflight-continuation.cjs'
 ,'tools/e2e/scripts/support/doc117-alm03-failure-evidence.cjs'
];
const json=file=>JSON.parse(fs.readFileSync(file,'utf8').replace(/^\uFEFF/,''));
const minimumEnvironment=()=>Object.fromEntries(Object.entries(process.env).filter(([k])=>['PATH','Path','SystemRoot','WINDIR','TEMP','TMP','ComSpec','PATHEXT'].includes(k)));
function codeFingerprint(){return sha(REVIEW_FILES.map(f=>sha(fs.readFileSync(path.join(ROOT,f)))).join('\n'));}
function closedPath(file){
 check(typeof file==='string','PATH_INVALID');const full=path.resolve(ROOT,file),relative=path.relative(ROOT,full);check(relative&&!relative.startsWith('..')&&!path.isAbsolute(relative),'PATH_INVALID');
 let cursor=full;while(cursor!==ROOT){if(fs.existsSync(cursor))check(!fs.lstatSync(cursor).isSymbolicLink(),'PATH_INVALID');cursor=path.dirname(cursor);}return full;
}
function loadPrepared(){
 const p=json(path.join(ROOT,PROPOSAL));check(p.issueKey==='DOC-117'&&p.scope==='ALM-03-COEXISTENCE'&&p.environment==='https://localhost/GestionDocumental-Docuarchi.net/'&&p.lotId==='22'&&p.actorRef==='141'&&p.ambitoRef==='2'&&p.levelRef==='7'&&p.individualLevelRef==='8'&&p.versionRef==='38'&&p.cabinetRef==='10'&&JSON.stringify(p.budget)===JSON.stringify(BUDGET),'PROPOSAL_CHANGED');
 const readyFile=closedPath(p.readyFile),ready=json(readyFile),scope=validatePrepared(ready.resourceScope,ready.commands);
 check(sha(fs.readFileSync(readyFile))===p.readyFileSha256&&scopeDigest(scope)===p.resourceScopeSha256,'RESOURCE_SCOPE_CHANGED');
 const candidate=closedPath(p.candidateFile),report=json(closedPath(p.candidateBuildReport));
 check(sha(fs.readFileSync(candidate))===p.candidateSha256&&report.candidateSha256===p.candidateSha256&&report.resourceScopeSha256===p.resourceScopeSha256&&report.originalSha256===p.originalSha256&&report.status==='COMPILED_SCOPED_NOT_INSTALLED'&&report.executionAuthorized===false&&report.deployed===false,'CANDIDATE_CHANGED');
 check(sha(fs.readFileSync(closedPath(p.candidateBuildReport)))===p.candidateBuildReportSha256&&report.guardSha256===sha(renderGuard(scope)),'CANDIDATE_SOURCES_CHANGED');
 const sourceDirectory=path.dirname(closedPath(p.candidateBuildReport));for(const [file,hash]of [['ClassAlmacenamiento.Laboratory.vb',report.overlaySha256],['Doc117Alm03MethodObserver.vb',report.helperSha256],['Doc117Alm03ClosedGuard.vb',report.guardSha256],['AdaptadorDocumentoImportacion.Legacy.Laboratory.vb',report.bindingSha256]])check(sha(fs.readFileSync(closedPath(path.join(sourceDirectory,file))))===hash,'CANDIDATE_SOURCES_CHANGED');
 const inputs=json(closedPath(p.sourceInputsFile));check(Object.keys(inputs).length===912&&sha(JSON.stringify(inputs))===p.sourceInputsSha256,'PRODUCT_INPUT_CHANGED');for(const [file,digest]of Object.entries(inputs))check(sha(fs.readFileSync(closedPath(file)))===digest,'PRODUCT_INPUT_CHANGED');
 const calculator=hashTool(closedPath(p.hashToolFile));check(sha(fs.readFileSync(calculator))===p.hashToolSha256,'HASH_TOOL_CHANGED');
 const fixture=fs.readFileSync(closedPath('tools/e2e/fixtures/'+scope.individual.fileName));check(fixture.length===677&&sha(fixture)===scope.individual.contentSha256,'CLIENT_ORIGINAL_CHANGED');
 const run=path.join(ROOT,'.tmp','doc117-alm03-live-lease-'+scope.runId),m={issueKey:'DOC-117',scenario:'ALM-03-COEXISTENCE',mode:'LIVE',runId:scope.runId,runtimeRoot:ROOT,environment:p.environment,actorRef:'141',ambitoRef:'2',levelRef:'7',individualLevelRef:'8',versionRef:'38',cabinetRef:'10',lotId:'22',resourceScope:scope,resourceScopeSha256:scopeDigest(scope),originalSha256:p.originalSha256,candidateSha256:p.candidateSha256,maximumRuntimeSeconds:600,candidatePath:path.join(run,'candidate',DLL),backupPath:path.join(run,'backup','original.dll'),reserveBackupPath:path.join(run,'reserve','original.dll'),leaseJournal:path.join(run,'lease.json')};
 return{p,ready,scope,candidate,calculator,m,run,markers:path.join(ROOT,'App_Data','DOC117-ALM03-'+scope.runId),proposalSha256:sha(fs.readFileSync(path.join(ROOT,PROPOSAL)))};
}
function validateGrant(a,prepared,codeSha256){
 validateAuthorization(a,prepared.scope);check(a.environment===prepared.p.environment&&a.proposalSha256===prepared.proposalSha256&&a.candidateSha256===prepared.p.candidateSha256&&a.codeSha256===codeSha256&&/^[a-f0-9]{32}$/.test(a.authorizationId),'SPECIFIC_AUTHORIZATION_REQUIRED');
 return priorReads(a.preflightContinuation,prepared);
}
function readObservation(prepared,complete){
 const dir=prepared.markers,read=name=>json(closedPath(path.join(dir,name+'.json'))),keys=complete?['IMPORT_A','IMPORT_B','INDIVIDUAL']:['IMPORT_A','INDIVIDUAL'];
 const names=keys.flatMap(k=>['reserved-'+k+'.json','entry-'+k+'.json','exit-'+k+'.json']);names.push('attempt-IMPORT_A.json');if(complete)names.push('attempt-IMPORT_B.json','summary.json');
 check(fs.readdirSync(dir).sort().join(',')===names.sort().join(','),'OBSERVATION_FILES_CHANGED');
 return{events:keys.flatMap(k=>[read('entry-'+k),read('exit-'+k)]),pins:Object.fromEntries(keys.filter(k=>k!=='INDIVIDUAL').map(k=>[k,read('attempt-'+k)])),summary:complete?read('summary'):null};
}
async function main(args=process.argv.slice(2)){
 const prepared=loadPrepared(),{p,ready,scope}=prepared,codeSha256=codeFingerprint();
 if(args.length===1&&args[0]==='--check'){check(sha(fs.readFileSync(path.join(ROOT,'bin',DLL)))===p.originalSha256,'ORIGINAL_RUNTIME_CHANGED');console.log('DOC117_ALM03_CHECK='+JSON.stringify({status:'PASS',proposalSha256:prepared.proposalSha256,codeSha256,candidateSha256:p.candidateSha256,resourceScopeSha256:p.resourceScopeSha256,budget:p.budget,realExecution:false}));return;}
 check(args.length===4&&args[0]==='--profile'&&args[2]==='--authorization','ARGUMENT_INVALID');
 const profile=validateProfile(json(path.resolve(args[1]))),approvalFile=path.resolve(args[3]),approval=json(approvalFile),previousSelectRequests=validateGrant(approval,prepared,codeSha256);
 check(profile.baseUrl===p.environment&&profile.nivelRef==='7'&&(!profile.ignoreHttpsErrors||approval.tls===true),'ENVIRONMENT_CHANGED');tty.requireInteractiveConsole();
 const manifestFile=path.join(prepared.run,'authorized-runtime-manifest.json'),artifact=path.join(ROOT,'tools/e2e/artifacts','doc117-alm03-coexistence-'+approval.authorizationId+'.json');
 check(!fs.existsSync(artifact)&&!fs.existsSync(prepared.run)&&!fs.existsSync(prepared.markers),'PREVIOUS_RUN_REQUIRES_REVIEW');check(sha(fs.readFileSync(path.join(ROOT,'bin',DLL)))===p.originalSha256,'ORIGINAL_RUNTIME_CHANGED');
 fs.mkdirSync(path.dirname(artifact),{recursive:true});const record={issueKey:'DOC-117',scope:p.scope,status:'AWAITING_CREDENTIALS',proposalSha256:prepared.proposalSha256,codeSha256,priorSelectRequests:previousSelectRequests,selectRequests:previousSelectRequests,thisSessionSelectRequests:0,history:[],originalSha256:p.originalSha256,candidateSha256:p.candidateSha256,createdUtc:new Date().toISOString()};fs.writeFileSync(artifact,JSON.stringify(record,null,2)+'\n',{flag:'wx'});
 const save=()=>fs.writeFileSync(artifact,JSON.stringify(record,null,2)+'\n'),step=value=>{record.history.push(value);record.status=value.stage;save();};
 const secrets={},environment=minimumEnvironment();let auth,individualAuth,browser,lease;
 try{
  console.log('DOC-117 ALM-03: lote 22, archivos 36/37, expedientes 666/667 (nivel 7); individual expediente 467 (nivel 8), tipologia 50. Tres cargas una sola vez; cero preparaciones o expedientes nuevos.');
  console.log('Dos sesiones independientes de actor 141/empresa 2. Variante temporal cerrada, respaldo doble y guardian de 600 segundos. Conservar todos los recursos; serializacion produce NO_ACREDITADO.');
  console.log('SELECT acumulados antes de esta sesion: '+previousSelectRequests+'/320; restantes: '+(320-previousSelectRequests)+'.');
  await tty.collectValue(secrets,'user','Cuenta de aplicacion autorizada',{secret:true});await tty.collectValue(secrets,'password','Contrasena de aplicacion',{secret:true});await tty.collectValue(secrets,'dbUser','Cuenta MySQL exclusivamente SELECT',{secret:true});await tty.collectValue(secrets,'dbPassword','Contrasena MySQL de lectura',{secret:true});
  validateGrant(json(approvalFile),prepared,codeFingerprint());loadPrepared();check(!fs.existsSync(prepared.run)&&!fs.existsSync(prepared.markers),'PREVIOUS_RUN_REQUIRES_REVIEW');
  Object.assign(environment,{DOC111_E2E_MODULE:profile.module,DOC111_E2E_USER:secrets.user,DOC111_E2E_PASSWORD:secrets.password});for(const [prefix,dsn]of [['DOC111_E2E',profile.odbcDsn],['DOC111_DA_E2E',profile.docuarchiOdbcDsn]])Object.assign(environment,{[prefix+'_ODBC_DSN']:dsn,[prefix+'_MYSQL_USER']:secrets.dbUser,[prefix+'_MYSQL_PASSWORD']:secrets.dbPassword});
  const query=async(origin,sql,params)=>{check(record.selectRequests<BUDGET.maximumSelectRequests&&['RA','DA'].includes(origin),'SELECT_BUDGET_EXHAUSTED');record.selectRequests++;record.thisSessionSelectRequests++;save();return queryRows(sql,params,environment,origin==='RA'?'DOC111_E2E':'DOC111_DA_E2E');};
  const rootNames=['DOC117-ALM03-A-20261106','DOC117-ALM03-B-20261106'],configured={...profile,nivelRef:'7',versionId:'38',mode:'LOTE',pdfFixture:'doc111-carga-individual.pdf',rootNames,expectations:rootNames.map(n=>({codigo:n,nombre:n,tipologiaId:'79',indices:[{referencia:'14',tipo:'TEXTO',valor:'EEE'}]}))};
  const controls=createControls(query,scope,configured),booleanGrants={environment:true,account:true,select:true,tls:approval.tls};
  const open=async()=>{auth=await openSession(browser,configured,environment,booleanGrants);await selectProductionLevel(auth.page,'7');const reader=createDriver(auth,configured,booleanGrants,{readOnly:true});await reader.refreshToken();const state=await reader.request('ConsultarLote',{loteId:'22'});check(state.loteId==='22'&&state.nivelRef==='7','ACCOUNT_PROOF_FAILED');};
  const close=async()=>{const contexts=[auth,individualAuth];auth=null;individualAuth=null;const done=await Promise.allSettled(contexts.filter(Boolean).map(current=>current.context.close()));check(done.every(r=>r.status==='fulfilled'),'SESSION_CLOSE_FAILED');};
  const calculate=async view=>{const input={Contexto:JSON.parse(view.lot.contexto_doc),Solicitudes:ready.commands.map(c=>({LoteId:c.loteId,ArchivoId:c.recursoId,RevisionEsperada:c.revisionEsperada,ClaveOperacion:c.claveOperacion,Fase:c.fase}))};const hashes=JSON.parse(execFileSync(prepared.calculator,[],{input:JSON.stringify(input),encoding:'utf8',env:minimumEnvironment(),windowsHide:true,stdio:['pipe','pipe','pipe'],timeout:15000}));check(hashes.length===2&&hashes.every((h,i)=>h===scope.resources[i].requestSha256),'REQUEST_HASH_CHANGED');};
  const checkDispatch=()=>{validateGrant(json(approvalFile),prepared,codeFingerprint());check(lease.current()===p.candidateSha256,'RUNTIME_WINDOW_ENDED');};
  const {chromium}=require('../node_modules/@playwright/test'),playwright=require('../playwright.config.cjs');browser=await chromium.launch({headless:true,channel:playwright.use.channel,executablePath:playwright.use.executablePath});
  const m=validateManifest({...prepared.m,authorization:true,runtimeActivationAuthorized:true,effectsAuthorized:true});lease=createLease(m);
  const result=await executeSession({lease,onProgress:step,closeSession:close,
   preflightOriginal:async()=>{check(lease.current()===p.originalSha256,'ORIGINAL_RUNTIME_CHANGED');await open();const view=await controls.snapshot('22');assertInitial(view,scope,ready.commands);await calculate(view);await controls.original(0);await controls.original(1);await controls.assertNoDocuments(view);const individual=await controls.individualBefore(),inventoryBefore=await controls.inventoryCount(),preserved=await controls.preservation({maxPlan:'37',maxLot:'21'});record.preservedBefore=preserved;record.inventoryBefore=inventoryBefore;record.individualBaseline={classificationSha256:scope.individual.classificationSha256,indicesSha256:sha(JSON.stringify(individual.indices)),canonicalSha256:sha(JSON.stringify(individual.canonical)),mappedIndices:individual.indices.length,canonicalIndices:individual.canonical.fields.length,counts:individual.counts,priorDocuments:individual.retained};save();
    fs.mkdirSync(path.dirname(m.candidatePath),{recursive:true});fs.copyFileSync(prepared.candidate,m.candidatePath,fs.constants.COPYFILE_EXCL);check(sha(fs.readFileSync(m.candidatePath))===p.candidateSha256,'CANDIDATE_CHANGED');fs.writeFileSync(manifestFile,JSON.stringify(m,null,2)+'\n',{flag:'wx'});return{originalSha256:p.originalSha256,individual,inventoryBefore,preserved};},
   startGuard:async()=>{const guard=spawn(process.execPath,[path.join(__dirname,'support/doc117-alm03-runtime-lease.cjs'),'--watch',manifestFile,String(process.pid)],{detached:true,windowsHide:true,stdio:'ignore',env:minimumEnvironment()});let failed=false;guard.once('error',()=>{failed=true;});guard.unref();let armed=false;for(let i=0;i<80;i++){check(!failed,'GUARD_NOT_READY');const journal=json(m.leaseJournal);if(journal.stage==='GUARD_READY'&&journal.ownerPid===process.pid&&journal.guardPid===guard.pid){armed=true;break;}await new Promise(resolve=>setTimeout(resolve,50));}check(armed,'GUARD_NOT_READY');},
   openCandidate:async()=>{check(lease.current()===p.candidateSha256,'CANDIDATE_NOT_ACTIVE');await open();const view=await controls.snapshot('22');assertInitial(view,scope,ready.commands);await calculate(view);await verifyIndividual(query,{individual:scope.individual,fixture:{bytes:677,sha256:scope.individual.contentSha256}});individualAuth=await openSession(browser,configured,environment,booleanGrants);check(individualAuth.context!==auth.context,'DISTINCT_CONTEXTS_REQUIRED');},
   executeWindows:before=>runCoexistence({scope,commands:ready.commands,authorization:approval,controls,before,onProgress:step,readObservation:complete=>readObservation(prepared,complete),
    send:async command=>{checkDispatch();const token=await auth.page.locator('#doc111-importacion').getAttribute('data-token');check(typeof token==='string'&&token.length>0,'TOKEN_REQUIRED');try{const response=await safePost(auth.context.request,new URL('webservice/WebServiceImportacionCarpetas.asmx/AvanzarLote',profile.baseUrl).href,{maxRedirects:0,headers:{'X-Importacion-Carpetas-CSRF':token,Origin:new URL(profile.baseUrl).origin},data:{solicitud:command}},'AvanzarLote');const raw=await response.json();return{status:response.status(),body:raw?.d??raw};}catch{throw Error('DOC117_ALM03_RESPONSE_UNCERTAIN');}},
    individual:hooks=>inspectProductionDocumentUploadPreviewUi({context:individualAuth.context,plan:{scenario:{expectations:['production-document-upload-execution-ui']},profile:{baseUrl:profile.baseUrl,budgetMs:60000,fixturePath:scope.individual.fileName,expedientId:467,documentTypeId:50,documentTypeName:'Comprobante de Egreso'}},beforeStorage:async()=>{await hooks.beforeStorage();checkDispatch();},onStored:hooks.onStored,onStorageOutcome:hooks.onStorageOutcome})
   }),
   reopenOriginal:open,
   verifyAfterRestore:async(before,r)=>{check(lease.current()===p.originalSha256,'ORIGINAL_RUNTIME_CHANGED');const view=await controls.snapshot('22');for(let i=0;i<2;i++){assertCheckpoint(view,scope,i,r.references[i],true);await controls.physical(view,i,r.references[i]);await controls.original(i);}check(view.lot.estado==='COMPLETADO'&&view.attempts.length===6&&view.attempts.every(a=>a.estado==='CONFIRMADO'&&a.fin_utc)&&view.roots.every(x=>x.estado==='COMPLETADA'&&!x.intento_activo_id),'FINAL_LOT_NOT_CONFIRMED');r.individualVerification=await controls.individualPhysical(r.references[2],before.individual);check(await controls.inventoryCount()===r.inventoryAfter,'UNEXPECTED_DOCUMENT_COUNT');const after=await controls.preservation(before.preserved.scope);record.preservedAfter=after;save();check(JSON.stringify(after)===JSON.stringify(before.preserved),'PRESERVED_RESOURCES_CHANGED');r.businessEvidenceVerified=true;r.alm03ConcurrentPass=r.methodObservation.writerBodyOverlapVerified;r.originalsVerified=true;return true;}
  });
  record.result=result;record.status=result.status;record.finishedUtc=new Date().toISOString();save();console.log('DOC117_ALM03_RESULT='+JSON.stringify(result));
 }catch(error){
  record.status='STOPPED_REVIEW_REQUIRED';record.code=/^(?:DOC117_(?:ALM03|INTERNAL|RUNTIME)|DOC111)_[A-Z0-9_]+$/.test(error?.message||'')?error.message:'DOC117_ALM03_STOPPED';
  record.failureCode=failureCode(error);
  try{if(error?.pairOutcomes)record.operationOutcomes=validatePairOutcomes(error.pairOutcomes);}catch{}
  // Retain only closed numeric identities from available exit markers, even if
  // a response, checkpoint, projection or final summary could not be verified.
  for(const key of ['IMPORT_A','IMPORT_B','INDIVIDUAL'])try{const e=json(closedPath(path.join(prepared.markers,'exit-'+key+'.json')));if(e.runId===scope.runId&&e.resourceKey===key&&/^[1-9][0-9]{0,9}$/.test(e.documentoId)&&/^[1-9][0-9]{0,9}$/.test(e.inventarioId))record.history.push({stage:'OBSERVED_IDENTITIES_REQUIRING_REVIEW',resourceKey:key,documentoId:e.documentoId,inventarioId:e.inventarioId});}catch{}
  try{record.runtimeRestored=!!lease&&lease.current()===p.originalSha256;}catch{record.runtimeRestored=false;}record.finishedUtc=new Date().toISOString();save();console.log('DOC117_ALM03_STOPPED='+JSON.stringify({status:record.status,code:record.code,runtimeRestored:record.runtimeRestored}));process.exitCode=2;
 }finally{for(const current of [auth,individualAuth])if(current)await current.context.close().catch(()=>{});if(browser)await browser.close().catch(()=>{});for(const k of Object.keys(secrets))delete secrets[k];for(const k of Object.keys(environment))delete environment[k];}
 console.log('DOC117_ALM03_FINISHED: no repetir; conservar recursos y marcadores para revision.');
}
if(require.main===module)main().catch(()=>{console.error('DOC117_ALM03_NOT_STARTED: revisar autorizacion, fuentes y recursos.');process.exitCode=2;});
module.exports={main,PROPOSAL,BUDGET,REVIEW_FILES,codeFingerprint,closedPath,loadPrepared,validateGrant,readObservation};
