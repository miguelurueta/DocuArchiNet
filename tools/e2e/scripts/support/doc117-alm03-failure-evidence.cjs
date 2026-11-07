'use strict';
// Closed diagnostics only. Never retain messages, causes, stacks, bodies or URLs.
const UI_CODES = new Set(`
CABINET_CONFIGURATION_REJECTED CABINET_INDEX_REJECTED CABINET_REJECTED CONFIGURATION_REJECTED
CONTEXT_MISMATCH CONTEXT_REJECTED DATE_FIELDS_REJECTED DATE_REJECTED DOCUMENT_ANALYSIS_REJECTED
DIAGNOSTIC_WRITER_BLOCKED DIAGNOSTIC_CONTEXT_REJECTED DIAGNOSTIC_EVIDENCE_FAILED DIAGNOSTIC_RUNTIME_UNVERIFIED
DOCUMENT_CLASS_REJECTED DOCUMENT_TYPE_INVALID DOCUMENT_TYPE_MISMATCH DOCUMENT_TYPE_READ_FAILED
DOCUMENT_TYPE_UNAVAILABLE EXPEDIENT_AMBIGUOUS EXPEDIENT_NOT_LISTED EXPEDIENT_REJECTED EXTENSION_REJECTED
FILE_PREPARATION_FAILED FILE_SELECTION_FAILED FILE_UNAVAILABLE FIXTURE_INVALID FIXTURE_UNAVAILABLE
HANDLER_NULL_REFERENCE HANDLER_OUTPUT_REJECTED HANDLER_REQUEST_REJECTED HANDLER_SERIALIZATION_REJECTED
HANDLER_SESSION_REJECTED HANDLER_STORAGE_REJECTED HANDLER_TEMP_CONTEXT_REJECTED HANDLER_TEMP_FILE_REJECTED
HANDLER_TEMP_PATH_REJECTED INDEX_RELATION_REJECTED INVALID_SELECTION_REQUESTED_UPLOAD
INVALID_SELECTION_RESPONSE_UNSAFE LEVEL_POSTBACK_FAILED LEVEL_POSTBACK_UNAVAILABLE LEVEL_RECOVERY_FAILED
LEVEL_SELECTION_FAILED PERSISTENCE_REJECTED PREPARATION_RESPONSE_INVALID PREPARATION_TRANSPORT_FAILED
PROJECTION_FAILED SAVE_FAILED SAVE_UNAVAILABLE SELECTION_POSTBACK_FAILED SELECTION_POSTBACK_UNAVAILABLE
SELECTION_RECOVERY_FAILED STORAGE_CONFIGURATION_REJECTED STORAGE_ENVELOPE_INVALID STORAGE_EXCEPTION
STORAGE_FILE_NAME_INVALID STORAGE_HTTP_FAILED STORAGE_IMAGE_ID_INVALID STORAGE_RECORD_ID_INVALID
STORAGE_REJECTED STORAGE_REQUESTED STORAGE_RESPONSE_INVALID STORAGE_RESPONSE_UNAVAILABLE
UI_CONTRACT_INVALID UPLOAD_ACTIVATION_AMBIGUOUS UPLOAD_ACTIVATION_FAILED UPLOAD_COUNT_INVALID
UPLOAD_FUNCTION_UNAVAILABLE UPLOAD_INTERCEPTION_FAILED UPLOAD_SAVE_EXCEPTION UPLOAD_UNAVAILABLE
`.trim().split(/\s+/).map(c=>'PRODUCCION_DOCUMENTAL_E2E_'+c));
const ALM_CODES = new Set(`RESPONSE_UNCERTAIN RUNTIME_WINDOW_ENDED SPECIFIC_AUTHORIZATION_REQUIRED
TOKEN_REQUIRED INDIVIDUAL_IDENTITIES_INVALID INDIVIDUAL_DISPATCH_DUPLICATED REQUEST_BUDGET_EXHAUSTED
PAIR_RESULT_UNCERTAIN INDIVIDUAL_NOT_READY SETTLEMENT_NOT_DURABLE SYNCHRONOUS_CHECKPOINT_REQUIRED
DURABLE_CHECKPOINT_REQUIRED`.split(/\s+/).filter(Boolean).map(c=>'DOC117_ALM03_'+c));
const UNKNOWN='UNCLASSIFIED_FAILURE',known=code=>code===UNKNOWN||UI_CODES.has(code)||ALM_CODES.has(code);
const check=(v)=>{if(!v)throw Error('DOC117_ALM03_DIAGNOSTIC_INVALID');};
function failureCode(error){
 for(const key of ['code','message'])try{const code=error?.[key];if(UI_CODES.has(code)||ALM_CODES.has(code))return code;}catch{}
 return UNKNOWN;
}
function pairOutcomes(pair){return ['IMPORT_A','INDIVIDUAL'].map((resourceKey,i)=>Object.freeze({resourceKey,outcome:pair[i]?.status==='fulfilled'?'FULFILLED':pair[i]?.status==='not_dispatched'?'NOT_DISPATCHED':'REJECTED',code:pair[i]?.status==='fulfilled'||pair[i]?.status==='not_dispatched'?null:failureCode(pair[i]?.reason)}));}
function validatePairOutcomes(records){
 check(Array.isArray(records)&&records.length===2);
 return records.map((r,i)=>{check(r&&Object.keys(r).sort().join(',')==='code,outcome,resourceKey'&&r.resourceKey===['IMPORT_A','INDIVIDUAL'][i]&&['FULFILLED','REJECTED','NOT_DISPATCHED'].includes(r.outcome)&&(r.outcome==='REJECTED'?known(r.code):r.code===null));return Object.freeze({resourceKey:r.resourceKey,outcome:r.outcome,code:r.code});});
}
function pairFailure(code,records){check(ALM_CODES.has(code));const e=new Error(code);e.code=code;e.pairOutcomes=validatePairOutcomes(records);return e;}
const PHASES=['PREPARATION','DISPATCH_GUARD','SAVE','RESPONSE','ENVELOPE','SERVER_RESULT','IDENTITIES','PROJECTION','COMPLETED'];
function validateUploadOutcome(r){
 check(r&&Object.keys(r).sort().join(',')==='code,httpStatus,identitiesObserved,outcome,phase,projectionVerified,resourceKey,saveRequested,serverResult,uploadRequestsObserved,version');
 check(r.version===1&&r.resourceKey==='INDIVIDUAL'&&PHASES.includes(r.phase)&&['FAILED','COMPLETED'].includes(r.outcome)&&(r.outcome==='FAILED'?known(r.code):r.code===null)&&[r.saveRequested,r.identitiesObserved,r.projectionVerified].every(v=>typeof v==='boolean')&&Number.isInteger(r.uploadRequestsObserved)&&r.uploadRequestsObserved>=0&&r.uploadRequestsObserved<=4294967295&&(r.httpStatus===null||Number.isInteger(r.httpStatus)&&r.httpStatus>=100&&r.httpStatus<=599)&&['NOT_OBSERVED','ACCEPTED','REJECTED'].includes(r.serverResult));
 check(r.outcome!=='COMPLETED'||r.phase==='COMPLETED'&&r.saveRequested&&r.uploadRequestsObserved===1&&r.serverResult==='ACCEPTED'&&r.identitiesObserved&&r.projectionVerified);
 return Object.freeze({...r});
}
function uploadOutcome(state,error,failed=error!==undefined){return validateUploadOutcome({version:1,resourceKey:'INDIVIDUAL',outcome:failed?'FAILED':'COMPLETED',code:failed?failureCode(error):null,phase:state.phase,saveRequested:state.saveRequested,uploadRequestsObserved:state.uploadRequestsObserved,httpStatus:state.httpStatus,serverResult:state.serverResult,identitiesObserved:state.identitiesObserved,projectionVerified:state.projectionVerified});}
module.exports={UI_CODES:Object.freeze({has:code=>UI_CODES.has(code)}),UNKNOWN,failureCode,pairOutcomes,validatePairOutcomes,pairFailure,uploadOutcome,validateUploadOutcome};
