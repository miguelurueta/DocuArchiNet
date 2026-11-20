'use strict';
// Offline compiled-reader roundtrip. No HTTP, credentials, database or IIS writes.
const fs=require('node:fs'),path=require('node:path'),cp=require('node:child_process'),crypto=require('node:crypto');
const root=path.resolve(__dirname,'../../..'),args=process.argv.slice(2);
if(args.length!==2||args[0]!=='--reader'||!/^\b[0-9a-f]{40}\b$/.test(args[1]))throw Error('Use --reader <40-character compatible commit>');
const reader=args[1],relative='Gestion/WebFormProducionDocumental.aspx.vb',project='GestionDocumental-Docuarchi.net.vbproj';
function git(...values){return cp.execFileSync('git',values,{cwd:root,maxBuffer:8*1024*1024});}
const normalize=b=>b.toString('utf8').replace(/\r\n/g,'\n'),hash=b=>crypto.createHash('sha256').update(b).digest('hex');
const projectText=fs.readFileSync(path.join(root,project),'utf8');
// Only the producer capability attributes may differ in compiled sources.
for(const file of [project,...[...projectText.matchAll(/<Compile Include="([^"]+)"/g)].map(m=>m[1].replace(/\\/g,'/'))]){
 if(file===relative)continue;
 if(normalize(git('show',reader+':'+file))!==normalize(fs.readFileSync(path.join(root,file))))throw Error('Reader sources differ: '+file);
}
const folder=path.join(root,'.tmp','doc120-plan-compatibility-'+Date.now());fs.mkdirSync(folder,{recursive:true});
const msbuild='C:/Program Files/Microsoft Visual Studio/18/Enterprise/MSBuild/Current/Bin/amd64/MSBuild.exe';
const csc='C:/Windows/Microsoft.NET/Framework64/v4.0.30319/csc.exe';
const packageDir=fs.readdirSync(path.join(root,'packages')).filter(n=>n.startsWith('Newtonsoft.Json.')).sort().at(-1);
const json=path.join(root,'packages',packageDir,'lib/net45/Newtonsoft.Json.dll');
const xml=s=>s.replace(/&/g,'&amp;').replace(/"/g,'&quot;');
const outputs={};
for(const name of ['A','B']){
 const out=path.join(folder,name);fs.mkdirSync(out);const buildLog=path.join(out,'build.log'),log=fs.openSync(buildLog,'w');
 const options=['/t:Compile','/p:Configuration=Debug','/p:BuildProjectReferences=false','/p:UseSharedCompilation=false','/v:minimal','/nologo',
  '/p:OutputPath='+out+'\\bin\\','/p:OutDir='+out+'\\bin\\','/p:IntermediateOutputPath='+out+'\\obj\\','/p:BaseIntermediateOutputPath='+out+'\\obj\\'];
 if(name==='A'){
  const source=path.join(out,'WebFormProducionDocumental.aspx.vb'),target=path.join(out,'reader.targets');fs.writeFileSync(source,git('show',reader+':'+relative));
  fs.writeFileSync(target,'<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003"><Target Name="Doc120CompatibleReaderSource" BeforeTargets="CoreCompile"><ItemGroup><Compile Remove="Gestion\\WebFormProducionDocumental.aspx.vb"/><Compile Include="'+xml(source)+'"/></ItemGroup></Target></Project>');
  options.push('/p:CustomBeforeMicrosoftCommonTargets='+target);
 }
 try{cp.execFileSync(msbuild,[project,...options],{cwd:root,stdio:['ignore',log,log]});}finally{fs.closeSync(log);}
 const dll=path.join(out,'GestionDocumental-Docuarchi.net.dll');fs.copyFileSync(path.join(out,'obj',path.basename(dll)),dll);fs.copyFileSync(json,path.join(out,'Newtonsoft.Json.dll'));
 cp.execFileSync(csc,['/nologo','/target:exe','/out:'+path.join(out,'probe.exe'),'/reference:'+dll,'/reference:'+json,'/reference:System.dll','/reference:System.Core.dll',path.join(root,'tests/ImportacionCarpetasPlanCompatibilityProbe.cs')],{cwd:root,stdio:'pipe'});
 outputs[name]={dllSha256:hash(fs.readFileSync(dll)),buildLog:path.relative(root,buildLog).replace(/\\/g,'/')};
}
const checks=[];
for(const [name,destination,source] of [['B','from-B.json',null],['A','from-A.json','from-B.json'],['B','returned-B.json','from-A.json']]){
 const parameters=[path.join(folder,destination)];if(source)parameters.push(path.join(folder,source));
 const output=cp.execFileSync(path.join(folder,name,'probe.exe'),parameters,{cwd:path.join(folder,name),encoding:'utf8'}).trim();
 if(!output.startsWith('PASS_SYNTHETIC_PLAN1_PLAN2_COMPATIBILITY'))throw Error('Compatibility not proved');checks.push({binary:name,output});
}
const result={status:'PASS_OFFLINE_COMPILED_READERS',readerCommit:reader,files:1000,checks,outputs,realSql:false,iisDeployment:false};
fs.writeFileSync(path.join(folder,'result.json'),JSON.stringify(result,null,2));
process.stdout.write(JSON.stringify({result:path.relative(root,path.join(folder,'result.json')).replace(/\\/g,'/'),...result},null,2)+'\n');
