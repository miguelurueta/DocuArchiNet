$(document).ready(function () {
    $.fn.inicio = function () {
        service_expedientes_clasificacion("TextBox_busqueda");
        service_documentos_clasificacion("TextBox_busqueda_documento");
        if (ESTATE == 0) {
            ESTATE = 1;
            Auto_zise_marco_principal();
            auto_zise_popup_visor_externo();      
        }
      
        $('#data_grid tr[id]').click(function () {
            $('#data_grid tr[id]').css({ "background": "White", "color": "Black" });
            $(this).css({ "background-color": "#e8e8f7", "color": "Black" });
 
        });
        $('#data_grid_documentos tr[id]').click(function () {
            $('#data_grid_documentos tr[id]').css({ "background": "White", "color": "Black" });
            $(this).css({ "background-color": "#e8e8f7", "color": "Black" });

        });
        $('#data_grid_documentos tr[id]').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });
        $('#data_grid tr[id]').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
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
        tab_sow("home-expediente");
        window.addEventListener("resize", rezize_event);
        ShowModalPopup("ModalPopupExtender_mensaje_personalizado_backgroundElement", "Panel_mensaje_personalizado", 100001);


    } catch (e) {
        alert(" funcion load " + e.message);
    }

});
function tab_sow(name) {
    $('#' + name).tab('show');
}
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
        Auto_zise_marco_principal();
        auto_zise_popup_visor_externo();
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

