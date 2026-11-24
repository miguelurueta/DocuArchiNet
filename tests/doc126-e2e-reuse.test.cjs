'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs'),path=require('node:path');
const {excelUiPlan,startExcelExternalWriter}=require('../tools/e2e/scripts/run-doc110-profile-session.cjs');
const support=require('../tools/e2e/tests/support/doc126-excel-scope.cjs'),previous=require('../tools/e2e/tests/support/doc125-excel-scope.cjs');
const root=path.resolve(__dirname,'..'),profile={baseUrl:'https://fixture.invalid/app/',module:'fixture',ignoreHttpsErrors:true};
function scope(mode='read'){
 const s={issue:'DOC-126',mode,baseUrl:profile.baseUrl,disposable:true,preserveResources:true,exclusiveAccountRequired:true,empresaId:2,usuarioId:141,restrictedUsuarioId:142,moduloId:11,
  controlPrefix:'DOC126_DA_E2E',raControlPrefix:'DOC126_RA_E2E',environmentHash:'a'.repeat(64),raEnvironmentHash:'b'.repeat(64),backendSha256:'c'.repeat(64),runtimeRoot:root,
  assets:Object.fromEntries(support.assetPaths.map(p=>[p,'d'.repeat(64)])),protectedTables:['imp_02_contratos','imp_02_egresos','imp_02_creditos'],
  destinationReadOnly:mode==='read',maxBusinessRows:mode==='read'?0:4000,maxDeletedRows:mode==='read'?0:2002,allowedEffects:mode==='read'?[]:['CREATE','INSERT','DELETE','RESULTADO','EXTERNAL_INSERT'],cases:[],selection:{conexionId:6,plantillaId:29}};
 if(mode==='write')s.cases=['crear','agregar','reemplazar','rollback','carrera'].map((name,i)=>({escenario:name,modo:['CREAR','AGREGAR','REEMPLAZAR','REEMPLAZAR','AGREGAR'][i],conexionId:6,plantillaId:[29,30,31,31,30][i],revisionEsperada:1,huellaEsperada:'e'.repeat(64),archivoHash:'f'.repeat(64),archivoPath:path.join(root,'.tmp','fixture-'+name+'.xlsx'),tabla:'imp_02_doc126_e2e_'+['crear','agregar','reemplazar','reemplazar','agregar'][i],hoja:'Datos',filasEsperadas:[1000,1000,1000,300,10][i],filasDestinoEsperadas:[0,1,2,1000,1001][i],codigoEsperado:i>=3?'CONFLICTO_CLAVE':'OK',...(name==='carrera'?{claveExterna:'DOC125R_synthetic',valorExterno:'synthetic-external'}:{})}));
 if(mode==='assistant')Object.assign(s,{maxBusinessRows:1000,maxDeletedRows:0,maxTemplateCreates:1,maxTemplateFieldCreates:2,analysisHandlerSha256:'e'.repeat(64),allowedEffects:['TEMPLATE_CREATE','CREATE','INSERT','RESULTADO'],assistant:{conexionId:6,tabla:'imp_02_doc126_e2e_asistente_1234abcd',archivoPath:path.join(root,'.tmp','fixture-assistant.xlsx'),archivoHash:'f'.repeat(64),hoja:'Datos',filasEsperadas:1000,campoUnico:'CLAVE',campos:['CLAVE','VALOR'].map(n=>({CAMPO_FUENTE:n,CAMPO_DESTINO:n,TIPO_CAMPO:'VARCHAR(100)'}))}});
 return s;
}
function env(t,s){const directory=fs.mkdtempSync(path.join(root,'.tmp','doc126-scope-')),file=path.join(directory,'scope.json');fs.writeFileSync(file,JSON.stringify(s));t.after(()=>{fs.unlinkSync(file);fs.rmdirSync(directory);});return {DOC126_E2E_SCOPE_PATH:file};}
test('DOC126 amplía el runner existente sin heredar autorización de fases anteriores',t=>{
 const s=scope(),e={...env(t,s),DOC125_E2E_AUTHORIZED:'YES',DOC110_E2E_AUTHORIZED:'YES'};
 const plan=excelUiPlan(path.join(root,'tools/e2e'),profile,['--excel-ui-read'],e);
 assert.equal(plan.values.DOC126_E2E_AUTHORIZED,'NO');assert.equal(plan.mode,'read');assert.ok(plan.childArgs.includes('@doc126-ui-read'));assert.ok(plan.childArgs.includes('tests/importacion-excel.spec.cjs'));assert.equal(plan.needed.length,10);assert.ok(plan.needed.includes('DOC126_RA_E2E_MYSQL_PASSWORD'));
});
test('DOC126 escritura exige scope propio, cinco casos y efectos explícitos; no reutiliza IDs ya confirmados',t=>{
 const s=scope('write');const plan=excelUiPlan(path.join(root,'tools/e2e'),profile,['--excel-ui-write'],env(t,s));assert.equal(plan.values.DOC126_E2E_AUTHORIZED,'NO');assert.ok(plan.childArgs.includes('@doc126-ui-write'));
 assert.throws(()=>previous.validateScope(s,s.baseUrl),/DOC125_SCOPE_INVALID/);
 for(const mutate of [x=>x.issue='DOC-125',x=>x.destinationReadOnly=true,x=>x.cases[0].tabla='imp_02_doc125_e2e_crear',x=>x.allowedEffects=[],x=>x.assets={},x=>x.cases[0].reanudarConfirmada={idCarga:'known'},x=>x.exclusiveAccountRequired=false,x=>x.raControlPrefix='OTHER',x=>x.maxBusinessRows=1]){
  const changed=JSON.parse(JSON.stringify(s));mutate(changed);assert.throws(()=>support.validateScope(changed,s.baseUrl,'write'));
 }
});
test('DOC126 modo lectura no admite recepción, edición, filas o efectos de escritura',()=>{
 const s=scope();for(const mutate of [x=>x.allowedEffects=['INSERT'],x=>x.maxBusinessRows=1,x=>x.destinationReadOnly=false,x=>x.cases=[{}],x=>x.restrictedUsuarioId=x.usuarioId]){
  const changed=JSON.parse(JSON.stringify(s));mutate(changed);assert.throws(()=>support.validateScope(changed,s.baseUrl,'read'),/DOC126_SCOPE_INVALID/);
 }
});
test('DOC126 rechaza mezclas de modos y otro ambiente antes de abrir una sesión',t=>{
 const e=env(t,scope());for(const args of [['--excel-ui-read','--excel-write'],['--excel-ui-read','--excel-ui-write'],['--excel-ui-read','--excel-confirm'],['--excel-ui-read','--source-profile']])assert.throws(()=>excelUiPlan(root,profile,args,e),/DOC126_MODE_CONFLICT/);
 assert.throws(()=>excelUiPlan(root,{...profile,baseUrl:'https://other.invalid/'},['--excel-ui-read'],e),/DOC126_SCOPE_INVALID/);
});
test('DOC126 actor externo existente exige autorización DOC126 aunque DOC125 esté autorizado',()=>{
 const s=scope('write'),item=s.cases.at(-1),prepared={IdCarga:'10000000-0000-4000-8000-000000000001',ArchivoHash:item.archivoHash,ConfiguracionHash:'a'.repeat(64),EsquemaHash:'b'.repeat(64)};
 assert.throws(()=>startExcelExternalWriter(s,item,prepared,{DOC125_E2E_AUTHORIZED:'YES',DOC126_E2E_AUTHORIZED:'NO',DOC125_NATIVE_HARNESS:'fixture'}),/DOC125_NATIVE_SCOPE_REQUIRED/);
});

