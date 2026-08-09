$(document).ready(function () {
    $.fn.inicio = function () {
        auto_zise_popup_lista_tramites(1, 1);
        
    }  
});
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
        auto_zise_popup_lista_tramites(1, 1);
       
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
function auto_zise_popup_lista_tramites(value_lista_general, value_lista_usuario) {
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

        //$('#div_contendor_principal').css("height", (espacio_iframe - 10) + "px");
        //$('#div_contendor_principal').css("wdth", (with_frame - 10) + "px");
        //$('#div_contendor_filtro_listado').css("height", ((document.getElementById("div_filtro__fil").clientHeight + 5)) + "px");
        //var total =  document.getElementById("navar_barra").clientHeight + document.getElementById("contenido_titulo_listado_solicitudes").clientHeight; 
        //var gridwith = with_frame - 10;
        var gridheihg_ = (espacio_iframe - (document.getElementById("navar_barra").clientHeight + document.getElementById("contenido_titulo_listado_solicitudes").clientHeight));
        //$('#contenido_titulo_listado_solicitudes').css("height", gridheihg_ + "px");
        //$('#content_grid').css("height", gridheihg_ + "px");
        $('#Panel_principal').css("height", (gridheihg_ - 100) + "px");

    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_lista_tramites " + err.message);
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
function progres_hiden(progres) {
    $("#progres_bar").css("display", "none");
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
function activa_export_lista(hiden_name, nombre_gred) {
    try {
        var hiden = document.getElementById(hiden_name);
        var nombre_gred;
        var x = $('#' + nombre_gred + ' th');
        var txt = "";
        var i;
        for (i = 1; i < x.length; i++) {
            txt = txt + x[i].innerText.toUpperCase() + "|";
        }
        hiden.value = txt;
        //document.getElementById("Button_export_lista_event").click();
        return txt;
    }
    catch (err) {
        alert(err.message + " Funcion activa_export_lista");
    }
}