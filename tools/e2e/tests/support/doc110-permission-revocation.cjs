'use strict';
const {expect}=require('@playwright/test');
const same=(a,b)=>expect(JSON.stringify(a)===JSON.stringify(b)).toBe(true);

async function verifyPermissionRevocation({call,bridge,profileId,checkUI=async()=>{}}){
  async function ok(method,data){
    const response=await call(method,data);expect(response.status).toBe(200);expect(response.result?.ok).toBe(true);
    return response.result.datos;
  }
  const refs={perfilId:profileId};
  async function snapshot(){return {profile:await ok('ObtenerPerfil',refs),
    history:await ok('ConsultarHistorial',{...refs,pagina:1,tamanoPagina:25})};}
  const before=await snapshot();
  expect(String(before.profile.AmbitoRef)===String(bridge.companyRef)).toBe(true);
  await ok('ListarPerfiles',{pagina:1,tamanoPagina:1});
  let changed=false;
  try{
    changed=true; // Incluso una respuesta de revocación incierta exige reconciliar/restaurar.
    const revoked=await bridge.send('REVOKE');
    expect(revoked.ok&&revoked.stage==='REVOKED'&&revoked.otherFieldsUnchanged).toBe(true);
    for(const [method,data] of [
      ['ListarPerfiles',{pagina:1,tamanoPagina:1}],['ObtenerPerfil',refs],
      ['ConsultarHistorial',{...refs,pagina:1,tamanoPagina:25}],
      ['GuardarBorrador',{...refs,versionId:'0',revisionVersionEsperada:0,configuracion:null,reglas:[]}],
      ['PublicarVersion',{...refs,versionId:'0',revisionPerfilEsperada:0,revisionVersionEsperada:0}]
    ]){
      const response=await call(method,data);expect(response.status).toBe(403);
      expect(response.result?.ok).toBe(false);expect(response.result?.codigo).toBe('ACCESO_DENEGADO');
      expect(response.result?.datos==null).toBe(true);
    }
    await checkUI(false);
    console.log('DOC110_PASS: REVOKED_PROFILE_PERMISSION_DENIED_NEXT_REQUEST_SAME_SESSION');
  }finally{
    if(changed){
      const restored=await bridge.send('RESTORE');
      expect(restored.ok&&restored.stage==='RESTORED'&&restored.otherFieldsUnchanged).toBe(true);
      console.log('DOC110_PASS: ORIGINAL_PROFILE_PERMISSION_RESTORED_OTHER_FIELDS_UNCHANGED');
    }
  }
  await ok('ListarPerfiles',{pagina:1,tamanoPagina:1});
  same(await snapshot(),before);
  await checkUI(true);
  console.log('DOC110_PASS: RESTORED_PERMISSION_SAME_SESSION_PROFILE_HISTORY_UNCHANGED');
  return {checks:5};
}

