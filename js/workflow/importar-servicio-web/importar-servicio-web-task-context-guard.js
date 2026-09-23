(function (root) {
    "use strict";
    function text(value) { return value == null ? "" : String(value).replace(/^\s+|\s+$/g, ""); }
    function number(value) { value = Number(value); return isFinite(value) && value > 0 ? value : 0; }
    function copy(value) { value = value || {}; return Object.freeze({ intentId: text(value.IntentId || value.intentId), operationId: text(value.OperationId || value.operationId), taskId: number(value.TaskId || value.taskId), routeId: text(value.RouteId || value.routeId), providerId: text(value.ProviderId || value.providerId), externalKeys: Object.freeze((value.ExternalKeys || value.externalKeys || []).map(text).filter(Boolean)), startedAt: text(value.StartedAt || value.startedAt) || new Date().toISOString() }); }
    function create(options) {
        options = options || {}; var currentTaskId = options.currentTaskId, controls = options.controls || [], eventTarget = options.eventTarget || root, context = null, writing = false, saved = [];
        if (typeof currentTaskId !== "function") { throw new Error("TASK_CONTEXT_READER_REQUIRED"); }
        function publish(name, detail) { if (!eventTarget || typeof eventTarget.dispatchEvent !== "function") { return; } eventTarget.dispatchEvent(typeof root.CustomEvent === "function" ? new root.CustomEvent(name, { detail: detail }) : { type: name, detail: detail }); }
        function capture(value) { var next = copy(value); if (!next.taskId) { throw new Error("TASK_CONTEXT_REQUIRED"); } context = next; return context; }
        function matches() { return !!context && number(currentTaskId()) === context.taskId; }
        function assertCurrent() { if (!context || !matches()) { throw new Error("TASK_CONTEXT_MISMATCH"); } return context; }
        function lock() { if (writing) { return; } assertCurrent(); writing = true; saved = controls.map(function (control) { var state = { control: control, disabled: !!control.disabled, ariaDisabled: control.getAttribute && control.getAttribute("aria-disabled"), pointerEvents: control.style && control.style.pointerEvents }; if ("disabled" in control) { control.disabled = true; } if (control.setAttribute) { control.setAttribute("aria-disabled", "true"); control.setAttribute("data-import-context-blocked", "true"); } if (control.style) { control.style.pointerEvents = "none"; } return state; }); publish("importar-servicio-web:writing", { active: true, context: context }); }
        function unlock() { saved.forEach(function (state) { if ("disabled" in state.control) { state.control.disabled = state.disabled; } if (state.control.style) { state.control.style.pointerEvents = state.pointerEvents || ""; } if (!state.control.setAttribute) { return; } state.control.removeAttribute("data-import-context-blocked"); if (state.ariaDisabled == null) { state.control.removeAttribute("aria-disabled"); } else { state.control.setAttribute("aria-disabled", state.ariaDisabled); } }); saved = []; writing = false; publish("importar-servicio-web:writing", { active: false, context: context }); }
        function observeTaskSignal(value) { if (!context || number(value && (value.TaskId || value.taskId)) === context.taskId) { return false; } publish("importar-servicio-web:context-conflict", { code: "TASK_CONTEXT_MISMATCH", context: context }); return true; }
        return { capture: capture, matches: matches, assertCurrent: assertCurrent, lock: lock, unlock: unlock, observeTaskSignal: observeTaskSignal, isWriting: function () { return writing; }, snapshot: function () { return context; } };
    }
    var api = { create: create, normalize: copy }; root.ImportarServicioWebTaskContextGuard = api; if (typeof module !== "undefined" && module.exports) { module.exports = api; }
}(typeof window !== "undefined" ? window : globalThis));
