const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const test = require("node:test");
const vm = require("node:vm");

const source = fs.readFileSync(path.resolve(__dirname, "../js/workflow/workflow-return-activity-confirmation.js"), "utf8");
const dialog = fs.readFileSync(path.resolve(__dirname, "../js/java_general/ConfirmationDialog.js"), "utf8");

function selection() {
    return { idTarea: 843, idConector: 7, tokenVersion: "v-843", destino: { nombreActividad: "Supervisor", destinatario: "Ana", tipoContexto: "Ruta" } };
}
function loadConfirmation() {
    const listeners = new Map();
    const window = {
        addEventListener(name, callback) { listeners.set(name, callback); },
        ConfirmationDialog: { open(config) { window.opened = config; }, close() { window.closed = true; } },
        WorkflowReturnActivityUi: { aplicarDevolucionExitosa(detail) { window.returnActivityUi = detail; } },
        WorkflowTransitionPagePresentation: { applySuccess(detail, options) { window.presentation = { detail, options }; } }
    };
    vm.runInNewContext(source, { window, Promise, JSON, Array, Object, Number, String, Error, isFinite });
    return { api: window.WorkflowReturnActivityConfirmation, window, listeners };
}

test("confirma y ejecuta solo tarea, conector y token", async () => {
    const { api, window } = loadConfirmation();
    let call;
    api.fetchImplementation = async (url, options) => {
        call = { url, options };
        return { ok: true, json: async () => ({ d: { Exito: true, MensajeFuncional: "Tarea devuelta." } }) };
    };
    assert.equal(api.openFromSelection(selection()), true);
    assert.deepEqual(JSON.parse(JSON.stringify(window.opened.executionContext)), { idTarea: 843, idConector: 7, tokenVersion: "v-843" });
    const result = window.opened.normalizeResult(await window.opened.execute());
    window.opened.onSuccess(result);
    assert.match(call.url, /EjecutarDevolverActividad$/);
    assert.equal(call.options.credentials, "same-origin");
    assert.deepEqual(JSON.parse(call.options.body), { idTarea: 843, idConector: 7, tokenVersion: "v-843" });
    assert.equal(result.status, "success");
    assert.deepEqual(JSON.parse(JSON.stringify(window.returnActivityUi)), selection());
    assert.deepEqual(JSON.parse(JSON.stringify(window.presentation.detail)), { idTarea: 843 });
    assert.equal(window.presentation.options.successElementId, "workflow-return-activity-success-message");
    assert.equal(window.closed, true);
});

test("clasifica bloqueo e invalida confirmación cuando cambia el preview", () => {
    const { api, window, listeners } = loadConfirmation();
    assert.equal(api.openFromSelection({ idTarea: 843 }), false);
    const blocked = api.normalizeResult({ d: { Exito: false, CodigoBloqueo: "WORKFLOW_RETURN_VERSION_CONFLICT", MensajeFuncional: "La tarea cambió.", EsReintentable: true } });
    assert.equal(blocked.status, "blocked");
    assert.equal(blocked.canRetry, true);
    listeners.get("workflow:return-activity-invalidated")({ detail: { idTarea: 843 } });
    assert.equal(window.closed, true);
});

test("permanece aislada de otras transiciones y usa el bloqueo compartido de doble envío", () => {
    assert.match(source, /workflow:return-activity-selected/);
    assert.match(source, /workflow:return-activity-invalidated/);
    assert.match(source, /ConfirmationDialog/);
    assert.doesNotMatch(source, /WorkflowUserSend|WorkflowTransitionUi|PreviewEnviar|EjecutarEnvio|Activa_devolver_actividades_anteriores/);
    assert.match(dialog, /if \(!context \|\| context\.sending\) \{\s*return;/);
    assert.match(dialog, /context\.sending = true/);
});
