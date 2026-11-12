'use strict';
const fs=require('node:fs'),path=require('node:path');
const {collectValue,requireInteractiveConsole}=require('./support/interactive-e2e-console.cjs');
const {openSession,loginObservation}=require('../tests/support/doc111-importacion-carpetas.cjs');
const {captureCodeEvidence}=require('../tests/support/doc110-code-evidence.cjs');
const {queryRows}=require('./support/doc32-e2e-odbc.cjs');
const {BASE_URL,LIMITS,check,id,hash,validateProfile,createBudget,permittedRequest,installGuard,makeControls,runSeg01Owner,runSeg01Other,runSeg02}=require('../tests/support/doc119-seguimiento.cjs');
const ROOT=path.resolve(__dirname,'../../..');
const SOURCES=['js/gestion/ImportacionCarpetas/ImportacionCarpetas.js','js/gestion/ImportacionCarpetas/ImportacionCarpetasVista.js','js/gestion/ImportacionCarpetas/ImportacionCarpetas.css'];
function parseArgs(args){
  if(args.length===1&&args[0]==='--check')return {check:true};
  check(args.length===3&&args[0]==='--run'&&/^[a-f0-9]{32}$/.test(args[1])&&args[2]==='--authorized-plan','ARGUMENT_INVALID');return {runId:args[1]};
}
async function main(args=process.argv.slice(2)){
  const options=parseArgs(args);
  if(options.check){for(const name of SOURCES)check(fs.statSync(path.join(ROOT,name)).isFile(),'SOURCE_MISSING');console.log('DOC119_CHECK=PASS; REAL_EXECUTION=false');return;}
  requireInteractiveConsole();
  const directory=path.join(ROOT,'.tmp','doc119-seguimiento',options.runId);fs.mkdirSync(directory,{recursive:true});
  const statePath=path.join(directory,'state.json'),state={version:1,runId:options.runId,pid:process.pid,authorizedPlan:true,baseUrl:BASE_URL,status:'WAITING_REFERENCES',stage:null,usage:{},results:{},events:[]};
  fs.writeFileSync(statePath,JSON.stringify(state),{flag:'wx'});
  const save=patch=>{Object.assign(state,patch);fs.writeFileSync(statePath,JSON.stringify(state,null,2));};
  const event=(status,stage=state.stage)=>{state.events.push({status,stage,utc:new Date().toISOString()});save({status,stage});console.log('DOC119_STATUS='+status);};
  const secrets={},references={};let browser,environment;
  try{
    console.log('DOC-119: lectura SEG-01/02. Sin recepcion, almacenamiento, cambios de permisos o limpieza.');
    console.log('Ambiente autorizado: '+BASE_URL+'; presupuestos: '+JSON.stringify(LIMITS));
    await collectValue(references,'lotId','Lote SEG-01 autorizado (estados mixtos y un filtro con varias paginas; sin valor predeterminado)');
    await collectValue(references,'recoveryLotId','Lote SEG-02 con recepcion por completar; 0 para omitir por falta de recurso',{defaultValue:'0'});
    check(id(references.lotId)&&(references.recoveryLotId==='0'||id(references.recoveryLotId)),'RESOURCE_INVALID');
    if(references.recoveryLotId!=='0'){
      await collectValue(references,'alternateLevel','Nivel alterno autorizado para seleccion del arbol SEG-02');
      await collectValue(secrets,'reselectionPath','Ruta local al PDF original para reseleccion SIN envio',{secret:true});
      check(id(references.alternateLevel),'RESOURCE_INVALID');
      check(!secrets.reselectionPath.startsWith('\\\\')&&/\.pdf$/i.test(secrets.reselectionPath),'LOCAL_FILE_REQUIRED');
      secrets.reselectionPath=fs.realpathSync(secrets.reselectionPath);check(fs.statSync(secrets.reselectionPath).isFile(),'LOCAL_FILE_REQUIRED');
    }
    await collectValue(references,'odbcDsn','DSN RA (solo nombre)',{defaultValue:'MembershipUsers'});
    await collectValue(references,'docuarchiOdbcDsn','DSN DA (solo nombre)',{defaultValue:'MembershipUsers'});
    await collectValue(references,'tls','Aceptar certificado local de pruebas: SI o NO',{defaultValue:'NO'});check(['SI','NO'].includes(references.tls.toUpperCase()),'TLS_CHOICE_INVALID');
    const profile=validateProfile({baseUrl:BASE_URL,module:'GESTOR',ignoreHttpsErrors:references.tls.toUpperCase()==='SI',timeoutMilliseconds:60000,odbcDsn:references.odbcDsn,docuarchiOdbcDsn:references.docuarchiOdbcDsn,lotId:references.lotId,recoveryLotId:references.recoveryLotId==='0'?null:references.recoveryLotId,alternateLevel:references.alternateLevel||null});
    save({resources:{lotId:profile.lotId,recoveryLotId:profile.recoveryLotId,alternateLevel:profile.alternateLevel},tlsException:profile.ignoreHttpsErrors});
    event('WAITING_CREDENTIALS');
    await collectValue(secrets,'ownerUser','Cuenta propietaria autorizada (entrada enmascarada)',{secret:true});await collectValue(secrets,'ownerPassword','Clave de la cuenta propietaria',{secret:true});
    await collectValue(secrets,'otherUser','Cuenta ajena autorizada con permiso de importar (entrada enmascarada)',{secret:true});await collectValue(secrets,'otherPassword','Clave de la cuenta ajena',{secret:true});
    check(secrets.ownerUser.toLowerCase()!==secrets.otherUser.toLowerCase(),'DISTINCT_ACCOUNTS_REQUIRED');
    await collectValue(secrets,'dbUser','Cuenta MySQL exclusivamente SELECT para RA/DA',{secret:true});await collectValue(secrets,'dbPassword','Clave de la cuenta MySQL SELECT',{secret:true});
    environment=Object.fromEntries(Object.entries(process.env).filter(([key])=>!/^DOC\d+(?:_DA)?_E2E/.test(key)));
    Object.assign(environment,{DOC111_E2E_MODULE:'GESTOR',DOC111_E2E_ODBC_DSN:profile.odbcDsn,DOC111_DA_E2E_ODBC_DSN:profile.docuarchiOdbcDsn});
    for(const prefix of ['DOC111_E2E','DOC111_DA_E2E'])Object.assign(environment,{[prefix+'_MYSQL_USER']:secrets.dbUser,[prefix+'_MYSQL_PASSWORD']:secrets.dbPassword});
    save({code:captureCodeEvidence({files:[...SOURCES,'tools/e2e/scripts/run-doc119-seguimiento.cjs','tools/e2e/tests/support/doc119-seguimiento.cjs','tools/e2e/tests/support/doc111-importacion-carpetas.cjs','tools/e2e/tests/support/doc113-ventana-importacion.cjs','tools/e2e/tests/support/authenticated-workflow-session.cjs','tools/e2e/scripts/support/doc32-e2e-odbc.cjs','tools/e2e/scripts/query-doc32-odbc-fingerprint.ps1','tools/e2e/scripts/support/interactive-e2e-console.cjs','tools/e2e/tests/support/doc110-code-evidence.cjs','bin/GestionDocumental-Docuarchi.net.dll']})});
    delete process.env.DEBUG;delete process.env.PWDEBUG;delete environment.DEBUG;delete environment.PWDEBUG;
    browser=await require('../node_modules/playwright').chromium.launch({headless:true});
    const runStage=async(stage,lotId,work)=>{
      event('PREFLIGHT',stage);const budget=createBudget(stage,counts=>{state.usage[stage]=counts;save({});});
      const controls=makeControls((origin,sql,parameters)=>queryRows(sql,parameters,environment,origin==='RA'?'DOC111_E2E':'DOC111_DA_E2E'),budget);
      const before=await controls.snapshot(lotId);save({preflight:{stage,lotId,originalFiles:Number(before.data.lot.total_archivos),originalRoots:Number(before.data.lot.total_raices),fingerprint:before.fingerprint}});
      const open=async role=>{
        budget.take('sessions');Object.assign(environment,{DOC111_E2E_USER:secrets[role+'User'],DOC111_E2E_PASSWORD:secrets[role+'Password']});
        const scope={lotId,level:before.context.NivelRef,planId:before.planId,allowTree:stage==='SEG-02',alternateLevel:profile.alternateLevel};let guard;
        const guardedBrowser={newContext:async contextOptions=>{const context=await browser.newContext(contextOptions);guard=await installGuard(context,budget,scope);return context;}};
        let auth;
        try{auth=await openSession(guardedBrowser,{...profile,nivelRef:scope.level},environment,{environment:true,account:true},{beforeDiagnosticPost(){check(permittedRequest('POST',new URL('webservice/WebServiceImportacionCarpetas.asmx/AbrirImportacion',BASE_URL).href,{nivelRef:scope.level},scope),'HTTP_SCOPE_DENIED');budget.take('http');}});}
        catch(error){save({guardViolation:guard?.violations[0]||null});throw error;}
        auth.page.setDefaultTimeout(30000);auth.guard=guard;
        for(const resource of SOURCES){budget.take('http');const response=await auth.context.request.get(new URL(resource,BASE_URL).href,{maxRetries:0,maxRedirects:0,timeout:30000});check(response.status()===200&&hash(await response.body())===hash(fs.readFileSync(path.join(ROOT,resource))),'SERVED_SOURCE_MISMATCH');}
        check(guard.violations.length===0,'HTTP_SCOPE_DENIED');return auth;
      };
      event('RUNNING',stage);let result,active;
      try{result=await work({before,open,close:async auth=>{check(auth.guard.violations.length===0,'HTTP_SCOPE_DENIED');await auth.context.close();},setActive:auth=>{active=auth;}});}
      finally{if(active)await active.context.close().catch(()=>{});delete environment.DOC111_E2E_USER;delete environment.DOC111_E2E_PASSWORD;}
      const after=await controls.snapshot(lotId);check(before.fingerprint===after.fingerprint,'READ_CHANGED_BUSINESS_STATE');
      const limited=stage==='SEG-02'&&(!result.homonymousRootsObserved||!result.newRelationsAdded);
      state.results[stage]={status:limited?'PASS_READONLY_WITH_LIMITS':'PASS_READONLY',...result,noBusinessMutation:true,daScope:before.daScope,beforeFingerprint:before.fingerprint,afterFingerprint:after.fingerprint,usage:{...budget.counts}};event('STAGE_COMPLETE',stage);
    };
    await runStage('SEG-01',profile.lotId,async({before,open,close,setActive})=>{
      const owner=await open('owner');setActive(owner);const result=await runSeg01Owner(owner.page,profile.lotId,before,owner.guard);await close(owner);
      const other=await open('other');setActive(other);const denial=await runSeg01Other(other.page,profile.lotId);await close(other);return {...result,...denial};
    });
    if(profile.recoveryLotId)await runStage('SEG-02',profile.recoveryLotId,async({before,open,close,setActive})=>{
      const owner=await open('owner');setActive(owner);const result=await runSeg02(owner.page,profile,before,secrets.reselectionPath);await close(owner);return result;
    });
    else state.results['SEG-02']={status:'NOT_EXECUTED_RESOURCE_NOT_SELECTED'};
    event(Object.values(state.results).every(r=>r.status==='PASS_READONLY')?'FINISHED':'FINISHED_WITH_LIMITS');
  }catch(error){
    const code=/^(?:DOC119|DOC111|DOC113)_[A-Z0-9_]{3,100}$/.test(error?.message)?error.message:'DOC119_STOPPED';
    const observation=loginObservation(error?.loginObservation);save({failure:{code,...observation?{loginObservation:observation}:{}}});event('STOPPED');process.exitCode=2;
  }finally{
    if(browser)await browser.close().catch(()=>{});for(const key of Object.keys(secrets))delete secrets[key];
    if(environment)for(const key of Object.keys(environment))if(/^DOC\d+(?:_DA)?_E2E/.test(key))delete environment[key];
    save({closed:true,credentialsCleared:true});console.log('DOC119_RESULT='+JSON.stringify({runId:state.runId,status:state.status,results:state.results,usage:state.usage,failure:state.failure?.code||null}));
  }
}
if(require.main===module)main().catch(()=>{console.error('DOC119_PRECHECK_STOPPED: sin ejecucion automatica ni detalle privado.');process.exitCode=2;});
module.exports={parseArgs,main};
