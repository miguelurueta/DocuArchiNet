$(document).ready(function () {
    $.fn.inicio = function () {
        auto_zise_visor();
        
        if ($.browser.webkit) {
            //asig();
        } 
        $(document).ready(bodyResize);
        $(window).resize(bodyResize);
        function bodyResize() {
            auto_zise_visor();
        }

    }


})
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

    $('#Contendor_derecho').css("height", (espacio_iframe - 40) + "px");
    heigt = (espacio_iframe - 40) - (((espacio_iframe - 40) * 10) / 100);
    $("#content").css("height", (heigt) + "px");
    $("#ifrm_visor_").css("height", (heigt) + "px");
    var widt = with_frame - 20;
    $("#ifrm_visor_").css("width", (widt) + "px");
   

}

function asig() {
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
            if ($('#ifrm_visor_')) {
                //var he = document.getElementById("tollimage").offsetHeight;
                if ($('#ifrm_visor_').contents().find("body").length > 0) {          
                    $('#ifrm_visor_').contents().find("body").css("height", ((espacio_iframe - 40)) + "px");
                    if ($('#ifrm_visor_').contents().find("body").contents().find("embed").length > 0) {
                        $('#ifrm_visor_').contents().find("body").contents().find("embed").css("height", (espacio_iframe - 40) + "px");
                        //clearInterval(asignar);
                        //return;
                    }
                    
                }
           

            }
       
    }
    catch (ex) {
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