
const init_row_feld_table_boostrap_table = (
    name_table,
    array_fiels_table_bootstra,
    array_row_table_bootstra,
    name_parent_table,
    class_delete,
    class_add,
    selectionMode = "single",      // "single" | "multiple"
    selectionColor = "bt-selected",    // hex (#e7ebf6) o clase CSS
    showCheckbox = false,
    noBorders = true,              // quitar bordes de tr/td
    enableHover = false,           // habilitar/deshabilitar table-hover
    roundedSelected = true,        // bordes redondeados en seleccionados
    allowedClasses = ["vis_doc_selecion_rad"], // SOLO estas clases disparan selección
    allowedcollapse = false
) => {
    const $table = $('#' + name_table);

    // --- CSS utilitario (una sola vez) ---
    if (!document.getElementById('bt-table-utils')) {
        const style = document.createElement('style');
        style.id = 'bt-table-utils';
        style.textContent = `
            .table-no-borders :is(th, td) {
                border: none !important;
            }
            /* Clase por si se usa selectionColor como nombre de clase */
            .bt-selected { background-color: #e7ebf6; }
        `;
        document.head.appendChild(style);
    }

    // Altura
    let height_table = 340;
    const parent = document.getElementById(name_parent_table);
    if (parent) height_table = parent.clientHeight - 5;

    // Clases de tabla
    const classes = ["table"];
    if (enableHover) classes.push("table-hover");
    if (class_add) classes.push(class_add);
    if (noBorders) classes.push("table-no-borders");

    // Columnas con checkbox opcional
    const columnsConfig = showCheckbox ? [{ checkbox: true }, ...array_fiels_table_bootstra]
        : [...array_fiels_table_bootstra];

    array_fiels_table_bootstra.forEach((field, index) => {
        // Verifica si el campo tiene la propiedad 'data_sortable' igual a true
        if (field.data_sortable === true || field.data_sortable === 'true') {
            field['data-sortable'] = true;  // Agregar el atributo data-sortable
        }
    });

    // Helper para aplicar título y estilo
    const applyTitlesToCells = () => {
        document.querySelectorAll(`#${name_table} td`).forEach(td => {
            var cellText = td.innerHTML.trim();
            if (!td.querySelector('input, select, textarea, button, div')) {
                td.setAttribute('title', cellText);
                td.style.maxWidth = '250px';
                td.style.textOverflow = 'ellipsis';
                td.style.overflow = 'hidden';
                td.style.whiteSpace = 'nowrap';
            }
        });
    };

    // Helpers
    const applyRowColor = ($tr, color, selected) => {
        // color por HEX o por clase
        if (color && color.startsWith && color.startsWith("#")) {
            $tr.find("td").css("background-color", selected ? color : "");
        } else if (color) {
            $tr.toggleClass(color, selected);
        } else {
            $tr.toggleClass("bt-selected", selected);
        }
        $tr.attr("data-selected", selected ? "1" : "0");

        // Bordes redondeados (en celdas extrema izquierda/derecha)
        if (roundedSelected) {
            $tr.find("td").css("border-radius", ""); // reset
            if (selected) {
                $tr.find("td:first-child").css({
                    "border-top-left-radius": "10px",
                    "border-bottom-left-radius": "10px"
                });
                $tr.find("td:last-child").css({
                    "border-top-right-radius": "10px",
                    "border-bottom-right-radius": "10px"
                });
            }
        }
    };

    const clearAllSelections = (color) => {
        $table.find("tbody tr").each(function () {
            applyRowColor($(this), color, false);
            if (showCheckbox) {
                const index = $(this).attr("data-index");
                if (index != null) $table.bootstrapTable('uncheck', parseInt(index, 10));
            }
        });
    };

    // 👉 No des-selecciona si ya está seleccionado (según lo que pediste antes)
    const forceSelectRow = ($tr) => {
        const mode = $table.data("selectionMode");
        const color = $table.data("selectionColor");
        const isSelected = $tr.attr("data-selected") === "1";
        if (isSelected) return; // no quitar color ni cambiar estado

        if (mode === "single") clearAllSelections(color);

        applyRowColor($tr, color, true);

        // sincroniza checkbox si corresponde
        if (showCheckbox) {
            const index = $tr.attr("data-index");
            if (index != null) $table.bootstrapTable('check', parseInt(index, 10));
        }
    };

    // Init tabla
    $table.bootstrapTable('destroy').bootstrapTable({
        height: height_table,
        columns: columnsConfig,
        data: array_row_table_bootstra,
        classes: classes.join(' '),
        clickToSelect: showCheckbox,
        singleSelect: (selectionMode === "single"),
        maintainMetaData: true
    }).on('sorted.bs.table', function () {
        applyTitlesToCells(); // Aplicar título y estilo a las celdas
    });

    // Guardar config
    $table.data("selectionMode", selectionMode);
    $table.data("selectionColor", selectionColor);

    // Re-aplicar los títulos a las celdas después del ordenamiento
    setTimeout(function () {
        $table.on('sorted.bs.table', function () {
            applyTitlesToCells(); // Aplicar título y estilo a las celdas
        });
    }, 1000); // Ajusta el tiempo si es necesario
    $table.on('reflow.bs.table', function () {
        applyTitlesToCells(); // Aplicar título y estilo a las celdas
    });
    // Inicializar títulos antes de la carga
    applyTitlesToCells();

    // --- Desactivar selección por clic general en fila ---
    // (Dejamos el evento pero no hacemos nada, para que la selección
    // solo ocurra por clic en elementos con clases permitidas)
    $table.off('click-row.bs.table').on('click-row.bs.table', function (e, row, $element) {
        // No togglear aquí
    });

    // --- Delegación de clic SOLO para clases permitidas ---
    const allowedSelector = allowedClasses
        .filter(Boolean)
        .map(cls => '.' + cls.replace(/^\./, ''))
        .join(', ');

    if (allowedSelector.length) {
        $table.off('click.allowedSelect');
        $table.on('click.allowedSelect', allowedSelector, function (ev) {
            const $target = $(ev.target);
            if ($target.closest("[data-toggle='dropdown'], [data-bs-toggle='dropdown']").length) return;

            const $tr = $target.closest('tr');
            if (!$tr.length) return;

            forceSelectRow($tr);
            ev.stopPropagation();
        });
    }

    // Mantener sync con checkbox, sin cambiar color desde el check
    if (showCheckbox) {
        $table
            .off('check.bs.table uncheck.bs.table check-all.bs.table uncheck-all.bs.table')
            .on('check.bs.table uncheck.bs.table', function (e, row, $element) { })
            .on('check-all.bs.table uncheck-all.bs.table', function () {
                const color = $table.data("selectionColor");
                $table.find("tbody tr").each(function () {
                    applyRowColor($(this), color, false);
                });
            });
    }

    // Aplicar el título a las celdas si corresponde
    if (allowedcollapse == true) {
        applyTitlesToCells();
    }
};

