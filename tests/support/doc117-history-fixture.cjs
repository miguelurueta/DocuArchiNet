'use strict';
// Synthetic, in-memory history for offline loader/grant tests. Nothing here can
// create a real authorization, contact the application or install a runtime.
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const {mock,after}=require('node:test');
const ROOT=path.resolve(__dirname,'../..');
const sha=b=>crypto.createHash('sha256').update(b).digest('hex');
const base='Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/';
const absolute=f=>path.resolve(ROOT,f),originalRead=fs.readFileSync.bind(fs);
const readJson=f=>JSON.parse(originalRead(absolute(f),'utf8').replace(/^\uFEFF/,''));
function scope(){
 const s=require('./doc117-alm03-coexistence-fixture.cjs').fixture().scope;
 s.individual.cabinetName='CONTABIL';
 s.individual.contentSha256=sha(originalRead(absolute('tools/e2e/fixtures/doc111-carga-individual.pdf')));
 for(const r of s.resources)r.contentSha256=s.individual.contentSha256;
 return s;
}
function probeScope(){return{issueKey:'DOC-117',protocol:'ALM03_INDIVIDUAL_GUARD_DIAGNOSTIC_NO_WRITER_V1',runtimeRoot:ROOT,runId:'d'.repeat(32),actorRef:'141',companyRef:'2',levelRef:'8',expedientRef:'467',relationRef:'1072',typologyRef:'50',cabinetRef:'10',cabinetName:'CONTABIL',fileName:'DOC117-ALM03-GUARD-DIAG-20261106.pdf',bytes:677,contentSha256:'a5f1924afbae4bd631de893c49a56d9f09219075113e6da2ec983602cff84c6c',classificationSha256:'083005fcf663febf27982bef6ce9b2f17c03b278ea99d70267da3741cfcdf5ff',writerAllowed:false,importsAllowed:false,priorCaseSelectRequests:146,maximumAdditionalSelectRequests:160,maximumCumulativeSelectRequests:306,maximumRuntimeSeconds:600};}
function install(){
 const files=new Map(),dirs=new Set();
 const put=(f,value)=>{const key=absolute(f),bytes=Buffer.isBuffer(value)?value:Buffer.from(typeof value==='string'?value:JSON.stringify(value));files.set(key,bytes);let cursor=path.dirname(key);while(cursor!==ROOT){dirs.add(cursor);cursor=path.dirname(cursor);}return sha(bytes);};
 const get=f=>files.get(absolute(f))??originalRead(absolute(f));
 const originalSha256=sha(originalRead(absolute('bin/GestionDocumental-Docuarchi.net.dll')));
 const sourceInputs={};for(let i=0;i<912;i++){const name='.tmp/doc117-unit-fixture/inputs/'+i+'.vb';sourceInputs[name]=put(name,'SYNTHETIC SOURCE '+i);}
 const sourceInputsFile='.tmp/doc117-unit-fixture/inputs.json';put(sourceInputsFile,sourceInputs);
 const calculator='.tmp/doc117-unit-fixture/calculator/Doc117SolicitudHash.exe';
 const hashToolSha256=put(calculator,'SYNTHETIC CALCULATOR - NOT EXECUTABLE');
 const productHarnessSha256=put(path.join(path.dirname(calculator),'PerfilImportacion.dll'),'SYNTHETIC HARNESS');
 const serializerSha256=put(path.join(path.dirname(calculator),'Newtonsoft.Json.dll'),'SYNTHETIC SERIALIZER');
 put(path.join(path.dirname(calculator),'hash-build-result.json'),{status:'COMPILED_OFFLINE',executableSha256:hashToolSha256,productHarnessSha256,serializerSha256,calculatorSha256:sha(get('tools/e2e/fixtures/Doc117SolicitudHash.vb')),consoleSha256:sha(get('tools/e2e/fixtures/Doc117SolicitudHashConsole.vb'))});
 function prepared(kind,s,policy){
  const name={internal:'doc117-alm02-interno-propuesto-20261106.json',pair:'doc117-alm03-coexistencia-propuesto-20261106.json',probe:'doc117-alm03-guard-diagnostic-propuesto-20261106.json',canonical:'doc117-alm03c-coexistencia-propuesto-20261106.json'}[kind];
  const p=readJson(base+name),dir='.tmp/doc117-unit-fixture/'+kind;
  Object.assign(p,{originalSha256,sourceInputsFile,sourceInputsSha256:sha(JSON.stringify(sourceInputs)),hashToolFile:calculator,hashToolSha256});
  const resourceScopeSha256=policy.scopeDigest(s);p.resourceScopeSha256=resourceScopeSha256;
  const commands=s.resources?.map((r,i)=>({loteId:s.lotId,recursoId:r.fileId,fase:'ALMACENAR',revisionEsperada:3,claveOperacion:String(i+1).repeat(32)}));
  if(commands){p.readyFile=dir+'/ready.json';p.readyFileSha256=put(p.readyFile,{resourceScope:s,commands});}
  const candidateFile=dir+'/candidate.dll',candidateSha256=put(candidateFile,'SYNTHETIC '+kind+' CANDIDATE - NOT EXECUTABLE');
  Object.assign(p,{candidateFile,candidateSha256});
  const report={status:kind==='probe'?'COMPILED_NO_WRITER_DIAGNOSTIC':'COMPILED_SCOPED_NOT_INSTALLED',candidateSha256,originalSha256,resourceScopeSha256,deployed:false,executionAuthorized:false,caseId:'ALM-03C',writerAllowed:false,importsAllowed:false};
  const sources={'ClassAlmacenamiento.Laboratory.vb':'overlaySha256','Doc117Alm03MethodObserver.vb':'helperSha256','Doc117Alm03ClosedGuard.vb':'guardSha256','AdaptadorDocumentoImportacion.Legacy.Laboratory.vb':'bindingSha256','WebServiceProducion.Laboratory.vb':'preparationServiceSha256','Doc117Alm03GuardDiagnostic.vb':'diagnosticSha256'};
  for(const [file,key]of Object.entries(sources))report[key]=put(dir+'/'+file,key==='guardSha256'&&policy.renderGuard?policy.renderGuard(s):key==='diagnosticSha256'&&policy.renderDiagnostic?policy.renderDiagnostic(s):'SYNTHETIC '+file);
  if(kind==='probe'){
   p.resourceScope=s;p.historicalResourceScopeFile=dir+'/historical.json';p.historicalResourceScopeFileSha256=put(p.historicalResourceScopeFile,scope());
   p.priorReadbackFile=dir+'/prior.json';p.priorReadbackSha256=put(p.priorReadbackFile,{status:'READ_VERIFICATION_COMPLETE',cumulativeSelectRequests:146,documentsObserved:1});
   p.buildReportFile=dir+'/build.json';p.buildReportSha256=put(p.buildReportFile,report);
  }else{p.candidateBuildReport=dir+'/build.json';p.candidateBuildReportSha256=put(p.candidateBuildReport,report);}
  if(kind==='canonical'){
   p.setupCertificateFile=dir+'/setup.json';p.setupCertificateSha256=put(p.setupCertificateFile,{status:'SETUP_PASS_CANONICAL_BOUND_NO_DOCUMENTS',cumulativeSelectRequests:339,documents:0,storageRequests:0,sealedReadyFile:{sha256:p.readyFileSha256}});
   p.rehearsalFile=dir+'/rehearsal.json';p.rehearsalSha256=put(p.rehearsalFile,{status:'PASS',candidateSha256,independentGuardian:true,temporaryRuntimeRestored:true,bothBackupsByteIdentical:true,liveRuntimeActivation:false});
  }
  put(base+name,p);return{p,scope:s,s,proposalSha256:sha(get(base+name))};
 }
 const pair=prepared('pair',scope(),require('../../tools/e2e/scripts/support/doc117-alm03-scoped-candidate.cjs'));
 const internalScope=scope();delete internalScope.individual;Object.assign(internalScope,{protocol:'FIRST_ATTEMPT_BEFORE_WRITER_V1',lotId:'21'});internalScope.resources.forEach((r,i)=>Object.assign(r,{rootId:String(33+i),fileId:String(34+i),expedienteId:String(664+i),relacionNivelId:String(1089+i)}));
 prepared('internal',internalScope,require('../../tools/e2e/scripts/support/doc117-alm02-candidate.cjs'));
 const probe=prepared('probe',probeScope(),require('../../tools/e2e/scripts/support/doc117-alm03-guard-diagnostic.cjs'));
 const c=scope();Object.assign(c,{caseId:'ALM-03C',protocol:'ALM03_CANONICAL_CLOSED_RESOURCES_BEFORE_WRITER_V2',lotId:'23',descriptionBinding:require('../../tools/e2e/scripts/support/doc117-alm03-canonical-description.cjs').bindingFor('COMPROBANTE DE EGRESO')});c.individual.fileName='DOC117-ALM03C-IND-20261106.pdf';c.resources.forEach((r,i)=>Object.assign(r,{rootId:String(37+i),fileId:String(38+i),expedienteId:String(668+i),relacionNivelId:String(1093+i),fileName:'DOC117-ALM03C-IMPORT-'+(i?'B':'A')+'-20261106.pdf'}));
 prepared('canonical',c,require('../../tools/e2e/scripts/support/doc117-alm03c-candidate.cjs'));
 put('.tmp/doc117-alm03-setup-57219fc625b867b54d8d282b6b1eb0a2-scope.json',scope());
 put('.tmp/doc117-alm03-guard-probe-scope-20261106.json',probeScope());
 put('.tmp/doc117-alm03-guard-probe-authorization.json',{scope:'ALM-03-GUARD-DIAGNOSTIC',environment:probe.p.environment,authorized:true,environmentAuthorized:true,account:true,select:true,temporaryUpload:true,runtimeChanges:true,exclusive:true,discardable:true,storage:false,imports:false,authorizationId:'b'.repeat(32),proposalSha256:probe.proposalSha256,candidateSha256:probe.p.candidateSha256,resourceScopeSha256:probe.p.resourceScopeSha256,codeSha256:'0'.repeat(64)});
 const prior=require('../../tools/e2e/scripts/support/doc117-alm03-preflight-continuation.cjs');
 const authorization={authorizationId:prior.PRIOR_ID,codeSha256:prior.PRIOR_CODE,scope:'ALM-03-COEXISTENCE',authorized:true,environmentAuthorized:true,account:true,select:true,storage:true,runtimeChanges:true,exclusive:true,discardable:true,proposalSha256:pair.proposalSha256,candidateSha256:pair.p.candidateSha256,resourceScopeSha256:pair.p.resourceScopeSha256,environment:pair.p.environment};
 put(prior.FILES.authorization,authorization);
 const priorArtifactSha256=put(prior.FILES.priorArtifact,{status:'STOPPED_REVIEW_REQUIRED',scope:authorization.scope,code:'DOC117_ALM03_CONTROL_NOT_UNIQUE',codeSha256:prior.PRIOR_CODE,proposalSha256:authorization.proposalSha256,candidateSha256:authorization.candidateSha256,originalSha256,runtimeRestored:true,selectRequests:32,history:[]});
 put(prior.FILES.diagnostic,{status:'DIAGNOSTIC_COMPLETE',authorizationId:prior.PRIOR_ID,scenario:'ALM-03-INDEX-DIAGNOSTIC',priorSelectRequests:32,selectRequests:3,cumulativeSelectRequests:35,storageRequests:0,applicationLogins:0,runtimeUnchanged:true,proof:{defaultRelationCount:0,relationCount:0,dateMappingCount:0},priorArtifact:prior.FILES.priorArtifact,priorArtifactSha256});
 const p=readJson(base+'doc117-alm03c-recursos-propuestos-20261106.json');Object.assign(p,{hashToolFile:calculator,hashToolSha256});
 const setup={p,proposalSha256:sha('SYNTHETIC SETUP PROPOSAL'),codeSha256:sha('SYNTHETIC SETUP CODE')};
 const parent={scope:'ALM-03C-SETUP',authorizationId:'b83e24a81417007ea8f92c54d3c624b1',authorized:true,budgetIncreaseAuthorized:true,priorSelectRequests:250,maximumAdditionalSelectRequests:160,maximumCumulativeSelectRequests:410,storage:false,runtimeChanges:false,individualUpload:false,proposalSha256:setup.proposalSha256,codeSha256:setup.codeSha256,hashToolSha256};
 put('.tmp/doc117-alm03c-setup-authorization.json',parent);
 put('tools/e2e/artifacts/doc117-alm03c-setup-'+parent.authorizationId+'.json',{status:'STOPPED_REVIEW_REQUIRED',code:'DOC117_ALM03C_PRIOR_INVENTORY_CHANGED',priorSelectRequests:250,selectRequests:3,cumulativeSelectRequests:253,failedAt:'SESSION_OPENED',references:['BROWSER_OPEN_REQUESTED','SESSION_OPEN_REQUESTED','SESSION_OPENED'].map(stage=>({stage})),proposalSha256:setup.proposalSha256,codeSha256:setup.codeSha256});
 const read=fs.readFileSync.bind(fs),exists=fs.existsSync.bind(fs),lstat=fs.lstatSync.bind(fs);
 mock.method(fs,'readFileSync',(file,options)=>{const key=typeof file==='string'?path.resolve(file):null;if(!files.has(key))return read(file,options);const b=Buffer.from(files.get(key)),encoding=typeof options==='string'?options:options?.encoding;return encoding?b.toString(encoding):b;});
 mock.method(fs,'existsSync',file=>files.has(path.resolve(file))||dirs.has(path.resolve(file))||exists(file));
 mock.method(fs,'lstatSync',file=>files.has(path.resolve(file))||dirs.has(path.resolve(file))?{isSymbolicLink:()=>false}:lstat(file));
 after(()=>mock.restoreAll());
 const corrupt=file=>{const key=absolute(file);if(!files.has(key))throw Error('Only synthetic files can be corrupted');const previous=files.get(key);files.set(key,Buffer.from('SYNTHETIC CORRUPTION'));return()=>files.set(key,previous);};
 return{corrupt,setup,preparation:{profile:{baseUrl:'https://localhost/GestionDocumental-Docuarchi.net/',ignoreHttpsErrors:true},profileSha256:sha('SYNTHETIC PROFILE'),previousSha256:sha('SYNTHETIC PREVIOUS'),codeSha256:sha('SYNTHETIC PREPARATION CODE')}};
}
module.exports={scope,probeScope,install};
