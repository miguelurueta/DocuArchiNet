'use strict';
const {expect}=require('@playwright/test');
const crypto=require('node:crypto');
const {safePost}=require('./doc110-safe-request.cjs');
const {sourceFixture}=require('./doc110-source-fixture.cjs');
const {openImportAdministrator}=require('./doc110-admin-navigation.cjs');

async function exerciseProviderLifecycle(auth,baseUrl,token,fixture=sourceFixture()){
  const page=auth.page;
  const panel=page.locator('[role=dialog][aria-label="Administrar proveedores"]');
  const action=id=>panel.locator('[data-proveedor='+id+']');
  const checked=name=>console.log('DOC110_PASS: '+name);
  async function body(response,status=200){
    expect(response.status()).toBe(status);
    let result;try{result=(await response.json()).d;}catch{throw new Error('DOC110_PROVIDER_NON_JSON');}
    expect(result?.ok).toBe(status===200);return result;
  }
  async function call(method,payload,status=200){
    return body(await safePost(auth.context.request,new URL('webservice/WebServiceImportacionCarpetas.asmx/'+method,baseUrl).href,
      {headers:{'X-Importacion-Perfil-CSRF':token},data:{solicitud:payload}},method),status);
  }
  async function click(id,method){
    const pending=page.waitForResponse(r=>r.url().endsWith('/'+method) && r.request().method()==='POST');
    await action(id).click();return (await body(await pending)).datos;
  }
  const refs=d=>({ProveedorId:d.ProveedorId,VersionId:d.VersionId,Revision:d.Revision,RevisionProveedor:d.RevisionProveedor});
  const ready=()=>expect(panel.getByText('Opciones autorizadas disponibles.',{exact:true}).last()).toBeVisible();
  // Comprobar autorización de columnas antes de crear un proveedor descartable.
  const columns=(await call('ConsultarMetadatosProveedor',{tipo:'COLUMNAS',conexionRef:'DA',tabla:fixture.table,pagina:1})).datos;
  expect(columns.some(c=>c.value==='CONSECUTIVO' && c.Tipo==='TEXTO')).toBe(true);
  checked('PROVIDER_AUTHORIZED_SOURCE_PREFLIGHT');
  await body(await openImportAdministrator(page,'admin-proveedores','ListarAdministracionProveedores'));
  await action('nuevo').click();
  await action('codigo').fill('E2E-PROV-'+crypto.randomBytes(10).toString('hex'));
  await action('nombre').fill('Proveedor descartable DOC-110');
  let current=await click('crear','CrearProveedor');
  expect(current.Activo).toBe(false);
  expect(current.ProveedorId).toMatch(/^[1-9][0-9]*$/);expect(current.VersionId).toMatch(/^[1-9][0-9]*$/);
  await expect(action('conexion').locator('option[value=DA]')).toHaveCount(1);
  await action('conexion').selectOption('DA');
  await expect(action('tabla').locator('option[value="'+fixture.table+'"]')).toHaveCount(1);
  await action('tabla').selectOption(fixture.table);
  await expect(action('clave').locator('option[value=CONSECUTIVO]')).toHaveCount(1);
  await action('clave').selectOption('CONSECUTIVO');
  await action('empresa').selectOption('');
  await action('agregar-propiedad').click();
  await panel.getByLabel('Propiedad',{exact:true}).fill('consecutivo');
  await panel.getByLabel('Columna',{exact:true}).selectOption('CONSECUTIVO');
  current=await click('guardar','GuardarProveedor');await ready();
  expect(current.Contrato.ColumnaClave).toBe('CONSECUTIVO');expect(current.Activo).toBe(false);
  checked('PROVIDER_CREATE_CONFIGURE_SAVE_UI');
  if(process.env.DOC110_E2E_PROVIDER_CONCURRENT==='YES'){
    await require('./doc110-provider-concurrency.cjs').exerciseProviderConcurrency(auth,baseUrl,token,current);
    return;
  }
  if(process.env.DOC110_E2E_PROVIDER_GUARDS==='YES')
    await require('./doc110-provider-guards.cjs').exerciseProviderGuards(auth,baseUrl,token,current);
  await click('validar','ValidarProveedor');
  checked('PROVIDER_VALIDATE_UI');
  const ambiguousKey=fixture.ambiguousKey||process.env.DOC110_E2E_AMBIGUOUS_KEY;
  const samples=[{key:'DOC110_ABSENT_'+crypto.randomBytes(16).toString('hex'),expected:'SIN_COINCIDENCIA'}];
  if(ambiguousKey&&ambiguousKey!=='NO_APLICA')samples.push({key:ambiguousKey,expected:'AMBIGUO'});
  else console.log('DOC110_NOT_RUN: PROVIDER_AMBIGUITY_REQUIRES_DUPLICATE_KEY_IN_THIS_FIXTURE');
  for(const sample of samples){
    await action('ejemplo').fill(sample.key);
    const trial=await click('ensayar','EnsayarProveedor');
    expect(trial.Codigo).toBe(sample.expected);
    expect(Object.keys(trial.Valores||{}).length).toBe(0);
    checked('PROVIDER_QUERY_'+sample.expected);
  }
  const unchanged=(await call('ObtenerAdministracionProveedor',refs(current))).datos;
  expect(JSON.stringify(unchanged)===JSON.stringify(current)).toBe(true);
  checked('PROVIDER_TRIALS_DO_NOT_PUBLISH_OR_CHANGE_CONFIGURATION');
  current=await click('publicar','PublicarProveedor');await ready();
  expect(current.Estado).toBe('PUBLICADA');expect(current.Activo).toBe(false);
  await expect(action('guardar')).toHaveCount(0);await expect(action('tabla')).toBeDisabled();
  const original=current;
  const rejected=await call('GuardarProveedor',{...refs(current),Contrato:current.Contrato},409);
  expect(rejected.codigo).toBe('CONFLICTO_REVISION');
  checked('PROVIDER_PUBLICATION_UI_AND_EDIT_REJECTED');
  current=await click('version','CrearVersionProveedor');await ready();
  expect(current.Numero).toBe(original.Numero+1);expect(current.VersionId!==original.VersionId).toBe(true);
  await panel.getByLabel('Propiedad',{exact:true}).fill('numero_orden');
  current=await click('guardar','GuardarProveedor');await ready();
  current=await click('publicar','PublicarProveedor');await ready();
  const old=(await call('ObtenerAdministracionProveedor',refs(original))).datos;
  expect(old.Estado).toBe('PUBLICADA');expect(old.Revision).toBe(original.Revision);
  expect(JSON.stringify(old.Contrato)===JSON.stringify(original.Contrato)).toBe(true);
  expect(current.Contrato.Propiedades[0].Nombre).toBe('numero_orden');
  checked('PROVIDER_SECOND_VERSION_PUBLISHED_FIRST_UNCHANGED');
  if(process.env.DOC110_E2E_PROVIDER_PROFILE==='YES'){
    const key=process.env.DOC110_E2E_UNIQUE_KEY;
    if(!key || key.length>250)throw new Error('DOC110_UNIQUE_KEY_REQUIRED');
    await action('ejemplo').fill(key);
    const trial=await click('ensayar','EnsayarProveedor');
    if(trial.Codigo==='AMBIGUO')
      throw new Error('DOC110_FIXTURE_KEY_AMBIGUOUS: el CONSECUTIVO ingresado tiene varias coincidencias. Repita con una clave autorizada de una sola fila; no se selecciona una coincidencia arbitraria. El proveedor descartable permanece inactivo.');
    if(trial.Codigo==='SIN_COINCIDENCIA')
      throw new Error('DOC110_FIXTURE_KEY_NOT_FOUND: el CONSECUTIVO ingresado no tiene coincidencias. Repita con una clave autorizada de una sola fila. El proveedor descartable permanece inactivo.');
    expect(trial.Codigo).toBe('OK');
    expect(Boolean(trial.Valores && trial.Valores.numero_orden===key)).toBe(true);
    checked('PROVIDER_UNIQUE_KEY_CONFIRMED');
    current=await click('disponibilidad','CambiarDisponibilidadProveedor');await ready();
    expect(current.Activo).toBe(true);
    let profileSnapshot;
    try{
      const draft=(await call('CrearVersionProveedor',refs(current))).datos;
      expect(draft.Estado).toBe('BORRADOR');expect(draft.Activo).toBe(true);
      expect(draft.VersionId!==current.VersionId).toBe(true);
      await action('cerrar').click();
      await page.locator('#doc110-navigation-toggle').click();
      const listing=page.waitForResponse(r=>r.url().endsWith('/ListarPerfiles'));
      await page.locator('[data-doc110=abrir]').click();await body(await listing);
      profileSnapshot=await require('./doc110-publication-lifecycle.cjs').exercisePublication(auth,baseUrl,token,null,{provider:current,key,draft});
    }finally{
      // Solo el proveedor descartable creado por esta corrida; recuperar revisión antes de desactivar.
      const latest=(await call('ObtenerAdministracionProveedor',refs(current))).datos;
      if(latest.Activo)current=(await call('CambiarDisponibilidadProveedor',{...refs(latest),Activo:false})).datos;
      else current=latest;
      expect(current.Activo).toBe(false);
      checked('INTEGRATION_DISPOSABLE_PROVIDER_DISABLED');
    }
    const history=(await call('ListarAdministracionProveedores',{Pagina:1,ProveedorId:current.ProveedorId})).datos;
    expect(history.length).toBe(3);expect(history.every(v=>!v.Activo)).toBe(true);
    expect(history.filter(v=>v.Estado==='PUBLICADA').length).toBe(2);
    expect(history.filter(v=>v.Estado==='BORRADOR').length).toBe(1);
    await require('./doc110-provider-history.cjs').verifyConsumerHistory(call,current,profileSnapshot);
    return {provider:current,profileSnapshot};
  }
  const history=await click('historial','ListarAdministracionProveedores');
  expect(history.length).toBe(2);expect(history.every(v=>v.Estado==='PUBLICADA' && v.Activo===false)).toBe(true);
  expect(new Set(history.map(v=>v.Numero)).size).toBe(2);
  checked('PROVIDER_HISTORY_TWO_VERSIONS_REMAINS_INACTIVE');
}
module.exports={exerciseProviderLifecycle};
