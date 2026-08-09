/* Menú vertical responsive sin cambiar Scoop ni el backend. */
(function () {
    "use strict";
    var puntoCierre = 992;

    function obtenerMenu() {
        return document.getElementById("scoop");
    }

    function vistaReducida() {
        return window.matchMedia("(max-width: " + puntoCierre + "px)").matches;
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

    function iniciar() {
        var menu = obtenerMenu();
        if (!menu) return;
        sincronizarVista();

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
