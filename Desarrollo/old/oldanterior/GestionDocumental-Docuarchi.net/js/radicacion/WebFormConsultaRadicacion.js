$(document).ready(function () {
    $.fn.inicio = function () {      
        //FUNCION ACTIVA SELECCION CLIK EN EL DATAGREDVIEW DE VALIDACION RADICACION
        $('#GridView_val_radicacion tr[id]').click(function () {
            $('#GridView_val_radicacion tr[id]').css({ "background": "White", "color": "Black" });
            $(this).css({ "background-color": " #e8e8f7", "color": "Black" });
            var fer = $(this).attr("id");
            $('#hdnEmailID_VAL').val(fer);
            var sele_row = $('#GridView_val_radicacion tr[id=' + $('#hdnEmailID_VAL').val() + ']');
            var columindex = colum_index("CONSECUTIVO_RADICADO");
            if (columindex == -1) {
                alert("Imposible encontrar el index de la columna ESTADO_EXPEDIENTE");
                return false;
            }
            if ($('#hdnEmailID_VAL').val() != "-1") {
                var recordgred = $('#GridView_val_radicacion tr[id=' + $('#hdnEmailID_VAL').val() + ']');
                document.getElementById("Hidden_id_tarea_sel").value = recordgred[0].cells[columindex].innerText;
                document.getElementById("Hidden_tipo_visor").value = "VISOR RADICADOR";
            }
                      
        });

        $('#GridView_val_radicacion tr[id]').dblclick(function (event) {

            if ($('#hdnEmailID_VAL').val() != "-1") {
                var recordgred = $('#GridView_val_radicacion tr[id=' + $('#hdnEmailID_VAL').val() + ']');
                var idex = colum_index('CONSECUTIVO_RADICADO');
                if (idex != -1) {
                    document.getElementById("Hidden_id_tarea_sel").value = recordgred[0].cells[idex].innerText;
                    document.getElementById("Hidden_tipo_visor").value = "VISOR RADICADOR";
                    document.getElementById("Button_visor_emergente").click();
                    event.preventDefault();
                    return false;
                }
            }

        });
        //ASIGNA EL CURSOR DE SELECCION CUANDO PASA EL CURSOR EN EL DATAGREDVIEW DE VALIDACION RADICACION
        $('#GridView_val_radicacion tr[id]').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });
        $('#GridView_list_documento_relacion tr[idd]').click(function () {
            try {
                $('#GridView_list_documento_relacion tr[idd]').css({ "background": "White", "color": "Black" });
                $(this).css({ "background-color": "#e8e8f7", "color": "Black" });
                var tag_split = $(this).attr("idd").split("|");
                Set_documento_seleccionado(tag_split[1], tag_split[0]);
                //$('#hiden_seleccion_documento').val($(this).attr("idd"));
                //$('#hiden_seleccion_documento_id').val($(this).attr("id"));
            }
            catch (err) {
                alert(err.message + " Funcion clik");
            }
        });
        $('#GridView_list_documento_relacion tr[id]').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });
        auto_zise_popup_detalle_trazabilidad();
        auto_zise_popup_editar_radicados();
        auto_zise_popup_plantilla_validacion();
        auto_zise_popup_validacion_radicados();
        auto_zise_popup_visor_externo();
        auto_zise_popup_campo_dinamico();
        auto_zise_popup_detalle_transacciones();
        actuo_zise_popup_compartir_correo_electronico();
        auto_zise_popup_cambio_tipo_tramite();
        auto_size_imprimir();
        auto_zise_popup_editar_radicados_salientes();
        auto_zise_popup_documentos_radicados_relacionados();
        auto_zise_popup_detalle_radicado();
        auto_zise_popup_adjunta_documento_workflow();
      
    }

    $('#contenido_datagrid_val_radicacion').contextMenu('context-menu-2', {
  
        'Ver documentos': {
            click: function (element) {  
                if ($('#hdnEmailID_VAL').val() != "-1") {
                    var recordgred = $('#GridView_val_radicacion tr[id=' + $('#hdnEmailID_VAL').val() + ']');
                        var idex = colum_index('CONSECUTIVO_RADICADO', 'GridView_val_radicacion');
                        if (idex != -1) {
                            document.getElementById("Hidden_id_tarea_sel").value = recordgred[0].cells[idex].innerText;
                            document.getElementById("Hidden_tipo_visor").value = "VISOR RADICADOR";
                            document.getElementById("Button_visor_emergente").click();
                            return false;
                        }
                    }
                
                
            }
        },
        'Salir del Menu': {
            click: function (element) { }
        }
    }


    );
})
var response_sevice_java_cinsulta_radicado;
var ESTADO_EVENT_GENERAL = "";
var INTERVAL_EVENT_GENERAL;
$(window).on("load", function () {
    try {
        var elment = document.getElementsByClassName("da_event_captive");
        if (elment) {
            for (var i = 0; i < elment.length; i++) {
                elment[i].addEventListener("click", event_click, false);
            }
        }
        //inicializa boton delte multiple row ventana enlace radicado
        let elment_delete_row_several_rad = document.getElementById("delete_row_several_rad");
        if (elment_delete_row_several_rad) {
            elment_delete_row_several_rad.addEventListener("click", handler_element_event, false);
        }
        ini_event_page();
        window.addEventListener("resize", rezize_event);
        ShowModalPopup("ModalPopupExtender_mensaje_personalizado_backgroundElement", "Panel_mensaje_personalizado", 100002);
        ShowModalPopup("ModalPopupExtender_edition_detalle_radicado_backgroundElement", "Panel_detalle_radicado", 100001);
        ShowModalPopup("ModalPopupExtender_sube_documento_adjunto_backgroundElement", "Panel_sube_documento_adjunto", 100001);
        ShowModalPopup("ModalPopupExtender_edition_actualiza_tipologia_documental_backgroundElement", "Panel_actualiza_tipologia_documental", 100001);
        ShowModalPopup("ModalPopupExtender_valiacion_plantilla_backgroundElement", "Panel_valiacion_plantilla", 100001);
        ShowModalPopup("ModalPopupExtender_edition_interface_regitra_meta_dato_backgroundElement", "Panel_interface_regitra_meta_dato", 100002);
        GetLista_radicados_general('TextBox_buequeda_general');
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
        actuo_zise_popup_compartir_correo_electronico();
        auto_zise_popup_editar_radicados();
        auto_zise_popup_plantilla_validacion();
        auto_zise_popup_visor_externo();
        auto_zise_popup_campo_dinamico();
        auto_zise_popup_detalle_trazabilidad();
        auto_zise_popup_detalle_transacciones();
        auto_zise_popup_validacion_radicados();
        auto_zise_popup_cambio_tipo_tramite();
        auto_size_imprimir();
        auto_zise_popup_editar_radicados_salientes();
        auto_zise_popup_documentos_radicados_relacionados();
        auto_zise_popup_detalle_radicado();
        auto_zise_popup_adjunta_documento_workflow();
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
//CONTROLA LOS EVENTOS DE LOS IMPUT
function event_element_clic(event, e) {
    try {
        ESTADO_EVENT_GENERAL = "intro";
        posicion_update_pogres('progres_bar');
        e.disabled = true;
        INTERVAL_EVENT_GENERAL = setInterval(fx_funcion, 400);
        function fx_funcion() {
            //--Sale del evento
            if (ESTADO_EVENT_GENERAL == "out") {
                progres_hiden('progres_bar');
                e.disabled = false;
                clearInterval(INTERVAL_EVENT_GENERAL);
                ESTADO_EVENT_GENERAL = "";
            }
            //--Entra al evento
            if (ESTADO_EVENT_GENERAL == "intro") {
                ESTADO_EVENT_GENERAL = "";
                if (e.id == "Button_registra_meta") {
                    agrega_meta_dato_documento(ID_IMAGEN_META_DATO, GABINETE_META_DATO, RADICADO_META_DATO, ID_TAREA_META_DATO, 1, 1, 1, ID_BOTON_META_DATO);
                    return true;
                }

                e.disabled = false;
                clearInterval(INTERVAL_EVENT_GENERAL);
                ESTADO_EVENT_GENERAL = "";
            }
        }
    }
    catch (ex) {
        alert('event_element_clic  ' + ex.message);
    }
}
function event_element_menu(evento, tip_event) {
    try {
        if (ESTADO_EVENT_GENERAL == "execute") {
            return true;
        }
        ESTADO_EVENT_GENERAL = "intro";
        posicion_update_pogres('progres_bar');
        INTERVAL_EVENT_GENERAL = setInterval(fx_funcion, 400);
        function fx_funcion() {
            //--Sale del evento
            if (ESTADO_EVENT_GENERAL == "out") {
                progres_hiden('progres_bar');
                clearInterval(INTERVAL_EVENT_GENERAL);
                ESTADO_EVENT_GENERAL = "";
            }
            //--Entra al evento
            if (ESTADO_EVENT_GENERAL == "intro") {
                ESTADO_EVENT_GENERAL = "execute";
                //--Crea meta dato y firma documento
                if (evento == "firma_doc_selecion_rad") {
                    var spliter = tip_event.split("|");
                    ITEMS_DATOS_SIST_META_ARCHIVO = new Array();
                    ID_IMAGEN_META_DATO = spliter[1];
                    GABINETE_META_DATO = spliter[0];
                    RADICADO_META_DATO = spliter[2];
                    ID_TAREA_META_DATO = spliter[5];
                    ID_BOTON_META_DATO = spliter[8];
                    service_crea_interface_registro_meta_dato(spliter[1], spliter[0], spliter[4]);
                    return true;
                }
                //sube documento enlace
                if (evento == "C-DW-RD") {
                    inicializa_upload_file_client(tip_event);
                    document.getElementById("Hidden_tip_adjunt").value = "rad";
                    parameter_upload(ESTADO_EVENT_GENERAL, "WORKFLOW", "Button_tool_activa_sube_documento", "multiple", tip_event);
                    return true;
                }
                //Delete row images on greed search on settled
                if (evento == "C-DW-DEL-IMAGE") {
                    event_multiple_row("", "GridView_list_documento_relacion", "elimina_doc_relacionado_consulta_radicado");
                    return true;
                }
                progres_hiden('progres_bar');
                clearInterval(INTERVAL_EVENT_GENERAL);
                ESTADO_EVENT_GENERAL = "";
            }
        }
    }
    catch (ex) {
        alert('event_element_menu  ' + ex.message);
    }
}
function inicializa_tipo_adjunto_documento(event, element, value_sel) {
    try {
        //Activa subir el documento desde el visor 
        if (value_sel == "C-DW-VIS") {
            event_element_menu("C-DW-VIS", "adjunto_doc_visor");
        }
        //Activa subir el documento desde enlace
        if (value_sel == "C-DW-RD") {
            event_element_menu("C-DW-RD", "adjunto_doc_visor");
        }
    }
    catch (err) {
        alert(err.message + " Funcion inicializa_tipo_adjunto_documento");
    }
}
const handler_element_event = (e) => {
    try {
        let name_ID = e.currentTarget.id;
        switch (name_ID) {
            case "sing_multiple_file": {
                event_element_click_promise(e);
                break;
            }
            case "delete_row_several_rad":  //Elimina multiplex row documento relacionado  
                hanler_delete_row_several("GridView_list_documento_relacion", "chek_selecion_list_rad", "C-DW-DEL-IMAGE");
                break;
        }

    } catch (ex) {
        alert(ex.mensaje);
    }
}
const ini_event_page = () => {
    //Active copy proceedings
    let array_element = new Array;

    //active note procesing
    array_element = new Array;
    array_element.push(
        { id: "sing_multiple_file" }
    );
    for (let i = 0; i < array_element.length; i++) {
        let elment_a_document_production = document.getElementById(array_element[i].id);
        if (elment_a_document_production) {
            elment_a_document_production.addEventListener("click", handler_element_event, false);
        }
    }

}
const event_element_click_promise = async (e) => {
    let name_control = e.currentTarget.id;
    try {
        let result = "";
        delete_alert_boot();
        e.currentTarget.disabled = true;
        posicion_update_pogres('progres_bar');
        if (name_control == "sing_multiple_file") {
            progres_hiden('progres_bar');
            let option = ({
                module: "5", valida_firma: "1",
                name_campo_estado_firma: "", name_tipo_table: "aspnettable",
                name_table: "GridView_list_documento_relacion",
                AtributeSingAspNet: "idd_rad",
                NameControlParent: "contenguia",
                content_error: "div_error_content_rad"
            });
            result = await LoadStampMultipleSing(option);
            if (result !== "YES") {
                alert_bot(result, 'warning', "div_error_content_rad");
            }

        }

    }
    catch (ex) {
        alert_bot(ex.message, 'warning', "div_error_content_general");
    } finally {
        progres_hiden('progres_bar');
        document.getElementById(name_control).disabled = false;
    }
}

//----------------------//-----------------------
//ZONE DELETE ROW DOCUMENTS
//---------------------//------------------------
//Activa la funcion de eiliminar multiplex row
const hanler_delete_row_several = (name_table, name_class, event_name) => {
    ITEMS_IMAGE_LIST_WF = new Array;
    ITEMS_IMAGE_LIST_WF = table_gred_select_check_item(name_table, name_class);
    if (ITEMS_IMAGE_LIST_WF.length == 0) {
        alert("Debe seleccionar los documentos de la lista");
        return true;
    }
    event_element_menu(event_name, "");

}
//Delete row link workflow
const elimina_row_gred_enlace = (table_gred, atrib, value_item) => {
    try {
        let id_dent = $("#" + table_gred + " tr[" + atrib + "=" + value_item + "]");
        if (id_dent.length > 0) {
            $("#" + table_gred + " tr[" + atrib + "=" + value_item + "]").remove();
            decrementa_documento_relacion();
        }
        return "YES";
    } catch (ex) {
        return "Funcion name elimina_row_gred_enlace error " + ex.mensaje;
    }
}
const decrementa_documento_relacion = () => {
    try {
        var element_table = document.getElementById("GridView_list_documento_relacion");
        var numero_fila = element_table.rows.length - 1;
        document.getElementById("Hidden_numero_doc_rel").value = numero_fila;
        document.getElementById("Label_documentos").innerHTML = "Documentos " + numero_fila;
    }
    catch (err) {
        alert(err.message + " Funcion decrementa_documento_relacion_estado");
    }
}
//ZONA CHEK DATA GREED
const table_gred_on_click_check=(elemen, table, name_class_check)=> {
    try {
        var value_ = true;
        if (elemen.checked == true) {
            value_ = true;
        } else {
            value_ = false;
        }
        $('#' + table + ' .' + name_class_check).each(function () {
            var nod = $(this);
            nod[0].checked = value_;
        });

    } catch (ex) {
        alert("Inconsistencia funcion table_gred_on_click_check " + ex.mensaje);
    }
}
const table_gred_select_check_item=(table, name_class_check)=> {
    try {
        let items_check_id = new Array;
        $('#' + table + ' .' + name_class_check).each(function () {
            var nod = $(this);
            if (nod[0].checked == true) {
                if (nod[0].getAttribute("chek_id")) {
                    items_check_id.push({
                        id_item: nod[0].getAttribute("chek_id")
                    });
                }
            }
        });
        return items_check_id;
    } catch (ex) {
        alert("Inconsistencia funcion table_gred_select_check_item " + ex.mensaje);
    }
}

//ZONA LOAD FILE
function start_file_save_UploadFile() {
    try {
        var funcion_name = ""; //Nombre de la funcion java que actualiza el elemento
        var evento_adjunta = ""; //Nombre del evento que adunta el documento
        var tipo_adjunta = 0; // Guarda si tipo documento de respueta se adunta formal o libre   1. formal  2.Libre
        var element_html_actuliza = ""; //Guarda el nombre del elemento que se actualiza
        var element_update_panel = ""; //Guarda el nombre del boton que actualiza el update panel
        var id_respuesta = 0; //Guarda el id respuesta
        var estado_relacion = 0; //Determina si el documento sube como relacionado
        var id_tipo_docuental = 0; //Guarda el tipo documental que se envia para guardar el documento
        var estado_adjunto = 0; //Determina si el documento sube como adjunto 
        var element_parent = "";  //Guarda el nombre del modal que contiene el control upload
        var numero_documento_relacionado = 0;
        var element_isert_table = "wf";
        if (document.getElementById("Hidden_tip_adjunt")) {
            element_isert_table = document.getElementById("Hidden_tip_adjunt").value;
        }
        var imp_load = document.getElementById('file_element_' + CONTEN_NAME_UPLOAD_FILE);
        if (CONTEN_NAME_UPLOAD_FILE == "adjunto_doc_visor") {
            var chek_relacion = document.getElementById("CheckBox_relacionado_radicado_adj");
            var chek_adjunto = document.getElementById("Check_anexo_radicado_adj");
            if (chek_relacion) {
                if (chek_relacion.checked == true) {
                    estado_relacion = 1;
                } else {
                    estado_relacion = 0;
                }
                funcion_name = "insert_row_documento_relacionado";
                evento_adjunta = "GESTION_RESPUESTA";
                element_html_actuliza = "";
            }

            if (chek_adjunto) {
                if (chek_adjunto.checked == true) {
                    estado_adjunto = 1;
                    funcion_name = "actualiza_contador_imagen";
                    evento_adjunta = "GESTION_RESPUESTA";
                    element_html_actuliza = "LabelConteo";

                } else {
                    estado_adjunto = 0;
                }

            }
            element_update_panel = "Button_update_update_adjunto_doc_visor";
            drow_tipo = document.getElementById("DropDownList_adjunta_documento");
            if (drow_tipo.value != "") {
                id_tipo_docuental = drow_tipo.value;
            }
            element_parent = "ModalPopupExtender_sube_documento_adjunto";
            numero_documento_relacionado = document.getElementById("GridView_list_documento_relacion").rows.length - 1;
            star_copy_interval_file_Upload(estado_adjunto, estado_relacion, id_tipo_docuental, funcion_name, element_parent, evento_adjunta,
                numero_documento_relacionado, element_html_actuliza, element_update_panel, id_respuesta, tipo_adjunta, element_isert_table, "", 0);
        }



    } catch (err) {
        alert(err.mensaje + " function start_file_save_UploadFile")
    }
}
function auto_zise_popup_adjunta_documento_workflow() {
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

        var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_sube_documento_adjunto').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_sube_documento_adjunto').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#Div_contenido_adjunta').css("height", (document.getElementById("modal_content_sube_documento_adjunto").clientHeight - (document.getElementById("Div_cabecera").clientHeight)) + "px");
        //Para los modal que contiene gred
        var elment_heig = document.getElementById("content_option_chek_adjunto_doc_visor").clientHeight + document.getElementById("content_boton_adjunto_doc_visor").clientHeight + document.getElementById("content_pie_title_adjunto_doc_visor").clientHeight + 20;
        $('#conten_file_element_adjunto_doc_visor').css("height", (document.getElementById("Div_contenido_adjunta").clientHeight - elment_heig) + "px");

    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_adjunta_documento_workflow " + err.message);
    }
}
//TERMINA ZONA LOAD FILE
function Set_documento_seleccionado(id_imagen_, nombre_gabinete_) {
    try {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "../webservice/WebServiceDocuarchi.asmx/Set_documento_seleccionado",
            data: "{'id_imagen':'" + id_imagen_ + "'," + "'nombre_gabinete':'" + nombre_gabinete_ + "'}",
            dataType: "json",
            success: function (data) {
                //response(data.d);
                if (data.d !== "YES") {
                    alert(data.d);
                    element.checked = false;
                }
            },
            error: function (result) {
                alert("Error......" + result);
                event.preventDefault();
            }
        });
    }
    catch (err) {
        alert(err.message + " Funcion Set_documento_seleccionado");
    }
}
function acti_busq_general_archivo(e, sender) {
    try {
        tecla = (document.all) ? e.keyCode : e.which;
        if (tecla == 13) {
            document.getElementById("Button_consulta_val_radicacion_general").click();
            e.preventDefault();
        }
    }   catch (err) {
        alert(err.message + " funcion acti_busq_general_archivo " + err.message);
    }


}
function acti_busq_general_archivo_boton(e, sender) {
    try {      
        document.getElementById("Button_consulta_val_radicacion_general").click();
            e.preventDefault();       
    } catch (err) {
        alert(err.message + " funcion acti_busq_general_archivo " + err.message);
    }
}
function preven_event_restor_search(e, sender) {
    try {
        document.getElementById("Button__consulta_val_radicacion_rest").click();
        e.preventDefault();
    } catch (err) {
        alert(err.message + " funcion preven_event_restor_search " + err.message);
    }
}

