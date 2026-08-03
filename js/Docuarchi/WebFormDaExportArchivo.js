$(document).ready(function () {
   
    $.fn.inicio = function () {
        redimenciona_marco_descarga();
        $(window.parent).resize(bodyResize);
        $(window).resize(bodyResize);
        function bodyResize() {
           
        }
    }

    
})
$(window).on("load", function () {
    var elment = document.getElementsByClassName("da_event_captive");
    if (elment) {
        for (var i = 0; i < elment.length; i++) {
            elment[i].addEventListener("click", event_click, false);
        }
    }
    window.addEventListener("resize", rezize_event);
    ShowModalPopup("ModalPopupExtender_mensaje_personalizado_backgroundElement", "Panel_mensaje_personalizado", 100001);
    
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
        redimenciona_marco_descarga();
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
function redimenciona_marco_descarga() {
    try {
        var espacio_iframe = 420;
        var hidenpadre = 0;
        var with_frame = 420;
        if (window.parent.innerHeight) {
            //navegadores basados en mozilla 
            espacio_iframe = window.parent.innerHeight;
            with_frame = window.parent.innerWidth;
        } else {
            if (document.parentWindow.body.clientHeight) {
                //Navegadores basados en IExplorer, es que no tengo innerheight 
                espacio_iframe = document.body.clientHeight;
                with_frame = document.parentWindow.body.clientWidth;
            } else {
                //otros navegadores y iframe
                //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val();
            }
        }
        //ifrm_indice_
        if (window.parent.document.getElementById("ifimpre_descarga_anexo_respuesta_")) {
            if (document.getElementById("Table1v")) {
                //window.parent.document.getElementById("Panel_descarga_anexo_respuesta").style.height = (document.getElementById("ContenidoImpresion").clientHeight + document.getElementById("Button_exportar").clientHeight  + 70) + "px";
                //window.parent.document.getElementById("ifimpre_descarga_anexo_respuesta").style.height = (document.getElementById("ContenidoImpresion").clientHeight + document.getElementById("Button_exportar").clientHeight + 50) + "px";
                //document.getElementById("ContenidoImpresion").style.height = (window.parent.document.getElementById("Panel_descarga_anexo_respuesta").clientHeight  - 40)  + "px";
                //document.getElementById("ContenidoImpresion").style.width = window.parent.document.getElementById("Panel_descarga_anexo_respuesta").clientWidth - 50 + "px";
                //window.parent.document.getElementById("ifimpre_descarga_anexo_respuesta").style.height = ((espacio_iframe - (espacio_iframe * 30 / 100)) - 1) + "px";
                //var value_top = (espacio_iframe - window.parent.document.getElementById("Panel_descarga_anexo_respuesta").clientHeight) / 2 + "px";
                //alert(value_top);
                //window.parent.document.getElementById("Panel_descarga_anexo_respuesta").style.top = value_top;

            } else {
                //window.parent.document.getElementById("Panel_descarga_anexo_respuesta").style.height = (document.getElementById("Button_exportar").clientHeight + 60) + "px";
                //window.parent.document.getElementById("ifimpre_descarga_anexo_respuesta").style.height = (document.getElementById("Button_exportar").clientHeight + 40) + "px";
                //document.getElementById("ContenidoImpresion").style.height = (document.getElementById("Button_exportar").clientHeight + 5) + "px";
                //var value_top = (espacio_iframe - window.parent.document.getElementById("Panel_descarga_anexo_respuesta").clientHeight) / 2 + "px";
                //alert(value_top);
                //window.parent.document.getElementById("Panel_descarga_anexo_respuesta").style.top = value_top;
                
            }
           
        }

    }
    catch (err) {
        alert(err.message + " Funcion redimenciona_marco_descarga");
    }
}
function hiden_marco_padre_descarga() {
    try {
        if (window.parent.document.getElementById("Button_cerrar_descarga_anexo_respuesta")) {
            window.parent.document.getElementById("Button_cerrar_descarga_anexo_respuesta").click();
            
        }
    }
    catch (err) {
        alert(err.message + " Funcion hiden_marco_padre_descarga");
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