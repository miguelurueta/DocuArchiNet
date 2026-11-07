const test=require('node:test');
const assert=require('node:assert/strict');
const fs=require('node:fs');
const {JSDOM}=require('../tools/opsxj/node_modules/jsdom');
function setup(allowed='1'){
 const dom=new JSDOM('<div id="host" data-proveedores="'+allowed+'"></div>',{runScripts:'outside-only'}),w=dom.window;
 w.eval(fs.readFileSync('js/ui/jquery-3.4.1.min.js','utf8'));w.confirm=()=>true;
 const calls=[],host=w.document.getElementById('host');
 w.eval(fs.readFileSync('js/gestion/ImportacionCarpetas/ProveedorAdminEditor.js','utf8'));
 w.ProveedorAdminEditor.inicializar(host,{ejecutar(op,data){const d=w.jQuery.Deferred();calls.push({op,data,d});return d.promise();}});
 return {dom,w,host,calls,click(id){host.querySelector('[data-proveedor="'+id+'"]').click();},reply(data,call=calls.at(-1)){call.d.resolve({mensaje:'Listo',datos:data});},open(){host.querySelector('button').click();this.reply([]);this.click('nuevo');host.querySelector('[data-proveedor=codigo]').value='QA';host.querySelector('[data-proveedor=nombre]').value='Prueba';this.click('crear');this.reply(doc());}};
}
function doc(state='BORRADOR'){return {ProveedorId:'1',VersionId:'2',Revision:1,RevisionProveedor:1,Codigo:'QA',Nombre:'Prueba',Numero:1,Estado:state,Activo:false,Contrato:{ConexionRef:'',Tabla:'',ColumnaClave:'',ColumnaEmpresa:'',TipoClave:'TEXTO',Propiedades:[]}};}

