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
 const dependencies={chromium:{launch:async()=>browser},openSession:async b=>{opens++;return{context:await b.newContext(),page:{locator:()=>({getAttribute:async()=>'synthetic-private-csrf'})}};},selectProductionLevel:async(_p,n)=>assert.equal(n,'7'),queryRows:async(sql,args)=>{
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
 assert.ok(!saved.includes('synthetic-private'));assert.ok(f.queries.every(q=>q.startsWith('SELECT')));assert.deepEqual(Object.keys(f.live.preservation),['18','21']);
}finally{f.cleanup();}});
test('DOC118 preflight refuses a changed publication before document effects',async()=>{const f=fixture();try{f.setPublication(false);await assert.rejects(preflight(f.live,f.dependencies),/PUBLICATION_UNAVAILABLE/);assert.equal(f.queries.length,1);assert.equal(fs.existsSync(path.join(f.live.directory,'preflight.json')),false);}finally{f.cleanup();}});
test('DOC118 preflight refuses another owner without inspecting protected attempts',async()=>{const f=fixture();try{f.setOwner('999');await assert.rejects(preflight(f.live,f.dependencies),/CURRENT_OWNER_UNCONFIRMED/);assert.equal(Object.hasOwn(f.live,'preservation'),false);assert.ok(f.requests.every(q=>q.loteId==='23'));}finally{f.cleanup();}});
