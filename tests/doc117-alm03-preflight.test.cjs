'use strict';
const {test}=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs'),path=require('node:path');
const {ROOT,PROPOSAL,sha,validateProposal,authorize,runPreflight}=require('../tools/e2e/scripts/run-doc117-alm03-preflight.cjs');
function fixture(){
 const p=JSON.parse(fs.readFileSync(path.join(ROOT,PROPOSAL),'utf8'));
 const config={GabineteId:10,OrigenGabinete:'DA',ModoIntegridad:'COMPLETO_OBLIGATORIO',AmbitoIntegridad:'LOTE'};
 const rule=(DestinoTipo,DestinoRef,FuenteTipo)=>({DestinoTipo,DestinoRef,FuenteTipo,Extraccion:'COMPLETO',EtapaResolucion:'PREFLIGHT',TipoValor:'TEXTO',QuitarExtension:false,LongitudMaxima:160});
 const rules=[rule('CAMPO_EXPEDIENTE','CODIGO','CARPETA'),rule('CAMPO_EXPEDIENTE','NOMBRE','CARPETA'),rule('INDICE_GABINETE','14','EXPEDIENTE')];p.expectedPublicationSha256=sha(JSON.stringify({config,rules}));
 const rows=[[{version_id:'38',configuracion_doc:JSON.stringify(config)}],rules.map(r=>({regla_doc:JSON.stringify(r)})),[],[],[],[{id_gabinete:'10',NOMBRE:'PRIVATE_CABINET'}],[{id_detalle_gabinete:'14',CAMPO:'PRIVATE_FIELD'}],[{ID_EXPEDIENTE:'467',ID_EMPRESA_EXPEDIENTE:'2',GABINETE_PRODUCION:'PRIVATE_CABINET',nivel_id:'7',relacion_id:'999',id_instrumento:'1',CODIGO_SERIE_TRD:'2',CODIGO_SUB_SERIE_TRD:null}],[{type_id:'50'}],[{id_nivel:'7'}],[{intento_id:'41',lote_id:'18',fase:'CREAR_EXPEDIENTE',estado:'EN_CURSO',fin_utc:null,evidencia_doc:'PRIVATE_EVIDENCE'},{intento_id:'52',lote_id:'21',fase:'ALMACENAR',estado:'CONFIRMADO',fin_utc:'2026-11-06',evidencia_doc:JSON.stringify({Estado:'CONFIRMADO',DocumentoId:'16222',InventarioId:'23320'})},{intento_id:'53',lote_id:'21',fase:'ALMACENAR',estado:'EN_CURSO',fin_utc:null,evidencia_doc:JSON.stringify({Estado:'EN_CURSO',DocumentoId:null,InventarioId:null})}],[]];
 const calls=[],progress=[];
 return{p,rows,calls,progress,run:()=>runPreflight({proposal:p,query:async(origin,sql,args)=>{const i=calls.length;calls.push({origin,sql,args});assert.equal(sql,p.controls[i].sql);assert.deepEqual(args,p.controls[i].parameters);return rows[i];},onProgress:v=>progress.push(v)})};
}
test('ALM03 preflight performs twelve bounded SELECT controls and exposes only sanitized proof',async()=>{
 const f=fixture(),r=await f.run();assert.equal(r.status,'PASS');assert.equal(r.selectRequests,12);assert.equal(f.calls.length,12);assert.equal(r.proof.individualRelationId,'999');assert.equal(r.proof.protectedAttemptsVerified,true);assert.equal(r.effectsAuthorized,false);
 const output=JSON.stringify([r,f.progress]);for(const value of ['PRIVATE_CABINET','PRIVATE_FIELD','PRIVATE_EVIDENCE','configuracion_doc','evidencia_doc'])assert.ok(!output.includes(value));
});
for(const [name,index,change]of [
 ['publication drift',1,f=>{f.rows[1][0].regla_doc='{}';}],
 ['prior expedition',2,f=>{f.rows[2]=[{ID_EXPEDIENTE:'999'}];}],
 ['prior plan',3,f=>{f.rows[3]=[{plan_id:'999'}];}],
 ['prior root',4,f=>{f.rows[4]=[{raiz_id:'999'}];}],
 ['missing cabinet index',6,f=>{f.rows[6]=[];}],
 ['individual destination missing',7,f=>{f.rows[7]=[];}],
 ['individual cabinet changed',7,f=>{f.rows[7][0].GABINETE_PRODUCION='OTHER';}],
 ['individual relation ambiguous',7,f=>{f.rows[7].push({...f.rows[7][0]});}],
 ['individual typology unavailable',8,f=>{f.rows[8]=[];}],
 ['level authority absent',9,f=>{f.rows[9]=[];}],
 ['protected uncertain attempt changed',10,f=>{f.rows[10][2].estado='CONFIRMADO';}],
 ['protected confirmed document changed',10,f=>{f.rows[10][1].evidencia_doc=JSON.stringify({Estado:'CONFIRMADO',DocumentoId:'999',InventarioId:'23320'});}],
 ['individual filename already used',11,f=>{f.rows[11]=[{ID_REGISTRO_PRODUCION_DOCUMENTAL:'999'}];}]
])test('ALM03 preflight stops without retry: '+name,async()=>{const f=fixture();change(f);await assert.rejects(f.run(),/DOC117_ALM03_PREFLIGHT_/);assert.equal(f.calls.length,index+1);assert.equal(f.progress.at(-1).phase,'REQUESTED');});
test('ALM03 preflight stops on uncertain transport and on failed durable progress before querying',async()=>{
 const f=fixture();let calls=0;const progress=[];await assert.rejects(runPreflight({proposal:f.p,query:async()=>{calls++;throw Error('DOC111_SELECT_FAILED');},onProgress:v=>progress.push(v)}),/DOC111_SELECT_FAILED/);assert.equal(calls,1);assert.equal(progress.at(-1).selectRequests,1);
 calls=0;await assert.rejects(runPreflight({proposal:f.p,query:async()=>{calls++;return[];},onProgress:()=>{throw Error('JOURNAL_FAILED');}}),/JOURNAL_FAILED/);assert.equal(calls,0);
});
test('ALM03 preflight rejects inherited scopes, effect grants, writes and enlarged budgets',()=>{
 const p=fixture().p,h='a'.repeat(64),a={authorized:true,scope:p.scope,environment:p.environment,proposalSha256:h,codeSha256:h,select:true,effects:false,applicationLogins:false,runtimeChanges:false,authorizationId:'b'.repeat(32)};
 assert.doesNotThrow(()=>authorize(a,p,h,h));for(const change of [{scope:'ALM-02-INTERNAL'},{scope:'ALM-02-PREFLIGHT-SELECT'},{effects:true},{applicationLogins:true},{runtimeChanges:true},{select:false},{codeSha256:'c'.repeat(64)}])assert.throws(()=>authorize({...a,...change},p,h,h),/SPECIFIC_AUTHORIZATION_REQUIRED/);
 for(const change of [f=>{f.p.budget.maximumSelectRequests=13;},f=>{f.p.controls[0].sql='DELETE FROM x';},f=>{f.p.controls[0].sql+='; SELECT 1';},f=>{f.p.proposedRootNames[0]='OTHER';}]){const f=fixture();change(f);assert.throws(()=>validateProposal(f.p),/DOC117_ALM03_PREFLIGHT_/);}
});
