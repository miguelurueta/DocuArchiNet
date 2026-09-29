'use strict';
const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');

const service = fs.readFileSync('webservice/WebServiceImportarServicioWebModern.asmx.vb', 'utf8');
const preview = fs.readFileSync('workflow/ImportarServicioWebPreview.ashx.vb', 'utf8');
const pageSource = fs.readFileSync('workflow/Webworkflow.aspx.vb', 'utf8');
const config = fs.readFileSync('Web.config', 'utf8');
const releaseConfig = fs.readFileSync('Web.Release.config', 'utf8');

test('la importación moderna es oficial y no depende de feature flags', () => {
  for (const source of [service, preview, pageSource, config, releaseConfig]) {
    assert.doesNotMatch(source, /WorkflowCentroTrabajoModernActive|WorkflowCentroTrabajoModernUsers|WorkflowCentroTrabajoModernGroups|ImportarServicioWebFeatureGate|FEATURE_DISABLED/);
  }
});

test('las ocho operaciones conservan validación de solicitud y contexto Workflow', () => {
  for (const operation of ['ResolveCapabilities', 'QueryItems', 'GetPreview', 'PreflightImport', 'CreateImportIntent', 'ExecuteImportIntent', 'GetImportIntent', 'ReconcileImportIntent']) {
    const start = service.indexOf(`Function ${operation}`);
    const end = service.indexOf('End Function', start);
    const body = service.slice(start, end);
    assert.ok(start >= 0, operation);
    assert.match(body, /ValidRequest\(/, operation);
    assert.match(body, /TryBuildImportContext\(/, operation);
  }
});

test('el preview conserva su validación de sesión y contexto', () => {
  assert.match(preview, /WorkflowPreviewSessionContextGate\(\)\.AsegurarContexto\(\)/);
  assert.match(preview, /session\.Contexto\.EsValido\(\)/);
  assert.match(preview, /TryResolveTrustedTaskId\(context, taskId\)/);
});
