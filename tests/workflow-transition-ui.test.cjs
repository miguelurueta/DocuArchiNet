const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const test = require("node:test");
const vm = require("node:vm");

const sourcePath = path.resolve(__dirname, "../js/workflow/workflow-transition-ui.js");
const source = fs.readFileSync(sourcePath, "utf8");
const transitionStylePath = path.resolve(__dirname, "../Styles/workflow-transition-modern.css");
const transitionStyle = fs.readFileSync(transitionStylePath, "utf8");
const workflowPagePath = path.resolve(__dirname, "../workflow/Webworkflow.aspx");
const workflowCodeBehindPath = path.resolve(__dirname, "../workflow/Webworkflow.aspx.vb");

function loadUi(trigger) {
    const document = {
        readyState: "complete",
        addEventListener() {},
        getElementById(id) {
            return id === "workflow-transition-trigger" ? trigger : null;
        }
    };
    const window = {
        CustomEvent: function CustomEvent(name, options) {
            this.name = name;
            this.detail = options.detail;
        },
        dispatchEvent() {},
        setTimeout(callback) { callback(); }
    };

    vm.runInNewContext(source, { window, document, JSON, Array, Object, Number, String, Error, isFinite });
    return window.WorkflowTransitionUi;
}

test("normaliza el contrato ASMX sin derivar campos no publicados", () => {
    const ui = loadUi();
    const preview = ui.desempaquetarRespuestaAsmx({
        d: {
            IdTarea: 41,
            TipoDecision: "Flujo documental",
            Contexto: { Radicado: "RAD-41", GrupoActual: "Gestión", ActividadOrigen: "No usar" },
            Tramite: "No usar",
            TokenVersion: "v-41",
            Destinos: [{ Id: 8, Nombre: "Revisión", Destinatario: "Ana", Grupo: "", Tipo: "Flujo", Orden: 2 }]
        }
    });

    assert.equal(preview.idTarea, 41);
    assert.equal(preview.contexto.radicado, "RAD-41");
    assert.equal(preview.contexto.grupoActual, "Gestión");
    assert.equal(preview.destinos[0].id, 8);
    assert.equal(preview.destinos[0].nombre, "Revisión");
    assert.equal(Object.prototype.hasOwnProperty.call(preview.contexto, "actividadOrigen"), false);
    assert.equal(Object.prototype.hasOwnProperty.call(preview, "tramite"), false);
});

test("rechaza un envoltorio ASMX inválido y conserva errores de red controlables", async () => {
    const ui = loadUi();

    assert.throws(() => ui.desempaquetarRespuestaAsmx({}), /envoltorio ASMX esperado/);
    await assert.rejects(
        () => ui.solicitarPrevisualizacion(41, async () => ({ ok: false, status: 500, json: async () => ({}) })),
        /HTTP 500/
    );

    await assert.rejects(
        () => ui.solicitarPrevisualizacion(41, async () => ({ ok: true, json: async () => { throw new Error("JSON"); } })),
        /respuesta no válida/
    );
});

test("conserva el mensaje funcional seguro devuelto por el ASMX", () => {
    const ui = loadUi();
    const preview = ui.desempaquetarRespuestaAsmx({
        d: {
            IdTarea: 41,
            Contexto: {},
            Destinos: [],
            Error: {
                Codigo: "WORKFLOW_TASK_UNAVAILABLE",
                MensajeVisible: "La tarea no está disponible para envío."
            }
        }
    });

    assert.deepEqual(JSON.parse(JSON.stringify(preview.error)), {
        codigo: "WORKFLOW_TASK_UNAVAILABLE",
        mensajeVisible: "La tarea no está disponible para envío."
    });
});

