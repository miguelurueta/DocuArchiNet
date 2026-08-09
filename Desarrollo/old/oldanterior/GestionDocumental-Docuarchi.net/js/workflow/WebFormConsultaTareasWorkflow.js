function limpiar_filtro() {
    $("#contenidobusqueda_filtro").val("");
}
$(document).ready(function () {

    $.fn.presionBoton = function (ident) {
        var semilla = " ";
        var datos = ident.split("|");
        var arc1 = datos[1];
    }
   
    $.fn.cligred = function () {
       
        $('#GridViewlista tr[id]').click(function () {
            $('#GridViewlista tr[id]').css({ "background-color": "White", "color": "Black" });
            $(this).css({ "background-color": "#e8e8f7", "color": "Black" });
            var fer = $(this).attr("id");
            $('#hdnEmailID').val(fer);
            document.getElementById("Hidden_id_tarea_sel").value = fer;
        });
        $('#GridViewlista tr[id]').dblclick(function () {
            if ($('#hdnEmailID').val() != "-1") {
                document.getElementById("Hidden_id_tarea_sel").value = fer;
                document.getElementById("Hidden_tipo_visor").value = "VISOR WORKFLOW";
                document.getElementById("Button_visor_emergente").click();
                return false;
               
            }

        });
        $('#GridViewlista tr[id]').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });

        if (ESTADO_INI == 0) {
            ESTADO_INI = 1;
            auto_zise_consulta_tarea(34);
            auto_zise_popup_visor_externo();
            auto_zise_popup_paginas_externas_libres();
            auto_zise_popup_detalle_trazabilidad();
            auto_zise_popup_imagen_respuesta();
            auto_zise_popup_detalle_radicado();
            auto_zise_popup_modal_conten_procesing_image_worflow("div_content_tabla_procesa_detail_document_proces_workflow", "contenido_procesa_detail_document_proces_workflow");
            auto_zise_popup_modal_conten_detail_notes_task_workflow("div_content_tabla_procesa_detail_notes_task_workflow", "div_content_tabla_procesa_detail_notes_task_workflow");
            auto_zise_popup_modal_conten_copy_document_expediente("div_content_tabla_procesa_detail_copy_document_expediente_wf", "div_content_tabla_procesa_detail_copy_document_expediente_wf");
        }    
    }
    var ESTADO_INI = 0;
    $('#Contenedorgrid').contextMenu('context-menu-1', {
        'Ver Documentos': {
            click: function (element) {  // element is the jquery obj clicked on when context menu launched
                var RowID = $('#hdnEmailID').val();
                if (RowID == "-1") {
                    alert("Por favor seleccione el documento");
                }
                else {
                    if ($('#hdnEmailID').val() != "-1" && $('#hdnEmailID').val() != "0" && $('#hdnEmailID').val() != "") {
                        var split = $('#hdnEmailID').val().split("-");
                        if (split.length > 0) {
                            document.getElementById("Hidden_id_tarea_sel").value = split[0];
                            document.getElementById("Hidden_tipo_visor").value = "VISOR WORKFLOW";
                            document.getElementById("Button_visor_emergente").click();
                            return false;
                        }

                    }
                }
            }
        },
        'Salir del Menu': {
            click: function (element) { }
        }
    })

})
var ESTADO_EVENT_GENERAL = "";
var INTERVAL_EVENT_GENERAL;
var REGISTRO;
var ID_TAREA_WORKFLOW_WF = 0;
$(window).on("load", function () {
    try {
        var elment = document.getElementsByClassName("da_event_captive");
        if (elment) {
            for (var i = 0; i < elment.length; i++) {
                elment[i].addEventListener("click", event_click, false);
            }
        }
       
        ini_event_page();
        ini_config_control();
        window.addEventListener("resize", rezize_event);
        ShowModalPopup("ModalPopupExtender_mensaje_personalizado_backgroundElement", "Panel_mensaje_personalizado", 100001);
        ShowModalPopup("ModalPopupExtender_edition_detalle_radicado_backgroundElement", "Panel_detalle_radicado", 100001);
        ShowModalPopup("ModalPopupExtender_edition_reasigna_tarea_workflow_sii_backgroundElement", "Panel_reasigna_tarea_workflow_sii", 100001);
        ShowModalPopup("ModalPopupExtender_edition_detail_document_proces_workflow_backgroundElement", "Panel_detail_document_proces_workflow", 100001);
        ShowModalPopup("ModalPopupExtender_edition_detail_document_proces_workflow_backgroundElement", "Panel_detail_document_proces_workflow", 100001);
        ShowModalPopup("ModalPopupExtender_edition_detail_notes_task_workflow_backgroundElement", "Panel_detail_notes_task_workflow", 100001);
        ShowModalPopup("ModalPopupExtender_edition_detail_copy_document_expediente_wf_backgroundElement", "Panel_detail_copy_document_expediente_wf", 100001);
        
    } catch (e) {
        alert(" funcion load " + e.message);
    }
});
const ini_event_page = () => {
    //inicializa boton detalle de operaciones con documentos
    let array_element = new Array;
    array_element.push(
        { id: "a_list_operation_document" }, { id: "a_list_notes_task" }, { id: "a_list_copy_document_expedient" }
    )
    for (let i = 0; i < array_element.length; i++) {
        let elment_a_document_production = document.getElementById(array_element[i].id);
        if (elment_a_document_production) {
            elment_a_document_production.addEventListener("click", handler_element_event, false);
        }
    }
}
const handler_element_event = (e) => {
    try {
        let name_ID = e.currentTarget.id;
        let resutl = "";
        switch (name_ID) {
            case "a_list_operation_document":
                resutl = handler_show_detail_document_procesing("table_boot_detail_document", "", "C-DW-DETAIL-DOCUMENT");
                if (resutl !== "YES") {
                    alert(resutl);
                }
                break;
            case "a_list_notes_task":
                resutl = handler_notes_task("table_boot_detail_notes_task_workflow", "", "C-DW-NOTE-WF");
                if (resutl !== "YES") {
                    alert(resutl);
                }
                break;
            case "a_list_copy_document_expedient":
                let resutl = handler_show_detail_document_procesing("table_boot_detail_copy_document_expdient ", "", "C-DW-DETAIL-COPY-DOCUMENT");
                if (resutl !== "YES") {
                    alert(resutl);
                }
                break;

        }
    } catch (ex) {
        return ex.mensaje;
    }
}
//--------Activa el show de las transaciones de la tarea con documentos
const handler_show_detail_document_procesing = (name_table, name_class, event_name) => {
    try {
        if (document.getElementById("Hidden_id_tarea_sel").value == 1 || document.getElementById("Hidden_id_tarea_sel").value == -1) {
            return "Debe selecionar la tarea";
        }
        ID_TAREA_WORKFLOW_WF = document.getElementById("Hidden_id_tarea_sel").value;
        event_element_menu(event_name, name_table);
        return "YES";
    } catch (ex) {
        return ex.mensaje;
    }
}
//--------Activa el show de las notas de la tarea
const handler_notes_task = (name_table, name_class, event_name) => {
    try {
        if (document.getElementById("Hidden_id_tarea_sel").value == 1 || document.getElementById("Hidden_id_tarea_sel").value == -1) {
            return "Debe selecionar la tarea";
        }
        ID_TAREA_WORKFLOW_WF = document.getElementById("Hidden_id_tarea_sel").value;
        event_element_menu(event_name, name_table);
        return "YES";
    } catch (ex) {
        return ex.mensaje;
    }
}


