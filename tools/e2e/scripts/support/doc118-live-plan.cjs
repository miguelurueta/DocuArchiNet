'use strict';
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const shared=require('../../tests/support/doc111-importacion-carpetas.cjs');
const bridge=require('../../tests/support/doc118-session-bridge.cjs');
const {queryRows}=require('./doc32-e2e-odbc.cjs');
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
 live.query=async(origin,sql,args)=>{live.budget.select(origin,sql);return (dependencies.queryRows||queryRows)(sql,args,live.environment,origin==='RA'?'DOC111_E2E':'DOC111_DA_E2E');};
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
 for(const loteId of ['18','21']){
  const snapshot=await live.controls.snapshot(loteId);live.preservation[loteId]=sha(JSON.stringify(snapshot));
 }
 save(live,'preflight',{issueKey:'DOC-118',stage:'AUTHENTICATED_PREFLIGHT_PASS',actorRef:'141',ambitoRef:'2',levelRef:'7',versionRef:'38',independentSessions:2,publicationVerified:true,cabinetVerified:true,protectedLots:['18','21'],counters:live.budget.snapshot(),omittedBeforeDispatch:live.omittedAssets,productionUiExercised:false,newPlans:0,newLots:0,newDocumentEffects:0,observerQualified:false});
}
async function execute(phase,live){
 if(phase==='PREFLIGHT')return preflight(live);
 throw Error('DOC118_LIVE_PHASE_NOT_READY');
}
module.exports={execute,preflight,assertDependencies};
