'use strict';
const {test}=require('node:test');
const assert=require('node:assert/strict');
const {runSourceProfile,exerciseSourceProfile,validResume,cleanupTestSource}=require('./support/doc110-source-profile.cjs');
const {sourceFixture}=require('./support/doc110-source-fixture.cjs');
const copy=x=>structuredClone(x);
function fixture(fault,spec=sourceFixture()){
  let source=null;const history=[],calls=[];
  const provider={ProveedorId:'2',VersionId:'3',Revision:3,RevisionProveedor:7,Estado:'PUBLICADA',Activo:false};
  const documents=['5','6'].map(VersionId=>({PerfilId:'4',Version:{VersionId,Estado:'PUBLICADA'}}));
  const consumers={provider,profileSnapshot:{documents,history:documents.map(d=>d.Version)}};
  function mutate(action){
    calls.push(action);
    if(action==='create')source={FuenteId:'1',ConexionRef:'DA',Tabla:spec.table,Activo:false,Revision:1,ModoAislamiento:'CONEXION',ColumnaEmpresa:'',Columnas:['CONSECUTIVO',spec.removedColumn]};
    else{source={...source,Revision:source.Revision+1};if(action==='edit')source.Columnas=['CONSECUTIVO'];else source.Activo=action==='activate';}
    history.unshift({Revision:source.Revision,Accion:{create:'ALTA',activate:'ACTIVAR',edit:'EDITAR',revoke:'REVOCAR'}[action],Estado:copy(source),ActorRef:'1',FechaUtc:'2026-10-26T00:00:00Z',Motivo:{create:'Autorizar fuente descartable DOC-110',activate:'Activar fuente descartable DOC-110',edit:'Limitar fuente descartable a CONSECUTIVO',revoke:'Revocar fuente descartable DOC-110'}[action]});
    return copy(source);
  }
  const ui=Object.fromEntries(['create','activate','edit','revoke'].map(action=>[action,async()=>{
    const result=mutate(action);
    if(fault==='activation-lost'&&action==='activate')throw new Error('response lost');return result;
  }]));
  const call=async(method,payload,status=200)=>{
    calls.push(method);
    if(method==='ListarFuentes'){
      if(fault==='limit')return {datos:Array.from({length:25},()=>({ConexionRef:'RA',Tabla:'imp_02_otro'}))};
      assert.equal(payload.Termino,spec.table);
      if(fault==='existing-active'||fault==='existing-inactive')return {datos:[{ConexionRef:'DA',Tabla:spec.table,Activo:fault==='existing-active'}]};
      if(fault==='existing-second-page')return {datos:payload.Pagina===1?Array.from({length:25},()=>({ConexionRef:'RA',Tabla:'imp_02_otro'})):[{ConexionRef:'DA',Tabla:spec.table,Activo:false}]};
      return {datos:source?[copy(source)]:[]};
    }
    if(method==='ConsultarMetadatosFuente'){
      assert.equal(payload.tabla,spec.table);
      return {datos:[{value:'CONSECUTIVO',Tipo:fault==='metadata'?'ENTERO':'TEXTO'},{value:spec.removedColumn,Tipo:spec.removedType}]};
    }
    if(method==='ConsultarMetadatosProveedor'){
      assert.equal(payload.tabla,spec.table);
      return {datos:(fault==='column-visible'?['CONSECUTIVO',spec.removedColumn]:['CONSECUTIVO']).map(value=>({value}))};
    }
    if(method==='ObtenerFuente')return {datos:copy(source)};
    if(method==='RevocarFuente'){
      assert.equal(payload.Cambio.Documento.FuenteId,source.FuenteId);assert.equal(payload.Cambio.Documento.Revision,source.Revision);
      return {datos:mutate('revoke')};
    }
    if(method==='ObtenerAdministracionProveedor'){
      assert.deepEqual(Object.keys(payload).sort(),['ProveedorId','Revision','RevisionProveedor','VersionId']);
      return {datos:fault==='provider-mutated'&&!source.Activo?{...provider,Revision:99}:copy(provider)};
    }
    if(method==='ValidarProveedor'){
      assert.equal(status,403);assert.equal(source.Activo,false);
      return {codigo:fault==='wrong-code'?'SOLICITUD_NO_VERIFICADA':'ACCESO_DENEGADO',datos:null};
    }
    if(method==='ObtenerPerfil')return {datos:copy(documents.find(d=>d.Version.VersionId===payload.versionId))};
    if(method==='ConsultarHistorial')return {datos:copy(consumers.profileSnapshot.history)};
    if(method==='HistorialFuente')return {datos:fault==='audit'?[]:copy(history)};
    throw new Error('unexpected request');
  };
  const lifecycle=async()=>{calls.push('providerLifecycle');if(fault==='consumer-failed')throw new Error('consumer failed');return copy(consumers);};
  return {call,ui,lifecycle,calls,source:()=>source,history,seed(){mutate('create');calls.length=0;}};
}

