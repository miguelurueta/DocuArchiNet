'use strict';
const {expect}=require('@playwright/test');
const {safePost}=require('./doc110-safe-request.cjs');
const fs=require('node:fs');
const path=require('node:path');
const {sourceFixture}=require('./doc110-source-fixture.cjs');
const {openImportAdministrator}=require('./doc110-admin-navigation.cjs');
const same=(a,b)=>expect(JSON.stringify(a)===JSON.stringify(b)).toBe(true);
const providerRefs=d=>({ProveedorId:d.ProveedorId,VersionId:d.VersionId,Revision:d.Revision,RevisionProveedor:d.RevisionProveedor});

function sourceRow(page,panel,table){
  // Los locators de has se evalúan desde cada fila; no incluir el ancestro dialog.
  return panel.getByRole('row').filter({has:page.getByRole('cell',{name:'DA',exact:true})})
    .filter({has:page.getByRole('cell',{name:table,exact:true})});
}

async function closeImportDialog(page){
  const visible=page.locator('#doc110-perfiles section[role=dialog]:not([hidden])');
  if(await visible.count()){
    await expect(visible).toHaveCount(1);
    await expect(visible).not.toHaveAttribute('aria-busy','true');
    await visible.getByRole('button',{name:'Cerrar',exact:true}).click();
  }
  await expect(visible).toHaveCount(0);
}

function validResume(source,history,fixture,previous){
  if(previous?.status!=='FAIL'||previous.code!=='CHECK_FAILED'||previous.sourceTable!==fixture.table)return false;
  if(!source||source.ConexionRef!=='DA'||source.Tabla!==fixture.table||source.Activo||source.Revision!==1||
    source.ModoAislamiento!=='CONEXION'||source.ColumnaEmpresa||
    JSON.stringify((source.Columnas||[]).slice().sort())!==JSON.stringify([fixture.keyColumn,fixture.removedColumn].sort()))return false;
  if(previous.sourceId&&previous.sourceId!==source.FuenteId)return false;
  if(!Array.isArray(history)||history.length!==1)return false;
  const entry=history[0];
  if(entry.Accion!=='ALTA'||entry.Revision!==1||entry.Motivo!=='Autorizar fuente descartable DOC-110'||!entry.ActorRef||
    JSON.stringify(entry.Estado)!==JSON.stringify(source))return false;
  const netDate=typeof entry.FechaUtc==='string'&&entry.FechaUtc.match(/^\/Date\((-?\d+)(?:[+-]\d{4})?\)\/$/);
  const timestamp=netDate?Number(netDate[1]):Date.parse(entry.FechaUtc);
  const start=Date.parse(previous.startedUtc),end=Date.parse(previous.finishedUtc);
  return Number.isFinite(timestamp)&&Number.isFinite(start)&&Number.isFinite(end)&&end>=start&&
    timestamp>=start-1000&&timestamp<=end+1000;
}

function verifiedTestSource(source,history,fixture,origin,sourceId){
  const ordered=Array.isArray(history)?history.slice().sort((a,b)=>a.Revision-b.Revision):[];
  const alta=ordered[0];
  const motives=[['Autorizar fuente descartable DOC-110'],['Activar fuente descartable DOC-110'],
    ['Limitar fuente descartable a CONSECUTIVO'],['Revocar fuente descartable DOC-110','Finalizar prueba DOC-110 de fuente descartable'],
    ['Activar fuente descartable DOC-110'],['Revocar fuente descartable DOC-110','Finalizar prueba DOC-110 de fuente descartable']];
  return Boolean(source&&source.FuenteId===sourceId&&source.Revision>=1&&source.Revision<=6&&ordered.length===source.Revision&&
    validResume(alta?.Estado,[alta],fixture,{...origin,sourceId})&&
    JSON.stringify(ordered.at(-1).Estado)===JSON.stringify(source)&&
    ordered.every((entry,index)=>{
      const state=entry.Estado,columns=index<2?[fixture.keyColumn,fixture.removedColumn]:[fixture.keyColumn];
      return entry.Revision===index+1&&entry.Accion===['ALTA','ACTIVAR','EDITAR','REVOCAR','ACTIVAR','REVOCAR'][index]&&
        entry.ActorRef===alta.ActorRef&&motives[index].includes(entry.Motivo)&&
        state.FuenteId===sourceId&&state.Revision===entry.Revision&&state.ConexionRef==='DA'&&state.Tabla===fixture.table&&
        state.ModoAislamiento==='CONEXION'&&!state.ColumnaEmpresa&&state.Activo===(index===1||index===2||index===4)&&
        JSON.stringify(state.Columnas.slice().sort())===JSON.stringify(columns.slice().sort());
    }));
}

