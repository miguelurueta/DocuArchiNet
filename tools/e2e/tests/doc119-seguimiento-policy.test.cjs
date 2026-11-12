'use strict';
const test=require('node:test'),assert=require('node:assert/strict');
const {BASE_URL,LIMITS,READS,STATIC_DEPENDENCIES,SESSION_IDLE_LIMIT_MS,assertSessionFresh,previousSelection,validateConnection,validateProfile,chooseItem,describeImport,discoverImports,discoverLevels,pendingFile,createBudget,permittedRequest,guardRequestSummary,installGuard,makeControls,responseFor,csvRows,hash,safeTreeDiagnostic}=require('./support/doc119-seguimiento.cjs');
const {parseArgs,main}=require('../scripts/run-doc119-seguimiento.cjs');
const profile=()=>({baseUrl:BASE_URL,module:'GESTOR',ignoreHttpsErrors:false,timeoutMilliseconds:60000,odbcDsn:'MembershipUsers',docuarchiOdbcDsn:'MembershipUsers',lotId:'5',recoveryLotId:null,alternateLevel:null});
const scope={lotId:'5',level:'7',planId:'17'};
const url=operation=>new URL('webservice/WebServiceImportacionCarpetas.asmx/'+operation,BASE_URL).href;
test('DOC119 diagnostico de arbol conserva solo conteos y categorias, sin DOM ni secretos',()=>{
  const input={before:{availability:'AVAILABLE',counts:[1],html:'private'},after:{availability:'AVAILABLE',counts:[2],headers:'private'},selectionPreserved:true,expansionPreserved:true,cookie:'private'};
  assert.deepEqual(safeTreeDiagnostic(input),{before:{availability:'AVAILABLE',counts:[1]},after:{availability:'AVAILABLE',counts:[2]},selectionPreserved:true,expansionPreserved:true});
  assert.equal(JSON.stringify(safeTreeDiagnostic(input)).includes('private'),false);
  for(const change of [v=>v.before.availability='private',v=>v.after.counts=[NaN],v=>v.after.counts=[1,2],v=>v.before.counts=[-1],v=>v.selectionPreserved='private']){const value=structuredClone(input);change(value);assert.equal(safeTreeDiagnostic(value),null);}
});