function service_expedientes_clasificacion(name_texbox) {
    function extractLast(term) {
        return term;
    }
    $("#" + name_texbox)
        .on("keydown", function (event) {
            if (event.keyCode === $.ui.keyCode.TAB &&
                $(this).autocomplete("instance").menu.active) {
                event.preventDefault();
            }
        })
        .autocomplete({
            source: function (request, response) {
                var param = { keyword: $('#' + name_texbox).val() };
                $.ajax({
                    url: "../webservice/WebServiceRadicacion.asmx/GetLista_expedientes_clasificacion",
                    data: "{'DName':'" + document.getElementById(name_texbox).value + "'}",
                    dataType: "json",
                    type: "POST",
                    traditional: true,
                    processData: false,
                    contentType: "application/json; charset=utf-8",
                    //dataFilter: function (data) { return data; },
                    success: function (data) {
                        term: extractLast(request.term)
                        response($.ui.autocomplete.filter(
                         data.d, extractLast(request.term)));

                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert(textStatus);
                    }
                });
            },


            focus: function () {
                // prevent value inserted on focus
                return false;
            },
            select: function (event, ui) {
                document.getElementById("Button_busqueda_expediente").click();
                this.value = ui.item.value;
                return false;
            }

            , minLength: 3, max: 10, scroll: true
        });
}
function service_documentos_clasificacion(name_texbox) {
    function extractLast(term) {
        return term;
    }
    $("#" + name_texbox)
        .on("keydown", function (event) {
            if (event.keyCode === $.ui.keyCode.TAB &&
                $(this).autocomplete("instance").menu.active) {
                event.preventDefault();
            }
        })
        .autocomplete({
            source: function (request, response) {
                var param = { keyword: $('#' + name_texbox).val() };
                $.ajax({
                    url: "../webservice/WebServiceRadicacion.asmx/GetLista_documentos_clasificacion",
                    data: "{'DName':'" + document.getElementById(name_texbox).value + "'}",
                    dataType: "json",
                    type: "POST",
                    traditional: true,
                    processData: false,
                    contentType: "application/json; charset=utf-8",
                    //dataFilter: function (data) { return data; },
                    success: function (data) {
                        term: extractLast(request.term)
                        response($.ui.autocomplete.filter(
                         data.d, extractLast(request.term)));

                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert(textStatus);
                    }
                });
            },


            focus: function () {
                // prevent value inserted on focus
                return false;
            },
            select: function (event, ui) {
                document.getElementById("Button_busqueda_documento").click();
                this.value = ui.item.value;
                return false;
            }

            , minLength: 3, max: 10, scroll: true
        });
}
function acti_busq_general_expediente(e, sender) {
    try {
        tecla = (document.all) ? e.keyCode : e.which;
        if (tecla == 13) {
            document.getElementById("Button_busqueda_expediente").click();
            e.preventDefault();         
        }
    } catch (err) {
        alert(err.message + " funcion acti_busq_general_expediente " + err.message);
    }
}
function acti_busq_general_documento(e, sender) {
    try {

        tecla = (document.all) ? e.keyCode : e.which;
        if (tecla == 13) {
            document.getElementById("Button_busqueda_documento").click();
            e.preventDefault();
        }
    } catch (err) {
        alert(err.message + " funcion acti_busq_general_documento " + err.message);
    }
}
function activa_boton_client_documento(e, sender) {
    document.getElementById("Button_busqueda_documento").click();
    e.preventDefault();
}
function restore_activa_boton_client_documento(e, sender) {
    document.getElementById("Button_ver_documentos_relacionados").click();
    e.preventDefault();
}
function activa_boton_client(e, sender) {
    document.getElementById("Button_busqueda_expediente").click();
    e.preventDefault();
}
function activa_restore_search_exp(e, sender) {
    document.getElementById("Button_restore_busqueda_expediente").click();
    e.preventDefault();
}
function prevent(event, element) {

    try {

        var fer = $(element).attr("id");      
        var tip_event = $(element).attr("tip_event");
        if (tip_event == "ver_doc_col") {
            $('#hdnEmailID').val(fer);
            document.getElementById("Button_ver_documentos_relacionados").click();
        }
       
        if (tip_event == "ver_doc") {
            var gab = $(element).attr("idd");
            $('#hdnEmailID_documentos').val(fer);
            $('#Hidden_gabienete').val(gab);
            document.getElementById("Button_ver_documento").click();
        }      
        event.preventDefault();
        element.focus();
    }
    catch (err) {
        alert(err.message + " Funcion prevent");
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
function auto_zise_popup_visor_externo() {
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

        $('#Panel_visor_externo').css("height", (espacio_iframe - 40) + "px");
        $('#Cotenedorpendiente_visor_externo').css("height", (espacio_iframe - 40) + "px");
        $('#Iframe_visor_externo__').css("height", (espacio_iframe - 40) + "px");
        
        //corrige el posicionamiento horizontal del modalpopuextender, la propiedad width del panel debe estar en auto
        //var widtth_procent_left_rigth = (with_frame - document.getElementById("Panel_agregar_expediente_carpeta").clientHeight) / 2;
        //$('#Panel_agregar_expediente_carpeta').css("left", (Math.round(widtth_procent_left_rigth)) + "px");
        var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_visor_externo').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_Panel_visor_externo').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#Cotenedorpendiente_visor_externo').css("height", (document.getElementById("modal_content_Panel_visor_externo").clientHeight - (document.getElementById("Cabecerapendiente_visor_externo").clientHeight )) + "px");
        //Para los modal que contiene gred
        $('#Iframe_visor_externo__').css("height", (document.getElementById("Cotenedorpendiente_visor_externo").clientHeight - 2) + "px");
    }

   
    catch (err) {
        alert(err.message + " Funcion actualiza_gre_campos_dinamicos");
    }
}
function Auto_zise_marco_principal() {
    try {
        //CODIGO AJUSTA EL ALTO DE LA PAGINA DEBE ESTAR EL FORM 100% ALTURA
        var espacio_iframe;
        if (window.innerHeight) {
            //navegadores basados en mozilla 
            espacio_iframe = window.innerHeight
        } else {
            if (document.hidden == true) {
                if (document.body.clientHeight != undefined) {
                    //Navegadores basados en IExplorer, es que no tengo innerheight 
                    espacio_iframe = document.body.clientHeight
                } else {
                    //otros navegadores 
                    espacio_iframe = 478
                }
            }
        }
        var heigconetedor = 0;
        $("#div_contenedor_drecho").css("height", (espacio_iframe - 20) + "px");
        $("#div_contenedor_izquierdo").css("height", (espacio_iframe - 20) + "px");
        $("#div_cuadro_clasficacion").css("height", ($("#DropDownList_nivel_clasficacion").height()) + "px");
        heigconetedor = $("#div_contenedor_izquierdo").height() - ($("#pie_cuerpo_left").height() + $("#div_cuadro_clasficacion").height() + $("#div_title_clasificacion").height());
        $("#div_treview").css("height", (heigconetedor - 40) + "px");
        $("#Panelactividad").css("height", ((document.getElementById("div_contenedor_drecho").clientHeight - (70 + 70)) + "px"));
        $("#Panelactividad_documentos").css("height", ((document.getElementById("div_contenedor_drecho").clientHeight - (70 + 70)) + "px"));
             
    }
    catch (ex) {
        alert("Error funcion Auto_zise_marco_principal " + ex.message)
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