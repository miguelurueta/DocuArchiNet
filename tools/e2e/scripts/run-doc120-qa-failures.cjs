'use strict';
for(const k of ['DEBUG','PWDEBUG','NODE_DEBUG','NODE_DEBUG_NATIVE'])delete process.env[k];
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const tty=require('./support/interactive-e2e-console.cjs'),{queryRows}=require('./support/doc32-e2e-odbc.cjs');
const {createWorkflowLoginPageSession}=require('../tests/support/authenticated-workflow-session.cjs');
const {classifyLoginMessage,classifyLoginPostback}=require('../tests/support/doc111-importacion-carpetas.cjs');
const {readPrepared}=require('../tests/support/doc119-acceptance-data.cjs');
const {selectLevel}=require('../tests/support/doc113-ventana-importacion.cjs');
const {responseFor,permittedRequest,STATIC_DEPENDENCIES}=require('../tests/support/doc119-seguimiento.cjs');
const {loginRequestAction}=require('./apply-doc120-egresos-length.cjs');
const {parseTiming}=require('./measure-doc120-three-files.cjs');
const {MAX,RESUME_MAX,SQL,check,id,hash,one,fingerprint,bind,bindResumed,confirmationBinding,EffectsGate,receiptPayload,failure}=require('../tests/support/doc120-qa-window.cjs');
const ROOT=path.resolve(__dirname,'../../..'),BASE='https://localhost/GestionDocumental-Docuarchi.net/';
const BINARY='8b27a942458d41aa87d8ddd0df684bf70b7b89ec7702f0607958169a91cc571d';
const SOURCES=['bin/GestionDocumental-Docuarchi.net.dll','js/gestion/ImportacionCarpetas/ImportacionCarpetas.js','js/gestion/ImportacionCarpetas/ImportacionCarpetasVista.js','js/gestion/ImportacionCarpetas/ImportacionCarpetasApi.js','tools/e2e/scripts/run-doc120-qa-failures.cjs','tools/e2e/tests/support/doc120-qa-window.cjs','tools/e2e/scripts/measure-doc120-three-files.cjs'];
const wait=ms=>new Promise(r=>setTimeout(r,ms));
function heldGate(){let ready,release;const reached=new Promise(r=>ready=r),continued=new Promise(r=>release=r);return{ready,release,reached,continued};}
async function gateOutcome(promise){let timer;try{return await Promise.race([promise,new Promise((_,reject)=>{timer=setTimeout(()=>reject(Error('DOC120_QA_HELD_RESPONSE_TIMEOUT')),60000);})]);}finally{clearTimeout(timer);}}
async function waitConsultReady(page){
 await page.locator('[data-doc111-action="continuar-importacion"]').waitFor();
 await page.waitForFunction(()=>{const b=document.querySelector('[data-doc111-action="continuar-importacion"]');return b&&!b.disabled;});
}
async function consultQa(page,lotId){
 const r=await responseFor(page,'ConsultarRecursosLote',async()=>{
  const access='[data-doc111-query-open]';if(await page.locator(access).count()){if(!await page.locator(access).isVisible()&&await page.locator('#doc110-navigation-toggle').count())await page.locator('#doc110-navigation-toggle').click();await page.locator(access).click();}
  await page.locator('[data-doc111-lote]').fill(lotId);await page.locator('[data-doc111-consultar]').click();
 },'ARCHIVOS');
 check(r.status===200&&r.body?.ok&&r.body.datos?.loteId===lotId,'QUERY_FAILED');
 // Both fixed lots have pending files. The current view can remove its legacy
 // Actualizar estado button when the primary action becomes Continuar.
 await waitConsultReady(page);return r.body.datos;
}
function reviewPrior(root,previous){
 check(/^[a-f0-9]{32}$/.test(previous),'PREVIOUS_INVALID');const file=path.join(root,'.tmp/doc120-qa-failures',previous,'state.json'),bytes=fs.readFileSync(file),s=JSON.parse(bytes);
 check(s.runId===previous&&s.closed===true&&s.credentialsCleared===true&&s.status==='STOPPED'&&s.http<=s.max.http&&s.select<=s.max.select&&s.sessions<=s.max.sessions,'PREVIOUS_UNCLOSED');
 check(s.operations.every(r=>r.operation==='ConfirmarPlan'&&r.ok===false)&&Object.keys(s.effectCalls||{}).every(k=>k==='CONFIRM'),'PREVIOUS_EFFECTS_REQUIRE_SPECIFIC_RESUME');
 return{runId:previous,file,sha256:hash(bytes),http:s.http,select:s.select,sessions:s.sessions,failedConfirmations:s.operations.length};
}
function reviewAfterUi(root,previous){
 if(previous==='c4afaca21fe2bb90d54f9b0ddeea4750'){
  const file=path.join(root,'.tmp/doc120-qa-failures',previous,'state.json'),bytes=fs.readFileSync(file),s=JSON.parse(bytes),parent=reviewAfterUi(root,'95b22a23844453a3529c4908060be669');
  check(s.runId===previous&&s.closed===true&&s.credentialsCleared===true&&s.previousStatePreserved===true&&s.status==='STOPPED'&&s.failure==='DOC120_QA_UI_TIMEOUT'&&s.failureStage==='VERIFY_LOT29_RECOVERY_READONLY'&&s.failureLoadStep==='QUERY_LOT_29'&&s.http===278&&s.select===5&&s.sessions===1,'READONLY_CONTINUATION_INVALID');
  check(s.operations.length===0&&Object.keys(s.effectCalls||{}).length===0&&s.previousReview.runId===parent.runId&&s.previousReview.sha256===parent.sha256&&s.before.completed29===2&&s.before.completed50===23&&s.before.fingerprint29===parent.before.fingerprint29&&s.before.fingerprint50===parent.before.fingerprint50,'READONLY_CONTINUATION_CHANGED');
  return{...parent,runId:previous,file,sha256:hash(bytes),http:s.http,select:s.select,sessions:s.sessions,parentReview:{runId:parent.runId,file:parent.file,sha256:parent.sha256},cumulative:{http:parent.http+s.http,select:parent.select+s.select,sessions:parent.sessions+s.sessions}};
 }
 check(previous==='95b22a23844453a3529c4908060be669','RESUME_RUN_INVALID');
 const file=path.join(root,'.tmp/doc120-qa-failures',previous,'state.json'),bytes=fs.readFileSync(file),s=JSON.parse(bytes);
 check(s.runId===previous&&s.closed===true&&s.credentialsCleared===true&&s.previousStatePreserved===true&&s.status==='STOPPED'&&s.failureStage==='UI_PAUSE_WAITING_FOR_RESPONSE'&&s.http===582&&s.select===12&&s.sessions===1,'RESUME_PREVIOUS_INVALID');
 check(s.results.confirmedPlanConvergence?.sameLot===true&&s.results.pauseAtMissing?.paused===true,'RESUME_PREVIOUS_INVALID');
 const expected=[['ConfirmarPlan',null,null],['ConfirmarPlan',null,null],['RecibirArchivo','52',null],['AvanzarLote','50','MATERIALIZAR_VALORES'],['AvanzarLote','50','ALMACENAR'],['AvanzarLote','50','CREAR_EXPEDIENTE'],['AvanzarLote','52','MATERIALIZAR_VALORES'],['AvanzarLote','52','ALMACENAR']];
 check(s.operations.length===expected.length&&s.operations.every((r,i)=>r.ok===true&&r.http===200&&r.operation===expected[i][0]&&r.resourceId===expected[i][1]&&r.phase===expected[i][2]&&(r.operation==='ConfirmarPlan'||r.lotId==='29')),'RESUME_PREVIOUS_EFFECTS_INVALID');
 const calls={CONFIRM:2,RECEIPT:1,'29:50:MATERIALIZAR_VALORES':1,'29:50:ALMACENAR':1,'29:50:CREAR_EXPEDIENTE':1,'29:52:MATERIALIZAR_VALORES':1,'29:52:ALMACENAR':1};
 check(JSON.stringify(s.effectCalls)===JSON.stringify(calls)&&s.before.completed29===0&&s.before.completed50===23,'RESUME_PREVIOUS_EFFECTS_INVALID');
 const confirmed=s.operations.filter(r=>r.phase==='ALMACENAR');check(confirmed.every(r=>['documentId','inventoryId','expedientId','relationId'].every(k=>id(r[k])))&&confirmed[0].documentId==='16282'&&confirmed[1].documentId==='16283','RESUME_PREVIOUS_DOCUMENTS_INVALID');
 return{type:'AFTER_UI_STORES',runId:previous,file,sha256:hash(bytes),http:s.http,select:s.select,sessions:s.sessions,bindings:s.bindings,before:s.before,confirmed};
}
async function main(args=process.argv.slice(2)){
 check([3,5].includes(args.length)&&args[0]==='--run'&&/^[a-f0-9]{32}$/.test(args[1])&&args[2]==='--authorized-qa'&&(args.length===3||['--reviewed-previous','--resume-after-ui'].includes(args[3])),'AUTHORIZATION_REQUIRED');tty.requireInteractiveConsole();
 check(args.length!==5||args[1]!==args[4],'PREVIOUS_INVALID');const resume=args[3]==='--resume-after-ui',previous=args.length===5?(resume?reviewAfterUi(ROOT,args[4]):reviewPrior(ROOT,args[4])):null,caps=resume?RESUME_MAX:MAX;
 if(previous){const link=path.join(ROOT,'.tmp/doc120-qa-failures/continuations',previous.runId+'.json');fs.mkdirSync(path.dirname(link),{recursive:true});check(!fs.existsSync(link),'PREVIOUS_ALREADY_CONTINUED');fs.writeFileSync(link,JSON.stringify({previous:previous.runId,next:args[1],stateSha256:previous.sha256}),{flag:'wx'});}
 const folder=path.join(ROOT,'.tmp/doc120-qa-failures',args[1]);check(!fs.existsSync(folder),'RUN_EXISTS');fs.mkdirSync(folder,{recursive:true});
 const state={runId:args[1],status:'PREPARING',max:caps,http:0,select:0,sessions:0,operations:[],timings:[],results:{},previousReview:previous,closed:false},secrets={},contexts=[];
 const save=()=>fs.writeFileSync(path.join(folder,'state.json'),JSON.stringify(state,null,2)+'\n'),stage=v=>{state.status=v;save();console.log('DOC120_QA_PROGRESS='+v);};save();
 let environment,browser,binding,effects,started=0,cabinet,expectedSources,holdingC=null,losingD=false;const pendingGates=[];
 const assertLive=()=>{check(!state.guardFailure,'GUARD_STOPPED');check(!started||Date.now()-started<caps.milliseconds,'WINDOW_EXPIRED');};
 const charge=kind=>{assertLive();check(state[kind]<caps[kind],'BUDGET_EXHAUSTED');state[kind]++;save();};
 const priorIntact=()=>!previous||[previous,previous.parentReview].filter(Boolean).every(p=>hash(fs.readFileSync(p.file))===p.sha256);
 const unchanged=()=>{check(SOURCES.every(f=>hash(fs.readFileSync(path.join(ROOT,f)))===expectedSources[f]),'SOURCE_CHANGED');check(priorIntact(),'PREVIOUS_STATE_CHANGED');};
 const query=async(sql,parameters,prefix='DOC120_E2E')=>{charge('select');return queryRows(sql,parameters,environment,prefix);};
 const snapshot=async()=>{const data={};for(const key of ['headers','roots','files'])data[key]=await query(SQL[key],['29','50','141','2']);return data;};
 const safeBody=(operation,p,response,body)=>{const row=effects.observe(operation,p,response.status(),body);if(row){state.operations.push(row);save();}if(!body?.ok&&operation==='AvanzarLote'&&!(p.loteId==='50'&&p.recursoId===binding.E)){state.serviceFailure={phase:p.fase,code:/^[A-Z][A-Z0-9_]{0,63}$/.test(body?.codigo||'')?body.codigo:'UNKNOWN'};save();}return body;};
 const captureTiming=(operation,p,response,body,elapsedMs)=>{if(operation==='AvanzarLote'&&p.fase==='ALMACENAR'&&response.status()===200&&body?.ok){state.timings.push({lotId:p.loteId,fileId:p.recursoId,elapsedMs,timing:parseTiming(response.headers()['server-timing'])});save();}};
 async function verifyFile(lot,fileId){
  const rows=await query(SQL.files,['29','50','141','2']),row=one(rows,r=>String(r.lote_id)===lot&&String(r.archivo_id)===fileId);check(row.estado_efecto==='COMPLETADO'&&!Number(row.active_marker)&&row.resultado_doc,'CHECKPOINT_UNCONFIRMED');
  const result=JSON.parse(row.resultado_doc);check(result.Estado==='CONFIRMADO'&&id(result.DocumentoId)&&id(result.InventarioId)&&id(result.Expediente?.ExpedienteId),'CHECKPOINT_UNCONFIRMED');
  const da=await query('SELECT g.ID AS documento,g.ID_EXPEDIENTE AS expediente,g.ID_INVENTARIO_DOCUMENTAL AS inventario FROM `'+cabinet+'` g INNER JOIN registro_producion_documental r ON r.ID_REGISTRO_PRODUCION_DOCUMENTAL=g.ID_INVENTARIO_DOCUMENTAL WHERE g.ID=? AND g.ID_EXPEDIENTE=? AND g.ID_INVENTARIO_DOCUMENTAL=? AND r.ID_EMPRESA_DOCUMENTO=? AND r.ID_USUARIO_GESTION=? LIMIT 2',[result.DocumentoId,result.Expediente.ExpedienteId,result.InventarioId,'2','141'],'DOC120_DA_E2E');
  check(da.length===1&&String(da[0].documento)===result.DocumentoId,'DA_UNCONFIRMED');return{fileId,documentId:result.DocumentoId,inventoryId:result.InventarioId,expedientId:result.Expediente.ExpedienteId,relationId:result.Expediente.RelacionNivelId};
 }
 async function api(auth,operation,p,{allowFailure=false}={}){
  assertLive();unchanged();effects.permit(operation,p,auth.role);charge('http');const began=performance.now(),response=await auth.context.request.post(new URL('webservice/WebServiceImportacionCarpetas.asmx/'+operation,BASE).href,{maxRedirects:0,maxRetries:0,timeout:operation==='AvanzarLote'?0:30000,headers:{Origin:new URL(BASE).origin,'X-Importacion-Carpetas-CSRF':auth.token,...(operation==='AvanzarLote'&&p.fase==='ALMACENAR'?{'X-DOC120-Medicion':state.runId}:{})},data:{solicitud:p}});
  const elapsedMs=performance.now()-began,raw=await response.json(),body=safeBody(operation,p,response,raw?.d??raw);captureTiming(operation,p,response,body,elapsedMs);if(!allowFailure)check(response.status()===200&&body.ok,'API_REJECTED');return{status:response.status(),body};
 }
 async function install(context,role){
  await context.route('**/*',async route=>{
   const request=route.request();try{
    assertLive();const target=new URL(request.url()),base=new URL(BASE),method=request.method();
    if(loginRequestAction(request)==='OMIT_FONT'||(method==='GET'&&target.pathname==='/favicon.ico'))return route.abort();
    if(target.origin!==base.origin){check(STATIC_DEPENDENCIES.some(d=>d.url===target.href)&&method==='GET','OUTSIDE_ORIGIN');charge('http');const headers=Object.fromEntries(Object.entries(await request.allHeaders()).filter(([k])=>!['cookie','authorization','proxy-authorization','referer','origin'].includes(k.toLowerCase())));return route.continue({headers});}
    if(loginRequestAction(request)==='OMIT_LANDING')return route.fulfill({status:200,contentType:'text/html',body:'<!doctype html><html></html>'});
    const relative=target.pathname.slice(base.pathname.length).toLowerCase();
    if(relative.startsWith('webservice/webserviceimportacioncarpetas.asmx/')){
     check(binding&&method==='POST'&&!target.search,'SERVICE_SCOPE_DENIED');const envelope=JSON.parse(request.postData());check(Object.keys(envelope).join(',')==='solicitud','REQUEST_INVALID');const operation=target.pathname.split('/').pop(),p=envelope.solicitud;effects.permit(operation,p,role);if(operation==='AvanzarLote')unchanged();charge('http');
     const measured=operation==='AvanzarLote'&&p.fase==='ALMACENAR',began=performance.now(),response=await route.fetch({maxRedirects:0,maxRetries:0,timeout:operation==='AvanzarLote'?0:30000,...(measured?{headers:{...(await request.allHeaders()),'X-DOC120-Medicion':state.runId}}:{})});const elapsedMs=performance.now()-began,raw=await response.json(),body=safeBody(operation,p,response,raw?.d??raw);captureTiming(operation,p,response,body,elapsedMs);
     if(operation==='AvanzarLote'&&p.fase==='ALMACENAR'&&((p.loteId==='29'&&p.recursoId===binding.C&&holdingC)||(p.loteId==='50'&&p.recursoId===binding.D&&losingD))){
      check(response.status()===200&&body.ok&&body.datos?.estado==='CONFIRMADO','HELD_OUTCOME_UNCONFIRMED');const proof=await verifyFile(p.loteId,p.recursoId);
      if(p.loteId==='50'){state.results.responseDeliveryBoundary={...proof,boundary:'BROWSER_RESPONSE_AFTER_SERVER_CHECKPOINT',responseSuppressed:true};losingD=false;save();return route.abort('failed');}
      const held=holdingC;holdingC=null;held.ready(proof);await held.continued;
     }
     return route.fulfill({response});
    }
    let payload=null;if(method==='POST'&&relative==='gestion/webformproduciondocumental.aspx')payload=Object.fromEntries(new URLSearchParams(request.postData()||''));else if(method==='POST'&&relative==='webservice/webserviceloginsegundofactor.asmx/obtenerestado')payload=JSON.parse(request.postData());
    check(permittedRequest(method,request.url(),payload,{level:'7',alternateLevel:'223',allowTree:role==='OWNER'}),'HTTP_SCOPE_DENIED');charge('http');await route.continue();
   }catch(e){state.guardFailure=failure(e);save();try{await route.abort('blockedbyclient');}catch{}}
  });
 }
 async function open(role,ui=false){
  let loginFailure=null;const env={...environment,DOC120_E2E_MODULE:'GESTOR',DOC120_E2E_USER:role==='OWNER'?secrets.ownerUser:'carolina.cruz',DOC120_E2E_PASSWORD:role==='OWNER'?secrets.ownerPassword:secrets.otherPassword};
  const guarded={newContext:async options=>{charge('sessions');const context=await browser.newContext(options);contexts.push(context);await install(context,role);return context;}};
  const auth=await createWorkflowLoginPageSession(guarded,{baseUrl:BASE,environment:env,moduleEnvironmentVariable:'DOC120_E2E_MODULE',userEnvironmentVariable:'DOC120_E2E_USER',passwordEnvironmentVariable:'DOC120_E2E_PASSWORD',ignoreHTTPSErrors:true,timeoutMilliseconds:60000,onDialogCompleted:async(_p,t)=>{loginFailure=classifyLoginMessage(t)||'LOGIN_SERVER_REJECTED';},onPostbackCompleted:async(_p,r)=>{try{loginFailure=classifyLoginPostback(await r.text())||loginFailure;}catch{}}});
  for(const k of ['DOC120_E2E_USER','DOC120_E2E_PASSWORD'])delete env[k];check(!loginFailure&&!state.guardFailure,'LOGIN_FAILED');auth.role=role;auth.page.setDefaultTimeout(30000);
  if(ui){const response=await auth.page.goto(new URL('Gestion/WebFormProducionDocumental.aspx',BASE).href,{waitUntil:'domcontentloaded',timeout:60000});check(response.status()===200,'PAGE_UNAVAILABLE');auth.token=await auth.page.locator('#doc111-importacion').getAttribute('data-token');}
  else{charge('http');const response=await auth.context.request.get(new URL('Gestion/WebFormProducionDocumental.aspx',BASE).href,{maxRedirects:0,timeout:60000});const html=await response.text(),tag=(html.match(/<div\b[^>]*\bid="doc111-importacion"[^>]*>/i)||[])[0];auth.token=tag&&(tag.match(/\bdata-token="([^"]+)"/i)||[])[1];}
  check(typeof auth.token==='string'&&auth.token.length>0,'TOKEN_UNAVAILABLE');return auth;
 }
 async function loaded(auth,lot,level){
  state.loadStep='NAVIGATE_'+lot;save();await auth.page.goto(new URL('Gestion/WebFormProducionDocumental.aspx',BASE).href,{waitUntil:'domcontentloaded',timeout:60000});state.loadStep='SELECT_LEVEL_'+level;save();await selectLevel(auth.page,level);state.loadStep='QUERY_LOT_'+lot;save();await consultQa(auth.page,lot);auth.token=await auth.page.locator('#doc111-importacion').getAttribute('data-token');state.loadStep='LOADED_'+lot;save();assertLive();
 }
 async function decision(page,fileId){await page.locator('[data-doc120-unavailable][open]').waitFor();const text=await page.locator('[data-doc120-unavailable][open]').textContent();check(text.includes('archivo '+fileId)&&text.includes('Envío del PDF')&&text.includes('Ruta dentro de la selección')&&text.includes('no está vinculado'),'DECISION_CONTEXT_MISSING');}
 async function paused(page){await page.waitForFunction(()=>{const b=document.querySelector('[data-doc111-action="continuar-importacion"]');return b&&!b.disabled&&!document.querySelector('[data-doc120-unavailable][open]');});assertLive();}
 async function shot(page,name){const target=page.locator('.doc111-content');if(await target.count())await target.screenshot({path:path.join(folder,name+'.png')});}
 async function qaUi(auth){
  const page=auth.page;await loaded(auth,'29','7');stage('UI_PAUSE_AT_MISSING_ORIGINAL');await page.locator('[data-doc111-action="continuar-importacion"]').click();await decision(page,binding.B);const knownA=await verifyFile('29',binding.A);await shot(page,'missing-original');
  const writes=state.operations.length;await wait(350);check(state.operations.length===writes,'DECISION_DID_NOT_WAIT');await page.locator('[data-doc120-unavailable][open] [data-doc111-action="pausar-revisar"]').last().click();await paused(page);state.results.pauseAtMissing={fileId:binding.B,waitsForConsent:true,paused:true};save();
  await loaded(auth,'29','7');stage('UI_CONTINUE_AFTER_RELOAD');await page.locator('[data-doc111-action="continuar-importacion"]').click();await decision(page,binding.B);
  const held=heldGate();pendingGates.push(held);holdingC=held;await page.locator('[data-doc120-unavailable][open] [data-doc111-action="continuar-demas"]').click();
  const proof=await gateOutcome(held.reached);stage('UI_PAUSE_WAITING_FOR_RESPONSE');await page.locator('[data-doc111-action="pausar-importacion"]').click();await page.getByRole('button',{name:'Aplicando pausa…',exact:true}).waitFor();await shot(page,'pause-waiting-response');held.release();await paused(page);
  const knownC=await verifyFile('29',binding.C);check(knownC.documentId===proof.documentId&&knownA.documentId!==knownC.documentId,'DOCUMENT_ID_CHANGED');state.results.pauseAfterAcknowledgement={...knownC,paused:true};save();
  stage('UI_RELOAD_KEEPS_RESULTS');await loaded(auth,'29','7');await page.locator('[data-doc111-action="continuar-importacion"]').click();await decision(page,binding.B);await page.locator('[data-doc120-unavailable][open] [data-doc111-action="pausar-revisar"]').last().click();await paused(page);
  const current=await snapshot(),b=one(current.files,r=>String(r.archivo_id)===binding.B);check(b.estado_recepcion==='PENDIENTE'&&b.estado_efecto==='NO_INICIADO'&&!Number(b.has_staging)&&!b.resultado_doc,'B_WAS_CHANGED');
  const root=one(current.roots,r=>String(r.raiz_id)===binding.createRoot),identity=JSON.parse(root.identidad_expediente_doc||'null');check(id(identity?.ExpedienteId)&&id(identity?.RelacionNivelId),'CREATION_UNCONFIRMED');
  const data=JSON.parse(root.datos_doc),code=one(data.ValoresPlan,v=>v.DestinoTipo==='CAMPO_EXPEDIENTE'&&v.DestinoRef==='CODIGO').Valor;
  const tree=await page.evaluate(({relation,level,expedient,code})=>{const key=relation+'|'+level+'|'+expedient;const nodes=Array.from(document.querySelectorAll('#TreeViewArchivo a[title]')).filter(a=>a.title.replace(/\|$/,'')===key);return{count:nodes.length,labelMatches:nodes.length===1&&nodes[0].textContent.trim()===code};},{relation:identity.RelacionNivelId,level:'7',expedient:identity.ExpedienteId,code});check(tree.count===1&&tree.labelMatches,'TREE_RESULT_MISSING');
  check(fingerprint(current.files.filter(f=>String(f.lote_id)==='29'),[binding.A,binding.C])===state.before.fingerprint29,'OTHER_QA_FILES_CHANGED');
  state.results.uiIndependence={A:knownA,B:{fileId:binding.B,pending:true,noUpload:true},C:knownC,reload:true,pausedAfterAcknowledgement:true,duplicateReplay:false,tree,other25FingerprintPreserved:true};save();
 }
 async function recoveredUi(auth){
  stage('VERIFY_LOT29_RECOVERY_READONLY');await loaded(auth,'29','7');
  const a=await verifyFile('29',binding.A),c=await verifyFile('29',binding.C);
  check(a.documentId==='16282'&&c.documentId==='16283','RESUME_DOCUMENT_CHANGED');
  await auth.page.locator('[data-doc111-action="continuar-importacion"]').click();await decision(auth.page,binding.B);await auth.page.locator('[data-doc120-unavailable][open] [data-doc111-action="pausar-revisar"]').last().click();await paused(auth.page);
  const root=one(binding.lots['29'].roots,r=>String(r.raiz_id)===binding.createRoot),identity=JSON.parse(root.identidad_expediente_doc),data=JSON.parse(root.datos_doc),code=one(data.ValoresPlan,v=>v.DestinoTipo==='CAMPO_EXPEDIENTE'&&v.DestinoRef==='CODIGO').Valor;
  const tree=await auth.page.evaluate(({identity,code})=>{const key=identity.RelacionNivelId+'|7|'+identity.ExpedienteId;const nodes=Array.from(document.querySelectorAll('#TreeViewArchivo a[title]')).filter(a=>a.title.replace(/\|$/,'')===key);return{count:nodes.length,labelMatches:nodes.length===1&&nodes[0].textContent.trim()===code};},{identity,code});check(tree.count===1&&tree.labelMatches,'TREE_RESULT_MISSING');
  await shot(auth.page,'lot29-recovered-readonly');state.results.uiRecovery={A:a,C:c,B:{fileId:binding.B,pending:true},reload:true,noRepeatedEffects:true,tree,other25FingerprintPreserved:true};save();
 }
 async function lostResponse(auth){
  await loaded(auth,'50','223');stage('LOST_STORAGE_RESPONSE');losingD=true;await auth.page.locator('[data-doc111-action="continuar-importacion"]').click();
  await auth.page.waitForFunction(()=>{const text=document.querySelector('.doc111-notice')?.textContent||'';return /guardado|confirm/i.test(text)&&!!document.querySelector('[data-doc111-action="consultar"]:not(:disabled)')&&!document.querySelector('[data-doc111-action="pausar-importacion"]');},null,{timeout:60000});assertLive();check(!losingD&&state.results.responseDeliveryBoundary,'RESPONSE_LOSS_NOT_INJECTED');
  const proof=await verifyFile('50',binding.D);await shot(auth.page,'response-delivery-lost');await loaded(auth,'50','223');const again=await verifyFile('50',binding.D);check(again.documentId===proof.documentId,'LOST_RESPONSE_DUPLICATED');state.results.lostResponse={...proof,reloadConfirmed:true,noReplay:true};save();
 }
 async function concurrency(first){
  stage('TWO_AUTHENTICATED_SESSIONS');const second=await open('OWNER');const view=(await api(first,'ConsultarRecursoLote',{loteId:'50',tipo:'ARCHIVOS',recursoId:binding.E,versionLectura:2})).body.datos;check(view.recurso.estado==='NO_INICIADO'&&!view.recurso.tieneIntentoActivo,'RACE_RESOURCE_CHANGED');
  const base={loteId:'50',recursoId:binding.E,fase:'ALMACENAR',revisionEsperada:view.recurso.revision},times=[];
  const call=async auth=>{const stamp={start:Date.now()};times.push(stamp);try{return await api(auth,'AvanzarLote',{...base,claveOperacion:crypto.randomBytes(16).toString('hex')},{allowFailure:true});}finally{stamp.end=Date.now();}};
  stage('CONCURRENT_SAME_FILE');const responses=await Promise.all([call(first),call(second)]);check(responses.filter(r=>r.status===200&&r.body.ok).length===1&&responses.filter(r=>[400,409].includes(r.status)&&!r.body.ok).length===1,'RACE_NOT_SINGLE_WINNER');check(times[0].start<times[1].end&&times[1].start<times[0].end,'REQUESTS_DID_NOT_OVERLAP');
  const proof=await verifyFile('50',binding.E),stale=await api(first,'AvanzarLote',{...base,claveOperacion:crypto.randomBytes(16).toString('hex')},{allowFailure:true});check([400,409].includes(stale.status)&&!stale.body.ok,'STALE_REVISION_ACCEPTED');check((await verifyFile('50',binding.E)).documentId===proof.documentId,'RACE_DUPLICATED');
  state.results.concurrentReservation={...proof,sessions:2,overlappingHttp:true,winners:1,rejected:1,staleRejected:true};save();await second.context.close();
 }
 async function convergence(auth){
  stage('CONFIRMED_PLAN_CONVERGENCE');const p={planId:'90',revisionEsperada:binding.plan.revision,planHashEsperado:binding.plan.hash,claveConfirmacion:crypto.randomBytes(16).toString('hex')};
  await api(auth,'ConfirmarPlan',p);const found=await api(auth,'ConsultarConfirmacionLote',{planId:'90'}),again=await api(auth,'ConfirmarPlan',{...p,claveConfirmacion:crypto.randomBytes(16).toString('hex')});check(found.body.datos.confirmada===true&&found.body.datos.loteId==='50'&&again.body.datos.loteId==='50','CONFIRMATION_DUPLICATED');state.results.confirmedPlanConvergence={planId:'90',lotId:'50',sameLot:true,newPlans:0,newLots:0};save();
 }
 try{
  const prepared=resume?null:readPrepared(ROOT);expectedSources=Object.fromEntries(SOURCES.map(f=>[f,hash(fs.readFileSync(path.join(ROOT,f)))]));check(expectedSources[SOURCES[0]]===BINARY,'BINARY_CHANGED');state.sources=expectedSources;save();
  if(resume){const used=previous.cumulative||{http:previous.http,select:previous.select,sessions:previous.sessions};state.accounting={used,maximumIncludingContinuation:{http:used.http+caps.http,select:used.select+caps.select,sessions:used.sessions+caps.sessions}};save();console.log('CONTINUACION REVISADA: lote29 solo lectura, documentos16282/16283 preservados. Lote50 archivos120/121: hasta4 ALMACENAR,2 documentos esperados (tope4 ante regresion). Cero recepciones, creaciones o confirmaciones.');console.log('Consumo anterior revisado: '+used.http+' HTTP/'+used.select+' SELECT/'+used.sessions+' sesiones/2 documentos. Nueva ventana:'+caps.http+' HTTP/'+caps.select+' SELECT/'+caps.sessions+' sesiones. Acumulados='+JSON.stringify(state.accounting.maximumIncludingContinuation)+'.45min. Claves privadas nuevas; sin borrados ni nuevos lotes.');}else{console.log('QA REAL: reutiliza lotes29 y50 del propietario141/empresa2. Sin nuevos planes/lotes ni borrados. Maximo1 PDF de677 bytes recibido,1 expediente nuevo y6 solicitudes de guardado (4 documentos previstos; hasta6 si aparece una regresion). Fallo controlado por entrega de respuesta y dos sesiones sobre un archivo.');
  console.log('Topes1200 HTTP,120 SELECT de control,3 sesiones,45 minutos. No se mueve ni borra ningun original. Las claves se ingresan enmascaradas y se eliminan al terminar.');}
  await tty.collectValue(secrets,'ra','DSN RA de lectura',{defaultValue:'MembershipUsers'});await tty.collectValue(secrets,'da','DSN DA de lectura',{defaultValue:'MembershipUsers'});check([secrets.ra,secrets.da].every(s=>/^[A-Za-z0-9 _.-]{1,128}$/.test(s)),'DSN_INVALID');
  await tty.collectValue(secrets,'tls','Aceptar certificado local: SI o NO',{defaultValue:'NO'});check(secrets.tls.toUpperCase()==='SI','TLS_NOT_ACCEPTED');
  await tty.collectValue(secrets,'dbUser','Cuenta MySQL exclusivamente SELECT',{secret:true});await tty.collectValue(secrets,'dbPassword','Clave MySQL SELECT',{secret:true});await tty.collectValue(secrets,'ownerUser','Cuenta propietaria de aplicacion (enmascarada)',{secret:true});await tty.collectValue(secrets,'ownerPassword','Clave de aplicacion',{secret:true});await tty.collectValue(secrets,'otherPassword','Clave de aplicacion de Carolina para privacidad (enmascarada)',{secret:true});
  environment=Object.fromEntries(Object.entries(process.env).filter(([k])=>!/^DOC\d+(?:_DA)?_E2E/.test(k)&&!['DEBUG','PWDEBUG','NODE_DEBUG','NODE_DEBUG_NATIVE'].includes(k)));for(const [prefix,dsn] of [['DOC120_E2E',secrets.ra],['DOC120_DA_E2E',secrets.da]])Object.assign(environment,{[prefix+'_ODBC_DSN']:dsn,[prefix+'_MYSQL_USER']:secrets.dbUser,[prefix+'_MYSQL_PASSWORD']:secrets.dbPassword});started=Date.now();
  stage('PREFLIGHT_READONLY');const principal=await query('SELECT id_Remit_Dest_Int AS usuario,Empresa_Gestion_Documental_id_empresa AS empresa FROM remit_dest_interno WHERE Login_Usuario=? AND Estado_Usuario=1 LIMIT 2',[secrets.ownerUser]);check(principal.length===1&&String(principal[0].usuario)==='141'&&String(principal[0].empresa)==='2','OWNER_CHANGED');
  const before=await snapshot();binding=resume?bindResumed(before,previous):bind(before);if(!resume){const plan=one(await query(SQL.plan,['50','141','2']),r=>String(r.plan_id)==='90');binding.plan=confirmationBinding(plan);}
  const cabinets=await query('SELECT id_gabinete,NOMBRE FROM system1 WHERE id_gabinete=? LIMIT 2',['10'],'DOC120_DA_E2E');check(cabinets.length===1&&/^[A-Za-z0-9_]{1,64}$/.test(cabinets[0].NOMBRE),'CABINET_CHANGED');cabinet=cabinets[0].NOMBRE;effects=new EffectsGate(binding);
  state.bindings=Object.fromEntries(['A','B','C','createRoot','D','E'].map(k=>[k,binding[k]]));state.before={completed29:binding.lots['29'].files.filter(f=>f.estado_efecto==='COMPLETADO').length,completed50:binding.lots['50'].files.filter(f=>f.estado_efecto==='COMPLETADO').length,fingerprint29:fingerprint(binding.lots['29'].files,[binding.A,binding.C]),fingerprint50:fingerprint(binding.lots['50'].files,[binding.D,binding.E])};save();
  browser=await require('../node_modules/playwright').chromium.launch({headless:true});stage('AUTHENTICATING_OWNER');const owner=await open('OWNER',true);if(resume){await recoveredUi(owner);}else{stage('PREFLIGHT_UI_READONLY');await loaded(owner,'29','7');await convergence(owner);
  stage('RECEIVING_ONLY_C');check(prepared.files.has(binding.CClient),'FIXTURE_CHANGED');const bytes=prepared.files.get(binding.CClient).bytes,p=receiptPayload(binding,bytes);effects.permit('RecibirArchivo',p);charge('http');unchanged();const response=await owner.context.request.post(new URL('generic_control/ImportacionCarpetasUpload.ashx',BASE).href,{maxRedirects:0,maxRetries:0,timeout:0,headers:{Origin:new URL(BASE).origin,'X-Importacion-Carpetas-CSRF':owner.token},multipart:{...p,file:{name:'DOC119-79.pdf',mimeType:'application/pdf',buffer:bytes}}});const raw=await response.json(),body=safeBody('RecibirArchivo',p,response,raw?.d??raw);check(response.status()===200&&body.ok&&body.datos?.estadoRecepcion==='RECIBIDO','RECEIPT_UNCONFIRMED');
  await qaUi(owner);}await lostResponse(owner);await concurrency(owner);
  stage('PRIVACY_OTHER_ACCOUNT');const other=await open('OTHER');
  for(const lot of ['29','50'])for(const operation of ['ConsultarLote','ExportarPaginaLote']){const p=operation==='ConsultarLote'?{loteId:lot}:{loteId:lot,tipo:'ARCHIVOS',pagina:1,tamanoPagina:1},denied=await api(other,operation,p,{allowFailure:true});check(denied.status===403&&!denied.body.ok&&denied.body.codigo==='ACCESO_DENEGADO'&&denied.body.datos==null,'OTHER_ACCOUNT_NOT_DENIED');}state.results.privacy={otherReadDenied:true,otherCsvDenied:true,lots:2};save();
  stage('FINAL_CONTROLS');const after=await snapshot();for(const [lot,excluded,expected] of [['29',[binding.A,binding.C],resume?0:2],['50',[binding.D,binding.E],2]]){const files=after.files.filter(f=>String(f.lote_id)===lot);check(files.filter(f=>f.estado_efecto==='COMPLETADO').length===state.before['completed'+lot]+expected&&fingerprint(files,excluded)===state.before['fingerprint'+lot],'FINAL_INVENTORY_CHANGED');}
  check(after.headers.every(h=>!Number(h.detencion_solicitada)&&Number(h.unresolved)===0),'FINAL_ACTIVE_OR_STOPPED');state.after={completed29:state.before.completed29+(resume?0:2),completed50:state.before.completed50+2,other25And998Preserved:true};state.effectCalls=effects.calls;stage('PASS_QA_WITH_LIMITS');
 }catch(e){state.failureStage=state.status;state.failureLoadStep=state.loadStep||null;state.failureType=e?.name==='TimeoutError'?'TIMEOUT':'ERROR';state.status='STOPPED';state.failure=state.guardFailure||failure(e);process.exitCode=2;}
 finally{for(const gate of pendingGates)gate.release();for(const context of contexts)await context.close().catch(()=>{});if(browser)await browser.close().catch(()=>{});for(const k of Object.keys(secrets))delete secrets[k];if(environment)for(const k of Object.keys(environment))if(/^DOC120(?:_DA)?_E2E/.test(k))delete environment[k];state.closed=true;state.credentialsCleared=true;state.finishedUtc=new Date().toISOString();if(effects)state.effectCalls=effects.calls;if(previous){state.previousStatePreserved=priorIntact();if(!state.previousStatePreserved){state.status='STOPPED';state.failure='DOC120_QA_PREVIOUS_STATE_CHANGED';process.exitCode=2;}}save();console.log('DOC120_QA_RESULT='+JSON.stringify(state));console.log('La QA termino. No se repetiran operaciones.');}
}
if(require.main===module)main().catch(()=>{console.error('DOC120_QA_PRECHECK_STOPPED');process.exitCode=2;});
module.exports={heldGate,gateOutcome,waitConsultReady,consultQa,reviewPrior,reviewAfterUi,main};