test('DOC119 selector exige autorizacion del plan y runId sin texto ejecutable',()=>{
  assert.deepEqual(parseArgs(['--check']),{check:true});
  assert.deepEqual(parseArgs(['--run','a'.repeat(32),'--authorized-plan']),{runId:'a'.repeat(32)});
  assert.deepEqual(parseArgs(['--run','a'.repeat(32),'--authorized-plan','--reuse-selection','b'.repeat(32)]),{runId:'a'.repeat(32),selectionRunId:'b'.repeat(32)});
  for(const prior of ['../state','a'.repeat(32)])assert.throws(()=>parseArgs(['--run','a'.repeat(32),'--authorized-plan','--reuse-selection',prior]),/DOC119_ARGUMENT_INVALID/);
  for(const args of [[],['--run','a'.repeat(32)],['--run','a;whoami','--authorized-plan'],['--run','a'.repeat(32),'--effects'],['--check','--run']])assert.throws(()=>parseArgs(args),/DOC119_ARGUMENT_INVALID/);
});
test('DOC119 perfil no acepta secretos, autoridad o destino distintos del plan',()=>{
  assert.equal(validateProfile(profile()).lotId,'5');
  for(const patch of [{password:'synthetic'},{connectionString:'synthetic'},{usuarioRef:'141'},{baseUrl:'https://elsewhere.invalid/'},{baseUrl:BASE_URL+'?password=synthetic'},{lotId:'05'},{lotId:'4294967296'},{odbcDsn:'DSN;other'},{ignoreHttpsErrors:'true'},{recoveryLotId:'18'}])assert.throws(()=>validateProfile({...profile(),...patch}),/^Error: DOC119_/);
  assert.equal(validateProfile({...profile(),recoveryLotId:'18',alternateLevel:'8'}).alternateLevel,'8');
});
test('DOC119 permite solamente lecturas del lote/plan/nivel concretos',()=>{
  assert.equal(permittedRequest('POST',url('ConsultarLote'),{loteId:'5'},scope),true);
  assert.equal(permittedRequest('POST',url('ConsultarRecursosLote'),{loteId:'5',tipo:'ARCHIVOS',pagina:2,tamanoPagina:25,raizId:'40',estado:'INCIERTO',estadoRecepcion:'RECIBIDO'},scope),true);
  assert.equal(permittedRequest('POST',url('ExportarPaginaLote'),{loteId:'5',tipo:'ARCHIVOS',pagina:2,tamanoPagina:25},scope),true);
  assert.equal(permittedRequest('POST',url('ConsultarConfirmacionLote'),{planId:'17'},scope),true);
  assert.equal(permittedRequest('POST',url('AbrirImportacion'),{nivelRef:'7'},scope),true);
  for(const [operation,payload]of [['ConsultarLote',{loteId:'6'}],['ConsultarConfirmacionLote',{planId:'18'}],['AbrirImportacion',{nivelRef:'8'}],['ConsultarLote',{loteId:'5',usuarioRef:'141'}]])assert.equal(permittedRequest('POST',url(operation),payload,scope),false);
});
test('DOC119 bloquea fases, confirmacion, upload, permisos y metodos no autorizados',()=>{
  for(const operation of ['ConfirmarPlan','PrepararPlan','AvanzarLote','ReconciliarRecurso','SolicitarDetencion','CerrarReservaSinInicio','RecuperarDocumentoV2','LimpiarOriginal','GuardarPerfil'])assert.equal(permittedRequest('POST',url(operation),{loteId:'5'},scope),false);
  assert.equal(READS.includes('ConfirmarPlan'),false);
  for(const method of ['GET','PUT','DELETE'])assert.equal(permittedRequest(method,url('ConsultarLote'),{loteId:'5'},scope),false);
  assert.equal(permittedRequest('POST',new URL('generic_control/ImportacionCarpetasUpload.ashx',BASE_URL).href,{loteId:'5'},scope),false);
  assert.equal(permittedRequest('GET',new URL('generic_control/ImportacionCarpetasUpload.ashx',BASE_URL).href,null,scope),false);
});
test('DOC119 limita origen, rutas de pagina y autoridad embebida',()=>{
  assert.equal(permittedRequest('GET',new URL('js/gestion/ImportacionCarpetas/ImportacionCarpetas.js',BASE_URL).href,null,scope),true);
  assert.equal(permittedRequest('POST',new URL('gestor.aspx',BASE_URL).href,null,scope),true);
  for(const target of ['https://remote.invalid/a.js','https://localhost/other/gestor.aspx','https://synthetic:synthetic@localhost/GestionDocumental-Docuarchi.net/gestor.aspx',new URL('Gestion/Delete.aspx',BASE_URL).href])assert.equal(permittedRequest('GET',target,null,scope),false);
  assert.equal(permittedRequest('POST',new URL('WebServiceOther.asmx/Read',BASE_URL).href,{},scope),false);
  assert.equal(permittedRequest('GET',new URL('WebServiceOther.asmx/Read',BASE_URL).href,null,scope),false);
});
test('DOC119 postback solo permite seleccionar niveles concretos y no botones de negocio',()=>{
  const target=new URL('Gestion/WebFormProducionDocumental.aspx',BASE_URL).href,tree={...scope,allowTree:true,alternateLevel:'8'};
  const form={__EVENTTARGET:'',__EVENTARGUMENT:'',Button_inline_trevie:'Editar',HiddenField_0003:'7\\\\Origen'};
  assert.equal(permittedRequest('POST',target,form,tree),true);
  assert.equal(permittedRequest('POST',target,{...form,HiddenField_0003:'8\\\\Otro'},tree),true);
  assert.equal(permittedRequest('POST',target,form,scope),false);
  for(const patch of [{__EVENTTARGET:'Button_eliminar_carpeta'},{Button_eliminar_carpeta:'Eliminar'},{__EVENTARGUMENT:'delete'},{HiddenField_0003:'9\\\\Ajeno'},{HiddenField_0003:'70|7|60|'},{HiddenField_0003:'7'}])assert.equal(permittedRequest('POST',target,{...form,...patch},tree),false);
});
test('DOC119 presupuestos incluyen sesiones y no autorizan superar el limite',()=>{
  const snapshots=[],budget=createBudget('SEG-01',v=>snapshots.push(v));
  for(const key of ['http','select','sessions']){for(let i=0;i<LIMITS['SEG-01'][key];i++)budget.take(key);assert.throws(()=>budget.take(key),/DOC119_BUDGET_EXHAUSTED/);}
  assert.deepEqual(budget.counts,{http:600,select:80,sessions:2});assert.notEqual(snapshots[0],budget.counts);
  assert.throws(()=>budget.take('write'),/DOC119_BUDGET_KIND_INVALID/);assert.throws(()=>createBudget('other'),/DOC119_STAGE_INVALID/);
  assert.deepEqual(LIMITS['SEG-02'],{http:600,select:60,sessions:2});
});
test('DOC119 guardia aborta y queda cerrada despues de una violacion',async()=>{
  let handler;const context={route:async(_pattern,value)=>{handler=value;}},budget=createBudget('SEG-01');
  const guard=await installGuard(context,budget,scope),outcomes=[];
  const route=(operation,payload)=>({request:()=>({method:()=> 'POST',url:()=>url(operation),postData:()=>JSON.stringify({solicitud:payload})}),continue:async()=>outcomes.push('continue'),abort:async()=>outcomes.push('abort')});
  await handler(route('ConsultarLote',{loteId:'5'}));await handler(route('AvanzarLote',{loteId:'5'}));await handler(route('ConsultarLote',{loteId:'5'}));
  assert.deepEqual(outcomes,['continue','abort','abort']);assert.equal(budget.counts.http,1);assert.equal(guard.calls.length,1);assert.equal(guard.violations.length,2);
});
test('DOC119 guardia no deja pasar JSON roto o otro lote',async()=>{
  for(const body of ['not JSON',JSON.stringify({solicitud:{loteId:'6'}}),JSON.stringify({solicitud:[]})]){
    let handler,continued=false,aborted=false;await installGuard({route:async(_pattern,value)=>{handler=value;}},createBudget('SEG-02'),scope);
    await handler({request:()=>({method:()=> 'POST',url:()=>url('ConsultarLote'),postData:()=>body}),continue:async()=>{continued=true;},abort:async()=>{aborted=true;}});
    assert.equal(continued,false);assert.equal(aborted,true);
  }
});

