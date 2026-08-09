
$(document).ready(function () {
    $.fn.inicio = function () {
        
    }
});

let CONS_ID_SCRIPT_SOLICITANTE = 0;
let CONS_ID_PLANTILLA_RAD = 0;
let CONS_ID_TIPO_PLANTILLA_RAD = 1;
let CONS_UTIL_ESTADO_PENDIENTE = 1;
let CONS_TIPO_PLANTILLA_RAD = "";
let CONST_NOMBRE_PLANTILLA_RAD = "";
let CONST_ID_REGISTRO_ESTADO = 0;
let CONST_STRU_RAD_ASIN = new Array();
let CONST_ID_IMAGEN_RAD = 0;
let CONST_ESTADO_ENVIO_SEND_MAIL = "";
let GroupManagerTomSlect; //---Agrupa las configuraciones 
let _CDeRelacionEstadoRetriccion = [];
//-----------------------ZONA LOAD-------------------------------------------
$(window).on("load", function () {
    try {
        var elment = document.getElementsByClassName("da_event_captive");
        if (elment) {
            for (var i = 0; i < elment.length; i++) {
                elment[i].addEventListener("click", event_click, false);
            }
        }
        ini_event_page();
        window.addEventListener("resize", rezize_event);
        ShowModalPopup("ModalPopupExtender_mensaje_personalizado_backgroundElement", "Panel_mensaje_personalizado", 100009);
        ShowModalPopup("ModalPopupExtender_edition_pro_gres_bar_backgroundElement", "Panel_pro_gres_bar", 100008);
        load_page_radicacion_simplificada(); 
    } catch (e) {
        alert(" funcion load " + e.message);
    }

});
const ini_event_page = () => {
    let array_element = new Array;
    array_element.push({ id: "Boton_event_registro_validacion_externo" }, { id: "Boton_event_actualizacion_validacion_externo" },
        { id: "Button_div_registro_tramite" }, { id: "delete_wodloa_file_rad" }, { id: "Button_cambia_tipologia_documental" }, { id: "save_document_scan" },
        { id: "boton_rad_simpl_send_task_flow" }, { id: "boton_rad_list_task" }, { id: "boton_rad_send_pend" }, { id:"Button_div_limpiaar_campo"}
     );
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
        let result = "";
        let name_espace_class
        delete_alert_boot();
        switch (name_ID) {
            case "Button_div_limpiaar_campo":
                event_element_click_promise(e);
                break;
            //Caso envio a pendiente
            case "boton_rad_send_pend":
                event_element_click_promise(e);
                break;
           //Caso lista tareas pendientes
            case "boton_rad_list_task" :
                event_element_click_promise(e);
                break;
            //Caso guarda documento digitalizado
            case "save_document_scan":
                event_element_click_promise(e);
                break;
            //Caso actualiza tipologia documental
            case "Button_cambia_tipologia_documental":
                event_element_click_promise(e);
                break;
            //Case adjunta archivo
            case "delete_wodloa_file_rad":
                event_element_click_promise(e);
                break;
            //Case auto vincula documentos
            case "boton_rad_simpl_auto_vincula":
                event_element_click_promise(e);
                break;
            //Case adjunta documento
            case "boton_rad_simpl_load_file":
                //inicializa_tipo_adjunto_documento(event, this, 'C-DW-RD')
                //event_element_click_promise(e);
                break;
            //Imprimir rotulo
            case "boton_rad_simpl_printer_rot":
                //"activa_boton_client_server('Button_tool_imprime_rotulo');"
                //event_element_click_promise(e);
                break;
            //Guardar Rotulo
            case "boton_rad_simpl_save_rot":
                //"inicializa_tipo_adjunto_documento(event,this,'I-GD-RD');"
                //event_element_click_promise(e);
                break;
            //Detalle radicado
            case "boton_rad_simpl_detail_rad":
                //"activa_boton_client_server('Button_tool_activa_detalle_radicado');"
                //event_element_click_promise(e);
                break;
            //Notas al radicado
            case "boton_rad_simpl_gestion_not_rad":
                //"activa_boton_client_server('ImageButtonanotacion');"
                //event_element_click_promise(e);
                break;
            //
            //Enviar tarea a usuario
            case "boton_rad_simpl_send_user_task":
                //"activa_boton_client_server('Button_tool_activa_enviar_usuario');"
                //event_element_click_promise(e);
                break;
            //Enviar tarea a grupo
            case "boton_rad_simpl_send_task_gorup":
                //"activa_boton_client_server('Button_tool_activa_enviar_actividad');"
                //event_element_click_promise(e);
                break;
            //Teminar tarea workflow - ruta
            case "boton_rad_simpl_send_task_flow":
                event_element_click_promise(e);
                break;
            //Auto terminar tarea
            case "boton_rad_simpl_send_task_rad_gestion":
                //"activa_boton_client_server('Button_tool_auto_terminar');"
                //event_element_click_promise(e);
                break;
            //Trminar radicación sin gestión worflow
            case "boton_rad_simpl_end_rad":
                //"prevent_terminar_radicado(event,this);"
                //event_element_click_promise(e);
                break;
            //Activa el registro del radicado
            case "Button_div_registro_tramite":
                result = valida_solicita_datos_control_general("content_radicacion_simplificada");
                if (result != "YES") {
                    alert_bot(result, 'warning', "error_div_datos_ingreso");
                } else {
                    event_element_click_promise(e);
                }
                break;
            //Activa el registro del solicitante o tercero
            case "Boton_active_registro_solicitante":
                event_element_click_promise(e);
                break;
            case "Boton_event_registro_validacion_externo": 
                result = valida_solicita_datos_control_general("registro_validacion_externo");
                if (result != "YES") {
                    alert_bot(result, 'warning', "error_content_registro_validacion_externo");
                } else {
                    event_element_click_promise(e);
                }
                break;
            case "Boton_event_actualizacion_validacion_externo":
                result = valida_solicita_datos_control_general("actualizacion_validacion_externo");
                if (result != "YES") {
                    alert_bot(result, 'warning', "error_content_actualizacion_validacion_externo");
                } else {
                    event_element_click_promise(e);
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
        posicion_update_pogres('progres_bar');
        e.currentTarget.disabled = true;
        //delete_wodloa_file_rad
        if (name_control == "Button_div_limpiaar_campo") {
            result = await LimpiarCamposRegistroRadicacion("div_datos_registro_tramite");
            if (result != "YES") {
                alert_bot(result, 'warning', "div_error_content_content_general_rad_simp");
                return true;
            }
        }
        if (name_control == "boton_rad_list_task") {
            result = await Service_REST_Solicita_radicados_pendientes_radicacion(CONS_ID_PLANTILLA_RAD, "table_list_radicados_table","modal_content_lista_radicados_pendientes");
            if (result != "YES") {
                alert_bot(result, 'warning', "div_error_content_content_general_rad_simp");
                return true;
            }
        }
        if (name_control == "boton_rad_simpl_auto_vincula") {
            result = await Service_REST_auto_vincula_documentos_a_expediente_estructura(CONST_STRU_RAD_ASIN[0].ID_TAREA_SELECCIONDA);
            if (result != "YES") {
                alert_bot(result, 'warning', "error_div_soporte_documento");
                return true;
            }
        }
        if (name_control == "delete_wodloa_file_rad") {
            result = await ActivaCargaArchivosRadicacionSimplificada();
            if (result != "YES") {
                alert_bot(result, 'warning', "error_div_soporte_documento");
                return true;
            }
        }
        if (name_control == "boton_rad_simpl_send_task_flow") {   
            result = await Service_REST_solicita_listado_actividades_para_envio_tarea_a_flujo(CONST_STRU_RAD_ASIN[0].RA_RADICADO_REGISTRO,
                CONST_STRU_RAD_ASIN[0].ID_TAREA_SELECCIONDA, "table_send_task_flow_table","modal_content_enviar_tarea_flujo");
            if (result != "YES") {
                alert_bot(result, 'warning', "error_div_soporte_documento");
                return true;
            }
        }
        if (name_control == "Button_div_registro_tramite") { 
            result = await Service_REST_registro_radicacion_simplificada(ITEM_GENERAL_CONTROL_ARRAY, CONST_NOMBRE_PLANTILLA_RAD);
            if (result != "YES") {
                alert_bot(result, 'warning', "error_div_datos_ingreso");
                return true;
            }
            let parameter_Service = new Array();
            parameter_Service.push({
                id_Plantilla: CONS_ID_PLANTILLA_RAD, Nombre_Plantilla_Radicado: CONST_NOMBRE_PLANTILLA_RAD, estado_asignado: "YES",
                Tipo_Plantilla: CONS_TIPO_PLANTILLA_RAD, id_tipo_plantilla: CONS_ID_TIPO_PLANTILLA_RAD,
                util_estado_pendiente_rad: CONS_UTIL_ESTADO_PENDIENTE
            });
            result = await Service_REST_solicita_estructura_estado_radicado_radicacion_simple(parameter_Service, CONST_ID_REGISTRO_ESTADO);
            if (result !== "YES") {
                alert_bot(result, 'warning', "error_div_datos_ingreso");
                return true;
            }
            result = await asigna_gestion_soporte_documental(CONST_STRU_RAD_ASIN)
            if (result !== "YES") {
                alert_bot(result, 'warning', "error_div_datos_ingreso");
                return true;
            }
            result = await LimpiarCamposRegistroRadicacion("div_datos_registro_tramite");
            active_toogle_tab("tab_rad_simple");
            auto_size_soporte_documental(1);
        } 
        if (name_control == "Button_cambia_tipologia_documental") {
            let array_paramenter = new Array();
            let html_drow_list = document.getElementById("option_cambia_tipo_documental");
            let texto_campo = html_drow_list.options[html_drow_list.selectedIndex].text;
            let valor_campo = html_drow_list.options[html_drow_list.selectedIndex].value;
            array_paramenter.push({
                DG_NOMBRE_GABINETE: CONST_STRU_RAD_ASIN[0].DG_NOMBRE_GABINETE, DG_RADICADO: CONST_STRU_RAD_ASIN[0].RA_RADICADO_REGISTRO,
                DG_LISTA_CHEQUEO: CONST_STRU_RAD_ASIN[0].DG_LISTA_CHEQUEO, DG_ID_CONFIG_DIGITALIZACION: CONST_STRU_RAD_ASIN[0].DG_ID_CONFIG_DIGITALIZACION,
                ID_IMAGEN: CONST_ID_IMAGEN_RAD, VALUE_ITEM: valor_campo, TEXT_ITEM: texto_campo})
            result = await Service_REST_actualiza_tipologia_rad_simplificada(array_paramenter, "table_doc_flow_select","modal_cambiar_tipologia_documento");
            if (result != "YES") {
                alert_bot(result, 'warning', "error_cambiar_tipologia_documento");
                return true;
            }
        }
        //Gaurda documento digitalizado 
        if (name_control == "save_document_scan") {
            let array_paramenter = new Array();
            array_paramenter.push({
                DG_NOMBRE_GABINETE: CONST_STRU_RAD_ASIN[0].DG_NOMBRE_GABINETE, DG_RADICADO: CONST_STRU_RAD_ASIN[0].RA_RADICADO_REGISTRO,
                DG_LISTA_CHEQUEO: CONST_STRU_RAD_ASIN[0].DG_LISTA_CHEQUEO, DG_ID_CONFIG_DIGITALIZACION: CONST_STRU_RAD_ASIN[0].DG_ID_CONFIG_DIGITALIZACION,
                ID_IMAGEN: CONST_ID_IMAGEN_RAD, VALUE_ITEM: 0, TEXT_ITEM: "", DG_TIPODIGITALIZACION: CONST_STRU_RAD_ASIN[0].DG_TIPODIGITALIZACION,
                ID_TAREA_SELECCIONDA: CONST_STRU_RAD_ASIN[0].ID_TAREA_SELECCIONDA
            })
            result = await Service_REST_almacenamiento_documentos_digitalizados_rad_simplificada(array_paramenter, "table_doc_flow_select", "");
            if (result !== "YES") {
                alert_bot(result, 'warning', "error_div_soporte_documento");
            }
        }
        if (name_control == "Boton_active_registro_solicitante") {
            let id = e.currentTarget.attributes["atrib_id_escript"].value;
            CONS_ID_SCRIPT_SOLICITANTE = id;
            let parameter_Service = new Array();
            parameter_Service.push({
                id_registro:  0 , class_service: "WebService_plantilla_validacion.asmx", name_service: "Service_solicita_estructura_formulario_registro_validacion_externo",
                name_container: "div_registro_validacion_externo", name_control_padre: "modal_content_registro_validacion_externo",
                asigna_valor: 0, apost_name_content: "div_registro_validacion_externo", add_check: 0, name_table: "", class_name_control: "registro_validacion_externo",
                id_script: id, name_control_tittle: "label_title_registro_validacion_externo", value_control_tittle: "Registrar solicitante", name_space_campo: "registro_validacion_externo"
            });
            result = await Service_REST_activa_gestion_solicitante_plantilla_validacion(parameter_Service);
            if (result != "YES") {
                alert_bot(result, 'warning', "error_div_datos_ingreso");
                return true;
            }
            
        }
        if (name_control == "Boton_event_registro_validacion_externo") {
            result = await Service_REST_registra_tercero_plantilla_validacion_simplificada(ITEM_GENERAL_CONTROL_ARRAY,
                "REMITENTE_COR_content_radicacion_simplificada", "modal_content_registro_validacion_externo", CONS_ID_SCRIPT_SOLICITANTE);
            if (result != "YES") {
                alert_bot(result, 'warning', "error_content_registro_validacion_externo");
                return true;
            }
        }
        if (name_control == "Boton_event_actualizacion_validacion_externo") {
            result = await Service_REST_update_tercero_plantilla_validacion_simplificada(ITEM_GENERAL_CONTROL_ARRAY,
                "REMITENTE_COR_content_radicacion_simplificada", "modal_content_actualizacion_validacion_externo", CONS_ID_SCRIPT_SOLICITANTE);
            if (result != "YES") {
                alert_bot(result, 'warning', "error_content_actualizacion_validacion_externo");
                return true;
            }
        }
        if (name_control == "boton_rad_send_pend") {
            envia_tarea_pendiente_radicado(CONST_ID_REGISTRO_ESTADO);       
        }
    }
    catch (ex) {
        alert_bot(ex.message, 'warning', "div_error_content_content_general_rad_simp");
    } finally {
        progres_hiden('progres_bar');
        document.getElementById(name_control).disabled = false;
    }
}
//----Activa la carga del archivo desde dispositivo---------////
const ActivaCargaArchivosRadicacionSimplificada = async () => {
    try {
        let Rest = await ServiceRESTSolicitaListaTramiteAutoVinculacionGabinete(CONST_STRU_RAD_ASIN[0].DG_ID_TRAMITE);
        if (Rest.error) {
            return Rest.message;
        }
        let _OPtionFileLoad = ({
            NameLoadProceso: "ADJUNTARADICACION",
            NameContenedorError: "error_content_adjunta_documeto_load_documento_006",
            funcion_name: "adjunta_documeto_version_document", evento_adjunta: "ADJUNTARADICACION",
            IdRespuestaIdExpediente: 0,
            NameContendorLoadDocumento: "div_soporte_documento", ModalWidth: 75, CargaTipologia: 1,
            CargaFecha: 1, CargaPreview: 1, multi_select: "multiple",
            element_parent: "modal_adjunta_documeto_load_documento_006", TipoFormulario: 1,
            name_serivce_list: "service_source_list_item_control_general_documento_radicado",
            name_class_serivce_list: "WebServiceRadicacion.asmx",
            element_html_table: "table_doc_flow_select", element_html_lab_conteo: "Label_documentos", apost_html_lab_conteo: "Documentos",
            setioption_obliga_tipologia: Rest.interface_config_digitaliza.Obliga_Lista_Chequeo
        });
        let result = await IniLoadPerson(_OPtionFileLoad);
        return result;  
    } catch (ex) {
        return "Inconsistencia carga de archivo error " + ex.mensaje;
    }
}

//---------activa firmado digital andes----////////
const stamp_file_doument = async (id_imgen) => {
    try {
        let result = "";
        delete_alert_boot();
        posicion_update_pogres('progres_bar');
        let Option = {
            id_imagen: id_imgen, name_gabinete: CONST_STRU_RAD_ASIN[0].DG_NOMBRE_GABINETE, module: "1",
            valida_firma: "1", name_table: "table_doc_flow_select", name_campo_estado_firma: "ESTADO_FIRMA_DIGITAL",
            name_tipo_table: "bootstrap", name_element_table_aspnet: ""
        };
        result = await LoadStampFile(Option);
        if (result != "YES") {
            alert_bot(result, 'warning', "error_div_soporte_documento");
            return true;
        }
       
    }
    catch (ex) {
        alert_bot(ex.message, 'warning', "error_div_soporte_documento");
} finally {
    progres_hiden('progres_bar');
}
}
const active_update_solicitante_validacion = async (id_script, id_registro) => {
    try {
        let result = "";
        CONS_ID_SCRIPT_SOLICITANTE = id_script;
        delete_alert_boot();
        posicion_update_pogres('progres_bar');
        let parameter_Service = new Array();
        parameter_Service.push({
            id_registro: id_registro, class_service: "WebService_plantilla_validacion.asmx", name_service: "Service_solicita_estructura_formulario_registro_validacion_externo_id",
            name_container: "div_actualizacion_validacion_externo", name_control_padre: "modal_content_actualizacion_validacion_externo",
            asigna_valor: 1, apost_name_content: "div_actualizacion_validacion_externo", add_check: 0, name_table: "", class_name_control: "actualizacion_validacion_externo",
            id_script: id_script, name_control_tittle: "label_title_actualizacion_validacion_externo", value_control_tittle: "Actualizar solicitante", name_space_campo: "actualizacion_validacion_externo"
        });
        result = await Service_REST_activa_gestion_solicitante_plantilla_validacion(parameter_Service);
        if (result != "YES") {
            alert_bot(result, 'warning', "error_div_datos_ingreso");
            return true;
        }
        
    }
    catch (ex) {
        alert_bot(ex.message, 'warning', "error_div_datos_ingreso");
    } finally {
        progres_hiden('progres_bar');   
    }
}
//-----Elimina tercero solicitante de validación
const delete_solicitante_validacion = async (id_script, id_registro) => {
    try {
        let result = "";
        CONS_ID_SCRIPT_SOLICITANTE = id_script;
        delete_alert_boot();
        posicion_update_pogres('progres_bar');
        result = await Service_REST_delete_tercero_plantilla_validacion_simplificada(id_registro,
            "REMITENTE_COR_content_radicacion_simplificada",  CONS_ID_SCRIPT_SOLICITANTE);
        if (result != "YES") {
            alert_bot(result, 'warning', "error_div_datos_ingreso");
            return true;
        }
        
    }
    catch (ex) {
    alert_bot(ex.message, 'warning', "error_div_datos_ingreso");
    } finally {
    progres_hiden('progres_bar');

    }
}
function prevent_event(event, element) {
    try {
        let id_registro = $(element).attr("idd");
        let id_escript = $(element).attr("id_escript");
        let tip_event = $(element).attr("tip_event");
        event.stopImmediatePropagation();
        if (tip_event == "delete_item") {
            var r = confirm("Desea eliminar el registro del solicitante");
            if (r == false) {       
                //event.stopImmediatePropagation();
                return false;
            }
            delete_solicitante_validacion(id_escript, id_registro);
        }
        if (tip_event == "edit_item") {    
            active_update_solicitante_validacion(id_escript, id_registro);
        }
        //event.stopImmediatePropagation();
        
    }
    catch (err) {
        alert(err.message + " Funcion prevent_event");
    }
}
//--------Iniciaiza todas las interfaz de inicicio de la radicación simple
const load_page_radicacion_simplificada = async () => {
    try {
        posicion_update_pogres('progres_bar');
        let result = "";
        result = await Service_REST_Inicializa_cliente_workflow_radicacion_simple(0);
        if (result !== "YES") {
            alert_bot(result, 'warning', "div_error_content_content_general_rad_simp");
            return true;
        }
        result = await load_interface_radicacion_simplificada();
        if (result !== "YES") {
            alert_bot(result, 'warning', "div_error_content_content_general_rad_simp");
            return true;
        }
        result = await Service_REST_solicita_nombre_plantilla_radicacion_simplificada("");
        if (result !== "YES") {
            alert_bot(result, 'warning', "div_error_content_content_general_rad_simp");
            return true;
        }
        result = await Service_REST_Solicita_solicita_numero_radicados_pendientes(CONS_ID_PLANTILLA_RAD);
        if (result !== "YES") {
            alert_bot(result, 'warning', "div_error_content_content_general_rad_simp");
            return true;
        }
        result = await Service_REST_Solicita_radicado_existencia_radicado_asignado(CONS_ID_PLANTILLA_RAD, CONS_ID_TIPO_PLANTILLA_RAD);
        if (result !== "YES") {
            alert_bot(result, 'warning', "div_error_content_content_general_rad_simp");
            return true;
        }
        if (CONST_ID_REGISTRO_ESTADO !== 0) {  
            let parameter_Service = new Array();
            parameter_Service.push({
                id_Plantilla: CONS_ID_PLANTILLA_RAD, Nombre_Plantilla_Radicado: CONST_NOMBRE_PLANTILLA_RAD, estado_asignado: "YES",
                Tipo_Plantilla: CONS_TIPO_PLANTILLA_RAD, id_tipo_plantilla: CONS_ID_TIPO_PLANTILLA_RAD,
                util_estado_pendiente_rad: CONS_UTIL_ESTADO_PENDIENTE
            });
            result = await Service_REST_solicita_estructura_estado_radicado_radicacion_simple(parameter_Service, CONST_ID_REGISTRO_ESTADO);
            if (result !== "YES") {
                alert_bot(result, 'warning', "div_error_content_content_general_rad_simp");
                return true;
            }
            result = await asigna_gestion_soporte_documental(CONST_STRU_RAD_ASIN)
            if (result !== "YES") {
                alert_bot(result, 'warning', "div_error_content_content_general_rad_simp");
                return true;
            }
            document.getElementById("home_soporte_documento").classList.add("active");
            auto_size_soporte_documental(1);
        } else {
            result = await Service_REST_Solicita_solicita_opciones_plantilla_radicacion(CONS_ID_PLANTILLA_RAD);
            if (result !== "YES") {
                alert_bot(result, 'warning', "div_error_content_content_general_rad_simp");
                return true;
            } 
            document.getElementById("home_datos_registro_tramite").classList.add("active");
            auto_size_soporte_documental(1);
        }   
    } catch (ex) {
        alert_bot(ex.message, 'warning', "div_error_content_content_general_rad_simp");
    } finally {
        progres_hiden('progres_bar');
    }
}
const active_toogle_tab = (name_class) => {
    let class_array_tab = document.getElementsByClassName(name_class);
    for (i = 0; i < class_array_tab.length; i++) {
        class_array_tab[i].classList.toggle("active");
    }
}
//------inicializa interface registro
const load_interface_radicacion_simplificada = async () => {
    let myPromise = new Promise(function (resolve) {
        try {
            let result;
            let parameter_Service = new Array();
            parameter_Service.push({
                id_registro: 0, class_service: "WebService_radicacion_Simplificada.asmx", name_service: "Service_solicita_estructura_radicacion_simplificada",
                name_container: "content_radicacion_simplificada", name_control_padre: "",
                asigna_valor: 1, apost_name_content: "content_radicacion_simplificada", add_check: 0, name_table: "", class_name_control: "content_radicacion_simplificada"
            });
            result =  Service_REST_solicita_estructura_radicacion_simplificada(parameter_Service);
            resolve(result);
        }
        catch (ex) {
            resolve(ex.message);
        }
    })
    let result = await myPromise;
    return result;
}
//-------------------ZONA EVENTOS TABLE BOOT---------------------------
function operateFormattertablebootmig(value, row, index) {  
    let element_atrit = row.ID;
    let selecion = "rad"
    let date_campo = row.ID;
    let IconoFle ="fal fa-file";
    if (row.ESTADO_FIRMA_DIGITAL == 1) {
        IconoFle = 'far fa-file-certificate';
    } else {
       let IconoPrev = TableAgregaIconoAwesonGabinete(row.DBT);
       IconoFle = 'far ' + IconoPrev;
    }
    return [
        '<div class="row pl-1 w-100" style="display:inline-flex; cursor:pointer">',
        '<div class="w-100 col-10 pl-2 row " style="margin-right:1px;" title="Ver documento"  tip_event="vis_doc_selecion_rad">',
        '<div class="col-2 pt-2 vis_doc_selecion_rad">',
        '<a  class=" font-weight-light" style="color: #0062cc;">',
        '<i class="' + IconoFle + '" style="color:#0062cc" id=' + row.ID + '></i>',
        '</a></div>',
        '<div class="col-9 pl-1 pt-2 vis_doc_selecion_rad">',
        '<spam class="pl-0 GridviewSpanOverFlow" style="color:black;">' + row.TIPODOCUMENTO + ' </spam>',
        '</div>',
        '</div>',
        '<div class="col-2 p-0 nav-item dropdown active">',
        '<a class="nav-link dropdown-toggle justify-content-start btn-lg mt-1" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false" href="#"></a>',
        '<div class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink">',
        '<a  class="dropdown-item pl-3 font-weight-light delete_file_document" ',
        ' class="dropdown-item font-weight-light" style="margin-left:1px; aria-hidden="true" focusable="false" data-prefix="fal" data-icon="trash-alt" role="img" > <i style="color: #0062cc;" class="fal fa-trash-alt"></i> <spam class="pl-1 font-weight-light"> Eliminar documento</spam></a>',
        '<a  class="dropdown-item pl-3 font-weight-light change_tipo_document"  title="Cambiar tipología documental"',
        'class="dropdown-item font-weight-light" style="margin-left:1px; aria-hidden="true" focusable="false" data-prefix="fal" data-icon="file-edit" role="img" > <i style="color: #0062cc;" class="fal fa-file-edit"></i> <spam class="pl-1 font-weight-light"> Cambiar tipología</spam></a>',
        '<a  class="dropdown-item pl-3 font-weight-light stamp_file_document"  title="Firmar documento"',
        'class="dropdown-item font-weight-light" style="margin-left:1px; aria-hidden="true" focusable="false" > <i style="color: #0062cc;" class="fal fa-file-signature"></i> <spam class="pl-1 font-weight-light"> Firma digital</spam></a>',
        '<a  class="dropdown-item pl-3 font-weight-light list_version_document"  title="Versiones del documento"',
        'class="dropdown-item font-weight-light" style="margin-left:1px; aria-hidden="true" focusable="false" > <i style="color: #0062cc;" class="fal fa-folder-open"></i> <spam class="pl-1 font-weight-light"> Versiones del documento</spam></a>',
        '<a  class="dropdown-item pl-3 font-weight-light replace_version_document"  title="Remplazar documento"',
        'class="dropdown-item font-weight-light" style="margin-left:1px; aria-hidden="true" focusable="false" > <i style="color: #0062cc;" class="fal fa-clone"></i> <spam class="pl-1 font-weight-light"> Remplazar documento</spam></a>',
        '</div>',
        '</div>',
        '</div>',
    ].join('')
}
window.operateEvents = {
    'click .vis_doc_selecion_rad': (e, value, row, index) => {
        delete_alert_boot();
        let ident = table_boot_return_objet_jonson(row);
        let parameter_Service = new Array();
        parameter_Service.push({
            id_imagen: ident.ID, gabinete: CONST_STRU_RAD_ASIN[0].DG_NOMBRE_GABINETE, radicado: CONST_STRU_RAD_ASIN[0].RA_RADICADO_REGISTRO,
            id_tarea_workflow: CONST_STRU_RAD_ASIN[0].ID_TAREA_SELECCIONDA
        });
        show_visualiza_documento_visor_rad_simple(parameter_Service);

    },
    'click .change_tipo_document': (e, value, row, index) => {
        delete_alert_boot();
        let ident = table_boot_return_objet_jonson(row);
        CONST_ID_IMAGEN_RAD = ident.ID;
        show_activa_cambio_tipo_documental_rad_simple("option_cambia_tipo_documental","modal_cambiar_tipologia_documento");
    }, 'click .delete_file_document': (e, value, row, index) => {
        delete_alert_boot();
        let ident = table_boot_return_objet_jonson(row);
        let confi = confirm("¿Desea eliminar el documento (" + ident.TIPODOCUMENTO + ")?");
        if (confi == true) {
            delete_file_documento_rad_simple(ident.ID,"table_doc_flow_select");
        }
    }, 'click .stamp_file_document': (e, value, row, index) => {
        delete_alert_boot();
        let ident = table_boot_return_objet_jonson(row);
        let confi = confirm("¿Desea firmar el documento (" + ident.TIPODOCUMENTO + ")?");
        if (confi == true) {
            stamp_file_doument(ident.ID);
        }
    }, 'click .list_version_document': (e, value, row, index) => {
        delete_alert_boot();
        let ident = table_boot_return_objet_jonson(row);
        let option = ({
            IdImagen: ident.ID,
            Gabinete: CONST_STRU_RAD_ASIN[0].DG_NOMBRE_GABINETE,
            TipoModulo: 5,
            ContentError: "div_error_content_wf",
            name_class_element_icono_aspnet: "",
            DocumentoTilte: ident.TIPODOCUMENTO,
            NameModulo: "RADICACION",
            NameControlParent: "div_content_general_radicacion_simpleficada",
            TipoTable: "bootstrap",
            NameTable: "table_doc_flow_select",
            NameCampo: "ESTADO_FIRMA_DIGITAL",
            NameCampoId:"ID"
        });
        ShowListVersionDocumento(option);
    },'click .replace_version_document': (e, value, row, index) => {
        delete_alert_boot();
        let ident = table_boot_return_objet_jonson(row);
        let option = ({
            IdImagen: ident.ID,
            Gabinete: CONST_STRU_RAD_ASIN[0].DG_NOMBRE_GABINETE,
            TipoModulo: 5,
            ContentError: "div_error_content_wf",
            name_class_element_icono_aspnet: "",
            DocumentoTilte: ident.TIPODOCUMENTO,
            NameModulo: "RADICACION",
            NameControlParent: "div_content_general_radicacion_simpleficada",
            TipoTable: "bootstrap",
            NameTable: "table_doc_flow_select",
            NameCampo: "ESTADO_FIRMA_DIGITAL",
            NameCampoId: "ID"
        });
        ShowActivaOpcionRemplazo(option);
    }
}
window.operateEventsSEND = {
    'click .send_task_flow': (e, value, row, index) => {
        delete_alert_boot();
        let ident = table_boot_return_objet_jonson(row);
        send_envio_tarea_flujo_trabajo(ident.ID_REGISTRO_ACTIVIDAD_ENVIO);
    }
}
function operateFormattertablebootSEND(value, row, index) {
    let IconoFle = "fas fa-share";
    return [
        '<div class="row pl-1 w-100" style="display:inline-flex; cursor:pointer">',
        '<div class="w-100 col-12 " title="Asignar">',
        '<div class="col-12  send_task_flow">',
        '<a  class=" font-weight-light" style="color: #0062cc;">',
        '<i class="' + IconoFle + '" ></i>',
        '</a></div>',
        '</div>',
        '</div>'
    ].join('')
}
window.operateEventsEstado = {
    'click .asing_task_radic': (e, value, row, index) => {
        delete_alert_boot();
        let ident = table_boot_return_objet_jonson(row);
        asigna_tarea_pendiente_radicado(ident.id_tarea_wf, ident.id_estado_radicado);
    }
}
function operateFormattertablebootEstado(value, row, index) {
    let IconoFle = "fas fa-arrow-down";
    return [
        '<div class="row pl-1 w-100" style="display:inline-flex; cursor:pointer">',
        '<div class="w-100 col-12 " title="Asignar radicado para envio">',
        '<div class="col-12  asing_task_radic">',
        '<a  class=" font-weight-light" style="color: #0062cc;">',
        '<i class="' + IconoFle + '" ></i>',
        '</a></div>',
        '</div>',
        '</div>'
    ].join('')
}
//-----Envia radicado a pendiente desde el mudulo de gestión de documentos----///
const envia_tarea_pendiente_radicado = async (id_registro_estado) => {
    //let myPromise = new Promise(function (resolve) {
    try {  
        let result = "";
        if (id_registro_estado == 0) {
            return "YES";
        }
        result = await Service_REST_actualiza_estado_registro_radicado_pendiente(id_registro_estado, 1);
        if (result !== "YES") {
            alert_bot(result, 'warning', "div_error_content_content_general_rad_simp");
            return true;
        }
        result = await Service_REST_solicita_estructura_estado_radicado_radicacion_simple_vacia(0);
        if (result !== "YES") {    
            alert_bot(result, 'warning', "div_error_content_content_general_rad_simp");
            return true;
        }
        active_toogle_tab("tab_rad_simple");
        result = await eliminar_gestion_soporte_documental(CONST_STRU_RAD_ASIN);
        if (result !== "YES") {  
            alert_bot(result, 'warning', "div_error_content_content_general_rad_simp");
            return true;
        }
        result = await Service_REST_Solicita_solicita_numero_radicados_pendientes(CONS_ID_PLANTILLA_RAD);
        if (result !== "YES") {
            alert_bot(result, 'warning', "div_error_content_content_general_rad_simp");
            return true;
        }
        progres_hiden('progres_bar');
        return result;
    }
    catch (ex) {
        alert_bot(ex.message, 'warning', "div_error_content_content_general_rad_simp");
    } finally {
        
    }
}
//-----Asigna radicado para gestión de documentos desde pendiente---///
const asigna_tarea_pendiente_radicado = async (id_tarea_workflow, id_registro_estado) => {
    try {
        posicion_update_pogres('progres_bar');
        let result = "";
        result = await Service_REST_solicita_estado_radicado_asignado_usuario_gestion_documentos(CONS_ID_PLANTILLA_RAD);
        if (result !== "YES") {
            alert_bot(result, 'warning', "error_content_lista_radicados_pendientes");
            return true;
        }
        if (id_tarea_workflow == 0) {
            result = await Service_REST_registra_flujo_tarea_workflow_radicado_simple(id_registro_estado);
            if (result !== "YES") {
                alert_bot(result, 'warning', "error_content_lista_radicados_pendientes");
                return true;
            } 
        } else {
            result = await Service_REST_actualiza_estado_registro_radicado_pendiente(id_registro_estado, 0);
            if (result !== "YES") {
                alert_bot(result, 'warning', "error_content_lista_radicados_pendientes");
                return true;
            }
        }   
        let parameter_Service = new Array();
        parameter_Service.push({
            id_Plantilla: CONS_ID_PLANTILLA_RAD, Nombre_Plantilla_Radicado: CONST_NOMBRE_PLANTILLA_RAD, estado_asignado: "YES",
            Tipo_Plantilla: CONS_TIPO_PLANTILLA_RAD, id_tipo_plantilla: CONS_ID_TIPO_PLANTILLA_RAD,
            util_estado_pendiente_rad: CONS_UTIL_ESTADO_PENDIENTE
        });
        result = await Service_REST_solicita_estructura_estado_radicado_radicacion_simple(parameter_Service, id_registro_estado);
        if (result !== "YES") {
            alert_bot(result, 'warning', "error_content_lista_radicados_pendientes");
            return true;
        }
        result = await asigna_gestion_soporte_documental(CONST_STRU_RAD_ASIN)
        if (result !== "YES") {
            alert_bot(result, 'warning', "error_content_lista_radicados_pendientes");
            return true;
        }
        result = await Service_REST_Solicita_solicita_numero_radicados_pendientes(CONS_ID_PLANTILLA_RAD);
        if (result !== "YES") {
            alert_bot(result, 'warning', "error_content_lista_radicados_pendientes");
            return true;
        }
        let name_control_padre = "modal_content_lista_radicados_pendientes";
        $("#" + name_control_padre).modal("hide");
        active_toogle_tab("tab_rad_simple");
        auto_size_soporte_documental(1);
    } catch (ex) {
        alert_bot(ex.message, 'warning', "error_content_lista_radicados_pendientes");
    } finally {
        progres_hiden('progres_bar');
    }
}

const send_envio_tarea_flujo_trabajo = async (identi_actividad_flujo_destino) => {
    try {    
        posicion_update_pogres('progres_bar');
        let result = "";
        result = await Service_REST_enviar_tarea_flujo_trabajo_radicacion_simple(identi_actividad_flujo_destino,
            CONST_STRU_RAD_ASIN[0].ID_TAREA_SELECCIONDA, "modal_content_enviar_tarea_flujo");
        if (result !== "YES") {
            alert_bot(result, 'warning', "error_content_enviar_tarea_flujo");
            return true;
        }
        result = await Service_REST_solicita_estructura_estado_radicado_radicacion_simple_vacia(0);
        if (result !== "YES") {
            alert_bot(result, 'warning', "error_content_enviar_tarea_flujo");
            return true;
        }
        result = await eliminar_gestion_soporte_documental(CONST_STRU_RAD_ASIN);
        if (result !== "YES") {
            alert_bot(result, 'warning', "error_content_enviar_tarea_flujo");
            return true;
        }
        active_toogle_tab("tab_rad_simple");
        $("#modal_content_enviar_tarea_flujo").modal("hide");
       
    } catch (ex) {
        alert_bot(ex.message, 'warning', "error_content_enviar_tarea_flujo");
    } finally {
        progres_hiden('progres_bar');
    }
}
//----Elimina documento radicado de las lista de documentos
const delete_file_documento_rad_simple = async (id_imagen, name_table) => {
    try {
        posicion_update_pogres('progres_bar');
        let result = "";
        result = await Service_REST_elimina_documento_enlace_radicado(id_imagen, name_table);
        if (result !== "YES") {
            alert_bot(result, 'warning', "error_div_soporte_documento");
            return true;
        }
    } catch (ex) {
        alert_bot(ex.message, 'warning', "error_div_soporte_documento");
    } finally {
        progres_hiden('progres_bar');
    }
}
//-----Activa la visualización de documentos
const show_visualiza_documento_visor_rad_simple = async (parameter) => {
    try {
        posicion_update_pogres('progres_bar');
        let result = "";
        result = await Service_REST_olicita_url_documento_soporte_documental_rad_simple(parameter);
        if (result !== "YES") {
            alert_bot(result, 'warning', "error_div_soporte_documento");
            return true;
        }
    } catch (ex) {
        alert_bot(ex.message, 'warning', "error_div_soporte_documento");
    } finally {
    progres_hiden('progres_bar');
    }
}
//-----Activa la ventana de actualización de tipo documental
const show_activa_cambio_tipo_documental_rad_simple = async (name_control, name_control_padre) => {
    try {
        posicion_update_pogres('progres_bar');
        let result = "";
        result = await Service_REST_source_list_tipos_documentales_radicacion_simple(name_control, name_control_padre);
        if (result !== "YES") {
            alert_bot(result, 'warning', "error_div_soporte_documento");
            return true;
        }
    } catch (ex) {
        alert_bot(ex.message, 'warning', "error_div_soporte_documento");
    } finally {
        progres_hiden('progres_bar');
    }
}
//-------------------TERMINA EVENTOS TABLE BOOT---------------------------
//---------------------Zona controles-----------------------------//
const Create_interface_radicacion_simplificada =  (name_control_padre, class_name_form_control, asigna_valor, apost_name_content, add_check) => {
    try {
        const ConfigsTom = [];
        //----------Limpia los controles anidados en los tab
        for (var i = 0; i < ITEM_GENERAL_CONTROL_ARRAY_ASING.length; i++) {
            let name_padre_control = document.getElementById(ITEM_GENERAL_CONTROL_ARRAY_ASING[i].name_tab_control);
            if (name_padre_control !== null && name_padre_control !== "") {
                //limpia los campos anteriores
                while (name_padre_control.hasChildNodes()) {
                    name_padre_control.removeChild(name_padre_control.firstChild);
                }
            }
        }
        //----------Agrega el evento de eliminar alerta
        let div_content_parent = document.getElementById("div_datos_registro_tramite");
        if (div_content_parent) {
            div_content_parent.addEventListener("click", delete_alert_boot, false);
        }
        for (var i = 0; i < ITEM_GENERAL_CONTROL_ARRAY_ASING.length; i++) {
            //Add campo DIV ROW
            divtml = document.createElement("div");
            divtml.classList.add("row");
            let name_padre_control = document.getElementById(ITEM_GENERAL_CONTROL_ARRAY_ASING[i].name_tab_control);  
            name_padre_control.appendChild(divtml);
            //Add campo DIV COLUMNA
            var divtml_ = document.createElement("div");
            if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].type_cells_alow_control == "group") {
                divtml_.classList.add("btn-group");
                divtml_.classList.add("col-12");
                divtml.classList.add("p-1");
                divtml.classList.add("pt-3");
            } else {
                divtml.classList.add("p-1");
                divtml_.classList.add("col-6");
            }
            //Add control chkek si eta actviva la function
            if (add_check == 1) {
                let imputhml = document.createElement("INPUT");
                imputhml.setAttribute("type", "checkbox");
                imputhml.setAttribute("atrib_campo_n", "chek_" + ITEM_GENERAL_CONTROL_ARRAY_ASING[i].name_campo);
                imputhml.classList.add("chek_" + apost_name_content);
                imputhml.id = "chek_item_" + ITEM_GENERAL_CONTROL_ARRAY_ASING[i].name_campo + "_" + class_name_form_control;
                imputhml.classList.add("mr-1");
                divtml_.appendChild(imputhml);
            }
            //Agrega el control SPAN  
            var spntml = document.createElement("span");
            spntml.classList.add("h6");
            //Agrega la clase agrega el formato de tonalidad del control label
            if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].label_input_class_font !== "" && ITEM_GENERAL_CONTROL_ARRAY_ASING[i].label_input_class_font !== null) {
                spntml.classList.add(ITEM_GENERAL_CONTROL_ARRAY_ASING[i].label_input_class_font);
            } else {
                spntml.classList.add("font-weight-light");
            }

            let string_ = ITEM_GENERAL_CONTROL_ARRAY_ASING[i].aleas_campo;
            var estado_obligatorio;
            if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].obligatorio_campo == "1") {
                estado_obligatorio = " *";
                //spntml.classList.add("text-danger");
            } else {
                //spntml.classList.add("text-dark");
                estado_obligatorio = " ";
            }
            //Agrega la clase que acerca el control label al control padre
            if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].type_cells_alow_control == "group") {
                spntml.classList.add("control-label");
            }
            if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].Tupcae_label == "MALL") {
                spntml.innerHTML = string_.toUpperCase() + estado_obligatorio;
            } else {
                spntml.innerHTML = string_ + estado_obligatorio;
            }
            divtml_.appendChild(spntml);
            //Agrega el control popou ayuda popup
            if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].tooltipAyuda !== null && ITEM_GENERAL_CONTROL_ARRAY_ASING[i].tooltipAyuda != "") {
                let itml = document.createElement("i");
                itml.classList.add("fal");
                itml.classList.add("fa-info-circle");
                itml.classList.add("ml-1");
                let atml = document.createElement("a");
                atml.setAttribute("data-bs-toggle", "tooltip");
                atml.setAttribute("data-bs-placement", "top");
                atml.setAttribute("title", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].tooltipAyuda);
                atml.appendChild(itml);
                divtml_.appendChild(atml);
            }
            //----Agrega el boton para agregar el solicitante o remitente
            if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].id_escript !== null && ITEM_GENERAL_CONTROL_ARRAY_ASING[i].id_escript !== 0) {
                let itml = document.createElement("i");
                itml.classList.add("fal");
                itml.classList.add("fas");
                itml.classList.add("fa-user-plus");
                itml.classList.add("ml-2");
                let atml = document.createElement("a");
                atml.setAttribute("data-bs-toggle", "tooltip");
                atml.setAttribute("data-bs-placement", "top");
                atml.setAttribute("title", "Registrar solicitante");
                atml.classList.add("form-control-cursor-person");
                atml.id = "Boton_active_registro_solicitante"
                atml.setAttribute("atrib_id_escript", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].id_escript);
                atml.addEventListener("click", handler_element_event, false);
                atml.appendChild(itml);  
                divtml_.appendChild(atml);
            }
            divtml.appendChild(divtml_);
            //Add campo DIV COLUMNA PARA CAMPO IMPUT/OPTION/TEXTAREA   
            divtml_ = document.createElement("div");
            if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].type_cells_alow_control == "group") {
                //---Agrega el nuevo row para los campos group
                divtml_.classList.add("btn-group");
                divtml_.classList.add("col-12");
                divtml = document.createElement("div");
                divtml.classList.add("row");
                divtml.classList.add("mt-0");
                name_padre_control.appendChild(divtml);
            } else {
                divtml_.classList.add("col-6");
            }
            //Agrega campo IMPUT/OPTION/TEXTAREA/TEXTSELCTTOM
            var imputhml;
            if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].campo_tip == 1) {
                imputhml = document.createElement("INPUT");
                if (asigna_valor == 1) {
                    imputhml.value = ITEM_GENERAL_CONTROL_ARRAY_ASING[i].value_campo;
                }
            }
            if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].campo_tip == 15) {
                imputhml = document.createElement("INPUT");
                imputhml.classList.add("form-control");
                imputhml.classList.add("form-control-borde-none");         
            }
            //----Agrega control drowslist
            if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].campo_tip == 0) {
                imputhml = document.createElement("SELECT");
                imputhml.classList.add("form-select");
                let m_compare = ITEM_GENERAL_CONTROL_ARRAY_ASING[i].ilist_row_drowlist;
                if (m_compare != null) {
                    for (let z = 0; z < ITEM_GENERAL_CONTROL_ARRAY_ASING[i].ilist_row_drowlist.length; z++) {
                        let opt = document.createElement("OPTION");
                        opt.text = ITEM_GENERAL_CONTROL_ARRAY_ASING[i].ilist_row_drowlist[z].value_campo;
                        opt.value = ITEM_GENERAL_CONTROL_ARRAY_ASING[i].ilist_row_drowlist[z].id_value;
                        //Asigna el valor default del campo seleccion
                        if (asigna_valor == 1) {
                            if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].texto_campo == ITEM_GENERAL_CONTROL_ARRAY_ASING[i].ilist_row_drowlist[z].value_campo) {
                                opt.selected = true;
                            }
                        }
                        imputhml.add(opt);

                    }
                }
                imputhml.addEventListener("change", event_change_drowslis_form);
                //Descripcion_Documento
                if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].name_campo == "Descripcion_Documento") {
                    imputhml.addEventListener("change", EventActualizaControlTom);
                }
            }
            //---Agrega control text
            if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].campo_tip == 2) {
                imputhml = document.createElement("TEXTAREA");
                if (asigna_valor == 1) {
                    imputhml.value = ITEM_GENERAL_CONTROL_ARRAY_ASING[i].value_campo;
                }
            }
            //Valida campo disabled o enabled
            if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].disable_campo == 0) {
                imputhml.disabled = true;
            }
            if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].type_cells_alow_control == "group") {
                imputhml.classList.add("w-100");
            }
            //valida numero maximo de caracteres
            imputhml.maxLength = ITEM_GENERAL_CONTROL_ARRAY_ASING[i].max_leng_campo;
            //Agrega los atributos al control
            imputhml.setAttribute("atrib_aleas_c", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].aleas_campo);
            imputhml.setAttribute("atrib_campo_O", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].obligatorio_campo);
            imputhml.setAttribute("atrib_campo_n", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].name_campo);
            imputhml.setAttribute("atrib_campo_v", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].alow_tipo_value);
            imputhml.setAttribute("atrib_campo_tip", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].campo_tip);
            imputhml.setAttribute("atrib_campo_nl", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].alow_null);
            imputhml.setAttribute("atrib_campo_id", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].dms_id_registro);
            imputhml.setAttribute("atrib_name_campo_id", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].name_campo_id);
            imputhml.setAttribute("atrib_campo_t", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].tipo_campo);
            imputhml.setAttribute("atrib_campo_tbl", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].tbl_control);
            imputhml.setAttribute("atrib_campo_drow_destino", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].drow_name_controls_destino);
            imputhml.setAttribute("atrib_name_espace_control", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].name_space_campo);
            imputhml.setAttribute("atrib_control_tip_correo", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].control_tip_correo);
            imputhml.setAttribute("atrib_value_campo_old", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].value_campo_old);
            imputhml.setAttribute("atrib_drow_name_control_id", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].drow_name_control_id);
            imputhml.setAttribute("atrib_Tom_alow", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].Tom_alow);
            imputhml.id = ITEM_GENERAL_CONTROL_ARRAY_ASING[i].name_campo + "_" + class_name_form_control;
            imputhml.classList.add(class_name_form_control); 
            //-------Agrega la clase del control control_input_class  
            if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].control_input_class !== "" && ITEM_GENERAL_CONTROL_ARRAY_ASING[i].control_input_class !== null) {
                imputhml.classList.add(ITEM_GENERAL_CONTROL_ARRAY_ASING[i].control_input_class);
            }
            //-------Agrega el campo IMPUT/OPTION/TEXTAREA a la celda
            divtml_.appendChild(imputhml);
            divtml.appendChild(divtml_);
            if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].campo_tip == 15) {
                ConfigsTom.push(
                    {
                        name: ITEM_GENERAL_CONTROL_ARRAY_ASING[i].name_campo, selectId: "#" + ITEM_GENERAL_CONTROL_ARRAY_ASING[i].name_campo + "_" + class_name_form_control,
                        config: {
                            name_dbs_auto: ITEM_GENERAL_CONTROL_ARRAY_ASING[i].dbms_control,
                            name_plantilla_validacion: ITEM_GENERAL_CONTROL_ARRAY_ASING[i].name_plantilla_validacion,
                            campo_nombre_plantilla_val: ITEM_GENERAL_CONTROL_ARRAY_ASING[i].campo_nombre_plantilla_val,
                            campo_primary_plantilla_val: ITEM_GENERAL_CONTROL_ARRAY_ASING[i].campo_primary_plantilla_val,
                            ur_web_service: ITEM_GENERAL_CONTROL_ARRAY_ASING[i].url_class_service_control_plantilla,
                            name_web_service: ITEM_GENERAL_CONTROL_ARRAY_ASING[i].service_control_plantilla,
                            TomParameter: [{ NombreCampo: "IdTipoRestriccion", ValorCampo: "0" }, { NombreCampo: "IdRestriccion", ValorCampo: "0"}],
                            create: false,
                            maxItems: 1,
                            case_Option: ITEM_GENERAL_CONTROL_ARRAY_ASING[i].Tom_option,
                            case_Item: ITEM_GENERAL_CONTROL_ARRAY_ASING[i].Tom_item,
                            mode: "single",
                            id_escript: ITEM_GENERAL_CONTROL_ARRAY_ASING[i].id_escript
                        },
                    }
                );
            }
            //Agrega atribute date           
            switch (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].tipo_campo) {
                case "DATE":
                    load_date_form_control(imputhml.id);
                    imputhml.addEventListener("keypress", validate_fecha_form_control);
                    imputhml.placeholder = "yyyy mm dd";
                    imputhml.classList.add("W-25");
                    imputhml.classList.add("form-control-person");
                    break;
                case "INT":
                    imputhml.addEventListener("keypress", validate_numero_form_control);
                    imputhml.classList.add("W-25");
                    imputhml.classList.add("form-control-person");
                    break;
                default:
                    imputhml.classList.add("form-control");

            }
            //Agrega place hold   Place_Holder
            if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].campo_tip == 1 || ITEM_GENERAL_CONTROL_ARRAY_ASING[i].campo_tip == 2 || ITEM_GENERAL_CONTROL_ARRAY_ASING[i].campo_tip == 15) {
                if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].Place_Holder != "" && ITEM_GENERAL_CONTROL_ARRAY_ASING[i].Place_Holder !== null) {
                    imputhml.placeholder = ITEM_GENERAL_CONTROL_ARRAY_ASING[i].Place_Holder;
                    imputhml.classList.add("form-controls");
                }
            }
            //Agrega los eventos del control
            if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].event_control) {
                for (let z = 0; z < ITEM_GENERAL_CONTROL_ARRAY_ASING[i].event_control.length; z++) {
                    let name_event = ITEM_GENERAL_CONTROL_ARRAY_ASING[i].event_control[z].name_event_control;
                    let name_funtion_event = ITEM_GENERAL_CONTROL_ARRAY_ASING[i].event_control[z].name_function_event_control;
                    switch (name_funtion_event) {
                        case "ValidateCapitalLeter":
                            imputhml.addEventListener("focusout", ValidateCapitalLeter);
                            break;
                        case "validateLowercase":
                            imputhml.addEventListener("focusout", validateLowercase);
                            break;
                        case "validateUpperCase":
                            imputhml.addEventListener("focusout", validateUpperCase);
                            break;
                    }
                }
            }
            if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].campo_tip == 1 || ITEM_GENERAL_CONTROL_ARRAY_ASING[i].campo_tip == 2) {

                if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].clas_service_control == "") {
                    service_auto_complete_form_control(ITEM_GENERAL_CONTROL_ARRAY_ASING[i].name_campo, ITEM_GENERAL_CONTROL_ARRAY_ASING[i].dbms_control,
                        ITEM_GENERAL_CONTROL_ARRAY_ASING[i].tbl_control, ITEM_GENERAL_CONTROL_ARRAY_ASING[i].name_campo);

                } else {
                    if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].clas_service_control != "NA") {
                        service_auto_complete_form_control_person(ITEM_GENERAL_CONTROL_ARRAY_ASING[i].name_campo + "_" + class_name_form_control, ITEM_GENERAL_CONTROL_ARRAY_ASING[i].dbms_control,
                            ITEM_GENERAL_CONTROL_ARRAY_ASING[i].tbl_control, ITEM_GENERAL_CONTROL_ARRAY_ASING[i].name_campo, ITEM_GENERAL_CONTROL_ARRAY_ASING[i].clas_service_control,
                            ITEM_GENERAL_CONTROL_ARRAY_ASING[i].service_control);
                    }

                }
            }
        }
        if (ConfigsTom.length > 0) {
            GroupManagerTomSlect = new TomSelectGroup(ConfigsTom);
        }
        return "YES";
    } catch (ex) {
        return "Inconsistencia general fucion Create_interface_radicacion_simplificada : " + ex.message;
    }

}
const EventActualizaControlTom = async (e)  => {
    try {
        let HtmlSelect = e.currentTarget;
        let SelectIndext = HtmlSelect.options[HtmlSelect.selectedIndex];
        let IdTipoTramite = SelectIndext.value;
        let Result = "";
        Result = await ServiceRESTSolicitaEstructuraRelacionTipoRestriccion(IdTipoTramite);
        if (Result.error) {
            alert_bot(Result.message, 'warning', "div_error_content_content_general_rad_simp");
            return true;
        }
        _CDeRelacionEstadoRetriccion = [];
        _CDeRelacionEstadoRetriccion = Result.CDeRelacionEstadoRetriccion;
        const managerDestinatarios = GroupManagerTomSlect.getManager("Destinatario_Cor");
        if (managerDestinatarios) {
            managerDestinatarios.clearAllTokens();
            if (_CDeRelacionEstadoRetriccion) {
                if (_CDeRelacionEstadoRetriccion.ModuloRadicacionSimple == 1) {
                    let TomParameter = [{ NombreCampo: "IdTipoRestriccion", ValorCampo: _CDeRelacionEstadoRetriccion.IdTipoRestriccion }, { NombreCampo: "IdRestriccion", ValorCampo: _CDeRelacionEstadoRetriccion.IdRestricionTipoDstInterno }];
                    managerDestinatarios.updateUrlBase("../webservice/WebService_radicacion_Simplificada.asmx", "ServiceSolicitaAutoCompleteDestinatarioRestriccion");
                    managerDestinatarios.setTomParameters(TomParameter);
                } else {
                    let TomParameter = [{ NombreCampo: "IdTipoRestriccion", ValorCampo: "0" }, { NombreCampo: "IdRestriccion", ValorCampo: "0" }];
                    managerDestinatarios.setTomParameters(TomParameter);
                    managerDestinatarios.updateUrlBase("../webservice/WebService_radicacion_Simplificada.asmx", "Service_Solicita_datos_auto_complete_interno");
                }
            } else {
                let TomParameter = [{ NombreCampo: "IdTipoRestriccion", ValorCampo: "0" }, { NombreCampo: "IdRestriccion", ValorCampo: "0" }];
                managerDestinatarios.setTomParameters(TomParameter);
                managerDestinatarios.updateUrlBase("../webservice/WebService_radicacion_Simplificada.asmx", "Service_Solicita_datos_auto_complete_interno");
            }
        }
        
    } catch (ex) {
        alert_bot("Inconsistencia general funcion EventActualizaControlTom : " + ex.message, 'warning', "div_error_content_content_general_rad_simp");
    }
}
const LimpiarCamposRegistroRadicacion = async (IdElment) => {
    try {
        const Element = document.getElementById(IdElment);
        if (!Element) return "Imposble emcontar el control (" + IdElment & ")";
        // Solo limpia inputs tipo text  div_datos_registro_tramite
        Element.querySelectorAll('input').forEach(input => {
            switch (input.type) {
                case 'text':
                case 'email':
                case 'number':
                case 'password':
                case 'date':
                case 'time':
                    input.value = "";
                    break;
                case 'checkbox':
                case 'radio':
                    //input.checked = false;
                    break;
            }
        });
        Element.querySelectorAll('textarea').forEach(area => area.value = "");
        GroupManagerTomSlect.clearAllTokens();
        return "YES";
    } catch (ex) {
        return ex.mensaje;
    }
}
const asigna_gestion_soporte_documental = async (stru_asing_soporte_array) => {
    let myPromise = new Promise(function (resolve) {
        try {
            
            let Panel_EnviarUsuario = document.getElementById("Panel_EnviarUsuario");
            let Panel_EnviaActividad = document.getElementById("Panel_EnviaActividad");
            let Panel_autoterminar = document.getElementById("Panel_auto_terminar");
            let panel_terminar_rad = document.getElementById("Panel_terminar_radicado");
            let Panel_enviar_flujo = document.getElementById("Panel_enviar_flujo");
            let Panel_imprime_rotulo = document.getElementById("Panel_imprime_rotulo");
            let Panel_cargar_archivo = document.getElementById("Panel_cargar_archivo");
            let Panel_auto_vincular = document.getElementById("Panel_auto_vincular");
            let Label_estado_selecion = document.getElementById("Label_estado_selecion");
            let Label_documentos = document.getElementById("Label_documentos");
            let Panel_pendiente_radicado = document.getElementById("Panel_pendiente_radicado");
            let Label_estado_transac = document.getElementById("Label_estado_transac");
            let IframeVisor_ = document.getElementById("IframeVisor_");
            let IframeDitaliza_ = document.getElementById("IframeDitaliza_");
            let Area_Visor = document.getElementById("Area_Visor");
            let Are_Digitalizacion = document.getElementById("Are_Digitalizacion");
           
            //-------Inicializa controles estado display------  "../workflow/WebFormEscan.aspx"
            Panel_EnviarUsuario.style.display = "none";
            Panel_EnviaActividad.style.display = "none";
            Panel_autoterminar.style.display = "none";
            panel_terminar_rad.style.display = "none";
            Panel_enviar_flujo.style.display = "none";
            Panel_imprime_rotulo.style.display = "none";
            Panel_cargar_archivo.style.display = "none";
            Panel_auto_vincular.style.display = "none";
            Label_documentos.innerText = "Documentos ";
            IframeVisor_.src = "";
            IframeDitaliza_.src = "";
            Area_Visor.style.display = "none";
            Are_Digitalizacion.style.display = "block";
            IframeDitaliza_.src = stru_asing_soporte_array[0].url_escaner;
            //-------Configura la interface para el tipo de asignación 2 
            if (stru_asing_soporte_array[0].RA_TIPO_MODULO_GESTION_ENVIO_RADICADO == 2) {
                //-----------Zona flujo de trabajo-------////
                if (stru_asing_soporte_array[0].id_flujo_trabajo !== 0) {
                    
                    //--------Configura para el tipo de respuesta obligatoria
                    if (stru_asing_soporte_array[0].estado_resp_obligatoria == 1) {
                        Panel_EnviarUsuario.style.display = "none";
                        Panel_EnviaActividad.style.display = "none";
                        Panel_autoterminar.style.display = "flex";
                        panel_terminar_rad.style.display = "none";
                        Panel_enviar_flujo.style.display = "none";
                        Label_estado_selecion.innerText = "Flujo : " + stru_asing_soporte_array[0].stru_permisos_interface_envio.nombre_flujo_ruta + " Tipo flujo : " + stru_asing_soporte_array[0].estado_cerrado;
                    } else {
                        Panel_EnviarUsuario.style.display = "none";
                        Panel_EnviaActividad.style.display = "none";
                        Panel_autoterminar.style.display = "none";
                        panel_terminar_rad.style.display = "none";
                        Panel_enviar_flujo.style.display = "flex";
                        Label_estado_selecion.innerText = "Flujo : " + stru_asing_soporte_array[0].stru_permisos_interface_envio.nombre_flujo_ruta + " Tipo flujo : " + stru_asing_soporte_array[0].estado_cerrado;
                    }
                } else {
                    //------Zona configuración ruta trabajo ----////
                    //------Congigura ruta cerrado
                    if (stru_asing_soporte_array[0].stru_permisos_interface_envio.estado_cerrado == 1) {
                        Panel_EnviarUsuario.style.display = "none";
                        Panel_EnviaActividad.style.display = "flex";
                        Panel_autoterminar.style.display = "none";
                        panel_terminar_rad.style.display = "none";
                        Panel_enviar_flujo.style.display = "none";
                        Label_estado_selecion.innerText = "Ruta : " + stru_asing_soporte_array[0].stru_permisos_interface_envio.nombre_flujo_ruta + " Tipo ruta : " + stru_asing_soporte_array[0].estado_cerrado;
                    } else {
                        //------Congigura ruta abierta
                        Panel_EnviarUsuario.style.display = "flex";
                        Panel_EnviaActividad.style.display = "flex";
                        Panel_autoterminar.style.display = "none";
                        panel_terminar_rad.style.display = "none";
                        Panel_enviar_flujo.style.display = "none";
                        Label_estado_selecion.innerText = "Ruta : " + stru_asing_soporte_array[0].stru_permisos_interface_envio.nombre_flujo_ruta + " Tipo ruta : " & stru_asing_soporte_array[0].estado_cerrado;
                    }

                }
               
                
            }
            //-------Configura la interface para el tipo de asignación 3 
            if (stru_asing_soporte_array[0].RA_TIPO_MODULO_GESTION_ENVIO_RADICADO == 3) {
                Panel_EnviarUsuario.style.display = "none";
                Panel_EnviaActividad.style.display = "none";
                Panel_autoterminar.style.display = "none";
                panel_terminar_rad.style.display = "flex";
                Panel_enviar_flujo.style.display = "none";
                Label_estado_selecion.innerText = "Flujo : " + stru_asing_soporte_array[0].stru_permisos_interface_envio.nombre_flujo_ruta + " Tipo flujo : " + stru_asing_soporte_array[0].estado_cerrado;
            }
            if (stru_asing_soporte_array[0].util_estado_pendiete_rad == 1) {
                Panel_pendiente_radicado.style.display = "flex";
            }
            if (stru_asing_soporte_array[0].util_opcion_auto_vincula == 1) {
                Panel_auto_vincular.style.display = "flex";
            }
            //--------Lista estado radicado----///
            Label_estado_transac.innerText = "Radicado : " + stru_asing_soporte_array[0].stru_registro_estado.consecutivo_radicado + "   Beneficiario : " + stru_asing_soporte_array[0].stru_registro_estado.remitente;
            //------Agrega los eventos para los controles del menu
            if (Panel_EnviarUsuario.style.display !== 'none') {
                boton_rad_simpl_send_user_task.addEventListener("click", handler_element_event, false);
            }
            if (Panel_EnviaActividad.style.display !== 'none') {
                boton_rad_simpl_send_task_gorup.addEventListener("click", handler_element_event, false);
            }
            if (Panel_autoterminar.style.display !== 'none') {
                boton_rad_simpl_send_task_rad_gestion.addEventListener("click", handler_element_event, false);
            }
            if (panel_terminar_rad.style.display !== 'none') {
                boton_rad_simpl_end_rad.addEventListener("click", handler_element_event, false);
            }
            if (Panel_enviar_flujo.style.display !== 'none') {
                boton_rad_simpl_send_task_flow.addEventListener("click", handler_element_event, false);
            }
            if (Panel_cargar_archivo.style.display !== 'none') {
                boton_rad_simpl_load_file.addEventListener("click", handler_element_event, false);
            }
            if (Panel_imprime_rotulo.style.display !== 'none') {
                boton_rad_simpl_printer_rot.addEventListener("click", handler_element_event, false);
                boton_rad_simpl_save_rot.addEventListener("click", handler_element_event, false);
                boton_rad_simpl_detail_rad.addEventListener("click", handler_element_event, false);
            }
            if (Panel_auto_vincular.style.display !== 'none') {
                boton_rad_simpl_auto_vincula.addEventListener("click", handler_element_event, false);
            }
            destroy_table_bootstrap_table('table_doc_flow_select');
            let class_stru_row_Gabinete_Generic = JSON.parse(stru_asing_soporte_array[0].ROW_GABINETE_GENERIC[0].Obj_ilist_row_generic);
            init_row_feld_table_boostrap_table("table_doc_flow_select", stru_asing_soporte_array[0].ROW_GABINETE_GENERIC[0].Obj_ilist_fileds_generic,
                class_stru_row_Gabinete_Generic, "div_rad_simple_content_table", null, null, "single","bt-selected",false,true,true);
            Label_documentos.innerText = "Documentos " + class_stru_row_Gabinete_Generic.length;
            let html_hiden = document.getElementById("HiddenIdFlujo");
            if (html_hiden) {
                html_hiden.value = stru_asing_soporte_array[0].ID_TAREA_SELECCIONDA;
            }
            CONST_ID_REGISTRO_ESTADO = stru_asing_soporte_array[0].RA_ID_REGISTRO_RADICADO;
            resolve("YES");
        } catch (ex) {
            resolve(ex.message + " funcion asigna_gestion_soporte_documental");
        }
    })
    let result = await myPromise;
    return result;
}
const eliminar_gestion_soporte_documental = async (stru_asing_soporte_array) => {
    let myPromise = new Promise(function (resolve) {
        try {
            let Panel_EnviarUsuario = document.getElementById("Panel_EnviarUsuario");
            let Panel_EnviaActividad = document.getElementById("Panel_EnviaActividad");
            let Panel_autoterminar = document.getElementById("Panel_auto_terminar");
            let panel_terminar_rad = document.getElementById("Panel_terminar_radicado");
            let Panel_enviar_flujo = document.getElementById("Panel_enviar_flujo");
            let Panel_imprime_rotulo = document.getElementById("Panel_imprime_rotulo");
            let Panel_cargar_archivo = document.getElementById("Panel_cargar_archivo");
            let Panel_auto_vincular = document.getElementById("Panel_auto_vincular");
            let Label_estado_selecion = document.getElementById("Label_estado_selecion");
            let Label_documentos = document.getElementById("Label_documentos");
            let Panel_pendiente_radicado = document.getElementById("Panel_pendiente_radicado");
            let Label_estado_transac = document.getElementById("Label_estado_transac");
            let IframeVisor_ = document.getElementById("IframeVisor_");
            let IframeDitaliza_ = document.getElementById("IframeDitaliza_");
            let Area_Visor = document.getElementById("Area_Visor");
            let Are_Digitalizacion = document.getElementById("Are_Digitalizacion");
            //-------Inicializa controles estado display------  
            Panel_EnviarUsuario.style.display = "none";
            Panel_EnviaActividad.style.display = "none";
            Panel_autoterminar.style.display = "none";
            panel_terminar_rad.style.display = "none";
            Panel_enviar_flujo.style.display = "none";
            Panel_imprime_rotulo.style.display = "none";
            Panel_cargar_archivo.style.display = "none";
            Panel_auto_vincular.style.display = "none";
            Label_documentos.innerText = "Documentos ";
            Label_estado_selecion.innerText = "";
            Label_estado_transac.innerText = "";
            IframeVisor_.src = "";
            IframeDitaliza_.src = "";
            Area_Visor.style.display = "none";
            Are_Digitalizacion.style.display = "block";
            IframeDitaliza_.src = stru_asing_soporte_array[0].url_escaner;
            Label_documentos.innerText = "Documentos ";
            let html_hiden = document.getElementById("HiddenIdFlujo");
            if (html_hiden) {
                html_hiden.value = 0;
            }
            CONST_ID_REGISTRO_ESTADO = 0;
            CONST_ID_IMAGEN_RAD = 0;
            destroy_table_bootstrap_table("table_doc_flow_select");
            resolve("YES");
        } catch (ex) {
            resolve(ex.message + " funcion asigna_gestion_soporte_documental");
        }
    })
    let result = await myPromise;
    return result;
}
function prevent_cerrar(event, element) {
    try {
        document.getElementById('Area_Visor').style.display = 'none';
        document.getElementById('Are_Digitalizacion').style.display = 'block';
        event.preventDefault();
    }
    catch (err) {
        alert(err.message + " Funcion prevent ");
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
//----------------------Array control----------------////
/*
 name_control : nonbre del control selector
 ur_web_service  : url web service del control
 name_web_service : nombre del servicio web
 seting_create : evalua si el usuario crea el servicio
 seting_maxItems : maximo numero de selecciones por el usuario
 seting_maxOptions : maximo numero de opciones mostradas en la lista

 */

//---------------------Zona service-------------------------------//

const ServiceRESTSolicitaEstructuraRelacionTipoRestriccion = async (parameter) => {
    try {
        // Si parameter es objeto -> se serializa
        // Si ya es string -> se manda directo
        const response = await fetch("../webservice/WebService_radicacion_Simplificada.asmx/ServiceSolicitaEstructuraRelacionTipoRestriccion", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: "{" + "'IdTipoTramite':'" + parameter + "'}", // tu data JSON se convierte en TEXTO
        });
        if (!response.ok) {
            return { error: true, status: response.status, message: response.statusText };
        }
        let responsejson = await response.json(); // 👈 respuesta en JSON
        if (responsejson.d[0].AppError != "YES") {
            return { error: true, message: responsejson.d[0].AppError, CDeRelacionEstadoRetriccion: responsejson.d[0].CDeRelacionEstadoRetriccion };
        } else {
            return { error: false, message: responsejson.d[0].AppError, CDeRelacionEstadoRetriccion: responsejson.d[0].CDeRelacionEstadoRetriccion };
        }
    } catch (err) {
        return {error: true, message: err.message};
    }
}

const Service_REST_solicita_estructura_radicacion_simplificada = async (parameter) => {
    var serialice = JSON.stringify(parameter);
    ITEM_GENERAL_CONTROL_ARRAY_ASING = new Array();
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebService_radicacion_Simplificada.asmx/Service_solicita_estructura_radicacion_simplificada', {
                data: "{" + "'parameter':'" + serialice + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].error_gestion !== "YES") {
                        resolve(data.d[0].error_gestion);

                    } else {
                        $.each(data.d, function (k, v) {
                            ITEM_GENERAL_CONTROL_ARRAY_ASING.push(v);
                        });
                        let resultado = Create_interface_radicacion_simplificada(parameter[0].name_container, parameter[0].class_name_control,
                            parameter[0].asigna_valor, parameter[0].apost_name_content, parameter[0].add_check);
                        if (resultado == "YES") {
                            if (parameter[0].name_control_padre !== "") {
                                $("#" + parameter[0].name_control_padre).modal("show");
                            }
                            resolve(resultado);
                        } else {
                            resolve(resultado);
                        }
                        
                    }
                }, error: function (xception, textStatus, errorThrown) {
                    //ESTADO_EVENT_GENERAL = "out";
                    if (xception.status === 0) {

                        resolve('Not connect: Verify Network.');

                    } else if (xception.status == 404) {

                        resolve('Requested page not found [404]');

                    } else if (xception.status == 500) {

                        resolve('Internal Server Error [500].' + xception.responseText);

                    } else if (textStatus === 'parsererror') {

                        resolve('Requested JSON parse failed.');

                    } else if (textStatus === 'timeout') {

                        resolve('Time out error.');

                    } else if (textStatus === 'abort') {

                        resolve('Ajax request aborted.');

                    } else {

                        resolve('Uncaught Error: ' + xception.responseText);

                    }
                }
            });
        } catch (ex) {
            resolve(ex.message + " funcion Service_REST_solicita_estructura_radicacion_simplificada");
        }
    })
    let result = await myPromise;
    return result;
}
const Service_REST_solicita_nombre_plantilla_radicacion_simplificada = async (parameter) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebService_radicacion_Simplificada.asmx/Service_solicita_nombre_plantilla_radicacion_simplificada', {
                data: "{" + "'parameter':'" + parameter + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].error_gestion !== "YES") {
                        resolve(data.d[0].error_gestion);
                    } else {
                        CONST_NOMBRE_PLANTILLA_RAD = data.d[0].Nombre_Plantilla_Radicado;
                        CONS_ID_PLANTILLA_RAD = data.d[0].id_Plantilla;
                        CONS_ID_TIPO_PLANTILLA_RAD = data.d[0].id_tipo_plantilla;
                        CONS_UTIL_ESTADO_PENDIENTE = data.d[0].util_estado_pendiente_rad;
                        CONS_TIPO_PLANTILLA_RAD = data.d[0].Tipo_Plantilla;
                        resolve("YES");
                    }
                }, error: function (xception, textStatus, errorThrown) {
                    //ESTADO_EVENT_GENERAL = "out";
                    if (xception.status === 0) {

                        resolve('Not connect: Verify Network.');

                    } else if (xception.status == 404) {

                        resolve('Requested page not found [404]');

                    } else if (xception.status == 500) {

                        resolve('Internal Server Error [500].' + xception.responseText);

                    } else if (textStatus === 'parsererror') {

                        resolve('Requested JSON parse failed.');

                    } else if (textStatus === 'timeout') {

                        resolve('Time out error.');

                    } else if (textStatus === 'abort') {

                        resolve('Ajax request aborted.');

                    } else {

                        resolve('Uncaught Error: ' + xception.responseText);

                    }
                }
            });
        } catch (ex) {
            resolve(ex.message + " funcion Service_REST_solicita_nombre_plantilla_radicacion_simplificada");
        }
    })
    let result = await myPromise;
    return result;
}
const Service_REST_activa_gestion_solicitante_plantilla_validacion = async (parameter) => {
    var serialice = JSON.stringify(parameter);
    ITEM_GENERAL_CONTROL_ARRAY_ASING = new Array();
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebService_plantilla_validacion.asmx/Service_solicita_estructura_formulario_registro_validacion_externo_id', {
                data: "{" + "'parameter':'" + serialice + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].error_gestion !== "YES") {
                        resolve(data.d[0].error_gestion);
                    } else {
                        $.each(data.d, function (k, v) {
                            ITEM_GENERAL_CONTROL_ARRAY_ASING.push(v);
                        });

                        let resultado = Create_interface_formulario_control(parameter[0].name_container, parameter[0].class_name_control,
                            parameter[0].asigna_valor, parameter[0].apost_name_content, parameter[0].add_check);
                        if (resultado == "YES") {
                            if (parameter[0].name_control_tittle != "") {
                                let control_title = document.getElementById(parameter[0].name_control_tittle);
                                if (control_title) {
                                    control_title.textContent = parameter[0].value_control_tittle;
                                }
                            }
                            if (parameter[0].name_control_padre !== "") {
                                $("#" + parameter[0].name_control_padre).modal("show");
                            }
                           
                            resolve(resultado);
                        } else {
                            resolve(resultado);
                        }
                    }
                }, error: function (xception, textStatus, errorThrown) {
                    //ESTADO_EVENT_GENERAL = "out";
                    if (xception.status === 0) {

                        resolve('Not connect: Verify Network.');

                    } else if (xception.status == 404) {

                        resolve('Requested page not found [404]');

                    } else if (xception.status == 500) {

                        resolve('Internal Server Error [500].' + xception.responseText);

                    } else if (textStatus === 'parsererror') {

                        resolve('Requested JSON parse failed.');

                    } else if (textStatus === 'timeout') {

                        resolve('Time out error.');

                    } else if (textStatus === 'abort') {

                        resolve('Ajax request aborted.');

                    } else {

                        resolve('Uncaught Error: ' + xception.responseText);

                    }
                }
            });
        } catch (ex) {
            resolve(ex.message + " funcion Service_REST_activa_gestion_solicitante_plantilla_validacion");
        }
    })
    let result = await myPromise;
    return result;
}
const Service_REST_registra_tercero_plantilla_validacion_simplificada = async (parameter, name_tom_select, name_container,id_script) => {
    var serialice = JSON.stringify(parameter);
    ITEM_GENERAL_CONTROL_ARRAY_ASING = new Array();
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebService_plantilla_validacion.asmx/Service_registra_tercero_plantilla_validacion_simplificada', {
                data: "{" + "'parameter':'" + serialice + "','id_script':'" + id_script + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].error_gestion !== "YES") {
                        resolve(data.d[0].error_gestion);
                    } else {
                        let select = document.getElementById(name_tom_select);
                        let control = select.tomselect;
                        control.addOption(data.d[0].row_tom);
                        control.addItem(data.d[0].row_tom[0].id_value);
                        $("#" + name_container).modal("hide");
                        resolve("YES");
                       
                    }
                }, error: function (xception, textStatus, errorThrown) {
                    //ESTADO_EVENT_GENERAL = "out";
                    if (xception.status === 0) {

                        resolve('Not connect: Verify Network.');

                    } else if (xception.status == 404) {

                        resolve('Requested page not found [404]');

                    } else if (xception.status == 500) {

                        resolve('Internal Server Error [500].' + xception.responseText);

                    } else if (textStatus === 'parsererror') {

                        resolve('Requested JSON parse failed.');

                    } else if (textStatus === 'timeout') {

                        resolve('Time out error.');

                    } else if (textStatus === 'abort') {

                        resolve('Ajax request aborted.');

                    } else {

                        resolve('Uncaught Error: ' + xception.responseText);

                    }
                }
            });
        } catch (ex) {
            resolve(ex.message + " funcion Service_REST_registra_tercero_plantilla_validacion_simplificada");
        }
    })
    let result = await myPromise;
    return result;
}
const Service_REST_update_tercero_plantilla_validacion_simplificada = async (parameter, name_tom_select, name_container, id_script) => {
    var serialice = JSON.stringify(parameter);
    ITEM_GENERAL_CONTROL_ARRAY_ASING = new Array();
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebService_plantilla_validacion.asmx/Service_update_tercero_plantilla_validacion_simplificada', {
                data: "{" + "'parameter':'" + serialice + "','id_script':'" + id_script + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].error_gestion !== "YES") {
                        resolve(data.d[0].error_gestion);
                    } else {
                        let select = document.getElementById(name_tom_select);
                        let control = select.tomselect;
                        control.removeOption(data.d[0].row_tom[0].id_value);
                        control.addOption(data.d[0].row_tom, user_created = false);
                        control.addItem(data.d[0].row_tom[0].id_value);
                        $("#" + name_container).modal("hide");
                        resolve("YES");

                    }
                }, error: function (xception, textStatus, errorThrown) {
                    //ESTADO_EVENT_GENERAL = "out";
                    if (xception.status === 0) {

                        resolve('Not connect: Verify Network.');

                    } else if (xception.status == 404) {

                        resolve('Requested page not found [404]');

                    } else if (xception.status == 500) {

                        resolve('Internal Server Error [500].' + xception.responseText);

                    } else if (textStatus === 'parsererror') {

                        resolve('Requested JSON parse failed.');

                    } else if (textStatus === 'timeout') {

                        resolve('Time out error.');

                    } else if (textStatus === 'abort') {

                        resolve('Ajax request aborted.');

                    } else {

                        resolve('Uncaught Error: ' + xception.responseText);

                    }
                }
            });
        } catch (ex) {
            resolve(ex.message + " funcion Service_REST_update_tercero_plantilla_validacion_simplificada");
        }
    })
    let result = await myPromise;
    return result;
}
const Service_REST_delete_tercero_plantilla_validacion_simplificada = async (parameter, name_tom_select, id_script) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebService_plantilla_validacion.asmx/Service_delete_tercero_plantilla_validacion_simplificada', {
                data: "{" + "'parameter':'" + parameter + "','id_script':'" + id_script + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].error_gestion !== "YES") {
                        resolve(data.d[0].error_gestion);
                    } else {
                        let select = document.getElementById(name_tom_select);
                        let control = select.tomselect;
                        control.removeOption(data.d[0].row_tom[0].id_value);
                        control.refreshOptions(false);
                        control.inputState();
                        resolve("YES");
                    }
                }, error: function (xception, textStatus, errorThrown) {
                    
                    if (xception.status === 0) {

                        resolve('Not connect: Verify Network.');

                    } else if (xception.status == 404) {

                        resolve('Requested page not found [404]');

                    } else if (xception.status == 500) {

                        resolve('Internal Server Error [500].' + xception.responseText);

                    } else if (textStatus === 'parsererror') {

                        resolve('Requested JSON parse failed.');

                    } else if (textStatus === 'timeout') {

                        resolve('Time out error.');

                    } else if (textStatus === 'abort') {

                        resolve('Ajax request aborted.');

                    } else {

                        resolve('Uncaught Error: ' + xception.responseText);

                    }
                }
            });
        } catch (ex) {
            resolve(ex.message + " funcion Service_REST_delete_tercero_plantilla_validacion_simplificada");
        }
    })
    let result = await myPromise;
    return result;
}
const Service_REST_registro_radicacion_simplificada = async (parameter, name_plantilla) => {
    var serialice = JSON.stringify(parameter);
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebService_radicacion_Simplificada.asmx/Service_registro_radicacion_simplificada', {
                data: "{" + "'parameter':'" + serialice + "','name_plantilla':'" + name_plantilla + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].error_gestion !== "YES") {
                        resolve(data.d[0].error_gestion);
                    } else {
                        CONST_ID_REGISTRO_ESTADO = data.d[0].id_registro_estado;
                        if (data.d[0].asignar_radicado !== "YES") {
                            resolve(data.d[0].asignar_radicado);
                        } else {
                            resolve("YES");
                        }
                    }
                }, error: function (xception, textStatus, errorThrown) {
                    //ESTADO_EVENT_GENERAL = "out";
                    if (xception.status === 0) {

                        resolve('Not connect: Verify Network.');

                    } else if (xception.status == 404) {

                        resolve('Requested page not found [404]');

                    } else if (xception.status == 500) {

                        resolve('Internal Server Error [500].' + xception.responseText);

                    } else if (textStatus === 'parsererror') {

                        resolve('Requested JSON parse failed.');

                    } else if (textStatus === 'timeout') {

                        resolve('Time out error.');

                    } else if (textStatus === 'abort') {

                        resolve('Ajax request aborted.');

                    } else {

                        resolve('Uncaught Error: ' + xception.responseText);

                    }
                }
            });
        } catch (ex) {
            resolve(ex.message + " funcion Service_REST_registro_radicacion_simplificada");
        }
    })
    let result = await myPromise;
    return result;
}
const Service_REST_Solicita_radicado_existencia_radicado_asignado = async (id_plantilla, tipo_plantilla) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebService_radicacion_Simplificada.asmx/Service_Solicita_radicado_existencia_radicado_asignado', {
                data: "{" + "'id_plantilla':'" + id_plantilla + "','tipo_plantilla':'" + tipo_plantilla + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].error_gestion !== "YES") {
                        resolve(data.d[0].error_gestion);
                    } else {
                        if (data.d[0].estado_asignado == "YES") {
                            CONST_ID_REGISTRO_ESTADO = data.d[0].id_registro_estado;
                        } else {
                            CONST_ID_REGISTRO_ESTADO = 0;
                        }
                       
                        resolve("YES");
                    }
                }, error: function (xception, textStatus, errorThrown) {
                    //ESTADO_EVENT_GENERAL = "out";
                    if (xception.status === 0) {

                        resolve('Not connect: Verify Network.');

                    } else if (xception.status == 404) {

                        resolve('Requested page not found [404]');

                    } else if (xception.status == 500) {

                        resolve('Internal Server Error [500].' + xception.responseText);

                    } else if (textStatus === 'parsererror') {

                        resolve('Requested JSON parse failed.');

                    } else if (textStatus === 'timeout') {

                        resolve('Time out error.');

                    } else if (textStatus === 'abort') {

                        resolve('Ajax request aborted.');

                    } else {

                        resolve('Uncaught Error: ' + xception.responseText);

                    }
                }
            });
        } catch (ex) {
            resolve(ex.message + " funcion Service_REST_Solicita_radicado_existencia_radicado_asignado");
        }
    })
    let result = await myPromise;
    return result;
}
const Service_REST_Inicializa_cliente_workflow_radicacion_simple = async (parameter) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebService_radicacion_Simplificada.asmx/Service_Inicializa_cliente_workflow_radicacion_simple', {
                data: "{" + "'parameter':'" + parameter  + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].error_gestion !== "YES") {
                        resolve(data.d[0].error_gestion);
                    } else {
                       
                        resolve("YES");
                    }
                }, error: function (xception, textStatus, errorThrown) {
                    //ESTADO_EVENT_GENERAL = "out";
                    if (xception.status === 0) {

                        resolve('Not connect: Verify Network.');

                    } else if (xception.status == 404) {

                        resolve('Requested page not found [404]');

                    } else if (xception.status == 500) {

                        resolve('Internal Server Error [500].' + xception.responseText);

                    } else if (textStatus === 'parsererror') {

                        resolve('Requested JSON parse failed.');

                    } else if (textStatus === 'timeout') {

                        resolve('Time out error.');

                    } else if (textStatus === 'abort') {

                        resolve('Ajax request aborted.');

                    } else {

                        resolve('Uncaught Error: ' + xception.responseText);

                    }
                }
            });
        } catch (ex) {
            resolve(ex.message + " funcion Service_REST_Solicita_radicado_existencia_radicado_asignado");
        }
    })
    let result = await myPromise;
    return result;
}
const Service_REST_solicita_estructura_estado_radicado_radicacion_simple = async (parameter, id_registro_estado) => {
    var serialice = JSON.stringify(parameter);
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebService_radicacion_Simplificada.asmx/Service_solicita_estructura_estado_radicado_radicacion_simple', {
                data: "{" + "'parameter':'" + serialice + "','id_registro_estado':'" + id_registro_estado + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].error_gestion !== "YES") {
                        resolve(data.d[0].error_gestion);
                    } else {
                        CONST_STRU_RAD_ASIN = new Array();
                        $.each(data.d, function (k, v) {
                            CONST_STRU_RAD_ASIN.push(v);
                        });
                        resolve("YES");
                    }
                }, error: function (xception, textStatus, errorThrown) {
                    //ESTADO_EVENT_GENERAL = "out";
                    if (xception.status === 0) {

                        resolve('Not connect: Verify Network.');

                    } else if (xception.status == 404) {

                        resolve('Requested page not found [404]');

                    } else if (xception.status == 500) {

                        resolve('Internal Server Error [500].' + xception.responseText);

                    } else if (textStatus === 'parsererror') {

                        resolve('Requested JSON parse failed.');

                    } else if (textStatus === 'timeout') {

                        resolve('Time out error.');

                    } else if (textStatus === 'abort') {

                        resolve('Ajax request aborted.');

                    } else {

                        resolve('Uncaught Error: ' + xception.responseText);

                    }
                }
            });
        } catch (ex) {
            resolve(ex.message + " funcion Service_REST_registra_tercero_plantilla_validacion_simplificada");
        }
    })
    let result = await myPromise;
    return result;
}
const Service_REST_solicita_estructura_estado_radicado_radicacion_simple_vacia = async (parameter) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebService_radicacion_Simplificada.asmx/Service_solicita_estructura_estado_radicado_radicacion_simple_vacia', {
                data: "{" + "'parameter':'" + parameter +  "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].error_gestion !== "YES") {
                        resolve(data.d[0].error_gestion);
                    } else {
                        CONST_STRU_RAD_ASIN = new Array();
                        $.each(data.d, function (k, v) {
                            CONST_STRU_RAD_ASIN.push(v);
                        });
                        resolve("YES");

                    }
                }, error: function (xception, textStatus, errorThrown) {
                    //ESTADO_EVENT_GENERAL = "out";
                    if (xception.status === 0) {

                        resolve('Not connect: Verify Network.');

                    } else if (xception.status == 404) {

                        resolve('Requested page not found [404]');

                    } else if (xception.status == 500) {

                        resolve('Internal Server Error [500].' + xception.responseText);

                    } else if (textStatus === 'parsererror') {

                        resolve('Requested JSON parse failed.');

                    } else if (textStatus === 'timeout') {

                        resolve('Time out error.');

                    } else if (textStatus === 'abort') {

                        resolve('Ajax request aborted.');

                    } else {

                        resolve('Uncaught Error: ' + xception.responseText);

                    }
                }
            });
        } catch (ex) {
            resolve(ex.message + " funcion Service_REST_registra_tercero_plantilla_validacion_simplificada");
        }
    })
    let result = await myPromise;
    return result;
}
const Service_REST_source_list_tipos_documentales_radicacion_simple = async (name_control, name_control_padre) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceRadicacion.asmx/service_source_list_item_control_general_documento_radicado', {
                data: "{'id':" + "'" + 0 + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].error_sistema !== "YES") {
                        resolve(data.d[0].error_sistema);
                    } else {
                        let items_drow = new Array();
                        $.each(data.d[0].item_sistema, function (k, v) {
                            items_drow.push(v);
                        });
                        if (document.getElementById(name_control)) {
                            var element_drow = document.getElementById(name_control);
                            $("#" + name_control).empty();
                            for (var i = 0; i < items_drow.length; i++) {
                                element_drow[i] = new Option(items_drow[i].text, items_drow[i].value);
                            }
                        }
                        if (document.getElementById(name_control_padre)) {
                            $("#" + name_control_padre).modal("show");
                        }
                        resolve("YES");
                    }
                }, error: function (xception, textStatus, errorThrown) {

                    if (xception.status === 0) {
                        resolve("Not connect: Verify Network.");


                    } else if (xception.status == 404) {
                        resolve("Requested page not found [404]");


                    } else if (xception.status == 500) {
                        resolve("Internal Server Error [500]." + xception.responseText);


                    } else if (textStatus === 'parsererror') {
                        resolve("Requested JSON parse failed.");


                    } else if (textStatus === 'timeout') {
                        return "Time out error.";


                    } else if (textStatus === 'abort') {
                        resolve("Ajax request aborted.");


                    } else {
                        resolve("Ajax request aborted." + xception.responseText);


                    }
                }
            });
        }
        catch (ex) {
            resolve(ex.message);
        }
    })
    let result = await myPromise;
    return result;
}
const Service_REST_actualiza_tipologia_rad_simplificada = async (parameter, name_table, name_control_padre) => {
    let serialice = JSON.stringify(parameter);
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebService_radicacion_Simplificada.asmx/Service_actualiza_tipologia_rad_simplificada', {
                data: "{'parameter':" + "'" + serialice + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].error_sistema !== "YES") {
                        resolve(data.d[0].error_sistema);
                    } else {
                        updateCelByUniqueIdReinit(name_table, 'TIPODOCUMENTO', parameter[0].ID_IMAGEN, parameter[0].TEXT_ITEM);
                        if (document.getElementById(name_control_padre)) {
                            $("#" + name_control_padre).modal("hide");
                        }
                        resolve("YES");
                    }
                }, error: function (xception, textStatus, errorThrown) {

                    if (xception.status === 0) {
                        resolve("Not connect: Verify Network.");
                    } else if (xception.status == 404) {
                        resolve("Requested page not found [404]");
                    } else if (xception.status == 500) {
                        resolve("Internal Server Error [500]." + xception.responseText);
                    } else if (textStatus === 'parsererror') {
                        resolve("Requested JSON parse failed.");
                    } else if (textStatus === 'timeout') {
                        return "Time out error.";
                    } else if (textStatus === 'abort') {
                        resolve("Ajax request aborted.");


                    } else {
                        resolve("Ajax request aborted." + xception.responseText);


                    }
                }
            });
        }
        catch (ex) {
            resolve(ex.message);
        }
    })
    let result = await myPromise;
    return result;
}
//-------Solicita la url del documento a visualizar
const Service_REST_olicita_url_documento_soporte_documental_rad_simple = async (parameter_) => {
    let serialice = JSON.stringify(parameter_);
    document.getElementById("IframeVisor_").src = "";
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceDocuarchi.asmx/Service_solicita_url_documento_soporte_documental_rad_simple', {
                data: "{" + "'parameter':'" + serialice + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_result !== "YES") {
                        resolve(data.d[0].Error_result);
                    } else {
                        document.getElementById("IframeVisor_").src = data.d[0].url_iframe;
                        document.getElementById("titel_visor").innerText = data.d[0].name_file;
                        let ref_Are_Digitalizacion = document.getElementById("Are_Digitalizacion");
                        if (ref_Are_Digitalizacion) {
                            ref_Are_Digitalizacion.style.display = "none";
                        }
                        document.getElementById("Area_Visor").style.display = "block";
                        resolve("YES");
                    }
                }, error: function (xception, textStatus, errorThrown) {

                    if (xception.status === 0) {
                        resolve("Not connect: Verify Network.");


                    } else if (xception.status == 404) {
                        resolve("Requested page not found [404]");


                    } else if (xception.status == 500) {
                        resolve("Internal Server Error [500]." + xception.responseText);


                    } else if (textStatus === 'parsererror') {
                        resolve("Requested JSON parse failed.");


                    } else if (textStatus === 'timeout') {
                        return "Time out error.";


                    } else if (textStatus === 'abort') {
                        resolve("Ajax request aborted.");


                    } else {
                        resolve("Ajax request aborted." + xception.responseText);


                    }
                }
            });
        }
        catch (ex) {
            resolve(ex.message);
        }
    })
    let result = await myPromise;
    return result;
}
const Service_REST_elimina_documento_enlace_radicado = async (id_image, table) => {
    let myPromise = new Promise(function (resolve) {
    try {
        $.ajax('../webservice/WebServiceDocuarchi.asmx/Service_elimina_documento_enlace_radicado_workflow', {
            data: "{" + "'parameter':'" + id_image + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_gestion !== "YES") {
                    resolve(data.d[0].error_gestion);
                   
                } else {
                    delete_row_table(table, "ID", id_image);
                    if (data.d[0].limpia_visor == 1) {
                        let res = clrear_view_radicado_simple();        
                    }
                    let numrowTables = 0;
                    numrowTables = total_row_table(table);
                    let html_lable = document.getElementById("Label_documentos");
                    if (html_lable) {
                        html_lable.innerText = "Documentos " + numrowTables;
                    }
                    resolve("YES");
                }
            }, error: function (xception, textStatus, errorThrown) {
                if (xception.status === 0) {
                    resolve("Not connect: Verify Network.");


                } else if (xception.status == 404) {
                    resolve("Requested page not found [404]");


                } else if (xception.status == 500) {
                    resolve("Internal Server Error [500]." + xception.responseText);


                } else if (textStatus === 'parsererror') {
                    resolve("Requested JSON parse failed.");


                } else if (textStatus === 'timeout') {
                    return "Time out error.";


                } else if (textStatus === 'abort') {
                    resolve("Ajax request aborted.");


                } else {
                    resolve("Ajax request aborted." + xception.responseText);


                }
            }
        });
    }
        catch (ex) {
            resolve(ex.message);
        }
    })
    let result = await myPromise;
    return result;
}
const Service_REST_almacenamiento_documentos_digitalizados_rad_simplificada = async (parameter, name_table, name_control_padre) => {
    let serialice = JSON.stringify(parameter);
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebService_radicacion_Simplificada.asmx/Service_almacenamiento_documentos_digitalizados_rad_simplificada', {
                data: "{'parameter':" + "'" + serialice + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].error_sistema !== "YES") {
                        resolve(data.d[0].error_sistema);
                    } else {
                        let row = {};
                        row = new Object();
                        row["ID"] = data.d[0].id_imagen;
                        row["PAG"] = 0;
                        row["TIPODOCUMENTO"] = data.d[0].notipodocumento;
                        row["ESTADO_FIRMA_DIGITAL"] = data.d[0].estado_firma_digital;
                        row["DBT"] = data.d[0].DBT;
                        insert_row_table(name_table, row);
                        let html_parent = document.getElementById(name_control_padre);
                        if (html_parent) {
                            $("#" + personIni.settings.element_parent).modal("hide");
                        }
                        let numrowTables = 0;
                        numrowTables = total_row_table(name_table);
                        let html_lable = document.getElementById("Label_documentos");
                        if (html_lable) {
                            html_lable.innerText = "Documentos " + numrowTables;
                        }
                        resolve("YES");
                    }
                }, error: function (xception, textStatus, errorThrown) {

                    if (xception.status === 0) {
                        resolve("Not connect: Verify Network.");
                    } else if (xception.status == 404) {
                        resolve("Requested page not found [404]");
                    } else if (xception.status == 500) {
                        resolve("Internal Server Error [500]." + xception.responseText);
                    } else if (textStatus === 'parsererror') {
                        resolve("Requested JSON parse failed.");
                    } else if (textStatus === 'timeout') {
                        return "Time out error.";
                    } else if (textStatus === 'abort') {
                        resolve("Ajax request aborted.");


                    } else {
                        resolve("Ajax request aborted." + xception.responseText);


                    }
                }
            });
        }
        catch (ex) {
            resolve(ex.message);
        }
    })
    let result = await myPromise;
    return result;
}
//-------Realiza la consulta y lista las actividades de flujo para envio
const Service_REST_solicita_listado_actividades_para_envio_tarea_a_flujo = async (radicado, id_tarea_workflow, name_table, name_control_padre) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceWorkflow.asmx/Service_solicita_listado_actividades_para_envio_tarea_a_flujo', {
                data: "{" + "'radicado':'" + radicado + "','" + "id_tarea_workflow':'" + id_tarea_workflow + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_result !== "YES") {
                        resolve(data.d[0].Error_result);
                    } else {
                        let class_stru_row_Gabinete_Generic = JSON.parse(data.d[0].Obj_ilist_row_generic);
                        init_row_feld_table_boostrap_table(name_table, data.d[0].Obj_ilist_fileds_generic, class_stru_row_Gabinete_Generic, "", "table-bordered", "table-borderless");
                        if (document.getElementById(name_control_padre)) {
                            $("#" + name_control_padre).modal("show");
                        }
                        resolve("YES");

                    }
                }, error: function (xception, textStatus, errorThrown) {

                    if (xception.status === 0) {
                        resolve("Not connect: Verify Network.");


                    } else if (xception.status == 404) {
                        resolve("Requested page not found [404]");


                    } else if (xception.status == 500) {
                        resolve("Internal Server Error [500]." + xception.responseText);


                    } else if (textStatus === 'parsererror') {
                        resolve("Requested JSON parse failed.");


                    } else if (textStatus === 'timeout') {
                        return "Time out error.";


                    } else if (textStatus === 'abort') {
                        resolve("Ajax request aborted.");


                    } else {
                        resolve("Ajax request aborted." + xception.responseText);


                    }
                }
            });
        }
        catch (ex) {
            resolve(ex.message);
        }
    })
    let result = await myPromise;
    return result;

}
//-------Realiza la consulta y lista las actividades de flujo para envio
const Service_REST_enviar_tarea_flujo_trabajo_radicacion_simple = async (identi_actividad_flujo_destino, id_tarea_workflow, name_control_padre) => {
   
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceWorkflow.asmx/Service_enviar_tarea_flujo_trabajo_radicacion_simple', {
                data: "{" + "'identi_actividad_flujo_destino':'" + identi_actividad_flujo_destino + "','" + "id_tarea_workflow':'" + id_tarea_workflow + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_result !== "YES") {
                        resolve(data.d[0].Error_result);
                    } else {    
                        if (document.getElementById(name_control_padre)) {
                            $("#" + name_control_padre).modal("hide");
                        }
                        CONST_ESTADO_ENVIO_SEND_MAIL = data.d[0].Resultado_send_correo;
                        resolve("YES");
                    }
                }, error: function (xception, textStatus, errorThrown) {

                    if (xception.status === 0) {
                        resolve("Not connect: Verify Network.");


                    } else if (xception.status == 404) {
                        resolve("Requested page not found [404]");


                    } else if (xception.status == 500) {
                        resolve("Internal Server Error [500]." + xception.responseText);


                    } else if (textStatus === 'parsererror') {
                        resolve("Requested JSON parse failed.");


                    } else if (textStatus === 'timeout') {
                        return "Time out error.";


                    } else if (textStatus === 'abort') {
                        resolve("Ajax request aborted.");


                    } else {
                        resolve("Ajax request aborted." + xception.responseText);


                    }
                }
            });
        }
        catch (ex) {
            resolve(ex.message);
        }
    })
    let result = await myPromise;
    return result;

}

