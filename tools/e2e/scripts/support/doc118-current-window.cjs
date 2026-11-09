'use strict';
// Validation/data adapter for the existing DOC118 phase engine. No transport,
// authentication or debugger implementation is duplicated here.
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const AUTH_REL='openspec/changes/doc-118-produccion-importacion-carga-carpeta-lote-recuperacion/execution-current-window.json';
const LIMITS=Object.freeze({totalSessions:2,loginAttemptsPerSession:1,newPlans:2,newLots:2,newRoots:4,newPdfs:4,newExpedientes:4,newStoredDocuments:3,stopRequests:1,pauseMilliseconds:30000,selectStatementsTotal:240,allHttpGlobal:1000,allHttpPerCase:{COORD02:925,COORD03:75},caseDurationMilliseconds:600000});
const HISTORY=Object.freeze({httpRequests:1277,selectStatements:113,sessions:9});
const check=(v,c)=>{if(!v)throw Error('DOC118_CURRENT_'+c);};
const digest=p=>crypto.createHash('sha256').update(fs.readFileSync(p)).digest('hex');
function rootNames(runId,scenario){check(/^[a-f0-9]{32}$/.test(runId)&&['COORD02','COORD03'].includes(scenario),'RESOURCE_SCOPE_INVALID');return ['A','B'].map(slot=>'DOC118-C'+scenario.slice(-2)+'-'+slot+'-20261109-'+runId.slice(0,8));}
function fileNames(scenario){check(['COORD02','COORD03'].includes(scenario),'RESOURCE_SCOPE_INVALID');return ['A','B'].map(slot=>'DOC118-C'+scenario.slice(-2)+'-'+slot+'-20261107.pdf');}
function scope(a,p){
 check(a?.issueKey==='DOC-118'&&a.kind==='DOC118_CURRENT_WINDOW'&&a.status==='AUTHORIZED'&&a.executionStatus==='CONTINUATION_AUTHORIZED_AWAITING_MASKED_CAPTURE'&&a.automaticPhases===true&&a.businessCasesAuthorized===true,'AUTHORIZATION_REQUIRED');
 check(/^[a-f0-9]{32}$/.test(a.runId)&&a.environment?.baseUrl==='https://localhost/GestionDocumental-Docuarchi.net/'&&a.environment.baseUrl===p.baseUrl&&a.environment.actorRef==='141'&&a.environment.ambitoRef==='2'&&a.environment.levelRef==='7'&&a.environment.versionRef==='38'&&a.environment.localTlsExceptionAuthorized===true,'ENVIRONMENT_CHANGED');
 check(p.module==='GESTOR'&&p.nivelRef==='7'&&p.versionId==='38'&&p.mode==='LOTE'&&p.odbcDsn==='MembershipUsers'&&p.docuarchiOdbcDsn==='MembershipUsers','PROFILE_CHANGED');
 check(JSON.stringify(a.maximum)===JSON.stringify(LIMITS)&&JSON.stringify(a.historicalTotals)===JSON.stringify(HISTORY),'LIMITS_OR_HISTORY_CHANGED');
 check(['COORD02','COORD03'].every(s=>JSON.stringify(a.newRootNames?.[s])===JSON.stringify(rootNames(a.runId,s))),'RESOURCE_NAMES_CHANGED');
 check(JSON.stringify(a.protectedLots)===JSON.stringify(['18','21','24'])&&JSON.stringify(a.protectedAttempts)===JSON.stringify(['41','53','69'])&&!a.preparedCaseRecovery&&a.sessionReuse==='TWO_SHARED_INDEPENDENT_SESSIONS','PROTECTION_CHANGED');
 check(a.runtimePin&&['dllSha256','pdbSha256','storageSha256'].every(k=>/^[a-f0-9]{64}$/.test(a.runtimePin[k]))&&a.historicalEvidence&&a.sourceBindingEvidence,'EVIDENCE_REQUIRED');
 return crypto.createHash('sha256').update(JSON.stringify(a)).digest('hex');
}
function assertEvidence(a,root){
 for(const e of [a.historicalEvidence,a.sourceBindingEvidence]){check(e&&typeof e.path==='string'&&e.path.startsWith('Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/')&&!e.path.includes('..')&&/^[a-f0-9]{64}$/.test(e.sha256),'EVIDENCE_PATH_INVALID');check(digest(path.join(root,e.path))===e.sha256,'EVIDENCE_CHANGED');}
 const bound=JSON.parse(fs.readFileSync(path.join(root,a.sourceBindingEvidence.path),'utf8'));
 check(bound.status==='SOURCE_BINDING_OBSERVER_ONLY_PASS'&&bound.acceptance?.sourceBindingConfirmed===true&&bound.acceptance.sdkDetachReleaseConfirmed===true,'SOURCE_BINDING_UNCONFIRMED');
 for(const [key,file]of [['dllSha256','bin/GestionDocumental-Docuarchi.net.dll'],['pdbSha256','bin/GestionDocumental-Docuarchi.net.pdb'],['storageSha256','workflow/ClassAlmacenamiento.vb']])check(bound.pinnedFiles?.find(f=>f.path===file)?.sha256===a.runtimePin[key],'RUNTIME_PIN_NOT_BOUND_TO_EVIDENCE');
 const history=JSON.parse(fs.readFileSync(path.join(root,a.historicalEvidence.path),'utf8'));
 check(JSON.stringify(history.cumulativeAcrossHistoricalAndP4b)===JSON.stringify(HISTORY),'HISTORICAL_TOTALS_CHANGED');
 check(digest(path.join(root,'workflow/ClassAlmacenamiento.vb'))===a.runtimePin.storageSha256,'STORAGE_SOURCE_CHANGED');
}
function initialState(a){check(a.kind==='DOC118_CURRENT_WINDOW'&&JSON.stringify(a.historicalTotals)===JSON.stringify(HISTORY),'HISTORY_REQUIRED');return {sessionAttempts:0,counters:{scenarios:{COORD02:{http:0,operations:{}},COORD03:{http:0,operations:{}}},selects:0}};}
function aggregate(a,counters,sessionAttempts){check(Number.isInteger(sessionAttempts)&&sessionAttempts>=0&&sessionAttempts<=2,'SESSION_LIMIT');return {httpRequests:a.historicalTotals.httpRequests+counters.scenarios.COORD02.http+counters.scenarios.COORD03.http,selectStatements:a.historicalTotals.selectStatements+counters.selects,sessions:a.historicalTotals.sessions+sessionAttempts};}
module.exports={AUTH_REL,LIMITS,HISTORY,rootNames,fileNames,scope,assertEvidence,initialState,aggregate};
