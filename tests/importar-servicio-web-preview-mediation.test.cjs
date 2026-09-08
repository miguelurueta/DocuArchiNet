const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");

const root = path.resolve(__dirname, "..");
const factory = fs.readFileSync(path.join(root, "Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiPreviewResponseFactory.vb"), "utf8");
const service = fs.readFileSync(path.join(root, "webservice/WebServiceImportarServicioWebModern.asmx.vb"), "utf8");
const descriptor = fs.readFileSync(path.join(root, "webservice/WebServiceImportarServicioWebModern.asmx"), "utf8");

test("preview media autorización, expiración, tipo, tamaño y disposición", () => {
  assert.match(factory, /PREVIEW_FORBIDDEN/);
  assert.match(factory, /PREVIEW_EXPIRED/);
  assert.match(factory, /PREVIEW_CONTENT_TYPE_INVALID/);
  assert.match(factory, /PREVIEW_SIZE_INVALID/);
  assert.match(factory, /PREVIEW_DISPOSITION_INVALID/);
  assert.match(factory, /\.Disposition = "inline"/);
  assert.match(factory, /SafeToken\(source\.DescriptorId\)/);
  assert.doesNotMatch(factory, /Authorization|PhysicalPath|FileSystem|ClassAlmacenamiento/);
});

test("ASMX aplica gate antes de resolver contexto o proveedor", () => {
  for (const method of ["ResolveCapabilities", "QueryItems", "GetPreview"]) {
    const start = service.indexOf(`Function ${method}`);
    const body = service.slice(start, service.indexOf("End Function", start));
    assert.ok(body.indexOf("FeatureEnabled()") >= 0);
    assert.ok(body.indexOf("FeatureEnabled()") < body.indexOf("ResolveProvider("));
  }
  assert.match(service, /FEATURE_DISABLED/);
  assert.match(service, /Await provider\.Cliente\./);
  assert.doesNotMatch(service, /\.Result\b|\.Wait\s*\(|GetAwaiter\s*\(/);
});

test("frontera es delgada y descriptor referencia code-behind", () => {
  assert.match(descriptor, /CodeBehind="WebServiceImportarServicioWebModern\.asmx\.vb"/);
  assert.match(descriptor, /Class="WebServiceImportarServicioWebModern"/);
  assert.doesNotMatch(service, /SELECT\s|INSERT\s|UPDATE\s|DELETE\s|ClassAlmacenamiento|System\.IO|File\./i);
});
