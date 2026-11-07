'use strict';
const {createSelectControls}=require('./doc111-importacion-carpetas.cjs');
const {createAlm01Controls}=require('./doc117-alm01-driver.cjs');
const {preservation}=require('./doc116-exp01-setup.cjs');
const {validateScope,scopeDigest,sha}=require('../../scripts/support/doc117-alm02-candidate.cjs');
const check=(v,c)=>{if(!v)throw Error('DOC117_INTERNAL_'+c);};
const id=v=>typeof v==='string'&&/^[1-9][0-9]{0,9}$/.test(v)&&BigInt(v)<=4294967295n;
const parse=v=>{try{return typeof v==='string'?JSON.parse(v):v;}catch{throw Error('DOC117_INTERNAL_JSON_INVALID');}};
const one=rows=>{check(Array.isArray(rows)&&rows.length===1,'CONTROL_NOT_UNIQUE');return rows[0];};
const equal=(a,b)=>JSON.stringify(a)===JSON.stringify(b),fingerprint=v=>sha(JSON.stringify(v));
function validatePrepared(scope,commands){
 validateScope(scope);check(Array.isArray(commands)&&commands.length===2,'COMMANDS_INVALID');
 for(let i=0;i<2;i++){const c=commands[i],r=scope.resources[i];check(c&&Object.keys(c).sort().join(',')==='claveOperacion,fase,loteId,recursoId,revisionEsperada'&&c.loteId===scope.lotId&&c.recursoId===r.fileId&&c.fase==='ALMACENAR'&&Number.isInteger(c.revisionEsperada)&&c.revisionEsperada>0&&c.revisionEsperada<4294967294&&/^[a-f0-9]{32}$/.test(c.claveOperacion),'COMMANDS_CHANGED');}return scope;
}
function assertIdentity(view,scope){
 const ctx=parse(view.lot.contexto_doc),snap=parse(view.lot.config_doc);
 check(String(view.lot.lote_id)===scope.lotId&&ctx.UsuarioRef==='141'&&ctx.AmbitoRef==='2'&&ctx.NivelRef==='7'&&snap.VersionId==='38'&&snap.Huella===scope.snapshotSha256&&snap.Configuracion?.OrigenGabinete==='DA'&&snap.Configuracion.GabineteId===10&&snap.Configuracion.ModoIntegridad==='COMPLETO_OBLIGATORIO'&&snap.Configuracion.AmbitoIntegridad==='LOTE','DESTINATION_CHANGED');
 check(view.roots.length===2&&view.files.length===2&&String(view.lot.total_raices)==='2'&&String(view.lot.total_archivos)==='2'&&String(view.lot.total_bytes)===scope.resources.reduce((n,r)=>n+BigInt(r.bytes),0n).toString()&&String(view.lot.detencion_solicitada)==='0','INVENTORY_CHANGED');
 for(const r of scope.resources){const root=one(view.roots.filter(x=>String(x.raiz_id)===r.rootId)),file=one(view.files.filter(x=>String(x.archivo_id)===r.fileId)),identity=parse(root.identidad_expediente_doc),values=parse(file.valores_efectivos_doc);
  check(identity?.OrigenLogico==='RA'&&identity.NivelRef==='7'&&identity.ExpedienteId===r.expedienteId&&identity.RelacionNivelId===r.relacionNivelId&&String(file.raiz_id)===r.rootId&&String(file.bytes_previstos)===r.bytes&&file.estado_recepcion==='RECIBIDO','IDENTITY_CHANGED');
  check(Array.isArray(values)&&values.length===2&&values.filter(v=>v.DestinoTipo==='TIPOLOGIA'&&v.DestinoRef==='TIPO_DOCUMENTAL'&&v.Valor==='79'&&!v.Diferido).length===1&&values.filter(v=>v.DestinoTipo==='INDICE_GABINETE'&&v.DestinoRef==='14'&&v.TipoValor==='TEXTO'&&v.Valor==='EEE'&&!v.Diferido).length===1,'VALUES_CHANGED');
 }
 return ctx;
}
function assertInitial(view,scope,commands){
 assertIdentity(view,scope);check(view.lot.estado==='EN_CURSO'&&view.attempts.length===4&&view.attempts.every(a=>['RECEPCION','CREAR_EXPEDIENTE'].includes(a.fase)&&a.estado==='CONFIRMADO'&&a.fin_utc)&&view.attempts.filter(a=>a.fase==='CREAR_EXPEDIENTE').length===2,'INITIAL_ATTEMPTS_CHANGED');
 for(let i=0;i<2;i++){const r=scope.resources[i],file=one(view.files.filter(f=>String(f.archivo_id)===r.fileId)),root=one(view.roots.filter(x=>String(x.raiz_id)===r.rootId));check(file.estado_efecto==='NO_INICIADO'&&!file.intento_activo_id&&!file.resultado_doc&&Number(file.revision)===commands[i].revisionEsperada&&root.estado==='EXPEDIENTE_CONFIRMADO'&&!root.intento_activo_id,'INITIAL_RESOURCE_CHANGED');}
}
function validateProof(proof,scope,index,mode){
 const r=scope.resources[index],p=proof?.pin,w=proof?.writer,m=proof?.fault;
 for(const record of [p,w])check(record&&Object.keys(record).sort().join(',')==='archivoId,intentoId,loteId,raizId,solicitudHash,version'&&record.version===1&&record.loteId===scope.lotId&&record.raizId===r.rootId&&record.archivoId===r.fileId&&id(record.intentoId)&&record.intentoId!=='41'&&record.solicitudHash===r.requestSha256,'PIN_OR_WRITER_INVALID');
 check(equal(p,w),'WRITER_IDENTITY_CHANGED');
 check(m&&Object.keys(m).sort().join(',')==='archivoId,documentoId,estado,intentoId,inventarioId,loteId,modo,raizId'&&m.modo===mode&&m.loteId===p.loteId&&m.raizId===p.raizId&&m.archivoId===p.archivoId&&m.intentoId===p.intentoId&&m.estado==='CONFIRMADO'&&id(m.documentoId)&&id(m.inventarioId),'FAULT_MARKER_INVALID');
 return{loteId:scope.lotId,raizId:r.rootId,archivoId:r.fileId,intentoId:m.intentoId,expedienteId:r.expedienteId,relacionNivelId:r.relacionNivelId,documentoId:m.documentoId,inventarioId:m.inventarioId};
}
function assertCheckpoint(view,scope,index,proof,confirmed){
 assertIdentity(view,scope);const r=scope.resources[index],file=one(view.files.filter(x=>String(x.archivo_id)===r.fileId)),attempt=one(view.attempts.filter(x=>x.fase==='ALMACENAR'&&String(x.archivo_id)===r.fileId));
 check(String(attempt.intento_id)===proof.intentoId&&String(attempt.raiz_id)===r.rootId,'ATTEMPT_CHANGED');
 const events=view.events.filter(e=>String(e.intento_id)===proof.intentoId);
 check(events.filter(e=>e.accion==='RESERVAR_INTENTO').length===1&&events.filter(e=>e.accion==='INICIAR_ALMACENAMIENTO').length===1,'START_NOT_UNIQUE');
 if(confirmed){
  const result=parse(file.resultado_doc),persisted=parse(attempt.evidencia_doc);
  check(attempt.estado==='CONFIRMADO'&&!!attempt.fin_utc&&file.estado_efecto==='COMPLETADO'&&!file.intento_activo_id&&result?.Estado==='CONFIRMADO'&&result.DocumentoId===proof.documentoId&&result.InventarioId===proof.inventarioId&&result.OrigenGabinete==='DA'&&result.GabineteId==='10'&&result.Expediente?.ExpedienteId===r.expedienteId&&result.Expediente.RelacionNivelId===r.relacionNivelId&&equal(result,persisted)&&equal(result.ValoresEfectivos,parse(file.valores_efectivos_doc))&&events.filter(e=>e.accion==='RESULTADO_ALMACENAMIENTO').length===1,'CONFIRMED_CHECKPOINT_INVALID');
 }else{
  const persisted=parse(attempt.evidencia_doc),root=one(view.roots.filter(x=>String(x.raiz_id)===r.rootId));
  check(attempt.estado==='EN_CURSO'&&!attempt.fin_utc&&file.estado_efecto==='EN_CURSO'&&String(file.intento_activo_id)===proof.intentoId&&!file.resultado_doc&&persisted?.Estado==='EN_CURSO'&&!persisted.DocumentoId&&!persisted.InventarioId&&root.estado==='EXPEDIENTE_CONFIRMADO'&&view.lot.estado==='EN_CURSO'&&events.every(e=>e.accion!=='RESULTADO_ALMACENAMIENTO'),'UNCERTAINTY_NOT_RETAINED');
 }
}
function assertResponse(response,scope,index,proof,confirmed,replay=false){
 const r=response?.body?.datos,ref=scope.resources[index];
 check(response?.status===(confirmed?200:409)&&response.body.ok===confirmed&&response.body.codigo===(confirmed?'OK':'RESULTADO_INCIERTO')&&r?.estado===(confirmed?'CONFIRMADO':'INCIERTO')&&r.fase==='ALMACENAR'&&r.loteId===scope.lotId&&r.raizId===ref.rootId&&r.archivoId===ref.fileId&&r.intentoId===proof.intentoId&&r.expedienteId===ref.expedienteId&&r.relacionNivelId===ref.relacionNivelId&&r.documentoId===proof.documentoId&&r.inventarioId===proof.inventarioId&&(!replay||r.repeticion===true),'RESPONSE_NOT_CORRELATED');
}
function createControls(query,scope,profile,runtimeRoot){
 const base=createSelectControls(query),retained=createAlm01Controls(query,profile,{userRef:'141',ambitoRef:'2'},runtimeRoot);
 return{...base,
  original:index=>{const r=scope.resources[index];return retained.original(scope.lotId,r.fileId,{bytes:Number(r.bytes),sha256:r.contentSha256});},
  physical:(view,index,proof)=>base.verifyKnownDocument(view,scope.resources[index].rootId,{DocumentoId:proof.documentoId,InventarioId:proof.inventarioId},profile.expectations[index],scope.resources[index].fileName),
  preservation:bounds=>preservation(query,{identity:{userRef:'141',ambitoRef:'2'},preservedLotIds:['18','19','20']},bounds),
  inventoryCount:async()=>{const result=one(await query('RA','SELECT COUNT(*) AS total FROM registro_producion_documental WHERE ID_USUARIO_GESTION=? AND ID_EMPRESA_DOCUMENTO=?',['141','2']));check(/^[0-9]+$/.test(String(result.total)),'INVENTORY_COUNT_INVALID');return String(result.total);}
 };
}
async function runWindows({scope,commands,authorization,send,controls,readProof,onProgress}){
 validatePrepared(scope,commands);check(authorization?.scope==='ALM-02-INTERNAL'&&['authorized','account','environmentAuthorized','select','storage','faultInjection','runtimeChanges','exclusive'].every(k=>authorization[k]===true)&&authorization.resourceScopeSha256===scopeDigest(scope),'SPECIFIC_AUTHORIZATION_REQUIRED');
 check(typeof onProgress==='function','DURABLE_CHECKPOINT_REQUIRED');
 let requests=0;const step=v=>{const written=onProgress(v);check(!written||typeof written.then!=='function','SYNCHRONOUS_CHECKPOINT_REQUIRED');};
 const before=await controls.snapshot(scope.lotId);assertInitial(before,scope,commands);await controls.original(0);await controls.original(1);await controls.assertNoDocuments(before);const countBefore=BigInt(await controls.inventoryCount());
 const request=async(index,replay=false)=>{check(requests<3&&(index===0||!replay),'REQUEST_BUDGET_EXHAUSTED');requests++;step({stage:'STORAGE_REQUESTED',archivoId:scope.resources[index].fileId,requestNumber:requests,replay});return send(structuredClone(commands[index]));};
 const a=await request(0),proofA=await readProof(0),refA=validateProof(proofA,scope,0,'AFTER_CHECKPOINT_COMMIT_BEFORE_RETURN');
 step({stage:'KNOWN_IDENTITIES',...refA});assertResponse(a,scope,0,refA,true);
 const confirmed=await controls.snapshot(scope.lotId);assertCheckpoint(confirmed,scope,0,refA,true);const physicalA=await controls.physical(confirmed,0,refA);await controls.original(0);check(BigInt(await controls.inventoryCount())===countBefore+1n,'UNEXPECTED_DOCUMENT_COUNT');
 const replay=await request(0,true);assertResponse(replay,scope,0,refA,true,true);const afterReplay=await controls.snapshot(scope.lotId),replayedPhysical=await controls.physical(afterReplay,0,refA);
 check(fingerprint(confirmed)===fingerprint(afterReplay)&&physicalA.fingerprint===replayedPhysical.fingerprint&&fingerprint(proofA)===fingerprint(await readProof(0)),'CONFIRMED_REPLAY_CHANGED');await controls.original(0);
 const pending=one(afterReplay.files.filter(f=>String(f.archivo_id)===scope.resources[1].fileId));check(pending.estado_efecto==='NO_INICIADO'&&!pending.intento_activo_id&&!pending.resultado_doc&&Number(pending.revision)===commands[1].revisionEsperada,'SECOND_FILE_CHANGED');
 step({stage:'CONFIRMED_REPLAY_VERIFIED',...refA});
 const b=await request(1),proofB=await readProof(1),refB=validateProof(proofB,scope,1,'AFTER_VERIFICATION_BEFORE_CHECKPOINT');
 step({stage:'KNOWN_IDENTITIES',...refB});assertResponse(b,scope,1,refB,false);
 check(refA.intentoId!==refB.intentoId&&refA.documentoId!==refB.documentoId&&refA.inventarioId!==refB.inventarioId,'IDENTITY_COLLISION');
 const after=await controls.snapshot(scope.lotId);assertCheckpoint(after,scope,0,refA,true);assertCheckpoint(after,scope,1,refB,false);check(after.attempts.filter(a=>a.fase==='ALMACENAR').length===2,'UNEXPECTED_ATTEMPT_COUNT');
 await controls.physical(after,0,refA);await controls.physical(after,1,refB);await controls.original(0);await controls.original(1);check(BigInt(await controls.inventoryCount())===countBefore+2n,'UNEXPECTED_DOCUMENT_COUNT');
 step({stage:'UNCERTAINTY_VERIFIED',...refB});
 return{status:'PASS',scenario:'ALM-02-INTERNAL',laboratoryVariant:true,storageRequests:requests,documents:2,inventoryBefore:countBefore.toString(),inventoryAfter:(countBefore+2n).toString(),afterCommitReturnFailureVerified:true,preCheckpointFailureVerified:true,confirmedReplayUnchanged:true,uncertainStorageNotRepeated:true,persistedUncertainAttemptState:'EN_CURSO',writerEntries:2,originalsVerified:true,references:[refA,refB]};
}
module.exports={validatePrepared,assertIdentity,assertInitial,validateProof,assertCheckpoint,assertResponse,createControls,runWindows};
