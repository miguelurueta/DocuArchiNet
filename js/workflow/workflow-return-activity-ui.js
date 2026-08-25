(function (window, document) {
    "use strict";

    var previewUrl = "../webservice/WebServiceWorkflowModern.asmx/PreviewDevolverActividad";
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
    function empty(element) { while (element && element.firstChild) { element.removeChild(element.firstChild); } }
    function find(id) { return document.getElementById(id); }
    function emit(name, detail) {
        var event;
        if (typeof window.CustomEvent === "function") { event = new window.CustomEvent(name, { detail: detail }); }
        else { event = document.createEvent("CustomEvent"); event.initCustomEvent(name, false, false, detail); }
        window.dispatchEvent(event);
    }
    function normalizeError(raw) {
        return isObject(raw) ? {
            codigo: asText(raw.Codigo),
            mensajeVisible: asText(raw.MensajeVisible || raw.MensajeFuncional, "No fue posible consultar las actividades anteriores.")
        } : null;
    }
    function normalizeDestination(raw, index) {
        raw = isObject(raw) ? raw : {};
        return {
            idConector: asPositiveInteger(raw.IdConector),
            nombreActividad: asText(raw.NombreActividad, "Actividad disponible"),
            destinatario: asText(raw.Destinatario || raw.GrupoDestino, "Sin destinatario"),
            tipoContexto: asText(raw.TipoContexto, "Workflow"),
            orden: asPositiveInteger(raw.Orden, index + 1)
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
                actividadActual: asText(context.ActividadActual, "No disponible"),
                grupoActual: asText(context.GrupoActual, "No disponible"),
                tipoContexto: asText(context.TipoContexto, "Workflow")
            },
            destinos: asArray(raw.Destinos).map(normalizeDestination),
            tokenVersion: asText(raw.TokenVersion),
            cursorSiguiente: asText(raw.CursorSiguiente),
            hayMas: raw.HayMas === true,
            tamanoPagina: Math.min(50, asPositiveInteger(raw.TamanoPagina, defaultPageSize)),
            pagina: asPositiveInteger(page, 1),
            error: normalizeError(raw.Error)
        };
    }
    function desempaquetarRespuestaAsmx(raw, page) {
        var content;
        if (!isObject(raw) || !Object.prototype.hasOwnProperty.call(raw, "d")) { throw new Error("La respuesta del servicio no contiene el envoltorio ASMX esperado."); }
        content = raw.d;
        if (typeof content === "string") {
            try { content = JSON.parse(content); } catch (ignored) { throw new Error("La respuesta del servicio no contiene JSON válido."); }
        }
        return normalizarPrevisualizacion(content, page);
    }
    function solicitarPrevisualizacion(idTarea, consulta, cursor, tamanoPagina, fetchImplementation, signal, page) {
        fetchImplementation = fetchImplementation || window.fetch;
        if (typeof fetchImplementation !== "function") { return Promise.reject(new Error("Este navegador no permite consultar actividades anteriores de forma segura.")); }
        return fetchImplementation(previewUrl, {
            method: "POST",
            credentials: "same-origin",
            headers: { "Content-Type": "application/json; charset=utf-8" },
            body: JSON.stringify({ idTarea: idTarea, termino: asText(consulta), cursor: asText(cursor), tamanoPagina: asPositiveInteger(tamanoPagina, defaultPageSize) }),
            signal: signal
        }).then(function (response) {
            if (!response || response.ok === false) { throw new Error("No fue posible consultar las actividades anteriores."); }
            return response.json();
        }).then(function (raw) { return desempaquetarRespuestaAsmx(raw, page); });
    }
    function addDefinition(container, label, value) {
        container.appendChild(createElement("dt", "workflow-transition-modal__context-label", label));
        container.appendChild(createElement("dd", "workflow-transition-modal__context-value", value));
    }
    function setStatus(control, state, message, kind) {
        control.modal.setAttribute("data-workflow-return-activity-state", state);
        control.status.setAttribute("data-workflow-return-activity-status", kind || "informacion");
        control.status.textContent = message || "";
    }
    function renderContext(control, preview) {
        empty(control.context);
        addDefinition(control.context, "Radicado", preview.contexto.radicado);
        addDefinition(control.context, "Actividad actual", preview.contexto.actividadActual);
        addDefinition(control.context, "Grupo actual", preview.contexto.grupoActual);
    }
    function renderPager(control, preview) {
        control.previous.disabled = preview.pagina <= 1;
        control.next.disabled = !preview.hayMas || !preview.cursorSiguiente;
        control.page.textContent = "Página " + preview.pagina + ".";
    }
    function invalidateSelection(control) {
        if (!control || !control.preview) { return; }
        emit("workflow:return-activity-invalidated", { idTarea: control.preview.idTarea, tokenVersion: control.preview.tokenVersion });
    }
    function selectionDetail(preview, destination) {
        return {
            idTarea: preview.idTarea,
            idConector: destination.idConector,
            tokenVersion: preview.tokenVersion,
            contexto: preview.contexto,
            destino: { nombreActividad: destination.nombreActividad, destinatario: destination.destinatario, tipoContexto: destination.tipoContexto }
        };
    }
    function selectDestination(control, destination) {
        var detail;
        if (!control.preview || destination.idConector <= 0) { return; }
        detail = selectionDetail(control.preview, destination);
        emit("workflow:return-activity-selected", detail);
        setStatus(control, "destino-seleccionado", "Actividad seleccionada: " + destination.nombreActividad + ".", "exito");
    }
    function selectButton(control, destination) {
        var button = createElement("button", "workflow-transition-modal__select", "Seleccionar");
        button.type = "button";
        button.disabled = destination.idConector <= 0;
        button.addEventListener("click", function () { selectDestination(control, destination); });
        return button;
    }
    function renderDestinations(control, preview) {
        var index;
        empty(control.tableBody); empty(control.cards); renderContext(control, preview);
        for (index = 0; index < preview.destinos.length; index += 1) {
            (function (destination) {
                var row = document.createElement("tr");
                var action = document.createElement("td");
                var card = createElement("article", "workflow-transition-modal__card");
                var metadata = createElement("dl", "workflow-transition-modal__card-metadata");
                row.appendChild(createElement("td", "workflow-transition-modal__destination", destination.nombreActividad));
                row.appendChild(createElement("td", "workflow-transition-modal__recipient", destination.destinatario));
                row.appendChild(createElement("td", "workflow-transition-modal__recipient", destination.tipoContexto));
                action.className = "workflow-transition-modal__action"; action.appendChild(selectButton(control, destination)); row.appendChild(action);
                control.tableBody.appendChild(row);
                card.appendChild(createElement("h3", "workflow-transition-modal__card-title", destination.nombreActividad));
                addDefinition(metadata, "Destino", destination.destinatario); addDefinition(metadata, "Contexto", destination.tipoContexto);
                card.appendChild(metadata); card.appendChild(selectButton(control, destination)); control.cards.appendChild(card);
            }(preview.destinos[index]));
        }
        renderPager(control, preview);
        setStatus(control, "lista-disponible", "Seleccione la actividad anterior de destino.", "informacion");
    }
    function renderEmpty(control, preview) {
        empty(control.tableBody); empty(control.cards); renderContext(control, preview); renderPager(control, preview);
        setStatus(control, "sin-destinos", preview.error ? preview.error.mensajeVisible : "No hay actividades anteriores disponibles.", preview.error ? "error" : "informacion");
    }
    function renderError(control, message) {
        empty(control.tableBody); empty(control.cards); control.previous.disabled = true; control.next.disabled = true;
        setStatus(control, "error-controlado", asText(message, "No fue posible consultar las actividades anteriores."), "error");
    }
    function taskId(control) {
        var current = find(control.trigger.getAttribute("data-workflow-current-task-input-id"));
        var fallback = find(control.trigger.getAttribute("data-workflow-task-input-id"));
        return asPositiveInteger((current && current.value) || (fallback && fallback.value));
    }
    function cancelRequest(control) {
        requestSequence += 1;
        if (control.abortController) { control.abortController.abort(); control.abortController = null; }
        if (control.searchTimer) { window.clearTimeout(control.searchTimer); control.searchTimer = null; }
    }
    function loadPreview(control, cursor, page) {
        var query = asText(control.search.value);
        var idTarea = taskId(control);
        var sequence;
        invalidateSelection(control); cancelRequest(control);
        control.preview = null;
        empty(control.tableBody); empty(control.cards);
        if (!idTarea) { renderError(control, "Seleccione una tarea para devolver."); return; }
        if (query && query.length < minimumSearchLength) { renderError(control, "Escriba al menos dos caracteres para buscar."); return; }
        sequence = ++requestSequence;
        if (typeof window.AbortController === "function") { control.abortController = new window.AbortController(); }
        setStatus(control, "cargando", "Consultando actividades anteriores…", "informacion");
        solicitarPrevisualizacion(idTarea, query, cursor || "", defaultPageSize, null, control.abortController && control.abortController.signal, page).then(function (preview) {
            if (sequence !== requestSequence || !control.isOpen) { return; }
            control.abortController = null; control.preview = preview; control.cursorHistory[preview.pagina] = cursor || "";
            if (preview.error || !preview.destinos.length) { renderEmpty(control, preview); } else { renderDestinations(control, preview); }
        }).catch(function (error) {
            if (sequence !== requestSequence || (error && error.name === "AbortError")) { return; }
            control.abortController = null; renderError(control, "No fue posible consultar las actividades anteriores.");
        });
    }
    function focusable(control) {
        return control.modal.querySelectorAll("button:not([disabled]), input:not([disabled])");
    }
    function openModal(control) {
        control.isOpen = true; control.modal.hidden = false; control.modal.removeAttribute("hidden"); control.modal.setAttribute("aria-hidden", "false");
        document.body.classList.add("workflow-transition-modal-open");
        window.setTimeout(function () { (control.search || control.close).focus(); }, 0);
        loadPreview(control, "", 1);
    }
    function closeModal(control) {
        if (control.executionPending) {
            setStatus(control, "ejecucion-pendiente", "La devolución está en curso. Espere la respuesta antes de cerrar.", "informacion");
            return false;
        }
        invalidateSelection(control); cancelRequest(control); control.isOpen = false; control.preview = null; control.cursorHistory = {};
        control.modal.hidden = true; control.modal.setAttribute("hidden", "hidden"); control.modal.setAttribute("aria-hidden", "true");
        document.body.classList.remove("workflow-transition-modal-open"); if (control.trigger && typeof control.trigger.focus === "function") { control.trigger.focus(); } setStatus(control, "cerrado", "", "informacion");
        return true;
    }
    function createControl(trigger) {
        return {
            trigger: trigger, modal: find("workflow-return-activity-modern-modal"), close: find("workflow-return-activity-modern-close"), search: find("workflow-return-activity-modern-search"), status: find("workflow-return-activity-modern-status"), context: find("workflow-return-activity-modern-context"), previous: find("workflow-return-activity-modern-previous"), next: find("workflow-return-activity-modern-next"), page: find("workflow-return-activity-modern-page"), tableBody: find("workflow-return-activity-modern-table-body"), cards: find("workflow-return-activity-modern-cards"), preview: null, cursorHistory: {}, abortController: null, searchTimer: null, isOpen: false, executionPending: false
        };
    }
    function attach(control) {
        control.trigger.addEventListener("click", function () { if (!control.isOpen) { openModal(control); } });
        control.close.addEventListener("click", function () { closeModal(control); });
        control.modal.querySelector("[data-workflow-return-activity-close]").addEventListener("click", function () { closeModal(control); });
        control.search.addEventListener("input", function () {
            cancelRequest(control);
            control.searchTimer = window.setTimeout(function () { loadPreview(control, "", 1); }, searchDelayMilliseconds);
        });
        control.previous.addEventListener("click", function () { if (control.preview && control.preview.pagina > 1) { loadPreview(control, control.cursorHistory[control.preview.pagina - 1] || "", control.preview.pagina - 1); } });
        control.next.addEventListener("click", function () { if (control.preview && control.preview.hayMas) { loadPreview(control, control.preview.cursorSiguiente, control.preview.pagina + 1); } });
        control.modal.addEventListener("keydown", function (event) {
            var items; var first; var last;
            if (event.key === "Escape") { event.preventDefault(); closeModal(control); return; }
            if (event.key !== "Tab") { return; }
            items = focusable(control); if (!items.length) { return; }
            first = items[0]; last = items[items.length - 1];
            if (event.shiftKey && document.activeElement === first) { event.preventDefault(); last.focus(); }
            else if (!event.shiftKey && document.activeElement === last) { event.preventDefault(); first.focus(); }
        });
    }
    function inicializar() {
        var trigger = find("workflow-return-activity-trigger");
        if (!trigger || trigger.getAttribute("data-workflow-return-activity-active") !== "true" || activeControl) { return false; }
        activeControl = createControl(trigger);
        if (!activeControl.modal || !activeControl.close || !activeControl.search || !activeControl.status || !activeControl.context || !activeControl.previous || !activeControl.next || !activeControl.page || !activeControl.tableBody || !activeControl.cards) { activeControl = null; return false; }
        attach(activeControl); return true;
    }
    function aplicarDevolucionExitosa(selection) {
        if (!activeControl || !activeControl.preview || !selection || activeControl.preview.idTarea !== asPositiveInteger(selection.idTarea) || activeControl.preview.tokenVersion !== asText(selection.tokenVersion)) { return false; }
        activeControl.executionPending = false;
        closeModal(activeControl);
        return true;
    }
    function establecerEjecucionPendiente(selection, pending) {
        if (!activeControl || !activeControl.preview || !selection || activeControl.preview.idTarea !== asPositiveInteger(selection.idTarea) || activeControl.preview.tokenVersion !== asText(selection.tokenVersion)) { return false; }
        activeControl.executionPending = pending === true;
        return true;
    }

    api.normalizarPrevisualizacion = normalizarPrevisualizacion;
    api.desempaquetarRespuestaAsmx = desempaquetarRespuestaAsmx;
    api.solicitarPrevisualizacion = solicitarPrevisualizacion;
    api.crearDetalleSeleccion = selectionDetail;
    api.aplicarDevolucionExitosa = aplicarDevolucionExitosa;
    api.establecerEjecucionPendiente = establecerEjecucionPendiente;
    api.inicializar = inicializar;
    window.WorkflowReturnActivityUi = api;
}(window, document));
