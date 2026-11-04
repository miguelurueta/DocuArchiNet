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
if(require.main===module)main().catch(()=>{console.error('DOC115_SESSION_STOPPED: revise referencias y estado antes de repetir; sin detalle privado.');process.exitCode=2;});
module.exports={parseArgs,safeLine,main};
