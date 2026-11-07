'use strict';
const fs=require('node:fs'),path=require('node:path'),{execFileSync}=require('node:child_process');
const {ROOT,STORAGE,sha,generate}=require('./support/doc117-alm03-candidate.cjs');
function main(args=process.argv.slice(2)){
 if(args.length!==1||args[0].startsWith('--'))throw Error('DOC117_ALM03_FRESH_OUTPUT_REQUIRED');
 const project='GestionDocumental-Docuarchi.net.vbproj',runtime='bin/GestionDocumental-Docuarchi.net.dll',originalSha=sha(fs.readFileSync(path.join(ROOT,runtime)));
 const files=[project,...[...fs.readFileSync(path.join(ROOT,project),'utf8').matchAll(/<(?:Compile|EmbeddedResource) Include="([^"]+)"/g)].map(x=>x[1].replaceAll('\\',path.sep))];
 const sources=Object.fromEntries([...new Set(files)].map(file=>{if(file.includes('$(')||path.isAbsolute(file)||path.relative(ROOT,path.resolve(ROOT,file)).startsWith('..'))throw Error('DOC117_ALM03_PROJECT_INPUT_INVALID');return[file,sha(fs.readFileSync(path.join(ROOT,file)))];}));
 const generated=generate(args[0]),dir=generated.dir,msbuild='C:/Program Files/Microsoft Visual Studio/18/Enterprise/MSBuild/Current/Bin/amd64/MSBuild.exe';
 let exitCode=0,output='';
 try{output=execFileSync(msbuild,[path.join(ROOT,project),'/t:Compile','/nologo','/v:minimal','/m:1','/p:Configuration=Debug','/p:BuildProjectReferences=false','/p:UseHostCompilerIfAvailable=false','/p:UseSharedCompilation=false','/p:OutputPath='+path.join(dir,'output')+'\\','/p:OutDir='+path.join(dir,'output')+'\\','/p:BaseIntermediateOutputPath='+path.join(dir,'obj')+'\\','/p:IntermediateOutputPath='+path.join(dir,'obj')+'\\','/p:CustomAfterMicrosoftCommonTargets='+generated.targetsPath],{cwd:ROOT,encoding:'utf8',windowsHide:true,maxBuffer:32*1024*1024,timeout:180000});}
 catch(error){exitCode=Number.isInteger(error.status)?error.status:1;output=String(error.stdout||'')+'\n'+String(error.stderr||'');}
 fs.writeFileSync(path.join(dir,'compile.log'),output,{flag:'wx'});
 if(sha(fs.readFileSync(path.join(ROOT,runtime)))!==originalSha||Object.entries(sources).some(([f,h])=>sha(fs.readFileSync(path.join(ROOT,f)))!==h))throw Error('DOC117_ALM03_PROTECTED_INPUT_CHANGED');
 const candidate=path.join(dir,'obj/GestionDocumental-Docuarchi.net.dll');
 const report={issueKey:'DOC-117',scenario:'ALM-03',status:exitCode===0?'COMPILED_DORMANT_CANDIDATE':'BUILD_NOT_COMPLETED',exitCode,errorCodes:[...new Set([...output.matchAll(/\berror\s+(MSB\d+|BC\d+)/g)].map(m=>m[1]))],sourceStorage:STORAGE,...Object.fromEntries(Object.entries(generated).filter(([k])=>!['dir','targetsPath'].includes(k))),sourceInputCount:Object.keys(sources).length,sourceInputsSha256:sha(JSON.stringify(sources)),originalSha256:originalSha,candidateSha256:exitCode===0?sha(fs.readFileSync(candidate)):null,deployed:false,executionAuthorized:false,authenticatedTestExecuted:false,liveRuntimeUnchanged:true,productSourcesUnchanged:true,scope:'Dormant observer; shared writer entry always refused. Synthetic core tested separately. Active resource/caller binding is not implemented.'};
 fs.writeFileSync(path.join(dir,'source-inputs.json'),JSON.stringify(sources,null,2)+'\n',{flag:'wx'});fs.writeFileSync(path.join(dir,'build-result.json'),JSON.stringify(report,null,2)+'\n',{flag:'wx'});console.log(JSON.stringify(report));process.exitCode=exitCode;
}
if(require.main===module)try{main();}catch{console.error('DOC117_ALM03_DORMANT_BUILD_STOPPED');process.exitCode=2;}
module.exports={main};