test('DOC119 omite la fuente opcional del login sin red externa ni cerrar la guardia',async()=>{
  let handler;const budget=createBudget('SEG-01'),outcomes=[];
  const guard=await installGuard({route:async(_pattern,value)=>{handler=value;}},budget,scope);
  const request=(method,url,type)=>({request:()=>({method:()=>method,url:()=>url,resourceType:()=>type}),abort:async()=>outcomes.push('abort'),continue:async()=>outcomes.push('continue')});
  const font='https://fonts.googleapis.com/css?family=Open+Sans:300,400,400i,600,700,800';
  await handler(request('GET',font,'stylesheet'));
  assert.deepEqual(guard.omittedPresentation,['GOOGLE_LOGIN_FONT']);assert.deepEqual(guard.violations,[]);assert.equal(budget.counts.http,0);
  await handler(request('GET',new URL('gestor.aspx',BASE_URL).href,'document'));
  assert.deepEqual(outcomes,['abort','continue']);assert.equal(budget.counts.http,1);
  await handler(request('POST',font,'stylesheet'));
  assert.deepEqual(guard.violations,['DOC119_HTTP_SCOPE_DENIED']);assert.equal(budget.counts.http,1);
});

test('DOC119 permite estado 2FA vacio y destino normal de login sin autorizar mutaciones',async()=>{
  const status=new URL('webservice/WebServiceLoginSegundoFactor.asmx/ObtenerEstado',BASE_URL).href;
  const landing=new URL('Defaul/WebFormInicioDocuarchiGestion.aspx',BASE_URL).href;
  const unbound={lotId:null,level:null,planId:null};
  assert.equal(permittedRequest('POST',status,{},unbound),true);
  for(const payload of [null,[],{codigo:'synthetic'},{solicitud:{}},{usuarioRef:'141'}])assert.equal(!!permittedRequest('POST',status,payload,unbound),false);
  for(const operation of ['Verificar','Reenviar','Cancelar'])assert.equal(permittedRequest('POST',status.replace('ObtenerEstado',operation),{},unbound),false);
  assert.equal(permittedRequest('POST',status+'?codigo=synthetic',{},unbound),false);
  assert.equal(permittedRequest('GET',landing,null,unbound),true);assert.equal(permittedRequest('POST',landing,{},unbound),false);
  let handler,continued=0;const budget=createBudget('SEG-01');
  const guard=await installGuard({route:async(_p,value)=>{handler=value;}},budget,unbound);
  await handler({request:()=>({method:()=> 'POST',url:()=>status,postData:()=> '{}'}),continue:async()=>continued++,abort:async()=>assert.fail('status read must be permitted')});
  assert.equal(continued,1);assert.equal(budget.counts.http,1);assert.deepEqual(guard.violations,[]);
  assert.deepEqual(guard.calls,[{operation:'ObtenerEstado',request:{}}]);
});

