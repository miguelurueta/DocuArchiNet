(function (root) {
    "use strict";

    var visibleStates = {
        disponible: "Disponible", verificando: "Verificando", resultadoincierto: "ResultadoIncierto",
        inconsistente: "Inconsistente", completado: "Completado", completada: "Completado",
        parcial: "Parcial", detenido: "Detenido", detenida: "Detenido",
        fallido: "Fallido", fallida: "Fallido", fallidaantesdepersistir: "Fallido"
    };

    function text(value) { return value === undefined || value === null ? "" : String(value).replace(/^\s+|\s+$/g, ""); }
    function key(value) { var result = text(value).toLowerCase(); return result.normalize ? result.normalize("NFD").replace(/[\u0300-\u036f]/g, "") : result; }
    function number(value) { value = Number(value); return isFinite(value) && value > 0 ? value : 0; }
    function state(item) {
        var raw = item && (item.Status || item.status || item.backendPhase || item.ReachedPhase || item.reachedPhase || item.visibleState);
        return visibleStates[key(raw)] || "Verificando";
    }
    function mapItem(item) {
        item = item || {};
        return {
            clientItemId: text(item.ClientItemId || item.clientItemId), externalKey: text(item.ExternalKey || item.externalKey),
            status: state(item), documentId: number(item.DocumentId || item.documentId) || null,
            taskId: number(item.TaskId || item.taskId), documentName: text(item.DocumentName || item.documentName),
            contentType: text(item.ContentType || item.contentType), message: text(item.Message || item.message),
            errorCode: text(item.ErrorCode || item.errorCode), correlationId: text(item.CorrelationId || item.correlationId)
        };
    }
    function adapt(response) {
        var items;
        if (!response || response.Error || response.error) { throw new Error(text(response && (response.Error || response.error) && (response.Error || response.error).Codigo) || "IMPORT_RECONCILIATION_UNAVAILABLE"); }
        items = Array.isArray(response.Items) ? response.Items : (Array.isArray(response.items) ? response.items : []);
        return { intentId: text(response.IntentId || response.intentId), status: visibleStates[key(response.Status || response.status)] || "Verificando", versionToken: text(response.VersionToken || response.versionToken), items: items.map(mapItem) };
    }
    function request(base, externalKey) {
        var result = Object.assign({}, base || {});
        if (!text(result.IntentId)) { throw new Error("IMPORT_INTENT_ID_REQUIRED"); }
        if (number(result.TaskId) <= 0 || !text(result.ProviderId)) { throw new Error("IMPORT_RECONCILIATION_CONTEXT_REQUIRED"); }
        if (externalKey) { result.ExternalKey = text(externalKey); }
        return result;
    }
    function create(options) {
        options = options || {};
        var api = options.api;
        if (!api || typeof api.getImportIntent !== "function" || typeof api.reconcileImportIntent !== "function") { throw new Error("RECONCILIATION_DEPENDENCY_REQUIRED"); }
        function get(base) { return Promise.resolve(api.getImportIntent(request(base))).then(adapt); }
        function reconcile(base, externalKey) { return Promise.resolve(api.reconcileImportIntent(request(base, externalKey))).then(adapt); }
        function complete(execution, base) {
            var snapshot = adapt(execution), uncertain = snapshot.items.filter(function (item) { return item.status === "ResultadoIncierto" || item.status === "Verificando"; }), seen = {};
            uncertain = uncertain.filter(function (item) { if (!item.externalKey || seen[item.externalKey]) { return false; } seen[item.externalKey] = true; return true; });
            if (!uncertain.length) { return Promise.resolve(snapshot); }
            return Promise.all(uncertain.map(function (item) { return reconcile(base, item.externalKey); })).then(function (responses) {
                var replacements = {};
                responses.forEach(function (result) { result.items.forEach(function (item) { replacements[item.externalKey] = item; }); });
                snapshot.items = snapshot.items.map(function (item) { return replacements[item.externalKey] || item; });
                snapshot.status = snapshot.items.some(function (item) { return item.status !== "Disponible" && item.status !== "Completado"; }) ? "Parcial" : "Completado";
                return snapshot;
            });
        }
        return { get: get, reconcile: reconcile, complete: complete, adapt: adapt, mapItem: mapItem };
    }

    var api = { create: create, adapt: adapt, mapItem: mapItem, state: state };
    root.ImportarServicioWebReconciliation = api;
    if (typeof module !== "undefined" && module.exports) { module.exports = api; }
}(typeof window !== "undefined" ? window : globalThis));
