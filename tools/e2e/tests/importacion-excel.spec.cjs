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
 let loginIssue;
 const loginCodes=require('./support/doc111-importacion-carpetas.cjs');
 const auth=await createWorkflowLoginPageSession(browser,{
  baseUrl,moduleEnvironmentVariable:prefix+'_E2E_MODULE',
  userEnvironmentVariable:prefix+(restricted?'_E2E_RESTRICTED_USER':'_E2E_USER'),
  passwordEnvironmentVariable:prefix+(restricted?'_E2E_RESTRICTED_PASSWORD':'_E2E_PASSWORD'),
  ignoreHTTPSErrors:process.env.DOC10_E2E_IGNORE_HTTPS_ERRORS==='true',
  ...(['DOC125','DOC126'].includes(prefix)?{
   onPostbackCompleted:async(_page,response)=>{const code=await loginCodes.classifyLoginPostbackResponse(response);if(code)loginIssue=code;},
   onDialogCompleted:async(_page,message)=>{const code=loginCodes.classifyLoginMessage(message);if(code)loginIssue=code;}
  }:{})
 });
 if(loginIssue){await auth.context.close();throw Error(prefix+'_'+loginIssue);}
 try{
  await auth.page.goto(new URL('Gestion/WebFormProducionDocumental.aspx',baseUrl).href,{waitUntil:'domcontentloaded'});
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
 if(!r.ok)throw Error('DOC122_OPEN_FAILED_'+(/^[A-Z_]{1,60}$/.test(r.codigo||'')?r.codigo:'UNCLASSIFIED'));
 check(r.ok && r.datos && typeof r.datos.CsrfToken==='string' && r.datos.Conexiones.some(c=>c.ConexionId===s.conexionId),'DOC122_OPEN_FAILED');
 auth.token=r.datos.CsrfToken;
 return r.datos;
}
async function detail(auth,s){
 const r=await call(auth,'ConsultarPlantilla',{ConexionId:s.conexionId,PlantillaId:s.plantillaId},auth.token);
 check(r.ok && r.datos,'DOC122_DETAIL_FAILED');return r.datos;
}
function edit(s,p){return {ConexionId:s.conexionId,RevisionEsperada:p.REVISION_CARGA,HuellaEsperada:p.Huella,Plantilla:JSON.parse(JSON.stringify(p))};}

