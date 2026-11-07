'use strict';
const {test}=require('node:test'),assert=require('node:assert/strict');
const {discoverStoredProvider}=require('./support/doc110-provider-source-diagnostics.cjs');
test('diagnostico busca solo fixture publicado inactivo, con lectura acotada y tabla exacta',async()=>{
  const calls=[];
  const call=async(method,payload)=>{
    calls.push(method);
    if(method==='ListarAdministracionProveedores')return {datos:[
      {Codigo:'OPERATIVO',Estado:'PUBLICADA',Activo:false},
      {Codigo:'E2E-PROV-aa',Estado:'BORRADOR',Activo:false},
      {Codigo:'E2E-PROV-ab',Estado:'PUBLICADA',Activo:true},
      {Codigo:'E2E-PROV-ac',Estado:'PUBLICADA',Activo:false,ProveedorId:'1',VersionId:'2'},
      {Codigo:'E2E-PROV-ad',Estado:'PUBLICADA',Activo:false,ProveedorId:'3',VersionId:'4'}]};
    assert.equal(method,'ObtenerAdministracionProveedor');
    assert.deepEqual(Object.keys(payload).sort(),['ProveedorId','VersionId']);
    return {datos:{Estado:'PUBLICADA',Activo:false,ProveedorId:payload.ProveedorId,Contrato:{ConexionRef:'DA',Tabla:payload.ProveedorId==='1'?'imp_02_egresos':'imp_02_creditos'}}};
  };
  assert.equal((await discoverStoredProvider(call,'imp_02_creditos')).ProveedorId,'3');
  assert.deepEqual(calls,['ListarAdministracionProveedores','ObtenerAdministracionProveedor','ObtenerAdministracionProveedor']);
});
test('diagnostico sin fixture o con listado invalido no inventa proveedor ni escribe',async()=>{
  await assert.rejects(discoverStoredProvider(async()=>({datos:[]}),'imp_02_creditos'),/FIXTURE_NOT_FOUND/);
  await assert.rejects(discoverStoredProvider(async()=>({datos:null}),'imp_02_creditos'),/LIST_INVALID/);
});
