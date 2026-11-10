'use strict';
// Interactive DOC118 console. Secrets remain in this process; the local command
// queue contains only closed phase names and reviewed implementation hashes.
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const tty=require('./support/interactive-e2e-console.cjs');
const {validateProfile}=require('../tests/support/doc111-importacion-carpetas.cjs');
const {createBudget}=require('../tests/support/doc118-session-bridge.cjs');
const {implementationSeal,acquireLease}=require('../tests/support/doc118-live-lifecycle.cjs');
const {preflightFailure}=require('./support/doc117-alm03-preflight-evidence.cjs');
const ROOT=path.resolve(__dirname,'../../..'),CHANGE='doc-118-produccion-importacion-carga-carpeta-lote-recuperacion';
const AUTH=path.join(ROOT,'openspec/changes',CHANGE,'execution-authorization.json');
const IMPL=path.join(__dirname,'support/doc118-live-plan.cjs');
const PHASES=['PREFLIGHT','PREPARE02','RUN02','FINISH02','PREPARE03','RUN03','VERIFY'];
const STAGES=new Set(['PREFLIGHT_BEFORE_LOGIN_A','PREFLIGHT_LOGIN_A','PREFLIGHT_LOT_A','PREFLIGHT_BEFORE_LOGIN_B','PREFLIGHT_LOGIN_B','PREFLIGHT_LOT_B','PREFLIGHT_PUBLICATION','PREFLIGHT_CABINET','PREFLIGHT_OWNER','PREFLIGHT_PROTECTION']);
const sha=b=>crypto.createHash('sha256').update(b).digest('hex');
const read=p=>JSON.parse(fs.readFileSync(p,'utf8').replace(/^\uFEFF/,''));
const check=(v,c)=>{if(!v)throw Error('DOC118_LIVE_'+c);};
const PUBLIC_GUARDS=new Set(['ARGUMENT_INVALID','PREVIOUS_SESSION_REQUIRES_REVIEW','AUTHORIZATION_CHANGED','PROFILE_CHANGED','LIMITS_CHANGED','CONTINUATION_NOT_AUTHORIZED','CARRIED_STATE_CHANGED','CONTINUATION_REVIEW_REQUIRED','CONTINUATION_CAPACITY_INSUFFICIENT','REQUEST_INVALID','PHASE_ORDER_INVALID','IMPLEMENTATION_CHANGED','SESSION_EXPIRED','CASE_DURATION_EXCEEDED','PREVIOUS_RUN_REQUIRES_REVIEW','LEASE_OWNER_CHANGED','OBSERVER_NOT_READY_BEFORE_CAPTURE','OBSERVER_BINDING_CHANGED','ORDINARY_BINARY_CHANGED','CAPTURE_NOT_COMPLETED'].map(c=>'DOC118_LIVE_'+c));
function publicGuardCode(error,fallback='DOC118_LIVE_START_UNCONFIRMED'){return PUBLIC_GUARDS.has(error?.message)?error.message:fallback;}
function phaseFailure(error){const safe=preflightFailure(error);return{code:safe.code==='UNCLASSIFIED_FAILURE'?publicGuardCode(error,'DOC118_LIVE_PHASE_UNCONFIRMED'):safe.code,preflightFailure:safe};}
async function captureCredentials(secrets,{collect=tty.collectValue,onStage=()=>{}}={}){
 for(const [name,label,stage]of [['user','Login de aplicacion de la cuenta autorizada (usuario 141)','APPLICATION_USER'],['password','Contrasena de aplicacion','APPLICATION_PASSWORD'],['dbUser','Cuenta MySQL exclusivamente SELECT','SELECT_USER'],['dbPassword','Contrasena MySQL SELECT','SELECT_PASSWORD']]){
  onStage(stage);try{await collect(secrets,name,label,{secret:name==='password'||name==='dbPassword'});}catch{throw Error('DOC118_LIVE_CAPTURE_NOT_COMPLETED');}
 }
 onStage('RUNTIME_RECHECK');
}
function scope(a,p){
 if(['DOC118_CURRENT_WINDOW','DOC118_INTEGRATION_CONTINUATION'].includes(a?.kind))return require('./support/doc118-current-window.cjs').scope(a,p);
 check(a.issueKey==='DOC-118'&&a.status==='AUTHORIZED'&&a.environment.actorRef==='141'&&a.environment.ambitoRef==='2'&&a.environment.levelRef==='7'&&a.environment.versionRef==='38','AUTHORIZATION_CHANGED');
 check(a.environment.baseUrl===p.baseUrl&&p.nivelRef==='7'&&p.versionId==='38'&&p.module==='GESTOR'&&p.odbcDsn==='MembershipUsers'&&p.docuarchiOdbcDsn==='MembershipUsers','PROFILE_CHANGED');
 const originalWindow=a.maximum.totalSessions===7&&a.maximum.allHttpGlobal===1200&&a.maximum.allHttpPerCase?.COORD02===1125;
 const preparedRecovery=a.maximum.totalSessions===9&&a.maximum.allHttpGlobal===1500&&a.maximum.allHttpPerCase?.COORD02===1425&&a.preparedRecoveryApproval?.status==='AUTHORIZED'&&a.preparedRecoveryApproval.proposal==='execution-prepared-recovery-proposal.json'&&a.preparedRecoveryApproval.carriedSessionAttempts===7;
 check(a.maximum.newPlans===2&&a.maximum.newLots===2&&a.maximum.newRoots===4&&a.maximum.newPdfs===4&&a.maximum.newExpedientes===4&&a.maximum.newStoredDocuments===3&&a.maximum.stopRequests===1&&a.maximum.selectStatementsTotal===240&&a.maximum.pauseMilliseconds===30000&&(originalWindow||preparedRecovery)&&a.environment.localTlsExceptionAuthorized===true,'LIMITS_CHANGED');
 check(a.maximum.allHttpPerCase?.COORD03===75&&a.sessionReuse==='TWO_SHARED_INDEPENDENT_SESSIONS'&&a.continuationApproval?.status==='AUTHORIZED'&&a.budgetContinuationApproval?.status==='AUTHORIZED'&&a.freshWindowApproval?.status==='AUTHORIZED'&&a.freshWindowApproval.legacyExitAcknowledged===true,'CONTINUATION_NOT_AUTHORIZED');
 if(preparedRecovery)check(JSON.stringify(a.preparedCaseRecovery)===JSON.stringify({planId:'40',loteId:'24',roots:['39','40'],files:['40','41'],expedientes:['670','671'],relaciones:['1095','1096']}),'AUTHORIZATION_CHANGED');
 return sha(JSON.stringify({environment:a.environment,maximum:a.maximum,newRootNames:a.newRootNames,debugger:a.debugger,protectedHistoricalAttempts:a.protectedHistoricalAttempts,freshWindowApproval:a.freshWindowApproval,preparedCaseRecovery:a.preparedCaseRecovery,preparedRecoveryApproval:a.preparedRecoveryApproval}));
}
function continuationConsumption(a,proof,priorCapture){
 // A previous approval is not permission to retry after a subsequent stop.
 check(a.executionStatus==='CONTINUATION_AUTHORIZED_AWAITING_MASKED_CAPTURE','CONTINUATION_REVIEW_REQUIRED');
 const acknowledged=proof?.status==='STOPPED_REVIEW_REQUIRED'&&proof.sessionsClosed===true&&proof.ephemeralSecretsCleared===true;
 const reviewedExit=proof?.status==='SOURCE_READINESS_AND_CLOSE_PASS'&&proof.cleanupConfirmed===true&&proof.newAuthenticatedSessions===0&&proof.newDocumentEffects===0&&proof.previousEvidence==='doc118-authenticated-preflight-20261107.json'&&a.freshWindowApproval?.status==='AUTHORIZED'&&a.freshWindowApproval.legacyExitAcknowledged===true&&priorCapture?.issueKey==='DOC-118'&&priorCapture.consoleExit?.processExited===true&&priorCapture.consoleExit.shellExited===true&&priorCapture.consoleExit.directChildrenObserved===0&&priorCapture.documentalPhasesRequested===0;
 check(proof?.issueKey==='DOC-118'&&(acknowledged||reviewedExit)&&Number.isInteger(proof.sessionAttempts),'CARRIED_STATE_CHANGED');
 const consumed=a.consumed,counters=proof.counters;
 check(consumed&&counters&&proof.sessionAttempts===consumed.totalSessionAttempts&&proof.sessionAttempts===consumed.loginAttempts&&proof.sessionAttempts>=0&&proof.sessionAttempts<=a.maximum.totalSessions&&counters.selects===consumed.selectStatements&&['COORD02','COORD03'].every(k=>counters.scenarios?.[k]?.http===consumed.allHttp?.[k]),'CARRIED_STATE_CHANGED');
 if(a.freshWindowApproval){const floor=a.freshWindowApproval.carriedCounters;check(floor&&proof.sessionAttempts>=a.freshWindowApproval.carriedSessionAttempts&&counters.selects>=floor.selects&&['COORD02','COORD03'].every(k=>counters.scenarios[k].http>=floor.scenarios[k].http&&Object.entries(floor.scenarios[k].operations).every(([op,n])=>(counters.scenarios[k].operations[op]||0)>=n)),'CARRIED_STATE_CHANGED');}
 if(proof.phaseIndex>0){
  check(proof.runId==='e372f72da5f9bfdabe6c004ce93137b6'&&proof.phaseIndex===2&&proof.runLeaseReleased===true&&proof.observerCleanupConfirmed===true&&proof.storageRequestsDispatched===0&&proof.sessionAttempts===7&&a.preparedRecoveryApproval?.status==='AUTHORIZED'&&JSON.stringify(proof.preparedCaseRecovery)===JSON.stringify(a.preparedCaseRecovery),'CONTINUATION_REVIEW_REQUIRED');
  const floor=a.preparedRecoveryApproval.carriedCounters;check(floor&&counters.selects>=40&&counters.scenarios.COORD02.http>=995&&JSON.stringify(floor)===JSON.stringify(counters)&&!counters.scenarios.COORD02.operations.ALMACENAR&&consumed.newPlans===1&&consumed.newLots===1&&consumed.newExpedientes===2&&consumed.newStoredDocuments===0,'CARRIED_STATE_CHANGED');
 }
 createBudget({allHttpPerCase:a.maximum.allHttpPerCase,allHttpGlobal:a.maximum.allHttpGlobal,carried:counters});
 check(proof.sessionAttempts+2<=a.maximum.totalSessions&&['COORD02','COORD03'].every(k=>counters.scenarios[k].http<a.maximum.allHttpPerCase[k]),'CONTINUATION_CAPACITY_INSUFFICIENT');
 return structuredClone({sessionAttempts:proof.sessionAttempts,counters});
}
async function main(args=process.argv.slice(2)){
 process.title='DOC118 - Captura enmascarada E2E';
 check(args.length===2&&['--run','--current-window','--integration-continuation'].includes(args[0])&&/^[a-f0-9]{32}$/.test(args[1]),'ARGUMENT_INVALID');tty.requireInteractiveConsole();
 const currentWindow=args[0]!=='--run',windowAdapter=currentWindow?require('./support/doc118-current-window.cjs'):null,authorizationPath=currentWindow?path.join(ROOT,args[0]==='--integration-continuation'?windowAdapter.CONTINUATION_AUTH_REL:windowAdapter.AUTH_REL):AUTH;
 const runId=args[1],directory=path.join(ROOT,'.tmp','doc118-live-'+runId);check(!fs.existsSync(directory),'PREVIOUS_SESSION_REQUIRES_REVIEW');
 const profile=validateProfile(read(path.join(ROOT,'tools/e2e/profiles/doc111-importacion-carpetas-lote-20261102.runtime.json'))),authorization=read(authorizationPath),pin=scope(authorization,profile);
 check(!currentWindow||authorization.runId===runId,'AUTHORIZATION_CHANGED');
 require(IMPL).assertDependencies(); // Fail before secret entry if a shared binding is unavailable.
 let carried;
 if(currentWindow){windowAdapter.assertEvidence(authorization,ROOT);carried=windowAdapter.initialState(authorization);}else{
 check(['doc118-login-preflight-20261107.json','doc118-continuation-preflight-20261107.json','doc118-authenticated-preflight-20261107.json','doc118-observer-readiness-20261108.json','doc118-prepared-stop-20261108.json','doc118-prepared-recovery-readiness-20261108.json'].includes(authorization.consumptionEvidence),'CARRIED_STATE_CHANGED');
 const proof=read(path.join(ROOT,'Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion',authorization.consumptionEvidence));
 const priorCapture=proof.previousEvidence==='doc118-authenticated-preflight-20261107.json'?read(path.join(ROOT,'Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-authenticated-preflight-20261107.json')):undefined;
 carried=continuationConsumption(authorization,proof,priorCapture);}
 const lease=acquireLease(ROOT,runId,pin),seal=implementationSeal(ROOT);
 fs.mkdirSync(directory);
 const record={issueKey:'DOC-118',runId,pid:process.pid,status:'VERIFYING_OBSERVER',createdUtc:new Date().toISOString(),phaseIndex:0,authorizationScopeSha256:pin,implementationSeal:seal,history:[],counters:null,references:[],carriedSessionAttempts:carried.sessionAttempts,carriedCounters:carried.counters};
 const statusFile=path.join(directory,'status.json'),requestFile=path.join(directory,'request.json');
 if(currentWindow){record.windowKind=authorization.kind;record.historicalTotals=authorization.historicalTotals;record.automaticPhases=true;}
 const save=()=>{record.heartbeatUtc=new Date().toISOString();if(currentWindow)record.accumulatedIncludingHistory=windowAdapter.aggregate(authorization,record.counters||carried.counters,record.sessionAttempts??carried.sessionAttempts);fs.writeFileSync(statusFile+'.new',JSON.stringify(record,null,2)+'\n');fs.renameSync(statusFile+'.new',statusFile);};save();
 const secrets={},environment=Object.fromEntries(Object.entries(process.env).filter(([k])=>['PATH','Path','SystemRoot','WINDIR','TEMP','TMP','ComSpec','PATHEXT'].includes(k)));
 const live={ROOT,directory,runId,profile,authorization,currentWindow,budget:createBudget({allHttpPerCase:authorization.maximum.allHttpPerCase,allHttpGlobal:authorization.maximum.allHttpGlobal,carried:carried.counters}),environment,sessions:[],prepared:{},browser:null,observer:null,sessionCount:carried.sessionAttempts,carriedSessionAttempts:carried.sessionAttempts,activeCase:'COORD02',startedCaseUtc:{},
   reference(ref){const safe={};for(const key of ['planId','loteId','raizId','archivoId','expedienteId','relacionNivelId','intentoId','documentoId','inventarioId'])if(typeof ref[key]==='string'&&/^[1-9][0-9]{0,9}$/.test(ref[key]))safe[key]=ref[key];record.references.push(safe);save();},
   mark(stage){check(STAGES.has(stage),'STAGE_INVALID');record.status=stage;record.lastStage=stage;record.sessionAttempts=live.sessionCount;record.counters=live.budget.snapshot();save();}};
 record.counters=live.budget.snapshot();save();
 live.assertActive=()=>{
  const current=read(authorizationPath);check(current.executionStatus==='CONTINUATION_AUTHORIZED_AWAITING_MASKED_CAPTURE'&&scope(current,profile)===pin,'AUTHORIZATION_CHANGED');
  check(implementationSeal(ROOT)===seal,'IMPLEMENTATION_CHANGED');
  const started=live.startedCaseUtc[live.activeCase];check(!started||Date.now()-started<=authorization.maximum.caseDurationMilliseconds,'CASE_DURATION_EXCEEDED');
 };
 let requestedClose=false;
 try{
  require(IMPL).assertRuntimeReady(live);record.status='AWAITING_CREDENTIALS';save();
  console.log('DOC-118: cuatro entradas enmascaradas: usuario/contrasena de aplicacion y usuario/contrasena SELECT. Solo en memoria; sin reintentos automaticos.');
  await captureCredentials(secrets,{onStage:stage=>{record.captureStage=stage;save();}});
  Object.assign(environment,{DOC111_E2E_MODULE:profile.module,DOC111_E2E_USER:secrets.user,DOC111_E2E_PASSWORD:secrets.password});
  for(const [prefix,dsn]of [['DOC111_E2E',profile.odbcDsn],['DOC111_DA_E2E',profile.docuarchiOdbcDsn]])Object.assign(environment,{[prefix+'_ODBC_DSN']:dsn,[prefix+'_MYSQL_USER']:secrets.dbUser,[prefix+'_MYSQL_PASSWORD']:secrets.dbPassword});
  for(const k of Object.keys(secrets))delete secrets[k];
  require(IMPL).assertRuntimeReady(live);
  record.status='CREDENTIALS_READY';save();console.log('Credenciales capturadas en memoria. Esperando la fase revisada; no se han iniciado sesiones.');
  const expires=Date.now()+30*60*1000;
  while(!requestedClose){
   check(Date.now()<expires,'SESSION_EXPIRED');
   if(currentWindow&&!fs.existsSync(requestFile))fs.writeFileSync(requestFile,JSON.stringify({sequence:record.phaseIndex+1,phase:PHASES[record.phaseIndex],implementationSha256:sha(fs.readFileSync(IMPL))}),{flag:'wx'});
   if(!fs.existsSync(requestFile)){if(Date.now()-Date.parse(record.heartbeatUtc)>5000)save();await new Promise(r=>setTimeout(r,250));continue;}
   const request=read(requestFile);check(Object.keys(request).sort().join()==='implementationSha256,phase,sequence','REQUEST_INVALID');
   check(request.sequence===record.phaseIndex+1&&(request.phase==='CLOSE'||request.phase===PHASES[record.phaseIndex]),'PHASE_ORDER_INVALID');
   fs.renameSync(requestFile,path.join(directory,'request-'+request.sequence+'.consumed.json'));
   if(request.phase==='CLOSE'){record.closeRequestAcknowledged=true;save();requestedClose=true;break;}
   live.assertActive();
   check(scope(read(authorizationPath),profile)===pin,'AUTHORIZATION_CHANGED');
   check(request.implementationSha256===sha(fs.readFileSync(IMPL)),'IMPLEMENTATION_CHANGED');
   record.status='RUNNING_'+request.phase;record.history.push({phase:request.phase,status:'REQUESTED',utc:new Date().toISOString()});save();
   delete require.cache[require.resolve(IMPL)];const implementation=require(IMPL);
   await implementation.execute(request.phase,live);
   record.phaseIndex++;record.status=request.phase+'_PASS';record.counters=live.budget.snapshot();record.history.push({phase:request.phase,status:'PASS',utc:new Date().toISOString()});save();
   console.log('DOC118_PHASE='+request.phase+'; status=PASS');
   if(record.phaseIndex===PHASES.length)requestedClose=true;
  }
  record.status=record.phaseIndex===PHASES.length?'FINISHED_REQUIRES_EVIDENCE_REVIEW':'CLOSED_BY_OPERATOR';
 }catch(error){
  record.status='STOPPED_REVIEW_REQUIRED';
  Object.assign(record,phaseFailure(error));
  console.log(record.code+'; sin reintentos automaticos.');process.exitCode=2;
 }finally{
  try{if(live.observer?.close){await live.observer.close();record.observerCloseConfirmed=true;}}catch{record.observerCloseConfirmed=false;}
  const closed=await Promise.allSettled(live.sessions.map(s=>s.context.close()));record.sessionsClosed=closed.every(r=>r.status==='fulfilled');
  try{if(live.browser)await live.browser.close();}catch{record.sessionsClosed=false;}
  for(const k of Object.keys(secrets))delete secrets[k];for(const k of Object.keys(environment))delete environment[k];
  record.sessionAttempts=live.sessionCount;record.ephemeralSecretsCleared=true;record.counters=live.budget.snapshot();record.finishedUtc=new Date().toISOString();save();
  if(record.sessionsClosed&&record.observerCloseConfirmed!==false){try{lease.release();record.runLeaseReleased=true;save();}catch{record.runLeaseReleased=false;save();}}
 }
}
if(require.main===module)main().catch(error=>{console.log(publicGuardCode(error)+'; sin detalle privado.');process.exitCode=2;});
module.exports={scope,continuationConsumption,PHASES,publicGuardCode,captureCredentials,phaseFailure};
