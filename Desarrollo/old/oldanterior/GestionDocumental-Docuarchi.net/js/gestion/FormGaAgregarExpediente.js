$(document).ready(function () {
   
    $.fn.auto_postback = function (data_grid_id, email_selector_id) {
        auto_zise();
        inicializa_datos_gextion_expediente();     
        auto_zise_reasigna_expe_unidad();
        $('.close').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });     
        if (window.parent.document.getElementById("Iframe_agregar_expdiente_popup_trabajo_")) {
            document.getElementById("Button_cerrar_principal").style.display = "Block";
        }
    };
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
        inicializa_datos_gextion_expediente();
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

function cerra_modal_expediente() {
    try {
        if (window.parent.document.getElementById("Iframe_agregar_expdiente_popup_trabajo_")) {
            document.getElementById("Button_cerrar_principal").style.display = "none";
            window.parent.document.getElementById("Buttoncerrar_agregar_expdiente_popup_trabajo").click();

        }
    } catch (ex) {
        alert("Inconistencia función cerra_modal_expediente " + ex.message)
    }
}
function inicializa_datos_gextion_expediente() {
    try {
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
        $('.def').click(function (event) {
            event.preventDefault();
        });
    }


    catch (err) {

        alert(err.message + " funcion inicializa_datos_gextion_expediente " + err.message);

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

function selecion_change_sub_area() {
    var drowplist = $('#DropDownListsub_seccion');
    if (drowplist == undefined) {
        alert("Imposible encontrar DropDownListsub_seccion");
        return false;
    }
    var buton = $('#Button_seleccion_sub_area');
    if (buton == undefined) {
        alert("Imposible encontrar Button_seleccion_sub_area");
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
    $('#Button1_seleccion_expediente_manual').click();
   

}
function onDataShown(sender, args) {
    sender._popupBehavior._element.style.zIndex = 1000001;
    //sender._popupBehavior._element.style.left = "54px";//set positions according to your requriment.
    //sender._popupBehavior._element.style.top = "50px";//set top postion accorind to you requirement.

    //you can either use left,top or right,bottom or any combination u want to set ur divlist.            
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
   
    if (window.parent.document.getElementById("Panel_agregar_expdiente_popup_trabajo")) {
        espacio_iframe = window.parent.document.getElementById("Panel_agregar_expdiente_popup_trabajo").clientHeight - 10;
        $("#contenedorcontroles").css("height", (espacio_iframe - 45) + "px");
        $("#contenido_campos").css("height", ($("#contenedorcontroles").height()) - ($("#contenido_botonoes").height() + $("#titulo").height() + 30) + "px");
    } else {
        $("#contenedorcontroles").css("width",  "100%");
        $("#contenedorcontroles").css("height", (espacio_iframe - 45) + "px");
        $("#contenido_campos").css("height", ($("#contenedorcontroles").height()) - ($("#contenido_botonoes").height() + $("#titulo").height() + 30) + "px");
    }
    
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
function create_element_popup(elemento_posicion, element_padre_everflow) {
    try {

        var document_posicion = document.getElementById(elemento_posicion);
        var documento = document.getElementById("myModal");
        $('#myModal').css("width", "400px");
        $('#myModal').css("height", "150px");
        $('#mytexto_').css("width", "395px");
        $('#mytexto_').css("height", "145px");
        //document.getElementById("tex_modal").innerHTML = texto_popup;
        var pagy = 1;
        if (document.getElementById(element_padre_everflow).scrollTop) {
            pagy = document.getElementById(element_padre_everflow).scrollTop;
        }
        if (document_posicion.offsetParent) {

            if (document_posicion.offsetParent.offsetTop > document_posicion.offsetTop) {

                documento.style.top = (document_posicion.offsetParent.offsetTop - pagy) + 45 + "px";
            } else {

                documento.style.top = (document_posicion.offsetTop - pagy) + 45 + "px";
            }

        } else {
            documento.style.top = (document_posicion.offsetTop - pagy) + 45 + "px";
        }
        documento.style.left = document_posicion.offsetLeft + document_posicion.offsetWidth + "px";
        documento.style.display = "block";
        $('#myModal').show();



    }
    catch (err) {
        alert(err.message + " Función create_element_popup");
    }
}
function hide_autonomo() {
    document.getElementById("myModal").style.display = "none";
}
//3. La funcion a continuacion es la encargda de asignar el texto de ayuda de la función 
function ayuda_compartir(nombre_boton_ayuda, element_padre_everflow) {
    try {
        web_service_solicitudes_ayuda(nombre_boton_ayuda, 'tex_modal');
        create_element_popup(nombre_boton_ayuda, element_padre_everflow);
    }
    catch (err) {
        alert(err.message + " Funcion web_service_solicitudes_ayuda");
    }

}
function web_service_solicitudes_ayuda(datas, hidemodal) {
    try {
        var obj = { "nombre_ayuda": datas, "nombre_modulo": "modulo" };
        var jsonData = JSON.stringify(obj);
        $.ajax({
            url: '../radicador/' + 'HandlerListaAyuda.ashx',
            type: 'POST',
            data: jsonData,
            success: function (data) {
                //alert(data);
                //web_service_solicitudes_ayuda = data;
                document.getElementById(hidemodal).innerHTML = data;
                //return web_service_solicitudes_ayuda;
            },
            error: function (errorText) {
                //alert("Error general funcion axion_script !" + errorText);
            }
        });
    }
    catch (err) {
        alert(err.message + " Funcion web_service_solicitudes_ayuda");
    }
}
