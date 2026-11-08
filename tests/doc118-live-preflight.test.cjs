'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs'),os=require('node:os'),path=require('node:path');
const {preflight,assertDependencies}=require('../tools/e2e/scripts/support/doc118-live-plan.cjs');
const {createBudget}=require('../tools/e2e/tests/support/doc118-session-bridge.cjs');
const profile=require('../tools/e2e/profiles/doc111-importacion-carpetas.profile.example.json');
function fixture(){
 const directory=fs.mkdtempSync(path.join(os.tmpdir(),'doc118-preflight-')),requests=[],queries=[],stages=[];
 const live={directory,profile:{...structuredClone(profile),nivelRef:'7',versionId:'38'},authorization:{maximum:{totalSessions:4}},budget:createBudget(),environment:{DOC111_E2E_PASSWORD:'synthetic-private-password',DOC111_E2E_MYSQL_PASSWORD:'synthetic-private-db'},sessions:[],sessionCount:0,mark:s=>stages.push(s)};live.budget.http('COORD02');
 let owner='141',publicationAvailable=true,opens=0;
 const browser={newContext:async()=>({
  route:async()=>{},close:async()=>{},
  request:{post:async(url,options)=>{
   requests.push(options.data.solicitud);
   return{status:()=>200,json:async()=>({ok:true,datos:{loteId:'23',nivelRef:'7'}})};
  }}
 })};
 const dependencies={chromium:{launch:async()=>browser},openSession:async b=>{opens++;return{context:await b.newContext(),page:{locator:()=>({getAttribute:async()=>'synthetic-private-csrf'})}};},selectProductionLevel:async()=>assert.fail('API preflight must not load or select the visual tree'),queryRows:async(sql,args)=>{
  queries.push(sql);
  if(sql.includes('ra_integracion_perfil_version'))return[{version_id:'38',perfil_id:'24',estado:publicationAvailable?'PUBLICADA':'BORRADOR',activo:1,configuracion_doc:JSON.stringify({GabineteId:10,ModoIntegridad:'COMPLETO_OBLIGATORIO',AmbitoIntegridad:'LOTE'})}];
  if(sql.includes('system1'))return[{id_gabinete:'10'}];
  if(sql.includes('FROM ra_integracion_lote'))return[{lote_id:args[0],contexto_doc:JSON.stringify({UsuarioRef:owner,AmbitoRef:'2',NivelRef:'7'})}];
  return[];
 }};
 return{live,dependencies,requests,queries,stages,opens:()=>opens,setOwner:v=>{owner=v;},setPublication:v=>{publicationAvailable=v;},cleanup:()=>fs.rmSync(directory,{recursive:true,force:true})};
}
test('DOC118 live preflight binds current shared APIs before credential capture',()=>assert.doesNotThrow(assertDependencies));
test('DOC118 authenticated preflight uses two read-only clients and preserves only sanitized evidence',async()=>{const f=fixture();try{
 await preflight(f.live,f.dependencies);const saved=fs.readFileSync(path.join(f.live.directory,'preflight.json'),'utf8'),e=JSON.parse(saved);
 assert.equal(f.opens(),2);assert.deepEqual(f.requests,[{loteId:'23'},{loteId:'23'}]);assert.equal(f.queries.length,17);assert.equal(e.counters.selects,17);assert.equal(e.counters.scenarios.COORD02.http,3);assert.equal(e.newDocumentEffects,0);assert.equal(e.observerQualified,false);
 assert.ok(!saved.includes('synthetic-private'));assert.equal(e.productionUiExercised,false);assert.ok(f.queries.every(q=>q.startsWith('SELECT')));assert.deepEqual(Object.keys(f.live.preservation),['18','21']);
}finally{f.cleanup();}});
test('DOC118 preflight refuses a changed publication before document effects',async()=>{const f=fixture();try{f.setPublication(false);await assert.rejects(preflight(f.live,f.dependencies),/PUBLICATION_UNAVAILABLE/);assert.equal(f.queries.length,1);assert.equal(fs.existsSync(path.join(f.live.directory,'preflight.json')),false);}finally{f.cleanup();}});
test('DOC118 preflight refuses another owner without inspecting protected attempts',async()=>{const f=fixture();try{f.setOwner('999');await assert.rejects(preflight(f.live,f.dependencies),/CURRENT_OWNER_UNCONFIRMED/);assert.equal(Object.hasOwn(f.live,'preservation'),false);assert.ok(f.requests.every(q=>q.loteId==='23'));}finally{f.cleanup();}});
test('DOC118 continuation keeps the rejected login and HTTP counts while opening only one new pair',async()=>{const f=fixture();try{
 f.live.carriedSessionAttempts=1;f.live.sessionCount=1;
 f.live.budget=createBudget({allHttpPerCase:{COORD02:425,COORD03:75},carried:{scenarios:{COORD02:{http:98,operations:{}},COORD03:{http:0,operations:{}}},selects:0}});
 await preflight(f.live,f.dependencies);assert.equal(f.live.sessionCount,3);assert.equal(f.opens(),2);assert.equal(f.live.budget.snapshot().scenarios.COORD02.http,100);
}finally{f.cleanup();}});
test('DOC118 console scope refuses a redistribution or effect increase not present in the explicit continuation',()=>{
 const {scope}=require('../tools/e2e/scripts/run-doc118-live.cjs');
 const base=JSON.parse(fs.readFileSync(path.join(__dirname,'../openspec/changes/doc-118-produccion-importacion-carga-carpeta-lote-recuperacion/execution-authorization.json'),'utf8'));
 const p={...structuredClone(profile),baseUrl:base.environment.baseUrl,nivelRef:'7',versionId:'38',odbcDsn:'MembershipUsers',docuarchiOdbcDsn:'MembershipUsers'};
 assert.doesNotThrow(()=>scope(base,p));for(const change of [a=>a.maximum.totalSessions=6,a=>a.maximum.allHttpPerCase.COORD02=1000,a=>a.maximum.newStoredDocuments=4,a=>a.budgetContinuationApproval.status='PENDING']){const a=structuredClone(base);change(a);assert.throws(()=>scope(a,p));}
});

