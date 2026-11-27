'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto'),cp=require('node:child_process');
const root=path.resolve(__dirname,'..');
const run=(program,args,cwd=root)=>cp.execFileSync(program,args,{cwd,encoding:'utf8',windowsHide:true,stdio:['ignore','pipe','pipe']});
test('DOC127 reutiliza generador DOC123 y lector real para18 libros locales, sin MySQL/HTTP',()=>{
 const directory=fs.mkdtempSync(path.join(root,'.tmp','doc127-reader-')),books=path.join(directory,'books'),legacy=path.join(directory,'legacy');
 const generator=path.join(root,'tests/Fixtures/ProduccionDocumental/ImportacionExcel/generar.py');
 run('python',[generator,'--capacity',books,crypto.randomUUID().replaceAll('-','')]);
 run('python',[generator,legacy]);
 const expected=JSON.parse(fs.readFileSync(path.join(root,'tests/Fixtures/ProduccionDocumental/ImportacionExcel/sha256.json'),'utf8'));
 assert.deepEqual(JSON.parse(fs.readFileSync(path.join(legacy,'sha256.json'),'utf8')),expected);
 require('./doc122-importacion-excel-behavior.test.cjs').compile(directory);
 const compiler=run('powershell.exe',['-NoProfile','-Command','(Get-Command csc.exe -ErrorAction Stop).Source']).trim(),exe=path.join(directory,'Doc127CapacityFixtureReader.exe');
 run(compiler,['/nologo','/target:exe','/out:'+exe,'/reference:System.dll','/reference:System.Core.dll',...['Newtonsoft.Json','ImportacionExcel','PerfilImportacion'].map(n=>'/reference:'+path.join(directory,n+'.dll')),path.join(root,'tests/support/Doc127CapacityFixtureReader.cs')]);
 const report=path.join(directory,'reader-results.json');assert.match(run(exe,[path.join(books,'capacity-manifest.json'),report],directory),/DOC127_LOCAL_READER_PASS cases=18/);
 const results=JSON.parse(fs.readFileSync(report,'utf8'));assert.equal(results.realE2E,false);assert.equal(results.cases.length,18);
 for(const format of ['XLS','XLSX'])for(const mode of ['CREAR','AGREGAR','REEMPLAZAR'])for(const rows of [1000,5000,10000])assert.ok(results.cases.some(c=>c.formato===format&&c.modo===mode&&c.filas===rows&&c.bytes<=10000000&&c.realMySql===false));
});
