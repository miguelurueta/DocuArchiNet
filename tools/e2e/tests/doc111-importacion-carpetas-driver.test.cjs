'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),path=require('node:path'),fs=require('node:fs'),crypto=require('node:crypto');
const {MODES,FILE_NAME,validateProfile,readFixture,assertAuthorizations,classifyLoginMessage,classifyLoginPostback,openSession,createDriver,createSelectControls,runRec,runResume,runReconciliationResume,runCompletedVerification}=require('./support/doc111-importacion-carpetas.cjs');
const runner=require('../scripts/run-doc111-importacion-carpetas.cjs');
const examplePath=path.resolve(__dirname,'../profiles/doc111-importacion-carpetas.profile.example.json');
const profile=()=>structuredClone(JSON.parse(fs.readFileSync(examplePath,'utf8')));

test('DOC114: preparacion acotada conserva referencia, rechaza efectos y no reenvia incertidumbre',async()=>{
  const plan=require('./support/doc114-plan-importacion.cjs'),p=profile();
  const scope={scenario:'PLAN-01',environment:true,account:true,select:true,effects:true,discardable:true,tls:true,maximumPreparations:1};
  assert.throws(()=>plan.authorize(p,{...scope,select:false}),/AUTHORIZATION/);
  const calls=[],refs=[];let fail=false;
  const auth={page:{locator:()=>({getAttribute:async()=>'synthetic-token'})},context:{request:{post:async(url,options)=>{calls.push({url,options});if(fail)throw Error('PRIVATE_COOKIE_SENTINEL');return {status:()=>200,json:async()=>({d:{ok:true,datos:{planId:'100'}}})};}}}};
  const client=plan.createPreparationClient(auth,p,scope,r=>refs.push(r));await client.refreshToken();
  for(const op of ['ConfirmarPlan','AvanzarLote','RecibirArchivo','ReconciliarRecurso','PublicarVersion'])await assert.rejects(client.request(op,{nivelRef:p.nivelRef}),/OPERATION_NOT_ALLOWED/);
  await assert.rejects(client.request('PrepararPlan',{nivelRef:'999',versionId:p.versionId}),/LEVEL_NOT_AUTHORIZED/);
  await client.request('PrepararPlan',{nivelRef:p.nivelRef,versionId:p.versionId,raices:[],archivos:[]});
  assert.deepEqual(refs,[{planId:'100'}]);assert.equal(calls.length,1);assert.equal(calls[0].options.maxRetries,0);assert.equal(calls[0].options.maxRedirects,0);
  await assert.rejects(client.request('PrepararPlan',{nivelRef:p.nivelRef,versionId:p.versionId}),/BUDGET_EXHAUSTED/);
  const uncertain=plan.createPreparationClient(auth,p,{...scope,maximumPreparations:2});await uncertain.refreshToken();fail=true;
  await assert.rejects(uncertain.request('PrepararPlan',{nivelRef:p.nivelRef,versionId:p.versionId}),e=>e.message==='DOC114_RESULT_NOT_CONFIRMED');
  assert.equal(uncertain.uncertain,true);await assert.rejects(uncertain.request('PrepararPlan',{nivelRef:p.nivelRef,versionId:p.versionId}),/UNCERTAIN_OPERATION/);assert.equal(calls.length,2);
  fail=false;const negative=plan.createPreparationClient(auth,p,{...scope,maximumPreparations:2});await negative.refreshToken();
  await assert.rejects(negative.request('PrepararPlan',{nivelRef:p.nivelRef,versionId:p.versionId},['SOLICITUD_INVALIDA']),/EXPECTED_REJECTION/);
  assert.equal(negative.uncertain,true);await assert.rejects(negative.request('PrepararPlan',{nivelRef:p.nivelRef,versionId:p.versionId}),/UNCERTAIN_OPERATION/);assert.equal(calls.length,3);
});

function doc114Result(){
  const p=profile(),request={nivelRef:p.nivelRef,versionId:p.versionId,raices:[{idCliente:'r1',seleccionId:'s1',nombre:'000114'}],archivos:[{idCliente:'a1',raizIdCliente:'r1',nombre:'doc.pdf',rutaRelativa:'000114/doc.pdf',bytes:'7',ultimaModificacionLocal:'0'}]};
  const data={planId:'100',revision:1,planHash:'a'.repeat(64),expiraUtc:'2030-01-01T00:01:00Z',nivelRef:p.nivelRef,snapshot:{VersionId:p.versionId},resultado:{PuedeConfirmar:true,UnidadesElegibles:['r1'],Incidencias:[],Elementos:[{Ambito:'RAIZ',RaizIdCliente:'r1',Valores:[{DestinoTipo:'CAMPO_EXPEDIENTE',DestinoRef:'CODIGO',Valor:'000114',Diferido:false}]},{Ambito:'ARCHIVO',RaizIdCliente:'r1',ArchivoIdCliente:'a1',Valores:[{DestinoTipo:'INDICE_GABINETE',DestinoRef:'99',Valor:null,Diferido:true,PropiedadPendiente:'ID'}]}]}};
  return {p,request,data};
}

test('DOC114: lector de publicaciones bloquea preparacion sin solicitar efectos o cuenta SQL',async()=>{
  const {createPreparationClient}=require('./support/doc114-plan-importacion.cjs'),p=profile(),calls=[];
  const auth={page:{locator:()=>({getAttribute:async()=>'synthetic-token'})},context:{request:{post:async(url)=>{calls.push(url);return {status:()=>200,json:async()=>({ok:true,datos:{perfiles:[]}})};}}}};
  const reader=createPreparationClient(auth,p,{environment:true,account:true,tls:true,effects:false,discardable:false},()=>{},{readOnly:true});await reader.refreshToken();
  await reader.request('AbrirImportacion',{nivelRef:p.nivelRef});
  await assert.rejects(reader.request('PrepararPlan',{nivelRef:p.nivelRef,versionId:p.versionId}),/READONLY_OPERATION_REQUIRED/);
  assert.equal(calls.length,1);assert.equal(reader.attempts,0);
});

function doc114Session(scenario){
  const f=doc114Result(),p={...f.p,mode:'EXPEDIENTE'},fixtures=require('../fixtures/doc114-plan-cases.json');
  const profiles=[p,{...structuredClone(p),versionId:'106'},{...structuredClone(p),versionId:'107',mode:'LOTE'},{...structuredClone(p),versionId:'108',mode:'PARCIAL'}];
  if(scenario==='PLAN-03')profiles.forEach(p=>p.baseUrl='https://localhost/GestionDocumental-Docuarchi.net/');
  const session={version:1,scenario,identity:{userRef:'1',ambitoRef:'2'},profiles,cases:[],foreignLevel:'999',invalidVersion:'998'};
  session.runtimeRoot=path.resolve(__dirname,'../../..');
  const make=(key,index=0)=>({key,profile:index,request:{...structuredClone(f.request),versionId:profiles[index].versionId},expected:{confirmable:true,eligible:['r1'],incidences:[],values:[]}});
  if(scenario==='PLAN-01'){
    session.cases=fixtures.plan01.map(c=>make(c.key,c.key==='valid-format-b'?1:0));
    session.cases.slice(0,2).forEach((c,i)=>{const format=fixtures.formats[i];c.request.raices[0].nombre=format.root;c.request.archivos[0].rutaRelativa=format.root+'/doc.pdf';c.expected.values=[{root:'r1',type:'CAMPO_EXPEDIENTE',ref:'CODIGO',value:format.expectedCode,deferred:false},{root:'r1',type:'CAMPO_EXPEDIENTE',ref:'FECHA_INICIAL',value:format.expectedDate,deferred:false},{root:'r1',file:'a1',type:'INDICE_GABINETE',ref:'99',value:null,deferred:true}];});
    for(const c of session.cases.slice(2)){c.expected.incidences=[{root:'r1',file:'a1',code:fixtures.plan01.find(r=>r.key===c.key).incidence}];c.expected.confirmable=false;c.expected.eligible=[];}
  }else if(scenario==='PLAN-02'){
    session.cases=fixtures.plan02.map((row,i)=>{const c=make(row.key,i===1?2:i===2||i===4?3:0);c.expected.confirmable=row.confirmable;c.expected.eligible=row.eligible;c.expected.incidences=[{root:'r2',file:'a2',code:'SEGMENTO_AUSENTE'}];c.request.raices.push({idCliente:'r2',seleccionId:'s2',nombre:'Otro'});c.request.archivos.push({...structuredClone(c.request.archivos[0]),raizIdCliente:'r2',idCliente:'a2',rutaRelativa:'Otro/doc.pdf'});if(i===3||row.key==='foreign-level'){delete c.expected;c.rejectCodes=['ACCESO_DENEGADO'];c.request.nivelRef=session.foreignLevel;}if(row.key==='inconsistent-manifest'){delete c.expected;c.rejectCodes=['SOLICITUD_INVALIDA'];c.request.archivos[0].raizIdCliente='absent';}if(i===4)c.request.archivos.push({...structuredClone(c.request.archivos[1]),idCliente:'a3',nombre:'otro.pdf',rutaRelativa:'Otro/otro.pdf'});return c;});
  }else{
    session.cases=[make('baseline'),...fixtures.plan03.map(row=>{const c=make(row.key,row.key==='compatible-version-control'?1:0);if(row.key!=='compatible-version-control'){delete c.expected;c.rejectCodes=[row.expectedCode||'ACCESO_DENEGADO'];}if(row.extraField)c.request[row.extraField]={};if(row.key==='foreign-level')c.request.nivelRef=session.foreignLevel;if(row.key==='incompatible-version')c.request.versionId=session.invalidVersion;return c;})];
  }
  return session;
}

test('DOC114: recursos cerrados exigen cobertura completa y perfiles base compatibles',()=>{
  const {validateSession}=require('./support/doc114-plan-importacion.cjs');
  for(const scenario of ['PLAN-01','PLAN-02','PLAN-03']){
    const s=doc114Session(scenario);assert.equal(validateSession(s),s);
    for(const change of [x=>x.password='PRIVATE',x=>x.profiles[0].extra=true,x=>x.cases.pop(),x=>x.cases[0].profile=99]){const bad=structuredClone(s);change(bad);assert.throws(()=>validateSession(bad),/DOC11/);}
  }
  const same=doc114Session('PLAN-01');same.profiles[1].versionId=same.profiles[0].versionId;same.cases[1].request.versionId=same.profiles[0].versionId;assert.throws(()=>validateSession(same),/TWO_PUBLISHED/);
});

