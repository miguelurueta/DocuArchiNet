'use strict';
const {expect}=require('@playwright/test');
const fs=require('node:fs'),path=require('node:path');
const {safePost}=require('./doc110-safe-request.cjs');
const {sourceFixture}=require('./doc110-source-fixture.cjs');
const {openImportAdministrator}=require('./doc110-admin-navigation.cjs');

async function assertReadOnlyProvider(panel,timeout=15000){
  const form=panel.locator('fieldset').first();
  await expect(form).toHaveAttribute('disabled','',{timeout});
  const controls=form.locator('input,select,textarea,button');
  expect(await controls.count()).toBeGreaterThan(0);
  for(const control of await controls.all())await expect(control).toBeDisabled({timeout});
  await expect(panel.locator('[data-proveedor=guardar]')).toHaveCount(0);
}

async function discoverStoredProvider(call,table){
  let inspected=0;
  for(let page=1;page<=20;page++){
    const items=(await call('ListarAdministracionProveedores',{Pagina:page})).datos;
    if(!Array.isArray(items)||items.length>25)throw new Error('DOC110_DIAGNOSTIC_LIST_INVALID');
    for(const item of items){
      if(!/^E2E-PROV-[a-f0-9]+$/.test(item.Codigo||'')||item.Estado!=='PUBLICADA'||item.Activo)continue;
      if(++inspected>100)throw new Error('DOC110_DIAGNOSTIC_READ_BUDGET');
      const saved=(await call('ObtenerAdministracionProveedor',{ProveedorId:item.ProveedorId,VersionId:item.VersionId})).datos;
      if(saved.Estado==='PUBLICADA'&&!saved.Activo&&saved.Contrato?.ConexionRef==='DA'&&saved.Contrato.Tabla===table)return saved;
    }
    if(items.length<25)break;
  }
  throw new Error('DOC110_DIAGNOSTIC_FIXTURE_NOT_FOUND');
}

async function exerciseProviderSourceDiagnostics(auth,baseUrl,token){
  const fixture=sourceFixture(process.env.DOC110_E2E_SOURCE_TABLE),page=auth.page;
  const evidence={status:'RUNNING',startedUtc:new Date().toISOString()};
  async function body(response){
    expect(response.status()).toBe(200);
    let result;try{result=(await response.json()).d;}catch{throw new Error('DOC110_DIAGNOSTIC_NON_JSON');}
    expect(result?.ok).toBe(true);return result;
  }
  const call=async(method,payload)=>body(await safePost(auth.context.request,
    new URL('webservice/WebServiceImportacionCarpetas.asmx/'+method,baseUrl).href,
    {headers:{'X-Importacion-Perfil-CSRF':token},data:{solicitud:payload}},method));
  let writes=0,forbiddenColumns=0;
  const observe=request=>{
    if(!request.url().includes('/WebServiceImportacionCarpetas.asmx/'))return;
    const method=request.url().split('/').pop();
    if(/^(Crear|Guardar|Publicar|Cambiar|Actualizar|Activar|Revocar)/.test(method))writes++;
    if(method==='ConsultarMetadatosProveedor'){
      try{const s=request.postDataJSON().solicitud;if(s.tipo==='COLUMNAS'&&s.tabla===fixture.table)forbiddenColumns++;}catch{}
    }
  };
  page.on('request',observe);
  try{
    const saved=await discoverStoredProvider(call,fixture.table);
    let inactive=false,finished=false;
    for(let pageIndex=1;pageIndex<=100;pageIndex++){
      const items=(await call('ListarFuentes',{Pagina:pageIndex,Termino:fixture.table})).datos;
      const source=items.find(d=>d.ConexionRef==='DA'&&d.Tabla===fixture.table);
      if(source){expect(source.Activo).toBe(false);inactive=true;}
      if(items.length<25){finished=true;break;}
    }
    expect(inactive&&finished).toBe(true);
    await body(await openImportAdministrator(page,'admin-proveedores','ListarAdministracionProveedores'));
    const panel=page.getByRole('dialog',{name:'Administrar proveedores',exact:true});
    await panel.getByRole('textbox',{name:'Buscar por código o nombre',exact:true}).fill(saved.Codigo);
    const listing=page.waitForResponse(r=>r.url().endsWith('/ListarAdministracionProveedores'),{timeout:15000});
    await panel.getByRole('button',{name:'Buscar',exact:true}).click();await body(await listing);
    const row=panel.getByRole('row').filter({has:page.getByRole('cell',{name:saved.Codigo,exact:true})})
      .filter({has:page.getByRole('cell',{name:String(saved.Numero),exact:true})})
      .filter({has:page.getByRole('cell',{name:'PUBLICADA',exact:true})});
    await expect(row).toHaveCount(1);
    const detail=page.waitForResponse(r=>r.url().endsWith('/ObtenerAdministracionProveedor'),{timeout:15000});
    await row.getByRole('button',{name:'Abrir',exact:true}).click();await body(await detail);
    await expect(panel.getByText('Referencias guardadas no disponibles.',{exact:false})).toBeVisible();
    for(const [id,value] of [['conexion','DA'],['tabla',fixture.table],['clave',saved.Contrato.ColumnaClave],['empresa',saved.Contrato.ColumnaEmpresa||'']])
      await expect(panel.locator('[data-proveedor='+id+']')).toHaveValue(value);
    const properties=panel.getByLabel('Columna',{exact:true});await expect(properties).toHaveCount(saved.Contrato.Propiedades.length);
    for(let i=0;i<saved.Contrato.Propiedades.length;i++)await expect(properties.nth(i)).toHaveValue(saved.Contrato.Propiedades[i].Columna);
    await assertReadOnlyProvider(panel);
    const after=(await call('ObtenerAdministracionProveedor',{ProveedorId:saved.ProveedorId,VersionId:saved.VersionId})).datos;
    expect(JSON.stringify(after)===JSON.stringify(saved)).toBe(true);expect(writes).toBe(0);expect(forbiddenColumns).toBe(0);
    evidence.status='PASS';console.log('DOC110_PASS: REVOKED_SOURCE_UI_DIAGNOSTIC_PRESERVES_PUBLISHED_REFERENCES_WITHOUT_WRITES');
  }catch(error){evidence.status='FAIL';throw error;}
  finally{
    page.off('request',observe);evidence.finishedUtc=new Date().toISOString();
    fs.writeFileSync(path.resolve(__dirname,'../../artifacts/doc110-provider-source-diagnostics-result.json'),JSON.stringify(evidence,null,2));
  }
}
module.exports={exerciseProviderSourceDiagnostics,discoverStoredProvider,assertReadOnlyProvider};
