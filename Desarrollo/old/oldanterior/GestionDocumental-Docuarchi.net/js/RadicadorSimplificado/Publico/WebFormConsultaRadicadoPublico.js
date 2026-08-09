$(document).ready(function () {
    $.fn.inicio = function () {
        //****************************************VALIDACION RADICACION**********************************************************************************
        //FUNCION ACTIVA SELECCION CLIK EN EL DATAGREDVIEW DE VALIDACION RADICACION
        $('#GridView_val_radicacion tr[id]').click(function () {
            $('#GridView_val_radicacion tr[id]').css({ "background": "White", "color": "Black" });
            $(this).css({ "background-color": "#E7EDF5", "color": "Black" });
            var fer = $(this).attr("id");
            $('#hdnEmailID_VAL').val(fer);
            var recordgred = $('#GridView_val_radicacion tr[id=' + $('#hdnEmailID_VAL').val() + ']');
            if ($('#hdnEmailID_VAL').val() != "-1") {
                var idex = colum_index('CONSECUTIVO_RADICADO', 'GridView_val_radicacion');
                if (idex != -1) {
                    document.getElementById("Hidden_id_tarea_sel").value = recordgred[0].cells[idex].innerText;
                    return false;
                } else {
                    alert("Imposible encontrar el index de la columna CONSECUTIVO_RADICADO");
                    return false;
                }
            }

        });

        $('#GridView_val_radicacion tr[id]').dblclick(function () {

            if ($('#hdnEmailID_VAL').val() != "-1") {
                var recordgred = $('#GridView_val_radicacion tr[id=' + $('#hdnEmailID_VAL').val() + ']');
                var idex = colum_index('CONSECUTIVO_RADICADO', 'GridView_val_radicacion');
                if (idex != -1) {
                    document.getElementById("Hidden_id_tarea_sel").value = recordgred[0].cells[idex].innerText;
                    document.getElementById("Hidden_tipo_visor").value = "VISOR RADICADOR";
                    document.getElementById("Button_visor_emergente").click();
                    return false;
                } else {
                    alert("Imposible encontrar el index de la columna CONSECUTIVO_RADICADO");
                    return false;
                }
            }

        });
        //ASIGNA EL CURSOR DE SELECCION CUANDO PASA EL CURSOR EN EL DATAGREDVIEW DE VALIDACION RADICACION
        $('#GridView_val_radicacion tr[id]').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });
        //INICIA INTERFACE POPUP VALIDACION RADICADOS
        var tempo = document.getElementById("idente_chekbi_actyive");
        if (tempo === null) {
            //$("#GridView_val_radicacion th:nth-child(1)").append(" <input id='idente_chekbi_actyive' type='checkbox' name='activa_deativa_chek' onchange=desactiva_ch_data_grid('idente_chekbi_actyive') class='mmmjjjkkkuuu'  />");
        }
        auto_zise_popup_validacion_radicados();
        auto_zise_popup_detalle_transacciones();
        auto_zise_popup_imagen_respuesta();
        auto_zise_popup_detalle_trazabilidad();
        
        //******************************************FIN****************************************************************************************************
    }

    $('#contenido_datagrid_val_radicacion').contextMenu('context-menu-2', {

        'Ver documentos': {
            click: function (element) {  // element is the jquery obj clicked on when context menu launched
                //$('#HiddenSeleccion').val("-1");


                //document.getElementById("Buttond_Filtro").click();
                if ($('#hdnEmailID_VAL').val() != "-1") {
                    var recordgred = $('#GridView_val_radicacion tr[id=' + $('#hdnEmailID_VAL').val() + ']');
                    var idex = colum_index('CONSECUTIVO_RADICADO', 'GridView_val_radicacion');
                    if (idex != -1) {
                        document.getElementById("Hidden_id_tarea_sel").value = recordgred[0].cells[idex].innerText;
                        document.getElementById("Hidden_tipo_visor").value = "VISOR RADICADOR";
                        document.getElementById("Button_visor_emergente").click();
                        return false;
                    }
                }


            }
        },
        'Salir del Menu': {
            click: function (element) { }
        }
    }


    );
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


    } catch (e) {
        alert(" funcion load " + e.message);
    }

});
function activa_retroceso_principal() {
    try {
        if (window.parent.document.getElementById("element_a_inicio")) {
            window.parent.document.getElementById("element_a_inicio").click();
        }

    }
    catch (err) {
        alert(err.message + " Funcion activa_retroceso_pagina");
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
function rezize_event() {
    try {
        auto_zise_popup_validacion_radicados();
        auto_zise_popup_detalle_transacciones();
        auto_zise_popup_imagen_respuesta();
        auto_zise_popup_detalle_trazabilidad();
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
function prevent(event, element) {
    try {

        var fer = $(element).attr("idd");
        var tip_event = $(element).attr("tip_event");
        if (tip_event == "est_radic") {
            $('#hdnEmailID_VAL').val(fer);
            document.getElementById("Button_Trazabilidad").click();
        }
        if (tip_event == "trans_radic") {
            $('#hdnEmailID_VAL').val(fer);
            document.getElementById("Button_Log_respuesta").click();
        }
        if (tip_event == "detalle_radic") {
            $('#hdnEmailID_VAL').val(fer);
            document.getElementById("Button_detalle_radicado").click();
        }
        if (tip_event == "detalle_radic") {
            $('#hdnEmailID_VAL').val(fer);
            document.getElementById("Button_detalle_radicado").click();
        }
        if (tip_event == "doc_rel_radic") {
            $('#hdnEmailID_VAL').val(fer);
            if ($('#hdnEmailID_VAL').val() != "-1") {
                var recordgred = $('#GridView_val_radicacion tr[id=' + $('#hdnEmailID_VAL').val() + ']');
                var idex = colum_index('CONSECUTIVO_RADICADO', 'GridView_val_radicacion');
                if (idex != -1) {
                    document.getElementById("Hidden_id_tarea_sel").value = recordgred[0].cells[idex].innerText;
                    document.getElementById("Hidden_tipo_visor").value = "VISOR RADICADOR";
                } else {
                    alert("Imposible encontrar el index de la columna CONSECUTIVO_RADICADO");

                }
            }
            document.getElementById("Button_visor_emergente").click();
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
function retorna_colum_mtriz(hiden_name) {
    var hiden = document.getElementById(hiden_name);
    var x = $('#GridView_val_radicacion th');
    var txt = "";
    var i;
    for (i = 0; i < x.length; i++) {
        txt = txt + x[i].innerText.toUpperCase() + "|";
    }
    hiden.value = txt;
    return txt;
}

function confirma_respuesta(mensaje) {
    try {
        var res = confirm(mensaje);
        if (res == true) {
            document.getElementById("Hidden_alert_respuesta").value = "YES";
        } else {
            document.getElementById("Hidden_alert_respuesta").value = "NO";
        }
    }
    catch (err) {
        alert(err.message + " Funcion confirma_respuesta");
    }
}
function auto_zise_popup_detalle_trazabilidad() {
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
        /*$('#Panel_trazabilidad').css("height", (espacio_iframe - 40) + "px");
        $('#Cotenedorpendiente_trazabilidad').css("height", (espacio_iframe - 40) + "px");
        $('#Iframe_trazabilidad_').css("height", (espacio_iframe - 40) + "px");*/
        //corrige el posicionamiento horizontal del modalpopuextender, la propiedad width del panel debe estar en auto
        //var widtth_procent_left_rigth = (with_frame - document.getElementById("Panel_agregar_expediente_carpeta").clientHeight) / 2;
        //$('#Panel_agregar_expediente_carpeta').css("left", (Math.round(widtth_procent_left_rigth)) + "px");
        var heig_porcent = espacio_iframe - ((espacio_iframe * 5) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_trazabilidad').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_Panel_trazabilidad').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#Cotenedorpendiente_trazabilidad').css("height", (document.getElementById("modal_content_Panel_trazabilidad").clientHeight - (document.getElementById("Cabecerapendiente_trazabilidad").clientHeight + 1)) + "px");
        //Para los modal que contiene gred
        $('#Iframe_trazabilidad_').css("height", (document.getElementById("Cotenedorpendiente_trazabilidad").clientHeight - 1) + "px");
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_detalle_trazabilidad");
    }
}
function auto_zise_popup_imagen_respuesta() {
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
        var heig_porcent = espacio_iframe - ((espacio_iframe * 5) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_imagen_respuesta').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_Panel_imagen_respuesta').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#Cotenedorpendiente_imagen_respuesta').css("height", (document.getElementById("modal_content_Panel_imagen_respuesta").clientHeight - (document.getElementById("Cabecerapendiente_imagen_respuesta").clientHeight + 5)) + "px");
        //Para los modal que contiene gred
        $('#Iframe_imagen_respuesta_').css("height", (document.getElementById("Cotenedorpendiente_imagen_respuesta").clientHeight - 1) + "px");
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_imagen_respuesta");
    }
}
function auto_zise_popup_detalle_transacciones() {
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
        var heig_porcent = espacio_iframe - ((espacio_iframe * 5) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_transacciones').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_Panel_transacciones').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#Cotenedorpendiente_transacciones').css("height", (document.getElementById("modal_content_Panel_transacciones").clientHeight - (document.getElementById("Cabecerapendiente_transacciones").clientHeight + 1)) + "px");
        //Para los modal que contiene gred
        $('#Iframe_transacciones_').css("height", (document.getElementById("Cotenedorpendiente_transacciones").clientHeight - 1) + "px");
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_detalle_transacciones");
    }
}
function auto_zise_popup_notificar() {
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
   
        $('#Panel_notifica_gestion').css("height", (espacio_iframe - 120) + "px");
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_notificar");
    }
}

function GetChar(event) {
    try {
        var chCode = ('charCode' in event) ? event.charCode : event.keyCode;
        if (chCode == 13) {

        }
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_editar_radicados");
    }
}
function progres_hiden(progres) {
    try {
        $("#progres_bar").css("display", "none");
    }
    catch (err) {
        alert(err.message + " Funcion progres_hiden");
    }
}
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
        alert(err.message + " Funcion posicion_update_pogres");
    }

}



function onDataShown(sender, args) {
    sender._popupBehavior._element.style.zIndex = 1000001;

}
//MUEVE EL SCCROL AL ID SELECCIONADO
function mueve_scroll_data_gred(data_grid, HiddenSeleccion) {
    if ($("#" + HiddenSeleccion).val() != "-1") {
        var scrollableDiv = $("#" + data_grid).parent();
        //limpia todos los seleccionados
        $("#" + data_grid + " tr[id]").css({ "background-color": "transparent", "color": "Black" });
        $("#" + data_grid + " tr[id=" + $("#" + HiddenSeleccion).val() + "]").css({ "background-color": "LightSkyBlue", "color": "Black" });
        $("#" + data_grid + " tr[id= " + $("#" + HiddenSeleccion).val() + "]").each(function () {
            $(scrollableDiv).scrollTop(70);
            $(scrollableDiv).scrollTop(($(this).offset().top));
            return true;
        });
    }
}
//FUNCION ACTIVA Y DEACTIVA LOS CAMPOS CHEKEADOS EN UNA TABLA
function desactiva_ch_data_grid(idente_chekbi_actyive) {
    var e = $("#" + idente_chekbi_actyive);

    if ($(e).is(':checked')) {
        var x = document.getElementsByClassName("jjjjjjjjjjj");
        for (i = 0; i < x.length; i++) {
            var z = x[i].firstChild;
            z.checked = false;

        }

    }
    else {

        var x = document.getElementsByClassName("jjjjjjjjjjj");
        for (i = 0; i < x.length; i++) {
            var z = x[i].firstChild;
            z.checked = true;

        }


    }
}
//AUTO SIZE POPUP VALIDACION RADICADOS
function auto_zise_popup_validacion_radicados() {
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


    
    //$('#contendor_principal').css("height", (espacio_iframe - 30) + "px");
    //$('#Contentizquierdo').css("height", (espacio_iframe - 30) + "px");
    //$('#Contenedorderecho').css("height", (espacio_iframe - 30) + "px");
   // $("#contenido_titulo_controles_consulta").css("height", $('#Label_title_consulta').height() + 5 + "px");
    //$("#contenido_controles_buton_consulta").css("height", $('#Button_consulta_val_radicacion').height() + 10 + "px");
   // $("#contenido_controles_consulta").css("height", (espacio_iframe - 10) - ($('#contenido_titulo_controles_consulta').height() + $('#contenido_controles_buton_consulta').height()) + "px");
   // $("#_Panelvalidacion_val_radicacion").css("height", (espacio_iframe - 10) - ($('#contenido_titulo_controles_consulta').height() + $('#contenido_controles_buton_consulta').height()) + "px");
   // $("#contenido_titulo_val_radicacion").css("height", $('#titulo_label_val_radicacion').height() + 5 + "px");
   // $("#Contenido_botones_tipo_radicado").css("height", $('#Button_Trazabilidad').height() + 10 + "px");
   // $("#contenido_datagrid_val_radicacion").css("height", (espacio_iframe - 10) - ($('#contenido_titulo_val_radicacion').height() + $('#Contenido_botones_tipo_radicado').height()) + "px");

}
//  AUTOSIZE DATA GREVIEW
function plugin_grwedview() {
    var gridwith = $('#contenido_datagrid_val_radicacion').width();
    var gridheihg = $('#contenido_datagrid_val_radicacion').height();
    //LLAMA PLUGIN FIJA HIDER O TITULOS   
    if ($('#GridView_val_radicacion td').children.length > 0) {
        //$(document).ready(function () { $('#GridView_val_radicacion').gridviewScroll({ width: gridwith, height: gridheihg }); })
    }
}
//Retorna el idex de una columna en una tabla
function colum_index(colum_name) {

    var x = $('#GridView_val_radicacion th');
    var txt = "";
    var i;
    for (i = 0; i < x.length; i++) {
        if (x[i].innerText.toUpperCase() == colum_name.toUpperCase()) {

            return i;
        }

    }
    return -1;
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