// helper para recuperar seleccionados
function getSelectedRows(tableId) {
    const $table = $('#' + tableId);
    const selected = [];
    $table.find("tbody tr[data-selected='1']").each(function () {
        const idx = $(this).attr("data-index");
        const row = $table.bootstrapTable('getRowByIndex', parseInt(idx, 10));
        selected.push(row);
    });
    return selected;
}
const init_row_constant_table_boostrap_table = (
    name_table,
    array_row_table_bootstra,
    name_parent_table,
    class_delete,
    class_add,
    RowResult,
    selectionMode = "single",      // "single" | "multiple"
    selectionColor = "bt-selected",    // color selección
    showCheckbox = false,          // mostrar checkboxes
    noBorders = true,              // quitar bordes de celdas
    enableHover = false,            // activar/desactivar hover
    roundedSelected = true,        // bordes redondeados
    allowedClasses = ["active_version_document_view_document"], // clases que activan selección
    allowedcollapse = true
) => {
    const $table = $('#' + name_table);

    // --- CSS utilitario (una sola vez) ---
    if (!document.getElementById('bt-table-utils')) {
        const style = document.createElement('style');
        style.id = 'bt-table-utils';
        style.textContent = `
            .table-no-borders :is(th, td) {
                border: none !important;
            }
            /* Clase por si se usa selectionColor como nombre de clase */
            .bt-selected { background-color: #e7ebf6; }
        `;
        document.head.appendChild(style);
    }

    // Altura
    let height_table = 340;
    const parent = document.getElementById(name_parent_table);
    if (parent) height_table = parent.clientHeight - 5;

    // Clases de tabla
    const classes = ["table"];
    if (enableHover) classes.push("table-hover");
    if (class_add) classes.push(class_add);
    if (noBorders) classes.push("table-no-borders");

    // Columnas con checkbox opcional
    //const columnsConfig = showCheckbox ? [{ checkbox: true }, ...array_fiels_table_bootstra]
    //    : [...array_fiels_table_bootstra];

    // Helpers
    const applyRowColor = ($tr, color, selected) => {
        // color por HEX o por clase
        if (color && color.startsWith && color.startsWith("#")) {
            $tr.find("td").css("background-color", selected ? color : "");
        } else if (color) {
            $tr.toggleClass(color, selected);
        } else {
            $tr.toggleClass("bt-selected", selected);
        }
        $tr.attr("data-selected", selected ? "1" : "0");

        // Bordes redondeados (en celdas extrema izquierda/derecha)
        if (roundedSelected) {
            $tr.find("td").css("border-radius", ""); // reset
            if (selected) {
                $tr.find("td:first-child").css({
                    "border-top-left-radius": "10px",
                    "border-bottom-left-radius": "10px"
                });
                $tr.find("td:last-child").css({
                    "border-top-right-radius": "10px",
                    "border-bottom-right-radius": "10px"
                });
            }
        }
    };

    const clearAllSelections = (color) => {
        $table.find("tbody tr").each(function () {
            applyRowColor($(this), color, false);
            if (showCheckbox) {
                const index = $(this).attr("data-index");
                if (index != null) $table.bootstrapTable('uncheck', parseInt(index, 10));
            }
        });
    };

    // 👉 No des-selecciona si ya está seleccionado (según lo que pediste antes)
    const forceSelectRow = ($tr) => {
        const mode = $table.data("selectionMode");
        const color = $table.data("selectionColor");
        const isSelected = $tr.attr("data-selected") === "1";
        if (isSelected) return; // no quitar color ni cambiar estado

        if (mode === "single") clearAllSelections(color);

        applyRowColor($tr, color, true);

        // sincroniza checkbox si corresponde
        if (showCheckbox) {
            const index = $tr.attr("data-index");
            if (index != null) $table.bootstrapTable('check', parseInt(index, 10));
        }
    };

    // Init tabla
    $table.bootstrapTable('destroy').bootstrapTable({
        height: height_table,
        data: array_row_table_bootstra,
        classes: classes.join(' '),
        formatNoMatches: function () {
            return "";
        }
    });

    // Guardar config
    $table.data("selectionMode", selectionMode);
    $table.data("selectionColor", selectionColor);

    // --- Desactivar selección por clic general en fila ---
    // (Dejamos el evento pero no hacemos nada, para que la selección
    // solo ocurra por clic en elementos con clases permitidas)
    $table.off('click-row.bs.table').on('click-row.bs.table', function (e, row, $element) {
        // No togglear aquí
    });

    // --- Delegación de clic SOLO para clases permitidas ---
    // Construye un selector CSS: ".clase1, .clase2, .clase3"
    const allowedSelector = allowedClasses
        .filter(Boolean)
        .map(cls => '.' + cls.replace(/^\./, ''))  // por si pasan ".clase"
        .join(', ');

    // Limpia handlers previos para estas clases (en caso de reinicializar)
    if (allowedSelector.length) {
        $table.off('click.allowedSelect');
        $table.on('click.allowedSelect', allowedSelector, function (ev) {
            const $target = $(ev.target);

            // Evitar si es un dropdown (BS4 y BS5)
            if ($target.closest("[data-toggle='dropdown'], [data-bs-toggle='dropdown']").length) return;

            // Encontrar el TR correspondiente
            const $tr = $target.closest('tr');
            if (!$tr.length) return;

            // Forzar selección (no des-selecciona si ya estaba seleccionado)
            forceSelectRow($tr);

            // Evitar que otros manejadores (incluyendo click-row) actúen
            ev.stopPropagation();
        });
    }

    // Mantener sync con checkbox, sin cambiar color desde el check
    if (showCheckbox) {
        $table
            .off('check.bs.table uncheck.bs.table check-all.bs.table uncheck-all.bs.table')
            .on('check.bs.table uncheck.bs.table', function (e, row, $element) {
                // No tocar color aquí para obedecer "no colorear por check"
            })
            .on('check-all.bs.table uncheck-all.bs.table', function () {
                const color = $table.data("selectionColor");
                // Quitar color a todos en check-all/uncheck-all
                $table.find("tbody tr").each(function () {
                    applyRowColor($(this), color, false);
                });
            });
    }
    if (allowedcollapse == true) {
        document.querySelectorAll(`#${name_table} td`).forEach(td => {
            // Verifica si la celda no contiene un control (input, select, textarea, button)
            var cellText = td.innerHTML.trim();
            if (!td.querySelector('input, select, textarea, button, div')) {
                //td.style.cursor = 'help';
                td.setAttribute('title', cellText);
                td.style.maxWidth = '300px';
                td.style.textOverflow = 'ellipsis';
                td.style.overflow = 'hidden';
                td.style.whiteSpace = 'nowrap';

            }

        });
    }
};

