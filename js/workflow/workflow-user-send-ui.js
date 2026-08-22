(function (window, document) {
    "use strict";

    var previewUrl = "../webservice/WebServiceWorkflowModern.asmx/PreviewEnviarUsuario";
    var defaultPageSize = 25;
    var minimumSearchLength = 2;
    var searchDelayMilliseconds = 300;
    var requestSequence = 0;
    var activeControl = null;
    var api = {};

    function isObject(value) { return value !== null && typeof value === "object" && !Array.isArray(value); }
    function asText(value, fallback) {
        var text = value === undefined || value === null ? "" : String(value).replace(/^\s+|\s+$/g, "");
        return text || (fallback || "");
    }
    function asPositiveInteger(value, fallback) {
        var parsed = Number(value);
        return isFinite(parsed) && parsed > 0 && Math.floor(parsed) === parsed ? parsed : (fallback || 0);
    }
    function asArray(value) { return Array.isArray(value) ? value : []; }
    function createElement(tagName, className, text) {
        var element = document.createElement(tagName);
        if (className) { element.className = className; }
        if (text !== undefined) { element.textContent = text; }
        return element;
    }
    function empty(node) { while (node && node.firstChild) { node.removeChild(node.firstChild); } }

    function normalizeError(raw) {
        return isObject(raw) ? {
            codigo: asText(raw.Codigo),
            mensajeVisible: asText(raw.MensajeVisible || raw.MensajeFuncional, "No fue posible cargar los usuarios disponibles.")
        } : null;
    }
    function normalizeDestination(raw, index) {
        raw = isObject(raw) ? raw : {};
        return {
            idUsuarioWorkflowDestino: asPositiveInteger(raw.IdUsuarioWorkflowDestino),
            idActividadDestino: asPositiveInteger(raw.IdActividadDestino),
            nombreUsuarioDestino: asText(raw.NombreUsuarioDestino, "Usuario disponible"),
            cargoUsuarioDestino: asText(raw.CargoUsuarioDestino, "No especificado"),
            nombreActividadDestino: asText(raw.NombreActividadDestino, "Actividad disponible"),
            orden: index + 1
        };
    }
    function normalizarPrevisualizacion(raw, page) {
        var context;
        if (!isObject(raw)) { throw new Error("La respuesta de previsualización no tiene el formato esperado."); }
        context = isObject(raw.Contexto) ? raw.Contexto : {};
        return {
            idTarea: asPositiveInteger(raw.IdTarea),
            contexto: {
                radicado: asText(context.Radicado, "No disponible"),
                grupoActual: asText(context.GrupoActual, "No disponible")
            },
            destinos: asArray(raw.Destinos).map(normalizeDestination),
            tokenVersion: asText(raw.TokenVersion),
            cursorSiguiente: asText(raw.CursorSiguiente),
            tieneMas: raw.TieneMas === true,
            tamanoPagina: Math.min(50, asPositiveInteger(raw.TamanoPagina, defaultPageSize)),
            pagina: asPositiveInteger(page, 1),
            error: normalizeError(raw.Error)
        };
    }
    function desempaquetarRespuestaAsmx(raw) {
        var content;
        if (!isObject(raw) || !Object.prototype.hasOwnProperty.call(raw, "d")) {
            throw new Error("La respuesta del servicio no contiene el envoltorio ASMX esperado.");
        }
        content = raw.d;
        if (typeof content === "string") {
            try { content = JSON.parse(content); } catch (ignored) { throw new Error("La respuesta del servicio no contiene JSON válido."); }
        }
        return normalizarPrevisualizacion(content);
    }
    function solicitarPrevisualizacion(idTarea, consulta, cursor, tamanoPagina, fetchImplementation, signal) {
        fetchImplementation = fetchImplementation || window.fetch;
        if (typeof fetchImplementation !== "function") {
            return Promise.reject(new Error("Este navegador no permite cargar usuarios de forma segura."));
        }
        return fetchImplementation(previewUrl, {
            method: "POST",
            credentials: "same-origin",
            headers: { "Content-Type": "application/json; charset=utf-8" },
            body: JSON.stringify({
                idTarea: idTarea,
                consulta: asText(consulta),
                cursor: asText(cursor),
                tamanoPagina: asPositiveInteger(tamanoPagina, defaultPageSize)
            }),
            signal: signal
        }).then(function (response) {
            if (!response || response.ok === false) { throw new Error("No fue posible consultar los usuarios disponibles."); }
            return response.json();
        }).then(desempaquetarRespuestaAsmx);
    }

    function addDefinition(container, label, value) {
        container.appendChild(createElement("dt", "workflow-transition-modal__context-label", label));
        container.appendChild(createElement("dd", "workflow-transition-modal__context-value", value));
    }
    function setStatus(control, state, message, kind) {
        control.modal.setAttribute("data-workflow-user-send-state", state);
        control.status.setAttribute("data-workflow-user-send-status", kind || "informacion");
        control.status.textContent = message || "";
    }
    function clearDestinations(control) { empty(control.tableBody); empty(control.cards); }
    function renderContext(control, preview) {
        empty(control.contexto);
        addDefinition(control.contexto, "Radicado", preview.contexto.radicado);
        addDefinition(control.contexto, "Grupo actual", preview.contexto.grupoActual);
    }
    function renderPager(control, preview) {
        control.previous.disabled = preview.pagina <= 1;
        control.next.disabled = !preview.tieneMas || !preview.cursorSiguiente;
        control.page.textContent = "Página " + preview.pagina + ".";
    }
    function emitirEvento(name, detail) {
        var event;
        if (typeof window.CustomEvent === "function") { event = new window.CustomEvent(name, { detail: detail }); }
        else { event = document.createEvent("CustomEvent"); event.initCustomEvent(name, false, false, detail); }
        window.dispatchEvent(event);
    }
    function invalidarSeleccion(control) {
        if (!control || !control.preview) { return; }
        emitirEvento("workflow:user-destination-invalidated", {
            idTarea: control.preview.idTarea,
            tokenVersion: control.preview.tokenVersion
        });
    }
    function crearDetalleSeleccion(preview, destination) {
        return {
            idTarea: preview.idTarea,
            idUsuarioWorkflowDestino: destination.idUsuarioWorkflowDestino,
            idActividadDestino: destination.idActividadDestino,
            tokenVersion: preview.tokenVersion,
            contexto: { radicado: preview.contexto.radicado, grupoActual: preview.contexto.grupoActual },
            destino: {
                nombreUsuarioDestino: destination.nombreUsuarioDestino,
                cargoUsuarioDestino: destination.cargoUsuarioDestino,
                nombreActividadDestino: destination.nombreActividadDestino
            }
        };
    }
    function seleccionarDestino(control, destination) {
        var detail;
        if (!control.preview || destination.idUsuarioWorkflowDestino <= 0 || destination.idActividadDestino <= 0) { return; }
        detail = crearDetalleSeleccion(control.preview, destination);
        if (typeof api.onDestinationSelected === "function") {
            try { api.onDestinationSelected(detail); } catch (ignored) {}
        }
        emitirEvento("workflow:user-destination-selected", detail);
        setStatus(control, "destino-seleccionado", "Usuario seleccionado: " + destination.nombreUsuarioDestino + ".", "exito");
    }
    function createSelectButton(control, destination) {
        var button = createElement("button", "workflow-transition-modal__select", "Seleccionar");
        button.type = "button";
        button.disabled = destination.idUsuarioWorkflowDestino <= 0 || destination.idActividadDestino <= 0;
        button.addEventListener("click", function () { seleccionarDestino(control, destination); });
        return button;
    }
    function createDestinationRow(control, destination) {
        var row = document.createElement("tr");
        var action = document.createElement("td");
        row.appendChild(createElement("td", "workflow-transition-modal__destination", destination.nombreUsuarioDestino));
        row.appendChild(createElement("td", "workflow-transition-modal__recipient", destination.cargoUsuarioDestino));
        row.appendChild(createElement("td", "workflow-transition-modal__recipient", destination.nombreActividadDestino));
        action.className = "workflow-transition-modal__action";
        action.appendChild(createSelectButton(control, destination));
        row.appendChild(action);
        return row;
    }
    function createDestinationCard(control, destination) {
        var card = createElement("article", "workflow-transition-modal__card");
        var metadata = createElement("dl", "workflow-transition-modal__card-metadata");
        card.appendChild(createElement("h3", "workflow-transition-modal__card-title", destination.nombreUsuarioDestino));
        addDefinition(metadata, "Cargo", destination.cargoUsuarioDestino);
        addDefinition(metadata, "Actividad destino", destination.nombreActividadDestino);
        card.appendChild(metadata);
        card.appendChild(createSelectButton(control, destination));
        return card;
    }
    function renderDestinations(control, preview) {
        var index;
        clearDestinations(control);
        renderContext(control, preview);
        for (index = 0; index < preview.destinos.length; index += 1) {
            control.tableBody.appendChild(createDestinationRow(control, preview.destinos[index]));
            control.cards.appendChild(createDestinationCard(control, preview.destinos[index]));
        }
        renderPager(control, preview);
        setStatus(control, "lista-disponible", "Seleccione un usuario destino.", "informacion");
    }
    function renderEmpty(control, preview) {
        clearDestinations(control);
        renderContext(control, preview);
        renderPager(control, preview);
        setStatus(control, "sin-destinos", "No hay usuarios que coincidan con la búsqueda.", "informacion");
    }
    function renderError(control, preview, retry, message) {
        var retryButton;
        clearDestinations(control);
        if (preview) { renderContext(control, preview); } else { empty(control.contexto); }
        control.previous.disabled = true;
        control.next.disabled = true;
        setStatus(control, "error-controlado", asText(message, preview && preview.error ? preview.error.mensajeVisible : "No fue posible cargar los usuarios disponibles."), "error");
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
        window.setTimeout(function () { (control.search || control.close).focus(); }, 0);
    }
    function closeModal(control) {
        invalidarSeleccion(control);
        cancelRequest(control);
        control.modal.hidden = true;
        control.modal.setAttribute("hidden", "hidden");
        control.modal.setAttribute("aria-hidden", "true");
        control.modal.setAttribute("data-workflow-user-send-state", "cerrado");
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
        first = elements[0];
        last = elements[elements.length - 1];
        if (event.shiftKey && document.activeElement === first) { event.preventDefault(); last.focus(); }
        else if (!event.shiftKey && document.activeElement === last) { event.preventDefault(); first.focus(); }
    }
    function currentTaskId(control) {
        var ids = [control.trigger.getAttribute("data-workflow-current-task-input-id"), control.trigger.getAttribute("data-workflow-task-input-id")];
        var index, input, idTarea;
        for (index = 0; index < ids.length; index += 1) {
            input = ids[index] ? document.getElementById(ids[index]) : null;
            idTarea = input ? asPositiveInteger(input.value) : 0;
            if (idTarea > 0) { return idTarea; }
        }
        return 0;
    }
    function cancelRequest(control) {
        requestSequence += 1;
        if (control.abortController && typeof control.abortController.abort === "function") { control.abortController.abort(); }
        control.abortController = null;
    }
    function loadPage(control, pageIndex, cursor, query) {
        var idTarea = currentTaskId(control);
        var sequence;
        var signal;
        if (!idTarea) { renderError(control, null, function () { loadPage(control, 0, "", ""); }, "Seleccione una tarea activa antes de continuar."); return; }
        invalidarSeleccion(control);
        cancelRequest(control);
        sequence = requestSequence;
        control.query = asText(query);
        control.pageIndex = pageIndex;
        clearDestinations(control);
        setStatus(control, "cargando", "Cargando usuarios disponibles…", "informacion");
        if (window.AbortController) { control.abortController = new window.AbortController(); signal = control.abortController.signal; }
        solicitarPrevisualizacion(idTarea, control.query, cursor, defaultPageSize, null, signal).then(function (preview) {
            if (sequence !== requestSequence) { return; }
            preview.pagina = pageIndex + 1;
            control.preview = preview;
            control.cursorHistory[pageIndex] = asText(cursor);
            control.cursorHistory[pageIndex + 1] = preview.cursorSiguiente;
            if (preview.error) { renderError(control, preview, function () { loadPage(control, pageIndex, cursor, control.query); }, preview.error.mensajeVisible); }
            else if (!preview.destinos.length) { renderEmpty(control, preview); }
            else { renderDestinations(control, preview); }
        }).catch(function (error) {
            if (sequence === requestSequence && !(error && error.name === "AbortError")) {
                renderError(control, null, function () { loadPage(control, pageIndex, cursor, control.query); }, error && error.message);
            }
        });
    }
    function startSearch(control, query) {
        query = asText(query);
        if (control.searchTimer) { window.clearTimeout(control.searchTimer); control.searchTimer = null; }
        invalidarSeleccion(control);
        if (query && query.length < minimumSearchLength) {
            cancelRequest(control);
            clearDestinations(control);
            if (control.preview) { renderContext(control, control.preview); }
            setStatus(control, "termino-corto", "Escriba al menos dos caracteres para buscar.", "informacion");
            return;
        }
        control.cursorHistory = [""];
        if (!query) { loadPage(control, 0, "", ""); return; }
        control.searchTimer = window.setTimeout(function () { loadPage(control, 0, "", query); }, searchDelayMilliseconds);
    }
    function goPrevious(control) {
        if (!control.preview || control.pageIndex <= 0) { return; }
        loadPage(control, control.pageIndex - 1, control.cursorHistory[control.pageIndex - 1], control.query);
    }
    function goNext(control) {
        if (!control.preview || !control.preview.tieneMas || !control.preview.cursorSiguiente) { return; }
        loadPage(control, control.pageIndex + 1, control.preview.cursorSiguiente, control.query);
    }
    function applySuccess(detail) {
        if (!activeControl || !activeControl.preview || !detail || activeControl.preview.idTarea !== detail.idTarea || activeControl.preview.tokenVersion !== detail.tokenVersion) { return false; }
        activeControl.preview = null;
        closeModal(activeControl);
        return true;
    }
    function initialize() {
        var trigger = document.getElementById("workflow-user-send-trigger");
        var control;
        if (!trigger || trigger.getAttribute("data-workflow-user-send-active") !== "true" || trigger.getAttribute("data-workflow-user-send-bound") === "true") { return; }
        control = {
            trigger: trigger,
            modal: document.getElementById("workflow-user-send-modern-modal"),
            dialog: document.getElementById("workflow-user-send-modern-dialog"),
            close: document.getElementById("workflow-user-send-modern-close"),
            status: document.getElementById("workflow-user-send-modern-status"),
            contexto: document.getElementById("workflow-user-send-modern-context"),
            search: document.getElementById("workflow-user-send-modern-search"),
            previous: document.getElementById("workflow-user-send-modern-previous"),
            next: document.getElementById("workflow-user-send-modern-next"),
            page: document.getElementById("workflow-user-send-modern-page"),
            tableBody: document.getElementById("workflow-user-send-modern-table-body"),
            cards: document.getElementById("workflow-user-send-modern-cards"),
            preview: null,
            pageIndex: 0,
            cursorHistory: [""],
            query: "",
            searchTimer: null,
            abortController: null
        };
        if (!control.modal || !control.dialog || !control.close || !control.status || !control.contexto || !control.search || !control.previous || !control.next || !control.page || !control.tableBody || !control.cards) { return; }
        activeControl = control;
        trigger.setAttribute("data-workflow-user-send-bound", "true");
        trigger.onclick = function (event) {
            if (event) { event.preventDefault(); event.stopPropagation(); }
            openModal(control);
            control.search.value = "";
            control.cursorHistory = [""];
            loadPage(control, 0, "", "");
            return false;
        };
        control.close.addEventListener("click", function () { closeModal(control); });
        control.modal.addEventListener("click", function (event) {
            if (event.target && event.target.getAttribute("data-workflow-user-send-close") === "true") { closeModal(control); }
        });
        control.dialog.addEventListener("keydown", function (event) { handleKeyboard(control, event); });
        control.search.addEventListener("input", function () { startSearch(control, control.search.value); });
        control.previous.addEventListener("click", function () { goPrevious(control); });
        control.next.addEventListener("click", function () { goNext(control); });
    }

    api.normalizarPrevisualizacion = normalizarPrevisualizacion;
    api.desempaquetarRespuestaAsmx = desempaquetarRespuestaAsmx;
    api.solicitarPrevisualizacion = solicitarPrevisualizacion;
    api.crearDetalleSeleccion = crearDetalleSeleccion;
    api.aplicarEnvioExitoso = applySuccess;
    api.inicializar = initialize;
    api.onDestinationSelected = null;
    window.WorkflowUserSendUi = api;
    if (window.Sys && window.Sys.Application && typeof window.Sys.Application.add_load === "function") { window.Sys.Application.add_load(initialize); }
    if (document && typeof document.getElementById === "function") {
        if (document.readyState === "loading") { document.addEventListener("DOMContentLoaded", initialize); } else { initialize(); }
    }
}(window, document));
