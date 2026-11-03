'use strict';
const fs=require('node:fs'),path=require('node:path');
const {collectConfirmation,collectValue,requireInteractiveConsole,runChild}=require('./support/interactive-e2e-console.cjs');
const {readFixture}=require('../tests/support/doc111-importacion-carpetas.cjs');
const {CASES,validateOptions}=require('../tests/support/doc113-ventana-importacion.cjs');
function parseArgs(args){
  const parsed={confirm:false,check:false};
  for(let i=0;i<args.length;i++){
    const flag=args[i];
    if(['--confirm','--check'].includes(flag)&&parsed[flag.slice(2)]===false){parsed[flag.slice(2)]=true;continue;}
    if(['--profile','--scenario','--alternate-level','--alternate-version'].includes(flag)&&parsed[flag.slice(2)]===undefined&&args[i+1]&&!args[i+1].startsWith('--')){parsed[flag.slice(2)]=args[++i];continue;}
    throw Error('DOC113_ARGUMENT_INVALID');
  }
  if(!parsed.profile||!CASES.includes(parsed.scenario))throw Error('DOC113_PROFILE_AND_SCENARIO_REQUIRED');return parsed;
}
function safeLine(line,scenario){
  line=String(line).replace(/\x1b\[[0-9;]*m/g,'').trim();
  const prefixes=['DOC113_UI_RESULT=','DOC113_UI_REFERENCE=','DOC113_UI_FAILURE='],prefix=prefixes.find(p=>line.startsWith(p));if(!prefix)return null;
  let v;try{v=JSON.parse(line.slice(prefix.length));}catch{return null;}if(v.scenario!==scenario)return null;
  const id=x=>typeof x==='string'&&/^[1-9][0-9]{0,9}$/.test(x)&&BigInt(x)<=4294967295n;
  if(prefix==='DOC113_UI_REFERENCE=')return id(v.planId)&&(!v.loteId||id(v.loteId))?{prefix,value:{scenario,planId:v.planId,...v.loteId?{loteId:v.loteId}:{}}}:null;
  if(prefix==='DOC113_UI_FAILURE=')return ['PRECHECK','LOGIN','SOURCE','UI'].includes(v.stage)&&/^DOC113_[A-Z0-9_]{3,70}$/.test(v.code)?{prefix,value:{scenario,stage:v.stage,code:v.code}}:null;
  if(v.status!=='PASS'||!['readOnlyControls','noDocumentEffects','domVerified'].every(k=>v[k]===true)||!/^[a-f0-9]{40}$/.test(v.head)||!['codeSha256','servedScriptSha256'].every(k=>/^[a-f0-9]{64}$/.test(v[k]))||!Number.isInteger(v.preparations)||!Number.isInteger(v.confirmations)||!Number.isInteger(v.opens))return null;
  if(v.preparations!==(scenario==='UI-02'?2:0)||![0,1].includes(v.confirmations)||scenario!=='UI-02'&&v.confirmations!==0||v.opens!==(scenario==='UI-03'?3:1))return null;
  return {prefix,value:Object.fromEntries(['scenario','status','preparations','confirmations','opens','readOnlyControls','noDocumentEffects','domVerified','head','codeSha256','servedScriptSha256'].map(k=>[k,v[k]]))};
}
async function main(args=process.argv.slice(2),runtime={}){
  const parsed=parseArgs(args),profile=JSON.parse(fs.readFileSync(path.resolve(parsed.profile),'utf8').replace(/^\uFEFF/,''));
  const options=validateOptions(profile,{scenario:parsed.scenario,alternateLevel:parsed['alternate-level'],alternateVersion:parsed['alternate-version'],confirm:parsed.confirm});readFixture(profile);
  if(parsed.check){console.log('DOC113_CHECK=PASS; REAL_EXECUTION=false');return;}
  (runtime.requireInteractiveConsole||requireInteractiveConsole)();
  const confirm=runtime.collectConfirmation||collectConfirmation,collect=runtime.collectValue||collectValue,run=runtime.runChild||runChild;
  const authorization={},secrets={};let environment;
  try{
    const scopes=[['environment','Autoriza este ambiente de pruebas para DOC-113 '+options.scenario],['account','Autoriza la cuenta operativa de pruebas'],['select','Autoriza controles RA/DA con cuentas exclusivamente SELECT']];
    if(options.scenario==='UI-02')scopes.push(['effects','Autoriza preparar dos planes para nivel '+profile.nivelRef+' con el perfil '+profile.versionId],['discardable','Confirma que los nombres '+profile.rootNames.join(' / ')+' y planes son recursos de prueba autorizados; se conservarán']);
    if(options.confirm)scopes.push(['confirm','Autoriza confirmar exclusivamente el segundo plan en un lote, sin recibir ni almacenar documentos']);
    if(options.scenario==='UI-03')scopes.push(['errorProbe','Autoriza una apertura con referencia inválida 0 para comprobar un rechazo real sin efectos']);
    if(profile.ignoreHttpsErrors)scopes.push(['tls','Autoriza el certificado de pruebas de este ambiente']);
    for(const [key,label]of scopes)await confirm(authorization,key,label);
    await collect(secrets,'user','Cuenta autorizada (visible solo en esta consola)',{secret:false});await collect(secrets,'password','Contraseña',{secret:true});
    await collect(secrets,'dbUser','Cuenta MySQL exclusivamente SELECT para los DSN registrados',{secret:true});await collect(secrets,'dbPassword','Contraseña MySQL de solo lectura',{secret:true});
    environment=Object.fromEntries(Object.entries(process.env).filter(([key])=>!/^DOC11[13]_/.test(key)));
    Object.assign(environment,{DOC113_E2E_AUTHORIZED:'true',DOC113_UI_SCENARIO:options.scenario,DOC113_UI_OPTIONS:JSON.stringify(options),DOC113_UI_PROFILE:JSON.stringify(profile),DOC111_E2E_MODULE:profile.module,DOC111_E2E_USER:secrets.user,DOC111_E2E_PASSWORD:secrets.password,DOC111_E2E_ODBC_DSN:profile.odbcDsn,DOC111_DA_E2E_ODBC_DSN:profile.docuarchiOdbcDsn});
    for(const prefix of ['DOC111_E2E','DOC111_DA_E2E'])Object.assign(environment,{[prefix+'_MYSQL_USER']:secrets.dbUser,[prefix+'_MYSQL_PASSWORD']:secrets.dbPassword});
    for(const [key]of scopes){if(authorization[key]!=='true')throw Error('DOC113_AUTHORIZATION_REQUIRED');environment['DOC113_AUTH_'+key.toUpperCase()]='true';}
    const results=[];
    const child=await run(process.execPath,[path.resolve(__dirname,'../node_modules/@playwright/test/cli.js'),'test','tests/doc113-ventana-importacion.spec.cjs','--grep',options.scenario,'--retries=0','--workers=1'],path.resolve(__dirname,'..'),environment,{nonInteractiveChild:true,redactOutput:text=>String(text).split(/\r?\n/).map(line=>{const safe=safeLine(line,options.scenario);if(!safe)return '';if(safe.prefix==='DOC113_UI_RESULT=')results.push(safe.value);return safe.prefix+JSON.stringify(safe.value)+'\n';}).join('')});
    if(child.code!==0||results.length!==1||results[0].confirmations!==(options.confirm?1:0))throw Error('DOC113_SESSION_STOPPED');
  }finally{for(const key of Object.keys(secrets))delete secrets[key];if(environment)for(const key of Object.keys(environment))if(/^DOC11[13]_/.test(key))delete environment[key];}
}
if(require.main===module)main().catch(()=>{console.error('DOC113_SESSION_STOPPED: consulte las referencias saneadas antes de repetir; sin detalle privado.');process.exitCode=2;});
module.exports={parseArgs,safeLine,main};
