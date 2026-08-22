const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const test = require("node:test");
const vm = require("node:vm");

const source = fs.readFileSync(path.resolve(__dirname, "../js/workflow/workflow-user-send-confirmation.js"), "utf8");
const dialogSource = fs.readFileSync(path.resolve(__dirname, "../js/java_general/ConfirmationDialog.js"), "utf8");

function selection() {
    return {
        idTarea: 41,
        idUsuarioWorkflowDestino: 7,
        idActividadDestino: 9,
        tokenVersion: "v-41",
        contexto: { radicado: "RAD-41", grupoActual: "Gestión" },
        destino: { nombreUsuarioDestino: "Usuario destino", cargoUsuarioDestino: "Analista", nombreActividadDestino: "Revisión" }
    };
}

function loadConfirmation() {
    const listeners = new Map();
    const window = {
        addEventListener(name, callback) { listeners.set(name, callback); },
        ConfirmationDialog: {
            open(config) { window.openedConfig = config; },
            close() { window.closed = true; }
        },
        WorkflowUserSendUi: {
            aplicarEnvioExitoso(value) { window.appliedSelection = value; }
        },
        WorkflowTransitionPagePresentation: {
            applySuccess(value, options) { window.presentation = { value, options }; }
        }
    };
    vm.runInNewContext(source, { window, Promise, JSON, Array, Object, Number, String, Error, isFinite });
    return { api: window.WorkflowUserSendConfirmation, window, listeners };
}

test("confirma y ejecuta solo el contrato usuario–actividad–token", async () => {
    const { api, window } = loadConfirmation();
    let call;
    api.fetchImplementation = async (url, options) => {
        call = { url, options };
        return { ok: true, json: async () => ({ d: { Exito: true, MensajeFuncional: "Tarea enviada.", TokenVersion: "v-41", ReferenciaAuditoria: "WF-USR-41", Advertencias: [] } }) };
    };

    assert.equal(api.openFromSelection(selection()), true);
    assert.deepEqual(JSON.parse(JSON.stringify(window.openedConfig.executionContext)), {
        idTarea: 41, idUsuarioWorkflowDestino: 7, idActividadDestino: 9, tokenVersion: "v-41"
    });
    const raw = await window.openedConfig.execute(window.openedConfig.executionContext);
    const result = window.openedConfig.normalizeResult(raw);
    window.openedConfig.onSuccess(result);

    assert.match(call.url, /EjecutarEnvioUsuario$/);
    assert.equal(call.options.credentials, "same-origin");
    assert.deepEqual(JSON.parse(call.options.body), {
        idTarea: 41, idUsuarioWorkflowDestino: 7, idActividadDestino: 9, tokenVersion: "v-41"
    });
    assert.equal(result.status, "success");
    assert.deepEqual(JSON.parse(JSON.stringify(window.appliedSelection)), selection());
    assert.equal(window.presentation.options.successElementId, "workflow-user-send-success-message");
    assert.equal(window.closed, true);
});

test("clasifica bloqueo, descarta una selección inválida e invalida la confirmación abierta", () => {
    const { api, window, listeners } = loadConfirmation();
    assert.equal(api.openFromSelection({ idTarea: 41 }), false);
    assert.equal(api.openFromSelection(selection()), true);
    const blocked = api.normalizeResult({ d: { Exito: false, CodigoBloqueo: "WORKFLOW_VERSION_CONFLICT", MensajeFuncional: "La tarea cambió.", EsReintentable: true } });
    assert.equal(blocked.status, "blocked");
    assert.equal(blocked.canRetry, true);
    listeners.get("workflow:user-destination-invalidated")({ detail: { idTarea: 41 } });
    assert.equal(window.closed, true);
});

test("la confirmación queda aislada de Continuar flujo y de rutas legacy", () => {
    assert.match(source, /workflow:user-destination-selected/);
    assert.match(source, /workflow:user-destination-invalidated/);
    assert.match(source, /ConfirmationDialog/);
    assert.doesNotMatch(source, /IdConector|WorkflowTransitionUi|workflow:destination-selected|Terminar_Tarea_Workflow|Cambia_Estado|After_envio_usuario_workflow|Reasigna_respuesta_envia_tarea_usuario/);
});

test("el diálogo reutilizado bloquea doble clic mientras la ejecución está en curso", () => {
    assert.match(source, /ConfirmationDialog\.open/);
    assert.match(dialogSource, /if \(!context \|\| context\.sending\) \{\s*return;/);
    assert.match(dialogSource, /context\.sending = true/);
});
