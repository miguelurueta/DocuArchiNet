(function (root) {
    "use strict";
    function text(value) { return value == null ? "" : String(value).trim(); }
    function normalize(row, taskId) {
        if (!row || !text(row.externalKey) || Number(taskId) <= 0) { throw new Error("PREPARATION_ITEM_INVALID"); }
        return { ClientItemId: text(row.clientItemId, text(row.externalKey)) || text(row.externalKey), ExternalKey: text(row.externalKey), TargetTaskId: Number(taskId), DocumentTypeId: row.documentTypeId == null ? null : Number(row.documentTypeId), DocumentTypeName: text(row.documentTypeName), FileName: text(row.fileName), ContentType: text(row.contentType) };
    }
    function unique(rows, taskId) {
        var seen = {}, items = [];
        (rows || []).forEach(function (row) { var item = normalize(row, taskId); if (seen[item.ExternalKey]) { throw new Error("PREPARATION_DUPLICATE_ITEM"); } seen[item.ExternalKey] = true; items.push(item); });
        if (!items.length) { throw new Error("PREPARATION_EMPTY"); }
        return items;
    }
    function individual(row, taskId) { return unique([row], taskId); }
    function multiple(rows, taskId) { return unique(rows, taskId); }
    function assignDocumentType(items, externalKey, option, catalog) {
        var allowed = (catalog || []).filter(function (entry) { return Number(entry.Id || entry.DocumentTypeId) === Number(option && (option.Id || option.DocumentTypeId)); })[0];
        if (!allowed) { throw new Error("DOCUMENT_TYPE_NOT_AUTHORIZED"); }
        return (items || []).map(function (item) { var copy = Object.assign({}, item); if (copy.ExternalKey === externalKey) { copy.DocumentTypeId = Number(allowed.Id || allowed.DocumentTypeId); copy.DocumentTypeName = text(allowed.Name || allowed.Nombre || allowed.DocumentTypeName); } return copy; });
    }
    var api = { individual: individual, multiple: multiple, assignDocumentType: assignDocumentType };
    root.ImportarServicioWebPreparation = api;
    if (typeof module !== "undefined" && module.exports) { module.exports = api; }
}(typeof window !== "undefined" ? window : globalThis));
