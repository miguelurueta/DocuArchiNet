const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");

const cases = JSON.parse(fs.readFileSync("Tests/Fixtures/Workflow/ImportarServicioWeb/expedient-planning-v1/cases.json", "utf8"));
const coordinator = fs.readFileSync("Services/Workflow/ImportarServicioWeb/ImportExpedientCoordinator.vb", "utf8");
const planSource = fs.readFileSync("Services/Workflow/ImportarServicioWeb/ImportExpedientPlan.vb", "utf8");
const normalizer = fs.readFileSync("Services/Workflow/ImportarServicioWeb/ImportExpedientIdentityNormalizer.vb", "utf8");

function build(value) {
  const inscriptions = value.inscriptions.map((x, index) => ({
    ...x,
    role: value.multiple ? (index === 0 ? "Primario" : "Secundario") : "Unico"
  }));
  const byKey = new Map(inscriptions.map(x => [x.key, x]));
  const destinations = value.items.map(item => {
    const inscription = byKey.get(item.key);
    if (!inscription?.expedient) return null;
    return { client: item.client, key: item.key, type: item.type, expedient: inscription.expedient, role: inscription.role };
  });
  return { inscriptions, destinations, confirmed: destinations.every(Boolean) && destinations.length === value.items.length };
}

test("planificación focal cubre los tres gabinetes", () => {
  assert.deepEqual(cases.map(x => x.cabinet), ["MERCANTIL", "ESAL", "RUP"]);
  for (const cabinet of cases.map(x => x.cabinet)) assert.match(normalizer, new RegExp(`Case "${cabinet}"`));
});

test("modo único asigna destino único", () => {
  const result = build(cases.find(x => x.case === "mercantil-single"));
  assert.equal(result.confirmed, true);
  assert.deepEqual(result.inscriptions.map(x => x.role), ["Unico"]);
  assert.equal(result.destinations[0].expedient, 101);
});

test("modo múltiple distingue principal y todos los secundarios", () => {
  for (const value of cases.filter(x => x.multiple)) {
    const result = build(value);
    assert.equal(result.confirmed, true, value.case);
    assert.deepEqual(result.inscriptions.map(x => x.role), value.roles, value.case);
    assert.equal(result.destinations.length, value.items.length, value.case);
  }
  assert.match(coordinator, /ordinal = 1, RolExpedienteImportacion\.Primario, RolExpedienteImportacion\.Secundario/);
});

test("cada item conserva inscripción tipología expediente y rol", () => {
  for (const value of cases) {
    const result = build(value);
    for (const [index, destination] of result.destinations.entries()) {
      assert.deepEqual(destination, {
        client: value.items[index].client,
        key: value.items[index].key,
        type: value.items[index].type,
        expedient: value.inscriptions[index].expedient,
        role: value.roles[index]
      });
    }
  }
  assert.match(planSource, /\.IdTipoDocumental = item\.IdTipoDocumental/);
  assert.match(planSource, /\.IdExpediente = inscription\.IdExpediente\.Value/);
});

test("item sin inscripción o expediente bloquea el plan", () => {
  const missingKey = build({ ...cases[0], items: [{ client: "x", key: "missing", type: 11 }] });
  const missingExpedient = build({ ...cases[0], inscriptions: [{ key: "m1", expedient: null }] });
  assert.equal(missingKey.confirmed, false);
  assert.equal(missingExpedient.confirmed, false);
  assert.match(planSource, /Not byKey\.TryGetValue/);
  assert.match(planSource, /Not inscription\.IdExpediente\.HasValue/);
});
