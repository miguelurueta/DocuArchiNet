'use strict';
const crypto=require('node:crypto');
const {createPreparationClient}=require('./doc114-plan-importacion.cjs');
function check(ok,code){if(!ok)throw Error('DOC117_'+code);}
function closed(value,keys){check(value&&typeof value==='object'&&!Array.isArray(value)&&Object.keys(value).length===keys.length&&keys.every(k=>Object.prototype.hasOwnProperty.call(value,k)),'UNKNOWN_OR_MISSING_FIELD');}
const sha=value=>crypto.createHash('sha256').update(JSON.stringify(value)).digest('hex');
function validateManifest(m){
 closed(m,['version','issueKey','scenario','status','levelId','cabinetId','fixture','budget','provenance','cases']);
 check(m?.version===1&&m.issueKey==='DOC-117'&&m.scenario==='ALM-01'&&m.status==='PROPOSED_NOT_EXECUTED','MANIFEST_INVALID');
 check(m.levelId==='7'&&m.cabinetId==='10'&&Array.isArray(m.cases)&&m.cases.length===2,'SCOPE_CHANGED');
 check(JSON.stringify(m.budget)===JSON.stringify({preparations:2,confirmations:2,lots:2,uploads:2,roots:2,documents:2,storageRequests:4}),'BUDGET_CHANGED');
 check(m.fixture?.file==='doc111-carga-individual.pdf'&&m.fixture.bytes===677&&/^[a-f0-9]{64}$/.test(m.fixture.sha256),'PDF_CHANGED');
 const wanted=[['format-a','26','40','00011701-20261105','DOC117-79.pdf','00011701'],['format-b','27','42','00011702#05-11-2026','79.pdf','00011702']];
 m.cases.forEach((c,i)=>{closed(c,['key','perfilId','versionId','rootName','fileName','manifest','rules','expected']);closed(c.manifest,['nivelRef','versionId','raices','archivos']);closed(c.expected,['codigo','nombre','fechaInicial','tipologiaId','indices']);const [key,profile,version,root,file,code]=wanted[i];
  check(c.key===key&&c.perfilId===profile&&c.versionId===version&&c.rootName===root&&c.fileName===file,'CASE_CHANGED');
  check(c.manifest?.nivelRef===m.levelId&&c.manifest.versionId===version&&c.manifest.raices?.length===1&&c.manifest.archivos?.length===1,'MANIFEST_SCOPE_CHANGED');
  check(JSON.stringify(c.manifest.raices[0])===JSON.stringify({seleccionId:'s1',idCliente:'r1',nombre:root})&&JSON.stringify(c.manifest.archivos[0])===JSON.stringify({idCliente:'a1',raizIdCliente:'r1',nombre:file,rutaRelativa:root+'/'+file,bytes:'677',ultimaModificacionLocal:'0'}),'RESOURCE_CHANGED');
  check(c.expected?.codigo===code&&c.expected.nombre===root&&c.expected.fechaInicial==='2026-11-05'&&c.expected.tipologiaId==='79','EXPECTED_CHANGED');
  check(JSON.stringify(c.expected.indices)===JSON.stringify([{referencia:'14',tipo:'TEXTO',binding:'CONFIRMED_EXPEDIENTE_ID',value:null}]),'DEFERRED_ID_CHANGED');
  check(Array.isArray(c.rules)&&c.rules.length===5&&new Set(c.rules.map(r=>r.DestinoTipo+':'+r.DestinoRef)).size===5,'RULES_INVALID');
 });return m;
}
function comparePublication(c,snapshot,cabinetId){
 check(snapshot?.PerfilId===c.perfilId&&snapshot.VersionId===c.versionId&&snapshot.Configuracion?.GabineteId===Number(cabinetId),'PUBLICATION_CHANGED');
 check(snapshot.Configuracion.ModoIntegridad==='COMPLETO_OBLIGATORIO'&&snapshot.Configuracion.AmbitoIntegridad==='EXPEDIENTE','POLICY_CHANGED');
 check(Array.isArray(snapshot.Reglas)&&snapshot.Reglas.length===c.rules.length,'RULES_CHANGED');
 const known=Object.keys(c.rules[0]).filter(k=>k!=='ReglaId');
 for(const expected of c.rules){const found=snapshot.Reglas.filter(r=>r.DestinoTipo===expected.DestinoTipo&&r.DestinoRef===expected.DestinoRef);check(found.length===1,'RULES_CHANGED');const live=found[0];check(live.Proveedor==null&&known.every(k=>live[k]===expected[k]),'RULES_CHANGED');}
 return {perfilId:snapshot.PerfilId,versionId:snapshot.VersionId,snapshotSha256:sha(snapshot),rulesMatched:true};
}
async function inspectPublications({manifest,auth,profile,authorization,createClient=createPreparationClient,onPublication}){
 validateManifest(manifest);
 check(authorization?.environment===true&&authorization.account===true&&authorization.effects!==true&&authorization.discardable!==true,'READ_AUTHORIZATION_REQUIRED');
 const client=createClient(auth,profile,authorization,()=>{},{readOnly:true});await client.refreshToken();
 let page=await client.request('AbrirImportacion',{nivelRef:manifest.levelId}),seen=[];
 for(let n=1;n<=20;n++){
  check(Array.isArray(page.perfiles),'CATALOG_INVALID');seen.push(...page.perfiles);
  if(manifest.cases.every(c=>seen.some(s=>s.VersionId===c.versionId)))break;
  if(!(page.paginaPerfiles?.puedeHaberMas===true||page.puedeHaberMas===true))break;
  check(n<20,'PAGE_BUDGET_EXHAUSTED');page=await client.request('ListarPerfilesImportacion',{nivelRef:manifest.levelId,pagina:n+1,tamanoPagina:50});
 }
 const results=manifest.cases.map(c=>{const hits=seen.filter(s=>s.VersionId===c.versionId);check(hits.length===1,'PUBLICATION_NOT_UNIQUE');const proof=comparePublication(c,hits[0],manifest.cabinetId);onPublication?.(c,structuredClone(hits[0]));return proof;});
 return {status:'PASS',scenario:'ALM-01-PUBLICATIONS-READONLY',manifestSha256:sha(manifest),publications:results,preparations:0,confirmations:0,uploads:0,documents:0,rootAbsenceVerified:false,executionAuthorized:false};
}
module.exports={validateManifest,comparePublication,inspectPublications,sha};
