'use strict';
const fs = require('node:fs');
const path = require('node:path');
const {collectConfirmation,collectValue,requireInteractiveConsole,runChild} = require('./support/interactive-e2e-console.cjs');
const {MODES,validateProfile,readFixture,loginObservation} = require('../tests/support/doc111-importacion-carpetas.cjs');

function parseArgs(args) {
  const result = {check:false,diagnose:false};
  for(let i=0;i<args.length;i++){
    const arg=args[i];
    if(arg==='--check'&&!result.check){result.check=true;continue;}
    if(arg==='--diagnose'&&!result.diagnose){result.diagnose=true;continue;}
    if(['--profile','--scenario','--resume-lot','--close-attempt','--reconcile-attempt','--verify-lot','--partial-profile','--degradation-profile'].includes(arg)&&result[arg.slice(2)]===undefined&&args[i+1]&&!args[i+1].startsWith('--')){result[arg.slice(2)]=args[++i];continue;}
    throw new Error('DOC111_ARGUMENT_INVALID');
  }
  if(!result.profile||!MODES.includes(result.scenario)||result.check&&result.diagnose)throw new Error('DOC111_PROFILE_AND_SCENARIO_REQUIRED');
  if(result['degradation-profile']&&(result.scenario!=='LOTE'||result.diagnose||result['resume-lot']||result['verify-lot']||result['partial-profile']||result['close-attempt']||result['reconcile-attempt']))throw new Error('DOC111_DEGRADATION_SCOPE_INVALID');
  if(result['partial-profile']&&(result.scenario!=='PARCIAL'||result.diagnose||result['resume-lot']||result['verify-lot']||result['close-attempt']||result['reconcile-attempt']))throw new Error('DOC111_PARTIAL_PROPOSAL_INVALID');
  if(result['verify-lot']&&(result.scenario!=='LOTE'||result.diagnose||result['resume-lot']||result['close-attempt']||result['reconcile-attempt']||!/^[1-9][0-9]{0,9}$/.test(result['verify-lot'])||BigInt(result['verify-lot'])>4294967295n))throw new Error('DOC111_VERIFY_ARGUMENT_INVALID');
  if(result['resume-lot']||result['close-attempt']||result['reconcile-attempt']){
    if(result.scenario!=='LOTE'||result.diagnose||Boolean(result['close-attempt'])===Boolean(result['reconcile-attempt'])||!['resume-lot',result['reconcile-attempt']?'reconcile-attempt':'close-attempt'].every(k=>/^[1-9][0-9]{0,9}$/.test(result[k]||'')&&BigInt(result[k])<=4294967295n))throw new Error('DOC111_RESUME_ARGUMENT_INVALID');
  }
  return result;
}
function safeResult(line, mode) {
  line=String(line).replace(/\x1b\[[0-9;]*m/g,'').trim();
  if(!line.startsWith('DOC111_REC_RESULT='))return null;
  let r;try{r=JSON.parse(line.slice('DOC111_REC_RESULT='.length));}catch{return null;}
  const flags=['rec01','rec02','barrierPreserved','idsAndIndicesVerified','replayAndReopenUnchanged'];
  if(r.status!=='PASS'||r.mode!==mode||r.roots!==2||r.documents!==2||r.originalFiles!==(mode==='PARCIAL'?3:2)||!flags.every(k=>r[k]===true)||!(/^[a-f0-9]{40}$/.test(r.head)&&/^[a-f0-9]{64}$/.test(r.codeSha256)&&/^[a-f0-9]{64}$/.test(r.driverSha256)))return null;
  return {status:'PASS',mode,roots:2,documents:2,originalFiles:r.originalFiles,...Object.fromEntries(flags.map(k=>[k,true])),head:r.head,codeSha256:r.codeSha256,driverSha256:r.driverSha256};
}
function safeReference(line, mode) {
  line=String(line).replace(/\x1b\[[0-9;]*m/g,'').trim();
  if(!line.startsWith('DOC111_REC_REFERENCE='))return null;
  let r;try{r=JSON.parse(line.slice('DOC111_REC_REFERENCE='.length));}catch{return null;}
  const id=value=>typeof value==='string'&&/^[1-9][0-9]{0,9}$/.test(value)&&BigInt(value)<=4294967295n;
  if(r.mode!==mode||!id(r.planId)||(r.loteId!==undefined&&!id(r.loteId)))return null;
  return {mode,planId:r.planId,...(r.loteId?{loteId:r.loteId}:{})};
}
function safeFailure(line, mode) {
  line=String(line).replace(/\x1b\[[0-9;]*m/g,'').trim();
  if(!line.startsWith('DOC111_REC_FAILURE='))return null;
  let r;try{r=JSON.parse(line.slice('DOC111_REC_FAILURE='.length));}catch{return null;}
  if(r.mode!==mode||!['PRECHECK','READONLY_RA','READONLY_DA','LOGIN','PROFILE','SCHEMA','REC','VERIFY'].includes(r.stage)||!/^DOC111_[A-Z_]{3,70}$/.test(r.code))return null;
  const result={mode,stage:r.stage,code:r.code};
  const observed=loginObservation(r.loginObservation);if(r.stage==='LOGIN'&&observed)result.loginObservation=observed;
  const operations=['AbrirImportacion','ListarPerfilesImportacion','PrepararPlan','ConfirmarPlan','ConsultarLote','ConsultarRecursosLote','AvanzarLote','CerrarReservaSinInicio','ReconciliarRecurso'];
  const codes=['CAPACIDAD_NO_DISPONIBLE','ACCESO_DENEGADO','SOLICITUD_INVALIDA','SESION_REQUERIDA','CSRF_INVALIDO','ORIGEN_INVALIDO','ERROR_INTERNO','CONFIGURACION_INVALIDA','OPERACION_EN_CURSO','CONFLICTO_REVISION','ARCHIVO_RECHAZADO','RESULTADO_INCIERTO','NO_CLASIFICADO'];
  if(r.code==='DOC111_OPERATION_NOT_CONFIRMED'&&operations.includes(r.operation)&&Number.isInteger(r.httpStatus)&&r.httpStatus>=100&&r.httpStatus<=599&&codes.includes(r.serverCode))Object.assign(result,{operation:r.operation,httpStatus:r.httpStatus,serverCode:r.serverCode});
  return result;
}
async function main(args=process.argv.slice(2),runtime={}) {
  const options=parseArgs(args),profile=validateProfile(JSON.parse(fs.readFileSync(path.resolve(options.profile),'utf8').replace(/^\uFEFF/,'')));
  if(profile.mode!==options.scenario)throw new Error('DOC111_MODE_MISMATCH');
  const partialProposal=options['partial-profile']?require('../tests/support/doc111-partial-profile.cjs').validatePartialProposal(JSON.parse(fs.readFileSync(path.resolve(options['partial-profile']),'utf8').replace(/^\uFEFF/,'')),profile):null;
  const degradationProfile=options['degradation-profile']?require('../tests/support/doc111-degradation.cjs').validateDegradationProfile(JSON.parse(fs.readFileSync(path.resolve(options['degradation-profile']),'utf8').replace(/^\uFEFF/,'')),profile):null;
  readFixture(profile);
  if(options.check){console.log('DOC111_CHECK=PASS; REAL_EXECUTION=false');return;}
  // Consola y proceso hijo compartidos. No se crea .env ni configuracion Playwright.
  const tty=runtime.requireInteractiveConsole||requireInteractiveConsole;tty();
  const confirm=runtime.collectConfirmation||collectConfirmation,collect=runtime.collectValue||collectValue,run=runtime.runChild||runChild;
  const authorization={},secrets={};let environment;
  try{
    const effectsLabel=options['reconcile-attempt']?'Autoriza verificar/reconciliar el intento '+options['reconcile-attempt']+' del lote '+options['resume-lot']+' por sus IDs persistidos y almacenar solo el segundo PDF pendiente, conservando el primero y ambos expedientes':options['resume-lot']?'Autoriza cerrar exclusivamente la reserva sin inicio '+options['close-attempt']+' del lote '+options['resume-lot']+' y almacenar/incorporar sus dos documentos en los expedientes existentes, sin crear otro lote ni expedientes':'Autoriza confirmar un lote, recibir PDFs, crear dos expedientes y almacenar/incorporar dos documentos; probar una barrera y repetir comandos idempotentes';
    const confirmations=[['environment','Autoriza este ambiente de pruebas para DOC-111'],['account','Autoriza la cuenta operativa DOC-111'],...options.diagnose?[]:options['verify-lot']?[['select','Autoriza los controles RA/DA exclusivamente SELECT para verificar el lote completado']]:[['effects',effectsLabel],['discardable','Confirma que nivel, perfil publicado, dos nombres y documentos del perfil son recursos descartables autorizados; se conservaran al terminar'],['select','Autoriza los controles RA/DA con cuentas exclusivamente de lectura']]];
    if(degradationProfile){const entry=confirmations.find(([key])=>key==='effects');entry[1]='Autoriza ocultar/restaurar exclusivamente ra_integracion_evento y almacenar/incorporar un PDF en el expediente 467, conservando recursos y lotes 2 a 5';confirmations.find(([key])=>key==='discardable')[1]='Confirma que el expediente de pruebas 467 y el PDF son recursos autorizados; se conservaran al terminar';}
    for(const [key,label] of confirmations)await confirm(authorization,key,label);
    if(profile.ignoreHttpsErrors)await confirm(authorization,'tls','Autoriza el certificado de pruebas de este ambiente');
    await collect(secrets,'user','Cuenta autorizada (visible solo en esta consola)',{secret:false});
    await collect(secrets,'password','Contraseña',{secret:true});
    if(!options.diagnose){
      await collect(secrets,'dbUser','Cuenta MySQL de solo lectura para los DSN registrados RA/DA',{secret:true});
      await collect(secrets,'dbPassword','Contraseña de la cuenta MySQL de solo lectura',{secret:true});
    }
    environment={...process.env,DOC111_E2E_AUTHORIZED:'true',DOC111_E2E_DIAGNOSE:String(options.diagnose),DOC111_E2E_SCENARIO:options.scenario,DOC111_E2E_PROFILE:JSON.stringify(profile),DOC111_E2E_MODULE:profile.module,DOC111_AUTH_TLS:authorization.tls||'false',
      DOC111_E2E_RESUME_LOT:options['resume-lot']||'',DOC111_E2E_CLOSE_ATTEMPT:options['close-attempt']||'',DOC111_E2E_RECONCILE_ATTEMPT:options['reconcile-attempt']||'',
      DOC111_E2E_VERIFY_LOT:options['verify-lot']||'',
      DOC111_E2E_PARTIAL_PROPOSAL:partialProposal?JSON.stringify(partialProposal):'',
      DOC111_E2E_DEGRADATION_PROFILE:degradationProfile?JSON.stringify(degradationProfile):'',
      DOC111_E2E_USER:secrets.user,DOC111_E2E_PASSWORD:secrets.password,
      DOC111_E2E_ODBC_DSN:profile.odbcDsn,DOC111_DA_E2E_ODBC_DSN:profile.docuarchiOdbcDsn,
      DOC111_E2E_MYSQL_USER:secrets.dbUser,DOC111_E2E_MYSQL_PASSWORD:secrets.dbPassword,
      DOC111_DA_E2E_MYSQL_USER:secrets.dbUser,DOC111_DA_E2E_MYSQL_PASSWORD:secrets.dbPassword};
    for(const k of ['effects','discardable','select'])environment['DOC111_AUTH_'+k.toUpperCase()]='false';
    for(const [k] of confirmations){
      if(authorization[k]!=='true')throw new Error('DOC111_AUTHORIZATION_REQUIRED');
      environment['DOC111_AUTH_'+k.toUpperCase()]='true';
    }
    const results=[];
    const child=await run(process.execPath,[path.resolve(__dirname,'../node_modules/@playwright/test/cli.js'),'test','tests/doc111-importacion-carpetas.spec.cjs','--grep',options.scenario,'--retries=0','--workers=1','--reporter=list'],path.resolve(__dirname,'..'),environment,
      {nonInteractiveChild:true,redactOutput:value=>String(value).split(/\r?\n/).map(line=>{
        const r=safeResult(line,options.scenario),reference=safeReference(line,options.scenario),failure=safeFailure(line,options.scenario);
        const diagnostic=safeDiagnostic(line,options.scenario);
        const degradation=safeDegradationResult(line,options.scenario);
        if(degradation&&degradationProfile){results.push(degradation);return 'DOC111_DEGRADATION_RESULT='+JSON.stringify(degradation)+'\n';}
        const profileReference=safeProfileReference(line,options.scenario);
        if(profileReference&&partialProposal)return 'DOC111_PROFILE_REFERENCE='+JSON.stringify(profileReference)+'\n';
        const resumed=safeResumeResult(line,options.scenario);
        const reconciled=safeReconcileResumeResult(line,options.scenario);
        const verified=safeCompletedVerification(line,options.scenario);
        if(verified&&verified.loteId===options['verify-lot']){results.push(verified);return 'DOC111_COMPLETED_VERIFICATION_RESULT='+JSON.stringify(verified)+'\n';}
        if(reconciled&&options['resume-lot']&&reconciled.loteId===options['resume-lot']&&reconciled.intentoId===options['reconcile-attempt']){results.push(reconciled);return 'DOC111_RECONCILE_RESUME_RESULT='+JSON.stringify(reconciled)+'\n';}
        if(resumed&&options['resume-lot']&&!options['reconcile-attempt']&&resumed.loteId===options['resume-lot']&&resumed.intentoId===options['close-attempt']){results.push(resumed);return 'DOC111_RESUME_RESULT='+JSON.stringify(resumed)+'\n';}
        if(diagnostic&&options.diagnose){results.push(diagnostic);return 'DOC111_DIAGNOSTIC_RESULT='+JSON.stringify(diagnostic)+'\n';}
        if(r&&!options.diagnose&&!options['resume-lot']&&!options['verify-lot']&&!degradationProfile){results.push(r);return 'DOC111_REC_RESULT='+JSON.stringify(r)+'\n';}
        if(failure)return 'DOC111_REC_FAILURE='+JSON.stringify(failure)+'\n';
        return reference?'DOC111_REC_REFERENCE='+JSON.stringify(reference)+'\n':'';
      }).join('')});
    if(child.code!==0||results.length!==1)throw new Error(options.diagnose?'DOC111_DIAGNOSTIC_FAILED':'DOC111_REC_FAILED');
  }finally{
    for(const k of Object.keys(secrets))delete secrets[k];
    if(environment)for(const k of Object.keys(environment))if(k.startsWith('DOC111_'))delete environment[k];
  }
}
if(require.main===module)main().catch(()=>{console.error('DOC111_RUN_STOPPED: sin detalle privado; consulte el estado antes de repetir.');process.exitCode=2;});
function safeDiagnostic(line,mode){
  line=String(line).trim();if(!line.startsWith('DOC111_DIAGNOSTIC_RESULT='))return null;
  let value;try{value=JSON.parse(line.slice('DOC111_DIAGNOSTIC_RESULT='.length));}catch{return null;}
  const observed=loginObservation(value.loginObservation);
  return value.status==='PASS'&&value.mode===mode&&value.pageReady===true?{status:'PASS',mode,pageReady:true,...(observed?{loginObservation:observed}:{})}:null;
}
function safeResumeResult(line,mode){
  return safeContinuationResult(line,mode,'DOC111_RESUME_RESULT=','closedWithoutStart');
}
function safeReconcileResumeResult(line,mode){
  return safeContinuationResult(line,mode,'DOC111_RECONCILE_RESUME_RESULT=','existingDocumentReconciled');
}
function safeCompletedVerification(line,mode){
  const prefix='DOC111_COMPLETED_VERIFICATION_RESULT=';
  line=String(line).replace(/\x1b\[[0-9;]*m/g,'').trim();if(!line.startsWith(prefix))return null;
  let r;try{r=JSON.parse(line.slice(prefix.length));}catch{return null;}
  if(mode!=='LOTE'||r.mode!==mode||r.status!=='PASS'||r.roots!==2||r.documents!==2||r.completedVerified!==true||r.reopenUnchanged!==true||!/^[1-9][0-9]{0,9}$/.test(r.loteId||'')||BigInt(r.loteId)>4294967295n||!/^[a-f0-9]{40}$/.test(r.head)||!['codeSha256','driverSha256'].every(k=>/^[a-f0-9]{64}$/.test(r[k])))return null;
  return {status:'PASS',mode,loteId:r.loteId,roots:2,documents:2,completedVerified:true,reopenUnchanged:true,head:r.head,codeSha256:r.codeSha256,driverSha256:r.driverSha256};
}
function safeContinuationResult(line,mode,prefix,firstFlag){
  line=String(line).replace(/\x1b\[[0-9;]*m/g,'').trim();if(!line.startsWith(prefix))return null;
  let r;try{r=JSON.parse(line.slice(prefix.length));}catch{return null;}
  const flags=[firstFlag,'existingExpeditionsPreserved','idsAndIndicesVerified','replayAndReopenUnchanged'];
  if(mode!=='LOTE'||r.mode!==mode||r.status!=='PASS'||r.roots!==2||r.documents!==2||!flags.every(k=>r[k]===true)||!['loteId','intentoId'].every(k=>/^[1-9][0-9]{0,9}$/.test(r[k]||'')&&BigInt(r[k])<=4294967295n)||!/^[a-f0-9]{40}$/.test(r.head)||!['codeSha256','driverSha256'].every(k=>/^[a-f0-9]{64}$/.test(r[k])))return null;
  return {status:'PASS',mode,loteId:r.loteId,intentoId:r.intentoId,roots:2,documents:2,...Object.fromEntries(flags.map(k=>[k,true])),head:r.head,codeSha256:r.codeSha256,driverSha256:r.driverSha256};
}
function safeProfileReference(line,mode){
  const prefix='DOC111_PROFILE_REFERENCE=';
  line=String(line).replace(/\x1b\[[0-9;]*m/g,'').trim();if(!line.startsWith(prefix))return null;
  let r;try{r=JSON.parse(line.slice(prefix.length));}catch{return null;}
  const valid=x=>typeof x==='string'&&/^[1-9][0-9]{0,9}$/.test(x)&&BigInt(x)<=4294967295n;
  return mode==='PARCIAL'&&r.mode===mode&&valid(r.perfilId)&&valid(r.versionId)?{mode,perfilId:r.perfilId,versionId:r.versionId}:null;
}
function safeDegradationResult(line,mode){
  const prefix='DOC111_DEGRADATION_RESULT=';line=String(line).replace(/\x1b\[[0-9;]*m/g,'').trim();if(!line.startsWith(prefix))return null;
  let r;try{r=JSON.parse(line.slice(prefix.length));}catch{return null;}
  const flags=['unavailableVerified','individualUploadVerified','schemaRestored','lotsUnchanged'];
  const id=x=>typeof x==='string'&&/^[1-9][0-9]{0,9}$/.test(x)&&BigInt(x)<=4294967295n;
  if(mode!=='LOTE'||r.mode!==mode||r.status!=='PASS'||r.documents!==1||r.expedienteId!=='467'||!id(r.documentoId)||!id(r.inventarioId)||!flags.every(k=>r[k]===true)||!/^[a-f0-9]{40}$/.test(r.head)||!['codeSha256','driverSha256'].every(k=>/^[a-f0-9]{64}$/.test(r[k])))return null;
  return {status:'PASS',mode,expedienteId:'467',documentoId:r.documentoId,inventarioId:r.inventarioId,documents:1,...Object.fromEntries(flags.map(k=>[k,true])),head:r.head,codeSha256:r.codeSha256,driverSha256:r.driverSha256};
}
module.exports={parseArgs,safeResult,safeReference,safeProfileReference,safeDegradationResult,safeFailure,safeDiagnostic,safeResumeResult,safeReconcileResumeResult,safeCompletedVerification,main};
