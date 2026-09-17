const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");

const source = fs.readFileSync("Infrastructure/Repositories/Workflow/ImportarServicioWeb/PhysicalImportExpedientRepository.vb", "utf8");
assert.match(source, /EXPEDIENT_PRIMARY_IDENTITY_CONFLICT/);
assert.match(source, /EXPEDIENT_IDENTITY_EXPECTATION_UNRESOLVED/);
assert.match(source, /field\.Obligatorio AndAlso String\.IsNullOrWhiteSpace\(field\.Valor\)/);
assert.match(source, /If\(field\.Valor, String\.Empty\)\.Trim\(\)/);
assert.match(source, /IdentityFieldCode\("EXPEDIENT_IDENTITY_EXPECTATION_UNRESOLVED", fieldName\)/);
assert.match(source, /EXPEDIENT_IDENTITY_FIELD_MISSING/);
assert.match(source, /EXPEDIENT_IDENTITY_FIELD_CONFLICT/);

function create(before, after, responseReceived = true) {
  let effects = 0;
  if (before.length === 1) return { state: "confirmed", effects };
  if (before.length > 1) return { state: "conflict", effects };
  effects++;
  if (after.length === 1) return { state: "confirmed", effects };
  if (after.length > 1) return { state: "conflict", effects };
  return { state: responseReceived ? "failed" : "uncertain", effects };
}

test("repetición reutiliza expediente sin efecto duplicado", () => {
  assert.deepEqual(create([{ id: 7 }], [{ id: 7 }]), { state: "confirmed", effects: 0 });
  assert.match(source, /Dim existente = Buscar\(contexto, inscripcion, configuracion\)/);
  assert.match(source, /If existente\.Estado <> EstadoEfectoExpedienteImportacion\.Ausente Then Return existente/);
});

test("respuesta perdida se consulta antes de cualquier nuevo intento", () => {
  assert.deepEqual(create([], [{ id: 8 }], false), { state: "confirmed", effects: 1 });
  assert.deepEqual(create([], [], false), { state: "uncertain", effects: 1 });
  assert.equal((source.match(/_gateway\.Crear\(/g) || []).length, 1);
  assert.match(source, /EXPEDIENT_CREATE_RESULT_UNKNOWN/);
});

test("identidad conflictiva bloquea creación", () => {
  assert.deepEqual(create([{ id: 7 }, { id: 8 }], []), { state: "conflict", effects: 0 });
  assert.match(source, /EXPEDIENT_IDENTITY_CONFLICT/);
});
