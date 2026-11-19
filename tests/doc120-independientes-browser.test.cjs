'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),path=require('node:path');
const {fixture,select,click,out}=require('./support/doc120-coordinacion-browser.cjs');
const {install,selectFiles}=require('./support/doc120-segmentada-browser.cjs');
async function start(page){await page.evaluate(()=>{doc120Independent=true;doc120MissingId='79';});await select(page,1,3);await click(page,'confirmar-importar');await page.locator('[data-doc120-unavailable][open]').waitFor();}
const modal=page=>page.locator('[data-doc120-unavailable]');
test('IND-UI01: archivo, ruta, causa, fase, avance y decision accesibles; continuar guarda C',()=>fixture(async page=>{
 await start(page);const text=await modal(page).textContent();assert.equal(await modal(page).locator('[data-doc111-action="recuperar-originales"]').count(),0);assert.match(text,/1.pdf/);assert.match(text,/Carpeta-0\/1.pdf/);assert.match(text,/almacenamiento temporal/);assert.match(text,/1 de 3 documentos guardados; 2 pendientes/);assert.doesNotMatch(text,/PRIVATE_UNTRUSTED_MESSAGE/);
 assert.equal(await page.evaluate(()=>document.activeElement.id),'doc120-original-title');
 const before=await page.evaluate(()=>calls.length);await page.waitForTimeout(80);assert.equal(await page.evaluate(()=>calls.length),before);
 for(const width of [390,768,1280]){await page.setViewportSize({width,height:900});assert.ok(await modal(page).evaluate(el=>el.scrollWidth<=el.clientWidth+2));await page.screenshot({path:path.join(out,'original-no-disponible-'+width+'.png')});}
 await page.getByRole('button',{name:'Continuar con los demás',exact:true}).click();await modal(page).waitFor({state:'hidden'});await page.getByRole('button',{name:'Continuar importación',exact:true}).waitFor();
 const result=await page.evaluate(()=>({files,calls}));assert.equal(result.files.filter(f=>f.estado==='COMPLETADO').length,2);assert.equal(result.files[1].estado,'NO_INICIADO');assert.equal(result.calls.filter(c=>c.operation==='AvanzarLote'&&c.request.recursoId==='79'&&c.request.fase==='ALMACENAR').length,1);assert.match(await page.locator('.doc111-notice').textContent(),/2 de 3.*1 pendientes/);assert.equal(await page.getByRole('heading',{name:'Importación completada',exact:true}).count(),0);
}));
for(const close of ['Pausar para revisar','Cerrar y pausar','Escape'])test('IND-UI02: '+close+' conserva A y no inicia C',()=>fixture(async page=>{
 await start(page);if(close==='Escape')await page.keyboard.press('Escape');else await modal(page).getByRole('button',{name:close,exact:true}).click();await modal(page).waitFor({state:'hidden'});
 await page.getByRole('button',{name:'Continuar importación',exact:true}).waitFor();assert.equal(await page.evaluate(()=>files[2].estadoRecepcion),'PENDIENTE');assert.equal(await page.evaluate(()=>files[0].estado),'COMPLETADO');assert.equal(await page.locator('.doc111-overlay').count(),1);
}));
test('IND-UI03: continuar explicitamente revalida el mismo pendiente sin repetir A ni crear otro expediente',()=>fixture(async page=>{
 await start(page);await modal(page).getByRole('button',{name:'Pausar para revisar',exact:true}).click();await modal(page).waitFor({state:'hidden'});await page.evaluate(()=>doc120MissingId=null);await click(page,'continuar-importacion');await page.getByRole('heading',{name:'Importación completada',exact:true}).waitFor();
 assert.equal(await page.evaluate(()=>calls.filter(c=>c.operation==='AvanzarLote'&&c.request.fase==='CREAR_EXPEDIENTE').length),1);assert.equal(await page.evaluate(()=>calls.filter(c=>c.operation==='AvanzarLote'&&c.request.fase==='ALMACENAR'&&c.request.recursoId==='78').length),1);
}));
test('IND-UI04: Tab queda en el dialogo; Enter en el titulo no toma una decision',()=>fixture(async page=>{
 await start(page);const before=await page.evaluate(()=>calls.length);await page.keyboard.press('Enter');assert.equal(await modal(page).isVisible(),true);assert.equal(await page.evaluate(()=>calls.length),before);
 for(let i=0;i<8;i++){await page.keyboard.press('Tab');assert.equal(await modal(page).evaluate(el=>el.contains(document.activeElement)),true);}await page.keyboard.press('Escape');await modal(page).waitFor({state:'hidden'});
}));

