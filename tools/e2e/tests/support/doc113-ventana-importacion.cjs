'use strict';
const fs=require('node:fs'),path=require('node:path');
const {validateProfile,readFixture,createSelectControls,FILE_NAME}=require('./doc111-importacion-carpetas.cjs');
const CASES=['UI-01','UI-02','UI-03'];
const READS=['AbrirImportacion','ListarPerfilesImportacion','ConsultarLote','ConsultarRecursosLote'];
const check=(ok,code)=>{if(!ok)throw new Error('DOC113_'+code);};
const id=value=>typeof value==='string'&&/^[1-9][0-9]{0,9}$/.test(value)&&BigInt(value)<=4294967295n;
function validateOptions(profile,options){
  validateProfile(profile);check(options&&CASES.includes(options.scenario),'SCENARIO_INVALID');
  if(options.scenario!=='UI-02')check(id(options.alternateLevel)&&options.alternateLevel!==profile.nivelRef,'ALTERNATE_LEVEL_REQUIRED');
  if(options.scenario==='UI-01')check(id(options.alternateVersion)&&options.alternateVersion!==profile.versionId,'ALTERNATE_VERSION_REQUIRED');
  check(typeof options.confirm==='boolean'&&(!options.confirm||options.scenario==='UI-02'),'CONFIRM_SCOPE_INVALID');
  check(profile.rootNames[0]!==profile.rootNames[1],'FIXTURE_NAMES_INVALID');return options;
}
function assertAuthorization(options,auth,profile){
  check(auth&&['environment','account','select'].every(k=>auth[k]===true),'AUTHORIZATION_REQUIRED');
  if(options.scenario==='UI-02')check(auth.effects===true&&auth.discardable===true,'EFFECTS_AUTHORIZATION_REQUIRED');
  if(options.confirm)check(auth.confirm===true,'CONFIRM_AUTHORIZATION_REQUIRED');
  if(options.scenario==='UI-03')check(auth.errorProbe===true,'PROBE_AUTHORIZATION_REQUIRED');
  if(profile.ignoreHttpsErrors)check(auth.tls===true,'TLS_AUTHORIZATION_REQUIRED');
}
function allowedOperation(operation,options){return READS.includes(operation)||(options.scenario==='UI-02'&&(operation==='PrepararPlan'||options.confirm&&operation==='ConfirmarPlan'));}
async function guardRequests(page,options,profile){
  const calls=[],violations=[];let invalidOpening=false,confirmationPlan=null;
  const pattern=/\/(?:webservice\/WebServiceImportacionCarpetas\.asmx(?:\/|$)|generic_control\/ImportacionCarpetasUpload\.ashx(?:\?|$))/i;
  const handler=async route=>{
    const request=route.request(),url=new URL(request.url());
    if(request.method()!=='POST')return route.continue();
    const operation=url.pathname.split('/').pop();
    if(!allowedOperation(operation,options)){violations.push('OPERATION_NOT_ALLOWED');return route.abort('blockedbyclient');}
    let payload;try{payload=JSON.parse(request.postData()).solicitud;}catch{violations.push('REQUEST_INVALID');return route.abort('blockedbyclient');}
    if(!payload||typeof payload!=='object'||Array.isArray(payload)){violations.push('REQUEST_INVALID');return route.abort('blockedbyclient');}
    if((operation==='PrepararPlan'&&(payload.nivelRef!==profile.nivelRef||payload.versionId!==profile.versionId))||(operation==='ConfirmarPlan'&&payload.planId!==confirmationPlan)){violations.push('RESOURCE_NOT_ALLOWED');return route.abort('blockedbyclient');}
    calls.push({operation,payload});
    if(invalidOpening&&operation==='AbrirImportacion'){
      invalidOpening=false;return route.continue({postData:JSON.stringify({solicitud:{nivelRef:'0'}})});
    }
    return route.continue();
  };
  await page.route(pattern,handler);
  return {calls,violations,permitConfirmation(planId){check(id(planId),'PLAN_INVALID');confirmationPlan=planId;},rejectNextOpening(){check(options.scenario==='UI-03','PROBE_SCOPE_INVALID');invalidOpening=true;},close:()=>page.unroute(pattern,handler)};
}
async function selectLevel(page,level){
  await page.waitForFunction(level=>Array.isArray(window.ITEMS_DATOS)&&window.ITEMS_DATOS.some(i=>String(document.getElementById(i.id)?.title||'').split('\\')[0]===level),level);
  const [response]=await Promise.all([
    page.waitForResponse(r=>r.request().method()==='POST'&&/\/Gestion\/WebFormProducionDocumental\.aspx(?:\?|$)/i.test(r.url())),
    page.evaluate(level=>{
      const matches=window.ITEMS_DATOS.filter(i=>String(document.getElementById(i.id)?.title||'').split('\\')[0]===level);
      if(matches.length!==1||typeof window.OnSearchClick!=='function')throw Error('LEVEL_UNAVAILABLE');
      window.OnSearchClick(matches[0].id,'TreeViewArchivo');
    },level)
  ]);
  check(response.status()===200,'LEVEL_SELECTION_FAILED');
  const data=response.request().postData()||'',asyncPost=new URLSearchParams(data).get('__ASYNCPOST')==='true';
  await page.waitForFunction(()=>!window.Sys?.WebForms?.PageRequestManager?.getInstance().get_isInAsyncPostBack());
  await page.waitForFunction(level=>document.getElementById('doc111-importacion')?.getAttribute('data-nivel')===level,level);
  return {asyncPost};
}
async function responseFor(page,operation,action){
  const [response]=await Promise.all([page.waitForResponse(r=>r.request().method()==='POST'&&new URL(r.url()).pathname.endsWith('/'+operation)),action()]);
  let body;try{const raw=await response.json();body=raw?.d??raw;}catch{check(false,'RESPONSE_INVALID');}
  return {status:response.status(),body};
}
async function openWindow(page){
  const result=await responseFor(page,'AbrirImportacion',()=>page.locator('[data-doc111-open]').click());
  check(result.status===200&&result.body?.ok,'OPENING_FAILED');await page.getByRole('dialog').waitFor();
  check(await page.getByRole('dialog').count()===1,'DUPLICATE_DIALOG');return result.body.datos;
}
async function chooseVersion(page,version){
  for(let n=0;n<20;n++){
    if(await page.getByLabel('Perfil publicado').locator('option[value="'+version+'"]').count())return page.getByLabel('Perfil publicado').selectOption(version);
    const more=page.getByRole('button',{name:'Más perfiles',exact:true});check(await more.count()===1,'VERSION_UNAVAILABLE');
    const r=await responseFor(page,'ListarPerfilesImportacion',()=>more.click());check(r.status===200&&r.body?.ok,'CATALOG_FAILED');
  }
  check(false,'VERSION_UNAVAILABLE');
}
async function focusWithinDialog(page){
  return page.getByRole('dialog').evaluate(dialog=>dialog.contains(dialog.ownerDocument.activeElement));
}
function createFixtures(parent,profile){
  const root=fs.mkdtempSync(path.join(parent,'doc113-ui-')),bytes=readFixture(profile);
  const dirs=[path.join(root,'primera',profile.rootNames[0]),path.join(root,'segunda',profile.rootNames[0]),path.join(root,'adicional',profile.rootNames[1])];
  dirs.forEach(dir=>fs.mkdirSync(path.join(dir,'Subcarpeta'),{recursive:true}));
  dirs.forEach(dir=>fs.writeFileSync(path.join(dir,'Subcarpeta',FILE_NAME),bytes));
  return {dirs,close(){check(path.resolve(root).startsWith(path.resolve(parent)+path.sep),'FIXTURE_PATH_INVALID');fs.rmSync(root,{recursive:true,force:true});}};
}
function createUiControls(query,profile){
  const controls=createSelectControls(query);
  async function counts(){
    const totals={};for(const table of ['plan','lote']){
      const rows=await query('RA','SELECT COUNT(*) AS total FROM ra_integracion_'+table+' WHERE nivel_ref=? AND version_id=?',[profile.nivelRef,profile.versionId]);
      check(rows.length===1&&/^\d+$/.test(String(rows[0].total)),'CONTROL_INVALID');totals[table]=BigInt(rows[0].total);
    }return totals;
  }
  return {counts,
    async precheck(){
      const version=await query('RA','SELECT gabinete_id FROM ra_integracion_perfil_version WHERE version_id=?',[profile.versionId]);check(version.length===1&&id(String(version[0].gabinete_id)),'VERSION_CONTROL_INVALID');
      const cabinet=await query('DA','SELECT id_gabinete FROM system1 WHERE id_gabinete=?',[String(version[0].gabinete_id)]);check(cabinet.length===1,'CABINET_CONTROL_INVALID');return counts();
    },
    async plan(planId,confirmed){const rows=await query('RA','SELECT estado,nivel_ref,version_id FROM ra_integracion_plan WHERE plan_id=?',[planId]);check(rows.length===1&&String(rows[0].nivel_ref)===profile.nivelRef&&String(rows[0].version_id)===profile.versionId&&(confirmed?rows[0].estado==='CONFIRMADO':rows[0].estado!=='CONFIRMADO'),'PLAN_CONTROL_INVALID');},
    async noEffects(loteId){
      const view=await controls.snapshot(loteId);
      check(view.attempts.length===0&&view.roots.every(r=>!r.identidad_expediente_doc&&!r.intento_activo_id)&&view.files.every(f=>!f.resultado_doc&&!f.intento_activo_id&&f.estado_recepcion!=='RECIBIDO'),'DOCUMENT_EFFECT_FOUND');
    }
  };
}
async function runUi(page,profile,options,auth,controls,fixtures,onReference=()=>{}){
  validateOptions(profile,options);assertAuthorization(options,auth,profile);
  const before=await controls.precheck(),guard=await guardRequests(page,options,profile);let preparations=0,confirmations=0;
  try{
    await selectLevel(page,profile.nivelRef);await openWindow(page);await chooseVersion(page,profile.versionId);
    if(options.scenario==='UI-01'){
      const destination=await page.locator('.doc111-destino').textContent();await chooseVersion(page,options.alternateVersion);await selectLevel(page,options.alternateLevel);
      check(await page.getByRole('dialog').count()===1&&await page.locator('.doc111-destino').textContent()===destination,'DESTINATION_CHANGED');
      check(guard.calls.filter(c=>['AbrirImportacion','ListarPerfilesImportacion'].includes(c.operation)).every(c=>c.payload.nivelRef===profile.nivelRef),'REQUEST_DESTINATION_CHANGED');
    }else if(options.scenario==='UI-02'){
      await page.getByRole('button',{name:'Agregar carpetas',exact:true}).click();
      for(const dir of fixtures.dirs.slice(0,2))await page.locator('input[webkitdirectory]').setInputFiles(dir);
      const prepare=async()=>{
        const r=await responseFor(page,'PrepararPlan',()=>page.getByRole('button',{name:'Preparar revisión',exact:true}).click());
        check(r.status===200&&r.body?.ok&&id(r.body.datos?.planId),'PREPARATION_FAILED');preparations++;const planId=r.body.datos.planId;onReference({planId});await controls.plan(planId,false);return planId;
      };
      const first=await prepare(),firstRequest=guard.calls.filter(c=>c.operation==='PrepararPlan')[0].payload;
      check(firstRequest.raices.length===2&&firstRequest.raices[0].nombre===firstRequest.raices[1].nombre&&new Set(firstRequest.raices.map(r=>r.idCliente)).size===2&&new Set(firstRequest.archivos.map(f=>f.idCliente)).size===firstRequest.archivos.length&&firstRequest.archivos.every(f=>f.rutaRelativa.includes('/Subcarpeta/')),'INVENTORY_INVALID');
      await page.getByRole('button',{name:'Cambiar selección',exact:true}).click();await page.locator('input[webkitdirectory]').setInputFiles(fixtures.dirs[2]);
      check(await page.getByRole('button',{name:'Confirmar plan',exact:true}).count()===0,'STALE_CONFIRMATION_AVAILABLE');
      const second=await prepare();check(second!==first&&!guard.calls.some(c=>c.operation==='ConfirmarPlan'),'STALE_PLAN_CONFIRMED');await controls.plan(first,false);
      if(options.confirm){
        guard.permitConfirmation(second);
        const r=await responseFor(page,'ConfirmarPlan',()=>page.getByRole('button',{name:'Confirmar plan',exact:true}).click());
        check(r.status===200&&r.body?.ok&&r.body.datos?.planId===second&&id(r.body.datos.loteId),'CONFIRMATION_FAILED');confirmations++;onReference({planId:second,loteId:r.body.datos.loteId});await controls.plan(second,true);await controls.noEffects(r.body.datos.loteId);
      }
    }else{
      const partial=await selectLevel(page,options.alternateLevel);check(partial.asyncPost&&await page.getByRole('dialog').count()===1,'PARTIAL_UPDATE_NOT_VERIFIED');
      await page.getByRole('button',{name:'Cerrar',exact:true}).focus();await page.keyboard.press('Shift+Tab');check(await focusWithinDialog(page),'FOCUS_ESCAPED');
      await page.keyboard.press('Escape');await page.getByRole('dialog').waitFor({state:'detached'});
      check(await page.evaluate(()=>document.activeElement?.hasAttribute('data-doc111-open')),'FOCUS_NOT_RESTORED');
      guard.rejectNextOpening();const r=await responseFor(page,'AbrirImportacion',()=>page.locator('[data-doc111-open]').click());
      check([400,403].includes(r.status)&&r.body?.ok===false,'REAL_ERROR_NOT_VERIFIED');await page.waitForFunction(()=>!document.querySelector('[data-doc111-open]').disabled);
      check(await page.getByRole('dialog').count()===0&&(await page.locator('[data-doc111-status]').textContent()).trim().length>0,'ERROR_NOT_VISIBLE');
      await openWindow(page);
    }
    const after=await controls.counts();check(after.plan-before.plan===BigInt(preparations)&&after.lote-before.lote===BigInt(confirmations),'CONTROL_CHANGED');
    check(guard.violations.length===0,'FORBIDDEN_REQUEST');
    const opens=guard.calls.filter(c=>c.operation==='AbrirImportacion').length;
    check(opens===(options.scenario==='UI-03'?3:1),'DUPLICATE_REQUEST');
    return {scenario:options.scenario,status:'PASS',preparations,confirmations,opens,readOnlyControls:true,noDocumentEffects:true,domVerified:true};
  }finally{await guard.close();}
}
module.exports={CASES,validateOptions,assertAuthorization,allowedOperation,createFixtures,createUiControls,focusWithinDialog,runUi,selectLevel};
