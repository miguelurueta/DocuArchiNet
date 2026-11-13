'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs'),path=require('node:path');
const {chromium}=require('../tools/e2e/node_modules/playwright');
const root=path.resolve(__dirname,'..');
async function fixture(work,{admin=false,adminCatalogs=false,level='7',delayOpening=false,delayVerification=false}={}){
  const browser=await chromium.launch({headless:true});
  try{
    const page=await browser.newPage({viewport:{width:1000,height:720}}),calls=[];page.setDefaultTimeout(5000);
    let releaseOpening,releaseVerification;
    const openingGate=delayOpening?new Promise(resolve=>{releaseOpening=resolve;}):null,verificationGate=delayVerification?new Promise(resolve=>{releaseVerification=resolve;}):null;
    const host='<div id="doc111-importacion" data-navegacion="menu" data-api="/control.asmx" data-upload="/upload.ashx" data-token="synthetic" data-recuperacion="'+ 'a'.repeat(32)+'" data-nivel="'+level+'"><button type="button" data-doc111-open hidden disabled>Importar carpetas</button><button type="button" data-doc111-resume hidden>Último lote</button><button type="button" data-doc111-verificar hidden>Verificar</button><span data-doc111-status hidden></span></div>';
    const reference='<li class="nav-item dropdown active ml-2 mr-0 active_"><a class="nav-link dropdown-toggle" style="color:#6d7fcc" href="#" id="reference-files"><i style="color:#0062cc" class="fas fa-file" aria-hidden="true"></i> Archivos</a><div class="dropdown-menu"><a class="dropdown-item" id="reference-item"><i class="fas fa-edit" aria-hidden="true"></i> Cambiar nombre</a></div></li>';
    const html='<html><body><form><nav id="menu_var" class="navbar navbar-expand-sm nav_botota_person_gray"><ul class="navbar-nav">'+reference+'</ul></nav><div id="TreeViewArchivo"><a id="TreeViewArchivot1" title="7\\\\Homónima">Homónima</a><a id="TreeViewArchivot2i" title="8\\\\Homónima"><img alt=""></a><a id="TreeViewArchivot2" title="8\\\\Homónima">Homónima</a><a id="TreeViewArchivot3" title="70|7|60|">Expediente</a><a id="TreeViewArchivot4" title="FONDO">Fondo</a></div>'+host+'<div id="layout-probe">Contenido estable</div>'+(admin?'<div id="doc110-perfiles" data-api="/control.asmx" data-token="synthetic" data-proveedores="'+(adminCatalogs?'1':'0')+'" data-fuentes="'+(adminCatalogs?'1':'0')+'"></div>':'')+'</form></body></html>';
    const code=['InventarioImportacion','ImportacionCarpetasApi','ImportacionCarpetasVista','ImportacionCarpetas'].map(name=>fs.readFileSync(path.join(root,'js/gestion/ImportacionCarpetas/'+name+'.js'),'utf8')).join('\n');
    const legacy=fs.readFileSync(path.join(root,'js/gestion/WebFormProducionDocumental.js'),'utf8');
    const adminCode=admin?['ProveedorAdminEditor','FuenteAdminEditor','PerfilImportacionEditor'].map(name=>fs.readFileSync(path.join(root,'js/gestion/ImportacionCarpetas/'+name+'.js'),'utf8')).join('\n'):'';
    await page.addInitScript({content:'window.$=value=>{if(typeof value==="function")document.addEventListener("DOMContentLoaded",value,{once:true});return {ready(){},on(){},autocomplete(){}};};window.jQuery=window.$;window.PerfilImportacionApi={crear:()=>({})};\n'+legacy+'\nwindow.Doc119NivelPulsado=Doc119NivelPulsado;window.Doc119InvocarImportacion=Doc119InvocarImportacion;\n'+code+'\n'+adminCode});
    await page.route('https://doc119.invalid/**',async route=>{
      const request=route.request();if(request.method()==='GET')return route.fulfill({status:200,contentType:'text/html',body:html});
      const operation=new URL(request.url()).pathname.split('/').pop(),body=request.postDataJSON().solicitud;calls.push({operation,request:body});
      if(operation==='AbrirImportacion'&&openingGate)await openingGate;
      if(operation==='ConsultarConfirmacionLote'&&verificationGate)await verificationGate;
      if(await page.evaluate(()=>window.deny===true))return route.fulfill({status:403,contentType:'application/json',body:JSON.stringify({ok:false,codigo:'ACCESO_DENEGADO',mensaje:'denegado',incidencias:[]})});
      let data;
      if(operation==='AbrirImportacion')data={nivelRef:body.nivelRef,rutaCanonica:'Ruta autorizada '+body.nivelRef,perfiles:[],paginaPerfiles:{puedeHaberMas:false}};
      else if(operation==='ConsultarLote')data={loteId:body.loteId,nivelRef:'8',rutaCanonica:'Destino original 8',estado:'RECIBIENDO',maximoTamanoPagina:25,totalRaices:0,totalArchivos:0,totalBytes:'0',accionesPermitidas:['CONSULTAR']};
      else if(operation==='ConsultarRecursosLote')data={loteId:body.loteId,tipo:body.tipo,pagina:1,tamanoPagina:25,totalOriginal:0,totalFiltrado:0,elementos:[],puedeHaberMas:false};
      else if(operation==='ConsultarConfirmacionLote')data={planId:body.planId,confirmada:await page.evaluate(()=>window.confirmed===true),loteId:await page.evaluate(()=>window.confirmed===true)?'29':null};
      else throw Error('Operacion fuera de alcance local: '+operation);
      return route.fulfill({status:200,contentType:'application/json',body:JSON.stringify({ok:true,codigo:'OK',mensaje:'OK',datos:data,incidencias:[]})});
    });
    await page.goto('https://doc119.invalid/production');
    await page.addStyleTag({content:fs.readFileSync(path.join(root,'bootstrap/css/bootstrap.min.css'),'utf8')});
    await page.addStyleTag({content:fs.readFileSync(path.join(root,'Styles/bootra-person.css'),'utf8')});
    const font=fs.readFileSync(path.join(root,'Awesome/webfonts/fa-solid-900.woff2')).toString('base64');
    await page.addStyleTag({content:'@font-face{font-family:"Font Awesome 5 Pro";font-weight:900;src:url(data:font/woff2;base64,'+font+') format("woff2");}.fas{font-family:"Font Awesome 5 Pro";font-weight:900;}'+fs.readFileSync(path.join(root,'Awesome/css/fontawesome.css'),'utf8')});
    await page.addStyleTag({content:fs.readFileSync(path.join(root,'js/gestion/ImportacionCarpetas/ImportacionCarpetas.css'),'utf8')});
    await page.locator('[data-doc111-query-open]').waitFor({state:'attached'});
    await work(page,calls,{releaseOpening:()=>releaseOpening?.(),releaseVerification:()=>releaseVerification?.()});
  }finally{await browser.close();}
}
async function access(page,selector){if(!await page.locator(selector).isVisible())await page.locator('#doc110-navigation-toggle').click();await page.locator(selector).click();}
test('DOC119 importador sin administracion tiene menu unico y abre el destino seleccionado',()=>fixture(async(page,calls)=>{
  assert.equal(await page.locator('#doc110-navigation').count(),1);assert.equal(await page.locator('[data-doc119-administracion]').count(),0);
  assert.equal(await page.locator('#doc111-importacion > button:visible').count(),0);
  await page.evaluate(()=>{ImportacionCarpetas.inicializar();ImportacionCarpetas.inicializar();});
  assert.equal(await page.locator('[data-doc119-operacion]').count(),1);
  await access(page,'[data-doc111-open]');await page.getByRole('dialog').waitFor();
  assert.equal(calls[0].request.nivelRef,'7');assert.match(await page.locator('.doc111-destino').innerText(),/Ruta autorizada 7/);
}));
test('DOC119 administracion conserva sus permisos y separacion dentro del menu compartido',()=>fixture(async page=>{
  assert.equal(await page.locator('#doc110-navigation').count(),1);
  assert.equal(await page.locator('[data-doc119-operacion]').count(),1);assert.equal(await page.locator('[data-doc119-administracion]').count(),1);
  assert.equal(await page.locator('[data-doc110="abrir"]').count(),1);
  assert.equal(await page.locator('[data-doc110="admin-proveedores"],[data-doc110="admin-fuentes"]').count(),0);
  await page.evaluate(()=>{const old=document.getElementById('doc110-perfiles'),replacement=old.cloneNode(false);replacement.removeAttribute('data-initialized');old.replaceWith(replacement);PerfilImportacionEditor.inicializar();});
  assert.equal(await page.locator('[data-doc119-administracion]').count(),1);
},{admin:true}));
test('DOC119 contextual usa nivel pulsado homonimo y no cambia destino de ventana abierta',()=>fixture(async(page,calls)=>{
  await page.evaluate(()=>{const target=document.getElementById('TreeViewArchivot2');window.context={data:{nivelRef:Doc119NivelPulsado(target)}};Doc119InvocarImportacion(context,false);});
  await page.getByRole('dialog').waitFor();assert.equal(calls[0].request.nivelRef,'8');
  await page.evaluate(()=>Doc119InvocarImportacion({data:{nivelRef:'7'}},false));
  assert.equal(calls.length,1);assert.match(await page.locator('.doc111-destino').innerText(),/Ruta autorizada 8/);
  await page.getByRole('button',{name:'Cerrar',exact:true}).click();
  const invalid=await page.evaluate(()=>['TreeViewArchivot3','TreeViewArchivot4'].map(id=>Doc119NivelPulsado(document.getElementById(id))));assert.deepEqual(invalid,[null,null]);
  await page.evaluate(()=>Doc119InvocarImportacion({data:{nivelRef:null}},false));assert.equal(calls.length,1);
}));
test('DOC119 consulta sin seleccion valida referencia por Enter y conserva destino original',()=>fixture(async(page,calls)=>{
  assert.equal(await page.locator('[data-doc111-open]').isDisabled(),true);
  await access(page,'[data-doc111-query-open]');await page.getByRole('dialog').waitFor();
  await page.getByLabel('Referencia del lote',{exact:true}).fill('abc');await page.keyboard.press('Enter');
  assert.equal(calls.length,0);assert.match(await page.locator('.doc111-notice').innerText(),/referencia.*válida/);
  await page.getByLabel('Referencia del lote',{exact:true}).fill('29');await page.keyboard.press('Enter');
  await page.waitForFunction(()=>document.querySelector('.doc111-destino').textContent.includes('Destino original 8'));
  assert.equal(calls.some(call=>call.operation==='AbrirImportacion'||call.operation==='ConfirmarPlan'),false);
  await page.setViewportSize({width:390,height:640});fs.mkdirSync(path.join(root,'.tmp'),{recursive:true});await page.screenshot({path:path.join(root,'.tmp/doc119-consulta-ui-20261112.png')});
  await page.getByRole('button',{name:'Cerrar',exact:true}).click();
  await page.reload();await page.locator('#doc110-navigation-toggle').click();await page.locator('[data-doc111-resume]').waitFor({state:'visible'});
  assert.match(await page.locator('[data-doc111-resume]').innerText(),/29/);
},{level:''}));
test('DOC119 permiso revocado no abre asistente ni expone datos de un destino anterior',()=>fixture(async(page,calls)=>{
  await page.evaluate(()=>window.deny=true);await access(page,'[data-doc111-open]');
  await page.waitForFunction(()=>document.querySelector('[data-doc111-status]').textContent.includes('autorización'));
  assert.equal(await page.getByRole('dialog').count(),0);assert.equal(calls.length,1);
}));
test('DOC119 confirmacion pendiente se verifica sin reconfirmar y las pistas se aislan por contexto',()=>fixture(async(page,calls)=>{
  await page.evaluate(()=>sessionStorage.setItem('DOC111_REF_'+'a'.repeat(32),JSON.stringify({v:1,planId:'45'})));await page.reload();
  await access(page,'[data-doc111-query-open]');await page.getByRole('dialog').waitFor();
  await page.getByRole('dialog').getByRole('button',{name:'Verificar confirmación pendiente',exact:true}).click();
  await page.waitForFunction(()=>document.querySelector('[data-doc111-status]').textContent.includes('Todavía no hay'));
  assert.equal(await page.evaluate(()=>document.getElementById('doc111-importacion').doc111ConfirmacionPendiente.planId),'45');
  await page.evaluate(()=>window.confirmed=true);await access(page,'[data-doc111-verificar]');
  await page.waitForFunction(()=>document.querySelector('.doc111-destino')?.textContent.includes('Destino original 8'));
  assert.equal(calls.some(call=>call.operation==='ConfirmarPlan'),false);
  await page.getByRole('button',{name:'Cerrar',exact:true}).click();
  await page.evaluate(()=>{const host=document.getElementById('doc111-importacion');host.setAttribute('data-recuperacion','b'.repeat(32));ImportacionCarpetas.inicializar();});
  assert.equal(await page.locator('[data-doc111-resume]').isVisible(),false);assert.equal(await page.locator('[data-doc111-verificar]').isVisible(),false);
  assert.equal(await page.locator('[data-doc119-operacion]').count(),1);
}));

