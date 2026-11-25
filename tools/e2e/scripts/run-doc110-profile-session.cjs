'use strict';
const fs=require('node:fs');
const path=require('node:path');
const {collectValue,collectConfirmation,requireInteractiveConsole,runChild}=require('./support/interactive-e2e-console.cjs');
const {sourceFixture}=require('../tests/support/doc110-source-fixture.cjs');

function excelUiPlan(root,profile,args,environment=process.env){
 const modes=args.filter(x=>['--excel-ui-read','--excel-ui-write','--excel-ui-assistant','--excel-ui-functional','--excel-ui-resilience'].includes(x));
 if(modes.length!==1||args.some(x=>!['--excel-ui-read','--excel-ui-write','--excel-ui-assistant','--excel-ui-functional','--excel-ui-resilience','--headed'].includes(x)))throw Error('DOC126_MODE_CONFLICT');
 const mode=modes[0].slice('--excel-ui-'.length),file=environment.DOC126_E2E_SCOPE_PATH;
 if(!file||!fs.existsSync(file))throw Error('DOC126_SCOPE_REQUIRED');
 const scope=require('../tests/support/doc126-excel-scope.cjs').readScope(file,profile.baseUrl,mode);
 const values={DOC126_E2E_BASE_URL:profile.baseUrl,DOC126_E2E_MODULE:profile.module,DOC126_E2E_SCOPE_PATH:file,DOC126_E2E_MODE:mode,
  DOC126_E2E_AUTHORIZED:(mode==='assistant'?environment.DOC126_ASSISTANT_E2E_AUTHORIZED:mode==='functional'?environment.DOC126_FUNCTIONAL_E2E_AUTHORIZED:mode==='resilience'?environment.DOC126_RESILIENCE_E2E_AUTHORIZED:environment.DOC126_E2E_AUTHORIZED)==='YES'?'YES':'NO',DOC10_E2E_IGNORE_HTTPS_ERRORS:String(profile.ignoreHttpsErrors===true)};
 if(mode==='assistant')values.DOC126_ASSISTANT_E2E_AUTHORIZED=values.DOC126_E2E_AUTHORIZED;
 if(mode==='functional')values.DOC126_FUNCTIONAL_E2E_AUTHORIZED=values.DOC126_E2E_AUTHORIZED;
 if(mode==='resilience')values.DOC126_RESILIENCE_E2E_AUTHORIZED=values.DOC126_E2E_AUTHORIZED;
 const aliases={DOC126_E2E_USER:'DOC110_E2E_USER',DOC126_E2E_PASSWORD:'DOC110_E2E_PASSWORD',DOC126_E2E_RESTRICTED_USER:'DOC110_E2E_SECOND_USER',DOC126_E2E_RESTRICTED_PASSWORD:'DOC110_E2E_SECOND_PASSWORD'};
 if(require('../tests/support/doc126-excel-scope.cjs').usesExistingAccount(scope)){delete aliases.DOC126_E2E_RESTRICTED_USER;delete aliases.DOC126_E2E_RESTRICTED_PASSWORD;}
 for(const [key,alias]of Object.entries(aliases))if(environment[key]||environment[alias])values[key]=environment[key]||environment[alias];
 const controls=[scope.controlPrefix,scope.raControlPrefix].flatMap(p=>['ODBC_DSN','MYSQL_USER','MYSQL_PASSWORD'].map(k=>p+'_'+k));
 for(const key of controls)if(environment[key])values[key]=environment[key];
 const childArgs=[path.join(root,'node_modules/@playwright/test/cli.js'),'test','tests/importacion-excel.spec.cjs','--grep','@doc126-ui-'+mode,'--reporter=list','--retries=0'];
 if(args.includes('--headed'))childArgs.push('--headed');
 return {values,needed:[...Object.keys(aliases),...controls],scope,mode,childArgs};
}
async function runExcelUi(root,profile,args){
 const plan=excelUiPlan(root,profile,args),values=plan.values;let environment;
 const support=require('../tests/support/doc126-excel-scope.cjs');support.verifyRuntime(plan.scope);
 if(['write','functional','resilience'].includes(plan.mode))for(const item of plan.scope.cases)support.readFixture(item);
 if(plan.mode==='assistant')support.readFixture(plan.scope.assistant);
 try{
  for(const name of plan.needed)if(!values[name]){requireInteractiveConsole();await collectValue(values,name,name.includes('PASSWORD')?'Contraseña efímera':name.endsWith('_ODBC_DSN')?'DSN existente del control SELECT':'Cuenta autorizada DOC126',{secret:true});}
  if(values.DOC126_E2E_AUTHORIZED!=='YES'){
   requireInteractiveConsole();await collectConfirmation(values,'DOC126_CONFIRMACION',plan.mode==='resilience'?'Autoriza DOC126 rollback de300 filas sobre1000 de fixture y carrera de10 sobre1001 con una sola fila externa; conservar todo, sin instalar/limpiar/cambiar permisos.':plan.mode==='functional'?'Autoriza DOC126 funcional con la cuenta existente: agregar 1000 filas y reemplazar dos filas de fixture por 1000; sólo los dos destinos DOC126 del scope, sin cambiar permisos, guardar plantillas, instalar ni limpiar. No acredita permiso exclusivo.':plan.mode==='assistant'?'Autoriza DOC126 asistente: una plantilla nueva de dos campos, destino '+plan.scope.assistant.tabla+', 1000 filas y resultado; cero DELETE, sin instalar/sembrar/limpiar y conservar todos los efectos.':plan.mode==='write'?'Autoriza DOC126 UI: cinco casos, sólo destinos imp_02_doc126_e2e_ del scope, CREATE funcional/INSERT/DELETE/resultado y una fila externa; máximos '+plan.scope.maxBusinessRows+'/'+plan.scope.maxDeletedRows+'; conservar todo sin instalar/sembrar/limpiar.':'Autoriza DOC126 UI: acceso exclusivo/restringido, navegación y consultas de sólo lectura; cero ediciones y confirmaciones.');values.DOC126_E2E_AUTHORIZED='YES';
   if(plan.mode==='assistant')values.DOC126_ASSISTANT_E2E_AUTHORIZED='YES';
   if(plan.mode==='functional')values.DOC126_FUNCTIONAL_E2E_AUTHORIZED='YES';
   if(plan.mode==='resilience')values.DOC126_RESILIENCE_E2E_AUTHORIZED='YES';
  }
  if(['write','resilience'].includes(plan.mode)){const directory=fs.mkdtempSync(path.join(root,'../../.tmp/doc125-native-'));values.DOC125_NATIVE_HARNESS=require('../../../tests/doc122-importacion-excel-behavior.test.cjs').compile(directory).exe;}
  environment={...process.env,...values};const result=await runChild(process.execPath,plan.childArgs,root,environment);process.exitCode=result.code;
 }finally{for(const key of Object.keys(values)){delete values[key];if(environment)delete environment[key];}}
}

