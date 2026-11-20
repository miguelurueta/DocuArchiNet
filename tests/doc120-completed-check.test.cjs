'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs'),path=require('node:path'),vm=require('node:vm');
const actual=require('../tools/e2e/tests/support/doc120-qa-window.cjs');
const config=JSON.stringify({VersionId:'45',Configuracion:{OrigenGabinete:'DA',GabineteId:10}});
const file=path.resolve(__dirname,'../tools/e2e/tests/support/doc120-completed-check.cjs'),moduleObject={exports:{}};
vm.runInNewContext(fs.readFileSync(file,'utf8'),{module:moduleObject,require:name=>name==='./doc120-qa-window.cjs'?{...actual,hash:value=>value===config?'9140cc7400c96a17e717372664f3db51a497e63c9348fbe2915520e4f6e28e82':actual.hash(value)}:require(name)});
const api=moduleObject.exports;
function dataset(){
 const files=Array.from({length:1000},(_,i)=>({lote_id:'52',archivo_id:String(2097+i),raiz_id:'73',revision:7,estado_recepcion:'RECIBIDO',estado_efecto:'COMPLETADO',bytes_previstos:String(i===999?44961:44859),bytes_recibidos:String(i===999?44961:44859),active_marker:0,fixed_values:1,resultado_doc:JSON.stringify({Estado:'CONFIRMADO',DocumentoId:String(20000+i),InventarioId:String(30000+i),Expediente:{ExpedienteId:'696',RelacionNivelId:'1120'}})}));
 const h={lote_id:'52',plan_id:'91',nivel_ref:'223',version_id:'45',estado:'COMPLETADO',revision:4000,detencion_solicitada:0,total_raices:1,total_archivos:1000,config_doc:config,unresolved:0};
 const docs=files.map((_,i)=>({document_id:String(20000+i),inventory_id:String(30000+i),expedient_id:'696',document_user:'141',company_id:'2',inventory_user:'141',inventory_document:String(20000+i),inventory_expedient:'696',cabinet_name:'CONTABIL',indexed_expedient:'696'}));
 return {before:[h],root:[{lote_id:'52',raiz_id:'73',intento_activo_id:null,identidad_expediente_doc:JSON.stringify({ExpedienteId:'696',RelacionNivelId:'1120'})}],files,attempts:[{fase:'RECEPCION',estado:'CONFIRMADO',total:1000,files:1000},{fase:'ALMACENAR',estado:'CONFIRMADO',total:1000,files:1000},{fase:'CREAR_EXPEDIENTE',estado:'CONFIRMADO',total:1,files:0}],cabinet:[{id_gabinete:'10',NOMBRE:'CONTABIL'}],documents:docs,relation:[{expedition_id:'696',company_id:'2',owner_id:'141',relation_id:'1120',level_id:'223'}],files_after:structuredClone(files),documents_after:structuredClone(docs),after:[structuredClone(h)]};
}
function reader(data,calls=[]){return async(origin,name,sql,parameters)=>{calls.push({origin,name,sql,parameters});assert.match(sql,/^SELECT\b/);assert.doesNotMatch(sql,/;|\b(?:INSERT|UPDATE|DELETE|ALTER|DROP|CREATE|FOR UPDATE|INTO OUTFILE)\b/i);return data[name];};}
test('COMPLETE01:1000 resultados unicos RA/DA y relacion correcta,10 SELECT sin efectos',async()=>{
 const calls=[],result=await api.inspect(reader(dataset(),calls));assert.equal(calls.length,10);assert.deepEqual(calls.map(c=>c.name),['before','root','files','attempts','cabinet','documents','relation','files_after','documents_after','after']);
 assert.equal(result.received,1000);assert.equal(result.stored,1000);assert.equal(result.uniqueDocuments,1000);assert.equal(result.receivedBytes,44859102);assert.equal(result.stableRead,true);assert.equal(result.proofs.length,1000);
 for(const c of calls.filter(c=>['before','root','files','files_after','after'].includes(c.name)))assert.deepEqual(Array.from(c.parameters),['52','52','141','2']);
});
test('COMPLETE02: alcance, pendientes, resultados malformados y duplicados nunca aprueban',async()=>{
 const cases=[['LOT_SCOPE_CHANGED',d=>d.before[0].lote_id='50'],['LOT_NOT_COMPLETED',d=>d.before[0].estado='EN_CURSO'],['CONFIG_CHANGED',d=>d.before[0].config_doc='{}'],['ROOT_CHANGED',d=>d.root[0].raiz_id='74'],['EXPEDITION_CHANGED',d=>d.root[0].identidad_expediente_doc='{}'],['FILE_COUNT_INVALID',d=>d.files.pop()],['FILE_SCOPE_CHANGED',d=>d.files[0].raiz_id='74'],['FILE_NOT_COMPLETED',d=>d.files[0].estado_efecto='NO_INICIADO'],['RECEIVED_BYTES_INVALID',d=>d.files[0].bytes_recibidos='1'],['JSON_INVALID',d=>d.files[0].resultado_doc='invalid'],['DUPLICATE_DOCUMENT',d=>d.files[1].resultado_doc=d.files[0].resultado_doc],['UNRESOLVED_ATTEMPT',d=>d.attempts.push({fase:'ALMACENAR',estado:'INCIERTO',total:1})],['CONFIRMED_ATTEMPTS_INVALID',d=>d.attempts[1].total=1001],['CREATION_COUNT_INVALID',d=>d.attempts[2].total=2]];
 for(const [code,mutate]of cases){const data=dataset();mutate(data);await assert.rejects(api.inspect(reader(data)),e=>e.message==='DOC120_COMPLETED_'+code);}
});
test('COMPLETE03: documentos faltantes, dobles, fuera de empresa y cambios entre lecturas se rechazan',async()=>{
 const cases=[['DA_DOCUMENT_COUNT_INVALID',d=>d.documents.pop()],['DA_DUPLICATE_OR_UNKNOWN_DOCUMENT',d=>d.documents[1]={...d.documents[0]}],['DA_RELATION_INVALID',d=>d.documents[0].company_id='3'],['DA_RELATION_INVALID',d=>d.documents[0].indexed_expedient=null],['EXPEDITION_RELATION_INVALID',d=>d.relation.push({...d.relation[0],relation_id:'1121'})],['READ_CHANGED',d=>d.files_after[0].revision++],['READ_CHANGED',d=>d.documents_after[0].inventory_user='999'],['READ_CHANGED',d=>d.after[0].revision++]];
 for(const [code,mutate]of cases){const data=dataset();mutate(data);await assert.rejects(api.inspect(reader(data)),e=>e.message==='DOC120_COMPLETED_'+code);}
 for(const name of ['CONTABIL;DELETE','a`','a.b','x'.repeat(65)])assert.throws(()=>api.documentSql(name),/CABINET_NAME_INVALID/);
});
for(const mode of ['no-consent','bad-ra','db-failure'])test('COMPLETE04 '+mode+': nueva ventana no accede sin SI y elimina secretos sin reintentar',async()=>{
 const file=path.resolve(__dirname,'../tools/e2e/scripts/inspect-doc120-final-state.cjs'),states=[],outputs=[],queries=[],environments=[],runtime={module:{exports:{}},__dirname:path.dirname(file),process:{argv:[],env:{DOC119_E2E_MYSQL_PASSWORD:'PRIVATE_OLD',DEBUG:'PRIVATE_DEBUG'}},console:{log:v=>outputs.push(v),error:v=>outputs.push(v)}};
 runtime.require=name=>{
  if(name==='node:fs')return {mkdirSync(){},writeFileSync(_p,value){states.push(JSON.parse(value));},readFileSync:()=>Buffer.from('sealed')};
  if(name==='./support/interactive-e2e-console.cjs')return {requireInteractiveConsole(){},async collectValue(values,key,_label,options={}){values[key]=key==='authorization'?(mode==='no-consent'?'NO':'SI'):key==='ra'&&mode==='bad-ra'?'INVALID;DSN':options.defaultValue||'PRIVATE_VALUE';}};
  if(name==='./support/doc32-e2e-odbc.cjs')return {queryRows:async(sql,args,env)=>{queries.push(sql);environments.push(env);assert.equal(env.DEBUG,undefined);assert.equal(env.DOC119_E2E_MYSQL_PASSWORD,undefined);throw Error('DOC111_SELECT_QUERY_FAILED');}};
  if(name==='../tests/support/doc120-completed-check.cjs')return {SCOPE:{maxSelect:10},inspect:read=>read('RA','before','SELECT lote_id FROM ra_integracion_lote WHERE lote_id=?',['52'])};
  return require(require.resolve(name,{paths:[path.dirname(file)]}));
 };
 vm.runInNewContext(fs.readFileSync(file,'utf8'),runtime);await runtime.module.exports.completedMain(['--lot','52','--root','73','--completed-readonly']);
 const last=states.at(-1);assert.equal(last.closed,true);assert.equal(last.credentialsCleared,true);assert.equal(last.status,'STOPPED');assert.equal(last.businessEffects,0);assert.equal(last.http,0);assert.equal(queries.length,mode==='db-failure'?1:0);assert.doesNotMatch(JSON.stringify(states)+outputs.join(''),/PRIVATE_|INVALID;DSN/);
 for(const env of environments)assert.equal(env.DOC120_E2E_MYSQL_PASSWORD,undefined);
 await assert.rejects(runtime.module.exports.completedMain(['--lot','50','--root','73','--completed-readonly']),/SCOPE_INVALID/);
});