for(const denied of [false,true])test('DOC120 apertura demorada sin franja amarilla ni salto de pagina: '+(denied?'error visible':'destino autorizado'),()=>fixture(async(page,calls,gate)=>{
  await page.evaluate(value=>{window.deny=value;},denied);
  const probeBefore=await page.locator('#layout-probe').boundingBox(),navBefore=await page.locator('#doc110-navigation-toggle').boundingBox();
  await access(page,'[data-doc111-open]');
  await page.waitForFunction(()=>document.querySelector('[data-doc111-open]').disabled);
  const probeDuring=await page.locator('#layout-probe').boundingBox(),navDuring=await page.locator('#doc110-navigation-toggle').boundingBox();
  assert.equal(probeDuring.y,probeBefore.y,'El estado de carga desplaza el contenido');
  assert.deepEqual(navDuring,navBefore,'El indicador altera la posición o tamaño del menú');
  const loading=await page.locator('[data-doc111-status]').evaluate(el=>({text:el.textContent,state:el.getAttribute('data-doc111-state'),background:getComputedStyle(el).backgroundColor,height:el.getBoundingClientRect().height}));
  assert.match(loading.text,/Validando el destino/);assert.equal(loading.state,'loading');assert.notEqual(loading.background,'rgb(255, 245, 217)');assert(loading.height<=1);
  assert.equal(await page.locator('#doc110-navigation').getAttribute('aria-busy'),'true');
  await page.evaluate(()=>ImportacionCarpetas.inicializar());
  assert.equal(calls.filter(c=>c.operation==='AbrirImportacion').length,1);
  gate.releaseOpening();
  if(denied){await page.waitForFunction(()=>document.querySelector('[data-doc111-status]').textContent.includes('autorización'));assert.equal(await page.getByRole('dialog').count(),0);assert.equal(await page.locator('[data-doc111-status]').isVisible(),true);assert.equal(await page.locator('[data-doc111-status]').getAttribute('data-doc111-state'),'notice');}
  else{await page.getByRole('dialog').waitFor();assert.equal(await page.locator('[data-doc111-status]').isVisible(),false);await page.getByRole('button',{name:'Cerrar',exact:true}).click();assert.equal((await page.locator('#layout-probe').boundingBox()).y,probeBefore.y);}
  assert.equal(await page.locator('#doc110-navigation').getAttribute('aria-busy'),'false');
},{delayOpening:true}));