test('DOC119 diagnostico de guardia conserva solo categorias sin rutas, parametros ni formularios',async()=>{
  const privateUrl='https://private-account.invalid/secret-path?password=synthetic';
  const request={method:()=> 'POST',url:()=>privateUrl,postData:()=> 'password=synthetic'};
  assert.deepEqual(guardRequestSummary(request),{resource:'OUTSIDE_ORIGIN',method:'POST',operation:null});
  let handler;const guard=await installGuard({route:async(_p,value)=>{handler=value;}},createBudget('SEG-01'),scope);
  await handler({request:()=>request,continue:async()=>assert.fail('must block'),abort:async()=>{}});
  assert.deepEqual(guard.firstViolation,{resource:'OUTSIDE_ORIGIN',method:'POST',operation:null});
  assert.doesNotMatch(JSON.stringify(guard.firstViolation),/private-account|secret-path|password|synthetic/);
});

test('DOC119 omite inicio y conserva el login compartido antes de abrir Produccion',async()=>{
  const {chromium}=require('../node_modules/playwright');
  const {openSession}=require('./support/doc111-importacion-carpetas.cjs');
  const {createWorkflowLoginPageSession}=require('./support/authenticated-workflow-session.cjs');
  const browser=await chromium.launch({headless:true});let guard,homeSent=0;
  try{
    const budget=createBudget('SEG-01');
    const facade={newContext:async()=>{
      const context=await browser.newContext({offline:true});
      const adapter={route:async(pattern,handler)=>context.route(pattern,route=>handler({request:()=>route.request(),abort:code=>route.abort(code),fulfill:options=>route.fulfill(options),continue:async()=>{
        const request=route.request(),relative=new URL(request.url()).pathname.toLowerCase();
        if(relative.endsWith('/defaul/webforminiciodocuarchigestion.aspx')){homeSent++;return route.fulfill({contentType:'text/html',body:'<script>fetch("/forbidden-service",{method:"POST"})</script>'});}
        if(relative.endsWith('/gestor.aspx')&&request.method()==='POST')return route.fulfill({status:200,headers:{'content-type':'text/html','set-cookie':'synthetic_session=valid; Path=/; Secure; HttpOnly'},body:'<script>location.replace('+JSON.stringify(new URL('Defaul/WebFormInicioDocuarchiGestion.aspx',BASE_URL).href)+')</script>'});
        if(relative.endsWith('/gestor.aspx'))return route.fulfill({contentType:'text/html',body:'<form method="post"><select id="ContentPlacenter_DropDownListmodulos"><option value="GESTOR">Gestor</option></select><input id="ContentPlacenter_TextBoxuser" name="TextBoxuser"><input id="ContentPlacenter_TextBoxpasw" name="TextBoxpasw" type="password"><a class="da-login-submit" href="#" onclick="document.forms[0].requestSubmit();return false">Entrar</a></form>'});
        if(relative.endsWith('/gestion/webformproduciondocumental.aspx'))return route.fulfill({contentType:'text/html',body:'<div id="doc111-importacion" data-token="synthetic-only"></div>'});
        return route.abort();
      }}))};
      guard=await installGuard(adapter,budget,{lotId:null,level:null,planId:null});return context;
    }};
    // Let the native redirect finish before the helper opens Production. This
    // reproduces the ordering that previously executed the home dashboard.
    const runtime={createSession:(browser,options)=>createWorkflowLoginPageSession(browser,{...options,onPostbackCompleted:async(page,response)=>{await options.onPostbackCompleted(page,response);await page.waitForURL(new URL('Defaul/WebFormInicioDocuarchiGestion.aspx',BASE_URL).href,{waitUntil:'domcontentloaded',timeout:5000});}})};
    const auth=await openSession(facade,{baseUrl:BASE_URL,ignoreHttpsErrors:false,timeoutMilliseconds:30000},{DOC111_E2E_MODULE:'GESTOR',DOC111_E2E_USER:'synthetic-owner',DOC111_E2E_PASSWORD:'synthetic-password'},{environment:true,account:true},runtime);
    assert.equal(homeSent,0);assert.ok(guard.omittedPresentation.includes('HOME_LANDING'));assert.deepEqual(guard.violations,[]);
    assert.match(auth.page.url(),/Gestion\/WebFormProducionDocumental\.aspx$/);
    assert.ok((await auth.context.cookies()).some(cookie=>cookie.name==='synthetic_session'&&cookie.value==='valid'));
    assert.equal(auth.loginObservation.postedFields,'MATCHED');await auth.context.close();
  }finally{await browser.close();}
});