function doc114SyntheticRuntime(session){
  const plans=[],events=[],refs=[];let index=0,revoked=false,failWhileRevoked=false;
  const auth={page:{locator:()=>({getAttribute:async()=>'synthetic-token'})},context:{request:{post:async(url,options)=>{
    if(url.endsWith('/AbrirImportacion')){assert.equal(revoked,false);events.push('READ_RESTORED');return {status:()=>200,json:async()=>({ok:true,datos:{nivelRef:session.profiles[0].nivelRef}})};}
    const c=session.cases[index++];assert.deepEqual(options.data.solicitud,c.request);assert.equal(options.maxRetries,0);
    if(c.key==='revoked-permission'){assert.equal(revoked,true);if(failWhileRevoked)throw Error('PRIVATE_COOKIE');}
    if(c.rejectCodes)return {status:()=>c.rejectCodes[0]==='ACCESO_DENEGADO'?403:400,json:async()=>({ok:false,codigo:c.rejectCodes[0],datos:null})};
    const p=session.profiles[c.profile],data={planId:String(100+plans.length),revision:1,planHash:'a'.repeat(64),nivelRef:p.nivelRef,expiraUtc:'2030-01-01T00:01:00Z',snapshot:{VersionId:p.versionId,Configuracion:{ModoIntegridad:p.mode==='PARCIAL'?'PARCIAL_PERMITIDO':'COMPLETO_OBLIGATORIO',AmbitoIntegridad:p.mode==='PARCIAL'?'NO_APLICA':p.mode}},resultado:{PuedeConfirmar:c.expected.confirmable,UnidadesElegibles:c.expected.eligible,Incidencias:c.expected.incidences.map(i=>({Codigo:i.code,RaizIdCliente:i.root,ArchivoIdCliente:i.file})),Elementos:[]}};
    for(const r of c.request.raices)data.resultado.Elementos.push({Ambito:'RAIZ',RaizIdCliente:r.idCliente,Valores:[]});
    for(const a of c.request.archivos)data.resultado.Elementos.push({Ambito:'ARCHIVO',RaizIdCliente:a.raizIdCliente,ArchivoIdCliente:a.idCliente,Valores:[]});
    for(const v of c.expected.values)data.resultado.Elementos.find(e=>e.RaizIdCliente===v.root&&(v.file?e.ArchivoIdCliente===v.file:e.Ambito==='RAIZ')).Valores.push({DestinoTipo:v.type,DestinoRef:v.ref,Valor:v.value,Diferido:v.deferred,PropiedadPendiente:v.deferred?'ID':null});
    const manifest={Raices:c.request.raices.map(r=>({SeleccionId:r.seleccionId,IdCliente:r.idCliente,Nombre:r.nombre})),Archivos:c.request.archivos.map(a=>({IdCliente:a.idCliente,RaizIdCliente:a.raizIdCliente,Nombre:a.nombre,RutaRelativa:a.rutaRelativa,Bytes:a.bytes,UltimaModificacionLocal:a.ultimaModificacionLocal}))};
    plans.push({data,profile:p,row:{estado:data.resultado.PuedeConfirmar?'REVISABLE':'BLOQUEADO',revision:1,huella:data.planHash,contexto_doc:JSON.stringify({UsuarioRef:session.identity.userRef,AmbitoRef:session.identity.ambitoRef,NivelRef:p.nivelRef}),config_doc:JSON.stringify(data.snapshot),manifiesto_doc:JSON.stringify(manifest),resultado_doc:JSON.stringify(data.resultado),creado_utc:'2030-01-01T00:00:00Z',expira_utc:data.expiraUtc}});
    return {status:()=>200,json:async()=>({ok:true,datos:data})};
  }}}};
  const query=async(origin,sql,args)=>{
    assert.match(sql,/^SELECT /);
    if(sql.includes('SELECT gabinete_id'))return [{gabinete_id:10}];if(sql.includes('SELECT NOMBRE'))return [{NOMBRE:'gabinete_test'}];
    if(sql.includes('COUNT(*)'))return [{total:String(sql.includes('FROM ra_integracion_plan')?plans.filter(x=>x.profile.versionId===args[3]).length:0)}];
    return plans.filter(p=>p.data.planId===args[0]).map(p=>p.row);
  };
  const startBridge=()=>({ready:async()=>({ok:true,stage:'READY',originalGranted:true,permissionColumn:'IMPORTAR_CARPETAS',userRef:session.identity.userRef,companyRef:session.identity.ambitoRef}),send:async op=>{events.push(op);revoked=op==='REVOKE';return {ok:true,stage:op==='REVOKE'?'REVOKED':'RESTORED',otherFieldsUnchanged:true};},close:async()=>{events.push('CLOSE');}});
  const authorization={environment:true,account:true,select:true,effects:true,discardable:true,tls:true,revocation:true};
  return {auth,query,authorization,refs,events,startBridge,plans,get postCount(){return index;},seedFirst:()=>auth.context.request.post('https://synthetic.invalid/PrepararPlan',{data:{solicitud:session.cases[0].request},maxRetries:0}),get revoked(){return revoked;},failOnRevocation(){failWhileRevoked=true;}};
}
test('DOC114: reanuda primer plan por lectura y prepara solo seis casos restantes',async()=>{
  const {runPlanSession}=require('./support/doc114-plan-importacion.cjs'),s=doc114Session('PLAN-01'),f=doc114SyntheticRuntime(s);
  await f.seedFirst();const preserved=structuredClone(f.plans[0]);
  const result=await runPlanSession(f.auth,s,f.authorization,f.query,r=>f.refs.push(r),{resumePlan:{caseKey:'valid-format-a',planId:'100',planHash:'a'.repeat(64)}});
  assert.equal(result.attempts,6);assert.equal(result.preparations,6);assert.equal(result.resumedPlans,1);assert.equal(f.postCount,7);assert.equal(f.refs.length,7);
  assert.deepEqual(f.plans[0],preserved);assert.equal(f.plans.length,7);
});
test('DOC114 PLAN-03: conserva baseline y ejecuta seis solicitudes con una nueva preparacion y restauracion',async()=>{
  const {runPlanSession}=require('./support/doc114-plan-importacion.cjs'),s=doc114Session('PLAN-03'),f=doc114SyntheticRuntime(s);await f.seedFirst();const preserved=structuredClone(f.plans[0]),proofs=[];
  const result=await runPlanSession(f.auth,s,f.authorization,f.query,()=>{},{resumePlan:{caseKey:'baseline',planId:'100',planHash:'a'.repeat(64)},startBridge:f.startBridge,onPlanVerified:p=>proofs.push(p)});
  assert.equal(result.attempts,6);assert.equal(result.preparations,1);assert.equal(result.rejections,5);assert.equal(result.resumedPlans,1);assert.equal(result.restorationVerified,true);assert.equal(f.postCount,7);assert.deepEqual(f.plans[0],preserved);assert.equal(proofs.length,2);assert.equal(f.revoked,false);
});
test('DOC114: rechazo discordante conserva solo HTTP/codigo/forma sin cuerpo privado',async()=>{
 const {createPreparationClient,safePlanObservation}=require('./support/doc114-plan-importacion.cjs'),p=profile();
 const auth={page:{locator:()=>({getAttribute:async()=>'private-token'})},context:{request:{post:async()=>({status:()=>400,json:async()=>({ok:false,codigo:'CONFIGURACION_INVALIDA',datos:null,mensaje:'private server details'})})}}};
 const client=createPreparationClient(auth,p,{environment:true,account:true,select:true,effects:true,discardable:true,tls:true,scenario:'PLAN-03',maximumPreparations:1});await client.refreshToken();
 await assert.rejects(client.request('PrepararPlan',{nivelRef:p.nivelRef,versionId:p.versionId},['SOLICITUD_INVALIDA']),e=>{assert.deepEqual(e.observation,{operation:'PrepararPlan',httpStatus:400,code:'CONFIGURACION_INVALIDA',domainEnvelope:true,hasData:false});assert.equal(JSON.stringify(e).includes('private'),false);return true;});
 assert.equal(safePlanObservation({operation:'PrepararPlan',httpStatus:400,code:'PRIVATE',domainEnvelope:false,hasData:false}).code,null);
});
test('DOC114: huella de plan reanudado cambiada detiene antes de nuevos POST',async()=>{
  const {runPlanSession,validateResumePlan}=require('./support/doc114-plan-importacion.cjs'),s=doc114Session('PLAN-01'),f=doc114SyntheticRuntime(s);await f.seedFirst();
  await assert.rejects(runPlanSession(f.auth,s,f.authorization,f.query,()=>{},{resumePlan:{caseKey:'valid-format-a',planId:'100',planHash:'b'.repeat(64)}}),/RESUME_PLAN_HASH_CHANGED/);
  assert.equal(f.postCount,1);assert.equal(f.plans.length,1);
  for(const resume of [{caseKey:'valid-format-b',planId:'100',planHash:'a'.repeat(64)},{caseKey:'valid-format-a',planId:'100',planHash:'a'.repeat(64),extra:true}])assert.throws(()=>validateResumePlan(s,resume),/RESUME_SCOPE_INVALID/);
  assert.throws(()=>validateResumePlan(doc114Session('PLAN-02'),{caseKey:'valid-format-a',planId:'100',planHash:'a'.repeat(64)}),/RESUME_SCOPE_INVALID/);
});
test('DOC114: selector reanudado limita a seis preparaciones y transmite referencia sin secretos persistidos',async()=>{
  const selector=require('../scripts/run-doc114-plan-importacion.cjs'),s=doc114Session('PLAN-01'),file=path.join(require('node:os').tmpdir(),'doc114-resume-'+crypto.randomBytes(8).toString('hex')+'.json');fs.writeFileSync(file,JSON.stringify(s));
  const resumePlan={caseKey:'valid-format-a',planId:'100',planHash:'a'.repeat(64)};let childEnv;
  try{await selector.main(['--session',file,'--scenario','PLAN-01'],{resumePlan,requireInteractiveConsole(){},backendFingerprint:()=> 'c'.repeat(64),collectConfirmation:async(v,k)=>{v[k]='true';},collectValue:async(v,k)=>{v[k]='synthetic-private-input';},runChild:async(command,args,cwd,env,options)=>{
    childEnv=env;assert.deepEqual(JSON.parse(env.DOC114_RESUME_PLAN),resumePlan);
    options.redactOutput('DOC114_PLAN_RESULT='+JSON.stringify({scenario:'PLAN-01',status:'PASS',attempts:6,preparations:6,rejections:0,resumedPlans:1,confirmations:0,noDocumentEffects:true,readOnlyControls:true,restorationVerified:null,head:'a'.repeat(40),codeSha256:'b'.repeat(64),backendSha256:'c'.repeat(64)}));return {code:0};
  }});assert.equal(childEnv.DOC111_E2E_PASSWORD,undefined);assert.equal(childEnv.DOC114_RESUME_PLAN,undefined);}finally{fs.unlinkSync(file);}
});
for(const scenario of ['PLAN-01','PLAN-02','PLAN-03'])test('DOC114 '+scenario+': orquestacion local con HTTP/SELECT dobles, no es E2E real',async()=>{
  const {runPlanSession}=require('./support/doc114-plan-importacion.cjs'),s=doc114Session(scenario),f=doc114SyntheticRuntime(s);
  const result=await runPlanSession(f.auth,s,f.authorization,f.query,r=>f.refs.push(r),{startBridge:f.startBridge});
  assert.equal(result.status,'PASS');assert.equal(result.attempts,s.cases.length);assert.equal(result.confirmations,0);assert.equal(f.refs.length,result.preparations);
  if(scenario==='PLAN-03'){assert.equal(result.restorationVerified,true);assert.deepEqual(f.events,['REVOKE','RESTORE','CLOSE','READ_RESTORED']);}
});
test('DOC114: error en sesion revocada restaura y cierra antes de propagar fallo saneado',async()=>{
  const {runPlanSession}=require('./support/doc114-plan-importacion.cjs'),s=doc114Session('PLAN-03'),f=doc114SyntheticRuntime(s),progress=[];f.failOnRevocation();
  await assert.rejects(runPlanSession(f.auth,s,f.authorization,f.query,()=>{},{startBridge:f.startBridge,onProgress:p=>progress.push(p.phase)}),e=>e.message==='DOC114_RESULT_NOT_CONFIRMED');
  assert.equal(f.revoked,false);assert.deepEqual(f.events,['REVOKE','RESTORE','CLOSE']);
  assert.deepEqual(progress.filter(p=>p.startsWith('PERMISSION_')),['PERMISSION_READY','PERMISSION_REVOKING','PERMISSION_REVOKED','PERMISSION_RESTORING','PERMISSION_RESTORED']);
});

