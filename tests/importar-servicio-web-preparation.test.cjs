const test = require('node:test');
const assert = require('node:assert/strict');
const preparation = require('../js/workflow/importar-servicio-web/importar-servicio-web-preparation.js');
const requirements = require('../js/workflow/importar-servicio-web/importar-servicio-web-requirements.js');

test('individual usa una colección de exactamente una fila y múltiple conserva selección', () => {
  const a={externalKey:'a'}, b={externalKey:'b'};
  assert.deepEqual(preparation.individual(a,220585).map(x=>x.ExternalKey),['a']);
  assert.deepEqual(preparation.multiple([b,a],220585).map(x=>x.ExternalKey),['b','a']);
});

test('tipología solo se asigna desde catálogo autorizado', () => {
  const items=preparation.individual({externalKey:'a'},1);
  assert.throws(()=>preparation.assignDocumentType(items,'a',{Id:9},[{Id:10,Name:'Permitida'}]),/DOCUMENT_TYPE_NOT_AUTHORIZED/);
  const next=preparation.assignDocumentType(items,'a',{Id:10},[{Id:10,Name:'Permitida'}]);
  assert.equal(next[0].DocumentTypeName,'Permitida');
});

test('estado habilita confirmar solo con datos completos y preflight ejecutable', () => {
  let model=requirements.initial([{DocumentTypeId:10,DocumentTypeName:'Tipo'}]);
  model=requirements.transition(model,'prepare');
  model=requirements.transition(model,'prepared',{Executable:true,ContextFingerprint:'fp',Requirements:[],EffectPlans:[]});
  assert.equal(requirements.canConfirm(model),true);
  assert.equal(requirements.transition(model,'stale').state,'edicion');
});
