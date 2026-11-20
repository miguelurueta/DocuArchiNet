'use strict';
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const {collectConfirmation,collectValue,requireInteractiveConsole,runChild}=require('./support/interactive-e2e-console.cjs');
const {validateSession}=require('../tests/support/doc115-recepcion-importacion.cjs');
const fixture=require('../fixtures/doc115-recepcion-cases.json');
function parseArgs(args){const result={check:false};for(let i=0;i<args.length;i++){if(args[i]==='--check'&&!result.check){result.check=true;continue;}if(args[i]==='--session'&&!result.session&&args[i+1]&&!args[i+1].startsWith('--')){result.session=args[++i];continue;}throw Error('DOC115_ARGUMENT_INVALID');}if(!result.session)throw Error('DOC115_SESSION_REQUIRED');return result;}
function safeLine(line,scenario){
  const prefix=['DOC115_REFERENCE=','DOC115_RESULT=','DOC115_FAILURE='].find(p=>String(line).startsWith(p));if(!prefix)return null;
  let v;try{v=JSON.parse(line.slice(prefix.length));}catch{return null;}if(v?.scenario!==scenario)return null;
  if(prefix==='DOC115_REFERENCE='){const id=x=>typeof x==='string'&&/^[1-9][0-9]{0,9}$/.test(x)&&BigInt(x)<=4294967295n;if(!id(v.planId)||v.loteId!==undefined&&!id(v.loteId))return null;return {prefix,value:{scenario,planId:v.planId,...(v.loteId?{loteId:v.loteId}:{})}};}
  if(prefix==='DOC115_FAILURE=')return ['PRECHECK','LOGIN','RECIB'].includes(v.stage)&&/^DOC115_[A-Z0-9_]{3,70}$/.test(v.code)?{prefix,value:{scenario,stage:v.stage,code:v.code}}:null;
  if(v.status!=='PASS'||v.noDocumentEffects!==true||v.readOnlyControls!==true||![v.preparations,v.confirmations,v.uploads,v.lots].every(x=>Number.isInteger(x)&&x>=0&&x<=16)||!/^[a-f0-9]{40}$/.test(v.head)||!['codeSha256','backendSha256'].every(k=>/^[a-f0-9]{64}$/.test(v[k]))||v.httpConcurrency!=='SESSION_MAY_SERIALIZE'||![null,'CLIENT_RESULT_DISCARDED'].includes(v.responseLoss))return null;
  if(v.preservedLotsUnchanged!==undefined&&v.preservedLotsUnchanged!==true)return null;
  if(v.scenario==='RECIB-03'&&(v.interruptedTransferVerified!==true||v.responseLoss!=='CLIENT_RESULT_DISCARDED'))return null;
  return {prefix,value:Object.fromEntries(['scenario','status','preparations','confirmations','uploads','lots','noDocumentEffects','readOnlyControls','httpConcurrency','responseLoss',...(v.interruptedTransferVerified===true?['interruptedTransferVerified']:[]),...(v.preservedLotsUnchanged===true?['preservedLotsUnchanged']:[]),'head','codeSha256','backendSha256'].map(k=>[k,v[k]]))};
}
async function main(args=process.argv.slice(2),runtime={}){
  if(args[0]==='--prepare-files-fixture')return prepareRepositoryFiles(args.slice(1));
  if(args[0]==='--repository-files-fixture')return repositorySegmented(args.slice(1),runtime,true,false,true);
  if(args[0]==='--repository-segmented')return repositorySegmented(args.slice(1),runtime);
  if(args[0]==='--repository-files')return repositorySegmented(args.slice(1),runtime,true);
  if(args[0]==='--repository-files-inspect')return repositorySegmented(args.slice(1),runtime,true,true);
  const parsed=parseArgs(args),file=path.resolve(parsed.session);if(fs.statSync(file).size>1024*1024)throw Error('DOC115_SESSION_TOO_LARGE');
  const bytes=fs.readFileSync(file),s=validateSession(JSON.parse(bytes.toString('utf8').replace(/^\uFEFF/,''))),budget=fixture.scenarios[s.scenario];
  if(parsed.check){console.log('DOC115_CHECK='+JSON.stringify({status:'PASS',scenario:s.scenario,...budget,resourcesSha256:crypto.createHash('sha256').update(bytes).digest('hex'),realExecution:false}));return;}
  (runtime.requireInteractiveConsole||requireInteractiveConsole)();const auth={},secrets={};let environment;
  try{
    const confirm=runtime.collectConfirmation||collectConfirmation,collect=runtime.collectValue||collectValue;
    console.log('DOC-115 '+s.scenario+': '+budget.preparations+' preparaciones, hasta '+budget.maximumLots+' lotes y '+budget.uploads+' solicitudes de recepcion. Recursos conservados; cero efectos documentales.');
    const scopes=[['environment','Autoriza este ambiente de pruebas DOC-115'],['account','Autoriza la cuenta operativa de pruebas'],['select','Autoriza controles RA/DA exclusivamente SELECT'],['effects','Autoriza los recursos y solicitudes del archivo revisado'],['discardable','Confirma que perfiles, nombres y archivos son recursos de prueba autorizados; se conservaran los planes y lotes']];
    if(s.scenario==='RECIB-03')scopes.push(['interruption','Autoriza interrumpir una transferencia HTTP y descartar deliberadamente un resultado recibido en estos recursos']);if(s.profile.ignoreHttpsErrors)scopes.push(['tls','Autoriza el certificado de este ambiente de pruebas']);
    for(const [k,label]of scopes)await confirm(auth,k,label);
    await collect(secrets,'user','Cuenta autorizada (visible solo aqui)',{secret:false});await collect(secrets,'password','Contraseña',{secret:true});
    await collect(secrets,'dbUser','Cuenta MySQL exclusivamente SELECT para los DSN registrados',{secret:true});await collect(secrets,'dbPassword','Contraseña MySQL de solo lectura',{secret:true});
    const backend=path.join(s.runtimeRoot,'bin/GestionDocumental-Docuarchi.net.dll'),backendSha256=crypto.createHash('sha256').update(fs.readFileSync(backend)).digest('hex');
    environment=Object.fromEntries(Object.entries(process.env).filter(([k])=>!/^DOC11[0-5]_/.test(k)));
    Object.assign(environment,{DOC115_E2E_AUTHORIZED:'true',DOC115_SCENARIO:s.scenario,DOC115_SESSION:JSON.stringify(s),DOC115_RUN_ID:crypto.randomBytes(16).toString('hex'),DOC115_BACKEND_SHA256:backendSha256,DOC111_E2E_MODULE:s.profile.module,DOC111_E2E_USER:secrets.user,DOC111_E2E_PASSWORD:secrets.password,DOC111_E2E_ODBC_DSN:s.profile.odbcDsn,DOC111_DA_E2E_ODBC_DSN:s.profile.docuarchiOdbcDsn});
    for(const prefix of ['DOC111_E2E','DOC111_DA_E2E'])Object.assign(environment,{[prefix+'_MYSQL_USER']:secrets.dbUser,[prefix+'_MYSQL_PASSWORD']:secrets.dbPassword});
    for(const [k]of scopes){if(auth[k]!=='true')throw Error('DOC115_AUTHORIZATION_REQUIRED');environment['DOC115_AUTH_'+k.toUpperCase()]='true';}
    const results=[];const child=await (runtime.runChild||runChild)(process.execPath,[path.resolve(__dirname,'../node_modules/@playwright/test/cli.js'),'test','tests/doc115-recepcion-importacion.spec.cjs','--grep',s.scenario,'--workers=1','--retries=0'],path.resolve(__dirname,'..'),environment,{nonInteractiveChild:true,redactOutput:text=>String(text).split(/\r?\n/).map(line=>{const safe=safeLine(line,s.scenario);if(!safe)return '';if(safe.prefix==='DOC115_RESULT=')results.push(safe.value);return safe.prefix+JSON.stringify(safe.value)+'\n';}).join('')});
    if(child.code!==0||results.length!==1||results[0].preparations!==budget.preparations||results[0].lots>budget.maximumLots||results[0].uploads>budget.uploads||results[0].backendSha256!==backendSha256)throw Error('DOC115_SESSION_STOPPED');return results[0];
  }finally{for(const k of Object.keys(secrets))delete secrets[k];if(environment)for(const k of Object.keys(environment))if(/^DOC11[0-5]_/.test(k))delete environment[k];}
}
async function repositorySegmented(args,runtime={},files=false,inspect=false,fixture=false){
  const parsed=parseArgs(args),file=path.resolve(parsed.session),root=path.resolve(__dirname,'../../..');
  if(fs.statSync(file).size>1024*1024)throw Error('DOC120_SEGMENTED_SCOPE_INVALID');
  const bytes=fs.readFileSync(file),s=JSON.parse(bytes.toString('utf8').replace(/^\uFEFF/,''));
  if(s.scenario!==(fixture?'DOC120-FILES-FIXTURE':files?'DOC120-FILES-REPOSITORY':'DOC120-SEGMENTED-REPOSITORY')||!/^[a-f0-9]{32}$/.test(s.runId)||path.resolve(s.runtimeRoot)!==root||s.files?.length!==2||files&&!fixture&&(s.lotRef!=='50'||s.files.join(',')!=='122,123')||fixture&&(s.profileRef!=='24'||s.versionRef!=='38'||s.levelRef!=='7'||s.files.join(',')!=='a1,a2'))throw Error('DOC120_SEGMENTED_SCOPE_INVALID');
  if(Boolean(s.inspectOnly)!==inspect)throw Error('DOC120_FILES_INSPECTION_SCOPE_REQUIRED');
  const env=Object.fromEntries(Object.entries(process.env).filter(([k])=>!/^DOC(?:11[0-9]|120)_/.test(k)&&!['DEBUG','PWDEBUG','NODE_DEBUG','NODE_DEBUG_NATIVE'].includes(k))),secrets={};
  const command=['-NoProfile','-ExecutionPolicy','Bypass','-File',path.join(__dirname,'run-doc111-persistence.ps1'),'-Mode',fixture?'Doc120FilesFixture':inspect?'Doc120FilesInspect':files?'Doc120Files':'Doc120Segmented','-ResourcesFile',file,'-RuntimeRoot',root];
  const output=text=>String(text).split(/\r?\n/).filter(line=>/^DOC115_07A_(?:RESULT|FAILURE)=/.test(line)||/^DOC120_SEGMENTED_CHECK=PASS_NOT_EXECUTED$/.test(line)).join('\n')+'\n';
  const run=runtime.runChild||runChild;
  let consoleState,consoleFile;
  const saveConsole=()=>{if(consoleFile)fs.writeFileSync(consoleFile,JSON.stringify(consoleState,null,2));};
  try{
    if(parsed.check){const result=await run('powershell.exe',[...command,'-Check'],root,env,{nonInteractiveChild:true,redactOutput:output});if(result.code!==0)throw Error('DOC120_SEGMENTED_CHECK_FAILED');return result;}
    if(s.approvedByUser!==true)throw Error('DOC120_SEGMENTED_AUTHORIZATION_REQUIRED');
    (runtime.requireInteractiveConsole||requireInteractiveConsole)();
    consoleFile=file+'.console-state.json';fs.closeSync(fs.openSync(consoleFile,'wx'));
    consoleState={runId:s.runId,status:'WAITING_PRIVATE_CREDENTIALS',pid:process.pid,credentialsCleared:false,startedUtc:new Date().toISOString()};saveConsole();
    // The user approved this bounded window in the active conversation. The
    // console captures credentials only; it does not request that approval again.
    if(fixture){
      console.log('DOC-120 3.187 autorizado: perfil E2E24-v38/nivel7/141-2; un lote nuevo,2 PDF de677 bytes,1 expediente y cero documentos almacenados.');
      console.log('Login/recepcion/creacion DOC111 existentes; luego reserva concurrente,3 rollbacks y2 cierres propios. Recursos conservados, sin replay.');
      console.log('Maximo600 HTTP/1 sesion/12 SELECT de preparacion incluidos en96 del envoltorio. Sin ALMACENAR ni cambios de perfil.');
    }else if(inspect){
      console.log('DOC-120: revision exclusivamente SELECT de lotes E2E29/50, propietario141/empresa2. Maximo12 consultas;0 conexiones escritoras o mutaciones.');
    }else if(files){
      console.log('DOC-120 3.187 aprobado: lote E2E50/plan90/perfil28-version45/nivel223/usuario141/empresa2; archivos122 y123.');
      console.log('Solo control nativo:2 reservas y2 cierres propios anteriores a invocacion;3 rollbacks. Quedan incidencias ORIGINAL_NO_COMPROBADO de prueba e historial.');
      console.log('0 invocaciones al escritor, PDF enviados, planes/lotes/documentos/expedientes nuevos o DDL. No se retiran originales.');
    }else{
      console.log('DOC-120 aprobado: perfil E2E24/nivel7/usuario141/empresa2; publicacion vigente validada por SELECT.');
      console.log('Dos PDF: solo metadatos. Maximo1 preparacion/plan/lote/raiz,2 archivos,3 rollbacks;0 PDF enviados/documentos/expedientes.');
    }
    if(!inspect)console.log('48 llamadas de repositorio,64 SELECT nativos+96 del envoltorio;15min de admision por etapa. Sin reintentos ni cambios de perfil.');
    const collect=runtime.collectValue||collectValue;
    await collect(secrets,'readUser','Cuenta MySQL de controles SELECT',{secret:true});
    await collect(secrets,'readPassword','Clave MySQL SELECT',{secret:true});
    if(!inspect){
      await collect(secrets,'writeUser','Cuenta MySQL de prueba con escritura en tablas de importacion',{secret:true});
      await collect(secrets,'writePassword','Clave MySQL de prueba',{secret:true});
    }
    if(fixture){await collect(secrets,'appUser','Cuenta operativa E2E141',{secret:true});await collect(secrets,'appPassword','Clave operativa E2E',{secret:true});}
    Object.assign(env,{DOC120_SEGMENTED_AUTHORIZED:'true',DOC120_SEGMENTED_RESOURCES_SHA256:crypto.createHash('sha256').update(bytes).digest('hex'),
      DOC111_E2E_MYSQL_USER:secrets.readUser,DOC111_E2E_MYSQL_PASSWORD:secrets.readPassword,
      ...(!inspect?{DOC120_SEGMENTED_WRITER_USER:secrets.writeUser,DOC120_SEGMENTED_WRITER_PASSWORD:secrets.writePassword}:{})});
    if(fixture)Object.assign(env,{DOC111_E2E_MODULE:'GESTOR',DOC111_E2E_USER:secrets.appUser,DOC111_E2E_PASSWORD:secrets.appPassword,DOC111_E2E_ODBC_DSN:s.odbcDsn});
    consoleState.status='RUNNING';saveConsole();
    const result=await run('powershell.exe',command,root,env,{nonInteractiveChild:true,redactOutput:output});
    if(result.code!==0)throw Error('DOC120_SEGMENTED_STOPPED_REVIEW_REQUIRED');consoleState.status='FINISHED';return result;
  }finally{for(const k of Object.keys(secrets))delete secrets[k];for(const k of Object.keys(env))delete env[k];if(consoleState){if(consoleState.status!=='FINISHED')consoleState.status='STOPPED';consoleState.credentialsCleared=true;consoleState.finishedUtc=new Date().toISOString();saveConsole();}}
}
async function prepareRepositoryFiles(args){
  const parsed=parseArgs(args),file=path.resolve(parsed.session),bytes=fs.readFileSync(file),scope=JSON.parse(bytes.toString('utf8').replace(/^\uFEFF/,''));
  const check=(ok,code)=>{if(!ok)throw Error('DOC120_FILES_'+code);};
  check(!parsed.check&&scope.scenario==='DOC120-FILES-FIXTURE'&&scope.approvedByUser===true&&process.env.DOC120_SEGMENTED_AUTHORIZED==='true'&&crypto.createHash('sha256').update(bytes).digest('hex')===process.env.DOC120_SEGMENTED_RESOURCES_SHA256,'AUTHORIZATION_REQUIRED');
  check(scope.profileRef==='24'&&scope.versionRef==='38'&&scope.levelRef==='7'&&scope.userRef==='141'&&scope.companyRef==='2'&&/^[a-f0-9]{32}$/.test(scope.runId),'SCOPE_INVALID');
  const support=require('../tests/support/doc111-importacion-carpetas.cjs');
  const profile=support.validateProfile(JSON.parse(fs.readFileSync(path.join(__dirname,'../profiles/doc111-importacion-carpetas-lote.runtime.json'),'utf8').replace(/^\uFEFF/,'')));
  check(profile.baseUrl===scope.baseUrl&&profile.odbcDsn===scope.odbcDsn&&profile.nivelRef==='7'&&profile.versionId==='38','PROFILE_CHANGED');
  profile.pdfFixture='doc111-carga-individual.pdf';const pdf=support.readFixture(profile);
  check(pdf.length===677&&crypto.createHash('sha256').update(pdf).digest('hex')===scope.fixtureSha256,'FIXTURE_CHANGED');
  const stateFile=file+'.fixture-state.json';fs.closeSync(fs.openSync(stateFile,'wx'));
  const state={runId:scope.runId,status:'STARTING',http:0,selects:0,references:[],storageCalls:0,sessions:0},started=Date.now();
  const save=()=>fs.writeFileSync(stateFile,JSON.stringify(state,null,2));save();
  const admit=()=>check(Date.now()-started<900000,'FIXTURE_WINDOW_EXPIRED');
  const http=()=>{admit();check(++state.http<=600,'HTTP_BUDGET');save();};
  let browser,auth;
  try{
    const {chromium}=require('../node_modules/playwright');browser=await chromium.launch({headless:true});
    const boundedBrowser={async newContext(options){check(++state.sessions===1,'SESSION_BUDGET');save();const context=await browser.newContext(options);
      await context.route('**/*',async route=>{try{http();check(new URL(route.request().url()).origin===new URL(scope.baseUrl).origin,'FOREIGN_ORIGIN');await route.continue();}catch{await route.abort().catch(()=>{});}});return context;}};
    const authorization={environment:true,account:true,effects:true,discardable:true,select:true};
    state.status='LOGIN';save();auth=await support.openSession(boundedBrowser,profile,process.env,authorization);
    const rawPost=auth.context.request.post.bind(auth.context.request);
    const guardedAuth={...auth,context:{request:{async post(url,options){http();return rawPost(url,options);}}}};
    const driver=support.createDriver(guardedAuth,profile,authorization,{onReference:r=>{for(const [key,value]of Object.entries(r)){state.references.push({kind:key==='planId'?'plan':'lote',id:value});save();}}});
    const request=driver.request.bind(driver);driver.request=async(operation,payload,...rest)=>{
      check(['AbrirImportacion','PrepararPlan','ConfirmarPlan','ConsultarLote','ConsultarRecursosLote','AvanzarLote'].includes(operation),'OPERATION_FORBIDDEN');
      if(operation==='AvanzarLote')check(['CREAR_EXPEDIENTE','MATERIALIZAR_VALORES'].includes(payload.fase),'STORAGE_FORBIDDEN');
      return request(operation,payload,...rest);
    };
    const reference=(kind,id)=>{check(['plan','lote','raiz','archivo','expediente','relacion'].includes(kind)&&/^[1-9][0-9]{0,9}$/.test(id),'REFERENCE_INVALID');if(!state.references.some(r=>r.kind===kind&&r.id===id))state.references.push({kind,id});save();};
    const verifyPlan=async plan=>{
      admit();check(++state.selects<=12,'SELECT_BUDGET');save();
      const rows=await require('./support/doc32-e2e-odbc.cjs').queryRows('SELECT usuario_ref,ambito_ref,nivel_ref,version_id FROM ra_integracion_plan WHERE plan_id=?',[plan],process.env,'DOC111_E2E');
      check(rows.length===1&&String(rows[0].usuario_ref)==='141'&&String(rows[0].ambito_ref)==='2'&&String(rows[0].nivel_ref)==='7'&&String(rows[0].version_id)==='38','PLAN_OWNER_CHANGED');
    };
    state.status='PREPARING';save();state.result=await support.prepareFileReservationFixture(driver,profile,pdf,scope.runId,reference,verifyPlan);state.status='PASS_FIXTURE_READY';
  }catch(error){state.status='STOPPED_REVIEW_REQUIRED';state.failure={code:/^(DOC120_FILES|DOC111)_[A-Z0-9_]{3,80}$/.test(error.message||'')?error.message:'DOC120_FILES_FIXTURE_FAILED',...(error.serverCode?{serverCode:error.serverCode}:{})};throw Error('DOC120_FILES_FIXTURE_STOPPED');}
  finally{if(auth)await auth.context.close().catch(()=>{});if(browser)await browser.close().catch(()=>{});state.closed=true;state.finishedUtc=new Date().toISOString();save();}
  return state.result;
}
if(require.main===module)main().catch(()=>{console.error('DOC115_SESSION_STOPPED: revise referencias y estado antes de repetir; sin detalle privado.');process.exitCode=2;});
module.exports={parseArgs,safeLine,main,repositorySegmented};
