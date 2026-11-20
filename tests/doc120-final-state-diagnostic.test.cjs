'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs'),path=require('node:path'),vm=require('node:vm');
const source=fs.readFileSync(path.resolve(__dirname,'../tools/e2e/scripts/inspect-doc120-final-state.cjs'),'utf8');
const failures=['AUTHENTICATION_FAILED','DSN_UNAVAILABLE','DRIVER_UNAVAILABLE','DATABASE_ACCESS_DENIED','DATABASE_UNAVAILABLE','CONNECTION_UNAVAILABLE','OPEN_FAILED','QUERY_FAILED','COLUMN_UNAVAILABLE','TABLE_UNAVAILABLE','QUERY_UNSUPPORTED','RESULT_FAILED','INPUT_FAILED','FAILED','REQUIRED'];
function runner(query){
 const files=new Map(),logs=[],environments=[],requests=[];
 const fakeProcess={argv:[],env:{PATH:'synthetic',DOC119_E2E_MYSQL_PASSWORD:'STALE_SECRET',DEBUG:'SECRET_DEBUG'}};
 const fakeRequire=name=>{
  if(name==='node:fs')return{mkdirSync(){},writeFileSync(file,body){files.set(path.basename(file),String(body));}};
  if(name==='./support/interactive-e2e-console.cjs')return{requireInteractiveConsole(){},async collectValue(values,name){values[name]={dsn:'SyntheticDsn',user:'SECRET_USER',password:'SECRET_PASSWORD'}[name];}};
  if(name==='./support/doc32-e2e-odbc.cjs')return{async queryRows(sql,parameters,environment,prefix){requests.push({sql,parameters,prefix});environments.push(environment);assert.equal(environment.DOC119_E2E_MYSQL_PASSWORD,undefined);assert.equal(environment.DEBUG,undefined);return query(requests.length);}};
  return require(name);
 };
 const module={exports:{}};
 vm.runInNewContext(source,{require:fakeRequire,module,__dirname:path.resolve(__dirname,'../tools/e2e/scripts'),process:fakeProcess,console:{log:value=>logs.push(value),error:value=>logs.push(value)}});
 return{api:module.exports,files,logs,environments,requests,process:fakeProcess};
}
test('DOC120-DIAG01: conserva cada categoría cerrada ODBC y detiene sin reintento ni secretos',async()=>{
 for(const suffix of failures){
  const run=runner(()=>{throw Error('DOC111_SELECT_'+suffix);});
  await run.api.main(['--lot','39','--root','60','--authorized']);
  const state=JSON.parse(run.files.get('state.json'));
  assert.equal(state.failure,'DOC120_DIAGNOSTIC_DB_'+suffix);assert.equal(state.query,'lot');
  assert.equal(state.select,1);assert.equal(state.completedSelect,0);assert.equal(state.status,'STOPPED');
  assert.equal(state.closed,true);assert.equal(state.credentialsCleared,true);assert.equal(state.http,0);assert.equal(state.sessions,0);
  assert.equal(run.requests.length,1);assert.equal(run.files.has('diagnostic.json'),false);assert.equal(run.process.exitCode,2);
  assert.doesNotMatch([...run.files.values(),...run.logs].join('\n'),/SECRET|SyntheticDsn|DOC111_SELECT/);
  for(const environment of run.environments){assert.equal(environment.DOC120_E2E_MYSQL_PASSWORD,undefined);assert.equal(environment.DOC120_E2E_MYSQL_USER,undefined);}
 }
});
test('DOC120-DIAG02: mensajes desconocidos o ampliados no se imprimen ni se conservan',async()=>{
 for(const message of ['password=SECRET_PASSWORD','DOC111_SELECT_OPEN_FAILED password=SECRET_PASSWORD','DOC120_DIAGNOSTIC_PRIVATE_SECRET','DOC111_SELECT_NOT_ALLOWED']){
  const run=runner(()=>{throw Error(message);});await run.api.main(['--lot','39','--root','60','--authorized']);
  assert.equal(JSON.parse(run.files.get('state.json')).failure,'DOC120_DIAGNOSTIC_STOPPED');
  assert.doesNotMatch([...run.files.values(),...run.logs].join('\n'),/SECRET_PASSWORD|PRIVATE_SECRET|NOT_ALLOWED/);assert.equal(run.requests.length,1);
 }
});
test('DOC120-DIAG03: una lectura concluida se cuenta antes del rechazo de alcance, sin leer la raíz',async()=>{
 const run=runner(()=>[]);await run.api.main(['--lot','39','--root','60','--authorized']);const state=JSON.parse(run.files.get('state.json'));
 assert.equal(state.failure,'DOC120_DIAGNOSTIC_LOT_NOT_FOUND');assert.equal(state.select,1);assert.equal(state.completedSelect,1);assert.equal(run.requests.length,1);
});
test('DOC120-DIAG04: una corrida íntegra distingue cinco lecturas concluidas y elimina secretos',async()=>{
 const run=runner(number=>number===1?[{lote_id:'39'}]:number===2?[{lote_id:'39',raiz_id:'60'}]:[]);
 await run.api.main(['--lot','39','--root','60','--authorized']);const state=JSON.parse(run.files.get('state.json'));
 assert.equal(state.status,'FINISHED');assert.equal(state.select,5);assert.equal(state.completedSelect,5);assert.equal(state.query,'events');assert.equal(state.maxSelect,5);assert.equal(state.closed,true);assert.equal(run.requests.length,5);assert.ok(run.files.has('diagnostic.json'));
 assert.doesNotMatch([...run.files.values(),...run.logs].join('\n'),/SECRET/);assert.equal(run.environments[0].DOC120_E2E_MYSQL_PASSWORD,undefined);
});
