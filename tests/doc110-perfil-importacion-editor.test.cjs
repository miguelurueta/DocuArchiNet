const assert=require('node:assert/strict');
const test=require('node:test');
const fs=require('node:fs');
const path=require('node:path');
const {JSDOM}=require('../tools/opsxj/node_modules/jsdom');
const root=path.resolve(__dirname,'..');
function setup() {
  const dom=new JSDOM('<div id="doc110-perfiles" data-api="/webservice/WebServiceImportacionCarpetas.asmx" data-token="synthetic" data-nivel="7"></div>',{url:'https://test.invalid/',runScripts:'outside-only'});
  const w=dom.window;w.eval(fs.readFileSync(path.join(root,'js/ui/jquery-3.4.1.min.js'),'utf8'));
  w.eval(fs.readFileSync(path.join(root,'js/ui/jquery-ui-1.8.22.custom.js'),'utf8'));
  const calls=[];
  w.PerfilImportacionApi={crear(){return {ejecutar(operation,payload){const result=w.jQuery.Deferred();calls.push({operation,payload,result});return result.promise();}};}};
  w.eval(fs.readFileSync(path.join(root,'js/gestion/ImportacionCarpetas/PerfilImportacionEditor.js'),'utf8'));
  w.PerfilImportacionEditor.inicializar();
  return {dom,w,calls,click(selector){w.document.querySelector(selector).click();},
    respond(data){calls.at(-1).result.resolve({ok:true,codigo:'OK',mensaje:'Listo',datos:data,incidencias:[]});}};
}
function profile(state='BORRADOR') {return {PerfilId:'1',Codigo:'QA',Nombre:'<img src=x onerror=alert(1)>',Revision:1,Activo:true,
  Version:{VersionId:'2',Numero:1,Revision:1,Estado:state,Configuracion:{NombreVisible:'Synthetic',ModoIntegridad:'COMPLETO_OBLIGATORIO',PoliticaCoincidencias:'BLOQUEAR',Extensiones:[],OrigenGabinete:'DA'},Reglas:[]}};}
function open(s,p) {
  s.click('[data-doc110=abrir]');s.respond({Elementos:[p],Total:1});
  s.w.document.querySelector('tbody button').click();s.respond(p);
}

