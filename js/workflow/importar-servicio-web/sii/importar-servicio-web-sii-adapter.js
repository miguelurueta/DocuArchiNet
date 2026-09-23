(function (root) {
    "use strict";
    var canonicalId = "INTEGRACIONSII";
    function text(value) { return value == null ? "" : String(value); }
    function appendCell(row, value, className) { var cell = row.ownerDocument.createElement("td"); cell.textContent = text(value) || "—"; if (className) { cell.className = className; } row.appendChild(cell); }
    function appendHeader(row, value, className) { var header = row.ownerDocument.createElement("th"); header.scope = "col"; header.textContent = value; if (className) { header.className = className; } row.appendChild(header); }
    function create(options) {
        options = options || {};
        var api = options.api, mapper = options.mapper || root.ImportarServicioWebSiiContractMapper, model = options.list || root.ImportarServicioWebSiiList, contextFactory = options.contextFactory || function () { return {}; }, list;
        if (!api || !mapper || !model) { throw new Error("SII_ADAPTER_DEPENDENCY_REQUIRED"); }
        list = model.create({ pageSize: 50 });
        return {
            queryItems: function (request) {
                var context = Object.assign({}, contextFactory(), request || {}), resolvedCapabilities;
                return api.resolveCapabilities(context).then(function (capabilities) {
                    if (!capabilities || capabilities.ContextAllowed !== true) { throw new Error("SII_CONTEXT_NOT_ALLOWED"); }
                    resolvedCapabilities = capabilities;
                    return api.queryItems(context);
                }).then(function (response) {
                    var mapped = mapper.mapResponse(response), snapshot = list.replace(mapped.items);
                    return { Items: snapshot.items, DocumentTypes: (resolvedCapabilities && resolvedCapabilities.DocumentTypes) || [], ContinuationToken: mapped.continuationToken, Radicado: text(response && response.Radicado) };
                });
            },
            renderItems: function (container, data) {
                var items = data && Array.isArray(data.Items) ? data.Items : [], doc = container.ownerDocument || document;
                var toolbar = doc.createElement("div"), count = doc.createElement("span"), bulk = doc.createElement("button"), scroll = doc.createElement("div"), table = doc.createElement("table"), caption = doc.createElement("caption"), head = doc.createElement("thead"), headerRow = doc.createElement("tr"), body = doc.createElement("tbody");
                toolbar.className = "importar-servicio-web-sii__toolbar";
                count.className = "importar-servicio-web-sii__count";
                count.textContent = items.length + (items.length === 1 ? " documento encontrado" : " documentos encontrados");
                bulk.type = "button"; bulk.className = "importar-servicio-web-sii__bulk"; bulk.setAttribute("data-import-prepare-selected", "true"); bulk.textContent = "Preparar seleccionados"; bulk.disabled = true;
                toolbar.appendChild(count); toolbar.appendChild(bulk); container.appendChild(toolbar);
                scroll.className = "importar-servicio-web-sii__table-scroll"; scroll.setAttribute("role", "region"); scroll.setAttribute("aria-label", "Tabla desplazable de documentos disponibles para importar"); scroll.setAttribute("tabindex", "0");
                table.className = "importar-servicio-web-sii__table"; caption.className = "importar-servicio-web-sii__caption"; caption.textContent = "Documentos disponibles desde SII";
                var selectAllHeader = doc.createElement("th"), selectAll = doc.createElement("input"), selectAllLabel = doc.createElement("span");
                selectAllHeader.scope = "col"; selectAllHeader.className = "importar-servicio-web-sii__select-heading";
                selectAll.type = "checkbox"; selectAll.disabled = !items.some(function (item) { return item.importable; }); selectAll.setAttribute("data-import-select-all", "true"); selectAll.setAttribute("aria-label", "Seleccionar o deseleccionar todas las inscripciones disponibles");
                selectAllLabel.textContent = "Seleccionar"; selectAllHeader.appendChild(selectAll); selectAllHeader.appendChild(selectAllLabel); headerRow.appendChild(selectAllHeader);
                ["Libro", "Inscripción", "Fecha", "Naturaleza / acto", "Noticia", "Referencia", "Estado"].forEach(function (label) { appendHeader(headerRow, label); });
                appendHeader(headerRow, "Acciones", "importar-servicio-web-sii__actions-heading"); head.appendChild(headerRow);
                items.forEach(function (item) {
                    var row = doc.createElement("tr"), selectCell = doc.createElement("td"), select = doc.createElement("input"), preview = doc.createElement("button"), prepare = doc.createElement("button"), actions = doc.createElement("div"), actionCell = doc.createElement("td");
                    row.setAttribute("data-external-key", text(item.externalKey)); row.setAttribute("data-internal-document-id", text(item.internalDocumentId));
                    select.type = "checkbox"; select.disabled = !item.importable; select.setAttribute("data-import-select", "true"); select.setAttribute("data-external-key", text(item.externalKey)); select.setAttribute("aria-label", "Seleccionar " + text(item.displayName));
                    selectCell.className = "importar-servicio-web-sii__select-cell"; selectCell.appendChild(select); row.appendChild(selectCell);
                    [item.book, item.inscription, item.date, item.act, item.news, item.reference, item.importStatus].forEach(function (column, index) { appendCell(row, column, index === 4 ? "importar-servicio-web-sii__news" : (index === 6 ? "importar-servicio-web-sii__state" : "")); });
                    preview.type = "button"; preview.className = "importar-servicio-web-sii__preview"; preview.setAttribute("data-import-preview", "true"); preview.setAttribute("data-external-key", text(item.externalKey)); preview.setAttribute("data-internal-document-id", text(item.internalDocumentId)); preview.textContent = "Vista previa";
                    prepare.type = "button"; prepare.className = "importar-servicio-web-sii__prepare"; prepare.disabled = !item.importable; prepare.setAttribute("data-import-prepare", "true"); prepare.setAttribute("data-external-key", text(item.externalKey)); prepare.textContent = "Preparar"; if (prepare.disabled) { prepare.title = "No disponible: " + (text(item.importStatus) || "estado sin determinar"); }
                    actions.className = "importar-servicio-web-sii__row-actions"; actions.appendChild(preview); actions.appendChild(prepare); actionCell.className = "importar-servicio-web-sii__actions-cell"; actionCell.appendChild(actions); row.appendChild(actionCell); body.appendChild(row);
                });
                table.appendChild(caption); table.appendChild(head); table.appendChild(body); scroll.appendChild(table); container.appendChild(scroll);
            },
            list: list
        };
    }
    var exported = { canonicalId: canonicalId, create: create };
    root.ImportarServicioWebSiiAdapter = exported;
    if (typeof module !== "undefined" && module.exports) { module.exports = exported; }
}(typeof window !== "undefined" ? window : globalThis));
