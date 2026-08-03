
$(document).ready(function () {
        //auto_zise_page_();
        auto_zise_popup_paginas_externas_libres();
        GetLista_listado_usuarios_workflow_ruta_asignacion("Text_usuario_workflow");
        //resize_table_boot();
        $('#table').on('page-change.bs.table', function (e, arg1, arg2) {
           //resize_table_boot();
       })
      $('#table').on('all.bs.table', function (e, arg1, arg2) {
          //resize_table_boot();
       })
    
})
let ESTADO_EVENT_GENERAL = "";
let INTERVAL_EVENT_GENERAL;
let DATOS_REGISTRO_RUE_SII;
let RECIBO_VIRTUAL_SII;

$(window).on("load", function () {
    try {   
        let elment = document.getElementsByClassName("da_event_captive");
        if (elment) {
            for (var i = 0; i < elment.length; i++) {
                elment[i].addEventListener("click", event_click, false);
            }
        }
        //agrega evento click a los botones  
        auto_zise_page_();
        ini_event_page();
        ini_config_control();
        load_page_gestion_flujo();
        window.addEventListener("resize", rezize_event);
        ShowModalPopup("ModalPopupExtender_mensaje_personalizado_backgroundElement", "Panel_mensaje_personalizado", 100001);
       
    } catch (e) {
        alert(" funcion load " + e.message);
    }

});
//--------Iniciaiza todas las interfaz de inicicio de la radicación simple
const load_page_gestion_flujo = async () => {
    try {
        posicion_update_pogres('progres_bar');
        let result = "";
        result = await config_interfaz_permisos();
        if (result !== "YES") {
            alert_bot(result, 'warning', "error_div_error_general");
            return true;
        }
    } catch (ex) {
        alert_bot(ex.message, 'warning', "error_div_error_general");
    } finally {
        progres_hiden('progres_bar');
    }
}
const ini_event_page = () => {
    let array_element = new Array;
    array_element.push({ id: "Button_consultar_radicado" }, { id: "Button_reasigna_sii" }, { id: "Button_consultar_recibo_sii_rut" }, { id: "Button_registro_actividad_ruta" },
        { id: "Button_consultar_recibo_sii_flujo" }, { id: "Button_registro_actividad_flujo" }, { id: "Button_elimina_tarea" },
        { id: "Button_edita_flujo" }, { id: "Button_load_file_rue" }, { id: "Button_registro_actividad_flujo_tarea_sii" }, { id: "Button_load_file_virtual" },
        { id: "Button_registro_actividad_flujo_tarea_virtual_sii" }, { id: "Button_consultar_usuario"});
    for (let i = 0; i < array_element.length; i++) {
        let elment_a_document_production = document.getElementById(array_element[i].id);
        if (elment_a_document_production) {
            elment_a_document_production.addEventListener("click", handler_element_event, false);
        }
    }
    //------Agrega los eventos onchange de los html option
    array_element = new Array;
    array_element.push({ id: "DropDownList_flujos_tarea_sii" },{ id: "DropDownList_list_actividad_workflow_sii" }, { id: "DropDownList_actividades_flujo" }, { id: "DropDownList_flujos" },{ id: "DropDownList_tramites_flujo_tarea_sii"});
    for (let i = 0; i < array_element.length; i++) {
        let elment_a_document_production = document.getElementById(array_element[i].id);
        if (elment_a_document_production) {
            elment_a_document_production.addEventListener("change", event_onchage_option_async, false);
        } 
    }
    show_popup_general();
}
const handler_element_event = (e) => {
    try {
        let name_ID = e.currentTarget.id;
        let result = "";
        let name_espace_class
        delete_alert_boot();
        switch (name_ID) {
            //Caso consulta usuario para inativacion
            case "Button_consultar_usuario":
                event_element_click_promise(e);
                break;
            //caso registra tarea tramite virtual
            case "Button_registro_actividad_flujo_tarea_virtual_sii":
                event_element_click_promise(e);
                break;
            //caso sube file virtual
            case "Button_load_file_virtual" :
                event_element_click_promise(e);
                break;
            //caso registra flujo traje sii
            case "Button_registro_actividad_flujo_tarea_sii":
                event_element_click_promise(e);
                break;
            //Caso prueba sube rue
            case "Button_load_file_rue":
                event_element_click_promise(e);
                break;
            //Caso actualiza imagen tarea ruta
            case "Button_edita_flujo":
                event_element_click_promise(e);
                break;
            //Caso elimina tarea workflow
            case "Button_elimina_tarea":
                event_element_click_promise(e);
                break;
            //Caso consulta recibo flujo SII
            case "Button_consultar_recibo_sii_flujo":
                event_element_click_promise(e);
                break;
            //Caso registro tarea ruta  de trabajo SII
            case "Button_registro_actividad_ruta":
                event_element_click_promise(e);
                break;
            //Caso registro tarea flujo  de trabajo SII
            case "Button_registro_actividad_flujo":
                event_element_click_promise(e);
                break;
            //Caso consulta recibo ruta SII
            case "Button_consultar_recibo_sii_rut":
                event_element_click_promise(e);
                break;
            //Caso consulta codigo barras tadicado SII para estado tarea
            case "Button_consultar_radicado":
                event_element_click_promise(e);
                break;
            //Caso reasigna tarea usuario  SII workflow 
            case "Button_reasigna_sii":
                event_element_click_promise(e);
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
        if (name_control == "Button_consultar_usuario") {
            let usuario_workflow = document.getElementById("Text_usuario_workflow").value;
            if (usuario_workflow == "") {
                alert_bot("Debe informar el usuario", 'warning', "error_div_gestion_balance");
                document.getElementById("Text_usuario_workflow").focus();
                return true;
            }
            result = await Service_Solicita_lista_usuarios_workflow_balanceo(usuario_workflow);
            if (result != "YES") {
                alert_bot(result, 'warning', "error_div_gestion_balance");
                return true;
            }
        }
        if (name_control == "Button_consultar_radicado") {
            let radicado = document.getElementById("Text_codigo_barra_sii").value;
            if (radicado == "") {
                alert_bot("Debe informar el codigo de barras", 'warning', "error_div_reasigna_sii");
                document.getElementById("Text_codigo_barra_sii").focus();
                return true;
            }
            result = await Service_REST_lista_estado_tarea_asignacion(radicado);
            if (result != "YES") {
                alert_bot(result, 'warning', "error_div_reasigna_sii");
                return true;
            }
        }
        if (name_control == "Button_reasigna_sii") {
            var value_drow_goup = $("#" + "DropDownList_list_actividad_workflow_sii").val();
            if (value_drow_goup == -1 || value_drow_goup == 0) {
                alert_bot("Debe seleccionar la actividad", 'warning', "error_div_reasigna_sii_popup");
                document.getElementById("DropDownList_list_actividad_workflow_sii").focus();
                return true;
            }
            var value_drow_user = $("#" + "DropDownList_list_usuario_workflow_sii").val();
            if (value_drow_user == -1 || value_drow_user == 0) {
                alert_bot("Debe seleccionar el usuario a reasignar", 'warning', "error_div_reasigna_sii_popup");
                document.getElementById("DropDownList_list_usuario_workflow_sii").focus();
                return true;
            }
            let change_estate_sii = 1;
            if (document.getElementById("cheked_cambia_estado_sii").checked == false) {
                change_estate_sii = 0;
            }
            result = await Service_REST_reasigna_tarea_usuario_sii_workflow(ID_TAREA_WORKFLOW_WF, value_drow_goup, value_drow_user, change_estate_sii);
            if (result != "YES") {
                alert_bot(result, 'warning', "error_div_reasigna_sii_popup");
                return true;
            }

        }
        if (name_control == "Button_consultar_recibo_sii_rut") {
            let radicado = document.getElementById("TextBox_recibo_caja_rut").value;
            if (radicado == "") {
                alert_bot("Por favor informe el recibo", 'warning', "error_div_registro_ruta");
                return true;
            }
            let ElemntHtmlOption = document.getElementById("DropDownList_ante_pone_rut");
            let apost_LETER = ElemntHtmlOption.options[ElemntHtmlOption.selectedIndex].text;
            radicado = zeroFillFReciboSII(apost_LETER, radicado);
            document.getElementById("TextBox_recibo_caja_rut").value = radicado;
            result = await Service_REST_solicita_datos_estructura_consulta_recibo_ruta_interfaz_SII(radicado);
            if (result != "YES") {
                alert_bot(result, 'warning', "error_div_registro_ruta");
                return true;
            } 
        }
        if (name_control == "Button_consultar_recibo_sii_flujo") {
            let radicado = document.getElementById("TextBox_recibo_caja_flujo").value;
            if (radicado == "") {
                alert_bot("Por favor informe el recibo", 'warning', "error_div_registro_flujo");
                return true;
            }
            let ElemntHtmlOption = document.getElementById("DropDownList_ante_pone_flujo");
            let apost_LETER = ElemntHtmlOption.options[ElemntHtmlOption.selectedIndex].text;
            radicado = zeroFillFReciboSII(apost_LETER, radicado);
            document.getElementById("TextBox_recibo_caja_flujo").value = radicado;
            result = await Service_REST_solicita_datos_estructura_consulta_recibo_flujo_interfaz_SII(radicado);
            if (result != "YES") {
                alert_bot(result, 'warning', "error_div_registro_flujo");
                return true;
            }
        }
        if (name_control == "Button_registro_actividad_ruta") {
            result = await valida_solicita_datos_control_general_async("conten_registro_ruta");     
            if (result != "YES") {
                alert_bot(result, 'warning', "error_div_registro_ruta");
                return true;
            }
            result = await Service_REST_Registra_tarea_ruta_SII(ITEM_GENERAL_CONTROL_ARRAY);
            if (result != "YES") {
                alert_bot(result, 'warning', "error_div_registro_ruta");
                return true;
            } else {
                restore_value_form_control("conten_registro_ruta");
            }   
        }
        if (name_control == "Button_registro_actividad_flujo") {
            result = await valida_solicita_datos_control_general_async("conten_registro_flujo");
            console.log(ITEM_GENERAL_CONTROL_ARRAY);
            if (result != "YES") {
                alert_bot(result, 'warning', "error_div_registro_flujo");
                return true;
            }   
            result = await Service_REST_Registra_tarea_flujo_SII(ITEM_GENERAL_CONTROL_ARRAY);
            if (result != "YES") {
                alert_bot(result, 'warning', "error_div_registro_flujo");
                return true;
            } else {
                restore_value_form_control("conten_registro_flujo");
               
            }
            
        }
        if (name_control == "Button_elimina_tarea") {
            let radicado = document.getElementById("TextBox_recibo_caja_elimina_flujo").value;
            if (radicado == "") {
                alert_bot("Por favor informe el recibo", 'warning', "error_div_elimina_flujo");
                return true;
            }
            let ElemntHtmlOption = document.getElementById("DropDownList_ante_pone_elimina_flujo");
            let apost_LETER = ElemntHtmlOption.options[ElemntHtmlOption.selectedIndex].text;
            radicado = zeroFillFReciboSII(apost_LETER, radicado);
            document.getElementById("TextBox_recibo_caja_elimina_flujo").value = radicado;
            result = await Service_REST_eliminar_flujo_workflow_SII(radicado);
            if (result != "YES") {
                alert_bot(result, 'warning', "error_div_elimina_flujo");
                return true;
            }
        }      
        if (name_control == "Button_edita_flujo") {
            let radicado = document.getElementById("TextBox_recibo_caja_edita_flujo").value;
            if (radicado == "") {
                alert_bot("Por favor informe el recibo", 'warning', "error_div_edita_flujo");
                return true;
            }
            let ElemntHtmlOption = document.getElementById("DropDownList_ante_pone_edita_flujo");
            let apost_LETER = ElemntHtmlOption.options[ElemntHtmlOption.selectedIndex].text;
            radicado = zeroFillFReciboSII(apost_LETER, radicado);
            document.getElementById("TextBox_recibo_caja_edita_flujo").value = radicado;
            result = await Service_REST_actualiza_datos_imagen_workflow_SII(radicado);
            if (result != "YES") {
                alert_bot(result, 'warning', "error_div_edita_flujo");
                return true;
            }
        }
        if (name_control == "Button_load_file_rue") {
            let Option = {
                funcion_name: "adjunta_archivo_rue_sii", evento_adjunta: "INTRUESII", element_parent: "modal_adjunta_archivo_rue_sii",
                element_html_table: "table_gestion_rue", element_html_lab_conteo: "", apost_html_lab_conteo: "",
                element_drow_list_tipo: "", name_class_serivce_list: "",
                name_serivce_list: "", element_parent_html_table: "div_content_tabla_gestion_rue"
            };
            IniLoadPerson(Option);        
        }
        if (name_control == "Button_registro_actividad_flujo_tarea_sii") {
            result = await valida_solicita_datos_control_general_async("conten_registro_flujo_tarea_sii");  
            if (result != "YES") {
                alert_bot(result, 'warning', "error_content_rue_registro_vitual_sii");
                return true;
            }
            let Class_Integracion_SII_registro_tarea_flujo = new Array;
            Class_Integracion_SII_registro_tarea_flujo.push({
                id_flujo: DATOS_REGISTRO_RUE_SII.id_flujo, id_usuario_workflow: DATOS_REGISTRO_RUE_SII.id_usuario_workflow,
                id_ruta: DATOS_REGISTRO_RUE_SII.id_ruta, id_grupo_workflow: DATOS_REGISTRO_RUE_SII.id_grupo_workflow,
                id_actividad_workflow: DATOS_REGISTRO_RUE_SII.id_actividad_workflow, id_actividad_flujo: DATOS_REGISTRO_RUE_SII.id_actividad_flujo,
                Class_config_general_service: ITEM_GENERAL_CONTROL_ARRAY, id_usuario_workflow_transacion: DATOS_REGISTRO_RUE_SII.id_usuario_workflow_transacion,
                codigo_rue: DATOS_REGISTRO_RUE_SII.class_row_rue_sii[0].CODIGOSERVCIORUE, option_registra_log:1
            });
            result = await Service_REST_registro_flujo_trabajo_sii_rue(Class_Integracion_SII_registro_tarea_flujo, DATOS_REGISTRO_RUE_SII.class_row_rue_sii[0].RECIBO);
            if (result != "YES") {
                alert_bot(result, 'warning', "error_content_rue_registro_vitual_sii");
                return true;
            } else {
                Hide_clear_interfaz_registro_tarea_sii_rue();
            }       
        }
        if (name_control == "Button_registro_actividad_flujo_tarea_virtual_sii") {
            result = await valida_solicita_datos_control_general_async("conten_registro_flujo_tarea_sii");
            if (result != "YES") {
                alert_bot(result, 'warning', "error_content_rue_registro_vitual_sii");
                return true;
            }
            let Class_Integracion_SII_registro_tarea_flujo = new Array;
            Class_Integracion_SII_registro_tarea_flujo.push({
                id_flujo: DATOS_REGISTRO_RUE_SII.id_flujo, id_usuario_workflow: DATOS_REGISTRO_RUE_SII.id_usuario_workflow,
                id_ruta: DATOS_REGISTRO_RUE_SII.id_ruta, id_grupo_workflow: DATOS_REGISTRO_RUE_SII.id_grupo_workflow,
                id_actividad_workflow: DATOS_REGISTRO_RUE_SII.id_actividad_workflow, id_actividad_flujo: DATOS_REGISTRO_RUE_SII.id_actividad_flujo,
                Class_config_general_service: ITEM_GENERAL_CONTROL_ARRAY, id_usuario_workflow_transacion: DATOS_REGISTRO_RUE_SII.id_usuario_workflow_transacion,
                codigo_rue: "", option_registra_log: 1
            });  
            result = await Service_REST_registro_flujo_trabajo_sii_virtual(Class_Integracion_SII_registro_tarea_flujo, DATOS_REGISTRO_RUE_SII.class_row_virtual_sii[0].CODIGOBARRAS);
            if (result != "YES") {
                alert_bot(result, 'warning', "error_content_rue_registro_vitual_sii");
                return true;
            } else {
                Hide_clear_interfaz_registro_tarea_sii_rue();
            }
        }
        if (name_control == "Button_load_file_virtual") {
            let Option = {
                funcion_name: "adjunta_archivo_virtual_sii", evento_adjunta: "INTVIRTUALSII", element_parent: "modal_adjunta_archivo_virtual_sii",
                element_html_table: "table_gestion_virtual", element_html_lab_conteo: "", apost_html_lab_conteo: "",
                element_drow_list_tipo: "", name_class_serivce_list: "",
                name_serivce_list: "", element_parent_html_table: "div_content_tabla_gestion_virtual"
            };
            IniLoadPerson(Option);
        }
    } 
    catch (ex) {
        alert_bot(ex.message, 'warning', "error_div_error_general");
    } finally {
        document.getElementById(name_control).disabled = false;
        progres_hiden('progres_bar');
        
    }
}

function rezize_event() {
    try {
        auto_zise_page_();
        resize_table_boot("table_balance");
        resize_table_boot("table");
        auto_zise_popup_paginas_externas_libres();
        auto_zise_lista_documentos_sii();
        
    } catch (ex) {
        alert(ex.message + " Función rezize_event")
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
const show_popup_general = () => {
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl)
    })
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
                //evento consulta radicado 
                if (evento == "C-RAD-SII__") {
                    let radicado = document.getElementById("Text_codigo_barra_sii").value;
                    if (radicado == "") {
                        finaly_event_element_menu();
                        alert("Debe informar el codigo de barras");
                        document.getElementById("Text_codigo_barra_sii").focus();
                        return true;
                    }
                    Service_lista_estado_tarea_asignacion(radicado);
                    return true;      
                }
                //evento activa lista actividades
                if (evento == "A-REA-SII") {
                    if (ID_TAREA_WORKFLOW_WF == 0) {
                        finaly_event_element_menu();
                        alert("Debe consultar y listar el codigo de barras");
                        return true;
                    }
                    service_lista_actividades_workflow(ID_TAREA_WORKFLOW_WF);
                    return true;
                }
                //Evento lista usuarios grupo workflow
                if (evento == "L-USER-WF_") {
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
                if (evento == "R-REA-SII_") {
                    var value_drow_goup = $("#" + "DropDownList_list_actividad_workflow_sii").val();
                    if (value_drow_goup == -1 || value_drow_goup == 0) {
                        document.getElementById("DropDownList_list_actividad_workflow_sii").focus();
                        finaly_event_element_menu();
                        return true;
                    }
                    var value_drow_user = $("#" + "DropDownList_list_usuario_workflow_sii").val();
                    if (value_drow_user == -1 || value_drow_user == 0) {
                        alert("Seleccion el usuario a reasignar");
                        document.getElementById("DropDownList_list_usuario_workflow_sii").focus();
                        finaly_event_element_menu();
                        return true;
                    }
                    let change_estate_sii = 1;
                    if (document.getElementById("cheked_cambia_estado_sii").checked == false) {
                        change_estate_sii = 0;
                    }
                    Service_reasigna_tarea_usuario_sii_workflow(ID_TAREA_WORKFLOW_WF, value_drow_goup, value_drow_user, change_estate_sii);
                    return true;
                }
                //evento lista usuarios workflow para balanceo
                //evento activa lista actividades
                if (evento == "C-CONS-USER__") {
                    let parameter = document.getElementById("Text_usuario_workflow").value;       
                    Service_Solicita_lista_usuarios_workflow_balanceo(parameter);
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
//-----------------------------------------------
//zona eventos  onchange
//-----------------------------------------------
const event_onchage_option_async = async (e) => {
    let name_control = e.currentTarget.id;
    try {
        let result = "";
        delete_alert_boot();
        posicion_update_pogres('progres_bar');
        //------Evento lista usuarios relacionados a la actividad
        if (name_control == "DropDownList_list_actividad_workflow_sii") {
            let control_option_html = document.getElementById(name_control);
            let value_drow = control_option_html.options[control_option_html.selectedIndex].value;
            if (value_drow == -1 || value_drow == 0) {
                $("#DropDownList_list_usuario_workflow_sii").empty();
                return true;
            }
            result = await Service_REST_lista_usuario_relacionado_actividad(value_drow, ID_TAREA_WORKFLOW_WF);
            if (result !== "YES") {
                alert_bot(result, 'warning', "error_div_reasigna_sii_popup");
            }
            return true;
        }
        //-----Evento lista actividades relacionadas flujo  
        if (name_control == "DropDownList_flujos") {
            let control_option_html = document.getElementById(name_control);
            let value_drow = control_option_html.options[control_option_html.selectedIndex].value;
            if (value_drow == -1 || value_drow == 0) {
                $("#DropDownList_actividades_flujo").empty();
                $("#DropDownList_usurios_flujo").empty();
                return true;
            }
            result = await Service_REST_solicita_actividades_workflow_flujo_inicio(value_drow);
            if (result !== "YES") {
                alert_bot(result, 'warning', "error_div_registro_flujo");
            }
            return true;
        }
        //-----Evento lista usuarios relacionados actividad flujo
        if (name_control == "DropDownList_actividades_flujo") {
            let control_option_html = document.getElementById(name_control);
            let value_drow = control_option_html.options[control_option_html.selectedIndex].value;
            if (value_drow == -1 || value_drow == 0) {
                $("#DropDownList_usurios_flujo").empty();
                return true;
            }
            result = await Service_REST_solicita_usuarios_relacionados_actividad_flujo(value_drow);
            if (result !== "YES") {
                alert_bot(result, 'warning', "error_div_registro_flujo");
            }
            return true;
        }
        //-----Evento valida lista actividades de ruta o flujo de trabajo
        if (name_control == "DropDownList_tramites_flujo_tarea_sii") {
            let control_option_html = document.getElementById(name_control);
            let value_drow = control_option_html.options[control_option_html.selectedIndex].value;
            if (value_drow != "0") {
                let split = value_drow.split("|");
                result = await validate_relacion_tramite_flujo(split[1]);
                return result;
            } else {
                result = await validate_relacion_tramite_flujo("");
                return result;
            }
            return true;
        }
        //------Evento lista actividades usuario flujo trabajo
        if (name_control == "DropDownList_flujos_tarea_sii") {     
            let control_option_html = document.getElementById(name_control);
            let value_drow = control_option_html.options[control_option_html.selectedIndex].value;
            if (value_drow == "0" || value_drow == "-1") {
                $("#DropDownList_actividades_flujo_sii").empty();
                return true;
            }
            result = await Service_REST_solicita_actividades_fjujo_usuario(value_drow, DATOS_REGISTRO_RUE_SII.id_actividad_workflow);
            if (result != "YES") {
                alert_bot(result, 'warning', "error_content_rue_registro_vitual_sii");
                return true;
            }       
        }
     }
     catch (ex) {
        alert_bot(ex.message, 'warning', "error_div_error_general");
    } finally {
            progres_hiden('progres_bar');
    }
}
function add_event_element_option(class_element) {
    //---Registra evento onchange de option
    try {
        var elment = document.getElementsByClassName(class_element);
        if (elment) {
            for (var i = 0; i < elment.length; i++) {
                elment[i].addEventListener("change", event_onchange, false);
            }
        }
    } catch (ex) {
        alert("incosistencia funciton add_event_element_option " + ex.mensaje);
    }
}
function event_onchange(e) {
    //----evento changue
    try {
        let name_elemnt_event = e.currentTarget.id;
        let element_event = document.getElementById(name_elemnt_event);
        if (element_event == null) {
            alert("Imposible encontar el control (" + name_elemnt_event + ")");
            return true;
        }
        let name_evento = element_event.getAttribute("name_event");
        let value_evento = element_event.getAttribute("value_event");
        event_element_menu(name_evento, value_evento);
    } catch (ex) {
        alert("Inconsistencia event_onchange " + ex.mensaje);
    }
}
//-----------------------------------------------
//zona eventos  botoon
//-----------------------------------------------
function add_event_element_booton(class_element) {
    //---Registra evento boton
    try {
        var elment = document.getElementsByClassName(class_element);
        if (elment) {
            for (var i = 0; i < elment.length; i++) {
                elment[i].addEventListener("click", event_botom_click, false);
            }
        }
    } catch (ex) {
        alert("incosistencia funciton add_event_element_booton " + ex.mensaje);
    }
}
function event_botom_click(e) {
    //---Evento boton 
    try {
        let name_elemnt_event = e.currentTarget.id;
        let element_event = document.getElementById(name_elemnt_event);
        if (element_event == null) {
            alert("Imposible encontar el control (" + name_elemnt_event + ")");
            return true;
        }
        let name_evento = element_event.getAttribute("name_event");
        let value_evento = element_event.getAttribute("value_event");
        event_element_menu(name_evento, value_evento);
    } catch (ex) {
        alert('event_element_menu  ' + ex.message);
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
let REGISTRO;
let ID_TAREA_WORKFLOW_WF = 0;
function mystopfunction_update_table(name_table, error) {
    try {
        finaly_event_element_menu();
        var $table = $('#' + name_table)
        if (error !== "") {
            ID_TAREA_WORKFLOW_WF = 0;
            $table.bootstrapTable('removeAll');
            resize_table_boot(name_table);
            if (error !== "ZERO") {
                alert(error);
            }
        } else {
            ID_TAREA_WORKFLOW_WF = REGISTRO[0].ID_TAREA;
            $table.bootstrapTable('destroy').bootstrapTable({ data: REGISTRO });
            resize_table_boot(name_table);
        }
        
    } catch (err) {
        alert(err.message + " Funcion mystopfunction_event_lista_radicado");
    } 
}
function mystopfunction_update_table_balance(name_table, error, name_contaniner) {
    try {
        finaly_event_element_menu();
        var $table = $('#' + name_table)
        if (error !== "") {
            $table.bootstrapTable('removeAll');
            resize_table_boot(name_table);
            if (error !== "ZERO") {
                alert(error);
            }     
        } else {   
            $table.bootstrapTable('destroy').bootstrapTable({ data: REGISTRO });
            resize_table_boot(name_table);
        }

    } catch (err) {
        alert(err.message + " Funcion mystopfunction_update_table_balance");
    }
}
///------Funcion que configura la interfaz según permisos de usuario workfow---////
let permisos_integracion_sii;
const config_interfaz_permisos = async () => {
    let myPromise = new Promise(async (resolve) => {
    try {
       let result = await Service_REST_solicita_permisos_usuario_workflow_intgracion_sii(0);
        if (result !== "YES") {
            resolve(result);     
        }
        let suitch = 0;
        if (permisos_integracion_sii.util_sii_registro_tarea_flujo == 1) {
            document.getElementById("util_sii_registro_tarea_flujo").classList.remove("d-none");
            document.getElementById("registro_flujo").classList.remove("d-none");
            if (suitch == 0) {
                suitch = 1;
                document.getElementById("util_sii_registro_tarea_flujo").firstChild.click();
            }
        }
        if (permisos_integracion_sii.util_sii_registro_tarea_ruta == 1) {
            document.getElementById("util_sii_registro_tarea_ruta").classList.remove("d-none");
            document.getElementById("registro_ruta").classList.remove("d-none");
            if (suitch == 0) {
                suitch = 1;
                document.getElementById("util_sii_registro_tarea_ruta").firstChild.click();
            }
        }
        if (permisos_integracion_sii.util_sii_getion_tarea == 1) {
            document.getElementById("util_sii_getion_tarea").classList.remove("d-none");
            document.getElementById("elimina_flujo").classList.remove("d-none");
            if (suitch == 0) {
                suitch = 1;
                document.getElementById("util_sii_getion_tarea").firstChild.click();
            }
        }
        
        if (permisos_integracion_sii.util_sii_getion_tarea == 1) {
            document.getElementById("util_sii_getion_tarea_").classList.remove("d-none");
            document.getElementById("edita_flujo").classList.remove("d-none");
            if (suitch == 0) {
                suitch = 1;
                document.getElementById("util_sii_getion_tarea_").firstChild.click();
            }
        }
        if (permisos_integracion_sii.util_reasigna_tarea_workflow_sii == 1) {
            document.getElementById("util_reasigna_tarea_workflow_sii").classList.remove("d-none");
            document.getElementById("tab_reasigna_sii").classList.remove("d-none");
            if (suitch == 0) {
                suitch = 1;
                document.getElementById("util_reasigna_tarea_workflow_sii").firstChild.click();
            }
        }
        if (permisos_integracion_sii.util_gestion_reasing_user == 1) {
            document.getElementById("util_gestion_reasing_user").classList.remove("d-none");
            document.getElementById("tab_gestion_balance").classList.remove("d-none");
            if (suitch == 0) {
                suitch = 1;
                document.getElementById("util_gestion_reasing_user").firstChild.click();
            }
        }
        if (permisos_integracion_sii.util_sii_gestion_tarea_rue == 1) {
            document.getElementById("util_sii_gestion_tarea_rue").classList.remove("d-none");
            document.getElementById("tab_gestion_rue").classList.remove("d-none");
            if (suitch == 0) {
                suitch = 1;
                document.getElementById("util_sii_gestion_tarea_rue").firstChild.click();
            }
        }
        if (permisos_integracion_sii.util_sii_gestion_tarea_virtual == 1) {
            document.getElementById("util_sii_gestion_tarea_virtual").classList.remove("d-none");
            document.getElementById("tab_gestion_virtual").classList.remove("d-none");
            if (suitch == 0) {
                suitch = 1;
                document.getElementById("util_sii_gestion_tarea_virtual").firstChild.click();
            }
        }
        resolve("YES");
    } catch (ex) {
        resolve(ex.message + " funcion config_interfaz_permisos");
    }
    })
    let result = await myPromise;
    return result;
}
function openCity(evt, cityName) {
    var i, tabcontent, tablinks;
    tabcontent = document.getElementsByClassName("tabcontent");
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    document.getElementById(cityName).style.display = "block";
    evt.currentTarget.className += " active";
    auto_zise_page_();
}
//-------------------------------------------------------------------------------------------------------
//                                     zona eventos  gestion rues SII
//-------------------------------------------------------------------------------------------------------
//-----------Despliega el listado de rues a importar-------////
const Show_row_table_boot_rue = (ob_row_table, obj_field_boot_table, name_table, name_parent_table) => {
    let class_stru_row_Gabinete_Generic = JSON.parse(ob_row_table);
    init_row_feld_table_boostrap_table(name_table, obj_field_boot_table, class_stru_row_Gabinete_Generic, name_parent_table, "table-bordered", "table-borderless");
    $("#modal_adjunta_archivo_rue_sii").modal("hide");

}
//-----------Despliega el listado de documentos del sii-------////
const Show_activa_lista_documentos_raicado_sii = async (radicao_sii) => {
    try {
        let result = "";
        delete_alert_boot();
        posicion_update_pogres('progres_bar');
        result = await Service_REST_solicita_lista_archivos_relacionados_radicado_sii(radicao_sii, "tabl_lista_sii_rues_file", "content_tabl_lista_sii_rues_file");
        if (result != "YES") {
            alert_bot(result, 'warning', "error_div_gestion_rue");
        }
    }
    catch (ex) {
        alert_bot(ex.message, 'warning', "error_div_gestion_rue");
    }
    finally {
        progres_hiden('progres_bar');
    }
}
//-----------Despliega el listado de documentos del sii-------////
const Show_activa_lista_documentos_raicado_virtual = async (radicao_sii) => {
    try {
        let result = "";
        delete_alert_boot();
        posicion_update_pogres('progres_bar');
        result = await Service_REST_solicita_lista_archivos_relacionados_radicado_sii(radicao_sii, "tabl_lista_sii_rues_file", "content_tabl_lista_sii_rues_file");
        if (result != "YES") {
            alert_bot(result, 'warning', "error_div_gestion_virtual");
        }
    }
    catch (ex) {
        alert_bot(ex.message, 'warning', "error_div_gestion_virtual");
    }
    finally {
        progres_hiden('progres_bar');
    }
}
//-----------Despliega la visualización de documento SII-------////
const Show_activa_visualzar_imagen_sii = async (radicao_sii, url, formato, observaciones) => {
    try {
        let result = "";
        delete_alert_boot();
        posicion_update_pogres('progres_bar');
        if (formato == "pdf") {
            document.getElementById("Iframe_view_sii_rues_file").src = url;
        } else {
            document.getElementById("Iframe_view_sii_rues_file").src = "";
            alert_down_load_file(url, "", observaciones ,"primary", "down_lad_file_sii_rues","");
        }    
    }
    catch (ex) {
        alert_bot(ex.message, 'warning', "error_content_sii_rues_file");
    }
    finally {
        progres_hiden('progres_bar');
    }
}

//----------Agrega zero fill al recibo SII-----///
const zeroFillFReciboSII = (asposT_letter, Recibo) => {

    let zeroFill = "-";
    let index = Recibo.indexOf(asposT_letter);
    if (index != -1) {
        return Recibo;
    }
    let recibo_length = Recibo.length;

    switch (recibo_length) {
        case 1:
            zeroFill = "00000000";
            break;
        case 2:
            zeroFill = "0000000";
            break;
        case 3:
            zeroFill = "000000";
            break;
        case 4:
            zeroFill = "00000";
            break;
        case 5:
            zeroFill = "0000";
            break;
        case 6:
            zeroFill = "000";
            break;
        case 7:
            zeroFill = "00";
            break;
        case 8:
            zeroFill = "0";
            break;
    }

    let out_ZeroFill = asposT_letter + zeroFill + Recibo;
    return out_ZeroFill;
}
//----------Activia la interfaz de registro de una tarea RUE SII-----///
const Show_activa_registro_tarea_rue_sii = async (obje_jon) => {
   try {
        let result = "";
        delete_alert_boot();
        posicion_update_pogres('progres_bar');
        let parameter = new Array;
        let CODIGOSERVCIORUE = ""; 
        if (obje_jon.SERVICIO != "") {
           let spli = obje_jon.SERVICIO.split("-");
            CODIGOSERVCIORUE = spli[0].trim();
       }
       parameter.push({ RECIBO: obje_jon.RECIBO ,  NOMBRE: obje_jon.NOMBRE ,  SERVICIO: obje_jon.SERVICIO ,  ESTADO: obje_jon.ESTADO ,  CODIGOSERVCIORUE: CODIGOSERVCIORUE });   
       result = await Service_REST_Solicita_datos_registro_flujo_rue_SII(parameter);
       if (result != "YES") {
           alert_bot(result, 'warning', "conten_gestion_rue");
       }
    }
    catch (ex) {
       alert_bot(ex.message, 'warning', "conten_gestion_rue");
    } finally {
        progres_hiden('progres_bar');
    }
}
//----------Activia la interfaz de registro de una tarea VIRTUAL SII-----///
const Show_activa_registro_tarea_virtual_sii = async (obje_jon) => {
    try {
        let result = "";
        delete_alert_boot();
        posicion_update_pogres('progres_bar');
        let parameter = new Array;
        parameter.push({ RECIBO: obje_jon.RECIBO, NOMBRE: obje_jon.NOMBRE, CODIGOBARRAS: obje_jon.CODIGOBARRAS});
        result = await Service_REST_Solicita_datos_registro_flujo_virtual_SII(parameter);
        if (result != "YES") {
            alert_bot(result, 'warning', "error_div_gestion_virtual");
        }
    }
    catch (ex) {
        alert_bot(ex.message, 'warning', "error_div_gestion_virtual");
    } finally {
        progres_hiden('progres_bar');
    }
}
//---------Valida la relación tramite flujo de trabajo, lista actiivades de ruta o flujos de trabjo RUE SII-----///
let ID_FLUJO_TEMP_SII = 0;
const validate_relacion_tramite_flujo = async (name_tramite) => {
    try {
        ID_FLUJO_TEMP_SII = 0;
        let result = "";
        delete_alert_boot();      
        if (name_tramite == "") {
            $("#DropDownList_flujos_tarea_sii").empty();
            $("#DropDownList_actividades_flujo_sii").empty();
            $("#DropDownList_actividades_ruta_sii").empty();
            return true;
        }
        result = await Service_REST_solicita_id_flujo_relaciondo_a_tramite(name_tramite);
        if (result != "YES") {
            alert_bot(result, 'warning', "error_content_rue_registro_vitual_sii");
        } 
        if (ID_FLUJO_TEMP_SII == 0) {
            document.getElementById("row_flujos_tarea_sii").classList.add("d-none");
            document.getElementById("row_actividades_flujo_sii").classList.add("d-none");
            document.getElementById("row_actividades_ruta_sii").classList.remove("d-none");
            document.getElementById("DropDownList_flujos_tarea_sii").setAttribute("atrib_campo_o", "0");
            document.getElementById("DropDownList_actividades_flujo_sii").setAttribute("atrib_campo_o", "0");
            document.getElementById("DropDownList_actividades_ruta_sii").setAttribute("atrib_campo_o", "1");
            result = await Service_REST_solicita_actividades_ruta_usuario(DATOS_REGISTRO_RUE_SII.id_actividad_workflow);
            if (result != "YES") {
                alert_bot(result, 'warning', "error_content_rue_registro_vitual_sii");
            }
        } else {
            document.getElementById("row_flujos_tarea_sii").classList.remove("d-none");
            document.getElementById("row_actividades_flujo_sii").classList.remove("d-none");
            document.getElementById("row_actividades_ruta_sii").classList.add("d-none");
            document.getElementById("DropDownList_flujos_tarea_sii").setAttribute("atrib_campo_o", "1");
            document.getElementById("DropDownList_actividades_flujo_sii").setAttribute("atrib_campo_o", "1");
            document.getElementById("DropDownList_actividades_ruta_sii").setAttribute("atrib_campo_o", "0");
            result = await Service_REST_solicita_lista_flujo_defult(ID_FLUJO_TEMP_SII);
            if (result != "YES") {
                alert_bot(result, 'warning', "error_content_rue_registro_vitual_sii");
            }
        }
        return true;
    }
    catch (ex) {
        alert_bot(ex.message, 'warning', "error_content_rue_registro_vitual_sii");
    } finally {
       
    }
}
//---------limpia los campos de registro de flujo SII RUE-----//////
const Hide_clear_interfaz_registro_tarea_sii_rue = () => {
    document.getElementById("row_flujos_tarea_sii").classList.add("d-none");
    document.getElementById("row_actividades_flujo_sii").classList.add("d-none");
    document.getElementById("row_actividades_ruta_sii").classList.add("d-none");
    $("#DropDownList_flujos_tarea_sii").empty();
    $("#DropDownList_actividades_flujo_sii").empty();
    $("#DropDownList_actividades_ruta_sii").empty();
    $("#modal_rue_registro_vitual_sii").modal("hide");
}
//-------------------------------------------------------------------------------------------------------
//                                     zona eventos  gestion virtual SII
//-------------------------------------------------------------------------------------------------------
//-----------Despliega el listado de rues a importar-------////
const Show_row_table_boot_vitual = (ob_row_table, obj_field_boot_table, name_table, name_parent_table) => {
    let class_stru_row_Gabinete_Generic = JSON.parse(ob_row_table);
    init_row_feld_table_boostrap_table(name_table, obj_field_boot_table, class_stru_row_Gabinete_Generic, name_parent_table, "table-bordered", "table-borderless");
    $("#modal_adjunta_archivo_virtual_sii").modal("hide");

}
//--------Funciones de reasignación de usuarios-----////
const Active_list_activities_task_asing = async (id_tarea) => {
    try {
        let result = "";
        delete_alert_boot();
        posicion_update_pogres('progres_bar');
        result = await service_REST_lista_actividades_workflow(id_tarea);
        if (result != "YES") {
            alert_bot(result, 'warning', "error_div_reasigna_sii");
        }
    }
    catch (ex) {
        alert_bot(ex.message, 'warning', "error_div_reasigna_sii");
    } finally {
        progres_hiden('progres_bar');
    }
}

//Agrega boton de la tabla reasignar
function operateFormatter_reasing(value, row, index) { 
    return [
        '<a class="active_reasing_wf ml-4" href="javascript:void(0)" title="Reasignar">',
        '<i class="fal fa-user"></i>',
       
    ].join('')
}
//Agrega el check de la tabla balanceo
function operateFormatter_balance(value, row, index) {
    let ident = table_boot_return_objet_jonson(row);
    let htmlI = ""
    let chekd = false;
    if (ident.estado_balanceo_grupo == 1) {
        chekd = "checked";      
    } else {     
        chekd = "";
    }
    return [
        '<label> <input data-index="0" class="cheke_event ml-4"  type="checkbox" ', chekd, '> </label>'
    ].join('')
}
function operateFormatter_asing(value, row, index) {
    let ident = table_boot_return_objet_jonson(row);
    let htmlI = ""
    let chekd = false;
    if (ident.estado_asigna_tarea == 1) {
        chekd = "checked";
    } else {
        chekd = "";
    }
    return [
        '<label> <input data-index="0" class="cheke_event_asing ml-4"  type="checkbox" ', chekd, '> </label>'
    ].join('')
}
function operateFormatter_ruesii(value, row, index) {
    return [
        '<div class="row pl-2">',
        '<div class="col-8 p-0">',
        '<a class="active_show_documentos_sii nav-link pl-5 justify-content-end font-weight-light" style="color: black" href="javascript:void(0)" title="Documentos sii">  <i style="color: black" class="far fa-folder-open"></i>  </a>',
        '</div > ',
        '<div class="col-4 p-0">',
        '<a class="nav-item dropdown active w-100">',
        '<a class="nav-link  dropdown-toggle " style="color: black" href="#" id="A5" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: black; display:none" class="fad fa-th-list"></i>  ',
        '</a>',
        '<div class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">',
        '<a style="color: black" href="#" class="dropdown-item font-weight-light active_show_documentos_sii" ><i class="far fa-folder-open"></i> Documentos SII </a>',
        '<a style="color: black" href="#" class="dropdown-item font-weight-light active_show_create_flujo_sii "><i class="fal fa-users-medical"></i> Crear flujo de trabajo</a>',
        '<a style="color: black" href="#" class="dropdown-item font-weight-light"><i class="far fa-sign-out"></i> Salir</a>',
        '</div>',
        '</a>',
        '</div>'

    ].join('')
}
function operateFormatter_virtualsii(value, row, index) {
    return [
        '<div class="row pl-2">',
        '<div class="col-8 p-0">',
        '<a class="active_show_documentos_virtual nav-link pl-5 justify-content-end font-weight-light" style="color: black" href="javascript:void(0)" title="Documentos sii">  <i style="color: black" class="far fa-folder-open"></i>  </a>',
        '</div > ',
        '<div class="col-4 p-0">',
        '<a class="nav-item dropdown active w-100">',
        '<a class="nav-link  dropdown-toggle " style="color: black" href="#" id="A5" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: black; display:none" class="fad fa-th-list"></i>  ',
        '</a>',
        '<div class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">',
        '<a style="color: black" href="#" class="dropdown-item font-weight-light active_show_documentos_virtual" ><i class="far fa-folder-open"></i> Documentos SII </a>',
        '<a style="color: black" href="#" class="dropdown-item font-weight-light active_show_create_flujo_virtual "><i class="fal fa-users-medical"></i> Crear flujo de trabajo</a>',
        '<a style="color: black" href="#" class="dropdown-item font-weight-light"><i class="far fa-sign-out"></i> Salir</a>',
        '</div>',
        '</a>',
        '</div>'

    ].join('')
}

function operateFormatter_image_sii(value, row, index) {
    return [
        '<div class="row pl-2">',
        '<div class="col-8 p-0">',
        '<a class="active_view_document_sii nav-link pl-5 justify-content-end font-weight-light" style="color: black" href="javascript:void(0)" title="Ver documento">  <i style="color: black" class="fal fa-file-image"></i>  </a>',
        '</div > ',
        '<div class="col-4 p-0">',
        '<a class="nav-link  dropdown-toggle justify-content-start" style="color: black" href="#"  data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: black; display:none" class="fad fa-th-list"></i>  ',
        '</a>',
        '<div class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">',
        '<a class="active_download_documento_sii dropdown-item font-weight-light" href="javascript:void(0)" title="Descarga documento">  <i style="color: #black" class="far fa-folder-open"></i> Descarga documento SII </a>',
        '<a style="color: black" href="#" class="dropdown-item font-weight-light"><i class="far fa-sign-out"></i> Salir del menu</a>',
        '</div>',
        '</a>',
        '</div>',
        '</div>',
    ].join('')
}
window.operateEvents = {
    'click .cheke_event': (e, value, row, index) => {
        let obje_jon = table_boot_return_objet_jonson(row);
        if (e.currentTarget.checked === false) {
            Service_Inactiva_usuario_workflow_balanceo_grupo(obje_jon.idU_suario);
        } else {
            Service_Activa_usuario_workflow_balanceo_grupo(obje_jon.idU_suario);       
        }
    }, 'click .active_reasing_wf': (e, value, row, index) => {
        let obje_jon = table_boot_return_objet_jonson(row);
        Active_list_activities_task_asing(obje_jon.ID_TAREA);
    }, 'click .cheke_event_asing': (e, value, row, index) => {
        let obje_jon = table_boot_return_objet_jonson(row);
        let estate_check = 0;
        if (e.currentTarget.checked === false) {
            estate_check = 0;
        } else {
            estate_check = 1;
        }
        Service_cambia_estado_asignacion_usuario_workflow(obje_jon.idU_suario, estate_check);
    }
    , 'click .active_show_documentos_sii': (e, value, row, index) => {
        let obje_jon = table_boot_return_objet_jonson(row);
        Show_activa_lista_documentos_raicado_sii(obje_jon.CODIGOBARRAS);
    }
    , 'click .active_show_documentos_virtual': (e, value, row, index) => {
        let obje_jon = table_boot_return_objet_jonson(row);
        Show_activa_lista_documentos_raicado_virtual(obje_jon.CODIGOBARRAS);
    }
    , 'click .active_view_document_sii': (e, value, row, index) => {
        let obje_jon = table_boot_return_objet_jonson(row);
        Show_activa_visualzar_imagen_sii(obje_jon.CODIGOBARRAS, obje_jon.url, obje_jon.formato, obje_jon.observaciones);
    }
    , 'click .active_download_documento_sii': (e, value, row, index) => {
        let obje_jon = table_boot_return_objet_jonson(row);
        let target = "";
        if (obje_jon.formato == "pdf") {
            target = "_blank";
        }
        donw_load_file_general(obje_jon.url, "", target);     
    }
    , 'click .active_show_create_flujo_sii': (e, value, row, index) => {
        let obje_jon = table_boot_return_objet_jonson(row);
        Show_activa_registro_tarea_rue_sii(obje_jon);
    }, 'click .active_show_create_flujo_virtual': (e, value, row, index) => {
        let obje_jon = table_boot_return_objet_jonson(row);
        Show_activa_registro_tarea_virtual_sii(obje_jon);
    }
}
//---------Solicita la estructura de permisos de interacion del usuario-------/////
const Service_REST_solicita_permisos_usuario_workflow_intgracion_sii = async (parameter) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceWorkflow.asmx/Service_Solicita_permisos_usuario_workflow_intgracion_sii', {
                data: "{'parameter':'" + parameter + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_gestion !== "YES") {
                        resolve(data.d[0].Error_gestion);
                    } else {
                        permisos_integracion_sii = data.d[0].permisos_int_sii;
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
                        resolve("Time out error.");


                    } else if (textStatus === 'abort') {
                        resolve("Ajax request aborted.");


                    } else {
                        resolve("Ajax request aborted." + xception.responseText);
                    }
                }, compelete: function () {


                }
            });
        } catch (ex) {
            resolve(ex.message + " funcion Service_REST_solicita_permisos_usuario_workflow_intgracion_sii");
        }
    })
    let result = await myPromise;
    return result;
}
const Service_REST_lista_estado_tarea_asignacion = async (radicado) => {
    let myPromise = new Promise(function (resolve) {
    try {
        $.ajax('../webservice/WebServiceWorkflow.asmx/Service_lista_estado_tarea_asignada', {
            data: "{'parameter':'" + radicado +  "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {       
                if (data.d[0].result !== "YES") {
                    init_row_constant_table_boostrap_table("table", data.d[0].row_estado_tarea, "div_content_tabla", "table-bordered", "table-borderless");
                    resolve(data.d[0].result);
                         
                } else {
                    
                    if (data.d[0].row_estado_tarea.length == 0) {
                        ID_TAREA_WORKFLOW_WF = 0;
                    } else {
                        ID_TAREA_WORKFLOW_WF = data.d[0].row_estado_tarea[0].ID_TAREA;
                    }
                    init_row_constant_table_boostrap_table("table", data.d[0].row_estado_tarea, "div_content_tabla", "table-bordered", "table-borderless");
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
                    resolve ("Time out error.");


                } else if (textStatus === 'abort') {
                    resolve("Ajax request aborted.");


                } else {
                    resolve("Ajax request aborted." + xception.responseText);
                }
            }, compelete: function () {
               
                
            }
        });
    } catch (ex) {
        resolve(ex.message + " funcion Service_REST_lista_estado_tarea_asignacion");
        }
    })
    let result = await myPromise;
    return result;
}
//ZONA LISTA GRUPOS USUARIOS ACTIVIDAD
const service_REST_lista_actividades_workflow = async (id_tarea_workflow) => {
    let myPromise = new Promise(function (resolve) {
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
                        $("#DropDownList_list_actividad_workflow_sii").empty();
                        $("#DropDownList_list_usuario_workflow_sii").empty();
                        resolve(data.d[0].result);
                    } else {
                        $("#DropDownList_list_actividad_workflow_sii").empty();
                        $("#DropDownList_list_usuario_workflow_sii").empty();
                        var ITEMS_DATOS_DROW_ = new Array();
                        $.each(data.d, function (k, v) {
                            ITEMS_DATOS_DROW_.push(v);
                        });
                        var element_drow = document.getElementById("DropDownList_list_actividad_workflow_sii");
                        for (var i = 0; i < ITEMS_DATOS_DROW_.length; i++) {
                            element_drow[i] = new Option(ITEMS_DATOS_DROW_[i].nombre_actividad, ITEMS_DATOS_DROW_[i].id_actividad);
                        }
                        $find("ModalPopupExtender_edition_reasigna_tarea_workflow_sii").show();
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
                        resolve("Time out error.");
                    } else if (textStatus === 'abort') {
                        resolve("Ajax request aborted.");
                    } else {
                        resolve("Ajax request aborted." + xception.responseText);
                    }
                   
                }
            });
        } catch (ex) {
            resolve(ex.message + " funcion Service_solicita_lista_registro_sii_migrados");
        }
    })
    let result = await myPromise;
    return result;
}
const Service_REST_lista_usuario_relacionado_actividad = async (id_actividad_wf, id_tarea) => {
    let myPromise = new Promise(function (resolve) {
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
                    $("#DropDownList_list_usuario_workflow_sii").empty();
                    resolve(data.d[0].result);
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
                    resolve("Time out error.");


                } else if (textStatus === 'abort') {
                    resolve("Ajax request aborted.");


                } else {
                    resolve("Ajax request aborted." + xception.responseText);
                }
            }
        });
        } catch (ex) {
        resolve(ex.message + " funcion Service_REST_lista_usuario_relacionado_actividad");
        }
    })
    let result = await myPromise;
    return result;
}
//ZONA SERVICE REASIGNA TAREA SII
const Service_REST_reasigna_tarea_usuario_sii_workflow = async (id_tarea, id_actividad, id_usuario_worlflow, asigna_tarea_sii) => {
    let myPromise = new Promise(function (resolve) {
    try {
        $.ajax('../webservice/WebServiceWorkflow.asmx/Service_reasigna_tarea_usuario_sii_workflow', {
            data: "{'id_tarea':'" + id_tarea + "','id_actividad':'" + id_actividad + "','id_usuario_worlflow':'" + id_usuario_worlflow + "','asigna_tarea_sii':'" + asigna_tarea_sii + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].result !== "YES") {
                    resolve(data.d[0].result);
                } else {
                    let stru_values = new Array();
                    stru_values.push({ value_campo: data.d[0].nombre_actividad, name_campo :  "ACTIVIDAD" });
                    stru_values.push({ value_campo: data.d[0].nombre_usuario,   name_campo :  "USUARIO" });
                    stru_values.push({ value_campo: data.d[0].cargo_usuario, name_campo: "CARGO" });
                    UpdaTeRowsReinit('table', id_tarea, stru_values);
                    if (data.d[0].reault_cambio_estado !== "YES") {
                        resolve(data.d[0].reault_cambio_estado);
                    }
                    $find("ModalPopupExtender_edition_reasigna_tarea_workflow_sii").hide();
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
                    resolve("Time out error.");


                } else if (textStatus === 'abort') {
                    resolve("Ajax request aborted.");


                } else {
                    resolve("Ajax request aborted." + xception.responseText);
                }
            }
        });
        } catch (ex) {
        resolve(ex.message + " funcion Service_REST_reasigna_tarea_usuario_sii_workflow");
        }
    })
    let result = await myPromise;
    return result;
}
const Service_REST_solicita_datos_estructura_consulta_recibo_ruta_interfaz_SII = async (radicado) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceWorkflow.asmx/Service_solicita_datos_estructura_consulta_recibo_ruta_interfaz_SII', {
                data: "{'parameter':'" + radicado + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_gestion !== "YES") {
                        $("#DropDownList_tramites_rut").empty();
                        $("#DropDownList_actividades_ruta").empty();
                        $("#DropDownList_usurios_ruta").empty();
                        document.getElementById("TextBox_razon_social_ruta").value = "";
                        document.getElementById("TextBox_matricula_rut").value = "";
                        document.getElementById("TextBox_codigo_barras_ruta").value = "";
                        resolve(data.d[0].Error_gestion);
                    } else {
                        $("#DropDownList_tramites_rut").empty();
                        $("#DropDownList_actividades_ruta").empty();
                        $("#DropDownList_usurios_ruta").empty();
                        let Rsocial = data.d[0].Class_parram_consultarRadicado.nombre;
                        if (Rsocial !== "") {
                            Rsocial = Rsocial.replace("'", "");
                            Rsocial = Rsocial.replace("/", "");
                        }
                        document.getElementById("TextBox_razon_social_ruta").value = Rsocial;
                        document.getElementById("TextBox_matricula_rut").value = data.d[0].Class_parram_consultarRadicado.matricula;
                        document.getElementById("TextBox_codigo_barras_ruta").value = data.d[0].Class_parram_consultarRadicado.radicado;
                        let ITEMS_DATOS_DROW_ = new Array();
                        $.each(data.d[0].Class_service_ilist_drowlist, function (k, v) {
                            ITEMS_DATOS_DROW_.push(v);
                        });
                        let element_drow = document.getElementById("DropDownList_tramites_rut");
                        for (var i = 0; i < ITEMS_DATOS_DROW_.length; i++) {
                            let spliT = ITEMS_DATOS_DROW_[i].id_value.split("|");
                            element_drow[i] = new Option(ITEMS_DATOS_DROW_[i].value_campo, spliT[0]);
                            if (spliT[1] == data.d[0].Class_parram_consultarRadicado.subtipotramite) {
                                element_drow[i].selected = true;
                            }
                        }
                        ITEMS_DATOS_DROW_ = new Array();
                        $.each(data.d[0].Class_service_ilist_drowlist_actividad, function (k, v) {
                            ITEMS_DATOS_DROW_.push(v);
                        });
                        element_drow = document.getElementById("DropDownList_actividades_ruta");
                        for (var i = 0; i < ITEMS_DATOS_DROW_.length; i++) {
                            element_drow[i] = new Option(ITEMS_DATOS_DROW_[i].value_campo, ITEMS_DATOS_DROW_[i].id_value);    
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
                        resolve("Time out error.");


                    } else if (textStatus === 'abort') {
                        resolve("Ajax request aborted.");


                    } else {
                        resolve("Ajax request aborted." + xception.responseText);
                    }
                }, compelete: function () {


                }
            });
        } catch (ex) {
            resolve(ex.message + " funcion Service_REST_solicita_datos_estructura_consulta_recibo_ruta_interfaz_SII");
        }
    })
    let result = await myPromise;
    return result;
}
const Service_REST_solicita_datos_estructura_consulta_recibo_flujo_interfaz_SII = async (radicado) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceWorkflow.asmx/Service_solicita_datos_estructura_consulta_recibo_flujo_interfaz_SII', {
                data: "{'parameter':'" + radicado + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_gestion !== "YES") {
                        $("#DropDownList_tramites_flujo").empty();
                        $("#DropDownList_flujos").empty();   
                        $("#DropDownList_actividades_flujo").empty();
                        $("#DropDownList_usurios_flujo").empty();
                        document.getElementById("TextBox_razon_social_flujo").value = "";
                        document.getElementById("TextBox_matricula_flujo").value = "";
                        document.getElementById("TextBox_codigo_barras_flujo").value = "";
                        resolve(data.d[0].Error_gestion);
                    } else {
                        $("#DropDownList_tramites_flujo").empty();
                        $("#DropDownList_flujos").empty();
                        $("#DropDownList_actividades_flujo").empty();
                        $("#DropDownList_usurios_flujo").empty();
                        let Rsocial = data.d[0].Class_parram_consultarRadicado.nombre;
                        if (Rsocial !== "") {
                            Rsocial = Rsocial.replace("'", "");
                            Rsocial = Rsocial.replace("/", "");
                        }
                        document.getElementById("TextBox_razon_social_flujo").value = Rsocial;
                        document.getElementById("TextBox_matricula_flujo").value = data.d[0].Class_parram_consultarRadicado.matricula;
                        document.getElementById("TextBox_codigo_barras_flujo").value = data.d[0].Class_parram_consultarRadicado.radicado;
                        let ITEMS_DATOS_DROW_ = new Array();
                        $.each(data.d[0].Class_service_ilist_drowlist, function (k, v) {
                            ITEMS_DATOS_DROW_.push(v);
                        });
                        let element_drow = document.getElementById("DropDownList_tramites_flujo");
                        for (var i = 0; i < ITEMS_DATOS_DROW_.length; i++) {
                            let spliT = ITEMS_DATOS_DROW_[i].id_value.split("|");
                            element_drow[i] = new Option(ITEMS_DATOS_DROW_[i].value_campo, spliT[0]);
                            if (spliT[1] == data.d[0].Class_parram_consultarRadicado.subtipotramite) {
                                element_drow[i].selected = true;
                            }
                        }
                        //
                        ITEMS_DATOS_DROW_ = new Array();
                        $.each(data.d[0].Class_service_ilist_drowlist_flujos, function (k, v) {
                            ITEMS_DATOS_DROW_.push(v);
                        });
                        element_drow = document.getElementById("DropDownList_flujos");
                        for (var i = 0; i < ITEMS_DATOS_DROW_.length; i++) {
                            element_drow[i] = new Option(ITEMS_DATOS_DROW_[i].value_campo, ITEMS_DATOS_DROW_[i].id_value);
                            if (data.d[0].id_flujo == ITEMS_DATOS_DROW_[i].id_value) {
                                element_drow[i].selected = true;
                            }
                        }
                        ITEMS_DATOS_DROW_ = new Array();
                        $.each(data.d[0].Class_service_ilist_drowlist_actividad_flujo, function (k, v) {
                            ITEMS_DATOS_DROW_.push(v);
                        });
                        element_drow = document.getElementById("DropDownList_actividades_flujo");
                        for (var i = 0; i < ITEMS_DATOS_DROW_.length; i++) {
                            element_drow[i] = new Option(ITEMS_DATOS_DROW_[i].value_campo, ITEMS_DATOS_DROW_[i].id_value);
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
                        resolve("Time out error.");


                    } else if (textStatus === 'abort') {
                        resolve("Ajax request aborted.");


                    } else {
                        resolve("Ajax request aborted." + xception.responseText);
                    }
                }, compelete: function () {


                }
            });
        } catch (ex) {
            resolve(ex.message + " funcion Service_REST_solicita_datos_estructura_consulta_recibo_flujo_interfaz_SII");
        }
    })
    let result = await myPromise;
    return result;
}
const Service_REST_solicita_recibo_radicado_sii = async (radicado_sii) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebService_integracion_sii.asmx/Service_solicita_recibo_radicado_sii', {
                data: "{'radicado_sii':" + "'" + radicado_sii + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_result !== "YES") {
                        resolve(data.d[0].Error_result);
                    } else {
                        RECIBO_VIRTUAL_SII = data.d[0].recibo;
                        resolve("YES");
                    }
                }, error: function (xception, textStatus, errorThrown) {

                    if (xception.status === 0) {
                        resolve("Not connect: Verify Network.");


                    } else if (xception.status == 404) {
                        resolve("Requested page not found [404]");


                    } else if (xception.status == 500) {
                        return ("Internal Server Error [500]." + xception.responseText);


                    } else if (textStatus === 'parsererror') {
                        resolve("Requested JSON parse failed.");


                    } else if (textStatus === 'timeout') {
                        resolve("Time out error.");


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
//ZONA GESTION RUE SII
const Service_REST_Service_Read_file_fast_Excell = async (parameter) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceWorkflow.asmx/Service_Read_file_fast_Excell', {
                data: "{" + "'parameter':'" + parameter + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].error_result !== "YES") {
                        resolve(data.d[0].error_result);
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
            resolve(ex.message + " funcion Service_REST_actualiza_datos_imagen_workflow_SII");
        }
    })
    let result = await myPromise;
    return result;
}
//-------Solicita estructura campo tabla BOOT head
const Service_REST_solicita_estructura_campos_dynamic_polantilla_externa_rue_SII = async (name_plantilla, name_table, name_parent_table) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServicePlantillaExterna.asmx/Service_solicita_estructura_campos_dynamic_polantilla_externa_rue_SII', {
                data: "{'name_plantilla':" + "'" + name_plantilla + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_result !== "YES") {
                        resolve(data.d[0].Error_result);
                    } else {
                        init_feld_table_boostrap_table(name_table, data.d[0].Obj_ilist_fileds_generic, name_parent_table, "", "table-borderless");
                        resolve("YES");
                    }
                }, error: function (xception, textStatus, errorThrown) {

                    if (xception.status === 0) {
                        resolve("Not connect: Verify Network.");


                    } else if (xception.status == 404) {
                        resolve("Requested page not found [404]");


                    } else if (xception.status == 500) {
                        return ("Internal Server Error [500]." + xception.responseText);


                    } else if (textStatus === 'parsererror') {
                        resolve("Requested JSON parse failed.");


                    } else if (textStatus === 'timeout') {
                        resolve("Time out error.");


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
//-------Solicita lista de archivos relacionados a un recibo SII
const Service_REST_solicita_lista_archivos_relacionados_radicado_sii = async (radicado_sii, name_table, name_parent_table) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebService_integracion_sii.asmx/Service_solicita_lista_archivos_relacionados_radicado_sii', {
                data: "{'radicado_sii':" + "'" + radicado_sii + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_result !== "YES") {
                        resolve(data.d[0].Error_result);
                    } else {
                        let class_stru_row_Gabinete_Generic = JSON.parse(data.d[0].row_table_boot);
                        init_row_feld_table_boostrap_table(name_table, data.d[0].field_table_boot, class_stru_row_Gabinete_Generic, name_parent_table, "table-bordered", "table-borderless");
                        if (document.getElementById("h_title_radicado_sii")) {
                            document.getElementById("h_title_radicado_sii").innerText = "RADICADO " + radicado_sii;
                        }
                        $("#modal_sii_rues_file").modal("show");
                        auto_zise_lista_documentos_sii();
                        document.getElementById("Iframe_view_sii_rues_file").src = "";
                        resolve("YES");
                    }
                }, error: function (xception, textStatus, errorThrown) {

                    if (xception.status === 0) {
                        resolve("Not connect: Verify Network.");


                    } else if (xception.status == 404) {
                        resolve("Requested page not found [404]");


                    } else if (xception.status == 500) {
                        resolve ("Internal Server Error [500]." + xception.responseText);


                    } else if (textStatus === 'parsererror') {
                        resolve("Requested JSON parse failed.");


                    } else if (textStatus === 'timeout') {
                        resolve("Time out error.");


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
//----------Solicita la estructura de datos de registro de tarea VIRTUAL SII-----------//
const Service_REST_Solicita_datos_registro_flujo_virtual_SII = async (parameter) => {
    DATOS_REGISTRO_RUE_SII = null;
    var serialice = JSON.stringify(parameter);
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceWorkflow.asmx/Service_Solicita_datos_registro_flujo_virtual_sii', {
                data: "{'parameter':'" + serialice + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_gestion !== "YES") {
                        $("#DropDownList_tramites_flujo_tarea_sii").empty();
                        $("#DropDownList_flujos_tarea_sii").empty();
                        $("#DropDownList_actividades_flujo_sii").empty();
                        $("#DropDownList_actividades_ruta_sii").empty();
                        document.getElementById("TextBox_recibo_flujo_tarea_sii").value = "";
                        document.getElementById("TextBox_razon_social_flujo_tarea_sii").value = "";
                        document.getElementById("TextBox_matricula_flujo_tarea_sii").value = "";
                        document.getElementById("TextBox_codigo_barras_flujo_tarea_sii").value = "";
                        resolve(data.d[0].Error_gestion);
                    } else {
                        DATOS_REGISTRO_RUE_SII = data.d[0];
                        $("#DropDownList_tramites_flujo_tarea_sii").empty();
                        $("#DropDownList_flujos_tarea_sii").empty();
                        $("#DropDownList_actividades_flujo_sii").empty();
                        $("#DropDownList_actividades_ruta_sii").empty();
                        let Rsocial = data.d[0].Class_parram_consultarRadicado.nombre;
                        if (Rsocial !== "") {
                            Rsocial = Rsocial.replace("'", "");
                            Rsocial = Rsocial.replace("/", "");
                        }
                        document.getElementById("TextBox_recibo_flujo_tarea_sii").value = data.d[0].Class_parram_consultarRecibo.recibo;
                        document.getElementById("title_registro_vitual_sii").innerHTML = "Radicado " + data.d[0].Class_parram_consultarRadicado.radicado + " Razon social " + Rsocial;
                        document.getElementById("TextBox_razon_social_flujo_tarea_sii").value = Rsocial;
                        document.getElementById("TextBox_matricula_flujo_tarea_sii").value = data.d[0].Class_parram_consultarRadicado.matricula;
                        document.getElementById("TextBox_codigo_barras_flujo_tarea_sii").value = data.d[0].Class_parram_consultarRadicado.radicado;
                        let ITEMS_DATOS_DROW_ = new Array();
                        $.each(data.d[0].Class_service_ilist_drowlist, function (k, v) {
                            ITEMS_DATOS_DROW_.push(v);
                        });
                        let element_drow = document.getElementById("DropDownList_tramites_flujo_tarea_sii");
                        for (var i = 0; i < ITEMS_DATOS_DROW_.length; i++) {
                            let spliT = ITEMS_DATOS_DROW_[i].id_value.split("|");
                            element_drow[i] = new Option(ITEMS_DATOS_DROW_[i].value_campo, ITEMS_DATOS_DROW_[i].id_value);
                            if (spliT[1] == data.d[0].Class_parram_consultarRadicado.subtipotramite) {
                                element_drow[i].selected = true;
                            }
                        }
                        ITEMS_DATOS_DROW_ = new Array();
                        $.each(data.d[0].Class_service_ilist_drowlist_flujos, function (k, v) {
                            ITEMS_DATOS_DROW_.push(v);
                        });
                        element_drow = document.getElementById("DropDownList_flujos_tarea_sii");
                        for (var i = 0; i < ITEMS_DATOS_DROW_.length; i++) {
                            element_drow[i] = new Option(ITEMS_DATOS_DROW_[i].value_campo, ITEMS_DATOS_DROW_[i].id_value);
                            if (data.d[0].id_flujo == ITEMS_DATOS_DROW_[i].id_value) {
                                element_drow[i].selected = true;
                            }
                        }
                        ITEMS_DATOS_DROW_ = new Array();
                        $.each(data.d[0].Class_service_ilist_drowlist_actividad_flujo, function (k, v) {
                            ITEMS_DATOS_DROW_.push(v);
                        });
                        element_drow = document.getElementById("DropDownList_actividades_flujo_sii");
                        for (var i = 0; i < ITEMS_DATOS_DROW_.length; i++) {
                            element_drow[i] = new Option(ITEMS_DATOS_DROW_[i].value_campo, ITEMS_DATOS_DROW_[i].id_value);
                        }
                        ITEMS_DATOS_DROW_ = new Array();
                        $.each(data.d[0].Class_service_ilist_drowlist_rutas, function (k, v) {
                            ITEMS_DATOS_DROW_.push(v);
                        });
                        element_drow = document.getElementById("DropDownList_actividades_ruta_sii");
                        for (var i = 0; i < ITEMS_DATOS_DROW_.length; i++) {
                            element_drow[i] = new Option(ITEMS_DATOS_DROW_[i].value_campo, ITEMS_DATOS_DROW_[i].id_value);
                        }
                        if (DATOS_REGISTRO_RUE_SII.id_flujo == 0) {
                            document.getElementById("row_flujos_tarea_sii").classList.add("d-none");
                            document.getElementById("row_actividades_flujo_sii").classList.add("d-none");
                            document.getElementById("row_actividades_ruta_sii").classList.remove("d-none");
                            document.getElementById("DropDownList_flujos_tarea_sii").setAttribute("atrib_campo_o", "0");
                            document.getElementById("DropDownList_actividades_flujo_sii").setAttribute("atrib_campo_o", "0");
                            document.getElementById("DropDownList_actividades_ruta_sii").setAttribute("atrib_campo_o", "1");
                        } else {
                            document.getElementById("row_flujos_tarea_sii").classList.remove("d-none");
                            document.getElementById("row_actividades_flujo_sii").classList.remove("d-none");
                            document.getElementById("row_actividades_ruta_sii").classList.add("d-none");
                            document.getElementById("DropDownList_flujos_tarea_sii").setAttribute("atrib_campo_o", "1");
                            document.getElementById("DropDownList_actividades_flujo_sii").setAttribute("atrib_campo_o", "1");
                            document.getElementById("DropDownList_actividades_ruta_sii").setAttribute("atrib_campo_o", "0");
                        }
                        document.getElementById("Button_registro_actividad_flujo_tarea_sii").classList.add("d-none");
                        document.getElementById("Button_registro_actividad_flujo_tarea_virtual_sii").classList.remove("d-none");
                        $("#modal_rue_registro_vitual_sii").modal("show");
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
                        resolve("Time out error.");


                    } else if (textStatus === 'abort') {
                        resolve("Ajax request aborted.");


                    } else {
                        resolve("Ajax request aborted." + xception.responseText);
                    }
                }, compelete: function () {


                }
            });
        } catch (ex) {
            resolve(ex.message + " funcion Service_REST_solicita_datos_estructura_consulta_recibo_flujo_interfaz_SII");
        }
    })
    let result = await myPromise;
    return result;
}
//----------Solicita la estructura de datos de registro de tarea RUE SII-----------//
const Service_REST_Solicita_datos_registro_flujo_rue_SII = async (parameter) => {
    DATOS_REGISTRO_RUE_SII = null;
    var serialice = JSON.stringify(parameter);
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceWorkflow.asmx/Service_Solicita_datos_registro_flujo_rue_sii', {
                data: "{'parameter':'" + serialice + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_gestion !== "YES") {
                        $("#DropDownList_tramites_flujo_tarea_sii").empty();
                        $("#DropDownList_flujos_tarea_sii").empty();
                        $("#DropDownList_actividades_flujo_sii").empty();
                        $("#DropDownList_actividades_ruta_sii").empty();
                        document.getElementById("TextBox_recibo_flujo_tarea_sii").value = "";
                        document.getElementById("TextBox_razon_social_flujo_tarea_sii").value = "";
                        document.getElementById("TextBox_matricula_flujo_tarea_sii").value = "";
                        document.getElementById("TextBox_codigo_barras_flujo_tarea_sii").value = "";
                        resolve(data.d[0].Error_gestion);
                    } else {
                        DATOS_REGISTRO_RUE_SII = data.d[0];
                        $("#DropDownList_tramites_flujo_tarea_sii").empty();
                        $("#DropDownList_flujos_tarea_sii").empty();
                        $("#DropDownList_actividades_flujo_sii").empty();
                        $("#DropDownList_actividades_ruta_sii").empty();
                        let Rsocial = data.d[0].Class_parram_consultarRadicado.nombre;
                        if (Rsocial !== "") {
                            Rsocial = Rsocial.replace("'", "");
                            Rsocial = Rsocial.replace("/", "");
                        }
                        document.getElementById("TextBox_recibo_flujo_tarea_sii").value = data.d[0].Class_parram_consultarRecibo.recibo;
                        document.getElementById("title_registro_vitual_sii").innerHTML = "Radicado " + data.d[0].Class_parram_consultarRadicado.radicado + " Razon social " + Rsocial;
                        document.getElementById("TextBox_razon_social_flujo_tarea_sii").value = Rsocial;
                        document.getElementById("TextBox_matricula_flujo_tarea_sii").value = data.d[0].Class_parram_consultarRadicado.matricula;
                        document.getElementById("TextBox_codigo_barras_flujo_tarea_sii").value = data.d[0].Class_parram_consultarRadicado.radicado;
                        let ITEMS_DATOS_DROW_ = new Array();
                        $.each(data.d[0].Class_service_ilist_drowlist, function (k, v) {
                            ITEMS_DATOS_DROW_.push(v);
                        });
                        let element_drow = document.getElementById("DropDownList_tramites_flujo_tarea_sii");
                        for (var i = 0; i < ITEMS_DATOS_DROW_.length; i++) {
                            let spliT = ITEMS_DATOS_DROW_[i].id_value.split("|");
                            element_drow[i] = new Option(ITEMS_DATOS_DROW_[i].value_campo, ITEMS_DATOS_DROW_[i].id_value);
                            if (spliT[1] == data.d[0].Class_parram_consultarRadicado.subtipotramite) {
                                element_drow[i].selected = true;
                            }
                        }
                        ITEMS_DATOS_DROW_ = new Array();
                        $.each(data.d[0].Class_service_ilist_drowlist_flujos, function (k, v) {
                            ITEMS_DATOS_DROW_.push(v);
                        });
                        element_drow = document.getElementById("DropDownList_flujos_tarea_sii");
                        for (var i = 0; i < ITEMS_DATOS_DROW_.length; i++) {
                            element_drow[i] = new Option(ITEMS_DATOS_DROW_[i].value_campo, ITEMS_DATOS_DROW_[i].id_value);
                            if (data.d[0].id_flujo == ITEMS_DATOS_DROW_[i].id_value) {
                                element_drow[i].selected = true;
                            }
                        }
                        ITEMS_DATOS_DROW_ = new Array();
                        $.each(data.d[0].Class_service_ilist_drowlist_actividad_flujo, function (k, v) {
                            ITEMS_DATOS_DROW_.push(v);
                        });
                        element_drow = document.getElementById("DropDownList_actividades_flujo_sii");
                        for (var i = 0; i < ITEMS_DATOS_DROW_.length; i++) {
                            element_drow[i] = new Option(ITEMS_DATOS_DROW_[i].value_campo, ITEMS_DATOS_DROW_[i].id_value);
                        }
                        ITEMS_DATOS_DROW_ = new Array();
                        $.each(data.d[0].Class_service_ilist_drowlist_rutas, function (k, v) {
                            ITEMS_DATOS_DROW_.push(v);
                        });
                        element_drow = document.getElementById("DropDownList_actividades_ruta_sii");
                        for (var i = 0; i < ITEMS_DATOS_DROW_.length; i++) {
                            element_drow[i] = new Option(ITEMS_DATOS_DROW_[i].value_campo, ITEMS_DATOS_DROW_[i].id_value);
                        }
                        if (DATOS_REGISTRO_RUE_SII.id_flujo == 0) {
                            document.getElementById("row_flujos_tarea_sii").classList.add("d-none");
                            document.getElementById("row_actividades_flujo_sii").classList.add("d-none");
                            document.getElementById("row_actividades_ruta_sii").classList.remove("d-none");
                            document.getElementById("DropDownList_flujos_tarea_sii").setAttribute("atrib_campo_o", "0");
                            document.getElementById("DropDownList_actividades_flujo_sii").setAttribute("atrib_campo_o", "0");
                            document.getElementById("DropDownList_actividades_ruta_sii").setAttribute("atrib_campo_o", "1");
                        } else {
                            document.getElementById("row_flujos_tarea_sii").classList.remove("d-none");
                            document.getElementById("row_actividades_flujo_sii").classList.remove("d-none");
                            document.getElementById("row_actividades_ruta_sii").classList.add("d-none");
                            document.getElementById("DropDownList_flujos_tarea_sii").setAttribute("atrib_campo_o", "1");
                            document.getElementById("DropDownList_actividades_flujo_sii").setAttribute("atrib_campo_o", "1");
                            document.getElementById("DropDownList_actividades_ruta_sii").setAttribute("atrib_campo_o", "0");
                        }
                        document.getElementById("Button_registro_actividad_flujo_tarea_sii").classList.remove("d-none");
                        document.getElementById("Button_registro_actividad_flujo_tarea_virtual_sii").classList.add("d-none");
                        $("#modal_rue_registro_vitual_sii").modal("show");
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
                        resolve("Time out error.");


                    } else if (textStatus === 'abort') {
                        resolve("Ajax request aborted.");


                    } else {
                        resolve("Ajax request aborted." + xception.responseText);
                    }
                }, compelete: function () {


                }
            });
        } catch (ex) {
            resolve(ex.message + " funcion Service_REST_solicita_datos_estructura_consulta_recibo_flujo_interfaz_SII");
        }
    })
    let result = await myPromise;
    return result;
}
//----------Solicita lista la  relación flujo de trabajo-----------//
const Service_REST_solicita_id_flujo_relaciondo_a_tramite = async (parameter) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceWorkflow.asmx/Service_solicita_id_flujo_relaciondo_a_tramite', {
                data: "{'parameter':'" + parameter + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].error_result !== "YES") {
                        resolve(data.d[0].error_result);
                        ID_FLUJO_TEMP_SII = 0;
                    } else {
                        ID_FLUJO_TEMP_SII = data.d[0].identificador;
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
                        resolve("Time out error.");


                    } else if (textStatus === 'abort') {
                        resolve("Ajax request aborted.");


                    } else {
                        resolve("Ajax request aborted." + xception.responseText);
                    }
                }, compelete: function () {


                }
            });
        } catch (ex) {
            resolve(ex.message + " funcion Service_REST_solicita_id_flujo_relaciondo_a_tramite");
        }
    })
    let result = await myPromise;
    return result;
}
//----------Solicita lista la  estructura tipo drowp de una actividad-----------//
const Service_REST_solicita_actividades_ruta_usuario = async (id_actividad) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceWorkflow.asmx/Service_solicita_actividades_ruta_usuario', {
                data: "{'parameter':'" + id_actividad + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_gestion !== "YES") {
                        $("#DropDownList_flujos_tarea_sii").empty();
                        $("#DropDownList_actividades_flujo_sii").empty();
                        $("#DropDownList_actividades_ruta_sii").empty(); 
                        resolve(data.d[0].Error_gestion);
                    } else {
                        $("#DropDownList_flujos_tarea_sii").empty();
                        $("#DropDownList_actividades_flujo_sii").empty();
                        $("#DropDownList_actividades_ruta_sii").empty();
                        var ITEMS_DATOS_DROW_ = new Array();
                        $.each(data.d[0].Class_service_ilist_drowlist, function (k, v) {
                            ITEMS_DATOS_DROW_.push(v);
                        });   
                        var element_drow = document.getElementById("DropDownList_actividades_ruta_sii");
                        for (var i = 0; i < ITEMS_DATOS_DROW_.length; i++) {
                            element_drow[i] = new Option(ITEMS_DATOS_DROW_[i].value_campo, ITEMS_DATOS_DROW_[i].id_value);
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
                        resolve("Time out error.");


                    } else if (textStatus === 'abort') {
                        resolve("Ajax request aborted.");


                    } else {
                        resolve("Ajax request aborted." + xception.responseText);
                    }
                }
            });
        } catch (ex) {
            resolve(ex.message + " funcion Service_REST_lista_usuario_relacionado_actividad");
        }
    })
    let result = await myPromise;
    return result;
}
//----------Solicita lista la  estructura tipo drowp de un flujo de trabajo-----------//
const Service_REST_solicita_lista_flujo_defult = async (id_flujo) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceWorkflow.asmx/Service_solicita_lista_flujo_defult', {
                data: "{'parameter':'" + id_flujo + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_gestion !== "YES") {
                        $("#DropDownList_flujos_tarea_sii").empty();
                        $("#DropDownList_actividades_flujo_sii").empty();
                        $("#DropDownList_actividades_ruta_sii").empty();
                        resolve(data.d[0].Error_gestion);
                    } else {
                        $("#DropDownList_flujos_tarea_sii").empty();
                        $("#DropDownList_actividades_flujo_sii").empty();
                        $("#DropDownList_actividades_ruta_sii").empty();
                        var ITEMS_DATOS_DROW_ = new Array();
                        $.each(data.d[0].Class_service_ilist_drowlist, function (k, v) {
                            ITEMS_DATOS_DROW_.push(v);
                        });
                        var element_drow = document.getElementById("DropDownList_flujos_tarea_sii");
                        for (var i = 0; i < ITEMS_DATOS_DROW_.length; i++) {
                            element_drow[i] = new Option(ITEMS_DATOS_DROW_[i].value_campo, ITEMS_DATOS_DROW_[i].id_value);
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
                        resolve("Time out error.");


                    } else if (textStatus === 'abort') {
                        resolve("Ajax request aborted.");


                    } else {
                        resolve("Ajax request aborted." + xception.responseText);
                    }
                }
            });
        } catch (ex) {
            resolve(ex.message + " funcion Service_REST_solicita_lista_flujo_defult");
        }
    })
    let result = await myPromise;
    return result;
}
const Service_REST_solicita_usuarios_relacionados_actividad_flujo = async (id_actividad_wf) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceWorkflow.asmx/Service_solicita_usuarios_relacionados_actividad_flujo', {
                data: "{'parameter':'" + id_actividad_wf + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_gestion !== "YES") {
                        $("#DropDownList_usurios_flujo").empty();
                        resolve(data.d[0].Error_gestion);
                    } else {
                        var ITEMS_DATOS_DROW_ = new Array();
                        $.each(data.d[0].Class_service_ilist_drowlist, function (k, v) {
                            ITEMS_DATOS_DROW_.push(v);
                        });

                        $("#DropDownList_usurios_flujo").empty();
                        var element_drow = document.getElementById("DropDownList_usurios_flujo");
                        for (var i = 0; i < ITEMS_DATOS_DROW_.length; i++) {
                            element_drow[i] = new Option(ITEMS_DATOS_DROW_[i].value_campo, ITEMS_DATOS_DROW_[i].id_value);
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
                        resolve("Time out error.");


                    } else if (textStatus === 'abort') {
                        resolve("Ajax request aborted.");


                    } else {
                        resolve("Ajax request aborted." + xception.responseText);
                    }
                }
            });
        } catch (ex) {
            resolve(ex.message + " funcion Service_REST_solicita_usuarios_relacionados_actividad_flujo");
        }
    })
    let result = await myPromise;
    return result;
}
///-----------Solicita activdades relacinadas al flujo de trabajo y a la actividad workflow a la que pertenece el usuario----////////// 
const Service_REST_solicita_actividades_fjujo_usuario = async (id_flujo, id_actividad_workflow) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceWorkflow.asmx/Service_solicita_actividades_fjujo_usuario', {
                data: "{'parameter':'" + id_flujo + "','id_actividad_workflow':'" + id_actividad_workflow + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_gestion !== "YES") {
                        $("#DropDownList_actividades_flujo_sii").empty();
                        resolve(data.d[0].Error_gestion);
                    } else {
                        var ITEMS_DATOS_DROW_ = new Array();
                        $.each(data.d[0].Class_service_ilist_drowlist, function (k, v) {
                            ITEMS_DATOS_DROW_.push(v);
                        });
                        $("#DropDownList_actividades_flujo_sii").empty();
                        var element_drow = document.getElementById("DropDownList_actividades_flujo_sii");
                        for (var i = 0; i < ITEMS_DATOS_DROW_.length; i++) {
                            element_drow[i] = new Option(ITEMS_DATOS_DROW_[i].value_campo, ITEMS_DATOS_DROW_[i].id_value);
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
                        resolve("Time out error.");


                    } else if (textStatus === 'abort') {
                        resolve("Ajax request aborted.");


                    } else {
                        resolve("Ajax request aborted." + xception.responseText);
                    }
                }
            });
        } catch (ex) {
            resolve(ex.message + " funcion Service_REST_solicita_usuarios_relacionados_actividad_flujo");
        }
    })
    let result = await myPromise;
    return result;
}
///-----------Registra tarea flujo ruta virtual SII----------------//////////////
const Service_REST_registro_flujo_trabajo_sii_virtual = async (parameter, codigo_barras) => {
    var serialice = JSON.stringify(parameter);
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceWorkflow.asmx/Service_registro_flujo_trabajo_sii_rue', {
                data: "{" + "'parameter':'" + serialice + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].error_result !== "YES") {
                        resolve(data.d[0].error_result);
                    } else {
                        delete_row_table("table_gestion_virtual", "CODIGOBARRAS", codigo_barras);
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
            resolve(ex.message + " funcion Service_REST_registro_flujo_trabajo_sii_rue");
        }
    })
    let result = await myPromise;
    return result;
}
///-----------Registra tarea flujo ruta rue SII----------------//////////////
const Service_REST_registro_flujo_trabajo_sii_rue = async (parameter,datos_recibo) => {
    var serialice = JSON.stringify(parameter);
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceWorkflow.asmx/Service_registro_flujo_trabajo_sii_rue', {
                data: "{" + "'parameter':'" + serialice + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].error_result !== "YES") {
                        resolve(data.d[0].error_result);
                    } else {
                        delete_row_table("table_gestion_rue", "RECIBO", datos_recibo);
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
            resolve(ex.message + " funcion Service_REST_registro_flujo_trabajo_sii_rue");
        }
    })
    let result = await myPromise;
    return result;
}
const Service_REST_solicita_actividades_workflow_flujo_inicio = async (id_actividad_wf) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceWorkflow.asmx/Service_solicita_actividades_workflow_flujo_inicio', {
                data: "{'parameter':'" + id_actividad_wf + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_gestion !== "YES") {
                        $("#DropDownList_actividades_flujo").empty();
                        $("#DropDownList_usurios_flujo").empty();
                        resolve(data.d[0].Error_gestion);
                    } else {
                        var ITEMS_DATOS_DROW_ = new Array();
                        $.each(data.d[0].Class_service_ilist_drowlist, function (k, v) {
                            ITEMS_DATOS_DROW_.push(v);
                        });
                        $("#DropDownList_actividades_flujo").empty();
                        $("#DropDownList_usurios_flujo").empty();
                        var element_drow = document.getElementById("DropDownList_actividades_flujo");
                        for (var i = 0; i < ITEMS_DATOS_DROW_.length; i++) {
                            element_drow[i] = new Option(ITEMS_DATOS_DROW_[i].value_campo, ITEMS_DATOS_DROW_[i].id_value);
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
                        resolve("Time out error.");


                    } else if (textStatus === 'abort') {
                        resolve("Ajax request aborted.");


                    } else {
                        resolve("Ajax request aborted." + xception.responseText);
                    }
                }
            });
        } catch (ex) {
            resolve(ex.message + " funcion Service_REST_solicita_actividades_workflow_flujo_inicio");
        }
    })
    let result = await myPromise;
    return result;
}
const Service_REST_Registra_tarea_ruta_SII = async (parameter) => {
    var serialice = JSON.stringify(parameter);
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceWorkflow.asmx/Service_registro_tarea_ruta_sii', {
                data: "{" + "'parameter':'" + serialice + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].error_result !== "YES") {
                        resolve(data.d[0].error_result);
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
            resolve(ex.message + " funcion Service_REST_Registra_tarea_ruta_SII");
        }
    })
    let result = await myPromise;
    return result;
}
const Service_REST_Registra_tarea_flujo_SII = async (parameter) => {
    var serialice = JSON.stringify(parameter);
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceWorkflow.asmx/Service_registro_tarea_flujo_sii', {
                data: "{" + "'parameter':'" + serialice + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].error_result !== "YES") {
                        resolve(data.d[0].error_result);
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
            resolve(ex.message + " funcion Service_REST_Registra_tarea_flujo_SII");
        }
    })
    let result = await myPromise;
    return result;
}
const Service_REST_eliminar_flujo_workflow_SII = async (parameter) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceWorkflow.asmx/Service_eliminar_flujo_workflow_SII', {
                data: "{" + "'parameter':'" + parameter + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].error_result !== "YES") {
                        resolve(data.d[0].error_result);
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
            resolve(ex.message + " funcion Service_REST_eliminar_flujo_workflow_SII");
        }
    })
    let result = await myPromise;
    return result;
}
const Service_REST_actualiza_datos_imagen_workflow_SII = async (parameter) => {
    let myPromise = new Promise( (resolve) => {
        try {
            $.ajax('../webservice/WebServiceWorkflow.asmx/Service_actualiza_datos_imagen_workflow_SII', {
                data: "{" + "'parameter':'" + parameter + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].error_result !== "YES") {
                        resolve(data.d[0].error_result);
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
            resolve(ex.message + " funcion Service_REST_actualiza_datos_imagen_workflow_SII");
        }
    })
    let result = await myPromise;
    return result;
}

//ZONA SERVICE BALANCEO DE CARGA
const Service_Solicita_lista_usuarios_workflow_balanceo = async (paremeter) => {
    let myPromise = new Promise((resolve) => {
    try {
        $.ajax('../webservice/WebServiceWorkflow.asmx/Service_Solicita_lista_usuarios_workflow_balanceo', {
            data: "{'parameter':'" + paremeter + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].result !== "YES") {    
                    resolve(data.d[0].result);
                } else {
                    init_row_constant_table_boostrap_table("table_balance", data.d[0].row_usuario_workflow_blance, "div_content_tabla_balance", "table-bordered", "table-borderless");
                    //let heigth_table = document.getElementById("div_content_tabla_balance").clientHeight;
                    //table_reize_heigth("table_balance", (heigth_table - 1), "table-borderless");
                    resolve(data.d[0].result);
                }
            },
            error: function (result) {
                resolve(result.innerHTML);
               
            }, compelete: function () {
                resolve("YES");
                
            }
        });
    } catch (ex) {

        resolve(ex.message + " funcion Service_Solicita_lista_usuarios_workflow_balanceo");

        }
    })
    let result = await myPromise;
    return result;
}

