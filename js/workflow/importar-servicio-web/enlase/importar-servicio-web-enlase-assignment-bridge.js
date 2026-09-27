(function (root) {
    "use strict";
    function create(options) {
        options = options || {}; var doc = options.document || root.document;
        function controls() { return [doc && doc.getElementById("enlase-assign-action"), doc && doc.getElementById("Buttonaceptar")].filter(Boolean); }
        function assignmentRemainsExplicit() { return controls().length > 0; }
        return { controls: controls, assignmentRemainsExplicit: assignmentRemainsExplicit };
    }
    var api = { create: create }; root.ImportarServicioWebEnlaseAssignmentBridge = api; if (typeof module !== "undefined" && module.exports) { module.exports = api; }
}(typeof window !== "undefined" ? window : globalThis));
