'use strict';
const fs=require('node:fs'),path=require('node:path');
const {requireInteractiveConsole,collectValue,runChild}=require('./support/interactive-e2e-console.cjs');
const {validateScope}=require('../tests/support/doc116-internal-fault.cjs');
const {ROOT,sha,validatePaths,validateManifest}=require('./support/doc116-runtime-lease.cjs');
function load(file,checkOnly=false){
  const m=JSON.parse(fs.readFileSync(file,'utf8'));validateScope(checkOnly?validatePaths(m):validateManifest(m));
  if(m.markerDirectory!==path.join(ROOT,'App_Data/DOC116-INTERNAL-LOTE18')||m.checkpointFile!==path.join(ROOT,'tools/e2e/artifacts/doc116-internal-lote18.json'))throw Error('DOC116_INTERNAL_SCOPE_INVALID');
  for(const [source,digest]of Object.entries(m.reviewedFiles)){if(sha(fs.readFileSync(path.join(ROOT,source)))!==digest)throw Error('DOC116_INTERNAL_REVIEW_CHANGED');}
  if(sha(fs.readFileSync(m.candidatePath))!==m.candidateSha256||sha(fs.readFileSync(path.join(ROOT,'bin/GestionDocumental-Docuarchi.net.dll')))!==m.originalSha256)throw Error('DOC116_INTERNAL_BINARY_CHANGED');
  if([m.leaseJournal,m.checkpointFile,...m.session.roots.map(r=>path.join(m.markerDirectory,'raiz-'+r.rootId+'.json'))].some(f=>fs.existsSync(f)))throw Error('DOC116_INTERNAL_PREVIOUS_RUN_REQUIRES_REVIEW');
  for(const copy of [m.backupPath,m.reserveBackupPath])if(fs.existsSync(copy)&&sha(fs.readFileSync(copy))!==m.originalSha256)throw Error('DOC116_RUNTIME_BACKUP_CHANGED');return m;
}
async function main(){
  const args=process.argv.slice(2),checkOnly=args.length===2&&args[1]==='--check';if(args.length!==(checkOnly?2:1))throw Error('DOC116_INTERNAL_ARGUMENT_INVALID');const file=path.resolve(args[0]),m=load(file,checkOnly);
  if(checkOnly){console.log('DOC116_INTERNAL_CHECK='+JSON.stringify({status:'PASS',authorized:m.authorization===true,activationAuthorized:m.runtimeActivationAuthorized===true,lotId:'18',roots:['29','30'],candidateSha256:m.candidateSha256,originalRuntimeUnchanged:true,realExecution:false}));return;}
  requireInteractiveConsole();console.log('DOC-116 EXP-02 INTERNO: lote 18, nivel 215, raices 29/30; sesion real 141/2.');console.log('Instala temporalmente SOLO el DLL de laboratorio revisado, con copia original verificada y guardian independiente de restauracion (maximo 10 minutos).');console.log('Raiz 29: fallo interno al devolver checkpoint confirmado y replay. Raiz 30: fallo tras legacy y antes del checkpoint; no se repite su creacion.');console.log('Hasta dos expedientes nuevos y tres solicitudes. Cero planes/lotes/recepciones adicionales, documentos definitivos y cambios de permisos.');console.log('La segunda raiz puede quedar EN_CURSO/INCIERTO con IDs conocidos y sin recreacion. Se conservan todos los recursos.');console.log('Ventana exclusiva: no ejecutar otras importaciones, cambios de permisos, despliegues ni reinicios de IIS. Credenciales solo aqui; nunca en el chat.');
  const secrets={};let environment;
  try{
    await collectValue(secrets,'user','Cuenta de APLICACION autorizada (visible solo aqui)',{secret:false});await collectValue(secrets,'password','Contraseña de la aplicacion',{secret:true});
    await collectValue(secrets,'dbUser','Cuenta MySQL exclusivamente SELECT (visible solo aqui)',{secret:false});await collectValue(secrets,'dbPassword','Contraseña MySQL de solo lectura',{secret:true});load(file);
    environment=Object.fromEntries(Object.entries(process.env).filter(([k])=>!/^DOC11\d_/.test(k)));Object.assign(environment,{DOC116_INTERNAL_AUTHORIZED:'true',DOC116_INTERNAL_MANIFEST:file,DOC116_INTERNAL_CHECKPOINT:m.checkpointFile,DOC111_E2E_MODULE:m.session.profile.module,DOC111_E2E_USER:secrets.user,DOC111_E2E_PASSWORD:secrets.password});
    for(const prefix of ['DOC111_E2E','DOC111_DA_E2E'])Object.assign(environment,{[prefix+'_ODBC_DSN']:prefix==='DOC111_E2E'?m.session.profile.odbcDsn:m.session.profile.docuarchiOdbcDsn,[prefix+'_MYSQL_USER']:secrets.dbUser,[prefix+'_MYSQL_PASSWORD']:secrets.dbPassword});
    let passed=false,restored=false;
    const child=await runChild(process.execPath,[path.resolve(__dirname,'../node_modules/@playwright/test/cli.js'),'test','tests/doc116-internal-fault.spec.cjs','--grep','DOC116 INTERNAL-EXP02$','--workers=1','--retries=0'],path.resolve(__dirname,'..'),environment,{nonInteractiveChild:true,redactOutput:output=>String(output).split(/\r?\n/).map(line=>{const match=/^(DOC116_INTERNAL_(?:REFERENCE|RESULT|FAILURE)|DOC116_RUNTIME_RESTORATION)=(.*)$/.exec(line);if(!match)return '';let v;try{v=JSON.parse(match[2]);}catch{return '';}
      const safe={};for(const key of ['stage','rootId','requestNumber','loteId','raizId','intentoId','expedienteId','relacionNivelId','scenario','code','status','laboratoryVariant','afterCommitReturnFailureVerified','preCheckpointFailureVerified','confirmedReplayUnchanged','uncertainCreationNotRepeated','persistedUncertainAttemptState','rootsCreated','creationRequests','documents','permissionChanges','preparations','confirmations','uploads','references','runtimeRestored','reopenedWithOriginalRuntime','preservedResourcesUnchanged','originalSha256','candidateSha256','restored'])if(Object.hasOwn(v,key))safe[key]=v[key];
      if(match[1]==='DOC116_INTERNAL_RESULT')passed=v.status==='PASS'&&v.scenario==='EXP-02-INTERNAL'&&v.afterCommitReturnFailureVerified===true&&v.preCheckpointFailureVerified===true&&v.confirmedReplayUnchanged===true&&v.uncertainCreationNotRepeated===true&&v.rootsCreated===2&&v.creationRequests===3&&v.documents===0&&v.runtimeRestored===true&&v.preservedResourcesUnchanged===true&&v.originalSha256===m.originalSha256&&v.candidateSha256===m.candidateSha256;
      if(match[1]==='DOC116_RUNTIME_RESTORATION')restored=v.restored===true&&v.originalSha256===m.originalSha256;return match[1]+'='+JSON.stringify(safe)+'\n';}).join('')});
    if(child.code!==0||!passed||!restored||sha(fs.readFileSync(path.join(ROOT,'bin/GestionDocumental-Docuarchi.net.dll')))!==m.originalSha256)throw Error('DOC116_INTERNAL_SESSION_STOPPED');console.log('DOC116_INTERNAL_SESSION_FINISHED: revise ambas ventanas y la restauracion; no repita el comando.');
  }finally{for(const key of Object.keys(secrets))delete secrets[key];if(environment)for(const key of Object.keys(environment))if(/^DOC11\d_/.test(key))delete environment[key];}
}
if(require.main===module)main().catch(()=>{console.error('DOC116_INTERNAL_SESSION_STOPPED: no repita; Codex revisara estado, DLL y diario de recuperacion.');process.exitCode=2;});
module.exports={load};
