'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs'),path=require('node:path'),{execFileSync}=require('node:child_process');
const root=path.resolve(__dirname,'..');
const run=(cmd,args,cwd=root)=>{try{return execFileSync(cmd,args,{cwd,encoding:'utf8',windowsHide:true,stdio:['ignore','pipe','pipe']});}catch(e){throw new Error((e.stdout||'')+'\n'+(e.stderr||''));}};
function compile(directory){
 fs.mkdirSync(directory,{recursive:true});
 require('./doc110-perfil-importacion-behavior.test.cjs').compileBehaviorHarness(directory);
 const compiler=name=>run('powershell.exe',['-NoProfile','-Command','(Get-Command '+name+' -ErrorAction Stop).Source']).trim();
 const sources=["Modelo/ProduccionDocumental/ImportacionExcel/ImportacionExcelModelos.vb","DTOs/ProduccionDocumental/ImportacionExcel/ImportacionExcelDtos.vb","Services/ProduccionDocumental/ImportacionExcel/ServicioPlantillasExcel.vb","Infrastructure/Repositories/ProduccionDocumental/ImportacionExcel/MySqlPlantillaExcelRepository.vb","Infrastructure/ProduccionDocumental/ImportacionExcel/ImportacionExcelContextAdapter.vb","Infrastructure/ProduccionDocumental/ImportacionExcel/ImportacionExcelConexionAdapter.vb","Infrastructure/ProduccionDocumental/ImportacionExcel/ImportacionExcelRequestGuard.vb","webservice/WebServiceImportacionExcel.asmx.vb"];
 sources.push('Services/ProduccionDocumental/ImportacionExcel/ServicioLecturaExcel.vb','Infrastructure/ProduccionDocumental/ImportacionExcel/LectorArchivoExcel.vb','Infrastructure/ProduccionDocumental/ImportacionExcel/InspectorFidelidadExcel.vb');
 sources.push('Services/ProduccionDocumental/ImportacionExcel/ServicioAnalisisPlantillaExcel.vb','generic_control/ImportacionExcelAnalisis.ashx.vb');
 sources.push('Services/ProduccionDocumental/ImportacionExcel/ReglasCargaExcel.vb','Services/ProduccionDocumental/ImportacionExcel/ServicioRecepcionExcel.vb','Services/ProduccionDocumental/ImportacionExcel/ServicioPreparacionExcel.vb','generic_control/ImportacionExcelUpload.ashx.vb');
 sources.push(...['ReferenciaRecepcionExcel','AlmacenArchivoExcel','ComposicionCargaExcel','PoliticaDestinoCargaExcelAdapter'].map(n=>'Infrastructure/ProduccionDocumental/ImportacionExcel/'+n+'.vb'));
 sources.push(...['SqlCargaExcel','MySqlCargaExcelRepository','MySqlPreparacionExcelValidator'].map(n=>'Infrastructure/Repositories/ProduccionDocumental/ImportacionExcel/'+n+'.vb'));
 for(const [name,version,framework] of [['ExcelDataReader','3.6.0','net45'],['ExcelNumberFormat','1.1.0','net20']]) fs.copyFileSync(path.join(root,'packages',name+'.'+version,'lib',framework,name+'.dll'),path.join(directory,name+'.dll'));
 fs.cpSync(path.join(root,'tests/Fixtures/ProduccionDocumental/ImportacionExcel'),path.join(directory,'excel-fixtures'),{recursive:true});
 sources.push('Services/ProduccionDocumental/ImportacionExcel/ServicioConfirmacionExcel.vb','Infrastructure/Repositories/ProduccionDocumental/ImportacionExcel/MySqlConfirmacionExcelWriter.vb','Infrastructure/Repositories/ProduccionDocumental/ImportacionExcel/MySqlEstructuraExcel.vb');
 const dll=path.join(directory,'ImportacionExcel.dll'),exe=path.join(directory,'ImportacionExcelTests.exe');
 fs.copyFileSync(path.join(root,'tests/Fixtures/ProduccionDocumental/doc122-snapshot-v1.json'),path.join(directory,'doc122-snapshot-v1.json'));
 const refs=['System.dll','System.Core.dll','System.Data.dll','System.Web.dll','System.Web.Services.dll','System.Web.Extensions.dll','System.Configuration.dll',path.join(directory,'PerfilImportacion.dll'),path.join(directory,'MySql.Data.dll'),path.join(directory,'Newtonsoft.Json.dll')].map(x=>'/reference:'+x);
 refs.push(...['System.Numerics.dll','System.Xml.dll','System.IO.Compression.dll',path.join(directory,'ExcelDataReader.dll'),path.join(directory,'ExcelNumberFormat.dll')].map(x=>'/reference:'+x));
 run(compiler('vbc.exe'),['/nologo','/target:library','/out:'+dll,...refs,...sources]);
 run(compiler('csc.exe'),['/nologo','/target:exe','/out:'+exe,...refs,'/reference:'+dll,'tests/ImportacionExcelBehaviorTests.cs','tests/ImportacionExcelLecturaTests.cs','tests/ImportacionExcelPreparacionTests.cs','tests/ImportacionExcelConfirmacionTests.cs']);
 return {exe,regression:path.join(directory,'PerfilImportacionTests.exe')};
}
module.exports={compile};
if(require.main===module)test('DOC-122: contrato nativo con fixtures sintéticos, sin base ni HTTP real',()=>{
 const directory=fs.mkdtempSync(path.join(root,'.tmp','doc122-native-')),h=compile(directory);
 const output=run(h.exe,[],directory); assert.match(output,/DOC122_NATIVE_PASS/); assert.match(output,/DOC123_NATIVE_PASS/); assert.match(output,/DOC124_NATIVE_PASS/); assert.match(output,/DOC125_NATIVE_PASS/); process.stdout.write(output);
 assert.match(run(h.regression,[],directory),/behavior tests: passed/);
});
