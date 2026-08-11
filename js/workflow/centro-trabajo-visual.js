/* DOC-2: adaptador de presentación opt-in. Nunca se ejecuta fuera del contenedor emitido por servidor. */
(function (window, document) {
    "use strict";

    var rootId = "div_content_general_wf";
    var updateHandlerRegistered = false;

    function rootIsModern(root) {
        return !!(root && root.classList && root.classList.contains("workflow-centro-trabajo-moderno"));
    }

    function addClasses(root, selector, classes) {
        var elements;
        var index;

        if (!rootIsModern(root)) {
            return;
        }

        elements = root.querySelectorAll(selector);
        for (index = 0; index < elements.length; index += 1) {
            elements[index].classList.add.apply(elements[index].classList, classes);
        }
    }

    function applyActionLayer(root) {
        if (!root.classList.contains("ctw-layer-actions")) {
            return;
        }

        addClasses(root, "#menucab", ["ctw-action-bar"]);
        addClasses(root, "#menucab li.dropdown", ["ctw-menu"]);
        addClasses(root, "#menucab .dropdown-toggle", ["ctw-menu__trigger"]);
        addClasses(root, "#menucab .dropdown-menu", ["ctw-menu__panel"]);
        addClasses(root, "#menucab .dropdown-item", ["ctw-menu__item"]);
    }

    function applyDocumentLayer(root) {
        var selectedDocumentId;
        var rows;
        var index;

        if (!root.classList.contains("ctw-layer-documents")) {
            return;
        }

        addClasses(root, "#content_seleccion_documentos", ["ctw-panel"]);
        addClasses(root, "#div_label", ["ctw-document-bar", "ctw-pane-head"]);

        selectedDocumentId = root.querySelector("#hiden_seleccion_documento_id_wf");
        selectedDocumentId = selectedDocumentId ? selectedDocumentId.value : "";
        rows = root.querySelectorAll("#GridView_list_documento_relacion_wf .GridviewRow");
        for (index = 0; index < rows.length; index += 1) {
            if (selectedDocumentId && selectedDocumentId !== "-1" && rows[index].getAttribute("id_wf") === selectedDocumentId) {
                rows[index].classList.add("ctw-document-row--selected");
            } else {
                rows[index].classList.remove("ctw-document-row--selected");
            }
        }
    }

    function applyPresentation() {
        var root = document.getElementById(rootId);

        if (!rootIsModern(root)) {
            return;
        }

        applyActionLayer(root);
        applyDocumentLayer(root);
    }

    function registerAspNetUpdateHandler() {
        var manager;

        if (updateHandlerRegistered || !window.Sys || !window.Sys.WebForms || !window.Sys.WebForms.PageRequestManager) {
            return;
        }

        manager = window.Sys.WebForms.PageRequestManager.getInstance();
        if (!manager) {
            return;
        }

        manager.add_endRequest(applyPresentation);
        updateHandlerRegistered = true;
    }

    function initialize() {
        applyPresentation();
        registerAspNetUpdateHandler();
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", initialize);
    } else {
        initialize();
    }

    window.setTimeout(registerAspNetUpdateHandler, 0);
}(window, document));
