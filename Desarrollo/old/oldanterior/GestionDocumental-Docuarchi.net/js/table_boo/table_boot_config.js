
function colum_index_table_boot(colum_name, nombre_table) {
    try {
        var x = $("#" + nombre_table + " th");
        for (var i = 0; i < x.length; i++) {
            let values = x[i].getAttribute("data-field");
            if (values.toUpperCase() == colum_name.toUpperCase()) {
                return i;
            }
        }
        return -1;
    }
    catch (err) {
        alert(err.message + " funcion colum_index_table_boot " + err.message);
    }
}
const resize_table_boot_manager = (name_content_table, name_content_parent) => {
    try {
        var element_table_body = document.getElementById(name_content_table).getElementsByClassName('fixed-table-body');
        var element_table = document.getElementById(name_content_table).getElementsByClassName('fixed-table-container');
        var element_toolbar = document.getElementById(name_content_table).getElementsByClassName('fixed-table-toolbar');
        var element_pagination = document.getElementById(name_content_table).getElementsByClassName('fixed-table-pagination');
        $(element_table_body[0]).css("height", (document.getElementById(name_content_parent).clientHeight - (element_toolbar[0].clientHeight + element_pagination[0].clientHeight)) + "px");
        $(element_table[0]).css("height", (element_table_body[0].clientHeight) + "px");
        if (element_pagination[0]) {
            element_pagination[0].classList.add("pt-2");
        }
        return "YES";
    }
    catch (err) {
        return err.message;
    }

}
function mystopfunction_update_table_boot_manager(name_table, error, row_table) {
    try {   
        var $table = $('#' + name_table)
        switch (error) {
            case "ZERO":
                $table.bootstrapTable('removeAll');
                break;
            case "YES":
                $table.bootstrapTable('destroy').bootstrapTable({ data: row_table });
                break;
  
            default:
                $table.bootstrapTable('removeAll');
                alert(error);
                break;
        }
       
    } catch (err) {
        alert(err.message + " Funcion mystopfunction_update_table_boot_manager");
    }
}
function actualiza_table_boot(name_table, id, stru_campos_value) {
    try {
        $("#" + name_table + " tr[data-index=" + id + "]").each(function () {
            for (i = 0; i < stru_campos_value.length; i++) {
                let idex = -1;
                idex = colum_index_table_boot(stru_campos_value[i].name_campo, name_table);
                if (idex != -1) {
                    if (stru_campos_value[i].value_campo == "") {
                        $(this)[0].cells[idex].innerText = "\u00a0";
                    } else {
                        $(this)[0].cells[idex].innerHTML = stru_campos_value[i].value_campo;
                    }
                }
            }
        })
        return true;
    }
    catch (err) {
        alert(err.message + " Funcion actualiza_table_boot");
    }
}
//retorna objeto jonson de row
const table_boot_return_objet_jonson = (row) => {
    let json_string = JSON.stringify(row);
    localStorage.setItem("testJSON", json_string);
    let text = localStorage.getItem("testJSON");
    let obj = JSON.parse(text);
    return obj;
}
const table_boot_return_get_data_Jonson = (table) => {
    var $table = $('#' + table);
    let json_string = JSON.stringify($table.bootstrapTable('getData'));
    localStorage.setItem("testJSON", json_string);
    let text = localStorage.getItem("testJSON");
    let obj = JSON.parse(text);
    return obj;
}
const table_boot_return_get_data = (table) => {
    var $table = $('#' + table);
    let obj = ($table.bootstrapTable('getData'));
    return obj;
}
const table_boot_remove_class = (table,nameClass) => {
    let classes = [];
    let $table = $('#' + table);
    classes.push(nameClass);
    $table.bootstrapTable('refreshOptions', {
        classes: classes.join(' ')
    })
}
//Ajusta el alto de la tabla segun el parent o contendor
const resize_table_boot = (name_table, class_delete, class_add) => {
    let heigTable = document.getElementById(name_table).parentElement.parentElement.parentElement.parentElement.clientHeight - 30;
    $('#' + name_table).bootstrapTable('resetView', { height: heigTable });
    if (class_delete !== null && class_add !== null) {
        document.getElementById(name_table).classList.remove(class_delete);
        document.getElementById(name_table).classList.add(class_add);
    }
}

