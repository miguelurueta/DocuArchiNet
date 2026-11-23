'use strict';
const {test}=require('@playwright/test');
const fs=require('node:fs');
const {createWorkflowLoginPageSession}=require('./support/authenticated-workflow-session.cjs');
const {safePost}=require('./support/doc110-safe-request.cjs');

test.use({screenshot:'off',trace:'off',video:'off'});
test.describe.configure({mode:'serial',retries:0,timeout:180000});
function check(value,code){if(!value)throw new Error(code);}
function required(name){check(!!process.env[name],'DOC122_CONFIG_REQUIRED_'+name);return process.env[name];}
function scope(){
 const s=JSON.parse(fs.readFileSync(required('DOC122_E2E_SCOPE_PATH'),'utf8').replace(/^\uFEFF/,''));
 check(s && s.issue==='DOC-122' && s.disposable===true && typeof s.baseUrl==='string' &&
  Number.isSafeInteger(s.conexionId)&&s.conexionId>0 && Number.isSafeInteger(s.plantillaId)&&s.plantillaId>0 &&
  Number.isSafeInteger(s.revisionEsperada)&&s.revisionEsperada>0 && /^[0-9a-f]{64}$/.test(s.huellaEsperada||'') &&
  Array.isArray(s.allowedModes) && s.allowedModes.every(x=>['read','write'].includes(x)),'DOC122_SCOPE_INVALID');
 check(s.baseUrl===required('DOC122_E2E_BASE_URL'),'DOC122_SCOPE_URL_MISMATCH');
 return s;
}
async function session(browser,restricted=false,prefix='DOC122'){
 const baseUrl=required(prefix+'_E2E_BASE_URL').replace(/\/?$/,'/');
 const auth=await createWorkflowLoginPageSession(browser,{
  baseUrl,moduleEnvironmentVariable:prefix+'_E2E_MODULE',
  userEnvironmentVariable:prefix+(restricted?'_E2E_RESTRICTED_USER':'_E2E_USER'),
  passwordEnvironmentVariable:prefix+(restricted?'_E2E_RESTRICTED_PASSWORD':'_E2E_PASSWORD'),
  ignoreHTTPSErrors:process.env.DOC10_E2E_IGNORE_HTTPS_ERRORS==='true'
 });
 try{
  await auth.page.goto(new URL('Gestion/WebFormProducionDocumental.aspx',baseUrl).href);
  return {...auth,baseUrl};
 }catch{await auth.context.close();throw new Error('DOC122_SESSION_FAILED');}
}
async function call(auth,method,payload,token){
 const headers=token?{'X-Importacion-Excel-CSRF':token}:{};
 const response=await safePost(auth.context.request,new URL('webservice/WebServiceImportacionExcel.asmx/'+method,auth.baseUrl).href,
  {headers,data:payload===undefined?{}:{solicitud:payload}},method);
 check(response.status()===200,'DOC122_HTTP_REJECTED');
 let b;try{b=(await response.json()).d;}catch{throw new Error('DOC122_JSON_REQUIRED');}
 check(b && typeof b.ok==='boolean','DOC122_ENVELOPE_REQUIRED');
 return b;
}
async function open(auth,s){
 const r=await call(auth,'AbrirCargaExcel');
 check(r.ok && r.datos && typeof r.datos.CsrfToken==='string' && r.datos.Conexiones.some(c=>c.ConexionId===s.conexionId),'DOC122_OPEN_FAILED');
 auth.token=r.datos.CsrfToken;
 return r.datos;
}
async function detail(auth,s){
 const r=await call(auth,'ConsultarPlantilla',{ConexionId:s.conexionId,PlantillaId:s.plantillaId},auth.token);
 check(r.ok && r.datos,'DOC122_DETAIL_FAILED');return r.datos;
}
function edit(s,p){return {ConexionId:s.conexionId,RevisionEsperada:p.REVISION_CARGA,HuellaEsperada:p.Huella,Plantilla:JSON.parse(JSON.stringify(p))};}

