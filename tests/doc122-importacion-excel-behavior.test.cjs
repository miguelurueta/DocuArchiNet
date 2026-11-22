'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs'),path=require('node:path'),{execFileSync}=require('node:child_process');
const root=path.resolve(__dirname,'..');
const run=(cmd,args,cwd=root)=>{try{return execFileSync(cmd,args,{cwd,encoding:'utf8',windowsHide:true,stdio:['ignore','pipe','pipe']});}catch(e){throw new Error((e.stdout||'')+'\n'+(e.stderr||''));}};
function compile(directory){
 fs.mkdirSync(directory,{recursive:true});
 require('./doc110-perfil-importacion-behavior.test.cjs').compileBehaviorHarness(directory);
 const compiler=name=>run('powershell.exe',['-NoProfile','-Command','(Get-Command '+name+' -ErrorAction Stop).Source']).trim();
 const sources=["Modelo/ProduccionDocumental/ImportacionExcel/ImportacionExcelModelos.vb","DTOs/ProduccionDocumental/ImportacionExcel/ImportacionExcelDtos.vb","Services/ProduccionDocumental/ImportacionExcel/ServicioPlantillasExcel.vb","Infrastructure/Repositories/ProduccionDocumental/ImportacionExcel/MySqlPlantillaExcelRepository.vb","Infrastructure/ProduccionDocumental/ImportacionExcel/ImportacionExcelContextAdapter.vb","Infrastructure/ProduccionDocumental/ImportacionExcel/ImportacionExcelConexionAdapter.vb","Infrastructure/ProduccionDocumental/ImportacionExcel/ImportacionExcelRequestGuard.vb","webservice/WebServiceImportacionExcel.asmx.vb"];
 const dll=path.join(directory,'ImportacionExcel.dll'),exe=path.join(directory,'ImportacionExcelTests.exe');
 fs.copyFileSync(path.join(root,'tests/Fixtures/ProduccionDocumental/doc122-snapshot-v1.json'),path.join(directory,'doc122-snapshot-v1.json'));
 const refs=['System.dll','System.Core.dll','System.Data.dll','System.Web.dll','System.Web.Services.dll','System.Web.Extensions.dll','System.Configuration.dll',path.join(directory,'PerfilImportacion.dll'),path.join(directory,'MySql.Data.dll'),path.join(directory,'Newtonsoft.Json.dll')].map(x=>'/reference:'+x);
 run(compiler('vbc.exe'),['/nologo','/target:library','/out:'+dll,...refs,...sources]);
 run(compiler('csc.exe'),['/nologo','/target:exe','/out:'+exe,...refs,'/reference:'+dll,'tests/ImportacionExcelBehaviorTests.cs']);
 return {exe,regression:path.join(directory,'PerfilImportacionTests.exe')};
}
module.exports={compile};
if(require.main===module)test('DOC-122: contrato nativo con fixtures sintéticos, sin base ni HTTP real',()=>{
 const directory=fs.mkdtempSync(path.join(root,'.tmp','doc122-native-')),h=compile(directory);
 assert.match(run(h.exe,[],directory),/DOC122_NATIVE_PASS/);
 assert.match(run(h.regression,[],directory),/behavior tests: passed/);
});
