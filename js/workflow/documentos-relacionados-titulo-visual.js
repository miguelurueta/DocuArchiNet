/* Reubica acciones secundarias sin modificar sus IDs ni manejadores legado. */
(function (window, document) {
    "use strict";

    function agregarTexto(accion, texto) {
        var textoExistente;
        if (!accion) {
            return;
        }
        textoExistente = accion.querySelector(".documentos-relacionados-texto-accion");
        if (!textoExistente) {
            textoExistente = document.createElement("span");
            textoExistente.className = "documentos-relacionados-texto-accion";
            textoExistente.appendChild(document.createTextNode(texto));
            accion.appendChild(textoExistente);
        }
        accion.setAttribute("aria-label", texto);
    }

    function conservaAccionesRapidasDelPiloto() {
        var root = document.getElementById("div_content_general_wf");
        return !!(root && root.classList &&
            root.classList.contains("workflow-centro-trabajo-moderno") &&
            root.classList.contains("ctw-layer-documents"));
    }

    function moverAccionesSecundarias() {
        var titulo = document.getElementById("div_label");
        var menu;
        var cargaArchivo;
        var servicio;
        var indice;
        var divisor;
        var acciones;
        var index;

        if (!titulo) {
            return;
        }

        /* En DOC-2 las acciones existentes forman parte de la cabecera contextual.
           No se mueven, no se cambian IDs y no se toca su comportamiento. */
        if (conservaAccionesRapidasDelPiloto()) {
            return;
        }

        menu = titulo.querySelector(".dropright .dropdown-menu");
        cargaArchivo = titulo.querySelector("#btnLoadFile");
        servicio = titulo.querySelector("#btnloadservice");
        indice = titulo.querySelector('a[title="Actualiza indice batch"]');
        if (!menu) {
            return;
        }

        acciones = [cargaArchivo, servicio, indice];
        for (index = 0; index < acciones.length; index += 1) {
            if (acciones[index]) {
                acciones[index].classList.add("dropdown-item", "font-weight-light");
            }
        }

        /* Las acciones operativas se mantienen primero y en un orden estable. */
        if (cargaArchivo) {
            menu.insertBefore(cargaArchivo, menu.firstChild);
        }
        if (servicio) {
            menu.insertBefore(servicio, cargaArchivo ? cargaArchivo.nextSibling : menu.firstChild);
        }
        if (indice) {
            menu.insertBefore(indice, servicio ? servicio.nextSibling : (cargaArchivo ? cargaArchivo.nextSibling : menu.firstChild));
        }

        divisor = menu.querySelector(".documentos-relacionados-divisor-rapidas");
        if (!divisor) {
            divisor = document.createElement("div");
            divisor.className = "dropdown-divider documentos-relacionados-divisor-rapidas";
        }
        if (indice) {
            menu.insertBefore(divisor, indice.nextSibling);
        } else if (servicio) {
            menu.insertBefore(divisor, servicio.nextSibling);
        } else if (cargaArchivo) {
            menu.insertBefore(divisor, cargaArchivo.nextSibling);
        }

        agregarTexto(cargaArchivo, "Cargar archivos");
        agregarTexto(servicio, "Adjuntar desde servicio web");
        agregarTexto(indice, "Actualizar índice batch");
    }

    function registrarActualizacionParcial() {
        if (window.Sys && window.Sys.WebForms && window.Sys.WebForms.PageRequestManager) {
            window.Sys.WebForms.PageRequestManager.getInstance().add_endRequest(moverAccionesSecundarias);
        }
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", function () {
            moverAccionesSecundarias();
            registrarActualizacionParcial();
        });
    } else {
        moverAccionesSecundarias();
        registrarActualizacionParcial();
    }
}(window, document));
