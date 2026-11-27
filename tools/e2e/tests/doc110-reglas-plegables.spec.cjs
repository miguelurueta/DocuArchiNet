'use strict';
const {test,expect}=require('@playwright/test');
const path=require('node:path');
const root=path.resolve(__dirname,'../../..');

// Componentes reales y transporte simulado. Cero autenticación, BD, cargas o HTTP.
async function setup(page,state='BORRADOR'){
 const errors=[];page.on('pageerror',error=>errors.push(error.message));
 await page.route('**/*',route=>route.abort());
 await page.setContent('<div id="doc110-perfiles" data-api="/webservice/WebServiceImportacionCarpetas.asmx" data-token="synthetic" data-nivel="7"></div>');
 for(const file of ['js/ui/jquery-3.4.1.min.js','js/ui/jquery-ui-1.8.22.custom.js'])await page.addScriptTag({path:path.join(root,file)});
 await page.addStyleTag({path:path.join(root,'js/gestion/ImportacionCarpetas/PerfilImportacionEditor.css')});
 await page.evaluate(state=>{
  const fields=['NOMBRETITULAR','IDENTIFICACION','FECHARECEPCION','FECHAENTREGA','VIN','PLACA','OBSERVACION','REFERENCIA','RESPONSABLE','CONSECUTIVO'];
  let profile={PerfilId:'1',Codigo:'SINTETICO',Nombre:'Perfil de prueba',Revision:1,Activo:true,Version:{VersionId:'2',PerfilId:'1',Numero:1,Revision:1,Estado:state,Configuracion:{NombreVisible:'Prueba local',GabineteId:7,OrigenGabinete:'DA',ModoIntegridad:'COMPLETO_OBLIGATORIO',AmbitoIntegridad:'EXPEDIENTE',PoliticaCoincidencias:'PERMITIR',Extensiones:['pdf']},Reglas:fields.map((field,i)=>({
   ReglaId:String(i+1),DestinoTipo:'INDICE_GABINETE',DestinoRef:String(81+i),FuenteTipo:'PROVEEDOR',EtapaResolucion:'PREFLIGHT',TipoValor:'TEXTO',LongitudMaxima:120,Extraccion:'COMPLETO',Obligatoria:true,
   Proveedor:{ProveedorId:'1',VersionId:'3',Propiedad:field,Clave:{FuenteTipo:i===9?'CARPETA':'ARCHIVO',Extraccion:i===9?'COMPLETO':'SEGMENTO',Separador:i===9?null:'-',Posicion:i===9?0:2,TipoValor:'TEXTO',LongitudMaxima:i===9?20:32,QuitarExtension:i!==9,RecortarEspacios:true}}
  }))}};
  window.collapseCalls=[];
  window.PerfilImportacionApi={crear(){return{ejecutar(operation,payload){
   window.collapseCalls.push({operation,payload});const result=jQuery.Deferred();
   setTimeout(()=>{
    let data;
    if(operation==='ListarPerfiles')data={Elementos:[profile],Total:1};
    else if(operation==='ObtenerPerfil')data=profile;
    else if(operation==='ConsultarCatalogo')data=payload.catalogo==='INDICE'?fields.map((text,i)=>({value:String(81+i),text,Tipo:'TEXTO',Longitud:120})):payload.catalogo==='GABINETE'?[{value:'7',text:'Gabinete sintético'}]:[];
    else if(operation==='ConsultarProveedores')data=[{ProveedorId:'1',VersionId:'3',Nombre:'Proveedor sintético',Numero:1,TipoClave:'TEXTO',Propiedades:fields.map(Nombre=>({Nombre,Tipo:'TEXTO'}))}];
    else if(operation==='EnsayarReglas')data=[{Valores:[],Incidencias:[]}];
    else if(operation==='GuardarBorrador')data=profile={...profile,Version:{...profile.Version,Revision:profile.Version.Revision+1,Configuracion:payload.configuracion,Reglas:payload.reglas.map((rule,i)=>({...rule,ReglaId:String(201+i)}))}};
    else if(operation==='PublicarVersion')data=profile={...profile,Version:{...profile.Version,Estado:'PUBLICADA',Revision:profile.Version.Revision+1}};
    else throw new Error('Unexpected operation '+operation);
    result.resolve({ok:true,codigo:'OK',mensaje:'Listo',datos:data,incidencias:[]});
   },10);return result.promise();
  }};}};
 },state);
 await page.addScriptTag({path:path.join(root,'js/gestion/ImportacionCarpetas/PerfilImportacionEditor.js')});
 await page.locator('[data-doc110=abrir]').click();await page.getByRole('button',{name:'Abrir',exact:true}).click();
 await expect(page.locator('[data-doc110=regla]')).toHaveCount(10);
 await expect(page.locator('[data-doc110-catalog=INDICE]')).toHaveAttribute('data-state','READY');
 await expect(page.locator('[data-doc110=resumen-regla]').first()).toContainText('Proveedor sintético');
 return errors;
}