test('DOC114: selector delega infraestructura, elimina secretos efimeros y sanea solo resultados cerrados',async()=>{
  const selector=require('../scripts/run-doc114-plan-importacion.cjs'),s=doc114Session('PLAN-01'),file=path.join(require('node:os').tmpdir(),'doc114-selector-'+crypto.randomBytes(8).toString('hex')+'.json');fs.writeFileSync(file,JSON.stringify(s));
  try{
    for(const args of [[],['--session','x','--scenario','REC'],['--session','x','--scenario','PLAN-01','--confirm'],['--session','x','--scenario','PLAN-01','--password','PRIVATE']])assert.throws(()=>selector.parseArgs(args),/DOC114_/);
    let spawned=0,childEnv;const prompts=[];
    await selector.main(['--session',file,'--scenario','PLAN-01','--check'],{requireInteractiveConsole(){throw Error('NO_TTY_ALLOWED');}});
    await selector.main(['--session',file,'--scenario','PLAN-01'],{requireInteractiveConsole(){},backendFingerprint:()=> 'c'.repeat(64),collectConfirmation:async(v,k)=>{prompts.push(k);v[k]='true';},collectValue:async(v,k)=>{v[k]='synthetic-private-input';},runChild:async(command,args,cwd,env,options)=>{
      spawned++;childEnv=env;assert.ok(args.includes('tests/doc114-plan-importacion.spec.cjs'));assert.ok(args.includes('--retries=0'));assert.equal(env.DOC114_AUTH_REVOCATION,undefined);assert.equal(env.DOC111_E2E_PASSWORD,'synthetic-private-input');
      const result={scenario:'PLAN-01',status:'PASS',attempts:s.cases.length,preparations:s.cases.length,rejections:0,confirmations:0,noDocumentEffects:true,readOnlyControls:true,restorationVerified:null,head:'a'.repeat(40),codeSha256:'b'.repeat(64),backendSha256:'c'.repeat(64),cookie:'PRIVATE'};
      const text=options.redactOutput('raw COOKIE=PRIVATE\nDOC114_PLAN_RESULT='+JSON.stringify(result));assert.ok(text.includes('DOC114_PLAN_RESULT'));assert.ok(!text.includes('PRIVATE'));return {code:0};
    }});
    assert.equal(spawned,1);assert.ok(prompts.includes('effects'));assert.ok(!prompts.includes('revocation'));assert.equal(childEnv.DOC111_E2E_PASSWORD,undefined);assert.equal(childEnv.DOC114_SESSION,undefined);
    assert.equal(selector.safeLine('DOC114_PLAN_REFERENCE='+JSON.stringify({scenario:'PLAN-01',planId:'100',secret:'PRIVATE'}),'PLAN-01').value.planId,'100');
    assert.equal(selector.safeLine('DOC114_PLAN_FAILURE='+JSON.stringify({scenario:'PLAN-01',stage:'PLAN',code:'DOC114_BAD PRIVATE'}),'PLAN-01'),null);
  }finally{fs.unlinkSync(file);}
});

test('DOC114: puente compartido exige autorizacion operacional antes de abrir procesos y conserva restauracion local',()=>{
  const {startDatabaseBridge}=require('./support/doc110-permission-revocation.cjs');
  const keys=['DOC114_AUTH_REVOCATION','DOC114_E2E_USER'],prior=Object.fromEntries(keys.map(k=>[k,process.env[k]]));
  try{for(const k of keys)delete process.env[k];assert.throws(()=>startDatabaseBridge('doc110-permission-db.ps1',{operationalImport:true}),/AUTHORIZATION_REQUIRED/);assert.throws(()=>startDatabaseBridge('doc110-profile-schema-db.ps1',{operationalImport:true}),/SCRIPT_INVALID/);}
  finally{for(const k of keys)if(prior[k]===undefined)delete process.env[k];else process.env[k]=prior[k];}
  const os=require('node:os'),cp=require('node:child_process'),parent=path.resolve(os.tmpdir()),temp=fs.mkdtempSync(path.join(parent,'doc114-permission-'));
  try{
    const output=cp.execFileSync('powershell.exe',['-NoProfile','-NonInteractive','-ExecutionPolicy','Bypass','-File',path.join(__dirname,'doc114-permission-bridge-local.ps1'),'-BridgePath',path.resolve(__dirname,'../scripts/support/doc110-permission-db.ps1'),'-FixtureDirectory',temp],{encoding:'utf8',stdio:['ignore','pipe','pipe']});
    assert.match(output,/DOC114_PERMISSION_BRIDGE_LOCAL=PASS; NO_DATABASE=true/);
  }finally{assert.ok(path.resolve(temp).startsWith(parent+path.sep));fs.rmSync(temp,{recursive:true,force:true});}
});
test('DOC114: aserciones conservan inventario, ceros, diferidos y detectan cambios efectivos',()=>{
  const {assertPreparation}=require('./support/doc114-plan-importacion.cjs'),f=doc114Result();
  const expected={confirmable:true,eligible:['r1'],values:[{root:'r1',type:'CAMPO_EXPEDIENTE',ref:'CODIGO',value:'000114',deferred:false},{root:'r1',file:'a1',type:'INDICE_GABINETE',ref:'99',value:null,deferred:true}]};
  const hash=assertPreparation(f.data,f.request,expected);assert.match(hash,/^[a-f0-9]{64}$/);
  assert.equal(assertPreparation({...f.data,planId:'101',expiraUtc:'2030-01-01T00:02:00Z'},f.request,expected),hash);
  for(const change of [d=>d.resultado.Elementos.pop(),d=>d.resultado.UnidadesElegibles.push('r2'),d=>d.resultado.Elementos[0].Valores[0].Valor='114',d=>d.resultado.Elementos[1].Valores[0].Valor='123',d=>d.resultado.Incidencias.push(null),d=>d.resultado.Elementos[0].Valores.push(null)]){
    const bad=structuredClone(f.data);change(bad);assert.throws(()=>assertPreparation(bad,f.request,expected),/DOC114_/);
  }
});

test('DOC114: controles SELECT rechazan alteracion persistida y efectos sin guardar filas',async()=>{
  const {createPlanControls}=require('./support/doc114-plan-importacion.cjs'),f=doc114Result(),queries=[];
  const manifest={Raices:[{SeleccionId:'s1',IdCliente:'r1',Nombre:'000114'}],Archivos:[{IdCliente:'a1',RaizIdCliente:'r1',Nombre:'doc.pdf',RutaRelativa:'000114/doc.pdf',Bytes:'7',UltimaModificacionLocal:'0'}]};
  let stored={estado:'REVISABLE',revision:1,huella:f.data.planHash.toUpperCase(),contexto_doc:JSON.stringify({UsuarioRef:'1',AmbitoRef:'2',NivelRef:f.p.nivelRef}),config_doc:JSON.stringify(f.data.snapshot),manifiesto_doc:JSON.stringify(manifest),resultado_doc:JSON.stringify(f.data.resultado),creado_utc:'2030-01-01T00:00:00Z',expira_utc:f.data.expiraUtc};
  const query=async(origin,sql,args)=>{assert.match(sql,/^SELECT /);queries.push({origin,sql,args});if(sql.includes('SELECT gabinete_id'))return [{gabinete_id:10}];if(sql.includes('SELECT NOMBRE'))return [{NOMBRE:'gabinete_test'}];if(sql.includes('COUNT(*)'))return [{total:'0'}];return [stored];};
  const controls=createPlanControls(query,f.p,{userRef:'1',ambitoRef:'2'});const before=await controls.snapshot();assert.equal(controls.unchanged(before,{...before,plans:'1'},1),true);
  assert.throws(()=>controls.unchanged(before,{...before,documents:'1'},0),/DOCUMENT_EFFECT/);
  assert.deepEqual(await controls.verifyPlan(f.data,f.request),{verified:true});
  assert.ok(queries.some(q=>q.sql.includes('solicitante_ref=?')));assert.ok(queries.at(-1).sql.includes('usuario_ref=? AND ambito_ref=?'));assert.deepEqual(queries.at(-1).args,['100','1','2',f.p.nivelRef,f.p.versionId]);
  stored={...stored,resultado_doc:JSON.stringify({...f.data.resultado,PuedeConfirmar:false})};await assert.rejects(controls.verifyPlan(f.data,f.request),/STORED_RESULT_CHANGED/);
  stored={...stored,resultado_doc:JSON.stringify(f.data.resultado),expira_utc:'2030-01-02T00:01:00Z'};await assert.rejects(controls.verifyPlan(f.data,f.request),/STORED_EXPIRY_INVALID/);
});

test('DOC113: selector y alcance UI mantienen cerrado el perfil y las mutaciones',()=>{
  const ui=require('./support/doc113-ventana-importacion.cjs'),selector=require('../scripts/run-doc113-ventana-importacion.cjs');
  const p=profile(),options={scenario:'UI-01',alternateLevel:'8',alternateVersion:'6',confirm:false};
  assert.equal(ui.validateOptions(p,options),options);
  for(const args of [[],['--profile','x','--scenario','LOTE'],['--profile','x','--scenario','UI-01','--confirm','--confirm'],['--profile','x','--scenario','UI-01','--password','private']])assert.throws(()=>selector.parseArgs(args),/DOC113_/);
  assert.throws(()=>ui.validateOptions({...p,unknown:true},options),/CONTRACT_INVALID/);
  assert.throws(()=>ui.validateOptions(p,{...options,alternateLevel:p.nivelRef}),/ALTERNATE_LEVEL/);
  assert.throws(()=>ui.validateOptions(p,{...options,confirm:true}),/CONFIRM_SCOPE/);
  assert.throws(()=>ui.assertAuthorization(options,{environment:true,account:true},p),/AUTHORIZATION/);
  assert.throws(()=>ui.assertAuthorization({...options,scenario:'UI-02'},{environment:true,account:true,select:true},p),/EFFECTS_AUTHORIZATION/);
  assert.throws(()=>ui.assertAuthorization({...options,scenario:'UI-03'},{environment:true,account:true,select:true},p),/PROBE_AUTHORIZATION/);
  for(const operation of ['AvanzarLote','RecibirArchivo','CerrarReservaSinInicio','ReconciliarRecurso','PublicarVersion'])assert.equal(ui.allowedOperation(operation,{scenario:'UI-02',confirm:true}),false);
  assert.equal(ui.allowedOperation('ConfirmarPlan',{scenario:'UI-02',confirm:false}),false);
  assert.equal(ui.allowedOperation('PrepararPlan',options),false);
});

test('DOC113: fixtures reutilizan PDF válido y conservan dos raíces homónimas',()=>{
  const {createFixtures}=require('./support/doc113-ventana-importacion.cjs');
  const parent=path.resolve(__dirname,'../../../.tmp');fs.mkdirSync(parent,{recursive:true});
  const fixtures=createFixtures(parent,profile());
  try{
    assert.equal(path.basename(fixtures.dirs[0]),path.basename(fixtures.dirs[1]));
    assert.notEqual(path.basename(fixtures.dirs[0]),path.basename(fixtures.dirs[2]));
    for(const dir of fixtures.dirs)assert.deepEqual(fs.readFileSync(path.join(dir,'Subcarpeta',FILE_NAME)),readFixture(profile()));
  }finally{fixtures.close();}
  assert.ok(fixtures.dirs.every(dir=>!fs.existsSync(dir)));
});

test('DOC113: selector delega consola/proceso y sanea resultados sin heredar autorizaciones',async()=>{
  const selector=require('../scripts/run-doc113-ventana-importacion.cjs');let childEnv,childArgs,tty=0,values=0;
  const pass={scenario:'UI-01',status:'PASS',preparations:0,confirmations:0,opens:1,readOnlyControls:true,noDocumentEffects:true,domVerified:true,head:'a'.repeat(40),codeSha256:'b'.repeat(64),servedScriptSha256:'c'.repeat(64)};
  await selector.main(['--profile',examplePath,'--scenario','UI-01','--alternate-level','8','--alternate-version','6'],{
    requireInteractiveConsole(){tty++;},collectConfirmation:async(object,key)=>object[key]='true',collectValue:async(object,key)=>{values++;object[key]='PRIVATE_SYNTHETIC';},
    runChild:async(_command,args,_cwd,env,options)=>{
      childEnv=env;childArgs=args;assert.equal(env.DOC111_AUTH_EFFECTS,undefined);assert.equal(env.DOC113_AUTH_EFFECTS,undefined);assert.equal(options.nonInteractiveChild,true);
      const sanitized=options.redactOutput('Cookie: PRIVATE_SYNTHETIC\nDOC113_UI_RESULT='+JSON.stringify({...pass,password:'PRIVATE_SYNTHETIC'})+'\n');
      assert.ok(!sanitized.includes('PRIVATE'));assert.ok(sanitized.startsWith('DOC113_UI_RESULT='));return {code:0};
    }
  });
  assert.equal(tty,1);assert.equal(values,4);assert.ok(childArgs.includes('tests/doc113-ventana-importacion.spec.cjs'));
  assert.equal(Object.keys(childEnv).filter(key=>/^DOC11[13]_/.test(key)).length,0);
  assert.equal(selector.safeLine('DOC113_UI_RESULT='+JSON.stringify({...pass,opens:2}),'UI-01'),null);
  assert.equal(selector.safeLine('DOC113_UI_FAILURE='+JSON.stringify({scenario:'UI-01',stage:'LOGIN',code:'PRIVATE password'}),'UI-01'),null);
  const reference=selector.safeLine('DOC113_UI_REFERENCE='+JSON.stringify({scenario:'UI-02',planId:'5',cookie:'PRIVATE'}),'UI-02');assert.deepEqual(reference.value,{scenario:'UI-02',planId:'5'});
});

