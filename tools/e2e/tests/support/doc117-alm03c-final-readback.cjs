'use strict';
const {assertCheckpoint}=require('./doc117-alm03c-coexistence.cjs');
const check=(v,c)=>{if(!v)throw Error('DOC117_ALM03C_FINAL_'+c);};
// RegistrarResultadoDocumento consolidates inside its outer transaction after
// PersistirResultadoDocumento. Verify the resulting aggregate without mutation.
async function verifyFinalAfter({controls,scope,before,result,onProgress}){
 const preserved=await controls.preservation();check(JSON.stringify(preserved)===JSON.stringify(before.preserved),'PRESERVATION_CHANGED');onProgress({stage:'PRIOR_RESOURCES_PRESERVED',preserved});
 const counts=await controls.documentCounts(),inventory=await controls.inventoryCount(),view=await controls.snapshot(scope.lotId);
 const lotComplete=view.lot.estado==='COMPLETADO',attemptsComplete=view.attempts.length===6&&view.attempts.every(a=>a.estado==='CONFIRMADO'&&a.fin_utc),rootsComplete=view.roots.length===2&&view.roots.every(r=>r.estado==='COMPLETADA'&&!r.intento_activo_id);
 onProgress({stage:'FINAL_COUNTS_AND_AGGREGATE_OBSERVED',counts,inventory,lotComplete,attemptCount:view.attempts.length,attemptsComplete,rootCount:view.roots.length,rootsComplete});
 check(lotComplete,'LOT_STATE_INVALID');check(attemptsComplete,'ATTEMPTS_INVALID');check(rootsComplete,'ROOTS_INVALID');
 check(['PASS','NO_ACREDITADO'].includes(result.status)&&result.references?.length===3,'RESULT_INVALID');
 for(const key of ['IMPORT_A','IMPORT_B','INDIVIDUAL']){const row=counts.filter(r=>r.resourceKey===key),baseline=key==='INDIVIDUAL'?before.individual.counts:['0','0','0'];check(row.length===1&&row[0].counts.length===3&&row[0].counts.every((n,i)=>BigInt(n)===BigInt(baseline[i])+1n),'DOCUMENT_EFFECT_INVALID');}
 check(counts.length===3&&BigInt(inventory)===BigInt(before.inventoryBefore)+3n,'INVENTORY_INVALID');
 for(let i=0;i<2;i++){await controls.original(i);assertCheckpoint(view,scope,i,result.references[i],true);await controls.physical(view,i,result.references[i]);onProgress({stage:'IMPORT_FINAL_PHYSICAL_VERIFIED',resourceKey:i?'IMPORT_B':'IMPORT_A',...result.references[i]});}
 result.individualVerification=await controls.individualPhysical(result.references[2],before.individual);onProgress({stage:'INDIVIDUAL_FINAL_PHYSICAL_VERIFIED',...result.individualVerification});
 result.businessEvidenceVerified=true;result.alm03ConcurrentPass=result.methodObservation.writerBodyOverlapVerified;result.readbackComplete=true;result.observedDocumentEffects=3;result.inventoryAfterObserved=inventory;result.originalsVerified=true;result.finalLotState='COMPLETADO';result.finalRootStates=['COMPLETADA','COMPLETADA'];return true;
}
module.exports={verifyFinalAfter};
