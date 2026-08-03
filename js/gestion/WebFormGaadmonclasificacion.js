$(document).ready(function () {
    $.fn.inicio = function () {
        
        if (ESTATE == 0) {
            ESTATE = 1;
            auto_zise();
        }
        $('#Paneltreview').contextMenu('context-menu', {

            'Agregar cuadro de clasficación': {
                click: function (element) {
                    document.getElementById("Button_activa_agregar_cuadro_clasificacion").click();
                },
                klass: "fas fa-layer-plus"
            },
            'Agregar nivel a cuadro de clasficación': {
                click: function (element) {
                    document.getElementById("Button_activa_agregar_nivel").click();
                },
                klass: "fas fa-layer-plus"
            },
            'Editar elemento': {
                click: function (element) {
                    document.getElementById("Button_activa_editar_cuadro_clasificacion").click();
                },
                klass: "fal fa-edit"
            },
            'Eliminar elemento': {
                click: function (element) {
                    document.getElementById("Button_eliminar_cuadro_clasificacion").click();
                },
                klass: "fal fa-trash-alt"
            },
            'Salir del menú': {
                click: function (element) {
                },
                klass: "fad fa-times"
            }
        });
    }
    

})
var ESTATE = 0;
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
        auto_zise();
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
function activa_menu_general_diference_local(event, e, event_name) {
    try {
        if (event_name == "A-CC") {
            document.getElementById("Button_activa_agregar_cuadro_clasificacion").click();
        }
        if (event_name == "A-NC") {
            document.getElementById("Button_activa_agregar_nivel").click();
        }
        if (event_name == "E-NCCC") {
            document.getElementById("Button_activa_editar_cuadro_clasificacion").click();
        }
        if (event_name == "D-NCCC") {
            document.getElementById("Button_eliminar_cuadro_clasificacion").click();
        }
       
        event.preventDefault();
    }
    catch (ex) {
        alert("Inconsistencia general function activa_menu_general_diference_local " + ex.message)
    }
}
function activa_boton_lista_cuadro() {
    try {
    
        document.getElementById("Button_lista_cuadro_clasficacion_treview").click();
    }
    catch (err) {
        alert(err.message + " Funcion activa_boton_lista_cuadro");
    }
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
function auto_zise() {
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

        $("#contenedor_general").css("height", (espacio_iframe - 20) + "px");
        var heigconetedor = 0;
        $("#Estru_clasificacion").css("height", ($("#DropDownList_nivel_clasficacion").height() + 15) + "px");
        $("#Div_opciones_multinivel").css("height", $("#Button_activa_agregar_nivel").height() + 15 + "px");
        heigconetedor = $("#contenedor_general").height() - ($("#Estru_clasificacion").height() + $("#Div_opciones_multinivel").height() + $("#menu_var").height());
        $("#Paneltreview").css("height", (heigconetedor - 5) + "px");
        $("#tre_claficacion").css("height", heigconetedor + "px");
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise");
    }
}
function progres_hiden(progres) {
    $("#progres_bar").css("display", "none");
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