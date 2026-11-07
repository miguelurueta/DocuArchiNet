'use strict';
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const {requireInteractiveConsole,collectConfirmation,collectValue,runChild}=require('./support/interactive-e2e-console.cjs');
const {validateSession,assertRunnable}=require('../tests/support/doc116-expedientes-importacion.cjs');
const fixtures=require('../fixtures/doc116-expedientes-cases.json');
function parseArgs(args){const r={check:false};for(let i=0;i<args.length;i++){if(args[i]==='--check'&&!r.check){r.check=true;continue;}if(args[i]==='--session'&&!r.session&&args[i+1]&&!args[i+1].startsWith('--')){r.session=args[++i];continue;}throw Error('DOC116_ARGUMENT_INVALID');}if(!r.session)throw Error('DOC116_SESSION_REQUIRED');return r;}
function safeLine(line,s){
  const prefix=['DOC116_REFERENCE=','DOC116_RESULT=','DOC116_FAILURE='].find(p=>String(line).startsWith(p));if(!prefix)return null;let v;try{v=JSON.parse(line.slice(prefix.length));}catch{return null;}
  if(v?.scenario!==s.scenario)return null;
  const id=x=>typeof x==='string'&&/^[1-9][0-9]{0,9}$/.test(x)&&BigInt(x)<=4294967295n;
  if(prefix==='DOC116_REFERENCE='){if(v.loteId!==s.lotId||!s.roots.some(r=>r.rootId===v.raizId)||![v.expedienteId,v.relacionNivelId].every(id))return null;return prefix+JSON.stringify(Object.fromEntries(['scenario','loteId','raizId','expedienteId','relacionNivelId'].map(k=>[k,v[k]])));}
  if(prefix==='DOC116_FAILURE='){const code=require('../tests/support/doc116-expedientes-importacion.cjs').safeFailureCode(Error(v.code));return ['PRECHECK','LOGIN','CREATION'].includes(v.stage)?prefix+JSON.stringify({scenario:s.scenario,stage:v.stage,code}):null;}
  const full=s.scenario!=='EXP-02';
  if(v.status!=='PASS'||v.caseComplete!==full||v.internalFailure!==(full?null:'NO_EJECUTADO')||v.responseObservation!==(full?null:'CLIENT_RESULT_DISCARDED')||v.rootsCreated!==(s.scenario==='EXP-01'?2:s.scenario==='EXP-03'?0:1)||v.documents!==0||v.readOnlyControls!==true||v.replayUnchanged!==(s.scenario==='EXP-03'?null:true)||s.scenario==='EXP-03'&&(v.permissionRestored!==true||v.denialUnchanged!==true)||[v.preparations,v.confirmations,v.uploads].some(n=>n!==0)||!/^[a-f0-9]{40}$/.test(v.head)||!['codeSha256','backendSha256'].every(k=>/^[a-f0-9]{64}$/.test(v[k])))return null;
  return prefix+JSON.stringify(Object.fromEntries(['scenario','status','caseComplete','internalFailure','responseObservation','rootsCreated','documents','readOnlyControls','replayUnchanged',...(s.scenario==='EXP-03'?['permissionRestored','denialUnchanged']:[]),'preparations','confirmations','uploads','head','codeSha256','backendSha256'].map(k=>[k,v[k]])));
}
async function main(args=process.argv.slice(2),runtime={}){
  const parsed=parseArgs(args),file=path.resolve(parsed.session);if(fs.statSync(file).size>1024*1024)throw Error('DOC116_SESSION_TOO_LARGE');
  const bytes=fs.readFileSync(file),s=validateSession(JSON.parse(bytes.toString('utf8').replace(/^\uFEFF/,'')));
  if(parsed.check){console.log('DOC116_CHECK='+JSON.stringify({scenario:s.scenario,status:'PASS',executionAvailable:(s.scenario!=='EXP-03'||s.levelPermission!==undefined)&&typeof s.runtimeRoot==='string',internalFailureAvailable:false,preparations:0,confirmations:0,uploads:0,maximumCreationRequests:fixtures.scenarios[s.scenario].maximumCreationRequests,documents:0,realExecution:false,resourcesSha256:crypto.createHash('sha256').update(bytes).digest('hex')}));return;}
  assertRunnable(s);if(!s.runtimeRoot)throw Error('DOC116_RUNTIME_ROOT_REQUIRED');
  (runtime.requireInteractiveConsole||requireInteractiveConsole)();const authorization={},secrets={};let environment;
  try{
    console.log('DOC-116 '+s.scenario+': lote existente con dos originales recibidos. Solo creación; cero preparación, confirmación, recepción y almacenamiento documental. Recursos conservados.');
    if(s.scenario==='EXP-02')console.log('EXP-02: solo descarte de resultado confirmado en cliente; NO acredita falla interna ni aceptación completa del caso.');
    const scopes=[['environment','Autoriza este ambiente de pruebas DOC-116'],['account','Autoriza la cuenta operativa'],['select','Autoriza controles RA/DA con cuenta exclusivamente SELECT'],['effects','Autoriza las solicitudes de creación y replay del lote/raíces revisados'],['discardable','Confirma recursos descartables autorizados; se conservaran expedientes y temporales'],['exclusive','Confirma ventana sin otras creaciones/importaciones ni cambios de permisos sobre los niveles observados']];
    if(s.scenario==='EXP-03')scopes.push(['levelPermission','Autoriza retirar/restaurar solo agregar_expediente de la fila revisada mediante el puente administrativo registrado y diario de recuperación']);
    if(s.profile.ignoreHttpsErrors)scopes.push(['tls','Autoriza el certificado de pruebas']);
    for(const [key,label]of scopes)await (runtime.collectConfirmation||collectConfirmation)(authorization,key,label);
    const collect=runtime.collectValue||collectValue;
    await collect(secrets,'user','Cuenta autorizada (visible solo aqui)',{secret:false});await collect(secrets,'password','Contraseña',{secret:true});
    await collect(secrets,'dbUser','Cuenta MySQL exclusivamente SELECT para los DSN registrados',{secret:true});await collect(secrets,'dbPassword','Contraseña MySQL de solo lectura',{secret:true});
    const backendSha256=crypto.createHash('sha256').update(fs.readFileSync(path.join(s.runtimeRoot,'bin/GestionDocumental-Docuarchi.net.dll'))).digest('hex');
    environment=Object.fromEntries(Object.entries(process.env).filter(([k])=>!/^DOC11\d_/.test(k)));
    Object.assign(environment,{DOC116_E2E_AUTHORIZED:'true',DOC116_SCENARIO:s.scenario,DOC116_SESSION:JSON.stringify(s),DOC116_RUN_ID:crypto.randomBytes(16).toString('hex'),DOC116_BACKEND_SHA256:backendSha256,DOC111_E2E_MODULE:s.profile.module,DOC111_E2E_USER:secrets.user,DOC111_E2E_PASSWORD:secrets.password,DOC111_E2E_ODBC_DSN:s.profile.odbcDsn,DOC111_DA_E2E_ODBC_DSN:s.profile.docuarchiOdbcDsn});
    for(const prefix of ['DOC111_E2E','DOC111_DA_E2E'])Object.assign(environment,{[prefix+'_MYSQL_USER']:secrets.dbUser,[prefix+'_MYSQL_PASSWORD']:secrets.dbPassword});
    for(const [key]of scopes){if(authorization[key]!=='true')throw Error('DOC116_AUTHORIZATION_REQUIRED');environment['DOC116_AUTH_'+key.toUpperCase()]='true';}
    const results=[];const child=await (runtime.runChild||runChild)(process.execPath,[path.resolve(__dirname,'../node_modules/@playwright/test/cli.js'),'test','tests/doc116-expedientes-importacion.spec.cjs','--grep','DOC116 '+s.scenario+'$','--workers=1','--retries=0'],path.resolve(__dirname,'..'),environment,{nonInteractiveChild:true,redactOutput:text=>String(text).split(/\r?\n/).map(line=>{const safe=safeLine(line,s);if(!safe)return '';if(safe.startsWith('DOC116_RESULT='))results.push(JSON.parse(safe.slice('DOC116_RESULT='.length)));return safe+'\n';}).join('')});
    if(child.code!==0||results.length!==1||results[0].backendSha256!==backendSha256)throw Error('DOC116_SESSION_STOPPED');return results[0];
  }finally{for(const key of Object.keys(secrets))delete secrets[key];if(environment)for(const key of Object.keys(environment))if(/^DOC11\d_/.test(key))delete environment[key];}
}
if(require.main===module)main().catch(()=>{console.error('DOC116_SESSION_STOPPED: revise estado y referencias antes de repetir; sin detalle privado.');process.exitCode=2;});
module.exports={parseArgs,safeLine,main};
