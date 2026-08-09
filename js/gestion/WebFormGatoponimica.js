$(document).ready(function () {
    $.fn.inicio = function () {
        if (ESTATE == 0) {
            ESTATE = 1;
            auto_zise();
            auto_zise_reasigna_expe_unidad();
        }
        $('#Paneltreview').contextMenu('context-menu', {

            'Salir del menú': {
                click: function (element) { },
                klass: "fad fa-times"
            },
            'Agregar nuevo elemento': {
                click: function (element) {
                    activa_menu_general_diference_event(event, this, 'AGRE-EL', 'ADD');
                },
                klass: "fal fa-plus"
            },
            'Eliminar elemento seleccionado': {
                click: function (element) {
                    activa_menu_general_diference_event(event, this, 'DELT-EL', 'DELETE');
                },
                klass: "fal fa-times"
            },
            'Editar elemento seleccionado': {
                click: function (element) {
                    activa_menu_general_diference_event(event, this, 'EDIT-EL', 'EDIT');
                },
                klass: "fal fa-pencil"
            }

        });
       
    }
    $('#Panel_unidad_treview_unidad').contextMenu('context-menu', {

        'Salir del menú': {
            click: function (element) { },
            klass: "fad fa-times"
        },

        'Agregar unidad de conservación': {
            click: function (element) {
                activa_menu_general_diference_boton(event, this, 'A-UC');
            },
            klass: "fal fa-archive"
        },
        'Editar unidad de conservación': {
            click: function (element) {
                activa_menu_general_diference_boton(event, this, 'E-UC');
            },
            klass: "fal fa-edit"
        },
        'Eliminar unidad de conservación': {
            click: function (element) {
                activa_menu_general_diference_boton(event, this, 'D-UC');
            },
            klass: "fad fa-times"
        },
        'Desarchivar unidad': {
            click: function (element) {
                activa_menu_general_diference_boton(event, this, 'DUA-UC');
            },
            klass: "fal fa-folder-tree"
        },
        'Reubicar unidad': {
            click: function (element) {
                activa_menu_general_diference_boton(event, this, 'RUR-UC');
            },
            klass: "fal fa-sitemap"
        },
        'Imprimir rotulo': {
            click: function (element) {
                activa_menu_general_diference_boton(event, this, 'IMPR-UC');
            },
            klass: "fad fa-print"
        },
        'Descargar rotulo': {
            click: function (element) {
                activa_menu_general_diference_boton(event, this, 'DESR-UC');
            },
            klass: "fal fa-file-download"
        },
        'Configurar rotulo': {
            click: function (element) {
                activa_menu_general_diference_boton(event, this, 'CONR-UC');
            },
            klass: "fad fa-tools"
        }


    });
});
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
        auto_zise_reasigna_expe_unidad();
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
function pront_confirmacion(mensaje) {
    try {
    var men = confirm(mensaje);
    if (men) {
        document.getElementById("HiddenField_botones_respuesta").value = "1";
    } else {
        document.getElementById("HiddenField_botones_respuesta").value = "0";
    }
    }
    catch (err) {
        alert(err.message + " funcion pront_confirmacion " + err.message);
    }
}
function buton_click(buton) {
    try {
        document.getElementById(buton).click();
    }
    catch (err) {
        alert(err.message + " funcion buton_click " + err.message);
    }
}
function activa_menu_general_diference_boton(event, e, event_name) {
    try {
        if (event_name == "A-UC") {
           
            document.getElementById("ButtonAgregar").click();
        }
        if (event_name == "E-UC") {

            document.getElementById("ButtonButtonEditar").click();
        }
        if (event_name == "D-UC") {

            document.getElementById("ButtonEliminar").click();
        }
        if (event_name == "DESR-UC") {

            document.getElementById("ButtonRotulo").click();
        }
        if (event_name == "IMPR-UC") {

            document.getElementById("ButtonImprimirRotulo").click();
        }
        if (event_name == "CONR-UC") {

            document.getElementById("Button_configura_rotulo").click();
        }
        if (event_name == "RUR-UC") {

            document.getElementById("ButtonReubicar").click();
        }
        if (event_name == "DUA-UC") {

            document.getElementById("Buttondesarchivar").click();
        }
        event.preventDefault();
    }
    catch (ex) {
        alert("Inconsistencia general function activa_menu_general_diference " + ex.message)
    }
}
function auto_zise_reasigna_expe_unidad() {
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
        //corrige el posicionamiento horizontal del modalpopuextender, la propiedad width del panel debe estar en auto
        //var widtth_procent_left_rigth = (with_frame - document.getElementById("Panel_agregar_expediente_carpeta").clientHeight) / 2;
        //$('#Panel_agregar_expediente_carpeta').css("left", (Math.round(widtth_procent_left_rigth)) + "px");
        var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_reubicar_unidad_expediente_popup').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_reubicar_unidad_expediente_popup').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#Contenido_reubicar_unidad_expediente_popup').css("height", (document.getElementById("modal_content_reubicar_unidad_expediente_popup").clientHeight - (document.getElementById("divcabecer_reubicar_unidad_expediente_popup").clientHeight + document.getElementById("contendor_botones_unidad_r_u_e").clientHeight)) + "px");
        //Para los modal que contiene gred
        $('#div_treview_archivo_r_u_e').css("height", (document.getElementById("Contenido_reubicar_unidad_expediente_popup").clientHeight - document.getElementById("drowlist_r_u_e").clientHeight) + "px");

      
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_reasigna_expe_unidad");
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


        hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val();
        if (hidenpadre != undefined) {
            var tempo = $('#Hiddenheigpaginapopup', window.parent.document).val();
            if (tempo != "0") {
                //espacio_iframe = (parseInt(tempo) - 1);
            }
        }
        espacio_iframe = espacio_iframe - document.getElementById("menu_var").clientHeight;
        $("#Contenedorderecho").css("height", (espacio_iframe - 20) + "px");
        $("#Contentizquierdo").css("height", (espacio_iframe - 20) + "px");
        var heigconetedor = 0;
        //heigconetedor = $("#Contenedorderecho").height() - (($("#Contenedorderecho").height() * 20) / 100);
        //$("#contendor_botones_unidad").css("height", $("#ButtonAgregar").height() + 15 + "px");
        //$("#titulo_unidad_conservacion").css("height", $("#ButtonAgregar").height() + "px");
        $("#Divbotnones_raiz").css("height", $("#ButtonAgregar").height() + "px");
        $("#Divtitulo_raiz").css("height", $("#ButtonAgregar").height() + "px");
        heigconetedor = $("#Contenedorderecho").height() - ( $("#titulo_unidad_conservacion").height());
        $("#contenedor_unidad_treview_unidad").css("height", heigconetedor + "px");
        heigconetedor = $("#Contentizquierdo").height() - ($("#drowlist").height() + $("#Div_title_estrucutura").height());
        $("#div_treview_archivo").css("height", (heigconetedor ) + "px");
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