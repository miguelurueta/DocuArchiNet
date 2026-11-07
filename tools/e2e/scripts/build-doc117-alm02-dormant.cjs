'use strict';
// Builds to a new .tmp directory using Compile only. Never copies to bin/IIS.
const fs=require('node:fs'),path=require('node:path'),{execFileSync}=require('node:child_process');
const {ROOT,sha,generate,validateScope,scopeDigest}=require('./support/doc117-alm02-candidate.cjs');
function main(args=process.argv.slice(2)){
  const scoped=args.length===3&&args[0]==='--scope';
  if(args.length!==1&&!scoped)throw Error('DOC117_ALM02_OUTPUT_DIRECTORY_REQUIRED');
  const scope=scoped?validateScope(JSON.parse(fs.readFileSync(path.resolve(args[1]),'utf8').replace(/^\uFEFF/,''))):null;
  if(scope&&scope.runtimeRoot!==ROOT)throw Error('DOC117_ALM02_SITE_INVALID');
  const project='GestionDocumental-Docuarchi.net.vbproj',runtime='bin/GestionDocumental-Docuarchi.net.dll';
  const baseline=sha(fs.readFileSync(path.join(ROOT,runtime)));
  const files=[project,...[...fs.readFileSync(path.join(ROOT,project),'utf8').matchAll(/<(?:Compile|EmbeddedResource) Include="([^"]+)"/g)].map(x=>x[1].replaceAll('\\',path.sep))];
  const sources=Object.fromEntries([...new Set(files)].map(file=>{if(file.includes('$(')||path.isAbsolute(file)||path.relative(ROOT,path.resolve(ROOT,file)).startsWith('..'))throw Error('DOC117_ALM02_PROJECT_INPUT_INVALID');return[file,sha(fs.readFileSync(path.join(ROOT,file)))];}));
  const generated=generate(scoped?args[2]:args[0],scope),dir=generated.dir;
  const msbuild='C:\\Program Files\\Microsoft Visual Studio\\18\\Enterprise\\MSBuild\\Current\\Bin\\amd64\\MSBuild.exe';
  let exitCode=0,output='';
  try{output=execFileSync(msbuild,[path.join(ROOT,project),'/t:Compile','/nologo','/v:minimal','/m:1','/p:Configuration=Debug','/p:BuildProjectReferences=false','/p:UseHostCompilerIfAvailable=false','/p:UseSharedCompilation=false','/p:OutputPath='+path.join(dir,'output')+'\\','/p:OutDir='+path.join(dir,'output')+'\\','/p:BaseIntermediateOutputPath='+path.join(dir,'obj')+'\\','/p:IntermediateOutputPath='+path.join(dir,'obj')+'\\','/p:CustomAfterMicrosoftCommonTargets='+generated.targetsPath],{cwd:ROOT,encoding:'utf8',windowsHide:true,maxBuffer:32*1024*1024,timeout:180000});}
  catch(error){exitCode=Number.isInteger(error.status)?error.status:1;output=String(error.stdout||'')+'\n'+String(error.stderr||'');}
  fs.writeFileSync(path.join(dir,'compile.log'),output,{flag:'wx'});
  if(sha(fs.readFileSync(path.join(ROOT,runtime)))!==baseline||Object.entries(sources).some(([file,hash])=>sha(fs.readFileSync(path.join(ROOT,file)))!==hash))throw Error('DOC117_ALM02_PROTECTED_INPUT_CHANGED');
  const candidate=path.join(dir,'obj/GestionDocumental-Docuarchi.net.dll');
  const report={issueKey:'DOC-117',scenario:'ALM-02',status:exitCode===0?'COMPILED_DORMANT_CANDIDATE':'BUILD_NOT_COMPLETED',exitCode,errorCodes:[...new Set([...output.matchAll(/\berror\s+(MSB\d+|BC\d+)/g)].map(m=>m[1]))],sourceBinding:generated.sourceBinding,sourceBindingSha256:generated.sourceBindingSha256,overlaySha256:generated.overlaySha256,helperSha256:generated.helperSha256,sourceInputCount:Object.keys(sources).length,sourceInputsSha256:sha(JSON.stringify(sources)),candidateSha256:exitCode===0?sha(fs.readFileSync(candidate)):null,originalSha256:baseline,liveRuntimeUnchanged:true,productSourcesUnchanged:true,deployed:false,executionAuthorized:false,authenticatedTestExecuted:false,scope:'Dormant invalid scope; all storage refused before writer. Not an ALM-02 real PASS.'};
  if(scope){report.status=exitCode===0?'COMPILED_SCOPED_NOT_INSTALLED':'BUILD_NOT_COMPLETED';report.resourceScopeSha256=scopeDigest(scope);report.protocol=scope.protocol;report.scope='Closed resource and request scope compiled locally; attempt is pinned before writer. No installation or effects authorization.';}
  fs.writeFileSync(path.join(dir,'source-inputs.json'),JSON.stringify(sources,null,2)+'\n',{flag:'wx'});
  fs.writeFileSync(path.join(dir,'build-result.json'),JSON.stringify(report,null,2)+'\n',{flag:'wx'});
  console.log(JSON.stringify(report));process.exitCode=exitCode;
}
if(require.main===module)try{main();}catch{console.error('DOC117_ALM02_DORMANT_BUILD_STOPPED');process.exitCode=2;}
module.exports={main};
