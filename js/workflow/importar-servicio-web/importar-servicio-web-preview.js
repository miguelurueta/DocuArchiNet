(function (root) {
    "use strict";

    var handlerPath = "../workflow/ImportarServicioWebPreview.ashx?d=";
    var visualTypes = { "application/pdf": true, "image/png": true, "image/jpeg": true };

    function text(value) { return value == null ? "" : String(value).replace(/^\s+|\s+$/g, ""); }
    function descriptorUrl(descriptorId) {
        var value = text(descriptorId);
        if (!/^[A-Za-z0-9_-]{20,200}$/.test(value)) { throw new Error("PREVIEW_DESCRIPTOR_INVALID"); }
        return handlerPath + encodeURIComponent(value);
    }
    function errorCode(error) {
        return text(error && (error.Codigo || error.Code || error.code || error.message)).toUpperCase();
    }
    function failureState(code, states) {
        if (/FORBIDDEN|UNAUTHORIZED|NOT_AUTHORIZED/.test(code)) { return states.unauthorized; }
        if (/EXPIR/.test(code)) { return states.expired; }
        if (/FEATURE_DISABLED|BLOCKED|B10/.test(code)) { return states.blocked; }
        return states.unavailable;
    }

    function create(options) {
        options = options || {};
        var api = options.api, stateApi = options.state || root.ImportarServicioWebPreviewState;
        var snapshot = stateApi.initial(), pending = null, listeners = [], generation = 0;
        if (!api || typeof api.getPreview !== "function" || !stateApi) { throw new Error("PREVIEW_DEPENDENCY_REQUIRED"); }

        function publish(next) { snapshot = next; listeners.slice(0).forEach(function (listener) { listener(snapshot); }); return snapshot; }
        function transition(next, data) { return publish(stateApi.move(snapshot, next, data)); }
        function request(item, context, force) {
            item = item || {}; context = context || {};
            if (pending && !force) { return pending; }
            generation += 1;
            var currentGeneration = generation;
            transition(stateApi.states.preparing, { externalKey: text(item.externalKey), internalDocumentId: text(item.internalDocumentId) });
            pending = Promise.resolve(api.getPreview(Object.assign({}, context, { ExternalKey: text(item.externalKey) }))).then(function (response) {
                if (currentGeneration !== generation) { return snapshot; }
                if (!response || response.Error) { throw response && response.Error ? response.Error : new Error("PREVIEW_RESPONSE_INVALID"); }
                var data = { descriptorId: text(response.DescriptorId), contentType: text(response.ContentType).toLowerCase(), expiresAtUtc: text(response.ExpiresAtUtc), externalKey: text(item.externalKey), internalDocumentId: text(item.internalDocumentId) };
                data.resourcePath = descriptorUrl(data.descriptorId);
                data.visualizable = visualTypes[data.contentType] === true;
                data.state = data.visualizable ? stateApi.states.available : stateApi.states.unsupported;
                return transition(data.state, data);
            }).catch(function (error) {
                if (currentGeneration !== generation) { return snapshot; }
                return transition(failureState(errorCode(error), stateApi.states), { externalKey: text(item.externalKey), internalDocumentId: text(item.internalDocumentId) });
            }).then(function (result) { if (currentGeneration === generation) { pending = null; } return result; });
            return pending;
        }
        function renew(item, context) { pending = null; return request(item, context, true); }
        function close() { generation += 1; pending = null; if (snapshot.state === stateApi.states.idle) { return snapshot; } return transition(stateApi.states.idle, {}); }
        function subscribe(listener) { listeners.push(listener); listener(snapshot); return function () { listeners = listeners.filter(function (candidate) { return candidate !== listener; }); }; }

        return { open: request, renew: renew, close: close, subscribe: subscribe, snapshot: function () { return snapshot; }, descriptorUrl: descriptorUrl };
    }

    var api = { create: create, descriptorUrl: descriptorUrl, failureState: failureState };
    root.ImportarServicioWebPreview = api;
    if (typeof module !== "undefined" && module.exports) { module.exports = api; }
}(typeof window !== "undefined" ? window : globalThis));
