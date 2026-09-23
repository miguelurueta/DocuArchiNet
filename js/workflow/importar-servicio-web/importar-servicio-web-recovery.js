(function (root) {
    "use strict";
    var conflicts = ["TASK_CONTEXT_MISMATCH", "PERSISTED_CONTEXT_MISMATCH"];
    function text(value) { return value == null ? "" : String(value).replace(/^\s+|\s+$/g, ""); }
    function codeOf(error) { return text(error && (error.code || error.Codigo || error.message || error.Message)); }
    function create(options) {
        options = options || {}; var api = options.api, adapt = options.adapt || function (value) { return value; };
        if (!api || typeof api.getImportIntent !== "function" || typeof api.reconcileImportIntent !== "function") { throw new Error("IMPORT_RECOVERY_DEPENDENCY_REQUIRED"); }
        function request(context) { context = context || {}; if (!text(context.IntentId)) { throw new Error("IMPORT_INTENT_ID_REQUIRED"); } return { IntentId: text(context.IntentId), TaskId: Number(context.TaskId) || 0, ProviderId: text(context.ProviderId), ExternalKey: text(context.ExternalKey), VersionToken: text(context.VersionToken) }; }
        function recover(context) { return Promise.resolve(api.getImportIntent(request(context))).then(adapt); }
        function reconcile(context) { return Promise.resolve(api.reconcileImportIntent(request(context))).then(adapt); }
        function recoverConflict(error, context) { var code = codeOf(error); if (conflicts.indexOf(code) < 0) { return Promise.reject(error); } return recover(context).then(function (snapshot) { return { conflict: true, code: code, snapshot: snapshot }; }); }
        return { recover: recover, reconcile: reconcile, recoverConflict: recoverConflict, isConflict: function (error) { return conflicts.indexOf(codeOf(error)) >= 0; } };
    }
    var api = { create: create, conflicts: conflicts.slice(0), codeOf: codeOf }; root.ImportarServicioWebRecovery = api; if (typeof module !== "undefined" && module.exports) { module.exports = api; }
}(typeof window !== "undefined" ? window : globalThis));
