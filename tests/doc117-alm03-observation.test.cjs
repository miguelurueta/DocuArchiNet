'use strict';
const {test}=require('node:test'),assert=require('node:assert/strict');
const {KIND,validateObservationScope,resourceDigest,assessDirectCallObservation}=require('../tools/e2e/tests/support/doc117-alm03-observation.cjs');
function fixture(){
 const scope={issueKey:'DOC-117',scenario:'ALM-03',runId:'a'.repeat(32),observationKind:KIND,maximumSeconds:600,resources:[{flow:'IMPORTACION',loteId:'900',raizId:'901',archivoId:'902',expedienteId:'903',tipologiaId:'79',fileName:'DOC117-ALM03-IMPORT-TEST.pdf',bytes:'677',sha256:'b'.repeat(64)},{flow:'INDIVIDUAL',expedienteId:'467',tipologiaId:'50',fileName:'DOC117-ALM03-INDIVIDUAL-TEST.pdf',bytes:'677',sha256:'b'.repeat(64)}]};
 const event=(index,phase,ticks)=>({version:1,runId:scope.runId,flow:scope.resources[index].flow,phase,boundary:KIND,clockDomainId:'c'.repeat(32),frequencyHz:'10000000',highResolution:true,processId:'500',appDomainId:'2',threadId:String(index+10),ticks,sessionSlot:index===0?'IMPORT_SESSION':'INDIVIDUAL_SESSION',sessionIsolation:'VERIFIED_DISTINCT_IN_MEMORY',resourceSha256:resourceDigest(scope.resources[index]),status:phase==='ENTER'?'PENDING':'SUCCESS',documentoId:phase==='EXIT'?String(index+20000):null,inventarioId:phase==='EXIT'?String(index+30000):null});
 return{scope,events:[event(0,'ENTER','100'),event(1,'ENTER','200'),event(1,'EXIT','300'),event(0,'EXIT','400')]};
}
test('ALM03 caller overlap is measured exactly without granting method-body or real concurrency PASS',()=>{
 const f=fixture(),r=assessDirectCallObservation(f.scope,f.events);
 assert.equal(r.classification,'CALLS_OVERLAPPED');assert.equal(r.overlapTicks,'100');assert.equal(r.overlapNanosecondsFloor,'10000');
 assert.equal(r.writerBodyOverlapVerified,false);assert.equal(r.businessEvidenceVerified,false);assert.equal(r.alm03ConcurrentPass,false);
});
test('ALM03 serialized and touching intervals never count as overlap',()=>{
 for(const start of ['400','500']){const f=fixture();f.events[1].ticks=start;f.events[2].ticks='600';const r=assessDirectCallObservation(f.scope,f.events);assert.equal(r.classification,'SERIALIZED_CALLS');assert.equal(r.overlapTicks,'0');assert.equal(r.alm03ConcurrentPass,false);}
});
test('ALM03 uses integer counter arithmetic beyond JavaScript safe integers',()=>{
 const f=fixture(),offset=9007199254740993n;for(const e of f.events)e.ticks=(BigInt(e.ticks)+offset).toString();assert.equal(assessDirectCallObservation(f.scope,f.events).overlapTicks,'100');
});
for(const [name,change]of Object.entries({
 'HTTP duration proxy':f=>{f.events[0].boundary='HTTP_RESPONSE';},
 'outer preparer duration proxy':f=>{f.events[0].boundary='UPLOAD_SAVE_FILE';},
 'other process':f=>{f.events[1].processId='501';},
 'other app domain':f=>{f.events[1].appDomainId='3';},
 'other monotonic domain':f=>{f.events[1].clockDomainId='d'.repeat(32);},
 'different counter frequency':f=>{f.events[1].frequencyHz='1000';},
 'unverified independent sessions':f=>{f.events[1].sessionIsolation='NOT_VERIFIED';},
 'raw session or cookie field':f=>{f.events[1].sessionId='forbidden-test-value';},
 'incomplete trace':f=>{f.events.pop();},
 'duplicate entry':f=>{f.events[2]={...f.events[1]};},
 'failed writer':f=>{f.events[2].status='FAILURE';},
 'missing document':f=>{f.events[2].documentoId=null;},
 'foreign resource':f=>{f.events[2].resourceSha256='e'.repeat(64);},
 'same-thread impossible overlap':f=>{f.events[1].threadId='10';f.events[2].threadId='10';},
 'reversed interval':f=>{f.events[3].ticks='90';},
 'unbounded interval':f=>{f.events[3].ticks='6000000101';},
 'negative counter':f=>{f.events[0].ticks='-1';},
 'rounded numeric counter':f=>{f.events[0].ticks=100;},
 'identity collision':f=>{f.events[2].documentoId=f.events[3].documentoId;}
}))test('ALM03 rejects '+name,()=>{const f=fixture();change(f);assert.throws(()=>assessDirectCallObservation(f.scope,f.events),/DOC117_ALM03_/);});
test('ALM03 refuses reuse of ALM02 resources, ambiguous fields and another individual destination',()=>{
 for(const change of [f=>{f.scope.resources[0].loteId='21';},f=>{f.scope.resources[0].expedienteId='665';},f=>{f.scope.resources[1].expedienteId='468';},f=>{f.scope.resources[0].requestBody={};}]){const f=fixture();change(f);assert.throws(()=>validateObservationScope(f.scope),/DOC117_ALM03_/);}
});