function excelConfirmationPlan(root,profile,args,environment=process.env){
 if(!args.includes('--excel-confirm')||args.some(x=>!['--excel-confirm','--headed'].includes(x))||args.filter(x=>x==='--excel-confirm').length!==1)throw Error('DOC125_MODE_CONFLICT');
 const scopePath=environment.DOC125_E2E_SCOPE_PATH;
 if(!scopePath||!fs.existsSync(scopePath))throw Error('DOC125_SCOPE_REQUIRED');
 const s=require('../tests/support/doc125-excel-scope.cjs').readScope(scopePath,profile.baseUrl);
 const values={DOC125_E2E_BASE_URL:profile.baseUrl,DOC125_E2E_MODULE:profile.module,DOC125_E2E_SCOPE_PATH:scopePath,DOC125_E2E_AUTHORIZED:environment.DOC125_E2E_AUTHORIZED==='YES'?'YES':'NO',DOC10_E2E_IGNORE_HTTPS_ERRORS:String(profile.ignoreHttpsErrors===true)};
 const aliases={DOC125_E2E_USER:'DOC110_E2E_USER',DOC125_E2E_PASSWORD:'DOC110_E2E_PASSWORD',DOC125_E2E_RESTRICTED_USER:'DOC110_E2E_SECOND_USER',DOC125_E2E_RESTRICTED_PASSWORD:'DOC110_E2E_SECOND_PASSWORD'};
 for(const [key,alias]of Object.entries(aliases))if(environment[key]||environment[alias])values[key]=environment[key]||environment[alias];
 const controls=['ODBC_DSN','MYSQL_USER','MYSQL_PASSWORD'].map(x=>s.controlPrefix+'_'+x);
 for(const key of controls)if(environment[key])values[key]=environment[key];
 const childArgs=[path.join(root,'node_modules/@playwright/test/cli.js'),'test','tests/importacion-excel.spec.cjs','--grep','@doc125-confirm','--reporter=list','--retries=0'];
 if(args.includes('--headed'))childArgs.push('--headed');
 return {values,needed:[...Object.keys(aliases),...controls],childArgs,scope:s};
}
async function runExcelConfirmation(root,profile,args){
 const plan=excelConfirmationPlan(root,profile,args),values=plan.values;let environment;
 try{for(const item of plan.scope.cases)require('../tests/support/doc125-excel-scope.cjs').readFixture(item);}catch{throw Error('DOC125_FIXTURE_INVALID');}
 try{
  for(const name of plan.needed){if(values[name])continue;requireInteractiveConsole();await collectValue(values,name,name.includes('PASSWORD')?'Contraseña efímera':name.endsWith('_ODBC_DSN')?'DSN del control SELECT existente':'Cuenta autorizada para DOC125',{secret:true});}
  if(values.DOC125_E2E_AUTHORIZED!=='YES'){
   requireInteractiveConsole();await collectConfirmation(values,'DOC125_CONFIRMACION','Autoriza DOC125: cinco casos sobre tablas imp_02_doc125_e2e_ descartables del manifiesto, CREATE funcional, INSERT/DELETE y resultado, una fila de escritor externo, máximo '+plan.scope.maxBusinessRows+' filas previstas y '+plan.scope.maxDeletedRows+' eliminaciones de fixture; conservar todos los efectos, sin instalar configuración ni limpiar; preservar CONTRATOS y destinos protegidos');values.DOC125_E2E_AUTHORIZED='YES';
  }
  const directory=fs.mkdtempSync(path.join(root,'../../.tmp/doc125-native-'));
  values.DOC125_NATIVE_HARNESS=require('../../../tests/doc122-importacion-excel-behavior.test.cjs').compile(directory).exe;
  environment={...process.env,...values};const result=await runChild(process.execPath,plan.childArgs,root,environment);process.exitCode=result.code;
 }finally{for(const name of Object.keys(values)){delete values[name];if(environment)delete environment[name];}}
}
function startExcelExternalWriter(scope,item,prepared,environment=process.env){
 const ui=scope.issue==='DOC-126';
 if(ui)require('../tests/support/doc126-excel-scope.cjs').validateScope(scope,scope.baseUrl,scope.mode==='resilience'?'resilience':'write');else require('../tests/support/doc125-excel-scope.cjs').validateScope(scope,scope.baseUrl);
 if(ui&&scope.mode==='resilience'&&environment.DOC126_RESILIENCE_E2E_AUTHORIZED!=='YES')throw Error('DOC125_NATIVE_SCOPE_REQUIRED');
 if(environment[ui?'DOC126_E2E_AUTHORIZED':'DOC125_E2E_AUTHORIZED']!=='YES'||item.escenario!=='carrera'||!scope.cases.includes(item)||!environment.DOC125_NATIVE_HARNESS||
  !/^[a-f0-9-]{36}$/.test(prepared.IdCarga||'')||!['ArchivoHash','ConfiguracionHash','EsquemaHash'].every(k=>/^[a-f0-9]{64}$/.test(prepared[k]||''))||prepared.ArchivoHash!==item.archivoHash)throw Error('DOC125_NATIVE_SCOPE_REQUIRED');
 const crypto=require('node:crypto'),{spawn}=require('node:child_process');
 const parent=path.resolve(__dirname,'../../../.tmp/doc125-external');fs.mkdirSync(parent,{recursive:true});
 const directory=fs.mkdtempSync(path.join(parent,'run-')),file=path.join(directory,'resources.json');
 const body=JSON.stringify({scope,item,prepared});fs.writeFileSync(file,body,{flag:'wx'});
 const child=spawn('powershell.exe',['-NoProfile','-NonInteractive','-ExecutionPolicy','Bypass','-File',path.join(__dirname,'support/doc115-repository-integration.ps1'),'-ResourcesFile',file,'-RuntimeRoot',scope.runtimeRoot,'-ExcelConfirmation'],{windowsHide:true,env:{...environment,DOC125_NATIVE_RESOURCES_SHA256:crypto.createHash('sha256').update(body).digest('hex')}});
 let readyResolve,readyReject,doneResolve,doneReject,buffer='',errorBuffer='',diagnostic=null,locked=false,committed=false,importWaitObserved=false,activeLocksObserved=false;
 const ready=new Promise((r,j)=>{readyResolve=r;readyReject=j;}),done=new Promise((r,j)=>{doneResolve=r;doneReject=j;});ready.catch(()=>{});done.catch(()=>{});
 const timer=setTimeout(()=>{child.kill();readyReject(Error('DOC125_NATIVE_RESULT_UNCERTAIN'));doneReject(Error('DOC125_NATIVE_RESULT_UNCERTAIN'));},60000);
 child.stdout.on('data',chunk=>{buffer+=String(chunk);let index;while((index=buffer.indexOf('\n'))>=0){const line=buffer.slice(0,index).trim();buffer=buffer.slice(index+1);if(line==='DOC125_EXTERNAL_LOCKED'){locked=true;readyResolve();}else if(line==='DOC125_EXTERNAL_IMPORT_WAIT_OBSERVED')importWaitObserved=true;else if(line==='DOC125_EXTERNAL_ACTIVE_LOCKS_OBSERVED')activeLocksObserved=true;else if(line==='DOC125_EXTERNAL_COMMITTED')committed=true;}if(buffer.length>1024)buffer='';});
 child.stderr.on('data',chunk=>{errorBuffer+=String(chunk);let index;while((index=errorBuffer.indexOf('\n'))>=0){const line=errorBuffer.slice(0,index).trim();errorBuffer=errorBuffer.slice(index+1);if(!line.startsWith('DOC125_NATIVE_DIAGNOSTIC '))continue;try{const d=JSON.parse(line.slice('DOC125_NATIVE_DIAGNOSTIC '.length));if(/^DOC(?:115|125)_[A-Z_]{1,90}$/.test(d.code||'')&&/^[A-Z_]{1,40}$/.test(d.stage||'')&&/^[A-Za-z0-9_]{1,80}$/.test(d.type||''))diagnostic={code:d.code,stage:d.stage,type:d.type,native:Number.isSafeInteger(d.native)?d.native:null};}catch{}}if(errorBuffer.length>2048)errorBuffer='';});
 child.on('error',()=>{clearTimeout(timer);readyReject(Error('DOC125_NATIVE_RESULT_UNCERTAIN'));doneReject(Error('DOC125_NATIVE_RESULT_UNCERTAIN'));});
 child.on('close',code=>{clearTimeout(timer);fs.writeFileSync(path.join(directory,'outcome.json'),JSON.stringify({locked,importWaitObserved,activeLocksObserved,committed,diagnostic,exitCode:code,effectsPreserved:true}));if(code===0&&locked&&importWaitObserved&&committed&&(scope.mode!=='resilience'||activeLocksObserved))doneResolve();else{readyReject(Error('DOC125_NATIVE_RESULT_UNCERTAIN'));doneReject(Error('DOC125_NATIVE_RESULT_UNCERTAIN'));}});
 return {ready,done};
}

