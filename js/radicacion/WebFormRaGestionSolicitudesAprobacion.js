$(document).ready(function () {
    $.fn.inicio = function () {
        //****************************************VALIDACION RADICACION**********************************************************************************
        //FUNCION ACTIVA SELECCION CLIK EN EL DATAGREDVIEW DE VALIDACION RADICACION
        $('#GridViewlista tr[id]').click(function () {
            var fer = $(this).attr("id");
            $('#hdnEmailID_user').val(fer);

        });
        rezise_boton_usuario();
        auto_zise_popup_paginas_externas_libres();
        auto_zise_popup_usuarios_relacionados();
        auto_zise_popup_lista_solicitudes();
        $('#data_grid tr[id]').click(function () {
            $('#data_grid tr[id]').css({ "background": "White", "color": "Black" });
            $(this).css({ "background-color": " #e8e8f7", "color": "Black" });
            var fer = $(this).attr("id");
            $('#hdnEmailID_documentos').val(fer);
            //document.getElementById("Button_listar_usuarios_relacionados_solicitud").click();
        });
        $('#data_grid_documentos tr[id]').click(function () {
            $('#data_grid_documentos tr[id]').css({ "background": "White", "color": "Black" });
            $(this).css({ "background-color": " #e8e8f7", "color": "Black" });
            var fer = $(this).attr("id");
            $('#Hidden_id_usuarios_sel').val(fer);

        });
        $('#data_grid tr[id]').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });
        $('#data_grid_documentos tr[id]').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });
        //ASIGNA EL CURSOR DE SELECCION CUANDO PASA EL CURSOR EN EL DATAGREDVIEW DE VALIDACION RADICACION
        $('#GridViewlista tr[id]').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });
        if (document.getElementById("Hidden_result_detalle").value == "Segunda") {
            document.getElementById("Hidden_result_detalle").value = "inhabilitado";


        }
        if (document.getElementById("Hidden_resultado_gred").value == "YES") {
            document.getElementById("Hidden_resultado_gred").value = "";
            document.getElementById("Hidden_result_detalle").value = "Segunda";

        }


      

    }


});

