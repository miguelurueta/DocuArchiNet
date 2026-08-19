const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const test = require("node:test");
const vm = require("node:vm");

const uiSource = fs.readFileSync(path.resolve(__dirname, "../js/workflow/workflow-group-send-ui.js"), "utf8");
const confirmationSource = fs.readFileSync(path.resolve(__dirname, "../js/workflow/workflow-group-send-confirmation.js"), "utf8");
const asmxSource = fs.readFileSync(path.resolve(__dirname, "../webservice/WebServiceWorkflowModern.asmx.vb"), "utf8");
const serviceSource = fs.readFileSync(path.resolve(__dirname, "../Services/Workflow/Terminar/ServicioEnvioGrupoTarea.vb"), "utf8");
const repositorySource = fs.readFileSync(path.resolve(__dirname, "../Infrastructure/Repositories/Workflow/MySqlEnvioGrupoRepository.vb"), "utf8");
const executorSource = fs.readFileSync(path.resolve(__dirname, "../Infrastructure/Workflow/Terminar/WorkflowLegacyEnvioGrupoExecutorAdapter.vb"), "utf8");
const requirementsSource = fs.readFileSync(path.resolve(__dirname, "../Infrastructure/Workflow/Terminar/WorkflowLegacyEnvioGrupoRequisitosAdapter.vb"), "utf8");
const modelsSource = fs.readFileSync(path.resolve(__dirname, "../Modelo/Workflow/Terminar/WorkflowModernModels.vb"), "utf8");
const interfacesSource = fs.readFileSync(path.resolve(__dirname, "../Modelo/Workflow/Terminar/WorkflowModernInterfaces.vb"), "utf8");
const pageSource = fs.readFileSync(path.resolve(__dirname, "../workflow/Webworkflow.aspx"), "utf8");
const codeBehindSource = fs.readFileSync(path.resolve(__dirname, "../workflow/Webworkflow.aspx.vb"), "utf8");

function loadUi(trigger) {
    const document = {
        readyState: "complete",
        addEventListener() {},
        getElementById(id) { return id === "workflow-group-send-trigger" ? trigger : null; }
    };
    const window = {
        CustomEvent: function CustomEvent(name, options) { this.name = name; this.detail = options.detail; },
        dispatchEvent() {},
        setTimeout(callback) { callback(); }
    };
    vm.runInNewContext(uiSource, { window, document, Promise, JSON, Array, Object, Number, String, Error, isFinite });
    return window.WorkflowGroupSendUi;
}

function loadConfirmation() {
    const listeners = new Map();
    const window = {
        addEventListener(name, callback) { listeners.set(name, callback); },
        ConfirmationDialog: {
            open(config) { window.openedConfig = config; },
            close() { window.closed = true; }
        },
        WorkflowGroupSendUi: {
            aplicarEnvioExitoso(selection) { window.appliedSelection = selection; }
        },
        WorkflowTransitionPagePresentation: {
            applySuccess(selection) { window.pageAppliedSelection = selection; }
        }
    };
    vm.runInNewContext(confirmationSource, { window, Promise, JSON, Array, Object, Number, String, Error, isFinite });
    return { api: window.WorkflowGroupSendConfirmation, window, listeners };
}

function selection() {
    return {
        idTarea: 41,
        idActividadDestino: 9,
        tokenVersion: "v-41",
        contexto: { radicado: "RAD-41", grupoActual: "Gestión" },
        destino: { nombreActividad: "Revisión", grupoDestino: "Grupo revisor" }
    };
}

test("normaliza el preview de grupo y publica una selección sin conector", () => {
    const ui = loadUi();
    const preview = ui.desempaquetarRespuestaAsmx({ d: {
        IdTarea: 41,
        Contexto: { Radicado: "RAD-41", GrupoActual: "Gestión", ActividadOrigen: "No publicar" },
        TokenVersion: "v-41",
        Destinos: [{ IdActividadDestino: 9, NombreActividad: "Revisión", GrupoDestino: "Grupo revisor" }]
    } });
    const detail = ui.crearDetalleSeleccion(preview, preview.destinos[0]);

    assert.deepEqual(JSON.parse(JSON.stringify(preview)), {
        idTarea: 41,
        contexto: { radicado: "RAD-41", grupoActual: "Gestión" },
        destinos: [{ idActividadDestino: 9, nombreActividad: "Revisión", grupoDestino: "Grupo revisor", orden: 1 }],
        tokenVersion: "v-41",
        error: null
    });
    assert.deepEqual(JSON.parse(JSON.stringify(detail)), selection());
    assert.doesNotMatch(uiSource, /idConector/);
});

