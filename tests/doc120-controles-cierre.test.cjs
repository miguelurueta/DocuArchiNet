'use strict';
const test=require('node:test'),assert=require('node:assert/strict');
const {politicaControles,accionesRecurso}=require('../js/gestion/ImportacionCarpetas/ImportacionCarpetas.js');
const api=require('../js/gestion/ImportacionCarpetas/ImportacionCarpetasApi.js')({});
function transport(){
 const requests=[];let aborts=0;
 const client=api.crear({url:'/synthetic.asmx',uploadUrl:'/synthetic.ashx',token:'synthetic',formDataFactory:()=>({fields:{},append(k,v){this.fields[k]=v;}}),xhrFactory:()=>{
  const xhr={upload:{},open(){},setRequestHeader(){},send(form){this.form=form;requests.push(this);},abort(){aborts++;if(this.onabort)this.onabort();}};return xhr;
 }});
 return{client,requests,get aborts(){return aborts;},ack(index){const x=requests[index];x.status=200;x.responseText=JSON.stringify({ok:true,codigo:'OK',mensaje:'Synthetic',incidencias:[],datos:{loteId:x.form.fields.loteId,archivoId:x.form.fields.archivoId,estadoRecepcion:'RECIBIDO'}});x.onload();}};
}
const items=()=>['30','31'].map(archivoId=>({solicitud:{loteId:'20',archivoId,claveIntento:'synthetic'},file:{}}));
test('CTL-P01: politica distingue ejecucion, pausa pendiente, navegacion, lectura e incertidumbre',()=>{
 const running={busy:true,coordinacion:{activa:true,estado:'running'}};assert.equal(politicaControles(running).puedeNavegar,true);assert.equal(politicaControles(running).puedePausar,true);assert.equal(politicaControles(running).bloquearEdicion,true);
 const applying={busy:true,coordinacion:{activa:true,estado:'pausing'}};assert.equal(politicaControles(applying).puedeNavegar,false);assert.equal(politicaControles(applying).puedePausar,false);
 const navigation={...running,transicionPendiente:{accion:'cerrar'}};assert.equal(politicaControles(navigation).puedeCerrar,false);assert.equal(politicaControles(navigation).puedeNavegar,false);
 assert.equal(politicaControles({busy:true}).puedeNavegar,false);assert.equal(politicaControles({busy:true}).puedeCerrar,true);
 const unknown={mutacionIncierta:true};assert.equal(politicaControles(unknown).resultadoIncierto,true);assert.equal(politicaControles(unknown).puedeNavegar,true);assert.deepEqual(accionesRecurso({...unknown,estado:{accionesPermitidas:['SOLICITAR_AVANCE']}},{},true),[]);
 assert.equal(politicaControles({coordinacion:{estado:'complete',activa:false}}).bloquearEdicion,false);
});
test('CTL-P02: pausa manual conserva acuse actual y no inicia el segundo PDF ni aborta',async()=>{
 const t=transport(),queue=t.client.enviarCola(items());await Promise.resolve();assert.equal(t.requests.length,1);assert.equal(t.requests[0].timeout,0);
 assert.equal(queue.pausar(),queue);assert.equal(queue.pausar(),queue);let settled=false;queue.then(()=>settled=true);await Promise.resolve();assert.equal(settled,false);assert.equal(t.aborts,0);
 t.ack(0);assert.deepEqual((await queue).map(x=>x.archivoId),['30']);assert.equal(t.requests.length,1);assert.equal(t.aborts,0);
});
test('CTL-P03: pausa antes de admision conserva cero solicitudes y cero recepciones',async()=>{
 const t=transport(),queue=t.client.enviarCola(items());queue.pausar();assert.deepEqual(await queue,[]);assert.equal(t.requests.length,0);assert.equal(t.aborts,0);
});
test('CTL-P04: desmontaje forzado conserva cancelacion explicita e incertidumbre sin replay',async()=>{
 const t=transport(),queue=t.client.enviarCola(items());await Promise.resolve();const rejected=assert.rejects(queue,e=>e.codigo==='SOLICITUD_CANCELADA'&&e.resultadoIncierto===true&&e.recepcionParcial.confirmados.length===0&&e.recepcionParcial.noIntentados===1);t.client.cerrar();await rejected;assert.equal(t.aborts,1);assert.equal(t.requests.length,1);
});
test('CTL-P05: fallo de respuesta despues de pedir pausa no se presenta como recepcion confirmada',async()=>{
 const t=transport(),queue=t.client.enviarCola(items());await Promise.resolve();queue.pausar();const rejected=assert.rejects(queue,e=>e.codigo==='ERROR_RED'&&e.resultadoIncierto===true&&e.recepcionParcial.archivoActual==='30'&&e.recepcionParcial.confirmados.length===0);t.requests[0].onerror();await rejected;assert.equal(t.aborts,0);assert.equal(t.requests.length,1);
});
