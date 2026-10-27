'use strict';
const {expect}=require('@playwright/test');
const same=(a,b)=>expect(JSON.stringify(a)===JSON.stringify(b)).toBe(true);

function sessionIdleMinutes(config){
  const tag=config.match(/<sessionState\b[^>]*>/i)?.[0];
  const value=tag?.match(/\btimeout\s*=\s*["'](\d+)["']/i)?.[1];
  const minutes=Number(value);
  if(!Number.isInteger(minutes)||minutes<1||minutes>60)throw new Error('DOC110_SESSION_TIMEOUT_CONFIGURATION_REQUIRED');
  return minutes;
}

async function verifySessionExpiry({control,expiring,foreignControl,beforeIdle,waitIdle,profileId}){
  async function ok(peer,method,data){
    const response=await peer(method,data);expect(response.status).toBe(200);expect(response.result?.ok).toBe(true);
    return response.result.datos;
  }
  const refs={perfilId:profileId};
  async function snapshot(){return {profile:await ok(control,'ObtenerPerfil',refs),
    history:await ok(control,'ConsultarHistorial',{...refs,pagina:1,tamanoPagina:25})};}
  const before=await snapshot();
  // ID de versión inválido: ni siquiera un fallo del guard debe provocar una escritura válida.
  const invalidWrite={perfilId:profileId,versionId:'0',revisionVersionEsperada:0,configuracion:null,reglas:[]};
  const list={pagina:1,tamanoPagina:1};
  async function reject(peer,method,data,status,code){
    const response=await peer(method,data);expect(response.status).toBe(status);
    expect(response.result?.ok).toBe(false);expect(response.result?.codigo).toBe(code);expect(response.result?.datos==null).toBe(true);
  }
  await ok(expiring,'ListarPerfiles',list);
  await reject(foreignControl,'ListarPerfiles',list,403,'SOLICITUD_NO_VERIFICADA');
  await reject(foreignControl,'GuardarBorrador',invalidWrite,403,'SOLICITUD_NO_VERIFICADA');
  same(await snapshot(),before);
  console.log('DOC110_PASS: OTHER_SESSION_TOKEN_READ_AND_WRITE_DENIED_PROFILE_HISTORY_UNCHANGED');
  await beforeIdle();
  await waitIdle(async()=>ok(control,'ListarPerfiles',list));
  await reject(expiring,'ListarPerfiles',list,401,'SESION_REQUERIDA');
  await reject(expiring,'GuardarBorrador',invalidWrite,401,'SESION_REQUERIDA');
  same(await snapshot(),before);
  console.log('DOC110_PASS: PREVIOUSLY_AUTHENTICATED_IDLE_SESSION_DENIED_PROFILE_HISTORY_UNCHANGED');
  return {checks:4};
}

async function exerciseSessionExpiry(auth,baseUrl,token,sample){
  if(!sample?.PerfilId)throw new Error('DOC110_SESSION_EXPIRY_EXISTING_PROFILE_REQUIRED');
  const minutes=Number(process.env.DOC110_E2E_SESSION_IDLE_MINUTES);
  if(!Number.isInteger(minutes)||minutes<1||minutes>60)throw new Error('DOC110_SESSION_TIMEOUT_CONFIGURATION_REQUIRED');
  const fs=require('node:fs'),path=require('node:path');
  const {safePost}=require('./doc110-safe-request.cjs');
  const {openImportAdministrator}=require('./doc110-admin-navigation.cjs');
  const {createWorkflowLoginPageSession}=require('./authenticated-workflow-session.cjs');
  const evidence={status:'RUNNING',configuredIdleMinutes:minutes,startedUtc:new Date().toISOString()};
  function peer(context,csrf){return async(method,data)=>{
    const response=await safePost(context.request,new URL('webservice/WebServiceImportacionCarpetas.asmx/'+method,baseUrl).href,
      {headers:{'X-Importacion-Perfil-CSRF':csrf},data:{solicitud:data}},method);
    let result;try{result=(await response.json()).d;}catch{throw new Error('DOC110_SESSION_EXPIRY_NON_JSON');}
    return {status:response.status(),result};
  };}
  let contender;
  try{
    contender=await createWorkflowLoginPageSession(auth.context.browser(),{
      baseUrl,moduleEnvironmentVariable:'DOC110_E2E_MODULE',userEnvironmentVariable:'DOC110_E2E_USER',
      passwordEnvironmentVariable:'DOC110_E2E_PASSWORD',ignoreHTTPSErrors:process.env.DOC10_E2E_IGNORE_HTTPS_ERRORS==='true'
    });
    await contender.page.goto(new URL('Gestion/WebFormProducionDocumental.aspx',baseUrl).href);
    const opened=await openImportAdministrator(contender.page,'abrir','ListarPerfiles');
    expect(opened.status()).toBe(200);expect((await opened.json()).d?.ok).toBe(true);
    const secondToken=opened.request().headers()['x-importacion-perfil-csrf'];
    if(!secondToken)throw new Error('DOC110_CSRF_NOT_OBSERVED');
    const session=async context=>(await context.cookies(baseUrl)).find(c=>c.name==='ASP.NET_SessionId')?.value;
    const firstId=await session(auth.context),secondId=await session(contender.context);
    expect(Boolean(firstId&&secondId&&firstId!==secondId&&token!==secondToken)).toBe(true);
    console.log('DOC110_PASS: SESSION_EXPIRY_INDEPENDENT_AUTHENTICATED_SESSIONS');
    const result=await verifySessionExpiry({
      control:peer(auth.context,token),expiring:peer(contender.context,secondToken),
      foreignControl:peer(auth.context,secondToken),profileId:sample.PerfilId,
      async beforeIdle(){
        // Detener páginas y temporizadores sin borrar cookies ni cerrar la sesión servidor.
        for(const context of [auth.context,contender.context])for(const page of context.pages())await page.close();
      },
      async waitIdle(keepControlAlive){
        const started=Date.now(),duration=minutes*60000+75000;
        let lastNotice=-1;
        while(Date.now()-started<duration){
          const remaining=Math.ceil((duration-(Date.now()-started))/60000);
          if(remaining!==lastNotice){console.log('DOC110_IDLE_WAIT: aproximadamente '+remaining+' min; sin solicitudes a la sesion que debe expirar.');lastNotice=remaining;}
          await new Promise(resolve=>setTimeout(resolve,Math.min(30000,duration-(Date.now()-started))));
          await keepControlAlive();
        }
        evidence.observedIdleMilliseconds=Date.now()-started;
      }
    });
    evidence.checks=result.checks;evidence.status='PASS';
  }catch(error){evidence.status='FAIL';throw error;}
  finally{
    evidence.finishedUtc=new Date().toISOString();
    const report=path.resolve(__dirname,'../../artifacts/doc110-session-expiry-result.json');
    fs.mkdirSync(path.dirname(report),{recursive:true});fs.writeFileSync(report,JSON.stringify(evidence,null,2));
    if(contender)await contender.context.close();
  }
}
module.exports={sessionIdleMinutes,verifySessionExpiry,exerciseSessionExpiry};
