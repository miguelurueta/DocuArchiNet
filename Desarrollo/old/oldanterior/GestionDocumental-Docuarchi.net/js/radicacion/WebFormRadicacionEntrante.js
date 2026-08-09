$(document).ready(function () {

    //FUNCION QUE MANEJA LOS EVENTOS CLIK DE LOS GREDVIEW
    $.fn.cligred = function () {      
        $('#data_grid tr[id]').click(function () {
            try {
            
            $('#data_grid tr[id]').css({ "background": "White", "color": "Black" });
            $(this).css({ "background-color": "#e8e8f7", "color": "Black" });
            var fer = $(this).attr("id");
            $('#hdnEmailID').val(fer);
            
            }
            catch (err) {
                alert(err.message + " Funcion clik");
            }
        });
       
        $('#data_grid tr[id=' + $('#hdnEmailID').val() + ']').css({ "background-color": "LightSkyBlue", "color": "Black" });
        $('#data_grid tr[id]').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });
      
        
        //FUNCION ACTIVA SELECCION CLIK EN EL DATAGREDVIEW DE VALIDACION RADICACION
        $('#GridView_val_radicacion tr[id]').click(function () {
            $('#GridView_val_radicacion tr[id]').css({ "background": "White", "color": "Black" });
            $(this).css({ "background-color": "#e8e8f7", "color": "Black" });
                var fer = $(this).attr("id");
                $('#hdnEmailID_VAL').val(fer);
             
        });

       
        $('#GridView_list_documento_relacion tr[id_rad]').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });
        $('#GridView_list_registro_rad tr[id]').click(function () {
            try {

                $('#GridView_list_registro_rad tr[id]').css({ "background": "White", "color": "Black" });
                $(this).css({ "background-color": "#e8e8f7", "color": "Black" });
                var fer = $(this).attr("idd");
                $('#Hidden_list_registro_rad').val(fer);
            }
            catch (err) {
                alert(err.message + " Funcion clik");
            }
        });
        $('#GridView_list_registro_rad tr[id]').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });
        //ASIGNA EL CURSOR DE SELECCION CUANDO PASA EL CURSOR EN EL DATAGREDVIEW DE VALIDACION RADICACION
        $('#GridView_val_radicacion tr[id]').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });
        $('#GridView_val_radicacion tr[id]').dblclick(function () {
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

        });
       
        //INICIA INTERFACE POPUP VALIDACION RADICADOS
        var tempo = document.getElementById("idente_chekbi_actyive");
        if (tempo === null) {
           //$("#GridView_val_radicacion th:nth-child(1)").append(" <input id='idente_chekbi_actyive' type='checkbox' name='activa_deativa_chek' onchange=desactiva_ch_data_grid('idente_chekbi_actyive') class='mmmjjjkkkuuu'  />");
        }
       

        //********************************DESTINATARIO INTERNO*********************************************************************************************
        //FUNCION QUE CONTROLA EL CLIK SOBRE EL GREVIEW DE DESTINATARIO INTERNO
        $('#data_grid_auxiliar_lista tr[id]').click(function () {
            $('#data_grid_auxiliar_lista tr[id]').css({ "background-color": "transparent", "color": "Black" });
            $(this).css({ "background-color": " #e8e8f7", "color": "Black" });
                   
        });
       
        
        //ASIGNA EL CURSOR DE SELECCION CUANDO PASA EL CURSOR EN EL DATAGREDVIEW DEL DESTINATARIO INTERNO
        $('#data_grid_auxiliar_lista tr[id]').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });
       
        //INICIA INTERFACE POPUP DESTINATARIO INTERNO
        //inicio_tab_radicador();
        auto_resize_radicacion();
        auto_zise_popup_internos();
        auto_zise_popup_usuarios_externos();
        auto_zise_popup_plantilla_validacion();
        auto_zise_popup_visor_externo();
        auto_zise();
        auto_zise_popup_ventana_expediente();
        auto_zise_popup_lista_chequeo_edita();
        auto_zise_popup_dias_habiles();
        auto_size_imprimir();
        auto_zise_popup_lista_radicados();
        GetLista_posibles_remitentes("TextBoxcontenidobusqueda");
        GetLista_lista_actividades_ruta("TextBox_buequeda_general_lista_actividades");
        GetLista_listado_usuarios_workflow_ruta("TextBox_buequeda_general_lista_usuarios");
        auto_zise_popup_trace_grafic();
        auto_zise_popup_lista_usuario_flujo();
        auto_zise_popup_lista_actividades_ruta();
        auto_zise_popup_lista_usuarios_ruta();
        auto_zise_popup_detalle_radicado();
        auto_zise_popup_adjunta_documento_workflow();
        auto_size_content_anotacion();
        auto_zise_nota_tarea();
    }
   
});
var RADICADO = "";
var RADICADO_PEDIENTE = 0;
var TIPO_ENVIO_TRAMITE = 0;
var ARRAY_CHEK_LIST = new Array();  //GUARDA DE LOS CHEK LIST SELECCIONADO
var ESTADO_EVENT_GENERAL = "";
var INTERVAL_EVENT_GENERAL;

let asmxClient;