test('DOC-110 ensayo: un error posterior no conserva resultados previos ni repite solicitudes',()=>{
  for(const code of ['SESION_REQUERIDA','ACCESO_DENEGADO','CONFLICTO_REVISION','CAPACIDAD_NO_DISPONIBLE','RESPUESTA_INVALIDA','TRANSPORTE_NO_DISPONIBLE']){
    const s=setup();try{
      const p=profile(),snapshot=JSON.stringify(p);open(s,p);
      const output=s.w.document.querySelector('[data-doc110=resultado-ensayo]');
      s.click('[data-doc110=ensayar]');
      s.respond([{Valores:[{DestinoTipo:'INDICE_GABINETE',DestinoRef:'8',Valor:'Resultado anterior'}],Incidencias:[]}]);
      assert.match(output.textContent,/Resultado anterior/);
      s.click('[data-doc110=ensayar]');
      assert.equal(output.textContent,'','los valores anteriores dejan de representar el ensayo solicitado');
      assert.equal(s.calls.at(-1).operation,'EnsayarReglas');
      s.calls.at(-1).result.reject({codigo:code,mensaje:'Ensayo rechazado: '+code,incidencias:[]});
      assert.equal(output.textContent,'');
      assert.match(s.w.document.querySelector('[data-doc110=estado]').textContent,/Ensayo rechazado/);
      assert.equal(s.w.document.querySelector('[role=dialog]').hasAttribute('aria-busy'),false);
      assert.equal(s.w.document.querySelector('[data-doc110=ensayar]').disabled,false);
      assert.equal(s.calls.filter(c=>c.operation==='EnsayarReglas').length,2);
      assert.equal(s.calls.some(c=>['GuardarBorrador','PublicarVersion'].includes(c.operation)),false);
      assert.equal(JSON.stringify(p),snapshot);
    }finally{s.dom.window.close();}
  }
});
test('DOC-110 editor: inicialización repetida, render seguro y guardado real del formulario',()=>{
  const s=setup();
  try {
    s.w.PerfilImportacionEditor.inicializar();assert.equal(s.w.document.querySelectorAll('[data-doc110=abrir]').length,1);
    const p=profile();open(s,p);
    assert.equal(s.w.document.querySelectorAll('img').length,0);
    const scope=s.w.document.querySelector('[data-doc110-field=AmbitoIntegridad]');scope.value='LOTE';
    s.click('[data-doc110=agregar-regla]');
    s.click('[data-doc110=guardar]');
    assert.equal(s.calls.at(-1).operation,'GuardarBorrador');
    assert.equal(s.calls.at(-1).payload.configuracion.AmbitoIntegridad,'LOTE');
    assert.equal(s.calls.at(-1).payload.reglas[0].DestinoRef,'CODIGO');
    assert.equal(s.calls.at(-1).payload.reglas[0].FuenteTipo,'CARPETA');
    s.calls.at(-1).result.reject({codigo:'CONFLICTO_REVISION',mensaje:'Conflicto',incidencias:[]});
    assert.equal(s.w.document.querySelector('[role=dialog]').hasAttribute('aria-busy'),false);
    assert.match(s.w.document.querySelector('[role=status]').textContent,/Conflicto/);
  } finally{s.dom.window.close();}
});
test('DOC-110 editor: publicación inmutable y nivel vigente conservado',()=>{
  const s=setup();
  try {
    s.w.document.getElementById('doc110-perfiles').setAttribute('data-nivel','9');
    open(s,profile('PUBLICADA'));
    assert.equal(s.w.document.querySelector('[data-doc110=guardar]'),null);
    assert.equal(s.w.document.querySelector('fieldset').disabled,true);
    s.click('[data-doc110=seleccionar]');
    assert.equal(s.calls.at(-1).payload.nivelRef,'9');
    assert.equal(s.calls.at(-1).payload.proposito,'ADMINISTRACION');
    assert.equal(s.calls.filter(x=>x.operation==='CrearVersion').length,0);
  } finally{s.dom.window.close();}
});

test('COR-03: widget existente consulta, descarta respuestas antiguas y selecciona código sin HTML',async()=>{
  const s=setup();
  try{
    s.click('[data-doc110=abrir]');s.respond({Elementos:[],Total:0});
    const field=s.w.document.querySelector('[data-doc110=buscar-texto]');
    const widget=s.w.jQuery(field).data('autocomplete');
    assert.ok(widget);assert.equal(widget.options.delay,300);
    field.value='an';s.w.jQuery(field).autocomplete('search');const old=s.calls.at(-1);
    field.value='nu';field.dispatchEvent(new s.w.Event('input'));s.w.jQuery(field).autocomplete('search');
    assert.equal(s.calls.at(-1).payload.termino,'nu');assert.equal(s.calls.at(-1).payload.tamanoPagina,10);
    s.respond({Elementos:[{PerfilId:'8',Codigo:'NUEVO',Nombre:'<img src=x>'}],Total:1});
    assert.equal(widget.menu.element.find('li').length,1);
    assert.equal(widget.menu.element.find('img').length,0);
    old.result.resolve({datos:{Elementos:[{PerfilId:'9',Codigo:'ANTIGUO',Nombre:'Anterior'}]}});
    assert.match(widget.menu.element.text(),/NUEVO/);
    widget.options.select(null,{item:{value:'NUEVO',perfilId:'8'}});
    await new Promise(r=>s.w.setTimeout(r,5));
    assert.equal(s.calls.at(-1).payload.codigo,'NUEVO');assert.equal(s.calls.at(-1).payload.termino,null);
    s.respond({Elementos:[],Total:0});
    assert.equal(s.w.document.querySelectorAll('.ui-autocomplete').length,1);
    s.click('[data-doc110=limpiar-busqueda]');assert.equal(s.calls.at(-1).payload.codigo,null);
    assert.equal(s.calls.at(-1).payload.termino,null);s.respond({Elementos:[],Total:0});
    const next=s.w.document.querySelector('[data-doc110=buscar-texto]');next.value='sin coincidencias';
    s.w.jQuery(next).autocomplete('search');s.respond({Elementos:[],Total:0});
    assert.match(s.w.document.querySelector('[role=status]').textContent,/No hay perfiles coincidentes/);
    s.click('[data-doc110=nuevo]');assert.equal(s.w.document.querySelectorAll('.ui-autocomplete').length,0);
  }finally{s.dom.window.close();}
});

