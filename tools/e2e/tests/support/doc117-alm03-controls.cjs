'use strict';
const fs=require('node:fs'),path=require('node:path');
const {ROOT,sha,verifyIndividual}=require('./doc117-alm03-setup-scope.cjs');
const {createControls:sharedControls}=require('./doc117-alm02-internal.cjs');
const {preservation}=require('./doc116-exp01-setup.cjs');
const {check,id}=require('./doc117-alm03-coexistence.cjs');
const {validateBinding,readBinding}=require('../../scripts/support/doc117-alm03-canonical-description.cjs');
const one=r=>{check(Array.isArray(r)&&r.length===1,'CONTROL_NOT_UNIQUE');return r[0];};
const ident=v=>{check(typeof v==='string'&&/^[A-Za-z_][A-Za-z0-9_]{0,63}$/.test(v),'IDENTIFIER_INVALID');return '`'+v+'`';};
const digest=v=>sha(JSON.stringify(v));
function expectedIndex(mapping,value){
 const from=String(mapping.tipo_campo).toUpperCase(),to=String(mapping.TIPO).toUpperCase();
 if(value===null||value===undefined)return ['INT','LONG'].includes(from)?'0':'';
 if(from==='VARCHAR'){const m=/^VARCHAR\(([1-9][0-9]{0,4})\)$/.exec(to);check(m,'INDEX_TYPE_UNSUPPORTED');return String(value).slice(0,Number(m[1]));}
 check(['INT','LONG','DATE','DATETIME'].includes(from)&&(to===from||from==='LONG'&&to==='INT'),'INDEX_TYPE_UNSUPPORTED');
 return String(value);
}
function sameIndex(actual,wanted,mapping){
 const type=String(mapping.tipo_campo).toUpperCase();
 if(['INT','LONG'].includes(type)){check(/^-?[0-9]+$/.test(String(actual))&&/^-?[0-9]+$/.test(wanted),'INDEX_INTEGER_INVALID');return BigInt(actual)===BigInt(wanted);}
 return (actual===null?'':String(actual))===wanted;
}
function createControls(query,scope,profile,options={}){
 const descriptionBinding=options.descriptionBinding?validateBinding(options.descriptionBinding):null;
 const description=descriptionBinding?descriptionBinding.description:'Comprobante de Egreso';
 const base=sharedControls(query,scope,profile,ROOT),individual=scope.individual,cabinet=ident(individual.cabinetName);
 async function classification(){
  const row=one(await query('RA','SELECT ID_EXPEDIENTE,ID_EMPRESA_EXPEDIENTE,GABINETE_PRODUCION,id_instrumento,CODIGO_AREA_TRD,CODIGO_SERIE_TRD,CODIGO_SUB_SERIE_TRD FROM expediente_archivo WHERE ID_EXPEDIENTE=? AND ID_EMPRESA_EXPEDIENTE=? LIMIT 2',['467','2']));check(digest(row)===individual.classificationSha256,'INDIVIDUAL_CLASSIFICATION_CHANGED');return row;
 }
 async function mapping(){
  const defaults=await query('DA','SELECT id_relacion_gab_expe FROM ra_gabexp_relacion_index_gabinete_expediente WHERE system1_id_gabinete=? AND default_relacion=1 LIMIT 2',['10']);
  check(Array.isArray(defaults)&&defaults.length<=1,'DEFAULT_MAPPING_AMBIGUOUS');
  // UploadSaveFile calls the existing preparer with ObligaViculoExpeGabinete=0.
  // With no default relation that preparer supplies no external mapped fields.
  if(defaults.length===0)return[];const rel=defaults[0];
  const rows=await query('DA','SELECT rgc.nombre_campo,rgc.tipo_campo,dg.id_detalle_gabinete,dg.CAMPO,dg.TIPO FROM ra_gabexp_relacion_campos_gabinete_expediente rgr INNER JOIN detalle_gabienete dg ON dg.id_detalle_gabinete=rgr.detalle_gabienete_id_detalle_gabinete INNER JOIN ra_gabexp_campos_detalle_expediente rgc ON rgc.id_campo_detalle_expediente=rgr.Ra_gabexp_campos_detalle_expediente_id_campo_detalle_expediente WHERE rgr.relacion_index_gabinete_expediente_id_relacion_gab_expe=? ORDER BY dg.id_detalle_gabinete LIMIT 33',[String(rel.id_relacion_gab_expe)]);
  check(rows.length>0&&rows.length<=32&&new Set(rows.map(r=>r.CAMPO.toLowerCase())).size===rows.length,'INDEX_MAPPING_AMBIGUOUS');for(const r of rows){ident(r.nombre_campo);ident(r.CAMPO);}return rows;
 }
 async function canonicalBefore(){
  if(descriptionBinding)check((await readBinding(query)).descriptionSha256===descriptionBinding.descriptionSha256,'INDIVIDUAL_DESCRIPTION_CHANGED');
  const fields=await query('DA','SELECT CAMPO,TIPO,VISIBLE FROM detalle_gabienete WHERE id_gabinete=? ORDER BY id_detalle_gabinete LIMIT 65',['10']);
  check(fields.length<=64&&new Set(fields.map(f=>String(f.CAMPO).toUpperCase())).size===fields.length&&fields.every(f=>['0','1'].includes(String(f.VISIBLE))),'CANONICAL_METADATA_INVALID');
  const exp=one(await query('RA','SELECT CODIGO_UNICO,NOMBRE_SERIE_TRD,NOMBRE_SUBSERIE_TRD FROM expediente_archivo WHERE ID_EXPEDIENTE=? AND ID_EMPRESA_EXPEDIENTE=? LIMIT 2',['467','2']));
  check(exp.CODIGO_UNICO!==null&&exp.CODIGO_UNICO!==undefined,'CANONICAL_EXPEDIENT_CODE_UNAVAILABLE');
  const day=new Date(),storageDate=[day.getFullYear(),String(day.getMonth()+1).padStart(2,'0'),String(day.getDate()).padStart(2,'0')].join('-');
  const expected={NOMBRE:description,EXPEDIENTE:String(exp.CODIGO_UNICO),CLASEDOCUMENTO:'DOCUMENTO ELECTRONICO',FECHAELABORACION:storageDate,TIPODOCUMENTO:description,NOMBRESERIE:String(exp.NOMBRE_SERIE_TRD??''),NOMBRESUBSERIE:String(exp.NOMBRE_SUBSERIE_TRD??'')};
  const selected=fields.filter(f=>String(f.VISIBLE)==='1'&&Object.hasOwn(expected,String(f.CAMPO).toUpperCase())).map(f=>{ident(f.CAMPO);const key=f.CAMPO.toUpperCase();return{field:f.CAMPO,type:f.TIPO,value:expected[key],date:key==='FECHAELABORACION'};});
  check(['EXPEDIENTE','CLASEDOCUMENTO','TIPODOCUMENTO'].every(k=>selected.some(f=>f.field.toUpperCase()===k)),'CANONICAL_FIELDS_UNAVAILABLE');
  return{metadataSha256:digest(fields),fields:selected,storageDate,...(descriptionBinding?{descriptionSha256:descriptionBinding.descriptionSha256}:{})};
 }
 async function individualBefore(){
  await verifyIndividual(query,{individual,fixture:{bytes:677,sha256:individual.contentSha256}});const selected=await classification(),fields=await mapping();
  const values=fields.length?one(await query('DA','SELECT '+fields.map((r,i)=>'CAST('+ident(r.nombre_campo)+' AS CHAR) AS value_'+i).join(',')+' FROM expediente_archivo WHERE ID_EXPEDIENTE=? LIMIT 2',['467'])):{};
  const indices=fields.map((r,i)=>({mapping:r,value:expectedIndex(r,values['value_'+i])}));
  const canonical=await canonicalBefore();
  const counts=[];for(const [origin,sql]of [['DA','SELECT COUNT(*) AS total FROM '+cabinet+' WHERE ID_EXPEDIENTE=?'],['RA','SELECT COUNT(*) AS total FROM registro_producion_documental WHERE EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE=? AND ID_EMPRESA_DOCUMENTO=2'],['RA','SELECT COUNT(*) AS total FROM ra_cert_indice_expediente WHERE expediente_archivo_ID_EXPEDIENTE=?']]){const n=String(one(await query(origin,sql,['467'])).total);check(/^[0-9]+$/.test(n),'INDIVIDUAL_COUNT_INVALID');counts.push(n);}
  const retained=await individualPreserved();
  return{classification:selected,indices,canonical,counts,retained}; // RAM only: never persisted.
 }
 async function individualPreserved(ref){
  const records=[];
  for(const [origin,sql,args]of [
   ['DA','SELECT g.* FROM '+cabinet+' g WHERE g.ID_EXPEDIENTE=?'+(ref?' AND g.ID<>?':'')+' ORDER BY g.ID LIMIT 2001',ref?['467',ref.documentoId]:['467']],
   ['RA','SELECT r.* FROM registro_producion_documental r WHERE r.EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE=? AND r.ID_EMPRESA_DOCUMENTO=2'+(ref?' AND r.ID_REGISTRO_PRODUCION_DOCUMENTAL<>?':'')+' ORDER BY r.ID_REGISTRO_PRODUCION_DOCUMENTAL LIMIT 2001',ref?['467',ref.inventarioId]:['467']],
   ['RA','SELECT i.* FROM ra_cert_indice_expediente i WHERE i.expediente_archivo_ID_EXPEDIENTE=?'+(ref?' AND i.registro_producion_documental_ID_REGISTRO_PRODUCION_DOCUMENTAL<>?':'')+' ORDER BY i.id_cert_indice_expediente LIMIT 2001',ref?['467',ref.inventarioId]:['467']]
  ]){const rows=await query(origin,sql,args);check(Array.isArray(rows)&&rows.length<2001,'PRESERVATION_LIMIT');records.push({count:rows.length,sha256:digest(rows)});}
  return records;
 }
 async function individualPhysical(ref,before){
  check(ref?.expedienteId==='467'&&id(ref.documentoId)&&id(ref.inventarioId)&&Array.isArray(before?.indices)&&before?.canonical?.fields?.length>=3,'INDIVIDUAL_PROOF_INVALID');
  check(digest(await classification())===digest(before.classification)&&digest(await mapping())===digest(before.indices.map(x=>x.mapping)),'INDIVIDUAL_DESTINATION_CHANGED');
  check(digest(await canonicalBefore())===digest(before.canonical),'INDIVIDUAL_CANONICAL_DESTINATION_CHANGED');
  const relationship=one(await query('RA','SELECT ra_pro_niveles_id_nivel FROM ra_pro_niveles_has_expediente_archivo WHERE id_registro=? AND expediente_archivo_ID_EXPEDIENTE=? LIMIT 2',['1072','467']));check(String(relationship.ra_pro_niveles_id_nivel)==='8','INDIVIDUAL_RELATION_CHANGED');
  const selected=[...before.indices.map((x,i)=>'CAST(g.'+ident(x.mapping.CAMPO)+' AS CHAR) AS value_'+i),...before.canonical.fields.map((x,i)=>'CAST(g.'+ident(x.field)+' AS CHAR) AS canonical_'+i)];
  const row=one(await query('DA','SELECT g.ID,g.ID_EXPEDIENTE,g.ID_USUARIO_GESTION,g.NOMBRE,g.DBT,r.ID_CLASE_DOCUMENTO,r.ID_TIPO_DOCUMENTO,r.ID_EMPRESA_DOCUMENTO,r.ID_USUARIO_GESTION AS inventory_user,r.SEGUNDO_NOMBRE_DOCUMENTO,r.FORMATO AS inventory_format,r.ID_AREA_DEPARTAMENTO,r.ID_SERIE_DOCUMENTO,r.ID_SUBSERIE_DOCUMENTO,i.segundo_nombre,i.formato AS index_format,i.expediente_archivo_ID_EXPEDIENTE AS indexed_expedition,'+selected.join(',')+' FROM '+cabinet+' g INNER JOIN registro_producion_documental r ON r.ID_REGISTRO_PRODUCION_DOCUMENTAL=g.ID_INVENTARIO_DOCUMENTAL AND r.ID_DOCUMENTO_DOCUARCHI_ALMACEN=g.ID AND r.EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE=g.ID_EXPEDIENTE AND r.NOMBRE_GABINETE=? INNER JOIN ra_cert_indice_expediente i ON i.registro_producion_documental_ID_REGISTRO_PRODUCION_DOCUMENTAL=r.ID_REGISTRO_PRODUCION_DOCUMENTAL WHERE g.ID=? AND g.ID_INVENTARIO_DOCUMENTAL=? LIMIT 2',[individual.cabinetName,ref.documentoId,ref.inventarioId]));
  const cl=one(await query('RA','SELECT ID_TIPO_DOCUMENTO AS id FROM ra_tipo_documento WHERE DOCUMENTO=? LIMIT 2',['DOCUMENTO ELECTRONICO'])),ext=one(await query('DA','SELECT ESTADO_NORMAL AS id FROM DA_EXTENSION WHERE ESTENSION=? LIMIT 2',['.PDF']));
  const names=await query('DA',"SELECT VISIBLE FROM detalle_gabienete WHERE GABINETE=? AND UPPER(CAMPO)='NOMBRE' LIMIT 2",[individual.cabinetName]);check(names.length<=1&&names.every(x=>['0','1'].includes(String(x.VISIBLE))),'NAME_METADATA_INVALID');
  const visible=names.length===1&&String(names[0].VISIBLE)==='1';
  if(descriptionBinding)check(row.NOMBRE===description||!visible&&(row.NOMBRE===null||row.NOMBRE===''),'INDIVIDUAL_DESCRIPTION_VALUE_CHANGED');
  check(String(row.ID)===ref.documentoId&&['ID_EXPEDIENTE','indexed_expedition'].every(k=>String(row[k])==='467')&&['ID_USUARIO_GESTION','inventory_user'].every(k=>String(row[k])==='141')&&String(row.ID_EMPRESA_DOCUMENTO)==='2'&&String(row.ID_TIPO_DOCUMENTO)==='50'&&String(row.ID_CLASE_DOCUMENTO)===String(cl.id)&&String(row.DBT)===String(ext.id)&&['SEGUNDO_NOMBRE_DOCUMENTO','segundo_nombre'].every(k=>row[k]===individual.fileName)&&['inventory_format','index_format'].every(k=>row[k]==='.PDF')&&(row.NOMBRE===description||!visible&&(row.NOMBRE===null||row.NOMBRE==='')),'INDIVIDUAL_PHYSICAL_VALUES_INVALID');
  for(const [a,b]of [['ID_AREA_DEPARTAMENTO','CODIGO_AREA_TRD'],['ID_SERIE_DOCUMENTO','CODIGO_SERIE_TRD'],['ID_SUBSERIE_DOCUMENTO','CODIGO_SUB_SERIE_TRD']])check(String(row[a]??0)===String(before.classification[b]??0),'INDIVIDUAL_INVENTORY_CLASSIFICATION_CHANGED');
  before.indices.forEach((x,i)=>check(sameIndex(row['value_'+i],x.value,x.mapping),'INDIVIDUAL_PHYSICAL_INDEX_CHANGED'));
  before.canonical.fields.forEach((x,i)=>{const actual=row['canonical_'+i],type=String(x.type).toUpperCase();let wanted=x.value;if(x.date){if(type==='DATETIME')wanted+=' 00:00:00';else if(type!=='DATE')wanted=wanted.replaceAll('-','/');}check((actual===null?'':String(actual))===wanted,'INDIVIDUAL_CANONICAL_INDEX_CHANGED');});
  let index=0;for(const [origin,sql]of [['DA','SELECT COUNT(*) AS total FROM '+cabinet+' WHERE ID_EXPEDIENTE=?'],['RA','SELECT COUNT(*) AS total FROM registro_producion_documental WHERE EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE=? AND ID_EMPRESA_DOCUMENTO=2'],['RA','SELECT COUNT(*) AS total FROM ra_cert_indice_expediente WHERE expediente_archivo_ID_EXPEDIENTE=?']])check(BigInt(one(await query(origin,sql,['467'])).total)===BigInt(before.counts[index++])+1n,'INDIVIDUAL_DOCUMENT_COUNT_CHANGED');
  const retained=fs.readFileSync(path.join(ROOT,'tools/e2e/fixtures',individual.fileName));check(retained.length===677&&sha(retained)===individual.contentSha256,'INDIVIDUAL_CLIENT_ORIGINAL_CHANGED');
  check(digest(await individualPreserved(ref))===digest(before.retained),'INDIVIDUAL_PRIOR_DOCUMENTS_CHANGED');
  return{knownIdsVerified:true,incorporations:1,physicalIndicesVerified:true,mappedIndicesVerified:before.indices.length,canonicalIndicesVerified:before.canonical.fields.length,mappingMode:before.indices.length?'DEFAULT_MAPPING':'NO_DEFAULT_MAPPING',classificationVerified:true,clientOriginalVerified:true};
 }
 return{...base,individualBefore,individualPhysical,preservation:bounds=>preservation(query,{identity:{userRef:'141',ambitoRef:'2'},preservedLotIds:['18','19','20','21']},bounds)};
}
module.exports={createControls,expectedIndex,sameIndex};
