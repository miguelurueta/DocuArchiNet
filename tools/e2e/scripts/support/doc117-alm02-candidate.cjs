'use strict';
// Local source generation only. This module cannot install a DLL or grant effects.
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const ROOT=path.resolve(__dirname,'../../../..');
const BINDING='Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorDocumentoImportacion.Legacy.vb';
const TEMPLATE='tools/e2e/fixtures/Doc117InternalDocumentCandidate.vb';
const sha=b=>crypto.createHash('sha256').update(b).digest('hex');
const PROTOCOL='FIRST_ATTEMPT_BEFORE_WRITER_V1';
const id=x=>typeof x==='string'&&/^[1-9][0-9]{0,9}$/.test(x)&&BigInt(x)<=4294967295n;
const digest=x=>typeof x==='string'&&/^[a-f0-9]{64}$/.test(x);
const check=(v,c)=>{if(!v)throw Error('DOC117_ALM02_'+c);};
function closed(value,keys){check(value&&typeof value==='object'&&!Array.isArray(value)&&Object.keys(value).length===keys.length&&keys.every(k=>Object.hasOwn(value,k)),'SCOPE_FIELDS_CHANGED');}
function validateScope(s){
 closed(s,['issueKey','protocol','runtimeRoot','runId','lotId','snapshotSha256','resources']);
 check(s.issueKey==='DOC-117'&&s.protocol===PROTOCOL&&/^[a-f0-9]{32}$/.test(s.runId)&&id(s.lotId)&&BigInt(s.lotId)>20n&&digest(s.snapshotSha256),'SCOPE_INVALID');
 const relative=path.relative(ROOT,s.runtimeRoot||'');
 check(path.isAbsolute(s.runtimeRoot)&&(!relative||(!relative.startsWith('..')&&!path.isAbsolute(relative)&&relative.split(path.sep)[0]==='.tmp')),'SITE_INVALID');
 check(Array.isArray(s.resources)&&s.resources.length===2,'RESOURCES_INVALID');
 for(const r of s.resources){
  closed(r,['rootId','fileId','expedienteId','relacionNivelId','fileName','bytes','contentSha256','requestSha256']);
  check(['rootId','fileId','expedienteId','relacionNivelId'].every(k=>id(r[k]))&&typeof r.fileName==='string'&&r.fileName.length<=255&&r.fileName.endsWith('.pdf')&&!/[\\/|<>:"?*\x00-\x1f]/.test(r.fileName)&&!r.fileName.endsWith(' ')&&typeof r.bytes==='string'&&/^[1-9][0-9]{0,9}$/.test(r.bytes)&&digest(r.contentSha256)&&digest(r.requestSha256),'RESOURCE_INVALID');
 }
 check(['rootId','fileId','expedienteId','relacionNivelId','requestSha256'].every(k=>new Set(s.resources.map(r=>r[k])).size===2),'RESOURCE_COLLISION');return s;
}
function scopeDigest(scope){const s=validateScope(scope);return sha(JSON.stringify({issueKey:s.issueKey,protocol:s.protocol,runtimeRoot:s.runtimeRoot,runId:s.runId,lotId:s.lotId,snapshotSha256:s.snapshotSha256,resources:s.resources.map(r=>({rootId:r.rootId,fileId:r.fileId,expedienteId:r.expedienteId,relacionNivelId:r.relacionNivelId,fileName:r.fileName,bytes:r.bytes,contentSha256:r.contentSha256,requestSha256:r.requestSha256}))}));}
function renderHelper(scope){
 const s=validateScope(scope),constants={SitioAutorizado:s.runtimeRoot,Ejecucion:s.runId,LoteAutorizado:s.lotId,HuellaSnapshot:s.snapshotSha256};
 for(let i=0;i<2;i++){const suffix=i===0?'A':'B',r=s.resources[i];for(const [name,key]of Object.entries({Raiz:'rootId',Archivo:'fileId',Expediente:'expedienteId',Relacion:'relacionNivelId',Nombre:'fileName',Bytes:'bytes',Contenido:'contentSha256',Solicitud:'requestSha256'}))constants[name+suffix]=r[key];}
 let helper=fs.readFileSync(path.join(ROOT,TEMPLATE),'utf8');
 for(const [key,value]of Object.entries(constants)){const pattern=new RegExp('(Private Const '+key+' As String = )"[^"]+"','g');check([...helper.matchAll(pattern)].length===1,'TEMPLATE_CHANGED');helper=helper.replace(pattern,(_,prefix)=>prefix+'"'+value.replaceAll('"','""')+'"');}
 // vbc on legacy Windows can otherwise decode non-ASCII file names as ANSI.
 return '\uFEFF'+helper.replace(/^\uFEFF/,'');
}
function once(source,needle,value){if(source.split(needle).length!==2)throw Error('DOC117_ALM02_BINDING_CHANGED');return source.replace(needle,()=>value);}
function overlay(source){
  let result=once(source,'        _datos = AddressOf catalogos.PrepararDatosAlmacenamiento',`        Dim autorizarOriginal = _autorizar
        Dim inicioLaboratorio As Boolean = False
        _autorizar = Sub(comando)
                         autorizarOriginal(comando)
                         Doc117InternalDocumentCandidate.ExigirAlcance(comando, inicioLaboratorio)
                     End Sub
        _datos = AddressOf catalogos.PrepararDatosAlmacenamiento`);
  result=once(result,'_iniciar = AddressOf repositorio.IniciarAlmacenamientoDocumento',`_iniciar = Function(comando, datos)
                       inicioLaboratorio = True
                       Return Doc117InternalDocumentCandidate.Iniciar(comando, Function() repositorio.IniciarAlmacenamientoDocumento(comando, datos))
                   End Function`);
  result=once(result,'_invocar = AddressOf InvocarAlmacenamiento','_invocar = Sub(comando, datos, copia, salida) Doc117InternalDocumentCandidate.Invocar(comando, Sub() InvocarAlmacenamiento(comando, datos, copia, salida))');
  return once(result,'_registrar = AddressOf repositorio.RegistrarResultadoDocumento','_registrar = Function(comando, resultado) Doc117InternalDocumentCandidate.Registrar(comando, resultado, AddressOf repositorio.RegistrarResultadoDocumento)');
}
function escapeXml(s){return s.replaceAll('&','&amp;').replaceAll('"','&quot;').replaceAll('<','&lt;');}
function generate(directory,scope){
  const dir=path.resolve(directory),relative=path.relative(path.join(ROOT,'.tmp'),dir);
  if(!relative||relative.startsWith('..')||path.isAbsolute(relative)||fs.existsSync(dir))throw Error('DOC117_ALM02_FRESH_DIRECTORY_REQUIRED');
  // Refuse a junction before creating build outputs.
  let parent=path.dirname(dir);while(parent!==ROOT){if(fs.existsSync(parent)&&fs.lstatSync(parent).isSymbolicLink())throw Error('DOC117_ALM02_PATH_INVALID');parent=path.dirname(parent);}
  const original=fs.readFileSync(path.join(ROOT,BINDING)),helper=scope?Buffer.from(renderHelper(scope),'utf8'):fs.readFileSync(path.join(ROOT,TEMPLATE));
  const transformed=overlay(original.toString('utf8'));
  fs.mkdirSync(dir,{recursive:true});
  const overlayPath=path.join(dir,'AdaptadorDocumentoImportacion.Legacy.Candidate.vb'),helperPath=path.join(dir,'Doc117InternalDocumentCandidate.vb');
  fs.writeFileSync(overlayPath,transformed,{flag:'wx'});fs.writeFileSync(helperPath,helper,{flag:'wx'});
  const targetsPath=path.join(dir,'laboratory-only.targets');
  fs.writeFileSync(targetsPath,`<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003"><Target Name="Doc117LaboratoryOverlay" BeforeTargets="CoreCompile"><ItemGroup><Compile Remove="${BINDING.replaceAll('/','\\')}" /><Compile Include="${escapeXml(overlayPath)}" /><Compile Include="${escapeXml(helperPath)}" /></ItemGroup></Target></Project>`,{flag:'wx'});
  return{dir,targetsPath,sourceBinding:BINDING,sourceBindingSha256:sha(original),overlaySha256:sha(transformed),helperSha256:sha(helper)};
}
module.exports={ROOT,BINDING,TEMPLATE,PROTOCOL,sha,overlay,generate,validateScope,scopeDigest,renderHelper};
