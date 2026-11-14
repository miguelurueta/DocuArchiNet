'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs'),path=require('node:path');
const {fixture,consult}=require('./support/doc120-importacion-browser.cjs');
const production=path.resolve(__dirname,'../js/gestion/WebFormProducionDocumental.js');
const output=path.resolve(__dirname,'../.tmp/doc120-arbol');fs.mkdirSync(output,{recursive:true});
async function tree(page,{branch=false}={}){
 await page.getByRole('button',{name:'Cerrar',exact:true}).click();
 await page.evaluate(branch=>{
  const host=document.getElementById('doc111-importacion');host.setAttribute('data-recuperacion','a'.repeat(32));
  const wrapper=document.createElement('div');wrapper.innerHTML='<div id="TreeViewArchivo"><table id="original-table"><tbody><tr><td><img src="data:image/gif;base64,R0lGODlhAQABAAD/ACwAAAAAAQABAAACADs=" alt=""></td><td></td><td><a id="TreeViewArchivot1i" title="7\\\\Origen"><img alt=""></a><a id="TreeViewArchivot1" title="7\\\\Origen">Origen</a></td></tr></tbody></table>'+(branch?'<div id="TreeViewArchivon1Nodes" style="display:none"></div>':'')+'<table><tr><td></td><td></td><td><a id="TreeViewArchivot2" title="8\\\\Origen">Origen</a></td></tr></table><div id="TreeViewArchivon2Nodes"></div></div><div id="pit"></div><input id="TextBox_busqueda_tre">';document.body.insertBefore(wrapper,host);
  window.$=value=>{const result=typeof value==='string'?Array.from(document.querySelectorAll(value)):[];result.ready=()=>{};result.on=()=>{};result.autocomplete=options=>{window.searchOptions=options;};return result;};window.alert=message=>{throw new Error(message);};
 },branch);
 await page.addScriptTag({path:production});
 await page.evaluate(()=>{SELECCION_TREVIEW_ID='TreeViewArchivot2';ITEMS_DATOS=[{id:'TreeViewArchivot1',text:'Origen',value:'Origen'},{id:'TreeViewArchivot2',text:'Origen',value:'Origen'}];window.clicks=[];window.OnTreeNodeClicked=id=>clicks.push(id);window.OnSearchClick=id=>clicks.push(id);
  const original=XMLHttpRequest.prototype.send;XMLHttpRequest.prototype.send=function(body){const request=JSON.parse(body).solicitud,op=this.operation;if(op==='ConsultarLote'||op==='ConsultarRecursosLote'){const callback=this.onload;this.onload=()=>{const response=JSON.parse(this.responseText),data=response.datos;if(op==='ConsultarLote')Object.assign(data,{estado:'COMPLETADO',revision:9,accionesPermitidas:['CONSULTAR','CONSULTAR_RAICES','EXPORTAR_PAGINA']});else if(request.tipo==='RAICES')for(const r of data.elementos)Object.assign(r,{estado:'COMPLETADA',expedienteId:'60',relacionNivelId:'70',nombreExpediente:'Egreso confirmado <b>texto</b>',tieneIntentoActivo:false});this.responseText=JSON.stringify(response);callback();};}original.call(this,body);};
 });
 await page.locator('[data-doc111-open]').click();await page.getByLabel('Perfil publicado').waitFor();
}
const context={nivelRef:'7',referenciaContexto:'a'.repeat(32)},row={estado:'COMPLETADA',expedienteId:'60',relacionNivelId:'70',nombreExpediente:'Egreso confirmado <b>texto</b>',tieneIntentoActivo:false};
test('DOC120-TREE01: cierre confirmado agrega el primer expediente, expansor y búsqueda sin repetir importación',()=>fixture(async page=>{
 await tree(page);await consult(page,'39');
 const link=page.locator('#TreeViewArchivon1Nodes a[title="70|7|60|"]');assert.equal(await link.count(),1);assert.equal(await link.textContent(),row.nombreExpediente);assert.equal(await page.locator('#TreeViewArchivon1Nodes b').count(),0);
 assert.equal(await page.evaluate(()=>SELECCION_TREVIEW_ID),'TreeViewArchivot2');assert.equal(await page.locator('#TreeViewArchivon2Nodes a').count(),0);
 assert.equal(await page.evaluate(()=>ITEMS_DATOS.filter(i=>i.text.startsWith('Egreso confirmado')).length),1);assert.equal(await page.evaluate(()=>EXPDIENTE_JERARQUIA),1);assert.match(await page.locator('#pit').textContent(),/Expedientes 1/);
 const expander=page.locator('#TreeViewArchivon1');await expander.waitFor({state:'attached'});assert.match(await expander.getAttribute('href'),/TreeView_ToggleNode/);
 const node=await link.getAttribute('id');await link.evaluate(el=>el.click());await page.evaluate(id=>searchOptions.select(null,{item:{id}}),node);assert.deepEqual(await page.evaluate(()=>clicks),[node.replace('TreeViewArchivot',''),node]);
 await page.getByRole('button',{name:'Actualizar estado',exact:true}).click();await page.waitForFunction(()=>document.querySelector('.doc111-feedback')?.textContent.includes('consulta 2'));assert.equal(await link.count(),1);assert.equal(await page.evaluate(()=>EXPDIENTE_JERARQUIA),1);
 assert.equal(await page.locator('[data-doc111-action="fase"]').count(),0);assert.equal(await page.evaluate(()=>calls.some(c=>['AvanzarLote','ConfirmarPlan','PrepararPlan','RecibirArchivo'].includes(c.operation))),false);
 if(process.platform==='win32'){
  const native=require('node:child_process').execFileSync('powershell.exe',['-NoProfile','-Command',"$assembly=[Reflection.Assembly]::Load('System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'); $reader=New-Object IO.StreamReader($assembly.GetManifestResourceStream('TreeView.js')); try { [Console]::Write($reader.ReadToEnd()) } finally { $reader.Dispose() }"],{encoding:'utf8',windowsHide:true});
  await page.addScriptTag({content:native});await page.evaluate(()=>{TreeViewArchivo_Data={images:Array(19).fill('data:image/gif;base64,R0lGODlhAQABAAD/ACwAAAAAAQABAAACADs='),expandToolTip:'Expandir {0}',collapseToolTip:'Contraer {0}',expandState:{value:'eee'}};});
  await page.getByRole('button',{name:'Cerrar',exact:true}).click();await expander.click();await page.waitForFunction(()=>document.getElementById('TreeViewArchivon1Nodes').style.display==='none');await expander.click();await page.waitForFunction(()=>document.getElementById('TreeViewArchivon1Nodes').style.display==='block');assert.equal(await page.evaluate(()=>SELECCION_TREVIEW_ID),'TreeViewArchivot2');
 }
 await page.screenshot({path:path.join(output,'primer-expediente.png')});
}));
test('DOC120-TREE02: rama existente contraída conserva expansión y selección al actualizar',()=>fixture(async page=>{
 await tree(page,{branch:true});await consult(page,'39');assert.equal(await page.locator('#TreeViewArchivon1Nodes a[title="70|7|60|"]').count(),1);assert.equal(await page.locator('#TreeViewArchivon1Nodes').evaluate(el=>el.style.display),'none');assert.equal(await page.evaluate(()=>SELECCION_TREVIEW_ID),'TreeViewArchivot2');
}));
test('DOC120-TREE03: rama no visible avisa; Actualizar recupera solo la vista cuando vuelve a estar visible',()=>fixture(async page=>{
 await tree(page);await page.locator('#original-table').evaluate(el=>el.style.display='none');await consult(page,'39');await page.getByRole('alert').filter({hasText:'no se pudo mostrar en el árbol'}).waitFor();assert.equal(await page.locator('#TreeViewArchivon1Nodes').count(),0);
 await page.locator('#original-table').evaluate(el=>el.style.display='');await page.getByRole('button',{name:'Actualizar estado',exact:true}).click();await page.waitForFunction(()=>document.querySelector('#TreeViewArchivon1Nodes a[title="70|7|60|"]'));assert.equal(await page.getByRole('alert').filter({hasText:'no se pudo mostrar en el árbol'}).count(),0);assert.equal(await page.evaluate(()=>calls.filter(c=>c.operation==='AvanzarLote').length),0);
}));
test('DOC120-TREE04: destino ambiguo/ajeno y resultados inciertos no insertan nodos',()=>fixture(async page=>{
 await tree(page);const result=await page.evaluate(({context,row})=>{
  const wrong=Doc111ActualizarRama({...context,referenciaContexto:'b'.repeat(32)},[row]);
  const duplicate=document.getElementById('original-table').cloneNode(true);duplicate.id='duplicate';duplicate.querySelector('#TreeViewArchivot1').id='TreeViewArchivot3';document.getElementById('TreeViewArchivo').appendChild(duplicate);const ambiguous=Doc111ActualizarRama(context,[row]);duplicate.remove();
  for(const r of [{...row,estado:'INCIERTO'},{...row,tieneIntentoActivo:true},{...row,expedienteId:null}])Doc111ActualizarRama(context,[r]);
  return{wrong,ambiguous,nodes:document.querySelectorAll('#TreeViewArchivo a[title="70|7|60|"]').length};
 },{context,row});assert.deepEqual(result,{wrong:false,ambiguous:false,nodes:0});
}));
test('DOC120-TREE05: insertor sin nodo no se anuncia como actualización exitosa',()=>fixture(async page=>{
 await tree(page);await page.evaluate(()=>create_trenode_treview=()=>true);await consult(page,'39');await page.getByRole('alert').filter({hasText:'no se pudo mostrar en el árbol'}).waitFor();assert.equal(await page.locator('#TreeViewArchivo a[title="70|7|60|"]').count(),0);
}));
