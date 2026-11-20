'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs'),path=require('node:path');
const {fixture,click,out,openFixture}=require('./support/doc120-coordinacion-browser.cjs');
const {install,selectFiles}=require('./support/doc120-segmentada-browser.cjs');
test('DOC120-SEGUI01: mil archivos completan con inicio único, DOM acotado y consulta focal',()=>fixture(async page=>{
 await install(page);await selectFiles(page,1000);await click(page,'preparar');await page.getByRole('button',{name:'Confirmar e importar',exact:true}).waitFor();
 assert.equal(await page.evaluate(()=>calls.filter(c=>['ConfirmarPlan','AvanzarLote','RECEPCION'].includes(c.operation)).length),0);
 assert.equal(await page.evaluate(()=>calls.filter(c=>c.operation==='IniciarPreparacionPlan').length),1);
 const readBefore=await page.evaluate(()=>calls.filter(c=>['ConsultarLote','ConsultarRecursosLote','ConsultarRecursoLote'].includes(c.operation)).length);
 await click(page,'confirmar-importar');await page.getByRole('heading',{name:'Importación completada',exact:true}).waitFor({timeout:180000});
 const result=await page.evaluate(()=>({files:files.filter(f=>f.documentoId).length,expeditions:roots.filter(r=>r.expedienteId).length,
  receipts:calls.filter(c=>c.operation==='RECEPCION').length,creates:calls.filter(c=>c.request.fase==='CREAR_EXPEDIENTE').length,
  reads:calls.filter(c=>['ConsultarLote','ConsultarRecursosLote','ConsultarRecursoLote'].includes(c.operation)).length,
  metrics:{renders:doc120Metrics.renders.length,p95:doc120Metrics.renders.slice().sort((a,b)=>a-b)[Math.floor(doc120Metrics.renders.length*.95)],maxDom:doc120Metrics.maxDom,heapIncrease:doc120Metrics.maxHeap-doc120Metrics.initialHeap}}));
 assert.equal(result.files,1000);assert.equal(result.expeditions,1);assert.equal(result.receipts,1000);assert.equal(result.creates,1);assert.equal(result.reads-readBefore,3047);
 assert.ok(result.metrics.maxDom<=2000);assert.ok(result.metrics.p95<=100);assert.ok(result.metrics.heapIncrease<=128*1024*1024);
 for(const width of [390,768,1280]){await page.setViewportSize({width,height:1000});await page.screenshot({path:path.join(out,'segmented-1000-'+width+'.png')});assert.ok(await page.locator('.doc111-dialog').evaluate(e=>e.scrollWidth<=e.clientWidth+2));}
 fs.writeFileSync(path.join(out,'segmented-1000-metrics.json'),JSON.stringify(result,null,2));
},{segmented:true}));
test('DOC120-SEGUI02: pausa de preparación conserva selección y solo continúa con otro clic',()=>fixture(async page=>{
 await install(page);await selectFiles(page,127);await page.evaluate(()=>holdPhase='ValidarPartePreparacion');await click(page,'preparar');await page.waitForFunction(()=>!!held);
 await page.getByRole('button',{name:'Pausar revisión',exact:true}).click();await page.evaluate(()=>{held();held=null;});await page.getByRole('button',{name:'Continuar revisión',exact:true}).waitFor();
 const before=await page.evaluate(()=>calls.length);await page.waitForTimeout(100);assert.equal(await page.evaluate(()=>calls.length),before);assert.equal(await page.evaluate(()=>transfers.length),0);
 await page.getByRole('button',{name:'Continuar revisión',exact:true}).click();await page.getByRole('button',{name:'Confirmar e importar',exact:true}).waitFor();assert.equal(await page.evaluate(()=>calls.filter(c=>c.operation==='IniciarPreparacionPlan').length),1);
},{segmented:true}));
test('DOC120-SEGUI03: vincular carpeta después de consultar no envía y omite los recibidos',()=>fixture(async page=>{
 await install(page);await selectFiles(page,127);await click(page,'preparar');await page.getByRole('button',{name:'Confirmar e importar',exact:true}).waitFor();
 await page.evaluate(()=>holdPhase='RECEPCION');await click(page,'confirmar-importar');await page.waitForFunction(()=>!!held);await click(page,'pausar-importacion');await page.evaluate(()=>{held();held=null;});await page.getByRole('button',{name:'Continuar importación',exact:true}).waitFor();
 const seed=await page.evaluate(()=>({roots,files,manifest,rev}));assert.equal(seed.files.filter(f=>f.estadoRecepcion==='RECIBIDO').length,1);
 await page.reload();await openFixture(page,{segmented:true,seed});await install(page);
 await click(page,'consultar-ultimo');await page.getByRole('button',{name:'Vincular carpeta de originales',exact:true}).waitFor();
 assert.equal(await page.evaluate(()=>calls.filter(c=>c.operation==='ConfirmarPlan').length),0);
 const before=await page.evaluate(()=>transfers.length);
 await page.evaluate(()=>{document.querySelector('[data-doc120-folder-reselect]').click=()=>{};});await page.getByRole('button',{name:'Vincular carpeta de originales',exact:true}).click();
 await page.evaluate(()=>{const transfer=new DataTransfer();for(let i=0;i<127;i++){const file=new File(['%PDF-x'],i+'.pdf',{type:'application/pdf',lastModified:0});Object.defineProperty(file,'webkitRelativePath',{value:'Egresos/'+i+'.pdf'});transfer.items.add(file);}const input=document.querySelector('[data-doc120-folder-reselect]');input.files=transfer.files;input.dispatchEvent(new Event('change'));});
 await page.getByText(/126 originales vinculados/).waitFor();assert.equal(await page.evaluate(()=>transfers.length),before);
 await click(page,'continuar-importacion');await page.getByRole('heading',{name:'Importación completada',exact:true}).waitFor({timeout:60000});assert.equal(await page.evaluate(()=>transfers.length),126);assert.equal(await page.evaluate(()=>calls.filter(c=>c.request.fase==='CREAR_EXPEDIENTE').length),1);
},{segmented:true}));