test('DOC113: controles SELECT acotados no certifican recepción o efectos presentes',async()=>{
  const {createUiControls}=require('./support/doc113-ventana-importacion.cjs');
  let received=false;const calls=[];
  const query=async(origin,sql,args)=>{
    calls.push({origin,sql,args});assert.ok(sql.startsWith('SELECT '));
    if(sql.includes('FROM ra_integracion_perfil_version'))return [{gabinete_id:10}];
    if(sql.includes('FROM system1'))return [{id_gabinete:10}];
    if(sql.startsWith('SELECT COUNT'))return [{total:'0'}];
    if(sql.startsWith('SELECT estado'))return [{estado:'REVISABLE',nivel_ref:'7',version_id:'5'}];
    if(sql.includes('FROM ra_integracion_lote'))return [{config_doc:'{}'}];
    if(sql.includes('FROM ra_integracion_raiz WHERE'))return [{identidad_expediente_doc:null,intento_activo_id:null}];
    if(sql.includes('FROM ra_integracion_archivo f'))return [{resultado_doc:null,intento_activo_id:null,estado_recepcion:received?'RECIBIDO':'PENDIENTE'}];
    return [];
  };
  const c=createUiControls(query,profile());assert.deepEqual(await c.precheck(),{plan:0n,lote:0n});await c.plan('5',false);await c.noEffects('5');received=true;await assert.rejects(c.noEffects('5'),/DOCUMENT_EFFECT_FOUND/);assert.ok(calls.some(c=>c.origin==='DA'));
  const {captureCodeEvidence}=require('./support/doc110-code-evidence.cjs');
  const proof=captureCodeEvidence({files:['js/gestion/ImportacionCarpetas/ImportacionCarpetas.js']});assert.equal(proof.files.length,1);assert.match(proof.scopeSha256,/^[a-f0-9]{64}$/);assert.throws(()=>captureCodeEvidence({files:['../private']}),/EVIDENCE_PATH_INVALID/);
});

test('DOC111: diagnostico de login convierte solo mensajes reconocidos a codigos sin conservar texto privado',()=>{
  for(const [message,code] of [['La contraseña  no es valida para el módulo GESTOR DOCUMENTAL','LOGIN_CREDENTIALS_REJECTED'],['El usuario  no coincide con el módulo GESTOR DOCUMENTAL','LOGIN_ACCOUNT_NOT_FOUND'],['El usuario esta bloqueado en la aplicación GESTOR DOCUMENTAL','LOGIN_ACCOUNT_BLOCKED'],['LOGIN_UNAVAILABLE','LOGIN_SERVICE_REJECTED']]){
    assert.equal(classifyLoginMessage(message),code);assert.equal(classifyLoginPostback("<script type=\"text/javascript\"> alert('"+message+"'); </script>"),code);
  }
  assert.equal(classifyLoginMessage('SQL PRIVATE_PASSWORD'),null);
  assert.equal(classifyLoginPostback('<script src="DocuArchiSegundoFactor.js"></script><input value="PRIVATE_PASSWORD">'),null);
  assert.equal(classifyLoginPostback('if(window.DocuArchiSegundoFactor){window.DocuArchiSegundoFactor.iniciar();}'),'LOGIN_SECOND_FACTOR_REQUIRED');
});
test('DOC111: espera postback y clasifica rechazo antes de navegar, sin reintentar login',async()=>{
  const authorization={environment:true,account:true};
  for(const outcome of ['password','second-factor','unknown-dialog','unavailable','body-unavailable','http-error','redirect','success']){
    let sessions=0,closed=0,navigations=0,finished=0;
    const auth={context:{close:async()=>closed++},page:{goto:async()=>{navigations++;assert.equal(finished,1);return {status:()=>200};},locator:s=>({count:async()=>s==='#doc111-importacion'?1:0,getAttribute:async()=>'PRIVATE_TOKEN'})}};
    const runtime={createSession:async(browser,options)=>{
      sessions++;
      const response={finished:async()=>{finished++;return outcome==='unavailable'?new Error('PRIVATE_PASSWORD'):null;},status:()=>outcome==='redirect'?302:outcome==='http-error'?500:200,text:async()=>{
        if(outcome==='redirect')throw new Error('Redirect body must not be read');
        if(outcome==='body-unavailable')throw new Error('Protocol error: No resource with given identifier PRIVATE_PASSWORD');
        return outcome==='password'?"<script> alert('La contraseña no es valida para el módulo GESTOR DOCUMENTAL'); </script>":outcome==='second-factor'?'if(window.DocuArchiSegundoFactor){window.DocuArchiSegundoFactor.iniciar();}':'<html>PRIVATE_COOKIE</html>';
      }};
      await options.onPostbackCompleted(auth.page,response);
      if(outcome==='unknown-dialog')await options.onDialogCompleted(auth.page,'PRIVATE_SQL_MESSAGE');
      return auth;
    }};
    const codes={password:'LOGIN_CREDENTIALS_REJECTED','second-factor':'LOGIN_SECOND_FACTOR_REQUIRED','unknown-dialog':'LOGIN_SERVER_REJECTED','http-error':'LOGIN_HTTP_REJECTED'};
    if(codes[outcome]){await assert.rejects(openSession({},profile(),{},authorization,runtime),new RegExp('^Error: DOC111_'+codes[outcome]+'$'));assert.equal(navigations,0);assert.equal(closed,1);}
    else{assert.equal(await openSession({},profile(),{},authorization,runtime),auth);assert.equal(navigations,1);assert.equal(closed,0);if(outcome==='body-unavailable')assert.equal(auth.loginObservation.category,'BODY_NOT_RETAINED');if(outcome==='unavailable')assert.equal(auth.loginObservation.unavailableAt,'FINISH');assert.ok(!JSON.stringify(auth.loginObservation).includes('PRIVATE'));}
    assert.equal(sessions,1);
  }
});
test('DOC111: una observacion incompleta no concede sesion cuando la consulta protegida devuelve 401',async()=>{
  const environment={DOC111_E2E_USER:'PRIVATE_USER',DOC111_E2E_PASSWORD:'PRIVATE_PASSWORD'};let closed=0,sessions=0,probes=0;
  const auth={context:{close:async()=>closed++,request:{post:async()=>{probes++;return {status:()=>401,json:async()=>({d:{ok:false,codigo:'SESION_REQUERIDA'}})};}}},page:{goto:async()=>({status:()=>200}),locator:()=>({count:async()=>0})}};
  await assert.rejects(openSession({},profile(),environment,{environment:true,account:true},{createSession:async(browser,options)=>{
    sessions++;await options.onPostbackCompleted(auth.page,{status:()=>200,request:()=>({postData:()=>new URLSearchParams({'ctl00$ContentPlacenter$TextBoxuser':environment.DOC111_E2E_USER,'ctl00$ContentPlacenter$TextBoxpasw':environment.DOC111_E2E_PASSWORD}).toString()}),finished:async()=>null,text:async()=>{throw new Error('No resource with given identifier PRIVATE_COOKIE');}});return auth;
  }}),error=>{
    assert.equal(error.message,'DOC111_SESSION_NOT_ESTABLISHED');assert.equal(error.loginObservation.postedFields,'MATCHED');assert.equal(error.loginObservation.unavailableAt,'BODY');assert.equal(error.loginObservation.probeStatus,401);
    const safe=runner.safeFailure('DOC111_REC_FAILURE='+JSON.stringify({mode:'EXPEDIENTE',stage:'LOGIN',code:error.message,loginObservation:{...error.loginObservation,password:'PRIVATE_PASSWORD'}}),'EXPEDIENTE');
    assert.ok(!JSON.stringify(safe).includes('PRIVATE'));assert.equal(safe.loginObservation.category,'BODY_NOT_RETAINED');return true;
  });assert.equal(sessions,1);assert.equal(probes,1);assert.equal(closed,1);
});