test.describe('DOC125 confirmación durable',()=>{
 test.skip(process.env.DOC125_E2E_AUTHORIZED!=='YES','NO_EJECUTADO: falta autorización escritora DOC125, instalación manual verificada y destinos descartables; DOC124 no autoriza estos efectos.');
 test('@doc125-confirm modos, rollback, resultado perdido y escritor externo',async({browser})=>{
  test.setTimeout(300000);
  const path=require('node:path'),{readScope,readFixture,snapshotDestination,lockNames}=require('./support/doc125-excel-scope.cjs');
  const {queryRows,queryFingerprint}=require('../scripts/support/doc32-e2e-odbc.cjs');
  const {startExcelExternalWriter}=require('../scripts/run-doc110-profile-session.cjs');
  const s=readScope(required('DOC125_E2E_SCOPE_PATH'),required('DOC125_E2E_BASE_URL'));
  const select=(sql,args=[])=>{console.log('DOC125_CONTROL_SELECT');return queryRows(sql,args,process.env,s.controlPrefix);};
  const fingerprint=(sql,value)=>{console.log('DOC125_CONTROL_SELECT');return queryFingerprint(sql,value,process.env,s.controlPrefix);};
  const ident=n=>{check(/^[A-Za-z_][A-Za-z0-9_]{0,63}$/.test(n),'DOC125_CONTROL_IDENTIFIER');return '`'+n+'`';};
  const count=async table=>Number((await select('SELECT COUNT(*) AS total FROM '+ident(table)))[0].total);
  const physical=(await select('SELECT @@hostname AS servidor,@@port AS puerto,DATABASE() AS base_ref,@@lower_case_table_names AS minusculas'))[0];
  const protectedBefore={};for(const table of s.protectedTables)protectedBefore[table]=await snapshotDestination(select,fingerprint,table,true);
  const a=await session(browser,false,'DOC125');let b,completed=0;
  try{
   b=await session(browser,false,'DOC125');await open(a,s.cases[0]);await open(b,s.cases[0]);
   const cookie=async auth=>(await auth.context.cookies()).find(c=>c.name==='ASP.NET_SessionId')?.value;
   const ca=await cookie(a),cb=await cookie(b);check(ca&&cb&&ca!==cb,'DOC125_INDEPENDENT_SESSIONS_REQUIRED');
   for(const c of s.cases){
    const p=await detail(a,c);check(p.REVISION_CARGA===c.revisionEsperada&&p.Huella===c.huellaEsperada&&p.NOMBRE_PLANTILLA===c.tabla,'DOC125_TEMPLATE_CHANGED');
    const bytes=readFixture(c),before=await snapshotDestination(select,fingerprint,c.tabla,c.modo!=='CREAR');
    const beforeCount=c.modo==='CREAR'?0:await count(c.tabla);
    check(beforeCount===c.filasDestinoEsperadas,'DOC125_DESTINATION_BASELINE_CHANGED');
    const started=await call(a,'IniciarRecepcionExcel',{ConexionId:c.conexionId,PlantillaId:c.plantillaId,RevisionEsperada:c.revisionEsperada,HuellaEsperada:c.huellaEsperada},a.token);
    check(started.ok&&started.datos?.IdCarga&&started.datos?.ReferenciaRecepcion,'DOC125_START_FAILED');
    const i=started.datos;
    const response=await safePost(a.context.request,new URL('generic_control/ImportacionExcelUpload.ashx',a.baseUrl).href,{headers:{'X-Importacion-Excel-CSRF':a.token},multipart:{idCarga:i.IdCarga,referenciaRecepcion:i.ReferenciaRecepcion,conexionId:String(c.conexionId),plantillaId:String(c.plantillaId),archivo:{name:path.basename(c.archivoPath),mimeType:'application/octet-stream',buffer:bytes}}},'ExcelUpload');
    let upload;try{upload=await response.json();}catch{throw Error('DOC125_UPLOAD_FAILED');}check(response.status()===200&&upload.ok&&upload.datos?.ArchivoHash===c.archivoHash,'DOC125_UPLOAD_FAILED');
    const owned=await select('SELECT ambito_ref,usuario_ref,conexion_ref,tabla_destino,HEX(archivo_hash) AS archivo_hash FROM imp01_carga_ejecucion WHERE id_carga=?',[i.IdCarga]);
    check(owned.length===1&&String(owned[0].ambito_ref)===String(s.empresaId)&&String(owned[0].usuario_ref)===String(s.usuarioId)&&owned[0].conexion_ref==='DA:'+s.moduloId+':'+c.conexionId&&owned[0].tabla_destino===c.tabla&&owned[0].archivo_hash.toLowerCase()===c.archivoHash,'DOC125_OWNER_ENVIRONMENT_CHANGED');
    const prepared=await call(a,'Preparar',{ConexionId:c.conexionId,PlantillaId:c.plantillaId,IdCarga:i.IdCarga,RevisionEsperada:c.revisionEsperada,HuellaEsperada:c.huellaEsperada,Hoja:c.hoja,Modo:c.modo},a.token);
    check(prepared.ok&&prepared.datos?.Carga.Estado==='PREPARADA'&&prepared.datos.ValidacionCompleta&&prepared.datos.FilasObservadas===c.filasEsperadas,'DOC125_PREPARATION_FAILED');
    const e=prepared.datos.Carga,request={ConexionId:c.conexionId,IdCarga:i.IdCarga,ArchivoHash:e.ArchivoHash,ConfiguracionHash:e.ConfiguracionHash,EsquemaHash:e.EsquemaHash,ConfirmacionReemplazo:c.modo==='REEMPLAZAR'};
    const ownConfig=await select('SELECT configuracion_doc FROM imp01_carga_ejecucion WHERE id_carga=? AND ambito_ref=? AND usuario_ref=? AND conexion_ref=?',[i.IdCarga,String(s.empresaId),String(s.usuarioId),'DA:'+s.moduloId+':'+c.conexionId]);
    check(ownConfig.length===1&&require('node:crypto').createHash('sha256').update(ownConfig[0].configuracion_doc,'utf8').digest('hex')===e.ConfiguracionHash,'DOC125_CONFIGURATION_HASH_CHANGED');
    const ownPhysical=JSON.parse(ownConfig[0].configuracion_doc).IdentidadFisica;
    const locks=lockNames({...physical,fisica:ownPhysical},c.tabla,i.IdCarga);
    const lockState=async()=> (await select('SELECT IS_FREE_LOCK(?) AS carga_libre,IS_FREE_LOCK(?) AS destino_libre',[locks.id,locks.destination]))[0];
    const badCsrf=await call(a,'Confirmar',request,'malformed');check(!badCsrf.ok&&badCsrf.codigo==='SOLICITUD_NO_VERIFICADA','DOC125_CSRF_NOT_REJECTED');
    if(c.modo==='REEMPLAZAR'){const noConsent=await call(a,'Confirmar',{...request,ConfirmacionReemplazo:false},a.token);check(!noConsent.ok&&noConsent.codigo==='CONFIRMACION_REEMPLAZO_REQUERIDA','DOC125_REPLACE_CONSENT_REQUIRED');}
    let result,excludedBefore;
    if(c.escenario==='carrera'){
     check(/^[A-Za-z_][A-Za-z0-9_]{0,63}$/.test(p.CAMPO_UNICO),'DOC125_KEY_IDENTIFIER');
     const columns=await select('SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE BINARY TABLE_SCHEMA=BINARY DATABASE() AND BINARY TABLE_NAME=BINARY ? ORDER BY ORDINAL_POSITION',[c.tabla]);
     const sql='SELECT * FROM '+ident(c.tabla)+' WHERE '+ident(p.CAMPO_UNICO)+'<>? ORDER BY '+columns.map(x=>'BINARY '+ident(x.COLUMN_NAME)).join(',');
     check(Number((await select('SELECT COUNT(*) AS total FROM '+ident(c.tabla)+' WHERE '+ident(p.CAMPO_UNICO)+'=?',[c.claveExterna]))[0].total)===0,'DOC125_EXTERNAL_KEY_ALREADY_EXISTS');
     excludedBefore=await fingerprint(sql,c.claveExterna);
     const external=startExcelExternalWriter(s,c,request);await external.ready;
     const confirming=call(a,'Confirmar',request,a.token);confirming.catch(()=>{});let observed=false;
     try{
      for(let n=0;n<4&&!observed;n++){const held=await lockState();observed=Number(held.carga_libre)===0&&Number(held.destino_libre)===0;if(!observed)await new Promise(r=>setTimeout(r,100));}
      result=await confirming;
     }finally{await external.done;}
     check(observed,'DOC125_BOTH_LOCKS_NOT_OBSERVED');
     check(await fingerprint(sql,c.claveExterna)===excludedBefore&&await count(c.tabla)===beforeCount+1,'DOC125_EXTERNAL_EFFECTS_CHANGED');
    }else if(c.escenario==='agregar'){
     const results=await Promise.all([call(a,'Confirmar',request,a.token),call(b,'Confirmar',request,b.token)]);
     check(results.some(x=>x.ok)&&results.every(x=>x.ok||x.codigo==='OPERACION_EN_CURSO'),'DOC125_CONFIRM_CONTENTION_FAILED');result=results.find(x=>x.ok);
    }else result=await call(a,'Confirmar',request,a.token);
    check(result.codigo===c.codigoEsperado,'DOC125_CONFIRM_CODE_MISMATCH');
    //Acuse ignorado deliberadamente por el cliente: recuperar por ID en otra sesión.
    //No se presenta como corte físico de red o del commit del servidor.
    const recovered=await call(b,'ConsultarResultado',{ConexionId:c.conexionId,IdCarga:i.IdCarga},b.token);
    check(recovered.ok&&recovered.datos?.IdCarga===i.IdCarga,'DOC125_RESULT_QUERY_FAILED');
    const rows=await select('SELECT estado,filas_confirmadas,codigo_incidencia,(confirmado_utc IS NOT NULL) AS confirmado FROM imp01_carga_ejecucion WHERE id_carga=? AND ambito_ref=? AND usuario_ref=? AND conexion_ref=?',[i.IdCarga,String(s.empresaId),String(s.usuarioId),'DA:'+s.moduloId+':'+c.conexionId]);
    check(rows.length===1,'DOC125_RESULT_OWNER_CHANGED');
    if(c.codigoEsperado==='OK'){
     check(recovered.datos.Estado==='CONFIRMADA'&&recovered.datos.FilasConfirmadas===c.filasEsperadas&&rows[0].estado==='CONFIRMADA'&&Number(rows[0].filas_confirmadas)===c.filasEsperadas&&Number(rows[0].confirmado)===1,'DOC125_DURABLE_RESULT_FAILED');
     check(await count(c.tabla)===(c.modo==='AGREGAR'?beforeCount+c.filasEsperadas:c.filasEsperadas),'DOC125_ROW_COUNT_FAILED');
     const confirmed=await snapshotDestination(select,fingerprint,c.tabla,true);
     const replay=await call(b,'Confirmar',request,b.token);check(replay.ok&&replay.datos.FilasConfirmadas===c.filasEsperadas&&await snapshotDestination(select,fingerprint,c.tabla,true)===confirmed,'DOC125_REPLAY_CHANGED_DESTINATION');
     const altered=await call(a,'Confirmar',{...request,ConfiguracionHash:'0'.repeat(64)},a.token);check(!altered.ok&&altered.codigo==='CONFLICTO_CONFIRMACION','DOC125_REPLAY_CONTENT_NOT_REJECTED');
    }else{
     check(recovered.datos.Estado==='FALLIDA'&&rows[0].estado==='FALLIDA'&&Number(rows[0].filas_confirmadas)===0&&rows[0].codigo_incidencia==='CONFLICTO_CLAVE','DOC125_KNOWN_ROLLBACK_NOT_PROVEN');
     if(c.escenario==='rollback')check(await snapshotDestination(select,fingerprint,c.tabla,true)===before,'DOC125_REPLACE_ROLLBACK_CHANGED_DESTINATION');
    }
    const afterTemplate=await detail(a,c);check(afterTemplate.Huella===p.Huella&&afterTemplate.REVISION_CARGA===p.REVISION_CARGA,'DOC125_CONFIGURATION_CHANGED');
    const free=await lockState();check(Number(free.carga_libre)===1&&Number(free.destino_libre)===1,'DOC125_OWN_LOCK_NOT_RELEASED');
    for(const table of s.protectedTables)check(await snapshotDestination(select,fingerprint,table,true)===protectedBefore[table],'DOC125_PROTECTED_DESTINATION_CHANGED');
    completed++;console.log('DOC125_CASE_PASS scenario='+c.escenario+'; effects preserved; no cleanup');
   }
  }finally{await a.context.close();if(b)await b.context.close();}
  const restricted=await session(browser,true,'DOC125');try{const denied=await call(restricted,'AbrirCargaExcel');check(!denied.ok&&['ACCESO_DENEGADO','SESION_REQUERIDA'].includes(denied.codigo),'DOC125_RESTRICTED_NOT_DENIED');}finally{await restricted.context.close();}
  console.log('DOC125_REAL_CONFIRM_PASS cases='+completed+'; real services and UNIQUE race; protected fingerprints unchanged; client ignored acknowledgement; no installation/cleanup');
 });
});

