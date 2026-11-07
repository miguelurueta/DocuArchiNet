'use strict';
const {test}=require('node:test'),assert=require('node:assert/strict');
const {PROTOCOL,ROOT,scopeDigest}=require('../tools/e2e/scripts/support/doc117-alm02-candidate.cjs');
const {runWindows}=require('../tools/e2e/tests/support/doc117-alm02-internal.cjs');
function fixture(failure){
 const scope={issueKey:'DOC-117',protocol:PROTOCOL,runtimeRoot:ROOT,runId:'e'.repeat(32),lotId:'900',snapshotSha256:'f'.repeat(64),resources:[{rootId:'901',fileId:'902',expedienteId:'910',relacionNivelId:'911',fileName:'a.pdf',bytes:'677',contentSha256:'d'.repeat(64),requestSha256:'a'.repeat(64)},{rootId:'904',fileId:'905',expedienteId:'912',relacionNivelId:'913',fileName:'b.pdf',bytes:'677',contentSha256:'d'.repeat(64),requestSha256:'b'.repeat(64)}]};
 const commands=scope.resources.map((r,i)=>({loteId:scope.lotId,recursoId:r.fileId,fase:'ALMACENAR',revisionEsperada:3,claveOperacion:String(i+1).repeat(32)}));
 const values=[{DestinoTipo:'TIPOLOGIA',DestinoRef:'TIPO_DOCUMENTAL',Valor:'79',Diferido:false},{DestinoTipo:'INDICE_GABINETE',DestinoRef:'14',TipoValor:'TEXTO',Valor:'EEE',Diferido:false}];
 const view={lot:{lote_id:'900',contexto_doc:JSON.stringify({UsuarioRef:'141',AmbitoRef:'2',NivelRef:'7'}),config_doc:JSON.stringify({VersionId:'38',Huella:scope.snapshotSha256,Configuracion:{OrigenGabinete:'DA',GabineteId:10,ModoIntegridad:'COMPLETO_OBLIGATORIO',AmbitoIntegridad:'LOTE'}}),estado:'EN_CURSO',total_raices:2,total_archivos:2,total_bytes:1354,detencion_solicitada:0},roots:scope.resources.map((r,i)=>({raiz_id:r.rootId,estado:'EXPEDIENTE_CONFIRMADO',identidad_expediente_doc:JSON.stringify({OrigenLogico:'RA',NivelRef:'7',ExpedienteId:r.expedienteId,RelacionNivelId:r.relacionNivelId})})),files:scope.resources.map(r=>({archivo_id:r.fileId,raiz_id:r.rootId,estado_efecto:'NO_INICIADO',estado_recepcion:'RECIBIDO',bytes_previstos:'677',revision:3,valores_efectivos_doc:JSON.stringify(values),resultado_doc:null,intento_activo_id:null})),attempts:scope.resources.flatMap((r,i)=>[{intento_id:String(10+i),raiz_id:r.rootId,archivo_id:r.fileId,fase:'RECEPCION',estado:'CONFIRMADO',fin_utc:'2026-11-06'},{intento_id:String(20+i),raiz_id:r.rootId,fase:'CREAR_EXPEDIENTE',estado:'CONFIRMADO',fin_utc:'2026-11-06'}]),events:[]};
 const proofs=[],calls=[],steps=[];let writers=0,physicalCalls=0;
 const send=async command=>{
  calls.push(command);const index=scope.resources.findIndex(r=>r.fileId===command.recursoId),r=scope.resources[index],replay=index===0&&calls.length===2;
  assert.deepEqual(command,commands[index]);if(!replay){writers++;const attemptId=index?'906':'903',documentoId=index?'991':'990',inventarioId=index?'993':'992';
   const pin={version:1,loteId:scope.lotId,raizId:r.rootId,archivoId:r.fileId,intentoId:attemptId,solicitudHash:r.requestSha256};
   proofs[index]={pin,writer:structuredClone(pin),fault:{modo:index?'AFTER_VERIFICATION_BEFORE_CHECKPOINT':'AFTER_CHECKPOINT_COMMIT_BEFORE_RETURN',loteId:scope.lotId,raizId:r.rootId,archivoId:r.fileId,intentoId:attemptId,documentoId,inventarioId,estado:'CONFIRMADO'}};
   const result={Estado:'CONFIRMADO',DocumentoId:documentoId,InventarioId:inventarioId,OrigenGabinete:'DA',GabineteId:'10',Expediente:{ExpedienteId:r.expedienteId,RelacionNivelId:r.relacionNivelId},ValoresEfectivos:values};
   const file=view.files[index];file.revision++;file.estado_efecto=index?'EN_CURSO':'COMPLETADO';file.intento_activo_id=index?attemptId:null;file.resultado_doc=index?null:JSON.stringify(result);
   view.attempts.push({intento_id:attemptId,raiz_id:r.rootId,archivo_id:r.fileId,fase:'ALMACENAR',estado:index?'EN_CURSO':'CONFIRMADO',fin_utc:index?null:'2026-11-06',evidencia_doc:JSON.stringify(index?{Estado:'EN_CURSO'}:result)});
   for(const accion of ['RESERVAR_INTENTO','INICIAR_ALMACENAMIENTO',...(!index?['RESULTADO_ALMACENAMIENTO']:[])])view.events.push({evento_id:String(view.events.length+1),intento_id:attemptId,accion});
  }
  if(failure==='transport'&&calls.length===1)throw Error('DOC117_INTERNAL_RESPONSE_UNCERTAIN');
  if(failure==='replay'&&replay)view.events.push({accion:'UNEXPECTED'});
  if(failure==='checkpoint-a'&&calls.length===1)view.attempts.at(-1).estado='INCIERTO';
  if(failure==='checkpoint-b'&&index===1)view.attempts.at(-1).estado='CONFIRMADO';
  const p=proofs[index].fault;
  return{status:index?409:200,body:{ok:!index,codigo:index?'RESULTADO_INCIERTO':'OK',datos:{...p,expedienteId:r.expedienteId,relacionNivelId:r.relacionNivelId,estado:index?'INCIERTO':'CONFIRMADO',fase:'ALMACENAR',repeticion:replay}}};
 };
 const readProof=async index=>{const proof=structuredClone(proofs[index]);if(failure==='marker')proof.fault.modo='CLIENT_RESPONSE_DISCARDED';if(failure==='writer')proof.writer.intentoId='999';return proof;};
 const controls={snapshot:async()=>structuredClone(view),original:async()=>{},assertNoDocuments:async()=>assert.equal(writers,0),inventoryCount:async()=>String(writers+(failure==='count'&&writers===1?1:0)),physical:async(view,index,ref)=>{physicalCalls++;if(failure==='physical')throw Error('DOC111_DOCUMENT_RELATION_INVALID');return{fingerprint:'stable-'+index};}};
 const authorization={scope:'ALM-02-INTERNAL',authorized:true,account:true,environmentAuthorized:true,select:true,storage:true,faultInjection:true,runtimeChanges:true,exclusive:true,resourceScopeSha256:scopeDigest(scope)};
 return{calls,steps,get writers(){return writers;},get physicalCalls(){return physicalCalls;},options:{scope,commands,authorization,send,controls,readProof,onProgress:p=>steps.push(p)}};
}
test('ALM02 internal driver proves A, exact replay and uncertain B with exactly three requests',async()=>{
 const f=fixture(),r=await runWindows(f.options);assert.equal(r.status,'PASS');assert.equal(r.storageRequests,3);assert.equal(r.writerEntries,2);assert.equal(f.writers,2);assert.deepEqual(f.calls[0],f.calls[1]);assert.equal(f.calls[2].recursoId,'905');assert.equal(r.persistedUncertainAttemptState,'EN_CURSO');assert.equal(r.references.length,2);assert.ok(!JSON.stringify(f.steps).includes('claveOperacion'));
});
for(const [failure,maximum]of [['transport',1],['marker',1],['writer',1],['checkpoint-a',1],['count',1],['physical',1],['replay',2],['checkpoint-b',3]])test('ALM02 internal stops without retry: '+failure,async()=>{
 const f=fixture(failure);await assert.rejects(runWindows(f.options));assert.equal(f.calls.length,maximum);assert.ok(f.calls.filter(c=>c.recursoId==='905').length<=1);
});
test('ALM02 internal refuses setup authorization before a storage request',async()=>{
 const f=fixture();f.options.authorization.scope='ALM-02-SETUP';await assert.rejects(runWindows(f.options),/SPECIFIC_AUTHORIZATION_REQUIRED/);assert.equal(f.calls.length,0);
});
