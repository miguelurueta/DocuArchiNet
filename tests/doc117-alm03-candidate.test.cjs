'use strict';
const {test}=require('node:test'),assert=require('node:assert/strict');
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto'),{execFileSync}=require('node:child_process');
const {ROOT,STORAGE,TEMPLATE,sha,observeMethod,overlay,generate}=require('../tools/e2e/scripts/support/doc117-alm03-candidate.cjs');
const {assessMethodObservation}=require('../tools/e2e/tests/support/doc117-alm03-method-observation.cjs');
const fresh=()=>path.join(ROOT,'.tmp','doc117-alm03-native-'+crypto.randomUUID());
test('ALM03 offline overlay adds only internal entry/finally and refuses source drift or live generation',()=>{
 const source=fs.readFileSync(path.join(ROOT,STORAGE),'utf8'),r=overlay(source),dir=fresh(),runtime=sha(fs.readFileSync(path.join(ROOT,'bin/GestionDocumental-Docuarchi.net.dll')));
 assert.notEqual(r.source,source);assert.ok(r.source.includes('doc117Observation.Salir('));assert.throws(()=>overlay(source+' '),/STORAGE_SOURCE_CHANGED/);
 generate(dir);assert.throws(()=>generate(dir),/FRESH_DIRECTORY_REQUIRED/);assert.throws(()=>generate(path.join(ROOT,'bin','observer')),/FRESH_DIRECTORY_REQUIRED/);
 assert.equal(sha(fs.readFileSync(path.join(ROOT,STORAGE))),sha(source));assert.equal(sha(fs.readFileSync(path.join(ROOT,'bin/GestionDocumental-Docuarchi.net.dll'))),runtime);
 assert.doesNotMatch(fs.readFileSync(path.join(ROOT,TEMPLATE),'utf8'),/Thread\.Sleep|WaitOne|CountdownEvent|Barrier\(/);
});
test('ALM03 compiled internal observer preserves returns/ByRef/exceptions and refuses incomplete or foreign evidence',()=>{
 const dir=fresh();fs.mkdirSync(dir);const helper=fs.readFileSync(path.join(ROOT,TEMPLATE),'utf8'),helperFile=path.join(dir,'observer.vb'),stubFile=path.join(dir,'storage.vb'),lib=path.join(dir,'Alm03Observer.dll'),exe=path.join(dir,'observer-tests.exe');
 const compiler=name=>execFileSync('powershell.exe',['-NoProfile','-Command','(Get-Command '+name+' -ErrorAction Stop).Source'],{encoding:'utf8',windowsHide:true}).trim();
 const vbc=compiler('vbc.exe'),csc=compiler('csc.exe'),json=path.join(dir,'Newtonsoft.Json.dll');fs.copyFileSync(path.join(ROOT,'bin/Newtonsoft.Json.dll'),json);
 const product=fs.readFileSync(path.join(ROOT,STORAGE),'utf8'),start=product.indexOf('    Function AlmacenamientoDocumentoProduccionDocumental('),header=product.slice(start,product.indexOf(' As String',product.indexOf('ByRef EstructuraDatosImagen',start))+' As String'.length);
 const method=header.replaceAll('\r\n','\n')+`\n        Try
            System.Threading.Interlocked.Increment(Calls)
            If NombreDocumento = "ESCAPE" Then Throw New ApplicationException("SYNTHETIC_UNCAUGHT")
            If NombreDocumento = "CATCH" Then Throw New ApplicationException("SYNTHETIC_CAUGHT")
            If NombreDocumento = "EARLY" Then Return "EARLY"
            WriterAction(IdExpediente)
            IdImagenAlamacenada = 20000 + IdExpediente
            EstructuraDatosImagen.id_registro = 30000 + IdExpediente
            AlmacenamientoDocumentoProduccionDocumental = "YES"
            Exit Function
        Catch ex As Exception
            If NombreDocumento = "ESCAPE" Then Throw
            AlmacenamientoDocumentoProduccionDocumental = "LEGACY_REJECTED"
        End Try
    End Function`;
 const stub=`Imports System\nImports System.Collections.Generic\nPublic Class CDcamposAsignaAlmacenamiento\nEnd Class\nPublic Structure stru_datos_image_lista\n Public id_registro As Integer\nEnd Structure\nFriend Class Doc117Alm03TestGate\n <ThreadStatic()> Friend Shared Current As Doc117Alm03Ticket\nEnd Class\nPublic Class ClassAlmacenamiento\n Public Shared Calls As Integer\n Public Shared WriterAction As Action(Of Integer)\n${observeMethod(method).overlay}\nEnd Class\n`;
 fs.writeFileSync(stubFile,stub);fs.writeFileSync(helperFile,helper);
 const compile=()=>execFileSync(vbc,['/nologo','/target:library','/out:'+lib,'/reference:System.dll','/reference:System.Core.dll','/reference:'+json,helperFile,stubFile],{cwd:dir,encoding:'utf8',windowsHide:true});compile();
 // The actual dormant entry must refuse before the synthetic writer can run.
 const defaultCheck=path.join(dir,'default-check.ps1');fs.writeFileSync(defaultCheck,`$a=[Reflection.Assembly]::LoadFrom('${lib.replaceAll("'","''")}');try{$a.GetType('Doc117Alm03MethodObserver').GetMethod('Entrar',[Reflection.BindingFlags]'Static,NonPublic').Invoke($null,@('G','p','n',1,79,0,'PDF'))}catch{if($_.Exception.InnerException.Message -ne 'DOC117_ALM03_DORMANT_SCOPE'){throw};exit 0};exit 2`);
 execFileSync('powershell.exe',['-NoProfile','-ExecutionPolicy','Bypass','-File',defaultCheck],{cwd:dir,encoding:'utf8',windowsHide:true});
 fs.writeFileSync(helperFile,helper.replace('Throw New InvalidOperationException("DOC117_ALM03_DORMANT_SCOPE")','Return Doc117Alm03TestGate.Current.Iniciar()'));compile();
 execFileSync(csc,['/nologo','/target:exe','/out:'+exe,'/reference:'+json,path.join(ROOT,'tests/Doc117Alm03ObserverBehaviorTests.cs')],{cwd:dir,encoding:'utf8',windowsHide:true});
 const output=execFileSync(exe,[lib,dir],{cwd:dir,encoding:'utf8',windowsHide:true});assert.match(output,/NATIVE_OBSERVER: PASS/);
 const cases=JSON.parse(fs.readFileSync(path.join(dir,'native-results.json'),'utf8'));
 for(const c of cases){
  if(['entry-io','reservation'].includes(c.mode)){assert.ok(!fs.existsSync(path.join(c.dir,'summary.json')));continue;}
  const summary=JSON.parse(fs.readFileSync(path.join(c.dir,'summary.json'),'utf8')),scope={runId:summary.runId,resourceHashes:summary.resourceHashes};
  const events=fs.readdirSync(c.dir).filter(f=>/^(entry|exit)-/.test(f)&&fs.statSync(path.join(c.dir,f)).isFile()).map(f=>JSON.parse(fs.readFileSync(path.join(c.dir,f),'utf8')));
  if(c.mode==='overlap'){const assessed=assessMethodObservation(scope,events,summary);assert.equal(assessed.classification,'METHOD_BODIES_OVERLAPPED');assert.equal(assessed.writerBodyOverlapVerified,true);assert.equal(assessed.alm03ConcurrentPass,false);}
  else if(['serialized','duplicate'].includes(c.mode))assert.equal(assessMethodObservation(scope,events,summary).classification,'SERIALIZED_METHODS');
  else assert.throws(()=>assessMethodObservation(scope,events,summary),/METHOD_/);
 }
});
