'use strict';
const {createCanonicalControls}=require('./doc117-alm03-canonical-controls.cjs'),{preservation}=require('./doc116-exp01-setup.cjs');
const {assertCheckpoint}=require('./doc117-alm03c-coexistence.cjs');
const check=(v,c)=>{if(!v)throw Error('DOC117_ALM03C_'+c);};
function compose(query,scope,profile){
 const c=createCanonicalControls(query,scope,profile,scope.descriptionBinding),cabinet='`'+scope.individual.cabinetName+'`';
 c.preservation=()=>preservation(query,{identity:{userRef:'141',ambitoRef:'2'},preservedLotIds:['18','19','20','21','22']},{maxPlan:'38',maxLot:'22'});
 c.documentCounts=async()=>{const results=[];for(const [key,expedient]of [['IMPORT_A',scope.resources[0].expedienteId],['IMPORT_B',scope.resources[1].expedienteId],['INDIVIDUAL','467']]){const counts=[];for(const [sql,parameters]of [
  ['SELECT COUNT(*) AS total FROM '+cabinet+' WHERE ID_EXPEDIENTE=?',[expedient]],
  ['SELECT COUNT(*) AS total FROM registro_producion_documental WHERE EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE=? AND ID_EMPRESA_DOCUMENTO=?',[expedient,'2']],
  ['SELECT COUNT(*) AS total FROM ra_cert_indice_expediente WHERE expediente_archivo_ID_EXPEDIENTE=?',[expedient]]]){const rows=await query(sql.includes('FROM '+cabinet+' ')?'DA':'RA',sql,parameters);check(Array.isArray(rows)&&rows.length===1&&/^[0-9]+$/.test(String(rows[0].total)),'DOCUMENT_COUNT_INVALID');counts.push(String(rows[0].total));}results.push({resourceKey:key,counts});}return results;};
 return c;
}
async function verifyAfter({controls,scope,before,result,onProgress}){
 const preserved=await controls.preservation();check(JSON.stringify(preserved)===JSON.stringify(before.preserved),'PRESERVED_RESOURCES_CHANGED');onProgress({stage:'PRIOR_RESOURCES_PRESERVED',preserved});
 const counts=await controls.documentCounts(),inventory=await controls.inventoryCount(),view=await controls.snapshot(scope.lotId);onProgress({stage:'POST_COUNTS_OBSERVED',counts,inventory});
 let added=0;const increments={};for(const r of counts){const expectedBefore=r.resourceKey==='INDIVIDUAL'?before.individual.counts:['0','0','0'],differences=r.counts.map((n,i)=>BigInt(n)-BigInt(expectedBefore[i]));check(differences.every(n=>n===differences[0])&&[0n,1n].includes(differences[0]),'INCOMPLETE_DOCUMENT_EFFECT');increments[r.resourceKey]=Number(differences[0]);added+=Number(differences[0]);}check(BigInt(inventory)===BigInt(before.inventoryBefore)+BigInt(added),'UNEXPECTED_DOCUMENT_COUNT');
 await controls.original(0);await controls.original(1);
 const successful=['PASS','NO_ACREDITADO'].includes(result.status);
 if(successful){check(result.references?.length===3&&added===3&&view.lot.estado==='EN_CURSO'&&view.attempts.length===6&&view.attempts.every(a=>a.estado==='CONFIRMADO'&&a.fin_utc)&&view.roots.every(r=>r.estado==='EXPEDIENTE_CONFIRMADO'&&!r.intento_activo_id),'FINAL_LOT_NOT_CONFIRMED');for(let i=0;i<2;i++){assertCheckpoint(view,scope,i,result.references[i],true);await controls.physical(view,i,result.references[i]);}result.individualVerification=await controls.individualPhysical(result.references[2],before.individual);result.businessEvidenceVerified=true;result.alm03ConcurrentPass=result.methodObservation.writerBodyOverlapVerified;}
 else{result.alm03ConcurrentPass=false;result.businessEvidenceVerified=false;const refs=result.observedReferences||[];for(let i=0;i<2;i++){const ref=refs.find(r=>r.resourceKey===(i?'IMPORT_B':'IMPORT_A'));if(ref&&increments[ref.resourceKey]===1)await controls.physical(view,i,ref);}if(increments.INDIVIDUAL===0){const after=await controls.individualBefore();check(JSON.stringify(after)===JSON.stringify(before.individual),'INDIVIDUAL_PRIOR_RECORDS_CHANGED');result.individualPreservationVerified=true;}else{const ref=refs.find(r=>r.resourceKey==='INDIVIDUAL');if(ref){await controls.individualPhysical(ref,before.individual);result.individualPreservationVerified=true;}else result.individualPreservationVerified=false;}}
 result.readbackComplete=true;result.observedDocumentEffects=added;result.inventoryAfterObserved=inventory;result.originalsVerified=true;return true;
}
module.exports={compose,verifyAfter};