test("solicita solo PreviewEnviarTarea con la tarea visual", async () => {
    const ui = loadUi();
    let call;

    await ui.solicitarPrevisualizacion(41, async (url, options) => {
        call = { url, options };
        return {
            ok: true,
            json: async () => ({ d: { IdTarea: 41, Contexto: {}, Destinos: [] } })
        };
    });

    assert.match(call.url, /PreviewEnviarTarea$/);
    assert.equal(call.options.credentials, "same-origin");
    assert.deepEqual(JSON.parse(call.options.body), { idTarea: 41 });
    assert.doesNotMatch(source, /EjecutarEnvioTarea/);
});

test("no reemplaza el enlace legacy cuando el bootstrap está inactivo", () => {
    const legacyClick = () => "legacy";
    const trigger = {
        onclick: legacyClick,
        getAttribute(name) {
            return name === "data-workflow-modern-active" ? "false" : "";
        }
    };

    loadUi(trigger);
    assert.equal(trigger.onclick, legacyClick);
});

test("registra recursos DOC-12 y DOC-14 sin bloques de servidor en la cabecera Web Forms", () => {
    const page = fs.readFileSync(workflowPagePath, "utf8");
    const codeBehind = fs.readFileSync(workflowCodeBehindPath, "utf8");
    const head = page.match(/<head\b[^>]*>[\s\S]*?<\/head>/i)[0];

    assert.doesNotMatch(head, /<%/);
    assert.doesNotMatch(page, /WorkflowModernPresentationBootstrap/);
    assert.match(codeBehind, /Page\.Header\.Controls\.Add\(style\)/);
    assert.match(codeBehind, /Page\.Header\.Controls\.Add\(script\)/);
    assert.match(codeBehind, /data-workflow-modern-active/);
    assert.match(codeBehind, /Hidden_id_tarea_selecionada\.ClientID/);
    assert.match(codeBehind, /ScriptManager\.RegisterStartupScript/);
    assert.match(source, /data-workflow-current-task-input-id/);
    assert.match(source, /Sys\.Application\.add_load/);
    assert.match(source, /data-workflow-modern-bound/);
    assert.match(source, /getClientRects\(\)\.length > 0/);
    assert.match(codeBehind, /workflow-transition-modern\.css\?v=20260816-doc12qa5/);
    assert.match(codeBehind, /confirmation-dialog\.css\?v=20260818-doc14fullflow1/);
    assert.match(codeBehind, /ConfirmationDialog\.js\?v=20260818-doc14fullflow1/);
    assert.match(codeBehind, /workflow-transition-confirmation-integration\.js\?v=20260818-doc14fullflow1/);
    assert.match(codeBehind, /workflow-transition-page-presentation\.js\?v=20260818-doc14fullflow1/);
    assert.match(page, /data-workflow-task-list="true"/);
    assert.match(page, /data-workflow-transition-success="true"/);
});

test("mantiene la cabecera y el cierre visibles en el modal móvil", () => {
    assert.match(transitionStyle, /height: 100dvh/);
    assert.match(transitionStyle, /\.workflow-transition-modal__header \{[\s\S]*?flex: 0 0 auto/);
    assert.match(transitionStyle, /\.workflow-transition-modal__close \{[\s\S]*?flex: 0 0 2\.5rem/);
    assert.match(transitionStyle, /\.workflow-transition-modal__body \{[\s\S]*?overflow-y: auto/);
});

test("la selección publica solamente el contrato para la confirmación posterior", () => {
    const ui = loadUi();
    const detail = ui.crearDetalleSeleccion(
        {
            idTarea: 41,
            tokenVersion: "v-41",
            tipoDecision: "Flujo",
            contexto: { radicado: "RAD-41", grupoActual: "Gestión documental" }
        },
        { id: 8, nombre: "Revisión", destinatario: "Ana", grupo: "", tipo: "Flujo" }
    );

    assert.deepEqual(JSON.parse(JSON.stringify(detail)), {
        idTarea: 41,
        idConector: 8,
        tokenVersion: "v-41",
        tipoDecision: "Flujo",
        contexto: { radicado: "RAD-41", grupoActual: "Gestión documental" },
        destino: { nombre: "Revisión", destinatario: "Ana", grupo: "", tipo: "Flujo" }
    });
});