function field(s,key){return s.w.document.querySelector('[data-doc110-field="'+key+'"]');}
function change(s,el,value){el.value=value;el.dispatchEvent(new s.w.Event('change',{bubbles:true}));}
function catalogCall(s,name,extra={}){return s.calls.findLast(c=>c.operation==='ConsultarCatalogo' && c.payload.catalogo===name && Object.keys(extra).every(k=>c.payload[k]===extra[k]));}
function resolveCatalog(call,items){call.result.resolve({datos:items,mensaje:'Listo'});}
function options(n){return Array.from({length:n},(_,i)=>({value:String(i+1),text:'Opción '+(i+1)}));}

test('COR-04: carga inicial, dependencias, página reiniciada y respuesta antigua descartada',()=>{
  const s=setup();try{
    open(s,profile());
    assert.equal(s.calls.filter(c=>c.operation==='ConsultarCatalogo').length,5);
    assert.equal(field(s,'AreaRef').disabled,true);
    assert.equal(s.w.document.body.textContent.includes('Cargar opciones'),false);
    resolveCatalog(catalogCall(s,'INSTRUMENTO'),options(2));
    change(s,field(s,'InstrumentoRef'),'1');const old=catalogCall(s,'AREA');
    change(s,field(s,'InstrumentoRef'),'2');const fresh=catalogCall(s,'AREA');
    resolveCatalog(fresh,options(50));s.click('[data-doc110=mas-AreaRef]');
    assert.equal(catalogCall(s,'AREA').payload.pagina,2);
    change(s,field(s,'InstrumentoRef'),'1');
    assert.equal(catalogCall(s,'AREA').payload.pagina,1);
    resolveCatalog(old,[{value:'999',text:'Antiguo'}]);
    assert.equal(field(s,'AreaRef').querySelector('[value="999"]'),null);
    resolveCatalog(catalogCall(s,'AREA'),[]);
    assert.equal(field(s,'AreaRef').disabled,true);
    assert.match(s.w.document.querySelector('[data-doc110-catalog=AreaRef]').textContent,/Sin opciones/);
    assert.equal(field(s,'SerieRef').disabled,true);
  }finally{s.dom.window.close();}
});

test('COR-04: etiqueta guardada fuera de página, publicado y recuperación de catálogo',()=>{
  const s=setup();try{
    const p=profile('PUBLICADA');p.Version.Configuracion.FondoRef='90';open(s,p);
    resolveCatalog(catalogCall(s,'FONDO'),options(50));
    const lookup=catalogCall(s,'FONDO',{referencia:'90'});assert.equal(lookup.payload.tamanoPagina,1);
    resolveCatalog(lookup,[{value:'90',text:'Fondo guardado'}]);
    assert.equal(field(s,'FondoRef').selectedOptions[0].textContent,'Fondo guardado');
    assert.equal(field(s,'FondoRef').value,'90');
    assert.equal(s.w.document.querySelector('fieldset').disabled,true);
    catalogCall(s,'GABINETE').result.reject({mensaje:'Fallo'});
    const retry=s.w.document.querySelector('[data-doc110=reintentar-GabineteId]');
    assert.equal(retry.hidden,false);assert.equal(retry.closest('fieldset'),null);
    retry.click();resolveCatalog(catalogCall(s,'GABINETE'),options(1));
    assert.equal(retry.hidden,true);
    assert.equal(s.w.document.querySelector('[data-doc110=recargar]').hidden,true);
  }finally{s.dom.window.close();}
});

