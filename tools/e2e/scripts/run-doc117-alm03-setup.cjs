'use strict';
const fs=require('node:fs'),path=require('node:path'),{execFileSync}=require('node:child_process');
const {runSetup,validateProposal,authorize}=require('../tests/support/doc117-alm03-setup.cjs');
const {openSession,validateProfile}=require('../tests/support/doc111-importacion-carpetas.cjs');
const {ROOT,sha}=require('../tests/support/doc117-alm03-setup-scope.cjs'),{hashTool}=require('./run-doc117-alm02-setup.cjs');
const {queryRows}=require('./support/doc32-e2e-odbc.cjs'),tty=require('./support/interactive-e2e-console.cjs');
const PROPOSAL='Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc117-alm03-recursos-propuestos-20261106.json';
const REVIEW_FILES=['tools/e2e/scripts/run-doc117-alm03-setup.cjs','tools/e2e/tests/support/doc117-alm03-setup.cjs','tools/e2e/tests/support/doc117-alm03-setup-scope.cjs','tools/e2e/scripts/run-doc117-alm02-setup.cjs','tools/e2e/tests/support/doc111-importacion-carpetas.cjs','tools/e2e/tests/support/doc117-alm01-driver.cjs','tools/e2e/tests/support/doc116-exp01-setup.cjs','tools/e2e/tests/support/doc114-plan-importacion.cjs','tools/e2e/tests/support/doc110-safe-request.cjs','tools/e2e/tests/support/authenticated-workflow-session.cjs','tools/e2e/scripts/support/doc32-e2e-odbc.cjs','tools/e2e/scripts/query-doc32-odbc-fingerprint.ps1','tools/e2e/scripts/support/interactive-e2e-console.cjs','tools/e2e/fixtures/Doc117SolicitudHash.vb','tools/e2e/fixtures/Doc117SolicitudHashConsole.vb'];
const codeFingerprint=()=>sha(REVIEW_FILES.map(f=>sha(fs.readFileSync(path.join(ROOT,f)))).join('\n')),parse=x=>JSON.parse(String(x).replace(/^\uFEFF/,'')),check=(v,c)=>{if(!v)throw Error('DOC117_ALM03_SETUP_'+c);};
const minimalEnvironment=()=>Object.fromEntries(Object.entries(process.env).filter(([k])=>['PATH','Path','SystemRoot','WINDIR','TEMP','TMP','ComSpec','PATHEXT'].includes(k)));
async function main(args=process.argv.slice(2)){
 const bytes=fs.readFileSync(path.join(ROOT,PROPOSAL)),p=validateProposal(parse(bytes)),proposalSha256=sha(bytes),codeSha256=codeFingerprint();
 if(args.length===1&&args[0]==='--check'){console.log('DOC117_ALM03_SETUP_CHECK='+JSON.stringify({status:'PASS',proposalSha256,codeSha256,budget:p.budget,individual:p.individual,realExecution:false}));return;}
 check(args.length===6&&args[0]==='--profile'&&args[2]==='--authorization'&&args[4]==='--hash-tool','ARGUMENTS_INVALID');
 const profile=validateProfile(parse(fs.readFileSync(path.resolve(args[1])))),approvalFile=path.resolve(args[3]),calculator=hashTool(args[5]);
 const verify=()=>{const a=parse(fs.readFileSync(approvalFile));authorize(a,p);check(a.proposalSha256===proposalSha256&&a.codeSha256===codeFingerprint()&&a.hashToolSha256===sha(fs.readFileSync(hashTool(calculator)))&&/^[a-f0-9]{32}$/.test(a.authorizationId),'AUTHORIZATION_CHANGED');check(profile.baseUrl===p.environment&&profile.nivelRef==='7'&&profile.odbcDsn==='MembershipUsers'&&profile.docuarchiOdbcDsn==='MembershipUsers'&&(!profile.ignoreHttpsErrors||a.tls===true),'ENVIRONMENT_CHANGED');check(sha(fs.readFileSync(path.join(ROOT,PROPOSAL)))===proposalSha256&&sha(fs.readFileSync(path.join(ROOT,'bin/GestionDocumental-Docuarchi.net.dll')))===p.originalRuntimeSha256,'PROTECTED_INPUT_CHANGED');return a;};
 const approval=verify();tty.requireInteractiveConsole();
 const stem='doc117-alm03-setup-'+approval.authorizationId,artifact=path.join(ROOT,'tools/e2e/artifacts',stem+'.json'),readyFile=path.join(ROOT,'.tmp',stem+'-ready.runtime.json'),scopeFile=path.join(ROOT,'.tmp',stem+'-scope.json');
 check(!fs.existsSync(readyFile)&&!fs.existsSync(scopeFile),'PREVIOUS_SETUP_REQUIRES_REVIEW');
 const state={issueKey:'DOC-117',scope:p.scope,status:'AWAITING_CREDENTIALS',proposalSha256,codeSha256,selectRequests:0,references:[],createdUtc:new Date().toISOString()};fs.mkdirSync(path.dirname(artifact),{recursive:true});fs.writeFileSync(artifact,JSON.stringify(state,null,2)+'\n',{flag:'wx'});const save=()=>fs.writeFileSync(artifact,JSON.stringify(state,null,2)+'\n');
 const secrets={},environment=minimalEnvironment();let browser,auth;
 try{
  console.log('DOC-117 ALM-03 SETUP: importacion nivel 7; individual expediente 467/nivel 8/relacion 1072 solo se verifica, sin cargar.');
  console.log('Un plan/lote, dos recepciones/expedientes/materializaciones; cero documentos, ALMACENAR o cambios de DLL. Hasta 160 SELECT. Conservar todo; no repetir.');
  await tty.collectValue(secrets,'user','Cuenta de aplicacion autorizada',{secret:true});await tty.collectValue(secrets,'password','Contrasena de aplicacion',{secret:true});await tty.collectValue(secrets,'dbUser','Cuenta MySQL exclusivamente SELECT',{secret:true});await tty.collectValue(secrets,'dbPassword','Contrasena MySQL de lectura',{secret:true});verify();
  Object.assign(environment,{DOC111_E2E_MODULE:profile.module,DOC111_E2E_USER:secrets.user,DOC111_E2E_PASSWORD:secrets.password});for(const [prefix,dsn]of [['DOC111_E2E',profile.odbcDsn],['DOC111_DA_E2E',profile.docuarchiOdbcDsn]])Object.assign(environment,{[prefix+'_ODBC_DSN']:dsn,[prefix+'_MYSQL_USER']:secrets.dbUser,[prefix+'_MYSQL_PASSWORD']:secrets.dbPassword});
  const query=async(origin,sql,params)=>{verify();check(state.selectRequests<160&&['RA','DA'].includes(origin),'SELECT_BUDGET_EXHAUSTED');state.selectRequests++;save();return queryRows(sql,params,environment,origin==='RA'?'DOC111_E2E':'DOC111_DA_E2E');};
  const {chromium}=require('../node_modules/@playwright/test'),config=require('../playwright.config.cjs');browser=await chromium.launch({headless:true,channel:config.use.channel,executablePath:config.use.executablePath});state.status='LOGIN';save();auth=await openSession(browser,profile,environment,{environment:true,account:true,tls:approval.tls});
  const result=await runSetup({proposal:p,profile,authorization:approval,auth,query,bytes:fs.readFileSync(path.join(ROOT,'tools/e2e/fixtures',p.fixture.file)),runtimeRoot:ROOT,
   calculate:async input=>JSON.parse(execFileSync(hashTool(calculator),[],{input:JSON.stringify(input),encoding:'utf8',env:minimalEnvironment(),windowsHide:true,timeout:15000,stdio:['pipe','pipe','pipe']})),
   onProgress:value=>{verify();const safe={stage:value.stage};for(const key of ['fase','planId','loteId','raizId','archivoId','intentoId','expedienteId','relacionNivelId','recursoId'])if(typeof value[key]==='string')safe[key]=value[key];state.references.push(safe);state.status=safe.stage;save();},
   onReady:value=>{verify();fs.writeFileSync(readyFile,JSON.stringify(value,null,2)+'\n',{flag:'wx'});fs.writeFileSync(scopeFile,JSON.stringify(value.resourceScope,null,2)+'\n',{flag:'wx'});}
  });verify();state.result=result;state.status='PASS';state.finishedUtc=new Date().toISOString();save();console.log('DOC117_ALM03_SETUP_RESULT='+JSON.stringify(result));
 }catch(error){state.status='STOPPED_REVIEW_REQUIRED';state.code=/^(?:DOC117_ALM03_SETUP|DOC111|DOC116_SETUP)_[A-Z0-9_]+$/.test(error?.message||'')?error.message:'DOC117_ALM03_SETUP_STOPPED';state.finishedUtc=new Date().toISOString();save();console.log('DOC117_ALM03_SETUP_STOPPED='+JSON.stringify({status:state.status,code:state.code,selectRequests:state.selectRequests}));process.exitCode=2;}
 finally{if(auth)await auth.context.close().catch(()=>{});if(browser)await browser.close().catch(()=>{});for(const k of Object.keys(secrets))delete secrets[k];for(const k of Object.keys(environment))delete environment[k];}
}
if(require.main===module)main().catch(()=>{console.error('DOC117_ALM03_SETUP_NOT_STARTED');process.exitCode=2;});
module.exports={main,PROPOSAL,REVIEW_FILES,codeFingerprint};
