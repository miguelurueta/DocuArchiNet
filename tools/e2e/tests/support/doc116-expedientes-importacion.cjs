'use strict';
const {safePost}=require('./doc110-safe-request.cjs');
const {validateProfile,createSelectControls}=require('./doc111-importacion-carpetas.cjs');
const fixture=require('../../fixtures/doc116-expedientes-cases.json');
const id=v=>typeof v==='string'&&/^[1-9][0-9]{0,9}$/.test(v)&&BigInt(v)<=4294967295n;
const check=(v,c)=>{if(!v)throw Error('DOC116_'+c);};
const closed=(v,keys)=>check(v&&typeof v==='object'&&!Array.isArray(v)&&Object.keys(v).every(k=>keys.includes(k)),'FIELDS_INVALID');
function validateSession(s){
  closed(s,['scenario','profile','identity','levelB','lotId','roots','runtimeRoot','levelPermission']);check(Object.hasOwn(fixture.scenarios,s.scenario),'SCENARIO_INVALID');validateProfile(s.profile);
  if(s.levelPermission!==undefined){closed(s.levelPermission,['permissionId','environmentHash']);check(s.scenario==='EXP-03'&&id(s.levelPermission.permissionId)&&typeof s.levelPermission.environmentHash==='string'&&/^[a-f0-9]{64}$/.test(s.levelPermission.environmentHash),'LEVEL_PERMISSION_RESOURCES_INVALID');}
  if(s.runtimeRoot!==undefined)check(typeof s.runtimeRoot==='string'&&require('node:path').isAbsolute(s.runtimeRoot),'RUNTIME_ROOT_INVALID');
  closed(s.identity,['userRef','ambitoRef']);check(id(s.identity.userRef)&&id(s.identity.ambitoRef),'IDENTITY_INVALID');
  check(id(s.levelB)&&s.levelB!==s.profile.nivelRef&&id(s.lotId),'DESTINATION_INVALID');
  check(Array.isArray(s.roots)&&s.roots.length===2&&new Set(s.roots.map(r=>r.rootId)).size===2,'ROOTS_INVALID');
  for(const r of s.roots){closed(r,['rootId','revision','key']);check(id(r.rootId)&&Number.isInteger(r.revision)&&r.revision>0&&r.revision<4294967294&&typeof r.key==='string'&&/^[A-Za-z0-9_-]{1,64}$/.test(r.key),'ROOT_INVALID');}
  return s;
}
function authorize(s,a){validateSession(s);check(a&&['environment','account','select','effects','discardable'].every(k=>a[k]===true)&&(!s.profile.ignoreHttpsErrors||a.tls===true),'AUTHORIZATION_REQUIRED');}
function assertLevelGrant(row,identity,level,granted=true){
  check(row&&String(row.nivel)===level&&String(row.actor)===identity.userRef&&String(row.empresa)===identity.ambitoRef,'LEVEL_CONTEXT_CHANGED');
  check(String(row.propietario)!==identity.userRef,'LEVEL_OWNER_BYPASS');
  check(id(String(row.propietario))&&String(row.nivel_activo)==='1'&&String(row.estado_permiso)==='1'&&String(row.agregar_expediente)===(granted?'1':'0'),'LEVEL_GRANT_NOT_EFFECTIVE');
  return {level,actor:identity.userRef,ownerBypass:false,granted};
}
function createLevelControls(query,session){
  const s=validateSession(session);
  return {async grant(granted=true){
    let rows;try{rows=await query('RA','SELECT n.id_nivel AS nivel,n.remit_dest_interno_id_Remit_Dest_Int AS propietario,n.estado_nivel AS nivel_activo,u.Empresa_Gestion_Documental_id_empresa AS empresa,p.remit_dest_interno_id_Remit_Dest_Int AS actor,p.id_permisos_niveles AS permission_id,p.estado_permiso,p.agregar_expediente FROM ra_pro_niveles n INNER JOIN remit_dest_interno u ON u.id_Remit_Dest_Int=n.remit_dest_interno_id_Remit_Dest_Int INNER JOIN ra_pro_permisos_niveles p ON p.ra_pro_niveles_id_nivel=n.id_nivel WHERE n.id_nivel=? AND p.remit_dest_interno_id_Remit_Dest_Int=? AND u.Empresa_Gestion_Documental_id_empresa=? LIMIT 2',[s.profile.nivelRef,s.identity.userRef,s.identity.ambitoRef]);}catch{throw Error('DOC116_LEVEL_SELECT_FAILED');}
    check(Array.isArray(rows)&&rows.length===1,'LEVEL_GRANT_NOT_UNIQUE');if(s.levelPermission)check(String(rows[0].permission_id)===s.levelPermission.permissionId,'LEVEL_PERMISSION_ROW_CHANGED');return assertLevelGrant(rows[0],s.identity,s.profile.nivelRef,granted);
  }};
}
const PUBLIC_FAILURES=new Set(['FIELDS_INVALID','SCENARIO_INVALID','IDENTITY_INVALID','DESTINATION_INVALID','ROOTS_INVALID','ROOT_INVALID','AUTHORIZATION_REQUIRED','LEVEL_CONTEXT_CHANGED','LEVEL_OWNER_BYPASS','LEVEL_GRANT_NOT_EFFECTIVE','LEVEL_SELECT_FAILED','LEVEL_GRANT_NOT_UNIQUE','TOKEN_REQUIRED','RESPONSE_INVALID','OPTIONS_INVALID','OBSERVATION_NOT_AUTHORIZED','UNCERTAINTY_REQUIRES_REVIEW','ROOT_NOT_AUTHORIZED','CREATION_BUDGET_EXHAUSTED','CREATION_NOT_CONFIRMED','UNEXPECTED_CREATION','REPLAY_CHANGED_IDENTITY','ROOT_IDENTITY_COLLISION','CREATED_ROOT_BUDGET_EXCEEDED','DESTINATION_CHANGED'].map(c=>'DOC116_'+c));
function safeFailureCode(error){return PUBLIC_FAILURES.has(error?.message)?error.message:'DOC116_OPERATION_NOT_CONFIRMED';}
function createCreationClient(auth,session,authorization,onReference=()=>{}){
  authorize(session,authorization);const s=JSON.parse(JSON.stringify(session)),budget=fixture.scenarios[s.scenario];
  let token,requests=0,blocked=false;const creations=new Map();
  const headers=()=>{check(typeof token==='string'&&token.length>0,'TOKEN_REQUIRED');return {'X-Importacion-Carpetas-CSRF':token,Origin:new URL(s.profile.baseUrl).origin};};
  async function refreshToken(){token=await auth.page.locator('#doc111-importacion').getAttribute('data-token');check(typeof token==='string'&&token.length>0,'TOKEN_REQUIRED');}
  async function send(operation,payload){
    let response,body;
    try{response=await safePost(auth.context.request,new URL('webservice/WebServiceImportacionCarpetas.asmx/'+operation,s.profile.baseUrl).href,{maxRedirects:0,headers:headers(),data:{solicitud:payload}},operation);const raw=await response.json();body=raw?.d??raw;}catch{throw Error('DOC116_OPERATION_NOT_CONFIRMED');}
    check(body&&typeof body.ok==='boolean'&&typeof response.status==='function','RESPONSE_INVALID');return {status:response.status(),body};
  }
  async function create(rootId,options={}){
    closed(options,['discardClientResult']);check(options.discardClientResult===undefined||typeof options.discardClientResult==='boolean','OPTIONS_INVALID');
    check(!options.discardClientResult||s.scenario==='EXP-02','OBSERVATION_NOT_AUTHORIZED');
    check(!blocked,'UNCERTAINTY_REQUIRES_REVIEW');const r=s.roots.find(x=>x.rootId===rootId);check(r,'ROOT_NOT_AUTHORIZED');
    check(requests<budget.maximumCreationRequests,'CREATION_BUDGET_EXHAUSTED');
    // A previous uncertain result can never be unlocked by merely consulting a page.
    requests++;blocked=true;
    const result=await send('AvanzarLote',{loteId:s.lotId,recursoId:r.rootId,fase:'CREAR_EXPEDIENTE',revisionEsperada:r.revision,claveOperacion:r.key});
    if(!result.body.ok){check(result.status===403&&result.body.codigo==='ACCESO_DENEGADO'&&s.scenario==='EXP-03','CREATION_NOT_CONFIRMED');return {denied:true};}
    const x=result.body.datos;
    check(result.status===200&&result.body.codigo==='OK'&&x?.loteId===s.lotId&&x.raizId===r.rootId&&x.fase==='CREAR_EXPEDIENTE'&&x.estado==='CONFIRMADO'&&id(x.expedienteId)&&id(x.relacionNivelId),'CREATION_NOT_CONFIRMED');
    const reference={loteId:s.lotId,raizId:r.rootId,expedienteId:x.expedienteId,relacionNivelId:x.relacionNivelId};
    // Persist known IDs before checking totals or returning a deliberately discarded result.
    onReference(reference);
    check(budget.maximumCreatedRoots>0,'UNEXPECTED_CREATION');const prior=creations.get(r.rootId);
    check(!prior||prior.expedienteId===reference.expedienteId&&prior.relacionNivelId===reference.relacionNivelId,'REPLAY_CHANGED_IDENTITY');
    check(![...creations.entries()].some(([k,v])=>k!==r.rootId&&(v.expedienteId===reference.expedienteId||v.relacionNivelId===reference.relacionNivelId)),'ROOT_IDENTITY_COLLISION');
    creations.set(r.rootId,reference);check(creations.size<=budget.maximumCreatedRoots,'CREATED_ROOT_BUDGET_EXCEEDED');blocked=false;
    return options.discardClientResult?{observation:'CLIENT_RESULT_DISCARDED'}:reference;
  }
  async function consult(){const r=await send('ConsultarLote',{loteId:s.lotId});check(r.status===200&&r.body.ok===true&&r.body.codigo==='OK'&&r.body.datos?.loteId===s.lotId&&r.body.datos.nivelRef===s.profile.nivelRef,'DESTINATION_CHANGED');return r.body.datos;}
  return {refreshToken,create,consult,counters:()=>({creationRequests:requests,createdRoots:creations.size,blocked})};
}
function assertRunnable(s){validateSession(s);check(s.scenario!=='EXP-03'||s.levelPermission,'LEVEL_PERMISSION_RESOURCES_REQUIRED');}
const stable=v=>JSON.stringify(v,(_k,x)=>x&&typeof x==='object'&&!Array.isArray(x)?Object.fromEntries(Object.keys(x).sort().map(k=>[k,x[k]])):x);
const digest=v=>require('node:crypto').createHash('sha256').update(stable(v)).digest('hex');
const parsed=v=>{try{return typeof v==='string'?JSON.parse(v):v;}catch{throw Error('DOC116_STORED_JSON_INVALID');}};
function assertInitialSnapshot(v,s){
  check(v?.lot&&v.roots?.length===2&&v.files?.length===2&&Array.isArray(v.attempts),'INITIAL_SNAPSHOT_INVALID');
  const ctx=parsed(v.lot.contexto_doc),snap=parsed(v.lot.config_doc);
  check(String(v.lot.lote_id)===s.lotId&&ctx?.UsuarioRef===s.identity.userRef&&ctx.AmbitoRef===s.identity.ambitoRef&&ctx.NivelRef===s.profile.nivelRef&&snap?.VersionId===s.profile.versionId,'STORED_CONTEXT_CHANGED');
  check(snap.Configuracion?.ModoIntegridad===(s.profile.mode==='PARCIAL'?'PARCIAL_PERMITIDO':'COMPLETO_OBLIGATORIO')&&snap.Configuracion?.AmbitoIntegridad===(s.profile.mode==='PARCIAL'?'NO_APLICA':s.profile.mode),'STORED_POLICY_CHANGED');
  check(String(v.lot.total_raices)==='2'&&String(v.lot.total_archivos)==='2'&&['0','false'].includes(String(v.lot.detencion_solicitada))&&['RECIBIENDO','LISTO'].includes(v.lot.estado),'LOT_NOT_READY');
  check(v.files.every(f=>/^[1-9][0-9]{0,19}$/.test(String(f.bytes_previstos)))&&v.files.reduce((n,f)=>n+BigInt(f.bytes_previstos),0n).toString()===String(v.lot.total_bytes),'ORIGINAL_BYTES_CHANGED');
  for(let i=0;i<s.roots.length;i++){
    const requested=s.roots[i],roots=v.roots.filter(r=>String(r.raiz_id)===requested.rootId);check(roots.length===1,'ROOT_CORRELATION_INVALID');const r=roots[0];
    check(Number(r.revision)===requested.revision&&r.estado==='PENDIENTE'&&!r.identidad_expediente_doc&&!r.intento_activo_id&&parsed(r.datos_doc)?.NombreOriginal===s.profile.rootNames[i],'ROOT_NOT_FRESH');
    const files=v.files.filter(f=>String(f.raiz_id)===requested.rootId);check(files.length===1&&files[0].estado_recepcion==='RECIBIDO'&&files[0].estado_efecto==='NO_INICIADO'&&!files[0].intento_activo_id&&!files[0].resultado_doc,'RECEPTION_PRECONDITION_REQUIRED');
  }
  check(v.attempts.every(i=>i.fase==='RECEPCION'&&['CONFIRMADO','RECHAZADO'].includes(i.estado)),'PREEXISTING_EFFECT_REQUIRES_REVIEW');
  return snap;
}
function createCreationControls(query,session){
  const s=validateSession(session),base=createSelectControls(query);
  async function rows(origin,sql,args){check(['RA','DA'].includes(origin)&&/^SELECT\b/.test(sql)&&!/;|\b(?:UPDATE|DELETE|INSERT|OUTFILE|DUMPFILE)\b/i.test(sql),'SELECT_REQUIRED');try{const r=await query(origin,sql,args);check(Array.isArray(r),'CONTROL_ROWS_INVALID');return r;}catch{throw Error('DOC116_CONTROL_SELECT_FAILED');}}
  const one=r=>{check(Array.isArray(r)&&r.length===1,'CONTROL_NOT_UNIQUE');return r[0];};
  async function snapshot(){
    const scope=one(await rows('RA','SELECT lote_id FROM ra_integracion_lote WHERE lote_id=? AND solicitante_ref=? AND ambito_ref=? AND nivel_ref=?',[s.lotId,s.identity.userRef,s.identity.ambitoRef,s.profile.nivelRef]));check(String(scope.lote_id)===s.lotId,'CONTROL_SCOPE_CHANGED');return base.snapshot(s.lotId);
  }
  async function levelCounts(){const counts=[];for(const level of [s.profile.nivelRef,s.levelB]){const r=one(await rows('RA','SELECT COUNT(*) AS total FROM ra_pro_niveles_has_expediente_archivo WHERE ra_pro_niveles_id_nivel=?',[level]));check(/^\d+$/.test(String(r.total)),'CONTROL_COUNT_INVALID');counts.push(String(r.total));}return counts;}
  async function verifyRoot(v,index,reference){
    const r=one(v.roots.filter(r=>String(r.raiz_id)===s.roots[index].rootId)),identity=parsed(r.identidad_expediente_doc),snap=parsed(v.lot.config_doc),config=snap.Configuracion;
    check(r.estado==='EXPEDIENTE_CONFIRMADO'&&!r.intento_activo_id&&identity?.NivelRef===s.profile.nivelRef&&identity.ExpedienteId===reference.expedienteId&&identity.RelacionNivelId===reference.relacionNivelId&&identity.OrigenLogico==='RA','ROOT_PROOF_INVALID');
    const attempt=one(v.attempts.filter(i=>String(i.raiz_id)===s.roots[index].rootId&&i.fase==='CREAR_EXPEDIENTE'));
    const proof=parsed(attempt.evidencia_doc);check(attempt.estado==='CONFIRMADO'&&attempt.fin_utc&&proof?.Estado==='CONFIRMADO'&&proof.Creacion?.IntentoId===String(attempt.intento_id)&&proof.Creacion.CicloArchivoNombre==='Archivo Gestión'&&id(proof.Creacion.CicloArchivoId)&&stable(proof.Expediente)===stable(identity),'CREATION_CHECKPOINT_INVALID');
    const row=one(await rows('RA','SELECT e.*,r.ra_pro_niveles_id_nivel AS nivel FROM expediente_archivo e INNER JOIN ra_pro_niveles_has_expediente_archivo r ON r.expediente_archivo_ID_EXPEDIENTE=e.ID_EXPEDIENTE WHERE e.ID_EXPEDIENTE=? AND r.id_registro=?',[identity.ExpedienteId,identity.RelacionNivelId]));
    const expected=s.profile.expectations[index];check(String(row.nivel)===s.profile.nivelRef&&String(row.ID_USUARIO_GESTION)===s.identity.userRef&&String(row.ID_EMPRESA_EXPEDIENTE)===s.identity.ambitoRef&&row.CODIGO_UNICO===expected.codigo&&row.ALEAS_EXPEDIENTE===expected.nombre&&row.TEMA_EXPEDIENTE===expected.nombre&&String(row.Id_tipos_ciclo_archivo)===proof.Creacion.CicloArchivoId,'EXPEDIENTE_VALUES_INVALID');
    const cycle=one(await rows('RA','SELECT Nombre_Tipo_ciclo_archivo AS nombre FROM ra_de_tipos_ciclos_archivo WHERE Id_tipos_ciclo_archivo=?',[proof.Creacion.CicloArchivoId]));check(cycle.nombre==='Archivo Gestión','CYCLE_CHANGED');
    for(const [column,key]of [['ID_FONDO','FondoRef'],['id_instrumento','InstrumentoRef'],['CODIGO_AREA_TRD','AreaRef'],['CODIGO_SERIE_TRD','SerieRef'],['CODIGO_SUB_SERIE_TRD','SubserieRef'],['RA_TIP_EXPE_ID_TIPO_EXPEDIENTE','TipoExpedienteRef'],['ID_TIPO_UNIDAD_DOCUMENTAL','UnidadDocumentalRef']])check((row[column]==null?null:String(row[column]))===(config[key]??null),'CLASSIFICATION_CHANGED');
    await base.assertNoDocuments({...v,roots:[r]});return digest({root:r,attempt,physical:row});
  }
  return {snapshot,levelCounts,verifyRoot};
}
async function selectProductionLevel(page,level){
  check(id(level),'LEVEL_REFERENCE_INVALID');
  try{
    await page.waitForFunction(level=>Array.isArray(window.ITEMS_DATOS)&&window.ITEMS_DATOS.some(i=>String(document.getElementById(i.id)?.title||'').split('\\')[0]===level),level);
    const [response]=await Promise.all([page.waitForResponse(r=>r.request().method()==='POST'&&/\/Gestion\/WebFormProducionDocumental\.aspx(?:\?|$)/i.test(r.url())),page.evaluate(level=>{const matches=window.ITEMS_DATOS.filter(i=>String(document.getElementById(i.id)?.title||'').split('\\')[0]===level);if(matches.length!==1||typeof window.OnSearchClick!=='function')throw Error('LEVEL_UNAVAILABLE');window.OnSearchClick(matches[0].id,'TreeViewArchivo');},level)]);
    check(response.status()===200,'LEVEL_SELECTION_FAILED');
    await page.waitForFunction(()=>!window.Sys?.WebForms?.PageRequestManager?.getInstance().get_isInAsyncPostBack());
    await page.waitForFunction(level=>document.getElementById('doc111-importacion')?.getAttribute('data-nivel')===level,level);
    return {level,postback:true};
  }catch{throw Error('DOC116_LEVEL_SELECTION_FAILED');}
}
async function runCreationScenario(auth,session,authorization,query,onReference=()=>{},runtime={}){
  assertRunnable(session);authorize(session,authorization);check(authorization.exclusive===true,'EXCLUSIVE_WINDOW_REQUIRED');
  const controls=createCreationControls(query,session),client=createCreationClient(auth,session,authorization,onReference);
  const before=await controls.snapshot();assertInitialSnapshot(before,session);const countsBefore=await controls.levelCounts();
  check(typeof runtime.selectLevel==='function','LEVEL_SELECTOR_REQUIRED');
  await runtime.selectLevel(auth.page,session.profile.nivelRef);if(session.scenario==='EXP-01')await runtime.selectLevel(auth.page,session.levelB);
  await client.refreshToken();await client.consult();
  if(session.scenario==='EXP-03'){
    check(authorization.levelPermission===true&&typeof runtime.createLevelBridge==='function','LEVEL_PERMISSION_AUTHORIZATION_REQUIRED');
    const levels=createLevelControls(query,session);await levels.grant(true);
    const bridge=await runtime.createLevelBridge(session);let attempted=false,restored=false;
    try{
      const ready=await bridge.ready();check(ready?.ok===true&&ready.stage==='READY'&&ready.userRef===session.identity.userRef&&ready.companyRef===session.identity.ambitoRef&&ready.levelRef===session.profile.nivelRef&&ready.permissionId===session.levelPermission.permissionId&&ready.otherFieldsUnchanged===true,'LEVEL_BRIDGE_SCOPE_CHANGED');
      attempted=true;const revoked=await bridge.send('REVOKE');check(revoked?.ok===true&&revoked.stage==='REVOKED'&&revoked.otherFieldsUnchanged===true,'LEVEL_REVOCATION_NOT_CONFIRMED');await levels.grant(false);
      const result=await client.create(session.roots[0].rootId);check(result.denied===true,'LEVEL_DENIAL_NOT_CONFIRMED');
      check(digest(before)===digest(await controls.snapshot())&&stable(countsBefore)===stable(await controls.levelCounts()),'DENIAL_CHANGED_DATA');
    }finally{
      try{if(attempted){const result=await bridge.send('RESTORE');check(result?.ok===true&&result.stage==='RESTORED'&&result.otherFieldsUnchanged===true,'LEVEL_RESTORATION_NOT_CONFIRMED');await levels.grant(true);restored=true;}}finally{await bridge.close();}
    }
    check(restored,'LEVEL_RESTORATION_NOT_CONFIRMED');
    await client.consult();check(digest(before)===digest(await controls.snapshot()),'DENIAL_CHANGED_DATA');
    return {scenario:session.scenario,status:'PASS',caseComplete:true,internalFailure:null,responseObservation:null,rootsCreated:0,documents:0,readOnlyControls:true,replayUnchanged:null,denialUnchanged:true,permissionRestored:true,counts:client.counters(),preparations:0,confirmations:0,uploads:0};
  }
  const number=session.scenario==='EXP-01'?2:1,references=[];
  for(let i=0;i<number;i++){
    if(session.scenario==='EXP-02'){
      const result=await client.create(session.roots[i].rootId,{discardClientResult:true});check(result.observation==='CLIENT_RESULT_DISCARDED','CLIENT_OBSERVATION_INVALID');
      await client.consult();
      // Verify the durable result before issuing the identical replay.
      const persisted=await controls.snapshot(),root=persisted.roots.find(r=>String(r.raiz_id)===session.roots[i].rootId),identity=parsed(root?.identidad_expediente_doc);
      check(id(identity?.ExpedienteId)&&id(identity?.RelacionNivelId),'ROOT_PROOF_INVALID');
      references[i]={loteId:session.lotId,raizId:session.roots[i].rootId,expedienteId:identity.ExpedienteId,relacionNivelId:identity.RelacionNivelId};
    }else references[i]=await client.create(session.roots[i].rootId);
    const after=await controls.snapshot(),fingerprint=await controls.verifyRoot(after,i,references[i]);
    await client.create(session.roots[i].rootId);
    const replay=await controls.snapshot();check(digest(after)===digest(replay)&&await controls.verifyRoot(replay,i,references[i])===fingerprint,'REPLAY_CHANGED_DATA');
  }
  const after=await controls.snapshot(),countsAfter=await controls.levelCounts();
  check(BigInt(countsAfter[0])===BigInt(countsBefore[0])+BigInt(number)&&countsAfter[1]===countsBefore[1],'LEVEL_COUNTS_CHANGED');
  check(after.files.every(f=>f.estado_recepcion==='RECIBIDO'&&f.estado_efecto==='NO_INICIADO'&&!f.intento_activo_id&&!f.resultado_doc)&&digest(before.files)===digest(after.files)&&after.attempts.every(i=>['CREAR_EXPEDIENTE','RECEPCION'].includes(i.fase)),'UNEXPECTED_DOCUMENT_EFFECT');
  if(number===1){const untouched=session.roots[1].rootId;check(digest(before.roots.find(r=>String(r.raiz_id)===untouched))===digest(after.roots.find(r=>String(r.raiz_id)===untouched)),'UNEXPECTED_ROOT_EFFECT');}
  return {scenario:session.scenario,status:'PASS',caseComplete:session.scenario==='EXP-01',internalFailure:session.scenario==='EXP-02'?'NO_EJECUTADO':null,responseObservation:session.scenario==='EXP-02'?'CLIENT_RESULT_DISCARDED':null,rootsCreated:number,documents:0,readOnlyControls:true,replayUnchanged:true,counts:client.counters(),preparations:0,confirmations:0,uploads:0};
}
for(const c of ['RUNTIME_ROOT_INVALID','LEVEL_PERMISSION_MUTATOR_NOT_COMPOSED','STORED_JSON_INVALID','INITIAL_SNAPSHOT_INVALID','STORED_CONTEXT_CHANGED','STORED_POLICY_CHANGED','LOT_NOT_READY','ORIGINAL_BYTES_CHANGED','ROOT_CORRELATION_INVALID','ROOT_NOT_FRESH','RECEPTION_PRECONDITION_REQUIRED','PREEXISTING_EFFECT_REQUIRES_REVIEW','SELECT_REQUIRED','CONTROL_ROWS_INVALID','CONTROL_SELECT_FAILED','CONTROL_NOT_UNIQUE','CONTROL_SCOPE_CHANGED','CONTROL_COUNT_INVALID','ROOT_PROOF_INVALID','CREATION_CHECKPOINT_INVALID','EXPEDIENTE_VALUES_INVALID','CYCLE_CHANGED','CLASSIFICATION_CHANGED','LEVEL_REFERENCE_INVALID','LEVEL_SELECTION_FAILED','EXCLUSIVE_WINDOW_REQUIRED','LEVEL_SELECTOR_REQUIRED','CLIENT_OBSERVATION_INVALID','REPLAY_CHANGED_DATA','LEVEL_COUNTS_CHANGED','UNEXPECTED_DOCUMENT_EFFECT','UNEXPECTED_ROOT_EFFECT'])PUBLIC_FAILURES.add('DOC116_'+c);
for(const c of ['LEVEL_PERMISSION_RESOURCES_INVALID','LEVEL_PERMISSION_RESOURCES_REQUIRED','LEVEL_PERMISSION_ROW_CHANGED','LEVEL_PERMISSION_AUTHORIZATION_REQUIRED','LEVEL_BRIDGE_SCOPE_CHANGED','LEVEL_REVOCATION_NOT_CONFIRMED','LEVEL_DENIAL_NOT_CONFIRMED','DENIAL_CHANGED_DATA','LEVEL_RESTORATION_NOT_CONFIRMED'])PUBLIC_FAILURES.add('DOC116_'+c);
module.exports={validateSession,authorize,assertLevelGrant,createLevelControls,safeFailureCode,createCreationClient,assertRunnable,assertInitialSnapshot,createCreationControls,selectProductionLevel,runCreationScenario};
