const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');

const root = path.resolve(__dirname, '..');
const service = fs.readFileSync(path.join(root, 'webservice/WebServiceImportarServicioWebModern.asmx.vb'), 'utf8');
const project = fs.readFileSync(path.join(root, 'GestionDocumental-Docuarchi.net.vbproj'), 'utf8');
const configuration = fs.readFileSync(path.join(root, 'Web.config'), 'utf8');
const releaseConfiguration = fs.readFileSync(path.join(root, 'Web.Release.config'), 'utf8');

test('el gate retirado no puede reaparecer en código, proyecto o configuración', () => {
  for (const source of [service, project, configuration, releaseConfiguration]) {
    assert.doesNotMatch(source, /ImportarServicioWebFeatureGate|WorkflowCentroTrabajoModernActive|WorkflowCentroTrabajoModernUsers|WorkflowCentroTrabajoModernGroups|FEATURE_DISABLED/);
  }
});

test('cada endpoint valida contexto antes de resolver dependencias con efectos', () => {
  for (const method of ['ResolveCapabilities', 'QueryItems', 'GetPreview', 'PreflightImport', 'CreateImportIntent', 'ExecuteImportIntent', 'GetImportIntent', 'ReconcileImportIntent']) {
    const start = service.indexOf(`Function ${method}`);
    const end = service.indexOf('End Function', start);
    const body = service.slice(start, end);
    const context = body.indexOf('TryBuildImportContext(');
    assert.ok(context >= 0, method);
    for (const effect of ['ResolveProvider(', 'Compose(', 'Await provider.']) {
      const index = body.indexOf(effect);
      if (index >= 0) assert.ok(context < index, `${method}: ${effect}`);
    }
  }
});

test('fallback no invoca simultáneamente ruta moderna y legacy', () => {
  assert.doesNotMatch(service, /WebServiceGaExpediente|WebService_integracion_sii|ServiceCreaExpedienteIntegracionSII|ServiceSolicitaRegistroExpedienteMatricula/);
});
