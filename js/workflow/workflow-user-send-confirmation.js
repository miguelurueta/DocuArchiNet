(function (window) {
    "use strict";

    var executeUrl = "../webservice/WebServiceWorkflowModern.asmx/EjecutarEnvioUsuario";
    var activeOpening = 0;
    var initialized = false;
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
    function normalizeSelection(selection) {
        selection = isObject(selection) ? selection : {};
        var context = isObject(selection.contexto) ? selection.contexto : {};
        var destination = isObject(selection.destino) ? selection.destino : {};
        return {
            idTarea: asPositiveInteger(selection.idTarea),
            idUsuarioWorkflowDestino: asPositiveInteger(selection.idUsuarioWorkflowDestino),
            idActividadDestino: asPositiveInteger(selection.idActividadDestino),
            tokenVersion: asText(selection.tokenVersion),
            contexto: { radicado: asText(context.radicado, "No disponible"), grupoActual: asText(context.grupoActual, "No disponible") },
            destino: {
                nombreUsuarioDestino: asText(destination.nombreUsuarioDestino, "Usuario disponible"),
                cargoUsuarioDestino: asText(destination.cargoUsuarioDestino, "No especificado"),
                nombreActividadDestino: asText(destination.nombreActividadDestino, "Actividad disponible")
            }
        };
    }
    function isValidSelection(selection) {
        return selection.idTarea > 0 && selection.idUsuarioWorkflowDestino > 0 && selection.idActividadDestino > 0 && !!selection.tokenVersion;
    }
    function createSummaryFields(selection) {
        return [
            { label: "Radicado", value: selection.contexto.radicado },
            { label: "Usuario destino", value: selection.destino.nombreUsuarioDestino },
            { label: "Cargo", value: selection.destino.cargoUsuarioDestino },
            { label: "Actividad destino", value: selection.destino.nombreActividadDestino }
        ];
    }
    function unwrapAsmx(raw) {
        var content;
        if (!isObject(raw) || !Object.prototype.hasOwnProperty.call(raw, "d")) { throw new Error("La respuesta del servicio no contiene el envoltorio ASMX esperado."); }
        content = raw.d;
        if (typeof content === "string") {
            try { content = JSON.parse(content); } catch (ignored) { throw new Error("La respuesta del servicio no contiene JSON válido."); }
        }
        return isObject(content) ? content : {};
    }
    function normalizeResult(raw) {
        var content = unwrapAsmx(raw);
        var error = isObject(content.Error) ? content.Error : {};
        var success = content.Exito === true;
        var message = asText(content.MensajeFuncional || error.MensajeVisible, success ? "La tarea fue enviada correctamente." : "No fue posible enviar la tarea.");
        return {
            status: success ? "success" : "blocked",
            message: message,
            warnings: asArray(content.Advertencias),
            canRetry: content.EsReintentable === true,
            tokenVersion: asText(content.TokenVersion),
            reference: asText(content.ReferenciaAuditoria),
            raw: content
        };
    }
    function executeSend(context, fetchImplementation) {
        fetchImplementation = fetchImplementation || api.fetchImplementation || window.fetch;
        if (typeof fetchImplementation !== "function") { return Promise.reject(new Error("Este navegador no permite enviar la tarea de forma segura.")); }
        return fetchImplementation(executeUrl, {
            method: "POST",
            credentials: "same-origin",
            headers: { "Content-Type": "application/json; charset=utf-8" },
            body: JSON.stringify({
                idTarea: context.idTarea,
                idUsuarioWorkflowDestino: context.idUsuarioWorkflowDestino,
                idActividadDestino: context.idActividadDestino,
                tokenVersion: context.tokenVersion
            })
        }).then(function (response) {
            if (!response || response.ok === false) { throw new Error("No fue posible enviar la tarea."); }
            return response.json();
        });
    }
    function notifySuccess(selection, result) {
        if (window.WorkflowUserSendUi && typeof window.WorkflowUserSendUi.aplicarEnvioExitoso === "function") {
            window.WorkflowUserSendUi.aplicarEnvioExitoso(selection);
        }
        if (window.WorkflowTransitionPagePresentation && typeof window.WorkflowTransitionPagePresentation.applySuccess === "function") {
            window.WorkflowTransitionPagePresentation.applySuccess(selection, {
                successElementId: "workflow-user-send-success-message",
                successMessage: result.message
            });
        }
        if (typeof api.onSuccess === "function") {
            try { api.onSuccess(selection, result); } catch (ignored) {}
        }
    }
    function openFromSelection(rawSelection) {
        var selection = normalizeSelection(rawSelection);
        var opening;
        if (!isValidSelection(selection) || !window.ConfirmationDialog || typeof window.ConfirmationDialog.open !== "function") { return false; }
        opening = activeOpening + 1;
        activeOpening = opening;
        window.ConfirmationDialog.open({
            title: "Confirmar envío a usuario",
            primaryLabel: "Enviar tarea",
            cancelLabel: "Cancelar",
            summaryFields: createSummaryFields(selection),
            confirmationNotice: "El destino se volverá a validar antes de enviar la tarea.",
            executionContext: {
                idTarea: selection.idTarea,
                idUsuarioWorkflowDestino: selection.idUsuarioWorkflowDestino,
                idActividadDestino: selection.idActividadDestino,
                tokenVersion: selection.tokenVersion
            },
            labels: {
                close: "Cerrar confirmación de envío a usuario",
                sending: "Enviando tarea…",
                sendingLocked: "La tarea se está enviando. Espere la respuesta antes de cerrar.",
                technicalError: "No fue posible enviar la tarea. Intente nuevamente."
            },
            execute: executeSend,
            normalizeResult: function (raw) {
                var result = normalizeResult(raw);
                return opening !== activeOpening || (result.tokenVersion && result.tokenVersion !== selection.tokenVersion) ? { status: "ignored", message: "" } : result;
            },
            onSuccess: function (result) {
                if (opening !== activeOpening) { return; }
                notifySuccess(selection, result);
                window.ConfirmationDialog.close();
            },
            onCancel: function () { if (opening === activeOpening) { activeOpening = 0; } }
        });
        return true;
    }
    function initialize() {
        if (initialized || typeof window.addEventListener !== "function") { return; }
        initialized = true;
        window.addEventListener("workflow:user-destination-selected", function (event) { openFromSelection(event && event.detail); });
        window.addEventListener("workflow:user-destination-invalidated", function () {
            if (activeOpening <= 0) { return; }
            activeOpening += 1;
            if (window.ConfirmationDialog && typeof window.ConfirmationDialog.close === "function") { window.ConfirmationDialog.close(); }
        });
    }

    api.normalizeSelection = normalizeSelection;
    api.createSummaryFields = createSummaryFields;
    api.normalizeResult = normalizeResult;
    api.executeSend = executeSend;
    api.openFromSelection = openFromSelection;
    api.initialize = initialize;
    api.fetchImplementation = null;
    api.onSuccess = null;
    window.WorkflowUserSendConfirmation = api;
    initialize();
}(window));
