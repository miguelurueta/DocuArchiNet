(function (window) {
    "use strict";

    var executeUrl = "../webservice/WebServiceWorkflowModern.asmx/EjecutarDevolverActividad";
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
    function validSelection(selection) {
        return isObject(selection) && asPositiveInteger(selection.idTarea) > 0 && asPositiveInteger(selection.idConector) > 0 && asText(selection.tokenVersion);
    }
    function unwrap(raw) {
        var value;
        if (!isObject(raw) || !Object.prototype.hasOwnProperty.call(raw, "d")) { throw new Error("La respuesta de devolución no tiene el formato esperado."); }
        value = raw.d;
        if (typeof value === "string") {
            try { value = JSON.parse(value); } catch (ignored) { throw new Error("La respuesta de devolución no contiene JSON válido."); }
        }
        return isObject(value) ? value : {};
    }
    function normalizeResult(raw) {
        var value = unwrap(raw);
        var error = isObject(value.Error) ? value.Error : {};
        var message = asText(value.MensajeFuncional || error.MensajeVisible, "No fue posible devolver la tarea.");
        if (value.Exito === true) {
            return { status: "success", message: message, idTarea: asPositiveInteger(value.IdTarea), actividadDestino: asText(value.ActividadDestino), warnings: Array.isArray(value.Advertencias) ? value.Advertencias : [] };
        }
        return { status: "blocked", message: message, code: asText(value.CodigoBloqueo || error.Codigo), canRetry: value.EsReintentable === true };
    }
    function execute(selection, fetchImplementation) {
        fetchImplementation = fetchImplementation || window.fetch;
        if (!validSelection(selection)) { return Promise.reject(new Error("La selección de devolución dejó de ser válida.")); }
        if (typeof fetchImplementation !== "function") { return Promise.reject(new Error("Este navegador no permite devolver tareas de forma segura.")); }
        return fetchImplementation(executeUrl, {
            method: "POST",
            credentials: "same-origin",
            headers: { "Content-Type": "application/json; charset=utf-8" },
            body: JSON.stringify({ idTarea: selection.idTarea, idConector: selection.idConector, tokenVersion: selection.tokenVersion })
        }).then(function (response) {
            if (!response || response.ok === false) { throw new Error("No fue posible devolver la tarea."); }
            return response.json();
        });
    }
    function setExecutionPending(selection, pending) {
        if (window.WorkflowReturnActivityUi && typeof window.WorkflowReturnActivityUi.establecerEjecucionPendiente === "function") {
            return window.WorkflowReturnActivityUi.establecerEjecucionPendiente(selection, pending === true);
        }
        return false;
    }
    function executeWithLock(selection) {
        setExecutionPending(selection, true);
        return execute(selection, api.fetchImplementation).then(function (raw) {
            setExecutionPending(selection, false);
            return raw;
        }, function (error) {
            setExecutionPending(selection, false);
            throw error;
        });
    }
    function openFromSelection(selection) {
        if (!validSelection(selection) || !window.ConfirmationDialog || typeof window.ConfirmationDialog.open !== "function") { return false; }
        window.ConfirmationDialog.open({
            title: "Confirmar devolución de tarea",
            primaryLabel: "Devolver tarea",
            cancelLabel: "Cancelar",
            labels: {
                close: "Cerrar confirmación de devolución",
                sending: "Devolviendo tarea…",
                sendingLocked: "La devolución está en curso. Espere la respuesta antes de cerrar.",
                technicalError: "No fue posible completar la devolución. Intente nuevamente."
            },
            summaryFields: [
                { label: "Actividad anterior", value: selection.destino.nombreActividad },
                { label: "Destino", value: selection.destino.destinatario },
                { label: "Contexto", value: selection.destino.tipoContexto }
            ],
            confirmationNotice: "El servidor volverá a validar el destino, el token y la concurrencia antes de devolver la tarea.",
            executionContext: { idTarea: selection.idTarea, idConector: selection.idConector, tokenVersion: selection.tokenVersion },
            execute: function () { return executeWithLock(selection); },
            normalizeResult: normalizeResult,
            onSuccess: function (result) {
                if (window.WorkflowReturnActivityUi && typeof window.WorkflowReturnActivityUi.aplicarDevolucionExitosa === "function") {
                    window.WorkflowReturnActivityUi.aplicarDevolucionExitosa(selection);
                }
                if (window.WorkflowTransitionPagePresentation && typeof window.WorkflowTransitionPagePresentation.applySuccess === "function") {
                    window.WorkflowTransitionPagePresentation.applySuccess({ idTarea: selection.idTarea }, { successElementId: "workflow-return-activity-success-message", successMessage: result.message });
                }
                if (window.ConfirmationDialog && typeof window.ConfirmationDialog.close === "function") { window.ConfirmationDialog.close(); }
            },
            onBlocked: function () {},
            onTechnicalError: function () {},
            onCancel: function () {}
        });
        return true;
    }

    if (typeof window.addEventListener === "function") {
        window.addEventListener("workflow:return-activity-selected", function (event) { openFromSelection(event && event.detail); });
        window.addEventListener("workflow:return-activity-invalidated", function () {
            if (window.ConfirmationDialog && typeof window.ConfirmationDialog.close === "function") { window.ConfirmationDialog.close(); }
        });
    }

    api.openFromSelection = openFromSelection;
    api.execute = execute;
    api.executeWithLock = executeWithLock;
    api.normalizeResult = normalizeResult;
    api.fetchImplementation = null;
    window.WorkflowReturnActivityConfirmation = api;
}(window));
