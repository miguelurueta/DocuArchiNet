'use strict';
// Bounded continuation over existing clients, checkpoint reads and observer.
const crypto=require('node:crypto');
const {inventoryFingerprint}=require('./doc118-case-preparation.cjs');
const hash=v=>crypto.createHash('sha256').update(JSON.stringify(v)).digest('hex');
const check=(v,c)=>{if(!v)throw Error('DOC118_RETAINED_'+c);};
const id=v=>typeof v==='string'&&/^[1-9][0-9]{0,9}$/.test(v);
const one=(rows,key,value)=>{const found=rows.filter(r=>String(r[key])===value);check(found.length===1,'RESOURCE_CHANGED');return found[0];};
async function completeRetainedResponse({client,createLostClient,controls,observer,profile,binding,expectedFingerprint,assertActive}){
 check(typeof assertActive==='function'&&binding.loteId==='27'&&binding.fileAId==='46'&&binding.fileBId==='47'&&binding.expA==='676'&&binding.expB==='677'&&/^[a-f0-9]{64}$/.test(expectedFingerprint),'SCOPE_CHANGED');
 assertActive();const before=await controls.snapshot(binding.loteId);check(hash(before)===expectedFingerprint,'AUDITED_SNAPSHOT_CHANGED');
 const context=JSON.parse(before.lot.contexto_doc),cfg=JSON.parse(before.lot.config_doc);
 check(context.UsuarioRef==='141'&&context.AmbitoRef==='2'&&context.NivelRef==='7'&&cfg.VersionId==='38'&&String(cfg.Configuracion.GabineteId)==='10'&&before.lot.estado==='EN_CURSO'&&!Number(before.lot.detencion_solicitada)&&before.roots.length===2&&before.files.length===2,'CONTEXT_CHANGED');
 const a=one(before.files,'archivo_id',binding.fileAId),b=one(before.files,'archivo_id',binding.fileBId),root=one(before.roots,'raiz_id',String(b.raiz_id)),knownA=JSON.parse(a.resultado_doc);
 check(a.estado_efecto==='COMPLETADO'&&knownA.DocumentoId==='16237'&&knownA.InventarioId==='23346'&&b.estado_efecto==='NO_INICIADO'&&b.estado_recepcion==='RECIBIDO'&&!b.resultado_doc&&!b.intento_activo_id&&root.estado==='EXPEDIENTE_CONFIRMADO'&&JSON.parse(root.identidad_expediente_doc).ExpedienteId===binding.expB&&!before.attempts.some(t=>(t.fase==='ALMACENAR'&&String(t.archivo_id)===binding.fileBId)||['RESERVADO','EN_CURSO','INCIERTO'].includes(t.estado)),'RESOURCE_NOT_ELIGIBLE');
 check(Number.isInteger(Number(b.revision))&&Number(b.revision)>0,'REVISION_CHANGED');
 const command={loteId:binding.loteId,recursoId:binding.fileBId,fase:'ALMACENAR',revisionEsperada:Number(b.revision),claveOperacion:crypto.randomBytes(16).toString('hex')},inventory=inventoryFingerprint(before);
 let checkpointProof;
 const checkpoint=async()=>{assertActive();const view=await controls.snapshot(binding.loteId),freshA=one(view.files,'archivo_id',binding.fileAId),freshB=one(view.files,'archivo_id',binding.fileBId),result=JSON.parse(freshB.resultado_doc||'null');
  check(inventoryFingerprint(view)===inventory&&freshA.resultado_doc===a.resultado_doc&&view.lot.estado==='COMPLETADO'&&!Number(view.lot.detencion_solicitada)&&view.roots.every(r=>!r.intento_activo_id)&&view.files.every(f=>!f.intento_activo_id)&&freshB.estado_efecto==='COMPLETADO'&&result?.Estado==='CONFIRMADO'&&id(result.DocumentoId)&&id(result.InventarioId)&&result.DocumentoId!==knownA.DocumentoId&&result.InventarioId!==knownA.InventarioId,'CHECKPOINT_UNCONFIRMED');
  const attempt=one(view.attempts.filter(t=>t.fase==='ALMACENAR'),'archivo_id',binding.fileBId);check(attempt.estado==='CONFIRMADO'&&attempt.fin_utc&&hash(JSON.parse(attempt.evidencia_doc))===hash(result),'CHECKPOINT_UNCONFIRMED');
  checkpointProof={view,result,documentoId:result.DocumentoId,inventarioId:result.InventarioId,fingerprint:hash(view)};return {documentoId:result.DocumentoId,inventarioId:result.InventarioId,fingerprint:checkpointProof.fingerprint};};
 await observer.arm({expA:Number(binding.expA),expB:Number(binding.expB),nameA:'DOC118-C02-A-20261107.pdf',nameB:'DOC118-C02-B-20261107.pdf',resourceSha256:hash(command)});
 // Observe B without freezing it; response loss is injected only after commit.
 await observer.resume();const lost=await createLostClient(command,checkpoint);assertActive();
 try{await lost.client.request('AvanzarLote',command);}catch{}
 const outcome=await lost.outcome();check(outcome.status==='DROPPED_AFTER_CHECKPOINT'&&checkpointProof&&outcome.documentoId===checkpointProof.documentoId&&outcome.inventarioId===checkpointProof.inventarioId&&outcome.fingerprint===checkpointProof.fingerprint,'RESPONSE_LOSS_UNCONFIRMED');
 await controls.verifyKnownDocument(checkpointProof.view,String(root.raiz_id),checkpointProof.result,profile.expectations[1],'DOC118-C02-B-20261107.pdf');
 const firstEntries=await observer.summary();check(firstEntries.entriesA===0&&firstEntries.entriesB===1,'WRITER_COUNT_CHANGED');
 assertActive();const replay=await client.request('AvanzarLote',command);
 check(replay?.estado==='CONFIRMADO'&&replay.loteId===binding.loteId&&replay.archivoId===binding.fileBId&&replay.fase==='ALMACENAR'&&replay.documentoId===checkpointProof.documentoId&&replay.inventarioId===checkpointProof.inventarioId,'REPLAY_IDENTITY_CHANGED');
 check(hash(await controls.snapshot(binding.loteId))===checkpointProof.fingerprint,'REPLAY_CHANGED_DATA');
 const entries=await observer.summary();check(entries.entriesA===0&&entries.entriesB===1,'REPLAY_ENTERED_WRITER');
 return{scenario:'RESPONSE_LOSS_RETAINED_B',loteId:binding.loteId,archivoId:binding.fileBId,documentoId:checkpointProof.documentoId,inventarioId:checkpointProof.inventarioId,existingAUnchanged:true,responseDroppedAfterCheckpoint:true,replaySameIdentity:true,physicalVerified:true,writerEntries:1,fingerprint:checkpointProof.fingerprint,integrationPass:false};
}
module.exports={completeRetainedResponse};
