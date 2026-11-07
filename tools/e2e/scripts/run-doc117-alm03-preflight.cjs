'use strict';
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const {queryRows}=require('./support/doc32-e2e-odbc.cjs'),tty=require('./support/interactive-e2e-console.cjs');
const ROOT=path.resolve(__dirname,'../../..'),PROPOSAL='Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc117-alm03-preflight-propuesto-20261106.json';
const REVIEW_FILES=['tools/e2e/scripts/run-doc117-alm03-preflight.cjs','tools/e2e/scripts/support/doc32-e2e-odbc.cjs','tools/e2e/scripts/query-doc32-odbc-fingerprint.ps1','tools/e2e/scripts/support/interactive-e2e-console.cjs'];
const sha=x=>crypto.createHash('sha256').update(x).digest('hex'),codeFingerprint=()=>sha(REVIEW_FILES.map(f=>sha(fs.readFileSync(path.join(ROOT,f)))).join('\n'));
const check=(v,c)=>{if(!v)throw Error('DOC117_ALM03_PREFLIGHT_'+c);},parse=v=>JSON.parse(String(v).replace(/^\uFEFF/,''));
const KEYS=['publication_recipe','publication_rules','root_absence','prior_plan_absence','prior_root_absence','cabinet','visible_indices','individual_destination','individual_typology','level_authority','protected_attempts','individual_filename_absence'];
const id=v=>/^[1-9][0-9]{0,9}$/.test(String(v))&&BigInt(v)<=4294967295n;
function validateProposal(p){
 check(p?.issueKey==='DOC-117'&&p.scope==='ALM-03-PREFLIGHT-SELECT'&&p.authorized===false&&p.environment==='https://localhost/GestionDocumental-Docuarchi.net/'&&p.applicationIdentity.userRef==='141'&&p.applicationIdentity.ambitoRef==='2'&&p.levelId==='7'&&p.versionId==='38'&&p.cabinetId==='10'&&p.individualExpedientId==='467'&&p.individualTypologyId==='50','SCOPE_CHANGED');
 check(JSON.stringify(p.budget)===JSON.stringify({maximumSelectRequests:12,applicationLogins:0,preparations:0,confirmations:0,uploads:0,newExpedients:0,documents:0,storageRequests:0,runtimeChanges:0}),'BUDGET_CHANGED');
 check(JSON.stringify(p.proposedRootNames)===JSON.stringify(['DOC117-ALM03-A-20261106','DOC117-ALM03-B-20261106'])&&JSON.stringify(p.proposedFileNames)===JSON.stringify(['DOC117-ALM03-IMPORT-A-20261106.pdf','DOC117-ALM03-IMPORT-B-20261106.pdf','DOC117-ALM03-IND-20261106.pdf'])&&/^[a-f0-9]{64}$/.test(p.expectedPublicationSha256),'MANIFEST_CHANGED');
 check(Array.isArray(p.controls)&&JSON.stringify(p.controls.map(c=>c.key))===JSON.stringify(KEYS),'CONTROLS_CHANGED');
 for(const c of p.controls)check(['RA','DA'].includes(c.origin)&&typeof c.sql==='string'&&/^SELECT\b/.test(c.sql)&&!/[;]|--|\/\*|\b(?:INSERT|UPDATE|DELETE|CALL|EXEC|DROP|ALTER|CREATE|REPLACE|TRUNCATE|GRANT|REVOKE|SET|USE|LOAD|OUTFILE|INTO|LOCK|SLEEP|BENCHMARK)\b/i.test(c.sql)&&Array.isArray(c.parameters)&&(c.sql.match(/\?/g)||[]).length===c.parameters.length,'SELECT_REQUIRED');
 return p;
}
function authorize(a,p,proposalSha256,codeSha256){
 check(a?.authorized===true&&a.scope===p.scope&&a.environment===p.environment&&a.proposalSha256===proposalSha256&&a.codeSha256===codeSha256&&a.select===true&&a.effects===false&&a.applicationLogins===false&&a.runtimeChanges===false&&/^[a-f0-9]{32}$/.test(a.authorizationId),'SPECIFIC_AUTHORIZATION_REQUIRED');
}
async function runPreflight({proposal,query,onProgress}){
 const p=validateProposal(proposal);check(typeof onProgress==='function','DURABLE_PROGRESS_REQUIRED');let issued=0,config,rules,cabinet;
 const proof={},counts={},one=rows=>{check(rows.length===1,'CONTROL_NOT_UNIQUE');return rows[0];};
 const progress=(stage,phase)=>{const r=onProgress({stage,phase,selectRequests:issued,counts:{...counts},proof:{...proof}});check(!r||typeof r.then!=='function','SYNCHRONOUS_PROGRESS_REQUIRED');};
 for(const c of p.controls){
  check(issued<12,'BUDGET_EXHAUSTED');issued++;progress(c.key,'REQUESTED');
  const rows=await query(c.origin,c.sql,c.parameters);check(Array.isArray(rows),'INVALID_ROWS');counts[c.key]=rows.length;
  switch(c.key){
   case 'publication_recipe':{const row=one(rows);check(String(row.version_id)==='38','PUBLICATION_CHANGED');config=parse(row.configuracion_doc);check(String(config.GabineteId)==='10'&&config.OrigenGabinete==='DA'&&config.ModoIntegridad==='COMPLETO_OBLIGATORIO'&&config.AmbitoIntegridad==='LOTE','CONFIGURATION_CHANGED');proof.publicationVerified=true;break;}
   case 'publication_rules':{
    check(rows.length>0&&rows.length<=100,'RULES_UNAVAILABLE');rules=rows.map(r=>parse(r.regla_doc));proof.publicationSha256=sha(JSON.stringify({config,rules}));check(proof.publicationSha256===p.expectedPublicationSha256,'PUBLICATION_CHANGED');
    for(const field of ['CODIGO','NOMBRE']){const hits=rules.filter(r=>r.DestinoTipo==='CAMPO_EXPEDIENTE'&&r.DestinoRef===field);check(hits.length===1&&hits[0].FuenteTipo==='CARPETA'&&hits[0].Extraccion==='COMPLETO'&&hits[0].EtapaResolucion==='PREFLIGHT'&&hits[0].TipoValor==='TEXTO'&&!hits[0].QuitarExtension&&p.proposedRootNames.every(n=>n.length<=hits[0].LongitudMaxima),'ROOT_RECIPE_CHANGED');}
    proof.rootRecipeVerified=true;break;
   }
   case 'root_absence':case 'prior_plan_absence':case 'prior_root_absence':case 'individual_filename_absence':check(rows.length===0,'PRIOR_RESOURCE_FOUND');proof[c.key]=true;break;
   case 'cabinet':{const row=one(rows);check(String(row.id_gabinete)==='10'&&/^[A-Za-z_][A-Za-z0-9_]{0,63}$/.test(row.NOMBRE),'CABINET_CHANGED');cabinet=row.NOMBRE;proof.cabinetVerified=true;break;}
   case 'visible_indices':{check(rows.length>0&&rows.length<=100&&rows.every(r=>/^[A-Za-z_][A-Za-z0-9_]{0,63}$/.test(r.CAMPO)),'INDICES_UNAVAILABLE');const expected=rules.filter(r=>r.DestinoTipo==='INDICE_GABINETE');check(expected.length>0&&expected.every(r=>rows.filter(f=>String(f.id_detalle_gabinete)===r.DestinoRef).length===1),'INDEX_REFERENCE_CHANGED');proof.indicesVerified=true;break;}
   case 'individual_destination':{const row=one(rows);check(String(row.ID_EXPEDIENTE)==='467'&&String(row.ID_EMPRESA_EXPEDIENTE)==='2'&&String(row.nivel_id)==='7'&&row.GABINETE_PRODUCION===cabinet&&id(row.relacion_id),'INDIVIDUAL_DESTINATION_CHANGED');proof.individualRelationId=String(row.relacion_id);proof.individualClassificationSha256=sha(JSON.stringify(row));proof.individualDestinationVerified=true;break;}
   case 'individual_typology':check(String(one(rows).type_id)==='50','INDIVIDUAL_TYPOLOGY_CHANGED');proof.individualTypologyVerified=true;break;
   case 'level_authority':check(String(one(rows).id_nivel)==='7','LEVEL_AUTHORITY_UNAVAILABLE');proof.levelAuthorityObserved=true;break;
   case 'protected_attempts':{
    check(rows.length===3,'PROTECTED_ATTEMPTS_CHANGED');const byId=new Map(rows.map(r=>[String(r.intento_id),r]));check(byId.size===3,'PROTECTED_ATTEMPTS_CHANGED');
    const old=byId.get('41'),a=byId.get('52'),b=byId.get('53');check(old&&a&&b&&String(old.lote_id)==='18'&&old.fase==='CREAR_EXPEDIENTE'&&old.estado==='EN_CURSO'&&!old.fin_utc&&[a,b].every(r=>String(r.lote_id)==='21'&&r.fase==='ALMACENAR')&&a.estado==='CONFIRMADO'&&!!a.fin_utc&&b.estado==='EN_CURSO'&&!b.fin_utc,'PROTECTED_ATTEMPTS_CHANGED');
    const confirmed=parse(a.evidencia_doc),uncertain=parse(b.evidencia_doc);check(confirmed.Estado==='CONFIRMADO'&&confirmed.DocumentoId==='16222'&&confirmed.InventarioId==='23320'&&uncertain.Estado==='EN_CURSO'&&!uncertain.DocumentoId&&!uncertain.InventarioId,'PROTECTED_EVIDENCE_CHANGED');
    proof.protectedAttemptsSha256=sha(JSON.stringify(rows));proof.protectedAttemptsVerified=true;break;
   }
  }
  progress(c.key,'VERIFIED');
 }
 return{status:'PASS',scenario:p.scope,selectRequests:issued,counts,proof,applicationLogins:0,preparations:0,uploads:0,documents:0,runtimeChanges:0,effectsAuthorized:false};
}
async function main(args=process.argv.slice(2)){
 const bytes=fs.readFileSync(path.join(ROOT,PROPOSAL)),p=validateProposal(parse(bytes)),proposalSha256=sha(bytes),codeSha256=codeFingerprint();
 if(args.length===1&&args[0]==='--check'){console.log('DOC117_ALM03_PREFLIGHT_CHECK='+JSON.stringify({status:'PASS',proposalSha256,codeSha256,maximumSelectRequests:12,realExecution:false}));return;}
 check(args.length===4&&args[0]==='--profile'&&args[2]==='--authorization','ARGUMENTS_INVALID');
 const profile=parse(fs.readFileSync(path.resolve(args[1]))),approvalFile=path.resolve(args[3]),approval=parse(fs.readFileSync(approvalFile));
 check(profile.baseUrl===p.environment&&profile.nivelRef==='7'&&profile.odbcDsn==='MembershipUsers'&&profile.docuarchiOdbcDsn==='MembershipUsers','PROFILE_CHANGED');
 const runtime=path.join(ROOT,'bin/GestionDocumental-Docuarchi.net.dll'),originalSha256=sha(fs.readFileSync(runtime));check(originalSha256==='4134f57f66fc259b5f33a553577c9a07e6bf176b2da027aaeeb8576325c282c5','RUNTIME_CHANGED');
 const verify=()=>{authorize(parse(fs.readFileSync(approvalFile)),p,proposalSha256,codeFingerprint());check(sha(fs.readFileSync(path.join(ROOT,PROPOSAL)))===proposalSha256&&sha(fs.readFileSync(runtime))===originalSha256,'PROTECTED_INPUT_CHANGED');};verify();tty.requireInteractiveConsole();
 const artifact=path.join(ROOT,'tools/e2e/artifacts','doc117-alm03-preflight-'+approval.authorizationId+'.json'),state={issueKey:'DOC-117',scope:p.scope,status:'AWAITING_CREDENTIALS',proposalSha256,codeSha256,originalSha256,selectRequests:0,counts:{},proof:{},createdUtc:new Date().toISOString()};
 fs.mkdirSync(path.dirname(artifact),{recursive:true});fs.writeFileSync(artifact,JSON.stringify(state,null,2)+'\n',{flag:'wx'});const save=()=>fs.writeFileSync(artifact,JSON.stringify(state,null,2)+'\n');
 const secrets={},environment=Object.fromEntries(Object.entries(process.env).filter(([k])=>['PATH','Path','SystemRoot','WINDIR','TEMP','TMP','ComSpec','PATHEXT'].includes(k)));
 try{
  console.log('DOC-117 ALM-03: control autorizado de hasta 12 SELECT, sin login de aplicacion, cargas o cambios de DLL. No repetir.');
  await tty.collectValue(secrets,'user','Cuenta MySQL exclusivamente SELECT',{secret:true});await tty.collectValue(secrets,'password','Contrasena MySQL de lectura',{secret:true});verify();
  for(const [prefix,dsn]of [['DOC111_E2E',profile.odbcDsn],['DOC111_DA_E2E',profile.docuarchiOdbcDsn]])Object.assign(environment,{[prefix+'_ODBC_DSN']:dsn,[prefix+'_MYSQL_USER']:secrets.user,[prefix+'_MYSQL_PASSWORD']:secrets.password});
  const result=await runPreflight({proposal:p,query:async(origin,sql,params)=>{verify();return queryRows(sql,params,environment,origin==='RA'?'DOC111_E2E':'DOC111_DA_E2E');},onProgress:v=>{Object.assign(state,v,{status:'RUNNING'});save();}});
  verify();Object.assign(state,result,{runtimeUnchanged:true,finishedUtc:new Date().toISOString()});save();console.log('DOC117_ALM03_PREFLIGHT_RESULT='+JSON.stringify(result));
 }catch(error){state.status='STOPPED_REVIEW_REQUIRED';state.code=/^(?:DOC117_ALM03_PREFLIGHT|DOC111_SELECT)_[A-Z0-9_]+$/.test(error?.message||'')?error.message:'DOC117_ALM03_PREFLIGHT_STOPPED';state.runtimeUnchanged=sha(fs.readFileSync(runtime))===originalSha256;state.finishedUtc=new Date().toISOString();save();console.log('DOC117_ALM03_PREFLIGHT_STOPPED='+JSON.stringify({status:state.status,code:state.code,selectRequests:state.selectRequests,runtimeUnchanged:state.runtimeUnchanged}));process.exitCode=2;}
 finally{for(const k of Object.keys(secrets))delete secrets[k];for(const k of Object.keys(environment))delete environment[k];}
}
if(require.main===module)main().catch(()=>{console.error('DOC117_ALM03_PREFLIGHT_NOT_STARTED');process.exitCode=2;});
module.exports={ROOT,PROPOSAL,REVIEW_FILES,sha,codeFingerprint,validateProposal,authorize,runPreflight};
