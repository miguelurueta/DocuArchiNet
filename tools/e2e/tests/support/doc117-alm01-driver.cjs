'use strict';
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const {validateManifest,sha,comparePublication,inspectPublications}=require('./doc117-almacenamiento.cjs');
const {createDriver,createSelectControls}=require('./doc111-importacion-carpetas.cjs');
const {assertPreparation,comparePublishedSnapshot}=require('./doc114-plan-importacion.cjs');
const id=x=>typeof x==='string'&&/^[1-9][0-9]{0,9}$/.test(x)&&BigInt(x)<=4294967295n;
function check(ok,code){if(!ok)throw Error('DOC117_'+code);}
function one(rows){check(Array.isArray(rows)&&rows.length===1,'CONTROL_NOT_UNIQUE');return rows[0];}
function parse(x){try{return typeof x==='string'?JSON.parse(x):x;}catch{throw Error('DOC117_JSON_INVALID');}}
const digest=x=>crypto.createHash('sha256').update(JSON.stringify(x)).digest('hex');
function createAlm01Controls(query,profile,identity,runtimeRoot){
 check(id(identity?.userRef)&&id(identity?.ambitoRef)&&path.isAbsolute(runtimeRoot),'CONTROL_SCOPE_INVALID');
 const base=createSelectControls(query);
 async function read(origin,sql,args){check(['RA','DA'].includes(origin)&&/^SELECT\b/.test(sql)&&!/;|\b(?:FOR\s+UPDATE|OUTFILE|DUMPFILE)\b/i.test(sql),'SELECT_REQUIRED');try{const rows=await query(origin,sql,args);check(Array.isArray(rows),'SELECT_INVALID');return rows;}catch{throw Error('DOC117_CONTROL_SELECT_FAILED');}}
 async function assertFresh(c){
  const found=await read('RA','SELECT e.ID_EXPEDIENTE AS id FROM expediente_archivo e WHERE e.ID_EMPRESA_EXPEDIENTE=? AND (e.CODIGO_UNICO=? OR e.ALEAS_EXPEDIENTE=? OR e.TEMA_EXPEDIENTE=?) LIMIT 2',[identity.ambitoRef,c.expected.codigo,c.rootName,c.rootName]);check(found.length===0,'ROOT_ALREADY_EXISTS');
  const plans=await read('RA','SELECT plan_id AS id FROM ra_integracion_plan WHERE ambito_ref=? AND nivel_ref=? AND manifiesto_doc LIKE ? LIMIT 2',[identity.ambitoRef,profile.nivelRef,'%'+c.rootName+'%']);check(plans.length===0,'PLAN_ALREADY_EXISTS');
  const roots=await read('RA','SELECT r.raiz_id AS id FROM ra_integracion_raiz r INNER JOIN ra_integracion_lote l ON l.lote_id=r.lote_id WHERE l.ambito_ref=? AND l.nivel_ref=? AND r.datos_doc LIKE ? LIMIT 2',[identity.ambitoRef,profile.nivelRef,'%'+c.rootName+'%']);check(roots.length===0,'ROOT_ALREADY_REFERENCED');
 }
 async function original(lotId,fileId,fixture){
  const v=await base.snapshot(lotId),ctx=parse(v.lot.contexto_doc);check(ctx.UsuarioRef===identity.userRef&&ctx.AmbitoRef===identity.ambitoRef&&ctx.NivelRef===profile.nivelRef&&/^[a-f0-9]{64}$/.test(ctx.SelloDestino),'ORIGINAL_SCOPE_INVALID');
  const row=one(await read('RA','SELECT f.staging_ref,LCASE(HEX(f.contenido_hash)) AS contenido_hash,f.bytes_previstos,f.bytes_recibidos,f.estado_recepcion FROM ra_integracion_archivo f INNER JOIN ra_integracion_raiz r ON r.raiz_id=f.raiz_id WHERE r.lote_id=? AND f.archivo_id=?',[lotId,fileId]));
  check(id(lotId)&&id(fileId)&&/^[a-f0-9]{32}$/.test(row.staging_ref)&&row.contenido_hash===fixture.sha256&&String(row.bytes_previstos)===String(fixture.bytes)&&String(row.bytes_recibidos)===String(fixture.bytes)&&row.estado_recepcion==='RECIBIDO','ORIGINAL_METADATA_CHANGED');
  const dir=path.resolve(runtimeRoot,'App_Data/ImportacionCarpetas'),target=path.join(dir,ctx.SelloDestino,ctx.AmbitoRef,ctx.UsuarioRef,lotId,fileId,row.staging_ref+'.bin');
  try{let current=runtimeRoot;for(const segment of path.relative(runtimeRoot,target).split(path.sep)){current=path.join(current,segment);check(!fs.lstatSync(current).isSymbolicLink(),'ORIGINAL_LINK_INVALID');}check(fs.realpathSync(target).startsWith(fs.realpathSync(dir)+path.sep),'ORIGINAL_PATH_INVALID');const stat=fs.statSync(target);check(stat.isFile()&&stat.size===fixture.bytes,'ORIGINAL_BYTES_CHANGED');const bytes=fs.readFileSync(target);check(bytes.length===fixture.bytes&&crypto.createHash('sha256').update(bytes).digest('hex')===fixture.sha256,'ORIGINAL_HASH_CHANGED');return{bytes:bytes.length,sha256:fixture.sha256};}catch{throw Error('DOC117_ORIGINAL_NOT_VERIFIED');}
 }
 async function assertMaterialized(lotId,fileId,c,expedition){
  const v=await base.snapshot(lotId),file=one(v.files.filter(f=>String(f.archivo_id)===fileId)),values=parse(file.valores_efectivos_doc);check(Array.isArray(values)&&values.length===2,'EFFECTIVE_VALUES_CHANGED');
  check(values.filter(x=>x.DestinoTipo==='TIPOLOGIA'&&x.DestinoRef==='TIPO_DOCUMENTAL'&&x.Valor===c.expected.tipologiaId&&!x.Diferido).length===1&&values.filter(x=>x.DestinoTipo==='INDICE_GABINETE'&&x.DestinoRef==='14'&&x.TipoValor==='TEXTO'&&x.Valor===expedition.expedienteId&&!x.Diferido).length===1,'EFFECTIVE_VALUES_CHANGED');
 }
 async function verify(lotId,c,expedition){
  const v=await base.snapshot(lotId);check(v.roots.length===1,'ROOT_COUNT_CHANGED');const ctx=parse(v.lot.contexto_doc);check(ctx.UsuarioRef===identity.userRef&&ctx.AmbitoRef===identity.ambitoRef&&ctx.NivelRef===profile.nivelRef,'CONTROL_CONTEXT_CHANGED');const actual=parse(v.roots[0].identidad_expediente_doc);check(actual?.ExpedienteId===expedition.expedienteId&&actual.RelacionNivelId===expedition.relacionNivelId,'CREATION_IDENTITY_CHANGED');
  const expected={codigo:c.expected.codigo,nombre:c.expected.nombre,tipologiaId:c.expected.tipologiaId,indices:c.expected.indices.map(x=>({referencia:x.referencia,tipo:x.tipo,valor:actual.ExpedienteId}))};
  return base.verify(lotId,{...profile,versionId:c.versionId,mode:'EXPEDIENTE',expectations:[expected]},{singleRoot:true,fileNames:[c.fileName],expectedDates:[c.expected.fechaInicial]});
 }
 async function preservation(ids){
  if(!ids){const lots=await read('RA','SELECT lote_id FROM ra_integracion_lote WHERE solicitante_ref=? AND ambito_ref=? ORDER BY lote_id LIMIT 201',[identity.userRef,identity.ambitoRef]);check(lots.length<=200,'PRESERVATION_BUDGET_EXCEEDED');ids=lots.map(x=>String(x.lote_id));check(ids.every(id)&&new Set(ids).size===ids.length,'PRESERVATION_IDS_INVALID');}
  const views=[];for(const lot of ids)views.push(await base.snapshot(lot));return{ids,fingerprint:digest(views)};
 }
 return{assertFresh,original,assertMaterialized,verify,preservation,snapshot:base.snapshot};
}
function assertEffectAuthorization(a,m,p){check(a?.scope==='ALM-01'&&a.manifestSha256===sha(m)&&['environment','account','select','effects','discardable','exclusive'].every(k=>a[k]===true)&&(!p.ignoreHttpsErrors||a.tls===true),'ALM01_EFFECT_AUTHORIZATION_REQUIRED');check(Array.isArray(a.publications)&&a.publications.length===2&&m.cases.every(c=>a.publications.filter(p=>p.versionId===c.versionId&&/^[a-f0-9]{64}$/.test(p.snapshotSha256)).length===1),'PUBLICATION_HASH_AUTHORIZATION_REQUIRED');}
async function runAlm01({manifest,profile,authorization,auth,query,identity,runtimeRoot,bytes,onCheckpoint,dependencies={}}){
 validateManifest(manifest);assertEffectAuthorization(authorization,manifest,profile);check(typeof onCheckpoint==='function','DURABLE_CHECKPOINT_REQUIRED');check(Buffer.isBuffer(bytes)&&bytes.length===manifest.fixture.bytes&&crypto.createHash('sha256').update(bytes).digest('hex')===manifest.fixture.sha256,'PDF_CHANGED');
 const controls=(dependencies.createControls||createAlm01Controls)(query,profile,identity,runtimeRoot),snapshots=new Map();
 const readProof=await (dependencies.inspectPublications||inspectPublications)({manifest,auth,profile,authorization:{environment:true,account:true,tls:authorization.tls,select:true},onPublication:(c,s)=>snapshots.set(c.versionId,s)});
 check(readProof.status==='PASS'&&readProof.manifestSha256===sha(manifest),'PUBLICATIONS_NOT_VERIFIED');
 check(authorization.publications.every(p=>readProof.publications?.filter(r=>r.versionId===p.versionId&&r.snapshotSha256===p.snapshotSha256).length===1),'AUTHORIZED_PUBLICATION_CHANGED');
 for(const c of manifest.cases)await controls.assertFresh(c);const prior=await controls.preservation();
 const results=[];
 for(const c of manifest.cases){
  await controls.assertFresh(c);let planId,lotId;const transportProfile={...profile,versionId:c.versionId,mode:'EXPEDIENTE'};
  const record=value=>{const safe={caseKey:c.key,stage:value.stage};if(value.phase)safe.phase=value.phase;for(const k of ['planId','lotId','loteId','recursoId','fileId','expedienteId','relacionNivelId','documentoId','inventarioId','intentoId'])if(id(value[k]))safe[k]=value[k];for(const k of ['originalVerified','replayAndReopenUnchanged'])if(typeof value[k]==='boolean')safe[k]=value[k];const saved=onCheckpoint(safe);check(!saved||typeof saved.then!=='function','SYNCHRONOUS_CHECKPOINT_REQUIRED');};
  const driver=(dependencies.createDriver||createDriver)(auth,transportProfile,authorization,{onReference:r=>{if(r.planId)planId=r.planId;if(r.loteId)lotId=r.loteId;record({stage:'KNOWN_REFERENCE',...r});}});await driver.refreshToken();
  record({stage:'PREPARATION_REQUESTED'});const plan=await driver.request('PrepararPlan',structuredClone(c.manifest));planId=plan.planId;record({stage:'PLAN_KNOWN',planId});
  assertPreparation(plan,c.manifest,{confirmable:true,eligible:['r1'],incidences:[],values:[{root:'r1',type:'CAMPO_EXPEDIENTE',ref:'CODIGO',value:c.expected.codigo,deferred:false},{root:'r1',type:'CAMPO_EXPEDIENTE',ref:'NOMBRE',value:c.rootName,deferred:false},{root:'r1',type:'CAMPO_EXPEDIENTE',ref:'FECHA_INICIAL',value:c.expected.fechaInicial,deferred:false},{root:'r1',file:'a1',type:'TIPOLOGIA',ref:'TIPO_DOCUMENTAL',value:c.expected.tipologiaId,deferred:false},{root:'r1',file:'a1',type:'INDICE_GABINETE',ref:'14',value:null,deferred:true}]});
  comparePublication(c,plan.snapshot,manifest.cabinetId);check(snapshots.has(c.versionId)&&comparePublishedSnapshot(plan.snapshot,snapshots.get(c.versionId)),'PLAN_SNAPSHOT_CHANGED');
  record({stage:'CONFIRMATION_REQUESTED',planId});const lot=await driver.request('ConfirmarPlan',{planId,revisionEsperada:plan.revision,planHashEsperado:plan.planHash,claveConfirmacion:crypto.randomBytes(16).toString('hex')});lotId=lot.loteId;check(id(lotId)&&lot.nivelRef===manifest.levelId,'LOT_NOT_CONFIRMED');record({stage:'LOT_KNOWN',planId,lotId});
  const roots=await driver.pages(lotId,'RAICES'),files=await driver.pages(lotId,'ARCHIVOS');check(roots.length===1&&files.length===1&&roots[0].idCliente==='r1'&&files[0].idCliente==='a1','INVENTORY_CHANGED');let file=files[0];
  record({stage:'RECEPTION_REQUESTED',planId,lotId,fileId:file.recursoId});await driver.receive(lotId,file,bytes,c.fileName);await controls.original(lotId,file.recursoId,manifest.fixture);record({stage:'ORIGINAL_VERIFIED',planId,lotId,fileId:file.recursoId});
  let expedition,stored,storageCommand;
  for(const phase of ['CREAR_EXPEDIENTE','MATERIALIZAR_VALORES','ALMACENAR']){
   const row=one(await driver.pages(lotId,phase==='CREAR_EXPEDIENTE'?'RAICES':'ARCHIVOS'));check(id(row.recursoId)&&Number.isInteger(row.revision)&&row.revision>0,'RESOURCE_REVISION_INVALID');
   const cmd={loteId:lotId,recursoId:row.recursoId,fase:phase,revisionEsperada:row.revision,claveOperacion:crypto.randomBytes(16).toString('hex')};record({stage:'PHASE_REQUESTED',phase,planId,lotId,recursoId:row.recursoId});const r=await driver.request('AvanzarLote',cmd);
   record({stage:'PHASE_OBSERVED',phase,planId,lotId,expedienteId:r.expedienteId,relacionNivelId:r.relacionNivelId,documentoId:r.documentoId,inventarioId:r.inventarioId,intentoId:r.intentoId});
   check(r.estado==='CONFIRMADO'&&r.fase===phase&&r.loteId===lotId,'PHASE_NOT_CONFIRMED');
   if(phase==='CREAR_EXPEDIENTE'){check(id(r.expedienteId)&&id(r.relacionNivelId)&&r.raizId===row.recursoId,'CREATION_NOT_CONFIRMED');expedition=r;}
   else check(r.archivoId===row.recursoId,'FILE_CORRELATION_CHANGED');
   if(phase==='MATERIALIZAR_VALORES')await controls.assertMaterialized(lotId,row.recursoId,c,expedition);
   if(phase==='ALMACENAR'){check(id(r.documentoId)&&id(r.inventarioId)&&id(r.intentoId),'DOCUMENT_NOT_CONFIRMED');stored=r;storageCommand=cmd;}
  }
  const before=await controls.verify(lotId,c,expedition);await controls.original(lotId,file.recursoId,manifest.fixture);record({stage:'REPLAY_REQUESTED',planId,lotId});
  const replay=await driver.request('AvanzarLote',storageCommand);check(replay.estado==='CONFIRMADO'&&['loteId','raizId','archivoId','intentoId','documentoId','inventarioId','fase'].every(k=>replay[k]===stored[k]),'REPLAY_CHANGED');
  await driver.reopen(lotId);const after=await controls.verify(lotId,c,expedition);check(before.fingerprint===after.fingerprint,'REPLAY_OR_REOPEN_CHANGED');await controls.original(lotId,file.recursoId,manifest.fixture);
  const protectedNow=await controls.preservation(prior.ids);check(protectedNow.fingerprint===prior.fingerprint,'PRESERVED_LOTS_CHANGED');
  const result={caseKey:c.key,planId,lotId,expedienteId:expedition.expedienteId,documentoId:stored.documentoId,inventarioId:stored.inventarioId,originalVerified:true,replayAndReopenUnchanged:true};results.push(result);record({stage:'CASE_PASS',...result});
 }
 return{status:'PASS',scenario:'ALM-01',cases:results,preparations:2,confirmations:2,uploads:2,roots:2,documents:2,storageRequests:4,preservedLotsUnchanged:true};
}
module.exports={createAlm01Controls,runAlm01,assertEffectAuthorization};
