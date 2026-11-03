const test = require('node:test');
const assert = require('node:assert/strict');
const inventoryApi = require('../js/gestion/ImportacionCarpetas/InventarioImportacion.js');
const operationApi = require('../js/gestion/ImportacionCarpetas/ImportacionCarpetas.js');
const transportFactory = require('../js/gestion/ImportacionCarpetas/ImportacionCarpetasApi.js');
// Metadatos sintéticos locales: no se leen archivos ni se transmite contenido.
const file = (path, size = 7) => ({webkitRelativePath:path, name:path.split('/').pop(), size, lastModified:0});
const result = {planId:'17', revision:2, planHash:'a'.repeat(64), resultado:{PuedeConfirmar:true}};

// Misma infraestructura Chromium local: sin login, servidor o tráfico HTTP.
async function withDoc113Page(work) {
  const fs=require('node:fs'),path=require('node:path');
  const {chromium}=require('../tools/e2e/node_modules/playwright');
  const root=path.resolve(__dirname,'..'),parent=path.join(root,'.tmp');fs.mkdirSync(parent,{recursive:true});
  const temp=fs.mkdtempSync(path.join(parent,'doc113-client-')),folder=path.join(temp,'Raiz');
  fs.mkdirSync(folder);fs.writeFileSync(path.join(folder,'prueba.pdf'),'synthetic UI metadata');
  const browser=await chromium.launch({headless:true});
  try {
    const page=await browser.newPage(),errors=[],network=[];
    page.on('pageerror',e=>errors.push(e.message));page.on('request',r=>{if(/^https?:/.test(r.url()))network.push(r.url());});
    await page.setContent('<!doctype html><html><body><button id="previous">Anterior</button><div id="doc111-importacion" data-api="/synthetic.asmx" data-upload="/synthetic.ashx" data-token="synthetic-only" data-nivel="7"><button type="button" data-doc111-open>Importar carpetas</button><button type="button" data-doc111-resume hidden>Consultar lote</button><span data-doc111-status role="status"></span></div></body></html>');
    await page.evaluate(()=>{
      window.confirm=()=>true;window.doc113Requests=[];window.doc113Held=[];window.doc113Bad='';window.doc113Hold=false;
      const profile={PerfilId:'24',VersionId:'38',NumeroVersion:1,Configuracion:{NombreVisible:'Perfil sintético',GabineteId:10,ModoIntegridad:'PARCIAL_PERMITIDO',AmbitoIntegridad:'NO_APLICA'}};
      window.XMLHttpRequest=class {
        open(_method,url){this.operation=url.split('/').pop();}setRequestHeader(){}
        abort(){this.aborted=true;if(this.onabort)this.onabort();}
        complete(){
          let data;
          if(this.operation==='AbrirImportacion') {
            const p=JSON.parse(JSON.stringify(profile));if(window.doc113Bad==='profile-null')p.Configuracion=null;if(window.doc113Bad==='profile-field')p.Configuracion.NombreVisible={};
            data={nivelRef:'7',rutaCanonica:'Destino sintético',perfiles:[p],paginaPerfiles:{puedeHaberMas:true}};
          }else if(this.operation==='ListarPerfilesImportacion')data={pagina:2,puedeHaberMas:false,perfiles:[null]};
          else {
            data={planId:'1001',revision:1,planHash:'a'.repeat(64),nivelRef:'7',resultado:{PuedeConfirmar:true,UnidadesElegibles:['r1'],Incidencias:[],Elementos:[]}};
            if(window.doc113Bad==='incident-null')data.resultado.Incidencias=[null];
            if(window.doc113Bad==='incident-field')data.resultado.Incidencias=[{Codigo:{},Mensaje:'x'}];
            if(window.doc113Bad==='element-null')data.resultado.Elementos=[null];
            if(window.doc113Bad==='value-null')data.resultado.Elementos=[{Valores:[null]}];
            if(window.doc113Bad==='value-field')data.resultado.Elementos=[{Valores:[{DestinoRef:'X',Diferido:true,PropiedadPendiente:{}}]}];
          }
          this.status=200;this.responseText=JSON.stringify({d:{ok:true,codigo:'OK',mensaje:'Sintético',datos:data,incidencias:[]}});if(this.onload)this.onload();
        }
        send(body){window.doc113Requests.push({operation:this.operation,request:JSON.parse(body).solicitud});if(this.operation==='AbrirImportacion'&&window.doc113Hold)window.doc113Held.push(this);else queueMicrotask(()=>this.complete());}
      };
    });
    for(const name of ['InventarioImportacion.js','ImportacionCarpetasApi.js','ImportacionCarpetasVista.js','ImportacionCarpetas.js'])await page.addScriptTag({path:path.join(root,'js/gestion/ImportacionCarpetas',name)});
    await work(page,folder);assert.deepEqual(errors,[]);assert.deepEqual(network,[]);
  }finally{
    await browser.close();
    if(!path.resolve(temp).startsWith(path.resolve(parent)+path.sep))throw Error('Unexpected fixture path');
    fs.rmSync(temp,{recursive:true,force:true});
  }
}

test('DOC-113: respuestas anidadas inválidas dejan error y revisión utilizable',async()=>{
  await withDoc113Page(async(page,folder)=>{
    for(const bad of ['profile-null','profile-field']){
      await page.evaluate(value=>window.doc113Bad=value,bad);await page.locator('[data-doc111-open]').click();
      await page.waitForFunction(()=>document.querySelector('[data-doc111-status]').textContent.includes('respuesta del servidor'));
      assert.equal(await page.getByRole('dialog').count(),0);
    }
    await page.evaluate(()=>window.doc113Bad='');await page.locator('[data-doc111-open]').click();await page.getByRole('dialog').waitFor();
    await page.getByRole('button',{name:'Más perfiles',exact:true}).click();
    await page.waitForFunction(()=>document.querySelector('.doc111-notice').textContent.includes('respuesta del servidor'));
    assert.equal(await page.getByLabel('Perfil publicado').locator('option').count(),2);
    await page.getByLabel('Perfil publicado').selectOption('38');await page.getByRole('button',{name:'Agregar carpetas',exact:true}).click();
    await page.locator('input[webkitdirectory]').setInputFiles(folder);
    for(const bad of ['incident-null','incident-field','element-null','value-null','value-field']){
      await page.evaluate(value=>window.doc113Bad=value,bad);await page.getByRole('button',{name:'Preparar revisión',exact:true}).click();
      await page.waitForFunction(()=>document.querySelector('.doc111-notice').textContent.includes('respuesta del servidor'));
      assert.equal(await page.getByRole('button',{name:'Confirmar plan',exact:true}).count(),0);
      assert.equal(await page.getByRole('button',{name:'Preparar revisión',exact:true}).isEnabled(),true);
    }
    await page.evaluate(()=>window.doc113Bad='');await page.getByRole('button',{name:'Preparar revisión',exact:true}).click();
    await page.getByRole('button',{name:'Confirmar plan',exact:true}).waitFor();assert.equal(await page.getByRole('button',{name:'Confirmar plan',exact:true}).isEnabled(),true);
    await page.getByRole('button',{name:'Cerrar',exact:true}).click();assert.equal(await page.getByRole('dialog').count(),0);
    assert.equal(await page.evaluate(()=>window.doc113Requests.some(r=>r.operation==='ConfirmarPlan')),false);
  });
});

