$(document).ready(function () {
    $.fn.inicio = function () {
        plugin_grwedview();     
    }


})
$(window).on("load", function () {
    window.addEventListener("resize", rezize_event);
});
function rezize_event() {
    try {
        plugin_grwedview();
    } catch (ex) {
        alert(ex.message + " Función rezize_event")
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

function progres_hiden(progres) {
    try {
        $("#progres_bar").css("display", "none");
    }
    catch (err) {
        alert(err.message + " Funcion progres_hiden");
    }
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
        alert(err.message + " Funcion posicion_update_pogres");
    }

}

function retorna_colum_mtriz(hiden_name) {
    try {
        var hiden = document.getElementById(hiden_name);
        var x = $('#GridView_val_radicacion th');
        var txt = "";
        var i;
        for (i = 0; i < x.length; i++) {
            txt = txt + x[i].innerText.toUpperCase() + "|";
        }
        hiden.value = txt;
        return txt;
    }
    catch (err) {
        alert(err.message + " Funcion retorna_colum_mtriz");
    }
}

function onDataShown(sender, args) {
    sender._popupBehavior._element.style.zIndex = 1000001;

}

//  AUTOSIZE DATA GREVIEW
function plugin_grwedview() {
    try{
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
    
    if (window.parent.document.getElementById("Iframe_transacciones_")) {   
        espacio_iframe = $('#Iframe_transacciones_', window.parent.document).height();
        if (espacio_iframe !== 0) {
            with_frame = $('#Iframe_transacciones_', window.parent.document).width() - 17;
            $('#Contenedorderecho').css("height", (espacio_iframe - 20) + "px");
        }
    }
    if (window.parent.document.getElementById("Iframe_log_transacciones_")) {
        espacio_iframe = $('#Iframe_log_transacciones_', window.parent.document).height();
        if (espacio_iframe !== 0) {
            with_frame = $('#Iframe_log_transacciones_', window.parent.document).width() - 17;
            $('#Contenedorderecho').css("height", (espacio_iframe - 20) + "px");
        }
    }
    if (window.parent.document.getElementById("Iframe_visor_externo__")) {
        espacio_iframe = $('#Iframe_visor_externo__', window.parent.document).height();
        if (espacio_iframe !== 0) {
            with_frame = $('#Iframe_visor_externo__', window.parent.document).width() - 17;
            $('#Contenedorderecho').css("height", (espacio_iframe - 20) + "px");
        }
    }
    if (window.parent.document.getElementById("Iframe_transacciones_historial_")) {
        espacio_iframe = $('#Iframe_transacciones_historial_', window.parent.document).height();
        if (espacio_iframe !== 0) {
            with_frame = $('#Iframe_transacciones_historial_', window.parent.document).width() - 17;
            $('#Contenedorderecho').css("height", (espacio_iframe - 20) + "px");
        }
    }
    $('#contenido_titulo_val_radicacion').css("height", ((document.getElementById("Button_Exportar_Radicados").clientHeight + 1)) + "px");
    $('#Contenido_botones_tipo_radicado').css("height", ((document.getElementById("Button_Exportar_Radicados").clientHeight + 20)) + "px");
  
    var heigconetedor = document.getElementById("Contenedorderecho").clientHeight - (document.getElementById("contenido_titulo_val_radicacion").clientHeight + document.getElementById("Contenido_botones_tipo_radicado").clientHeight);
    $('#contenido_datagrid_val_radicacion').css("height", (heigconetedor - 10) + "px");
    
    
}
    catch (err) {
        alert(err.message + " Funcion plugin_grwedview");
}
}

