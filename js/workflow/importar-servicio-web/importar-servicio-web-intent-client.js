(function (root) {
    "use strict";
    function text(value) { return value == null ? "" : String(value).trim(); }
    function create(options) {
        options = options || {}; var api = options.api, pending = null, prepared = null;
        if (!api || typeof api.preflightImport !== "function" || typeof api.createImportIntent !== "function") { throw new Error("INTENT_CLIENT_DEPENDENCY_REQUIRED"); }
        function preflight(context, items) {
            prepared = null;
            return Promise.resolve(api.preflightImport(Object.assign({}, context || {}, { Items: items || [] }))).then(function (response) {
                if (!response || response.Error || response.Executable !== true || !text(response.ContextFingerprint) || !Array.isArray(response.Requirements) || !Array.isArray(response.EffectPlans)) { throw new Error(response && response.Error ? text(response.Error.Codigo) : "PREFLIGHT_NOT_EXECUTABLE"); }
                prepared = { context: Object.assign({}, context || {}), items: (items || []).slice(0), response: response };
                return response;
            });
        }
        function confirm(extra) {
            if (pending) { return pending; }
            if (!prepared) { return Promise.reject(new Error("PREFLIGHT_REQUIRED")); }
            var request = Object.assign({}, prepared.context, extra || {}, { Items: prepared.items, Requirements: prepared.response.Requirements, ContextFingerprint: prepared.response.ContextFingerprint });
            pending = Promise.resolve(api.createImportIntent(request)).then(function (response) {
                if (!response || response.Error) { var code = response && response.Error ? text(response.Error.Codigo) : "INTENT_RESPONSE_INVALID"; if (code === "PREFLIGHT_STALE") { prepared = null; } throw new Error(code); }
                return response;
            }).finally(function () { pending = null; });
            return pending;
        }
        return { preflight: preflight, confirm: confirm, hasPrepared: function () { return !!prepared; } };
    }
    var api = { create: create };
    root.ImportarServicioWebIntentClient = api;
    if (typeof module !== "undefined" && module.exports) { module.exports = api; }
}(typeof window !== "undefined" ? window : globalThis));