function startDatabaseBridge(script='doc110-permission-db.ps1'){
  if(!['doc110-permission-db.ps1','doc110-profile-schema-db.ps1'].includes(script))throw new Error('DOC110_DATABASE_BRIDGE_SCRIPT_INVALID');
  const {spawn}=require('node:child_process');
  const readline=require('node:readline'),path=require('node:path');
  const child=spawn('powershell.exe',['-NoProfile','-NonInteractive','-ExecutionPolicy','Bypass','-File',
    path.resolve(__dirname,'../../scripts/support',script)],{windowsHide:true,stdio:['pipe','pipe','pipe']});
  // Nunca reenviar stderr ni errores internos del proveedor de BD.
  child.stderr.resume();
  let pending=null,ended=false,exitCode=null;
  const buffered=[];
  function fail(){if(pending){pending.reject(new Error('DOC110_PERMISSION_BRIDGE_FAILED'));pending=null;}}
  const lines=readline.createInterface({input:child.stdout});
  lines.on('line',line=>{
    let value;try{value=JSON.parse(line);}catch{fail();return;}
    if(pending){const next=pending;pending=null;next.resolve(value);}else buffered.push(value);
  });
  child.stdin.on('error',fail);child.on('error',fail);
  const exited=new Promise(resolve=>child.on('close',code=>{ended=true;exitCode=code;fail();resolve();}));
  function next(){
    if(buffered.length)return Promise.resolve(buffered.shift());
    if(ended)return Promise.reject(new Error('DOC110_PERMISSION_BRIDGE_CLOSED'));
    return new Promise((resolve,reject)=>{
      const timer=setTimeout(()=>{pending=null;reject(new Error('DOC110_PERMISSION_BRIDGE_TIMEOUT'));},30000);
      pending={resolve:value=>{clearTimeout(timer);resolve(value);},reject:error=>{clearTimeout(timer);reject(error);}};
    });
  }
  return {ready:()=>next(),async send(operation){
    const allowed=script==='doc110-permission-db.ps1'?['REVOKE','RESTORE']:['HIDE','RESTORE','LEGACY_COUNT'];
    if(!allowed.includes(operation)||ended)throw new Error('DOC110_PERMISSION_BRIDGE_CLOSED');
    const response=next();child.stdin.write(JSON.stringify({operation})+'\n');return response;
  },async close(){
    child.stdin.end();
    let timer;try{await Promise.race([exited,new Promise((_,reject)=>{timer=setTimeout(()=>reject(new Error('DOC110_PERMISSION_BRIDGE_EXIT_UNCONFIRMED')),35000);})]);}
    finally{clearTimeout(timer);}
    if(exitCode!==0)throw new Error('DOC110_PERMISSION_BRIDGE_RECOVERY_CHECK_REQUIRED');
  }};
}

async function exercisePermissionRevocation(auth,baseUrl,token,sample){
  if(process.env.DOC110_E2E_USER?.toUpperCase()!=='LUZ.AGUILERA'||!sample?.PerfilId)
    throw new Error('DOC110_PERMISSION_AUTHORIZED_ACCOUNT_AND_PROFILE_REQUIRED');
  const fs=require('node:fs'),path=require('node:path');
  const {safePost}=require('./doc110-safe-request.cjs');
  const evidence={status:'RUNNING',startedUtc:new Date().toISOString(),permission:'ADMINISTRAR_PERFILES_IMPORTACION'};
  const bridge=startDatabaseBridge();
  try{
    const ready=await bridge.ready();
    expect(ready.ok&&ready.stage==='READY'&&ready.originalGranted).toBe(true);
    bridge.companyRef=ready.companyRef;
    const call=async(method,data)=>{
      const response=await safePost(auth.context.request,new URL('webservice/WebServiceImportacionCarpetas.asmx/'+method,baseUrl).href,
        {headers:{'X-Importacion-Perfil-CSRF':token},data:{solicitud:data}},method);
      let result;try{result=(await response.json()).d;}catch{throw new Error('DOC110_PERMISSION_NON_JSON');}
      return {status:response.status(),result};
    };
    const result=await verifyPermissionRevocation({call,bridge,profileId:sample.PerfilId,
      async checkUI(allowed){
        await auth.page.goto(new URL('Gestion/WebFormProducionDocumental.aspx',baseUrl).href);
        await expect(auth.page.locator('#doc110-perfiles')).toHaveCount(allowed?1:0);
      }
    });
    evidence.checks=result.checks;evidence.status='PASS';
  }catch(error){evidence.status='FAIL';throw error;}
  finally{
    try{await bridge.close();}
    catch{evidence.status='FAIL';throw new Error('DOC110_PERMISSION_RESTORATION_MUST_BE_VERIFIED');}
    finally{
      evidence.finishedUtc=new Date().toISOString();
      const report=path.resolve(__dirname,'../../artifacts/doc110-permission-revocation-result.json');
      fs.mkdirSync(path.dirname(report),{recursive:true});fs.writeFileSync(report,JSON.stringify(evidence,null,2));
    }
  }
}
module.exports={verifyPermissionRevocation,exercisePermissionRevocation,startDatabaseBridge};
