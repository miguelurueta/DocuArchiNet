'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs'),path=require('node:path');
const {chromium}=require('../tools/e2e/node_modules/playwright');
const root=path.resolve(__dirname,'..');
async function fixture(work){
  const browser=await chromium.launch({headless:true});
  try{const page=await browser.newPage({viewport:{width:390,height:260}});page.setDefaultTimeout(3000);await page.setContent('<button id="target" style="position:fixed;bottom:4px;right:4px">Recurso</button>');
    await page.addStyleTag({content:fs.readFileSync(path.join(root,'js/jquery.contextMenu.css'),'utf8')});
    for(const name of ['js/ui/jquery-3.4.1.min.js','js/jquery.contextMenu.js'])await page.addScriptTag({content:fs.readFileSync(path.join(root,name),'utf8')});
    await work(page);
  }finally{await browser.close();}
}
test('DOC119 contextual alto cabe en viewport y permite alcanzar la ultima accion',()=>fixture(async page=>{
  await page.evaluate(()=>{const actions={};for(let i=0;i<30;i++)actions['Accion '+i]={click(){}};$('#target').contextMenu('context-test',actions);});
  await page.locator('#target').click({button:'right'});
  const box=await page.locator('.context-menu:visible').boundingBox();
  assert(box.x>=0&&box.y>=0&&box.x+box.width<=390&&box.y+box.height<=260,JSON.stringify(box));
  const menu=page.locator('.context-menu:visible');await menu.evaluate(el=>{el.scrollTop=el.scrollHeight;});
  await page.getByText('Accion 29',{exact:true}).click();assert.equal(await menu.count(),0);
}));
test('DOC119 contextual abre por teclado, conserva invocador y devuelve foco con Escape',()=>fixture(async page=>{
  await page.evaluate(()=>{window.keyboardActions=0;$('#target').contextMenu('context-test',{'Primera':{click(){}},'Segunda':{click(){window.keyboardActions++;}}});});
  await page.locator('#target').focus();await page.keyboard.press('Shift+F10');
  await page.getByRole('menu').waitFor({state:'visible'});assert.equal(await page.evaluate(()=>document.activeElement.textContent.trim()),'Primera');
  await page.keyboard.press('ArrowDown');assert.equal(await page.evaluate(()=>document.activeElement.textContent.trim()),'Segunda');
  await page.keyboard.press('Escape');assert.equal(await page.evaluate(()=>document.activeElement.id),'target');
  await page.keyboard.press('Shift+F10');await page.keyboard.press('End');await page.keyboard.press('Space');assert.equal(await page.evaluate(()=>keyboardActions),1);
}));
test('DOC119 contextual conserva captura al abrir, bloquea deshabilitados y no duplica tras reemplazo',()=>fixture(async page=>{
  await page.evaluate(()=>{
    window.seen=[];const bind=()=>$('#target').contextMenu('context-test',{'Deshabilitada':{enabled:()=>false,click(){seen.push('wrong');}},'Activa':{click(element,invocation){seen.push({id:element.attr('id'),value:invocation.data});}}},{capture:target=>target.getAttribute('data-ref')});
    document.getElementById('target').setAttribute('data-ref','original');bind();bind();
  });
  assert.equal(await page.locator('.context-menu').count(),1);
  await page.locator('#target').click({button:'right'});await page.evaluate(()=>document.getElementById('target').setAttribute('data-ref','changed'));
  await page.getByRole('menuitem',{name:'Activa',exact:true}).click();
  assert.deepEqual(await page.evaluate(()=>seen),[{id:'target',value:'original'}]);
  await page.evaluate(()=>{document.getElementById('target').outerHTML='<button id="target">Nuevo</button>';$('#target').contextMenu('context-test',{'Nuevo':{click(){}}});});
  assert.equal(await page.locator('.context-menu').count(),1);
}));
test('DOC119 consumidores inventariados conservan callback y recurso invocador',()=>fixture(async page=>{
  const inventory=JSON.parse(fs.readFileSync(path.join(root,'Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc119-contextuales-inventario-20261112.json'),'utf8')).consumers;
  assert.equal(inventory.length,26);
  for(let index=0;index<inventory.length;index++){
    const entry=inventory[index];assert.match(entry.selector,/^#[A-Za-z0-9_-]+$/);
    await page.evaluate(({entry,index})=>{
      $('#consumer-fixture').remove();const wrapper=$('<div id="consumer-fixture">').appendTo('body');
      const container=$('<div>').attr('id',entry.selector.slice(1)).appendTo(wrapper);$('<button>').attr('id','resource-'+index).text('Invocador').appendTo(container);
      window.observed=null;container.contextMenu(entry.menuName,{'Accion observada':{click(element,invocation){window.observed={element:element.attr('id'),target:invocation.target.id};}}});
    },{entry,index});
    await page.locator('#resource-'+index).click({button:'right'});
    await page.getByRole('menuitem',{name:'Accion observada',exact:true}).click();
    assert.deepEqual(await page.evaluate(()=>observed),{element:entry.selector.slice(1),target:'resource-'+index},entry.file);
  }
}));
