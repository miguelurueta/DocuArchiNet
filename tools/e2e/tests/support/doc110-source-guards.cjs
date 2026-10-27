'use strict';
const {expect}=require('@playwright/test');
const fs=require('node:fs'),path=require('node:path');
const {safePost}=require('./doc110-safe-request.cjs');
const {sourceFixture}=require('./doc110-source-fixture.cjs');

async function verifySourceGuards(post,read,current){
  expect(current.Activo).toBe(false);expect(current.Revision).toBeGreaterThan(1);
  const baseline=await read();
  expect(JSON.stringify(baseline.source)===JSON.stringify(current)).toBe(true);
  const payload={Cambio:{Documento:{...current,Columnas:current.Columnas.slice(),Revision:current.Revision-1},Motivo:'Verificar rechazo DOC-110 sin cambios'}};
  let checks=0;
  async function reject(mode,method,data,status,code,label){
    const response=await post(mode,method,data);
    expect(response.status).toBe(status);expect(response.result?.ok).toBe(false);
    expect(response.result?.codigo).toBe(code);expect(response.result?.datos==null).toBe(true);
    expect(JSON.stringify(await read())===JSON.stringify(baseline)).toBe(true);
    checks++;console.log('DOC110_PASS: SOURCE_'+label+'_'+method.toUpperCase()+'_DOCUMENT_AND_HISTORY_UNCHANGED');
  }
  for(const method of ['CrearFuente','GuardarFuente','ActivarFuente','RevocarFuente']){
    await reject('MISSING',method,payload,403,'SOLICITUD_NO_VERIFICADA','MISSING_CSRF');
    await reject('INVALID',method,payload,403,'SOLICITUD_NO_VERIFICADA','INVALID_CSRF');
    await reject('ANONYMOUS',method,payload,401,'SESION_REQUERIDA','NO_SESSION');
    // Crear conserva FuenteId: su control positivo de transporte rechaza la forma de alta,
    // sin intentar duplicar una identidad. Las otras operaciones usan revisión antigua.
    await reject('TRUSTED',method,payload,method==='CrearFuente'?400:409,
      method==='CrearFuente'?'SOLICITUD_INVALIDA':'CONFLICTO_REVISION','INVALID_OR_STALE_REQUEST');
  }
  await reject('TRUSTED','GuardarFuente',{...payload,UsuarioRef:'0',AmbitoRef:'0'},400,'CONFIGURACION_INVALIDA','CLIENT_AUTHORITY_REJECTED');
  return {checks};
}

async function exerciseSourceGuards(auth,baseUrl,token){
  const fixture=sourceFixture(process.env.DOC110_E2E_SOURCE_TABLE);
  const completed=JSON.parse(fs.readFileSync(path.resolve(__dirname,'../../artifacts/doc110-source-profile-result.json'),'utf8'));
  if(completed.status!=='PASS'||completed.sourceTable!==fixture.table||!/^[1-9][0-9]*$/.test(completed.sourceId||''))
    throw new Error('DOC110_SOURCE_GUARDS_COMPLETED_FIXTURE_REQUIRED');
  const anonymous=await auth.context.browser().newContext({ignoreHTTPSErrors:process.env.DOC10_E2E_IGNORE_HTTPS_ERRORS==='true'});
  const evidence={status:'RUNNING',startedUtc:new Date().toISOString()};
  async function post(mode,method,data){
    const context=mode==='ANONYMOUS'?anonymous:auth.context;
    const headers=mode==='MISSING'?{}:{'X-Importacion-Perfil-CSRF':mode==='INVALID'?'doc110-invalid-token':token};
    const response=await safePost(context.request,new URL('webservice/WebServiceImportacionCarpetas.asmx/'+method,baseUrl).href,
      {headers,data:{solicitud:data}},method);
    let result;try{result=(await response.json()).d;}catch{throw new Error('DOC110_SOURCE_GUARDS_NON_JSON');}
    return {status:response.status(),result};
  }
  async function read(){
    const identity={FuenteId:completed.sourceId};
    const source=await post('TRUSTED','ObtenerFuente',identity);
    const history=await post('TRUSTED','HistorialFuente',{...identity,Pagina:1});
    for(const response of [source,history]){expect(response.status).toBe(200);expect(response.result?.ok).toBe(true);}
    return {source:source.result.datos,history:history.result.datos};
  }
  try{
    const before=await read();
    expect(before.source.ConexionRef==='DA'&&before.source.Tabla===fixture.table).toBe(true);
    const result=await verifySourceGuards(post,read,before.source);
    evidence.checks=result.checks;evidence.status='PASS';
  }catch(error){evidence.status='FAIL';throw error;}
  finally{
    await anonymous.close();evidence.finishedUtc=new Date().toISOString();
    fs.writeFileSync(path.resolve(__dirname,'../../artifacts/doc110-source-guards-result.json'),JSON.stringify(evidence,null,2));
  }
}
module.exports={verifySourceGuards,exerciseSourceGuards};
