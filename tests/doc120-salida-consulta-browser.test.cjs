'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),path=require('node:path');
const {fixture,select,click,openFixture,out}=require('./support/doc120-coordinacion-browser.cjs');
const {install,selectFiles}=require('./support/doc120-segmentada-browser.cjs');
const modal=page=>page.locator('.doc120-close-modal[open]');
async function release(page){await page.evaluate(()=>{const response=held;held=null;response();});}
const writes=page=>page.evaluate(()=>calls.filter(c=>['RECEPCION','AvanzarLote','ConfirmarPlan'].includes(c.operation)).length);
async function queried(page,guard='empty'){
 await install(page);await select(page,1,4);await page.evaluate(()=>holdPhase='RECEPCION');await click(page,'confirmar-importar');await page.waitForFunction(()=>!!held);await click(page,'pausar-importacion');await release(page);await page.locator('[data-doc111-action="continuar-importacion"]').waitFor();
 // Seed a legal existing expedition; all data/transport remain synthetic.
 const seed=await page.evaluate(()=>{Object.assign(roots[0],{estado:'EXPEDIENTE_CONFIRMADO',expedienteId:'100',relacionNivelId:'200',codigoExpediente:'MISMO-CODIGO',recepcionListaParaCrear:true,revision:4});files[0].valoresEfectivosFijados=true;files[0].revision++;rev++;return {roots,files,manifest,rev};});
 if(guard==='other-lot'){await page.evaluate(()=>doc120Independent=true);await page.locator('[data-doc111-action="abrir-consulta"]').click();await page.locator('[data-doc111-lote]').fill('35');await page.locator('[data-doc111-consultar]').click();}
 else{await page.reload();await openFixture(page,{segmented:true,seed});await install(page);await page.evaluate(()=>doc120Independent=true);if(guard==='selection')await selectFiles(page,1);await click(page,'consultar-ultimo');}
 await page.locator('[data-doc111-action="continuar-importacion"]').waitFor();const chooser=page.waitForEvent('filechooser');await page.getByRole('button',{name:'Vincular carpeta de originales',exact:true}).click();await chooser;
 await page.evaluate(()=>{const data=new DataTransfer();for(let i=0;i<4;i++){const f=new File(['%PDF-x'],i+'.pdf',{type:'application/pdf',lastModified:0});Object.defineProperty(f,'webkitRelativePath',{value:'Carpeta-0/'+i+'.pdf'});data.items.add(f);}const input=document.querySelector('[data-doc120-folder-reselect]');input.files=data.files;input.dispatchEvent(new Event('change',{bubbles:true}));});await page.getByText(/3 originales vinculados/).waitFor();
 await page.evaluate(()=>{holdPhase='ALMACENAR';doc120Aborts=0;const abort=XMLHttpRequest.prototype.abort;XMLHttpRequest.prototype.abort=function(){doc120Aborts++;return abort.call(this);};});await click(page,'continuar-importacion');await page.waitForFunction(()=>!!held);
}

for(const close of ['button','Escape'])test('CTX-UI01 '+close+': consulta retomada con guardado vacio pregunta por lote actual y cancelar conserva ejecucion',()=>fixture(async page=>{
 await queried(page);const before=await writes(page);if(close==='Escape')await page.keyboard.press('Escape');else await page.getByRole('button',{name:'Pausar y salir',exact:true}).click();await modal(page).waitFor({timeout:1800});assert.match(await modal(page).textContent(),/Lote 34/);assert.equal(await writes(page),before);assert.equal(await page.evaluate(()=>doc120Aborts),0);
 await modal(page).getByRole('button',{name:'Seguir importando',exact:true}).click();await release(page);await page.getByRole('heading',{name:'Importación completada',exact:true}).waitFor();assert.equal(await page.evaluate(()=>files.filter(f=>f.documentoId).length),4);assert.equal(await page.evaluate(()=>calls.filter(c=>c.operation==='ConfirmarPlan').length),0);
 await page.locator('.doc111-head [data-doc111-action="cerrar"]').click();assert.equal(await page.locator('.doc111-overlay').count(),0);
},{segmented:true}));

