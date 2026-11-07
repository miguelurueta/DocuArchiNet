'use strict';
const {sha}=require('./doc117-alm03c-scope.cjs');
const {compose}=require('./doc117-alm03c-post-read.cjs');
const check=(v,c)=>{if(!v)throw Error('DOC117_ALM03C_READBACK_'+c);};
// Reconstruct only the already-hashed baseline by selecting prior records while
// excluding the one individual document identified by the native method exit.
// No result is fabricated or persisted: the original aggregate hash must match.
function baselineQuery(query,scope,ref){
 check(ref?.resourceKey==='INDIVIDUAL'&&ref.expedienteId==='467'&&ref.relacionNivelId==='1072'&&[ref.documentoId,ref.inventarioId].every(v=>typeof v==='string'&&/^[1-9][0-9]{0,9}$/.test(v)&&BigInt(v)<=4294967295n),'REFERENCE_INVALID');
 check(/^[A-Za-z_][A-Za-z0-9_]{0,63}$/.test(scope.individual.cabinetName),'CABINET_INVALID');
 const cabinet='`'+scope.individual.cabinetName+'`',rules=[
  ['RA','SELECT ID_REGISTRO_PRODUCION_DOCUMENTAL FROM registro_producion_documental WHERE ID_EMPRESA_DOCUMENTO=? AND EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE=? AND SEGUNDO_NOMBRE_DOCUMENTO=? LIMIT 2',' LIMIT 2',' AND ID_REGISTRO_PRODUCION_DOCUMENTAL<>? LIMIT 2',ref.inventarioId],
  ['DA','SELECT COUNT(*) AS total FROM '+cabinet+' WHERE ID_EXPEDIENTE=?','',' AND ID<>?',ref.documentoId],
  ['RA','SELECT COUNT(*) AS total FROM registro_producion_documental WHERE EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE=? AND ID_EMPRESA_DOCUMENTO=2','',' AND ID_REGISTRO_PRODUCION_DOCUMENTAL<>?',ref.inventarioId],
  ['RA','SELECT COUNT(*) AS total FROM ra_cert_indice_expediente WHERE expediente_archivo_ID_EXPEDIENTE=?','',' AND registro_producion_documental_ID_REGISTRO_PRODUCION_DOCUMENTAL<>?',ref.inventarioId],
  ['DA','SELECT g.* FROM '+cabinet+' g WHERE g.ID_EXPEDIENTE=? ORDER BY g.ID LIMIT 2001',' ORDER BY',' AND g.ID<>? ORDER BY',ref.documentoId],
  ['RA','SELECT r.* FROM registro_producion_documental r WHERE r.EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE=? AND r.ID_EMPRESA_DOCUMENTO=2 ORDER BY r.ID_REGISTRO_PRODUCION_DOCUMENTAL LIMIT 2001',' ORDER BY',' AND r.ID_REGISTRO_PRODUCION_DOCUMENTAL<>? ORDER BY',ref.inventarioId],
  ['RA','SELECT i.* FROM ra_cert_indice_expediente i WHERE i.expediente_archivo_ID_EXPEDIENTE=? ORDER BY i.id_cert_indice_expediente LIMIT 2001',' ORDER BY',' AND i.registro_producion_documental_ID_REGISTRO_PRODUCION_DOCUMENTAL<>? ORDER BY',ref.inventarioId]
 ];
 const seen=new Set();
 const wrapped=async(origin,sql,args)=>{const index=rules.findIndex(r=>r[0]===origin&&r[1]===sql);if(index<0)return query(origin,sql,args);check(!seen.has(index)&&Array.isArray(args)&&(index===0?args.length===3&&args[0]==='2'&&args[1]==='467'&&args[2]===scope.individual.fileName:args.length===1&&args[0]==='467'),'BASELINE_QUERY_CHANGED');seen.add(index);const r=rules[index],changed=r[2]?sql.replace(r[2],r[3]):sql+r[3];return query(origin,changed,[...args,r[4]]);};
 wrapped.verifyComplete=()=>check(seen.size===rules.length,'BASELINE_QUERY_INCOMPLETE');return wrapped;
}
async function reconstructBaseline({query,scope,profile,before,reference}){
 const wrapped=baselineQuery(query,scope,reference),value=await compose(wrapped,scope,profile).individualBefore();wrapped.verifyComplete();
 check(sha(JSON.stringify(value))===before.individualSha256&&JSON.stringify(value.counts)===JSON.stringify(before.individualCounts),'BASELINE_HASH_CHANGED');return value;
}
module.exports={baselineQuery,reconstructBaseline};