/*
 * Compatibilidad con los llamados históricos de Workflow.
 * Algunos modales calculan una altura específica y la entregan directamente
 * a la tabla Bootstrap. La función original no quedó incluida en el bundle
 * común, por lo que el modal terminaba con un ReferenceError.
 */
function table_reize_heigth(name_table, height_table, class_delete, class_add) {
    try {
        const table = document.getElementById(name_table);
        const height = Number(height_table);

        if (!table || !Number.isFinite(height) || height < 0) {
            return "NO";
        }

        if (class_delete) {
            table.classList.remove(class_delete);
        }

        if (class_add) {
            table.classList.add(class_add);
        }

        const $table = $(table);
        if (typeof $table.bootstrapTable === "function" && $table.data("bootstrap.table")) {
            $table.bootstrapTable("resetView", { height: Math.round(height) });
        }

        return "YES";
    } catch (error) {
        return error.message;
    }
}
//actualiza tabla   - table: nombre de la tabla  -field_name : nombre del campo actualizar -identi: identificador  - value: valor a actualizar
const updateCelByUniqueId = (table, field_name, identi, value) => {
    let $table = $('#' + table);
    $table.bootstrapTable('updateCellByUniqueId', {
        id: identi,
        field: field_name,
        value: value,
        reinit: false
    });
}
const updateCelByUniqueIdReinit = (table, field_name, identi, value) => {
    let $table = $('#' + table);
    $table.bootstrapTable('updateCellByUniqueId', {
        id: identi,
        field: field_name,
        value: value,
        reinit: true
    });
}
const UpdaTeRows = (table, identi, parameTerRows) => {
    for (let i = 0; i <= parameTerRows.length - 1; i++) {
        updateCelByUniqueId(table, parameTerRows[i].name_campo, identi, parameTerRows[i].value_campo);
    }
}
const UpdaTeRowsReinit = (table, identi, parameTerRows) => {
    for (let i = 0; i <= parameTerRows.length - 1; i++) {
        updateCelByUniqueIdReinit(table, parameTerRows[i].name_campo, identi, parameTerRows[i].value_campo);
    }
}
const insert_row_table = (name_table, row) => {
    let $table = $('#' + name_table);
    $table.bootstrapTable('insertRow', {
        index: 150000000,
        row: row
    })
}
const delete_row_table = (name_table, field, value) => {
    let $table = $('#' + name_table);
    $table.bootstrapTable('removeByUniqueId', value);
}
const delete_row_all = (name_table) => {
    let $table = $('#' + name_table);
    $table.bootstrapTable('removeAll');
}
const total_row_table = (name_table) => {
    let $table = $('#' + name_table);
    let row = $table.bootstrapTable('getOptions').totalRows;
    return row;
}
const TableAgregaIconoAwesonGabinete = (ValueDbt) => {
    let icono_font;
    switch (ValueDbt) {
        case -1 : {
        icono_font = "fa-file-pdf";
        break;
        }
        case -2: {
            icono_font = "fa-file-pdf";
            break;
        }
        case -10: {
            icono_font = "fa-file";
            break;
        }
        case -11: {
            icono_font = "fa-file";
            break;
        }
        case -3: {
            icono_font = "fa-file";
            break;
        }
        case -30: {
            icono_font = "fa-file";
            break;
        }
        case -33: {
            icono_font = "fa-file";
            break;
        }
        case -4: {
            icono_font = "fa-file";
            break;
        }
        case -40 : {
            icono_font = "fa-file";
            break;
        }
        case -44 : {
            icono_font = "fa-file";
            break;
        }
        case -2: {
            icono_font = "fa-file-pdf";
            break;
        }
        case -20: {
            icono_font = "fa-file-pdf";
            break;
        }
        case -22: {
            icono_font = "fa-file-pdf";
            break;
        }
        case -5 : {
            icono_font = "fa-file-word";
            break;
        }
        case -50 : {
            icono_font = "fa-file-word";
            break
        }
        case -55 : {
            icono_font = "fa-file-word";
            break;
        }
        case -51 : {
            icono_font = "fa-file-word";
            break;
        }
        case -510 : {
            icono_font = "fa-file-word";
            break;
        }
        case -561 : {
            icono_font = "fa-file-word";
            break;
        }
        case -52 : {
            icono_font = "fa-file-excel";
            break;
        }
        case -520 : {
            icono_font = "fa-file-excel";
            break;
        }
        case -572 : {
            icono_font = "fa-file-excel";
            break;
        }
        case -53 : {
            icono_font = "fa-file-excel";
            break;
        }
        case -530 : {
            icono_font = "fa-file-excel";
            break;
        }
        case -583 : {
            icono_font = "fa-file-excel";
            break;
        }
        case -54 : {
            icono_font = "fa-file-powerpoint";
            break;
        }
        case -540 : {
            icono_font = "fa-file-powerpoint";
            break;
        }
        case -594 : {
            icono_font = "fal fa-file-powerpoint";
            break;
        }
        case -15 : {
            icono_font = "fal fa-file-powerpoint";
            break;
        }
        case -550 : {
            icono_font = "fa-file-powerpoint";
            break;
        }
        case -605 : {
            icono_font = "fa-file-powerpoint";
            break;
        }
        default: {
            icono_font = "fa-file-exclamation";
            break;
        }
    }
    return icono_font;
}
/**
 * Convierte el array del backend en un objeto dinámico
 */