test.describe('DOC122 autorización y plantillas',()=>{
test.skip(process.env.DOC122_E2E_AUTHORIZED!=='YES','NO_EJECUTADO: requiere autorización específica de ambiente, cuentas y plantilla descartable.');
test('@doc122-read permiso propio, lectura estable y sesión restringida',async({browser})=>{
 const s=scope();check(s.allowedModes.includes('read'),'DOC122_READ_NOT_AUTHORIZED');
 const auth=await session(browser);
 try{
  const opened=await open(auth,s);check(opened.MaxFilasDatos>0 && opened.MaxArchivoMB>0,'DOC122_LIMITS_INVALID');
  const before=await detail(auth,s);
  const list=await call(auth,'ListarPlantillas',{ConexionId:s.conexionId,Termino:before.NOMBRE_PLANTILLA,Pagina:1,TamanoPagina:25},auth.token);
  check(list.ok && list.datos.Plantillas.some(p=>p.PlantillaId===s.plantillaId),'DOC122_LIST_SCOPE_FAILED');
  const forged=await call(auth,'ConsultarPlantilla',{ConexionId:s.conexionId,PlantillaId:s.plantillaId},'malformed');
  check(!forged.ok && forged.codigo==='SOLICITUD_NO_VERIFICADA','DOC122_CSRF_NOT_REJECTED');
  const after=await detail(auth,s);check(after.Huella===before.Huella && after.REVISION_CARGA===before.REVISION_CARGA,'DOC122_READ_CHANGED_TEMPLATE');
 }finally{await auth.context.close();}
 const restricted=await session(browser,true);
 try{const denied=await call(restricted,'AbrirCargaExcel');check(!denied.ok && ['ACCESO_DENEGADO','SESION_REQUERIDA'].includes(denied.codigo),'DOC122_RESTRICTED_NOT_DENIED');}
 finally{await restricted.context.close();}
});

test('@doc122-write roundtrip y conflicto de revisión sobre plantilla descartable',async({browser})=>{
 const s=scope();check(s.allowedModes.includes('write'),'DOC122_WRITE_NOT_AUTHORIZED');
 const auth=await session(browser);
 try{
  await open(auth,s);const initial=await detail(auth,s);
  check(initial.REVISION_CARGA===s.revisionEsperada && initial.Huella===s.huellaEsperada,'DOC122_TEMPLATE_DRIFT');
  check(['CREAR','AGREGAR','REEMPLAZAR'].includes(initial.Modo),'DOC122_TEMPLATE_MODE_REQUIRED');
  const saved=await call(auth,'GuardarPlantilla',edit(s,initial),auth.token);
  check(saved.ok && saved.datos.REVISION_CARGA===initial.REVISION_CARGA+1 &&
   saved.datos.RUTA_ARCHIVO_FUENTE===initial.RUTA_ARCHIVO_FUENTE,'DOC122_ROUNDTRIP_FAILED');
  const request=edit(s,saved.datos);
  const results=await Promise.all([call(auth,'GuardarPlantilla',request,auth.token),call(auth,'GuardarPlantilla',request,auth.token)]);
  check(results.filter(x=>x.ok).length===1 && results.filter(x=>!x.ok && x.codigo==='CONFLICTO_REVISION').length===1,'DOC122_CONCURRENCY_FAILED');
  const final=await detail(auth,s);
  check(final.REVISION_CARGA===initial.REVISION_CARGA+2 &&
   final.RUTA_ARCHIVO_FUENTE===initial.RUTA_ARCHIVO_FUENTE &&
   JSON.stringify(final.Campos)===JSON.stringify(initial.Campos),'DOC122_ROUNDTRIP_CHANGED_MAPPING');
  // Dos guardados funcionales; sin SQL, creación, eliminación ni restauración automática.
 }finally{await auth.context.close();}
});
});

