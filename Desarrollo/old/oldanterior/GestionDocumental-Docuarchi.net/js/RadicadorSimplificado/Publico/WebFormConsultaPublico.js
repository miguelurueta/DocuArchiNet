$(document).ready(function () {
    $.fn.inicio = function () {
        auto_zise_popup_consulta();
        
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
        auto_zise_popup_consulta();
    } catch (ex) {
        alert(ex.message + " Función rezize_event")
    }
}
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
function auto_zise_popup_consulta() {
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
                //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val(); Contenido_consulta_documento tol_pie

            }
        }    
       /* var heigconetedor = (espacio_iframe - 40) - (((espacio_iframe - 40) * 1) / 100);
        //var widthconetedor = (with_frame) - (((with_frame - 1) * 1) / 100);
        var widthconetedor = with_frame;
        $('#Panel_consulta_documento').css("height", heigconetedor + "px");
        $('#Contenido_consulta_documento').css("height", heigconetedor + "px");
        $('#ifimpre_consulta_documento').css("height", heigconetedor + "px");
        $('#Panel_consulta_documento').css("width", widthconetedor + "px");
        $('#Contenido_consulta_documento').css("width", widthconetedor + "px");
        $('#ifimpre_consulta_documento').css("width", widthconetedor + "px");*/

        //corrige el posicionamiento horizontal del modalpopuextender, la propiedad width del panel debe estar en auto
        //var widtth_procent_left_rigth = (with_frame - document.getElementById("Panel_agregar_expediente_carpeta").clientHeight) / 2;
        //$('#Panel_agregar_expediente_carpeta').css("left", (Math.round(widtth_procent_left_rigth)) + "px");
        var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_consulta_documento').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_Panel_consulta_documento').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#Contenido_consulta_documento').css("height", (document.getElementById("modal_content_Panel_consulta_documento").clientHeight - (document.getElementById("divcabecer_consulta_documento").clientHeight + 1)) + "px");
        //Para los modal que contiene gred
        $('#ifimpre_consulta_documento_').css("height", (document.getElementById("Contenido_consulta_documento").clientHeight - 5) + "px");

    }
    catch (err) {
        alert(err.message + " funcion auto_zise_docuarchi " + err.message);
    }
}

function auto_zise_docuarchi() {
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
        var heigconetedor = (espacio_iframe - 5) - (((espacio_iframe - 5) * 95) / 100);
        //$('#menu_tulbar').css("height", heigconetedor + "px");
        $('#bar_herramineta').css("height", heigconetedor + "px");
        heigconetedor = (espacio_iframe - 5) - (((espacio_iframe - 5) * 10) / 100);
        $('#area_trabjo').css("height", heigconetedor + "px");
        //heigconetedor = (espacio_iframe - 25) - (((espacio_iframe - 25) * 15) / 100);
        //$('#div_carpetas').css("height", heigconetedor + "px");
        heigconetedor = (espacio_iframe - 5) - (((espacio_iframe - 5) * 95) / 100);
        $('#tol_pie').css("height", heigconetedor + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_docuarchi " + err.message);
    }
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