const test = require("node:test");
const assert = require("node:assert/strict");

function createHarness(overrides = {}) {
  const state = { expedients: new Map(), stored: new Map(), related: new Map(), links: new Map(), cache: new Map(), indices: new Map(), xml: new Set(), calls: [] };
  const ports = {
    persistLogical: overrides.persistLogical ?? (() => true),
    persistPhysical: overrides.persistPhysical ?? (() => true),
    link: overrides.link ?? ((image, expedient) => (state.links.set(image, expedient), true)),
    index: overrides.index ?? ((image) => (state.indices.set(image, true), state.xml.add(image), true)),
  };
  function execute(input) {
    const key = `${input.cabinet}|${input.inscription}`;
    let expedient = state.expedients.get(key);
    if (!expedient) state.expedients.set(key, expedient = state.expedients.size + 501);
    state.calls.push("expedient");
    if (!ports.persistLogical(input.intent, expedient)) return { status: "Parcial", code: "LOGICAL_WRITER_UNAVAILABLE" };
    for (const item of input.items) if (!state.stored.has(item.clientId)) state.stored.set(item.clientId, item.imageId);
    state.calls.push("storage");
    for (const image of input.universe) {
      const id = image.imageId;
      if (!state.related.has(id)) state.related.set(id, { imageId: id, expedient });
      if (!ports.persistPhysical(input.intent, state.related.get(id))) return { status: "Parcial", code: "PHYSICAL_WRITER_UNAVAILABLE" };
      if (state.links.get(id) !== expedient && !ports.link(id, expedient)) return { status: "Parcial", code: "LINK_UNAVAILABLE" };
      state.cache.set(`${input.task}|${id}|${input.cabinet}`, expedient);
      if ((!state.indices.get(id) || !state.xml.has(id)) && !ports.index(id)) return { status: "Parcial", code: "INDEX_UNAVAILABLE" };
    }
    state.calls.push("related");
    return { status: "Completada", expedient };
  }
  return { state, execute };
}

function canonicalEnrollment(cabinet, enrollment, proponent = "") {
  const source = cabinet === "RUP" ? proponent : enrollment;
  return source.replace(/[^0-9]/g, "").replace(/^0+/, "");
}

const input = { intent: "i-1", task: 219887, cabinet: "MERCANTIL", inscription: "book|registration", items: [{ clientId: "a", imageId: 10 }, { clientId: "b", imageId: 11 }], universe: [{ imageId: 9 }, { imageId: 10 }, { imageId: 11 }] };

test("saga local completa expediente almacenamiento ENLASE vínculo caché índices y XML", () => {
  const h = createHarness();
  assert.equal(h.execute(input).status, "Completada");
  assert.deepEqual(h.state.calls, ["expedient", "storage", "related"]);
  assert.equal(h.state.expedients.size, 1);
  assert.equal(h.state.stored.size, 2);
  assert.equal(h.state.related.size, 3);
  assert.equal(h.state.links.size, 3);
  assert.equal(h.state.cache.size, 3);
  assert.equal(h.state.xml.size, 3);
});

test("saga completa cubre MERCANTIL ESAL y RUP con su identidad canónica", () => {
  const cases = [
    { cabinet: "MERCANTIL", enrollment: "RM001234A", proponent: "", expected: "1234" },
    { cabinet: "ESAL", enrollment: "S000987", proponent: "", expected: "987" },
    { cabinet: "RUP", enrollment: "IGNORED", proponent: "RP000567A", expected: "567" },
  ];
  for (const [index, value] of cases.entries()) {
    const identity = canonicalEnrollment(value.cabinet, value.enrollment, value.proponent);
    assert.equal(identity, value.expected, value.cabinet);
    const h = createHarness();
    const result = h.execute({
      ...input,
      intent: `matrix-${value.cabinet}`,
      task: input.task + index,
      cabinet: value.cabinet,
      inscription: identity,
      items: [{ clientId: `${value.cabinet}-item`, imageId: 100 + index }],
      universe: [{ imageId: 90 + index }, { imageId: 100 + index }],
    });
    assert.equal(result.status, "Completada", value.cabinet);
    assert.deepEqual(h.state.calls, ["expedient", "storage", "related"], value.cabinet);
    assert.equal(h.state.expedients.size, 1, value.cabinet);
    assert.equal(h.state.stored.size, 1, value.cabinet);
    assert.equal(h.state.links.size, 2, value.cabinet);
    assert.equal(h.state.cache.size, 2, value.cabinet);
    assert.equal(h.state.indices.size, 2, value.cabinet);
    assert.equal(h.state.xml.size, 2, value.cabinet);
  }
});

test("reintento conserva efectos y procesa solo un IdImagen nuevo", () => {
  const h = createHarness();
  const first = h.execute(input);
  const retried = h.execute({ ...input, universe: [...input.universe, { imageId: 12 }] });
  assert.equal(first.expedient, retried.expedient);
  assert.equal(h.state.expedients.size, 1);
  assert.equal(h.state.stored.size, 2);
  assert.equal(h.state.links.size, 4);
});

test("la saga falla cerrada si falta cualquier escritor obligatorio", () => {
  assert.equal(createHarness({ persistLogical: () => false }).execute(input).code, "LOGICAL_WRITER_UNAVAILABLE");
  assert.equal(createHarness({ persistPhysical: () => false }).execute(input).code, "PHYSICAL_WRITER_UNAVAILABLE");
  assert.equal(createHarness({ link: () => false }).execute(input).code, "LINK_UNAVAILABLE");
  assert.equal(createHarness({ index: () => false }).execute(input).code, "INDEX_UNAVAILABLE");
});
