'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs'),os=require('node:os'),path=require('node:path');
const {excelPreparationPlan}=require('../tools/e2e/scripts/run-doc110-profile-session.cjs');
const {validateScope,readFixture,snapshotDestination}=require('../tools/e2e/tests/support/doc124-excel-scope.cjs');
const {sourceFixture}=require('../tools/e2e/tests/support/doc110-source-fixture.cjs');
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
test('DOC124 archivo real usa el mismo runner y no sustituye la matriz completa',t=>{
 const s=scope();s.escenario='archivo';s.maxFilasDatos=20000;s.cases=[{...s.cases[0],filasEsperadas:12759}];s.concurrencyCase=0;
 const env={...fixture(t,s),DOC124_E2E_AUTHORIZED:'YES'},plan=excelPreparationPlan(root,profile,['--excel-file'],env);
 assert.equal(plan.mode,'file');assert.equal(plan.cases,1);assert.equal(plan.childArgs.includes('@doc124-file'),true);
 assert.throws(()=>excelPreparationPlan(root,profile,['--excel-prepare'],env),/DOC124_SCOPE_INVALID/);
 assert.throws(()=>excelPreparationPlan(root,profile,['--excel-file','--excel-prepare'],env),/DOC124_MODE_CONFLICT/);
 for(const change of [x=>delete x.escenario,x=>x.escenario='all',x=>x.cases[0].modo='AGREGAR',x=>x.maxFilasDatos=10000]){const invalid=structuredClone(s);change(invalid);assert.throws(()=>validateScope(invalid,profile.baseUrl),/DOC124_SCOPE_INVALID/);}
});
test('DOC124 exige ambiente, cuentas en scope y matriz real de tres modos/duplicados',()=>{
 validateScope(scope(),profile.baseUrl);
 for(const mutate of [s=>s.issue='DOC-122',s=>s.disposable=false,s=>s.baseUrl='https://other.invalid/',s=>s.empresaId=0,s=>s.usuarioId=0,s=>s.moduloId=0,s=>s.cases.pop(),s=>s.cases[0].tabla='imp_02_egresos;DROP',s=>s.cases[0].archivoPath='relative.xlsx',s=>s.cases[0].huellaEsperada='bad',s=>s.cases[1].filasEsperadas=1]){const s=scope();mutate(s);assert.throws(()=>validateScope(s,profile.baseUrl),/DOC124_SCOPE_INVALID/);}
});
test('DOC124 permite reutilizar fuentes DOC110 y tablas existentes sin declararlas descartables',()=>{
 for(const table of [sourceFixture('imp_02_egresos').table,sourceFixture('imp_02_creditos').table,'imp_02_contratos']){
  const s=scope();s.disposable=false;s.destinationReadOnly=true;s.cases.filter(c=>c.modo!=='CREAR').forEach(c=>c.tabla=table);assert.equal(validateScope(s,profile.baseUrl),s);
 }
 const s=scope();s.disposable=false;s.destinationReadOnly=false;assert.throws(()=>validateScope(s,profile.baseUrl),/DOC124_SCOPE_INVALID/);
});
test('DOC124 admite el archivo de órdenes con12759 filas y el máximo configurado20000',()=>{
 for(const count of [12759,20000]){const s=scope();s.cases.forEach(c=>c.filasEsperadas=count);assert.equal(validateScope(s,profile.baseUrl),s);}
 const s=scope();s.cases[0].filasEsperadas=20001;assert.throws(()=>validateScope(s,profile.baseUrl),/DOC124_SCOPE_INVALID/);
});
test('DOC124 reutiliza huella completa DOC32 para una fuente existente de más de10000 filas',async()=>{
 const table=sourceFixture('imp_02_egresos').table,calls=[];let hash='a'.repeat(64);
 const query=async(sql,args)=>{calls.push({sql,args});assert.match(sql,/^SELECT\b/);assert.deepEqual(args,[table]);if(sql.includes('information_schema.TABLES'))return [{ENGINE:'InnoDB',TABLE_TYPE:'BASE TABLE',TABLE_COLLATION:'latin1_swedish_ci'}];if(sql.includes('information_schema.COLUMNS'))return [{COLUMN_NAME:'CONSECUTIVO',COLUMN_TYPE:'varchar(250)'},{COLUMN_NAME:'BENEFICIARIO',COLUMN_TYPE:'varchar(255)'}];if(sql.includes('information_schema.STATISTICS'))return [];throw Error('unsupported control');};
 const fingerprint=async(sql,marker)=>{calls.push({sql,marker});assert.equal(marker,'DOC124');assert.match(sql,/^SELECT \* FROM `imp_02_egresos` WHERE \? IS NOT NULL ORDER BY BINARY `CONSECUTIVO`,BINARY `BENEFICIARIO`$/);assert.doesNotMatch(sql,/LIMIT|OFFSET|INSERT|UPDATE|DELETE/);return hash;};
 const before=await snapshotDestination(query,fingerprint,table,true);assert.equal(calls.filter(c=>c.sql.startsWith('SELECT *')).length,1);assert.equal(calls.filter(c=>c.args).length,3);
 assert.equal(await snapshotDestination(query,fingerprint,table,true),before);hash='b'.repeat(64);assert.notEqual(await snapshotDestination(query,fingerprint,table,true),before);
});
test('DOC124 protege nombre/ausencia de CREAR y no toma muestra como huella de origen',async()=>{
 await assert.rejects(snapshotDestination(async()=>[],async()=>'', 'imp_02_x;DROP',true),/DOC124_SCOPE_INVALID/);
 await assert.rejects(snapshotDestination(async()=>[],async()=>'', 'imp_02_egresos',true),/DOC124_DESTINATION_SCOPE_CHANGED/);
 let fingerprintCalls=0;const result=await snapshotDestination(async()=>[],async()=>{fingerprintCalls++;return 'a'.repeat(64);},'imp_02_ausente',false);assert.match(result,/^[a-f0-9]{64}$/);assert.equal(fingerprintCalls,0);
});
test('DOC124 no abre sesión si falta alcance o fixture íntegro',t=>{
 assert.throws(()=>excelPreparationPlan(root,profile,['--excel-prepare'],{}),/DOC124_SCOPE_REQUIRED/);assert.throws(()=>readFixture(scope().cases[0]),/DOC124_FIXTURE_INVALID/);
});
test('preflights DOC124 sólo contienen SELECT y mantienen scripts históricos',()=>{
 for(const file of ['11-preflight-doc124.sql','12-preflight-recursos-e2e-doc124.sql','14-preflight-contratos-e2e-doc124.sql']){
  const sql=fs.readFileSync(path.resolve(__dirname,'../Doc/Actualizacion/Importacion/Sql',file),'utf8').replace(/--[^\n]*/g,'');
  const statements=sql.split(';').map(x=>x.trim()).filter(Boolean);assert.ok(statements.length>=10);for(const stmt of statements)assert.match(stmt,/^SELECT\b/);
  assert.doesNotMatch(sql,/\b(?:INSERT|UPDATE|DELETE|CREATE|DROP|ALTER|SET|USE|CALL)\b/i);
 }
});