for(const viewport of [{width:1366,height:900},{width:390,height:844}]){
 test('Reglas plegables: teclado, barra visible y contratos conservados '+viewport.width,async({page},testInfo)=>{
  await page.setViewportSize(viewport);const errors=await setup(page);
  const panels=page.locator('[data-doc110=regla]'),add=page.locator('[data-doc110=agregar-regla]');
  await expect(page.locator('[data-doc110=regla][open]')).toHaveCount(0);
  await page.locator('[data-doc110=ensayar]').click();await expect(page.locator('[data-doc110=estado]')).toContainText('Prueba sin incidencias');
  const baseline=await page.evaluate(()=>window.collapseCalls.find(c=>c.operation==='EnsayarReglas').payload.reglas);
  const calls=await page.evaluate(()=>window.collapseCalls.length);
  const summary=panels.first().locator('summary');await summary.focus();await page.keyboard.press('Enter');
  await expect(panels.first()).toHaveAttribute('open','');
  await expect(panels.first().locator('[data-doc110-rule=claveSeparador]')).toHaveValue('-');
  await summary.focus();await page.keyboard.press('Space');await expect(panels.first()).not.toHaveAttribute('open','');
  expect(await page.evaluate(()=>window.collapseCalls.length)).toBe(calls);
  await expect(page.locator('[data-doc110=publicar]')).toBeEnabled();
  await summary.scrollIntoViewIfNeeded();await page.screenshot({path:testInfo.outputPath('reglas-plegadas.png')});
  await summary.click();
  await page.locator('.doc110-body').evaluate(el=>{el.scrollTop=el.scrollHeight;});
  await expect(add).toBeInViewport();
  const body=await page.locator('.doc110-body').boundingBox(),bar=await page.locator('[data-doc110=barra-reglas]').boundingBox();
  expect(bar.y+bar.height).toBeLessThanOrEqual(body.y+1);
  await add.click();await expect(panels).toHaveCount(11);await expect(panels.last()).toHaveAttribute('open','');
  await expect(panels.last().locator('[data-doc110-rule=destino]')).toBeFocused();await expect(add).toBeInViewport();
  await expect(page.locator('[data-doc110=cantidad-reglas]')).toHaveText('11 reglas');
  await page.screenshot({path:testInfo.outputPath('regla-nueva-abierta.png')});
  await panels.first().getByRole('button',{name:'Quitar',exact:true}).click();await expect(panels).toHaveCount(10);
  await expect(panels.first().locator('summary')).toContainText('Regla 1 · IDENTIFICACION');
  const overflow=await page.locator('.doc110-body').evaluate(el=>el.scrollWidth>el.clientWidth+1);expect(overflow).toBe(false);
  await page.locator('[data-doc110=guardar]').click();await expect(page.locator('[data-doc110=cambios-pendientes]')).toBeEmpty();
  const saved=await page.evaluate(()=>window.collapseCalls.filter(c=>c.operation==='GuardarBorrador').at(-1).payload.reglas);
  expect(saved.slice(0,-1)).toEqual(baseline.slice(1).map((rule,Orden)=>({...rule,Orden})));
  await expect(panels.last()).toHaveAttribute('open','');await expect(page.locator('[data-doc110=publicar]')).toBeEnabled();
  await page.locator('[data-doc110=publicar]').click();await expect(add).toBeDisabled();
  await expect(page.locator('[data-doc110=guardar]')).toHaveCount(0);expect(errors).toEqual([]);
 });
}

test('Reglas publicadas: se pueden desplegar y navegar con teclado sin habilitar edición',async({page})=>{
 const errors=await setup(page,'PUBLICADA');const add=page.locator('[data-doc110=agregar-regla]'),panel=page.locator('[data-doc110=regla]').first();
 await expect(add).toBeDisabled();await panel.locator('summary').focus();await page.keyboard.press('Enter');
 await expect(panel).toHaveAttribute('open','');await expect(panel.locator('[data-doc110-rule=claveLongitud]')).toBeDisabled();
 await expect(panel.getByRole('button',{name:'Quitar',exact:true})).toBeDisabled();
 await page.keyboard.press('Tab');expect(await page.evaluate(()=>document.activeElement.closest('[role=dialog]')!==null)).toBe(true);
 expect(await page.evaluate(()=>window.collapseCalls.some(c=>['GuardarBorrador','PublicarVersion','CrearVersion'].includes(c.operation)))).toBe(false);
 expect(errors).toEqual([]);
});
