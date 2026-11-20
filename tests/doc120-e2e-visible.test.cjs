'use strict';
const test=require('node:test'),assert=require('node:assert/strict');
const fs=require('node:fs'),path=require('node:path'),vm=require('node:vm');
const {parseArgs,launchOptions}=require('../tools/e2e/scripts/run-doc119-seguimiento.cjs');
const id='a'.repeat(32),prior='b'.repeat(32),args=['--run',id,'--authorized-plan'];
test('VISIBLE-01: modo existente y check conservan contrato sin lanzar navegador',()=>{
 assert.deepEqual(parseArgs(args),{runId:id});assert.deepEqual(parseArgs(['--check']),{check:true});assert.deepEqual(launchOptions(parseArgs(args)),{headless:true});
 assert.deepEqual(parseArgs([...args,'--reuse-selection',prior]),{runId:id,selectionRunId:prior});
});
test('VISIBLE-02: headed es presentacion observable sobre la misma prueba y seleccion',()=>{
 assert.deepEqual(parseArgs([...args,'--headed']),{runId:id,headed:true});assert.deepEqual(launchOptions({headed:true}),{headless:false,slowMo:300});
 assert.deepEqual(parseArgs([...args,'--reuse-selection',prior,'--headed']),{runId:id,headed:true,selectionRunId:prior});
});
test('VISIBLE-03: no acepta flags extra, combinaciones ambiguas o identidad reutilizada',()=>{
 for(const input of [['--check','--headed'],[...args,'--headed','--headed'],['--headed',...args],[...args,'--reuse-selection',id,'--headed'],[...args,'--effects'],['--run',id,'--headed']])assert.throws(()=>parseArgs(input),/DOC119_ARGUMENT_INVALID/);
});
for(const answer of ['NO','SI'])test('VISIBLE-04 '+answer+': consentimiento antes de credenciales y cero acceso al ambiente',async()=>{
 const file=path.resolve(__dirname,'../tools/e2e/scripts/run-doc119-seguimiento.cjs'),states=[],prompts=[];
 const runtime={module:{exports:{}},exports:{},__dirname:path.dirname(file),process:{argv:[],env:{},pid:1},console:{log(){},error(){}},Buffer,Set,Date};
 runtime.require=name=>{
  if(name==='node:fs')return {mkdirSync(){},writeFileSync(_path,value){states.push(JSON.parse(value));}};
  if(name==='./support/interactive-e2e-console.cjs')return {requireInteractiveConsole(){},async collectValue(target,key,_label,options={}){prompts.push(key);if(key==='authorizeVisible'){target[key]=answer;return;}if(['odbcDsn','docuarchiOdbcDsn','tls'].includes(key)){target[key]=options.defaultValue;return;}throw Error('STOP_AT_PRIVATE_INPUT');}};
  if(name==='./support/doc32-e2e-odbc.cjs')return {queryRows(){throw Error('UNEXPECTED_ENVIRONMENT_ACCESS');}};
  if(name==='../node_modules/playwright')throw Error('UNEXPECTED_BROWSER_START');
  return require(require.resolve(name,{paths:[path.dirname(file)]}));
 };
 vm.runInNewContext(fs.readFileSync(file,'utf8'),runtime,{filename:file});await runtime.module.exports.main([...args,'--headed']);
 assert.equal(states[0].authorizedPlan,false);assert.equal(states.at(-1).authorizedPlan,answer==='SI');assert.equal(states.at(-1).closed,true);assert.equal(states.at(-1).credentialsCleared,true);assert.deepEqual(states.at(-1).usage,{});
 assert.equal(prompts[0],'authorizeVisible');assert.equal(prompts.includes('ownerPassword'),false);assert.equal(prompts.includes('ownerUser'),answer==='SI');
});
