const test=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs');
const {JSDOM}=require('../tools/opsxj/node_modules/jsdom');
function setup(permission='1'){
 const dom=new JSDOM('<div id="host" data-fuentes="'+permission+'"></div>',{runScripts:'outside-only'}),w=dom.window;w.confirm=()=>true;
 w.eval(fs.readFileSync('js/ui/jquery-3.4.1.min.js','utf8'));w.eval(fs.readFileSync('js/gestion/ImportacionCarpetas/FuenteAdminEditor.js','utf8'));
 const host=w.document.getElementById('host'),calls=[];w.FuenteAdminEditor.inicializar(host,{ejecutar(op,data){const d=w.jQuery.Deferred();calls.push({op,data,d});return d.promise();}});
 const s={dom,w,host,calls,reply(data,call=calls.at(-1)){call.d.resolve({mensaje:'Listo',datos:data});},click(id){host.querySelector('[data-fuente='+id+']').click();},open(){host.querySelector('[data-doc110=admin-fuentes]').click();s.reply([]);s.click('nueva');s.reply([{value:'RA'},{value:'DA'}]);}};return s;
}
test('Fuentes: permiso independiente y acceso no duplicado',()=>{const s=setup('0');try{assert.equal(s.host.children.length,0);}finally{s.dom.window.close();}});
test('Fuentes: respuestas antiguas RA/DA/RA no sustituyen tablas vigentes',()=>{
 const s=setup();try{s.open();const c=s.host.querySelector('[data-fuente=conexion]');function change(v){c.value=v;c.dispatchEvent(new s.w.Event('change'));return s.calls.at(-1);}
 const first=change('RA');change('DA');const last=change('RA');s.reply([{value:'actual'}],last);s.reply([{value:'vieja'}],first);assert.match(s.host.querySelector('[data-fuente=tabla]').textContent,/actual/);assert.doesNotMatch(s.host.querySelector('[data-fuente=tabla]').textContent,/vieja/);
 }finally{s.dom.window.close();}
});
test('Fuentes: cambio de tabla descarta columnas y la creacion incierta no se repite',()=>{
 const s=setup();try{s.open();const c=s.host.querySelector('[data-fuente=conexion]'),t=s.host.querySelector('[data-fuente=tabla]');c.value='DA';c.dispatchEvent(new s.w.Event('change'));s.reply([{value:'uno'},{value:'dos'}]);t.value='uno';t.dispatchEvent(new s.w.Event('change'));s.reply([{value:'codigo',Tipo:'TEXTO'}]);const box=s.host.querySelector('input[type=checkbox]');box.checked=true;box.dispatchEvent(new s.w.Event('change'));
 t.value='dos';t.dispatchEvent(new s.w.Event('change'));s.reply([{value:'numero',Tipo:'TEXTO'}]);const second=s.host.querySelector('input[type=checkbox]');assert.equal(second.checked,false);second.checked=true;second.dispatchEvent(new s.w.Event('change'));
 s.host.querySelector('[data-fuente=motivo]').value='Prueba autorizada';s.click('guardar');assert.equal(s.calls.at(-1).op,'CrearFuente');assert.deepEqual(Array.from(s.calls.at(-1).data.Cambio.Documento.Columnas),['numero']);assert.equal(c.disabled,true);
 const count=s.calls.length;s.calls.at(-1).d.reject({mensaje:'No confirmado',resultadoIncierto:true});s.click('guardar');assert.equal(s.calls.length,count);assert.equal(c.disabled,false);
 }finally{s.dom.window.close();}
});
