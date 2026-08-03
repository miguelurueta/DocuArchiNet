$(document).ready(function () {
    $.fn.clired_user = function () {
        auto_zise_visor();
        mueve_scrool_seleccion();
        inicio_seleccion();
        display_descarag();
        
        $("#noaming").bind("contextmenu", function (e) {
            e.preventDefault();
        });

    }

    $("#ocultaleft").click(function (e) {

        if ($("#Contendor_Izquiero").is(":hidden")) {
            if ($("#div_contendor_externo").is(":hidden")) {
                $("#Contendor_derecho").css("width", "79%");
                $("#content").css("width", "100%");
                $("#Contendor_Izquiero").css("width", "20%");
                $("#Contendor_Izquiero").show(1)
                auto_zise_visor();
            } else {
                $("#div_contendor_externo").css("width", "79%");
                $("#ifrm_visor_").css("width", "100%");
                $("#Contendor_Izquiero").css("width", "20.1%");
                $("#Contendor_Izquiero").show(1)
                auto_zise_visor();
            }

        } else {
            if ($("#div_contendor_externo").is(":hidden")) {
                $("#Contendor_Izquiero").hide(1);
                $("#Contendor_derecho").css("width", "99%");
                $("#content").css("width", "98%");
            } else {
                $("#Contendor_Izquiero").hide(1);
                $("#div_contendor_externo").css("width", "100%");
                $("#ifrm_visor_").css("width", "100%");
            }


        }


    });
});
$(window).on("load", function () {
    var elment = document.getElementsByClassName("da_event_captive");
    if (elment) {
        for (var i = 0; i < elment.length; i++) {
            elment[i].addEventListener("click", event_click, false);
        }
    }
    window.addEventListener("resize", rezize_event);    
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
        auto_zise_visor();
        inicio_seleccion();   
        display_descarag();
    } catch (ex) {
        alert(ex.message + " Función rezize_event")
    }
}
function preven_event_search_keypres_enter(e, sender) {
    try {
        tecla = (document.all) ? e.keyCode : e.which;
        if (tecla == 13) {
            document.getElementById("ImageButton_ir_pagina").click();
            e.preventDefault();    
        }
    } catch (err) {
        alert(err.message + " funcion preven_event_search_keypres_enter " + err.message);
    }
}
function mueve_scrool_seleccion() {
    try {

        $("#TreeViewseleccion .LeafNodeStyle_2  a").each(function () {
            var element = $(this);
            if (element[0].style.color == "red") {
                var tempo = Math.round((Math.round($(this).offset().top) - Math.round($('#TreeViewseleccionn0Nodes').offset().top)));
                $('#TreeViewseleccionn0Nodes').scrollTop(Math.round(tempo / 4));
            }
        })
    }
    catch (err) {
        alert(err.message + " Funcion mueve_scrool_seleccion");
    }

}
function inicio_seleccion() {
    if (document.getElementById("Hidden_estado_visor").value == "1") {
        document.getElementById("Hidden_estado_visor").value = "0";
        if (document.getElementById("Hidden_tipo_visor_externo").value == "0") {
            document.getElementById("content").style.display = "block";
            document.getElementById("tollimage").style.display = "block";
            document.getElementById("div_contendor_externo").style.display = "none";
        } else {
            document.getElementById("content").style.display = "none";
            document.getElementById("tollimage").style.display = "none";
            document.getElementById("div_contendor_externo").style.display = "block";
        }
        //auto_zise_visor();
    }
}
function selecciona_treview_seleccion() {
    try {

        $("#" + "TreeViewseleccion" + " td: a").each(function () {
            $(this).css("color", "Black");
        })
        var nodo = $("#" + "TreeViewseleccion" + " td: a");
        for (var index = 0; index < nodo.length; index++) {
            if (document.getElementById("hidden_selecion_documento_treview").value == nodo[index].title) {
                nodo[index].style.color = "Red";
                return;
            }
        }

    }
    catch (err) {
        alert(err.message + " Funcion selecciona_treview_seleccion");
    }
}
function mantiene_oculta_lef() {
    if ($("#Contendor_Izquiero").is(":hidden")) {
        $("#Contendor_derecho").css("width", "98%");
        $("#content").css("width", "96%");
       
    } 
}
function seleccion_visor() {
    
    var hiden_tipo_visor = window.parent.document.getElementById("Hidden_tipo_visor");
    if (hiden_tipo_visor == undefined) {
        alert("Imposible ecnontrar el control Hidden_tipo_visor en la pagina fuente, no se puede visualizar la imagen");
        return false;
    }

    var Hidden_id_tarea_sel = window.parent.document.getElementById("Hidden_id_tarea_sel");
    if (Hidden_id_tarea_sel == undefined) {
        alert("Imposible ecnontrar el control Hidden_id_tarea_sel en la pagina fuente, no se puede visualizar la imagen");
        return false;
    }
    document.getElementById("Hidden_tipo_visor").value = hiden_tipo_visor.value;
    document.getElementById("Hidden_id_tarea_sel").value = Hidden_id_tarea_sel.value;
    
   
    
}
//ACTIVA EL GIF DE PROGRESO DE UN EVENTO
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
function display_descarag() {
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
        //corrige el posicionamiento horizontal del modalpopuextender, la propiedad width del panel debe estar en auto
        //var widtth_procent_left_rigth = (with_frame - document.getElementById("Panel_agregar_expediente_carpeta").clientHeight) / 2;
        //$('#Panel_agregar_expediente_carpeta').css("left", (Math.round(widtth_procent_left_rigth)) + "px");
        var heig_porcent = espacio_iframe - ((espacio_iframe * 20) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_guardar').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_guardar').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#ContenidoImpresion_guardar').css("height", (document.getElementById("modal_content_guardar").clientHeight - (document.getElementById("divcabecer2_post_guardar").clientHeight + document.getElementById("content_boton_guardar").clientHeight)) + "px");
        $('#Iframe_guardar').css("height", (document.getElementById("modal_content_guardar").clientHeight - (document.getElementById("divcabecer2_post_guardar").clientHeight + document.getElementById("content_boton_guardar").clientHeight + 10)) + "px");
        
    } catch (ex) { alert(ex.message + " funcion display_descarag") }
}
function auto_zise_visor() {
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
    
    if (window.parent.document.getElementById("Div_imagen_respuesta_imagen")) {
        espacio_iframe = (window.parent.document.getElementById("Panel_imagen_respuesta").clientHeight - 20);
    }
    if (window.parent.document.getElementById("Iframe_visor_externo_wf_")) {
        espacio_iframe = window.parent.document.getElementById("Iframe_visor_externo_wf_").clientHeight - 20;
    }
    if (window.parent.document.getElementById("Iframe_visor_externo_")) {
        espacio_iframe = window.parent.document.getElementById("Iframe_visor_externo_").clientHeight - 20;
    }
    if (window.parent.document.getElementById("Iframe_visor_tareas_pendiente_")) {
        espacio_iframe = window.parent.document.getElementById("Iframe_visor_tareas_pendiente_").clientHeight - 20;
    }
    if (window.parent.document.getElementById("Iframe_imagen_respuesta_")) {
        espacio_iframe = window.parent.document.getElementById("Iframe_imagen_respuesta_").clientHeight - 20;
    }
    var heigh = espacio_iframe;
    $('#conte_waper').css("height", (heigh) + "px");
    $('#da_content_wraper').css("height", (heigh) + "px");
    $('#sidebar').css("height", (heigh ) + "px");
    $('#content_').css("height", (heigh) + "px");
    $('#div_contendor_externo').css("height", (heigh - 3) + "px");
    $('#ifrm_visor_').css("height", (heigh - 3) + "px");
    $('#content_image').css("height", (heigh - document.getElementById("tollimage").clientHeight) + "px");
    $('#content').css("height", (heigh - 3) + "px");
    var height_ = document.getElementById('da_content_wraper').clientHeight - (document.getElementById('contenido_pie').clientHeight + document.getElementById('title_table').clientHeight);
    $('#contenido_treeview').css("height", (height_) + "px");
    $('#Panel_scroll').css("height", (height_ - 2) + "px")
    
}
    catch (err) {
        alert(err.message + " funcion auto_zise_visor " + err.message);
}

}
