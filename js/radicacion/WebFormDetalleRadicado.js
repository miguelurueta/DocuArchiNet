$(document).ready(function () {
    $.fn.inicio = function () {
        auto_zise_popup_validacion_radicados();
    }
})
$(window).on("load", function () {
    window.addEventListener("resize", rezize_event);
});
function rezize_event() {
    try {
        auto_zise_popup_validacion_radicados();
    } catch (ex) {
        alert(ex.message + " Función rezize_event")
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
function on_clik() {
    try {
        document.getElementById("Button_activa_detalle_tramite").click();
    }
    catch (err) {
        alert(err.message + " Funcion on_clik");
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
    espacio_iframe = espacio_iframe - 20;
    $("#contenido_detallle_radicado").css("height", (espacio_iframe) + "px");
    $('#contenido_titulo_detalle_respuesta').css("height", ((document.getElementById("row-title").clientHeight + 30)) + "px");
    $('#contenido_botones').css("height", ((document.getElementById("Button_generar").clientHeight + 30)) + "px");
    var heigconetedor = document.getElementById("contenido_detallle_radicado").clientHeight - (document.getElementById("contenido_titulo_detalle_respuesta").clientHeight + document.getElementById("contenido_botones").clientHeight);
    $('#div_gabinetes').css("height", (heigconetedor - 10) + "px");
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