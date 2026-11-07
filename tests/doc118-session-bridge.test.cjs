'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),crypto=require('node:crypto');
const {LIMITS,authorize,createBudget,openPair,prepareCommand,runPreparedWindow}=require('../tools/e2e/tests/support/doc118-session-bridge.cjs');
const {METHOD}=require('../tools/e2e/tests/support/doc118-active-call.cjs');
const example=require('../tools/e2e/profiles/doc111-importacion-carpetas.profile.example.json');
const digest=s=>crypto.createHash('sha256').update(s).digest('hex');
function fixture(scenario='COORD02'){
 const profile=structuredClone(example),config=JSON.stringify({VersionId:profile.versionId});
 const prepared={loteId:'100',archivoAId:'300',archivoBId:'301',revisionA:1,claveA:'synthetic-a',claveB:'synthetic-b',snapshotSha256:digest(config)};
 const command=prepareCommand(prepared),scope={runId:'a'.repeat(32),resourceSha256:digest(JSON.stringify(command))};
 const grant={scope:'DOC-118-'+scenario,runId:scope.runId,environment:true,account:true,effects:true,discardable:true,select:true,concurrency:true,observer:true,tls:false,baseUrl:profile.baseUrl,levelRef:profile.nivelRef,versionRef:profile.versionId,actorRef:'141',ambitoRef:'2',...Object.fromEntries(['loteId','archivoAId','archivoBId','snapshotSha256'].map(k=>[k,prepared[k]])),resourceSha256:scope.resourceSha256};
 const view={lot:{lote_id:'100',config_doc:config,contexto_doc:JSON.stringify({UsuarioRef:'141',AmbitoRef:'2',NivelRef:profile.nivelRef}),estado:'EN_CURSO',revision:10,detencion_solicitada:0,total_raices:2,total_archivos:2},roots:[200,201].map(raiz_id=>({raiz_id:String(raiz_id),estado:'EXPEDIENTE_CONFIRMADO'})),files:[300,301].map((archivo_id,i)=>({archivo_id:String(archivo_id),raiz_id:String(200+i),estado_recepcion:'RECIBIDO',estado_efecto:'NO_INICIADO',intento_activo_id:null,resultado_doc:null,valores_efectivos_doc:'[{"Valor":"9"}]',revision:i?7:1})),attempts:[],events:[]};
 const contexts=[],requests=[],queries=[];let resolveFirst,rejectFirst,firstStarted=false,releaseCount=0,secondCode='OPERACION_EN_CURSO';
 const response=(status,body)=>({status:()=>status,json:async()=>({d:body})});
 const browser={newContext:async()=>{
   const context={closed:0,routes:[],route:async(pattern,handler)=>context.routes.push(handler),close:async()=>{context.closed++;},request:{post:async(url,options)=>{
     const operation=url.split('/').pop(),payload=options.data.solicitud;requests.push({operation,payload,options,context});
     if(operation==='ConsultarLote')return response(200,{ok:true,datos:{loteId:'100',nivelRef:profile.nivelRef,revision:view.lot.revision,detencionSolicitada:!!view.lot.detencion_solicitada}});
     if(operation==='SolicitarDetencion'){view.lot.detencion_solicitada=1;view.lot.revision++;return response(200,{ok:true,datos:{loteId:'100',estado:'EN_CURSO',revision:view.lot.revision,detencionSolicitada:true}});}
     if(operation==='AvanzarLote'&&payload.recursoId==='300'&&!firstStarted){
       firstStarted=true;view.files[0].estado_efecto='EN_CURSO';view.files[0].revision++;view.files[0].intento_activo_id='500';view.files[1].revision=8;
       return new Promise((resolve,reject)=>{resolveFirst=()=>resolve(response(200,{ok:true,datos:{loteId:'100',archivoId:'300',fase:'ALMACENAR',estado:'CONFIRMADO',documentoId:'600',inventarioId:'700'}}));rejectFirst=()=>reject(Error('PRIVATE_COOKIE'));});
     }
     if(operation==='AvanzarLote')return response(409,{ok:false,codigo:payload.recursoId==='300'?'RESULTADO_INCIERTO':secondCode});
     throw Error('PRIVATE_UNEXPECTED_REQUEST');
   }}};contexts.push(context);return context;
 }};
 const openSession=async b=>{const context=await b.newContext();assert.equal(context.routes.length,1);return{context,page:{locator:()=>({getAttribute:async()=>'synthetic-csrf-'+contexts.indexOf(context)})}};};
 const query=async(origin,sql,args)=>{
   queries.push({origin,sql,args});let rows;
   if(sql.includes('FROM ra_integracion_lote'))rows=[view.lot];
   else if(sql.includes('FROM ra_integracion_raiz'))rows=view.roots;
   else if(sql.includes('FROM ra_integracion_archivo'))rows=view.files;
   else if(sql.includes('FROM ra_integracion_intento'))rows=view.attempts;
   else if(sql.includes('FROM ra_integracion_evento'))rows=view.events;
   else throw Error('PRIVATE_SQL');return structuredClone(rows);
 };
 const proof={...scope,processId:10,threadId:20,method:METHOD,insideBody:true,frozen:true,runtimeQualified:true};
 const observer={waitInside:async()=>{for(let i=0;i<10&&!firstStarted;i++)await new Promise(r=>setImmediate(r));assert.equal(firstStarted,true);return proof;},inspect:async()=>proof,resume:async()=>{releaseCount++;resolveFirst?.();}};
 const options={browser,profile,environment:{DOC111_E2E_USER:'synthetic-owner'},grant,scenario,openSession,budget:createBudget()};
 return{profile,prepared,scope,grant,view,browser,contexts,requests,queries,query,observer,options,setSecondCode:v=>{secondCode=v;},rejectFirst:()=>rejectFirst?.(),releases:()=>releaseCount,window:pair=>({pair,profile,grant,scope,prepared,query,observer})};
}
test('DOC118 bridge reuses two DOC111 clients, session CSRF, SELECT controls and the fresh other-file revision',async()=>{
 const f=fixture(),pair=await openPair(f.options);try{
   const result=await runPreparedWindow(f.window(pair));
   assert.deepEqual(result.codes,['RESULTADO_INCIERTO','OPERACION_EN_CURSO']);assert.equal(result.integrationPass,false);assert.equal(result.businessEvidenceVerified,false);
   const advances=f.requests.filter(r=>r.operation==='AvanzarLote');assert.equal(advances.length,3);assert.deepEqual(advances[1].payload,advances[0].payload);assert.equal(advances[2].payload.revisionEsperada,8);
   assert.notEqual(advances[0].context,advances[1].context);assert.notEqual(advances[0].options.headers['X-Importacion-Carpetas-CSRF'],advances[1].options.headers['X-Importacion-Carpetas-CSRF']);
   assert.ok(f.requests.every(r=>r.options.maxRetries===0&&r.options.maxRedirects===0));assert.equal(result.budget.selects,10);assert.equal(result.budget.scenarios.COORD02.operations.ALMACENAR,3);assert.equal(f.releases(),1);
   assert.ok(!JSON.stringify(result).includes('synthetic-a'));assert.ok(!JSON.stringify(result).includes('synthetic-csrf'));
 }finally{await pair.close();}assert.ok(f.contexts.every(c=>c.closed===1));await pair.close();assert.ok(f.contexts.every(c=>c.closed===1));
});
test('DOC118 bridge sends existing SolicitarDetencion contract from second session with current lot revision',async()=>{
 const f=fixture('COORD03'),pair=await openPair(f.options);try{const result=await runPreparedWindow(f.window(pair));assert.deepEqual(result.codes,['DETENCION_SOLICITADA']);assert.deepEqual(f.requests.find(r=>r.operation==='SolicitarDetencion').payload,{loteId:'100',revisionEsperada:10});assert.equal(f.requests.filter(r=>r.operation==='AvanzarLote').length,1);assert.equal(result.integrationPass,false);}finally{await pair.close();}
});
test('DOC118 refuses inherited or incomplete grants before opening any session',async()=>{
 for(const change of [f=>f.grant.scope='ALM-03',f=>f.grant.concurrency=false,f=>f.grant.observer=false,f=>f.grant.select=false,f=>f.grant.actorRef='invalid',f=>f.grant.loteId='21']){
   const f=fixture();change(f);await assert.rejects(openPair(f.options));assert.equal(f.contexts.length,0);
 }
});
test('DOC118 refuses TLS exception without specific grant before login',async()=>{const f=fixture();f.profile.ignoreHttpsErrors=true;await assert.rejects(openPair(f.options),/TLS_AUTHORIZATION_REQUIRED/);assert.equal(f.contexts.length,0);});
test('DOC118 closes the first session when opening the second fails and suppresses private errors',async()=>{
 const f=fixture();let opens=0;const original=f.options.openSession;f.options.openSession=async(...args)=>{if(++opens===2)throw Error('PRIVATE_PASSWORD');return original(...args);};
 await assert.rejects(openPair(f.options),e=>{assert.equal(e.code,'DOC118_BRIDGE_SESSION_OPEN_UNCONFIRMED');assert.ok(!JSON.stringify(e).includes('PRIVATE'));return true;});assert.equal(f.contexts[0].closed,1);
});
test('DOC118 rejects a shared session and closes its context only once',async()=>{const f=fixture();let session;f.options.openSession=async b=>session||(session={context:await b.newContext(),page:{}});await assert.rejects(openPair(f.options),/DISTINCT_SESSIONS_REQUIRED/);assert.equal(f.contexts[0].closed,1);});
test('DOC118 budgets forbid zero-limit stop and unknown phases before transport',async()=>{const f=fixture(),pair=await openPair(f.options);try{await assert.rejects(pair.clients[1].request('SolicitarDetencion',{loteId:'100',revisionEsperada:1}));await assert.rejects(pair.clients[1].request('AvanzarLote',{fase:'INCORPORAR'}));assert.equal(f.requests.length,0);}finally{await pair.close();}});
test('DOC118 fixed operation budget counts even a lost or rejected transport attempt',()=>{const b=createBudget();for(let i=0;i<5;i++)b.http('COORD02','AvanzarLote',{fase:'ALMACENAR'});assert.throws(()=>b.http('COORD02','AvanzarLote',{fase:'ALMACENAR'}),/HTTP_OPERATION_LIMIT/);assert.equal(b.snapshot().scenarios.COORD02.http,5);});
test('DOC118 shared SELECT budget caps both cases and rejects writes, wait functions and locks',()=>{
 const b=createBudget();for(const sql of ['UPDATE t SET n=1','SELECT 1; SELECT 2','SELECT SLEEP(1)','SELECT n FROM t FOR UPDATE'])assert.throws(()=>b.select('RA',sql),/SELECT_REQUIRED/);
 for(let i=0;i<240;i++)b.select(i%2?'RA':'DA','SELECT n FROM t WHERE id=?');assert.throws(()=>b.select('DA','SELECT 1'),/SELECT_LIMIT/);assert.equal(b.snapshot().selects,240);
});
test('DOC118 browser budget is installed before shared login and aborts requests beyond total cap',async()=>{
 const f=fixture(),pair=await openPair(f.options);try{
   let proceeded=0,aborted=0;const route={request:()=>({url:()=>f.profile.baseUrl+'asset.css'}),continue:async()=>{proceeded++;},abort:async()=>{aborted++;}};
   for(let i=0;i<251;i++)await f.contexts[0].routes[0](route);assert.equal(proceeded,250);assert.equal(aborted,1);assert.equal(pair.budget.snapshot().scenarios.COORD02.http,250);
 }finally{await pair.close();}
});
test('DOC118 keeps browser requests inside selected environment and path',async()=>{const f=fixture(),pair=await openPair(f.options);try{let aborted=false;await f.contexts[0].routes[0]({request:()=>({url:()=>'https://other.invalid/private'}),continue:async()=>assert.fail('out-of-scope dispatch'),abort:async()=>{aborted=true;}});assert.equal(aborted,true);}finally{await pair.close();}});
test('DOC118 rejects changed owner or destination before issuing an advance',async()=>{for(const context of [{UsuarioRef:'999',AmbitoRef:'2',NivelRef:'7'},{UsuarioRef:'141',AmbitoRef:'2',NivelRef:'99'}]){const f=fixture(),pair=await openPair(f.options);try{f.view.lot.contexto_doc=JSON.stringify(context);await assert.rejects(runPreparedWindow(f.window(pair)),e=>e.code==='DOC118_BRIDGE_PERSISTED_CONTEXT_CHANGED'&&e.firstRequested===false);assert.equal(f.requests.length,0);}finally{await pair.close();}}});
test('DOC118 rejects changed snapshot or stale first resource without calling writer',async()=>{for(const change of [f=>f.view.lot.config_doc='{}',f=>f.view.files[0].revision++]){const f=fixture(),pair=await openPair(f.options);try{change(f);await assert.rejects(runPreparedWindow(f.window(pair)));assert.equal(f.requests.length,0);}finally{await pair.close();}}});
test('DOC118 rejects an unpinned operation or altered session grant before reading data',async()=>{for(const change of [f=>f.prepared.claveA='changed',f=>f.grant.archivoBId='999']){const f=fixture(),pair=await openPair(f.options);try{change(f);await assert.rejects(runPreparedWindow(f.window(pair)),/REQUEST_NOT_PINNED|SESSION_AUTHORIZATION_CHANGED/);assert.equal(f.queries.length,0);assert.equal(f.requests.length,0);}finally{await pair.close();}}});
test('DOC118 does not accept revision conflict as evidence of the active documentary guard',async()=>{const f=fixture(),pair=await openPair(f.options);try{f.setSecondCode('CONFLICTO_REVISION');await assert.rejects(runPreparedWindow(f.window(pair)),e=>e.code==='DOC118_BRIDGE_WINDOW_NOT_CONFIRMED'&&e.firstRequested===true);assert.equal(f.requests.filter(r=>r.operation==='AvanzarLote').length,3);assert.equal(f.releases(),1);}finally{await pair.close();}});
test('DOC118 does not turn a lost first response into a repeat or integration PASS',async()=>{const f=fixture(),pair=await openPair(f.options);try{f.observer.resume=async()=>f.rejectFirst();const r=await runPreparedWindow(f.window(pair));assert.equal(r.coordination.firstOutcome,'REJECTED');assert.equal(r.integrationPass,false);assert.equal(f.requests.filter(r=>r.operation==='AvanzarLote').length,3);}finally{await pair.close();}});
test('DOC118 stops on invalid body observation, releases once and retains only safe uncertainty',async()=>{const f=fixture(),pair=await openPair(f.options);try{f.observer.inspect=async()=>{throw Error('PRIVATE_CONNECTION');};await assert.rejects(runPreparedWindow(f.window(pair)),e=>{assert.equal(e.code,'DOC118_BRIDGE_WINDOW_NOT_CONFIRMED');assert.equal(e.firstRequested,true);assert.ok(!JSON.stringify(e).includes('PRIVATE'));return true;});assert.equal(f.requests.filter(r=>r.operation==='AvanzarLote').length,1);assert.equal(f.releases(),1);}finally{await pair.close();}});
test('DOC118 refuses to use a closed pair',async()=>{const f=fixture(),pair=await openPair(f.options);await pair.close();await assert.rejects(runPreparedWindow(f.window(pair)),/SESSION_PAIR_CLOSED/);assert.equal(f.queries.length,0);});
test('DOC118 fixed limits match reviewed business HTTP budgets',()=>{assert.equal(Object.values(LIMITS.COORD02).reduce((a,b)=>a+b,0),53);assert.equal(Object.values(LIMITS.COORD03).reduce((a,b)=>a+b,0),51);});
test('DOC118 requires an explicit shared budget before opening contexts',async()=>{const f=fixture();delete f.options.budget;await assert.rejects(openPair(f.options),/SHARED_BUDGET_REQUIRED/);assert.equal(f.contexts.length,0);});
test('DOC118 consumes the window once even when preflight fails without sending first',async()=>{const f=fixture(),pair=await openPair(f.options);try{f.view.files[0].revision++;await assert.rejects(runPreparedWindow(f.window(pair)));f.view.files[0].revision--;await assert.rejects(runPreparedWindow(f.window(pair)),/WINDOW_ALREADY_REQUESTED/);assert.equal(f.requests.length,0);}finally{await pair.close();}});
test('DOC118 observes claims from other documentary phases before starting storage',async()=>{const f=fixture(),pair=await openPair(f.options);try{f.view.attempts.push({fase:'CREAR_EXPEDIENTE',estado:'INCIERTO'});await assert.rejects(runPreparedWindow(f.window(pair)),/INITIAL_CLAIM_CHANGED/);assert.equal(f.requests.length,0);}finally{await pair.close();}});
test('DOC111 read-only driver refuses the newly exposed stop before HTTP',async()=>{const shared=require('../tools/e2e/tests/support/doc111-importacion-carpetas.cjs'),f=fixture();let calls=0;const reader=shared.createDriver({context:{request:{post:()=>{calls++;}}}},f.profile,{environment:true,account:true,select:true},{readOnly:true});await assert.rejects(reader.request('SolicitarDetencion',{loteId:'100',revisionEsperada:1}),/READONLY_OPERATION_REQUIRED/);assert.equal(calls,0);});
