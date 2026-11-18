'use strict';
const test=require('node:test'),assert=require('node:assert/strict');
const core=require('../js/gestion/ImportacionCarpetas/ImportacionCarpetas.js');
function fixture(count=3,options={}){
 const state={loteId:'34',nivelRef:'7',perfilId:'28',versionId:'43',rutaCanonica:'PRUEBA-IMPORT',estado:'EN_CURSO',revision:1,totalRaices:1,totalArchivos:count,detencionSolicitada:false,hayIntentosActivos:false,hayIncertidumbre:false,procesamientoIndependiente:true,escritorVivo:false,accionesPermitidas:['SOLICITAR_AVANCE']};
 const roots=[{recursoId:'52',idCliente:'r1',nombre:'Egresos',estado:'EXPEDIENTE_CONFIRMADO',expedienteId:'100',relacionNivelId:'200',revision:1,tieneIntentoActivo:false}];
 const files=Array.from({length:count},(_,i)=>({recursoId:String(78+i),raizId:'52',idCliente:'a'+i,nombre:i+'.pdf',rutaRelativa:'Egresos/'+i+'.pdf',estadoRecepcion:options.received===false?'PENDIENTE':'RECIBIDO',estado:'NO_INICIADO',revision:1,tieneIntentoActivo:false,valoresEfectivosFijados:true}));
 let live=true;const calls=[],decisions=[],errors=[],reads=[];
 const f={state,roots,files,calls,decisions,errors,reads,unavailable:options.unavailable,localMissing:options.localMissing};
 const engine=core.crearCoordinacion({vigente:()=>live,original:row=>row.recursoId!==f.localMissing,
  cambiar:value=>{if(value.decision)decisions.push(structuredClone(value.decision));},fallar:error=>errors.push(error),
  leer:async(consume,limits,op,previous)=>{
   consume();reads.push(op?op.row.recursoId:'ALL');
   if(!op || !options.focal)return structuredClone({estado:state,raices:roots,archivos:files});
   const next={estado:structuredClone(state),raices:previous.raices.slice(),archivos:previous.archivos.slice(),focalValidado:2,anterior:previous};
   if(op.tipo==='ARCHIVOS'){const i=files.findIndex(f=>f.recursoId===op.row.recursoId);next.archivos[i]=structuredClone(files[i]);next.raices[0]=structuredClone(roots[0]);}
   else next.raices[0]=structuredClone(roots[0]);return next;
  },ejecutar:async op=>{
   calls.push([op.row.recursoId,op.fase]);
   if(options.error)throw options.error;
   if(op.row.recursoId===f.unavailable){
    const error=Object.assign(Error('closed diagnostic'),{codigo:'ARCHIVO_NO_DISPONIBLE',noDisponible:{loteId:'34',archivoId:op.row.recursoId,raizId:'52',fase:op.fase,causa:'ORIGINAL_AUSENTE',certeza:'NO_INICIADO'}});
    files.find(row=>row.recursoId===op.row.recursoId).codigoResultado='ORIGINAL_NO_DISPONIBLE';state.revision++;throw error;
   }
   const row=files.find(f=>f.recursoId===op.row.recursoId),result={loteId:'34',archivoId:row.recursoId,fase:op.fase,estado:'CONFIRMADO'};
   if(op.fase==='RECEPCION'){row.estadoRecepcion='RECIBIDO';result.estadoRecepcion='RECIBIDO';}
   if(op.fase==='ALMACENAR'){row.estado='COMPLETADO';row.documentoId=String(300+files.indexOf(row));row.gabineteId='10';row.inventarioId=String(400+files.indexOf(row));Object.assign(result,{documentoId:row.documentoId,gabineteId:'10',inventarioId:row.inventarioId});}
   row.revision++;state.revision++;if(files.every(f=>f.estado==='COMPLETADO'))state.estado='COMPLETADO';return result;
  }});
 Object.assign(f,{engine,close(){live=false;engine.destruir();}});return f;
}
const tick=()=>new Promise(setImmediate);
async function waitDecision(f){for(let i=0;i<20&&f.engine.estado().estado!=='awaiting-decision';i++)await tick();assert.equal(f.engine.estado().estado,'awaiting-decision');}
test('IND01: A confirmado; B ausente espera decision; continuar guarda C y conserva B pendiente',async()=>{
 const f=fixture(3,{unavailable:'79',focal:true}),run=f.engine.iniciar();await waitDecision(f);
 assert.deepEqual(f.calls,[['78','ALMACENAR'],['79','ALMACENAR']]);assert.equal(f.engine.estado().guardados,1);await tick();assert.equal(f.calls.length,2);
 assert.equal(f.engine.decidir('continuar'),true);assert.equal(f.engine.decidir('continuar'),false);await run;
 assert.deepEqual(f.calls,[['78','ALMACENAR'],['79','ALMACENAR'],['80','ALMACENAR']]);assert.equal(f.files[1].estado,'NO_INICIADO');assert.equal(f.engine.estado().guardados,2);assert.equal(f.engine.estado().pendientes,1);assert.equal(f.engine.estado().completo,false);assert.equal(f.errors.length,0);
});
test('IND02: pausar conserva A y no solicita C; la siguiente ejecucion vuelve a enumerar y no repite A',async()=>{
 const f=fixture(3,{unavailable:'79'}),run=f.engine.iniciar();await waitDecision(f);f.engine.decidir('pausar');await run;
 assert.equal(f.calls.length,2);f.unavailable=null;await f.engine.iniciar();assert.equal(f.engine.estado().completo,true);assert.equal(f.calls.filter(([id])=>id==='78').length,1);assert.equal(f.reads.filter(id=>id==='ALL').length,2);
});
test('IND03: falta File local, cero envio remoto; excluido solo en la ejecucion consentida',async()=>{
 const f=fixture(3,{received:false,localMissing:'79'}),run=f.engine.iniciar();await waitDecision(f);
 assert.equal(f.decisions.at(-1).causa,'ORIGINAL_LOCAL_NO_SELECCIONADO');assert.equal(f.decisions.at(-1).local,true);assert.equal(f.calls.filter(([id])=>id==='79').length,0);
 f.engine.decidir('continuar');await run;assert.equal(f.files[1].estadoRecepcion,'PENDIENTE');assert.equal(f.engine.estado().guardados,2);
 f.localMissing=null;await f.engine.iniciar();assert.equal(f.engine.estado().completo,true);assert.equal(f.calls.filter(([id,phase])=>id==='78'&&phase==='ALMACENAR').length,1);
});
for(const mode of ['pausar','cerrar'])test('IND04: '+mode+' mientras espera decision no inicia otro archivo',async()=>{
 const f=fixture(3,{unavailable:'79'}),run=f.engine.iniciar();await waitDecision(f);if(mode==='pausar')await f.engine.pausar();else f.close();await run;assert.equal(f.calls.length,2);
});
for(const change of [s=>s.escritorVivo=true,s=>s.detencionSolicitada=true])test('IND05: una decision no evita escritor vivo ni detencion',async()=>{
 const f=fixture(3,{unavailable:'79'}),run=f.engine.iniciar();await waitDecision(f);change(f.state);f.engine.decidir('continuar');await run;assert.equal(f.calls.length,2);assert.equal(f.engine.estado().estado,'blocked');
});
test('IND06: incertidumbre sin retorno bloquea; con capacidad explicita procesa otro ID y conserva B incierto',async()=>{
 const f=fixture(3);Object.assign(f.state,{estado:'PENDIENTE_VERIFICACION',hayIncertidumbre:true,hayIntentosActivos:true});Object.assign(f.files[1],{estado:'INCIERTO',tieneIntentoActivo:true});f.roots[0].estado='INCIERTO';await f.engine.iniciar();assert.deepEqual(f.calls,[['78','ALMACENAR'],['80','ALMACENAR']]);assert.equal(f.files[1].estado,'INCIERTO');assert.equal(f.engine.estado().completo,false);
});
for(const error of [{codigo:'RESULTADO_INCIERTO',resultadoIncierto:true},{codigo:'ACCESO_DENEGADO'},{codigo:'ARCHIVO_NO_DISPONIBLE'},{codigo:'ERROR_RED',resultadoIncierto:true}])test('IND07: '+error.codigo+' sin certeza tipada no abre decision ni encadena',async()=>{
 const f=fixture(3,{error});await f.engine.iniciar();assert.equal(f.decisions.length,0);assert.equal(f.calls.length,1);assert.equal(f.engine.estado().estado,'blocked');
});
test('IND08: diagnostico ajeno no habilita continuar',async()=>{
 const f=fixture(3,{error:{codigo:'ARCHIVO_NO_DISPONIBLE',noDisponible:{loteId:'999',archivoId:'79',raizId:'52',fase:'ALMACENAR',certeza:'NO_INICIADO'}}});await f.engine.iniciar();assert.equal(f.decisions.length,0);assert.equal(f.calls.length,1);assert.equal(f.engine.estado().estado,'blocked');
});
for(const count of [1,100,1000])test('IND09: '+count+' archivos, una enumeracion inicial y lecturas focales sin huella global',async t=>{
 const f=fixture(count,{focal:true}),start=performance.now();await f.engine.iniciar();assert.equal(f.engine.estado().completo,true);assert.equal(f.calls.length,count);assert.equal(f.reads.length,count+1);assert.equal(f.reads.filter(id=>id==='ALL').length,1);t.diagnostic(JSON.stringify({count,logicalReads:f.reads.length,effects:f.calls.length,elapsedMs:Math.round(performance.now()-start),sql:'not executed (ports)'}));
});