test('DOC-113: mismo host y retirada durante apertura no duplican ni reviven instancia',async()=>{
  await withDoc113Page(async(page)=>{
    await page.evaluate(()=>{ImportacionCarpetas.inicializar();ImportacionCarpetas.inicializar();});
    await page.locator('[data-doc111-open]').click();await page.getByRole('dialog').waitFor();
    await page.evaluate(()=>ImportacionCarpetas.inicializar());assert.equal(await page.getByRole('dialog').count(),1);
    assert.equal(await page.evaluate(()=>window.doc113Requests.length),1);
    await page.getByRole('button',{name:'Cerrar',exact:true}).click();assert.equal(await page.evaluate(()=>document.activeElement.hasAttribute('data-doc111-open')),true);
    for(const replace of [true,false]){
      await page.evaluate(()=>window.doc113Hold=true);await page.locator('[data-doc111-open]').click();
      await page.waitForFunction(()=>window.doc113Held.length>0);
      const outcome=await page.evaluate(replace=>{
        const host=document.getElementById('doc111-importacion'),next=host.cloneNode(true),old=host.doc111Instancia;
        if(replace)host.replaceWith(next);else host.remove();ImportacionCarpetas.inicializar();
        const held=window.doc113Held.splice(0);held.forEach(xhr=>xhr.complete());
        if(!replace){document.body.appendChild(next);ImportacionCarpetas.inicializar();}
        return {destroyed:!host.doc111Instancia,replaced:old!==next.doc111Instancia,aborted:held.every(xhr=>xhr.aborted)};
      },replace);
      assert.deepEqual(outcome,{destroyed:true,replaced:true,aborted:true});
      await page.waitForFunction(()=>document.querySelector('[data-doc111-open]').disabled===false);
      assert.equal(await page.getByRole('dialog').count(),0);
    }
    await page.evaluate(()=>window.doc113Hold=false);await page.locator('[data-doc111-open]').click();await page.getByRole('dialog').waitFor();
    await page.getByRole('button',{name:'Cerrar',exact:true}).focus();await page.keyboard.press('Escape');assert.equal(await page.getByRole('dialog').count(),0);
  });
});

test('DOC-113: comprobar foco ignora dialogos legacy ocultos anteriores',async()=>{
  const {focusWithinDialog}=require('../tools/e2e/tests/support/doc113-ventana-importacion.cjs');
  await withDoc113Page(async(page)=>{
    await page.evaluate(()=>{
      const legacy=document.createElement('section');legacy.setAttribute('role','dialog');legacy.hidden=true;document.body.prepend(legacy);
    });
    await page.locator('[data-doc111-open]').click();await page.getByRole('dialog').waitFor();
    await page.getByRole('button',{name:'Cerrar',exact:true}).focus();await page.keyboard.press('Shift+Tab');
    assert.equal(await page.evaluate(()=>document.querySelector('[role=dialog]').contains(document.activeElement)),false);
    assert.equal(await focusWithinDialog(page),true);
    await page.keyboard.press('Tab');assert.equal(await focusWithinDialog(page),true);
    await page.locator('#previous').focus();assert.equal(await focusWithinDialog(page),false);
  });
});

test('DOC-111: acciones necesitan facultad servidor, recurso y padre confirmados', () => {
  const model={estado:{accionesPermitidas:['SOLICITAR_AVANCE']},raices:{elementos:[{recursoId:'40',estado:'EXPEDIENTE_CONFIRMADO'}]}};
  const root={recursoId:'41',estado:'PENDIENTE'}, file={recursoId:'30',raizId:'40',estadoRecepcion:'RECIBIDO',estado:'NO_INICIADO',valoresEfectivosFijados:false};
  assert.deepEqual(operationApi.accionesRecurso(model,root,true),['CREAR_EXPEDIENTE']);
  assert.deepEqual(operationApi.accionesRecurso(model,file,false),['MATERIALIZAR_VALORES']);
  file.valoresEfectivosFijados=true;assert.deepEqual(operationApi.accionesRecurso(model,file,false),['ALMACENAR']);
  for(const change of [m=>m.busy=true,m=>m.mutacionIncierta=true,m=>m.estado.accionesPermitidas=[],m=>m.raices.elementos[0].estado='INCIERTO',m=>m.raices.elementos[0].tieneIntentoActivo=true]){
    const copy=structuredClone(model);change(copy);assert.deepEqual(operationApi.accionesRecurso(copy,file,false),[]);
  }
  model.estado.accionesPermitidas=['SOLICITAR_VERIFICACION'];file.estado='INCIERTO';file.intentoActivoId='50';file.tieneIntentoActivo=true;
  assert.deepEqual(operationApi.accionesRecurso(model,file,false),['RECONCILIAR']);file.estado='EN_CURSO';assert.deepEqual(operationApi.accionesRecurso(model,file,false),[]);
});

test('DOC-111: homónimos mantienen identidades y File al quitar y reconstruir filas', () => {
  const inventory = inventoryApi.crear(), a = file('Carpeta/a.pdf'), b = file('Carpeta/a.pdf');
  const r1 = inventory.agregarSeleccion([a]), r2 = inventory.agregarSeleccion([b]);
  const before = inventory.manifiesto();
  assert.notEqual(r1, r2);
  assert.notEqual(before.raices[0].seleccionId, before.raices[1].seleccionId);
  assert.notEqual(before.archivos[0].idCliente, before.archivos[1].idCliente);
  assert.equal(inventory.obtenerFile(before.archivos[0].idCliente), a);
  assert.equal(inventory.obtenerFile(before.archivos[1].idCliente), b);
  before.raices[1].nombre = 'DOM modificado'; before.archivos.length = 0;
  inventory.quitarRaiz(r1);
  assert.deepEqual(inventory.manifiesto().raices.map(r => r.nombre), ['Carpeta']);
  assert.equal(inventory.obtenerFile(inventory.manifiesto().archivos[0].idCliente), b);
  inventory.quitarArchivo(inventory.manifiesto().archivos[0].idCliente);
  assert.deepEqual(inventory.manifiesto(), {raices:[], archivos:[]});
});

function transportFixture() {
  const requests = [];
  class Xhr {
    constructor() { this.upload = {}; this.status = 0; this.headers = {}; requests.push(this); }
    open(method, url) { this.method = method; this.url = url; }
    setRequestHeader(name, value) { this.headers[name] = value; }
    send(body) { this.body = body; }
    abort() { if (this.onabort) this.onabort(); }
    reply(status, data) { this.status = status; this.responseText = typeof data === 'string' ? data : JSON.stringify(data); if (this.onload) this.onload(); }
  }
  class Form { constructor() { this.fields = []; } append(name, value) { this.fields.push([name, value]); } }
  const api = transportFactory({XMLHttpRequest:Xhr, FormData:Form}).crear({url:'/app/control.asmx', uploadUrl:'/app/upload.ashx', token:'synthetic-only'});
  const ok = datos => ({ok:true, codigo:'OK', mensaje:'OK', incidencias:[], datos});
  return {api, requests, ok};
}

