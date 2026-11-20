'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),path=require('node:path');
const api=require('../js/gestion/ImportacionCarpetas/ImportacionCarpetasApi.js')({});
const {fixture,select,click,out}=require('./support/doc120-coordinacion-browser.cjs');
function transport(){let xhr;const client=api.crear({url:'/synthetic.asmx',uploadUrl:'/synthetic.ashx',token:'synthetic',xhrFactory:()=>xhr={upload:{},open(){},setRequestHeader(){},send(){this.sent=(this.sent||0)+1;},abort(){this.onabort();}},formDataFactory:()=>({append(){}})});return{client,get xhr(){return xhr;},tick(ms){if(xhr.timeout&&ms>=xhr.timeout&&xhr.ontimeout)xhr.ontimeout();},ack(data){xhr.status=200;xhr.responseText=JSON.stringify({ok:true,codigo:'OK',mensaje:'Synthetic',incidencias:[],datos:data});xhr.onload();}};}
for(const phase of ['RECEPCION','ALMACENAR'])test('WAIT01 '+phase+': después de 31 segundos sigue pendiente hasta el acuse',async()=>{
 const t=transport();let resolved=false;const promise=phase==='RECEPCION'?t.client.recibir({loteId:'20',archivoId:'30',claveIntento:'synthetic'},{},()=>{}):t.client.invocar('AvanzarLote',{loteId:'20',fase:phase,recursoId:'30'});promise.then(()=>resolved=true,()=>{});
 assert.equal(t.xhr.timeout,0);if(t.xhr.upload.onprogress)t.xhr.upload.onprogress({lengthComputable:true,loaded:100,total:100});t.tick(31001);await Promise.resolve();assert.equal(resolved,false);t.ack({confirmed:true});assert.deepEqual(await promise,{confirmed:true});
});
test('WAIT02: consultas, creación y control conservan timeout; fallos y cierre no reintentan',async()=>{
 for(const operation of ['ConsultarLote','ReanudarLote','AvanzarLote']){const t=transport(),promise=t.client.invocar(operation,{loteId:'20',fase:'CREAR_EXPEDIENTE'});assert.equal(t.xhr.timeout,30000);const rejected=assert.rejects(promise,e=>e.codigo==='TIEMPO_AGOTADO');t.tick(31001);await rejected;}
 for(const kind of ['network','session','denied','server','unknown','close']){
  const t=transport(),promise=t.client.invocar('AvanzarLote',{loteId:'20',fase:'ALMACENAR'}),expected={network:'ERROR_RED',session:'SESION_REQUERIDA',denied:'ACCESO_DENEGADO',server:'CAPACIDAD_NO_DISPONIBLE',unknown:'RESPUESTA_INVALIDA',close:'SOLICITUD_CANCELADA'}[kind];
  const rejected=assert.rejects(promise,e=>{assert.equal(e.codigo,expected);assert.equal(e.resultadoIncierto,!['session','denied'].includes(kind));return true;});
  if(kind==='network')t.xhr.onerror();else if(kind==='close')t.client.cerrar();else{t.xhr.status=kind==='session'?401:kind==='denied'?403:kind==='server'?500:200;t.xhr.responseText=kind==='unknown'?'not-json':JSON.stringify({ok:false,codigo:expected,mensaje:'Synthetic',incidencias:[],datos:null});t.xhr.onload();}await rejected;assert.equal(t.xhr.sent,1);
 }
});
for(const phase of ['RECEPCION','ALMACENAR'])for(const closing of [false,true])test('WAIT03 '+phase+' '+(closing?'cierre':'pausa')+': espera y no inicia otro recurso',()=>fixture(async page=>{
 await select(page,1,2);await page.clock.install();await page.evaluate(phase=>{window.holdPhase=phase;const original=XMLHttpRequest.prototype.send;XMLHttpRequest.prototype.send=function(body){window.lastTransport=this;if(body instanceof FormData){const result=original.call(this,body);this.upload.onprogress({lengthComputable:true,loaded:100,total:100});return result;}return original.call(this,body);};},phase);
 await click(page,'confirmar-importar');await page.waitForFunction(()=>!!held);await page.clock.fastForward(31001);
 assert.equal(await page.evaluate(()=>lastTransport.timeout),0);assert.equal(await page.evaluate(()=>files.filter(f=>f.estado==='COMPLETADO').length),0);
 const panel=page.locator('.doc111-next');assert.match(await panel.textContent(),phase==='RECEPCION'?/Esperando confirmación del servidor/:/Guardando documento/);
 for(const width of [390,768,1280]){await page.setViewportSize({width,height:1000});assert.equal(await page.evaluate(()=>document.documentElement.scrollWidth<=innerWidth),true);await page.screenshot({path:path.join(out,'espera-'+phase+'-'+width+'.png')});}
 const before=await page.evaluate(()=>calls.filter(c=>['RECEPCION','AvanzarLote'].includes(c.operation)).length);
 if(closing){await page.getByRole('button',{name:'Pausar y salir',exact:true}).click();await page.locator('.doc120-close-modal[open] [data-doc111-action="confirmar-cierre"]').click();}else await click(page,'pausar-importacion');
 await page.evaluate(()=>{held();held=null;});if(!closing)await page.getByRole('button',{name:'Continuar importación',exact:true}).waitFor();else await page.locator('.doc111-overlay').waitFor({state:'hidden'});
 assert.equal(await page.evaluate(()=>calls.filter(c=>['RECEPCION','AvanzarLote'].includes(c.operation)).length),before);
}));

