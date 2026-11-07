'use strict';
const {test}=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs'),path=require('node:path');
const {runPreflight,validateProposal,authorize,PROPOSAL,ROOT}=require('../tools/e2e/scripts/run-doc117-alm02-preflight.cjs');
const proposal=()=>JSON.parse(fs.readFileSync(path.join(ROOT,PROPOSAL),'utf8'));
function fixture(){
 const p=proposal(),rule=(type,ref,source)=>({DestinoTipo:type,DestinoRef:ref,FuenteTipo:source,Extraccion:'COMPLETO',EtapaResolucion:'PREFLIGHT',TipoValor:'TEXTO',QuitarExtension:false,LongitudMaxima:160});
 const rules=[rule('CAMPO_EXPEDIENTE','CODIGO','CARPETA'),rule('CAMPO_EXPEDIENTE','NOMBRE','CARPETA'),rule('INDICE_GABINETE','14','EXPEDIENTE')];
 const rows=[[{version_id:'38',perfil_id:'25',configuracion_doc:JSON.stringify({GabineteId:10,OrigenGabinete:'DA',ModoIntegridad:'COMPLETO_OBLIGATORIO',AmbitoIntegridad:'LOTE'})}],rules.map((r,i)=>({regla_id:String(i+1),regla_doc:JSON.stringify(r)})),[],[],[],[{id_gabinete:'10',NOMBRE:'TEST_CABINET'}],[{id_detalle_gabinete:'14',CAMPO:'TEST_FIELD'}],[{intento_id:'41',lote_id:'18',estado:'EN_CURSO',fase:'CREAR_EXPEDIENTE',evidencia_doc:'PRIVATE_EVIDENCE'}]];
 const calls=[],progress=[];
 return{p,rows,calls,progress,run:()=>runPreflight({proposal:p,query:async(origin,sql,args)=>{const i=calls.length;calls.push({origin,sql,args});assert.equal(sql,p.controls[i].sql);assert.deepEqual(args,p.controls[i].parameters);return rows[i];},onProgress:x=>progress.push(x)})};
}
test('ALM02 SELECT preflight consumes exactly the eight approved controls and emits no rows',async()=>{
 const f=fixture(),r=await f.run();assert.equal(f.calls.length,8);assert.equal(r.status,'PASS');assert.equal(r.selectRequests,8);assert.equal(r.executionAuthorized,false);assert.equal(r.applicationLogins,0);assert.equal(r.documents,0);assert.equal(r.runtimeChanges,0);assert.equal(r.protectedAttemptObserved,true);assert.match(r.publicationSha256,/^[a-f0-9]{64}$/);
 const output=JSON.stringify([r,f.progress]);for(const text of ['PRIVATE_EVIDENCE','TEST_CABINET','TEST_FIELD','regla_doc','configuracion_doc'])assert.ok(!output.includes(text));
});
for(const [name,index,change,requests]of [
 ['missing publication',0,f=>{f.rows[0]=[];},1],
 ['recipe changes effective root code',1,f=>{const r=JSON.parse(f.rows[1][0].regla_doc);r.Extraccion='SEGMENTO';f.rows[1][0].regla_doc=JSON.stringify(r);},2],
 ['prior expedition',2,f=>{f.rows[2]=[{ID_EXPEDIENTE:'999'}];},3],
 ['prior plan',3,f=>{f.rows[3]=[{plan_id:'999'}];},4],
 ['prior root',4,f=>{f.rows[4]=[{raiz_id:'999'}];},5],
 ['missing index',6,f=>{f.rows[6]=[];},7],
 ['protected attempt differs',7,f=>{f.rows[7][0].estado='CONFIRMADO';},8]
])test('ALM02 preflight stops without retry: '+name,async()=>{const f=fixture();change(f);await assert.rejects(f.run(),/^Error: DOC117_ALM02_/);assert.equal(f.calls.length,requests);assert.equal(f.progress.at(-1).stage,f.p.controls[index].key);});
test('ALM02 preflight records the issued request and stops on uncertain SELECT result',async()=>{
 const f=fixture(),progress=[];let issued=0;await assert.rejects(runPreflight({proposal:f.p,query:async()=>{issued++;throw Error('DOC111_SELECT_FAILED');},onProgress:p=>progress.push(p)}),/DOC111_SELECT_FAILED/);assert.equal(issued,1);assert.equal(progress.at(-1).selectRequests,1);
});
test('ALM02 authorization binds scope, proposal, code and absence of effect permissions',()=>{
 const p=proposal(),hash='a'.repeat(64),a={authorized:true,scope:p.scope,environment:p.environment,proposalSha256:hash,codeSha256:hash,select:true,effects:false,applicationLogins:false,runtimeChanges:false,authorizationId:'b'.repeat(32)};
 assert.doesNotThrow(()=>authorize(a,p,hash,hash));for(const changed of [{effects:true},{applicationLogins:true},{runtimeChanges:true},{select:false},{scope:'ALM-01'},{codeSha256:'c'.repeat(64)},{proposalSha256:'c'.repeat(64)}])assert.throws(()=>authorize({...a,...changed},p,hash,hash),/SPECIFIC_AUTHORIZATION_REQUIRED/);
 assert.throws(()=>validateProposal({...p,budget:{...p.budget,documents:1}}),/BUDGET_CHANGED/);
});