const previous={status:'FAIL',code:'CHECK_FAILED',sourceTable:'imp_02_creditos',startedUtc:'2026-10-25T23:59:59Z',finishedUtc:'2026-10-26T00:00:10Z'};
const recovery={status:'PASS',sourceTable:'imp_02_creditos',startedUtc:'2026-10-26T00:01:00Z',finishedUtc:'2026-10-26T00:01:01Z'};
async function recoveredFixture(fault){
  const spec=sourceFixture('imp_02_creditos'),f=fixture(fault,spec);f.seed();
  await f.ui.activate();await f.ui.edit();await f.ui.revoke();f.calls.length=0;
  return {...f,spec};
}
test('continua fuente recuperada sin repetir alta o edicion y conserva cuatro revisiones previas',async()=>{
  const f=await recoveredFixture(),before=copy(f.history);
  await runSourceProfile(f.call,f.ui,f.lifecycle,f.spec,{resume:{...previous,sourceId:'1'},recovered:recovery});
  assert.equal(f.source().Activo,false);assert.equal(f.source().Revision,6);
  assert.equal(f.calls.includes('create'),false);assert.equal(f.calls.includes('edit'),false);
  assert.deepEqual(f.history.slice(2),before);
  assert.deepEqual(f.calls.filter(c=>['activate','revoke','providerLifecycle'].includes(c)),['activate','providerLifecycle','revoke']);
});
test('continuacion exige recuperacion PASS posterior al fallo, misma tabla e historial exacto antes de escribir',async()=>{
  for(const invalid of [{...recovery,status:'FAIL'},{...recovery,sourceTable:'imp_02_egresos'},
    {...recovery,startedUtc:'2026-10-25T00:00:00Z',finishedUtc:'2026-10-25T00:00:01Z'},
    {...recovery,finishedUtc:'invalid'}]){
    const f=await recoveredFixture();
    await assert.rejects(runSourceProfile(f.call,f.ui,f.lifecycle,f.spec,{resume:{...previous,sourceId:'1'},recovered:invalid}),/RESUME_NOT_VERIFIED/);
    assert.equal(f.calls.some(c=>['create','edit','activate','revoke','RevocarFuente'].includes(c)),false);
  }
  const f=await recoveredFixture();f.history[0].ActorRef='otra';
  await assert.rejects(runSourceProfile(f.call,f.ui,f.lifecycle,f.spec,{resume:{...previous,sourceId:'1'},recovered:recovery}),/RESUME_NOT_VERIFIED/);
  assert.equal(f.calls.includes('activate'),false);
});
test('fallo de consumidor tras continuar recupera fuente y permite verificar limpieza sin repetir revocacion',async()=>{
  const f=await recoveredFixture('consumer-failed');
  await assert.rejects(runSourceProfile(f.call,f.ui,f.lifecycle,f.spec,{resume:{...previous,sourceId:'1'},recovered:recovery}),/consumer failed/);
  assert.equal(f.source().Activo,false);assert.equal(f.source().Revision,6);
  f.calls.length=0;
  await cleanupTestSource(f.call,f.spec,previous,'1');assert.equal(f.calls.includes('RevocarFuente'),false);
});
test('recuperacion verifica secuencia y revoca solo fuente propia activa sin reactivar consumidores',async()=>{
  const spec=sourceFixture('imp_02_creditos'),f=fixture(undefined,spec);f.seed();await f.ui.activate();await f.ui.edit();f.calls.length=0;
  await cleanupTestSource(f.call,spec,previous,'1');
  assert.equal(f.source().Activo,false);assert.equal(f.source().Revision,4);
  assert.equal(f.calls.filter(c=>c==='RevocarFuente').length,1);assert.equal(f.calls.includes('providerLifecycle'),false);
  f.calls.length=0;await cleanupTestSource(f.call,spec,previous,'1');
  assert.equal(f.calls.includes('RevocarFuente'),false,'estado ya inactivo no produce otra escritura');
});
test('recuperacion rechaza auditoria alterada, otro actor o identidad sin escribir',async()=>{
  const spec=sourceFixture('imp_02_creditos');
  for(const change of [f=>{f.history[0].ActorRef='99';},f=>{f.history[0].Motivo='Cambio externo';},
    f=>{f.source().Columnas=['otra'];},f=>{f.history.at(-1).FechaUtc='2026-10-20T00:00:00Z';}]){
    const f=fixture(undefined,spec);f.seed();await f.ui.activate();await f.ui.edit();f.calls.length=0;change(f);
    await assert.rejects(cleanupTestSource(f.call,spec,previous,'1'),/CLEANUP_NOT_VERIFIED/);
    assert.equal(f.calls.includes('RevocarFuente'),false);
  }
});
test('reanuda solo alta inactiva de corrida fallida verificada, sin repetir creacion',async()=>{
  const spec=sourceFixture('imp_02_creditos'),f=fixture(undefined,spec);f.seed();let ready;
  await runSourceProfile(f.call,f.ui,f.lifecycle,spec,{resume:previous,onSourceReady:id=>{ready=id;}});
  assert.equal(ready,'1');assert.equal(f.calls.includes('create'),false);assert.equal(f.source().Activo,false);
  assert.equal(f.source().Revision,4);
});
test('reanudacion rechaza fuente modificada, auditoria ajena, otra ventana o falta de fuente sin escribir',async()=>{
  const spec=sourceFixture('imp_02_creditos');
  for(const change of [f=>{f.source().Activo=true;},f=>{f.source().Revision=2;},f=>{f.history[0].Motivo='Otro motivo';},
    f=>{f.history[0].FechaUtc='2026-10-24T00:00:00Z';},f=>{f.history[0].Estado.Columnas=[];}]){
    const f=fixture(undefined,spec);f.seed();change(f);
    await assert.rejects(runSourceProfile(f.call,f.ui,f.lifecycle,spec,{resume:previous}),/RESUME_NOT_VERIFIED/);
    assert.equal(f.calls.some(c=>['create','activate','edit','revoke','RevocarFuente'].includes(c)),false);
  }
  const empty=fixture(undefined,spec);
  await assert.rejects(runSourceProfile(empty.call,empty.ui,empty.lifecycle,spec,{resume:previous}),/RESUME_NOT_FOUND/);
  assert.equal(empty.source(),null);
});
test('ventana UTC de reanudacion admite fecha ASMX y exige reporte fallido coincidente',()=>{
  const spec=sourceFixture('imp_02_creditos'),f=fixture(undefined,spec);f.seed();
  f.history[0].FechaUtc='/Date('+Date.parse('2026-10-26T00:00:00Z')+')/';
  assert.equal(validResume(f.source(),f.history,spec,previous),true);
  for(const change of [{status:'PASS'},{sourceTable:'imp_02_egresos'},{sourceId:'99'},{finishedUtc:'invalid'},{code:'OTHER'}])
    assert.equal(validResume(f.source(),f.history,spec,{...previous,...change}),false);
});
test('fuente UI nueva alimenta consumidores, revocacion conserva documentos e historial',async()=>{
  const f=fixture();await runSourceProfile(f.call,f.ui,f.lifecycle);
  assert.equal(f.source().Activo,false);assert.equal(f.source().Revision,4);
  assert.deepEqual(f.calls.filter(c=>['create','activate','edit','revoke','providerLifecycle'].includes(c)),['create','activate','edit','providerLifecycle','revoke']);
  assert.equal(f.calls.includes('RevocarFuente'),false,'cleanup no repite revocacion confirmada');
});
test('creditos usa FECHADOCUMENTO FECHA sin confundirla con FECHA TEXTO de egresos',async()=>{
  const spec=sourceFixture('imp_02_creditos'),f=fixture(undefined,spec);
  assert.equal(spec.removedType,'FECHA');assert.equal(spec.ambiguousKey,null);
  await runSourceProfile(f.call,f.ui,f.lifecycle,spec);
  assert.equal(f.source().Tabla,'imp_02_creditos');assert.equal(f.source().Activo,false);
  const existing=fixture('existing-inactive',spec);
  await assert.rejects(runSourceProfile(existing.call,existing.ui,existing.lifecycle,spec),/SOURCE_ALREADY_REGISTERED/);
  assert.equal(existing.source(),null);
  for(const invalid of ['usuarios','imp_02_otro','imp_02_creditos;DROP',''])assert.throws(()=>sourceFixture(invalid),/FIXTURE_UNSUPPORTED/);
});
test('identidad existente activa o inactiva, paginacion incompleta y metadatos incompatibles no escriben',async()=>{
  for(const fault of ['existing-active','existing-inactive','existing-second-page','limit','metadata']){
    const f=fixture(fault);await assert.rejects(runSourceProfile(f.call,f.ui,f.lifecycle));
    assert.equal(f.source(),null);assert.equal(f.calls.includes('providerLifecycle'),false);
  }
});
test('error tras activar, editar o consumir reconcilia y revoca solo fuente creada',async()=>{
  for(const fault of ['activation-lost','consumer-failed','column-visible']){
    const f=fixture(fault);await assert.rejects(runSourceProfile(f.call,f.ui,f.lifecycle));
    assert.equal(f.source().Activo,false);assert.equal(f.calls.filter(c=>c==='RevocarFuente').length,1);
  }
});
test('no acredita rechazo por CSRF, version reescrita ni auditoria ausente',async()=>{
  for(const fault of ['wrong-code','provider-mutated','audit']){
    const f=fixture(fault);await assert.rejects(runSourceProfile(f.call,f.ui,f.lifecycle));assert.equal(f.source().Activo,false);
  }
});
test('entrada autenticada exige autorizacion adicional de mutaciones de fuentes antes de acceder',async()=>{
  const previous=process.env.DOC110_E2E_SOURCE_CATALOG_AUTHORIZED;
  try{delete process.env.DOC110_E2E_SOURCE_CATALOG_AUTHORIZED;await assert.rejects(exerciseSourceProfile(null,null,null),/SOURCE_AUTHORIZATION_REQUIRED/);}
  finally{if(previous===undefined)delete process.env.DOC110_E2E_SOURCE_CATALOG_AUTHORIZED;else process.env.DOC110_E2E_SOURCE_CATALOG_AUTHORIZED=previous;}
});