test('DOC120 verificar confirmacion pendiente conserva el aviso visible durante la carga',()=>fixture(async(page,calls,gate)=>{
  await page.evaluate(()=>sessionStorage.setItem('DOC111_REF_'+'a'.repeat(32),JSON.stringify({v:1,planId:'45'})));await page.reload();
  await access(page,'[data-doc111-verificar]');
  await page.waitForFunction(()=>document.querySelector('[data-doc111-verificar]').disabled);
  const notice=page.locator('[data-doc111-status]');assert.equal(await notice.isVisible(),true);assert.match(await notice.innerText(),/requiere verificar/);assert.equal(await notice.getAttribute('data-doc111-state'),'notice');
  gate.releaseVerification();await page.waitForFunction(()=>document.querySelector('[data-doc111-status]').textContent.includes('Todavía no hay'));
  assert.equal(calls.some(c=>c.operation==='ConfirmarPlan'),false);assert.equal(await notice.isVisible(),true);
},{delayVerification:true}));

for(const admin of [false,true])test('DOC120 menu con iconos y colores de Archivos: '+(admin?'administracion autorizada':'solo operacion'),()=>fixture(async(page,calls)=>{
  const color=selector=>page.locator(selector).evaluate(el=>getComputedStyle(el).color);
  assert.equal(await color('#doc110-navigation-toggle'),await color('#reference-files'));
  assert.equal(await color('#doc110-navigation-toggle .doc120-menu-icon'),await color('#reference-files i'));
  await page.locator('#doc110-navigation-toggle').click();
  const buttons=page.locator('#doc110-navigation .dropdown-item:visible');assert.equal(await buttons.count(),admin?5:2);
  for(const button of await buttons.all()){
    assert.equal(await button.evaluate(el=>getComputedStyle(el).color),await color('#reference-item'));
    assert.equal(await button.locator('[aria-hidden="true"]').count(),1);
    assert.equal(await button.locator('[data-doc120-menu-label]').count(),1);
    assert.notEqual(await button.getAttribute('type'),'submit');
  }
  await page.evaluate(()=>{ImportacionCarpetas.inicializar();ImportacionCarpetas.inicializar();});
  assert.equal(await page.locator('#doc110-navigation-toggle .doc120-menu-icon').count(),1);
  assert.equal(await page.locator('[data-doc111-query-open] .doc120-menu-icon').count(),1);
  assert.equal(calls.length,0);
  await page.evaluate(()=>document.fonts.ready);
  await page.screenshot({path:path.join(root,'.tmp/doc120-menu-'+(admin?'admin':'operacion')+'-20261113.png'),clip:{x:0,y:0,width:650,height:360}});
  await page.evaluate(()=>{sessionStorage.setItem('DOC111_REF_'+'a'.repeat(32),JSON.stringify({v:1,loteId:'29',planId:'45'}));});await page.reload();
  await page.locator('#doc110-navigation-toggle').click();
  assert.equal(await page.getByRole('button',{name:'Consultar lote 29',exact:true}).locator('.doc120-menu-icon').count(),1);
  assert.equal(await page.getByRole('button',{name:'Consultar importación… · verificación pendiente',exact:true}).locator('.doc120-menu-icon').count(),1);
  assert.equal(await page.locator('[data-doc111-verificar] .doc120-menu-icon').count(),1);
},{admin,adminCatalogs:admin}));