function event_element_menu(evento, value) {
    try {
        ESTADO_EVENT_GENERAL = "intro";
        posicion_update_pogres('progres_bar');
        INTERVAL_EVENT_GENERAL = setInterval(fx_funcion, 100);
        function fx_funcion() {
            //--Sale del evento  
            if (ESTADO_EVENT_GENERAL == "out") {
                progres_hiden('progres_bar');
                clearInterval(INTERVAL_EVENT_GENERAL);
                ESTADO_EVENT_GENERAL = "";

            }
            //--Entra a los eventos
            if (ESTADO_EVENT_GENERAL == "intro") {
                ESTADO_EVENT_GENERAL = "";
               
                //evento activa lista actividades
                if (evento == "A-REA-SII") {
                    if (ID_TAREA_WORKFLOW_WF == 0 || ID_TAREA_WORKFLOW_WF == -1) {
                        finaly_event_element_menu();
                        alert("Debe selecionar la tarea de la lista");
                        return true;
                    }
                    service_lista_actividades_workflow(ID_TAREA_WORKFLOW_WF);
                    return true;
                }
                //Evento lista usuarios grupo workflow
                if (evento == "L-USER-WF") {
                    var value_drow = $("#" + "DropDownList_list_actividad_workflow_sii").val();
                    if (value_drow == -1 || value_drow == 0) {
                        $("#DropDownList_list_usuario_workflow_sii").empty();
                        finaly_event_element_menu();
                        return true;
                    }
                    Service_lista_usuario_relacionado_actividad(value_drow, ID_TAREA_WORKFLOW_WF);
                    return true;
                }
                //Evento reasigna tarea usuario
                if (evento == "R-REA-SII") {
                    var value_drow_goup = $("#" + "DropDownList_list_actividad_workflow_sii").val();
                    if (value_drow_goup == -1 || value_drow_goup == 0) {
                        alert("Seleccione la actividad o grupo");
                        document.getElementById("DropDownList_list_actividad_workflow_sii").focus();
                        finaly_event_element_menu();
                        return true;
                    }
                    var value_drow_user = $("#" + "DropDownList_list_usuario_workflow_sii").val();
                    Service_reasigna_tarea_workflow(ID_TAREA_WORKFLOW_WF, value_drow_goup, value_drow_user);
                    return true;
                }
                //Evento lista detalles procesing imagen workflow
                if (evento == "C-DW-DETAIL-DOCUMENT") {
                    Service_lista_log_procesing_image_workflow(value, ID_TAREA_WORKFLOW_WF);
                    return true;
                }
                //Event list notes workflow
                if (evento == "C-DW-NOTE-WF") {
                    Service_lista_notas_tarea_workflow(value, ID_TAREA_WORKFLOW_WF);
                    return true;
                }
                //Event list copy document expedient
                if (evento == "C-DW-DETAIL-COPY-DOCUMENT") {
                    Service_lista_copia_documento_expediente(value, ID_TAREA_WORKFLOW_WF);
                    return true;
                }
                progres_hiden('progres_bar');
                clearInterval(INTERVAL_EVENT_GENERAL);
                ESTADO_EVENT_GENERAL = "";
            }
        }
    }
    catch (ex) {
        progres_hiden('progres_bar');
        clearInterval(INTERVAL_EVENT_GENERAL);
        ESTADO_EVENT_GENERAL = "";
        alert('event_element_menu  ' + ex.message);
    }
}
function finaly_event_element_menu() {
    progres_hiden('progres_bar');
    clearInterval(INTERVAL_EVENT_GENERAL);
    ESTADO_EVENT_GENERAL = "out";
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
            auto_zise_popup_modal_conten_procesing_image_worflow("div_content_tabla_procesa_detail_document_proces_workflow", "contenido_procesa_detail_document_proces_workflow");
            auto_zise_popup_modal_conten_detail_notes_task_workflow("div_content_tabla_procesa_detail_notes_task_workflow", "div_content_tabla_procesa_detail_notes_task_workflow");
            auto_zise_popup_modal_conten_copy_document_expediente("div_content_tabla_procesa_detail_copy_document_expediente_wf", "div_content_tabla_procesa_detail_copy_document_expediente_wf");
            auto_zise_consulta_tarea(34);
            auto_zise_popup_visor_externo();
            auto_zise_popup_paginas_externas_libres();
            auto_zise_popup_detalle_trazabilidad();
            auto_zise_popup_imagen_respuesta();
            auto_zise_popup_detalle_radicado();        
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
function prevent(event, element) {
        try {
            var fer = $(element).attr("idd");
            var tip_event = $(element).attr("tip_event");
            if (tip_event == "e_c_b_004") {
                //$('#hdnEmailID_VAL').val(fer);
                document.getElementById("Hidden_id_tarea_sel").value = fer;
                document.getElementById("Button_Trazabilidad").click();
            }
            if (tip_event == "e_c_d_005") {
                //$('#hdnEmailID_VAL').val(fer);
                document.getElementById("Hidden_id_tarea_sel").value = fer;
                document.getElementById("Button_trazabilidad_grafica").click();
            }
            if (tip_event == "e_c_d_006") {
                //$('#hdnEmailID_VAL').val(fer);
                document.getElementById("Hidden_id_tarea_sel").value = fer;
                document.getElementById("ImageButton_ista_autorizacio").click();
            }
            if (tip_event == "e_c_d_007") {
                //$('#hdnEmailID_VAL').val(fer);
                document.getElementById("Hidden_id_tarea_sel").value = fer;
                document.getElementById("Hidden_tipo_visor").value = "VISOR WORKFLOW";
                document.getElementById("Button_visor_emergente").click();
            }
            if (tip_event == "e_c_d_008") {
                //$('#hdnEmailID_VAL').val(fer);
                document.getElementById("Hidden_id_tarea_sel").value = fer;
                document.getElementById("Button_tool_activa_detalle_radicado").click();
            }
            event.preventDefault();
            element.focus();
        }
        catch (err) {
            alert(err.message + " Funcion prevent");
        }
    }
function prevent_autoriza_xx(event, element) {
    try {

        var fer = $(element).attr("id");
        var tip_event = $(element).attr("tip_event");
        if (tip_event == "descarga_xml") {
            $('#Hidden_selec_list').val(fer);
            document.getElementById("Button_dowload_xml").click();
        }

        event.preventDefault();

    }
    catch (err) {
        alert(err.message + " Funcion prevent_autoriza_xx");
    }
}
function auto_zise_popup_detalle_radicado() {
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
        var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_detalle_radicado').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_detalle_radicado').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#contenido_procesa_detalle_radicado').css("height", (document.getElementById("modal_content_detalle_radicado").clientHeight - (document.getElementById("diver_cabcera_detalle_radicado").clientHeight + document.getElementById("modal-footer_detalle_radicado").clientHeight)) + "px");
        //Para los modal que contiene gred
        $('#Panel_detalle_radicado_user').css("height", (document.getElementById("contenido_procesa_detalle_radicado").clientHeight - 5) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_detalle_radicado " + err.message);
    }
}
function activa_menu(clave) {
    try {
        if (clave == "A-REA-SII") {
            if (document.getElementById("Hidden_id_tarea_sel").value == 1 || document.getElementById("Hidden_id_tarea_sel").value == -1) {
                alert("Debe selecionar la tarea a reasignar");
            } else {
                ID_TAREA_WORKFLOW_WF = document.getElementById("Hidden_id_tarea_sel").value;
                event_element_menu("A-REA-SII", "");
            }
        }
        if (clave == "e_c_d_005") {
            document.getElementById("Button_trazabilidad_grafica").click();
        }
        if (clave == "e_c_b_004") {
            document.getElementById("Button_Trazabilidad").click();
        }
        if (clave == "e_r_r_002") {
            document.getElementById("Button_Exportar_Radicados").click();
        }
        if (clave == "e_c_d_006") {
            document.getElementById("ImageButton_ista_autorizacio").click();
        }
    } catch (err) {
        alert(err.message + " funcion activa_menu " + err.message);
    }
}
function retorna_colum_mtriz(hiden_name) {
    try {
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
    catch (err) {
        alert(err.message + " Funcion retorna_colum_mtriz");
    }
}
function auto_zise_popup_imagen_respuesta() {
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
        var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_imagen_respuesta').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_Panel_imagen_respuesta').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#Cotenedorpendiente_imagen_respuesta').css("height", (document.getElementById("modal_content_Panel_imagen_respuesta").clientHeight - (document.getElementById("Cabecerapendiente_imagen_respuesta").clientHeight)) + "px");
        //Para los modal que contiene gred
        $('#Iframe_imagen_respuesta_').css("height", (document.getElementById("Cotenedorpendiente_imagen_respuesta").clientHeight - 2) + "px");
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_imagen_respuesta");
    }
}
function auto_zise_popup_detalle_trazabilidad() {
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
        //$('#Panel_trazabilidad').css("height", (espacio_iframe - 40) + "px");
        //$('#Cotenedorpendiente_trazabilidad').css("height", (espacio_iframe - 40) + "px");
        //$('#Iframe_trazabilidad_').css("height", (espacio_iframe - 40) + "px");
        //corrige el posicionamiento horizontal del modalpopuextender, la propiedad width del panel debe estar en auto
        //var widtth_procent_left_rigth = (with_frame - document.getElementById("Panel_agregar_expediente_carpeta").clientHeight) / 2;
        //$('#Panel_agregar_expediente_carpeta').css("left", (Math.round(widtth_procent_left_rigth)) + "px");
        var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_trazabilidad').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_Panel_trazabilidad').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#Cotenedorpendiente_trazabilidad').css("height", (document.getElementById("modal_content_Panel_trazabilidad").clientHeight - (document.getElementById("Cabecerapendiente_trazabilidad").clientHeight)) + "px");
        //Para los modal que contiene gred
        $('#Iframe_trazabilidad_').css("height", (document.getElementById("Cotenedorpendiente_trazabilidad").clientHeight - 1) + "px");
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_detalle_trazabilidad");
    }
}
function auto_zise_popup_paginas_externas_libres() {
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

        //$('#PanelLibre').css("height", (espacio_iframe) + "px");
        //$('#Div9').css("height", (espacio_iframe) + "px");
        //$('#Iframelibre').css("height", (espacio_iframe) + "px");
        //corrige el posicionamiento horizontal del modalpopuextender, la propiedad width del panel debe estar en auto
        //var widtth_procent_left_rigth = (with_frame - document.getElementById("Panel_agregar_expediente_carpeta").clientHeight) / 2;
        //$('#Panel_agregar_expediente_carpeta').css("left", (Math.round(widtth_procent_left_rigth)) + "px");
        var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#PanelLibre').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_PanelLibre').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#Div_contenedor_PanelLibre').css("height", (document.getElementById("modal_content_PanelLibre").clientHeight - (document.getElementById("Div_cabecera_PanelLibre").clientHeight )) + "px");
        //Para los modal que contiene gred
        $('#Iframelibre_').css("height", (document.getElementById("Div_contenedor_PanelLibre").clientHeight - 1) + "px");
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_paginas_externas_libres");
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
            var index = $("#" + data_grid + " tr[id= " + $("#" + HiddenSeleccion).val() + "]");
            //limpia todos los seleccionados
            $("#" + data_grid + " tr[id]").css({ "background-color": "transparent", "color": "Black" });
            $("#" + data_grid + " tr[id=" + $("#" + HiddenSeleccion).val() + "]").css({ "background-color": "#E7EDF5", "color": "Black" });
            $("#" + data_grid + " tr[id= " + $("#" + HiddenSeleccion).val() + "]").each(function () {
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
function acti_busq_general_archivo(e, sender) {
    try {
        tecla = (document.all) ? e.keyCode : e.which;
        if (tecla == 13) {
            document.getElementById("Button_activa_busqueda_general").click();
            e.preventDefault();
        }
    } catch (err) {
        alert(err.message + " funcion acti_busq_general_archivo " + err.message);
    }


}
function acti_busq_general_archivo_boton(e, sender) {
    try {

        document.getElementById("Button_activa_busqueda_general").click();
        e.preventDefault();

    } catch (err) {
        alert(err.message + " funcion acti_busq_general_archivo_boton " + err.message);
    }


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
function activa_busqueda() {
    try {
        busqueda_gred('hdnEmailID', 'GridViewlista', 'contenidobusqueda', 'checkbox');
    }
    catch (err) {
        alert(err.message + " funcion activa_busqueda " + err.message);
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
        $("#" + data_grid + " tr[id]").css({ "background-color": "transparent", "color": "Black" });
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
                            $(this).parent().css({ "background-color": "LightSkyBlue", "color": "orange" });
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
                            $(this).parent().css({ "background-color": "LightSkyBlue", "color": "orange" });
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
function filtro_gred(HiddenSeleccion, data_grid, contenido_busqueda, CheckboxBusqueda) {
    try {
        if (document.getElementById(data_grid).rows.length == 1) {
            return true;
        }
        $("#" + HiddenSeleccion).val("-1");
        var refgrid;
        var filtro;
        var ito = 0;
        var confirma_hidem_fila = 0;
        var showtr;
        //$("#" + data_grid + " tr[id]").css({ "background-color": "transparent", "color": "Black" });
        var s = $("#" + contenido_busqueda).val().toLowerCase();
        //var grid = $("#" + data_grid);
        //$("#" + data_grid + " tr").hide();
        var acierto = -1;
        $("#" + data_grid + " tr:has(td)").each(function () {
            var refdif = $(this);
            var confirm = -1;
            $(this).children("td").each(function (idex) {
                var tempotd = $(this).text().toLowerCase()
                var check = document.getElementById(CheckboxBusqueda).checked;
                if (check == true) {
                    if (idex >= 0) {
                        if (s == tempotd) {
                            refdif.show();
                            confirm = 1;
                            acierto = 1;
                            return false;
                        } else {

                        }
                    }
                }

                if (check == false) {
                    if (idex >= 0) {
                        var compare = tempotd;
                        var strcompre = compare.indexOf(s);
                        if (strcompre >= 0) {
                            refdif.show();
                            acierto = 1;
                            confirm = 1;
                            return false;
                        } else {

                        }
                    }
                }

            })
            ito++;

            if (confirm == -1 && ito != 1) {
                refdif.hide();
                $("#" + data_grid).append(refdif.clone());
                refdif.remove();
            }
            if (confirm == -1 && ito == 1) {
                refdif.hide();
                $("#" + data_grid).append(refdif.clone());
                refdif.remove();
            }
            if (acierto == -1) {
                $("#" + data_grid + " tr:hidden").show();
            }
        });
    }
    catch (err) {
        alert(err.message + " funcion filtro_gred " + err.message);
    }
}
//-------------------------------------------ZONA DETALLE PROCESING IMAGEN WORKFLOW ---------------------------------
function Service_lista_log_procesing_image_workflow(table, id_tarea) {
    try {
        $.ajax('../webservice/WebServiceWorkflow.asmx/Service_lista_log_procesing_image_workflow', {
            data: "{'parameter':'" + id_tarea + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].result !== "YES") {
                    mystopfunction_update_table_boot_manager(table, data.d[0].result, data.d);
                    $find("ModalPopupExtender_edition_detail_document_proces_workflow").show();
                    auto_zise_popup_modal_conten_procesing_image_worflow("div_content_tabla_procesa_detail_document_proces_workflow", "div_content_tabla_procesa_detail_document_proces_workflow");
                    ESTADO_EVENT_GENERAL = "out";
                } else {
                    if (data.d[0].Id_log_docuarchi == -1) {
                        mystopfunction_update_table_boot_manager(table, "ZERO", data.d);
                        $find("ModalPopupExtender_edition_detail_document_proces_workflow").show();
                        auto_zise_popup_modal_conten_procesing_image_worflow("div_content_tabla_procesa_detail_document_proces_workflow", "div_content_tabla_procesa_detail_document_proces_workflow");
                        ESTADO_EVENT_GENERAL = "out";
                    } else {
                        mystopfunction_update_table_boot_manager(table, data.d[0].result, data.d);
                        $find("ModalPopupExtender_edition_detail_document_proces_workflow").show();
                        auto_zise_popup_modal_conten_procesing_image_worflow("div_content_tabla_procesa_detail_document_proces_workflow", "div_content_tabla_procesa_detail_document_proces_workflow");
                        ESTADO_EVENT_GENERAL = "out";
                    }
                   
                }
            },
            error: function (result) {
                mystopfunction_update_table(table, result.innerHTML, "");
                ESTADO_EVENT_GENERAL = "out";
            }, compelete: function () {

            }
        });
    } catch (ex) {

        alert(ex.message + " funcion Service_lista_log_procesing_image_workflow");
    }
}
//---------------------Zone list notes workflow---------------------------
function Service_lista_notas_tarea_workflow(table, id_tarea) {
    try {
        $.ajax('../webservice/WebServiceWorkflow.asmx/Service_lista_notas_tarea_workflow', {
            data: "{'parameter':'" + id_tarea + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].result !== "YES") {
                    mystopfunction_update_table_boot_manager(table, data.d[0].result, data.d);
                    document.getElementById("title_detail_notes_task_workflow").innerText = "Notas";
                    $find("ModalPopupExtender_edition_detail_notes_task_workflow").show();
                    auto_zise_popup_modal_conten_detail_notes_task_workflow("div_content_tabla_procesa_detail_notes_task_workflow", "div_content_tabla_procesa_detail_notes_task_workflow");
                    ESTADO_EVENT_GENERAL = "out";
                } else {
                    if (data.d[0].Id_Anotacion == -1) {
                        mystopfunction_update_table_boot_manager(table, "ZERO", data.d);
                        document.getElementById("title_detail_notes_task_workflow").innerText = "Notas";
                        $find("ModalPopupExtender_edition_detail_notes_task_workflow").show();
                        auto_zise_popup_modal_conten_detail_notes_task_workflow("div_content_tabla_procesa_detail_notes_task_workflow", "div_content_tabla_procesa_detail_notes_task_workflow");
                        ESTADO_EVENT_GENERAL = "out";
                    } else {
                        mystopfunction_update_table_boot_manager(table, data.d[0].result, data.d);
                        document.getElementById("title_detail_notes_task_workflow").innerText = data.d[0].title_anotacion;
                        $find("ModalPopupExtender_edition_detail_notes_task_workflow").show();
                        auto_zise_popup_modal_conten_detail_notes_task_workflow("div_content_tabla_procesa_detail_notes_task_workflow", "div_content_tabla_procesa_detail_notes_task_workflow");
                        ESTADO_EVENT_GENERAL = "out";
                    }

                }
            },
            error: function (result) {
                mystopfunction_update_table(table, result.innerHTML, "");
                ESTADO_EVENT_GENERAL = "out";
            }, compelete: function () {

            }
        });
    } catch (ex) {
        alert(ex.message + " funcion Service_lista_notas_tarea_workflow");
    }
}
//-------------------------------------------ZONA DETAIL COPY DOCUMENTO EXPEDIENTE-----------------------------------
function Service_lista_copia_documento_expediente(table, id_tarea) {
    try {
        $.ajax('../webservice/WebServiceWorkflow.asmx/Service_lista_copia_documento_expediente', {
            data: "{'parameter':'" + id_tarea + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].result !== "YES") {
                    mystopfunction_update_table_boot_manager(table, data.d[0].result, data.d);
                    $find("ModalPopupExtender_edition_detail_copy_document_expediente_wf").show();
                    auto_zise_popup_modal_conten_copy_document_expediente("div_content_tabla_procesa_detail_copy_document_expediente_wf", "div_content_tabla_procesa_detail_copy_document_expediente_wf");
                    ESTADO_EVENT_GENERAL = "out";
                } else {
                    if (data.d[0].id_relacion_wf_produccion == -1) {
                        mystopfunction_update_table_boot_manager(table, "ZERO", data.d);
                        $find("ModalPopupExtender_edition_detail_copy_document_expediente_wf").show();
                        auto_zise_popup_modal_conten_copy_document_expediente("div_content_tabla_procesa_detail_copy_document_expediente_wf", "div_content_tabla_procesa_detail_copy_document_expediente_wf");
                        ESTADO_EVENT_GENERAL = "out";
                    } else {
                        mystopfunction_update_table_boot_manager(table, data.d[0].result, data.d);
                        $find("ModalPopupExtender_edition_detail_copy_document_expediente_wf").show();
                        auto_zise_popup_modal_conten_copy_document_expediente("div_content_tabla_procesa_detail_copy_document_expediente_wf", "div_content_tabla_procesa_detail_copy_document_expediente_wf");
                        ESTADO_EVENT_GENERAL = "out";
                    }

                }
            },
            error: function (result) {
                mystopfunction_update_table_boot_manager(table, result.responseText, "");
                ESTADO_EVENT_GENERAL = "out";
            }, compelete: function () {

            }
        });
    } catch (ex) {

        alert(ex.message + " funcion Service_lista_copia_documento_expediente");
    }
}

