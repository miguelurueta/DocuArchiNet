'use strict';
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const shared=require('../../tests/support/doc111-importacion-carpetas.cjs');
const bridge=require('../../tests/support/doc118-session-bridge.cjs');
const {queryRows}=require('./doc32-e2e-odbc.cjs');
const {prepareCase,recoverCase}=require('../../tests/support/doc118-case-preparation.cjs');
const {inspectCheckpoint,finish02,finish03}=require('../../tests/support/doc118-case-completion.cjs');
const {suppressFirstResponse}=require('../../tests/support/doc118-lost-response.cjs');
const {assertObserverReady}=require('../../tests/support/doc118-live-lifecycle.cjs');
const {connectObserver}=require('../../tests/support/doc118-debugger-observer.cjs');
const {verifySymbolFiles}=require('../../tests/support/doc118-symbols.cjs');
const check=(v,c)=>{if(!v)throw Error('DOC118_LIVE_'+c);};
const sha=b=>crypto.createHash('sha256').update(b).digest('hex');
const one=rows=>{check(Array.isArray(rows)&&rows.length===1,'READ_RESULT_AMBIGUOUS');return rows[0];};
function save(live,name,value){fs.writeFileSync(path.join(live.directory,name+'.json'),JSON.stringify(value,null,2)+'\n');}
function assertDependencies(){
 check(['createScopedBrowser','budgetedAuth','bindExistingPair'].every(k=>typeof bridge[k]==='function')&&['openSession','createDriver','createSelectControls'].every(k=>typeof shared[k]==='function'),'DEPENDENCIES_NOT_READY');
}
async function preflight(live,dependencies={}){
 assertDependencies();
 check(live.sessionCount===(live.carriedSessionAttempts||0)&&live.sessions.length===0,'PREVIOUS_SESSION_REQUIRES_REVIEW');
 const p=live.profile,grant={environment:true,account:true,select:true,tls:true,effects:false,discardable:false};
 const config=require('../../playwright.config.cjs'),chromium=dependencies.chromium||require('../../node_modules/@playwright/test').chromium;
 live.browser=await chromium.launch({headless:true,channel:config.use.channel,executablePath:config.use.executablePath});
 live.omittedAssets=0;
 const browser=bridge.createScopedBrowser(live.browser,p,live.budget,()=>live.activeCase||'COORD02',{omitNonessentialAssets:true,omitProductionSubresources:true,onOmittedAsset:()=>{live.omittedAssets++;}});
 live.readClients=[];
 for(let i=0;i<2;i++){
  if(live.observerBinding)(dependencies.assertRuntimeReady||assertRuntimeReady)(live);
  check(live.sessionCount<live.authorization.maximum.totalSessions,'SESSION_LIMIT');live.sessionCount++;
  live.mark?.('PREFLIGHT_BEFORE_LOGIN_'+(i?'B':'A'));
  const auth=await (dependencies.openSession||shared.openSession)(browser,p,live.environment,grant,{beforeDiagnosticPost:operation=>live.budget.http(live.activeCase||'COORD02',operation)});live.sessions.push(auth);
  live.mark?.('PREFLIGHT_LOGIN_'+(i?'B':'A'));
  const reader=shared.createDriver(bridge.budgetedAuth(auth,p,live.budget,()=>live.activeCase||'COORD02'),p,grant,{readOnly:true});
  await reader.refreshToken();const lot=await reader.request('ConsultarLote',{loteId:'23'});
  check(lot.loteId==='23'&&lot.nivelRef==='7','CURRENT_AUTHORITY_UNCONFIRMED');live.readClients.push(reader);
  live.mark?.('PREFLIGHT_LOT_'+(i?'B':'A'));
 }
 check(live.sessions[0].context!==live.sessions[1].context,'DISTINCT_SESSIONS_REQUIRED');
 live.rawQuery=async(origin,sql,args)=>{live.assertActive?.();return (dependencies.queryRows||queryRows)(sql,args,live.environment,origin==='RA'?'DOC111_E2E':'DOC111_DA_E2E');};
 live.query=async(origin,sql,args)=>{live.budget.select(origin,sql);return live.rawQuery(origin,sql,args);};
 live.mark?.('PREFLIGHT_PUBLICATION');
 const publication=one(await live.query('RA','SELECT v.version_id,v.perfil_id,v.estado,v.configuracion_doc,p.activo,p.version_publicada_id FROM ra_integracion_perfil_version v INNER JOIN ra_integracion_perfil p ON p.perfil_id=v.perfil_id WHERE v.version_id=? AND p.ambito_ref=?',['38','2']));
 check(String(publication.version_id)==='38'&&publication.estado==='PUBLICADA'&&Number(publication.activo)===1,'PUBLICATION_UNAVAILABLE');
 const configuration=JSON.parse(publication.configuracion_doc);
 check(String(configuration.GabineteId)==='10'&&configuration.ModoIntegridad==='COMPLETO_OBLIGATORIO'&&configuration.AmbitoIntegridad==='LOTE','PUBLICATION_CHANGED');
 live.mark?.('PREFLIGHT_CABINET');
 const cabinet=one(await live.query('DA','SELECT id_gabinete FROM system1 WHERE id_gabinete=?',['10']));check(String(cabinet.id_gabinete)==='10','CABINET_UNAVAILABLE');
 live.mark?.('PREFLIGHT_OWNER');
 live.controls=shared.createSelectControls(live.query);const historical=await live.controls.snapshot('23'),context=JSON.parse(historical.lot.contexto_doc);
 check(context.UsuarioRef==='141'&&context.AmbitoRef==='2'&&context.NivelRef==='7','CURRENT_OWNER_UNCONFIRMED');
 live.publication={perfilId:String(publication.perfil_id),versionId:'38',configuration};
 live.preservation={};
 live.mark?.('PREFLIGHT_PROTECTION');
 for(const loteId of (live.currentWindow?live.authorization.protectedLots:['18','21'])){
  const snapshot=await live.controls.snapshot(loteId);live.preservation[loteId]=sha(JSON.stringify(snapshot));
 }
 save(live,'preflight',{issueKey:'DOC-118',stage:'AUTHENTICATED_PREFLIGHT_PASS',actorRef:'141',ambitoRef:'2',levelRef:'7',versionRef:'38',independentSessions:2,publicationVerified:true,cabinetVerified:true,protectedLots:live.currentWindow?live.authorization.protectedLots:['18','21'],counters:live.budget.snapshot(),omittedBeforeDispatch:live.omittedAssets,productionUiExercised:false,newPlans:0,newLots:0,newDocumentEffects:0,observerQualified:false});
 live.protocolPhaseIndex=1;
}
function assertRuntimeReady(live){
 const result=verifySymbolFiles(fs.readFileSync(path.join(live.ROOT,'bin/GestionDocumental-Docuarchi.net.dll')),fs.readFileSync(path.join(live.ROOT,'bin/GestionDocumental-Docuarchi.net.pdb')));
 if(live.currentWindow)require('./doc118-current-window.cjs').assertEvidence(live.authorization,live.ROOT);
 const expected=live.currentWindow?live.authorization.runtimePin:JSON.parse(fs.readFileSync(path.join(live.ROOT,'Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-preflight-20261107.json'),'utf8')).runtime;
 check(result.dllSha256===expected.dllSha256&&result.pdbSha256===expected.pdbSha256,'ORDINARY_BINARY_CHANGED');
 const binding=assertObserverReady(live);
 if(!live.observer)live.observer=connectObserver({directory:binding.directory,runId:live.runId,resourceSha256:'0'.repeat(64)});
 return binding;
}
function guardedSession(auth,live){return{...auth,context:{request:{post:(...args)=>{live.assertActive?.();return auth.context.request.post(...args);}}}};}
function state(live,index){check(live.protocolPhaseIndex===index,'PROTOCOL_PHASE_ORDER_INVALID');live.assertActive?.();}
async function prepare(live,scenario,dependencies={}){
 state(live,scenario==='COORD02'?1:4);check(live.sessions.length===2&&live.sessions[0].context!==live.sessions[1].context,'DISTINCT_SESSIONS_REQUIRED');
 check(!live.preparationRequested?.[scenario],'PREPARATION_ALREADY_REQUESTED');
 live.activeCase=scenario;live.startedCaseUtc[scenario]=Date.now();
 const ready=dependencies.assertRuntimeReady||assertRuntimeReady;ready(live);
 const recovery=scenario==='COORD02'?live.authorization.preparedCaseRecovery:undefined;
 const authorityLot=recovery?.loteId||'23';
 for(const reader of live.readClients){const s=await reader.request('ConsultarLote',{loteId:authorityLot});check(s.loteId===authorityLot&&s.nivelRef===live.profile.nivelRef,'CURRENT_AUTHORITY_UNCONFIRMED');}
 live.preparationRequested={...live.preparationRequested,[scenario]:true};
 const grant={environment:true,account:true,effects:true,discardable:true,select:true,tls:true};
 const client=shared.createDriver(bridge.budgetedAuth(guardedSession(live.sessions[0],live),live.profile,live.budget,scenario),live.profile,grant,{onReference:r=>live.reference(r)});await client.refreshToken();
 const c=await (recovery?(dependencies.recoverCase||recoverCase):(dependencies.prepareCase||prepareCase))({scenario,profile:live.profile,rootNames:live.authorization.newRootNames[scenario],actorRef:live.authorization.environment.actorRef,ambitoRef:live.authorization.environment.ambitoRef,client,controls:live.controls,query:live.query,assertSourceReady:()=>ready(live),onReference:r=>live.reference(r),recovery,executionScope:live.currentWindow?{runId:live.runId}:undefined});
 const bound={...grant,scope:'DOC-118-'+scenario,runId:live.runId,baseUrl:c.profile.baseUrl,levelRef:c.profile.nivelRef,versionRef:c.profile.versionId,actorRef:c.actorRef,ambitoRef:c.ambitoRef,concurrency:true,observer:true,...Object.fromEntries(['loteId','archivoAId','archivoBId','snapshotSha256'].map(k=>[k,c.prepared[k]])),resourceSha256:c.resourceSha256};
 let sessions=live.sessions.map(s=>guardedSession(s,live));
 if(scenario==='COORD02'){
  const lost=(dependencies.suppressFirstResponse||suppressFirstResponse)({auth:sessions[0],profile:c.profile,command:c.command,resourceSha256:c.resourceSha256,checkpoint:async()=>{const proof=inspectCheckpoint(await live.controls.snapshot(c.prepared.loteId),c);return{documentoId:proof.documentoId,inventarioId:proof.inventarioId,fingerprint:proof.fingerprint};}});
  sessions=[lost.session,sessions[1]];c.lostResponse=lost;
 }
 c.grant=bound;c.pair=await (dependencies.bindExistingPair||bridge.bindExistingPair)({sessions,profile:c.profile,grant:bound,scenario,budget:live.budget});
 live.prepared[scenario]=c;live.protocolPhaseIndex++;
 save(live,'prepared-'+scenario,{issueKey:'DOC-118',scenario,loteId:c.prepared.loteId,files:[c.prepared.archivoAId,c.prepared.archivoBId],bothEligible:true,storedDocuments:0,recoveredPreparation:c.recoveredPreparation===true,integrationPass:false,counters:live.budget.snapshot()});
}
async function run(live,scenario,dependencies={}){
 state(live,scenario==='COORD02'?2:5);const c=live.prepared[scenario];check(c,'CASE_NOT_PREPARED');
 check(!c.windowRequested,'WINDOW_ALREADY_REQUESTED');c.windowRequested=true;
 (dependencies.assertRuntimeReady||assertRuntimeReady)(live);
 const observer=(dependencies.connectObserver||connectObserver)({directory:live.observerBinding.directory,runId:live.runId,resourceSha256:c.resourceSha256,peerProbe:async()=>{const s=await c.pair.clients[1].request('ConsultarLote',{loteId:c.prepared.loteId});check(s.loteId===c.prepared.loteId&&s.nivelRef===c.profile.nivelRef,'PEER_AUTHORITY_UNCONFIRMED');}});
 live.observer=observer;c.observer=observer;
 try{
 await observer.arm(c.targets);
 let settle;c.firstSettlement=new Promise(resolve=>{settle=resolve;});
 c.window=await (dependencies.runPreparedWindow||bridge.runPreparedWindow)({pair:c.pair,profile:c.profile,grant:c.grant,scope:{runId:live.runId,resourceSha256:c.resourceSha256},prepared:c.prepared,query:live.rawQuery,observer,onFirstSettled:settle});
 check(c.window.coordination.coordinationVerified===true&&!c.window.coordination.releaseUnconfirmed,'COORDINATION_NOT_CONFIRMED');
 live.protocolPhaseIndex++;save(live,'window-'+scenario,c.window);
 }catch(error){
  save(live,'stopped-window-'+scenario,{issueKey:'DOC-118',scenario,loteId:c.prepared.loteId,files:[c.prepared.archivoAId,c.prepared.archivoBId],firstRequested:typeof error.firstRequested==='boolean'?error.firstRequested:null,firstOutcome:['FULFILLED','REJECTED','UNKNOWN'].includes(error.firstOutcome)?error.firstOutcome:'UNKNOWN',coordination:error.coordination||null,codes:Array.isArray(error.codes)?error.codes:[],releaseUnconfirmed:error.releaseUnconfirmed===true,counters:live.budget.snapshot(),integrationPass:false});
  throw error;
 }
}
async function finish(live,dependencies={}){
 state(live,3);const c=live.prepared.COORD02;
 check(!c.finishRequested,'FINISH_ALREADY_REQUESTED');c.finishRequested=true;
 c.completed=await (dependencies.finish02||finish02)({caseData:c,firstOutcome:await c.lostResponse.outcome(),client:c.pair.clients[0],controls:live.controls,observer:c.observer});
 await c.pair.close();live.protocolPhaseIndex=4;save(live,'completed-COORD02',c.completed);
}
async function verify(live,dependencies={}){
 state(live,6);const c=live.prepared.COORD03;
 check(!c.finishRequested,'FINISH_ALREADY_REQUESTED');c.finishRequested=true;
 let timer;try{const outcome=await Promise.race([c.firstSettlement,new Promise((_,reject)=>{timer=setTimeout(()=>reject(Error('DOC118_LIVE_FIRST_OUTCOME_UNCONFIRMED')),60000);})]);check(outcome.fulfilled&&outcome.estado==='CONFIRMADO','FIRST_OUTCOME_UNCONFIRMED');}finally{clearTimeout(timer);}
 c.completed=await (dependencies.finish03||finish03)({caseData:c,client:c.pair.clients[1],controls:live.controls,observer:c.observer,closeFirstClient:()=>live.sessions[0].context.close()});
 await c.pair.close();
 for(const loteId of (live.currentWindow?live.authorization.protectedLots:['18','21']))check(sha(JSON.stringify(await live.controls.snapshot(loteId)))===live.preservation[loteId],'PROTECTED_LOT_CHANGED');
 const a=live.prepared.COORD02,b=live.prepared.COORD03;
 check(a.completed.documents===2&&b.completed.documents===1&&a.prepared.loteId!==b.prepared.loteId&&new Set([a.targets.expA,a.targets.expB,b.targets.expA,b.targets.expB]).size===4,'FINAL_TOTALS_UNCONFIRMED');
 const identities=[...a.completed.identities,...b.completed.identities];check(identities.length===3&&new Set(identities.map(i=>i.documentoId)).size===3&&new Set(identities.map(i=>i.inventarioId)).size===3,'FINAL_IDENTITY_COLLISION');
 await live.observer.close();live.protocolPhaseIndex=7;
 save(live,'completed-COORD03',c.completed);
 save(live,'final-evidence',{issueKey:'DOC-118',protocolCompleted:true,protectedBaselinesUnchanged:true,roots:4,documents:3,cases:[a.completed,b.completed],counters:live.budget.snapshot(),integrationPass:false,reviewRequired:true});
}
async function execute(phase,live,dependencies={}){
 if(phase==='PREFLIGHT'){check(!live.protocolPhaseIndex,'PROTOCOL_PHASE_ORDER_INVALID');(dependencies.assertRuntimeReady||assertRuntimeReady)(live);return preflight(live,dependencies);}
 if(phase==='PREPARE02')return prepare(live,'COORD02',dependencies);
 if(phase==='RUN02')return run(live,'COORD02',dependencies);
 if(phase==='FINISH02')return finish(live,dependencies);
 if(phase==='PREPARE03')return prepare(live,'COORD03',dependencies);
 if(phase==='RUN03')return run(live,'COORD03',dependencies);
 if(phase==='VERIFY')return verify(live,dependencies);
 throw Error('DOC118_LIVE_PHASE_INVALID');
}
module.exports={execute,preflight,assertDependencies,assertRuntimeReady};
