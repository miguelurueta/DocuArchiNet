const assert = require('node:assert/strict');
const test = require('node:test');
const factory = require('../js/gestion/ImportacionCarpetas/PerfilImportacionApi.js');
function deferred() {
  const done = [], fail = [];
  let state, args;
  const api = {
    done(fn) { if (state==='resolved') fn(...args); else done.push(fn); return api; },
    fail(fn) { if (state==='rejected') fn(...args); else fail.push(fn); return api; },
    resolve(...values) { if(state) return api; state='resolved'; args=values; done.forEach(fn=>fn(...values)); return api; },
    reject(...values) { if(state) return api; state='rejected'; args=values; fail.forEach(fn=>fn(...values)); return api; },
    promise() { return api; }
  };
  return api;
}
function setup() {
  let calls = [];
  const $ = { Deferred: deferred, ajax(options) { const result=deferred(); calls.push({options,result}); return result; } };
  const client = factory($).crear({url:'/app/webservice/WebServiceImportacionCarpetas.asmx',token:'synthetic-unit-token'});
  return {client,calls};
}
const ok = {ok:true,codigo:'OK',mensaje:'Listo',datos:{Revision:2},incidencias:[]};
test('DOC-110: solicitud tipada, sobre ASMX y respuesta real', () => {
  const {client,calls}=setup();
  let received;
  client.ejecutar('GuardarBorrador',{perfilId:'p'}).done(value=>received=value);
  assert.deepEqual(JSON.parse(calls[0].options.data),{solicitud:{perfilId:'p'}});
  assert.equal(calls[0].options.type,'POST');
  assert.ok(calls[0].options.headers['X-Importacion-Perfil-CSRF']);
  calls[0].result.resolve({d:ok});
  assert.equal(received.datos.Revision,2);
});
test('DOC-110: HTML, JSON doble y timeout no confirman ni repiten mutaciones', () => {
  for(const raw of ['<html>login</html>',{d:JSON.stringify(ok)},null]) {
    const {client,calls}=setup(); let failure;
    client.ejecutar('PublicarVersion',{}).fail(value=>failure=value);
    calls[0].result.resolve(raw);
    assert.equal(failure.codigo,'RESPUESTA_INVALIDA');
    assert.equal(failure.resultadoIncierto,true);
    assert.equal(calls.length,1);
  }
  const {client,calls}=setup(); let failure;
  client.ejecutar('CrearPerfil',{}).fail(value=>failure=value);
  calls[0].result.reject({status:0},'timeout');
  assert.equal(failure.resultadoIncierto,true);
  assert.equal(calls.length,1);
});
test('DOC-110: errores HTTP finalizan solicitud y 401/403 invalidan contexto', () => {
  for(const status of [400,401,403,409,503]) {
    const {client,calls}=setup(); let failure;
    client.ejecutar('GuardarBorrador',{}).fail(value=>failure=value);
    calls[0].result.reject({status,responseText:'sensitive server HTML'},'error');
    assert.equal(failure.httpStatus,status);
    assert.ok(!JSON.stringify(failure).includes('sensitive'));
    client.ejecutar('ObtenerPerfil',{});
    assert.equal(calls.length,status===401 || status===403 ? 1 : 2);
  }
});
test('DOC-110: rechaza operación desconocida y URL externa', () => {
  const {client,calls}=setup(); let failure;
  client.ejecutar('EjecutarImportacion',{}).fail(value=>failure=value);
  assert.equal(failure.codigo,'SOLICITUD_INVALIDA');
  assert.equal(calls.length,0);
  assert.throws(()=>factory({}).crear({url:'https://other.test',token:'x'}));
});

test('Fuentes: esquema ausente antes de escritura es fallo conocido, otros 503 siguen inciertos',()=>{
 for(const operation of ['CrearFuente','GuardarBorrador']){
  const {client,calls}=setup();let failure;
  client.ejecutar(operation,{}).fail(value=>failure=value);
  calls[0].result.reject({status:503,responseJSON:{d:{ok:false,codigo:'ESQUEMA_FUENTES_INCOMPLETO',mensaje:'No se guardo la fuente. Falta historial SQL 07.',datos:null,incidencias:[]}}},'error');
  assert.equal(failure.resultadoIncierto,operation!=='CrearFuente');assert.equal(calls.length,1);
 }
});
