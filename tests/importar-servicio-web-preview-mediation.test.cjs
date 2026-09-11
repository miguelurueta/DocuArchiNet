const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");

const root = path.resolve(__dirname, "..");
const factory = fs.readFileSync(path.join(root, "Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiPreviewResponseFactory.vb"), "utf8");
const config = fs.readFileSync(path.join(root, "web.config"), "utf8");
const service = fs.readFileSync(path.join(root, "webservice/WebServiceImportarServicioWebModern.asmx.vb"), "utf8");
const descriptor = fs.readFileSync(path.join(root, "webservice/WebServiceImportarServicioWebModern.asmx"), "utf8");

test("preview media autorización, expiración, tipo, tamaño y disposición", () => {
  assert.match(factory, /PREVIEW_FORBIDDEN/);
  assert.match(factory, /PREVIEW_EXPIRY_MISSING/);
  assert.match(factory, /PREVIEW_EXPIRY_PAST/);
  assert.match(factory, /PREVIEW_CONTENT_TYPE_INVALID/);
  assert.match(factory, /PREVIEW_SIZE_INVALID/);
  assert.match(factory, /PREVIEW_DISPOSITION_INVALID/);
  assert.match(factory, /\.Disposition = "inline"/);
  assert.match(factory, /SafeToken\(source\.DescriptorId\)/);
  assert.doesNotMatch(factory, /Authorization|PhysicalPath|FileSystem|ClassAlmacenamiento/);
});

test("preview SII usa allowlist versionada y diagnosticos seguros", () => {
  assert.match(config, /ImportarServicioWebSiiDownloadHosts/);
  assert.match(config, /value="confecamaras\.co,repositoriosii\.s3\.amazonaws\.com"/);
  assert.match(service, /SafeExternalCode\(ex\)/);
  assert.match(service, /SafeProviderCode\(ex\.Message\)/);
  assert.match(service, /SII_RESOURCE_HOST_NOT_ALLOWED/);
  assert.match(service, /StartsWith\("SII_RESOURCE_HOST_NOT_ALLOWED_"/);
});

test("ASMX aplica gate antes de resolver contexto o proveedor", () => {
  for (const method of ["ResolveCapabilities", "QueryItems", "GetPreview"]) {
    const start = service.indexOf(`Function ${method}`);
    const body = service.slice(start, service.indexOf("End Function", start));
    assert.ok(body.indexOf("FeatureEnabled()") >= 0);
    assert.ok(body.indexOf("FeatureEnabled()") < body.indexOf("ResolveProvider("));
  }
  assert.match(service, /FEATURE_DISABLED/);
  assert.match(service, /provider\.Cliente\.[A-Za-z]+Async\([^\r\n]+\)\.GetAwaiter\(\)\.GetResult\(\)/);
  assert.doesNotMatch(service, /\.Result\b|\.Wait\s*\(/);
});

test("contexto de importación usa tarea y trámite confiables de sesión", () => {
  assert.match(service, /Session\.Item\("ID_TAREA_SELECCIONDA"\)/);
  assert.match(service, /Session\.Item\("DG_ID_TRAMITE"\)/);
  assert.match(service, /request\.TaskId <> trustedTaskId/);
  assert.match(service, /trustedProcedureId, request\.ProviderId/);
  assert.doesNotMatch(service, /IdRutaWorkflow, request\.TaskId, request\.ProviderId/);
});

test("frontera es delgada y descriptor referencia code-behind", () => {
  assert.match(descriptor, /CodeBehind="WebServiceImportarServicioWebModern\.asmx\.vb"/);
  assert.match(descriptor, /Class="GestionDocumental_Docuarchi\.net\.WebServiceImportarServicioWebModern"/);
  assert.doesNotMatch(service, /SELECT\s|INSERT\s|UPDATE\s|DELETE\s|ClassAlmacenamiento|System\.IO|File\./i);
});
