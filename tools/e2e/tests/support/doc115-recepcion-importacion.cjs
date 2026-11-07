'use strict';
const crypto=require('node:crypto');
const {safePost}=require('./doc110-safe-request.cjs');
const {validateProfile,readFixture}=require('./doc111-importacion-carpetas.cjs');
const {createPreparationClient,createPlanControls,comparePreparationDocuments}=require('./doc114-plan-importacion.cjs');
const fixture=require('../../fixtures/doc115-recepcion-cases.json');
const SCENARIOS=Object.freeze(Object.keys(fixture.scenarios));
const OPERATIONS=Object.freeze(['ConfirmarPlan','ConsultarConfirmacionLote','ConsultarLote','ConsultarRecursosLote']);
const CODES=Object.freeze(['OK','ACCESO_DENEGADO','SOLICITUD_INVALIDA','CONFIGURACION_INVALIDA','CONFLICTO_REVISION','SOLICITUD_NO_VERIFICADA','SESION_REQUERIDA','LIMITE_EXCEDIDO','ARCHIVO_RECHAZADO','FORMATO_NO_SOPORTADO','RECEPCION_OCUPADA','OPERACION_EN_CURSO','CONTENIDO_DISTINTO','RESULTADO_INCIERTO','CAPACIDAD_NO_DISPONIBLE']);
const check=(v,c)=>{if(!v)throw Error('DOC115_'+c);};
const id=v=>typeof v==='string'&&/^[1-9][0-9]{0,9}$/.test(v)&&BigInt(v)<=4294967295n;
const hash=v=>crypto.createHash('sha256').update(v).digest('hex');
const stable=v=>JSON.stringify(v,(_k,x)=>x&&typeof x==='object'&&!Array.isArray(x)?Object.fromEntries(Object.keys(x).sort().map(k=>[k,x[k]])):x);
const closed=(v,keys)=>check(v&&typeof v==='object'&&!Array.isArray(v)&&Object.keys(v).every(k=>keys.includes(k)),'FIELDS_INVALID');
function safeFailureCode(error){
  const code=error?.message;
  if(typeof code!=='string')return 'DOC115_E2E_FAILED';
  if(/^DOC115_[A-Z0-9_]{3,70}$/.test(code))return code;
  const controls=['VERSION','CABINET','PLANS','LOTS','ROOTS','DOCUMENTS','STORED_PLAN','RESUME_PLAN'];
  const failures=['AUTHENTICATION_FAILED','DSN_UNAVAILABLE','DRIVER_UNAVAILABLE','DATABASE_ACCESS_DENIED','DATABASE_UNAVAILABLE','CONNECTION_UNAVAILABLE','OPEN_FAILED','QUERY_FAILED','COLUMN_UNAVAILABLE','TABLE_UNAVAILABLE','QUERY_UNSUPPORTED','RESULT_FAILED','INPUT_FAILED','FAILED'];
  for(const label of controls)for(const failure of failures)if(code==='DOC114_CONTROL_'+label+'_'+failure)return 'DOC115_CONTROL_'+label+'_'+failure;
  for(const failure of failures)if(code==='DOC111_SELECT_'+failure)return 'DOC115_SELECT_'+failure;
  const preparation=['RESULT_NOT_CONFIRMED','TOKEN_UNAVAILABLE','CONTROL_ROW_NOT_UNIQUE','CONTROL_COUNT_INVALID','CONTROL_CABINET_INVALID','CONTROL_IDENTIFIER_INVALID','STORED_PLAN_INVALID','STORED_CONTEXT_INVALID','STORED_RESULT_CHANGED','STORED_MANIFEST_CHANGED','STORED_JSON_INVALID'];
  if(preparation.some(c=>code==='DOC114_'+c))return 'DOC115_PREPARATION_'+code.slice('DOC114_'.length);
  return 'DOC115_E2E_FAILED';
}
function validateSession(s){
  closed(s,['scenario','profile','runtimeRoot','requests','identity','preservedLotIds']);check(SCENARIOS.includes(s.scenario),'SCENARIO_INVALID');validateProfile(s.profile);
  if(s.preservedLotIds!==undefined)check(Array.isArray(s.preservedLotIds)&&s.preservedLotIds.length<=32&&s.preservedLotIds.every(id)&&new Set(s.preservedLotIds).size===s.preservedLotIds.length,'PRESERVED_LOTS_INVALID');
  closed(s.identity,['userRef','ambitoRef']);check(id(s.identity.userRef)&&id(s.identity.ambitoRef),'CONTROL_IDENTITY_REQUIRED');
  check(typeof s.runtimeRoot==='string'&&require('node:path').isAbsolute(s.runtimeRoot),'RUNTIME_ROOT_REQUIRED');
  const budget=fixture.scenarios[s.scenario];check(Array.isArray(s.requests)&&s.requests.length===budget.preparations,'REQUEST_BUDGET_INVALID');
  const bytes=readFixture(s.profile);const names=new Set();
  for(const r of s.requests){
    closed(r,['nivelRef','versionId','raices','archivos']);
    check(r.nivelRef===s.profile.nivelRef&&r.versionId===s.profile.versionId&&Array.isArray(r.raices)&&r.raices.length===1&&Array.isArray(r.archivos)&&r.archivos.length===1,'MANIFEST_INVALID');
    const root=r.raices[0],file=r.archivos[0];closed(root,['idCliente','seleccionId','nombre']);closed(file,['idCliente','raizIdCliente','nombre','rutaRelativa','bytes','ultimaModificacionLocal']);
    check(typeof root.nombre==='string'&&s.profile.rootNames.includes(root.nombre)&&!names.has(root.nombre),'ROOT_NOT_AUTHORIZED');names.add(root.nombre);
    check(typeof root.idCliente==='string'&&root.idCliente.length>0&&typeof root.seleccionId==='string'&&root.seleccionId.length>0&&file.raizIdCliente===root.idCliente&&typeof file.idCliente==='string'&&file.idCliente.length>0,'CLIENT_IDENTITIES_INVALID');
    check(file.bytes===String(bytes.length)&&typeof file.nombre==='string'&&file.nombre.length<=160&&/\.pdf$/i.test(file.nombre)&&!/[\\/|<>:"?*\x00-\x1f]/.test(file.nombre)&&file.rutaRelativa===root.nombre+'/'+file.nombre,'FILE_NOT_AUTHORIZED');
    check(typeof file.ultimaModificacionLocal==='string'&&/^(0|[1-9][0-9]{0,19})$/.test(file.ultimaModificacionLocal)&&BigInt(file.ultimaModificacionLocal)<=18446744073709551615n,'FILE_TIMESTAMP_INVALID');
  }
  return s;
}
function authorize(s,a){check(a&&['environment','account','select','effects','discardable'].every(k=>a[k]===true)&&(!s.profile.ignoreHttpsErrors||a.tls===true)&&(s.scenario!=='RECIB-03'||a.interruption===true),'AUTHORIZATION_REQUIRED');}
function assertRejectedReceipt(view){
  const f=view?.files?.[0];check(view?.files?.length===1&&f.estado_recepcion==='RECHAZADO'&&f.estado_efecto==='NO_INICIADO'&&f.intento_activo_id===null&&f.bytes_recibidos===null&&f.contenido_hash_hex===null&&f.staging_ref===null&&view.attempts.length>0&&view.attempts.every(a=>a.fase==='RECEPCION'&&a.estado==='RECHAZADO'),'REJECTION_REQUIRES_REVIEW');
}
function assertStoredReceipt(view,bytes){
  const f=view?.files?.[0];check(view?.files?.length===1&&Buffer.isBuffer(bytes)&&f.estado_recepcion==='RECIBIDO'&&f.estado_efecto==='NO_INICIADO'&&String(f.bytes_previstos)===String(bytes.length)&&String(f.bytes_recibidos)===String(bytes.length)&&typeof f.contenido_hash_hex==='string'&&f.contenido_hash_hex.toLowerCase()===hash(bytes)&&typeof f.staging_ref==='string'&&/^[a-f0-9]{32}$/.test(f.staging_ref)&&f.intento_activo_id===null&&view.attempts.filter(a=>a.estado==='CONFIRMADO'&&a.fase==='RECEPCION').length===1&&view.attempts.every(a=>a.fase==='RECEPCION'&&['CONFIRMADO','RECHAZADO'].includes(a.estado)),'STORED_RECEIPT_INVALID');
}
function assertReceiptResponse(result,loteId,archivoId,bytes){
  const r=result?.data;check(result?.ok===true&&r?.loteId===loteId&&r.archivoId===archivoId&&r.estadoRecepcion==='RECIBIDO'&&r.estadoEfecto==='NO_INICIADO'&&r.bytesRecibidos===String(bytes.length)&&r.contenidoHash===hash(bytes)&&Number.isInteger(r.revision)&&r.revision>0,'RECEIPT_RESPONSE_INVALID');
}
function interruptionObservation(value){
  check(value?.interrupted===true&&Number.isSafeInteger(value.declaredBytes)&&Number.isSafeInteger(value.sentBytes)&&value.sentBytes>0&&value.sentBytes<value.declaredBytes,'INTERRUPTION_NOT_PARTIAL');
  return {interrupted:true,declaredBytes:value.declaredBytes,sentBytes:value.sentBytes};
}
function assertInterruptedState(before,after){
  check(after?.files?.length===1&&after.files[0].estado_recepcion!=='RECIBIDO','PARTIAL_MARKED_RECEIVED');
  if(after.files[0].estado_recepcion==='RECHAZADO'){assertRejectedReceipt(after);return;}
  check(after.files[0].estado_recepcion==='PENDIENTE'&&after.files[0].estado_efecto==='NO_INICIADO'&&after.files[0].intento_activo_id===null&&stable(before)===stable(after),'INTERRUPTION_REQUIRES_REVIEW');
}
function createReceiptClient(auth,profile,budget,onReference=()=>{},authorization){
  validateProfile(profile);check(authorization&&['environment','account','select','effects','discardable'].every(k=>authorization[k]===true)&&(!profile.ignoreHttpsErrors||authorization.tls===true),'AUTHORIZATION_REQUIRED');
  let token,confirmations=0,uploads=0,uncertain=false;const lots=new Set();
  async function refreshToken(){token=await auth.page.locator('#doc111-importacion').getAttribute('data-token');check(typeof token==='string'&&token.length>0,'TOKEN_UNAVAILABLE');}
  function headers(){check(typeof token==='string'&&token.length>0,'TOKEN_UNAVAILABLE');return {'X-Importacion-Carpetas-CSRF':token,Origin:new URL(profile.baseUrl).origin};}
  async function decode(response,operation){
    let b;try{const raw=await response.json();b=raw?.d??raw;}catch{throw Error('DOC115_RESPONSE_UNCLASSIFIED');}
    check(b&&typeof b.ok==='boolean'&&CODES.includes(b.codigo)&&typeof response.status==='function','RESPONSE_INVALID');
    if(b.ok)check(response.status()===200&&b.codigo==='OK','RESPONSE_INVALID');else check(response.status()>=400,'REJECTION_STATUS_INVALID');
    return {ok:b.ok,code:b.codigo,status:response.status(),data:b.datos,operation};
  }
  async function request(operation,payload){
    check(OPERATIONS.includes(operation),'OPERATION_NOT_ALLOWED');
    if(operation==='ConfirmarPlan'){check(!uncertain,'UNCERTAIN_REQUIRES_READ');check(confirmations<budget.confirmations,'CONFIRMATION_BUDGET_EXHAUSTED');confirmations++;}
    let result;try{result=await decode(await safePost(auth.context.request,new URL('webservice/WebServiceImportacionCarpetas.asmx/'+operation,profile.baseUrl).href,{maxRedirects:0,headers:headers(),data:{solicitud:payload}},operation),operation);}catch{if(operation==='ConfirmarPlan')uncertain=true;throw Error('DOC115_RESULT_NOT_CONFIRMED');}
    if(result.ok&&operation==='ConfirmarPlan'){check(id(result.data?.loteId),'LOT_REFERENCE_INVALID');lots.add(result.data.loteId);onReference({planId:payload.planId,loteId:result.data.loteId});check(lots.size<=budget.maximumLots,'LOT_BUDGET_EXCEEDED');}
    if(operation==='ConsultarConfirmacionLote'&&result.ok)uncertain=false;
    return result;
  }
  async function upload(loteId,archivoId,bytes,key,options={}){
    check(!uncertain,'UNCERTAIN_REQUIRES_READ');check(lots.has(loteId)&&id(archivoId)&&Buffer.isBuffer(bytes)&&/^[a-zA-Z0-9_-]{1,64}$/.test(key),'UPLOAD_INVALID');
    closed(options,['extra','omitToken','loseResponse']);check(uploads<budget.uploads,'UPLOAD_BUDGET_EXHAUSTED');uploads++;
    const h=headers();if(options.omitToken)delete h['X-Importacion-Carpetas-CSRF'];
    let result;try{result=await decode(await safePost(auth.context.request,new URL('generic_control/ImportacionCarpetasUpload.ashx',profile.baseUrl).href,{maxRedirects:0,headers:h,multipart:{loteId,archivoId,claveIntento:key,...(options.extra||{}),file:{name:profile.pdfFixture,mimeType:'application/pdf',buffer:bytes}}},'RecibirArchivo'),'RecibirArchivo');}catch{uncertain=true;throw Error('DOC115_RECEIPT_NOT_CONFIRMED');}
    if(options.loseResponse){check(result.ok&&result.data?.estadoRecepcion==='RECIBIDO','LOST_RESPONSE_NOT_COMMITTED');return {responseDiscarded:true};}
    return result;
  }
  function noteInterruption(){check(!uncertain&&uploads<budget.uploads,'UPLOAD_BUDGET_EXHAUSTED');uploads++;uncertain=true;}
  function readCompleted(){uncertain=false;}
  return {refreshToken,request,upload,noteInterruption,readCompleted,counters:()=>({confirmations,uploads,lots:lots.size})};
}
function createReceiptControls(query,profile,identity){
  const base=createPlanControls(query,profile,identity);
  async function rows(sql,args){check(/^SELECT\b/i.test(sql)&&!/[;]|\b(INTO|OUTFILE|FOR\s+UPDATE)\b/i.test(sql),'SELECT_REQUIRED');const r=await query('RA',sql,args);check(Array.isArray(r),'CONTROL_INVALID');return r;}
  async function snapshot(loteId){
    check(id(loteId),'LOT_INVALID');
    const lots=await rows('SELECT lote_id,plan_id,solicitante_ref,ambito_ref,nivel_ref,config_doc,modo_integridad,ambito_integridad,total_raices,total_archivos,total_bytes,revision,estado FROM ra_integracion_lote WHERE lote_id=? AND solicitante_ref=? AND ambito_ref=? AND nivel_ref=?',[loteId,identity.userRef,identity.ambitoRef,profile.nivelRef]);
    check(lots.length===1,'LOT_NOT_OWNED');
    const files=await rows('SELECT f.archivo_id,f.raiz_id,f.estado_recepcion,f.estado_efecto,f.bytes_previstos,f.bytes_recibidos,HEX(f.contenido_hash) AS contenido_hash_hex,f.staging_ref,f.revision,f.intento_activo_id FROM ra_integracion_archivo f INNER JOIN ra_integracion_raiz r ON r.raiz_id=f.raiz_id WHERE r.lote_id=? ORDER BY f.archivo_id',[loteId]);
    const attempts=await rows('SELECT intento_id,archivo_id,fase,estado FROM ra_integracion_intento WHERE lote_id=? ORDER BY intento_id',[loteId]);
    check(files.length===1&&attempts.every(a=>a.fase==='RECEPCION')&&files.every(f=>f.estado_efecto==='NO_INICIADO'),'DOCUMENTARY_EFFECT_DETECTED');
    return {lot:lots[0],files,attempts};
  }
  async function noDocuments(before){const after=await base.snapshot();check(before.roots===after.roots&&before.documents===after.documents,'DOCUMENTARY_COUNTS_CHANGED');return after;}
  async function assertNamesUnused(requests,onReference=()=>{}){
    const names=new Set(requests.flatMap(r=>r.raices.map(root=>root.nombre)));
    const plans=await rows('SELECT plan_id,manifiesto_doc FROM ra_integracion_plan WHERE usuario_ref=? AND ambito_ref=? AND nivel_ref=? AND version_id=? ORDER BY plan_id DESC LIMIT 201',[identity.userRef,identity.ambitoRef,profile.nivelRef,profile.versionId]);
    check(plans.length<=200,'PRIOR_PLAN_REVIEW_REQUIRED');
    for(const plan of plans){let manifest;try{manifest=JSON.parse(plan.manifiesto_doc);}catch{throw Error('DOC115_PRIOR_MANIFEST_INVALID');}
      check(Array.isArray(manifest.Raices),'PRIOR_MANIFEST_INVALID');
      if(manifest.Raices.some(r=>names.has(r.Nombre))){check(id(String(plan.plan_id)),'PRIOR_PLAN_REFERENCE_INVALID');onReference({planId:String(plan.plan_id)});throw Error('DOC115_PRIOR_PLAN_REQUIRES_REVIEW');}
    }
  }
  return {snapshot,baseline:base.snapshot,noDocuments,verifyPlan:base.verifyPlan,assertNamesUnused};
}
// Abort a real HTTP request before its declared multipart body is complete.
// Session/cookie material remains ephemeral and errors never retain request data.
async function interruptUpload(auth,profile,loteId,archivoId,key,bytes,runtime={}){
  const url=new URL('generic_control/ImportacionCarpetasUpload.ashx',profile.baseUrl),boundary='doc115'+crypto.randomBytes(12).toString('hex');
  let cookies,token,body;
  try{
    cookies=await auth.context.cookies(url.href);token=await auth.page.locator('#doc111-importacion').getAttribute('data-token');check(cookies.length>0&&typeof token==='string'&&token.length>0,'INTERRUPTION_SESSION_REQUIRED');
    const fields={loteId,archivoId,claveIntento:key};let prefix='';for(const [name,value]of Object.entries(fields))prefix+='--'+boundary+'\r\nContent-Disposition: form-data; name="'+name+'"\r\n\r\n'+value+'\r\n';
    prefix+='--'+boundary+'\r\nContent-Disposition: form-data; name="file"; filename="synthetic.pdf"\r\nContent-Type: application/pdf\r\n\r\n';
    body=Buffer.concat([Buffer.from(prefix),bytes.subarray(0,Math.max(1,Math.floor(bytes.length/2)))]);
    const length=Buffer.byteLength(prefix)+bytes.length+Buffer.byteLength('\r\n--'+boundary+'--\r\n');
    await new Promise((resolve,reject)=>{
      let sent=false;const transport=require(url.protocol==='https:'?'node:https':'node:http');
      const req=(runtime.request||transport.request)(url,{method:'POST',rejectUnauthorized:!profile.ignoreHttpsErrors,headers:{Cookie:cookies.map(c=>c.name+'='+c.value).join('; '),Origin:url.origin,'X-Importacion-Carpetas-CSRF':token,'Content-Type':'multipart/form-data; boundary='+boundary,'Content-Length':length}});
      req.setTimeout(15000,()=>req.destroy());req.on('error',()=>sent?resolve():reject(Error('DOC115_INTERRUPTION_SEND_FAILED')));req.on('response',r=>{r.resume();req.destroy();reject(Error('DOC115_INTERRUPTION_TOO_LATE'));});
      req.on('close',()=>sent?resolve():reject(Error('DOC115_INTERRUPTION_SEND_FAILED')));
      req.write(body,()=>{sent=true;req.destroy();});
    });
    return {declaredBytes:length,sentBytes:body.length,interrupted:true};
  }catch{throw Error('DOC115_INTERRUPTION_UNCONFIRMED');}finally{cookies=null;token=null;if(body)body.fill(0);}
}
async function runReceiptSession(auth,session,authorization,query,onReference=()=>{},options={}){
  const progress=phase=>options.onProgress?.({phase});
  progress('VALIDATE_SESSION');
  const s=validateSession(session);authorize(s,authorization);const budget=fixture.scenarios[s.scenario],bytes=readFixture(s.profile);
  // Control IDs are checked against the server-persisted plan context below;
  // they never confer application authority or enter a write request.
  const identity=s.identity;
  const prep=createPreparationClient(auth,s.profile,{...authorization,scenario:s.scenario,maximumPreparations:budget.preparations},onReference);
  progress('READ_OPENING');await prep.refreshToken();await prep.request('AbrirImportacion',{nivelRef:s.profile.nivelRef});
  progress('READ_BASELINE');const controls=createReceiptControls(query,s.profile,identity),before=await controls.baseline();
  const preserved=[];progress('READ_PRESERVED_LOTS');for(const loteId of s.preservedLotIds||[]){const view=await controls.snapshot(loteId);preserved.push({loteId,hash:hash(stable(view))});}
  progress('READ_PRIOR_PLANS');
  await controls.assertNamesUnused(s.requests,onReference);
  const client=createReceiptClient(auth,s.profile,budget,onReference,authorization);await client.refreshToken();
  const plans=[];
  for(const r of s.requests){progress('PREPARE_PLAN');const plan=await prep.request('PrepararPlan',r);check(id(plan.planId)&&plan.resultado?.PuedeConfirmar===true&&plan.nivelRef===s.profile.nivelRef,'PLAN_NOT_CONFIRMABLE');progress('READ_STORED_PLAN');await controls.verifyPlan(plan,r);plans.push(plan);}
  const key=crypto.randomBytes(16).toString('hex'),command=(p,k=key)=>({planId:p.planId,revisionEsperada:p.revision,planHashEsperado:p.planHash,claveConfirmacion:k});
  async function success(p,k){const r=await client.request('ConfirmarPlan',command(p,k));check(r.ok,'CONFIRMATION_FAILED');return r.data.loteId;}
  let loteId;
  if(s.scenario==='RECIB-01'){
    progress('CONFIRM_PLAN');
    const pair=await Promise.all([client.request('ConfirmarPlan',command(plans[0])),client.request('ConfirmarPlan',command(plans[0]))]);
    check(pair.every(r=>r.ok||r.code==='CONFLICTO_REVISION'),'CONCURRENT_CONFIRMATION_FAILED');
    const proof=await client.request('ConsultarConfirmacionLote',{planId:plans[0].planId});check(proof.ok&&proof.data?.confirmada===true&&id(proof.data.loteId),'CONFIRMATION_NOT_FOUND');loteId=proof.data.loteId;
    check(pair.filter(r=>r.ok).every(r=>r.data.loteId===loteId),'DUPLICATE_LOT');
    check(await success(plans[0])===loteId,'CONFIRMATION_REPLAY_CHANGED');
    const changed=command(plans[0]);changed.revisionEsperada++;check((await client.request('ConfirmarPlan',changed)).code==='CONFLICTO_REVISION','CHANGED_PAYLOAD_ACCEPTED');
    check((await client.request('ConfirmarPlan',command(plans[1]))).code==='CONFLICTO_REVISION','CROSS_PLAN_KEY_ACCEPTED');
    const view=await controls.snapshot(loteId);check(String(view.lot.plan_id)===plans[0].planId&&String(view.lot.total_archivos)==='1'&&comparePreparationDocuments(JSON.parse(view.lot.config_doc),plans[0].snapshot,{},{}).snapshotEqual,'SNAPSHOT_CHANGED');
    check(view.files[0].estado_recepcion==='PENDIENTE'&&view.attempts.length===0,'CONFIRMATION_RECEIVED_BYTES');
  }else{
    progress('CONFIRM_PLAN');loteId=await success(plans[0]);let state=await controls.snapshot(loteId);const fileId=String(state.files[0].archivo_id);
    if(s.scenario==='RECIB-02'){
      const other=await success(plans[1],crypto.randomBytes(16).toString('hex')),otherState=await controls.snapshot(other),foreignId=String(otherState.files[0].archivo_id);
      const rejected=async(result,codes)=>check(!result.ok&&codes.includes(result.code),'EXPECTED_REJECTION_MISSING');
      const noMutation=async()=>{check(stable(await controls.snapshot(loteId))===stable(state)&&stable(await controls.snapshot(other))===stable(otherState),'INVALID_UPLOAD_MUTATED_RESOURCES');};
      progress('REJECT_FOREIGN_FILE');
      await rejected(await client.upload(loteId,foreignId,bytes,'cross-lot'),['ACCESO_DENEGADO']);
      await noMutation();progress('REJECT_DESTINATION_FIELDS');
      await rejected(await client.upload(loteId,fileId,bytes,'extra',{extra:{expedienteId:'1',rutaRelativa:'../outside.pdf'}}),['SOLICITUD_INVALIDA']);
      await noMutation();progress('REJECT_MISSING_TOKEN');
      await rejected(await client.upload(loteId,fileId,bytes,'token',{omitToken:true}),['SOLICITUD_NO_VERIFICADA']);
      await noMutation();progress('REJECT_TRUNCATED_FILE');
      await rejected(await client.upload(loteId,fileId,bytes.subarray(0,bytes.length-1),'truncated'),['ARCHIVO_RECHAZADO']);
      assertRejectedReceipt(await controls.snapshot(loteId));progress('REJECT_EXCESS_BYTES');
      await rejected(await client.upload(loteId,fileId,Buffer.concat([bytes,Buffer.from([0])]),'excess'),['LIMITE_EXCEDIDO']);
      assertRejectedReceipt(await controls.snapshot(loteId));progress('RECEIVE_VALID_FILE');
      assertReceiptResponse(await client.upload(loteId,fileId,bytes,'valid'),loteId,fileId,bytes);const committed=await controls.snapshot(loteId);assertStoredReceipt(committed,bytes);
      progress('REPLAY_VALID_FILE');assertReceiptResponse(await client.upload(loteId,fileId,bytes,'valid'),loteId,fileId,bytes);const changed=Buffer.from(bytes);changed[20]^=1;
      progress('REJECT_CHANGED_CONTENT');
      await rejected(await client.upload(loteId,fileId,changed,'valid'),['CONTENIDO_DISTINTO']);
      check(stable(await controls.snapshot(loteId))===stable(committed),'REPLAY_MUTATED_RECEIPT');
      check(stable(await controls.snapshot(other))===stable(otherState),'FOREIGN_LOT_CHANGED');
    }else{
      progress('INTERRUPT_TRANSFER');client.noteInterruption();const interrupted=interruptionObservation(await (options.interruptUpload||interruptUpload)(auth,s.profile,loteId,fileId,'interrupted',bytes));
      options.onInterruptionVerified?.(interrupted);progress('READ_AFTER_INTERRUPTION');
      const afterInterruption=await controls.snapshot(loteId);assertInterruptedState(state,afterInterruption);client.readCompleted();
      // The server commits the real upload; this deliberate client-side discard
      // tests replay after a lost application result, not a server crash.
      progress('RECEIVE_AND_DISCARD_RESULT');const discarded=await client.upload(loteId,fileId,bytes,'after-interruption',{loseResponse:true});check(discarded.responseDiscarded===true,'RESULT_NOT_DISCARDED');progress('READ_AFTER_DISCARDED_RESULT');const committed=await controls.snapshot(loteId);assertStoredReceipt(committed,bytes);
      progress('REPLAY_AFTER_DISCARDED_RESULT');assertReceiptResponse(await client.upload(loteId,fileId,bytes,'after-interruption'),loteId,fileId,bytes);check(stable(await controls.snapshot(loteId))===stable(committed),'LOST_RESULT_REPLAY_CHANGED');
    }
    state=await controls.snapshot(loteId);check(state.files[0].estado_recepcion==='RECIBIDO'&&String(state.files[0].bytes_recibidos)===String(bytes.length)&&state.attempts.filter(a=>a.estado==='CONFIRMADO').length===1,'RECEIPT_NOT_UNIQUE');
  }
  await controls.noDocuments(before);
  progress('VERIFY_PRESERVED_LOTS');for(const prior of preserved)check(hash(stable(await controls.snapshot(prior.loteId)))===prior.hash,'PRESERVED_LOT_CHANGED');
  return {scenario:s.scenario,status:'PASS',...client.counters(),preparations:plans.length,noDocumentEffects:true,readOnlyControls:true,httpConcurrency:'SESSION_MAY_SERIALIZE',responseLoss:s.scenario==='RECIB-03'?'CLIENT_RESULT_DISCARDED':null,...(s.scenario==='RECIB-03'?{interruptedTransferVerified:true}:{}),...(preserved.length?{preservedLotsUnchanged:true}:{})};
}
module.exports={SCENARIOS,OPERATIONS,CODES,validateSession,authorize,createReceiptClient,createReceiptControls,interruptUpload,runReceiptSession,safeFailureCode,assertRejectedReceipt,assertStoredReceipt,assertReceiptResponse,interruptionObservation,assertInterruptedState};