function Service_Activa_usuario_workflow_balanceo_grupo(paremeter) {
    try {
        $.ajax('../webservice/WebServiceWorkflow.asmx/Service_Activa_usuario_workflow_balanceo_grupo', {
            data: "{'parameter':'" + paremeter + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].result !== "YES") {
                    //mystopfunction_update_table_balance("table_balance", data.d[0].result);
                    alert(data.d[0].result);

                } else {
                    //REGISTRO = data.d;
                   // mystopfunction_update_table_balance("table_balance", "");

                }
            },
            error: function (result) {
                //mystopfunction_update_table_balance("table_balance", result.innerHTML);

            }, compelete: function () {
               // mystopfunction_update_table_balance("table_balance", "");

            }
        });
    } catch (ex) {

        alert(ex.message + " funcion Service_Activa_usuario_workflow_balanceo_grupo");

    }
}
function Service_Inactiva_usuario_workflow_balanceo_grupo(paremeter) {
    try {
        $.ajax('../webservice/WebServiceWorkflow.asmx/Service_Inactiva_usuario_workflow_balanceo_grupo', {
            data: "{'parameter':'" + paremeter + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].result !== "YES") {
                    //mystopfunction_update_table_balance("table_balance", data.d[0].result);
                    alert(data.d[0].result);
                } else {
                    //REGISTRO = data.d;
                    // mystopfunction_update_table_balance("table_balance", "");

                }
            },
            error: function (result) {
                //mystopfunction_update_table_balance("table_balance", result.innerHTML);

            }, compelete: function () {
                // mystopfunction_update_table_balance("table_balance", "");

            }
        });
    } catch (ex) {

        alert(ex.message + " funcion Service_Inactiva_usuario_workflow_balanceo_grupo");

    }
}
function Service_cambia_estado_asignacion_usuario_workflow(paremeter, estado_asig) {
    try {
        $.ajax('../webservice/WebServiceWorkflow.asmx/Service_cambia_estado_asignacion_usuario_workflow', {
            data: "{'parameter':'" + paremeter + "','estado_asig':'" + estado_asig + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].result !== "YES") {
                    //mystopfunction_update_table_balance("table_balance", data.d[0].result);
                    alert(data.d[0].result);
                } else {
                    //REGISTRO = data.d;
                    // mystopfunction_update_table_balance("table_balance", "");

                }
            },
            error: function (result) {
                //mystopfunction_update_table_balance("table_balance", result.innerHTML);

            }, compelete: function () {
                // mystopfunction_update_table_balance("table_balance", "");

            }
        });
    } catch (ex) {

        alert(ex.message + " funcion Service_cambia_estado_asignacion_usuario_workflow");

    }
}
function GetLista_listado_usuarios_workflow_ruta_asignacion(name_texbox) {
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
                    url: "../webservice/WebServiceWorkflow.asmx/GetLista_listado_usuarios_workflow_ruta_asignacion",
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
                document.getElementById("Button_consultar_usuario").click();
            }, minLength: 3, max: 10, scroll: true
        });
}
//----------Debijua y redimenciona la tabla que muestra la lista de versiones
function auto_zise_lista_documentos_sii() {
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
        let height_header = document.getElementById('header_modal_sii_rues_file').clientHeight;
        let height_footer = document.getElementById('footer_modal_sii_rues_file').clientHeight;
        let height_toolbar = document.getElementById('tool_bar_sii_rues_file').clientHeight;
        if (height_footer < 1) {
            height_footer = 73;
        }
        if (height_header < 1) {
            height_header = 73;
        }
        if (height_toolbar < 1) {
            height_toolbar = 73;
        }

        $('#content_tabl_lista_sii_rues_file').css("height", ((espacio_iframe - (height_header + height_header + height_toolbar))) + "px");
        $('#content_view_sii_rues_file').css("height", ((espacio_iframe - (height_header + height_header + height_toolbar))) + "px");
        let heig_table = (espacio_iframe - (height_header + height_header + height_toolbar));
        table_reize_heigth("tabl_lista_sii_rues_file", heig_table, "", "table-borderless");

    } catch (ex) { alert("Funcion auto_zise_lista_documentos_sii " + ex.message); }

}
function resize_table_boot_() {
    try {
        if (document.getElementById('div_content_tabla')) {
            let heigTable = document.getElementById('div_content_tabla').clientHeight - 20;
            $('#table').bootstrapTable('resetView', { height: heigTable });
        }
        if (document.getElementById('div_content_tabla_balance')) {
            let heigTable = document.getElementById('div_content_tabla_balance').clientHeight - 20;
            $('#table_balance').bootstrapTable('resetView', { height: heigTable });
        }
    }
    catch (err) {
        //alert(err.message + " resize_table_boot");
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

        $('#Panel_paginas_externas_popou').css("height", (espacio_iframe) + "px");
        $('#contenido_procesa_paginas_externas_popou').css("height", (espacio_iframe - 20) + "px");
        $('#Iframe_paginas_externas_popup_').css("height", (espacio_iframe - 20) + "px");

    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_paginas_externas_libres");
    }

}
function auto_zise_page_() {
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

        $('#pag_plantilla').css("height", (espacio_iframe - 1) + "px");
        $('.tabcontent').css("height", (espacio_iframe - (document.getElementById("tab_option_integracion").clientHeight + 30)) + "px");
        $('#conten_elimina_flujo').css("height", (document.getElementById("elimina_flujo").clientHeight - (document.getElementById("modal_foter_elimina_flujo").clientHeight + 10 + document.getElementById("title_elimina_flujo").clientHeight)) + "px");
        $('#conten_edita_flujo').css("height", (document.getElementById("edita_flujo").clientHeight - (document.getElementById("modal_foter_edita_flujo").clientHeight + 10 + document.getElementById("title_edita_flujo").clientHeight)) + "px");
        //-----resize registro flujo--------///
        $('#conten_registro_flujo').css("height", (document.getElementById("registro_flujo").clientHeight - (document.getElementById("modal_foter_registro_flujo").clientHeight + document.getElementById("title_registro_flujo").clientHeight + 30)) + "px");
        //-----resize registro ruta--------///
        $('#conten_registro_ruta').css("height", (document.getElementById("registro_ruta").clientHeight - (document.getElementById("modal_foter_registro_ruta").clientHeight + document.getElementById("title_registro_ruta").clientHeight + 30)) + "px");
        //------resize gestión tramites rues--------////
        $('#conten_gestion_rue').css("height", document.getElementById("tab_gestion_rue").clientHeight + "px");
        let heigth_table = document.getElementById("tab_gestion_rue").clientHeight - (document.getElementById("modal_foter_gestion_rue").clientHeight  + document.getElementById("title_gestion_rue").clientHeight);
        document.getElementById('div_content_tabla_gestion_rue').style.height = (heigth_table - 1) + "px";
        table_reize_heigth("table_gestion_rue", (heigth_table - 1), "table-borderless");
         //------resize gestión tramites virtuales--------////
        $('#conten_gestion_virtual').css("height", document.getElementById("tab_gestion_virtual").clientHeight + "px");
        heigth_table = document.getElementById("tab_gestion_virtual").clientHeight - (document.getElementById("modal_foter_gestion_virtual").clientHeight  + document.getElementById("title_gestion_virtual").clientHeight);
        document.getElementById('div_content_tabla_gestion_virtual').style.height = (heigth_table - 1) + "px";
        table_reize_heigth("table_gestion_virtual", (heigth_table - 1), "table-borderless");
        //-------resize gestión usuarios balanceo--------////
        $('#conten_consulta_balance').css("height", document.getElementById("tab_gestion_balance").clientHeight + "px");
        heigth_table = document.getElementById("tab_gestion_balance").clientHeight - (document.getElementById("conte_foter_balanceo").clientHeight + 20 + document.getElementById("div_contenido_controles_consulta_balance").clientHeight);
        document.getElementById('div_content_tabla_balance').style.height = (heigth_table - 1) + "px";
        table_reize_heigth("table_balance", (heigth_table - 1), "table-borderless");

        //-------resize reasigna tarea workflow--------////
        $('#conten_consulta_sii').css("height", document.getElementById("tab_reasigna_sii").clientHeight + "px");
        heigth_table = document.getElementById("tab_reasigna_sii").clientHeight - (document.getElementById("conte_foter_consulta").clientHeight + 20 + document.getElementById("div_contenido_controles_consulta").clientHeight);
        document.getElementById('div_content_tabla_balance').style.height = (heigth_table - 1) + "px";
        table_reize_heigth("table", (heigth_table - 1), "table-borderless");
        //$('#table').css("width", "100%");

    }
    catch (err) {
       alert(err.message + " Funcion auto_zise_page");  
    }
}