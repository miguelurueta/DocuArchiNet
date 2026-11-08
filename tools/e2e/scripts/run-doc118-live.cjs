'use strict';
// Interactive DOC118 console. Secrets remain in this process; the local command
// queue contains only closed phase names and reviewed implementation hashes.
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const tty=require('./support/interactive-e2e-console.cjs');
const {validateProfile}=require('../tests/support/doc111-importacion-carpetas.cjs');
const {createBudget}=require('../tests/support/doc118-session-bridge.cjs');
const ROOT=path.resolve(__dirname,'../../..'),CHANGE='doc-118-produccion-importacion-carga-carpeta-lote-recuperacion';
const AUTH=path.join(ROOT,'openspec/changes',CHANGE,'execution-authorization.json');
const IMPL=path.join(__dirname,'support/doc118-live-plan.cjs');
const PHASES=['PREFLIGHT','PREPARE02','RUN02','FINISH02','PREPARE03','RUN03','VERIFY'];
const STAGES=new Set(['PREFLIGHT_BEFORE_LOGIN_A','PREFLIGHT_LOGIN_A','PREFLIGHT_LOT_A','PREFLIGHT_BEFORE_LOGIN_B','PREFLIGHT_LOGIN_B','PREFLIGHT_LOT_B','PREFLIGHT_PUBLICATION','PREFLIGHT_CABINET','PREFLIGHT_OWNER','PREFLIGHT_PROTECTION']);
const sha=b=>crypto.createHash('sha256').update(b).digest('hex');
const read=p=>JSON.parse(fs.readFileSync(p,'utf8').replace(/^\uFEFF/,''));
const check=(v,c)=>{if(!v)throw Error('DOC118_LIVE_'+c);};
function scope(a,p){
 check(a.issueKey==='DOC-118'&&a.status==='AUTHORIZED'&&a.environment.actorRef==='141'&&a.environment.ambitoRef==='2'&&a.environment.levelRef==='7'&&a.environment.versionRef==='38','AUTHORIZATION_CHANGED');
 check(a.environment.baseUrl===p.baseUrl&&p.nivelRef==='7'&&p.versionId==='38'&&p.module==='GESTOR'&&p.odbcDsn==='MembershipUsers'&&p.docuarchiOdbcDsn==='MembershipUsers','PROFILE_CHANGED');
 check(a.maximum.newLots===2&&a.maximum.newExpedientes===4&&a.maximum.newStoredDocuments===3&&a.maximum.selectStatementsTotal===240&&a.maximum.pauseMilliseconds===30000&&a.maximum.totalSessions===5&&a.environment.localTlsExceptionAuthorized===true,'LIMITS_CHANGED');
 check(a.maximum.allHttpPerCase?.COORD02===925&&a.maximum.allHttpPerCase?.COORD03===75&&a.maximum.allHttpGlobal===1000&&a.sessionReuse==='TWO_SHARED_INDEPENDENT_SESSIONS'&&a.continuationApproval?.status==='AUTHORIZED'&&a.budgetContinuationApproval?.status==='AUTHORIZED','CONTINUATION_NOT_AUTHORIZED');
 return sha(JSON.stringify({environment:a.environment,maximum:a.maximum,newRootNames:a.newRootNames,debugger:a.debugger,protectedHistoricalAttempts:a.protectedHistoricalAttempts}));
}
function continuationConsumption(a,proof){
 // A previous approval is not permission to retry after a subsequent stop.
 check(a.executionStatus==='CONTINUATION_AUTHORIZED_AWAITING_MASKED_CAPTURE','CONTINUATION_REVIEW_REQUIRED');
 check(proof?.issueKey==='DOC-118'&&proof.status==='STOPPED_REVIEW_REQUIRED'&&proof.sessionsClosed===true&&proof.ephemeralSecretsCleared===true&&Number.isInteger(proof.sessionAttempts),'CARRIED_STATE_CHANGED');
 const consumed=a.consumed,counters=proof.counters;
 check(consumed&&counters&&proof.sessionAttempts===consumed.totalSessionAttempts&&proof.sessionAttempts===consumed.loginAttempts&&proof.sessionAttempts>=0&&proof.sessionAttempts<=a.maximum.totalSessions&&counters.selects===consumed.selectStatements&&['COORD02','COORD03'].every(k=>counters.scenarios?.[k]?.http===consumed.allHttp?.[k]),'CARRIED_STATE_CHANGED');
 createBudget({allHttpPerCase:a.maximum.allHttpPerCase,allHttpGlobal:a.maximum.allHttpGlobal,carried:counters});
 check(proof.sessionAttempts+2<=a.maximum.totalSessions&&['COORD02','COORD03'].every(k=>counters.scenarios[k].http<a.maximum.allHttpPerCase[k]),'CONTINUATION_CAPACITY_INSUFFICIENT');
 return structuredClone({sessionAttempts:proof.sessionAttempts,counters});
}
async function main(args=process.argv.slice(2)){
 process.title='DOC118 - Captura enmascarada E2E';
 check(args.length===2&&args[0]==='--run'&&/^[a-f0-9]{32}$/.test(args[1]),'ARGUMENT_INVALID');tty.requireInteractiveConsole();
 const runId=args[1],directory=path.join(ROOT,'.tmp','doc118-live-'+runId);check(!fs.existsSync(directory),'PREVIOUS_SESSION_REQUIRES_REVIEW');
 const profile=validateProfile(read(path.join(ROOT,'tools/e2e/profiles/doc111-importacion-carpetas-lote-20261102.runtime.json'))),authorization=read(AUTH),pin=scope(authorization,profile);
 require(IMPL).assertDependencies(); // Fail before secret entry if a shared binding is unavailable.
 check(['doc118-login-preflight-20261107.json','doc118-continuation-preflight-20261107.json','doc118-authenticated-preflight-20261107.json'].includes(authorization.consumptionEvidence),'CARRIED_STATE_CHANGED');
 const carried=continuationConsumption(authorization,read(path.join(ROOT,'Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion',authorization.consumptionEvidence)));
 fs.mkdirSync(directory);
 const record={issueKey:'DOC-118',runId,pid:process.pid,status:'AWAITING_CREDENTIALS',createdUtc:new Date().toISOString(),phaseIndex:0,authorizationScopeSha256:pin,history:[],counters:null,references:[],carriedSessionAttempts:carried.sessionAttempts,carriedCounters:carried.counters};
 const statusFile=path.join(directory,'status.json'),requestFile=path.join(directory,'request.json');
 const save=()=>fs.writeFileSync(statusFile,JSON.stringify(record,null,2)+'\n');save();
 const secrets={},environment=Object.fromEntries(Object.entries(process.env).filter(([k])=>['PATH','Path','SystemRoot','WINDIR','TEMP','TMP','ComSpec','PATHEXT'].includes(k)));
 const live={ROOT,directory,runId,profile,authorization,budget:createBudget({allHttpPerCase:authorization.maximum.allHttpPerCase,allHttpGlobal:authorization.maximum.allHttpGlobal,carried:carried.counters}),environment,sessions:[],prepared:{},browser:null,observer:null,sessionCount:carried.sessionAttempts,carriedSessionAttempts:carried.sessionAttempts,activeCase:'COORD02',startedCaseUtc:{},
   reference(ref){const safe={};for(const key of ['planId','loteId','raizId','archivoId','expedienteId','relacionNivelId','intentoId','documentoId','inventarioId'])if(typeof ref[key]==='string'&&/^[1-9][0-9]{0,9}$/.test(ref[key]))safe[key]=ref[key];record.references.push(safe);save();},
   mark(stage){check(STAGES.has(stage),'STAGE_INVALID');record.status=stage;record.lastStage=stage;record.sessionAttempts=live.sessionCount;record.counters=live.budget.snapshot();save();}};
 record.counters=live.budget.snapshot();save();
 let requestedClose=false;
 try{
  console.log('DOC-118: continuacion autorizada. Entradas solo en memoria; sin reintentos automaticos.');
  await tty.collectValue(secrets,'user','Login de aplicacion de la cuenta autorizada (usuario 141)',{secret:true});
  await tty.collectValue(secrets,'password','Contrasena de aplicacion',{secret:true});
  await tty.collectValue(secrets,'dbUser','Cuenta MySQL exclusivamente SELECT',{secret:true});
  await tty.collectValue(secrets,'dbPassword','Contrasena MySQL SELECT',{secret:true});
  Object.assign(environment,{DOC111_E2E_MODULE:profile.module,DOC111_E2E_USER:secrets.user,DOC111_E2E_PASSWORD:secrets.password});
  for(const [prefix,dsn]of [['DOC111_E2E',profile.odbcDsn],['DOC111_DA_E2E',profile.docuarchiOdbcDsn]])Object.assign(environment,{[prefix+'_ODBC_DSN']:dsn,[prefix+'_MYSQL_USER']:secrets.dbUser,[prefix+'_MYSQL_PASSWORD']:secrets.dbPassword});
  for(const k of Object.keys(secrets))delete secrets[k];
  record.status='CREDENTIALS_READY';save();console.log('Credenciales capturadas en memoria. Esperando la fase revisada; no se han iniciado sesiones.');
  const expires=Date.now()+30*60*1000;
  while(!requestedClose){
   check(Date.now()<expires,'SESSION_EXPIRED');
   if(!fs.existsSync(requestFile)){await new Promise(r=>setTimeout(r,250));continue;}
   const request=read(requestFile);check(Object.keys(request).sort().join()==='implementationSha256,phase,sequence','REQUEST_INVALID');
   check(request.sequence===record.phaseIndex+1&&(request.phase==='CLOSE'||request.phase===PHASES[record.phaseIndex]),'PHASE_ORDER_INVALID');
   fs.renameSync(requestFile,path.join(directory,'request-'+request.sequence+'.consumed.json'));
   if(request.phase==='CLOSE'){requestedClose=true;break;}
   check(scope(read(AUTH),profile)===pin,'AUTHORIZATION_CHANGED');
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
  const own=['ARGUMENT_INVALID','PREVIOUS_SESSION_REQUIRES_REVIEW','AUTHORIZATION_CHANGED','PROFILE_CHANGED','LIMITS_CHANGED','CONTINUATION_NOT_AUTHORIZED','CARRIED_STATE_CHANGED','REQUEST_INVALID','PHASE_ORDER_INVALID','IMPLEMENTATION_CHANGED','SESSION_EXPIRED'];
  const known=['DOC111_LOGIN_CREDENTIALS_REJECTED','DOC111_LOGIN_ACCOUNT_NOT_FOUND','DOC111_LOGIN_ACCOUNT_BLOCKED','DOC111_LOGIN_SECOND_FACTOR_REQUIRED','DOC111_LOGIN_HTTP_REJECTED','DOC111_SESSION_NOT_ESTABLISHED','DOC111_IMPORT_ACCESS_DENIED','DOC111_IMPORT_CAPABILITY_UNAVAILABLE'];
  record.code=known.includes(error?.message)?error.message:own.map(c=>'DOC118_LIVE_'+c).includes(error?.message)?error.message:'DOC118_LIVE_PHASE_UNCONFIRMED';
  console.log(record.code+'; sin reintentos automaticos.');process.exitCode=2;
 }finally{
  try{if(live.observer?.close)await live.observer.close();}catch{record.observerCloseConfirmed=false;}
  const closed=await Promise.allSettled(live.sessions.map(s=>s.context.close()));record.sessionsClosed=closed.every(r=>r.status==='fulfilled');
  try{if(live.browser)await live.browser.close();}catch{record.sessionsClosed=false;}
  for(const k of Object.keys(secrets))delete secrets[k];for(const k of Object.keys(environment))delete environment[k];
  record.sessionAttempts=live.sessionCount;record.ephemeralSecretsCleared=true;record.counters=live.budget.snapshot();record.finishedUtc=new Date().toISOString();save();
 }
}
if(require.main===module)main().catch(()=>{console.log('DOC118_LIVE_START_UNCONFIRMED; sin detalle privado.');process.exitCode=2;});
module.exports={scope,continuationConsumption,PHASES};
