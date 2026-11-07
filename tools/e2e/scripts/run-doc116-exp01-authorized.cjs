'use strict';
const fs=require('node:fs'),path=require('node:path');
const {requireInteractiveConsole,collectValue,runChild}=require('./support/interactive-e2e-console.cjs');
const {hash,validateSetup}=require('../tests/support/doc116-exp01-setup.cjs');
const {readFixture}=require('../tests/support/doc111-importacion-carpetas.cjs');
// Playwright matches the full title, including its file name before the test title.
const TEST_FILTER='DOC116 SETUP-EXP01$';
function testFilter(scenario){if(!['EXP-01','EXP-02','EXP-03'].includes(scenario))throw Error('DOC116_SETUP_SCOPE_INVALID');return 'DOC116 SETUP-'+scenario.replace('-','')+'$';}
function acceptsResult(v,p,approval,backendSha256){
  if(v?.scenario!==p.scenario||v.status!=='PASS'||v.documents!==0||v.readOnlyControls!==true||v.preservedResourcesUnchanged!==true||v.backendSha256!==backendSha256)return false;
  if(approval.readOnlyProbe===true)return v.preflightOnly===true&&[v.rootsCreated,v.preparations,v.confirmations,v.uploads,v.creationRequests].every(n=>n===0)&&v.replayUnchanged===null;
  if(v.preparations!==1||v.confirmations!==1||v.uploads!==2||v.lots!==1)return false;
  if(p.scenario==='EXP-03')return v.caseComplete===true&&v.internalFailure===null&&v.responseObservation===null&&v.rootsCreated===0&&v.creationRequests===1&&v.replayUnchanged===null&&v.denialUnchanged===true&&v.permissionRestored===true;
  if(v.replayUnchanged!==true)return false;
  return p.scenario==='EXP-01'?v.rootsCreated===2&&v.creationRequests===4&&v.caseComplete===true&&v.internalFailure===null&&v.responseObservation===null:v.rootsCreated===1&&v.creationRequests===2&&v.caseComplete===false&&v.internalFailure==='NO_EJECUTADO'&&v.responseObservation==='CLIENT_RESULT_DISCARDED';
}
function load(approvalFile){
  const approval=JSON.parse(fs.readFileSync(approvalFile,'utf8')),raw=fs.readFileSync(approval.proposalFile),p=JSON.parse(raw);validateSetup(p,readFixture(p.profile));
  if(approval.issueKey!=='DOC-116'||approval.scenario!==p.scenario||approval.instruction!=='autorizado'||approval.authorization!==true||hash(raw)!==approval.resourcesSha256)throw Error('DOC116_APPROVAL_INVALID');
  if(p.scenario==='EXP-03'&&approval.levelPermissionAuthorized!==true)throw Error('DOC116_LEVEL_PERMISSION_AUTHORIZATION_REQUIRED');
  for(const [file,digest]of Object.entries(approval.reviewedFiles)){if(hash(fs.readFileSync(path.resolve(p.runtimeRoot,file)))!==digest)throw Error('DOC116_REVIEW_CHANGED');}
  const backendSha256=hash(fs.readFileSync(path.join(p.runtimeRoot,'bin/GestionDocumental-Docuarchi.net.dll')));if(backendSha256!==approval.backendSha256)throw Error('DOC116_BACKEND_CHANGED');
  if(fs.existsSync(approval.checkpointFile))throw Error('DOC116_EXISTING_SESSION_REVIEW_REQUIRED');return {approval,p,backendSha256};
}
async function main(){
  if(process.argv.length!==3)throw Error('DOC116_ARGUMENT_INVALID');const {approval,p,backendSha256}=load(path.resolve(process.argv[2]));requireInteractiveConsole();
  if(approval.readOnlyProbe===true){console.log('DOC-116 EXP-01: diagnostico SOLO LECTURA del acceso al lote 11 y apertura del nivel 7.');console.log('No prepara, confirma, recibe PDFs ni crea expedientes/documentos; no cambia permisos.');}
  else if(p.scenario==='EXP-03'){console.log('DOC-116 EXP-03: nivel '+p.profile.nivelRef+', version '+p.profile.versionId+'; un plan, un lote y dos PDF de 677 bytes en staging.');console.log(p.profile.rootNames.join(' / '));console.log('Contexto 141/2 no propietario; permiso agregar_expediente de fila '+p.levelPermission.permissionId+': 1 -> 0 -> 1.');console.log('Una solicitud de creacion denegada. Cero expedientes y documentos nuevos; conserva recursos anteriores y nuevos.');console.log('Puente administrativo existente, diario durable y restauracion con lectura independiente. No ejecutar otras importaciones/creaciones ni cambios de permisos durante la prueba.');}
  else if(p.scenario==='EXP-02'){console.log('DOC-116 EXP-02 CLIENTE: un plan, un lote, dos PDF de 677 bytes; nivel 7/version 38.');console.log(p.profile.rootNames.join(' / '));console.log('Un expediente y dos solicitudes: crear, descartar resultado confirmado en cliente, consultar evidencia SELECT y repetir identico.');console.log('Segunda raiz sin crear. Cero documentos definitivos y cambios de permisos; conserva todos los recursos.');console.log('NO acredita fallo interno ni caida del servidor; EXP-02 completo permanece pendiente.');console.log('No ejecutar otras importaciones/creaciones durante esta prueba.');}
  else{
  console.log('DOC-116 EXP-01 autorizado: nivel 7, version 38; un plan, un lote, dos PDF de 677 bytes.');
  console.log('Dos expedientes DOC116-20261105-EXP01-01/02; cuatro solicitudes de creación/replay.');
  console.log('Cambio visible al nivel 8; destino fijo 7. Cero documentos definitivos y cambios de permisos.');
  console.log('Conserva recursos y lotes anteriores. No ejecutar otras importaciones/creaciones durante esta prueba.');
  }
  console.log('Autorizacion registrada. Credenciales solo aqui; usuario visible y contrasenas enmascaradas. DSN MembershipUsers; no URL MySQL.');
  const secrets={};let environment;
  try{
    await collectValue(secrets,'user','Cuenta de APLICACION autorizada, distinta de la cuenta MySQL (visible solo aqui)',{secret:false});await collectValue(secrets,'password','Contraseña de la aplicacion',{secret:true});
    await collectValue(secrets,'dbUser','Cuenta MySQL exclusivamente SELECT (visible solo aqui)',{secret:false});await collectValue(secrets,'dbPassword','Contraseña MySQL de solo lectura',{secret:true});
    // Recheck immutable reviewed scope after private input and before the child can write.
    load(path.resolve(process.argv[2]));
    environment=Object.fromEntries(Object.entries(process.env).filter(([k])=>!/^DOC11\d_/.test(k)));
    Object.assign(environment,{DOC116_SETUP_AUTHORIZED:'true',DOC116_SETUP_SCENARIO:p.scenario,DOC116_SETUP_PROPOSAL:JSON.stringify(p),DOC116_SETUP_CHECKPOINT:approval.checkpointFile,DOC116_SETUP_CODE_SHA256:hash(approval.reviewedFiles),DOC116_SETUP_RESOURCES_SHA256:approval.resourcesSha256,DOC116_BACKEND_SHA256:backendSha256,DOC111_E2E_MODULE:p.profile.module,DOC111_E2E_USER:secrets.user,DOC111_E2E_PASSWORD:secrets.password});
    if(p.scenario==='EXP-03')environment.DOC116_AUTH_LEVEL_PERMISSION='true';
    environment.DOC116_SETUP_PROBE_ONLY=approval.readOnlyProbe===true?'true':'false';
    for(const prefix of ['DOC111_E2E','DOC111_DA_E2E'])Object.assign(environment,{[prefix+'_ODBC_DSN']:p.profile.odbcDsn,[prefix+'_MYSQL_USER']:secrets.dbUser,[prefix+'_MYSQL_PASSWORD']:secrets.dbPassword});
    let passed=false;
    const child=await runChild(process.execPath,[path.resolve(__dirname,'../node_modules/@playwright/test/cli.js'),'test','tests/doc116-exp01-authorized.spec.cjs','--grep',testFilter(p.scenario),'--workers=1','--retries=0'],path.resolve(__dirname,'..'),environment,{nonInteractiveChild:true,redactOutput:output=>String(output).split(/\r?\n/).map(line=>{const match=/^(DOC116_(?:SETUP_REFERENCE|REFERENCE|SETUP_RESULT|SETUP_FAILURE))=(.*)$/.exec(line);if(!match)return '';let v;try{v=JSON.parse(match[2]);}catch{return '';}if(v.scenario!==p.scenario)return '';const result={scenario:p.scenario};for(const key of ['stage','code','operation','httpStatus','serverCode','preflightOnly','caseComplete','internalFailure','responseObservation','denialUnchanged','permissionRestored','planId','loteId','raizId','expedienteId','relacionNivelId','status','preparations','confirmations','uploads','lots','rootsCreated','creationRequests','documents','replayUnchanged','readOnlyControls','preservedResourcesUnchanged','backendSha256'])if(Object.hasOwn(v,key))result[key]=v[key];if(match[1]==='DOC116_SETUP_RESULT'&&acceptsResult(v,p,approval,backendSha256))passed=true;return match[1]+'='+JSON.stringify(result)+'\n';}).join('')});
    if(child.code!==0||!passed)throw Error('DOC116_SESSION_STOPPED');console.log('DOC116_'+p.scenario.replace('-','')+'_SESSION_FINISHED: revise el resultado saneado. No repita el comando.');
  }finally{for(const k of Object.keys(secrets))delete secrets[k];if(environment)for(const k of Object.keys(environment))if(/^DOC11\d_/.test(k))delete environment[k];}
}
if(require.main===module)main().catch(()=>{console.error('DOC116_SESSION_STOPPED: no repita; Codex revisara estado y referencias.');process.exitCode=2;});
module.exports={load,TEST_FILTER,testFilter,acceptsResult};