async function cleanupTestSource(call,fixture,origin,sourceId){
  if(!/^[1-9][0-9]*$/.test(sourceId||''))throw new Error('DOC110_SOURCE_CLEANUP_ID_REQUIRED');
  const identity={FuenteId:sourceId};
  const source=(await call('ObtenerFuente',identity)).datos;
  const history=(await call('HistorialFuente',{...identity,Pagina:1})).datos;
  if(!verifiedTestSource(source,history,fixture,origin,sourceId))
    throw new Error('DOC110_SOURCE_CLEANUP_NOT_VERIFIED: historial o estado difieren del fixture; sin escrituras.');
  if(source.Activo){
    const revoked=(await call('RevocarFuente',{Cambio:{Documento:source,Motivo:'Finalizar prueba DOC-110 de fuente descartable'}})).datos;
    expect(!revoked.Activo&&revoked.FuenteId===sourceId&&revoked.Revision===source.Revision+1).toBe(true);
  }
  expect((await call('ObtenerFuente',identity)).datos.Activo).toBe(false);
  console.log('DOC110_PASS: VERIFIED_FAILED_RUN_SOURCE_RECOVERED_INACTIVE');
}

function resumeEvidence(report){
  const previous=JSON.parse(fs.readFileSync(report,'utf8'));
  let origin=previous.resumeOrigin||previous;
  if(!origin.sourceId&&previous.sourceId)origin={...origin,sourceId:previous.sourceId};
  return {previous,origin};
}

