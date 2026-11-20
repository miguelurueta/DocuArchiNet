'use strict';
const test=require('node:test'),assert=require('node:assert/strict');
const {MAX,RESUME_MAX,SQL,bind,bindResumed,confirmationBinding,EffectsGate,fingerprint,receiptPayload,failure,hash}=require('../tools/e2e/tests/support/doc120-qa-window.cjs');
const {heldGate,gateOutcome,waitConsultReady,observeQaTree,independentTreeFailure,reviewPrior,reviewAfterUi}=require('../tools/e2e/scripts/run-doc120-qa-failures.cjs');
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
function resumedFixture(){
 const d=fixture(),q=d.files.filter(f=>f.lote_id==='29'),main=d.files.filter(f=>f.lote_id==='50');
 d.roots[0].raiz_id='49';d.roots[1].raiz_id='50';d.roots[2].raiz_id='71';q.forEach((f,i)=>{f.archivo_id=String(50+i);f.raiz_id=i?'50':'49';});
 main.forEach((f,i)=>{f.archivo_id=String(97+i);f.raiz_id='71';if(i<23){f.estado_efecto='COMPLETADO';f.resultado_doc='{}';}});
 const bindings={A:'50',B:'51',C:'52',createRoot:'50',D:'120',E:'121'},confirmed=[{resourceId:'50',documentId:'16282',inventoryId:'23412',expedientId:'681',relationId:'1105'},{resourceId:'52',documentId:'16283',inventoryId:'23413',expedientId:'694',relationId:'1118'}];
 const review={type:'AFTER_UI_STORES',bindings,confirmed,before:{completed29:0,completed50:23,fingerprint29:fingerprint(q,['50','52']),fingerprint50:fingerprint(main,['120','121'])}};
 for(const ack of confirmed){const f=q.find(r=>r.archivo_id===ack.resourceId);Object.assign(f,{estado_recepcion:'RECIBIDO',bytes_recibidos:'677',has_staging:'1',estado_efecto:'COMPLETADO',resultado_doc:JSON.stringify({Estado:'CONFIRMADO',DocumentoId:ack.documentId,InventarioId:ack.inventoryId,Expediente:{ExpedienteId:ack.expedientId,RelacionNivelId:ack.relationId}})});}
 Object.assign(d.roots[1],{estado:'EXPEDIENTE_CONFIRMADO',identidad_expediente_doc:JSON.stringify({ExpedienteId:'694',RelacionNivelId:'1118'})});return{d,review};
}
test('QA10: continuacion preserva 16282/16283 y solo permite cuatro llamadas restantes sobre 120/121',()=>{
 const {d,review}=resumedFixture(),b=bindResumed(d,review),gate=new EffectsGate(b);
 for(const [op,p] of [['RecibirArchivo',{loteId:'29',archivoId:b.C,claveIntento:'a'.repeat(32)}],['ConfirmarPlan',{}],['AvanzarLote',command('29',b.A)],['AvanzarLote',command('29',b.C)],['AvanzarLote',command('29',b.createRoot,'CREAR_EXPEDIENTE')]])assert.throws(()=>gate.permit(op,p),/RESUME_MUTATION_DENIED/);
 gate.permit('AvanzarLote',command('50',b.D));for(let i=0;i<3;i++)gate.permit('AvanzarLote',command('50',b.E));assert.throws(()=>gate.permit('AvanzarLote',command('50',b.D)));assert.throws(()=>gate.permit('AvanzarLote',command('50',b.E)));
 assert.equal(RESUME_MAX.receipts,0);assert.equal(RESUME_MAX.creations,0);assert.equal(RESUME_MAX.storageCalls,4);assert.equal(RESUME_MAX.confirmationCalls,0);
});
test('QA11: cambios en documentos confirmados, B, ajenos o siguientes impiden continuar antes de HTTP',()=>{
 for(const change of [d=>d.files[0].resultado_doc='{}',d=>d.files[1].has_staging='1',d=>d.files[4].revision='changed',d=>d.files.find(f=>f.archivo_id==='120').estado_efecto='COMPLETADO',d=>d.roots[1].identidad_expediente_doc='{}']){const {d,review}=resumedFixture();change(d);assert.throws(()=>bindResumed(d,review));}
 assert.equal(failure(Object.assign(Error('PRIVATE_SENTINEL'),{name:'TimeoutError'})),'DOC120_QA_UI_TIMEOUT');
});
test('QA12: revision especifica de la corrida con efectos conserva identidad/consumo y rechaza reprocesarla',()=>{
 const folder=fs.mkdtempSync(path.join(os.tmpdir(),'doc120-qa-resume-')),runId='95b22a23844453a3529c4908060be669',file=path.join(folder,'.tmp/doc120-qa-failures',runId,'state.json');fs.mkdirSync(path.dirname(file),{recursive:true});const {review}=resumedFixture();
 const operations=[['ConfirmarPlan',null,null],['ConfirmarPlan',null,null],['RecibirArchivo','52',null],['AvanzarLote','50','MATERIALIZAR_VALORES'],['AvanzarLote','50','ALMACENAR'],['AvanzarLote','50','CREAR_EXPEDIENTE'],['AvanzarLote','52','MATERIALIZAR_VALORES'],['AvanzarLote','52','ALMACENAR']].map(([operation,resourceId,phase])=>({operation,resourceId,phase,lotId:operation==='ConfirmarPlan'?null:'29',http:200,ok:true,...(phase==='ALMACENAR'?review.confirmed.find(a=>a.resourceId===resourceId):{})}));
 const state={runId,max:MAX,status:'STOPPED',failureStage:'UI_PAUSE_WAITING_FOR_RESPONSE',closed:true,credentialsCleared:true,previousStatePreserved:true,http:582,select:12,sessions:1,before:review.before,bindings:review.bindings,results:{confirmedPlanConvergence:{sameLot:true},pauseAtMissing:{paused:true}},operations,effectCalls:{CONFIRM:2,RECEIPT:1,'29:50:MATERIALIZAR_VALORES':1,'29:50:ALMACENAR':1,'29:50:CREAR_EXPEDIENTE':1,'29:52:MATERIALIZAR_VALORES':1,'29:52:ALMACENAR':1}};
 try{fs.writeFileSync(file,JSON.stringify(state));const parsed=reviewAfterUi(folder,runId);assert.equal(parsed.confirmed.length,2);assert.equal(parsed.select,12);assert.throws(()=>reviewPrior(folder,runId),/SPECIFIC_RESUME/);
  const childId='c4afaca21fe2bb90d54f9b0ddeea4750',childFile=path.join(folder,'.tmp/doc120-qa-failures',childId,'state.json'),child={runId:childId,status:'STOPPED',failure:'DOC120_QA_UI_TIMEOUT',failureStage:'VERIFY_LOT29_RECOVERY_READONLY',failureLoadStep:'QUERY_LOT_29',closed:true,credentialsCleared:true,previousStatePreserved:true,http:278,select:5,sessions:1,operations:[],effectCalls:{},previousReview:{runId,sha256:parsed.sha256},before:{...state.before,completed29:2}};fs.mkdirSync(path.dirname(childFile),{recursive:true});fs.writeFileSync(childFile,JSON.stringify(child));
  assert.deepEqual(reviewAfterUi(folder,childId).cumulative,{http:860,select:17,sessions:2});assert.equal(reviewAfterUi(folder,childId).parentReview.sha256,parsed.sha256);
  const latestId='9bad855f684d064ed35d8a7667e13151',latestFile=path.join(folder,'.tmp/doc120-qa-failures',latestId,'state.json'),latest={...child,runId:latestId,http:283,select:9,failure:'DOC120_QA_TREE_RESULT_MISSING',failureLoadStep:'LOADED_29',previousReview:{runId:childId,sha256:reviewAfterUi(folder,childId).sha256}};fs.mkdirSync(path.dirname(latestFile),{recursive:true});fs.writeFileSync(latestFile,JSON.stringify(latest));
  assert.deepEqual(reviewAfterUi(folder,latestId).cumulative,{http:1143,select:26,sessions:3});assert.equal(reviewAfterUi(folder,latestId).ancestorReviews[0].sha256,parsed.sha256);
  for(const change of [s=>s.operations.push({operation:'AvanzarLote'}),s=>s.effectCalls.RECEIPT=1,s=>s.previousReview.sha256='a'.repeat(64),s=>s.credentialsCleared=false]){const broken=structuredClone(child);change(broken);fs.writeFileSync(childFile,JSON.stringify(broken));assert.throws(()=>reviewAfterUi(folder,childId));}
  for(const change of [s=>s.operations.push(s.operations[0]),s=>s.operations[4].documentId='999',s=>s.closed=false,s=>s.select=13,s=>s.effectCalls.RECEIPT=2]){const broken=structuredClone(state);change(broken);fs.writeFileSync(file,JSON.stringify(broken));assert.throws(()=>reviewAfterUi(folder,runId));}
 }finally{assert.equal(path.dirname(folder),os.tmpdir());assert.ok(path.basename(folder).startsWith('doc120-qa-resume-'));fs.rmSync(folder,{recursive:true,force:true});}
});
test('QA13: consulta con recibidos guardados y original pendiente queda lista sin boton heredado Actualizar estado',async()=>{
 const {fixture,select,click}=require('./support/doc120-coordinacion-browser.cjs');
 await fixture(async page=>{
  await select(page,1,1);await click(page,'confirmar-importar');await page.getByRole('heading',{name:'Importación completada',exact:true}).waitFor();await page.getByRole('button',{name:'Cerrar',exact:true}).first().click();
  await page.evaluate(()=>{doc120Independent=true;files.push({...files[0],recursoId:'79',idCliente:'a2',nombre:'pendiente.pdf',rutaRelativa:'Carpeta-0/pendiente.pdf',estadoRecepcion:'PENDIENTE',estado:'NO_INICIADO',documentoId:null,bytesRecibidos:null,valoresEfectivosFijados:false,accionesPermitidas:['RECIBIR']});roots[0].estado='EXPEDIENTE_CONFIRMADO';rev++;});
  const before=await page.evaluate(()=>calls.filter(c=>['AvanzarLote','RECEPCION','ConfirmarPlan'].includes(c.operation)).length);
  await page.locator('[data-doc111-resume]').click();
  await waitConsultReady(page);assert.equal(await page.getByRole('button',{name:'Actualizar estado',exact:true}).count(),0);
  assert.equal(await page.locator('[data-doc111-action="continuar-importacion"]').isEnabled(),true);
  assert.equal(await page.evaluate(()=>calls.filter(c=>['AvanzarLote','RECEPCION','ConfirmarPlan'].includes(c.operation)).length),before);
 });
});
test('QA14: imagen/texto WebForms representan un nodo; una relacion logica duplicada sigue siendo fallo',async()=>{
 const {chromium}=require('../tools/e2e/node_modules/playwright'),browser=await chromium.launch({headless:true});
 try{const page=await browser.newPage();await page.setContent('<div id="TreeViewArchivo"><a id="TreeViewArchivot7i" title="1118|7|694|"><span></span></a><a id="TreeViewArchivot7" title="1118|7|694|">CODIGO-PRUEBA</a></div>');
  const input={identity:{RelacionNivelId:'1118',ExpedienteId:'694'},level:'7',code:'CODIGO-PRUEBA'};assert.deepEqual(await page.evaluate(observeQaTree,input),{count:1,matchingAnchors:2,labelMatches:true});assert.equal((await page.evaluate(observeQaTree,{...input,code:'OTRO'})).labelMatches,false);
  await page.evaluate(()=>{const node=document.querySelector('#TreeViewArchivot7').cloneNode(true);node.id='TreeViewArchivot8';document.querySelector('#TreeViewArchivo').append(node);});assert.deepEqual(await page.evaluate(observeQaTree,input),{count:2,matchingAnchors:3,labelMatches:false});
 }finally{await browser.close();}
});

test('QA15: solo fallo visual con checkpoints RA/DA permite continuar casos independientes sin ocultarlo',()=>{
 const error=Error('DOC120_QA_TREE_RESULT_MISSING'),state={operations:[],results:{uiRecoveryCheckpoint:{A:{documentId:'16282'},C:{documentId:'16283'},reload:true,noRepeatedEffects:true}}};assert.equal(independentTreeFailure(error,state),true);
 for(const change of [s=>s.guardFailure='BLOCKED',s=>s.operations.push({operation:'AvanzarLote'}),s=>s.results.uiRecoveryCheckpoint.C.documentId='999',s=>s.results.uiRecoveryCheckpoint.reload=false]){const broken=structuredClone(state);change(broken);assert.equal(independentTreeFailure(error,broken),false);}assert.equal(independentTreeFailure(Error('DOC120_QA_UI_TIMEOUT'),state),false);
});
