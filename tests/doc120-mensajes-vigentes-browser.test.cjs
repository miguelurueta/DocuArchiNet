'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),path=require('node:path');
const {fixture,select,click,out}=require('./support/doc120-coordinacion-browser.cjs');
const {install}=require('./support/doc120-segmentada-browser.cjs');
async function release(page){await page.evaluate(()=>{const response=held;held=null;response();});}
const writes=page=>page.evaluate(()=>calls.filter(c=>['RECEPCION','AvanzarLote','ConfirmarPlan'].includes(c.operation)).length);

test('MNS-UI01: consulta retenida al continuar sustituye pausa previa sin enviar otro archivo',()=>fixture(async page=>{
 await select(page,1,2);await page.evaluate(()=>holdPhase='RECEPCION');await click(page,'confirmar-importar');await page.waitForFunction(()=>!!held);await click(page,'pausar-importacion');await release(page);await page.locator('[data-doc111-action="continuar-importacion"]').waitFor();assert.match(await page.locator('.doc111-notice').textContent(),/Importación en pausa/);
 const before=await writes(page);await page.evaluate(()=>holdPhase='ConsultarLote');await click(page,'continuar-importacion');await page.waitForFunction(()=>!!held);const notice=await page.locator('.doc111-notice').textContent();assert.match(notice,/Consultando el lote 34 para continuar/);assert.doesNotMatch(notice,/Importación en pausa|originales vinculados/);assert.equal(await writes(page),before);assert.match(await page.locator('.doc111-next').textContent(),/Consultando las condiciones/);
 await release(page);await page.getByRole('heading',{name:'Importación completada',exact:true}).waitFor();assert.equal(await page.evaluate(()=>calls.filter(c=>c.operation==='ConfirmarPlan').length),1);
}));

test('MNS-UI02: pausa solicitada distingue espera y solo muestra pausa aplicada tras acuse',()=>fixture(async page=>{
 await select(page,1,2);await page.evaluate(()=>holdPhase='ALMACENAR');await click(page,'confirmar-importar');await page.waitForFunction(()=>!!held);const before=await writes(page);await click(page,'pausar-importacion');assert.match(await page.locator('.doc111-notice').textContent(),/Pausa solicitada.*espera la respuesta/);assert.doesNotMatch(await page.locator('.doc111-notice').textContent(),/Importación en pausa/);assert.equal(await writes(page),before);
 await release(page);await page.locator('[data-doc111-action="continuar-importacion"]').waitFor();assert.match(await page.locator('.doc111-notice').textContent(),/Importación en pausa/);assert.equal(await writes(page),before);assert.equal(await page.evaluate(()=>files.filter(f=>f.documentoId).length),1);
}));

for(const fault of ['lost-response','post-read'])test('MNS-UI03 '+fault+': fallo conserva diagnostico y consulta sobre informacion transitoria',()=>fixture(async page=>{
 await select(page,1,2);await page.evaluate(()=>holdPhase='ALMACENAR');await click(page,'confirmar-importar');await page.waitForFunction(()=>!!held);const before=await writes(page);await click(page,'pausar-importacion');await page.evaluate(fault=>{if(fault==='lost-response')lostPhase='ALMACENAR';else failReadAfter='ALMACENAR';},fault);await release(page);await page.getByRole('button',{name:'Consultar resultado',exact:true}).waitFor();
 const text=await page.locator('.doc111-notice').textContent();assert.doesNotMatch(text,/^Pausa solicitada|^Consultando el lote/);assert.match(await page.locator('.doc111-next').textContent(),/guardado|confirmad|Consulta/i);assert.equal(await writes(page),before);assert.equal(await page.evaluate(()=>calls.filter(c=>c.operation==='AvanzarLote'&&c.request.fase==='ALMACENAR').length),1);
}));

test('MNS-UI04: aviso local compacto, titulo dentro del viewport y detalle sin operaciones',()=>fixture(async page=>{
 await install(page);await page.evaluate(()=>doc120Independent=true);await select(page,1,2);await page.evaluate(()=>holdPhase='RECEPCION');await click(page,'confirmar-importar');await page.waitForFunction(()=>!!held);await click(page,'pausar-importacion');await release(page);await page.locator('[data-doc111-action="continuar-importacion"]').waitFor();await click(page,'consultar-ultimo');await click(page,'continuar-importacion');const modal=page.locator('[data-doc120-unavailable][open]');await modal.waitFor();
 const before=await page.evaluate(()=>calls.length);assert.match(await modal.locator('#doc120-original-description').textContent(),/Vincula la carpeta original/);assert.equal(await modal.locator('.doc120-original-details').evaluate(el=>el.open),false);
 for(const [width,height] of [[1552,912],[768,600],[390,600]]){
  await page.setViewportSize({width,height});await modal.evaluate(el=>el.scrollTop=0);const bounds=await modal.evaluate(el=>{const rect=el.getBoundingClientRect(),title=el.querySelector('h2').getBoundingClientRect();return {top:rect.top,bottom:rect.bottom,height:innerHeight,titleTop:title.top,titleBottom:title.bottom,noOverflow:el.scrollWidth<=el.clientWidth};});assert.ok(bounds.top>=14&&bounds.bottom<=bounds.height-14,JSON.stringify(bounds));assert.ok(bounds.titleTop>=bounds.top&&bounds.titleBottom<=bounds.bottom);assert.equal(bounds.noOverflow,true);
  await page.screenshot({path:path.join(out,'aviso-compacto-'+width+'x'+height+'.png')});await modal.locator('.doc120-original-details summary').click();assert.match(await modal.locator('.doc120-original-details').textContent(),/Carpeta-0\/1.pdf/);assert.match(await modal.locator('.doc120-original-details').textContent(),/Lote 34/);await modal.locator('.doc120-original-details summary').click();
  const pause=modal.getByRole('button',{name:'Pausar para revisar',exact:true});await pause.scrollIntoViewIfNeeded();const reachable=await pause.evaluate(el=>{const r=el.getBoundingClientRect();return r.top>=0&&r.bottom<=innerHeight;});assert.equal(reachable,true);await modal.evaluate(el=>el.scrollTop=0);
 }
 assert.equal(await page.evaluate(()=>calls.length),before);assert.equal(await modal.getByRole('button',{name:'Vincular carpeta de originales',exact:true}).isEnabled(),true);await page.keyboard.press('Escape');await modal.waitFor({state:'hidden'});await page.locator('.doc111-actionbar [data-doc111-action="vincular-carpeta"]').waitFor();assert.equal(await page.evaluate(()=>calls.length),before);
},{segmented:true}));