async function runSourceProfile(call,ui,providerLifecycle,fixture=sourceFixture(),options={}){
  // Recorrer también fuentes inactivas: la unicidad no depende de disponibilidad.
  let exhausted=false,existing=null;
  for(let page=1;page<=100;page++){
    const items=(await call('ListarFuentes',{Pagina:page,Termino:fixture.table})).datos;
    expect(Array.isArray(items)&&items.length<=25).toBe(true);
    for(const item of items.filter(d=>d.ConexionRef==='DA'&&d.Tabla===fixture.table)){
      if(!options.resume)
        throw new Error('DOC110_SOURCE_ALREADY_REGISTERED: la fuente DA ya existe. No se modifica ni revoca. Prepare un fixture aislado autorizado.');
      if(existing)throw new Error('DOC110_SOURCE_RESUME_NOT_VERIFIED');
      existing=item;
    }
    if(items.length<25){exhausted=true;break;}
  }
  if(!exhausted)throw new Error('DOC110_SOURCE_PREFLIGHT_PAGE_LIMIT: no se comprobo ausencia; sin escrituras.');
  let resumed,recoveredHistory;
  if(options.resume){
    if(!existing)throw new Error('DOC110_SOURCE_RESUME_NOT_FOUND: no se crea otra fuente en modo reanudacion.');
    resumed=(await call('ObtenerFuente',{FuenteId:existing.FuenteId})).datos;
    const history=(await call('HistorialFuente',{FuenteId:existing.FuenteId,Pagina:1})).datos;
    let verified=validResume(resumed,history,fixture,options.resume);
    if(options.recovered){
      const recovery=options.recovered;
      const start=Date.parse(recovery.startedUtc),end=Date.parse(recovery.finishedUtc);
      verified=recovery.status==='PASS'&&recovery.sourceTable===fixture.table&&Number.isFinite(start)&&Number.isFinite(end)&&
        start>=Date.parse(options.resume.finishedUtc)&&end>=start&&resumed.Revision===4&&!resumed.Activo&&
        verifiedTestSource(resumed,history,fixture,options.resume,options.resume.sourceId);
      if(verified)recoveredHistory=JSON.parse(JSON.stringify(history));
    }
    if(!verified)
      throw new Error('DOC110_SOURCE_RESUME_NOT_VERIFIED: fuente o historial no coinciden con el alta de la corrida fallida; sin escrituras.');
  }
  const columns=(await call('ConsultarMetadatosFuente',{tipo:'COLUMNAS',conexionRef:'DA',tabla:fixture.table,pagina:1})).datos;
  expect(columns.some(c=>c.value===fixture.keyColumn&&c.Tipo==='TEXTO')).toBe(true);
  expect(columns.some(c=>c.value===fixture.removedColumn&&c.Tipo===fixture.removedType)).toBe(true);
  console.log('DOC110_PASS: '+(resumed?'SOURCE_FAILED_RUN_IDENTITY_AND_AUDIT_VERIFIED':'SOURCE_NEW_IDENTITY_AND_METADATA_PREFLIGHT'));
  const created=resumed||await ui.create();
  expect(created.FuenteId).toMatch(/^[1-9][0-9]*$/);
  expect(created.ConexionRef==='DA'&&created.Tabla===fixture.table&&!created.Activo&&created.Revision===(recoveredHistory?4:1)).toBe(true);
  const identity={FuenteId:created.FuenteId};
  if(options.onSourceReady)options.onSourceReady(created.FuenteId);
  try{
    let source=await ui.activate(created);
    expect(source.Activo&&source.Revision===created.Revision+1).toBe(true);expect(source.FuenteId===created.FuenteId).toBe(true);
    if(!recoveredHistory){
      source=await ui.edit(source);
      expect(source.Activo&&source.Revision===3).toBe(true);expect(source.FuenteId===created.FuenteId).toBe(true);
    }else console.log('DOC110_PASS: SOURCE_PREVIOUS_UI_EDIT_VERIFIED_FROM_AUDIT');
    const ordinary=(await call('ConsultarMetadatosProveedor',{tipo:'COLUMNAS',conexionRef:'DA',tabla:fixture.table,pagina:1})).datos;
    expect(ordinary.some(c=>c.value===fixture.keyColumn)).toBe(true);
    expect(ordinary.some(c=>c.value===fixture.removedColumn)).toBe(false);
    console.log('DOC110_PASS: SOURCE_UI_EDIT_REMOVES_COLUMN_FROM_PROVIDER_METADATA');
    const consumers=await providerLifecycle();
    expect(consumers?.profileSnapshot?.documents?.length).toBe(2);
    const beforeProvider=(await call('ObtenerAdministracionProveedor',providerRefs(consumers.provider))).datos;
    const revoked=await ui.revoke(source);
    expect(!revoked.Activo&&revoked.Revision===source.Revision+1).toBe(true);expect(revoked.FuenteId===created.FuenteId).toBe(true);
    const denied=await call('ValidarProveedor',providerRefs(beforeProvider),403);
    expect(denied.codigo).toBe('ACCESO_DENEGADO');expect(denied.datos==null).toBe(true);
    same((await call('ObtenerAdministracionProveedor',providerRefs(consumers.provider))).datos,beforeProvider);
    for(const before of consumers.profileSnapshot.documents)
      same((await call('ObtenerPerfil',{perfilId:before.PerfilId,versionId:before.Version.VersionId})).datos,before);
    same((await call('ConsultarHistorial',{perfilId:consumers.profileSnapshot.documents[0].PerfilId,pagina:1,tamanoPagina:25})).datos,consumers.profileSnapshot.history);
    const history=(await call('HistorialFuente',{...identity,Pagina:1})).datos;
    expect(history.map(h=>h.Revision)).toEqual(recoveredHistory?[6,5,4,3,2,1]:[4,3,2,1]);
    expect(history.map(h=>h.Accion)).toEqual(recoveredHistory?['REVOCAR','ACTIVAR','REVOCAR','EDITAR','ACTIVAR','ALTA']:['REVOCAR','EDITAR','ACTIVAR','ALTA']);
    if(recoveredHistory)same(history.slice(2),recoveredHistory);
    for(const entry of history){
      expect(entry.Estado.FuenteId===created.FuenteId&&entry.Estado.Revision===entry.Revision).toBe(true);
      expect(Boolean(entry.ActorRef&&entry.FechaUtc&&entry.Motivo)).toBe(true);
    }
    expect(history[0].Estado.Activo).toBe(false);
    console.log('DOC110_PASS: SOURCE_UI_REVOCATION_DENIES_USE_PRESERVES_CONSUMERS_AND_AUDIT');
  }finally{
    // Reconciliar tras cualquier error sin repetir mutaciones de resultado incierto.
    const latest=(await call('ObtenerFuente',identity)).datos;
    if(latest.Activo)
      await call('RevocarFuente',{Cambio:{Documento:latest,Motivo:'Finalizar prueba DOC-110 de fuente descartable'}});
    expect((await call('ObtenerFuente',identity)).datos.Activo).toBe(false);
    console.log('DOC110_PASS: SOURCE_OWNED_BY_TEST_FINALLY_INACTIVE');
  }
}

