'use strict';
const {createProfileAdministrator,equalProfileData:equal,normalizeProfileRules:rules}=require('./doc111-partial-profile.cjs');
const {createPreparationClient}=require('./doc114-plan-importacion.cjs');
const {validateProfile}=require('./doc111-importacion-carpetas.cjs');
const check=(ok,code)=>{if(!ok)throw Error('DOC114_'+code);};
const id=x=>typeof x==='string'&&/^[1-9][0-9]{0,9}$/.test(x)&&BigInt(x)<=4294967295n;
async function publishReferenceVersion(auth,profile,proposal,authorization,onReference=()=>{},runtime={}){
 validateProfile(profile);
 check(proposal?.authorized===true&&proposal.issueKey==='DOC-114'&&['environment','account','publication','discardable'].every(k=>authorization?.[k]===true)&&(!profile.ignoreHttpsErrors||authorization.tls===true),'B_VERSION_AUTHORIZATION_REQUIRED');
 check(profile.baseUrl==='https://localhost/GestionDocumental-Docuarchi.net/'&&profile.nivelRef==='7'&&proposal.source?.perfilId==='27'&&proposal.source.versionId==='41'&&proposal.source.codigo==='DOC114-FORMATO-B-20261103','B_VERSION_SCOPE_INVALID');
 check(equal(proposal.effects,{newProfiles:0,newPublishedVersions:1,maximumAdministrativeMutations:3,plans:0,documents:0,permissionChanges:0})&&equal(proposal.operations,['CrearVersion','GuardarBorrador','PublicarVersion']),'B_VERSION_EFFECTS_INVALID');
 check(equal(proposal.preserve,[{perfilId:'24',versionId:'38'},{perfilId:'25',versionId:'39'},{perfilId:'26',versionId:'40'},{perfilId:'27',versionId:'41'}]),'B_VERSION_PRESERVATION_INVALID');
 const reviewed=require('../../../../Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc114-perfiles-propuestos-20261103.json').profiles.find(p=>p.key==='format-b');
 const expected=structuredClone(reviewed.reglas);expected.find(r=>r.DestinoTipo==='TIPOLOGIA').TipoValor='REFERENCIA';
 check(equal(proposal.source.expectedRules,reviewed.reglas)&&equal(proposal.change?.rules,expected)&&equal(proposal.change.configurationChanges,[])&&equal(proposal.change.ruleChanges,[{destination:'TIPOLOGIA:TIPO_DOCUMENTAL',property:'TipoValor',before:'TEXTO',after:'REFERENCIA'}]),'B_VERSION_DELTA_INVALID');
 const admin=runtime.administrator||await createProfileAdministrator(auth,profile,runtime),before=[];
 for(const ref of proposal.preserve){
  const p=await admin.call('ObtenerPerfil',ref);
  check(p.PerfilId===ref.perfilId&&p.AmbitoRef==='2'&&p.Activo===true&&p.Version?.VersionId===ref.versionId&&p.Version.Estado==='PUBLICADA','B_VERSION_SOURCE_UNAVAILABLE');
  before.push(p);
 }
 const source=before[3],original=before[0];
 check(source.Codigo===proposal.source.codigo&&equal(rules(source.Version.Reglas),rules(reviewed.reglas))&&equal(source.Version.Configuracion,{...original.Version.Configuracion,...reviewed.configurationPatch}),'B_VERSION_SOURCE_CHANGED');
 check(Number.isInteger(source.Revision)&&source.Revision>0&&Number.isInteger(source.Version.Revision)&&source.Version.Revision>0,'B_VERSION_REVISION_INVALID');
 async function mutate(operation,payload){runtime.onAttempt?.({operation,perfilId:'27'});try{return await admin.call(operation,payload);}catch{throw Error('DOC114_B_VERSION_OPERATION_NOT_CONFIRMED');}}
 let p=await mutate('CrearVersion',{perfilId:'27',versionOrigenId:'41',revisionPerfilEsperada:source.Revision,revisionOrigenEsperada:source.Version.Revision});
 if(id(p?.PerfilId)&&id(p?.Version?.VersionId))onReference({perfilId:p.PerfilId,versionId:p.Version.VersionId});
 const versionId=p?.Version?.VersionId;
 function identity(){check(p?.PerfilId==='27'&&p.AmbitoRef==='2'&&p.Codigo===proposal.source.codigo&&p.Activo===true&&id(versionId)&&!proposal.preserve.some(r=>r.versionId===versionId)&&p.Version?.VersionId===versionId&&p.Version.PerfilId==='27'&&Number.isInteger(p.Revision)&&p.Revision>0&&Number.isInteger(p.Version.Revision)&&p.Version.Revision>0,'B_VERSION_IDENTITY_INVALID');}
 function content(){check(equal(p.Version.Configuracion,source.Version.Configuracion)&&equal(rules(p.Version.Reglas),rules(expected)),'B_VERSION_CONTENT_CHANGED');}
 identity();check(p.Version.Estado==='BORRADOR'&&equal(p.Version.Configuracion,source.Version.Configuracion)&&equal(rules(p.Version.Reglas),rules(source.Version.Reglas)),'B_VERSION_CLONE_CHANGED');
 p=await mutate('GuardarBorrador',{perfilId:'27',versionId,revisionVersionEsperada:p.Version.Revision,configuracion:structuredClone(source.Version.Configuracion),reglas:structuredClone(expected)});identity();content();check(p.Version.Estado==='BORRADOR','B_VERSION_DRAFT_UNCONFIRMED');
 p=await mutate('PublicarVersion',{perfilId:'27',versionId,revisionPerfilEsperada:p.Revision,revisionVersionEsperada:p.Version.Revision});identity();content();check(p.Version.Estado==='PUBLICADA','B_VERSION_PUBLICATION_UNCONFIRMED');
 p=await admin.call('ObtenerPerfil',{perfilId:'27',versionId});identity();content();check(p.Version.Estado==='PUBLICADA','B_VERSION_READBACK_UNCONFIRMED');
 for(const old of before){
  const actual=await admin.call('ObtenerPerfil',{perfilId:old.PerfilId,versionId:old.Version.VersionId});
  check(actual.PerfilId===old.PerfilId&&actual.Codigo===old.Codigo&&actual.AmbitoRef===old.AmbitoRef&&actual.Activo===old.Activo&&equal(actual.Version,old.Version),'B_VERSION_HISTORY_CHANGED');
 }
 const reader=runtime.reader||createPreparationClient(auth,profile,{environment:true,account:true,tls:authorization.tls,effects:false,discardable:false},()=>{},{readOnly:true});
 await reader.refreshToken();let page=await reader.request('AbrirImportacion',{nivelRef:'7'}),found=false;
 for(let index=1;index<=20;index++){
  check(Array.isArray(page.perfiles),'B_VERSION_CATALOG_INVALID');
  const snap=page.perfiles.find(s=>s.PerfilId==='27'&&s.VersionId===versionId);
  if(snap){check(equal(snap.Configuracion,source.Version.Configuracion)&&equal(rules(snap.Reglas),rules(expected)),'B_VERSION_OPERATIVE_CONTENT_CHANGED');found=true;break;}
  if(!(page.paginaPerfiles?.puedeHaberMas===true||page.puedeHaberMas===true))break;
  page=await reader.request('ListarPerfilesImportacion',{nivelRef:'7',pagina:index+1,tamanoPagina:50});
 }
 check(found,'B_VERSION_OPERATIVE_UNAVAILABLE');
 return {status:'PASS',perfilId:'27',versionId,sourceVersionId:'41',historyPreserved:true,sourceProfilesPreserved:true,plans:0,documents:0,permissionChanges:0};
}
module.exports={publishReferenceVersion};
