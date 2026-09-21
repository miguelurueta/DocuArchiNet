const assert = require("node:assert/strict");
const test = require("node:test");
const core = require("../js/workflow/importar-servicio-web/importar-servicio-web-core.js");

function registry(adapter) {
    return { resolve(id) { return id === "DEMO" ? { found: true, providerId: id, adapter } : { found: false, code: "PROVIDER_NOT_SUPPORTED" }; } };
}

test("recorre estados cerrados y rechaza saltos inválidos", async () => {
    const machine = core.create({ registry: registry({ queryItems: async () => ({ Items: [{ DisplayName: "Uno" }] }), executeImportIntent: async () => ({ Items: [] }) }) });
    const states = [];
    machine.subscribe(snapshot => states.push(snapshot.state));
    await machine.open("DEMO");
    assert.deepEqual(states, ["cerrado", "resolviendo-proveedor", "consultando", "resultados"]);
    assert.throws(() => machine.transition("reconciliando"), /IMPORT_STATE_TRANSITION_INVALID/);
});

test("una intención concurrente produce una sola ejecución global", async () => {
    let calls = 0;
    let release;
    const pending = new Promise(resolve => { release = resolve; });
    const machine = core.create({ registry: registry({ queryItems: async () => ({ Items: [{}] }), executeImportIntent: async () => { calls += 1; await pending; return { Items: [{ Status: "Disponible" }] }; } }) });
    await machine.open("DEMO");
    const first = machine.execute({ IntentId: "I-1" });
    const second = machine.execute({ IntentId: "I-1" });
    assert.equal(first, second);
    assert.equal(machine.getState().state, "ejecutando");
    release();
    const result = await first;
    assert.equal(calls, 1);
    assert.equal(result.state, "completado");
});

test("proveedor no soportado falla cerrado sin consultar", async () => {
    const machine = core.create({ registry: registry({}) });
    const result = await machine.open("UNKNOWN");
    assert.equal(result.state, "error");
    assert.equal(result.message, "PROVIDER_NOT_SUPPORTED");
});
