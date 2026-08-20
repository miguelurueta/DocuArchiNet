/* DOC-2: adaptador de presentación opt-in. Nunca se ejecuta fuera del contenedor emitido por servidor. */
(function (window, document) {
    "use strict";

    var rootId = "div_content_general_wf";
    var updateHandlerRegistered = false;
    var legacyModalPanelSelector = [
        ".modal_content_general",
        ".modal_content_general_",
        "#Panel_detalle_flujo",
        "#Panel_detalle_sesion",
        "#Panelmensaj",
        "#Panel_filtro",
        "#Panel_autoriza_reasignacion_tarea",
        "#Panel_indice_enlace",
        "#Panel_envia_actividad_flujo_trabjo",
        "#Panel_detalle_actividad_flujo_user",
        "#Panel_detalle_actividad_flujo",
        "#Panel_lista_actividades_ruta_flujo",
        "#Panel_autoriza_reasignacion_tarea_recuperada",
        "#Panel_autoriza_reasignacion_tarea_recuperada_enlazada",
        "#Panel_pro_gres_bar",
        "#Panel_mensaje_personalizado"
    ].join(",");

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

    function addClassesToActionHosts(root, selector, classes) {
        var elements;
        var host;
        var index;

        if (!rootIsModern(root)) {
            return;
        }

        elements = root.querySelectorAll(selector);
        for (index = 0; index < elements.length; index += 1) {
            host = elements[index];
            while (host && host !== root) {
                if (host.classList && host.classList.contains("ctw-action-host")) {
                    host.classList.add.apply(host.classList, classes);
                    break;
                }
                host = host.parentElement;
            }
        }
    }

    function toggleClass(element, className, enabled) {
        if (!element || !element.classList) {
            return;
        }

        if (enabled) {
            element.classList.add(className);
        } else {
            element.classList.remove(className);
        }
    }

    function isRendered(element) {
        return !!(element && (element.offsetWidth || element.offsetHeight || element.getClientRects().length));
    }

    function markFirstTerminalActionHost(root) {
        var hosts;
        var index;

        hosts = root.querySelectorAll("#nav_menu .ctw-action-slot--terminal");
        for (index = 0; index < hosts.length; index += 1) {
            hosts[index].classList.remove("ctw-action-slot--terminal-start");
        }
        if (hosts.length > 0) {
            hosts[0].classList.add("ctw-action-slot--terminal-start");
        }
    }

    function applyActionLayer(root) {
        if (!root.classList.contains("ctw-layer-actions")) {
            return;
        }

        addClasses(root, "#menucab", ["ctw-action-bar", "ctw-action-bar--navigation", "ctw-commandbar"]);
        addClasses(root, "#nav_menu", ["ctw-action-bar", "ctw-action-bar--operations", "ctw-commandbar", "ctw-commandbar--secondary"]);
        addClasses(root, "#menucab .nav-link, #nav_menu .nav-link", ["ctw-btn"]);
        addClasses(root, "#menucab li.dropdown, #nav_menu li.dropdown", ["ctw-menu"]);
        addClasses(root, "#menucab .dropdown-toggle, #nav_menu .dropdown-toggle", ["ctw-menu__trigger"]);
        addClasses(root, "#menucab .dropdown-menu, #nav_menu .dropdown-menu", ["ctw-menu__panel"]);
        addClasses(root, "#menucab .dropdown-item, #nav_menu .dropdown-item, #nav_menu .dropdown-menu .nav-link", ["ctw-menu__item"]);
        addClasses(root, "#menucab .dropdown-divider, #nav_menu .dropdown-divider", ["ctw-menu__divider"]);
        addClasses(root, "#menucab .ctw-menu, #nav_menu .ctw-menu", ["ctw-command-menu"]);
        addClasses(root, "#menucab [onclick*='prevent_elimina_adjunto']", ["ctw-menu__item--danger"]);

        addClasses(root, "#menucab #navbarNavDropdown > .navbar-nav > .navbar-nav, #nav_menu #navbarNavDropdown_ > .col-md-9 > .navbar-nav, #nav_menu #navbarNavDropdown_ > .col-md-3 > .navbar-nav", ["ctw-action-host"]);
        addClassesToActionHosts(root, "#menucab #pendiente_db", ["ctw-action-slot", "ctw-action-slot--terminal"]);
        addClassesToActionHosts(root, "#nav_menu #nota_db", ["ctw-action-slot", "ctw-action-slot--operational", "ctw-action-slot--notes"]);
        addClassesToActionHosts(root, "#nav_menu .ctw-authorize-control", ["ctw-action-slot", "ctw-action-slot--operational"]);
        addClassesToActionHosts(root, "#nav_menu [title='Detalle del radicado'], #nav_menu .fa-user-headset", ["ctw-action-slot", "ctw-action-slot--support"]);
        addClassesToActionHosts(root, "#nav_menu [title='Devuelve la tarea'], #nav_menu [onclick*='ImageButtonEnviarUsuario'], #nav_menu [onclick*='ImageButtonEnviaActividad'], #nav_menu #workflow-group-send-trigger, #nav_menu #workflow-transition-trigger, #nav_menu [onclick*='ImageButtonautoterminar']", ["ctw-action-slot", "ctw-action-slot--terminal"]);
        addClassesToActionHosts(root, "#nav_menu [title='Devuelve la tarea']", ["ctw-action-slot--return"]);
        addClassesToActionHosts(root, "#nav_menu [onclick*='ImageButtonEnviarUsuario'], #nav_menu [onclick*='ImageButtonEnviaActividad'], #nav_menu #workflow-group-send-trigger", ["ctw-action-slot--handoff"]);
        addClassesToActionHosts(root, "#nav_menu [onclick*='ImageButtonEnviarUsuario']", ["ctw-action-slot--handoff-user"]);
        addClassesToActionHosts(root, "#nav_menu [onclick*='ImageButtonEnviaActividad'], #nav_menu #workflow-group-send-trigger", ["ctw-action-slot--handoff-group"]);
        addClassesToActionHosts(root, "#nav_menu #workflow-transition-trigger, #nav_menu [onclick*='ImageButtonautoterminar']", ["ctw-action-slot--send"]);
        addClassesToActionHosts(root, "#nav_menu #pendiente_selec_tarea", ["ctw-action-slot", "ctw-action-slot--terminal"]);
        markFirstTerminalActionHost(root);
    }

    function applyLayoutLayer(root) {
        if (!root.classList.contains("ctw-layer-layout")) {
            return;
        }

        addClasses(root, "#content_selecion_tarea", ["ctw-workspace"]);
        addClasses(root, "#content_seleccion_documentos", ["ctw-documents-pane", "ctw-panel"]);
        addClasses(root, "#contenido_imagen", ["ctw-viewer-pane", "ctw-panel"]);
        addClasses(root, "#contenido_indice", ["ctw-index-pane", "ctw-panel"]);
        addClasses(root, "#title_indice", ["ctw-pane-head"]);
        addClasses(root, "#Panel_tolbar_pdf, #tollimage", ["ctw-document-bar"]);
    }

    function applyDocumentLayer(root) {
        var documentPane;
        var documentCount;
        var documentCountValue;
        var selectedDocumentId;
        var rows;
        var viewerPane;
        var viewerContent;
        var index;

        if (!root.classList.contains("ctw-layer-documents")) {
            return;
        }

        addClasses(root, "#content_seleccion_documentos", ["ctw-panel"]);
        addClasses(root, "#div_label", ["ctw-document-bar", "ctw-pane-head"]);
        addClasses(root, "#Panel_scroll", ["ctw-documents-list"]);
        addClasses(root, "#GridView_list_documento_relacion_wf .GridviewRow", ["ctw-document-row"]);
        addClasses(root, "#div_label .nav-link, #div_label .dropdown-toggle", ["ctw-icon-btn"]);
        addClasses(root, "#div_label #btnLoadFile, #div_label #btnloadservice, #div_label a[title='Actualiza indice batch']", ["ctw-document-quick-action"]);
        addClasses(root, "#div_label .dropright", ["ctw-menu", "ctw-document-more-actions"]);
        addClasses(root, "#div_label .dropright .dropdown-menu", ["ctw-menu__panel", "ctw-document-menu__panel"]);
        addClasses(root, "#div_label .dropright .dropdown-item", ["ctw-menu__item"]);
        addClasses(root, "#div_label .dropright .dropdown-divider", ["ctw-menu__divider"]);
        addClasses(root, "#div_label .dropright [onclick*='C-DW-DEL-IMAGE']", ["ctw-menu__item--danger"]);
        addClasses(root, "#Panel_tolbar_pdf .ctw-viewer-document-actions .nav-link", ["ctw-viewer-action"]);
        addClasses(root, "#Panel_tolbar_pdf #ctw-document-action-service", ["ctw-viewer-action--service"]);

        documentPane = root.querySelector("#content_seleccion_documentos");
        documentCount = root.querySelector("#Hidden_numero_doc_rel_wf");
        documentCountValue = documentCount ? parseInt(documentCount.value, 10) : -1;
        selectedDocumentId = root.querySelector("#hiden_seleccion_documento_id_wf");
        selectedDocumentId = selectedDocumentId ? selectedDocumentId.value : "";
        rows = root.querySelectorAll("#GridView_list_documento_relacion_wf .GridviewRow");
        toggleClass(documentPane, "ctw-documents-pane--empty", documentCountValue === 0 && rows.length === 0);

        for (index = 0; index < rows.length; index += 1) {
            if (selectedDocumentId && selectedDocumentId !== "-1" && rows[index].getAttribute("id_wf") === selectedDocumentId) {
                rows[index].classList.add("ctw-document-row--selected");
            } else {
                rows[index].classList.remove("ctw-document-row--selected");
            }
        }

        viewerPane = root.querySelector("#contenido_imagen");
        viewerContent = root.querySelector("#ifrm_visor_, #content, #UpdatePanelVisor");
        toggleClass(viewerPane, "ctw-viewer-pane--empty", !isRendered(viewerContent));

    }

    /* Conserva AjaxControlToolkit y sus postbacks: solo normaliza las piezas
       visuales internas a las clases Bootstrap ya disponibles en la página. */
    function applyModalLayer(root) {
        if (!root.classList.contains("ctw-layer-layout")) {
            return;
        }

        addClasses(root, legacyModalPanelSelector, ["ctw-legacy-modal"]);
        addClasses(root, ".ctw-legacy-modal .modal-content_, .ctw-legacy-modal .modal-content", ["ctw-modal-content"]);
        addClasses(root, ".ctw-legacy-modal .modal-header, .ctw-legacy-modal .modal-header_, .ctw-legacy-modal .modal_title_superior, .ctw-legacy-modal .modal_title_superior_", ["modal-header", "ctw-modal-header"]);
        addClasses(root, ".ctw-legacy-modal .modal-body, .ctw-legacy-modal .modal_content_back, .ctw-legacy-modal .modal_content_back_", ["modal-body", "ctw-modal-body"]);
        addClasses(root, ".ctw-legacy-modal .modal-footer", ["ctw-modal-footer"]);
        addClasses(root, ".ctw-legacy-modal .ctw-modal-header h1, .ctw-legacy-modal .ctw-modal-header h2, .ctw-legacy-modal .ctw-modal-header h3, .ctw-legacy-modal .ctw-modal-header h4, .ctw-legacy-modal .ctw-modal-header h5, .ctw-legacy-modal .ctw-modal-header h6, .ctw-legacy-modal .ctw-modal-header .modal-title", ["modal-title", "ctw-modal-title"]);
    }

    function applyPresentation() {
        var root = document.getElementById(rootId);

        if (!rootIsModern(root)) {
            return;
        }

        applyLayoutLayer(root);
        applyActionLayer(root);
        applyDocumentLayer(root);
        applyModalLayer(root);
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
