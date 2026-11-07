'use strict';
const {test}=require('node:test'),assert=require('node:assert/strict');
const {runSetup}=require('../tools/e2e/tests/support/doc117-alm03-setup.cjs');
const {fixture}=require('./support/doc117-alm03-setup-fixture.cjs');
test('ALM03 setup creates only two expedients and prepares closed requests without storage',async()=>{
 const f=fixture(),r=await runSetup(f.options);assert.equal(r.status,'PASS');assert.equal(r.documents,0);assert.equal(r.storageRequests,0);assert.equal(r.createdExpedients,2);assert.equal(r.runtimeChanges,0);assert.equal(f.calls.filter(c=>c==='PrepararPlan').length,1);assert.equal(f.calls.filter(c=>c==='ConfirmarPlan').length,1);assert.equal(f.calls.filter(c=>c==='RECEIVE').length,2);assert.equal(f.calls.filter(c=>c==='AvanzarLote:CREAR_EXPEDIENTE').length,2);assert.equal(f.calls.filter(c=>c==='AvanzarLote:MATERIALIZAR_VALORES').length,2);assert.equal(f.originals.length,4);assert.equal(f.ready.commands.length,2);assert.ok(!f.ready.resourceScope.resources.some(r=>'attemptId'in r));assert.ok(!JSON.stringify(f.steps).includes('claveOperacion'));
});
for(const failure of ['existing','journal','lost-plan','uncertain-creation','values','documents','preserved','calculator','individual-classification','individual-relation','individual-authority','individual-cabinet','individual-document'])test('ALM03 setup stops without retry or storage on '+failure,async()=>{
 const f=fixture(failure);await assert.rejects(runSetup(f.options));assert.equal(f.ready,undefined);assert.ok(f.calls.filter(c=>c==='PrepararPlan').length<=1);assert.ok(f.calls.filter(c=>c==='ConfirmarPlan').length<=1);assert.ok(!f.calls.some(c=>c==='AvanzarLote:ALMACENAR'));if(failure==='uncertain-creation')assert.equal(f.calls.filter(c=>c==='AvanzarLote:CREAR_EXPEDIENTE').length,1);if(['journal','existing'].includes(failure))assert.ok(!f.calls.includes('PrepararPlan'));
});
test('ALM03 setup refuses editorial or SELECT-only permission before transport',async()=>{
 const f=fixture();f.options.authorization={scope:'ALM-02-PREFLIGHT-SELECT',authorized:true,select:true};await assert.rejects(runSetup(f.options),/SPECIFIC_AUTHORIZATION_REQUIRED/);assert.equal(f.calls.length,0);
});
test('ALM03 setup reuses published-snapshot comparison for persisted rule order and IDs',async()=>{
 const f=fixture('rule-order');const result=await runSetup(f.options);assert.equal(result.status,'PASS');assert.equal(f.calls.filter(c=>c==='PrepararPlan').length,1);
});
