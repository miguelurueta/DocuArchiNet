'use strict';
// Protocol over existing DOC111 clients/controls; no login, debugger or retry.
const crypto=require('node:crypto');
const shared=require('./doc111-importacion-carpetas.cjs');
const {prepareCommand}=require('./doc118-session-bridge.cjs');
const check=(value,code)=>{if(!value)throw Error('DOC118_PREPARATION_'+code);};
const id=value=>typeof value==='string'&&/^[1-9][0-9]{0,9}$/.test(value)&&BigInt(value)<=4294967295n;
const one=(items,predicate)=>{const rows=items.filter(predicate);check(rows.length===1,'CORRELATION_INVALID');return rows[0];};
const hash=value=>crypto.createHash('sha256').update(value).digest('hex');
const inventoryFingerprint=view=>hash(JSON.stringify({roots:view.roots.map(r=>[String(r.raiz_id),r.cliente_raiz_id,r.datos_doc,r.identidad_expediente_doc]),files:view.files.map(f=>[String(f.archivo_id),String(f.raiz_id),f.cliente_archivo_id,String(f.bytes_previstos)])}));
function caseProfile(profile,scenario,rootNames){
 shared.validateProfile(profile);check(['COORD02','COORD03'].includes(scenario)&&profile.mode==='LOTE','SCOPE_INVALID');
 const suffix=scenario==='COORD02'?'02':'03';
 check(JSON.stringify(rootNames)===JSON.stringify(['A','B'].map(slot=>'DOC118-C'+suffix+'-'+slot+'-20261107')),'ROOT_NAMES_CHANGED');
 const result=structuredClone(profile);result.rootNames=rootNames.slice();
 result.expectations=result.expectations.map((e,i)=>({...e,codigo:rootNames[i],nombre:rootNames[i]}));
 return shared.validateProfile(result);
}
function effective(values,expected){
 check(Array.isArray(values)&&values.length===expected.indices.length+1,'EFFECTIVE_VALUES_INVALID');
 check(values.filter(v=>v.DestinoTipo==='TIPOLOGIA'&&v.Valor===expected.tipologiaId&&v.Diferido===false).length===1,'EFFECTIVE_VALUES_INVALID');
 for(const wanted of expected.indices){const v=one(values,x=>x.DestinoTipo==='INDICE_GABINETE'&&x.DestinoRef===wanted.referencia);check(v.TipoValor===wanted.tipo&&v.Valor===wanted.valor&&v.Diferido===false,'EFFECTIVE_VALUES_INVALID');}
}
async function prepareCase({scenario,profile,rootNames,actorRef,ambitoRef,client,controls,query,assertSourceReady,onReference=()=>{}}){
 const p=caseProfile(profile,scenario,rootNames),bytes=shared.readFixture(p),fileNames=p.rootNames.map(name=>name+'.pdf');
 check(id(actorRef)&&id(ambitoRef)&&typeof query==='function'&&typeof assertSourceReady==='function','DEPENDENCIES_REQUIRED');
 await assertSourceReady(); // Must fail before plan/confirmation/receipt effects.
 const absent=await query('RA','SELECT COUNT(*) AS total FROM expediente_archivo WHERE ID_EMPRESA_EXPEDIENTE=? AND (CODIGO_UNICO IN (?,?) OR ALEAS_EXPEDIENTE IN (?,?))',[ambitoRef,...p.rootNames,...p.rootNames]);
 check(absent.length===1&&String(absent[0].total)==='0','ROOT_NAMES_ALREADY_PRESENT');
 const opened=await client.request('AbrirImportacion',{nivelRef:p.nivelRef});check(opened.nivelRef===p.nivelRef,'DESTINATION_CHANGED');
 const roots=p.rootNames.map((nombre,i)=>({seleccionId:'s'+(i+1),idCliente:'r'+(i+1),nombre}));
 const files=roots.map((r,i)=>({idCliente:'a'+(i+1),raizIdCliente:r.idCliente,nombre:fileNames[i],rutaRelativa:r.nombre+'/'+fileNames[i],bytes:String(bytes.length),ultimaModificacionLocal:'0'}));
 const plan=await client.request('PrepararPlan',{nivelRef:p.nivelRef,versionId:p.versionId,raices:roots,archivos:files});
 if(id(plan?.planId))onReference({planId:plan.planId});
 check(id(plan?.planId)&&plan.nivelRef===p.nivelRef&&plan.snapshot?.VersionId===p.versionId&&plan.snapshot.Configuracion.ModoIntegridad==='COMPLETO_OBLIGATORIO'&&plan.snapshot.Configuracion.AmbitoIntegridad==='LOTE'&&plan.resultado?.PuedeConfirmar===true&&plan.resultado.UnidadesElegibles?.length===2,'PLAN_NOT_ELIGIBLE');
 for(let i=0;i<2;i++){
  const values=one(plan.resultado.Elementos,e=>e.Ambito==='RAIZ'&&e.RaizIdCliente===roots[i].idCliente).Valores;
  for(const field of ['CODIGO','NOMBRE'])check(one(values,v=>v.DestinoRef===field).Valor===p.rootNames[i],'PLAN_VALUES_CHANGED');
 }
 const lot=await client.request('ConfirmarPlan',{planId:plan.planId,revisionEsperada:plan.revision,planHashEsperado:plan.planHash,claveConfirmacion:crypto.randomBytes(16).toString('hex')});
 if(id(lot?.loteId))onReference({planId:plan.planId,loteId:lot.loteId});
 check(id(lot?.loteId)&&!['18','21'].includes(lot.loteId)&&lot.nivelRef===p.nivelRef,'CONFIRMATION_INVALID');
 const initialRoots=await client.pages(lot.loteId,'RAICES'),initialFiles=await client.pages(lot.loteId,'ARCHIVOS');
 check(initialRoots.length===2&&initialFiles.length===2,'ORIGINAL_INVENTORY_CHANGED');
 for(let i=0;i<2;i++)await client.receive(lot.loteId,one(initialFiles,f=>f.idCliente===files[i].idCliente),bytes,fileNames[i]);
 async function advance(fase,tipo,clientId){
  const row=one(await client.pages(lot.loteId,tipo),r=>r.idCliente===clientId);
  const command={loteId:lot.loteId,recursoId:row.recursoId,fase,revisionEsperada:row.revision,claveOperacion:crypto.randomBytes(16).toString('hex')};
  const result=await client.request('AvanzarLote',command);
  check(result?.estado==='CONFIRMADO'&&result.loteId===lot.loteId&&result.fase===fase&&(fase==='CREAR_EXPEDIENTE'?result.raizId:result.archivoId)===row.recursoId,'PHASE_NOT_CONFIRMED');
  if(fase==='CREAR_EXPEDIENTE'){check(id(result.expedienteId)&&id(result.relacionNivelId),'ROOT_IDENTITY_INVALID');onReference({raizId:row.recursoId,expedienteId:result.expedienteId,relacionNivelId:result.relacionNivelId});}
 }
 for(const root of roots)await advance('CREAR_EXPEDIENTE','RAICES',root.idCliente);
 for(const file of files)await advance('MATERIALIZAR_VALORES','ARCHIVOS',file.idCliente);
 const view=await controls.snapshot(lot.loteId),context=JSON.parse(view.lot.contexto_doc);
 check(context.UsuarioRef===actorRef&&context.AmbitoRef===ambitoRef&&context.NivelRef===p.nivelRef&&view.lot.estado==='EN_CURSO'&&!Number(view.lot.detencion_solicitada)&&Number(view.lot.total_raices)===2&&Number(view.lot.total_archivos)===2&&String(view.lot.total_bytes)===String(bytes.length*2)&&view.roots.length===2&&view.files.length===2,'PERSISTED_CONTEXT_CHANGED');
 check(view.roots.every(r=>r.estado==='EXPEDIENTE_CONFIRMADO'&&!r.intento_activo_id)&&!view.attempts.some(a=>['RESERVADO','EN_CURSO','INCIERTO'].includes(a.estado)),'CLAIM_NOT_CLEAR');
 const selected=files.map((file,i)=>{
  const f=one(view.files,x=>x.cliente_archivo_id===file.idCliente),r=one(view.roots,x=>String(x.raiz_id)===String(f.raiz_id));
  check(String(f.bytes_previstos)===String(bytes.length),'ORIGINAL_INVENTORY_CHANGED');
  check(r.cliente_raiz_id===roots[i].idCliente&&f.estado_recepcion==='RECIBIDO'&&f.estado_efecto==='NO_INICIADO'&&!f.intento_activo_id&&!f.resultado_doc,'RESOURCE_NOT_ELIGIBLE');
  effective(JSON.parse(f.valores_efectivos_doc),p.expectations[i]);
  const identity=JSON.parse(r.identidad_expediente_doc);check(id(identity.ExpedienteId)&&id(identity.RelacionNivelId)&&identity.NivelRef===p.nivelRef,'ROOT_IDENTITY_INVALID');
  return{file:f,identity};
 });
 check(selected[0].identity.ExpedienteId!==selected[1].identity.ExpedienteId,'ROOT_IDENTITY_COLLISION');
 const prepared={loteId:lot.loteId,archivoAId:String(selected[0].file.archivo_id),archivoBId:String(selected[1].file.archivo_id),revisionA:Number(selected[0].file.revision),claveA:crypto.randomBytes(16).toString('hex'),claveB:crypto.randomBytes(16).toString('hex'),snapshotSha256:hash(view.lot.config_doc)};
 const command=prepareCommand(prepared);
 return{scenario,actorRef,ambitoRef,profile:p,fileNames,prepared,command,resourceSha256:hash(JSON.stringify(command)),inventorySha256:inventoryFingerprint(view),targets:{expA:Number(selected[0].identity.ExpedienteId),expB:Number(selected[1].identity.ExpedienteId),nameA:fileNames[0],nameB:fileNames[1]},originalTotalBytes:String(view.lot.total_bytes)};
}
module.exports={caseProfile,effective,prepareCase,inventoryFingerprint};
