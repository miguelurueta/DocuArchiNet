(function (root) {
    "use strict";

    var recoverableReasons = { timeout: true, "lost-response": true, "authorized-reopen": true };

    function text(value) { return value === undefined || value === null ? "" : String(value).replace(/^\s+|\s+$/g, ""); }
    function normalized(value) { return text(value).toLowerCase().normalize ? text(value).toLowerCase().normalize("NFD").replace(/[\u0300-\u036f]/g, "") : text(value).toLowerCase(); }

    function visibleState(item) {
        var phase = normalized(item && (item.Status || item.ReachedPhase)), confirmed = Number(item && item.DocumentId) > 0;
        if (phase === "creada") { return "Disponible"; }
        if (phase === "disponible" && confirmed) { return "Importada"; }
        if (phase === "disponible") { return "Verificando"; }
        if (phase === "validada") { return "Preparando"; }
        if (["recursoobtenido", "expedientepreparado", "documentoalmacenado", "indicesactualizados", "cacheactualizado"].indexOf(phase) >= 0) { return "Procesando"; }
        if (phase === "resultadoincierto") { return "Verificando"; }
        if ((phase === "reconciliada" || phase === "completada") && confirmed) { return "Importada"; }
        if (phase === "reconciliada" || phase === "completada") { return "Verificando"; }
        if (phase === "requieredecision") { return "Requiere decisión"; }
        if (phase === "fallidaantesdepersistir") { return "Fallida"; }
        if (phase === "parcial") { return "Parcial"; }
        if (phase === "detenida") { return "No procesada"; }
        if (phase === "omitida") { return "Omitida"; }
        return "No procesada";
    }

    function mapItem(item) {
        item = item || {};
        return {
            clientItemId: text(item.ClientItemId), externalKey: text(item.ExternalKey), backendPhase: text(item.Status || item.ReachedPhase),
            visibleState: visibleState(item), message: text(item.Message), errorCode: text(item.ErrorCode), documentId: item.DocumentId || null,
            taskId: Number(item.TaskId) || 0, documentName: text(item.DocumentName), contentType: text(item.ContentType),
            persistenceKnown: item.PersistenceKnown === true, retryable: item.Retryable === true, correlationId: text(item.CorrelationId)
        };
    }

    function summarize(items) {
        var summary = { saved: 0, skipped: 0, failed: 0, notProcessed: 0 };
        items.forEach(function (item) {
            if (item.visibleState === "Importada") { summary.saved += 1; }
            else if (item.visibleState === "Omitida") { summary.skipped += 1; }
            else if (item.visibleState === "No procesada" || item.visibleState === "Procesando" || item.visibleState === "Preparando" || item.visibleState === "Disponible" || item.visibleState === "Verificando") { summary.notProcessed += 1; }
            else { summary.failed += 1; }
        });
        summary.guardadas = summary.saved; summary.omitidas = summary.skipped; summary.fallidas = summary.failed; summary.noProcesadas = summary.notProcessed;
        return summary;
    }

    function adapt(response) {
        var items;
        if (!response || response.Error || !Array.isArray(response.Items)) { throw new Error(response && response.Error ? text(response.Error.Codigo) : "IMPORT_EXECUTION_RESPONSE_INVALID"); }
        items = response.Items.map(mapItem);
        return { intentId: text(response.IntentId), phase: text(response.Status), status: text(response.Status), versionToken: text(response.VersionToken), items: items, summary: summarize(items), isComplete: true, isTotalSuccess: items.length > 0 && items.every(function (item) { return item.visibleState === "Importada"; }) };
    }

    function create(options) {
        options = options || {};
        var api = options.api, pending = {};
        if (!api || typeof api.executeImportIntent !== "function" || typeof api.getImportIntent !== "function") { throw new Error("PROGRESS_ADAPTER_DEPENDENCY_REQUIRED"); }
        function execute(request) {
            var intentId = text(request && request.IntentId);
            if (!intentId) { return Promise.reject(new Error("IMPORT_INTENT_ID_REQUIRED")); }
            if (pending[intentId]) { return pending[intentId]; }
            pending[intentId] = Promise.resolve(api.executeImportIntent(Object.assign({}, request))).then(adapt).finally(function () { delete pending[intentId]; });
            return pending[intentId];
        }
        function recover(request, reason) {
            if (!recoverableReasons[text(reason).toLowerCase()]) { return Promise.reject(new Error("IMPORT_RECOVERY_REASON_REQUIRED")); }
            if (!text(request && request.IntentId)) { return Promise.reject(new Error("IMPORT_INTENT_ID_REQUIRED")); }
            return Promise.resolve(api.getImportIntent(Object.assign({}, request))).then(adapt);
        }
        return { execute: execute, recover: recover, adapt: adapt, mapItem: mapItem, visibleState: visibleState };
    }

    var api = { create: create, adapt: adapt, mapItem: mapItem, visibleState: visibleState, summarize: summarize };
    root.ImportarServicioWebProgressAdapter = api;
    if (typeof module !== "undefined" && module.exports) { module.exports = api; }
}(typeof window !== "undefined" ? window : globalThis));
