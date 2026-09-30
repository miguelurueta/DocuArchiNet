const test = require('node:test');
const assert = require('node:assert/strict');
const reconciliation = require('../js/workflow/importar-servicio-web/importar-servicio-web-reconciliation.js');

const context = {IntentId:'intent-1', TaskId:1001, ProviderId:'INTEGRACIONSII'};

test('usa GetImportIntent y ReconcileImportIntent con contexto autorizado', async () => {
  const calls=[];
  const adapter=reconciliation.create({api:{
    getImportIntent:async request=>{calls.push(['get',request]);return {IntentId:request.IntentId,Status:'Parcial',Items:[]}},
    reconcileImportIntent:async request=>{calls.push(['reconcile',request]);return {IntentId:request.IntentId,Status:'Completado',Items:[{ExternalKey:request.ExternalKey,Status:'Disponible',TaskId:1001,DocumentId:9}]}}
  }});
  await adapter.get(context);
  const result=await adapter.reconcile(context,'external-1');
  assert.equal(result.items[0].status,'Disponible');
  assert.deepEqual(calls.map(entry=>entry[0]),['get','reconcile']);
  assert.equal(calls[1][1].TaskId,1001);
  assert.equal(calls[1][1].ProviderId,'INTEGRACIONSII');
  assert.equal(calls[1][1].ExternalKey,'external-1');
});

test('resultado incierto se reconcilia una sola vez por identidad externa', async () => {
  let reconciliations=0;
  const adapter=reconciliation.create({api:{getImportIntent:async()=>({Items:[]}),reconcileImportIntent:async request=>{reconciliations++;return {IntentId:request.IntentId,Status:'Completado',Items:[{ExternalKey:request.ExternalKey,Status:'Disponible',TaskId:1001,DocumentId:41}]}}}});
  const result=await adapter.complete({IntentId:'intent-1',Status:'ResultadoIncierto',Items:[{ExternalKey:'external-1',Status:'ResultadoIncierto',TaskId:1001},{ExternalKey:'external-1',Status:'Verificando',TaskId:1001}]},context);
  assert.equal(reconciliations,1);
  assert.equal(result.items[0].status,'Disponible');
  assert.equal(result.items[0].documentId,41);
});

test('éxito de ejecución se reconcilia antes de proyectar TaskId y nombre autoritativos', async () => {
  let reconciliations=0;
  const adapter=reconciliation.create({api:{getImportIntent:async()=>({Items:[]}),reconcileImportIntent:async request=>{
    reconciliations++;
    return {IntentId:request.IntentId,Status:'Completado',Items:[{ExternalKey:request.ExternalKey,Status:'Disponible',TaskId:1001,DocumentId:42,DocumentName:'Recibo De Caja'}]};
  }}});
  const projection={CabinetName:'MERCANTIL',DocumentId:42,Radicado:'S002469800',StorageType:'PDF',DocumentName:'Recibo De Caja',TaskId:1001,SignatureStatus:0,IconClass:'fa-file-pdf'};
  const result=await adapter.complete({IntentId:'intent-1',Status:'Completada',Items:[{ExternalKey:'external-1',Status:'Completada',DocumentId:42,TaskId:0,DocumentName:'',EnlaseProjection:projection}]},context);
  assert.equal(reconciliations,1);
  assert.equal(result.items[0].status,'Disponible');
  assert.equal(result.items[0].taskId,1001);
  assert.equal(result.items[0].documentName,'Recibo De Caja');
  assert.equal(result.items[0].enlaseProjection.cabinetName,'MERCANTIL');
  assert.equal(result.items[0].enlaseProjection.documentId,42);
});
test('la reconciliación conserva la proyección Workflow confirmada por la ejecución', async () => {
  const adapter=reconciliation.create({api:{getImportIntent:async()=>({Items:[]}),reconcileImportIntent:async request=>({IntentId:request.IntentId,Status:'Completado',Items:[{ExternalKey:request.ExternalKey,Status:'Disponible',TaskId:1001,DocumentId:43,DocumentName:'Certificado'}]})}});
  const projection={CabinetName:'MERCANTIL',DocumentId:43,Radicado:'S002469800',StorageType:'PDF',DocumentTypeName:'Certificado',TaskId:1001,SignatureStatus:0,IconClass:'fa-file-pdf'};
  const result=await adapter.complete({IntentId:'intent-1',Status:'Completada',Items:[{ExternalKey:'external-2',Status:'Completada',DocumentId:43,TaskId:1001,WorkflowProjection:projection}]},context);
  assert.equal(result.items[0].workflowProjection.documentTypeName,'Certificado');
  assert.equal(result.items[0].workflowProjection.documentId,43);
});
test('timeout, ausencia y estado desconocido nunca se convierten en disponible', () => {
  for (const state of ['Verificando','ResultadoIncierto','EstadoFuturo','']) {
    assert.notEqual(reconciliation.mapItem({Status:state,DocumentId:88}).status,'Disponible');
  }
  assert.throws(()=>reconciliation.adapt(null),/IMPORT_RECONCILIATION_UNAVAILABLE/);
});

test('reapertura obtiene el snapshot persistido sin polling', async () => {
  let reads=0;
  const adapter=reconciliation.create({api:{getImportIntent:async()=>{reads++;return {IntentId:'intent-1',Status:'Completado',Items:[{ExternalKey:'external-1',Status:'Disponible',TaskId:1001,DocumentId:55}]}},reconcileImportIntent:async()=>({Items:[]})}});
  const reopened=await adapter.get(context);
  assert.equal(reads,1);
  assert.equal(reopened.items[0].documentId,55);
});