test('IND-UI06: revision invalida en lectura2 conserva el acuse y no crea ni guarda',()=>fixture(async page=>{
 await install(page);await page.evaluate(()=>{
  doc120Independent=true;document.getElementById('doc111-importacion').setAttribute('data-consulta-focal','2');
  const original=doc120ExtraReply;doc120ExtraReply=(op,input)=>{
   if(op==='ConsultarRecursoLote'&&input.versionLectura===2){const resource=structuredClone(files.find(f=>f.recursoId===input.recursoId));resource.revision='invalid';return {versionLectura:2,estado:doc120State(input.loteId),tipo:input.tipo,recurso:resource,padre:roots[0],huellaRecursos:null,progreso:null};}
   return original(op,input);
  };
 });
 await selectFiles(page,2);await click(page,'preparar');await page.getByRole('button',{name:'Confirmar e importar',exact:true}).waitFor();await click(page,'confirmar-importar');await page.getByRole('button',{name:'Consultar resultado',exact:true}).waitFor();
 assert.equal(await page.evaluate(()=>files.filter(f=>f.estadoRecepcion==='RECIBIDO').length),1);assert.equal(await page.evaluate(()=>calls.filter(c=>c.operation==='AvanzarLote').length),0);assert.equal(await modal(page).isVisible(),false);
},{segmented:true}));

test('IND-UI05: mil archivos por interfaz con protocolo2, sin calcular huella global por fase',()=>fixture(async page=>{
 await install(page);await page.evaluate(()=>{
  doc120Independent=true;document.getElementById('doc111-importacion').setAttribute('data-consulta-focal','2');
  const original=doc120ExtraReply;
  doc120ExtraReply=(op,input)=>{
   if(op==='ConsultarRecursoLote'&&input.versionLectura===2){
    const resource=(input.tipo==='RAICES'?roots:files).find(r=>r.recursoId===input.recursoId);
    return {versionLectura:2,estado:doc120State(input.loteId),tipo:input.tipo,recurso:resource,padre:input.tipo==='ARCHIVOS'?roots.find(r=>r.recursoId===resource.raizId):null,huellaRecursos:null,progreso:null};
   }
   return original(op,input);
  };
  PreparacionSegmentadaImportacion.huellaRecursos=()=>{throw Error('Unexpected global digest in protocol2');};
 });
 await selectFiles(page,1000);await click(page,'preparar');await page.getByRole('button',{name:'Confirmar e importar',exact:true}).waitFor();const start=Date.now();await click(page,'confirmar-importar');await page.getByRole('heading',{name:'Importación completada',exact:true}).waitFor({timeout:180000});
 const result=await page.evaluate(()=>({stored:files.filter(f=>f.estado==='COMPLETADO').length,focal:calls.filter(c=>c.operation==='ConsultarRecursoLote').length,versions:[...new Set(calls.filter(c=>c.operation==='ConsultarRecursoLote').map(c=>c.request.versionLectura))],maxDom:doc120Metrics.maxDom,p95:doc120Metrics.renders.slice().sort((a,b)=>a-b)[Math.floor(doc120Metrics.renders.length*.95)],heapIncrease:doc120Metrics.maxHeap-doc120Metrics.initialHeap}));
 assert.equal(result.stored,1000);assert.equal(result.focal,3001);assert.deepEqual(result.versions,[2]);assert.ok(result.maxDom<=2000);assert.ok(result.p95<=100);assert.ok(result.heapIncrease<=128*1024*1024);
 require('node:fs').writeFileSync(path.join(out,'independientes-1000-metrics.json'),JSON.stringify({...result,elapsedMs:Date.now()-start,transport:'synthetic; no SQL or real storage'},null,2));
},{segmented:true}));
