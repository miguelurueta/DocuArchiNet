'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs'),path=require('node:path');
const {parse}=require('../tools/opsxj/node_modules/acorn');
const {chromium}=require('../tools/e2e/node_modules/playwright');
const root=path.resolve(__dirname,'..'),read=f=>fs.readFileSync(path.join(root,f),'utf8');
const baseline=require('./Fixtures/ProduccionDocumental/doc120-barra-acciones.json');
function declaredFunctions(file,names){const code=read(file),ast=parse(code,{ecmaVersion:'latest'});return names.map(name=>{const n=ast.body.find(n=>n.type==='FunctionDeclaration'&&n.id.name===name);assert(n,name);return code.slice(n.start,n.end);}).join('\n');}
const routing=declaredFunctions('js/gestion/WebFormProducionDocumental.js',['activa_menu','preven_event_search','preven_event_search_keypres_enter','preven_event_restor_search','auto_zise'])+'\n'+declaredFunctions('js/java_general/general_code_java.js',['activa_boton_client_server']);
async function fixture(work){
  const browser=await chromium.launch({headless:true});
  try{
    const page=await browser.newPage({viewport:{width:1554,height:848}}),errors=[],network=[];page.setDefaultTimeout(5000);page.on('pageerror',e=>errors.push(e.message));
    await page.route('**/*',route=>{network.push(route.request().url());return route.abort();});
    const source=read('Gestion/WebFormProducionDocumental.aspx');
    const start=source.indexOf('<nav id="menu_var"'),nav=source.slice(start,source.indexOf('</nav>',start)+6).replace(/<asp:TextBox\b([^>]*)><\/asp:TextBox>/g,'<input $1>');
    const serverIds=[...new Set([...routing.matchAll(/getElementById\("(Button[^"\n]+)"\)/g),...nav.matchAll(/activa_boton_client_server\('([^']+)'\)/g)].map(m=>m[1]).concat('Button_Restaura_busqueda','Button_busca_general_archivo'))];
    await page.setContent('<html><head><meta charset="utf-8"></head><body><form onsubmit="event.preventDefault()">'+nav+'<div id="doc111-importacion" data-navegacion="menu" data-nivel="7" data-recuperacion="'+ 'a'.repeat(32)+'"><button type="button" data-doc111-open hidden>Importar carpetas</button><button type="button" data-doc111-resume hidden>Último lote</button><button type="button" data-doc111-verificar hidden>Verificar</button><span role="status" data-doc111-status hidden></span></div><div id="Contentizquierdo"><div id="title_treview" style="height:30px"></div><div id="bar_table" style="height:30px"></div><div id="div_treview_archivo"></div><div id="contenido_pie" style="height:20px"></div></div><div id="Contenedorderecho"><div id="Panel_unidad_treview_unidad"></div><div id="foter_estado" style="height:20px"></div></div><input id="HiddenField_rest_text_node" value="Destino original"><input id="Hidden_sele_docu" value="11,12"><span id="Label_title_selecion"></span><span id="titulo_label_grid"></span><div id="Panelactividad_documentos"></div><div id="id_tag_spiner"></div>'+serverIds.map(id=>'<button type="button" hidden id="'+id+'"></button>').join('')+'</form></body></html>');
    for(const f of ['bootstrap/css/bootstrap.min.css','Styles/bootra-person.css','Styles/Menu3.css','Styles/styleMenu.css','Styles/Aplicaction.css','js/gestion/ImportacionCarpetas/ImportacionCarpetas.css','Styles/produccion-documental-barra.css'])if(fs.existsSync(path.join(root,f)))await page.addStyleTag({content:read(f)});
    await page.addScriptTag({content:read('js/ui/jquery-3.4.1.min.js')});await page.addScriptTag({content:read('bootstrap/js/bootstrap.min.js')});
    await page.addScriptTag({content:'window.SELECCION_TREVIEW_ID="nivel-7";window.calls=[];window.partialLoads=[];window.Sys={Application:{add_load:fn=>partialLoads.push(fn)}};window.alert=message=>{throw Error(message)};\n'+routing});
    await page.evaluate(ids=>{
      ids.forEach(id=>document.getElementById(id).addEventListener('click',()=>calls.push({kind:'postback',id})));
      ['ShowInterfazSolicitudFirma','Get_cortar_expediente','Get_pegar_expediente','clear_seleccion_treview'].forEach(name=>window[name]=()=>calls.push({kind:'function',name}));
      window.event_multiple_row=(event,grid,operation)=>calls.push({kind:'multiple',grid,operation,eventType:event.type});
      window.activa_copia_archivo=grid=>calls.push({kind:'copy',grid});
      window.asigna_id_seleccionados_cheked_general_hiden=(grid,hidden)=>calls.push({kind:'selection',grid,hidden});
    },serverIds);
    for(const name of ['InventarioImportacion','ImportacionCarpetasVista','ImportacionCarpetas'])await page.addScriptTag({content:read('js/gestion/ImportacionCarpetas/'+name+'.js')});
    await page.addScriptTag({content:read('Awesome/js/all.js')});
    if(fs.existsSync(path.join(root,'js/gestion/ProduccionDocumentalBarra.js')))await page.addScriptTag({content:read('js/gestion/ProduccionDocumentalBarra.js')});
    await page.waitForFunction(()=>document.querySelector('#menu_var svg'));
    await work(page);assert.deepEqual(errors,[]);assert.deepEqual(network,[],'La prueba no debe solicitar servicios reales');
  }finally{await browser.close();}
}
async function openBar(page){if(await page.locator('#nav_togle_display').isVisible()&&!await page.locator('#navbarNavDropdown').isVisible()){await page.locator('#nav_togle_display').click();await page.locator('#navbarNavDropdown.show').waitFor();}}
async function openAction(page,index){await openBar(page);const item=page.locator('#menu_var [onclick]').nth(index);const ids=await item.evaluate(el=>({root:el.closest('li.nav-item')?.querySelector(':scope > a').id,sub:el.closest('.dropdown-submenu')?.querySelector(':scope > a').id}));if(ids.root&&await page.locator('#'+ids.root).getAttribute('aria-expanded')!=='true')await page.locator('#'+ids.root).click();if(ids.sub)await page.locator('#'+ids.sub).hover();return item;}

test('DOC120 barra conserva los 36 manejadores y la búsqueda originales',()=>fixture(async page=>{
  assert.deepEqual(await page.locator('#menu_var [onclick]').evaluateAll(nodes=>nodes.map(n=>n.getAttribute('onclick'))),baseline.handlers);
  assert.equal(await page.locator('#TextBox_buequeda_general').getAttribute('onkeypress'),baseline.searchKeypress);
  for(const width of [1554,390]){
    await page.setViewportSize({width,height:848});await openBar(page);await page.locator('#TextBox_buequeda_general').fill('Contrato original');
    for(let i=0;i<baseline.handlers.length;i++){
      const item=await openAction(page,i);await page.evaluate(()=>{calls.length=0;});await item.click();
      assert.deepEqual(await page.evaluate(()=>calls),baseline.expected[i],width+': '+baseline.handlers[i]);
      assert.equal(await page.inputValue('#TextBox_buequeda_general'),'Contrato original');assert.equal(await page.inputValue('#Hidden_sele_docu'),'11,12');
    }
    await openBar(page);await page.evaluate(()=>{calls.length=0;});await page.locator('#TextBox_buequeda_general').press('Enter');assert.deepEqual(await page.evaluate(()=>calls),[{kind:'postback',id:'Button_busca_general_archivo'}]);
    assert.equal(await page.locator('#Label_title_selecion').textContent(),'Expediente (Destino original)');
  }
  await page.evaluate(()=>{SELECCION_TREVIEW_ID='';calls.length=0;});await page.locator('#td-boton').click();assert.deepEqual(await page.evaluate(()=>calls),[],'Restaurar sin selección conserva el comportamiento original');
}));

test('DOC120 barra alinea cabeceras sin desbordar con texto normal o aumentado',()=>fixture(async page=>{
  for(const font of [16,20])for(const width of [390,768,1024,1280,1554]){
    await page.setViewportSize({width,height:848});await page.addStyleTag({content:'html{font-size:'+font+'px!important}'});await openBar(page);
    const state=await page.evaluate(()=>({width:innerWidth,scrollWidth:document.querySelector('#menu_var').scrollWidth,links:[...document.querySelectorAll('#menu_var .navbar-nav > li > a')].map(el=>{const r=el.getBoundingClientRect(),s=getComputedStyle(el);return {label:el.textContent.trim(),x:r.x,right:r.right,height:r.height,line:parseFloat(s.lineHeight),padding:parseFloat(s.paddingTop)+parseFloat(s.paddingBottom)};}),search:document.querySelector('#menu_var .input-group').getBoundingClientRect().toJSON()}));
    assert(state.scrollWidth<=width,JSON.stringify({font,width,state}));assert.equal(state.links.length,7);
    for(const link of state.links){assert(link.x>=0&&link.right<=width,JSON.stringify(link));assert(link.height<=link.line+link.padding+1,JSON.stringify({width,font,link}));}
    assert(state.search.x>=0&&state.search.right<=width,JSON.stringify(state.search));
  }
}));

test('DOC120 barra actualiza altura al colapsar y conserva foco y montaje tras postback',()=>fixture(async page=>{
  await page.setViewportSize({width:390,height:848});await openBar(page);
  await page.waitForFunction(()=>document.getElementById('Contentizquierdo').clientHeight===innerHeight-document.getElementById('menu_var').clientHeight-20);
  const expanded=await page.locator('#Contentizquierdo').evaluate(el=>el.clientHeight);
  await page.evaluate(()=>{partialLoads.forEach(fn=>fn());ImportacionCarpetas.inicializar();ImportacionCarpetas.inicializar();});
  assert.equal(await page.locator('#doc110-navigation').count(),1);assert.equal(await page.locator('[data-doc119-operacion]').count(),1);
  await page.locator('#A5').focus();await page.keyboard.press('Enter');await page.locator('#A5 + .dropdown-menu.show').waitFor();await page.keyboard.press('Escape');assert.equal(await page.locator('#A5').evaluate(el=>el===document.activeElement),true);
  await page.locator('#nav_togle_display').click();await page.waitForFunction(()=>document.getElementById('Contentizquierdo').clientHeight>500&&document.getElementById('Contentizquierdo').clientHeight===innerHeight-document.getElementById('menu_var').clientHeight-20);
  assert((await page.locator('#Contentizquierdo').evaluate(el=>el.clientHeight))>expanded);
}));

test('DOC120 barra abre Importación una sola vez y conserva el destino tras reinicializar',()=>fixture(async page=>{
  await page.setViewportSize({width:768,height:848});await openBar(page);
  await page.evaluate(()=>{window.ImportacionCarpetasApi={crear:()=>({cerrar(){},invocar(operation,request){calls.push({kind:'api',operation,request});return Promise.resolve({nivelRef:'7',rutaCanonica:'Destino autorizado 7',perfiles:[],paginaPerfiles:{puedeHaberMas:false}});}})};partialLoads.forEach(fn=>fn());ImportacionCarpetas.inicializar();calls.length=0;});
  await page.locator('#doc110-navigation-toggle').click();await page.locator('[data-doc111-open]').click();await page.getByRole('dialog').waitFor();
  assert.match(await page.locator('.doc111-destino').innerText(),/Destino autorizado 7/);
  const calls=await page.evaluate(()=>window.calls);assert.equal(calls.length,1);assert.equal(calls[0].operation,'AbrirImportacion');assert.equal(calls[0].request.nivelRef,'7');
  await page.getByRole('button',{name:'Cerrar',exact:true}).click();assert.equal(await page.locator('#doc110-navigation-toggle').evaluate(el=>el===document.activeElement),true);
}));

test('DOC120 barra mantiene accesibles los desplegables largos dentro de la pantalla',()=>fixture(async page=>{
  for(const font of [16,20]){
    await page.setViewportSize({width:390,height:848});await page.addStyleTag({content:'html{font-size:'+font+'px!important}'});await openBar(page);
    if(await page.locator('#navbarDropdownMenuLink').getAttribute('aria-expanded')!=='true')await page.locator('#navbarDropdownMenuLink').click();await page.locator('#A6').hover();
    await page.waitForFunction(()=>document.getElementById('Contentizquierdo').clientHeight===innerHeight-document.getElementById('menu_var').clientHeight-20);
    const sizes=await page.evaluate(()=>({bar:document.getElementById('menu_var').getBoundingClientRect().height,scrollWidth:document.getElementById('menu_var').scrollWidth,tree:document.getElementById('Contentizquierdo').clientHeight,viewport:innerHeight,width:innerWidth}));
    assert(sizes.bar<sizes.viewport*.8,JSON.stringify(sizes));assert(sizes.scrollWidth<=sizes.width,JSON.stringify(sizes));assert(sizes.tree>100,JSON.stringify(sizes));
    await page.evaluate(()=>{calls.length=0;});const last=page.locator('#menu_var [onclick]').filter({hasText:'Meta datos'});await last.locator('xpath=ancestor::li').locator(':scope > a').click();await last.click();assert.deepEqual(await page.evaluate(()=>calls),[{kind:'postback',id:'Button_activa_gestion_meta_dato'}]);
  }
}));
