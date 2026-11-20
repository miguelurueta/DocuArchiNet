'use strict';
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto'),cp=require('node:child_process');
const {collectValue,requireInteractiveConsole}=require('./support/interactive-e2e-console.cjs');
const {createWorkflowLoginPageSession}=require('../tests/support/authenticated-workflow-session.cjs');
const {classifyLoginMessage,classifyLoginPostback}=require('../tests/support/doc111-importacion-carpetas.cjs');
const {safePost}=require('../tests/support/doc110-safe-request.cjs');
const ROOT=path.resolve(__dirname,'../../..'),BASE='https://localhost/GestionDocumental-Docuarchi.net/';
const MUTATIONS=['CrearVersion','GuardarBorrador','PublicarVersion'],READS=['ObtenerPerfil','ConsultarCatalogo','ListarPerfiles','ConsultarHistorial','ConsultarProveedores'];
const id=v=>typeof v==='string'&&/^[1-9][0-9]{0,9}$/.test(v)&&Number(v)<=4294967295;
function check(ok,code='SCOPE_CHANGED'){if(!ok)throw Error('DOC120_LENGTH_CHANGE_'+code);}
function canonical(value){if(Array.isArray(value))return '['+value.map(canonical).join(',')+']';if(value&&typeof value==='object')return '{'+Object.keys(value).sort().map(k=>JSON.stringify(k)+':'+canonical(value[k])).join(',')+'}';return JSON.stringify(value);}
function versionContent(profile){return{config:profile.Version.Configuracion,rules:profile.Version.Reglas.map(r=>{const copy={...r};delete copy.ReglaId;return copy;})};}
function checkedProfile(p,m){check(p&&p.PerfilId===m.profileId&&p.AmbitoRef===m.companyRef&&p.Nombre==='Egresos'&&p.Activo===true&&p.Version&&String(p.Version.Configuracion.GabineteId)==='10'&&Array.isArray(p.Version.Reglas));return p;}
function updatePayload(profile,maximum){
 check(Number.isInteger(maximum)&&maximum>=63&&maximum<=250,'LIMIT_INVALID');const rules=structuredClone(profile.Version.Reglas),matching=rules.filter(r=>r.DestinoTipo==='INDICE_GABINETE'&&r.DestinoRef==='17');
 check(matching.length===1&&matching[0].TipoValor==='TEXTO'&&matching[0].LongitudMaxima===60,'RULE_CHANGED');matching[0].LongitudMaxima=maximum;
 return{perfilId:profile.PerfilId,versionId:profile.Version.VersionId,revisionVersionEsperada:profile.Version.Revision,configuracion:structuredClone(profile.Version.Configuracion),reglas:rules};
}
function assertOnlyLengthChanged(original,updated){
 const before=versionContent(original),after=versionContent(updated),copy=structuredClone(after),rules=copy.rules.filter(r=>r.DestinoTipo==='INDICE_GABINETE'&&r.DestinoRef==='17');
 check(rules.length===1&&rules[0].LongitudMaxima===250,'UPDATED_RULE_INVALID');rules[0].LongitudMaxima=60;check(canonical(copy)===canonical(before),'UNRELATED_PROFILE_CHANGE');
}
async function changeProfile(call,measurement,onStep=()=>{}){
 const original=checkedProfile(await call('ObtenerPerfil',{perfilId:measurement.profileId}),measurement);
 onStep('PROFILE_OBSERVED',original);
 check(original.Version.VersionId===measurement.versionId&&original.VersionPublicadaId===measurement.versionId&&original.Revision===measurement.profileRevision&&original.Version.Revision===measurement.versionRevision&&original.Version.Estado==='PUBLICADA','PUBLICATION_CHANGED');
 updatePayload(original,250);
 onStep('PROFILE_VERIFIED');
 return{original,async apply(){
  const draft=checkedProfile(await call('CrearVersion',{perfilId:original.PerfilId,versionOrigenId:original.Version.VersionId,revisionPerfilEsperada:original.Revision,revisionOrigenEsperada:original.Version.Revision}),measurement);
  check(draft.Version.Estado==='BORRADOR'&&id(draft.Version.VersionId)&&draft.Version.VersionId!==original.Version.VersionId,'DRAFT_INVALID');
  check(canonical(versionContent(draft))===canonical(versionContent(original)),'CLONE_CHANGED');onStep('DRAFT_CREATED',draft);
  const updated=checkedProfile(await call('GuardarBorrador',updatePayload(draft,250)),measurement);assertOnlyLengthChanged(original,updated);onStep('DRAFT_UPDATED',updated);
  const published=checkedProfile(await call('PublicarVersion',{perfilId:updated.PerfilId,versionId:updated.Version.VersionId,revisionPerfilEsperada:updated.Revision,revisionVersionEsperada:updated.Version.Revision}),measurement);
  check(published.Version.Estado==='PUBLICADA'&&published.VersionPublicadaId===updated.Version.VersionId,'PUBLICATION_UNCONFIRMED');assertOnlyLengthChanged(original,published);onStep('PUBLISHED',published);
  const old=checkedProfile(await call('ObtenerPerfil',{perfilId:original.PerfilId,versionId:original.Version.VersionId}),measurement);
  check(canonical(old.Version)===canonical(original.Version),'ORIGINAL_PUBLICATION_CHANGED');
  const current=checkedProfile(await call('ObtenerPerfil',{perfilId:published.PerfilId}),measurement);check(current.VersionPublicadaId===published.Version.VersionId&&current.Version.VersionId===published.Version.VersionId,'FINAL_PUBLICATION_CHANGED');assertOnlyLengthChanged(original,current);
  return{profileId:current.PerfilId,previousVersionId:original.Version.VersionId,versionId:current.Version.VersionId,versionNumber:current.Version.Numero,oldLimit:60,newLimit:250,originalPublicationUnchanged:true,onlyTargetRuleChanged:true};
 }};
}
function csrfFromHtml(html){const tag=(html.match(/<div\b[^>]*\bid="doc110-perfiles"[^>]*>/i)||[])[0];const token=tag&&(tag.match(/\bdata-token="([^"]+)"/i)||[])[1];check(typeof token==='string'&&/^[A-Za-z0-9+/]{43}=$/.test(token),'ADMIN_CONTROL_UNAVAILABLE');return token;}
function loginRequestAction(request){
 const url=new URL(request.url()),method=request.method();
 if(method==='GET'&&request.resourceType()==='stylesheet'&&request.url()==='https://fonts.googleapis.com/css?family=Open+Sans:300,400,400i,600,700,800')return 'OMIT_FONT';
 const base=new URL(BASE);if(url.origin!==base.origin||url.username||url.password||!url.pathname.toLowerCase().startsWith(base.pathname.toLowerCase()))return 'DENY';
 const relative=url.pathname.slice(base.pathname.length).toLowerCase();
 if(relative==='defaul/webforminiciodocuarchigestion.aspx')return method==='GET'&&!url.search?'OMIT_LANDING':'DENY';
 if(relative==='webservice/webserviceloginsegundofactor.asmx/obtenerestado'){
  try{return method==='POST'&&!url.search&&canonical(JSON.parse(request.postData()))==='{}'?'ALLOW':'DENY';}catch{return 'DENY';}
 }
 if(method==='POST')return relative==='gestor.aspx'&&!url.search?'ALLOW':'DENY';
 if(method!=='GET'||/\.(?:asmx|ashx)(?:\/|$)/i.test(relative)||/\.aspx$/i.test(relative)&&relative!=='gestor.aspx')return 'DENY';
 return 'ALLOW';
}
async function physicalChange(input){
 const script=path.join(__dirname,'support/apply-doc120-beneficiario-capacity.ps1');
 return new Promise((resolve,reject)=>{
  const child=cp.spawn('powershell.exe',['-NoProfile','-NonInteractive','-ExecutionPolicy','Bypass','-File',script],{windowsHide:true,stdio:['pipe','pipe','pipe'],env:Object.fromEntries(Object.entries(process.env).filter(([k])=>!/^DOC\d+(?:_DA)?_E2E/.test(k)&&!['DEBUG','PWDEBUG','NODE_DEBUG','NODE_DEBUG_NATIVE'].includes(k)))});
  let out='',size=0,settled=false;const timer=setTimeout(()=>{child.kill();},180000);
  child.stdout.on('data',b=>{size+=b.length;if(size>65536)child.kill();else out+=b;});child.stderr.on('data',()=>{});child.stdin.on('error',()=>{});
  child.on('error',()=>{clearTimeout(timer);settled=true;reject(Error('DOC120_LENGTH_CHANGE_PHYSICAL_UNCONFIRMED'));});
  child.on('close',()=>{clearTimeout(timer);if(settled)return;try{const value=JSON.parse(out.replace(/^\uFEFF/,''));check(value&&['PASS_PHYSICAL_AND_METADATA_UPDATED','STOPPED'].includes(value.status)&&Number.isInteger(value.select)&&value.select<=12,'PHYSICAL_RESPONSE_INVALID');resolve(value);}catch{reject(Error('DOC120_LENGTH_CHANGE_PHYSICAL_UNCONFIRMED'));}});
  child.stdin.end(JSON.stringify(input));
 });
}
function errorCode(e){const text=e?.message||'';if(/^DOC120_LENGTH_CHANGE_[A-Z_]+$/.test(text))return text;if(/^DOC111_LOGIN_[A-Z_]+$/.test(text))return text;return 'DOC120_LENGTH_CHANGE_STOPPED';}
async function main(args=process.argv.slice(2)){
 check(args.length===5&&args[0]==='--measurement'&&/^[a-f0-9]{32}$/.test(args[1])&&args[2]==='--run'&&/^[a-f0-9]{32}$/.test(args[3])&&args[4]==='--authorized-change','AUTHORIZATION_REQUIRED');requireInteractiveConsole();
 const measured=JSON.parse(fs.readFileSync(path.join(ROOT,'.tmp/doc120-egresos-length',args[1],'state.json'),'utf8'));check(measured.status==='MEASURED'&&measured.closed&&measured.credentialsCleared,'MEASUREMENT_REQUIRED');
 const m=measured.result;check(m.indexId==='17'&&m.cabinetId==='10'&&m.physical.tabla==='CONTABIL'&&m.physical.columna==='BENEFICIARIO'&&m.source.table==='imp_02_egresos'&&m.source.column==='BENEFICIARIO'&&m.source.declaredCapacity===250&&m.source.maximum<=250&&m.ruleLimit===60&&m.physicalCapacity===60,'MEASUREMENT_CHANGED');
 const runId=args[3],directory=path.join(ROOT,'.tmp/doc120-egresos-length-change',runId);check(!fs.existsSync(directory),'RUN_EXISTS');fs.mkdirSync(directory,{recursive:true});
 const state={runId,measurementRunId:args[1],status:'WAITING_INPUT',http:0,maxHttp:600,sessions:0,maxSessions:1,controlSelect:0,maxControlSelect:12,mutations:{},importEffects:0,closed:false},values={};let browser,auth,environment;
 const save=()=>fs.writeFileSync(path.join(directory,'state.json'),JSON.stringify(state,null,2));const step=(name,p)=>{state.status=name;if(name==='PROFILE_OBSERVED'){state.observedProfile={versionId:p.Version.VersionId,publishedVersionId:p.VersionPublicadaId,profileRevision:p.Revision,versionRevision:p.Version.Revision,published:p.Version.Estado==='PUBLICADA'};}else if(name==='DRAFT_CREATED'&&p?.Version?.VersionId){state.newVersionId=p.Version.VersionId;}save();console.log('DOC120_LENGTH_CHANGE_PROGRESS='+name);};
 function take(){check(state.http<state.maxHttp,'HTTP_BUDGET_EXHAUSTED');state.http++;save();}save();
 try{
  process.title='DOC-120 - ampliar BENEFICIARIO y publicar Egresos';
  console.log('Cambio autorizado: CONTABIL.BENEFICIARIO60 a250, metadato del indice17 y UNA nueva version del perfil Egresos; se conserva la publicacion anterior. No importa ni modifica documentos o registros del origen.');
  console.log('Maximo600 HTTP,1 sesion,12 SELECT de control,1 ALTER y1 UPDATE del metadato. Se detiene al primer error y no repite operaciones.');
  await collectValue(values,'dsn','DSN DA',{defaultValue:'MembershipUsers'});check(/^[A-Za-z0-9 _.-]{1,128}$/.test(values.dsn),'DSN_INVALID');
  await collectValue(values,'user','Cuenta de aplicacion con administracion de perfiles (enmascarada)',{secret:true});await collectValue(values,'password','Clave de aplicacion',{secret:true});
  await collectValue(values,'dbUser','Cuenta MySQL con ALTER en CONTABIL y UPDATE en detalle_gabienete (enmascarada)',{secret:true});await collectValue(values,'dbPassword','Clave de esa cuenta MySQL',{secret:true});
  environment=Object.fromEntries(Object.entries(process.env).filter(([k])=>!/^DOC\d+(?:_DA)?_E2E/.test(k)&&!['DEBUG','PWDEBUG','NODE_DEBUG','NODE_DEBUG_NATIVE'].includes(k)));
  Object.assign(environment,{DOC120_E2E_MODULE:'GESTOR',DOC120_E2E_USER:values.user,DOC120_E2E_PASSWORD:values.password});
  browser=await require('../node_modules/playwright').chromium.launch({headless:true});let loginFailure;
  const guarded={newContext:async options=>{check(state.sessions<1,'SESSION_BUDGET_EXHAUSTED');state.sessions++;save();const context=await browser.newContext(options);await context.route('**/*',async route=>{
   const request=route.request(),action=loginRequestAction(request);
   if(action==='OMIT_FONT'){state.omittedFont=true;save();return route.abort();}
   if(action==='OMIT_LANDING'){state.omittedLanding=true;save();return route.fulfill({status:200,contentType:'text/html',body:'<!doctype html><html><body></body></html>'});}
   if(action==='DENY'){state.deniedRequest=true;save();return route.abort();}try{take();await route.continue();}catch{await route.abort();}
  });return context;}};
  step('AUTHENTICATING');auth=await createWorkflowLoginPageSession(guarded,{baseUrl:BASE,environment,moduleEnvironmentVariable:'DOC120_E2E_MODULE',userEnvironmentVariable:'DOC120_E2E_USER',passwordEnvironmentVariable:'DOC120_E2E_PASSWORD',ignoreHTTPSErrors:true,timeoutMilliseconds:60000,onDialogCompleted:async(_p,text)=>{loginFailure=classifyLoginMessage(text)||'LOGIN_SERVER_REJECTED';},onPostbackCompleted:async(_p,response)=>{try{loginFailure=classifyLoginPostback(await response.text())||loginFailure;}catch{}}});
  if(loginFailure)throw Error('DOC111_'+loginFailure);check(!state.deniedRequest,'LOGIN_SCOPE_DENIED');await auth.page.close();take();
  const pageResponse=await auth.context.request.get(new URL('Gestion/WebFormProducionDocumental.aspx',BASE).href,{maxRedirects:0,timeout:60000});check(pageResponse.status()===200,'PRODUCTION_UNAVAILABLE');const token=csrfFromHtml(await pageResponse.text());
  async function call(method,payload){
   check(READS.includes(method)||MUTATIONS.includes(method),'OPERATION_DENIED');if(MUTATIONS.includes(method)){check(!state.mutations[method],'MUTATION_ALREADY_ATTEMPTED');state.mutations[method]=1;save();}take();
   const response=await safePost(auth.context.request,new URL('webservice/WebServiceImportacionCarpetas.asmx/'+method,BASE).href,{maxRedirects:0,headers:{'X-Importacion-Perfil-CSRF':token,Origin:new URL(BASE).origin},data:{solicitud:payload}},method);
   let result;try{const raw=await response.json();result=raw.d??raw;}catch{throw Error('DOC120_LENGTH_CHANGE_RESPONSE_INVALID');}
   check(response.status()===200&&result?.ok===true,'API_REJECTED');return result.datos;
  }
  const prepared=await changeProfile(call,m,step);check(/^[a-f0-9]{64}$/.test(prepared.original.Version.Configuracion.HuellaOrigenGabinete),'ORIGIN_HASH_INVALID');
  const beforeFields=await call('ConsultarCatalogo',{catalogo:'INDICE',referencia:'17',configuracion:prepared.original.Version.Configuracion,pagina:1,tamanoPagina:1});
  check(Array.isArray(beforeFields)&&beforeFields.filter(f=>f.value==='17'&&f.Tipo==='TEXTO'&&f.Longitud===60).length===1,'INITIAL_CATALOG_CHANGED');
  step('APPLYING_PHYSICAL_CAPACITY');state.physical=await physicalChange({authorization:'EGRESOS_CONTABIL_BENEFICIARIO_60_TO_250',dsn:values.dsn,user:values.dbUser,password:values.dbPassword,expectedOriginHash:prepared.original.Version.Configuracion.HuellaOrigenGabinete});state.controlSelect=state.physical.select;save();check(state.physical.status==='PASS_PHYSICAL_AND_METADATA_UPDATED','PHYSICAL_UNCONFIRMED');
  const fields=await call('ConsultarCatalogo',{catalogo:'INDICE',referencia:'17',configuracion:prepared.original.Version.Configuracion,pagina:1,tamanoPagina:1});const list=Array.isArray(fields)?fields:fields?.Elementos;check(Array.isArray(list)&&list.filter(f=>f.value==='17'&&f.Tipo==='TEXTO'&&f.Longitud===250).length===1,'CATALOG_CAPACITY_UNCONFIRMED');
  state.result=await prepared.apply();step('FINISHED');
 }catch(e){state.status='STOPPED';state.failure=errorCode(e);process.exitCode=2;}
 finally{
  if(auth?.context)await auth.context.close().catch(()=>{});if(browser)await browser.close().catch(()=>{});
  for(const k of Object.keys(values))delete values[k];if(environment)for(const k of Object.keys(environment))if(/^DOC120_E2E_/.test(k))delete environment[k];Object.assign(state,{closed:true,credentialsCleared:true,finishedUtc:new Date().toISOString()});save();console.log('DOC120_EGRESOS_LENGTH_CHANGE_RESULT='+JSON.stringify(state));
 }
}
if(require.main===module)main().catch(()=>{console.error('DOC120_LENGTH_CHANGE_PRECHECK_STOPPED');process.exitCode=2;});
module.exports={canonical,versionContent,updatePayload,assertOnlyLengthChanged,changeProfile,csrfFromHtml,loginRequestAction,physicalChange,errorCode,main};
