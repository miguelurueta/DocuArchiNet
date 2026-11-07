'use strict';
const {test,expect}=require('@playwright/test');
const path=require('node:path');
const root=path.resolve(__dirname,'../../..');

// Componentes productivos y transporte simulado; no usa sesión ni modifica RA/DA.
test('DOC-110: tablas, formulario adaptable y navegación de proveedores',async({page},testInfo)=>{
 await page.setViewportSize({width:1366,height:900});
 await page.setContent('<div id="doc110-perfiles" data-proveedores="1"></div>');
 for(const file of ['js/ui/jquery-3.4.1.min.js','js/gestion/ImportacionCarpetas/ProveedorAdminEditor.js'])await page.addScriptTag({path:path.join(root,file)});
 await page.addStyleTag({path:path.join(root,'js/gestion/ImportacionCarpetas/PerfilImportacionEditor.css')});
 await page.evaluate(()=>{
  const properties=['CODIGOTIPO','TIPO','CONSECUTIVO','FECHA','NIT','BENEFICIARIO'].map(Columna=>({Nombre:Columna.toLowerCase(),Columna,Tipo:'TEXTO'}));
  const doc={ProveedorId:'1',VersionId:'2',Revision:1,RevisionProveedor:1,Codigo:'EGRESOS',Nombre:'Consulta de egresos',Numero:1,Estado:'BORRADOR',Activo:false,Contrato:{ConexionRef:'DA',Tabla:'imp_02_egresos',ColumnaClave:'CONSECUTIVO',ColumnaEmpresa:'',TipoClave:'TEXTO',Propiedades:properties}};
  window.layoutCalls=[];
  window.ProveedorAdminEditor.inicializar(document.getElementById('doc110-perfiles'),{ejecutar(op,s){window.layoutCalls.push(op);const d=jQuery.Deferred();setTimeout(()=>{
   const data=op==='ListarAdministracionProveedores'?[doc]:op==='ObtenerAdministracionProveedor'?doc:op==='ConsultarMetadatosProveedor'?(s.tipo==='CONEXIONES'?[{value:'DA'}]:s.tipo==='TABLAS'?[{value:'imp_02_egresos'}]:properties.map(p=>({value:p.Columna,Tipo:p.Tipo}))):null;
   if(data===null)throw new Error('Unexpected operation '+op);d.resolve({mensaje:'Configuración disponible',datos:data});
  },10);return d.promise();}});
 });
 const p=id=>page.locator('[data-proveedor='+id+']');
 await page.locator('[data-doc110=admin-proveedores]').click();
 await expect(page.getByRole('table',{name:'Proveedores registrados'})).toBeVisible();
 await page.screenshot({path:testInfo.outputPath('proveedores-listado.png')});
 await p('abrir').click();await expect(page.getByRole('status').last()).toHaveText('Opciones autorizadas disponibles.');
 await expect(page.getByRole('table',{name:'Propiedades del proveedor'}).locator('tbody tr')).toHaveCount(6);
 await page.screenshot({path:testInfo.outputPath('proveedor-configuracion.png')});
 await p('ensayar').scrollIntoViewIfNeeded();
 const status=await p('resultado').boundingBox(),close=await p('cerrar').boundingBox();
 expect(status.y).toBeGreaterThan(0);expect(status.y+status.height).toBeLessThan(900);expect(close.y).toBeGreaterThan(0);
 await page.screenshot({path:testInfo.outputPath('proveedor-consulta.png')});
 await page.setViewportSize({width:390,height:844});
 await p('conexion').scrollIntoViewIfNeeded();
 const overflow=await page.locator('.doc110-body').evaluate(el=>el.scrollWidth>el.clientWidth+1);expect(overflow).toBe(false);
 await expect(p('conexion')).toBeInViewport();await expect(p('cerrar')).toBeInViewport();
 await page.screenshot({path:testInfo.outputPath('proveedor-movil.png')});
 await p('cerrar').focus();await page.keyboard.press('Tab');expect(await page.evaluate(()=>document.activeElement.closest('[role=dialog]')!==null)).toBe(true);
 await page.keyboard.press('Escape');await expect(page.getByRole('dialog')).toBeHidden();await expect(page.locator('[data-doc110=admin-proveedores]')).toBeFocused();
 expect(await page.evaluate(()=>window.layoutCalls.every(op=>['ListarAdministracionProveedores','ObtenerAdministracionProveedor','ConsultarMetadatosProveedor'].includes(op)))).toBe(true);
});