test.describe('DOC126 interfaz Excel con servicios y controles existentes',()=>{
 test.skip(process.env.DOC126_E2E_AUTHORIZED!=='YES','NO_EJECUTADO: autorización, instalación y scope DOC126 propios requeridos. No heredar ventanas anteriores.');
 async function acceptance(browser,mode){
  test.setTimeout(900000);
  const support=require('./support/doc126-excel-scope.cjs'),{openExcelImporter}=require('./support/doc110-admin-navigation.cjs');
  const {queryRows,queryFingerprint}=require('../scripts/support/doc32-e2e-odbc.cjs');
  const {startExcelExternalWriter}=require('../scripts/run-doc110-profile-session.cjs');
  const s=support.readScope(required('DOC126_E2E_SCOPE_PATH'),required('DOC126_E2E_BASE_URL'),mode);support.verifyRuntime(s);
  const environment={...process.env,[s.controlPrefix+'_EXPECTED_TARGET_HASH']:s.environmentHash,[s.raControlPrefix+'_EXPECTED_TARGET_HASH']:s.raEnvironmentHash};
  const select=(sql,args=[])=>queryRows(sql,args,environment,s.controlPrefix),ra=(sql,args=[])=>queryRows(sql,args,environment,s.raControlPrefix);
  const fingerprint=(sql,v)=>queryFingerprint(sql,v,environment,s.controlPrefix);
  const quote=t=>{check(/^imp_02_[A-Za-z0-9_]{1,57}$/.test(t),'DOC126_TABLE_SCOPE');return '`'+t+'`';};
  const count=async t=>Number((await select('SELECT COUNT(*) AS total FROM '+quote(t)))[0].total);
  // El perfil exclusivo y el funcional con cuenta existente se acreditan por separado.
  const accounts=await ra('SELECT u.id_Remit_Dest_Int AS id,u.Login_Usuario AS login_ref,u.Estado_Usuario AS activo,p.ADMINISTRAR_CARGAS_IMPORTACION AS cargas,p.IMPORTAR_CARPETAS AS carpetas,p.ADMINISTRAR_PERFILES_IMPORTACION AS perfiles,p.ADMINISTRAR_FUENTES_IMPORTACION AS fuentes,p.ADMINISTRAR_PROVEEDORES_IMPORTACION AS proveedores FROM remit_dest_interno u INNER JOIN remit_dest_interno_perfil_produccion p ON p.remit_dest_interno_idremit_dest_interno=u.id_Remit_Dest_Int WHERE u.Empresa_Gestion_Documental_id_empresa=? AND u.id_Remit_Dest_Int IN (?,?)',[s.empresaId,s.usuarioId,s.restrictedUsuarioId]);
  support.verifyAccounts(s,accounts,required('DOC126_E2E_USER'),support.usesExistingAccount(s)?null:required('DOC126_E2E_RESTRICTED_USER'));
  const before={};for(const [i,t]of s.protectedTables.entries()){before[t]=await support.snapshotDestination(select,fingerprint,t,true);if(i===0||(i+1)%5===0||i+1===s.protectedTables.length)console.log('DOC126_PROTECTED_BASELINE '+(i+1)+'/'+s.protectedTables.length);}
  const configBefore=await fingerprint('SELECT * FROM imp01_plantillaimp WHERE ?=1 ORDER BY ID_PLANTILLAIMP',1);
  const fieldsBefore=await fingerprint('SELECT * FROM imp01_campos_plantilla WHERE ?=1 ORDER BY ID_CAMPOS_PLANTILLA',1);
  const physical=(await select('SELECT @@hostname AS servidor,@@port AS puerto,DATABASE() AS base_ref,@@lower_case_table_names AS minusculas'))[0];
  if(['write','functional','resilience'].includes(mode))for(const item of s.cases)support.readFixture(item);
  if(mode==='assistant')support.readFixture(s.assistant);
  if(!support.usesExistingAccount(s)){
   const denied=await session(browser,true,'DOC126');
   try{check(await denied.page.locator('[data-doc126=abrir]').count()===0,'DOC126_DENIED_MENU_VISIBLE');const r=await call(denied,'AbrirCargaExcel');check(!r.ok&&r.codigo==='ACCESO_DENEGADO','DOC126_DENIED_SERVICE_ALLOWED');}finally{await denied.context.close();}
  }
  const a=await session(browser,false,'DOC126');let completed=0,createdTemplate=0,templateWrites=0,assistantConfirm=null,assistantConfirmCalls=0;
  try{
   let confirmRequests=0;a.page.on('request',r=>{if(r.method()==='POST'&&r.url().endsWith('/Confirmar'))confirmRequests++;});
   if(!support.usesExistingAccount(s))check(await a.page.locator('#doc110-perfiles,#doc111-importacion').count()===0,'DOC126_OTHER_IMPORT_PERMISSIONS_PRESENT');
   let unexpectedMutation=false;
   const readOps=['AbrirCargaExcel','ListarPlantillas','ConsultarPlantilla','ConsultarResultado','ConsultarCargaExcel'];
   await a.page.route('**/WebServiceImportacionExcel.asmx/*',route=>{
    const operation=new URL(route.request().url()).pathname.split('/').pop();
    if(operation==='GuardarPlantilla'&&mode==='assistant'){
     let request;try{request=route.request().postDataJSON().solicitud;}catch{}
     if(templateWrites!==0||!support.templateRequestAllowed(s,request)){unexpectedMutation=true;return route.abort('blockedbyclient');}
     templateWrites++;return route.continue();
    }
    if(operation==='Confirmar'&&mode==='assistant'){
     let request;try{request=route.request().postDataJSON().solicitud;}catch{}
     if(!assistantConfirm||!request||Object.keys(request).length!==Object.keys(assistantConfirm).length||!Object.keys(assistantConfirm).every(k=>request[k]===assistantConfirm[k])||assistantConfirmCalls!==0){unexpectedMutation=true;return route.abort('blockedbyclient');}
     assistantConfirmCalls++;
    }
    if((mode==='read'&&!readOps.includes(operation))||operation==='GuardarPlantilla'){unexpectedMutation=true;return route.abort('blockedbyclient');}
    return route.continue();
   });
   // Comprobar también los assets servidos; no basta el hash del directorio local.
   for(const asset of support.assetPaths){const r=await a.context.request.get(new URL(asset,a.baseUrl).href);check(r.status()===200&&require('node:crypto').createHash('sha256').update(await r.body()).digest('hex')===s.assets[asset],'DOC126_HTTP_ASSET_CHANGED');}
   const response=(method)=>a.page.waitForResponse(r=>r.url().endsWith('/'+method)&&r.request().method()==='POST',{timeout:30000});
   async function clickResponse(key,method){const pending=response(method);await a.page.locator('[data-doc126='+key+']').click();const r=await pending;await a.page.waitForFunction(()=>document.querySelector('#doc126-excel [role=dialog]').getAttribute('aria-busy')==='false');return r;}
   async function choose(item){
    await openExcelImporter(a.page);
    await a.page.locator('[data-doc126=tab-carga]').click();
    const c=a.page.locator('[data-doc126=conexion]');
    if(await c.inputValue()!==String(item.conexionId)){const pending=response('ListarPlantillas');await c.selectOption(String(item.conexionId));await pending;}
    await a.page.locator('[data-doc126=buscar]').fill(item.tabla||'');await clickResponse('buscar-plantillas','ListarPlantillas');
    const pending=response('ConsultarPlantilla');await a.page.locator('[data-doc126=plantilla]').selectOption(String(item.plantillaId));const r=(await(await pending).json()).d;
    await a.page.waitForFunction(()=>document.querySelector('#doc126-excel [role=dialog]').getAttribute('aria-busy')==='false');
    check(r.ok&&r.datos.ID_PLANTILLAIMP===item.plantillaId,'DOC126_UI_TEMPLATE_REQUIRED');
    if(['write','functional','resilience'].includes(mode))check(r.datos.Huella===item.huellaEsperada&&r.datos.REVISION_CARGA===item.revisionEsperada&&r.datos.NOMBRE_PLANTILLA===item.tabla&&r.datos.Modo===item.modo,'DOC126_TEMPLATE_CHANGED');
    return r.datos;
   }
   if(mode==='read'){
    await choose(s.selection);check(await a.page.locator('[data-doc126=destino]').isVisible(),'DOC126_UI_DETAIL_NOT_VISIBLE');
    await a.page.keyboard.press('Escape');check(!await a.page.locator('#doc126-excel [role=dialog]').isVisible(),'DOC126_ESCAPE_FAILED');
    check(!unexpectedMutation,'DOC126_READ_MUTATION_ATTEMPTED');console.log('DOC126_REAL_UI_READ_PASS; exclusive/restricted accounts, existing services and unchanged fingerprints');
   }else if(mode==='assistant'){
    check(process.env.DOC126_ASSISTANT_E2E_AUTHORIZED==='YES','DOC126_ASSISTANT_AUTHORIZATION_REQUIRED');
    const item=s.assistant;support.readFixture(item);
    check((await select('SELECT TABLE_NAME FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME=?',[item.tabla])).length===0,'DOC126_ASSISTANT_TABLE_EXISTS');
    check((await select('SELECT ID_PLANTILLAIMP FROM imp01_plantillaimp WHERE NOMBRE_PLANTILLA=?',[item.tabla])).length===0,'DOC126_ASSISTANT_TEMPLATE_EXISTS');
    await openExcelImporter(a.page);
    if(await a.page.locator('[data-doc126=conexion]').inputValue()!==String(item.conexionId)){const pending=response('ListarPlantillas');await a.page.locator('[data-doc126=conexion]').selectOption(String(item.conexionId));await pending;}
    await a.page.locator('[data-doc126=crear-desde-excel]').click();
    await a.page.locator('[data-doc126=analisis-archivo]').setInputFiles(item.archivoPath);
    const analysis=(await(await clickResponse('analizar','ImportacionExcelAnalisis.ashx')).json());
    check(analysis.ok&&analysis.datos.ArchivoHash===item.archivoHash&&analysis.datos.Hoja===item.hoja&&analysis.datos.FilasObservadas===1000&&analysis.datos.Campos.length===2&&analysis.datos.Campos.every((c,i)=>['CAMPO_FUENTE','CAMPO_DESTINO','TIPO_CAMPO'].every(k=>c[k]===item.campos[i][k])),'DOC126_ASSISTANT_PROPOSAL_CHANGED');
    check(await a.page.locator('[data-doc126=analisis-preview]').isVisible(),'DOC126_ASSISTANT_PREVIEW_MISSING');
    await a.page.locator('[data-doc126=tabla]').fill(item.tabla);await a.page.locator('[data-doc126=analisis-clave]').selectOption(item.campoUnico);
    const saved=(await(await clickResponse('guardar','GuardarPlantilla')).json()).d;
    check(saved.ok&&Number.isSafeInteger(saved.datos.ID_PLANTILLAIMP)&&saved.datos.ID_PLANTILLAIMP>0&&saved.datos.NOMBRE_PLANTILLA===item.tabla&&saved.datos.Campos.length===2&&templateWrites===1,'DOC126_ASSISTANT_SAVE_FAILED');
    createdTemplate=saved.datos.ID_PLANTILLAIMP;
    const ownTemplate=await select('SELECT ID_PLANTILLAIMP,IMP01_XMLCONEXION_ID_XMLCONEXION,CAMPO_UNICO,REVISION_CARGA FROM imp01_plantillaimp WHERE ID_PLANTILLAIMP=?',[createdTemplate]);
    check(ownTemplate.length===1&&Number(ownTemplate[0].IMP01_XMLCONEXION_ID_XMLCONEXION)===item.conexionId&&ownTemplate[0].CAMPO_UNICO===item.campoUnico&&Number(ownTemplate[0].REVISION_CARGA)===1,'DOC126_ASSISTANT_TEMPLATE_OWNER_CHANGED');
    check(Number((await select('SELECT COUNT(*) AS total FROM imp01_campos_plantilla WHERE IMP01_PLANTILLAIMP_ID_PLANTILLAIMP=?',[createdTemplate]))[0].total)===2,'DOC126_ASSISTANT_FIELD_COUNT_CHANGED');
    check(await a.page.locator('[data-doc126=archivo]').inputValue()===''&&await a.page.locator('[data-doc126=archivo-conservado]').isVisible(),'DOC126_ASSISTANT_FILE_NOT_RETAINED');
    check((await select('SELECT TABLE_NAME FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME=?',[item.tabla])).length===0,'DOC126_ASSISTANT_SAVE_IMPORTED');
    const uploaded=await(await clickResponse('recibir','ImportacionExcelUpload.ashx')).json();
    check(uploaded.ok&&uploaded.datos.ArchivoHash===item.archivoHash&&uploaded.datos.PlantillaId===createdTemplate,'DOC126_ASSISTANT_UPLOAD_CHANGED');
    const id=uploaded.datos.IdCarga;
    const prepared=(await(await clickResponse('preparar','Preparar')).json()).d;
    check(prepared.ok&&prepared.datos.ValidacionCompleta&&prepared.datos.LecturaCompleta&&prepared.datos.FilasObservadas===1000,'DOC126_ASSISTANT_PREPARE_FAILED');
    const records=await select('SELECT ambito_ref,usuario_ref,conexion_ref,id_plantilla,tabla_destino,estado,LOWER(HEX(archivo_hash)) AS archivo_hash,LOWER(HEX(configuracion_hash)) AS configuracion_hash,LOWER(HEX(esquema_hash)) AS esquema_hash FROM imp01_carga_ejecucion WHERE id_carga=?',[id]);
    check(records.length===1&&String(records[0].ambito_ref)===String(s.empresaId)&&String(records[0].usuario_ref)===String(s.usuarioId)&&records[0].conexion_ref==='DA:'+s.moduloId+':'+item.conexionId&&Number(records[0].id_plantilla)===createdTemplate&&records[0].tabla_destino===item.tabla&&records[0].estado==='PREPARADA'&&records[0].archivo_hash===item.archivoHash,'DOC126_ASSISTANT_LOAD_OWNER_CHANGED');
    assistantConfirm={ConexionId:item.conexionId,IdCarga:id,ArchivoHash:item.archivoHash,ConfiguracionHash:records[0].configuracion_hash,EsquemaHash:records[0].esquema_hash,ConfirmacionReemplazo:false};
    const pending=response('Confirmar');await a.page.locator('[data-doc126=confirmar]').dblclick();const confirmed=(await(await pending).json()).d;
    check(confirmed.ok&&confirmed.datos.Estado==='CONFIRMADA'&&confirmed.datos.FilasConfirmadas===1000&&confirmRequests===1,'DOC126_ASSISTANT_CONFIRM_FAILED');
    await a.page.waitForFunction(()=>document.querySelector('#doc126-excel [role=dialog]').getAttribute('aria-busy')==='false');
    await a.page.locator('[data-doc126=tab-consulta]').click();await a.page.locator('[data-doc126=id-carga]').fill(id);
    const recovered=(await(await clickResponse('consultar','ConsultarResultado')).json()).d;
    check(recovered.ok&&recovered.datos.Estado==='CONFIRMADA'&&recovered.datos.FilasConfirmadas===1000&&await count(item.tabla)===1000,'DOC126_ASSISTANT_RECOVERY_FAILED');
    completed=1;
   }else for(const item of s.cases){
    if(await a.page.locator('#doc126-excel [role=dialog]').isVisible())await a.page.locator('[data-doc126=cerrar]').click();
    const previous=await support.snapshotDestination(select,fingerprint,item.tabla,item.modo!=='CREAR');
    check((item.modo==='CREAR'?0:await count(item.tabla))===item.filasDestinoEsperadas,'DOC126_DESTINATION_BASELINE_CHANGED');
    await choose(item);
    await a.page.locator('[data-doc126=archivo]').setInputFiles(item.archivoPath);
    const uploaded=(await(await clickResponse('recibir','ImportacionExcelUpload.ashx')).json());check(uploaded.ok&&uploaded.datos.ArchivoHash===item.archivoHash,'DOC126_UI_UPLOAD_FAILED');
    const id=uploaded.datos.IdCarga;await a.page.locator('[data-doc126=hoja]').selectOption(item.hoja);
    const prepared=(await(await clickResponse('preparar','Preparar')).json()).d;
    check(prepared.ok&&prepared.datos.Carga.Estado==='PREPARADA'&&prepared.datos.FilasObservadas===item.filasEsperadas,'DOC126_UI_PREPARE_FAILED');
    const records=await select('SELECT ambito_ref,usuario_ref,conexion_ref,tabla_destino,estado,LOWER(HEX(archivo_hash)) AS archivo_hash,LOWER(HEX(configuracion_hash)) AS configuracion_hash,LOWER(HEX(esquema_hash)) AS esquema_hash,configuracion_doc FROM imp01_carga_ejecucion WHERE id_carga=?',[id]);
    check(records.length===1&&String(records[0].ambito_ref)===String(s.empresaId)&&String(records[0].usuario_ref)===String(s.usuarioId)&&records[0].conexion_ref==='DA:'+s.moduloId+':'+item.conexionId&&records[0].tabla_destino===item.tabla&&records[0].archivo_hash===item.archivoHash&&records[0].estado==='PREPARADA','DOC126_LOAD_OWNER_CHANGED');
    const req={ConexionId:item.conexionId,IdCarga:id,ArchivoHash:records[0].archivo_hash,ConfiguracionHash:records[0].configuracion_hash,EsquemaHash:records[0].esquema_hash,ConfirmacionReemplazo:item.modo==='REEMPLAZAR'};
    const lockNames=support.lockNames({...physical,fisica:JSON.parse(records[0].configuracion_doc).IdentidadFisica},item.tabla,id);
    if(item.modo==='REEMPLAZAR'){check(await a.page.locator('[data-doc126=confirmar]').isDisabled(),'DOC126_REPLACE_CONSENT_BYPASSED');await a.page.locator('[data-doc126=consentimiento]').check();}
    let actor=null;
    if(item.escenario==='carrera'){actor=startExcelExternalWriter(s,item,req,environment);await actor.ready;}
    let lostAcknowledgement=false;
    if(item.escenario==='crear')await a.page.route('**/WebServiceImportacionExcel.asmx/Confirmar',async route=>{
     // El servidor real respondió; sólo se ignora el acuse en el cliente. No es un corte de COMMIT.
     const actual=await route.fetch();check(actual.status()===200,'DOC126_ACK_FETCH_FAILED');lostAcknowledgement=true;await route.abort('failed');
    });
    const pending=item.escenario==='crear'?null:response('Confirmar');
    const attemptsBefore=confirmRequests;await a.page.locator('[data-doc126=confirmar]').dblclick();
    if(actor&&mode!=='resilience'){let observed=false;for(let n=0;n<6&&!observed;n++){const locks=(await select('SELECT IS_USED_LOCK(?) AS carga,IS_USED_LOCK(?) AS destino',[lockNames.id,lockNames.destination]))[0];observed=locks.carga!==null&&locks.destino!==null&&locks.carga!==locks.destino;if(!observed)await new Promise(r=>setTimeout(r,250));}check(observed,'DOC126_ACTIVE_LOCKS_NOT_OBSERVED');}
    if(pending){const r=(await(await pending).json()).d;check(r.codigo===item.codigoEsperado,'DOC126_UI_CONFIRM_CODE_MISMATCH');}
    else {await a.page.waitForFunction(()=>document.querySelector('[data-doc126=estado]').getAttribute('data-state')==='RESULTADO_INCIERTO',null,{timeout:45000});check(lostAcknowledgement,'DOC126_ACK_NOT_IGNORED');await a.page.unroute('**/WebServiceImportacionExcel.asmx/Confirmar');}
    check(confirmRequests===attemptsBefore+1,'DOC126_DOUBLE_CONFIRM_REQUEST');
    if(actor)await actor.done;
    await a.page.locator('[data-doc126=tab-consulta]').click();await a.page.locator('[data-doc126=id-carga]').fill(id);
    const recovered=(await(await clickResponse('consultar','ConsultarResultado')).json()).d;
    const failure=['rollback','carrera'].includes(item.escenario);check(recovered.ok&&recovered.datos.Estado===(failure?'FALLIDA':'CONFIRMADA')&&recovered.datos.FilasConfirmadas===(failure?0:item.filasEsperadas),'DOC126_UI_RECOVERY_FAILED');
    const actualCount=await count(item.tabla),expected=failure?item.filasDestinoEsperadas+(actor?1:0):item.modo==='AGREGAR'?item.filasDestinoEsperadas+item.filasEsperadas:item.filasEsperadas;
    check(actualCount===expected,'DOC126_UI_ATOMIC_COUNT_FAILED');
    if(item.escenario==='rollback')check(await support.snapshotDestination(select,fingerprint,item.tabla,true)===previous,'DOC126_ROLLBACK_CONTENT_CHANGED');
    if(actor){
     check(Number((await select('SELECT COUNT(*) AS total FROM '+quote(item.tabla)+' WHERE CLAVE=?',[item.claveExterna]))[0].total)===1,'DOC126_EXTERNAL_ROW_NOT_RETAINED');
     if(mode==='resilience'){
      const withoutExternal=(sql)=>{check(sql.includes(' WHERE ? IS NOT NULL '),'DOC126_CONTROL_QUERY_CHANGED');return fingerprint(sql.replace(' WHERE ? IS NOT NULL ',' WHERE `CLAVE`<>? '),item.claveExterna);};
      check(await support.snapshotDestination(select,withoutExternal,item.tabla,true)===previous,'DOC126_PREVIOUS_ROWS_CHANGED');
     }
    }
    const free=(await select('SELECT IS_FREE_LOCK(?) AS carga,IS_FREE_LOCK(?) AS destino',[lockNames.id,lockNames.destination]))[0];check(Number(free.carga)===1&&Number(free.destino)===1,'DOC126_LOCKS_NOT_RELEASED');
    completed++;console.log('DOC126_UI_CASE_PASS scenario='+item.escenario+'; effects preserved');
   }
   check(!unexpectedMutation,'DOC126_UI_UNAUTHORIZED_MUTATION');
   for(const [i,t]of s.protectedTables.entries()){check(await support.snapshotDestination(select,fingerprint,t,true)===before[t],'DOC126_PROTECTED_CONTENT_CHANGED');if(i===0||(i+1)%5===0||i+1===s.protectedTables.length)console.log('DOC126_PROTECTED_VERIFIED '+(i+1)+'/'+s.protectedTables.length);}
   const headerControl=mode==='assistant'?'SELECT * FROM imp01_plantillaimp WHERE ID_PLANTILLAIMP<>? ORDER BY ID_PLANTILLAIMP':'SELECT * FROM imp01_plantillaimp WHERE ?=1 ORDER BY ID_PLANTILLAIMP';
   const fieldsControl=mode==='assistant'?'SELECT * FROM imp01_campos_plantilla WHERE IMP01_PLANTILLAIMP_ID_PLANTILLAIMP<>? ORDER BY ID_CAMPOS_PLANTILLA':'SELECT * FROM imp01_campos_plantilla WHERE ?=1 ORDER BY ID_CAMPOS_PLANTILLA';
   check(await fingerprint(headerControl,mode==='assistant'?createdTemplate:1)===configBefore&&await fingerprint(fieldsControl,mode==='assistant'?createdTemplate:1)===fieldsBefore,'DOC126_CONFIGURATION_CHANGED');
   if(mode==='assistant'){check(completed===1&&createdTemplate>0&&templateWrites===1,'DOC126_ASSISTANT_INCOMPLETE');console.log('DOC126_REAL_UI_ASSISTANT_PASS; one new template/two fields, 1000 rows, explicit receipt/validation/confirmation and preserved previous fingerprints');}
   if(mode==='write')check(completed===5,'DOC126_MATRIX_INCOMPLETE');
   if(mode==='write')console.log('DOC126_REAL_UI_WRITE_PASS cases=5; real UI/services, known rollback and observed external writer; no installation/cleanup');
   if(mode==='functional'){check(completed===2,'DOC126_FUNCTIONAL_INCOMPLETE');console.log('DOC126_REAL_UI_FUNCTIONAL_PASS cases=2; authorized existing account, add/replace with preserved fingerprints; exclusive permission not tested');}
   if(mode==='resilience'){check(completed===2,'DOC126_RESILIENCE_INCOMPLETE');console.log('DOC126_REAL_UI_RESILIENCE_PASS cases=2; rollback unchanged, native active locks/import wait and one external commit observed, previous rows and protected fingerprints preserved');}
  }finally{await a.context.close();for(const key of Object.keys(environment))if(/MYSQL_(USER|PASSWORD)$/.test(key))delete environment[key];}
 }
 test('@doc126-ui-read acceso exclusivo, navegación y consultas sin mutación',async({browser})=>{test.skip(process.env.DOC126_E2E_MODE!=='read');await acceptance(browser,'read');});
 test('@doc126-ui-write tres modos, rollback y carrera por controles reales',async({browser})=>{test.skip(process.env.DOC126_E2E_MODE!=='write');await acceptance(browser,'write');});
 test('@doc126-ui-assistant propuesta, alta de plantilla y carga por controles reales',async({browser})=>{test.skip(process.env.DOC126_E2E_MODE!=='assistant'||process.env.DOC126_ASSISTANT_E2E_AUTHORIZED!=='YES');await acceptance(browser,'assistant');});
 test('@doc126-ui-functional agregar y reemplazar con cuenta existente autorizada',async({browser})=>{test.skip(process.env.DOC126_E2E_MODE!=='functional'||process.env.DOC126_FUNCTIONAL_E2E_AUTHORIZED!=='YES');await acceptance(browser,'functional');});
 test('@doc126-ui-resilience rollback y escritor concurrente con cuenta existente',async({browser})=>{test.skip(process.env.DOC126_E2E_MODE!=='resilience'||process.env.DOC126_RESILIENCE_E2E_AUTHORIZED!=='YES');await acceptance(browser,'resilience');});
});

