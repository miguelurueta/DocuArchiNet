(function (root) {
    "use strict";

    function number(value) { value = Number(value); return isFinite(value) && value > 0 ? value : 0; }
    function text(value) { return value === undefined || value === null ? "" : String(value).replace(/^\s+|\s+$/g, ""); }
    function legacyField(value) {
        return text(value).replace(/\|/g, "").replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, "&quot;").replace(/'/g, "&#39;");
    }
    function create(options) {
        options = options || {};
        var document = options.document, insertRow = options.insertRow, currentTaskId = options.currentTaskId;
        function contains(grid, documentId) {
            var rows = grid && grid.getElementsByTagName ? grid.getElementsByTagName("tr") : [], index;
            for (index = 0; index < rows.length; index += 1) {
                if (number(rows[index].getAttribute && rows[index].getAttribute("id_wf")) === documentId) { return true; }
            }
            return false;
        }
        return function append(documentItem) {
            var documentId = number(documentItem && documentItem.documentId), taskId = number(documentItem && documentItem.taskId);
            var projection = documentItem && documentItem.workflowProjection || {}, grid, data;
            if (!document || typeof insertRow !== "function" || typeof currentTaskId !== "function" || !documentId || !taskId || taskId !== number(currentTaskId())) { return false; }
            if (number(projection.documentId) !== documentId || number(projection.taskId) !== taskId || !text(projection.cabinetName) || !text(projection.radicado) || !text(projection.storageType) || !text(projection.documentTypeName) || !text(projection.iconClass)) { return false; }
            grid = document.getElementById("GridView_list_documento_relacion_wf");
            if (!grid) { return false; }
            if (contains(grid, documentId)) { return true; }
            data = [legacyField(projection.cabinetName), documentId, legacyField(projection.radicado), legacyField(projection.storageType), legacyField(projection.documentTypeName), taskId, Number(projection.signatureStatus) || 0, legacyField(projection.iconClass)].join("|");
            insertRow(data, "wf", 1);
            return contains(grid, documentId);
        };
    }

    var api = { create: create };
    root.ImportarServicioWebWorkflowDocumentListAdapter = api;
    if (typeof module !== "undefined" && module.exports) { module.exports = api; }
}(typeof window !== "undefined" ? window : globalThis));