function auto_zise_popup_documentos_radicados_relacionados() {
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
        $('#Panel_admon_documentos').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_admon_documentos').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#contenido_procesa_admon_documentos').css("height", ($("#modal_content_admon_documentos").height() - (document.getElementById("divcabecer2_admon_documentos").clientHeight + 60)) + "px");
        //Para los modal que contiene gred
        //$('#content_data_grid').css("height", (document.getElementById("contenido_procesa_usu_rel_solicitud").clientHeight - document.getElementById("contenido_titulo_val_radicacion_documentos").clientHeight) + "px");
        $("#conte_waper").css("height", (($("#contenido_procesa_admon_documentos").height() - 60)) + "px");
        $("#da_content_wraper_").css("height", (($("#contenido_procesa_admon_documentos").height() - 60)) + "px");
        $("#Contenedorderecho_").css("height", (($("#contenido_procesa_admon_documentos").height() - 60)) + "px");
        $("#Contentizquierdo_").css("height", (($("#contenido_procesa_admon_documentos").height() - 60)) + "px");
        $("#div_treview_archivo").css("height", ($("#Contentizquierdo_").height() - 30) + "px");
        $("#Paneltreview").css("height", ($("#Contentizquierdo_").height() - 30) + "px");
        document.getElementById('Area_Visor').style.height = ((document.getElementById("Contenedorderecho_").clientHeight - document.getElementById("div_cerrar").clientHeight)) + "px";
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_documentos_radicados_relacionados " + err.message);
    }
}
function dispalyInterfaceEscaner() {
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
        //document.getElementById('Are_Digitalizacion').style.width = (document.getElementById("Contenedorderecho").clientWidth - 5) + "px";
        document.getElementById('Are_Digitalizacion').style.display = 'block';
        document.getElementById('Area_Visor').style.display = 'none';
    }
    catch (err) {
        alert(err.message + " Funcion dispalyInterfaceEscaner");
    }

}
function dispalyVisorEmergente() {
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
        document.getElementById('Are_Digitalizacion').style.display = 'none';
        document.getElementById('Area_Visor').style.display = 'block';
        document.getElementById('Area_Visor').style.height = ((document.getElementById("Contenedorderecho_").clientHeight - document.getElementById("div_cerrar").clientHeight)) + "px";

    }
    catch (err) {
        alert(err.message + " Funcion dispalyVisorEmergente");
    }
}
function GetLista_radicados_general(name_texbox) {
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
                    url: "../webservice/WebServiceRadicacion.asmx/GetLista_radicados_general",
                    data: "{'DName':'" + document.getElementById(name_texbox).value + "'}",
                    dataType: "json",
                    type: "POST",
                    traditional: true,
                    processData: false,
                    contentType: "application/json; charset=utf-8",
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
                
                return false;
            },
            select: function (event, ui) {
                //var terms = split(this.value);
                // remove the current input
                //terms.pop();
                // add the selected item
                //terms.push(ui.item.value);
                // add placeholder to get the comma-and-space at the end
                //terms.push("");
                this.value = ui.item.value;
                document.getElementById("TextBox_buequeda_general").value = ui.item.label;
                document.getElementById("Button_consulta_val_radicacion_general").click();
                return false;
            }

                ,minLength: 3, max: 10, scroll: true
        });
}
function acti_busq_lista(e, sender) {
    try {
        tecla = (document.all) ? e.keyCode : e.which;
        if (tecla == 13) {       
            busqueda_gred('hdnEmailID_VAL', 'GridView_val_radicacion', 'TextBox_busqueda', 'CheckBox_busqueda');
            e.preventDefault();
        }
    } catch (err) {
        alert(err.message + " funcion acti_busq_lista " + err.message);
    }
}
function activa_menu(clave) {
    try {
        if (clave == "i_r_r_001") {
            document.getElementById("Button_Reimpresion_radicado").click();
        }
        if (clave == "e_r_r_002") {
            document.getElementById("Button_Exportar_Radicados").click();
        }

        if (clave == "d_d_r_003") {
            document.getElementById("Button_certificado_radicado").click();
        }

        if (clave == "e_c_b_004") {
            document.getElementById("Button_Editar_radicados").click();
        }

        if (clave == "e_c_d_005") {
            document.getElementById("Button_editar_campos_dinamicos_consulta").click();
        }

        if (clave == "c_t_t_006") {
            document.getElementById("Button_editar_tipo_tramite").click();
        }
        if (clave == "t_r_s_006") {
            document.getElementById("Button_Log_respuesta").click();
        }
        if (clave == "e_r_s_007") {
            document.getElementById("Button_Trazabilidad").click();
        }
        if (clave == "d_r_s_008") {
            document.getElementById("Button_detalle_radicado").click();
        }
        if (clave == "l_t_r_009") {
            document.getElementById("Button_log").click();
        }
        if (clave == "n_u_a_010") {
            document.getElementById("Button_compartir").click();
        }
        if (clave == "n_u_c_011") {
            document.getElementById("Button_notificar_envio").click();
        }
        if (clave == "v_d_r_012") {
            document.getElementById("Button_visor_emergente").click();
        }
        if (clave == "t_r_s_020") {
            document.getElementById("Button_log").click();
        }
        if (clave == "d_r_r_010") {
            document.getElementById("Button_tool_activa_lista_documentos").click();
        }
    } catch (err) {
        alert(err.message + " funcion activa_menu " + err.message);
    }
}
function prevent(event, element) {
    try {
        var fer_id = $(element).attr("id");
        var fer = $(element).attr("idd");
        var idd_rad = $(element).attr("idd_rad");
        var tip_event = $(element).attr("tip_event");
        if (tip_event == "v_d_r_012") {
            $('#hdnEmailID_VAL').val(fer);
            var recordgred = $('#GridView_val_radicacion tr[id=' + $('#hdnEmailID_VAL').val() + ']');
            var idex = colum_index('CONSECUTIVO_RADICADO');
            if (idex != -1) {
                document.getElementById("Hidden_id_tarea_sel").value = recordgred[0].cells[idex].innerText;
                document.getElementById("Hidden_tipo_visor").value = "VISOR RADICADOR";
                document.getElementById("Button_visor_emergente").click();
            } else {
                alert("Imposible encontrar el campo CONSECUTIVO_RADICADO ")
            }
            
        }
       
        if (tip_event == "vis_doc_selecion_rad") {
            var ref_idd = $(element).attr("idd_rad");
            var ref_id_rad = $(element).attr("id_rad");
            $('#hiden_seleccion_documento').val(ref_idd);
            var spliter;
            var text_content = document.getElementById("hiden_seleccion_documento").value;
            if (text_content != "") {
                spliter = text_content.split("|");
                if (spliter.length > 3) {
                    document.getElementById("titel_visor").innerHTML = reemplazarAcentos(spliter[4]);
                }
            }
            $('#GridView_list_documento_relacion tr[id_rad]').css({ "background": "White", "color": "Black" });
            $('#GridView_list_documento_relacion tr[id_rad=' + ref_id_rad + ']').css({ "background-color": "#e8e8f7", "color": "Black" });
            document.getElementById("Button_tool_visualiza_documento").click();     
            
        }
        if (tip_event == "cambia_doc_selecion_rad") {
            var ref_idd = $(element).attr("idd_rad");
            var ref_id_rad = $(element).attr("id_rad");
            $('#hiden_seleccion_documento').val(ref_idd);
            $('#hiden_seleccion_documento_id').val(ref_id_rad);
            document.getElementById("Button_tool_activa_cambia_tipologia").click();
        }
        if (tip_event == "elim_doc_selecion_rad") {
           
            ITEMS_IMAGE_LIST_WF = new Array;
            ITEMS_IMAGE_LIST_WF.push({
                id_item: $(element).attr("id_rad")
            });
            event_element_menu("C-DW-DEL-IMAGE", "");
            
        }
        if (tip_event == "e_c_b_004") {
            $('#hdnEmailID_VAL').val(fer);
            document.getElementById("Button_Editar_radicados").click();
        }
        if (tip_event == "e_c_d_005") {
            $('#hdnEmailID_VAL').val(fer);
            document.getElementById("Button_editar_campos_dinamicos_consulta").click();
        }
        if (tip_event == "c_t_t_006") {
            $('#hdnEmailID_VAL').val(fer);
            document.getElementById("Button_editar_tipo_tramite").click();
        }
        //Versionamiento de documento
        if (tip_event == "lista_ver_doc_selecion_rad") {
            let ref_idd = $(element).attr("idd_rad");
            let spliter = ref_idd.split("|");
            let id_imagen = spliter[1];
            let gabinete = spliter[0];
            NAME_GABINETE_VERSION = gabinete;
            ID_IMAGEN_VERSION = id_imagen;
            let name_class_element_icono_aspnet = spliter[8];
            let DocumentoTilte = spliter[4];
            let option = ({
                IdImagen: id_imagen, Gabinete: gabinete, TipoModulo: 5,
                ContentError: "div_error_content_rad", name_class_element_icono_aspnet: name_class_element_icono_aspnet,
                DocumentoTilte: DocumentoTilte, NameModulo: "RADICACION", NameControlParent: "contenguia"
            });
            ShowListVersionDocumento(option);
        }
        //Activa remplazo versión documento asignado
        if (tip_event == "remplaza_ver_doc_selecion_rad") {
            let ref_idd = $(element).attr("idd_rad");
            let spliter = ref_idd.split("|");
            let id_imagen = spliter[1];
            let gabinete = spliter[0];
            let name_class_element_icono_aspnet = spliter[8];
            let DocumentoTilte = spliter[4];
            let option =
                ({
                    IdImagen: id_imagen, Gabinete: gabinete, name_class_element_icono_aspnet: name_class_element_icono_aspnet,
                    DocumentoTilte: DocumentoTilte, OptionRemPlazo: "RAD", ContentError: "div_error_content_rad",
                    NameControlParent: "contenguia"
                })
            ShowActivaOpcionRemplazo(option);
        }
        if (tip_event == "firma_doc_selecion_rad") {
            ars_sele = [];
            var ref_id = $(element).attr("idd_rad") + "|" + element.id;
            if (ref_id != "") {
                var spliter = ref_id.split("|");
                let confi = confirm("¿Desea firmar el documento (" + spliter[4] + ")?");
                if (confi == false) {
                    return true;
                }
                if (spliter.length > 3) {
                    stamp_file_doument_genral(spliter[1], "aspnettable", spliter[0], element.id, spliter[8], "div_error_content_rad", "1", "fa-file-certificate");
                } else {
                    alert("Inconsistencia en el evento, spliter incompleto (" + spliter.length + ")");
                }
            }

        }
        event.preventDefault();
        element.focus();
    }
    catch (err) {
        alert(err.message + " Funcion prevent");
    }
}
function confirma_eliminar_documento_relacion(mensaje) {
    try {
        if (document.getElementById("hiden_seleccion_documento").value == "") {
            document.getElementById("HiddenPROMP").value = 1;
            return false;
        }
        var x = 1;
        var r = confirm(mensaje);
        if (r == true) {
            x = "0";
        }
        else {
            x = "1";
        }
        document.getElementById("HiddenPROMP").value = x;
    } catch (err) {
        alert(err.message + " Funcion confirma_eliminar_documento_relacion");
    }
}
var reemplazarAcentos = function (cadena) {
    var chars = {
        "á": "a", "é": "e", "í": "i", "ó": "o", "ú": "u",
        "à": "a", "è": "e", "ì": "i", "ò": "o", "ù": "u", "ñ": "n",
        "Á": "A", "É": "E", "Í": "I", "Ó": "O", "Ú": "U",
        "À": "A", "È": "E", "Ì": "I", "Ò": "O", "Ù": "U", "Ñ": "N"
    }
    var expr = /[áàéèíìóòúùñ]/ig;
    var res = cadena.replace(expr, function (e) { return chars[e] });
    return res;
}
function prevent_cerrar(event, element) {
    try {
        //Evita el posback del boton
        event.preventDefault();
        dispalyInterfaceEscaner();

    }
    catch (err) {
        alert(err.message + " Funcion prevent ");
    }
}
function auto_zise_popup_validacion_radicados() {
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
    $('#Contentizquierdo').css("height", ((espacio_iframe - 5) - document.getElementById("menu_var").clientHeight) + "px");
    $('#sidebar_').css("height", ((espacio_iframe - 5) - document.getElementById("menu_var").clientHeight) + "px");
    $("#contenido_controles_consulta").css("height", (document.getElementById("Contentizquierdo").clientHeight) - (document.getElementById('contenido_titulo_controles_consulta').clientHeight + document.getElementById('contenido_controles_buton_consulta').clientHeight) + "px");
    $("#_Panelvalidacion_val_radicacion").css("height", (document.getElementById("Contentizquierdo").clientHeight) - (document.getElementById('contenido_titulo_controles_consulta').clientHeight + document.getElementById('contenido_controles_buton_consulta').clientHeight) + "px");
    $('#Contenedorderecho').css("height", ((espacio_iframe - 5) - document.getElementById("menu_var").clientHeight) + "px");
    $("#contenido_datagrid_val_radicacion").css("height", (document.getElementById("Contenedorderecho").clientHeight - document.getElementById('contenido_titulo_val_radicacion').clientHeight) + "px");
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_validacion_radicados");
    }
}
function eliminar_fila_data_gred_simple_(gred, nombre_hiden, nombre_hiden_, seter, seter_) {
    try {
        var id_dent = $("#" + gred + " tr[id_rad=" + $("#" + nombre_hiden).val() + "]");
        
        if (id_dent.length > 0) {
            $("#" + gred + " tr[id_rad=" + $("#" + nombre_hiden).val() + "]").remove();
            $('#' + nombre_hiden).val(seter);
            $('#' + nombre_hiden_).val(seter_);
            decrementa_documento_relacion_estado();
        }

    }
    catch (err) {
        alert(err.message + " Funcion eliminar_fila_data_gred_simple_");
    }
}
function decrementa_documento_relacion_estado() {
    try {
        var element_table = document.getElementById("GridView_list_documento_relacion");
        var numero_fila = element_table.rows.length - 1;
        document.getElementById("Hidden_numero_doc_rel").value = numero_fila;
        document.getElementById("Label_documentos").innerHTML = "Documentos " + numero_fila;
    }
    catch (err) {
        alert(err.message + " Funcion decrementa_documento_relacion_estado");
    }
}
function incrementa_documento_relacion_estado() {
    try {
        var element_table = document.getElementById("GridView_list_documento_relacion");
        var numero_fila = element_table.rows.length - 1;
        document.getElementById("Hidden_numero_doc_rel").value = numero_fila;
        document.getElementById("Label_documentos").innerHTML = "Documentos " + numero_fila;
    }
    catch (err) {
        alert(err.message + " Funcion incrementa_documento_relacion_estado");
    }
}
function actualiza_gre_campo_lista(nombre_grid, id, valor_campo, nombre_campo) {
    try {
        $("#" + nombre_grid + " tr[id_rad=" + id + "]").each(function () {
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
        alert(err.message + " Funcion actualiza_gre_campo_lista");
    }
}
function actualiza_gre_campo(nombre_grid, id, valor_campo, nombre_campo) {
    try {
        $("#" + nombre_grid + " tr[id_rad=" + id + "]").each(function () {
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
function Set_documento_seleccionado(id_imagen_, nombre_gabinete_) {
    try {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "../webservice/WebServiceDocuarchi.asmx/Set_documento_seleccionado",
            data: "{'id_imagen':'" + id_imagen_ + "'," + "'nombre_gabinete':'" + nombre_gabinete_ + "'}",
            dataType: "json",
            success: function (data) {
                //response(data.d);
                if (data.d !== "YES") {
                    alert(data.d);
                    element.checked = false;
                }
            },
            error: function (result) {
                alert("Error......" + result);
                event.preventDefault();
            }
        });
    }
    catch (err) {
        alert(err.message + " Funcion Set_documento_seleccionado");
    }
}
function busqueda_gred(HiddenSeleccion, data_grid, contenido_busqueda, CheckboxBusqueda) {
    try {
        if ($("#" + contenido_busqueda).val() == "") {
            $("#" + data_grid + " tr[id]").css({ "background-color": "transparent", "color": "Black" });
            $("#" + HiddenSeleccion).val("-1");
            return false;
        }
        $("#" + HiddenSeleccion).val("-1");
        var refgrid;
        var filtro;
        $("#" + data_grid + " tr[id]").css({ "background-color": "transparent", "color": "Black" });
        var s = $("#" + contenido_busqueda).val().toLowerCase();
        var grid = $("#" + data_grid);
        var cel_indes = 0;
        $("#" + data_grid + " tr:has(td)").each(function () {
            cel_indes = cel_indes + 1;
            var scrollableDiv = grid.parent();
            var rowtd = $(this);
            $(this).children("td").each(function (idex) {
                var tempotd = $(this).text().toLowerCase()
                var check = document.getElementById(CheckboxBusqueda).checked;
                if (check == true) {

                    if (idex >= 0) {
                        if (s == tempotd) {
                            $(this).parent().css({ "background-color": "LightSkyBlue", "color": "green" });
                            //$(scrollableDiv).scrollTop(70);
                            var id_ref = $(this).parent();
                            if (cel_indes == 2) {
                                $(scrollableDiv).scrollTop(($(id_ref).offset().top - id_ref[0].offsetHeight));
                            }
                            if (cel_indes !== 2) {
                                $(scrollableDiv).scrollTop(rowtd[0].offsetTop - id_ref[0].offsetHeight);
                            }

                        }
                    }
                }

                if (check == false) {
                    if (idex >= 0) {
                        var compare = tempotd;
                        var strcompre = compare.indexOf(s);
                        if (strcompre >= 0) {
                            $(this).parent().css({ "background-color": "LightSkyBlue", "color": "green" });
                            $(scrollableDiv).scrollTop(0);
                            var id_ref = $(this).parent();

                            if (cel_indes == 2) {
                                $(scrollableDiv).scrollTop(($(id_ref).offset().top - id_ref[0].offsetHeight));
                            }
                            if (cel_indes !== 2) {
                                $(scrollableDiv).scrollTop(rowtd[0].offsetTop - id_ref[0].offsetHeight);
                            }

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
function retorna_colum_mtriz(hiden_name) {
    try {
    var hiden = document.getElementById(hiden_name);
    var x = $('#GridView_val_radicacion th');
    var txt = "";
    var i;
    for (i = 1; i < x.length; i++) {
        txt = txt + x[i].innerText.toUpperCase() + "|";
    }
    hiden.value = txt;
    return txt;
    }
    catch (err) {
        alert(err.message + " Funcion retorna_colum_mtriz");
    }
}
//actualiza campos dinamicos consulta radicados entrates y salientes
function actualiza_gre_campos_dinamicos_() {
    try {
        var hidendcampos = document.getElementById("Hidden_campos_dinamicos_edita").value;
        var hidencamposaleas = document.getElementById("hidden_campos_dinamicos_aleas").value;
        //hidden_campos_dinamicos_aleas
        var spli_campos = hidendcampos.split("|");
        var spli_campos_aleas = hidencamposaleas.split("|");
        $("#GridView_val_radicacion tr[id=" + $("#hdnEmailID_VAL").val() + "]").each(function () {
            var idex = -1;
            //cargo_destinatario
            for (i = 0; i <= (spli_campos.length - 1) ; i++) {
                var control = document.getElementById(spli_campos[i]);
                var name = spli_campos_aleas[i].split("-");
                name[1] = name[1].replace(" ", "_");
                if (control != undefined) {
                    idex = colum_index(name[1]);
                    if (idex != -1) {

                        $(this)[0].cells[idex].innerText = control.value;

                    }
                }
            }
        })
    }
     catch (err) {
         alert(err.message + " Funcion actualiza_gre_campos_dinamicos");
    }
}
function actualiza_gre_campos_dinamicos() {
    try {
        var hidendcampos = document.getElementById("Hidden_campos_dinamicos_edita").value;
        var hidencamposaleas = document.getElementById("hidden_campos_dinamicos_aleas").value;
        var hidenvalores = document.getElementById("hidden_valore_campos").value;
        var spli_campos = hidencamposaleas.split("|");
        var valores = hidenvalores.split("|||||");
        $("#GridView_val_radicacion tr[id=" + $("#hdnEmailID_VAL").val() + "]").each(function () {
            var idex = -1;
            for (i = 0; i <= (spli_campos.length - 1) ; i++) {
                var control = document.getElementById(spli_campos[i]);
                var name = spli_campos[i];
                if (valores[i] != undefined) {
                    idex = colum_index(name);
                    if (idex != -1) {
                        if (valores[i] == "") {
                            $(this)[0].cells[idex].innerText = "\u00a0";                         
                        }
                        if (valores[i] !== "") {
                            $(this)[0].cells[idex].innerText = valores[i];
                        }
                    }
                }
            }
        })
        return true;
    }
    catch (err) {
        alert(err.message + " Funcion actualiza_gre_campos_dinamicos");
    }
}
function confirma_respuesta(mensaje) {
    try {
    var res = confirm(mensaje);
    if (res == true) {
        document.getElementById("Hidden_alert_respuesta").value = "YES";
    }else {
        document.getElementById("Hidden_alert_respuesta").value = "NO";
    }
    }
    catch (err) {
        alert(err.message + " Funcion confirma_respuesta");
    }
}

//Actualiza el campo fecha limite de respuesta del documento
function actualiza_gred_limite_respuesta() {
    try {
    $("#GridView_val_radicacion tr[id=" + $("#hdnEmailID_VAL").val() + "]").each(function () {
        var idex = -1;
        //cargo_destinatario
        var drop = document.getElementById("DropDownList_edita_tipo_tramite");
        var text = document.getElementById("TextBox_fecha_tramite_vence");
        
        idex = colum_index("FECHALIMITERESPUESTA");
        if (idex != -1) {
            if (drop.value != "SELECCIONE" && drop.value != "") {
                $(this)[0].cells[idex].innerText = text.value;
            }

        }
        idex = colum_index("DESCRIPCION_TRAMITE");
        if (idex != -1) {
            if (drop.value != "SELECCIONE" && drop.value != "") {
                $(this)[0].cells[idex].innerText = drop.value;
            }
        }
        //plugin_grwedview();
        mueve_scroll_data_gred('GridView_val_radicacion', 'hdnEmailID_VAL');
    })
    }
    catch (err) {
        alert(err.message + " Funcion actualiza_gred_limite_respuesta");
    }
}
//Funcion activa fecha limite de respuesta
function selecciona_fecha_limite_resp_tramite() {
    try {
       var drop = document.getElementById("DropDownList_edita_tipo_tramite");
       if (drop.value != "SELECCIONE" && drop.value != "") {
        var boton = document.getElementById("Button_actualiza_fecha_limte_respuesta");
        boton.click();
       }
    }
    catch (err) {
        alert(err.message + " Funcion selecciona_fecha_limite_resp_tramite");
    }
}
function selecciona_lista_activididades_flujos() {
    try {
        var drop = document.getElementById("DropDownList_flujo_tramite");
       if (drop.value != "SELECCIONE" && drop.value != "") {
        var boton = document.getElementById("Button_actualiza_lista_actividades_flujo");
        boton.click();
       }
    } catch (err) {
        alert(err.message + " Funcion selecciona_lista_activididades_flujos");
    }
}
function selecciona_lista_usuario_flujos() {
    try {
        var drop = document.getElementById("DropDownList_lista_actividades_flujo");
        if (drop.value != "SELECCIONE" && drop.value != "") {
            var boton = document.getElementById("Button_actualiza_lista_usuarios_actividades");
            boton.click();
        }
    } catch (err) {
        alert(err.message + " Funcion selecciona_lista_flujos");
    }
}
function actualiza_grid_campos_fijos() {
    try {
    var Hidden_tipo_plantilla = document.getElementById("Hidden_tipo_plantilla");
    var nombre_plantilla = document.getElementById("Hidden_nombre_plantilla_radicado").value;
    var Destinatario_Externo_id_Dest_Ext = document.getElementById("Hidden_remitente_destinario_interno").value;
    var Id_area_remit_dest_interno = document.getElementById("Hidden_area_remitente_destinatario").value;
    var Remit_Dest_Interno_id_Remit_Dest_Int = document.getElementById("Hidden_remitente_destinatario").value;
    var hdnEmailID_VAL_ref = document.getElementById("hdnEmailID_VAL");
    var DropDownList_area_destinatario_entrate = document.getElementById("DropDownList_area_destinatario_entrate");
    var DropDownList_destinatario_entrante = document.getElementById("DropDownList_destinatario_entrante");
    var TextBox_remitente_entrante = document.getElementById("TextBox_remitente_entrante");
    var TextBoxIdentificacion_remitente = document.getElementById("TextBoxIdentificacion_remitente");
    var TextBox_asunto_entrante = document.getElementById("TextBox_asunto_entrante");
    var TextBox_cita_radicado_entrante = document.getElementById("TextBox_cita_radicado_entrante");
    var TextBox_Numero_Folios_entrante = document.getElementById("TextBox_Numero_Folios_entrante");
    var TextBox_anexos_entrante = document.getElementById("TextBox_anexos_entrante");
    var TextBox_fecha_documento_entrante = document.getElementById("TextBox_fecha_documento_entrante");
    var cargo_destinatario = "";
    var spliter = DropDownList_destinatario_entrante.value.split("((");
    var spliteruno = spliter[1].split("))");
    cargo_destinatario = spliteruno[0];
    $("#GridView_val_radicacion tr[id=" + $("#hdnEmailID_VAL").val() + "]").each(function () {
        var idex = -1;
        //cargo_destinatario
        idex = colum_index("cargo_destinatario");
        if (idex != -1) {
            $(this)[0].cells[idex].innerText = cargo_destinatario;
        }
        idex = colum_index("Area_destinatario");
        if (idex != -1) {
            $(this)[0].cells[idex].innerText = DropDownList_area_destinatario_entrate.value;
        }
        idex = colum_index("Destinatario");
        if (idex != -1) {
            $(this)[0].cells[idex].innerText = spliter[0];
        }
        idex = colum_index("Remitente");
        if (idex != -1) {
            $(this)[0].cells[idex].innerText = TextBox_remitente_entrante.value;
        }
        idex = colum_index("IDENTIFICACION_REMITENTE");
        if (idex != -1) {
            $(this)[0].cells[idex].innerText = TextBoxIdentificacion_remitente.value;
        }
        idex = colum_index("CITARADICADO");
        if (idex != -1) {
            $(this)[0].cells[idex].innerText = TextBox_cita_radicado_entrante.value;
        }
        idex = colum_index("Numero_Folios");
        if (idex != -1) {
            $(this)[0].cells[idex].innerText = TextBox_Numero_Folios_entrante.value;
        }
        idex = colum_index("Anexos_Cor");
        if (idex != -1) {
            $(this)[0].cells[idex].innerText = TextBox_anexos_entrante.value;
        }
        var idex = colum_index("Asunto");
        if (idex != -1) {
            $(this)[0].cells[idex].innerText = TextBox_asunto_entrante.value;
        }
        idex = colum_index("Fecha_Documento");
        if (idex != -1) {
            $(this)[0].cells[idex].innerText = TextBox_fecha_documento_entrante.value;
        }
    })
    //plugin_grwedview();
    mueve_scroll_data_gred('GridView_val_radicacion', 'hdnEmailID_VAL');
    var but = document.getElementById("Button_cerrar_editar_radicacion_entrante");
    but.click();
    return true;
    }
    catch (err) {
        alert(err.message + " Funcion actualiza_grid_campos_fijos");
    }
}
function actualiza_plantillla_radicacion_web_service() {
    try {
    var Hidden_tipo_plantilla = document.getElementById("Hidden_tipo_plantilla");
    var nombre_plantilla = document.getElementById("Hidden_nombre_plantilla_radicado").value;
    var Destinatario_Externo_id_Dest_Ext = document.getElementById("Hidden_remitente_destinario_interno").value;
    var Id_area_remit_dest_interno = document.getElementById("Hidden_area_remitente_destinatario").value;
    var Remit_Dest_Interno_id_Remit_Dest_Int = document.getElementById("Hidden_remitente_destinatario").value;
    var hdnEmailID_VAL_ref = document.getElementById("hdnEmailID_VAL");
    if (hdnEmailID_VAL_ref.value == "-1") {
        alert("Debe seleccionar un registro para editar");
        return false;
    }
    var confir = confirm("Desea actualizar el registro?");
    if (confir == true) {

    } else {
        return true;
    }
    var update_string = "";
    if (Hidden_tipo_plantilla.value == "RADICACION ENTRANTE") {
        var DropDownList_area_destinatario_entrate = document.getElementById("DropDownList_area_destinatario_entrate");
        var DropDownList_destinatario_entrante = document.getElementById("DropDownList_destinatario_entrante");
        var TextBox_remitente_entrante = document.getElementById("TextBox_remitente_entrante");
        var TextBoxIdentificacion_remitente = document.getElementById("TextBoxIdentificacion_remitente");
        var TextBox_asunto_entrante = document.getElementById("TextBox_asunto_entrante");
        var TextBox_cita_radicado_entrante = document.getElementById("TextBox_cita_radicado_entrante");
        var TextBox_Numero_Folios_entrante = document.getElementById("TextBox_Numero_Folios_entrante");
        var TextBox_anexos_entrante = document.getElementById("TextBox_anexos_entrante");
        var TextBox_fecha_documento_entrante = document.getElementById("TextBox_fecha_documento_entrante");
        var cargo_destinatario = "";
        if (DropDownList_area_destinatario_entrate.selectedIndex == -1) {
            alert("Por favor seleccione el área del  destintario interno");
            return false;
        }
        if (DropDownList_area_destinatario_entrate.value == "TODAS LAS AREAS") {
            alert("Por favor seleccione el área del  destintario interno, o asignela");
            return false;
        }
        if (DropDownList_area_destinatario_entrate.value == "SELECCIONE") {
            alert("Por favor seleccione el área del  destintario interno, o asignela");
            return false;
        }
        if (DropDownList_destinatario_entrante.selectedIndex == -1) {
            alert("Por favor seleccione el  destintario interno");
            return false;
        }
        if (DropDownList_destinatario_entrante.value == "SELECCIONE") {
            alert("Por favor seleccione el  destintario interno");
            return false;
        }
        if (TextBox_remitente_entrante == "") {
            alert("Por favor seleccione el  remitente");
            return false;
        }
        if (Destinatario_Externo_id_Dest_Ext == "-1") {
            alert("El destinatario interno esta en en estado (-1) no se puede actualizar");
            return false;
        }
        if (Id_area_remit_dest_interno == "-1") {
            alert("El area del destinatario interno esta en en estado (-1) no se puede actualizar");
            return false;
        }
        var spliter = DropDownList_destinatario_entrante.value.split("((");
        update_string = "update " + nombre_plantilla + " set Destinatario_Externo_id_Dest_Ext=" + "|" + Destinatario_Externo_id_Dest_Ext + "|";
        update_string = update_string + ",Id_area_remit_dest_interno=" + "|" + Id_area_remit_dest_interno + "|";
        update_string = update_string + ",Remit_Dest_Interno_id_Remit_Dest_Int=" + "|" + Remit_Dest_Interno_id_Remit_Dest_Int + "|";
        update_string = update_string + ",Area_remit_dest_interno=" + "|" + DropDownList_area_destinatario_entrate.value.replace("'","|") + "|";
        update_string = update_string + ",Destinatario_Cor=" + "|" + spliter[0].replace("'", "|") + "|";
        var spliteruno = spliter[1].split("))");
        cargo_destinatario = spliteruno[0];
        if (cargo_destinatario.value == "") {
            update_string = update_string + ",cargo_destinatario=" + "null";
        } else {
            update_string = update_string + ",cargo_destinatario=" + "|" + cargo_destinatario.replace("'", "|") + "|";
        }
        if (TextBox_remitente_entrante.value == "") {
            update_string = update_string + ",Remitente_Cor=" + "null";
        } else {
            update_string = update_string + ",Remitente_Cor=" + "|" + TextBox_remitente_entrante.value.replace("'", "|") + "|";
        }
       
        if (TextBoxIdentificacion_remitente.value == "") {
            update_string = update_string + ",IDENTIFICACION_REMITENTE=" + "null" ;
        } else {
            update_string = update_string + ",IDENTIFICACION_REMITENTE=" + "|" + TextBoxIdentificacion_remitente.value.replace("'", "|") + "|";
        }

        if (TextBox_asunto_entrante.value == "") {
            update_string = update_string + ",Asunto=" + "null";
        } else {
            update_string = update_string + ",Asunto=" + "|" + TextBox_asunto_entrante.value.replace("'", "|") + "|";
        }
        
        if (TextBox_cita_radicado_entrante.value == "") {
            update_string = update_string + ",CITARADICADO=" + "null";
        } else {
            update_string = update_string + ",CITARADICADO=" + "|" + TextBox_cita_radicado_entrante.value.replace("'", "|") + "|";
        }
        if (TextBox_Numero_Folios_entrante.value == "") {
            update_string = update_string + ",Numero_Folios=" + "null";
            alert("Numero de folios no pude se null");
            return false;
        } else {
            update_string = update_string + ",Numero_Folios=" + "|" + TextBox_Numero_Folios_entrante.value.replace("'", "|") + "|";
        }
        if (TextBox_anexos_entrante.value == "") {
            update_string = update_string + ",Anexos_Cor=" + "null" ;
        } else {
            update_string = update_string + ",Anexos_Cor=" + "|" + TextBox_anexos_entrante.value.replace("'", "|") + "|";
        }
        if (TextBox_fecha_documento_entrante.value == "") {
            update_string = update_string + ",Fecha_Documento=" + "null";
        } {
            update_string = update_string + ",Fecha_Documento=" + "|" + TextBox_fecha_documento_entrante.value.replace("'", "|") + "|";
        }
        
        update_string = update_string + " where Consecutivo_Rad=" + "|" + hdnEmailID_VAL_ref.value.replace("'", "|") + "|";
      
       //web_service_actualiza(update_string);
       if ($("#Hidden_resultado_web_service").val() != "YES") {
           //alert($("#Hidden_resultado_web_service").val());
            return false
        }
       $("#GridView_val_radicacion tr[id=" + $("#hdnEmailID_VAL").val() + "]").each(function () {
           var idex = -1;
           //cargo_destinatario
           idex = colum_index("cargo_destinatario");
           if (idex != -1) {
               $(this)[0].cells[idex].innerText = cargo_destinatario;
           }
           idex = colum_index("Area_destinatario");
           if (idex != -1) {
               $(this)[0].cells[idex].innerText = DropDownList_area_destinatario_entrate.value;
           }
           idex = colum_index("Destinatario");
           if (idex != -1) {
               $(this)[0].cells[idex].innerText = spliter[0];
           }
           idex = colum_index("Remitente");
           if (idex != -1) {
               $(this)[0].cells[idex].innerText = TextBox_remitente_entrante.value;
           }
           idex = colum_index("IDENTIFICACION_REMITENTE");
           if (idex != -1) {
               $(this)[0].cells[idex].innerText = TextBoxIdentificacion_remitente.value;
           }
           idex = colum_index("CITARADICADO");
           if (idex != -1) {
               $(this)[0].cells[idex].innerText = TextBox_cita_radicado_entrante.value;
           }
           idex = colum_index("Numero_Folios");
           if (idex != -1) {
               $(this)[0].cells[idex].innerText = TextBox_Numero_Folios_entrante.value;
           }
           idex = colum_index("Anexos_Cor");
           if (idex != -1) {
               $(this)[0].cells[idex].innerText = TextBox_anexos_entrante.value;
           }
           var idex = colum_index("Asunto");
           if (idex != -1) {
               $(this)[0].cells[idex].innerText = TextBox_asunto_entrante.value;
           }
           idex = colum_index("Fecha_Documento");
           if (idex != -1) {
               $(this)[0].cells[idex].innerText = TextBox_fecha_documento_entrante.value;
           }
       })
       var but = document.getElementById("Button_cerrar_editar_radicacion_entrante");
       but.click();    
       return true;
    }

    if (Hidden_tipo_plantilla.value == "RADICACION SALIENTE") {
        var DropDownList_area_remitente_saliente = document.getElementById("DropDownList_area_remitente_saliente");
        var DropDownList_remitente_saliente = document.getElementById("DropDownList_remitente_saliente");
        var TextBox_remitente_saliente = document.getElementById("TextBox_remitente_saliente");
        var TextBox_identificacion_destinatario = document.getElementById("TextBox_identificacion_destinatario");
        var TextBox_asunto_saliente = document.getElementById("TextBox_asunto_saliente");
        var TextBox_cita_radicado_saliente = document.getElementById("TextBox_cita_radicado_saliente");
        var TextBox_Numero_Folios_saliente = document.getElementById("TextBox_Numero_Folios_saliente");
        var TextBox_anexos_saliente = document.getElementById("TextBox_anexos_saliente");
        var TextBox_fecha_documento_saliente = document.getElementById("TextBox_fecha_documento_saliente");
        var cargo_remitente = "";

        if (DropDownList_area_remitente_saliente.selectedIndex == -1) {
            alert("Por favor seleccione el área del remitente interno");
            return false;
        }
        if (DropDownList_area_remitente_saliente.value == "TODAS LAS AREAS") {
            alert("Por favor seleccione el área del remitente interno, o asignela");
            return false;
        }
        if (DropDownList_area_remitente_saliente.value == "SELECCIONE") {
            alert("Por favor seleccione el área del  remitente interno, o asignela");
            return false;
        }
        if (DropDownList_remitente_saliente.selectedIndex == -1) {
            alert("Por favor seleccione el  remitente interno");
            return false;
        }
        if (DropDownList_remitente_saliente.value == "SELECCIONE") {
            alert("Por favor seleccione el  remitente interno");
            return false;
        }
        if (TextBox_remitente_saliente == "") {
            alert("Por favor seleccione el  destinatario");
            return false;
        }
        if (Destinatario_Externo_id_Dest_Ext == "-1") {
            alert("El remitente interno esta en en estado (-1) no se puede actualizar");
            return false;
        }
        if (Id_area_remit_dest_interno == "-1") {
            alert("El area del remitente interno esta en en estado (-1) no se puede actualizar");
            return false;
        }

        var spliter = DropDownList_remitente_saliente.value.split("((");
        update_string = "update " + nombre_plantilla + " set Destinatario_Externo_id_Dest_Ext=" + "|" + Destinatario_Externo_id_Dest_Ext + "|";
        update_string = update_string + ",Id_area_remit_dest_interno=" + "|" + Id_area_remit_dest_interno + "|";
        update_string = update_string + ",Remit_Dest_Interno_id_Remit_Dest_Int=" + "|" + Remit_Dest_Interno_id_Remit_Dest_Int + "|";
        update_string = update_string + ",Area_remit_dest_interno=" + "|" + DropDownList_area_remitente_saliente.value.replace("'", "|") + "|";
        update_string = update_string + ",Remitente_Cor=" + "|" + spliter[0].replace("'", "|") + "|";
        var spliteruno = spliter[1].split("))");
        cargo_remitente = spliteruno[0];
        if (cargo_remitente.value == "") {
            update_string = update_string + ",cargo_remitente=" + "null";
        } else {
            update_string = update_string + ",cargo_remitente=" + "|" + cargo_remitente.replace("'", "|") + "|";
        }
        if (TextBox_remitente_saliente.value == "") {
            update_string = update_string + ",Destinatario_Cor=" + "null";
        } else {
            update_string = update_string + ",Destinatario_Cor=" + "|" + TextBox_remitente_saliente.value.replace("'", "|") + "|";
        }

        if (TextBox_identificacion_destinatario.value == "") {
            update_string = update_string + ",IDENTIFICACION_DESTINATARIO=" + "null";
        } else {
            update_string = update_string + ",IDENTIFICACION_DESTINATARIO=" + "|" + TextBox_identificacion_destinatario.value.replace("'", "|") + "|";
        }

        if (TextBox_asunto_saliente.value == "") {
            update_string = update_string + ",Asunto=" + "null";
        } else {
            update_string = update_string + ",Asunto=" + "|" + TextBox_asunto_saliente.value.replace("'", "|") + "|";
        }

        if (TextBox_cita_radicado_saliente.value == "") {
            update_string = update_string + ",CITARADICADO=" + "null";
        } else {
            update_string = update_string + ",CITARADICADO=" + "|" + TextBox_cita_radicado_saliente.value.replace("'", "|") + "|";
        }
        if (TextBox_Numero_Folios_saliente.value == "") {
            update_string = update_string + ",Numero_Folios=" + "null";
            alert("Numero de folios no pude se null");
            return false;
        } else {
            update_string = update_string + ",Numero_Folios=" + "|" + TextBox_Numero_Folios_saliente.value.replace("'", "|") + "|";
        }
        if (TextBox_anexos_saliente.value == "") {
            update_string = update_string + ",Anexos_Cor=" + "null";
        } else {
            update_string = update_string + ",Anexos_Cor=" + "|" + TextBox_anexos_saliente.value.replace("'", "|") + "|";
        }
        if (TextBox_fecha_documento_saliente.value == "") {
            update_string = update_string + ",Fecha_Documento=" + "null";
        } {
            update_string = update_string + ",Fecha_Documento=" + "|" + TextBox_fecha_documento_saliente.value.replace("'", "|") + "|";
        }

        update_string = update_string + " where Consecutivo_Rad=" + "|" + hdnEmailID_VAL_ref.value.replace("'", "|") + "|";
    }

    web_service_actualiza(update_string);
    if ($("#Hidden_resultado_web_service").val() != "YES") {
        //alert($("#Hidden_resultado_web_service").val());
        return false
    }
    $("#GridView_val_radicacion tr[id=" + $("#hdnEmailID_VAL").val() + "]").each(function () {
        var idex = -1;
        idex = colum_index("cargo_remitente");
        if (idex != -1) {
            $(this)[0].cells[idex].innerText = cargo_remitente;
        }
        idex = colum_index("AREA_REMITENTE");
        if (idex != -1) {
            $(this)[0].cells[idex].innerText = DropDownList_area_remitente_saliente.value;
        }
        idex = colum_index("Remitente");
        if (idex != -1) {
            $(this)[0].cells[idex].innerText = spliter[0];
        }
        idex = colum_index("Destinatario");
        if (idex != -1) {
            $(this)[0].cells[idex].innerText = TextBox_remitente_saliente.value;
        }
        idex = colum_index("IDENTIFICACION_DESTINATARIO");
        if (idex != -1) {
            $(this)[0].cells[idex].innerText = TextBox_identificacion_destinatario.value;
        }
        idex = colum_index("CITARADICADO");
        if (idex != -1) {
            $(this)[0].cells[idex].innerText = TextBox_cita_radicado_saliente.value;
        }
        idex = colum_index("Numero_Folios");
        if (idex != -1) {
            $(this)[0].cells[idex].innerText = TextBox_Numero_Folios_saliente.value;
        }
        idex = colum_index("Anexos_Cor");
        if (idex != -1) {
            $(this)[0].cells[idex].innerText = TextBox_anexos_saliente.value;
        }
        var idex = colum_index("Asunto");
        if (idex != -1) {
            $(this)[0].cells[idex].innerText = TextBox_asunto_saliente.value;
        }
        idex = colum_index("Fecha_Documento");
        if (idex != -1) {
            $(this)[0].cells[idex].innerText = TextBox_fecha_documento_saliente.value;
        }
        var but = document.getElementById("Button_cerrar_editar_radicacion_saliente");
        but.click();
        return true;
    })
    }
    catch (err) {
        alert(err.message + " Funcion actualiza_plantillla_radicacion_web_service");
    }
}

function web_service_actualiza(data) {
    try {
    $.ajax({
        type: 'POST',
        url: '../webservice/WebServiceRadicacion.asmx/update_radic_plantilla_radicado',
        data: "{'update':'" + data + "'}",
        contentType: 'application/json; utf-8',
        dataType: 'json',
        success: function (data) {
            if (data.d != null) {
                $("#Hidden_resultado_web_service").val(data.d);
                //alert($("#Hidden_resultado_web_service").val());
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            $("#Hidden_resultado_web_service").val(jqXHR.responseText);
            //alert($("#Hidden_resultado_web_service").val());
            alert(jqXHR.responseText);
        }
     
    });       
    }
    catch (err) {
        alert(err.message + " Funcion web_service_actualiza");
    }
}
function auto_zise_popup_detalle_transacciones() {
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
        var heig_porcent = espacio_iframe - 10;  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_transacciones').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_Panel_transacciones').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#Cotenedorpendiente_transacciones').css("height", (document.getElementById("modal_content_Panel_transacciones").clientHeight - document.getElementById("Cabecerapendiente_transacciones").clientHeight) + "px");
        //Para los modal que contiene gred
        $('#Iframe_log_transacciones_').css("height", (document.getElementById("Cotenedorpendiente_transacciones").clientHeight - 5) + "px");
        
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_detalle_transacciones");
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
        //corrige el posicionamiento horizontal del modalpopuextender, la propiedad width del panel debe estar en auto
        //var widtth_procent_left_rigth = (with_frame - document.getElementById("Panel_agregar_expediente_carpeta").clientHeight) / 2;
        //$('#Panel_agregar_expediente_carpeta').css("left", (Math.round(widtth_procent_left_rigth)) + "px");
        var heig_porcent = espacio_iframe - ((espacio_iframe * 20) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_trazabilidad').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_trazabilidad').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#Cotenedorpendiente_trazabilidad').css("height", (document.getElementById("modal_content_trazabilidad").clientHeight - document.getElementById("Cabecerapendiente_trazabilidad").clientHeight ) + "px");
        $('#Iframe_trazabilidad_').css("height", (document.getElementById("Cotenedorpendiente_trazabilidad").clientHeight - 5) + "px");

    } catch (ex) {
        alert("Función auto_zise_popup_detalle_trazabilidad " + ex.message)
    }
}
function auto_zise_popup_plantilla_validacion() {
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
        //var heig_porcent = espacio_iframe - ((espacio_iframe * 20) / 100);  // Indica el porcentaje de espacio vertical del elemento
    var heig_porcent = espacio_iframe;
    $('#Panel_valiacion_plantilla').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
    $('#modal_content_validacion_plantilla').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
    $('#contenido_procesa_usu_rel_solicitud').css("height", (document.getElementById("modal_content_validacion_plantilla").clientHeight - (document.getElementById("divcabecer2_validacion_plantilla").clientHeight + document.getElementById("modal-footer_validacion_plantilla").clientHeight)) + "px");
    $('#Iframe_validacion_plantilla_').css("height", (document.getElementById("modal_content_validacion_plantilla").clientHeight - (document.getElementById("divcabecer2_validacion_plantilla").clientHeight + document.getElementById("modal-footer_validacion_plantilla").clientHeight + 5)) + "px");
     
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_plantilla_validacion");
    }
}
function auto_zise_popup_campo_dinamico() {
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

      
       
        $('#panel_edita_campos_dinamicos').css("width", (with_frame / 2) + "px");
        $('#panel_edita_campos_dinamicos').css("height", (espacio_iframe - 10) + "px");
        $('#botones_edita_campos_dinamicos').css("height", ($('#Button_edita_campos_dinamicos').height() + 10) + "px");
        $('#campos_edita_campos_dinamicos').css("height", ((espacio_iframe - 45) - $('#botones_edita_campos_dinamicos').height()) + "px");
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_campo_dinamico");
    }
}
function actuo_zise_popup_compartir_correo_electronico() {
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
        var heig_porcent = espacio_iframe - 20;  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_notifica_gestion').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_notifica_gestion').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#contenido_procesa_notifica_gestion').css("height", (document.getElementById("modal_content_notifica_gestion").clientHeight - document.getElementById("divcabecer2_notifica_gestion").clientHeight) + "px");
        //Para los modal que contiene gred
        $('#Iframe_comparte_coreo').css("height", (document.getElementById("contenido_procesa_notifica_gestion").clientHeight - 5) + "px");

       
    }
    catch (ex) {
        alert("Incosistencia general función actuo_zise_popup_compartir_correo_electronico " + ex)
    }
}
function auto_zise_popup_editar_radicados() {
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
    var heig_porcent = espacio_iframe - 5;
    //var heig_porcent = espacio_iframe - ((espacio_iframe * 20) / 100);  // Indica el porcentaje de espacio vertical del elemento
    $('#panel_editar_radicacion_entrante').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
    $('#modal_content_radicacion_entrante').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
    $('#content_ditar_radicacion_entrante').css("height", (document.getElementById("modal_content_radicacion_entrante").clientHeight - (document.getElementById("divcabecer2_editar_radicacion_entrante").clientHeight + document.getElementById("modal-footer_boton_edit").clientHeight)) + "px");
    
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_editar_radicados");
    }
}
function auto_zise_popup_editar_radicados_salientes() {
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
        var heig_porcent = espacio_iframe - 5;
        //var heig_porcent = espacio_iframe - ((espacio_iframe * 20) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#panel_editar_radicacion_saliente').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_radicacion_radicacion_saliente').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#content_ditar_radicacion_saliente').css("height", (document.getElementById("modal_content_radicacion_radicacion_saliente").clientHeight - (document.getElementById("divcabecer2_editar_radicacion_saliente").clientHeight + document.getElementById("modal-footer_boton_edit_").clientHeight)) + "px");

    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_editar_radicados");
    }
}
function auto_zise_popup_cambio_tipo_tramite() {
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
        var heig_porcent = espacio_iframe - ((espacio_iframe * 5) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_edita_tipo_tramite').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_tipo_tramite').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#contenido_edita_tipo_tramite').css("height", (document.getElementById("modal_content_tipo_tramite").clientHeight - (document.getElementById("divcabecer2_edita_tipo_tramite").clientHeight + document.getElementById("contenido_botones_edita_tipo_tramite").clientHeight)) + "px");
       
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_cambio_tipo_tramite " + err.message);
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
function auto_size_imprimir() {
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
        var heig_porcent = espacio_iframe - 100;  // Indica el porcentaje de espacio vertical del elemento
        $('#Panelimpresion').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_Panelimpresion').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#ContenidoImpresion').css("height", (document.getElementById("modal_content_Panelimpresion").clientHeight - document.getElementById("divcabecer2").clientHeight) + "px");
        $('#ifimpre').css("height", (document.getElementById("ContenidoImpresion").clientHeight - 5) + "px");
    } catch (ex) { alert("Funcion auto_size_imprimir " + ex.mensaje); }
}

function llenardestinatrio_entrante() {
    try {
    var drowplist = document.getElementById("DropDownList_area_destinatario_entrate");
    if (drowplist.selectedIndex != -1) {
        var boton = document.getElementById("Button_listar_destinatarios_entrantes");
        boton.click();
        auto_zise_popup_editar_radicados();
    }
    }
         catch (err) {
             alert(err.message + " Funcion llenardestinatrio_entrante");
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

        //corrige el posicionamiento horizontal del modalpopuextender, la propiedad width del panel debe estar en auto
        //var widtth_procent_left_rigth = (with_frame - document.getElementById("Panel_agregar_expediente_carpeta").clientHeight) / 2;
        //$('#Panel_agregar_expediente_carpeta').css("left", (Math.round(widtth_procent_left_rigth)) + "px");
        var heig_porcent = espacio_iframe;  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_visor_externo').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_visor_externo').css("height", (heig_porcent) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#Cotenedorpendiente_visor_externo').css("height", (document.getElementById("modal_content_visor_externo").clientHeight - (document.getElementById("Cabecerapendiente_visor_externo").clientHeight)) + "px");
        $('#Iframe_visor_externo_wf_').css("height", (document.getElementById("modal_content_visor_externo").clientHeight - (document.getElementById("Cabecerapendiente_visor_externo").clientHeight)) + "px");

    }
    catch (err) {
        alert(err.message + " Función auto_zise_popup_visor_externo");
    }
}
function llenar_remitente() {
    try {
    var drowplist = document.getElementById("DropDownList_area_remitente_saliente");
    if (drowplist.selectedIndex != -1) {
        var boton = document.getElementById("Button_listar_destinatarios_entrantes");
        boton.click();
        auto_zise_popup_editar_radicados();
    }
    }
    catch (err) {
        alert(err.message + " Funcion llenar_remitente");
    }
}
function listar_id_destintario_remitente() {
    try {
    var boton = document.getElementById("Button_listar_id_destinatario");
    boton.click();
    }
    catch (err) {
        alert(err.message + " Funcion listar_id_destintario_remitente");
    }
}
function onDataShown(sender, args) {
    sender._popupBehavior._element.style.zIndex = 1000001;

}
//MUEVE EL SCCROL AL ID SELECCIONADO
function mueve_scroll_data_gred(data_grid, HiddenSeleccion) {
    if ($("#" + HiddenSeleccion).val() != "-1") {
        var scrollableDiv = $("#" + data_grid).parent();
        var scrolllabelplugin = $("#" + data_grid + "VerticalBar");
        var scrolllabelplugin2 = $("#" + data_grid + "PanelItemContent");
        var scrolllabelplugin3 = $("#" + data_grid + "VerticalRail");
        var scrolllabelplugin4 = $("#" + data_grid + "VerticalBar");
        var scrolllabelplugin5 = $("#" + data_grid + "PanelItem");
        var scrolllabelplugin6 = $("#" + data_grid + "Wrapper");
        //limpia todos los seleccionados PanelItem Wrapper
        $("#" + data_grid + " tr[id]").css({ "background-color": "transparent", "color": "Black" });
        $("#" + data_grid + " tr[id=" + $("#" + HiddenSeleccion).val() + "]").css({ "background-color": "#E7EDF5", "color": "Black" });
        $("#" + data_grid + " tr[id= " + $("#" + HiddenSeleccion).val() + "]").each(function () {
            //$(scrollableDiv).scrollTop(70);
            //$(scrollableDiv).scrollTop(($(this).offset().top));
            $(scrollableDiv).scrollTop(document.getElementById("Hidden_offset").value);
            //$(scrolllabelplugin).scrollTop(70);
            //$(scrolllabelplugin).scrollTop(($(this).offset().top));
            //$(scrolllabelplugin).scrollTop(document.getElementById("Hidden_offset").value);
            //document.getElementById(data_grid + "VerticalBar").style.top = $(this).offset().top + "px";
            document.getElementById(data_grid + "VerticalBar").style.top = document.getElementById("Hidden_offset").value + "px";
            return true;         
        });
    }
}
//FUNCION ACTIVA Y DEACTIVA LOS CAMPOS CHEKEADOS EN UNA TABLA
function desactiva_ch_data_grid(idente_chekbi_actyive) {
    var e = $("#" + idente_chekbi_actyive);

    if ($(e).is(':checked')) {
        var x = document.getElementsByClassName("jjjjjjjjjjj");
        for (i = 0; i < x.length; i++) {
            var z = x[i].firstChild;
            z.checked = false;

        }

    }
    else {

        var x = document.getElementsByClassName("jjjjjjjjjjj");
        for (i = 0; i < x.length; i++) {
            var z = x[i].firstChild;
            z.checked = true;

        }


    }
}
//AUTO SIZE POPUP VALIDACION RADICADOS

function activa_boton_dowload() {
    try {

        document.getElementById("Button_guardar_desicion").click();
    }
    catch (err) {
        alert(err.message + " funcion activa_boton_dowload " + err.message);
    }
}
function AjaxFileUpload_change_text() {
    Sys.Extended.UI.Resources.AjaxFileUpload_SelectFile = "Adjuntar";
    Sys.Extended.UI.Resources.AjaxFileUpload_DropFiles = "Soltar y arrastrar archivos aquí";
    Sys.Extended.UI.Resources.AjaxFileUpload_Pending = "Pendiente";
    Sys.Extended.UI.Resources.AjaxFileUpload_Remove = "Eliminar";
    Sys.Extended.UI.Resources.AjaxFileUpload_Upload = "Guardar";
    Sys.Extended.UI.Resources.AjaxFileUpload_Uploaded = "Cargando";
    Sys.Extended.UI.Resources.AjaxFileUpload_UploadedPercentage = "Cargando {0} %";
    Sys.Extended.UI.Resources.AjaxFileUpload_Uploading = "Cargando";
    Sys.Extended.UI.Resources.AjaxFileUpload_FileInQueue = "{0} archivos(s) de .";
    Sys.Extended.UI.Resources.AjaxFileUpload_AllFilesUploaded = "All Files Uploaded.";
    Sys.Extended.UI.Resources.AjaxFileUpload_FileList = "Archivos a cargar:";
    Sys.Extended.UI.Resources.AjaxFileUpload_SelectFileToUpload = "archivos(s) para cargar.";
    Sys.Extended.UI.Resources.AjaxFileUpload_Cancelling = "Cancelando...";
    Sys.Extended.UI.Resources.AjaxFileUpload_UploadError = "Ocurrio un error cargando el archivo.";
    Sys.Extended.UI.Resources.AjaxFileUpload_CancellingUpload = "Cancelando carga...";
    Sys.Extended.UI.Resources.AjaxFileUpload_UploadingInputFile = "Cargando archivos: {0}.";
    Sys.Extended.UI.Resources.AjaxFileUpload_Cancel = "Cancelar";
    Sys.Extended.UI.Resources.AjaxFileUpload_Canceled = "cancelando";
    Sys.Extended.UI.Resources.AjaxFileUpload_UploadCanceled = "Carga de archivo cancelada";
    Sys.Extended.UI.Resources.AjaxFileUpload_DefaultError = "Error cargando archivo";
    Sys.Extended.UI.Resources.AjaxFileUpload_UploadingHtml5File = "Cargando archivo: {0} of size {1} bytes.";
    Sys.Extended.UI.Resources.AjaxFileUpload_error = "error";
}
//Retorna el idex de una columna en una tabla
function colum_index(colum_name) {

    var x = $('#GridView_val_radicacion th');
    var txt = "";
    var i;
    for (i = 0; i < x.length; i++) {
        if (x[i].innerText.toUpperCase() == colum_name.toUpperCase()) {

            return i;
        }

    }
    return -1;
}

$(document).on('keydown', function (e) {
    if (e.which == 9) {
        var id_element = e.srcElement.id;
        var matr_id = id_element.split("-");
        if (matr_id.length < 2) {
            return;
        }
        var salidadato;
        if (matr_id[2] == "DATE" || matr_id[2] == "DATE_2") {
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
                salidadato = Año_F + "/" + Mes_f + "/" + Dia_f;
                e.srcElement.value = salidadato;
            }

            if (numerocaracter == 10) {
                salidadato = Año_F + "/" + Mes_f + "/" + Dia_f;
                e.srcElement.value = salidadato;
            }

        }
    }
});
