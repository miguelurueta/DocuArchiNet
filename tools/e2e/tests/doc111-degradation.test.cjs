'use strict';
const test=require('node:test'),assert=require('node:assert/strict');
const runner=require('../scripts/run-doc111-importacion-carpetas.cjs');
const {validateDegradationProfile}=require('./support/doc111-degradation.cjs');
const profile=()=>({scenarioId:'production-document-upload-execution',baseUrl:'https://localhost/GestionDocumental-Docuarchi.net/',module:'GESTOR',environment:'pruebas-locales',odbcDsn:'MembershipUsers',expedientId:467,documentTypeId:50,documentTypeName:'Comprobante de Egreso',fixturePath:'doc111-carga-individual.pdf',budgetMs:60000,ignoreHttpsErrors:true});
const operative={mode:'LOTE',baseUrl:'https://localhost/GestionDocumental-Docuarchi.net/',module:'GESTOR',nivelRef:'7'};
test('degradación: solo admite el recurso individual y escenario aprobados',()=>{
  assert.equal(validateDegradationProfile(profile(),operative).expedientId,467);
  for(const change of [p=>p.expedientId=648,p=>p.documentTypeId=79,p=>p.fixturePath='other.pdf',p=>p.scenarioId='production-document-upload-preview']){
    const p=profile();change(p);assert.throws(()=>validateDegradationProfile(p,operative));
  }
});
test('degradación: excluye REC, diagnóstico, publicación y recuperación combinados',()=>{
  const args=['--profile','p','--scenario','LOTE','--degradation-profile','d'];assert.equal(runner.parseArgs(args)['degradation-profile'],'d');
  for(const a of [[...args,'--diagnose'],[...args,'--verify-lot','2'],[...args,'--partial-profile','p'],args.map(x=>x==='LOTE'?'PARCIAL':x)])assert.throws(()=>runner.parseArgs(a),/DOC111_/);
});
test('degradación: resultado requiere restauración, ausencia de cambios y documento verificado; sanea extras',()=>{
  const value={status:'PASS',mode:'LOTE',expedienteId:'467',documentoId:'19000',inventarioId:'29000',documents:1,unavailableVerified:true,individualUploadVerified:true,schemaRestored:true,lotsUnchanged:true,head:'a'.repeat(40),codeSha256:'b'.repeat(64),driverSha256:'c'.repeat(64),private:'PRIVATE'};
  const line=r=>'DOC111_DEGRADATION_RESULT='+JSON.stringify(r);
  assert.equal(runner.safeDegradationResult(line(value),'LOTE').documentoId,'19000');assert.ok(!JSON.stringify(runner.safeDegradationResult(line(value),'LOTE')).includes('PRIVATE'));
  for(const key of ['unavailableVerified','individualUploadVerified','schemaRestored','lotsUnchanged'])assert.equal(runner.safeDegradationResult(line({...value,[key]:false}),'LOTE'),null);
  assert.equal(runner.safeResult(line(value),'LOTE'),null);
});

test('degradación DOC120: abre el menú moderno antes del rechazo UI sin ejecutar DDL',async()=>{
  const fs=require('node:fs'),path=require('node:path'),{chromium}=require('../node_modules/playwright');
  const {checkUnavailableUI}=require('./support/doc111-degradation.cjs');
  const root=path.resolve(__dirname,'../../..'),browser=await chromium.launch({headless:true});
  try{
    const page=await browser.newPage();page.setDefaultTimeout(1500);let openings=0;
    const html='<html><body><nav id="menu_var"><ul class="navbar-nav"></ul></nav><div id="doc111-importacion" data-navegacion="menu" data-api="/control.asmx" data-upload="/upload.ashx" data-token="synthetic-only" data-recuperacion="'+'a'.repeat(32)+'" data-nivel="7"><button type="button" data-doc111-open hidden disabled>Importar</button><button type="button" data-doc111-resume hidden>Último</button><button type="button" data-doc111-verificar hidden>Verificar</button><span data-doc111-status hidden></span></div></body></html>';
    await page.route('https://doc120.invalid/**',async route=>{
      const request=route.request();
      if(request.method()==='GET')return route.fulfill({status:200,contentType:'text/html',body:html});
      assert.equal(new URL(request.url()).pathname,'/control.asmx/AbrirImportacion');openings++;
      return route.fulfill({status:503,contentType:'application/json',body:JSON.stringify({d:{ok:false,codigo:'CAPACIDAD_NO_DISPONIBLE',mensaje:'Indisponible',incidencias:[]}})});
    });
    await page.goto('https://doc120.invalid/production');
    await page.addStyleTag({content:fs.readFileSync(path.join(root,'bootstrap/css/bootstrap.min.css'),'utf8')});
    await page.addStyleTag({content:fs.readFileSync(path.join(root,'js/gestion/ImportacionCarpetas/ImportacionCarpetas.css'),'utf8')});
    for(const name of ['InventarioImportacion','ImportacionCarpetasApi','ImportacionCarpetasVista','ImportacionCarpetas'])await page.addScriptTag({content:fs.readFileSync(path.join(root,'js/gestion/ImportacionCarpetas/'+name+'.js'),'utf8')});
    assert.equal(await page.locator('[data-doc111-open]').isVisible(),false);
    await checkUnavailableUI(page);
    assert.equal(openings,1);assert.equal(await page.getByRole('dialog').count(),0);
    assert.equal(await page.locator('[data-doc111-status]').isVisible(),true);
  }finally{await browser.close();}
});
