$(document).ready(function () {
    $.fn.inicio = function () {

        $('#data_grid_listado_solicitudes tr[id]').click(function () {
            $('#data_grid_listado_solicitudes tr[id]').css({ "background": "White" });
            $(this).css({ "background-color": "#e8e8f7" });
            var fer = $(this).attr("id");
            $('#hdnEmailID').val(fer);
            $('#Hidden_solicitud_compartido').val(fer);
        });

        $('#data_grid_listado_solicitudes tr[id]').dblclick(function () {
            var fer = $(this).attr("id");
            $('#Hidden_solicitud_compartido').val(fer);
            $(this).css({ "font-weight": "100" });
            document.getElementById("Button_ver_documentos_relacionados").click();
           
        });
        $('#data_grid_listado_solicitudes tr[id]').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });
        if (document.getElementById("Hidden_control_lista").value == "Segunda") {
            document.getElementById("Hidden_control_lista").value = "inhabilitado";
            auto_zise_popup_lista_solicitudes("0", "1");

        }
        if (document.getElementById("Hidden_control_lista").value == "") {
            document.getElementById("Hidden_control_lista").value = "Segunda";
            auto_zise_popup_lista_solicitudes("1", "1");
        }
       
        auto_zise_popup_lista_solicitudes("0", "1");
        auto_zise_popup_compartir_documento();
        auto_zise_popup_registro_colaboracion();
        auto_zise_popup_usuarios_relacionados();
        
    }

    $('#Contenedorgrid_listado_solicitud').contextMenu('context-menu-1', {

        'Salir del menú': {
            click: function (element) { },
            klass: "fad fa-times"
        },
        'Ver documentos relacionados': {
            click: function (element) {
                document.getElementById("Button_ver_documentos_relacionados").click();
            },
            klass: "fal fa-folder-open"
        },
        'Eliminar registro': {
            click: function (element) {
                document.getElementById("Button_eliminar_registro").click();
            },
            klass: "fal fa-times"
        }
    });
})
$(window).on("load", function () {
    try {
        var elment = document.getElementsByClassName("da_event_captive");
        if (elment) {
            for (var i = 0; i < elment.length; i++) {
                elment[i].addEventListener("click", event_click, false);
            }
        }
        window.addEventListener("resize", rezize_event);
        ShowModalPopup("ModalPopupExtender_mensaje_personalizado_backgroundElement", "Panel_mensaje_personalizado", 100001);
        GetLista_documentos_compartidos_otros_usuarios('TextBox_busqueda');
    } catch (e) {
        alert(" funcion load " + e.message);
    }

});
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
function rezize_event() {
    try {
        auto_zise_popup_lista_solicitudes("0", "1");
        auto_zise_popup_compartir_documento();
        auto_zise_popup_registro_colaboracion();
        auto_zise_popup_usuarios_relacionados();
    } catch (ex) {
        alert(ex.message + " Función rezize_event")
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
function GetLista_documentos_compartidos_otros_usuarios(name_texbox) {
    function extractLast(term) {
        return term;
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
                    url: "../webservice/WebServiceRadicacion.asmx/GetLista_documentos_compartidos_otros_usuarios",
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
            }, minLength: 3, max: 10, scroll: true
        });
}
function preven_event_search(event, e) {
    try {
        document.getElementById("ImageButton_buscar").click();
        event.preventDefault();
    }
    catch (err) {
        alert(err.message + " Funcion preven_event_search");
    }
}
function preven_event_restor_search(event, e) {
    try {
        document.getElementById("Button_link_service_actualiza").click();
        event.preventDefault();
    }
    catch (err) {
        alert(err.message + " Funcion preven_event_restor_search");
    }
}
function preven_event_search_keypres_enter(e, sender) {
    try {

        tecla = (document.all) ? e.keyCode : e.which;
        if (tecla == 13) {
            document.getElementById("ImageButton_buscar").click();
            e.preventDefault();            
        }

    } catch (err) {
        alert(err.message + " funcion preven_event_search_keypres_enter " + err.message);
    }
}
function prevent(event, element) {
    try {

        var fer = $(element).attr("idd");
        var tip_event = $(element).attr("tip_event");
        if (tip_event == "ver_doc_comp") {
            $('#hdnEmailID').val(fer);
            document.getElementById("Button_ver_documentos_relacionados").click();
        }
        if (tip_event == "elimina_doc_comp") {
            $('#hdnEmailID').val(fer);
           
                document.getElementById("Button_eliminar_registro").click();
            
        }
        if (tip_event == "ver_reg_colab") {
            $('#hdnEmailID').val(fer);
            document.getElementById("Button_ver_registro_colaboracion").click();
        }
        //ver_cert_colab
        if (tip_event == "ver_cert_colab") {
            $('#hdnEmailID').val(fer);
            document.getElementById("Button_descarga_certificado").click();
        }
        if (tip_event == "archiva_doc_colab") {
            $('#hdnEmailID').val(fer);
            document.getElementById("Button_archiva_documento").click();
        }
        event.preventDefault();
        element.focus();
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
function actualiza_estado_boton_seleccion(gred, nombre_hiden, valor_weight, estado) {
    try {
        $("#" + gred + " tr[id=" + $("#" + nombre_hiden).val() + "]").each(function () {
            var fer = $(this).attr("id");
            var estado_visto = $(this).attr("id_estado_visto");
            if (estado_visto !== estado) {
                pre_actualiza_web_service_estado_visto(fer, 'data_grid_listado_solicitudes', 'id_estado', 'Red', estado);
                $(this).css({ "font-weight": valor_weight });
                $(this).attr("id_estado_visto", estado);
            }
        });
    }
    catch (err) {
        alert("Funcion actualiza_estado_boton_seleccion dice " + err.message);
    }
}
function pre_actualiza_web_service_estado_visto(id_solicitud, nombre_grid, nombre_campo, color_estado, estado) {
    try {

        var update = "Update ra_cd_usuarios_documentos_compartidos set ESTADO_VISTO_SOLICITANTE=" + estado + " where ID_USUARIOS_DOCUMENTOS_COMPARTIDOS=" + id_solicitud;
        web_service_actualiza(update);
        $("#" + nombre_grid + " tr[" + nombre_campo + "]").each(function () {
            var fer = $(this).attr(nombre_campo);
            if (fer == "2") {
                $(this).css({ "color": color_estado });

            }
        });

    }
    catch (err) {
        alert(err.message + " Funcion pre_actualiza_web_service_estado_visto");
    }
}
function web_service_actualiza(data) {
    try {
        $.ajax({
            type: 'POST',
            url: '../webservice/WebServiceRadicacion.asmx/update_radic_plantilla_radicado',
            data: "{'update':'" + data + "'}",
            contentType: 'application/json; utf-8',
            dataType: 'json',
            success: function (data) {
                if (data.d != null) {
                    $("#Hidden_resultado_web_service").val(data.d);
                    //alert($("#Hidden_resultado_web_service").val());
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                $("#Hidden_resultado_web_service").val(jqXHR.responseText);
                //alert($("#Hidden_resultado_web_service").val());
                alert(jqXHR.responseText);
            }

        });
    }
    catch (err) {
        alert(err.message + " Funcion web_service_actualiza");
    }
}
function Lista_tareas_estados(nombre_grid, nombre_campo, color_estado) {
    try {
        $("#" + nombre_grid + " tr[" + nombre_campo + "]").each(function () {
            var fer = $(this).attr(nombre_campo);
            if (fer == "2") {
                $(this).css({ "color": color_estado });

            }
        });
    }
    catch (err) {
        alert(err.message + " Función Lista_tareas_estados ");
    }
}
function Lista_tareas_lectura(nombre_grid, nombre_campo, color_estado) {
    try {
        $("#" + nombre_grid + " tr[" + nombre_campo + "]").each(function () {
            var fer = $(this).attr(nombre_campo);
            if (fer == "0") {
                $(this).css({ "font-weight": color_estado });
            }
        });
    }
    catch (err) {
        alert(err.message + " Función Lista_tareas_lectura ");
    }
}
function activa_boton_interface(nombre_buton) {
    try {
        document.getElementById(nombre_buton).click();
    }
    catch (err) {
        alert(err.message + " Función activa_boton_interface ");
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

        //corrige el posicionamiento horizontal del modalpopuextender, la propiedad width del panel debe estar en auto
        //var widtth_procent_left_rigth = (with_frame - document.getElementById("Panel_agregar_expediente_carpeta").clientHeight) / 2;
        //$('#Panel_agregar_expediente_carpeta').css("left", (Math.round(widtth_procent_left_rigth)) + "px");
        var heig_porcent = espacio_iframe - ((espacio_iframe * 20) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_usu_rel_solicitud').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_user_rel').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#contenido_procesa_usu_rel_solicitud').css("height", (document.getElementById("modal_content_user_rel").clientHeight - (document.getElementById("diver_cabcera_user_rel").clientHeight + document.getElementById("content_boton_user_rel").clientHeight)) + "px");
        //Para los modal que contiene gred
        $('#content_data_grid').css("height", (document.getElementById("contenido_procesa_usu_rel_solicitud").clientHeight - document.getElementById("contenido_titulo_val_radicacion_documentos").clientHeight) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_usuarios_relacionados " + err.message);
    }
}
function auto_zise_popup_compartir_documento() {
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


   
    $('#Panel_autoriza_compartir_documento').css("height", (espacio_iframe - 5) + "px");
    $('#contenido_procesa_autoriza_compartir_documento').css("height", ((espacio_iframe - 10) - document.getElementById("divcabecer2_autoriza_compartir_documento").clientHeight) + "px");
    $('#Iframe_compartir_documento_').css("height", ((espacio_iframe - 10) - document.getElementById("divcabecer2_autoriza_compartir_documento").clientHeight) + "px");

}
function auto_zise_popup_visor_externo() {
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

    $('#Panel_visor_externo').css("height", (espacio_iframe - 40) + "px");
    $('#Cotenedorpendiente_visor_externo').css("height", (espacio_iframe - 40) + "px");
    $('#Iframe_visor_externo_').css("height", (espacio_iframe - 40) + "px");

}
function auto_zise_popup_registro_colaboracion() {
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
    var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
    $('#Panel_registro_colaboracion').css("height", (heig_porcent) + "px");
    $('#contenido_procesa_registro_colaboracion').css("height", (document.getElementById("Panel_registro_colaboracion").clientHeight - document.getElementById("divcabecer2_registro_colaboracion").clientHeight) + "px");
    $('#Iframe_registro_colaboracion_').css("height", (document.getElementById("Panel_registro_colaboracion").clientHeight - (document.getElementById("divcabecer2_registro_colaboracion").clientHeight + 10)) + "px");
}
function activa_export_lista(hiden_name, name_evento) {
    try {
        var hiden = document.getElementById(hiden_name);
        var nombre_gred = "data_grid_documentos";
        var x = $('#' + nombre_gred + ' th');
        var txt = "";
        var i;
        for (i = 0; i < x.length; i++) {
            txt = txt + x[i].innerText.toUpperCase() + "|";
        }

        hiden.value = txt;
        document.getElementById("Hidden_name_event").value = name_evento;
        //document.getElementById("Button_export_lista_event").click();
        return txt;
    }
    catch (err) {
        alert(err.message + " Funcion activa_export_lista");
    }
}
function eliminar_fila_data_gred(gred, nombre_hiden) {
    try {
        var idex = 0;
        $("#" + gred + " tr[id=" + $("#" + nombre_hiden).val() + "]").each(function () {
            idex = $(this)[0].rowIndex;
        })
        $("#" + gred + " tr[id=" + $("#" + nombre_hiden).val() + "]").remove();
        $('#' + nombre_hiden).val("-1");
        if (idex == 1) {
            auto_zise_popup_lista_solicitudes("1", "");
        } else {
            //if (document.getElementById(gred).clientHeight < document.getElementById(gred + "PanelItemContent").clientHeight) {
                //document.getElementById(gred + "VerticalRail").style.display = "none"; .style.visibility = "hidden";VerticalBar

                /*if (document.getElementById(gred + "VerticalRail") !== undefined) {
                    document.getElementById(gred + "VerticalRail").style.display = "none";
                    document.getElementById(gred + "VerticalRail").style.visibility = "hidden";
                    document.getElementById(gred + "VerticalBar").style.display = "none";
                    document.getElementById(gred + "VerticalBar").style.visibility = "hidden";
                }*/

            //}
        }
    }
    catch (err) {
        alert(err.message + " Funcion eliminar_fila_data_gred");
    }

}

function busqueda_gred(HiddenSeleccion, data_grid, contenido_busqueda, CheckboxBusqueda, color_ref) {
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
        var cel_indes = 0;
        $("#" + data_grid + " tr:has(td)").each(function () {
            cel_indes = cel_indes + 1;
            var scrollableDiv = grid.parent();
            var rowtd = $(this);
            $(this).children("td").each(function (idex) {
                var tempotd = $(this).text().toLowerCase()
                var check = document.getElementById(CheckboxBusqueda).checked;
                if (check == true) {

                    if (idex >= 0) {
                        if (s == tempotd) {
                            $(this).parent().css({ "background-color": "LightSkyBlue", "color": color_ref });
                            //$(scrollableDiv).scrollTop(70);
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
                            $(this).parent().css({ "background-color": "LightSkyBlue", "color": color_ref });
                            $(scrollableDiv).scrollTop(0);
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
function auto_zise_popup_lista_solicitudes(value_lista_general, value_lista_usuario) {
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
        $('#div_contendor_principal').css("height", (espacio_iframe - 5) + "px");
        var total = document.getElementById("div_titulo_listado").clientHeight + document.getElementById("div_filtro__fil").clientHeight + document.getElementById("contenido_titulo_listado_solicitudes").clientHeight;
        var gridwith = with_frame - 10;
        var gridheihg_ = ((espacio_iframe - 5) - (total + 20));
        $('#Contenedorgrid_listado_solicitud').css("height", gridheihg_ + "px");
        if (value_lista_general == "1") {
            Lista_tareas_estados("data_grid_listado_solicitudes", "id_estado", "Red");
            Lista_tareas_lectura("data_grid_listado_solicitudes", "id_estado_visto", "700");
        }
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_lista_solicitudes " + err.message);
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
function colum_index(colum_name, nombre_grid) {
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
function actualiza_gre_campo(nombre_grid, id, valor_campo, nombre_campo) {
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
                    //var trfirst = $('#' + nombre_grid + ' tr:first').next();
                    var sas = $(this)[0].cells[idex];
                    if (sas.childElementCount <= 0) {
                        //var clinet_widt_old = sas.firstChild.clientWidth;
                        //var div_element = document.createElement("div");
                        //var p_element = document.createElement("p");
                        //p_element.innerHTML = valor_campo;
                        //div_element.appendChild(p_element);
                        //$(this)[0].appendChild(div_element);
                        $(this)[0].cells[idex].innerText = valor_campo;
                        //var clinet_widt_new = p_element.clientWidth;
                        //verifcar que la fila uno tenga childs
                        /*if (($(this)[0].cells[idex].clientWidth - 10) > trfirst[0].cells[idex].firstChild.clientWidth) {
                            trfirst[0].cells[idex].firstChild.style.width = $(this)[0].cells[idex].clientWidth + "px";
                            var x2 = $('#' + nombre_grid + 'Copy th');
                            x2[idex].firstChild.style.width = ($(this)[0].cells[idex].clientWidth - 10) + "px";
                            x2[idex].clientWidth = ($(this)[0].cells[idex].clientWidth - 10);
                        }*/
                        //$(this)[0].removeChild(div_element);
                    }
                    //Opcion para actualizar la primera fila de la tabla que se le agrega un div, cuado trae mas de un elemento
                    /*if (sas.childElementCount >= 1) {
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
                            //var x2 = $('#' + +nombre_grid + 'Copy th');
                            //x2[idex].firstChild.style.width = clinet_widt_new + "px";
                            //x2[idex].clientWidth = clinet_widt_new;
                        }
                        //sas.removeChild(div_element);
                    }*/

                }

            }


        })
        return true;
    }
    catch (err) {
        alert(err.message + " Funcion actualiza_gre_campo");
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
