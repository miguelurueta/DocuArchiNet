$(document).ready(function () {
    $.fn.inicio = function () {
        auto_zise_reportes();
    }
   
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
        auto_zise_reportes();
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
        if (tip_event == "a_nuevo_radic") {
            $('#hdnEmailID_VAL').val(fer);
            document.getElementById("Button_Asignar_nuevo_radicado").click();
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

function auto_zise_reportes() {
    try {
    var espacio_iframe = 420;
    var hidenpadre = 0;
    var with_frame = 420;
    if (window.innerHeight) {
        //navegadores basados en mozilla 
        espacio_iframe = window.parent.innerHeight;
        with_frame = window.innerWidth;
    } else {
        if (document.body.clientHeight) {
            //Navegadores basados en IExplorer, es que no tengo innerheight 
            espacio_iframe = document.parentWindow.body.clientHeight;
            with_frame = document.body.clientWidth;
        } else {
            //otros navegadores y iframe
            //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val();
        }
    }
   
    /*var value = 100;
    var value_fot = 60;
    var value_pie = 30;
    if (window.parent.document.getElementById("cabecera")) {
        value_fot = window.parent.document.getElementById("cabecera").clientHeight;
    }
    if (window.parent.document.getElementById("Piepagina")) {
        value_pie = window.parent.document.getElementById("Piepagina").clientHeight;
    }
    value = value_fot + value_pie + 20;
    if (window.parent.document.getElementById("ContentPlacenter_ifrm_ds")) {
        var paren_ifrms = window.parent.document.getElementById("ContentPlacenter_ifrm_ds");
        $(paren_ifrms).css("height", (espacio_iframe - value) + "px");
    }*/

    if (window.parent.document.getElementById("ContentPlacenter_ifrm_ds_")) {
        espacio_iframe = window.parent.document.getElementById("ContentPlacenter_ifrm_ds_").clientHeight ;
    }
    $('#contendor_reporte').css("height", (espacio_iframe) + "px");
    $('#derecha').css("height", (espacio_iframe) + "px");
    var heigt = (espacio_iframe) - (((espacio_iframe) * 10) / 100);
    $('#resultados').css("height", (heigt - 10) + "px");
    heigt = (espacio_iframe) - (((espacio_iframe) * 90) / 100);
    $('#Opciones').css("height", (heigt - 10) + "px");
    $('#div_pan_radicacion').css("height", (document.getElementById("resultados").clientHeight - (document.getElementById("div_resultado").clientHeight + document.getElementById("Opciones").clientHeight)) + "px");
    //$('#reportes').css("height", (espacio_iframe) - (((espacio_iframe) * 60) / 100))
    $('#Panel_parametros_consulta').css("height", (document.getElementById("parametros").clientHeight - 5) + "px");
    $('#Panel_reportes').css("height", (document.getElementById("reportes").clientHeight - (document.getElementById("div_title_reportes").clientHeight + 5)) + "px");
    /*$('#contendor_reporte').css("height", (espacio_iframe - value) + "px");
    $('#derecha').css("height", (espacio_iframe - value) + "px");
    var heigt = (espacio_iframe - value) - (((espacio_iframe - value) * 10) / 100);
    $('#resultados').css("height", (heigt - 10) + "px");
    heigt = (espacio_iframe - value) - (((espacio_iframe - value) * 90) / 100);
    $('#Opciones').css("height", (heigt) + "px");*/
} catch (ex) {
    alert("Error funcion auto_zise_reportes " + ex.message)
}
}
function onDataShown(sender, args) {
    sender._popupBehavior._element.style.zIndex = 1000001;

}