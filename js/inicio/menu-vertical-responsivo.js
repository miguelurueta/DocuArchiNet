/* Menú vertical responsive sin cambiar Scoop ni el backend. */
(function () {
    "use strict";
    var puntoCierre = 992;
    var puntoCierreCentroTrabajo = 1199;
    var idViewportCentroTrabajo = "workflowCentroTrabajoModernShellViewport";
    var idIframeContenido = "ContentPlacenter_ifrm_ds_";
    var rutaCentroTrabajo = /(?:^|\/)workflow\/Webworkflow\.aspx(?:[?#]|$)/i;

    function obtenerMenu() {
        return document.getElementById("scoop");
    }

    /* El Workbench se carga dentro del iframe del shell. En la banda intermedia
       el menú abierto le resta 290 px y hace que el iframe active su breakpoint
       táctil antes de tiempo. El meta solo se emite para el piloto autorizado;
       por ello el ajuste no afecta baseline ni otros módulos del mismo shell. */
    function esCentroTrabajoPilotoActivo() {
        var iframe = document.getElementById(idIframeContenido);
        var origen = iframe ? (iframe.getAttribute("src") || "") : "";

        return document.getElementById(idViewportCentroTrabajo) !== null && rutaCentroTrabajo.test(origen);
    }

    function puntoCierreActual() {
        return esCentroTrabajoPilotoActivo() ? puntoCierreCentroTrabajo : puntoCierre;
    }

    function vistaReducida() {
        return window.matchMedia("(max-width: " + puntoCierreActual() + "px)").matches;
    }

    function establecerEstado(menu, cerrado) {
        menu.classList.add("da-menu-responsivo");
        menu.classList.toggle("da-menu-cerrado", cerrado);
        menu.setAttribute("data-da-menu-estado", cerrado ? "cerrado" : "abierto");

        /* Refuerzo contra reglas heredadas de Scoop con !important. */
        var barra = menu.querySelector(".scoop-navbar");
        var cabeceraIzquierda = menu.querySelector(".scoop-header .scoop-left-header");
        var cabeceraDerecha = menu.querySelector(".scoop-header .scoop-right-header");
        var contenido = menu.querySelector(".scoop-content");
        if (cerrado) {
            if (barra) {
                barra.style.setProperty("margin-left", "calc(-1 * var(--da-menu-ancho))", "important");
                barra.style.setProperty("visibility", "hidden", "important");
                barra.style.setProperty("pointer-events", "none", "important");
            }
            if (cabeceraIzquierda) cabeceraIzquierda.style.setProperty("width", "0", "important");
            if (cabeceraDerecha) cabeceraDerecha.style.setProperty("margin-left", "0", "important");
            if (contenido) contenido.style.setProperty("margin-left", "0", "important");
        } else {
            if (barra) {
                barra.style.removeProperty("margin-left");
                barra.style.removeProperty("visibility");
                barra.style.removeProperty("pointer-events");
            }
            if (cabeceraIzquierda) cabeceraIzquierda.style.removeProperty("width");
            if (cabeceraDerecha) cabeceraDerecha.style.removeProperty("margin-left");
            if (contenido) contenido.style.removeProperty("margin-left");
        }

        var botones = menu.querySelectorAll(".sidebar_toggle a");
        for (var i = 0; i < botones.length; i++) {
            botones[i].setAttribute("aria-expanded", cerrado ? "false" : "true");
            botones[i].setAttribute("aria-controls", "nav_menu");
        }
    }

    function sincronizarVista() {
        var menu = obtenerMenu();
        if (!menu) return;
        establecerEstado(menu, vistaReducida());
    }

    function alternarMenu(evento) {
        if (evento) {
            evento.preventDefault();
            evento.stopImmediatePropagation();
        }
        var menu = obtenerMenu();
        if (!menu) return;
        establecerEstado(menu, !menu.classList.contains("da-menu-cerrado"));
    }

    function manejarClickMenu(evento) {
        var objetivo = evento.target.closest ? evento.target.closest("#scoop .sidebar_toggle a") : null;
        if (!objetivo) return;
        alternarMenu(evento);
    }

    function sincronizarIframeContenido() {
        var iframe = document.getElementById(idIframeContenido);
        if (!iframe) return;
        iframe.addEventListener("load", sincronizarVista);
    }

    function iniciar() {
        var menu = obtenerMenu();
        if (!menu) return;
        sincronizarVista();
        sincronizarIframeContenido();

        window.daAlternarMenuVertical = alternarMenu;
        document.addEventListener("click", manejarClickMenu, true);
        window.addEventListener("resize", sincronizarVista);
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", iniciar);
    } else {
        iniciar();
    }
}());
