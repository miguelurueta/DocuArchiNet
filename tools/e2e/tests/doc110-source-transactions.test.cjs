'use strict';
const {test}=require('node:test');
const assert=require('node:assert/strict');
const {verifySourceTransactions,prepareSourceTransactionFixture}=require('./support/doc110-source-transactions.cjs');
const copy=x=>JSON.parse(JSON.stringify(x));
function fixture(fault){
  const initial={FuenteId:'9',Revision:1,Activo:false,ConexionRef:'DA',Tabla:'imp_02_fixture_nuevo',ModoAislamiento:'CONEXION',ColumnaEmpresa:'',Columnas:['A','B']};
  let source=copy(initial),history=[{Revision:1,Accion:'ALTA',Estado:copy(initial)}],duplicates=0,edits=0;
  const peer=async(method,data)=>{
    const ok=datos=>({status:200,result:{ok:true,datos:copy(datos)}});
    const conflict=()=>({status:409,result:{ok:false,codigo:'CONFLICTO_REVISION',datos:null}});
    if(method==='ObtenerFuente')return ok(source);
    if(method==='HistorialFuente')return ok(history);
    if(method==='ListarFuentes')return ok(duplicates?[source,copy(source)]:[source]);
    if(method==='CrearFuente'){
      if(fault==='duplicate-insert')duplicates++;
      return conflict();
    }
    assert.equal(method,'GuardarFuente');edits++;
    if(data.Cambio.Documento.Revision!==source.Revision){
      if(fault==='loser-partial-columns')source.Columnas=copy(data.Cambio.Documento.Columnas);
      if(fault==='double-success')return ok(source);
      return conflict();
    }
    source={...source,Revision:2,Columnas:copy(data.Cambio.Documento.Columnas)};
    history.push({Revision:2,Accion:'EDITAR',Estado:copy(source)});
    if(fault==='extra-audit')history.push({Revision:3,Accion:'EDITAR',Estado:copy(source)});
    if(fault==='transport-after-commit')throw new Error('synthetic transport failure');
    return ok(source);
  };
  return {initial,peers:[peer,peer],edits:()=>edits};
}
test('duplicado no agrega fuente; una sola edicion conserva columnas e historial',async()=>{
  const f=fixture();const result=await verifySourceTransactions(f.peers,f.initial);
  assert.equal(result.source.Activo,false);assert.equal(result.checks,2);assert.equal(f.edits(),3);
});
test('valida tambien ganador de segunda sesion',async()=>{
  const f=fixture();const delayed=async(...args)=>{await new Promise(r=>setImmediate(r));return f.peers[0](...args);};
  const result=await verifySourceTransactions([delayed,f.peers[1]],f.initial);
  assert.deepEqual(result.source.Columnas,['B']);
});
test('detecta duplicado insertado, doble exito, columnas parciales e historial extra',async()=>{
  for(const fault of ['duplicate-insert','double-success','loser-partial-columns','extra-audit']){
    const f=fixture(fault);await assert.rejects(verifySourceTransactions(f.peers,f.initial));
  }
});
test('transporte incierto no reintenta automaticamente',async()=>{
  const f=fixture('transport-after-commit');
  await assert.rejects(verifySourceTransactions(f.peers,f.initial),/OUTCOME_UNCONFIRMED/);
  assert.equal(f.edits(),2);
});
test('no admite fuentes anteriores, activas ni revisiones reutilizadas',async()=>{
  for(const patch of [{Tabla:'imp_02_creditos'},{Tabla:'imp_02_egresos'},{Activo:true},{Revision:2}]){
    let calls=0;const f=fixture();
    await assert.rejects(verifySourceTransactions([()=>{calls++;},()=>{calls++;}],{...f.initial,...patch}),/NEW_INACTIVE_FIXTURE_REQUIRED/);
    assert.equal(calls,0);
  }
});

test('facturas exige ausencia incluyendo inactivas y metadatos antes del alta',async()=>{
  for(const active of [false,true]){
    let creates=0;
    const call=async()=>({status:200,result:{ok:true,datos:[{ConexionRef:'DA',Tabla:'imp_02_facturas',Activo:active}]}});
    await assert.rejects(prepareSourceTransactionFixture(call,()=>{creates++;}),/ALREADY_REGISTERED/);
    assert.equal(creates,0);
  }
  for(const columns of [[],[{value:'NIT',Tipo:'TEXTO'},{value:'NUMERO',Tipo:'NUMERO'}]]){
    let creates=0;
    const call=async method=>({status:200,result:{ok:true,datos:method==='ListarFuentes'?[]:columns}});
    await assert.rejects(prepareSourceTransactionFixture(call,()=>{creates++;}));
    assert.equal(creates,0);
  }
});
test('preflight acotado no crea si no se puede demostrar ausencia',async()=>{
  let creates=0,calls=0;
  const call=async()=>{calls++;return {status:200,result:{ok:true,datos:Array.from({length:25},()=>({ConexionRef:'RA',Tabla:'otra'}))}};};
  await assert.rejects(prepareSourceTransactionFixture(call,()=>{creates++;}),/PAGE_LIMIT/);
  assert.equal(calls,100);assert.equal(creates,0);
});
test('facturas crea exactamente una autorizacion inactiva con NIT y NUMERO',async()=>{
  let creates=0;const operations=[];
  const call=async(method,data)=>{
    operations.push(method);assert.equal(data.tabla||data.Termino,'imp_02_facturas');
    return {status:200,result:{ok:true,datos:method==='ListarFuentes'?[]:
      ['NUMERO','NIT'].map(value=>({value,Tipo:'TEXTO'}))}};
  };
  const current={...fixture().initial,Tabla:'imp_02_facturas',Columnas:['NUMERO','NIT']};
  const result=await prepareSourceTransactionFixture(call,async()=>{creates++;return current;});
  assert.equal(creates,1);assert.equal(result.Activo,false);
  assert.deepEqual(operations,['ListarFuentes','ConsultarMetadatosFuente']);
});
