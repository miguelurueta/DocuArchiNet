'use strict';
const test=require('node:test'),assert=require('node:assert/strict');
const {queryRows,queryFingerprint}=require('../scripts/support/doc32-e2e-odbc.cjs');
const environment={DOC111_E2E_ODBC_DSN:'test-dsn',DOC111_E2E_MYSQL_USER:'PRIVATE_USER',DOC111_E2E_MYSQL_PASSWORD:'PRIVATE_PASSWORD'};

test('ODBC filas: PowerShell distingue alias semanticos para columnas que solo difieren en mayusculas', {skip:process.platform!=='win32'},()=>{
  const {execFileSync}=require('node:child_process');
  const output=execFileSync('powershell.exe',['-NoProfile','-Command',"$collision=[ordered]@{}; $collision['FORMATO']='inventory'; $collision['formato']='index'; $distinct=[ordered]@{}; $distinct['inventory_format']='inventory'; $distinct['index_format']='index'; @{collisionCount=$collision.Count; distinct=$distinct} | ConvertTo-Json -Compress"],{encoding:'utf8',windowsHide:true});
  const result=JSON.parse(output);assert.equal(result.collisionCount,1);assert.deepEqual(result.distinct,{inventory_format:'inventory',index_format:'index'});
});

test('ODBC filas: SELECT parametrizado reutiliza el puente existente y borra entradas efimeras',async()=>{
  let seen;
  const result=await queryRows('SELECT id,value FROM fixture WHERE id=? AND value=?',['17','0007'],environment,'DOC111_E2E',{execute:async(command,args,options)=>{
    seen=options.env;assert.equal(command,'powershell.exe');assert.ok(args.some(a=>a.endsWith('query-doc32-odbc-fingerprint.ps1')));assert.ok(args.includes('Rows'));assert.equal(options.windowsHide,true);
    assert.deepEqual(JSON.parse(options.env.DOC111_E2E_SELECT_PARAMETERS),['17','0007']);assert.ok(!args.includes('PRIVATE_PASSWORD'));
    return {stdout:'[{"id":"17","value":"0007","missing":null}]'};
  }});
  assert.deepEqual(result,[{id:'17',value:'0007',missing:null}]);assert.equal(seen.DOC111_E2E_MYSQL_PASSWORD,undefined);assert.equal(seen.DOC111_E2E_SELECT_PARAMETERS,undefined);assert.equal(environment.DOC111_E2E_MYSQL_PASSWORD,'PRIVATE_PASSWORD');assert.equal(typeof queryFingerprint,'function');
});

test('ODBC filas: rechaza escrituras, multiples sentencias, bloqueos y parametros discordantes antes de abrir',async()=>{
  let executions=0;const runtime={execute:async()=>{executions++;throw new Error('PRIVATE_PASSWORD');}};
  for(const [sql,args] of [['UPDATE fixture SET x=?',[1]],['SELECT ?; DELETE FROM fixture',[1]],['SELECT * FROM fixture FOR UPDATE',[]],['SELECT SLEEP(?)',[1]],['SELECT ?',[{}]],['SELECT ?',[NaN]],['SELECT ?',[]],['SELECT ?',[null,null]]])await assert.rejects(queryRows(sql,args,environment,'DOC111_E2E',runtime),/^Error: DOC111_SELECT_REQUIRED$/);
  assert.equal(executions,0);
});

test('ODBC filas: errores de apertura y resultados invalidos no exponen stdout, stderr ni credenciales',async()=>{
  for(const execute of [async()=>{throw Object.assign(new Error('PRIVATE_PASSWORD'),{stdout:'PRIVATE_USER',stderr:'mysql://PRIVATE_PASSWORD'});},async()=>({stdout:'{"password":"PRIVATE_PASSWORD"}'}),async()=>({stdout:'not-json PRIVATE_PASSWORD'})]){
    let seen;await assert.rejects(queryRows('SELECT ?',[null],environment,'DOC111_E2E',{execute:async(...args)=>{seen=args[2].env;return execute(...args);}}),/^Error: DOC111_SELECT_FAILED$/);assert.equal(seen.DOC111_E2E_MYSQL_PASSWORD,undefined);
  }
});

test('ODBC filas: conserva solo marcadores cerrados de diagnostico, nunca detalles del controlador',async()=>{
  for(const code of ['AUTHENTICATION_FAILED','DSN_UNAVAILABLE','DRIVER_UNAVAILABLE','DATABASE_ACCESS_DENIED','DATABASE_UNAVAILABLE','CONNECTION_UNAVAILABLE','OPEN_FAILED','QUERY_FAILED','RESULT_FAILED','INPUT_FAILED']){
    await assert.rejects(queryRows('SELECT ?',[0],environment,'DOC111_E2E',{execute:async()=>{throw {stderr:'DOC32_ODBC_'+code,message:'PRIVATE_PASSWORD'};}}),new RegExp('^Error: DOC111_SELECT_'+code+'$'));
  }
  await assert.rejects(queryRows('SELECT ?',[0],environment,'DOC111_E2E',{execute:async()=>{throw {stderr:'DOC32_ODBC_AUTHENTICATION_FAILED PRIVATE_PASSWORD'};}}),/^Error: DOC111_SELECT_FAILED$/);
});

test('ODBC filas: PowerShell real acepta arrays JSON vacios, escalares y null antes de abrir el DSN', {skip:process.platform!=='win32'},async()=>{
  const env={...environment,DOC111_E2E_ODBC_DSN:'doc111-unit-dsn-not-installed-'+process.pid};
  for(const [sql,parameters] of [['SELECT 1',[]],['SELECT ?',[0]],['SELECT ?',[null]],['SELECT ?,?',['one','two']]]){
    // An absent DSN stops before any network access; INPUT_FAILED would reveal a parsing regression.
    await assert.rejects(queryRows(sql,parameters,env,'DOC111_E2E'),/^Error: DOC111_SELECT_(?:DSN_UNAVAILABLE|OPEN_FAILED)$/);
  }
});
