'use strict';
// Internal method intervals only. Physical business checks and authorization
// remain independent; this assessor never grants an E2E ALM-03 PASS.
const KEYS=['IMPORT_A','IMPORT_B','INDIVIDUAL'],KIND='METHOD_BODY_ENTRY_EXIT_V1';
const check=(v,c)=>{if(!v)throw Error('DOC117_ALM03_METHOD_'+c);};
const closed=(v,keys)=>check(v&&typeof v==='object'&&!Array.isArray(v)&&Object.keys(v).sort().join(',')===keys.slice().sort().join(','),'FIELDS_INVALID');
const digest=v=>typeof v==='string'&&/^[a-f0-9]{64}$/.test(v);
const counter=v=>typeof v==='string'&&/^(0|[1-9][0-9]{0,18})$/.test(v)&&BigInt(v)<=9223372036854775807n;
const id=v=>typeof v==='string'&&/^[1-9][0-9]{0,9}$/.test(v)&&BigInt(v)<=4294967295n;
function assessMethodObservation(scope,events,summary){
 closed(scope,['runId','resourceHashes']);
 check(/^[a-f0-9]{32}$/.test(scope.runId)&&Array.isArray(scope.resourceHashes)&&scope.resourceHashes.length===3&&scope.resourceHashes.every(digest)&&new Set(scope.resourceHashes).size===3,'SCOPE_INVALID');
 closed(summary,['version','runId','clockDomainId','processId','appDomainId','frequencyHz','expectedResources','resourceHashes','sessionsDistinct','entries','exits','observationFaults','complete']);
 check(summary.version===1&&summary.runId===scope.runId&&/^[a-f0-9]{32}$/.test(summary.clockDomainId)&&id(summary.processId)&&id(summary.appDomainId)&&counter(summary.frequencyHz)&&BigInt(summary.frequencyHz)>0n&&JSON.stringify(summary.expectedResources)===JSON.stringify(KEYS)&&JSON.stringify(summary.resourceHashes)===JSON.stringify(scope.resourceHashes)&&summary.sessionsDistinct===true&&summary.entries===3&&summary.exits===3&&summary.observationFaults===0&&summary.complete===true,'SUMMARY_NOT_COMPLETE');
 check(Array.isArray(events)&&events.length===6,'COMPLETE_PAIRS_REQUIRED');
 const fields=['version','runId','resourceKey','resourceSha256','boundary','phase','clockDomainId','frequencyHz','highResolution','processId','appDomainId','threadId','nativeThreadId','ticks','sessionSlot','status','documentoId','inventarioId'];
 for(const e of events){
  closed(e,fields);const i=KEYS.indexOf(e.resourceKey);
  check(i>=0&&e.version===1&&e.runId===scope.runId&&e.boundary===KIND&&e.resourceSha256===scope.resourceHashes[i]&&['ENTER','EXIT'].includes(e.phase)&&['clockDomainId','frequencyHz','processId','appDomainId'].every(k=>e[k]===summary[k])&&e.highResolution===true&&id(e.threadId)&&id(e.nativeThreadId)&&counter(e.ticks)&&e.sessionSlot===(i===2?'INDIVIDUAL_SESSION':'IMPORT_SESSION'),'EVENT_NOT_CORRELATED');
  check(e.phase==='ENTER'?e.status==='PENDING'&&e.documentoId===null&&e.inventarioId===null:e.status==='SUCCESS'&&id(e.documentoId)&&id(e.inventarioId),'SUCCESS_NOT_CONFIRMED');
 }
 const spans=KEYS.map(key=>{
  const enter=events.filter(e=>e.resourceKey===key&&e.phase==='ENTER'),exit=events.filter(e=>e.resourceKey===key&&e.phase==='EXIT');
  check(enter.length===1&&exit.length===1,'COMPLETE_PAIRS_REQUIRED');
  const a=enter[0],b=exit[0],start=BigInt(a.ticks),end=BigInt(b.ticks);
  check(a.threadId===b.threadId&&a.nativeThreadId===b.nativeThreadId&&end>start&&end-start<=600n*BigInt(summary.frequencyHz),'INTERVAL_INVALID');
  return{key,start,end,thread:a.threadId,native:a.nativeThreadId,documentoId:b.documentoId,inventarioId:b.inventarioId};
 });
 check(['documentoId','inventarioId'].every(k=>new Set(spans.map(s=>s[k])).size===3),'IDENTITY_COLLISION');
 const a=spans[0],b=spans[2],start=a.start>b.start?a.start:b.start,end=a.end<b.end?a.end:b.end,overlap=end>start?end-start:0n;
 check(overlap===0n||(a.thread!==b.thread&&a.native!==b.native),'SAME_THREAD_OVERLAP_INVALID');
 check(spans[1].start>=a.end&&spans[1].start>=b.end,'SECOND_IMPORT_STARTED_EARLY');
 return{classification:overlap>0n?'METHOD_BODIES_OVERLAPPED':'SERIALIZED_METHODS',observationKind:KIND,overlapTicks:overlap.toString(),frequencyHz:summary.frequencyHz,overlapNanosecondsFloor:(overlap*1000000000n/BigInt(summary.frequencyHz)).toString(),writerBodyOverlapVerified:overlap>0n,businessEvidenceVerified:false,alm03ConcurrentPass:false,entries:3,exits:3,limitation:'Instrumented method lifetime only; no simultaneous-commit, ordinary-binary performance or XML/SII claim.'};
}
module.exports={KEYS,KIND,assessMethodObservation};
