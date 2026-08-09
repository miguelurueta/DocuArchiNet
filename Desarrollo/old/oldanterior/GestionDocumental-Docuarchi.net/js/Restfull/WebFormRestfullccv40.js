$(document).ready(function () {
    $.fn.inicio = function () {
        auto_zise_popup_paginas_externas_libres();
        //Detecta cuando se dibuja
        $(window).resize(bodyResize);
        function bodyResize() {
            auto_zise_popup_paginas_externas_libres();
        }
    }


})

function solicitarToken() {
    try {
        var empresa = "";
        var usuario = "siidocu";
        var clavews = "intdocusii2018";
        var obj = { "codigoempresa": empresa, "usuariows": usuario, "clavews": clavews };
       
        var jsonData = JSON.stringify(obj);
        $.ajax({
            url: 'https://virtual.ccv.org.co/librerias/wsRestSII/v1/solicitarToken.php',
            type: 'POST',
            data: jsonData,
            dataType: 'json',
            success: function (data) {
                alert("ojo");
                //web_service_solicitudes_ayuda = data;
                //document.getElementById("TextBox_resultado").innerHTML = data;
                //return web_service_solicitudes_ayuda;
            },
            error: function (errorText) {
                alert("Error general funcion axion_script !" + errorText);
            }
        });
    }
    catch (err) {
        alert(err.message + " Funcion solicitarToken");
    }
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

function auto_zise_popup_paginas_externas_libres() {
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

            $('#Panel_paginas_externas_popou').css("height", (espacio_iframe) + "px");
            $('#contenido_procesa_paginas_externas_popou').css("height", (espacio_iframe - 20) + "px");
            $('#Iframe_paginas_externas_popup_').css("height", (espacio_iframe - 20) + "px");



        }
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_paginas_externas_libres");
    }

}