function resumeFixture(){
  const p=profile();p.mode='LOTE';const calls=[],commands=new Map();let closures=0,stores=0,reopened=0;
  const values=i=>[{DestinoTipo:'TIPOLOGIA',Valor:p.expectations[i].tipologiaId},...p.expectations[i].indices.map(v=>({DestinoTipo:'INDICE_GABINETE',DestinoRef:v.referencia,TipoValor:v.tipo,Valor:v.valor}))];
  const v={lot:{lote_id:2,estado:'EN_CURSO',revision:13,detencion_solicitada:0,total_raices:2,total_archivos:2,total_bytes:14,config_doc:JSON.stringify({VersionId:p.versionId,Configuracion:{ModoIntegridad:'COMPLETO_OBLIGATORIO',AmbitoIntegridad:'LOTE'}}),contexto_doc:JSON.stringify({NivelRef:p.nivelRef})},roots:[],files:[],attempts:[],events:[{intento_id:5,accion:'RESERVAR_INTENTO',codigo:'INTENTO_RESERVADO'}]};
  for(let i=0;i<2;i++){
    v.roots.push({raiz_id:3+i,cliente_raiz_id:'r'+(i+1),datos_doc:JSON.stringify({NombreOriginal:p.rootNames[i]}),estado:'EXPEDIENTE_CONFIRMADO',identidad_expediente_doc:JSON.stringify({NivelRef:p.nivelRef,ExpedienteId:String(648+i),RelacionNivelId:String(1073+i)})});
    v.files.push({archivo_id:3+i,raiz_id:3+i,cliente_archivo_id:'a'+(i+1),bytes_previstos:7,estado_recepcion:'RECIBIDO',estado_efecto:i?'NO_INICIADO':'EN_CURSO',intento_activo_id:i?null:5,valores_efectivos_doc:i?null:JSON.stringify(values(i)),revision:5});
    v.attempts.push({intento_id:3+i,raiz_id:3+i,fase:'CREAR_EXPEDIENTE',estado:'CONFIRMADO',fin_utc:'fixed'});
  }
  v.attempts.push({intento_id:5,archivo_id:3,raiz_id:3,fase:'ALMACENAR',estado:'RESERVADO'});
  const driver={refreshToken:async()=>{},reopen:async()=>reopened++,pages:async()=>v.files.map(f=>({recursoId:String(f.archivo_id),idCliente:f.cliente_archivo_id,revision:f.revision})),request:async(op,r)=>{
    calls.push({op,r});
    if(op==='ConsultarLote')return {loteId:'2',nivelRef:p.nivelRef,estado:v.lot.estado,revision:v.lot.revision};
    if(op==='CerrarReservaSinInicio'){
      const a=v.attempts.find(a=>a.intento_id===5),repeated=closures>0;
      if(!repeated){closures++;a.estado='RECHAZADO';a.fin_utc='fixed';a.evidencia_doc=JSON.stringify({Codigo:'RESERVA_CERRADA_SIN_INICIO'});v.files[0].estado_efecto='NO_INICIADO';v.files[0].intento_activo_id=null;v.files[0].revision++;v.lot.revision++;}
      return {estado:'CONFIRMADO',codigo:'RESERVA_CERRADA_SIN_INICIO',intentoId:'5',resuelta:true,repeticion:repeated};
    }
    assert.equal(op,'AvanzarLote');const key=JSON.stringify(r);if(commands.has(key))return commands.get(key);
    const f=v.files.find(f=>String(f.archivo_id)===r.recursoId),i=Number(r.recursoId)-3;
    if(r.fase==='MATERIALIZAR_VALORES')f.valores_efectivos_doc=JSON.stringify(values(i));
    else{assert.equal(r.fase,'ALMACENAR');assert.equal(f.estado_efecto,'NO_INICIADO');stores++;f.estado_efecto='COMPLETADO';f.resultado_doc='confirmed';}
    f.revision++;v.lot.revision++;const result={estado:'CONFIRMADO',loteId:'2',archivoId:r.recursoId,fase:r.fase,documentoId:String(90+i),inventarioId:String(100+i)};commands.set(key,result);return result;
  }};
  const controls={snapshot:async()=>structuredClone(v),assertNoDocuments:async()=>{},verify:async()=>({fingerprint:JSON.stringify(v),roots:2,documents:2})};
  return {p,v,driver,controls,calls,stats:()=>({closures,stores,reopened})};
}
function reconciliationResumeFixture(){
  const f=resumeFixture(),v=f.v,baseRequest=f.driver.request;let reconciliations=0;
  v.lot.estado='PENDIENTE_VERIFICACION';v.roots[0].estado='INCIERTO';
  const proof={Estado:'INCIERTO',DocumentoId:'16204',InventarioId:'23293',Expediente:JSON.parse(v.roots[0].identidad_expediente_doc),ValoresEfectivos:JSON.parse(v.files[0].valores_efectivos_doc)};
  const a=v.attempts.at(-1);Object.assign(a,{intento_id:6,estado:'INCIERTO',evidencia_doc:JSON.stringify(proof)});
  Object.assign(v.files[0],{estado_efecto:'INCIERTO',intento_activo_id:6,resultado_doc:JSON.stringify(proof)});
  f.driver.request=async(op,r)=>{
    assert.notEqual(op,'CerrarReservaSinInicio');assert.ok(!(op==='AvanzarLote'&&r.recursoId==='3'),'Reconciliation replays existing first document');
    if(op!=='ReconciliarRecurso')return baseRequest(op,r);
    f.calls.push({op,r});const repeated=reconciliations>0;
    if(!repeated){reconciliations++;const result={...proof,Estado:'CONFIRMADO'};a.estado='CONFIRMADO';a.fin_utc='fixed';a.evidencia_doc=JSON.stringify(result);v.files[0].estado_efecto='COMPLETADO';v.files[0].intento_activo_id=null;v.files[0].resultado_doc=JSON.stringify(result);v.files[0].revision++;v.lot.revision++;v.lot.estado='EN_CURSO';v.roots[0].estado='COMPLETADA';}
    return {estado:'CONFIRMADO',resuelta:true,repeticion:repeated,loteId:'2',intentoId:'6',documentoId:proof.DocumentoId,inventarioId:proof.InventarioId};
  };
  return {...f,reconciliations:()=>reconciliations};
}
test('DOC111: reconciliacion conserva documento existente y solo ejecuta el segundo almacenamiento',async()=>{
  const f=reconciliationResumeFixture(),result=await runReconciliationResume(f.driver,f.controls,f.p,Buffer.alloc(7),'2','6');
  assert.equal(result.existingDocumentReconciled,true);assert.equal(result.rec01,undefined);assert.equal(result.closedWithoutStart,undefined);
  assert.deepEqual(f.stats(),{closures:0,stores:1,reopened:1});assert.equal(f.reconciliations(),1);
  assert.equal(f.calls.filter(c=>c.op==='ReconciliarRecurso').length,2);
});
test('DOC111: reconciliacion sin IDs/evidencia o con resultado incierto no inicia el segundo archivo',async()=>{
  for(const mutate of [f=>f.v.attempts.at(-1).evidencia_doc='{}',f=>f.v.files[0].resultado_doc='{}',f=>f.v.files[1].intento_activo_id=99,f=>f.v.roots[0].estado='CREANDO',f=>{const base=f.driver.request;f.driver.request=async(op,r)=>op==='ReconciliarRecurso'?{estado:'INCIERTO',resuelta:false}:base(op,r);}]){
    const f=reconciliationResumeFixture();mutate(f);await assert.rejects(runReconciliationResume(f.driver,f.controls,f.p,Buffer.alloc(7),'2','6'),/^Error: DOC111_/);assert.equal(f.stats().stores,0);
  }
});
test('DOC111: selector distingue reconciliar/cerrar y sanea resultado de continuacion sin aprobar REC',()=>{
  const args=['--profile','x','--scenario','LOTE','--resume-lot','2','--reconcile-attempt','6'];assert.equal(runner.parseArgs(args)['reconcile-attempt'],'6');
  assert.throws(()=>runner.parseArgs([...args,'--close-attempt','5']),/RESUME_ARGUMENT_INVALID/);
  const r={status:'PASS',mode:'LOTE',loteId:'2',intentoId:'6',roots:2,documents:2,existingDocumentReconciled:true,existingExpeditionsPreserved:true,idsAndIndicesVerified:true,replayAndReopenUnchanged:true,head:'a'.repeat(40),codeSha256:'b'.repeat(64),driverSha256:'c'.repeat(64),private:'MUST_NOT_ESCAPE'};
  const line='DOC111_RECONCILE_RESUME_RESULT='+JSON.stringify(r);
  assert.equal(runner.safeResult(line,'LOTE'),null);assert.equal(runner.safeResumeResult(line,'LOTE'),null);assert.ok(!JSON.stringify(runner.safeReconcileResumeResult(line,'LOTE')).includes('MUST_NOT_ESCAPE'));
});
test('DOC111: recuperacion conserva lote/expedientes, cierra una reserva y solo almacena los documentos pendientes',async()=>{
  const f=resumeFixture();const r=await runResume(f.driver,f.controls,f.p,Buffer.alloc(7),'2','5');
  assert.deepEqual(f.stats(),{closures:1,stores:2,reopened:1});assert.equal(r.closedWithoutStart,true);assert.equal(r.rec01,undefined);
  assert.ok(!f.calls.some(c=>['PrepararPlan','ConfirmarPlan','RecibirArchivo'].includes(c.op)||c.r.fase==='CREAR_EXPEDIENTE'));
  assert.equal(f.calls.filter(c=>c.r.fase==='MATERIALIZAR_VALORES').length,1);
});
test('DOC111: recuperacion se detiene antes del cierre ante inicio, incertidumbre, destino cambiado o documentos presentes',async()=>{
  for(const mutate of [f=>f.v.attempts.at(-1).estado='EN_CURSO',f=>f.v.attempts.at(-1).estado='INCIERTO',f=>f.v.attempts.at(-1).evidencia_doc='{}',f=>f.v.events.push({intento_id:5,accion:'INICIAR_ALMACENAMIENTO'}),f=>f.v.roots[0].estado='INCIERTO',f=>f.v.files[0].resultado_doc='{}',f=>f.p.versionId='99',f=>f.controls.assertNoDocuments=async()=>{throw new Error('DOC111_DOCUMENT_ALREADY_PRESENT');}]){
    const f=resumeFixture();mutate(f);await assert.rejects(runResume(f.driver,f.controls,f.p,Buffer.alloc(7),'2','5'),/^Error: DOC111_/);assert.deepEqual(f.stats(),{closures:0,stores:0,reopened:0});
  }
});
test('DOC111: recuperacion no continua despues de cierre perdido ni reintenta un almacenamiento incierto',async()=>{
  for(const phase of ['CerrarReservaSinInicio','ALMACENAR']){
    const f=resumeFixture(),request=f.driver.request;
    f.driver.request=async(op,r)=>{if(op===phase||r.fase===phase)throw new Error('DOC111_OPERATION_NOT_CONFIRMED');return request(op,r);};
    await assert.rejects(runResume(f.driver,f.controls,f.p,Buffer.alloc(7),'2','5'),/OPERATION_NOT_CONFIRMED/);assert.equal(f.stats().stores,0);
  }
});
test('DOC111: ausencia documental se comprueba por SELECT en gabinete, inventario e indice de ambos expedientes',async()=>{
  for(const count of [0,1]){
    const f=resumeFixture(),queries=[];const controls=createSelectControls(async(origin,sql,args)=>{queries.push({origin,sql,args});assert.ok(sql.startsWith('SELECT ')&&!/FOR UPDATE|;|DELETE|INSERT|UPDATE /i.test(sql));return sql.includes('FROM system1')?[{NOMBRE:'cabinet_fixture'}]:[{total:count}];});
    f.v.lot.config_doc=JSON.stringify({Configuracion:{GabineteId:10}});
    if(count===0){await controls.assertNoDocuments(f.v);assert.equal(queries.length,7);assert.equal(queries.filter(q=>q.args[0]==='648').length,3);assert.equal(queries.filter(q=>q.args[0]==='649').length,3);}
    else await assert.rejects(controls.assertNoDocuments(f.v),/DOCUMENT_ALREADY_PRESENT/);
  }
});
test('DOC111: parametros de recuperacion son pareados, numericos, solo LOTE y no aprueban REC',()=>{
  const args=['--profile','x','--scenario','LOTE','--resume-lot','2','--close-attempt','5'];assert.equal(runner.parseArgs(args)['resume-lot'],'2');
  for(const bad of [args.slice(0,-2),[...args,'--diagnose'],args.map(x=>x==='LOTE'?'EXPEDIENTE':x),args.map(x=>x==='5'?'0':x)])assert.throws(()=>runner.parseArgs(bad),/DOC111_/);
  const r={status:'PASS',mode:'LOTE',loteId:'2',intentoId:'5',roots:2,documents:2,closedWithoutStart:true,existingExpeditionsPreserved:true,idsAndIndicesVerified:true,replayAndReopenUnchanged:true,head:'a'.repeat(40),codeSha256:'b'.repeat(64),driverSha256:'c'.repeat(64),secret:'PRIVATE'};
  assert.equal(runner.safeResult('DOC111_RESUME_RESULT='+JSON.stringify(r),'LOTE'),null);
  assert.ok(!JSON.stringify(runner.safeResumeResult('DOC111_RESUME_RESULT='+JSON.stringify(r),'LOTE')).includes('PRIVATE'));
  assert.equal(runner.safeResumeResult('DOC111_RESUME_RESULT='+JSON.stringify({...r,closedWithoutStart:false}),'LOTE'),null);
});

test('DOC111: contrato y selector rechazan secretos, modos/argumentos y expectativas ambiguas',()=>{
  assert.equal(validateProfile(profile()).mode,'EXPEDIENTE');assert.ok(readFixture(profile()).length>0);
  for(const change of [p=>p.password='private',p=>p.baseUrl='https://u:p@test.invalid/',p=>p.versionId='05',p=>p.module='OTRO',p=>p.pdfFixture='../secret.pdf',p=>p.expectations[0].indices=[{referencia:'91',tipo:'TEXTO',valor:'a'},{referencia:'91',tipo:'TEXTO',valor:'b'}]]){const p=profile();change(p);assert.throws(()=>validateProfile(p),/^Error: DOC111_/);}
  for(const args of [[],['--profile','x','--scenario','all'],['--profile','x','--scenario','EXPEDIENTE','--check','--check'],['--profile','x','--scenario','LOTE','--password','private']])assert.throws(()=>runner.parseArgs(args),/^Error: DOC111_/);
  assert.throws(()=>assertAuthorizations({environment:true,account:true}),/AUTHORIZATION_REQUIRED/);
  for(const dsn of ['mysql://private@host/db','DSN=x;PWD=private','../outside']){const p=profile();p.odbcDsn=dsn;assert.throws(()=>validateProfile(p),/PROFILE_DSN_INVALID/);}
});

test('DOC111: --check no solicita secretos ni arranca un proceso o sesion',async()=>{
  const forbidden=()=>{throw new Error('unexpected interaction');};
  await runner.main(['--check','--profile',examplePath,'--scenario','EXPEDIENTE'],{requireInteractiveConsole:forbidden,collectConfirmation:forbidden,collectValue:forbidden,runChild:forbidden});
});