test('DOC-111: fases, detención y reconciliación fallan sin reenvío del transporte', async () => {
  for(const operation of ['AvanzarLote','SolicitarDetencion','ReconciliarRecurso']){
    const f=transportFixture(), pending=f.api.invocar(operation,{loteId:'20'});
    const rejected=assert.rejects(pending,e=>e.resultadoIncierto===true);f.requests[0].ontimeout();await rejected;assert.equal(f.requests.length,1);
  }
});

test('DOC-111: transporte procesa ASMX/ASHX y finaliza HTML, timeout o aborto sin replay', async () => {
  for (const failure of ['html', 'timeout', 'network', 'abort', 'server']) {
    const f = transportFixture(), pending = f.api.invocar('ConfirmarPlan', {planId:'17'});
    const rejected = assert.rejects(pending, error => error.resultadoIncierto === true);
    const xhr = f.requests[0];
    if (failure === 'html') xhr.reply(200, '<html>login</html>');
    if (failure === 'timeout') xhr.ontimeout();
    if (failure === 'network') xhr.onerror();
    if (failure === 'abort') pending.cancelar();
    if (failure === 'server') xhr.reply(503, {ok:false, codigo:'CAPACIDAD_NO_DISPONIBLE', mensaje:'No disponible', incidencias:[]});
    await rejected;
    assert.equal(f.requests.length, 1);
    assert.equal(xhr.onload, null);
    assert.equal(xhr.upload.onprogress, null);
  }
  const f = transportFixture(), pending = f.api.invocar('ConsultarLote', {loteId:'20'});
  assert.equal(f.requests[0].headers['X-Importacion-Carpetas-CSRF'], 'synthetic-only');
  assert.deepEqual(JSON.parse(f.requests[0].body), {solicitud:{loteId:'20'}});
  f.requests[0].reply(200, {d:f.ok({loteId:'20'})});
  assert.deepEqual(await pending, {loteId:'20'});
  await assert.rejects(f.api.invocar('Almacenar', {}), /SOLICITUD_INVALIDA/);
  assert.equal(f.requests.length, 1);
  const denied = f.api.invocar('ConsultarLote', {loteId:'20'}), caught = assert.rejects(denied);
  f.requests[1].reply(401, '<html>login</html>'); await caught;
  await assert.rejects(f.api.invocar('ConsultarLote', {loteId:'20'}), /SESION_REQUERIDA/);
  assert.equal(f.requests.length, 2);
});

test('DOC-111: cola transfiere un archivo a la vez y se detiene ante incertidumbre', async () => {
  const f = transportFixture(), progress = [], files = [file('A/a.pdf'), file('A/b.pdf'), file('A/c.pdf')];
  const items = files.map((value, i) => ({file:value, solicitud:{loteId:'20', archivoId:String(i + 30), claveIntento:'explicit_' + i}}));
  const pending = f.api.enviarCola(items, (item, event) => progress.push([item, event]));
  const caught = assert.rejects(pending, error => error.resultadoIncierto === true);
  await new Promise(resolve => setImmediate(resolve));
  assert.equal(f.requests.length, 1);
  const first = f.requests[0];
  assert.deepEqual(first.body.fields.map(pair => pair[0]), ['loteId', 'archivoId', 'claveIntento', 'file']);
  assert.equal(first.body.fields[3][1], files[0]);
  assert.equal(first.headers['Content-Type'], undefined);
  first.upload.onprogress({lengthComputable:true, loaded:80, total:100});
  assert.deepEqual(progress[0][1], {enviados:80, total:100, fase:'TRANSFERENCIA'});
  first.reply(200, f.ok({loteId:'20', archivoId:'30', estadoRecepcion:'RECIBIDO', estadoEfecto:'NO_INICIADO'}));
  await new Promise(resolve => setImmediate(resolve));
  assert.equal(f.requests.length, 2);
  f.requests[1].ontimeout(); await caught;
  assert.equal(f.requests.length, 2);
  const untouched = transportFixture(), stopped = untouched.api.enviarCola(items);
  stopped.cancelar(); await assert.rejects(stopped, /SOLICITUD_CANCELADA/);
  assert.equal(untouched.requests.length, 0);
  untouched.api.cerrar(); await assert.rejects(untouched.api.recibir(items[0].solicitud, files[0]), /SESION_REQUERIDA/);
});

test('DOC-111: cancelación y selección inválida no alteran inventario ni emiten cambio', () => {
  const inventory = inventoryApi.crear(); let changes = 0;
  inventory.alCambiar(() => changes++);
  inventory.agregarSeleccion([file('Original/a.pdf')]);
  const before = inventory.manifiesto();
  assert.equal(inventory.agregarSeleccion([]), null);
  for (const selection of [
    [file('Uno/a.pdf'), file('Dos/b.pdf')], [file('Suelto.pdf')], [file('/A/a.pdf')],
    [file('A/../a.pdf')], [file('A/./a.pdf')], [file('A//a.pdf')], [file('C:/a.pdf')],
    [file('A/a.pdf'), file('A/a.pdf')], [file('A/é.pdf'), file('A/e\u0301.pdf')],
    [file('A/a.pdf', 9007199254740992)]
  ]) { assert.throws(() => inventory.agregarSeleccion(selection)); }
  assert.deepEqual(inventory.manifiesto(), before);
  assert.equal(changes, 1);
});

test('DOC-111: cada apertura agrega una raíz, permite repetir y libera el selector', () => {
  const inventory = inventoryApi.crear(), handlers = {}, failures = [];
  const input = {webkitdirectory:false, value:'', files:[], clicks:0,
    setAttribute() {}, addEventListener(name, fn) { handlers[name] = fn; },
    removeEventListener(name, fn) { assert.equal(handlers[name], fn); delete handlers[name]; },
    click() { assert.equal(this.value, ''); this.clicks++; }};
  assert.throws(() => inventoryApi.vincularSelector({}, inventory), /SELECCION_DIRECTORIOS_NO_DISPONIBLE/);
  const picker = inventoryApi.vincularSelector(input, inventory, error => failures.push(error.codigo));
  for (let i = 0; i < 2; i++) {
    input.value = 'selección anterior'; picker.abrir(); input.files = [file('A/a.pdf')]; handlers.change();
    assert.equal(input.value, '');
  }
  assert.equal(inventory.manifiesto().raices.length, 2);
  input.files = [file('suelto.pdf')]; handlers.change();
  assert.equal(input.value, ''); assert.equal(failures.length, 1);
  assert.equal(inventory.manifiesto().raices.length, 2);
  picker.destruir(); assert.equal(handlers.change, undefined);
});

