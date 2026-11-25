'use strict';
const fs=require('node:fs'),path=require('node:path');
const {readFixture,snapshotDestination}=require('./doc124-excel-scope.cjs');
function validateScope(s,baseUrl,expectedIssue='DOC-125'){
 if(expectedIssue==='DOC-127')return validateCapacityScope(s,baseUrl);
 if(!['DOC-125','DOC-126'].includes(expectedIssue))throw Error('DOC125_SCOPE_INVALID');
 const fixturePrefix=expectedIssue==='DOC-125'?'imp_02_doc125_e2e_':'imp_02_doc126_e2e_';
 const positive=n=>Number.isSafeInteger(n)&&n>0&&n<=0xffffffff,hash=x=>/^[a-f0-9]{64}$/.test(x||'');
 if(!s||s.issue!==expectedIssue||s.disposable!==true||s.destinationReadOnly===true||s.preserveResources!==true||s.baseUrl!==baseUrl||
  !positive(s.empresaId)||!positive(s.usuarioId)||!positive(s.moduloId)||!/^DOC[0-9]+_DA_E2E$/.test(s.controlPrefix||'')||
  !hash(s.environmentHash)||!hash(s.backendSha256)||!path.isAbsolute(s.runtimeRoot||'')||
  !positive(s.maxBusinessRows)||s.maxBusinessRows>100000||!positive(s.maxDeletedRows)||s.maxDeletedRows>100000||!Array.isArray(s.allowedEffects)||
  !['CREATE','INSERT','DELETE','RESULTADO','EXTERNAL_INSERT'].every(x=>s.allowedEffects.includes(x))||
  s.allowedEffects.some(x=>!['CREATE','INSERT','DELETE','RESULTADO','EXTERNAL_INSERT'].includes(x))||
  !Array.isArray(s.protectedTables)||!s.protectedTables.includes('imp_02_contratos')||
  s.protectedTables.some(t=>!/^imp_02_[A-Za-z0-9_]{1,57}$/.test(t)||t.startsWith(fixturePrefix))||
  !Array.isArray(s.cases)||s.cases.length<5||s.cases.length>8)throw Error('DOC125_SCOPE_INVALID');
 const modes={crear:'CREAR',agregar:'AGREGAR',reemplazar:'REEMPLAZAR',rollback:'REEMPLAZAR',carrera:'AGREGAR'};
 for(const c of s.cases)if(!c||modes[c.escenario]!==c.modo||!positive(c.conexionId)||!positive(c.plantillaId)||!positive(c.revisionEsperada)||!hash(c.huellaEsperada)||!hash(c.archivoHash)||
  !(new RegExp('^'+fixturePrefix+'[A-Za-z0-9_]{1,44}$')).test(c.tabla||'')||!path.isAbsolute(c.archivoPath||'')||!/^\.(?:xls|xlsx)$/i.test(path.extname(c.archivoPath))||
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
function lockNames(physical,table,id,expectedIssue='DOC-125'){
 const prefix=expectedIssue==='DOC-125'?'imp_02_doc125_e2e_':expectedIssue==='DOC-126'?'imp_02_doc126_e2e_':expectedIssue==='DOC-127'?'imp_02_doc12[567]_e2e_':null;
 if(!prefix||!physical||typeof physical.servidor!=='string'||typeof physical.base_ref!=='string'||!Number.isSafeInteger(Number(physical.puerto))||Number(physical.puerto)<1||Number(physical.puerto)>65535||![0,1,2].includes(Number(physical.minusculas))||!(new RegExp('^'+prefix+'[A-Za-z0-9_]{1,44}$')).test(table)||!/^[a-f0-9]{8}(?:-[a-f0-9]{4}){3}-[a-f0-9]{12}$/.test(id))throw Error('DOC125_LOCK_IDENTITY_INVALID');
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
// DOC127: una celda/formato/modo por ventana. No relaja los scopes DOC125/126.
function validateCapacityScope(s,baseUrl){
 const uint=n=>Number.isSafeInteger(n)&&n>0&&n<=0xffffffff,hash=x=>typeof x==='string'&&/^[a-f0-9]{64}$/.test(x),zero=n=>Number.isSafeInteger(n)&&n>=0;
 const permissionKeys=['cargas','carpetas','perfiles','fuentes','proveedores'];
 if(!s||s.issue!=='DOC-127'||s.mode!=='capacity'||s.baseUrl!==baseUrl||s.disposable!==true||s.preserveResources!==true||s.destinationReadOnly!==false||
  !uint(s.empresaId)||!uint(s.usuarioId)||!uint(s.moduloId)||s.controlPrefix!=='DOC127_DA_E2E'||s.raControlPrefix!=='DOC127_RA_E2E'||
  !hash(s.environmentHash)||!hash(s.raEnvironmentHash)||!hash(s.backendSha256)||!hash(s.webConfigSha256)||!hash(s.localSettingsSha256)||!path.isAbsolute(s.runtimeRoot||'')||
  s.accountPolicy!=='EXISTING_AUTHORIZED'||!s.expectedImportPermissions||Object.keys(s.expectedImportPermissions).length!==5||!permissionKeys.every(k=>[0,1].includes(s.expectedImportPermissions[k]))||s.expectedImportPermissions.cargas!==1||
  !s.assets||Object.keys(s.assets).length!==3||!['js/gestion/ImportacionExcel/ImportacionExcelApi.js','js/gestion/ImportacionExcel/ImportacionExcelVista.js','Styles/ImportacionExcel.css'].every(p=>hash(s.assets[p]))||
  !Array.isArray(s.cases)||s.cases.length!==1||!Array.isArray(s.protectedTables)||new Set(s.protectedTables).size!==s.protectedTables.length||s.protectedTables.length>64||
  !['imp_02_contratos','imp_02_egresos','imp_02_creditos'].every(t=>s.protectedTables.includes(t))||s.protectedTables.some(t=>!/^imp_02_[A-Za-z0-9_]{1,57}$/.test(t)))throw Error('DOC127_SCOPE_INVALID');
 let url;try{url=new URL(baseUrl);}catch{throw Error('DOC127_SCOPE_INVALID');}if(!['http:','https:'].includes(url.protocol)||url.username||url.password||url.search||url.hash)throw Error('DOC127_SCOPE_INVALID');
 const w=s.window,c=s.cases[0],start=Date.parse(w?.validFromUtc),end=Date.parse(w?.expiresUtc);
 if(!w||!/^[a-f0-9]{32}$/.test(w.id||'')||!Number.isFinite(start)||!Number.isFinite(end)||end<=start||end-start>1800000||
  typeof w.authorizationReference!=='string'||!w.authorizationReference.trim()||w.authorizationReference.length>300||/[\r\n]/.test(w.authorizationReference)||
  !uint(w.timeoutMs)||w.timeoutMs<60000||w.timeoutMs>900000||!uint(w.maxHttpRequests)||w.maxHttpRequests>20||!uint(w.maxSelectStatements)||w.maxSelectStatements>1000)throw Error('DOC127_SCOPE_INVALID');
 if(!c||c.escenario!=='capacidad'||!['CREAR','AGREGAR','REEMPLAZAR'].includes(c.modo)||!['XLS','XLSX'].includes(c.formato)||![1000,5000,10000].includes(c.filasEsperadas)||
  !uint(c.conexionId)||!uint(c.plantillaId)||!uint(c.revisionEsperada)||!hash(c.huellaEsperada)||!hash(c.archivoHash)||!hash(c.huellaDestino)||
  !/^imp_02_doc12[567]_e2e_[A-Za-z0-9_]{1,44}$/.test(c.tabla||'')||s.protectedTables.includes(c.tabla)||
  !path.isAbsolute(c.archivoPath||'')||path.extname(c.archivoPath).toUpperCase()!=='.'+c.formato||typeof c.hoja!=='string'||!c.hoja||c.hoja.length>120||
  !zero(c.filasDestinoEsperadas)||c.filasDestinoEsperadas>100000||c.codigoEsperado!=='OK'||
  ['reanudarConfirmada','reanudarFallida','continuarPreparada','claveExterna','valorExterno'].some(k=>c[k]!==undefined)||
  (c.modo==='CREAR'&&(c.filasDestinoEsperadas!==0||!c.tabla.startsWith('imp_02_doc127_e2e_'))))throw Error('DOC127_SCOPE_INVALID');
 const effects=['INSERT','RESULTADO',...(c.modo==='CREAR'?['CREATE']:c.modo==='REEMPLAZAR'?['DELETE']:[])];
 if(s.maxBusinessRows!==c.filasEsperadas||s.maxDeletedRows!==(c.modo==='REEMPLAZAR'?c.filasDestinoEsperadas:0)||!Array.isArray(s.allowedEffects)||s.allowedEffects.length!==effects.length||!effects.every(e=>s.allowedEffects.includes(e)))throw Error('DOC127_SCOPE_INVALID');
 return s;
}
function capacityPaths(s){
 const directory=path.join(s.runtimeRoot,'.tmp','doc127-windows');return {directory,claim:path.join(directory,s.window.id+'.json'),state:path.join(directory,s.window.id+'.state.json'),memory:path.join(directory,s.window.id+'.memory.json'),stop:path.join(directory,s.window.id+'.stop')};
}
function assertCapacityWindow(s,now=Date.now()){
 validateCapacityScope(s,s.baseUrl);
 if(now<Date.parse(s.window.validFromUtc)||now>=Date.parse(s.window.expiresUtc))throw Error('DOC127_WINDOW_EXPIRED');
}
function verifyCapacityRuntime(s){
 require('./doc126-excel-scope.cjs').verifyRuntime(s);
 const sha=p=>require('node:crypto').createHash('sha256').update(fs.readFileSync(p)).digest('hex');
 const local=path.join(s.runtimeRoot,'AppSettings.local.config');
 if(sha(path.join(s.runtimeRoot,'Web.config'))!==s.webConfigSha256|| (fs.existsSync(local)?sha(local):require('node:crypto').createHash('sha256').update('').digest('hex'))!==s.localSettingsSha256)throw Error('DOC127_RUNTIME_CHANGED');
}
function capacityScopeHash(file){return require('node:crypto').createHash('sha256').update(fs.readFileSync(file)).digest('hex');}
function claimCapacityWindow(s,file){
 assertCapacityWindow(s);verifyCapacityRuntime(s);const p=capacityPaths(s);fs.mkdirSync(p.directory,{recursive:true});
 const contents=fs.readFileSync(file);let saved;try{saved=JSON.parse(contents.toString('utf8').replace(/^\uFEFF/,''));}catch{throw Error('DOC127_SCOPE_CHANGED');}
 if(JSON.stringify(saved)!==JSON.stringify(s))throw Error('DOC127_SCOPE_CHANGED');
 const record={status:'ARMED',windowId:s.window.id,scopeSha256:require('node:crypto').createHash('sha256').update(contents).digest('hex'),recordedUtc:new Date().toISOString(),runnerPid:process.pid};
 let fd;try{fd=fs.openSync(p.claim,'wx');fs.writeFileSync(fd,JSON.stringify(record));}catch{throw Error('DOC127_WINDOW_CONSUMED');}finally{if(fd!==undefined)fs.closeSync(fd);}return p;
}
function verifyCapacityClaim(s,file){
 assertCapacityWindow(s);let record;try{record=JSON.parse(fs.readFileSync(capacityPaths(s).claim,'utf8'));}catch{throw Error('DOC127_CLAIM_REQUIRED');}
 if(record.status!=='ARMED'||record.windowId!==s.window.id||record.scopeSha256!==capacityScopeHash(file))throw Error('DOC127_CLAIM_REQUIRED');
 return record;
}
function beginCapacityExecution(s,file){
 const record=verifyCapacityClaim(s,file);let fd;
 try{fd=fs.openSync(capacityPaths(s).claim+'.started','wx');fs.writeFileSync(fd,JSON.stringify({startedUtc:new Date().toISOString(),scopeSha256:record.scopeSha256}));}catch{throw Error('DOC127_WINDOW_CONSUMED');}finally{if(fd!==undefined)fs.closeSync(fd);}
 return record;
}
function summarizeCapacityMemory(samples){
 if(!Array.isArray(samples)||!samples.length)return {available:false,sharedIisProcesses:true,reason:'NO_SAMPLES'};
 const metrics=samples.map(s=>({utc:s.utc,phase:s.phase,rss:s.workers.reduce((n,w)=>n+w.workingSetBytes,0),private:s.workers.reduce((n,w)=>n+w.privateBytes,0),processCount:s.workers.length}));
 const gaps=metrics.slice(1).map((m,i)=>Date.parse(m.utc)-Date.parse(metrics[i].utc));
 return {available:true,sharedIisProcesses:true,exclusiveAllocationClaimed:false,requestedIntervalMs:2000,sampleCount:metrics.length,firstUtc:metrics[0].utc,lastUtc:metrics.at(-1).utc,baseline:metrics[0],maxRssBytes:Math.max(...metrics.map(x=>x.rss)),maxPrivateBytes:Math.max(...metrics.map(x=>x.private)),maxGapMs:gaps.length?Math.max(...gaps):0,phases:[...new Set(metrics.map(m=>m.phase))],measuredPhases:[...new Set(metrics.filter(m=>m.processCount>0).map(m=>m.phase))],emptyProcessSamples:metrics.filter(m=>!m.processCount).length,limitation:'Sampling can miss peaks; shared IIS process totals are not per-load allocation.'};
}
module.exports={validateScope,readScope,readFixture,snapshotDestination,lockNames,resumeRequest,validateCapacityScope,capacityPaths,assertCapacityWindow,verifyCapacityRuntime,capacityScopeHash,claimCapacityWindow,verifyCapacityClaim,beginCapacityExecution,summarizeCapacityMemory};
