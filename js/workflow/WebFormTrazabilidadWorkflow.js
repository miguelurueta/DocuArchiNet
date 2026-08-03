$(document).ready(function () {
    $.fn.inicio = function (ini) {
       
        //auto_zise_popup_validacion_radicados();
        plugin_grwedview();
        resize_detalle_transacion();
    }

    
})
$(window).on("load", function () {  
    window.addEventListener("resize", rezize_event);
});
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
function rezize_event() {
    try {
        //auto_zise_popup_validacion_radicados();
        plugin_grwedview();
        resize_detalle_transacion();
    } catch (ex) {
        alert(ex.message + " Función rezize_event")
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
function onDataShown(sender, args) {
    sender._popupBehavior._element.style.zIndex = 1000001;

}
function prevent(event, element) {
    try {
        event.preventDefault();
        //document.getElementById("HiddenFi").value = "YES";
        //var g = element;
        var fer = $(element).attr("idd");
        $('#hdnEmailID_VAL').val(fer);
        document.getElementById("Button_detalle").click();
        //auto_zise_popup_visor_externo();
        element.focus();
    }
    catch (err) {
        alert(err.message + " Funcion prevent");
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
  
    if ($('#Iframe_visor_externo_', window.parent.document)) {
        espacio_iframe = $('#Iframe_visor_externo_', window.parent.document).height();
    }
    if ($('#Iframe_trazabilidad_', window.parent.document)) {
        espacio_iframe = $('#Iframe_trazabilidad_', window.parent.document).height();
    }
    $('#Contenedorderecho').css("height", (espacio_iframe - 70) + "px");
    var heigconetedor = $("#Contenedorderecho").height() - (($("#Contenedorderecho").height() * 15) / 100);
    $("#contenido_datagrid_val_radicacion").css("height", (heigconetedor) + "px");
    // $("#contenido_controles_consulta").css("height", (heigconetedor) + "px");
    //$("#_Panelvalidacion_val_radicacion").css("height", (heigconetedor - 10) + "px");
    heigconetedor = $("#Contenedorderecho").height() - (($("#Contenedorderecho").height() * 89) / 100);
    $("#Contenido_botones_tipo_radicado").css("height", (heigconetedor) + "px");
    //$("#contenido_controles_buton_consulta").css("height", (heigconetedor) + "px");
    heigconetedor = $("#Contenedorderecho").height() - (($("#Contenedorderecho").height() * 96) / 100);
    $("#contenido_titulo_controles_consulta").css("height", (heigconetedor) + "px");
    

}
//  AUTOSIZE DATA GREVIEW contenido_datagrid_val_radicacion 
function plugin_grwedview() {
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
        $("#Contenedorderecho").css("height", (espacio_iframe - 40)  + "px");
        var gridheigth = (document.getElementById("Contenedorderecho").clientHeight - 20) - (document.getElementById("contenido_titulo_val_radicacion").clientHeight + document.getElementById("Contenido_botones_tipo_radicado").clientHeight);
        $("#Panel_gred").css("height", gridheigth + "px");
        $("#contenido_datagrid_val_radicacion").css("height", gridheigth + "px");
       
    } catch (ex) {
        alert(ex.message + " funcion plugin_grwedview")
    }
}
function resize_detalle_transacion() {
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
                //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val(); Panel_detalle_conector_trazabilidad 

            }
        }       
        $("#Panel_detalle_conector_trazabilidad").css("height", (espacio_iframe - 1) + "px");
        var heigth = document.getElementById("Panel_detalle_conector_trazabilidad").clientHeight - (document.getElementById("Divcerrarbuton2_detalle_conector_trazabilidad").clientHeight + document.getElementById("conten_detalle_traza").clientHeight);
        $("#content_detalle").css("height", heigth + "px");
       
    } catch (ex) { alert(ex.message + " funcion resize_detalle_transacion") }
}
function retorna_colum_mtriz(hiden_name) {
    try {
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
    catch (err) {
        alert(err.message + " Funcion retorna_colum_mtriz");
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

