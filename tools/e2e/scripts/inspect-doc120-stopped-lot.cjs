'use strict';
const fs=require('node:fs'),path=require('node:path');
const {collectValue,requireInteractiveConsole}=require('./support/interactive-e2e-console.cjs');
const {queryRows}=require('./support/doc32-e2e-odbc.cjs');
const ROOT=path.resolve(__dirname,'../../..'),SCOPE=['50','141','2'];
const LOT=['RECIBIENDO','LISTO','EN_CURSO','PENDIENTE_VERIFICACION','DETENIDO','COMPLETADO','PARCIAL','FALLIDO'];
const RECEIPT=['PENDIENTE','RECIBIENDO','RECIBIDO','RECHAZADO','INCIERTO','CANCELADO'];
const EFFECT=['NO_INICIADO','EN_CURSO','ALMACENADO','COMPLETADO','RECHAZADO','INCIERTO','CANCELADO'];
const ATTEMPT=['RESERVADO','EN_CURSO','CONFIRMADO','RECHAZADO','INCIERTO'];
const PHASE=['RECEPCION','CREAR_EXPEDIENTE','ALMACENAR','INCORPORAR'];
const check=(ok,code='RESULT_INVALID')=>{if(!ok)throw Error('DOC120_STOP_DIAGNOSTIC_'+code);};
const id=v=>typeof v==='string'&&/^[1-9][0-9]{0,9}$/.test(v)&&Number(v)<=4294967295;
function number(v){check(/^(0|[1-9][0-9]*)$/.test(String(v)));const n=Number(v);check(Number.isSafeInteger(n));return n;}
function token(v,values){check(values.includes(v));return v;}
function reference(v,nullable=false){if(nullable&&v===null)return null;check(id(String(v)));return String(v);}
function date(v){if(v===null)return null;check(typeof v==='string'&&/^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}$/.test(v));return v;}
function queries(){
 const header="SELECT l.lote_id,l.plan_id,l.solicitante_ref,l.ambito_ref,l.nivel_ref,l.version_id,l.estado,l.revision,l.detencion_solicitada,l.total_raices,l.total_archivos,(SELECT COUNT(*) FROM ra_integracion_raiz r WHERE r.lote_id=l.lote_id) AS roots_observed,(SELECT COUNT(*) FROM ra_integracion_raiz r WHERE r.lote_id=l.lote_id AND r.identidad_expediente_doc IS NOT NULL) AS roots_with_identity,(SELECT COUNT(*) FROM ra_integracion_raiz r WHERE r.lote_id=l.lote_id AND r.intento_activo_id IS NOT NULL) AS roots_with_active_marker,(SELECT COUNT(*) FROM ra_integracion_raiz r WHERE r.lote_id=l.lote_id AND r.estado IN ('CREANDO','INCIERTO')) AS roots_unresolved,(SELECT COALESCE(MAX(e.evento_id),0) FROM ra_integracion_evento e WHERE e.lote_id=l.lote_id) AS latest_event,e.evento_id AS stop_event,e.actor_ref AS stop_actor,DATE_FORMAT(e.ocurrido_utc,'%Y-%m-%d %H:%i:%s') AS stop_utc FROM ra_integracion_lote l LEFT JOIN ra_integracion_evento e ON e.evento_id=(SELECT MAX(x.evento_id) FROM ra_integracion_evento x WHERE x.lote_id=l.lote_id AND x.accion='SOLICITAR_DETENCION') WHERE l.lote_id=? AND l.solicitante_ref=? AND l.ambito_ref=?";
 const scoped=' FROM ra_integracion_lote l INNER JOIN ra_integracion_raiz r ON r.lote_id=l.lote_id INNER JOIN ra_integracion_archivo f ON f.raiz_id=r.raiz_id WHERE l.lote_id=? AND l.solicitante_ref=? AND l.ambito_ref=?';
 return[
  {name:'before',sql:header,parameters:[...SCOPE]},
  {name:'files',sql:'SELECT f.estado_recepcion,f.estado_efecto,COUNT(*) AS cantidad,SUM(CASE WHEN f.intento_activo_id IS NULL THEN 0 ELSE 1 END) AS active_markers,SUM(CASE WHEN f.resultado_doc IS NULL THEN 0 ELSE 1 END) AS with_result,SUM(CASE WHEN f.valores_efectivos_doc IS NULL THEN 0 ELSE 1 END) AS with_values'+scoped+' GROUP BY f.estado_recepcion,f.estado_efecto LIMIT 65',parameters:[...SCOPE]},
  {name:'attempts',sql:'SELECT i.fase,i.estado,COUNT(*) AS cantidad FROM ra_integracion_lote l INNER JOIN ra_integracion_intento i ON i.lote_id=l.lote_id WHERE l.lote_id=? AND l.solicitante_ref=? AND l.ambito_ref=? GROUP BY i.fase,i.estado LIMIT 33',parameters:[...SCOPE]},
  {name:'unresolved',sql:"SELECT i.intento_id,i.raiz_id,i.archivo_id,i.fase,i.estado,DATE_FORMAT(i.inicio_utc,'%Y-%m-%d %H:%i:%s') AS inicio_utc,DATE_FORMAT(i.ultimo_contacto_utc,'%Y-%m-%d %H:%i:%s') AS ultimo_contacto_utc,DATE_FORMAT(i.fin_utc,'%Y-%m-%d %H:%i:%s') AS fin_utc,CASE WHEN i.evidencia_doc IS NULL THEN 0 ELSE 1 END AS evidence_present FROM ra_integracion_lote l INNER JOIN ra_integracion_intento i ON i.lote_id=l.lote_id LEFT JOIN ra_integracion_raiz r ON r.raiz_id=i.raiz_id AND r.lote_id=l.lote_id LEFT JOIN ra_integracion_archivo f ON f.archivo_id=i.archivo_id AND f.raiz_id=r.raiz_id WHERE l.lote_id=? AND l.solicitante_ref=? AND l.ambito_ref=? AND (i.estado IN ('RESERVADO','EN_CURSO','INCIERTO') OR r.intento_activo_id=i.intento_id OR f.intento_activo_id=i.intento_id) ORDER BY i.intento_id LIMIT 51",parameters:[...SCOPE]},
  {name:'after',sql:header,parameters:[...SCOPE]}
 ];
}
function sanitize(name,rows){
 check(Array.isArray(rows));
 if(name==='before'||name==='after'){
  check(rows.length===1,'SCOPE_UNAVAILABLE');const r=rows[0];
  check(String(r.lote_id)===SCOPE[0]&&String(r.solicitante_ref)===SCOPE[1]&&String(r.ambito_ref)===SCOPE[2],'SCOPE_UNAVAILABLE');
  const result={lotId:reference(r.lote_id),planId:reference(r.plan_id),ownerRef:reference(r.solicitante_ref),companyRef:reference(r.ambito_ref),levelId:reference(r.nivel_ref),versionId:reference(r.version_id),state:token(r.estado,LOT),revision:number(r.revision),stopRequested:number(r.detencion_solicitada),roots:number(r.total_raices),files:number(r.total_archivos),rootsObserved:number(r.roots_observed),rootsWithIdentity:number(r.roots_with_identity),rootActiveMarkers:number(r.roots_with_active_marker),rootsUnresolved:number(r.roots_unresolved),latestEvent:number(r.latest_event),stopEvent:reference(r.stop_event,true),stopActor:reference(r.stop_actor,true),stopUtc:date(r.stop_utc)};
  check(result.roots===1&&result.files===1000&&result.rootsObserved===1&&result.revision>0&&result.stopRequested<=1,'SCOPE_CHANGED');return result;
 }
 if(name==='files'){check(rows.length>0&&rows.length<=64);return rows.map(r=>({receipt:token(r.estado_recepcion,RECEIPT),effect:token(r.estado_efecto,EFFECT),count:number(r.cantidad),activeMarkers:number(r.active_markers),withResult:number(r.with_result),withValues:number(r.with_values)}));}
 if(name==='attempts'){check(rows.length>0&&rows.length<=32);return rows.map(r=>({phase:token(r.fase,PHASE),state:token(r.estado,ATTEMPT),count:number(r.cantidad)}));}
 check(name==='unresolved'&&rows.length<=50,'TOO_MANY_UNRESOLVED');
 return rows.map(r=>({attemptId:reference(r.intento_id),rootId:reference(r.raiz_id,true),fileId:reference(r.archivo_id,true),phase:token(r.fase,PHASE),state:token(r.estado,ATTEMPT),startedUtc:date(r.inicio_utc),lastContactUtc:date(r.ultimo_contacto_utc),finishedUtc:date(r.fin_utc),evidencePresent:number(r.evidence_present)}));
}
async function inspect(read,onStage=()=>{}){
 const result={};for(const q of queries()){onStage(q.name);result[q.name]=sanitize(q.name,await read(q.sql,q.parameters));}
 const same=JSON.stringify(result.before)===JSON.stringify(result.after);
 const total=result.files.reduce((n,r)=>n+r.count,0);check(total===result.after.files,'INVENTORY_CHANGED');
 const counts={received:0,completed:0,completedWithResult:0,fileActiveMarkers:0,activeAttempts:0,uncertainAttempts:0,activeFiles:0,uncertainFiles:0};
 for(const r of result.files){if(r.receipt==='RECIBIDO')counts.received+=r.count;if(r.receipt==='RECIBIENDO'||r.effect==='EN_CURSO')counts.activeFiles+=r.count;if(r.receipt==='INCIERTO'||r.effect==='INCIERTO')counts.uncertainFiles+=r.count;if(r.effect==='COMPLETADO'){counts.completed+=r.count;counts.completedWithResult+=r.withResult;}counts.fileActiveMarkers+=r.activeMarkers;}
 for(const r of result.attempts){if(['RESERVADO','EN_CURSO'].includes(r.state))counts.activeAttempts+=r.count;if(r.state==='INCIERTO')counts.uncertainAttempts+=r.count;}
 result.summary={stableRevisionAndEvent:same,counts,pendingStorageConfirmation:total-counts.completed,reason:!same?'CHANGED_DURING_READ':counts.uncertainAttempts||counts.uncertainFiles||result.after.rootsUnresolved?'UNCERTAIN_ATTEMPTS':counts.activeAttempts||counts.activeFiles||counts.fileActiveMarkers||result.after.rootActiveMarkers?'ACTIVE_ATTEMPTS_OR_MARKERS':counts.completed!==counts.completedWithResult?'INCONSISTENT_CONTROL_STATE':result.after.state==='DETENIDO'&&result.after.stopRequested===1&&result.after.stopEvent?'PERSISTED_STOP_WITHOUT_GENERAL_RESUME':'STOP_PRECONDITION_NOT_CONFIRMED',scope:'RA_CONTROL_METADATA_ONLY'};
 return result;
}
function failureCode(error){const m=error?.message||'';if(['RESULT_INVALID','SCOPE_UNAVAILABLE','SCOPE_CHANGED','TOO_MANY_UNRESOLVED','INVENTORY_CHANGED','BUDGET_EXHAUSTED','AUTHORIZATION_REQUIRED','DSN_INVALID'].some(c=>m==='DOC120_STOP_DIAGNOSTIC_'+c))return m;const db=['AUTHENTICATION_FAILED','DSN_UNAVAILABLE','DRIVER_UNAVAILABLE','DATABASE_ACCESS_DENIED','DATABASE_UNAVAILABLE','CONNECTION_UNAVAILABLE','OPEN_FAILED','QUERY_FAILED','COLUMN_UNAVAILABLE','TABLE_UNAVAILABLE','QUERY_UNSUPPORTED','RESULT_FAILED','INPUT_FAILED','FAILED','REQUIRED'].find(c=>m==='DOC111_SELECT_'+c);return db?'DOC120_STOP_DIAGNOSTIC_DB_'+db:'DOC120_STOP_DIAGNOSTIC_STOPPED';}
async function main(args=process.argv.slice(2)){
 check(args.length===3&&args[0]==='--run'&&/^[a-f0-9]{32}$/.test(args[1])&&args[2]==='--authorized-readonly','AUTHORIZATION_REQUIRED');requireInteractiveConsole();
 const directory=path.join(ROOT,'.tmp/doc120-stopped-lot',args[1]);check(!fs.existsSync(directory),'AUTHORIZATION_REQUIRED');fs.mkdirSync(directory,{recursive:true});
 const state={runId:args[1],lotId:'50',status:'WAITING_INPUT',select:0,completedSelect:0,maxSelect:5,http:0,sessions:0,businessEffects:0,closed:false},values={};let environment;
 const save=()=>fs.writeFileSync(path.join(directory,'state.json'),JSON.stringify(state,null,2));save();
 try{
  process.title='DOC-120 - diagnostico SOLO LECTURA del lote50';console.log('Diagnostico autorizado: lote50, propietario141/empresa2,1.000 archivos. Maximo5 SELECT RA; cero HTTP y cero cambios. No reanuda ni importa.');console.log('Las claves se ingresan enmascaradas; no se guardan ni se imprimen.');
  await collectValue(values,'dsn','DSN RA de lectura',{defaultValue:'MembershipUsers'});check(/^[A-Za-z0-9 _.-]{1,128}$/.test(values.dsn),'DSN_INVALID');
  await collectValue(values,'user','Cuenta MySQL exclusivamente SELECT',{secret:true});await collectValue(values,'password','Clave MySQL SELECT',{secret:true});
  environment=Object.fromEntries(Object.entries(process.env).filter(([k])=>!/^DOC\d+(?:_DA)?_E2E/.test(k)&&!['DEBUG','PWDEBUG','NODE_DEBUG','NODE_DEBUG_NATIVE'].includes(k)));Object.assign(environment,{DOC120_E2E_ODBC_DSN:values.dsn,DOC120_E2E_MYSQL_USER:values.user,DOC120_E2E_MYSQL_PASSWORD:values.password});
  state.status='READING';save();
  state.result=await inspect(async(sql,parameters)=>{check(state.select<5,'BUDGET_EXHAUSTED');state.select++;save();const rows=await queryRows(sql,parameters,environment,'DOC120_E2E');state.completedSelect++;save();return rows;},name=>{state.stage=name;save();console.log('DOC120_STOP_DIAGNOSTIC_PROGRESS='+name);});
  state.status=state.result.summary.stableRevisionAndEvent?'FINISHED':'INCONCLUSIVE_CHANGED_DURING_READ';
 }catch(error){state.status='STOPPED';state.failure=failureCode(error);process.exitCode=2;}
 finally{for(const k of Object.keys(values))delete values[k];if(environment)for(const k of Object.keys(environment))if(/^DOC120_E2E_/.test(k))delete environment[k];Object.assign(state,{closed:true,credentialsCleared:true,finishedUtc:new Date().toISOString()});save();console.log('DOC120_STOP_DIAGNOSTIC_RESULT='+JSON.stringify(state));console.log('La lectura termino. No se repetira automaticamente.');}
}
if(require.main===module)main().catch(()=>{console.error('DOC120_STOP_DIAGNOSTIC_PRECHECK_STOPPED');process.exitCode=2;});
module.exports={queries,sanitize,inspect,failureCode,main};
