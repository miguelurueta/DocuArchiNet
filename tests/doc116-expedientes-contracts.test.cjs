'use strict';
const test=require('node:test'),assert=require('node:assert/strict');
const {validateSession,assertLevelGrant,createLevelControls,createCreationClient,safeFailureCode,assertRunnable,assertInitialSnapshot,runCreationScenario}=require('../tools/e2e/tests/support/doc116-expedientes-importacion.cjs');
const profile={baseUrl:'https://test.invalid/app/',module:'GESTOR',ignoreHttpsErrors:false,timeoutMilliseconds:90000,odbcDsn:'MembershipUsers',docuarchiOdbcDsn:'MembershipUsers',nivelRef:'7',versionId:'38',mode:'LOTE',pdfFixture:'doc111-carga-individual.pdf',rootNames:['DOC116-SYNTHETIC-01','DOC116-SYNTHETIC-02'],expectations:[0,1].map(i=>({codigo:'synthetic-'+i,nombre:'synthetic-'+i,tipologiaId:'1',indices:[{referencia:'1',tipo:'TEXTO',valor:'synthetic'}]}))};
const session=scenario=>({scenario:scenario||'EXP-01',profile:structuredClone(profile),identity:{userRef:'141',ambitoRef:'2'},levelB:'8',lotId:'1',roots:[{rootId:'2',revision:1,key:'root-two'},{rootId:'3',revision:1,key:'root-three'}]});
const a={environment:true,account:true,select:true,effects:true,discardable:true};
const grant=()=>({nivel:'7',actor:'141',empresa:'2',propietario:'142',nivel_activo:'1',estado_permiso:'1',agregar_expediente:'1'});
function transport(respond){const calls=[],references=[];const auth={page:{locator:()=>({getAttribute:async()=> 'SYNTHETIC_TOKEN'})},context:{request:{post:async(url,options)=>{calls.push({url,options});return respond(url,options,calls.length);}}}};return {auth,calls,references};}
const response=(data,status=200,ok=true,code='OK')=>({status:()=>status,json:async()=>({d:{ok,codigo:code,datos:data}})});
const created=(root='2',exp='60',rel='70')=>({loteId:'1',raizId:root,fase:'CREAR_EXPEDIENTE',estado:'CONFIRMADO',expedienteId:exp,relacionNivelId:rel});
test('session cannot inject authority, destinations, failure hooks, or duplicate roots',()=>{
 for(const mutate of [s=>s.password='rejected-synthetic',s=>s.snapshot={},s=>s.phase='ALMACENAR',s=>s.identity.owner=true,s=>s.roots[0].revision=0,s=>s.roots[1].rootId='2',s=>s.levelB='7',s=>s.lotId='1 OR 1=1',s=>s.failAfterCreation=true]){const s=session();mutate(s);assert.throws(()=>validateSession(s),/^Error: DOC116_/);}
});
test('grant verification rejects owner bypass, wrong scope and ineffective concessions',()=>{
 assert.equal(assertLevelGrant(grant(),session().identity,'7').ownerBypass,false);
 for(const [k,v]of Object.entries({propietario:'141',actor:'142',empresa:'3',nivel:'8',nivel_activo:'0',estado_permiso:'0',agregar_expediente:'0'})){const r=grant();r[k]=v;assert.throws(()=>assertLevelGrant(r,session().identity,'7'),/^Error: DOC116_/);}
 const r=grant();r.agregar_expediente='0';assert.equal(assertLevelGrant(r,session().identity,'7',false).granted,false);
});
test('level controls use only a bounded SELECT and parameterized actor, company, level',async()=>{
 let queries=0;const controls=createLevelControls(async(origin,sql,args)=>{queries++;assert.equal(origin,'RA');assert.match(sql,/^SELECT /);assert.doesNotMatch(sql,/;|FOR UPDATE|OUTFILE|UPDATE\b/);assert.match(sql,/LIMIT 2$/);assert.deepEqual(args,['7','141','2']);return [grant()];},session('EXP-03'));
 await controls.grant();assert.equal(queries,1);
 await assert.rejects(createLevelControls(async()=>[grant(),grant()],session('EXP-03')).grant(),/NOT_UNIQUE/);
 await assert.rejects(createLevelControls(async()=>{throw Error('PRIVATE_CONNECTION_SENTINEL');},session('EXP-03')).grant(),e=>e.message==='DOC116_LEVEL_SELECT_FAILED'&&!e.cause);
});
test('creation requires every authorization before any transport',()=>{
 const t=transport(()=>{throw Error('not reached');});for(const key of Object.keys(a)){const auth={...a,[key]:false};assert.throws(()=>createCreationClient(t.auth,session(),auth),/AUTHORIZATION_REQUIRED/);}assert.equal(t.calls.length,0);
 const s=session();s.profile.ignoreHttpsErrors=true;assert.throws(()=>createCreationClient(t.auth,s,a),/AUTHORIZATION_REQUIRED/);
});
test('only the fixed creation phase is sent and historical replay preserves identity',async()=>{
 const t=transport((_url,o)=>response(created(o.data.solicitud.recursoId)));const s=session(),client=createCreationClient(t.auth,s,a,r=>t.references.push(r));await client.refreshToken();s.roots[0].key='changed-after-validation';
 const first=await client.create('2'),second=await client.create('2');assert.deepEqual(first,second);assert.equal(t.references.length,2);
 assert.deepEqual(t.calls[0].options.data,{solicitud:{loteId:'1',recursoId:'2',fase:'CREAR_EXPEDIENTE',revisionEsperada:1,claveOperacion:'root-two'}});
 assert.equal(t.calls[0].options.maxRetries,0);assert.equal(t.calls[0].options.maxRedirects,0);assert.equal(client.counters().createdRoots,1);
 await assert.rejects(client.create('999'),/ROOT_NOT_AUTHORIZED/);assert.equal(t.calls.length,2);
});
test('unknown transport outcome cannot be unlocked by a successful state read',async()=>{
 const t=transport(url=>{if(url.endsWith('ConsultarLote'))return response({loteId:'1',nivelRef:'7'});throw Error('PRIVATE_COOKIE_SENTINEL');});const client=createCreationClient(t.auth,session(),a);await client.refreshToken();
 await assert.rejects(client.create('2'),e=>e.message==='DOC116_OPERATION_NOT_CONFIRMED'&&!e.cause);await client.consult();await assert.rejects(client.create('2'),/UNCERTAINTY_REQUIRES_REVIEW/);assert.equal(t.calls.length,2);
});
test('known IDs are retained before an unexpected success in a denial scenario is rejected',async()=>{
 const t=transport(()=>response(created()));const client=createCreationClient(t.auth,session('EXP-03'),a,r=>t.references.push(r));await client.refreshToken();await assert.rejects(client.create('2'),/UNEXPECTED_CREATION/);assert.equal(t.references[0].expedienteId,'60');assert.equal(client.counters().blocked,true);
});
test('only the expected level-permission rejection is accepted for EXP03',async()=>{
 const t=transport(()=>response(null,403,false,'ACCESO_DENEGADO'));const client=createCreationClient(t.auth,session('EXP-03'),a);await client.refreshToken();assert.deepEqual(await client.create('2'),{denied:true});await assert.rejects(client.create('2'),/UNCERTAINTY_REQUIRES_REVIEW/);assert.equal(t.calls.length,1);
});
test('discarding a client result is explicit and does not assert an internal failure',async()=>{
 const t=transport(()=>response(created())),client=createCreationClient(t.auth,session('EXP-02'),a,r=>t.references.push(r));await client.refreshToken();assert.deepEqual(await client.create('2',{discardClientResult:true}),{observation:'CLIENT_RESULT_DISCARDED'});assert.equal(t.references.length,1);await client.create('2');await assert.rejects(client.create('2'),/BUDGET_EXHAUSTED/);assert.equal(t.calls.length,2);
});
test('replay with changed identity is retained but never accepted as confirmation',async()=>{
 const t=transport((_url,_o,n)=>response(created('2',n===1?'60':'61'))),client=createCreationClient(t.auth,session(),a,r=>t.references.push(r));await client.refreshToken();await client.create('2');await assert.rejects(client.create('2'),/REPLAY_CHANGED_IDENTITY/);assert.equal(t.references[1].expedienteId,'61');assert.equal(client.counters().blocked,true);
});
test('read response must preserve the frozen level',async()=>{
 const t=transport(()=>response({loteId:'1',nivelRef:'8'})),client=createCreationClient(t.auth,session(),a);await client.refreshToken();await assert.rejects(client.consult(),/DESTINATION_CHANGED/);
 assert.equal(safeFailureCode(Error('PRIVATE_DB_SENTINEL')),'DOC116_OPERATION_NOT_CONFIRMED');
 assert.equal(safeFailureCode(Error('DOC116_PRIVATE_SENTINEL')),'DOC116_OPERATION_NOT_CONFIRMED');
});
function readyView(){
 const config={GabineteId:'10',ModoIntegridad:'COMPLETO_OBLIGATORIO',AmbitoIntegridad:'LOTE',FondoRef:'1',InstrumentoRef:'2',AreaRef:'3',SerieRef:'4',SubserieRef:null,TipoExpedienteRef:'5',UnidadDocumentalRef:'6'};
 return {lot:{lote_id:'1',config_doc:JSON.stringify({VersionId:'38',Configuracion:config}),contexto_doc:JSON.stringify({UsuarioRef:'141',AmbitoRef:'2',NivelRef:'7'}),estado:'LISTO',revision:1,detencion_solicitada:0,total_raices:2,total_archivos:2,total_bytes:'1354'},roots:[0,1].map(i=>({raiz_id:String(i+2),cliente_raiz_id:'r'+(i+1),datos_doc:JSON.stringify({NombreOriginal:profile.rootNames[i]}),estado:'PENDIENTE',identidad_expediente_doc:null,intento_activo_id:null,revision:1})),files:[0,1].map(i=>({archivo_id:String(i+4),raiz_id:String(i+2),cliente_archivo_id:'a'+(i+1),bytes_previstos:'677',estado_recepcion:'RECIBIDO',estado_efecto:'NO_INICIADO',valores_efectivos_doc:null,resultado_doc:null,intento_activo_id:null,revision:1})),attempts:[0,1].map(i=>({intento_id:String(i+1),raiz_id:String(i+2),archivo_id:String(i+4),fase:'RECEPCION',estado:'CONFIRMADO',evidencia_doc:'{}',fin_utc:'2026-11-05'})),events:[]};
}
function integratedFixture(scenario,options={}){
 const s=session(scenario),v=readyView(),createdRoots=new Map(),calls=[],selections=[],references=[],bridgeEvents=[];let granted=1;
 if(scenario==='EXP-03')s.levelPermission={permissionId:'9',environmentHash:'a'.repeat(64)};
 const t=transport((url,o)=>{
   calls.push(url);
   if(url.endsWith('/ConsultarLote'))return response({loteId:'1',nivelRef:'7'});
   assert.ok(url.endsWith('/AvanzarLote'));assert.equal(o.data.solicitud.fase,'CREAR_EXPEDIENTE');
   if(scenario==='EXP-03'&&granted===0)return response(null,403,false,'ACCESO_DENEGADO');
   const id=o.data.solicitud.recursoId;
   if(!createdRoots.has(id)){
    const n=s.roots.findIndex(r=>r.rootId===id),identity={OrigenLogico:'RA',NivelRef:'7',ExpedienteId:String(60+n),RelacionNivelId:String(70+n)},attemptId=String(10+n);
    createdRoots.set(id,identity);const r=v.roots[n];r.estado='EXPEDIENTE_CONFIRMADO';r.identidad_expediente_doc=JSON.stringify(identity);r.revision++;v.lot.revision++;
    v.attempts.push({intento_id:attemptId,raiz_id:id,archivo_id:null,fase:'CREAR_EXPEDIENTE',estado:'CONFIRMADO',evidencia_doc:JSON.stringify({Estado:'CONFIRMADO',Expediente:identity,Creacion:{IntentoId:attemptId,CicloArchivoId:'13',CicloArchivoNombre:'Archivo Gestión'}}),fin_utc:'2026-11-05'});v.events.push({evento_id:attemptId,intento_id:attemptId,accion:'CONFIRMAR'});
   }
   const identity=createdRoots.get(id);return response(created(id,identity.ExpedienteId,identity.RelacionNivelId));
 });
 async function query(origin,sql,args){
  assert.match(sql,/^SELECT /);assert.doesNotMatch(sql,/;|\b(?:UPDATE|INSERT|DELETE)\b/);
  if(sql.startsWith('SELECT n.id_nivel AS nivel'))return [{...grant(),permission_id:'9',agregar_expediente:String(granted)}];
  if(sql.includes('AND solicitante_ref=')){assert.deepEqual(args,['1','141','2','7']);return [{lote_id:'1'}];}
  if(sql.startsWith('SELECT lote_id,config_doc'))return [structuredClone(v.lot)];
  if(sql.startsWith('SELECT raiz_id,'))return structuredClone(v.roots);
  if(sql.startsWith('SELECT f.archivo_id,'))return structuredClone(v.files);
  if(sql.startsWith('SELECT intento_id,'))return structuredClone(v.attempts);
  if(sql.startsWith('SELECT evento_id,'))return structuredClone(v.events);
  if(sql.includes('COUNT(*) AS total FROM ra_pro_niveles_has_expediente_archivo'))return [{total:args[0]==='7'?10+createdRoots.size:20}];
  if(sql.startsWith('SELECT e.*')){
   const n=Number(args[0])-60;return [{nivel:options.wrongLevel?'8':'7',ID_USUARIO_GESTION:'141',ID_EMPRESA_EXPEDIENTE:'2',CODIGO_UNICO:profile.expectations[n].codigo,ALEAS_EXPEDIENTE:profile.expectations[n].nombre,TEMA_EXPEDIENTE:profile.expectations[n].nombre,Id_tipos_ciclo_archivo:'13',ID_FONDO:'1',id_instrumento:'2',CODIGO_AREA_TRD:'3',CODIGO_SERIE_TRD:'4',CODIGO_SUB_SERIE_TRD:null,RA_TIP_EXPE_ID_TIPO_EXPEDIENTE:'5',ID_TIPO_UNIDAD_DOCUMENTAL:'6'}];
  }
  if(sql.startsWith('SELECT Nombre_Tipo_ciclo_archivo'))return [{nombre:'Archivo Gestión'}];
  if(sql.startsWith('SELECT NOMBRE FROM system1')){assert.equal(origin,'DA');return [{NOMBRE:'synthetic_cabinet'}];}
  if(sql.startsWith('SELECT COUNT(*) AS total'))return [{total:options.hasDocument?1:0}];
  throw Error('Unexpected SELECT in synthetic fixture');
 }
 const createLevelBridge=async()=>({ready:async()=>({ok:true,stage:'READY',userRef:'141',companyRef:'2',levelRef:'7',permissionId:'9',otherFieldsUnchanged:true}),send:async operation=>{bridgeEvents.push(operation);if(operation==='REVOKE'){granted=0;if(options.lostRevocationResponse)throw Error('SYNTHETIC_REVOCATION_LOST');return {ok:true,stage:'REVOKED',otherFieldsUnchanged:true};}granted=options.restoreFails?0:1;return {ok:true,stage:'RESTORED',otherFieldsUnchanged:true};},close:async()=>{bridgeEvents.push('CLOSE');granted=1;}});
 return {s,v,t,calls,selections,references,createdRoots,bridgeEvents,run:(authorization={...a,exclusive:true,levelPermission:scenario==='EXP-03'})=>runCreationScenario(t.auth,s,authorization,query,r=>references.push(r),{selectLevel:async(_page,level)=>{assert.equal(calls.length,0);selections.push(level);},createLevelBridge})};
}
test('prepared-lot preconditions reject effects, missing reception, stale revision and changed context',()=>{
 assert.doesNotThrow(()=>assertInitialSnapshot(readyView(),session()));
 for(const mutate of [v=>v.roots[0].identidad_expediente_doc='{}',v=>v.roots[0].revision=2,v=>v.files[0].estado_recepcion='PENDIENTE',v=>v.files[0].resultado_doc='{}',v=>v.lot.detencion_solicitada=1,v=>v.lot.estado='UNKNOWN',v=>v.lot.total_bytes='1353',v=>v.lot.contexto_doc=JSON.stringify({UsuarioRef:'999',AmbitoRef:'2',NivelRef:'7'}),v=>v.attempts.push({fase:'CREAR_EXPEDIENTE',estado:'EN_CURSO'})]){const v=readyView();mutate(v);assert.throws(()=>assertInitialSnapshot(v,session()),/^Error: DOC116_/);}
});
test('EXP01 composes two creations and identical replays, A-to-B selection, scoped SELECT and no documents',async()=>{
 const f=integratedFixture('EXP-01'),r=await f.run();assert.equal(r.caseComplete,true);assert.equal(r.rootsCreated,2);assert.equal(r.documents,0);assert.equal(r.counts.creationRequests,4);assert.equal(f.createdRoots.size,2);assert.deepEqual(f.selections,['7','8']);assert.equal(f.references.length,4);assert.equal(r.uploads,0);assert.equal(r.confirmations,0);
});
test('EXP02 returns only client-observation coverage and leaves the other root unchanged',async()=>{
 const f=integratedFixture('EXP-02'),r=await f.run();assert.equal(r.caseComplete,false);assert.equal(r.internalFailure,'NO_EJECUTADO');assert.equal(r.responseObservation,'CLIENT_RESULT_DISCARDED');assert.equal(f.createdRoots.size,1);assert.equal(f.v.roots[1].estado,'PENDIENTE');assert.deepEqual(f.selections,['7']);
});
test('physical destination and definitive-document contradictions stop acceptance',async()=>{
 await assert.rejects(integratedFixture('EXP-01',{wrongLevel:true}).run(),/EXPEDIENTE_VALUES_INVALID/);
 await assert.rejects(integratedFixture('EXP-01',{hasDocument:true}).run(),/DOCUMENT_ALREADY_PRESENT/);
});
test('exclusive window and missing permission mutator stop before HTTP effects',async()=>{
 const f=integratedFixture('EXP-01');await assert.rejects(f.run(a),/EXCLUSIVE_WINDOW_REQUIRED/);assert.equal(f.calls.length,0);
 assert.throws(()=>assertRunnable(session('EXP-03')),/LEVEL_PERMISSION_RESOURCES_REQUIRED/);
});
const {parseArgs,safeLine,main}=require('../tools/e2e/scripts/run-doc116-expedientes-importacion.cjs');
test('selector rejects secret arguments and safe output cannot upgrade incomplete EXP02 coverage',()=>{
 assert.throws(()=>parseArgs(['--session','x','--password','private']),/ARGUMENT_INVALID/);
 const value={scenario:'EXP-02',status:'PASS',caseComplete:false,internalFailure:'NO_EJECUTADO',responseObservation:'CLIENT_RESULT_DISCARDED',rootsCreated:1,documents:0,readOnlyControls:true,replayUnchanged:true,preparations:0,confirmations:0,uploads:0,head:'a'.repeat(40),codeSha256:'b'.repeat(64),backendSha256:'c'.repeat(64),private:'not-output'};
 const line=safeLine('DOC116_RESULT='+JSON.stringify(value),session('EXP-02'));assert.ok(line);assert.ok(!line.includes('not-output'));assert.equal(safeLine('DOC116_RESULT='+JSON.stringify({...value,caseComplete:true}),session('EXP-02')),null);
 assert.equal(safeLine('DOC116_REFERENCE='+JSON.stringify({scenario:'EXP-01',loteId:'999',raizId:'2',expedienteId:'60',relacionNivelId:'70'}),session()),null);
});
test('--check validates resources without console, network or credentials; EXP03 execution stops before prompting',async()=>{
 const fs=require('node:fs'),os=require('node:os'),path=require('node:path'),dir=fs.mkdtempSync(path.join(os.tmpdir(),'doc116-check-')),file=path.join(dir,'session.json'),s=session('EXP-03');
 try{fs.writeFileSync(file,JSON.stringify(s));await main(['--session',file,'--check'],{requireInteractiveConsole:()=>{throw Error('must not run');}});await assert.rejects(main(['--session',file],{requireInteractiveConsole:()=>{throw Error('must not run');}}),/LEVEL_PERMISSION_RESOURCES_REQUIRED/);}finally{fs.unlinkSync(file);fs.rmdirSync(dir);}
});
test('EXP03 revokes exactly the level grant, verifies denial without changes, then verifies restoration',async()=>{
 const f=integratedFixture('EXP-03'),r=await f.run();assert.equal(r.permissionRestored,true);assert.equal(r.denialUnchanged,true);assert.equal(r.rootsCreated,0);assert.equal(r.replayUnchanged,null);assert.equal(f.createdRoots.size,0);assert.deepEqual(f.bridgeEvents,['REVOKE','RESTORE','CLOSE']);assert.equal(r.counts.creationRequests,1);
});
test('lost revocation acknowledgement still requires restoration; a false restoration readback never passes',async()=>{
 const lost=integratedFixture('EXP-03',{lostRevocationResponse:true});await assert.rejects(lost.run(),/SYNTHETIC_REVOCATION_LOST/);assert.deepEqual(lost.bridgeEvents,['REVOKE','RESTORE','CLOSE']);assert.equal(lost.createdRoots.size,0);
 const failed=integratedFixture('EXP-03',{restoreFails:true});await assert.rejects(failed.run(),/LEVEL_GRANT_NOT_EFFECTIVE/);assert.deepEqual(failed.bridgeEvents,['REVOKE','RESTORE','CLOSE']);
});
test('EXP03 requires closed row/environment resources and cannot report success without restored-permission proof',()=>{
 const s=session('EXP-03');s.levelPermission={permissionId:'9',environmentHash:'a'.repeat(64)};assert.doesNotThrow(()=>assertRunnable(s));
 for(const mutate of [s=>s.scenario='EXP-01',s=>s.levelPermission.permissionId='0',s=>s.levelPermission.environmentHash=['a'.repeat(64)],s=>s.levelPermission.password='rejected-synthetic']){const changed=structuredClone(s);mutate(changed);assert.throws(()=>validateSession(changed),/^Error: DOC116_/);}
 const value={scenario:'EXP-03',status:'PASS',caseComplete:true,internalFailure:null,responseObservation:null,rootsCreated:0,documents:0,readOnlyControls:true,replayUnchanged:null,denialUnchanged:true,permissionRestored:true,preparations:0,confirmations:0,uploads:0,head:'a'.repeat(40),codeSha256:'b'.repeat(64),backendSha256:'c'.repeat(64)};
 assert.ok(safeLine('DOC116_RESULT='+JSON.stringify(value),s));assert.equal(safeLine('DOC116_RESULT='+JSON.stringify({...value,permissionRestored:false}),s),null);assert.equal(safeLine('DOC116_RESULT='+JSON.stringify({...value,denialUnchanged:false}),s),null);
});
test('runner delegates login, masks passwords, filters child output and clears its credential environment',async()=>{
 const fs=require('node:fs'),os=require('node:os'),path=require('node:path'),dir=fs.mkdtempSync(path.join(os.tmpdir(),'doc116-runner-')),file=path.join(dir,'session.json'),backend=path.join(dir,'bin/GestionDocumental-Docuarchi.net.dll');
 const s=session();s.runtimeRoot=dir;fs.mkdirSync(path.dirname(backend));fs.writeFileSync(backend,'synthetic fingerprint fixture, not an executable');fs.writeFileSync(file,JSON.stringify(s));let childEnvironment;
 try{
  const result=await main(['--session',file],{
   requireInteractiveConsole:()=>{},collectConfirmation:async(o,k)=>{o[k]='true';},
   collectValue:async(o,k,_label,options)=>{assert.equal(options.secret,k!=='user');o[k]='SYNTHETIC_'+k;},
   runChild:async(_cmd,args,cwd,env,options)=>{
    childEnvironment=env;assert.ok(args.includes('DOC116 EXP-01$'));assert.equal(env.DOC111_E2E_USER,'SYNTHETIC_user');assert.equal(env.DOC111_E2E_PASSWORD,'SYNTHETIC_password');assert.equal(env.DOC116_AUTH_EXCLUSIVE,'true');assert.equal(path.basename(cwd),'e2e');
    const value={scenario:'EXP-01',status:'PASS',caseComplete:true,internalFailure:null,responseObservation:null,rootsCreated:2,documents:0,readOnlyControls:true,replayUnchanged:true,preparations:0,confirmations:0,uploads:0,head:'a'.repeat(40),codeSha256:'b'.repeat(64),backendSha256:env.DOC116_BACKEND_SHA256};
    const output=options.redactOutput('PRIVATE_COOKIE_SENTINEL\nDOC116_RESULT='+JSON.stringify(value));assert.ok(!output.includes('PRIVATE_COOKIE_SENTINEL'));return {code:0};
   }
  });assert.equal(result.caseComplete,true);assert.ok(!Object.keys(childEnvironment).some(k=>/^DOC11\d_/.test(k)));
 }finally{fs.unlinkSync(file);fs.unlinkSync(backend);fs.rmdirSync(path.dirname(backend));fs.rmdirSync(dir);}
});
