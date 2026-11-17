'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),path=require('node:path');
const api=require('../js/gestion/ImportacionCarpetas/ImportacionCarpetasApi.js')({});
const {fixture,select,click,out}=require('./support/doc120-coordinacion-browser.cjs');
const reference={tipo:'ARCHIVOS',id:'98',fase:'ALMACENAR',nombre:'1098.pdf',rutaRelativa:'Egreso/1098.pdf',posicion:2,total:1000};

test('DOC120-STOP01: timeout de consulta conserva espera configurada y certeza sin ocultar la causa',async()=>{
 for(const timeout of [undefined,45000]){
  let xhr;const client=api.crear({url:'/synthetic.asmx',uploadUrl:'/synthetic.ashx',token:'synthetic',timeout,xhrFactory:()=>xhr={open(){},setRequestHeader(){},send(){}}});
  const pending=client.invocar('ConsultarLote',{loteId:'50'});
  const caught=assert.rejects(pending,error=>{assert.equal(error.esperaSegundos,(timeout||30000)/1000);const detail=client.detalleOperacion(error,reference);assert.match(detail.causa,new RegExp(error.esperaSegundos+' segundos'));assert.equal(error.resultadoIncierto,false);assert.match(detail.certeza,/no obtuvo una confirmación/);assert.match(client.describirError(error,{fase:'ALMACENAR',loteId:'50'}),/tiempo de espera/);return true;});
  xhr.ontimeout();await caught;
 }
});
for(const fase of ['RECEPCION','CREAR_EXPEDIENTE','RETOMAR_CREACION','MATERIALIZAR_VALORES','ALMACENAR'])test('DOC120-STOP02: motivo publico, recurso y posicion en '+fase,()=>{
 const detail=api.detalleOperacion({codigo:'CAPACIDAD_NO_DISPONIBLE',resultadoIncierto:true,mensaje:'PRIVATE_SQL_PATH'}, {...reference,fase});
 assert.equal(detail.nombre,'1098.pdf');assert.equal(detail.id,'98');assert.equal(detail.posicion,2);assert.equal(detail.total,1000);assert.equal(detail.causa,api.diagnosticos.CAPACIDAD_NO_DISPONIBLE);assert.doesNotMatch(JSON.stringify(detail),/PRIVATE_SQL_PATH/);
});
test('DOC120-STOP03: acuse confirmado y lectura fallida conservan resultado sin atribuir fallo al guardado',()=>{
 const detail=api.detalleOperacion({codigo:'ERROR_RED',lecturaPosterior:true},reference);
 assert.match(detail.titulo,/confirmada/);assert.match(detail.certeza,/quedó confirmada/);assert.doesNotMatch(detail.certeza,/no se sabe|seguir proces/);
});
test('DOC120-STOP04: respuesta desconocida, denegacion y ruta absoluta no exponen datos internos',()=>{
 const detail=api.detalleOperacion({codigo:'PRIVATE_SQL_ERROR',mensaje:'PRIVATE_PASSWORD'}, {...reference,rutaRelativa:'C:\\private\\document.pdf',posicion:1001});
 assert.match(detail.causa,/No se pudo identificar/);assert.equal(detail.rutaRelativa,'');assert.equal(detail.posicion,null);assert.doesNotMatch(JSON.stringify(detail),/PRIVATE/);
 assert.match(api.detalleOperacion({codigo:'RESPUESTA_INVALIDA'},reference).causa,/No se pudo identificar la causa/);
 for(const codigo of ['ACCESO_DENEGADO','SESION_REQUERIDA','SOLICITUD_NO_VERIFICADA'])assert.equal(api.detalleOperacion({codigo},reference),null);
});

