'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs'),path=require('node:path'),vm=require('node:vm');
const api=require('../tools/e2e/scripts/inspect-doc120-schema-readiness.cjs');
const source=fs.readFileSync(path.resolve(__dirname,'../tools/e2e/scripts/inspect-doc120-schema-readiness.cjs'),'utf8');
const good={ok:true,legacy:true,preparation:true,newTables:3,newColumns:31,readOnly:true};
test('DOC120-SCHEMA01: diez SELECT cerrados replican el lector de metadatos sin datos de negocio',()=>{
 const book=api.queries();assert.equal(book.length,10);assert.equal(api.MAX_SELECT,10);
 assert.deepEqual(book.map(q=>q.name),['Tablas','Columnas','Indices','Claves','Reglas','Tablas','Columnas','Indices','Claves','Reglas']);
 for(const q of book){assert.match(q.sql,/^SELECT /);assert.doesNotMatch(q.sql,/;|\b(?:INSERT|UPDATE|DELETE|ALTER|CREATE|DROP|CALL|OUTFILE|INTO|LOCK)\b/i);assert.match(q.sql,/FROM information_schema\./);assert.doesNotMatch(q.sql,/contenido_doc|config_doc|contexto_doc/);}
 assert.match(book[7].sql,/'ra_integracion_perfil_version'/);
});
test('DOC120-SCHEMA02: inspeccion entrega ambos contratos completos al validador real sin publicarlos',async()=>{
 let count=0;const result=await api.inspect(async q=>{count++;return[{TABLE_NAME:q.scope}];},packet=>{
  assert.equal(Object.keys(packet.legacy).length,5);assert.equal(Object.keys(packet.preparation).length,5);return good;
 });assert.deepEqual(result,good);assert.equal(count,10);
});
test('DOC120-SCHEMA03: error, exceso de filas o respuesta incompleta detiene sin repetir',async()=>{
 let reads=0;await assert.rejects(api.inspect(async()=>{reads++;throw Error('synthetic');},()=>good),/synthetic/);assert.equal(reads,1);
 await assert.rejects(api.inspect(async()=>Array(501).fill({}),()=>good),/ROWS_INVALID/);
 for(const result of [{...good,preparation:false},{...good,newTables:2},{...good,readOnly:false},{ok:false}])await assert.rejects(api.inspect(async()=>[],()=>result),/INCOMPATIBLE/);
});
function runner(failAt){
 const files=new Map(),logs=[],environments=[];let reads=0;
 const fakeProcess={argv:[],env:{PATH:'synthetic',DOC119_E2E_MYSQL_PASSWORD:'STALE_SECRET',DEBUG:'SECRET_DEBUG'}};
 const fakeRequire=name=>{
  if(name==='node:fs')return{existsSync:()=>false,mkdirSync(){},copyFileSync(){},readFileSync:()=>Buffer.from('synthetic-binary'),writeFileSync:(file,body)=>files.set(path.basename(file),String(body))};
  if(name==='node:child_process')return{execFileSync:exe=>exe.endsWith('csc.exe')?'':JSON.stringify(good)};
  if(name==='./support/interactive-e2e-console.cjs')return{requireInteractiveConsole(){},async collectValue(values,name){values[name]={dsn:'SyntheticDsn',user:'SECRET_USER',password:'SECRET_PASSWORD'}[name];}};
  if(name==='./support/doc32-e2e-odbc.cjs')return{async queryRows(sql,parameters,environment){environments.push(environment);assert.equal(environment.DOC119_E2E_MYSQL_PASSWORD,undefined);assert.equal(environment.DEBUG,undefined);reads++;if(reads===failAt)throw Error('DOC111_SELECT_AUTHENTICATION_FAILED');return[];}};
  return require(name);
 };
 const module={exports:{}};vm.runInNewContext(source,{require:fakeRequire,module,__dirname:path.resolve(__dirname,'../tools/e2e/scripts'),process:fakeProcess,console:{log:v=>logs.push(v),error:v=>logs.push(v)}});
 return{api:module.exports,files,logs,environments,process:fakeProcess,reads:()=>reads};
}
test('DOC120-SCHEMA04: consola saneada cuenta diez intentos y elimina credenciales tras exito',async()=>{
 const f=runner();await f.api.main(['--run','a'.repeat(32),'--authorized-readonly']);const state=JSON.parse(f.files.get('state.json'));
 assert.equal(state.status,'PASS_SCHEMA_COMPATIBLE');assert.equal(state.select,10);assert.equal(state.completedSelect,10);assert.equal(state.businessEffects,0);assert.equal(state.http,0);assert.equal(state.sessions,0);assert.equal(state.credentialsCleared,true);assert.equal(state.closed,true);
 assert.doesNotMatch([...f.files.values(),...f.logs].join('\n'),/SECRET|SyntheticDsn/);for(const env of f.environments){assert.equal(env.DOC120_E2E_MYSQL_PASSWORD,undefined);assert.equal(env.DOC120_E2E_MYSQL_USER,undefined);}
});
test('DOC120-SCHEMA05: fallo SELECT conserva consumo, categoria cerrada y cero reintentos',async()=>{
 const f=runner(3);await f.api.main(['--run','b'.repeat(32),'--authorized-readonly']);const state=JSON.parse(f.files.get('state.json'));
 assert.equal(state.status,'STOPPED');assert.equal(state.failure,'DOC120_SCHEMA_DB_AUTHENTICATION_FAILED');assert.equal(state.select,3);assert.equal(state.completedSelect,2);assert.equal(f.reads(),3);assert.equal(state.credentialsCleared,true);assert.equal(f.process.exitCode,2);
 assert.doesNotMatch([...f.files.values(),...f.logs].join('\n'),/SECRET|SyntheticDsn/);assert.equal(api.errorCode(Error('password=SECRET')),'DOC120_SCHEMA_STOPPED');
});
