'use strict';
const test=require('node:test'),assert=require('node:assert/strict');
const {fixture,select,click}=require('./support/doc120-coordinacion-browser.cjs');
const {install,selectFiles}=require('./support/doc120-segmentada-browser.cjs');
const writes=page=>page.evaluate(()=>calls.filter(c=>['ConfirmarPlan','RECEPCION','AvanzarLote'].includes(c.operation)).length);
async function query(page,id){
 await click(page,'abrir-consulta');await page.getByLabel('Referencia del lote',{exact:true}).filter({visible:true}).fill(id);await page.locator('[data-doc111-consultar]').filter({visible:true}).click();
 await page.waitForFunction(id=>calls.some(c=>c.operation==='ConsultarRecursosLote'&&c.request.loteId===id&&c.request.tipo==='RAICES')&&document.querySelector('.doc111-dialog').getAttribute('aria-busy')==='false',id);
}
async function lots(page,preserveOriginal=false){
 await page.evaluate(preserveOriginal=>{
  function dataset(n,prefix){const rs=Array.from({length:n},(_,i)=>({recursoId:String(prefix+i),idCliente:'r'+i,nombre:'Raiz-'+prefix+'-'+i,estado:'PENDIENTE',revision:1,tieneIntentoActivo:false,recepcionListaParaCrear:false}));return {roots:rs,files:rs.map((r,i)=>({recursoId:String(prefix+100+i),raizId:r.recursoId,idCliente:'a'+i,nombre:i+'.pdf',rutaRelativa:i+'.pdf',bytesPrevistos:'6',bytesRecibidos:'0',ultimaModificacionLocal:'0',estadoRecepcion:'PENDIENTE',estado:'NO_INICIADO',revision:1,tieneIntentoActivo:false,valoresEfectivosFijados:false}))};}
  const data={'34':preserveOriginal?{roots,files}:dataset(26,1000),'35':dataset(1,2000)};
  const previous=window.doc120ExtraReply;window.doc120ExtraReply=(op,r)=>{if(op==='ConsultarLote'){const d=data[r.loteId];if(!d)return Promise.reject({status:403,codigo:'ACCESO_DENEGADO'});roots=d.roots;files=d.files;}return previous?previous(op,r):undefined;};
 },preserveOriginal);await query(page,'34');
}
async function filters(page){const panel=page.locator('[data-doc111-panel="ARCHIVOS"]');if(!await panel.evaluate(e=>e.open))await panel.locator('summary').click();}

test('AUD-01: pagina de raices del lote A no oculta las raices del lote B',()=>fixture(async page=>{
 await lots(page);await page.locator('[data-doc111-action="pagina-raices-resultado"]').filter({hasText:'Siguiente'}).click();
 await page.waitForFunction(()=>calls.filter(c=>c.operation==='ConsultarRecursosLote'&&c.request.tipo==='RAICES').at(-1).request.pagina===2&&document.querySelector('.doc111-dialog').getAttribute('aria-busy')==='false');
 await query(page,'35');const request=await page.evaluate(()=>calls.filter(c=>c.operation==='ConsultarRecursosLote'&&c.request.loteId==='35'&&c.request.tipo==='RAICES').at(-1).request);
 assert.equal(request.pagina,1);assert.equal(await page.getByText('Raiz-2000-0',{exact:true}).count(),1);assert.equal(await writes(page),0);
}));

test('AUD-02: nueva consulta y CSV no muestran un filtro borrador de otra visita',()=>fixture(async page=>{
 await lots(page);await filters(page);await page.getByLabel('Recepción',{exact:true}).selectOption('RECIBIDO');
 // A draft was never applied; visiting B and returning to A starts an unfiltered query.
 await query(page,'35');await query(page,'34');await filters(page);
 assert.equal(await page.getByLabel('Recepción',{exact:true}).inputValue(),'');
 const download=page.waitForEvent('download');await click(page,'exportar');await download;
 assert.equal(await page.evaluate(()=>calls.filter(c=>c.operation==='ExportarPaginaLote').at(-1).request.estadoRecepcion),undefined);assert.equal(await writes(page),0);
}));

test('AUD-03: ayuda sigue abierta mientras llegan respuestas de la importacion',()=>fixture(async page=>{
 await select(page,1,2);await page.evaluate(()=>holdPhase='RECEPCION');await click(page,'confirmar-importar');await page.waitForFunction(()=>!!held);
 await page.getByRole('button',{name:'Cómo funciona',exact:true}).click();assert.equal(await page.locator('.doc111-query-modal').evaluate(e=>e.open),true);
 await page.evaluate(()=>{holdPhase='ALMACENAR';const respond=held;held=null;respond();});await page.waitForFunction(()=>!!held&&calls.some(c=>c.request.fase==='ALMACENAR'));
 assert.equal(await page.locator('.doc111-query-modal').evaluate(e=>e.open),true);assert.match(await page.locator('.doc111-query-modal').textContent(),/Cómo continuar/);
 await page.keyboard.press('Escape');await click(page,'pausar-importacion');await page.evaluate(()=>{const respond=held;held=null;respond();});await page.getByRole('button',{name:'Continuar importación',exact:true}).waitFor();
 assert.equal(await page.evaluate(()=>files.filter(f=>f.documentoId).length),1);
}));

