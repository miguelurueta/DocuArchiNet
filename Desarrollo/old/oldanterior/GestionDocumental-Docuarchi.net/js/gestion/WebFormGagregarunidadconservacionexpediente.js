$(document).ready(function () {

    $.fn.auto_postback = function (data_grid_id, email_selector_id) {
        //alert("ojo");
        inicializa_datos_gextion_expediente();
        auto_zise();
    };
   
    if (window.parent.document.getElementById("Iframe_agregar_expdiente_popup_")) {
        document.getElementById("Button_cerrar_principal").style.display = "Block";       
    }
    if (window.parent.document.getElementById("Iframe_agregar_expdiente_popup_")) {
        document.getElementById("Button_cerrar_principal").style.display = "Block";
    }
    if (window.parent.document.getElementById("Iframe_agregar_unidad_conservacion_popup")) {
        document.getElementById("Button_cerrar_principal").style.display = "Block";
    }
    

});
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

function cerra_modal_expediente() {
    try {
        if (window.parent.document.getElementById("Iframe_agregar_expdiente_popup_")) {
            document.getElementById("Button_cerrar_principal").style.display = "none";
            window.parent.document.getElementById("Buttoncerrar_agregar_expdiente_popup").click();

        }
        if (window.parent.document.getElementById("Iframe_agregar_expdiente_popup_")) {
            document.getElementById("Button_cerrar_principal").style.display = "none";
            window.parent.document.getElementById("Buttoncerrar_agregar_expdiente_popup").click();

        }
        if (window.parent.document.getElementById("Iframe_agregar_unidad_conservacion_popup")) {
            document.getElementById("Button_cerrar_principal").style.display = "none";
            window.parent.document.getElementById("Buttoncerrar_agregar_unidad_conservacion_popup").click();

        }
        if (window.parent.document.getElementById("Iframe_agregar_expdiente_popup_trabajo_")) {
            document.getElementById("Button_cerrar_principal").style.display = "none";
            window.parent.document.getElementById("Buttoncerrar_agregar_expdiente_popup_trabajo").click();

        }
    } catch (ex) {
        alert("Inconistencia función cerra_modal_expediente " + ex.message)
    }
}
function activa_agrega_unidad_interface() {
    try {
        var buton = $('#Button_agrega_unidad_conservacion_interface', window.parent.document);
        if (buton[0].value != undefined) {
            buton[0].click();
            }
    }
    catch (err) {

        alert(err.message + " funcion activa_agrega_unidad_interface " + err.message);

    }
}
function inicializa_datos_gextion_expediente() {
    try {
        var hiden_selecion = $('#Hidden_tipo_unidad_seleccion', window.parent.document);
       
            if (hiden_selecion[0].value != undefined) {
                $('#Hidden_tipo_unidad_seleccion').val(hiden_selecion[0].value);
            }
           
       
        var drowplist = $('#DropDownListEntidadEmpresa', window.parent.document);
        if (drowplist.length == 0) {
            //alert("Imposible encontrar DropDownListEntidadEmpresa");
            return false;
        }
        if (drowplist === undefined) {
            if (drowplist[0].value != undefined) {
                $('#Hiddenname_empresagestion').val(drowplist[0].value);
            }
        }

    }


    catch (err) {

        alert(err.message + " funcion inicializa_datos_gextion_expediente " + err.message);

    }
}
function lista_ayuda_expediente() {
    var drowplist = $('#DropDownListBoxtipoexpediente');
    if (drowplist === undefined) {
        alert("Imposible encontrar DropDownListBoxtipoexpediente");
        return false;
    }
    var buton = $('#Button_lista_ayuda_expediente');
    if (buton === undefined) {
        alert("Imposible encontrar Button_lista_ayuda_expediente");
        return false;
    }
    buton.click();
}
function selecion_change_organigrama() {
    var drowplist = $('#DropDownListorganigrama');
    if (drowplist == undefined) {
        alert("Imposible encontrar DropDownListorganigrama");
        return false;
    }
    var buton = $('#Button_selecion_organigrama');
    if (buton == undefined) {
        alert("Imposible encontrar Button_selecion_organigrama");
        return false;
    }
    buton.click();
}
function selecion_change_area() {
    var drowplist = $('#DropDownListArea');
    if (drowplist == undefined) {
        alert("Imposible encontrar DropDownListArea");
        return false;
    }
    var buton = $('#Button_selecion_area');
    if (buton == undefined) {
        alert("Imposible encontrar Button_selecion_area");
        return false;
    }
    buton.click();
}

function selecion_change_serie() {
    var drowplist = $('#DropDownListSerie');
    if (drowplist == undefined) {
        alert("Imposible encontrar DropDownListSerie");
        return false;
    }
    var buton = $('#Button_selecion_serie');
    if (buton == undefined) {
        alert("Imposible encontrar Button_selecion_serie");
        return false;
    }
    buton.click();
}

function changue_option_manual() {
    $('#Button_lista_ayuda_tipo_unidad').click();
  

}
function changue_manual_consecutivo() {
    $('#Button1_seleccion_expediente_manual').click();
}
function onDataShown(sender, args) {
    sender._popupBehavior._element.style.zIndex = 1000001;
    //sender._popupBehavior._element.style.left = "54px";//set positions according to your requriment.
    //sender._popupBehavior._element.style.top = "50px";//set top postion accorind to you requirement.

    //you can either use left,top or right,bottom or any combination u want to set ur divlist.            
}

function auto_zise() {
    var espacio_iframe;
    var hidenpadre;
    if (window.innerHeight) {
        //navegadores basados en mozilla 
        espacio_iframe = window.innerHeight;
    } else {
        if (document.body.clientHeight) {
            //Navegadores basados en IExplorer, es que no tengo innerheight 
            espacio_iframe = document.body.clientHeight;
        } else {
            //otros navegadores y iframe
            //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val();

        }
    }


    hidenpadre = $('#Hiddenheigpagina', window.parent.document).val();
    if (hidenpadre != undefined) {
        var tempo = $('#Hiddenheigpagina', window.parent.document).val();
        if (tempo != "0") {
            //espacio_iframe = (parseInt(tempo) - 50);
        }
    }


    if (window.parent.document.getElementById("Iframe_agregar_unidad_conservacion_popup")) {
        espacio_iframe = window.parent.document.getElementById("Iframe_agregar_unidad_conservacion_popup").clientHeight - 40;
    }
    if (window.parent.document.getElementById("Iframe_agregar_expdiente_popup_")) {
        espacio_iframe = window.parent.document.getElementById("Iframe_agregar_expdiente_popup_").clientHeight - 20;
    }
    $("#contenedorcontroles").css("height", (espacio_iframe - 10 ) + "px");
    $("#contenido_campos").css("height", ($("#contenedorcontroles").height() - ($("#titulo").height() + $("#panel_botones").height() + 30)) + "px");
    $('.solo-numero').keyup(function () {
        this.value = (this.value + '').replace(/[^0-9]/g, '');
    });
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