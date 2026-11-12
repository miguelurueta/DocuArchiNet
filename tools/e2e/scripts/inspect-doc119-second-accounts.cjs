'use strict';
const fs=require('node:fs'),path=require('node:path');
const {collectValue,requireInteractiveConsole}=require('./support/interactive-e2e-console.cjs');
const {queryRows}=require('./support/doc32-e2e-odbc.cjs');
const {sourceLot,findSecondAccounts}=require('../tests/support/doc119-second-accounts.cjs');
const {check}=require('../tests/support/doc119-seguimiento.cjs');
const ROOT=path.resolve(__dirname,'../../..');
function parseArgs(args){
  check(args.length===4&&args[0]==='--run'&&/^[a-f0-9]{32}$/.test(args[1])&&args[2]==='--source-run'&&/^[a-f0-9]{32}$/.test(args[3]),'ACCOUNT_LOOKUP_ARGUMENT_INVALID');
  return {runId:args[1],sourceRunId:args[3]};
}
async function main(args=process.argv.slice(2)){
  const options=parseArgs(args);requireInteractiveConsole();
  process.title='DOC-119 - buscar segunda cuenta - '+options.runId.slice(0,8);
  const source=JSON.parse(fs.readFileSync(path.join(ROOT,'.tmp','doc119-seguimiento',options.sourceRunId,'state.json'),'utf8'));
  const lotId=sourceLot(source,options.sourceRunId);
  const directory=path.join(ROOT,'.tmp','doc119-account-lookup',options.runId);fs.mkdirSync(directory,{recursive:true});
  const statePath=path.join(directory,'state.json');
  const state={runId:options.runId,sourceRunId:options.sourceRunId,lotId,status:'WAITING_DB_INPUT',http:0,sessions:0,select:0,maxSelect:2,closed:false};
  fs.writeFileSync(statePath,JSON.stringify(state),{flag:'wx'});
  const save=patch=>{Object.assign(state,patch);fs.writeFileSync(statePath,JSON.stringify(state,null,2));};
  const values={};let environment;
  try{
    console.log('Consulta solicitada: cuentas activas de la empresa de la importacion, distintas de su propietario y habilitadas para importar.');
    console.log('Solo 2 SELECT. No inicia sesiones de aplicacion, consulta claves de usuarios ni cambia permisos. La prueba principal sigue esperando.');
    await collectValue(values,'dsn','DSN RA de lectura',{defaultValue:'MembershipUsers'});
    check(/^[A-Za-z0-9 _.-]{1,128}$/.test(values.dsn),'DSN_INVALID');
    await collectValue(values,'user','Cuenta MySQL de lectura (enmascarada)',{secret:true});
    await collectValue(values,'password','Clave MySQL de lectura',{secret:true});
    environment=Object.fromEntries(Object.entries(process.env).filter(([key])=>!/^DOC\d+(?:_DA)?_E2E/.test(key)&&!['DEBUG','PWDEBUG','NODE_DEBUG','NODE_DEBUG_NATIVE'].includes(key)));
    Object.assign(environment,{DOC119_E2E_ODBC_DSN:values.dsn,DOC119_E2E_MYSQL_USER:values.user,DOC119_E2E_MYSQL_PASSWORD:values.password});
    save({status:'QUERYING',startedUtc:new Date().toISOString()});
    const result=await findSecondAccounts(async(sql,parameters)=>{
      check(state.select<state.maxSelect,'ACCOUNT_LOOKUP_BUDGET_EXHAUSTED');save({select:state.select+1});
      return queryRows(sql,parameters,environment,'DOC119_E2E');
    },lotId);
    // Requested account identifiers only; no passwords, tokens, DB credentials
    // or full user rows are retained. The result stays in excluded local data.
    fs.writeFileSync(path.join(directory,'account-candidates.json'),JSON.stringify(result,null,2),{flag:'wx'});
    save({status:result.candidates.length?'FOUND':'NO_ELIGIBLE_SECOND_ACCOUNT',candidateCount:result.candidates.length});
    if(result.candidates.length){console.log('Cuentas candidatas verificadas por SELECT (no es una prueba de login):');for(const item of result.candidates)console.log(item.login);}
    else console.log('No hay otra cuenta activa de esta empresa con IMPORTAR_CARPETAS=1 y una identidad/perfil unicos.');
  }catch(error){save({status:'STOPPED',failure:/^(?:DOC119|DOC111)_[A-Z0-9_]+$/.test(error?.message)?error.message:'DOC119_ACCOUNT_LOOKUP_STOPPED'});process.exitCode=2;}
  finally{
    for(const key of Object.keys(values))delete values[key];
    if(environment)for(const key of Object.keys(environment))if(/^DOC119_E2E_/.test(key))delete environment[key];
    save({closed:true,credentialsCleared:true,finishedUtc:new Date().toISOString()});
    console.log('DOC119_ACCOUNT_LOOKUP='+JSON.stringify({runId:state.runId,status:state.status,select:state.select,candidateCount:state.candidateCount??null,failure:state.failure||null}));
  }
}
if(require.main===module)main().catch(()=>{console.error('DOC119_ACCOUNT_LOOKUP_PRECHECK_STOPPED');process.exitCode=2;});
module.exports={parseArgs,main};