test('DOC111: runner exige autorizacion concreta y un resultado completo, sanea salida y borra secretos efimeros',async()=>{
  const args=['--profile',examplePath,'--scenario','EXPEDIENTE'];let prompts=0,seen;
  await assert.rejects(runner.main(args,{requireInteractiveConsole(){},collectConfirmation:async()=>{throw new Error('declined');},collectValue:async()=>prompts++,runChild:async()=>prompts++}),/declined/);assert.equal(prompts,0);
  const flags={rec01:true,rec02:true,barrierPreserved:true,idsAndIndicesVerified:true,replayAndReopenUnchanged:true};
  const result={status:'PASS',mode:'EXPEDIENTE',roots:2,documents:2,originalFiles:2,...flags,head:'a'.repeat(40),codeSha256:'b'.repeat(64),driverSha256:'c'.repeat(64),password:'MUST_NOT_ESCAPE'};
  assert.equal(runner.safeResult('DOC111_REC_RESULT='+JSON.stringify({...result,rec02:false}),'EXPEDIENTE'),null);
  assert.equal(runner.safeResult('DOC111_REC_RESULT='+JSON.stringify(result),'LOTE'),null);
  assert.ok(!JSON.stringify(runner.safeResult('DOC111_REC_RESULT='+JSON.stringify(result),'EXPEDIENTE')).includes('MUST_NOT_ESCAPE'));
  assert.deepEqual(runner.safeReference('DOC111_REC_REFERENCE='+JSON.stringify({mode:'EXPEDIENTE',planId:'17',loteId:'20',password:'MUST_NOT_ESCAPE'}),'EXPEDIENTE'),{mode:'EXPEDIENTE',planId:'17',loteId:'20'});
  const runtime={requireInteractiveConsole(){},collectConfirmation:async(a,k)=>{a[k]='true';},collectValue:async(s,k)=>{s[k]='EPHEMERAL_ONLY';},runChild:async(command,argv,cwd,env,options)=>{
    seen=env;assert.equal(env.DOC111_AUTH_EFFECTS,'true');assert.ok(argv.includes('--retries=0'));assert.ok(argv.includes('--workers=1'));assert.ok(argv.includes('--reporter=list'));assert.equal(env.DOC111_E2E_PASSWORD,'EPHEMERAL_ONLY');
    assert.equal(env.DOC111_E2E_ODBC_DSN,'dsn-ra-pruebas');assert.equal(env.DOC111_DA_E2E_ODBC_DSN,'dsn-da-pruebas');assert.equal(env.DOC111_E2E_MYSQL_PASSWORD,'EPHEMERAL_ONLY');assert.equal(env.DOC111_E2E_RA_READONLY_URL,undefined);
    const safe=options.redactOutput('Cookie: SECRET\nDOC111_REC_RESULT='+JSON.stringify(result));assert.ok(!safe.includes('SECRET')&&!safe.includes('MUST_NOT_ESCAPE'));return {code:0};
  }};
  await runner.main(args,runtime);assert.equal(seen.DOC111_E2E_PASSWORD,undefined);assert.equal(seen.DOC111_E2E_MYSQL_PASSWORD,undefined);assert.equal(seen.DOC111_DA_E2E_MYSQL_PASSWORD,undefined);
  runtime.runChild=async()=>({code:0});await assert.rejects(runner.main(args,runtime),/REC_FAILED/);
});

test('DOC111: diagnostico conserva solo etapa/codigo y nunca datos privados ni PASS',async()=>{
  const failure={mode:'EXPEDIENTE',stage:'READONLY_RA',code:'DOC111_READONLY_CONNECTION_INVALID',password:'PRIVATE_SENTINEL'};
  const line='DOC111_REC_FAILURE='+JSON.stringify(failure);
  assert.deepEqual(runner.safeFailure(line,'EXPEDIENTE'),{mode:'EXPEDIENTE',stage:'READONLY_RA',code:failure.code});
  for(const change of [r=>r.stage='mysql://PRIVATE_SENTINEL',r=>r.code='PRIVATE_SENTINEL',r=>r.code='DOC111_ERROR mysql://PRIVATE_SENTINEL',r=>r.mode='LOTE']){
    const altered={...failure};change(altered);assert.equal(runner.safeFailure('DOC111_REC_FAILURE='+JSON.stringify(altered),'EXPEDIENTE'),null);
  }
  assert.equal(runner.safeFailure('DOC111_REC_FAILURE=invalid','EXPEDIENTE'),null);
  let environment;
  await assert.rejects(runner.main(['--profile',examplePath,'--scenario','EXPEDIENTE'],{
    requireInteractiveConsole(){},collectConfirmation:async(a,k)=>a[k]='true',collectValue:async(s,k)=>s[k]='PRIVATE_SENTINEL',
    runChild:async(command,args,cwd,env,options)=>{environment=env;const output=options.redactOutput('Cookie: PRIVATE_SENTINEL\n'+line);assert.ok(output.includes('READONLY_RA'));assert.ok(!output.includes('PRIVATE_SENTINEL'));return {code:1};}
  }),/DOC111_REC_FAILED/);
  assert.equal(environment.DOC111_E2E_PASSWORD,undefined);
});

test('DOC111: rechazo HTTP conserva operacion y codigo publico sin cuerpo, mensaje ni token',async()=>{
  const auth={page:{locator:()=>({getAttribute:async()=>'PRIVATE_TOKEN'})},context:{request:{post:async()=>({status:()=>503,json:async()=>({d:{ok:false,codigo:'CAPACIDAD_NO_DISPONIBLE',mensaje:'PRIVATE_MESSAGE',datos:{password:'PRIVATE_PASSWORD'}}})})}}};
  const authorization=Object.fromEntries(['environment','account','effects','discardable','select'].map(k=>[k,true]));
  const driver=createDriver(auth,profile(),authorization);await driver.refreshToken();
  await assert.rejects(driver.request('AbrirImportacion',{nivelRef:'7'}),error=>{
    assert.equal(error.message,'DOC111_OPERATION_NOT_CONFIRMED');
    const result=runner.safeFailure('DOC111_REC_FAILURE='+JSON.stringify({mode:'LOTE',stage:'REC',code:error.message,operation:error.operation,httpStatus:error.httpStatus,serverCode:error.serverCode,body:'PRIVATE_MESSAGE'}),'LOTE');
    assert.deepEqual(result,{mode:'LOTE',stage:'REC',code:'DOC111_OPERATION_NOT_CONFIRMED',operation:'AbrirImportacion',httpStatus:503,serverCode:'CAPACIDAD_NO_DISPONIBLE'});assert.ok(!JSON.stringify(error).includes('PRIVATE'));return true;
  });
  const unsafe=runner.safeFailure('DOC111_REC_FAILURE='+JSON.stringify({mode:'LOTE',stage:'REC',code:'DOC111_OPERATION_NOT_CONFIRMED',operation:'PRIVATE_OPERATION',httpStatus:503,serverCode:'PRIVATE_CODE'}),'LOTE');
  assert.deepEqual(unsafe,{mode:'LOTE',stage:'REC',code:'DOC111_OPERATION_NOT_CONFIRMED'});
});

test('DOC111: preflight exige pagina autenticada, control y token antes del REC; falla saneado y cierra sesion',async()=>{
  const authorization=Object.fromEntries(['environment','account','effects','discardable','select'].map(k=>[k,true]));
  for(const [state,expected] of [[{},null],[{login:true},'AUTHENTICATED_PAGE_UNAVAILABLE'],[{controls:0},'IMPORT_CONTROL_UNAVAILABLE'],[{token:''},'TOKEN_UNAVAILABLE'],[{status:500},'PRODUCTION_OPEN_FAILED'],[{navigationError:true},'PRODUCTION_OPEN_FAILED']]){
    let closed=0;const auth={context:{close:async()=>closed++},page:{goto:async(url,options)=>{assert.equal(options.waitUntil,'domcontentloaded');if(state.navigationError)throw new Error('PRIVATE_COOKIE');return {status:()=>state.status||200};},locator:selector=>({count:async()=>selector==='#doc111-importacion'?(state.controls??1):(state.login?1:0),getAttribute:async()=>state.token??'PRIVATE_TOKEN'})}};
    const invoke=()=>openSession({},profile(),{},authorization,{createSession:async()=>auth});
    if(expected){await assert.rejects(invoke(),new RegExp('^Error: DOC111_'+expected+'$'));assert.equal(closed,1);}else{assert.equal(await invoke(),auth);assert.equal(closed,0);}
  }
});

test('DOC111: control ausente reutiliza diagnostico ASMX de lectura sin falsificar token ni preparar plan',async()=>{
  const authorization=Object.fromEntries(['environment','account','effects','discardable','select'].map(k=>[k,true]));
  for(const [status,codigo,expected] of [[401,'SESION_REQUERIDA','SESSION_NOT_ESTABLISHED'],[403,'ACCESO_DENEGADO','IMPORT_ACCESS_DENIED'],[503,'CAPACIDAD_NO_DISPONIBLE','IMPORT_CAPABILITY_UNAVAILABLE'],[403,'SOLICITUD_NO_VERIFICADA','IMPORT_CONTROL_UNAVAILABLE'],[500,'PRIVATE_MESSAGE','IMPORT_CONTROL_UNAVAILABLE']]){
    let calls=0,closed=0;const auth={context:{close:async()=>closed++,request:{post:async(url,options)=>{calls++;assert.ok(url.endsWith('/AbrirImportacion'));assert.equal(options.maxRetries,0);assert.equal(options.maxRedirects,0);assert.equal(options.headers['X-Importacion-Carpetas-CSRF'],undefined);assert.deepEqual(options.data,{solicitud:{nivelRef:profile().nivelRef}});return {status:()=>status,json:async()=>({d:{ok:false,codigo,mensaje:'PRIVATE_MESSAGE'}})};}}},page:{goto:async()=>({status:()=>200}),locator:()=>({count:async()=>0})}};
    await assert.rejects(openSession({},profile(),{},authorization,{createSession:async()=>auth}),new RegExp('^Error: DOC111_'+expected+'$'));assert.equal(calls,1);assert.equal(closed,1);
  }
});

test('DOC111: diagnostico solo solicita aplicacion, no concede efectos ni acepta un PASS de carga',async()=>{
  const confirmations=[],secrets=[],args=['--diagnose','--profile',examplePath,'--scenario','EXPEDIENTE'];let seen;
  const runtime={requireInteractiveConsole(){},collectConfirmation:async(a,k)=>{confirmations.push(k);a[k]='true';},collectValue:async(s,k)=>{secrets.push(k);s[k]='PRIVATE_SENTINEL';},runChild:async(command,argv,cwd,env,options)=>{
    seen=env;assert.equal(env.DOC111_E2E_DIAGNOSE,'true');assert.equal(env.DOC111_AUTH_EFFECTS,'false');assert.equal(env.DOC111_AUTH_SELECT,'false');assert.equal(env.DOC111_AUTH_DISCARDABLE,'false');
    const result=options.redactOutput('DOC111_DIAGNOSTIC_RESULT='+JSON.stringify({status:'PASS',mode:'EXPEDIENTE',pageReady:true,password:'PRIVATE_SENTINEL'}));assert.ok(!result.includes('PRIVATE_SENTINEL'));assert.ok(result.includes('pageReady'));return {code:0};
  }};
  await runner.main(args,runtime);assert.deepEqual(confirmations,['environment','account']);assert.deepEqual(secrets,['user','password']);assert.equal(seen.DOC111_E2E_PASSWORD,undefined);
  runtime.runChild=async()=>({code:0});await assert.rejects(runner.main(args,runtime),/DOC111_DIAGNOSTIC_FAILED/);
  assert.throws(()=>runner.parseArgs([...args,'--check']),/DOC111_/);
  assert.throws(()=>createDriver({},profile(),{environment:true,account:true}),/AUTHORIZATION_REQUIRED/);
});