test('DOC120-STOP05: segundo archivo detenido identifica causa y avance antes de terminar la consulta',()=>fixture(async page=>{
 await select(page,1,3);
 await page.evaluate(()=>{
  const base=XMLHttpRequest.prototype.send;
  XMLHttpRequest.prototype.send=function(body){
   const request=body instanceof FormData?null:JSON.parse(body).solicitud;
   if(this.operation==='AvanzarLote'&&request.fase==='ALMACENAR'&&request.recursoId==='79'){
    calls.push({operation:this.operation,request});window.waitingDiagnostic=true;queueMicrotask(()=>this.onerror());return;
   }
   if(this.operation==='ConsultarLote'&&window.waitingDiagnostic){window.releaseDiagnostic=()=>{window.waitingDiagnostic=false;base.call(this,body);};return;}
   base.call(this,body);
  };
 });
 await click(page,'confirmar-importar');await page.waitForFunction(()=>!!window.releaseDiagnostic);
 const panel=page.locator('.doc111-next');await panel.getByRole('heading',{name:'Se interrumpió la comunicación',exact:true}).waitFor();
 assert.doesNotMatch(await panel.textContent(),/30 segundos/);assert.match(await panel.textContent(),/1\.pdf/);assert.match(await panel.textContent(),/2 de 3/);assert.match(await panel.textContent(),/Carpeta-0\/1\.pdf/);
 assert.match(await panel.textContent(),/Último avance confirmado: 1 de 3 documentos guardados/);
 assert.doesNotMatch(await panel.textContent(),/Guardando documento|Operación en curso/);
 assert.equal(await panel.getByText('Código: ERROR_RED · lote 34 · archivo 79',{exact:true}).count(),1);
 assert.equal(await page.getByRole('button',{name:'Consultando resultado…',exact:true}).isDisabled(),true);
 assert.equal(await page.locator('.doc111-sheet>.doc111-notice').evaluate(el=>el.classList.contains('warn')),true);
 for(const width of [390,768,1280]){await page.setViewportSize({width,height:1000});assert.equal(await page.evaluate(()=>document.documentElement.scrollWidth<=innerWidth),true);await page.screenshot({path:path.join(out,'parada-'+width+'.png')});}
 assert.equal(await page.evaluate(()=>calls.filter(c=>c.operation==='AvanzarLote'&&c.request.fase==='ALMACENAR'&&c.request.recursoId==='79').length),1);
 await page.evaluate(()=>releaseDiagnostic());await page.getByRole('button',{name:'Consultar resultado',exact:true}).waitFor();
 assert.equal(await page.getByRole('button',{name:'Consultar resultado',exact:true}).isEnabled(),true);
 assert.equal(await page.evaluate(()=>files.filter(f=>f.estado==='COMPLETADO').length),1);
 assert.equal(await page.evaluate(()=>calls.filter(c=>c.operation==='AvanzarLote'&&c.request.fase==='ALMACENAR').length),2);
}));

test('DOC120-STOP07: consulta posterior que confirma guardado reemplaza la incertidumbre sin continuar sola',()=>fixture(async page=>{
 await select(page,1,3);await page.evaluate(()=>window.lostPhase='ALMACENAR');await click(page,'confirmar-importar');
 await page.getByRole('heading',{name:'Documento guardado; importación en pausa',exact:true}).waitFor();
 const panel=page.locator('.doc111-next');assert.match(await panel.textContent(),/La consulta confirmó que este documento quedó guardado/);assert.doesNotMatch(await panel.textContent(),/Todavía no se sabe/);
 assert.equal(await page.getByRole('button',{name:'Consultar resultado',exact:true}).isEnabled(),true);
 assert.equal(await page.evaluate(()=>files.filter(f=>f.estado==='COMPLETADO').length),1);
 assert.equal(await page.evaluate(()=>calls.filter(c=>c.operation==='AvanzarLote'&&c.request.fase==='ALMACENAR').length),1);
}));

