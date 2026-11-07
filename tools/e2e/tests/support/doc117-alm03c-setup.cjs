'use strict';
const legacy=require('./doc117-alm03-setup.cjs'),scope=require('./doc117-alm03c-scope.cjs');
const {readBinding}=require('../../scripts/support/doc117-alm03-canonical-description.cjs');
const check=(v,c)=>{if(!v)throw Error('DOC117_ALM03C_'+c);};
const BUDGET=Object.freeze({plans:1,lots:1,receipts:2,createdExpedients:2,materializations:2,documents:0,storageRequests:0,runtimeChanges:0,maximumSelectRequests:160});
function validateProposal(p){
 check(p?.issueKey==='DOC-117'&&p.scope==='ALM-03C-SETUP'&&p.status==='PROPOSED_NOT_EXECUTED'&&p.environment==='https://localhost/GestionDocumental-Docuarchi.net/'&&p.identity?.userRef==='141'&&p.identity.ambitoRef==='2'&&p.levelId==='7'&&p.versionId==='38'&&p.cabinetId==='10'&&p.proofLotId==='21','SCOPE_INVALID');
 check(JSON.stringify(p.rootNames)===JSON.stringify(scope.ROOT_NAMES)&&JSON.stringify(p.fileNames)===JSON.stringify(scope.FILE_NAMES)&&p.individual?.expedienteId==='467'&&p.individual.levelId==='8'&&p.individual.relacionNivelId==='1072'&&p.individual.typologyId==='50'&&p.individual.fileName===scope.INDIVIDUAL_NAME&&/^[a-f0-9]{64}$/.test(p.individual.classificationSha256),'RESOURCES_CHANGED');
 check(p.fixture?.file==='doc111-carga-individual.pdf'&&p.fixture.bytes===677&&p.fixture.sha256==='a5f1924afbae4bd631de893c49a56d9f09219075113e6da2ec983602cff84c6c','FIXTURE_CHANGED');
 check(JSON.stringify(p.budget)===JSON.stringify(BUDGET)&&p.priorSelectRequests===250&&p.previousCumulativeSelectCeiling===320&&p.maximumCumulativeSelectRequests===410&&p.budgetIncreaseRequired===true,'BUDGET_CHANGED');
 check(p.expected?.typologyId==='79'&&p.expected.indexId==='14'&&p.expected.indexType==='TEXTO'&&p.expected.indexValue==='EEE'&&p.preserveAllExistingResources===true&&p.preserveNewResources===true&&p.noRetry===true&&JSON.stringify(p.preservedLotIds)==='["18","19","20","21","22"]','PRESERVATION_REQUIRED');
 check(/^[a-f0-9]{64}$/.test(p.publicationSha256)&&p.originalRuntimeSha256==='4134f57f66fc259b5f33a553577c9a07e6bf176b2da027aaeeb8576325c282c5','BASELINE_CHANGED');return p;
}
function authorize(a,p){validateProposal(p);legacy.authorize(a,p);check(a.budgetIncreaseAuthorized===true&&a.priorSelectRequests===250&&a.maximumAdditionalSelectRequests===160&&a.maximumCumulativeSelectRequests===410&&a.individualUpload===false,'BUDGET_INCREASE_AUTHORIZATION_REQUIRED');}
async function runSetup(options){
 const p=validateProposal(options.proposal);authorize(options.authorization,p);check(options.resumePlanId===undefined,'RESUME_NOT_ALLOWED');
 check(Buffer.isBuffer(options.bytes)&&options.bytes.length===677&&scope.sha(options.bytes)===p.fixture.sha256,'FIXTURE_CHANGED');
 const documents=async()=>{const count=await options.query('RA','SELECT COUNT(*) AS total FROM registro_producion_documental WHERE ID_USUARIO_GESTION=? AND ID_EMPRESA_DOCUMENTO=?',['141','2']),rows=await options.query('RA','SELECT r.* FROM registro_producion_documental r WHERE r.EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE=? AND r.ID_EMPRESA_DOCUMENTO=? ORDER BY r.ID_REGISTRO_PRODUCION_DOCUMENTAL LIMIT 2001',['467','2']);check(Array.isArray(count)&&count.length===1&&/^[0-9]+$/.test(String(count[0].total))&&Array.isArray(rows)&&rows.length<2001,'DOCUMENT_CONTROL_INVALID');return{inventory:String(count[0].total),inventoryScope:'ACTOR_141_COMPANY_2',individualCount:rows.length,individualSha256:scope.sha(JSON.stringify(rows))};};
 const recordCounts=(stage,value)=>{const result=options.onProgress({stage,inventoryCount:value.inventory,individualCount:String(value.individualCount)});if(result&&typeof result.then==='function'){Promise.resolve(result).catch(()=>{});check(false,'SYNCHRONOUS_PROGRESS_REQUIRED');}};
 check(typeof options.onProgress==='function','DURABLE_PROGRESS_REQUIRED');const binding=await readBinding(options.query),before=await documents();recordCounts('DOCUMENT_BASELINE_OBSERVED',before);check(before.inventory==='8814','PRIOR_INVENTORY_CHANGED');const dependencies={...options.dependencies,validateProposal,
  validatePlan:(plan,bounds)=>check(BigInt(plan.planId)>38n&&BigInt(plan.planId)>BigInt(bounds.maxPlan),'HISTORICAL_PLAN_REFUSED'),
  validateLot:(lot,bounds)=>check(BigInt(lot.loteId)>22n&&BigInt(lot.loteId)>BigInt(bounds.maxLot),'HISTORICAL_LOT_REFUSED'),
  validatePaired:paired=>check(paired.length===2&&paired.every(r=>BigInt(r.rootId)>36n&&BigInt(r.fileId)>37n)&&new Set(paired.map(r=>r.rootId)).size===2&&new Set(paired.map(r=>r.fileId)).size===2,'HISTORICAL_RESOURCES_REFUSED'),
  validateIdentity:(result,paired)=>check(BigInt(result.expedienteId)>667n&&BigInt(result.relacionNivelId)>1092n&&!paired.some(r=>r.identity&&(r.identity.ExpedienteId===result.expedienteId||r.identity.RelacionNivelId===result.relacionNivelId)),'HISTORICAL_EXPEDIENT_REFUSED'),
  createResourceScope:s=>scope.validateScope({...s,caseId:scope.CASE,protocol:scope.PROTOCOL,descriptionBinding:binding}),
  verifyPreparedScope:async()=>{check((await readBinding(options.query)).descriptionSha256===binding.descriptionSha256,'CANONICAL_DESCRIPTION_CHANGED');const after=await documents();recordCounts('DOCUMENT_FINAL_OBSERVED',after);check(JSON.stringify(after)===JSON.stringify(before),'PRIOR_DOCUMENTS_CHANGED');}};
 const result=await legacy.runSetup({...options,dependencies});return{...result,scenario:'ALM-03C-SETUP',canonicalDescriptionBound:true,priorSelectRequests:250,documentPreservation:before};
}
module.exports={BUDGET,validateProposal,authorize,runSetup};
