'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),crypto=require('node:crypto');
const {suppressFirstResponse}=require('../tools/e2e/tests/support/doc118-lost-response.cjs');
function fixture(){
 const command={loteId:'100',recursoId:'300',fase:'ALMACENAR',revisionEsperada:1,claveOperacion:'synthetic-key'},profile={baseUrl:'https://example.invalid/app/'},resourceSha256=crypto.createHash('sha256').update(JSON.stringify(command)).digest('hex');let calls=0,disposed=0,confirmed=true;
 const response={status:()=>200,dispose:async()=>{disposed++;},json:async()=>assert.fail('first response body must not reach driver')};
 const fault=suppressFirstResponse({auth:{context:{request:{post:async()=>{calls++;return response;}}}},profile,command,resourceSha256,checkpoint:async()=>{if(!confirmed)throw Error('synthetic-private-row');return{documentoId:'600',inventarioId:'700',fingerprint:'a'.repeat(64),private:'synthetic-private-row'};}});
 const invoke=()=>fault.session.context.request.post(profile.baseUrl+'webservice/WebServiceImportacionCarpetas.asmx/AvanzarLote',{data:{solicitud:command}});
 return{fault,invoke,response,calls:()=>calls,disposed:()=>disposed,unconfirm:()=>{confirmed=false;}};
}
test('DOC118 suppresses delivery only after checkpoint, without reading response IDs or resending first',async()=>{const f=fixture();await assert.rejects(f.invoke(),/DELIVERY_SUPPRESSED/);const r=await f.fault.outcome();assert.equal(r.status,'DROPPED_AFTER_CHECKPOINT');assert.equal(r.boundary,'CLIENT_API_RESPONSE_DELIVERY');assert.equal(JSON.stringify(r).includes('synthetic-private'),false);assert.equal(f.calls(),1);assert.equal(f.disposed(),1);});
test('DOC118 response suppression leaves an explicit caller replay untouched',async()=>{const f=fixture();await assert.rejects(f.invoke());assert.equal(await f.invoke(),f.response);assert.equal(f.calls(),2);assert.equal(f.disposed(),1);});
test('DOC118 cannot claim lost-after-checkpoint when durable proof is missing',async()=>{const f=fixture();f.unconfirm();await assert.rejects(f.invoke(),/DELIVERY_SUPPRESSED/);assert.deepEqual(await f.fault.outcome(),{status:'FIRST_OUTCOME_UNCONFIRMED',boundary:'CLIENT_API_RESPONSE_DELIVERY'});assert.equal(f.calls(),1);});
