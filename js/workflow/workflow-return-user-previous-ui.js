(function (window, document) {
    "use strict";

    var previewUrl = "../webservice/WebServiceWorkflowModern.asmx/PreviewDevolverUsuarioAnterior";
    var requestTimeoutMilliseconds = 15000;
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
    function find(id) { return document.getElementById(id); }
    function empty(element) { while (element && element.firstChild) { element.removeChild(element.firstChild); } }
    function createElement(tagName, className, text) {
        var element = document.createElement(tagName);
        if (className) { element.className = className; }
        if (text !== undefined) { element.textContent = text; }
        return element;
    }
    function emit(name, detail) {
        var event;
        if (typeof window.CustomEvent === "function") { event = new window.CustomEvent(name, { detail: detail }); }
        else { event = document.createEvent("CustomEvent"); event.initCustomEvent(name, false, false, detail); }
        window.dispatchEvent(event);
    }
    function normalizeError(raw) {
        return isObject(raw) ? {
            codigo: asText(raw.Codigo),
            mensajeVisible: asText(raw.MensajeVisible || raw.MensajeFuncional, "No fue posible consultar el usuario anterior.")
        } : null;
    }
    function normalizarPrevisualizacion(raw) {
        var context;
        if (!isObject(raw)) { throw new Error("La respuesta de previsualización no tiene el formato esperado."); }
        context = isObject(raw.Contexto) ? raw.Contexto : {};
        return {
            idTarea: asPositiveInteger(raw.IdTarea),
            contexto: {
                actividadActual: asText(context.ActividadActual, "No disponible"),
                actividadAnterior: asText(context.ActividadAnterior, "No disponible"),
                usuarioAnterior: asText(context.UsuarioAnterior, "No disponible")
            },
            tokenVersion: asText(raw.TokenVersion),
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
    function solicitarPrevisualizacion(idTarea, fetchImplementation, signal) {
        fetchImplementation = fetchImplementation || window.fetch;
        if (typeof fetchImplementation !== "function") {
            return Promise.reject(new Error("Este navegador no permite consultar el usuario anterior de forma segura."));
        }
        return fetchImplementation(previewUrl, {
            method: "POST",
            credentials: "same-origin",
            headers: { "Content-Type": "application/json; charset=utf-8" },
            body: JSON.stringify({ idTarea: idTarea }),
            signal: signal
        }).then(function (response) {
            if (!response || response.ok === false) { throw new Error("No fue posible consultar el usuario anterior."); }
            return response.json();
        }).then(desempaquetarRespuestaAsmx);
    }
    function addDefinition(container, label, value) {
        container.appendChild(createElement("dt", "workflow-transition-modal__context-label", label));
        container.appendChild(createElement("dd", "workflow-transition-modal__context-value", value));
    }
    function setStatus(control, state, message, kind) {
        control.modal.setAttribute("data-workflow-return-user-previous-state", state);
        control.status.setAttribute("data-workflow-return-user-previous-status", kind || "informacion");
        control.status.textContent = message || "";
    }
    function renderContext(control, preview) {
        empty(control.context);
        addDefinition(control.context, "Actividad actual", preview.contexto.actividadActual);
        addDefinition(control.context, "Actividad anterior", preview.contexto.actividadAnterior);
        addDefinition(control.context, "Usuario anterior", preview.contexto.usuarioAnterior);
    }
    function buildSelection(preview) {
        return {
            idTarea: preview.idTarea,
            tokenVersion: preview.tokenVersion,
            contexto: {
                actividadActual: preview.contexto.actividadActual,
                actividadAnterior: preview.contexto.actividadAnterior,
                usuarioAnterior: preview.contexto.usuarioAnterior
            }
        };
    }
    function isEligible(preview) {
        return !!preview && !preview.error && preview.idTarea > 0 && !!preview.tokenVersion;
    }
    function invalidateSelection(control) {
        if (control && control.preview) {
            emit("workflow:return-user-previous-invalidated", buildSelection(control.preview));
        }
    }
    function renderPreview(control, preview) {
        control.preview = preview;
        renderContext(control, preview);
        control.confirm.disabled = !isEligible(preview);
        if (preview.error) {
            setStatus(control, "bloqueo-funcional", preview.error.mensajeVisible, "error");
            return;
        }
        if (!isEligible(preview)) {
            setStatus(control, "respuesta-invalida", "No fue posible validar el usuario anterior.", "error");
            return;
        }
        setStatus(control, "listo-para-confirmar", "Revise el usuario histórico antes de continuar.", "informacion");
    }
    function renderTechnicalError(control, message) {
        var retry = createElement("button", "workflow-transition-modal__retry", "Reintentar");
        control.preview = null;
        control.confirm.disabled = true;
        empty(control.context);
        setStatus(control, "error-controlado", asText(message, "No fue posible consultar el usuario anterior."), "error");
        retry.type = "button";
        retry.addEventListener("click", function () { loadPreview(control); });
        control.status.appendChild(retry);
    }
    function currentTaskId(control) {
        var ids = [control.trigger.getAttribute("data-workflow-current-task-input-id"), control.trigger.getAttribute("data-workflow-task-input-id")];
        var index;
        var input;
        var value;
        for (index = 0; index < ids.length; index += 1) {
            input = ids[index] ? find(ids[index]) : null;
            value = input ? asPositiveInteger(input.value) : 0;
            if (value > 0) { return value; }
        }
        return 0;
    }
    function stopTimeout(control) {
        if (control.timeoutHandle !== null && typeof window.clearTimeout === "function") {
            window.clearTimeout(control.timeoutHandle);
        }
        control.timeoutHandle = null;
    }
    function cancelRequest(control) {
        control.requestSequence += 1;
        stopTimeout(control);
        if (control.abortController && typeof control.abortController.abort === "function") { control.abortController.abort(); }
        control.abortController = null;
        control.timedOut = false;
    }
    function loadPreview(control) {
        var idTarea = currentTaskId(control);
        var sequence;
        var signal;
        invalidateSelection(control);
        cancelRequest(control);
        control.preview = null;
        control.confirm.disabled = true;
        if (!idTarea) {
            renderTechnicalError(control, "Seleccione una tarea antes de devolverla.");
            return;
        }
        sequence = control.requestSequence;
        control.timedOut = false;
        if (typeof window.AbortController === "function") { control.abortController = new window.AbortController(); signal = control.abortController.signal; }
        if (control.abortController && typeof window.setTimeout === "function") {
            control.timeoutHandle = window.setTimeout(function () {
                if (sequence === control.requestSequence && control.abortController) {
                    control.timedOut = true;
                    control.abortController.abort();
                }
            }, requestTimeoutMilliseconds);
        }
        setStatus(control, "cargando", "Consultando el usuario anterior…", "informacion");
        solicitarPrevisualizacion(idTarea, null, signal).then(function (preview) {
            if (sequence !== control.requestSequence || !control.isOpen) { return; }
            stopTimeout(control);
            control.abortController = null;
            renderPreview(control, preview);
        }).catch(function (error) {
            if (sequence !== control.requestSequence || !control.isOpen) { return; }
            stopTimeout(control);
            control.abortController = null;
            if (error && error.name === "AbortError" && !control.timedOut) { return; }
            renderTechnicalError(control, control.timedOut ? "La consulta tardó demasiado. Intente nuevamente." : "No fue posible consultar el usuario anterior.");
        });
    }
    function focusable(control) {
        return control.dialog.querySelectorAll("button:not([disabled]), [href], input:not([disabled]), [tabindex]:not([tabindex='-1'])");
    }
    function openModal(control) {
        control.isOpen = true;
        control.modal.hidden = false;
        control.modal.removeAttribute("hidden");
        control.modal.setAttribute("aria-hidden", "false");
        document.body.classList.add("workflow-transition-modal-open");
        window.setTimeout(function () { control.close.focus(); }, 0);
        loadPreview(control);
    }
    function closeModal(control) {
        if (control.executionPending) {
            setStatus(control, "ejecucion-pendiente", "La devolución está en curso. Espere la respuesta antes de cerrar.", "informacion");
            return false;
        }
        invalidateSelection(control);
        cancelRequest(control);
        control.preview = null;
        control.isOpen = false;
        control.confirm.disabled = true;
        control.modal.hidden = true;
        control.modal.setAttribute("hidden", "hidden");
        control.modal.setAttribute("aria-hidden", "true");
        document.body.classList.remove("workflow-transition-modal-open");
        if (control.trigger && typeof control.trigger.focus === "function") { control.trigger.focus(); }
        setStatus(control, "cerrado", "", "informacion");
        return true;
    }
    function openConfirmation(control) {
        var selection;
        if (!isEligible(control.preview)) { return; }
        selection = buildSelection(control.preview);
        emit("workflow:return-user-previous-selected", selection);
        setStatus(control, "confirmacion-abierta", "Confirme la devolución para continuar.", "informacion");
    }
    function attach(control) {
        control.trigger.addEventListener("click", function () { if (!control.isOpen) { openModal(control); } });
        control.close.addEventListener("click", function () { closeModal(control); });
        control.confirm.addEventListener("click", function () { openConfirmation(control); });
        control.modal.querySelector("[data-workflow-return-user-previous-close]").addEventListener("click", function () { closeModal(control); });
        control.dialog.addEventListener("keydown", function (event) {
            var items;
            var first;
            var last;
            if (event.key === "Escape" || event.keyCode === 27) { event.preventDefault(); closeModal(control); return; }
            if (event.key !== "Tab" && event.keyCode !== 9) { return; }
            items = focusable(control);
            if (!items.length) { event.preventDefault(); control.dialog.focus(); return; }
            first = items[0];
            last = items[items.length - 1];
            if (event.shiftKey && document.activeElement === first) { event.preventDefault(); last.focus(); }
            else if (!event.shiftKey && document.activeElement === last) { event.preventDefault(); first.focus(); }
        });
    }
    function inicializar() {
        var trigger = find("workflow-return-user-previous-trigger");
        if (!trigger || trigger.getAttribute("data-workflow-return-user-previous-active") !== "true" || activeControl) { return false; }
        activeControl = {
            trigger: trigger,
            modal: find("workflow-return-user-previous-modern-modal"),
            dialog: find("workflow-return-user-previous-modern-dialog"),
            close: find("workflow-return-user-previous-modern-close"),
            status: find("workflow-return-user-previous-modern-status"),
            context: find("workflow-return-user-previous-modern-context"),
            confirm: find("workflow-return-user-previous-modern-confirm"),
            preview: null,
            abortController: null,
            timeoutHandle: null,
            timedOut: false,
            requestSequence: 0,
            isOpen: false,
            executionPending: false
        };
        if (!activeControl.modal || !activeControl.dialog || !activeControl.close || !activeControl.status || !activeControl.context || !activeControl.confirm) {
            activeControl = null;
            return false;
        }
        attach(activeControl);
        return true;
    }
    function establecerEjecucionPendiente(selection, pending) {
        if (!activeControl || !activeControl.preview || !selection || activeControl.preview.idTarea !== asPositiveInteger(selection.idTarea) || activeControl.preview.tokenVersion !== asText(selection.tokenVersion)) { return false; }
        activeControl.executionPending = pending === true;
        return true;
    }
    function aplicarDevolucionExitosa(selection) {
        if (!establecerEjecucionPendiente(selection, false)) { return false; }
        return closeModal(activeControl);
    }

    api.normalizarPrevisualizacion = normalizarPrevisualizacion;
    api.desempaquetarRespuestaAsmx = desempaquetarRespuestaAsmx;
    api.solicitarPrevisualizacion = solicitarPrevisualizacion;
    api.crearDetalleSeleccion = buildSelection;
    api.establecerEjecucionPendiente = establecerEjecucionPendiente;
    api.aplicarDevolucionExitosa = aplicarDevolucionExitosa;
    api.inicializar = inicializar;
    window.WorkflowReturnUserPreviousUi = api;
}(window, document));
