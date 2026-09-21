(function (root) {
    "use strict";

    var transitions = {
        "cerrado": ["resolviendo-proveedor"],
        "resolviendo-proveedor": ["consultando", "error", "cerrado"],
        "consultando": ["vacio", "resultados", "error", "cerrado"],
        "vacio": ["consultando", "cerrado"],
        "resultados": ["preparando", "consultando", "cerrado"],
        "preparando": ["ejecutando", "error", "resultados", "cerrado"],
        "ejecutando": ["reconciliando", "completado", "error"],
        "reconciliando": ["completado", "error"],
        "completado": ["consultando", "cerrado"],
        "error": ["resolviendo-proveedor", "consultando", "cerrado"]
    };

    function create(options) {
        options = options || {};
        var registry = options.registry;
        var state = "cerrado";
        var listeners = [];
        var execution = null;
        var context = {};

        if (!registry || typeof registry.resolve !== "function") { throw new Error("PROVIDER_REGISTRY_REQUIRED"); }

        function snapshot(extra) {
            var value = { state: state, providerId: context.providerId || "", message: context.message || "", data: context.data || null };
            Object.keys(extra || {}).forEach(function (key) { value[key] = extra[key]; });
            return value;
        }

        function notify() { listeners.slice(0).forEach(function (listener) { listener(snapshot()); }); }

        function transition(next, update) {
            if ((transitions[state] || []).indexOf(next) < 0) { throw new Error("IMPORT_STATE_TRANSITION_INVALID:" + state + ":" + next); }
            state = next;
            context = Object.assign({}, context, update || {});
            notify();
            return snapshot();
        }

        function open(providerId) {
            var resolution;
            transition("resolviendo-proveedor", { providerId: providerId || "", message: "Resolviendo proveedor.", data: null });
            resolution = registry.resolve(providerId);
            if (!resolution.found) {
                transition("error", { message: resolution.code, data: resolution });
                return Promise.resolve(snapshot({ resolution: resolution }));
            }
            context.resolution = resolution;
            return query({});
        }

        function query(request) {
            var resolution = context.resolution;
            if (!resolution || !resolution.adapter || typeof resolution.adapter.queryItems !== "function") {
                transition("error", { message: "PROVIDER_QUERY_UNAVAILABLE" });
                return Promise.resolve(snapshot());
            }
            transition("consultando", { message: "Consultando documentos." });
            return Promise.resolve(resolution.adapter.queryItems(request || {})).then(function (response) {
                var items = response && Array.isArray(response.Items) ? response.Items : [];
                transition(items.length ? "resultados" : "vacio", { message: items.length ? "Documentos disponibles." : "No hay documentos disponibles.", data: response || { Items: [] } });
                return snapshot();
            }).catch(function (error) {
                transition("error", { message: error && error.message ? error.message : "PROVIDER_QUERY_FAILED" });
                return snapshot();
            });
        }

        function execute(request) {
            var resolution = context.resolution;
            if (execution) { return execution; }
            if (!resolution || !resolution.adapter || typeof resolution.adapter.executeImportIntent !== "function") {
                return Promise.reject(new Error("IMPORT_EXECUTION_UNAVAILABLE"));
            }
            transition("preparando", { message: "Preparando importación." });
            transition("ejecutando", { message: "Importando documentos. Espere…" });
            execution = Promise.resolve(resolution.adapter.executeImportIntent(request || {})).then(function (response) {
                transition("completado", { message: "Importación finalizada.", data: response || {} });
                return snapshot();
            }).catch(function (error) {
                transition("error", { message: error && error.message ? error.message : "IMPORT_EXECUTION_FAILED" });
                throw error;
            });
            return execution;
        }

        function close() {
            if (state !== "cerrado") { transition("cerrado", { message: "", data: null }); }
            execution = null;
            context = {};
        }

        function subscribe(listener) {
            if (typeof listener !== "function") { throw new Error("IMPORT_LISTENER_INVALID"); }
            listeners.push(listener);
            listener(snapshot());
            return function () { listeners = listeners.filter(function (item) { return item !== listener; }); };
        }

        return { open: open, query: query, execute: execute, close: close, transition: transition, subscribe: subscribe, getState: function () { return snapshot(); } };
    }

    var core = { create: create, transitions: transitions };
    root.ImportarServicioWebCore = core;
    if (typeof module !== "undefined" && module.exports) { module.exports = core; }
}(typeof window !== "undefined" ? window : globalThis));
