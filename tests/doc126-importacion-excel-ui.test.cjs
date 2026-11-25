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
  const carga=state=>({IdCarga:id,PlantillaId:template.ID_PLANTILLAIMP,Estado:state,Tabla:template.NOMBRE_PLANTILLA,Modo:template.Modo,ArchivoHash:options.changedFile?'9'.repeat(64):'a'.repeat(64),ConfiguracionHash:configuration,EsquemaHash:'c'.repeat(64),FilasPrevistas:1000,FilasConfirmadas:state==='CONFIRMADA'?1000:0,ConfirmadoUtc:'/Date(1735776000000)/',Hojas:['Datos','Otra']});
  const prepared=()=>({Carga:carga('PREPARADA'),LecturaCompleta:true,ValidacionCompleta:true,DuplicadosEvaluados:true,FilasObservadas:1000,TotalIncidencias:0,Incidencias:[],Muestra:[{Fila:2,Valores:[{CampoDestino:'CLAVE',Tipo:'TEXTO',Texto:'00017'},{CampoDestino:'NUMERO',Tipo:'DECIMAL',NumeroCanonico:'123456789012345678901234567890123456789.123'},{CampoDestino:'FECHA',Tipo:'DATE',Fecha:'/Date(1735776000000)/'},{CampoDestino:'VACIO',Tipo:'TEXTO',EsNulo:true},{CampoDestino:'HTML',Tipo:'TEXTO',Texto:'<img src=x onerror=window.pwned=true>'}]}]});
  if(options.noStorage)await page.addInitScript(()=>Object.defineProperty(window,'sessionStorage',{get(){throw Error('synthetic storage denial');}}));
  await page.route('https://doc126.invalid/**',async route=>{
   const request=route.request();if(request.method()==='GET')return route.fulfill({status:200,contentType:'text/html',body:'<!doctype html><html><body><form><nav id="menu_var"><ul class="navbar-nav"></ul></nav><div id="doc126-excel" data-api="/excel.asmx" data-upload="/upload.ashx" '+(options.assistant?'data-analisis="/analysis.ashx" ':'')+'data-recuperacion="'+'d'.repeat(32)+'"></div></form></body></html>'});
   const routeName=new URL(request.url()).pathname.split('/').pop();
   const operation=routeName==='upload.ashx'?'Upload':routeName==='analysis.ashx'?'Analizar':routeName;
   const body=['Upload','Analizar'].includes(operation)?null:request.postDataJSON().solicitud;
   calls.push({operation,body,multipart:body===null?request.postData():null});
   assert.equal(request.headers()['x-importacion-excel-contexto'],'d'.repeat(32));
   if(operation!=='AbrirCargaExcel')assert.equal(request.headers()['x-importacion-excel-csrf'],'synthetic-ui-token');
   let data,ok=true,code='OK',validation=[],totalValidation=0;
   if(operation==='AbrirCargaExcel')data={CsrfToken:'synthetic-ui-token',Conexiones:[{ConexionId:6,Nombre:'Fixture autorizada'}],PermiteNuevaCarga:!options.closedCapacity,MaxFilasDatos:20000,MaxArchivoMB:10};
   else if(operation==='ListarPlantillas')data={Total:1,Pagina:1,TamanoPagina:25,Plantillas:[{PlantillaId:29,Nombre:template.NOMBRE_PLANTILLA,Activa:true}]};
   else if(operation==='ConsultarPlantilla'){
    if(options.detailGate)await options.detailGate;
    if(confirmed&&options.failRefresh){ok=false;code='DESTINO_INVALIDO';}
    else {if(confirmed&&options.createThenAdd)template.Modo='AGREGAR';data=clone(template);}
   }
   else if(operation==='GuardarPlantilla'){
    if(options.configErrors&&(!options.errorsOnce||calls.filter(c=>c.operation==='GuardarPlantilla').length===1)){ok=false;code='CONFIGURACION_INVALIDA';validation=options.configErrors;totalValidation=options.validationTotal||validation.length;}
    else if(options.conflictSave){ok=false;code='CONFLICTO_REVISION';}
    else if(options.ignoreSave){return route.abort('failed');}
    else {Object.assign(template,body.Plantilla,{ID_PLANTILLAIMP:body.Plantilla.ID_PLANTILLAIMP||30,REVISION_CARGA:2,Huella:'e'.repeat(64)});data=clone(template);}
   }
   else if(operation==='Analizar'){
    if(options.analysisGate)await options.analysisGate;
    if(options.analysisFailure){ok=false;code='ENCABEZADO_AMBIGUO';}
    else data={Formato:'XLSX',Hoja:request.postData().includes('\r\nOtra\r\n')?'Otra':'Datos',Hojas:['Datos','Otra'],ArchivoHash:'a'.repeat(64),FilasObservadas:2,Campos:[{CAMPO_FUENTE:'Código',CAMPO_DESTINO:'CLAVE',TIPO_CAMPO:'VARCHAR(100)'},{CAMPO_FUENTE:'Nombre',CAMPO_DESTINO:'NOMBRE',TIPO_CAMPO:'VARCHAR(250)'}],Muestra:[['00017','<img src=x onerror=window.pwned=true>'],[null,'Texto']],Avisos:['PROPUESTA_REQUIERE_REVISION']};
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
   const payload={ok,codigo:code,mensaje:'private provider detail must not escape',datos:data,validacion:validation,totalValidacion:totalValidation};
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
const stepTones=page=>page.locator('[data-doc126-step]').evaluateAll(items=>items.map(e=>e.getAttribute('data-tone')));
test('DOC126 semáforo guía cada acción y reutiliza CREAR como AGREGAR tras consultar la misma plantilla',()=>fixture(async(page,calls)=>{
 await open(page);assert.deepEqual(await stepTones(page),['amber','gray','gray','gray']);
 await page.locator('[data-doc126=plantilla]').selectOption('29');await page.locator('[data-doc126=destino]').filter({hasText:'imp_02_fixture'}).waitFor();
 assert.deepEqual(await stepTones(page),['green','amber','gray','gray']);
 await page.locator('[data-doc126=archivo]').setInputFiles({name:'fixture.xlsx',mimeType:'application/octet-stream',buffer:Buffer.from('synthetic')});
 await page.locator('[data-doc126=siguiente-paso]').click();await page.locator('[data-doc126=hoja] option[value=Datos]').waitFor({state:'attached'});
 assert.deepEqual(await stepTones(page),['green','green','amber','gray']);
 await page.locator('[data-doc126=siguiente-paso]').click();await page.locator('[data-doc126=preview] table').waitFor();
 assert.deepEqual(await stepTones(page),['green','green','green','amber']);
 await page.locator('[data-doc126=siguiente-paso]').click();assert.equal(await page.locator('[data-doc126=confirmar]').evaluate(e=>e===document.activeElement),true);
 assert.equal(calls.filter(c=>c.operation==='Confirmar').length,0);
 await page.locator('[data-doc126=confirmar]').click();await page.getByRole('heading',{name:'Carga confirmada',exact:true}).waitFor();
 assert.deepEqual(await stepTones(page),['green','green','green','green']);assert.equal(await page.locator('[data-doc126=archivo]').isDisabled(),true);
 assert.equal(await page.locator('[data-doc126=hoja]').isDisabled(),true);assert.equal(await page.locator('[data-doc126=preparar]').isDisabled(),true);
 await page.locator('[data-doc126=siguiente-paso]').click();await page.locator('[data-doc126=estado]').filter({hasText:'Plantilla actualizada. Operación: Agregar filas'}).waitFor();
 assert.equal(await page.locator('[data-doc126=modo]').inputValue(),'AGREGAR');assert.equal(await page.locator('[data-doc126=archivo]').inputValue(),'');
 assert.deepEqual(await stepTones(page),['green','amber','gray','gray']);assert.equal(calls.filter(c=>c.operation==='GuardarPlantilla').length,0);
 assert.equal(calls.filter(c=>c.operation==='ConsultarPlantilla').length,2);assert.ok(calls.filter(c=>c.operation==='ConsultarPlantilla').every(c=>c.body.PlantillaId===29));
 assert.equal(calls.filter(c=>c.operation==='Upload').length,1);assert.equal(calls.filter(c=>c.operation==='Confirmar').length,1);
 assert.match(await page.locator('[data-doc126=resultado]').innerText(),/1000 filas confirmadas/);
},{mode:'CREAR',createThenAdd:true}));
test('DOC126 cambiar operación consulta y edita el mismo ID con CAS sin crear otra plantilla ni importar',()=>fixture(async(page,calls)=>{
 await select(page);await page.locator('[data-doc126=cambiar-operacion]').click();await page.locator('[data-doc126=editor-modo]').waitFor({state:'visible'});
 assert.equal(await page.locator('[data-doc126=editor-modo]').evaluate(e=>e===document.activeElement),true);
 await page.locator('[data-doc126=editor-modo]').selectOption('REEMPLAZAR');await page.locator('[data-doc126=guardar]').click();
 await page.locator('[data-doc126=estado]').filter({hasText:'Plantilla guardada'}).waitFor();
 const saves=calls.filter(c=>c.operation==='GuardarPlantilla');assert.equal(saves.length,1);assert.equal(saves[0].body.Plantilla.ID_PLANTILLAIMP,29);
 assert.equal(saves[0].body.Plantilla.Modo,'REEMPLAZAR');assert.equal(saves[0].body.RevisionEsperada,1);assert.equal(saves[0].body.HuellaEsperada,'a'.repeat(64));
 assert.equal(saves[0].body.Plantilla.Campos[0].ID_CAMPOS_PLANTILLA,10);assert.equal(saves[0].body.Plantilla.Campos[0].REMPLAZA_COMA,null);
 assert.equal(calls.some(c=>['IniciarRecepcionExcel','Upload','Preparar','Confirmar'].includes(c.operation)),false);
}));
test('DOC126 guía de reemplazo lleva al consentimiento sin aceptarlo ni confirmar automáticamente',()=>fixture(async(page,calls)=>{
 await prepare(page);await page.locator('[data-doc126=siguiente-paso]').click();assert.equal(await page.locator('[data-doc126=consentimiento]').evaluate(e=>e===document.activeElement),true);
 assert.equal(await page.locator('[data-doc126=consentimiento]').isChecked(),false);assert.equal(await page.locator('[data-doc126=confirmar]').isDisabled(),true);
 await page.locator('[data-doc126=consentimiento]').check();await page.locator('[data-doc126=siguiente-paso]').click();assert.equal(await page.locator('[data-doc126=confirmar]').evaluate(e=>e===document.activeElement),true);
 assert.deepEqual(await stepTones(page),['green','green','green','amber']);assert.equal(calls.filter(c=>c.operation==='Confirmar').length,0);
},{mode:'REEMPLAZAR'}));
test('DOC126 incidencias ponen validación en rojo sin marcar confirmación completada',()=>fixture(async(page,calls)=>{
 await prepare(page);assert.deepEqual(await stepTones(page),['green','green','red','gray']);
 assert.match(await page.locator('[data-doc126=guia-mensaje]').innerText(),/Revisa el mensaje y las incidencias/);
 await page.locator('[data-doc126=siguiente-paso]').click();assert.equal(await page.locator('[data-doc126=incidencias]').evaluate(e=>e===document.activeElement),true);
 assert.equal(calls.filter(c=>c.operation==='Preparar').length,1);
 assert.equal(await page.locator('[data-doc126=confirmar]').isDisabled(),true);assert.equal(calls.filter(c=>c.operation==='Confirmar').length,0);
},{incidents:true}));
test('DOC126 incertidumbre guía a consultar el mismo ID sin nueva carga ni replay',()=>fixture(async(page,calls)=>{
 await prepare(page);await page.locator('[data-doc126=confirmar]').click();await page.locator('[data-doc126=siguiente-paso]').filter({hasText:'Consultar resultado pendiente'}).waitFor();
 assert.deepEqual(await stepTones(page),['green','green','green','red']);await page.locator('[data-doc126=siguiente-paso]').click();
 assert.equal(await page.locator('[data-doc126=consultar]').evaluate(e=>e===document.activeElement),true);assert.equal(await page.locator('[data-doc126=id-carga]').inputValue(),id);
 assert.equal(calls.filter(c=>c.operation==='ConsultarResultado').length,0);assert.equal(calls.filter(c=>c.operation==='Confirmar').length,1);
 await page.locator('[data-doc126=consultar]').click();await page.getByRole('heading',{name:'Carga confirmada',exact:true}).waitFor();assert.deepEqual(await stepTones(page),['green','green','green','green']);
},{ignoreConfirm:true}));
test('DOC126 fallo al refrescar nueva importación impide reutilizar el modo CREAR obsoleto',()=>fixture(async(page,calls)=>{
 await prepare(page);await page.locator('[data-doc126=confirmar]').click();await page.getByRole('heading',{name:'Carga confirmada',exact:true}).waitFor();
 await page.locator('[data-doc126=siguiente-paso]').click();await page.locator('[data-doc126=estado]').filter({hasText:'El destino no es compatible'}).waitFor();
 assert.equal(await page.locator('[data-doc126=recibir]').isDisabled(),true);assert.deepEqual(await stepTones(page),['red','gray','gray','gray']);
 assert.equal(calls.filter(c=>c.operation==='IniciarRecepcionExcel').length,1);assert.equal(calls.filter(c=>c.operation==='GuardarPlantilla').length,0);
},{mode:'CREAR',failRefresh:true}));
test('DOC126 consultar referencia histórica sin archivo actual no completa los pasos de otra importación',()=>fixture(async(page,calls)=>{
 await open(page);await page.locator('[data-doc126=tab-consulta]').click();await page.locator('[data-doc126=id-carga]').fill(id);await page.locator('[data-doc126=consultar]').click();
 await page.locator('[data-doc126=resultado] h3').waitFor();assert.deepEqual(await stepTones(page),['amber','gray','gray','gray']);
 assert.equal(calls.some(c=>['Upload','Preparar','Confirmar'].includes(c.operation)),false);
}));
test('DOC126 guía y siguiente acción permanecen dentro del diálogo con 320px y zoom 200%',()=>fixture(async page=>{
 await select(page);await page.evaluate(()=>document.documentElement.style.zoom='2');
 await page.locator('[data-doc126=siguiente-paso]').scrollIntoViewIfNeeded();
 const bounds=await page.locator('[data-doc126=guia]').evaluate(e=>{const outer=e.closest('.doc126-body').getBoundingClientRect();return {left:outer.left,right:outer.right,items:Array.from(e.querySelectorAll('li,button')).map(n=>{const b=n.getBoundingClientRect();return {left:b.left,right:b.right};})};});
 assert.ok(bounds.items.every(b=>b.left>=bounds.left-1&&b.right<=bounds.right+1),JSON.stringify(bounds));
},{viewport:{width:320,height:568}}));
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

async function analyzeNew(page,filename='propuesta.xlsx'){
 await open(page);await page.locator('[data-doc126=crear-desde-excel]').click();
 await page.locator('[data-doc126=analisis-archivo]').setInputFiles({name:filename,mimeType:'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',buffer:Buffer.from('same synthetic workbook bytes')});
 await page.locator('[data-doc126=analizar]').click();await page.locator('[data-doc126=analisis-clave] option[value=CLAVE]').waitFor({state:'attached'});
}
async function saveAnalysis(page){await page.locator('[data-doc126=tabla]').fill('imp_02_nueva');await page.locator('[data-doc126=analisis-clave]').selectOption('CLAVE');await page.locator('[data-doc126=guardar]').click();await page.locator('[data-doc126=archivo-conservado]').filter({hasText:'propuesta.xlsx'}).waitFor();}

test('DOC126 asistente propone campos y guarda sin importar; recepción explícita conserva el File',()=>fixture(async(page,calls)=>{
 await analyzeNew(page);assert.equal(await page.locator('[data-field=CAMPO_FUENTE]').first().inputValue(),'Código');assert.equal(await page.locator('[data-field=TIPO_CAMPO]').first().inputValue(),'VARCHAR(100)');
 assert.match(await page.locator('[data-doc126=analisis-preview]').innerText(),/00017/);assert.equal(await page.locator('[data-doc126=analisis-preview] img').count(),0);assert.equal(await page.evaluate(()=>window.pwned),undefined);
 await page.setViewportSize({width:320,height:568});await page.locator('[data-doc126=analisis-preview] .doc126-table-scroll').scrollIntoViewIfNeeded();assert.equal(await page.locator('[data-doc126=analisis-preview] .doc126-table-scroll').evaluate(e=>e.getBoundingClientRect().width<=e.closest('.doc126-body').clientWidth),true);await page.setViewportSize({width:1280,height:720});
 assert.equal(calls.some(c=>['GuardarPlantilla','IniciarRecepcionExcel','Upload','Confirmar'].includes(c.operation)),false);
 await saveAnalysis(page);const save=calls.find(c=>c.operation==='GuardarPlantilla');assert.equal(save.body.RevisionEsperada,0);assert.equal(save.body.Plantilla.CAMPO_UNICO,'CLAVE');assert.equal(save.body.Plantilla.Campos.length,2);
 assert.equal(calls.some(c=>c.operation==='Upload'),false);assert.equal(await page.locator('[data-doc126=archivo]').inputValue(),'');assert.equal(await page.locator('[data-doc126=recibir]').isEnabled(),true);
 await page.locator('[data-doc126=recibir]').dblclick();await page.locator('[data-doc126=hoja] option[value=Datos]').waitFor({state:'attached'});
 assert.equal(calls.filter(c=>c.operation==='Upload').length,1);for(const c of calls.filter(c=>['Upload','Analizar'].includes(c.operation))){assert.match(c.multipart,/filename="propuesta.xlsx"/);assert.match(c.multipart,/same synthetic workbook bytes/);}
 await page.locator('[data-doc126=preparar]').click();await page.waitForFunction(()=>document.querySelector('[data-doc126=estado]').getAttribute('data-state')==='PREPARADO');assert.equal(calls.some(c=>c.operation==='Confirmar'),false);
},{assistant:true}));

test('DOC126 cambiar archivo invalida propuesta y exige analizar de nuevo antes de guardar',()=>fixture(async(page,calls)=>{
 await analyzeNew(page);await page.locator('[data-doc126=tabla]').fill('imp_02_nueva');await page.locator('[data-doc126=analisis-clave]').selectOption('CLAVE');
 await page.locator('[data-doc126=analisis-archivo]').setInputFiles({name:'otro.xlsx',buffer:Buffer.from('different synthetic workbook')});await page.locator('[data-doc126=guardar]').click();assert.match(await page.locator('[data-doc126=estado]').innerText(),/Analiza el archivo/);assert.equal(calls.some(c=>c.operation==='GuardarPlantilla'),false);
 page.on('dialog',d=>d.accept());await page.locator('[data-doc126=analizar]').click();await page.locator('[data-doc126=analisis-clave] option[value=CLAVE]').waitFor({state:'attached'});assert.equal(calls.filter(c=>c.operation==='Analizar').length,2);assert.equal(await page.locator('[data-doc126=clave]').inputValue(),'');
},{assistant:true}));

test('DOC126 hoja distinta vuelve a analizar y exige renovar la clave',()=>fixture(async(page,calls)=>{
 await analyzeNew(page);await page.locator('[data-doc126=analisis-clave]').selectOption('CLAVE');page.on('dialog',d=>d.accept());await page.locator('[data-doc126=analisis-hoja]').selectOption('Otra');await page.waitForFunction(()=>document.querySelector('[data-doc126=libro]').value==='Otra');assert.equal(await page.locator('[data-doc126=clave]').inputValue(),'');assert.equal(calls.filter(c=>c.operation==='Analizar').length,2);
},{assistant:true}));

test('DOC126 hash de recepción distinto bloquea preparación sin confirmar ni repetir upload',()=>fixture(async(page,calls)=>{
 await analyzeNew(page);await saveAnalysis(page);await page.locator('[data-doc126=recibir]').click();await page.locator('[data-doc126=estado]').filter({hasText:'distinto del analizado'}).waitFor();assert.equal(await page.locator('[data-doc126=preparar]').isDisabled(),true);assert.equal(await page.locator('[data-doc126=confirmar]').isDisabled(),true);assert.equal(calls.filter(c=>c.operation==='Upload').length,1);assert.equal(calls.some(c=>['Preparar','Confirmar'].includes(c.operation)),false);
},{assistant:true,changedFile:true}));

test('DOC126 cerrar durante análisis descarta respuesta tardía sin crear incertidumbre durable',async()=>{
 let release;const gate=new Promise(r=>{release=r;});
 await fixture(async(page,calls)=>{
  await open(page);await page.locator('[data-doc126=crear-desde-excel]').click();await page.locator('[data-doc126=analisis-archivo]').setInputFiles({name:'propuesta.xlsx',buffer:Buffer.from('synthetic')});await page.locator('[data-doc126=analizar]').dblclick();await page.waitForFunction(()=>document.querySelector('[role=dialog]').getAttribute('aria-busy')==='true');await page.locator('[data-doc126=cerrar]').click();release();await open(page);await page.waitForFunction(()=>document.querySelector('[role=dialog]').getAttribute('aria-busy')==='false');assert.equal(await page.locator('[data-doc126=analisis-clave] option[value=CLAVE]').count(),0);assert.equal(calls.filter(c=>c.operation==='Analizar').length,1);assert.doesNotMatch(await page.locator('[data-doc126=estado]').innerText(),/No se conoce el resultado/);assert.equal(calls.some(c=>c.operation==='GuardarPlantilla'),false);
 },{assistant:true,analysisGate:gate});
});

test('DOC126 error de análisis permite corregir el archivo y conserva edición manual',()=>fixture(async(page,calls)=>{
 await open(page);await page.locator('[data-doc126=crear-desde-excel]').click();await page.locator('[data-doc126=analisis-archivo]').setInputFiles({name:'bad.xlsx',buffer:Buffer.from('synthetic')});await page.locator('[data-doc126=analizar]').click();await page.locator('[data-doc126=estado]').filter({hasText:'encabezados repetidos'}).waitFor();assert.equal(calls.filter(c=>c.operation==='Analizar').length,1);assert.equal(calls.some(c=>c.operation==='IniciarRecepcionExcel'),false);
 await page.locator('[data-doc126=analisis-archivo]').setInputFiles([]);await page.locator('[data-doc126=tabla]').fill('imp_02_manual');await page.locator('[data-doc126=clave]').fill('CLAVE');await page.locator('[data-field=CAMPO_FUENTE]').fill('Código');await page.locator('[data-field=CAMPO_DESTINO]').fill('CLAVE');await page.locator('[data-field=TIPO_CAMPO]').fill('VARCHAR(100)');await page.locator('[data-doc126=guardar]').click();await page.locator('[data-doc126=destino]').filter({hasText:'imp_02_manual'}).waitFor();assert.equal(calls.filter(c=>c.operation==='GuardarPlantilla').length,1);
},{assistant:true,analysisFailure:true}));

test('DOC126 editar o quitar un destino renueva las opciones de campo único',()=>fixture(async(page)=>{
 await analyzeNew(page);await page.locator('[data-doc126=analisis-clave]').selectOption('CLAVE');await page.locator('[data-field=CAMPO_DESTINO]').first().fill('ORDEN');assert.equal(await page.locator('[data-doc126=clave]').inputValue(),'');await page.locator('[data-doc126=analisis-clave]').selectOption('ORDEN');assert.equal(await page.locator('[data-doc126=clave]').inputValue(),'ORDEN');await page.locator('[data-doc126=quitar-campo]').first().click();assert.equal(await page.locator('[data-doc126=clave]').inputValue(),'');assert.equal(await page.locator('[data-doc126=analisis-clave] option[value=ORDEN]').count(),0);
},{assistant:true}));

test('DOC126 propuesta no persiste el libro y recrear host obliga a seleccionarlo nuevamente',()=>fixture(async(page)=>{
 await analyzeNew(page);await saveAnalysis(page);assert.equal(await page.locator('[data-doc126=recibir]').isEnabled(),true);assert.doesNotMatch(await page.evaluate(()=>JSON.stringify(Object.values(sessionStorage))),/propuesta.xlsx|synthetic workbook/);
 await page.locator('[data-doc126=cerrar]').click();await page.evaluate(()=>{const old=document.getElementById('doc126-excel');old.replaceWith(old.cloneNode(false));ImportacionExcelVista.inicializar();});await open(page);await page.locator('[data-doc126=plantilla]').selectOption('29');await page.locator('[data-doc126=destino]').filter({hasText:'imp_02_nueva'}).waitFor();assert.equal(await page.locator('[data-doc126=archivo-conservado]').innerText(),'');assert.equal(await page.locator('[data-doc126=recibir]').isDisabled(),true);
},{assistant:true}));

test('DOC126 Plantillas ofrece análisis al crear y conserva el editor de una plantilla existente',()=>fixture(async(page,calls)=>{
 await open(page);await page.locator('[data-doc126=tab-plantillas]').click();assert.equal(await page.locator('[data-doc126=asistente]').isVisible(),true);
 await page.locator('[data-doc126=tab-carga]').click();await page.locator('[data-doc126=plantilla]').selectOption('29');await page.locator('[data-doc126=destino]').filter({hasText:'imp_02_fixture'}).waitFor();await page.locator('[data-doc126=tab-plantillas]').click();assert.equal(await page.locator('[data-doc126=asistente]').isVisible(),false);assert.equal(await page.locator('[data-field=CAMPO_FUENTE]').inputValue(),'Código');assert.equal(calls.some(c=>c.operation==='Analizar'),false);
},{assistant:true}));

test('DOC126 video crear plantilla: sugiere tabla desde RELACIÓN ORDENES y guarda al elegir clave',()=>fixture(async(page,calls)=>{
 await analyzeNew(page,'RELACIÓN ORDENES 2025.xlsx');assert.equal(await page.locator('[data-doc126=tabla]').inputValue(),'imp_02_relacion_ordenes_2025');
 assert.equal(await page.locator('[data-field=REMPLAZA_COMA]').first().inputValue(),'0');assert.equal(await page.locator('[data-field=REMPLAZA_PUNTO]').first().inputValue(),'0');assert.equal(await page.locator('[data-field=OPCION_SPLITER]').first().inputValue(),'0');assert.equal(await page.locator('[data-field=NUMERO_MATRIZ_EXPLITER]').first().inputValue(),'0');
 await page.locator('[data-doc126=analisis-clave]').selectOption('CLAVE');await page.locator('[data-doc126=guardar]').click();await page.locator('[data-doc126=destino]').filter({hasText:'imp_02_relacion_ordenes_2025'}).waitFor();
 assert.equal(calls.filter(c=>c.operation==='GuardarPlantilla').length,1);assert.equal(calls.some(c=>['Upload','Preparar','Confirmar'].includes(c.operation)),false);
},{assistant:true}));

test('DOC126 errores explican opción/fila/regla/corrección y permiten volver al control indicado',()=>fixture(async(page,calls)=>{
 await analyzeNew(page);await page.locator('[data-doc126=analisis-clave]').selectOption('CLAVE');await page.locator('[data-field=REMPLAZA_COMA]').first().selectOption('');await page.locator('[data-doc126=guardar]').click();
 const panel=page.locator('[data-doc126=diagnostico-configuracion]');await panel.waitFor();assert.match(await panel.innerText(),/Campo 1 · CLAVE — Cambiar coma/);assert.match(await panel.innerText(),/Sin configurar.*instalación exige/);assert.match(await panel.innerText(),/Cómo corregirlo: Selecciona «No»/);assert.match(await panel.innerText(),/Escribe 0 si «Separar valor»/);assert.match(await panel.innerText(),/2 de 5 observaciones/);assert.doesNotMatch(await panel.innerText(),/private provider|provider-password/);
 await page.setViewportSize({width:320,height:568});await panel.getByRole('button',{name:'Corregir Cambiar coma',exact:true}).click();assert.equal(await page.locator('[data-field=REMPLAZA_COMA]').first().evaluate(e=>e===document.activeElement),true);assert.equal(await page.locator('[data-field=REMPLAZA_COMA]').first().getAttribute('aria-invalid'),'true');
 await page.locator('[data-field=REMPLAZA_COMA]').first().selectOption('0');await page.locator('[data-doc126=guardar]').click();await page.locator('[data-doc126=archivo-conservado]').filter({hasText:'propuesta.xlsx'}).waitFor();assert.equal(calls.filter(c=>c.operation==='GuardarPlantilla').length,2);assert.equal(await panel.isVisible(),false);
},{assistant:true,configErrors:[{Campo:'REMPLAZA_COMA',FilaCampo:1,CampoDestino:'CLAVE',Regla:'REQUERIDO',Mensaje:'provider-password'},{Campo:'NUMERO_MATRIZ_EXPLITER',FilaCampo:2,CampoDestino:'NOMBRE',Regla:'REQUERIDO'}],validationTotal:5,errorsOnce:true}));

test('DOC126 rechazo sin detalle reconoce la incertidumbre y conserva formulario sin inventar una causa',()=>fixture(async(page,calls)=>{
 await analyzeNew(page);await page.locator('[data-doc126=analisis-clave]').selectOption('CLAVE');await page.locator('[data-doc126=guardar]').click();const panel=page.locator('[data-doc126=diagnostico-configuracion]');await panel.waitFor();assert.match(await panel.innerText(),/No es posible afirmar una causa concreta/);assert.match(await panel.innerText(),/Consulta la lista y abre el detalle/);assert.doesNotMatch(await panel.innerText(),/No se guardaron los cambios|private provider/);assert.equal(await page.locator('[data-doc126=guardar]').isDisabled(),true);assert.equal(await page.locator('[data-doc126=tabla]').inputValue(),'imp_02_propuesta');assert.equal(calls.filter(c=>c.operation==='GuardarPlantilla').length,1);
},{assistant:true,configErrors:[{Campo:'__proto__',Regla:'REQUERIDO'}]}));

test('DOC126 tipo y longitud incompatibles explican los límites sin mostrar valores sensibles',()=>fixture(async(page)=>{
 await analyzeNew(page);await page.locator('[data-doc126=analisis-clave]').selectOption('CLAVE');await page.locator('[data-doc126=guardar]').click();const panel=page.locator('[data-doc126=diagnostico-configuracion]');await panel.waitFor();assert.match(await panel.innerText(),/máximo de 10 caracteres; se enviaron 11/);assert.match(await panel.innerText(),/se espera varchar\(250\)/);assert.match(await panel.innerText(),/Este guardado no cambia la estructura/);assert.equal(await page.locator('[data-field=TIPO_CAMPO]').first().getAttribute('aria-invalid'),'true');
},{assistant:true,configErrors:[{Campo:'TIPO_CAMPO',FilaCampo:1,CampoDestino:'CLAVE',Regla:'LONGITUD_EXCEDIDA',Limite:10,LongitudActual:11},{Campo:'TIPO_CAMPO',FilaCampo:2,CampoDestino:'NOMBRE',Regla:'TIPO_DESTINO_INCOMPATIBLE',TipoEsperado:'varchar(250)'}]}));

test('DOC126 propuesta de nombre cambia con el archivo y conserva un destino escrito por el usuario',()=>fixture(async(page)=>{
 await analyzeNew(page);page.on('dialog',d=>d.accept());
 await page.locator('[data-doc126=analisis-archivo]').setInputFiles({name:'Á'.repeat(100)+'.xlsx',buffer:Buffer.from('synthetic')});await page.locator('[data-doc126=analizar]').click();await page.waitForFunction(()=>document.querySelector('[data-doc126=tabla]').value==='imp_02_'+'a'.repeat(57));
 await page.locator('[data-doc126=tabla]').fill('imp_02_mi_destino');await page.locator('[data-doc126=analisis-archivo]').setInputFiles({name:'Otro archivo.xlsx',buffer:Buffer.from('synthetic')});await page.locator('[data-doc126=analizar]').click();await page.locator('[data-doc126=analisis-clave] option[value=CLAVE]').waitFor({state:'attached'});assert.equal(await page.locator('[data-doc126=tabla]').inputValue(),'imp_02_mi_destino');
},{assistant:true}));

test('DOC126 destino incompleto y clave ausente se señalan con foco sin enviar GuardarPlantilla',()=>fixture(async(page,calls)=>{
 await analyzeNew(page);const removeBox=await page.locator('[data-doc126=quitar-campo]').first().boundingBox();assert.ok(removeBox.height<=48,'Quitar se mantiene en una línea');
 await page.locator('[data-doc126=tabla]').fill('imp_02_');await page.locator('[data-doc126=analisis-clave]').selectOption('CLAVE');await page.locator('[data-doc126=guardar]').click();assert.match(await page.locator('[data-doc126=estado]').innerText(),/Completa Tabla de destino/);assert.equal(await page.locator('[data-doc126=tabla]').getAttribute('aria-invalid'),'true');assert.equal(await page.locator('[data-doc126=tabla]').evaluate(e=>e===document.activeElement),true);assert.equal(calls.some(c=>c.operation==='GuardarPlantilla'),false);
 await page.locator('[data-doc126=tabla]').fill('imp_02_corregida');await page.locator('[data-doc126=clave]').fill('NO_EXISTE');await page.locator('[data-doc126=guardar]').click();assert.match(await page.locator('[data-doc126=estado]').innerText(),/campo único que exista/);assert.equal(await page.locator('[data-doc126=clave]').getAttribute('aria-invalid'),'true');assert.equal(await page.locator('[data-doc126=clave]').evaluate(e=>e===document.activeElement),true);assert.equal(calls.some(c=>c.operation==='GuardarPlantilla'),false);
 await page.locator('[data-doc126=analisis-clave]').selectOption('CLAVE');assert.equal(await page.locator('[data-doc126=clave]').getAttribute('aria-invalid'),null);await page.locator('[data-doc126=guardar]').click();await page.locator('[data-doc126=destino]').filter({hasText:'imp_02_corregida'}).waitFor();assert.equal(calls.filter(c=>c.operation==='GuardarPlantilla').length,1);
},{assistant:true}));
