'use strict';
const {test}=require('node:test');
const assert=require('node:assert/strict');
const {verifyConsumerHistory}=require('./support/doc110-provider-history.cjs');
function fixture(fault){
  const provider={ProveedorId:'1',VersionId:'2',Activo:false};
  const documents=['3','4'].map(VersionId=>({PerfilId:'5',Version:{VersionId,Estado:'PUBLICADA',Reglas:[{FuenteTipo:'PROVEEDOR',Proveedor:{ProveedorId:'1',VersionId:'2'}}]}}));
  const snapshot={documents,history:documents.map(d=>d.Version)};
  const calls=[];
  const call=async(method,payload)=>{
    calls.push(method);let datos;
    if(method==='ObtenerPerfil')datos=documents.find(d=>d.Version.VersionId===payload.versionId);
    else if(method==='ConsultarHistorial')datos=snapshot.history;
    else if(method==='ConsultarProveedores')datos=fault==='catalog'? [provider]:[];
    else throw new Error('Unexpected mutation');
    if(method==='ObtenerPerfil' && fault==='reference')datos={...datos,Version:{...datos.Version,Reglas:[]}};
    if(method==='ConsultarHistorial' && fault==='history')datos=[];
    return {datos};
  };
  return {provider,snapshot,call,calls};
}
test('conserva ambas publicaciones y excluye proveedor inactivo usando solo lecturas',async()=>{
  const f=fixture();await verifyConsumerHistory(f.call,f.provider,f.snapshot);
  assert.deepEqual(f.calls,['ObtenerPerfil','ObtenerPerfil','ConsultarHistorial','ConsultarProveedores']);
});
test('detecta perdida de referencia, historial alterado y proveedor inactivo seleccionable',async()=>{
  for(const fault of ['reference','history','catalog']){
    const f=fixture(fault);await assert.rejects(verifyConsumerHistory(f.call,f.provider,f.snapshot));
  }
});
