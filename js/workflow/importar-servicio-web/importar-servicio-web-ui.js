(function (window, document) {
    "use strict";

    var activeControl = null;

    function previewMessage(state) {
        var messages = { "cerrado": "", "preparando": "Preparando vista segura…", "disponible": "Recurso externo disponible.", "formato-no-visualizable": "Este formato no se puede mostrar aquí. Puede descargarlo de forma temporal.", "recurso-vencido": "El recurso temporal venció. Solicite uno nuevo.", "proveedor-no-disponible": "El proveedor no está disponible.", "no-autorizado": "No tiene autorización para ver este recurso.", "bloqueado": "La vista segura aún no está disponible." };
        return messages[state] || "No fue posible preparar la vista segura.";
    }

    function createImportedViewerAdapter(control) {
        function resolve(internalDocumentId) {
            var id = text(internalDocumentId), grid = document.getElementById("GridView_list_documento_relacion_wf"), rows, index, row, selection, parts, trustedTaskId;
            if (!/^\d+$/.test(id) || Number(id) <= 0 || !grid) { return null; }
            trustedTaskId = String(taskId(control));
            rows = grid.getElementsByTagName("tr");
            for (index = 0; index < rows.length; index += 1) {
                row = rows[index];
                if (text(row.getAttribute("id_wf")) !== id) { continue; }
                selection = text(row.getAttribute("idd_wf"));
                parts = selection.split("|");
                if (parts.length < 6 || text(parts[1]) !== id || text(parts[5]) !== trustedTaskId) { return null; }
                return { id: id, selection: selection, row: row };
            }
            return null;
        }
        function open(internalDocumentId) {
            var resolved = resolve(internalDocumentId), selectionInput, idInput, submit;
            if (!resolved) { return false; }
            selectionInput = document.getElementById("hiden_seleccion_documento_wf");
            idInput = document.getElementById("hiden_seleccion_documento_id_wf");
            submit = document.getElementById("Button_selecion_treview_documento");
            if (!selectionInput || !idInput || !submit || typeof submit.click !== "function") { return false; }
            selectionInput.value = resolved.selection;
            idInput.value = resolved.id;
            submit.click();
            return true;
        }
        return { canOpen: function (id) { return !!resolve(id); }, open: open };
    }

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

    function radicado(control) {
        var input = document.getElementById(control.trigger.getAttribute("data-import-radicado-input-id"));
        return text(input && input.value);
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
        control.resultData = data || { Items: [], DocumentTypes: [] };
        control.preparationCatalog = Array.isArray(control.resultData.DocumentTypes) ? control.resultData.DocumentTypes.slice(0) : [];
        clear(control.results);
        if (control.adapter && typeof control.adapter.renderItems === "function") { control.adapter.renderItems(control.results, data); return; }
        items.forEach(function (item) {
            var entry = document.createElement("li");
            entry.className = "importar-servicio-web__item";
            entry.textContent = text(item.DisplayName, "Documento disponible");
            control.results.appendChild(entry);
        });
    }

    function itemByKey(control, key) {
        return (control.resultData && control.resultData.Items || []).filter(function (item) { return text(item.externalKey || item.ExternalKey) === key; })[0];
    }

    function preparationRow(item) {
        return { externalKey: text(item.externalKey || item.ExternalKey), clientItemId: text(item.externalKey || item.ExternalKey), fileName: text(item.displayName || item.DisplayName), contentType: text(item.contentType || item.ContentType) };
    }

    function renderPreparationItems(control) {
        clear(control.preparationItems);
        control.preparationModel.items.forEach(function (item) {
            var wrapper = document.createElement("div"), label = document.createElement("label"), select = document.createElement("select"), empty = document.createElement("option");
            wrapper.className = "importar-servicio-web__preparation-item"; label.textContent = item.FileName || item.ExternalKey; empty.value = ""; empty.textContent = "Seleccione tipología"; select.appendChild(empty);
            control.preparationCatalog.forEach(function (entry) { var option = document.createElement("option"), id = entry.Id || entry.DocumentTypeId, name = entry.Name || entry.Nombre || entry.DocumentTypeName; option.value = String(id); option.textContent = text(name); select.appendChild(option); });
            select.setAttribute("data-import-document-type", item.ExternalKey); select.setAttribute("aria-label", "Tipología de " + (item.FileName || item.ExternalKey)); wrapper.appendChild(label); wrapper.appendChild(select); control.preparationItems.appendChild(wrapper);
        });
    }

    function renderPreparationPlan(control, response) {
        clear(control.preparationPlan);
        (response.EffectPlans || []).forEach(function (plan) { var block = document.createElement("div"), effects = (plan.Effects || []).map(function (effect) { return text(effect.Code) + ": " + text(effect.Status); }); block.className = "importar-servicio-web__preparation-plan"; block.textContent = "Tarea " + text(plan.TargetTaskId) + " · " + text(plan.DocumentTypeName) + " · Efectos previstos: " + effects.join(", "); control.preparationPlan.appendChild(block); });
    }

    function closePreparation(control) {
        control.preparationPanel.hidden = true; control.listPanel.hidden = false; control.preparationStatus.textContent = ""; control.preparationConfirm.disabled = true;
        if (control.preparationContext) { control.body.scrollTop = control.preparationContext.scrollTop; control.body.scrollLeft = control.preparationContext.scrollLeft; if (control.preparationContext.focus && typeof control.preparationContext.focus.focus === "function") { control.preparationContext.focus.focus(); } }
    }

    function prepareCurrent(control) {
        control.preparationStatus.textContent = "Validando requisitos y plan previsto…"; control.preparationConfirm.disabled = true;
        control.intentClient.preflight(requestContext(control), control.preparationModel.items).then(function (response) { control.preparationResponse = response; renderPreparationPlan(control, response); control.preparationStatus.textContent = "Plan previsto confirmado. No se ha ejecutado ningún efecto."; control.preparationConfirm.disabled = false; }).catch(function () { control.preparationStatus.textContent = "No fue posible confirmar el plan de importación."; control.preparationConfirm.disabled = true; });
    }

    function openPreparation(control, keys, trigger) {
        var rows = keys.map(function (key) { return itemByKey(control, key); }).filter(Boolean);
        if (!rows.length || !control.preparationCatalog.length) { control.status.textContent = "No hay catálogo autorizado para preparar la importación."; return; }
        control.preparationContext = captureListContext(control, trigger); control.preparationModel = { items: control.preparation.multiple(rows.map(preparationRow), taskId(control)) }; control.preparationResponse = null; control.listPanel.hidden = true; control.previewPanel.hidden = true; control.preparationPanel.hidden = false; control.preparationPanel.setAttribute("data-preparation-state", "edicion"); renderPreparationItems(control); clear(control.preparationPlan); control.preparationStatus.textContent = "Seleccione una tipología autorizada para cada elemento."; control.preparationTitle.focus();
    }

    function renderPreview(control, snapshot) {
        if (!control.previewPanel) { return; }
        control.previewPanel.setAttribute("data-preview-state", snapshot.state);
        control.previewStatus.textContent = previewMessage(snapshot.state);
        control.previewFrame.hidden = true;
        control.previewFrame.removeAttribute("src");
        control.previewDownload.hidden = true;
        control.previewRenew.hidden = snapshot.state !== "recurso-vencido" && snapshot.state !== "proveedor-no-disponible";
        control.previewImported.hidden = !snapshot.internalDocumentId || !control.importedViewer.canOpen(snapshot.internalDocumentId);
        if (snapshot.state === "disponible" && snapshot.resourcePath) {
            control.previewFrame.title = "Recurso externo temporal";
            control.previewFrame.src = snapshot.resourcePath;
            control.previewFrame.hidden = false;
        } else if (snapshot.state === "formato-no-visualizable" && snapshot.resourcePath) {
            control.previewDownload.href = snapshot.resourcePath;
            control.previewDownload.hidden = false;
        }
    }

    function captureListContext(control, trigger) {
        return { focus: trigger || document.activeElement, scrollTop: control.body.scrollTop, scrollLeft: control.body.scrollLeft };
    }

    function restoreListContext(control) {
        var saved = control.previewContext;
        control.dialog.classList.remove("importar-servicio-web__dialog--preview");
        control.previewPanel.hidden = true;
        control.listPanel.hidden = false;
        if (!saved) { return; }
        control.body.scrollTop = saved.scrollTop;
        control.body.scrollLeft = saved.scrollLeft;
        if (saved.focus && typeof saved.focus.focus === "function") { saved.focus.focus(); }
    }

    function openPreview(control, button) {
        var item = { externalKey: text(button.getAttribute("data-external-key")), internalDocumentId: text(button.getAttribute("data-internal-document-id")) };
        if (!item.externalKey) { return; }
        control.previewItem = item;
        control.previewContext = captureListContext(control, button);
        control.listPanel.hidden = true;
        control.previewPanel.hidden = false;
        control.dialog.classList.add("importar-servicio-web__dialog--preview");
        control.previewTitle.focus();
        control.preview.open(item, requestContext(control));
    }

    function viewImported(control) {
        var id = control.preview && control.preview.snapshot().internalDocumentId;
        if (!id) { return; }
        if (control.importedViewer.open(id)) { close(control); }
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

    function restoreTriggerFocus(control) {
        var fallback;
        if (control.trigger && (!control.trigger.getClientRects || control.trigger.getClientRects().length > 0)) {
            control.trigger.focus();
            return;
        }
        fallback = control.trigger && control.trigger.closest ? control.trigger.closest(".dropright") : null;
        fallback = fallback && fallback.querySelector ? fallback.querySelector(".dropdown-toggle") : null;
        if (fallback && typeof fallback.focus === "function") { fallback.focus(); }
    }

    function close(control) {
        if (control.preparationPanel && !control.preparationPanel.hidden) { closePreparation(control); }
        if (control.preview) { control.preview.close(); restoreListContext(control); }
        control.core.close();
        control.modal.hidden = true;
        control.modal.setAttribute("aria-hidden", "true");
        document.body.classList.remove("importar-servicio-web-open");
        restoreTriggerFocus(control);
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
        var control, registry, api, siiFactory;

        if (!trigger || trigger.getAttribute("data-import-modern-active") !== "true" || trigger.getAttribute("data-import-modern-bound") === "true") { return null; }
        if (!modal || !apiFactory || !registryFactory || !coreFactory) { return null; }
        control = { trigger: trigger, modal: modal, dialog: document.getElementById("importar-servicio-web-dialog"), closeButton: document.getElementById("importar-servicio-web-close"), body: document.getElementById("importar-servicio-web-body"), listPanel: document.getElementById("importar-servicio-web-list"), status: document.getElementById("importar-servicio-web-status"), results: document.getElementById("importar-servicio-web-results"), previewPanel: document.getElementById("importar-servicio-web-preview"), previewTitle: document.getElementById("importar-servicio-web-preview-title"), previewStatus: document.getElementById("importar-servicio-web-preview-status"), previewFrame: document.getElementById("importar-servicio-web-preview-frame"), previewBack: document.getElementById("importar-servicio-web-preview-back"), previewRenew: document.getElementById("importar-servicio-web-preview-renew"), previewDownload: document.getElementById("importar-servicio-web-preview-download"), previewImported: document.getElementById("importar-servicio-web-preview-imported"), preparationPanel: document.getElementById("importar-servicio-web-preparation"), preparationTitle: document.getElementById("importar-servicio-web-preparation-title"), preparationStatus: document.getElementById("importar-servicio-web-preparation-status"), preparationItems: document.getElementById("importar-servicio-web-preparation-items"), preparationPlan: document.getElementById("importar-servicio-web-preparation-plan"), preparationClose: document.getElementById("importar-servicio-web-preparation-close"), preparationCancel: document.getElementById("importar-servicio-web-preparation-cancel"), preparationConfirm: document.getElementById("importar-servicio-web-preparation-confirm"), providerId: text(trigger.getAttribute("data-import-provider-id")), viewImported: options.viewImported };
        if (!control.dialog || !control.closeButton || !control.body || !control.listPanel || !control.status || !control.results || !control.previewPanel || !control.previewTitle || !control.previewStatus || !control.previewFrame || !control.previewBack || !control.previewRenew || !control.previewDownload || !control.previewImported) { return null; }
        api = apiFactory.create(options.apiOptions || {});
        registry = registryFactory.create({ knownNotMigrated: options.knownNotMigrated || [] });
        siiFactory = options.sii || window.ImportarServicioWebSiiAdapter;
        if (siiFactory && registryFactory.normalizeProviderId(control.providerId) === siiFactory.canonicalId) { control.adapter = siiFactory.create({ api: api, mapper: options.siiMapper, list: options.siiList, contextFactory: function () { return requestContext(control); } }); registry.register(siiFactory.canonicalId, control.adapter); }
        else if (control.providerId) { control.adapter = createBackendAdapter(api, control); registry.register(control.providerId, control.adapter); }
        control.core = coreFactory.create({ registry: registry });
        if (!options.preview && (!window.ImportarServicioWebPreview || !window.ImportarServicioWebPreviewState)) { return null; }
        control.importedViewer = options.importedViewer || createImportedViewerAdapter(control);
        control.preview = (options.preview || window.ImportarServicioWebPreview).create({ api: api, state: options.previewState || window.ImportarServicioWebPreviewState });
        control.preparation = options.preparation || window.ImportarServicioWebPreparation;
        if (!control.preparation || !window.ImportarServicioWebIntentClient || !control.preparationPanel || !control.preparationTitle || !control.preparationStatus || !control.preparationItems || !control.preparationPlan || !control.preparationClose || !control.preparationCancel || !control.preparationConfirm) { return null; }
        control.intentClient = (options.intentClient || window.ImportarServicioWebIntentClient).create({ api: api });
        control.preview.subscribe(function (snapshot) { renderPreview(control, snapshot); });
        control.core.subscribe(function (snapshot) { render(control, snapshot); });
        trigger.setAttribute("data-import-modern-bound", "true");
        trigger.onclick = function (event) { return open(control, event || window.event); };
        if (legacy) { legacy.hidden = true; legacy.setAttribute("aria-hidden", "true"); }
        control.closeButton.addEventListener("click", function () { close(control); });
        control.modal.addEventListener("click", function (event) { if (event.target && event.target.getAttribute("data-import-close") === "true") { close(control); } });
        control.dialog.addEventListener("keydown", function (event) { onKeydown(control, event); });
        control.results.addEventListener("click", function (event) { var target = event.target, keys; if (!target) { return; } if (target.getAttribute("data-import-preview") === "true") { event.preventDefault(); openPreview(control, target); } else if (target.getAttribute("data-import-prepare") === "true") { event.preventDefault(); openPreparation(control, [text(target.getAttribute("data-external-key"))], target); } else if (target.getAttribute("data-import-prepare-selected") === "true") { keys = Array.prototype.map.call(control.results.querySelectorAll('[data-import-select="true"]:checked'), function (input) { return text(input.getAttribute("data-external-key")); }); openPreparation(control, keys, target); } });
        control.results.addEventListener("change", function (event) { var bulk; if (event.target && event.target.getAttribute("data-import-select") === "true") { bulk = control.results.querySelector('[data-import-prepare-selected="true"]'); if (bulk) { bulk.disabled = !control.results.querySelector('[data-import-select="true"]:checked'); } } });
        control.preparationItems.addEventListener("change", function (event) { var target = event.target, entry; if (!target || !target.getAttribute("data-import-document-type")) { return; } entry = control.preparationCatalog.filter(function (candidate) { return Number(candidate.Id || candidate.DocumentTypeId) === Number(target.value); })[0]; control.preparationModel.items = control.preparation.assignDocumentType(control.preparationModel.items, target.getAttribute("data-import-document-type"), entry, control.preparationCatalog); control.preparationConfirm.disabled = true; clear(control.preparationPlan); if (window.ImportarServicioWebRequirements.complete(control.preparationModel.items)) { prepareCurrent(control); } });
        control.preparationClose.addEventListener("click", function () { closePreparation(control); });
        control.preparationCancel.addEventListener("click", function () { closePreparation(control); });
        control.preparationConfirm.addEventListener("click", function () { control.preparationConfirm.disabled = true; control.preparationStatus.textContent = "Creando intención…"; control.intentClient.confirm({ IdempotencyKey: "ui-" + String(new Date().getTime()), Radicado: radicado(control) }).then(function (response) { control.preparationStatus.textContent = "Intención creada: " + text(response.Status, "Creada") + ". Aún no se ha ejecutado."; }).catch(function (error) { control.preparationStatus.textContent = error && error.message === "PREFLIGHT_STALE" ? "El plan cambió; prepare nuevamente." : "No fue posible crear la intención."; }); });
        control.previewBack.addEventListener("click", function () { control.preview.close(); restoreListContext(control); });
        control.previewRenew.addEventListener("click", function () { control.preview.renew(control.previewItem, requestContext(control)); });
        control.previewImported.addEventListener("click", function () { viewImported(control); });
        activeControl = control;
        return control;
    }

    var ui = { initialize: initialize, open: open, close: close, onKeydown: onKeydown, createBackendAdapter: createBackendAdapter, createImportedViewerAdapter: createImportedViewerAdapter, openPreview: openPreview, openPreparation: openPreparation, closePreparation: closePreparation, restoreListContext: restoreListContext, restoreTriggerFocus: restoreTriggerFocus, getActiveControl: function () { return activeControl; } };
    window.ImportarServicioWebUi = ui;
    if (typeof module !== "undefined" && module.exports) { module.exports = ui; }
    if (window.Sys && window.Sys.Application && typeof window.Sys.Application.add_load === "function") { window.Sys.Application.add_load(initialize); }
    if (document && document.readyState === "loading") { document.addEventListener("DOMContentLoaded", initialize); }
    else if (document) { initialize(); }
}(typeof window !== "undefined" ? window : globalThis, typeof document !== "undefined" ? document : null));
