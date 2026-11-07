'use strict';
const {safePost}=require('./doc110-safe-request.cjs');
const {validateSession,assertInitialSnapshot,createCreationControls,selectProductionLevel}=require('./doc116-expedientes-importacion.cjs');
const {hash}=require('./doc116-exp01-setup.cjs');
const check=(v,c)=>{if(!v)throw Error('DOC116_INTERNAL_'+c);};
const id=x=>typeof x==='string'&&/^[1-9][0-9]{0,9}$/.test(x)&&BigInt(x)<=4294967295n;
function validateScope(m){
  check(m?.scenario==='EXP-02-INTERNAL'&&m.lotId==='18'&&m.afterCommitRoot==='29'&&m.beforeCheckpointRoot==='30'&&m.maximumCreationRequests===3&&m.maximumNewExpedients===2,'SCOPE_INVALID');
  validateSession(m.session);check(m.session.scenario==='EXP-02'&&m.session.lotId===m.lotId&&m.session.identity.userRef==='141'&&m.session.identity.ambitoRef==='2'&&m.session.profile.nivelRef==='215'&&m.session.profile.versionId==='38'&&m.session.roots.map(r=>r.rootId).join(',')==='29,30','SCOPE_INVALID');return m;
}
function validateMarker(marker,m,root,mode){
  check(marker?.modo===mode&&marker.loteId===m.lotId&&marker.raizId===root&&marker.estado==='CONFIRMADO'&&id(marker.intentoId)&&marker.creacion?.IntentoId===marker.intentoId&&marker.creacion.CicloArchivoNombre==='Archivo Gestión'&&id(marker.creacion.CicloArchivoId),'MARKER_INVALID');
  const x=marker.expediente;check(x?.OrigenLogico==='RA'&&x.NivelRef==='215'&&id(x.ExpedienteId)&&id(x.RelacionNivelId),'MARKER_IDENTITY_INVALID');return {loteId:m.lotId,raizId:root,intentoId:marker.intentoId,expedienteId:x.ExpedienteId,relacionNivelId:x.RelacionNivelId};
}
async function verifyPhysical(query,m,view,index,marker){
  const identity=marker.expediente,config=JSON.parse(view.lot.config_doc).Configuracion,expected=m.session.profile.expectations[index];
  const one=rows=>{check(Array.isArray(rows)&&rows.length===1,'CONTROL_NOT_UNIQUE');return rows[0];};
  const row=one(await query('RA','SELECT e.*,r.ra_pro_niveles_id_nivel AS nivel FROM expediente_archivo e INNER JOIN ra_pro_niveles_has_expediente_archivo r ON r.expediente_archivo_ID_EXPEDIENTE=e.ID_EXPEDIENTE WHERE e.ID_EXPEDIENTE=? AND r.id_registro=?',[identity.ExpedienteId,identity.RelacionNivelId]));
  check(String(row.nivel)==='215'&&String(row.ID_USUARIO_GESTION)==='141'&&String(row.ID_EMPRESA_EXPEDIENTE)==='2'&&row.CODIGO_UNICO===expected.codigo&&row.ALEAS_EXPEDIENTE===expected.nombre&&row.TEMA_EXPEDIENTE===expected.nombre&&String(row.Id_tipos_ciclo_archivo)===marker.creacion.CicloArchivoId,'PHYSICAL_IDENTITY_INVALID');
  for(const [column,key]of [['ID_FONDO','FondoRef'],['id_instrumento','InstrumentoRef'],['CODIGO_AREA_TRD','AreaRef'],['CODIGO_SERIE_TRD','SerieRef'],['CODIGO_SUB_SERIE_TRD','SubserieRef'],['RA_TIP_EXPE_ID_TIPO_EXPEDIENTE','TipoExpedienteRef'],['ID_TIPO_UNIDAD_DOCUMENTAL','UnidadDocumentalRef']])check((row[column]==null?null:String(row[column]))===(config[key]??null),'CLASSIFICATION_CHANGED');
  const cycle=one(await query('RA','SELECT Nombre_Tipo_ciclo_archivo AS nombre FROM ra_de_tipos_ciclos_archivo WHERE Id_tipos_ciclo_archivo=?',[marker.creacion.CicloArchivoId]));check(cycle.nombre==='Archivo Gestión','CYCLE_CHANGED');
  const cabinet=one(await query('DA','SELECT NOMBRE FROM system1 WHERE id_gabinete=?',[config.GabineteId]));check(/^[A-Za-z_][A-Za-z0-9_]{0,63}$/.test(cabinet.NOMBRE),'CABINET_INVALID');
  for(const [origin,sql]of [['DA','SELECT COUNT(*) AS total FROM `'+cabinet.NOMBRE+'` WHERE ID_EXPEDIENTE=?'],['RA','SELECT COUNT(*) AS total FROM registro_producion_documental WHERE EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE=?'],['RA','SELECT COUNT(*) AS total FROM ra_cert_indice_expediente WHERE expediente_archivo_ID_EXPEDIENTE=?']])check(String(one(await query(origin,sql,[identity.ExpedienteId])).total)==='0','DOCUMENT_EFFECT_FOUND');
}
async function runInternalFault(auth,manifest,query,readMarker,onStep=()=>{},runtime={}){
  const m=validateScope(manifest),controls=runtime.controls||createCreationControls(query,m.session),before=await controls.snapshot();assertInitialSnapshot(before,m.session);const countsBefore=await controls.levelCounts();
  await (runtime.selectLevel||selectProductionLevel)(auth.page,'215');const token=await auth.page.locator('#doc111-importacion').getAttribute('data-token');check(typeof token==='string'&&token.length>0,'TOKEN_REQUIRED');
  let requests=0;const refs=[];
  async function send(root){check(requests<3,'BUDGET_EXHAUSTED');requests++;onStep({stage:'CREATION_REQUESTED',rootId:root.rootId,requestNumber:requests});
    let response,body;try{response=await safePost(auth.context.request,new URL('webservice/WebServiceImportacionCarpetas.asmx/AvanzarLote',m.session.profile.baseUrl).href,{maxRedirects:0,headers:{'X-Importacion-Carpetas-CSRF':token,Origin:new URL(m.session.profile.baseUrl).origin},data:{solicitud:{loteId:'18',recursoId:root.rootId,fase:'CREAR_EXPEDIENTE',revisionEsperada:root.revision,claveOperacion:root.key}}},'AvanzarLote');const raw=await response.json();body=raw?.d??raw;}catch{throw Error('DOC116_INTERNAL_RESPONSE_UNCERTAIN');}
    const x=body?.datos;if(x?.loteId==='18'&&x.raizId===root.rootId&&id(x.expedienteId)&&id(x.relacionNivelId)){const ref={loteId:'18',raizId:root.rootId,expedienteId:x.expedienteId,relacionNivelId:x.relacionNivelId};refs.push(ref);onStep({stage:'KNOWN_IDENTITIES',...ref});}
    return {status:response.status(),body};
  }
  const first=m.session.roots[0],last=m.session.roots[1];
  const r1=await send(first),mark1=await readMarker(first.rootId),proof1=validateMarker(mark1,m,first.rootId,'AFTER_CHECKPOINT_COMMIT_BEFORE_RETURN');
  check(r1.status===200&&r1.body?.ok===true&&r1.body.codigo==='OK'&&r1.body.datos?.estado==='CONFIRMADO'&&r1.body.datos.expedienteId===proof1.expedienteId&&r1.body.datos.relacionNivelId===proof1.relacionNivelId,'POSTCOMMIT_RESULT_INVALID');
  const confirmed=await controls.snapshot();await controls.verifyRoot(confirmed,0,proof1);onStep({stage:'POSTCOMMIT_CHECKPOINT_VERIFIED',...proof1});
  const replay=await send(first);check(replay.status===200&&replay.body?.datos?.estado==='CONFIRMADO'&&replay.body.datos.expedienteId===proof1.expedienteId&&replay.body.datos.relacionNivelId===proof1.relacionNivelId,'CONFIRMED_REPLAY_CHANGED');
  check(hash(confirmed)===hash(await controls.snapshot()),'CONFIRMED_REPLAY_CHANGED');
  const r2=await send(last),mark2=await readMarker(last.rootId),proof2=validateMarker(mark2,m,last.rootId,'AFTER_LEGACY_BEFORE_CHECKPOINT');
  check(r2.status===409&&r2.body?.ok===false&&r2.body.codigo==='RESULTADO_INCIERTO'&&r2.body.datos?.estado==='INCIERTO'&&r2.body.datos.expedienteId===proof2.expedienteId&&r2.body.datos.relacionNivelId===proof2.relacionNivelId,'PRECHECKPOINT_RESULT_INVALID');
  check(proof1.expedienteId!==proof2.expedienteId&&proof1.relacionNivelId!==proof2.relacionNivelId,'IDENTITY_COLLISION');
  const after=await controls.snapshot(),root=after.roots.find(r=>String(r.raiz_id)===last.rootId),attempt=after.attempts.find(i=>String(i.intento_id)===proof2.intentoId);
  check(root&&root.estado==='CREANDO'&&!root.identidad_expediente_doc&&String(root.intento_activo_id)===proof2.intentoId&&attempt?.estado==='EN_CURSO'&&attempt.fase==='CREAR_EXPEDIENTE'&&!attempt.fin_utc,'UNCERTAINTY_NOT_RETAINED');
  const persisted=JSON.parse(attempt.evidencia_doc);check(!persisted.Expediente,'UNEXPECTED_CHECKPOINT');check(!after.events.some(e=>String(e.intento_id)===proof2.intentoId&&e.accion==='RESULTADO_CREACION'),'UNEXPECTED_CHECKPOINT');
  await (runtime.verifyPhysical||verifyPhysical)(query,m,after,1,mark2);
  const countsAfter=await controls.levelCounts();check(BigInt(countsAfter[0])===BigInt(countsBefore[0])+2n&&countsAfter[1]===countsBefore[1]&&hash(before.files)===hash(after.files),'UNEXPECTED_EFFECTS');
  check(after.attempts.filter(i=>i.fase==='CREAR_EXPEDIENTE').length===2&&refs.length===3&&requests===3,'UNEXPECTED_EFFECTS');
  onStep({stage:'PRECHECKPOINT_UNCERTAINTY_VERIFIED',...proof2});
  return {scenario:'EXP-02-INTERNAL',status:'PASS',laboratoryVariant:true,afterCommitReturnFailureVerified:true,preCheckpointFailureVerified:true,confirmedReplayUnchanged:true,uncertainCreationNotRepeated:true,persistedUncertainAttemptState:'EN_CURSO',rootsCreated:2,creationRequests:3,documents:0,permissionChanges:0,preparations:0,confirmations:0,uploads:0,references:[proof1,proof2]};
}
module.exports={validateScope,validateMarker,verifyPhysical,runInternalFault};
