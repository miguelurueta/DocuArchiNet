'use strict';
const {test,expect}=require('@playwright/test');
const path=require('node:path');
const {createFacturasSourceUI}=require('./support/doc110-source-transactions.cjs');
const root=path.resolve(__dirname,'../../..');

// Página y respuestas locales simuladas: nunca inicia sesión ni accede a DA/RA.
for(const available of [true,false])test('facturas UI: '+(available?'alta inactiva con tabla paginada':'sin tabla no intenta alta'),async({page})=>{
  const calls=[];
  await page.route('http://doc110.test/**',async route=>{
    const method=new URL(route.request().url()).pathname.split('/').pop();
    if(!method)return route.fulfill({contentType:'text/html',body:'<button id="doc110-navigation-toggle" aria-expanded="true">Menú</button><div id="doc110-perfiles" data-fuentes="1"></div>'});
    const data=route.request().postDataJSON().solicitud;calls.push({method,data});
    let result;
    if(method==='ListarFuentes')result=[];
    else if(method==='ConsultarMetadatosFuente'){
      if(data.tipo==='CONEXIONES')result=[{value:'DA'}];
      else if(data.tipo==='TABLAS')result=data.pagina===1?
        Array.from({length:25},(_,i)=>({value:'imp_02_otra_'+i})):
        available?[{value:'imp_02_facturas'}]:[];
      else result=['NUMERO','NIT','BENEFICIARIO','FECHA'].map(value=>({value,Tipo:'TEXTO'}));
    }else{
      expect(method).toBe('CrearFuente');
      expect(data.Cambio.Documento.Columnas).toEqual(['NIT','NUMERO']);
      expect(data.Cambio.Documento.Tabla).toBe('imp_02_facturas');
      result={...data.Cambio.Documento,FuenteId:'7',Revision:1,Activo:false};
    }
    return route.fulfill({contentType:'application/json',body:JSON.stringify({d:{ok:true,mensaje:'Listo',datos:result}})});
  });
  await page.goto('http://doc110.test/');
  await page.addScriptTag({path:path.join(root,'js/ui/jquery-3.4.1.min.js')});
  await page.addScriptTag({path:path.join(root,'js/gestion/ImportacionCarpetas/FuenteAdminEditor.js')});
  await page.evaluate(()=>{
    const api={ejecutar(method,data){
      const result=jQuery.Deferred();
      fetch('/api/'+method,{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({solicitud:data})})
        .then(r=>r.json()).then(r=>result.resolve(r.d),()=>result.reject({mensaje:'Fallo local'}));
      return result.promise();
    }};
    FuenteAdminEditor.inicializar(document.getElementById('doc110-perfiles'),api);
  });
  let attempted=0,created=0;
  if(available){
    const source=await createFacturasSourceUI(page,()=>attempted++,()=>created++);
    expect(source.Activo).toBe(false);expect(attempted).toBe(1);expect(created).toBe(1);
  }else{
    await expect(createFacturasSourceUI(page,()=>attempted++,()=>created++)).rejects.toThrow('NOT_DISCOVERED');
    expect(attempted).toBe(0);expect(created).toBe(0);
  }
  expect(calls.filter(c=>c.method==='CrearFuente').length).toBe(available?1:0);
  expect(calls.some(c=>c.method==='ActivarFuente')).toBe(false);
});
