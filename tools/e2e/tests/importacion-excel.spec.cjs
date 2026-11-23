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
 test.skip(process.env.DOC124_E2E_AUTHORIZED!=='YES','NO_EJECUTADO: falta autorización DOC124 del ambiente, cuentas y datos descartables.');
 test('@doc124-prepare servicios reales, tres modos, recuperación y dos sesiones',async({browser})=>{
  const {validateScope,readFixture}=require('./support/doc124-excel-scope.cjs');
  const {queryRows}=require('../scripts/support/doc32-e2e-odbc.cjs');
  const path=require('node:path'),crypto=require('node:crypto');
  const s=validateScope(JSON.parse(fs.readFileSync(required('DOC124_E2E_SCOPE_PATH'),'utf8').replace(/^\uFEFF/,'')),required('DOC124_E2E_BASE_URL'));
  //Controles SELECT existentes: sólo se devuelven huellas al reporte, nunca filas o secretos.
  async function snapshot(c){
   const query=(sql,params=[])=>queryRows(sql,params,process.env,s.controlPrefix);
   const table=await query("SELECT ENGINE,TABLE_TYPE,TABLE_COLLATION FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME=?",[c.tabla]);
   const columns=await query('SELECT COLUMN_NAME,COLUMN_TYPE,IS_NULLABLE,COLUMN_DEFAULT,CHARACTER_SET_NAME,COLLATION_NAME,EXTRA FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME=? ORDER BY ORDINAL_POSITION',[c.tabla]);
   const indexes=await query('SELECT INDEX_NAME,NON_UNIQUE,SEQ_IN_INDEX,COLUMN_NAME,SUB_PART FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME=? ORDER BY INDEX_NAME,SEQ_IN_INDEX',[c.tabla]);
   check((c.modo==='CREAR')===!table.length,'DOC124_DESTINATION_SCOPE_CHANGED');
   const rows=table.length?await query('SELECT * FROM `'+c.tabla+'`'):[];
   const canonicalRows=rows.map(r=>JSON.stringify(r)).sort();
   return crypto.createHash('sha256').update(JSON.stringify({table,columns,indexes,rows:canonicalRows})).digest('hex');
  }
  async function emit(auth,c){const r=await call(auth,'IniciarRecepcionExcel',{ConexionId:c.conexionId,PlantillaId:c.plantillaId,RevisionEsperada:c.revisionEsperada,HuellaEsperada:c.huellaEsperada},auth.token);check(r.ok&&r.datos?.IdCarga&&r.datos?.ReferenciaRecepcion,'DOC124_START_FAILED');return r.datos;}
  async function upload(auth,c,i,b){const r=await safePost(auth.context.request,new URL('generic_control/ImportacionExcelUpload.ashx',auth.baseUrl).href,{headers:{'X-Importacion-Excel-CSRF':auth.token},multipart:{idCarga:i.IdCarga,referenciaRecepcion:i.ReferenciaRecepcion,conexionId:String(c.conexionId),plantillaId:String(c.plantillaId),archivo:{name:path.basename(c.archivoPath),mimeType:'application/octet-stream',buffer:b}}},'ExcelUpload');let body;try{body=await r.json();}catch{throw Error('DOC124_UPLOAD_JSON_REQUIRED');}check(r.status()===200&&body.ok&&body.datos?.IdCarga===i.IdCarga&&body.datos.ArchivoHash===c.archivoHash,'DOC124_UPLOAD_FAILED');return body.datos;}
  const prep=(c,i)=>({ConexionId:c.conexionId,PlantillaId:c.plantillaId,IdCarga:i.IdCarga,RevisionEsperada:c.revisionEsperada,HuellaEsperada:c.huellaEsperada,Hoja:c.hoja,Modo:c.modo});
  const a=await session(browser,false,'DOC124');let b,completed=0;
  try{
   b=await session(browser,false,'DOC124');
   const sessionCookie=async auth=>(await auth.context.cookies()).find(c=>c.name==='ASP.NET_SessionId')?.value;
   const sessionA=await sessionCookie(a),sessionB=await sessionCookie(b);check(sessionA&&sessionB&&sessionA!==sessionB,'DOC124_INDEPENDENT_SESSIONS_REQUIRED');
   await open(a,s.cases[0]);await open(b,s.cases[0]);
   for(const [index,c]of s.cases.entries()){
    const before=await snapshot(c),bytes=readFixture(c),i=await emit(a,c);
    const initial=await call(a,'ConsultarCargaExcel',{ConexionId:c.conexionId,IdCarga:i.IdCarga},a.token);check(!initial.ok&&initial.codigo==='RESULTADO_INCIERTO','DOC124_NO_FAKE_RECEIPT');
    await upload(a,c,i,bytes);
    const owned=await queryRows('SELECT ambito_ref,usuario_ref,conexion_ref,tabla_destino,archivo_bytes,HEX(archivo_hash) AS archivo_hash FROM imp01_carga_ejecucion WHERE id_carga=?',[i.IdCarga],process.env,s.controlPrefix);
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
  console.log('DOC124_PASS cases='+completed+'; destination SELECT fingerprints unchanged; no import/DDL/cleanup');
 });
});
