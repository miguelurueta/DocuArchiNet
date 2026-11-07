'use strict';
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const root=path.resolve(__dirname,'../../..');
const {validateManifest,inspectPublications,sha}=require('../tests/support/doc117-almacenamiento.cjs');
const {validateProfile,openSession}=require('../tests/support/doc111-importacion-carpetas.cjs');
const tty=require('./support/interactive-e2e-console.cjs');
function parseArgs(args){if(args.length===1&&args[0]==='--check')return{check:true};if(args.length===2&&args[0]==='--profile'&&typeof args[1]==='string')return{profileFile:path.resolve(args[1])};throw Error('DOC117_READ_ARGUMENTS_INVALID');}
async function main(args=process.argv.slice(2)){
 const options=parseArgs(args),manifest=validateManifest(JSON.parse(fs.readFileSync(path.join(root,'tools/e2e/fixtures/doc117-alm01-manifest.json'),'utf8')));
 if(options.check){console.log('DOC117_READ_CHECK='+JSON.stringify({status:'PASS',manifestSha256:sha(manifest),versions:manifest.cases.map(c=>c.versionId),preparations:0,documents:0,realExecution:false}));return;}
 const profile=validateProfile(JSON.parse(fs.readFileSync(options.profileFile,'utf8').replace(/^\uFEFF/,'')));
 if(profile.nivelRef!==manifest.levelId||profile.module!=='GESTOR')throw Error('DOC117_READ_PROFILE_SCOPE_CHANGED');
 tty.requireInteractiveConsole();
 console.log('DOC-117: comprobacion SOLO LECTURA de publicaciones 40/42 del nivel 7.');
 console.log('Ambiente solicitado: '+profile.baseUrl);
 console.log('Manifiesto SHA256: '+sha(manifest));
 console.log('No prepara planes, confirma lotes, recibe PDFs ni crea expedientes/documentos. No pide MySQL.');
 console.log('No comprueba aun ausencia de las nuevas raices ni autoriza la carga.');
 console.log('Usuario visible y contrasena enmascarada solo aqui; nunca en el chat.');
 const confirmations={},secrets={},environment=Object.fromEntries(Object.entries(process.env).filter(([k])=>!/^DOC11\d_/.test(k)));
 let browser,auth,stage='AUTHORIZATION',output,state;
 try{
  const approvalFile=path.join(root,'.tmp/doc117-publications-readonly-authorization.json');
  if(fs.existsSync(approvalFile)){
   const approved=JSON.parse(fs.readFileSync(approvalFile,'utf8'));
   if(approved.issueKey!=='DOC-117'||approved.scope!=='PUBLICATIONS_READONLY'||approved.authorized!==true||approved.environment!==profile.baseUrl||approved.levelId!==manifest.levelId||JSON.stringify(approved.versionIds)!==JSON.stringify(manifest.cases.map(c=>c.versionId))||approved.manifestSha256!==sha(manifest)||approved.applicationAccountAuthorized!==true||approved.effectsAuthorized!==false||profile.ignoreHttpsErrors&&approved.testCertificatePreviouslyAuthorized!==true)throw Error('DOC117_READ_AUTHORIZATION_SCOPE_CHANGED');
   console.log('Autorizacion de lectura registrada; no se repiten confirmaciones.');
  }else{
   await tty.collectConfirmation(confirmations,'environment','Autoriza este ambiente de pruebas para lectura DOC-117');
   await tty.collectConfirmation(confirmations,'account','Autoriza la cuenta de aplicacion para esta lectura');
   if(profile.ignoreHttpsErrors)await tty.collectConfirmation(confirmations,'tls','Autoriza el certificado de pruebas de este ambiente');
   if(confirmations.environment!=='true'||confirmations.account!=='true'||profile.ignoreHttpsErrors&&confirmations.tls!=='true')throw Error('DOC117_READ_AUTHORIZATION_REQUIRED');
  }
  output=path.join(root,'tools/e2e/artifacts/doc117-publications-readonly-'+crypto.randomBytes(8).toString('hex')+'.json');fs.mkdirSync(path.dirname(output),{recursive:true});
  state={issueKey:'DOC-117',status:'RUNNING',manifestSha256:sha(manifest),preparations:0,confirmations:0,uploads:0,documents:0};
  fs.writeFileSync(output,JSON.stringify(state,null,2)+'\n',{flag:'wx'});
  stage='CREDENTIALS';await tty.collectValue(secrets,'user','Cuenta de aplicacion autorizada (visible solo aqui)',{secret:false});await tty.collectValue(secrets,'password','Contrasena de aplicacion',{secret:true});
  Object.assign(environment,{DOC111_E2E_MODULE:profile.module,DOC111_E2E_USER:secrets.user,DOC111_E2E_PASSWORD:secrets.password});
  const {chromium}=require('../node_modules/@playwright/test'),config=require('../playwright.config.cjs');
  stage='LOGIN';browser=await chromium.launch({headless:true,channel:config.use.channel,executablePath:config.use.executablePath});
  const authorization={environment:true,account:true,tls:profile.ignoreHttpsErrors,select:true};auth=await openSession(browser,profile,environment,authorization);
  stage='READONLY_PUBLICATIONS';state.result=await inspectPublications({manifest,auth,profile,authorization});state.status='PASS';fs.writeFileSync(output,JSON.stringify(state,null,2)+'\n');console.log('DOC117_PUBLICATIONS_READONLY_RESULT='+JSON.stringify(state.result));
 }catch(error){const failure={stage,code:typeof error?.message==='string'&&/^(DOC117|DOC111)_[A-Z0-9_]+$/.test(error.message)?error.message:'DOC117_READ_FAILED'};if(state){state.status='STOPPED_REVIEW_REQUIRED';state.failure=failure;fs.writeFileSync(output,JSON.stringify(state,null,2)+'\n');}console.log('DOC117_PUBLICATIONS_READONLY_FAILURE='+JSON.stringify(failure));process.exitCode=2;
 }finally{if(auth)await auth.context.close().catch(()=>{});if(browser)await browser.close().catch(()=>{});for(const key of Object.keys(secrets))delete secrets[key];for(const key of Object.keys(environment))delete environment[key];for(const key of Object.keys(confirmations))delete confirmations[key];}
 console.log('DOC117_READ_FINISHED: resultado visible; no repita el comando. Codex revisara el estado.');
}
if(require.main===module)main().catch(()=>{console.error('DOC117_READ_STOPPED: revisar perfil y estado antes de repetir.');process.exitCode=2;});
module.exports={parseArgs,main};
