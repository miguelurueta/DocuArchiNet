const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");

const root = path.resolve(__dirname, "..");
const service = fs.readFileSync(path.join(root, "webservice/WebServiceImportarServicioWebModern.asmx.vb"), "utf8");
const configuration = fs.readFileSync(path.join(root, "web.config"), "utf8");

test("cada endpoint ASMX implementado evalua el gate antes de sus dependencias", () => {
  for (const method of ["ResolveCapabilities", "QueryItems", "GetPreview"]) {
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

test("gate apagado produce codigo funcional estable", () => {
  assert.match(service, /ConfigurationManager\.AppSettings\("WorkflowCentroTrabajoModernActive"\)/);
  assert.ok((service.match(/FEATURE_DISABLED/g) || []).length >= 3);
});

test("configuracion versionada conserva gate y alcance vacios", () => {
  assert.match(configuration, /<add key="WorkflowCentroTrabajoModernActive" value="false"\s*\/>/i);
  assert.match(configuration, /<add key="WorkflowCentroTrabajoModernUsers" value=""\s*\/>/i);
  assert.match(configuration, /<add key="WorkflowCentroTrabajoModernGroups" value=""\s*\/>/i);
});
