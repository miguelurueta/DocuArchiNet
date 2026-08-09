/* Adaptador de presentación para GridView_list_documento_relacion_wf.
   No inserta, borra, mueve ni intercepta filas o acciones de documentos. */
(function (window, document) {
    "use strict";

    var tableId = "GridView_list_documento_relacion_wf";
    var visualClass = "gridview-documentos-relacionados";

    function buscarFila(element) {
        while (element && element.tagName !== "TR") {
            element = element.parentNode;
        }
        return element;
    }

    function sincronizarSeleccion(table) {
        var checks = table.querySelectorAll(".chek_selecion_list_wf");
        var index;
        for (index = 0; index < checks.length; index += 1) {
            var row = buscarFila(checks[index]);
            if (row) {
                if (checks[index].checked) {
                    row.classList.add("gridview-documento-marcado");
                } else {
                    row.classList.remove("gridview-documento-marcado");
                }
            }
        }
    }

    function sincronizarFilaSeleccionada(table, fallbackRow) {
        var campoSeleccion = document.getElementById("hiden_seleccion_documento_id_wf");
        var idSeleccionado = campoSeleccion ? campoSeleccion.value : "";
        var rows = table.querySelectorAll(".GridviewRow");
        var selectedRow = null;
        var index;

        if (idSeleccionado && idSeleccionado !== "-1") {
            for (index = 0; index < rows.length; index += 1) {
                if (rows[index].getAttribute("id_wf") === idSeleccionado) {
                    selectedRow = rows[index];
                    break;
                }
            }
        } else if (fallbackRow) {
            selectedRow = fallbackRow;
        }

        for (index = 0; index < rows.length; index += 1) {
            rows[index].classList.remove("gridview-documento-seleccionado");
        }
        if (selectedRow) {
            selectedRow.classList.add("gridview-documento-seleccionado");
        }
    }

    function textoDocumento(row) {
        var title = row.querySelector(".GridviewSpanOverFlow");
        return title ? title.textContent.replace(/^\s+|\s+$/g, "") : "documento";
    }

    function aplicarPresentacion() {
        var tables = document.querySelectorAll('[id="' + tableId + '"]');
        var table;
        var firstHeader;
        var toggles;
        var index;

        /* La duplicidad es un defecto de renderizado que no se oculta ni se
           corrige aquí: operar sobre IDs duplicados sería inseguro. */
        if (tables.length !== 1) {
            if (window.console && window.console.warn) {
                window.console.warn("Documentos relacionados: se esperó una tabla y se encontraron " + tables.length + ".");
            }
            return;
        }

        table = tables[0];
        if (table.className.indexOf(visualClass) === -1) {
            table.className += " " + visualClass;
        }

        firstHeader = table.querySelector(".GridviewScrollHeader_line_boot_none th:first-child");
        if (firstHeader) {
            firstHeader.setAttribute("aria-label", "Selección");
        }

        toggles = table.querySelectorAll('a[data-toggle="dropdown"]');
        for (index = 0; index < toggles.length; index += 1) {
            var toggle = toggles[index];
            var row = buscarFila(toggle);
            toggle.setAttribute("title", "Más acciones");
            toggle.setAttribute("aria-label", "Más acciones para: " + (row ? textoDocumento(row) : "documento"));
        }

        sincronizarSeleccion(table);
        sincronizarFilaSeleccionada(table);
    }

    function registrarActualizacionParcial() {
        if (window.Sys && window.Sys.WebForms && window.Sys.WebForms.PageRequestManager) {
            window.Sys.WebForms.PageRequestManager.getInstance().add_endRequest(aplicarPresentacion);
        }
    }

    document.addEventListener("change", function (event) {
        var check = event.target;
        var row;
        if (!check || check.className.indexOf("chek_selecion_list_wf") === -1) {
            return;
        }
        row = buscarFila(check);
        if (row) {
            if (check.checked) {
                row.classList.add("gridview-documento-marcado");
            } else {
                row.classList.remove("gridview-documento-marcado");
            }
        }
    }, true);

    document.addEventListener("click", function (event) {
        var target = event.target;
        var row = buscarFila(target);
        var table = document.getElementById(tableId);

        if (!row || !table || !table.contains(row)) {
            return;
        }

        /* Las acciones, menú y checkboxes conservan su propio comportamiento. */
        while (target && target !== row) {
            if (target.tagName === "A" || target.tagName === "INPUT" || target.tagName === "BUTTON") {
                return;
            }
            target = target.parentNode;
        }

        /* El manejador legado actualiza los hidden de selección durante el
           mismo clic. Se lee después para no mostrar una fila distinta. */
        window.setTimeout(function () {
            sincronizarFilaSeleccionada(table, row);
        }, 0);
    }, true);

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", function () {
            aplicarPresentacion();
            registrarActualizacionParcial();
        });
    } else {
        aplicarPresentacion();
        registrarActualizacionParcial();
    }
}(window, document));
