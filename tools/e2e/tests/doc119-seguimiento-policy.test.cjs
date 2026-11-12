'use strict';
const test=require('node:test'),assert=require('node:assert/strict');
const {BASE_URL,LIMITS,READS,validateProfile,createBudget,permittedRequest,installGuard,makeControls,csvRows,hash}=require('./support/doc119-seguimiento.cjs');
const {parseArgs,main}=require('../scripts/run-doc119-seguimiento.cjs');
const profile=()=>({baseUrl:BASE_URL,module:'GESTOR',ignoreHttpsErrors:false,timeoutMilliseconds:60000,odbcDsn:'MembershipUsers',docuarchiOdbcDsn:'MembershipUsers',lotId:'5',recoveryLotId:null,alternateLevel:null});
const scope={lotId:'5',level:'7',planId:'17'};
const url=operation=>new URL('webservice/WebServiceImportacionCarpetas.asmx/'+operation,BASE_URL).href;

test('DOC119 selector exige autorizacion del plan y runId sin texto ejecutable',()=>{
  assert.deepEqual(parseArgs(['--check']),{check:true});
  assert.deepEqual(parseArgs(['--run','a'.repeat(32),'--authorized-plan']),{runId:'a'.repeat(32)});
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
  assert.deepEqual(budget.counts,{http:120,select:80,sessions:2});assert.notEqual(snapshots[0],budget.counts);
  assert.throws(()=>budget.take('write'),/DOC119_BUDGET_KIND_INVALID/);assert.throws(()=>createBudget('other'),/DOC119_STAGE_INVALID/);
  assert.deepEqual(LIMITS['SEG-02'],{http:80,select:60,sessions:2});
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
  assert.equal(first.fingerprint,same.fingerprint);assert.equal(queries.length,14);assert.equal(queries.filter(q=>q.origin==='DA').length,2);
  assert.deepEqual(queries.find(q=>q.origin==='DA').args,['141','650']);revision++;
  assert.notEqual((await controls.snapshot('5')).fingerprint,first.fingerprint);
});
test('DOC119 recursos/contextos no validos fallan antes de continuar',async()=>{
  const controls=makeControls(async(_origin,sql)=>sql.includes('config_doc,contexto_doc')?[{lote_id:'5',contexto_doc:'{}'}]:[],createBudget('SEG-02'));
  await assert.rejects(()=>controls.snapshot('5'),/DOC119_CONTEXT_INVALID/);await assert.rejects(()=>controls.snapshot('0'),/DOC119_LOT_INVALID/);
});
test('DOC119 --check no pide credenciales ni abre una sesion',async()=>{await main(['--check']);});
