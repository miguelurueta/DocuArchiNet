'use strict';
const test=require('node:test'),assert=require('node:assert/strict');
const fs=require('node:fs'),path=require('node:path'),os=require('node:os');
const {SCENARIOS,OPERATIONS,validateSession,authorize,createReceiptClient,createReceiptControls,safeFailureCode,assertRejectedReceipt,assertStoredReceipt,assertReceiptResponse,interruptUpload,interruptionObservation,assertInterruptedState}=require('../tools/e2e/tests/support/doc115-recepcion-importacion.cjs');
const {createPreparationClient}=require('../tools/e2e/tests/support/doc114-plan-importacion.cjs');
const {parseArgs,safeLine,main}=require('../tools/e2e/scripts/run-doc115-recepcion-importacion.cjs');
const budgets=require('../tools/e2e/fixtures/doc115-recepcion-cases.json');
test('every receipt client ASMX operation resolves to the existing public facade',()=>{
  const source=fs.readFileSync(path.join(__dirname,'../webservice/WebServiceImportacionCarpetas.asmx.vb'),'utf8');
  const methods=new Set([...source.matchAll(/Public Function ([A-Za-z]+)\(/g)].map(m=>m[1]));
  for(const operation of OPERATIONS)assert.ok(methods.has(operation),'Missing ASMX operation: '+operation);
  assert.ok(!OPERATIONS.includes('ConsultarConfirmacion'));
});
const profile={baseUrl:'https://test.invalid/app/',module:'GESTOR',ignoreHttpsErrors:false,timeoutMilliseconds:90000,odbcDsn:'MembershipUsers',docuarchiOdbcDsn:'MembershipUsers',nivelRef:'7',versionId:'38',mode:'LOTE',pdfFixture:'doc111-carga-individual.pdf',rootNames:['DOC115-SYNTHETIC-01','DOC115-SYNTHETIC-02'],expectations:[0,1].map(i=>({codigo:'synthetic-'+i,nombre:'synthetic-'+i,tipologiaId:'1',indices:[{referencia:'1',tipo:'TEXTO',valor:'synthetic'}]}))};
const authorization={environment:true,account:true,select:true,effects:true,discardable:true,interruption:true};
const fixtureBytes=fs.readFileSync(path.join(__dirname,'../tools/e2e/fixtures',profile.pdfFixture));
test('interruption evidence requires measured partial bytes and retains only safe numeric fields',()=>{
  assert.deepEqual(interruptionObservation({interrupted:true,declaredBytes:1000,sentBytes:400,cookie:'PRIVATE'}),{interrupted:true,declaredBytes:1000,sentBytes:400});
  for(const v of [{interrupted:true,declaredBytes:1000,sentBytes:1000},{interrupted:true,declaredBytes:1000,sentBytes:0},{interrupted:true,declaredBytes:1000,sentBytes:-1},{interrupted:false,declaredBytes:1000,sentBytes:400}])assert.throws(()=>interruptionObservation(v),/INTERRUPTION_NOT_PARTIAL/);
});
test('interrupted upload never permits retry from an active or uncertain state',()=>{
  const before={lot:{revision:'1'},files:[{estado_recepcion:'PENDIENTE',estado_efecto:'NO_INICIADO',intento_activo_id:null}],attempts:[]};
  assert.doesNotThrow(()=>assertInterruptedState(before,structuredClone(before)));
  for(const state of ['RECIBIDO','INCIERTO','RECIBIENDO']){const after=structuredClone(before);after.files[0].estado_recepcion=state;assert.throws(()=>assertInterruptedState(before,after),/^Error: DOC115_/);}
  const changed=structuredClone(before);changed.lot.revision='2';assert.throws(()=>assertInterruptedState(before,changed),/INTERRUPTION_REQUIRES_REVIEW/);
});
function fakeInterruptedTransport(mode){
  const req=new (require('node:events').EventEmitter)();let observed;
  req.setTimeout=(ms)=>{assert.equal(ms,15000);return req;};
  req.destroy=()=>{req.emit('close');return req;};
  req.write=(body,done)=>{observed.bodyReference=body;observed.written=Buffer.from(body);if(mode==='error')req.emit('error',Error('PRIVATE_COOKIE_CONNECTION_SENTINEL'));else if(mode==='early-response')req.emit('response',{resume:()=>{}});else done();};
  return {request:(url,options)=>{observed={url,options};return req;},inspect:()=>observed};
}
function interruptionAuth(){return {context:{cookies:async()=>[{name:'synthetic',value:'PRIVATE_FAKE_COOKIE'}]},page:{locator:()=>({getAttribute:async()=> 'PRIVATE_FAKE_TOKEN'})}};}
test('controlled transport sends less than Content-Length then closes and clears the temporary body',async()=>{
  const transport=fakeInterruptedTransport('partial');const result=await interruptUpload(interruptionAuth(),profile,'1','2','cut',fixtureBytes,transport);const seen=transport.inspect();
  assert.equal(result.interrupted,true);assert.ok(result.sentBytes<result.declaredBytes);assert.equal(result.sentBytes,seen.written.length);assert.equal(result.declaredBytes,seen.options.headers['Content-Length']);assert.equal(seen.options.rejectUnauthorized,true);
  assert.ok(seen.written.includes(Buffer.from('name="archivoId"')));assert.equal(seen.bodyReference.every(n=>n===0),true);assert.deepEqual(Object.keys(result).sort(),['declaredBytes','interrupted','sentBytes']);
});
test('interruption failure and early server response do not expose transport or cookie details',async()=>{
  for(const mode of ['error','early-response'])await assert.rejects(interruptUpload(interruptionAuth(),profile,'1','2','cut',fixtureBytes,fakeInterruptedTransport(mode)),e=>e.message==='DOC115_INTERRUPTION_UNCONFIRMED'&&!e.cause);
});
test('a rejected upload must be terminal without received content before the next write',()=>{
  const view={files:[{estado_recepcion:'RECHAZADO',estado_efecto:'NO_INICIADO',intento_activo_id:null,bytes_recibidos:null,contenido_hash_hex:null,staging_ref:null}],attempts:[{fase:'RECEPCION',estado:'RECHAZADO'}]};
  assert.doesNotThrow(()=>assertRejectedReceipt(view));
  for(const mutate of [v=>v.files[0].estado_recepcion='INCIERTO',v=>v.files[0].intento_activo_id='9',v=>v.files[0].bytes_recibidos='677',v=>v.attempts[0].estado='EN_CURSO']){const changed=structuredClone(view);mutate(changed);assert.throws(()=>assertRejectedReceipt(changed),/REJECTION_REQUIRES_REVIEW/);}
});
test('receipt response binds identity, actual bytes and hash without claiming document completion',()=>{
  const digest=require('node:crypto').createHash('sha256').update(fixtureBytes).digest('hex');
  const response={ok:true,data:{loteId:'1',archivoId:'2',estadoRecepcion:'RECIBIDO',estadoEfecto:'NO_INICIADO',bytesRecibidos:String(fixtureBytes.length),contenidoHash:digest,revision:3}};
  assert.doesNotThrow(()=>assertReceiptResponse(response,'1','2',fixtureBytes));
  for(const mutate of [v=>v.data.archivoId='3',v=>v.data.contenidoHash='0'.repeat(64),v=>v.data.estadoEfecto='COMPLETADO',v=>v.data.bytesRecibidos='0']){const changed=structuredClone(response);mutate(changed);assert.throws(()=>assertReceiptResponse(changed,'1','2',fixtureBytes),/RECEIPT_RESPONSE_INVALID/);}
});
test('persisted receipt requires one confirmed attempt and the exact retained content hash',()=>{
  const digest=require('node:crypto').createHash('sha256').update(fixtureBytes).digest('hex');
  const view={files:[{estado_recepcion:'RECIBIDO',estado_efecto:'NO_INICIADO',bytes_previstos:String(fixtureBytes.length),bytes_recibidos:String(fixtureBytes.length),contenido_hash_hex:digest.toUpperCase(),staging_ref:'a'.repeat(32),intento_activo_id:null}],attempts:[{fase:'RECEPCION',estado:'RECHAZADO'},{fase:'RECEPCION',estado:'CONFIRMADO'}]};
  assert.doesNotThrow(()=>assertStoredReceipt(view,fixtureBytes));
  for(const mutate of [v=>v.files[0].contenido_hash_hex='0'.repeat(64),v=>v.files[0].bytes_previstos='1',v=>v.attempts.push({fase:'RECEPCION',estado:'CONFIRMADO'}),v=>v.attempts[0].estado='INCIERTO']){const changed=structuredClone(view);mutate(changed);assert.throws(()=>assertStoredReceipt(changed,fixtureBytes),/STORED_RECEIPT_INVALID/);}
});
function session(scenario='RECIB-02') {return {scenario,profile:structuredClone(profile),runtimeRoot:path.resolve(__dirname,'..'),identity:{userRef:'1',ambitoRef:'2'},requests:profile.rootNames.slice(0,budgets.scenarios[scenario].preparations).map((nombre,i)=>({nivelRef:'7',versionId:'38',raices:[{idCliente:'r'+i,seleccionId:'s'+i,nombre}],archivos:[{idCliente:'f'+i,raizIdCliente:'r'+i,nombre:profile.pdfFixture,rutaRelativa:nombre+'/'+profile.pdfFixture,bytes:String(fixtureBytes.length),ultimaModificacionLocal:'0'}]}))};}
function auth(post){return {page:{locator:()=>({getAttribute:async()=> 'ephemeral-token'})},context:{request:{post}}};}
const response=(ok,code,data,status=ok?200:409)=>({status:()=>status,json:async()=>({ok,codigo:code,datos:data})});
test('session validation binds roots, file size, identity and exact preparation budget',()=>{
  for(const scenario of SCENARIOS)assert.equal(validateSession(session(scenario)).scenario,scenario);
  for(const mutate of [s=>s.requests.pop(),s=>s.requests[0].archivos[0].bytes='1',s=>s.requests[0].nivelRef='8',s=>s.requests[0].expedienteId='1',s=>s.requests[0].raices[0].nombre='../escape',s=>s.identity.userRef='0',s=>s.requests[0].archivos[0].ultimaModificacionLocal=null,s=>s.password='never-permitted']){const s=session();mutate(s);assert.throws(()=>validateSession(s),/^Error: DOC115_/);}
});
test('runtime authorization is independent from editorial approval and includes network interruption',()=>{
  for(const key of ['environment','account','select','effects','discardable','interruption'])assert.throws(()=>authorize(session('RECIB-03'),{...authorization,[key]:false}),/AUTHORIZATION_REQUIRED/);
  assert.doesNotThrow(()=>authorize(session(),authorization));
  assert.throws(()=>createReceiptClient(auth(()=>{}),profile,budgets.scenarios['RECIB-02']),/AUTHORIZATION_REQUIRED/);
});
test('preservation scope accepts only unique explicit lot IDs',()=>{
  assert.doesNotThrow(()=>validateSession({...session(),preservedLotIds:['6']}));
  for(const preservedLotIds of [['6','6'],['0'],[6],'6'])assert.throws(()=>validateSession({...session(),preservedLotIds}),/PRESERVED_LOTS_INVALID/);
});
test('failure classification retains only known control codes without private messages',()=>{
  for(const code of ['DOC114_CONTROL_VERSION_TABLE_UNAVAILABLE','DOC114_CONTROL_DOCUMENTS_COLUMN_UNAVAILABLE','DOC111_SELECT_AUTHENTICATION_FAILED'])assert.equal(safeFailureCode(Error(code)),code.replace(/^DOC11[14]_/, 'DOC115_'));
  for(const code of ['password=PRIVATE','DOC114_CONTROL_VERSION_PRIVATE','PRIVATE_CONNECTION_SENTINEL'])assert.equal(safeFailureCode(Error(code)),'DOC115_E2E_FAILED');
  assert.equal(safeFailureCode(Error('DOC114_CONTROL_ROW_NOT_UNIQUE')),'DOC115_PREPARATION_CONTROL_ROW_NOT_UNIQUE');
});
test('receipt client rejects documentary operations and preserves controlled upload metadata',async()=>{
  const calls=[];const a=auth(async(url,options)=>{calls.push({url,options});return url.endsWith('/ConfirmarPlan')?response(true,'OK',{loteId:'10'}):response(true,'OK',{loteId:'10',archivoId:'20',estadoRecepcion:'RECIBIDO'});});
  const client=createReceiptClient(a,profile,budgets.scenarios['RECIB-02'],()=>{},authorization);await client.refreshToken();
  for(const operation of ['AvanzarLote','ReconciliarRecurso','CerrarReservaSinInicio','GuardarBorrador'])await assert.rejects(client.request(operation,{}),/OPERATION_NOT_ALLOWED/);
  assert.deepEqual(OPERATIONS,['ConfirmarPlan','ConsultarConfirmacionLote','ConsultarLote','ConsultarRecursosLote']);
  await assert.rejects(client.upload('10','20',fixtureBytes,'first'),/UPLOAD_INVALID/);
  await client.request('ConfirmarPlan',{planId:'1'});await client.upload('10','20',fixtureBytes,'first');
  assert.equal(calls.length,2);assert.equal(calls[1].options.maxRedirects,0);assert.equal(calls[1].options.maxRetries,0);
  assert.deepEqual(Object.keys(calls[1].options.multipart),['loteId','archivoId','claveIntento','file']);assert.equal(calls[1].options.multipart.file.buffer,fixtureBytes);
});
test('unknown result blocks the next write until explicit confirmation read',async()=>{
  let failure=true;const a=auth(async(url)=>{if(failure)throw Error('PRIVATE_CONNECTION_AND_COOKIE_SENTINEL');return url.endsWith('/ConsultarConfirmacionLote')?response(true,'OK',{confirmada:true,loteId:'10'}):response(true,'OK',{loteId:'10'});});
  const client=createReceiptClient(a,profile,budgets.scenarios['RECIB-01'],()=>{},authorization);await client.refreshToken();
  await assert.rejects(client.request('ConfirmarPlan',{planId:'1'}),e=>e.message==='DOC115_RESULT_NOT_CONFIRMED'&&!e.cause);
  failure=false;await assert.rejects(client.request('ConfirmarPlan',{planId:'1'}),/UNCERTAIN_REQUIRES_READ/);
  await client.request('ConsultarConfirmacionLote',{planId:'1'});assert.equal((await client.request('ConfirmarPlan',{planId:'1'})).ok,true);
});
test('reference is recorded before detecting an exceeded lot budget',async()=>{
  const references=[];let count=0;const a=auth(async()=>response(true,'OK',{loteId:String(++count)}));
  const client=createReceiptClient(a,profile,budgets.scenarios['RECIB-01'],r=>references.push(r),authorization);await client.refreshToken();
  await client.request('ConfirmarPlan',{planId:'1'});await assert.rejects(client.request('ConfirmarPlan',{planId:'2'}),/LOT_BUDGET_EXCEEDED/);assert.equal(references.length,2);
});
test('select controls reject foreign ownership and do not expose a writer',async()=>{
  const calls=[];const control=createReceiptControls(async(origin,sql,args)=>{calls.push({origin,sql,args});return [];},profile,{userRef:'1',ambitoRef:'2'});
  await assert.rejects(control.snapshot('10'),/LOT_NOT_OWNED/);assert.equal(calls.length,1);assert.match(calls[0].sql,/^SELECT /);assert.deepEqual(calls[0].args,['10','1','2','7']);assert.equal(control.advance,undefined);
});
test('existing plan names stop preparation and preserve the reference for read recovery',async()=>{
  const references=[],calls=[];const controls=createReceiptControls(async(origin,sql,args)=>{calls.push({origin,sql,args});return [{plan_id:'50',manifiesto_doc:JSON.stringify({Raices:[{Nombre:profile.rootNames[0]}]})}];},profile,{userRef:'1',ambitoRef:'2'});
  await assert.rejects(controls.assertNamesUnused(session().requests,r=>references.push(r)),/PRIOR_PLAN_REQUIRES_REVIEW/);
  assert.deepEqual(references,[{planId:'50'}]);assert.equal(calls.length,1);assert.match(calls[0].sql,/^SELECT /);assert.deepEqual(calls[0].args,['1','2','7','38']);
});
test('preparation client accepts RECIB scope while still refusing document operations',async()=>{
  for(const scenario of SCENARIOS){const p=createPreparationClient(auth(()=>{}),profile,{...authorization,scenario,maximumPreparations:1});await assert.rejects(p.request('AvanzarLote',{}),/OPERATION_NOT_ALLOWED/);}
  assert.throws(()=>createPreparationClient(auth(()=>{}),profile,{...authorization,scenario:'OTHER',maximumPreparations:1}),/SCENARIO_INVALID/);
});
test('runner accepts only resource file and sanitizes output to closed fields',()=>{
  assert.deepEqual(parseArgs(['--session','local.json','--check']),{session:'local.json',check:true});
  assert.throws(()=>parseArgs(['--session','a','--password','x']),/ARGUMENT_INVALID/);
  assert.equal(safeLine('SECRET SENTINEL','RECIB-01'),null);
  assert.deepEqual(safeLine('DOC115_REFERENCE='+JSON.stringify({scenario:'RECIB-01',planId:'1',cookie:'PRIVATE'}),'RECIB-01'),{prefix:'DOC115_REFERENCE=',value:{scenario:'RECIB-01',planId:'1'}});
  assert.equal(safeLine('DOC115_FAILURE='+JSON.stringify({scenario:'RECIB-01',stage:'LOGIN',code:'PRIVATE VALUE'}),'RECIB-01'),null);
});
test('runner retains a positive preservation proof and rejects a false one',()=>{
  const result={scenario:'RECIB-02',status:'PASS',preparations:2,confirmations:2,uploads:8,lots:2,noDocumentEffects:true,readOnlyControls:true,httpConcurrency:'SESSION_MAY_SERIALIZE',responseLoss:null,preservedLotsUnchanged:true,head:'a'.repeat(40),codeSha256:'b'.repeat(64),backendSha256:'c'.repeat(64),privateField:'PRIVATE_SENTINEL'};
  const safe=safeLine('DOC115_RESULT='+JSON.stringify(result),'RECIB-02');assert.equal(safe.value.preservedLotsUnchanged,true);assert.equal(safe.value.privateField,undefined);
  assert.equal(safeLine('DOC115_RESULT='+JSON.stringify({...result,preservedLotsUnchanged:false}),'RECIB-02'),null);
});
test('RECIB-03 result requires both interrupted transport and the explicit client-discard mode',()=>{
  const result={scenario:'RECIB-03',status:'PASS',preparations:1,confirmations:1,uploads:3,lots:1,noDocumentEffects:true,readOnlyControls:true,httpConcurrency:'SESSION_MAY_SERIALIZE',responseLoss:'CLIENT_RESULT_DISCARDED',interruptedTransferVerified:true,head:'a'.repeat(40),codeSha256:'b'.repeat(64),backendSha256:'c'.repeat(64)};
  assert.equal(safeLine('DOC115_RESULT='+JSON.stringify(result),'RECIB-03').value.interruptedTransferVerified,true);
  assert.equal(safeLine('DOC115_RESULT='+JSON.stringify({...result,interruptedTransferVerified:false}),'RECIB-03'),null);
  assert.equal(safeLine('DOC115_RESULT='+JSON.stringify({...result,responseLoss:null}),'RECIB-03'),null);
});
test('check mode validates synthetic resources without login, SQL or prompting',async()=>{
  const dir=fs.mkdtempSync(path.join(os.tmpdir(),'doc115-check-'));try{const file=path.join(dir,'session.json');fs.writeFileSync(file,JSON.stringify(session()));await main(['--session',file,'--check'],{requireInteractiveConsole:()=>{throw Error('must not prompt');}});}finally{assert.equal(path.dirname(dir),path.resolve(os.tmpdir()));assert.ok(path.basename(dir).startsWith('doc115-check-'));fs.rmSync(dir,{recursive:true,force:true});}
});
