(function (window) {
    "use strict";

    var executionUrl = "../webservice/WebServiceWorkflowModern.asmx/EjecutarEnvioGrupo";
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
    function asWarnings(value) {
        return Array.isArray(value) ? value.map(function (item) { return asText(item); }).filter(Boolean) : [];
    }
    function unwrapAsmx(raw) {
        var value = isObject(raw) && Object.prototype.hasOwnProperty.call(raw, "d") ? raw.d : raw;
        if (typeof value === "string") { value = JSON.parse(value); }
        if (!isObject(value)) { throw new Error("El servicio devolvió una respuesta no válida."); }
        return value;
    }
    function normalizeSelection(detail) {
        var context, destination, idTarea, idActividadDestino, tokenVersion;
        detail = isObject(detail) ? detail : {};
        context = isObject(detail.contexto) ? detail.contexto : {};
        destination = isObject(detail.destino) ? detail.destino : {};
        idTarea = asPositiveInteger(detail.idTarea);
        idActividadDestino = asPositiveInteger(detail.idActividadDestino);
        tokenVersion = asText(detail.tokenVersion);
        if (!idTarea || !idActividadDestino || !tokenVersion) { return null; }
        return {
            idTarea: idTarea,
            idActividadDestino: idActividadDestino,
            tokenVersion: tokenVersion,
            contexto: { radicado: asText(context.radicado, "No disponible"), grupoActual: asText(context.grupoActual, "No disponible") },
            destino: { nombreActividad: asText(destination.nombreActividad, "Actividad disponible"), grupoDestino: asText(destination.grupoDestino, "No especificado") }
        };
    }
    function createSummaryFields(selection) {
        return [
            { label: "Radicado", value: selection.contexto.radicado },
            { label: "Grupo actual", value: selection.contexto.grupoActual },
            { label: "Actividad destino", value: selection.destino.nombreActividad },
            { label: "Grupo destino", value: selection.destino.grupoDestino }
        ];
    }
    function normalizeResult(raw) {
        var result = unwrapAsmx(raw);
        var success = result.Exito === true;
        return {
            status: success ? "success" : (result.EsReintentable === true ? "technical-error" : "blocked"),
            message: asText(result.MensajeFuncional, success ? "La tarea fue enviada correctamente." : "No fue posible enviar la tarea."),
            warnings: asWarnings(result.Advertencias),
            canRetry: result.EsReintentable === true,
            reference: asText(result.ReferenciaAuditoria),
            tokenVersion: asText(result.TokenVersion),
            raw: result
        };
    }
    function executeSend(context, fetchImplementation) {
        fetchImplementation = fetchImplementation || api.fetchImplementation || window.fetch;
        if (typeof fetchImplementation !== "function") { return Promise.reject(new Error("Este navegador no permite enviar la tarea de forma segura.")); }
        return fetchImplementation(executionUrl, {
            method: "POST",
            credentials: "same-origin",
            headers: { "Content-Type": "application/json; charset=utf-8" },
            body: JSON.stringify({ idTarea: context.idTarea, idActividadDestino: context.idActividadDestino, tokenVersion: context.tokenVersion })
        }).then(function (response) {
            if (!response || response.ok === false) { throw new Error("No fue posible enviar la tarea."); }
            return response.json();
        });
    }
    function notifySuccess(selection, result) {
        if (window.WorkflowTransitionPagePresentation && typeof window.WorkflowTransitionPagePresentation.applySuccess === "function") { window.WorkflowTransitionPagePresentation.applySuccess(selection); }
        if (window.WorkflowGroupSendUi && typeof window.WorkflowGroupSendUi.aplicarEnvioExitoso === "function") { window.WorkflowGroupSendUi.aplicarEnvioExitoso(selection); }
        if (typeof api.onSuccess === "function") { api.onSuccess(selection, result); }
    }
    function openFromSelection(detail) {
        var selection = normalizeSelection(detail);
        var opening;
        if (!selection || !window.ConfirmationDialog || typeof window.ConfirmationDialog.open !== "function") { return false; }
        opening = activeOpening + 1;
        activeOpening = opening;
        window.ConfirmationDialog.open({
            title: "Enviar tarea a grupo",
            primaryLabel: "Enviar a " + selection.destino.nombreActividad,
            cancelLabel: "Cancelar",
            confirmationNotice: "La tarea actual quedará finalizada.",
            summaryFields: createSummaryFields(selection),
            requirements: [], warnings: [],
            executionContext: { idTarea: selection.idTarea, idActividadDestino: selection.idActividadDestino, tokenVersion: selection.tokenVersion },
            labels: { close: "Cerrar confirmación", sending: "Enviando tarea…", sendingLocked: "La tarea se está enviando. Espere la respuesta antes de cerrar.", technicalError: "No fue posible enviar la tarea. Intente nuevamente." },
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
        window.addEventListener("workflow:group-destination-selected", function (event) { openFromSelection(event && event.detail); });
        window.addEventListener("workflow:group-destination-invalidated", function () {
            if (activeOpening <= 0) { return; }
            activeOpening += 1;
            if (window.ConfirmationDialog && typeof window.ConfirmationDialog.close === "function") {
                window.ConfirmationDialog.close();
            }
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
    window.WorkflowGroupSendConfirmation = api;
    initialize();
}(window));
