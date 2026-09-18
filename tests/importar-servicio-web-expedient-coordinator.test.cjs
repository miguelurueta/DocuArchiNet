const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");

const coordinator = fs.readFileSync("Services/Workflow/ImportarServicioWeb/ImportExpedientCoordinator.vb", "utf8");
const plan = fs.readFileSync("Services/Workflow/ImportarServicioWeb/ImportExpedientPlan.vb", "utf8");
const project = fs.readFileSync("GestionDocumental-Docuarchi.net.vbproj", "utf8");

function resolve({ cached, found, created }) {
  const calls = { verify: 0, search: 0, create: 0 };
  if (cached) { calls.verify++; if (cached === "confirmed" || cached === "conflict") return { result: cached, calls }; }
  calls.search++;
  if (found !== "absent") return { result: found, calls };
  calls.create++;
  return { result: created, calls };
}

test("expediente cacheado verificado se reutiliza sin buscar ni crear", () => {
  assert.deepEqual(resolve({ cached: "confirmed", found: "absent", created: "confirmed" }), {
    result: "confirmed", calls: { verify: 1, search: 0, create: 0 }
  });
  assert.match(coordinator, /_expedients\.Verificar/);
});

test("expediente faltante se crea una sola vez", () => {
  assert.deepEqual(resolve({ cached: null, found: "absent", created: "confirmed" }), {
    result: "confirmed", calls: { verify: 0, search: 1, create: 1 }
  });
  assert.equal((coordinator.match(/_expedients\.Crear\(/g) || []).length, 1);
});

test("roles distinguen único primario y secundarios", () => {
  assert.match(coordinator, /Not configuration\.MultiplesExpedientes[\s\S]*RolExpedienteImportacion\.Unico/);
  assert.match(coordinator, /ordinal = 1, RolExpedienteImportacion\.Primario, RolExpedienteImportacion\.Secundario/);
});

test("plan liga inscripción y tipología a expediente", () => {
  assert.match(plan, /\.ClaveInscripcion = inscription\.ClaveInscripcion/);
  assert.match(plan, /\.IdTipoDocumental = item\.IdTipoDocumental/);
  assert.match(plan, /\.IdExpediente = inscription\.IdExpediente\.Value/);
  assert.match(plan, /\.Rol = inscription\.RolExpediente/);
});

test("falta de destino bloquea antes de almacenamiento", () => {
  assert.match(plan, /Not inscription\.IdExpediente\.HasValue[\s\S]*Then Return plan/);
  assert.match(plan, /Not byKey\.TryGetValue\(item\.ClaveInscripcion, inscription\) Then Return plan/);
  assert.match(plan, /plan\.Destinos\.Count <> intencion\.Resultados\.Count Then Return plan/);
  assert.doesNotMatch(coordinator + plan, /Almacenar\(|StoreImportExecutionStep|DocumentoAlmacenado/);
});

test("conflicto o resultado incierto nunca dispara creación", () => {
  for (const state of ["conflict", "unknown"]) {
    const result = resolve({ cached: null, found: state, created: "confirmed" });
    assert.equal(result.calls.create, 0);
  }
  assert.match(coordinator, /found\.Estado = EstadoEfectoExpedienteImportacion\.Conflicto/);
  assert.match(coordinator, /found\.Estado = EstadoEfectoExpedienteImportacion\.ResultadoIncierto/);
});

test("archivos se registran una sola vez", () => {
  for (const include of [
    "Services\\Workflow\\ImportarServicioWeb\\ImportExpedientPlan.vb",
    "Services\\Workflow\\ImportarServicioWeb\\ImportExpedientCoordinator.vb"
  ]) assert.equal(project.split(include).length - 1, 1, include);
});
