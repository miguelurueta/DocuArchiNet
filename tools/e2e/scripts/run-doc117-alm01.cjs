'use strict';
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const root=path.resolve(__dirname,'../../..'),{validateManifest,sha}=require('../tests/support/doc117-almacenamiento.cjs');
const {createAlm01Controls,runAlm01}=require('../tests/support/doc117-alm01-driver.cjs');
const {validateProfile,openSession}=require('../tests/support/doc111-importacion-carpetas.cjs');
const {queryRows}=require('./support/doc32-e2e-odbc.cjs'),tty=require('./support/interactive-e2e-console.cjs');
function codeFingerprint(){return crypto.createHash('sha256').update(['tools/e2e/scripts/run-doc117-alm01.cjs','tools/e2e/tests/support/doc117-alm01-driver.cjs','tools/e2e/tests/support/doc117-almacenamiento.cjs','tools/e2e/tests/support/doc111-importacion-carpetas.cjs','tools/e2e/tests/support/doc114-plan-importacion.cjs','tools/e2e/tests/support/doc110-safe-request.cjs','tools/e2e/tests/support/authenticated-workflow-session.cjs'].map(f=>crypto.createHash('sha256').update(fs.readFileSync(path.join(root,f))).digest('hex')).join('\n')).digest('hex');}
function check(v,c){if(!v)throw Error('DOC117_'+c);}
function parseArgs(args){if(args.length===1&&args[0]==='--check')return{mode:'check'};if(args.length===3&&['--preflight','--run'].includes(args[0])&&args[1]==='--profile')return{mode:args[0].slice(2),profileFile:path.resolve(args[2])};throw Error('DOC117_ALM01_ARGUMENTS_INVALID');}
async function preflight({query,manifest,profile,identity,runtimeRoot}){
 validateManifest(manifest);const controls=createAlm01Controls(query,profile,identity,runtimeRoot);
 for(const c of manifest.cases)await controls.assertFresh(c);
 const rows=await query('RA',"SELECT CAST(v.version_id AS CHAR) AS version_id,CAST(p.perfil_id AS CHAR) AS perfil_id,v.configuracion_doc FROM ra_integracion_perfil_version v INNER JOIN ra_integracion_perfil p ON p.perfil_id=v.perfil_id WHERE p.ambito_ref=? AND p.activo=1 AND v.estado='PUBLICADA' AND v.version_id IN (?,?) ORDER BY v.version_id LIMIT 3",[identity.ambitoRef,'40','42']);
 check(rows.length===2,'PREFLIGHT_PUBLICATIONS_CHANGED');
 for(const c of manifest.cases){const hits=rows.filter(r=>r.version_id===c.versionId&&r.perfil_id===c.perfilId);check(hits.length===1,'PREFLIGHT_PUBLICATIONS_CHANGED');let config;try{config=JSON.parse(hits[0].configuracion_doc);}catch{throw Error('DOC117_PREFLIGHT_CONFIGURATION_INVALID')};check(String(config.GabineteId)===manifest.cabinetId&&config.ModoIntegridad==='COMPLETO_OBLIGATORIO'&&config.AmbitoIntegridad==='EXPEDIENTE','PREFLIGHT_CONFIGURATION_CHANGED');
  const types=await query('RA','SELECT CAST(t.Id_Tipo_Doc_Series AS CHAR) AS type_id FROM tipo_doc_series t INNER JOIN ra_registro_instrumento_archivistico i ON i.id_instrumento=t.id_instrumento INNER JOIN registro_organigrama o ON o.ID_ORGANIGRAMA=i.registro_organigrama_ID_ORGANIGRAMA WHERE t.Estado_Tipo=1 AND i.Estado_instrumento=1 AND o.EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA=? AND t.id_instrumento=? AND ((t.Series_Documentales_Id_Series=? AND COALESCE(t.sub_serie_id_serie,0)=COALESCE(?,0)) OR t.tipo_doc_trasversal=1) AND t.Id_Tipo_Doc_Series=? LIMIT 2',[identity.ambitoRef,config.InstrumentoRef,config.SerieRef,config.SubserieRef||null,c.expected.tipologiaId]);
  check(types.length===1&&types[0].type_id===c.expected.tipologiaId,'PREFLIGHT_TYPOLOGY_UNAVAILABLE');
 }
 const cabinets=await query('DA','SELECT NOMBRE FROM system1 WHERE id_gabinete=? LIMIT 2',[manifest.cabinetId]);check(cabinets.length===1&&typeof cabinets[0].NOMBRE==='string'&&/^[A-Za-z_][A-Za-z0-9_]{0,63}$/.test(cabinets[0].NOMBRE),'PREFLIGHT_CABINET_UNAVAILABLE');
 const fields=await query('DA','SELECT CAMPO FROM detalle_gabienete WHERE id_gabinete=? AND id_detalle_gabinete=? AND VISIBLE=1 LIMIT 2',[manifest.cabinetId,'14']);check(fields.length===1&&typeof fields[0].CAMPO==='string'&&/^[A-Za-z_][A-Za-z0-9_]{0,63}$/.test(fields[0].CAMPO),'PREFLIGHT_INDEX_UNAVAILABLE');
 return{status:'PASS',scenario:'ALM-01-PREFLIGHT-SELECT',manifestSha256:sha(manifest),rootAbsenceVerified:true,typologyVerified:true,cabinetAndIndexVerified:true,preparations:0,confirmations:0,uploads:0,documents:0,executionAuthorized:false};
}
async function main(args=process.argv.slice(2)){
 const options=parseArgs(args),manifest=validateManifest(JSON.parse(fs.readFileSync(path.join(root,'tools/e2e/fixtures/doc117-alm01-manifest.json'),'utf8')));
 if(options.mode==='check'){console.log('DOC117_ALM01_CHECK='+JSON.stringify({status:'PASS',manifestSha256:sha(manifest),preflightAvailable:true,executionImplemented:true,codeSha256:codeFingerprint(),realExecution:false}));return;}
 const profile=validateProfile(JSON.parse(fs.readFileSync(options.profileFile,'utf8').replace(/^\uFEFF/,'')));check(profile.baseUrl==='https://localhost/GestionDocumental-Docuarchi.net/'&&profile.nivelRef==='7','ALM01_ENVIRONMENT_CHANGED');
 const authorizationFile=path.join(root,'.tmp/doc117-alm01-'+(options.mode==='run'?'effects':'preflight')+'-authorization.json');check(fs.existsSync(authorizationFile),'ALM01_SPECIFIC_AUTHORIZATION_REQUIRED');const approval=JSON.parse(fs.readFileSync(authorizationFile,'utf8'));
 check(approval.authorized===true&&approval.scope===(options.mode==='run'?'ALM-01':'ALM-01-PREFLIGHT-SELECT')&&approval.manifestSha256===sha(manifest)&&approval.environment===profile.baseUrl&&/^[a-f0-9]{32}$/.test(approval.authorizationId)&&approval.select===true&&approval.account===true&&approval.environmentAuthorized===true,'ALM01_AUTHORIZATION_SCOPE_CHANGED');
 check(approval.codeSha256===codeFingerprint(),'ALM01_CODE_CHANGED');
 check(options.mode==='run'?approval.effects===true&&approval.discardable===true&&approval.exclusive===true:approval.effects===false,'ALM01_AUTHORIZATION_SCOPE_CHANGED');
 tty.requireInteractiveConsole();const marker=path.join(root,'.tmp/doc117-alm01-consumed-'+approval.authorizationId+'.json');fs.writeFileSync(marker,JSON.stringify({status:'STARTED',scope:approval.scope,manifestSha256:sha(manifest)})+'\n',{flag:'wx'});
 const artifact=path.join(root,'tools/e2e/artifacts/doc117-alm01-'+options.mode+'-'+crypto.randomBytes(8).toString('hex')+'.json');const state={issueKey:'DOC-117',scope:approval.scope,status:'RUNNING',manifestSha256:sha(manifest),references:[]};fs.writeFileSync(artifact,JSON.stringify(state,null,2)+'\n',{flag:'wx'});const save=()=>fs.writeFileSync(artifact,JSON.stringify(state,null,2)+'\n');
 const secrets={},environment=Object.fromEntries(Object.entries(process.env).filter(([k])=>!/^DOC11\d_/.test(k))),identity={userRef:'141',ambitoRef:'2'};let stage='CREDENTIALS',browser,auth;
 try{
  console.log(options.mode==='run'?'DOC-117 ALM-01: dos formatos autorizados, dos lotes/expedientes/documentos; conservar recursos.':'DOC-117 ALM-01: preflight SOLO SELECT; ausencia de raices, publicaciones, tipologia y gabinete/indice.');
  console.log('Autorizacion especifica registrada. Credenciales solo aqui; usuario visible, contrasenas enmascaradas.');
  console.log('MySQL usa los DSN registrados MembershipUsers; nunca se solicita URL.');
  if(options.mode==='run'){await tty.collectValue(secrets,'user','Cuenta de APLICACION autorizada (visible solo aqui)',{secret:false});check(secrets.user==='LUZ.AGUILERA','ALM01_APPLICATION_ACCOUNT_CHANGED');await tty.collectValue(secrets,'password','Contrasena de aplicacion',{secret:true});}
  await tty.collectValue(secrets,'dbUser','Cuenta MySQL exclusivamente SELECT (puede ser distinta de la aplicacion)',{secret:false});await tty.collectValue(secrets,'dbPassword','Contrasena MySQL de solo lectura',{secret:true});
  for(const [prefix,dsn]of [['DOC111_E2E',profile.odbcDsn],['DOC111_DA_E2E',profile.docuarchiOdbcDsn]])Object.assign(environment,{[prefix+'_ODBC_DSN']:dsn,[prefix+'_MYSQL_USER']:secrets.dbUser,[prefix+'_MYSQL_PASSWORD']:secrets.dbPassword});
  const query=async(origin,sql,params)=>{check(['RA','DA'].includes(origin)&&/^SELECT\b/.test(sql)&&!/;|\b(?:FOR\s+UPDATE|OUTFILE|DUMPFILE)\b/i.test(sql),'SELECT_REQUIRED');return queryRows(sql,params,environment,origin==='RA'?'DOC111_E2E':'DOC111_DA_E2E');};
  stage='PREFLIGHT_SELECT';state.preflight=await preflight({query,manifest,profile,identity,runtimeRoot:root});save();
  if(options.mode==='run'){
   check(approval.tls===true||!profile.ignoreHttpsErrors,'ALM01_TLS_AUTHORIZATION_REQUIRED');stage='LOGIN';Object.assign(environment,{DOC111_E2E_MODULE:profile.module,DOC111_E2E_USER:secrets.user,DOC111_E2E_PASSWORD:secrets.password});
   const {chromium}=require('../node_modules/@playwright/test'),config=require('../playwright.config.cjs');browser=await chromium.launch({headless:true,channel:config.use.channel,executablePath:config.use.executablePath});auth=await openSession(browser,profile,environment,{environment:true,account:true,tls:approval.tls});
   const proof=JSON.parse(fs.readFileSync(path.join(root,'Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc117-publicaciones-lectura-pass-20261105.json'),'utf8'));check(proof.status==='PASS'&&proof.manifestSha256===sha(manifest),'ALM01_PUBLICATION_REVIEW_REQUIRED');
   stage='ALM01';state.result=await runAlm01({manifest,profile,authorization:{...approval,environment:true,publications:proof.publications},auth,query,identity,runtimeRoot:root,bytes:fs.readFileSync(path.join(root,'tools/e2e/fixtures',manifest.fixture.file)),onCheckpoint:r=>{state.references.push(r);save();console.log('DOC117_ALM01_REFERENCE='+JSON.stringify(r));}});
  }else state.result=state.preflight;
  check(approval.codeSha256===codeFingerprint(),'ALM01_CODE_CHANGED');state.codeSha256=codeFingerprint();
  state.status='PASS';save();console.log('DOC117_ALM01_RESULT='+JSON.stringify(state.result));
 }catch(error){state.status='STOPPED_REVIEW_REQUIRED';state.failure={stage,code:/^DOC11[0-9]_[A-Z0-9_]+$/.test(error?.message||'')?error.message:'DOC117_ALM01_FAILED'};save();console.log('DOC117_ALM01_FAILURE='+JSON.stringify(state.failure));process.exitCode=2;
 }finally{if(auth)await auth.context.close().catch(()=>{});if(browser)await browser.close().catch(()=>{});for(const k of Object.keys(secrets))delete secrets[k];for(const k of Object.keys(environment))delete environment[k];}
 console.log('DOC117_ALM01_FINISHED: no repita el comando; Codex revisara estado y referencias.');
}
if(require.main===module)main().catch(()=>{console.error('DOC117_ALM01_STOPPED: revisar autorizacion/estado antes de repetir; sin detalle privado.');process.exitCode=2;});
module.exports={parseArgs,preflight,main,codeFingerprint};
