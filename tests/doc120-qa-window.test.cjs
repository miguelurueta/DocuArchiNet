'use strict';
const test=require('node:test'),assert=require('node:assert/strict');
const {MAX,SQL,bind,confirmationBinding,EffectsGate,fingerprint,receiptPayload,failure,hash}=require('../tools/e2e/tests/support/doc120-qa-window.cjs');
const {heldGate,gateOutcome,reviewPrior}=require('../tools/e2e/scripts/run-doc120-qa-failures.cjs');
const fs=require('node:fs'),os=require('node:os'),path=require('node:path');
function fixture(){
 const header=(lot,level,version,n,r)=>({lote_id:lot,nivel_ref:level,version_id:version,total_archivos:String(n),total_raices:String(r),unresolved:'0',detencion_solicitada:'0',estado:'EN_CURSO',config_doc:JSON.stringify({Configuracion:{OrigenGabinete:'DA',GabineteId:10}})});
 const file=(lot,root,id,client,received=true)=>({lote_id:lot,raiz_id:root,archivo_id:id,cliente_archivo_id:client,estado_recepcion:received?'RECIBIDO':'PENDIENTE',estado_efecto:'NO_INICIADO',active_marker:'0',fixed_values:'1',bytes_previstos:lot==='29'?'677':'44884',bytes_recibidos:received?(lot==='29'?'677':'44884'):null,has_staging:received?'1':'0',resultado_doc:null});
 return{headers:[header('29','7','40',27,2),header('50','223','45',1000,1)],roots:[{lote_id:'29',raiz_id:'1',cliente_raiz_id:'r1',estado:'EXPEDIENTE_CONFIRMADO',identidad_expediente_doc:'{}'},{lote_id:'29',raiz_id:'2',cliente_raiz_id:'r2',estado:'PENDIENTE',identidad_expediente_doc:null},{lote_id:'50',raiz_id:'3',estado:'EXPEDIENTE_CONFIRMADO',identidad_expediente_doc:'{}'}],files:[...Array.from({length:27},(_,i)=>file('29',i===0?'1':'2',String(10+i),'a'+(i+1),i===0)),...Array.from({length:1000},(_,i)=>file('50','3',String(100+i),'m'+i))]};
}
const command=(lot,id,phase='ALMACENAR')=>({loteId:lot,recursoId:id,fase:phase,revisionEsperada:1,claveOperacion:'qa_'+ 'a'.repeat(32)});
test('QA01: preflight rechaza cambios de corpus, actividad, detencion y ausencia previa del original esperado',()=>{
 assert.equal(bind(fixture()).C,'12');for(const change of [d=>d.files.pop(),d=>d.headers[0].unresolved='1',d=>d.headers[1].detencion_solicitada='1',d=>d.files[2].has_staging='1',d=>d.roots[1].identidad_expediente_doc='{}',d=>d.files[0].resultado_doc='{}']){const data=fixture();change(data);assert.throws(()=>bind(data));}
});
test('QA02: solo A/C/D/E y una raiz pueden mutar; B, otros recursos, fases y cuenta ajena se bloquean',()=>{
 const b=bind(fixture()),gate=new EffectsGate(b);for(const p of [command('29',b.B),command('50','150'),command('29',b.A,'CREAR_EXPEDIENTE'),command('50',b.D,'MATERIALIZAR_VALORES'),{...command('29',b.A),usuarioRef:'142'}])assert.throws(()=>gate.permit('AvanzarLote',p));assert.throws(()=>gate.permit('AvanzarLote',command('29',b.A),'OTHER'));
 for(const operation of ['PrepararPlan','ReanudarLote','SolicitarDetencion','CerrarReservaSinInicio','RecuperarDocumentoV2'])assert.throws(()=>gate.permit(operation,{loteId:'29'}));
 gate.permit('AvanzarLote',command('29',b.A,'MATERIALIZAR_VALORES'));gate.permit('AvanzarLote',command('29',b.A));assert.throws(()=>gate.permit('AvanzarLote',command('29',b.A)));gate.permit('AvanzarLote',command('29',b.createRoot,'CREAR_EXPEDIENTE'));assert.throws(()=>gate.permit('AvanzarLote',command('29',b.createRoot,'CREAR_EXPEDIENTE')));
});
test('QA03: excepcion de tres solicitudes solo para carrera E; recibo y reconfirmacion conservan limites',()=>{
 const b=bind(fixture());b.plan={revision:2,hash:'a'.repeat(64)};const gate=new EffectsGate(b);for(let i=0;i<3;i++)gate.permit('AvanzarLote',command('50',b.E));assert.throws(()=>gate.permit('AvanzarLote',command('50',b.E)));
 const receipt={loteId:'29',archivoId:b.C,claveIntento:'a'.repeat(32)};gate.permit('RecibirArchivo',receipt);assert.throws(()=>gate.permit('RecibirArchivo',receipt));assert.throws(()=>gate.permit('RecibirArchivo',{...receipt,archivoId:b.B}));
 const confirmation={planId:'90',revisionEsperada:2,planHashEsperado:'a'.repeat(64),claveConfirmacion:'a'.repeat(32)};gate.permit('ConfirmarPlan',confirmation);gate.permit('ConfirmarPlan',confirmation);assert.throws(()=>gate.permit('ConfirmarPlan',confirmation));assert.throws(()=>gate.permit('ConfirmarPlan',{...confirmation,planId:'45'}));assert.throws(()=>receiptPayload(b,Buffer.alloc(677)));
});
test('QA04: solo SELECT acotados, huella de ajenos estable y errores cerrados sin datos privados',()=>{
 for(const sql of Object.values(SQL)){assert.match(sql,/^SELECT /);assert.doesNotMatch(sql,/;|\b(?:UPDATE|INSERT|DELETE|ALTER|CREATE|CALL|FOR UPDATE)\b/i);assert.match(sql,/\?/);}
 const d=fixture(),b=bind(d),before=fingerprint(d.files,[b.A,b.C]);d.files[0].resultado_doc='allowed';assert.equal(fingerprint(d.files,[b.A,b.C]),before);d.files[1].revision='2';assert.notEqual(fingerprint(d.files,[b.A,b.C]),before);assert.equal(failure(Error('private-sentinel')),'DOC120_QA_STOPPED');assert.equal(MAX.receiptBytes,677);assert.equal(MAX.storageCalls,6);
});
test('QA05: retener una respuesta espera liberacion explicita; el acuse conserva su identidad',async()=>{
 const gate=heldGate();let delivered=false;const delivery=gate.continued.then(()=>{delivered=true;});gate.ready({documentId:'100'});assert.deepEqual(await gateOutcome(gate.reached),{documentId:'100'});await new Promise(setImmediate);assert.equal(delivered,false);gate.release();await delivery;assert.equal(delivered,true);
});
test('QA06: reconfirmacion usa revision original verificada contra huella de solicitud, nunca revision actual',()=>{
 const planHash='a'.repeat(64),row=revision=>({estado:'CONFIRMADO',plan_id:'90',revision:String(revision),plan_hash:Buffer.from(planHash,'hex').toString('base64'),confirmed_request_hash:Buffer.from(hash('{"huella":"'+planHash+'","planId":"90","revision":'+(revision-1)+'}'),'hex').toString('base64')});
 assert.deepEqual(confirmationBinding(row(2)),{revision:1,hash:planHash});assert.equal(confirmationBinding(row(3)).revision,2);const wrong=row(2);wrong.confirmed_request_hash=Buffer.from(hash({planId:'90',revision:2,huella:planHash}),'hex').toString('base64');assert.throws(()=>confirmationBinding(wrong),/CONFIRMATION_REQUEST_HASH_INVALID/);assert.throws(()=>confirmationBinding({...row(2),revision:'1'}),/PLAN_CHANGED/);
});
test('QA09: solicitud de confirmacion coincide con serializador real de DLL instalada, sin conexion ni escritor',()=>{
 const {execFileSync}=require('node:child_process');
 const output=execFileSync('powershell.exe',['-NoProfile','-NonInteractive','-ExecutionPolicy','Bypass','-File',path.resolve(__dirname,'../tools/e2e/tests/support/doc120-confirmation-native.ps1')],{encoding:'utf8',windowsHide:true});
 const native=JSON.parse(output.replace(/^\uFEFF/,''));
 assert.equal(native.json,'{"huella":"'+'a'.repeat(64)+'","planId":"90","revision":1}');
 assert.equal(native.sha256,hash(native.json));
 assert.deepEqual(confirmationBinding({estado:'CONFIRMADO',plan_id:'90',revision:'2',plan_hash:Buffer.from('a'.repeat(64),'hex').toString('base64'),confirmed_request_hash:Buffer.from(native.sha256,'hex').toString('base64')}),{revision:1,hash:'a'.repeat(64)});
 assert.notEqual(native.sha256,hash({planId:'90',revision:1,huella:'a'.repeat(64)}));
});
test('QA07: B/C siguen orden persistido de archivo, no orden numerico del identificador cliente',()=>{
 const d=fixture(),clients=['a1',...Array.from({length:26},(_,i)=>'a'+(i+2)).sort()];d.files.filter(r=>r.lote_id==='29').forEach((r,i)=>r.cliente_archivo_id=clients[i]);const b=bind(d);assert.equal(b.B,'11');assert.equal(b.C,'12');assert.equal(b.CClient,'a11');
});
test('QA08: continuacion revisa una ventana cerrada sin efectos y rechaza cualquier recibo o guardado previo',()=>{
 const folder=fs.mkdtempSync(path.join(os.tmpdir(),'doc120-qa-review-')),previous='a'.repeat(32),file=path.join(folder,'.tmp/doc120-qa-failures',previous,'state.json');fs.mkdirSync(path.dirname(file),{recursive:true});const state={runId:previous,status:'STOPPED',closed:true,credentialsCleared:true,max:MAX,http:278,select:6,sessions:1,operations:[{operation:'ConfirmarPlan',ok:false}],effectCalls:{CONFIRM:1}};
 try{fs.writeFileSync(file,JSON.stringify(state));assert.equal(reviewPrior(folder,previous).failedConfirmations,1);for(const change of [s=>s.closed=false,s=>s.credentialsCleared=false,s=>s.operations.push({operation:'AvanzarLote',ok:false}),s=>s.effectCalls.RECEIPT=1]){const broken=structuredClone(state);change(broken);fs.writeFileSync(file,JSON.stringify(broken));assert.throws(()=>reviewPrior(folder,previous));}}
 finally{assert.equal(path.dirname(folder),os.tmpdir());assert.ok(path.basename(folder).startsWith('doc120-qa-review-'));fs.rmSync(folder,{recursive:true,force:true});}
});
