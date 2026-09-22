const test = require('node:test');
const assert = require('node:assert/strict');
const preparation = require('../js/workflow/importar-servicio-web/importar-servicio-web-preparation.js');
const requirements = require('../js/workflow/importar-servicio-web/importar-servicio-web-requirements.js');
const fs = require('node:fs');

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

test('UI integra preparación explícita, plan previsto y restauración de foco', () => {
  const ui=fs.readFileSync('js/workflow/importar-servicio-web/importar-servicio-web-ui.js','utf8');
  const markup=fs.readFileSync('workflow/Webworkflow.aspx','utf8');
  assert.match(ui,/data-import-prepare-selected/);
  assert.match(ui,/intentClient\.preflight/);
  assert.match(ui,/Efectos previstos/);
  assert.match(ui,/preparationContext\.focus\.focus/);
  assert.match(markup,/importar-servicio-web-preparation-confirm/);
  assert.match(markup,/Crear intención/);
});

test('frontend no crea transporte, no ejecuta intención ni toca persistencia legacy', () => {
  const files=['js/workflow/importar-servicio-web/importar-servicio-web-preparation.js','js/workflow/importar-servicio-web/importar-servicio-web-requirements.js','js/workflow/importar-servicio-web/importar-servicio-web-intent-client.js'];
  const source=files.map(file=>fs.readFileSync(file,'utf8')).join('\n');
  assert.doesNotMatch(source,/fetch\s*\(|XMLHttpRequest|executeImportIntent|AlmacenaDocumentoTareaWorkflow|ClassAlmacenamiento|JSExpediente|JSProgresBar|ExpedientId/);
});

test('tabla SII mantiene scroll horizontal y vertical dentro del viewport', () => {
  const adapter=fs.readFileSync('js/workflow/importar-servicio-web/sii/importar-servicio-web-sii-adapter.js','utf8');
  const css=fs.readFileSync('Styles/importar-servicio-web-modern.css','utf8');
  assert.match(adapter,/importar-servicio-web-sii__table-scroll/);
  assert.match(adapter,/Tabla desplazable/);
  assert.match(css,/\.importar-servicio-web-sii__table-scroll\s*\{[^}]*max-height:[^}]*overflow:\s*auto/);
  assert.match(css,/\.importar-servicio-web-sii__table\s*\{[^}]*width:\s*max-content;[^}]*min-width:\s*100%/);
  assert.match(css,/@media \(max-width:\s*760px\)[^{]*\{[^}]*\.importar-servicio-web__dialog\s*\{[^}]*width:\s*100vw;[^}]*height:\s*100dvh/);
});
