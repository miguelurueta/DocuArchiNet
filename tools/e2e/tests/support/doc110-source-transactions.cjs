'use strict';
const {expect}=require('@playwright/test');
const copy=value=>JSON.parse(JSON.stringify(value));
const same=(a,b)=>expect(JSON.stringify(a)===JSON.stringify(b)).toBe(true);

// Verificador preparado para un fixture NUEVO, creado por su runner autorizado.
// No autentica, crea tablas, activa fuentes ni introduce fallos en MySQL.
async function verifySourceTransactions(peers,current){
  if(peers.length!==2||current.Activo||current.Revision!==1||current.ConexionRef!=='DA'||
    !/^imp_02_[A-Za-z0-9_]+$/.test(current.Tabla)||
    ['imp_02_creditos','imp_02_egresos'].includes(current.Tabla)||
    current.ModoAislamiento!=='CONEXION'||current.ColumnaEmpresa||
    current.Columnas?.length!==2||new Set(current.Columnas).size!==2)
    throw new Error('DOC110_SOURCE_TRANSACTIONS_NEW_INACTIVE_FIXTURE_REQUIRED');
  const identity={FuenteId:current.FuenteId};
  async function ok(peer,method,data){
    const response=await peer(method,data);
    expect(response.status).toBe(200);expect(response.result?.ok).toBe(true);
    return response.result.datos;
  }
  async function state(peer){
    return {source:await ok(peer,'ObtenerFuente',identity),
      history:await ok(peer,'HistorialFuente',{...identity,Pagina:1})};
  }
  const before=await state(peers[0]);same(before.source,current);
  expect(before.history.length).toBe(1);expect(before.history[0].Accion).toBe('ALTA');
  same(before.history[0].Estado,current);same(await state(peers[1]),before);
  async function rejected(response){
    expect(response.status).toBe(409);expect(response.result?.ok).toBe(false);
    expect(response.result?.codigo).toBe('CONFLICTO_REVISION');expect(response.result?.datos==null).toBe(true);
  }
  const duplicate={Cambio:{Documento:{...copy(current),FuenteId:null,Revision:0},Motivo:'Verificar duplicado descartable DOC-110'}};
  await rejected(await peers[0]('CrearFuente',duplicate));
  same(await state(peers[0]),before);
  async function uniqueIdentity(peer){
    let count=0,exhausted=false;
    for(let page=1;page<=100;page++){
      const items=await ok(peer,'ListarFuentes',{Pagina:page,Termino:current.Tabla});
      expect(Array.isArray(items)&&items.length<=25).toBe(true);
      count+=items.filter(d=>d.ConexionRef==='DA'&&d.Tabla===current.Tabla).length;
      if(items.length<25){exhausted=true;break;}
    }
    expect(exhausted).toBe(true);expect(count).toBe(1);
  }
  await uniqueIdentity(peers[0]);
  console.log('DOC110_PASS: SOURCE_DUPLICATE_REJECTED_SINGLE_IDENTITY_HISTORY_UNCHANGED');
  const proposals=current.Columnas.map(column=>({Cambio:{Documento:{...copy(current),Columnas:[column]},Motivo:'Carrera de columnas descartables DOC-110'}}));
  // Esperar ambas respuestas antes de leer, incluso si una falla por transporte.
  const settled=await Promise.allSettled(peers.map((peer,i)=>peer('GuardarFuente',proposals[i])));
  if(settled.some(r=>r.status==='rejected'))throw new Error('DOC110_SOURCE_TRANSACTION_OUTCOME_UNCONFIRMED');
  const results=settled.map(r=>r.value);
  expect(results.map(r=>r.status).sort()).toEqual([200,409]);
  const winner=results.findIndex(r=>r.status===200),loser=1-winner;
  expect(results[winner].result?.ok).toBe(true);await rejected(results[loser]);
  const expected={...copy(current),Revision:2,Columnas:proposals[winner].Cambio.Documento.Columnas};
  same(results[winner].result.datos,expected);
  const after=await state(peers[0]);same(after.source,expected);same(await state(peers[1]),after);
  expect(after.history.length).toBe(2);
  const ordered=after.history.slice().sort((a,b)=>a.Revision-b.Revision);
  same(ordered[0],before.history[0]);expect(ordered[1].Revision).toBe(2);expect(ordered[1].Accion).toBe('EDITAR');
  same(ordered[1].Estado,expected);
  await rejected(await peers[loser]('GuardarFuente',proposals[loser]));
  same(await state(peers[0]),after);await uniqueIdentity(peers[1]);
  console.log('DOC110_PASS: SOURCE_CONCURRENT_EDIT_ONE_WINNER_COLUMNS_AND_HISTORY_ATOMIC_INACTIVE');
  return {checks:2,source:expected};
}
const transactionFixture={table:'imp_02_facturas',columns:['NIT','NUMERO']};
async function prepareSourceTransactionFixture(call,create){
  const fixture=transactionFixture;
  let exhausted=false;
  for(let page=1;page<=100;page++){
    const response=await call('ListarFuentes',{Pagina:page,Termino:fixture.table});
    expect(response.status).toBe(200);expect(response.result?.ok).toBe(true);
    const items=response.result.datos;expect(Array.isArray(items)&&items.length<=25).toBe(true);
    if(items.some(d=>d.ConexionRef==='DA'&&d.Tabla===fixture.table))
      throw new Error('DOC110_SOURCE_ALREADY_REGISTERED: facturas ya existe; no se reutiliza ni modifica.');
    if(items.length<25){exhausted=true;break;}
  }
  if(!exhausted)throw new Error('DOC110_SOURCE_PREFLIGHT_PAGE_LIMIT');
  const metadata=await call('ConsultarMetadatosFuente',{tipo:'COLUMNAS',conexionRef:'DA',tabla:fixture.table,pagina:1});
  expect(metadata.status).toBe(200);expect(metadata.result?.ok).toBe(true);
  for(const name of fixture.columns)expect(metadata.result.datos.some(c=>c.value===name&&c.Tipo==='TEXTO')).toBe(true);
  console.log('DOC110_PASS: SOURCE_FACTURAS_ABSENT_AND_METADATA_VERIFIED');
  const current=await create();
  expect(Boolean(/^[1-9][0-9]*$/.test(current.FuenteId)&&current.Tabla===fixture.table&&current.ConexionRef==='DA'&&
    current.Revision===1&&!current.Activo&&current.ModoAislamiento==='CONEXION'&&!current.ColumnaEmpresa)).toBe(true);
  same(current.Columnas.slice().sort(),fixture.columns);
  return current;
}

