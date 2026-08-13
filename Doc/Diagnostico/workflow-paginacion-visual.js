/* Activa la capa visual reutilizable y normaliza el paginador nativo de Workflow. */
(function () {
    "use strict";
    var manejadorRegistrado = false;

    function importante(elemento, propiedad, valor) {
        elemento.style.setProperty(propiedad, valor, "important");
    }

    function activarGridviewModerno() {
        var tabla = document.getElementById("GridView2");
        if (tabla) tabla.classList.add("gridview-moderno");
        return tabla;
    }

    function normalizarPaginacionWorkflow(tabla) {
        if (!tabla) return;

        var tablaInterna = tabla.querySelector("tr.pagination-ys > td > table");
        if (!tablaInterna) return;

        importante(tablaInterna, "border-collapse", "collapse");
        importante(tablaInterna, "border-spacing", "0");
        importante(tablaInterna, "table-layout", "fixed");

        var celdas = tablaInterna.querySelectorAll("tbody > tr > td");
        for (var i = 0; i < celdas.length; i++) {
            var celda = celdas[i];
            importante(celda, "display", "table-cell");
            importante(celda, "width", "38px");
            importante(celda, "min-width", "38px");
            importante(celda, "max-width", "38px");
            importante(celda, "padding", "0");
            importante(celda, "margin", "0");
            importante(celda, "text-align", "center");
            importante(celda, "vertical-align", "middle");

            var control = celda.querySelector("a, span");
            if (!control) continue;
            importante(control, "display", "flex");
            importante(control, "box-sizing", "border-box");
            importante(control, "width", "38px");
            importante(control, "height", "34px");
            importante(control, "padding", "0");
            importante(control, "margin", "0");
            importante(control, "float", "none");
            importante(control, "align-items", "center");
            importante(control, "justify-content", "center");
            importante(control, "text-align", "center");
        }
    }

    function aplicarCapaVisualWorkflow() {
        normalizarPaginacionWorkflow(activarGridviewModerno());
    }

    function registrarActualizacionAspNet() {
        if (manejadorRegistrado || !window.Sys || !Sys.WebForms || !Sys.WebForms.PageRequestManager) return;
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(aplicarCapaVisualWorkflow);
        manejadorRegistrado = true;
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", aplicarCapaVisualWorkflow);
    } else {
        aplicarCapaVisualWorkflow();
    }
    window.setTimeout(registrarActualizacionAspNet, 0);
    window.addEventListener("load", function () {
        registrarActualizacionAspNet();
        aplicarCapaVisualWorkflow();
    });
}());
