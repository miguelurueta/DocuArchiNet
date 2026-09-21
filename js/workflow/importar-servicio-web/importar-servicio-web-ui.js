(function (window, document) {
    "use strict";

    var activeControl = null;

    function text(value, fallback) {
        value = value === undefined || value === null ? "" : String(value).replace(/^\s+|\s+$/g, "");
        return value || (fallback || "");
    }

    function taskId(control) {
        var input = document.getElementById(control.trigger.getAttribute("data-import-task-input-id"));
        var value = input ? Number(input.value) : 0;
        return isFinite(value) && value > 0 ? value : 0;
    }

    function requestContext(control) {
        return {
            SchemaVersion: "1.0",
            OperationId: "ui-" + String(new Date().getTime()),
            CorrelationId: "ui-" + String(new Date().getTime()),
            TaskId: taskId(control),
            ProviderId: control.providerId
        };
    }

    function createBackendAdapter(api, control) {
        return {
            capabilities: { multipleSelection: true, preview: true, download: true, documentType: true, additionalRequirements: true, allowedActions: ["query", "preview", "import"] },
            queryItems: function (request) {
                var context = requestContext(control);
                if (context.TaskId <= 0) { return Promise.reject(new Error("TASK_NOT_SELECTED")); }
                return api.resolveCapabilities(context).then(function (capabilities) {
                    if (capabilities.Error) { throw new Error(text(capabilities.Error.Codigo, "CAPABILITIES_UNAVAILABLE")); }
                    context.CodigoBarras = text(request && request.CodigoBarras);
                    context.PageSize = 50;
                    return api.queryItems(context);
                });
            },
            executeImportIntent: function (request) { return api.executeImportIntent(Object.assign(requestContext(control), request || {})); }
        };
    }

    function clear(node) { while (node && node.firstChild) { node.removeChild(node.firstChild); } }

    function renderItems(control, data) {
        var items = data && Array.isArray(data.Items) ? data.Items : [];
        clear(control.results);
        items.forEach(function (item) {
            var entry = document.createElement("li");
            entry.className = "importar-servicio-web__item";
            entry.textContent = text(item.DisplayName, "Documento disponible");
            control.results.appendChild(entry);
        });
    }

    function render(control, snapshot) {
        var messages = {
            "cerrado": "", "resolviendo-proveedor": "Resolviendo proveedor…", "consultando": "Consultando documentos…",
            "vacio": "No hay documentos disponibles.", "resultados": "Documentos disponibles.", "preparando": "Preparando importación…",
            "ejecutando": "Importando documentos. Espere…", "reconciliando": "Verificando resultado…",
            "completado": "Importación finalizada.", "error": "No fue posible abrir la importación: " + text(snapshot.message, "error no disponible") + "."
        };
        control.modal.setAttribute("data-import-state", snapshot.state);
        control.status.textContent = messages[snapshot.state] || text(snapshot.message);
        control.status.setAttribute("data-status-kind", snapshot.state === "error" ? "error" : "information");
        if (snapshot.state === "resultados" || snapshot.state === "completado") { renderItems(control, snapshot.data); }
    }

    function focusable(dialog) {
        return Array.prototype.filter.call(dialog.querySelectorAll("button:not([disabled]), [href], input:not([disabled]), [tabindex]:not([tabindex='-1'])"), function (element) {
            return !element.hidden && element.getAttribute("aria-hidden") !== "true" && (!element.getClientRects || element.getClientRects().length > 0);
        });
    }

    function close(control) {
        control.core.close();
        control.modal.hidden = true;
        control.modal.setAttribute("aria-hidden", "true");
        document.body.classList.remove("importar-servicio-web-open");
        if (control.trigger && typeof control.trigger.focus === "function") { control.trigger.focus(); }
    }

    function onKeydown(control, event) {
        var elements, first, last;
        if (event.key === "Escape" || event.keyCode === 27) { event.preventDefault(); close(control); return; }
        if (event.key !== "Tab" && event.keyCode !== 9) { return; }
        elements = focusable(control.dialog);
        if (!elements.length) { event.preventDefault(); control.dialog.focus(); return; }
        first = elements[0]; last = elements[elements.length - 1];
        if (event.shiftKey && document.activeElement === first) { event.preventDefault(); last.focus(); }
        else if (!event.shiftKey && document.activeElement === last) { event.preventDefault(); first.focus(); }
    }

    function open(control, event) {
        if (event) { event.preventDefault(); event.stopPropagation(); }
        control.modal.hidden = false;
        control.modal.removeAttribute("hidden");
        control.modal.setAttribute("aria-hidden", "false");
        document.body.classList.add("importar-servicio-web-open");
        control.closeButton.focus();
        control.core.open(control.providerId);
        return false;
    }

    function initialize(options) {
        options = options || {};
        var trigger = document.getElementById("ctw-document-action-service");
        var legacy = document.getElementById("btnloadservice");
        var modal = document.getElementById("importar-servicio-web-modal");
        var apiFactory = options.api || window.ImportarServicioWebApi;
        var registryFactory = options.registry || window.ImportarServicioWebProviderRegistry;
        var coreFactory = options.core || window.ImportarServicioWebCore;
        var control, registry, api;

        if (!trigger || trigger.getAttribute("data-import-modern-active") !== "true" || trigger.getAttribute("data-import-modern-bound") === "true") { return null; }
        if (!modal || !apiFactory || !registryFactory || !coreFactory) { return null; }
        control = { trigger: trigger, modal: modal, dialog: document.getElementById("importar-servicio-web-dialog"), closeButton: document.getElementById("importar-servicio-web-close"), status: document.getElementById("importar-servicio-web-status"), results: document.getElementById("importar-servicio-web-results"), providerId: text(trigger.getAttribute("data-import-provider-id")) };
        if (!control.dialog || !control.closeButton || !control.status || !control.results) { return null; }
        api = apiFactory.create(options.apiOptions || {});
        registry = registryFactory.create({ knownNotMigrated: options.knownNotMigrated || [] });
        if (control.providerId) { registry.register(control.providerId, createBackendAdapter(api, control)); }
        control.core = coreFactory.create({ registry: registry });
        control.core.subscribe(function (snapshot) { render(control, snapshot); });
        trigger.setAttribute("data-import-modern-bound", "true");
        trigger.onclick = function (event) { return open(control, event || window.event); };
        if (legacy) { legacy.hidden = true; legacy.setAttribute("aria-hidden", "true"); }
        control.closeButton.addEventListener("click", function () { close(control); });
        control.modal.addEventListener("click", function (event) { if (event.target && event.target.getAttribute("data-import-close") === "true") { close(control); } });
        control.dialog.addEventListener("keydown", function (event) { onKeydown(control, event); });
        activeControl = control;
        return control;
    }

    var ui = { initialize: initialize, open: open, close: close, onKeydown: onKeydown, createBackendAdapter: createBackendAdapter, getActiveControl: function () { return activeControl; } };
    window.ImportarServicioWebUi = ui;
    if (typeof module !== "undefined" && module.exports) { module.exports = ui; }
    if (window.Sys && window.Sys.Application && typeof window.Sys.Application.add_load === "function") { window.Sys.Application.add_load(initialize); }
    if (document && document.readyState === "loading") { document.addEventListener("DOMContentLoaded", initialize); }
    else if (document) { initialize(); }
}(typeof window !== "undefined" ? window : globalThis, typeof document !== "undefined" ? document : null));
