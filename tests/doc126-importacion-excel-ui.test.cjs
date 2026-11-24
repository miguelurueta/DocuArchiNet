'use strict';
// Browser real, transporte y respuestas sintéticos. No acredita IIS/MySQL/E2E real.
const test=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs'),path=require('node:path');
const {chromium}=require('../tools/e2e/node_modules/playwright');
const root=path.resolve(__dirname,'..'),id='10000000-0000-4000-8000-000000000001';
const clone=v=>JSON.parse(JSON.stringify(v));
async function fixture(work,options={}){
 const browser=await chromium.launch({headless:true});
 try{
  const page=await browser.newPage({viewport:options.viewport||{width:1280,height:720}});page.setDefaultTimeout(7000);
  const errors=[];page.on('pageerror',e=>errors.push(e.message));
  const calls=[],mode=options.mode||'AGREGAR';let confirmed=false,configuration='b'.repeat(64);
  const template={ID_PLANTILLAIMP:29,IMP01_XMLCONEXION_ID_XMLCONEXION:6,REVISION_CARGA:1,Huella:'a'.repeat(64),NOMBRE_PLANTILLA:'imp_02_fixture',CAMPO_UNICO:'CLAVE',LIBRO_EXCEL:'Datos',Formato:'XLSX',Modo:mode,ESTADO_PLANTILLA:options.inactive?0:1,Campos:[{ID_CAMPOS_PLANTILLA:10,CAMPO_FUENTE:'Código',CAMPO_DESTINO:'CLAVE',TIPO_CAMPO:'VARCHAR',REMPLAZA_COMA:null,REMPLAZA_PUNTO:0,OPCION_SPLITER:null,CARACTER_SPLITER:null,NUMERO_MATRIZ_EXPLITER:null}]};
  const carga=state=>({IdCarga:id,PlantillaId:29,Estado:state,Tabla:template.NOMBRE_PLANTILLA,Modo:mode,ArchivoHash:'a'.repeat(64),ConfiguracionHash:configuration,EsquemaHash:'c'.repeat(64),FilasPrevistas:1000,FilasConfirmadas:state==='CONFIRMADA'?1000:0,ConfirmadoUtc:'/Date(1735776000000)/',Hojas:['Datos','Otra']});
  const prepared=()=>({Carga:carga('PREPARADA'),LecturaCompleta:true,ValidacionCompleta:true,DuplicadosEvaluados:true,FilasObservadas:1000,TotalIncidencias:0,Incidencias:[],Muestra:[{Fila:2,Valores:[{CampoDestino:'CLAVE',Tipo:'TEXTO',Texto:'00017'},{CampoDestino:'NUMERO',Tipo:'DECIMAL',NumeroCanonico:'123456789012345678901234567890123456789.123'},{CampoDestino:'FECHA',Tipo:'DATE',Fecha:'/Date(1735776000000)/'},{CampoDestino:'VACIO',Tipo:'TEXTO',EsNulo:true},{CampoDestino:'HTML',Tipo:'TEXTO',Texto:'<img src=x onerror=window.pwned=true>'}]}]});
  if(options.noStorage)await page.addInitScript(()=>Object.defineProperty(window,'sessionStorage',{get(){throw Error('synthetic storage denial');}}));
  await page.route('https://doc126.invalid/**',async route=>{
   const request=route.request();if(request.method()==='GET')return route.fulfill({status:200,contentType:'text/html',body:'<!doctype html><html><body><form><nav id="menu_var"><ul class="navbar-nav"></ul></nav><div id="doc126-excel" data-api="/excel.asmx" data-upload="/upload.ashx" data-recuperacion="'+'d'.repeat(32)+'"></div></form></body></html>'});
   const operation=new URL(request.url()).pathname.split('/').pop()==='upload.ashx'?'Upload':new URL(request.url()).pathname.split('/').pop();
   const body=operation==='Upload'?null:request.postDataJSON().solicitud;
   calls.push({operation,body});
   assert.equal(request.headers()['x-importacion-excel-contexto'],'d'.repeat(32));
   if(operation!=='AbrirCargaExcel')assert.equal(request.headers()['x-importacion-excel-csrf'],'synthetic-ui-token');
   let data,ok=true,code='OK';
   if(operation==='AbrirCargaExcel')data={CsrfToken:'synthetic-ui-token',Conexiones:[{ConexionId:6,Nombre:'Fixture autorizada'}],PermiteNuevaCarga:!options.closedCapacity,MaxFilasDatos:20000,MaxArchivoMB:10};
   else if(operation==='ListarPlantillas')data={Total:1,Pagina:1,TamanoPagina:25,Plantillas:[{PlantillaId:29,Nombre:template.NOMBRE_PLANTILLA,Activa:true}]};
   else if(operation==='ConsultarPlantilla'){if(options.detailGate)await options.detailGate;data=clone(template);}
   else if(operation==='GuardarPlantilla'){
    if(options.conflictSave){ok=false;code='CONFLICTO_REVISION';}
    else if(options.ignoreSave){return route.abort('failed');}
    else {Object.assign(template,body.Plantilla,{ID_PLANTILLAIMP:body.Plantilla.ID_PLANTILLAIMP||30,REVISION_CARGA:2,Huella:'e'.repeat(64)});data=clone(template);}
   }
   else if(operation==='IniciarRecepcionExcel')data={IdCarga:id,ReferenciaRecepcion:'synthetic-reference'};
   else if(operation==='Upload')data=carga('RECIBIDA');
   else if(operation==='Preparar'){
    if(body.Hoja==='Otra')configuration='f'.repeat(64);data=prepared();
    if(options.incidents){ok=false;code='LECTURA_INVALIDA';data.TotalIncidencias=30;data.IncidenciasTruncadas=true;data.Incidencias=[{Codigo:'VALOR_INVALIDO',Hoja:'Datos',Fila:2,Columna:1,CampoFuente:'Código',Mensaje:'private provider message'}];data.ValidacionCompleta=false;data.Carga.Estado='RECIBIDA';}
   }
   else if(operation==='Confirmar'){
    if(options.confirmGate)await options.confirmGate;confirmed=true;
    if(options.ignoreConfirm)return route.abort('failed');data=carga('CONFIRMADA');
   }
   else if(operation==='ConsultarResultado')data=carga(confirmed?'CONFIRMADA':'PREPARADA');
   else throw Error('Operación local no prevista '+operation);
   const payload={ok,codigo:code,mensaje:'fixture',datos:data};
   await route.fulfill({status:200,contentType:'application/json',body:JSON.stringify(operation==='Upload'?payload:{d:payload})});
  });
  await page.goto('https://doc126.invalid/production');
  for(const file of ['js/ui/jquery-3.4.1.min.js','js/gestion/ImportacionCarpetas/ImportacionCarpetas.js','js/gestion/ImportacionExcel/ImportacionExcelApi.js','js/gestion/ImportacionExcel/ImportacionExcelVista.js'])await page.addScriptTag({content:fs.readFileSync(path.join(root,file),'utf8')});
  await page.addStyleTag({content:fs.readFileSync(path.join(root,'Styles/ImportacionExcel.css'),'utf8')});
  await work(page,calls,{template,carga,prepared});
  assert.deepEqual(errors,[],'Sin errores de JavaScript del producto');
 }finally{await browser.close();}
}
async function open(page){await page.locator('[data-doc126=abrir]').click();await page.locator('[data-doc126=plantilla] option[value="29"]').waitFor({state:'attached'});}
async function select(page){await open(page);await page.locator('[data-doc126=plantilla]').selectOption('29');await page.locator('[data-doc126=destino]').filter({hasText:'imp_02_fixture'}).waitFor();}
async function prepare(page){await select(page);await page.locator('[data-doc126=archivo]').setInputFiles({name:'fixture.xlsx',mimeType:'application/octet-stream',buffer:Buffer.from('synthetic workbook transport')});await page.locator('[data-doc126=recibir]').click();await page.locator('[data-doc126=hoja] option[value=Datos]').waitFor({state:'attached'});await page.locator('[data-doc126=preparar]').click();await page.locator('[data-doc126=preview] table').waitFor();}
for(const mode of ['CREAR','AGREGAR','REEMPLAZAR'])test('DOC126 UI local recorre '+mode+' con muestra tipada y una sola confirmación',()=>fixture(async(page,calls)=>{
 await prepare(page);assert.match(await page.locator('[data-doc126=limites]').innerText(),/20000.*10 MB/);
 const preview=await page.locator('[data-doc126=preview]').innerText();assert.match(preview,/00017/);assert.match(preview,/123456789012345678901234567890123456789\.123/);assert.match(preview,/2025-01-02/);assert.match(preview,/NULL/);assert.match(preview,/<img/);assert.equal(await page.locator('[data-doc126=preview] img').count(),0);
 if(mode==='REEMPLAZAR'){assert.equal(await page.locator('[data-doc126=confirmar]').isDisabled(),true);await page.locator('[data-doc126=consentimiento]').check();}
 await page.locator('[data-doc126=confirmar]').evaluate(button=>{button.click();button.click();});
 await page.getByRole('heading',{name:'Carga confirmada',exact:true}).waitFor();assert.equal(calls.filter(c=>c.operation==='Confirmar').length,1);assert.equal(calls.find(c=>c.operation==='Confirmar').body.ConfirmacionReemplazo,mode==='REEMPLAZAR');
 assert.match(await page.locator('[data-doc126=resultado]').innerText(),/1000 filas confirmadas/);
 const stored=await page.evaluate(()=>Object.keys(sessionStorage).map(k=>sessionStorage.getItem(k)));assert.equal(stored.length,1);assert.deepEqual(JSON.parse(stored[0]),[{IdCarga:id,ConexionId:6}]);
},{mode}));
test('DOC126 incidencias de preparación ok=false se muestran sin permitir confirmar ni mostrar mensajes privados',()=>fixture(async page=>{
 await prepare(page);assert.equal(await page.locator('[data-doc126=confirmar]').isDisabled(),true);
 const message=await page.locator('[data-doc126=incidencias]').innerText();assert.match(message,/30 en total/);assert.match(message,/fila 2.*columna 1/);assert.match(message,/más incidencias/);assert.doesNotMatch(message,/private provider/);
},{incidents:true}));
test('DOC126 cambiar hoja invalida consentimiento y conserva CAS para volver a preparar',()=>fixture(async(page,calls)=>{
 await prepare(page);await page.locator('[data-doc126=consentimiento]').check();await page.locator('[data-doc126=hoja]').selectOption('Otra');assert.equal(await page.locator('[data-doc126=confirmar]').isDisabled(),true);
 await page.locator('[data-doc126=preparar]').click();await page.locator('[data-doc126=preview] table').waitFor();
 assert.equal(calls.filter(c=>c.operation==='Preparar').at(-1).body.PreparacionHashEsperada,'b'.repeat(64));assert.equal(await page.locator('[data-doc126=consentimiento]').isChecked(),false);
},{mode:'REEMPLAZAR'}));
test('DOC126 acuse perdido se consulta por ID y una recarga no reconstruye una intención confirmable',()=>fixture(async(page,calls)=>{
 await prepare(page);await page.locator('[data-doc126=confirmar]').click();await page.waitForFunction(()=>document.querySelector('[data-doc126=estado]').getAttribute('data-state')==='RESULTADO_INCIERTO');
 await page.locator('[data-doc126=tab-consulta]').click();await page.locator('[data-doc126=consultar]').click();await page.getByRole('heading',{name:'Carga confirmada',exact:true}).waitFor();assert.equal(calls.filter(c=>c.operation==='Confirmar').length,1);
 await page.locator('[data-doc126=cerrar]').click();
 await page.evaluate(()=>{const old=document.getElementById('doc126-excel'),next=old.cloneNode(false);old.replaceWith(next);ImportacionExcelVista.inicializar();});
 assert.equal(await page.locator('[data-doc126=abrir]').count(),1);await open(page);await page.locator('[data-doc126=tab-consulta]').click();await page.locator('[data-doc126=recientes]').selectOption(id);await page.locator('[data-doc126=consultar]').click();await page.getByRole('heading',{name:'Carga confirmada',exact:true}).waitFor();assert.equal(calls.filter(c=>c.operation==='Confirmar').length,1);
},{ignoreConfirm:true}));
test('DOC126 cerrar descarta respuesta tardía y reabrir conserva consulta sin cancelar servidor',async()=>{
 let release;const gate=new Promise(r=>{release=r;});
 await fixture(async(page,calls)=>{
  await prepare(page);await page.locator('[data-doc126=confirmar]').click();await page.waitForFunction(()=>document.querySelector('[data-doc126=estado]').getAttribute('data-state')==='CONFIRMANDO');
  await page.locator('[data-doc126=cerrar]').click();await page.locator('[data-doc126=abrir]').click();release();await page.waitForFunction(()=>document.querySelector('[role=dialog]').getAttribute('aria-busy')==='false');
  assert.equal(await page.getByRole('heading',{name:'Carga confirmada',exact:true}).count(),0);assert.equal(await page.locator('[data-doc126=confirmar]').isDisabled(),true);
  await page.locator('[data-doc126=tab-consulta]').click();await page.locator('[data-doc126=consultar]').click();await page.getByRole('heading',{name:'Carga confirmada',exact:true}).waitFor();assert.equal(calls.filter(c=>c.operation==='Confirmar').length,1);
 },{confirmGate:gate});
});
test('DOC126 editor conserva IDs/null, guarda con CAS y no permite retry automático de un alta incierta',()=>fixture(async(page,calls)=>{
 await select(page);await page.locator('[data-doc126=tab-plantillas]').click();await page.locator('[data-doc126=tabla]').fill('imp_02_editada');await page.locator('[data-doc126=guardar]').click();
 await page.locator('[data-doc126=destino]').filter({hasText:'imp_02_editada'}).waitFor();const saved=calls.find(c=>c.operation==='GuardarPlantilla').body;
 assert.equal(saved.RevisionEsperada,1);assert.equal(saved.HuellaEsperada,'a'.repeat(64));assert.equal(saved.Plantilla.Campos[0].ID_CAMPOS_PLANTILLA,10);assert.equal(saved.Plantilla.Campos[0].REMPLAZA_COMA,null);assert.equal(saved.Plantilla.Campos[0].REMPLAZA_PUNTO,0);assert.equal(saved.Plantilla.Campos[0].NUMERO_MATRIZ_EXPLITER,null);
}));
test('DOC126 conflicto de edición mantiene el formulario sin sobrescribir',()=>fixture(async(page,calls)=>{
 await select(page);await page.locator('[data-doc126=tab-plantillas]').click();await page.locator('[data-doc126=tabla]').fill('imp_02_editada');await page.locator('[data-doc126=guardar]').click();await page.locator('[data-doc126=estado]').filter({hasText:'La plantilla cambió'}).waitFor();assert.equal(await page.locator('[data-doc126=tabla]').inputValue(),'imp_02_editada');assert.equal(calls.filter(c=>c.operation==='GuardarPlantilla').length,1);
},{conflictSave:true}));
test('DOC126 guardado incierto bloquea repetición y conserva formulario',()=>fixture(async(page,calls)=>{
 await select(page);await page.locator('[data-doc126=tab-plantillas]').click();await page.locator('[data-doc126=guardar]').click();await page.waitForFunction(()=>document.querySelector('[data-doc126=guardar]').disabled);await page.locator('[data-doc126=estado]').filter({hasText:'No se conoce si la plantilla se guardó'}).waitFor();assert.equal(calls.filter(c=>c.operation==='GuardarPlantilla').length,1);assert.equal(await page.locator('[data-doc126=tabla]').inputValue(),'imp_02_fixture');
},{ignoreSave:true}));
test('DOC126 almacenamiento rechazado conserva ID manual y avisa sin perder el flujo',()=>fixture(async page=>{
 await prepare(page);assert.equal(await page.locator('[data-doc126=storage-aviso]').isVisible(),true);assert.match(await page.locator('[data-doc126=referencia]').innerText(),new RegExp(id));assert.equal(await page.locator('[data-doc126=confirmar]').isDisabled(),false);
},{noStorage:true}));
test('DOC126 acceso exclusivo y montaje repetido conservan un menú y foco; viewport pequeño y zoom',()=>fixture(async(page,calls)=>{
 await page.evaluate(()=>{ImportacionExcelVista.inicializar();ImportacionExcelVista.inicializar();});assert.equal(await page.locator('#doc110-navigation').count(),1);assert.equal(await page.locator('[data-doc126=abrir]').count(),1);
 await open(page);await page.waitForFunction(()=>document.querySelector('[role=dialog]').getAttribute('aria-busy')==='false');
 for(const viewport of [{width:320,height:568},{width:768,height:1024},{width:1280,height:720}]){
  await page.setViewportSize(viewport);for(const zoom of ['1','2']){await page.evaluate(z=>document.documentElement.style.zoom=z,zoom);const box=await page.locator('[role=dialog]').boundingBox();assert.ok(box.x>=-1&&box.y>=-1&&box.x+box.width<=viewport.width+1&&box.y+box.height<=viewport.height+1,JSON.stringify({viewport,zoom,box}));await page.locator('[data-doc126=confirmar]').scrollIntoViewIfNeeded();const bounds=await page.locator('[data-doc126=confirmar]').evaluate(e=>{const a=e.getBoundingClientRect(),b=e.closest('.doc126-body').getBoundingClientRect();return {a:{top:a.top,bottom:a.bottom},b:{top:b.top,bottom:b.bottom}};});assert.ok(bounds.a.top>=bounds.b.top-1&&bounds.a.bottom<=bounds.b.bottom+1,JSON.stringify({viewport,zoom,bounds}));await page.locator('[data-doc126=cerrar]').focus();await page.keyboard.press('Shift+Tab');assert.equal(await page.evaluate(()=>document.querySelector('[role=dialog]').contains(document.activeElement)),true);}
 }
 await page.evaluate(()=>document.documentElement.style.zoom='1');await page.keyboard.press('Escape');assert.equal(await page.locator('[role=dialog]').isVisible(),false);assert.equal(calls.filter(c=>c.operation==='Confirmar').length,0);
}));