function buildFieldsToUpdate(arrayCampos) {
    let obj = {};

    arrayCampos.forEach(campo => {
        let keys = Object.keys(campo);

        // Buscamos la clave que haga referencia al nombre del campo
        let keyNombre = keys.find(k => k.toLowerCase().includes("nombre") || k.toLowerCase().includes("vombre") || k.toLowerCase().includes("campo"));
        // Buscamos la clave que haga referencia al valor
        let keyValor = keys.find(k => k.toLowerCase().includes("valor") || k.toLowerCase().includes("value"));

        if (keyNombre && keyValor) {
            let nombreCampo = campo[keyNombre];
            let valorCampo = campo[keyValor];
            obj[nombreCampo] = valorCampo;
        }
    });

    return obj;
}

/**
 * Actualiza un registro en bootstrapTable por ID dinámicamente
 * @param {string} tableId - ID de la tabla
 * @param {any} recordId - Valor de la clave primaria (ej: Id=101)
 * @param {Object|Array} campos - Puede ser:
 *    - Objeto directo {campo: valor}
 *    - Array del backend [{VombreCampo, TipoCampo, ValorCampo}]
 * @param {boolean} uncheckRow - Opcional: si true desmarca el check y la selección de la fila
 */
const updateRecordById = async (tableId, recordId, campos, uncheckRow = false) => {
    try {
        let fieldsToUpdate;
        // 🔹 Detecta si viene array del backend o un objeto plano
        if (Array.isArray(campos)) {
            fieldsToUpdate = buildFieldsToUpdate(campos);
        } else if (typeof campos === "object" && campos !== null) {
            fieldsToUpdate = campos;
        } else {
            throw new Error("Formato de campos inválido");
        }
        const $table = $('#' + tableId);

        // Detectar el campo único (por defecto "id")
        const uniqueIdField = $table.attr('data-unique-id') || 'id';
        let row = $table.bootstrapTable('getRowByUniqueId', recordId);
        if (!row) {
            console.warn(`⚠️ Registro con ${uniqueIdField}=${recordId} no encontrado en la tabla ${tableId}`);
            return `⚠️ Registro con ${uniqueIdField}=${recordId} no encontrado en la tabla ${tableId}`;
        }
        // 🔹 Actualizar registro
        $('#' + tableId).bootstrapTable('updateByUniqueId', {
            id: recordId,
            row: fieldsToUpdate
        });

        // 🔹 Opcional: quitar selección y check
        if (uncheckRow) {
            $table.bootstrapTable('uncheckBy', { field: uniqueIdField, values: [recordId] });
        }

        return "YES";
    } catch (ex) {
        return "Error en updateRecordById: " + ex.message;
    }
}

const deleteRecordById = async (tableId, recordId) => {
    try {
        const $table = $('#' + tableId);

        // Detectar el campo único (por defecto "ID")
        const uniqueIdField = $table.attr('data-unique-id') || 'ID';

        let row = $table.bootstrapTable('getRowByUniqueId', recordId);
        if (!row) {
            console.warn(`⚠️ Registro con ${uniqueIdField}=${recordId} no encontrado en la tabla ${tableId}`);
            return `⚠️ Registro con ${uniqueIdField}=${recordId} no encontrado en la tabla ${tableId}`;
        }

        // 🔹 Eliminar registro de la tabla
        $table.bootstrapTable('removeByUniqueId', recordId);

        return "YES";
    } catch (ex) {
        return "Error en deleteRecordById: " + ex.message;
    }
};

