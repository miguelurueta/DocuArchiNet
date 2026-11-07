'use strict';
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto'),{execFileSync}=require('node:child_process');
const {runSetup,validateProposal,authorize}=require('../tests/support/doc117-alm02-setup.cjs');
const {openSession,validateProfile}=require('../tests/support/doc111-importacion-carpetas.cjs');
const {ROOT,sha}=require('./support/doc117-alm02-candidate.cjs');
const {queryRows}=require('./support/doc32-e2e-odbc.cjs'),tty=require('./support/interactive-e2e-console.cjs');
const PROPOSAL='Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc117-alm02-recursos-propuestos-20261106.json';
const check=(v,c)=>{if(!v)throw Error('DOC117_SETUP_'+c);};
function codeFingerprint(){return sha(['tools/e2e/scripts/run-doc117-alm02-setup.cjs','tools/e2e/tests/support/doc117-alm02-setup.cjs','tools/e2e/tests/support/doc111-importacion-carpetas.cjs','tools/e2e/tests/support/doc117-alm01-driver.cjs','tools/e2e/tests/support/doc116-exp01-setup.cjs','tools/e2e/tests/support/doc114-plan-importacion.cjs','tools/e2e/tests/support/doc110-safe-request.cjs','tools/e2e/tests/support/authenticated-workflow-session.cjs','tools/e2e/scripts/support/doc117-alm02-candidate.cjs','tools/e2e/scripts/support/doc32-e2e-odbc.cjs','tools/e2e/scripts/query-doc32-odbc-fingerprint.ps1','tools/e2e/scripts/support/interactive-e2e-console.cjs','tools/e2e/fixtures/Doc117SolicitudHash.vb','tools/e2e/fixtures/Doc117SolicitudHashConsole.vb'].map(f=>sha(fs.readFileSync(path.join(ROOT,f)))).join('\n'));}
function resumeReference(a,planId){
 if(planId===undefined){check(a.resumePlanId===undefined,'RESUME_SCOPE_INVALID');return 0;}
 check(a.resumePlanId===planId&&/^[1-9][0-9]{0,9}$/.test(planId)&&a.maximumAdditionalPreparations===0&&/^[a-f0-9]{32}$/.test(a.previousAuthorizationId||''),'RESUME_SCOPE_INVALID');
 const before=JSON.parse(fs.readFileSync(path.join(ROOT,'tools/e2e/artifacts','doc117-alm02-setup-'+a.previousAuthorizationId+'.json'),'utf8'));
 check(before.scope==='ALM-02-SETUP'&&before.status==='STOPPED_REVIEW_REQUIRED'&&before.code==='DOC117_SETUP_PLAN_CHANGED'&&before.proposalSha256===a.proposalSha256&&before.selectRequests===a.priorSelectRequests&&Number.isInteger(before.selectRequests)&&before.selectRequests<160&&before.references.filter(r=>r.stage==='PLAN_KNOWN'&&r.planId===planId).length===1&&!before.references.some(r=>r.stage==='CONFIRMATION_REQUESTED'||r.stage==='LOT_KNOWN'),'RESUME_REFERENCE_CHANGED');return before.selectRequests;
}
function hashTool(file){
 const target=path.resolve(file),relative=path.relative(path.join(ROOT,'.tmp'),target);check(relative&&!relative.startsWith('..')&&!path.isAbsolute(relative)&&path.basename(target)==='Doc117SolicitudHash.exe','HASH_TOOL_INVALID');
 let cursor=target;while(cursor!==ROOT){check(!fs.lstatSync(cursor).isSymbolicLink(),'HASH_TOOL_INVALID');cursor=path.dirname(cursor);}
 const report=JSON.parse(fs.readFileSync(path.join(path.dirname(target),'hash-build-result.json'),'utf8'));
 check(report.status==='COMPILED_OFFLINE'&&report.executableSha256===sha(fs.readFileSync(target))&&report.calculatorSha256===sha(fs.readFileSync(path.join(ROOT,'tools/e2e/fixtures/Doc117SolicitudHash.vb')))&&report.consoleSha256===sha(fs.readFileSync(path.join(ROOT,'tools/e2e/fixtures/Doc117SolicitudHashConsole.vb')))&&report.productHarnessSha256===sha(fs.readFileSync(path.join(path.dirname(target),'PerfilImportacion.dll')))&&report.serializerSha256===sha(fs.readFileSync(path.join(path.dirname(target),'Newtonsoft.Json.dll'))),'HASH_TOOL_CHANGED');return target;
}
async function main(args=process.argv.slice(2)){
 const proposalBytes=fs.readFileSync(path.join(ROOT,PROPOSAL)),p=validateProposal(JSON.parse(proposalBytes)),proposalSha256=sha(proposalBytes),codeSha256=codeFingerprint();
 if(args.length===1&&args[0]==='--check'){console.log('DOC117_SETUP_CHECK='+JSON.stringify({status:'PASS',proposalSha256,codeSha256,budget:p.budget,realExecution:false}));return;}
 const resumePlanId=args.length===8&&args[6]==='--resume-plan'?args[7]:undefined;
 check((args.length===6||resumePlanId!==undefined)&&args[0]==='--profile'&&args[2]==='--authorization'&&args[4]==='--hash-tool','ARGUMENT_INVALID');
 const profile=validateProfile(JSON.parse(fs.readFileSync(path.resolve(args[1]),'utf8').replace(/^\uFEFF/,''))),approvalFile=path.resolve(args[3]),calculator=hashTool(args[5]);
 const verify=()=>{const a=JSON.parse(fs.readFileSync(approvalFile,'utf8'));authorize(a,p);check(a.proposalSha256===proposalSha256&&a.codeSha256===codeFingerprint()&&a.hashToolSha256===sha(fs.readFileSync(hashTool(calculator)))&&/^[a-f0-9]{32}$/.test(a.authorizationId),'AUTHORIZATION_CHANGED');check(profile.baseUrl===p.environment&&profile.nivelRef===p.levelId&&(!profile.ignoreHttpsErrors||a.tls===true),'ENVIRONMENT_CHANGED');check(sha(fs.readFileSync(path.join(ROOT,PROPOSAL)))===proposalSha256,'PROPOSAL_CHANGED');check(sha(fs.readFileSync(path.join(ROOT,'bin/GestionDocumental-Docuarchi.net.dll')))===p.originalRuntimeSha256,'ORIGINAL_RUNTIME_CHANGED');return a;};
 const approval=verify(),priorSelectRequests=resumeReference(approval,resumePlanId);tty.requireInteractiveConsole();
 const stem='doc117-alm02-setup-'+approval.authorizationId,artifact=path.join(ROOT,'tools/e2e/artifacts',stem+'.json'),readyFile=path.join(ROOT,'.tmp',stem+'-ready.runtime.json'),scopeFile=path.join(ROOT,'.tmp',stem+'-scope.json');
 check(!fs.existsSync(readyFile)&&!fs.existsSync(scopeFile),'PREVIOUS_SETUP_REQUIRES_REVIEW');
 fs.mkdirSync(path.dirname(artifact),{recursive:true});const state={issueKey:'DOC-117',scope:p.scope,status:'AWAITING_CREDENTIALS',proposalSha256,codeSha256,priorSelectRequests,selectRequests:priorSelectRequests,...(resumePlanId?{resumePlanId,previousAuthorizationId:approval.previousAuthorizationId}:{}),references:[],createdUtc:new Date().toISOString()};fs.writeFileSync(artifact,JSON.stringify(state,null,2)+'\n',{flag:'wx'});const save=()=>fs.writeFileSync(artifact,JSON.stringify(state,null,2)+'\n');
 const secrets={},environment=Object.fromEntries(Object.entries(process.env).filter(([k])=>!/^DOC\d+_/.test(k)));let browser,auth;
 try{
  console.log('DOC-117 ALM-02 SETUP autorizado: un plan/lote, dos recepciones/expedientes/materializaciones; cero documentos o cambios de DLL. Conservar recursos; no repetir.');
  if(resumePlanId)console.log('Continuacion del mismo plan '+resumePlanId+': cero preparaciones adicionales; presupuesto SELECT acumulado.');
  await tty.collectValue(secrets,'user','Cuenta de aplicacion autorizada (enmascarada)',{secret:true});await tty.collectValue(secrets,'password','Contrasena de aplicacion',{secret:true});await tty.collectValue(secrets,'dbUser','Cuenta MySQL exclusivamente SELECT',{secret:true});await tty.collectValue(secrets,'dbPassword','Contrasena MySQL de lectura',{secret:true});verify();
  Object.assign(environment,{DOC111_E2E_MODULE:profile.module,DOC111_E2E_USER:secrets.user,DOC111_E2E_PASSWORD:secrets.password});
  for(const [prefix,dsn]of [['DOC111_E2E',profile.odbcDsn],['DOC111_DA_E2E',profile.docuarchiOdbcDsn]])Object.assign(environment,{[prefix+'_ODBC_DSN']:dsn,[prefix+'_MYSQL_USER']:secrets.dbUser,[prefix+'_MYSQL_PASSWORD']:secrets.dbPassword});
  const query=async(origin,sql,params)=>{check(state.selectRequests<p.budget.maximumSelectRequests&&['RA','DA'].includes(origin),'SELECT_BUDGET_EXHAUSTED');state.selectRequests++;save();return queryRows(sql,params,environment,origin==='RA'?'DOC111_E2E':'DOC111_DA_E2E');};
  const {chromium}=require('../node_modules/@playwright/test'),config=require('../playwright.config.cjs');browser=await chromium.launch({headless:true,channel:config.use.channel,executablePath:config.use.executablePath});state.status='LOGIN';save();auth=await openSession(browser,profile,environment,{environment:true,account:true,tls:approval.tls});
  const result=await runSetup({proposal:p,profile,authorization:approval,auth,query,bytes:fs.readFileSync(path.join(ROOT,'tools/e2e/fixtures',p.fixture.file)),runtimeRoot:ROOT,resumePlanId,
   calculate:async input=>{const env=Object.fromEntries(Object.entries(process.env).filter(([k])=>['PATH','Path','SystemRoot','WINDIR','TEMP','TMP','ComSpec','PATHEXT'].includes(k)));return JSON.parse(execFileSync(hashTool(calculator),[],{input:JSON.stringify(input),encoding:'utf8',env,windowsHide:true,timeout:15000,stdio:['pipe','pipe','pipe']}));},
   onProgress:r=>{verify();const safe={stage:r.stage};for(const k of ['fase','planId','loteId','raizId','archivoId','intentoId','expedienteId','relacionNivelId','recursoId'])if(typeof r[k]==='string')safe[k]=r[k];state.references.push(safe);state.status=safe.stage;save();},
   onReady:value=>{verify();fs.writeFileSync(readyFile,JSON.stringify(value,null,2)+'\n',{flag:'wx'});fs.writeFileSync(scopeFile,JSON.stringify(value.resourceScope,null,2)+'\n',{flag:'wx'});}
  });verify();state.result=result;state.status='PASS';state.finishedUtc=new Date().toISOString();save();console.log('DOC117_SETUP_RESULT='+JSON.stringify(result));
 }catch(error){state.status='STOPPED_REVIEW_REQUIRED';state.code=/^(?:DOC117_SETUP|DOC111|DOC116_SETUP)_[A-Z0-9_]+$/.test(error?.message||'')?error.message:'DOC117_SETUP_STOPPED';state.finishedUtc=new Date().toISOString();save();console.log('DOC117_SETUP_STOPPED='+JSON.stringify({status:state.status,code:state.code,selectRequests:state.selectRequests}));process.exitCode=2;}
 finally{if(auth)await auth.context.close().catch(()=>{});if(browser)await browser.close().catch(()=>{});for(const k of Object.keys(secrets))delete secrets[k];for(const k of Object.keys(environment))delete environment[k];}
 console.log('DOC117_SETUP_FINISHED: conservar recursos; Codex revisara IDs. No repetir la preparacion.');
}
if(require.main===module)main().catch(()=>{console.error('DOC117_SETUP_NOT_STARTED: revisar alcance y autorizacion especifica.');process.exitCode=2;});
module.exports={main,PROPOSAL,codeFingerprint,hashTool,resumeReference};
