(function (root) {
    "use strict";

    function number(value) { value = Number(value); return isFinite(value) && value > 0 ? value : 0; }
    function create(options) {
        options = options || {};
        var currentTaskId = options.currentTaskId, appendDocument = options.appendDocument, refresh = options.refresh, openDocument = options.openDocument;
        if (typeof currentTaskId !== "function") { throw new Error("DOCUMENT_LIST_TASK_REQUIRED"); }
        function collect(snapshot) {
            var task = number(currentTaskId()), seen = {}, confirmed = [];
            (snapshot && snapshot.items || []).forEach(function (item) {
                var documentId = number(item && (item.documentId || item.DocumentId)), itemTask = number(item && (item.taskId || item.TaskId)), status = String(item && (item.status || item.Status || item.visibleState) || "");
                if (status !== "Disponible" || !documentId || !task || itemTask !== task || seen[documentId]) { return; }
                seen[documentId] = true;
                confirmed.push({ documentId: documentId, taskId: itemTask, externalKey: String(item.externalKey || item.ExternalKey || ""), documentName: String(item.documentName || item.DocumentName || ""), contentType: String(item.contentType || item.ContentType || "") });
            });
            return confirmed;
        }
        function synchronize(snapshot) {
            var documents = collect(snapshot), appended = 0, requiresRefresh = false;
            documents.forEach(function (document) {
                if (typeof appendDocument === "function" && appendDocument(document) === true) { appended += 1; }
                else { requiresRefresh = true; }
            });
            if (requiresRefresh && typeof refresh === "function") { refresh({ taskId: number(currentTaskId()), documents: documents.slice(0) }); }
            return { documents: documents, appended: appended, refreshed: requiresRefresh };
        }
        function isAuthorized(item) {
            var status = String(item && (item.status || item.Status || item.backendPhase || item.visibleState) || "");
            return number(item && (item.documentId || item.DocumentId)) > 0 && number(item && (item.taskId || item.TaskId)) === number(currentTaskId()) && (status === "Disponible" || status === "Importada");
        }
        function canOpen(documentId) { return number(documentId) > 0 && typeof openDocument === "function"; }
        function open(documentId) { return canOpen(documentId) ? openDocument(number(documentId)) === true : false; }
        return { collect: collect, synchronize: synchronize, isAuthorized: isAuthorized, canOpen: canOpen, open: open };
    }

    var api = { create: create };
    root.ImportarServicioWebDocumentListAdapter = api;
    if (typeof module !== "undefined" && module.exports) { module.exports = api; }
}(typeof window !== "undefined" ? window : globalThis));
