const test = require('node:test');
const assert = require('node:assert/strict');
const factory = require('../js/workflow/importar-servicio-web/importar-servicio-web-intent-client.js');

test('una confirmación concurrente crea una sola intención con toda la colección', async () => {
  let calls=0, release;
  const wait=new Promise(resolve=>{release=resolve});
  const client=factory.create({api:{preflightImport:async()=>({Executable:true,ContextFingerprint:'fp',Requirements:[{Codigo:'OK',Satisfecho:true}],EffectPlans:[{}]}),createImportIntent:async request=>{calls++; await wait; return {IntentId:'i',count:request.Items.length};}}});
  await client.preflight({TaskId:1},[{ExternalKey:'a'},{ExternalKey:'b'}]);
  const first=client.confirm({IdempotencyKey:'key'}), second=client.confirm({IdempotencyKey:'key'});
  assert.equal(first,second); release();
  assert.equal((await first).count,2); assert.equal(calls,1);
});

test('PREFLIGHT_STALE invalida la preparación', async () => {
  const client=factory.create({api:{preflightImport:async()=>({Executable:true,ContextFingerprint:'fp',Requirements:[],EffectPlans:[]}),createImportIntent:async()=>({Error:{Codigo:'PREFLIGHT_STALE'}})}});
  await client.preflight({},[{ExternalKey:'a'}]);
  await assert.rejects(client.confirm({}),/PREFLIGHT_STALE/);
  assert.equal(client.hasPrepared(),false);
});
