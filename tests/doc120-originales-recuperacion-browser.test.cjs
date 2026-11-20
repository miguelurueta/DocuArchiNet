'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),path=require('node:path');
const {fixture,click,openFixture,out}=require('./support/doc120-coordinacion-browser.cjs');
const {install,selectFiles}=require('./support/doc120-segmentada-browser.cjs');
const modal=page=>page.locator('[data-doc120-unavailable][open]');
const effects=page=>page.evaluate(()=>calls.filter(c=>['ConfirmarPlan','RECEPCION','AvanzarLote'].includes(c.operation)).length);
async function recover(page,count=30){
 await install(page);await selectFiles(page,count);await click(page,'preparar');await page.locator('[data-doc111-action="confirmar-importar"]').waitFor();await page.evaluate(()=>holdPhase='RECEPCION');await click(page,'confirmar-importar');await page.waitForFunction(()=>!!held);await click(page,'pausar-importacion');await page.evaluate(()=>{held();held=null;});await page.locator('[data-doc111-action="continuar-importacion"]').waitFor();
 // Corpus synthetic, never connected to RA/DA: ten existing confirmed results.
 const seed=await page.evaluate(()=>{Object.assign(roots[0],{estado:'EXPEDIENTE_CONFIRMADO',expedienteId:'100',relacionNivelId:'200',codigoExpediente:'MISMO-CODIGO',recepcionListaParaCrear:true,revision:4});files.slice(0,10).forEach((f,i)=>Object.assign(f,{estadoRecepcion:'RECIBIDO',bytesRecibidos:f.bytesPrevistos,estado:'COMPLETADO',valoresEfectivosFijados:true,documentoId:String(300+i),gabineteId:'10',revision:4}));rev+=10;return {roots,files,manifest,rev};});
 await page.reload();await openFixture(page,{segmented:true,seed});await install(page);await page.evaluate(()=>{
  doc120Independent=true;document.getElementById('doc111-importacion').setAttribute('data-consulta-focal','2');const previous=doc120ExtraReply;
  doc120ExtraReply=(op,input)=>{if(op==='ConsultarRecursoLote'&&input.versionLectura===2){const row=(input.tipo==='RAICES'?roots:files).find(r=>r.recursoId===input.recursoId);return {versionLectura:2,estado:doc120State(input.loteId),tipo:input.tipo,recurso:row,padre:input.tipo==='ARCHIVOS'?roots.find(r=>r.recursoId===row.raizId):null,huellaRecursos:null,progreso:null};}return previous(op,input);};
 });await click(page,'consultar-ultimo');await page.locator('[data-doc111-action="continuar-importacion"]').waitFor();assert.equal(await effects(page),0);
}
async function attachFolder(page,count=30,badIndex=-1){await page.evaluate(({count,badIndex})=>{const data=new DataTransfer();for(let i=0;i<count;i++){const f=new File([i===badIndex?'changed':'%PDF-x'],i+'.pdf',{type:'application/pdf',lastModified:0});Object.defineProperty(f,'webkitRelativePath',{value:'Egresos/'+i+'.pdf'});data.items.add(f);}const input=document.querySelector('[data-doc120-folder-reselect]');input.files=data.files;input.dispatchEvent(new Event('change',{bubbles:true}));},{count,badIndex});}
async function choose(page,mode){const chooser=page.waitForEvent('filechooser');await modal(page).locator('[data-doc111-action="recuperar-originales"]').filter({hasText:mode==='CARPETA'?'Vincular carpeta':'Vincular solo'}).click();await chooser;}

test('ORI-UI01: lote consultado diez de mil, carpeta vincula990 sin enviar y retoma una vez',()=>fixture(async page=>{
 await recover(page,1000);await click(page,'continuar-importacion');await modal(page).waitFor();assert.match(await modal(page).textContent(),/no se ha enviado/);assert.match(await modal(page).textContent(),/10 de 1000 documentos guardados; 990 pendientes/);assert.equal(await effects(page),0);
 for(const width of [390,768,1280]){await page.setViewportSize({width,height:1000});assert.equal(await modal(page).evaluate(el=>el.scrollWidth<=el.clientWidth),true);await page.screenshot({path:path.join(out,'recuperar-originales-'+width+'.png')});}
 await choose(page,'CARPETA');await modal(page).waitFor({state:'hidden'});await attachFolder(page,1000);await page.getByText(/990 originales vinculados/).waitFor();assert.equal(await effects(page),0);assert.match(await page.locator('.doc111-notice').textContent(),/10 ya recibidos se omitieron/);
 await click(page,'continuar-importacion');await page.getByRole('heading',{name:'Importación completada',exact:true}).waitFor({timeout:180000});
 const result=await page.evaluate(()=>({done:files.filter(f=>f.documentoId).length,receipts:calls.filter(c=>c.operation==='RECEPCION'),stores:calls.filter(c=>c.operation==='AvanzarLote'&&c.request.fase==='ALMACENAR'),creates:calls.filter(c=>c.request.fase==='CREAR_EXPEDIENTE').length,confirm:calls.filter(c=>c.operation==='ConfirmarPlan').length,first:files.slice(0,10).map(f=>f.documentoId)}));
 assert.equal(result.done,1000);assert.equal(result.receipts.length,990);assert.equal(new Set(result.receipts.map(c=>c.request.archivoId)).size,990);assert.equal(result.stores.length,990);assert.equal(new Set(result.stores.map(c=>c.request.recursoId)).size,990);assert.equal(result.creates,0);assert.equal(result.confirm,0);assert.deepEqual(result.first,Array.from({length:10},(_,i)=>String(300+i)));
},{segmented:true}));

