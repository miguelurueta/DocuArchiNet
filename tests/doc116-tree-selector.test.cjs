'use strict';
const test=require('node:test'),assert=require('node:assert/strict');
const {chromium}=require('../tools/e2e/node_modules/@playwright/test');
const {selectProductionLevel}=require('../tools/e2e/tests/support/doc116-expedientes-importacion.cjs');
test('local Chromium exercises the existing-tree callback and postback contract without an application server',async()=>{
 const browser=await chromium.launch({headless:true});const page=await browser.newPage();let posts=0;
 try{
  await page.route('**/*',async route=>{
   if(route.request().method()==='POST'){posts++;return route.fulfill({status:200,contentType:'text/plain',body:'synthetic postback'});}
   return route.fulfill({status:200,contentType:'text/html',body:'<div id="node7" title="7\\A"></div><div id="node8" title="8\\B"></div><div id="doc111-importacion" data-nivel="7"></div><script>window.ITEMS_DATOS=[{id:"node7"},{id:"node8"}];window.selected=[];window.OnSearchClick=async function(id,tree){selected.push({id,tree});await fetch(location.href,{method:"POST",body:"__ASYNCPOST=true"});document.getElementById("doc111-importacion").setAttribute("data-nivel",id==="node7"?"7":"8");};</script>'});
  });
  await page.goto('https://test.invalid/Gestion/WebFormProducionDocumental.aspx');
  assert.deepEqual(await selectProductionLevel(page,'7'),{level:'7',postback:true});
  assert.deepEqual(await selectProductionLevel(page,'8'),{level:'8',postback:true});
  assert.equal(posts,2);assert.deepEqual(await page.evaluate(()=>window.selected),[{id:'node7',tree:'TreeViewArchivo'},{id:'node8',tree:'TreeViewArchivo'}]);
 }finally{await browser.close();}
});
