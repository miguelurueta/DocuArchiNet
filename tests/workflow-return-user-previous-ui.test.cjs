const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const test = require("node:test");
const vm = require("node:vm");

const root = path.resolve(__dirname, "..");
const read = (...parts) => fs.readFileSync(path.join(root, ...parts), "utf8");
const uiSource = read("js", "workflow", "workflow-return-user-previous-ui.js");
const confirmationSource = read("js", "workflow", "workflow-return-user-previous-confirmation.js");
const page = read("workflow", "Webworkflow.aspx");
const codeBehind = read("workflow", "Webworkflow.aspx.vb");
const designer = read("workflow", "Webworkflow.aspx.designer.vb");
const legacyScript = read("js", "workflow", "Webworkflow.js");
const styles = read("Styles", "workflow-transition-modern.css");
const project = read("GestionDocumental-Docuarchi.net.vbproj");

function loadUi() {
    const events = [];
    const document = {
        createEvent() { return { initCustomEvent(name, bubbles, cancelable, detail) { this.name = name; this.detail = detail; } }; },
        createElement() { return {}; },
        getElementById() { return null; }
    };
    const window = {
        CustomEvent: function CustomEvent(name, options) { this.name = name; this.detail = options.detail; },
        dispatchEvent(event) { events.push(event); }
    };
    vm.runInNewContext(uiSource, { window, document, Promise, JSON, Array, Object, Number, String, Error, isFinite });
    return { api: window.WorkflowReturnUserPreviousUi, events };
}

function loadConfirmation() {
    const window = { addEventListener() {} };
    vm.runInNewContext(confirmationSource, { window, Promise, JSON, Array, Object, Number, String, Error, isFinite });
    return window.WorkflowReturnUserPreviousConfirmation;
}

test("DOC-37: normaliza solo el usuario histórico y token opaco del preview", () => {
    const { api } = loadUi();
    const preview = api.desempaquetarRespuestaAsmx({ d: {
        IdTarea: 843,
        Contexto: { ActividadActual: "Contabilidad", ActividadAnterior: "Revisión", UsuarioAnterior: "Ana Pérez" },
        TokenVersion: "opaque-token"
    } });

    assert.deepEqual(JSON.parse(JSON.stringify(preview)), {
        idTarea: 843,
        contexto: { actividadActual: "Contabilidad", actividadAnterior: "Revisión", usuarioAnterior: "Ana Pérez" },
        tokenVersion: "opaque-token",
        error: null
    });
    assert.deepEqual(JSON.parse(JSON.stringify(api.crearDetalleSeleccion(preview))), {
        idTarea: 843,
        tokenVersion: "opaque-token",
        contexto: { actividadActual: "Contabilidad", actividadAnterior: "Revisión", usuarioAnterior: "Ana Pérez" }
    });
});

test("DOC-37: el preview y la ejecución envían únicamente el contrato mínimo", async () => {
    const { api } = loadUi();
    const confirmation = loadConfirmation();
    let previewCall;
    let executeCall;

    await api.solicitarPrevisualizacion(843, async (url, options) => {
        previewCall = { url, options };
        return { ok: true, json: async () => ({ d: { IdTarea: 843, Contexto: {}, TokenVersion: "opaque-token" } }) };
    });
    await confirmation.execute({ idTarea: 843, tokenVersion: "opaque-token" }, async (url, options) => {
        executeCall = { url, options };
        return { ok: true, json: async () => ({ d: { Exito: true, MensajeFuncional: "Correcto" } }) };
    });

    assert.match(previewCall.url, /PreviewDevolverUsuarioAnterior$/);
    assert.deepEqual(JSON.parse(previewCall.options.body), { idTarea: 843 });
    assert.match(executeCall.url, /EjecutarDevolverUsuarioAnterior$/);
    assert.deepEqual(JSON.parse(executeCall.options.body), { idTarea: 843, tokenVersion: "opaque-token" });
});

test("DOC-37: la ruta moderna es exclusiva, accesible y no depende del feature gate", () => {
    const presentation = codeBehind.match(/Private Sub RegisterWorkflowReturnUserPreviousModernPresentation\(\)[\s\S]*?End Sub/);
    assert.ok(presentation, "Falta el registro de presentación exclusivo.");
    assert.match(page, /<button id="workflow-return-user-previous-trigger" type="button"[^>]*data-workflow-return-user-previous-active="false"/);
    assert.match(page, /workflow-return-user-previous-modern-modal/);
    assert.match(page, /workflow-return-user-previous-modern-confirm/);
    assert.match(codeBehind, /RegisterWorkflowReturnUserPreviousModernPresentation\(\)/);
    assert.match(codeBehind, /workflow-return-user-previous-ui\.js/);
    assert.match(codeBehind, /workflow-return-user-previous-confirmation\.js/);
    assert.doesNotMatch(presentation[0], /WorkflowCentroTrabajoModernActive|WorkflowTransitionModernActive/);
    assert.match(styles, /#workflow-return-user-previous-modern-dialog/);
    assert.match(styles, /data-workflow-return-user-previous-status="error"/);
    assert.match(uiSource, /event\.key === "Escape"/);
    assert.match(uiSource, /event\.shiftKey/);
    assert.match(uiSource, /AbortController/);
    assert.match(uiSource, /requestTimeoutMilliseconds = 15000/);
    assert.match(confirmationSource, /requestTimeoutMilliseconds = 15000/);
    assert.match(confirmationSource, /establecerEjecucionPendiente/);
    assert.match(confirmationSource, /WorkflowTransitionPagePresentation\.applySuccess/);
    assert.match(project, /js\\workflow\\workflow-return-user-previous-ui\.js/);
    assert.match(project, /js\\workflow\\workflow-return-user-previous-confirmation\.js/);
});

test("DOC-37: elimina el fallback Web Forms y no mezcla otras operaciones", () => {
    for (const source of [page, codeBehind, designer, legacyScript]) {
        assert.doesNotMatch(source, /D-TWU-ANT|Button_tool_devolver_a_usuario|Devolver_tarea_workflow_usuario_anterior/);
    }
    for (const source of [uiSource, confirmationSource]) {
        assert.doesNotMatch(source, /WorkflowReturnActivity|PreviewDevolverActividad|EjecutarDevolverActividad|PreviewEnviar|EjecutarEnvio|workflow:return-activity|workflow:user-destination/);
    }
});
