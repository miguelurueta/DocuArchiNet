(function (root) {
    "use strict";
    function text(value) { return value == null ? "" : String(value).replace(/^\s+|\s+$/g, ""); }
    function metadata(items) { var result = {}; (Array.isArray(items) ? items : []).forEach(function (item) { var code = text(item && item.Code).toUpperCase(); if (code) { result[code] = text(item.Value); } }); return result; }
    function value(meta, names) { var found = ""; names.some(function (name) { found = meta[name] || ""; return !!found; }); return found; }
    function mapItem(item) {
        if (!item || !text(item.ExternalKey) || !text(item.DisplayName)) { throw new Error("SII_QUERY_RESPONSE_INVALID"); }
        var meta = metadata(item.Metadata), actions = Array.isArray(item.AllowedActions) ? item.AllowedActions.map(text) : [], status = text(item.ImportStatus).toUpperCase();
        return { externalKey: text(item.ExternalKey), displayName: text(item.DisplayName), contentType: text(item.ContentType), previewAvailable: item.PreviewAvailable === true, importStatus: status, allowedActions: actions,
            book: value(meta, ["BOOK", "LIBRO"]), inscription: value(meta, ["INSCRIPTION", "INSCRIPCION"]), date: value(meta, ["DATE", "FECHA"]), act: value(meta, ["NATURE", "NATURALEZA", "ACT", "ACTO"]), news: value(meta, ["NEWS", "NOTICIA"]), reference: value(meta, ["REFERENCE", "REFERENCIA"]), documentTypeId: value(meta, ["DOCUMENT_TYPE_ID", "TIPOLOGIA"]), internalDocumentId: value(meta, ["INTERNAL_DOCUMENT_ID", "IMAGE_ID", "ID_IMAGEN"]),
            importable: status !== "IMPORTED" && actions.some(function (action) { return /^(IMPORT|IMPORTAR)$/i.test(action); }) };
    }
    function mapResponse(response) { if (response && response.Error) { throw new Error(text(response.Error.Codigo || response.Error.Code) || "SII_QUERY_FAILED"); } if (!response || !Array.isArray(response.Items)) { throw new Error("SII_QUERY_RESPONSE_INVALID"); } return { items: response.Items.map(mapItem), continuationToken: text(response.ContinuationToken), providerResultCode: text(response.ProviderResultCode) }; }
    var api = { mapItem: mapItem, mapResponse: mapResponse }; root.ImportarServicioWebSiiContractMapper = api; if (typeof module !== "undefined" && module.exports) { module.exports = api; }
}(typeof window !== "undefined" ? window : globalThis));
