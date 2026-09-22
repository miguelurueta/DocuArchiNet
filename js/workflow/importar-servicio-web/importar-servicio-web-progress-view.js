(function (root) {
    "use strict";
    function clear(node) { while (node && node.firstChild) { node.removeChild(node.firstChild); } }
    function text(value) { return value === undefined || value === null ? "" : String(value); }
    function create(options) {
        options = options || {};
        var panel = options.panel, status = options.status, results = options.results, summary = options.summary;
        if (!panel || !status || !results || !summary) { throw new Error("PROGRESS_VIEW_DEPENDENCY_REQUIRED"); }
        function show() { panel.hidden = false; panel.removeAttribute("hidden"); }
        function renderPending() {
            show(); panel.setAttribute("data-progress-state", "pending"); status.setAttribute("role", "status"); status.setAttribute("aria-live", "polite");
            status.textContent = "Importando documentos. Espere a que finalice la operación."; clear(results); summary.textContent = "";
        }
        function renderResult(snapshot) {
            show(); panel.setAttribute("data-progress-state", snapshot.isTotalSuccess ? "success" : "partial"); clear(results);
            snapshot.items.forEach(function (item) { var entry = results.ownerDocument.createElement("li"), title = item.documentName || item.externalKey || "Documento"; entry.className = "importar-servicio-web__progress-item"; entry.setAttribute("data-result-state", item.visibleState); entry.textContent = title + ": " + item.visibleState + (item.message ? ". " + item.message : ""); results.appendChild(entry); });
            summary.textContent = "Guardadas: " + snapshot.summary.saved + "; omitidas: " + snapshot.summary.skipped + "; fallidas: " + snapshot.summary.failed + "; no procesadas: " + snapshot.summary.notProcessed + ".";
            status.textContent = snapshot.isTotalSuccess ? "Importación finalizada correctamente." : "Importación finalizada con resultados parciales.";
        }
        function renderFailure() { show(); panel.setAttribute("data-progress-state", "failure"); clear(results); summary.textContent = ""; status.textContent = "No fue posible confirmar el resultado de la importación."; }
        function hide() { panel.hidden = true; panel.setAttribute("hidden", "hidden"); }
        return { renderPending: renderPending, renderResult: renderResult, renderFailure: renderFailure, hide: hide };
    }
    var api = { create: create };
    root.ImportarServicioWebProgressView = api;
    if (typeof module !== "undefined" && module.exports) { module.exports = api; }
}(typeof window !== "undefined" ? window : globalThis));
