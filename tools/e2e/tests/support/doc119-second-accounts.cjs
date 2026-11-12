'use strict';
const {id,check,BASE_URL}=require('./doc119-seguimiento.cjs');

function sourceLot(source,runId){
  check(/^[a-f0-9]{32}$/.test(runId)&&source?.runId===runId&&source.authorizedPlan===true&&source.authorizationRevision==='20261112-http600'&&source.baseUrl===BASE_URL&&id(source.resources?.lotId),'ACCOUNT_LOOKUP_SOURCE_INVALID');
  return source.resources.lotId;
}

async function findSecondAccounts(query,lotId){
  check(id(lotId),'ACCOUNT_LOOKUP_LOT_INVALID');
  const lots=await query('SELECT solicitante_ref AS owner_ref,ambito_ref AS company_ref FROM ra_integracion_lote WHERE lote_id=? LIMIT 2',[lotId]);
  check(lots.length===1&&id(String(lots[0].owner_ref))&&id(String(lots[0].company_ref)),'ACCOUNT_LOOKUP_SCOPE_INVALID');
  const ownerRef=String(lots[0].owner_ref),companyRef=String(lots[0].company_ref);
  // Same base authority as ImportacionCarpetasContextAdapter. Duplicate
  // identities/profile rows are excluded rather than treated as usable logins.
  const rows=await query("SELECT u.id_Remit_Dest_Int AS user_ref,u.Login_Usuario AS login,u.Empresa_Gestion_Documental_id_empresa AS company_ref,u.Estado_Usuario AS active,p.IMPORTAR_CARPETAS AS can_import FROM remit_dest_interno u INNER JOIN empresa_gestion_documental e ON e.ID_EMPRESA=u.Empresa_Gestion_Documental_id_empresa INNER JOIN remit_dest_interno_perfil_produccion p ON p.remit_dest_interno_idremit_dest_interno=u.id_Remit_Dest_Int WHERE u.Empresa_Gestion_Documental_id_empresa=? AND u.id_Remit_Dest_Int<>? AND u.Estado_Usuario=1 AND p.IMPORTAR_CARPETAS=1 AND TRIM(u.Login_Usuario)<>'' AND UPPER(TRIM(u.Login_Usuario))<>'CONSULTAPUBLICO' AND (SELECT COUNT(*) FROM remit_dest_interno_perfil_produccion p2 WHERE p2.remit_dest_interno_idremit_dest_interno=u.id_Remit_Dest_Int)=1 AND (SELECT COUNT(*) FROM remit_dest_interno u2 WHERE u2.Login_Usuario=u.Login_Usuario)=1 ORDER BY u.Login_Usuario,u.id_Remit_Dest_Int LIMIT 30",[companyRef,ownerRef]);
  check(rows.length<=30,'ACCOUNT_LOOKUP_RESULT_INVALID');
  const seen=new Set();
  const candidates=rows.map(row=>{
    const userRef=String(row.user_ref),login=typeof row.login==='string'?row.login.trim():'';
    check(id(userRef)&&userRef!==ownerRef&&String(row.company_ref)===companyRef&&String(row.active)==='1'&&String(row.can_import)==='1'&&login.length>0&&login.length<=128&&!/[\u0000-\u001f\u007f-\u009f]/.test(login)&&login.toUpperCase()!=='CONSULTAPUBLICO'&&!seen.has(login.toUpperCase()),'ACCOUNT_LOOKUP_RESULT_INVALID');
    seen.add(login.toUpperCase());return {userRef,login};
  });
  return {lotId,ownerRef,companyRef,candidates};
}
module.exports={sourceLot,findSecondAccounts};
