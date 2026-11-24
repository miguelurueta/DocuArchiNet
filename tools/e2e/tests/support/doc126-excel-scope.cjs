'use strict';
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const previous=require('./doc125-excel-scope.cjs');
const uint=n=>Number.isSafeInteger(n)&&n>0&&n<=0xffffffff,hash=x=>/^[a-f0-9]{64}$/.test(x||'');
const assetPaths=['js/gestion/ImportacionExcel/ImportacionExcelApi.js','js/gestion/ImportacionExcel/ImportacionExcelVista.js','Styles/ImportacionExcel.css'];
function validateScope(s,baseUrl,mode){
 if(!s||s.issue!=='DOC-126'||!['read','write'].includes(mode)||s.mode!==mode||s.baseUrl!==baseUrl||
    s.disposable!==true||s.preserveResources!==true||s.exclusiveAccountRequired!==true||!uint(s.empresaId)||!uint(s.usuarioId)||!uint(s.restrictedUsuarioId)||s.usuarioId===s.restrictedUsuarioId||!uint(s.moduloId)||
    !/^DOC126_DA_E2E$/.test(s.controlPrefix||'')||s.raControlPrefix!=='DOC126_RA_E2E'||!hash(s.environmentHash)||!hash(s.raEnvironmentHash)||!hash(s.backendSha256)||!path.isAbsolute(s.runtimeRoot||'')||
    !s.assets||Object.keys(s.assets).length!==3||!assetPaths.every(p=>hash(s.assets[p]))||!Array.isArray(s.protectedTables)||!['imp_02_contratos','imp_02_egresos','imp_02_creditos'].every(t=>s.protectedTables.includes(t))||
    s.protectedTables.some(t=>!/^imp_02_[A-Za-z0-9_]{1,57}$/.test(t)||t.startsWith('imp_02_doc126_e2e_')))throw Error('DOC126_SCOPE_INVALID');
 if(mode==='write'){
  previous.validateScope(s,baseUrl,'DOC-126');
  if(s.destinationReadOnly!==false||s.cases.some(c=>c.reanudarConfirmada||c.reanudarFallida||c.continuarPreparada))throw Error('DOC126_SCOPE_INVALID');
 }else if(s.destinationReadOnly!==true||s.maxBusinessRows!==0||s.maxDeletedRows!==0||!Array.isArray(s.allowedEffects)||s.allowedEffects.length||!Array.isArray(s.cases)||s.cases.length||
   !s.selection||!uint(s.selection.conexionId)||!uint(s.selection.plantillaId))throw Error('DOC126_SCOPE_INVALID');
 return s;
}
function readScope(file,baseUrl,mode){try{return validateScope(JSON.parse(fs.readFileSync(file,'utf8').replace(/^\uFEFF/,'')),baseUrl,mode);}catch{throw Error('DOC126_SCOPE_INVALID');}}
function verifyRuntime(s){
 const sha=p=>crypto.createHash('sha256').update(fs.readFileSync(p)).digest('hex');
 if(sha(path.join(s.runtimeRoot,'bin/GestionDocumental-Docuarchi.net.dll'))!==s.backendSha256||assetPaths.some(p=>sha(path.join(s.runtimeRoot,p))!==s.assets[p]))throw Error('DOC126_RUNTIME_CHANGED');
}
module.exports={validateScope,readScope,verifyRuntime,assetPaths,readFixture:previous.readFixture,snapshotDestination:previous.snapshotDestination,lockNames:previous.lockNames};
