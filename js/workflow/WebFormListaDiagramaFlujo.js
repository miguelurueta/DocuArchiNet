$(document).ready(function () {
    $.fn.inicio = function () {
        auto_zise_ventana_diagrama();
        //Detecta cuando se dibuja
        $(window).resize(bodyResize);
        function bodyResize() {
            auto_zise_ventana_diagrama();
            
        }
    }


})
function auto_zise_ventana_diagrama() {
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

        var client_keigth = document.getElementById("header").clientHeight + document.getElementById("menucab").clientHeight + document.getElementById("footer").clientHeight;
        $('#content').css("height", (espacio_iframe - (client_keigth + 60)) + "px");

    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_ventana_diagrama");
    }

}
function onNodeSelected(sender, args) {
    try {
        if (event.ctrlKey) {
            if (document.getElementById("HiddenField_value_selecion").value == "") {
                document.getElementById("HiddenField_value_selecion").value = args.getNode().getText();
            } else {
                document.getElementById("HiddenField_value_selecion").value = document.getElementById("HiddenField_value_selecion").value + "|" + args.getNode().getText();
            }

        } else {
            //document.getElementById("HiddenField_value_selecion").value = args.getNode().getText();
            //var f = args.getNode();
            //alert(f.id);
        }
    }
    catch (err) {
        alert(err.message + " Funcion onNodeSelected");
    }
}
function onLinkSelected(sender, args) {
    try {
        if (event.ctrlKey) {
            if (document.getElementById("HiddenField_value_selecion").value == "") {
                document.getElementById("HiddenField_value_selecion").value = args.getNode().getText();
            } else {
                document.getElementById("HiddenField_value_selecion").value = document.getElementById("HiddenField_value_selecion").value + "|" + args.getNode().getText();
            }

        } else {
            //document.getElementById("HiddenField_value_selecion").value = args.getNode().getText();
            //var f = args.link;
            //alert(f.id);
        }
    }
    catch (err) {
        alert(err.message + " Funcion onLinkSelected");
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
