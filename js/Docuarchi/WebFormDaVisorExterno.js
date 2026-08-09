$(document).ready(function () {
   
    $(document).bind("contextmenu", function (e) {
        e.preventDefault();
    });
    
    $.fn.inicio = function () {
        asignacion();
        auto_zise_visor_externo();
        visualiza_indice_documento_rezise();
        auto_zise_popup_consulta_meta_dato();
    }

    $('#Ocultaindice').click(function () {
        $('#cuerpoindice').css("display", "none");
        $('#Ocultaindice').css("display", "none");
        $('#cuerpoindice').css("width", "0%");
        $('#content').css("width", "99%");
        resize_contenedor();
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
        auto_zise_visor_externo();
        visualiza_indice_documento_rezise();
        auto_zise_popup_consulta_meta_dato();
    } catch (ex) {
        alert(ex.message + " Función rezize_event")
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
        if (namegabinete.length > 0) {
            document.getElementById("Hidden_gabinete").value = namegabinete[0].value;
        }
        if (nameimagen.length > 0) {
            document.getElementById("Hidden_imagen").value = nameimagen[0].value;
        }
        
    }
    catch (err) {
        alert(err.message + " Funcion asignacion ojo");
    }
}
function event_click_indice_meta_dato(e, sender) {
    try {
        ID_IMAGEN_VIS_WF = document.getElementById("Hidden_imagen_").value;
        GABIENTE_VIS_WF = document.getElementById("Hidden_gabinete_").value;
        Service_Solicita_listar_meta_datos_Archivo(ID_IMAGEN_VIS_WF, GABIENTE_VIS_WF);
        if (e) {
            e.preventDefault();
        }

    }
    catch (err) {
        alert(err.message + " Funcion event_click_indice_meta_dato");
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
    if (window.parent.document.getElementById("Iframe_visor_externo_da_")) {
        espacio_iframe = window.parent.document.getElementById("Iframe_visor_externo_da_").clientHeight - 30;
    }
    var heigconetedor = (espacio_iframe);
    $('#ContentGeneral').css("height", heigconetedor + "px");
    $('#tollimage').css("height", "40px");
    $('#cuerpoindice').css("height", ((heigconetedor - document.getElementById("tollimage").clientHeight) - 10) + "px");
    $('#ifrm_visor_').css("height", ((heigconetedor - document.getElementById("tollimage").clientHeight) - 11) + "px");
    $('#content').css("height", ((heigconetedor - document.getElementById("tollimage").clientHeight) - 10) + "px");
   
}
    catch (err) {
        alert(err.message + " Funcion auto_zise_visor_externo");
}
}
function resize_contenedor() {
    $('#ifrm_visor_').css("height", $('#content').height() + "px");
    $('#ifrm_visor_').css("width", $('#content').width() + "px");
}
function visualiza_indice_documento() {
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
        } else {
            $('#cuerpoindice').css("width", "320px");
            $('#content').css("width", with_frame - (320 + 7) + "px");
            $('#ifrm_visor_').css("width", with_frame - (320 + 10) + "px");
            $('#cuerpoindice').css('display', 'block');
            $('#Ocultaindice').css('display', 'none');
        }
    
    }
    catch (ex) {
        alert(ex.message + " Fuccion java visualiza_indice_documento")
    }
}
function event_click_indice(e, sender) {
    try {
        var d = document.getElementById("ImageButtonindice");
        if (document.getElementById("cuerpoindice").style.display == "none") {
            $("#indice_title").toggleClass("fa-info");
            $("#indice_title").toggleClass("fa-bars");
            document.getElementById("ImageButtonindice_").title = "Oculta indice documento ";
            d.click();
        } else {
            $("#indice_title").toggleClass("fa-info");
            $("#indice_title").toggleClass("fa-bars");
            $('#cuerpoindice').css("display", "none");
            $('#Ocultaindice').css("display", "none");
            $('#cuerpoindice').css("width", "0%");
            $('#content').css("width", "99%");
            document.getElementById("ImageButtonindice_").title = "Visualiza indice documento ";
            resize_contenedor();
        }
        if (e) {
            e.preventDefault();
        }

    }
    catch (err) {
        alert(err.message + " Funcion event_click_indice");
    }
}
function visualiza_indice_documento_rezise() {
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
            $('#content').css("width", with_frame - (320 + 20) + "px");
            $('#ifrm_visor_').css("width", with_frame - (320 + 21) + "px");
        }
     
    }
    catch (ex) {
        alert("Inconsistencia general función visualiza_indice_documento " + ex.message)
    }
}