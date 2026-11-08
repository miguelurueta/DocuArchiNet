'use strict';
const crypto=require('node:crypto');
const {inventoryFingerprint}=require('./doc118-case-preparation.cjs');
const check=(v,c)=>{if(!v)throw Error('DOC118_COMPLETION_'+c);};
const hash=s=>crypto.createHash('sha256').update(s).digest('hex');
const one=(rows,p)=>{const selected=rows.filter(p);check(selected.length===1,'CORRELATION_INVALID');return selected[0];};
const id=v=>typeof v==='string'&&/^[1-9][0-9]{0,9}$/.test(v)&&BigInt(v)<=4294967295n;
function inspectCheckpoint(view,c){
 const p=c.prepared,context=JSON.parse(view.lot.contexto_doc);
 check(String(view.lot.lote_id)===p.loteId&&!['18','21'].includes(p.loteId)&&hash(view.lot.config_doc)===p.snapshotSha256&&context.UsuarioRef===c.actorRef&&context.AmbitoRef===c.ambitoRef&&context.NivelRef===c.profile.nivelRef&&view.roots.length===2&&view.files.length===2&&Number(view.lot.total_raices)===2&&Number(view.lot.total_archivos)===2&&String(view.lot.total_bytes)===c.originalTotalBytes,'ORIGINAL_CONTEXT_CHANGED');
 check(inventoryFingerprint(view)===c.inventorySha256&&view.lot.estado===(c.scenario==='COORD02'?'EN_CURSO':'DETENIDO'),'ORIGINAL_CONTEXT_CHANGED');
 check(view.attempts.every(a=>!['RESERVADO','EN_CURSO','INCIERTO'].includes(a.estado))&&view.roots.every(r=>!r.intento_activo_id)&&view.files.every(f=>!f.intento_activo_id),'OUTCOME_UNCERTAIN');
 const a=one(view.files,f=>String(f.archivo_id)===p.archivoAId),b=one(view.files,f=>String(f.archivo_id)===p.archivoBId),root=one(view.roots,r=>String(r.raiz_id)===String(a.raiz_id)),rootB=one(view.roots,r=>String(r.raiz_id)===String(b.raiz_id));
 const result=JSON.parse(a.resultado_doc),identity=JSON.parse(root.identidad_expediente_doc);
 check(root.estado==='COMPLETADA'&&rootB.estado==='EXPEDIENTE_CONFIRMADO','ROOT_STATE_UNCONFIRMED');
 check(a.estado_efecto==='COMPLETADO'&&a.estado_recepcion==='RECIBIDO'&&id(result?.DocumentoId)&&id(result?.InventarioId)&&result.Estado==='CONFIRMADO'&&result.Expediente?.ExpedienteId===identity.ExpedienteId&&result.Expediente?.RelacionNivelId===identity.RelacionNivelId&&String(identity.ExpedienteId)===String(c.targets.expA),'CHECKPOINT_UNCONFIRMED');
 const proof=one(view.attempts,t=>String(t.archivo_id)===p.archivoAId&&t.fase==='ALMACENAR');
 check(proof.estado==='CONFIRMADO'&&proof.fin_utc&&JSON.stringify(JSON.parse(proof.evidencia_doc))===JSON.stringify(result),'CHECKPOINT_UNCONFIRMED');
 check(b.estado_recepcion==='RECIBIDO'&&b.estado_efecto==='NO_INICIADO'&&!b.resultado_doc&&String(JSON.parse(rootB.identidad_expediente_doc).ExpedienteId)===String(c.targets.expB),'SECOND_RESOURCE_CHANGED');
 return{a,b,root,rootB,result,documentoId:result.DocumentoId,inventarioId:result.InventarioId,fingerprint:hash(JSON.stringify(view))};
}
function checkReply(reply,c,proof){check(reply?.estado==='CONFIRMADO'&&reply.loteId===c.prepared.loteId&&reply.archivoId===c.prepared.archivoAId&&reply.fase==='ALMACENAR'&&reply.documentoId===proof.documentoId&&reply.inventarioId===proof.inventarioId,'REPLAY_IDENTITY_CHANGED');}
async function finish02({caseData:c,firstOutcome,client,controls,observer}){
 check(c.scenario==='COORD02'&&firstOutcome?.status==='DROPPED_AFTER_CHECKPOINT','RESPONSE_LOSS_NOT_CONFIRMED');
 const before=await controls.snapshot(c.prepared.loteId),proof=inspectCheckpoint(before,c);
 check(!Number(before.lot.detencion_solicitada)&&firstOutcome.documentoId===proof.documentoId&&firstOutcome.inventarioId===proof.inventarioId&&firstOutcome.fingerprint===proof.fingerprint,'LOST_CHECKPOINT_CHANGED');
 const initialEntries=await observer.summary();check(initialEntries.entriesA===1&&initialEntries.entriesB===0,'WRITER_ENTRY_COUNT_CHANGED');
 // Exactly one replay, and only after the same durable checkpoint is proven.
 checkReply(await client.request('AvanzarLote',c.command),c,proof);
 check(hash(JSON.stringify(await controls.snapshot(c.prepared.loteId)))===proof.fingerprint,'REPLAY_CHANGED_DATA');
 const replayEntries=await observer.summary();check(replayEntries.entriesA===1&&replayEntries.entriesB===0,'REPLAY_ENTERED_WRITER');
 // The unchanged post-replay snapshot already proves B's current revision.
 const revisionB=Number(proof.b.revision);check(Number.isInteger(revisionB)&&revisionB>0&&revisionB<4294967295,'SECOND_REVISION_CHANGED');
 const reply=await client.request('AvanzarLote',{loteId:c.prepared.loteId,recursoId:c.prepared.archivoBId,fase:'ALMACENAR',revisionEsperada:revisionB,claveOperacion:c.prepared.claveB});
 check(reply?.estado==='CONFIRMADO'&&reply.archivoId===c.prepared.archivoBId&&reply.loteId===c.prepared.loteId&&reply.fase==='ALMACENAR'&&id(reply.documentoId)&&id(reply.inventarioId)&&reply.documentoId!==proof.documentoId&&reply.inventarioId!==proof.inventarioId,'SECOND_EFFECT_UNCONFIRMED');
 const physical=await controls.verify(c.prepared.loteId,c.profile,{fileNames:c.fileNames});check(physical.roots===2&&physical.documents===2,'PHYSICAL_EVIDENCE_UNCONFIRMED');
 const final=await controls.snapshot(c.prepared.loteId),storedB=JSON.parse(one(final.files,f=>String(f.archivo_id)===c.prepared.archivoBId).resultado_doc);
 check(final.lot.estado==='COMPLETADO'&&inventoryFingerprint(final)===c.inventorySha256&&storedB.DocumentoId===reply.documentoId&&storedB.InventarioId===reply.inventarioId,'FINAL_IDENTITY_CHANGED');
 const entries=await observer.summary();check(entries.entriesA===1&&entries.entriesB===1,'WRITER_ENTRY_COUNT_CHANGED');
 return{scenario:c.scenario,loteId:c.prepared.loteId,documents:2,roots:2,identities:[{archivoId:c.prepared.archivoAId,documentoId:proof.documentoId,inventarioId:proof.inventarioId},{archivoId:c.prepared.archivoBId,documentoId:reply.documentoId,inventarioId:reply.inventarioId}],responseLossBoundary:firstOutcome.boundary,replaySameIdentity:true,physicalVerified:true,writerEntries:2,integrationPass:false};
}
async function finish03({caseData:c,client,controls,observer,closeFirstClient}){
 check(c.scenario==='COORD03'&&typeof closeFirstClient==='function','SCOPE_INVALID');
 const before=await controls.snapshot(c.prepared.loteId),proof=inspectCheckpoint(before,c);
 check(before.lot.estado==='DETENIDO'&&Number(before.lot.detencion_solicitada)===1,'STOP_NOT_DURABLE');
 await controls.verifyKnownDocument(before,String(proof.root.raiz_id),proof.result,c.profile.expectations[0],c.fileNames[0]);
 await controls.assertNoDocuments({...before,roots:[proof.rootB]});
 await closeFirstClient();
 const state=await client.request('ConsultarLote',{loteId:c.prepared.loteId});
 check(state.loteId===c.prepared.loteId&&state.nivelRef===c.profile.nivelRef&&state.estado==='DETENIDO'&&state.detencionSolicitada===true&&!state.hayIntentosActivos&&!state.hayIncertidumbre,'REOPEN_STATE_CHANGED');
 check(hash(JSON.stringify(await controls.snapshot(c.prepared.loteId)))===proof.fingerprint,'CLIENT_CLOSE_CHANGED_DATA');
 const b=one(await client.pages(c.prepared.loteId,'ARCHIVOS'),f=>f.recursoId===c.prepared.archivoBId);check(Number.isInteger(b.revision)&&b.revision===Number(proof.b.revision),'SECOND_REVISION_CHANGED');let rejected=false;
 try{await client.request('AvanzarLote',{loteId:c.prepared.loteId,recursoId:c.prepared.archivoBId,fase:'ALMACENAR',revisionEsperada:b.revision,claveOperacion:c.prepared.claveB});}
 catch(error){rejected=error.httpStatus===409&&error.serverCode==='CONFLICTO_REVISION';}
 check(rejected,'POST_STOP_BLOCK_UNCONFIRMED');
 check(hash(JSON.stringify(await controls.snapshot(c.prepared.loteId)))===proof.fingerprint,'POST_STOP_CHANGED_DATA');
 const entries=await observer.summary();check(entries.entriesA===1&&entries.entriesB===0,'POST_STOP_ENTERED_WRITER');
 return{scenario:c.scenario,loteId:c.prepared.loteId,documents:1,roots:2,identities:[{archivoId:c.prepared.archivoAId,documentoId:proof.documentoId,inventarioId:proof.inventarioId}],stopDurable:true,firstContextClosed:true,queryFromSecondSession:true,physicalVerified:true,writerEntries:1,integrationPass:false};
}
module.exports={inspectCheckpoint,finish02,finish03};
