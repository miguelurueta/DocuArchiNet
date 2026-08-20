(function (window, document) {
    "use strict";

    var previewUrl = "../webservice/WebServiceWorkflowModern.asmx/PreviewEnviarGrupo";
    var searchUrl = "../webservice/WebServiceWorkflowModern.asmx/BuscarDestinosEnvioGrupo";
    var defaultPageSize = 25;
    var minimumSearchLength = 2;
    var searchDelayMilliseconds = 300;
    var requestSequence = 0;
    var activeAbortController = null;
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

    function normalizeDestination(raw, index, page, pageSize) {
        raw = isObject(raw) ? raw : {};
        return {
            idActividadDestino: asPositiveInteger(raw.IdActividadDestino),
            nombreActividad: asText(raw.NombreActividad, "Actividad disponible"),
            grupoDestino: asText(raw.GrupoDestino, "No especificado"),
            orden: ((page - 1) * pageSize) + index + 1
        };
    }
    function normalizeError(raw) {
        return isObject(raw) ? {
            codigo: asText(raw.Codigo),
            mensajeVisible: asText(raw.MensajeVisible, "No fue posible cargar las actividades.")
        } : null;
    }
    function normalizePage(raw) {
        raw = isObject(raw) ? raw : {};
        var page = asPositiveInteger(raw.Pagina, 1);
        var pageSize = asPositiveInteger(raw.TamanoPagina, defaultPageSize);
        return {
            pagina: page,
            tamanoPagina: Math.min(50, pageSize),
            tieneMas: raw.TieneMas === true
        };
    }
    function normalizePreview(raw) {
        var context;
        var page;
        if (!isObject(raw)) { throw new Error("La respuesta de previsualización no tiene el formato esperado."); }
        context = isObject(raw.Contexto) ? raw.Contexto : {};
        page = normalizePage(raw);
        return {
            idTarea: asPositiveInteger(raw.IdTarea),
            contexto: {
                radicado: asText(context.Radicado, "No disponible"),
                grupoActual: asText(context.GrupoActual, "No disponible")
            },
            destinos: asArray(raw.Destinos).map(function (destination, index) {
                return normalizeDestination(destination, index, page.pagina, page.tamanoPagina);
            }),
            tokenVersion: asText(raw.TokenVersion),
            pagina: page.pagina,
            tamanoPagina: page.tamanoPagina,
            tieneMas: page.tieneMas,
            error: normalizeError(raw.Error)
        };
    }
    function normalizeSearch(raw) {
        var page;
        if (!isObject(raw)) { throw new Error("La respuesta de búsqueda no tiene el formato esperado."); }
        page = normalizePage(raw);
        return {
            idTarea: asPositiveInteger(raw.IdTarea),
            destinos: asArray(raw.Destinos).map(function (destination, index) {
                return normalizeDestination(destination, index, page.pagina, page.tamanoPagina);
            }),
            tokenVersion: asText(raw.TokenVersion),
            pagina: page.pagina,
            tamanoPagina: page.tamanoPagina,
            tieneMas: page.tieneMas,
            error: normalizeError(raw.Error)
        };
    }
    function unwrapAsmx(raw, normalize) {
        var value;
        if (!isObject(raw) || !Object.prototype.hasOwnProperty.call(raw, "d")) {
            throw new Error("La respuesta del servicio no contiene el envoltorio ASMX esperado.");
        }
        value = raw.d;
        if (typeof value === "string") {
            try { value = JSON.parse(value); } catch (ignored) { throw new Error("La respuesta del servicio no contiene JSON válido."); }
        }
        return normalize(value);
    }
    function unwrapPreviewAsmx(raw) { return unwrapAsmx(raw, normalizePreview); }
    function unwrapSearchAsmx(raw) { return unwrapAsmx(raw, normalizeSearch); }

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
            if (!response || response.ok === false) { throw new Error("No fue posible consultar las actividades disponibles."); }
            return response.json();
        }).then(unwrapPreviewAsmx);
    }
    function requestSearch(idTarea, term, page, pageSize, fetchImplementation, signal) {
        fetchImplementation = fetchImplementation || window.fetch;
        if (typeof fetchImplementation !== "function") {
            return Promise.reject(new Error("Este navegador no permite buscar actividades de forma segura."));
        }
        return fetchImplementation(searchUrl, {
            method: "POST",
            credentials: "same-origin",
            headers: { "Content-Type": "application/json; charset=utf-8" },
            body: JSON.stringify({ idTarea: idTarea, termino: term, pagina: page, tamanoPagina: pageSize }),
            signal: signal
        }).then(function (response) {
            if (!response || response.ok === false) { throw new Error("No fue posible consultar las actividades disponibles."); }
            return response.json();
        }).then(unwrapSearchAsmx);
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
    function renderPager(control, preview) {
        if (!control.previous || !control.next || !control.page) { return; }
        control.previous.disabled = preview.pagina <= 1;
        control.next.disabled = !preview.tieneMas;
        control.page.textContent = "Página " + preview.pagina + ".";
    }
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
    function dispatchCustomEvent(name, detail) {
        var event;
        if (typeof window.CustomEvent === "function") {
            event = new window.CustomEvent(name, { detail: detail });
        } else {
            event = document.createEvent("CustomEvent");
            event.initCustomEvent(name, false, false, detail);
        }
        window.dispatchEvent(event);
    }
    function invalidateSelection(control) {
        if (!control || !control.preview) { return; }
        dispatchCustomEvent("workflow:group-destination-invalidated", {
            idTarea: control.preview.idTarea,
            tokenVersion: control.preview.tokenVersion
        });
    }
    function dispatchSelection(control, destination) {
        var detail = selectedDetail(control.preview, destination);
        if (typeof api.onDestinationSelected === "function") {
            try { api.onDestinationSelected(detail); } catch (ignored) {}
        }
        dispatchCustomEvent("workflow:group-destination-selected", detail);
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
        renderPager(control, preview);
        setStatus(control, "lista-disponible", "Resultados disponibles. Página " + preview.pagina + ".", "informacion");
    }
    function renderEmpty(control, preview) {
        clearDestinations(control);
        renderContext(control, preview);
        renderPager(control, preview);
        setStatus(control, "sin-destinos", "No hay actividades que coincidan con la búsqueda.", "informacion");
    }
    function renderError(control, preview, retry, message) {
        var retryButton;
        clearDestinations(control);
        if (preview) { renderContext(control, preview); } else { empty(control.contexto); }
        if (control.previous) { control.previous.disabled = true; }
        if (control.next) { control.next.disabled = true; }
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
        window.setTimeout(function () {
            if (control.search && typeof control.search.focus === "function") { control.search.focus(); }
            else { control.close.focus(); }
        }, 0);
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
    function nextRequestSequence() {
        requestSequence += 1;
        if (activeAbortController && typeof activeAbortController.abort === "function") { activeAbortController.abort(); }
        activeAbortController = window.AbortController ? new window.AbortController() : null;
        return requestSequence;
    }
    function loadSearch(control, term, page) {
        var idTarea = currentTaskId(control);
        var sequence;
        var signal;
        if (!idTarea) { renderError(control, control.preview, function () { loadDestinations(control); }, "Seleccione una tarea activa antes de continuar."); return; }
        invalidateSelection(control);
        sequence = nextRequestSequence();
        signal = activeAbortController ? activeAbortController.signal : undefined;
        clearDestinations(control);
        setStatus(control, "cargando", "Buscando actividades disponibles…", "informacion");
        requestSearch(idTarea, term, page, defaultPageSize, null, signal).then(function (search) {
            if (sequence !== requestSequence) { return; }
            control.preview = {
                idTarea: search.idTarea,
                contexto: control.preview ? control.preview.contexto : { radicado: "No disponible", grupoActual: "No disponible" },
                destinos: search.destinos,
                tokenVersion: search.tokenVersion || (control.preview ? control.preview.tokenVersion : ""),
                pagina: search.pagina,
                tamanoPagina: search.tamanoPagina,
                tieneMas: search.tieneMas,
                error: search.error
            };
            if (search.error && search.error.codigo === "WORKFLOW_NO_DESTINATIONS") { renderEmpty(control, control.preview); }
            else if (search.error) { renderError(control, control.preview, function () { loadSearch(control, term, page); }, search.error.mensajeVisible); }
            else if (!search.destinos.length) { renderEmpty(control, control.preview); }
            else { renderDestinations(control, control.preview); }
        }).catch(function (error) {
            if (sequence === requestSequence && !(error && error.name === "AbortError")) {
                renderError(control, control.preview, function () { loadSearch(control, term, page); }, error && error.message);
            }
        });
    }
    function scheduleSearch(control) {
        var term = asText(control.search.value);
        if (control.searchTimer) { window.clearTimeout(control.searchTimer); }
        invalidateSelection(control);
        if (term && term.length < minimumSearchLength) {
            clearDestinations(control);
            if (control.preview) { renderContext(control, control.preview); }
            setStatus(control, "termino-corto", "Escriba al menos dos caracteres para buscar.", "informacion");
            return;
        }
        if (!term) {
            loadSearch(control, "", 1);
            return;
        }
        control.searchTimer = window.setTimeout(function () { loadSearch(control, term, 1); }, searchDelayMilliseconds);
    }
    function loadDestinations(control) {
        var idTarea = currentTaskId(control);
        var sequence;
        if (!idTarea) { renderError(control, null, function () { loadDestinations(control); }, "Seleccione una tarea activa antes de continuar."); return; }
        sequence = nextRequestSequence();
        control.preview = null;
        clearDestinations(control);
        empty(control.contexto);
        if (control.search) { control.search.value = ""; }
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
        nextRequestSequence();
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
            search: document.getElementById("workflow-group-send-modern-search"),
            previous: document.getElementById("workflow-group-send-modern-previous"),
            next: document.getElementById("workflow-group-send-modern-next"),
            page: document.getElementById("workflow-group-send-modern-page"),
            tableBody: document.getElementById("workflow-group-send-modern-table-body"),
            cards: document.getElementById("workflow-group-send-modern-cards"),
            preview: null,
            searchTimer: null
        };
        if (!control.modal || !control.dialog || !control.close || !control.status || !control.contexto || !control.search || !control.previous || !control.next || !control.page || !control.tableBody || !control.cards) { return; }
        activeControl = control;
        trigger.setAttribute("data-workflow-group-modern-bound", "true");
        trigger.onclick = function (event) { return intercept(control, event || window.event); };
        control.close.addEventListener("click", function () { closeModal(control); });
        control.search.addEventListener("input", function () { scheduleSearch(control); });
        control.previous.addEventListener("click", function () {
            if (control.preview && control.preview.pagina > 1) { loadSearch(control, asText(control.search.value), control.preview.pagina - 1); }
        });
        control.next.addEventListener("click", function () {
            if (control.preview && control.preview.tieneMas) { loadSearch(control, asText(control.search.value), control.preview.pagina + 1); }
        });
        control.modal.addEventListener("click", function (event) {
            if (event.target && event.target.getAttribute("data-workflow-group-send-close") === "true") { closeModal(control); }
        });
        control.dialog.addEventListener("keydown", function (event) { handleKeyboard(control, event); });
    }

    api.normalizarPrevisualizacion = normalizePreview;
    api.normalizarBusqueda = normalizeSearch;
    api.desempaquetarRespuestaAsmx = unwrapPreviewAsmx;
    api.desempaquetarBusquedaAsmx = unwrapSearchAsmx;
    api.solicitarPrevisualizacion = requestPreview;
    api.solicitarBusqueda = requestSearch;
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
