'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),cp=require('node:child_process'),path=require('node:path'),fs=require('node:fs');
const diagnostic=require('../tools/e2e/scripts/inspect-doc120-creation-prerequisites.cjs');
const root=path.resolve(__dirname,'..'),bin=path.join(root,'bin');let exe,rows,fixtureExe;
test.before(()=>{
 exe=diagnostic.compile();const fixture=path.join(path.dirname(exe),'Doc120PrerequisiteFixture.exe');fixtureExe=fixture;
 cp.execFileSync(path.join(process.env.WINDIR,'Microsoft.NET/Framework64/v4.0.30319/csc.exe'),['/nologo','/target:exe','/out:'+fixture,'/r:System.Core.dll','/r:'+path.join(bin,'Newtonsoft.Json.dll'),'/r:'+path.join(bin,'GestionDocumental-Docuarchi.net.dll'),path.join(__dirname,'support/Doc120PrerequisiteFixture.cs')],{windowsHide:true,stdio:'pipe'});
 rows=JSON.parse(cp.execFileSync(fixture,[bin],{encoding:'utf8',windowsHide:true}));
});
const scope={lot:'39',root:'60',attempt:'96'};
function data(){return diagnostic.payload(structuredClone(rows),scope);}
function catalog(q){
 if(q.kind==='ORIGIN')return[{host:'host-DA',puerto:'3306',servidor:'1',base:'base-DA'}];
 if(q.kind==='LEVEL_AUTHORITY')return[{id_nivel:'7'}];if(q.kind==='RA_IDENTITY')return[{RELACION_DA:'10'}];if(q.kind==='DA_IDENTITY')return[{Clave_Usuario:'10'}];
 if(q.kind==='CABINET_AUTHORITY')return[{id_gabinete:'7',INVENTARIO_DOCUMENTAL:'1',APLICA_TRD:'1',ASIGNA_UNIDAD:'1'}];
 if(q.kind==='RESPONSIBLE')return[{id:'8',nombre:'Responsible',identificacion:'00001234'}];
 const table=[...q.sql.matchAll(/\bFROM\s+([A-Za-z_][A-Za-z0-9_]*)/g)][0]?.[1];
 const ref={ra_de_tipos_ciclos_archivo:['13','Archivo Gestión'],registro_organigrama:['11','Organigrama'],ra_de_fondo_documental:['1','Fondo'],areas_depart_radicacion:['3','Area'],series_documentales:['4','Serie'],subseries_documentales:['12','Subserie'],ra_tipo_expediente:['5','Tipo'],tipos_unidad_documental:['6','Unidad'],system1:['7','Gabinete']}[table];
 assert.ok(ref,'Unexpected synthetic catalog table '+table);return[{id:ref[0],nombre:ref[1]}];
}
test('DOC120-PRE01: clases reales validan copia contrafactual con solo SELECT y sin alterar la reserva',async()=>{
 const input=data(),before=JSON.stringify(input),reads=[];const output=await diagnostic.inspect(input,async q=>{reads.push(q);return catalog(q);},exe);
 assert.equal(output.status,'PASS',JSON.stringify(output));assert.equal(output.code,'CATALOG_COMPATIBLE');assert.equal(output.counterfactual,true);assert.equal(output.recoveryAuthorized,false);assert.equal(output.sessionAuthorityChecked,false);assert.equal(output.startTransactionChecked,false);
 assert.equal(JSON.stringify(input),before);assert.equal(input.lot.DetencionSolicitada,true);assert.ok(reads.length>15&&reads.length<60);assert.doesNotMatch(JSON.stringify(output),/synthetic-private|Archivo Gestión|a{32}|b{64}/);
});
test('DOC120-PRE02: dato incompatible reproduce rechazo antes de leer catálogos, sin probar causa histórica',async()=>{
 const input=data();input.root.ValoresPlan.find(v=>v.DestinoRef==='NOMBRE').Valor="O'Brien";let reads=0;const output=await diagnostic.inspect(input,async()=>{reads++;return[];},exe);
 assert.equal(output.stage,'LEGACY_INPUT');assert.equal(output.code,'CONFIGURACION_INVALIDA');assert.equal(output.status,'STOPPED');assert.equal(reads,0);assert.equal(output.recoveryAuthorized,false);
});
test('DOC120-PRE03: catálogo ausente o ambiguo conserva el rechazo real y su etapa',async()=>{
 for(const count of [0,2]){let reads=0;const output=await diagnostic.inspect(data(),async q=>{reads++;if(q.sql.includes('FROM ra_de_tipos_ciclos_archivo'))return Array.from({length:count},()=>({id:'13',nombre:'Archivo Gestión'}));return catalog(q);},exe);assert.equal(output.stage,'CATALOG');assert.equal(output.code,'ACCESO_DENEGADO');assert.equal(output.status,'STOPPED');assert.ok(reads<15);}
});
test('DOC120-PRE04: SQL del catálogo se limita a origen/tablas permitidas y parámetros posicionales',()=>{
 const q=diagnostic.statement({type:'SELECT',origin:'RA',sql:'SELECT id,nombre FROM remit_dest_interno WHERE id=@id OR id=@id',parameters:['@id','8']});assert.deepEqual(q.parameters,['8','8']);assert.equal(q.sql,'SELECT id,nombre FROM remit_dest_interno WHERE id=? OR id=?');
 const origin=diagnostic.statement({type:'SELECT',origin:'DA',sql:'SELECT @@hostname AS host,@@port AS puerto,@@server_id AS servidor,DATABASE() AS base',parameters:[]});assert.deepEqual(origin.parameters,[]);
 for(const sql of ['UPDATE remit_dest_interno SET id=1','SELECT * FROM gestor_modulos','SELECT * FROM remit_dest_interno FOR UPDATE','SELECT * FROM remit_dest_interno; DELETE FROM remit_dest_interno','SELECT * FROM remit_dest_interno -- SELECT'])assert.throws(()=>diagnostic.statement({type:'SELECT',origin:'RA',sql,parameters:[]}),/SELECT_DENIED/);
 assert.throws(()=>diagnostic.statement({type:'SELECT',origin:'DA',sql:'SELECT * FROM remit_dest_interno',parameters:[]}),/SELECT_DENIED/);
 assert.throws(()=>diagnostic.statement({type:'SELECT',origin:'RA',sql:'SELECT * FROM remit_dest_interno WHERE id=@id',parameters:[]}),/PARAMETERS_INVALID/);
});
test('DOC120-PRE05: cambios de alcance/estado o evidencia detienen antes del host',()=>{
 for(const edit of [r=>r.lote_id='40',r=>r.raiz_id='61',r=>r.intento_id='97',r=>r.intento_estado='EN_CURSO',r=>r.evidencia_doc='{}',r=>r.identidad_expediente_doc='{}']){const copy=structuredClone(rows);edit(copy[0]);assert.throws(()=>diagnostic.payload(copy,scope),/SCOPE_CHANGED/);}
});
test('DOC120-PRE06: error ODBC/budget corta el IPC sin retry y conserva solo códigos cerrados',async()=>{
 let reads=0;await assert.rejects(diagnostic.inspect(data(),async()=>{reads++;throw Error('DOC111_SELECT_CONNECTION_UNAVAILABLE');},exe),/DOC111_SELECT_CONNECTION_UNAVAILABLE/);assert.equal(reads,1);assert.equal(diagnostic.errorCode(Error('DOC111_SELECT_CONNECTION_UNAVAILABLE')),'DOC120_PREREQUISITES_DB_CONNECTION_UNAVAILABLE');
 assert.equal(diagnostic.errorCode(Error('password=PRIVATE_SQL')),'DOC120_PREREQUISITES_STOPPED');
 let calls=0;await assert.rejects(diagnostic.inspect(data(),async()=>{calls++;throw Error('DOC120_PREREQUISITES_BUDGET_EXHAUSTED');},exe),/BUDGET_EXHAUSTED/);assert.equal(calls,1);
});
test('DOC120-PRE07: ningún resultado concede autoridad, transacción ni replay por protocolo',()=>{
 const safe={type:'RESULT',status:'PASS',stage:'CATALOG',code:'CATALOG_COMPATIBLE',counterfactual:true,sessionAuthorityChecked:false,sessionConnectionCompatibilityChecked:false,startTransactionChecked:false,recoveryAuthorized:false};
 for(const patch of [{recoveryAuthorized:true},{sessionAuthorityChecked:true},{startTransactionChecked:true},{code:'PRIVATE_MESSAGE'},{stage:'PRIVATE_MESSAGE'}])assert.throws(()=>diagnostic.result({...safe,...patch}),/PROTOCOL_INVALID/);
 assert.equal(diagnostic.result({...safe,private:'PRIVATE_MESSAGE'}).private,undefined);
});
test('DOC120-PRE08: el host no compone escritor y el selector conserva tope y autorización antes de conectar',async()=>{
 const host=fs.readFileSync(path.join(root,'tools/e2e/scripts/support/Doc120CreationPrerequisites.cs'),'utf8');assert.doesNotMatch(host,/new (?:AdaptadorExpedienteImportacion|MySqlImportacionCarpetasRepository|ClassGaExpediente)|Registrar_Expediente|\.Crear\(/);assert.equal(diagnostic.MAX_SELECT,30);
 await assert.rejects(diagnostic.main(['--lot','39','--root','60','--attempt','96']),/INPUT_INVALID/);
 assert.match(diagnostic.metadataSql,/l\.lote_id=\? AND r\.raiz_id=\? AND i\.intento_id=\?/);assert.doesNotMatch(diagnostic.metadataSql,/PASW_DBMS|PASSWORD|FOR UPDATE/);
});
test('DOC120-PRE09: nombre de catálogo incompatible se distingue de dato de entrada y catálogo ambiguo',async()=>{
 const output=await diagnostic.inspect(data(),async q=>{const result=catalog(q);if(q.sql.includes('FROM ra_de_fondo_documental'))result[0].nombre="O'Brien";return result;},exe);
 assert.equal(output.status,'STOPPED');assert.equal(output.stage,'LEGACY_CATALOG_VALUES');assert.equal(output.code,'CONFIGURACION_INVALIDA');assert.doesNotMatch(JSON.stringify(output),/O'Brien/);
});
test('DOC120-PRE10: límite consume intentos sin reset y elimina credenciales de cada control SELECT',async()=>{
 const state={select:0,completedSelect:0,maxSelect:2,queries:[]},environments=[];let count=0;
 const read=diagnostic.createReader(state,()=>{},async(sql,parameters,environment)=>{count++;environments.push(environment);return[];},{ra:'private-ra-dsn',da:'private-da-dsn'},{DOC120_E2E_MYSQL_USER:'PRIVATE_USER',DOC120_E2E_MYSQL_PASSWORD:'PRIVATE_PASSWORD'});
 const q={origin:'RA',kind:'CATALOG',tables:['remit_dest_interno'],sql:'SELECT * FROM remit_dest_interno',parameters:[]};await read(q);await read({...q,origin:'DA'});await assert.rejects(read(q),/BUDGET_EXHAUSTED/);
 assert.equal(count,2);assert.equal(state.select,2);assert.equal(state.completedSelect,2);for(const e of environments){assert.equal(e.DOC120_E2E_MYSQL_PASSWORD,undefined);assert.equal(e.DOC120_E2E_MYSQL_USER,undefined);assert.equal(e.DOC120_E2E_ODBC_DSN,undefined);}assert.doesNotMatch(JSON.stringify(state),/PRIVATE_|private-|SELECT/);
});
test('DOC120-PRE11: rechazo ODBC conserva intento fallido sin filas ni SQL privado',async()=>{
 const state={select:0,completedSelect:0,maxSelect:30,queries:[]};let environment;
 const read=diagnostic.createReader(state,()=>{},async(sql,parameters,e)=>{environment=e;throw Error('DOC111_SELECT_OPEN_FAILED');},{ra:'private-dsn'},{DOC120_E2E_MYSQL_PASSWORD:'PRIVATE_PASSWORD'});
 await assert.rejects(read({origin:'RA',kind:'METADATA',sql:diagnostic.metadataSql,parameters:['39','60','96']}),/OPEN_FAILED/);assert.equal(state.select,1);assert.equal(state.completedSelect,0);assert.equal(state.queries[0].completed,false);assert.equal(environment.DOC120_E2E_MYSQL_PASSWORD,undefined);assert.doesNotMatch(JSON.stringify(state),/PRIVATE_|contexto_doc|token_reclamacion/);
});
test('DOC120-PRE12: clasifica el predicado del comando rechazado con validadores reales y sin valores privados',async()=>{
 const cases=[
  ['SNAPSHOT_SHAPE',input=>input.lot.Snapshot.PerfilId='0'],
  ['PROFILE_VALIDATION',input=>input.lot.Snapshot.Configuracion.Esquema=2],
  ['CONFIGURATION_NORMALIZED',input=>input.lot.Snapshot.Configuracion.ModoIntegridad='PARCIAL_PERMITIDO'],
  ['SNAPSHOT_FINGERPRINT',input=>input.lot.Snapshot.Huella='0'.repeat(64)],
  ['ROOT_FIELD_COUNT',input=>input.root.ValoresPlan.pop()],
  ['ROOT_FIELD_SHAPE',input=>input.root.ValoresPlan[0].PropiedadPendiente='PRIVATE_VALUE'],
  ['ROOT_FIELD_DUPLICATE',input=>input.root.ValoresPlan[1].DestinoRef='CODIGO'],
  ['ROOT_FIELD_RULE',input=>input.root.ValoresPlan[0].FuenteTipo='CONSTANTE'],
  ['ROOT_FIELD_RULE',input=>input.root.ValoresPlan[0].TipoValor='ENTERO'],
  ['ROOT_FIELD_RULE',input=>input.root.ValoresPlan.find(v=>v.DestinoRef==='NOMBRE').Valor=null],
  ['RESPONSIBLE_REFERENCE',input=>input.root.ValoresPlan.find(v=>v.DestinoRef==='RESPONSABLE').Valor='0008'],
  ['DATE_RANGE',input=>input.root.ValoresPlan.find(v=>v.DestinoRef==='FECHA_FINAL').Valor='2020-01-01']
 ];
 for(const [guard,edit] of cases){const input=data();edit(input);let reads=0;const output=await diagnostic.inspect(input,async()=>{reads++;return[];},exe);assert.equal(output.stage,'COMMAND');assert.equal(output.code,'CONFIGURACION_INVALIDA');assert.equal(output.detail.guard,guard);assert.equal(reads,0);assert.doesNotMatch(JSON.stringify(output),/PRIVATE_VALUE|synthetic-private|0008|2020-01-01/);}
});
test('DOC120-PRE13: modo de comando termina antes de consultar catálogos aun cuando el comando sea compatible',async()=>{
 const input=data();input.commandOnly=true;let reads=0;const output=await diagnostic.inspect(input,async()=>{reads++;return[];},exe);assert.equal(output.status,'PASS');assert.equal(output.code,'COMMAND_COMPATIBLE');assert.equal(reads,0);assert.equal(output.recoveryAuthorized,false);
});
test('DOC120-PRE14: detalle contiene solo categorías/campos públicos y conteos válidos',()=>{
 const safe={type:'RESULT',status:'STOPPED',stage:'COMMAND',code:'CONFIGURACION_INVALIDA',counterfactual:true,sessionAuthorityChecked:false,sessionConnectionCompatibilityChecked:false,startTransactionChecked:false,recoveryAuthorized:false,detail:{guard:'ROOT_FIELD_RULE',field:'PRIVATE_FIELD',issue:'PRIVATE_CODE',message:'PRIVATE_MESSAGE',value:'PRIVATE_VALUE',sourceMatches:false,typeMatches:true,valueValid:false,valueLength:15,incidenceCount:Infinity}};
 const output=diagnostic.result(safe);assert.equal(output.detail.field,'OTHER');assert.equal(output.detail.issue,'OTHER');assert.equal(output.detail.valueLength,15);assert.equal(output.detail.incidenceCount,undefined);assert.doesNotMatch(JSON.stringify(output),/PRIVATE_/);
 assert.throws(()=>diagnostic.result({...safe,detail:{guard:'PRIVATE_GUARD'}}),/PROTOCOL_INVALID/);
});
test('DOC120-PRE15: DLL compilada acepta fechas canónicas de origen compacto sin alterar el snapshot',async()=>{
 const compactRows=JSON.parse(cp.execFileSync(fixtureExe,[bin,'SHORT_DATE'],{encoding:'utf8',windowsHide:true}));
 const input=diagnostic.payload(compactRows,scope);input.commandOnly=true;const before=JSON.stringify(input);let reads=0;
 const output=await diagnostic.inspect(input,async()=>{reads++;return[];},exe);assert.equal(output.status,'PASS',JSON.stringify(output));assert.equal(output.code,'COMMAND_COMPATIBLE');assert.equal(reads,0);assert.equal(JSON.stringify(input),before);assert.equal(input.lot.Snapshot.Reglas.find(r=>r.DestinoRef==='FECHA_INICIAL').LongitudMaxima,8);
});