async function createFacturasSourceUI(page,onAttempt=()=>{},onCreated=()=>{}){
  const {openImportAdministrator}=require('./doc110-admin-navigation.cjs');
  const panel=page.getByRole('dialog',{name:'Administrar fuentes',exact:true});
  const f=id=>panel.locator('[data-fuente='+id+']');
  const response=await openImportAdministrator(page,'admin-fuentes','ListarFuentes');
  expect(response.status()).toBe(200);expect((await response.json()).d?.ok).toBe(true);
  await f('nueva').click();await f('conexion').selectOption('DA');
  let found=false;
  for(let i=0;i<100;i++){
    await expect(panel.getByText(/^(Opciones disponibles\.|Sin opciones disponibles\.)$/).last()).toBeVisible();
    if(await f('tabla').locator('option[value="imp_02_facturas"]').count()){found=true;break;}
    const more=panel.getByRole('button',{name:'Más tablas',exact:true});
    if(!await more.count())break;
    await more.click();
  }
  if(!found)throw new Error('DOC110_SOURCE_FACTURAS_NOT_DISCOVERED');
  await f('tabla').selectOption(transactionFixture.table);
  for(const column of transactionFixture.columns)await panel.getByLabel('Permitir '+column,{exact:true}).check();
  await expect(f('modo')).toHaveValue('CONEXION');await expect(f('empresa')).toBeDisabled();
  await f('motivo').fill('Autorizar fuente descartable transacciones DOC-110');
  onAttempt();
  const pending=page.waitForResponse(r=>r.url().endsWith('/CrearFuente')&&r.request().method()==='POST',{timeout:15000});
  const [created]=await Promise.all([pending,f('guardar').click()]);
  expect(created.status()).toBe(200);
  const result=(await created.json()).d;expect(result?.ok).toBe(true);
  onCreated(result.datos.FuenteId);
  return result.datos;
}

