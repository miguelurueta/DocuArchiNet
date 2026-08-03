
$(document).ready(function () {
    $.fn.inicio = function () {
        auto_zise_consulta();
        auto_zise_version_document();
    }
});

//---ZONA DE VARIBLES GENERALES
let ITEMS_DATOS_DROW;
let ID_GABINETE_MIG=0;
let GABINETE_MIG = "";
let ID_IMAGEN_MIG = 0;
let ID_REGISTRO_MIGRA = 0;
let INDEX_ROW_TABLE = 0;
let DATOS_ARRAY_CAMBIA_TIPO;
let NAME_TABLE_BOT_MIGRA = $('#table_consulta_migracion');
let SEARCH_MIG_ARRAY_REGISTRO_MIGRACION = new Array();;
let SEARCH_MIG_ID_REG_VERSION_MIGRA_FUENTE;
let SEARCH_MIG_ID_REG_VERSION_MIGRA_DESTINO;
//-----------------------ZONA LOAD-------------------------------------------
$(window).on("load", function () {
    try {
        var elment = document.getElementsByClassName("da_event_captive");
        if (elment) {
            for (var i = 0; i < elment.length; i++) {
                elment[i].addEventListener("click", event_click, false);
            }
        }
        load_gabinete_versionado();             
        window.addEventListener("resize", rezize_event);
        ShowModalPopup("ModalPopupExtender_mensaje_personalizado_backgroundElement", "Panel_mensaje_personalizado", 100009);
        
    } catch (e) {
        alert(" funcion load " + e.message);
    }

});
//----------------------TERMNA ZONA LOAD-------------------------------------
//----------------------Zona export modulos----------------------------------

//----------------------Termina zona export modulos--------------------------
//----------------------ZONA EVENTOS-----------------------------------------

