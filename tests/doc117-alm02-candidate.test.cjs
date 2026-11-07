'use strict';
const {test}=require('node:test'),assert=require('node:assert/strict');
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto'),{execFileSync}=require('node:child_process');
const {ROOT,BINDING,TEMPLATE,PROTOCOL,sha,overlay,generate,renderHelper,validateScope}=require('../tools/e2e/scripts/support/doc117-alm02-candidate.cjs');
const fresh=tag=>path.join(ROOT,'.tmp','doc117-alm02-test-'+tag+'-'+crypto.randomUUID());
test('Offline overlay rejects changed bindings and preserves the production invocation',()=>{
  const source=fs.readFileSync(path.join(ROOT,BINDING),'utf8'),result=overlay(source);
  assert.throws(()=>overlay(source.replace('RegistrarResultadoDocumento','Other')),/BINDING_CHANGED/);
  assert.throws(()=>overlay(source+source),/BINDING_CHANGED/);
  assert.equal(result.slice(result.indexOf('    Private Shared Sub InvocarAlmacenamiento')),source.slice(source.indexOf('    Private Shared Sub InvocarAlmacenamiento')));
  const dir=fresh('overlay'),runtime=path.join(ROOT,'bin/GestionDocumental-Docuarchi.net.dll'),before=sha(fs.readFileSync(runtime));
  const r=generate(dir);assert.equal(r.sourceBindingSha256,sha(source));assert.equal(sha(fs.readFileSync(runtime)),before);
  assert.throws(()=>generate(dir),/FRESH_DIRECTORY_REQUIRED/);
  assert.throws(()=>generate(path.join(ROOT,'bin/doc117-test')),/FRESH_DIRECTORY_REQUIRED/);
});
test('Hash matches actual coordinator and attempt pin uses actual immutable product commands',()=>{
 const dir=fresh('product');fs.mkdirSync(dir,{recursive:true});
 const {compileBehaviorHarness}=require('./doc110-perfil-importacion-behavior.test.cjs');compileBehaviorHarness(dir);
 const command=name=>execFileSync('powershell.exe',['-NoProfile','-Command','(Get-Command '+name+' -ErrorAction Stop).Source'],{encoding:'utf8',windowsHide:true}).trim();
 const vbc=command('vbc.exe'),csc=command('csc.exe'),helper=path.join(dir,'helper.vb'),lab=path.join(dir,'Doc117Candidate.dll'),exe=path.join(dir,'pin-tests.exe'),scopeFile=path.join(dir,'synthetic-scope.json');
 const compile=()=>execFileSync(vbc,['/nologo','/target:library','/out:'+lab,'/reference:System.dll','/reference:System.Core.dll','/reference:'+path.join(dir,'PerfilImportacion.dll'),'/reference:'+path.join(dir,'Newtonsoft.Json.dll'),helper,path.join(ROOT,'tools/e2e/fixtures/Doc117SolicitudHash.vb')],{cwd:dir,encoding:'utf8',windowsHide:true});
 fs.copyFileSync(path.join(ROOT,TEMPLATE),helper);compile();
 execFileSync(csc,['/nologo','/target:exe','/out:'+exe,'/reference:System.dll','/reference:System.Core.dll','/reference:System.Web.dll','/reference:System.Data.dll',...['PerfilImportacion.dll','PerfilImportacionTests.exe','Doc117Candidate.dll','Newtonsoft.Json.dll'].map(f=>'/reference:'+path.join(dir,f)),path.join(ROOT,'tests/Doc117AttemptPinBehaviorTests.cs')],{cwd:dir,encoding:'utf8',windowsHide:true});
 execFileSync(exe,['--describe',scopeFile],{cwd:dir,encoding:'utf8',windowsHide:true});
 fs.writeFileSync(helper,renderHelper(JSON.parse(fs.readFileSync(scopeFile,'utf8'))));compile();
 const output=execFileSync(exe,['--verify'],{cwd:dir,encoding:'utf8',windowsHide:true});
 assert.match(output,/DOC117_ACTUAL_COORDINATOR_HASH_PARITY: PASS/);assert.match(output,/DOC117_ATTEMPT_PIN_WITH_ACTUAL_IMMUTABLE_COMMAND: PASS/);
});
test('Closed scope rejects predicted attempts, changed resources and unsafe site paths',()=>{
 const s={issueKey:'DOC-117',protocol:PROTOCOL,runtimeRoot:ROOT,runId:'e'.repeat(32),lotId:'900',snapshotSha256:'f'.repeat(64),resources:[{rootId:'901',fileId:'902',expedienteId:'910',relacionNivelId:'911',fileName:'a.pdf',bytes:'677',contentSha256:'d'.repeat(64),requestSha256:'a'.repeat(64)},{rootId:'904',fileId:'905',expedienteId:'912',relacionNivelId:'913',fileName:'b.pdf',bytes:'677',contentSha256:'d'.repeat(64),requestSha256:'b'.repeat(64)}]};
 assert.doesNotThrow(()=>validateScope(s));const predicted=structuredClone(s);predicted.resources[0].attemptId='999';assert.throws(()=>validateScope(predicted),/SCOPE_FIELDS_CHANGED/);
 assert.throws(()=>validateScope({...s,lotId:'18'}),/SCOPE_INVALID/);assert.throws(()=>validateScope({...s,runtimeRoot:path.resolve(ROOT,'..')}),/SITE_INVALID/);
 const collision=structuredClone(s);collision.resources[1].fileId=collision.resources[0].fileId;assert.throws(()=>validateScope(collision),/RESOURCE_COLLISION/);
 const name=structuredClone(s);name.resources[0].fileName='../other.pdf';assert.throws(()=>validateScope(name),/RESOURCE_INVALID/);
});
test('Compiled laboratory helper refuses dormant scope and verifies both windows with local repository doubles',()=>{
  const dir=fresh('compiled');fs.mkdirSync(dir,{recursive:true});
  const vbc=execFileSync('powershell.exe',['-NoProfile','-Command','(Get-Command vbc.exe -ErrorAction Stop).Source'],{encoding:'utf8',windowsHide:true}).trim();
  const template=fs.readFileSync(path.join(ROOT,TEMPLATE),'utf8');
  const json=path.join(ROOT,'packages/Newtonsoft.Json.13.0.2/lib/net45/Newtonsoft.Json.dll');
  for(const dormant of [true,false]){
    const out=path.join(dir,dormant?'dormant':'test-double-scope');fs.mkdirSync(out);
    let helper=template;
    // Synthetic IDs and hashes only in disposable test copies.
    if(!dormant)helper=renderHelper({issueKey:'DOC-117',protocol:PROTOCOL,runtimeRoot:out,runId:'e'.repeat(32),lotId:'900',snapshotSha256:'f'.repeat(64),resources:[{rootId:'901',fileId:'902',expedienteId:'910',relacionNivelId:'911',fileName:'test-a.pdf',bytes:'677',contentSha256:'d'.repeat(64),requestSha256:'a'.repeat(64)},{rootId:'904',fileId:'905',expedienteId:'912',relacionNivelId:'913',fileName:'test-b.pdf',bytes:'677',contentSha256:'d'.repeat(64),requestSha256:'b'.repeat(64)}]});
    const file=path.join(out,'helper.vb'),exe=path.join(out,'candidate-tests.exe');fs.writeFileSync(file,helper,{flag:'wx'});fs.copyFileSync(json,path.join(out,'Newtonsoft.Json.dll'));
    execFileSync(vbc,['/nologo','/target:exe','/optionstrict+','/out:'+exe,'/reference:System.dll','/reference:System.Core.dll','/reference:'+json,file,path.join(ROOT,'tests/Fixtures/ProduccionDocumental/Doc117CandidateTestDoubles.vb')],{cwd:out,encoding:'utf8',windowsHide:true});
    const output=execFileSync(exe,dormant?['dormant']:[],{cwd:out,encoding:'utf8',windowsHide:true});
    assert.match(output,dormant?/DOC117_DORMANT_REFUSES_BEFORE_EFFECT: PASS/:/DOC117_INTERNAL_WINDOWS_AND_SCOPE_WITH_DOUBLES: PASS/);
  }
});
