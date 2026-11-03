'use strict';
const {safePost}=require('./doc110-safe-request.cjs');
const {assertAuthorizations}=require('./doc111-importacion-carpetas.cjs');
const id=x=>typeof x==='string'&&/^[1-9][0-9]{0,9}$/.test(x)&&BigInt(x)<=4294967295n;
function check(value,code){if(!value)throw new Error('DOC111_'+code);}
function canonical(value){
  if(Array.isArray(value))return value.map(canonical);
  if(value&&typeof value==='object')return Object.fromEntries(Object.keys(value).sort().map(k=>[k,canonical(value[k])]));
  return value;
}
const equal=(a,b)=>JSON.stringify(canonical(a))===JSON.stringify(canonical(b));
const rules=value=>value.map(({ReglaId,...rule})=>{
  // Newtonsoft omite únicamente Proveedor nulo por atributo del modelo;
  // JavaScriptSerializer de ASMX sí lo incluye. Conservar toda otra diferencia.
  if(rule.Proveedor===null)delete rule.Proveedor;
  return rule;
});
function validatePartialProposal(p,profile){
  check(p?.authorized===true&&profile.mode==='PARCIAL'&&id(p.sourceProfileId)&&id(p.sourceVersionId)&&p.sourceVersionId===profile.versionId&&p.levelId===profile.nivelRef&&equal(p.rootNames,profile.rootNames)&&p.validPdfs===2&&p.rejectedManifestFiles===1,'PARTIAL_PROPOSAL_INVALID');
  check(typeof p.newProfileCode==='string'&&/^[A-Z0-9_-]{1,64}$/.test(p.newProfileCode)&&typeof p.newProfileName==='string'&&p.newProfileName.length>0&&p.newProfileName.length<=150,'PARTIAL_PROPOSAL_INVALID');
  check(p.configuracion?.ModoIntegridad==='PARCIAL_PERMITIDO'&&p.configuracion.AmbitoIntegridad==='NO_APLICA'&&String(p.configuracion.GabineteId)===p.cabinetId&&Array.isArray(p.reglas)&&p.reglas.length>0,'PARTIAL_PROPOSAL_INVALID');
  return p;
}
// Usa la sesión y CSRF administrativos existentes. Cada mutación se envía una
// sola vez; IDs nuevos se anuncian inmediatamente para revisar respuestas perdidas.
async function createProfileAdministrator(auth,profile,runtime={}){
  const host=auth.page.locator('#doc110-perfiles');
  check(await host.count()===1,'PARTIAL_ADMIN_UNAVAILABLE');
  let token=await host.getAttribute('data-token');
  // El editor consume y retira data-token al inicializarse. Reutilizar la
  // navegación oficial y observar su lectura, como las suites DOC-110 existentes.
  if(!token){
    try{
      const open=runtime.openAdministrator||require('./doc110-admin-navigation.cjs').openImportAdministrator;
      const response=await open(auth.page,'abrir','ListarPerfiles');
      check(response.status()===200,'PARTIAL_ADMIN_UNAVAILABLE');
      token=response.request().headers()['x-importacion-perfil-csrf'];
    }catch{throw new Error('DOC111_PARTIAL_ADMIN_UNAVAILABLE');}
  }
  check(typeof token==='string'&&token.length>0,'PARTIAL_ADMIN_UNAVAILABLE');
  async function call(operation,solicitud){
    check(['ListarPerfiles','ObtenerPerfil','CrearPerfil','CrearVersion','GuardarBorrador','PublicarVersion','CambiarDisponibilidad'].includes(operation),'PARTIAL_OPERATION_INVALID');
    let response,body;
    try{
      response=await safePost(auth.context.request,new URL('webservice/WebServiceImportacionCarpetas.asmx/'+operation,profile.baseUrl).href,{maxRedirects:0,headers:{'X-Importacion-Perfil-CSRF':token,Origin:new URL(profile.baseUrl).origin},data:{solicitud}},operation);
      const raw=await response.json();body=raw?.d??raw;
    }catch{throw new Error('DOC111_PARTIAL_PROFILE_NOT_CONFIRMED');}
    check(response.status()===200&&body?.ok===true&&body.datos,'PARTIAL_PROFILE_NOT_CONFIRMED');
    return body.datos;
  }
  return {call};
}
async function preparePartialProfile(auth,profile,proposal,authorization,onReference=()=>{},runtime={}){
  assertAuthorizations(authorization);validatePartialProposal(proposal,profile);
  const {call}=await createProfileAdministrator(auth,profile,runtime);
  const source=await call('ObtenerPerfil',{perfilId:proposal.sourceProfileId,versionId:proposal.sourceVersionId});
  check(source.PerfilId===proposal.sourceProfileId&&source.Version?.VersionId===proposal.sourceVersionId&&source.Version.Estado==='PUBLICADA','PARTIAL_SOURCE_CHANGED');
  const configuration={...source.Version.Configuracion,ModoIntegridad:'PARCIAL_PERMITIDO',AmbitoIntegridad:'NO_APLICA'};
  check(equal(configuration,proposal.configuracion)&&equal(rules(source.Version.Reglas),rules(proposal.reglas)),'PARTIAL_SOURCE_CHANGED');
  let p=await call('CrearPerfil',{codigo:proposal.newProfileCode,nombre:proposal.newProfileName,descripcion:'Perfil de pruebas PARCIAL autorizado para DOC-111; conservar recursos.'});
  check(id(p.PerfilId)&&p.PerfilId!==proposal.sourceProfileId&&id(p.Version?.VersionId)&&p.Version.PerfilId===p.PerfilId&&p.Version.Estado==='BORRADOR','PARTIAL_PROFILE_NOT_CONFIRMED');
  const createdId=p.PerfilId,versionId=p.Version.VersionId;
  onReference({perfilId:createdId,versionId});
  function identity(){check(p.PerfilId===createdId&&p.Version?.VersionId===versionId&&p.Version.PerfilId===createdId,'PARTIAL_PROFILE_NOT_CONFIRMED');}
  p=await call('GuardarBorrador',{perfilId:createdId,versionId,revisionVersionEsperada:p.Version.Revision,configuracion:proposal.configuracion,reglas:proposal.reglas.map(r=>({...r,ReglaId:null}))});identity();
  check(equal(p.Version.Configuracion,proposal.configuracion)&&equal(rules(p.Version.Reglas),rules(proposal.reglas)),'PARTIAL_PROFILE_NOT_CONFIRMED');
  p=await call('PublicarVersion',{perfilId:createdId,versionId,revisionPerfilEsperada:p.Revision,revisionVersionEsperada:p.Version.Revision});identity();
  check(p.Version.Estado==='PUBLICADA'&&equal(p.Version.Configuracion,proposal.configuracion)&&equal(rules(p.Version.Reglas),rules(proposal.reglas)),'PARTIAL_PROFILE_NOT_CONFIRMED');
  if(p.Activo!==true){p=await call('CambiarDisponibilidad',{perfilId:createdId,revisionPerfilEsperada:p.Revision,activo:true});identity();}
  check(p.Activo===true,'PARTIAL_PROFILE_NOT_CONFIRMED');
  return {...profile,versionId};
}
module.exports={validatePartialProposal,preparePartialProfile,createProfileAdministrator,equalProfileData:equal,normalizeProfileRules:rules};