for(const pause of ['Pausar para revisar','Escape'])test('ORI-UI02 '+pause+': accion visible tras pausar y cancelar selector no envia',()=>fixture(async page=>{
 await recover(page);await click(page,'continuar-importacion');await modal(page).waitFor();if(pause==='Escape')await page.keyboard.press('Escape');else await modal(page).getByRole('button',{name:pause,exact:true}).click();await modal(page).waitFor({state:'hidden'});
 const primary=page.locator('.doc111-actionbar [data-doc111-action="vincular-carpeta"]');await primary.waitFor();assert.equal(await primary.isEnabled(),true);assert.equal(await page.locator('.doc111-actionbar [data-doc111-action="continuar-importacion"]').count(),0);
 const chooser=page.waitForEvent('filechooser');await primary.click();await chooser;await page.evaluate(()=>document.querySelector('[data-doc120-folder-reselect]').dispatchEvent(new Event('cancel')));assert.equal(await effects(page),0);assert.equal(await primary.isEnabled(),true);
},{segmented:true}));

test('ORI-UI03: opcion individual fuera del filtro no toma File de la preparacion y no envia al vincular',()=>fixture(async page=>{
 await recover(page);await page.locator('[data-doc111-panel="ARCHIVOS"] summary').click();await page.getByLabel('Recepción',{exact:true}).selectOption('RECIBIDO');await click(page,'filtrar-resultados');await click(page,'continuar-importacion');await modal(page).waitFor();assert.match(await modal(page).textContent(),/10.pdf/);
 await choose(page,'ARCHIVO');await page.evaluate(()=>{const data=new DataTransfer();data.items.add(new File(['%PDF-x'],'10.pdf',{type:'application/pdf',lastModified:0}));const input=document.querySelector('input[aria-label="Reseleccionar archivo pendiente"]');input.files=data.files;input.dispatchEvent(new Event('change',{bubbles:true}));});await page.getByText(/Archivo vinculado al pendiente 88/).waitFor();assert.equal(await effects(page),0);await page.locator('.doc111-actionbar [data-doc111-action="continuar-importacion"]').waitFor();assert.equal(await page.getByLabel('Recepción',{exact:true}).inputValue(),'RECIBIDO');
},{segmented:true}));

test('ORI-UI04: carpeta de raiz fuera del filtro y archivo modificado conserva recuperacion',()=>fixture(async page=>{
 await recover(page);await page.locator('[data-doc111-panel="RAICES"] summary').click();await page.getByLabel('Estado de carpeta',{exact:true}).selectOption('PENDIENTE');await click(page,'filtrar-raices');await click(page,'continuar-importacion');await modal(page).waitFor();await choose(page,'CARPETA');await attachFolder(page,30,10);
 await page.getByText(/19 originales vinculados/).waitFor();assert.match(await page.locator('.doc111-notice').textContent(),/1 modificados/);await page.locator('.doc111-actionbar [data-doc111-action="vincular-carpeta"]').waitFor();assert.equal(await effects(page),0);assert.equal(await page.getByLabel('Estado de carpeta',{exact:true}).inputValue(),'PENDIENTE');
},{segmented:true}));

test('ORI-UI05: sin selector de carpeta conserva recuperacion individual',()=>fixture(async page=>{
 await recover(page);await page.evaluate(()=>document.getElementById('doc111-importacion').setAttribute('data-consulta-focal','0'));await click(page,'consultar-ultimo');await click(page,'continuar-importacion');await modal(page).waitFor();const button=modal(page).getByRole('button',{name:'Vincular PDF pendiente',exact:true});await button.waitFor();assert.equal(await modal(page).getByRole('button',{name:'Vincular carpeta de originales',exact:true}).count(),0);
 const chooser=page.waitForEvent('filechooser');await button.click();await chooser;await page.locator('.doc111-actionbar [data-doc111-action="reseleccionar"]').waitFor();assert.equal(await effects(page),0);
},{segmented:true}));
