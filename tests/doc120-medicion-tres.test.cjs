'use strict';
const test=require('node:test'),assert=require('node:assert/strict');
const {parseTiming,selectFiles,protectedHash,allowed,runThree,MAX,SELECT_FILES,SELECT_LOT}=require('../tools/e2e/scripts/measure-doc120-three-files.cjs');
const {prepareWindow,reviewPrevious,assertPreviousUnchanged,validateIndex,waitBinary,SELECT_INDEX}=require('../tools/e2e/scripts/measure-doc120-three-files.cjs');
const fs=require('node:fs'),os=require('node:os'),path=require('node:path');
const {collectApplicationCredentials,failure}=require('../tools/e2e/scripts/measure-doc120-three-files.cjs');
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

test('MEASURE07: captura categorias internas nuevas sin ampliar la ventana real cerrada',()=>{
 const required=['total','originales_previos','originales_retenidos','escritor_legacy','comprobacion_posterior','checkpoint'].map(k=>'doc120_'+k+';dur=1.000;desc="n=1"').join(', ');
 const value=parseTiming(required+', doc120_sql_archivo;dur=2.000;desc="n=3", doc120_json_chars;dur=0.000;desc="n=2000000", doc120_pdf_bytes;dur=0.000;desc="n=4000000", doc120_original_open;dur=0.100;desc="n=2"');
 assert.equal(value.sql_archivo.calls,3);assert.equal(value.pdf_bytes.calls,4000000);assert.equal(value.json_chars.calls,2000000);assert.equal(value.original_open.calls,2);
 assert.throws(()=>parseTiming(required+', doc120_sql_archivo;dur=0.000;desc="n=1000001"'));assert.throws(()=>parseTiming(required+', doc120_pdf_bytes;dur=0.000;desc="n=9007199254740992"'));assert.equal(MAX.storage,3);
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

test('MEASURE08: continuacion revisada crea ventana nueva y conserva estado/ventana cerrados byte a byte',()=>{
 const parent=fs.mkdtempSync(path.join(os.tmpdir(),'doc120-measure-review-')),previous='a'.repeat(32),next='b'.repeat(32),folder=path.join(parent,previous);
 fs.mkdirSync(folder);const state={runId:previous,status:'FINISHED_THREE',closed:true,credentialsCleared:true,http:104,select:14,sessions:1,max:MAX,dispatched:['104','105','106'],results:[{fileId:'104'},{fileId:'105'},{fileId:'106'}],before:{completed:7,protectedHash:'same'},after:{completed:10,protectedHash:'same'}};
 fs.writeFileSync(path.join(folder,'state.json'),JSON.stringify(state));fs.writeFileSync(path.join(parent,'window.json'),JSON.stringify({runId:previous}));
 try{
  const review=reviewPrevious(parent,previous),result=prepareWindow(parent,next,previous);assertPreviousUnchanged(review);assert.equal(result.review.http,104);assert.equal(result.review.confirmed.length,3);assert.equal(JSON.parse(fs.readFileSync(path.join(result.folder,'window.json'))).previousRunId,previous);
  assert.throws(()=>prepareWindow(parent,'c'.repeat(32),previous),/ALREADY_CONTINUED/);assert.throws(()=>prepareWindow(parent,next,previous),/RUN_ALREADY_EXISTS/);assert.throws(()=>prepareWindow(parent,'c'.repeat(32)),/REQUIRES_REVIEW/);
  for(const change of [s=>s.closed=false,s=>s.credentialsCleared=false,s=>s.after.completed=11,s=>s.dispatched.push('107'),s=>s.sessions=2]){const broken=structuredClone(state);change(broken);fs.writeFileSync(path.join(folder,'state.json'),JSON.stringify(broken));assert.throws(()=>reviewPrevious(parent,previous));}
  fs.writeFileSync(path.join(folder,'state.json'),JSON.stringify({...state,http:105}));assert.throws(()=>assertPreviousUnchanged(review),/PREVIOUS_WINDOW_CHANGED/);
 }finally{assert.equal(path.dirname(parent),os.tmpdir());assert.ok(path.basename(parent).startsWith('doc120-measure-review-'));fs.rmSync(parent,{recursive:true,force:true});}
});

test('MEASURE09: indice debe ser completo, no unico, sin prefijos; lectura2 se solicita explicitamente',()=>{
 const indexRows=['lote_id','recurso_tipo','recurso_id','accion'].map((column_name,i)=>({column_name,sequence_index:i+1,non_unique:1,sub_part:null}));assert.equal(validateIndex(indexRows),true);
 for(const change of [r=>r.pop(),r=>r[0].non_unique=0,r=>r[0].sub_part=1,r=>r[0].column_name='other',r=>r.reverse()]){const broken=structuredClone(indexRows);change(broken);assert.equal(validateIndex(broken),false);}
 assert.match(SELECT_INDEX,/^SELECT /);assert.doesNotMatch(SELECT_INDEX,/\b(?:ALTER|UPDATE|INSERT|DELETE|FOR UPDATE)\b/);
 const state={selected:selectFiles(rows()),dispatched:[],readVersion:2},request={loteId:'50',tipo:'ARCHIVOS',recursoId:'8',versionLectura:2};allowed('ConsultarRecursoLote',request,state);assert.throws(()=>allowed('ConsultarRecursoLote',{...request,versionLectura:1},state));assert.throws(()=>allowed('ConsultarRecursoLote',{...request,foreign:'bad'},state));
});

test('MEASURE10: espera local de DLL no permite usar binario anterior ni ajeno ni esperar sin limite',async()=>{
 let calls=0,values=['old','old','new'];await waitBinary('new','old',()=>false,()=>values.shift(),async()=>calls++);assert.equal(calls,2);
 await assert.rejects(waitBinary('new','old',()=>false,()=> 'foreign',async()=>{}),/BINARY_CHANGED_BY_OTHER/);await assert.rejects(waitBinary('new','old',()=>true,()=> 'old',async()=>{}),/WINDOW_EXPIRED/);
});

test('MEASURE11: diagnostico de entrada privada conserva solo fase y codigo cerrado',async()=>{
 for(const failedName of ['user','password']){
  const stages=[],calls=[],values={};
  await assert.rejects(collectApplicationCredentials(values,phase=>stages.push(phase),async(target,name,_label,options)=>{
   calls.push(name);assert.equal(options.secret,true);if(name===failedName)throw Error('private-sentinel');target[name]='synthetic';
  }),error=>{
   assert.equal(failure(error),'DOC120_MEASURE_APPLICATION_'+(failedName==='user'?'ACCOUNT':'PASSWORD')+'_INPUT_FAILED');assert.doesNotMatch(error.message,/private-sentinel/);return true;
  });
  assert.deepEqual(calls,failedName==='user'?['user']:['user','password']);assert.equal(stages.at(-1),failedName==='user'?'WAITING_APPLICATION_ACCOUNT':'WAITING_APPLICATION_PASSWORD');
 }
 const values={};await collectApplicationCredentials(values,()=>{},async(target,name)=>{target[name]='synthetic';});assert.deepEqual(values,{user:'synthetic',password:'synthetic'});
});
