'use strict';
// Offline evidence assessment only. This module cannot authenticate, upload,
// install instrumentation, or promote caller intervals to a real ALM-03 PASS.
const crypto=require('node:crypto');
const sha=value=>crypto.createHash('sha256').update(value).digest('hex');
const check=(value,code)=>{if(!value)throw Error('DOC117_ALM03_'+code);};
const closed=(value,keys)=>check(value&&typeof value==='object'&&!Array.isArray(value)&&Object.keys(value).sort().join(',')===keys.slice().sort().join(','),'FIELDS_INVALID');
const id=value=>typeof value==='string'&&/^[1-9][0-9]{0,9}$/.test(value)&&BigInt(value)<=4294967295n;
const hash=value=>typeof value==='string'&&/^[a-f0-9]{64}$/.test(value);
const counter=value=>typeof value==='string'&&/^(0|[1-9][0-9]{0,18})$/.test(value)&&BigInt(value)<=9223372036854775807n;
const KIND='DIRECT_CALL_BOUNDARIES_V1';
function validateObservationScope(s){
 closed(s,['issueKey','scenario','runId','observationKind','maximumSeconds','resources']);
 check(s.issueKey==='DOC-117'&&s.scenario==='ALM-03'&&/^[a-f0-9]{32}$/.test(s.runId)&&s.observationKind===KIND&&s.maximumSeconds===600,'SCOPE_INVALID');
 check(Array.isArray(s.resources)&&s.resources.length===2,'RESOURCES_INVALID');
 for(let i=0;i<2;i++){
  const r=s.resources[i],common=['flow','expedienteId','tipologiaId','fileName','bytes','sha256'];
  closed(r,i===0?[...common,'loteId','raizId','archivoId']:common);
  check(r.flow===(i===0?'IMPORTACION':'INDIVIDUAL')&&id(r.expedienteId)&&r.tipologiaId===(i===0?'79':'50')&&r.bytes==='677'&&hash(r.sha256)&&typeof r.fileName==='string'&&/^DOC117-ALM03-[A-Za-z0-9-]+\.pdf$/.test(r.fileName),'RESOURCE_INVALID');
  if(i===0)check(['loteId','raizId','archivoId'].every(k=>id(r[k]))&&BigInt(r.loteId)>21n&&BigInt(r.raizId)>34n&&BigInt(r.archivoId)>35n&&BigInt(r.expedienteId)>665n,'PREVIOUS_RESOURCE_REFUSED');
  else check(r.expedienteId==='467','INDIVIDUAL_DESTINATION_CHANGED');
 }
 check(s.resources[0].fileName!==s.resources[1].fileName,'RESOURCE_COLLISION');return s;
}
function resourceDigest(r){return sha(JSON.stringify(r));}
function assessDirectCallObservation(scope,events){
 const s=validateObservationScope(scope);
 check(Array.isArray(events)&&events.length===4,'COMPLETE_PAIRS_REQUIRED');
 const keys=['version','runId','flow','phase','boundary','clockDomainId','frequencyHz','highResolution','processId','appDomainId','threadId','ticks','sessionSlot','sessionIsolation','resourceSha256','status','documentoId','inventarioId'];
 for(const event of events){
  closed(event,keys);const r=s.resources.find(x=>x.flow===event.flow);
  check(r&&event.version===1&&event.runId===s.runId&&['ENTER','EXIT'].includes(event.phase)&&event.boundary===KIND&&/^[a-f0-9]{32}$/.test(event.clockDomainId)&&counter(event.frequencyHz)&&BigInt(event.frequencyHz)>0n&&event.highResolution===true&&['processId','appDomainId','threadId'].every(k=>id(event[k]))&&counter(event.ticks)&&event.sessionSlot===(r.flow==='IMPORTACION'?'IMPORT_SESSION':'INDIVIDUAL_SESSION')&&event.sessionIsolation==='VERIFIED_DISTINCT_IN_MEMORY'&&event.resourceSha256===resourceDigest(r),'EVENT_INVALID');
  check(event.phase==='ENTER'?event.status==='PENDING'&&event.documentoId===null&&event.inventarioId===null:event.status==='SUCCESS'&&id(event.documentoId)&&id(event.inventarioId),'SUCCESSFUL_RETURN_REQUIRED');
 }
 const first=events[0];
 for(const field of ['clockDomainId','frequencyHz','processId','appDomainId'])check(events.every(e=>e[field]===first[field]),'COUNTER_DOMAIN_CHANGED');
 const spans=s.resources.map(r=>{
  const pair=events.filter(e=>e.flow===r.flow),enter=pair.filter(e=>e.phase==='ENTER'),exit=pair.filter(e=>e.phase==='EXIT');
  check(pair.length===2&&enter.length===1&&exit.length===1,'COMPLETE_PAIRS_REQUIRED');
  const start=enter[0],end=exit[0],duration=BigInt(end.ticks)-BigInt(start.ticks);
  check(start.threadId===end.threadId&&duration>0n&&duration<=BigInt(first.frequencyHz)*BigInt(s.maximumSeconds),'INTERVAL_INVALID');
  return{flow:r.flow,threadId:start.threadId,start:BigInt(start.ticks),end:BigInt(end.ticks),documentoId:end.documentoId,inventarioId:end.inventarioId};
 });
 check(spans[0].documentoId!==spans[1].documentoId&&spans[0].inventarioId!==spans[1].inventarioId,'IDENTITY_COLLISION');
 const start=spans[0].start>spans[1].start?spans[0].start:spans[1].start,end=spans[0].end<spans[1].end?spans[0].end:spans[1].end;
 const overlap=end>start?end-start:0n;
 check(overlap===0n||spans[0].threadId!==spans[1].threadId,'SAME_THREAD_OVERLAP_INVALID');
 return{
  classification:overlap>0n?'CALLS_OVERLAPPED':'SERIALIZED_CALLS',observationKind:KIND,
  overlapTicks:overlap.toString(),frequencyHz:first.frequencyHz,
  overlapNanosecondsFloor:(overlap*1000000000n/BigInt(first.frequencyHz)).toString(),
  writerBodyOverlapVerified:false,businessEvidenceVerified:false,alm03ConcurrentPass:false,
  limitation:'Caller brackets enclose invocation; scheduling between marker and call can mimic method-body overlap.',
  requiresEditorialDecision:true
 };
}
module.exports={KIND,validateObservationScope,resourceDigest,assessDirectCallObservation};
