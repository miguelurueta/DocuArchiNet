'use strict';
const {expect}=require('@playwright/test');
const {safePost}=require('./doc110-safe-request.cjs');

// Escenario del ambiente sin excepciones al prefijo imp_02. No ejecuta consultas del proveedor.
async function exerciseProviderScope(auth,baseUrl,token,fixture=null){
  async function call(method,payload,expected=200){
    const response=await safePost(auth.context.request,new URL('webservice/WebServiceImportacionCarpetas.asmx/'+method,baseUrl).href,{
      headers:{'X-Importacion-Perfil-CSRF':token},data:{solicitud:payload}
    },method);
    expect(response.status()).toBe(expected);
    let result;try{result=(await response.json()).d;}catch{throw new Error('DOC110_PROVIDER_SCOPE_NON_JSON');}
    expect(result?.ok).toBe(expected===200);
    if(expected===403){expect(result.codigo).toBe('ACCESO_DENEGADO');expect(result.datos==null).toBe(true);}
    return result.datos;
  }
  let finished=false,checked=0,fixtureChecked=false;
  const seen=new Set();
  for(let page=1;page<=100;page++){
    const items=await call('ListarAdministracionProveedores',{Pagina:page});
    expect(Array.isArray(items)).toBe(true);expect(items.length).toBeLessThanOrEqual(25);
    for(const item of items){
      expect(seen.has(item.VersionId)).toBe(false);seen.add(item.VersionId);
      const contract=item.Contrato;
      if(!contract || !['RA','DA'].includes(contract.ConexionRef) || typeof contract.Tabla!=='string' ||
        !/^[A-Za-z_][A-Za-z0-9_]{0,63}$/.test(contract.Tabla) || contract.Tabla.startsWith('imp_02'))continue;
      const ref={ProveedorId:item.ProveedorId,VersionId:item.VersionId};
      const before=await call('ObtenerAdministracionProveedor',ref);
      const isFixture=fixture && ref.ProveedorId===fixture.providerId && ref.VersionId===fixture.versionId;
      if(isFixture){
        expect(before.Codigo===fixture.code && before.Activo===false && before.Estado==='BORRADOR' &&
          before.Contrato.ConexionRef===fixture.connectionRef && before.Contrato.Tabla===fixture.table).toBe(true);
        fixtureChecked=true;
      }
      await call('ValidarProveedor',{...ref,Revision:before.Revision,RevisionProveedor:before.RevisionProveedor},403);
      const after=await call('ObtenerAdministracionProveedor',ref);
      // No emitir nombres, contratos ni referencias persistidas al fallar.
      expect(JSON.stringify(before)===JSON.stringify(after)).toBe(true);
      checked++;
    }
    if(items.length<25){finished=true;break;}
  }
  expect(finished).toBe(true);
  if(fixture)expect(fixtureChecked).toBe(true);
  if(!checked){
    console.log('DOC110_NOT_RUN: PREEXISTING_PROVIDER_OUTSIDE_SCOPE_NOT_FOUND');
    return {status:'NOT_RUN',checked:0};
  }
  console.log('DOC110_PASS: PREEXISTING_PROVIDER_OUTSIDE_SCOPE_VALIDATION_DENIED');
  console.log('DOC110_PASS: PREEXISTING_PROVIDER_UNCHANGED');
  if(fixture)console.log('DOC110_PASS: SYNTHETIC_LEGACY_PROVIDER_DENIED_REMAINS_INACTIVE');
  return {status:'PASS',checked};
}
module.exports={exerciseProviderScope};