test('DOC-111: cambio de inventario o perfil descarta plan y respuestas tardías', () => {
  const inventory = inventoryApi.crear(), operation = operationApi.crear(inventory);
  inventory.agregarSeleccion([file('A/a.pdf')]); operation.seleccionarPerfil('38');
  const ticket = operation.iniciarPreparacion();
  assert.equal(operation.registrarPreparacion(ticket, result), true);
  const reference = operation.referenciaConfirmacion(); reference.planId = '999';
  assert.equal(operation.referenciaConfirmacion().planId, '17');
  inventory.agregarSeleccion([file('A/a.pdf')]);
  assert.throws(() => operation.referenciaConfirmacion(), /PREPARACION_REQUERIDA/);
  assert.equal(operation.registrarPreparacion(ticket, result), false);
  const next = operation.iniciarPreparacion(); operation.seleccionarPerfil('37');
  assert.equal(operation.registrarPreparacion(next, result), false);
  const stale = operation.iniciarPreparacion(), current = operation.iniciarPreparacion();
  assert.equal(operation.registrarPreparacion(stale, result), false);
  assert.equal(operation.registrarPreparacion(current, result), true);
  inventory.quitarArchivo(inventory.manifiesto().archivos[0].idCliente);
  assert.throws(() => operation.referenciaConfirmacion(), /PREPARACION_REQUERIDA/);
  const denied = operation.iniciarPreparacion();
  assert.equal(operation.registrarPreparacion(denied, {...result, resultado:{PuedeConfirmar:false}}), false);
  operation.destruir(); assert.equal(operation.registrarPreparacion(denied, result), false);
});

test('DOC-111: referencias tras recarga guardan solo IDs y quedan separadas por contexto', () => {
  const values = new Map(), storage = {getItem:k=>values.get(k) || null, setItem:(k,v)=>values.set(k,v), removeItem:k=>values.delete(k)};
  const scope = 'a'.repeat(32), memory = operationApi.crearReferencias(storage, scope);
  assert.equal(memory.guardar({loteId:'20', planId:'17', token:'never', claveConfirmacion:'never', file:file('A/a.pdf'), ruta:'never'}), true);
  assert.deepEqual(JSON.parse(values.get('DOC111_REF_'+scope)), {v:1,loteId:'20',planId:'17'});
  assert.deepEqual(operationApi.crearReferencias(storage, scope).leer(), {v:1,loteId:'20',planId:'17'});
  assert.deepEqual(operationApi.crearReferencias(storage, 'b'.repeat(32)).leer(), {});
  values.set('DOC111_REF_'+scope, '{"v":1,"loteId":"20","token":"injected"}'); assert.deepEqual(memory.leer(), {});
  for (const bad of ['020','0','-1','4294967296','1e3']) { memory.guardar({loteId:bad}); assert.deepEqual(memory.leer(), {v:1}); }
  const blocked = operationApi.crearReferencias({getItem(){throw new Error();},setItem(){throw new Error();},removeItem(){throw new Error();}}, scope);
  assert.deepEqual(blocked.leer(), {}); assert.equal(blocked.guardar({loteId:'20'}), false); assert.equal(blocked.disponible(), false);
  assert.equal(operationApi.crearReferencias(storage, null).guardar({loteId:'20'}), false);
});

test('DOC-111: reselección exige archivo pendiente explícito y no reemplaza recibidos o inciertos', () => {
  const expected = {recursoId:'30',raizId:'40',nombre:'a.pdf',bytesPrevistos:'7',ultimaModificacionLocal:'0',estadoRecepcion:'PENDIENTE',estado:'NO_INICIADO',tieneIntentoActivo:false};
  const selected = file('A/a.pdf'); assert.equal(operationApi.validarReseleccion(expected, selected), selected);
  for (const change of [{nombre:'b.pdf'},{bytesPrevistos:'8'},{ultimaModificacionLocal:'1'},{estadoRecepcion:'RECIBIDO'},{estadoRecepcion:'INCIERTO'},{estadoRecepcion:'RECIBIENDO'},{estado:'COMPLETADO'},{tieneIntentoActivo:true}]) {
    assert.throws(()=>operationApi.validarReseleccion({...expected,...change},selected), /RESELECCION_NO_COINCIDE/);
  }
  // Dos homonimos pueden tener metadatos iguales: el consumidor debe vincular
  // cada seleccion al recurso elegido, nunca buscar una identidad por nombre.
  assert.equal(operationApi.validarReseleccion({...expected,recursoId:'31',raizId:'41'},selected), selected);
});