//DOC124 conserva autenticación/consola/perfil; autorización y fixtures son propios.
function excelPreparationPlan(root,profile,args,environment=process.env){
  const scenarios={'--excel-prepare':'matriz','--excel-file':'archivo','--excel-add':'agregar','--excel-replace':'reemplazar'};
  const modes=args.filter(x=>Object.hasOwn(scenarios,x));
  if(modes.length!==1||args.some(x=>!Object.hasOwn(scenarios,x)&&x!=='--headed'))throw new Error('DOC124_MODE_CONFLICT');
  const fileMode=modes[0]==='--excel-file';
  const scopePath=environment.DOC124_E2E_SCOPE_PATH;
  if(!scopePath||!fs.existsSync(scopePath))throw new Error('DOC124_SCOPE_REQUIRED');
  let scope;try{scope=JSON.parse(fs.readFileSync(scopePath,'utf8').replace(/^\uFEFF/,''));}catch{throw new Error('DOC124_SCOPE_INVALID');}
  require('../tests/support/doc124-excel-scope.cjs').validateScope(scope,profile.baseUrl);
  const scenario=scenarios[modes[0]];
  if(scenario!==(scope.escenario||'matriz'))throw new Error('DOC124_SCOPE_INVALID');
  const values={DOC124_E2E_BASE_URL:profile.baseUrl,DOC124_E2E_MODULE:profile.module,DOC124_E2E_SCOPE_PATH:scopePath,
    DOC124_E2E_AUTHORIZED:environment.DOC124_E2E_AUTHORIZED==='YES'?'YES':'NO',DOC10_E2E_IGNORE_HTTPS_ERRORS:String(profile.ignoreHttpsErrors===true)};
  const aliases={DOC124_E2E_USER:'DOC110_E2E_USER',DOC124_E2E_PASSWORD:'DOC110_E2E_PASSWORD',DOC124_E2E_RESTRICTED_USER:'DOC110_E2E_SECOND_USER',DOC124_E2E_RESTRICTED_PASSWORD:'DOC110_E2E_SECOND_PASSWORD'};
  for(const [name,alias]of Object.entries(aliases))if(environment[name]||environment[alias])values[name]=environment[name]||environment[alias];
  const controlNames=['ODBC_DSN','MYSQL_USER','MYSQL_PASSWORD'].map(suffix=>scope.controlPrefix+'_'+suffix);
  for(const name of controlNames)if(environment[name])values[name]=environment[name];
  const tag={'matriz':'prepare','archivo':'file','agregar':'add','reemplazar':'replace'}[scenario];
  const childArgs=[path.join(root,'node_modules/@playwright/test/cli.js'),'test','tests/importacion-excel.spec.cjs','--grep','@doc124-'+tag,'--reporter=list','--retries=0'];
  if(args.includes('--headed'))childArgs.push('--headed');
  return {values,needed:[...Object.keys(aliases),...controlNames],childArgs,cases:scope.cases.length,mode:fileMode?'file':scenario==='matriz'?'prepare':tag};
}
async function runExcelPreparation(root,profile,args){
  const plan=excelPreparationPlan(root,profile,args),values=plan.values;let environment;
  try{
    for(const name of plan.needed){if(values[name])continue;requireInteractiveConsole();await collectValue(values,name,name.includes('PASSWORD')?'Contraseña efímera':name.endsWith('_ODBC_DSN')?'DSN del control SELECT existente':'Cuenta de prueba autorizada para Excel',{secret:true});}
    if(values.DOC124_E2E_AUTHORIZED!=='YES'){
      requireInteractiveConsole();await collectConfirmation(values,'DOC124_CONFIRMACION','Autoriza DOC-124: '+plan.cases+' casos de archivo privado/ejecución/temporales, dos sesiones del propietario y cuenta restringida; controles SELECT; sin importar, crear destino, guardar plantilla o limpiar datos');values.DOC124_E2E_AUTHORIZED='YES';
    }
    environment={...process.env,...values};const result=await runChild(process.execPath,plan.childArgs,root,environment);process.exitCode=result.code;
  }finally{for(const name of Object.keys(values)){delete values[name];if(environment)delete environment[name];}}
}

