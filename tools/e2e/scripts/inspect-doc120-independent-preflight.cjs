'use strict';
// Existing SELECT bridge, compiled production readers and pure validators.
// Business JSON travels only in memory/stdin, never artifacts or command arguments.
const fs=require('node:fs'),path=require('node:path'),cp=require('node:child_process'),crypto=require('node:crypto');
const tty=require('./support/interactive-e2e-console.cjs'),{queryRows}=require('./support/doc32-e2e-odbc.cjs');
const ROOT=path.resolve(__dirname,'../../..'),BIN=path.join(ROOT,'bin'),SCOPE=['50','141','2'],MAX_SELECT=7;
const check=(v,c)=>{if(!v)throw Error('DOC120_PURE_'+c);};
const scoped=' WHERE l.lote_id=? AND l.solicitante_ref=? AND l.ambito_ref=?';
function queries(){
 const lot='SELECT l.*,(SELECT MAX(e.evento_id) FROM ra_integracion_evento e WHERE e.lote_id=l.lote_id) AS latest_event FROM ra_integracion_lote l'+scoped+' LIMIT 2';
 return [
  {name:'lot',sql:lot,max:1},
  {name:'plan',sql:'SELECT p.* FROM ra_integracion_lote l INNER JOIN ra_integracion_plan p ON p.plan_id=l.plan_id AND p.ambito_ref=l.ambito_ref AND p.usuario_ref=l.solicitante_ref AND p.nivel_ref=l.nivel_ref'+scoped+' LIMIT 2',max:1},
  {name:'fragments',sql:'SELECT f.tipo,f.numero,f.contenido_doc,f.contenido_hash FROM ra_integracion_lote l INNER JOIN ra_integracion_preparacion p ON p.plan_id=l.plan_id AND p.ambito_ref=l.ambito_ref AND p.usuario_ref=l.solicitante_ref INNER JOIN ra_integracion_preparacion_resultado f ON f.preparacion_id=p.preparacion_id'+scoped+" AND p.estado='SELLADA' AND f.tipo IN ('RAICES','ARCHIVOS','ELEMENTOS','INCIDENCIAS','ELEGIBLES','CONFIRMABLE') ORDER BY f.tipo,f.numero LIMIT 10002",max:10001},
  {name:'roots',sql:'SELECT r.* FROM ra_integracion_lote l INNER JOIN ra_integracion_raiz r ON r.lote_id=l.lote_id'+scoped+' ORDER BY r.raiz_id LIMIT 3',max:2},
  {name:'files',sql:'SELECT f.*,r.cliente_raiz_id AS cliente_raiz,r.datos_doc AS datos_raiz FROM ra_integracion_lote l INNER JOIN ra_integracion_raiz r ON r.lote_id=l.lote_id INNER JOIN ra_integracion_archivo f ON f.raiz_id=r.raiz_id'+scoped+' ORDER BY f.archivo_id LIMIT 1002',max:1001},
  {name:'attempts',sql:'SELECT i.* FROM ra_integracion_lote l INNER JOIN ra_integracion_intento i ON i.lote_id=l.lote_id'+scoped+" AND (i.estado IN ('RESERVADO','EN_CURSO','INCIERTO') OR (i.fase<>'RECEPCION' AND i.estado='CONFIRMADO')) ORDER BY i.intento_id LIMIT 3003",max:3002},
  {name:'after',sql:lot,max:1}
 ];
}
async function inspect(read,validate){
 const packet={};for(const q of queries()){const rows=await read(q);check(Array.isArray(rows)&&rows.length<=q.max,'ROWS_INVALID');packet[q.name]=rows;}
 check(packet.lot.length===1&&JSON.stringify(packet.lot)===JSON.stringify(packet.after),'READ_CHANGED');
 const l=packet.lot[0];check(String(l.lote_id)==='50'&&String(l.solicitante_ref)==='141'&&String(l.ambito_ref)==='2'&&String(l.nivel_ref)==='223'&&String(l.detencion_solicitada)==='0'&&String(l.total_archivos)==='1000'&&String(l.total_raices)==='1','SCOPE_CHANGED');
 check(packet.plan.length===1&&packet.roots.length===1&&packet.files.length===1000&&!packet.attempts.some(i=>['RESERVADO','EN_CURSO','INCIERTO'].includes(i.estado)),'SCOPE_CHANGED');
 delete packet.after;
 const result=await validate(packet);
 check(result&&result.readOnly===true&&result.writerInvoked===false&&typeof result.ok==='boolean','HOST_RESULT_INVALID');
 const allowed=['ok','stage','files','roots','attempts','proofs','controlCharacters','readOnly','writerInvoked','sqlWritesTested','exceptionType','methods','code','selectReserved'];
 check(Object.keys(result).every(k=>allowed.includes(k)),'HOST_RESULT_INVALID');
 check(/^[A-Z_]{1,64}$/.test(result.stage),'HOST_RESULT_INVALID');
 if(result.exceptionType!=null)check(/^[A-Za-z0-9]{1,80}Exception$/.test(result.exceptionType),'HOST_RESULT_INVALID');
 if(result.code!=null)check(/^[A-Z_]{1,64}$/.test(result.code),'HOST_RESULT_INVALID');
 if(result.methods!=null)check(Array.isArray(result.methods)&&result.methods.length<=8&&result.methods.every(m=>typeof m==='string'&&/^[A-Za-z0-9_.$-]{1,200}$/.test(m)),'HOST_RESULT_INVALID');
 return result;
}
function compile(folder){
 const exe=path.join(folder,'Doc120IndependentPreflight.exe');
 try{cp.execFileSync(path.join(process.env.WINDIR||'C:/Windows','Microsoft.NET/Framework64/v4.0.30319/csc.exe'),['/nologo','/target:exe','/platform:x64','/out:'+exe,'/r:System.Core.dll','/r:System.Data.dll','/r:'+path.join(BIN,'MySql.Data.dll'),'/r:'+path.join(BIN,'Newtonsoft.Json.dll'),'/r:'+path.join(BIN,'GestionDocumental-Docuarchi.net.dll'),path.join(__dirname,'support/Doc120IndependentPreflight.cs')],{windowsHide:true,stdio:'pipe'});}catch{throw Error('DOC120_PURE_COMPILE_FAILED');}return exe;
}
function validateCompiled(exe,packet,native=false){
 const out=cp.spawnSync(exe,[BIN,...(native?['--native-select']:[])],{input:JSON.stringify(packet),encoding:'utf8',windowsHide:true,maxBuffer:65536,timeout:120000});
 check(!out.error&&[0,2].includes(out.status),'HOST_FAILED');try{return JSON.parse(out.stdout);}catch{throw Error('DOC120_PURE_HOST_FAILED');}
}
function hash(){return crypto.createHash('sha256').update(fs.readFileSync(path.join(BIN,'GestionDocumental-Docuarchi.net.dll'))).digest('hex');}
async function main(args=process.argv.slice(2)){
 check(args.length===3&&args[0]==='--run'&&/^[0-9a-f]{32}$/.test(args[1])&&['--authorized-readonly','--authorized-native-select'].includes(args[2]),'AUTHORIZATION_REQUIRED');tty.requireInteractiveConsole();const native=args[2]==='--authorized-native-select';
 const folder=path.join(ROOT,'.tmp/doc120-independent-preflight',args[1]);check(!fs.existsSync(folder),'RUN_EXISTS');fs.mkdirSync(folder,{recursive:true});
 const state={runId:args[1],lotId:'50',status:'PREPARING',select:0,maxSelect:native?12:MAX_SELECT,http:0,sessions:0,businessEffects:0,closed:false,native},values={};let environment;
 const save=()=>fs.writeFileSync(path.join(folder,'state.json'),JSON.stringify(state,null,2)+'\n');save();
 try{
  const exe=compile(folder);state.binarySha256=hash();save();
  console.log(native?'Diagnostico nativo del lote50: maximo12 SELECT RA reservados por metodo;0 HTTP y0 cambios. Verifica metadatos, indice y lectores con el mismo proveedor MySQL de la aplicacion. Sin transacciones, reservas ni escritor.':'Diagnostico del lote50: maximo7 SELECT RA;0 HTTP y0 cambios. Ejecuta lectores/validadores reales en memoria; no importa ni invoca escritor. Los datos del plan solo viajan en memoria y no se guardan.');
  await tty.collectValue(values,'dsn','DSN RA de lectura',{defaultValue:'MembershipUsers'});check(/^[A-Za-z0-9 _.-]{1,128}$/.test(values.dsn),'INPUT_INVALID');
  await tty.collectValue(values,'user','Cuenta MySQL exclusivamente SELECT',{secret:true});await tty.collectValue(values,'password','Clave MySQL SELECT',{secret:true});
  if(native){check(values.dsn==='MembershipUsers'&&hash()===state.binarySha256,'SCOPE_CHANGED');state.status='NATIVE_READING';state.select=null;state.selectBudgetReserved=12;save();state.result=validateCompiled(exe,values,true);check(state.result?.readOnly===true&&state.result.writerInvoked===false&&Number.isSafeInteger(state.result.selectReserved)&&state.result.selectReserved<=12,'HOST_RESULT_INVALID');state.selectUpperBound=state.result.selectReserved;check(hash()===state.binarySha256,'LIMIT_OR_BINARY_CHANGED');state.status=state.result.ok?'NATIVE_READS_PASSED':'NATIVE_READ_FAILURE_OBSERVED';return;}
  environment=Object.fromEntries(Object.entries(process.env).filter(([k])=>!/^DOC\d+(?:_DA)?_E2E/.test(k)&&!['DEBUG','PWDEBUG','NODE_DEBUG','NODE_DEBUG_NATIVE'].includes(k)));
  Object.assign(environment,{DOC120_E2E_ODBC_DSN:values.dsn,DOC120_E2E_MYSQL_USER:values.user,DOC120_E2E_MYSQL_PASSWORD:values.password});
  state.status='READING';save();
  state.result=await inspect(async q=>{check(state.select<MAX_SELECT&&hash()===state.binarySha256,'LIMIT_OR_BINARY_CHANGED');state.select++;state.stage=q.name;save();console.log('DOC120_PURE_PROGRESS='+q.name);return queryRows(q.sql,SCOPE,environment,'DOC120_E2E');},packet=>validateCompiled(exe,packet));
  check(hash()===state.binarySha256,'LIMIT_OR_BINARY_CHANGED');state.status=state.result.ok?'PURE_PREFLIGHT_PASSED':'PURE_FAILURE_REPRODUCED';
 }catch(e){state.status='STOPPED';state.failure=/^DOC120_PURE_[A-Z_]+$|^DOC111_SELECT_[A-Z_]+$/.test(e?.message||'')?e.message:'DOC120_PURE_STOPPED';process.exitCode=2;}
 finally{for(const k of Object.keys(values))delete values[k];if(environment)for(const k of Object.keys(environment))if(/^DOC120_E2E_/.test(k))delete environment[k];state.closed=true;state.credentialsCleared=true;state.finishedUtc=new Date().toISOString();save();console.log('DOC120_PURE_RESULT='+JSON.stringify(state));console.log('Diagnostico terminado. No se repetiran operaciones.');}
}
if(require.main===module)main().catch(()=>{console.error('DOC120_PURE_PRECHECK_STOPPED');process.exitCode=2;});
module.exports={queries,inspect,compile,validateCompiled,MAX_SELECT,main};
