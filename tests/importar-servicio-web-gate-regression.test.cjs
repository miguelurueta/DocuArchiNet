const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");

const root = path.resolve(__dirname, "..");
const service = fs.readFileSync(path.join(root, "webservice/WebServiceImportarServicioWebModern.asmx.vb"), "utf8");
const gate = fs.readFileSync(path.join(root, "Infrastructure/Workflow/ImportarServicioWeb/ImportarServicioWebFeatureGate.vb"), "utf8");
const configuration = fs.readFileSync(path.join(root, "web.config"), "utf8");

test("cada endpoint ASMX implementado evalua el gate antes de sus dependencias", () => {
  for (const method of ["ResolveCapabilities", "QueryItems", "GetPreview", "PreflightImport", "CreateImportIntent", "ExecuteImportIntent", "GetImportIntent", "ReconcileImportIntent"]) {
    const start = service.indexOf(`Function ${method}`);
    const end = service.indexOf("End Function", start);
    assert.ok(start >= 0 && end > start, method);
    const body = service.slice(start, end);
    const gate = body.indexOf("FeatureEnabled()");
    assert.ok(gate >= 0, method);
    for (const effect of ["ResolveTrustedContext(", "ResolveProvider(", "Await provider."]) {
      const index = body.indexOf(effect);
      if (index >= 0) assert.ok(gate < index, `${method}: ${effect}`);
    }
  }
});

test("gate apagado corta antes de crear servicios o producir efectos", () => {
  for (const method of ["PreflightImport", "CreateImportIntent", "ExecuteImportIntent", "GetImportIntent", "ReconcileImportIntent"]) {
    const start = service.indexOf(`Function ${method}`);
    const end = service.indexOf("End Function", start);
    const body = service.slice(start, end);
    const gate = body.indexOf("FeatureEnabled()");
    assert.ok(gate >= 0, method);
    for (const operation of ["ResolveTrustedContext(", "Build", ".Execute(", ".Crear(", ".ReconcileImportIntent("]) {
      const effect = body.indexOf(operation);
      if (effect >= 0) assert.ok(gate < effect, `${method}: ${operation}`);
    }
  }
});

test("fallback no invoca simultáneamente ruta moderna y legacy", () => {
  assert.doesNotMatch(service, /WebServiceGaExpediente|WebService_integracion_sii|ServiceCreaExpedienteIntegracionSII|ServiceSolicitaRegistroExpedienteMatricula/);
});

test("gate apagado produce codigo funcional estable", () => {
  assert.match(gate, /ConfigurationManager\.AppSettings\(key\)/);
  assert.match(gate, /WorkflowCentroTrabajoModernActive/);
  assert.match(service, /ImportarServicioWebFeatureGate/);
  assert.ok((service.match(/FEATURE_DISABLED/g) || []).length >= 3);
});

test("configuración versionada conserva el gate desactivado y sin listas de audiencia", () => {
  assert.match(configuration, /<add key="WorkflowCentroTrabajoModernActive" value="false"\s*\/>/i);
  assert.match(configuration, /<add key="WorkflowCentroTrabajoModernUsers" value=""\s*\/>/i);
  assert.match(configuration, /<add key="WorkflowCentroTrabajoModernGroups" value=""\s*\/>/i);
});
