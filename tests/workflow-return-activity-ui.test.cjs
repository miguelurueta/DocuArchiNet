const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const test = require("node:test");
const vm = require("node:vm");

const source = fs.readFileSync(path.resolve(__dirname, "../js/workflow/workflow-return-activity-ui.js"), "utf8");
const page = fs.readFileSync(path.resolve(__dirname, "../workflow/Webworkflow.aspx"), "utf8");
const codeBehind = fs.readFileSync(path.resolve(__dirname, "../workflow/Webworkflow.aspx.vb"), "utf8");
const styles = fs.readFileSync(path.resolve(__dirname, "../Styles/workflow-transition-modern.css"), "utf8");

function loadUi() {
    const events = [];
    const document = {
        createEvent() { return { initCustomEvent(name, bubbles, cancelable, detail) { this.name = name; this.detail = detail; } }; },
        createElement() { return {}; },
        getElementById() { return null; }
    };
    const window = { CustomEvent: function CustomEvent(name, options) { this.name = name; this.detail = options.detail; }, dispatchEvent(event) { events.push(event); } };
    vm.runInNewContext(source, { window, document, Promise, JSON, Array, Object, Number, String, Error, isFinite });
    return { api: window.WorkflowReturnActivityUi, events };
}

test("normaliza el preview de devolución sin identidad de Ruta o Flujo", () => {
    const { api } = loadUi();
    const preview = api.desempaquetarRespuestaAsmx({ d: {
        IdTarea: 843,
        Contexto: { Radicado: "RAD-843", ActividadActual: "Contador", GrupoActual: "Contabilidad", TipoContexto: "Ruta" },
        TokenVersion: "v-843",
        CursorSiguiente: "cursor-2",
        HayMas: true,
        TamanoPagina: 25,
        Destinos: [{ IdConector: 7, NombreActividad: "Supervisor", Destinatario: "Ana", TipoContexto: "Ruta", Orden: 1 }]
    } }, 1);

    assert.deepEqual(JSON.parse(JSON.stringify(preview)), {
        idTarea: 843,
        contexto: { radicado: "RAD-843", actividadActual: "Contador", grupoActual: "Contabilidad", tipoContexto: "Ruta" },
        destinos: [{ idConector: 7, nombreActividad: "Supervisor", destinatario: "Ana", tipoContexto: "Ruta", orden: 1 }],
        tokenVersion: "v-843", cursorSiguiente: "cursor-2", hayMas: true, tamanoPagina: 25, pagina: 1, error: null
    });
    assert.deepEqual(JSON.parse(JSON.stringify(api.crearDetalleSeleccion(preview, preview.destinos[0]))), {
        idTarea: 843, idConector: 7, tokenVersion: "v-843",
        contexto: { radicado: "RAD-843", actividadActual: "Contador", grupoActual: "Contabilidad", tipoContexto: "Ruta" },
        destino: { nombreActividad: "Supervisor", destinatario: "Ana", tipoContexto: "Ruta" }
    });
});

test("consulta el preview con el payload mínimo paginado", async () => {
    const { api } = loadUi();
    let call;
    await api.solicitarPrevisualizacion(843, "su", "cursor-1", 25, async (url, options) => {
        call = { url, options };
        return { ok: true, json: async () => ({ d: { IdTarea: 843, Contexto: {}, Destinos: [] } }) };
    }, null, 2);
    assert.match(call.url, /PreviewDevolverActividad$/);
    assert.equal(call.options.credentials, "same-origin");
    assert.deepEqual(JSON.parse(call.options.body), { idTarea: 843, termino: "su", cursor: "cursor-1", tamanoPagina: 25 });
});

test("mantiene estado aislado con debounce, cursor, cancelación y descarte obsoleto", () => {
    assert.match(source, /minimumSearchLength = 2/);
    assert.match(source, /searchDelayMilliseconds = 300/);
    assert.match(source, /AbortController/);
    assert.match(source, /requestSequence \+= 1/);
    assert.match(source, /control\.preview = null/);
    assert.match(source, /sequence !== requestSequence/);
    assert.match(source, /cursorHistory/);
    assert.match(source, /workflow:return-activity-selected/);
    assert.match(source, /workflow:return-activity-invalidated/);
    assert.doesNotMatch(source, /WorkflowUserSendUi|WorkflowTransitionUi|workflow:user-destination|PreviewEnviarUsuario|PreviewEnviarTarea/);
});

test("el markup y bootstrap son exclusivos, accesibles y sin postback legacy", () => {
    assert.match(page, /<button id="workflow-return-activity-trigger" type="button"[^>]*data-workflow-return-activity-active="false"/);
    assert.match(page, /workflow-return-activity-modern-modal/);
    assert.match(page, /workflow-return-activity-modern-search/);
    assert.doesNotMatch(page, /D-TASK-ANT|Button_tool_devolver_a_actividades_anterior/);
    assert.match(codeBehind, /RegisterWorkflowReturnActivityModernPresentation\(\)/);
    assert.match(codeBehind, /workflow-return-activity-ui\.js/);
    assert.match(codeBehind, /workflow-return-activity-confirmation\.js/);
    assert.doesNotMatch(codeBehind, /Button_tool_devolver_a_actividades_anterior|Activa_devolver_actividades_anteriores/);
    assert.match(styles, /#workflow-return-activity-modern-dialog/);
    assert.match(styles, /data-workflow-return-activity-status="error"/);
    assert.match(source, /event\.key === "Escape"/);
    assert.match(source, /event\.shiftKey/);
    assert.match(source, /aplicarDevolucionExitosa/);
    assert.match(source, /executionPending/);
    assert.match(source, /establecerEjecucionPendiente/);
    assert.match(source, /La devolución está en curso\. Espere la respuesta antes de cerrar\./);
});
