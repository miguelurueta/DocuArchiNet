'use strict';
const fs=require('node:fs'),path=require('node:path');
const {ROOT,sha}=require('./run-doc117-alm03-preflight.cjs'),{queryRows}=require('./support/doc32-e2e-odbc.cjs'),tty=require('./support/interactive-e2e-console.cjs');
const PROPOSAL='Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc117-alm03-destino-diagnostico-20261106.json';
const REVIEW_FILES=['tools/e2e/scripts/run-doc117-alm03-destination-readonly.cjs','tools/e2e/scripts/run-doc117-alm03-preflight.cjs','tools/e2e/scripts/support/doc32-e2e-odbc.cjs','tools/e2e/scripts/query-doc32-odbc-fingerprint.ps1','tools/e2e/scripts/support/interactive-e2e-console.cjs'];
const codeFingerprint=()=>sha(REVIEW_FILES.map(f=>sha(fs.readFileSync(path.join(ROOT,f)))).join('\n')),parse=v=>JSON.parse(String(v).replace(/^\uFEFF/,'')),check=(v,c)=>{if(!v)throw Error('DOC117_ALM03_DESTINATION_'+c);};
const id=v=>/^[1-9][0-9]{0,9}$/.test(String(v))&&BigInt(v)<=4294967295n;
function validateProposal(p){
 check(p?.issueKey==='DOC-117'&&p.scope==='ALM-03-DESTINATION-SELECT'&&p.environment==='https://localhost/GestionDocumental-Docuarchi.net/'&&p.authorized===false&&p.priorSelectRequests===8&&p.maximumSelectRequests===4&&p.cumulativeMaximumSelectRequests===12&&p.effects===false&&p.applicationLogins===false&&p.runtimeChanges===false,'SCOPE_CHANGED');
 check(p.priorArtifact==='tools/e2e/artifacts/doc117-alm03-preflight-33a33a3671b9e27a0dc92aeece3e1f22.json'&&/^[a-f0-9]{64}$/.test(p.priorArtifactSha256)&&JSON.stringify(p.controls.map(c=>c.key))===JSON.stringify(['individual_expedition','individual_relations','individual_typology','protected_attempts']),'BASELINE_CHANGED');
 for(const c of p.controls)check(c.origin==='RA'&&/^SELECT\b/.test(c.sql)&&!/[;]|--|\/\*|\b(?:INSERT|UPDATE|DELETE|CALL|EXEC|DROP|ALTER|CREATE|REPLACE|TRUNCATE|GRANT|REVOKE|SET|USE|LOAD|OUTFILE|INTO|LOCK|SLEEP|BENCHMARK)\b/i.test(c.sql)&&Array.isArray(c.parameters)&&(c.sql.match(/\?/g)||[]).length===c.parameters.length,'SELECT_REQUIRED');return p;
}
function authorize(a,p,proposalSha256,codeSha256){check(a?.authorized===true&&a.scope===p.scope&&a.environment===p.environment&&a.proposalSha256===proposalSha256&&a.codeSha256===codeSha256&&a.select===true&&a.effects===false&&a.applicationLogins===false&&a.runtimeChanges===false&&a.priorArtifactSha256===p.priorArtifactSha256&&/^[a-f0-9]{32}$/.test(a.authorizationId),'SPECIFIC_AUTHORIZATION_REQUIRED');}
async function runDiagnostic({proposal,query,onProgress}){
 const p=validateProposal(proposal);check(typeof onProgress==='function','DURABLE_PROGRESS_REQUIRED');let issued=0;const counts={},proof={};
 const step=(stage,phase)=>{const x=onProgress({stage,phase,selectRequests:issued,cumulativeSelectRequests:8+issued,counts:{...counts},proof:{...proof}});check(!x||typeof x.then!=='function','SYNCHRONOUS_PROGRESS_REQUIRED');};
 for(const c of p.controls){
  check(issued<4&&8+issued<12,'BUDGET_EXHAUSTED');issued++;step(c.key,'REQUESTED');const rows=await query(c.origin,c.sql,c.parameters);check(Array.isArray(rows),'INVALID_ROWS');counts[c.key]=rows.length;step(c.key,'OBSERVED');
  switch(c.key){
   case 'individual_expedition':check(rows.length<=1&&rows.every(r=>String(r.ID_EXPEDIENTE)==='467'&&String(r.ID_EMPRESA_EXPEDIENTE)==='2'),'EXPEDITION_AMBIGUOUS');proof.individualExpedientFound=rows.length===1;if(rows.length===1)proof.individualClassificationSha256=sha(JSON.stringify(rows[0]));break;
   case 'individual_relations':check(rows.length<=8&&rows.every(r=>id(r.relacion_id)&&id(r.nivel_id)&&(String(r.parent_id)==='0'||r.parent_id===null||id(r.parent_id))&&['0','1'].includes(String(r.estado_nivel))),'RELATIONS_REQUIRE_REVIEW');proof.individualRelations=rows.map(r=>({relacionId:String(r.relacion_id),nivelId:String(r.nivel_id),parentId:r.parent_id===null?null:String(r.parent_id),active:String(r.estado_nivel)==='1'}));break;
   case 'individual_typology':check(rows.length<=1&&rows.every(r=>String(r.type_id)==='50'),'TYPOLOGY_AMBIGUOUS');proof.individualTypologyAvailable=rows.length===1;break;
   case 'protected_attempts':{
    check(rows.length===3,'PROTECTED_ATTEMPTS_CHANGED');const map=new Map(rows.map(r=>[String(r.intento_id),r])),old=map.get('41'),a=map.get('52'),b=map.get('53');
    check(map.size===3&&old&&a&&b&&String(old.lote_id)==='18'&&old.fase==='CREAR_EXPEDIENTE'&&old.estado==='EN_CURSO'&&!old.fin_utc&&String(a.lote_id)==='21'&&a.fase==='ALMACENAR'&&a.estado==='CONFIRMADO'&&!!a.fin_utc&&String(b.lote_id)==='21'&&b.fase==='ALMACENAR'&&b.estado==='EN_CURSO'&&!b.fin_utc,'PROTECTED_ATTEMPTS_CHANGED');
    const confirmed=parse(a.evidencia_doc),uncertain=parse(b.evidencia_doc);check(confirmed.Estado==='CONFIRMADO'&&confirmed.DocumentoId==='16222'&&confirmed.InventarioId==='23320'&&uncertain.Estado==='EN_CURSO'&&!uncertain.DocumentoId&&!uncertain.InventarioId,'PROTECTED_EVIDENCE_CHANGED');proof.protectedAttemptsVerified=true;proof.protectedAttemptsSha256=sha(JSON.stringify(rows));break;
   }
  }
  step(c.key,'VERIFIED');
 }
 return{status:'DIAGNOSTIC_COMPLETE',scenario:p.scope,selectRequests:issued,priorSelectRequests:8,cumulativeSelectRequests:8+issued,counts,proof,preflightAccepted:false,realAlm03Pass:false,effectsAuthorized:false,applicationLogins:0,documents:0,runtimeChanges:0};
}
async function main(args=process.argv.slice(2)){
 const bytes=fs.readFileSync(path.join(ROOT,PROPOSAL)),p=validateProposal(parse(bytes)),proposalSha256=sha(bytes),codeSha256=codeFingerprint();
 const prior=fs.readFileSync(path.join(ROOT,p.priorArtifact)),priorState=parse(prior);check(sha(prior)===p.priorArtifactSha256&&priorState.status==='STOPPED_REVIEW_REQUIRED'&&priorState.selectRequests===8&&priorState.stage==='individual_destination','BASELINE_CHANGED');
 if(args.length===1&&args[0]==='--check'){console.log('DOC117_ALM03_DESTINATION_CHECK='+JSON.stringify({status:'PASS',proposalSha256,codeSha256,maximumSelectRequests:4,priorSelectRequests:8,cumulativeMaximum:12,realExecution:false}));return;}
 check(args.length===4&&args[0]==='--profile'&&args[2]==='--authorization','ARGUMENTS_INVALID');const profile=parse(fs.readFileSync(path.resolve(args[1]))),approvalFile=path.resolve(args[3]),approval=parse(fs.readFileSync(approvalFile));
 check(profile.baseUrl===p.environment&&profile.odbcDsn==='MembershipUsers'&&profile.docuarchiOdbcDsn==='MembershipUsers','PROFILE_CHANGED');const runtime=path.join(ROOT,'bin/GestionDocumental-Docuarchi.net.dll'),originalSha256=sha(fs.readFileSync(runtime));check(originalSha256==='4134f57f66fc259b5f33a553577c9a07e6bf176b2da027aaeeb8576325c282c5','RUNTIME_CHANGED');
 const verify=()=>{authorize(parse(fs.readFileSync(approvalFile)),p,proposalSha256,codeFingerprint());check(sha(fs.readFileSync(path.join(ROOT,PROPOSAL)))===proposalSha256&&sha(fs.readFileSync(path.join(ROOT,p.priorArtifact)))===p.priorArtifactSha256&&sha(fs.readFileSync(runtime))===originalSha256,'PROTECTED_INPUT_CHANGED');};verify();tty.requireInteractiveConsole();
 const artifact=path.join(ROOT,'tools/e2e/artifacts','doc117-alm03-destination-'+approval.authorizationId+'.json'),state={issueKey:'DOC-117',scope:p.scope,status:'AWAITING_CREDENTIALS',proposalSha256,codeSha256,originalSha256,priorArtifactSha256:p.priorArtifactSha256,priorSelectRequests:8,selectRequests:0,cumulativeSelectRequests:8,createdUtc:new Date().toISOString()};
 fs.writeFileSync(artifact,JSON.stringify(state,null,2)+'\n',{flag:'wx'});const save=()=>fs.writeFileSync(artifact,JSON.stringify(state,null,2)+'\n'),secrets={},environment=Object.fromEntries(Object.entries(process.env).filter(([k])=>['PATH','Path','SystemRoot','WINDIR','TEMP','TMP','ComSpec','PATHEXT'].includes(k)));
 try{
  console.log('DOC-117 ALM-03: cuatro SELECT diagnosticos; total acumulado maximo 12. No hay login de aplicacion, cargas o cambios de DLL.');await tty.collectValue(secrets,'user','Cuenta MySQL exclusivamente SELECT',{secret:true});await tty.collectValue(secrets,'password','Contrasena MySQL de lectura',{secret:true});verify();
  Object.assign(environment,{DOC111_E2E_ODBC_DSN:profile.odbcDsn,DOC111_E2E_MYSQL_USER:secrets.user,DOC111_E2E_MYSQL_PASSWORD:secrets.password});
  const result=await runDiagnostic({proposal:p,query:async(origin,sql,params)=>{verify();return queryRows(sql,params,environment,'DOC111_E2E');},onProgress:v=>{Object.assign(state,v,{status:'RUNNING'});save();}});verify();Object.assign(state,result,{runtimeUnchanged:true,finishedUtc:new Date().toISOString()});save();console.log('DOC117_ALM03_DESTINATION_RESULT='+JSON.stringify(result));
 }catch(error){state.status='STOPPED_REVIEW_REQUIRED';state.code=/^(?:DOC117_ALM03_DESTINATION|DOC111_SELECT)_[A-Z0-9_]+$/.test(error?.message||'')?error.message:'DOC117_ALM03_DESTINATION_STOPPED';state.runtimeUnchanged=sha(fs.readFileSync(runtime))===originalSha256;state.finishedUtc=new Date().toISOString();save();console.log('DOC117_ALM03_DESTINATION_STOPPED='+JSON.stringify({status:state.status,code:state.code,cumulativeSelectRequests:state.cumulativeSelectRequests}));process.exitCode=2;}
 finally{for(const k of Object.keys(secrets))delete secrets[k];for(const k of Object.keys(environment))delete environment[k];}
}
if(require.main===module)main().catch(()=>{console.error('DOC117_ALM03_DESTINATION_NOT_STARTED');process.exitCode=2;});
module.exports={ROOT,PROPOSAL,REVIEW_FILES,sha,codeFingerprint,validateProposal,authorize,runDiagnostic};