(function ($) {
    $.fn.exportTableToExcelHybrid = function (optionsOrMethod) {
        // Verifica si es un llamado a un método (en este caso, 'update')
        if (typeof optionsOrMethod === "string") {
            let method = optionsOrMethod;
            let args = arguments[1] || {};
            return this.each(function () {
                const $table = $(this);
                let settings = $table.data("exportTableToExcelHybrid");

                if (!settings) return;

                // Si el método es 'update', actualizamos la configuración
                if (method === "update") {
                    settings = $.extend(settings, args);
                    $table.data("exportTableToExcelHybrid", settings);
                }
            });
        } else {
            // Modo inicialización
            const settings = $.extend({
                fileName: "Reporte",          // Nombre por defecto del archivo
                buttonSelector: null,         // Botón externo (si se desea usar)
                spinnerSelector: null,        // Spinner dentro del toolbar (opcional)
                toolbarButton: true,          // Activar el botón en el toolbar de la tabla
                companyName: "Mi Empresa",    // Nombre de la empresa
                reportName: "Reporte Anual",  // Nombre del reporte
                userName: "Juan Pérez",       // Nombre del usuario
                excludedHeaders: ["check", "operation", "OPERATION", "OPOCIONES"] // Encabezados excluidos
            }, optionsOrMethod);

            return this.each(function () {
                const $table = $(this);
                $table.data("exportTableToExcelHybrid", settings); // Guardar la configuración en data

                let $button, $spinner;

                // Si hay botón externo
                if (settings.buttonSelector) {
                    $button = $(settings.buttonSelector);
                    $spinner = settings.spinnerSelector ? $(settings.spinnerSelector) : null;
                    if ($button.length === 0) {
                        console.warn("⚠️ No se encontró el botón externo definido en buttonSelector");
                        return;
                    }
                }
                // Si NO hay botón externo, agregar al toolbar
                else if (settings.toolbarButton) {
                    let $toolbar;

                    // Detectar si usa toolbar externo
                    const externalToolbarSelector = $table.data("toolbar");
                    if (externalToolbarSelector) {
                        $toolbar = $(externalToolbarSelector);
                    } else {
                        // Toolbar interno de bootstrap-table   
                        $toolbar = $table.closest(".bootstrap-table").find(".fixed-table-toolbar .columns-right");
                    }

                    if ($toolbar.length) {
                        $button = $('<button class="btn menu-button"><i class="fas fa-download"></i> Descargar</button>');
                        $spinner = $('<span class="spinner-border spinner-border-sm text-success ms-2 d-none"></span>');
                        $toolbar.append($button).append($spinner);
                    }
                }

                if (!$button) return;

                // Acción del botón de exportación
                $button.on("click", function () {
                    if ($spinner && $spinner.length) $spinner.removeClass("d-none");

                    // Obtener los datos de la tabla
                    const data = $table.bootstrapTable("getData", { useCurrentPage: false });

                    // Crear la hoja de trabajo con los encabezados personalizados
                    const worksheet = XLSX.utils.aoa_to_sheet([
                        ["Reporte", settings.reportName],      // A1: Reporte, B1: Nombre del reporte
                        ["Fecha", new Date().toLocaleDateString()], // A2: Fecha, B2: Fecha actual
                        ["Empresa", settings.companyName],    // A3: Empresa, B3: Nombre de la empresa
                        ["Usuario", settings.userName]        // A4: Usuario, B4: Nombre del usuario
                    ]);

                    // Obtener los encabezados de la tabla (th) sin los de "check" y "operation"
                    const headers = [];
                    $table.find('thead th').each(function () {
                        const headerText = $(this).text().trim();

                        // Filtrar encabezados no deseados, basado en una lista de términos excluidos
                        const excluded = settings.excludedHeaders.some(term => headerText.toLowerCase().includes(term.toLowerCase()));

                        // Solo incluir encabezados que no estén en la lista de exclusión
                        if (headerText !== "" && !excluded) {
                            headers.push(headerText);
                        }
                    });

                    // Insertar los encabezados en la primera fila debajo de los títulos
                    XLSX.utils.sheet_add_aoa(worksheet, [headers], { origin: 4 });

                    // Añadir los datos de la tabla debajo de los encabezados
                    XLSX.utils.sheet_add_json(worksheet, data, { skipHeader: true, origin: -1 });

                    // Escribir el archivo Excel
                    const workbook = XLSX.utils.book_new();
                    XLSX.utils.book_append_sheet(workbook, worksheet, "Reporte");

                    // Generar y descargar el archivo
                    XLSX.writeFile(workbook, settings.fileName + ".xlsx");

                    // Ocultar el spinner después de un breve retraso
                    if ($spinner && $spinner.length) {
                        setTimeout(() => $spinner.addClass("d-none"), 500);
                    }
                });
            });
        }
    };
})(jQuery);
/*
 * Inicializa el reporte
 * $('#table_consulta_gabinete').exportTableToExcelHybrid({
    buttonSelector: "#btn_dow_load_gabonete",
    toolbarButton: false,
    companyName: "Mi Empresa",
    reportName: "Reporte Anual",
    userName: "Juan Pérez"
});

$('#table_consulta_gabinete').exportTableToExcelHybrid("update", {
    reportName: "Nuevo Reporte",
    userName: "Carlos Pérez",
    companyName: "Nueva Empresa"
});
/*
 * Descripción de los parámetros para la función exportTableToExcelHybrid:
 *
 * 1. fileName:
 *    - (string) El nombre del archivo que se generará al exportar la tabla a Excel.
 *    - Valor predeterminado: "Reporte".
 *    - Ejemplo: "ReporteAnual.xlsx".
 *
 * 2. buttonSelector:
 *    - (string) El selector del botón externo que activa la exportación de la tabla a Excel.
 *    - Si se proporciona, el plugin añadirá un evento `click` a este botón.
 *    - Valor predeterminado: null.
 *    - Ejemplo: "#btn_dow_load_gabonete".
 *
 * 3. spinnerSelector:
 *    - (string) El selector de un spinner (ícono de carga) que se mostrará mientras el archivo está siendo generado y descargado.
 *    - Valor predeterminado: null.
 *    - Ejemplo: "#spinner_id".
 *
 * 4. toolbarButton:
 *    - (boolean) Determina si se debe agregar un botón de exportación a la barra de herramientas de la tabla automáticamente.
 *    - Si es true, el botón será agregado. Si es false, no se agrega.
 *    - Valor predeterminado: true.
 *    - Ejemplo: false.
 *
 * 5. companyName:
 *    - (string) El nombre de la empresa que aparecerá en el archivo Excel exportado.
 *    - Valor predeterminado: "Mi Empresa".
 *    - Ejemplo: "Tech Solutions S.A.".
 *
 * 6. reportName:
 *    - (string) El nombre del reporte que aparecerá en el archivo Excel exportado.
 *    - Valor predeterminado: "Reporte Anual".
 *    - Ejemplo: "Reporte de Ventas 2025".
 *
 * 7. userName:
 *    - (string) El nombre del usuario que generó el reporte.
 *    - Valor predeterminado: "Juan Pérez".
 *    - Ejemplo: "Carlos Gómez".
 *
 * 8. excludedHeaders:
 *    - (array) Lista de encabezados de tabla que deben ser excluidos del archivo Excel.
 *    - Estos encabezados no serán añadidos al archivo Excel generado.
 *    - Se filtran comparando el texto de los encabezados con los valores en este arreglo.
 *    - Valor predeterminado: ["check", "operation", "OPERATION", "OPOCIONES"].
 *    - Ejemplo: ["acciones", "editar"].
 *
 * Métodos disponibles:
 *
 * 1. update:
 *    - Después de la inicialización, se puede usar este método para actualizar los parámetros.
 *    - Ejemplo:
 *      $('#table_consulta_gabinete').exportTableToExcelHybrid("update", {
 *          reportName: "Nuevo Reporte",
 *          userName: "Carlos Pérez",
 *          companyName: "Nueva Empresa"
 *      });
 *
 * La función genera un archivo Excel con:
 * - El nombre de la empresa (companyName),
 * - El nombre del reporte (reportName),
 * - El nombre del usuario (userName),
 * - La fecha actual,
 * - Los encabezados y los datos de la tabla.
 *
 * Este archivo Excel se descarga automáticamente con el nombre especificado en fileName.
 */

 


