function continuationFixture(){
 const a=JSON.parse(fs.readFileSync(path.join(__dirname,'../openspec/changes/doc-118-produccion-importacion-carga-carpeta-lote-recuperacion/execution-authorization.json'),'utf8'));
 const proof=JSON.parse(fs.readFileSync(path.join(__dirname,'../Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-login-preflight-20261107.json'),'utf8'));
 a.executionStatus='CONTINUATION_AUTHORIZED_AWAITING_MASKED_CAPTURE';
 Object.assign(a.consumed,{allHttp:{COORD02:98,COORD03:0},totalSessionAttempts:1,loginAttempts:1,selectStatements:0});
 return{a,proof,load:require('../tools/e2e/scripts/run-doc118-live.cjs').continuationConsumption};
}
test('DOC118 continuation carries immutable evidence and rejects a lower authorization ledger',()=>{
 const {a,proof,load}=continuationFixture(),carried=load(a,proof);proof.counters.scenarios.COORD02.http=0;
 assert.equal(carried.counters.scenarios.COORD02.http,98);assert.throws(()=>load(a,proof),/CARRIED_STATE_CHANGED/);
});
test('DOC118 stopped continuation cannot reuse a previous approval or start another login',()=>{
 const {a,proof,load}=continuationFixture();a.executionStatus='STOPPED_BUDGET_REVIEW_REQUIRED';assert.throws(()=>load(a,proof),/CONTINUATION_REVIEW_REQUIRED/);
 a.executionStatus='CONTINUATION_AUTHORIZED_AWAITING_MASKED_CAPTURE';proof.sessionAttempts=5;proof.counters.scenarios.COORD02.http=925;
 Object.assign(a.consumed,{allHttp:{COORD02:925,COORD03:0},totalSessionAttempts:5,loginAttempts:5});assert.throws(()=>load(a,proof),/CONTINUATION_CAPACITY_INSUFFICIENT/);
});
test('DOC118 continuation refuses unclosed sessions and uncleared secrets',()=>{
 for(const field of ['sessionsClosed','ephemeralSecretsCleared']){const {a,proof,load}=continuationFixture();proof[field]=false;assert.throws(()=>load(a,proof),/CARRIED_STATE_CHANGED/);}
});
test('DOC118 public startup diagnostics expose only closed guard codes',()=>{const {publicGuardCode}=require('../tools/e2e/scripts/run-doc118-live.cjs');assert.equal(publicGuardCode(Error('DOC118_LIVE_OBSERVER_NOT_READY_BEFORE_CAPTURE')),'DOC118_LIVE_OBSERVER_NOT_READY_BEFORE_CAPTURE');assert.equal(publicGuardCode(Error('private-password')),'DOC118_LIVE_START_UNCONFIRMED');assert.equal(publicGuardCode(Error('DOC118_LIVE_CARRIED_STATE_CHANGED_PRIVATE_TOKEN')),'DOC118_LIVE_START_UNCONFIRMED');});

test('DOC118 approved second continuation carries425 HTTP, one read and three openings before the last pair',()=>{
 const {a,load}=continuationFixture(),proof=JSON.parse(fs.readFileSync(path.join(__dirname,'../Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-continuation-preflight-20261107.json'),'utf8'));
 Object.assign(a.consumed,{allHttp:{COORD02:425,COORD03:0},totalSessionAttempts:3,loginAttempts:3});
 const carried=load(a,proof);assert.equal(carried.sessionAttempts,3);assert.equal(carried.counters.scenarios.COORD02.http,425);assert.equal(carried.counters.scenarios.COORD02.operations.ConsultarLote,1);
});

for(const exhausted of [false,true])test('DOC118 missing-host diagnostic '+(exhausted?'stops before API transport when budget is exhausted':'counts its read-only API probe in the same budget'),async()=>{
 const shared=require('../tools/e2e/tests/support/doc111-importacion-carpetas.cjs'),budget=createBudget();let posts=0,closed=0;
 if(exhausted)for(let i=0;i<250;i++)budget.http('COORD02');
 const auth={page:{goto:async()=>({status:()=>200}),locator:()=>({count:async()=>0})},context:{close:async()=>{closed++;},request:{post:async()=>{posts++;return{status:()=>403,json:async()=>({ok:false,codigo:'ACCESO_DENEGADO'})};}}}};
 await assert.rejects(shared.openSession({},profile,{}, {environment:true,account:true},{createSession:async()=>auth,beforeDiagnosticPost:op=>budget.http('COORD02',op)}),exhausted?/IMPORT_CONTROL_UNAVAILABLE/:/IMPORT_ACCESS_DENIED/);
 assert.equal(posts,exhausted?0:1);assert.equal(closed,1);assert.equal(budget.snapshot().scenarios.COORD02.http,exhausted?250:1);
 assert.equal(budget.snapshot().scenarios.COORD02.operations.AbrirImportacion,exhausted?undefined:1);
});
