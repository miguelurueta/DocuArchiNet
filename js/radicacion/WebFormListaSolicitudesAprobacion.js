$(document).ready(function () {
    $.fn.inicio = function () {
        //****************************************VALIDACION RADICACION**********************************************************************************
        //FUNCION ACTIVA SELECCION CLIK EN EL DATAGREDVIEW DE VALIDACION RADICACION
        if (document.getElementById("Hidden_seleccion").value == "PRIMERA") {
            document.getElementById("Hidden_seleccion").value = "SEGUNDA";
            //auto_zise_popup_pendinetes();
        }
        if (document.getElementById("Hidden_seleccion").value == "YES") {
            document.getElementById("Hidden_seleccion").value = "PRIMERA";
            auto_zise_popup_pendinetes();
        }
        
        $('#GridViewlista tr[id]').click(function () {
            $('#GridViewlista tr[id]').css({ "background" : "White" });
            $(this).css({ "background-color" : "#E7EDF5" });
            var fer = $(this).attr("id");
            $('#hdnEmailID_VAL').val(fer);
            $('#Hidden_id').val(fer);
            $('#Hidden_rad').val($(this).attr("id_radicado"));

        });
        $('#GridViewlista tr[id]').dblclick(function () {

            if ($('#Hidden_id').val() != "-1") {
                document.getElementById("Button_ver_documento_solicitud").click();
               
            }

        });
        //ASIGNA EL CURSOR DE SELECCION CUANDO PASA EL CURSOR EN EL DATAGREDVIEW DE VALIDACION RADICACION
        $('#GridViewlista tr[id]').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });
        //INICIA INTERFACE POPUP VALIDACION RADICADOS
        var tempo = document.getElementById("idente_chekbi_actyive");
        if (tempo === null) {
            //$("#GridViewlista th:nth-child(1)").append(" <input id='idente_chekbi_actyive' type='checkbox' name='activa_deativa_chek' onchange=desactiva_ch_data_grid('idente_chekbi_actyive') class='mmmjjjkkkuuu'  />");
        }
        //document.getElementById("Button_actualiza_lista_pendiente").click();


        if ($("#" + 'GridViewlista' + " tr:visible").length > 0) {
            //mueve_scroll_data_gred('GridViewlista', 'Hidden_id');
        }
        auto_zise_popup_pendinetes();
        auto_zise_popup_visor_externo();
        auto_zise_popup_solicitud_aprobacion();
        auto_zise_popup_paginas_externas_libres();
        auto_zise_popup_usuarios_relacionados();
        $(window).resize(bodyResize);
        function bodyResize() {
            auto_zise_popup_pendinetes();
            auto_zise_popup_visor_externo();
            auto_zise_popup_solicitud_aprobacion();
            auto_zise_popup_paginas_externas_libres();
            auto_zise_popup_usuarios_relacionados();

        }
        //******************************************FIN****************************************************************************************************
        $('#Lista').contextMenu('context-menu-1', {
            'Ver Documentos': {
                click: function (element) {  // element is the jquery obj clicked on when context menu launched
                    var RowID = $('#Hidden_id').val();
                    if (RowID == "-1") {
                        alert("Por favor seleccione el documento");
                    }
                    else {
                        if ($('#Hidden_id').val() != "-1" && $('#Hidden_id').val() != "0" && $('#Hidden_id').val() != "") {
                            var split = $('#Hidden_id').val().split("-");
                            if (split.length > 0) {
                                document.getElementById("Hidden_id_tarea_sel").value = split[1];
                                document.getElementById("Hidden_tipo_visor").value = "VISOR WORKFLOW";
                                document.getElementById("Button_visor_emergente").click();
                                return false;
                            }

                        }
                    }
                }
            },
            'Salir del Menu': {
                click: function (element) { }
            }
        })

        $('.alterna_image').hover(function () {
            //$(this).animate({ opacity: 0 });
            var boton = $(this);
            if (boton[0].alt == "Actualizar lista") {
                var sr = boton[0].src;
                sr = sr.replace("actualizar.jpg", "actualizar-1.jpg");
                boton[0].src = sr;
            }
            if (boton[0].alt == "Seleccionar de la lista") {
                var sr = boton[0].src;
                sr = sr.replace("seleccionar.jpg", "seleccionar-2.jpg");
                boton[0].src = sr;
            }
            if (boton[0].alt == "Renviar a Usuario") {
                var sr = boton[0].src;
                sr = sr.replace("envia_usuario.jpg", "envia_usuario-2.jpg");
                boton[0].src = sr;
            }
            if (boton[0].alt == "Renviar a Grupo") {
                var sr = boton[0].src;
                sr = sr.replace("enviar_actividad.jpg", "enviar_actividad-2.jpg");
                boton[0].src = sr;
            }
            if (boton[0].alt == "Sin tareas pendientes") {
                var sr = boton[0].src;
                sr = sr.replace("pendiente.jpg", "pendiente2.jpg");
                boton[0].src = sr;
            }
            if (boton[0].alt == "tareas pendientes") {
                var sr = boton[0].src;
                sr = sr.replace("pendiente.jpg", "pendiente2.jpg");
                boton[0].src = sr;
            }
            if (boton[0].alt == "Anotacion a tarea actual") {
                var sr = boton[0].src;
                sr = sr.replace("notas.jpg", "notas-2.jpg");
                boton[0].src = sr;
            }
            if (boton[0].alt == "Enviar tarea a") {
                var sr = boton[0].src;
                sr = sr.replace("terminar.jpg", "terminar-2.jpg");
                boton[0].src = sr;
            }
            if (boton[0].alt == "El sistema decide el envío por usted") {
                var sr = boton[0].src;
                sr = sr.replace("autoterminar.jpg", "autoterminar-2.jpg");
                boton[0].src = sr;
            }
        }, function () {
            //$(this).animate({ opacity: 1 });
            var boton = $(this);
            if (boton[0].alt == "Actualizar lista") {
                var sr = boton[0].src;
                sr = sr.replace("actualizar-1.jpg", "actualizar.jpg");
                boton[0].src = sr;
            }
            if (boton[0].alt == "Seleccionar de la lista") {
                var sr = boton[0].src;
                sr = sr.replace("seleccionar-2.jpg", "seleccionar.jpg");
                boton[0].src = sr;
            }
            if (boton[0].alt == "Renviar a Usuario") {
                var sr = boton[0].src;
                sr = sr.replace("envia_usuario-2.jpg", "envia_usuario.jpg");
                boton[0].src = sr;
            }
            if (boton[0].alt == "Renviar a Grupo") {
                var sr = boton[0].src;
                sr = sr.replace("enviar_actividad-2.jpg", "enviar_actividad.jpg");
                boton[0].src = sr;
            }
            if (boton[0].alt == "Sin tareas pendientes") {
                var sr = boton[0].src;
                sr = sr.replace("pendiente2.jpg", "pendiente.jpg");
                boton[0].src = sr;
            }
            if (boton[0].alt == "tareas pendientes") {
                var sr = boton[0].src;
                sr = sr.replace("pendiente2.jpg", "pendiente.jpg");
                boton[0].src = sr;
            }
            if (boton[0].alt == "Anotacion a tarea actual") {
                var sr = boton[0].src;
                sr = sr.replace("notas-2.jpg", "notas.jpg");
                boton[0].src = sr;
            }
            if (boton[0].alt == "Enviar tarea a") {
                var sr = boton[0].src;
                sr = sr.replace("terminar-2.jpg", "terminar.jpg");
                boton[0].src = sr;
            }
            if (boton[0].alt == "El sistema decide el envío por usted") {
                var sr = boton[0].src;
                sr = sr.replace("autoterminar-2.jpg", "autoterminar.jpg");
                boton[0].src = sr;
            }
        });
    }


    $("#ButtonFiltro").click(function (e) {
        var check = true;
        var s = $("#contenidobusqueda_filtro").val().toLowerCase();
        $("#HiddenFiltro").val(s);
    });
    
    $("#TextBoxdatos").keypress(function (e) {
        if (e.which == 13) {
            event.preventDefault();
        }
    })
    $("#contenidobusqueda_filtro").keypress(function (e) {
        if (e.which == 13) {
            event.preventDefault();
        }
    })

});
function cambia_estado_boton_reasignar(estado) {
    if (estado == "VISIBLE") {
        $("#ButtonReasignarTerminar").css("display", "block")
    } else {
        $("#ButtonReasignarTerminar").css("display", "none")
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
//ACTUALIZA GRED VIEW PENDIENTE
function actualiza_gred_pendiente(contenido) {
    try {
        $("#GridViewlista tr[id=" + $("#Hidden_id").val() + "]").each(function () {
            var idex = -1;
            idex = colum_index("DETALLEPENDIENTE");
            if (idex != -1) {
                $(this)[0].cells[idex].innerText = contenido;
            }

        })
    }
    catch (err) {
        alert(err.message + " funcion filtro_gred " + err.message);
    }
}
function colum_index(colum_name) {
    try {
        var x = $('#GridViewlista th');
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
function prevent(event, element) {
    try {
       
        var fer = $(element).attr("idd");
        var id_radicado = $(element).attr("id_radicado_");
        var tip_event = $(element).attr("tip_event");
        if (tip_event == "comp_gestion_solic") {
            $('#hdnEmailID_VAL').val(fer);
            $('#Hidden_id').val(fer);
            $('#Hidden_rad').val(id_radicado);
            document.getElementById("Button_sacar_pendiente").click();
        }
        if (tip_event == "ver_doc_solic") {
            $('#hdnEmailID_VAL').val(fer);
            $('#Hidden_id').val(fer);
            $('#Hidden_rad').val(id_radicado);
            document.getElementById("Button_ver_documento_solicitud").click();
        }
        if (tip_event == "descargar_doc_resp_solic") {
            $('#hdnEmailID_VAL').val(fer);
            $('#Hidden_id').val(fer);
            $('#Hidden_rad').val(id_radicado);
            document.getElementById("Button_ver_documento_respuesta_solicitud").click();
        }
        if (tip_event == "gest_solic_aprobacion") {
            $('#hdnEmailID_VAL').val(fer);
            $('#Hidden_id').val(fer);
            $('#Hidden_rad').val(id_radicado);
            document.getElementById("ButtonActiva_solicitud_aprobacion").click();
        }
        if (tip_event == "ver_not_solic_aprobacion") {
            $('#hdnEmailID_VAL').val(fer);
            $('#Hidden_id').val(fer);
            $('#Hidden_rad').val(id_radicado);
            document.getElementById("Button_activa_ver_nota_general").click();
        }
        if (tip_event == "ver_user_rel_sol") {
            $('#hdnEmailID_VAL').val(fer);
            $('#Hidden_id').val(fer);
            $('#Hidden_rad').val(id_radicado);
            document.getElementById("Button_listar_usuarios_relacionados_solicitud").click();
        }
        event.preventDefault();
        element.focus();
    }
    catch (err) {
        alert(err.message + " Funcion prevent");
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

        //espacio_iframe = (espacio_iframe - ((espacio_iframe * 40) / 100));
        //with_frame = (with_frame - ((with_frame * 30) / 100));
        var gridwith = with_frame - 2;
        var gridheihg = 250;
        $('#content_data_grid').css("height", (gridheihg) + "px");
        $('#Panelactividad_documentos').css("height", (gridheihg) + "px");
        gridwith = document.getElementById("Panelactividad_documentos").offsetWidth - 3;
        if ($('#data_grid_documentos td').children.length > 0 && $('#data_grid_documentos tr:visible').length > 0) {
            $(document).ready(function () { $('#data_grid_documentos').gridviewScroll({ width: gridwith, height: (gridheihg - 5) }); })
            //data_grid_documentosHorizontalRail
            //$('#data_grid_documentosHorizontalRail').css("top", (gridheihg - 30) + "px");
            //document.getElementById("data_grid_documentosHorizontalRail").style.top = (gridheihg - 30) + "px";
        }

    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_usuarios_relacionados " + err.message);
    }
}
function prevent_scrol(event, e) {
    try {

        if (e.className == "GridviewScrollItem_line_corte_tr") {
            e.classList.remove("GridviewScrollItem_line_corte_tr");
            e.classList.toggle("GridviewScrollItem_line_corte_tr_scrol");
        } else {
            e.classList.remove("GridviewScrollItem_line_corte_tr_scrol");
            e.classList.toggle("GridviewScrollItem_line_corte_tr");
        }
        event.preventDefault();
    }
    catch (err) {
        alert(err.message + " Funcion prevent_scrol");
    }
}
//Actualiza detalle pendiente
function actualiza_gred(columname, valor) {
    try {
        $("#GridViewlista tr[id=" + $("#Hidden_id").val() + "]").each(function () {
            var idex = -1;
            idex = colum_index(columname);
            if (idex != -1) {
                if ($(this)[0].cells[idex].children.length > 0) {
                    $(this)[0].cells[idex].children[0].innerText = valor;
                    $(this)[0].cells[idex].children[0].style.overflow = "auto";
                } else {
                    $(this)[0].cells[idex].innerText = "";
                    var heade = $("#GridViewlistaCopy th");
                    var div = document.createElement("div");
                    div.innerText = valor;
                    div.style.overflow = "auto";
                    //heade[idex].children[0].style.width = div.style.width
                    div.style.width = heade[idex].children[0].style.width;
                    $(this)[0].cells[idex].appendChild(div);

                }
            }


        })

    }
    catch (err) {
        alert(err.message + " Funcion actualiza_gred");
    }
}
//Retorna el idex de una columna en una tabla
function colum_index(colum_name) {
    try {
        var x = $('#GridViewlista th');
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
//ELIMINA REGISTROS GRED PENDIENTES
function elimina_registro_gred_pendiente() {
    try {
        if (document.getElementById("Hidden_lista_eliminar_tarea") == "0") {
            return false;
        }
        var spli = document.getElementById("Hidden_lista_eliminar_tarea").value.split(".");
        for (i = 0; i <= spli.length - 1 ; i++) {
            $('#GridViewlista .dummychkstyle').each(function () {
                if (this.checked == true) {
                    var cel = $(this).parent().parent().parent();
                    var atri = $(this).parent().parent().parent().attr("id");
                    if (atri == undefined) {
                        cel = $(this).parent().parent();
                        atri = $(this).parent().parent().attr("id");
                    }
                    if (atri == spli[i]) {
                        document.getElementById("GridViewlista").deleteRow(cel[0].rowIndex);
                    }
                }
            })
        }

    }
    catch (err) {
        alert(err.message);
    }
    finally {
        document.getElementById("Hidden_lista_eliminar_tarea").value = "0";
    }
}

//ACTIVA EL BOTON ASIGNAR TAREA
function asignar_tarea_pendiente(valor) {
    try {
       
        var bto = window.parent.document.getElementById("ButtonAsignar");
        if (bto !== null) {
            $('#hdnEmailID', window.parent.document).val(valor);
            bto.click();
        }
    }
    catch (err) {
        alert(err.message + " funcion asignar_tarea_pendiente " + err.message);
    }
}
//ACTIVA BOTON SUBIR A PEDIENTE
function sube_tarea_a_pendiente() {
    try {
        if (document.getElementById("Hidden_resultado").value == "YES") {
            var bto = window.parent.document.getElementById("Button_sube_pediente");
            if (bto !== null) {
                bto.click();
            }
        }
    }
    catch (err) {
        alert(err.message + " funcion sube_tarea_a_pendiente " + err.message);
    }
}

function onDataShown(sender, args) {
    sender._popupBehavior._element.style.zIndex = 1000001;

}
//MUEVE EL SCCROL AL ID SELECCIONADO
function mueve_scroll_data_gred(data_grid, HiddenSeleccion) {
    try {
        if ($("#" + data_grid + " td").children.length == 0 && $("#" + data_grid + " tr:visible").length == 0) {
            return true;
        }
        if ($("#" + HiddenSeleccion).val() != "-1" && $("#" + HiddenSeleccion).val() != "0") {
            var scrollableDiv = $("#" + data_grid).parent();
            var index = $("#" + data_grid + " tr[id= " + $("#" + HiddenSeleccion).val() + "]");
            //limpia todos los seleccionados
            $("#" + data_grid + " tr[id]").css({ "background-color": "transparent", "color": "Black" });
            $("#" + data_grid + " tr[id=" + $("#" + HiddenSeleccion).val() + "]").css({ "background-color": "LightSkyBlue", "color": "Red" });
            $("#" + data_grid + " tr[id= " + $("#" + HiddenSeleccion).val() + "]").each(function () {
                if (index[0].rowIndex > 1) {
                    $(scrollableDiv).scrollTop(70);
                    $(scrollableDiv).scrollTop(($(this).offset().top));
                    return true;
                }

            });
        }
    }
    catch (err) {
        alert(err.message + " Funcion mueve_scroll_data_gred");
    }
}
function confirma_respuesta(mensaje) {
    try {
        if (document.getElementById("hdnEmailID").value == "0" || document.getElementById("hdnEmailID").value == "-1") {
            alert("Por favor seleccione un usuario o grupo para enviar la tarea")
            return false;
        }
        var res = confirm(mensaje);
        if (res == true) {
            document.getElementById("HiddenPROMP").value = "1";
        } else {
            document.getElementById("HiddenPROMP").value = "0";
        }
    }
    catch (err) {
        alert("Funcion confirma_respuesta" + err.message);
    }
}
function confirma_respuesta_terminar(mensaje) {
    try {
        if (document.getElementById("hdnEmailID_sel").value == "0" || document.getElementById("hdnEmailID_sel").value == "-1") {
            alert("Por favor seleccione un usuario o grupo para enviar la tarea")
            return false;
        }
        var res = confirm(mensaje + " " + document.getElementById("DropDownActividades").value);
        if (res == true) {
            document.getElementById("HiddenPROMP").value = "1";
        } else {
            document.getElementById("HiddenPROMP").value = "0";
        }
    }
    catch (err) {
        alert(err.message + " funcion confirma_respuesta_terminar");
    }
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

function auto_zise_popup_pendinetes() {
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
        if (parent.document.getElementById("Iframependiente_")) {
            espacio_iframe = parent.document.getElementById("Iframependiente_").clientHeight;
        }    
        $('#div_contendor_principal').css("height", (espacio_iframe - 40) + "px");
        $('#div_titulo_listado').css("height", (document.getElementById("contenidobusqueda").clientHeight + 3) + "px");
        $('#div_contendor_filtro_listado').css("height", (document.getElementById("div_filtro__fil").clientHeight) + "px");
        var total = document.getElementById("div_titulo_listado").clientHeight + document.getElementById("div_contendor_filtro_listado").clientHeight + document.getElementById("contenido_titulo_listado_solicitudes").clientHeight;
        var gridwith = with_frame - 5;
        var gridheihg_ = (espacio_iframe - (total + 20));
        $('#content_grid').css("height", gridheihg_ + "px");
        $('#Panel_principal').css("height", (gridheihg_) + "px");
     
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_pendinetes " + err.message);
    }
}
function auto_zise_popup_envia_usuario_grupo() {
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
        $('#DivBotones').css("height", (document.getElementById("btnCancelpagina").clientHeight + 5) + "px");
        var total = document.getElementById("DivBotones").clientHeight + document.getElementById("Divcab").clientHeight;
        $('#DivColorPagina').css("height", ((espacio_iframe - 1) - (total + 5)) + "px");
        $('#frameeditexpanse_').css("height", ((espacio_iframe - 1) - (total + 5)) + "px");
        //$('#Panelpagina').css("height", (total + document.getElementById("DivColorPagina").clientHeight) + "px");
        //Panelpagina
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_envia_usuario_grupo " + err.message);
    }
}

function auto_zise_popup_visor_externo() {
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



        $('#Panel_visor_externo').css("height", (espacio_iframe - 1) + "px");
        $('#Cotenedorpendiente_visor_externo').css("height", (espacio_iframe - 1) + "px");
        $('#Iframe_visor_externo_wf_').css("height", (espacio_iframe) + "px");
    }
    catch (err) {
        alert(err.message + " Función auto_zise_popup_visor_externo");
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


        $('#Iframelibre_notas_general_').css("height", (espacio_iframe - 120) + "px");
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_paginas_externas_libres");
    }

}
function auto_zise_popup_solicitud_aprobacion() {
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


    

    $('#Panel_solicitud_aprobacion').css("height", (espacio_iframe - 5) + "px");
    $('#contenido_procesa_solicitud_aprobacion').css("height", (espacio_iframe - 5) + "px");
    $('#Iframe_solicitud_aprobacion').css("height", (espacio_iframe - 5) + "px");
}
function progres_hiden(progres) {
    $("#progres_bar").css("display", "none");
}
//ACTIVA EL GIF DE PROGRESO DE UN EVENTO
function posicion_update_pogres(progres) {
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
        var prog = document.getElementById(progres);
        var widtop = (espacio_iframe / 2);
        var heitop = (with_frame / 2);
        prog.style.top = widtop + "px";
        prog.style.left = heitop + "px";
        prog.style.zIndex = "1000009";
        $("#progres_bar").css("display", "block");
        prog.style.position = "fixed";
    }
    catch (err) {
        alert(err.message + " funcion posicion_update_pogres " + err.message);
    }

}
function busqueda_gred__(HiddenSeleccion, data_grid, contenido_busqueda, CheckboxBusqueda) {
    try {
        if ($("#" + contenido_busqueda).val() == "") {
            $("#" + data_grid + " tr[id]").css({ "background-color": "transparent", "color": "Black" });
            $("#" + HiddenSeleccion).val("0");
            return false;
        }
        $("#" + HiddenSeleccion).val("0");
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
                            $(this).parent().css({ "background-color": "LightSkyBlue", "color": "orange" });
                            //$(scrollableDiv).scrollTop(70);
                            //var id_ref = $(this).parent();
                            //$(scrollableDiv).scrollTop($(id_ref).offset().top);
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
                            $(this).parent().css({ "background-color": "LightSkyBlue", "color": "orange" });
                            //$(scrollableDiv).scrollTop(70);
                            //var id_ref = $(this).parent();
                            //$(scrollableDiv).scrollTop($(id_ref).offset().top var id_ref = $(this).parent();
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
function busqueda_gred(HiddenSeleccion, data_grid, contenido_busqueda, CheckboxBusqueda) {
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
                        $(this).parent().css({ "background-color": "LightSkyBlue", "color": "green" });
                        //$(scrollableDiv).scrollTop(70);
                        //var id_ref = $(this).parent();
                        //$(scrollableDiv).scrollTop($(id_ref).offset().top);
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
                        //$(scrollableDiv).scrollTop(70);
                        //var id_ref = $(this).parent();
                        //$(scrollableDiv).scrollTop($(id_ref).offset().top);
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
function filtro_gred(HiddenSeleccion, data_grid, contenido_busqueda, CheckboxBusqueda) {
    try {
        if (document.getElementById(data_grid).rows.length == 1) {
            return true;
        }
        var ro = document.getElementById(data_grid).rows.length;
        //desactiva_chek();
        $("#" + HiddenSeleccion).val("-1");
        var refgrid;
        var filtro;
        var ito = 0;
        var confirma_hidem_fila = 0;
        var showtr;
        $("#" + data_grid + " tr[id]").css({ "background-color": "transparent", "color": "Black" });
        var s = $("#" + contenido_busqueda).val().toLowerCase();
        var grid = $("#" + data_grid);
        $("#" + data_grid + " tr:hidden").show();
        var acierto = -1;
        var check = document.getElementById(CheckboxBusqueda).checked;
        if (check == true && s == "") {
            return true;
        }
        //$('#data_grid_auxiliar_listaHeader').hide();
        $("#" + data_grid + " tr:has(td)").each(function () {
            var refdif = $(this);
            var confirm = -1;
            $(this).children("td").each(function (idex) {
                var tempotd = $(this).text().toLowerCase()
                var check = document.getElementById(CheckboxBusqueda).checked;
                if (check == true) {
                    if (idex >= 0) {
                        if (s == tempotd) {
                            refdif.show();
                            confirm = 1;
                            acierto = 1;
                        } else {

                            //refdif.hide();
                        }
                    }
                }

                if (check == false) {
                    if (idex >= 0) {
                        var compare = tempotd;
                        var strcompre = compare.indexOf(s);
                        if (strcompre >= 0) {
                            refdif.show();
                            acierto = 1;
                            confirm = 1;
                        } else {
                            //refdif.hide();
                        }
                    }
                }

            })
            ito++;

            if (confirm == -1 && ito != 1) {
                refdif.hide();
                $("#" + data_grid).append(refdif.clone());
                refdif.remove();
            }
            if (confirm == -1 && ito == 1) {
                refdif.hide();
                $("#" + data_grid).append(refdif.clone());
                refdif.remove();
            }
            if (acierto == -1) {
                $("#" + data_grid + " tr:hidden").show();
            }
        });
    }
    catch (err) {
        alert(err.message + " funcion filtro_gred " + err.message);
    }
}
function filtro_gred_1(HiddenSeleccion, data_grid, contenido_busqueda, CheckboxBusqueda) {
    try {
        if (document.getElementById(data_grid).rows.length == 1) {
            return true;
        }
        var ro = document.getElementById(data_grid).rows.length;
        desactiva_chek();
        $("#" + HiddenSeleccion).val("-1");
        $("#Hidden_id").val("0");
        var refgrid;
        var filtro;
        var ito = 0;
        var confirma_hidem_fila = 0;
        var showtr;
        $("#" + data_grid + " tr[id]").css({ "background-color": "transparent", "color": "Black" });
        var s = $("#" + contenido_busqueda).val().toLowerCase();
        var grid = $("#" + data_grid);
        $("#" + data_grid + " tr:hidden").show();
        var acierto = -1;
        //$('#data_grid_auxiliar_listaHeader').hide();
        $("#" + data_grid + " tr:has(td)").each(function () {

            var refdif = $(this);
            var confirm = -1;
            $(this).children("td").each(function (idex) {

                var tempotd = $(this).text().toLowerCase()
                var check = document.getElementById(CheckboxBusqueda).checked;
                if (check == true) {

                    if (idex >= 0) {
                        if (s == tempotd) {
                            (this).parent().show();

                            acierto = 1;
                            confirm = 1;
                        } else {


                        }
                    }
                }

                if (check == false) {
                    if (idex >= 0) {
                        var compare = tempotd;
                        var strcompre = compare.indexOf(s);
                        if (strcompre >= 0) {
                            refdif.show();
                            acierto = 1;
                            confirm = 1;
                        } else {

                        }
                    }
                }

            })
            ito++;
            if (confirm == -1 && ito != 1) {
                refdif.hide();
                $("#" + data_grid).append(refdif.clone());
                refdif.remove();
            }
            if (confirm == -1 && ito == 1) {
                refdif.hide();
                $("#" + data_grid).append(refdif.clone());
                refdif.remove();
            }
            if (acierto == -1) {
                $("#" + data_grid + " tr:hidden").show();

            }
        });
    }
    catch (err) {
        alert(err.message + " funcion filtro_gred " + err.message);
    }
}