async function exerciseSourceProfile(auth,baseUrl,token){
  if(process.env.DOC110_E2E_SOURCE_CATALOG_AUTHORIZED!=='true')
    throw new Error('DOC110_SOURCE_AUTHORIZATION_REQUIRED');
  const fixture=sourceFixture(process.env.DOC110_E2E_SOURCE_TABLE);
  const page=auth.page,panel=page.getByRole('dialog',{name:'Administrar fuentes',exact:true});
  const f=id=>panel.locator('[data-fuente='+id+']');
  async function body(response,status=200){
    expect(response.status()).toBe(status);
    let result;try{result=(await response.json()).d;}catch{throw new Error('DOC110_SOURCE_NON_JSON');}
    expect(result?.ok).toBe(status===200);return result;
  }
  const call=async(method,payload,status=200)=>body(await safePost(auth.context.request,
    new URL('webservice/WebServiceImportacionCarpetas.asmx/'+method,baseUrl).href,
    {headers:{'X-Importacion-Perfil-CSRF':token},data:{solicitud:payload}},method),status);
  if(process.env.DOC110_E2E_SOURCE_CLEANUP==='YES'){
    const recovery={status:'RUNNING',sourceTable:fixture.table,startedUtc:new Date().toISOString()};
    try{
      const {origin}=resumeEvidence(path.resolve(__dirname,'../../artifacts/doc110-source-profile-result.json'));
      await cleanupTestSource(call,fixture,origin,origin.sourceId);recovery.status='PASS';
    }catch(error){recovery.status='FAIL';throw error;}
    finally{
      recovery.finishedUtc=new Date().toISOString();
      fs.writeFileSync(path.resolve(__dirname,'../../artifacts/doc110-source-cleanup-result.json'),JSON.stringify(recovery,null,2));
    }
    return;
  }
  async function click(id,method){
    const pending=page.waitForResponse(r=>r.url().endsWith('/'+method)&&r.request().method()==='POST');
    await f(id).click();return (await body(await pending)).datos;
  }
  async function open(){
    await body(await openImportAdministrator(page,'admin-fuentes','ListarFuentes'));
  }
  async function detail(){
    await open();
    // La fuente creada es la última; no abrir otra autorización por nombre parcial.
    const row=sourceRow(page,panel,fixture.table);
    await expect(row).toHaveCount(1);
    const pending=page.waitForResponse(r=>r.url().endsWith('/ObtenerFuente'));
    await row.getByRole('button',{name:'Abrir',exact:true}).click();await body(await pending);
    await expect(panel.getByText('Opciones disponibles.',{exact:true}).last()).toBeVisible();
  }
  const ui={
    async create(){
      await open();await f('nueva').click();await f('conexion').selectOption('DA');
      for(let pageIndex=0;pageIndex<100;pageIndex++){
        await expect(panel.getByText('Opciones disponibles.',{exact:true}).last()).toBeVisible();
        if(await f('tabla').locator('option[value="'+fixture.table+'"]').count())break;
        await panel.getByRole('button',{name:'Más tablas',exact:true}).click();
      }
      await f('tabla').selectOption(fixture.table);
      for(const column of [fixture.keyColumn,fixture.removedColumn])await panel.getByLabel('Permitir '+column,{exact:true}).check();
      await expect(f('modo')).toHaveValue('CONEXION');await expect(f('empresa')).toBeDisabled();
      await f('motivo').fill('Autorizar fuente descartable DOC-110');return click('guardar','CrearFuente');
    },
    async activate(){await detail();await f('motivo').fill('Activar fuente descartable DOC-110');return click('disponibilidad','ActivarFuente');},
    async edit(){await detail();await panel.getByLabel('Permitir '+fixture.removedColumn,{exact:true}).uncheck();await f('motivo').fill('Limitar fuente descartable a CONSECUTIVO');return click('guardar','GuardarFuente');},
    async revoke(){await detail();await f('motivo').fill('Revocar fuente descartable DOC-110');return click('disponibilidad','RevocarFuente');}
  };
  const report=path.resolve(__dirname,'../../artifacts/doc110-source-profile-result.json');
  const evidence={status:'RUNNING',sourceTable:fixture.table,startedUtc:new Date().toISOString()};
  let resume,recovered;
  if(process.env.DOC110_E2E_SOURCE_RESUME==='YES'||process.env.DOC110_E2E_SOURCE_CONTINUE==='YES'){
    try{
      resume=resumeEvidence(report).origin;
    }
    catch{throw new Error('DOC110_SOURCE_RESUME_EVIDENCE_REQUIRED');}
    evidence.resumeOrigin={status:resume.status,code:resume.code,sourceTable:resume.sourceTable,sourceId:resume.sourceId,
      startedUtc:resume.startedUtc,finishedUtc:resume.finishedUtc};
    if(process.env.DOC110_E2E_SOURCE_CONTINUE==='YES'){
      try{recovered=JSON.parse(fs.readFileSync(path.resolve(__dirname,'../../artifacts/doc110-source-cleanup-result.json'),'utf8'));}
      catch{throw new Error('DOC110_SOURCE_RECOVERY_REPORT_REQUIRED');}
      evidence.recovery={status:recovered.status,startedUtc:recovered.startedUtc,finishedUtc:recovered.finishedUtc};
    }
  }
  try{
    await runSourceProfile(call,ui,async()=>{
      return require('./doc110-provider-lifecycle.cjs').exerciseProviderLifecycle(auth,baseUrl,token,fixture);
    },fixture,{resume,recovered,onSourceReady(id){evidence.sourceId=id;evidence.sourceOrigin=recovered?'CONTINUED_AFTER_RECOVERY':resume?'RESUMED_FAILED_RUN':'CREATED_THIS_RUN';}});
    evidence.status='PASS';
    const ambiguousKey=fixture.ambiguousKey||process.env.DOC110_E2E_AMBIGUOUS_KEY;
    evidence.ambiguityStatus=ambiguousKey&&ambiguousKey!=='NO_APLICA'?'EJECUTADO_OK':'NO_EJECUTADO';
  }catch(error){
    const code=String(error.message||'').match(/^DOC110_[A-Z_]+/);
    evidence.code=code?code[0]:'CHECK_FAILED';
    evidence.status=['DOC110_SOURCE_ALREADY_REGISTERED','DOC110_SOURCE_PREFLIGHT_PAGE_LIMIT','DOC110_SOURCE_RESUME_NOT_VERIFIED','DOC110_SOURCE_RESUME_NOT_FOUND'].includes(evidence.code)?'BLOCKED':'FAIL';
    throw error;
  }finally{
    evidence.finishedUtc=new Date().toISOString();
    fs.mkdirSync(path.dirname(report),{recursive:true});fs.writeFileSync(report,JSON.stringify(evidence,null,2));
  }
}
module.exports={runSourceProfile,exerciseSourceProfile,closeImportDialog,validResume,sourceRow,cleanupTestSource};
