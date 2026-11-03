'use strict';
const crypto=require('node:crypto');
const {validateProfile}=require('./doc111-importacion-carpetas.cjs');
const {createPreparationClient}=require('./doc114-plan-importacion.cjs');
const {createProfileAdministrator,equalProfileData:equal,normalizeProfileRules:rules}=require('./doc111-partial-profile.cjs');
const check=(ok,code)=>{if(!ok)throw Error('DOC114_'+code);};
const id=x=>typeof x==='string'&&/^[1-9][0-9]{0,9}$/.test(x)&&BigInt(x)<=4294967295n;
const sha=x=>crypto.createHash('sha256').update(JSON.stringify(x)).digest('hex');
function validatePublicationProposal(proposal,profile,authorization){
  validateProfile(profile);
  check(proposal?.issueKey==='DOC-114'&&proposal.authorized===true&&['environment','account','publication','discardable'].every(k=>authorization?.[k]===true)&&(!profile.ignoreHttpsErrors||authorization.tls===true),'PROFILE_PUBLICATION_AUTHORIZATION_REQUIRED');
  check(proposal.levelId===profile.nivelRef&&proposal.cabinetId==='10'&&proposal.source?.perfilId==='24'&&proposal.source.versionId===profile.versionId&&/^[a-f0-9]{64}$/.test(proposal.source.snapshotSha256),'PROFILE_SOURCE_SCOPE_INVALID');
  check(proposal.maximumEffects?.newProfiles===2&&proposal.maximumEffects.newPublishedVersions===2&&['newPlans','newLots','permissionChanges','documents'].every(k=>proposal.maximumEffects[k]===0),'PROFILE_EFFECTS_SCOPE_INVALID');
  check(Array.isArray(proposal.profiles)&&proposal.profiles.length===2&&new Set(proposal.profiles.map(p=>p.codigo)).size===2,'PROFILE_COUNT_INVALID');
  check(Array.isArray(proposal.preservePublications)&&proposal.preservePublications.length===2&&proposal.preservePublications.some(p=>p.perfilId==='24'&&p.versionId==='38'&&p.snapshotSha256===proposal.source.snapshotSha256)&&proposal.preservePublications.some(p=>p.perfilId==='25'&&p.versionId==='39'&&/^[a-f0-9]{64}$/.test(p.snapshotSha256)),'PROFILE_PRESERVATION_REQUIRED');
  for(const [i,p]of proposal.profiles.entries()){
    check(p.key===['format-a','format-b'][i]&&p.codigo===['DOC114-FORMATO-A-20261103','DOC114-FORMATO-B-20261103'][i]&&typeof p.nombre==='string'&&p.nombre.length>0&&p.nombre.length<=150,'PROFILE_NAME_SCOPE_INVALID');
    check(equal(Object.keys(p.configurationPatch).sort(),['AmbitoIntegridad','ModoIntegridad','NombreVisible'])&&p.configurationPatch.ModoIntegridad==='COMPLETO_OBLIGATORIO'&&p.configurationPatch.AmbitoIntegridad==='EXPEDIENTE'&&p.configurationPatch.NombreVisible===p.nombre,'PROFILE_CONFIGURATION_SCOPE_INVALID');
    check(Array.isArray(p.reglas)&&p.reglas.length===5&&p.reglas.every(r=>r.ReglaId===null&&r.ValorConstante===null&&r.FuenteTipo!=='PROVEEDOR')&&new Set(p.reglas.map(r=>r.DestinoTipo+':'+r.DestinoRef)).size===5,'PROFILE_RULES_SCOPE_INVALID');
    const find=(type,ref)=>p.reglas.find(r=>r.DestinoTipo===type&&r.DestinoRef===ref),code=find('CAMPO_EXPEDIENTE','CODIGO'),name=find('CAMPO_EXPEDIENTE','NOMBRE'),date=find('CAMPO_EXPEDIENTE','FECHA_INICIAL'),type=find('TIPOLOGIA','TIPO_DOCUMENTAL'),deferred=find('INDICE_GABINETE','14');
    check(code?.FuenteTipo==='CARPETA'&&code.Extraccion==='SEGMENTO'&&code.Separador===['-','#'][i]&&code.Posicion===0&&code.LongitudMaxima===45&&name?.FuenteTipo==='CARPETA'&&name.Extraccion==='COMPLETO','PROFILE_FOLDER_RULES_INVALID');
    check(date?.FuenteTipo==='CARPETA'&&date.Extraccion==='SEGMENTO'&&date.Separador===['-','#'][i]&&date.Posicion===1&&date.TipoValor==='FECHA'&&date.FormatoFecha===['yyyyMMdd','dd-MM-yyyy'][i],'PROFILE_DATE_RULE_INVALID');
    check(type?.FuenteTipo==='ARCHIVO'&&type.TipoValor===['REFERENCIA','TEXTO'][i]&&type.QuitarExtension===true&&(i===0?type.Extraccion==='SEGMENTO'&&type.Separador==='-'&&type.Posicion===1:type.Extraccion==='COMPLETO'),'PROFILE_TYPE_RULE_INVALID');
    check(deferred?.FuenteTipo==='EXPEDIENTE'&&deferred.PropiedadExpediente==='ID'&&deferred.EtapaResolucion==='TRAS_EXPEDIENTE'&&deferred.TipoValor==='TEXTO'&&deferred.LongitudMaxima===20,'PROFILE_DEFERRED_RULE_INVALID');
  }
  return proposal;
}
async function publishTestProfiles(auth,profile,proposal,authorization,onReference=()=>{},runtime={}){
  validatePublicationProposal(proposal,profile,authorization);
  const reader=runtime.reader||createPreparationClient(auth,profile,{environment:authorization.environment,account:authorization.account,tls:authorization.tls,effects:false,discardable:false},()=>{},{readOnly:true});
  await reader.refreshToken();
  async function publications(){
    let result=await reader.request('AbrirImportacion',{nivelRef:profile.nivelRef}),more=result.paginaPerfiles?.puedeHaberMas===true;
    const snapshots=new Map();
    for(let page=1;page<=20;page++){
      check(Array.isArray(result.perfiles),'PROFILE_CATALOG_INVALID');
      for(const snapshot of result.perfiles)snapshots.set(String(snapshot.VersionId),snapshot);
      if(!more)return snapshots;
      check(page<20,'PROFILE_CATALOG_BUDGET_EXCEEDED');
      result=await reader.request('ListarPerfilesImportacion',{nivelRef:profile.nivelRef,pagina:page+1,tamanoPagina:50});more=result.puedeHaberMas===true;
    }
  }
  function preserve(snapshots){for(const expected of proposal.preservePublications){const s=snapshots.get(expected.versionId);check(s?.PerfilId===expected.perfilId&&sha(s)===expected.snapshotSha256,'PROFILE_SOURCE_CHANGED');}}
  const before=await publications();preserve(before);
  const source=before.get(proposal.source.versionId);check(String(source.Configuracion.GabineteId)===proposal.cabinetId,'PROFILE_CABINET_CHANGED');
  const admin=runtime.administrator||await createProfileAdministrator(auth,profile,runtime);
  const original=await admin.call('ObtenerPerfil',{perfilId:proposal.source.perfilId,versionId:proposal.source.versionId});
  check(original.PerfilId===proposal.source.perfilId&&id(original.AmbitoRef)&&original.Version?.VersionId===proposal.source.versionId&&original.Version.Estado==='PUBLICADA'&&equal(original.Version.Configuracion,source.Configuracion)&&equal(rules(original.Version.Reglas),rules(source.Reglas)),'PROFILE_ADMIN_SOURCE_CHANGED');
  for(const p of proposal.profiles){const page=await admin.call('ListarPerfiles',{codigo:p.codigo,pagina:1,tamanoPagina:2});check(page.Total===0&&Array.isArray(page.Elementos)&&page.Elementos.length===0,'PROFILE_NAME_ALREADY_PRESENT_OR_UNVERIFIED');}
  const created=[];
  async function mutate(operation,payload,key){if(runtime.onAttempt)runtime.onAttempt({key,operation});try{return await admin.call(operation,payload);}catch{throw Error('DOC114_PROFILE_OPERATION_NOT_CONFIRMED');}}
  for(const item of proposal.profiles){
    const config={...structuredClone(source.Configuracion),...item.configurationPatch};
    let p=await mutate('CrearPerfil',{codigo:item.codigo,nombre:item.nombre,descripcion:'Recursos de prueba DOC-114 autorizados; conservar perfiles y publicaciones.'},item.key);
    if(id(p?.PerfilId)&&id(p?.Version?.VersionId))onReference({key:item.key,perfilId:p.PerfilId,versionId:p.Version.VersionId});
    const perfilId=p?.PerfilId,versionId=p?.Version?.VersionId;
    function identity(){check(id(perfilId)&&id(versionId)&&p?.PerfilId===perfilId&&p.Codigo===item.codigo&&p.AmbitoRef===original.AmbitoRef&&p.Version?.VersionId===versionId&&p.Version.PerfilId===perfilId&&Number.isInteger(p.Revision)&&p.Revision>0&&Number.isInteger(p.Version.Revision)&&p.Version.Revision>0&&!proposal.preservePublications.some(o=>o.perfilId===perfilId||o.versionId===versionId)&&!created.some(o=>o.perfilId===perfilId||o.versionId===versionId),'PROFILE_CREATED_IDENTITY_INVALID');}
    identity();check(p.Version.Estado==='BORRADOR','PROFILE_DRAFT_NOT_CONFIRMED');
    p=await mutate('GuardarBorrador',{perfilId,versionId,revisionVersionEsperada:p.Version.Revision,configuracion:config,reglas:structuredClone(item.reglas)},item.key);identity();
    check(equal(p.Version.Configuracion,config)&&equal(rules(p.Version.Reglas),rules(item.reglas)),'PROFILE_DRAFT_CONTENT_CHANGED');
    p=await mutate('PublicarVersion',{perfilId,versionId,revisionPerfilEsperada:p.Revision,revisionVersionEsperada:p.Version.Revision},item.key);identity();
    check(p.Version.Estado==='PUBLICADA'&&equal(p.Version.Configuracion,config)&&equal(rules(p.Version.Reglas),rules(item.reglas)),'PROFILE_PUBLICATION_NOT_VERIFIED');
    if(p.Activo!==true){p=await mutate('CambiarDisponibilidad',{perfilId,revisionPerfilEsperada:p.Revision,activo:true},item.key);identity();}
    const readBack=await admin.call('ObtenerPerfil',{perfilId,versionId});
    check(readBack.PerfilId===perfilId&&readBack.Codigo===item.codigo&&readBack.AmbitoRef===original.AmbitoRef&&readBack.Version?.VersionId===versionId&&readBack.Version.PerfilId===perfilId&&readBack.Version.Estado==='PUBLICADA'&&readBack.Activo===true&&equal(readBack.Version.Configuracion,config)&&equal(rules(readBack.Version.Reglas),rules(item.reglas)),'PROFILE_READBACK_NOT_VERIFIED');
    created.push({key:item.key,perfilId,versionId});
  }
  const after=await publications();preserve(after);
  for(const record of created){const actual=after.get(record.versionId),item=proposal.profiles.find(p=>p.key===record.key);check(actual?.PerfilId===record.perfilId&&equal(actual.Configuracion,{...source.Configuracion,...item.configurationPatch})&&equal(rules(actual.Reglas),rules(item.reglas)),'PROFILE_OPERATIVE_PUBLICATION_UNAVAILABLE');}
  return {status:'PASS',profiles:created,sourcePreserved:true,plans:0,documents:0,permissionChanges:0};
}
module.exports={validatePublicationProposal,publishTestProfiles};
