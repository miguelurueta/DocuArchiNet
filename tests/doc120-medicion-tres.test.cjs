'use strict';
const test=require('node:test'),assert=require('node:assert/strict');
const {parseTiming,selectFiles,protectedHash,allowed,runThree,MAX,SELECT_FILES,SELECT_LOT}=require('../tools/e2e/scripts/measure-doc120-three-files.cjs');
function rows(){return Array.from({length:1000},(_,i)=>({archivo_id:String(i+1),raiz_id:'60',revision:1,estado_recepcion:'RECIBIDO',estado_efecto:i<7?'COMPLETADO':'NO_INICIADO',bytes_previstos:'927',bytes_recibidos:'927',active_marker:0,fixed_values:1,result_hash:'synthetic'}));}
test('MEASURE01: selecciona solo tres pendientes recibidos con valores y conserva las huellas ajenas',()=>{
 const input=rows(),selected=selectFiles(input);assert.deepEqual(selected.map(r=>r.archivo_id),['8','9','10']);const before=protectedHash(input,selected);selected[0].estado_efecto='COMPLETADO';assert.equal(protectedHash(input,selected),before);input[0].result_hash='changed';assert.notEqual(protectedHash(input,selected),before);input[7].active_marker=1;assert.equal(selectFiles(input)[0].archivo_id,'9');assert.throws(()=>selectFiles(input.slice(1)));assert.equal(MAX.storage,3);
});
test('MEASURE02: rechaza otros efectos, replay, recursos ajenos y campos ignorados',()=>{
 const state={selected:selectFiles(rows()),dispatched:[]},request={loteId:'50',recursoId:'8',fase:'ALMACENAR',revisionEsperada:1,claveOperacion:'measure_'+'a'.repeat(32)};allowed('AvanzarLote',request,state);
 for(const name of ['ConfirmarPlan','ReanudarLote','SolicitarDetencion','PrepararPlan','RecibirArchivo'])assert.throws(()=>allowed(name,request,state));for(const change of [{fase:'CREAR_EXPEDIENTE'},{recursoId:'7'},{loteId:'51'},{usuarioRef:'99'},{revisionEsperada:2}])assert.throws(()=>allowed('AvanzarLote',{...request,...change},state));state.dispatched.push('8');assert.throws(()=>allowed('AvanzarLote',request,state));
});
test('MEASURE03: tiempos cerrados, separados, con conteos; nunca acepta texto privado',()=>{
 const names=['total','originales_previos','originales_retenidos','escritor_legacy','comprobacion_posterior','checkpoint','v2_inicio','v2_hash','original_v2'],header=names.map(k=>'doc120_'+k+';dur=1.250;desc="n=1"').join(', ');assert.equal(parseTiming(header).total.durationMs,1.25);for(const invalid of ['',header+', doc120_private_sql;dur=0.000;desc="n=1"',header+', '+header,header.replace('1.250','NaN'),header.replace('n=1','n=0')])assert.throws(()=>parseTiming(invalid));
});
test('MEASURE04: una solicitud pendiente impide siguiente archivo; una perdida termina sin retry',async()=>{
 const selected=selectFiles(rows()),started=[];let release;
 const read=async resourceId=>({recurso:{recursoId:resourceId,estadoRecepcion:'RECIBIDO',estado:'NO_INICIADO',valoresEfectivosFijados:true,tieneIntentoActivo:false,revision:1},padre:{recursoId:'60',expedienteId:'700',relacionNivelId:'800'},estado:{loteId:'50',detencionSolicitada:false,hayIntentosActivos:false,hayIncertidumbre:false}});
 const running=runThree({selected,read,store:row=>{started.push(row.archivo_id);return new Promise(resolve=>release=resolve);},verify:async()=>{}});
 await new Promise(resolve=>setImmediate(resolve));assert.deepEqual(started,['8']);release({});await new Promise(resolve=>setImmediate(resolve));assert.deepEqual(started,['8','9']);release({});await new Promise(resolve=>setImmediate(resolve));assert.deepEqual(started,['8','9','10']);release({});await running;
 started.length=0;await assert.rejects(runThree({selected,read,store:async row=>{started.push(row.archivo_id);throw Error('response lost');},verify:async()=>{}}));assert.deepEqual(started,['8']);
});
test('MEASURE05: controles solo SELECT, parametrizados al lote/propietario/empresa y acotados',()=>{
 for(const sql of [SELECT_FILES,SELECT_LOT]){assert.match(sql,/^SELECT /);assert.match(sql,/l\.lote_id=\? AND l\.solicitante_ref=\? AND l\.ambito_ref=\?/);assert.doesNotMatch(sql,/\b(?:UPDATE|DELETE|INSERT|ALTER|CREATE|FOR UPDATE)\b/);assert.equal((sql.match(/\?/g)||[]).length,3);}assert.match(SELECT_FILES,/LIMIT 1001$/);
});
test('MEASURE06: un cambio de recurso, padre, revision o actividad detiene antes del escritor',async()=>{
 const selected=selectFiles(rows());
 for(const change of [v=>v.estado.hayIntentosActivos=true,v=>v.estado.hayIncertidumbre=true,v=>v.estado.detencionSolicitada=true,v=>v.recurso.revision++,v=>v.padre.recursoId='61',v=>v.recurso.estado='COMPLETADO']){
  let writes=0;await assert.rejects(runThree({selected,read:async id=>{const v={recurso:{recursoId:id,estadoRecepcion:'RECIBIDO',estado:'NO_INICIADO',valoresEfectivosFijados:true,tieneIntentoActivo:false,revision:1},padre:{recursoId:'60',expedienteId:'700',relacionNivelId:'800'},estado:{loteId:'50',detencionSolicitada:false,hayIntentosActivos:false,hayIncertidumbre:false}};change(v);return v;},store:async()=>writes++,verify:async()=>{}}));assert.equal(writes,0);
 }
});