test("solicita exclusivamente PreviewEnviarGrupo por POST autenticado", async () => {
    const ui = loadUi();
    let call;
    await ui.solicitarPrevisualizacion(41, async (url, options) => {
        call = { url, options };
        return { ok: true, json: async () => ({ d: { IdTarea: 41, Contexto: {}, Destinos: [] } }) };
    });

    assert.match(call.url, /PreviewEnviarGrupo$/);
    assert.equal(call.options.credentials, "same-origin");
    assert.deepEqual(JSON.parse(call.options.body), { idTarea: 41 });
    assert.doesNotMatch(uiSource, /EjecutarEnvioGrupo/);
});

test("preserva el enlace legacy de grupo si el bootstrap no está activo", () => {
    const legacyClick = () => "legacy";
    const trigger = {
        onclick: legacyClick,
        getAttribute(name) { return name === "data-workflow-group-modern-active" ? "false" : ""; }
    };
    loadUi(trigger);
    assert.equal(trigger.onclick, legacyClick);
});

test("confirma y ejecuta con la terna directa de grupo", async () => {
    const { api, window } = loadConfirmation();
    let call;
    api.fetchImplementation = async (url, options) => {
        call = { url, options };
        return { ok: true, json: async () => ({ d: {
            Exito: true, MensajeFuncional: "Tarea enviada.", TokenVersion: "v-41", ReferenciaAuditoria: "WF-GRP-41", Advertencias: []
        } }) };
    };
    assert.equal(api.openFromSelection(selection()), true);
    assert.deepEqual(JSON.parse(JSON.stringify(window.openedConfig.executionContext)), {
        idTarea: 41, idActividadDestino: 9, tokenVersion: "v-41"
    });
    const raw = await window.openedConfig.execute(window.openedConfig.executionContext);
    const result = window.openedConfig.normalizeResult(raw);

    assert.match(call.url, /EjecutarEnvioGrupo$/);
    assert.equal(call.options.credentials, "same-origin");
    assert.deepEqual(JSON.parse(call.options.body), { idTarea: 41, idActividadDestino: 9, tokenVersion: "v-41" });
    assert.equal(result.status, "success");
    assert.doesNotMatch(confirmationSource, /idConector|Terminar_Tarea_Workflow|Cambia_Estado/);
});

test("los contratos de grupo no relajan el contrato existente por conector", () => {
    const requestBlock = modelsSource.match(/Public Class SolicitudEnvioGrupoWorkflow[\s\S]*?End Class/)[0];
    const destinationBlock = modelsSource.match(/Public Class DestinoEnvioGrupoWorkflow[\s\S]*?End Class/)[0];

    assert.match(requestBlock, /IdTarea/);
    assert.match(requestBlock, /IdActividadDestino/);
    assert.match(requestBlock, /TokenVersion/);
    assert.doesNotMatch(requestBlock, /IdConector|Page|Session/);
    assert.doesNotMatch(destinationBlock, /IdConector/);
    assert.match(interfacesSource, /Interface IEnvioGrupoDestinosRepository/);
    assert.match(interfacesSource, /Interface IEnvioGrupoEjecucionRepository/);
    assert.match(interfacesSource, /Interface IEnvioGrupoRequisitosRepository/);
    assert.match(interfacesSource, /Interface IEnvioGrupoLegacyExecutor/);
    assert.match(modelsSource, /Public Class SolicitudTransicionWorkflow[\s\S]*?IdConector/);
});

