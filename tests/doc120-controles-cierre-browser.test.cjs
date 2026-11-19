'use strict';
const test=require('node:test'),assert=require('node:assert/strict');
const {fixture,select,click}=require('./support/doc120-coordinacion-browser.cjs');
async function instrument(page,phase){await page.evaluate(phase=>{doc120Aborts=0;holdPhase=phase;const original=XMLHttpRequest.prototype.abort;XMLHttpRequest.prototype.abort=function(){doc120Aborts++;return original.call(this);};},phase);}
const writes=page=>page.evaluate(()=>calls.filter(c=>['RECEPCION','AvanzarLote','ConfirmarPlan'].includes(c.operation)).length);
async function release(page){await page.evaluate(()=>{const reply=held;held=null;reply();});}
for(const phase of ['RECEPCION','CREAR_EXPEDIENTE','MATERIALIZAR_VALORES','ALMACENAR'])for(const close of ['X','Escape'])test('CTL-UI01 '+phase+' '+close+': espera acuse sin aborto y no inicia siguiente efecto',()=>fixture(async page=>{
 await select(page,1,2);await instrument(page,phase);await click(page,'confirmar-importar');await page.waitForFunction(()=>!!held);const before=await writes(page);
 if(close==='X')await page.getByRole('button',{name:'Cerrar',exact:true}).click();else await page.keyboard.press('Escape');
 assert.equal(await page.locator('.doc111-overlay').count(),1);assert.equal(await page.getByRole('button',{name:'Cerrar',exact:true}).isDisabled(),true);assert.match(await page.locator('.doc111-notice').textContent(),/Cierre solicitado/);
 await page.keyboard.press('Escape');assert.equal(await writes(page),before);assert.equal(await page.evaluate(()=>doc120Aborts),0);
 await release(page);await page.locator('.doc111-overlay').waitFor({state:'hidden'});assert.equal(await writes(page),before);assert.equal(await page.evaluate(()=>doc120Aborts),0);
 await page.locator('[data-doc111-resume]').click();await page.locator('[data-doc111-action="continuar-importacion"]').waitFor();assert.equal(await writes(page),before);
}));
test('CTL-UI02: confirmacion tardia conserva lote sin iniciar importacion tras pedir cierre',()=>fixture(async page=>{
 await select(page,1,2);await instrument(page,'ConfirmarPlan');await click(page,'confirmar-importar');await page.waitForFunction(()=>!!held);await page.getByRole('button',{name:'Cerrar',exact:true}).click();await release(page);await page.locator('.doc111-overlay').waitFor({state:'hidden'});
 assert.equal(await page.evaluate(()=>calls.filter(c=>c.operation==='ConfirmarPlan').length),1);assert.equal(await page.evaluate(()=>calls.filter(c=>['RECEPCION','AvanzarLote'].includes(c.operation)).length),0);assert.equal(await page.evaluate(()=>doc120Aborts),0);
 await page.locator('[data-doc111-resume]').click();await page.locator('[data-doc111-action="continuar-importacion"]').waitFor();assert.equal(await page.evaluate(()=>calls.filter(c=>c.operation==='RECEPCION').length),0);
}));
test('CTL-UI03: una consulta pendiente deshabilita clics repetidos y Escape no sustituye su intencion',()=>fixture(async page=>{
 await select(page,1,2);await instrument(page,'ALMACENAR');await click(page,'confirmar-importar');await page.waitForFunction(()=>!!held);const before=await writes(page);
 assert.match(await page.locator('[data-doc111-action="abrir-consulta"]').textContent(),/Pausar y consultar/);
 await page.locator('[data-doc111-action="abrir-consulta"]').click();await page.locator('[data-doc111-action="abrir-consulta"]').evaluate(el=>el.click());await page.keyboard.press('Escape');
 assert.equal(await page.locator('[data-doc111-action="consultar-ultimo"]').isDisabled(),true);assert.equal(await page.locator('.doc111-query-modal').evaluate(e=>e.open),false);
 await release(page);await page.locator('.doc111-query-modal[open]').waitFor();assert.equal(await writes(page),before);assert.equal(await page.evaluate(()=>doc120Aborts),0);
 await page.locator('[data-doc111-lote]').fill('35');await page.locator('[data-doc111-consultar]').click();await page.locator('[data-doc111-action="continuar-importacion"]').waitFor();assert.equal(await writes(page),before);
}));
test('CTL-UI04: cierre de envio manual espera PDF actual y deja los restantes sin recibir',()=>fixture(async page=>{
 await select(page,1,3);await instrument(page,'RECEPCION');await click(page,'confirmar-importar');await page.waitForFunction(()=>!!held);await click(page,'pausar-importacion');await release(page);await page.locator('[data-doc111-action="continuar-importacion"]').waitFor();
 const manual=page.locator('[data-doc111-panel="envio"]');if(!await manual.evaluate(e=>e.open))await manual.locator('summary').click();await page.evaluate(()=>holdPhase='RECEPCION');await click(page,'enviar');await page.waitForFunction(()=>!!held);const before=await writes(page);
 await page.getByRole('button',{name:'Cerrar',exact:true}).click();assert.equal(await page.evaluate(()=>doc120Aborts),0);await release(page);await page.locator('.doc111-overlay').waitFor({state:'hidden'});
 assert.equal(await writes(page),before);assert.equal(await page.evaluate(()=>files.filter(f=>f.estadoRecepcion==='RECIBIDO').length),2);assert.equal(await page.evaluate(()=>files[2].estadoRecepcion),'PENDIENTE');assert.equal(await page.evaluate(()=>doc120Aborts),0);
}));
test('CTL-UI05: cierre con respuesta perdida conserva aviso y referencia, sin replay',()=>fixture(async page=>{
 await select(page,1,2);await instrument(page,'ALMACENAR');await click(page,'confirmar-importar');await page.waitForFunction(()=>!!held);const before=await writes(page);await page.getByRole('button',{name:'Cerrar',exact:true}).click();await page.evaluate(()=>lostPhase='ALMACENAR');await release(page);await page.locator('.doc111-overlay').waitFor({state:'hidden'});
 assert.equal(await writes(page),before);assert.equal(await page.evaluate(()=>doc120Aborts),0);assert.match(await page.locator('[data-doc111-status]').textContent(),/resultado pendiente de comprobar/);
 await page.locator('[data-doc111-resume]').click();await page.locator('[data-doc111-action="continuar-importacion"]').waitFor();assert.equal(await page.evaluate(()=>files.filter(f=>f.documentoId).length),1);assert.equal(await writes(page),before);
}));
test('CTL-UI06: desmontaje forzado invalida callbacks sin esperar ni iniciar siguiente',()=>fixture(async page=>{
 await select(page,1,2);await instrument(page,'ALMACENAR');await click(page,'confirmar-importar');await page.waitForFunction(()=>!!held);const before=await writes(page);await page.evaluate(()=>document.getElementById('doc111-importacion').doc111Instancia.destruir());assert.equal(await page.locator('.doc111-overlay').count(),0);assert.equal(await page.evaluate(()=>doc120Aborts),1);
 await release(page);await page.waitForTimeout(30);assert.equal(await writes(page),before);
}));
test('CTL-UI07: espera de cierre accesible en390/768/1280 y ayuda disponible',()=>fixture(async page=>{
 await select(page,1,2);await instrument(page,'RECEPCION');await click(page,'confirmar-importar');await page.waitForFunction(()=>!!held);await page.getByRole('button',{name:'Cerrar',exact:true}).click();
 for(const width of [390,768,1280]){await page.setViewportSize({width,height:1000});assert.equal(await page.evaluate(()=>document.documentElement.scrollWidth<=innerWidth),true);assert.equal(await page.getByRole('button',{name:'Cómo funciona',exact:true}).isEnabled(),true);assert.equal(await page.locator('[data-doc111-action="consultar-ultimo"]').isDisabled(),true);}
 await page.getByRole('button',{name:'Cómo funciona',exact:true}).click();await page.locator('.doc111-query-modal[open]').waitFor();await page.keyboard.press('Escape');assert.equal(await page.locator('.doc111-overlay').count(),1);await release(page);await page.locator('.doc111-overlay').waitFor({state:'hidden'});
}));
for(const phase of ['IniciarPreparacionPlan','ValidarPartePreparacion'])test('CTL-UI08 '+phase+': cerrar espera metadatos iniciados y conserva referencia sin otra parte',()=>fixture(async page=>{
 const {install,selectFiles}=require('./support/doc120-segmentada-browser.cjs');await install(page);await selectFiles(page,127);await instrument(page,phase);await click(page,'preparar');await page.waitForFunction(()=>!!held);
 const metadataWrites=()=>page.evaluate(()=>calls.filter(c=>['IniciarPreparacionPlan','RegistrarPartePreparacion','ValidarPartePreparacion','SellarPreparacionPlan'].includes(c.operation)).length),before=await metadataWrites();
 await page.getByRole('button',{name:'Cerrar',exact:true}).click();assert.equal(await page.locator('.doc111-overlay').count(),1);assert.equal(await page.evaluate(()=>doc120Aborts),0);await release(page);await page.locator('.doc111-overlay').waitFor({state:'hidden'});
 assert.equal(await metadataWrites(),before);assert.equal(await writes(page),0);assert.equal(await page.evaluate(()=>transfers.length),0);assert.equal(await page.evaluate(()=>document.getElementById('doc111-importacion').doc120PreparacionPendiente),'77');assert.equal(await page.evaluate(()=>doc120Aborts),0);
},{segmented:true}));
test('CTL-UI09: fallo de respuesta en cola manual resuelve cierre con aviso y sin otro envio',()=>fixture(async page=>{
 await select(page,1,3);await instrument(page,'RECEPCION');await click(page,'confirmar-importar');await page.waitForFunction(()=>!!held);await click(page,'pausar-importacion');await release(page);await page.locator('[data-doc111-action="continuar-importacion"]').waitFor();
 await page.locator('[data-doc111-panel="envio"] summary').click();await page.evaluate(()=>holdPhase='RECEPCION');await click(page,'enviar');await page.waitForFunction(()=>!!held);const before=await writes(page);await page.getByRole('button',{name:'Cerrar',exact:true}).click();await page.evaluate(()=>lostPhase='RECEPCION');await release(page);await page.locator('.doc111-overlay').waitFor({state:'hidden'});
 assert.equal(await writes(page),before);assert.equal(await page.evaluate(()=>doc120Aborts),0);assert.match(await page.locator('[data-doc111-status]').textContent(),/resultado pendiente de comprobar/);assert.equal(await page.evaluate(()=>files[2].estadoRecepcion),'PENDIENTE');
}));
