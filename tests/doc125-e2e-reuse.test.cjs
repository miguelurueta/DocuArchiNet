'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs'),os=require('node:os'),path=require('node:path');
const {excelConfirmationPlan,startExcelExternalWriter,excelPreparationPlan}=require('../tools/e2e/scripts/run-doc110-profile-session.cjs');
const {validateScope,readFixture,lockNames}=require('../tools/e2e/tests/support/doc125-excel-scope.cjs');
const root=path.resolve(__dirname,'../tools/e2e'),profile={baseUrl:'https://example.invalid/test/',module:'fixture'};
function scope(){
 const names=['crear','agregar','reemplazar','rollback','carrera'],modes=['CREAR','AGREGAR','REEMPLAZAR','REEMPLAZAR','AGREGAR'];
 return {issue:'DOC-125',disposable:true,preserveResources:true,baseUrl:profile.baseUrl,empresaId:2,usuarioId:1,moduloId:3,controlPrefix:'DOC110_DA_E2E',runtimeRoot:path.resolve('synthetic-runtime'),environmentHash:'e'.repeat(64),backendSha256:'f'.repeat(64),maxBusinessRows:5000,maxDeletedRows:2000,allowedEffects:['CREATE','INSERT','DELETE','RESULTADO','EXTERNAL_INSERT'],protectedTables:['imp_02_contratos'],cases:names.map((escenario,i)=>({escenario,modo:modes[i],conexionId:7,plantillaId:5+i,revisionEsperada:1,huellaEsperada:'a'.repeat(64),archivoHash:'b'.repeat(64),tabla:'imp_02_doc125_e2e_'+i,archivoPath:path.resolve('synthetic.xlsx'),hoja:'Datos',filasEsperadas:i===1?1000:2,filasDestinoEsperadas:i===0?0:2,codigoEsperado:i>=3?'CONFLICTO_CLAVE':'OK',...(escenario==='carrera'?{claveExterna:'DOC125R_SYNTHETIC'}:{})}))};
}
function fixture(t,s=scope()){const dir=fs.mkdtempSync(path.join(os.tmpdir(),'doc125-scope-')),file=path.join(dir,'scope.json');fs.writeFileSync(file,JSON.stringify(s));t.after(()=>{fs.unlinkSync(file);fs.rmdirSync(dir);});return {DOC125_E2E_SCOPE_PATH:file};}
test('DOC125 reutiliza runner, autenticación y controles sin heredar autorización DOC124',t=>{
 const env={...fixture(t),DOC124_E2E_AUTHORIZED:'YES',DOC110_E2E_AUTHORIZED:'YES',DOC110_E2E_USER:'synthetic',DOC110_E2E_PASSWORD:'synthetic-only'};
 const p=excelConfirmationPlan(root,profile,['--excel-confirm'],env);
 assert.equal(p.values.DOC125_E2E_AUTHORIZED,'NO');assert.equal(p.values.DOC125_E2E_USER,'synthetic');assert.equal(p.values.DOC125_E2E_BASE_URL,profile.baseUrl);assert.equal(p.needed.length,7);assert.equal(p.childArgs.includes('@doc125-confirm'),true);assert.equal(p.childArgs.includes('--retries=0'),true);assert.equal(p.scope.cases.length,5);
});
test('DOC125 conserva autorización propia y rechaza modos mezclados',t=>{
 const env={...fixture(t),DOC125_E2E_AUTHORIZED:'YES'};
 const p=excelConfirmationPlan(root,profile,['--excel-confirm','--headed'],env);assert.equal(p.values.DOC125_E2E_AUTHORIZED,'YES');assert.equal(p.childArgs.includes('--headed'),true);
 for(const args of [[],['--excel-add'],['--excel-confirm','--excel-prepare'],['--excel-confirm','--source-profile'],['--excel-confirm','--excel-confirm']])assert.throws(()=>excelConfirmationPlan(root,profile,args,env),/DOC125_MODE_CONFLICT/);
 assert.throws(()=>excelPreparationPlan(root,profile,['--excel-add','--excel-confirm'],env),/DOC124_MODE_CONFLICT/);
});
test('DOC125 exige destinos descartables exclusivos, efectos y límites identificados',()=>{
 validateScope(scope(),profile.baseUrl);
 for(const mutate of [s=>s.issue='DOC-124',s=>s.disposable=false,s=>s.destinationReadOnly=true,s=>s.preserveResources=false,s=>s.baseUrl='https://other.invalid/',s=>s.empresaId=0,s=>s.usuarioId=0,s=>s.moduloId=0,s=>s.runtimeRoot='relative',s=>s.backendSha256='bad',s=>s.environmentHash='bad',s=>s.maxBusinessRows=1,s=>s.maxBusinessRows=100001,s=>s.allowedEffects.pop(),s=>s.allowedEffects.push('INSTALL'),s=>s.protectedTables=[],s=>s.protectedTables=['imp_02_doc125_e2e_owned'],s=>s.cases[0].tabla='imp_02_contratos',s=>s.cases[0].tabla='imp_02_doc125_e2e_bad;DROP',s=>s.cases[0].archivoPath='relative.xlsx',s=>s.cases[0].archivoHash='bad',s=>s.cases[0].filasEsperadas=20001,s=>s.cases[1].filasEsperadas=999]){
  const s=scope();mutate(s);assert.throws(()=>validateScope(s,profile.baseUrl),/DOC125_SCOPE_INVALID/);
 }
});
test('DOC125 requiere la matriz real de rollback/UNIQUE y no acepta un éxito parcial',()=>{
 for(const mutate of [s=>s.cases.pop(),s=>s.cases[0].escenario='agregar',s=>s.cases[3].codigoEsperado='OK',s=>s.cases[3].modo='AGREGAR',s=>s.cases[4].claveExterna='existing-business-key',s=>s.cases[4].filasEsperadas=1001,s=>s.cases[0].tabla=s.cases[1].tabla,s=>s.maxDeletedRows=1,s=>s.cases[0].filasDestinoEsperadas=1,s=>s.cases[1].filasDestinoEsperadas=100001,s=>s.cases.reverse()]){const s=scope();mutate(s);assert.throws(()=>validateScope(s,profile.baseUrl),/DOC125_SCOPE_INVALID/);}
});
test('DOC125 escritor externo no se inicia con scopes o permisos de preparación',()=>{
 const s=scope(),c=s.cases[4],request={IdCarga:'11111111-1111-1111-1111-111111111111',ArchivoHash:c.archivoHash,ConfiguracionHash:'c'.repeat(64),EsquemaHash:'d'.repeat(64)};
 assert.throws(()=>startExcelExternalWriter(s,c,request,{DOC124_E2E_AUTHORIZED:'YES'}),/DOC125_NATIVE_SCOPE_REQUIRED/);
 assert.throws(()=>startExcelExternalWriter(s,s.cases[0],request,{DOC125_E2E_AUTHORIZED:'YES'}),/DOC125_NATIVE_SCOPE_REQUIRED/);
 assert.throws(()=>startExcelExternalWriter(s,c,{...request,ArchivoHash:'0'.repeat(64)},{DOC125_E2E_AUTHORIZED:'YES',DOC125_NATIVE_HARNESS:'unused'}),/DOC125_NATIVE_SCOPE_REQUIRED/);
});
test('DOC125 conserva verificación de hash del archivo existente antes de la carga',()=>{
 const p=path.resolve(__dirname,'Fixtures/ProduccionDocumental/ImportacionExcel/valido.xlsx'),crypto=require('node:crypto'),b=fs.readFileSync(p);
 assert.deepEqual(readFixture({archivoPath:p,archivoHash:crypto.createHash('sha256').update(b).digest('hex')}),b);
 assert.throws(()=>readFixture({archivoPath:p,archivoHash:'0'.repeat(64)}),/DOC124_FIXTURE_INVALID/);
});
test('DOC125 ejecutor nativo rechaza falta de autorización antes de leer recursos o abrir conexión',()=>{
 const {spawnSync}=require('node:child_process');
 const r=spawnSync('powershell.exe',['-NoProfile','-NonInteractive','-ExecutionPolicy','Bypass','-File',path.join(root,'scripts/support/doc115-repository-integration.ps1'),'-ResourcesFile',path.resolve('missing-resources.json'),'-RuntimeRoot',path.resolve('missing-runtime'),'-ExcelConfirmation'],{encoding:'utf8',windowsHide:true,env:{...process.env,DOC125_E2E_AUTHORIZED:'NO'}});
 assert.equal(r.status,1);assert.equal(r.stdout.trim(),'');assert.equal(r.stderr.trim(),'DOC125_NATIVE_EXTERNAL_FAILED_RESULT_UNCERTAIN');
});
test('DOC125 conserva redirecciones de login y clasifica rechazos sin retener cuerpos privados',async()=>{
 const {classifyLoginPostbackResponse}=require('../tools/e2e/tests/support/doc111-importacion-carpetas.cjs');
 let bodyReads=0;
 assert.equal(await classifyLoginPostbackResponse({status:()=>302,text:()=>{bodyReads++;throw Error('must not read redirect body');}}),null);
 assert.equal(bodyReads,0);
 assert.equal(await classifyLoginPostbackResponse({status:()=>403,text:()=>{throw Error('must not read denied body');}}),'LOGIN_HTTP_REJECTED');
 assert.equal(await classifyLoginPostbackResponse({status:()=>200,text:async()=> 'if(window.DocuArchiSegundoFactor){window.DocuArchiSegundoFactor.iniciar();}'}),'LOGIN_SECOND_FACTOR_REQUIRED');
 assert.equal(await classifyLoginPostbackResponse({status:()=>200,text:async()=>{throw Error('unavailable private body');}}),null);
});
test('DOC125 sólo reanuda confirmaciones positivas con ID y huellas congeladas',()=>{
 const s=scope();s.cases[0].reanudarConfirmada={idCarga:'11111111-1111-1111-1111-111111111111',configuracionHash:'c'.repeat(64),esquemaHash:'d'.repeat(64),huellaDestino:'e'.repeat(64)};validateScope(s,profile.baseUrl);
 for(const mutate of [x=>x.cases[0].reanudarConfirmada.idCarga='invalid',x=>x.cases[0].reanudarConfirmada.configuracionHash='bad',x=>x.cases[0].reanudarConfirmada.huellaDestino='bad',x=>{x.cases[3].reanudarConfirmada=x.cases[0].reanudarConfirmada;}]){const copy=JSON.parse(JSON.stringify(s));mutate(copy);assert.throws(()=>validateScope(copy,profile.baseUrl),/DOC125_SCOPE_INVALID/);}
});
test('DOC125 recuperar un ID no permite cambiar propietario, archivo ni resultado durable',()=>{
 const {resumeRequest}=require('../tools/e2e/tests/support/doc125-excel-scope.cjs'),s=scope(),c=s.cases[0];c.reanudarConfirmada={idCarga:'11111111-1111-1111-1111-111111111111',configuracionHash:'c'.repeat(64),esquemaHash:'d'.repeat(64),huellaDestino:'e'.repeat(64)};
 const row={id_carga:c.reanudarConfirmada.idCarga,id_plantilla:String(c.plantillaId),ambito_ref:String(s.empresaId),usuario_ref:String(s.usuarioId),conexion_ref:'DA:'+s.moduloId+':'+c.conexionId,tabla_destino:c.tabla,estado:'CONFIRMADA',modo:c.modo,filas_previstas:String(c.filasEsperadas),filas_confirmadas:String(c.filasEsperadas),confirmado:'1',archivo_hash:c.archivoHash,configuracion_hash:c.reanudarConfirmada.configuracionHash,esquema_hash:c.reanudarConfirmada.esquemaHash};
 assert.equal(resumeRequest(s,c,row).IdCarga,c.reanudarConfirmada.idCarga);
 for(const mutate of [r=>r.id_carga='22222222-2222-2222-2222-222222222222',r=>r.usuario_ref='999',r=>r.ambito_ref='999',r=>r.conexion_ref='DA:999:6',r=>r.id_plantilla='999',r=>r.tabla_destino='imp_02_contratos',r=>r.estado='PREPARADA',r=>r.archivo_hash='0'.repeat(64),r=>r.configuracion_hash='0'.repeat(64),r=>r.esquema_hash='0'.repeat(64),r=>r.filas_confirmadas='0',r=>r.confirmado='0']){const copy={...row};mutate(copy);assert.throws(()=>resumeRequest(s,c,copy),/DOC125_RESUME_OWNER_RESULT_CHANGED/);}
});
test('DOC125 conserva rollback conocido y carga pendiente sin permitir saltar estados',()=>{
 const {resumeRequest}=require('../tools/e2e/tests/support/doc125-excel-scope.cjs'),s=scope();
 for(const [index,option,state]of [[3,'reanudarFallida','FALLIDA'],[4,'continuarPreparada','PREPARADA']]){
  const c=s.cases[index];c[option]={idCarga:'11111111-1111-1111-1111-111111111111',configuracionHash:'c'.repeat(64),esquemaHash:'d'.repeat(64),huellaDestino:'e'.repeat(64)};
  const row={id_carga:c[option].idCarga,id_plantilla:c.plantillaId,ambito_ref:String(s.empresaId),usuario_ref:String(s.usuarioId),conexion_ref:'DA:'+s.moduloId+':'+c.conexionId,tabla_destino:c.tabla,estado:state,modo:c.modo,filas_previstas:c.filasEsperadas,filas_confirmadas:0,confirmado:0,codigo_incidencia:index===3?'CONFLICTO_CLAVE':null,archivo_hash:c.archivoHash,configuracion_hash:c[option].configuracionHash,esquema_hash:c[option].esquemaHash};
  validateScope(s,profile.baseUrl);assert.equal(resumeRequest(s,c,row).IdCarga,c[option].idCarga);
  for(const mutate of [r=>r.estado='CONFIRMADA',r=>r.filas_confirmadas=1,r=>r.confirmado=1,r=>r.codigo_incidencia='ESCRITURA_RECHAZADA']){const copy={...row};mutate(copy);assert.throws(()=>resumeRequest(s,c,copy),/DOC125_RESUME_OWNER_RESULT_CHANGED/);}
 }
 const changed=scope();changed.cases[0].reanudarFallida=s.cases[3].reanudarFallida;assert.throws(()=>validateScope(changed,profile.baseUrl),/DOC125_SCOPE_INVALID/);
 const both=scope();both.cases[4].continuarPreparada=s.cases[4].continuarPreparada;both.cases[4].reanudarConfirmada=s.cases[4].continuarPreparada;assert.throws(()=>validateScope(both,profile.baseUrl),/DOC125_SCOPE_INVALID/);
});
test('DOC125 controles de locks coinciden con el vector calculado por el código VB real',()=>{
 const physical={servidor:'synthetic',puerto:3306,base_ref:'DA',minusculas:1},id='11111111-1111-1111-1111-111111111111';
 //Vector obtenido de ReglasCargaExcel.HashDocumento/NombreLockDestino del harness compilado.
 assert.deepEqual(lockNames(physical,'imp_02_doc125_e2e_agregar',id),{destination:'dtc2ad372aadda4e8b18107d13e76110f7ae556542cfd3f91afa5c2fa7a64f5f',id:'dx869344d7358066a12d2c04053db27211e5b1c6d084ca368e5f4d5c07f0bdc1'});
 assert.equal(lockNames({...physical,base_ref:'da'},'imp_02_doc125_e2e_agregar',id).destination,lockNames(physical,'imp_02_doc125_e2e_agregar',id).destination);
 assert.throws(()=>lockNames({...physical,fisica:'bad'},'imp_02_doc125_e2e_agregar',id),/DOC125_LOCK_IDENTITY_INVALID/);
});
