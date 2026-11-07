'use strict';
const {expect}=require('@playwright/test');

// Solo se invoca con el proveedor descartable creado por el recorrido UI actual.
async function verifyProviderRevalidation(call,profile,provider){
  expect(profile.Version.Estado).toBe('BORRADOR');
  expect(provider.Estado).toBe('PUBLICADA');expect(provider.Activo).toBe(true);
  expect(profile.Version.Reglas.some(r=>r.FuenteTipo==='PROVEEDOR' &&
    r.Proveedor?.ProveedorId===provider.ProveedorId && r.Proveedor?.VersionId===provider.VersionId)).toBe(true);
  const refs={ProveedorId:provider.ProveedorId,VersionId:provider.VersionId};
  const same=(a,b)=>expect(JSON.stringify(a)===JSON.stringify(b)).toBe(true);
  // Crear el borrador de proveedor también incrementa su cabecera; usar la revisión recién leída.
  const initial=(await call('ObtenerAdministracionProveedor',refs)).datos;
  expect(initial.Activo).toBe(true);expect(initial.Estado).toBe('PUBLICADA');
  const history=(await call('ConsultarHistorial',{perfilId:profile.PerfilId,pagina:1,tamanoPagina:25})).datos;
  try{
    const disabled=(await call('CambiarDisponibilidadProveedor',{...refs,Revision:initial.Revision,RevisionProveedor:initial.RevisionProveedor,Activo:false})).datos;
    expect(disabled.Activo).toBe(false);
    expect(disabled.RevisionProveedor).toBe(initial.RevisionProveedor+1);
    const rejected=await call('PublicarVersion',{perfilId:profile.PerfilId,versionId:profile.Version.VersionId,
      revisionPerfilEsperada:profile.Revision,revisionVersionEsperada:profile.Version.Revision},403);
    expect(rejected.codigo).toBe('ACCESO_DENEGADO');expect(rejected.datos==null).toBe(true);
    same((await call('ObtenerPerfil',{perfilId:profile.PerfilId,versionId:profile.Version.VersionId})).datos,profile);
    same((await call('ConsultarHistorial',{perfilId:profile.PerfilId,pagina:1,tamanoPagina:25})).datos,history);
    console.log('DOC110_PASS: DISABLED_PROVIDER_REVALIDATED_BEFORE_PUBLICATION_PROFILE_AND_HISTORY_UNCHANGED');
  }finally{
    // Leer la revisión actual incluso si se perdió la respuesta de desactivación; no repetir esa escritura.
    let current=(await call('ObtenerAdministracionProveedor',refs)).datos;
    if(!current.Activo)
      current=(await call('CambiarDisponibilidadProveedor',{...refs,Revision:current.Revision,RevisionProveedor:current.RevisionProveedor,Activo:true})).datos;
    expect(current.Activo).toBe(true);expect(current.Estado).toBe('PUBLICADA');
    expect(current.Revision).toBe(provider.Revision);
    same(current.Contrato,provider.Contrato);
    console.log('DOC110_PASS: DISPOSABLE_PROVIDER_RESTORED_FOR_PUBLICATION_LIFECYCLE');
  }
}
module.exports={verifyProviderRevalidation};