test('DOC119 carga solo dependencias CDN fijas sin enviar contexto privado y cuenta cada GET',async()=>{
  const fs=require('node:fs'),path=require('node:path');
  const markup=fs.readFileSync(path.resolve(__dirname,'../../../Gestion/WebFormProducionDocumental.aspx'),'utf8');
  let handler;const budget=createBudget('SEG-01'),sent=[];
  const guard=await installGuard({route:async(_p,value)=>{handler=value;}},budget,scope);
  for(const asset of STATIC_DEPENDENCIES){
    assert.ok(markup.includes(asset.url));
    const request={method:()=> 'GET',url:()=>asset.url,resourceType:()=>asset.type,postData:()=>null,allHeaders:async()=>({Cookie:'synthetic',Authorization:'synthetic','proxy-authorization':'synthetic',Referer:'synthetic',Origin:'synthetic',accept:'*/*'})};
    await handler({request:()=>request,continue:async options=>sent.push(options),abort:async()=>assert.fail('known public asset must load')});
  }
  assert.equal(budget.counts.http,6);assert.equal(sent.length,6);assert.deepEqual(guard.externalAssets,STATIC_DEPENDENCIES.map(a=>a.code));assert.deepEqual(guard.violations,[]);
  for(const options of sent)assert.deepEqual(options.headers,{accept:'*/*'});
});

