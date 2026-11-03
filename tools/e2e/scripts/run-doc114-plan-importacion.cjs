'use strict';
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const {collectConfirmation,collectValue,requireInteractiveConsole,runChild}=require('./support/interactive-e2e-console.cjs');
const {SCENARIOS,validateSession,validateResumePlan,safePlanObservation}=require('../tests/support/doc114-plan-importacion.cjs');
function parseArgs(args){
  const parsed={check:false};
  for(let i=0;i<args.length;i++){
    const flag=args[i];
    if(flag==='--check'&&!parsed.check){parsed.check=true;continue;}
    if(['--session','--scenario'].includes(flag)&&parsed[flag.slice(2)]===undefined&&args[i+1]&&!args[i+1].startsWith('--')){parsed[flag.slice(2)]=args[++i];continue;}
    throw Error('DOC114_ARGUMENT_INVALID');
  }
  if(!parsed.session||!SCENARIOS.includes(parsed.scenario))throw Error('DOC114_SESSION_AND_SCENARIO_REQUIRED');return parsed;
}
function safeLine(line,scenario){
  line=String(line).replace(/\x1b\[[0-9;]*m/g,'').trim();
  const prefix=['DOC114_PLAN_REFERENCE=','DOC114_PLAN_RESULT=','DOC114_PLAN_FAILURE='].find(p=>line.startsWith(p));if(!prefix)return null;
  let v;try{v=JSON.parse(line.slice(prefix.length));}catch{return null;}if(v?.scenario!==scenario)return null;
  if(prefix==='DOC114_PLAN_REFERENCE=')return typeof v.planId==='string'&&/^[1-9][0-9]{0,9}$/.test(v.planId)&&BigInt(v.planId)<=4294967295n?{prefix,value:{scenario,planId:v.planId}}:null;
  if(prefix==='DOC114_PLAN_FAILURE='){const observation=safePlanObservation(v.observation);return ['PRECHECK','LOGIN','PLAN'].includes(v.stage)&&/^DOC114_[A-Z0-9_]{3,80}$/.test(v.code)?{prefix,value:{scenario,stage:v.stage,code:v.code,...(observation?{observation}: {})}}:null;}
  if(v.status!=='PASS'||v.confirmations!==0||v.noDocumentEffects!==true||v.readOnlyControls!==true||![v.attempts,v.preparations,v.rejections].every(n=>Number.isInteger(n)&&n>=0&&n<=64)||v.attempts!==v.preparations+v.rejections||!(/^[a-f0-9]{40}$/).test(v.head)||!['codeSha256','backendSha256'].every(k=>(/^[a-f0-9]{64}$/).test(v[k]))||(scenario==='PLAN-03'?v.restorationVerified!==true:v.restorationVerified!==null))return null;
  if(v.resumedPlans!==undefined&&(!['PLAN-01','PLAN-03'].includes(scenario)||v.resumedPlans!==1))return null;
  return {prefix,value:Object.fromEntries(['scenario','status','attempts','preparations','rejections',...(v.resumedPlans===1?['resumedPlans']:[]),'confirmations','noDocumentEffects','readOnlyControls','restorationVerified','head','codeSha256','backendSha256'].map(k=>[k,v[k]]))};
}
async function main(args=process.argv.slice(2),runtime={}){
  const parsed=parseArgs(args),file=path.resolve(parsed.session);if(fs.statSync(file).size>1024*1024)throw Error('DOC114_SESSION_TOO_LARGE');
  const bytes=fs.readFileSync(file),session=validateSession(JSON.parse(bytes.toString('utf8').replace(/^\uFEFF/,'')));
  if(session.scenario!==parsed.scenario)throw Error('DOC114_SCENARIO_INVALID');
  const resume=validateResumePlan(session,runtime.resumePlan),maximumPreparations=session.cases.length-(resume?1:0);
  const digest=crypto.createHash('sha256').update(bytes).digest('hex');
  if(parsed.check){console.log('DOC114_CHECK='+JSON.stringify({status:'PASS',scenario:session.scenario,attempts:maximumPreparations,...(resume?{resumedPlans:1}:{}),resourcesSha256:digest,realExecution:false}));return;}
  (runtime.requireInteractiveConsole||requireInteractiveConsole)();
  const backendSha256=(runtime.backendFingerprint||(()=>crypto.createHash('sha256').update(fs.readFileSync(path.join(session.runtimeRoot,'bin/GestionDocumental-Docuarchi.net.dll'))).digest('hex')))();
  if(!(/^[a-f0-9]{64}$/).test(backendSha256))throw Error('DOC114_BACKEND_FINGERPRINT_INVALID');
  const confirm=runtime.collectConfirmation||collectConfirmation,collect=runtime.collectValue||collectValue,run=runtime.runChild||runChild;
  const p=session.profiles[0],auth={},secrets={};let environment;
  try{
    const runId=crypto.randomBytes(16).toString('hex');
    console.log('DOC-114 '+session.scenario+': recursos SHA256 '+digest+'; '+maximumPreparations+' intentos de preparación, planes conservados; cero confirmaciones/cargas.');
    if(resume)console.log('DOC-114: conserva plan '+resume.planId+' por lectura; no repite '+resume.caseKey+'.');
    console.log('DOC-114: referencias saneadas en artifacts/doc114-'+runId+'.json.');
    const scopes=[['environment','Autoriza este ambiente de pruebas DOC-114'],['account','Autoriza la cuenta operativa de pruebas'],['select','Autoriza controles RA/DA exclusivamente SELECT'],['effects','Autoriza los '+maximumPreparations+' intentos de preparacion enumerados en el archivo revisado'],['discardable','Confirma publicaciones e inventarios del archivo como recursos de prueba autorizados; se conservaran los planes']];
    if(session.scenario==='PLAN-03')scopes.push(['revocation','Autoriza revocar y restaurar solo IMPORTAR_CARPETAS de la cuenta indicada mediante el puente existente, sin cambiar otros campos']);
    if(p.ignoreHttpsErrors)scopes.push(['tls','Autoriza el certificado de pruebas de este ambiente']);
    for(const [key,label]of scopes)await confirm(auth,key,label);
    await collect(secrets,'user','Cuenta autorizada (visible solo en esta consola)',{secret:false});await collect(secrets,'password','Contraseña',{secret:true});
    if(session.scenario==='PLAN-03'&&secrets.user.toUpperCase()!=='LUZ.AGUILERA')throw Error('DOC114_REVOCATION_ACCOUNT_NOT_AUTHORIZED');
    await collect(secrets,'dbUser','Cuenta MySQL exclusivamente SELECT para los DSN registrados',{secret:true});await collect(secrets,'dbPassword','Contraseña MySQL de solo lectura',{secret:true});
    environment=Object.fromEntries(Object.entries(process.env).filter(([k])=>!/^DOC11[0-4]_/.test(k)));
    Object.assign(environment,{DOC114_E2E_AUTHORIZED:'true',DOC114_SCENARIO:session.scenario,DOC114_SESSION:JSON.stringify(session),DOC114_RUN_ID:runId,DOC114_BACKEND_SHA256:backendSha256,DOC114_E2E_USER:secrets.user,DOC111_E2E_MODULE:p.module,DOC111_E2E_USER:secrets.user,DOC111_E2E_PASSWORD:secrets.password,DOC111_E2E_ODBC_DSN:p.odbcDsn,DOC111_DA_E2E_ODBC_DSN:p.docuarchiOdbcDsn});
    if(resume)environment.DOC114_RESUME_PLAN=JSON.stringify(resume);
    for(const prefix of ['DOC111_E2E','DOC111_DA_E2E'])Object.assign(environment,{[prefix+'_MYSQL_USER']:secrets.dbUser,[prefix+'_MYSQL_PASSWORD']:secrets.dbPassword});
    for(const [key]of scopes){if(auth[key]!=='true')throw Error('DOC114_AUTHORIZATION_REQUIRED');environment['DOC114_AUTH_'+key.toUpperCase()]='true';}
    const results=[];
    const child=await run(process.execPath,[path.resolve(__dirname,'../node_modules/@playwright/test/cli.js'),'test','tests/doc114-plan-importacion.spec.cjs','--grep',session.scenario,'--retries=0','--workers=1'],path.resolve(__dirname,'..'),environment,{nonInteractiveChild:true,redactOutput:text=>String(text).split(/\r?\n/).map(line=>{const safe=safeLine(line,session.scenario);if(!safe)return '';if(safe.prefix==='DOC114_PLAN_RESULT=')results.push(safe.value);return safe.prefix+JSON.stringify(safe.value)+'\n';}).join('')});
    if(child.code!==0||results.length!==1||results[0].attempts!==maximumPreparations||(results[0].resumedPlans||0)!==(resume?1:0)||results[0].backendSha256!==backendSha256)throw Error('DOC114_SESSION_STOPPED');
    return results[0];
  }finally{for(const k of Object.keys(secrets))delete secrets[k];if(environment)for(const k of Object.keys(environment))if(/^DOC11[0-4]_/.test(k))delete environment[k];}
}
if(require.main===module)main().catch(()=>{console.error('DOC114_SESSION_STOPPED: consulte referencias y recuperacion antes de repetir; sin detalle privado.');process.exitCode=2;});
module.exports={parseArgs,safeLine,main};
