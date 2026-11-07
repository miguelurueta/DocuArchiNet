'use strict';
const syntheticHistory=require('./support/doc117-history-fixture.cjs').install();
const {test}=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto'),{execFileSync}=require('node:child_process');
const policy=require('../tools/e2e/scripts/support/doc117-alm03-canonical-description.cjs'),{ROOT,bindingFor,validateBinding,readBinding,transformGuard,generateDormant}=policy;
test('catalog binding preserves exact representation and rejects forged hashes, extra fields and different identities',async()=>{
 const upper=bindingFor('COMPROBANTE DE EGRESO'),mixed=bindingFor('Comprobante de Egreso');assert.notEqual(upper.descriptionSha256,mixed.descriptionSha256);assert.equal(upper.description,'COMPROBANTE DE EGRESO');assert.ok(Object.isFrozen(validateBinding({...upper})));
 for(const value of ['Otro documento','Comprobante de Egreso\u0000','Comprobante de Egreso\ud800','',null])assert.throws(()=>bindingFor(value));
 for(const change of [{typologyId:'79'},{companyId:'3'},{expedientId:'666'},{descriptionSha256:mixed.descriptionSha256},{extra:true}])assert.throws(()=>validateBinding({...upper,...change}));
 const query=async(origin,sql,parameters)=>{assert.equal(origin,'RA');assert.match(sql,/^SELECT\b/);assert.ok(!sql.includes(';'));assert.match(sql,/t.Estado_Tipo=1/);assert.match(sql,/i.Estado_instrumento=1/);assert.deepEqual(parameters,['467','2','2','50']);return[{type_id:'50',canonical_description:upper.description}];};assert.deepEqual(await readBinding(query),upper);
 for(const rows of [[],[{type_id:'79',canonical_description:upper.description}],[{type_id:'50',canonical_description:upper.description},{type_id:'50',canonical_description:upper.description}]])await assert.rejects(readBinding(async()=>rows));
});
test('canonical guard transformation preserves all other comparisons and does not rewrite writer arguments',()=>{
 const original=fs.readFileSync(path.join(ROOT,'tools/e2e/fixtures/Doc117Alm03ClosedGuard.vb'),'utf8'),updated=transformGuard(original,bindingFor('COMPROBANTE DE EGRESO'));assert.ok(updated.includes('Not MatchesCanonicalDescription(nombre)'));assert.ok(updated.includes('nombre <> value.Name'));assert.ok(updated.includes('Return Prepare(h, "INDIVIDUAL", gabinete, ruta, nombre,'));assert.throws(()=>transformGuard(updated),/GUARD_CHANGED/);
 const dir=path.join(ROOT,'.tmp','doc117-canonical-generation-'+crypto.randomUUID()),generated=generateDormant(dir);assert.equal(generated.writerAllowed,false);assert.equal(generated.canonicalBindingPresent,false);assert.ok(fs.readFileSync(path.join(dir,'Doc117Alm03ClosedGuard.Canonical.vb'),'utf8').includes('NO_CANONICAL_BINDING'));assert.throws(()=>generateDormant(dir),/FRESH_DIRECTORY_REQUIRED/);assert.throws(()=>generateDormant(path.join(ROOT,'bin','canonical')),/FRESH_DIRECTORY_REQUIRED/);
});
test('compiled native guard accepts an exact catalog label but rejects equivalent drift, wrong selection, changed file and writer argument mutation',()=>{
 const base=path.join(ROOT,'.tmp','doc117-canonical-native-'+crypto.randomUUID());fs.mkdirSync(base);require('./doc110-perfil-importacion-behavior.test.cjs').compileBehaviorHarness(base);
 const compiler=name=>execFileSync('powershell.exe',['-NoProfile','-Command','(Get-Command '+name+' -ErrorAction Stop).Source'],{encoding:'utf8',windowsHide:true}).trim(),vbc=compiler('vbc.exe'),csc=compiler('csc.exe'),historical=JSON.parse(fs.readFileSync(path.join(ROOT,'.tmp/doc117-alm03-setup-57219fc625b867b54d8d282b6b1eb0a2-scope.json'),'utf8')),render=require('../tools/e2e/scripts/support/doc117-alm03-scoped-candidate.cjs').renderGuard;
 for(const [index,description]of ['COMPROBANTE DE EGRESO','  Comprobante\u00a0de  Egreso  ','Ｃｏｍｐｒｏｂａｎｔｅ de Egreso',null].entries()){
  const dir=path.join(base,String(index));fs.mkdirSync(dir);for(const name of ['PerfilImportacion.dll','Newtonsoft.Json.dll'])fs.copyFileSync(path.join(base,name),path.join(dir,name));
  const lib=path.join(dir,'Canonical.dll'),exe=path.join(dir,'canonical-tests.exe'),guard=path.join(dir,'guard.vb'),observer=path.join(dir,'observer.vb'),scope={...historical,runId:crypto.randomBytes(16).toString('hex')};
  const text=transformGuard(render(scope),description?bindingFor(description):null).replace(/Private Const Site As String = "[^"]+"/,'Private Const Site As String = "'+dir+'"');fs.writeFileSync(guard,text);fs.copyFileSync(path.join(ROOT,'tools/e2e/fixtures/Doc117Alm03MethodObserver.vb'),observer);
  execFileSync(vbc,['/nologo','/target:library','/out:'+lib,...['System.dll','System.Core.dll','System.Web.dll',path.join(dir,'PerfilImportacion.dll'),path.join(dir,'Newtonsoft.Json.dll')].map(f=>'/reference:'+f),guard,observer],{cwd:dir,encoding:'utf8',windowsHide:true});
  execFileSync(csc,['/nologo','/target:exe','/out:'+exe,...['System.dll','System.Core.dll','System.Web.dll'].map(f=>'/reference:'+f),path.join(ROOT,'tests/Doc117Alm03CanonicalDescriptionTests.cs')],{cwd:dir,encoding:'utf8',windowsHide:true});
  assert.match(execFileSync(exe,[lib,path.join(ROOT,'tools/e2e/fixtures/doc111-carga-individual.pdf'),Buffer.from(description||'Comprobante de Egreso','utf8').toString('base64'),description?'bound':'dormant'],{cwd:dir,encoding:'utf8',windowsHide:true}),/CANONICAL_GUARD: PASS/);
 }
});