test('DOC120-STOP08: intento activo consultado conserva motivo y recurso sin ofrecer repeticion',()=>fixture(async page=>{
 await select(page,1,2);await page.evaluate(()=>{
  const base=XMLHttpRequest.prototype.send;
  XMLHttpRequest.prototype.send=function(body){
   const request=body instanceof FormData?null:JSON.parse(body).solicitud;
   if(this.operation==='AvanzarLote'&&request.fase==='ALMACENAR'){
    calls.push({operation:this.operation,request});Object.assign(files[0],{estado:'EN_CURSO',tieneIntentoActivo:true,intentoActivoId:'250'});files[0].revision++;rev++;queueMicrotask(()=>this.onerror());return;
   }base.call(this,body);
  };
 });await click(page,'confirmar-importar');await page.getByRole('button',{name:'Consultar resultado',exact:true}).waitFor();
 const panel=page.locator('.doc111-next');assert.match(await panel.textContent(),/Se interrumpió la comunicación/);assert.match(await panel.textContent(),/0\.pdf/);assert.match(await panel.textContent(),/1 de 2/);assert.doesNotMatch(await panel.textContent(),/30 segundos/);assert.match(await panel.textContent(),/Intento activo; guardado todavía sin confirmar/);assert.match(await panel.textContent(),/intento 250/);
 assert.equal(await page.locator('[data-doc111-action="fase"]:visible').count(),0);
 assert.equal(await page.evaluate(()=>calls.filter(c=>c.operation==='AvanzarLote'&&c.request.fase==='ALMACENAR').length),1);
}));

test('DOC120-STOP06: nombre largo como texto, consulta fallida y cierre no repiten efectos',()=>fixture(async page=>{
 await select(page,1,2);
 await page.evaluate(()=>{
  files.forEach(f=>{f.estadoRecepcion='RECIBIDO';f.bytesRecibidos=f.bytesPrevistos;});roots.forEach(r=>r.recepcionListaParaCrear=true);
  files[0].nombre='<img src=x onerror=alert(1)>'+('nombre'.repeat(35))+'.pdf';files[0].rutaRelativa='Carpeta/'+files[0].nombre;
  const base=XMLHttpRequest.prototype.send;
  XMLHttpRequest.prototype.send=function(body){
   const request=body instanceof FormData?null:JSON.parse(body).solicitud;
   if(this.operation==='AvanzarLote'&&request.fase==='ALMACENAR'){calls.push({operation:this.operation,request});window.waitingDiagnostic=true;queueMicrotask(()=>this.onerror());return;}
   if(this.operation==='ConsultarLote'&&window.waitingDiagnostic){window.releaseDiagnostic=()=>{this.status=503;this.responseText='not-json PRIVATE';this.onload();};return;}
   base.call(this,body);
  };
 });
 await click(page,'confirmar-importar');await page.waitForFunction(()=>!!window.releaseDiagnostic);
 const panel=page.locator('.doc111-next');await panel.getByRole('heading',{name:'Se interrumpió la comunicación',exact:true}).waitFor();
 assert.equal(await panel.locator('img').count(),0);assert.match(await panel.textContent(),/<img src=x/);assert.doesNotMatch(await panel.textContent(),/PRIVATE/);
 await page.setViewportSize({width:390,height:844});assert.equal(await page.evaluate(()=>document.documentElement.scrollWidth<=innerWidth),true);
 await page.evaluate(()=>releaseDiagnostic());await page.getByRole('button',{name:'Consultar resultado',exact:true}).waitFor();
 assert.match(await panel.textContent(),/Se interrumpió la comunicación/);assert.match(await panel.textContent(),/La consulta posterior también falló/);assert.doesNotMatch(await panel.textContent(),/PRIVATE/);assert.equal(await page.evaluate(()=>calls.filter(c=>c.operation==='AvanzarLote'&&c.request.fase==='ALMACENAR').length),1);
 await page.getByRole('button',{name:'Cerrar',exact:true}).click();assert.equal(await page.locator('.doc111-overlay').count(),0);
}));
