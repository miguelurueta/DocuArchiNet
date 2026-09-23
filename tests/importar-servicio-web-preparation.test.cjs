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

test('predetermina Constancia de Inscripción sin depender de mayúsculas o tildes', () => {
  const catalog=[{DocumentTypeId:10,Name:'Sello de inscripción'},{DocumentTypeId:154,Name:'Constancia De Inscripción'}];
  const result=preparation.assignDefaultDocumentType(preparation.multiple([{externalKey:'a'},{externalKey:'b'}],1),catalog);
  assert.equal(result.documentType.DocumentTypeId,154);
  assert.deepEqual(result.items.map(item=>[item.DocumentTypeId,item.DocumentTypeName]),[[154,'Constancia De Inscripción'],[154,'Constancia De Inscripción']]);
});

test('predetermina el único tipo autorizado y falla cerrado ante coincidencia ambigua', () => {
  assert.equal(preparation.defaultDocumentType([{DocumentTypeId:77,Name:'Certificado registral'}]).DocumentTypeId,77);
  const ambiguous=[{DocumentTypeId:1,Name:'Constancia de inscripción'},{DocumentTypeId:2,Name:'Constancia inscripción especial'}];
  const result=preparation.assignDefaultDocumentType(preparation.individual({externalKey:'a'},1),ambiguous);
  assert.equal(result.documentType,null);
  assert.equal(result.items[0].DocumentTypeId,null);
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
  const css=fs.readFileSync('Styles/importar-servicio-web-modern.css','utf8');
  assert.match(ui,/data-import-prepare-selected/);
  assert.match(ui,/intentClient\.preflight/);
  assert.match(ui,/Efectos previstos/);
  assert.match(ui,/preparationContext\.focus\.focus/);
  assert.match(markup,/importar-servicio-web-preparation-confirm/);
  assert.match(markup,/Crear intención/);
  assert.match(ui,/Button_actualiza_trevie_seleccion/);
  assert.match(ui,/function updateSelectionState\(control\)/);
  assert.match(ui,/data-import-select-all/);
  assert.match(ui,/selectAll\.indeterminate/);
  assert.match(ui,/data-import-select="true"\]:not\(\[disabled\]\)/);
  assert.match(ui,/assignDefaultDocumentType/);
  assert.match(ui,/Tipología predeterminada/);
  assert.doesNotMatch(ui,/window\.location\.reload/);
  assert.match(ui,/function setExecutionCloseLock\(control, locked\)/);
  assert.match(ui,/control\.executionCloseLocked && force !== true/);
  assert.match(ui,/closeAfterResult\(control, reconciled\)/);
  assert.match(ui,/remove_endRequest\(onEndRequest\)/);
  assert.match(css,/#importar-servicio-web-preparation-confirm/);
  assert.match(css,/#importar-servicio-web-preparation-cancel/);
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
  assert.match(css,/\.importar-servicio-web-sii__table-scroll\s*\{[^}]*height:\s*max\(12rem,\s*calc\(100dvh - 28rem\)\)/);
});
