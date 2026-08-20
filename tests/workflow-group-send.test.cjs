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
const stylesSource = fs.readFileSync(path.resolve(__dirname, "../Styles/workflow-transition-modern.css"), "utf8");

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
        pagina: 1,
        tamanoPagina: 25,
        tieneMas: false,
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

test("normaliza y solicita una página de búsqueda de grupo con el contrato mínimo", async () => {
    const ui = loadUi();
    let call;
    const search = ui.desempaquetarBusquedaAsmx({ d: {
        IdTarea: 41,
        TokenVersion: "v-41",
        Pagina: 2,
        TamanoPagina: 25,
        TieneMas: true,
        Destinos: [{ IdActividadDestino: 10, NombreActividad: "Archivo", GrupoDestino: "2 grupos asociados" }]
    } });
    await ui.solicitarBusqueda(41, "archivo", 2, 25, async (url, options) => {
        call = { url, options };
        return { ok: true, json: async () => ({ d: {
            IdTarea: 41, TokenVersion: "v-41", Pagina: 2, TamanoPagina: 25, TieneMas: true, Destinos: []
        } }) };
    });

    assert.deepEqual(JSON.parse(JSON.stringify(search)), {
        idTarea: 41,
        destinos: [{ idActividadDestino: 10, nombreActividad: "Archivo", grupoDestino: "2 grupos asociados", orden: 26 }],
        tokenVersion: "v-41",
        pagina: 2,
        tamanoPagina: 25,
        tieneMas: true,
        error: null
    });
    assert.match(call.url, /BuscarDestinosEnvioGrupo$/);
    assert.equal(call.options.credentials, "same-origin");
    assert.deepEqual(JSON.parse(call.options.body), { idTarea: 41, termino: "archivo", pagina: 2, tamanoPagina: 25 });
});

test("no enlaza el control moderno de grupo si el bootstrap no está activo", () => {
    const trigger = {
        getAttribute(name) { return name === "data-workflow-group-modern-active" ? "false" : ""; }
    };
    loadUi(trigger);
    assert.equal(trigger.onclick, undefined);
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
    assert.match(interfacesSource, /Interface IEnvioGrupoBusquedaRepository/);
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
    assert.match(repositorySource, /BuscarDestinos/);
    assert.match(repositorySource, /EXISTS \(SELECT 1 FROM grupos_workflow AS grupoFiltro/);
    assert.match(repositorySource, /GROUP BY actividad\.ID_ACTIVIDAD/);
    assert.match(repositorySource, /LIMIT @limite OFFSET @desplazamiento/);
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
    assert.match(executorSource, /Terminar_Tarea_Workflow\([\s\S]*?\r?\n\s*If\(destino\.RequiereNotificacion, 1, 0\),\r?\n\s*0,\r?\n\s*contexto\.IdUsuarioWorkflow,\r?\n\s*tarea\.IdActividadOrigen/);
});

test("el ASMX compone operaciones de grupo sin exponer errores técnicos ni invocar el motor", () => {
    assert.match(asmxSource, /Public Function PreviewEnviarGrupo\(ByVal idTarea As Long\)[\s\S]*?AsegurarContextoEnvioGrupo\(\)/);
    assert.match(asmxSource, /Public Function BuscarDestinosEnvioGrupo\(ByVal idTarea As Long,[\s\S]*?ByVal termino As String,[\s\S]*?ByVal pagina As Integer,[\s\S]*?ByVal tamanoPagina As Integer\)/);
    assert.match(asmxSource, /Public Function EjecutarEnvioGrupo\(ByVal idTarea As Long,[\s\S]*?ByVal idActividadDestino As Integer,[\s\S]*?ByVal tokenVersion As String\)/);
    assert.match(asmxSource, /AsegurarContextoEnvioGrupo\(True\)/);
    assert.match(asmxSource, /New WorkflowLegacyEnvioGrupoExecutorAdapter\(\)/);
    assert.doesNotMatch(asmxSource, /Terminar_Tarea_Workflow/);
    assert.doesNotMatch(asmxSource, /Catch ex As Exception[\s\S]{0,120}ex\.Message/);
});

test("la presentación moderna de grupo no deja un acceso legacy cuando el contexto no enlaza operaciones", () => {
    assert.match(pageSource, /<button id="workflow-group-send-trigger" type="button"[^>]*?WorkflowCentroTrabajoModernOperationDisabledAttribute[^>]*?>/);
    assert.match(pageSource, /<button id="workflow-transition-trigger" type="button"[^>]*?WorkflowCentroTrabajoModernOperationDisabledAttribute[^>]*?>/);
    assert.doesNotMatch(pageSource, /activa_boton_client_server\('ImageButton(?:EnviaActividad|terminar)'\)/);
    assert.match(pageSource, /workflow-group-send-modern-modal/);
    assert.match(codeBehindSource, /RegisterWorkflowEnvioGrupoModernScript\(\)/);
    assert.match(codeBehindSource, /workflow-group-send-ui\.js\?v=20260820-doc26search1/);
    assert.match(codeBehindSource, /data-workflow-group-modern-active/);
    assert.match(codeBehindSource, /Hidden_id_tarea_selecionada\.ClientID/);
    assert.match(fs.readFileSync(path.resolve(__dirname, "workflow-transition-ui.test.cjs"), "utf8"), /PreviewEnviarTarea/);
    assert.match(fs.readFileSync(path.resolve(__dirname, "workflow-transition-confirmation-integration.test.cjs"), "utf8"), /idConector/);
});

test("la búsqueda conserva una representación responsive sincronizada para tabla y tarjetas", () => {
    assert.match(pageSource, /<label for="workflow-group-send-modern-search">Buscar actividad o grupo<\/label>/);
    assert.match(pageSource, /id="workflow-group-send-modern-previous"/);
    assert.match(pageSource, /id="workflow-group-send-modern-next"/);
    assert.match(uiSource, /control\.tableBody\.appendChild\(destinationRow/);
    assert.match(uiSource, /control\.cards\.appendChild\(destinationCard/);
    assert.match(uiSource, /searchDelayMilliseconds = 300/);
    assert.match(uiSource, /minimumSearchLength = 2/);
    assert.match(uiSource, /sequence !== requestSequence/);
    assert.match(stylesSource, /@media \(max-width: 767px\)[\s\S]*?workflow-transition-modal__desktop[\s\S]*?display: none/);
    assert.match(stylesSource, /@media \(max-width: 767px\)[\s\S]*?workflow-transition-modal__mobile[\s\S]*?display: grid/);
    assert.match(stylesSource, /workflow-transition-modal__pager/);
});

test("un cambio de búsqueda invalida la confirmación abierta", () => {
    const { api, window, listeners } = loadConfirmation();
    assert.equal(api.openFromSelection(selection()), true);
    listeners.get("workflow:group-destination-invalidated")({ detail: { idTarea: 41 } });
    assert.equal(window.closed, true);
});