//---------Inicializa la carga del formulario de gestion de migracion y versionamiento
const load_gabinete_versionado = async () => {
    ID_GABINETE_MIG = 0;
    GABINETE_MIG = "";
    ID_IMAGEN_MIG = 0;
    ID_REGISTRO_MIGRA = 0;
    ini_event_page();
    ini_event_version_document();
    let result = await Service_REST_solicita_gabinetes_migracion(1);
    if (result !== "YES") {
        alert_bot(result, 'warning', "contenido_controles_consulta");
    }
}
const ini_event_page = () => {   
    let array_element = new Array;
    array_element.push({ id: "Button_restore_gabinete" }, { id: "Button_search_gabinete" }, { id: "Button_search_gabinete_general" },
        { id: "Button_update_reload_docuent_migra" }, { id: "Button_migra_formato_document" }, { id: "Button_activa_adjunta_document_remplazo" },
        { id: "Button_remplaza_version_documento" }, { id: "Button_activa_migra_vincula_document" }, { id: "Button_vincula_migra_documento" },
        { id: "Button_active_update_index_bacth" }, { id: "Button_cambia_tipologia_documental" }, { id: "Button_actualiza_indice_mig" },
        { id: "Button_migra_remplaza_version_documento" }, { id: "Button_activa_cambia_tipologia" }, { id: "Button_search_registro_migracion_lik" },
        { id: "Button_restore_consulta" }, { id: "Button_activa_digitaliza_document_remplazo" }, { id:"save_document_scan"}
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
            //Guarda documento escaneado
            case "save_document_scan":
                event_element_click_promise(e);
                break;
            //Activa interface de escaner 
            case "Button_activa_digitaliza_document_remplazo":
                event_element_click_promise(e);
                break;
            //Activa el restore search de la tabla de consulta
            case "Button_restore_consulta":
                event_element_click_promise(e);
                break;
            //Activa consulta like registro migracion
            case "Button_search_registro_migracion_lik":
                let vaLueText = document.getElementById("textBox_buequeda_general_migra").value;
                search_gabinete_general_auto_complete_migracion(vaLueText);
                break;
            //Activa cambia tioplogia 
            case "Button_activa_cambia_tipologia":
                if (GABINETE_MIG == "") { return true; }
                if (ID_IMAGEN_MIG == 0) { return true; }
                if (ID_GABINETE_MIG == 0) { return true; }
                active_cambia_tipologia(ID_IMAGEN_MIG, ID_GABINETE_MIG, GABINETE_MIG);
                break;
            //Migra y remplaza version de documentos por lotes
            case "Button_migra_remplaza_version_documento":
                if (GABINETE_MIG == "") { return true; }
                let $table_migra_remplaza = $('#table_consulta_migracion');
                let selection_migra_remplaza = new Array();
                selection_migra_remplaza = $table_migra_remplaza.bootstrapTable('getSelections');
                if (selection_migra_remplaza.length > 0 ) {
                    event_multiple_row(selection_migra_remplaza, GABINETE_MIG, "migra_remplaza_documento_batch_migracion");
                }
                break;
            //Actualiza tipologia documental
            case "Button_cambia_tipologia_documental":
                if (GABINETE_MIG == "") { return true; }
                event_element_click_promise(e);
                break;
            //Actualiza index bacth
            case "Button_actualiza_indice_mig":
                if (GABINETE_MIG == "") { return true; }
                let $table_indice_batch = $('#table_consulta_migracion');
                let selection_indice_batch = new Array();
                selection_indice_batch = $table_indice_batch.bootstrapTable('getSelections');
                valida_solicita_datos_control_general("form_control_indice_docuarchi");
                ITEM_GENERAL_CONTROL_ARRAY_DIFERENT = new Array();
                ITEM_GENERAL_CONTROL_ARRAY_DIFERENT = Detec_chanque_valor_campo();
                if (selection_indice_batch.length > 0 && ITEM_GENERAL_CONTROL_ARRAY_DIFERENT.length > 0) {
                    event_multiple_row(selection_indice_batch, GABINETE_MIG, "actualiza_indice_batch_migracion");
                }
                break;
            //Activa update index batch
            case "Button_active_update_index_bacth":
                if (GABINETE_MIG == "") { return true; }
                let $table_indice = $('#table_consulta_migracion');
                let selection_indice = new Array();
                selection_indice = $table_indice.bootstrapTable('getSelections');   
                if (selection_indice.length > 0) {
                    ID_IMAGEN_MIG = selection_indice[0].ID;
                    event_element_click_promise(e);
                }
                break;
            //Activa vinculación documentos expedientes
            case "Button_activa_migra_vincula_document":
                if (GABINETE_MIG == "") { return true; }
                let selection = new Array();
                let $table = $('#table_consulta_migracion');
                selection = $table.bootstrapTable('getSelections');
                if (selection.length > 0 && ID_GABINETE_MIG != 0) {
                    event_element_click_promise(e);
                }
                break;
            //Confirma vinculación documento a expediente
            case "Button_vincula_migra_documento":
                if (GABINETE_MIG == "") { return true; }
                let selection_a;
                let $table_a = $('#table_consulta_migracion');
                selection_a = JSON.stringify($table_a.bootstrapTable('getSelections'));
                //console.log(selection);
                if (selection_a.length > 2 && ID_GABINETE_MIG != 0) {
                    event_element_click_promise(e);
                }
                break;
            //restaura valores consulta gabinete
            case "Button_restore_gabinete":
                name_espace_class  = GABINETE_MIG + "_search_gabinet_" + ID_GABINETE_MIG;
                result = restore_value_form_control(name_espace_class);
                if (result !== "YES") {
                    alert_bot(result, 'warning', "contenido_controles_consulta");
                }
                break;
            case "Button_search_gabinete":
                //Realiza la busqueda en el gabinete
                if (GABINETE_MIG == "") { return true; }
                name_espace_class = GABINETE_MIG + "_search_gabinet_" + ID_GABINETE_MIG;
                result = search_form_control_gabinete(name_espace_class);
                if (result !== "YES") {
                    alert_bot(result, 'warning', "contenido_controles_consulta");
                } else {
                    event_element_click_promise(e);
                }
                break;
            //Realiza la busqueda general en el gabinete
            case "Button_search_gabinete_general":
                if (GABINETE_MIG == "") { return true; }
                event_element_click_promise(e);
                break;
            //Realiza la actualiacion de la carga del documento a migrar
            case "Button_update_reload_docuent_migra" :
                if (GABINETE_MIG == "" || ID_IMAGEN_MIG == 0) { return true; }
                event_element_click_promise(e);
                break;
            //Realiza la mmigracion de formato
            case "Button_migra_formato_document" :
                if (GABINETE_MIG == "" || ID_IMAGEN_MIG == 0) { return true; }
                event_element_click_promise(e);
                break;
            //Activa carga de archivos para remplazo
            case "Button_activa_adjunta_document_remplazo":
                if (GABINETE_MIG == "" || ID_IMAGEN_MIG == 0) { return true; }
                event_element_click_promise(e);
                break;
            case "Button_remplaza_version_documento" :
                if (GABINETE_MIG == "" || ID_IMAGEN_MIG == 0 || ID_REGISTRO_MIGRA==0) { return true; }
                event_element_click_promise(e);
                break;
        }
    } catch (ex) {
        alert(ex.mensaje);
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
//-----------Eventos consulta gabinete----------------------------
const event_change_drowslis_gabinete_migracion = async (e) => {
    delete_alert_boot();
    let ecourrent = e.currentTarget;
    let value_e = ecourrent.value;
    let text_e = ecourrent.options[ecourrent.selectedIndex].label;
    ID_GABINETE_MIG = value_e;
    GABINETE_MIG = text_e;
    posicion_update_pogres('progres_bar');
    if (document.getElementById("state_migracion") !== null) {
        document.getElementById("state_migracion").innerText = "Estado";
    }
    let  result = await add_form_control_search_gabinete(ID_GABINETE_MIG, "WebServiceDocuarchi.asmx", "Service_lista_interface_busqueda_gabinete", "", "div_consulta_gabinetes_migracion", 0,
        "consulta_gabinetes_migracion", 0, "div_consulta_gabinetes_migracion", "", "", 0, 1, 1);
    if (result !== "YES") {
        alert_bot(result, 'warning', "contenido_controles_consulta");
        progres_hiden('progres_bar');
        return true
    } 
    if (ID_GABINETE_MIG == 0 || ID_GABINETE_MIG == -1) {
        destroy_table_bootstrap_table("table_consulta_migracion", "contenido_table_boot_migracion");
        progres_hiden('progres_bar');
        return true
    }
    result = await Service_REST_estructura_campos_dynamic_migracion(ID_GABINETE_MIG, "table_consulta_migracion", "contenido_table_boot_migracion");
    if (result !== "YES") {
        alert_bot(result, 'warning', "contenido_controles_consulta");
        progres_hiden('progres_bar');
        return true
    }
   
    Service_REST_auto_complete_gabinete_migracion("textBox_buequeda_general_migra", "DA", GABINETE_MIG, "");
    progres_hiden('progres_bar');
}
const event_element_click_promise = async (e) => {
    let name_control = e.currentTarget.id;
    try {
        let result = "";    
        delete_alert_boot();
        e.currentTarget.disabled = true;
        posicion_update_pogres('progres_bar');
        if (name_control == "Button_restore_consulta") {    
            restore_value_form_control("consulta_gabinetes_migracion");      
        }
        //Activa la interface de actualización de indice
        if (name_control == "Button_active_update_index_bacth") {
            let parameter_Service = new Array();
            parameter_Service.push({
                id_registro: ID_IMAGEN_MIG, class_service: "WebServiceDocuarchi.asmx", name_service: "Service_crea_interface_indice_migracion",
                name_container: "div_actualiza_indice_batch_mig",name_control_padre: "modal_actualiza_indice_batch_mig",
                asigna_valor: 1, apost_name_content: "actualiza_indice_batch_mig", add_check: 1, name_table: GABINETE_MIG, class_name_control:"form_control_indice_docuarchi"
            });
            result = await Service_REST_interface_form_clontrol_bootStrap(parameter_Service);
            if (result !== "YES") {
                alert_bot(result, 'warning', "error_content_migracion");
            }
        }
        //Cambia la tipologia documental
        if (name_control == "Button_cambia_tipologia_documental") {
            DATOS_ARRAY_CAMBIA_TIPO = new Array();
            let option_cambia_tipo_serie = "";
            let id_serie = 0;
            let nombre_serie = "";
            if (document.getElementById("option_cambia_tipo_serie_documental")) {
                option_cambia_tipo_serie = document.getElementById("option_cambia_tipo_serie_documental");
                nombre_serie = option_cambia_tipo_serie.options[option_cambia_tipo_serie.selectedIndex].text;
                id_serie = option_cambia_tipo_serie.options[option_cambia_tipo_serie.selectedIndex].value;
            }
            let id_sub_serie = 0;
            let nombre_sub_serie = "";
            if (document.getElementById("option_cambia_tipo_sub_serie_documental")) {
                option_cambia_tipo_serie = document.getElementById("option_cambia_tipo_sub_serie_documental");
                nombre_sub_serie = option_cambia_tipo_serie.options[option_cambia_tipo_serie.selectedIndex].text;
                id_sub_serie = option_cambia_tipo_serie.options[option_cambia_tipo_serie.selectedIndex].value;
            }
            let id_tipo_documental = 0;
            let nombre_tipo_documental = "";
            if (document.getElementById("option_ambia_tipo_documental")) {
                option_cambia_tipo_serie = document.getElementById("option_ambia_tipo_documental");
                nombre_tipo_documental = option_cambia_tipo_serie.options[option_cambia_tipo_serie.selectedIndex].text;
                id_tipo_documental = option_cambia_tipo_serie.options[option_cambia_tipo_serie.selectedIndex].value;
            }
            DATOS_ARRAY_CAMBIA_TIPO.push({
                id_serie: id_serie, nombre_serie: nombre_serie, id_sub_serie: id_sub_serie, nombre_sub_serie: nombre_sub_serie,
                id_tipo_documental: id_tipo_documental, nombre_tipo_documental: nombre_tipo_documental, Gabinete: GABINETE_MIG, id_imagen: ID_IMAGEN_MIG
            });
            result = await Service_REST_actualiza_tipologia_migracion(DATOS_ARRAY_CAMBIA_TIPO);
            if (result != "YES") {
                alert_bot(result, 'warning', "error_cambiar_tipologia_documento");
            }
        }
        //Activa la vinculación de documentos seleccionados a expediente
        if (name_control == "Button_activa_migra_vincula_document") {
            result = await Service_REST_lista_tramite_auto_vinculacion_gabinete(ID_GABINETE_MIG);
            
        }
        //Activa registro expediente para vinculacón de documentos
        if (name_control == "Button_vincula_migra_documento") {
            let selection_;
            let $table_ = $('#table_consulta_migracion');
            selection_ = $table_.bootstrapTable('getSelections');  
            let ElementOption = document.getElementById("option_tramite_vincula");
            let Id_auto_registro = 0;
            d_auto_registro = ElementOption.options[ElementOption.selectedIndex].value;
            if (d_auto_registro == 0) {
                alert_bot("Debe seleccionar el tipo de vinculación", 'warning', "error_tipo_tramite_vinculacion");
                return true;
            }
            result = await Service_REST_auto_registra_gabinete_expediente(ID_GABINETE_MIG, GABINETE_MIG, d_auto_registro, selection_[0].ID, selection_,'table_consulta_migracion');
            if (result != "YES") {
                alert_bot(result, 'warning', "error_tipo_tramite_vinculacion");
            }
        }
        //Consulta gabinete migración
        if (name_control == "Button_search_gabinete") {
            result = await Service_REST_consulta_gabinete_migracion(ITEM_GENERAL_CONTROL_ARRAY, 1, "", "table_consulta_migracion", ID_GABINETE_MIG, "contenido_table_boot_migracion");
            if (result !== "YES") {
                alert_bot(result, 'warning', "contenido_controles_consulta");
            }
        }
        //Consulta general gaibinete migración
        if (name_control == "Button_search_gabinete_general") {
            let text_search = document.getElementById("textBox_buequeda_general_migra").value;
            result = await Service_REST_consulta_gabinete_migracion(ITEM_GENERAL_CONTROL_ARRAY, 2, text_search, "table_consulta_migracion", ID_GABINETE_MIG, "contenido_table_boot_migracion");
            if (result !== "YES") {
                alert_bot(result, 'warning', "error_content_migracion");
            }
        }
        //Actualiza visualizacion imagen a migrar
        if (name_control == "Button_update_reload_docuent_migra") {
            result = await Service_REST_Lista_documentos_visor_a_migrar(ID_IMAGEN_MIG, GABINETE_MIG);
            if (result !== "YES") {
                alert_bot(result, 'warning', "error_content_popup_migracion");
            }
        }
        //Activa interfaz de digitalización 
        if (name_control == "Button_activa_digitaliza_document_remplazo") {
            result = await Service_REST_inicializa_intefaz_escaner(ID_IMAGEN_MIG, GABINETE_MIG);
            if (result !== "YES") {
                alert_bot(result, 'warning', "error_content_popup_migracion");
            }
        }
        //Gaurda documento digitalizado 
        if (name_control == "save_document_scan") {
            result = await Service_REST_guarda_documento_digitalizado(ID_IMAGEN_MIG, GABINETE_MIG);
            if (result !== "YES") {
                alert_bot(result, 'warning', "error_content_scan_document_migracion");
            }
        }
        //Migra formato documento 
        if (name_control == "Button_migra_formato_document") {
            result = await Service_REST_migra_formato_documento(ID_IMAGEN_MIG, GABINETE_MIG,1);
            if (result !== "YES") {
                alert_bot(result, 'warning', "error_content_popup_migracion");
            }
        }
        //Activa evento de carga de archivo migracion -
        if (name_control == "Button_activa_adjunta_document_remplazo") {
            result = await InitUploadFileClientAsync("adjunta_documeto_migra","start_file_save_UploadFile");
            if (result !== "YES") {
                alert_bot(result, 'warning', "error_content_popup_migracion");
                return true;
            }
            result = await service_REST_parameter_upload_boot("MIGRACION", "modal_adjunta_documeto_migra", "", "adjunta_documeto_migra");
            if (result !== "YES") {
                alert_bot(result, 'warning', "error_content_popup_migracion");

            }
        }
        //Remplaza documento migrado
        if (name_control == "Button_remplaza_version_documento") {
            e.currentTarget.disabled = true;
            posicion_update_pogres('progres_bar');
            if (ID_REGISTRO_MIGRA == -1 || ID_REGISTRO_MIGRA == 0) {
                return true;
            }
            result = await Service_REST_remplaza_version_documento(ID_IMAGEN_MIG, GABINETE_MIG, ID_REGISTRO_MIGRA);
            if (result !== "YES") {
                alert_bot(result, 'warning', "error_content_popup_migracion");
            } else {
                $("#modal_migracion").modal("hide");
            }
        }
        
    }
    catch (ex) {    
        alert_bot(ex.message, 'warning', "error_content_migracion");      
    } finally {
        document.getElementById(name_control).disabled = false;
        progres_hiden('progres_bar');
    }
}
//-----------Termina Eventos consulta gabinete --------------------------
//-----------Zona load file archivos-------------------------------------
const start_file_save_UploadFile = () => {
   let funcion_name = "adjunta_migra_documento";
   let evento_adjunta = "MIGRACION";
   let element_html_actuliza = "";
   let tipo_adjunta = -1;
   let id_respuesta = 0;
   let element_update_panel = "";
   let id_tipo_docuental = -1;
   let element_parent = "modal_adjunta_documeto_migra";
   let numero_documento_relacionado = - 1;
   let estado_adjunto = -1;
   let estado_relacion = -1;
   let element_isert_table = "modal_adjunta_documeto_migra";
    star_copy_interval_file_Upload(estado_adjunto, estado_relacion, id_tipo_docuental, funcion_name, element_parent, evento_adjunta,
        numero_documento_relacionado, element_html_actuliza, element_update_panel, id_respuesta, tipo_adjunta, element_isert_table, "", GABINETE_MIG, ID_IMAGEN_MIG);
}
const search_gabinete_general_auto_complete_migracion = async (value_search) => {
    try {
    posicion_update_pogres('progres_bar');
    let result = await Service_REST_consulta_gabinete_migracion(ITEM_GENERAL_CONTROL_ARRAY, 2, value_search, "table_consulta_migracion", ID_GABINETE_MIG, "contenido_table_boot_migracion");
        
    if (result !== "YES") {
        progres_hiden('progres_bar');
        alert_bot(result, 'warning', "error_content_migracion");

    } else {
        progres_hiden('progres_bar');     
    }
    }
    catch (ex) {
        progres_hiden('progres_bar');
        alert_bot(ex.message, 'warning', "error_content_migracion");
    } finally {

    }
}
//-----------Eventos migracion gabinete--------------------------------
const load_interfaz_document_migracion = async (id_imagen) => {
    try {
        let result = "";
        posicion_update_pogres('progres_bar');
        result = await Service_REST_solicita_estructura_lista_documento_migrado(id_imagen, GABINETE_MIG);
        if (result !== "YES") {
            alert_bot(result, 'warning', "error_content_migracion");
            return true;
        }
        result = await Service_REST_Lista_documentos_visor_a_migrar(id_imagen, GABINETE_MIG);
        if (result !== "YES") {
            alert_bot(result, 'warning', "error_content_migracion");
            return true;
        }
        $("#modal_migracion").modal("show");
        
    } catch (ex) {  
        alert_bot(ex.message, 'warning', "error_content_migracion");
    } finally {
        progres_hiden('progres_bar');
    }
   
}
const load_visor_cosulta_migracion = async (id_imagen, gabinete) => {
    try {
        let result = "";
        posicion_update_pogres('progres_bar');
        result = await Service_REST_Lista_documentos_visor_consulta_migracion(id_imagen, gabinete);
        if (result !== "YES") {
            progres_hiden('progres_bar');
            alert_bot(result, 'warning', "error_content_migracion");
            return true;
        }
        $("#modal_visor_migracion_documento").modal("show");
        progres_hiden('progres_bar');
    } catch (ex) {
        progres_hiden('progres_bar');
        alert_bot(ex.message, 'warning', "error_content_migracion");
    }
}
//-----------Termina Eventos migracion gabinete---------------------------
//-----------Eventos cambia tipologia documental--------------------------
const event_change_drowslisi_lista_tipos_documentales = async (e) => {
    try {
        delete_alert_boot();
        let ecourrent = e.currentTarget;
        let value_e = ecourrent.value;
        let text_e = ecourrent.options[ecourrent.selectedIndex].label;
        posicion_update_pogres('progres_bar');
        let result = await Service_REST_Solicita_lista_tipos_documentales_relacionados_id_sub_serie(value_e);
        if (result !== "YES") {
            alert_bot(result, 'warning', "error_cambiar_tipologia_documento");
            return true
        }
       
    } catch (ex) {
        alert_bot(ex.mensaje, 'warning', "error_cambiar_tipologia_documento");
    } finally {
        progres_hiden('progres_bar');
    }
}
const event_change_drowslisi_lista_sub_series = async (e) => {
    try {
        delete_alert_boot();
        let ecourrent = e.currentTarget;
        let value_e = ecourrent.value;
        let text_e = ecourrent.options[ecourrent.selectedIndex].label;
        posicion_update_pogres('progres_bar');
        let result = await Service_REST_Solicita_lista_sub_series_documentales_id_serie(value_e);
        if (result !== "YES") {
            alert_bot(result, 'warning', "error_cambiar_tipologia_documento");
            return true
        }
        
    } catch (ex) {
        alert_bot(ex.mensaje, 'warning', "error_cambiar_tipologia_documento");
    } finally {
        progres_hiden('progres_bar');
    }
}
const event_change_drowslisi_lista_gestion_auto_registro = async (e) => {
    try {
        delete_alert_boot();
        let ecourrent = e.currentTarget;
        let value_e = ecourrent.value;
        let text_e = ecourrent.options[ecourrent.selectedIndex].label;
        posicion_update_pogres('progres_bar');
        let result = await Service_REST_Service_Solicita_gestion_autoregistro_gabinete(value_e);
        if (result !== "YES") {
            alert_bot(result, 'warning', "error_cambiar_tipologia_documento");
            return true
        }
        
    } catch (ex) {
        alert_bot(ex.mensaje, 'warning', "error_cambiar_tipologia_documento");
    } finally {
        progres_hiden('progres_bar');
    }
}
//-----------TerminaEventos cambia tipologia documental ------------------
const active_cambia_tipologia = async (id_imagen, id_gabinete, gabinete) => {
    try {
        let result = "";
        posicion_update_pogres('progres_bar');
        result = await Service_REST_Solicita_lista_series_relacionadas_gabinete_migracion(id_imagen, gabinete, id_gabinete);
        if (result !== "YES") {
            progres_hiden('progres_bar');
            alert_bot(result, 'warning', "error_content_migracion");
            return true;
        }
        progres_hiden('progres_bar');
    } catch (ex) {
        progres_hiden('progres_bar');
        alert_bot(ex.message, 'warning', "error_content_migracion");
    }
   
}
const Show_detail_registro_migracion = async (id_gabinete, gabinete) => {
    try {
        let result = "";
        posicion_update_pogres('progres_bar');
        result = await Service_REST_solicita_estructura_registro_migracion_documento_gestion(id_gabinete, gabinete)
        if (result !== "YES") {
            progres_hiden('progres_bar');
            alert_bot(result, 'warning', "error_content_migracion")
            return true;
        }
        if (SEARCH_MIG_ARRAY_REGISTRO_MIGRACION) {
            document.getElementById("spn_id_registro_migracion").innerText = SEARCH_MIG_ARRAY_REGISTRO_MIGRACION.id_registro_migracion;
            document.getElementById("spn_fecha_registro").innerText = SEARCH_MIG_ARRAY_REGISTRO_MIGRACION.fecha_registro;
            document.getElementById("spn_user_loguin").innerText = SEARCH_MIG_ARRAY_REGISTRO_MIGRACION.user_loguin;
            document.getElementById("spn_nombre_gabinete").innerText = SEARCH_MIG_ARRAY_REGISTRO_MIGRACION.nombre_gabinete;
            document.getElementById("spn_id_imagen").innerText = SEARCH_MIG_ARRAY_REGISTRO_MIGRACION.id_imagen;
            document.getElementById("spn_aplica_ocr").innerText = SEARCH_MIG_ARRAY_REGISTRO_MIGRACION.aplica_ocr;
            document.getElementById("spn_aplica_compresion").innerText = SEARCH_MIG_ARRAY_REGISTRO_MIGRACION.aplica_compresion;
            document.getElementById("spn_version_pdf").innerText = SEARCH_MIG_ARRAY_REGISTRO_MIGRACION.version_pdf;
            document.getElementById("spn_valor_campo_gabinete").innerText = SEARCH_MIG_ARRAY_REGISTRO_MIGRACION.valor_campo_gabinete;
            document.getElementById("spn_nombre_archivo").innerText = SEARCH_MIG_ARRAY_REGISTRO_MIGRACION.nombre_archivo;
            document.getElementById("spn_Extension_doc_migrado").innerText = SEARCH_MIG_ARRAY_REGISTRO_MIGRACION.Extension_doc_migrado;
            document.getElementById("spn_num_page_anterior").innerText = SEARCH_MIG_ARRAY_REGISTRO_MIGRACION.num_page_anterior;
            document.getElementById("spn_num_page_nuevo").innerText = SEARCH_MIG_ARRAY_REGISTRO_MIGRACION.num_page_nuevo;
            document.getElementById("spn_leng_file").innerText = SEARCH_MIG_ARRAY_REGISTRO_MIGRACION.leng_file;
            document.getElementById("spn_fecha_registro_elimina_doc_fuente").innerText = SEARCH_MIG_ARRAY_REGISTRO_MIGRACION.fecha_registro_elimina_doc_fuente;
            document.getElementById("spn_user_loguin_elimina_doc_fuente").innerText = SEARCH_MIG_ARRAY_REGISTRO_MIGRACION.user_loguin_elimina_doc_fuente;
        }
        $("#modal_detalle_registro_migracion").modal("show");


    }
    catch (ex) {

        alert_bot(ex.message, 'warning', "error_content_migracion");
    } finally {
        progres_hiden('progres_bar');
    }
}
//-------------------ZONA EVENTOS TABLE BOOT---------------------------
function operateFormattertablebootmig(value, row, index) {   
    return [
        '<div class="row pl-2">',
        '<div class="col-8 p-0">',
        '<a class="active_view_document nav-link pl-5 justify-content-end font-weight-light" style="color: black" href="javascript:void(0)" title="Visualiza versión documento">  <i style="color: black" class="fal fa-file-image"></i>  </a>',
        '</div > ',
        '<div class="col-4 p-0">',
        '<a class="nav-link  dropdown-toggle justify-content-start" style="color: black" href="#"  data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: black; display:none" class="fad fa-th-list"></i>  ',
        '</a>',
        '<div class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">',
        '<a class="active_view_version dropdown-item font-weight-light" href="javascript:void(0)" title="Lista versiones del documento">  <i style="color: #black" class="far fa-folder-open"></i> Versiones del documento </a>',
        '<a class="active_view_detail_row dropdown-item font-weight-light" href="javascript:void(0)" title="Lista datos de registro de migración">  <i style="color: #black" class="fal fa-list"></i> Registro de migracion </a>',,
        '<a class="active_migra_change_tipologia btn dropdown-item font-weight-light" href="javascript:void(0)" title="Cambiar tipologia del documento">  <i style="color: #black" class="fal fa-file-edit"></i> Cambiar tipologia documental </a>',
        '<a class="active_migra_documento dropdown-item font-weight-light" href="javascript:void(0)" title="Migra de formato y remplaza versión del documento">  <i style="color: #black" class="fal fas fa-clone"></i> Migra formato de documento </a>',
        '<a style="color: black" href="#" class="dropdown-item font-weight-light"><i class="far fa-sign-out"></i> Salir del menu</a>',
        '</div>',
        '</a>',
        '</div>',
        '</div>',
    ].join('')
}
window.operateEvents = {
    'click .active_migra_change_tipologia': (e, value, row, index) => {
        delete_alert_boot();
        let ident = table_boot_return_objet_jonson(row);
        ID_IMAGEN_MIG = ident.ID;
        active_cambia_tipologia(ID_IMAGEN_MIG, ID_GABINETE_MIG, GABINETE_MIG);
        
    }, 'click .active_migra_documento': (e, value, row, index) => {
        delete_alert_boot();
        let ident = table_boot_return_objet_jonson(row);
        ID_IMAGEN_MIG = ident.ID;
        INDEX_ROW_TABLE = index;
        load_interfaz_document_migracion(ident.ID);

    }, 'click .active_view_version': (e, value, row, index) => {
        delete_alert_boot();
        let ident = table_boot_return_objet_jonson(row);
        ID_IMAGEN_MIG = ident.ID;
        INDEX_ROW_TABLE = index;
        list_show_version_document(ident.ID, ID_GABINETE_MIG,"error_content_version_documento",1);
    }, 'click .active_view_document': (e, value, row, index) => {
        delete_alert_boot();
        let ident = table_boot_return_objet_jonson(row);
        ID_IMAGEN_MIG = ident.ID;
        load_visor_cosulta_migracion(ID_IMAGEN_MIG, GABINETE_MIG);
       
    }, 'click .active_view_detail_row': (e, value, row, index) => {
        delete_alert_boot();
        let ident = table_boot_return_objet_jonson(row);
        ID_IMAGEN_MIG = ident.ID;
        Show_detail_registro_migracion(ID_IMAGEN_MIG, GABINETE_MIG);
    }
}

//-------------TERMINA ZONA EVENTOS TABLE BOOT--------------------

//-------------ZONA WEB SERVICE-----------------------------------
//Service agregar controles interfaz consulta de gabinete
//----------------------------------------------------------------

//-------Solicita lista gabinetes migracion
const Service_REST_solicita_gabinetes_migracion = async (id_) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceDocuarchi.asmx/Service_solicita_gabinetes_migracion', {
                data: "{'id':" + "'" + id_ + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].error_sistema !== "YES") {
                        
                        resolve(data.d[0].error_sistema);
                    } else {

                        ITEMS_DATOS_DROW = new Array();
                        $.each(data.d[0].item_sistema, function (k, v) {
                            ITEMS_DATOS_DROW.push(v);
                        });
                        if (document.getElementById('option_gabinet')) {
                            var element_drow = document.getElementById('option_gabinet');
                            $("#option_gabinet").empty();
                            for (var i = 0; i < ITEMS_DATOS_DROW.length; i++) {
                                element_drow[i] = new Option(ITEMS_DATOS_DROW[i].text, ITEMS_DATOS_DROW[i].value);
                            }
                            element_drow.addEventListener("change", event_change_drowslis_gabinete_migracion);
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
                        resolve ("Ajax request aborted.");
                      

                    } else {
                        resolve ("Ajax request aborted." + xception.responseText);
                       

                    }
                }
            });
        }
        catch (ex) {
            resolve(ex.message);
        }
    })
    let result =  myPromise;
    return result;
}
//-------Solicita estructura campo tabla BOOT head
const Service_REST_estructura_campos_dynamic_migracion = async (id_gabinete, name_table, name_parent_table) => {
    let myPromise = new Promise(function (resolve) {
    try {
        $.ajax('../webservice/WebServiceDocuarchi.asmx/Service_estructura_campos_dynamic_migracion', {
            data: "{'id_gabinete':" + "'" + id_gabinete + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].Error_result !== "YES") {
           
                    if (document.getElementById("state_migracion") !== null) {
                        document.getElementById("state_migracion").innerText = "Estate";
                    }
                    resolve(data.d[0].Error_result);
                } else {
                    init_feld_table_boostrap_table(name_table, data.d[0].Obj_ilist_fileds_generic, name_parent_table, "", "table-borderless");
                    if (document.getElementById("state_migracion") !== null) {
                        document.getElementById("state_migracion").innerText = "Estate";
                    }  
                    resolve("YES");
                    
                }
            }, error: function (xception, textStatus, errorThrown) {

                if (xception.status === 0) {
                    resolve("Not connect: Verify Network.");


                } else if (xception.status == 404) {
                    resolve("Requested page not found [404]");


                } else if (xception.status == 500) {
                    return("Internal Server Error [500]." + xception.responseText);


                } else if (textStatus === 'parsererror') {
                    resolve("Requested JSON parse failed.");


                } else if (textStatus === 'timeout') {
                    resolve ("Time out error.");


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

//-------Realiza la consulta de gabinetes de migracion y retorna los resultados
const Service_REST_consulta_gabinete_migracion = async (control_array_config_service, tipo_consulta, valor_consulta, name_table, id_gabinete, name_parent_table) => {
    let serialice = JSON.stringify(control_array_config_service);
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceDocuarchi.asmx/Service_consulta_gabinete_migracion', {
                data: "{" + "'parameter':'" + serialice + "','" + "tipo_consulta':'" + tipo_consulta + "','" + "valor_consulta':'" + valor_consulta + "','" + "id_gabinete':'" + id_gabinete + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_result !== "YES") {
                        resolve(data.d[0].Error_result);
                        if (document.getElementById("state_migracion") !== null) {
                            document.getElementById("state_migracion").innerText = "Estado";
                        }
                    } else {
                        let class_stru_row_Gabinete_Generic = JSON.parse(data.d[0].Obj_ilist_row_generic);
                        if (document.getElementById("state_migracion") !== null) {
                            document.getElementById("state_migracion").innerText = class_stru_row_Gabinete_Generic.length + " Registro (s) de Gabinete : " + GABINETE_MIG;
                        }
                        init_row_feld_table_boostrap_table(name_table, data.d[0].Obj_ilist_fileds_generic, class_stru_row_Gabinete_Generic, name_parent_table, "table-bordered", "table-borderless");
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
//-------Agrega el auto complete en la consulta general del gabinete
const Service_REST_auto_complete_gabinete_migracion = async (name_control, name_dbs_auto, name_table_auto, name_campo_auto)=> {
    var ITEM_SERIAL = new Array();
    ITEM_SERIAL.push({
        "name_dbs_auto": name_dbs_auto, "name_table_auto": name_table_auto, "name_campo_auto": name_campo_auto,
        value_auto: document.getElementById(name_control).value
    })
    var serialice = JSON.stringify(ITEM_SERIAL);
    function split(val) {
        return val.split(/,\s*/);
    }
    function extractLast(term) {
        return split(term).pop();
    }
    $("#" + name_control)
        .on("keydown", function (event) {
            if (event.keyCode === $.ui.keyCode.TAB &&
                $(this).autocomplete("instance").menu.active) {
                event.preventDefault();
            }
        })
        .autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: "../webservice/WebServiceDocuarchi.asmx/Service_auto_complete_gabinete_migracion",
                    data: "{'parameter':'" + serialice + "','value':'" + document.getElementById(name_control).value + "'}",
                    dataType: "json",
                    type: "POST",
                    traditional: true,
                    processData: false,
                    contentType: "application/json; charset=utf-8",
                    //dataFilter: function (data) { return data; },
                    success: function (data) {
                        term: extractLast(request.term);
                        response($.ui.autocomplete.filter(
                            data.d[0].country, extractLast(request.term)));
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
                document.getElementById(name_control).value = ui.item.label;
                search_gabinete_general_auto_complete_migracion(document.getElementById(name_control).value);
            }
            , minLength: 4, max: 10, scroll: true
        });
}
//-------Solicita los datos de visualización del documento a migrar
const Service_REST_Lista_documentos_visor_a_migrar = async (id_imagen, gabinete) => {
    let myPromise = new Promise(function (resolve) {
        document.getElementById("IframeVisor_").src = "";
        try {
            $.ajax('../webservice/WebServiceDocuarchi.asmx/Service_Lista_documentos_visor_a_migrar', {
                data: "{" + "'id_imagen':'" + id_imagen + "','" + "gabinete':'" + gabinete +  "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_result !== "YES") {
                        document.getElementById("h_gabibete_imagen").innerText = "";
                        resolve(data.d[0].Error_result);
                    } else {   
                        document.getElementById("IframeVisor_").src = data.d[0].url_iframe + "?nameframe=" + "IframeVisor_";
                        document.getElementById("h_gabibete_imagen").innerText = "DOCUMENTO A MIGRAR : " + id_imagen + " DEL GABINETE : " + gabinete;
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
//-------Gaurda documento digitalizado
const Service_REST_guarda_documento_digitalizado = async (id_imagen, gabinete) => {
    let myPromise = new Promise(function (resolve) {
        document.getElementById("Iframe_visor_pdf").src = "";
        try {
            $.ajax('../webservice/WebServiceMigracion.asmx/Service_guarda_documento_digitalizado', {
                data: "{" + "'id_imagen':'" + id_imagen + "','" + "gabinete':'" + gabinete + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_result !== "YES") {
                        resolve(data.d[0].Error_result);
                    } else {       
                        document.getElementById("Iframe_visor_pdf").src = data.d[0].url_ruta_documento;
                        $("#modal_scan_document_migracion").modal("hide");
                        ID_REGISTRO_MIGRA = data.d[0].id_registro;
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
//-------Gaurda documento digitalizado
const Service_REST_inicializa_intefaz_escaner = async (id_imagen, gabinete) => {
    let myPromise = new Promise(function (resolve) {
        document.getElementById("Iframe_scan_document_migracion").src = "";
        try {
            $.ajax('../webservice/WebServiceMigracion.asmx/Service_inicializa_intefaz_escaner', {
                data: "{" + "'id_imagen':'" + id_imagen + "','" + "gabinete':'" + gabinete + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_result !== "YES") {
                        resolve(data.d[0].Error_result);
                    } else {
                        document.getElementById("HiddenIdFlujo").value = id_imagen + "|"
                        document.getElementById("Iframe_scan_document_migracion").src = data.d[0].url_ruta_documento;
                        $("#modal_scan_document_migracion").modal("show");
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
//-------Solicita los datos de visualización de migración
const Service_REST_Lista_documentos_visor_consulta_migracion = async (id_imagen, gabinete) => {
    let myPromise = new Promise(function (resolve) {
        document.getElementById("Iframe_visor_migracion_documento").src = "";
        try {
            $.ajax('../webservice/WebServiceDocuarchi.asmx/Service_Lista_documentos_visor_a_migrar', {
                data: "{" + "'id_imagen':'" + id_imagen + "','" + "gabinete':'" + gabinete + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_result !== "YES") {
                        document.getElementById("h_title_gabinete_image_migracion").innerText = "";
                        resolve(data.d[0].Error_result);
                    } else {
                        document.getElementById("Iframe_visor_migracion_documento").src = data.d[0].url_iframe;
                        document.getElementById("h_title_gabinete_image_migracion").innerText = "DOCUMENTO  : " + id_imagen + "  GABINETE : " + gabinete;
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
//-------Solicita los datos de visualización del documento migrado
const Service_REST_solicita_estructura_lista_documento_migrado = async (id_imagen, gabinete) => {
    let myPromise = new Promise(function (resolve) {
        document.getElementById("Iframe_visor_pdf").src = "";
        try {
            $.ajax('../webservice/WebServiceMigracion.asmx/Service_Solicita_estructura_lista_documento_migrado', {
                data: "{" + "'id_imagen':'" + id_imagen + "','" + "gabinete':'" + gabinete + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_result !== "YES") {
                        resolve(data.d[0].Error_result);
                    } else {
                        document.getElementById("Iframe_visor_pdf").src = data.d[0].url_ruta_documento;
                        ID_REGISTRO_MIGRA = data.d[0].id_registro_migracion;
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
//-------Migra formato documento 
const Service_REST_migra_formato_documento = async (id_imagen, gabinete, option_visor) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceMigracion.asmx/Service_migra_formato_documento', {
                data: "{" + "'id_imagen':'" + id_imagen + "','" + "gabinete':'" + gabinete + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_result !== "YES") {
                        resolve(data.d[0].Error_result);
                    } else {
                        if (option_visor == 1) {
                            document.getElementById("Iframe_visor_pdf").src = data.d[0].url_ruta_documento;
                        }    
                        ID_REGISTRO_MIGRA = data.d[0].id_registro_migracion;
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
//-------Remplaza versión documento migración
const Service_REST_remplaza_version_documento = async (id_imagen, gabinete, id_registro_migracion) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceVersionDocumento.asmx/Service_remplaza_version_documento', {
                data: "{" + "'id_imagen':'" + id_imagen + "','" + "gabinete':'" + gabinete + "','id_registro_migracion':'" + id_registro_migracion + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_result !== "YES") {
                        resolve(data.d[0].Error_result);
                    } else {
                        updateCelByUniqueId('table_consulta_migracion', 'ESTENSION', id_imagen, data.d[0].Extension_doc_migrado);
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
const Service_REST_Service_Solicita_gestion_autoregistro_gabinete = async (id_auto_registro) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceGestorDocumental.asmx/Service_Solicita_gestion_autoregistro_gabinete', {
                data: "{" + "'id_auto_registro':'" + id_auto_registro +  "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_result !== "YES") {
                        resolve(data.d[0].Error_result);
                    } else {       
                        if (data.d[0].iLIStSerie == null) {          
                            $("#option_cambia_tipo_serie_documental").empty();
                        } else {   
                            var element_drow = document.getElementById('option_cambia_tipo_serie_documental');
                            for (var i = 0; i < data.d[0].iLIStSerie.length; i++) {
                                element_drow[i] = new Option(data.d[0].iLIStSerie[i].text, data.d[0].iLIStSerie[i].value);
                            }
                            element_drow.addEventListener("change", event_change_drowslisi_lista_sub_series);
                        }
                        if (data.d[0].iLIStSubSerie == null) {            
                            $("#option_cambia_tipo_sub_serie_documental").empty();
                        } else {
                            document.getElementById("row_cambia_tipo_sub_serie_documental").classList.remove("d-none");
                            var element_drow = document.getElementById('option_cambia_tipo_sub_serie_documental');
                            for (var i = 0; i < data.d[0].iLIStSubSerie.length; i++) {
                                element_drow[i] = new Option(data.d[0].iLIStSubSerie[i].text, data.d[0].iLIStSubSerie[i].value);
                            }
                            element_drow.addEventListener("change", event_change_drowslisi_lista_tipos_documentales);
                        }
                        if (data.d[0].iLIStTipo == null) {          
                            $("#option_ambia_tipo_documental").empty();
                        } else {        
                            var element_drow = document.getElementById('option_ambia_tipo_documental');
                            for (var i = 0; i < data.d[0].iLIStTipo.length; i++) {
                                element_drow[i] = new Option(data.d[0].iLIStTipo[i].text, data.d[0].iLIStTipo[i].value);
                            }
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
//-------Solicita estructura para clasificación tipologia documental de migracion
const Service_REST_Solicita_lista_series_relacionadas_gabinete_migracion = async (id_imagen, gabinete, id_gabinete) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceMigracion.asmx/Service_Solicita_lista_series_relacionadas_gabinete_migracion', {
                data: "{" + "'id_imagen':'" + id_imagen + "','" + "gabinete':'" + gabinete + "','id_gabinete':'" + id_gabinete + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_result !== "YES") {
                        resolve(data.d[0].Error_result);
                    } else {
                        //------Lista tramites o procesos de auto vinculación
                        if (data.d[0].iLIStTipoTramite == null) {
                            document.getElementById("row_tipo_tramite_migra").classList.add("d-none");
                            $("#option_cambia_tipo_tramite_migra").empty();
                        } else {
                            document.getElementById("row_tipo_tramite_migra").classList.remove("d-none");
                            var element_drow = document.getElementById('option_cambia_tipo_tramite_migra');
                            for (var i = 0; i < data.d[0].iLIStTipoTramite.length; i++) {
                                element_drow[i] = new Option(data.d[0].iLIStTipoTramite[i].text, data.d[0].iLIStTipoTramite[i].value);
                            }
                            element_drow.addEventListener("change", event_change_drowslisi_lista_gestion_auto_registro);
                        }
                        //------Lista series de clasificación
                        if (data.d[0].iLIStSerie == null) {
                            document.getElementById("row_cambia_tipo_serie_documental").classList.add("d-none");
                            $("#option_cambia_tipo_serie_documental").empty();
                        } else {
                            document.getElementById("row_cambia_tipo_serie_documental").classList.remove("d-none");
                            var element_drow = document.getElementById('option_cambia_tipo_serie_documental');
                            for (var i = 0; i < data.d[0].iLIStSerie.length; i++) {
                                element_drow[i] = new Option(data.d[0].iLIStSerie[i].text, data.d[0].iLIStSerie[i].value);
                            }
                            element_drow.addEventListener("change", event_change_drowslisi_lista_sub_series);
                        }
                        //------Lista sub series de clasificación
                        if (data.d[0].iLIStSubSerie == null) {
                            document.getElementById("row_cambia_tipo_sub_serie_documental").classList.add("d-none");
                            $("#option_cambia_tipo_sub_serie_documental").empty();
                        } else {
                            document.getElementById("row_cambia_tipo_sub_serie_documental").classList.remove("d-none");
                            var element_drow = document.getElementById('option_cambia_tipo_sub_serie_documental');
                            for (var i = 0; i < data.d[0].iLIStSubSerie.length; i++) {
                                element_drow[i] = new Option(data.d[0].iLIStSubSerie[i].text, data.d[0].iLIStSubSerie[i].value);
                            }
                            element_drow.addEventListener("change", event_change_drowslisi_lista_tipos_documentales);
                        }
                        //------Lista tipologias de clasificación
                        if (data.d[0].iLIStTipo == null) {
                            $("#option_ambia_tipo_documental").empty();
                        } else {
                            var element_drow = document.getElementById('option_ambia_tipo_documental');
                            for (var i = 0; i < data.d[0].iLIStTipo.length; i++) {
                                element_drow[i] = new Option(data.d[0].iLIStTipo[i].text, data.d[0].iLIStTipo[i].value);
                            }
                        }     
                        $("#modal_cambiar_tipologia_documento").modal("show");
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
//-------Solicita tipos documentales reloacionados a la sub serie
const Service_REST_Solicita_lista_tipos_documentales_relacionados_id_sub_serie = async (id_sub_serie) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceGestorDocumental.asmx/Solicita_lista_tipos_documentales_relacionados_sub_serie', {
                data: "{" + "'id_sub_serie':'" + id_sub_serie + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_result !== "YES") {
                        resolve(data.d[0].Error_result);
                    } else {                
                        if (data.d[0].iLIStTipo == null) {        
                            $("#option_ambia_tipo_documental").empty();
                        } else {
                            $("#option_ambia_tipo_documental").empty();
                            var element_drow = document.getElementById('option_ambia_tipo_documental');
                            for (var i = 0; i < data.d[0].iLIStTipo.length; i++) {
                                element_drow[i] = new Option(data.d[0].iLIStTipo[i].text, data.d[0].iLIStTipo[i].value);
                            }
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
const Service_REST_Solicita_lista_sub_series_documentales_id_serie = async (id_serie) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceGestorDocumental.asmx/Service_Solicita_lista_sub_series_documentales_id_serie', {
                data: "{" + "'id_serie':'" + id_serie + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_result !== "YES") {
                        resolve(data.d[0].Error_result);
                    } else {
                        if (data.d[0].iLIStSubSerie == null) {
                            $("#option_cambia_tipo_sub_serie_documental").empty();
                            $("#option_ambia_tipo_documental").empty();
                        } else {
                            $("#option_cambia_tipo_sub_serie_documental").empty();
                            $("#option_ambia_tipo_documental").empty();
                            var element_drow = document.getElementById('option_cambia_tipo_sub_serie_documental');
                            for (var i = 0; i < data.d[0].iLIStSubSerie.length; i++) {
                                element_drow[i] = new Option(data.d[0].iLIStSubSerie[i].text, data.d[0].iLIStSubSerie[i].value);
                            }
                        }
                        if (data.d[0].iLIStTipo == null) {
                            $("#option_ambia_tipo_documental").empty();
                        } else {
                            $("#option_ambia_tipo_documental").empty();
                            var element_drow = document.getElementById('option_ambia_tipo_documental');
                            for (var i = 0; i < data.d[0].iLIStTipo.length; i++) {
                                element_drow[i] = new Option(data.d[0].iLIStTipo[i].text, data.d[0].iLIStTipo[i].value);
                            }
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
//-------Servicio que actualiza tipo documental
const Service_REST_actualiza_tipologia_migracion = async (parameter) => {
    var serialice = JSON.stringify(parameter);
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceGestorDocumental.asmx/Service_actualiza_tipologia_migracion', {
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
                        updateCelByUniqueId('table_consulta_migracion', 'TIPODOCUMENTO', data.d[0].id_imagen, data.d[0].nombre_tipo_documental);
                        updateCelByUniqueId('table_consulta_migracion', 'NOMBRESERIE', data.d[0].id_imagen, data.d[0].nombre_serie);
                        updateCelByUniqueId('table_consulta_migracion', 'NOMBRESUBSERIE', data.d[0].id_imagen, data.d[0].nombre_sub_serie);
                        $("#modal_cambiar_tipologia_documento").modal("hide");
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
//-------Solicita lista tramites gabinete auto vinculacion
const Service_REST_lista_tramite_auto_vinculacion_gabinete = async (id_gabinete) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebService_tipodocentrante.asmx/Service_solicita_lista_tramite_auto_vinculacion_gabinete', {
                data: "{'id_gabinete':" + "'" + id_gabinete + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].error_sistema !== "YES") {

                        resolve(data.d[0].error_sistema);
                    } else {

                        ITEMS_DATOS_DROW = new Array();
                        $.each(data.d[0].item_sistema, function (k, v) {
                            ITEMS_DATOS_DROW.push(v);
                        });
                        if (document.getElementById('option_tramite_vincula')) {
                            var element_drow = document.getElementById('option_tramite_vincula');
                            $("#option_tramite_vincula").empty();
                            for (var i = 0; i < ITEMS_DATOS_DROW.length; i++) {
                                element_drow[i] = new Option(ITEMS_DATOS_DROW[i].text, ITEMS_DATOS_DROW[i].value);
                            }
                           
                        }
                        $("#modal_tipo_tramite_vinculacion").modal("show");
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
    let result = myPromise;
    return result;
}
//-------Solicita datos registro de migracion
const Service_REST_solicita_estructura_registro_migracion_documento_gestion = async (id_imagen, gabinete) => {
    SEARCH_MIG_ARRAY_REGISTRO_MIGRACION = new Array();
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceMigracion.asmx/Service_solicita_estructura_registro_migracion_documento_gestion', {
                data: "{" + "'id_imagen':'" + id_imagen + "','gabinete':'" + gabinete + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_result !== "YES") {
                        resolve(data.d[0].Error_result);
                    } else {
                        SEARCH_MIG_ARRAY_REGISTRO_MIGRACION = data.d[0];
                        SEARCH_MIG_ID_REG_VERSION_MIGRA_FUENTE = data.d[0].id_registro_version_anterior;
                        SEARCH_MIG_ID_REG_VERSION_MIGRA_DESTINO = data.d[0].id_registro_version_nuevo;
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
//---------TERMINA ZONA WEB SERGICE------------------------------
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

        auto_zise_consulta();
        auto_zise_version_document();

    } catch (ex) {
        alert(ex.message + " Función rezize_event")
    }
}
function auto_zise_consulta() {
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
       
        $('#Contentizquierdo').css("height", ((espacio_iframe - 1) - 1) + "px");
        $('#sidebar_').css("height", ((espacio_iframe - 1) - 1) + "px");
        $("#contenido_controles_consulta").css("height", (document.getElementById("Contentizquierdo").clientHeight) - (document.getElementById('contenido_titulo_controles_consulta').clientHeight  + document.getElementById('contenido_controles_buton_consulta').clientHeight) + "px");
        $("#contenido_consulta_gabinetes_migracion").css("height", (document.getElementById("Contentizquierdo").clientHeight) - (document.getElementById('contenido_titulo_controles_consulta').clientHeight + document.getElementById('contenido_controles_buton_consulta').clientHeight) + "px");
        $('#Contenedorderecho').css("height", ((espacio_iframe - 1) - 1) + "px");
        $('#contenido_table_boot_migracion').css("height", (document.getElementById("Contenedorderecho").clientHeight - (document.getElementById("contenido_icon_boton_migra").clientHeight + 5 + document.getElementById("contenido_footer_migracion").clientHeight)) + "px");
        let heig_table = document.getElementById('contenido_table_boot_migracion').clientHeight -5;
        table_reize_heigth("table_consulta_migracion", heig_table, "", "table-borderless");
       
        
    } catch (ex) { alert("Funcion auto_zise_consulta " + ex.message); }

}

