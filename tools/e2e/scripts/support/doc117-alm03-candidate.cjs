'use strict';
// Offline dormant source overlay only. No active scope, deployment or grants.
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const ROOT=path.resolve(__dirname,'../../../..'),STORAGE='workflow/ClassAlmacenamiento.vb',TEMPLATE='tools/e2e/fixtures/Doc117Alm03MethodObserver.vb';
const sha=b=>crypto.createHash('sha256').update(b).digest('hex');
const SOURCE_SHA='77d96def65f27c95817524dc2be25e2a0bcaf3e3a3035ff0be3828e818347a8c';
const START='        Dim doc117Observation = Doc117Alm03MethodObserver.Entrar(NombreGabinete, RutaArchivoAlmacenar, NombreDocumento, IdExpediente, IdTipoLogia, TipoAlmacenamiento, NombreClaseFormatoDocumento)';
const END='            doc117Observation.Salir(AlmacenamientoDocumentoProduccionDocumental, IdImagenAlamacenada, EstructuraDatosImagen.id_registro)';
function observeMethod(method){
 const newline=method.includes('\r\n')?'\r\n':'\n',first=method.indexOf('        Try'+newline),end=method.lastIndexOf('    End Function');
 if(first<0||end<first||!/^\s*Function AlmacenamientoDocumentoProduccionDocumental\(/.test(method)||method.includes('doc117Observation'))throw Error('DOC117_ALM03_METHOD_CHANGED');
 const prefix=START+newline+'        Try'+newline,suffix='        Finally'+newline+END+newline+'        End Try'+newline;
 return{overlay:method.slice(0,first)+prefix+method.slice(first,end)+suffix+method.slice(end),prefix,suffix,originalBody:method.slice(first,end)};
}
function overlay(source){
 if(sha(source)!==SOURCE_SHA)throw Error('DOC117_ALM03_STORAGE_SOURCE_CHANGED');
 const start=source.indexOf('    Function AlmacenamientoDocumentoProduccionDocumental('),end=source.indexOf('    End Function',start)+'    End Function'.length;
 if(start<0||end<start)throw Error('DOC117_ALM03_METHOD_CHANGED');
 const method=source.slice(start,end),r=observeMethod(method),result=source.slice(0,start)+r.overlay+source.slice(end);
 if(result.replace(r.prefix,'').replace(r.suffix,'')!==source)throw Error('DOC117_ALM03_UNRELATED_CHANGE');
 return{source:result,originalMethodSha256:sha(method),originalBodySha256:sha(r.originalBody),overlayMethodSha256:sha(r.overlay)};
}
function generate(directory){
 const dir=path.resolve(directory),relative=path.relative(path.join(ROOT,'.tmp'),dir);
 if(!relative||relative.startsWith('..')||path.isAbsolute(relative)||fs.existsSync(dir))throw Error('DOC117_ALM03_FRESH_DIRECTORY_REQUIRED');
 let parent=path.dirname(dir);while(parent!==ROOT){if(fs.existsSync(parent)&&fs.lstatSync(parent).isSymbolicLink())throw Error('DOC117_ALM03_PATH_INVALID');parent=path.dirname(parent);}
 const original=fs.readFileSync(path.join(ROOT,STORAGE)),r=overlay(original.toString('utf8'));
 fs.mkdirSync(dir,{recursive:true});
 const storage=path.join(dir,'ClassAlmacenamiento.Laboratory.vb'),helper=path.join(dir,'Doc117Alm03MethodObserver.vb'),targetsPath=path.join(dir,'laboratory-only.targets'),xml=s=>s.replaceAll('&','&amp;').replaceAll('"','&quot;').replaceAll('<','&lt;');
 fs.writeFileSync(storage,'\uFEFF'+r.source.replace(/^\uFEFF/,''),{flag:'wx'});fs.copyFileSync(path.join(ROOT,TEMPLATE),helper,fs.constants.COPYFILE_EXCL);
 fs.writeFileSync(targetsPath,`<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003"><Target Name="Doc117Alm03DormantOverlay" BeforeTargets="CoreCompile"><ItemGroup><Compile Remove="workflow\\ClassAlmacenamiento.vb" /><Compile Include="${xml(storage)}" /><Compile Include="${xml(helper)}" /></ItemGroup></Target></Project>`,{flag:'wx'});
 return{dir,targetsPath,originalMethodSha256:r.originalMethodSha256,originalBodySha256:r.originalBodySha256,overlayMethodSha256:r.overlayMethodSha256,sourceStorageSha256:sha(original),overlaySha256:sha(fs.readFileSync(storage)),helperSha256:sha(fs.readFileSync(helper))};
}
module.exports={ROOT,STORAGE,TEMPLATE,SOURCE_SHA,sha,observeMethod,overlay,generate};
