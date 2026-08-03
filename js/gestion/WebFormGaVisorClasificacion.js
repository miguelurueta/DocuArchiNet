$(document).ready(function () {
    $("#noaming").bind("contextmenu", function (e) {
        e.preventDefault();
    });
    $(document).bind("contextmenu", function (e) {
        e.preventDefault();
    });
    $(document).keyup(function (tecla) {
        if (tecla.keyCode == 18) {
            isCtrl = true;
        }

    });
    $(document).keydown(function (tecla) {

        if (tecla.keyCode == 109) {
            document.getElementById("ImageMenos").click();
        }

        if (tecla.keyCode == 107) {
            document.getElementById("ImageMas").click();
        }
        if (tecla.keyCode == 68) {
            document.getElementById("ImageButtonInicio").click();
            //isCtrl = false;
        }
        if (tecla.keyCode == 65) {
            document.getElementById("ImageButtonAnterior").click();
            //isCtrl = false;
        }
        if (tecla.keyCode == 83) {
            document.getElementById("ImageButtonSiguiente").click();
            //isCtrl = false;
        }
        if (tecla.keyCode == 70) {
            document.getElementById("ImageButtonFinal").click();
            //isCtrl = false;
        }
        if (tecla.keyCode == 71) {
            document.getElementById("ImageButtonguardardocumento").click();
            //isCtrl = false;
        }
        if (tecla.keyCode == 71) {
            document.getElementById("ImageButtonguardardocumento").click();
            //isCtrl = false;
        }
        if (tecla.keyCode == 80) {
            document.getElementById("ImageButtonimprimir").click();
            //isCtrl = false;
        }
        if (tecla.keyCode == 82) {
            document.getElementById("ImageRotate45").click();
            //isCtrl = false;
        }
        if (tecla.keyCode == 69) {
            document.getElementById("ImageButtoninfo").click();
            //isCtrl = false;
        }

        if (tecla.keyCode == 73) {
            event_click_indice();
            //document.getElementById("ImageButtonindice").click();
            //isCtrl = false;
        }
    });
    $.fn.inicio = function () {
        determina_visor();
        auto_zise_visor();
        controla_botones_permiso_visor();
        auto_zise_ubicacion_toponimica();
        visualiza_indice_documento_rezise(0);
        auto_zise_descarga_documento();
        auto_zise_imprime_documento();
       
    }


})
var isCtrl = false;
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
function rezize_event() {
    try {
        auto_zise_visor();
        visualiza_indice_documento_rezise(0);
        auto_zise_ubicacion_toponimica();
        auto_zise_descarga_documento();
        auto_zise_imprime_documento();
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
function event_click_indice(e, sender) {
    try {
        var d = document.getElementById("ImageButtonindice");
        if (document.getElementById("cuerpoindice").style.display == "none") {
            $("#id_indice_image").toggleClass("fa-info");
            $("#id_indice_image").toggleClass("fa-bars");
            document.getElementById("id_a_indice").title = "Oculta indice documento (tecla I)";
            d.click();
        } else {
            $("#id_indice_image").toggleClass("fa-info");
            $("#id_indice_image").toggleClass("fa-bars");
            $('#cuerpoindice').css("display", "none");
            $('#Ocultaindice').css("display", "none");
            $('#cuerpoindice').css("width", "0%");
            $('#content').css("width", "100%");
            document.getElementById("id_a_indice").title = "Visualiza indice documento (tecla I)";
        }
        if (e) {
            e.preventDefault();
        }

    }
    catch (err) {
        alert(err.message + " Funcion event_click_indice");
    }
}
function preven_event_search_keypres_enter(e, sender) {
    try {
        tecla = (document.all) ? e.keyCode : e.which;
        if (tecla == 13) {
            document.getElementById("ImageButton_ir_pagina").click();
            e.preventDefault();
        }
    } catch (err) {
        alert(err.message + " funcion preven_event_search_keypres_enter " + err.message);
    }
}

function controla_botones_permiso_visor() {
    try {
        if (document.getElementById("ImageButtonindice")) {

        } else {
            document.getElementById("ImageButtonindice_").style.display = "none";
        }
        if (document.getElementById("ImageButtonguardardocumento")) {

        } else {
            document.getElementById("ImageButtonguardardocumento_").style.display = "none";
        }
        if (document.getElementById("ImageButton_ir_pagina")) {

        } else {
            document.getElementById("ImageButton_ir_pagina_").style.display = "none";
        }
       
    }
    catch (ex) {
        alert("Error general funcion controla_botones_permiso_visor " + ex.message)
    }
}
function visualiza_indice_documento(estado_display) {
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

        $('#cuerpoindice').css("width", "330px");
        $('#content').css("width", with_frame - (330 + 60) + "px");
        //$('#cuerpoindice').css("width", "30%");
        //$('#content').css("width",  "67%");
        if (estado_display == 1) {
            if ($('#cuerpoindice').is(":visible")) {
                $('#cuerpoindice').css('display', 'none');
                $('#Ocultaindice').css("display", "none");
                $('#cuerpoindice').css("width", "0%");
                $('#content').css("width", "98%");
            } else {
                $('#cuerpoindice').css('display', 'block');
            }

        }
    }
    catch (ex) {
        alert("Inconsistencia general función visualiza_indice_documento " + ex.message)
    }
}
function visualiza_indice_documento_rezise(estado_display) {
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
        if ($('#cuerpoindice').is(":visible")) {
            $('#cuerpoindice').css("width", "330px");
            $('#content').css("width", with_frame - (350 + 7) + "px");
        }

    }
    catch (ex) {
        alert("Inconsistencia general función visualiza_indice_documento " + ex.message)
    }
}
function auto_zise_ubicacion_toponimica() {
    try {
        var espacio_iframe;
        var hidenpadre;
        var with_frame;
        if (window.innerHeight) {
            //navegadores basados en mozilla 
            espacio_iframe = window.innerHeight
        } else {
            if (document.body.clientHeight) {
                //Navegadores basados en IExplorer, es que no tengo innerheight 
                with_frame = window.innerWidth;
                espacio_iframe = window.innerHeight;
            } else {
                //otros navegadores y iframe
                //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val();

                espacio_iframe = document.body.clientHeight;
                with_frame = document.body.clientWidth;

            }
        }

        $("#Panel_ubicacion_toponimica_expediente_popup").css("height", (espacio_iframe - 70) + "px");
        $("#Contenido_ubicacion_toponimica_expediente").css("height", (espacio_iframe - 70) + "px");
        var heigconetedor = 0;
        $("#contendor_botones_unidad_u_b_t").css("height", ($("#Button_exportar").height() + 10) + "px");
        heigconetedor = $("#Panel_ubicacion_toponimica_expediente_popup").height() - ($("#contendor_botones_unidad_u_b_t").height());
        $("#div_treview_archivo_u_b_t").css("height", (heigconetedor) + "px");
        $("#Paneltreview_u_b_t").css("height", (heigconetedor) + "px");
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_reasigna_expe_unidad");
    }
}
function auto_zise_descarga_documento() {
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

    var heigconetedor = (espacio_iframe - 20);
    $('#Panel_descarga_anexo_respuesta').css("height", heigconetedor + "px");
    $('#contenido_procesa_descarga_anexo_respuesta').css("height", (document.getElementById("Panel_descarga_anexo_respuesta").clientHeight - (document.getElementById("div_title_descarga").clientHeight + 20)) + "px");
    $('#ifimpre_descarga_anexo_respuesta_').css("height", (document.getElementById("Panel_descarga_anexo_respuesta").clientHeight - (document.getElementById("div_title_descarga").clientHeight + 30)) + "px");

}
function auto_zise_imprime_documento() {
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

    var heigconetedor = (espacio_iframe - 20);
    $('#ContenidoImpresion_post').css("height", (document.getElementById("Panelimpresionpost").clientHeight - (document.getElementById("divcabecer2_post").clientHeight + 20)) + "px");
    $('#ifimpre_post_').css("height", (document.getElementById("Panelimpresionpost").clientHeight - (document.getElementById("divcabecer2_post").clientHeight + 30)) + "px");

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
function determina_visor() {
    try {
    if (document.getElementById("Hidden_tipo_visor_externo").value == "0") {
        document.getElementById("Contendor_derecho").style.display = "block";
        document.getElementById("div_contendor_externo").style.display = "none";
    } else {
        document.getElementById("Contendor_derecho").style.display = "none";
        document.getElementById("div_contendor_externo").style.display = "block";
    }
    }
    catch (err) {
        alert(err.message + " determina_visor");
    }
}
function auto_zise_visor() {
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
    if (window.parent.document.getElementById("Iframe_visor_externo_clasficacion_")) {
        espacio_iframe = window.parent.document.getElementById("Iframe_visor_externo_clasficacion_").clientHeight - 10;
    }
    //Iframe_visor_externo_  
    $('#Contendor_derecho').css("height", (espacio_iframe - 40) + "px");
    var heigt = document.getElementById("Contendor_derecho").clientHeight - (document.getElementById("tollimage").clientHeight + document.getElementById("Pietolbar").clientHeight)
    $("#content").css("height", (heigt) + "px");
    $('#div_contendor_externo').css("height", (espacio_iframe - 40) + "px");
    $('#cuerpoindice').css("height", (espacio_iframe - 50) + "px");
    $('#ifrm_indice_visor_docuarchi_').css("height", (espacio_iframe - 55) + "px");
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