test('DOC-111: interfaz Chromium local con servicios sintéticos, foco y cierre sin duplicados', async () => {
  // Navegador sin autenticacion, servidor, BD ni trafico HTTP. Reutiliza el
  // Playwright ya instalado; no crea otra configuracion/login/runner E2E.
  const fs = require('node:fs'), path = require('node:path'), os = require('node:os');
  const {chromium} = require('../tools/e2e/node_modules/playwright');
  const root = path.resolve(__dirname, '..'), parent = path.resolve(os.tmpdir());
  const folder = fs.mkdtempSync(path.join(parent, 'doc111-ui-')), selected = path.join(folder, 'Carpeta');
  fs.mkdirSync(selected); fs.writeFileSync(path.join(selected, 'a.pdf'), 'fixture');
  fs.mkdirSync(path.join(root, '.tmp'), {recursive:true});
  const browser = await chromium.launch({headless:true});
  try {
    const page = await browser.newPage({viewport:{width:1280,height:800}}), errors = [], network = [];
    page.on('pageerror', error => errors.push(error.message));
    page.on('request', request => { if (/^https?:/.test(request.url())) network.push(request.url()); });
    await page.setContent('<!doctype html><html lang="es"><head><meta charset="utf-8"></head><body><div id="doc111-importacion" data-api="/control.asmx" data-upload="/upload.ashx" data-token="synthetic-only" data-nivel=""><button type="button" data-doc111-open disabled>Importar carpetas</button><button type="button" data-doc111-resume hidden>Consultar último lote</button><span data-doc111-status role="status"></span></div></body></html>');
    await page.addStyleTag({path:path.join(root, 'js/gestion/ImportacionCarpetas/ImportacionCarpetas.css')});
    await page.evaluate(() => {
      window.confirm = () => true;
      window.doc111Requests = []; window.lostConfirmation = false; window.denyOpening = false; window.doc111Received = false;
      const profile = {PerfilId:'4', VersionId:'5', NumeroVersion:1, Configuracion:{NombreVisible:'Perfil de prueba', GabineteId:7, ModoIntegridad:'PARCIAL_PERMITIDO', AmbitoIntegridad:'NO_APLICA', FondoRef:'1', SerieRef:'4'}};
      let manifest = null;
      window.XMLHttpRequest = class {
        constructor() { this.upload = {}; this.status = 0; }
        open(method, url) { this.url = url; }
        setRequestHeader() {}
        abort() { this.aborted = true; if (this.onabort) this.onabort(); }
        send(body) {
          const operation = this.url.split('/').pop(), request = operation === 'upload.ashx' ? Object.fromEntries(body.entries()) : JSON.parse(body).solicitud;
          window.doc111Requests.push({operation, request});
          setTimeout(() => {
            if (this.aborted) return;
            let data;
            if (operation === 'AbrirImportacion') {
              if (window.denyOpening) { this.status = 403; this.responseText = JSON.stringify({ok:false, codigo:'ACCESO_DENEGADO', mensaje:'Denegado', incidencias:[]}); this.onload(); return; }
              data = {nivelRef:request.nivelRef, rutaCanonica:'Compras / Órdenes', perfiles:[profile], paginaPerfiles:{puedeHaberMas:false}};
            } else if (operation === 'PrepararPlan') {
              manifest = request;
              data = {planId:'17', nivelRef:request.nivelRef, revision:2, planHash:'a'.repeat(64), resultado:{PuedeConfirmar:true, UnidadesElegibles:[request.raices[0].idCliente],
                Incidencias:[{RaizIdCliente:request.raices[1].idCliente, ArchivoIdCliente:request.archivos[1].idCliente, Campo:'TIPO', Codigo:'RECHAZADO', Mensaje:'<img src=x onerror=alert(1)>', BloqueaUnidad:true}],
                Elementos:request.archivos.map(f => ({RaizIdCliente:f.raizIdCliente, ArchivoIdCliente:f.idCliente, Valores:[{DestinoRef:'EXPEDIENTE', Diferido:true, PropiedadPendiente:'ID'}]}))}};
            } else if (operation === 'ConfirmarPlan') {
              if (window.lostConfirmation) { this.ontimeout(); return; }
              data = {loteId:'20', planId:'17', nivelRef:manifest.nivelRef, estado:'RECIBIENDO', revision:1};
            } else if (operation === 'ConsultarLote') {
              data = {loteId:'20', nivelRef:manifest.nivelRef, rutaCanonica:'Compras / Órdenes', estado:'RECIBIENDO', maximoTamanoPagina:25, totalRaices:2, totalArchivos:2, totalBytes:'14', accionesPermitidas:['CONSULTAR','EXPORTAR_PAGINA']};
            } else if (operation === 'ConsultarRecursosLote') {
              data = {loteId:'20', pagina:request.pagina, tamanoPagina:request.tamanoPagina, totalOriginal:2, totalFiltrado:2, elementos:manifest.archivos.map((f, i) => ({recursoId:String(30+i), raizId:String(40+i), idCliente:f.idCliente, nombre:f.nombre, bytesPrevistos:f.bytes, estadoRecepcion:i===1?'RECHAZADO':window.doc111Received?'RECIBIDO':'PENDIENTE', estado:i===1?'RECHAZADO':'NO_INICIADO'}))};
            } else if (operation === 'upload.ashx') {
              if (this.upload.onprogress) this.upload.onprogress({lengthComputable:true, loaded:80, total:100});
              window.doc111Received = true;
              data = {loteId:request.loteId, archivoId:request.archivoId, estadoRecepcion:'RECIBIDO', estadoEfecto:'NO_INICIADO'};
            } else if (operation === 'ExportarPaginaLote') {
              data = {contenido:'"nombre"\r\n"\'=1+1"\r\n'};
            } else { throw new Error('Operacion sintetica inesperada'); }
            this.status = 200; this.responseText = JSON.stringify({d:{ok:true, codigo:'OK', mensaje:'OK', incidencias:[], datos:data}}); this.onload();
          }, 0);
        }
      };
    });
    for (const name of ['InventarioImportacion', 'ImportacionCarpetasApi', 'ImportacionCarpetasVista', 'ImportacionCarpetas']) await page.addScriptTag({path:path.join(root, `js/gestion/ImportacionCarpetas/${name}.js`)});
    assert.equal(await page.locator('[data-doc111-open]').isDisabled(), true);
    await page.evaluate(() => { document.getElementById('doc111-importacion').setAttribute('data-nivel','7'); ImportacionCarpetas.inicializar(); });
    await page.locator('[data-doc111-open]').click(); await page.getByRole('dialog').waitFor();
    await page.getByLabel('Perfil publicado').selectOption('5');
    await page.getByRole('button', {name:'Agregar carpetas', exact:true}).click();
    for (let i=0;i<2;i++) await page.locator('input[type=file][webkitdirectory]').setInputFiles(selected);
    await page.getByRole('button', {name:'Preparar revisión', exact:true}).click();
    await page.getByRole('button', {name:'Confirmar plan', exact:true}).waitFor();
    assert.equal(await page.getByRole('button', {name:'Confirmar plan', exact:true}).isEnabled(), true);
    assert.equal(await page.locator('.doc111-content img').count(), 0);
    assert.match(await page.locator('.doc111-content').innerText(), /Unidades elegibles: 1 de 2/);
    assert.match(await page.locator('.doc111-content').innerText(), /Pendiente: ID/);
    await page.screenshot({path:path.join(root,'.tmp/doc111-ui-revision-desktop.png')});
    await page.setViewportSize({width:390,height:844});
    assert.equal(await page.evaluate(() => document.documentElement.scrollWidth <= innerWidth), true);
    assert.equal(await page.locator('[role=dialog]').evaluate(n => n.scrollWidth <= n.clientWidth), true);
    await page.screenshot({path:path.join(root,'.tmp/doc111-ui-revision-movil.png')});
    await page.evaluate(() => { document.getElementById('doc111-importacion').setAttribute('data-nivel','8'); ImportacionCarpetas.inicializar(); ImportacionCarpetas.inicializar(); });
    assert.match(await page.locator('.doc111-destino').innerText(), /Compras \/ Órdenes/);
    await page.getByRole('button', {name:'Confirmar plan', exact:true}).click();
    await page.getByRole('button', {name:'Enviar pendientes de esta página', exact:true}).waitFor();
    await page.getByRole('button', {name:'Enviar pendientes de esta página', exact:true}).click();
    await page.waitForFunction(() => window.doc111Received && document.querySelector('.doc111-content').textContent.includes('RECIBIDO'));
    assert.match(await page.locator('.doc111-content').innerText(), /NO_INICIADO/);
    assert.match(await page.locator('.doc111-content').innerText(), /RECHAZADO/);
    const calls = await page.evaluate(() => window.doc111Requests.map(r => ({operation:r.operation, request:r.operation==='upload.ashx'?{loteId:r.request.loteId,archivoId:r.request.archivoId}:r.request})));
    assert.equal(calls.filter(r => r.operation==='ConfirmarPlan').length, 1);
    assert.equal(calls.filter(r => r.operation==='upload.ashx').length, 1);
    assert.equal(calls.find(r => r.operation==='PrepararPlan').request.nivelRef, '7');
    assert.deepEqual(Object.keys(calls.find(r => r.operation==='ConfirmarPlan').request).sort(), ['claveConfirmacion','planHashEsperado','planId','revisionEsperada']);
    const download = page.waitForEvent('download'); await page.getByRole('button', {name:'Exportar esta página'}).click();
    assert.equal((await download).suggestedFilename(), 'importacion-20-pagina-1.csv');
    await page.getByRole('button', {name:'Cerrar', exact:true}).click();
    assert.equal(await page.getByRole('dialog').count(), 0);
    await page.locator('[data-doc111-resume]').click(); await page.getByRole('dialog').waitFor();
    await page.waitForFunction(() => document.querySelector('.doc111-content').textContent.includes('Resultados persistidos'));
    assert.equal(await page.getByRole('button', {name:'Enviar pendientes de esta página', exact:true}).isDisabled(), true);
    await page.getByRole('button', {name:'Cerrar', exact:true}).focus(); await page.keyboard.press('Shift+Tab');
    assert.equal(await page.evaluate(() => document.querySelector('[role=dialog]').contains(document.activeElement)), true);
    await page.keyboard.press('Escape'); assert.equal(await page.getByRole('dialog').count(), 0);
    // Apertura denegada no deja una ventana abierta ni una espera bloqueada.
    await page.evaluate(() => { window.denyOpening=true; }); await page.locator('[data-doc111-open]').click();
    await page.waitForFunction(() => document.querySelector('[data-doc111-status]').textContent.includes('autorización'));
    assert.equal(await page.getByRole('dialog').count(), 0);
    await page.evaluate(() => { window.denyOpening=false; window.lostConfirmation=true; });
    await page.locator('[data-doc111-open]').click(); await page.getByRole('dialog').waitFor();
    await page.getByLabel('Perfil publicado').selectOption('5'); await page.getByRole('button', {name:'Agregar carpetas', exact:true}).click();
    for (let i=0;i<2;i++) await page.locator('input[type=file][webkitdirectory]').setInputFiles(selected);
    await page.getByRole('button', {name:'Preparar revisión', exact:true}).click(); await page.getByRole('button', {name:'Confirmar plan', exact:true}).click();
    await page.waitForFunction(() => document.querySelector('.doc111-notice').textContent.includes('No se pudo confirmar'));
    assert.equal(await page.getByRole('button', {name:'Confirmar plan', exact:true}).isDisabled(), true);
    assert.equal(await page.evaluate(() => window.doc111Requests.filter(r=>r.operation==='ConfirmarPlan').length), 2);
    await page.getByRole('button', {name:'Cerrar', exact:true}).click();
    assert.equal(await page.locator('[data-doc111-open]').isDisabled(), true);
    assert.equal(await page.evaluate(() => document.getElementById('doc111-importacion').doc111ConfirmacionPendiente.planId), '17');
    assert.equal(await page.evaluate(() => document.body.style.overflow), '');
    assert.deepEqual(errors, []); assert.deepEqual(network, []);
    await page.close();
  } finally {
    await browser.close(); assert.equal(path.dirname(folder), parent); assert.ok(path.basename(folder).startsWith('doc111-ui-')); fs.rmSync(folder, {recursive:true,force:true});
  }
});