const _init_row_feld_table_boostrap_table = (name_table, array_fiels_table_bootstra, array_row_table_bootstra, name_parent_table, class_delete,class_add) => {
    let $table = $('#' + name_table);
    let height_table = 340;
    if (document.getElementById(name_parent_table) ) {
        height_table = document.getElementById(name_parent_table).clientHeight -5 ;
    }
    let classes = [];
    classes.push("table");
    //classes.push("table-hover");
    if (class_delete == null) {
        classes.push("table-bordered");
    }
    if (class_add !== null) {
        classes.push(class_add);
    }
    if (document.getElementById(name_parent_table) != null) {
        height_table = document.getElementById(name_parent_table).clientHeight - 5;
    }
    $table.bootstrapTable('destroy').bootstrapTable({
        height: height_table,
        columns: array_fiels_table_bootstra,
        data: array_row_table_bootstra,
        classes: classes.join(' ')
    })
    
}
const _init_row_constant_table_boostrap_table = (name_table, array_row_table_bootstra, name_parent_table, class_delete, class_add,RowResult) => {
    let $table = $('#' + name_table);
    let height_table = 330;
    let classes = [];
    classes.push("table");
    //classes.push("table-hover");
    if (class_delete == null) {
        classes.push("table-bordered");
    }
    if (class_add !== null) {
        classes.push(class_add);
    }
    if (document.getElementById(name_parent_table) != null) {
        height_table = document.getElementById(name_parent_table).clientHeight - 5;
    }
    let RowResultRef = "";
    if (RowResult !== null) {
        RowResultRef = RowResult;
    }
    $table.bootstrapTable('destroy').bootstrapTable({
        height: height_table,
        data: array_row_table_bootstra,
        classes: classes.join(' '),
        formatNoMatches: function () {
            return RowResultRef;
        }
    }) 
}


