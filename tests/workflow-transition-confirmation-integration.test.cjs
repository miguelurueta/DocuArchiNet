const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const test = require("node:test");
const vm = require("node:vm");

const sourcePath = path.resolve(__dirname, "../js/workflow/workflow-transition-confirmation-integration.js");
const source = fs.readFileSync(sourcePath, "utf8");

function loadIntegration() {
    const listeners = new Map();
    const dispatched = [];
    const document = {
        createEvent() {
            return { initCustomEvent(name, bubbles, cancelable, detail) { this.name = name; this.detail = detail; } };
        }
    };
    const window = {
        document,
        CustomEvent: function CustomEvent(name, options) { this.name = name; this.detail = options.detail; },
        addEventListener(name, callback) { listeners.set(name, callback); },
        dispatchEvent(event) { dispatched.push(event); },
        ConfirmationDialog: {
            open(config) { window.openedConfig = config; },
            close() { window.closed = true; }
        },
        WorkflowTransitionUi: {
            aplicarTransicionExitosa(selection) { window.appliedSelection = selection; }
        },
        WorkflowTransitionPagePresentation: {
            applySuccess(selection) { window.pageAppliedSelection = selection; }
        }
    };
    vm.runInNewContext(source, { window, Promise, JSON, Array, Object, Number, String, Error, isFinite });
    return { api: window.WorkflowTransitionConfirmationIntegration, window, listeners, dispatched };
}

function detail() {
    return {
        idTarea: 41,
        idConector: 8,
        tokenVersion: "v-41",
        tipoDecision: "FLUJO",
        contexto: { radicado: "RAD-41", grupoActual: "Gestión" },
        destino: { nombre: "Revisión", destinatario: "Ana", grupo: "", tipo: "Flujo" }
    };
}

test("mapea la selección permitida a un config de Workflow", () => {
    const { api, window } = loadIntegration();
    assert.equal(api.openFromSelection(detail()), true);
    assert.equal(window.openedConfig.title, "Enviar tarea");
    assert.equal(window.openedConfig.primaryLabel, "Enviar a Revisión");
    assert.deepEqual(JSON.parse(JSON.stringify(window.openedConfig.executionContext)), {
        idTarea: 41, idConector: 8, tokenVersion: "v-41"
    });
    assert.deepEqual(JSON.parse(JSON.stringify(window.openedConfig.summaryFields)), [
        { label: "Radicado", value: "RAD-41" },
        { label: "Tipo", value: "FLUJO" },
        { label: "Grupo actual", value: "Gestión" },
        { label: "Actividad destino", value: "Revisión" },
        { label: "Destinatario o grupo", value: "Ana" },
        { label: "Mecanismo", value: "Flujo" }
    ]);
    assert.doesNotMatch(source, /Hidden_id_tarea|Terminar_Tarea_Workflow|Cambia_Estado/);
});

test("solicita solo la terna de ejecución al ASMX y normaliza el resultado", async () => {
    const { api, window } = loadIntegration();
    let call;
    api.fetchImplementation = async (url, options) => {
        call = { url, options };
        return {
            ok: true,
            json: async () => ({ d: {
                Exito: true,
                MensajeFuncional: "Tarea enviada.",
                TokenVersion: "v-41",
                ReferenciaAuditoria: "WF-MOD-41",
                Advertencias: []
            } })
        };
    };
    api.openFromSelection(detail());
    const raw = await window.openedConfig.execute(window.openedConfig.executionContext);
    const result = window.openedConfig.normalizeResult(raw);

    assert.match(call.url, /EjecutarEnvioTarea$/);
    assert.equal(call.options.credentials, "same-origin");
    assert.deepEqual(JSON.parse(call.options.body), { idTarea: 41, idConector: 8, tokenVersion: "v-41" });
    assert.equal(result.status, "success");
    assert.equal(result.reference, "WF-MOD-41");
});

test("clasifica bloqueo, error reintentable y respuestas de otro token", () => {
    const { api, window } = loadIntegration();
    assert.deepEqual(JSON.parse(JSON.stringify(api.normalizeResult({ d: {
        Exito: false, EstadoFinal: "bloqueado", MensajeFuncional: "No permitido.", EsReintentable: false, TokenVersion: "v-41"
    } }))), {
        status: "blocked", message: "No permitido.", warnings: [], canRetry: false, reference: "", tokenVersion: "v-41", raw: {
            Exito: false, EstadoFinal: "bloqueado", MensajeFuncional: "No permitido.", EsReintentable: false, TokenVersion: "v-41"
        }
    });
    assert.equal(api.normalizeResult({ d: {
        Exito: false, MensajeFuncional: "Intente luego.", EsReintentable: true, TokenVersion: "v-41"
    } }).status, "technical-error");

    api.openFromSelection(detail());
    const ignored = window.openedConfig.normalizeResult({ d: { Exito: true, TokenVersion: "v-otro" } });
    assert.equal(ignored.status, "ignored");
});

test("publica éxito solo para la selección correlacionada", () => {
    const { api, window, dispatched } = loadIntegration();
    let callbackSelection;
    api.onSuccess = (selection) => { callbackSelection = selection; };
    api.openFromSelection(detail());
    const result = window.openedConfig.normalizeResult({ d: {
        Exito: true, MensajeFuncional: "Tarea enviada.", TokenVersion: "v-41", ReferenciaAuditoria: "WF-MOD-41"
    } });
    window.openedConfig.onSuccess(result);

    assert.deepEqual(JSON.parse(JSON.stringify(window.pageAppliedSelection)), detail());
    assert.deepEqual(JSON.parse(JSON.stringify(window.appliedSelection)), detail());
    assert.deepEqual(JSON.parse(JSON.stringify(callbackSelection)), detail());
    assert.equal(window.closed, true);
    assert.equal(dispatched[0].name, "workflow:transition-succeeded");
    assert.equal(dispatched[0].detail.reference, "WF-MOD-41");
});
