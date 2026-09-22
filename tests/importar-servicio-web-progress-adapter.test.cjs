const test = require('node:test');
const assert = require('node:assert/strict');
const progress = require('../js/workflow/importar-servicio-web/importar-servicio-web-progress-adapter.js');

test('individual y múltiple ejecutan una sola vez cada intención completa', async () => {
  const calls=[];
  const adapter=progress.create({api:{executeImportIntent:async request=>{calls.push(request);return {IntentId:request.IntentId,Status:'Completada',Items:request.expected.map((key,index)=>({ExternalKey:key,Status:'Completada',DocumentId:index+1}))}},getImportIntent:async()=>({Items:[]})}});
  assert.equal((await adapter.execute({IntentId:'one',VersionToken:'v1',expected:['a']})).items.length,1);
  assert.equal((await adapter.execute({IntentId:'many',VersionToken:'v2',expected:['a','b']})).items.length,2);
  assert.equal(calls.length,2);
});

test('confirmaciones concurrentes comparten la promesa en vuelo', async () => {
  let calls=0, release;
  const waiting=new Promise(resolve=>{release=resolve});
  const adapter=progress.create({api:{executeImportIntent:async()=>{calls++;await waiting;return {IntentId:'i',Status:'Completada',Items:[{ExternalKey:'a',Status:'Completada',DocumentId:1}]}},getImportIntent:async()=>({Items:[]})}});
  const first=adapter.execute({IntentId:'i'}), second=adapter.execute({IntentId:'i'});
  assert.equal(first,second); release(); await first; assert.equal(calls,1);
});

test('recuperación exige causa explícita y realiza una sola lectura', async () => {
  let reads=0;
  const adapter=progress.create({api:{executeImportIntent:async()=>({Items:[]}),getImportIntent:async()=>{reads++;return {IntentId:'i',Status:'Detenida',Items:[]}}}});
  await assert.rejects(adapter.recover({IntentId:'i'},'manual'),/IMPORT_RECOVERY_REASON_REQUIRED/);
  await adapter.recover({IntentId:'i'},'timeout');
  assert.equal(reads,1);
});
