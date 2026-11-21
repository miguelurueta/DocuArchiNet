'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs'),os=require('node:os'),path=require('node:path');
const {randomUUID}=require('node:crypto');
const {excelProfilePlan}=require('../tools/e2e/scripts/run-doc110-profile-session.cjs');
const root=path.resolve(__dirname,'../tools/e2e');
const profile={baseUrl:'https://example.invalid/test/',module:'fixture',ignoreHttpsErrors:true};
function fixture(t,change={}){
 const folder=fs.mkdtempSync(path.join(os.tmpdir(),'doc122-reuse-'));
 const scope=path.join(folder,'scope.json');
 t.after(()=>{fs.unlinkSync(scope);fs.rmdirSync(folder);});
 fs.writeFileSync(scope,JSON.stringify({issue:'DOC-122',disposable:true,baseUrl:profile.baseUrl,conexionId:7,plantillaId:5,revisionEsperada:1,huellaEsperada:'a'.repeat(64),allowedModes:['read','write'],...change}));
 return {DOC122_E2E_SCOPE_PATH:scope};
}
test('DOC122 usa perfil, runner y credenciales efímeras existentes sin heredar autorización DOC110',t=>{
 const env={...fixture(t),DOC110_E2E_USER:'fixture-owner',DOC110_E2E_PASSWORD:randomUUID(),DOC110_E2E_SECOND_USER:'fixture-restricted',DOC110_E2E_SECOND_PASSWORD:randomUUID(),DOC110_E2E_AUTHORIZED:'YES'};
 const plan=excelProfilePlan(root,profile,['--excel-read'],env);
 assert.equal(plan.values.DOC122_E2E_BASE_URL,profile.baseUrl);assert.equal(plan.values.DOC122_E2E_MODULE,profile.module);
 assert.equal(plan.values.DOC122_E2E_USER,env.DOC110_E2E_USER);assert.equal(plan.values.DOC122_E2E_PASSWORD,env.DOC110_E2E_PASSWORD);
 assert.equal(plan.values.DOC122_E2E_RESTRICTED_USER,env.DOC110_E2E_SECOND_USER);assert.equal(plan.values.DOC122_E2E_RESTRICTED_PASSWORD,env.DOC110_E2E_SECOND_PASSWORD);
 assert.equal(plan.values.DOC122_E2E_AUTHORIZED,'NO');
 assert.deepEqual(plan.childArgs.slice(1),['test','tests/importacion-excel.spec.cjs','--grep','@doc122-read','--reporter=list','--retries=0']);
 assert.equal(fs.existsSync(plan.childArgs[0]),true);
});
test('DOC122 conserva selección explícita y modo escritura sin requerir segunda cuenta',t=>{
 const env={...fixture(t),DOC122_E2E_USER:'selected-fixture',DOC110_E2E_USER:'other-fixture',DOC122_E2E_AUTHORIZED:'YES'};
 const plan=excelProfilePlan(root,profile,['--excel-write','--headed'],env);
 assert.equal(plan.values.DOC122_E2E_USER,'selected-fixture');assert.equal(plan.values.DOC122_E2E_AUTHORIZED,'YES');
 assert.deepEqual(plan.needed,['DOC122_E2E_USER','DOC122_E2E_PASSWORD']);assert.equal(plan.childArgs.at(-1),'--headed');
 assert.equal(plan.childArgs.includes('@doc122-write'),true);
});
test('DOC122 no autentica ni mezcla modos que pueden cambiar permisos o sembrar datos',t=>{
 const env=fixture(t);
 for(const args of [['--excel-read','--excel-write'],['--excel-read','--permission-revocation'],['--excel-write','--missing-schema'],['--excel-read','--source-profile']])
  assert.throws(()=>excelProfilePlan(root,profile,args,env),/DOC122_MODE_CONFLICT/);
});
test('DOC122 rechaza ámbito ausente o de otro ambiente antes de crear proceso o sesión',t=>{
 assert.throws(()=>excelProfilePlan(root,profile,['--excel-read'],{}),/DOC122_SCOPE_REQUIRED/);
 assert.throws(()=>excelProfilePlan(root,profile,['--excel-read'],fixture(t,{baseUrl:'https://other.invalid/'})),/DOC122_SCOPE_INVALID/);
});
test('DOC122 rechaza escritura si solo fue autorizada lectura',t=>{
 assert.throws(()=>excelProfilePlan(root,profile,['--excel-write'],fixture(t,{allowedModes:['read']})),/DOC122_SCOPE_INVALID/);
});
test('DOC122 exige identidad y versión concreta de una plantilla descartable',t=>{
 for(const change of [{disposable:false},{conexionId:0},{plantillaId:0},{revisionEsperada:0},{huellaEsperada:'invalid'},{allowedModes:['read','seed']}])
  assert.throws(()=>excelProfilePlan(root,profile,['--excel-read'],fixture(t,change)),/DOC122_SCOPE_INVALID/);
});
