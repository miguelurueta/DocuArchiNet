'use strict';

// Adapter for existing DOC111 sessions, transport and SELECT controls. This file
// neither obtains secrets nor attaches a debugger nor prepares business data.
const crypto=require('node:crypto');
const shared=require('./doc111-importacion-carpetas.cjs');
const {coordinateActiveCall}=require('./doc118-active-call.cjs');
const LIMITS=Object.freeze({
  COORD02:Object.freeze({AbrirImportacion:2,ListarPerfilesImportacion:2,PrepararPlan:1,ConfirmarPlan:1,RecibirArchivo:2,CREAR_EXPEDIENTE:2,MATERIALIZAR_VALORES:2,ALMACENAR:5,SolicitarDetencion:0,ConsultarLote:20,ConsultarRecursosLote:16}),
  COORD03:Object.freeze({AbrirImportacion:2,ListarPerfilesImportacion:2,PrepararPlan:1,ConfirmarPlan:1,RecibirArchivo:2,CREAR_EXPEDIENTE:2,MATERIALIZAR_VALORES:2,ALMACENAR:2,SolicitarDetencion:1,ConsultarLote:20,ConsultarRecursosLote:16})
});
class BridgeFault extends Error{constructor(code){super('DOC118_BRIDGE_'+code);this.code='DOC118_BRIDGE_'+code;}}
const check=(v,c)=>{if(!v)throw new BridgeFault(c);};
const id=v=>typeof v==='string'&&/^[1-9][0-9]{0,9}$/.test(v)&&BigInt(v)<=4294967295n;
const revision=v=>Number.isInteger(v)&&v>0&&v<4294967295;
const hash=v=>crypto.createHash('sha256').update(v).digest('hex');
const binding=(p,g)=>hash(JSON.stringify([p.baseUrl,p.module,p.nivelRef,p.versionId,p.mode,p.ignoreHttpsErrors,g.scope,g.runId,g.actorRef,g.ambitoRef,g.loteId,g.archivoAId,g.archivoBId,g.resourceSha256,g.snapshotSha256]));
const closed=(v,keys)=>check(v&&typeof v==='object'&&Object.keys(v).sort().join()===keys.slice().sort().join(),'FIELDS_INVALID');
function authorize(profile,grant,scenario){
  shared.validateProfile(profile);
  check(Object.hasOwn(LIMITS,scenario),'SCENARIO_INVALID');
  check(grant&&grant.scope==='DOC-118-'+scenario&&['environment','account','effects','discardable','select','concurrency','observer'].every(k=>grant[k]===true),'SPECIFIC_AUTHORIZATION_REQUIRED');
  check(grant.baseUrl===profile.baseUrl&&grant.levelRef===profile.nivelRef&&grant.versionRef===profile.versionId&&id(grant.actorRef)&&id(grant.ambitoRef),'AUTHORIZED_CONTEXT_CHANGED');
  check(/^[a-f0-9]{32}$/.test(grant.runId)&&['loteId','archivoAId','archivoBId'].every(k=>id(grant[k]))&&!['18','21'].includes(grant.loteId)&&grant.archivoAId!==grant.archivoBId&&['resourceSha256','snapshotSha256'].every(k=>/^[a-f0-9]{64}$/.test(grant[k])),'AUTHORIZED_RESOURCES_REQUIRED');
  check(!profile.ignoreHttpsErrors||grant.tls===true,'TLS_AUTHORIZATION_REQUIRED');
}
function createBudget(){
  const counts={COORD02:{http:0,operations:{}},COORD03:{http:0,operations:{}}};let selects=0;
  function http(scenario,operation,payload){
    check(Object.hasOwn(LIMITS,scenario),'SCENARIO_INVALID');
    const counter=counts[scenario];
    const key=operation==='AvanzarLote'?payload?.fase:operation;
    if(operation)check(Object.hasOwn(LIMITS[scenario],key)&&(counter.operations[key]||0)<LIMITS[scenario][key],'HTTP_OPERATION_LIMIT');
    check(counter.http<250,'HTTP_TOTAL_LIMIT');
    if(operation)counter.operations[key]=(counter.operations[key]||0)+1;
    counter.http++;
  }
  return Object.freeze({
    http,
    select(origin,sql){check(['RA','DA'].includes(origin)&&/^SELECT\b/.test(sql)&&!/;|\b(?:FOR\s+UPDATE|INTO\s+(?:OUTFILE|DUMPFILE)|SLEEP|BENCHMARK)\b/i.test(sql),'SELECT_REQUIRED');check(selects<240,'SELECT_LIMIT');selects++;},
    snapshot:()=>structuredClone({scenarios:counts,selects})
  });
}
function operationAt(url,profile){
  let u;try{u=new URL(url);}catch{throw new BridgeFault('REQUEST_SCOPE_INVALID');}
  const base=new URL(profile.baseUrl);
  check(u.origin===base.origin&&u.pathname.startsWith(base.pathname)&&!u.username&&!u.password,'REQUEST_SCOPE_INVALID');
  const prefix=new URL('webservice/WebServiceImportacionCarpetas.asmx/',base).pathname;
  if(u.pathname.startsWith(prefix))return u.pathname.slice(prefix.length);
  if(u.pathname===new URL('generic_control/ImportacionCarpetasUpload.ashx',base).pathname)return 'RecibirArchivo';
  return null;
}
function budgetedAuth(auth,profile,budget,scenario){
  return{...auth,context:{request:{post:(url,options)=>{
    const operation=operationAt(url,profile);check(operation,'REQUEST_SCOPE_INVALID');
    budget.http(scenario,operation,options.data?.solicitud);
    return auth.context.request.post(url,options);
  }}}};
}
function createScopedBrowser(browser,profile,budget,scenario){
  // Also reusable by the authorized read-only preflight, before resource IDs exist.
  return{newContext:async(...args)=>{
    const context=await browser.newContext({...args[0],serviceWorkers:'block'});
    try{await context.route('**/*',async route=>{
      try{
        const request=route.request(),url=request.url();
        if(/^https?:/.test(url)){
          const operation=operationAt(url,profile);let payload;
          if(operation==='AvanzarLote'){try{payload=request.postDataJSON()?.solicitud;}catch{throw new BridgeFault('REQUEST_SCOPE_INVALID');}}
          budget.http(scenario,operation,payload);
        }
        await route.continue();
      }catch{await route.abort().catch(()=>{});}
    });return context;}catch{await context.close().catch(()=>{});throw new BridgeFault('CONTEXT_BUDGET_NOT_INSTALLED');}
  }};
}
async function openPair({browser,profile,environment,grant,scenario,budget,openSession=shared.openSession}){
  authorize(profile,grant,scenario);check(budget&&['http','select','snapshot'].every(k=>typeof budget[k]==='function'),'SHARED_BUDGET_REQUIRED');
  profile=structuredClone(profile);grant=Object.freeze({...grant});const sessions=[];let closedPair=false,windowClaimed=false;
  const limitedBrowser=createScopedBrowser(browser,profile,budget,scenario);
  async function close(){
    if(closedPair)return;closedPair=true;
    const outcomes=await Promise.allSettled([...new Set(sessions.map(s=>s.context))].map(c=>c.close()));
    check(outcomes.every(o=>o.status==='fulfilled'),'SESSION_CLOSE_UNCONFIRMED');
  }
  try{
    for(let i=0;i<2;i++)sessions.push(await openSession(limitedBrowser,profile,environment,grant));
    check(sessions[0].context!==sessions[1].context,'DISTINCT_SESSIONS_REQUIRED');
    const clients=sessions.map(auth=>shared.createDriver(budgetedAuth(auth,profile,budget,scenario),profile,grant));
    for(const client of clients)await client.refreshToken();
    return Object.freeze({clients:Object.freeze(clients),budget,scenario,authorizationSha256:binding(profile,grant),close,isClosed:()=>closedPair,
      claimWindow:()=>{check(!windowClaimed,'WINDOW_ALREADY_REQUESTED');windowClaimed=true;}});
  }catch(error){
    try{await close();}catch{throw new BridgeFault('SESSION_CLOSE_UNCONFIRMED');}
    throw error instanceof BridgeFault?error:new BridgeFault('SESSION_OPEN_UNCONFIRMED');
  }
}
function prepareCommand(prepared){
  closed(prepared,['loteId','archivoAId','archivoBId','revisionA','claveA','claveB','snapshotSha256']);
  check(['loteId','archivoAId','archivoBId'].every(k=>id(prepared[k]))&&!['18','21'].includes(prepared.loteId)&&prepared.archivoAId!==prepared.archivoBId&&revision(prepared.revisionA),'PREPARED_RESOURCE_INVALID');
  check(['claveA','claveB'].every(k=>typeof prepared[k]==='string'&&/^[A-Za-z0-9_-]{1,64}$/.test(prepared[k]))&&/^[a-f0-9]{64}$/.test(prepared.snapshotSha256),'PREPARED_RESOURCE_INVALID');
  return{loteId:prepared.loteId,recursoId:prepared.archivoAId,fase:'ALMACENAR',revisionEsperada:prepared.revisionA,claveOperacion:prepared.claveA};
}
const one=(rows,predicate)=>{const found=rows.filter(predicate);check(found.length===1,'PERSISTED_RESOURCE_CHANGED');return found[0];};
function inspectSnapshot(view,prepared,grant,initial){
  let context;try{context=JSON.parse(view.lot.contexto_doc);}catch{throw new BridgeFault('PERSISTED_CONTEXT_INVALID');}
  check(String(view.lot.lote_id)===prepared.loteId&&context.UsuarioRef===grant.actorRef&&context.AmbitoRef===grant.ambitoRef&&context.NivelRef===grant.levelRef&&hash(view.lot.config_doc)===prepared.snapshotSha256,'PERSISTED_CONTEXT_CHANGED');
  check(view.roots.length===2&&view.files.length===2&&Number(view.lot.total_raices)===2&&Number(view.lot.total_archivos)===2,'ORIGINAL_INVENTORY_CHANGED');
  const a=one(view.files,f=>String(f.archivo_id)===prepared.archivoAId),b=one(view.files,f=>String(f.archivo_id)===prepared.archivoBId);
  check(String(a.raiz_id)!==String(b.raiz_id)&&[a,b].every(f=>f.estado_recepcion==='RECIBIDO'&&f.valores_efectivos_doc&&revision(Number(f.revision))),'RESOURCE_NOT_ELIGIBLE');
  for(const file of [a,b])check(one(view.roots,r=>String(r.raiz_id)===String(file.raiz_id)).estado==='EXPEDIENTE_CONFIRMADO','RESOURCE_NOT_ELIGIBLE');
  check(b.estado_efecto==='NO_INICIADO'&&!b.intento_activo_id&&!b.resultado_doc,'SECOND_RESOURCE_CHANGED');
  if(initial)check(view.lot.estado==='EN_CURSO'&&!Number(view.lot.detencion_solicitada)&&a.estado_efecto==='NO_INICIADO'&&!a.intento_activo_id&&!a.resultado_doc&&Number(a.revision)===prepared.revisionA&&view.roots.every(r=>!r.intento_activo_id)&&!view.attempts.some(t=>t.fase!=='RECEPCION'&&['RESERVADO','EN_CURSO','INCIERTO'].includes(t.estado)),'INITIAL_CLAIM_CHANGED');
  return b;
}
async function expectBlocked(client,command,uncertain){
  try{await client.request('AvanzarLote',command);}catch(error){
    if(error.httpStatus===409&&(error.serverCode==='OPERACION_EN_CURSO'||uncertain&&error.serverCode==='RESULTADO_INCIERTO'))return error.serverCode;
    throw new BridgeFault('BLOCK_NOT_PROVEN');
  }
  throw new BridgeFault('BLOCK_NOT_PROVEN');
}
async function runPreparedWindow({pair,profile,grant,scope,prepared,query,observer}){
  authorize(profile,grant,pair.scenario);check(!pair.isClosed(),'SESSION_PAIR_CLOSED');
  check(pair.authorizationSha256===binding(profile,grant),'SESSION_AUTHORIZATION_CHANGED');
  const command=prepareCommand(prepared);
  closed(scope,['runId','resourceSha256']);
  check(scope.runId===grant.runId&&scope.resourceSha256===hash(JSON.stringify(command))&&scope.resourceSha256===grant.resourceSha256&&prepared.snapshotSha256===grant.snapshotSha256&&['loteId','archivoAId','archivoBId'].every(k=>prepared[k]===grant[k]),'REQUEST_NOT_PINNED');
  pair.claimWindow();
  prepared=Object.freeze({...prepared});scope=Object.freeze({...scope});grant=Object.freeze({...grant});
  const controls=shared.createSelectControls(async(origin,sql,args)=>{pair.budget.select(origin,sql);return query(origin,sql,args);});
  const codes=[];let firstRequested=false;
  try{
    inspectSnapshot(await controls.snapshot(prepared.loteId),prepared,grant,true);
    for(const client of pair.clients){const state=await client.request('ConsultarLote',{loteId:prepared.loteId});check(state.loteId===prepared.loteId&&state.nivelRef===profile.nivelRef&&!state.detencionSolicitada,'CURRENT_AUTHORITY_NOT_PROVEN');}
    const actions=pair.scenario==='COORD02'?[
      {kind:'SAME_COMMAND',execute:async()=>{codes.push(await expectBlocked(pair.clients[1],command,true));}},
      {kind:'OTHER_FILE',execute:async()=>{const fresh=inspectSnapshot(await controls.snapshot(prepared.loteId),prepared,grant,false);codes.push(await expectBlocked(pair.clients[1],{loteId:prepared.loteId,recursoId:prepared.archivoBId,fase:'ALMACENAR',revisionEsperada:Number(fresh.revision),claveOperacion:prepared.claveB},false));}}
    ]:[{kind:'STOP',execute:async()=>{
      const current=await pair.clients[1].request('ConsultarLote',{loteId:prepared.loteId});check(revision(current.revision),'LOT_REVISION_INVALID');
      const stopped=await pair.clients[1].request('SolicitarDetencion',{loteId:prepared.loteId,revisionEsperada:current.revision});
      check(stopped.loteId===prepared.loteId&&stopped.detencionSolicitada===true&&stopped.estado==='EN_CURSO','STOP_NOT_CONFIRMED');codes.push('DETENCION_SOLICITADA');
    }}];
    const coordination=await coordinateActiveCall({scenario:pair.scenario,scope,first:()=>{firstRequested=true;return pair.clients[0].request('AvanzarLote',command);},actions,observer});
    return{scenario:pair.scenario,loteId:prepared.loteId,archivoAId:prepared.archivoAId,archivoBId:prepared.archivoBId,codes,coordination,budget:pair.budget.snapshot(),businessEvidenceVerified:false,integrationPass:false};
  }catch(error){
    const safe=error instanceof BridgeFault?error:new BridgeFault('WINDOW_NOT_CONFIRMED');
    safe.loteId=prepared.loteId;safe.firstRequested=firstRequested;
    safe.firstOutcome=['FULFILLED','REJECTED','UNKNOWN'].includes(error.coordination?.firstOutcome)?error.coordination.firstOutcome:'UNKNOWN';
    safe.releaseUnconfirmed=error.coordination?.releaseUnconfirmed===true;
    throw safe;
  }
}
module.exports={LIMITS,authorize,createBudget,createScopedBrowser,budgetedAuth,openPair,prepareCommand,runPreparedWindow};
