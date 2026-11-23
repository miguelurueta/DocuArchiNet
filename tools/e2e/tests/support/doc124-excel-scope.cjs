'use strict';
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
function validateScope(s,baseUrl){
 const positive=n=>Number.isSafeInteger(n)&&n>0,hash=h=>/^[0-9a-f]{64}$/.test(h||'');
 if(!s||s.issue!=='DOC-124'||s.disposable!==true||s.baseUrl!==baseUrl||!positive(s.empresaId)||!positive(s.usuarioId)||!positive(s.moduloId)||
  !/^DOC[0-9]+_DA_E2E$/.test(s.controlPrefix||'')||!Array.isArray(s.cases)||s.cases.length<5||s.cases.length>12||
  !Number.isSafeInteger(s.concurrencyCase)||s.concurrencyCase<0||s.concurrencyCase>=s.cases.length)throw Error('DOC124_SCOPE_INVALID');
 for(const c of s.cases)if(!c||!['CREAR','AGREGAR','REEMPLAZAR'].includes(c.modo)||!positive(c.conexionId)||!positive(c.plantillaId)||!positive(c.revisionEsperada)||!hash(c.huellaEsperada)||!hash(c.archivoHash)||
  !/^imp_02_[A-Za-z0-9_]{1,57}$/.test(c.tabla||'')||['imp_02_egresos','imp_02_contratos'].includes(c.tabla.toLowerCase())||
  !path.isAbsolute(c.archivoPath||'')||!/^\.(?:xls|xlsx)$/i.test(path.extname(c.archivoPath))||typeof c.hoja!=='string'||!c.hoja||c.hoja.length>120||!positive(c.filasEsperadas)||c.filasEsperadas>10000||
  !['OK','CLAVES_INVALIDAS','LECTURA_INVALIDA'].includes(c.codigoEsperado)||!Array.isArray(c.incidenciasEsperadas)||!c.incidenciasEsperadas.every(x=>/^[A-Z_]+$/.test(x)))throw Error('DOC124_SCOPE_INVALID');
 for(const mode of ['CREAR','AGREGAR','REEMPLAZAR'])if(!s.cases.some(c=>c.modo===mode&&c.codigoEsperado==='OK'))throw Error('DOC124_SCOPE_INVALID');
 for(const code of ['CLAVE_DUPLICADA_ARCHIVO','CLAVE_EXISTENTE_DESTINO'])if(!s.cases.some(c=>c.incidenciasEsperadas.includes(code)))throw Error('DOC124_SCOPE_INVALID');
 if(s.cases[s.concurrencyCase].codigoEsperado!=='OK'||s.cases[s.concurrencyCase].filasEsperadas<8000)throw Error('DOC124_SCOPE_INVALID');
 return s;
}
function readFixture(c){
 try{const stat=fs.statSync(c.archivoPath);if(!stat.isFile()||stat.size<8||stat.size>10000000)throw Error();const b=fs.readFileSync(c.archivoPath);if(b.length!==stat.size||crypto.createHash('sha256').update(b).digest('hex')!==c.archivoHash)throw Error();return b;}catch{throw Error('DOC124_FIXTURE_INVALID');}
}
module.exports={validateScope,readFixture};
