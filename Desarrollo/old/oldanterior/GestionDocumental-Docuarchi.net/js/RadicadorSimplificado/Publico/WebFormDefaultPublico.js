$(document).ready(function () {
    $.fn.inicio = function () {
       oculta_lef();
       auto_zise_publico();
        $(window).resize(bodyResize);
        function bodyResize() {
            auto_zise_publico();
        }

    }
});
$(window).on("load", function () {
    var elment = document.getElementsByClassName("da_event_captive");
    if (elment) {
        for (var i = 0; i < elment.length; i++) {
            elment[i].addEventListener("click", event_click, false);
        }
    }
    var elment_ = document.getElementsByClassName("coll_sap_active");
    if (elment_) {
        for (var i = 0; i < elment_.length; i++) {
            elment_[i].addEventListener("click", event_onclick_colapse_card, false);
        }
    }
   
});
function event_onclick_colapse_card(e) {
    var element = e.currentTarget;
    if (element.classList.contains("collapsed")) {
        element.firstChild.classList.remove("fa-caret-up");
        element.firstChild.classList.add("fa-caret-down");
    } else {
        element.firstChild.classList.add("fa-caret-up");
        element.firstChild.classList.remove("fa-caret-down");
    }
}
function menu_public_general(value_Select) {
    try {
        if (value_Select == "P-QR-G") {
            loading_iframe('ifrm_ds_', "../Publico/WebFormPqrsGestion.aspx");
            document.getElementById("ifrm_ds_").style.display = "flex";
           
        }
        if (value_Select == "G-PQ") {
            ///document.getElementById("ifrm_ds_").src = "../Publico/WebFormPqrsPrincipal.aspx";
            loading_iframe('ifrm_ds_', "../Publico/WebFormPqrsPrincipal.aspx");
            document.getElementById("content_opcion_public").style.display = "none";
            document.getElementById("ifrm_ds_").style.display = "flex";
            document.getElementById("Label_title_seleccion").textContent = "PQRSD";
        }
        if (value_Select == "G-CER") {
            //document.getElementById("ifrm_ds_").src = "../Publico/WebFormConsultaRadicadoPublico.aspx";
            loading_iframe('ifrm_ds_', "../Publico/WebFormConsultaRadicadoPublico.aspx");
            document.getElementById("content_opcion_public").style.display = "none";
            document.getElementById("ifrm_ds_").style.display = "flex";
            document.getElementById("Label_title_seleccion").textContent = "CONSULTA RADICADO";
        }
        if (value_Select == "G-CDP") {
            //document.getElementById("ifrm_ds_").src = "../Publico/WebFormConsultaPublico.aspx";
            loading_iframe('ifrm_ds_', "../Publico/WebFormConsultaPublico.aspx");
            document.getElementById("content_opcion_public").style.display = "none";
            document.getElementById("ifrm_ds_").style.display = "flex";
            document.getElementById("Label_title_seleccion").textContent = "CONSULTA DOCUMENTOS";
        }
        if (value_Select == "G-CEO") {
            //document.getElementById("ifrm_ds_").src = "../Publico/WebFormConsultaOficiales.aspx";
            loading_iframe('ifrm_ds_', "../Publico/WebFormConsultaOficiales.aspx");
            document.getElementById("content_opcion_public").style.display = "none";
            document.getElementById("ifrm_ds_").style.display = "flex";
            document.getElementById("Label_title_seleccion").textContent = "ENTIDADES OFICIALES";
        }
    } catch (ex) {
        alert("function menu_public_general " + ex.message);
    }
}
function even_diplay_ini() {
    if (document.getElementById("content_opcion_public").style.display == "flex" && document.getElementById("ifrm_ds_").src) {
        //document.getElementById("content_opcion_public").style.display = "none";
        //document.getElementById("ifrm_ds_").style.display = "flex";
    } else {
        document.getElementById("ifrm_ds_").style.display = "none";
        document.getElementById("content_opcion_public").style.display = "flex";
        document.getElementById("Label_title_seleccion").textContent = "";
    }


}
function oculta_lef() {
    try {
       
        $('#cuerpoleft', window.parent.document).css("display", "none");
        //$("#cuerpoleft", window.parent.document).css("width", "0.2%")
        $("#pie_cuerpo_left", window.parent.document).css("display", "none")
        //$("#pie_cuerpo_left", window.parent.document).css("width", "0.2%")
        //$("#tre", window.parent.document).css("overflow", "hidden")
        $("#cuerporigth", window.parent.document).css("width", "100%")
        $("#Ocultarigth", window.parent.document).css("display", "none")
        $("#ocultaleft", window.parent.document).css("display", "none")
        //$("#TreeView1"), window.parent.document.css("display", "none")
    }
    catch (err) {
        alert(err.message + " funcion oculta_lef " + err.message);
    }
}
function auto_zise_publico() {
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
                //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val(); Contenido_consulta_documento tol_pie

            }
        }
        if (parent.document.getElementById("ContentPlacenter_ifrm_ds_")) {
            espacio_iframe = parent.document.getElementById("ContentPlacenter_ifrm_ds_").clientHeight;
        }
        //$('#div_titulo_form').css("height", $('#Label_title_seleccion').height() + 5 + "px");
        $('#opciones_seleccion').css("height", $('#Menu_principal').height() + "px");
        $('#div_iframe').css("height", (espacio_iframe) - ($('#opciones_seleccion').height() + $('#div_titulo_form').height()) + "px");
        $('#card_general_ini_text_public').css("height", (espacio_iframe) - ($('#opciones_seleccion').height() + $('#div_titulo_form').height()  + 100) + "px");
        $('#ifrm_ds_').css("height", (espacio_iframe) - ($('#opciones_seleccion').height() + $('#div_titulo_form').height() ) + "px");
    
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_publico " + err.message);
    }
}



function progres_hiden(progres) {
    $("#progres_bar").css("display", "none");
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