test('DOC119 excepciones de arranque rechazan landing con parametros y CDN alterado',async()=>{
  const landing=new URL('Defaul/WebFormInicioDocuarchiGestion.aspx',BASE_URL).href;
  assert.equal(permittedRequest('GET',landing+'?action=delete',null,scope),false);
  const asset=STATIC_DEPENDENCIES[0];
  for(const patch of [{method:'POST'},{type:'document'},{url:asset.url+'?private=synthetic'},{body:'synthetic'}]){
    let handler;const budget=createBudget('SEG-01'),guard=await installGuard({route:async(_p,value)=>{handler=value;}},budget,scope);
    await handler({request:()=>({method:()=>patch.method||'GET',url:()=>patch.url||asset.url,resourceType:()=>patch.type||asset.type,postData:()=>patch.body||null}),continue:async()=>assert.fail('must block'),abort:async()=>{}});
    assert.equal(budget.counts.http,0);assert.equal(guard.violations[0],'DOC119_HTTP_SCOPE_DENIED');
  }
});
test('DOC119 CSV conserva comillas, saltos y pagina vacia sin guardarlo',()=>{
  assert.deepEqual(csvRows('"recursoId","nombre"\r\n"31","a,""b""\r\nc"\r\n'),[['recursoId','nombre'],['31','a,"b"\r\nc']]);
  assert.deepEqual(csvRows('"recursoId"\r\n'),[['recursoId']]);assert.throws(()=>csvRows('"unfinished'),/DOC119_CSV_INVALID/);
  assert.equal(hash(Buffer.from([0,255,10])),require('node:crypto').createHash('sha256').update(Buffer.from([0,255,10])).digest('hex'));
});
test('DOC119 controles reutilizan SELECT existente y detectan cambio de auditoria',async()=>{
  let revision=1;const queries=[];
  const query=async(origin,sql,args)=>{
    queries.push({origin,sql,args});assert.match(sql,/^SELECT /);assert.doesNotMatch(sql,/\b(?:UPDATE|INSERT|DELETE|CALL|SET|DROP)\b/i);
    if(sql.includes('config_doc,contexto_doc'))return [{lote_id:'5',config_doc:'{}',contexto_doc:JSON.stringify({UsuarioRef:'141',AmbitoRef:'2',NivelRef:'7'}),revision}];
    if(sql.startsWith('SELECT plan_id'))return [{plan_id:'17'}];
    if(sql.includes('FROM ra_integracion_raiz WHERE'))return [{identidad_expediente_doc:JSON.stringify({ExpedienteId:'650',RelacionNivelId:'1080'})}];
    return [];
  };
  const controls=makeControls(query,createBudget('SEG-01')),first=await controls.snapshot('5'),same=await controls.snapshot('5');
  assert.equal(first.fingerprint,same.fingerprint);assert.equal(queries.length,16);assert.equal(queries.filter(q=>q.origin==='DA').length,2);
  assert.deepEqual(queries.find(q=>q.origin==='DA').args,['141','650']);revision++;
  assert.notEqual((await controls.snapshot('5')).fingerprint,first.fingerprint);
});
test('DOC119 controles correlacionan metadatos por ID y los incluyen en la huella',async()=>{
  let changed=false,duplicate=false;
  const query=async(origin,sql,args)=>{
    assert.equal(origin,'RA');assert.deepEqual(args,['5']);
    if(sql.includes('config_doc,contexto_doc'))return [{lote_id:'5',contexto_doc:JSON.stringify({UsuarioRef:'141',AmbitoRef:'2',NivelRef:'7'})}];
    if(sql.startsWith('SELECT plan_id'))return [{plan_id:'17'}];
    if(sql.startsWith('SELECT f.archivo_id,f.origen_doc'))return [{archivo_id:'32',origen_doc:JSON.stringify({Nombre:changed?'changed.pdf':'same.pdf',RutaRelativa:'b/same.pdf'}),bytes_recibidos:'0'},{archivo_id:duplicate?'32':'31',origen_doc:JSON.stringify({Nombre:'same.pdf',RutaRelativa:'a/same.pdf'}),bytes_recibidos:'677'}];
    if(sql.startsWith('SELECT f.archivo_id'))return [{archivo_id:'31'},{archivo_id:'32'}];
    return [];
  };
  const controls=makeControls(query,createBudget('SEG-01')),before=await controls.snapshot('5');
  assert.equal(JSON.parse(before.data.files[0].origen_doc).RutaRelativa,'a/same.pdf');assert.equal(before.data.files[0].bytes_recibidos,'677');
  changed=true;assert.notEqual((await controls.snapshot('5')).fingerprint,before.fingerprint);
  duplicate=true;await assert.rejects(()=>controls.snapshot('5'),/FILE_METADATA_INVALID/);
});
test('DOC119 recursos/contextos no validos fallan antes de continuar',async()=>{
  const controls=makeControls(async(_origin,sql)=>sql.includes('config_doc,contexto_doc')?[{lote_id:'5',contexto_doc:'{}'}]:[],createBudget('SEG-02'));
  await assert.rejects(()=>controls.snapshot('5'),/DOC119_CONTEXT_INVALID/);await assert.rejects(()=>controls.snapshot('0'),/DOC119_LOT_INVALID/);
});
test('DOC119 --check no pide credenciales ni abre una sesion',async()=>{await main(['--check']);});

