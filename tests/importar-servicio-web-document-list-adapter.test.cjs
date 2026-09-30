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

test('proyecta un documento reconciliado como Completado y conserva su contrato ENLASE', () => {
  const inserted=[];
  const projection={cabinetName:'MERCANTIL',documentId:93,radicado:'S002469800',storageType:'PDF',documentName:'Recibo De Caja',taskId:1001,signatureStatus:0,iconClass:'fa-file-pdf'};
  const adapter=documentList.create({currentTaskId:()=>1001,appendDocument:document=>{inserted.push(document);return true}});
  const result=adapter.synchronize({items:[
    {status:'Completado',taskId:1001,documentId:93,documentName:'Recibo De Caja',enlaseProjection:projection},
    {status:'Verificando',taskId:1001,documentId:94,enlaseProjection:projection}
  ]});
  assert.equal(result.appended,1);
  assert.equal(result.refreshed,false);
  assert.equal(inserted.length,1);
  assert.equal(inserted[0].documentId,93);
  assert.deepEqual(inserted[0].enlaseProjection,projection);
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

test('el adaptador ENLASE no fabrica filas parciales para Workflow', () => {
  let inserts=0;
  const append=documentList.createLegacyGridAppender({document:{getElementById:()=>({getElementsByTagName:()=>[]})},insertRow:()=>{inserts+=1;}});
  assert.equal(append({documentId:90,taskId:220586,documentName:'Recibo.pdf'}),false);
  assert.equal(inserts,0);
});

test('ENLASE proyecta el contrato completo en el grid del radicado mediante destino rad', () => {
  const rows=[];
  const grid={getElementsByTagName:name=>name==='tr'?rows:[]};
  const calls=[];
  const append=documentList.createLegacyGridAppender({
    document:{getElementById:id=>id==='GridView_list_documento_relacion'?grid:null},
    resolveTarget:()=>({gridId:'GridView_list_documento_relacion',destination:'rad',rowIdAttribute:'id_rad'}),
    insertRow:(data,destination)=>{calls.push({data,destination});const fields=data.split('|');rows.push({getAttribute:name=>name==='id_rad'?fields[1]:null});}
  });
  const projection={cabinetName:'MERCANTIL',documentId:92,radicado:'S002469800',storageType:'PDF',documentName:'Recibo <Caja>|SII',taskId:220586,signatureStatus:0,iconClass:'fa-file-pdf'};
  assert.equal(append({documentId:92,taskId:220586,documentName:'Anexo SII',enlaseProjection:projection}),true);
  assert.equal(calls.length,1);
  assert.equal(calls[0].destination,'rad');
  assert.equal(calls[0].data,'MERCANTIL|92|S002469800|PDF|Recibo &lt;Caja&gt;SII|220586|0|fa-file-pdf');
});

test('ENLASE rechaza la proyección incompleta y no inserta una fila parcial', () => {
  const grid={getElementsByTagName:()=>[]};
  let inserts=0;
  const append=documentList.createLegacyGridAppender({
    document:{getElementById:()=>grid},
    resolveTarget:()=>({gridId:'GridView_list_documento_relacion',destination:'rad',rowIdAttribute:'id_rad'}),
    insertRow:()=>{inserts+=1;}
  });
  assert.equal(append({documentId:92,taskId:220586,enlaseProjection:{documentId:92,taskId:220586,radicado:'S002469800'}}),false);
  assert.equal(inserts,0);
});
test('ENLASE no duplica una fila ya proyectada por documentId', () => {
  const rows=[{getAttribute:name=>name==='id_rad'?'90':null}];
  const grid={getElementsByTagName:()=>rows};
  let inserts=0;
  const append=documentList.createLegacyGridAppender({document:{getElementById:()=>grid},resolveTarget:()=>({gridId:'GridView_list_documento_relacion',destination:'rad',rowIdAttribute:'id_rad'}),insertRow:()=>{inserts+=1;}});
  assert.equal(append({documentId:90,taskId:220586,enlaseProjection:{cabinetName:'MERCANTIL',documentId:90,radicado:'S1',storageType:'PDF',documentName:'Recibo',taskId:220586,iconClass:'fa-file-pdf'}}),true);
  assert.equal(inserts,0);
});
