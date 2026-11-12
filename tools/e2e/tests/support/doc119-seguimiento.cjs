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
function validateProfile(p){
  check(p&&Object.keys(p).every(k=>['baseUrl','module','ignoreHttpsErrors','timeoutMilliseconds','odbcDsn','docuarchiOdbcDsn','lotId','recoveryLotId','alternateLevel'].includes(k)),'PROFILE_INVALID');
  check(p.baseUrl===BASE_URL&&p.module==='GESTOR'&&typeof p.ignoreHttpsErrors==='boolean'&&p.timeoutMilliseconds===60000,'PROFILE_INVALID');
  check(id(p.lotId)&&(p.recoveryLotId===null||id(p.recoveryLotId))&&(p.alternateLevel===null||id(p.alternateLevel)),'RESOURCE_INVALID');
  check(!p.recoveryLotId||p.alternateLevel,'ALTERNATE_LEVEL_REQUIRED');
  check([p.odbcDsn,p.docuarchiOdbcDsn].every(v=>typeof v==='string'&&/^[A-Za-z0-9 _.-]{1,128}$/.test(v)),'DSN_INVALID');return p;
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
    if(operation==='AbrirImportacion'||operation==='ListarPerfilesImportacion')return payload.nivelRef===scope.level;
    return payload.loteId===scope.lotId;
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
  const calls=[],violations=[];
  await context.route('**/*',async route=>{
    const request=route.request();let payload;
    try{
      check(violations.length===0,'RUN_STOPPED');
      if(request.method()==='POST'&&/\/webservice\//i.test(request.url()))payload=JSON.parse(request.postData()).solicitud;
      else if(request.method()==='POST'&&/\/Gestion\/WebFormProducionDocumental\.aspx(?:\?|$)/i.test(request.url()))payload=Object.fromEntries(new URLSearchParams(request.postData()||''));
      check(permittedRequest(request.method(),request.url(),payload,scope),'HTTP_SCOPE_DENIED');
      budget.take('http');
      if(payload&&/\/webservice\//i.test(request.url()))calls.push({operation:new URL(request.url()).pathname.split('/').pop(),request:payload});
      await route.continue();
    }catch(error){violations.push(/^DOC119_[A-Z_]+$/.test(error.message)?error.message:'DOC119_HTTP_SCOPE_DENIED');await route.abort('blockedbyclient');}
  });return {calls,violations};
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
module.exports={BASE_URL,LIMITS,READS,check,id,hash,validateProfile,createBudget,permittedRequest,installGuard,makeControls,responseFor,consult,applyFilter,csvRows,runSeg01Owner,runSeg01Other,runSeg02};