const init_feld_table_boostrap_table = (name_table, array_fiels_table_bootstra, name_parent_table, class_delete, class_add, RowResult) => {
    let $table = $('#' + name_table);
    let height_table = 340;
    let classes = [];
    classes.push("table");
    //classes.push("table-hover");
    if (class_delete == null) {
        classes.push("table-bordered");
    } 
    if (class_add !== null) {
        classes.push(class_add);
    }
    if (document.getElementById(name_parent_table) != null) {
        height_table = document.getElementById(name_parent_table).clientHeight - 5;
    }
    let RowResultRef = "";
    if (RowResult !== null) {
        RowResultRef = RowResult;
    }
   $table.bootstrapTable('destroy').bootstrapTable({
        height: height_table,
       columns: array_fiels_table_bootstra,
       classes: classes.join(' '),
        formatNoMatches: function () {
           return RowResultRef;
       }
    })
   
}
const destroy_table_bootstrap_table = (name_table, name_parent_table, class_delete, class_add) => {
    let $table = $('#' + name_table);
    let height_table = 440;
    let classes = [];
    classes.push("table");
    //classes.push("table-hover");
    if (class_delete == null) {
        classes.push("table-bordered");
    }
    if (class_add !== null) {
        classes.push(class_add);
    }
    if (document.getElementById(name_parent_table) != null) {
        height_table = document.getElementById(name_parent_table).clientHeight - 5;
    }
    if (document.getElementById(name_parent_table) != null) {
        height_table = document.getElementById(name_parent_table).clientHeight - 5;
        $table.bootstrapTable('destroy').bootstrapTable({
            height: height_table,
            classes: classes.join(' ')
        })
    }
}
const table_reize_heigth = (name_table, heig_table, class_delete, class_add, allowedcollapse = false) => {
    let $table = $('#' + name_table);
    let classes = [];
    classes.push("table");
    //classes.push("table-hover");
    if (class_delete == null) {
        classes.push("table-bordered");
    }
    if (class_add !== null) {
        classes.push(class_add);
    }
    $table.bootstrapTable('refreshOptions', {
        height: heig_table,
        classes: classes.join(' ')
    })
    const applyTitlesToCells = () => {
        document.querySelectorAll(`#${name_table} td`).forEach(td => {
            var cellText = td.innerHTML.trim();
            if (!td.querySelector('input, select, textarea, button, div')) {
                td.setAttribute('title', cellText);
                td.style.maxWidth = '250px';
                td.style.textOverflow = 'ellipsis';
                td.style.overflow = 'hidden';
                td.style.whiteSpace = 'nowrap';
            }
        });
    };
    $table.on('sorted.bs.table', function () {
        applyTitlesToCells(); // Aplicar título y estilo a las celdas
    });
    if (allowedcollapse == true) {
        applyTitlesToCells();
    }
   
}

// --- Función adicional para aplicar títulos a las celdas ---
const _applyTitlesToCells = () => {
    document.querySelectorAll(`#table_consulta_gabinete td`).forEach(td => {
        var cellText = td.innerHTML.trim();
        if (!td.querySelector('input, select, textarea, button, div')) {
            td.setAttribute('title', cellText);
            td.style.maxWidth = '250px';
            td.style.textOverflow = 'ellipsis';
            td.style.overflow = 'hidden';
            td.style.whiteSpace = 'nowrap';
        }
    });
};
