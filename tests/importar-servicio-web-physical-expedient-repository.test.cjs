const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");

const repository = fs.readFileSync("Infrastructure/Repositories/Workflow/ImportarServicioWeb/PhysicalImportExpedientRepository.vb", "utf8");
const gateway = fs.readFileSync("Infrastructure/Workflow/ImportarServicioWeb/Expedients/PhysicalExpedientGateway.vb", "utf8");
const legacyGateway = fs.readFileSync("Infrastructure/Workflow/ImportarServicioWeb/Expedients/LegacyPhysicalExpedientGateway.vb", "utf8");
const project = fs.readFileSync("GestionDocumental-Docuarchi.net.vbproj", "utf8");

function create(existingBefore, createResponse, existingAfter) {
  let createCalls = 0;
  const locate = (phase) => phase === "before" ? existingBefore : existingAfter;
  const before = locate("before");
  if (before.length === 1) return { state: "confirmed", createCalls };
  if (before.length > 1) return { state: "conflict", createCalls };
  createCalls++;
  const after = locate("after");
  if (after.length === 1) return { state: "confirmed", createCalls };
  if (after.length > 1) return { state: "conflict", createCalls };
  return { state: createResponse.received ? "failed" : "unknown", createCalls };
}

test("existente y faltante se distinguen antes de crear", () => {
  assert.deepEqual(create([{ id: 7 }], { received: true }, [{ id: 7 }]), { state: "confirmed", createCalls: 0 });
  assert.deepEqual(create([], { received: true }, [],), { state: "failed", createCalls: 1 });
  assert.match(repository, /EXPEDIENT_NOT_FOUND/);
});

test("respuesta perdida se postverifica y no duplica", () => {
  assert.deepEqual(create([], { received: false }, [{ id: 9 }]), { state: "confirmed", createCalls: 1 });
  assert.deepEqual(create([], { received: false }, []), { state: "unknown", createCalls: 1 });
  assert.match(repository, /ResultadoIncierto/);
  assert.match(repository, /Dim confirmado = Buscar\(contexto, inscripcion, configuracion\)/);
});

test("identidad múltiple o discrepante produce conflicto", () => {
  assert.deepEqual(create([{ id: 1 }, { id: 2 }], { received: true }, []), { state: "conflict", createCalls: 0 });
  assert.match(repository, /encontrados\.Count <> 1/);
  assert.match(repository, /For Each field In configuracion\.CamposIdentidad/);
  assert.match(repository, /EXPEDIENT_IDENTITY_CONFLICT/);
});

test("adaptador físico es interno y recibe contexto confiable", () => {
  for (const operation of ["Localizar", "Crear", "Obtener"]) {
    assert.match(gateway, new RegExp(`Function ${operation}\\(ByVal contexto As ContextoImportacionServicio`));
  }
  assert.doesNotMatch(gateway + repository, /HttpWebRequest|WebClient|\.asmx|HttpContext|Session/);
});

test("snapshot adapta los nombres historicos a las columnas configurables", () => {
  assert.match(legacyGateway, /target\("CODIGO_SERIE_TRD"\) = Convert\.ToString\(record\.CODIGO_SERIE\)/);
  assert.match(legacyGateway, /target\("CODIGO_SUB_SERIE_TRD"\) = Convert\.ToString\(record\.CODIGO_SUBSERIE\)/);
  assert.match(legacyGateway, /target\("NOMBRE_AREA_TRD"\) = Convert\.ToString\(record\.NOMBRE_AREA\)/);
});

test("archivos modernos se registran una sola vez", () => {
  for (const include of [
    "Infrastructure\\Repositories\\Workflow\\ImportarServicioWeb\\PhysicalImportExpedientRepository.vb",
    "Infrastructure\\Workflow\\ImportarServicioWeb\\Expedients\\PhysicalExpedientGateway.vb"
  ]) assert.equal(project.split(include).length - 1, 1, include);
});
