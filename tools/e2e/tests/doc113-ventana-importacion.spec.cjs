'use strict';
const {test}=require('@playwright/test');
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const {openSession}=require('./support/doc111-importacion-carpetas.cjs');
const {captureCodeEvidence}=require('./support/doc110-code-evidence.cjs');
const {queryRows}=require('../scripts/support/doc32-e2e-odbc.cjs');
const {CASES,validateOptions,assertAuthorization,createFixtures,createUiControls,runUi}=require('./support/doc113-ventana-importacion.cjs');
test.use({screenshot:'off',trace:'off',video:'off'});
test.describe.configure({mode:'serial',retries:0});
for(const scenario of CASES)test('DOC113 '+scenario,async({browser},testInfo)=>{
  test.skip(process.env.DOC113_E2E_AUTHORIZED!=='true'||process.env.DOC113_UI_SCENARIO!==scenario,'NO_EJECUTADO: requiere autorización específica DOC-113 de ambiente, cuenta, recursos y efectos aplicables.');
  let auth,fixtures,stage='PRECHECK';
  try{
    const profile=JSON.parse(process.env.DOC113_UI_PROFILE),options=JSON.parse(process.env.DOC113_UI_OPTIONS);
    validateOptions(profile,options);if(options.scenario!==scenario)throw Error('DOC113_SCENARIO_INVALID');
    const authorization=Object.fromEntries(['environment','account','select','effects','discardable','confirm','errorProbe','tls'].map(k=>[k,process.env['DOC113_AUTH_'+k.toUpperCase()]==='true']));
    assertAuthorization(options,authorization,profile);test.setTimeout(profile.timeoutMilliseconds);
    stage='LOGIN';auth=await openSession(browser,profile,process.env,authorization);
    stage='SOURCE';
    const root=path.resolve(__dirname,'../../..'),script='js/gestion/ImportacionCarpetas/ImportacionCarpetas.js';
    const local=crypto.createHash('sha256').update(fs.readFileSync(path.join(root,script))).digest('hex');
    const served=await auth.context.request.get(new URL(script,profile.baseUrl).href,{maxRetries:0,maxRedirects:0});
    if(served.status()!==200||crypto.createHash('sha256').update(await served.body()).digest('hex')!==local)throw Error('DOC113_SOURCE_NOT_VERIFIED');
    const code=captureCodeEvidence({files:[script,'js/gestion/ImportacionCarpetas/InventarioImportacion.js','js/gestion/ImportacionCarpetas/ImportacionCarpetasApi.js','js/gestion/ImportacionCarpetas/ImportacionCarpetasVista.js','tools/e2e/tests/doc113-ventana-importacion.spec.cjs','tools/e2e/tests/support/doc113-ventana-importacion.cjs','tools/e2e/tests/support/doc110-code-evidence.cjs','tools/e2e/tests/support/authenticated-workflow-session.cjs','tools/e2e/scripts/run-doc113-ventana-importacion.cjs']});
    const query=(origin,sql,args)=>queryRows(sql,args,process.env,origin==='RA'?'DOC111_E2E':'DOC111_DA_E2E');
    const controls=createUiControls(query,profile);
    if(scenario==='UI-02'){const parent=path.join(root,'.tmp');fs.mkdirSync(parent,{recursive:true});fixtures=createFixtures(parent,profile);}
    stage='UI';const result=await runUi(auth.page,profile,options,authorization,controls,fixtures,value=>console.log('DOC113_UI_REFERENCE='+JSON.stringify({scenario,...value})));
    // Solo el diálogo en Producción, sin formularios de login, cookies o trazas.
    if(await auth.page.getByRole('dialog').count()===1)await auth.page.getByRole('dialog').screenshot({path:testInfo.outputPath('doc113-'+scenario+'.png')});
    console.log('DOC113_UI_RESULT='+JSON.stringify({...result,head:code.head,codeSha256:code.scopeSha256,servedScriptSha256:local}));
  }catch(error){
    const code=/^DOC113_[A-Z0-9_]{3,70}$/.test(error?.message)?error.message:'DOC113_UI_FAILED';
    console.log('DOC113_UI_FAILURE='+JSON.stringify({scenario,stage,code}));throw new Error(code);
  }finally{if(fixtures)fixtures.close();if(auth)await auth.context.close().catch(()=>{});}
});
