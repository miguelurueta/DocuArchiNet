'use strict';
const {test}=require('node:test');
const assert=require('node:assert/strict');
const {verifyDraftConsumer}=require('./support/doc110-provider-draft-consumer.cjs');
function fixture(fault){
  const draft={ProveedorId:'1',VersionId:'3',Activo:true,Estado:'BORRADOR'};
  const profile={PerfilId:'4',Version:{VersionId:'5',Revision:1,Configuracion:{},Reglas:[{FuenteTipo:'PROVEEDOR',Proveedor:{ProveedorId:'1',VersionId:'2'}}]}};
  const calls=[];
  const call=async(method,payload,status=200)=>{
    calls.push(method);
    if(method==='ConsultarProveedores')return {datos:fault==='catalog'?[draft]:[]};
    if(method==='ObtenerPerfil')return {datos:fault==='mutation'?{...profile,Revision:99}:profile};
    if(method==='ObtenerAdministracionProveedor')return {datos:draft};
    assert.equal(status,403);assert.equal(payload.reglas[0].Proveedor.VersionId,'3');
    return {codigo:fault==='wrong-code'?'SOLICITUD_NO_VERIFICADA':'ACCESO_DENEGADO',datos:null};
  };
  return {call,profile,draft,calls};
}
test('borrador activo no es seleccionable ni aceptado por guardar o ensayar',async()=>{
  const f=fixture();await verifyDraftConsumer(f.call,f.profile,f.draft);
  assert.deepEqual(f.calls,['ConsultarProveedores','GuardarBorrador','ObtenerPerfil','EnsayarReglas','ObtenerPerfil','ObtenerAdministracionProveedor']);
  assert.equal(f.profile.Version.Reglas[0].Proveedor.VersionId,'2');
});
test('no acredita borrador visible, rechazo por otro motivo ni perfil alterado',async()=>{
  for(const fault of ['catalog','wrong-code','mutation']){
    const f=fixture(fault);await assert.rejects(verifyDraftConsumer(f.call,f.profile,f.draft));
  }
});
