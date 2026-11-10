'use strict';
// Closed SELECT-only audit of the existing lot. No browser, login or mutations.
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const tty=require('./support/interactive-e2e-console.cjs'),shared=require('../tests/support/doc111-importacion-carpetas.cjs'),{caseProfile}=require('../tests/support/doc118-case-preparation.cjs'),{createBudget}=require('../tests/support/doc118-session-bridge.cjs'),{queryRows}=require('./support/doc32-e2e-odbc.cjs');
const ROOT=path.resolve(__dirname,'../../..'),CHANGE='doc-118-produccion-importacion-carga-carpeta-lote-recuperacion';
const hash=v=>crypto.createHash('sha256').update(JSON.stringify(v)).digest('hex'),check=v=>{if(!v)throw Error('DOC118_AUDIT_UNCONFIRMED');};
const read=p=>JSON.parse(fs.readFileSync(p,'utf8').replace(/^\uFEFF/,'')),id=v=>typeof v==='string'&&/^[1-9][0-9]{0,9}$/.test(v),states=new Set(['EN_CURSO','DETENIDO','COMPLETADO','COMPLETADA','PENDIENTE_VERIFICACION','EXPEDIENTE_CONFIRMADO','RECIBIDO','NO_INICIADO','INCIERTO','RESERVADO','CONFIRMADO','RECHAZADO']);
const state=v=>states.has(v)?v:'UNCLASSIFIED';
const HISTORICAL_BINDING=Object.freeze({loteId:'24',roots:['39','40'],files:['40','41'],expedientes:['670','671'],relaciones:['1095','1096'],fileNames:['DOC118-C02-A-20261107.pdf','DOC118-C02-B-20261107.pdf']});
function validateBinding(binding){
 check(binding&&Object.keys(binding).sort().join()==='expedientes,fileNames,files,loteId,relaciones,roots'&&id(binding.loteId));
 for(const k of ['roots','files','expedientes','relaciones'])check(Array.isArray(binding[k])&&binding[k].length===2&&binding[k].every(id)&&binding[k][0]!==binding[k][1]);
 check(JSON.stringify(binding.fileNames)===JSON.stringify(HISTORICAL_BINDING.fileNames));
 check(!['18','21'].includes(binding.loteId));return binding;
}
async function inspectCurrentPhysical({query,controls,view,root,profile,index,binding}){
 const rows=await query('DA','SELECT NOMBRE FROM system1 WHERE id_gabinete=?',['10']);check(rows.length===1&&typeof rows[0].NOMBRE==='string'&&/^[A-Za-z_][A-Za-z0-9_]{0,63}$/.test(rows[0].NOMBRE));const cabinet=rows[0].NOMBRE,expedienteId=binding.expedientes[index],counts={};
 for(const [key,origin,sql]of [
  ['cabinet','DA','SELECT COUNT(*) AS total FROM `'+cabinet+'` WHERE ID_EXPEDIENTE=?'],
  ['inventory','RA','SELECT COUNT(*) AS total FROM registro_producion_documental WHERE EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE=?'],
  ['index','RA','SELECT COUNT(*) AS total FROM ra_cert_indice_expediente WHERE expediente_archivo_ID_EXPEDIENTE=?']
 ]){const selected=await query(origin,sql,[expedienteId]);check(selected.length===1&&/^[0-9]+$/.test(String(selected[0].total))&&Number.isSafeInteger(Number(selected[0].total)));counts[key]=Number(selected[0].total);}
 const result={status:'PARTIAL_OR_AMBIGUOUS',counts,documentoId:null,inventarioId:null,replayAllowed:false};
 if(Object.values(counts).every(n=>n===0)){result.status='CURRENT_ABSENCE_VERIFIED';return result;}
 if(!Object.values(counts).every(n=>n===1))return result;
 const candidates=await query('RA','SELECT ID_REGISTRO_PRODUCION_DOCUMENTAL AS inventario_id,ID_DOCUMENTO_DOCUARCHI_ALMACEN AS documento_id FROM registro_producion_documental WHERE EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE=? AND ID_USUARIO_GESTION=? AND ID_EMPRESA_DOCUMENTO=? AND NOMBRE_GABINETE=? AND SEGUNDO_NOMBRE_DOCUMENTO=? AND FORMATO=? LIMIT 2',[expedienteId,'141','2',cabinet,binding.fileNames[index],'.PDF']);
 if(candidates.length!==1||!id(String(candidates[0].documento_id))||!id(String(candidates[0].inventario_id)))return result;
 const known={DocumentoId:String(candidates[0].documento_id),InventarioId:String(candidates[0].inventario_id)};
 await controls.verifyKnownDocument(view,String(root.raiz_id),known,profile.expectations[index],binding.fileNames[index]);
 return{...result,status:'CURRENT_KNOWN_IDS_VERIFIED_NO_CHECKPOINT',documentoId:known.DocumentoId,inventarioId:known.InventarioId};
}
async function audit({controls,profile,query,inspectActiveEffect=false,binding=HISTORICAL_BINDING}){
 binding=validateBinding(binding);
 const view=await controls.snapshot(binding.loteId),ctx=JSON.parse(view.lot.contexto_doc),cfg=JSON.parse(view.lot.config_doc);
 check(String(view.lot.lote_id)===binding.loteId&&ctx.UsuarioRef==='141'&&ctx.AmbitoRef==='2'&&ctx.NivelRef==='7'&&cfg.VersionId==='38'&&String(cfg.Configuracion.GabineteId)==='10'&&view.roots.length===2&&view.files.length===2);
 const result={loteId:binding.loteId,estado:state(view.lot.estado),detencionSolicitada:Number(view.lot.detencion_solicitada)===1,activeClaims:view.roots.some(r=>r.intento_activo_id)||view.files.some(f=>f.intento_activo_id),files:[],fingerprint:hash(view)};
 for(let i=0;i<2;i++){
  const roots=view.roots.filter(r=>String(r.raiz_id)===binding.roots[i]),files=view.files.filter(f=>String(f.archivo_id)===binding.files[i]);check(roots.length===1&&files.length===1);const root=roots[0],file=files[0],identity=JSON.parse(root.identidad_expediente_doc);
  check(String(file.raiz_id)===String(root.raiz_id)&&identity.ExpedienteId===binding.expedientes[i]&&identity.RelacionNivelId===binding.relaciones[i]);
  const attempts=view.attempts.filter(a=>String(a.archivo_id)===String(file.archivo_id)&&a.fase==='ALMACENAR');
  const item={archivoId:String(file.archivo_id),raizId:String(root.raiz_id),expedienteId:identity.ExpedienteId,rootState:state(root.estado),receiptState:state(file.estado_recepcion),effectState:state(file.estado_efecto),activeClaim:!!file.intento_activo_id,storageAttempts:attempts.map(a=>({intentoId:String(a.intento_id),estado:state(a.estado),finished:!!a.fin_utc})),documentoId:null,inventarioId:null,physical:'UNCONFIRMED'};
  if(file.resultado_doc){const stored=JSON.parse(file.resultado_doc);if(id(stored.DocumentoId))item.documentoId=stored.DocumentoId;if(id(stored.InventarioId))item.inventarioId=stored.InventarioId;
   if(stored.Estado==='CONFIRMADO'&&attempts.length===1&&attempts[0].estado==='CONFIRMADO'&&attempts[0].fin_utc&&hash(JSON.parse(attempts[0].evidencia_doc))===hash(stored)){
    try{await controls.verifyKnownDocument(view,String(root.raiz_id),stored,profile.expectations[i],binding.fileNames[i]);item.physical='KNOWN_IDS_VERIFIED';}catch{item.physical='VERIFICATION_FAILED';}
   }
  }else if(file.estado_efecto==='NO_INICIADO'&&!file.intento_activo_id&&attempts.length===0){try{await controls.assertNoDocuments({...view,roots:[root]});item.physical='ABSENCE_VERIFIED';}catch{item.physical='VERIFICATION_FAILED';}}
  else if(inspectActiveEffect&&i===0){
   check(typeof query==='function'&&attempts.length===1&&id(String(attempts[0].intento_id))&&(binding.loteId==='24'?String(attempts[0].intento_id)==='69':!['41','53','69'].includes(String(attempts[0].intento_id))));
   try{item.physicalEvidence=await inspectCurrentPhysical({query,controls,view,root,profile,index:i,binding});item.physical=item.physicalEvidence.status;}catch{item.physical='VERIFICATION_FAILED';}
  }
  result.files.push(item);
 }
 result.snapshotUnchanged=hash(await controls.snapshot(binding.loteId))===result.fingerprint;
 result.confirmedDocuments=result.files.filter(f=>f.physical==='KNOWN_IDS_VERIFIED').length;result.integrationPass=false;return result;
}
function validateAuditResumption(m,{runId,attemptId,prior,cancelled,cancelledSha256}){
 const s=m?.readAudit,rel='.tmp/doc118-live-'+runId+'/select-audit.json';
 check(m?.issueKey==='DOC-118'&&m.status==='ACTIVE'&&s?.authorized===true&&s.readOnly===true&&s.runId===runId&&s.attemptId===attemptId&&/^[a-f0-9]{32}$/.test(attemptId)&&s.loteId==='25');
 check(s.maximumSelectStatementsTotal===240&&s.carriedSelectStatements===34&&s.maximumAdditionalSelectStatements===206&&s.historicalCountersNotReset===true&&s.carriedHttpRequests===301&&s.carriedSessions===2&&s.newApplicationSessions===0&&s.newApplicationHttp===0&&s.newBusinessMutations===0);
 check(s.originalCancelledReport===rel&&/^[a-f0-9]{64}$/.test(cancelledSha256)&&s.originalCancelledReportSha256===cancelledSha256);
 check(prior.status==='STOPPED_REVIEW_REQUIRED'&&prior.sessionAttempts===2&&prior.counters.selects===34&&prior.counters.scenarios.COORD02.http===301&&prior.counters.scenarios.COORD03.http===0);
 check(cancelled.issueKey==='DOC-118'&&cancelled.runId===runId&&cancelled.status==='CANCELLED_ADMINISTRATIVE_CLOSURE'&&cancelled.ephemeralSecretsCleared===true&&cancelled.mutations===0&&JSON.stringify(cancelled.counters)===JSON.stringify(prior.counters));
 return 'select-audit-'+attemptId+'.json';
}
async function auditCurrentWindow(args,{resume=false}={}){
 tty.requireInteractiveConsole();check(args.length===(resume?2:1)&&/^[a-f0-9]{32}$/.test(args[0]));
 const runId=args[0],a=read(path.join(ROOT,'openspec/changes',CHANGE,'execution-current-window.json')),directory=path.join(ROOT,'.tmp','doc118-live-'+runId),prior=read(path.join(directory,'status.json'));
 check(a.runId===runId&&a.executionStatus==='STOPPED_REVIEW_REQUIRED'&&a.selectAuditAuthorized===true&&a.maximum.selectStatementsTotal===240&&a.maximum.totalSessions===2&&prior.status==='STOPPED_REVIEW_REQUIRED'&&prior.sessionsClosed===true&&prior.ephemeralSecretsCleared===true&&prior.observerCloseConfirmed===true&&prior.sessionAttempts===2&&prior.counters.selects===a.newWindowConsumption.selectStatements);
 const binding=validateBinding(a.retainedCase);check(binding.loteId==='25'&&JSON.stringify(binding.roots)===JSON.stringify(['41','42'])&&JSON.stringify(binding.files)===JSON.stringify(['42','43'])&&JSON.stringify(binding.expedientes)===JSON.stringify(['672','673'])&&JSON.stringify(binding.relaciones)===JSON.stringify(['1097','1098']));
 check(prior.references.some(r=>r.loteId===binding.loteId)&&binding.roots.every((r,i)=>prior.references.some(x=>x.raizId===r&&x.expedienteId===binding.expedientes[i]&&x.relacionNivelId===binding.relaciones[i])));
 let outputName='select-audit.json';
 if(resume){const cancelledPath=path.join(directory,'select-audit.json'),cancelled=read(cancelledPath),resumption=read(path.join(ROOT,'openspec/changes',CHANGE,'implementation-resumption.json'));outputName=validateAuditResumption(resumption,{runId,attemptId:args[1],prior,cancelled,cancelledSha256:crypto.createHash('sha256').update(fs.readFileSync(cancelledPath)).digest('hex')});}
 const output=path.join(directory,outputName);check(!fs.existsSync(output));
 const report={issueKey:'DOC-118',runId,mode:'CURRENT_WINDOW_SELECT_ONLY',status:'AWAITING_SELECT_CREDENTIALS',recordedUtc:new Date().toISOString(),loginAttempts:0,httpRequests:0,mutations:0,ephemeralSecretsCleared:false,counters:prior.counters};
 if(resume){report.resumeAttemptId=args[1];report.historicalCountersNotReset=true;report.priorCancelledReportPreserved=true;process.title='DOC-118 SELECT lote 25';}
 const save=()=>fs.writeFileSync(output,JSON.stringify(report,null,2)+'\n');fs.writeFileSync(output,JSON.stringify(report,null,2)+'\n',{flag:'wx'});
 const secrets={},environment=Object.fromEntries(Object.entries(process.env).filter(([k])=>['PATH','Path','SystemRoot','WINDIR','TEMP','TMP'].includes(k))),budget=createBudget({allHttpPerCase:a.maximum.allHttpPerCase,allHttpGlobal:a.maximum.allHttpGlobal,carried:prior.counters});
 try{
  console.log('DOC-118: auditoria SELECT del lote conservado25. Sin login de aplicacion ni escrituras.');
  await tty.collectValue(secrets,'user','Cuenta MySQL exclusivamente SELECT');await tty.collectValue(secrets,'password','Contrasena MySQL SELECT',{secret:true});
  for(const prefix of ['DOC111_E2E','DOC111_DA_E2E'])Object.assign(environment,{[prefix+'_ODBC_DSN']:'MembershipUsers',[prefix+'_MYSQL_USER']:secrets.user,[prefix+'_MYSQL_PASSWORD']:secrets.password});for(const k of Object.keys(secrets))delete secrets[k];
  const query=async(origin,sql,parameters)=>{check(budget.snapshot().selects<a.maximum.selectStatementsTotal);budget.select(origin,sql);report.counters=budget.snapshot();save();return queryRows(sql,parameters,environment,origin==='RA'?'DOC111_E2E':'DOC111_DA_E2E');};
  const profile=caseProfile(read(path.join(ROOT,'tools/e2e/profiles/doc111-importacion-carpetas-lote-20261102.runtime.json')),'COORD02',a.newRootNames.COORD02,{runId});
  report.status='AUDITING_SELECT_ONLY';save();report.result=await audit({controls:shared.createSelectControls(query),profile,query,inspectActiveEffect:true,binding});report.status='AUDIT_COMPLETE_REVIEW_REQUIRED';
 }catch{report.status='AUDIT_UNCONFIRMED';process.exitCode=2;}finally{
  for(const k of Object.keys(secrets))delete secrets[k];for(const k of Object.keys(environment))delete environment[k];report.ephemeralSecretsCleared=true;report.counters=budget.snapshot();report.finishedUtc=new Date().toISOString();save();console.log(report.status+'; secretos eliminados.');
 }
}
async function main(){
 if(process.argv[2]==='--resume-current-audit')return auditCurrentWindow(process.argv.slice(3),{resume:true});
 if(process.argv[2]==='--current-window')return auditCurrentWindow(process.argv.slice(3));
 tty.requireInteractiveConsole();const args=process.argv.slice(2);check(args.length===0||args.length===1&&args[0]==='--inspect-active-effect');const inspectActiveEffect=args.length===1;
 const authFile=path.join(ROOT,'openspec/changes',CHANGE,'execution-authorization.json'),a=read(authFile),ev=path.join(ROOT,'Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion'),stop=read(path.join(ev,'doc118-active-window-stop-20261108.json')),prior=inspectActiveEffect?read(path.join(ev,'doc118-select-audit-20261108.json')):stop;
 check(a.executionStatus==='STOPPED_RUNTIME_REVIEW_REQUIRED'&&a.maximum.totalSessions===9&&a.consumed.selectStatements===(inspectActiveEffect?95:81)&&stop.sessionsClosed&&stop.ephemeralSecretsCleared&&stop.observerCleanupConfirmed&&prior.ephemeralSecretsCleared&&prior.counters.selects===a.consumed.selectStatements);
 const output=path.join(ROOT,inspectActiveEffect?'.tmp/doc118-active-effect-audit-20261108.json':'.tmp/doc118-select-audit-20261108.json');check(!fs.existsSync(output));
 const report={issueKey:'DOC-118',mode:inspectActiveEffect?'INSPECT_ACTIVE_EFFECT':'CHECKPOINT_AUDIT',status:'AWAITING_SELECT_CREDENTIALS',recordedUtc:new Date().toISOString(),loginAttempts:0,httpRequests:0,mutations:0,ephemeralSecretsCleared:false,counters:prior.counters};
 const save=()=>fs.writeFileSync(output,JSON.stringify(report,null,2)+'\n');save();
 const secrets={},environment=Object.fromEntries(Object.entries(process.env).filter(([k])=>['PATH','Path','SystemRoot','WINDIR','TEMP','TMP'].includes(k))),budget=createBudget({allHttpPerCase:a.maximum.allHttpPerCase,allHttpGlobal:a.maximum.allHttpGlobal,carried:prior.counters});
 try{
  console.log('DOC-118: auditoria SELECT del lote24. Sin login de aplicacion ni escrituras.');
  await tty.collectValue(secrets,'user','Cuenta MySQL exclusivamente SELECT');await tty.collectValue(secrets,'password','Contrasena MySQL SELECT',{secret:true});
  for(const prefix of ['DOC111_E2E','DOC111_DA_E2E'])Object.assign(environment,{[prefix+'_ODBC_DSN']:'MembershipUsers',[prefix+'_MYSQL_USER']:secrets.user,[prefix+'_MYSQL_PASSWORD']:secrets.password});for(const k of Object.keys(secrets))delete secrets[k];
  const query=async(origin,sql,args)=>{budget.select(origin,sql);report.counters=budget.snapshot();save();return queryRows(sql,args,environment,origin==='RA'?'DOC111_E2E':'DOC111_DA_E2E');};
  const profile=caseProfile(read(path.join(ROOT,'tools/e2e/profiles/doc111-importacion-carpetas-lote-20261102.runtime.json')),'COORD02',a.newRootNames.COORD02);
  report.status='AUDITING_SELECT_ONLY';save();report.result=await audit({controls:shared.createSelectControls(query),profile,query,inspectActiveEffect});report.status='AUDIT_COMPLETE_REVIEW_REQUIRED';
 }catch{report.status='AUDIT_UNCONFIRMED';process.exitCode=2;}finally{
  for(const k of Object.keys(secrets))delete secrets[k];for(const k of Object.keys(environment))delete environment[k];report.ephemeralSecretsCleared=true;report.counters=budget.snapshot();report.finishedUtc=new Date().toISOString();save();console.log(report.status+'; secretos eliminados.');
 }
}
if(require.main===module)main().catch(()=>{console.log('DOC118_AUDIT_START_UNCONFIRMED');process.exitCode=2;});module.exports={audit,validateAuditResumption};
