'use strict';
const fs=require('node:fs'),path=require('node:path');
const dormant=require('./doc117-alm03-candidate.cjs');
const {ROOT,sha}=dormant;
const PROTOCOL='ALM03_INDIVIDUAL_GUARD_DIAGNOSTIC_NO_WRITER_V1',TEMPLATE='tools/e2e/fixtures/Doc117Alm03GuardDiagnostic.vb';
const check=(v,c)=>{if(!v)throw Error('DOC117_ALM03_PROBE_'+c);};
const KEYS=['site','actor','company','caller','relation','level','selectedExpedient','workflow','cabinet','expedient','typology','storageType','descriptionExact','descriptionEquivalent','documentClass','sessionTempPath','fileName'];
const GUARD_CODES=['SITE_CHANGED','CONTEXT_CHANGED','CALLER_CHANGED','INDIVIDUAL_SELECTION_CHANGED','FILE_CHANGED','INDIVIDUAL_ARGUMENTS_CHANGED','CALLER_SCOPE_CHANGED','UNBOUND_WRITER','WRITER_ARGUMENTS_CHANGED'].map(c=>'DOC117_ALM03_GUARD_'+c);
const CORE_CODES=['SCOPE_INVALID','PATH_INVALID','FRESH_JOURNAL_REQUIRED','RESOURCE_INVALID','RESOURCE_ALREADY_PREPARED','SESSIONS_NOT_DISTINCT','SESSION_CHANGED'].map(c=>'DOC117_ALM03_'+c);
const CODES=new Set([...GUARD_CODES,...CORE_CODES,'UNCLASSIFIED_FAILURE']);
function validateScope(s){
 const fields=['issueKey','protocol','runtimeRoot','runId','actorRef','companyRef','levelRef','expedientRef','relationRef','typologyRef','cabinetRef','cabinetName','fileName','bytes','contentSha256','classificationSha256','writerAllowed','importsAllowed','priorCaseSelectRequests','maximumAdditionalSelectRequests','maximumCumulativeSelectRequests','maximumRuntimeSeconds'];
 check(s&&Object.keys(s).sort().join(',')===fields.sort().join(','),'SCOPE_FIELDS_CHANGED');
 check(s.issueKey==='DOC-117'&&s.protocol===PROTOCOL&&s.runtimeRoot===ROOT&&/^[a-f0-9]{32}$/.test(s.runId)&&s.actorRef==='141'&&s.companyRef==='2'&&s.levelRef==='8'&&s.expedientRef==='467'&&s.relationRef==='1072'&&s.typologyRef==='50'&&s.cabinetRef==='10'&&/^[A-Za-z_][A-Za-z0-9_]{0,63}$/.test(s.cabinetName)&&s.fileName==='DOC117-ALM03-GUARD-DIAG-20261106.pdf'&&s.bytes===677&&s.contentSha256==='a5f1924afbae4bd631de893c49a56d9f09219075113e6da2ec983602cff84c6c'&&s.classificationSha256==='083005fcf663febf27982bef6ce9b2f17c03b278ea99d70267da3741cfcdf5ff'&&s.writerAllowed===false&&s.importsAllowed===false&&s.priorCaseSelectRequests===146&&s.maximumAdditionalSelectRequests===160&&s.maximumCumulativeSelectRequests===306&&s.maximumRuntimeSeconds===600,'SCOPE_INVALID');return s;
}
const scopeDigest=s=>sha(JSON.stringify(validateScope(s)));
function render(source,constants){for(const [name,value]of Object.entries(constants)){const pattern=new RegExp('(Private Const '+name+' As String = )"[^"]+"','g');check([...source.matchAll(pattern)].length===1,'TEMPLATE_CHANGED');source=source.replace(pattern,(_,p)=>p+'"'+value.replaceAll('"','""')+'"');}return '\uFEFF'+source.replace(/^\uFEFF/,'');}
function renderGuard(s){
 validateScope(s);const disabled=sha('DISABLED_IMPORT_A_'+s.runId),disabledB=sha('DISABLED_IMPORT_B_'+s.runId),constants={Site:s.runtimeRoot,Run:s.runId,Lot:'0',Snapshot:'0'.repeat(64),Cabinet:s.cabinetName,IndividualName:s.fileName,IndividualContent:s.contentSha256,IndividualDigest:scopeDigest(s),DigestA:disabled,DigestB:disabledB};
 for(const tag of ['A','B'])for(const key of ['Root','File','Exp','Rel','Name','Content','Request'])constants[key+tag]=['Content','Request'].includes(key)?'0'.repeat(64):'0';
 return render(fs.readFileSync(path.join(ROOT,'tools/e2e/fixtures/Doc117Alm03ClosedGuard.vb'),'utf8'),constants);
}
const renderDiagnostic=s=>render(fs.readFileSync(path.join(ROOT,TEMPLATE),'utf8'),{Site:validateScope(s).runtimeRoot,Run:s.runId,ScopeHash:scopeDigest(s),Cabinet:s.cabinetName,FileName:s.fileName});
function generate(directory,scope){
 const s=validateScope(scope),g=dormant.generate(directory),storage=path.join(g.dir,'ClassAlmacenamiento.Laboratory.vb'),guard=path.join(g.dir,'Doc117Alm03ClosedGuard.vb'),helper=path.join(g.dir,'Doc117Alm03GuardDiagnostic.vb'),service=path.join(g.dir,'WebServiceProducion.Laboratory.vb');
 let code=fs.readFileSync(storage,'utf8');const start='            Result = AlmacenamientoDocumentoProduccionDocumental(NombreGabinete,',end='                                                                 EstructuraDatosImagen)',index=code.indexOf(start),finish=code.indexOf(end,index)+end.length;
 check(index>=0&&code.indexOf(start,index+1)<0&&finish>index,'CALLER_CHANGED');const originalCall=code.slice(index,finish),replacement='            Result = Doc117Alm03GuardDiagnostic.Evaluar(NombreGabinete, RutaArchivoAlmacenar, DescripcionTipo, IdExpediente, IdTipoLogia, TipoAlmacenamiento, NombreClaseFormatoDocumento)';
 code=code.slice(0,index)+replacement+code.slice(finish);check(code.replace(replacement,()=>originalCall)===fs.readFileSync(storage,'utf8'),'UNRELATED_CHANGE');fs.writeFileSync(storage,code);fs.writeFileSync(guard,renderGuard(s),{flag:'wx'});fs.writeFileSync(helper,renderDiagnostic(s),{flag:'wx'});
 const originalService=fs.readFileSync(path.join(ROOT,'webservice/WebServiceProducion.asmx.vb'),'utf8'),signature='    Public Function ServiceSolicitaCargarDocumentoExpediente(ByVal parameter As Object)';check(originalService.split(signature).length===2,'PREPARATION_SERVICE_CHANGED');fs.writeFileSync(service,originalService.replace(signature,()=>signature+'\r\n        Doc117Alm03GuardDiagnostic.MarcarPreparacion()'),{flag:'wx'});
 const xml=v=>v.replaceAll('&','&amp;').replaceAll('"','&quot;'),targets=fs.readFileSync(g.targetsPath,'utf8');fs.writeFileSync(g.targetsPath,targets.replace('</ItemGroup>',()=>'<Compile Remove="webservice\\WebServiceProducion.asmx.vb" /><Compile Include="'+xml(service)+'" /><Compile Include="'+xml(guard)+'" /><Compile Include="'+xml(helper)+'" /></ItemGroup>'));
 return{...g,overlaySha256:sha(fs.readFileSync(storage)),guardSha256:sha(fs.readFileSync(guard)),diagnosticSha256:sha(fs.readFileSync(helper)),preparationServiceSha256:sha(fs.readFileSync(service)),resourceScopeSha256:scopeDigest(s),diagnostic:true,writerAllowed:false,importsAllowed:false};
}
function validateProof(p,s){
 validateScope(s);check(p&&Object.keys(p).sort().join(',')==='checks,code,outcome,protocol,runId,scopeSha256,version,writerInvoked','PROOF_FIELDS_CHANGED');
 check(p.version===1&&p.protocol===PROTOCOL&&p.runId===s.runId&&p.scopeSha256===scopeDigest(s)&&p.writerInvoked===false&&['GUARDS_ACCEPTED','GUARDS_REJECTED'].includes(p.outcome)&&p.checks&&Object.keys(p.checks).sort().join(',')===KEYS.slice().sort().join(',')&&Object.values(p.checks).every(v=>typeof v==='boolean'),'PROOF_CHANGED');
 check(p.outcome==='GUARDS_ACCEPTED'?p.code==='GUARDS_ACCEPTED'&&Object.values(p.checks).every(Boolean):CODES.has(p.code),'OUTCOME_CHANGED');return p;
}
module.exports={ROOT,sha,PROTOCOL,TEMPLATE,KEYS,GUARD_CODES,CORE_CODES,validateScope,scopeDigest,renderGuard,renderDiagnostic,generate,validateProof};
