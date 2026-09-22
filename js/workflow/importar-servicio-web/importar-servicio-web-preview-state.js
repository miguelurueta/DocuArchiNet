(function (root) {
    "use strict";

    var states = {
        idle: "cerrado",
        preparing: "preparando",
        available: "disponible",
        unsupported: "formato-no-visualizable",
        expired: "recurso-vencido",
        unavailable: "proveedor-no-disponible",
        unauthorized: "no-autorizado",
        blocked: "bloqueado"
    };
    var transitions = {};
    transitions[states.idle] = [states.preparing, states.blocked];
    transitions[states.preparing] = [states.available, states.unsupported, states.expired, states.unavailable, states.unauthorized, states.blocked, states.idle];
    transitions[states.available] = [states.preparing, states.expired, states.idle];
    transitions[states.unsupported] = [states.preparing, states.expired, states.idle];
    transitions[states.expired] = [states.preparing, states.idle];
    transitions[states.unavailable] = [states.preparing, states.idle];
    transitions[states.unauthorized] = [states.preparing, states.idle];
    transitions[states.blocked] = [states.preparing, states.idle];

    function initial() {
        return { state: states.idle, descriptorId: "", contentType: "", expiresAtUtc: "", externalKey: "", internalDocumentId: "", message: "" };
    }

    function move(current, next, data) {
        current = current || initial();
        if ((transitions[current.state] || []).indexOf(next) < 0) { throw new Error("PREVIEW_STATE_TRANSITION_INVALID"); }
        data = data || {};
        return {
            state: next,
            descriptorId: data.descriptorId || "",
            contentType: data.contentType || "",
            expiresAtUtc: data.expiresAtUtc || "",
            externalKey: data.externalKey || current.externalKey || "",
            internalDocumentId: data.internalDocumentId || current.internalDocumentId || "",
            resourcePath: data.resourcePath || "",
            visualizable: data.visualizable === true,
            message: data.message || ""
        };
    }

    var api = { states: states, initial: initial, move: move };
    root.ImportarServicioWebPreviewState = api;
    if (typeof module !== "undefined" && module.exports) { module.exports = api; }
}(typeof window !== "undefined" ? window : globalThis));
