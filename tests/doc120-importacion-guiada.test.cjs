const test=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs'),path=require('node:path'),vm=require('node:vm');
const {pathToFileURL}=require('node:url'),{chromium}=require('../tools/e2e/node_modules/playwright'),acorn=require('../tools/opsxj/node_modules/acorn');
const root=path.resolve(__dirname,'..'),core=require('../js/gestion/ImportacionCarpetas/ImportacionCarpetas.js');
const mockFile=path.join(root,'Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Prototipo/propuesta-importacion-guiada-doc120.html');
async function withView(work){const browser=await chromium.launch({headless:true});try{const page=await browser.newPage({viewport:{width:1280,height:900}}),errors=[],requests=[];page.setDefaultTimeout(5000);page.on('pageerror',e=>errors.push(e.message));page.on('request',r=>{if(/^https?:/.test(r.url()))requests.push(r.url());});await page.setContent('<html lang="es"><body><button id="before">Abrir</button><div id="guiada-fixture"></div></body></html>');await page.addStyleTag({path:path.join(root,'js/gestion/ImportacionCarpetas/ImportacionCarpetas.css')});await page.addScriptTag({path:path.join(root,'js/gestion/ImportacionCarpetas/ImportacionCarpetasVista.js')});await page.addScriptTag({path:path.join(root,'js/gestion/ImportacionCarpetas/ImportacionCarpetas.js')});await page.evaluate(()=>{document.getElementById('guiada-fixture').id='doc111-importacion';document.getElementById('before').focus();window.calls=[];window.model={etapa:0,contexto:{rutaCanonica:'Destino sintético'},perfiles:[{VersionId:'43',PerfilId:'28',NumeroVersion:1,Configuracion:{NombreVisible:'Egresos',GabineteId:'10'}}],perfil:null,masPerfiles:true,selectorDisponible:true,paginaRaices:1,paginaArchivos:1,manifiesto:{raices:[{idCliente:'r1',nombre:'Egreso'}],archivos:[{idCliente:'a1',nombre:'1.pdf',rutaRelativa:'Egreso/1.pdf',bytes:7}]}};window.view=ImportacionCarpetasVista.crear(document.getElementById('doc111-importacion'),(name,value)=>calls.push({name,value}));view.render(model);});await work(page);assert.deepEqual(errors,[]);assert.deepEqual(requests,[]);}finally{await browser.close();}}

test('DOC120-GUI01: selección reunida conserva inventario, versiones y callbacks; revisión no encadena',async()=>withView(async page=>{
  assert.deepEqual(await page.locator('.doc111-steps .step > span:last-child').allTextContents(),['Seleccionar','Revisar','Seguimiento']);
  assert.equal(await page.getByRole('button',{name:'Preparar revisión',exact:true}).isDisabled(),true);
  await page.getByLabel('Perfil publicado').selectOption('43');assert.deepEqual(await page.evaluate(()=>calls.pop()),{name:'perfil',value:'43'});
  await page.evaluate(()=>{model.perfil=model.perfiles[0];view.render(model);});
  for(const name of ['Más perfiles','Agregar carpeta','Preparar revisión'])await page.getByRole('button',{name,exact:true}).click();
  await page.locator('[data-doc111-panel="inventario-archivos"] > summary').click();
  const removes=page.getByRole('button',{name:'Quitar',exact:true});assert.equal(await removes.count(),2);await removes.nth(0).click();await removes.nth(1).click();
  assert.deepEqual(await page.evaluate(()=>calls.map(c=>c.name)),['mas-perfiles','agregar','preparar','quitar-raiz','quitar-archivo']);
  await page.evaluate(()=>{model.etapa=2;model.confirmable=true;model.plan={planId:'45',resultado:{PuedeConfirmar:true,UnidadesElegibles:['r1'],Incidencias:[],Elementos:[]}};view.render(model);});
  await page.getByRole('button',{name:'Confirmar plan',exact:true}).click();assert.equal(await page.evaluate(()=>calls.at(-1).name),'confirmar');
  await page.keyboard.press('Escape');assert.equal(await page.evaluate(()=>calls.at(-1).name),'cerrar');
}));

