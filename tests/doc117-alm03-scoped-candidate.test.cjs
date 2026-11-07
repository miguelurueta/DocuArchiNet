'use strict';
const {test}=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto'),{execFileSync}=require('node:child_process');
const {ROOT,GUARD,renderGuard,generate,resourceHashes}=require('../tools/e2e/scripts/support/doc117-alm03-scoped-candidate.cjs');
const {TEMPLATE}=require('../tools/e2e/scripts/support/doc117-alm03-candidate.cjs');
const {assessMethodObservation}=require('../tools/e2e/tests/support/doc117-alm03-method-observation.cjs');
test('ALM03 closed callers validate actual immutable commands, body arguments and independent sessions in an isolated .NET process',()=>{
 const dir=path.join(ROOT,'.tmp','doc117-alm03-closed-native-'+crypto.randomUUID());fs.mkdirSync(dir);
 require('./doc110-perfil-importacion-behavior.test.cjs').compileBehaviorHarness(dir);
 const command=name=>execFileSync('powershell.exe',['-NoProfile','-Command','(Get-Command '+name+' -ErrorAction Stop).Source'],{encoding:'utf8',windowsHide:true}).trim(),vbc=command('vbc.exe'),csc=command('csc.exe'),lib=path.join(dir,'ClosedGuard.dll'),exe=path.join(dir,'closed-tests.exe'),helper=path.join(dir,'observer.vb'),guard=path.join(dir,'guard.vb'),scopeFile=path.join(dir,'scope.json');
 fs.writeFileSync(helper,fs.readFileSync(path.join(ROOT,TEMPLATE),'utf8'));fs.copyFileSync(path.join(ROOT,GUARD),guard);
 const compile=()=>execFileSync(vbc,['/nologo','/target:library','/out:'+lib,...['System.dll','System.Core.dll','System.Web.dll',path.join(dir,'PerfilImportacion.dll'),path.join(dir,'Newtonsoft.Json.dll')].map(f=>'/reference:'+f),helper,guard,path.join(ROOT,'tools/e2e/fixtures/Doc117SolicitudHash.vb')],{cwd:dir,encoding:'utf8',windowsHide:true});compile();
 execFileSync(csc,['/nologo','/target:exe','/out:'+exe,...['System.dll','System.Core.dll','System.Web.dll','System.Data.dll',path.join(dir,'PerfilImportacion.dll'),path.join(dir,'PerfilImportacionTests.exe'),lib,path.join(dir,'Newtonsoft.Json.dll')].map(f=>'/reference:'+f),path.join(ROOT,'tests/Doc117Alm03ClosedGuardBehaviorTests.cs')],{cwd:dir,encoding:'utf8',windowsHide:true});
 const pdf=path.join(ROOT,'tools/e2e/fixtures/doc111-carga-individual.pdf');execFileSync(exe,['--describe',scopeFile,pdf,ROOT],{cwd:dir,encoding:'utf8',windowsHide:true});const scope=JSON.parse(fs.readFileSync(scopeFile,'utf8'));
 fs.writeFileSync(helper,fs.readFileSync(helper,'utf8').replace('Throw New InvalidOperationException("DOC117_ALM03_DORMANT_SCOPE")','Return Doc117Alm03ClosedGuard.Entrar(gabinete, ruta, nombre, expediente, tipologia, tipo, clase)'));
 // Only this disposable test copy changes its site constant; no live scope is generated.
 fs.writeFileSync(guard,renderGuard(scope).replace(/Private Const Site As String = "[^"]+"/,'Private Const Site As String = "'+dir+'"'));compile();
 assert.match(execFileSync(exe,['--verify',lib,pdf],{cwd:dir,encoding:'utf8',windowsHide:true}),/CLOSED_CALLERS_WITH_PRODUCT_COMMAND: PASS/);
 const journal=path.join(dir,'App_Data','DOC117-ALM03-'+scope.runId),read=f=>JSON.parse(fs.readFileSync(path.join(journal,f),'utf8')),events=fs.readdirSync(journal).filter(f=>/^(entry|exit)-/.test(f)).map(read);
 const r=assessMethodObservation({runId:scope.runId,resourceHashes:resourceHashes(scope)},events,read('summary.json'));assert.equal(r.classification,'METHOD_BODIES_OVERLAPPED');assert.equal(r.alm03ConcurrentPass,false);
});
test('ALM03 scoped generation is fresh, confined to temporary output and adds guards in both callers',()=>{
 const scope=JSON.parse(fs.readFileSync(path.join(ROOT,'.tmp/doc117-alm03-setup-57219fc625b867b54d8d282b6b1eb0a2-scope.json'),'utf8')),dir=path.join(ROOT,'.tmp','doc117-alm03-scoped-test-'+crypto.randomUUID());
 const r=generate(dir,scope);assert.equal(r.resourceHashes.length,3);assert.ok(fs.readFileSync(path.join(dir,'ClassAlmacenamiento.Laboratory.vb'),'utf8').includes('PrepararIndividual('));assert.ok(fs.readFileSync(path.join(dir,'AdaptadorDocumentoImportacion.Legacy.Laboratory.vb'),'utf8').includes('PrepararImportacion('));assert.throws(()=>generate(dir,scope),/FRESH_DIRECTORY_REQUIRED/);assert.throws(()=>generate(path.join(ROOT,'bin','scoped'),scope),/FRESH_DIRECTORY_REQUIRED/);
});
