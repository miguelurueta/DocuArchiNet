'use strict';
const crypto=require('node:crypto');
const {createSelectControls}=require('./doc111-importacion-carpetas.cjs');
const {selectLevel}=require('./doc113-ventana-importacion.cjs');
const BASE_URL='https://localhost/GestionDocumental-Docuarchi.net/';
const LOGIN_STATUS_PATH='webservice/webserviceloginsegundofactor.asmx/obtenerestado';
const LOGIN_LANDING_PATH='defaul/webforminiciodocuarchigestion.aspx';
const STATIC_DEPENDENCIES=Object.freeze([
  Object.freeze({code:'POPPER_1_11_0',type:'script',url:'https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.11.0/umd/popper.min.js'}),
  Object.freeze({code:'BOOTSTRAP_TABLE_1_23_2_CSS',type:'stylesheet',url:'https://cdn.jsdelivr.net/npm/bootstrap-table@1.23.2/dist/bootstrap-table.min.css'}),
  Object.freeze({code:'BOOTSTRAP_TABLE_1_23_2_JS',type:'script',url:'https://cdn.jsdelivr.net/npm/bootstrap-table@1.23.2/dist/bootstrap-table.min.js'}),
  Object.freeze({code:'TABLE_EXPORT_1_29_0',type:'script',url:'https://cdn.jsdelivr.net/npm/tableexport.jquery.plugin@1.29.0/tableExport.min.js'}),
  Object.freeze({code:'TABLE_EXPORT_1_29_0_JSPDF',type:'script',url:'https://cdn.jsdelivr.net/npm/tableexport.jquery.plugin@1.29.0/libs/jsPDF/jspdf.umd.min.js'}),
  Object.freeze({code:'BOOTSTRAP_TABLE_1_23_2_EXPORT',type:'script',url:'https://cdn.jsdelivr.net/npm/bootstrap-table@1.23.2/dist/extensions/export/bootstrap-table-export.min.js'})
]);
const LIMITS=Object.freeze({'SEG-01':Object.freeze({http:600,select:80,sessions:2}),'SEG-02':Object.freeze({http:600,select:60,sessions:2})});
const SESSION_IDLE_LIMIT_MS=8*60*1000;
const READS=Object.freeze(['AbrirImportacion','ListarPerfilesImportacion','ConsultarLote','ConsultarRecursosLote','ExportarPaginaLote','ConsultarConfirmacionLote']);
const check=(ok,code)=>{if(!ok)throw Error('DOC119_'+code);};
const id=value=>typeof value==='string'&&/^[1-9][0-9]{0,9}$/.test(value)&&BigInt(value)<=4294967295n;
const hash=value=>crypto.createHash('sha256').update(typeof value==='string'||Buffer.isBuffer(value)?value:JSON.stringify(value)).digest('hex');
function assertSessionFresh(verifiedAt,now=Date.now()){
  check(Number.isFinite(verifiedAt)&&Number.isFinite(now)&&now>=verifiedAt&&now-verifiedAt<SESSION_IDLE_LIMIT_MS,'SESSION_IDLE_EXPIRED');
}
function previousSelection(p,runId){
  check(typeof runId==='string'&&/^[a-f0-9]{32}$/.test(runId)&&p?.runId===runId&&p.closed===true&&p.authorizedPlan===true&&p.baseUrl===BASE_URL&&p.authorizationRevision==='20261112-http600','PREVIOUS_SELECTION_INVALID');
  check(id(p.resources?.lotId)&&p.resources.recoveryLotId===null&&p.resources.alternateLevel===null,'PREVIOUS_SELECTION_INVALID');
  return {lotId:p.resources.lotId};
}
const CONNECTION_FIELDS=['baseUrl','module','ignoreHttpsErrors','timeoutMilliseconds','odbcDsn','docuarchiOdbcDsn'];
function validateConnection(p){
  check(p&&Object.keys(p).every(k=>CONNECTION_FIELDS.includes(k)),'PROFILE_INVALID');
  check(p.baseUrl===BASE_URL&&p.module==='GESTOR'&&typeof p.ignoreHttpsErrors==='boolean'&&p.timeoutMilliseconds===60000,'PROFILE_INVALID');
  check([p.odbcDsn,p.docuarchiOdbcDsn].every(v=>typeof v==='string'&&/^[A-Za-z0-9 _.-]{1,128}$/.test(v)),'DSN_INVALID');return p;
}
function validateProfile(p){
  check(p&&Object.keys(p).every(k=>['baseUrl','module','ignoreHttpsErrors','timeoutMilliseconds','odbcDsn','docuarchiOdbcDsn','lotId','recoveryLotId','alternateLevel'].includes(k)),'PROFILE_INVALID');
  validateConnection(Object.fromEntries(CONNECTION_FIELDS.map(k=>[k,p[k]])));
  check(id(p.lotId)&&(p.recoveryLotId===null||id(p.recoveryLotId))&&(p.alternateLevel===null||id(p.alternateLevel)),'RESOURCE_INVALID');
  check(!p.recoveryLotId||p.alternateLevel,'ALTERNATE_LEVEL_REQUIRED');
  return p;
}
function displayText(value){return String(value??'').replace(/[\u0000-\u001f\u007f-\u009f]/g,' ').slice(0,180);}
function chooseItem(items,choice,allowSkip=false){
  if(allowSkip&&choice==='0')return null;
  check(typeof choice==='string'&&/^[1-9][0-9]{0,2}$/.test(choice)&&Number(choice)<=items.length,'SELECTION_INVALID');
  return items[Number(choice)-1];
}
function describeImport(item,index){
  const states={RECIBIENDO:'con archivos por recibir',LISTO:'lista para procesar',EN_CURSO:'en proceso',PENDIENTE_VERIFICACION:'por verificar',DETENIDO:'detenida',COMPLETADO:'completada',PARCIAL:'resultado parcial',FALLIDO:'con fallos'};
  return (index+1)+'. '+displayText(item.createdAt)+' UTC | '+displayText(item.levelName)+' | '+(states[item.state]||'estado registrado')+' | '+item.totalFiles+' archivos, '+item.totalRoots+' carpetas, '+item.pendingFiles+' por recibir';
}
async function discoverImports(query,budget,login){
  check(typeof login==='string'&&login.length>0&&login.length<=128&&!/[\u0000-\u001f]/.test(login),'ACCOUNT_INVALID');
  budget.take('select');
  const users=await query('RA','SELECT id_Remit_Dest_Int AS usuario_ref,Empresa_Gestion_Documental_id_empresa AS empresa_ref FROM remit_dest_interno WHERE Login_Usuario=? AND Estado_Usuario=1 LIMIT 2',[login]);
  check(users.length===1&&id(String(users[0].usuario_ref))&&id(String(users[0].empresa_ref)),'ACCOUNT_AMBIGUOUS_OR_UNAVAILABLE');
  const principal={userRef:String(users[0].usuario_ref),companyRef:String(users[0].empresa_ref)};
  budget.take('select');
  const rows=await query('RA',"SELECT l.lote_id,l.nivel_ref,n.nombre_nivel AS level_name,DATE_FORMAT(l.creado_utc,'%Y-%m-%d %H:%i') AS created_at,l.estado,l.total_raices,l.total_archivos,SUM(CASE WHEN f.estado_recepcion='PENDIENTE' AND f.estado_efecto='NO_INICIADO' AND f.intento_activo_id IS NULL THEN 1 ELSE 0 END) AS pending_files FROM ra_integracion_lote l INNER JOIN ra_pro_niveles n ON n.id_nivel=l.nivel_ref INNER JOIN remit_dest_interno u ON u.id_Remit_Dest_Int=n.remit_dest_interno_id_Remit_Dest_Int LEFT JOIN ra_integracion_raiz r ON r.lote_id=l.lote_id LEFT JOIN ra_integracion_archivo f ON f.raiz_id=r.raiz_id WHERE l.solicitante_ref=? AND l.ambito_ref=? AND n.estado_nivel=1 AND u.Empresa_Gestion_Documental_id_empresa=? AND (u.id_Remit_Dest_Int=? OR EXISTS(SELECT 1 FROM ra_pro_permisos_niveles p WHERE p.ra_pro_niveles_id_nivel=n.id_nivel AND p.remit_dest_interno_id_Remit_Dest_Int=? AND p.estado_permiso=1 AND p.agregar_expediente=1 AND p.carga_archivo=1)) GROUP BY l.lote_id,l.nivel_ref,n.nombre_nivel,l.creado_utc,l.estado,l.total_raices,l.total_archivos ORDER BY l.creado_utc DESC,l.lote_id DESC LIMIT 30",[principal.userRef,principal.companyRef,principal.companyRef,principal.userRef,principal.userRef]);
  check(rows.length<=30,'IMPORT_LIST_INVALID');
  const items=rows.map(row=>{
    check(id(String(row.lote_id))&&id(String(row.nivel_ref))&&[row.total_archivos,row.total_raices,row.pending_files].every(n=>/^\d+$/.test(String(n))&&Number.isSafeInteger(Number(n))),'IMPORT_LIST_INVALID');
    return {id:String(row.lote_id),level:String(row.nivel_ref),levelName:displayText(row.level_name),createdAt:displayText(row.created_at),state:displayText(row.estado),totalFiles:Number(row.total_archivos),totalRoots:Number(row.total_raices),pendingFiles:Number(row.pending_files)};
  });return {principal,items};
}
async function discoverLevels(query,budget,principal,originalLevel){
  check(id(principal.userRef)&&id(principal.companyRef)&&id(originalLevel),'RESOURCE_INVALID');budget.take('select');
  const rows=await query('RA','SELECT n.id_nivel,n.nombre_nivel,padre.nombre_nivel AS parent_name FROM ra_pro_niveles n INNER JOIN remit_dest_interno u ON u.id_Remit_Dest_Int=n.remit_dest_interno_id_Remit_Dest_Int LEFT JOIN ra_pro_niveles padre ON padre.id_nivel=n.id_nivel_padre WHERE n.id_nivel<>? AND n.estado_nivel=1 AND u.Empresa_Gestion_Documental_id_empresa=? AND (u.id_Remit_Dest_Int=? OR EXISTS(SELECT 1 FROM ra_pro_permisos_niveles p WHERE p.ra_pro_niveles_id_nivel=n.id_nivel AND p.remit_dest_interno_id_Remit_Dest_Int=? AND p.estado_permiso=1 AND p.agregar_expediente=1 AND p.carga_archivo=1)) ORDER BY CASE WHEN n.id_nivel_padre=? THEN 0 WHEN n.id_nivel_padre=(SELECT original.id_nivel_padre FROM ra_pro_niveles original WHERE original.id_nivel=?) THEN 1 ELSE 2 END,n.nombre_nivel,n.id_nivel LIMIT 30',[originalLevel,principal.companyRef,principal.userRef,principal.userRef,originalLevel,originalLevel]);
  check(rows.length<=30&&rows.every(row=>id(String(row.id_nivel))&&String(row.id_nivel)!==originalLevel),'LEVEL_LIST_INVALID');
  return rows.map(row=>({id:String(row.id_nivel),name:displayText(row.nombre_nivel),parent:displayText(row.parent_name)}));
}
async function pendingFile(query,budget,principal,lotId){
  check(id(principal.userRef)&&id(principal.companyRef)&&id(lotId),'RESOURCE_INVALID');budget.take('select');
  const rows=await query('RA',"SELECT f.archivo_id,f.origen_doc,f.bytes_previstos FROM ra_integracion_archivo f INNER JOIN ra_integracion_raiz r ON r.raiz_id=f.raiz_id INNER JOIN ra_integracion_lote l ON l.lote_id=r.lote_id WHERE l.lote_id=? AND l.solicitante_ref=? AND l.ambito_ref=? AND f.estado_recepcion='PENDIENTE' AND f.estado_efecto='NO_INICIADO' AND f.intento_activo_id IS NULL ORDER BY f.archivo_id LIMIT 1",[lotId,principal.userRef,principal.companyRef]);
  check(rows.length===1&&id(String(rows[0].archivo_id)),'PENDING_RECEPTION_RESOURCE_REQUIRED');
  let origin;try{origin=JSON.parse(rows[0].origen_doc);}catch{check(false,'FILE_METADATA_INVALID');}
  check(typeof origin.Nombre==='string'&&origin.Nombre.length>0,'FILE_METADATA_INVALID');
  check(typeof origin.RutaRelativa==='string'&&!/^(?:[a-z]:|[\\/])/i.test(origin.RutaRelativa)&&!origin.RutaRelativa.split(/[\\/]/).includes('..'),'FILE_METADATA_INVALID');
  return {id:String(rows[0].archivo_id),name:displayText(origin.Nombre),relativePath:displayText(origin.RutaRelativa),bytes:String(rows[0].bytes_previstos)};
}
function createBudget(stage,notify=()=>{}){
  check(Object.hasOwn(LIMITS,stage),'STAGE_INVALID');const counts={http:0,select:0,sessions:0};
  return {counts,take(kind){check(Object.hasOwn(counts,kind),'BUDGET_KIND_INVALID');check(counts[kind]<LIMITS[stage][kind],'BUDGET_EXHAUSTED');counts[kind]++;notify({...counts});}};
}
function permittedRequest(method,url,payload,scope){
  let target;try{target=new URL(url);}catch{return false;}
  const base=new URL(BASE_URL);
  if(target.username||target.password||target.origin!==base.origin||!target.pathname.toLowerCase().startsWith(base.pathname.toLowerCase()))return false;
  const relative=target.pathname.slice(base.pathname.length).toLowerCase();
  if(/\.ashx(?:$|\/)/i.test(relative))return false;
  if(relative===LOGIN_LANDING_PATH)return ['GET','HEAD'].includes(method)&&!target.search;
  if(relative===LOGIN_STATUS_PATH)return method==='POST'&&!target.search&&payload&&typeof payload==='object'&&!Array.isArray(payload)&&Object.keys(payload).length===0;
  if(relative.startsWith('webservice/')||/\.asmx(?:\/|$)/i.test(relative)){
    if(method!=='POST'||!relative.startsWith('webservice/webserviceimportacioncarpetas.asmx/'))return false;
    const operation=target.pathname.split('/').pop();
    if(!READS.includes(operation)||!payload||typeof payload!=='object'||Array.isArray(payload))return false;
    const fields=operation==='ConsultarConfirmacionLote'?['planId']:operation==='ConsultarLote'?['loteId']:operation==='ConsultarRecursosLote'||operation==='ExportarPaginaLote'?['loteId','tipo','pagina','tamanoPagina','raizId','estado','estadoRecepcion']:['nivelRef','pagina','tamanoPagina'];
    if(Object.keys(payload).some(k=>!fields.includes(k)))return false;
    if(operation==='ConsultarConfirmacionLote')return id(scope.planId)&&payload.planId===scope.planId;
    if(operation==='AbrirImportacion'||operation==='ListarPerfilesImportacion')return id(scope.level)&&payload.nivelRef===scope.level;
    return id(scope.lotId)&&payload.loteId===scope.lotId;
  }
  if(method==='POST'){
    if(relative==='gestor.aspx')return true;
    if(relative!=='gestion/webformproduciondocumental.aspx'||scope.allowTree!==true||!payload)return false;
    const target=payload.__EVENTTARGET||'',buttons=Object.keys(payload).filter(k=>/(?:^|\$)Button_[^$]+$/.test(k));
    if(target&&!/(?:^|\$)Button_inline_trevie$/.test(target))return false;
    if(!target&&!buttons.some(k=>/(?:^|\$)Button_inline_trevie$/.test(k)))return false;
    if(buttons.some(k=>!/(?:^|\$)Button_inline_trevie$/.test(k))||payload.__EVENTARGUMENT)return false;
    const key=Object.keys(payload).find(k=>k==='HiddenField_0003'||k.endsWith('$HiddenField_0003')),title=key&&payload[key];
    if(typeof title!=='string'||!title.includes('\\')||title.includes('|')||title.length>4096)return false;
    return [scope.level,scope.alternateLevel].includes(title.split('\\')[0]);
  }
  if(/\.aspx$/i.test(relative)&&!['gestor.aspx','gestion/webformproduciondocumental.aspx',LOGIN_LANDING_PATH].includes(relative))return false;
  return method==='GET'||method==='HEAD';
}
function guardRequestSummary(request){
  // Only fixed categories reach evidence. Never retain URL parameters, form
  // fields, headers, accounts, tokens, return URLs or unknown operation names.
  let resource='INVALID_URL',operation=null;
  try{
    const target=new URL(request.url()),base=new URL(BASE_URL);
    if(target.username||target.password||target.origin!==base.origin)resource='OUTSIDE_ORIGIN';
    else if(!target.pathname.toLowerCase().startsWith(base.pathname.toLowerCase()))resource='OUTSIDE_APPLICATION';
    else{
      const relative=target.pathname.slice(base.pathname.length).toLowerCase();
      resource=relative===LOGIN_STATUS_PATH?'LOGIN_STATUS':relative===LOGIN_LANDING_PATH?'LOGIN_LANDING':relative==='gestor.aspx'?'LOGIN_FORM':/\.ashx(?:$|\/)/i.test(relative)?'HANDLER':/\.asmx(?:$|\/)/i.test(relative)?'SERVICE':/\.aspx$/i.test(relative)?'PAGE':'STATIC';
      const candidate=target.pathname.split('/').pop();if(READS.includes(candidate))operation=candidate;
    }
  }catch{}
  return {resource,method:['GET','HEAD','POST'].includes(request.method())?request.method():'OTHER',operation};
}
async function installGuard(context,budget,scope){
  const calls=[],violations=[],omittedPresentation=[],externalAssets=[];
  let firstViolation=null;
  await context.route('**/*',async route=>{
    const request=route.request();let payload;
    try{
      check(violations.length===0,'RUN_STOPPED');
      // The login references this optional font. Abort it locally without
      // making an external request or stopping the authorized application flow.
      if(request.method()==='GET'&&request.resourceType?.()==='stylesheet'&&request.url()==='https://fonts.googleapis.com/css?family=Open+Sans:300,400,400i,600,700,800'){
        omittedPresentation.push('GOOGLE_LOGIN_FONT');await route.abort('blockedbyclient');return;
      }
      const dependency=STATIC_DEPENDENCIES.find(item=>item.url===request.url()&&item.type===request.resourceType?.());
      if(dependency&&request.method()==='GET'&&!request.postData?.()){
        budget.take('http');
        const headers=Object.fromEntries(Object.entries(await request.allHeaders()).filter(([key])=>!['cookie','authorization','proxy-authorization','referer','origin'].includes(key.toLowerCase())));
        externalAssets.push(dependency.code);await route.continue({headers});return;
      }
      if(request.method()==='POST'&&/\/webservice\//i.test(request.url())){
        const envelope=JSON.parse(request.postData());
        payload=new URL(request.url()).pathname.toLowerCase()===new URL(LOGIN_STATUS_PATH,BASE_URL).pathname.toLowerCase()?envelope:envelope?.solicitud;
      }
      else if(request.method()==='POST'&&/\/Gestion\/WebFormProducionDocumental\.aspx(?:\?|$)/i.test(request.url()))payload=Object.fromEntries(new URLSearchParams(request.postData()||''));
      check(permittedRequest(request.method(),request.url(),payload,scope),'HTTP_SCOPE_DENIED');
      // The real login already issued its session cookie. The shared helper
      // opens Production next; do not execute the unrelated home dashboard.
      if(guardRequestSummary(request).resource==='LOGIN_LANDING'){
        omittedPresentation.push('HOME_LANDING');
        await route.fulfill({status:200,contentType:'text/html; charset=utf-8',body:'<!doctype html><html><body></body></html>'});return;
      }
      budget.take('http');
      if(payload&&/\/webservice\//i.test(request.url()))calls.push({operation:new URL(request.url()).pathname.split('/').pop(),request:payload});
      await route.continue();
    }catch(error){if(!firstViolation)firstViolation=guardRequestSummary(request);violations.push(/^DOC119_[A-Z_]+$/.test(error.message)?error.message:'DOC119_HTTP_SCOPE_DENIED');await route.abort('blockedbyclient');}
  });return {calls,violations,omittedPresentation,externalAssets,get firstViolation(){return firstViolation;}};
}
function makeControls(query,budget){
  const read=async(origin,sql,args)=>{budget.take('select');return query(origin,sql,args);};
  const existing=createSelectControls(read);
  return {async snapshot(lotId){
    check(id(lotId),'LOT_INVALID');const snapshot=await existing.snapshot(lotId);
    let context;try{context=JSON.parse(snapshot.lot.contexto_doc);}catch{check(false,'CONTEXT_INVALID');}
    check(id(context.UsuarioRef)&&id(context.NivelRef)&&id(context.AmbitoRef),'CONTEXT_INVALID');
    const plans=await read('RA','SELECT plan_id FROM ra_integracion_lote WHERE lote_id=?',[lotId]);
    check(plans.length===1&&id(String(plans[0].plan_id)),'PLAN_INVALID');
    const metadata=await read('RA','SELECT f.archivo_id,f.origen_doc,f.bytes_recibidos FROM ra_integracion_archivo f INNER JOIN ra_integracion_raiz r ON r.raiz_id=f.raiz_id WHERE r.lote_id=? ORDER BY f.archivo_id',[lotId]);
    check(metadata.length===snapshot.files.length&&new Set(metadata.map(f=>String(f.archivo_id))).size===metadata.length,'FILE_METADATA_INVALID');
    const byId=new Map(metadata.map(f=>[String(f.archivo_id),f]));
    snapshot.files=snapshot.files.map(file=>{const item=byId.get(String(file.archivo_id));check(item&&typeof item.origen_doc==='string','FILE_METADATA_INVALID');return {...file,origen_doc:item.origen_doc,bytes_recibidos:item.bytes_recibidos};});
    const expeditions=[];
    for(const root of snapshot.roots){if(root.identidad_expediente_doc){let identity;try{identity=JSON.parse(root.identidad_expediente_doc);}catch{check(false,'RELATION_INVALID');}if(id(identity.ExpedienteId))expeditions.push(identity.ExpedienteId);}}
    const refs=[...new Set(expeditions)];check(refs.length<=100,'RESOURCE_TOO_LARGE');
    const documents=refs.length?await read('DA','SELECT r.ID_REGISTRO_PRODUCION_DOCUMENTAL AS inventory_id,r.ID_DOCUMENTO_DOCUARCHI_ALMACEN AS document_id,r.EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE AS expedition_id,r.NOMBRE_GABINETE AS cabinet_name,i.expediente_archivo_ID_EXPEDIENTE AS indexed_expedition FROM registro_producion_documental r LEFT JOIN ra_cert_indice_expediente i ON i.registro_producion_documental_ID_REGISTRO_PRODUCION_DOCUMENTAL=r.ID_REGISTRO_PRODUCION_DOCUMENTAL WHERE r.ID_USUARIO_GESTION=? AND r.EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE IN ('+refs.map(()=>'?').join(',')+') ORDER BY r.ID_REGISTRO_PRODUCION_DOCUMENTAL,i.expediente_archivo_ID_EXPEDIENTE',[context.UsuarioRef,...refs]):[];
    return {data:snapshot,context,planId:String(plans[0].plan_id),fingerprint:hash({snapshot,documents}),documents:documents.length,daScope:refs.length?'KNOWN_EXPEDITIONS':'NONE_CONFIRMED'};
  }};
}
async function responseFor(page,operation,action,type){
  const upstream=r=>operation==='ConsultarRecursosLote'&&new URL(r.url()).pathname.endsWith('/ConsultarLote');
  const response=page.waitForResponse(async r=>{
    if(r.request().method()!=='POST')return false;
    if(upstream(r)){
      if(r.status()>=300)return true;
      try{const raw=await r.json();return (raw?.d??raw)?.ok!==true;}catch{return true;}
    }
    if(!new URL(r.url()).pathname.endsWith('/'+operation))return false;
    if(!type)return true;
    try{return JSON.parse(r.request().postData()).solicitud.tipo===type;}catch{return false;}
  });
  let r;
  try{[r]=await Promise.all([response,Promise.resolve().then(action)]);}
  catch(error){if(error?.name==='TimeoutError')check(false,'READ_STEP_TIMEOUT');throw error;}
  let body;try{const raw=await r.json();body=raw?.d??raw;}catch{check(!upstream(r)||!(r.status()===401||r.status()>=300&&r.status()<400),'SESSION_REQUIRED');check(false,'RESPONSE_INVALID');}
  if(upstream(r)){
    check(r.status()!==401&&!(r.status()>=300&&r.status()<400)&&body?.codigo!=='SESION_REQUERIDA','SESSION_REQUIRED');
    check(r.status()!==403&&body?.codigo!=='ACCESO_DENEGADO','RESOURCE_ACCESS_DENIED');
    check(false,'LOT_QUERY_REJECTED');
  }
  return {status:r.status(),body};
}
async function consult(page,lotId,resume=false){
  const r=await responseFor(page,'ConsultarRecursosLote',async()=>{
    const access=resume?'[data-doc111-resume]':'[data-doc111-query-open]';
    if(await page.locator(access).count()){
      if(!await page.locator(access).isVisible()&&await page.locator('#doc110-navigation-toggle').count())await page.locator('#doc110-navigation-toggle').click();
      await page.locator(access).click();
    }
    if(!resume){await page.locator('[data-doc111-lote]').fill(lotId);await page.locator('[data-doc111-consultar]').click();}
  },'ARCHIVOS');
  check(r.status===200&&r.body?.ok&&r.body.datos?.loteId===lotId,'QUERY_FAILED');
  await page.getByRole('button',{name:'Consultar estado',exact:true}).waitFor();
  await page.waitForFunction(()=>{const b=document.querySelector('[data-doc111-action="consultar"]');return b&&!b.disabled;});return r.body.datos;
}
function csvRows(text){
  check(typeof text==='string'&&text.length<=8*1024*1024,'CSV_INVALID');const rows=[];let row=[],value='',quoted=false;
  for(let i=0;i<text.length;i++){
    const c=text[i];
    if(c==='"'){if(quoted&&text[i+1]==='"'){value+='"';i++;}else quoted=!quoted;}
    else if(!quoted&&(c===','||c==='\n'||c==='\r')){row.push(value);value='';if(c!==','){if(c==='\r'&&text[i+1]==='\n')i++;rows.push(row);row=[];}}
    else value+=c;
  }
  check(!quoted,'CSV_INVALID');if(value.length||row.length){row.push(value);rows.push(row);}return rows;
}
async function applyFilter(page,values){
  await page.getByLabel('Referencia de carpeta',{exact:true}).fill(values.raizId||'');
  await page.getByLabel('Efecto documental',{exact:true}).selectOption(values.estado||'');
  await page.getByLabel('Recepción',{exact:true}).selectOption(values.estadoRecepcion||'');
  const r=await responseFor(page,'ConsultarRecursosLote',()=>page.getByRole('button',{name:'Aplicar filtros de archivos',exact:true}).click(),'ARCHIVOS');
  check(r.status===200&&r.body?.ok,'FILTER_FAILED');await page.waitForFunction(()=>!document.querySelector('[data-doc111-action="consultar"]')?.disabled);return r.body.datos;
}
async function runSeg01Owner(page,lotId,baseline,guard){
  let pageData=await consult(page,lotId);const original=Number(baseline.data.lot.total_archivos);
  check(pageData.totalOriginal===original&&pageData.elementos.length>0,'TOTALS_INVALID');
  check(new Set(baseline.data.files.map(f=>f.estado_recepcion+'|'+f.estado_efecto)).size>1,'MIXED_STATES_REQUIRED');
  const first=pageData.elementos[0];
  const filters={raizId:first.raizId,estado:first.estado,estadoRecepcion:first.estadoRecepcion};
  const narrowed=await applyFilter(page,filters);check(narrowed.totalOriginal===original&&narrowed.elementos.every(f=>f.raizId===filters.raizId&&f.estado===filters.estado&&f.estadoRecepcion===filters.estadoRecepcion),'FILTER_RESULT_INVALID');
  // Find an existing filter whose result really has a second UI page.
  const size=pageData.tamanoPagina;let pageable;
  for(const [field,column] of [['estadoRecepcion','estado_recepcion'],['estado','estado_efecto'],['raizId','raiz_id']]){
    const groups=new Map();for(const f of baseline.data.files){const v=String(f[column]);groups.set(v,(groups.get(v)||0)+1);}
    for(const [value,total]of groups)if(total>size&&!pageable)pageable={[field]:value};
  }
  let pagination=false;
  if(pageable){
    pageData=await applyFilter(page,pageable);
    const second=await responseFor(page,'ConsultarRecursosLote',()=>page.getByRole('button',{name:'Siguiente',exact:true}).last().click(),'ARCHIVOS');
    check(second.status===200&&second.body?.ok&&second.body.datos.pagina===2,'SECOND_PAGE_FAILED');pageData=second.body.datos;
    check(pageData.totalOriginal===original&&pageData.elementos.length>0,'PAGE_TOTALS_INVALID');pagination=true;
  }else pageData=narrowed;
  const expected=guard.calls.filter(c=>c.operation==='ConsultarRecursosLote'&&c.request.tipo==='ARCHIVOS').at(-1).request;
  for(const [key,value]of Object.entries(pageable||filters))check(expected[key]===value,'PAGE_FILTER_LOST');
  await page.waitForFunction(()=>!document.querySelector('[data-doc111-action="exportar"]')?.disabled);
  const [download]=await Promise.all([page.waitForEvent('download'),page.getByRole('button',{name:'Exportar esta página',exact:true}).click()]);
  check(!(await download.failure()),'DOWNLOAD_FAILED');const stream=await download.createReadStream(),chunks=[];let bytes=0;
  for await(const chunk of stream){bytes+=chunk.length;check(bytes<=8*1024*1024,'CSV_TOO_LARGE');chunks.push(chunk);}
  const csv=csvRows(Buffer.concat(chunks).toString('utf8'));
  check(csv.length===pageData.elementos.length+1&&csv[0][0]==='recursoId','CSV_PAGE_INVALID');
  check(JSON.stringify(csv.slice(1).map(row=>row[0]))===JSON.stringify(pageData.elementos.map(f=>f.recursoId)),'CSV_IDENTITIES_INVALID');
  check(csv.every(row=>row.length===csv[0].length),'CSV_COLUMNS_INVALID');
  check(csv.slice(1).every(row=>row.every(cell=>! /^[\s\u0000-\u001f\u007f-\u009f\uFEFF\u200B]*[=+@-]/.test(cell))),'CSV_FORMULA_UNSAFE');
  const exported=guard.calls.filter(c=>c.operation==='ExportarPaginaLote').at(-1)?.request;
  check(exported&&JSON.stringify(exported)===JSON.stringify(expected),'CSV_QUERY_MISMATCH');
  const emptyState=['NO_INICIADO','EN_CURSO','ALMACENADO','COMPLETADO','RECHAZADO','INCIERTO','CANCELADO'].find(s=>!baseline.data.files.some(f=>String(f.raiz_id)===first.raizId&&f.estado_efecto===s));
  check(emptyState,'EMPTY_FILTER_RESOURCE_REQUIRED');const empty=await applyFilter(page,{raizId:first.raizId,estado:emptyState});
  check(empty.totalFiltrado===0&&empty.elementos.length===0&&empty.totalOriginal===original,'EMPTY_FILTER_INVALID');
  return {ownerRead:true,filters:true,pagination,csvDownload:true,emptyFilter:true,originalFiles:original,csvRows:csv.length-1,...pagination?{}:{paginationLimitation:'INSUFFICIENT_EXISTING_ROWS'}};
}
async function runSeg01Other(page,lotId){
  const denied=await responseFor(page,'ConsultarLote',async()=>{await page.locator('[data-doc111-lote]').fill(lotId);await page.locator('[data-doc111-consultar]').click();});
  check(!denied.body?.ok&&denied.body?.codigo==='ACCESO_DENEGADO'&&denied.body.datos==null,'OTHER_ACCOUNT_NOT_DENIED');
  const exported=await page.evaluate(async id=>{
    const host=document.getElementById('doc111-importacion');
    const api=ImportacionCarpetasApi.crear({url:host.getAttribute('data-api'),uploadUrl:host.getAttribute('data-upload'),token:host.getAttribute('data-token')});
    try{await api.invocar('ExportarPaginaLote',{loteId:id,tipo:'ARCHIVOS',pagina:1,tamanoPagina:1});return false;}
    catch(error){return error.codigo==='ACCESO_DENEGADO';}finally{api.cerrar();}
  },lotId);check(exported,'OTHER_CSV_NOT_DENIED');return {otherAccountDenied:true,otherCsvDenied:true};
}
function observeOriginalBranch({level,identities}){
  const tree=document.getElementById('TreeViewArchivo');
  const nodes=Array.from(tree?.querySelectorAll('a[id][title]')||[]).filter(a=>/^TreeViewArchivot[0-9]+$/.test(a.id));
  const parents=nodes.filter(a=>a.title.split('\\')[0]===level&&a.title.includes('\\'));
  const parent=parents.length===1?parents[0]:null;
  const branch=parent?document.getElementById(parent.id.replace('TreeViewArchivot','TreeViewArchivon')+'Nodes'):null;
  const reason=parents.length===0?'PARENT_ABSENT':parents.length!==1?'PARENT_AMBIGUOUS':!parent.getClientRects().length?'PARENT_HIDDEN':!branch?'BRANCH_ABSENT':'AVAILABLE';
  return {available:reason==='AVAILABLE',reason,selection:window.SELECCION_TREVIEW_ID,display:branch?.style.display??null,counts:identities.map(key=>nodes.filter(a=>a.title.replace(/\|$/,'')===key.replace(/\|$/,'')).length)};
}
function safeTreeDiagnostic(value){
  if(!value||typeof value!=='object'||typeof value.selectionPreserved!=='boolean'||typeof value.expansionPreserved!=='boolean')return null;
  const reasons=['AVAILABLE','PARENT_ABSENT','PARENT_AMBIGUOUS','PARENT_HIDDEN','BRANCH_ABSENT'];
  const valid=side=>side&&reasons.includes(side.availability)&&Array.isArray(side.counts)&&side.counts.length>0&&side.counts.length<=100&&side.counts.every(n=>Number.isSafeInteger(n)&&n>=0&&n<=10000);
  if(!valid(value.before)||!valid(value.after)||value.before.counts.length!==value.after.counts.length)return null;
  return {before:{availability:value.before.availability,counts:[...value.before.counts]},after:{availability:value.after.availability,counts:[...value.after.counts]},selectionPreserved:value.selectionPreserved,expansionPreserved:value.expansionPreserved};
}
async function runSeg02(page,profile,baseline,filePath){
  const pending=baseline.data.files.find(f=>f.estado_recepcion==='PENDIENTE'&&f.estado_efecto==='NO_INICIADO'&&!f.intento_activo_id);
  check(pending,'PENDING_RECEPTION_RESOURCE_REQUIRED');
  check(profile.alternateLevel!==baseline.context.NivelRef,'ALTERNATE_LEVEL_REQUIRED');
  const identities=baseline.data.roots.filter(r=>['EXPEDIENTE_CONFIRMADO','COMPLETADA','PARCIAL'].includes(r.estado)&&!r.intento_activo_id&&r.identidad_expediente_doc).map(r=>{
    let identity;try{identity=JSON.parse(r.identidad_expediente_doc);}catch{check(false,'RELATION_INVALID');}
    check(id(identity.RelacionNivelId)&&id(identity.ExpedienteId),'RELATION_INVALID');return identity.RelacionNivelId+'|'+baseline.context.NivelRef+'|'+identity.ExpedienteId+'|';
  });
  check(identities.length>0,'CONFIRMED_ROOT_RESOURCE_REQUIRED');
  await selectLevel(page,baseline.context.NivelRef);await consult(page,profile.recoveryLotId);
  await page.getByRole('button',{name:'Cerrar',exact:true}).click();
  await page.reload({waitUntil:'domcontentloaded'});
  await page.locator('[data-doc111-resume]').waitFor({state:'visible'});
  await selectLevel(page,profile.alternateLevel);
  const before=await page.evaluate(observeOriginalBranch,{level:baseline.context.NivelRef,identities});
  let result=await consult(page,profile.recoveryLotId,true);
  while(!result.elementos.some(row=>row.recursoId===String(pending.archivo_id))&&result.puedeHaberMas){
    const next=await responseFor(page,'ConsultarRecursosLote',()=>page.getByRole('button',{name:'Siguiente',exact:true}).last().click(),'ARCHIVOS');
    check(next.status===200&&next.body?.ok,'QUERY_FAILED');result=next.body.datos;
    await page.waitForFunction(()=>!document.querySelector('[data-doc111-action="consultar"]')?.disabled);
  }
  check(result.elementos.some(row=>row.recursoId===String(pending.archivo_id)),'PENDING_RECEPTION_NOT_VISIBLE');
  const [chooser]=await Promise.all([page.waitForEvent('filechooser'),page.getByRole('button',{name:'Reseleccionar archivo '+pending.archivo_id,exact:true}).click()]);await chooser.setFiles(filePath);
  await page.waitForFunction(()=>document.querySelector('.doc111-notice')?.textContent.includes('Archivo vinculado')||document.querySelector('.doc111-notice')?.textContent.includes('no coincide'));
  check((await page.locator('.doc111-notice').textContent()).includes('Archivo vinculado'),'RESELECTION_METADATA_MISMATCH');
  const repeated=await responseFor(page,'ConsultarRecursosLote',()=>page.getByRole('button',{name:'Consultar estado',exact:true}).click(),'ARCHIVOS');
  check(repeated.body?.ok,'REQUERY_FAILED');await page.waitForFunction(()=>!document.querySelector('[data-doc111-action="consultar"]')?.disabled);
  const after=await page.evaluate(observeOriginalBranch,{level:baseline.context.NivelRef,identities});
  const treeDiagnostic=safeTreeDiagnostic({before:{availability:before.reason,counts:before.counts},after:{availability:after.reason,counts:after.counts},selectionPreserved:after.selection===before.selection,expansionPreserved:after.reason===before.reason&&after.display===before.display});
  const treeCheck=(ok,code)=>{if(!ok){const error=Error('DOC119_'+code);error.treeDiagnostic=treeDiagnostic;throw error;}};
  treeCheck(treeDiagnostic.selectionPreserved,'TREE_SELECTION_CHANGED');
  treeCheck(treeDiagnostic.expansionPreserved,'TREE_EXPANSION_CHANGED');
  treeCheck(before.available?after.counts.every(n=>n===1):after.counts.every((n,i)=>n<=1&&n===before.counts[i]),'TREE_RELATION_COUNT_INVALID');
  const names=baseline.data.roots.map(r=>{try{return JSON.parse(r.datos_doc).NombreOriginal;}catch{return null;}}).filter(Boolean);
  return {reload:true,reselectionMetadataOnly:true,noUpload:true,selectionPreserved:true,treeDeduplicated:before.available,originalBranchAvailability:before.reason,...before.available?{}:{originalBranchUntouched:true},newRelationsAdded:after.counts.some((n,i)=>n>before.counts[i]),homonymousRootsObserved:new Set(names).size<names.length,confirmedRelations:identities.length,treeDiagnostic};
}
module.exports={BASE_URL,LIMITS,READS,STATIC_DEPENDENCIES,SESSION_IDLE_LIMIT_MS,check,id,hash,assertSessionFresh,previousSelection,validateConnection,validateProfile,displayText,chooseItem,describeImport,discoverImports,discoverLevels,pendingFile,createBudget,permittedRequest,guardRequestSummary,installGuard,makeControls,responseFor,consult,applyFilter,csvRows,safeTreeDiagnostic,runSeg01Owner,runSeg01Other,runSeg02};
