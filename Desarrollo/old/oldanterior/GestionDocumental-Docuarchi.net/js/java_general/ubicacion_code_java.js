function event_add_municipio(name_campo) {
    try {
        const newLocal = document.getElementById(name_campo);
        if (document.getElementById(name_campo)) {
            var value_drow = $("#" + name_campo).val();
            $("#municipio").empty();
            service_source_list_item(value_drow, "service_solicita_lista_municipio", "municipio");
        }
    } catch (ex) {
        alert("Error event_add_municipio " + ex.message);
    }
}
function event_add_departanmento(name_campo) {
    try {
        const newLocal = document.getElementById(name_campo);
        if (document.getElementById(name_campo)) {
            var value_drow = $("#" + name_campo).val();
            $("#departamento").empty();
            $("#municipio").empty();
            service_source_list_item(value_drow, "service_solicita_lista_departamentos", "departamento");
        }
    } catch (ex) {
        alert("Error event_add_municipio " + ex.message);
    }
}
function service_source_list_item(id_, name_service, name_control) {
    try {
        $.ajax('../webservice/WebServiceRadicacion.asmx/' + name_service, {
            data: "{'id':" + "'" + id_ + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_sistema !== "YES") {
                    alert(data.d[0].error_sistema);

                } else {
                    ITEMS_DATOS_DROW = new Array();
                    $.each(data.d[0].item_sistema, function (k, v) {
                        ITEMS_DATOS_DROW.push(v);
                    });
                    if (document.getElementById(name_control)) {
                        var element_drow = document.getElementById(name_control);
                        for (var i = 0; i < ITEMS_DATOS_DROW.length; i++) {
                            element_drow[i] = new Option(ITEMS_DATOS_DROW[i].text, ITEMS_DATOS_DROW[i].value);
                        }
                    }
                }
            }, error: function (xception, textStatus, errorThrown) {

                if (xception.status === 0) {

                    alert('Not connect: Verify Network.');

                } else if (xception.status == 404) {

                    alert('Requested page not found [404]');

                } else if (xception.status == 500) {

                    alert('Internal Server Error [500].' + xception.responseText);

                } else if (textStatus === 'parsererror') {

                    alert('Requested JSON parse failed.');

                } else if (textStatus === 'timeout') {

                    alert('Time out error.');

                } else if (textStatus === 'abort') {

                    alert('Ajax request aborted.');

                } else {

                    alert('Uncaught Error: ' + xception.responseText);

                }
            }
        });
    }
    catch (ex) {

        alert('service_source_list_departamento ' + ex.message);
    }
}