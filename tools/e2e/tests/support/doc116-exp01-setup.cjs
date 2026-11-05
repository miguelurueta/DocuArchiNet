'use strict';
const crypto=require('node:crypto');
const {validateProfile,FILE_NAME}=require('./doc111-importacion-carpetas.cjs');
const {validateSession,assertLevelGrant}=require('./doc116-expedientes-importacion.cjs');
const hash=v=>crypto.createHash('sha256').update(typeof v==='string'||Buffer.isBuffer(v)?v:JSON.stringify(v)).digest('hex');
const check=(v,c)=>{if(!v)throw Error('DOC116_SETUP_'+c);};
const id=v=>typeof v==='string'&&/^[1-9][0-9]{0,9}$/.test(v)&&BigInt(v)<=4294967295n;
function publicFailure(error){
  const result={code:/^DOC116_SETUP_[A-Z_]+$/.test(error?.message||'')?error.message:'DOC116_OPERATION_NOT_CONFIRMED'};
  if(['ConsultarLote','AbrirImportacion','PrepararPlan','ConfirmarPlan'].includes(error?.operation))result.operation=error.operation;
  if(Number.isInteger(error?.httpStatus)&&error.httpStatus>=100&&error.httpStatus<=599)result.httpStatus=error.httpStatus;
  if(['CAPACIDAD_NO_DISPONIBLE','ACCESO_DENEGADO','SOLICITUD_INVALIDA','SESION_REQUERIDA','CSRF_INVALIDO','ORIGEN_INVALIDO','ERROR_INTERNO','CONFIGURACION_INVALIDA','OPERACION_EN_CURSO','CONFLICTO_REVISION','ARCHIVO_RECHAZADO','RESULTADO_INCIERTO','NO_CLASIFICADO'].includes(error?.serverCode))result.serverCode=error.serverCode;
  return result;
}
function validateSetup(p,bytes){
  validateProfile(p.profile);
  check(p.issueKey==='DOC-116'&&['EXP-01','EXP-02','EXP-03'].includes(p.scenario)&&p.profile.mode==='LOTE'&&p.profile.rootNames.length===2&&new Set(p.profile.rootNames).size===2,'SCOPE_INVALID');
  check(id(p.identity?.userRef)&&id(p.identity?.ambitoRef)&&id(p.levelB)&&p.levelB!==p.profile.nivelRef&&id(p.authProofLotId),'IDENTITY_INVALID');
  check(JSON.stringify(p.budget)===JSON.stringify({preparations:1,maximumNewLots:1,confirmations:1,uploads:2,maximumCreationRequests:p.scenario==='EXP-01'?4:p.scenario==='EXP-02'?2:1,maximumCreatedExpedients:p.scenario==='EXP-01'?2:p.scenario==='EXP-02'?1:0,definitiveDocuments:0,permissionChanges:p.scenario==='EXP-03'?2:0}),'BUDGET_INVALID');
  check(p.preserveResources===true&&p.preserveAllExistingPlans===true&&p.levelPermissionMutation===(p.scenario==='EXP-03')&&Array.isArray(p.preservedLotIds)&&p.preservedLotIds.length>0&&p.preservedLotIds.every(id),'PRESERVATION_REQUIRED');
  if(p.scenario==='EXP-03')check(id(p.authProofLevelRef)&&id(p.levelPermission?.permissionId)&&/^[a-f0-9]{64}$/.test(p.levelPermission?.environmentHash||'')&&Object.keys(p.levelPermission).length===2,'LEVEL_RESOURCES_INVALID');
  else check(p.levelPermission===undefined&&p.authProofLevelRef===undefined,'LEVEL_RESOURCES_INVALID');
  check(Buffer.isBuffer(bytes)&&bytes.length===p.pdfBytes&&hash(bytes)===p.pdfSha256,'PDF_CHANGED');return p;
}
async function preservation(query,p,scope){
  const read=async(sql,args)=>{check(/^SELECT\b/.test(sql)&&!sql.includes(';'),'SELECT_REQUIRED');const rows=await query('RA',sql,args);check(Array.isArray(rows)&&rows.length<2001,'CONTROL_LIMIT');return rows;};
  const who=[p.identity.userRef,p.identity.ambitoRef];
  if(!scope){const bounds=await read('SELECT COALESCE(MAX(plan_id),0) AS maxPlan FROM ra_integracion_plan WHERE usuario_ref=? AND ambito_ref=?',who);const lots=await read('SELECT lote_id FROM ra_integracion_lote WHERE solicitante_ref=? AND ambito_ref=? ORDER BY lote_id LIMIT 2001',who);check(bounds.length===1&&p.preservedLotIds.every(id=>lots.some(r=>String(r.lote_id)===id)),'PREVIOUS_RESOURCES_CHANGED');scope={maxPlan:String(bounds[0].maxPlan),maxLot:String(lots.reduce((n,r)=>BigInt(r.lote_id)>n?BigInt(r.lote_id):n,0n))};}
  const fingerprints={};
  const plans=await read('SELECT p.* FROM ra_integracion_plan p WHERE p.usuario_ref=? AND p.ambito_ref=? AND p.plan_id<=? ORDER BY p.plan_id LIMIT 2001',[...who,scope.maxPlan]);fingerprints.plans={count:plans.length,hash:hash(plans)};
  const tables=[['lots','ra_integracion_lote','lote_id'],['roots','ra_integracion_raiz','raiz_id'],['files','ra_integracion_archivo','archivo_id'],['attempts','ra_integracion_intento','intento_id'],['events','ra_integracion_evento','evento_id']];
  for(const [key,table,pk]of tables){const join=key==='lots'?'':key==='files'?' INNER JOIN ra_integracion_raiz r ON r.raiz_id=x.raiz_id INNER JOIN ra_integracion_lote l ON l.lote_id=r.lote_id':' INNER JOIN ra_integracion_lote l ON l.lote_id=x.lote_id';const alias=key==='lots'?'x':'l';const rows=await read(`SELECT x.* FROM ${table} x${join} WHERE ${alias}.solicitante_ref=? AND ${alias}.ambito_ref=? AND ${alias}.lote_id<=? ORDER BY x.${pk} LIMIT 2001`,[...who,scope.maxLot]);fingerprints[key]={count:rows.length,hash:hash(rows)};}
  return {scope,fingerprints};
}
async function prepareFreshLot(driver,query,p,bytes,checkpoint,options={}){
  validateSetup(p,bytes);
  check(Object.keys(options).every(k=>k==='preflightOnly')&&(options.preflightOnly===undefined||typeof options.preflightOnly==='boolean'),'OPTIONS_INVALID');
  checkpoint({stage:'PRIOR_PLAN_CHECK'});
  const prior=await query('RA','SELECT plan_id FROM ra_integracion_plan WHERE usuario_ref=? AND ambito_ref=? AND nivel_ref=? AND (LOCATE(?,manifiesto_doc)>0 OR LOCATE(?,manifiesto_doc)>0) ORDER BY plan_id LIMIT 3',[p.identity.userRef,p.identity.ambitoRef,p.profile.nivelRef,...p.profile.rootNames]);
  check(Array.isArray(prior),'CONTROL_INVALID');
  if(prior.length){checkpoint({stage:'EXISTING_PLAN_REVIEW',planIds:prior.map(r=>String(r.plan_id))});throw Error('DOC116_SETUP_EXISTING_PLAN_REVIEW');}
  checkpoint({stage:'ACCOUNT_PROOF_SELECT'});
  const proofLevel=p.authProofLevelRef||p.profile.nivelRef;
  const owner=await query('RA','SELECT lote_id,contexto_doc FROM ra_integracion_lote WHERE lote_id=? AND solicitante_ref=? AND ambito_ref=? AND nivel_ref=? LIMIT 2',[p.authProofLotId,p.identity.userRef,p.identity.ambitoRef,proofLevel]);check(owner.length===1,'ACCOUNT_PROOF_FAILED');
  checkpoint({stage:'TOKEN_READ'});await driver.refreshToken();
  checkpoint({stage:'OPEN_READONLY'});
  const opened=await driver.request('AbrirImportacion',{nivelRef:proofLevel});check(opened?.nivelRef===proofLevel,'DESTINATION_CHANGED');
  let stored;try{stored=JSON.parse(owner[0].contexto_doc);}catch{throw Error('DOC116_SETUP_PROOF_CONTEXT_INVALID');}
  check(stored?.UsuarioRef===p.identity.userRef&&stored.AmbitoRef===p.identity.ambitoRef&&stored.NivelRef===proofLevel&&stored.PuedeImportar===true&&typeof opened.selloDestino==='string'&&opened.selloDestino.length===64&&typeof opened.rutaCanonica==='string'&&opened.rutaCanonica.length>0&&stored.SelloDestino===opened.selloDestino&&stored.RutaCanonica===opened.rutaCanonica,'PROOF_DESTINATION_MISMATCH');
  checkpoint({stage:'ACCOUNT_PROOF_HTTP'});
  const proof=await driver.request('ConsultarLote',{loteId:p.authProofLotId});check(proof?.loteId===p.authProofLotId&&proof.nivelRef===proofLevel,'ACCOUNT_PROOF_FAILED');
  if(p.scenario==='EXP-03'){
    checkpoint({stage:'LEVEL_GRANT_SELECT'});
    const grant=await query('RA','SELECT n.id_nivel AS nivel,n.remit_dest_interno_id_Remit_Dest_Int AS propietario,n.estado_nivel AS nivel_activo,u.Empresa_Gestion_Documental_id_empresa AS empresa,p.remit_dest_interno_id_Remit_Dest_Int AS actor,p.id_permisos_niveles AS permission_id,p.estado_permiso,p.agregar_expediente,p.carga_archivo FROM ra_pro_niveles n INNER JOIN remit_dest_interno u ON u.id_Remit_Dest_Int=n.remit_dest_interno_id_Remit_Dest_Int INNER JOIN ra_pro_permisos_niveles p ON p.ra_pro_niveles_id_nivel=n.id_nivel WHERE n.id_nivel=? AND p.remit_dest_interno_id_Remit_Dest_Int=? AND u.Empresa_Gestion_Documental_id_empresa=? LIMIT 2',[p.profile.nivelRef,p.identity.userRef,p.identity.ambitoRef]);
    check(Array.isArray(grant)&&grant.length===1&&String(grant[0].permission_id)===p.levelPermission.permissionId&&String(grant[0].carga_archivo)==='1','LEVEL_GRANT_INVALID');assertLevelGrant(grant[0],p.identity,p.profile.nivelRef,true);
    const target=await driver.request('AbrirImportacion',{nivelRef:p.profile.nivelRef});check(target?.nivelRef===p.profile.nivelRef,'DESTINATION_CHANGED');
  }
  if(options.preflightOnly){checkpoint({stage:'PREFLIGHT_READONLY_COMPLETE'});return {preflightOnly:true};}
  const roots=p.profile.rootNames.map((nombre,i)=>({seleccionId:'s'+(i+1),idCliente:'r'+(i+1),nombre}));
  const files=roots.map((r,i)=>({idCliente:'a'+(i+1),raizIdCliente:r.idCliente,nombre:FILE_NAME,rutaRelativa:r.nombre+'/'+FILE_NAME,bytes:String(bytes.length),ultimaModificacionLocal:'0'}));
  checkpoint({stage:'PREPARATION_REQUESTED'});
  const plan=await driver.request('PrepararPlan',{nivelRef:p.profile.nivelRef,versionId:p.profile.versionId,raices:roots,archivos:files});
  check(id(plan?.planId),'PLAN_INVALID');checkpoint({stage:'PLAN_KNOWN',planId:plan.planId});
  check(plan.nivelRef===p.profile.nivelRef&&plan.snapshot?.VersionId===p.profile.versionId&&plan.snapshot.Configuracion.ModoIntegridad==='COMPLETO_OBLIGATORIO'&&plan.snapshot.Configuracion.AmbitoIntegridad==='LOTE'&&plan.resultado?.PuedeConfirmar===true&&plan.resultado.UnidadesElegibles.length===2,'PLAN_INVALID');
  for(let i=0;i<2;i++){const rows=plan.resultado.Elementos.filter(e=>e.Ambito==='RAIZ'&&e.RaizIdCliente===roots[i].idCliente);check(rows.length===1,'PLAN_VALUES_INVALID');for(const [field,key]of [['CODIGO','codigo'],['NOMBRE','nombre']]){const values=rows[0].Valores.filter(v=>v.DestinoRef===field);check(values.length===1&&values[0].Valor===p.profile.expectations[i][key],'PLAN_VALUES_INVALID');}}
  const confirmation={planId:plan.planId,revisionEsperada:plan.revision,planHashEsperado:plan.planHash,claveConfirmacion:crypto.randomBytes(16).toString('hex')};
  checkpoint({stage:'CONFIRMATION_REQUESTED',planId:plan.planId});
  const lot=await driver.request('ConfirmarPlan',confirmation);check(id(lot?.loteId),'LOT_INVALID');checkpoint({stage:'LOT_KNOWN',planId:plan.planId,loteId:lot.loteId});check(lot.nivelRef===p.profile.nivelRef,'LOT_INVALID');
  let rootRows=await driver.pages(lot.loteId,'RAICES');const fileRows=await driver.pages(lot.loteId,'ARCHIVOS');check(rootRows.length===2&&fileRows.length===2,'RESOURCE_TOTAL_CHANGED');
  for(let i=0;i<2;i++){const row=fileRows.filter(f=>f.idCliente===files[i].idCliente);check(row.length===1&&id(row[0].recursoId),'FILE_CORRELATION_INVALID');checkpoint({stage:'RECEPTION_REQUESTED',planId:plan.planId,loteId:lot.loteId,archivoId:row[0].recursoId});await driver.receive(lot.loteId,row[0],bytes);checkpoint({stage:'RECEPTION_CONFIRMED',planId:plan.planId,loteId:lot.loteId,archivoId:row[0].recursoId});}
  rootRows=await driver.pages(lot.loteId,'RAICES');
  const session=validateSession({scenario:p.scenario,profile:p.profile,identity:p.identity,levelB:p.levelB,lotId:lot.loteId,runtimeRoot:p.runtimeRoot,...(p.scenario==='EXP-03'?{levelPermission:p.levelPermission}:{}),roots:roots.map(r=>{const rows=rootRows.filter(x=>x.idCliente===r.idCliente);check(rows.length===1,'ROOT_CORRELATION_INVALID');return {rootId:rows[0].recursoId,revision:rows[0].revision,key:crypto.randomBytes(16).toString('hex')};})});
  checkpoint({stage:'CREATION_READY',planId:plan.planId,loteId:lot.loteId,session});return session;
}
module.exports={hash,validateSetup,preservation,prepareFreshLot,publicFailure};
