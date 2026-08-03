$(document).ready(function () {
    $.fn.inicio = function () {      
    }
   

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
function mostrarPassword() {
    var cambio = document.getElementById("ContentPlacenter_TextBoxpasw");
    if (cambio.type == "password") {
        cambio.type = "text";
        $('.icon').removeClass('fa fa-eye-slash').addClass('fa fa-eye');
    } else {
        cambio.type = "password";
        $('.icon').removeClass('fa fa-eye').addClass('fa fa-eye-slash');
    }
}
function sesion_cli() {
    try {
        window.location.assign("Publico/WebFormDefaultPublico.aspx");
    } catch (ex) {
        alert("Error función sesion_cli " + ex.message)
    }
};

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


    function activa_boton() {
        if (document.getElementById("ContentPlacenter_CheckBox_privado").checked == true) {
            document.getElementById("ContentPlacenter_Button_sesion").click();
        } else {
            var panel = document.getElementById("ContentPlacenter_Panel_sesion_privado_login");
            if (panel != null) {
                document.getElementById("ContentPlacenter_Panel_sesion_privado_login").style.display = "none";
            }
        }
        if (document.getElementById("ContentPlacenter_Check_publico").checked == true) {
            document.getElementById("ContentPlacenter_Button_sesion_empresa").click();
        }

    }
    function llenarmodulos() {

        var drowplist = document.getElementById("ContentPlacenter_DropDownListempresa");
        var idsel = document.getElementById("ContentPlacenter_Hiddenempresagestion");
        if (drowplist.selectedIndex != -1) {
            idsel.value = drowplist.options[drowplist.selectedIndex].text;
            var boton = document.getElementById("ContentPlacenter_Buttonlistarmodulo");
            boton.click();
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