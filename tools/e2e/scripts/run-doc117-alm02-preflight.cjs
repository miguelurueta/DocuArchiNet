'use strict';
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const {queryRows}=require('./support/doc32-e2e-odbc.cjs');
const tty=require('./support/interactive-e2e-console.cjs');
const ROOT=path.resolve(__dirname,'../../..');
const PROPOSAL='Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc117-alm02-preflight-propuesto-20261106.json';
const sha=b=>crypto.createHash('sha256').update(b).digest('hex');
const requireValue=(v,c)=>{if(!v)throw Error('DOC117_ALM02_'+c);};
const json=value=>{try{return JSON.parse(value);}catch{throw Error('DOC117_ALM02_INVALID_JSON');}};
function codeFingerprint(){return sha(['tools/e2e/scripts/run-doc117-alm02-preflight.cjs','tools/e2e/scripts/support/doc32-e2e-odbc.cjs','tools/e2e/scripts/query-doc32-odbc-fingerprint.ps1','tools/e2e/scripts/support/interactive-e2e-console.cjs'].map(f=>sha(fs.readFileSync(path.join(ROOT,f)))).join('\n'));}
function validateProposal(p){
 requireValue(p.issueKey==='DOC-117'&&p.scope==='ALM-02-PREFLIGHT-SELECT'&&p.authorized===false&&p.environment==='https://localhost/GestionDocumental-Docuarchi.net/'&&p.levelId==='7'&&p.versionId==='38'&&p.cabinetId==='10'&&p.applicationIdentity.userRef==='141'&&p.applicationIdentity.ambitoRef==='2','SCOPE_CHANGED');
 requireValue(p.budget.maximumSelectRequests===8&&Object.entries(p.budget).filter(([k])=>k!=='maximumSelectRequests').every(([,v])=>v===0),'BUDGET_CHANGED');
 requireValue(JSON.stringify(p.controls.map(c=>c.key))===JSON.stringify(['publication_recipe','publication_rules','root_absence','prior_plan_absence','prior_root_absence','cabinet','visible_indices','protected_attempt']),'CONTROLS_CHANGED');
 requireValue(p.controls.every(c=>['RA','DA'].includes(c.origin)&&/^SELECT\b/.test(c.sql)&&!c.sql.includes(';')&&(c.sql.match(/\?/g)||[]).length===c.parameters.length),'SELECT_REQUIRED');return p;
}
function authorize(a,p,proposalSha256,codeSha256){
 requireValue(a?.authorized===true&&a.scope===p.scope&&a.environment===p.environment&&a.proposalSha256===proposalSha256&&a.codeSha256===codeSha256&&a.select===true&&a.effects===false&&a.applicationLogins===false&&a.runtimeChanges===false&&/^[a-f0-9]{32}$/.test(a.authorizationId),'SPECIFIC_AUTHORIZATION_REQUIRED');
}
async function runPreflight({proposal,query,onProgress=()=>{}}){
 const p=validateProposal(proposal),counts={},proof={publicationVerified:false,rootRecipeMatchesProposedNames:false,rootAbsenceVerified:false,priorPlansAbsent:false,priorRootsAbsent:false,cabinetVerified:false,visibleIndicesVerified:false,protectedAttemptObserved:false};
 let issued=0,config,rules,cabinet;
 const snapshot=()=>({selectRequests:issued,counts:{...counts},...proof});
 for(const c of p.controls){
  requireValue(issued<8,'BUDGET_EXHAUSTED');issued++;onProgress({stage:c.key,phase:'REQUESTED',...snapshot()});
  const rows=await query(c.origin,c.sql,c.parameters);requireValue(Array.isArray(rows),'INVALID_ROWS');counts[c.key]=rows.length;
  switch(c.key){
   case 'publication_recipe':
    requireValue(rows.length===1&&String(rows[0].version_id)==='38','PUBLICATION_UNAVAILABLE');config=json(rows[0].configuracion_doc);
    requireValue(String(config.GabineteId)==='10'&&config.OrigenGabinete==='DA'&&config.ModoIntegridad==='COMPLETO_OBLIGATORIO'&&config.AmbitoIntegridad==='LOTE','CONFIGURATION_CHANGED');proof.publicationVerified=true;break;
   case 'publication_rules':{
    requireValue(rows.length>0&&rows.length<=100,'RULES_UNAVAILABLE');rules=rows.map(r=>json(r.regla_doc));
    proof.publicationSha256=sha(JSON.stringify({config,rules}));
    // Conservative compatibility gate, not a replacement for the product resolver.
    for(const field of ['CODIGO','NOMBRE']){
     const hits=rules.filter(r=>r.DestinoTipo==='CAMPO_EXPEDIENTE'&&r.DestinoRef===field);
     requireValue(hits.length===1&&hits[0].FuenteTipo==='CARPETA'&&hits[0].Extraccion==='COMPLETO'&&hits[0].EtapaResolucion==='PREFLIGHT'&&hits[0].TipoValor==='TEXTO'&&!hits[0].QuitarExtension&&p.proposedRootNames.every(n=>n.length<=hits[0].LongitudMaxima),'ROOT_RECIPE_REQUIRES_REVIEW');
    }
    proof.rootRecipeMatchesProposedNames=true;break;
   }
   case 'root_absence':requireValue(rows.length===0,'ROOT_ALREADY_EXISTS');proof.rootAbsenceVerified=true;break;
   case 'prior_plan_absence':requireValue(rows.length===0,'PLAN_ALREADY_EXISTS');proof.priorPlansAbsent=true;break;
   case 'prior_root_absence':requireValue(rows.length===0,'ROOT_ALREADY_REFERENCED');proof.priorRootsAbsent=true;break;
   case 'cabinet':requireValue(rows.length===1&&String(rows[0].id_gabinete)==='10'&&/^[A-Za-z_][A-Za-z0-9_]{0,63}$/.test(rows[0].NOMBRE),'CABINET_UNAVAILABLE');cabinet=rows[0].NOMBRE;proof.cabinetVerified=true;break;
   case 'visible_indices':{
    requireValue(rows.length>0&&rows.length<=100&&rows.every(r=>/^[A-Za-z_][A-Za-z0-9_]{0,63}$/.test(r.CAMPO)),'INDICES_UNAVAILABLE');
    const expected=rules.filter(r=>r.DestinoTipo==='INDICE_GABINETE');
    requireValue(expected.length>0&&expected.every(r=>rows.filter(f=>String(f.id_detalle_gabinete)===r.DestinoRef).length===1)&&!!cabinet,'INDEX_REFERENCE_UNAVAILABLE');proof.visibleIndicesVerified=true;break;
   }
   case 'protected_attempt':requireValue(rows.length===1&&String(rows[0].intento_id)==='41'&&String(rows[0].lote_id)==='18'&&rows[0].estado==='EN_CURSO'&&rows[0].fase==='CREAR_EXPEDIENTE','PROTECTED_ATTEMPT_REQUIRES_REVIEW');proof.protectedAttemptObserved=true;break;
  }
  onProgress({stage:c.key,phase:'VERIFIED',...snapshot()});
 }
 return{status:'PASS',scenario:p.scope,...snapshot(),applicationLogins:0,preparations:0,confirmations:0,uploads:0,documents:0,runtimeChanges:0,executionAuthorized:false};
}
async function main(args=process.argv.slice(2)){
 const source=fs.readFileSync(path.join(ROOT,PROPOSAL)),proposal=validateProposal(json(source.toString('utf8'))),proposalSha256=sha(source),codeSha256=codeFingerprint();
 if(args.length===1&&args[0]==='--check'){console.log('DOC117_ALM02_PREFLIGHT_CHECK='+JSON.stringify({status:'PASS',proposalSha256,codeSha256,maximumSelectRequests:8,realExecution:false}));return;}
 requireValue(args.length===4&&args[0]==='--profile'&&args[2]==='--authorization','ARGUMENTS_INVALID');
 const profile=json(fs.readFileSync(path.resolve(args[1]),'utf8').replace(/^\uFEFF/,'')),approval=json(fs.readFileSync(path.resolve(args[3]),'utf8'));
 authorize(approval,proposal,proposalSha256,codeSha256);
 requireValue(profile.baseUrl===proposal.environment&&profile.nivelRef==='7'&&[profile.odbcDsn,profile.docuarchiOdbcDsn].every(x=>typeof x==='string'&&/^[A-Za-z0-9 _.-]+$/.test(x)),'RUNTIME_PROFILE_CHANGED');
 tty.requireInteractiveConsole();
 const stem='doc117-alm02-preflight-'+approval.authorizationId,artifact=path.join(ROOT,'tools/e2e/artifacts',stem+'.json');
 fs.mkdirSync(path.dirname(artifact),{recursive:true});
 const state={issueKey:'DOC-117',scope:proposal.scope,status:'AWAITING_CREDENTIALS',proposalSha256,codeSha256,selectRequests:0,counts:{},createdUtc:new Date().toISOString(),applicationLogins:0,preparations:0,confirmations:0,uploads:0,documents:0,runtimeChanges:0};
 fs.writeFileSync(artifact,JSON.stringify(state,null,2)+'\n',{flag:'wx'});
 const save=()=>fs.writeFileSync(artifact,JSON.stringify(state,null,2)+'\n');
 const runtime=path.join(ROOT,'bin/GestionDocumental-Docuarchi.net.dll'),runtimeSha256=sha(fs.readFileSync(runtime));
 const secrets={},environment=Object.fromEntries(Object.entries(process.env).filter(([k])=>!/^DOC\d+_/.test(k)));
 try{
  console.log('DOC-117 ALM-02: preflight ya autorizado. Maximo 8 SELECT; cero login de aplicacion, cargas o cambios de DLL.');
  console.log('Ingrese una cuenta MySQL exclusivamente SELECT. Ambos valores se enmascaran; nunca los pegue en el chat.');
  await tty.collectValue(secrets,'user','Cuenta MySQL de solo lectura',{secret:true});
  await tty.collectValue(secrets,'password','Contrasena MySQL de solo lectura',{secret:true});
  authorize(json(fs.readFileSync(path.resolve(args[3]),'utf8')),proposal,proposalSha256,codeFingerprint());requireValue(sha(fs.readFileSync(path.join(ROOT,PROPOSAL)))===proposalSha256,'PROPOSAL_CHANGED');
  for(const [prefix,dsn]of [['DOC111_E2E',profile.odbcDsn],['DOC111_DA_E2E',profile.docuarchiOdbcDsn]])Object.assign(environment,{[prefix+'_ODBC_DSN']:dsn,[prefix+'_MYSQL_USER']:secrets.user,[prefix+'_MYSQL_PASSWORD']:secrets.password});
  state.status='RUNNING';save();
  const result=await runPreflight({proposal,query:(origin,sql,params)=>queryRows(sql,params,environment,origin==='RA'?'DOC111_E2E':'DOC111_DA_E2E'),onProgress:p=>{Object.assign(state,p);save();}});
  requireValue(sha(fs.readFileSync(runtime))===runtimeSha256&&codeFingerprint()===codeSha256,'PROTECTED_SOURCE_CHANGED');
  Object.assign(state,result,{runtimeUnchanged:true,status:'PASS',finishedUtc:new Date().toISOString()});save();console.log('DOC117_ALM02_PREFLIGHT_RESULT='+JSON.stringify(result));
 }catch(error){state.status='STOPPED_REVIEW_REQUIRED';state.code=/^(?:DOC117_ALM02|DOC111_SELECT)_[A-Z0-9_]+$/.test(error?.message||'')?error.message:'DOC117_ALM02_PREFLIGHT_STOPPED';state.runtimeUnchanged=sha(fs.readFileSync(runtime))===runtimeSha256;state.finishedUtc=new Date().toISOString();save();console.log('DOC117_ALM02_PREFLIGHT_STOPPED='+JSON.stringify({status:state.status,stage:state.stage||'CREDENTIALS',code:state.code,selectRequests:state.selectRequests,runtimeUnchanged:state.runtimeUnchanged}));process.exitCode=2;}
 finally{for(const k of Object.keys(secrets))delete secrets[k];for(const k of Object.keys(environment))delete environment[k];}
 console.log('DOC117_ALM02_PREFLIGHT_FINISHED: no repita el comando; Codex revisara evidencia saneada.');
}
if(require.main===module)main().catch(()=>{console.error('DOC117_ALM02_PREFLIGHT_NOT_STARTED: revisar alcance y autorizacion.');process.exitCode=2;});
module.exports={runPreflight,validateProposal,authorize,codeFingerprint,PROPOSAL,ROOT,sha};
