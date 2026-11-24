'use strict';
const test=require('node:test'),assert=require('node:assert/strict');
const factory=require('../js/gestion/ImportacionExcel/ImportacionExcelApi.js');
function fixture(extra={}){
 const requests=[];
 const jq={ajax(settings){const entry={settings};requests.push(entry);return {done(fn){entry.resolve=fn;return this;},fail(fn){entry.reject=fn;return this;}};}};
 class Form{constructor(){this.values=[];}append(...args){this.values.push(args);}}
 const options={url:'/app/webservice/excel.asmx',upload:'/app/upload.ashx',recuperacion:'a'.repeat(32),...extra};
 const api=factory(jq,{FormData:Form}).crear(options);
 async function open(){const p=api.ejecutar('AbrirCargaExcel');requests.at(-1).resolve({d:{ok:true,codigo:'OK',datos:{CsrfToken:'synthetic-fixture',Conexiones:[]}}});await p;}
 return {requests,api,open,options,jq,Form};
}
test('DOC126 cliente conserva los datos de una validación rechazada y usa contexto/CSRF propios',async()=>{
 const f=fixture();await f.open();const pending=f.api.ejecutar('Preparar',{IdCarga:'synthetic'});const call=f.requests.at(-1);
 assert.equal(call.settings.headers['X-Importacion-Excel-Contexto'],'a'.repeat(32));assert.equal(call.settings.headers['X-Importacion-Excel-CSRF'],'synthetic-fixture');
 assert.equal(call.settings.timeout,30000);assert.deepEqual(JSON.parse(call.settings.data),{solicitud:{IdCarga:'synthetic'}});
 const datos={Carga:{IdCarga:'synthetic'},Incidencias:[{Fila:2}],TotalIncidencias:1};call.resolve({d:{ok:false,codigo:'LECTURA_INVALIDA',datos}});
 await assert.rejects(pending,e=>e.codigo==='LECTURA_INVALIDA'&&e.datos===datos&&!e.resultadoIncierto);
});

test('DOC126 análisis es opcional, independiente del upload y no genera incertidumbre durable',async()=>{
 const previous=fixture();await previous.open();await assert.rejects(previous.api.analizar({}, {name:'book.xlsx'}),e=>e.codigo==='ANALISIS_NO_DISPONIBLE');assert.equal(previous.requests.length,1);
 for(const analisis of ['/app/analysis.ashx','//outside.invalid/analysis','/app/../analysis']){
  const f=fixture({analisis});await f.open();const p=f.api.analizar({conexionId:6,hoja:'Datos',tabla:'imp_02_fixture',modo:'CREAR'},{name:'book.xlsx'});
  if(analisis!=='/app/analysis.ashx'){await assert.rejects(p);assert.equal(f.requests.length,1);continue;}
  const call=f.requests.at(-1);assert.equal(call.settings.url,analisis);assert.equal(call.settings.contentType,false);assert.equal(call.settings.headers['X-Importacion-Excel-CSRF'],'synthetic-fixture');
  assert.deepEqual(call.settings.data.values.map(v=>v[0]),['conexionId','hoja','tabla','modo','archivo']);call.reject({status:0});await assert.rejects(p,e=>!e.resultadoIncierto);assert.equal(f.requests.length,2);
  const controlled=f.api.analizar({conexionId:6,modo:'CREAR'},{name:'book.xlsx'});f.requests.at(-1).resolve({ok:false,codigo:'RESULTADO_INCIERTO'});await assert.rejects(controlled,e=>!e.resultadoIncierto);assert.equal(f.requests.length,3);
 }
});
test('DOC126 un acuse inválido o perdido de Confirmar queda incierto sin retry',async()=>{
 for(const broken of ['invalid','network','controlled']){const f=fixture();await f.open();const p=f.api.ejecutar('Confirmar',{IdCarga:'synthetic'});const call=f.requests.at(-1);
  if(broken==='invalid')call.resolve('<html>error</html>');else if(broken==='network')call.reject({status:0});else call.resolve({d:{ok:false,codigo:'RESULTADO_INCIERTO'}});
  await assert.rejects(p,e=>e.resultadoIncierto===true);assert.equal(f.requests.length,2);
 }
});
test('DOC126 denegación cierra el token sin convertir datos crudos en error visible',async()=>{
 const f=fixture();await f.open();const p=f.api.ejecutar('ListarPlantillas',{ConexionId:6});f.requests.at(-1).resolve({d:{ok:false,codigo:'ACCESO_DENEGADO',mensaje:'private provider detail'}});
 await assert.rejects(p,e=>e.codigo==='ACCESO_DENEGADO'&&!JSON.stringify(e).includes('private provider'));
 await assert.rejects(f.api.ejecutar('GuardarPlantilla',{}),e=>e.codigo==='SESION_REQUERIDA');assert.equal(f.requests.length,2);
});
test('DOC126 multipart mantiene exactamente el contrato de recepción, sin boundary manual',async()=>{
 const f=fixture();await f.open();const file={name:'fixture.xlsx'};const p=f.api.cargar({idCarga:'id',referenciaRecepcion:'reference',conexionId:6,plantillaId:29},file);const call=f.requests.at(-1);
 assert.equal(call.settings.url,f.options.upload);assert.equal(call.settings.contentType,false);assert.equal(call.settings.processData,false);
 assert.deepEqual(call.settings.data.values.map(v=>v[0]),['idCarga','referenciaRecepcion','conexionId','plantillaId','archivo']);
 call.resolve({ok:true,codigo:'OK',datos:{Estado:'RECIBIDA'}});assert.equal((await p).datos.Estado,'RECIBIDA');
});
test('DOC126 bootstrap no envía token heredado y no acepta operaciones/rutas libres',async()=>{
 const f=fixture();const opening=f.api.ejecutar('AbrirCargaExcel');assert.deepEqual(JSON.parse(f.requests[0].settings.data),{});assert.equal(f.requests[0].settings.headers['X-Importacion-Excel-CSRF'],undefined);
 f.requests[0].resolve({d:{ok:true,codigo:'OK',datos:{CsrfToken:'synthetic-fixture'}}});await opening;
 await assert.rejects(f.api.ejecutar('EjecutarSql',{}),e=>e.codigo==='SOLICITUD_INVALIDA');assert.equal(f.requests.length,1);
 for(const url of ['https://outside.invalid/api','//outside.invalid/api','/app/../api','/app/api?x=1'])assert.throws(()=>factory(f.jq,{}).crear({...f.options,url}));
});

test('DOC126 transporte conserva sólo descriptores de validación tipados, acotados y sin mensajes crudos',async()=>{
 const f=fixture();await f.open();const p=f.api.ejecutar('GuardarPlantilla',{ConexionId:6});
 f.requests.at(-1).resolve({d:{ok:false,codigo:'CONFIGURACION_INVALIDA',mensaje:'private provider detail',totalValidacion:40,validacion:[{Campo:'REMPLAZA_COMA',Regla:'REQUERIDO',FilaCampo:1,CampoDestino:'CLAVE',Mensaje:'private provider detail'},{Campo:'<img>',Regla:'REQUERIDO'},{Campo:'TIPO_CAMPO',Regla:'TIPO_DESTINO_INCOMPATIBLE',FilaCampo:2,TipoEsperado:'varchar(250)',LongitudActual:11,Limite:10}]}});
 await assert.rejects(p,e=>{assert.equal(e.validacion.length,2);assert.equal(e.totalValidacion,40);assert.equal(e.validacion[1].TipoEsperado,'varchar(250)');assert.doesNotMatch(JSON.stringify(e),/private provider|<img>/);return !e.resultadoIncierto;});
});
