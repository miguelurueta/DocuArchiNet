'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto'),vm=require('node:vm');
const root=path.resolve(__dirname,'..'),support=require('../tools/e2e/tests/support/doc125-excel-scope.cjs'),runner=require('../tools/e2e/scripts/run-doc110-profile-session.cjs');
const profile={baseUrl:'https://fixture.invalid/app/',module:'SYNTHETIC',ignoreHttpsErrors:true};
const sha=p=>crypto.createHash('sha256').update(fs.readFileSync(p)).digest('hex');
const directory=fs.mkdtempSync(path.join(root,'.tmp','doc127-unit-'));
const runtime=path.join(directory,'synthetic-runtime');
for(const relative of ['bin/GestionDocumental-Docuarchi.net.dll','Web.config',...require('../tools/e2e/tests/support/doc126-excel-scope.cjs').assetPaths]){
 const file=path.join(runtime,relative);fs.mkdirSync(path.dirname(file),{recursive:true});fs.writeFileSync(file,'SYNTHETIC RUNTIME ONLY '+relative);
}
function scope(modo='AGREGAR',formato='XLSX',filas=1000){
 const fixture=path.join(root,'tests/Fixtures/ProduccionDocumental/ImportacionExcel/valido.'+formato.toLowerCase());
 const settings=path.join(runtime,'AppSettings.local.config');
 return {issue:'DOC-127',mode:'capacity',baseUrl:profile.baseUrl,disposable:true,preserveResources:true,destinationReadOnly:false,
  empresaId:2,usuarioId:141,moduloId:11,accountPolicy:'EXISTING_AUTHORIZED',expectedImportPermissions:{cargas:1,carpetas:1,perfiles:1,fuentes:1,proveedores:1},
  controlPrefix:'DOC127_DA_E2E',raControlPrefix:'DOC127_RA_E2E',environmentHash:'a'.repeat(64),raEnvironmentHash:'b'.repeat(64),runtimeRoot:runtime,
  backendSha256:sha(path.join(runtime,'bin/GestionDocumental-Docuarchi.net.dll')),webConfigSha256:sha(path.join(runtime,'Web.config')),localSettingsSha256:fs.existsSync(settings)?sha(settings):crypto.createHash('sha256').update('').digest('hex'),
  assets:Object.fromEntries(require('../tools/e2e/tests/support/doc126-excel-scope.cjs').assetPaths.map(p=>[p,sha(path.join(runtime,p))])),
  protectedTables:['imp_02_contratos','imp_02_egresos','imp_02_creditos'],maxBusinessRows:filas,maxDeletedRows:modo==='REEMPLAZAR'?1000:0,
  allowedEffects:['INSERT','RESULTADO',...(modo==='CREAR'?['CREATE']:modo==='REEMPLAZAR'?['DELETE']:[])],
  window:{id:crypto.randomUUID().replaceAll('-',''),authorizationReference:'SYNTHETIC UNIT ONLY; no real authorization',validFromUtc:new Date(Date.now()-1000).toISOString(),expiresUtc:new Date(Date.now()+600000).toISOString(),timeoutMs:60000,maxHttpRequests:12,maxSelectStatements:100},
  cases:[{escenario:'capacidad',modo,formato,filasEsperadas:filas,conexionId:6,plantillaId:58,revisionEsperada:1,huellaEsperada:'c'.repeat(64),huellaDestino:'d'.repeat(64),archivoHash:sha(fixture),archivoPath:fixture,tabla:modo==='CREAR'?'imp_02_doc127_e2e_crear':'imp_02_doc126_e2e_'+(modo==='AGREGAR'?'agregar':'reemplazar'),hoja:'Datos',filasDestinoEsperadas:modo==='CREAR'?0:1000,codigoEsperado:'OK'}]};
}
const save=s=>{const p=path.join(directory,s.window.id+'.json');fs.writeFileSync(p,JSON.stringify(s));return p;};
test('DOC127 acepta las18 combinaciones declaradas sin relajar DOC125/126',()=>{
 for(const mode of ['CREAR','AGREGAR','REEMPLAZAR'])for(const format of ['XLS','XLSX'])for(const n of [1000,5000,10000])assert.equal(support.validateScope(scope(mode,format,n),profile.baseUrl,'DOC-127').cases[0].filasEsperadas,n);
 assert.throws(()=>support.validateScope(scope(),profile.baseUrl),/DOC125_SCOPE_INVALID/);
});
test('DOC127 rechaza alcances incompletos, recursos ajenos y efectos mayores',()=>{
 const changes=[s=>s.issue='DOC-126',s=>s.cases.push({...s.cases[0]}),s=>s.cases[0].tabla='imp_02_egresos',s=>s.cases[0].filasEsperadas=20000,s=>s.cases[0].formato='CSV',s=>s.cases[0].archivoHash='',s=>s.cases[0].huellaDestino='',s=>s.cases[0].filasDestinoEsperadas=-1,s=>s.maxBusinessRows++,s=>s.maxDeletedRows=1,s=>s.allowedEffects.push('DELETE'),s=>s.protectedTables=[],s=>s.expectedImportPermissions.cargas=0,s=>s.window.authorizationReference='',s=>s.window.timeoutMs=900001,s=>s.window.maxHttpRequests=21,s=>s.cases[0].reanudarConfirmada={},s=>s.webConfigSha256='',s=>s.localSettingsSha256='',s=>s.protectedTables.push(s.cases[0].tabla)];
 for(const change of changes){const s=scope();change(s);assert.throws(()=>support.validateCapacityScope(s,profile.baseUrl),/DOC127_SCOPE_INVALID/);}
});
test('DOC127 no hereda autorizacion previa ni mezcla modos',()=>{
 const s=scope(),file=save(s),env={DOC127_E2E_SCOPE_PATH:file,DOC126_E2E_AUTHORIZED:'YES'};
 assert.throws(()=>runner.excelCapacityPlan(path.join(runtime,'tools/e2e'),profile,['--excel-capacity'],env),/DOC127_AUTHORIZATION_REQUIRED/);
 env.DOC127_CAPACITY_E2E_AUTHORIZED='YES';assert.throws(()=>runner.excelCapacityPlan(path.join(runtime,'tools/e2e'),profile,['--excel-capacity','--excel-confirm'],env),/DOC127_MODE_CONFLICT/);
 const plan=runner.excelCapacityPlan(path.join(runtime,'tools/e2e'),profile,['--excel-capacity'],env);assert.ok(plan.childArgs.includes('@doc127-capacity'));assert.ok(plan.childArgs.includes('--retries=0'));assert.equal(plan.needed.length,8);
});
test('DOC127 ventana vencida, futura o consumida se rechaza antes de autenticar',()=>{
 const s=scope();assert.throws(()=>support.assertCapacityWindow(s,Date.parse(s.window.expiresUtc)),/DOC127_WINDOW_EXPIRED/);assert.throws(()=>support.assertCapacityWindow(s,Date.parse(s.window.validFromUtc)-1),/DOC127_WINDOW_EXPIRED/);
 const file=save(s);support.claimCapacityWindow(s,file);assert.throws(()=>support.claimCapacityWindow(s,file),/DOC127_WINDOW_CONSUMED/);
 support.beginCapacityExecution(s,file);assert.throws(()=>support.beginCapacityExecution(s,file),/DOC127_WINDOW_CONSUMED/);
 assert.throws(()=>runner.excelCapacityPlan(path.join(runtime,'tools/e2e'),profile,['--excel-capacity'],{DOC127_CAPACITY_E2E_AUTHORIZED:'YES',DOC127_E2E_SCOPE_PATH:file}),/DOC127_WINDOW_CONSUMED/);
});
test('DOC127 cambio de scope o runtime invalida el permiso de ejecucion local',()=>{
 const s=scope(),file=save(s);support.claimCapacityWindow(s,file);fs.appendFileSync(file,'\n');assert.throws(()=>support.verifyCapacityClaim(s,file),/DOC127_CLAIM_REQUIRED/);
 const changed=scope();changed.backendSha256='0'.repeat(64);assert.throws(()=>support.verifyCapacityRuntime(changed),/DOC126_RUNTIME_CHANGED/);
 changed.backendSha256=s.backendSha256;changed.webConfigSha256='0'.repeat(64);assert.throws(()=>support.verifyCapacityRuntime(changed),/DOC127_RUNTIME_CHANGED/);
});
test('DOC127 memoria declara baseline, intervalo observado y procesos compartidos',()=>{
 const result=support.summarizeCapacityMemory([{utc:'2026-11-25T10:00:00Z',phase:'BASELINE',workers:[{workingSetBytes:100,privateBytes:150}]},{utc:'2026-11-25T10:00:05Z',phase:'CONFIRM',workers:[{workingSetBytes:200,privateBytes:400},{workingSetBytes:50,privateBytes:100}]}]);
 assert.equal(result.baseline.rss,100);assert.equal(result.maxRssBytes,250);assert.equal(result.maxPrivateBytes,500);assert.equal(result.maxGapMs,5000);assert.equal(result.exclusiveAllocationClaimed,false);assert.equal(support.summarizeCapacityMemory([]).available,false);
});
// Ejecuta el recorrido nuevo con transporte y controles SINTETICOS en memoria.
// Nunca escribe un resultado REAL_PASS a disco ni crea una sesion de aplicacion.
async function simulatedFlow(fault,mode='AGREGAR',format='XLSX',rows=1000){
 const s=scope(mode,format,rows),file=save(s),c=s.cases[0],writes=new Map(),calls=[],requests=[];let handler,recording=false,committed=false,commitWrites=0,logins=0;
 const doc=JSON.stringify({Limites:{MaxFilasDatos:20000,MaxArchivoMB:10},IdentidadFisica:'e'.repeat(64)}),configurationHash=crypto.createHash('sha256').update(doc).digest('hex');
 const load={IdCarga:'11111111-1111-4111-8111-111111111111',ArchivoHash:c.archivoHash,ConfiguracionHash:configurationHash,EsquemaHash:'f'.repeat(64),Estado:'PREPARADA',FilasConfirmadas:rows};
 const tableHash=()=>committed?'e'.repeat(64):(fault==='baseline'?'0'.repeat(64):c.huellaDestino);
 const scopeSupport={...support,beginCapacityExecution:()=>({scopeSha256:support.capacityScopeHash(file)}),verifyCapacityRuntime:()=>{},snapshotDestination:async(_q,_f,table)=>table===c.tabla?tableHash():'a'.repeat(64)};
 const select=async sql=>{
  assert.match(sql,/^SELECT /);
  if(sql.includes('AS login_ref'))return [{id:141,login_ref:'synthetic',activo:1,...s.expectedImportPermissions,...(fault==='permission'?{cargas:0}:{})}];
  if(sql.startsWith('SELECT COLUMN_NAME'))return [{COLUMN_NAME:'remit_dest_interno_idremit_dest_interno'},{COLUMN_NAME:'ADMINISTRAR_CARGAS_IMPORTACION'}];
  if(sql.includes('COUNT(*)'))return [{total:sql.includes('imp01_carga_ejecucion')?0:committed?(mode==='AGREGAR'?c.filasDestinoEsperadas+rows:rows):c.filasDestinoEsperadas}];
  if(sql.includes('configuracion_doc'))return [{ambito_ref:'2',usuario_ref:'141',conexion_ref:'DA:11:6',tabla_destino:c.tabla,configuracion_doc:doc}];
  if(sql.includes('filas_confirmadas'))return [{estado:'CONFIRMADA',filas_confirmadas:rows}];
  if(sql.includes('@@hostname'))return [{servidor:'synthetic',puerto:3306,base_ref:'synthetic',minusculas:0}];
  if(sql.includes('IS_FREE_LOCK'))return [{carga_libre:1,destino_libre:1}];throw Error('Unclassified synthetic SELECT');
 };
 const transport=async(_request,url,options)=>{
  const operation=new URL(url).pathname.split('/').pop();calls.push(operation);requests.push({operation,options});let body;
  if(operation==='AbrirCargaExcel')body={ok:true,datos:{CsrfToken:'synthetic-token',Conexiones:[{ConexionId:6}]}};
  else if(operation==='ConsultarPlantilla')body={ok:true,datos:{REVISION_CARGA:1,Huella:c.huellaEsperada,NOMBRE_PLANTILLA:c.tabla,Formato:fault==='format'?'CSV':format}};
  else if(operation==='IniciarRecepcionExcel')body={ok:true,datos:{...load,ReferenciaRecepcion:'synthetic-reference'}};
  else if(operation==='ImportacionExcelUpload.ashx')return {status:()=>200,json:async()=>({ok:true,datos:load})};
  else if(operation==='Preparar')body={ok:fault!=='prepare',datos:{ValidacionCompleta:true,Carga:load,FilasObservadas:fault==='partial'?rows-1:rows}};
  else if(operation==='Confirmar'){if(!committed)commitWrites++;committed=true;if(fault==='uncertain')throw Error('physical outcome deliberately unknown in this synthetic transport');body={ok:true,datos:{...load,Estado:'CONFIRMADA'}};}
  else if(operation==='ConsultarResultado')body={ok:true,datos:{...load,Estado:'CONFIRMADA'}};
  else throw Error('Unclassified synthetic HTTP');return {status:()=>200,json:async()=>({d:body})};
 };
 const fakeTest=(_name,fn)=>{if(recording)handler=fn;};fakeTest.skip=()=>{};fakeTest.use=()=>{};fakeTest.setTimeout=()=>{};fakeTest.describe=(_name,fn)=>{const previous=recording;recording=_name.startsWith('DOC127');if(recording)fn();recording=previous;};fakeTest.describe.configure=()=>{};
 const fakeFs={...fs,writeFileSync:(p,data)=>writes.set(p,String(data)),renameSync:()=>{}};
 const imports={'@playwright/test':{test:fakeTest},'node:fs':fakeFs,'node:path':path,'node:crypto':crypto,'./support/doc125-excel-scope.cjs':scopeSupport,'./support/doc126-excel-scope.cjs':require('../tools/e2e/tests/support/doc126-excel-scope.cjs'),'./support/doc111-importacion-carpetas.cjs':{},'./support/doc110-safe-request.cjs':{safePost:transport},'./support/authenticated-workflow-session.cjs':{createWorkflowLoginPageSession:async()=>{logins++;return {page:{goto:async()=>{}},context:{request:{},close:async()=>{}}};}},'../scripts/support/doc32-e2e-odbc.cjs':{queryRows:select,queryFingerprint:async()=> 'a'.repeat(64)}};
 vm.runInNewContext(fs.readFileSync(path.join(root,'tools/e2e/tests/importacion-excel.spec.cjs'),'utf8'),{require:name=>{if(!(name in imports))throw Error('Unexpected synthetic import: '+name);return imports[name];},process:{env:{DOC127_CAPACITY_E2E_AUTHORIZED:'YES',DOC127_E2E_SCOPE_PATH:file,DOC127_E2E_BASE_URL:profile.baseUrl,DOC127_E2E_USER:'synthetic'}},URL,performance,console:{log:()=>{}},setTimeout});
 assert.equal(typeof handler,'function');let failure;try{await handler({browser:{}});}catch(error){failure=error.message;}
 const result=JSON.parse(writes.get(support.capacityPaths(s).claim+'.result.json'));return {calls,requests,commitWrites,logins,result:{...result,status:result.status==='REAL_PASS'?'SYNTHETIC_PASS':result.status,real:false,executionType:'SYNTHETIC_TRANSPORT_ONLY'},failure};
}