test("el preview de grupo usa solamente lecturas y devuelve el destino de la ruta actual", () => {
    assert.match(repositorySource, /SELECT TIPO_RUTA_ABIERTA_CERRADA/);
    assert.match(repositorySource, /SELECT estado_ruta_open_close/);
    assert.match(repositorySource, /FROM LISTADO_ACTIVIDADES_WORKFLOW[\s\S]*?RUTAS_WORKFLOW_ID_RUTA = @idRuta/);
    assert.match(repositorySource, /AND actividad\.ID_ACTIVIDAD = @idActividadDestino/);
    assert.doesNotMatch(repositorySource, /\b(?:INSERT|UPDATE|DELETE|CALL)\b/i);
});

test("la ejecución revalida dentro del guard y conserva los requisitos directos de grupo", () => {
    const guardPosition = serviceSource.indexOf("Using guard.Lease");
    const permissionPosition = serviceSource.indexOf("If Not TieneCambioRuta(contexto)", guardPosition);
    const taskPosition = serviceSource.indexOf("_tareaRepository.ObtenerTarea", guardPosition);
    const destinationPosition = serviceSource.indexOf("_ejecucionRepository.ResolverDestino", guardPosition);
    const requirementsPosition = serviceSource.indexOf("_requisitosRepository.Evaluar", guardPosition);
    const executorPosition = serviceSource.indexOf("_ejecutor.Ejecutar", guardPosition);

    assert.ok(guardPosition >= 0 && permissionPosition > guardPosition && taskPosition > permissionPosition);
    assert.ok(destinationPosition > taskPosition && requirementsPosition > destinationPosition && executorPosition > requirementsPosition);
    assert.match(requirementsSource, /Verifica_solicitudes_de_aprobacion_sin_desicion/);
    assert.match(requirementsSource, /CodigosBloqueoPrevisualizacion\.AprobacionPendiente/);
    assert.doesNotMatch(requirementsSource, /Classgestionrespuesta|respuesta radicada/i);
    assert.match(executorSource, /Dim pagina As System\.Web\.UI\.Page = Nothing/);
    assert.match(executorSource, /Terminar_Tarea_Workflow\([\s\S]*?\n\s*0,\n\s*\n?\s*contexto\.IdUsuarioWorkflow/);
});

test("el ASMX compone operaciones de grupo sin exponer errores técnicos ni invocar el motor", () => {
    assert.match(asmxSource, /Public Function PreviewEnviarGrupo\(ByVal idTarea As Long\)[\s\S]*?AsegurarContextoEnvioGrupo\(\)/);
    assert.match(asmxSource, /Public Function EjecutarEnvioGrupo\(ByVal idTarea As Long,[\s\S]*?ByVal idActividadDestino As Integer,[\s\S]*?ByVal tokenVersion As String\)/);
    assert.match(asmxSource, /AsegurarContextoEnvioGrupo\(True\)/);
    assert.match(asmxSource, /New WorkflowLegacyEnvioGrupoExecutorAdapter\(\)/);
    assert.doesNotMatch(asmxSource, /Terminar_Tarea_Workflow/);
    assert.doesNotMatch(asmxSource, /Catch ex As Exception[\s\S]{0,120}ex\.Message/);
});

test("la presentación moderna de grupo queda aislada tras el gate y no cambia Continuar flujo", () => {
    assert.match(pageSource, /If WorkflowCentroTrabajoModernActive Then[\s\S]*?workflow-group-send-trigger[\s\S]*?Else[\s\S]*?activa_boton_client_server\('ImageButtonEnviaActividad'\)/);
    assert.match(pageSource, /workflow-group-send-modern-modal/);
    assert.match(codeBehindSource, /RegisterWorkflowEnvioGrupoModernScript\(\)/);
    assert.match(codeBehindSource, /workflow-group-send-ui\.js\?v=20260819-doc15base1/);
    assert.match(codeBehindSource, /data-workflow-group-modern-active/);
    assert.match(codeBehindSource, /Hidden_id_tarea_selecionada\.ClientID/);
    assert.match(fs.readFileSync(path.resolve(__dirname, "workflow-transition-ui.test.cjs"), "utf8"), /PreviewEnviarTarea/);
    assert.match(fs.readFileSync(path.resolve(__dirname, "workflow-transition-confirmation-integration.test.cjs"), "utf8"), /idConector/);
});
