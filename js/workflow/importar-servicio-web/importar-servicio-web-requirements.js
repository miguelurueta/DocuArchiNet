(function (root) {
    "use strict";
    var states = { editing: "edicion", preparing: "preparando", ready: "listo", creating: "creando", created: "creado", blocked: "bloqueado", error: "error" };
    function copy(value) { return JSON.parse(JSON.stringify(value)); }
    function initial(items) { return { state: states.editing, items: copy(items || []), preflight: null, intent: null, message: "" }; }
    function complete(items) { return Array.isArray(items) && items.length > 0 && items.every(function (item) { return Number(item.DocumentTypeId) > 0 && String(item.DocumentTypeName || "").trim(); }); }
    function canConfirm(model) { return !!(model && model.state === states.ready && complete(model.items) && model.preflight && model.preflight.Executable === true && !model.preflight.Error && String(model.preflight.ContextFingerprint || "").trim()); }
    function transition(model, event, value) {
        var next = copy(model || initial([]));
        if (event === "edit") { next.items = copy(value || []); next.preflight = null; next.intent = null; next.message = ""; next.state = states.editing; }
        else if (event === "prepare") { next.state = complete(next.items) ? states.preparing : states.editing; }
        else if (event === "prepared") { next.preflight = copy(value || {}); next.state = value && value.Executable === true && !value.Error ? states.ready : states.blocked; next.message = value && value.Error ? String(value.Error.MensajeVisible || "") : ""; }
        else if (event === "create") { next.state = canConfirm(next) ? states.creating : states.blocked; }
        else if (event === "created") { next.intent = copy(value || {}); next.state = states.created; }
        else if (event === "stale") { next.preflight = null; next.intent = null; next.state = states.editing; next.message = "El plan cambió; prepare nuevamente."; }
        else if (event === "block") { next.state = states.blocked; next.message = String(value || ""); }
        else if (event === "error") { next.state = states.error; next.message = String(value || ""); }
        return next;
    }
    var api = { states: states, initial: initial, complete: complete, canConfirm: canConfirm, transition: transition };
    root.ImportarServicioWebRequirements = api;
    if (typeof module !== "undefined" && module.exports) { module.exports = api; }
}(typeof window !== "undefined" ? window : globalThis));
