'use strict';
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto'),cp=require('node:child_process');
const {collectValue,requireInteractiveConsole}=require('./support/interactive-e2e-console.cjs');
const {queryRows}=require('./support/doc32-e2e-odbc.cjs');
const ROOT=path.resolve(__dirname,'../../..'),PACKAGE=path.join(ROOT,'.tmp/doc120-deploy-20261117/bin');
const NEW_TABLES=['ra_integracion_preparacion','ra_integracion_preparacion_parte','ra_integracion_preparacion_resultado'];
const LEGACY_TABLES=['ra_integracion_plan','ra_integracion_lote','ra_integracion_raiz','ra_integracion_archivo','ra_integracion_intento','ra_integracion_evento'];
const MAX_SELECT=10;
function queries(){
 const book=[];
 for(const [scope,tables,parents] of [['legacy',LEGACY_TABLES,[...LEGACY_TABLES,'ra_integracion_perfil_version','remit_dest_interno_perfil_produccion']],['preparation',NEW_TABLES,NEW_TABLES]]){
  const own=tables.map(t=>"'"+t+"'").join(','),all=parents.map(t=>"'"+t+"'").join(',');
  const rows={
   Tablas:'SELECT TABLE_NAME,ENGINE,TABLE_COLLATION FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME IN ('+all+')',
   Columnas:"SELECT TABLE_NAME,COLUMN_NAME,DATA_TYPE,COLUMN_TYPE,CHARACTER_MAXIMUM_LENGTH,COLLATION_NAME,IS_NULLABLE,EXTRA,COLUMN_DEFAULT FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND (TABLE_NAME IN ("+own+") OR (TABLE_NAME='ra_integracion_perfil_version' AND COLUMN_NAME='version_id') OR (TABLE_NAME='remit_dest_interno_perfil_produccion' AND COLUMN_NAME='IMPORTAR_CARPETAS'))",
   Indices:"SELECT TABLE_NAME,INDEX_NAME,NON_UNIQUE,SEQ_IN_INDEX,COLUMN_NAME,SUB_PART FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME IN ("+own+",'ra_integracion_perfil_version')",
   Claves:'SELECT TABLE_NAME,CONSTRAINT_NAME,COLUMN_NAME,ORDINAL_POSITION,REFERENCED_TABLE_NAME,REFERENCED_COLUMN_NAME,(REFERENCED_TABLE_SCHEMA=DATABASE()) AS MISMO_ESQUEMA FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME IN ('+own+') AND REFERENCED_TABLE_NAME IS NOT NULL',
   Reglas:'SELECT TABLE_NAME,CONSTRAINT_NAME,UPDATE_RULE,DELETE_RULE FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND TABLE_NAME IN ('+own+')'};
  for(const [name,sql] of Object.entries(rows))book.push({scope,name,sql});
 }
 return book;
}
async function inspect(read,validate){
 const packet={legacy:{},preparation:{}};
 for(const q of queries()){
  const rows=await read(q);
  if(!Array.isArray(rows)||rows.length>500||rows.some(r=>!r||typeof r!=='object'||Array.isArray(r)))throw Error('DOC120_SCHEMA_ROWS_INVALID');
  packet[q.scope][q.name]=rows;
 }
 const result=await validate(packet);
 if(result?.ok!==true||result.legacy!==true||result.preparation!==true||result.newTables!==3||result.newColumns!==31||result.readOnly!==true)throw Error('DOC120_SCHEMA_INCOMPATIBLE');
 return result;
}
function artifactHash(){return crypto.createHash('sha256').update(fs.readFileSync(path.join(PACKAGE,'GestionDocumental-Docuarchi.net.dll'))).digest('hex');}
function compile(directory){
 const exe=path.join(directory,'Doc120SchemaReadiness.exe');
 fs.copyFileSync(path.join(PACKAGE,'Newtonsoft.Json.dll'),path.join(directory,'Newtonsoft.Json.dll'));
 try{cp.execFileSync(path.join(process.env.WINDIR||'C:/Windows','Microsoft.NET/Framework64/v4.0.30319/csc.exe'),['/nologo','/target:exe','/out:'+exe,'/r:System.Core.dll','/r:System.Data.dll','/r:'+path.join(PACKAGE,'Newtonsoft.Json.dll'),path.join(__dirname,'support/Doc120SchemaReadiness.cs')],{windowsHide:true,stdio:'pipe'});}catch{throw Error('DOC120_SCHEMA_COMPILE_FAILED');}
 return exe;
}
function validateCompiled(exe,packet){
 try{return JSON.parse(cp.execFileSync(exe,[PACKAGE],{input:JSON.stringify(packet),encoding:'utf8',windowsHide:true,maxBuffer:1024*1024,timeout:30000}));}
 catch{return{ok:false};}
}
function errorCode(error){
 const message=error?.message;
 if(['ROWS_INVALID','INCOMPATIBLE','COMPILE_FAILED','BINARY_CHANGED','BUDGET_EXHAUSTED','INPUT_INVALID'].some(s=>message==='DOC120_SCHEMA_'+s))return message;
 const databaseCodes=['AUTHENTICATION_FAILED','DSN_UNAVAILABLE','DRIVER_UNAVAILABLE','DATABASE_ACCESS_DENIED','DATABASE_UNAVAILABLE','CONNECTION_UNAVAILABLE','OPEN_FAILED','QUERY_FAILED','COLUMN_UNAVAILABLE','TABLE_UNAVAILABLE','QUERY_UNSUPPORTED','RESULT_FAILED','INPUT_FAILED','FAILED','REQUIRED'];
 const suffix=databaseCodes.find(s=>message==='DOC111_SELECT_'+s);
 return suffix?'DOC120_SCHEMA_DB_'+suffix:'DOC120_SCHEMA_STOPPED';
}
async function main(args=process.argv.slice(2)){
 if(args.length!==3||args[0]!=='--run'||!/^[a-f0-9]{32}$/.test(args[1])||args[2]!=='--authorized-readonly')throw Error('DOC120_SCHEMA_AUTHORIZATION_REQUIRED');
 requireInteractiveConsole();const runId=args[1],directory=path.join(ROOT,'.tmp/doc120-schema-readiness',runId);
 if(fs.existsSync(directory))throw Error('DOC120_SCHEMA_RUN_ALREADY_EXISTS');fs.mkdirSync(directory,{recursive:true});
 const state={runId,status:'WAITING_DB_INPUT',select:0,completedSelect:0,maxSelect:MAX_SELECT,http:0,sessions:0,businessEffects:0,closed:false};
 const save=()=>fs.writeFileSync(path.join(directory,'state.json'),JSON.stringify(state,null,2));const values={};let environment;
 save();
 try{
  const exe=compile(directory);state.assemblySha256=artifactHash();save();process.title='DOC-120 - verificar esquema - solo lectura';
  console.log('Verificacion autorizada del esquema RA de pruebas: maximo10 SELECT de metadatos,0 HTTP,0 importaciones. No aplica SQL ni repite operaciones.');
  console.log('Las credenciales solo se usan en esta consola y se eliminan al terminar.');
  await collectValue(values,'dsn','DSN RA de lectura',{defaultValue:'MembershipUsers'});
  if(!/^[A-Za-z0-9 _.-]{1,128}$/.test(values.dsn))throw Error('DOC120_SCHEMA_INPUT_INVALID');
  await collectValue(values,'user','Cuenta MySQL exclusivamente SELECT',{secret:true});await collectValue(values,'password','Clave MySQL SELECT',{secret:true});
  environment=Object.fromEntries(Object.entries(process.env).filter(([k])=>!/^DOC\d+(?:_DA)?_E2E/.test(k)&&!['DEBUG','PWDEBUG','NODE_DEBUG','NODE_DEBUG_NATIVE'].includes(k)));
  Object.assign(environment,{DOC120_E2E_ODBC_DSN:values.dsn,DOC120_E2E_MYSQL_USER:values.user,DOC120_E2E_MYSQL_PASSWORD:values.password});
  state.status='QUERYING';save();
  state.result=await inspect(async q=>{
   if(state.select>=MAX_SELECT)throw Error('DOC120_SCHEMA_BUDGET_EXHAUSTED');state.query=q.scope+':'+q.name;state.select++;save();
   const rows=await queryRows(q.sql,[],environment,'DOC120_E2E');state.completedSelect++;save();return rows;
  },packet=>validateCompiled(exe,packet));
  if(artifactHash()!==state.assemblySha256)throw Error('DOC120_SCHEMA_BINARY_CHANGED');
  state.status='PASS_SCHEMA_COMPATIBLE';
 }catch(error){state.status='STOPPED';state.failure=errorCode(error);process.exitCode=2;}
 finally{
  for(const k of Object.keys(values))delete values[k];if(environment)for(const k of Object.keys(environment))if(/^DOC120_E2E_/.test(k))delete environment[k];
  Object.assign(state,{closed:true,credentialsCleared:true,finishedUtc:new Date().toISOString()});save();console.log('DOC120_SCHEMA_RESULT='+JSON.stringify(state));
 }
}
if(require.main===module)main().catch(()=>{console.error('DOC120_SCHEMA_PRECHECK_STOPPED');process.exitCode=2;});
module.exports={queries,inspect,errorCode,main,compile,validateCompiled,MAX_SELECT};