test('COR-04: gabinete carga índices, pagina automáticamente e invalida destinos',()=>{
  const s=setup();try{
    const p=profile();p.Version.Configuracion.GabineteId=1;
    p.Version.Reglas=[{DestinoTipo:'INDICE_GABINETE',DestinoRef:'99',FuenteTipo:'CONSTANTE',TipoValor:'TEXTO',ValorConstante:'A',Extraccion:'COMPLETO',LongitudMaxima:20}];open(s,p);
    resolveCatalog(catalogCall(s,'GABINETE'),options(2));
    resolveCatalog(catalogCall(s,'INDICE'),options(50).map(x=>({...x,Tipo:'TEXTO',Longitud:20})));
    assert.equal(catalogCall(s,'INDICE').payload.pagina,2);
    resolveCatalog(catalogCall(s,'INDICE'),[{value:'99',text:'Índice guardado',Tipo:'TEXTO',Longitud:20}]);
    const ref=s.w.document.querySelector('[data-doc110-rule=campo]');assert.equal(ref.value,'99');
    assert.equal(ref.selectedOptions[0].textContent,'Índice guardado');
    change(s,field(s,'GabineteId'),'2');assert.equal(ref.value,'');assert.equal(ref.disabled,true);
    assert.equal(catalogCall(s,'INDICE').payload.configuracion.GabineteId,2);
    resolveCatalog(catalogCall(s,'INDICE'),[{value:'2',text:'Otro',Tipo:'TEXTO',Longitud:20}]);
    assert.equal(ref.value,'');assert.equal(ref.querySelector('[value="99"]'),null);
  }finally{s.dom.window.close();}
});

test('COR-04: edición pendiente impide publicar y protege navegación/recuperación',()=>{
  const s=setup();try{
    open(s,profile());const publish=s.w.document.querySelector('[data-doc110=publicar]');
    assert.equal(publish.disabled,false);
    change(s,field(s,'NombreVisible'),'Modificado');assert.equal(publish.disabled,true);
    let confirmations=0;s.w.confirm=()=>{confirmations++;return false;};
    const count=s.calls.length;s.click('[data-doc110=historial]');assert.equal(s.calls.length,count);assert.equal(confirmations,1);
    s.click('[data-doc110=guardar]');s.calls.at(-1).result.reject({codigo:'CONFLICTO_REVISION',mensaje:'Conflicto'});
    assert.equal(s.w.document.querySelector('[data-doc110=recargar]').hidden,false);
    s.click('[data-doc110=recargar]');assert.equal(field(s,'NombreVisible').value,'Modificado');
    s.w.confirm=()=>true;s.click('[data-doc110=recargar]');assert.equal(s.calls.at(-1).operation,'ObtenerPerfil');s.respond(profile());
    assert.equal(field(s,'NombreVisible').value,'Synthetic');assert.equal(s.w.document.querySelector('[data-doc110=recargar]').hidden,true);
  }finally{s.dom.window.close();}
});

test('COR-04: controles aplicables y ámbito parcial, sin guardar automáticamente',()=>{
  const s=setup();try{
    open(s,profile());change(s,field(s,'ModoIntegridad'),'PARCIAL_PERMITIDO');
    assert.equal(field(s,'AmbitoIntegridad').value,'NO_APLICA');assert.equal(field(s,'AmbitoIntegridad').disabled,true);
    change(s,field(s,'ModoIntegridad'),'COMPLETO_OBLIGATORIO');assert.equal(field(s,'AmbitoIntegridad').disabled,false);assert.equal(field(s,'AmbitoIntegridad').value,'');
    s.click('[data-doc110=agregar-regla]');const r=key=>s.w.document.querySelector('[data-doc110-rule='+key+']');
    assert.equal(r('constante').disabled,true);assert.equal(r('separador').disabled,true);assert.equal(r('extension').disabled,true);
    change(s,r('extraccion'),'SEGMENTO');assert.equal(r('separador').disabled,false);
    change(s,r('fuente'),'CONSTANTE');assert.equal(r('constante').disabled,false);assert.equal(r('extraccion').value,'COMPLETO');assert.equal(r('extraccion').disabled,true);
    change(s,r('campo'),'FECHA_INICIAL');assert.equal(r('formato').value,'yyyy-MM-dd');assert.equal(r('formato').disabled,false);
    change(s,r('campo'),'CODIGO');assert.equal(r('formato').disabled,true);
    assert.equal(s.calls.filter(c=>c.operation==='GuardarBorrador' || c.operation==='PublicarVersion').length,0);
  }finally{s.dom.window.close();}
});

