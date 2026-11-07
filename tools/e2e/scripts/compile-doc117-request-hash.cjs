'use strict';
const fs=require('node:fs'),path=require('node:path'),{execFileSync}=require('node:child_process');
const {ROOT,sha}=require('./support/doc117-alm02-candidate.cjs');
function compile(directory){
 const dir=path.resolve(directory),relative=path.relative(path.join(ROOT,'.tmp'),dir);
 if(!relative||relative.startsWith('..')||path.isAbsolute(relative)||fs.existsSync(dir))throw Error('DOC117_HASH_FRESH_DIRECTORY_REQUIRED');
 let current=path.dirname(dir);while(current!==ROOT){if(fs.existsSync(current)&&fs.lstatSync(current).isSymbolicLink())throw Error('DOC117_HASH_PATH_INVALID');current=path.dirname(current);}
 const runtime=path.join(ROOT,'bin/GestionDocumental-Docuarchi.net.dll'),before=sha(fs.readFileSync(runtime));fs.mkdirSync(dir,{recursive:true});
 require(path.join(ROOT,'tests/doc110-perfil-importacion-behavior.test.cjs')).compileBehaviorHarness(dir);
 const vbc=execFileSync('powershell.exe',['-NoProfile','-Command','(Get-Command vbc.exe -ErrorAction Stop).Source'],{encoding:'utf8',windowsHide:true}).trim();
 const exe=path.join(dir,'Doc117SolicitudHash.exe');
 execFileSync(vbc,['/nologo','/target:exe','/out:'+exe,'/reference:System.dll','/reference:System.Core.dll','/reference:'+path.join(dir,'PerfilImportacion.dll'),'/reference:'+path.join(dir,'Newtonsoft.Json.dll'),path.join(ROOT,'tools/e2e/fixtures/Doc117SolicitudHash.vb'),path.join(ROOT,'tools/e2e/fixtures/Doc117SolicitudHashConsole.vb')],{cwd:dir,windowsHide:true,encoding:'utf8'});
 if(sha(fs.readFileSync(runtime))!==before)throw Error('DOC117_HASH_RUNTIME_CHANGED');
 const report={status:'COMPILED_OFFLINE',executableSha256:sha(fs.readFileSync(exe)),calculatorSha256:sha(fs.readFileSync(path.join(ROOT,'tools/e2e/fixtures/Doc117SolicitudHash.vb'))),consoleSha256:sha(fs.readFileSync(path.join(ROOT,'tools/e2e/fixtures/Doc117SolicitudHashConsole.vb'))),productHarnessSha256:sha(fs.readFileSync(path.join(dir,'PerfilImportacion.dll'))),serializerSha256:sha(fs.readFileSync(path.join(dir,'Newtonsoft.Json.dll'))),liveRuntimeUnchanged:true,realExecution:false};
 fs.writeFileSync(path.join(dir,'hash-build-result.json'),JSON.stringify(report,null,2)+'\n',{flag:'wx'});return{exe,report};
}
if(require.main===module)try{if(process.argv.length!==3)throw Error();console.log(JSON.stringify(compile(process.argv[2]).report));}catch{console.error('DOC117_HASH_BUILD_STOPPED');process.exitCode=2;}
module.exports={compile};
