(function (window) {
    "use strict";

    var executionUrl = "../webservice/WebServiceWorkflowModern.asmx/EjecutarEnvioTarea";
    var activeOpening = 0;
    var initialized = false;
    var api = {};

    function isObject(value) {
        return value !== null && typeof value === "object" && !Array.isArray(value);
    }

    function asText(value, fallback) {
        var text = value === undefined || value === null ? "" : String(value).replace(/^\s+|\s+$/g, "");
        return text || (fallback || "");
    }

    function asPositiveInteger(value) {
        var parsed = Number(value);
        return isFinite(parsed) && parsed > 0 && Math.floor(parsed) === parsed ? parsed : 0;
    }

    function asWarnings(value) {
        return Array.isArray(value) ? value.map(function (item) {
            return asText(item);
        }).filter(function (item) {
            return !!item;
        }) : [];
    }

    function unwrapAsmx(raw) {
        var value = raw;

        if (isObject(raw) && Object.prototype.hasOwnProperty.call(raw, "d")) {
            value = raw.d;
        }
        if (typeof value === "string") {
            value = JSON.parse(value);
        }
        if (!isObject(value)) {
            throw new Error("El servicio devolvió una respuesta no válida.");
        }
        return value;
    }

    function normalizeSelection(detail) {
        var context;
        var destination;
        var idTarea;
        var idConector;
        var tokenVersion;

        detail = isObject(detail) ? detail : {};
        context = isObject(detail.contexto) ? detail.contexto : {};
        destination = isObject(detail.destino) ? detail.destino : {};
        idTarea = asPositiveInteger(detail.idTarea);
        idConector = asPositiveInteger(detail.idConector);
        tokenVersion = asText(detail.tokenVersion);
        if (!idTarea || !idConector || !tokenVersion) {
            return null;
        }

        return {
            idTarea: idTarea,
            idConector: idConector,
            tokenVersion: tokenVersion,
            tipoDecision: asText(detail.tipoDecision, "No especificado"),
            contexto: {
                radicado: asText(context.radicado, "No disponible"),
                grupoActual: asText(context.grupoActual, "No disponible")
            },
            destino: {
                nombre: asText(destination.nombre, "Destino disponible"),
                destinatario: asText(destination.destinatario),
                grupo: asText(destination.grupo),
                tipo: asText(destination.tipo, "No especificado")
            }
        };
    }

    function recipientOrGroup(destination) {
        return destination.destinatario || destination.grupo || "No especificado";
    }

    function createSummaryFields(selection) {
        return [
            { label: "Radicado", value: selection.contexto.radicado },
            { label: "Tipo", value: selection.tipoDecision },
            { label: "Grupo actual", value: selection.contexto.grupoActual },
            { label: "Actividad destino", value: selection.destino.nombre },
            { label: "Destinatario o grupo", value: recipientOrGroup(selection.destino) },
            { label: "Mecanismo", value: selection.destino.tipo }
        ];
    }

    function normalizeResult(raw) {
        var result = unwrapAsmx(raw);
        var message = asText(result.MensajeFuncional,
            result.Exito === true ? "La tarea fue enviada correctamente." : "No fue posible enviar la tarea.");
        var isSuccess = result.Exito === true;
        var isTechnical = !isSuccess && result.EsReintentable === true;

        return {
            status: isSuccess ? "success" : (isTechnical ? "technical-error" : "blocked"),
            message: message,
            warnings: asWarnings(result.Advertencias),
            canRetry: result.EsReintentable === true,
            reference: asText(result.ReferenciaAuditoria),
            tokenVersion: asText(result.TokenVersion),
            raw: result
        };
    }

    function executeSend(context, fetchImplementation) {
        var response;

        fetchImplementation = fetchImplementation || api.fetchImplementation || window.fetch;
        if (typeof fetchImplementation !== "function") {
            return Promise.reject(new Error("Este navegador no permite enviar la tarea de forma segura."));
        }

        return fetchImplementation(executionUrl, {
            method: "POST",
            credentials: "same-origin",
            headers: { "Content-Type": "application/json; charset=utf-8" },
            body: JSON.stringify({
                idTarea: context.idTarea,
                idConector: context.idConector,
                tokenVersion: context.tokenVersion
            })
        }).then(function (httpResponse) {
            response = httpResponse;
            if (!response || response.ok === false) {
                throw new Error("No fue posible enviar la tarea.");
            }
            return response.json();
        });
    }

    function dispatch(name, detail) {
        var event;
        if (typeof window.CustomEvent === "function") {
            event = new window.CustomEvent(name, { detail: detail });
        } else if (window.document && window.document.createEvent) {
            event = window.document.createEvent("CustomEvent");
            event.initCustomEvent(name, false, false, detail);
        }
        if (event && typeof window.dispatchEvent === "function") {
            window.dispatchEvent(event);
        }
    }

    function notifySuccess(selection, result) {
        if (window.WorkflowTransitionPagePresentation && typeof window.WorkflowTransitionPagePresentation.applySuccess === "function") {
            window.WorkflowTransitionPagePresentation.applySuccess(selection);
        }
        if (window.WorkflowTransitionUi && typeof window.WorkflowTransitionUi.aplicarTransicionExitosa === "function") {
            window.WorkflowTransitionUi.aplicarTransicionExitosa(selection);
        }
        dispatch("workflow:transition-succeeded", {
            idTarea: selection.idTarea,
            idConector: selection.idConector,
            tokenVersion: selection.tokenVersion,
            result: result.raw,
            reference: result.reference
        });
        if (typeof api.onSuccess === "function") {
            api.onSuccess(selection, result);
        }
    }

    function openFromSelection(detail) {
        var selection = normalizeSelection(detail);
        var opening;
        var config;

        if (!selection || !window.ConfirmationDialog || typeof window.ConfirmationDialog.open !== "function") {
            return false;
        }

        opening = activeOpening + 1;
        activeOpening = opening;
        config = {
            title: "Enviar tarea",
            primaryLabel: "Enviar a " + selection.destino.nombre,
            cancelLabel: "Cancelar",
            confirmationNotice: "La tarea actual quedará finalizada.",
            summaryFields: createSummaryFields(selection),
            requirements: [],
            warnings: [],
            executionContext: {
                idTarea: selection.idTarea,
                idConector: selection.idConector,
                tokenVersion: selection.tokenVersion
            },
            labels: {
                close: "Cerrar confirmación",
                sending: "Enviando tarea…",
                sendingLocked: "La tarea se está enviando. Espere la respuesta antes de cerrar.",
                technicalError: "No fue posible enviar la tarea. Intente nuevamente."
            },
            execute: function (context) {
                return executeSend(context);
            },
            normalizeResult: function (raw) {
                var result = normalizeResult(raw);
                if (opening !== activeOpening || (result.tokenVersion && result.tokenVersion !== selection.tokenVersion)) {
                    return { status: "ignored", message: "" };
                }
                return result;
            },
            onSuccess: function (result) {
                if (opening !== activeOpening) {
                    return;
                }
                notifySuccess(selection, result);
                window.ConfirmationDialog.close();
            },
            onCancel: function () {
                if (opening === activeOpening) {
                    activeOpening = 0;
                }
            }
        };
        window.ConfirmationDialog.open(config);
        return true;
    }

    function initialize() {
        if (initialized || typeof window.addEventListener !== "function") {
            return;
        }
        initialized = true;
        window.addEventListener("workflow:destination-selected", function (event) {
            openFromSelection(event && event.detail);
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
    window.WorkflowTransitionConfirmationIntegration = api;
    initialize();
}(window));
