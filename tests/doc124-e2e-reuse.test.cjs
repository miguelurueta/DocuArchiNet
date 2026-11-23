'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs'),os=require('node:os'),path=require('node:path');
const {excelPreparationPlan}=require('../tools/e2e/scripts/run-doc110-profile-session.cjs');
const {validateScope,readFixture}=require('../tools/e2e/tests/support/doc124-excel-scope.cjs');
const root=path.resolve(__dirname,'../tools/e2e'),profile={baseUrl:'https://example.invalid/test/',module:'fixture'};
function scope(){const c={modo:'AGREGAR',conexionId:7,plantillaId:5,revisionEsperada:1,huellaEsperada:'a'.repeat(64),tabla:'imp_02_doc124_sintetico',archivoPath:path.resolve('synthetic.xlsx'),archivoHash:'b'.repeat(64),hoja:'Datos',filasEsperadas:8000,codigoEsperado:'OK',incidenciasEsperadas:[]};return {issue:'DOC-124',disposable:true,baseUrl:profile.baseUrl,empresaId:2,usuarioId:1,moduloId:3,controlPrefix:'DOC110_DA_E2E',concurrencyCase:1,cases:[{...c,modo:'CREAR'},c,{...c,modo:'REEMPLAZAR'},{...c,codigoEsperado:'CLAVES_INVALIDAS',incidenciasEsperadas:['CLAVE_DUPLICADA_ARCHIVO']},{...c,codigoEsperado:'CLAVES_INVALIDAS',incidenciasEsperadas:['CLAVE_EXISTENTE_DESTINO']}]};}
function fixture(t,s=scope()){const dir=fs.mkdtempSync(path.join(os.tmpdir(),'doc124-scope-')),file=path.join(dir,'scope.json');fs.writeFileSync(file,JSON.stringify(s));t.after(()=>{fs.unlinkSync(file);fs.rmdirSync(dir);});return {DOC124_E2E_SCOPE_PATH:file};}
test('DOC124 reutiliza runner/consola y no hereda autorización DOC122/DOC110',t=>{
 const env={...fixture(t),DOC110_E2E_USER:'synthetic',DOC110_E2E_PASSWORD:'synthetic-only',DOC122_E2E_AUTHORIZED:'YES',DOC110_E2E_AUTHORIZED:'YES'};
 const plan=excelPreparationPlan(root,profile,['--excel-prepare'],env);assert.equal(plan.values.DOC124_E2E_AUTHORIZED,'NO');assert.equal(plan.values.DOC124_E2E_USER,'synthetic');assert.equal(plan.values.DOC124_E2E_BASE_URL,profile.baseUrl);assert.equal(plan.values.DOC124_E2E_MODULE,profile.module);assert.equal(plan.needed.length,7);assert.equal(plan.needed.includes('DOC110_DA_E2E_ODBC_DSN'),true);assert.equal(plan.childArgs.includes('@doc124-prepare'),true);
});
test('DOC124 conserva autorización explícita y rechaza modos mezclados',t=>{
 const env={...fixture(t),DOC124_E2E_AUTHORIZED:'YES'};assert.equal(excelPreparationPlan(root,profile,['--excel-prepare','--headed'],env).values.DOC124_E2E_AUTHORIZED,'YES');
 for(const args of [['--excel-read','--excel-prepare'],['--excel-prepare','--source-profile'],['--excel-prepare','--excel-write']])assert.throws(()=>excelPreparationPlan(root,profile,args,env),/DOC124_MODE_CONFLICT/);
});
test('DOC124 exige ambiente, cuentas en scope y matriz real de tres modos/duplicados',()=>{
 validateScope(scope(),profile.baseUrl);
 for(const mutate of [s=>s.issue='DOC-122',s=>s.disposable=false,s=>s.baseUrl='https://other.invalid/',s=>s.empresaId=0,s=>s.usuarioId=0,s=>s.moduloId=0,s=>s.cases.pop(),s=>s.cases[0].tabla='imp_02_egresos',s=>s.cases[0].tabla='imp_02_contratos',s=>s.cases[0].archivoPath='relative.xlsx',s=>s.cases[0].huellaEsperada='bad',s=>s.cases[1].filasEsperadas=1]){const s=scope();mutate(s);assert.throws(()=>validateScope(s,profile.baseUrl),/DOC124_SCOPE_INVALID/);}
});
test('DOC124 no abre sesión si falta alcance o fixture íntegro',t=>{
 assert.throws(()=>excelPreparationPlan(root,profile,['--excel-prepare'],{}),/DOC124_SCOPE_REQUIRED/);assert.throws(()=>readFixture(scope().cases[0]),/DOC124_FIXTURE_INVALID/);
});
test('preflight DOC124 sólo contiene SELECT y mantiene scripts históricos',()=>{
 const sql=fs.readFileSync(path.resolve(__dirname,'../Doc/Actualizacion/Importacion/Sql/11-preflight-doc124.sql'),'utf8').replace(/--[^\n]*/g,'');
 const statements=sql.split(';').map(x=>x.trim()).filter(Boolean);assert.ok(statements.length>=10);for(const stmt of statements)assert.match(stmt,/^SELECT\b/);
 assert.doesNotMatch(sql,/\b(?:INSERT|UPDATE|DELETE|CREATE|DROP|ALTER|SET|USE|CALL)\b/i);
});
