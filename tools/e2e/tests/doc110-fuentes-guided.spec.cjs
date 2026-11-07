'use strict';
const {test,expect}=require('@playwright/test');
const path=require('node:path');
const {closeImportDialog,sourceRow}=require('./support/doc110-source-profile.cjs');
const root=path.resolve(__dirname,'../../..');
test('driver fuentes: fila exacta DA dentro del dialogo, sin confundir RA, sufijos ni otra tabla',async({page})=>{
 await page.setContent('<section role="dialog" aria-label="Administrar fuentes"><table><tbody>'+
  '<tr><td>RA</td><td>imp_02_creditos</td><td><button>Abrir</button></td></tr>'+
  '<tr><td>DA</td><td>imp_02_creditos_extra</td><td><button>Abrir</button></td></tr>'+
  '<tr data-target="1"><td>DA</td><td>imp_02_creditos</td><td><button>Abrir</button></td></tr>'+
  '<tr><td>DA</td><td>imp_02_egresos</td><td><button>Abrir</button></td></tr>'+
  '</tbody></table></section><table><tbody><tr><td>DA</td><td>imp_02_creditos</td></tr></tbody></table>');
 const panel=page.getByRole('dialog',{name:'Administrar fuentes',exact:true});
 const row=sourceRow(page,panel,'imp_02_creditos');
 await expect(row).toHaveCount(1);await expect(row).toHaveAttribute('data-target','1');
 await row.getByRole('button',{name:'Abrir',exact:true}).click();
 await expect(row.getByRole('button')).toBeFocused();
 await expect(sourceRow(page,panel,'imp_02_inexistente')).toHaveCount(0);
});
// Interfaz real, transporte simulado: no acredita ASMX, permisos ni escrituras RA.
test('AMP-02: fuente DA por interfaz, edicion, proveedor publicado, perfil, ensayo e historial tras revocacion',async({page},testInfo)=>{
 await page.setContent('<div id="doc110-perfiles" data-fuentes="1" data-proveedores="1"></div>');
 await page.addScriptTag({path:path.join(root,'js/ui/jquery-3.4.1.min.js')});
 await page.addScriptTag({path:path.join(root,'js/ui/jquery-ui-1.8.22.custom.js')});
 await page.addStyleTag({path:path.join(root,'js/gestion/ImportacionCarpetas/PerfilImportacionEditor.css')});
 for(const file of ['FuenteAdminEditor.js','ProveedorAdminEditor.js'])await page.addScriptTag({path:path.join(root,'js/gestion/ImportacionCarpetas',file)});
 await page.evaluate(()=>{
  let source=null,history=[],provider=null,profile=null;window.sourceCalls=[];
  window.integrationSnapshot=()=>JSON.parse(JSON.stringify({source,history,provider,profile}));
  const cols=['CONSECUTIVO','NIT','BENEFICIARIO'].map(value=>({value,Tipo:'TEXTO'}));
  const api={ejecutar(op,s){window.sourceCalls.push(op);const d=jQuery.Deferred();setTimeout(()=>{
   let data;
   if(op==='ListarFuentes')data=source?[source]:[];
   else if(op==='ObtenerFuente')data=source;
   else if(op==='ConsultarMetadatosFuente')data=s.tipo==='CONEXIONES'?[{value:'DA'}]:s.tipo==='TABLAS'?[{value:'imp_02_egresos'}]:cols;
   else if(['CrearFuente','GuardarFuente','ActivarFuente','RevocarFuente'].includes(op)){
    if(op==='CrearFuente')source={...s.Cambio.Documento,FuenteId:'1',Revision:1,Activo:false};
    else if(op==='GuardarFuente')source={...s.Cambio.Documento,Activo:source.Activo,Revision:source.Revision+1};
    else source={...source,Activo:op==='ActivarFuente',Revision:source.Revision+1};
    history.unshift({Revision:source.Revision,Accion:op,ActorRef:'1',FechaUtc:'2026-10-23',Motivo:s.Cambio.Motivo,Estado:JSON.parse(JSON.stringify(source))});data=source;
   }else if(op==='HistorialFuente')data=history;
   else if(op==='ListarAdministracionProveedores')data=provider?[provider]:[];
   else if(op==='ObtenerAdministracionProveedor')data=provider;
   else if(op==='CrearProveedor')data=provider={ProveedorId:'2',VersionId:'3',Revision:1,RevisionProveedor:1,Codigo:s.Codigo,Nombre:s.Nombre,Estado:'BORRADOR',Numero:1,Activo:false,Contrato:{TipoClave:'TEXTO',Propiedades:[]}};
   else if(op==='ConsultarMetadatosProveedor')data=!source?.Activo?[]:s.tipo==='CONEXIONES'?[{value:'DA'}]:s.tipo==='TABLAS'?[{value:source.Tabla}]:cols.filter(c=>source.Columnas.includes(c.value));
   else if(op==='GuardarProveedor')data=provider={...provider,Contrato:s.Contrato,Revision:2,RevisionProveedor:2};
   else if(op==='PublicarProveedor')data=provider={...provider,Estado:'PUBLICADA',Revision:provider.Revision+1,RevisionProveedor:provider.RevisionProveedor+1};
   else if(op==='CambiarDisponibilidadProveedor')data=provider={...provider,Activo:s.Activo,RevisionProveedor:provider.RevisionProveedor+1};
   else if(op==='ListarPerfiles')data={Elementos:profile?[profile]:[],Total:profile?1:0};
   else if(op==='CrearPerfil')data=profile={PerfilId:'4',Codigo:s.codigo,Nombre:s.nombre,Revision:1,Activo:true,
    Version:{PerfilId:'4',VersionId:'5',Revision:1,Numero:1,Estado:'BORRADOR',Configuracion:{OrigenGabinete:'DA',ModoIntegridad:'COMPLETO_OBLIGATORIO',Extensiones:[]},Reglas:[]}};
   else if(op==='ObtenerPerfil')data=profile;
   else if(op==='ConsultarCatalogo')data=s.catalogo==='GABINETE'?[{value:'7',text:'Gabinete autorizado'}]:s.catalogo==='INDICE'?[{value:'8',text:'Índice autorizado',Tipo:'TEXTO',Longitud:100}]:[];
   else if(op==='ConsultarProveedores')data=source?.Activo&&provider?.Activo&&provider.Estado==='PUBLICADA'?
    [{ProveedorId:provider.ProveedorId,VersionId:provider.VersionId,Nombre:provider.Nombre,Numero:1,TipoClave:'TEXTO',Propiedades:[{Nombre:'beneficiario',Tipo:'TEXTO'}]}]:[];
   else if(op==='GuardarBorrador')data=profile={...profile,Version:{...profile.Version,Configuracion:s.configuracion,Reglas:s.reglas,Revision:profile.Version.Revision+1}};
   else if(op==='EnsayarReglas'){
    if(!source?.Activo){d.reject({codigo:'ACCESO_DENEGADO',mensaje:'Fuente revocada. No se consultaron datos.'});return;}
    const rule=s.reglas[0];
    if(rule?.Proveedor?.ProveedorId!==provider.ProveedorId||rule.Proveedor.VersionId!==provider.VersionId||rule.Proveedor.Propiedad!=='beneficiario')throw new Error('Unexpected provider binding');
    data=[{Valores:[{DestinoTipo:rule.DestinoTipo,DestinoRef:rule.DestinoRef,Valor:'Ejemplo autorizado',Diferido:false,FuenteTipo:'PROVEEDOR'}],Incidencias:[]}];
   }
   else throw new Error('Unexpected '+op);
   d.resolve({mensaje:'Listo',datos:JSON.parse(JSON.stringify(data))});
  },15);return d.promise();}};
  const host=document.getElementById('doc110-perfiles');window.FuenteAdminEditor.inicializar(host,api);window.ProveedorAdminEditor.inicializar(host,api);
  window.PerfilImportacionApi={crear:()=>api};
 });
 await page.addScriptTag({path:path.join(root,'js/gestion/ImportacionCarpetas/PerfilImportacionEditor.js')});
 const f=id=>page.locator('[data-fuente='+id+']');
 await page.locator('[data-doc110=admin-fuentes]').click();await f('nueva').click();
 await f('conexion').selectOption('DA');await f('tabla').selectOption('imp_02_egresos');
 await page.getByLabel('Permitir CONSECUTIVO',{exact:true}).check();await page.getByLabel('Permitir BENEFICIARIO',{exact:true}).check();
 await expect(f('empresa')).toBeDisabled();await f('motivo').fill('Autorizar fuente exclusiva');await f('guardar').click();
 await expect(page.getByRole('heading',{level:3})).toContainText('revisión 1');await expect(f('conexion')).toBeDisabled();
 // Tras regenerar detalle el botón pulsado ya no existe y el foco puede quedar en body.
 await page.evaluate(()=>{document.body.tabIndex=-1;document.body.focus();});
 await closeImportDialog(page);
 await page.locator('[data-doc110=admin-fuentes]').click();await f('abrir').click();
 await expect(page.getByRole('status').last()).toHaveText('Opciones disponibles.');await f('motivo').fill('Habilitar consultas');await f('disponibilidad').click();
 await expect(page.getByRole('heading',{level:3})).toContainText('Activa');await f('conexion').scrollIntoViewIfNeeded();await page.screenshot({path:testInfo.outputPath('fuente-configuracion.png')});
 await page.setViewportSize({width:390,height:844});await f('motivo').scrollIntoViewIfNeeded();await expect(f('cerrar')).toBeInViewport();
 expect(await page.locator('.doc110-body').first().evaluate(el=>el.scrollWidth>el.clientWidth+1)).toBe(false);
 await page.setViewportSize({width:1280,height:720});await f('cerrar').click();
 // Editar una autorización ya activa no debe perder identidad ni crear otra fuente.
 await page.locator('[data-doc110=admin-fuentes]').click();await f('abrir').click();
 await page.getByLabel('Permitir NIT',{exact:true}).check();await f('motivo').fill('Ampliar columnas de prueba');await f('guardar').click();
 await expect(page.getByRole('heading',{level:3})).toContainText('revisión 3');await f('cerrar').click();
 await page.locator('[data-doc110=admin-proveedores]').click();await page.locator('[data-proveedor=nuevo]').click();await page.locator('[data-proveedor=codigo]').fill('QA');await page.locator('[data-proveedor=nombre]').fill('Egresos');await page.locator('[data-proveedor=crear]').click();
 await page.locator('[data-proveedor=conexion]').selectOption('DA');await page.locator('[data-proveedor=tabla]').selectOption('imp_02_egresos');await page.locator('[data-proveedor=clave]').selectOption('CONSECUTIVO');
 await page.locator('[data-proveedor=agregar-propiedad]').click();await page.getByLabel('Propiedad',{exact:true}).fill('beneficiario');await page.getByLabel('Columna',{exact:true}).selectOption('BENEFICIARIO');await page.locator('[data-proveedor=guardar]').click();
 // Esperar la respuesta del guardado y el formulario regenerado: el aviso de
 // opciones del formulario anterior puede seguir visible mientras se guarda.
 await expect.poll(()=>page.evaluate(()=>window.integrationSnapshot().provider?.Revision)).toBe(2);
 const providerPanel=page.getByRole('dialog',{name:'Administrar proveedores'});
 await expect(providerPanel.getByRole('status').last()).toContainText('Opciones autorizadas');
 await page.locator('[data-proveedor=publicar]').click();await expect(page.locator('[data-proveedor=guardar]')).toHaveCount(0);
 await expect(providerPanel.getByRole('status').last()).toContainText('Opciones autorizadas');
 await page.locator('[data-proveedor=disponibilidad]').click();await expect(page.locator('[data-proveedor=disponibilidad]')).toHaveText('Desactivar proveedor');
 await expect(providerPanel.getByRole('status').last()).toContainText('Opciones autorizadas');await page.locator('[data-proveedor=cerrar]').click();
 // Conectar los tres editores usando una sola autoridad simulada de fuentes.
 await page.locator('[data-doc110=abrir]').click();await page.locator('[data-doc110=nuevo]').click();
 await page.locator('[data-doc110=codigo]').fill('PER-FUENTE');await page.locator('[data-doc110=nombre]').fill('Perfil de fuente autorizada');await page.locator('[data-doc110=crear]').click();
 const profilePanel=page.getByRole('dialog',{name:'Perfiles de importación',exact:true});
 await page.locator('[data-doc110-field=GabineteId]').selectOption('7');
 await expect(page.locator('[data-doc110-catalog=INDICE]')).toHaveAttribute('data-state','READY');
 await page.locator('[data-doc110=agregar-regla]').click();
 const r=id=>page.locator('[data-doc110-rule='+id+']');
 await r('destino').selectOption('INDICE_GABINETE');await r('campo').selectOption('8');await r('fuente').selectOption('PROVEEDOR');
 await r('proveedor').selectOption('2:3');await r('proveedorPropiedad').selectOption('beneficiario');await r('claveFuente').selectOption('CONSTANTE');await r('claveConstante').fill('00012');
 await page.locator('[data-doc110=guardar]').click();await expect(r('proveedor')).toHaveValue('2:3');
 await page.locator('[data-doc110=ensayar]').click();await expect(page.locator('[data-doc110=resultado-ensayo]')).toContainText('Ejemplo autorizado');
 const consumerBefore=await page.evaluate(()=>window.integrationSnapshot());
 expect(consumerBefore.profile.Version.Reglas[0].Proveedor).toMatchObject({ProveedorId:'2',VersionId:'3',Propiedad:'beneficiario'});
 await profilePanel.getByRole('button',{name:'Cerrar',exact:true}).click();
 await page.locator('[data-doc110=admin-fuentes]').click();await f('abrir').click();await f('historial').click();await expect(page.getByText('Habilitar consultas',{exact:true})).toBeVisible();await page.getByRole('button',{name:'Volver a la fuente',exact:true}).click();
 await f('motivo').fill('Revocar para prueba');await f('disponibilidad').click();await expect(page.getByRole('heading',{level:3})).toContainText('Inactiva');await f('cerrar').click();
 const consumerAfter=await page.evaluate(()=>window.integrationSnapshot());
 expect(consumerAfter.provider).toEqual(consumerBefore.provider);expect(consumerAfter.profile).toEqual(consumerBefore.profile);
 expect(consumerAfter.history.map(h=>h.Revision)).toEqual([4,3,2,1]);expect(consumerAfter.history[0].Estado.Activo).toBe(false);
 await page.locator('[data-doc110=abrir]').click();await profilePanel.getByRole('button',{name:'Abrir',exact:true}).click();
 await expect(r('proveedor')).toHaveValue('2:3');
 await page.locator('[data-doc110=ensayar]').click();await expect(page.locator('[data-doc110=estado]')).toContainText('Fuente revocada');
 expect((await page.evaluate(()=>window.integrationSnapshot())).profile).toEqual(consumerBefore.profile);
 await profilePanel.getByRole('button',{name:'Cerrar',exact:true}).click();
 // Reabrir la publicación guardada con fuente revocada conserva sus referencias.
 await page.locator('[data-doc110=admin-proveedores]').click();await page.locator('[data-proveedor=abrir]').click();
 await expect(providerPanel.getByText('Referencias guardadas no disponibles.',{exact:false})).toBeVisible();
 await expect(page.locator('[data-proveedor=conexion]')).toHaveValue('DA');
 await expect(page.locator('[data-proveedor=tabla]')).toHaveValue('imp_02_egresos');
 await expect(page.locator('[data-proveedor=clave]')).toHaveValue('CONSECUTIVO');
 await expect(providerPanel.getByLabel('Columna',{exact:true})).toHaveValue('BENEFICIARIO');
 await expect(page.locator('[data-proveedor=guardar]')).toHaveCount(0);
 expect((await page.evaluate(()=>window.integrationSnapshot())).provider).toEqual(consumerBefore.provider);
 await page.locator('[data-proveedor=cerrar]').click();
 await page.locator('[data-doc110=admin-proveedores]').click();await page.locator('[data-proveedor=nuevo]').click();await page.locator('[data-proveedor=codigo]').fill('QB');await page.locator('[data-proveedor=nombre]').fill('Sin fuente');await page.locator('[data-proveedor=crear]').click();
 await expect(page.getByRole('dialog',{name:'Administrar proveedores'}).getByRole('status').last()).toContainText('Sin opciones');
 expect(await page.evaluate(()=>window.sourceCalls.filter(x=>x==='RevocarFuente').length)).toBe(1);
});

