(function (root) {
    "use strict";
    var capability = "ANEXOS_RADICADO_ENLASE";
    function create(options) {
        options = options || {}; var base = options.base, list = options.list || root.ImportarServicioWebEnlaseList, contextFactory = options.contextFactory || function () { return {}; };
        if (!base || typeof base.queryItems !== "function" || !list || typeof list.render !== "function") { throw new Error("ENLASE_ADAPTER_DEPENDENCY_REQUIRED"); }
        return {
            capability: capability,
            queryItems: function (request) { var context = Object.assign({}, contextFactory(), request || {}, { Capability: capability }); return base.queryItems(context); },
            renderItems: function (container, data) { return list.render(container, data); }
        };
    }
    var api = { capability: capability, create: create }; root.ImportarServicioWebEnlaseAdapter = api; if (typeof module !== "undefined" && module.exports) { module.exports = api; }
}(typeof window !== "undefined" ? window : globalThis));
