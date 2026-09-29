(function (root) {
    "use strict";

    function number(value) { value = Number(value); return isFinite(value) && value > 0 ? value : 0; }
    function text(value) { return value === undefined || value === null ? "" : String(value).replace(/^\s+|\s+$/g, ""); }
    function legacyField(value) {
        return text(value).replace(/\|/g, "").replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, "&quot;").replace(/'/g, "&#39;");
    }
    function createLegacyGridAppender(options) {
        options = options || {};
        var document = options.document, insertRow = options.insertRow, resolveTarget = options.resolveTarget;
        function contains(grid, documentId, rowIdAttribute) {
            var rows = grid && grid.getElementsByTagName ? grid.getElementsByTagName("tr") : [], index;
            for (index = 0; index < rows.length; index += 1) {
                if (number(rows[index].getAttribute && rows[index].getAttribute(rowIdAttribute)) === documentId) { return true; }
            }
            return false;
        }
        function enlaseData(documentItem, documentId, taskId) {
            var value = documentItem && documentItem.enlaseProjection || {};
            if (number(value.documentId) !== documentId || number(value.taskId) !== taskId || !text(value.cabinetName) ||
                !text(value.radicado) || !text(value.storageType) || !text(value.documentName) || !text(value.iconClass)) { return ""; }
            return [legacyField(value.cabinetName), documentId, legacyField(value.radicado), legacyField(value.storageType),
                legacyField(value.documentName), taskId, Number(value.signatureStatus) || 0, legacyField(value.iconClass)].join("|");
        }
        return function append(documentItem) {
            var documentId = number(documentItem && documentItem.documentId), taskId = number(documentItem && documentItem.taskId), target, grid, legacyData;
            if (!document || !documentId || !taskId || typeof insertRow !== "function") { return false; }
            target = typeof resolveTarget === "function" ? resolveTarget() : null;
            target = target || { gridId: "GridView_list_documento_relacion_wf", destination: "wf", rowIdAttribute: "id_wf" };
            grid = document.getElementById(target.gridId);
            if (!grid) { return false; }
            if (contains(grid, documentId, target.rowIdAttribute)) { return true; }
            // ENLASE y Workflow tienen contratos legacy diferentes. Solo la frontera cliente crea la cadena delimitada.
            legacyData = target.destination === "rad" ? enlaseData(documentItem, documentId, taskId) :
                ["", documentId, "", "", legacyField(documentItem.documentName) || "Documento importado", taskId, "", "fa-file"].join("|");
            if (!legacyData) { return false; }
            insertRow(legacyData, target.destination, 1);
            return contains(grid, documentId, target.rowIdAttribute);
        };
    }
    function create(options) {
        options = options || {};
        var currentTaskId = options.currentTaskId, appendDocument = options.appendDocument, refresh = options.refresh, openDocument = options.openDocument;
        if (typeof currentTaskId !== "function") { throw new Error("DOCUMENT_LIST_TASK_REQUIRED"); }
        function collect(snapshot) {
            var task = number(currentTaskId()), seen = {}, confirmed = [];
            (snapshot && snapshot.items || []).forEach(function (item) {
                var documentId = number(item && (item.documentId || item.DocumentId)), itemTask = number(item && (item.taskId || item.TaskId)), status = String(item && (item.status || item.Status || item.visibleState) || "");
                if ((status !== "Disponible" && status !== "Completado") || !documentId || !task || itemTask !== task || seen[documentId]) { return; }
                seen[documentId] = true;
                confirmed.push({ documentId: documentId, taskId: itemTask, externalKey: String(item.externalKey || item.ExternalKey || ""), documentName: String(item.documentName || item.DocumentName || ""), contentType: String(item.contentType || item.ContentType || ""), enlaseProjection: item.enlaseProjection || item.EnlaseProjection || null });
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

    var api = { create: create, createLegacyGridAppender: createLegacyGridAppender };
    root.ImportarServicioWebDocumentListAdapter = api;
    if (typeof module !== "undefined" && module.exports) { module.exports = api; }
}(typeof window !== "undefined" ? window : globalThis));