test('DOC-111: recarga real conserva referencias y recupera sin reconfirmar ni asociar homónimos', async () => {
  const fs = require('node:fs'), path = require('node:path'), os = require('node:os');
  const {chromium} = require('../tools/e2e/node_modules/playwright');
  const root = path.resolve(__dirname, '..'), parent = path.resolve(os.tmpdir());
  const folder = fs.mkdtempSync(path.join(parent, 'doc111-reload-')), selected = path.join(folder,'Carpeta');
  fs.mkdirSync(selected); fs.writeFileSync(path.join(selected,'a.pdf'), 'fixture');
  const browser = await chromium.launch({headless:true});
  try {
    const page = await browser.newPage(), calls = [], errors = [], exterior = []; page.setDefaultTimeout(5000);
    let manifest, visible = false, received = false, scope = 'a'.repeat(32);
    const profile = {PerfilId:'4',VersionId:'5',NumeroVersion:1,Configuracion:{NombreVisible:'Perfil sintético',GabineteId:7,ModoIntegridad:'PARCIAL_PERMITIDO',AmbitoIntegridad:'NO_APLICA'}};
    await page.exposeFunction('__doc111Server', (operation, request) => {
      calls.push({operation,request}); let data;
      if (operation==='AbrirImportacion') data={nivelRef:request.nivelRef,rutaCanonica:'Destino original',perfiles:[profile]};
      else if (operation==='PrepararPlan') {
        manifest=request; data={planId:'17',nivelRef:manifest.nivelRef,revision:2,planHash:'a'.repeat(64),resultado:{PuedeConfirmar:true,UnidadesElegibles:manifest.raices.map(r=>r.idCliente),Incidencias:[],Elementos:[]}};
      } else if (operation==='ConfirmarPlan') return {timeout:true}; // Respuesta perdida; consulta posterior puede encontrar el lote.
      else if (operation==='ConsultarConfirmacionLote') data={planId:request.planId,confirmada:visible,loteId:visible?'20':null,nivelRef:manifest.nivelRef};
      else if (operation==='ConsultarLote') data={loteId:'20',nivelRef:manifest.nivelRef,rutaCanonica:'Destino original',estado:'RECIBIENDO',maximoTamanoPagina:25,totalRaices:2,totalArchivos:2,totalBytes:'14',accionesPermitidas:['CONSULTAR','EXPORTAR_PAGINA']};
      else if (operation==='ConsultarRecursosLote') data={loteId:'20',pagina:1,tamanoPagina:request.tamanoPagina,totalOriginal:2,totalFiltrado:2,elementos:manifest.archivos.map((f,i)=>({recursoId:String(30+i),raizId:String(40+i),idCliente:f.idCliente,nombre:f.nombre,rutaRelativa:f.rutaRelativa,bytesPrevistos:f.bytes,ultimaModificacionLocal:f.ultimaModificacionLocal,tieneIntentoActivo:false,estado:'NO_INICIADO',estadoRecepcion:i===0||received?'RECIBIDO':'PENDIENTE'}))};
      else if (operation==='upload.ashx') { received=true; data={loteId:request.loteId,archivoId:request.archivoId,estadoRecepcion:'RECIBIDO',estadoEfecto:'NO_INICIADO'}; }
      else throw new Error('Operacion inesperada en fixture de recuperacion');
      return {ok:true,codigo:'OK',mensaje:'OK',incidencias:[],datos:data};
    });
    await page.addInitScript(() => {
      window.confirm=()=>true;
      window.XMLHttpRequest=class {
        constructor(){this.upload={};this.status=0;}
        open(method,url){this.url=url;}
        setRequestHeader(){}
        abort(){this.aborted=true;if(this.onabort)this.onabort();}
        send(body){
          const operation=this.url.split('/').pop();let request;
          if(operation==='upload.ashx')request={loteId:body.get('loteId'),archivoId:body.get('archivoId'),claveIntento:body.get('claveIntento'),file:{name:body.get('file').name,size:body.get('file').size}};
          else request=JSON.parse(body).solicitud;
          window.__doc111Server(operation,request).then(response=>{if(this.aborted)return;if(response.timeout){this.ontimeout();return;}this.status=200;this.responseText=JSON.stringify({d:response});this.onload();});
        }
      };
    });
    const assets=['InventarioImportacion','ImportacionCarpetasApi','ImportacionCarpetasVista','ImportacionCarpetas'].map(name=>'<script>'+fs.readFileSync(path.join(root,`js/gestion/ImportacionCarpetas/${name}.js`),'utf8').replace(/<\/script/gi,'<\\/script')+'</script>').join('');
    // Todas las navegaciones se sirven por route.fulfill: no hay servidor ni
    // HTTP exterior; el origen sintético permite sessionStorage nativo/recarga.
    await page.route('**/*',route=>{
      if(route.request().url()!=='https://doc111.invalid/')exterior.push(route.request().url());
      route.fulfill({contentType:'text/html; charset=utf-8',body:'<!doctype html><html><head><style>'+fs.readFileSync(path.join(root,'js/gestion/ImportacionCarpetas/ImportacionCarpetas.css'),'utf8')+'</style></head><body><div id="doc111-importacion" data-api="/control.asmx" data-upload="/upload.ashx" data-token="synthetic-only" data-recuperacion="'+scope+'" data-nivel="7"><button type="button" data-doc111-open>Importar carpetas</button><button type="button" data-doc111-resume hidden>Consultar último lote</button><button type="button" data-doc111-verificar hidden>Verificar confirmación pendiente</button><label>Referencia del lote <input data-doc111-lote /></label><button type="button" data-doc111-consultar>Consultar lote</button><span data-doc111-status role="status"></span></div>'+assets+'</body></html>'});
    });
    page.on('pageerror',error=>errors.push(error.message));
    await page.goto('https://doc111.invalid/');
    await page.locator('[data-doc111-open]').click();await page.getByRole('dialog').waitFor();await page.getByLabel('Perfil publicado').selectOption('5');await page.getByRole('button',{name:'Agregar carpetas',exact:true}).click();
    for(let i=0;i<2;i++)await page.locator('input[webkitdirectory]').setInputFiles(selected);
    assert.deepEqual(errors,[]);assert.match(await page.locator('.doc111-content').innerText(),/Preparar revisión/);
    await page.getByRole('button',{name:'Preparar revisión'}).click();await page.getByRole('button',{name:'Confirmar plan',exact:true}).click();
    await page.waitForFunction(()=>document.querySelector('.doc111-notice').textContent.includes('No se pudo confirmar'));
    assert.deepEqual(await page.evaluate(()=>JSON.parse(sessionStorage.getItem('DOC111_REF_'+'a'.repeat(32)))),{v:1,planId:'17'});
    await page.reload();assert.equal(await page.locator('[data-doc111-open]').isDisabled(),true);
    await page.locator('[data-doc111-verificar]').click();
    await page.waitForFunction(()=>document.querySelector('[data-doc111-status]').textContent.includes('Todavía no hay'));
    assert.equal(calls.filter(c=>c.operation==='ConfirmarPlan').length,1);
    assert.equal(await page.locator('[data-doc111-open]').isDisabled(),true);
    visible=true;await page.evaluate(()=>{document.getElementById('doc111-importacion').setAttribute('data-nivel','8');ImportacionCarpetas.inicializar();});
    await page.locator('[data-doc111-verificar]').click();await page.getByRole('dialog').waitFor();
    assert.match(await page.locator('.doc111-destino').innerText(),/Destino original/);
    assert.equal(await page.getByRole('button',{name:'Enviar pendientes de esta página'}).isDisabled(),true);
    assert.equal(await page.getByRole('button',{name:'Reseleccionar archivo 30',exact:true}).count(),0);
    await page.setViewportSize({width:390,height:844});
    assert.equal(await page.evaluate(()=>document.documentElement.scrollWidth<=innerWidth),true);
    fs.mkdirSync(path.join(root,'.tmp'),{recursive:true});await page.screenshot({path:path.join(root,'.tmp/doc111-recuperacion-movil.png')});
    await page.setViewportSize({width:1280,height:800});
    const badChoice=page.waitForEvent('filechooser');await page.getByRole('button',{name:'Reseleccionar archivo 31',exact:true}).click();await(await badChoice).setFiles({name:'otro.pdf',mimeType:'application/pdf',buffer:Buffer.from('fixture')});
    await page.waitForFunction(()=>document.querySelector('.doc111-notice').textContent.includes('no coincide'));
    assert.equal(await page.getByRole('button',{name:'Enviar pendientes de esta página'}).isDisabled(),true);
    const goodChoice=page.waitForEvent('filechooser');await page.getByRole('button',{name:'Reseleccionar archivo 31',exact:true}).click();await(await goodChoice).setFiles(path.join(selected,'a.pdf'));
    await page.waitForFunction(()=>document.querySelector('.doc111-notice').textContent.includes('Archivo vinculado'));
    await page.getByRole('button',{name:'Enviar pendientes de esta página'}).click();
    await page.waitForFunction(()=>document.querySelector('[data-doc111-action=enviar]').disabled);
    assert.deepEqual(calls.filter(c=>c.operation==='upload.ashx').map(c=>c.request.archivoId),['31']);
    assert.equal(calls.filter(c=>c.operation==='ConfirmarPlan').length,1);
    assert.deepEqual(await page.evaluate(()=>JSON.parse(sessionStorage.getItem('DOC111_REF_'+'a'.repeat(32)))),{v:1,loteId:'20'});
    await page.reload();await page.locator('[data-doc111-resume]').click();await page.getByRole('dialog').waitFor();
    assert.equal(await page.getByRole('button',{name:'Enviar pendientes de esta página'}).isDisabled(),true);
    assert.equal(calls.filter(c=>c.operation==='upload.ashx').length,1);
    scope='b'.repeat(32);await page.reload();assert.equal(await page.locator('[data-doc111-resume]').isHidden(),true);assert.equal(await page.locator('[data-doc111-verificar]').isHidden(),true);
    assert.deepEqual(errors,[]);assert.deepEqual(exterior,[]);
  }finally{await browser.close();assert.equal(path.dirname(folder),parent);assert.ok(path.basename(folder).startsWith('doc111-reload-'));fs.rmSync(folder,{recursive:true,force:true});}
});