test('DOC126 asistente requiere autorización propia y no hereda la de lectura/escritura',t=>{
 const s=scope('assistant'),e={...env(t,s),DOC126_E2E_AUTHORIZED:'YES'};
 const plan=excelUiPlan(path.join(root,'tools/e2e'),profile,['--excel-ui-assistant'],e);assert.equal(plan.mode,'assistant');assert.equal(plan.values.DOC126_E2E_AUTHORIZED,'NO');assert.equal(plan.values.DOC126_ASSISTANT_E2E_AUTHORIZED,'NO');assert.ok(plan.childArgs.includes('@doc126-ui-assistant'));
 const allowed=excelUiPlan(root,profile,['--excel-ui-assistant'],{...e,DOC126_ASSISTANT_E2E_AUTHORIZED:'YES'});assert.equal(allowed.values.DOC126_E2E_AUTHORIZED,'YES');assert.equal(allowed.values.DOC126_ASSISTANT_E2E_AUTHORIZED,'YES');
 for(const extra of ['--excel-ui-read','--excel-ui-write','--excel-confirm'])assert.throws(()=>excelUiPlan(root,profile,['--excel-ui-assistant',extra],e),/DOC126_MODE_CONFLICT/);
});

test('DOC126 scope del asistente acota una plantilla, dos campos, 1000 filas y cero borrados',()=>{
 const s=scope('assistant');assert.equal(support.validateScope(s,s.baseUrl,'assistant'),s);
 for(const mutate of [x=>x.maxTemplateCreates=2,x=>x.maxTemplateFieldCreates=3,x=>x.maxBusinessRows=1001,x=>x.maxDeletedRows=1,x=>x.allowedEffects.push('DELETE'),x=>delete x.analysisHandlerSha256,x=>x.assistant.tabla='imp_02_egresos',x=>x.assistant.campos.push({...x.assistant.campos[0]}),x=>x.assistant.campos[0].TIPO_CAMPO='VARCHAR(4097)',x=>x.assistant.campos[0].CAMPO_FUENTE='OTRA',x=>x.cases=[{}],x=>x.protectedTables.push(x.assistant.tabla)]){
  const changed=JSON.parse(JSON.stringify(s));mutate(changed);assert.throws(()=>support.validateScope(changed,s.baseUrl,'assistant'),/DOC126_SCOPE_INVALID/);
 }
 const protectedPrevious=scope('assistant');protectedPrevious.protectedTables.push('imp_02_doc126_e2e_agregar');assert.doesNotThrow(()=>support.validateScope(protectedPrevious,s.baseUrl,'assistant'));
});

