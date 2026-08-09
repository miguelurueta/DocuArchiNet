$(document).ready(function () {
    $.fn.cligred = function () {
        auto_zise_anotacion(0);
        var RowID = $('#hdnEmailID').val();
        var datos = RowID.split("|");
        var arc1 = datos[0];
        if (RowID != "0") {
            $('#GridViewlista tr[id=' + RowID + ']').css({ "background-color": "#e8e8f7", "color": "Black" });
        }
        $('#GridViewlista tr[id]').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });
        $('#GridViewlista tr[id]').click(function () {
            $('#GridViewlista tr[id]').css({ "background-color": "White", "color": "Black" });
            $(this).css({ "background-color": "#e8e8f7", "color": "Black" });
            var fer = $(this).attr("id");
            $('#hdnEmailID').val(fer.toString());

        });
        $('#GridViewlista tr[id]').dblclick(function () {
            $('#GridViewlista tr[id]').css({ "background-color": "White", "color": "Black" });
            $(this).css({ "background-color": "#e8e8f7", "color": "Black" });
            var fer = $(this).attr("id");
            $('#hdnEmailID').val(fer.toString());
            var boton = $('#Buttonclidatos');
            boton.click();
        });

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
        auto_zise_anotacion(0);
    } catch (ex) {
        alert(ex.message + " Función rezize_event")
    }
}
function prevent(event, element) {
    try {

        var fer = $(element).attr("idd");
        var tip_event = $(element).attr("tip_event");
        if (tip_event == "vista_nota") {
            $('#hdnEmailID').val(fer);
            document.getElementById("Buttonclidatos").click();
        }      
        event.preventDefault();

    }
    catch (err) {
        alert(err.message + " Funcion prevent");
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
function prevent_scrol(event, e) {
    try {

        if (e.className == "GridviewScrollItem_line_cort_tr_flex") {
            e.classList.remove("GridviewScrollItem_line_cort_tr_flex");
            e.classList.toggle("GridviewScrollItem_line_corte_tr_flex_scrol");
        } else {
            e.classList.remove("GridviewScrollItem_line_corte_tr_flex_scrol");
            e.classList.toggle("GridviewScrollItem_line_cort_tr_flex");
        }
        event.preventDefault();
    }
    catch (err) {
        alert(err.message + " Funcion prevent_scrol");
    }
}

function ConfirmMensaje(mensaje) {
    var x;
    var r = confirm(mensaje);
    if (r == true) {
        x = "0";

    }
    else {
        x = "1";

    }
    document.getElementById("HiddenPROMP").value = x;


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
        alert(err.message + " funcion posicion_update_pogres " + err.message);
    }

}
function progres_hiden(progres) {
    $("#progres_bar").css("display", "none");
}
function labe_texto_modal_nota(nota) {
    document.getElementById("Label_nota_respuesta").innerText = nota;
}

function auto_zise_anotacion(porcentaje) {
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

        $('#Listnotacion').css("height", (espacio_iframe - 30)  + "px");
        $('#Panelactividad').css("height", (espacio_iframe - 25)  + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_pendinetes " + err.message);
    }
}

