const test = require('node:test');
const assert = require('node:assert/strict');
const documentList = require('../js/workflow/importar-servicio-web/importar-servicio-web-document-list-adapter.js');

test('recorre el lote y deduplica documentos disponibles por DocumentId', () => {
  const inserted=[];
  const adapter=documentList.create({currentTaskId:()=>1001,appendDocument:document=>{inserted.push(document.documentId);return true}});
  const result=adapter.synchronize({items:[
    {status:'Disponible',taskId:1001,documentId:90},
    {status:'Disponible',taskId:1001,documentId:90},
    {status:'Disponible',taskId:1001,documentId:91},
    {status:'Fallido',taskId:1001,documentId:92}
  ]});
  assert.deepEqual(inserted,[90,91]);
  assert.equal(result.appended,2);
  assert.equal(result.refreshed,false);
});

test('usa refresco autoritativo cuando no existe contrato visual seguro', () => {
  let refreshed;
  const adapter=documentList.create({currentTaskId:()=>1001,refresh:request=>{refreshed=request}});
  const result=adapter.synchronize({items:[{status:'Disponible',taskId:1001,documentId:90,documentName:'a.pdf'}]});
  assert.equal(result.refreshed,true);
  assert.deepEqual(refreshed.documents.map(item=>item.documentId),[90]);
  assert.equal(JSON.stringify(refreshed).includes('dato_lista'),false);
});

test('solo ofrece apertura para identificador interno positivo', () => {
  const opened=[];
  const adapter=documentList.create({currentTaskId:()=>1001,openDocument:id=>{opened.push(id);return true}});
  assert.equal(adapter.canOpen(0),false);
  assert.equal(adapter.open('no-valido'),false);
  assert.equal(adapter.open(45),true);
  assert.deepEqual(opened,[45]);
});

test('autoriza la acción solo para documento confirmado de la tarea visible', () => {
  const adapter=documentList.create({currentTaskId:()=>1001,openDocument:()=>true});
  assert.equal(adapter.isAuthorized({visibleState:'Importada',taskId:1001,documentId:45}),true);
  assert.equal(adapter.isAuthorized({visibleState:'Importada',taskId:2002,documentId:45}),false);
  assert.equal(adapter.isAuthorized({visibleState:'Verificando',taskId:1001,documentId:45}),false);
});
