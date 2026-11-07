'use strict';
const {validateScope,scopeDigest,validateProof}=require('../../scripts/support/doc117-alm03-guard-diagnostic.cjs');
const {failureCode,validateUploadOutcome}=require('../../scripts/support/doc117-alm03-failure-evidence.cjs');
const check=(v,c)=>{if(!v)throw Error('DOC117_ALM03_PROBE_'+c);};
function authorize(a,s){check(a?.scope==='ALM-03-GUARD-DIAGNOSTIC'&&a.resourceScopeSha256===scopeDigest(s)&&['authorized','environmentAuthorized','account','select','temporaryUpload','runtimeChanges','exclusive','discardable'].every(k=>a[k]===true)&&a.storage===false&&a.imports===false,'SPECIFIC_AUTHORIZATION_REQUIRED');}
async function runProbe({scope,authorization,invoke,readProof,onProgress}){
 validateScope(scope);authorize(authorization,scope);check(typeof onProgress==='function','DURABLE_PROGRESS_REQUIRED');let uploads=0,diagnostic,knownIds=false,code='UNCLASSIFIED_FAILURE';
 const step=value=>{const result=onProgress(value);if(result&&typeof result.then==='function'){Promise.resolve(result).catch(()=>{});check(false,'SYNCHRONOUS_PROGRESS_REQUIRED');}};
 try{
  await invoke({beforeStorage:()=>{check(uploads===0,'UPLOAD_ALREADY_REQUESTED');uploads++;step({stage:'PROBE_UPLOAD_REQUESTED',uploadNumber:uploads});},onStored:ref=>{knownIds=true;const safe=Object.fromEntries(['documentoId','inventarioId','expedienteId'].filter(k=>typeof ref?.[k]==='string'&&/^[1-9][0-9]{0,9}$/.test(ref[k])&&BigInt(ref[k])<=4294967295n).map(k=>[k,ref[k]]));step({stage:'PROBE_UNEXPECTED_IDENTITIES',...safe});throw Error('DOC117_ALM03_PROBE_UNEXPECTED_DOCUMENT');},onStorageOutcome:value=>{diagnostic=validateUploadOutcome(value);step({stage:'PROBE_UI_DIAGNOSTIC',diagnostic});}});
  code='UNEXPECTED_UI_SUCCESS';
 }catch(error){code=failureCode(error);}
 step({stage:'PROBE_UI_SETTLED',code,uploads,knownIds});
 // Even an uncertain UI outcome is returned for restoration and read-only
 // verification. It never causes a second request to obtain another result.
 let proof=null,proofCode=null;try{const raw=await readProof();if(raw)proof=validateProof(raw,scope);}catch{proofCode='PROOF_UNAVAILABLE_OR_INVALID';}
 const completed=uploads===1&&!knownIds&&code==='PRODUCCION_DOCUMENTAL_E2E_DIAGNOSTIC_WRITER_BLOCKED'&&diagnostic?.outcome==='FAILED'&&diagnostic.phase==='SERVER_RESULT'&&diagnostic.httpStatus===200&&diagnostic.serverResult==='REJECTED'&&diagnostic.uploadRequestsObserved===1&&!diagnostic.identitiesObserved&&!diagnostic.projectionVerified&&!!proof;
 if(proof)step({stage:'PROBE_NATIVE_DIAGNOSTIC',proof});
 return{status:completed?'DIAGNOSTIC_COMPLETE_NO_WRITER':'DIAGNOSTIC_INCONCLUSIVE',scenario:'ALM-03-GUARD-DIAGNOSTIC',uploadRequests:uploads,uiCode:code,uiDiagnostic:diagnostic??null,proof,proofCode,knownDocumentResponseObserved:knownIds,writerInvoked:proof?.writerInvoked??null,businessEffectsVerified:false,alm03ConcurrentPass:false};
}
module.exports={runProbe,authorize};