$(window).on("load", function () {
    var elment = document.getElementsByClassName("da_event_captive");
    if (elment) {
        for (var i = 0; i < elment.length; i++) {
            elment[i].addEventListener("click", event_click, false);
        }
    }
    window.addEventListener("resize", rezize_event);
    toke_ini_solicitud('tokenize-callable-demo_respuesta___');
    toke_ini_solicitud_usuario('tokenize-callable-demo_respuesta____');
    $('.tokenize-callable-demo_respuesta___').on('tokenize:tokens:added', function (e, value, text) {
        ITEMS_DATOS_TOKENIZE_5.push({ text: text, value: value });
    });
    $('.tokenize-callable-demo_respuesta___').on('tokenize:tokens:remove', function (e, value) {
        delete_array_tokenize_solicitud(value);
    });
    $('.tokenize-callable-demo_respuesta____').on('tokenize:tokens:added', function (e, value, text) {
        ITEMS_DATOS_TOKENIZE_6.push({ text: text, value: value });
    });
    $('.tokenize-callable-demo_respuesta____').on('tokenize:tokens:remove', function (e, value) {
        delete_array_tokenize_solicitud_usuario(value);
    });
    ShowModalPopup("ModalPopupExtender_mensaje_personalizado_backgroundElement", "Panel_mensaje_personalizado", 100001);
    ShowModalPopup("ModalPopupExtender_registro_solicitud_usuario_backgroundElement", "Panel_registro_solicitud_usuario", 100001);
    ShowModalPopup("ModalPopupExtenderLibre_backgroundElement", "PanelLibre", 100001);
    ShowModalPopup("ModalPopupExtender_edition_lista_documentos_colaboracion_backgroundElement", "Panel_lista_documentos_colaboracion", 100001);
});
function rezize_event() {
    try {
        rezise_boton_usuario();
        auto_zise_popup_usuarios_relacionados();
        auto_zise_popup_paginas_externas_libres();
        auto_zise_popup_lista_solicitudes();
    } catch (ex) {
        alert(ex.message + " Función rezize_event")
    }
}
function event_click(e) {
    try {
        var k = e.currentTarget.value;
        var d = document.getElementById(k);
        d.click();
        e.preventDefault();
    }
    catch (err) {
        alert(err.message + " Funcion event_click");
    }
}
function ShowModalPopup(modalPopupId_, name_panel, zIndex) {
    try {
        var modalPopupId = document.getElementById(modalPopupId_);
        var name_panel_ = document.getElementById(name_panel);
        var modalPopupBehavior = modalPopupId;
        zIndex = typeof (zIndex) != 'undefined' ? zIndex : null;
        if (zIndex != null) {
            modalPopupBehavior.style.zIndex = zIndex;
            name_panel_.style.zIndex = zIndex + 1;
        }
    }
    catch (ex) {
        alert('Exception in ShowModalPopup: ' + ex.message);
    }
}
var ITEMS_DATOS_TOKENIZE_5 = new Array();  //GUARDA LOS ITEM SELECIONANDO EN TEX SELECTOR
var ITEMS_DATOS_TOKENIZE_6 = new Array();  //GUARDA LOS ITEM SELECIONANDO EN TEX SELECTOR
function toke_ini_solicitud(name_token) {
    try {
        
        $('.' + name_token).tokenize2({
            placeholder: "Para relacionar el usuario puede digitar el nombre del usuario o el cargo del usuario...",
            dataSource: function (search, object) {
                $.ajax('../webservice/WebServiceWorkflow.asmx/GetLista_usuarios_workflow_z2', {
                    data: "{'DName':'" + search + "'}",
                    dataType: 'json',
                    type: "POST",
                    traditional: true,
                    processData: false,
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        var $items = [];
                        $.each(data.d, function (k, v) {
                            $items.push(v);
                        });
                        object.trigger('tokenize:dropdown:fill', [$items]);

                    }
                });
            }

        });
    } catch (ex) { alert(ex.message + " funcion toke_ini_solicitud ") }
}
function toke_ini_solicitud_usuario(name_token) {
    try {
        
        $('.' + name_token).tokenize2({
            tokensMaxItems: 1,
            placeholder: "Para relacionar los usuarios puede digitar el nombre del usuario o el cargo del usuario...",
            dataSource: function (search, object) {
                $.ajax('../webservice/WebServiceWorkflow.asmx/GetLista_usuarios_workflow_z2', {
                    data: "{'DName':'" + search + "'}",
                    dataType: 'json',
                    type: "POST",
                    traditional: true,
                    processData: false,
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        var $items = [];
                        $.each(data.d, function (k, v) {
                            $items.push(v);
                        });
                        object.trigger('tokenize:dropdown:fill', [$items]);

                    }
                });
            }

        });
    } catch (ex) { alert(ex.message + " funcion toke_ini_solicitud ") }
}
function delete_array_tokenize_solicitud(value_id) {
    try {
        for (var i = 0; i < ITEMS_DATOS_TOKENIZE_5.length; i++) {
            if (ITEMS_DATOS_TOKENIZE_5[i].value == value_id) {
                ITEMS_DATOS_TOKENIZE_5.splice(i, 1);
                i = ITEMS_DATOS_TOKENIZE_5.length;
            }
        }
    } catch (err) {
        alert(err.message + " Funcion delete_array_tokenize_solicitud");
    }
}
function delete_array_tokenize_solicitud_usuario(value_id) {
    try {
        for (var i = 0; i < ITEMS_DATOS_TOKENIZE_6.length; i++) {
            if (ITEMS_DATOS_TOKENIZE_6[i].value == value_id) {
                ITEMS_DATOS_TOKENIZE_6.splice(i, 1);
                i = ITEMS_DATOS_TOKENIZE_6.length;
            }
        }
    } catch (err) {
        alert(err.message + " Funcion delete_array_tokenize_solicitud");
    }
}
function Solicitud_aprobacion_tokenize() {
    try {
        if (ITEMS_DATOS_TOKENIZE_5.length == 0) {
            alert("Debe selecionar los usuarios a compartir");
            return false;
        }
        var valParam = JSON.stringify(ITEMS_DATOS_TOKENIZE_5);
        var para_meter_ca = new Array();
        var asunto_ = "";
        var nota_ = document.getElementsByName('TextBox_nota_aprobacion');
        var nivel_urgencia_solicitud_ = document.getElementsByName('DropDownList_prioridad_solicitud');
        var tipo_solicitud_ = "";
        var fecha_limite_ = document.getElementsByName("TextBox_fecha_limite_solicitud");
        var radicado_relacionado_ = "";
        var id_usuario_propietario_ = document.getElementById("Hidden_id_respuesta").value;
        var matri_documentos_ = "";
        para_meter_ca.push({ asunto_: asunto_, nota_: nota_[0].value, nivel_urgencia_solicitud_: nivel_urgencia_solicitud_[0].value, tipo_solicitud_: tipo_solicitud_, radicado_relacionado_: radicado_relacionado_, id_usuario_propietario_: id_usuario_propietario_, matri_documentos_: matri_documentos_, fecha_limite_: fecha_limite_[0].value });
        var serialice = JSON.stringify(para_meter_ca);
        $.ajax('../webservice/WebServiceWorkflow.asmx/Set_Registra_solicitud_aprobacion', {
            data: "{'item_user':'" + valParam + "'," + "'parameter':'" + serialice + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d) {
                    var split = data.d.split("|");
                    if (split[0] !== "YES") {
                        alert(data.d);
                    } else {
                        insert_row_solicitud_aprbacion(data.d);
                        document.getElementById("Button_cancelar_registro").click();
                        
                    }
                }
            }
        });
    } catch (ex) { alert(ex.message + " funcion Solicitud_aprobacion_tokenize"); }
}
function insert_row_solicitud_aprbacion(date_campo) {
    try {
        var element_table = document.getElementById("data_grid");
        if (element_table) {
        } else {
            document.getElementById("Button_actualiza").click();
            return true;
        }
        var split = date_campo.split("|");
        var conta_td = 0;
        var element_row = element_table.insertRow(1);
        var element_td = element_row.insertCell(conta_td);
        element_row.id = split[1];
        element_row.style.cursor = "pointer";
        element_row.style.background = "white";
        element_row.style.color = "black";
        var divhtml = document.createElement("div");
        var ihtml = document.createElement("i");
        ihtml.style.color = "white";
        ihtml.classList.add("fal", "fa-archive",  "fa-lg");
        var ahtml = document.createElement("a");
        ahtml.classList.add("btn" , "btn-warning",  "btn-sm");
        ahtml.setAttribute("onclick", "prevent(event,this);")
        ahtml.setAttribute("title", "Anular solicitud de aprobación");
        ahtml.setAttribute("idd", split[1]);
        ahtml.setAttribute("tip_event", "anular_sol");
        ahtml.style.marginLeft = "3px";
        ahtml.appendChild(ihtml);
        divhtml.appendChild(ahtml);

        ihtml = document.createElement("i");
        ihtml.style.color = "white";
        ihtml.classList.add("fal" , "fa-envelope", "fa-lg");
        ahtml = document.createElement("a");
        ahtml.classList.add("btn", "btn-primary", "btn-sm");
        ahtml.setAttribute("onclick", "prevent(event,this);")
        ahtml.setAttribute("title", "Notificar al correo electrónico");
        ahtml.setAttribute("idd", split[1]);
        ahtml.setAttribute("tip_event", "noticor_sol");
        ahtml.style.marginLeft = "3px";
        ahtml.appendChild(ihtml);
        divhtml.appendChild(ahtml);

        ihtml = document.createElement("i");
        ihtml.style.color = "white";
        ihtml.classList.add("fal", "fa-sticky-note", "fa-lg");
        ahtml = document.createElement("a");
        ahtml.classList.add("btn", "btn-info", "btn-sm");
        ahtml.setAttribute("onclick", "prevent(event,this);")
        ahtml.setAttribute("title", "Ver notas de la solicitud");
        ahtml.setAttribute("idd", split[1]);
        ahtml.setAttribute("tip_event", "vernot_sol");
        ahtml.style.marginLeft = "3px";
        ahtml.appendChild(ihtml);
        divhtml.appendChild(ahtml);
      
        ihtml = document.createElement("i");
        ihtml.style.color = "white";
        ihtml.classList.add("fal", "fa-folder-open", "fa-lg");
        ahtml = document.createElement("a");
        ahtml.classList.add("btn" , "btn-primary", "btn-sm");
        ahtml.setAttribute("onclick", "prevent(event,this);")
        ahtml.setAttribute("title", "Ver los anexos de la solicitud");
        ahtml.setAttribute("idd", split[1]);
        ahtml.setAttribute("tip_event", "veranex_sol");
        ahtml.style.marginLeft = "3px";
        ahtml.appendChild(ihtml);
        divhtml.appendChild(ahtml);

        ihtml = document.createElement("i");
        ihtml.style.color = "white";
        ihtml.classList.add("fal", "fa-user-friends", "fa-lg");
        ahtml = document.createElement("a");
        ahtml.classList.add("btn",  "btn-success", "btn-sm");
        ahtml.setAttribute("onclick", "prevent(event,this);")
        ahtml.setAttribute("title", "Ver usuarios relacionados a la solicitud");
        ahtml.setAttribute("idd", split[1]);
        ahtml.setAttribute("tip_event", "ver_user_rel_sol");
        ahtml.style.marginLeft = "3px";
        ahtml.appendChild(ihtml);
        divhtml.appendChild(ahtml);
        element_td.appendChild(divhtml);
        for (var i = 1; i < split.length; i++) {
            conta_td++;
            element_td = element_row.insertCell(conta_td);
            element_td.innerHTML = split[i];
            element_td.classList.add("GridviewScrollItem_line_cort_tr_flex");
            element_td.setAttribute("onclick", "prevent_scrol(event,this,'')");

        }
        document.getElementById("titulo_label_expedientes").innerHTML = "Se encontraron " + (element_table.rows.length - 1) + " registro(s) ";
    } catch (err) {
        alert(err.message + " Funcion insert_row_solicitud_aprbacion");
    }
}
function Agrega_usuario_a_la_solicitud_aprobacion_() {
    try {
        if (ITEMS_DATOS_TOKENIZE_6.length == 0) {
            alert("Debe selecionar el usuario a compartir");
            return false;
        }
        var id_registro = document.getElementById("hdnEmailID_documentos").value;
        if (id_registro == "-1" || id_registro == "0") {
            alert("Debe seleccionar el registro de la solicitud de aprobación");
            return false;
        }
        var valParam = JSON.stringify(ITEMS_DATOS_TOKENIZE_6);
        var para_meter_ca = new Array();
        $.ajax('../webservice/WebServiceWorkflow.asmx/Set_Agrega_usuario_a_la_solicitud_aprobacion', {
            data: "{'item_user':'" + valParam + "'," + "'parameter':'" + id_registro + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d) {
                    var split = data.d.split("|");
                    if (split[0] !== "YES") {
                        alert(data.d);
                    } else {
                        insert_row_usuario_a_la_solicitud_aprobacion_(data.d);
                        document.getElementById("Button_cancela_registro_solictud").click();

                    }
                }
            }
        });
    } catch (ex) { alert(ex.message + " funcion Agrega_usuario_a_la_solicitud_aprobacion");}
}
function insert_row_usuario_a_la_solicitud_aprobacion_(date_campo) {
    try {
        var element_table = document.getElementById("data_grid_documentos");
        if (element_table) {
        } else {
            document.getElementById("Button_actualiza_registro_solicitud").click();
            return true;
        }
        var split = date_campo.split("|");
        var conta_td = 0;
        var element_row = element_table.insertRow(1);
        var element_td = element_row.insertCell(conta_td);
        //element_row.classList.add("GridviewScrollItem_line");
        element_row.id = split[1];
        element_row.style.cursor = "pointer";
        element_row.style.background = "white";
        element_row.style.color = "black";
        var divhtml = document.createElement("div");
        var ihtml = document.createElement("i");
        ihtml.style.color = "white";
        ihtml.classList.add("fal", "fa-sticky-note", "fa-lg");
        var ahtml = document.createElement("a");
        ahtml.classList.add("btn", "btn-info", "btn-sm");
        ahtml.setAttribute("onclick", "prevent(event,this);")
        ahtml.setAttribute("title", "Lista de notas de usuario");
        ahtml.setAttribute("idd", split[1]);
        ahtml.setAttribute("tip_event", "notas_usuario_solicitud");
        ahtml.style.marginLeft = "3px";
        ahtml.appendChild(ihtml);
        divhtml.appendChild(ahtml);

        ihtml = document.createElement("i");
        ihtml.style.color = "white";
        ihtml.classList.add("fal", "fa-folder-open", "fa-lg");
        ahtml = document.createElement("a");
        ahtml.classList.add("btn", "btn-primary", "btn-sm");
        ahtml.setAttribute("onclick", "prevent(event,this);")
        ahtml.setAttribute("title", "Lista anexos usuario");
        ahtml.setAttribute("idd", split[1]);
        ahtml.setAttribute("tip_event", "anexos_usuario_solicitud");
        ahtml.style.marginLeft = "3px";
        ahtml.appendChild(ihtml);
        divhtml.appendChild(ahtml);

        ihtml = document.createElement("i");
        ihtml.style.color = "white";
        ihtml.classList.add("fal", "fa-archive", "fa-lg");
        ahtml = document.createElement("a");
        ahtml.classList.add("btn", "btn-warning", "btn-sm");
        ahtml.setAttribute("onclick", "prevent(event,this);")
        ahtml.setAttribute("title", "Archiva solicitud usuario");
        ahtml.setAttribute("idd", split[1]);
        ahtml.setAttribute("tip_event", "archiva_usuario_solicitud");
        ahtml.style.marginLeft = "3px";
        ahtml.appendChild(ihtml);
        divhtml.appendChild(ahtml);

        ihtml = document.createElement("i");
        ihtml.style.color = "white";
        ihtml.classList.add("fal", "fa-envelope", "fa-lg");
        ahtml = document.createElement("a");
        ahtml.classList.add("btn", "btn-primary", "btn-sm");
        ahtml.setAttribute("onclick", "prevent(event,this);")
        ahtml.setAttribute("title", "Notifica solicitud usuario al correo electrónico");
        ahtml.setAttribute("idd", split[1]);
        ahtml.setAttribute("tip_event", "notifica_usuario_solicitud");
        ahtml.style.marginLeft = "3px";
        ahtml.appendChild(ihtml);
        divhtml.appendChild(ahtml);
        element_td.appendChild(divhtml);
        for (var i = 2; i < split.length; i++) {
            conta_td++;
            element_td = element_row.insertCell(conta_td);
            element_td.innerHTML = split[i];
            element_td.classList.add("GridviewScrollItem_line_cort_tr_flex");
            element_td.setAttribute("onclick", "prevent_scrol(event,this,'')");
        }
        document.getElementById("titulo_label_expedientes_documentos").innerHTML = "Se encontraron " + (element_table.rows.length - 1) + " registro(s) ";
    } catch (err) {
        alert(err.message + " Funcion insert_row_usuario_a_la_solicitud_aprobacion_");
    }
}
//modulo auto completar
function rezise_boton_usuario() {

    //$('#Button_seleccion_usuario').css("height", document.getElementById("TextBox_user_seleccionado").clientHeight + "px");

}
function dercaga_documento(elmen) {
    document.getElementById("Hidden_documento_descarga").value = elmen.id;
    document.getElementById("Button_descarga_documento").click();
}
function prevent(event, element) {
    try {
        
        var fer = $(element).attr("idd");     
        var tip_event = $(element).attr("tip_event");
        if (tip_event == "anular_sol") {
            $('#hdnEmailID_documentos').val(fer);
            document.getElementById("Button_activa_anulacion_solicitud").click();
        }
        if (tip_event == "noticor_sol") {
            $('#hdnEmailID_documentos').val(fer);
            document.getElementById("Button_envia_correo_notificacion").click();
        }
        if (tip_event == "vernot_sol") {
            $('#hdnEmailID_documentos').val(fer);
            document.getElementById("Button_activa_ver_nota_general").click();
        }
        if (tip_event == "veranex_sol") {
            $('#hdnEmailID_documentos').val(fer);
            document.getElementById("Button_todos_documentos_correccion").click();
        }
        if (tip_event == "ver_user_rel_sol") {
            $('#hdnEmailID_documentos').val(fer);
            document.getElementById("Button_listar_usuarios_relacionados_solicitud").click();
        }
        if (tip_event == "notas_usuario_solicitud") {
            $('#Hidden_id_usuarios_sel').val(fer);
            document.getElementById("Button_estado_solicitud").click();
        }
        if (tip_event == "anexos_usuario_solicitud") {
            $('#Hidden_id_usuarios_sel').val(fer);
            document.getElementById("Button_documentos_correccion").click();
        }
        if (tip_event == "archiva_usuario_solicitud") {
            $('#Hidden_id_usuarios_sel').val(fer);
            document.getElementById("Button_archiva_solicitud").click();
        }
        if (tip_event == "notifica_usuario_solicitud") {
            $('#Hidden_id_usuarios_sel').val(fer);
            document.getElementById("Button_notifica_solicitud_usuario_correo").click();
            
        }
        event.preventDefault();
       
    }
    catch (err) {
        alert(err.message + " Funcion prevent");
    }
}
function prevent_scrol(event, e) {
    try {

        if (e.className == "GridviewScrollItem_line_cort_tr_flex") {
            e.classList.remove("GridviewScrollItem_line_cort_tr_flex");
            e.classList.toggle("GridviewScrollItem_line_corte_tr_flex_scrol");
        } else {
            e.classList.remove("GridviewScrollItem_line_corte_tr_flex_scrol");
            e.classList.toggle("GridviewScrollItem_line_cort_tr_flex");
        }
        event.preventDefault();
    }
    catch (err) {
        alert(err.message + " Funcion prevent_scrol");
    }
}

