'use strict';
const {expect}=require('@playwright/test');

async function verifyConsumerHistory(call,provider,snapshot){
  expect(provider.Activo).toBe(false);
  expect(snapshot?.documents?.length).toBe(2);
  for(const before of snapshot.documents){
    expect(before.Version.Estado).toBe('PUBLICADA');
    expect(before.Version.Reglas.some(r=>r.FuenteTipo==='PROVEEDOR' &&
      r.Proveedor?.ProveedorId===provider.ProveedorId && r.Proveedor?.VersionId===provider.VersionId)).toBe(true);
    const after=(await call('ObtenerPerfil',{perfilId:before.PerfilId,versionId:before.Version.VersionId})).datos;
    // Comparacion booleana: no incluir claves de negocio ni reglas en errores.
    expect(JSON.stringify(after)===JSON.stringify(before)).toBe(true);
  }
  const history=(await call('ConsultarHistorial',{perfilId:snapshot.documents[0].PerfilId,pagina:1,tamanoPagina:25})).datos;
  expect(JSON.stringify(history)===JSON.stringify(snapshot.history)).toBe(true);
  console.log('DOC110_PASS: DISABLED_PROVIDER_PUBLISHED_PROFILE_REFERENCES_AND_HISTORY_UNCHANGED');
  const options=(await call('ConsultarProveedores',{pagina:1,tamanoPagina:25,proveedorId:provider.ProveedorId,versionId:provider.VersionId})).datos;
  expect(Array.isArray(options)).toBe(true);expect(options.length).toBe(0);
  console.log('DOC110_PASS: DISABLED_PROVIDER_EXCLUDED_FROM_CONSUMER_CATALOG');
}
module.exports={verifyConsumerHistory};
