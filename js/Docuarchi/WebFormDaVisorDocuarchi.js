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
        if ( tecla.keyCode == 68) {
            document.getElementById("ImageButtonInicio").click();
            //isCtrl = false;
        }
        if ( tecla.keyCode == 65) {
            document.getElementById("ImageButtonAnterior").click();
            //isCtrl = false;
        }
        if ( tecla.keyCode == 83) {
            document.getElementById("ImageButtonSiguiente").click();
            //isCtrl = false;
        }
        if ( tecla.keyCode == 70) {
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
        auto_zise_visor_externo();
        visualiza_indice_documento_rezise(0);
        auto_zise_descarga_documento();
        auto_zise_imprime_documento();
        auto_zise_popup_consulta_meta_dato();
    }

    $('#Ocultaindice').click(function () {
        $('#cuerpoindice').css("display", "none");
        $('#Ocultaindice').css("display", "none");
        $('#cuerpoindice').css("width", "0%");
        $('#content').css("width", "100%");
    });
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
        auto_zise_visor_externo();
        visualiza_indice_documento_rezise(0);
        auto_zise_descarga_documento();
        auto_zise_imprime_documento();
        auto_zise_popup_consulta_meta_dato();
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
function event_click_indice(e,sender) {
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
function event_click_indice_meta_dato(e, sender) {
    try {
        ID_IMAGEN_VIS_WF = document.getElementById("Hidden_imagen").value;
        GABIENTE_VIS_WF = document.getElementById("Hidden_gabinete").value;
        Service_Solicita_listar_meta_datos_Archivo(ID_IMAGEN_VIS_WF, GABIENTE_VIS_WF);
        if (e) {
            e.preventDefault();
        }

    }
    catch (err) {
        alert(err.message + " Funcion event_click_indice_meta_dato");
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
function actualiza_indice_padre() {
    try {
        var boton_indice = window.parent.document.getElementById("Button_actualiza_indice")
        if (boton_indice != undefined) {
          
                boton_indice.click();             
        }
        
    }
    catch (err) {
        alert(err.message + " Funcion actualiza_indice_padre");
    }
}
function asignacion() {
    try {
        var namegabinete = $('#Hidden_gabinete', window.parent.document);
        var nameimagen = $('#hdnEmailID_VAL', window.parent.document);
        
        //  Hidden_gabinete
        if (namegabinete.length > 0) {
            document.getElementById("Hidden_gabinete").value = namegabinete[0].value;
        }
        if (nameimagen.length > 0) {
            document.getElementById("Hidden_imagen").value = nameimagen[0].value;
        }
       
    }
          catch (err) {
              alert(err.message + " Funcion asignacion");
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
function auto_zise_popup_consulta_meta_dato() {
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
        var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_interface_consulta_meta_dato').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_consulta_meta_dato').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#contenido_procesa_interface_consulta_meta_dato').css("height", (document.getElementById("modal_content_consulta_meta_dato").clientHeight - (document.getElementById("divcabecer2_interface_consulta_meta_dato").clientHeight)) + "px");
        $('#div_content_tabla').css("height", (document.getElementById("modal_content_consulta_meta_dato").clientHeight - (document.getElementById("divcabecer2_interface_consulta_meta_dato").clientHeight)) + "px");
        $('#table_meta_row').bootstrapTable('resetView', { height: (document.getElementById("contenido_procesa_interface_consulta_meta_dato").clientHeight - 30) });

    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_consulta_meta_dato " + err.message);
    }
}
function auto_zise_visor_externo() {
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
    if (window.parent.document.getElementById("Iframe_visor_externo_da_")) {
        espacio_iframe = window.parent.document.getElementById("Iframe_visor_externo_da_").clientHeight - 30;
    }  
    var heigconetedor = (espacio_iframe );
    $('#ContentGeneral').css("height", heigconetedor + "px");
    //$('#tollimage').css("minimo-height",  "50px");
    $('#cuerpoindice').css("height", ((heigconetedor - document.getElementById("tollimage").clientHeight) - 10) + "px");
    $('#ifrm_indice_visor_docuarchi_').css("height", ((heigconetedor - document.getElementById("tollimage").clientHeight) - 15) + "px");
    $('#content').css("height", ((heigconetedor - document.getElementById("tollimage").clientHeight) - 10) + "px");
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
    //$('#Panelimpresionpost').css("height", heigconetedor + "px");
    $('#ContenidoImpresion_post').css("height", (document.getElementById("Panelimpresionpost").clientHeight - (document.getElementById("divcabecer2_post").clientHeight + 20)) + "px");
    $('#ifimpre_post_').css("height", (document.getElementById("Panelimpresionpost").clientHeight - (document.getElementById("divcabecer2_post").clientHeight + 30)) + "px");

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
        
        $('#cuerpoindice').css("width",  "320px");
        $('#content').css("width", with_frame - (320 + 7) + "px");
        if (estado_display == 1) {
            $('#cuerpoindice').css('display', 'block');
            $('#Ocultaindice').css('display', 'block');

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
            $('#cuerpoindice').css("width", "320px");
            $('#content').css("width", with_frame - (320 + 7) + "px");           
        }
       
    }
    catch (ex) {
        alert("Inconsistencia general función visualiza_indice_documento " + ex.message)
    }
}