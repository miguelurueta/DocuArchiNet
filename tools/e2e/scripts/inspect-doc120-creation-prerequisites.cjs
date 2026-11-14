'use strict';
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto'),cp=require('node:child_process'),readline=require('node:readline');
const {collectValue,requireInteractiveConsole}=require('./support/interactive-e2e-console.cjs');
const {queryRows}=require('./support/doc32-e2e-odbc.cjs');
const ROOT=path.resolve(__dirname,'../../..'),BIN=path.join(ROOT,'bin'),MAX_SELECT=30;
const metadataSql="SELECT l.lote_id,l.plan_id,l.estado AS lote_estado,l.revision AS lote_revision,l.detencion_solicitada,l.contexto_doc,l.config_doc,r.raiz_id,r.cliente_raiz_id,r.datos_doc,r.estado AS raiz_estado,r.revision AS raiz_revision,r.intento_activo_id,r.identidad_expediente_doc,r.valores_efectivos_doc,i.intento_id,i.fase,i.estado AS intento_estado,i.token_reclamacion,LOWER(HEX(i.solicitud_hash)) AS solicitud_hash,i.fin_utc,i.evidencia_doc FROM ra_integracion_lote l INNER JOIN ra_integracion_raiz r ON r.lote_id=l.lote_id INNER JOIN ra_integracion_intento i ON i.lote_id=l.lote_id AND i.raiz_id=r.raiz_id AND i.intento_id=r.intento_activo_id WHERE l.lote_id=? AND r.raiz_id=? AND i.intento_id=? AND i.archivo_id IS NULL AND i.fase='CREAR_EXPEDIENTE'";
const tables={RA:new Set(['ra_pro_niveles','remit_dest_interno','ra_pro_permisos_niveles','registro_organigrama','ra_registro_instrumento_archivistico','areas_depart_radicacion','series_documentales','subseries_documentales','ra_tipo_expediente','tipos_unidad_documental','ra_de_fondo_documental','ra_de_tipos_ciclos_archivo']),DA:new Set(['system1','usuarios_da','permisos_usuarios_gabinetes','permisos_grupos_gabinetes','relacion_usu_grup'])};
const safeCodes=new Set(['ACCESO_DENEGADO','CONFLICTO_REVISION','CONFIGURACION_INVALIDA','CAPACIDAD_NO_DISPONIBLE','SOLICITUD_INVALIDA','RESULTADO_INCIERTO','CATALOG_COMPATIBLE','COMMAND_COMPATIBLE','UNCLASSIFIED']);
function commandDetail(detail){
 const guards=['SNAPSHOT_SHAPE','PROFILE_VALIDATION','CONFIGURATION_NORMALIZED','SNAPSHOT_FINGERPRINT','ROOT_FIELD_COUNT','ROOT_FIELD_SHAPE','ROOT_FIELD_DUPLICATE','ROOT_FIELD_RULE','RESPONSIBLE_REFERENCE','DATE_RANGE','UNCLASSIFIED'];
 if(!detail||!guards.includes(detail.guard))throw fail('PROTOCOL_INVALID');
 const fields=['CODIGO','NOMBRE','FECHA_INICIAL','FECHA_FINAL','RESPONSABLE','configuracion','Esquema','NombreVisible','PoliticaCoincidencias','AmbitoIntegridad','ModoIntegridad','EstrategiaCarpetas','PoliticaSubcarpetas','PoliticaCarpetasVacias','PoliticaArchivosSueltos','FondoRef','InstrumentoRef','AreaRef','SerieRef','SubserieRef','TipoExpedienteRef','UnidadDocumentalRef','GabineteId','MaximoBytesArchivo','Extensiones','TIPOLOGIA'];
 const issues=['CONFIGURACION_REQUERIDA','ESQUEMA_INVALIDO','NOMBRE_INVALIDO','POLITICA_INVALIDA','AMBITO_INVALIDO','AMBITO_REQUERIDO','MODO_INVALIDO','REFERENCIA_INVALIDA','REFERENCIA_REQUERIDA','GABINETE_REQUERIDO','LIMITE_INVALIDO','EXTENSIONES_INVALIDAS','EXTENSION_INVALIDA','FUENTE_REQUERIDA','OPCION_REQUERIDA','OPCION_INVALIDA','REGLA_INVALIDA','LIMITE_REGLAS','DESTINO_INVALIDO','DESTINO_DUPLICADO','FUENTE_INVALIDA','PROVEEDOR_INVALIDO','TIPO_INVALIDO','DEPENDENCIA_INVALIDA','TIPO_DEPENDENCIA_INVALIDO','ETAPA_INVALIDA','LONGITUD_INVALIDA','FORMATO_FECHA_INVALIDO','VALOR_REQUERIDO','VALOR_VACIO','LONGITUD_EXCEDIDA','CARACTER_NO_REPRESENTABLE','CODIGO_EXCEDE_45','FECHA_INVALIDA','VALOR_NO_CANONICO','ENTERO_INVALIDO','DECIMAL_INVALIDO','FECHA_CONSTANTE_NO_CANONICA','RANGO_FECHAS_INVALIDO'];
 const output={guard:detail.guard,field:detail.field==null?null:fields.includes(detail.field)?detail.field:'OTHER',issue:detail.issue==null?null:issues.includes(detail.issue)?detail.issue:'OTHER'};
 for(const key of ['sourceMatches','typeMatches','valueValid'])if(typeof detail[key]==='boolean')output[key]=detail[key];
 for(const key of ['incidenceCount','actualCount','expectedCount','valueLength'])if(Number.isSafeInteger(detail[key])&&detail[key]>=0&&detail[key]<=1000000)output[key]=detail[key];return output;
}
function fail(code){return Error('DOC120_PREREQUISITES_'+code);}
function fingerprint(exe){return [path.join(BIN,'GestionDocumental-Docuarchi.net.dll'),path.join(BIN,'Newtonsoft.Json.dll'),path.join(__dirname,'support/Doc120CreationPrerequisites.cs'),exe].map(file=>({file:path.relative(ROOT,file).replace(/\\/g,'/'),sha256:crypto.createHash('sha256').update(fs.readFileSync(file)).digest('hex')}));}
function createReader(state,save,query,connections,environment){return async q=>{
 if(!['RA','DA'].includes(q.origin))throw fail('SELECT_DENIED');if(state.select>=state.maxSelect)throw fail('BUDGET_EXHAUSTED');
 state.status='QUERYING';state.select++;const record={index:state.select,origin:q.origin,kind:q.kind,tables:q.tables||['ra_integracion_lote','ra_integracion_raiz','ra_integracion_intento'],completed:false};state.queries.push(record);save();
 const current={...environment,DOC120_E2E_ODBC_DSN:q.origin==='RA'?connections.ra:connections.da};
 try{const rows=await query(q.sql,q.parameters,current,'DOC120_E2E');state.completedSelect++;record.completed=true;record.rowCount=rows.length;save();return rows;}
 finally{for(const key of Object.keys(current))if(/^DOC120_E2E_/.test(key))delete current[key];}
};}
function validId(id){return typeof id==='string'&&/^[1-9][0-9]{0,9}$/.test(id)&&Number(id)<=4294967295;}
function payload(rows,scope){
 if(!Array.isArray(rows)||rows.length!==1)throw fail('SCOPE_CHANGED');const r=rows[0];
 if(String(r.lote_id)!==scope.lot||String(r.raiz_id)!==scope.root||String(r.intento_id)!==scope.attempt||String(r.intento_activo_id)!==scope.attempt||r.lote_estado!=='EN_CURSO'||r.raiz_estado!=='CREANDO'||r.intento_estado!=='RESERVADO'||r.fase!=='CREAR_EXPEDIENTE'||r.fin_utc!=null||r.evidencia_doc!=null||r.identidad_expediente_doc!=null)throw fail('SCOPE_CHANGED');
 let context,snapshot,data,values;try{context=JSON.parse(r.contexto_doc);snapshot=JSON.parse(r.config_doc);data=JSON.parse(r.datos_doc);values=r.valores_efectivos_doc==null?[]:JSON.parse(r.valores_efectivos_doc);}catch{throw fail('DOCUMENT_INVALID');}
 if(data?.Esquema!==1||data.IdCliente!==r.cliente_raiz_id||!Array.isArray(data.ValoresPlan)||!Array.isArray(values))throw fail('DOCUMENT_INVALID');
 return {lot:{LoteId:scope.lot,PlanId:String(r.plan_id),Estado:r.lote_estado,Revision:Number(r.lote_revision),DetencionSolicitada:r.detencion_solicitada==='1',Contexto:context,Snapshot:snapshot},root:{LoteId:scope.lot,RaizId:scope.root,ClienteRaizId:r.cliente_raiz_id,NombreOriginal:data.NombreOriginal,Estado:r.raiz_estado,Revision:Number(r.raiz_revision),IntentoActivoId:scope.attempt,ValoresPlan:data.ValoresPlan,ValoresEfectivos:values},attempt:{IntentoId:scope.attempt,LoteId:scope.lot,RaizId:scope.root,Fase:r.fase,Estado:r.intento_estado,TokenReclamacion:r.token_reclamacion,SolicitudHash:r.solicitud_hash}};
}
function statement(request){
 if(request?.type!=='SELECT'||!tables[request.origin]||typeof request.sql!=='string'||request.sql.length>12000||!/^SELECT /i.test(request.sql)||/;|--|\/\*|\b(?:INSERT|UPDATE|DELETE|CALL|EXEC|DROP|ALTER|CREATE|REPLACE|TRUNCATE|GRANT|REVOKE|SET|USE|LOAD|OUTFILE|INTO|LOCK|SLEEP|BENCHMARK)\b/i.test(request.sql))throw fail('SELECT_DENIED');
 const names=[...request.sql.matchAll(/\b(?:FROM|JOIN)\s+([A-Za-z_][A-Za-z0-9_]*)/gi)].map(m=>m[1]);
 if(names.some(name=>!tables[request.origin].has(name))||(!names.length&&request.sql!=='SELECT @@hostname AS host,@@port AS puerto,@@server_id AS servidor,DATABASE() AS base'))throw fail('SELECT_DENIED');
 if(!Array.isArray(request.parameters)||request.parameters.length%2||request.parameters.length>64)throw fail('PARAMETERS_INVALID');
 const parameters=new Map();for(let i=0;i<request.parameters.length;i+=2){const key=request.parameters[i],value=request.parameters[i+1];if(typeof key!=='string'||!/^@[A-Za-z][A-Za-z0-9_]*$/.test(key)||parameters.has(key)||value!==null&&!(typeof value==='string'&&value.length<=2048)&&!Number.isSafeInteger(value))throw fail('PARAMETERS_INVALID');parameters.set(key,value);}
 const positional=[];const sql=request.sql.replace(/(?<!@)@[A-Za-z][A-Za-z0-9_]*/g,key=>{if(!parameters.has(key))throw fail('PARAMETERS_INVALID');positional.push(parameters.get(key));return '?';});
 let columns=['id','nombre'],kind='CATALOG';
 if(request.sql.startsWith('SELECT @@hostname')){columns=['host','puerto','servidor','base'];kind='ORIGIN';}
 else if(request.sql.startsWith('SELECT n.id_nivel')){columns=['id_nivel'];kind='LEVEL_AUTHORITY';}
 else if(request.sql.startsWith('SELECT RELACION_DA')){columns=['RELACION_DA'];kind='RA_IDENTITY';}
 else if(request.sql.startsWith('SELECT Clave_Usuario')){columns=['Clave_Usuario'];kind='DA_IDENTITY';}
 else if(request.sql.startsWith('SELECT s.id_gabinete')){columns=['id_gabinete','INVENTARIO_DOCUMENTAL','APLICA_TRD','ASIGNA_UNIDAD'];kind='CABINET_AUTHORITY';}
 else if(request.sql.includes('AS identificacion')){columns=['id','nombre','identificacion'];kind='RESPONSIBLE';}
 return{sql,parameters:positional,columns,kind,origin:request.origin,tables:[...new Set(names)]};
}
function result(message){
 if(message?.type!=='RESULT'||!['PASS','STOPPED'].includes(message.status)||!['INPUT','COMMAND','LEGACY_INPUT','CATALOG','LEGACY_CATALOG_VALUES'].includes(message.stage)||!safeCodes.has(message.code)||message.counterfactual!==true||message.sessionAuthorityChecked!==false||message.sessionConnectionCompatibilityChecked!==false||message.startTransactionChecked!==false||message.recoveryAuthorized!==false||message.status==='PASS'&&!['CATALOG_COMPATIBLE','COMMAND_COMPATIBLE'].includes(message.code))throw fail('PROTOCOL_INVALID');
 return{status:message.status,stage:message.stage,code:message.code,detail:message.detail&&message.stage==='COMMAND'&&message.code==='CONFIGURACION_INVALIDA'?commandDetail(message.detail):null,counterfactual:true,sessionAuthorityChecked:false,sessionConnectionCompatibilityChecked:false,startTransactionChecked:false,recoveryAuthorized:false};
}
function compile(){
 const directory=path.join(ROOT,'.tmp/doc120-prerequisite-host');fs.mkdirSync(directory,{recursive:true});const exe=path.join(directory,'Doc120CreationPrerequisites.exe');
 const csc=path.join(process.env.WINDIR||'C:/Windows','Microsoft.NET/Framework64/v4.0.30319/csc.exe');
 try{cp.execFileSync(csc,['/nologo','/target:exe','/out:'+exe,'/r:System.Data.dll','/r:System.Core.dll','/r:'+path.join(BIN,'Newtonsoft.Json.dll'),'/r:'+path.join(BIN,'GestionDocumental-Docuarchi.net.dll'),path.join(__dirname,'support/Doc120CreationPrerequisites.cs')],{windowsHide:true,stdio:'pipe'});}catch{throw fail('COMPILE_FAILED');}return exe;
}
async function inspect(data,read,exe=compile()){
 const child=cp.spawn(exe,[BIN],{windowsHide:true,stdio:['pipe','pipe','ignore'],env:Object.fromEntries(Object.entries(process.env).filter(([key])=>!/^DOC\d+(?:_DA)?_E2E/.test(key)&&!['DEBUG','NODE_DEBUG','NODE_DEBUG_NATIVE'].includes(key)))});
 let processError=false;child.on('error',()=>{processError=true;});child.stdin.on('error',()=>{});
 const lines=readline.createInterface({input:child.stdout,crlfDelay:Infinity});let bytes=0;const timer=setTimeout(()=>child.kill(),180000);
 try{child.stdin.write(JSON.stringify(data)+'\n');for await(const line of lines){bytes+=Buffer.byteLength(line);if(bytes>8*1024*1024)throw fail('PROTOCOL_INVALID');let message;try{message=JSON.parse(line);}catch{throw fail('PROTOCOL_INVALID');}if(message.type==='RESULT')return result(message);const q=statement(message),rows=await read(q);if(!Array.isArray(rows))throw fail('ROWS_INVALID');child.stdin.write(JSON.stringify({type:'ROWS',columns:q.columns,rows})+'\n');}throw fail(processError?'HOST_UNAVAILABLE':'HOST_STOPPED');}
 finally{clearTimeout(timer);lines.close();child.stdin.end();if(child.exitCode===null)child.kill();}
}
function errorCode(error){const text=error?.message||'';if(/^DOC120_PREREQUISITES_(?:SCOPE_CHANGED|DOCUMENT_INVALID|SELECT_DENIED|PARAMETERS_INVALID|PROTOCOL_INVALID|COMPILE_FAILED|HOST_UNAVAILABLE|HOST_STOPPED|ROWS_INVALID|BUDGET_EXHAUSTED|INPUT_INVALID|ASSEMBLY_CHANGED)$/.test(text))return text;const suffix=['AUTHENTICATION_FAILED','DSN_UNAVAILABLE','DRIVER_UNAVAILABLE','DATABASE_ACCESS_DENIED','DATABASE_UNAVAILABLE','CONNECTION_UNAVAILABLE','OPEN_FAILED','QUERY_FAILED','COLUMN_UNAVAILABLE','TABLE_UNAVAILABLE','QUERY_UNSUPPORTED','RESULT_FAILED','INPUT_FAILED','FAILED','REQUIRED'].find(code=>text==='DOC111_SELECT_'+code);return suffix?'DOC120_PREREQUISITES_DB_'+suffix:'DOC120_PREREQUISITES_STOPPED';}
async function main(args=process.argv.slice(2)){
 const commandOnly=args.length===8&&args[6]==='--command-only'&&args[7]==='--authorized';
 if(!(commandOnly||args.length===7&&args[6]==='--authorized')||args[0]!=='--lot'||args[2]!=='--root'||args[4]!=='--attempt'||![args[1],args[3],args[5]].every(validId)||args[1]!=='39'||args[3]!=='60'||args[5]!=='96')throw fail('INPUT_INVALID');requireInteractiveConsole();const scope={lot:args[1],root:args[3],attempt:args[5]},exe=compile();
 const runId=crypto.randomBytes(16).toString('hex'),directory=path.join(ROOT,'.tmp/doc120-prerequisites',runId);fs.mkdirSync(directory,{recursive:true});
 const state={runId,scope,status:'WAITING_DB_INPUT',select:0,completedSelect:0,maxSelect:commandOnly?1:MAX_SELECT,priorAttempts:commandOnly?7:6,http:0,sessions:0,queries:[],closed:false,counterfactual:true,commandOnly,recovered:false,binaryBefore:fingerprint(exe)};const save=()=>fs.writeFileSync(path.join(directory,'state.json'),JSON.stringify(state,null,2));save();const values={};let environment;
 try{process.title='DOC-120 - prerrequisitos - lote '+scope.lot;console.log('Diagnostico de prerrequisitos AUTORIZADO: lote '+scope.lot+', carpeta '+scope.root+', intento '+scope.attempt+'. Hasta'+state.maxSelect+' SELECT;0 HTTP/efectos. Conserva'+state.priorAttempts+' intentos anteriores.');console.log('Solo valida una copia previa a la detencion. No inicia, reconcilia ni libera el intento. No comprueba la sesion HTTP, su conexion legacy ni la transaccion real de inicio.');
  await collectValue(values,'ra','DSN RA',{defaultValue:'MembershipUsers'});if(commandOnly)values.da=values.ra;else await collectValue(values,'da','DSN DA',{defaultValue:'MembershipUsers'});if(![values.ra,values.da].every(d=>/^[A-Za-z0-9 _.-]{1,128}$/.test(d)))throw fail('INPUT_INVALID');await collectValue(values,'user','Cuenta MySQL exclusivamente SELECT',{secret:true});await collectValue(values,'password','Clave MySQL SELECT',{secret:true});
  environment=Object.fromEntries(Object.entries(process.env).filter(([key])=>!/^DOC\d+(?:_DA)?_E2E/.test(key)&&!['DEBUG','PWDEBUG','NODE_DEBUG','NODE_DEBUG_NATIVE'].includes(key)));Object.assign(environment,{DOC120_E2E_MYSQL_USER:values.user,DOC120_E2E_MYSQL_PASSWORD:values.password});
  const read=createReader(state,save,queryRows,values,environment);
  const data=payload(await read({origin:'RA',kind:'METADATA',sql:metadataSql,parameters:[scope.lot,scope.root,scope.attempt]}),scope);data.commandOnly=commandOnly;
  state.result=await inspect(data,read,exe);state.status=state.result.status==='PASS'?'FINISHED_WITH_LIMITS':'STOPPED';if(state.status==='STOPPED')process.exitCode=2;
 }catch(error){state.status='STOPPED';state.failure=errorCode(error);process.exitCode=2;}
 finally{for(const key of Object.keys(values))delete values[key];if(environment)for(const key of Object.keys(environment))if(/^DOC120_E2E_/.test(key))delete environment[key];try{state.binaryStable=JSON.stringify(state.binaryBefore)===JSON.stringify(fingerprint(exe));}catch{state.binaryStable=false;}if(!state.binaryStable){state.status='STOPPED';state.failure='DOC120_PREREQUISITES_ASSEMBLY_CHANGED';process.exitCode=2;}Object.assign(state,{closed:true,credentialsCleared:true,finishedUtc:new Date().toISOString()});save();console.log('DOC120_PREREQUISITES_RESULT='+JSON.stringify(state));}
}
if(require.main===module)main().catch(()=>{console.error('DOC120_PREREQUISITES_PRECHECK_STOPPED');process.exitCode=2;});
module.exports={payload,statement,result,compile,inspect,errorCode,metadataSql,main,MAX_SELECT,createReader};
