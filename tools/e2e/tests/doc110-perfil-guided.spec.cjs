'use strict';
const {test,expect}=require('@playwright/test');
const path=require('node:path');
const {assertReadOnlyProvider}=require('./support/doc110-provider-source-diagnostics.cjs');
const root=path.resolve(__dirname,'../../..');

test('driver proveedor publicado: atributo del fieldset y controles internos deshabilitados',async({page})=>{
 await page.setContent('<section role="dialog"><fieldset disabled><legend>Configuración</legend><input><select><option>Dato sintético</option></select><textarea></textarea><button>Acción</button></fieldset></section>');
 await assertReadOnlyProvider(page.getByRole('dialog'));
});
test('driver proveedor publicado: rechaza formulario editable y control habilitado dentro de legend',async({page})=>{
 for(const form of ['<fieldset><input disabled></fieldset>','<fieldset disabled><legend><input></legend><input></fieldset>']){
  await page.setContent('<section role="dialog">'+form+'</section>');
  await expect(assertReadOnlyProvider(page.getByRole('dialog'),300)).rejects.toThrow();
 }
});

// UI real y biblioteca existente, transporte simulado. No acredita ASMX/BD ni permisos.
test('COR-04: detalle guiado, edición pendiente, guardado y recuperación por campo',async({page})=>{
  await page.setContent('<div id="doc110-perfiles" data-api="/webservice/WebServiceImportacionCarpetas.asmx" data-token="synthetic"></div>');
  for(const script of ['js/ui/jquery-3.4.1.min.js','js/ui/jquery-ui-1.8.22.custom.js'])await page.addScriptTag({path:path.join(root,script)});
  await page.addStyleTag({path:path.join(root,'js/gestion/ImportacionCarpetas/PerfilImportacionEditor.css')});
  await page.evaluate(()=>{
    window.guidedCalls=[];let seriesFails=true;
    let profile={PerfilId:'1',Codigo:'QA',Nombre:'Perfil de prueba',Revision:1,Activo:true,Version:{VersionId:'2',PerfilId:'1',Numero:1,Revision:1,Estado:'BORRADOR',Reglas:[],Configuracion:{NombreVisible:'Prueba',OrigenGabinete:'DA',ModoIntegridad:'COMPLETO_OBLIGATORIO',Extensiones:[]}}};
    const option=(id,name)=>({value:id,text:name});
    window.PerfilImportacionApi={crear(){return {ejecutar(operation,payload){
      window.guidedCalls.push({operation,payload});const result=window.jQuery.Deferred();
      setTimeout(()=>{
        let data;
        if(operation==='ListarPerfiles')data={Elementos:[profile],Total:1};
        else if(operation==='ObtenerPerfil')data=profile;
        else if(operation==='ConsultarProveedores')data=[{ProveedorId:'1',VersionId:'3',Nombre:'Fuente configurable',Numero:2,TipoClave:'TEXTO',Propiedades:[{Nombre:'beneficiario',Tipo:'TEXTO'},{Nombre:'fecha',Tipo:'TEXTO'}]}];
        else if(operation==='ConsultarCatalogo'){
          const c=payload.catalogo;
          if(c==='SERIE' && seriesFails){seriesFails=false;result.reject({mensaje:'Catálogo temporalmente no disponible'});return;}
          data=c==='INSTRUMENTO'?[option('1','Instrumento A'),option('2','Instrumento B')]:c==='AREA'?[option('3','Área autorizada')]:c==='SERIE'?[option('4','Serie')]:c==='SUBSERIE'?[]:c==='GABINETE'?[option('7','Gabinete')]:c==='INDICE'?[{...option('8','Orden'),Tipo:'TEXTO',Longitud:20},{...option('9','Fecha'),Tipo:'FECHA',Longitud:10}]:[option('1','Opción')];
        }else if(operation==='GuardarBorrador'){
          profile={...profile,Version:{...profile.Version,Revision:2,Configuracion:payload.configuracion,Reglas:payload.reglas}};data=profile;
        }else if(operation==='EnsayarReglas'){
          if(window.guidedTrialError){result.reject({codigo:'ACCESO_DENEGADO',mensaje:'Fuente revocada. Ensayo rechazado.'});return;}
          const prop=payload.reglas[0].PropiedadExpediente;
          data=[{Valores:prop==='SUBSERIE_ID'?[]:[{DestinoTipo:'INDICE_GABINETE',DestinoRef:'8',Valor:prop==='SERIE_CODIGO'?'0007':null,Diferido:prop==='SERIE_NOMBRE',Procedencia:'PROPUESTA_PERFIL'}],Incidencias:prop==='SUBSERIE_ID'?[{Campo:'INDICE_GABINETE:8',Codigo:'VALOR_REQUERIDO'}]:[]}];
        }else throw new Error('Operación inesperada en fixture');
        result.resolve({mensaje:'Listo',datos:data});
      },20);
      return result.promise();
    }};}};
  });
  await page.addScriptTag({path:path.join(root,'js/gestion/ImportacionCarpetas/PerfilImportacionEditor.js')});
  await page.locator('[data-doc110=abrir]').click();await page.getByRole('button',{name:'Abrir',exact:true}).click();
  const f=key=>page.locator('[data-doc110-field='+key+']');
  await expect(page.getByRole('button',{name:'Cargar opciones',exact:true})).toHaveCount(0);
  await expect(f('AreaRef')).toBeDisabled();await expect(f('InstrumentoRef')).toBeEnabled();
  await f('InstrumentoRef').selectOption('1');await expect(f('AreaRef')).toBeEnabled();await f('AreaRef').selectOption('3');
  await expect(page.locator('[data-doc110=reintentar-SerieRef]')).toBeVisible();
  await page.locator('[data-doc110=reintentar-SerieRef]').click();await expect(f('SerieRef')).toBeEnabled();await f('SerieRef').selectOption('4');
  await expect(page.locator('[data-doc110-catalog=SubserieRef]')).toHaveText('Sin opciones disponibles.');
  await f('GabineteId').selectOption('7');await expect(page.locator('[data-doc110-catalog=INDICE]')).toHaveAttribute('data-state','READY');
  await f('ModoIntegridad').selectOption('PARCIAL_PERMITIDO');await expect(f('AmbitoIntegridad')).toBeDisabled();await expect(f('AmbitoIntegridad')).toHaveValue('NO_APLICA');
  await expect(page.locator('[data-doc110=publicar]')).toBeDisabled();await expect(page.locator('[data-doc110=recargar]')).toBeHidden();
  page.once('dialog',dialog=>dialog.dismiss());await page.getByRole('button',{name:'Volver al listado',exact:true}).click();await expect(f('SerieRef')).toHaveValue('4');
  await page.locator('[data-doc110=guardar]').click();await expect(page.locator('[data-doc110=publicar]')).toBeEnabled();
  await expect(f('InstrumentoRef')).toBeEnabled();await f('InstrumentoRef').selectOption('2');
  await expect(f('AreaRef')).toHaveValue('');await expect(f('SerieRef')).toBeDisabled();await expect(f('SerieRef')).toHaveValue('');
  const writes=await page.evaluate(()=>window.guidedCalls.filter(c=>c.operation==='GuardarBorrador').length);expect(writes).toBe(1);
  await page.locator('[data-doc110=agregar-regla]').click();
  const r=key=>page.locator('[data-doc110-rule='+key+']');
  await r('destino').selectOption('INDICE_GABINETE');await r('campo').selectOption('8');
  await r('fuente').selectOption('PROVEEDOR');await r('proveedor').selectOption('1:3');await r('proveedorPropiedad').selectOption('beneficiario');
  await r('claveExtraccion').selectOption('SEGMENTO');await r('claveSeparador').fill('-');await r('claveExtension').check();
  await page.locator('[data-doc110=guardar]').click();
  await expect(r('proveedor')).toHaveValue('1:3');await expect(r('proveedorPropiedad')).toHaveValue('beneficiario');await expect(r('claveSeparador')).toHaveValue('-');
  const saved=await page.evaluate(()=>window.guidedCalls.filter(c=>c.operation==='GuardarBorrador').slice(-1)[0].payload.reglas[0]);
  expect(saved.Proveedor.Clave.Posicion).toBe(0);expect(saved.Proveedor.Clave.QuitarExtension).toBe(true);expect(saved.FuenteTipo).toBe('PROVEEDOR');
  await r('campo').selectOption('9');await r('proveedorPropiedad').selectOption('fecha');await r('formato').selectOption('dd/MM/yyyy H:mm:ss');
  await page.locator('[data-doc110=guardar]').click();await expect(r('formato')).toHaveValue('dd/MM/yyyy H:mm:ss');
  const dateRule=await page.evaluate(()=>window.guidedCalls.filter(c=>c.operation==='GuardarBorrador').slice(-1)[0].payload.reglas[0]);
  expect(dateRule.TipoValor).toBe('FECHA');expect(dateRule.FormatoFecha).toBe('dd/MM/yyyy H:mm:ss');
  await r('campo').selectOption('8');
  await r('fuente').selectOption('EXPEDIENTE');
  for(const prop of ['SERIE_ID','SERIE_CODIGO','SERIE_NOMBRE','SUBSERIE_ID','SUBSERIE_CODIGO','SUBSERIE_NOMBRE'])await expect(r('propiedad').locator('option[value="'+prop+'"]')).toHaveCount(1);
  await r('propiedad').selectOption('SERIE_CODIGO');await page.locator('[data-doc110=guardar]').click();await expect(r('propiedad')).toHaveValue('SERIE_CODIGO');
  await page.locator('[data-doc110=ensayar]').click();await expect(page.locator('[data-doc110=resultado-ensayo]')).toContainText('0007');await expect(page.locator('[data-doc110=resultado-ensayo]')).toContainText('propuesta del perfil');
  await page.evaluate(()=>{window.guidedTrialError=true;});
  await page.locator('[data-doc110=ensayar]').click();await expect(page.locator('[data-doc110=estado]')).toContainText('Ensayo rechazado');
  await expect(page.locator('[data-doc110=resultado-ensayo]')).toBeEmpty();await expect(page.locator('[data-doc110=ensayar]')).toBeEnabled();
  await page.evaluate(()=>{window.guidedTrialError=false;});
  await r('propiedad').selectOption('SERIE_NOMBRE');await page.locator('[data-doc110=ensayar]').click();await expect(page.locator('[data-doc110=resultado-ensayo]')).toContainText('Se resolverá');
  await r('propiedad').selectOption('SUBSERIE_ID');await page.locator('[data-doc110=ensayar]').click();await expect(page.locator('[data-doc110=resultado-ensayo]')).toContainText('VALOR_REQUERIDO');
});
// Administración visual con transporte simulado; no acredita permisos ni persistencia reales.
test('AMP-01: proveedor creado, configurado, validado, ensayado y publicado desde UI',async({page})=>{
 await page.setContent('<div id="doc110-perfiles" data-proveedores="1"></div>');
 await page.addScriptTag({path:path.join(root,'js/ui/jquery-3.4.1.min.js')});
 await page.addStyleTag({path:path.join(root,'js/gestion/ImportacionCarpetas/PerfilImportacionEditor.css')});
 await page.addScriptTag({path:path.join(root,'js/gestion/ImportacionCarpetas/ProveedorAdminEditor.js')});
 await page.evaluate(()=>{
  let saved=null;window.adminCalls=[];
  window.ProveedorAdminEditor.inicializar(document.getElementById('doc110-perfiles'),{ejecutar(op,s){const d=jQuery.Deferred();window.adminCalls.push(op);setTimeout(()=>{
   let data;
   if(op==='ListarAdministracionProveedores')data=saved?[saved]:[];
   else if(op==='CrearProveedor')data=saved={ProveedorId:'1',VersionId:'2',Codigo:s.Codigo,Nombre:s.Nombre,Revision:1,RevisionProveedor:1,Numero:1,Estado:'BORRADOR',Activo:false,Contrato:{TipoClave:'TEXTO',Propiedades:[]}};
   else if(op==='ConsultarMetadatosProveedor')data=s.tipo==='CONEXIONES'?[{value:'RA'}]:s.tipo==='TABLAS'?[{value:'ordenes'}]:[{value:'numero',Tipo:'TEXTO'},{value:'empresa',Tipo:'ENTERO'},{value:'nombre',Tipo:'TEXTO'}];
   else if(op==='GuardarProveedor')data=saved={...saved,Revision:2,RevisionProveedor:2,Contrato:s.Contrato};
   else if(op==='ValidarProveedor')data=saved;
   else if(op==='EnsayarProveedor'){
    if(s.ClaveEjemplo==='error'){d.reject({mensaje:'Origen temporalmente no disponible'});return;}
    data=s.ClaveEjemplo==='44'?{Codigo:'AMBIGUO',Valores:{}}:s.ClaveEjemplo==='ausente'?{Codigo:'SIN_COINCIDENCIA',Valores:{}}:{Codigo:'OK',Valores:{beneficiario:'Ejemplo'}};
   }
   else if(op==='PublicarProveedor')data=saved={...saved,Estado:'PUBLICADA',Revision:3,RevisionProveedor:3};
   else if(op==='CrearVersionProveedor')data=saved={...saved,Estado:'BORRADOR',VersionId:'3',Numero:2,Revision:1,RevisionProveedor:4};
   else throw new Error('Unexpected operation '+op);
   d.resolve({mensaje:'Listo',datos:data});
  },10);return d.promise();}});
 });
 const action=id=>page.locator('[data-proveedor='+id+']');
 await page.locator('[data-doc110=admin-proveedores]').click();await action('nuevo').click();await action('codigo').fill('QA');await action('nombre').fill('Proveedor configurable');await action('crear').click();
 await action('conexion').selectOption('RA');await action('tabla').selectOption('ordenes');await action('clave').selectOption('numero');await action('empresa').selectOption('empresa');await action('agregar-propiedad').click();
 await page.getByLabel('Propiedad',{exact:true}).fill('beneficiario');await page.getByLabel('Columna',{exact:true}).selectOption('nombre');await action('guardar').click();
 await expect(page.getByRole('status').last()).toHaveText('Opciones autorizadas disponibles.');await action('validar').click();await expect(page.getByRole('status').first()).toContainText('Estructura válida');
 await action('ejemplo').fill('00012');await action('ensayar').click();await expect(page.getByText('beneficiario: Ejemplo',{exact:true})).toBeVisible();await action('publicar').click();
 await expect(action('tabla')).toBeDisabled();await expect(action('guardar')).toHaveCount(0);await expect(page.getByRole('status').last()).toHaveText('Opciones autorizadas disponibles.');
 await assertReadOnlyProvider(page.getByRole('dialog',{name:'Administrar proveedores',exact:true}));
 await page.setViewportSize({width:1000,height:600});
 await action('ejemplo').fill('00012');await action('ensayar').click();await expect(action('valores')).toContainText('beneficiario: Ejemplo');
 await action('ejemplo').fill('44');await action('ensayar').click();await expect(action('resultado')).toContainText('más de un registro');
 await expect(action('valores')).not.toContainText('beneficiario: Ejemplo');await expect(action('valores')).toContainText('No se seleccionó ninguno');
 const rect=await action('resultado').boundingBox();expect(rect.y).toBeGreaterThanOrEqual(0);expect(rect.y+rect.height).toBeLessThanOrEqual(600);
 await action('ejemplo').fill('ausente');await action('ensayar').click();await expect(action('valores')).toContainText('No se encontró un registro');
 await action('ejemplo').fill('00012');await action('ensayar').click();await expect(action('valores')).toContainText('beneficiario: Ejemplo');
 await action('ejemplo').fill('error');await action('ensayar').click();await expect(action('resultado')).toContainText('Origen temporalmente');await expect(action('valores')).toBeEmpty();
 const before=await page.evaluate(()=>window.adminCalls.filter(x=>x==='EnsayarProveedor').length);
 await action('ejemplo').fill('');await action('ensayar').click();await expect(action('resultado')).toContainText('Ingrese una clave');
 expect(await page.evaluate(()=>window.adminCalls.filter(x=>x==='EnsayarProveedor').length)).toBe(before);
 await action('version').click();await expect(page.getByRole('heading',{level:3})).toContainText('versión 2');await expect(action('tabla')).toBeEnabled();
 const calls=await page.evaluate(()=>window.adminCalls);expect(calls.filter(x=>x==='PublicarProveedor')).toHaveLength(1);
});
