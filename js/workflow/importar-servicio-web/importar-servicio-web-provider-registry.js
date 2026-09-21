(function (root) {
    "use strict";

    var defaultCapabilities = {
        multipleSelection: false,
        preview: false,
        download: false,
        documentType: false,
        additionalRequirements: false,
        allowedActions: []
    };

    function normalizeProviderId(value) {
        return value === undefined || value === null ? "" : String(value).replace(/^\s+|\s+$/g, "").toUpperCase();
    }

    function copyCapabilities(value) {
        var source = value || {};
        return {
            multipleSelection: source.multipleSelection === true,
            preview: source.preview === true,
            download: source.download === true,
            documentType: source.documentType === true,
            additionalRequirements: source.additionalRequirements === true,
            allowedActions: Array.isArray(source.allowedActions) ? source.allowedActions.slice(0) : []
        };
    }

    function create(options) {
        options = options || {};
        var adapters = Object.create(null);
        var knownNotMigrated = Object.create(null);

        (options.knownNotMigrated || []).forEach(function (id) {
            id = normalizeProviderId(id);
            if (id) { knownNotMigrated[id] = true; }
        });

        function register(providerId, adapter) {
            var canonicalId = normalizeProviderId(providerId);
            if (!canonicalId || !adapter || typeof adapter !== "object") {
                throw new Error("PROVIDER_REGISTRATION_INVALID");
            }
            adapters[canonicalId] = {
                id: canonicalId,
                adapter: adapter,
                capabilities: copyCapabilities(adapter.capabilities || defaultCapabilities)
            };
            return api;
        }

        function resolve(providerId) {
            var canonicalId = normalizeProviderId(providerId);
            if (!canonicalId) { return { found: false, code: "PROVIDER_NOT_CONFIGURED", providerId: "" }; }
            if (adapters[canonicalId]) { return { found: true, code: "", providerId: canonicalId, adapter: adapters[canonicalId].adapter, capabilities: copyCapabilities(adapters[canonicalId].capabilities) }; }
            if (knownNotMigrated[canonicalId]) { return { found: false, code: "PROVIDER_NOT_MIGRATED", providerId: canonicalId }; }
            return { found: false, code: "PROVIDER_NOT_SUPPORTED", providerId: canonicalId };
        }

        var api = { register: register, resolve: resolve, normalizeProviderId: normalizeProviderId };
        return api;
    }

    var registry = { create: create, normalizeProviderId: normalizeProviderId, defaultCapabilities: defaultCapabilities };
    root.ImportarServicioWebProviderRegistry = registry;
    if (typeof module !== "undefined" && module.exports) { module.exports = registry; }
}(typeof window !== "undefined" ? window : globalThis));
