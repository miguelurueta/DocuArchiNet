'use strict';
const crypto=require('node:crypto');
const check=(v,c)=>{if(!v)throw Error('DOC120_QA_'+c);};
const id=v=>typeof v==='string'&&/^[1-9][0-9]{0,9}$/.test(v)&&Number(v)<=4294967295;
const hash=v=>crypto.createHash('sha256').update(typeof v==='string'||Buffer.isBuffer(v)?v:JSON.stringify(v)).digest('hex');
const MAX=Object.freeze({http:1200,select:120,sessions:3,receipts:1,receiptBytes:677,creations:1,storageCalls:6,newDocuments:6,expectedNewDocuments:4,confirmationCalls:2,milliseconds:45*60*1000});
const SQL={
 headers:"SELECT l.lote_id,l.plan_id,l.nivel_ref,l.version_id,l.estado,l.revision,l.detencion_solicitada,l.total_raices,l.total_archivos,l.config_doc,(SELECT COUNT(*) FROM ra_integracion_intento i WHERE i.lote_id=l.lote_id AND i.estado IN ('RESERVADO','EN_CURSO','INCIERTO')) AS unresolved FROM ra_integracion_lote l WHERE l.lote_id IN (?,?) AND l.solicitante_ref=? AND l.ambito_ref=? ORDER BY l.lote_id",
 roots:'SELECT l.lote_id,r.raiz_id,r.cliente_raiz_id,r.estado,r.revision,r.intento_activo_id,r.datos_doc,r.identidad_expediente_doc FROM ra_integracion_lote l INNER JOIN ra_integracion_raiz r ON r.lote_id=l.lote_id WHERE l.lote_id IN (?,?) AND l.solicitante_ref=? AND l.ambito_ref=? ORDER BY l.lote_id,r.raiz_id LIMIT 4',
 files:"SELECT l.lote_id,f.archivo_id,f.raiz_id,f.cliente_archivo_id,f.revision,f.estado_recepcion,f.estado_efecto,f.bytes_previstos,f.bytes_recibidos,CASE WHEN f.intento_activo_id IS NULL THEN 0 ELSE 1 END AS active_marker,CASE WHEN f.valores_efectivos_doc IS NULL THEN 0 ELSE 1 END AS fixed_values,CASE WHEN f.staging_ref IS NULL THEN 0 ELSE 1 END AS has_staging,f.resultado_doc FROM ra_integracion_lote l INNER JOIN ra_integracion_raiz r ON r.lote_id=l.lote_id INNER JOIN ra_integracion_archivo f ON f.raiz_id=r.raiz_id WHERE l.lote_id IN (?,?) AND l.solicitante_ref=? AND l.ambito_ref=? ORDER BY l.lote_id,f.archivo_id LIMIT 1028",
 plan:"SELECT p.plan_id,p.revision,p.plan_hash,p.estado,l.solicitud_hash AS confirmed_request_hash FROM ra_integracion_plan p INNER JOIN ra_integracion_lote l ON l.plan_id=p.plan_id WHERE l.lote_id=? AND l.solicitante_ref=? AND l.ambito_ref=? LIMIT 2"
};
function one(rows,predicate,code='RESOURCE_CHANGED'){const matches=rows.filter(predicate);check(matches.length===1,code);return matches[0];}
function fingerprint(rows,excluded){const targets=new Set(excluded);return hash(rows.filter(r=>!targets.has(String(r.archivo_id))));}
function bind(data){
 check(data.headers.length===2&&data.roots.length===3&&data.files.length===1027,'INVENTORY_CHANGED');
 const lots={};
 for(const [lot,level,version,count,roots] of [['29','7','40',27,2],['50','223','45',1000,1]]){
  const h=one(data.headers,r=>String(r.lote_id)===lot);check(String(h.nivel_ref)===level&&String(h.version_id)===version&&Number(h.total_archivos)===count&&Number(h.total_raices)===roots&&Number(h.unresolved)===0&&Number(h.detencion_solicitada)===0&&['RECIBIENDO','LISTO','EN_CURSO','PARCIAL'].includes(h.estado),'LOT_NOT_QUIESCENT');
  const snapshot=JSON.parse(h.config_doc);check(snapshot?.Configuracion?.OrigenGabinete==='DA'&&String(snapshot.Configuracion.GabineteId)==='10','CABINET_CHANGED');
  const files=data.files.filter(r=>String(r.lote_id)===lot),r=data.roots.filter(r=>String(r.lote_id)===lot);check(files.length===count&&r.length===roots&&files.every(f=>!Number(f.active_marker))&&r.every(x=>!x.intento_activo_id),'LOT_NOT_QUIESCENT');lots[lot]={header:h,files,roots:r};
 }
 const ordered=lots['29'].files.slice().sort((x,y)=>Number(x.archivo_id)-Number(y.archivo_id));
 const a=ordered[0],b=ordered[1],c=ordered[2];check(a.cliente_archivo_id==='a1','QA_A_CHANGED');
 check(a.estado_recepcion==='RECIBIDO'&&a.estado_efecto==='NO_INICIADO'&&!a.resultado_doc&&Number(a.bytes_recibidos)===677,'QA_A_CHANGED');
 for(const f of [b,c])check(f.estado_recepcion==='PENDIENTE'&&f.estado_efecto==='NO_INICIADO'&&!f.resultado_doc&&!Number(f.has_staging)&&Number(f.bytes_previstos)===677,'QA_PENDING_CHANGED');
 const parent=one(lots['29'].roots,r=>String(r.raiz_id)===String(c.raiz_id));check(String(b.raiz_id)===String(c.raiz_id)&&parent.cliente_raiz_id==='r2'&&parent.estado==='PENDIENTE'&&!parent.identidad_expediente_doc,'QA_ROOT_CHANGED');
 const ready=lots['50'].files.filter(f=>f.estado_recepcion==='RECIBIDO'&&f.estado_efecto==='NO_INICIADO'&&!f.resultado_doc&&Number(f.fixed_values)===1).slice(0,2);check(ready.length===2,'TWO_PENDING_REQUIRED');
 const binding={A:String(a.archivo_id),B:String(b.archivo_id),C:String(c.archivo_id),CClient:c.cliente_archivo_id,createRoot:String(c.raiz_id),D:String(ready[0].archivo_id),E:String(ready[1].archivo_id),lots};
 check(new Set([binding.A,binding.B,binding.C,binding.D,binding.E]).size===5,'RESOURCE_CHANGED');return binding;
}
function confirmationBinding(row){
 const current=Number(row.revision),planHash=Buffer.from(row.plan_hash,'base64').toString('hex');
 check(row.estado==='CONFIRMADO'&&String(row.plan_id)==='90'&&Number.isInteger(current)&&current>1&&current<4294967295&&/^[a-f0-9]{64}$/.test(planHash),'PLAN_CHANGED');
 // ConfirmarPlan increments revision once. Verify the reconstructed ORIGINAL
 // request against the immutable hash in the already existing lot before POST.
 // PerfilImportacionJson.Escribir sorts properties with StringComparer.Ordinal.
 // JSON.stringify preserves insertion order, so use that exact canonical order.
 const original=current-1,requestHash=hash({huella:planHash,planId:'90',revision:original});
 check(Buffer.from(row.confirmed_request_hash,'base64').toString('hex')===requestHash,'CONFIRMATION_REQUEST_HASH_INVALID');
 return{revision:original,hash:planHash};
}
class EffectsGate {
 constructor(binding){this.binding=binding;this.calls={};this.acks=[];}
 permit(operation,p,role='OWNER'){
  const b=this.binding;check(p&&typeof p==='object'&&!Array.isArray(p),'REQUEST_INVALID');
  const exact=fields=>check(Object.keys(p).sort().join(',')===fields.slice().sort().join(','),'REQUEST_FIELDS_INVALID');
  if(['AbrirImportacion','ListarPerfilesImportacion'].includes(operation)){check(['7','223'].includes(p.nivelRef)&&Object.keys(p).every(k=>['nivelRef','pagina','tamanoPagina'].includes(k)),'READ_SCOPE_DENIED');return;}
  if(operation==='ConsultarConfirmacionLote'){exact(['planId']);check(p.planId==='90','READ_SCOPE_DENIED');return;}
  if(['ConsultarLote','ConsultarRecursosLote','ConsultarRecursoLote','ExportarPaginaLote'].includes(operation)){
   check(['29','50'].includes(p.loteId),'READ_SCOPE_DENIED');
   if(operation==='ConsultarLote')exact(['loteId']);
   else if(operation==='ConsultarRecursoLote'){check(['RAICES','ARCHIVOS'].includes(p.tipo)&&Object.keys(p).every(k=>['loteId','tipo','recursoId','versionLectura'].includes(k)),'READ_SCOPE_DENIED');const rows=b.lots[p.loteId][p.tipo==='RAICES'?'roots':'files'],key=p.tipo==='RAICES'?'raiz_id':'archivo_id';check(rows.some(r=>String(r[key])===p.recursoId),'READ_SCOPE_DENIED');}
   else check(['RAICES','ARCHIVOS'].includes(p.tipo)&&Number.isInteger(p.pagina)&&p.pagina>=1&&p.pagina<=41&&Number.isInteger(p.tamanoPagina)&&p.tamanoPagina>=1&&p.tamanoPagina<=200&&Object.keys(p).every(k=>['loteId','tipo','pagina','tamanoPagina','raizId','estado','estadoRecepcion'].includes(k)),'READ_SCOPE_DENIED');
   return;
  }
  check(role==='OWNER','OTHER_ACCOUNT_MUTATION_DENIED');
  let key,maximum=1;
  if(operation==='ConfirmarPlan'){exact(['planId','revisionEsperada','planHashEsperado','claveConfirmacion']);check(b.plan&&p.planId==='90'&&p.revisionEsperada===b.plan.revision&&p.planHashEsperado===b.plan.hash&&/^[a-f0-9]{32}$/.test(p.claveConfirmacion),'CONFIRMATION_SCOPE_DENIED');key='CONFIRM';maximum=2;}
  else if(operation==='RecibirArchivo'){exact(['loteId','archivoId','claveIntento']);check(p.loteId==='29'&&p.archivoId===b.C&&/^[a-f0-9]{32}$/.test(p.claveIntento),'RECEIPT_SCOPE_DENIED');key='RECEIPT';}
  else{
   check(operation==='AvanzarLote','OPERATION_DENIED');exact(['loteId','recursoId','fase','revisionEsperada','claveOperacion']);check(Number.isInteger(p.revisionEsperada)&&p.revisionEsperada>0&&/^[A-Za-z0-9_-]{1,64}$/.test(p.claveOperacion),'REQUEST_INVALID');
   const ui=p.loteId==='29'&&[b.A,b.C].includes(p.recursoId)&&['MATERIALIZAR_VALORES','ALMACENAR'].includes(p.fase),creation=p.loteId==='29'&&p.recursoId===b.createRoot&&p.fase==='CREAR_EXPEDIENTE',main=p.loteId==='50'&&[b.D,b.E].includes(p.recursoId)&&p.fase==='ALMACENAR';check(ui||creation||main,'MUTATION_SCOPE_DENIED');key=p.loteId+':'+p.recursoId+':'+p.fase;if(main&&p.recursoId===b.E)maximum=3;
  }
  check((this.calls[key]||0)<maximum,'REPLAY_OR_LIMIT_DENIED');this.calls[key]=(this.calls[key]||0)+1;
 }
 observe(operation,p,status,body){
  if(!['AvanzarLote','RecibirArchivo','ConfirmarPlan'].includes(operation))return;
  const value={operation,lotId:p.loteId||null,resourceId:p.recursoId||p.archivoId||null,phase:p.fase||null,http:status,code:/^[A-Z][A-Z0-9_]{0,63}$/.test(body?.codigo||'')?body.codigo:'UNKNOWN',ok:body?.ok===true};
  for(const [key,field] of [['documentId','documentoId'],['inventoryId','inventarioId'],['expedientId','expedienteId'],['relationId','relacionNivelId']])if(id(String(body?.datos?.[field])))value[key]=String(body.datos[field]);
  this.acks.push(value);return value;
 }
}
function receiptPayload(binding,bytes){check(Buffer.isBuffer(bytes)&&bytes.length===677&&hash(bytes)==='a5f1924afbae4bd631de893c49a56d9f09219075113e6da2ec983602cff84c6c','FIXTURE_CHANGED');return{loteId:'29',archivoId:binding.C,claveIntento:crypto.randomBytes(16).toString('hex')};}
function failure(e){const v=e?.message||'';return /^DOC120_QA_[A-Z_]+$|^DOC111_(?:SELECT|LOGIN)_[A-Z_]+$|^DOC113_[A-Z_]+$|^DOC119_[A-Z_]+$/.test(v)?v:'DOC120_QA_STOPPED';}
module.exports={MAX,SQL,check,id,hash,one,fingerprint,bind,confirmationBinding,EffectsGate,receiptPayload,failure};
