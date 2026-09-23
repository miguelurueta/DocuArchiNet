const test = require('node:test');
const assert = require('node:assert/strict');
const documentList = require('../js/workflow/importar-servicio-web/importar-servicio-web-document-list-adapter.js');

test('no proyecta documentos de otra tarea', () => {
  const inserted=[];
  const adapter=documentList.create({currentTaskId:()=>2002,appendDocument:document=>{inserted.push(document);return true}});
  const result=adapter.synchronize({items:[{status:'Disponible',taskId:1001,documentId:90}]});
  assert.deepEqual(result.documents,[]);
  assert.deepEqual(inserted,[]);
});

test('revalida la tarea visible inmediatamente antes de sincronizar', () => {
  let visibleTask=1001, inserted=0;
  const adapter=documentList.create({currentTaskId:()=>visibleTask,appendDocument:()=>{inserted++;return true}});
  const snapshot={items:[{status:'Disponible',taskId:1001,documentId:90}]};
  assert.equal(adapter.collect(snapshot).length,1);
  visibleTask=2002;
  assert.equal(adapter.synchronize(snapshot).documents.length,0);
  assert.equal(inserted,0);
});

test('rechaza documento sin identificador o sin confirmación disponible', () => {
  const adapter=documentList.create({currentTaskId:()=>1001,appendDocument:()=>true});
  assert.deepEqual(adapter.collect({items:[
    {status:'Disponible',taskId:1001,documentId:null},
    {status:'Verificando',taskId:1001,documentId:90},
    {status:'Inconsistente',taskId:1001,documentId:91}
  ]}),[]);
});