function flowFixture(mode){
  const p=profile();p.mode=mode;
  const state={received:[false,false],created:[false,false],fixed:[false,false],stored:[false,false],confirmed:false,writes:0},calls=[],commands=new Map();let reopened=0;
  const image=()=>({...structuredClone(state),files:p.expectations.map((e,i)=>({archivo_id:30+i,valores_efectivos_doc:state.fixed[i]?JSON.stringify([{DestinoTipo:'TIPOLOGIA',Valor:e.tipologiaId},...e.indices.map(v=>({DestinoTipo:'INDICE_GABINETE',DestinoRef:v.referencia,TipoValor:v.tipo,Valor:v.valor}))]):null}))}),fingerprint=()=>crypto.createHash('sha256').update(JSON.stringify(state)).digest('hex');
  const rootRows=()=>p.rootNames.map((nombre,i)=>({recursoId:String(40+i),idCliente:'r'+(i+1),nombre,revision:state.writes+1}));
  const fileRows=()=>[...p.rootNames.map((_,i)=>({recursoId:String(30+i),raizId:String(40+i),idCliente:'a'+(i+1),revision:state.writes+1})),...(mode==='PARCIAL'?[{recursoId:'32',raizId:'41',idCliente:'bad',revision:1}]:[])];
  const driver={refreshToken:async()=>{},pages:async(lot,kind)=>kind==='RAICES'?rootRows():fileRows(),receive:async(lot,file,bytes)=>{state.bytes=bytes.length;state.received[Number(file.recursoId)-30]=true;state.writes++;},reopen:async()=>{reopened++;},request:async(op,request,rejected=false)=>{
    calls.push({op,request,rejected});
    if(op==='AbrirImportacion')return {nivelRef:p.nivelRef};
    if(op==='PrepararPlan')return {planId:'17',revision:1,planHash:'a'.repeat(64),nivelRef:p.nivelRef,snapshot:{VersionId:p.versionId,Configuracion:{ModoIntegridad:mode==='PARCIAL'?'PARCIAL_PERMITIDO':'COMPLETO_OBLIGATORIO',AmbitoIntegridad:mode==='PARCIAL'?'NO_APLICA':mode}},resultado:{PuedeConfirmar:true,UnidadesElegibles:['r1','r2'],Elementos:p.expectations.map((e,i)=>({Ambito:'RAIZ',RaizIdCliente:'r'+(i+1),Valores:[{DestinoRef:'CODIGO',Valor:e.codigo},{DestinoRef:'NOMBRE',Valor:e.nombre}]}))}};
    if(op==='ConfirmarPlan'){if(!state.confirmed){state.confirmed=true;state.writes++;}return {loteId:'20',nivelRef:p.nivelRef};}
    if(op==='ConsultarLote')return {estado:mode==='PARCIAL'?'PARCIAL':'COMPLETADO',totalRaices:2,totalArchivos:mode==='PARCIAL'?3:2,totalBytes:String(state.bytes*(mode==='PARCIAL'?3:2))};
    assert.equal(op,'AvanzarLote');const index=Number(request.recursoId)-(request.fase==='CREAR_EXPEDIENTE'?40:30);
    if(rejected){assert.equal(request.fase,'CREAR_EXPEDIENTE');assert.equal(mode==='LOTE'?state.received.every(Boolean):state.received[index],false);return;}
    const key=JSON.stringify(request);if(commands.has(key))return commands.get(key);
    assert.ok(state.received[index]);if(mode==='LOTE')assert.ok(state.received.every(Boolean));
    if(request.fase==='CREAR_EXPEDIENTE'){assert.equal(state.created[index],false);state.created[index]=true;}
    else if(request.fase==='MATERIALIZAR_VALORES'){assert.ok(state.created[index]);state.fixed[index]=true;}
    else{assert.ok(state.fixed[index]);assert.equal(state.stored[index],false);state.stored[index]=true;}
    state.writes++;const r={estado:'CONFIRMADO',loteId:'20',raizId:String(40+index),archivoId:request.fase==='CREAR_EXPEDIENTE'?null:request.recursoId,fase:request.fase,expedienteId:String(60+index),relacionNivelId:String(70+index)};commands.set(key,r);return r;
  }};
  const controls={snapshot:async()=>image(),verify:async()=>({fingerprint:fingerprint(),roots:2,documents:2})};
  return {p,driver,controls,calls,state,reopened:()=>reopened};
}
for(const mode of MODES)test('DOC111 REC drivers '+mode+': barrera, dos raices, fases y reapertura sin nuevos efectos (dobles)',async()=>{
  const f=flowFixture(mode),references=[],r=await runRec(f.driver,f.controls,f.p,Buffer.from('%PDF-fixture'),r=>references.push(r));
  assert.deepEqual(f.state.stored,[true,true]);assert.equal(r.rec02,true);assert.equal(r.originalFiles,mode==='PARCIAL'?3:2);assert.equal(f.reopened(),1);
  assert.equal(f.calls.filter(c=>c.rejected).length,1);assert.equal(f.calls.filter(c=>c.op==='AvanzarLote'&&!c.rejected).length,12);
  assert.deepEqual(references,[{planId:'17'},{planId:'17',loteId:'20'}]);
});
test('DOC111: REC no aprueba si una repeticion muta datos',async()=>{
  const f=flowFixture('LOTE'),original=f.driver.request;let advances=0;
  f.driver.request=async(op,req,rejected)=>{const result=await original(op,req,rejected);if(op==='AvanzarLote'&&!rejected&&++advances===7)f.state.writes++;return result;};
  await assert.rejects(runRec(f.driver,f.controls,f.p,Buffer.from('%PDF-fixture')),/REPLAY_OR_REOPEN_CHANGED_DATA/);
});
test('DOC111: valores fijados inesperados detienen REC antes del almacenamiento',async()=>{
  const f=flowFixture('EXPEDIENTE'),original=f.controls.snapshot;
  f.controls.snapshot=async()=>{const v=await original();if(f.state.fixed[0])v.files[0].valores_efectivos_doc='[]';return v;};
  await assert.rejects(runRec(f.driver,f.controls,f.p,Buffer.from('%PDF-fixture')),/EFFECTIVE_VALUES_INVALID/);
  assert.equal(f.calls.filter(c=>c.op==='AvanzarLote'&&c.request.fase==='ALMACENAR').length,0);
});
test('DOC111: REC no aprueba respuestas de repeticion con otros IDs',async()=>{
  const f=flowFixture('LOTE'),original=f.driver.request;let advances=0;
  f.driver.request=async(op,req,rejected)=>{const r=await original(op,req,rejected);return op==='AvanzarLote'&&!rejected&&++advances===7?{...r,expedienteId:'999'}:r;};
  await assert.rejects(runRec(f.driver,f.controls,f.p,Buffer.from('%PDF-fixture')),/REPLAY_NOT_CONFIRMED/);
});