test('DOC126 permiso de alta comprueba la solicitud exacta y conserva el bloqueo para otros modos',()=>{
 const s=scope('assistant'),a=s.assistant;
 const request={ConexionId:a.conexionId,RevisionEsperada:0,HuellaEsperada:'',Plantilla:{ID_PLANTILLAIMP:0,REVISION_CARGA:0,IMP01_XMLCONEXION_ID_XMLCONEXION:a.conexionId,NOMBRE_PLANTILLA:a.tabla,CAMPO_UNICO:'CLAVE',LIBRO_EXCEL:'Datos',Formato:'XLSX',Modo:'CREAR',ESTADO_PLANTILLA:1,Campos:a.campos.map(c=>({...c,ID_CAMPOS_PLANTILLA:0,REMPLAZA_COMA:0,REMPLAZA_PUNTO:0,OPCION_SPLITER:0,CARACTER_SPLITER:'',NUMERO_MATRIZ_EXPLITER:0}))}};
 assert.equal(support.templateRequestAllowed(s,request),true);assert.equal(support.templateRequestAllowed(scope('write'),request),false);assert.equal(support.templateRequestAllowed(scope(),request),false);
 for(const mutate of [r=>r.ConexionId=7,r=>r.RevisionEsperada=1,r=>r.Plantilla.ID_PLANTILLAIMP=29,r=>r.Plantilla.NOMBRE_PLANTILLA='imp_02_egresos',r=>r.Plantilla.Modo='REEMPLAZAR',r=>r.Plantilla.Campos[0].REMPLAZA_COMA=1,r=>r.Plantilla.Campos[0].TIPO_CAMPO='VARCHAR(250)',r=>r.Plantilla.Sql='untrusted',r=>r.Plantilla.Campos[0].Extra='untrusted']){
  const changed=JSON.parse(JSON.stringify(request));mutate(changed);assert.equal(support.templateRequestAllowed(s,changed),false);
 }
});

test('DOC126 usa nombres de locks del destino propio manteniendo el contrato DOC125',()=>{
 const physical={servidor:'synthetic',puerto:3306,base_ref:'DA',minusculas:1},id='11111111-1111-1111-1111-111111111111';
 const locks=support.lockNames(physical,'imp_02_doc126_e2e_agregar',id);
 // Vector del método VB real MySqlCargaExcelRepository.NombreLockDestino en el harness existente.
 assert.equal(locks.destination,'dt947a94d25b4278c6ed284659f052cfbd7ca62e408b15ffc4a7378a4ba13398');assert.equal(locks.id,'dx869344d7358066a12d2c04053db27211e5b1c6d084ca368e5f4d5c07f0bdc1');
 assert.throws(()=>previous.lockNames(physical,'imp_02_doc126_e2e_agregar',id),/DOC125_LOCK_IDENTITY_INVALID/);assert.throws(()=>support.lockNames(physical,'imp_02_doc125_e2e_agregar',id),/DOC125_LOCK_IDENTITY_INVALID/);assert.throws(()=>previous.lockNames(physical,'imp_02_doc126_e2e_agregar',id,'OTHER'),/DOC125_LOCK_IDENTITY_INVALID/);
 assert.equal(support.lockNames({...physical,base_ref:'da'},'imp_02_doc126_e2e_agregar',id).destination,locks.destination);
});