test('DOC-111: fases explícitas y rama original usan relaciones confirmadas sin mover selección', async () => {
  const path=require('node:path'),{chromium}=require('../tools/e2e/node_modules/playwright'),root=path.resolve(__dirname,'..');
  const browser=await chromium.launch({headless:true});
  try{
    const page=await browser.newPage(),errors=[];page.setDefaultTimeout(5000);page.on('pageerror',e=>errors.push(e.message));
    await page.setContent(`<!doctype html><html lang="es"><body>
      <div id="TreeViewArchivo"><table><tr><td></td><td></td><td><a id="TreeViewArchivot1" title="7\\\\Origen">Origen</a></td></tr></table><div id="TreeViewArchivon1Nodes" style="display:none"></div>
      <table><tr><td></td><td></td><td><a id="TreeViewArchivot2" title="8\\\\Otra seleccion">Otra selección</a></td></tr></table><div id="TreeViewArchivon2Nodes"></div></div><div id="pit"></div>
      <div id="doc111-importacion" data-api="/control.asmx" data-upload="/upload.ashx" data-token="synthetic-only" data-recuperacion="${'a'.repeat(32)}" data-nivel="8"><button type="button" data-doc111-open>Importar carpetas</button><button type="button" data-doc111-resume>Consultar último lote</button><span data-doc111-status></span></div></body></html>`);
    await page.addStyleTag({path:path.join(root,'js/gestion/ImportacionCarpetas/ImportacionCarpetas.css')});
    await page.evaluate(()=>{
      window.$=()=>({ready(){},on(){},autocomplete(){}});window.alert=message=>{throw new Error(message);};window.confirm=()=>true;
      document.getElementById('doc111-importacion').doc111UltimoLote={loteId:'20'};
      window.phaseCalls=[];window.rootState='PENDIENTE';window.fixedValues=false;window.documentState='NO_INICIADO';window.lotRevision=1;window.stopped=false;
      window.XMLHttpRequest=class{
        constructor(){this.upload={};}open(method,url){this.url=url;}setRequestHeader(){}abort(){if(this.onabort)this.onabort();}
        send(body){
          const operation=this.url.split('/').pop(),request=JSON.parse(body).solicitud;window.phaseCalls.push({operation,request});
          setTimeout(()=>{
            if(!this.onload)return;let data;
            const uncertain=window.documentState==='INCIERTO';
            if(operation==='ConsultarLote')data={loteId:'20',nivelRef:'7',rutaCanonica:'Origen fijado',estado:uncertain?'PENDIENTE_VERIFICACION':window.stopped?'DETENIDO':'EN_CURSO',revision:window.lotRevision,maximoTamanoPagina:25,totalRaices:2,totalArchivos:1,totalBytes:'7',accionesPermitidas:['CONSULTAR','EXPORTAR_PAGINA','CONSULTAR_RAICES',...(!window.stopped?['SOLICITAR_DETENCION']:[]),...(uncertain?['SOLICITAR_VERIFICACION']:window.stopped?[]:['SOLICITAR_AVANCE'])]};
            else if(operation==='ConsultarRecursosLote'){
              const roots=request.tipo==='RAICES';data={loteId:'20',tipo:request.tipo,pagina:1,tamanoPagina:25,totalOriginal:roots?2:1,totalFiltrado:roots?2:1,elementos:roots?[
                {recursoId:'40',nombre:'Carpeta homónima',estado:window.rootState,revision:window.lotRevision,tieneIntentoActivo:false,...(window.rootState!=='PENDIENTE'?{expedienteId:'60',relacionNivelId:'70',nombreExpediente:'Nombre final <img src=x onerror=alert(1)>'}:{})},
                {recursoId:'41',nombre:'Carpeta homónima',estado:'PENDIENTE',revision:1,tieneIntentoActivo:false}
              ]:[{recursoId:'30',raizId:'40',nombre:'a.pdf',estadoRecepcion:'RECIBIDO',estado:window.documentState,revision:window.lotRevision,valoresEfectivosFijados:window.fixedValues,tieneIntentoActivo:uncertain,intentoActivoId:uncertain?'50':null,documentoId:window.documentState==='COMPLETADO'?'90':null}]};
            }else if(operation==='AvanzarLote'){
              window.lotRevision++;
              if(request.fase==='CREAR_EXPEDIENTE')window.rootState='EXPEDIENTE_CONFIRMADO';
              else if(request.fase==='MATERIALIZAR_VALORES')window.fixedValues=true;
              else{window.documentState='INCIERTO';window.rootState='INCIERTO';this.ontimeout();return;}
              data={loteId:'20',raizId:'40',archivoId:request.fase==='CREAR_EXPEDIENTE'?null:'30',fase:request.fase,estado:'CONFIRMADO',expedienteId:'60',relacionNivelId:'70'};
            }else if(operation==='ReconciliarRecurso'){
              window.documentState='COMPLETADO';window.rootState='COMPLETADA';window.lotRevision++;data={loteId:'20',intentoId:'50',resuelta:true,estado:'CONFIRMADO'};
            }else if(operation==='SolicitarDetencion'){window.stopped=true;window.lotRevision++;data={loteId:'20',estado:'DETENIDO',detencionSolicitada:true};}
            else throw new Error('Unexpected synthetic operation '+operation);
            this.status=200;this.responseText=JSON.stringify({ok:true,codigo:'OK',mensaje:'OK',incidencias:[],datos:data});this.onload();
          },0);
        }
      };
    });
    await page.addScriptTag({path:path.join(root,'js/gestion/WebFormProducionDocumental.js')});
    await page.evaluate(()=>{SELECCION_TREVIEW_ID='TreeViewArchivot2';});
    for(const name of ['InventarioImportacion','ImportacionCarpetasApi','ImportacionCarpetasVista','ImportacionCarpetas'])await page.addScriptTag({path:path.join(root,`js/gestion/ImportacionCarpetas/${name}.js`)});
    await page.getByRole('button',{name:'Consultar lote 20',exact:true}).click();await page.getByRole('button',{name:'Crear expediente 40',exact:true}).waitFor();
    await page.getByRole('button',{name:'Crear expediente 40',exact:true}).click();await page.getByRole('button',{name:'Preparar índices 30',exact:true}).waitFor();
    assert.equal(await page.locator('#TreeViewArchivon1Nodes a[title="70|7|60|"]').count(),1);
    assert.equal(await page.evaluate(()=>SELECCION_TREVIEW_ID),'TreeViewArchivot2');
    assert.equal(await page.locator('#TreeViewArchivon1Nodes').evaluate(n=>n.style.display),'none');
    assert.equal(await page.locator('#TreeViewArchivon2Nodes a').count(),0);
    assert.equal(await page.locator('.doc111-content img').count(),0);
    assert.equal(await page.evaluate(()=>phaseCalls.filter(c=>c.operation==='AvanzarLote').length),1);
    await page.getByRole('button',{name:'Preparar índices 30',exact:true}).click();await page.getByRole('button',{name:'Almacenar archivo 30',exact:true}).waitFor();
    await page.getByRole('button',{name:'Almacenar archivo 30',exact:true}).click();await page.waitForFunction(()=>document.querySelector('.doc111-notice').textContent.includes('Consulta el estado'));
    assert.equal(await page.locator('[data-doc111-action="fase"]').count(),0);
    assert.equal(await page.evaluate(()=>phaseCalls.filter(c=>c.operation==='AvanzarLote'&&c.request.fase==='ALMACENAR').length),1);
    await page.getByRole('button',{name:'Consultar estado',exact:true}).click();await page.getByRole('button',{name:'Verificar intento 30',exact:true}).waitFor();
    await page.getByRole('button',{name:'Verificar intento 30',exact:true}).click();await page.waitForFunction(()=>document.querySelector('.doc111-content').textContent.includes('COMPLETADA'));
    assert.equal(await page.locator('#TreeViewArchivon1Nodes a[title="70|7|60|"]').count(),1);
    assert.equal(await page.evaluate(()=>phaseCalls.filter(c=>c.operation==='AvanzarLote'&&c.request.fase==='ALMACENAR').length),1);
    await page.getByRole('button',{name:'Solicitar detención',exact:true}).click();await page.waitForFunction(()=>document.querySelector('.doc111-content').textContent.includes('DETENIDO'));
    assert.equal(await page.locator('[data-doc111-action="fase"]').count(),0);
    const outcome=await page.evaluate(()=>{
      const context={nivelRef:'7',referenciaContexto:'a'.repeat(32)},row={estado:'EXPEDIENTE_CONFIRMADO',expedienteId:'61',relacionNivelId:'71',nombreExpediente:'Nombre final <img src=x onerror=alert(1)>'};
      Doc111ActualizarRama(context,[row,row]);Doc111ActualizarRama(context,[row]);
      const before=document.querySelectorAll('#TreeViewArchivon1Nodes a[title]').length;
      document.getElementById('TreeViewArchivot1').style.display='none';const hidden=Doc111ActualizarRama(context,[{...row,expedienteId:'62',relacionNivelId:'72'}]);document.getElementById('TreeViewArchivot1').style.display='';
      const wrong=Doc111ActualizarRama({...context,referenciaContexto:'b'.repeat(32)},[row]);
      document.getElementById('TreeViewArchivon1Nodes').remove();const missing=Doc111ActualizarRama(context,[row]);
      return {before,hidden,wrong,missing,selection:SELECCION_TREVIEW_ID};
    });
    assert.deepEqual(outcome,{before:2,hidden:false,wrong:false,missing:false,selection:'TreeViewArchivot2'});
    await page.setViewportSize({width:390,height:844});assert.equal(await page.evaluate(()=>document.documentElement.scrollWidth<=innerWidth),true);
    assert.deepEqual(errors,[]);
  }finally{await browser.close();}
});
