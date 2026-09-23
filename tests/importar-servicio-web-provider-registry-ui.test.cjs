const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const test = require("node:test");
const api = require("../js/workflow/importar-servicio-web/importar-servicio-web-api.js");
const registryFactory = require("../js/workflow/importar-servicio-web/importar-servicio-web-provider-registry.js");

test("normaliza identidad y publica capacidades sin compartir mutaciones", () => {
    const registry = registryFactory.create({ knownNotMigrated: ["legacy"] });
    registry.register(" demo ", { capabilities: { multipleSelection: true, allowedActions: ["query"] } });
    const first = registry.resolve("DEMO");
    first.capabilities.allowedActions.push("mutated");
    assert.equal(first.found, true);
    assert.deepEqual(registry.resolve("demo").capabilities.allowedActions, ["query"]);
    assert.equal(registry.resolve("").code, "PROVIDER_NOT_CONFIGURED");
    assert.equal(registry.resolve("LEGACY").code, "PROVIDER_NOT_MIGRATED");
    assert.equal(registry.resolve("unknown").code, "PROVIDER_NOT_SUPPORTED");
});

test("cliente API usa exclusivamente envelopes ASMX y transporte inyectado", async () => {
    let call;
    const client = api.create({ transport: async (url, options) => { call = { url, options }; return { ok: true, json: async () => ({ d: JSON.stringify({ Items: [] }) }) }; } });
    const result = await client.queryItems({ TaskId: 72, ProviderId: "DEMO" });
    assert.deepEqual(result, { Items: [] });
    assert.match(call.url, /QueryItems$/);
    assert.deepEqual(JSON.parse(call.options.body), { request: { TaskId: 72, ProviderId: "DEMO" } });
    assert.throws(() => api.unwrapAsmx({}), /IMPORT_RESPONSE_INVALID/);
});

test("integración WebForms conserva legacy y activa la experiencia moderna global", () => {
    const page = fs.readFileSync(path.resolve(__dirname, "../workflow/Webworkflow.aspx"), "utf8");
    const codeBehind = fs.readFileSync(path.resolve(__dirname, "../workflow/Webworkflow.aspx.vb"), "utf8");
    const config = fs.readFileSync(path.resolve(__dirname, "../Web.config"), "utf8");
    const ui = fs.readFileSync(path.resolve(__dirname, "../js/workflow/importar-servicio-web/importar-servicio-web-ui.js"), "utf8");
    assert.match(config, /WorkflowCentroTrabajoModernActive" value="true"/);
    assert.match(config, /WorkflowCentroTrabajoModernUsers" value=""/);
    assert.match(config, /WorkflowCentroTrabajoModernGroups" value=""/);
    assert.match(page, /id="btnloadservice"/);
    assert.match(page, /id="ctw-document-action-service"/);
    assert.match(page, /id="importar-servicio-web-modal"/);
    assert.match(codeBehind, /If Not WorkflowTransitionModernActive Then[\s\S]*?Return[\s\S]*?RegisterImportarServicioWebModernAssets/);
    assert.match(ui, /legacy\.hidden = true/);
    assert.doesNotMatch(ui, /fetch\s*\(|XMLHttpRequest|\$\.ajax/);
});