async function tracking(page){await page.evaluate(()=>{Object.assign(model,{etapa:3,loteId:'29',estado:{totalRaices:27,totalArchivos:27,totalBytes:'18279',accionesPermitidas:['CONSULTAR_RAICES','SOLICITAR_AVANCE','EXPORTAR_PAGINA']},resultados:{pagina:2,totalFiltrado:27,tamanoPagina:25,elementos:[{recursoId:'78',raizId:'52',nombre:'1.pdf',estadoRecepcion:'RECIBIDO',estado:'NO_INICIADO',tieneIntentoActivo:false,valoresEfectivosFijados:false}]},raices:{pagina:1,totalFiltrado:1,tamanoPagina:25,elementos:[{recursoId:'52',nombre:'Egreso',estado:'EXPEDIENTE_CONFIRMADO',expedienteId:'60',tieneIntentoActivo:false}]},filtrosRaices:{},filtrosArchivos:{},ultimaConsulta:Date.now(),consultasCompletadas:1});model.accionesRecurso=(r,isRoot)=>ImportacionCarpetas.accionesRecurso(model,r,isRoot);view.render(model);});}
test('DOC120-GUI02: seguimiento conserva borradores, paneles, foco, páginas y barreras al destacar una operación',async()=>withView(async page=>{
  await tracking(page);assert.equal(await page.getByRole('button',{name:'Preparar índices 78',exact:true}).count(),1);
  assert.equal(await page.locator('.doc111-actionbar [data-doc111-action="fase"]').count(),1);
  await page.getByRole('button',{name:'Preparar índices 78',exact:true}).click();assert.deepEqual(await page.evaluate(()=>calls.pop()),{name:'fase',value:{tipo:'ARCHIVOS',id:'78',fase:'MATERIALIZAR_VALORES'}});
  const filePanel=page.locator('details[data-doc111-panel="ARCHIVOS"]');await filePanel.locator('summary').click();
  const field=page.getByLabel('Referencia de carpeta',{exact:true});await field.fill('52');await field.focus();await page.evaluate(()=>view.render(model));assert.equal(await field.inputValue(),'52');assert.equal(await field.evaluate(e=>document.activeElement===e),true);assert.equal(await filePanel.evaluate(e=>e.open),true);
  await page.getByRole('button',{name:'Aplicar filtros de archivos',exact:true}).click();assert.deepEqual(await page.evaluate(()=>calls.pop()),{name:'filtrar-resultados',value:{raizId:'52',estado:'',estadoRecepcion:''}});
  await page.evaluate(()=>{model.filtrosArchivos={raizId:'52'};model.busy=true;view.render(model);model.busy=false;view.render(model);});
  assert.equal(await field.inputValue(),'52');assert.match(await filePanel.locator('..').locator('.filters-title').textContent(),/1 filtro activo/);
  await page.getByRole('button',{name:'Exportar esta página',exact:true}).click();assert.equal(await page.evaluate(()=>calls.pop().name),'exportar');
  await page.evaluate(()=>{model.raices.elementos=[];model.filtrosRaices={estado:'PENDIENTE'};view.render(model);});assert.equal(await page.locator('[data-doc111-action="fase"]').count(),0);
  assert.equal(await page.getByRole('button',{name:'Actualizar estado',exact:true}).count(),1);
  for(const width of [390,768,1280]){await page.setViewportSize({width,height:900});assert.equal(await page.evaluate(()=>document.documentElement.scrollWidth<=innerWidth&&document.querySelector('.doc111-dialog').scrollWidth<=document.querySelector('.doc111-dialog').clientWidth),true);}
  await page.evaluate(()=>{model.mutacionIncierta=true;model.raices.elementos=[{recursoId:'52',estado:'EXPEDIENTE_CONFIRMADO'}];view.render(model);});assert.equal(await page.locator('[data-doc111-action="fase"]').count(),0);
}));

test('DOC120-GUI03: maqueta conserva página vacía, capacidad declarada y escape CSV del servidor',()=>{
  const html=fs.readFileSync(mockFile,'utf8'),script=html.match(/<script>([\s\S]*?)<\/script>/)[1],body=acorn.parse(script,{ecmaVersion:'latest'}).body[0].expression.callee.body.body;
  const source=name=>{const fn=body.find(n=>n.type==='FunctionDeclaration'&&n.id.name===name);return script.slice(fn.start,fn.end);};
  const context={state:{filePage:2,rootPage:1},PAGE_SIZE:25};vm.createContext(context);vm.runInContext(source('pageOf')+';'+source('csvCell'),context);
  assert.equal(vm.runInContext("pageOf(Array(25),'files').length",context),0);assert.equal(context.state.filePage,2);context.PAGE_SIZE=2;context.state.filePage=1;assert.equal(vm.runInContext("pageOf(Array(10),'files').length",context),2);
  for(const [input,expected] of [['a"b','"a""b"'],[' =1+1',`"' =1+1"`],['\u200b@SUM(A1)',`"'\u200b@SUM(A1)"`],['\ttext',`"'\ttext"`],['ok','"ok"']]){context.value=input;assert.equal(vm.runInContext('csvCell(value)',context),expected);}
  const model={estado:{accionesPermitidas:['SOLICITAR_AVANCE']},raices:{elementos:[]}},resource={raizId:'52',estado:'NO_INICIADO',estadoRecepcion:'RECIBIDO'};assert.deepEqual(core.accionesRecurso(model,resource,false),[]);
});

test('DOC120-GUI04: maqueta bloquea índices/almacenamiento con padre oculto y descarga trece columnas',async()=>{
  const browser=await chromium.launch({headless:true});try{const page=await browser.newPage();page.setDefaultTimeout(5000);await page.goto(pathToFileURL(mockFile).href);const b=name=>page.getByRole('button',{name,exact:true});
    await page.getByLabel('Perfil de importación',{exact:true}).selectOption('egresos');await b('Agregar carpeta de ejemplo').click();await b('Preparar revisión').click();await b('Confirmar plan').click();await page.locator('#primary-action').click();await page.locator('#primary-action').click();
    await page.locator('[data-panel="roots-filters"] summary').click();await page.getByLabel('Estado de carpeta',{exact:true}).selectOption('PENDIENTE');await b('Aplicar filtros de carpetas').click();assert.equal(await page.getByRole('button',{name:'Preparar índices',exact:true}).count(),0);assert.match(await page.locator('#primary-action').textContent(),/Limpiar filtros/);
    await page.locator('#primary-action').click();await b('Preparar índices').click();await page.getByLabel('Estado de carpeta',{exact:true}).selectOption('PENDIENTE');await b('Aplicar filtros de carpetas').click();assert.equal(await page.getByRole('button',{name:'Almacenar archivo',exact:true}).count(),0);
    const event=page.waitForEvent('download');await b('Exportar esta página').click();const download=await event,stream=await download.createReadStream(),chunks=[];for await(const c of stream)chunks.push(c);const csv=Buffer.concat(chunks).toString('utf8');assert.equal(csv.split('\r\n')[0].split(',').length,13);assert.match(csv,/"recursoId","raizId","idCliente","nombre"/);assert.match(download.suggestedFilename(),/^DEMO-importacion-\d+-archivos-pagina-1.csv$/);
  }finally{await browser.close();}
});