test('DOC126 plantilla inactiva no recibe archivos aunque el usuario tenga permiso',()=>fixture(async(page,calls)=>{
 await select(page);await page.locator('[data-doc126=archivo]').setInputFiles({name:'fixture.xlsx',mimeType:'application/octet-stream',buffer:Buffer.from('synthetic')});assert.equal(await page.locator('[data-doc126=recibir]').isDisabled(),true);assert.match(await page.locator('[data-doc126=estado]').innerText(),/activa/);assert.equal(calls.filter(c=>c.operation==='IniciarRecepcionExcel').length,0);
},{inactive:true}));

test('DOC126 capacidad cerrada conserva consulta sin listar ni recibir',()=>fixture(async(page,calls)=>{
 await page.locator('[data-doc126=abrir]').click();await page.waitForFunction(()=>document.querySelector('[role=dialog]').getAttribute('aria-busy')==='false');await page.locator('[data-doc126=id-carga]').fill(id);await page.locator('[data-doc126=consultar]').click();await page.locator('[data-doc126=resultado] h3').waitFor();assert.equal(calls.some(c=>['ListarPlantillas','IniciarRecepcionExcel','Confirmar'].includes(c.operation)),false);
},{closedCapacity:true}));

test('DOC126 endRequest se registra una vez y reemplaza sólo su propio host',()=>fixture(async(page)=>{
 await page.evaluate(()=>{window.manager={count:0,add_endRequest(fn){this.fn=fn;this.count++;}};window.Sys={WebForms:{PageRequestManager:{getInstance:()=>window.manager}}};ImportacionExcelVista.inicializar();ImportacionExcelVista.inicializar();const old=document.getElementById('doc126-excel');old.replaceWith(old.cloneNode(false));manager.fn();});assert.equal(await page.evaluate(()=>manager.count),1);assert.equal(await page.locator('[data-doc126=abrir]').count(),1);assert.equal(await page.locator('#doc110-navigation').count(),1);
}));

test('DOC126 posición fraccionaria no se redondea ni se guarda',()=>fixture(async(page,calls)=>{
 await select(page);await page.locator('[data-doc126=tab-plantillas]').click();await page.locator('[data-field=NUMERO_MATRIZ_EXPLITER]').fill('1.5');await page.locator('[data-doc126=guardar]').click();assert.match(await page.locator('[data-doc126=estado]').innerText(),/entero no negativo/);assert.equal(calls.filter(c=>c.operation==='GuardarPlantilla').length,0);
}));