for(const mode of ['CREAR','AGREGAR','REEMPLAZAR'])for(const format of ['XLS','XLSX'])for(const rows of [1000,5000,10000]){
 test(`DOC127 transporte sintetico ${format}/${rows}/${mode}: preparar, confirmar, consultar y replay`,async()=>{
  const r=await simulatedFlow(undefined,mode,format,rows);
  assert.equal(r.failure,undefined);assert.equal(r.result.real,false);assert.equal(r.result.status,'SYNTHETIC_PASS');assert.equal(r.commitWrites,1);
  assert.equal(r.result.filas,rows);assert.equal(r.result.formato,format);assert.equal(r.result.modo,mode);
  assert.ok(r.result.timingsMs.preparar>=0&&r.result.timingsMs.confirmar>=0);
  const prepare=r.requests.find(x=>x.operation==='Preparar').options.data.solicitud;
  assert.equal(prepare.Modo,mode);assert.equal(prepare.Hoja,'Datos');
  const upload=r.requests.find(x=>x.operation==='ImportacionExcelUpload.ashx').options.multipart;
  assert.equal(path.extname(upload.archivo.name),'.'+format.toLowerCase());
  const confirms=r.requests.filter(x=>x.operation==='Confirmar');assert.equal(confirms.length,2);
  assert.equal(confirms[0].options.data.solicitud.ConfirmacionReemplazo,mode==='REEMPLAZAR');
  assert.deepEqual(confirms[1].options.data,confirms[0].options.data);
  assert.equal(r.calls.filter(x=>x==='IniciarRecepcionExcel').length,1);
  assert.equal(r.calls.includes('GuardarPlantilla'),false);
 });
}