function service_usuarios_gestion(name_texbox) {
    function split(val) {
        return val.split(/,\s*/);
    }
    function extractLast(term) {
        return split(term).pop();
    }
    $("#" + name_texbox)
        .on("keydown", function (event) {
            if (event.keyCode === $.ui.keyCode.TAB &&
                $(this).autocomplete("instance").menu.active) {
                event.preventDefault();
            }
        })
        .autocomplete({
            source: function (request, response) {
                var param = { keyword: $('#' + name_texbox).val() };
                $.ajax({
                    url: "../webservice/WebServiceRadicacion.asmx/GetLista_usuarios_gestion",
                    data: "{'DName':'" + document.getElementById(name_texbox).value + "'}",
                    dataType: "json",
                    type: "POST",
                    traditional: true,
                    processData: false,
                    contentType: "application/json; charset=utf-8",
                    //dataFilter: function (data) { return data; },
                    success: function (data) {
                        term: extractLast(request.term)
                        response($.ui.autocomplete.filter(
               data.d, extractLast(request.term)));

                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert(textStatus);
                    }
                });
            },


            focus: function () {
                // prevent value inserted on focus
                return false;
            },
            select: function (event, ui) {
                var terms = split(this.value);
                // remove the current input
                terms.pop();
                // add the selected item
                terms.push(ui.item.value);
                // add placeholder to get the comma-and-space at the end
                terms.push("");
                this.value = terms.join(", ");
                return false;
            }

            , minLength: 3, max: 10, scroll: true
        });
}
//DESACTIVA CHEK
function desactiva_chek() {
    try {
        var x = document.getElementsByClassName("dummychkstyle");
        for (i = 0; i < x.length; i++) {
            var z = x[i];
            if (z !== null) {
                z.checked = false;
            }

        }
    }
    catch (err) {
        alert(err.message + " funcion desactiva_chek " + err.message);
    }
}
function asigna_usuario_grupos_cheked() {
    try {
        var fer = "0";
        $('#hdnEmailID_sel').val("0");
        var correos = document.getElementById("Hidden_correos_electronico");
        $('#Hidden_correos_electronico').val("");
        correos = "";
        $('#GridViewlista .dummychkstyle').each(function () {
            if (this.checked == true) {
                var cel = $(this).parent().parent().parent();
                var atri = $(this).parent().parent().parent().attr("id");
                if (atri == undefined) {
                    atri = $(this).parent().parent().attr("id");
                    cel = $(this).parent().parent();
                }

                if (atri !== undefined && cel[0].display !== "none") {
                    if (fer == "0") {
                        fer = atri;
                    } else {
                        fer = fer + "." + atri;
                    }
                }
                $("#GridViewlista tr[id=" + atri + "]").each(function () {
                var idex = -1;
                idex = colum_index('CORREO_ELECTRONICO', 'GridViewlista')     
                if (idex != -1 && $(this)[0].cells[idex].innerText !== "") {
                    if ($('#Hidden_correos_electronico').val() == "") {
                        $('#Hidden_correos_electronico').val($(this)[0].cells[idex].innerText);
                    } else {
                        $('#Hidden_correos_electronico').val($('#Hidden_correos_electronico').val() + "," + $(this)[0].cells[idex].innerText);
                       
                    }
                }
                })
            }

            //$('#Hiddenseltareas', window.parent.document).val(fer)
        });
        $('#hdnEmailID_sel').val(fer);
    }
    catch (err) {
        alert(err.message + " funcion asigna_usuario_grupos_cheked " + err.message);
    }
}
//FUNCION ACTIVA Y DEACTIVA LOS CAMPOS CHEKEADOS EN UNA TABLA
function desactiva_ch_data_grid(idente_chekbi_actyive) {
    try {
        var e = $("#" + idente_chekbi_actyive);
        if ($(e).is(':checked')) {
            var x = document.getElementsByClassName("dummychkstyle");
            for (i = 0; i < x.length; i++) {
                var z = x[i];
                if (z !== null) {
                    var parnt = z.parentNode.parentNode.parentNode;
                    if (z.parentNode.parentNode.parentNode.hasAttribute("id") == false) {
                        parnt = z.parentNode.parentNode;
                    }
                    if (parnt.style.display != "none") {
                        z.checked = false;
                    }
                }
            }
        }
        else {

            var x = document.getElementsByClassName("dummychkstyle");
            for (i = 0; i < x.length; i++) {
                var z = x[i];
                if (z !== null) {
                    var parnt = z.parentNode.parentNode.parentNode;
                    if (z.parentNode.parentNode.parentNode.hasAttribute("id") == false) {
                        parnt = z.parentNode.parentNode;
                    }
                    if (parnt.style.display != "none") {
                        z.checked = true;
                    }
                }
            }
        }
    }
    catch (err) {
        alert(err.message + " funcion desactiva_ch_data_grid " + err.message);
    }
}
function auto_zise_popup_lista_usuarios() {
    try {
        var espacio_iframe = 420;
        var hidenpadre = 0;
        var with_frame = 420;
        if (window.innerHeight) {
            //navegadores basados en mozilla 
            espacio_iframe = window.innerHeight;
            with_frame = window.innerWidth;
        } else {
            if (document.body.clientHeight) {
                //Navegadores basados en IExplorer, es que no tengo innerheight 
                espacio_iframe = document.body.clientHeight;
                with_frame = document.body.clientWidth;
            } else {
                //otros navegadores y iframe
                //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val();

            }
        }


        $(document).ready(bodyResize);
        $(window).resize(bodyResize);
        function bodyResize() {
         
        }
        $('#Panel_lista_usuarios_solicitud').css("height", (espacio_iframe - 35) + "px");
        $('#contenido_label').css("height", (document.getElementById("Buttonbuscar").clientHeight + 5) + "px");
        $('#contenido_botonoes').css("height", (document.getElementById("Buttonbuscar").clientHeight + 5) + "px");
        var total = document.getElementById("contenido_label").clientHeight + document.getElementById("contenido_botonoes").clientHeight;
        $('#Lista').css("height", ((espacio_iframe - 35) - (total + 10)) + "px");
        var gridwith = document.getElementById("Lista").clientWidth;
        var gridheihg = document.getElementById("Lista").clientHeight - 3;
        //LLAMA PLUGIN FIJA HIDER O TITULOS   
        if ($('#GridViewlista td').children.length > 0 && $('#GridViewlista tr:visible').length > 0) {
            $(document).ready(function () { $('#GridViewlista').gridviewScroll({ width: gridwith, height: (gridheihg) }); })
        }

    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_pendinetes " + err.message);
    }
}
function auto_zise_popup_paginas_externas_libres() {
    try {
        var espacio_iframe = 420;
        var hidenpadre = 0;
        var with_frame = 420;
        if (window.innerHeight) {
            //navegadores basados en mozilla 
            espacio_iframe = window.innerHeight;
            with_frame = window.innerWidth;
        } else {
            if (document.body.clientHeight) {
                //Navegadores basados en IExplorer, es que no tengo innerheight 
                espacio_iframe = document.body.clientHeight;
                with_frame = document.body.clientWidth;
            } else {
                //otros navegadores y iframe
                //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val();  

            }
        }
        $('#PanelLibre').css("height", (espacio_iframe - 20) + "px");
        $('#conten_iframe').css("height", ((espacio_iframe - 20) - document.getElementById("title_bot").clientHeight) + "px");
        $('#Iframelibre_notas_general_').css("height", ((espacio_iframe - 40 ) - document.getElementById("title_bot").clientHeight) + "px");
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_paginas_externas_libres");
    }

}
function auto_zise_popup_lista_solicitudes() {
    try {
        var espacio_iframe = 420;
        var hidenpadre = 0;
        var with_frame = 420;
        if (window.innerHeight) {
            //navegadores basados en mozilla 
            espacio_iframe = window.innerHeight;
            with_frame = window.innerWidth;
        } else {
            if (document.body.clientHeight) {
                //Navegadores basados en IExplorer, es que no tengo innerheight 
                espacio_iframe = document.body.clientHeight;
                with_frame = document.body.clientWidth;
            } else {
                //otros navegadores y iframe
                //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val();

            }
        }
        $('#div_contenedor_drecho').css("height", (espacio_iframe - 20 ) + "px");
        $('#div_unidades_documentales').css("height", (document.getElementById("Button_activa_registro_solicitud").clientHeight ) + "px");
        $('#contenedor_opciones_solictitud_general').css("height", (document.getElementById("Button_activa_registro_solicitud").clientHeight + 20) + "px");
        var gridheihg = (espacio_iframe - 20) - (document.getElementById("div_unidades_documentales").clientHeight + document.getElementById("contenedor_opciones_solictitud_general").clientHeight);
        $('#Contenedorgrid').css("height", (gridheihg) + "px");
        $('#contenido_titulo_val_radicacion').css("height", (document.getElementById("Button_activa_registro_solicitud").clientHeight) + "px");
        $('#Panel_principal').css("height", (gridheihg - (document.getElementById("contenido_titulo_val_radicacion").clientHeight + 50)) + "px");
        
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_lista_solicitudes " + err.message);
    }
}
function auto_zise_popup_usuarios_relacionados() {
    try {
        var espacio_iframe = 420;
        var hidenpadre = 0;
        var with_frame = 420;
        if (window.innerHeight) {
            //navegadores basados en mozilla 
            espacio_iframe = window.innerHeight;
            with_frame = window.innerWidth;
        } else {
            if (document.body.clientHeight) {
                //Navegadores basados en IExplorer, es que no tengo innerheight 
                espacio_iframe = document.body.clientHeight;
                with_frame = document.body.clientWidth;
            } else {
                //otros navegadores y iframe
                //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val();

            }
        }
        $("#div_expediente_seleccionado").css("height", (document.getElementById("Button_nuevo_integrante").clientHeight + 20) + "px");
        $("#contenido_procesa_usu_rel_solicitud").css("height", (document.getElementById("Panel_usu_rel_solicitud").clientHeight - (document.getElementById("div_expediente_seleccionado").clientHeight + document.getElementById("div_expediente_seleccionado").clientHeight)) + "px");
        $('#content_data_grid').css("height", (document.getElementById("contenido_procesa_usu_rel_solicitud").clientHeight - (document.getElementById("div_contenedor_titulo_documentos_relacionados").clientHeight + document.getElementById("contenido_titulo_val_radicacion_documentos").clientHeight)) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_usuarios_relacionados " + err.message);
    }
}
function activa_boton_dowload() {
    try {

        document.getElementById("Button_guardar_desicion").click();
    }
    catch (err) {
        alert(err.message + " funcion activa_boton_dowload " + err.message);
    }
}
//MUEVE EL SCCROL AL ID SELECCIONADO
function mueve_scroll_data_gred(data_grid, HiddenSeleccion) {
    try {
    if ($("#" + HiddenSeleccion).val() != "-1" || $("#" + HiddenSeleccion).val() != "0") {
        var scrollableDiv = $("#" + data_grid).parent();
        //limpia todos los seleccionados
        $("#" + data_grid + " tr[id]").css({ "background-color": "transparent", "color": "Black" });
        $("#" + data_grid + " tr[id=" + $("#" + HiddenSeleccion).val() + "]").css({ "background-color": "LightSkyBlue", "color": "Red" });
        $("#" + data_grid + " tr[id= " + $("#" + HiddenSeleccion).val() + "]").each(function () {
            $(scrollableDiv).scrollTop(70);
            $(scrollableDiv).scrollTop(($(this).offset().top));
            return true;
        });
    }
}
    catch (err) {
        alert(err.message + " funcion mueve_scroll_data_gred " + err.message);
}
}
function activa_busqueda() {
    try {


        busqueda_gred('HiddenSeleccion', 'GridViewlista', 'contenidobusqueda', 'CheckboxBusqueda');

    }
    catch (err) {
        alert(err.message + " funcion activa_busqueda " + err.message);
    }
}
function busqueda_gred(HiddenSeleccion, data_grid, contenido_busqueda, CheckboxBusqueda) {
    try {
        if ($("#" + contenido_busqueda).val() == "") {
            $("#" + data_grid + " tr[id]").css({ "background-color": "transparent", "color": "Black" });
            $("#" + HiddenSeleccion).val("-1");
            return false;
        }
        $("#" + HiddenSeleccion).val("-1");
        var refgrid;
        var filtro;
        $("#" + data_grid + " tr[id]").css({ "background-color": "transparent", "color": "Black" });
        var s = $("#" + contenido_busqueda).val().toLowerCase();
        var grid = $("#" + data_grid);
        var confirma_ok = "0";
        var cel_indes = 0;
        $("#" + data_grid + " tr:has(td)").each(function () {
            cel_indes = cel_indes + 1;
            var rowtd = $(this);
            var scrollableDiv = grid.parent();
            $(this).children("td").each(function (idex) {
                var tempotd = $(this).text().toLowerCase()
                var check = document.getElementById(CheckboxBusqueda).checked;
                if (check == true) {
                    if (idex >= 0) {
                        if (s == tempotd) {
                            $(this).parent().css({ "background-color": "LightSkyBlue", "color": "green" });
                            //var id_ref = $(this).parent();
                            //confirma_ok = $(id_ref).offset().top;
                            var id_ref = $(this).parent();
                            if (cel_indes == 2) {
                                $(scrollableDiv).scrollTop(($(id_ref).offset().top - id_ref[0].offsetHeight));
                            }
                            if (cel_indes !== 2) {
                                $(scrollableDiv).scrollTop(rowtd[0].offsetTop - id_ref[0].offsetHeight);
                            }


                        }
                    }
                }

                if (check == false) {
                    if (idex >= 0) {
                        var compare = tempotd;
                        var strcompre = compare.indexOf(s);
                        if (strcompre >= 0) {
                            $(this).parent().css({ "background-color": "LightSkyBlue", "color": "green" });
                            //var id_ref = $(this).parent();
                            //confirma_ok = $(id_ref).offset().top;
                            var id_ref = $(this).parent();
                            if (cel_indes == 2) {
                                $(scrollableDiv).scrollTop(($(id_ref).offset().top - id_ref[0].offsetHeight));
                            }
                            if (cel_indes !== 2) {
                                $(scrollableDiv).scrollTop(rowtd[0].offsetTop - id_ref[0].offsetHeight);
                            }


                        }
                    }
                }


            })

        });

    }
    catch (err) {
        alert(err.message + " funcion busqueda_gred " + err.message);
    }
}
function progres_hiden(progres) {
    $("#progres_bar").css("display", "none");
}
function ConfirmMensajeGeneral(mensaje, name_hiden) {
    try {
        var element_hiden = document.getElementById(name_hiden)
        if (element_hiden === null) {
            alert("Imposible encontrar el control " + name_hiden);
            return false;
        }
        var x = "";
        var r = confirm(mensaje);
        if (r == true) {
            x = "1";
        }
        else {
            x = "0";
        }
        document.getElementById(name_hiden).value = x;
    }
    catch (err) {
        alert(err.message + " ConfirmMensajeGeneral");
    }
}
function posicion_update_pogres(progres) {
    var espacio_iframe = 420;
    var hidenpadre = 0;
    var with_frame = 420;
    if (window.innerHeight) {
        //navegadores basados en mozilla 
        espacio_iframe = window.innerHeight;
        with_frame = window.innerWidth;
    } else {
        if (document.body.clientHeight) {
            //Navegadores basados en IExplorer, es que no tengo innerheight 
            espacio_iframe = document.body.clientHeight;
            with_frame = document.body.clientWidth;
        } else {
            //otros navegadores y iframe
            //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val();

        }
    }
    var prog = document.getElementById(progres);
    var widtop = (espacio_iframe / 2);
    var heitop = (with_frame / 2);
    prog.style.top = widtop + "px";
    prog.style.left = heitop + "px";
    prog.style.zIndex = "1000009";
    $("#progres_bar").css("display", "block");
    prog.style.position = "fixed";


}
function ejecuta_boton_formulario_padre(nombre_buton) {
    try {

        var bto = window.parent.document.getElementById(nombre_buton);
        if (bto !== null) {

            bto.click();
        }
    }
    catch (err) {
        alert(err.message + " funcion ejecuta_boton_formulario_padre " + err.message);
    }
}
function actualiza_hiden_formulario_padre(nombre_hiden) {
    try {

        var hiden = window.parent.document.getElementById(nombre_hiden);
        if (hiden) {
            hiden.value = "YES";
        }
    }
    catch (err) {
        alert(err.message + " funcion ejecuta_boton_formulario_padre " + err.message);
    }
}
function colum_index(colum_name,nombre_grid) {
    try {
        var x = $('#' + nombre_grid + ' th');
    var txt = "";
    var i;
    for (i = 0; i < x.length; i++) {
        if (x[i].innerText.toUpperCase() == colum_name.toUpperCase()) {

            return i;
        }

    }
    return -1;
}
    catch (err) {
        alert(err.message + " funcion colum_index " + err.message);
}
}
function actualiza_gre_campo(nombre_grid,id,valor_campo,nombre_campo) {
    try {   
        $("#" + nombre_grid + " tr[id=" + id + "]").each(function () {
            var idex = -1;
            var name = nombre_campo;  
            idex = colum_index(name, nombre_grid);
                    if (idex != -1) {
                        if (valor_campo == "") {
                            var sas = $(this)[0].cells[idex];
                            //var nodetext = document.getElementById("ocultop");
                            var trfirst = $('#' + nombre_grid + ' tr:first').next();
                            if (sas.childElementCount == 0) {
                                $(this)[0].cells[idex].innerText = "\u00a0";
                                //nodetext.innerText = "\u00a0";
                            }
                            if (sas.childElementCount >= 1) {
                                sas.firstChild.innerHTML = "&nbsp;";
                                //nodetext.innerText = "\u00a0";
                            }
                        }
                        if (valor_campo !== "") {
                            var trfirst = $('#' + nombre_grid + ' tr:first').next();
                            var sas = $(this)[0].cells[idex];
                            if (sas.childElementCount <= 0 && sas.firstChild ) {
                                var clinet_widt_old = sas.firstChild.clientWidth;
                                var div_element = document.createElement("div");
                                var p_element = document.createElement("p");
                                p_element.innerHTML = valor_campo;
                                div_element.appendChild(p_element);
                                $(this)[0].appendChild(div_element);
                                $(this)[0].cells[idex].innerText = valor_campo;
                                var clinet_widt_new = p_element.clientWidth;
                                //verifcar que la fila uno tenga childs
                                if (($(this)[0].cells[idex].clientWidth - 10) > trfirst[0].cells[idex].firstChild.clientWidth) {
                                    trfirst[0].cells[idex].firstChild.style.width = $(this)[0].cells[idex].clientWidth + "px";
                                    var x2 = $('#'+ nombre_grid + 'Copy th');
                                    x2[idex].firstChild.style.width = ($(this)[0].cells[idex].clientWidth - 10) + "px";
                                    x2[idex].clientWidth = ($(this)[0].cells[idex].clientWidth - 10);
                                }
                                $(this)[0].removeChild(div_element);
                                return false;
                                
                            }
                            //Opcion para actualizar la primera fila de la tabla que se le agrega un div, cuado trae mas de un elemento
                            if (sas.childElementCount >= 1 && sas.firstChild) {
                                var clinet_widt_old = sas.firstChild.clientWidth;
                                var div_element = document.createElement("div");
                                var p_element = document.createElement("p");
                                p_element.innerHTML = valor_campo;
                                div_element.appendChild(p_element);
                                sas.firstChild.innerHTML = valor_campo;
                                sas.appendChild(div_element);
                                
                                var clinet_widt_new = p_element.clientWidth;
                                if (clinet_widt_new > trfirst[0].cells[idex].firstChild.clientWidth) {
                                   
                                    if (trfirst[0].cells[idex].firstChild.childElementCount > 0) {
                                        trfirst[0].cells[idex].firstChild[0].style.width = clinet_widt_new + "px";
                                    }
                                    else {
                                        trfirst[0].cells[idex].firstChild.style.width = clinet_widt_new + "px";
                                    }
                                   
                                    var x2 = $('#' + +nombre_grid + 'Copy th');
                                    //if (x2[idex].firstChild) {
                                        //x2[idex].firstChild.style.width = clinet_widt_new + "px";
                                        //x2[idex].clientWidth = clinet_widt_new;
                                    //}
                                   
                                }
                                sas.removeChild(div_element);
                                return false;
                                
                            }

                        }

                    }
                
            
        })
        return true;
    }
    catch (err) {
        alert(err.message + " Funcion actualiza_gre_campo");
    }
}
$(document).on('keydown', function (e) {
    if (e.which == 9) {
        var id_element = e.srcElement.id;
        var matr_id = id_element.split("-");
        if (matr_id.length < 2) {
            return;
        }
        var salidadato;
        if (matr_id[2] == "DATE" || matr_id[2] == "DATE_2") {
            var dato = e.srcElement.value;


            if (dato == "") {

                return false;
            }


            if (salidadato == "Formato fecha no cumple") {
                alert(salidadato);
                e.preventDefault();
                return false;
            }
            var BisestA;
            var Año_F, Mes_f, Dia_f, tip;
            var numerocaracter = dato.length;
            if (numerocaracter == 10 || numerocaracter == 8) {

            }
            else {
                alert("Formato fecha no cumple");
                e.preventDefault();
                return false;
            }

            if (numerocaracter == 10) {

                Año_F = dato.substring(0, 4);
                Mes_f = dato.substring(0, 7);
                Mes_f = Mes_f.substring(7, 5);
                Dia_f = dato.substring(8, 10);
            }
            else {
                Año_F = dato.substring(0, 4);
                Mes_f = dato.substring(0, 6);
                Mes_f = Mes_f.substring(6, 4);
                Dia_f = dato.substring(6, 8);
            }

            //Verifica el formato del dia
            if (Dia_f > 31 || Dia_f == 0) {

                alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                e.preventDefault();
                return false;
            }

            //verifica el formato del mes
            if (Mes_f > 12 || Mes_f < 1) {
                alert("EM_" + Año_F + "(" + Mes_f + ")" + Dia_f);
                e.preventDefault();
                return false;
            }

            switch (Mes_f) {
                case "01":
                    if (Dia_f > 31) {
                        alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                        e.preventDefault();
                    }
                    break;

                case "02":
                    if (Dia_f % 4 == 0) {

                        BisestA = 29;
                    }
                    else {
                        BisestA = 28;
                    }
                    if (Dia_f > BisestA) {
                        alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                        e.preventDefault();
                    }
                    break;
                case "03":
                    if (Dia_f > 31) {
                        alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                        e.preventDefault();
                    }
                    break;

                case "04":
                    if (Dia_f > 30) {
                        alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                        e.preventDefault();
                    }
                    break;

                case "05":
                    if (Dia_f > 31) {
                        alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                        e.preventDefault();
                    }
                    break;

                case "06":
                    if (Dia_f > 30) {
                        alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                        e.preventDefault();
                    }
                    break;

                case "07":
                    if (Dia_f > 31) {
                        alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                        e.preventDefault();
                    }
                    break;

                case "08":
                    if (Dia_f > 31) {
                        alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                        e.preventDefault();
                    }
                    break;

                case "09":
                    if (Dia_f > 30) {
                        alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                        e.preventDefault();
                    }
                    break;

                case "10":
                    if (Dia_f > 31) {
                        alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                        e.preventDefault();
                    }
                    break;

                case "11":
                    if (Dia_f > 30) {
                        alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                        e.preventDefault();
                    }
                    break;

                case "12":
                    if (Dia_f > 31) {
                        alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                        e.preventDefault();
                    }
                    break;
            }

            if (numerocaracter == 8) {
                salidadato = Año_F + "/" + Mes_f + "/" + Dia_f;
                e.srcElement.value = salidadato;
            }

            if (numerocaracter == 10) {
                salidadato = Año_F + "/" + Mes_f + "/" + Dia_f;
                e.srcElement.value = salidadato;
            }

        }
    }
});