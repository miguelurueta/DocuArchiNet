'use strict';
const {validateScope}=require('./doc117-alm03-setup-scope.cjs');
const {scopeDigest,resourceHashes}=require('../../scripts/support/doc117-alm03-scoped-candidate.cjs');
// Shared data invariants, not ALM-02 grants or its fault protocol.
const {assertInitial,assertCheckpoint,assertResponse}=require('./doc117-alm02-internal.cjs');
const {KEYS,assessMethodObservation}=require('./doc117-alm03-method-observation.cjs');
const {pairOutcomes,pairFailure,validateUploadOutcome}=require('../../scripts/support/doc117-alm03-failure-evidence.cjs');
const check=(v,c)=>{if(!v)throw Error('DOC117_ALM03_'+c);};
const id=x=>typeof x==='string'&&/^[1-9][0-9]{0,9}$/.test(x)&&BigInt(x)<=4294967295n;
function validatePrepared(s,commands){
 validateScope(s);check(s.lotId==='22'&&s.resources.map(r=>[r.rootId,r.fileId,r.expedienteId,r.relacionNivelId].join('/')).join(',')==='35/36/666/1091,36/37/667/1092','RESOURCES_CHANGED');
 check(Array.isArray(commands)&&commands.length===2,'COMMANDS_INVALID');
 commands.forEach((c,i)=>check(c&&Object.keys(c).sort().join(',')==='claveOperacion,fase,loteId,recursoId,revisionEsperada'&&c.loteId===s.lotId&&c.recursoId===s.resources[i].fileId&&c.fase==='ALMACENAR'&&Number.isInteger(c.revisionEsperada)&&c.revisionEsperada>0&&c.revisionEsperada<4294967294&&/^[a-f0-9]{32}$/.test(c.claveOperacion),'COMMANDS_CHANGED'));
 return s;
}
function validateAuthorization(a,s){check(a?.scope==='ALM-03-COEXISTENCE'&&['authorized','environmentAuthorized','account','select','storage','runtimeChanges','exclusive','discardable'].every(k=>a[k]===true)&&a.resourceScopeSha256===scopeDigest(s),'SPECIFIC_AUTHORIZATION_REQUIRED');}
function reference(scope,key,event,pin,hashResources=resourceHashes){
 const index=KEYS.indexOf(key),hashes=hashResources(scope);
 check(index>=0&&event?.version===1&&event.runId===scope.runId&&event.resourceKey===key&&event.resourceSha256===hashes[index]&&event.phase==='EXIT'&&event.boundary==='METHOD_BODY_ENTRY_EXIT_V1'&&event.status==='SUCCESS'&&id(event.documentoId)&&id(event.inventarioId),'METHOD_EXIT_NOT_CONFIRMED');
 const ref={resourceKey:key,documentoId:event.documentoId,inventarioId:event.inventarioId};
 if(index===2)return{...ref,expedienteId:'467',relacionNivelId:'1072'};
 const r=scope.resources[index];check(pin&&Object.keys(pin).sort().join(',')==='intentoId,resourceKey,runId,solicitudHash,version'&&pin.version===1&&pin.runId===scope.runId&&pin.resourceKey===key&&id(pin.intentoId)&&!['41','52','53'].includes(pin.intentoId)&&pin.solicitudHash===r.requestSha256,'ATTEMPT_NOT_CORRELATED');
 return{...ref,loteId:scope.lotId,raizId:r.rootId,archivoId:r.fileId,intentoId:pin.intentoId,expedienteId:r.expedienteId,relacionNivelId:r.relacionNivelId};
}
function sameIds(a,b){return a?.documentoId===b?.documentoId&&a?.inventarioId===b?.inventarioId&&a?.expedienteId===b?.expedienteId;}
function validatePair(scope,events,hashResources=resourceHashes){
 check(Array.isArray(events)&&events.length===4,'PAIR_OBSERVATION_INVALID');
 const fields='appDomainId,boundary,clockDomainId,documentoId,frequencyHz,highResolution,inventarioId,nativeThreadId,phase,processId,resourceKey,resourceSha256,runId,sessionSlot,status,threadId,ticks,version';
 const first=events[0],hashes=hashResources(scope);
 for(const e of events){const index=KEYS.indexOf(e.resourceKey);check(Object.keys(e).sort().join(',')===fields&&[0,2].includes(index)&&e.version===1&&e.runId===scope.runId&&e.resourceSha256===hashes[index]&&e.boundary==='METHOD_BODY_ENTRY_EXIT_V1'&&e.highResolution===true&&['ENTER','EXIT'].includes(e.phase)&&/^[a-f0-9]{32}$/.test(e.clockDomainId)&&id(e.processId)&&id(e.appDomainId)&&id(e.threadId)&&id(e.nativeThreadId)&&/^[1-9][0-9]{0,18}$/.test(e.frequencyHz)&&/^[0-9]{1,19}$/.test(e.ticks)&&BigInt(e.ticks)<=9223372036854775807n&&['clockDomainId','processId','appDomainId','frequencyHz'].every(k=>e[k]===first[k])&&e.sessionSlot===(index===0?'IMPORT_SESSION':'INDIVIDUAL_SESSION'),'PAIR_OBSERVATION_INVALID');check(e.phase==='ENTER'?e.status==='PENDING'&&e.documentoId===null&&e.inventarioId===null:e.status==='SUCCESS'&&id(e.documentoId)&&id(e.inventarioId),'PAIR_OBSERVATION_INVALID');}
 for(const key of ['IMPORT_A','INDIVIDUAL']){const enter=events.filter(e=>e.resourceKey===key&&e.phase==='ENTER'),exit=events.filter(e=>e.resourceKey===key&&e.phase==='EXIT');check(enter.length===1&&exit.length===1&&enter[0].threadId===exit[0].threadId&&enter[0].nativeThreadId===exit[0].nativeThreadId&&BigInt(exit[0].ticks)>BigInt(enter[0].ticks)&&BigInt(exit[0].ticks)-BigInt(enter[0].ticks)<=600n*BigInt(first.frequencyHz),'PAIR_OBSERVATION_INVALID');}
}
// Wait for both dispatched operations, including a failed/uncertain partner.
// The rendezvous is in the client before dispatch, never inside either writer.
async function dispatchPair(sendA,individual,onStored,onSettled){
 const publish=(records,code)=>{const evidence=pairOutcomes(records);try{const saved=onSettled?.(evidence);if(saved&&typeof saved.then==='function'){Promise.resolve(saved).catch(()=>{});check(false,'SYNCHRONOUS_CHECKPOINT_REQUIRED');}}catch{throw pairFailure('DOC117_ALM03_SETTLEMENT_NOT_DURABLE',evidence);}if(code)throw pairFailure(code,evidence);};
 let readyResolve,release;const ready=new Promise(r=>{readyResolve=r;}),gate=new Promise(r=>{release=r;});let reached=false;
 const one=Promise.resolve().then(()=>individual({beforeStorage:async()=>{check(!reached,'INDIVIDUAL_DISPATCH_DUPLICATED');reached=true;readyResolve();await gate;},onStored}));
 const settled=one.then(v=>({status:'fulfilled',value:v}),reason=>({status:'rejected',reason}));
 const first=await Promise.race([ready.then(()=>true),settled.then(()=>false)]);
 if(!first){const done=await settled;publish([{status:'not_dispatched'},done],'DOC117_ALM03_INDIVIDUAL_NOT_READY');}
 const a=Promise.resolve().then(sendA);release();
 const pair=await Promise.all([a.then(value=>({status:'fulfilled',value}),reason=>({status:'rejected',reason})),settled]);
 publish(pair,pair.every(r=>r.status==='fulfilled')?null:'DOC117_ALM03_PAIR_RESULT_UNCERTAIN');return pair.map(r=>r.value);
}
async function runCoexistence({scope,commands,authorization,send,individual,controls,readObservation,onProgress,before,policy={}}){
 (policy.validatePrepared||validatePrepared)(scope,commands);(policy.validateAuthorization||validateAuthorization)(authorization,scope);check(typeof onProgress==='function','DURABLE_CHECKPOINT_REQUIRED');
 const hashResources=policy.resourceHashes||resourceHashes,refFor=(key,event,pin)=>policy.reference?policy.reference(scope,key,event,pin):reference(scope,key,event,pin,hashResources);
 const step=v=>{const result=onProgress(v);if(result&&typeof result.then==='function'){Promise.resolve(result).catch(()=>{});check(false,'SYNCHRONOUS_CHECKPOINT_REQUIRED');}};
 check(before&&/^[0-9]+$/.test(before.inventoryBefore),'PREFLIGHT_REQUIRED');const count=BigInt(before.inventoryBefore);
 let requests=0,storedIndividual;
 const request=async index=>{check(requests<3,'REQUEST_BUDGET_EXHAUSTED');requests++;step({stage:'IMPORT_STORAGE_REQUESTED',resourceKey:KEYS[index],archivoId:scope.resources[index].fileId,requestNumber:requests});const response=await send(structuredClone(commands[index])),data=response?.body?.datos;const known=Object.fromEntries(['intentoId','documentoId','inventarioId','expedienteId','relacionNivelId'].filter(k=>id(data?.[k])).map(k=>[k,data[k]]));if(Object.keys(known).length)step({stage:'IMPORT_RESPONSE_IDENTITIES_OBSERVED',resourceKey:KEYS[index],archivoId:scope.resources[index].fileId,...known});return response;};
 const [a,ui]=await dispatchPair(()=>request(0),hooks=>individual({...hooks,beforeStorage:async()=>{await hooks.beforeStorage();requests++;check(requests<=3,'REQUEST_BUDGET_EXHAUSTED');step({stage:'INDIVIDUAL_STORAGE_REQUESTED',resourceKey:'INDIVIDUAL',requestNumber:requests});},onStorageOutcome:value=>step({stage:'INDIVIDUAL_STORAGE_DIAGNOSTIC',diagnostic:validateUploadOutcome(value)})}),ref=>{check(!storedIndividual&&id(ref?.documentoId)&&id(ref?.inventarioId)&&ref.expedienteId==='467','INDIVIDUAL_IDENTITIES_INVALID');storedIndividual={resourceKey:'INDIVIDUAL',...ref};step({stage:'INDIVIDUAL_IDENTITIES_KNOWN',...storedIndividual});},outcomes=>step({stage:'PAIR_OPERATIONS_SETTLED',outcomes}));
 check(ui?.codes?.storage==='CONFIRMED'&&ui?.codes?.projection==='CONFIRMED'&&ui.count===1&&storedIndividual,'INDIVIDUAL_UI_NOT_CONFIRMED');
 const pair=await readObservation(false);validatePair(scope,pair.events,hashResources);const refA=refFor('IMPORT_A',pair.events.find(e=>e.resourceKey==='IMPORT_A'&&e.phase==='EXIT'),pair.pins.IMPORT_A),refI=refFor('INDIVIDUAL',pair.events.find(e=>e.resourceKey==='INDIVIDUAL'&&e.phase==='EXIT'));
 step({stage:'IMPORT_IDENTITIES_KNOWN',...refA});check(sameIds(refI,storedIndividual),'INDIVIDUAL_RESPONSE_NOT_CORRELATED');assertResponse(a,scope,0,refA,true);
 check(refA.documentoId!==refI.documentoId&&refA.inventarioId!==refI.inventarioId,'IDENTITY_COLLISION');
 const middle=await controls.snapshot(scope.lotId);assertCheckpoint(middle,scope,0,refA,true);await controls.physical(middle,0,refA);await controls.individualPhysical(refI,before.individual);await controls.original(0);await controls.original(1);
 check(BigInt(await controls.inventoryCount())===count+2n,'UNEXPECTED_DOCUMENT_COUNT');
 const pending=middle.files.filter(f=>String(f.archivo_id)===scope.resources[1].fileId);check(pending.length===1&&pending[0].estado_efecto==='NO_INICIADO'&&!pending[0].intento_activo_id&&!pending[0].resultado_doc&&Number(pending[0].revision)===commands[1].revisionEsperada&&middle.attempts.filter(a=>a.fase==='ALMACENAR').length===1,'SECOND_FILE_CHANGED');
 step({stage:'PAIR_PHYSICAL_VERIFIED'});const b=await request(1),all=await readObservation(true),refB=refFor('IMPORT_B',all.events.find(e=>e.resourceKey==='IMPORT_B'&&e.phase==='EXIT'),all.pins.IMPORT_B);step({stage:'IMPORT_IDENTITIES_KNOWN',...refB});assertResponse(b,scope,1,refB,true);
 const observation=assessMethodObservation({runId:scope.runId,resourceHashes:hashResources(scope)},all.events,all.summary);
 for(const ref of [refA,refB,refI])check(sameIds(ref,refFor(ref.resourceKey,all.events.find(e=>e.resourceKey===ref.resourceKey&&e.phase==='EXIT'),all.pins[ref.resourceKey])),'OBSERVATION_CHANGED');
 check(requests===3,'REQUEST_COUNT_INVALID');return{status:observation.writerBodyOverlapVerified?'PASS':'NO_ACREDITADO',scenario:policy.scenario||'ALM-03-COEXISTENCE',laboratoryVariant:true,storageRequests:requests,documents:3,inventoryBefore:count.toString(),inventoryAfter:(count+3n).toString(),methodObservation:observation,businessEvidenceVerified:false,alm03ConcurrentPass:false,references:[refA,refB,refI]};
}
module.exports={check,id,validatePrepared,validateAuthorization,reference,dispatchPair,runCoexistence,assertInitial,assertCheckpoint};