test('DOC127 permiso perdido se rechaza antes de autenticar en el transporte sintetico',async()=>{
 const r=await simulatedFlow('permission');assert.equal(r.failure,'DOC127_OPERATION_FAILED');
 assert.equal(r.logins,0);assert.equal(r.calls.length,0);assert.equal(r.commitWrites,0);
});

test('DOC127 formato distinto o filas incompletas impiden confirmar en todos los modos sinteticos',async()=>{
 for(const mode of ['CREAR','AGREGAR','REEMPLAZAR'])for(const format of ['XLS','XLSX'])for(const fault of ['format','partial']){
  const r=await simulatedFlow(fault,mode,format,5000);assert.match(r.failure,/DOC127_/);
  assert.equal(r.calls.includes('Confirmar'),false);assert.equal(r.commitWrites,0);
 }
});
test('DOC127 recorrido simulado mide etapas y confirma una vez mas replay sin instalar',async()=>{
 const {calls,result,failure}=await simulatedFlow();assert.equal(failure,undefined);assert.equal(calls.filter(x=>x==='Confirmar').length,2);assert.ok(result.timingsMs.preparar>=0&&result.timingsMs.confirmar>=0);assert.ok(!calls.includes('GuardarPlantilla'));
});
test('DOC127 cambio de destino o preparacion invalida detiene la escritura en transporte simulado',async()=>{
 for(const fault of ['baseline','prepare']){const r=await simulatedFlow(fault);assert.match(r.failure,/DOC127_/);assert.equal(r.calls.includes('Confirmar'),false);}
});
test('DOC127 acuse incierto conserva ID y detiene reintentos en transporte simulado',async()=>{
 const r=await simulatedFlow('uncertain');assert.equal(r.calls.filter(x=>x==='Confirmar').length,1);assert.equal(r.calls.includes('ConsultarResultado'),false);assert.equal(r.result.confirmationState,'SENDING');assert.ok(r.result.idCarga);assert.equal(r.result.status,'FAILED_REQUIRES_REVIEW');
});
