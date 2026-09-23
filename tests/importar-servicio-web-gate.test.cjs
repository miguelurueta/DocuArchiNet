'use strict';
const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');

const service = fs.readFileSync('webservice/WebServiceImportarServicioWebModern.asmx.vb', 'utf8');
const gate = fs.readFileSync('Infrastructure/Workflow/ImportarServicioWeb/ImportarServicioWebFeatureGate.vb', 'utf8');
const pageSource = fs.readFileSync('workflow/Webworkflow.aspx.vb', 'utf8');
const config = fs.readFileSync('web.config', 'utf8');
const operations = ['ResolveCapabilities', 'QueryItems', 'GetPreview', 'PreflightImport', 'CreateImportIntent', 'ExecuteImportIntent', 'GetImportIntent', 'ReconcileImportIntent'];

test('las ocho operaciones cortan por gate antes de validar o resolver dependencias', () => {
  for (const operation of operations) {
    const start = service.indexOf(`Function ${operation}`);
    const end = service.indexOf('End Function', start);
    const body = service.slice(start, end);
    assert.ok(start >= 0 && body.indexOf('FeatureEnabled()') >= 0, operation);
    for (const later of ['ValidRequest(', 'TryBuildImportContext(', 'ResolveProvider(', 'Compose(']) {
      const index = body.indexOf(later);
      if (index >= 0) assert.ok(body.indexOf('FeatureEnabled()') < index, `${operation}: ${later}`);
    }
  }
});

test('el gate exige bandera y audiencia explícita de usuario o grupo', () => {
  assert.match(gate, /WorkflowCentroTrabajoModernActive/);
  assert.match(gate, /WorkflowCentroTrabajoModernUsers/);
  assert.match(gate, /WorkflowCentroTrabajoModernGroups/);
  assert.match(gate, /users\) AndAlso String\.IsNullOrWhiteSpace\(groups\) Then Return False/);
  assert.match(gate, /Contains\(users, contexto\.LoginUsuario\) OrElse Contains\(groups, contexto\.IdGrupoWorkflow\.ToString\(\)\)/);
  assert.match(service, /New ImportarServicioWebFeatureGate\(\)\.EstaHabilitado\(session\.Contexto\)/);
  assert.match(pageSource, /Private ReadOnly Property ImportarServicioWebModernActive As Boolean/);
});

test('la configuración versionada queda cerrada y sin audiencias', () => {
  assert.match(config, /WorkflowCentroTrabajoModernActive" value="false"/i);
  assert.match(config, /WorkflowCentroTrabajoModernUsers" value=""/i);
  assert.match(config, /WorkflowCentroTrabajoModernGroups" value=""/i);
});
