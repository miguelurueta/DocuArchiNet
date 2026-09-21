(function (root) {
    "use strict";

    var baseUrl = "../webservice/WebServiceImportarServicioWebModern.asmx/";
    var operations = [
        "ResolveCapabilities", "QueryItems", "GetPreview", "PreflightImport",
        "CreateImportIntent", "ExecuteImportIntent", "GetImportIntent", "ReconcileImportIntent"
    ];

    function isObject(value) {
        return value !== null && typeof value === "object" && !Array.isArray(value);
    }

    function unwrapAsmx(raw) {
        var value;
        if (!isObject(raw) || !Object.prototype.hasOwnProperty.call(raw, "d")) {
            throw new Error("IMPORT_RESPONSE_INVALID");
        }
        value = raw.d;
        if (typeof value === "string") {
            try { value = JSON.parse(value); } catch (error) { throw new Error("IMPORT_RESPONSE_INVALID"); }
        }
        if (!isObject(value)) {
            throw new Error("IMPORT_RESPONSE_INVALID");
        }
        return value;
    }

    function defaultTransport(url, options) {
        if (!root || typeof root.fetch !== "function") {
            return Promise.reject(new Error("IMPORT_TRANSPORT_UNAVAILABLE"));
        }
        return root.fetch(url, options);
    }

    function create(options) {
        options = options || {};
        var transport = options.transport || defaultTransport;
        var endpoint = options.baseUrl || baseUrl;
        var client = {};

        function invoke(operation, request) {
            if (operations.indexOf(operation) < 0) {
                return Promise.reject(new Error("IMPORT_OPERATION_NOT_SUPPORTED"));
            }
            return Promise.resolve(transport(endpoint + operation, {
                method: "POST",
                credentials: "same-origin",
                headers: { "Content-Type": "application/json; charset=utf-8" },
                body: JSON.stringify({ request: request || {} })
            })).then(function (response) {
                if (!response || response.ok === false || typeof response.json !== "function") {
                    throw new Error("IMPORT_TRANSPORT_FAILED");
                }
                return response.json();
            }).then(unwrapAsmx);
        }

        operations.forEach(function (operation) {
            var method = operation.charAt(0).toLowerCase() + operation.slice(1);
            client[method] = function (request) { return invoke(operation, request); };
        });
        client.invoke = invoke;
        return client;
    }

    var api = { create: create, unwrapAsmx: unwrapAsmx, operations: operations.slice(0) };
    root.ImportarServicioWebApi = api;
    if (typeof module !== "undefined" && module.exports) { module.exports = api; }
}(typeof window !== "undefined" ? window : globalThis));
