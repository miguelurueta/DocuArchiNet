'use strict';
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const previous=require('./doc125-excel-scope.cjs');
const uint=n=>Number.isSafeInteger(n)&&n>0&&n<=0xffffffff,hash=x=>/^[a-f0-9]{64}$/.test(x||'');
const assetPaths=['js/gestion/ImportacionExcel/ImportacionExcelApi.js','js/gestion/ImportacionExcel/ImportacionExcelVista.js','Styles/ImportacionExcel.css'];
const permissionKeys=['cargas','carpetas','perfiles','fuentes','proveedores'];
function usesExistingAccount(s){return !!s&&(['functional','resilience'].includes(s.mode)||(s.mode==='assistant'&&s.exclusiveAccountRequired===false&&s.accountPolicy==='EXISTING_AUTHORIZED'));}
function validateScope(s,baseUrl,mode){
 const functional=mode==='functional',existingAccount=usesExistingAccount(s);
 if(!s||s.issue!=='DOC-126'||!['read','write','assistant','functional','resilience'].includes(mode)||s.mode!==mode||s.baseUrl!==baseUrl||
    s.disposable!==true||s.preserveResources!==true||!uint(s.empresaId)||!uint(s.usuarioId)||!uint(s.moduloId)||
    (existingAccount?(s.exclusiveAccountRequired!==false||s.accountPolicy!=='EXISTING_AUTHORIZED'||s.restrictedUsuarioId!==null||!s.expectedImportPermissions||Object.keys(s.expectedImportPermissions).length!==5||!permissionKeys.every(k=>[0,1].includes(s.expectedImportPermissions[k]))||s.expectedImportPermissions.cargas!==1):(s.exclusiveAccountRequired!==true||!uint(s.restrictedUsuarioId)||s.usuarioId===s.restrictedUsuarioId))||
    !/^DOC126_DA_E2E$/.test(s.controlPrefix||'')||s.raControlPrefix!=='DOC126_RA_E2E'||!hash(s.environmentHash)||!hash(s.raEnvironmentHash)||!hash(s.backendSha256)||!path.isAbsolute(s.runtimeRoot||'')||
    !s.assets||Object.keys(s.assets).length!==3||!assetPaths.every(p=>hash(s.assets[p]))||!Array.isArray(s.protectedTables)||!['imp_02_contratos','imp_02_egresos','imp_02_creditos'].every(t=>s.protectedTables.includes(t))||
    s.protectedTables.some(t=>!/^imp_02_[A-Za-z0-9_]{1,57}$/.test(t)||(mode!=='assistant'&&t.startsWith('imp_02_doc126_e2e_'))||(mode==='assistant'&&s.assistant&&t===s.assistant.tabla)))throw Error('DOC126_SCOPE_INVALID');
 if(functional){
  const effects=['INSERT','DELETE','RESULTADO'];
  if(s.destinationReadOnly!==false||s.maxBusinessRows!==2000||s.maxDeletedRows!==2||!Array.isArray(s.allowedEffects)||s.allowedEffects.length!==3||!effects.every(e=>s.allowedEffects.includes(e))||!Array.isArray(s.cases)||s.cases.length!==2)throw Error('DOC126_SCOPE_INVALID');
  for(const [i,c]of s.cases.entries())if(!c||c.escenario!==['agregar','reemplazar'][i]||c.modo!==['AGREGAR','REEMPLAZAR'][i]||c.tabla!=='imp_02_doc126_e2e_'+c.escenario||!uint(c.conexionId)||!uint(c.plantillaId)||!uint(c.revisionEsperada)||!hash(c.huellaEsperada)||!hash(c.archivoHash)||!path.isAbsolute(c.archivoPath||'')||path.extname(c.archivoPath).toLowerCase()!=='.xlsx'||c.hoja!=='Datos'||c.filasEsperadas!==1000||c.filasDestinoEsperadas!==i+1||c.codigoEsperado!=='OK'||['reanudarConfirmada','reanudarFallida','continuarPreparada','claveExterna','valorExterno'].some(k=>c[k]!==undefined))throw Error('DOC126_SCOPE_INVALID');
 }else if(mode==='resilience'){
  const effects=['INSERT','DELETE','RESULTADO','EXTERNAL_INSERT'];
  if(s.destinationReadOnly!==false||s.maxBusinessRows!==311||s.maxDeletedRows!==1000||!Array.isArray(s.allowedEffects)||s.allowedEffects.length!==4||!effects.every(e=>s.allowedEffects.includes(e))||!Array.isArray(s.cases)||s.cases.length!==2)throw Error('DOC126_SCOPE_INVALID');
  for(const [i,c]of s.cases.entries())if(!c||c.escenario!==['rollback','carrera'][i]||c.modo!==['REEMPLAZAR','AGREGAR'][i]||c.tabla!=='imp_02_doc126_e2e_'+['reemplazar','agregar'][i]||!uint(c.conexionId)||!uint(c.plantillaId)||!uint(c.revisionEsperada)||!hash(c.huellaEsperada)||!hash(c.archivoHash)||!path.isAbsolute(c.archivoPath||'')||path.extname(c.archivoPath).toLowerCase()!=='.xlsx'||c.hoja!=='Datos'||c.filasEsperadas!==[300,10][i]||c.filasDestinoEsperadas!==[1000,1001][i]||c.codigoEsperado!=='CONFLICTO_CLAVE'||['reanudarConfirmada','reanudarFallida','continuarPreparada'].some(k=>c[k]!==undefined)||(i===0&&(c.claveExterna!==undefined||c.valorExterno!==undefined))||(i===1&&(!/^DOC125R_[A-Za-z0-9_]{1,100}$/.test(c.claveExterna||'')||c.valorExterno!==undefined)))throw Error('DOC126_SCOPE_INVALID');
 }else if(mode==='write'){
  previous.validateScope(s,baseUrl,'DOC-126');
  if(s.destinationReadOnly!==false||s.cases.some(c=>c.reanudarConfirmada||c.reanudarFallida||c.continuarPreparada))throw Error('DOC126_SCOPE_INVALID');
 }else if(mode==='assistant'){
  const a=s.assistant,effects=['TEMPLATE_CREATE','CREATE','INSERT','RESULTADO'];
  if(s.destinationReadOnly!==false||s.maxTemplateCreates!==1||s.maxTemplateFieldCreates!==2||s.maxBusinessRows!==1000||s.maxDeletedRows!==0||!hash(s.analysisHandlerSha256)||
    !Array.isArray(s.allowedEffects)||s.allowedEffects.length!==effects.length||!effects.every(e=>s.allowedEffects.includes(e))||!Array.isArray(s.cases)||s.cases.length||
    !a||!uint(a.conexionId)||!/^imp_02_doc126_e2e_asistente_[a-f0-9]{8}$/.test(a.tabla||'')||!path.isAbsolute(a.archivoPath||'')||path.extname(a.archivoPath).toLowerCase()!=='.xlsx'||!hash(a.archivoHash)||a.hoja!=='Datos'||a.filasEsperadas!==1000||a.campoUnico!=='CLAVE'||
    !Array.isArray(a.campos)||a.campos.length!==2||a.campos.some((c,i)=>!c||c.CAMPO_FUENTE!==['CLAVE','VALOR'][i]||c.CAMPO_DESTINO!==c.CAMPO_FUENTE||!/^VARCHAR\([1-9][0-9]{0,3}\)$/.test(c.TIPO_CAMPO||'')||Number(c.TIPO_CAMPO.slice(8,-1))>4096))throw Error('DOC126_SCOPE_INVALID');
 }else if(s.destinationReadOnly!==true||s.maxBusinessRows!==0||s.maxDeletedRows!==0||!Array.isArray(s.allowedEffects)||s.allowedEffects.length||!Array.isArray(s.cases)||s.cases.length||
   !s.selection||!uint(s.selection.conexionId)||!uint(s.selection.plantillaId))throw Error('DOC126_SCOPE_INVALID');
 return s;
}
function verifyAccounts(s,rows,login,restrictedLogin){
 const owners=rows.filter(a=>Number(a.id)===s.usuarioId);
 if(owners.length!==1||Number(owners[0].activo)!==1||Number(owners[0].cargas)!==1||String(owners[0].login_ref).toLowerCase()!==String(login||'').toLowerCase())throw Error('DOC126_ACCOUNT_SCOPE_CHANGED');
 if(usesExistingAccount(s)){
  if(rows.length!==1||!permissionKeys.every(k=>Number(owners[0][k])===s.expectedImportPermissions[k]))throw Error('DOC126_ACCOUNT_SCOPE_CHANGED');
  return;
 }
 const restricted=rows.filter(a=>Number(a.id)===s.restrictedUsuarioId);
 if(rows.length!==2||restricted.length!==1||!permissionKeys.slice(1).every(k=>Number(owners[0][k])===0))throw Error('DOC126_EXCLUSIVE_ACCOUNT_REQUIRED');
 if(Number(restricted[0].activo)!==1||Number(restricted[0].cargas)!==0||String(restricted[0].login_ref).toLowerCase()!==String(restrictedLogin||'').toLowerCase())throw Error('DOC126_ACCOUNT_SCOPE_CHANGED');
}
function readScope(file,baseUrl,mode){try{return validateScope(JSON.parse(fs.readFileSync(file,'utf8').replace(/^\uFEFF/,'')),baseUrl,mode);}catch{throw Error('DOC126_SCOPE_INVALID');}}
function verifyRuntime(s){
 const sha=p=>crypto.createHash('sha256').update(fs.readFileSync(p)).digest('hex');
 if(sha(path.join(s.runtimeRoot,'bin/GestionDocumental-Docuarchi.net.dll'))!==s.backendSha256||assetPaths.some(p=>sha(path.join(s.runtimeRoot,p))!==s.assets[p]))throw Error('DOC126_RUNTIME_CHANGED');
 if(s.mode==='assistant'&&sha(path.join(s.runtimeRoot,'generic_control/ImportacionExcelAnalisis.ashx'))!==s.analysisHandlerSha256)throw Error('DOC126_RUNTIME_CHANGED');
}
function templateRequestAllowed(s,request){
 if(!s||s.mode!=='assistant'||!request||request.ConexionId!==s.assistant.conexionId||request.RevisionEsperada!==0||request.HuellaEsperada!=='')return false;
 const p=request.Plantilla,a=s.assistant;
 return Object.keys(request).length===4&&!!p&&Object.keys(p).length===10&&p.ID_PLANTILLAIMP===0&&p.REVISION_CARGA===0&&p.IMP01_XMLCONEXION_ID_XMLCONEXION===a.conexionId&&p.NOMBRE_PLANTILLA===a.tabla&&p.CAMPO_UNICO===a.campoUnico&&p.LIBRO_EXCEL===a.hoja&&p.Formato==='XLSX'&&p.Modo==='CREAR'&&p.ESTADO_PLANTILLA===1&&
  Array.isArray(p.Campos)&&p.Campos.length===2&&p.Campos.every((c,i)=>c&&Object.keys(c).length===9&&c.ID_CAMPOS_PLANTILLA===0&&['CAMPO_FUENTE','CAMPO_DESTINO','TIPO_CAMPO'].every(k=>c[k]===a.campos[i][k])&&['REMPLAZA_COMA','REMPLAZA_PUNTO','OPCION_SPLITER','NUMERO_MATRIZ_EXPLITER'].every(k=>c[k]===0)&&(c.CARACTER_SPLITER===null||c.CARACTER_SPLITER===''));
}
module.exports={validateScope,readScope,verifyRuntime,verifyAccounts,usesExistingAccount,assetPaths,templateRequestAllowed,readFixture:previous.readFixture,snapshotDestination:previous.snapshotDestination,lockNames:(physical,table,id)=>previous.lockNames(physical,table,id,'DOC-126')};
