'use strict';
const {test}=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs'),path=require('node:path');
const {ROOT,PROPOSAL,runDiagnostic,validateProposal,authorize}=require('../tools/e2e/scripts/run-doc117-alm03-destination-readonly.cjs');
function fixture(){
 const p=JSON.parse(fs.readFileSync(path.join(ROOT,PROPOSAL),'utf8')),rows=[[{ID_EXPEDIENTE:'467',ID_EMPRESA_EXPEDIENTE:'2',GABINETE_PRODUCION:'PRIVATE_CABINET'}],[{relacion_id:'999',nivel_id:'99',parent_id:'7',estado_nivel:'1'}],[{type_id:'50'}],[{intento_id:'41',lote_id:'18',fase:'CREAR_EXPEDIENTE',estado:'EN_CURSO',fin_utc:null,evidencia_doc:'PRIVATE_EVIDENCE'},{intento_id:'52',lote_id:'21',fase:'ALMACENAR',estado:'CONFIRMADO',fin_utc:'2026-11-06',evidencia_doc:JSON.stringify({Estado:'CONFIRMADO',DocumentoId:'16222',InventarioId:'23320'})},{intento_id:'53',lote_id:'21',fase:'ALMACENAR',estado:'EN_CURSO',fin_utc:null,evidencia_doc:JSON.stringify({Estado:'EN_CURSO'})}]];
 let count=0;const progress=[];return{p,rows,progress,get count(){return count;},run:()=>runDiagnostic({proposal:p,query:async(origin,sql,args)=>{const index=count++;assert.equal(origin,'RA');assert.equal(sql,p.controls[index].sql);assert.deepEqual(args,p.controls[index].parameters);return rows[index];},onProgress:v=>progress.push(v)})};
}
test('ALM03 destination diagnostic uses only four further SELECTs and never grants acceptance',async()=>{
 const f=fixture(),r=await f.run();assert.equal(r.status,'DIAGNOSTIC_COMPLETE');assert.equal(r.cumulativeSelectRequests,12);assert.equal(f.count,4);assert.equal(r.preflightAccepted,false);assert.equal(r.realAlm03Pass,false);assert.equal(r.proof.individualRelations[0].nivelId,'99');assert.equal(r.proof.protectedAttemptsVerified,true);const text=JSON.stringify([r,f.progress]);assert.ok(!text.includes('PRIVATE_CABINET')&&!text.includes('PRIVATE_EVIDENCE'));
});
test('ALM03 missing individual expedition remains a diagnostic finding without recreating it',async()=>{
 const f=fixture();f.rows[0]=[];f.rows[1]=[];f.rows[2]=[];const r=await f.run();assert.equal(r.proof.individualExpedientFound,false);assert.equal(r.proof.individualTypologyAvailable,false);assert.equal(r.realAlm03Pass,false);assert.equal(f.count,4);
});
test('ALM03 destination diagnostic records failed cardinality and stops before further controls',async()=>{
 const f=fixture();f.rows[0].push({...f.rows[0][0]});await assert.rejects(f.run(),/EXPEDITION_AMBIGUOUS/);assert.equal(f.count,1);assert.equal(f.progress.at(-1).phase,'OBSERVED');assert.equal(f.progress.at(-1).counts.individual_expedition,2);
});
test('ALM03 destination diagnostic stops without retry on transport uncertainty and altered protected attempts',async()=>{
 const f=fixture();let count=0;await assert.rejects(runDiagnostic({proposal:f.p,query:async()=>{count++;throw Error('DOC111_SELECT_FAILED');},onProgress:()=>{}}),/DOC111_SELECT_FAILED/);assert.equal(count,1);
 const changed=fixture();changed.rows[3][2].estado='CONFIRMADO';await assert.rejects(changed.run(),/PROTECTED_ATTEMPTS_CHANGED/);assert.equal(changed.count,4);
});
test('ALM03 destination diagnostic refuses a new budget, writes or effects authorization',()=>{
 const f=fixture(),p=f.p,h='a'.repeat(64),a={authorized:true,scope:p.scope,environment:p.environment,proposalSha256:h,codeSha256:h,select:true,effects:false,applicationLogins:false,runtimeChanges:false,priorArtifactSha256:p.priorArtifactSha256,authorizationId:'b'.repeat(32)};
 assert.doesNotThrow(()=>authorize(a,p,h,h));for(const change of [{scope:'ALM-03-PREFLIGHT-SELECT'},{effects:true},{priorArtifactSha256:'c'.repeat(64)}])assert.throws(()=>authorize({...a,...change},p,h,h),/SPECIFIC_AUTHORIZATION_REQUIRED/);
 assert.throws(()=>validateProposal({...p,maximumSelectRequests:5}),/SCOPE_CHANGED/);
 const mutated=JSON.parse(JSON.stringify(p));mutated.controls[0].sql='UPDATE x';assert.throws(()=>validateProposal(mutated),/SELECT_REQUIRED/);
});