test('AMP-02: error al crear visible junto al desplazamiento, conserva formulario y no reenvia',async({page})=>{
 await page.setViewportSize({width:1000,height:600});
 await page.setContent('<div id="doc110-perfiles" data-fuentes="1"></div>');
 for(const file of ['js/ui/jquery-3.4.1.min.js','js/gestion/ImportacionCarpetas/PerfilImportacionApi.js','js/gestion/ImportacionCarpetas/FuenteAdminEditor.js'])await page.addScriptTag({path:path.join(root,file)});
 await page.addStyleTag({path:path.join(root,'js/gestion/ImportacionCarpetas/PerfilImportacionEditor.css')});
 await page.evaluate(()=>{
  window.writes=0;
  jQuery.ajax=function(options){const d=jQuery.Deferred(),s=JSON.parse(options.data).solicitud,op=options.url.split('/').pop();setTimeout(()=>{
   if(op==='CrearFuente'){window.writes++;d.reject({status:503,responseJSON:{d:{ok:false,codigo:'ESQUEMA_FUENTES_INCOMPLETO',mensaje:'No se guardó la fuente. Falta completar la tabla de historial del SQL 07 en RA.',datos:null,incidencias:[]}}},'error');return;}
   const data=op==='ListarFuentes'?[]:s.tipo==='CONEXIONES'?[{value:'DA'}]:s.tipo==='TABLAS'?[{value:'imp_02_egresos'}]:['CODIGOTIPO','TIPO','CONSECUTIVO','FECHA','NIT','BENEFICIARIO'].map(value=>({value,Tipo:'TEXTO'}));
   d.resolve({d:{ok:true,codigo:'OK',mensaje:'Listo',datos:data,incidencias:[]}});
  },10);return d.promise();};
  window.FuenteAdminEditor.inicializar(document.getElementById('doc110-perfiles'),window.PerfilImportacionApi.crear({url:'/service',token:'synthetic'}));
 });
 const f=id=>page.locator('[data-fuente='+id+']');
 await page.locator('[data-doc110=admin-fuentes]').click();await f('nueva').click();await f('conexion').selectOption('DA');await f('tabla').selectOption('imp_02_egresos');
 for(const name of ['CODIGOTIPO','TIPO','CONSECUTIVO','FECHA','NIT','BENEFICIARIO'])await page.getByLabel('Permitir '+name,{exact:true}).check();
 await f('motivo').fill('Fuente egresos');await f('guardar').click();
 await expect(f('resultado')).toContainText('No se guardó');await expect(f('resultado')).toContainText('SQL 07');
 await f('guardar').scrollIntoViewIfNeeded();const box=await f('resultado').boundingBox();expect(box.y).toBeGreaterThanOrEqual(0);expect(box.y+box.height).toBeLessThan(600);
 await expect(f('motivo')).toHaveValue('Fuente egresos');await expect(f('guardar')).toBeEnabled();expect(await page.evaluate(()=>window.writes)).toBe(1);
 await expect(f('resultado')).not.toContainText('no se pudo confirmar');
});
