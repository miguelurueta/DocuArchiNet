'use strict';
const {test}=require('node:test'),assert=require('node:assert/strict');
const {KEYS,KIND,assessMethodObservation}=require('../tools/e2e/tests/support/doc117-alm03-method-observation.cjs');
function fixture(){
 const scope={runId:'a'.repeat(32),resourceHashes:['b','c','d'].map(c=>c.repeat(64))};
 const summary={version:1,runId:scope.runId,clockDomainId:'e'.repeat(32),processId:'500',appDomainId:'2',frequencyHz:'10000000',expectedResources:KEYS,resourceHashes:scope.resourceHashes,sessionsDistinct:true,entries:3,exits:3,observationFaults:0,complete:true};
 const event=(i,phase,ticks)=>({version:1,runId:scope.runId,resourceKey:KEYS[i],resourceSha256:scope.resourceHashes[i],boundary:KIND,phase,clockDomainId:summary.clockDomainId,frequencyHz:summary.frequencyHz,highResolution:true,processId:'500',appDomainId:'2',threadId:i===2?'20':'10',nativeThreadId:i===2?'2000':'1000',ticks,sessionSlot:i===2?'INDIVIDUAL_SESSION':'IMPORT_SESSION',status:phase==='ENTER'?'PENDING':'SUCCESS',documentoId:phase==='EXIT'?String(20000+i):null,inventarioId:phase==='EXIT'?String(30000+i):null});
 return{scope,summary,events:[event(0,'ENTER','100'),event(0,'EXIT','400'),event(1,'ENTER','500'),event(1,'EXIT','600'),event(2,'ENTER','200'),event(2,'EXIT','300')]};
}
test('ALM03 internal method evidence measures overlap while retaining separate business and E2E acceptance',()=>{
 const f=fixture(),r=assessMethodObservation(f.scope,f.events,f.summary);assert.equal(r.classification,'METHOD_BODIES_OVERLAPPED');assert.equal(r.overlapTicks,'100');assert.equal(r.writerBodyOverlapVerified,true);assert.equal(r.businessEvidenceVerified,false);assert.equal(r.alm03ConcurrentPass,false);
});
test('ALM03 internal method evidence reports natural serialization without a concurrency PASS',()=>{
 const f=fixture();f.events[1].ticks='200';f.events[4].ticks='300';f.events[5].ticks='400';const r=assessMethodObservation(f.scope,f.events,f.summary);assert.equal(r.classification,'SERIALIZED_METHODS');assert.equal(r.writerBodyOverlapVerified,false);
});
test('ALM03 internal method counter arithmetic preserves precision above 2^53',()=>{
 const f=fixture();for(const e of f.events)e.ticks=(BigInt(e.ticks)+9007199254740993n).toString();assert.equal(assessMethodObservation(f.scope,f.events,f.summary).overlapTicks,'100');
});
for(const [name,change]of Object.entries({
 'external caller markers':f=>{f.events[0].boundary='DIRECT_CALL_BOUNDARIES_V1';},
 'foreign resource':f=>{f.events[0].resourceSha256='f'.repeat(64);},
 'foreign execution':f=>{f.events[0].runId='f'.repeat(32);},
 'different clock':f=>{f.events[0].clockDomainId='f'.repeat(32);},
 'different process':f=>{f.events[0].processId='501';},
 'different domain':f=>{f.events[0].appDomainId='3';},
 'different frequency':f=>{f.events[0].frequencyHz='1000';},
 'unverified sessions':f=>{f.summary.sessionsDistinct=false;},
 'incomplete seal':f=>{f.summary.complete=false;},
 'observation I/O failure':f=>{f.summary.observationFaults=1;},
 'missing exit':f=>{f.events.pop();},
 'duplicate entry':f=>{f.events[1]={...f.events[0]};},
 'changed native thread':f=>{f.events[1].nativeThreadId='3000';},
 'failed writer result':f=>{f.events[1].status='FAILURE';},
 'missing IDs':f=>{f.events[1].documentoId=null;},
 'repeated document':f=>{f.events[3].documentoId=f.events[1].documentoId;},
 'second import too early':f=>{f.events[2].ticks='250';},
 'rounded numeric counter':f=>{f.events[0].ticks=100;},
 'reversed counter':f=>{f.events[1].ticks='99';},
 'raw cookie field':f=>{f.events[0].cookie='forbidden-test-value';}
}))test('ALM03 internal method evidence rejects '+name,()=>{const f=fixture();change(f);assert.throws(()=>assessMethodObservation(f.scope,f.events,f.summary),/DOC117_ALM03_METHOD_/);});