test.describe('DOC124 recepción y preparación',()=>{
 test.skip(process.env.DOC124_E2E_AUTHORIZED!=='YES','NO_EJECUTADO: falta autorización DOC124 del ambiente, cuentas y archivos de prueba; se conservan los destinos existentes.');
 test('@doc124-prepare servicios reales, tres modos, recuperación y dos sesiones',async({browser})=>runPreparation(browser,'matriz'));
 test('@doc124-file archivo Órdenes, recuperación y preparación con dos sesiones',async({browser})=>runPreparation(browser,'archivo'));
 test('@doc124-add agregar y duplicados sobre destino existente sin mutación',async({browser})=>runPreparation(browser,'agregar'));
 test('@doc124-replace reemplazar y duplicados sobre destino existente sin mutación',async({browser})=>runPreparation(browser,'reemplazar'));
 async function runPreparation(browser,escenario){
  const {validateScope,readFixture,snapshotDestination}=require('./support/doc124-excel-scope.cjs');
  const {queryRows,queryFingerprint}=require('../scripts/support/doc32-e2e-odbc.cjs');
  const path=require('node:path');
  const s=validateScope(JSON.parse(fs.readFileSync(required('DOC124_E2E_SCOPE_PATH'),'utf8').replace(/^\uFEFF/,'')),required('DOC124_E2E_BASE_URL'));
  check((s.escenario||'matriz')===escenario,'DOC124_TEST_SCOPE_MISMATCH');
  const selectControl=(sql,args=[])=>{console.log('DOC124_CONTROL_SELECT');return queryRows(sql,args,process.env,s.controlPrefix);};
  const fingerprintControl=(sql,marker)=>{console.log('DOC124_CONTROL_SELECT');return queryFingerprint(sql,marker,process.env,s.controlPrefix);};
  //Controles SELECT existentes: sólo se devuelven huellas al reporte, nunca filas o secretos.
  async function snapshot(c){
   return snapshotDestination(selectControl,fingerprintControl,c.tabla,c.modo!=='CREAR');
  }
  async function emit(auth,c){const r=await call(auth,'IniciarRecepcionExcel',{ConexionId:c.conexionId,PlantillaId:c.plantillaId,RevisionEsperada:c.revisionEsperada,HuellaEsperada:c.huellaEsperada},auth.token);check(r.ok&&r.datos?.IdCarga&&r.datos?.ReferenciaRecepcion,'DOC124_START_FAILED');return r.datos;}
  async function upload(auth,c,i,b){const r=await safePost(auth.context.request,new URL('generic_control/ImportacionExcelUpload.ashx',auth.baseUrl).href,{headers:{'X-Importacion-Excel-CSRF':auth.token},multipart:{idCarga:i.IdCarga,referenciaRecepcion:i.ReferenciaRecepcion,conexionId:String(c.conexionId),plantillaId:String(c.plantillaId),archivo:{name:path.basename(c.archivoPath),mimeType:'application/octet-stream',buffer:b}}},'ExcelUpload');let body;try{body=await r.json();}catch{throw Error('DOC124_UPLOAD_JSON_REQUIRED');}check(r.status()===200&&body.ok&&body.datos?.IdCarga===i.IdCarga&&body.datos.ArchivoHash===c.archivoHash,'DOC124_UPLOAD_FAILED');return body.datos;}
  const prep=(c,i)=>({ConexionId:c.conexionId,PlantillaId:c.plantillaId,IdCarga:i.IdCarga,RevisionEsperada:c.revisionEsperada,HuellaEsperada:c.huellaEsperada,Hoja:c.hoja,Modo:c.modo});
  const a=await session(browser,false,'DOC124');let b,completed=0;
  try{
   b=await session(browser,false,'DOC124');
   const sessionCookie=async auth=>(await auth.context.cookies()).find(c=>c.name==='ASP.NET_SessionId')?.value;
   const sessionA=await sessionCookie(a),sessionB=await sessionCookie(b);check(sessionA&&sessionB&&sessionA!==sessionB,'DOC124_INDEPENDENT_SESSIONS_REQUIRED');
   const opened=await open(a,s.cases[0]);await open(b,s.cases[0]);
   if(s.maxFilasDatos)check(opened.MaxFilasDatos===s.maxFilasDatos,'DOC124_SERVER_LIMIT_MISMATCH');
   for(const [index,c]of s.cases.entries()){
    const before=await snapshot(c),bytes=readFixture(c),i=await emit(a,c);
    const initial=await call(a,'ConsultarCargaExcel',{ConexionId:c.conexionId,IdCarga:i.IdCarga},a.token);check(!initial.ok&&initial.codigo==='RESULTADO_INCIERTO','DOC124_NO_FAKE_RECEIPT');
    await upload(a,c,i,bytes);
    const owned=await selectControl('SELECT ambito_ref,usuario_ref,conexion_ref,tabla_destino,archivo_bytes,HEX(archivo_hash) AS archivo_hash FROM imp01_carga_ejecucion WHERE id_carga=?',[i.IdCarga]);
    check(owned.length===1&&String(owned[0].ambito_ref)===String(s.empresaId)&&String(owned[0].usuario_ref)===String(s.usuarioId)&&owned[0].conexion_ref==='DA:'+s.moduloId+':'+c.conexionId&&owned[0].tabla_destino===c.tabla&&Number(owned[0].archivo_bytes)===bytes.length&&owned[0].archivo_hash.toLowerCase()===c.archivoHash,'DOC124_OWNER_ENVIRONMENT_MISMATCH');
    //El ID conocido se consulta en otra sesión; recuperación del acuse sin nueva identidad.
    const recovered=await call(b,'ConsultarCargaExcel',{ConexionId:c.conexionId,IdCarga:i.IdCarga},b.token);check(recovered.ok&&recovered.datos.Estado==='RECIBIDA','DOC124_RECOVERY_FAILED');
    await upload(b,c,i,bytes); //Reintento explícito idéntico de la misma carga.
    const denied=await call(a,'Preparar',prep(c,i),'malformed');check(!denied.ok&&denied.codigo==='SOLICITUD_NO_VERIFICADA','DOC124_CSRF_NOT_REJECTED');
    const results=index===s.concurrencyCase?await Promise.all([call(a,'Preparar',prep(c,i),a.token),call(b,'Preparar',prep(c,i),b.token)]):[await call(a,'Preparar',prep(c,i),a.token)];
    if(index===s.concurrencyCase)check(results.filter(r=>r.ok).length===1&&results.filter(r=>!r.ok&&r.codigo==='OPERACION_EN_CURSO').length===1,'DOC124_CONTENTION_NOT_PROVEN');
    const result=results.find(r=>r.codigo!=='OPERACION_EN_CURSO');check(result?.codigo===c.codigoEsperado&&result.datos,'DOC124_PREPARATION_RESULT_MISMATCH');
    const d=result.datos;check(d.Muestra.length<=25&&d.Incidencias.length<=25&&d.FilasObservadas===c.filasEsperadas,'DOC124_PREVIEW_BOUNDS');
    for(const code of c.incidenciasEsperadas)check(d.Incidencias.some(x=>x.Codigo===code),'DOC124_INCIDENCE_MISSING');
    check(d.Carga.Estado===(result.ok?'PREPARADA':'RECIBIDA'),'DOC124_STATE_MISMATCH');
    const consulted=await call(b,'ConsultarCargaExcel',{ConexionId:c.conexionId,IdCarga:i.IdCarga},b.token);check(consulted.ok&&consulted.datos.Estado===d.Carga.Estado&&consulted.datos.ConfiguracionHash===d.Carga.ConfiguracionHash,'DOC124_READBACK_MISMATCH');
    check(await snapshot(c)===before,'DOC124_DESTINATION_CHANGED');completed++;
   }
  }finally{await a.context.close();if(b)await b.context.close();}
  const restricted=await session(browser,true,'DOC124');try{const r=await call(restricted,'AbrirCargaExcel');check(!r.ok&&['ACCESO_DENEGADO','SESION_REQUERIDA'].includes(r.codigo),'DOC124_PERMISSION_NOT_DENIED');}finally{await restricted.context.close();}
  console.log('DOC124_PASS scenario='+escenario+' cases='+completed+'; destination SELECT fingerprints unchanged; no import/DDL/cleanup');
 }
});
