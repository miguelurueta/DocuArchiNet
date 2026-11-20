'use strict';
const {SQL,hash,id}=require('./doc120-qa-window.cjs');
const SCOPE=Object.freeze({lot:'52',root:'73',owner:'141',company:'2',expedition:'696',relation:'1120',files:1000,maxSelect:10});
const check=(ok,code)=>{if(!ok)throw Error('DOC120_COMPLETED_'+code);};
const object=value=>{try{const result=JSON.parse(value);check(result&&typeof result==='object'&&!Array.isArray(result),'JSON_INVALID');return result;}catch{throw Error('DOC120_COMPLETED_JSON_INVALID');}};
const args=()=>[SCOPE.lot,SCOPE.lot,SCOPE.owner,SCOPE.company];
function header(rows){
 check(rows.length===1,'LOT_NOT_FOUND');const h=rows[0];
 check(String(h.lote_id)===SCOPE.lot&&Number(h.total_raices)===1&&Number(h.total_archivos)===SCOPE.files&&id(String(h.nivel_ref)),'LOT_SCOPE_CHANGED');
 check(h.estado==='COMPLETADO'&&!Number(h.detencion_solicitada)&&Number(h.unresolved)===0,'LOT_NOT_COMPLETED');
 check(hash(h.config_doc)==='9140cc7400c96a17e717372664f3db51a497e63c9348fbe2915520e4f6e28e82','CONFIG_CHANGED');
 const config=object(h.config_doc);check(config.Configuracion?.OrigenGabinete==='DA'&&String(config.Configuracion.GabineteId)==='10'&&String(config.VersionId)===String(h.version_id),'CABINET_CHANGED');return h;
}
function files(rows){
 check(rows.length===SCOPE.files,'FILE_COUNT_INVALID');const documents=new Map(),inventories=new Set(),fileIds=new Set();let bytes=0;
 for(const f of rows){
  check(String(f.lote_id)===SCOPE.lot&&String(f.raiz_id)===SCOPE.root&&id(String(f.archivo_id))&&!fileIds.has(String(f.archivo_id)),'FILE_SCOPE_CHANGED');fileIds.add(String(f.archivo_id));
  check(f.estado_recepcion==='RECIBIDO'&&f.estado_efecto==='COMPLETADO'&&!Number(f.active_marker)&&Number(f.fixed_values)===1,'FILE_NOT_COMPLETED');
  check(Number.isSafeInteger(Number(f.bytes_previstos))&&Number(f.bytes_previstos)>0&&String(f.bytes_recibidos)===String(f.bytes_previstos),'RECEIVED_BYTES_INVALID');bytes+=Number(f.bytes_previstos);
  const result=object(f.resultado_doc);check(result.Estado==='CONFIRMADO'&&id(result.DocumentoId)&&id(result.InventarioId)&&result.Expediente?.ExpedienteId===SCOPE.expedition&&result.Expediente?.RelacionNivelId===SCOPE.relation,'FILE_RESULT_INVALID');
  check(!documents.has(result.DocumentoId)&&!inventories.has(result.InventarioId),'DUPLICATE_DOCUMENT');inventories.add(result.InventarioId);documents.set(result.DocumentoId,{fileId:String(f.archivo_id),documentId:result.DocumentoId,inventoryId:result.InventarioId});
 }
 check(bytes===44859102,'INVENTORY_BYTES_CHANGED');return {documents,bytes};
}
function documentSql(cabinet){
 check(typeof cabinet==='string'&&/^[A-Za-z0-9_]{1,64}$/.test(cabinet),'CABINET_NAME_INVALID');
 return 'SELECT g.ID AS document_id,g.ID_INVENTARIO_DOCUMENTAL AS inventory_id,g.ID_EXPEDIENTE AS expedient_id,g.ID_USUARIO_GESTION AS document_user,r.ID_EMPRESA_DOCUMENTO AS company_id,r.ID_USUARIO_GESTION AS inventory_user,r.ID_DOCUMENTO_DOCUARCHI_ALMACEN AS inventory_document,r.EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE AS inventory_expedient,r.NOMBRE_GABINETE AS cabinet_name,i.expediente_archivo_ID_EXPEDIENTE AS indexed_expedient FROM `'+cabinet+'` g LEFT JOIN registro_producion_documental r ON r.ID_REGISTRO_PRODUCION_DOCUMENTAL=g.ID_INVENTARIO_DOCUMENTAL LEFT JOIN ra_cert_indice_expediente i ON i.registro_producion_documental_ID_REGISTRO_PRODUCION_DOCUMENTAL=r.ID_REGISTRO_PRODUCION_DOCUMENTAL WHERE g.ID_EXPEDIENTE=? ORDER BY g.ID,r.ID_REGISTRO_PRODUCION_DOCUMENTAL,i.expediente_archivo_ID_EXPEDIENTE LIMIT 1002';
}
function verifyDocuments(rows,expected,cabinet){
 check(rows.length===SCOPE.files,'DA_DOCUMENT_COUNT_INVALID');const observed=new Set();
 for(const r of rows){const key=String(r.document_id),proof=expected.get(key);
  check(proof&&!observed.has(key),'DA_DUPLICATE_OR_UNKNOWN_DOCUMENT');observed.add(key);
  check(String(r.inventory_id)===proof.inventoryId&&String(r.expedient_id)===SCOPE.expedition&&String(r.document_user)===SCOPE.owner&&String(r.company_id)===SCOPE.company&&String(r.inventory_user)===SCOPE.owner&&String(r.inventory_document)===key&&String(r.inventory_expedient)===SCOPE.expedition&&r.cabinet_name===cabinet&&String(r.indexed_expedient)===SCOPE.expedition,'DA_RELATION_INVALID');
 }
}
async function inspect(read){
 const before=header(await read('RA','before',SQL.headers,args()));
 const roots=await read('RA','root',SQL.roots,args());check(roots.length===1&&String(roots[0].raiz_id)===SCOPE.root&&String(roots[0].lote_id)===SCOPE.lot&&!roots[0].intento_activo_id,'ROOT_CHANGED');
 const identity=object(roots[0].identidad_expediente_doc);check(identity.ExpedienteId===SCOPE.expedition&&identity.RelacionNivelId===SCOPE.relation,'EXPEDITION_CHANGED');
 const beforeFiles=await read('RA','files',SQL.files,args()),inventory=files(beforeFiles);
 const attempts=await read('RA','attempts',"SELECT i.fase,i.estado,COUNT(*) AS total,COUNT(DISTINCT i.archivo_id) AS files FROM ra_integracion_lote l INNER JOIN ra_integracion_intento i ON i.lote_id=l.lote_id WHERE l.lote_id=? AND l.solicitante_ref=? AND l.ambito_ref=? GROUP BY i.fase,i.estado ORDER BY i.fase,i.estado",[SCOPE.lot,SCOPE.owner,SCOPE.company]);
 check(attempts.every(i=>!['RESERVADO','EN_CURSO','INCIERTO'].includes(i.estado)),'UNRESOLVED_ATTEMPT');
 for(const phase of ['RECEPCION','ALMACENAR']){const rows=attempts.filter(i=>i.fase===phase&&i.estado==='CONFIRMADO');check(rows.length===1&&Number(rows[0].total)===SCOPE.files&&Number(rows[0].files)===SCOPE.files,'CONFIRMED_ATTEMPTS_INVALID');}
 check(attempts.filter(i=>i.fase==='CREAR_EXPEDIENTE'&&i.estado==='CONFIRMADO').reduce((n,i)=>n+Number(i.total),0)===1,'CREATION_COUNT_INVALID');
 const cabinets=await read('DA','cabinet','SELECT id_gabinete,NOMBRE FROM system1 WHERE id_gabinete=? LIMIT 2',['10']);check(cabinets.length===1&&String(cabinets[0].id_gabinete)==='10','CABINET_CHANGED');const cabinet=cabinets[0].NOMBRE,sql=documentSql(cabinet);
 const documents=await read('DA','documents',sql,[SCOPE.expedition]);verifyDocuments(documents,inventory.documents,cabinet);
 const relations=await read('RA','relation','SELECT e.ID_EXPEDIENTE AS expedition_id,e.ID_EMPRESA_EXPEDIENTE AS company_id,e.ID_USUARIO_GESTION AS owner_id,r.id_registro AS relation_id,r.ra_pro_niveles_id_nivel AS level_id FROM ra_pro_niveles_has_expediente_archivo r INNER JOIN expediente_archivo e ON e.ID_EXPEDIENTE=r.expediente_archivo_ID_EXPEDIENTE WHERE e.ID_EXPEDIENTE=? ORDER BY r.id_registro LIMIT 3',[SCOPE.expedition]);
 check(relations.length===1&&String(relations[0].expedition_id)===SCOPE.expedition&&String(relations[0].relation_id)===SCOPE.relation&&String(relations[0].company_id)===SCOPE.company&&String(relations[0].owner_id)===SCOPE.owner&&String(relations[0].level_id)===String(before.nivel_ref),'EXPEDITION_RELATION_INVALID');
 const afterFiles=await read('RA','files_after',SQL.files,args());check(hash(afterFiles)===hash(beforeFiles),'READ_CHANGED');
 const afterDocuments=await read('DA','documents_after',sql,[SCOPE.expedition]);check(hash(afterDocuments)===hash(documents),'READ_CHANGED');
 const after=header(await read('RA','after',SQL.headers,args()));check(hash(before)===hash(after),'READ_CHANGED');
 return {lotId:SCOPE.lot,state:'COMPLETADO',received:SCOPE.files,stored:SCOPE.files,receivedBytes:inventory.bytes,uniqueDocuments:inventory.documents.size,uniqueInventories:inventory.documents.size,expeditionId:SCOPE.expedition,relationId:SCOPE.relation,levelId:String(before.nivel_ref),versionId:String(before.version_id),planId:String(before.plan_id),revision:Number(before.revision),stableRead:true,unresolved:0,creationAttemptsConfirmed:1,attempts,checkpointFingerprint:hash(beforeFiles),documentRelationFingerprint:hash(documents),proofs:[...inventory.documents.values()]};
}
module.exports={SCOPE,header,files,documentSql,verifyDocuments,inspect};
