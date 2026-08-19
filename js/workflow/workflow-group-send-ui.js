(function (window, document) {
    "use strict";

    var previewUrl = "../webservice/WebServiceWorkflowModern.asmx/PreviewEnviarGrupo";
    var requestSequence = 0;
    var activeControl = null;
    var api = {};

    function isObject(value) { return value !== null && typeof value === "object" && !Array.isArray(value); }
    function asText(value, fallback) {
        var text = value === undefined || value === null ? "" : String(value).replace(/^\s+|\s+$/g, "");
        return text || (fallback || "");
    }
    function asPositiveInteger(value) {
        var parsed = Number(value);
        return isFinite(parsed) && parsed > 0 && Math.floor(parsed) === parsed ? parsed : 0;
    }
    function asArray(value) { return Array.isArray(value) ? value : []; }
    function createElement(tagName, className, text) {
        var element = document.createElement(tagName);
        if (className) { element.className = className; }
        if (text !== undefined) { element.textContent = text; }
        return element;
    }
    function empty(node) { while (node && node.firstChild) { node.removeChild(node.firstChild); } }

    function normalizeDestination(raw, index) {
        raw = isObject(raw) ? raw : {};
        return {
            idActividadDestino: asPositiveInteger(raw.IdActividadDestino),
            nombreActividad: asText(raw.NombreActividad, "Actividad disponible"),
            grupoDestino: asText(raw.GrupoDestino, "No especificado"),
            orden: index + 1
        };
    }

    function normalizePreview(raw) {
        var context;
        var error;
        if (!isObject(raw)) { throw new Error("La respuesta de previsualización no tiene el formato esperado."); }
        context = isObject(raw.Contexto) ? raw.Contexto : {};
        error = isObject(raw.Error) ? raw.Error : null;
        return {
            idTarea: asPositiveInteger(raw.IdTarea),
            contexto: {
                radicado: asText(context.Radicado, "No disponible"),
                grupoActual: asText(context.GrupoActual, "No disponible")
            },
            destinos: asArray(raw.Destinos).map(normalizeDestination),
            tokenVersion: asText(raw.TokenVersion),
            error: error ? {
                codigo: asText(error.Codigo),
                mensajeVisible: asText(error.MensajeVisible, "No fue posible cargar las actividades.")
            } : null
        };
    }

    function unwrapAsmx(raw) {
        var value;
        if (!isObject(raw) || !Object.prototype.hasOwnProperty.call(raw, "d")) {
            throw new Error("La respuesta del servicio no contiene el envoltorio ASMX esperado.");
        }
        value = raw.d;
        if (typeof value === "string") {
            try { value = JSON.parse(value); } catch (ignored) { throw new Error("La respuesta del servicio no contiene JSON válido."); }
        }
        return normalizePreview(value);
    }

    function requestPreview(idTarea, fetchImplementation) {
        fetchImplementation = fetchImplementation || window.fetch;
        if (typeof fetchImplementation !== "function") {
            return Promise.reject(new Error("Este navegador no permite cargar actividades de forma segura."));
        }
        return fetchImplementation(previewUrl, {
            method: "POST",
            credentials: "same-origin",
            headers: { "Content-Type": "application/json; charset=utf-8" },
            body: JSON.stringify({ idTarea: idTarea })
        }).then(function (response) {
            if (!response || response.ok === false) {
                throw new Error("No fue posible consultar las actividades disponibles.");
            }
            return response.json();
        }).then(unwrapAsmx);
    }

    function addDefinition(container, label, value) {
        container.appendChild(createElement("dt", "workflow-transition-modal__context-label", label));
        container.appendChild(createElement("dd", "workflow-transition-modal__context-value", value));
    }
    function setStatus(control, state, message, kind) {
        control.modal.setAttribute("data-workflow-transition-state", state);
        control.status.setAttribute("data-workflow-transition-status", kind || "informacion");
        control.status.textContent = message || "";
    }
    function renderContext(control, preview) {
        empty(control.contexto);
        addDefinition(control.contexto, "Radicado", preview.contexto.radicado);
        addDefinition(control.contexto, "Grupo actual", preview.contexto.grupoActual);
    }
    function clearDestinations(control) { empty(control.tableBody); empty(control.cards); }

    function selectedDetail(preview, destination) {
        return {
            idTarea: preview.idTarea,
            idActividadDestino: destination.idActividadDestino,
            tokenVersion: preview.tokenVersion,
            contexto: {
                radicado: preview.contexto.radicado,
                grupoActual: preview.contexto.grupoActual
            },
            destino: {
                nombreActividad: destination.nombreActividad,
                grupoDestino: destination.grupoDestino
            }
        };
    }
    function dispatchSelection(control, destination) {
        var detail = selectedDetail(control.preview, destination);
        var event;
        if (typeof api.onDestinationSelected === "function") {
            try { api.onDestinationSelected(detail); } catch (ignored) {}
        }
        if (typeof window.CustomEvent === "function") {
            event = new window.CustomEvent("workflow:group-destination-selected", { detail: detail });
        } else {
            event = document.createEvent("CustomEvent");
            event.initCustomEvent("workflow:group-destination-selected", false, false, detail);
        }
        window.dispatchEvent(event);
        setStatus(control, "destino-seleccionado", "Actividad seleccionada: " + destination.nombreActividad + ".", "exito");
    }
    function selectButton(control, destination) {
        var button = createElement("button", "workflow-transition-modal__select", "Seleccionar");
        button.type = "button";
        button.disabled = destination.idActividadDestino <= 0;
        button.addEventListener("click", function () {
            if (control.preview && destination.idActividadDestino > 0) { dispatchSelection(control, destination); }
        });
        return button;
    }
    function destinationRow(control, destination) {
        var row = document.createElement("tr");
        var action = document.createElement("td");
        row.appendChild(createElement("td", "workflow-transition-modal__destination", destination.nombreActividad));
        row.appendChild(createElement("td", "workflow-transition-modal__recipient", destination.grupoDestino));
        action.className = "workflow-transition-modal__action";
        action.appendChild(selectButton(control, destination));
        row.appendChild(action);
        return row;
    }
    function destinationCard(control, destination) {
        var card = createElement("article", "workflow-transition-modal__card");
        var metadata = createElement("dl", "workflow-transition-modal__card-metadata");
        card.appendChild(createElement("h3", "workflow-transition-modal__card-title", destination.nombreActividad));
        addDefinition(metadata, "Grupo destino", destination.grupoDestino);
        card.appendChild(metadata);
        card.appendChild(selectButton(control, destination));
        return card;
    }
    function renderDestinations(control, preview) {
        var index;
        clearDestinations(control);
        renderContext(control, preview);
        for (index = 0; index < preview.destinos.length; index += 1) {
            control.tableBody.appendChild(destinationRow(control, preview.destinos[index]));
            control.cards.appendChild(destinationCard(control, preview.destinos[index]));
        }
        setStatus(control, "lista-disponible", "Seleccione una actividad para continuar.", "informacion");
    }
    function renderEmpty(control, preview) {
        clearDestinations(control);
        renderContext(control, preview);
        setStatus(control, "sin-destinos", "No hay actividades disponibles para esta tarea.", "informacion");
    }
    function renderError(control, preview, retry, message) {
        var retryButton;
        clearDestinations(control);
        if (preview) { renderContext(control, preview); } else { empty(control.contexto); }
        setStatus(control, "error-controlado", asText(message, preview && preview.error ? preview.error.mensajeVisible : "No fue posible cargar las actividades. Intente nuevamente."), "error");
        retryButton = createElement("button", "workflow-transition-modal__retry", "Reintentar");
        retryButton.type = "button";
        retryButton.addEventListener("click", retry);
        control.status.appendChild(retryButton);
    }

    function openModal(control) {
        control.modal.hidden = false;
        control.modal.removeAttribute("hidden");
        control.modal.setAttribute("aria-hidden", "false");
        document.body.classList.add("workflow-transition-modal-open");
        window.setTimeout(function () { control.close.focus(); }, 0);
    }
    function closeModal(control) {
        control.modal.hidden = true;
        control.modal.setAttribute("hidden", "hidden");
        control.modal.setAttribute("aria-hidden", "true");
        control.modal.setAttribute("data-workflow-transition-state", "cerrado");
        document.body.classList.remove("workflow-transition-modal-open");
        if (control.trigger && typeof control.trigger.focus === "function") { control.trigger.focus(); }
    }
    function focusable(dialog) {
        var candidates = dialog.querySelectorAll("button:not([disabled]), [href], input:not([disabled]), select:not([disabled]), textarea:not([disabled]), [tabindex]:not([tabindex='-1'])");
        return Array.prototype.filter.call(candidates, function (element) {
            return element.getAttribute("aria-hidden") !== "true" && !element.hidden && (!element.getClientRects || element.getClientRects().length > 0);
        });
    }
    function handleKeyboard(control, event) {
        var elements, first, last;
        if (event.key === "Escape" || event.keyCode === 27) { event.preventDefault(); closeModal(control); return; }
        if (event.key !== "Tab" && event.keyCode !== 9) { return; }
        elements = focusable(control.dialog);
        if (!elements.length) { event.preventDefault(); control.dialog.focus(); return; }
        first = elements[0]; last = elements[elements.length - 1];
        if (event.shiftKey && document.activeElement === first) { event.preventDefault(); last.focus(); }
        if (!event.shiftKey && document.activeElement === last) { event.preventDefault(); first.focus(); }
    }
    function currentTaskId(control) {
        var inputIds = [control.trigger.getAttribute("data-workflow-current-task-input-id"), control.trigger.getAttribute("data-workflow-task-input-id")];
        var index, input, idTarea;
        for (index = 0; index < inputIds.length; index += 1) {
            input = inputIds[index] ? document.getElementById(inputIds[index]) : null;
            idTarea = input ? asPositiveInteger(input.value) : 0;
            if (idTarea > 0) { return idTarea; }
        }
        return 0;
    }
    function loadDestinations(control) {
        var idTarea = currentTaskId(control);
        var sequence;
        if (!idTarea) { renderError(control, null, function () { loadDestinations(control); }, "Seleccione una tarea activa antes de continuar."); return; }
        sequence = requestSequence + 1;
        requestSequence = sequence;
        control.preview = null;
        clearDestinations(control);
        empty(control.contexto);
        setStatus(control, "cargando", "Cargando actividades disponibles…", "informacion");
        requestPreview(idTarea).then(function (preview) {
            if (sequence !== requestSequence) { return; }
            control.preview = preview;
            if (preview.error && preview.error.codigo === "WORKFLOW_NO_DESTINATIONS") { renderEmpty(control, preview); }
            else if (preview.error) { renderError(control, preview, function () { loadDestinations(control); }, preview.error.mensajeVisible); }
            else if (!preview.destinos.length) { renderEmpty(control, preview); }
            else { renderDestinations(control, preview); }
        }).catch(function (error) {
            if (sequence === requestSequence) { renderError(control, null, function () { loadDestinations(control); }, error && error.message); }
        });
    }
    function intercept(control, event) {
        if (event) { event.preventDefault(); event.stopPropagation(); }
        openModal(control);
        loadDestinations(control);
        return false;
    }
    function applySuccess(detail) {
        if (!activeControl || !activeControl.preview || !detail || activeControl.preview.idTarea !== detail.idTarea || activeControl.preview.tokenVersion !== detail.tokenVersion) { return false; }
        requestSequence += 1;
        activeControl.preview = null;
        clearDestinations(activeControl);
        empty(activeControl.contexto);
        closeModal(activeControl);
        return true;
    }
    function initialize() {
        var trigger = document.getElementById("workflow-group-send-trigger");
        var control;
        if (!trigger || trigger.getAttribute("data-workflow-group-modern-active") !== "true" || trigger.getAttribute("data-workflow-group-modern-bound") === "true") { return; }
        control = {
            trigger: trigger,
            modal: document.getElementById("workflow-group-send-modern-modal"),
            dialog: document.getElementById("workflow-group-send-modern-dialog"),
            close: document.getElementById("workflow-group-send-modern-close"),
            status: document.getElementById("workflow-group-send-modern-status"),
            contexto: document.getElementById("workflow-group-send-modern-context"),
            tableBody: document.getElementById("workflow-group-send-modern-table-body"),
            cards: document.getElementById("workflow-group-send-modern-cards"),
            preview: null
        };
        if (!control.modal || !control.dialog || !control.close || !control.status || !control.contexto || !control.tableBody || !control.cards) { return; }
        activeControl = control;
        trigger.setAttribute("data-workflow-group-modern-bound", "true");
        trigger.onclick = function (event) { return intercept(control, event || window.event); };
        control.close.addEventListener("click", function () { closeModal(control); });
        control.modal.addEventListener("click", function (event) {
            if (event.target && event.target.getAttribute("data-workflow-group-send-close") === "true") { closeModal(control); }
        });
        control.dialog.addEventListener("keydown", function (event) { handleKeyboard(control, event); });
    }

    api.normalizarPrevisualizacion = normalizePreview;
    api.desempaquetarRespuestaAsmx = unwrapAsmx;
    api.solicitarPrevisualizacion = requestPreview;
    api.crearDetalleSeleccion = selectedDetail;
    api.aplicarEnvioExitoso = applySuccess;
    api.inicializar = initialize;
    api.onDestinationSelected = null;
    window.WorkflowGroupSendUi = api;
    if (window.Sys && window.Sys.Application && typeof window.Sys.Application.add_load === "function") { window.Sys.Application.add_load(initialize); }
    if (document && typeof document.getElementById === "function") {
        if (document.readyState === "loading") { document.addEventListener("DOMContentLoaded", initialize); } else { initialize(); }
    }
}(window, document));
