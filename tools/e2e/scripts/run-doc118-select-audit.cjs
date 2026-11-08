'use strict';
// Closed SELECT-only audit of the existing lot. No browser, login or mutations.
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const tty=require('./support/interactive-e2e-console.cjs'),shared=require('../tests/support/doc111-importacion-carpetas.cjs'),{caseProfile}=require('../tests/support/doc118-case-preparation.cjs'),{createBudget}=require('../tests/support/doc118-session-bridge.cjs'),{queryRows}=require('./support/doc32-e2e-odbc.cjs');
const ROOT=path.resolve(__dirname,'../../..'),CHANGE='doc-118-produccion-importacion-carga-carpeta-lote-recuperacion';
const hash=v=>crypto.createHash('sha256').update(JSON.stringify(v)).digest('hex'),check=v=>{if(!v)throw Error('DOC118_AUDIT_UNCONFIRMED');};
const read=p=>JSON.parse(fs.readFileSync(p,'utf8').replace(/^\uFEFF/,'')),id=v=>typeof v==='string'&&/^[1-9][0-9]{0,9}$/.test(v),states=new Set(['EN_CURSO','DETENIDO','COMPLETADO','COMPLETADA','PENDIENTE_VERIFICACION','EXPEDIENTE_CONFIRMADO','RECIBIDO','NO_INICIADO','INCIERTO','RESERVADO','CONFIRMADO','RECHAZADO']);
const state=v=>states.has(v)?v:'UNCLASSIFIED';
async function audit({controls,profile}){
 const view=await controls.snapshot('24'),ctx=JSON.parse(view.lot.contexto_doc),cfg=JSON.parse(view.lot.config_doc);
 check(String(view.lot.lote_id)==='24'&&ctx.UsuarioRef==='141'&&ctx.AmbitoRef==='2'&&ctx.NivelRef==='7'&&cfg.VersionId==='38'&&String(cfg.Configuracion.GabineteId)==='10'&&view.roots.length===2&&view.files.length===2);
 const result={loteId:'24',estado:state(view.lot.estado),detencionSolicitada:Number(view.lot.detencion_solicitada)===1,activeClaims:view.roots.some(r=>r.intento_activo_id)||view.files.some(f=>f.intento_activo_id),files:[],fingerprint:hash(view)};
 for(let i=0;i<2;i++){
  const roots=view.roots.filter(r=>String(r.raiz_id)===String(39+i)),files=view.files.filter(f=>String(f.archivo_id)===String(40+i));check(roots.length===1&&files.length===1);const root=roots[0],file=files[0],identity=JSON.parse(root.identidad_expediente_doc);
  check(String(file.raiz_id)===String(root.raiz_id)&&identity.ExpedienteId===String(670+i)&&identity.RelacionNivelId===String(1095+i));
  const attempts=view.attempts.filter(a=>String(a.archivo_id)===String(file.archivo_id)&&a.fase==='ALMACENAR');
  const item={archivoId:String(file.archivo_id),raizId:String(root.raiz_id),expedienteId:identity.ExpedienteId,rootState:state(root.estado),receiptState:state(file.estado_recepcion),effectState:state(file.estado_efecto),activeClaim:!!file.intento_activo_id,storageAttempts:attempts.map(a=>({intentoId:String(a.intento_id),estado:state(a.estado),finished:!!a.fin_utc})),documentoId:null,inventarioId:null,physical:'UNCONFIRMED'};
  if(file.resultado_doc){const stored=JSON.parse(file.resultado_doc);if(id(stored.DocumentoId))item.documentoId=stored.DocumentoId;if(id(stored.InventarioId))item.inventarioId=stored.InventarioId;
   if(stored.Estado==='CONFIRMADO'&&attempts.length===1&&attempts[0].estado==='CONFIRMADO'&&attempts[0].fin_utc&&hash(JSON.parse(attempts[0].evidencia_doc))===hash(stored)){
    try{await controls.verifyKnownDocument(view,String(root.raiz_id),stored,profile.expectations[i],profile.rootNames[i]+'.pdf');item.physical='KNOWN_IDS_VERIFIED';}catch{item.physical='VERIFICATION_FAILED';}
   }
  }else if(file.estado_efecto==='NO_INICIADO'&&!file.intento_activo_id&&attempts.length===0){try{await controls.assertNoDocuments({...view,roots:[root]});item.physical='ABSENCE_VERIFIED';}catch{item.physical='VERIFICATION_FAILED';}}
  result.files.push(item);
 }
 result.snapshotUnchanged=hash(await controls.snapshot('24'))===result.fingerprint;
 result.confirmedDocuments=result.files.filter(f=>f.physical==='KNOWN_IDS_VERIFIED').length;result.integrationPass=false;return result;
}
async function main(){
 tty.requireInteractiveConsole();const authFile=path.join(ROOT,'openspec/changes',CHANGE,'execution-authorization.json'),a=read(authFile),prior=read(path.join(ROOT,'Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-active-window-stop-20261108.json'));
 check(a.executionStatus==='STOPPED_RUNTIME_REVIEW_REQUIRED'&&a.maximum.totalSessions===9&&a.consumed.selectStatements===81&&prior.sessionsClosed&&prior.ephemeralSecretsCleared&&prior.observerCleanupConfirmed);
 const output=path.join(ROOT,'.tmp/doc118-select-audit-20261108.json');check(!fs.existsSync(output));
 const report={issueKey:'DOC-118',status:'AWAITING_SELECT_CREDENTIALS',recordedUtc:new Date().toISOString(),loginAttempts:0,httpRequests:0,mutations:0,ephemeralSecretsCleared:false,counters:prior.counters};
 const save=()=>fs.writeFileSync(output,JSON.stringify(report,null,2)+'\n');save();
 const secrets={},environment=Object.fromEntries(Object.entries(process.env).filter(([k])=>['PATH','Path','SystemRoot','WINDIR','TEMP','TMP'].includes(k))),budget=createBudget({allHttpPerCase:a.maximum.allHttpPerCase,allHttpGlobal:a.maximum.allHttpGlobal,carried:prior.counters});
 try{
  console.log('DOC-118: auditoria SELECT del lote24. Sin login de aplicacion ni escrituras.');
  await tty.collectValue(secrets,'user','Cuenta MySQL exclusivamente SELECT',{secret:true});await tty.collectValue(secrets,'password','Contrasena MySQL SELECT',{secret:true});
  for(const prefix of ['DOC111_E2E','DOC111_DA_E2E'])Object.assign(environment,{[prefix+'_ODBC_DSN']:'MembershipUsers',[prefix+'_MYSQL_USER']:secrets.user,[prefix+'_MYSQL_PASSWORD']:secrets.password});for(const k of Object.keys(secrets))delete secrets[k];
  const query=async(origin,sql,args)=>{budget.select(origin,sql);report.counters=budget.snapshot();save();return queryRows(sql,args,environment,origin==='RA'?'DOC111_E2E':'DOC111_DA_E2E');};
  const profile=caseProfile(read(path.join(ROOT,'tools/e2e/profiles/doc111-importacion-carpetas-lote-20261102.runtime.json')),'COORD02',a.newRootNames.COORD02);
  report.status='AUDITING_SELECT_ONLY';save();report.result=await audit({controls:shared.createSelectControls(query),profile});report.status='AUDIT_COMPLETE_REVIEW_REQUIRED';
 }catch{report.status='AUDIT_UNCONFIRMED';process.exitCode=2;}finally{
  for(const k of Object.keys(secrets))delete secrets[k];for(const k of Object.keys(environment))delete environment[k];report.ephemeralSecretsCleared=true;report.counters=budget.snapshot();report.finishedUtc=new Date().toISOString();save();console.log(report.status+'; secretos eliminados.');
 }
}
if(require.main===module)main().catch(()=>{console.log('DOC118_AUDIT_START_UNCONFIRMED');process.exitCode=2;});module.exports={audit};