test('AUD-04: fallo del lote consultado no se atribuye a revision pausada y volver conserva su ticket',()=>fixture(async page=>{
 await install(page);await selectFiles(page,127);await page.evaluate(()=>holdPhase='ValidarPartePreparacion');await click(page,'preparar');await page.waitForFunction(()=>!!held);await click(page,'pausar-preparacion');
 await page.evaluate(()=>{const respond=held;held=null;respond();});await page.getByRole('button',{name:'Continuar revisión',exact:true}).waitFor();
 await lots(page);await query(page,'35');await page.evaluate(()=>{roots.forEach(r=>Object.assign(r,{estado:'EXPEDIENTE_CONFIRMADO',expedienteId:'500',relacionNivelId:'600',codigoExpediente:'OTRO',recepcionListaParaCrear:true}));files.forEach(f=>Object.assign(f,{estadoRecepcion:'RECIBIDO',bytesRecibidos:f.bytesPrevistos,valoresEfectivosFijados:true}));rev++;const previous=doc120ExtraReply;doc120ExtraReply=(op,r)=>op==='ConsultarRecursoLote'&&files.some(f=>f.documentoId)?Promise.reject({status:503,codigo:'CAPACIDAD_NO_DISPONIBLE'}):previous(op,r);});
 assert.equal(await page.locator('.doc120-preparation-progress').count(),0);
 await click(page,'continuar-importacion');await page.getByRole('button',{name:'Consultar resultado',exact:true}).waitFor();
 const notice=await page.locator('.doc111-notice').textContent();assert.doesNotMatch(notice,/La preparación se detuvo|No se enviaron PDF ni se crearon expedientes/);assert.match(notice,/confirmada.*vista/);
 assert.equal(await page.evaluate(()=>files.filter(f=>f.documentoId).length),1);await click(page,'volver-importacion');await page.getByRole('button',{name:'Continuar revisión',exact:true}).waitFor();await click(page,'continuar-preparacion');await page.getByRole('button',{name:'Confirmar e importar',exact:true}).waitFor();
 assert.equal(await page.evaluate(()=>calls.filter(c=>c.operation==='IniciarPreparacionPlan').length),1);assert.equal(await page.evaluate(()=>calls.filter(c=>c.operation==='ConfirmarPlan').length),0);
},{segmented:true}));

test('AUD-05: volver al lote guardado conserva sus filtros aplicados y su borrador sin mezclarlos con CSV',()=>fixture(async page=>{
 await select(page,1,2);await click(page,'confirmar-importar');await page.getByRole('heading',{name:'Importación completada',exact:true}).waitFor();
 await filters(page);await page.getByLabel('Efecto documental',{exact:true}).selectOption('COMPLETADO');await click(page,'filtrar-resultados');await page.getByRole('button',{name:'Exportar esta página',exact:true}).waitFor();
 await page.getByLabel('Recepción',{exact:true}).selectOption('RECIBIDO');const before=await writes(page);await lots(page,true);await query(page,'35');await click(page,'volver-importacion');await page.getByRole('heading',{name:'Importación completada',exact:true}).waitFor();await filters(page);
 assert.equal(await page.getByLabel('Efecto documental',{exact:true}).inputValue(),'COMPLETADO');assert.equal(await page.getByLabel('Recepción',{exact:true}).inputValue(),'RECIBIDO');
 const download=page.waitForEvent('download');await click(page,'exportar');await download;const request=await page.evaluate(()=>calls.filter(c=>c.operation==='ExportarPaginaLote').at(-1).request);
 assert.equal(request.loteId,'34');assert.equal(request.estado,'COMPLETADO');assert.equal(request.estadoRecepcion,undefined);assert.equal(await writes(page),before);
}));

test('AUD-06: denegacion cierra transporte sin datos ni efectos y reabrir reautoriza la referencia conocida',()=>fixture(async page=>{
 await lots(page);await click(page,'abrir-consulta');await page.getByLabel('Referencia del lote',{exact:true}).fill('99');await page.locator('[data-doc111-consultar]').click();await page.getByText('No tiene autorización vigente para esta operación.',{exact:true}).waitFor();
 assert.equal(await page.getByText('Raiz-1000-0',{exact:true}).count(),0);assert.equal(await page.locator('[data-doc111-action="continuar-importacion"]').count(),0);assert.equal(await writes(page),0);
 const before=await page.evaluate(()=>calls.length);await page.getByLabel('Referencia del lote',{exact:true}).filter({visible:true}).fill('35');await page.locator('[data-doc111-consultar]').filter({visible:true}).click();await page.getByText('La sesión no está vigente. Inicie sesión y consulte la referencia conservada.',{exact:true}).waitFor();assert.equal(await page.evaluate(()=>calls.length),before);
 await page.locator('.doc111-head [data-doc111-action="cerrar"]').click();await page.locator('[data-doc111-action="confirmar-cierre"]').click();await page.locator('[data-doc111-resume]').click();await page.getByRole('button',{name:'Continuar importación',exact:true}).waitFor();assert.equal(await page.getByText('Raiz-1000-0',{exact:true}).count(),1);assert.equal(await writes(page),0);
}));

test('AUD-07: fallo en lectura inicial de continuacion desbloquea consulta sin enviar ni perder referencia',()=>fixture(async page=>{
 await lots(page);await page.evaluate(()=>failNextRead=true);await click(page,'continuar-importacion');await page.getByRole('button',{name:'Consultar resultado',exact:true}).waitFor();assert.equal(await writes(page),0);
 assert.equal(await page.getByRole('button',{name:'Consultar importación',exact:true}).isEnabled(),true);await click(page,'consultar');await page.getByRole('button',{name:'Continuar importación',exact:true}).waitFor();assert.equal(await writes(page),0);
 assert.match(await page.locator('.doc111-destino').textContent(),/34/);
}));
