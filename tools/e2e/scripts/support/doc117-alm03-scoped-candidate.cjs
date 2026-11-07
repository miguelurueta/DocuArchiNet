'use strict';
const fs=require('node:fs'),path=require('node:path');
const {ROOT,sha,validateScope}=require('../../tests/support/doc117-alm03-setup-scope.cjs');
const dormant=require('./doc117-alm03-candidate.cjs');
const BINDING='Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorDocumentoImportacion.Legacy.vb',GUARD='tools/e2e/fixtures/Doc117Alm03ClosedGuard.vb';
const scopeDigest=s=>sha(JSON.stringify(validateScope(s)));
const resourceHashes=s=>[...s.resources,s.individual].map(r=>sha(JSON.stringify(r)));
const once=(source,from,to)=>{if(source.split(from).length!==2)throw Error('DOC117_ALM03_BINDING_CHANGED');return source.replace(from,()=>to);};
function renderGuard(scope){
 const s=validateScope(scope),hashes=resourceHashes(s),constants={Site:s.runtimeRoot,Run:s.runId,Lot:s.lotId,Snapshot:s.snapshotSha256,Cabinet:s.individual.cabinetName,IndividualName:s.individual.fileName,IndividualContent:s.individual.contentSha256,IndividualDigest:hashes[2]};
 for(let i=0;i<2;i++)for(const [key,field]of Object.entries({Root:'rootId',File:'fileId',Exp:'expedienteId',Rel:'relacionNivelId',Name:'fileName',Content:'contentSha256',Request:'requestSha256'}))constants[key+(i===0?'A':'B')]=s.resources[i][field];
 constants.DigestA=hashes[0];constants.DigestB=hashes[1];let source=fs.readFileSync(path.join(ROOT,GUARD),'utf8');
 for(const [key,value]of Object.entries(constants)){const pattern=new RegExp('(Private Const '+key+' As String = )"[^"]+"','g');if([...source.matchAll(pattern)].length!==1)throw Error('DOC117_ALM03_GUARD_CHANGED');source=source.replace(pattern,(_,prefix)=>prefix+'"'+value.replaceAll('"','""')+'"');}
 return '\uFEFF'+source.replace(/^\uFEFF/,'');
}
function generate(directory,scope,policy={}){
 const s=(policy.validateScope||validateScope)(scope),g=dormant.generate(directory),dir=g.dir,storage=path.join(dir,'ClassAlmacenamiento.Laboratory.vb'),observer=path.join(dir,'Doc117Alm03MethodObserver.vb'),guard=path.join(dir,'Doc117Alm03ClosedGuard.vb'),binding=path.join(dir,'AdaptadorDocumentoImportacion.Legacy.Laboratory.vb');
 let code=fs.readFileSync(storage,'utf8'),start='            Result = AlmacenamientoDocumentoProduccionDocumental(NombreGabinete,',finish='                                                                 EstructuraDatosImagen)';
 const pos=code.indexOf(start);if(pos<0||code.indexOf(start,pos+1)>=0)throw Error('DOC117_ALM03_INDIVIDUAL_CALLER_CHANGED');const end=code.indexOf(finish,pos)+finish.length;if(end<pos)throw Error('DOC117_ALM03_INDIVIDUAL_CALLER_CHANGED');
 code=code.slice(0,pos)+'            Using doc117Caller = Doc117Alm03ClosedGuard.PrepararIndividual(NombreGabinete, RutaArchivoAlmacenar, DescripcionTipo, IdExpediente, IdTipoLogia, TipoAlmacenamiento, NombreClaseFormatoDocumento)\r\n'+code.slice(pos,end)+'\r\n            End Using'+code.slice(end);fs.writeFileSync(storage,code);
 fs.writeFileSync(observer,once(fs.readFileSync(observer,'utf8'),'Throw New InvalidOperationException("DOC117_ALM03_DORMANT_SCOPE")','Return Doc117Alm03ClosedGuard.Entrar(gabinete, ruta, nombre, expediente, tipologia, tipo, clase)'));
 const original=fs.readFileSync(path.join(ROOT,BINDING),'utf8'),transformed=once(original,'        _invocar = AddressOf InvocarAlmacenamiento',`        _invocar = Sub(comando, datos, copia, salida)
                       Using doc117Caller = Doc117Alm03ClosedGuard.PrepararImportacion(comando, datos, copia)
                           InvocarAlmacenamiento(comando, datos, copia, salida)
                       End Using
                   End Sub`);
 fs.writeFileSync(binding,'\uFEFF'+transformed.replace(/^\uFEFF/,''),{flag:'wx'});fs.writeFileSync(guard,(policy.renderGuard||renderGuard)(s),{flag:'wx'});
 const xml=x=>x.replaceAll('&','&amp;').replaceAll('"','&quot;').replaceAll('<','&lt;');let targets=fs.readFileSync(g.targetsPath,'utf8');targets=once(targets,'</ItemGroup>',`<Compile Remove="${BINDING.replaceAll('/','\\')}" /><Compile Include="${xml(binding)}" /><Compile Include="${xml(guard)}" /></ItemGroup>`);fs.writeFileSync(g.targetsPath,targets);
 return{...g,resourceScopeSha256:(policy.scopeDigest||scopeDigest)(s),resourceHashes:(policy.resourceHashes||resourceHashes)(s),overlaySha256:sha(fs.readFileSync(storage)),helperSha256:sha(fs.readFileSync(observer)),guardSha256:sha(fs.readFileSync(guard)),sourceBindingSha256:sha(original),bindingSha256:sha(fs.readFileSync(binding))};
}
module.exports={ROOT,GUARD,BINDING,generate,scopeDigest,resourceHashes,renderGuard};
