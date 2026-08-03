$(document).ready(function () {
    $.fn.inicio = function () {
        //auto_zise_popup_visor_externo();         
        $(window).resize(bodyResize);
        function bodyResize() {
            //if (document.getElementById("HiddenFi").value == "NO") {
                //auto_zise_consulta_rue("GridView_detalle_consulta_rue");
           // }
        }
    }
})

function prevent(event,element) {
 try  {
          event.preventDefault();
          document.getElementById("HiddenFi").value = "YES";
          var g = element;
          var fer = $(element).attr("idd");
          $('#hdnEmailID_VAL').val(fer);
          document.getElementById("Button_inicio").click();
          auto_zise_popup_visor_externo();
          element.focus();
    }
    catch (err) {
        alert(err.message + " Funcion prevent");
    }
}
function auto_zise_popup_visor_externo() {
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


    $(document).ready(bodyResize);
    $(window).resize(bodyResize);
    function bodyResize() {

        $('#Panel_visor_externo').css("height", (espacio_iframe - 40) + "px");
        $('#Cotenedorpendiente_visor_externo').css("height", (espacio_iframe - 40) + "px");
        $('#Iframe_visor_externo_').css("height", (espacio_iframe - 40) + "px");

    }


}
function web_service_solicitudes_ayuda(datas, hidemodal) {
    try {
        var obj = { "m_update_panel": document.getElementById("UpdatePanel_visor_externo"), "m_modal_popup": document.getElementById("ModalPopupExtender_visor_externo") };
        var jsonData = JSON.stringify(obj);
        $.ajax({
            url: '../radicador/' + 'Handler_visualiza_imagen.ashx',
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
function auto_zise_consulta_rue(nombre_grid) {
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


        $(document).ready(bodyResize);
        $(window).resize(bodyResize);
        function bodyResize() {

        }
      
        var total = document.getElementById("div_log_camara_rue").clientHeight + document.getElementById("div_title_pagina").clientHeight + document.getElementById("div_table_descripcion").clientHeight + document.getElementById("div_title_resultado").clientHeight;
        total = (espacio_iframe - total) - 40;
        var gridwith = with_frame - 20;
        var gridheihg = total;
        $('#contenido_datagrid_val_radicacion').css("height", (gridheihg) + "px");
        $(document).ready(function () { $('#GridView_detalle_consulta_rue').gridviewScroll({ width: gridwith - 5 , height: (gridheihg - 40) }); })
        if ($('#' + nombre_grid + ' td').children.length > 0 && $('#' + nombre_grid + 'tr:visible').length > 0) {
            
        }
        
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_consulta_rue " + err.message);
    }
}
function capturar_rue() {
    try {
       /* if (document.getElementById("HiddenField_resultado_rue").value == "YES") {
            return false;
        }
        var elDiv = window.opener.document.forms[0].elements['param'].value;
        if (elDiv) {
            document.getElementById("HiddenField_param").value = elDiv;
        } else {
            alert("Imposible encontrar el control param " + " capturar_rue");
            return false;
        }
        
        var cod_camara = window.opener.document.forms[0].elements['codigoCamara'].value;
        if (cod_camara) {
            document.getElementById("HiddenField_cod_camara").value = cod_camara;
        } else {
            alert("Imposible encontrar el control codigoCamara " + " capturar_rue");
            return false;
        }*/
        //document.getElementById("HiddenFi").value == "YES";
       
        
    }

    catch (err) {
        alert(err.message + " capturar_rue");
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