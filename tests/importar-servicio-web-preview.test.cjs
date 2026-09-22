const test = require("node:test");
const assert = require("node:assert/strict");

const state = require("../js/workflow/importar-servicio-web/importar-servicio-web-preview-state.js");
const preview = require("../js/workflow/importar-servicio-web/importar-servicio-web-preview.js");

test("la máquina de estados cubre disponibilidad, fallback, expiración y bloqueo", () => {
  let current = state.initial();
  current = state.move(current, state.states.preparing, { externalKey: "sii-1" });
  current = state.move(current, state.states.available, { descriptorId: "a".repeat(32), contentType: "application/pdf" });
  assert.equal(current.state, "disponible");
  current = state.move(current, state.states.expired, { externalKey: "sii-1" });
  assert.equal(current.state, "recurso-vencido");
  current = state.move(current, state.states.preparing, { externalKey: "sii-1" });
  current = state.move(current, state.states.unsupported, { contentType: "application/octet-stream" });
  assert.equal(current.state, "formato-no-visualizable");
  assert.throws(() => state.move(current, state.states.available, {}), /PREVIEW_STATE_TRANSITION_INVALID/);
});

test("una apertura concurrente comparte la solicitud y renovar crea otra", async () => {
  let calls = 0;
  let resolveRequest;
  const api = { getPreview() { calls += 1; return new Promise(resolve => { resolveRequest = resolve; }); } };
  const control = preview.create({ api, state });
  const item = { externalKey: "sii-1" };
  const first = control.open(item, { ProviderId: "INTEGRACIONSII" });
  const second = control.open(item, { ProviderId: "INTEGRACIONSII" });
  assert.equal(first, second);
  assert.equal(calls, 1);
  resolveRequest({ DescriptorId: "a".repeat(32), ContentType: "application/pdf", ExpiresAtUtc: "2026-09-22T12:00:00Z" });
  assert.equal((await first).state, "disponible");
  const renewed = control.renew(item, { ProviderId: "INTEGRACIONSII" });
  assert.equal(calls, 2);
  resolveRequest({ DescriptorId: "b".repeat(32), ContentType: "application/zip" });
  assert.equal((await renewed).state, "formato-no-visualizable");
  assert.match(control.snapshot().resourcePath, /ImportarServicioWebPreview\.ashx\?d=/);
});

test("errores de backend se traducen a estados cerrados", async () => {
  const cases = [["PREVIEW_FORBIDDEN", "no-autorizado"], ["PREVIEW_EXPIRY_PAST", "recurso-vencido"], ["FEATURE_DISABLED", "bloqueado"], ["EXTERNAL_PROVIDER_UNAVAILABLE", "proveedor-no-disponible"]];
  for (const [code, expected] of cases) {
    const control = preview.create({ api: { getPreview() { return Promise.resolve({ Error: { Codigo: code } }); } }, state });
    assert.equal((await control.open({ externalKey: "sii-1" }, {})).state, expected);
  }
});

test("cerrar es idempotente e invalida la vista actual", async () => {
  const control = preview.create({ api: { getPreview() { return Promise.resolve({ DescriptorId: "c".repeat(32), ContentType: "image/tiff" }); } }, state });
  assert.equal(control.close().state, "cerrado");
  assert.equal((await control.open({ externalKey: "sii-1" }, {})).state, "formato-no-visualizable");
  assert.equal(control.close().state, "cerrado");
  assert.equal(control.close().state, "cerrado");
});