test('DOC119 detecta seleccion demorada antes de reutilizar una sesion sin renovarla',()=>{
  assert.equal(SESSION_IDLE_LIMIT_MS,8*60*1000);
  assert.doesNotThrow(()=>assertSessionFresh(1000,1000+SESSION_IDLE_LIMIT_MS-1));
  for(const now of [1000+SESSION_IDLE_LIMIT_MS,1000+64*60*1000,999,NaN])assert.throws(()=>assertSessionFresh(1000,now),/DOC119_SESSION_IDLE_EXPIRED/);
});

test('DOC119 reutiliza solo la eleccion de una corrida cerrada y no sus secretos, resultados o consumos',()=>{
  const runId='b'.repeat(32),prior={runId,closed:true,authorizedPlan:true,baseUrl:BASE_URL,authorizationRevision:'20261112-http600',resources:{lotId:'5',recoveryLotId:null,alternateLevel:null},usage:{'SEG-01':{http:185}},results:{},password:'synthetic-private'};
  assert.deepEqual(previousSelection(prior,runId),{lotId:'5'});
  for(const patch of [{closed:false},{baseUrl:'https://elsewhere.invalid/'},{authorizationRevision:'old'},{runId:'a'.repeat(32)},{resources:{lotId:'0',recoveryLotId:null,alternateLevel:null}},{resources:{lotId:'5',recoveryLotId:'6',alternateLevel:'8'}}])assert.throws(()=>previousSelection({...prior,...patch},runId),/DOC119_PREVIOUS_SELECTION_INVALID/);
});

test('DOC119 timeout de lectura tiene codigo cerrado sin incluir mensaje privado',async()=>{
  const error=Object.assign(new Error('synthetic-private-detail'),{name:'TimeoutError'});
  const page={waitForResponse:()=>Promise.reject(error)};
  await assert.rejects(()=>responseFor(page,'ConsultarRecursosLote',async()=>{},'ARCHIVOS'),error=>error.message==='DOC119_READ_STEP_TIMEOUT');
});

