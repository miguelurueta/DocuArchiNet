'use strict';
const {test}=require('@playwright/test');
const fs=require('node:fs');
const {createWorkflowLoginPageSession}=require('./support/authenticated-workflow-session.cjs');
const {safePost}=require('./support/doc110-safe-request.cjs');

test.use({screenshot:'off',trace:'off',video:'off'});
test.describe.configure({mode:'serial',retries:0});
test.skip(process.env.DOC122_E2E_AUTHORIZED!=='YES','NO_EJECUTADO: requiere autorización específica de ambiente, cuentas y plantilla descartable.');
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
async function session(browser,restricted=false){
 const baseUrl=required('DOC122_E2E_BASE_URL').replace(/\/?$/,'/');
 const auth=await createWorkflowLoginPageSession(browser,{
  baseUrl,moduleEnvironmentVariable:'DOC122_E2E_MODULE',
  userEnvironmentVariable:restricted?'DOC122_E2E_RESTRICTED_USER':'DOC122_E2E_USER',
  passwordEnvironmentVariable:restricted?'DOC122_E2E_RESTRICTED_PASSWORD':'DOC122_E2E_PASSWORD',
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
  check(['AGREGAR','REEMPLAZAR'].includes(initial.Modo),'DOC122_EXISTING_DESTINATION_REQUIRED');
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
