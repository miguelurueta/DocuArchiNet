'use strict';
const fs=require('node:fs'),path=require('node:path');
const {collectValue,requireInteractiveConsole}=require('./support/interactive-e2e-console.cjs');
const {queryRows}=require('./support/doc32-e2e-odbc.cjs');
const {errorCode:schemaError}=require('./inspect-doc120-schema-readiness.cjs');
const ROOT=path.resolve(__dirname,'../../..'),SOURCE_COLUMNS=['CODIGOTIPO','TIPO','CONSECUTIVO','FECHA','NIT','BENEFICIARIO'];
const validId=v=>/^[1-9][0-9]{0,9}$/.test(String(v))&&Number(v)<=4294967295;
function requireValue(ok){if(!ok)throw Error('DOC120_LENGTH_SCOPE_CHANGED');}
function single(rows){requireValue(Array.isArray(rows)&&rows.length===1);return rows[0];}
function parse(text){try{return JSON.parse(text);}catch{throw Error('DOC120_LENGTH_DOCUMENT_INVALID');}}
function safeInteger(value){const n=Number(value);requireValue(Number.isSafeInteger(n)&&n>=0);return n;}
async function inspect(read,login){
 const identity=single(await read('RA',"SELECT u.id_Remit_Dest_Int AS owner_ref,u.Empresa_Gestion_Documental_id_empresa AS company_ref FROM remit_dest_interno u INNER JOIN empresa_gestion_documental e ON e.ID_EMPRESA=u.Empresa_Gestion_Documental_id_empresa WHERE u.Login_Usuario=? AND u.Estado_Usuario=1 LIMIT 2",[login]));
 requireValue(validId(identity.owner_ref)&&validId(identity.company_ref));
 const physical=single(await read('DA',"SELECT s.id_gabinete,s.NOMBRE AS tabla,d.CAMPO AS columna,d.ALEAS_CAMPO AS alias_campo,ic.DATA_TYPE,ic.COLUMN_TYPE,ic.CHARACTER_MAXIMUM_LENGTH,ic.CHARACTER_SET_NAME,ic.COLLATION_NAME,ic.IS_NULLABLE,ic.COLUMN_DEFAULT,ic.EXTRA FROM system1 s INNER JOIN detalle_gabienete d ON d.id_gabinete=s.id_gabinete AND d.GABINETE=s.NOMBRE INNER JOIN information_schema.COLUMNS ic ON ic.TABLE_SCHEMA=DATABASE() AND ic.TABLE_NAME=s.NOMBRE AND ic.COLUMN_NAME=d.CAMPO WHERE d.id_detalle_gabinete=? AND d.VISIBLE=1 LIMIT 2",['17']));
 requireValue(validId(physical.id_gabinete)&&/^[A-Za-z_][A-Za-z0-9_]{0,63}$/.test(physical.tabla)&&/^[A-Za-z_][A-Za-z0-9_]{0,63}$/.test(physical.columna)&&['varchar','char','text','tinytext','mediumtext','longtext'].includes(physical.DATA_TYPE));
 const candidates=await read('RA',"SELECT p.perfil_id,p.revision AS perfil_revision,p.version_publicada_id,v.numero,v.revision AS version_revision,v.configuracion_doc FROM ra_integracion_perfil p INNER JOIN ra_integracion_perfil_version v ON v.version_id=p.version_publicada_id AND v.perfil_id=p.perfil_id WHERE p.ambito_ref=? AND p.nombre=? AND p.activo=1 AND v.estado='PUBLICADA' LIMIT 20",[String(identity.company_ref),'Egresos']);
 requireValue(Array.isArray(candidates)&&candidates.length<20);
 const profile=single(candidates.filter(p=>String(parse(p.configuracion_doc).GabineteId)===String(physical.id_gabinete)));
 const ruleRow=single(await read('RA',"SELECT r.regla_id,r.regla_doc FROM ra_integracion_perfil_regla r INNER JOIN ra_integracion_perfil_version v ON v.version_id=r.version_id INNER JOIN ra_integracion_perfil p ON p.perfil_id=v.perfil_id WHERE p.ambito_ref=? AND p.perfil_id=? AND p.version_publicada_id=v.version_id AND v.version_id=? AND r.destino_tipo='INDICE_GABINETE' AND r.destino_ref=? LIMIT 2",[String(identity.company_ref),String(profile.perfil_id),String(profile.version_publicada_id),'17']));
 const rule=parse(ruleRow.regla_doc);requireValue(rule.DestinoTipo==='INDICE_GABINETE'&&rule.DestinoRef==='17'&&rule.TipoValor==='TEXTO'&&rule.Proveedor&&validId(rule.Proveedor.ProveedorId)&&validId(rule.Proveedor.VersionId));
 const provider=single(await read('RA',"SELECT v.conexion_ref,v.tabla_nombre,v.propiedades_doc FROM ra_integracion_proveedor p INNER JOIN ra_integracion_proveedor_version v ON v.proveedor_id=p.proveedor_id WHERE p.ambito_ref=? AND p.proveedor_id=? AND v.version_id=? AND p.activo=1 AND v.estado='PUBLICADA' LIMIT 2",[String(identity.company_ref),String(rule.Proveedor.ProveedorId),String(rule.Proveedor.VersionId)]));
 requireValue(provider.conexion_ref==='DA'&&provider.tabla_nombre==='imp_02_egresos');
 const property=single(parse(provider.propiedades_doc).filter(p=>p.Nombre===rule.Proveedor.Propiedad));requireValue(SOURCE_COLUMNS.includes(property.Columna));
 const source=single(await read('DA',"SELECT DATA_TYPE,CHARACTER_MAXIMUM_LENGTH FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME=? AND COLUMN_NAME=?",['imp_02_egresos',property.Columna]));
 requireValue(['varchar','char','text','tinytext','mediumtext','longtext'].includes(source.DATA_TYPE));
 const column='`'+property.Columna+'`',current=safeInteger(rule.LongitudMaxima);
 const stats=single(await read('DA','SELECT COUNT(*) AS registros,COALESCE(MAX(CHAR_LENGTH('+column+')),0) AS longitud_maxima,COALESCE(MAX(CHAR_LENGTH(TRIM('+column+'))),0) AS longitud_maxima_sin_espacios,COALESCE(SUM(CASE WHEN CHAR_LENGTH('+column+')>? THEN 1 ELSE 0 END),0) AS exceden_regla FROM imp_02_egresos',[String(current)]));
 const maximum=safeInteger(stats.longitud_maxima),declared=safeInteger(source.CHARACTER_MAXIMUM_LENGTH),capacity=safeInteger(physical.CHARACTER_MAXIMUM_LENGTH);
 requireValue(current>0&&maximum<=declared);
 const details=await read('DA',"SELECT COLUMN_NAME,DATA_TYPE,CHARACTER_MAXIMUM_LENGTH FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='detalle_gabienete' ORDER BY ORDINAL_POSITION",[]);
 const indexes=await read('DA','SELECT INDEX_NAME,NON_UNIQUE,SEQ_IN_INDEX,COLUMN_NAME,SUB_PART FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME=? AND COLUMN_NAME=?',[physical.tabla,physical.columna]);
 return{ownerRef:String(identity.owner_ref),companyRef:String(identity.company_ref),profileId:String(profile.perfil_id),versionId:String(profile.version_publicada_id),profileRevision:safeInteger(profile.perfil_revision),versionRevision:safeInteger(profile.version_revision),versionNumber:safeInteger(profile.numero),indexId:'17',cabinetId:String(physical.id_gabinete),physical,source:{table:'imp_02_egresos',column:property.Columna,declaredCapacity:declared,maximum,maximumTrimmed:safeInteger(stats.longitud_maxima_sin_espacios),records:safeInteger(stats.registros),exceedingCurrentRule:safeInteger(stats.exceden_regla)},ruleLimit:current,physicalCapacity:capacity,requiredForExistingData:maximum,metadataColumns:details.map(r=>r.COLUMN_NAME),physicalIndexes:indexes,profileOnlyPossible:capacity>=maximum};
}
function errorCode(e){if(['SCOPE_CHANGED','DOCUMENT_INVALID','INPUT_INVALID','BUDGET_EXHAUSTED'].some(s=>e?.message==='DOC120_LENGTH_'+s))return e.message;return schemaError(e).replace('DOC120_SCHEMA_','DOC120_LENGTH_');}
async function main(args=process.argv.slice(2)){
 requireValue(args.length===3&&args[0]==='--run'&&/^[a-f0-9]{32}$/.test(args[1])&&args[2]==='--authorized-readonly');requireInteractiveConsole();
 const runId=args[1],directory=path.join(ROOT,'.tmp/doc120-egresos-length',runId);if(fs.existsSync(directory))throw Error('DOC120_LENGTH_RUN_ALREADY_EXISTS');fs.mkdirSync(directory,{recursive:true});
 const state={runId,status:'WAITING_INPUT',select:0,completedSelect:0,maxSelect:10,http:0,sessions:0,businessEffects:0,closed:false},values={};let environment;
 const save=()=>fs.writeFileSync(path.join(directory,'state.json'),JSON.stringify(state,null,2));save();
 try{
  process.title='DOC-120 - medir longitud Egresos - indice17';console.log('Medicion autorizada: Egresos, indice17, origen imp_02_egresos. Maximo10 SELECT;0 HTTP/cambios/importaciones. Solo guarda metadatos y cantidades, nunca los textos de beneficiarios.');
  await collectValue(values,'ra','DSN RA de lectura',{defaultValue:'MembershipUsers'});await collectValue(values,'da','DSN DA de lectura',{defaultValue:'MembershipUsers'});
  requireValue([values.ra,values.da].every(d=>/^[A-Za-z0-9 _.-]{1,128}$/.test(d)));
  await collectValue(values,'owner','Cuenta de aplicacion que usa Egresos (enmascarada)',{secret:true});
  await collectValue(values,'user','Cuenta MySQL exclusivamente SELECT',{secret:true});await collectValue(values,'password','Clave MySQL SELECT',{secret:true});
  environment=Object.fromEntries(Object.entries(process.env).filter(([k])=>!/^DOC\d+(?:_DA)?_E2E/.test(k)&&!['DEBUG','PWDEBUG','NODE_DEBUG','NODE_DEBUG_NATIVE'].includes(k)));
  Object.assign(environment,{DOC120_E2E_MYSQL_USER:values.user,DOC120_E2E_MYSQL_PASSWORD:values.password});state.status='QUERYING';save();
  state.result=await inspect(async(origin,sql,parameters)=>{
   if(state.select>=state.maxSelect)throw Error('DOC120_LENGTH_BUDGET_EXHAUSTED');state.select++;save();
   const current={...environment,DOC120_E2E_ODBC_DSN:origin==='RA'?values.ra:values.da};
   try{const rows=await queryRows(sql,parameters,current,'DOC120_E2E');state.completedSelect++;save();return rows;}
   finally{for(const k of Object.keys(current))if(/^DOC120_E2E_/.test(k))delete current[k];}
  },values.owner);
  state.status='MEASURED';console.log('Medicion completada: regla='+state.result.ruleLimit+', columna='+state.result.physicalCapacity+', maximo real='+state.result.source.maximum+', excedentes='+state.result.source.exceedingCurrentRule+'.');
 }catch(e){state.status='STOPPED';state.failure=errorCode(e);process.exitCode=2;}
 finally{for(const k of Object.keys(values))delete values[k];if(environment)for(const k of Object.keys(environment))if(/^DOC120_E2E_/.test(k))delete environment[k];Object.assign(state,{closed:true,credentialsCleared:true,finishedUtc:new Date().toISOString()});save();console.log('DOC120_EGRESOS_LENGTH_RESULT='+JSON.stringify(state));}
}
if(require.main===module)main().catch(()=>{console.error('DOC120_LENGTH_PRECHECK_STOPPED');process.exitCode=2;});
module.exports={inspect,errorCode,main,SOURCE_COLUMNS};