test('DOC119 conexion y guardia sin importacion elegida no inventan referencias',()=>{
  const p=profile();const {lotId,recoveryLotId,alternateLevel,...connection}=p;
  assert.equal(validateConnection(connection).baseUrl,BASE_URL);assert.throws(()=>validateConnection({...connection,lotId:'5'}),/PROFILE_INVALID/);
  const empty={lotId:null,planId:null,level:null};
  for(const [operation,payload]of [['ConsultarLote',{loteId:null}],['ConsultarConfirmacionLote',{planId:null}],['AbrirImportacion',{nivelRef:null}]])assert.equal(permittedRequest('POST',url(operation),payload,empty),false);
});
test('DOC119 permite elegir una opcion por fecha/estado sin saber el ID interno',()=>{
  const items=[{id:'105',createdAt:'2026-11-10 08:00',levelName:'Archivo',state:'PARCIAL',totalFiles:3,totalRoots:2,pendingFiles:1},{id:'210'}];
  assert.equal(chooseItem(items,'1').id,'105');assert.equal(chooseItem(items,'2').id,'210');assert.equal(chooseItem(items,'0',true),null);
  for(const value of ['105','3','0','01','1;other'])assert.throws(()=>chooseItem(items,value),/SELECTION_INVALID/);
  const text=describeImport(items[0],0);assert.match(text,/2026-11-10 08:00 UTC/);assert.match(text,/resultado parcial/);assert.match(text,/3 archivos/);assert.doesNotMatch(text,/105/);
});
test('DOC119 descubrimiento usa cuenta y empresa parametrizadas, facultades vigentes y limite',async()=>{
  const login="owner' OR 1=1 --",calls=[],budget=createBudget('SEG-01');
  const result=await discoverImports(async(origin,sql,args)=>{
    calls.push({origin,sql,args});
    if(calls.length===1)return [{usuario_ref:'141',empresa_ref:'2'}];
    return [{lote_id:'5',nivel_ref:'7',level_name:'Area\u001b[31mA',created_at:'2026-11-10 08:00',estado:'PARCIAL',total_archivos:'3',total_raices:'2',pending_files:'1',password:'synthetic-never-projected'}];
  },budget,login);
  assert.equal(budget.counts.select,2);assert.deepEqual(calls[0].args,[login]);assert.equal(calls[0].sql.includes(login),false);
  assert.deepEqual(calls[1].args,['141','2','2','141','141']);assert.match(calls[1].sql,/l\.solicitante_ref=\?/);assert.match(calls[1].sql,/l\.ambito_ref=\?/);
  assert.match(calls[1].sql,/p\.agregar_expediente=1 AND p\.carga_archivo=1/);assert.match(calls[1].sql,/LIMIT 30$/);
  assert.equal(result.items[0].id,'5');assert.equal(result.items[0].pendingFiles,1);assert.doesNotMatch(describeImport(result.items[0],0),/\u001b/);
  assert.equal(JSON.stringify(result).includes(login),false);assert.equal(JSON.stringify(result).includes('synthetic-never-projected'),false);
  for(const call of calls){assert.match(call.sql,/^SELECT /);assert.doesNotMatch(call.sql,/\b(?:UPDATE|INSERT|DELETE|CALL|SET|DROP)\b/i);}
});
test('DOC119 descubrimiento rechaza cuentas ambiguas y no lista otra identidad',async()=>{
  for(const users of [[],[{usuario_ref:'141',empresa_ref:'2'},{usuario_ref:'142',empresa_ref:'3'}],[{usuario_ref:'0',empresa_ref:'2'}]]){
    let count=0;await assert.rejects(()=>discoverImports(async()=>{count++;return users;},createBudget('SEG-01'),'synthetic'),/ACCOUNT_AMBIGUOUS_OR_UNAVAILABLE/);assert.equal(count,1);
  }
  const empty=await discoverImports(async(_origin,sql)=>sql.includes('Login_Usuario')?[{usuario_ref:'141',empresa_ref:'2'}]:[],createBudget('SEG-01'),'synthetic');assert.deepEqual(empty.items,[]);
});
test('DOC119 destinos y archivo por recibir se descubren dentro de la cuenta elegida',async()=>{
  const principal={userRef:'141',companyRef:'2'},budget=createBudget('SEG-02'),calls=[];
  const query=async(origin,sql,args)=>{calls.push({origin,sql,args});return sql.includes('parent_name')?[{id_nivel:'8',nombre_nivel:'Archivo',parent_name:'Central'}]:[{archivo_id:'30',origen_doc:JSON.stringify({Nombre:'a.pdf',RutaRelativa:'Carpeta/a.pdf'}),bytes_previstos:'7'}];};
  assert.deepEqual(await discoverLevels(query,budget,principal,'7'),[{id:'8',name:'Archivo',parent:'Central'}]);
  assert.deepEqual(calls[0].args,['7','2','141','141','7','7']);assert.match(calls[0].sql,/n\.id_nivel<>\?/);
  assert.match(calls[0].sql,/ORDER BY CASE WHEN n\.id_nivel_padre=\? THEN 0/);
  assert.deepEqual(await pendingFile(query,budget,principal,'5'),{id:'30',name:'a.pdf',relativePath:'Carpeta/a.pdf',bytes:'7'});
  assert.deepEqual(calls[1].args,['5','141','2']);assert.match(calls[1].sql,/l\.solicitante_ref=\? AND l\.ambito_ref=\?/);assert.match(calls[1].sql,/LIMIT 1$/);
  assert.equal(budget.counts.select,2);
  await assert.rejects(()=>pendingFile(async()=>[{archivo_id:'30',origen_doc:JSON.stringify({Nombre:'a.pdf',RutaRelativa:'C:\\private\\a.pdf'}),bytes_previstos:'7'}],budget,principal,'5'),/FILE_METADATA_INVALID/);
});