async function stopped(page,count=3,mode='normal'){
 await select(page,1,count);await page.evaluate(({mode,count})=>{
  window.persistedStop=true;window.resumeCount=0;
  files.forEach((f,i)=>{f.estadoRecepcion='RECIBIDO';f.bytesRecibidos=f.bytesPrevistos;f.valoresEfectivosFijados=true;if(count===1000&&i<4)Object.assign(f,{estado:'COMPLETADO',documentoId:String(300+i),gabineteId:'10'});});
  Object.assign(roots[0],{estado:'EXPEDIENTE_CONFIRMADO',expedienteId:'100',relacionNivelId:'200',codigoExpediente:'MISMO-CODIGO',recepcionListaParaCrear:true});
  window.doc120ExtraReply=(op,request)=>{
   if(op==='ConsultarLote'){const state=doc120State(request.loteId);if(persistedStop){Object.assign(state,{estado:'DETENIDO',detencionSolicitada:true,motivoBloqueo:'REANUDACION_DISPONIBLE',accionesPermitidas:['CONSULTAR','CONSULTAR_RAICES','EXPORTAR_PAGINA',...(mode==='old'?[]:['REANUDAR_LOTE'])]});}if(mode==='stale'&&!persistedStop)state.revision=rev-1;return state;}
   if(op==='ReanudarLote'){resumeCount++;if(request.revisionEsperada!==rev)throw Error('STALE_REVISION');persistedStop=false;rev++;if(mode==='new-stop'){persistedStop=true;rev++;return{loteId:'34',revision:request.revisionEsperada+1,detencionSolicitada:false,estado:'EN_CURSO'};}return{loteId:'34',revision:rev,detencionSolicitada:false,estado:'EN_CURSO'};}
  };
  if(mode==='lost'){const send=XMLHttpRequest.prototype.send;XMLHttpRequest.prototype.send=function(body){if(this.operation==='ReanudarLote'){this.onload=()=>{if(this.onerror)this.onerror();};}return send.call(this,body);};}
 },{mode,count});await click(page,'confirmar-importar');
 await page.getByRole('heading',{name:mode==='old'?'Importación detenida':'Importación detenida; puedes reanudarla',exact:true}).waitFor();
}
test('RESUME01: 1000 recibidos y 4 guardados retoma ID pendiente sin repetir confirmación, recibos ni expediente',()=>fixture(async page=>{
 await stopped(page,1000);await page.evaluate(()=>holdPhase='ALMACENAR');await click(page,'reanudar-importacion');await page.waitForFunction(()=>!!held);await click(page,'pausar-importacion');await page.evaluate(()=>held());await page.getByRole('button',{name:'Continuar importación',exact:true}).waitFor();
 const result=await page.evaluate(()=>({calls,resumeCount,roots,done:files.filter(f=>f.estado==='COMPLETADO').length}));
 assert.equal(result.resumeCount,1);assert.equal(result.done,5);assert.equal(result.roots[0].expedienteId,'100');assert.equal(result.calls.filter(c=>c.operation==='ConfirmarPlan').length,1);assert.equal(result.calls.filter(c=>c.operation==='RECEPCION'||c.request.fase==='CREAR_EXPEDIENTE').length,0);
 assert.deepEqual(result.calls.filter(c=>c.operation==='AvanzarLote').map(c=>[c.request.fase,c.request.recursoId]),[['ALMACENAR','82']]);
}));
for(const mode of ['lost','stale','new-stop','old'])test('RESUME02 '+mode+': no continúa por respuesta perdida, detención nueva o servidor anterior',()=>fixture(async page=>{
 await stopped(page,3,mode);if(mode==='old'){assert.equal(await page.locator('[data-doc111-action="reanudar-importacion"]').count(),0);assert.match(await page.locator('.doc111-next').textContent(),/versión|servidor|Actualizar/);}else{await click(page,'reanudar-importacion');if(mode==='lost'||mode==='stale')await page.getByRole('button',{name:'Consultar resultado',exact:true}).waitFor();else await page.getByRole('heading',{name:'Importación detenida; puedes reanudarla',exact:true}).waitFor();}
 assert.equal(await page.evaluate(()=>calls.filter(c=>c.operation==='AvanzarLote'||c.operation==='RECEPCION').length),0);
 if(mode==='lost'){await click(page,'consultar');assert.equal(await page.evaluate(()=>resumeCount),1);assert.equal(await page.evaluate(()=>calls.filter(c=>c.operation==='AvanzarLote').length),0);await page.getByRole('button',{name:'Cerrar',exact:true}).click();await page.locator('.doc120-close-modal[open] [data-doc111-action="confirmar-cierre"]').click();await page.locator('.doc111-overlay').waitFor({state:'hidden'});await page.locator('[data-doc111-resume]').click();await page.getByRole('button',{name:'Continuar importación',exact:true}).waitFor();assert.equal(await page.evaluate(()=>resumeCount),1);assert.equal(await page.evaluate(()=>calls.filter(c=>c.operation==='AvanzarLote').length),0);}
}));
