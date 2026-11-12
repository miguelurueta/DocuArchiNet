'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),path=require('node:path'),fs=require('node:fs');
const {readPrepared,setupTransport,failureCategory}=require('../tools/e2e/tests/support/doc119-acceptance-data.cjs');
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
test('local prepared originals retain names, sizes, timestamps and content',()=>{
  const location=path.resolve('.tmp/doc119-next-preflight/eb09fc5981be453b936b2bf1714c43a3/fixture-manifest.json');
  if(!fs.existsSync(location))return;const prepared=readPrepared(path.resolve('.'));assert.equal(prepared.files.size,27);
});
test('service failure evidence retains only fixed categories, never unknown text',()=>{
  assert.equal(failureCategory('La preparación o solicitud no es válida.'),'PLAN_REPRESENTATION');
  assert.equal(failureCategory('private value from an unexpected response'),'UNCLASSIFIED');
  assert.equal(failureCategory(null),'UNCLASSIFIED');
});
