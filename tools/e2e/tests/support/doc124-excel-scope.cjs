'use strict';
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
function validateScope(s,baseUrl){
 const positive=n=>Number.isSafeInteger(n)&&n>0,hash=h=>/^[0-9a-f]{64}$/.test(h||'');
 const archivo=s?.escenario==='archivo';
 if(s?.escenario!==undefined&&!['archivo','matriz'].includes(s.escenario))throw Error('DOC124_SCOPE_INVALID');
 if(!s||s.issue!=='DOC-124'||(s.disposable!==true&&s.destinationReadOnly!==true)||s.baseUrl!==baseUrl||!positive(s.empresaId)||!positive(s.usuarioId)||!positive(s.moduloId)||
  !/^DOC[0-9]+_DA_E2E$/.test(s.controlPrefix||'')||!Array.isArray(s.cases)||s.cases.length<(archivo?1:5)||s.cases.length>(archivo?1:12)||
  !Number.isSafeInteger(s.concurrencyCase)||s.concurrencyCase<0||s.concurrencyCase>=s.cases.length)throw Error('DOC124_SCOPE_INVALID');
 for(const c of s.cases)if(!c||!['CREAR','AGREGAR','REEMPLAZAR'].includes(c.modo)||!positive(c.conexionId)||!positive(c.plantillaId)||!positive(c.revisionEsperada)||!hash(c.huellaEsperada)||!hash(c.archivoHash)||
  !/^imp_02_[A-Za-z0-9_]{1,57}$/.test(c.tabla||'')||
  !path.isAbsolute(c.archivoPath||'')||!/^\.(?:xls|xlsx)$/i.test(path.extname(c.archivoPath))||typeof c.hoja!=='string'||!c.hoja||c.hoja.length>120||!positive(c.filasEsperadas)||c.filasEsperadas>20000||
  !['OK','CLAVES_INVALIDAS','LECTURA_INVALIDA'].includes(c.codigoEsperado)||!Array.isArray(c.incidenciasEsperadas)||!c.incidenciasEsperadas.every(x=>/^[A-Z_]+$/.test(x)))throw Error('DOC124_SCOPE_INVALID');
 if(archivo){
  if(s.cases[0].modo!=='CREAR'||s.cases[0].codigoEsperado!=='OK'||s.maxFilasDatos!==20000)throw Error('DOC124_SCOPE_INVALID');
 }else{
  for(const mode of ['CREAR','AGREGAR','REEMPLAZAR'])if(!s.cases.some(c=>c.modo===mode&&c.codigoEsperado==='OK'))throw Error('DOC124_SCOPE_INVALID');
  for(const code of ['CLAVE_DUPLICADA_ARCHIVO','CLAVE_EXISTENTE_DESTINO'])if(!s.cases.some(c=>c.incidenciasEsperadas.includes(code)))throw Error('DOC124_SCOPE_INVALID');
 }
 if(s.cases[s.concurrencyCase].codigoEsperado!=='OK'||s.cases[s.concurrencyCase].filasEsperadas<8000)throw Error('DOC124_SCOPE_INVALID');
 return s;
}
function readFixture(c){
 try{const stat=fs.statSync(c.archivoPath);if(!stat.isFile()||stat.size<8||stat.size>10000000)throw Error();const b=fs.readFileSync(c.archivoPath);if(b.length!==stat.size||crypto.createHash('sha256').update(b).digest('hex')!==c.archivoHash)throw Error();return b;}catch{throw Error('DOC124_FIXTURE_INVALID');}
}
//Reutiliza queryRows de metadatos y queryFingerprint DOC32. No materializa miles
//de filas en Node ni depende del máximo10000 de la vía Rows del control existente.
async function snapshotDestination(query,fingerprint,table,exists){
 if(!/^imp_02_[A-Za-z0-9_]{1,57}$/.test(table||''))throw Error('DOC124_SCOPE_INVALID');
 const tables=await query('SELECT ENGINE,TABLE_TYPE,TABLE_COLLATION FROM information_schema.TABLES WHERE BINARY TABLE_SCHEMA=BINARY DATABASE() AND BINARY TABLE_NAME=BINARY ?',[table]);
 if(exists!==(tables.length===1)||tables.length>1)throw Error('DOC124_DESTINATION_SCOPE_CHANGED');
 const columns=await query('SELECT COLUMN_NAME,COLUMN_TYPE,IS_NULLABLE,COLUMN_DEFAULT,CHARACTER_SET_NAME,COLLATION_NAME,EXTRA FROM information_schema.COLUMNS WHERE BINARY TABLE_SCHEMA=BINARY DATABASE() AND BINARY TABLE_NAME=BINARY ? ORDER BY ORDINAL_POSITION',[table]);
 const indexes=await query('SELECT INDEX_NAME,NON_UNIQUE,SEQ_IN_INDEX,COLUMN_NAME,SUB_PART FROM information_schema.STATISTICS WHERE BINARY TABLE_SCHEMA=BINARY DATABASE() AND BINARY TABLE_NAME=BINARY ? ORDER BY INDEX_NAME,SEQ_IN_INDEX',[table]);
 let rowsHash=null;
 if(exists){
  if(!columns.length||columns.some(c=>!/^[A-Za-z_][A-Za-z0-9_]{0,63}$/.test(c.COLUMN_NAME||'')))throw Error('DOC124_CONTROL_COLUMNS_INVALID');
  const order=columns.map(c=>'BINARY `'+c.COLUMN_NAME+'`').join(',');
  rowsHash=await fingerprint('SELECT * FROM `'+table+'` WHERE ? IS NOT NULL ORDER BY '+order,'DOC124');
  if(!/^[0-9a-f]{64}$/.test(rowsHash||''))throw Error('DOC124_CONTROL_HASH_INVALID');
 }
 return crypto.createHash('sha256').update(JSON.stringify({tables,columns,indexes,rowsHash})).digest('hex');
}
module.exports={validateScope,readFixture,snapshotDestination};
