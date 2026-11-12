'use strict';
const crypto=require('node:crypto');
const {createSelectControls}=require('./doc111-importacion-carpetas.cjs');
const {selectLevel}=require('./doc113-ventana-importacion.cjs');
const BASE_URL='https://localhost/GestionDocumental-Docuarchi.net/';
const LIMITS=Object.freeze({'SEG-01':Object.freeze({http:120,select:80,sessions:2}),'SEG-02':Object.freeze({http:80,select:60,sessions:2})});
const READS=Object.freeze(['AbrirImportacion','ListarPerfilesImportacion','ConsultarLote','ConsultarRecursosLote','ExportarPaginaLote','ConsultarConfirmacionLote']);
const check=(ok,code)=>{if(!ok)throw Error('DOC119_'+code);};
const id=value=>typeof value==='string'&&/^[1-9][0-9]{0,9}$/.test(value)&&BigInt(value)<=4294967295n;
const hash=value=>crypto.createHash('sha256').update(typeof value==='string'||Buffer.isBuffer(value)?value:JSON.stringify(value)).digest('hex');
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
  const rows=await query('RA','SELECT n.id_nivel,n.nombre_nivel,padre.nombre_nivel AS parent_name FROM ra_pro_niveles n INNER JOIN remit_dest_interno u ON u.id_Remit_Dest_Int=n.remit_dest_interno_id_Remit_Dest_Int LEFT JOIN ra_pro_niveles padre ON padre.id_nivel=n.id_nivel_padre WHERE n.id_nivel<>? AND n.estado_nivel=1 AND u.Empresa_Gestion_Documental_id_empresa=? AND (u.id_Remit_Dest_Int=? OR EXISTS(SELECT 1 FROM ra_pro_permisos_niveles p WHERE p.ra_pro_niveles_id_nivel=n.id_nivel AND p.remit_dest_interno_id_Remit_Dest_Int=? AND p.estado_permiso=1 AND p.agregar_expediente=1 AND p.carga_archivo=1)) ORDER BY n.nombre_nivel,n.id_nivel LIMIT 30',[originalLevel,principal.companyRef,principal.userRef,principal.userRef]);
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
  if(/\.aspx$/i.test(relative)&&!['gestor.aspx','gestion/webformproduciondocumental.aspx'].includes(relative))return false;
  return method==='GET'||method==='HEAD';
}
async function installGuard(context,budget,scope){
  const calls=[],violations=[],omittedPresentation=[];
  await context.route('**/*',async route=>{
    const request=route.request();let payload;
    try{
      check(violations.length===0,'RUN_STOPPED');
      // The login references this optional font. Abort it locally without
      // making an external request or stopping the authorized application flow.
      if(request.method()==='GET'&&request.resourceType?.()==='stylesheet'&&request.url()==='https://fonts.googleapis.com/css?family=Open+Sans:300,400,400i,600,700,800'){
        omittedPresentation.push('GOOGLE_LOGIN_FONT');await route.abort('blockedbyclient');return;
      }
      if(request.method()==='POST'&&/\/webservice\//i.test(request.url()))payload=JSON.parse(request.postData()).solicitud;
      else if(request.method()==='POST'&&/\/Gestion\/WebFormProducionDocumental\.aspx(?:\?|$)/i.test(request.url()))payload=Object.fromEntries(new URLSearchParams(request.postData()||''));
      check(permittedRequest(request.method(),request.url(),payload,scope),'HTTP_SCOPE_DENIED');
      budget.take('http');
      if(payload&&/\/webservice\//i.test(request.url()))calls.push({operation:new URL(request.url()).pathname.split('/').pop(),request:payload});
      await route.continue();
    }catch(error){violations.push(/^DOC119_[A-Z_]+$/.test(error.message)?error.message:'DOC119_HTTP_SCOPE_DENIED');await route.abort('blockedbyclient');}
  });return {calls,violations,omittedPresentation};
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
    const expeditions=[];
    for(const root of snapshot.roots){if(root.identidad_expediente_doc){let identity;try{identity=JSON.parse(root.identidad_expediente_doc);}catch{check(false,'RELATION_INVALID');}if(id(identity.ExpedienteId))expeditions.push(identity.ExpedienteId);}}
    const refs=[...new Set(expeditions)];check(refs.length<=100,'RESOURCE_TOO_LARGE');
    const documents=refs.length?await read('DA','SELECT r.ID_REGISTRO_PRODUCION_DOCUMENTAL AS inventory_id,r.ID_DOCUMENTO_DOCUARCHI_ALMACEN AS document_id,r.EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE AS expedition_id,r.NOMBRE_GABINETE AS cabinet_name,i.expediente_archivo_ID_EXPEDIENTE AS indexed_expedition FROM registro_producion_documental r LEFT JOIN ra_cert_indice_expediente i ON i.registro_producion_documental_ID_REGISTRO_PRODUCION_DOCUMENTAL=r.ID_REGISTRO_PRODUCION_DOCUMENTAL WHERE r.ID_USUARIO_GESTION=? AND r.EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE IN ('+refs.map(()=>'?').join(',')+') ORDER BY r.ID_REGISTRO_PRODUCION_DOCUMENTAL,i.expediente_archivo_ID_EXPEDIENTE',[context.UsuarioRef,...refs]):[];
    return {data:snapshot,context,planId:String(plans[0].plan_id),fingerprint:hash({snapshot,documents}),documents:documents.length,daScope:refs.length?'KNOWN_EXPEDITIONS':'NONE_CONFIRMED'};
  }};
}
async function responseFor(page,operation,action,type){
  const response=page.waitForResponse(r=>{
    if(r.request().method()!=='POST'||!new URL(r.url()).pathname.endsWith('/'+operation))return false;
    if(!type)return true;
    try{return JSON.parse(r.request().postData()).solicitud.tipo===type;}catch{return false;}
  });
  const [r]=await Promise.all([response,Promise.resolve().then(action)]);let body;try{const raw=await r.json();body=raw?.d??raw;}catch{check(false,'RESPONSE_INVALID');}
  return {status:r.status(),body};
}
async function consult(page,lotId,resume=false){
  const r=await responseFor(page,'ConsultarRecursosLote',async()=>{
    if(resume)await page.locator('[data-doc111-resume]').click();
    else{await page.locator('[data-doc111-lote]').fill(lotId);await page.locator('[data-doc111-consultar]').click();}
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
  check(pageable,'PAGEABLE_FILTER_RESOURCE_REQUIRED');
  pageData=await applyFilter(page,pageable);
  const second=await responseFor(page,'ConsultarRecursosLote',()=>page.getByRole('button',{name:'Siguiente',exact:true}).last().click(),'ARCHIVOS');
  check(second.status===200&&second.body?.ok&&second.body.datos.pagina===2,'SECOND_PAGE_FAILED');pageData=second.body.datos;
  check(pageData.totalOriginal===original&&pageData.elementos.length>0,'PAGE_TOTALS_INVALID');
  const expected=guard.calls.filter(c=>c.operation==='ConsultarRecursosLote'&&c.request.tipo==='ARCHIVOS').at(-1).request;
  for(const [key,value]of Object.entries(pageable))check(expected[key]===value,'PAGE_FILTER_LOST');
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
  return {ownerRead:true,filters:true,pagination:true,csvDownload:true,emptyFilter:true,originalFiles:original,csvRows:csv.length-1};
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
  const before=await page.evaluate(({level,identities})=>{
    const tree=document.getElementById('TreeViewArchivo'),parents=Array.from(tree?.querySelectorAll('a[title]')||[]).filter(a=>a.title.split('\\')[0]===level&&a.title.includes('\\'));
    if(parents.length!==1||!parents[0].getClientRects().length)return null;
    const branch=document.getElementById(parents[0].id.replace('TreeViewArchivot','TreeViewArchivon')+'Nodes');if(!branch)return null;
    return {selection:window.SELECCION_TREVIEW_ID,display:branch.style.display,counts:identities.map(key=>Array.from(tree.querySelectorAll('a[title]')).filter(a=>a.title.replace(/\|$/,'')===key.replace(/\|$/,'')).length)};
  },{level:baseline.context.NivelRef,identities});
  check(before,'ORIGINAL_BRANCH_REQUIRED');
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
  const after=await page.evaluate(({level,identities})=>{
    const tree=document.getElementById('TreeViewArchivo'),parents=Array.from(tree.querySelectorAll('a[title]')).filter(a=>a.title.split('\\')[0]===level&&a.title.includes('\\'));
    const branch=parents.length===1?document.getElementById(parents[0].id.replace('TreeViewArchivot','TreeViewArchivon')+'Nodes'):null;
    return {selection:window.SELECCION_TREVIEW_ID,display:branch?.style.display,counts:identities.map(key=>Array.from(tree.querySelectorAll('a[title]')).filter(a=>a.title.replace(/\|$/,'')===key.replace(/\|$/,'')).length)};
  },{level:baseline.context.NivelRef,identities});
  check(after.selection===before.selection,'TREE_SELECTION_CHANGED');
  check(after.display===before.display,'TREE_EXPANSION_CHANGED');
  check(after.counts.every(n=>n===1),'TREE_RELATION_COUNT_INVALID');
  const names=baseline.data.roots.map(r=>{try{return JSON.parse(r.datos_doc).NombreOriginal;}catch{return null;}}).filter(Boolean);
  return {reload:true,reselectionMetadataOnly:true,noUpload:true,selectionPreserved:true,treeDeduplicated:true,newRelationsAdded:after.counts.some((n,i)=>n>before.counts[i]),homonymousRootsObserved:new Set(names).size<names.length,confirmedRelations:identities.length};
}
module.exports={BASE_URL,LIMITS,READS,check,id,hash,validateConnection,validateProfile,displayText,chooseItem,describeImport,discoverImports,discoverLevels,pendingFile,createBudget,permittedRequest,installGuard,makeControls,responseFor,consult,applyFilter,csvRows,runSeg01Owner,runSeg01Other,runSeg02};