//ZONA REASIGNA TAREA WORKFLOW
//ZONA LISTA GRUPOS USUARIOS ACTIVIDAD
function service_lista_actividades_workflow(id_tarea_workflow) {
    try {
        $.ajax('../webservice/WebServiceWorkflow.asmx/Service_lista_actividades_workflow', {
            data: "{'parameter':'" + id_tarea_workflow + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].result !== "YES") {
                    finaly_event_element_menu();
                    alert(data.d[0].result);
                } else {

                    var ITEMS_DATOS_DROW_ = new Array();
                    $.each(data.d, function (k, v) {
                        ITEMS_DATOS_DROW_.push(v);
                    });
                    $("#DropDownList_list_actividad_workflow_sii").empty();
                    $("#DropDownList_list_usuario_workflow_sii").empty();
                    var element_drow = document.getElementById("DropDownList_list_actividad_workflow_sii");
                    for (var i = 0; i < ITEMS_DATOS_DROW_.length; i++) {
                        element_drow[i] = new Option(ITEMS_DATOS_DROW_[i].nombre_actividad, ITEMS_DATOS_DROW_[i].id_actividad);
                    }
                    $find("ModalPopupExtender_edition_reasigna_tarea_workflow_sii").show();
                    finaly_event_element_menu();
                }
            }, error: function (xception, textStatus, errorThrown) {

                if (xception.status === 0) {
                    finaly_event_element_menu();
                    alert('Not connect: Verify Network.');

                } else if (xception.status == 404) {
                    finaly_event_element_menu();
                    alert('Requested page not found [404]');

                } else if (xception.status == 500) {
                    finaly_event_element_menu();
                    alert('Internal Server Error [500].' + xception.responseText);

                } else if (textStatus === 'parsererror') {
                    finaly_event_element_menu();
                    alert('Requested JSON parse failed.');

                } else if (textStatus === 'timeout') {
                    finaly_event_element_menu();
                    alert('Time out error.');

                } else if (textStatus === 'abort') {
                    finaly_event_element_menu();
                    alert('Ajax request aborted.');

                } else {
                    finaly_event_element_menu();
                    alert('Uncaught Error: ' + xception.responseText);

                }
            }
        });
    }
    catch (ex) {
        finaly_event_element_menu();
        alert('service_lista_actividades_workflow ' + ex.message);
    }
}
function Service_lista_usuario_relacionado_actividad(id_actividad_wf,id_tarea) {
    try {
        $.ajax('../webservice/WebServiceWorkflow.asmx/Service_lista_usuario_relacionado_actividad', {
            data: "{'parameter':'" + id_actividad_wf + "','id_tarea':'" + id_tarea + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].result !== "YES") {
                    finaly_event_element_menu();
                    alert(data.d[0].result);
                } else {

                    var ITEMS_DATOS_DROW_ = new Array();
                    $.each(data.d, function (k, v) {
                        ITEMS_DATOS_DROW_.push(v);
                    });

                    $("#DropDownList_list_usuario_workflow_sii").empty();
                    var element_drow = document.getElementById("DropDownList_list_usuario_workflow_sii");
                    for (var i = 0; i < ITEMS_DATOS_DROW_.length; i++) {
                        element_drow[i] = new Option(ITEMS_DATOS_DROW_[i].nombre_actividad, ITEMS_DATOS_DROW_[i].id_actividad);
                    }
                    finaly_event_element_menu();
                }
            }, error: function (xception, textStatus, errorThrown) {

                if (xception.status === 0) {
                    finaly_event_element_menu();
                    alert('Not connect: Verify Network.');

                } else if (xception.status == 404) {
                    finaly_event_element_menu();
                    alert('Requested page not found [404]');

                } else if (xception.status == 500) {
                    finaly_event_element_menu();
                    alert('Internal Server Error [500].' + xception.responseText);

                } else if (textStatus === 'parsererror') {
                    finaly_event_element_menu();
                    alert('Requested JSON parse failed.');

                } else if (textStatus === 'timeout') {
                    finaly_event_element_menu();
                    alert('Time out error.');

                } else if (textStatus === 'abort') {
                    finaly_event_element_menu();
                    alert('Ajax request aborted.');

                } else {
                    finaly_event_element_menu();
                    alert('Uncaught Error: ' + xception.responseText);

                }
            }
        });
    }
    catch (ex) {
        finaly_event_element_menu();
        alert('Service_lista_usuario_relacionado_actividad ' + ex.message);
    }
}
//ZONA SERVICE REASIGNA TAREA WORKFLOW
function Service_reasigna_tarea_workflow(id_tarea, id_actividad, id_usuario_worlflow) {
    try {
        $.ajax('../webservice/WebServiceWorkflow.asmx/Service_reasigna_tarea_workflow', {
            data: "{'id_tarea':'" + id_tarea + "','id_actividad':'" + id_actividad + "','id_usuario_worlflow':'" + id_usuario_worlflow + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].result !== "YES") {
                    finaly_event_element_menu();
                    alert(data.d[0].result);
                } else {
                    $find("ModalPopupExtender_edition_reasigna_tarea_workflow_sii").hide();
                    let stru_values = new Array();
                    //stru_values.push({ value_campo: data.d[0].nombre_actividad, name_campo: "ACTIVIDAD" });
                    //stru_values.push({ value_campo: data.d[0].nombre_usuario, name_campo: "USUARIO" });
                    //stru_values.push({ value_campo: data.d[0].cargo_usuario, name_campo: "CARGO" });
                    actualiza_gre_campo("GridViewlista", id_tarea, data.d[0].nombre_actividad, "GRUPO");
                    actualiza_gre_campo("GridViewlista", id_tarea, data.d[0].nombre_usuario, "USUARIO");
                    actualiza_gre_campo("GridViewlista", id_tarea, data.d[0].cargo_usuario, "CARGO");
                    //actualiza_table_boot("table", "0", stru_values);
                    finaly_event_element_menu();
                }
            }, error: function (xception, textStatus, errorThrown) {

                if (xception.status === 0) {
                    finaly_event_element_menu();
                    alert('Not connect: Verify Network.');

                } else if (xception.status == 404) {
                    finaly_event_element_menu();
                    alert('Requested page not found [404]');

                } else if (xception.status == 500) {
                    finaly_event_element_menu();
                    alert('Internal Server Error [500].' + xception.responseText);

                } else if (textStatus === 'parsererror') {
                    finaly_event_element_menu();
                    alert('Requested JSON parse failed.');

                } else if (textStatus === 'timeout') {
                    finaly_event_element_menu();
                    alert('Time out error.');

                } else if (textStatus === 'abort') {
                    finaly_event_element_menu();
                    alert('Ajax request aborted.');

                } else {
                    finaly_event_element_menu();
                    alert('Uncaught Error: ' + xception.responseText);

                }
            }
        });
    }
    catch (ex) {
        finaly_event_element_menu();
        alert('Service_lista_usuario_relacionado_actividad ' + ex.message);
    }
}
function actualiza_gre_campo(nombre_grid, id, valor_campo, nombre_campo) {
    try {
        $("#" + nombre_grid + " tr[id=" + id + "]").each(function () {
            var idex = -1;
            var name = nombre_campo;
            idex = colum_index_(name, nombre_grid);
            if (idex != -1) {
                if (valor_campo == "") {
                    var sas = $(this)[0].cells[idex];
                    //var nodetext = document.getElementById("ocultop");
                    var trfirst = $('#' + nombre_grid + ' tr:first').next();
                    if (sas.childElementCount == 0) {
                        $(this)[0].cells[idex].innerText = "\u00a0";
                        //nodetext.innerText = "\u00a0";
                    }
                    if (sas.childElementCount >= 1) {
                        sas.firstChild.innerHTML = "&nbsp;";
                        //nodetext.innerText = "\u00a0";
                    }
                }
                if (valor_campo !== "") {
                    var k = $(this)[0].cells[idex];
                    $(this)[0].cells[idex].innerText = valor_campo;
                    $(this)[0].cells[idex].innerHTML = valor_campo;
                }
            }
        })
        return true;
    }
    catch (err) {
        alert(err.message + " Funcion actualiza_gre_campo");
    }
}
function colum_index_(colum_name, nombre_grid) {
    try {
        var x = $('#' + nombre_grid + ' th');
        var txt = "";
        var i;
        for (i = 0; i < x.length; i++) {
            if (x[i].innerText.toUpperCase() == colum_name.toUpperCase()) {

                return i;
            }

        }
        return -1;
    }
    catch (err) {
        alert(err.message + " funcion colum_index " + err.message);
    }
}
function auto_zise_consulta_tarea(porcentaje) {
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
        var heigh_tool = document.getElementById("menu_var").offsetHeight;
        $('#contenido_general').css("height", (espacio_iframe - document.getElementById("menu_var").clientHeight) + "px");
        $('#content_').css("height", (espacio_iframe - document.getElementById("menu_var").clientHeight) + "px");
        $('#Contenedorgrid').css("height", (document.getElementById("content_").clientHeight - document.getElementById("contenido_titulo_resultado").clientHeight) + "px");
        $('#Content_consulta').css("height", (document.getElementById("contenido_general").clientHeight - document.getElementById("contenido_titulo_campos").offsetHeight) + "px");
        $('#Panelactividad').css("height", (document.getElementById("Contenedorgrid").clientHeight - 2) + "px");
        $('#Panel1').css("height", (document.getElementById("Content_consulta").offsetHeight - (document.getElementById("contenido_consulta").offsetHeight + document.getElementById("opciones_busqueda").offsetHeight )) + "px");

        
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_consulta_tarea " + err.message);
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

    $('#Panel_visor_externo').css("height", (espacio_iframe - 40) + "px");
    $('#Cotenedorpendiente_visor_externo').css("height", (espacio_iframe - 40) + "px");
    $('#Iframe_visor_externo_').css("height", (espacio_iframe - 40) + "px");
    

}
function auto_zise_popup_modal_conten_procesing_image_worflow(name_content_table, name_content) {
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
        var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_detail_document_proces_workflow').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_detail_document_proces_workflow').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#div_content_trace_grafic').css("height", (document.getElementById("modal_content_detail_document_proces_workflow").clientHeight - (document.getElementById("diver_cabcera_detail_document_proces_workflow").clientHeight + document.getElementById("modal-footer_detail_document_proces_workflow").clientHeight + 5)) + "px");
        $('#' + name_content).css("height", (document.getElementById("modal_content_detail_document_proces_workflow").clientHeight - (document.getElementById("diver_cabcera_detail_document_proces_workflow").clientHeight + document.getElementById("modal-footer_detail_document_proces_workflow").clientHeight + 5)) + "px");
        let result = resize_table_boot_manager(name_content_table, name_content);
        if (result !== "YES") {
            alert(result);
        }
       
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_modal_conten_procesing_image_worflow");
    }
    
}
function auto_zise_popup_modal_conten_detail_notes_task_workflow(name_content_table, name_content) {
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
        var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_detail_notes_task_workflow').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_detail_notes_task_workflow').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#div_content_trace_grafic').css("height", (document.getElementById("modal_content_detail_notes_task_workflow").clientHeight - (document.getElementById("diver_cabcera_detail_notes_task_workflow").clientHeight + document.getElementById("modal-footer_detail_notes_task_workflow").clientHeight + 5)) + "px");
        $('#' + name_content).css("height", (document.getElementById("modal_content_detail_notes_task_workflow").clientHeight - (document.getElementById("diver_cabcera_detail_notes_task_workflow").clientHeight + document.getElementById("modal-footer_detail_notes_task_workflow").clientHeight + 5)) + "px");
        let result = resize_table_boot_manager(name_content_table, name_content);
        if (result !== "YES") {
            alert(result);
        }

    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_modal_conten_detail_notes_task_workflow");
    }

}
function auto_zise_popup_modal_conten_copy_document_expediente(name_content_table, name_content) {
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
        var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_detail_copy_document_expediente_wf').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_detail_copy_document_expediente_wf').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        //$('#div_content_trace_grafic').css("height", (document.getElementById("modal_content_detail_copy_document_expediente_wf").clientHeight - (document.getElementById("diver_cabcera_detail_document_proces_workflow").clientHeight + document.getElementById("modal-footer_detail_document_proces_workflow").clientHeight + 5)) + "px");
        $('#' + name_content).css("height", (document.getElementById("modal_content_detail_copy_document_expediente_wf").clientHeight - (document.getElementById("diver_cabcera_detail_copy_document_expediente_wf").clientHeight + document.getElementById("modal-footer_detail_copy_document_expediente_wf").clientHeight + 5)) + "px");
        let result = resize_table_boot_manager(name_content_table, name_content);
        if (result !== "YES") {
            alert(result);
        }

    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_modal_conten_copy_document_expediente");
    }

}

