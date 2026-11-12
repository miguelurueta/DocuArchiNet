'use strict';
const fs=require('node:fs'),path=require('node:path');
const {collectValue,requireInteractiveConsole}=require('./support/interactive-e2e-console.cjs');
const {openSession,loginObservation}=require('../tests/support/doc111-importacion-carpetas.cjs');
const {captureCodeEvidence}=require('../tests/support/doc110-code-evidence.cjs');
const {queryRows}=require('./support/doc32-e2e-odbc.cjs');
const {BASE_URL,LIMITS,check,id,hash,validateConnection,validateProfile,chooseItem,describeImport,discoverImports,discoverLevels,pendingFile,createBudget,permittedRequest,installGuard,makeControls,runSeg01Owner,runSeg01Other,runSeg02}=require('../tests/support/doc119-seguimiento.cjs');
const ROOT=path.resolve(__dirname,'../../..');
const SOURCES=['js/gestion/ImportacionCarpetas/ImportacionCarpetas.js','js/gestion/ImportacionCarpetas/ImportacionCarpetasVista.js','js/gestion/ImportacionCarpetas/ImportacionCarpetas.css'];
const EVIDENCE_FILES=[...SOURCES,'tools/e2e/scripts/run-doc119-seguimiento.cjs','tools/e2e/tests/support/doc119-seguimiento.cjs','tools/e2e/tests/support/doc111-importacion-carpetas.cjs','tools/e2e/tests/support/doc113-ventana-importacion.cjs','tools/e2e/tests/support/authenticated-workflow-session.cjs','tools/e2e/scripts/support/doc32-e2e-odbc.cjs','tools/e2e/scripts/query-doc32-odbc-fingerprint.ps1','tools/e2e/scripts/support/interactive-e2e-console.cjs','tools/e2e/tests/support/doc110-code-evidence.cjs','bin/GestionDocumental-Docuarchi.net.dll'];
function parseArgs(args){
  if(args.length===1&&args[0]==='--check')return {check:true};
  check(args.length===3&&args[0]==='--run'&&/^[a-f0-9]{32}$/.test(args[1])&&args[2]==='--authorized-plan','ARGUMENT_INVALID');return {runId:args[1]};
}
async function main(args=process.argv.slice(2)){
  const options=parseArgs(args);
  if(options.check){for(const name of SOURCES)check(fs.statSync(path.join(ROOT,name)).isFile(),'SOURCE_MISSING');console.log('DOC119_CHECK=PASS; REAL_EXECUTION=false');return;}
  requireInteractiveConsole();
  const directory=path.join(ROOT,'.tmp','doc119-seguimiento',options.runId);fs.mkdirSync(directory,{recursive:true});
  const statePath=path.join(directory,'state.json'),state={version:2,runId:options.runId,pid:process.pid,authorizedPlan:true,baseUrl:BASE_URL,status:'WAITING_ACCOUNT',stage:null,usage:{},results:{},events:[]};
  fs.writeFileSync(statePath,JSON.stringify(state),{flag:'wx'});
  const save=patch=>{Object.assign(state,patch);fs.writeFileSync(statePath,JSON.stringify(state,null,2));};
  const messages={AUTHENTICATING_OWNER:'Comprobando el acceso de la cuenta de pruebas...',DISCOVERING_IMPORTS:'Buscando sus importaciones de carpetas...',WAITING_IMPORT_SELECTION:'Elige una importacion de la lista.',WAITING_RECOVERY_SELECTION:'Elige si deseas comprobar la recuperacion de archivos por recibir.',PREFLIGHT:'Comprobando los datos actuales de la importacion elegida...',RUNNING:'Ejecutando las comprobaciones de consulta...',WAITING_SECOND_ACCOUNT:'Falta la segunda cuenta para comprobar la privacidad de la importacion.',STAGE_COMPLETE:'Esta parte de la comprobacion termino.',FINISHED:'La ejecucion termino; el informe queda listo para revision.',FINISHED_WITH_LIMITS:'La ejecucion termino con partes omitidas o limites que deben revisarse.',STOPPED:'La comprobacion se detuvo; no se repetira automaticamente.'};
  const event=(status,stage=state.stage)=>{state.events.push({status,stage,utc:new Date().toISOString()});save({status,stage});console.log(messages[status]||'Progreso de la comprobacion actualizado.');};
  const secrets={},references={},contexts=new Set(),budgets={};let browser,environment,preparedOwner;
  const budgetFor=stage=>{
    if(!budgets[stage])budgets[stage]=createBudget(stage,counts=>{state.usage[stage]=counts;save({});});
    return budgets[stage];
  };
  async function menu(items,label,allowSkip=false){
    for(let attempt=0;attempt<3;attempt++){
      await collectValue(references,'choice',label,allowSkip?{defaultValue:'0'}:{});
      try{return chooseItem(items,references.choice,allowSkip);}
      catch(error){if(error.message!=='DOC119_SELECTION_INVALID')throw error;console.log('Elige el numero de una opcion mostrada'+(allowSkip?' o 0 para omitir.':'.'));}
    }
    check(false,'SELECTION_INVALID');
  }
  try{
    console.log('Esta prueba revisa importaciones de carpetas que ya existen en Produccion Documental.');
    console.log('Comprobara consulta, filtros, descarga CSV y recuperacion al recargar. No crea carpetas ni envia archivos.');
    console.log('No necesitas conocer numeros internos: se mostraran las importaciones de la cuenta que ingreses.');
    console.log('Ambiente autorizado: '+BASE_URL);
    await collectValue(references,'odbcDsn','Conexion de consultas RA (Enter conserva el DSN de pruebas)',{defaultValue:'MembershipUsers'});
    await collectValue(references,'docuarchiOdbcDsn','Conexion de consultas DA (Enter conserva el DSN de pruebas)',{defaultValue:'MembershipUsers'});
    await collectValue(references,'tls','Aceptar certificado local de pruebas: SI o NO',{defaultValue:'NO'});check(['SI','NO'].includes(references.tls.toUpperCase()),'TLS_CHOICE_INVALID');
    const connection=validateConnection({baseUrl:BASE_URL,module:'GESTOR',ignoreHttpsErrors:references.tls.toUpperCase()==='SI',timeoutMilliseconds:60000,odbcDsn:references.odbcDsn,docuarchiOdbcDsn:references.docuarchiOdbcDsn});
    save({tlsException:connection.ignoreHttpsErrors});
    await collectValue(secrets,'ownerUser','Cuenta de pruebas que realizo las importaciones (enmascarada)',{secret:true});await collectValue(secrets,'ownerPassword','Clave de esa cuenta',{secret:true});
    console.log('Las siguientes credenciales solo permiten los controles de lectura de la prueba.');
    await collectValue(secrets,'dbUser','Cuenta MySQL exclusivamente SELECT para RA/DA',{secret:true});await collectValue(secrets,'dbPassword','Clave de la cuenta MySQL SELECT',{secret:true});
    environment=Object.fromEntries(Object.entries(process.env).filter(([key])=>!/^DOC\d+(?:_DA)?_E2E/.test(key)));
    Object.assign(environment,{DOC111_E2E_MODULE:'GESTOR',DOC111_E2E_ODBC_DSN:connection.odbcDsn,DOC111_DA_E2E_ODBC_DSN:connection.docuarchiOdbcDsn});
    for(const prefix of ['DOC111_E2E','DOC111_DA_E2E'])Object.assign(environment,{[prefix+'_MYSQL_USER']:secrets.dbUser,[prefix+'_MYSQL_PASSWORD']:secrets.dbPassword});
    save({code:captureCodeEvidence({files:EVIDENCE_FILES})});
    delete process.env.DEBUG;delete process.env.PWDEBUG;delete environment.DEBUG;delete environment.PWDEBUG;
    browser=await require('../node_modules/playwright').chromium.launch({headless:true});
    const query=(origin,sql,parameters)=>queryRows(sql,parameters,environment,origin==='RA'?'DOC111_E2E':'DOC111_DA_E2E');
    const openRole=async(role,budget,scope)=>{
      budget.take('sessions');Object.assign(environment,{DOC111_E2E_USER:secrets[role+'User'],DOC111_E2E_PASSWORD:secrets[role+'Password']});
      let guard;
      const guardedBrowser={newContext:async contextOptions=>{const context=await browser.newContext(contextOptions);contexts.add(context);guard=await installGuard(context,budget,scope);return context;}};
      let auth;
      try{auth=await openSession(guardedBrowser,{...connection,nivelRef:scope.level},environment,{environment:true,account:true},{beforeDiagnosticPost(){check(id(scope.level)&&permittedRequest('POST',new URL('webservice/WebServiceImportacionCarpetas.asmx/AbrirImportacion',BASE_URL).href,{nivelRef:scope.level},scope),'HTTP_SCOPE_DENIED');budget.take('http');}});}
      catch(error){save({guardViolation:guard?.violations[0]||null});if(guard?.violations.length)throw Error(guard.violations[0]);throw error;}
      auth.page.setDefaultTimeout(30000);auth.guard=guard;auth.scope=scope;
      for(const resource of SOURCES){budget.take('http');const response=await auth.context.request.get(new URL(resource,BASE_URL).href,{maxRetries:0,maxRedirects:0,timeout:30000});check(response.status()===200&&hash(await response.body())===hash(fs.readFileSync(path.join(ROOT,resource))),'SERVED_SOURCE_MISMATCH');}
      check(guard.violations.length===0,'HTTP_SCOPE_DENIED');return auth;
    };
    // Authenticate before showing any account-owned import. Until selection,
    // no import ASMX operation has a bound lot/plan/level in the guard.
    event('AUTHENTICATING_OWNER','SEG-01');
    preparedOwner=await openRole('owner',budgetFor('SEG-01'),{lotId:null,level:null,planId:null,allowTree:false,alternateLevel:null});
    event('DISCOVERING_IMPORTS','SEG-01');
    const catalog=await discoverImports(query,budgetFor('SEG-01'),secrets.ownerUser);
    save({discovery:{imports:catalog.items.length}});
    if(!catalog.items.length){console.log('Esta cuenta no tiene importaciones consultables. La prueba no creara una para forzar el resultado.');check(false,'NO_IMPORTS_FOR_ACCOUNT');}
    console.log('\nImportaciones de carpetas de esta cuenta (fecha UTC, destino, estado y cantidades):');
    catalog.items.forEach((item,index)=>console.log(describeImport(item,index)));
    console.log('Elige una que tenga varios archivos y estados diferentes. La prueba verificara si alcanza para pasar a otra pagina.');
    event('WAITING_IMPORT_SELECTION','SEG-01');
    const selected=await menu(catalog.items,'Numero de la opcion para revisar filtros y CSV');
    let recovery=null,alternate=null;
    const unfinished=catalog.items.filter(item=>item.pendingFiles>0);
    if(unfinished.length){
      console.log('\nPara comprobar la recuperacion, estas importaciones todavia tienen archivos por recibir:');
      unfinished.forEach((item,index)=>console.log(describeImport(item,index)));
      console.log('Solo elige una si conservas el archivo original en este equipo. No se enviara.');
      event('WAITING_RECOVERY_SELECTION','SEG-02');
      recovery=await menu(unfinished,'Opcion para recuperacion, o 0 para omitir',true);
      if(recovery){
        const file=await pendingFile(query,budgetFor('SEG-02'),catalog.principal,recovery.id);
        console.log('Archivo que debe reseleccionarse: '+file.name+' ('+file.relativePath+').');
        await collectValue(secrets,'reselectionPath','Ruta local de ese archivo; escribe 0 si no lo tienes (entrada enmascarada)',{secret:true});
        if(secrets.reselectionPath==='0'){recovery=null;delete secrets.reselectionPath;}
        else{
          check(!secrets.reselectionPath.startsWith('\\\\')&&/\.pdf$/i.test(secrets.reselectionPath),'LOCAL_FILE_REQUIRED');
          try{secrets.reselectionPath=fs.realpathSync(secrets.reselectionPath);check(fs.statSync(secrets.reselectionPath).isFile(),'LOCAL_FILE_REQUIRED');}
          catch{check(false,'LOCAL_FILE_UNAVAILABLE');}
          const levels=await discoverLevels(query,budgetFor('SEG-02'),catalog.principal,recovery.level);
          if(levels.length){
            console.log('\nPara comprobar que la seleccion del arbol se conserva, elige otro destino disponible:');
            levels.forEach((level,index)=>console.log((index+1)+'. '+(level.parent?level.parent+' / ':'')+level.name));
            alternate=await menu(levels,'Opcion del otro destino');
          }else{console.log('No hay otro destino disponible para este caso; recuperacion quedara sin ejecutar.');recovery=null;}
        }
      }
    }else console.log('No hay importaciones con archivos por recibir: la parte de recuperacion quedara sin ejecutar.');
    const profile=validateProfile({...connection,lotId:selected.id,recoveryLotId:recovery?.id||null,alternateLevel:alternate?.id||null});
    save({resources:{lotId:profile.lotId,recoveryLotId:profile.recoveryLotId,alternateLevel:profile.alternateLevel},selectionsResolved:true});
    const runStage=async(stage,lotId,work)=>{
      event('PREFLIGHT',stage);const budget=budgetFor(stage),controls=makeControls(query,budget);
      const before=await controls.snapshot(lotId);
      check(before.context.UsuarioRef===catalog.principal.userRef&&before.context.AmbitoRef===catalog.principal.companyRef,'RESOURCE_OWNER_CHANGED');
      save({preflight:{stage,lotId,originalFiles:Number(before.data.lot.total_archivos),originalRoots:Number(before.data.lot.total_raices),fingerprint:before.fingerprint}});
      const open=async role=>{
        const scope={lotId,level:before.context.NivelRef,planId:before.planId,allowTree:stage==='SEG-02',alternateLevel:profile.alternateLevel};
        if(stage==='SEG-01'&&role==='owner'&&preparedOwner){
          const auth=preparedOwner;preparedOwner=null;Object.assign(auth.scope,scope);check(auth.guard.violations.length===0,'HTTP_SCOPE_DENIED');return auth;
        }
        return openRole(role,budget,scope);
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
      event('WAITING_SECOND_ACCOUNT','SEG-01');
      console.log('Ahora se comprobara que otra cuenta de pruebas no pueda ver ni exportar esa importacion.');
      await collectValue(secrets,'otherUser','Segunda cuenta de pruebas que use importacion (enmascarada)',{secret:true});await collectValue(secrets,'otherPassword','Clave de esa segunda cuenta',{secret:true});
      check(secrets.ownerUser.toLowerCase()!==secrets.otherUser.toLowerCase(),'DISTINCT_ACCOUNTS_REQUIRED');
      const other=await open('other');setActive(other);const denial=await runSeg01Other(other.page,profile.lotId);await close(other);return {...result,...denial};
    });
    if(profile.recoveryLotId)await runStage('SEG-02',profile.recoveryLotId,async({before,open,close,setActive})=>{
      const owner=await open('owner');setActive(owner);const result=await runSeg02(owner.page,profile,before,secrets.reselectionPath);await close(owner);return result;
    });
    else state.results['SEG-02']={status:'NOT_EXECUTED_RESOURCE_NOT_SELECTED'};
    event(Object.values(state.results).every(r=>r.status==='PASS_READONLY')?'FINISHED':'FINISHED_WITH_LIMITS');
  }catch(error){
    const code=/^(?:DOC119|DOC111|DOC113)_[A-Z0-9_]{3,100}$/.test(error?.message)?error.message:'DOC119_STOPPED';
    const explanations={DOC119_NO_IMPORTS_FOR_ACCOUNT:'No se encontraron importaciones consultables de esta cuenta.',DOC119_PAGEABLE_FILTER_RESOURCE_REQUIRED:'La importacion elegida no tiene suficientes coincidencias para comprobar una segunda pagina con el tamano actual.',DOC119_MIXED_STATES_REQUIRED:'La importacion elegida no tiene los distintos estados necesarios para esta comprobacion.',DOC119_RESELECTION_METADATA_MISMATCH:'El archivo elegido no coincide en nombre, tamano o fecha con el archivo por recibir. No se envio.',DOC119_SERVED_SOURCE_MISMATCH:'Los archivos servidos no coinciden con los preparados para esta prueba.',DOC111_SELECT_FAILED:'No se pudo completar una consulta de control; revisa la cuenta de lectura y la conexion del ambiente.'};
    if(explanations[code])console.log(explanations[code]);
    const observation=loginObservation(error?.loginObservation);save({failure:{code,...observation?{loginObservation:observation}:{}}});event('STOPPED');process.exitCode=2;
  }finally{
    for(const context of contexts)await context.close().catch(()=>{});
    if(browser)await browser.close().catch(()=>{});for(const key of Object.keys(secrets))delete secrets[key];
    if(environment)for(const key of Object.keys(environment))if(/^DOC\d+(?:_DA)?_E2E/.test(key))delete environment[key];
    save({closed:true,credentialsCleared:true});console.log('DOC119_RESULT='+JSON.stringify({runId:state.runId,status:state.status,results:state.results,usage:state.usage,failure:state.failure?.code||null}));
  }
}
if(require.main===module)main().catch(()=>{console.error('DOC119_PRECHECK_STOPPED: sin ejecucion automatica ni detalle privado.');process.exitCode=2;});
module.exports={parseArgs,main};