//const asmxClient = new ASMXClient(AsmxServicesConfig);
$(window).on("load", function () {
    try {
        var elment = document.getElementsByClassName("da_event_captive");
        if (elment) {
            for (var i = 0; i < elment.length; i++) {
                elment[i].addEventListener("click", event_click, false);
            }
        }
        var elemnt_service_vlidate = document.getElementById("buton_validate_chek");
        if (elemnt_service_vlidate) {
            elemnt_service_vlidate.addEventListener("click", service_validate_chek_list, false);
        }
        //inicializa boton delte multiple row ventana enlace radicado
        let elment_delete_row_several_rad = document.getElementById("delete_row_several_rad");
        if (elment_delete_row_several_rad) {
            elment_delete_row_several_rad.addEventListener("click", handler_element_event, false);
        }
        ini_event_page();
        inicio_tab_radicador();
        window.addEventListener("resize", rezize_event);
        ShowModalPopup("ModalPopupExtender_mensaje_personalizado_backgroundElement", "Panel_mensaje_personalizado", 100001);      
        ShowModalPopup("ModalPopupExtendermensaje_autoterminar_backgroundElement", "Panelmensaj_autoterminar", 100001);
        ShowModalPopup("ModalPopupExtender_edition_detalle_actividad_flujo_backgroundElement", "Panel_detalle_actividad_flujo", 100001);
        ShowModalPopup("ModalPopupExtender_edition_detalle_actividad_flujo_user_backgroundElement", "Panel_detalle_actividad_flujo_user", 100001);
        ShowModalPopup("ModalPopupExtender_edition_detalle_radicado_backgroundElement", "Panel_detalle_radicado", 100001);
        ShowModalPopup("ModalPopupExtender_edition_interface_regitra_meta_dato_backgroundElement", "Panel_interface_regitra_meta_dato", 100002);
        ShowModalPopup("ModalPopupExtender_sube_documento_content_general_backgroundElement", "Panel_sube_documento_content_general", 100002);
        ShowModalPopup("ModalPopupExtender_edition_nota_respuesta_backgroundElement", "Panel_nota_respuesta", 1000012); 
        GetLista_posibles_remitentes("TextBoxcontenidobusqueda");
        GetLista_lista_actividades_ruta("TextBox_buequeda_general_lista_actividades");
         
    } catch (e) {
        alert(" funcion load " + e.message);
    }

})
const ini_event_page = () => {
    asmxClient = new ASMXClient(AsmxServicesConfig);
    //active note procesing
    array_element = new Array;
    array_element.push(
        { id: "Button_actualizar_nota" }, { id: "Button_Show_Guardar" }, { id: "Button_duardar_nota" }, { id: "Button_actualizar_nota" },
        { id: "sing_multiple_file" }, { id: "a_load_file" }, { id: "a_load_file_nav"}
    );
    for (let i = 0; i < array_element.length; i++) {
        let elment_a_document_production = document.getElementById(array_element[i].id);
        if (elment_a_document_production) {
            elment_a_document_production.addEventListener("click", handler_element_event, false);
        }
    }
    
}
//CONTROLA LOS EVENTOS DE LOS IMPUT
const handler_element_event = (e) => {
    try {
        let name_ID = e.currentTarget.id;
        switch (name_ID) {
            case "a_load_file_nav":
                event_element_click_promise(e);
                break;
            case "a_load_file":
                event_element_click_promise(e);
                break;
            case "sing_multiple_file": {
                event_element_click_promise(e);
                break;
            }
            case "delete_row_several_rad":   //Elimina multiplex row documento relacionado
                hanler_delete_row_several("GridView_list_documento_relacion", "chek_selecion_list_rad", "C-DW-DEL-IMAGE");
                break;
            case "Button_actualizar_nota":
                result = Event_note_workflow(document.getElementById("hdnidlista").value, document.getElementById("TextBox_nota").value, "Button_actualizar_nota");
                if (result !== "YES") {
                    alert(result);
                }
                break;
            case "Button_Show_Guardar":
                result = Event_note_workflow("", "", "Button_Show_Guardar");
                if (result !== "YES") {
                    alert(result);
                }
                break;
            case "Button_duardar_nota":
                result = Event_note_workflow("", document.getElementById("TextBox_nota").value, "Button_duardar_nota");
                if (result !== "YES") {
                    alert(result);
                }
                break;
        }

    } catch (ex) {
        alert(ex.mensaje);
    }
}
const event_element_click_promise = async (e) => {
    let name_control = e.currentTarget.id;
    try {
        let result = "";
        delete_alert_boot();
        e.currentTarget.disabled = true;
        posicion_update_pogres('progres_bar');
        if (name_control == "a_load_file" || name_control == "a_load_file_nav") {
            result = await ActivaAdjuntarDocumentoRadicacion();
            if (result !== "YES") {
                alert_bot(result, 'warning', "div_error_content_rad");
            }
        }
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
const Event_note_workflow = (ident_note, date_note, ident_booton) => {
    try {
        if (ident_booton == "Button_actualizar_nota") {
            if (ident_note == - 1 || ident_note == 1) {
                return "Debe selecionar la nota";
            }
            if (date_note == "") {
                return "Debe informar la nota";
            }
            event_element_menu(ident_booton, "");
            return "YES"
        }
        //Activa ventana guardar nota
        if (ident_booton == "Button_Show_Guardar") {
            let result = Show_new_note_workflow();
            return result;
        }
        //Guarda la nota workflow
        if (ident_booton == "Button_duardar_nota") {
            event_element_menu(ident_booton, "");
            return "YES"
        }
        return "YES"
    } catch (ex) {
        return "Error funcion Event_note_workflow " & ex.mensaje
    }
}
//Active venata add new note task workflow
const Show_new_note_workflow = () => {
    try {
        document.getElementById("TextBox_nota").value = "";
        document.getElementById("Button_actualizar_nota").style.display = "none";
        document.getElementById("Button_duardar_nota").style.display = "flex";
        document.getElementById("Label_nota_respuesta").innerHTML = "Nueva nota";
        $find("ModalPopupExtender_edition_nota_respuesta").show();
        auto_zise_nota_tarea();
        return "YES";
    }
    catch (ex) {
        return "Error funcion Event_note_workflow " & ex.mensaje
    }
}
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
function event_element_clic(event, e) {
    try {
        ESTADO_EVENT_GENERAL = "intro";
        posicion_update_pogres('progres_bar');
        e.disabled = true;
        INTERVAL_EVENT_GENERAL = setInterval(fx_funcion, 100);
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
                if (e.id == "Button_guarda_rotulo") {
                    service_save_file_save_rotulo_radicado();
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
        INTERVAL_EVENT_GENERAL = setInterval(fx_funcion, 100);
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
                if (evento == "firma_doc_selecion_rad_") {
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
                    //inicializa_upload_file_client(tip_event);
                    //document.getElementById("Hidden_tip_adjunt").value = "rad";
                    //parameter_upload(ESTADO_EVENT_GENERAL, "WORKFLOW", "Button_tool_activa_sube_documento", "multiple", tip_event);
                    //return true;
                }
                if (evento == "I-GD-RD") {
                    $("#DropDownList_documento_sube_documento_content_general").empty();
                    service_source_list_tipos_documentales_radicado(0, 'DropDownList_documento_sube_documento_content_general', 'service_source_list_item_control_general_documento_radicado');
                    return true;
                }
                if (evento == "C-DW-DEL-IMAGE") {
                    event_multiple_row("", "GridView_list_documento_relacion", "elimina_doc_relacionado_wf_radicado");
                }
                if (evento == "Button_actualizar_nota") {
                    Service_actualiza_nota_tarea_workflow(document.getElementById("hdnidlista").value, document.getElementById("TextBox_nota").value);
                    return true;
                }
                if (evento == "Button_duardar_nota") {
                    Service_add_nota_tarea_workflow(document.getElementById("TextBox_nota").value);
                    return true;
                }
                if (evento == "delete_note_workflow") {
                    Service_delete_nota_tarea_workflow(tip_event, document.getElementById("TextBox_nota").value);
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
        //Activa guardar rotulo radicado
        if (value_sel == "I-GD-RD") {
            event_element_menu("I-GD-RD", "adjunto_doc_visor_");
           
        }
        //Activa subir el documento desde el visor 
        if (value_sel == "C-DW-VIS") {
            event_element_menu("C-DW-VIS", "adjunto_doc_visor");
        }
        //Activa subir el documento desde enlace
        if (value_sel == "C-DW-RD") {
            event_element_menu("C-DW-RD", "adjunto_doc_visor");
        }
        if (value_sel == "C-DW-DEL-IMAGE") {
            ITEMS_IMAGE_LIST_WF = new Array;
            ITEMS_IMAGE_LIST_WF = table_gred_select_check_item("GridView_list_documento_relacion_wf", "chek_selecion_list_wf");
            if (ITEMS_IMAGE_LIST_WF.length == 0) {
                alert("Debe seleccionar los documentos de la lista");
                return true;
            }
            event_element_menu("C-DW-DEL-IMAGE", "");
        }

    }
    catch (err) {
        alert(err.message + " Funcion inicializa_tipo_adjunto_documento");
    }
}
const ActivaAdjuntarDocumentoRadicacion = async () => {
    try {
        let resp1 = await asmxClient
            .use("Radicacion")
            .call("ServiceSolicitaEstructuraTramiteAsignado", { Parameter: 0 });
        if (resp1.error) {
            return resp1.message;
        }
        let Data = resp1.data[0];
        resp1 = await asmxClient
            .use("ConfigDigitaliacion")
            .call("ServiceSolicitaEstructuraConfiguracion", { IdTipoTramite: Data.CDRAsginaGestionDocumentos[0].IdTipoTramite});
        if (resp1.error) {
            return resp1.message;
        }
        Data = resp1.data[0];
        let _OPtionFileLoad = ({
            NameLoadProceso: "ADJUNTARADICACION",
            NameContenedorError: "error_content_adjunta_documeto_load_documento_006",
            funcion_name: "insert_row_documento_relacionado", evento_adjunta: "ADJUNTARADICACION",
            IdRespuestaIdExpediente: 0,
            NameContendorLoadDocumento: "soporte_envio", ModalWidth: 75, CargaTipologia: 1,
            CargaFecha: 1, CargaPreview: 1, multi_select: "multiple",
            element_parent: "modal_adjunta_documeto_load_documento_006", TipoFormulario: 1,
            name_serivce_list: "service_source_list_item_control_general_documento_radicado",
            name_class_serivce_list: "WebServiceRadicacion.asmx",
            element_html_table: "GridView_list_documento_relacion", element_html_lab_conteo: "Label_documentos", apost_html_lab_conteo: "Documentos",
            setioption_obliga_tipologia: Data.Obliga_Lista_Chequeo
        });
        let result = await IniLoadPerson(_OPtionFileLoad);
        if (result != "YES") {
            return result;
        }
        document.getElementById("Hidden_tip_adjunt").value = "rad";
        return "YES"
    } catch (ex) {
        return "Inconsistencia funcion ActivaAdjuntarDocumentoRadicacion " + ex.mensaje;
    }
}
function rezize_event() {
    try {
        auto_zise_popup_visor_externo();
        auto_resize_radicacion();
        auto_zise_popup_internos();
        auto_zise_popup_usuarios_externos();
        auto_zise_popup_plantilla_validacion();
        auto_zise();
        auto_zise_popup_ventana_expediente();
        auto_zise_popup_lista_chequeo_edita();
        auto_size_imprimir();
        plugin_grwedview();
        auto_zise_popup_dias_habiles();
        auto_zise_popup_trace_grafic();
        auto_zise_popup_lista_radicados();
        GetLista_posibles_remitentes("TextBoxcontenidobusqueda");
        GetLista_lista_actividades_ruta("TextBox_buequeda_general_lista_actividades");
        GetLista_listado_usuarios_workflow_ruta("TextBox_buequeda_general_lista_usuarios");
        auto_zise_popup_lista_usuario_flujo();
        auto_zise_popup_lista_actividades_ruta();
        auto_zise_popup_lista_usuarios_ruta();
        auto_zise_popup_detalle_radicado();
        auto_zise_popup_adjunta_documento_workflow();
        auto_size_content_anotacion();
        auto_zise_nota_tarea();
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
//------------------------------------//--------------------------
//      ZONA NOTAS



////WEB SERVICE ACTUALIZA NOTA TAREA
function Service_actualiza_nota_tarea_workflow(id_nota, value_nota) {
    try {
        $.ajax('../webservice/WebServiceWorkflow.asmx/Service_actualiza_nota_tarea_workflow', {
            data: "{" + "'parameter':'" + id_nota + "','value_nota':'" + value_nota + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_result !== "YES") {
                    alert(data.d[0].error_result);
                    ESTADO_EVENT_GENERAL = "out";
                } else {
                    actualiza_gre_campo_wf_lista('GridView_lista_notas', data.d[0].identificador, data.d[0].value, 'NOTA');
                    $find("ModalPopupExtender_edition_nota_respuesta").hide();
                    ESTADO_EVENT_GENERAL = "out";
                }
            }, error: function (xception, textStatus, errorThrown) {
                ESTADO_EVENT_GENERAL = "out";
                if (xception.status === 0) {
                    alert('Not connect: Verify Network.');

                } else if (xception.status == 404) {
                    alert('Requested page not found [404]');

                } else if (xception.status == 500) {
                    alert('Internal Server Error [500].' + xception.responseText);


                } else if (textStatus === 'parsererror') {
                    alert('Requested JSON parse failed.');


                } else if (textStatus === 'timeout') {
                    alert('Time out error.');


                } else if (textStatus === 'abort') {
                    alert('Ajax request aborted.');

                } else {
                    alert('Uncaught Error: ' + xception.responseText);

                }
            }
        });
    }
    catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        alert(ex.message);
    }
}
//WEB SERVICE ELIMINA ANOTAION
function Service_delete_nota_tarea_workflow(id_nota, value_nota) {
    try {
        $.ajax('../webservice/WebServiceWorkflow.asmx/Service_delete_nota_tarea_workflow', {
            data: "{" + "'parameter':'" + id_nota + "','value_nota':'" + value_nota + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_result !== "YES") {
                    alert(data.d[0].error_result);
                    ESTADO_EVENT_GENERAL = "out";
                } else {
                    let result = eliminar_fila_data_gred_nota(data.d[0].identificador, 'GridView_lista_notas');
                    if (result !== "YES") {
                        alert(result);
                    }
                    ESTADO_EVENT_GENERAL = "out";
                }
            }, error: function (xception, textStatus, errorThrown) {
                ESTADO_EVENT_GENERAL = "out";
                if (xception.status === 0) {
                    alert('Not connect: Verify Network.');

                } else if (xception.status == 404) {
                    alert('Requested page not found [404]');

                } else if (xception.status == 500) {
                    alert('Internal Server Error [500].' + xception.responseText);


                } else if (textStatus === 'parsererror') {
                    alert('Requested JSON parse failed.');


                } else if (textStatus === 'timeout') {
                    alert('Time out error.');


                } else if (textStatus === 'abort') {
                    alert('Ajax request aborted.');

                } else {
                    alert('Uncaught Error: ' + xception.responseText);

                }
            }
        });
    }
    catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        alert(ex.message);
    }
}
//WEB SERVICE ADD ANOTACION
function Service_add_nota_tarea_workflow(value_nota) {
    try {
        $.ajax('../webservice/WebServiceWorkflow.asmx/Service_add_nota_tarea_workflow', {
            data: "{'value_nota':'" + value_nota + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_result !== "YES") {
                    alert(data.d[0].error_result);
                    ESTADO_EVENT_GENERAL = "out";
                } else {
                    let result = insert_row_list_anotation(data, "GridView_lista_notas");
                    if (result !== "YES") {
                        alert(result);
                    }
                    $find("ModalPopupExtender_edition_nota_respuesta").hide();
                    ESTADO_EVENT_GENERAL = "out";
                }
            }, error: function (xception, textStatus, errorThrown) {
                ESTADO_EVENT_GENERAL = "out";
                if (xception.status === 0) {
                    alert('Not connect: Verify Network.');

                } else if (xception.status == 404) {
                    alert('Requested page not found [404]');

                } else if (xception.status == 500) {
                    alert('Internal Server Error [500].' + xception.responseText);


                } else if (textStatus === 'parsererror') {
                    alert('Requested JSON parse failed.');


                } else if (textStatus === 'timeout') {
                    alert('Time out error.');


                } else if (textStatus === 'abort') {
                    alert('Ajax request aborted.');

                } else {
                    alert('Uncaught Error: ' + xception.responseText);

                }
            }
        });
    }
    catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        alert(ex.message);
    }
}
//WEB SERVICE SOLICITA CONTENIDO NOTA
function Service_contenido_nota_tarea_workflow(id_nota, element_name) {
    try {
        $.ajax('../webservice/WebServiceWorkflow.asmx/Service_contenido_nota_tarea_workflow', {
            data: "{" + "'parameter':'" + id_nota + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_result !== "YES") {
                    alert(data.d[0].error_result);
                    ESTADO_EVENT_GENERAL = "out";
                } else {
                    document.getElementById(element_name).value = data.d[0].value;
                    document.getElementById("Button_actualizar_nota").style.display = "flex";
                    document.getElementById("Button_duardar_nota").style.display = "none";
                    document.getElementById("Label_nota_respuesta").innerHTML = "Nota " + data.d[0].identificador;
                    $find("ModalPopupExtender_edition_nota_respuesta").show();
                    auto_zise_nota_tarea();
                    ESTADO_EVENT_GENERAL = "out";
                }
            }, error: function (xception, textStatus, errorThrown) {
                ESTADO_EVENT_GENERAL = "out";
                if (xception.status === 0) {
                    alert('Not connect: Verify Network.');

                } else if (xception.status == 404) {
                    alert('Requested page not found [404]');

                } else if (xception.status == 500) {
                    alert('Internal Server Error [500].' + xception.responseText);


                } else if (textStatus === 'parsererror') {
                    alert('Requested JSON parse failed.');


                } else if (textStatus === 'timeout') {
                    alert('Time out error.');


                } else if (textStatus === 'abort') {
                    alert('Ajax request aborted.');

                } else {
                    alert('Uncaught Error: ' + xception.responseText);

                }
            }
        });
    }
    catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        alert(ex.message);
    }
}
function auto_size_content_anotacion() {
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
        $('#modal_content_anotacion').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#contenido_procesa_content_anotacion').css("height", (document.getElementById("modal_content_anotacion").clientHeight - (document.getElementById("content_boton").clientHeight + document.getElementById("diver_cabcera_content_anotacion").clientHeight)) + "px");
        //Para los modal que contiene gred
        $('#Panel_content_anotacion_gred').css("height", (document.getElementById("contenido_procesa_content_anotacion").clientHeight - 5) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_size_content_anotacion " + err.message);
    }
}
function auto_zise_nota_tarea() {
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
        var heig_porcent = espacio_iframe - ((espacio_iframe * 50) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_nota_respuesta').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_nota_respuesta').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#contenido_procesa_nota_respuesta').css("height", (document.getElementById("modal_content_nota_respuesta").clientHeight - (document.getElementById("divcabecer_nota_respuesta").clientHeight + document.getElementById("content_boton_nota").clientHeight)) + "px");
        //Para los modal que contiene gred
        //$('#TextBox_nota').css("height", (document.getElementById("contenido_procesa_nota_respuesta").clientHeight - 5) + "px");

    }
    catch (err) {
        alert(err.message + " funcion auto_zise_nota_tarea " + err.message);
    }
}
const eliminar_fila_data_gred_nota = (id_nota, name_table) => {
    try {
        $("#" + name_table + " tr[id=" + id_nota + "]").remove();
        $('#hdnidlista').val("-1");
        return "YES";
    }
    catch (err) {
        return "Funcion eliminar_fila_data_gred_nota error : " + err.mensaje;
    }

}
//Inserta row lista anotaciones
const insert_row_list_anotation = (array_date, data_table) => {
    try {
        var element_table = document.getElementById(data_table);
        var element_row;
        var element_td;
        var index_tr_title = -1;
        for (i = 0; i < element_table.rows.length; i++) {
            if (element_table.rows[i].className == "GridviewScrollHeader_line_boot") {
                index_tr_title = i + 1;
            }
        }
        element_row = element_table.insertRow(index_tr_title);
        //Agrega los atributos del row
        var conta_td = 0;
        element_td = element_row.insertCell(conta_td);
        element_row.setAttribute("id", array_date.d[0].detailt_note.id_anotacion);
        element_row.style.cursor = "pointer";
        element_row.style.background = "#e8e8f7";
        element_row.style.color = "black";
        //Agrega el boton de ver anotacion
        var divhtml = document.createElement("div");
        var ihtml = document.createElement("i");
        ihtml.style.color = "white";
        ihtml.classList.add("fas");
        ihtml.classList.add("fa-sticky-note");
        var ahtml = document.createElement("a");
        ahtml.classList.add("btn");
        ahtml.classList.add("btn-success");
        ahtml.classList.add("btn-sm");
        ahtml.setAttribute("onclick", "prevent_event(event,this);");
        ahtml.setAttribute("title", "nota");
        ahtml.setAttribute("idd", array_date.d[0].detailt_note.id_anotacion);
        ahtml.setAttribute("tip_event", "ver_nota");
        ahtml.style.marginLeft = "3px";
        ahtml.appendChild(ihtml);
        divhtml.appendChild(ahtml);
        //Agrega boton eliminar nota
        ihtml = document.createElement("i");
        ihtml.style.color = "white";
        ihtml.classList.add("far");
        ihtml.classList.add("fa-trash-alt");
        ihtml.classList.add("fa-lg");
        ahtml = document.createElement("a");
        ahtml.classList.add("btn");
        ahtml.classList.add("btn-danger");
        ahtml.classList.add("btn-sm");
        ahtml.setAttribute("onclick", "prevent_event(event,this);");
        ahtml.setAttribute("title", "Eliminar nota");
        ahtml.setAttribute("idd", array_date.d[0].detailt_note.id_anotacion);
        ahtml.setAttribute("tip_event", "eli_nota");
        ahtml.style.marginLeft = "3px";
        ahtml.appendChild(ihtml);
        divhtml.appendChild(ahtml);
        divhtml.style.display = "inline-flex";
        element_td.appendChild(divhtml);
        conta_td++;
        element_td = element_row.insertCell(conta_td);
        element_td.innerHTML = array_date.d[0].detailt_note.nombre_usuario;
        element_td.classList.add("GridviewScrollItem_line_cort_tr_flex");
        element_td.setAttribute("onclick", "prevent_scrol(event,this,'')");
        conta_td++;
        element_td = element_row.insertCell(conta_td);
        element_td.innerHTML = array_date.d[0].detailt_note.loguin_usuario;
        element_td.classList.add("GridviewScrollItem_line_cort_tr_flex");
        element_td.setAttribute("onclick", "prevent_scrol(event,this,'')");
        conta_td++;
        element_td = element_row.insertCell(conta_td);
        element_td.innerHTML = array_date.d[0].detailt_note.dato_anotacion;
        element_td.classList.add("GridviewScrollItem_line_cort_tr_flex");
        element_td.setAttribute("onclick", "prevent_scrol(event,this,'')");
        conta_td++;
        element_td = element_row.insertCell(conta_td);
        element_td.innerHTML = array_date.d[0].detailt_note.fecha_anotacion;
        element_td.classList.add("GridviewScrollItem_line_cort_tr_flex");
        element_td.setAttribute("onclick", "prevent_scrol(event,this,'')");
        return "YES";
    }
    catch (ex) {
        return "function insert_row_list_anotation error " + ex.mensaje;
    }
}
function prevent_event(event, element) {
    try {
        var fer = $(element).attr("idd");
        var tip_event = $(element).attr("tip_event");
        if (tip_event == "eli_nota") {
            var r = confirm("Desea eliminar la nota");
            if (r == false) {
                return false;
            }
            $('#hdnidlista').val(fer);
            event_element_menu("delete_note_workflow", fer);
        }
        if (tip_event == "ver_nota") {
            $('#hdnidlista').val(fer);
            Service_contenido_nota_tarea_workflow(fer, "TextBox_nota");
        }
        event.preventDefault();
        element.focus();
    }
    catch (err) {
        alert(err.message + " Funcion prevent_event");
    }
}
function actualiza_gre_campo_wf_lista(nombre_grid, id, valor_campo, nombre_campo) {
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
        alert(err.message + " Funcion actualiza_gre_campo_wf_lista");
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
//-----------------------------------//---------------------------
//                ZONA DELETE MULTIPLEX ROWS
//----------------------------------//----------------------------
//ZONA CHEK DATA GREED
function table_gred_on_click_check(elemen, table, name_class_check) {
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
function table_gred_select_check_item(table, name_class_check) {
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
//ZONA ELIMINACION ROW
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
const decrementa_documento_relacion=() => {
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

//ZONA GUARDA ROTULO RADICADO
function service_source_list_tipos_documentales_radicado(id_, name_control, name_service) {
    try {
        $.ajax('../webservice/WebServiceRadicacion.asmx/' + name_service, {
            data: "{'id':" + "'" + id_ + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_sistema !== "YES") {
                    alert(data.d[0].error_sistema);
                    ESTADO_EVENT_GENERAL = "out";
                } else {
                    ITEMS_DATOS_DROW = new Array();
                    $.each(data.d[0].item_sistema, function (k, v) {
                        ITEMS_DATOS_DROW.push(v);
                    });
                    if (document.getElementById(name_control)) {
                        var element_drow = document.getElementById(name_control);
                        for (var i = 0; i < ITEMS_DATOS_DROW.length; i++) {
                            element_drow[i] = new Option(ITEMS_DATOS_DROW[i].text, ITEMS_DATOS_DROW[i].value);
                        }
                        $find("ModalPopupExtender_sube_documento_content_general").show();
                        ESTADO_EVENT_GENERAL = "out";
                    }
                }
            }, error: function (xception, textStatus, errorThrown) {
                ESTADO_EVENT_GENERAL = "out";
                if (xception.status === 0) {

                    alert('Not connect: Verify Network.');

                } else if (xception.status == 404) {

                    alert('Requested page not found [404]');

                } else if (xception.status == 500) {

                    alert('Internal Server Error [500].' + xception.responseText);

                } else if (textStatus === 'parsererror') {

                    alert('Requested JSON parse failed.');

                } else if (textStatus === 'timeout') {

                    alert('Time out error.');

                } else if (textStatus === 'abort') {

                    alert('Ajax request aborted.');

                } else {

                    alert('Uncaught Error: ' + xception.responseText);

                }
            }
        });
    }
    catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        alert('service_source_list_tipos_documentales_radicado ' + ex.message);
    }
}

function service_save_file_save_rotulo_radicado() {
    try {  
        var control_element_ = document.getElementById("DropDownList_documento_sube_documento_content_general");
        var texto_campo = control_element_.options[control_element_.selectedIndex].text;
        var valor_campo = control_element_.options[control_element_.selectedIndex].value;
        var array_file = new Array();
        array_file.push({ 'id_expediente' : '0', 'id_tipo_documento' : valor_campo, 'nombre_tipo_documento' : texto_campo, 'estado_adjunta_anexo' : '0', 'estado_adjunta_relacionado' : '0', 'numero_documento_relacionado' : '0' });
        var serialice = JSON.stringify(array_file);
        $.ajax('../webservice/WebServiceRadicacion.asmx/service_save_file_save_rotulo_radicado' , {
            data: "{'parameter':" + "'" + serialice + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_sistema !== "YES") {
                    alert(data.d[0].error_sistema);
                    ESTADO_EVENT_GENERAL = "out";
                } else {
                    var file_icon_some = "fa-file"
                    if (data.d[0].icono_icono_awe_some != "") {
                        var espacio = " ";
                        var spli_some = data.d[0].icono_icono_awe_some.split(espacio);
                        file_icon_some = spli_some[1];
                    }
                    var date_campo = data.d[0].name_gabinete + "|" + data.d[0].id_image + "|" + data.d[0].radicado + "|" +
                        data.d[0].tipodocumental + "|" + data.d[0].notitipodocumental + "|" + data.d[0].id_tarea_workflow + "|" +
                        data.d[0].estado_firma_digital + "|" + file_icon_some;
                    insert_row_documento_relacionado(date_campo, "rad");
                    $find("ModalPopupExtender_sube_documento_content_general").hide();
                    ESTADO_EVENT_GENERAL = "out";
                }
            }, error: function (xception, textStatus, errorThrown) {
                ESTADO_EVENT_GENERAL = "out";
                if (xception.status === 0) {

                    alert('Not connect: Verify Network.');

                } else if (xception.status == 404) {

                    alert('Requested page not found [404]');

                } else if (xception.status == 500) {

                    alert('Internal Server Error [500].' + xception.responseText);

                } else if (textStatus === 'parsererror') {

                    alert('Requested JSON parse failed.');

                } else if (textStatus === 'timeout') {

                    alert('Time out error.');

                } else if (textStatus === 'abort') {

                    alert('Ajax request aborted.');

                } else {

                    alert('Uncaught Error: ' + xception.responseText);

                }
            }
        });
    }
    catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        alert('service_save_file_save_rotulo_radicado ' + ex.message);
    }
}
//TERMINA ZONA ROTULO RADICADO
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
                numero_documento_relacionado, element_html_actuliza, element_update_panel, id_respuesta, tipo_adjunta, element_isert_table,"", "", 0);
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
function inicio_tab_radicador() {
    if (document.getElementById("Hidden_radicado_seleccion").value !== "") {
        tab_sow('soporte_envio', 'soporte-envio_nav');
        tab_enabled('soporte-envio_nav');
        tab_disable('home-radicador');
        document.getElementById("Are_Digitalizacion").style.display = "block";
        show_tab_boton_content_gestion_radicado();
        if (document.getElementById("IframeDitaliza_").src == "") {
            document.getElementById("IframeDitaliza_").src = "../workflow/WebFormEscan.aspx";
        }
       
    } else {
        
        if (document.getElementById("Hidden_numero_rad_pend").value == "0") {
            tab_sow('home_radic', 'home-radicador');
            tab_enabled('home-radicador');
            tab_disable('soporte-envio_nav');
            document.getElementById("Are_Digitalizacion").style.display = "none";
            document.getElementById('Area_Visor').style.display = 'none';
            show_tab_boton_content_radicado();
        } else {
            tab_sow('soporte_envio', 'soporte-envio_nav');
            tab_enabled('soporte-envio_nav');
            document.getElementById("Are_Digitalizacion").style.display = "none";
            document.getElementById('Area_Visor').style.display = 'none';
            show_tab_boton_content_gestion_radicado();
        }
               
    }
}
function tab_sow(name, namepadre) {  
    $('#' + name).toggleClass('active');
    $('#' + namepadre).toggleClass('active');
}
function tab_disable(name) {
    if ($('#' + name).hasClass("disabled") === false) {
        $('#' + name).toggleClass('disabled');
        $('#' + name + 'i').toggleClass('d-none');
    }

}
function tab_enabled(name) {
    if ($('#' + name).hasClass("disabled") === true) {
        $('#' + name).toggleClass('disabled');
        $('#' + name + 'i').toggleClass('d-none');
    }

}
function nuevo_radicado_tab() {
    try {
    $(".active").each(function () {
        if ($(this).hasClass("active") === true) {
            $(this).toggleClass("active");
        }     
    });
    tab_sow('home_radic', 'home-radicador');
    tab_enabled('home-radicador');
    if (document.getElementById("Hidden_numero_rad_pend").value == "0") {
        tab_disable('soporte-envio_nav');
    } else {
        tab_enabled('soporte-envio_nav');
    }
    document.getElementById("Are_Digitalizacion").style.display = "none";
    document.getElementById('Area_Visor').style.display = 'none';
    if (document.getElementById("tab_nuevo_radicado")) {
       document.getElementById("tab_nuevo_radicado").style.display = "none";
    }
    if (document.getElementById("content_radicado_boton")) {
            document.getElementById("content_radicado_boton").style.display = "block";
    }     
}
    catch (err) {
        alert(err.message + " Funcion nuevo_radicado_tab");
}
}
function show_tab_boton_content_radicado() {
    try {
        if (document.getElementById("home-radicador").classList.contains("disabled") === false) {
            document.getElementById("content_radicado_boton").style.display = "block";
            document.getElementById("tab_nuevo_radicado").style.display = "none";
        }
       
    } catch (ex) {
        alert("Funcion show_tab_boton_content_radicado " + ex.mensaje);
    }
}
function show_tab_boton_content_gestion_radicado() {
    try {
        
        if (document.getElementById("soporte-envio_nav").classList.contains("disabled") === false) {
            document.getElementById("content_radicado_boton").style.display = "none";
            document.getElementById("tab_nuevo_radicado").style.display = "block";
        }
       
    } catch (ex) {
        alert("Funcion show_tab_boton_content_gestion_radicado " + ex.mensaje);
    }
}
function asig_radicado_tab() {
    try {
        $(".active").each(function () {
            if ($(this).hasClass("active") === true) {
                $(this).toggleClass("active");
            }
        });
        tab_sow('soporte_envio', 'soporte-envio_nav');
        tab_enabled('soporte-envio_nav');
        tab_disable('home-radicador');
        document.getElementById("Are_Digitalizacion").style.display = "block";
        if (document.getElementById("IframeDitaliza_").src == "") {
            document.getElementById("IframeDitaliza_").src = "../workflow/WebFormEscan.aspx";
        }
        document.getElementById('Area_Visor').style.display = 'none';
        show_tab_boton_content_gestion_radicado();
    }

    catch (err) {
        alert(err.message + " Funcion asig_radicado_tab");
    }
}
function terminar_radicado_tab() {
    try {
        if (document.getElementById("Hidden_numero_rad_pend").value == "0") {
            tab_disable('soporte-envio_nav');
            
        } else {
            tab_enabled('soporte-envio_nav');
        }
        document.getElementById("Are_Digitalizacion").style.display = "none";
        document.getElementById('Area_Visor').style.display = 'none';
        if (document.getElementById("GridView_list_documento_relacion")) {
            if ($('#GridView_list_documento_relacion tr').children.length > 0) {
                $("#GridView_list_documento_relacion tr").remove();
            }
        }
        document.getElementById("Label_documentos").innerHTML = "Documentos 0";
    } catch (ex) {
        alert("Funcion terminar_radicado_tab " + ex.message);
    }
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
function service_validate_chek_list(e) {
    try {
        $('#data_grid_chequeo_actualiza tr[id]').css({ "background-color": "White" });
        retorna_check_lista_check();
        if (ARRAY_CHEK_LIST.length == 0) {
            alert("No hay elementos selecionados en la lista de chequeo para validar");
            return true;
        }
        var id_tipo_tramite = document.getElementById("RE_Descripcion_Documento").value;
        if (id_tipo_tramite == 0) {
            alert("Debe seleccionar el tipo tramite");
            return true;
        }
        var valParam = JSON.stringify(ARRAY_CHEK_LIST);
        $.ajax('../webservice/WebServiceRadicacion.asmx/verifica_cheklis_documentos_radicados', {
            data: "{'item_chek':'" + valParam + "'," + "'id_tipo_tramite':'" + id_tipo_tramite + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                var res = data.d.split("|");
                if (res[0] !== "YES") {
                    alert(res[0]);
                } else {
                    if (res[1] == "") {
                        alert("Cumple con el requisito documental obligatorio");
                        return true;
                    }
                    var trs = document.getElementById("data_grid_chequeo_actualiza").tBodies[0].getElementsByTagName('tr');
                    var spliter = res[1].split(",")
                    for (i = 0; i < spliter.length; i++) {
                        if (spliter[i] !== "") {
                            $('#data_grid_chequeo_actualiza tr[id=' + spliter[i] + ']').css({ "background-color": "#e8e8f7" });
                        }
                    }
                    alert("No cumple con el requisito documental obligatorio");
                    return true;
                }
            }
        });
        e.preventDefault();
    } catch (ex) {
        alert("Funcion service_validate_chek_list " + ex.message)
    }
}
function retorna_check_lista_check() {
    try {
        var x = document.getElementsByClassName("r_p_check_box");
        ARRAY_CHEK_LIST = new Array();
        for (i = 0; i < x.length; i++) {
            var z = x[i].firstChild;
            if (z.checked == true) {
                ARRAY_CHEK_LIST.push({ text: "0", value: x[i].attributes.getNamedItem("idd").value });
            }
        }
    }
    catch (err) {
        alert(err.message + " Funcion retorna_check_lista_check");
    }
}

