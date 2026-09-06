const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");

const root = path.resolve(__dirname, "..");
const read = (...parts) => fs.readFileSync(path.join(root, ...parts), "utf8");
const dto = read("DTOs", "Workflow", "ImportarServicioWeb", "ImportarServicioWebDtos.vb");

test("publica las ocho operaciones canonicas y auxiliares serializables", () => {
  const operations = ["ResolveCapabilities", "QueryItems", "GetPreview", "PreflightImport", "CreateImportIntent", "ExecuteImportIntent", "GetImportIntent", "ReconcileImportIntent"];
  for (const operation of operations) {
    assert.match(dto, new RegExp(`<Serializable\\(\\)> Public Class ${operation}(Request|Response)Dto`));
  }
  assert.match(dto, /Class DocumentCommandDto/);
  assert.match(dto, /Class ImportItemResultDto/);
  for (const field of ["SchemaVersion", "OperationId", "CorrelationId", "TaskId", "ProviderId"]) assert.match(dto, new RegExp(`Property ${field} `));
});

test("mantiene DTOs libres de estado web, SQL y conceptos SII", () => {
  assert.doesNotMatch(dto, /HttpContext|Session|HttpClient|WebRequest|\b(?:SELECT|INSERT|UPDATE|DELETE)\b/i);
  assert.doesNotMatch(dto, /INTEGRACIONSII|matr[ií]cula|c[oó]digo\s*de\s*barras|dato_lista/i);
});

test("los ocho fixtures son JSON v1 saneado", () => {
  const dir = path.join(root, "Tests", "Fixtures", "Workflow", "ImportarServicioWeb", "contracts-v1");
  const files = fs.readdirSync(dir).filter((file) => file.endsWith(".json"));
  assert.equal(files.length, 8);
  for (const file of files) {
    const raw = fs.readFileSync(path.join(dir, file), "utf8");
    const fixture = JSON.parse(raw);
    assert.equal(fixture.schemaVersion, "1.0", file);
    assert.doesNotMatch(raw, /INTEGRACIONSII|matr[ií]cula|c[oó]digo.?de.?barras|dato_lista/i);
  }
});

test("registra exactamente los seis archivos VB canonicos", () => {
  const project = read("GestionDocumental-Docuarchi.net.vbproj");
  const paths = [
    "DTOs\\Workflow\\ImportarServicioWeb\\ImportarServicioWebDtos.vb",
    "Modelo\\Workflow\\ImportarServicioWeb\\ImportarServicioWebModels.vb",
    "Modelo\\Workflow\\ImportarServicioWeb\\ImportarServicioWebInterfaces.vb",
    "Services\\Workflow\\ImportarServicioWeb\\ServicioImportarServicioWeb.vb",
    "Services\\Workflow\\ImportarServicioWeb\\RegistroProveedoresImportacion.vb",
    "Services\\Workflow\\ImportarServicioWeb\\ValidadorContextoImportacion.vb",
  ];
  for (const item of paths) assert.equal(project.split(`Compile Include="${item}"`).length - 1, 1, item);
});

test("no introduce contratos en fronteras legacy", () => {
  for (const dir of ["webservice", "App_Code", "ServiciosIntegracion", "Integracionccv", "workflow", "Infrastructure"]) {
    const absolute = path.join(root, dir);
    if (!fs.existsSync(absolute)) continue;
    const stack = [absolute];
    while (stack.length) {
      const current = stack.pop();
      for (const entry of fs.readdirSync(current, { withFileTypes: true })) {
        const child = path.join(current, entry.name);
        if (entry.isDirectory()) stack.push(child);
        else if (/\.(?:vb|asmx)$/i.test(entry.name)) assert.doesNotMatch(fs.readFileSync(child, "utf8"), /Class ResolveCapabilitiesRequestDto|Interface IExternalImportProvider/);
      }
    }
  }
});