function selectFixture(){
  const p=profile();p.expectations.forEach(e=>e.indices=[{referencia:'91',tipo:'TEXTO',valor:'0007'},{referencia:'92',tipo:'DECIMAL',valor:'1.20'}]);
  const c={GabineteId:7,ModoIntegridad:'COMPLETO_OBLIGATORIO',AmbitoIntegridad:'EXPEDIENTE',FondoRef:'1',InstrumentoRef:'2',AreaRef:'3',SerieRef:'4',SubserieRef:null,TipoExpedienteRef:'5',UnidadDocumentalRef:'6'};
  const lot={config_doc:JSON.stringify({VersionId:p.versionId,Configuracion:c}),contexto_doc:JSON.stringify({UsuarioRef:'1',AmbitoRef:'2',NivelRef:'7'}),total_raices:2,total_archivos:2,total_bytes:14,estado:'COMPLETADO'},roots=[],files=[],attempts=[],physical=[],exps=[];
  for(let i=0;i<2;i++){
    const identity={OrigenLogico:'RA',NivelRef:'7',ExpedienteId:String(60+i),RelacionNivelId:String(70+i)};
    roots.push({raiz_id:40+i,cliente_raiz_id:'r'+(i+1),estado:'COMPLETADA',identidad_expediente_doc:JSON.stringify(identity)});
    attempts.push({intento_id:49+i,raiz_id:40+i,fase:'CREAR_EXPEDIENTE',estado:'CONFIRMADO',fin_utc:'2026-10-30',evidencia_doc:JSON.stringify({Estado:'CONFIRMADO',Expediente:identity,Creacion:{CicloArchivoId:'13',IntentoId:String(49+i)}})});
    const values=[{DestinoTipo:'TIPOLOGIA',Valor:'9'},...p.expectations[i].indices.map(v=>({DestinoTipo:'INDICE_GABINETE',DestinoRef:v.referencia,TipoValor:v.tipo,Valor:v.valor,Diferido:false}))];
    const result={Estado:'CONFIRMADO',Expediente:identity,GabineteId:'7',OrigenGabinete:'DA',DocumentoId:String(90+i),InventarioId:String(100+i),ValoresEfectivos:values};
    files.push({archivo_id:30+i,raiz_id:40+i,cliente_archivo_id:'a'+(i+1),bytes_previstos:7,estado_recepcion:'RECIBIDO',estado_efecto:'COMPLETADO',valores_efectivos_doc:JSON.stringify(values),resultado_doc:JSON.stringify(result)});
    attempts.push({raiz_id:40+i,archivo_id:30+i,fase:'ALMACENAR',estado:'CONFIRMADO',fin_utc:'2026-10-30',evidencia_doc:JSON.stringify(result)});
    exps.push({nivel:'7',ID_USUARIO_GESTION:1,ID_EMPRESA_EXPEDIENTE:2,CODIGO_UNICO:p.expectations[i].codigo,ALEAS_EXPEDIENTE:p.expectations[i].nombre,TEMA_EXPEDIENTE:p.expectations[i].nombre,Id_tipos_ciclo_archivo:13,NOMBRE_CICLO_ARCHIVO:'Archivo Gestión',ID_FONDO:1,id_instrumento:2,CODIGO_AREA_TRD:3,CODIGO_SERIE_TRD:4,CODIGO_SUB_SERIE_TRD:null,RA_TIP_EXPE_ID_TIPO_EXPEDIENTE:5,ID_TIPO_UNIDAD_DOCUMENTAL:6});
    physical.push({ID_EXPEDIENTE:String(60+i),indexed_expedition:String(60+i),ID_USUARIO_GESTION:1,inventory_user:1,ID_EMPRESA_DOCUMENTO:2,ID_TIPO_DOCUMENTO:9,ID_CLASE_DOCUMENTO:3,DBT:6,NOMBRE:FILE_NAME,SEGUNDO_NOMBRE_DOCUMENTO:FILE_NAME,segundo_nombre:FILE_NAME,inventory_format:'.PDF',index_format:'.PDF',value_0:'0007',value_1:'1.2000'});
  }
  let field=null;const queries=[],nameMetadata=[{VISIBLE:1}];
  const query=async(origin,sql,args)=>{
    queries.push({origin,sql,args});assert.match(sql,/^SELECT /);assert.ok(!sql.includes(';'));
    if(sql.includes(' FROM ra_integracion_lote '))return [structuredClone(lot)];
    if(sql.includes(' FROM ra_integracion_raiz '))return structuredClone(roots);
    if(sql.includes(' FROM ra_integracion_archivo '))return structuredClone(files);
    if(sql.includes(' FROM ra_integracion_intento '))return structuredClone(attempts);
    if(sql.includes(' FROM ra_integracion_evento '))return [];
    if(sql.includes(' FROM system1 '))return [{NOMBRE:'gab_fixture'}];
    if(sql.includes(' FROM ra_de_tipos_ciclos_archivo '))return [{id:13,nombre:'Archivo Gestión'}];
    if(sql.includes(' FROM ra_tipo_documento '))return [{id:3}];
    if(sql.includes(' FROM DA_EXTENSION '))return [{id:6}];
    if(sql.startsWith('SELECT e.*'))return [structuredClone(exps[Number(args[0])-60])];
    if(sql.startsWith('SELECT VISIBLE FROM detalle_gabienete '))return structuredClone(nameMetadata);
    if(sql.includes(' FROM detalle_gabienete '))return [{CAMPO:field||'CAMPO_'+args[1]}];
    if(sql.startsWith('SELECT g.ID,')){assert.ok(sql.includes('r.FORMATO AS inventory_format')&&sql.includes('i.formato AS index_format'));return [structuredClone(physical[Number(args[1])-90])];}
    throw new Error('UNEXPECTED_SELECT');
  };
  return {p,lot,roots,files,attempts,physical,exps,queries,nameMetadata,controls:createSelectControls(query),badField(){field='bad`; DELETE FROM x';}};
}
test('DOC111: controles SELECT verifican destino, Archivo Gestion dinamico, nombres y campos fisicos tipados',async()=>{
  const f=selectFixture(),r=await f.controls.verify('20',f.p);assert.equal(r.documents,2);assert.match(r.fingerprint,/^[a-f0-9]{64}$/);assert.ok(f.queries.some(q=>q.origin==='DA'&&q.sql.includes('CAST(g.`CAMPO_91` AS CHAR)')));
});
test('DOC111: ambos formatos se verifican por alias distintos y ninguno se pierde por mayusculas',async()=>{
  for(const field of ['inventory_format','index_format']){const f=selectFixture();f.physical[0][field]='.TIF';await assert.rejects(f.controls.verify('20',f.p),/DOCUMENT_VALUES_INVALID/);}
});
test('DOC111: verificacion de lote completo solo consulta y reabre sin invocar fases',async()=>{
  const f=selectFixture();f.p.mode='LOTE';const config=JSON.parse(f.lot.config_doc);config.Configuracion.AmbitoIntegridad='LOTE';f.lot.config_doc=JSON.stringify(config);
  let reopened=0;const calls=[];const driver={refreshToken:async()=>{},request:async(op,r)=>{calls.push(op);assert.equal(op,'ConsultarLote');return {loteId:r.loteId,nivelRef:f.p.nivelRef,estado:'COMPLETADO',totalRaices:2,totalArchivos:2,totalBytes:'14'};},reopen:async()=>reopened++};
  const result=await runCompletedVerification(driver,f.controls,f.p,Buffer.alloc(7),'20');assert.equal(result.completedVerified,true);assert.equal(result.reopenUnchanged,true);assert.equal(result.rec01,undefined);assert.equal(reopened,1);assert.deepEqual(calls,['ConsultarLote','ConsultarLote']);
  let reads=0;f.controls.verify=async()=>({fingerprint:String(++reads),roots:2,documents:2});await assert.rejects(runCompletedVerification(driver,f.controls,f.p,Buffer.alloc(7),'20'),/REOPEN_CHANGED_DATA/);
});
test('DOC111: driver de lectura rechaza fases, reconciliacion y recepcion antes del transporte',async()=>{
  let posts=0;const auth={page:{locator:()=>({getAttribute:async()=>'fixture-token'})},context:{request:{post:async()=>{posts++;return {status:()=>200,json:async()=>({d:{ok:true,datos:{}}})};}}}};
  const a={environment:true,account:true,select:true,effects:false,discardable:false};const driver=createDriver(auth,profile(),a,{readOnly:true});await driver.refreshToken();
  for(const operation of ['PrepararPlan','ConfirmarPlan','AvanzarLote','CerrarReservaSinInicio','ReconciliarRecurso'])await assert.rejects(driver.request(operation,{}),/READONLY_OPERATION_REQUIRED/);
  await assert.rejects(driver.receive('2',{},Buffer.alloc(1)),/READONLY_OPERATION_REQUIRED/);assert.equal(posts,0);
  await driver.request('ConsultarLote',{loteId:'2'});assert.equal(posts,1);
});
test('DOC111: selector de verificacion no autoriza efectos y solo acepta su resultado de lectura',async()=>{
  const p=profile();p.mode='LOTE';const file=path.join(require('node:os').tmpdir(),'doc111-verify-unit-'+process.pid+'.json');fs.writeFileSync(file,JSON.stringify(p));
  const result={status:'PASS',mode:'LOTE',loteId:'2',roots:2,documents:2,completedVerified:true,reopenUnchanged:true,head:'a'.repeat(40),codeSha256:'b'.repeat(64),driverSha256:'c'.repeat(64)};
  const args=['--profile',file,'--scenario','LOTE','--verify-lot','2'];const confirmations=[];let seen;
  try{
    await runner.main(args,{requireInteractiveConsole(){},collectConfirmation:async(a,k)=>{confirmations.push(k);a[k]='true';},collectValue:async(s,k)=>{s[k]='EPHEMERAL';},runChild:async(command,args,cwd,env,options)=>{seen=env;assert.equal(env.DOC111_AUTH_EFFECTS,'false');assert.equal(env.DOC111_AUTH_DISCARDABLE,'false');assert.equal(env.DOC111_AUTH_SELECT,'true');assert.equal(env.DOC111_E2E_VERIFY_LOT,'2');assert.ok(options.redactOutput('DOC111_COMPLETED_VERIFICATION_RESULT='+JSON.stringify({...result,secret:'PRIVATE'})).includes('completedVerified'));return {code:0};}});
    assert.deepEqual(confirmations,['environment','account','select']);assert.equal(seen.DOC111_E2E_PASSWORD,undefined);assert.equal(runner.safeResult('DOC111_COMPLETED_VERIFICATION_RESULT='+JSON.stringify(result),'LOTE'),null);
    assert.throws(()=>runner.parseArgs([...args,'--resume-lot','2','--close-attempt','5']),/VERIFY_ARGUMENT_INVALID/);
  }finally{fs.unlinkSync(file);}
});
test('DOC111: nombre invisible admite solo vacio/NULL y mantiene los nombres canonicos obligatorios',async()=>{
  for(const name of [null,'']){const f=selectFixture();f.nameMetadata[0].VISIBLE=0;f.physical[0].NOMBRE=name;assert.equal((await f.controls.verify('20',f.p)).documents,2);f.physical[0].SEGUNDO_NOMBRE_DOCUMENTO='other.pdf';await assert.rejects(f.controls.verify('20',f.p),/DOCUMENT_VALUES_INVALID/);}
  const visible=selectFixture();visible.physical[0].NOMBRE='';await assert.rejects(visible.controls.verify('20',visible.p),/DOCUMENT_VALUES_INVALID/);
  const ambiguous=selectFixture();ambiguous.nameMetadata.push({VISIBLE:0});await assert.rejects(ambiguous.controls.verify('20',ambiguous.p),/NAME_METADATA_INVALID/);
});
for(const [name,change,code] of [
  ['nombre',f=>f.physical[0].NOMBRE='otro.pdf','DOCUMENT_VALUES_INVALID'],
  ['ceros',f=>f.physical[0].value_0='7','PHYSICAL_INDEX_INVALID'],
  ['destino',f=>f.exps[0].nivel='8','EXPEDITION_VALUES_INVALID'],
  ['prueba',f=>f.attempts.splice(1,1),'DOCUMENT_PROOF_INVALID'],
  ['actividad',f=>f.attempts[0].estado='INCIERTO','FINAL_PROOF_INVALID'],
  ['ciclo correlacionado',f=>{const r=JSON.parse(f.attempts[0].evidencia_doc);r.Creacion.CicloArchivoId='14';f.attempts[0].evidencia_doc=JSON.stringify(r);},'CYCLE_CHANGED'],
  ['identificador fisico',f=>f.badField(),'PHYSICAL_IDENTIFIER_INVALID']
])test('DOC111: control rechaza '+name+' discordante sin PASS (SELECT sintetico)',async()=>{const f=selectFixture();change(f);await assert.rejects(f.controls.verify('20',f.p),new RegExp(code));});

test('DOC111: driver usa POST compartido sin retries y no expone excepciones de transporte',async()=>{
  const p=profile(),calls=[];const auth={page:{locator:()=>({getAttribute:async()=>'synthetic-token'})},context:{request:{post:async(url,options)=>{calls.push(options);throw new Error('COOKIE_PRIVATE_SENTINEL');}}}};
  const authorization=Object.fromEntries(['environment','account','effects','discardable','select'].map(k=>[k,true]));
  const d=createDriver(auth,p,authorization);await d.refreshToken();await assert.rejects(d.request('ConsultarLote',{loteId:'20'}),e=>!e.message.includes('COOKIE_PRIVATE_SENTINEL')&&e.message.includes('Sin reintento'));assert.equal(calls.length,1);assert.equal(calls[0].maxRetries,0);assert.equal(calls[0].maxRedirects,0);
  await assert.rejects(d.request('Almacenamiento',{loteId:'20'}),/OPERATION_INVALID/);assert.equal(calls.length,1);
});

test('DOC117: control compartido verifica una raiz con nombre y fecha propios',async()=>{const f=selectFixture();f.lot.total_raices=1;f.lot.total_archivos=1;f.lot.total_bytes=7;f.roots.splice(1);f.files.splice(1);f.attempts.splice(2);f.p.expectations.splice(1);f.physical.splice(1);f.exps.splice(1);for(const k of ['NOMBRE','SEGUNDO_NOMBRE_DOCUMENTO','segundo_nombre'])f.physical[0][k]='DOC117-79.pdf';f.exps[0].doc117_fecha_inicial='2026-11-05';const options={singleRoot:true,fileNames:['DOC117-79.pdf'],expectedDates:['2026-11-05']};const r=await f.controls.verify('20',f.p,options);assert.equal(r.roots,1);assert.equal(r.documents,1);f.exps[0].doc117_fecha_inicial='2026-11-04';await assert.rejects(f.controls.verify('20',f.p,options),/EXPEDITION_DATE_INVALID/);});
test('DOC117: control compartido no permite reducir raiz parcial ni ocultar nombres',async()=>{const f=selectFixture();await assert.rejects(f.controls.verify('20',{...f.p,mode:'PARCIAL'},{singleRoot:true}),/CONTROL_SCOPE_INVALID/);await assert.rejects(f.controls.verify('20',f.p,{fileNames:['one.pdf']}),/CONTROL_SCOPE_INVALID/);await assert.rejects(f.controls.verify('20',f.p,{other:true}),/CONTRACT_INVALID/);});
test('DOC117: transporte compartido usa nombre original y rechaza rutas antes de enviar',async()=>{const seen=[];const p=profile(),authorization=Object.fromEntries(['environment','account','effects','discardable','select'].map(k=>[k,true]));const auth={page:{locator:()=>({getAttribute:async()=>'synthetic-token'})},context:{request:{post:async(url,options)=>{seen.push(options);return{status:()=>200,json:async()=>({ok:true,datos:{loteId:'20',archivoId:'30',estadoRecepcion:'RECIBIDO'}})}}}}};const driver=createDriver(auth,p,authorization);await driver.refreshToken();await driver.receive('20',{recursoId:'30'},Buffer.from('PDF'),'DOC117-79.pdf');assert.equal(seen[0].multipart.file.name,'DOC117-79.pdf');assert.equal(seen[0].maxRetries,0);await assert.rejects(driver.receive('20',{recursoId:'30'},Buffer.from('PDF'),'../other.pdf'),/UPLOAD_NAME_INVALID/);assert.equal(seen.length,1);});
test('DOC117: referencia conocida se retiene aun si la respuesta de confirmacion es discordante',async()=>{const references=[],p=profile(),authorization=Object.fromEntries(['environment','account','effects','discardable','select'].map(k=>[k,true]));const auth={page:{locator:()=>({getAttribute:async()=>'synthetic-token'})},context:{request:{post:async()=>({status:()=>409,json:async()=>({ok:false,codigo:'RESULTADO_INCIERTO',datos:{loteId:'88'}})})}}};const driver=createDriver(auth,p,authorization,{onReference:r=>references.push(r)});await driver.refreshToken();await assert.rejects(driver.request('ConfirmarPlan',{}),/OPERATION_NOT_CONFIRMED/);assert.deepEqual(references,[{loteId:'88'}]);});
