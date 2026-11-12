'use strict';
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const {collectValue,requireInteractiveConsole}=require('./support/interactive-e2e-console.cjs');
const {queryRows}=require('./support/doc32-e2e-odbc.cjs');
const {openSession,createDriver,loginObservation}=require('../tests/support/doc111-importacion-carpetas.cjs');
const {captureCodeEvidence}=require('../tests/support/doc110-code-evidence.cjs');
const {selectSecondAccount}=require('../tests/support/doc119-second-accounts.cjs');
const {readPrepared,setupTransport}=require('../tests/support/doc119-acceptance-data.cjs');
const {BASE_URL,LIMITS,check,id,hash,validateConnection,installGuard,discoverImports,discoverLevels,makeControls,runSeg01Owner,runSeg01Other,runSeg02}=require('../tests/support/doc119-seguimiento.cjs');
const ROOT=path.resolve(__dirname,'../../..');
const SOURCES=['js/gestion/ImportacionCarpetas/ImportacionCarpetas.js','js/gestion/ImportacionCarpetas/ImportacionCarpetasVista.js','js/gestion/ImportacionCarpetas/ImportacionCarpetas.css'];
const SETUP_LIMITS={http:400,select:40,sessions:1};
async function main(args=process.argv.slice(2)){
  const prepared=readPrepared(ROOT),manifest=prepared.fixture.request;
  if(args.length===1&&args[0]==='--check'){console.log('DOC119_DATA_CHECK=PASS; FILES=27; REAL_EXECUTION=false');return;}
  check(args.length===3&&args[0]==='--run'&&/^[a-f0-9]{32}$/.test(args[1])&&args[2]==='--authorized-plan','ARGUMENT_INVALID');
  requireInteractiveConsole();process.title='DOC-119 - datos y aceptacion - '+args[1].slice(0,8);
  const directory=path.join(ROOT,'.tmp','doc119-datos-aceptacion',args[1]);fs.mkdirSync(directory,{recursive:true});
  const statePath=path.join(directory,'state.json');
  const state={runId:args[1],pid:process.pid,authorizedPlan:true,authorizationRevision:'20261112-setup-approved',baseUrl:BASE_URL,status:'WAITING_ACCOUNT',limits:{SETUP:SETUP_LIMITS,...LIMITS},usage:{},results:{},operations:{},resources:{level:'7',ownerRef:'141',companyRef:'2',versionId:'40'},events:[]};
  fs.writeFileSync(statePath,JSON.stringify(state),{flag:'wx'});
  const save=patch=>{Object.assign(state,patch);fs.writeFileSync(statePath,JSON.stringify(state,null,2));};
  const event=status=>{state.events.push({status,utc:new Date().toISOString()});save({status});console.log('DOC119_PROGRESS='+status);};
  const budgets={};
  const budget=stage=>budgets[stage]||(budgets[stage]={counts:{http:0,select:0,sessions:0},take(kind){check(Object.hasOwn(this.counts,kind)&&this.counts[kind]<state.limits[stage][kind],'BUDGET_EXHAUSTED');this.counts[kind]++;state.usage[stage]={...this.counts};save({});}});
  const secrets={},input={},contexts=new Set();let environment,browser,principal,alternate;
  try{
    console.log('Preparacion AUTORIZADA: 1 lote, 27 archivos en 2 raices, recepcion de 1 PDF de 677 bytes y creacion de 1 expediente. Sin almacenar documentos ni borrar.');
    console.log('Despues: pruebas de lectura, paginacion/CSV/privacidad y recuperacion sin enviar los 26 PDF restantes.');
    console.log('Ambiente: '+BASE_URL);
    await collectValue(input,'ra','Conexion RA',{defaultValue:'MembershipUsers'});await collectValue(input,'da','Conexion DA',{defaultValue:'MembershipUsers'});
    await collectValue(input,'tls','Aceptar certificado local: SI o NO',{defaultValue:'NO'});check(['SI','NO'].includes(input.tls.toUpperCase()),'TLS_CHOICE_INVALID');
    const connection=validateConnection({baseUrl:BASE_URL,module:'GESTOR',ignoreHttpsErrors:input.tls.toUpperCase()==='SI',timeoutMilliseconds:60000,odbcDsn:input.ra,docuarchiOdbcDsn:input.da});save({tlsException:connection.ignoreHttpsErrors});
    await collectValue(secrets,'ownerUser','Cuenta propietaria de las importaciones (enmascarada)',{secret:true});await collectValue(secrets,'ownerPassword','Clave de aplicacion',{secret:true});
    await collectValue(secrets,'dbUser','Cuenta MySQL exclusivamente SELECT',{secret:true});await collectValue(secrets,'dbPassword','Clave MySQL SELECT',{secret:true});
    environment=Object.fromEntries(Object.entries(process.env).filter(([key])=>!/^DOC\d+(?:_DA)?_E2E/.test(key)));
    Object.assign(environment,{DOC111_E2E_MODULE:'GESTOR',DOC111_E2E_ODBC_DSN:connection.odbcDsn,DOC111_DA_E2E_ODBC_DSN:connection.docuarchiOdbcDsn});
    for(const prefix of ['DOC111_E2E','DOC111_DA_E2E'])Object.assign(environment,{[prefix+'_MYSQL_USER']:secrets.dbUser,[prefix+'_MYSQL_PASSWORD']:secrets.dbPassword});
    delete process.env.DEBUG;delete process.env.PWDEBUG;delete environment.DEBUG;delete environment.PWDEBUG;
    save({code:captureCodeEvidence({files:[...SOURCES,'tools/e2e/scripts/run-doc119-datos-aceptacion.cjs','tools/e2e/tests/support/doc119-acceptance-data.cjs','tools/e2e/tests/support/doc119-seguimiento.cjs','bin/GestionDocumental-Docuarchi.net.dll']})});
    const query=(origin,sql,parameters)=>queryRows(sql,parameters,environment,origin==='RA'?'DOC111_E2E':'DOC111_DA_E2E');
    event('PREFLIGHT_SETUP');const b=budget('SETUP');
    const catalog=await discoverImports(query,b,secrets.ownerUser);principal=catalog.principal;
    check(principal.userRef==='141'&&principal.companyRef==='2','RESOURCE_OWNER_CHANGED');
    const levels=await discoverLevels(query,b,principal,'7');check(levels.length>0,'ALTERNATE_LEVEL_REQUIRED');alternate=levels[0];save({resources:{...state.resources,alternateLevel:alternate.id}});
    browser=await require('../node_modules/playwright').chromium.launch({headless:true});
    const open=async(role,stage,scope)=>{
      const current=budget(stage);current.take('sessions');let guard;save({activeRole:role});
      Object.assign(environment,{DOC111_E2E_USER:secrets[role+'User'],DOC111_E2E_PASSWORD:secrets[role+'Password']});
      const guarded={newContext:async options=>{const context=await browser.newContext(options);contexts.add(context);guard=await installGuard(context,current,scope);return context;}};
      let auth;
      try{auth=await openSession(guarded,{...connection,nivelRef:'7'},environment,{environment:true,account:true},{beforeDiagnosticPost(){current.take('http');}});}
      catch(error){save({guardViolation:guard?.violations[0]||null,guardRequest:guard?.firstViolation||null});if(guard?.violations.length)throw Error(guard.violations[0]);throw error;}
      auth.page.setDefaultTimeout(30000);auth.guard=guard;
      for(const source of SOURCES){current.take('http');const response=await auth.context.request.get(new URL(source,BASE_URL).href,{maxRetries:0,maxRedirects:0,timeout:30000});check(response.status()===200&&hash(await response.body())===hash(fs.readFileSync(path.join(ROOT,source))),'SERVED_SOURCE_MISMATCH');}
      check(guard.violations.length===0,'HTTP_SCOPE_DENIED');return auth;
    };
    const scope={lotId:null,level:'7',planId:null,allowTree:false,alternateLevel:null};
    const auth=await open('owner','SETUP',scope);
    const refs={};
    const transport=setupTransport(auth.context.request,b,manifest,refs,operation=>{state.operations[operation]=(state.operations[operation]||0)+1;save({});},failure=>save({lastServiceFailure:failure}));
    const driver=createDriver({page:auth.page,context:{request:transport}},{...connection,nivelRef:'7'},{environment:true,account:true,effects:true,discardable:true,select:true},{onReference:reference=>{Object.assign(scope,reference);save({resources:{...state.resources,...reference}});}});
    await driver.refreshToken();const opened=await driver.request('AbrirImportacion',{nivelRef:'7'});check(opened.nivelRef==='7','DESTINATION_CHANGED');
    event('PREPARING_PLAN');
    const plan=await driver.request('PrepararPlan',manifest);
    check(id(plan.planId)&&plan.nivelRef==='7'&&plan.snapshot?.VersionId==='40'&&plan.snapshot.Configuracion.ModoIntegridad==='COMPLETO_OBLIGATORIO'&&plan.snapshot.Configuracion.AmbitoIntegridad==='EXPEDIENTE'&&String(plan.snapshot.Configuracion.GabineteId)==='10','PLAN_SCOPE_CHANGED');
    const result=plan.resultado;
    save({planReview:{eligibleRoots:result?.UnidadesElegibles?.length||0,blockingIncidents:result?.Incidencias?.filter(i=>i.BloqueaUnidad).length??null,incidentCodes:[...new Set((result?.Incidencias||[]).map(i=>i.Codigo).filter(v=>/^[A-Z0-9_]{1,80}$/.test(v)))]}});
    check(result?.PuedeConfirmar===true&&result.UnidadesElegibles.length===2&&['r1','r2'].every(r=>result.UnidadesElegibles.includes(r))&&Array.isArray(result.Incidencias)&&!result.Incidencias.some(i=>i.BloqueaUnidad),'PREFLIGHT_NOT_ELIGIBLE');
    const rootValues=result.Elementos.filter(e=>e.Ambito==='RAIZ');
    check(rootValues.length===2&&rootValues.every(e=>['CODIGO','NOMBRE'].every(field=>e.Valores.filter(v=>v.DestinoRef===field&&v.Valor===manifest.raices[0].nombre).length===1)),'PLAN_VALUES_CHANGED');
    refs.confirmation={planId:plan.planId,revisionEsperada:plan.revision,planHashEsperado:plan.planHash,claveConfirmacion:crypto.randomBytes(16).toString('hex')};
    event('CONFIRMING_ONE_LOT');const lot=await driver.request('ConfirmarPlan',refs.confirmation);check(id(lot.loteId)&&lot.nivelRef==='7','CONFIRMATION_INVALID');refs.lotId=lot.loteId;
    let roots=await driver.pages(lot.loteId,'RAICES'),files=await driver.pages(lot.loteId,'ARCHIVOS');
    check(roots.length===2&&files.length===27&&new Set(roots.map(r=>r.idCliente)).size===2&&manifest.archivos.every(f=>files.filter(row=>row.idCliente===f.idCliente).length===1),'ORIGINAL_TOTALS_CHANGED');
    const receipt=files.find(f=>f.idCliente==='a1');check(receipt&&id(receipt.recursoId),'FILE_CORRELATION_INVALID');refs.receiptFileId=receipt.recursoId;
    event('RECEIVING_ONE_PDF');await driver.receive(lot.loteId,receipt,prepared.files.get('a1').bytes,manifest.archivos[0].nombre);
    roots=await driver.pages(lot.loteId,'RAICES');const root=roots.find(r=>r.idCliente==='r1');check(root&&id(root.recursoId),'ROOT_CORRELATION_INVALID');
    refs.creation={loteId:lot.loteId,recursoId:root.recursoId,fase:'CREAR_EXPEDIENTE',revisionEsperada:root.revision,claveOperacion:crypto.randomBytes(16).toString('hex')};
    event('CREATING_ONE_EXPEDITION');const created=await driver.request('AvanzarLote',refs.creation);
    check(created.estado==='CONFIRMADO'&&created.loteId===lot.loteId&&created.raizId===root.recursoId&&created.fase==='CREAR_EXPEDIENTE'&&id(created.expedienteId)&&id(created.relacionNivelId),'CREATION_NOT_CONFIRMED');
    check(auth.guard.violations.length===0,'HTTP_SCOPE_DENIED');await auth.context.close();
    const setup=await makeControls(query,b).snapshot(lot.loteId);
    check(setup.context.UsuarioRef==='141'&&setup.context.AmbitoRef==='2'&&setup.context.NivelRef==='7'&&setup.data.files.length===27&&setup.data.roots.length===2&&setup.data.files.filter(f=>f.estado_recepcion==='PENDIENTE'&&f.estado_efecto==='NO_INICIADO'&&!f.intento_activo_id).length===26&&setup.data.files.filter(f=>f.estado_recepcion==='RECIBIDO').length===1&&setup.documents===0&&setup.data.attempts.length===1&&setup.data.attempts[0].fase==='CREAR_EXPEDIENTE'&&setup.data.attempts[0].estado==='CONFIRMADO','SETUP_POSTCONDITION_FAILED');
    save({results:{SETUP:{status:'PASS',files:27,pendingFiles:26,receivedFiles:1,receivedBytes:677,createdExpeditions:1,storedDocuments:0,fingerprint:setup.fingerprint,usage:{...b.counts}}}});
    const profile={...connection,lotId:lot.loteId,recoveryLotId:lot.loteId,alternateLevel:alternate.id};
    const runStage=async(stage,work)=>{
      event('RUNNING_'+stage);const current=budget(stage),controls=makeControls(query,current),before=await controls.snapshot(lot.loteId);
      check(before.context.UsuarioRef==='141'&&before.context.AmbitoRef==='2'&&before.context.NivelRef==='7','RESOURCE_OWNER_CHANGED');
      const readScope={lotId:lot.loteId,level:'7',planId:before.planId,allowTree:stage==='SEG-02',alternateLevel:alternate.id};
      const stageOpen=role=>open(role,stage,readScope);
      const result=await work(before,stageOpen,current);
      const after=await controls.snapshot(lot.loteId);check(before.fingerprint===after.fingerprint,'READ_CHANGED_BUSINESS_STATE');
      const limited=stage==='SEG-01'?!result.pagination:!result.homonymousRootsObserved||!result.newRelationsAdded;
      state.results[stage]={status:limited?'PASS_READONLY_WITH_LIMITS':'PASS_READONLY',...result,noBusinessMutation:true,daScope:before.daScope,beforeFingerprint:before.fingerprint,afterFingerprint:after.fingerprint,usage:{...current.counts}};save({});
    };
    await runStage('SEG-01',async(before,stageOpen,current)=>{
      const owner=await stageOpen('owner');const result=await runSeg01Owner(owner.page,lot.loteId,before,owner.guard);check(owner.guard.violations.length===0,'HTTP_SCOPE_DENIED');await owner.context.close();save({ownerChecks:result});
      const candidate=await selectSecondAccount(async(sql,params)=>{current.take('select');return query('RA',sql,params);},lot.loteId,async candidates=>candidates.find(c=>c.userRef==='136'));
      check(candidate.userRef==='136'&&candidate.companyRef==='2','SECOND_ACCOUNT_SCOPE_CHANGED');secrets.otherUser=candidate.login;save({secondAccount:{userRef:'136',companyRef:'2',source:'CURRENT_RA_SELECT'}});
      console.log('Cuenta de privacidad verificada: '+candidate.login);await collectValue(secrets,'otherPassword','Clave de aplicacion de Carolina (enmascarada)',{secret:true});
      const other=await stageOpen('other');const denial=await runSeg01Other(other.page,lot.loteId);check(other.guard.violations.length===0,'HTTP_SCOPE_DENIED');await other.context.close();return {...result,...denial};
    });
    await runStage('SEG-02',async(before,stageOpen)=>{
      const pending=before.data.files.find(f=>f.estado_recepcion==='PENDIENTE'&&f.estado_efecto==='NO_INICIADO'&&!f.intento_activo_id);check(pending,'PENDING_RECEPTION_RESOURCE_REQUIRED');
      const origin=JSON.parse(pending.origen_doc),file=manifest.archivos.find(f=>f.nombre===origin.Nombre);check(file&&prepared.files.has(file.idCliente),'FILE_CORRELATION_INVALID');
      const owner=await stageOpen('owner');const result=await runSeg02(owner.page,profile,before,prepared.files.get(file.idCliente).path);check(owner.guard.violations.length===0,'HTTP_SCOPE_DENIED');await owner.context.close();return result;
    });
    event(['SEG-01','SEG-02'].every(stage=>state.results[stage].status==='PASS_READONLY')?'FINISHED':'FINISHED_WITH_LIMITS');
  }catch(error){
    const code=/^(?:DOC119|DOC111|DOC113)_[A-Z0-9_]{3,100}$/.test(error?.message)?error.message:'DOC119_SETUP_STOPPED';
    const observation=loginObservation(error?.loginObservation);save({failure:{code,...observation?{loginObservation:observation}:{},...error?.operation?{operation:error.operation,httpStatus:error.httpStatus,serverCode:error.serverCode}:{}}});event('STOPPED');process.exitCode=2;
  }finally{
    for(const context of contexts)await context.close().catch(()=>{});if(browser)await browser.close().catch(()=>{});
    for(const key of Object.keys(secrets))delete secrets[key];if(environment)for(const key of Object.keys(environment))if(/^DOC\d+(?:_DA)?_E2E/.test(key))delete environment[key];
    save({closed:true,credentialsCleared:true,finishedUtc:new Date().toISOString()});console.log('DOC119_RESULT='+JSON.stringify({runId:state.runId,status:state.status,results:state.results,usage:state.usage,failure:state.failure?.code||null}));
  }
}
if(require.main===module)main().catch(()=>{console.error('DOC119_DATA_PRECHECK_STOPPED');process.exitCode=2;});
module.exports={main};