test.describe('DOC125 confirmación durable',()=>{
 test.skip(process.env.DOC125_E2E_AUTHORIZED!=='YES','NO_EJECUTADO: falta autorización escritora DOC125, instalación manual verificada y destinos descartables; DOC124 no autoriza estos efectos.');
 test('@doc125-confirm modos, rollback, resultado perdido y escritor externo',async({browser})=>{
  // Cinco casos y controles SELECT PowerShell medidos: presupuesto de prueba15min.
  // Cada HTTP conserva30s y cero reintentos; no amplía el presupuesto de importación.
  test.setTimeout(900000);
  const path=require('node:path'),{readScope,readFixture,snapshotDestination,lockNames,resumeRequest}=require('./support/doc125-excel-scope.cjs');
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
    if(c.reanudarConfirmada||c.reanudarFallida){
     const resume=c.reanudarConfirmada||c.reanudarFallida,confirmed=!!c.reanudarConfirmada;
     readFixture(c);
     const owned=await select('SELECT id_carga,id_plantilla,ambito_ref,usuario_ref,conexion_ref,tabla_destino,estado,modo,filas_previstas,filas_confirmadas,codigo_incidencia,(confirmado_utc IS NOT NULL) AS confirmado,LOWER(HEX(archivo_hash)) AS archivo_hash,LOWER(HEX(configuracion_hash)) AS configuracion_hash,LOWER(HEX(esquema_hash)) AS esquema_hash FROM imp01_carga_ejecucion WHERE id_carga=? AND ambito_ref=? AND usuario_ref=? AND conexion_ref=?',[resume.idCarga,String(s.empresaId),String(s.usuarioId),'DA:'+s.moduloId+':'+c.conexionId]);
     check(owned.length===1,'DOC125_RESUME_NOT_FOUND');const request=resumeRequest(s,c,owned[0]);
     const before=await snapshotDestination(select,fingerprint,c.tabla,true);
     check(before===resume.huellaDestino&&await count(c.tabla)===(confirmed?(c.modo==='AGREGAR'?c.filasDestinoEsperadas+c.filasEsperadas:c.filasEsperadas):c.filasDestinoEsperadas),'DOC125_RESUME_DESTINATION_CHANGED');
     const csrf=await call(a,'Confirmar',request,'malformed');check(!csrf.ok&&csrf.codigo==='SOLICITUD_NO_VERIFICADA','DOC125_CSRF_NOT_REJECTED');
     if(c.modo==='REEMPLAZAR'){const denied=await call(a,'Confirmar',{...request,ConfirmacionReemplazo:false},a.token);check(!denied.ok&&denied.codigo==='CONFIRMACION_REEMPLAZO_REQUERIDA','DOC125_REPLACE_CONSENT_NOT_REQUIRED');}
     const queried=await call(a,'ConsultarResultado',{ConexionId:c.conexionId,IdCarga:request.IdCarga},a.token);
     const replay=await call(b,confirmed?'Confirmar':'ConsultarResultado',confirmed?request:{ConexionId:c.conexionId,IdCarga:request.IdCarga},b.token);
     check(queried.ok&&queried.datos?.Estado===(confirmed?'CONFIRMADA':'FALLIDA')&&queried.datos.FilasConfirmadas===(confirmed?c.filasEsperadas:0)&&replay.ok&&replay.datos?.IdCarga===request.IdCarga&&replay.datos.FilasConfirmadas===(confirmed?c.filasEsperadas:0),'DOC125_RESUME_DURABLE_RESULT_FAILED');
     const changed=await call(a,'Confirmar',{...request,ConfiguracionHash:'0'.repeat(64)},a.token);check(!changed.ok&&changed.codigo==='CONFLICTO_CONFIRMACION','DOC125_REPLAY_CONTENT_NOT_REJECTED');
     check(await snapshotDestination(select,fingerprint,c.tabla,true)===before,'DOC125_RESUME_REPLAY_CHANGED_DESTINATION');
     const saved=await select('SELECT configuracion_doc FROM imp01_carga_ejecucion WHERE id_carga=? AND ambito_ref=? AND usuario_ref=? AND conexion_ref=?',[request.IdCarga,String(s.empresaId),String(s.usuarioId),'DA:'+s.moduloId+':'+c.conexionId]);
     check(saved.length===1&&require('node:crypto').createHash('sha256').update(saved[0].configuracion_doc,'utf8').digest('hex')===request.ConfiguracionHash,'DOC125_RESUME_CONFIGURATION_CHANGED');
     const names=lockNames({...physical,fisica:JSON.parse(saved[0].configuracion_doc).IdentidadFisica},c.tabla,request.IdCarga);
     const free=(await select('SELECT IS_FREE_LOCK(?) AS carga_libre,IS_FREE_LOCK(?) AS destino_libre',[names.id,names.destination]))[0];
     check(Number(free.carga_libre)===1&&Number(free.destino_libre)===1,'DOC125_OWN_LOCK_NOT_RELEASED');
     for(const table of s.protectedTables)check(await snapshotDestination(select,fingerprint,table,true)===protectedBefore[table],'DOC125_PROTECTED_DESTINATION_CHANGED');
     completed++;console.log('DOC125_CASE_PASS scenario='+c.escenario+'; durable result recovered by ID; no repeated effects');continue;
    }
    const bytes=readFixture(c),before=await snapshotDestination(select,fingerprint,c.tabla,c.modo!=='CREAR');
    const beforeCount=c.modo==='CREAR'?0:await count(c.tabla);
    check(beforeCount===c.filasDestinoEsperadas,'DOC125_DESTINATION_BASELINE_CHANGED');
    let i,e;
    if(c.continuarPreparada){
     const frozen=c.continuarPreparada;
     const owned=await select('SELECT id_carga,id_plantilla,ambito_ref,usuario_ref,conexion_ref,tabla_destino,estado,modo,filas_previstas,filas_confirmadas,codigo_incidencia,(confirmado_utc IS NOT NULL) AS confirmado,LOWER(HEX(archivo_hash)) AS archivo_hash,LOWER(HEX(configuracion_hash)) AS configuracion_hash,LOWER(HEX(esquema_hash)) AS esquema_hash FROM imp01_carga_ejecucion WHERE id_carga=? AND ambito_ref=? AND usuario_ref=? AND conexion_ref=?',[frozen.idCarga,String(s.empresaId),String(s.usuarioId),'DA:'+s.moduloId+':'+c.conexionId]);
     check(owned.length===1&&before===frozen.huellaDestino,'DOC125_PENDING_RESUME_CHANGED');
     const request=resumeRequest(s,c,owned[0]);i={IdCarga:request.IdCarga};e={IdCarga:request.IdCarga,ArchivoHash:request.ArchivoHash,ConfiguracionHash:request.ConfiguracionHash,EsquemaHash:request.EsquemaHash};
    }else{
    const started=await call(a,'IniciarRecepcionExcel',{ConexionId:c.conexionId,PlantillaId:c.plantillaId,RevisionEsperada:c.revisionEsperada,HuellaEsperada:c.huellaEsperada},a.token);
    check(started.ok&&started.datos?.IdCarga&&started.datos?.ReferenciaRecepcion,'DOC125_START_FAILED');
    i=started.datos;
    const response=await safePost(a.context.request,new URL('generic_control/ImportacionExcelUpload.ashx',a.baseUrl).href,{headers:{'X-Importacion-Excel-CSRF':a.token},multipart:{idCarga:i.IdCarga,referenciaRecepcion:i.ReferenciaRecepcion,conexionId:String(c.conexionId),plantillaId:String(c.plantillaId),archivo:{name:path.basename(c.archivoPath),mimeType:'application/octet-stream',buffer:bytes}}},'ExcelUpload');
    let upload;try{upload=await response.json();}catch{throw Error('DOC125_UPLOAD_FAILED');}check(response.status()===200&&upload.ok&&upload.datos?.ArchivoHash===c.archivoHash,'DOC125_UPLOAD_FAILED');
    const owned=await select('SELECT ambito_ref,usuario_ref,conexion_ref,tabla_destino,HEX(archivo_hash) AS archivo_hash FROM imp01_carga_ejecucion WHERE id_carga=?',[i.IdCarga]);
    check(owned.length===1&&String(owned[0].ambito_ref)===String(s.empresaId)&&String(owned[0].usuario_ref)===String(s.usuarioId)&&owned[0].conexion_ref==='DA:'+s.moduloId+':'+c.conexionId&&owned[0].tabla_destino===c.tabla&&owned[0].archivo_hash.toLowerCase()===c.archivoHash,'DOC125_OWNER_ENVIRONMENT_CHANGED');
    const prepared=await call(a,'Preparar',{ConexionId:c.conexionId,PlantillaId:c.plantillaId,IdCarga:i.IdCarga,RevisionEsperada:c.revisionEsperada,HuellaEsperada:c.huellaEsperada,Hoja:c.hoja,Modo:c.modo},a.token);
    check(prepared.ok&&prepared.datos?.Carga.Estado==='PREPARADA'&&prepared.datos.ValidacionCompleta&&prepared.datos.FilasObservadas===c.filasEsperadas,'DOC125_PREPARATION_FAILED');
    e=prepared.datos.Carga;
    }
    const request={ConexionId:c.conexionId,IdCarga:i.IdCarga,ArchivoHash:e.ArchivoHash,ConfiguracionHash:e.ConfiguracionHash,EsquemaHash:e.EsquemaHash,ConfirmacionReemplazo:c.modo==='REEMPLAZAR'};
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
    check(result.codigo===c.codigoEsperado,'DOC125_CONFIRM_CODE_MISMATCH_'+(/^[A-Z_]{1,60}$/.test(result.codigo||'')?result.codigo:'UNCLASSIFIED'));
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
  console.log('DOC125_REAL_CONFIRM_PASS cases='+completed+'; real services, retained confirmations and UNIQUE race; protected fingerprints unchanged; client ignored acknowledgement; no installation/cleanup');
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
