'use strict';
const crypto=require('node:crypto');
const {createDriver,createSelectControls}=require('./doc111-importacion-carpetas.cjs');
const {createAlm01Controls}=require('./doc117-alm01-driver.cjs');
const {preservation}=require('./doc116-exp01-setup.cjs');
const {comparePublishedSnapshot}=require('./doc114-plan-importacion.cjs');
const {PROTOCOL,validateScope,verifyIndividual}=require('./doc117-alm03-setup-scope.cjs');
const check=(v,c)=>{if(!v)throw Error('DOC117_ALM03_SETUP_'+c);},id=v=>typeof v==='string'&&/^[1-9][0-9]{0,9}$/.test(v)&&BigInt(v)<=4294967295n;
const sha=v=>crypto.createHash('sha256').update(v).digest('hex');
const parse=v=>{try{return typeof v==='string'?JSON.parse(v):v;}catch{throw Error('DOC117_ALM03_SETUP_JSON_INVALID');}};
const one=rows=>{check(Array.isArray(rows)&&rows.length===1,'CONTROL_NOT_UNIQUE');return rows[0];};
const samePublication=(ac,ar,bc,br)=>comparePublishedSnapshot({Configuracion:ac,Reglas:ar},{Configuracion:bc,Reglas:br});
function validateProposal(p){
 check(p?.issueKey==='DOC-117'&&p.scope==='ALM-03-SETUP'&&p.status==='PROPOSED_NOT_EXECUTED'&&p.environment==='https://localhost/GestionDocumental-Docuarchi.net/'&&p.identity?.userRef==='141'&&p.identity.ambitoRef==='2'&&p.levelId==='7'&&p.versionId==='38'&&p.cabinetId==='10'&&p.proofLotId==='21','SCOPE_INVALID');
 check(JSON.stringify(p.rootNames)===JSON.stringify(['DOC117-ALM03-A-20261106','DOC117-ALM03-B-20261106'])&&JSON.stringify(p.fileNames)===JSON.stringify(['DOC117-ALM03-IMPORT-A-20261106.pdf','DOC117-ALM03-IMPORT-B-20261106.pdf'])&&p.individual?.expedienteId==='467'&&p.individual.levelId==='8'&&p.individual.relacionNivelId==='1072'&&p.individual.typologyId==='50'&&p.individual.fileName==='DOC117-ALM03-IND-20261106.pdf','RESOURCES_CHANGED');
 check(p.fixture?.file==='doc111-carga-individual.pdf'&&p.fixture.bytes===677&&p.fixture.sha256==='a5f1924afbae4bd631de893c49a56d9f09219075113e6da2ec983602cff84c6c','FIXTURE_CHANGED');
 check(JSON.stringify(p.budget)===JSON.stringify({plans:1,lots:1,receipts:2,createdExpedients:2,materializations:2,documents:0,storageRequests:0,runtimeChanges:0,maximumSelectRequests:160}),'BUDGET_CHANGED');
 check(p.expected?.typologyId==='79'&&p.expected.indexId==='14'&&p.expected.indexType==='TEXTO'&&p.expected.indexValue==='EEE'&&p.preserveAllExistingResources===true&&p.preserveNewResources===true&&p.noRetry===true&&JSON.stringify(p.preservedLotIds)===JSON.stringify(['18','19','20','21']),'PRESERVATION_REQUIRED');check(/^[a-f0-9]{64}$/.test(p.individual.classificationSha256),'INDIVIDUAL_FINGERPRINT_INVALID');return p;
}
function authorize(a,p){check(a?.scope===p.scope&&a.environment===p.environment&&['authorized','environmentAuthorized','account','select','effects','discardable','exclusive'].every(k=>a[k]===true)&&a.storage===false&&a.runtimeChanges===false,'SPECIFIC_AUTHORIZATION_REQUIRED');}
function controls(query,p,profile,runtimeRoot){
 const base=createSelectControls(query),original=createAlm01Controls(query,profile,p.identity,runtimeRoot);
 return{...base,original:original.original,assertFresh:async exceptPlan=>{for(const name of p.rootNames){
  if(!exceptPlan){await original.assertFresh({rootName:name,expected:{codigo:name}});continue;}
  check((await query('RA','SELECT ID_EXPEDIENTE FROM expediente_archivo WHERE ID_EMPRESA_EXPEDIENTE=? AND (CODIGO_UNICO=? OR ALEAS_EXPEDIENTE=? OR TEMA_EXPEDIENTE=?) LIMIT 2',[p.identity.ambitoRef,name,name,name])).length===0,'ROOT_ALREADY_EXISTS');
  check((await query('RA','SELECT plan_id FROM ra_integracion_plan WHERE ambito_ref=? AND nivel_ref=? AND plan_id<>? AND LOCATE(?,manifiesto_doc)>0 LIMIT 2',[p.identity.ambitoRef,p.levelId,exceptPlan,name])).length===0,'OTHER_PLAN_ALREADY_EXISTS');
  check((await query('RA','SELECT r.raiz_id FROM ra_integracion_raiz r INNER JOIN ra_integracion_lote l ON l.lote_id=r.lote_id WHERE l.ambito_ref=? AND l.nivel_ref=? AND LOCATE(?,r.datos_doc)>0 LIMIT 2',[p.identity.ambitoRef,p.levelId,name])).length===0,'ROOT_ALREADY_REFERENCED');
 }},preservation:bounds=>preservation(query,{identity:p.identity,preservedLotIds:p.preservedLotIds},bounds)};
}
async function runSetup({proposal,profile,authorization,auth,query,bytes,runtimeRoot,calculate,onProgress,onReady,resumePlanId,dependencies={}}){
 const p=(dependencies.validateProposal||validateProposal)(proposal);authorize(authorization,p);
 check(resumePlanId===undefined||id(resumePlanId)&&authorization.resumePlanId===resumePlanId&&authorization.maximumAdditionalPreparations===0,'RESUME_SCOPE_INVALID');
 check(Buffer.isBuffer(bytes)&&bytes.length===p.fixture.bytes&&sha(bytes)===p.fixture.sha256,'PDF_CHANGED');
 check(typeof calculate==='function'&&typeof onProgress==='function'&&typeof onReady==='function','DURABLE_OUTPUT_REQUIRED');
 const record=value=>{const result=onProgress(value);check(!result||typeof result.then!=='function','SYNCHRONOUS_CHECKPOINT_REQUIRED');};
 const c=(dependencies.controls||controls)(query,p,profile,runtimeRoot);
 const transport={...profile,nivelRef:p.levelId,versionId:p.versionId,mode:'LOTE',pdfFixture:p.fixture.file,rootNames:p.rootNames,expectations:p.rootNames.map(name=>({codigo:name,nombre:name,tipologiaId:p.expected.typologyId,indices:[{referencia:p.expected.indexId,tipo:p.expected.indexType,valor:p.expected.indexValue}]}))};
 const driver=(dependencies.createDriver||createDriver)(auth,transport,{...authorization,environment:true},{onReference:r=>record({stage:'KNOWN_REFERENCE',...r})});
 await driver.refreshToken();await c.assertFresh(resumePlanId);let preserved=await c.preservation();
 const individual=await verifyIndividual(query,p);
 if(resumePlanId)preserved=await c.preservation({maxPlan:(BigInt(resumePlanId)-1n).toString(),maxLot:preserved.scope.maxLot});
 const publication=one(await query('RA',"SELECT v.version_id,p.perfil_id,v.configuracion_doc FROM ra_integracion_perfil_version v INNER JOIN ra_integracion_perfil p ON p.perfil_id=v.perfil_id WHERE p.ambito_ref=? AND p.activo=1 AND v.estado='PUBLICADA' AND v.version_id=? LIMIT 2",[p.identity.ambitoRef,p.versionId]));
 const config=parse(publication.configuracion_doc),ruleRows=await query('RA','SELECT regla_id,regla_doc FROM ra_integracion_perfil_regla WHERE version_id=? ORDER BY orden,destino_tipo,destino_ref LIMIT 101',[p.versionId]),rawRules=ruleRows.map(r=>parse(r.regla_doc));
 check(String(publication.version_id)===p.versionId&&rawRules.length>0&&rawRules.length<=100&&sha(JSON.stringify({config,rules:rawRules}))===p.publicationSha256&&String(config.GabineteId)===p.cabinetId&&config.ModoIntegridad==='COMPLETO_OBLIGATORIO'&&config.AmbitoIntegridad==='LOTE','PUBLICATION_CHANGED');
 const rules=rawRules.map((r,i)=>ruleRows[i].regla_id==null?r:{...r,ReglaId:String(ruleRows[i].regla_id)});
 const typology=one(await query('RA','SELECT CAST(t.Id_Tipo_Doc_Series AS CHAR) AS type_id FROM tipo_doc_series t INNER JOIN ra_registro_instrumento_archivistico i ON i.id_instrumento=t.id_instrumento INNER JOIN registro_organigrama o ON o.ID_ORGANIGRAMA=i.registro_organigrama_ID_ORGANIGRAMA WHERE t.Estado_Tipo=1 AND i.Estado_instrumento=1 AND o.EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA=? AND t.id_instrumento=? AND ((t.Series_Documentales_Id_Series=? AND COALESCE(t.sub_serie_id_serie,0)=COALESCE(?,0)) OR t.tipo_doc_trasversal=1) AND t.Id_Tipo_Doc_Series=? LIMIT 2',[p.identity.ambitoRef,config.InstrumentoRef,config.SerieRef,config.SubserieRef||null,p.expected.typologyId]));check(String(typology.type_id)===p.expected.typologyId,'TYPOLOGY_UNAVAILABLE');
 const proof=parse(one(await query('RA','SELECT contexto_doc FROM ra_integracion_lote WHERE lote_id=? AND solicitante_ref=? AND ambito_ref=? AND nivel_ref=? LIMIT 2',[p.proofLotId,p.identity.userRef,p.identity.ambitoRef,p.levelId])).contexto_doc);
 const opened=await driver.request('AbrirImportacion',{nivelRef:p.levelId});
 check(proof.UsuarioRef===p.identity.userRef&&proof.AmbitoRef===p.identity.ambitoRef&&proof.NivelRef===p.levelId&&opened.nivelRef===p.levelId&&opened.selloDestino===proof.SelloDestino&&opened.rutaCanonica===proof.RutaCanonica,'ACCOUNT_OR_DESTINATION_CHANGED');
 const owned=await driver.request('ConsultarLote',{loteId:p.proofLotId});check(owned.loteId===p.proofLotId&&owned.nivelRef===p.levelId,'ACCOUNT_PROOF_FAILED');
 await c.assertFresh(resumePlanId);
 const manifest={nivelRef:p.levelId,versionId:p.versionId,raices:p.rootNames.map((name,i)=>({seleccionId:'s'+(i+1),idCliente:'r'+(i+1),nombre:name})),archivos:p.rootNames.map((name,i)=>({idCliente:'a'+(i+1),raizIdCliente:'r'+(i+1),nombre:p.fileNames[i],rutaRelativa:name+'/'+p.fileNames[i],bytes:String(bytes.length),ultimaModificacionLocal:'0'}))};
 let plan;
 if(resumePlanId){
  record({stage:'EXISTING_PLAN_READ_REQUESTED',planId:resumePlanId});
  const row=one(await query('RA','SELECT plan_id,nivel_ref,estado,revision,contexto_doc,config_doc,manifiesto_doc,resultado_doc,LCASE(HEX(plan_hash)) AS plan_hash FROM ra_integracion_plan WHERE plan_id=? AND usuario_ref=? AND ambito_ref=? AND nivel_ref=? LIMIT 2',[resumePlanId,p.identity.userRef,p.identity.ambitoRef,p.levelId]));
  check(String(row.plan_id)===resumePlanId&&row.estado==='REVISABLE'&&Number(row.revision)===1&&/^[a-f0-9]{64}$/.test(row.plan_hash),'EXISTING_PLAN_CHANGED');
  check((await query('RA','SELECT lote_id FROM ra_integracion_lote WHERE plan_id=? LIMIT 2',[resumePlanId])).length===0,'EXISTING_PLAN_ALREADY_CONFIRMED');
  const ctx=parse(row.contexto_doc),saved=parse(row.manifiesto_doc);check(ctx.UsuarioRef===p.identity.userRef&&ctx.AmbitoRef===p.identity.ambitoRef&&ctx.NivelRef===p.levelId&&ctx.SelloDestino===proof.SelloDestino&&ctx.RutaCanonica===proof.RutaCanonica,'EXISTING_PLAN_CONTEXT_CHANGED');
  check(saved.Raices?.length===2&&saved.Archivos?.length===2&&p.rootNames.every((name,i)=>saved.Raices.filter(r=>r.IdCliente==='r'+(i+1)&&r.Nombre===name&&r.SeleccionId==='s'+(i+1)).length===1&&saved.Archivos.filter(f=>f.IdCliente==='a'+(i+1)&&f.RaizIdCliente==='r'+(i+1)&&f.Nombre===p.fileNames[i]&&f.RutaRelativa===name+'/'+p.fileNames[i]&&f.Bytes==='677').length===1),'EXISTING_MANIFEST_CHANGED');
  plan={planId:resumePlanId,nivelRef:String(row.nivel_ref),revision:Number(row.revision),planHash:row.plan_hash,snapshot:parse(row.config_doc),resultado:parse(row.resultado_doc)};
 }else{record({stage:'PREPARATION_REQUESTED'});plan=await driver.request('PrepararPlan',manifest);}
 check(id(plan?.planId),'PLAN_INVALID');record({stage:'PLAN_KNOWN',planId:plan.planId});if(dependencies.validatePlan)dependencies.validatePlan(plan,preserved.scope);
 check(plan.nivelRef===p.levelId&&plan.snapshot?.VersionId===p.versionId&&samePublication(plan.snapshot.Configuracion,plan.snapshot.Reglas,config,rules)&&plan.resultado?.PuedeConfirmar===true&&JSON.stringify([...plan.resultado.UnidadesElegibles].sort())==='["r1","r2"]','PLAN_CHANGED');
 for(let i=0;i<2;i++){
  const root=one(plan.resultado.Elementos.filter(e=>e.Ambito==='RAIZ'&&e.RaizIdCliente==='r'+(i+1))),file=one(plan.resultado.Elementos.filter(e=>e.Ambito==='ARCHIVO'&&e.ArchivoIdCliente==='a'+(i+1)));
  for(const field of ['CODIGO','NOMBRE'])check(one(root.Valores.filter(v=>v.DestinoTipo==='CAMPO_EXPEDIENTE'&&v.DestinoRef===field)).Valor===p.rootNames[i],'ROOT_VALUES_CHANGED');
  check(one(file.Valores.filter(v=>v.DestinoTipo==='TIPOLOGIA'&&v.DestinoRef==='TIPO_DOCUMENTAL')).Valor===p.expected.typologyId&&one(file.Valores.filter(v=>v.DestinoTipo==='INDICE_GABINETE'&&v.DestinoRef===p.expected.indexId)).Valor===p.expected.indexValue,'FILE_VALUES_CHANGED');
 }
 record({stage:'CONFIRMATION_REQUESTED',planId:plan.planId});const lot=await driver.request('ConfirmarPlan',{planId:plan.planId,revisionEsperada:plan.revision,planHashEsperado:plan.planHash,claveConfirmacion:crypto.randomBytes(16).toString('hex')});check(id(lot?.loteId)&&BigInt(lot.loteId)>21n&&lot.nivelRef===p.levelId,'LOT_INVALID');record({stage:'LOT_KNOWN',planId:plan.planId,loteId:lot.loteId});
 if(dependencies.validateLot)dependencies.validateLot(lot,preserved.scope);
 const roots=await driver.pages(lot.loteId,'RAICES'),files=await driver.pages(lot.loteId,'ARCHIVOS');check(roots.length===2&&files.length===2,'INVENTORY_CHANGED');
 const paired=roots.map((r,i)=>{const root=one(roots.filter(x=>x.idCliente==='r'+(i+1))),file=one(files.filter(x=>x.idCliente==='a'+(i+1)));check(id(root.recursoId)&&id(file.recursoId),'RESOURCE_ID_INVALID');return{rootId:root.recursoId,fileId:file.recursoId,index:i};});
 if(dependencies.validatePaired)dependencies.validatePaired(paired);
 for(const r of paired){const file=one(files.filter(x=>x.recursoId===r.fileId));record({stage:'RECEPTION_REQUESTED',loteId:lot.loteId,archivoId:r.fileId});await driver.receive(lot.loteId,file,bytes,p.fileNames[r.index]);await c.original(lot.loteId,r.fileId,p.fixture);}
 for(const phase of ['CREAR_EXPEDIENTE','MATERIALIZAR_VALORES'])for(const r of paired){
  const row=one((await driver.pages(lot.loteId,phase==='CREAR_EXPEDIENTE'?'RAICES':'ARCHIVOS')).filter(x=>x.recursoId===(phase==='CREAR_EXPEDIENTE'?r.rootId:r.fileId)));
  check(Number.isInteger(row.revision)&&row.revision>0,'REVISION_INVALID');record({stage:'PHASE_REQUESTED',fase:phase,loteId:lot.loteId,recursoId:row.recursoId});
  const result=await driver.request('AvanzarLote',{loteId:lot.loteId,recursoId:row.recursoId,fase:phase,revisionEsperada:row.revision,claveOperacion:crypto.randomBytes(16).toString('hex')});
  record({stage:'PHASE_OBSERVED',fase:phase,loteId:lot.loteId,raizId:result.raizId,archivoId:result.archivoId,intentoId:result.intentoId,expedienteId:result.expedienteId,relacionNivelId:result.relacionNivelId});
  check(result.estado==='CONFIRMADO'&&result.fase===phase&&result.loteId===lot.loteId,'PHASE_NOT_CONFIRMED');
  if(phase==='CREAR_EXPEDIENTE'){check(result.raizId===r.rootId&&id(result.expedienteId)&&id(result.relacionNivelId),'CREATION_NOT_CONFIRMED');if(dependencies.validateIdentity)dependencies.validateIdentity(result,paired);r.identity={ExpedienteId:result.expedienteId,RelacionNivelId:result.relacionNivelId};}else check(result.archivoId===r.fileId,'MATERIALIZATION_NOT_CONFIRMED');
 }
 const view=await c.snapshot(lot.loteId),context=parse(view.lot.contexto_doc),stored=parse(view.lot.config_doc);
 check(view.roots.length===2&&view.files.length===2&&view.lot.estado==='EN_CURSO'&&String(view.lot.detencion_solicitada)==='0'&&String(view.lot.total_raices)==='2'&&String(view.lot.total_archivos)==='2'&&String(view.lot.total_bytes)==='1354'&&view.attempts.length===4&&view.attempts.every(a=>['CREAR_EXPEDIENTE','RECEPCION'].includes(a.fase)&&a.estado==='CONFIRMADO'&&a.fin_utc)&&view.attempts.filter(a=>a.fase==='CREAR_EXPEDIENTE').length===2&&stored.VersionId===p.versionId&&samePublication(stored.Configuracion,stored.Reglas,config,rules),'READY_STATE_CHANGED');
 check(context.UsuarioRef===p.identity.userRef&&context.AmbitoRef===p.identity.ambitoRef&&context.NivelRef===p.levelId&&context.SelloDestino===proof.SelloDestino&&context.RutaCanonica===proof.RutaCanonica,'READY_CONTEXT_CHANGED');
 const commands=[],resources=[];
 for(const r of paired){
  const root=one(view.roots.filter(x=>String(x.raiz_id)===r.rootId)),file=one(view.files.filter(x=>String(x.archivo_id)===r.fileId)),identity=parse(root.identidad_expediente_doc),values=parse(file.valores_efectivos_doc);
  check(root.estado==='EXPEDIENTE_CONFIRMADO'&&!root.intento_activo_id&&identity?.OrigenLogico==='RA'&&identity.NivelRef===p.levelId&&identity.ExpedienteId===r.identity.ExpedienteId&&identity.RelacionNivelId===r.identity.RelacionNivelId&&String(file.raiz_id)===r.rootId&&file.estado_recepcion==='RECIBIDO'&&file.estado_efecto==='NO_INICIADO'&&!file.resultado_doc&&!file.intento_activo_id,'READY_RESOURCE_CHANGED');
  check(Array.isArray(values)&&values.length===2&&one(values.filter(v=>v.DestinoTipo==='TIPOLOGIA')).Valor===p.expected.typologyId&&one(values.filter(v=>v.DestinoTipo==='INDICE_GABINETE'&&v.DestinoRef===p.expected.indexId)).Valor===p.expected.indexValue&&values.every(v=>!v.Diferido),'READY_VALUES_CHANGED');
  const exp=one(await query('RA','SELECT e.CODIGO_UNICO,e.ALEAS_EXPEDIENTE,e.TEMA_EXPEDIENTE,e.ID_USUARIO_GESTION,e.ID_EMPRESA_EXPEDIENTE,r.ra_pro_niveles_id_nivel AS nivel FROM expediente_archivo e INNER JOIN ra_pro_niveles_has_expediente_archivo r ON r.expediente_archivo_ID_EXPEDIENTE=e.ID_EXPEDIENTE WHERE e.ID_EXPEDIENTE=? AND r.id_registro=? LIMIT 2',[identity.ExpedienteId,identity.RelacionNivelId]));
  check(exp.CODIGO_UNICO===p.rootNames[r.index]&&exp.ALEAS_EXPEDIENTE===p.rootNames[r.index]&&exp.TEMA_EXPEDIENTE===p.rootNames[r.index]&&String(exp.ID_USUARIO_GESTION)===p.identity.userRef&&String(exp.ID_EMPRESA_EXPEDIENTE)===p.identity.ambitoRef&&String(exp.nivel)===p.levelId,'PHYSICAL_EXPEDIENT_CHANGED');
  await c.original(lot.loteId,r.fileId,p.fixture);const revision=Number(file.revision);check(Number.isSafeInteger(revision)&&revision>0&&revision<4294967294,'REVISION_INVALID');
  commands.push({loteId:lot.loteId,recursoId:r.fileId,fase:'ALMACENAR',revisionEsperada:revision,claveOperacion:crypto.randomBytes(16).toString('hex')});resources.push({rootId:r.rootId,fileId:r.fileId,expedienteId:identity.ExpedienteId,relacionNivelId:identity.RelacionNivelId,fileName:p.fileNames[r.index],bytes:String(p.fixture.bytes),contentSha256:p.fixture.sha256});
 }
 await c.assertNoDocuments(view);const after=await c.preservation(preserved.scope);check(JSON.stringify(preserved)===JSON.stringify(after),'PRESERVED_RESOURCES_CHANGED');
 const hashes=await calculate({Contexto:context,Solicitudes:commands.map(c=>({LoteId:c.loteId,ArchivoId:c.recursoId,RevisionEsperada:c.revisionEsperada,ClaveOperacion:c.claveOperacion,Fase:c.fase}))});check(Array.isArray(hashes)&&hashes.length===2&&hashes.every(h=>/^[a-f0-9]{64}$/.test(h))&&hashes[0]!==hashes[1],'HASH_NOT_VERIFIED');
 const resourceScope=(dependencies.createResourceScope||validateScope)({individual,issueKey:'DOC-117',protocol:PROTOCOL,runtimeRoot,runId:crypto.randomBytes(16).toString('hex'),lotId:lot.loteId,snapshotSha256:stored.Huella,resources:resources.map((r,i)=>({...r,requestSha256:hashes[i]}))});
 await verifyIndividual(query,p);
 if(dependencies.verifyPreparedScope)await dependencies.verifyPreparedScope(resourceScope);
 const ready=onReady({resourceScope,commands});check(!ready||typeof ready.then!=='function','SYNCHRONOUS_CHECKPOINT_REQUIRED');
 return{status:'PASS',scenario:'ALM-03-SETUP',planId:plan.planId,loteId:lot.loteId,plans:1,additionalPreparations:resumePlanId?0:1,resumedExistingPlan:!!resumePlanId,lots:1,receipts:2,createdExpedients:2,materializations:2,documents:0,storageRequests:0,runtimeChanges:0,preservedResourcesUnchanged:true,originalsVerified:true,resourceScopePrepared:true};
}
module.exports={runSetup,validateProposal,authorize};