test('COR-04: cambio de destino exige fuente compatible y no conserva archivo para expediente',()=>{
  const s=setup();try{
    open(s,profile());s.click('[data-doc110=agregar-regla]');
    const r=k=>s.w.document.querySelector('[data-doc110-rule='+k+']');
    change(s,r('destino'),'TIPOLOGIA');change(s,r('fuente'),'ARCHIVO');
    assert.equal(r('extension').disabled,false);
    change(s,r('destino'),'CAMPO_EXPEDIENTE');assert.equal(r('fuente').value,'');
    assert.match(s.w.document.querySelector('[data-doc110=regla-aviso]').textContent,/Seleccione una fuente válida/);
    assert.equal(r('fuente').querySelector('[value=ARCHIVO]').disabled,true);
    change(s,r('fuente'),'CARPETA');assert.equal(r('extraccion').disabled,false);
  }finally{s.dom.window.close();}
});

test('AMP-01: proveedor guardado carga opciones seguras y conserva clave al guardar',()=>{
  const s=setup();try{
    const p=profile();p.Version.Reglas=[{DestinoTipo:'INDICE_GABINETE',DestinoRef:'99',FuenteTipo:'PROVEEDOR',TipoValor:'TEXTO',Extraccion:'COMPLETO',LongitudMaxima:100,Proveedor:{ProveedorId:'1',VersionId:'3',Propiedad:'nombre',Clave:{FuenteTipo:'ARCHIVO',Extraccion:'SEGMENTO',Separador:'-',Posicion:0,QuitarExtension:true,TipoValor:'TEXTO',LongitudMaxima:30}}}];
    open(s,p);
    const call=s.calls.find(x=>x.operation==='ConsultarProveedores');assert.ok(call);
    call.result.resolve({datos:[{ProveedorId:'1',VersionId:'3',Nombre:'<img src=x>',Numero:2,TipoClave:'TEXTO',Propiedades:[{Nombre:'nombre',Tipo:'TEXTO'}]}]});
    assert.equal(s.w.document.querySelectorAll('img').length,0);
    assert.equal(s.w.document.querySelector('[data-doc110-rule=proveedor]').value,'1:3');
    s.click('[data-doc110=guardar]');
    const rule=s.calls.at(-1).payload.reglas[0];assert.equal(rule.Proveedor.Propiedad,'nombre');assert.equal(rule.Proveedor.Clave.Separador,'-');assert.equal(rule.Proveedor.Clave.Posicion,0);assert.equal(rule.Proveedor.Clave.QuitarExtension,true);assert.equal(rule.EtapaResolucion,'PREFLIGHT');assert.equal(rule.QuitarExtension,false);
  }finally{s.dom.window.close();}
});

test('AMP-01: proveedor vacío y respuesta antigua no sustituyen selección actual',()=>{
  const s=setup();try{
    open(s,profile());s.click('[data-doc110=agregar-regla]');
    const destination=s.w.document.querySelector('[data-doc110-rule=destino]'),source=s.w.document.querySelector('[data-doc110-rule=fuente]');
    destination.value='INDICE_GABINETE';destination.dispatchEvent(new s.w.Event('change'));
    source.value='PROVEEDOR';source.dispatchEvent(new s.w.Event('change'));
    const old=s.calls.at(-1);assert.equal(old.operation,'ConsultarProveedores');
    source.value='CONSTANTE';source.dispatchEvent(new s.w.Event('change'));
    source.value='PROVEEDOR';source.dispatchEvent(new s.w.Event('change'));
    s.calls.at(-1).result.resolve({datos:[]});old.result.resolve({datos:[{ProveedorId:'8',VersionId:'9',Nombre:'Antiguo',Numero:1,Propiedades:[]}]});
    assert.match(s.w.document.body.textContent,/No hay proveedores habilitados/);assert.equal(s.w.document.querySelector('[data-doc110-rule=proveedor]').options.length,1);
    destination.value='TIPOLOGIA';destination.dispatchEvent(new s.w.Event('change'));assert.equal(source.value,'');
  }finally{s.dom.window.close();}
});
