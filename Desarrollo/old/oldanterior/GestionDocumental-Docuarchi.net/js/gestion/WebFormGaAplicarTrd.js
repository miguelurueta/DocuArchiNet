$(document).ready(function () {
    $.fn.inicio = function () {
        //****************************************VALIDACION RADICACION**********************************************************************************
        //FUNCION ACTIVA SELECCION CLIK EN EL DATAGREDVIEW DE VALIDACION RADICACION
        $('#GridViewlista tr[id]').click(function () {
            $('#GridViewlista tr[id]').css({ "background": "White", "color": "Black" });
            $(this).css({ "background-color": "#e8e8f7", "color": "Black" });
            var fer = $(this).attr("id");
            $('#Hidden_id').val(fer);

        });
      
        //ASIGNA EL CURSOR DE SELECCION CUANDO PASA EL CURSOR EN EL DATAGREDVIEW DE VALIDACION RADICACION
        $('#GridViewlista tr[id]').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });
        auto_zise_aplicar_trd();
        
       
       
        //******************************************FIN****************************************************************************************************       
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
        auto_zise_aplicar_trd();
        plugin_grwedview();
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
function onDataShown(sender, args) {
    sender._popupBehavior._element.style.zIndex = 1000001;

}

function retorna_colum_mtriz(hiden_name) {
    var hiden = document.getElementById(hiden_name);
    var x = $('#GridViewlista th');
    var txt = "";
    var i;
    for (i = 0; i < x.length; i++) {
        txt = txt + x[i].innerText.toUpperCase() + "|";
    }
    hiden.value = txt;
    return txt;
}
function colum_index(colum_name) {
    try {
        var x = $('#GridViewlista th');
        var txt = "";
        var i;
        for (i = 0; i < x.length; i++) {
            if (x[i].innerText == colum_name) {

                return i;
            }

        }
        return -1;
    }
    catch (err) {
        alert(err.message + " Funcion colum_index");
    }
}
function importa_dato_trd() {
    try {
        var namepaginapopup = $('#Hiddennameasigna', window.parent.document);
        if (namepaginapopup == undefined) {
            alert("Imposible encontrar el tipo seleccion en en la pagina fuente fal el control Hiddenheigpaginapopup");
            return false;
        }
        if (namepaginapopup.val() == "RADICACION_ENTRANTE") {
            //asigna_expdiente_radicacion_entrante();
            //return false;
        }

        if (namepaginapopup.val() == "EXPEDIENTE_WORKFLOW") {
            asigna_trd_workflow_indice();
            //asigna_expdiente_radicacion_entrante();
            return false;
        }
        if (namepaginapopup.val() == "DOCUARCHI_NET") {
            asigna_trd_workflow_indice();
            return false;
        }
        if (namepaginapopup.val() == "DOCUARCHI.VISOR") {
            asigna_trd_workflow_indice();
            return false;
        }
        if (namepaginapopup.val() == "RADICACION_SALIENTE") {
            //asigna_expdiente_radicacion_entrante();
            //return false;
        }

    }
    catch (err) {
        alert(err.message + " Funcion importa_dato_expediente");
    }
}
function asigna_trd_workflow_indice() {
    try {
        var modal_popup = $('#Buttoncerrar_trd_popup', window.parent.document);
        var Hidden_id_serie = $('#Hidden_id_serie', window.parent.document);
        var Hidden_id_sub_serie = $('#Hidden_id_sub_serie', window.parent.document);
        var Hidden_id_documento = $('#Hidden_id_documento', window.parent.document);
        var Hidden_id_area = $('#Hidden_id_area', window.parent.document);
        var text_box_NOMBRESERIE = $('#NOMBRESERIE', window.parent.document);
        var text_box_NOMBRESUBSERIE = $('#NOMBRESUBSERIE', window.parent.document);
        var text_box_TIPODOCUMENTO = $('#TIPODOCUMENTO', window.parent.document);
        if ($('#Hidden_id').val() == "0" || $('#Hidden_id').val() == "-1") {
            alert("Debe seleccionar un registro");
            return false;
        }
        if (Hidden_id_serie == undefined) {
            alert("Imposible encontrar el control Hidden_id_serie en el indice workflow");
            return false;
        }
        if (Hidden_id_sub_serie == undefined) {
            alert("Imposible encontrar el control Hidden_id_sub_serie en el indice workflow");
            return false;
        }
        if (Hidden_id_documento == undefined) {
            alert("Imposible encontrar el control Hidden_id_documento en el indice workflow");
            return false;
        }
        if (Hidden_id_area == undefined) {
            alert("Imposible encontrar el control Hidden_id_area en el indice workflow");
            return false;
        }
        if (text_box_NOMBRESERIE == undefined) {
            alert("Imposible encontrar el control text_box_NOMBRESERIE en el indice workflow");
            return false;
        }
        if (text_box_NOMBRESUBSERIE == undefined) {
            alert("Imposible encontrar el control text_box_NOMBRESUBSERIE en el indice workflow");
            return false;
        }
        if (text_box_TIPODOCUMENTO == undefined) {
            alert("Imposible encontrar el control text_box_TIPODOCUMENTO en el indice workflow");
            return false;
        }
      
        var sele_row = $('#GridViewlista tr[id=' + $('#Hidden_id').val() + ']');
        var columindex = colum_index("nombre_serie");
        if (columindex == -1) {
            alert("Imposible encontrar el index de la columna nombre_serie");
            return false;
        }
        text_box_NOMBRESERIE.val(sele_row[0].cells[columindex].innerText);

        columindex = colum_index("Nombre_Subserie");
        if (columindex == -1) {
            alert("Imposible encontrar el index de la columna Nombre_Subserie");
            return false;
        }
        text_box_NOMBRESUBSERIE.val(sele_row[0].cells[columindex].innerText);

        columindex = colum_index("Descripcion_Documento");
        if (columindex == -1) {
            alert("Imposible encontrar el index de la columna Descripcion_Documento");
            return false;
        }
        text_box_TIPODOCUMENTO.val(sele_row[0].cells[columindex].innerText);
        var spli = $('#Hidden_id').val().split("-");
        Hidden_id_serie.val(spli[0]);
        Hidden_id_sub_serie.val(spli[1]);
        Hidden_id_documento.val(spli[2]);
        Hidden_id_area.val(document.getElementById("Hidden_id_area").value);
        modal_popup.click();
    }
    catch (err) {
        alert(err.message + " Funcion asigna_trd_workflow_indice");
    }
}
function auto_zise_aplicar_trd() {
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
        if (window.parent.document.getElementById("Divcerrarbuton_trd_popup_visor_indice")) {
            espacio_iframe = (window.parent.document.getElementById("Panel_trd_popup").clientHeight - window.parent.document.getElementById("Divcerrarbuton_trd_popup_visor_indice").clientHeight) - 40;
        }
        if (window.parent.document.getElementById("divcabecer_trd_popup")) {
            espacio_iframe = window.parent.document.getElementById("Iframe_trd_popup_").clientHeight;
        }
        //$("#Contenido_superior").css("height", ($("#Button_activa_bus").height() + 13) + "px");
        $("#contenido_general").css("height", (espacio_iframe) + "px");
        $("#contenido_gred").css("height", (espacio_iframe - 30) - ($("#contenido_inferior").height() + $("#Contenido_superior").height()) + "px");
        var gridwith = (with_frame - 30);
        $('#contenido_inferior').css("width", gridwith + "px");
        $('#contenido_gred').css("width", gridwith + "px");
        $('#Contenido_superior').css("width", gridwith + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_aplicar_trd " + err.message);
    }
}
function plugin_grwedview() {
    var gridwith = $('#contenido_gred').width();
    var gridheihg = $('#contenido_gred').height();
    //LLAMA PLUGIN FIJA HIDER O TITULOS   
    if ($('#GridViewlista td').children.length > 0 && $('#GridViewlista tr:visible').length > 0) {
        //$(document).ready(function () { $('#GridViewlista').gridviewScroll({ width: gridwith, height: gridheihg }); })
    }
}
//MUEVE EL SCCROL AL ID SELECCIONADO
function mueve_scroll_data_gred(data_grid, HiddenSeleccion) {
    try {
        if ($("#" + data_grid + " td").children.length == 0 && $("#" + data_grid + " tr:visible").length == 0) {
            return true;
        }
        if ($("#" + HiddenSeleccion).val() != "-1" && $("#" + HiddenSeleccion).val() != "0") {
            var scrollableDiv = $("#" + data_grid).parent();
            var index = $("#" + data_grid + " tr[id=" + $("#" + HiddenSeleccion).val() + "]");
            //limpia todos los seleccionados
            $("#" + data_grid + " tr[id]").css({ "background-color": "transparent", "color": "Black" });
            $("#" + data_grid + " tr[id=" + $("#" + HiddenSeleccion).val() + "]").css({ "background-color": "LightSkyBlue", "color": "Red" });
            $("#" + data_grid + " tr[id=" + $("#" + HiddenSeleccion).val() + "]").each(function () {
                if (index[0].rowIndex > 1) {
                    $(scrollableDiv).scrollTop(70);
                    $(scrollableDiv).scrollTop(($(this).offset().top));
                    return true;
                }

            });
        }
    }
    catch (err) {
        alert(err.message + " Funcion mueve_scroll_data_gred");
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
function busqueda_gred(HiddenSeleccion, data_grid, contenido_busqueda, CheckboxBusqueda) {
    try {
        if ($("#" + contenido_busqueda).val() == "") {
            return false;
        }
        $("#" + HiddenSeleccion).val("0");
        var refgrid;
        var filtro;
        $("#" + data_grid + " tr[id]").css({ "background-color": "transparent", "color": "green" });
        var s = $("#" + contenido_busqueda).val().toLowerCase();
        var grid = $("#" + data_grid);

        $("#" + data_grid + " tr:has(td)").each(function () {
            var scrollableDiv = grid.parent();

            $(this).children("td").each(function (idex) {

                var tempotd = $(this).text().toLowerCase()
                var check = document.getElementById(CheckboxBusqueda).checked;
                if (check == true) {

                    if (idex >= 0) {
                        if (s == tempotd) {
                            $(this).parent().css({ "background-color": "LightSkyBlue", "color": "green" });
                            $(scrollableDiv).scrollTop(70);
                            var id_ref = $(this).parent();
                            $(scrollableDiv).scrollTop($(id_ref).offset().top);

                        }
                    }
                }

                if (check == false) {
                    if (idex >= 0) {
                        var compare = tempotd;
                        var strcompre = compare.indexOf(s);
                        if (strcompre >= 0) {
                            $(this).parent().css({ "background-color": "LightSkyBlue", "color": "green" });
                            $(scrollableDiv).scrollTop(70);
                            var id_ref = $(this).parent();
                            $(scrollableDiv).scrollTop($(id_ref).offset().top);

                        }
                    }
                }


            })
        });
    }
    catch (err) {
        alert(err.message + " funcion busqueda_gred " + err.message);
    }
}
function acti_busq_general_archivo(e, sender) {
    try {
        tecla = (document.all) ? e.keyCode : e.which;
        if (tecla == 13) {
            document.getElementById("Button_busqueda_tipo_general").click();
            e.preventDefault();
        }
    } catch (err) {
        alert(err.message + " funcion acti_busq_general_archivo " + err.message);
    }
}
function acti_busq_general_archivo_boton(e, sender) {
    try {

        document.getElementById("Button_busqueda_tipo_general").click();
        e.preventDefault();

    } catch (err) {
        alert(err.message + " funcion acti_busq_general_archivo " + err.message);
    }


}