function savedProvider(s){
 const saved=doc('PUBLICADA');saved.Contrato={ConexionRef:'DA',Tabla:'imp_02_creditos',ColumnaClave:'CONSECUTIVO',ColumnaEmpresa:'',TipoClave:'TEXTO',Propiedades:[{Nombre:'fecha',Columna:'FECHADOCUMENTO',Tipo:'FECHA'}]};
 s.host.querySelector('button').click();s.reply([saved]);s.click('abrir');s.reply(saved);return saved;
}
test('Proveedor: fuente guardada retirada conserva referencias y diagnostica sin consultar otra fuente',()=>{
 for(const level of ['CONEXIONES','TABLAS']){
  const s=setup();try{
   const saved=savedProvider(s),before=JSON.stringify(saved);
   if(level==='CONEXIONES')s.reply([{value:'RA'}]);
   else{s.reply([{value:'DA'}]);s.reply([{value:'imp_02_egresos'}]);}
   const panel=s.host.querySelector('[role=dialog]');
   assert.match(panel.textContent,/guardad[ao]s? no disponibles/);
   assert.equal(panel.querySelector('[data-proveedor=conexion]').value,'DA');
   assert.equal(panel.querySelector('[data-proveedor=tabla]').value,'imp_02_creditos');
   assert.equal(panel.querySelector('[data-proveedor=clave]').value,'CONSECUTIVO');
   assert.equal(panel.querySelector('[data-proveedor=empresa]').value,'');
   assert.equal(panel.querySelector('select[aria-label=Columna]').value,'FECHADOCUMENTO');
   assert.equal(panel.querySelector('[data-proveedor=guardar]'),null);
   assert.equal(s.calls.filter(c=>c.op==='ConsultarMetadatosProveedor').length,level==='CONEXIONES'?1:2);
   assert.equal(JSON.stringify(saved),before);
  }finally{s.dom.window.close();}
 }
});
test('Proveedor: columna revocada se conserva con diagnostico y no se sustituye por otra permitida',()=>{
 const s=setup();try{
  const saved=savedProvider(s),before=JSON.stringify(saved);
  s.reply([{value:'DA'}]);s.reply([{value:'imp_02_creditos'}]);s.reply([{value:'CONSECUTIVO',Tipo:'TEXTO'}]);
  assert.match(s.host.textContent,/guardad[ao]s? no disponibles/);
  assert.equal(s.host.querySelector('select[aria-label=Columna]').value,'FECHADOCUMENTO');
  assert.equal(s.host.querySelector('select[aria-label=Columna]').closest('tr').querySelector('input[readonly]').value,'FECHA');
  assert.equal(s.calls.some(c=>['GuardarProveedor','PublicarProveedor'].includes(c.op)),false);
  assert.equal(JSON.stringify(saved),before);
 }finally{s.dom.window.close();}
});
test('Proveedor: permiso adicional requerido para mostrar acceso',()=>{const s=setup('0');assert.equal(s.host.children.length,0);s.dom.window.close();});
test('Proveedor: alta guiada, escritura bloqueada y publicación de configuración guardada',()=>{
 const s=setup();try{
 s.open();s.reply([{value:'RA'}]);
 const connection=s.host.querySelector('[data-proveedor=conexion]');connection.value='RA';connection.dispatchEvent(new s.w.Event('change'));s.reply([{value:'ordenes'}]);
 const table=s.host.querySelector('[data-proveedor=tabla]');table.value='ordenes';table.dispatchEvent(new s.w.Event('change'));s.reply([{value:'codigo',Tipo:'TEXTO'},{value:'empresa',Tipo:'ENTERO'}]);
 const key=s.host.querySelector('[data-proveedor=clave]');key.value='codigo';key.dispatchEvent(new s.w.Event('change'));
 s.click('publicar');assert.equal(s.calls.at(-1).op,'ConsultarMetadatosProveedor');
 s.click('guardar');assert.equal(s.calls.at(-1).op,'GuardarProveedor');assert.equal(key.disabled,true);assert.equal(s.calls.at(-1).data.Contrato.Tabla,'ordenes');
 const saved=doc();saved.Contrato=JSON.parse(JSON.stringify(s.calls.at(-1).data.Contrato));s.reply(saved);
 s.reply([{value:'RA'}]);s.reply([{value:'ordenes'}]);s.reply([{value:'codigo',Tipo:'TEXTO'},{value:'empresa',Tipo:'ENTERO'}]);
 s.click('publicar');assert.equal(s.calls.at(-1).op,'PublicarProveedor');saved.Estado='PUBLICADA';s.reply(saved);
 assert.equal(s.host.querySelector('fieldset').disabled,true);assert.equal(s.host.querySelector('[data-proveedor=guardar]'),null);assert.ok(s.host.querySelector('[data-proveedor=version]'));
 }finally{s.dom.window.close();}
});
test('Proveedor: descarta metadatos atrasados incluso al volver a la misma conexión',()=>{
 const s=setup();try{s.open();s.reply([{value:'RA'},{value:'DA'}]);const c=s.host.querySelector('[data-proveedor=conexion]');
 function change(v){c.value=v;c.dispatchEvent(new s.w.Event('change'));return s.calls.at(-1);}
 const first=change('RA');change('DA');const last=change('RA');s.reply([{value:'actual'}],last);s.reply([{value:'obsoleta'}],first);
 const table=s.host.querySelector('[data-proveedor=tabla]');assert.match(table.textContent,/actual/);assert.doesNotMatch(table.textContent,/obsoleta/);
 }finally{s.dom.window.close();}
});
test('Proveedor: fallo incierto no reenvía automáticamente la escritura',()=>{
 const s=setup();try{s.open();s.reply([]);s.click('guardar');const count=s.calls.length;s.calls.at(-1).d.reject({mensaje:'Tiempo agotado',resultadoIncierto:true});s.click('guardar');assert.equal(s.calls.length,count);assert.equal(s.host.querySelector('[data-proveedor=estado]').disabled,false);}finally{s.dom.window.close();}
});