test('CTX-UI02: aceptar desde consulta espera acuse y reabrir solo consulta sin efecto repetido',()=>fixture(async page=>{
 await queried(page);const before=await writes(page);await page.getByRole('button',{name:'Pausar y salir',exact:true}).click();await modal(page).waitFor();await modal(page).getByRole('button',{name:'Pausar y salir',exact:true}).click();assert.equal(await page.locator('.doc111-overlay').count(),1);assert.equal(await page.evaluate(()=>doc120Aborts),0);await release(page);await page.locator('.doc111-overlay').waitFor({state:'hidden'});assert.equal(await writes(page),before);
 await page.locator('[data-doc111-resume]').click();await page.locator('[data-doc111-action="continuar-importacion"]').waitFor();assert.equal(await writes(page),before);assert.equal(await page.evaluate(()=>files.filter(f=>f.documentoId).length),1);assert.equal(await page.evaluate(()=>doc120Aborts),0);
},{segmented:true}));

test('CTX-UI03: seleccion no confirmada guardada se advierte aparte del lote visible',()=>fixture(async page=>{
 await queried(page,'selection');await page.getByRole('button',{name:'Pausar y salir',exact:true}).click();await modal(page).waitFor();const text=await modal(page).textContent();assert.match(text,/Lote 34/);assert.match(text,/1 archivo.*revisión sin confirmar/);
 for(const [width,height] of [[1280,900],[768,600],[390,600]]){await page.setViewportSize({width,height});await modal(page).getByRole('button',{name:'Pausar y salir',exact:true}).scrollIntoViewIfNeeded();const bounds=await modal(page).evaluate(el=>{const m=el.getBoundingClientRect(),h=el.querySelector('h2').getBoundingClientRect();return {inside:h.top>=m.top&&h.bottom<=m.bottom&&m.top>=14&&m.bottom<=innerHeight-14,overflow:el.scrollWidth>el.clientWidth};});assert.equal(bounds.inside,true);assert.equal(bounds.overflow,false);await page.screenshot({path:path.join(out,'salida-consulta-'+width+'x'+height+'.png')});}
 await modal(page).getByRole('button',{name:'Seguir importando',exact:true}).click();await release(page);await page.getByRole('heading',{name:'Importación completada',exact:true}).waitFor();
 await page.locator('.doc111-head [data-doc111-action="cerrar"]').click();await modal(page).waitFor();assert.match(await modal(page).textContent(),/revisión.*guardada/);assert.doesNotMatch(await modal(page).textContent(),/Lote 34.*pendiente/);await modal(page).getByRole('button',{name:'Volver a la revisión',exact:true}).click();assert.equal(await page.getByRole('heading',{name:'Importación completada',exact:true}).isVisible(),true);
},{segmented:true}));

test('CTX-UI04: consultar otro lote prioriza ID actual y conserva referencia del guardado',()=>fixture(async page=>{
 await queried(page,'other-lot');await page.getByRole('button',{name:'Pausar y salir',exact:true}).click();await modal(page).waitFor();const text=await modal(page).textContent();assert.match(text,/Lote 35/);assert.match(text,/lote 34.*guardad/);await modal(page).getByRole('button',{name:'Seguir importando',exact:true}).click();await release(page);await page.getByRole('heading',{name:'Importación completada',exact:true}).waitFor();
 await page.locator('.doc111-head [data-doc111-action="cerrar"]').click();await modal(page).waitFor();assert.match(await modal(page).textContent(),/vista guardada del lote 34/);await modal(page).getByRole('button',{name:'Volver a la importación',exact:true}).click();assert.equal(await page.getByRole('heading',{name:'Importación completada',exact:true}).isVisible(),true);
},{segmented:true}));
