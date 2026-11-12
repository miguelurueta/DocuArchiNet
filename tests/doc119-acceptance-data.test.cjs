'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),path=require('node:path'),fs=require('node:fs');
const {readPrepared,findPreparedFile,resumeSource,recoverySource,preparedSummary,setupTransport,failureCategory}=require('../tools/e2e/tests/support/doc119-acceptance-data.cjs');
const {BASE_URL}=require('../tools/e2e/tests/support/doc119-seguimiento.cjs');
const manifest={nivelRef:'7',versionId:'40',raices:[],archivos:[{nombre:'DOC119-001.pdf'}]};
function harness(){
  const sent=[],counts=[];const refs={lotId:'31',receiptFileId:'41',confirmation:{planId:'21',revisionEsperada:1,planHashEsperado:'hash',claveConfirmacion:'confirmation'},creation:{loteId:'31',recursoId:'51',fase:'CREAR_EXPEDIENTE',revisionEsperada:1,claveOperacion:'creation'}};
  const transport=setupTransport({post:async(url,options)=>{sent.push({url,options});return 'response';}},{take:kind=>counts.push(kind)},manifest,refs);
  const post=(operation,payload)=>transport.post(new URL('webservice/WebServiceImportacionCarpetas.asmx/'+operation,BASE_URL).href,{maxRedirects:0,maxRetries:0,data:{solicitud:payload}});
  return {sent,counts,refs,transport,post};
}
test('only one confirmation is sent; uncertain result cannot be retried',async()=>{
  const h=harness();await h.post('ConfirmarPlan',h.refs.confirmation);
  await assert.rejects(h.post('ConfirmarPlan',h.refs.confirmation),/SETUP_REPLAY_DENIED/);assert.equal(h.sent.length,1);
});
test('confirmation must match the reviewed plan',async()=>{
  const h=harness();await assert.rejects(h.post('ConfirmarPlan',{...h.refs.confirmation,planId:'22'}),/SETUP_SCOPE_DENIED/);assert.equal(h.sent.length,0);
});
test('creation is restricted to the single authorized root and phase',async()=>{
  const h=harness();for(const payload of [{...h.refs.creation,recursoId:'52'},{...h.refs.creation,loteId:'32'},{...h.refs.creation,fase:'ALMACENAR'}])await assert.rejects(h.post('AvanzarLote',payload),/SETUP_SCOPE_DENIED/);
  await h.post('AvanzarLote',h.refs.creation);await assert.rejects(h.post('AvanzarLote',h.refs.creation),/SETUP_REPLAY_DENIED/);assert.equal(h.sent.length,1);
});
test('no cleanup, stopping, recovery or materialization operation can be sent',async()=>{
  const h=harness();for(const op of ['SolicitarDetencion','ReconciliarRecurso','RecuperarDocumentoV2','LimpiarOriginal','CerrarReservaSinInicio'])await assert.rejects(h.post(op,{loteId:'31'}),/SETUP_SCOPE_DENIED/);assert.equal(h.sent.length,0);
});
test('reads cannot escape the selected lot or exceed the prepared page range',async()=>{
  const h=harness();for(const payload of [{loteId:'32',tipo:'ARCHIVOS',pagina:1,tamanoPagina:25},{loteId:'31',tipo:'ARCHIVOS',pagina:3,tamanoPagina:25},{loteId:'31',tipo:'ARCHIVOS',pagina:1,tamanoPagina:200}])await assert.rejects(h.post('ConsultarRecursosLote',payload),/SETUP_SCOPE_DENIED/);
  await h.post('ConsultarRecursosLote',{loteId:'31',tipo:'ARCHIVOS',pagina:2,tamanoPagina:25});assert.equal(h.sent.length,1);
});
test('upload sends only the authorized PDF content once',async()=>{
  const h=harness(),bytes=fs.readFileSync(path.resolve('tools/e2e/fixtures/doc110-carga-individual.pdf'));
  const multipart={loteId:'31',archivoId:'41',claveIntento:'a'.repeat(32),file:{name:'DOC119-001.pdf',mimeType:'application/pdf',buffer:bytes}};
  const send=body=>h.transport.post(new URL('generic_control/ImportacionCarpetasUpload.ashx',BASE_URL).href,{maxRedirects:0,maxRetries:0,multipart:body});
  await assert.rejects(send({...multipart,archivoId:'42'}),/SETUP_SCOPE_DENIED/);
  await assert.rejects(send({...multipart,file:{...multipart.file,buffer:Buffer.alloc(678)}}),/SETUP_SCOPE_DENIED/);
  await send(multipart);await assert.rejects(send(multipart),/SETUP_REPLAY_DENIED/);assert.equal(h.sent.length,1);
});
test('network budget is checked before an effect; no request on exhausted budget',async()=>{
  let called=false;const transport=setupTransport({post:async()=>{called=true;}},{take(){throw Error('EXHAUSTED');}},manifest,{});
  const send=()=>transport.post(new URL('webservice/WebServiceImportacionCarpetas.asmx/PrepararPlan',BASE_URL).href,{maxRedirects:0,maxRetries:0,data:{solicitud:manifest}});
  await assert.rejects(send(),/EXHAUSTED/);assert.equal(called,false);await assert.rejects(send(),/SETUP_REPLAY_DENIED/);
});
test('external origins and redirect/retry settings are rejected before transport',async()=>{
  const h=harness();for(const url of ['https://example.org/webservice/WebServiceImportacionCarpetas.asmx/PrepararPlan',BASE_URL+'../webservice/WebServiceImportacionCarpetas.asmx/PrepararPlan'])await assert.rejects(h.transport.post(url,{maxRedirects:0,maxRetries:0,data:{solicitud:manifest}}),/SETUP_SCOPE_DENIED/);
  await assert.rejects(h.transport.post(BASE_URL+'webservice/WebServiceImportacionCarpetas.asmx/PrepararPlan',{maxRedirects:1,maxRetries:0,data:{solicitud:manifest}}),/SETUP_SCOPE_DENIED/);assert.equal(h.sent.length,0);
});
test('local prepared originals retain names, sizes, timestamps and content',t=>{
  const location=path.resolve('.tmp/doc119-next-preflight/eb09fc5981be453b936b2bf1714c43a3/fixture-manifest-v2.json');
  if(!fs.existsSync(location)){t.skip('Originales locales excluidos de Git no disponibles');return;}const prepared=readPrepared(path.resolve('.'));assert.equal(prepared.files.size,27);
});
test('reselection chooses the original by relative path among equal PDF names',()=>{
  const prepared={fixture:{request:{archivos:[{idCliente:'a1',nombre:'DOC119-79.pdf',rutaRelativa:'root-a/DOC119-79.pdf'},{idCliente:'a2',nombre:'DOC119-79.pdf',rutaRelativa:'root-b/pendiente-001/DOC119-79.pdf'}]}},files:new Map([['a1',{path:'first'}],['a2',{path:'second'}]])};
  assert.equal(findPreparedFile(prepared,{Nombre:'DOC119-79.pdf',RutaRelativa:'root-b/pendiente-001/DOC119-79.pdf'}).path,'second');
  assert.throws(()=>findPreparedFile(prepared,{Nombre:'DOC119-79.pdf',RutaRelativa:'unknown/DOC119-79.pdf'}),/FILE_CORRELATION_INVALID/);
  prepared.fixture.request.archivos.push({...prepared.fixture.request.archivos[1]});
  assert.throws(()=>findPreparedFile(prepared,{Nombre:'DOC119-79.pdf',RutaRelativa:'root-b/pendiente-001/DOC119-79.pdf'}),/FILE_CORRELATION_INVALID/);
});
test('service failure evidence retains only fixed categories, never unknown text',()=>{
  assert.equal(failureCategory('La preparación o solicitud no es válida.'),'PLAN_REPRESENTATION');
  assert.equal(failureCategory('private value from an unexpected response'),'UNCLASSIFIED');
  assert.equal(failureCategory(null),'UNCLASSIFIED');
});
function preparedSnapshot(){
  return {context:{UsuarioRef:'141',AmbitoRef:'2',NivelRef:'7'},documents:0,data:{lot:{total_archivos:'27',total_raices:'2',total_bytes:'18279'},files:Array.from({length:27},(_,i)=>({archivo_id:String(i+1),raiz_id:i===0?'1':'2',estado_recepcion:i===0?'RECIBIDO':'PENDIENTE',estado_efecto:'NO_INICIADO',bytes_recibidos:i===0?'677':'0',intento_activo_id:null})),roots:[{raiz_id:'1',estado:'EXPEDIENTE_CONFIRMADO',identidad_expediente_doc:'{}',intento_activo_id:null},{raiz_id:'2',estado:'PENDIENTE',intento_activo_id:null}],attempts:[{fase:'RECEPCION',archivo_id:'1',raiz_id:'1',estado:'CONFIRMADO',fin_utc:'2026-11-12'},{fase:'CREAR_EXPEDIENTE',archivo_id:null,raiz_id:'1',estado:'CONFIRMADO',fin_utc:'2026-11-12'}]}};
}
test('prepared data includes a confirmed reception attempt and a separate creation attempt',()=>{
  const result=preparedSummary(preparedSnapshot());assert.equal(result.passed,true);assert.equal(result.receivedBytes,677);assert.deepEqual(result.attemptPhases,['RECEPCION','CREAR_EXPEDIENTE']);
});
test('prepared data rejects missing receipt, extra effects, wrong identity or mutated inventory',()=>{
  for(const mutate of [s=>s.data.attempts.shift(),s=>s.data.attempts.push({...s.data.attempts[1],fase:'ALMACENAR'}),s=>s.data.attempts[1].raiz_id='2',s=>s.context.UsuarioRef='136',s=>s.data.files[1].estado_recepcion='RECIBIDO',s=>s.data.files[0].bytes_recibidos='678',s=>s.documents=1]){
    const snapshot=preparedSnapshot();mutate(snapshot);assert.equal(preparedSummary(snapshot).passed,false);
  }
});
test('readonly continuation requires a closed source with the known effects and bound lot',()=>{
  const runId='a'.repeat(32),source={runId,closed:true,credentialsCleared:true,authorizedPlan:true,authorizationRevision:'20261112-setup-revision2',fixtureRevision:2,baseUrl:BASE_URL,resources:{ownerRef:'141',companyRef:'2',level:'7',versionId:'40',loteId:'29',planId:'45'},operations:{ConfirmarPlan:1,RecibirArchivo:1,AvanzarLote:1},failure:{code:'DOC119_SETUP_POSTCONDITION_FAILED'},manifestHash:'manifest'};
  assert.equal(resumeSource(source,runId).lotId,'29');
  for(const mutate of [s=>s.closed=false,s=>s.operations.AvanzarLote=2,s=>s.resources.ownerRef='136',s=>s.resources.loteId=null,s=>s.failure.code='DIFFERENT_FAILURE']){
    const invalid=structuredClone(source);mutate(invalid);assert.throws(()=>resumeSource(invalid,runId),/RESUME_SOURCE_INVALID/);
  }
});
test('recovery-only continuation requires the real completed SEG-01 and identical fingerprints',()=>{
  const runId='c'.repeat(32),fingerprint='d'.repeat(64);
  const source={runId,sourceRunId:'a'.repeat(32),closed:true,credentialsCleared:true,authorizedPlan:true,authorizationRevision:'20261112-readonly-existing',fixtureRevision:2,mode:'READONLY_EXISTING',baseUrl:BASE_URL,manifestHash:'manifest',resources:{ownerRef:'141',companyRef:'2',level:'7',versionId:'40',loteId:'29',planId:'45'},results:{'SEG-01':{status:'PASS_READONLY',ownerRead:true,filters:true,pagination:true,csvDownload:true,emptyFilter:true,otherAccountDenied:true,otherCsvDenied:true,noBusinessMutation:true,beforeFingerprint:fingerprint,afterFingerprint:fingerprint}}};
  assert.equal(recoverySource(source,runId).acceptedFingerprint,fingerprint);
  for(const mutate of [s=>s.closed=false,s=>s.results['SEG-01'].status='PASS_READONLY_WITH_LIMITS',s=>s.results['SEG-01'].pagination=false,s=>s.results['SEG-01'].afterFingerprint='e'.repeat(64),s=>s.resources.loteId=null,s=>s.sourceRunId='../other']){
    const invalid=structuredClone(source);mutate(invalid);assert.throws(()=>recoverySource(invalid,runId),/RECOVERY_SOURCE_INVALID/);
  }
});