function activa_boton_dowload() {
    try {

        document.getElementById("Button_guardar_desicion").click();
    }
    catch (err) {
        alert(err.message + " funcion activa_boton_dowload " + err.message);
    }
}
function preven_event_search_remit_interno(event, e) {
    try {
        document.getElementById("Button_consulta_destinatario_interno").click();
        event.preventDefault();
    }
    catch (err) {
        alert(err.message + " Funcion preven_event_search_remit_interno");
    }
}
function preven_event_search_remit_interno_restore(event, e) {
    try {
        document.getElementById("Button_consulta_destinatario_interno_restore").click();
        event.preventDefault();
    }
    catch (err) {
        alert(err.message + " Funcion preven_event_search_remit_interno_restore");
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
function close_popup_valida() {
    document.getElementById('Button_cerrar_ventana_date').click();
}
function prevent(event, element) {
    try {
        var fer = $(element).attr("idd");
        var fer_id = $(element).attr("id");
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
        if (tip_event == "a_nuevo_radic") {
            $('#hdnEmailID_VAL').val(fer);
            document.getElementById("Button_Asignar_nuevo_radicado").click();
        }
        if (tip_event == "a_asig_dat_expe") {
            $('#hdnEmailID_VAL').val(fer);
            document.getElementById("Button_Asignar_relacionado_expediente").click();
        }
        if (tip_event == "asig_dest_0002") {
            $('#Hidden_auxiliar_id').val(fer);
            document.getElementById("Button_asignar_auxiliar_destinatarios_internos_popup").click();
        }
        if (tip_event == "a_s_r_p_333") {
            if (document.getElementById("IframeDitaliza_").src == "") {
                document.getElementById("IframeDitaliza_").src = "../workflow/WebFormEscan.aspx";
            }
            $('#Hidden_list_registro_rad').val(fer);
            document.getElementById("Button_tool_asigna_radicados_pendientes").click();
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
                    Set_documento_seleccionado(spliter[1], spliter[0]);
                }
            }
            $('#GridView_list_documento_relacion tr[id_rad]').css({ "background": "White", "color": "Black" });
            $('#GridView_list_documento_relacion tr[id_rad=' + ref_id_rad + ']').css({ "background-color": "#e8e8f7", "color": "Black" });
            document.getElementById("Button_tool_visualiza_documento").click();      
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
        //Cammbia tipologia 
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
            if (ITEMS_IMAGE_LIST_WF.length == 0) {
                alert("Debe seleccionar los documentos de la lista");
                return true;
            }
            event_element_menu("C-DW-DEL-IMAGE", "");
           /* var ref_idd = $(element).attr("idd_rad");
            var ref_id_rad = $(element).attr("id_rad");
            $('#hiden_seleccion_documento').val(ref_idd);
            $('#hiden_seleccion_documento_id').val(ref_id_rad);
            var spliter;
            var value;
            var text_content = document.getElementById("hiden_seleccion_documento").value;
            if (text_content != "") {
                spliter = text_content.split("|");
                if (spliter.length > 3) {
                    value = reemplazarAcentos(spliter[4]);
                }
            }
            confirma_eliminar_documento_relacion("Desea elimnar el documento " + value + " ?");
            if (document.getElementById("HiddenPROMP").value == 0) {
                document.getElementById("Button_tool_elimina_documento").click();
            }*/
            
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
        event.preventDefault();
        element.focus();
    }
    catch (err) {
        alert(err.message + " Funcion prevent");
    }
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

function GetLista_posibles_remitentes(name_texbox) {
    function extractLast(term) {
        return term;
    }
    var value_select;
    if (document.getElementById("Area_Destinatario_Cor")) {
        value_select=document.getElementById("Area_Destinatario_Cor").value;
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
                    url: "../webservice/WebServiceRadicacion.asmx/GetLista_posibles_destinatarios",
                    data: "{'DName':'" + document.getElementById(name_texbox).value + "','nombre_area' : '" + value_select + "'}",
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
            }, select: function (event, ui) {
                document.getElementById(name_texbox).value = ui.item.label;
                document.getElementById("Button_consulta_destinatario_interno").click();
            }, minLength: 3, max: 10, scroll: true
        });
}
function auto_zise_popup_lista_radicados() {
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
        var heig_porcent = espacio_iframe - ((espacio_iframe * 10) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_list_registro_rad').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_list_registro_rad').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#contenido_procesa_list_registro_rad').css("height", (document.getElementById("modal_content_list_registro_rad").clientHeight - (document.getElementById("diver_cabcera_list_registro_rad").clientHeight )) + "px");
        //Para los modal que contiene gred
        $('#content_data_grid_list_registro_rad').css("height", (document.getElementById("contenido_procesa_list_registro_rad").clientHeight - (document.getElementById("contenido_titulo_list_registro_rad").clientHeight + 40)) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_lista_radicados " + err.message);
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
        var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_visor_externo').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#Cotenedorpendiente_visor_externo').css("height", (document.getElementById("Panel_visor_externo").clientHeight - (document.getElementById("Cabecerapendiente_visor_externo").clientHeight)) + "px");
        //Para los modal que contiene gred
        $('#Iframe_visor_externo_wf_').css("height", (document.getElementById("Cotenedorpendiente_visor_externo").clientHeight -   3) + "px");
    }
    catch (err) {
        alert(err.message + " Función auto_zise_popup_visor_externo");
    }
}
//AUTO SIZE DE POPUP DESTINATARIOS EXTERNOS
function auto_zise() {
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
        $('#Panel_Val_Radicacion').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        //$('#modal_content_user_rel').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#Diupdate_val_radciacion').css("height", (document.getElementById("Panel_Val_Radicacion").clientHeight - (document.getElementById("divcabecera_val_radicacion").clientHeight)) + "px");
        //Para los modal que contiene gred
        $('#contenido_derecho_validacion_radicados').css("height", (document.getElementById("Diupdate_val_radciacion").clientHeight - 5) + "px");
        $('#contenido_datagrid_val_radicacion').css("height", (document.getElementById("contenido_derecho_validacion_radicados").clientHeight - (document.getElementById("contenido_titulo_val_radicacion").clientHeight + document.getElementById("Contenido_botones_tipo_radicado").clientHeight)) + "px");
        $('#contenido_derecho_validacion_radicados').css("height", (document.getElementById("Diupdate_val_radciacion").clientHeight - 5) + "px");
        $('#contenido_izquierdo_val_radicacion').css("height", (document.getElementById("Diupdate_val_radciacion").clientHeight - 5) + "px");
        $('#contenido_consulta_val_radicacion').css("height", (document.getElementById("contenido_izquierdo_val_radicacion").clientHeight - (document.getElementById("contenido_titulo_campos_consulta").clientHeight + document.getElementById("contenido_botones_val_radicacion").clientHeight)) + "px");
        $('#_Panelvalidacion_val_radicacion').css("height", document.getElementById("contenido_consulta_val_radicacion").clientHeight + "px");       
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise");
    }
}
function auto_zise_popup_dias_habiles() {
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
        $('#Panel_dias_horas_habiles_popup').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        //$('#Contenido_dias_horas_habiles_popup').css("height", (heig_porcent - 5) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#Paneltreview_r_u_e').css("height", (document.getElementById("Panel_dias_horas_habiles_popup").clientHeight - (document.getElementById("divcabecer_dias_horas_habiles_popup").clientHeight + document.getElementById("contendor_botones_unidad_u_b_t").clientHeight)) + "px");
        $('#Contenido_dias_horas_habiles_popup').css("height", (document.getElementById("Panel_dias_horas_habiles_popup").clientHeight - (document.getElementById("divcabecer_dias_horas_habiles_popup").clientHeight + document.getElementById("contendor_botones_unidad_u_b_t").clientHeight)) + "px");
        //Para los modal que contiene gred  
        //$('#content_data_grid').css("height", (document.getElementById("contenido_procesa_usu_rel_solicitud").clientHeight - document.getElementById("contenido_titulo_val_radicacion_documentos").clientHeight) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_dias_habiles " + err.message);
    }
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
        document.getElementById('Area_Visor').style.height = ((document.getElementById("Contenedorderecho").clientHeight - document.getElementById("div_cerrar").clientHeight)) + "px";
        
    }
    catch (err) {
        alert(err.message + " Funcion dispalyVisorEmergente");
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
//ACTIVA VENTANA POPOUP PREVALIDACION RADICADO
function activa_ventan_validacion_radicados() {
    $('#Button_Abrir_Val_Radicacion').click();
  
}
function retorna_check_radicados_gred() {
     try {
        var x = document.getElementsByClassName("jjjjjjjjjjj");
        var ref_hode = document.getElementById("Hidden_selecion_radicado");
        ref_hode.value = "";
        for (i = 0; i < x.length; i++) {
            var z = x[i].firstChild;
            if (z.checked == true) {
                if (x[i].parentNode.parentNode.attributes.length > 0) {
                   var valrad = x[i].parentNode.parentNode.attributes.getNamedItem("id").value;
                   if (ref_hode.value  == "") {
                    ref_hode.value = valrad;
                   }
                    else {
                    ref_hode.value = ref_hode.value + "-" +  valrad;
                   }
                } else {
                    var valrad = x[i].parentNode.parentNode.parentNode.attributes.getNamedItem("id").value;
                    if (ref_hode.value == "") {
                        ref_hode.value = valrad;
                    }
                    else {
                        ref_hode.value = ref_hode.value + "-" + valrad;
                    }
                }
            }
          
        }

}
    catch (err) {
        alert(err.message + " Funcion retorna_check_radicados_gred");
}
}
//FUNCION ACTIVA Y DEACTIVA LOS CAMPOS CHEKEADOS EN UNA TABLA
function desactiva_ch_data_grid(idente_chekbi_actyive) {
    try {
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
    catch (err) {
        alert(err.message + " Funcion desactiva_ch_data_grid");
}
}
function asignar_tipo_flujo() {
    try {
        var drowplist = document.getElementById("RE_flujo_trabajo");
        var idsel = document.getElementById("Hidden_nom_flu");
        if (drowplist.selectedIndex != -1) {
            document.getElementById("Hidden_id_flu").value = drowplist.options[drowplist.selectedIndex].value;
            idsel.value = drowplist.options[drowplist.selectedIndex].text;

        }
    }
    catch (err) {
        alert(err.message + " Funcion asignar_tipo_flujo");
    }
}
function activa_buton_asignar_dest_iterno(idente_chekbi_actyive) {
    
   
}
function activa_dest_externo() {
    try {

        $('#Button_inicia_selecion_validacion').click();
    }
    catch (err) {
        alert(err.message + " Funcion activa_dest_externo");
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
function auto_zise_popup_trace_grafic() {
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

        $('#Paneltraza_grafica').css("height", (espacio_iframe) + "px");
        $('#div_content_trace_grafic').css("height", (espacio_iframe) + "px");
        $('#Iframetraza_grafica_').css("height", (espacio_iframe) + "px");
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_trace_grafic");
    }

}
function auto_zise_popup_lista_usuario_flujo() {
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
        var heig_porcent = espacio_iframe - ((espacio_iframe * 2) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_lista_actividades_worflow_ruta').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_lista_actividades_worflow_ruta').css("height", (heig_porcent - 3) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#contenido_procesa_lista_actividades_workflow').css("height", (document.getElementById("modal_content_lista_actividades_worflow_ruta").clientHeight - (document.getElementById("divcabecer2_lista_actividades_worflow_ruta").clientHeight)) + "px");
        //Para los modal que contiene gred
        $('#div_gred').css("height", (document.getElementById("contenido_procesa_lista_actividades_workflow").clientHeight - (document.getElementById("contenido_titulo_data_grid_dos_title").clientHeight  + document.getElementById("div_contenido_procesa_lista_actividades_worflow_ruta_botones_desicion").clientHeight)) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_lista_usuario_flujo " + err.message);
    }
}
function auto_zise_popup_lista_actividades_ruta() {
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
        var heig_porcent = espacio_iframe - ((espacio_iframe * 2) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_lista_actividades_ruta').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_lista_actividades_ruta').css("height", (heig_porcent - 3) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#contenido_procesa_lista_actividades_ruta').css("height", (document.getElementById("modal_content_lista_actividades_ruta").clientHeight - (document.getElementById("divcabecer2_lista_actividades_ruta").clientHeight)) + "px");
        //Para los modal que contiene gred
        $('#div_gred_actividades').css("height", (document.getElementById("contenido_procesa_lista_actividades_ruta").clientHeight - (document.getElementById("contenido_titulo_data_grid_lista_actividades_ruta").clientHeight + document.getElementById("div_contenido_procesa_lista_actividades_ruta_botones_desicion").clientHeight)) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_lista_actividades_ruta " + err.message);
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
function prevent_envio_actividad_flujo(event, element) {
    try {
        event.preventDefault();
        var fer = $(element).attr("id");
        $('#Hidden_id_actividad_flujo').val(fer);
        var x;
        var r = confirm("Desea enviar la tarea ?");
        if (r == true) {
            document.getElementById("Button_activa_enviar_actividad_flujo_trabajo").click();
        }
        element.focus();
    }
    catch (err) {
        alert(err.message + " Funcion prevent_envio_actividad_flujo");
    }
}
function prevent_detalle(event, element) {
    try {   
        event.preventDefault();
        $('#data_grid tr[id]').css({ "background": "White", "color": "Black" });
        $('#data_grid tr[id]').each(function () {
            $(this).css({ "background-color": "#E7EDF5", "color": "Black" });
        });
        var fer = $(element).attr("id");
        $('#Hidden_id_actividad_flujo').val(fer);
        $('#Hidden_id_flujo_trabjo').val($(element).attr("id_flujo_trabjo"));
        $('#Hidden_id_actividad_destino').val($(element).attr("id_actividad_destino"));
        $('#Hidden_id_usuario_workflow').val($(element).attr("id_usuario_workflow"));
        document.getElementById("Button_detalle_enviar_actividad_flujo_trabajo").click();
        element.focus();
    }
    catch (err) {
        alert(err.message + " Funcion prevent_detalle");
    }
}
function prevent_detalle_actividad(event, element) {
    try {
        event.preventDefault();
        var fer = $(element).attr("id");   
        $('#Hidden_id_actividad_destino').val(fer);
        $('#Hidden_id_usuario_workflow').val(0);
        document.getElementById("Button_detalle_enviar_actividad_flujo_trabajo").click();
        element.focus();
    }
    catch (err) {
        alert(err.message + " Funcion prevent_detalle_actividad");
    }
}
function GetLista_lista_actividades_ruta(name_texbox) {
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
                    url: "../webservice/WebServiceWorkflow.asmx/GetLista_lista_actividades_ruta",
                    data: "{'DName':'" + document.getElementById(name_texbox).value  + "'}",
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
            }, select: function (event, ui) {
                document.getElementById(name_texbox).value = ui.item.label;
                document.getElementById("Button_tool_busqueda_enviar_actividad").click();
            }, minLength: 3, max: 10, scroll: true
        });
}
function preven_event_search_lista_actividad(event, e) {
    try {
        document.getElementById("Button_tool_busqueda_enviar_actividad").click();
        event.preventDefault();
    }
    catch (err) {
        alert(err.message + " Funcion preven_event_search_keypres_enter_lista_actividad");
    }
}

function preven_event_restor_search_lista_actividad(event, e) {
    try {
        document.getElementById("Button_tool_restore_busqueda_enviar_actividad").click();
        event.preventDefault();
    }
    catch (err) {
        alert(err.message + " Funcion preven_event_search_keypres_enter_lista_actividad");
    } finally {

    }
}
function preven_event_search_keypres_enter_lista_actividad(e, sender) {
    try {

        tecla = (document.all) ? e.keyCode : e.which;
        if (tecla == 13) {
            document.getElementById("Button_tool_busqueda_enviar_actividad").click();
            e.preventDefault();
        }
    } catch (err) {
        alert(err.message + " funcion preven_event_search_keypres_enter_lista_actividad " + err.message);
    }
}
function prevent_envio_actividad_tarea(event, element) {
    try {   
        event.preventDefault();
        var fer = $(element).attr("id");
        $('#Hidden_id_tarea').val(fer);
        var x;
        var r = confirm("Desea enviar la tarea ?");
        if (r == true) {
            document.getElementById("Button_tool_enviar_actividad").click();
        }
        element.focus();
    }
    catch (err) {
        alert(err.message + " Funcion prevent_envio_actividad");
    }
}
function auto_zise_popup_lista_usuarios_ruta() {
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
        var heig_porcent = espacio_iframe - ((espacio_iframe * 2) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_lista_usuarios_ruta').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_lista_usuarios_ruta').css("height", (heig_porcent - 3) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#contenido_procesa_lista_usuarios_ruta').css("height", (document.getElementById("modal_content_lista_usuarios_ruta").clientHeight - (document.getElementById("divcabecer2_lista_usuarios_ruta").clientHeight)) + "px");
        //Para los modal que contiene gred
        $('#div_gred_usuarios').css("height", (document.getElementById("contenido_procesa_lista_usuarios_ruta").clientHeight - (document.getElementById("contenido_titulo_data_grid_lista_usuarios_ruta").clientHeight + document.getElementById("div_contenido_procesa_lista_usuarios_ruta_botones_desicion").clientHeight)) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_lista_usuarios_ruta " + err.message);
    }
}
function GetLista_listado_usuarios_workflow_ruta(name_texbox) {
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
                    url: "../webservice/WebServiceWorkflow.asmx/GetLista_listado_usuarios_workflow_ruta",
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
            }, select: function (event, ui) {
                document.getElementById(name_texbox).value = ui.item.label;
                document.getElementById("Button_tool_busqueda_enviar_usuario").click();
            }, minLength: 3, max: 10, scroll: true
        });
}
function preven_event_search_lista_usuario(event, e) {
    try {
        document.getElementById("Button_tool_busqueda_enviar_usuario").click();
        event.preventDefault();
    }
    catch (err) {
        alert(err.message + " Funcion preven_event_search_lista_usuario");
    }
}

function preven_event_restor_search_lista_usuario(event, e) {
    try {
        document.getElementById("Button_tool_restore_busqueda_enviar_usuario").click();
        event.preventDefault();
    }
    catch (err) {
        alert(err.message + " Funcion preven_event_restor_search_lista_usuario");
    } finally {

    }
}
function preven_event_search_keypres_enter_lista_usuario(e, sender) {
    try {

        tecla = (document.all) ? e.keyCode : e.which;
        if (tecla == 13) {
            document.getElementById("Button_tool_busqueda_enviar_usuario").click();
            e.preventDefault();
        }
    } catch (err) {
        alert(err.message + " funcion preven_event_search_keypres_enter_lista_usuario " + err.message);
    }
}
function prevent_detalle_usuario(event, element) {
    try {
        event.preventDefault();
        var fer = $(element).attr("id");
        $('#Hidden_id_usuario_workflow').val(fer);
        document.getElementById("Button_detalle_enviar_actividad_flujo_trabajo").click();
        element.focus();
    }
    catch (err) {
        alert(err.message + " Funcion prevent_detalle_usuario");
    }
}
function prevent_envio_usuario_actividad(event, element) {
    try {
        event.preventDefault();
        var x;
        var r = confirm("Desea enviar la tarea ?");
        if (r == true) {
            $('#Hidden_id_usuario_envio').val($(element).attr("id"));
            $('#Hidden_id_actividad_envio').val($(element).attr("idd"));
            document.getElementById("Button_tool_enviar_usuario").click();
        } else {
            $('#Hidden_id_usuario_envio').val(0);
            $('#Hidden_id_actividad_envio').val(0);
        }
        element.focus();
    }
    catch (err) {
        alert(err.message + " Funcion prevent_envio_actividad");
    }
}
function prevent_envio_ruta_actividad(event, element) {
    try {
        event.preventDefault();
        var x;
        var r = confirm("Desea enviar la tarea ?");
        if (r == true) {
            $('#Hidden_id_actividad_envio').val($(element).attr("id"));
            $('#Hidden_id_actividad_disp_envio').val($(element).attr("idd"));
            document.getElementById("Button_tool_enviar_ruta").click();
        } else {
            $('#Hidden_id_usuario_envio').val(0);
            $('#Hidden_id_actividad_disp_envio').val(0);
        }
        element.focus();
    }
    catch (err) {
        alert(err.message + " Funcion prevent_envio_ruta_actividad");
    }
}
function prevent_terminar_radicado(event, element) {
    try {
        event.preventDefault();
        var x;
        var r = confirm("Desea terminar el radicado ?");
        if (r == true) {
            document.getElementById("Button_tool_termitar_radicado").click();
        }
        element.focus();
    }
    catch (err) {
        alert(err.message + " Funcion prevent_terminar_radicado");
    }
}
//ACTIVA VENTANA POPUP DESTINATARION INTERNO
function activa_ventana_auxiliar_dest_iterno() {
    try {
    var t = $('#Area_Destinatario_Cor')[0].value;
    if ($('#Area_Destinatario_Cor')[0].value != "SELECCIONE" && $('#Area_Destinatario_Cor')[0].value != "") {      
        $('#Button_abrir_auxiliar_destinatarios_internos_popup').click();
    }
}
    catch (err) {
        alert(err.message + " Funcion activa_ventana_auxiliar_dest_iterno");
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
        var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_valiacion_plantilla').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#Contenido_validacion_plantilla').css("height", (document.getElementById("Panel_valiacion_plantilla").clientHeight - (document.getElementById("divcabecer2_validacion_plantilla").clientHeight + 1)) + "px");
        $('#Iframe_validacion_plantilla_').css("height", (document.getElementById("Panel_valiacion_plantilla").clientHeight - (document.getElementById("divcabecer2_validacion_plantilla").clientHeight + 3)) + "px");
       
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_plantilla_validacion");
    }
}
function GetChar(event) {
    try {
    var chCode = ('charCode' in event) ? event.charCode : event.keyCode;
    if (chCode == 13) {

    }
}
    catch (err) {
        alert(err.message + " Funcion GetChar");
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
//FUNCION CONFIGURA POPUP VENTANA GESTION DE EXPEDIENTE
function tamano_ventana_expediente() {
    try {
    //$("#Hiddenheigpaginapopup").val(($("#Panel_expdiente_popup").height() + 60));
   
}
    catch (err) {
        alert(err.message + " Funcion tamano_ventana_expediente");
}
}
function auto_zise_popup_ventana_expediente() {
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
        $('#Panel_expdiente_popup').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_expdiente_popup').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#Contenido_expdiente_popup').css("height", (document.getElementById("modal_content_expdiente_popup").clientHeight - document.getElementById("divcabecer_expdiente_popup").clientHeight) + "px");
        //Para los modal que contiene gred
        //$('#content_data_grid').css("height", (document.getElementById("contenido_procesa_usu_rel_solicitud").clientHeight - document.getElementById("contenido_titulo_val_radicacion_documentos").clientHeight) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_ventana_expediente " + err.message);
    }
}
function onDataShown(sender, args) {
    try {
    sender._popupBehavior._element.style.zIndex = 1000001;
}
    catch (err) {
        alert(err.message + " Funcion onDataShown");
}     
}
function llenardepartamento() {
    try {
    var drowplist = document.getElementById("PAIS");
    var idsel = document.getElementById("Hiddenselecionpais");
    if (drowplist.selectedIndex != -1) {
        idsel.value = drowplist.options[drowplist.selectedIndex].text;
        var boton = document.getElementById("Buttonllenardepartamento");
        var idsel2 = document.getElementById("Hiddenseleciondepartamento");
        idsel2.value = "";
        boton.click();
    }
}
    catch (err) {
        alert(err.message + " Funcion llenardepartamento");
} 
}
function llenarciudad() {
    try {
    var drowplist = document.getElementById("DEPARTEMENTO");
    var idsel = document.getElementById("Hiddenseleciondepartamento");
    if (drowplist.selectedIndex != -1) {
        idsel.value = drowplist.options[drowplist.selectedIndex].text;
        var boton = document.getElementById("Buttonllenarciudad");
        boton.click();
    }
}
    catch (err) {
        alert(err.message + " Funcion llenarciudad");
} 
}

function seleccionmuicipio() {
    try {
    var drowplist = document.getElementById("MUNICIPIO");
    var idsel = document.getElementById("Hiddenmunicipio");
    if (drowplist.selectedIndex != -1) {
        idsel.value = drowplist.options[drowplist.selectedIndex].text;
    }
    }
    catch (err) {
        alert(err.message + " Funcion seleccionmuicipio");
    }
}

function llenardestinatario() {
    try {
    var drowplist = document.getElementById("Area_Destinatario_Cor");
    var idsel = document.getElementById("Hiddenareagestion");
    if (drowplist.selectedIndex != -1) {
        idsel.value = drowplist.options[drowplist.selectedIndex].text;
        var boton = document.getElementById("Buttonllenardestinatario");
        boton.click();
    }
    }
    catch (err) {
        alert(err.message + " Funcion llenardestinatario");
    }
}
function seleccionardestinatario() {
    try {
    var drowplist = document.getElementById("Destinatario_Cor");
    var idsel = document.getElementById("Hiddendestinatario");
    if (drowplist.selectedIndex != -1) {
        idsel.value = drowplist.options[drowplist.selectedIndex].text;
        var boton = document.getElementById("Button_ra_destinatario");
        boton.click();
    }
}
    catch (err) {
        alert(err.message + " Funcion seleccionardestinatario");
}
}
function seleccionardestinatario_evento() {
    try {
    var drowplist = document.getElementById("Destinatario_Cor");
    var idsel = document.getElementById("Hiddendestinatario");
    if (drowplist.selectedIndex != -1) {
        idsel.value = drowplist.options[drowplist.selectedIndex].text;
        //var boton = document.getElementById("Button_ra_destinatario");
        //boton.click();
    }
}
    catch (err) {
        alert(err.message + " Funcion seleccionardestinatario_evento");
}
}

function asignar_validacion() {
    try {
    var boton = document.getElementById("Buttonrefasignar");
        //boton.click();
    }
    catch (err) {
        alert(err.message + " Funcion asignar_validacion");
    }
}
function asignar_fecha_vence_tramite() {
    try {
    var boton = document.getElementById("Buttontramitevence");
    var drowplist = document.getElementById("RE_Descripcion_Documento");
    var idsel = document.getElementById("Hiddentramiteseleccion");
    if (drowplist.selectedIndex != -1) {
        idsel.value = drowplist.options[drowplist.selectedIndex].text;
        document.getElementById("Hiddentramiteseleccionvalue").value = drowplist.options[drowplist.selectedIndex].value;
        boton.click();
    }
}
    catch (err) {
    alert(err.message + " Funcion asignar_validacion");
}
}

//Funcion filtra campos enteros
function key_pres_entero(semilla) {
    try {
    valtexbox = document.getElementById(semilla);
    if (e.charCode < 48 || e.charCode > 57) return false;
}
    catch (err) {
        alert(err.message + " Funcion key_pres_entero");
}
}

function xd() {
  
    if (document.getElementById("hdnEmailID").value == "-1" && document.getElementById("data_grid").rows != undefined) {
        //$('#data_grid tr').css({ "background-color": "Black", "color": "White" });
    }

}
function xdlimpiar(datgrid, hdnemail) {
    try {
  
    if (document.getElementById(hdnemail).value == "-1" && document.getElementById(datgrid).rows != undefined) {
        var _dat_grid = document.getElementById(hdnemail);
      
    }
}
    catch (err) {
        alert(err.message + " Funcion xdlimpiar");
}

}
function ConfirmMensajeEliminar(mensaje) {
    try {
    var x = 1;
    document.getElementById("HiddenPROMP").value = x;
    var t = document.getElementById("hdnEmailID").value;
    if (t != -1) {
        var r = confirm(mensaje);
        if (r == true) {
            x = "0";
        }
        else {
            x = "1";
        }
        document.getElementById("HiddenPROMP").value = x;
    }
}
    catch (err) {
        alert(err.message + " Funcion ConfirmMensajeEliminar");
}
}
function ConfirmMensaje(mensaje) {
    try {
    var x;
    var r = confirm(mensaje);
    if (r == true) {
        x = "0";
    }
    else {
        x = "1";
    }
    document.getElementById("HiddenPROMP").value = x;
}
    catch (err) {
        alert(err.message + " Funcion ConfirmMensaje");
}
}
function xd2() {
    //var t = document.getElementById("hdnEmailID").value();
    //alert(t);
    try {
    var saber = document.getElementById("data_grid");
    if (saber == null) {
        document.getElementById("hdnEmailID").value = "-1"
    }
    if (document.getElementById("hdnEmailID").value != "-1" && saber != null) {
        for (var i = 2; i < document.getElementById("data_grid").rows.length; i++) {
            var id = document.getElementById("hdnEmailID").value;
            if (id == document.getElementById("data_grid").rows[i].cells[0].innerText) {
                for (var j = 0; j < document.getElementById("data_grid").rows[i].cells.length; j++) {
                    var rtr = document.getElementById("data_grid").rows[i].cells[j].style.color = "White";
                    var rtr = document.getElementById("data_grid").rows[i].cells[j].style.background = "Black";

                }
            }
        }
    }
}
    catch (err) {
        alert(err.message + " Funcion  xd2");
}
}

function xd4(datgrid, hdnemail) {
    try {
    var t = document.getElementById(hdnemail).value;
    //alert(t);
    if (document.getElementById(hdnemail).value != "-1" && document.getElementById(datgrid).rows != undefined) {
        for (var i = 2; i < document.getElementById(datgrid).rows.length; i++) {
            var id = document.getElementById(hdnemail).value;

            if (id == document.getElementById(datgrid).rows[i].cells[1].innerText) {
                for (var j = 0; j < document.getElementById(datgrid).rows[i].cells.length; j++) {

                    var rtr = document.getElementById(datgrid).rows[i].cells[j].style.color = "White";
                    var rtr = document.getElementById(datgrid).rows[i].cells[j].style.background = "Black";

                }
            }
        }
    }
}
    catch (err) {
        alert(err.message + " Funcion  xd4");
}
}
function xd5(datgrid, hdnemail) {
    //Lispia los campos seleccionados por la funcion clidred
    try {
    if (document.getElementById(hdnemail).value != "-1" && document.getElementById(datgrid).rows != undefined) {
        for (var i = 2; i < document.getElementById(datgrid).rows.length; i++) {
            for (var j = 0; j < document.getElementById(datgrid).rows[i].cells.length; j++) {
                var rtr = document.getElementById(datgrid).rows[i].cells[j].style.color = "Black";
                var rtr = document.getElementById(datgrid).rows[i].cells[j].style.background = "White";
                //alert("ojo");
            }
        }
    }

}
    catch (err) {
    alert(err.message + " Funcion  xd5");
}
}

function xd3() {
    //Lispia los campos seleccionados por la funcion clidred
    try {
    if (document.getElementById("hdnEmailID").value != "-1") {
        for (var i = 2; i < document.getElementById("data_grid").rows.length; i++) {
            for (var j = 0; j < document.getElementById("data_grid").rows[i].cells.length; j++) {
                var rtr = document.getElementById("data_grid").rows[i].cells[j].style.color = "Black";
                var rtr = document.getElementById("data_grid").rows[i].cells[j].style.background = "White";
                //alert("ojo");
            }
        }
    }
}
    catch (err) {
    alert(err.message + " Funcion  xd5");
}

}


function inactiva_chek() {
    //document.getElementById("hdnEmailID_VAL").value == "-1";
    //xd5("GridView_val_radicacion", "hdnEmailID_VAL");
}


//CAPTURA FILTRADO DESTINATARIOS INTERNOS TECLA BORRAR
function consulta_documentos_busqueda_keycode(e, textarea) {

    if (e.keyCode == 8) {
       // if (textarea.value.length >= 0) {

            //filtro_gred_destinatarios_internos('Hidden_auxiliar_id', 'data_grid_auxiliar_lista', 'TextBoxcontenidobusqueda', 'CheckboxBusqueda', 'panel_data_grid_auxiliar_destinatarios_internos_popup', 'Contenido_auxiliar_destinatarios_internos_popup', 'panel_data_grid_auxiliar_destinatarios_internos_popup');
        //}


    }
}


//AUTO SIZE DESTINATARIOS INTERNOS
function auto_zise_popup_internos() {
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
    $('#Panel_auxiliar_destinatarios_internos_popup').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del moda
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
    $('#Contenido_auxiliar_destinatarios_internos_popup').css("height", (document.getElementById("Panel_auxiliar_destinatarios_internos_popup").clientHeight - (document.getElementById("divcabecer_auxiliar_destinatarios_internos_popup").clientHeight + document.getElementById("contedor_botones_auxiliar_destinatarios_internos_popup").clientHeight + 15)) + "px");    
}
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_internos");
}
}
//AUTO SIZE RADICACION
function auto_resize_radicacion() {
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
    $('#contenguia').css("height", (espacio_iframe - 15) + "px");
    var conten_hei_tab = $("#contenguia").height() - ($("#tab_content_item").height() + $("#tab_content_boton").height());
    $("#tab_content").css("height", (conten_hei_tab - 20) + "px");
    $("#home_radic").css("height", (conten_hei_tab - 20) + "px");
    $("#soporte_envio").css("height", (conten_hei_tab - 20) + "px"); 
    $("#conte_waper").css("height", (($("#soporte_envio").height() - 60)) + "px");
    $("#da_content_wraper").css("height", (($("#soporte_envio").height() - 60)) + "px");
    $("#Contenedorderecho").css("height", (($("#soporte_envio").height() - 60)) + "px");
    $("#Contentizquierdo").css("height", (($("#soporte_envio").height() - 60)) + "px");
    $("#div_treview_archivo").css("height", ($("#Contentizquierdo").height() - 60) + "px");
    $("#Paneltreview").css("height", ($("#Contentizquierdo").height() - 60) + "px");
    document.getElementById('Area_Visor').style.height = ((document.getElementById("Contenedorderecho").clientHeight - document.getElementById("div_cerrar").clientHeight)) + "px";
    
}
    catch (err) {
        alert(err.message + " Funcion auto_resize_radicacion");
}
}
//AUTO SIZE POPUP VALIDACION RADICADOS
function hiden_popup_resize_popup_validacion_radicados() {
    try {
    $("#Diupdate_val_radciacion").hide();
}
    catch (err) {
        alert(err.message + " Funcion hiden_popup_resize_popup_validacion_radicados");
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
    $("#Diupdate_val_radciacion").show();
    
}
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_validacion_radicados");
}
}
function plugin_grwedview() {
    try {
    var gridwith = $('#contenido_datagrid_val_radicacion').width();
    var gridheihg = $('#contenido_datagrid_val_radicacion').height();
    //LLAMA PLUGIN FIJA HIDER O TITULOS   
    //if ($('#GridView_val_radicacion td').children.length > 0) {
        //$(document).ready(function () { $('#GridView_val_radicacion').gridviewScroll({ width: gridwith, height: gridheihg }); })
    //}
}
    catch (err) {
        alert(err.message + " Funcion plugin_grwedview");
}
}
function auto_zise_popup_usuarios_externos() {
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
   
    $('#contenido_general').css("height", (espacio_iframe - 20) + "px");
    $('#contenido_general').css("width", (with_frame - 10) + "px");
    $('#Cosulta_valid').css("width", (with_frame - 15) + "px");
    var heigconetedor = $("#contenido_general").height() - (($("#contenido_general").height() * 60) / 100);
    $("#contenido_consulta").css("height", (heigconetedor) + "px");
    $("#_Panelvalidacion").css("height", (heigconetedor) + "px");
    heigconetedor = $("#contenido_general").height() - (($("#contenido_general").height() * 91) / 100);
    $("#contenido_titulo").css("height", (heigconetedor) + "px");
    heigconetedor = $("#contenido_general").height() - (($("#contenido_general").height() * 60) / 100);
    $("#contenido_datagrid").css("height", (heigconetedor) + "px");
    $("#Cosulta_valid").css("height", (heigconetedor) + "px");
    heigconetedor = $("#contenido_general").height() - (($("#contenido_general").height() * 91) / 100);
    $("#tolbalboton").css("height", (heigconetedor) + "px");

    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_usuarios_externos");
    }
}
//MUEVE EL SCCROL AL ID SELECCIONADO
function mueve_scroll_data_gred(data_grid, HiddenSeleccion) {
    try {
    if ( $("#" + HiddenSeleccion).val() != "-1" ) {
        var scrollableDiv = $("#" + data_grid).parent();
      
        //limpia todos los seleccionados
        $("#" + data_grid + " tr[id]").css({ "background-color": "transparent", "color": "Black" });
        $("#" + data_grid + " tr[id=" + $("#" + HiddenSeleccion).val() + "]").css({ "background-color": "LightSkyBlue", "color": "Black" });
        $("#" + data_grid + " tr[id= " + $("#" + HiddenSeleccion).val() + "]").each(function () {
            $(scrollableDiv).scrollTop(70);
            $(scrollableDiv).scrollTop($(this).offset().top);
            return false;
        });
      
           
    }
}
    catch (err) {
        alert(err.message + " Funcion mueve_scroll_data_gred");
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

function fnExcelTre(control) {
    try {

        var tab = document.getElementById(control); // id of table
        var ficha = document.getElementById(control);
        var ventimp = window.open(' ', 'popimpr');
        ventimp.document.write(ficha.innerHTML);
        ventimp.document.close();
        ventimp.print();
        ventimp.close();
    }
    catch (err) {
        alert(err.message + " Funcion fnExcelReport ");
    }
}
function auto_zise_popup_lista_chequeo_edita(value_lista_general) {
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
        $('#Panel_lista_chequeo_actualiza').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        //$('#modal_content_user_rel').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#contenido_procesa_lista_chequeo_actualiza').css("height", (document.getElementById("Panel_lista_chequeo_actualiza").clientHeight - (document.getElementById("divcabecer2_lista_chequeo_actualiza").clientHeight + document.getElementById("modal-footer_boton_lista_chequeo_actualiza").clientHeight)) + "px");
        //Para los modal que contiene gred
        $('#Contenedorgrid_edita').css("height", (document.getElementById("contenido_procesa_lista_chequeo_actualiza").clientHeight - 3) + "px");
        $('#Panel_principal_actualiza').css("height", (document.getElementById("contenido_procesa_lista_chequeo_actualiza").clientHeight - 4) + "px");
        document.getElementById("Hidden_0003").value = "-1";
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_lista_chequeo_edita " + err.message);
    }
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