'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),path=require('node:path');
const {fixture,select,click,out}=require('./support/doc120-coordinacion-browser.cjs');
const {install,selectFiles}=require('./support/doc120-segmentada-browser.cjs');
const consent=page=>page.locator('.doc120-close-modal[open]');
async function release(page){await page.evaluate(()=>{const response=held;held=null;response();});}
async function noNative(page){await page.evaluate(()=>window.confirm=()=>{throw Error('NATIVE_CONFIRM_NOT_ALLOWED');});}
const effects=page=>page.evaluate(()=>calls.filter(c=>['ConfirmarPlan','RECEPCION','AvanzarLote'].includes(c.operation)).length);

for(const cancel of ['button','Escape'])test('REV-UI01 '+cancel+': cancelar salida no pausa revision ni inicia otra preparacion',()=>fixture(async page=>{
 await install(page);await selectFiles(page,127);await noNative(page);await page.evaluate(()=>holdPhase='ValidarPartePreparacion');await click(page,'preparar');await page.waitForFunction(()=>!!held);const before=await page.evaluate(()=>calls.length);
 await page.getByRole('button',{name:'Pausar y salir',exact:true}).click();await consent(page).waitFor();assert.match(await consent(page).textContent(),/revisión.*no está confirmada/);assert.equal(await page.evaluate(()=>calls.length),before);assert.equal(await page.getByRole('button',{name:'Pausando revisión…',exact:true}).count(),0);
 if(cancel==='Escape')await page.keyboard.press('Escape');else await consent(page).getByRole('button',{name:'Seguir revisando',exact:true}).click();await consent(page).waitFor({state:'hidden'});assert.equal(await page.getByRole('button',{name:'Pausar revisión',exact:true}).isEnabled(),true);assert.equal(await page.locator('.doc111-actionbar [data-doc111-action="continuar-preparacion"]').isDisabled(),true);
 await release(page);await page.locator('[data-doc111-action="confirmar-importar"]').waitFor();assert.equal(await page.evaluate(()=>calls.filter(c=>c.operation==='IniciarPreparacionPlan').length),1);assert.equal(await effects(page),0);
},{segmented:true}));

test('REV-UI02: pausa600/1000 ofrece una continuacion y reutiliza preparacion y partes',()=>fixture(async page=>{
 await install(page);await selectFiles(page,1000);await page.evaluate(()=>{const send=XMLHttpRequest.prototype.send;XMLHttpRequest.prototype.send=function(body){const input=JSON.parse(body).solicitud;if(this.operation==='ValidarPartePreparacion'&&input.tipo==='ARCHIVOS'&&input.numero===6)holdPhase='ValidarPartePreparacion';return send.call(this,body);};});await click(page,'preparar');await page.waitForFunction(()=>!!held);await click(page,'pausar-preparacion');await release(page);
 await page.getByRole('button',{name:'Continuar revisión',exact:true}).waitFor();assert.equal(await page.getByRole('button',{name:'Continuar revisión',exact:true}).count(),1);assert.equal(await page.locator('.doc111-actionbar [data-doc111-action="continuar-preparacion"]').evaluate(el=>el.classList.contains('primary')),true);assert.equal(await page.getByRole('button',{name:'Preparar revisión',exact:true}).count(),0);assert.match(await page.locator('.doc120-preparation-progress').textContent(),/Revisión en pausa.*600 de 1000/);assert.equal(await page.locator('.doc111-notice').getAttribute('role'),'status');
 for(const width of [390,768,1280]){await page.setViewportSize({width,height:900});assert.equal(await page.evaluate(()=>document.documentElement.scrollWidth<=innerWidth),true);await page.screenshot({path:path.join(out,'revision-pausada-'+width+'.png')});}
 const before=await page.evaluate(()=>calls.length);await page.waitForTimeout(60);assert.equal(await page.evaluate(()=>calls.length),before);await click(page,'continuar-preparacion');await page.locator('[data-doc111-action="confirmar-importar"]').waitFor();const result=await page.evaluate(()=>({starts:calls.filter(c=>c.operation==='IniciarPreparacionPlan').length,parts:calls.filter(c=>c.operation==='ValidarPartePreparacion').map(c=>c.request.tipo+':'+c.request.numero)}));assert.equal(result.starts,1);assert.equal(result.parts.length,new Set(result.parts).size);assert.equal(await effects(page),0);
},{segmented:true}));

test('REV-UI03: modificar seleccion pausada invalida continuacion y pide revision nueva',()=>fixture(async page=>{
 await install(page);await selectFiles(page,127);await page.evaluate(()=>holdPhase='ValidarPartePreparacion');await click(page,'preparar');await page.waitForFunction(()=>!!held);await click(page,'pausar-preparacion');await release(page);await page.getByRole('button',{name:'Continuar revisión',exact:true}).waitFor();
 await page.locator('[data-doc111-panel="inventario-archivos"] summary').click();await page.locator('[data-doc111-action="quitar-archivo"]').first().click();await page.getByRole('button',{name:'Preparar revisión',exact:true}).waitFor();assert.equal(await page.getByRole('button',{name:'Continuar revisión',exact:true}).count(),0);assert.equal(await page.locator('.doc120-preparation-progress').count(),0);assert.equal(await effects(page),0);
 await page.evaluate(()=>holdPhase='IniciarPreparacionPlan');await click(page,'preparar');await page.waitForFunction(()=>!!held);assert.equal(await page.evaluate(()=>calls.filter(c=>c.operation==='IniciarPreparacionPlan').length),2);assert.equal(await page.evaluate(()=>calls.at(-1).request.totalArchivos),126);
},{segmented:true}));

test('REV-UI04: seleccion sin revisar usa consentimiento del modulo antes de liberar File',()=>fixture(async page=>{
 await install(page);await selectFiles(page,2);await noNative(page);await page.getByRole('button',{name:'Cerrar',exact:true}).click();await consent(page).waitFor();await consent(page).getByRole('button',{name:'Volver a la revisión',exact:true}).click();assert.equal(await page.locator('.doc111-overlay').count(),1);assert.equal(await effects(page),0);
 await page.getByRole('button',{name:'Cerrar',exact:true}).click();await consent(page).getByRole('button',{name:'Salir de la revisión',exact:true}).click();await page.locator('.doc111-overlay').waitFor({state:'hidden'});assert.equal(await effects(page),0);assert.equal(await page.evaluate(()=>calls.some(c=>c.operation==='IniciarPreparacionPlan')),false);
},{segmented:true}));

test('REV-UI05: confirmacion responde mientras decide y cancelar mantiene flujo sin replay',()=>fixture(async page=>{
 await select(page,1,2);await noNative(page);await page.evaluate(()=>holdPhase='ConfirmarPlan');await click(page,'confirmar-importar');await page.waitForFunction(()=>!!held);await page.getByRole('button',{name:'Pausar y salir',exact:true}).click();await consent(page).waitFor();assert.match(await consent(page).textContent(),/confirmación del plan 45/);assert.doesNotMatch(await consent(page).textContent(),/Todavía no se enviaron PDF/);
 await page.evaluate(()=>holdPhase='RECEPCION');await release(page);await page.waitForFunction(()=>!!held&&calls.some(c=>c.operation==='RECEPCION'));await consent(page).getByRole('button',{name:'Seguir en la importación',exact:true}).click();await release(page);await page.getByRole('heading',{name:'Importación completada',exact:true}).waitFor();assert.equal(await page.evaluate(()=>calls.filter(c=>c.operation==='ConfirmarPlan').length),1);assert.equal(await page.evaluate(()=>files.filter(f=>f.documentoId).length),2);
}));