$(document).on('keydown', function (e) {
    if (e.which == 9) {
        var id_element = e.srcElement.className;

        var salidadato;
        if (id_element == "DATE" || id_element == "date_2") {
            var dato = e.srcElement.value;


            if (dato == "") {

                return false;
            }


            if (salidadato == "Formato fecha no cumple") {
                alert(salidadato);
                e.preventDefault();
                return false;
            }
            var BisestA;
            var Año_F, Mes_f, Dia_f, tip;
            var numerocaracter = dato.length;
            if (numerocaracter == 10 || numerocaracter == 8) {

            }
            else {
                alert("Formato fecha no cumple");
                e.preventDefault();
                return false;
            }

            if (numerocaracter == 10) {

                Año_F = dato.substring(0, 4);
                Mes_f = dato.substring(0, 7);
                Mes_f = Mes_f.substring(7, 5);
                Dia_f = dato.substring(8, 10);
            }
            else {
                Año_F = dato.substring(0, 4);
                Mes_f = dato.substring(0, 6);
                Mes_f = Mes_f.substring(6, 4);
                Dia_f = dato.substring(6, 8);
            }

            //Verifica el formato del dia
            if (Dia_f > 31 || Dia_f == 0) {

                alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                e.preventDefault();
                return false;
            }

            //verifica el formato del mes
            if (Mes_f > 12 || Mes_f < 1) {
                alert("EM_" + Año_F + "(" + Mes_f + ")" + Dia_f);
                e.preventDefault();
                return false;
            }

            switch (Mes_f) {
                case "01":
                    if (Dia_f > 31) {
                        alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                        e.preventDefault();
                    }
                    break;

                case "02":
                    if (Dia_f % 4 == 0) {

                        BisestA = 29;
                    }
                    else {
                        BisestA = 28;
                    }
                    if (Dia_f > BisestA) {
                        alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                        e.preventDefault();
                    }
                    break;
                case "03":
                    if (Dia_f > 31) {
                        alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                        e.preventDefault();
                    }
                    break;

                case "04":
                    if (Dia_f > 30) {
                        alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                        e.preventDefault();
                    }
                    break;

                case "05":
                    if (Dia_f > 31) {
                        alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                        e.preventDefault();
                    }
                    break;

                case "06":
                    if (Dia_f > 30) {
                        alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                        e.preventDefault();
                    }
                    break;

                case "07":
                    if (Dia_f > 31) {
                        alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                        e.preventDefault();
                    }
                    break;

                case "08":
                    if (Dia_f > 31) {
                        alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                        e.preventDefault();
                    }
                    break;

                case "09":
                    if (Dia_f > 30) {
                        alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                        e.preventDefault();
                    }
                    break;

                case "10":
                    if (Dia_f > 31) {
                        alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                        e.preventDefault();
                    }
                    break;

                case "11":
                    if (Dia_f > 30) {
                        alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                        e.preventDefault();
                    }
                    break;

                case "12":
                    if (Dia_f > 31) {
                        alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                        e.preventDefault();
                    }
                    break;
            }

            if (numerocaracter == 8) {
                salidadato = Año_F + "-" + Mes_f + "-" + Dia_f;
                e.srcElement.value = salidadato;
            }

            if (numerocaracter == 10) {
                salidadato = Año_F + "-" + Mes_f + "-" + Dia_f;
                e.srcElement.value = salidadato;
            }

        }
    }
});