//-------Realiza la consulta y lista radicados pendientes
const Service_REST_Solicita_radicados_pendientes_radicacion = async (id_plantilla_radicado, name_table, name_control_padre) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebService_radicacion_Simplificada.asmx/Service_Solicita_radicados_pendientes_radicacion', {
                data: "{" + "'id_plantilla_radicado':'" + id_plantilla_radicado  + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_result !== "YES") {
                        resolve(data.d[0].Error_result);
                    } else {
                        let class_stru_row_Gabinete_Generic = JSON.parse(data.d[0].Obj_ilist_row_generic);
                        init_row_feld_table_boostrap_table(name_table, data.d[0].Obj_ilist_fileds_generic, class_stru_row_Gabinete_Generic, "", "table-bordered", "table-borderless");
                        if (document.getElementById(name_control_padre)) {
                            $("#" + name_control_padre).modal("show");
                        }
                        resolve("YES");

                    }
                }, error: function (xception, textStatus, errorThrown) {

                    if (xception.status === 0) {
                        resolve("Not connect: Verify Network.");


                    } else if (xception.status == 404) {
                        resolve("Requested page not found [404]");


                    } else if (xception.status == 500) {
                        resolve("Internal Server Error [500]." + xception.responseText);


                    } else if (textStatus === 'parsererror') {
                        resolve("Requested JSON parse failed.");


                    } else if (textStatus === 'timeout') {
                        return "Time out error.";


                    } else if (textStatus === 'abort') {
                        resolve("Ajax request aborted.");


                    } else {
                        resolve("Ajax request aborted." + xception.responseText);


                    }
                }
            });
        }
        catch (ex) {
            resolve(ex.message);
        }
    })
    let result = await myPromise;
    return result;

}
//-------Solicita configuración plantilla raicacion
const Service_REST_Solicita_solicita_opciones_plantilla_radicacion = async (id_plantilla_radicado) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebService_radicacion_Simplificada.asmx/Service_solicita_opciones_plantilla_radicacion', {
                data: "{" + "'id_plantilla_radicado':'" + id_plantilla_radicado + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_result !== "YES") {
                        resolve(data.d[0].Error_result);
                    } else {
                        if (data.d[0].util_estado_pendiente_rad == 1) {
                            document.getElementById("Panel_pendiente_radicado").style.display = "flex";
                        } else {
                            document.getElementById("Panel_pendiente_radicado").style.display = "none";
                        }
                        resolve("YES");

                    }
                }, error: function (xception, textStatus, errorThrown) {

                    if (xception.status === 0) {
                        resolve("Not connect: Verify Network.");


                    } else if (xception.status == 404) {
                        resolve("Requested page not found [404]");


                    } else if (xception.status == 500) {
                        resolve("Internal Server Error [500]." + xception.responseText);


                    } else if (textStatus === 'parsererror') {
                        resolve("Requested JSON parse failed.");


                    } else if (textStatus === 'timeout') {
                        return "Time out error.";


                    } else if (textStatus === 'abort') {
                        resolve("Ajax request aborted.");


                    } else {
                        resolve("Ajax request aborted." + xception.responseText);


                    }
                }
            });
        }
        catch (ex) {
            resolve(ex.message);
        }
    })
    let result = await myPromise;
    return result;

}
//-------Solicita numero de tareas de radicación en pendientes 
const Service_REST_Solicita_solicita_numero_radicados_pendientes = async (id_plantilla_radicado) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebService_radicacion_Simplificada.asmx/Service_solicita_numero_radicados_pendientes', {
                data: "{" + "'id_plantilla_radicado':'" + id_plantilla_radicado + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].error_gestion !== "YES") {
                        resolve(data.d[0].error_gestion);
                    } else {
                        document.getElementById("Label_numero_item").innerText = data.d[0].total_pendiente;
                        resolve("YES");

                    }
                }, error: function (xception, textStatus, errorThrown) {

                    if (xception.status === 0) {
                        resolve("Not connect: Verify Network.");


                    } else if (xception.status == 404) {
                        resolve("Requested page not found [404]");


                    } else if (xception.status == 500) {
                        resolve("Internal Server Error [500]." + xception.responseText);


                    } else if (textStatus === 'parsererror') {
                        resolve("Requested JSON parse failed.");


                    } else if (textStatus === 'timeout') {
                        return "Time out error.";


                    } else if (textStatus === 'abort') {
                        resolve("Ajax request aborted.");


                    } else {
                        resolve("Ajax request aborted." + xception.responseText);


                    }
                }
            });
        }
        catch (ex) {
            resolve(ex.message);
        }
    })
    let result = await myPromise;
    return result;

}
//-------Registra flujo trabajo radicación simple
const Service_REST_registra_flujo_tarea_workflow_radicado_simple = async (id_registro_estado) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceWorkflow.asmx/Service_registra_flujo_tarea_workflow_radicado_simple', {
                data: "{" + "'id_registro_estado':'" + id_registro_estado + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_result !== "YES") {
                        resolve(data.d[0].Error_result);
                    } else {     
                        resolve("YES");
                    }
                }, error: function (xception, textStatus, errorThrown) {

                    if (xception.status === 0) {
                        resolve("Not connect: Verify Network.");


                    } else if (xception.status == 404) {
                        resolve("Requested page not found [404]");


                    } else if (xception.status == 500) {
                        resolve("Internal Server Error [500]." + xception.responseText);


                    } else if (textStatus === 'parsererror') {
                        resolve("Requested JSON parse failed.");


                    } else if (textStatus === 'timeout') {
                        return "Time out error.";


                    } else if (textStatus === 'abort') {
                        resolve("Ajax request aborted.");


                    } else {
                        resolve("Ajax request aborted." + xception.responseText);


                    }
                }
            });
        }
        catch (ex) {
            resolve(ex.message);
        }
    })
    let result = await myPromise;
    return result;

}
//-------Realiza la actualización del radicado en la lista de radicados pendientes 
const Service_REST_actualiza_estado_registro_radicado_pendiente = async (id_registro_estado, estado_radicado) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebService_radicacion_Simplificada.asmx/Service_actualiza_estado_registro_radicado_pendiente', {
                data: "{" + "'id_registro_estado':'" + id_registro_estado + "','" + "estado_radicado':'" + estado_radicado + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].error_gestion !== "YES") {
                        resolve(data.d[0].error_gestion);
                    } else {
                        resolve("YES");
                    }
                }, error: function (xception, textStatus, errorThrown) {

                    if (xception.status === 0) {
                        resolve("Not connect: Verify Network.");


                    } else if (xception.status == 404) {
                        resolve("Requested page not found [404]");


                    } else if (xception.status == 500) {
                        resolve("Internal Server Error [500]." + xception.responseText);


                    } else if (textStatus === 'parsererror') {
                        resolve("Requested JSON parse failed.");


                    } else if (textStatus === 'timeout') {
                        return "Time out error.";


                    } else if (textStatus === 'abort') {
                        resolve("Ajax request aborted.");


                    } else {
                        resolve("Ajax request aborted." + xception.responseText);


                    }
                }
            });
        }
        catch (ex) {
            resolve(ex.message);
        }
    })
    let result = await myPromise;
    return result;

}
//-------Solicita estado radicado asignado para gestión de documentos 
const Service_REST_solicita_estado_radicado_asignado_usuario_gestion_documentos = async (id_plantilla_radicado) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebService_radicacion_Simplificada.asmx/Service_solicita_estado_radicado_asignado_usuario_gestion_documentos', {
                data: "{" + "'id_plantilla_radicado':'" + id_plantilla_radicado + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].error_gestion !== "YES") {
                        resolve(data.d[0].error_gestion);
                    } else {
                        if (data.d[0].estado_asignado == "YES") {
                            resolve("Tarea asignada para gestión y asignación, debe terminar la tarea actual o subirla a estado pendiente para continuar con la asignación");
                        } else {
                            resolve("YES");
                        }
                    }
                }, error: function (xception, textStatus, errorThrown) {

                    if (xception.status === 0) {
                        resolve("Not connect: Verify Network.");


                    } else if (xception.status == 404) {
                        resolve("Requested page not found [404]");


                    } else if (xception.status == 500) {
                        resolve("Internal Server Error [500]." + xception.responseText);


                    } else if (textStatus === 'parsererror') {
                        resolve("Requested JSON parse failed.");


                    } else if (textStatus === 'timeout') {
                        return "Time out error.";


                    } else if (textStatus === 'abort') {
                        resolve("Ajax request aborted.");


                    } else {
                        resolve("Ajax request aborted." + xception.responseText);


                    }
                }
            });
        }
        catch (ex) {
            resolve(ex.message);
        }
    })
    let result = await myPromise;
    return result;

}
const clrear_view_radicado_simple = () => {
    try {
        let ref_titel_visor = document.getElementById("titel_visor");
        if (ref_titel_visor) {
            ref_titel_visor.innerHTML = "";
        }
        let ref_IframeVisor_ = document.getElementById("IframeVisor_");
        if (ref_IframeVisor_) {
            ref_IframeVisor_.setAttribute("SRC", "");
        }
        let ref_Area_Visor = document.getElementById("Area_Visor");
        if (ref_Area_Visor) {
            ref_Area_Visor.style.display = "none";
        }
        let ref_Are_Digitalizacion = document.getElementById("Are_Digitalizacion");
        if (ref_Are_Digitalizacion) {
            ref_Are_Digitalizacion.style.display = "block";
        }
        return "YES";
    } catch (ex) {
        return "funcion name clrear_view_radicado_simple : " + ex.mensaje;
    }
}
const auto_size_soporte_documental = (value_resize) => {
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
        //let heig_porcent = espacio_iframe - ((espacio_iframe * 2) / 100);
        let heig_porcent = espacio_iframe
        let heigth_element = heig_porcent - (document.getElementById('menu_content_radicacion_simplificada').clientHeight + 10);
        document.getElementById('div_soporte_documento').style.height = heigth_element + "px";
        let heigth_div_foter_registro_tramite = document.getElementById('foter_registro_tramite').clientHeight;
        if (heigth_div_foter_registro_tramite == 0) {
            heigth_div_foter_registro_tramite = 75;
        }
        document.getElementById('div_datos_registro_tramite').style.height = (heigth_element - heigth_div_foter_registro_tramite) + "px";
        //-------Asigna el valor de la altura de la tabla documentos relacionados  rad_simple_contentizquierdo  rad_simple_contenedorderecho 
        let heigth_div_title_soporte_documental = document.getElementById('div_title_soporte_documental').clientHeight;
        if (heigth_div_title_soporte_documental == 0) {
            heigth_div_title_soporte_documental = 1;
        }
        let heigth_navar_rad_simple_barra = document.getElementById('navar_rad_simple_barra').clientHeight;
        if (heigth_navar_rad_simple_barra == 0) {
            heigth_navar_rad_simple_barra = 50;
        }
        let heigth_div_rad_simple_footer = document.getElementById('div_rad_simple_footer').clientHeight;
        if (heigth_div_rad_simple_footer == 0) {
            heigth_div_rad_simple_footer = 50;
        }
        let heigth_element_content_izq = heigth_div_title_soporte_documental + heigth_navar_rad_simple_barra + heigth_div_rad_simple_footer;
        document.getElementById('conte_rad_simpe_table_waper').style.height = (heigth_element - heigth_element_content_izq) + "px";
        document.getElementById('rad_simple_content_wraper').style.height = (heigth_element - heigth_element_content_izq) + "px";
        document.getElementById('rad_simple_contentizquierdo').style.height = (heigth_element - heigth_element_content_izq) + "px";
        document.getElementById('rad_simple_contenedorderecho').style.height = (heigth_element - heigth_element_content_izq) + "px";
        let heigth_div_rad_simple_title = document.getElementById('div_rad_simple_title').clientHeight;
        if (heigth_div_rad_simple_title == 0) {
            heigth_div_rad_simple_title = 1;
        }
        let heigth_div_rad_simple_contenido_pie = document.getElementById('div_rad_simple_contenido_pie').clientHeight;
        if (heigth_div_rad_simple_contenido_pie == 0) {
            heigth_div_rad_simple_contenido_pie = 20;
        }
        let heigth_element_content_izq_table = heigth_div_rad_simple_title + heigth_div_rad_simple_contenido_pie;
        let heigth_content_izq = document.getElementById('rad_simple_contentizquierdo').clientHeight;
        let heigth_table = heigth_content_izq - heigth_element_content_izq_table;
        let heigth_content_der = document.getElementById('rad_simple_contenedorderecho').clientHeight;
        document.getElementById('div_rad_simple_content_table').style.height = (heigth_table - 5) + "px";
        document.getElementById('Are_Digitalizacion').style.height = (heigth_content_der - 5) + "px";
       
        let heigth_div_cerrar_rad_simple = document.getElementById("div_cerrar_rad_simple").clientHeight;
        if (heigth_div_cerrar_rad_simple == 0) {
            heigth_div_cerrar_rad_simple = 40;
        }
        document.getElementById('Area_Visor').style.height = (heigth_content_der - heigth_div_cerrar_rad_simple) + "px";
        let HeigthIframe = espacio_iframe - (document.getElementById('menu_content_radicacion_simplificada').style.height + document.getElementById('navar_rad_simple_barra').style.height + document.getElementById('div_rad_simple_footer').style.height);
        if (value_resize == 1) {
            table_reize_heigth("table_doc_flow_select", (heigth_table - 5), "table-borderless");
        }
    }
    catch (err) {
        alert(err.message + " Funcion auto_size_soporte_documental");
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
function rezize_event() {
    try {
        auto_size_soporte_documental(1);
    } catch (ex) {
        alert(ex.message + " Función rezize_event")
    }
}