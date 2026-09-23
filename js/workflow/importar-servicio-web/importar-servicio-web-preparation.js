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
    function catalogId(entry) { return Number(entry && (entry.Id || entry.DocumentTypeId)); }
    function catalogName(entry) { return text(entry && (entry.Name || entry.Nombre || entry.DocumentTypeName)); }
    function normalizeCatalogName(value) {
        return text(value).toUpperCase().replace(/[ÁÀÄÂ]/g, "A").replace(/[ÉÈËÊ]/g, "E").replace(/[ÍÌÏÎ]/g, "I").replace(/[ÓÒÖÔ]/g, "O").replace(/[ÚÙÜÛ]/g, "U").replace(/Ñ/g, "N").replace(/[^A-Z0-9]+/g, " ").trim();
    }
    function defaultDocumentType(catalog) {
        var allowed = (catalog || []).filter(function (entry) { return catalogId(entry) > 0 && catalogName(entry); }), matches;
        if (allowed.length === 1) { return allowed[0]; }
        matches = allowed.filter(function (entry) { var name = normalizeCatalogName(catalogName(entry)); return /(^| )CONSTANCIA( |$)/.test(name) && /(^| )INSCRIPCION( |$)/.test(name); });
        return matches.length === 1 ? matches[0] : null;
    }
    function assignDefaultDocumentType(items, catalog) {
        var selected = defaultDocumentType(catalog);
        if (!selected) { return { items: (items || []).slice(0), documentType: null }; }
        return { items: (items || []).map(function (item) { var copy = Object.assign({}, item); copy.DocumentTypeId = catalogId(selected); copy.DocumentTypeName = catalogName(selected); return copy; }), documentType: selected };
    }
    function assignDocumentType(items, externalKey, option, catalog) {
        var allowed = (catalog || []).filter(function (entry) { return Number(entry.Id || entry.DocumentTypeId) === Number(option && (option.Id || option.DocumentTypeId)); })[0];
        if (!allowed) { throw new Error("DOCUMENT_TYPE_NOT_AUTHORIZED"); }
        return (items || []).map(function (item) { var copy = Object.assign({}, item); if (copy.ExternalKey === externalKey) { copy.DocumentTypeId = Number(allowed.Id || allowed.DocumentTypeId); copy.DocumentTypeName = text(allowed.Name || allowed.Nombre || allowed.DocumentTypeName); } return copy; });
    }
    var api = { individual: individual, multiple: multiple, assignDocumentType: assignDocumentType, defaultDocumentType: defaultDocumentType, assignDefaultDocumentType: assignDefaultDocumentType };
    root.ImportarServicioWebPreparation = api;
    if (typeof module !== "undefined" && module.exports) { module.exports = api; }
}(typeof window !== "undefined" ? window : globalThis));
