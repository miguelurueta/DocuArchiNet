const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const test = require("node:test");
const vm = require("node:vm");

const source = fs.readFileSync(path.resolve(__dirname, "../js/workflow/workflow-user-send-ui.js"), "utf8");
const page = fs.readFileSync(path.resolve(__dirname, "../workflow/Webworkflow.aspx"), "utf8");
const codeBehind = fs.readFileSync(path.resolve(__dirname, "../workflow/Webworkflow.aspx.vb"), "utf8");
const styles = fs.readFileSync(path.resolve(__dirname, "../Styles/workflow-transition-modern.css"), "utf8");
const visualLayer = fs.readFileSync(path.resolve(__dirname, "../js/workflow/centro-trabajo-visual.js"), "utf8");

function loadUi(trigger) {
    const events = [];
    const document = {
        readyState: "complete",
        addEventListener() {},
        createEvent() { return { initCustomEvent(name, bubbles, cancelable, detail) { this.name = name; this.detail = detail; } }; },
        getElementById(id) { return id === "workflow-user-send-trigger" ? trigger : null; }
    };
    const window = {
        CustomEvent: function CustomEvent(name, options) { this.name = name; this.detail = options.detail; },
        dispatchEvent(event) { events.push(event); },
        setTimeout(callback) { callback(); return 1; },
        clearTimeout() {}
    };
    vm.runInNewContext(source, { window, document, Promise, JSON, Array, Object, Number, String, Error, isFinite });
    return { api: window.WorkflowUserSendUi, events };
}

test("normaliza el preview paginado de usuario sin conector", () => {
    const { api } = loadUi();
    const preview = api.desempaquetarRespuestaAsmx({ d: {
        IdTarea: 41,
        Contexto: { Radicado: "RAD-41", GrupoActual: "Gestión" },
        TokenVersion: "v-41",
        CursorSiguiente: "cursor-2",
        TieneMas: true,
        TamanoPagina: 25,
        Destinos: [{
            IdUsuarioWorkflowDestino: 7,
            IdActividadDestino: 9,
            NombreUsuarioDestino: "Usuario destino",
            CargoUsuarioDestino: "Analista",
            NombreActividadDestino: "Revisión"
        }]
    } });
    const detail = api.crearDetalleSeleccion(preview, preview.destinos[0]);

    assert.deepEqual(JSON.parse(JSON.stringify(preview)), {
        idTarea: 41,
        contexto: { radicado: "RAD-41", grupoActual: "Gestión" },
        destinos: [{
            idUsuarioWorkflowDestino: 7,
            idActividadDestino: 9,
            nombreUsuarioDestino: "Usuario destino",
            cargoUsuarioDestino: "Analista",
            nombreActividadDestino: "Revisión",
            orden: 1
        }],
        tokenVersion: "v-41",
        cursorSiguiente: "cursor-2",
        tieneMas: true,
        tamanoPagina: 25,
        pagina: 1,
        error: null
    });
    assert.deepEqual(JSON.parse(JSON.stringify(detail)), {
        idTarea: 41,
        idUsuarioWorkflowDestino: 7,
        idActividadDestino: 9,
        tokenVersion: "v-41",
        contexto: { radicado: "RAD-41", grupoActual: "Gestión" },
        destino: {
            nombreUsuarioDestino: "Usuario destino",
            cargoUsuarioDestino: "Analista",
            nombreActividadDestino: "Revisión"
        }
    });
    assert.doesNotMatch(source, /IdConector|WorkflowTransitionUi|workflow:destination-selected|PreviewEnviarTarea|EjecutarEnvioTarea/);
});

test("consulta PreviewEnviarUsuario con el contrato paginado mínimo", async () => {
    const { api } = loadUi();
    let call;
    await api.solicitarPrevisualizacion(41, "ana", "cursor-1", 25, async (url, options) => {
        call = { url, options };
        return { ok: true, json: async () => ({ d: { IdTarea: 41, Contexto: {}, Destinos: [] } }) };
    });

    assert.match(call.url, /PreviewEnviarUsuario$/);
    assert.equal(call.options.credentials, "same-origin");
    assert.deepEqual(JSON.parse(call.options.body), { idTarea: 41, consulta: "ana", cursor: "cursor-1", tamanoPagina: 25 });
});

test("el adaptador conserva cursor, debounce e invalidación de respuestas obsoletas", () => {
    assert.match(source, /searchDelayMilliseconds = 300/);
    assert.match(source, /minimumSearchLength = 2/);
    assert.match(source, /cursorHistory/);
    assert.match(source, /preview\.cursorSiguiente/);
    assert.match(source, /sequence !== requestSequence/);
    assert.match(source, /workflow:user-destination-invalidated/);
    assert.match(source, /workflow:user-destination-selected/);
});

test("el modal conserva teclado, foco y cierre propios", () => {
    assert.match(source, /event\.key === "Escape"/);
    assert.match(source, /event\.shiftKey/);
    assert.match(source, /\(control\.search \|\| control\.close\)\.focus\(\)/);
    assert.match(source, /data-workflow-user-send-close/);
});

test("no enlaza el trigger de usuario sin bootstrap y el marcado no ofrece fallback legacy", () => {
    const trigger = { getAttribute(name) { return name === "data-workflow-user-send-active" ? "false" : ""; } };
    loadUi(trigger);
    assert.equal(trigger.onclick, undefined);
    assert.match(page, /<button id="workflow-user-send-trigger" type="button"[^>]*?>/);
    assert.match(page, /workflow-user-send-modern-modal/);
    assert.match(page, /workflow-user-send-modern-search/);
    assert.match(page, /centro-trabajo-visual\.js\?v=20260821-modern-actions4/);
    assert.match(styles, /data-workflow-user-send-status="error"/);
    assert.match(styles, /data-workflow-user-send-status="exito"/);
    assert.match(styles, /#workflow-user-send-modern-dialog\s*\{[\s\S]*?height:\s*42rem;/);
    assert.match(styles, /#workflow-user-send-modern-dialog \.workflow-transition-modal__body\s*\{[\s\S]*?overflow-y:\s*scroll;/);
    assert.doesNotMatch(page, /ImageButtonEnviarUsuario|Button_tool_enviar_usuario|ModalPopupExtender_edition_lista_usuarios_ruta/);
    assert.match(codeBehind, /RegisterWorkflowEnvioUsuarioModernPresentation\(\)[\s\S]*?RegisterWorkflowEnvioUsuarioModernBootstrap\(\)/);
    assert.match(codeBehind, /workflow-user-send-ui\.js\?v=20260821-doc29ui1/);
    assert.match(codeBehind, /workflow-transition-modern\.css\?v=20260821-doc29ui2/);
    assert.match(codeBehind, /data-workflow-user-send-active/);
    assert.match(visualLayer, /#nav_menu #workflow-user-send-trigger[\s\S]*?ctw-action-slot--terminal/);
    assert.match(visualLayer, /#nav_menu #workflow-user-send-trigger[\s\S]*?ctw-action-slot--handoff/);
    assert.match(visualLayer, /#nav_menu #workflow-user-send-trigger[\s\S]*?ctw-action-slot--handoff-user/);
});
