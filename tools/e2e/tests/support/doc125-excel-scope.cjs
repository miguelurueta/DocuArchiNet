'use strict';
const fs=require('node:fs'),path=require('node:path');
const {readFixture,snapshotDestination}=require('./doc124-excel-scope.cjs');
function validateScope(s,baseUrl){
 const positive=n=>Number.isSafeInteger(n)&&n>0&&n<=0xffffffff,hash=x=>/^[a-f0-9]{64}$/.test(x||'');
 if(!s||s.issue!=='DOC-125'||s.disposable!==true||s.destinationReadOnly===true||s.preserveResources!==true||s.baseUrl!==baseUrl||
  !positive(s.empresaId)||!positive(s.usuarioId)||!positive(s.moduloId)||!/^DOC[0-9]+_DA_E2E$/.test(s.controlPrefix||'')||
  !hash(s.environmentHash)||!hash(s.backendSha256)||!path.isAbsolute(s.runtimeRoot||'')||
  !positive(s.maxBusinessRows)||s.maxBusinessRows>100000||!positive(s.maxDeletedRows)||s.maxDeletedRows>100000||!Array.isArray(s.allowedEffects)||
  !['CREATE','INSERT','DELETE','RESULTADO','EXTERNAL_INSERT'].every(x=>s.allowedEffects.includes(x))||
  s.allowedEffects.some(x=>!['CREATE','INSERT','DELETE','RESULTADO','EXTERNAL_INSERT'].includes(x))||
  !Array.isArray(s.protectedTables)||!s.protectedTables.includes('imp_02_contratos')||
  s.protectedTables.some(t=>!/^imp_02_[A-Za-z0-9_]{1,57}$/.test(t)||t.startsWith('imp_02_doc125_e2e_'))||
  !Array.isArray(s.cases)||s.cases.length<5||s.cases.length>8)throw Error('DOC125_SCOPE_INVALID');
 const modes={crear:'CREAR',agregar:'AGREGAR',reemplazar:'REEMPLAZAR',rollback:'REEMPLAZAR',carrera:'AGREGAR'};
 for(const c of s.cases)if(!c||modes[c.escenario]!==c.modo||!positive(c.conexionId)||!positive(c.plantillaId)||!positive(c.revisionEsperada)||!hash(c.huellaEsperada)||!hash(c.archivoHash)||
  !/^imp_02_doc125_e2e_[A-Za-z0-9_]{1,44}$/.test(c.tabla||'')||!path.isAbsolute(c.archivoPath||'')||!/^\.(?:xls|xlsx)$/i.test(path.extname(c.archivoPath))||
  typeof c.hoja!=='string'||!c.hoja||c.hoja.length>120||!positive(c.filasEsperadas)||c.filasEsperadas>20000||
  !Number.isSafeInteger(c.filasDestinoEsperadas)||c.filasDestinoEsperadas<0||c.filasDestinoEsperadas>100000||(c.modo==='CREAR'&&c.filasDestinoEsperadas!==0)||
  c.codigoEsperado!==(['rollback','carrera'].includes(c.escenario)?'CONFLICTO_CLAVE':'OK'))throw Error('DOC125_SCOPE_INVALID');
 for(const c of s.cases){
  const options=['reanudarConfirmada','reanudarFallida','continuarPreparada'].filter(k=>c[k]!==undefined);
  if(options.length>1)throw Error('DOC125_SCOPE_INVALID');
  if(!options.length)continue;
  const name=options[0],r=c[name],allowed=name==='reanudarConfirmada'?['crear','agregar','reemplazar']:name==='reanudarFallida'?['rollback']:['carrera'];
  if(!allowed.includes(c.escenario)||!r||!/^[a-f0-9]{8}(?:-[a-f0-9]{4}){3}-[a-f0-9]{12}$/.test(r.idCarga||'')||!hash(r.configuracionHash)||!hash(r.esquemaHash)||!hash(r.huellaDestino))throw Error('DOC125_SCOPE_INVALID');
 }
 for(const name of Object.keys(modes))if(s.cases.filter(c=>c.escenario===name).length!==1)throw Error('DOC125_SCOPE_INVALID');
 if(s.cases.length!==5||s.cases.reduce((n,c)=>n+c.filasEsperadas,0)+1>s.maxBusinessRows||s.cases.find(c=>c.escenario==='agregar').filasEsperadas<1000)throw Error('DOC125_SCOPE_INVALID');
 if(s.cases.map(c=>c.escenario).join(',')!=='crear,agregar,reemplazar,rollback,carrera'||s.cases.filter(c=>c.modo==='REEMPLAZAR').reduce((n,c)=>n+c.filasDestinoEsperadas,0)>s.maxDeletedRows)throw Error('DOC125_SCOPE_INVALID');
 const races=s.cases.filter(c=>c.escenario==='carrera');
 if(races.length!==1||races[0].filasEsperadas>1000||typeof races[0].claveExterna!=='string'||!/^DOC125R_[A-Za-z0-9_]{1,100}$/.test(races[0].claveExterna))throw Error('DOC125_SCOPE_INVALID');
 for(const c of s.cases)if(s.cases.some(other=>other!==c&&other.tabla===c.tabla&&other.modo!==c.modo))throw Error('DOC125_SCOPE_INVALID');
 return s;
}
function readScope(file,baseUrl){try{return validateScope(JSON.parse(fs.readFileSync(file,'utf8').replace(/^\uFEFF/,'')),baseUrl);}catch{throw Error('DOC125_SCOPE_INVALID');}}
function lockNames(physical,table,id){
 if(!physical||typeof physical.servidor!=='string'||typeof physical.base_ref!=='string'||!Number.isSafeInteger(Number(physical.puerto))||Number(physical.puerto)<1||Number(physical.puerto)>65535||![0,1,2].includes(Number(physical.minusculas))||!/^imp_02_doc125_e2e_[A-Za-z0-9_]{1,44}$/.test(table)||!/^[a-f0-9]{8}(?:-[a-f0-9]{4}){3}-[a-f0-9]{12}$/.test(id))throw Error('DOC125_LOCK_IDENTITY_INVALID');
 const hash=value=>{const obj=Object.fromEntries(Object.keys(value).sort().map(k=>[k,value[k]]));const json=JSON.stringify(obj).replace(/[\u007f-\uffff]/g,c=>'\\u'+c.charCodeAt(0).toString(16).padStart(4,'0'));return require('node:crypto').createHash('sha256').update(json,'utf8').digest('hex');};
 if(physical.fisica!==undefined&&!/^[a-f0-9]{64}$/.test(physical.fisica))throw Error('DOC125_LOCK_IDENTITY_INVALID');
 const fisica=physical.fisica||hash({Servidor:physical.servidor,Puerto:Number(physical.puerto),Base:physical.base_ref});
 const fold=Number(physical.minusculas)!==0;
 return {id:'dx'+hash({Fisica:fisica,Id:id}).slice(0,62),destination:'dt'+hash({Servidor:physical.servidor,Puerto:Number(physical.puerto),Base:fold?physical.base_ref.toLowerCase():physical.base_ref,Tabla:fold?table.toLowerCase():table}).slice(0,62)};
}
function resumeRequest(s,c,row){
 const r=c.reanudarConfirmada||c.reanudarFallida||c.continuarPreparada,confirmed=!!c.reanudarConfirmada,failed=!!c.reanudarFallida;
 if(!r||!row||row.id_carga!==r.idCarga||row.estado!==(confirmed?'CONFIRMADA':failed?'FALLIDA':'PREPARADA')||row.modo!==c.modo||Number(row.id_plantilla)!==c.plantillaId||String(row.ambito_ref)!==String(s.empresaId)||String(row.usuario_ref)!==String(s.usuarioId)||row.conexion_ref!=='DA:'+s.moduloId+':'+c.conexionId||row.tabla_destino!==c.tabla||row.archivo_hash!==c.archivoHash||row.configuracion_hash!==r.configuracionHash||row.esquema_hash!==r.esquemaHash||Number(row.filas_previstas)!==c.filasEsperadas||Number(row.filas_confirmadas)!==(confirmed?c.filasEsperadas:0)||Number(row.confirmado)!==(confirmed?1:0)||(failed&&row.codigo_incidencia!=='CONFLICTO_CLAVE')||(!confirmed&&!failed&&row.codigo_incidencia))throw Error('DOC125_RESUME_OWNER_RESULT_CHANGED');
 return {ConexionId:c.conexionId,IdCarga:r.idCarga,ArchivoHash:c.archivoHash,ConfiguracionHash:r.configuracionHash,EsquemaHash:r.esquemaHash,ConfirmacionReemplazo:c.modo==='REEMPLAZAR'};
}
module.exports={validateScope,readScope,readFixture,snapshotDestination,lockNames,resumeRequest};
