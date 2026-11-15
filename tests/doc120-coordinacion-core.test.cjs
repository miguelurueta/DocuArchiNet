'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),core=require('../js/gestion/ImportacionCarpetas/ImportacionCarpetas.js');
function fixture(count=2){
 let state={loteId:'34',nivelRef:'7',perfilId:'28',versionId:'43',rutaCanonica:'PRUEBA-IMPORT',estado:'RECIBIENDO',revision:1,totalRaices:count,totalArchivos:count,detencionSolicitada:false,hayIntentosActivos:false,hayIncertidumbre:false,accionesPermitidas:['CONSULTAR','CONSULTAR_RAICES','SOLICITAR_AVANCE']};
 const roots=Array.from({length:count},(_,i)=>({recursoId:String(52+i),idCliente:'r'+i,nombre:'Carpeta '+i,estado:'PENDIENTE',revision:1,recepcionListaParaCrear:false,tieneIntentoActivo:false,codigoExpediente:'MISMO-CODIGO'}));
 const files=roots.map((r,i)=>({recursoId:String(78+i),idCliente:'a'+i,raizId:r.recursoId,nombre:i+'.pdf',estadoRecepcion:'PENDIENTE',estado:'NO_INICIADO',revision:1,valoresEfectivosFijados:false,tieneIntentoActivo:false}));
 const calls=[],updates=[],f={roots,files,calls,updates,live:true,local:true,reads:0,failReadAt:0,failPhase:null,lostPhase:null,hold:null,errors:[]};
 const options={vigente:()=>f.live,original:()=>f.local,cambiar:(value)=>updates.push(value),fallar:(e,op,confirmed)=>f.errors.push({code:e.codigo||e.message,op:op&&op.fase,confirmed}),
 leer:async consume=>{consume();f.reads++;if(f.reads===f.failReadAt)throw Object.assign(Error('ERROR_RED'),{codigo:'ERROR_RED'});return structuredClone({estado:state,raices:roots,archivos:files});},
 ejecutar:async(op)=>{
  calls.push({tipo:op.tipo,id:op.row.recursoId,phase:op.fase});if(f.hold)await f.hold;if(f.failPhase===op.fase)throw Object.assign(Error('ACCESO_DENEGADO'),{codigo:'ACCESO_DENEGADO'});
  const row=(op.tipo==='RAICES'?roots:files).find(r=>r.recursoId===op.row.recursoId),result={loteId:'34',fase:op.fase,estado:'CONFIRMADO'};
  if(op.tipo==='RAICES'){Object.assign(row,{estado:'EXPEDIENTE_CONFIRMADO',expedienteId:String(100+roots.indexOf(row)),relacionNivelId:String(200+roots.indexOf(row))});Object.assign(result,{raizId:row.recursoId,expedienteId:row.expedienteId,relacionNivelId:row.relacionNivelId});}
  else{result.archivoId=row.recursoId;if(op.fase==='RECEPCION'){row.estadoRecepcion='RECIBIDO';result.estadoRecepcion='RECIBIDO';if(files.every(f=>f.estadoRecepcion==='RECIBIDO'))roots.forEach(r=>r.recepcionListaParaCrear=true);}
   if(op.fase==='MATERIALIZAR_VALORES')row.valoresEfectivosFijados=true;
   if(op.fase==='ALMACENAR'){Object.assign(row,{estado:'COMPLETADO',documentoId:String(300+files.indexOf(row)),gabineteId:'10'});Object.assign(result,{documentoId:row.documentoId,gabineteId:'10'});}}
  row.revision++;state.revision++;state.estado=files.every(r=>r.estado==='COMPLETADO')?'COMPLETADO':'EN_CURSO';
  if(f.lostPhase===op.fase)throw Object.assign(Error('ERROR_RED'),{codigo:'ERROR_RED',resultadoIncierto:true});return result;
 }};
 f.options=options;f.state=state;f.create=()=>f.engine=core.crearCoordinacion(options);return f;
}
test('DOC120-COORD01: un inicio coordina todas las fases por ID y conserva códigos iguales',async()=>{
 const f=fixture(),engine=f.create();await engine.iniciar();assert.equal(engine.estado().estado,'complete');assert.equal(f.calls.length,8);assert.deepEqual(f.calls.map(c=>c.phase),['RECEPCION','RECEPCION','CREAR_EXPEDIENTE','CREAR_EXPEDIENTE','MATERIALIZAR_VALORES','MATERIALIZAR_VALORES','ALMACENAR','ALMACENAR']);assert.notEqual(f.roots[0].expedienteId,f.roots[1].expedienteId);assert.equal(f.roots[0].codigoExpediente,f.roots[1].codigoExpediente);await engine.iniciar();assert.equal(f.calls.length,8);
});
test('DOC120-COORD02: doble inicio comparte una ejecución; pausa espera el acuse iniciado',async()=>{
 const f=fixture(1);let release;f.hold=new Promise(r=>release=r);const e=f.create(),first=e.iniciar(),second=e.iniciar();assert.equal(first,second);await new Promise(setImmediate);assert.equal(f.calls.length,1);const paused=e.pausar();f.hold=null;release();await paused;assert.equal(f.calls.length,1);assert.equal(f.files[0].estadoRecepcion,'RECIBIDO');assert.equal(e.estado().estado,'paused');await e.iniciar();assert.equal(e.estado().estado,'complete');assert.equal(f.calls.filter(c=>c.phase==='RECEPCION').length,1);
});
for(const phase of ['RECEPCION','CREAR_EXPEDIENTE','MATERIALIZAR_VALORES','ALMACENAR'])test('DOC120-COORD03: pérdida de acuse en '+phase+' consulta sin repetir ni encadenar',async()=>{
 const f=fixture(1);f.lostPhase=phase;const e=f.create();await e.iniciar();const calls=f.calls.length;assert.equal(f.errors.length,1);assert.equal(e.estado().estado,'blocked');assert.equal(f.reads,calls+1);assert.equal(f.calls.filter(c=>c.phase===phase).length,1);await new Promise(setImmediate);assert.equal(f.calls.length,calls);
});
for(const phase of ['RECEPCION','CREAR_EXPEDIENTE','MATERIALIZAR_VALORES','ALMACENAR'])test('DOC120-COORD04: acuse '+phase+' seguido de lectura fallida conserva confirmación',async()=>{
 const f=fixture(1),index=['RECEPCION','CREAR_EXPEDIENTE','MATERIALIZAR_VALORES','ALMACENAR'].indexOf(phase);f.failReadAt=index+2;const e=f.create();await e.iniciar();assert.equal(f.errors[0].confirmed,true);assert.equal(f.errors[0].op,phase);assert.equal(f.calls.length,index+1);assert.equal(e.estado().estado,'blocked');
});
test('DOC120-COORD05: original ausente pausa sin efectos y requiere nuevo inicio',async()=>{
 const f=fixture(1);f.local=false;const e=f.create();await e.iniciar();assert.equal(e.estado().estado,'needs-original');assert.equal(f.calls.length,0);f.local=true;assert.equal(f.calls.length,0);await e.iniciar();assert.equal(e.estado().completo,true);
});
test('DOC120-COORD06: cierre invalida respuestas tardías y no solicita lectura ni otro efecto',async()=>{
 const f=fixture(1);let release;f.hold=new Promise(r=>release=r);const e=f.create(),run=e.iniciar();await new Promise(setImmediate);e.destruir();f.live=false;release();await run;assert.equal(f.calls.length,1);assert.equal(f.reads,1);
});
test('DOC120-COORD07: estado activo, incertidumbre o detención impiden toda mutación',async()=>{
 for(const change of [f=>f.state.hayIntentosActivos=true,f=>f.state.hayIncertidumbre=true,f=>f.state.detencionSolicitada=true,f=>f.roots[0].tieneIntentoActivo=true,f=>f.files[0].estadoRecepcion='INCIERTO']){const f=fixture(1);change(f);await f.create().iniciar();assert.equal(f.calls.length,0);assert.equal(f.engine.estado().estado,'blocked');}
});
test('DOC120-COORD08: IDs duplicados, padre ajeno o inventario incompleto rechazan enumeración',async()=>{
 for(const change of [f=>f.roots[1].recursoId=f.roots[0].recursoId,f=>f.files[1].recursoId=f.files[0].recursoId,f=>f.files[0].raizId='999',f=>f.files.pop(),f=>f.state.totalArchivos=10001]){const f=fixture();change(f);await f.create().iniciar();assert.equal(f.calls.length,0);assert.equal(f.engine.estado().estado,'blocked');}
});
test('DOC120-COORD09: inventario o ámbito cambiado después de confirmar pausa sin otro efecto',async()=>{
 for(const field of ['loteId','nivelRef','versionId','perfilId']){const f=fixture(1),base=f.options.ejecutar;f.options.ejecutar=async(...args)=>{const result=await base(...args);f.state[field]='999';return result;};await f.create().iniciar();assert.equal(f.calls.length,1);assert.equal(f.engine.estado().estado,'blocked');}
});
test('DOC120-COORD10: guardas reales de accionesRecurso impiden índices sin padre confirmado',async()=>{
 const f=fixture(1);f.files[0].estadoRecepcion='RECIBIDO';f.roots[0].recepcionListaParaCrear=false;await f.create().iniciar();assert.equal(f.calls.length,0);assert.equal(f.engine.estado().estado,'paused');
});
test('DOC120-COORD11: consumo excesivo de lecturas corta antes del siguiente efecto',async()=>{
 const f=fixture(1);f.options.leer=async consume=>{for(let i=0;i<10001;i++)consume();};await f.create().iniciar();assert.equal(f.engine.estado().codigo,'COORDINACION_LIMITE');assert.equal(f.calls.length,0);
});
test('DOC120-COORD12: permisos revocados y respuesta sin IDs nunca habilitan la siguiente fase',async()=>{
 for(const invalid of [false,true]){const f=fixture(1);if(!invalid)f.failPhase='CREAR_EXPEDIENTE';else{const call=f.options.ejecutar;f.options.ejecutar=async(...args)=>{const r=await call(...args);if(r.fase==='CREAR_EXPEDIENTE')delete r.expedienteId;return r;};}await f.create().iniciar();assert.equal(f.calls.length,2);assert.equal(f.engine.estado().estado,'blocked');}
});
for(const [mode,scope] of [['COMPLETO_OBLIGATORIO','EXPEDIENTE'],['COMPLETO_OBLIGATORIO','LOTE'],['PARCIAL_PERMITIDO','NO_APLICA']])test('DOC120-COORD13: secuencia respeta barreras publicadas '+mode+'/'+scope,async()=>{
 const f=fixture(2);Object.assign(f.state,{modoIntegridad:mode,ambitoIntegridad:scope});await f.create().iniciar();assert.deepEqual(f.calls.slice(0,2).map(c=>c.phase),['RECEPCION','RECEPCION']);for(const phase of ['CREAR_EXPEDIENTE','MATERIALIZAR_VALORES','ALMACENAR'])assert.equal(f.calls.filter(c=>c.phase===phase).length,2);assert.equal(f.engine.estado().completo,true);
});
test('DOC120-COORD14: dos vistas con revisión compartida detienen la perdedora sin duplicar efectos',async()=>{
 const f=fixture(1),execute=f.options.ejecutar;let attempts=0;f.options.ejecutar=async(op,...args)=>{attempts++;const row=(op.tipo==='RAICES'?f.roots:f.files).find(r=>r.recursoId===op.row.recursoId);if(row.revision!==op.row.revision)throw Object.assign(Error('CONFLICTO_REVISION'),{codigo:'CONFLICTO_REVISION'});return execute(op,...args);};const a=core.crearCoordinacion(f.options),b=core.crearCoordinacion(f.options);await Promise.all([a.iniciar(),b.iniciar()]);assert.deepEqual([a.estado().estado,b.estado().estado].sort(),['blocked','complete']);assert.equal(f.calls.filter(c=>c.phase==='CREAR_EXPEDIENTE').length,1);assert.equal(f.calls.filter(c=>c.phase==='ALMACENAR').length,1);assert.equal(attempts,5);
});
test('DOC120-COORD15: lectura incompatible con un acuse conserva identidad confirmada y no continúa',async()=>{
 const f=fixture(1),execute=f.options.ejecutar;f.options.ejecutar=async(op,...args)=>{const result=await execute(op,...args);if(op.fase==='CREAR_EXPEDIENTE')f.roots[0].expedienteId='999';return result;};await f.create().iniciar();assert.equal(f.calls.length,2);assert.equal(f.errors.at(-1).confirmed,true);assert.equal(f.engine.estado().codigo,'RESULTADO_PENDIENTE_LECTURA');
});