// DOC-122 comparte el perfil local y la consola efímera de esta misma suite.
function excelProfilePlan(root,profile,args,environment=process.env){
  const modes=args.filter(arg=>arg==='--excel-read'||arg==='--excel-write');
  if(modes.length!==1||args.some(arg=>!['--excel-read','--excel-write','--headed'].includes(arg)))throw new Error('DOC122_MODE_CONFLICT');
  const mode=modes[0]==='--excel-read'?'read':'write';
  const scopePath=environment.DOC122_E2E_SCOPE_PATH;
  if(!scopePath||!fs.existsSync(scopePath))throw new Error('DOC122_SCOPE_REQUIRED');
  let scope;try{scope=JSON.parse(fs.readFileSync(scopePath,'utf8').replace(/^\uFEFF/,''));}catch{throw new Error('DOC122_SCOPE_INVALID');}
  if(!scope||scope.issue!=='DOC-122'||scope.disposable!==true||scope.baseUrl!==profile.baseUrl||
     !Number.isSafeInteger(scope.conexionId)||scope.conexionId<1||
     !Number.isSafeInteger(scope.plantillaId)||scope.plantillaId<1||
     !Number.isSafeInteger(scope.revisionEsperada)||scope.revisionEsperada<1||
     !/^[0-9a-f]{64}$/.test(scope.huellaEsperada||'')||
     !Array.isArray(scope.allowedModes)||!scope.allowedModes.includes(mode)||
     scope.allowedModes.some(value=>!['read','write'].includes(value)))throw new Error('DOC122_SCOPE_INVALID');
  const values={
    DOC122_E2E_BASE_URL:profile.baseUrl,DOC122_E2E_MODULE:profile.module,
    DOC122_E2E_SCOPE_PATH:scopePath,
    DOC122_E2E_AUTHORIZED:environment.DOC122_E2E_AUTHORIZED==='YES'?'YES':'NO',
    DOC10_E2E_IGNORE_HTTPS_ERRORS:String(profile.ignoreHttpsErrors===true)
  };
  const aliases={DOC122_E2E_USER:'DOC110_E2E_USER',DOC122_E2E_PASSWORD:'DOC110_E2E_PASSWORD'};
  if(mode==='read'){
    aliases.DOC122_E2E_RESTRICTED_USER='DOC110_E2E_SECOND_USER';
    aliases.DOC122_E2E_RESTRICTED_PASSWORD='DOC110_E2E_SECOND_PASSWORD';
  }
  for(const [name,alias]of Object.entries(aliases))if(environment[name]||environment[alias])values[name]=environment[name]||environment[alias];
  const childArgs=[path.join(root,'node_modules/@playwright/test/cli.js'),'test','tests/importacion-excel.spec.cjs','--grep','@doc122-'+mode,'--reporter=list','--retries=0'];
  if(args.includes('--headed'))childArgs.push('--headed');
  return {values,needed:Object.keys(aliases),childArgs,plantillaId:scope.plantillaId,mode};
}
async function runExcelProfileSession(root,profile,args){
  const plan=excelProfilePlan(root,profile,args),values=plan.values;
  let environment;
  try{
    for(const name of plan.needed){
      if(values[name])continue;
      requireInteractiveConsole();
      await collectValue(values,name,name.includes('PASSWORD')?'Contraseña efímera':'Cuenta de prueba autorizada para Excel',{secret:true});
    }
    if(values.DOC122_E2E_AUTHORIZED!=='YES'){
      requireInteractiveConsole();
      await collectConfirmation(values,'DOC122_CONFIRMACION',
        'Autoriza DOC-122 en el ambiente del perfil local, con las cuentas seleccionadas y plantilla descartable '+plan.plantillaId+
        (plan.mode==='write'?' (tres solicitudes de guardado: dos éxitos y un conflicto; revisión +2)':' (lectura y rechazo de acceso; sin guardados)'));
      values.DOC122_E2E_AUTHORIZED='YES';
    }
    environment={...process.env,...values};
    const result=await runChild(process.execPath,plan.childArgs,root,environment);
    process.exitCode=result.code;
  }finally{
    for(const name of Object.keys(values)){delete values[name];if(environment)delete environment[name];}
  }
}
async function main(){
  if(process.argv.some(x=>['--excel-ui-read','--excel-ui-write','--excel-ui-assistant','--excel-ui-functional','--excel-ui-resilience'].includes(x))){
    const root=path.resolve(__dirname,'..');const profile=JSON.parse(fs.readFileSync(path.join(root,'profiles/doc88-production-document-upload.local.runtime.json'),'utf8').replace(/^\uFEFF/,''));
    return runExcelUi(root,profile,process.argv.slice(2));
  }
  if(process.argv.includes('--excel-confirm')){
    const root=path.resolve(__dirname,'..');const profile=JSON.parse(fs.readFileSync(path.join(root,'profiles/doc88-production-document-upload.local.runtime.json'),'utf8').replace(/^\uFEFF/,''));
    return runExcelConfirmation(root,profile,process.argv.slice(2));
  }
  if(process.argv.some(x=>['--excel-prepare','--excel-file','--excel-add','--excel-replace'].includes(x))){
    const root=path.resolve(__dirname,'..');const profile=JSON.parse(fs.readFileSync(path.join(root,'profiles/doc88-production-document-upload.local.runtime.json'),'utf8').replace(/^\uFEFF/,''));
    return runExcelPreparation(root,profile,process.argv.slice(2));
  }
  if(process.argv.some(arg=>arg==='--excel-read'||arg==='--excel-write')){
    const root=path.resolve(__dirname,'..');
    const profile=JSON.parse(fs.readFileSync(path.join(root,'profiles/doc88-production-document-upload.local.runtime.json'),'utf8').replace(/^\uFEFF/,''));
    return runExcelProfileSession(root,profile,process.argv.slice(2));
  }
  if(['--missing-schema','--acceptance','--draft','--publish','--concurrent','--concurrent-publish','--isolation','--permission-revocation','--session-expiry','--source-scope','--source-profile','--source-guards','--source-transactions','--provider-diagnostics','--provider-scope','--inheritance','--matching','--guards','--tabs','--updatepanel','--provider-lifecycle','--provider-profile','--provider-guards','--provider-concurrent','--provider-availability'].filter(flag=>process.argv.includes(flag)).length>1)throw new Error('DOC110_MODE_CONFLICT');
  const tableIndex=process.argv.indexOf('--source-table');
  if(['--resume-source','--cleanup-source','--continue-source'].filter(flag=>process.argv.includes(flag)).length>1)throw new Error('DOC110_SOURCE_ACTION_CONFLICT');
  if(process.argv.includes('--continue-source')&&!process.argv.includes('--source-profile'))throw new Error('DOC110_SOURCE_CONTINUE_REQUIRES_SOURCE_PROFILE');
  if(process.argv.includes('--cleanup-source')&&!process.argv.includes('--source-profile'))throw new Error('DOC110_SOURCE_CLEANUP_REQUIRES_SOURCE_PROFILE');
  if(process.argv.includes('--resume-source')&&!process.argv.includes('--source-profile'))throw new Error('DOC110_SOURCE_RESUME_REQUIRES_SOURCE_PROFILE');
  if(tableIndex!==-1&&!['--source-profile','--source-guards','--provider-diagnostics'].some(flag=>process.argv.includes(flag)))throw new Error('DOC110_SOURCE_TABLE_REQUIRES_SOURCE_PROFILE');
  const fixture=sourceFixture(tableIndex===-1?undefined:(process.argv[tableIndex+1]||''));
  const root=path.resolve(__dirname,'..');
  const profile=JSON.parse(fs.readFileSync(path.join(root,'profiles/doc88-production-document-upload.local.runtime.json'),'utf8').replace(/^\uFEFF/,''));
  const values={
    DOC110_E2E_BASE_URL:profile.baseUrl,
    DOC110_E2E_MODULE:profile.module,
    DOC110_E2E_DRAFT_LIFECYCLE:['--draft','--concurrent','--guards','--tabs'].some(flag=>process.argv.includes(flag))?'YES':'NO',
    DOC110_E2E_TABS:process.argv.includes('--tabs')?'YES':'NO',
    DOC110_E2E_INDEPENDENT_SESSIONS:['--concurrent','--concurrent-publish','--guards'].some(flag=>process.argv.includes(flag))?'YES':'NO',
    DOC110_E2E_GUARDS:process.argv.includes('--guards')?'YES':'NO',
    DOC110_E2E_PUBLICATION_LIFECYCLE:process.argv.includes('--publish') || process.argv.includes('--concurrent-publish') || process.argv.includes('--inheritance') || process.argv.includes('--matching')?'YES':'NO',
    DOC110_E2E_MATCHING:process.argv.includes('--matching')?'YES':'NO',
    DOC110_E2E_INHERITANCE:process.argv.includes('--inheritance')?'YES':'NO',
    DOC10_E2E_IGNORE_HTTPS_ERRORS:String(profile.ignoreHttpsErrors===true)
  };
  const needed=['DOC110_E2E_USER','DOC110_E2E_PASSWORD'];
  values.DOC110_E2E_MISSING_SCHEMA=process.argv.includes('--missing-schema')?'YES':'NO';
  if(values.DOC110_E2E_MISSING_SCHEMA==='YES')
    console.log('DOC-110 SIN ESQUEMA: autorizacion del usuario para usar las tablas del ambiente de prueba. Renombra temporalmente SOLO las tres tablas de perfiles, exige 503/aviso y carga un PDF sintetico en el expediente descartable 467 con el flujo oficial DOC-88. Restaura nombres y verifica hashes de datos/esquema/identidades en finally. No borra tablas, cambia permisos ni modifica SII. No ejecutar otra consola de pruebas simultaneamente.');
  values.DOC110_E2E_ACCEPTANCE=process.argv.includes('--acceptance')?'YES':'NO';
  if(values.DOC110_E2E_ACCEPTANCE==='YES')
    console.log('DOC-110 PER-01/02/03: crea dos perfiles descartables (borrador y dos publicaciones EXPEDIENTE/LOTE) y los desactiva. Reutiliza retirada temporal/restauracion autorizada SOLO del permiso de perfiles de LUZ.AGUILERA para comprobar guardar/publicar denegados. Registra SHA de codigo/DLL, UI/HTTP y controles SELECT de esos perfiles. No carga documentos ni modifica fuentes/SII.');
  values.DOC110_E2E_PERMISSION_REVOCATION=process.argv.includes('--permission-revocation')?'YES':'NO';
  if(values.DOC110_E2E_PERMISSION_REVOCATION==='YES')
    console.log('DOC-110 PERMISO: autorizacion explicita del usuario para retirar temporalmente SOLO ADMINISTRAR_PERFILES_IMPORTACION de LUZ.AGUILERA y restaurarlo. Comprueba rechazo 403 en sesion vigente, UI sin editor, recuperacion de acceso sin relogin y perfil/historial intactos. Usa RA del modulo configurado; exige una fila inequivoca y permiso inicial 1. No modifica otros campos ni SII.');
  values.DOC110_E2E_SESSION_EXPIRY=process.argv.includes('--session-expiry')?'YES':'NO';
  if(values.DOC110_E2E_SESSION_EXPIRY==='YES'){
    const {sessionIdleMinutes}=require('../tests/support/doc110-session-expiry.cjs');
    values.DOC110_E2E_SESSION_IDLE_MINUTES=String(sessionIdleMinutes(fs.readFileSync(path.resolve(root,'../../Web.config'),'utf8')));
    console.log('DOC-110 SESION: dos sesiones de la cuenta autorizada. Comprueba token ajeno y deja una sin paginas/solicitudes durante '+values.DOC110_E2E_SESSION_IDLE_MINUTES+' min + 75 s; mantiene activa solo la sesion control con lecturas. Exige 401 tras inactividad y perfil/historial intactos. No cambia timeout/permisos/datos ni borra cookies para simular expiracion.');
  }
  values.DOC110_E2E_PROVIDER_LIFECYCLE=['--provider-lifecycle','--provider-profile','--provider-guards','--provider-concurrent','--provider-availability'].some(flag=>process.argv.includes(flag))?'YES':'NO';
  values.DOC110_E2E_PROVIDER_CONCURRENT=['--provider-concurrent','--provider-availability'].some(flag=>process.argv.includes(flag))?'YES':'NO';
  values.DOC110_E2E_PROVIDER_AVAILABILITY=process.argv.includes('--provider-availability')?'YES':'NO';
  if(values.DOC110_E2E_PROVIDER_AVAILABILITY==='YES')console.log('DOC-110: activa temporalmente SOLO el proveedor descartable mediante dos sesiones concurrentes y lo desactiva en finally; conserva historial/versiones.');
  if(values.DOC110_E2E_PROVIDER_CONCURRENT==='YES')console.log('DOC-110: dos sesiones independientes compiten por guardar, publicar y crear version del proveedor descartable INACTIVO. Exige un ganador y un conflicto por operacion; no modifica fuentes ni origen.');
  values.DOC110_E2E_PROVIDER_GUARDS=process.argv.includes('--provider-guards')?'YES':'NO';
  if(values.DOC110_E2E_PROVIDER_GUARDS==='YES')
    console.log('DOC-110: verifica 33 rechazos en nueve endpoints de proveedor por CSRF ausente/invalido, sin sesion y revision obsoleta; exige documento inalterado tras cada intento. El ciclo UI crea/publica SOLO un proveedor descartable INACTIVO; no modifica fuentes ni perfiles.');
  values.DOC110_E2E_SOURCE_PROFILE=process.argv.includes('--source-profile')?'YES':'NO';
  values.DOC110_E2E_SOURCE_TRANSACTIONS=process.argv.includes('--source-transactions')?'YES':'NO';
  values.DOC110_E2E_SOURCE_TRANSACTIONS_AUTHORIZED=values.DOC110_E2E_SOURCE_TRANSACTIONS==='YES'?'true':'false';
  if(values.DOC110_E2E_SOURCE_TRANSACTIONS==='YES')
    console.log('DOC-110 TRANSACCIONES: verifica que DA imp_02_facturas no tenga autorizacion previa, crea desde UI una fuente NUEVA INACTIVA con NIT/NUMERO y prueba duplicado y edicion desde dos sesiones independientes. Nunca activa la fuente. No crea tablas/proveedores/perfiles ni consulta filas DA; solo metadatos. Si la fuente ya existe, se detiene sin modificarla.');
  values.DOC110_E2E_PROVIDER_DIAGNOSTICS=process.argv.includes('--provider-diagnostics')?'YES':'NO';
  values.DOC110_E2E_SOURCE_GUARDS=process.argv.includes('--source-guards')?'YES':'NO';
  if(values.DOC110_E2E_SOURCE_GUARDS==='YES')console.log('DOC-110 GUARD FUENTES: solicitudes que deben rechazarse por CSRF/sesion/revision/contrato. Usa fixture inactivo de corrida PASS, revision antigua e ID conservado en Crear para impedir alta. Comprueba documento/historial despues de cada intento; no concede permisos ni consulta DA.');
  if(values.DOC110_E2E_PROVIDER_DIAGNOSTICS==='YES')console.log('DOC-110 DIAGNOSTICO: solo lecturas. Reabre proveedor E2E publicado inactivo cuya fuente esta revocada; verifica aviso y referencias originales sin sustituir, guardar ni publicar.');
  values.DOC110_E2E_SOURCE_TABLE=fixture.table;
  values.DOC110_E2E_SOURCE_CLEANUP=process.argv.includes('--cleanup-source')?'YES':'NO';
  values.DOC110_E2E_SOURCE_CONTINUE=process.argv.includes('--continue-source')?'YES':'NO';
  values.DOC110_E2E_SOURCE_RESUME=process.argv.includes('--resume-source')?'YES':'NO';
  values.DOC110_E2E_SOURCE_CATALOG_AUTHORIZED='false';
  values.DOC110_E2E_PROVIDER_PROFILE=['--provider-profile','--source-profile'].some(flag=>process.argv.includes(flag))?'YES':'NO';
  if(values.DOC110_E2E_SOURCE_CLEANUP==='YES')values.DOC110_E2E_PROVIDER_PROFILE='NO';
  if(values.DOC110_E2E_SOURCE_PROFILE==='YES'&&values.DOC110_E2E_SOURCE_CLEANUP!=='YES'&&values.DOC110_E2E_SOURCE_CONTINUE!=='YES')
    console.log('DOC-110: registra DA '+fixture.table+' desde UI solo si no existe autorizacion previa; activa, edita columnas, conecta proveedor/perfil y revoca SOLO la nueva fuente al finalizar. Si ya existe, se detiene sin modificarla. No escribe en DA. Requiere autorizacion especifica adicional.');
  if(values.DOC110_E2E_SOURCE_RESUME==='YES')
    console.log('DOC-110 REANUDACION: no crea otra fuente. Solo permite continuar una fuente inactiva revision 1, con un unico ALTA que coincida con identidad, columnas, motivo y ventana UTC del reporte fallido local. Si no coincide, aborta sin escribir.');
  if(values.DOC110_E2E_SOURCE_CONTINUE==='YES')
    console.log('DOC-110 CONTINUACION: exige reporte de recuperacion PASS y fuente propia inactiva revision 4 con alta/activacion/edicion/revocacion verificadas. Reactiva revision 5, ejecuta proveedor/perfil y revoca revision 6. No repite alta ni edicion; conserva historial anterior.');
  if(values.DOC110_E2E_SOURCE_CLEANUP==='YES')console.log('DOC-110 RECUPERACION: solo verifica estado/historial de la fuente identificada por el reporte de la prueba fallida y revoca si permanece activa. No crea fuentes/proveedores/perfiles ni consulta DA.');
  if(values.DOC110_E2E_SOURCE_PROFILE==='YES'&&values.DOC110_E2E_SOURCE_CLEANUP!=='YES'&&!fixture.ambiguousKey)needed.push('DOC110_E2E_AMBIGUOUS_KEY');
  if(values.DOC110_E2E_PROVIDER_PROFILE==='YES'){
    needed.push('DOC110_E2E_UNIQUE_KEY');
    console.log('DOC-110: activa temporalmente SOLO el proveedor descartable, lo conecta por UI a un perfil descartable y verifica el indice CONSECUTIVO. Lo desactiva para comprobar rechazo de publicacion sin cambios al perfil/historial y lo restaura con revision vigente para continuar. Desactiva proveedor al finalizar.');
  }
  if(values.DOC110_E2E_PROVIDER_LIFECYCLE==='YES' && values.DOC110_E2E_PROVIDER_CONCURRENT!=='YES')
    console.log('DOC-110: crea proveedor descartable INACTIVO desde UI para DA imp_02_egresos/CONSECUTIVO, prueba ausente y ambiguo (44), publica dos versiones. No modifica fuentes ni registros de origen.');
  values.DOC110_E2E_UPDATEPANEL=process.argv.includes('--updatepanel')?'YES':'NO';
  if(values.DOC110_E2E_UPDATEPANEL==='YES')
    console.log('DOC-110: dos actualizaciones parciales reales del listado y reapertura del editor por teclado. No crea perfiles, expedientes ni documentos.');
  values.DOC110_E2E_PROVIDER_SCOPE=process.argv.includes('--provider-scope')?'YES':'NO';
  if(values.DOC110_E2E_PROVIDER_SCOPE==='YES')
    console.log('DOC-110: metadatos RA/DA restringidos a imp_02 y validacion de proveedores ya guardados fuera del prefijo, sin excepciones. Comprueba documento intacto y fixture legado sintetico inactivo cuando hay reporte de preparacion. Esta corrida no crea proveedores ni consulta filas de negocio. Si no hay candidato registra NO EJECUTADO.');
  values.DOC110_E2E_SOURCE_SCOPE=process.argv.includes('--source-scope')?'YES':'NO';
  if(values.DOC110_E2E_SOURCE_SCOPE==='YES')
    console.log('DOC-110: solo metadatos RA/DA; verifica prefijo literal imp_02 sin excepciones y rechazo de tablas ajenas. No consulta registros ni modifica fuentes.');
  values.DOC110_E2E_ACCESS_ISOLATION=process.argv.includes('--isolation')?'YES':'NO';
  if(values.DOC110_E2E_ACCESS_ISOLATION==='YES'){
    needed.push('DOC110_E2E_SECOND_USER','DOC110_E2E_SECOND_PASSWORD');
    console.log('DOC-110: solo lecturas con dos cuentas autorizadas. La segunda debe carecer de acceso a perfiles y administracion de proveedores. Comprueba listado, detalle y metadatos; no modifica permisos, fuentes ni proveedores. No acredita aislamiento entre empresas.');
  }
  if(values.DOC110_E2E_INDEPENDENT_SESSIONS==='YES')
    console.log('DOC-110: dos sesiones independientes de la cuenta autorizada; prueba revisiones concurrentes. No acredita aislamiento entre cuentas o empresas.');
  if(values.DOC110_E2E_PUBLICATION_LIFECYCLE==='YES')
    console.log('DOC-110: consulta catálogos autorizados, crea un perfil descartable, publica dos versiones y lo desactiva. No crea expedientes ni documentos.');
  if(values.DOC110_E2E_DRAFT_LIFECYCLE==='YES')
    console.log('DOC-110: crea un perfil descartable, prueba guardado/reglas/concurrencia y lo desactiva al completar. No carga documentos.');
  for(const name of needed)if(process.env[name])values[name]=process.env[name];
  if(needed.some(name=>!values[name])){
    requireInteractiveConsole();
    if(!values.DOC110_E2E_USER)await collectValue(values,'DOC110_E2E_USER','Cuenta de prueba autorizada para DOC-110');
    if(!values.DOC110_E2E_PASSWORD)await collectValue(values,'DOC110_E2E_PASSWORD','Contraseña',{secret:true});
    if(values.DOC110_E2E_PROVIDER_PROFILE==='YES' && !values.DOC110_E2E_UNIQUE_KEY)
      await collectValue(values,'DOC110_E2E_UNIQUE_KEY','CONSECUTIVO con un unico registro en '+fixture.table,{secret:true});
    if(values.DOC110_E2E_SOURCE_PROFILE==='YES'&&values.DOC110_E2E_SOURCE_CLEANUP!=='YES'&&!fixture.ambiguousKey&&!values.DOC110_E2E_AMBIGUOUS_KEY)
      await collectValue(values,'DOC110_E2E_AMBIGUOUS_KEY','CONSECUTIVO duplicado en '+fixture.table+'; escriba NO_APLICA si no dispone de uno (escenario ambiguo no ejecutado)',{secret:true});
    if(values.DOC110_E2E_ACCESS_ISOLATION==='YES'){
      if(!values.DOC110_E2E_SECOND_USER)await collectValue(values,'DOC110_E2E_SECOND_USER','Segunda cuenta de prueba expresamente autorizada');
      if(!values.DOC110_E2E_SECOND_PASSWORD)await collectValue(values,'DOC110_E2E_SECOND_PASSWORD','Contraseña de segunda cuenta',{secret:true});
    }
  }
  if(values.DOC110_E2E_SOURCE_PROFILE==='YES'){
    requireInteractiveConsole();
    await collectConfirmation(values,'DOC110_E2E_SOURCE_CATALOG_AUTHORIZED',
      'Autoriza en el ambiente configurado y con esta cuenta '+(values.DOC110_E2E_SOURCE_CLEANUP==='YES'?'verificar y dejar inactiva SOLO la fuente de prueba identificada por reporte e historial DA ':values.DOC110_E2E_SOURCE_CONTINUE==='YES'?'continuar desde la recuperacion PASS, reactivar y revocar SOLO la fuente de prueba DA ':values.DOC110_E2E_SOURCE_RESUME==='YES'?'reanudar exclusivamente el alta de la prueba fallida y activar/editar/revocar su autorizacion DA ':'crear/activar/editar/revocar una NUEVA autorizacion DA ')+fixture.table+(values.DOC110_E2E_SOURCE_CLEANUP==='YES'?'':', con proveedor y perfil descartables'));
  }
  const environment={...process.env,...values};
  try{
    const args=[
      path.join(root,'node_modules/@playwright/test/cli.js'),'test',
      'tests/doc110-perfil-importacion-session.spec.cjs','--reporter=list'
    ];
    // Opción nativa del runner existente; no modifica la configuración global.
    if(process.argv.includes('--headed'))args.push('--headed');
    const result=await runChild(process.execPath,args,root,environment);
    process.exitCode=result.code;
  }finally{
    for(const name of Object.keys(values)){delete values[name];delete environment[name];}
  }
}
module.exports={excelProfilePlan,excelPreparationPlan,excelConfirmationPlan,excelUiPlan,runExcelUi,startExcelExternalWriter};
if(require.main===module)main().catch(error=>{
  console.error(/^DOC12[2456]_[A-Z_]+$/.test(error.message)?error.message:
    'DOC110_SESSION_BLOCKED: faltan credenciales efímeras. Ejecute este comando en su consola interactiva para ingresarlas; no se inició login.');
  process.exitCode=2;
});