async function exerciseSourceTransactions(auth,baseUrl,token){
  if(process.env.DOC110_E2E_SOURCE_TRANSACTIONS_AUTHORIZED!=='true')
    throw new Error('DOC110_SOURCE_TRANSACTIONS_AUTHORIZATION_REQUIRED');
  const fs=require('node:fs'),path=require('node:path');
  const {safePost}=require('./doc110-safe-request.cjs');
  const {openImportAdministrator}=require('./doc110-admin-navigation.cjs');
  const {createWorkflowLoginPageSession}=require('./authenticated-workflow-session.cjs');
  const evidence={status:'RUNNING',sourceTable:transactionFixture.table,startedUtc:new Date().toISOString()};
  const report=path.resolve(__dirname,'../../artifacts/doc110-source-transactions-result.json');
  const save=()=>{fs.mkdirSync(path.dirname(report),{recursive:true});fs.writeFileSync(report,JSON.stringify(evidence,null,2));};
  function peer(context,csrf){return async(method,data)=>{
    const response=await safePost(context.request,new URL('webservice/WebServiceImportacionCarpetas.asmx/'+method,baseUrl).href,
      {headers:{'X-Importacion-Perfil-CSRF':csrf},data:{solicitud:data}},method);
    let result;try{result=(await response.json()).d;}catch{throw new Error('DOC110_SOURCE_TRANSACTIONS_NON_JSON');}
    return {status:response.status(),result};
  };}
  const first=peer(auth.context,token);
  let contender;
  save();
  try{
    contender=await createWorkflowLoginPageSession(auth.context.browser(),{
      baseUrl,moduleEnvironmentVariable:'DOC110_E2E_MODULE',userEnvironmentVariable:'DOC110_E2E_USER',
      passwordEnvironmentVariable:'DOC110_E2E_PASSWORD',ignoreHTTPSErrors:process.env.DOC10_E2E_IGNORE_HTTPS_ERRORS==='true'
    });
    await contender.page.goto(new URL('Gestion/WebFormProducionDocumental.aspx',baseUrl).href);
    const opened=await openImportAdministrator(contender.page,'abrir','ListarPerfiles');
    expect(opened.status()).toBe(200);expect((await opened.json()).d?.ok).toBe(true);
    const otherToken=opened.request().headers()['x-importacion-perfil-csrf'];
    if(!otherToken)throw new Error('DOC110_CSRF_NOT_OBSERVED');
    const session=async context=>(await context.cookies(baseUrl)).find(c=>c.name==='ASP.NET_SessionId')?.value;
    const firstId=await session(auth.context),secondId=await session(contender.context);
    expect(Boolean(firstId&&secondId&&firstId!==secondId)).toBe(true);
    console.log('DOC110_PASS: SOURCE_INDEPENDENT_ASPNET_SESSIONS');
    const second=peer(contender.context,otherToken);
    const probe=await second('ListarFuentes',{Pagina:1,Termino:transactionFixture.table});
    expect(probe.status).toBe(200);expect(probe.result?.ok).toBe(true);
    const current=await prepareSourceTransactionFixture(first,()=>createFacturasSourceUI(auth.page,
      ()=>{evidence.creationAttempted=true;save();},id=>{evidence.sourceId=id;save();}));
    const result=await verifySourceTransactions([first,second],current);
    evidence.checks=result.checks;evidence.status='PASS';
  }catch(error){
    const code=String(error.message||'').match(/^DOC110_[A-Z_]+/);
    evidence.code=code?code[0]:'CHECK_FAILED';evidence.status='FAIL';throw error;
  }finally{
    let unconfirmed=false;
    if(evidence.sourceId){
      try{
        const final=await first('ObtenerFuente',{FuenteId:evidence.sourceId});
        expect(final.status).toBe(200);expect(final.result?.ok).toBe(true);
        expect(final.result.datos.Activo).toBe(false);
        evidence.finalState='INACTIVE';console.log('DOC110_PASS: SOURCE_FACTURAS_FINALLY_INACTIVE_WITHOUT_ACTIVATION');
      }catch{evidence.finalState='UNCONFIRMED';evidence.status='FAIL';unconfirmed=true;}
    }
    evidence.finishedUtc=new Date().toISOString();save();
    if(contender)await contender.context.close();
    if(unconfirmed)throw new Error('DOC110_SOURCE_FINAL_STATE_UNCONFIRMED');
  }
}
module.exports={verifySourceTransactions,prepareSourceTransactionFixture,createFacturasSourceUI,exerciseSourceTransactions};
