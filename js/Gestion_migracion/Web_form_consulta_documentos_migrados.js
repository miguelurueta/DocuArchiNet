
$(document).ready(function () {
    $.fn.inicio = function () {
        auto_zise_consulta();
    }
});
let SEARCH_MIG_ID_REG_VERSION_MIGRA_FUENTE = 0; //Registra la idenitifcacion de version del documento fuente de migrado
let SEARCH_MIG_ID_REG_VERSION_MIGRA_DESTINO = 0; //Registra la idenitifcacion de version del documento destino migrado
let SEARCH_MIG_ARRAY_REGISTRO_MIGRACION = new Array(); //Registra la idenitifcacion de version del documento destino migrado
//-----------------------ZONA LOAD-------------------------------------------
$(window).on("load", function () {
    try {
        var elment = document.getElementsByClassName("da_event_captive");
        if (elment) {
            for (var i = 0; i < elment.length; i++) {
                elment[i].addEventListener("click", event_click, false);
            }
        }
        load_interface_consulta();
        Service_REST_auto_complete_registro_migracion("textBox_buequeda_general_migra", "DA", "", "");
        window.addEventListener("resize", rezize_event);
        ShowModalPopup("ModalPopupExtender_mensaje_personalizado_backgroundElement", "Panel_mensaje_personalizado", 100009);

    } catch (e) {
        alert(" funcion load " + e.message);
    }

});
const load_interface_consulta = async () => {
    ini_event_page();
    let parameter_Service = new Array();
    parameter_Service.push({
        id_registro: 1, class_service: "WebServiceMigracion.asmx", name_service: "Service_lista_interface_busqueda_gabinete",
        name_container: "div_consulta_gabinetes_migracion", name_control_padre: "",
        asigna_valor: 0, apost_name_content: "consulta_gabinetes_migracion", add_check: 0, name_table: "ra_mig_registro_migracion", class_name_control: "consulta_gabinetes_migracion"
    });
    let result = await Service_REST_lista_interface_busqueda_documentos_migrados(parameter_Service);
    if (result !== "YES") {
        alert_bot(result, 'warning', "error_content_migracion");
    }
    result = await Service_REST_estructura_campos_dynamic_registro_migracion(0, "table_consulta_migracion", "contenido_table_boot_migracion");
    if (result !== "YES") {
        alert_bot(result, 'warning', "error_content_migracion");
        progres_hiden('progres_bar');
        return true
    }
}
//----------------------TERMNA ZONA LOAD-------------------------------------
//----------------------ZONA EVENTOS-----------------------------------------
const ini_event_page = () => {
    let array_element = new Array;
    array_element.push({ id: "Button_search_registro_migracion" }, { id: "Button_update_reload_docuent_fuente" }, { id: "Button_update_reload_docuent_destino" }
        , { id: "Button_dtalle_docuent_fuente" }, { id: "Button_dtalle_docuent_migrado" }, { id: "Button_search_registro_migracion_lik" }, { id:"Button_restore_consulta"}
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
            //Activa el restore search de la tabla de consulta
            case "Button_restore_consulta":
                event_element_click_promise(e);
                break;
            //Activa consulta like registro migracion
            case "Button_search_registro_migracion_lik":
                let vaLueText = document.getElementById("textBox_buequeda_general_migra").value;
                search_auto_complete_registro_migracion(vaLueText);
                break;
            //Activa consulta registro migracion
            case "Button_search_registro_migracion":
                result = search_form_control_gabinete("consulta_gabinetes_migracion");
                if (result !== "YES") {
                    alert_bot(result, 'warning', "contenido_controles_consulta");
                } else {
                    event_element_click_promise(e);
                }
                break;
            //Activa refresf visor imgen fuente de migracion
            case "Button_update_reload_docuent_fuente":
                if (SEARCH_MIG_ID_REG_VERSION_MIGRA_FUENTE == 0) { return true; }
                event_element_click_promise(e);
                break;
            //Activa refresf visor imgen migrada
            case "Button_update_reload_docuent_destino":
                if (SEARCH_MIG_ID_REG_VERSION_MIGRA_DESTINO == 0) { return true; }
                event_element_click_promise(e);
                break;
            //Activa detalle version documento fuente
            case "Button_dtalle_docuent_fuente": {
                if (SEARCH_MIG_ID_REG_VERSION_MIGRA_FUENTE == 0) { return true; }
                event_element_click_promise(e);
                break;
            }
            //Activa detalle version documento migrado
            case "Button_dtalle_docuent_migrado": {
                if (SEARCH_MIG_ID_REG_VERSION_MIGRA_DESTINO == 0) { return true; }
                event_element_click_promise(e);
                break;
            }
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
const event_element_click_promise = async (e) => {
    try {
        let result = "";
        let name_control = e.currentTarget.id;
        delete_alert_boot();
        //Consulta gabinete migración
        if (name_control == "Button_restore_consulta") {
            e.currentTarget.disabled = true;
            posicion_update_pogres('progres_bar');
            restore_value_form_control("consulta_gabinetes_migracion");
            document.getElementById(name_control).disabled = false;
        }
        if (name_control == "Button_search_registro_migracion") {
            e.currentTarget.disabled = true;
            posicion_update_pogres('progres_bar');
            result = await Service_REST_consulta_documentos_migrados(ITEM_GENERAL_CONTROL_ARRAY, 1, "", "table_consulta_migracion", "contenido_table_boot_migracion");
            if (result !== "YES") {
                progres_hiden('progres_bar');
                document.getElementById(name_control).disabled = false;
                alert_bot(result, 'warning', "contenido_controles_consulta");

            } else {
                progres_hiden('progres_bar');
                document.getElementById(name_control).disabled = false;
            }
        }
        if (name_control == "Button_update_reload_docuent_fuente") {
            //----Solicita documento fuente de migrado
            result = await Service_REST_solicita_documentos_version(SEARCH_MIG_ID_REG_VERSION_MIGRA_FUENTE, "IframeVisor_");
            if (result !== "YES") {
                progres_hiden('progres_bar');
                alert_bot(result, 'warning', "error_content_popup_migracion")
            }
        }      
        if (name_control == "Button_update_reload_docuent_destino") {
            //----Solicita documento  migrado
            result = await Service_REST_solicita_documentos_version(SEARCH_MIG_ID_REG_VERSION_MIGRA_DESTINO, "Iframe_visor_pdf");
            if (result !== "YES") {
                progres_hiden('progres_bar');
                alert_bot(result, 'warning', "error_content_popup_migracion")
            }
        }
        if (name_control == "Button_dtalle_docuent_fuente") {
            //----Solicita documento  migrado
            result = await Service_REST_detalle_version_documento_coonsulta_migracion(SEARCH_MIG_ID_REG_VERSION_MIGRA_FUENTE);
            if (result !== "YES") {
                progres_hiden('progres_bar');
                alert_bot(result, 'warning', "error_content_popup_migracion")
            }
        }
        if (name_control == "Button_dtalle_docuent_migrado") {
            //----Solicita documento  migrado
            result = await Service_REST_detalle_version_documento_coonsulta_migracion(SEARCH_MIG_ID_REG_VERSION_MIGRA_DESTINO);
            if (result !== "YES") {
                progres_hiden('progres_bar');
                alert_bot(result, 'warning', "error_content_popup_migracion")
            }
        }
        
        progres_hiden('progres_bar');
    }
    catch (ex) {
        progres_hiden('progres_bar');
        alert_bot(ex.message, 'warning', "error_content_migracion");

    } finally {

    }
}
//Lista los documentos relacionados al registro de migración
const Show_documentos_migracion = async (id_registro_migracion) => {
    try {
        let result = "";
        posicion_update_pogres('progres_bar');
        //----Solicita la estructura de datos de registro 
        result = await Service_REST_solicita_estructura_registro_migracion_documento(id_registro_migracion);
        if (result !== "YES") {
            progres_hiden('progres_bar');
            alert_bot(result, 'warning', "error_content_migracion")
            return true;
        }
        //----Solicita documento fuente de migrado
        if (SEARCH_MIG_ID_REG_VERSION_MIGRA_FUENTE != 0) {
            result = await Service_REST_solicita_documentos_version(SEARCH_MIG_ID_REG_VERSION_MIGRA_FUENTE, "IframeVisor_");
            if (result !== "YES") {
                progres_hiden('progres_bar');
                alert_bot(result, 'warning', "error_content_migracion")
                return true;
            }
        } else {
            document.getElementById("IframeVisor_").src = "" ;
        }
       
       
        //----Solicita documento  migrado
        if (SEARCH_MIG_ID_REG_VERSION_MIGRA_DESTINO != 0) {
            result = await Service_REST_solicita_documentos_version(SEARCH_MIG_ID_REG_VERSION_MIGRA_DESTINO, "Iframe_visor_pdf");
            if (result !== "YES") {
                progres_hiden('progres_bar');
                alert_bot(result, 'warning', "error_content_migracion")
                return true;
            }
        } else {
            document.getElementById("Iframe_visor_pdf").src = "";
        }
        $("#modal_migracion").modal("show");
        progres_hiden('progres_bar');
        }
       catch (ex) {
            progres_hiden('progres_bar');
            alert_bot(ex.message, 'warning', "error_content_migracion");
        }
}
const Show_detail_registro_migracion = async (id_registro_migracion) => {
    try {
        let result = "";
        posicion_update_pogres('progres_bar');
        result = await Service_REST_solicita_estructura_registro_migracion_documento(id_registro_migracion)
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
        progres_hiden('progres_bar');

    }
     catch (ex) {
    progres_hiden('progres_bar');
    alert_bot(ex.message, 'warning', "error_content_migracion");
      }
}
const Delete_version_document_migrado = async (id_registro_migracion) => {
    try {
        let confirM = confirm("Desea eliminar el documento fuente de migración");
        if (confirM == false) { return true; }
        posicion_update_pogres('progres_bar');
        let result = "";
        result = await Service_REST_elimina_version_documento_migrado(id_registro_migracion);
        if (result !== "YES") {
            progres_hiden('progres_bar');
            alert_bot(result, 'warning', "error_content_migracion")
        }
        progres_hiden('progres_bar');
    }
    catch (ex) {
        progres_hiden('progres_bar');
        alert_bot(ex.message, 'warning', "error_content_migracion");
    }
}
const search_auto_complete_registro_migracion = async (value_search) => {
    try {
        posicion_update_pogres('progres_bar');
        let result = await Service_REST_consulta_documentos_migrados(ITEM_GENERAL_CONTROL_ARRAY, 2, value_search, "table_consulta_migracion", "contenido_table_boot_migracion");
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
const Show_index_imagen_migracion = async (gabinete, id_imagen_index) => {
    try {
        posicion_update_pogres('progres_bar');
        let parameter_Service = new Array();
        parameter_Service.push({
            id_registro: id_imagen_index, class_service: "WebServiceDocuarchi.asmx", name_service: "Service_crea_interface_indice_migracion",
            name_container: "div_actualiza_indice_batch_mig", name_control_padre: "modal_actualiza_indice_batch_mig",
            asigna_valor: 1, apost_name_content: "actualiza_indice_batch_mig", add_check: 0, name_table: gabinete, class_name_control: "form_control_indice_docuarchi"
        });
        let result = await Service_REST_interface_form_clontrol_bootStrap(parameter_Service);
        if (result == "YES") {
            progres_hiden('progres_bar');
        } else {
            progres_hiden('progres_bar');
            alert_bot(result, 'warning', "error_content_migracion");
        }
        progres_hiden('progres_bar');
    } catch (ex) {
        progres_hiden('progres_bar');
        alert_bot(ex.message, 'warning', "error_content_migracion");
    }
}
//-------------------ZONA EVENTOS TABLE BOOT---------------------------
function operateFormattertablebootmig(value, row, index) {

    return [
        '<div class="row pl-2">',
        '<div class="col-8 p-0">',
        '<a class="active_view_version  nav-link pl-5 justify-content-end font-weight-light" style="color: black" href="javascript:void(0)" title="Lista documentos relacionados con el registro de migración">  <i style="color: #black" class="far fa-folder-open"></i>  </a>',
        '</div > ',
        '<div class="col-4 p-0">',
        '<a class="nav-link  dropdown-toggle justify-content-start" style="color: black" href="#" id="A5" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: black; display:none" class="fad fa-th-list"></i>  ',
        '</a>',
        '<div class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">',
        '<a class="active_view_detail_row dropdown-item font-weight-light" href="javascript:void(0)" title="Lista datos de registro de migración">  <i style="color: #black" class="fal fa-list"></i> Registro de migracion </a>',
        '<a class="active_view_iddex_imagen btn dropdown-item font-weight-light" href="javascript:void(0)" title="Lista datos de indexación del documento migrado">  <i style="color: #black" class="fal fa-info-square"></i> Datos de indexación </a>',
        '<a class="active_elimina_version dropdown-item font-weight-light" href="javascript:void(0)" title="Elimina versión del documento fuente de migración">  <i style="color: #black" class="fal fa-file-times"></i> Elimina documento fuente de migracion </a>',
        '<a style="color: black" href="#" class="dropdown-item font-weight-light"><i class="far fa-sign-out"></i> Salir del menu</a>',
        '</div>',
        '</a>',
        '</div>',
        '</div>',

           
    ].join('')
}
window.operateEvents = {
    'click .active_view_version': (e, value, row, index) => {
        delete_alert_boot();
        let ident = table_boot_return_objet_jonson(row);
        Show_documentos_migracion(ident.id_registro_migracion);

    }, 'click .active_view_detail_row': (e, value, row, index) => {
        delete_alert_boot();
        let ident = table_boot_return_objet_jonson(row);
        Show_detail_registro_migracion(ident.id_registro_migracion);
        

    }, 'click .active_elimina_version': (e, value, row, index) => {
        delete_alert_boot();
        let ident = table_boot_return_objet_jonson(row);
        Delete_version_document_migrado(ident.id_registro_migracion);
       
    }, 'click .active_view_iddex_imagen': (e, value, row, index) => {
        delete_alert_boot();
        let ident = table_boot_return_objet_jonson(row);
        let gabinete = ident.nombre_gabinete;
        let id_imagen = ident.id_imagen;
        Show_index_imagen_migracion(gabinete, id_imagen);

    }
}
//-------------TERMINA ZONA EVENTOS TABLE BOOT--------------------
//-------Solicita estructura campo tabla BOOT head
const Service_REST_estructura_campos_dynamic_registro_migracion = async (parameter, name_table, name_parent_table) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceMigracion.asmx/Service_estructura_campos_dynamic_registro_migracion', {
                data: "{'parameter':" + "'" + parameter + "'}",
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
//-------Realiza la consulta de gabinetes de migracion y retorna los resultados
const Service_REST_consulta_documentos_migrados = async (control_array_config_service, tipo_consulta, valor_consulta, name_table, name_parent_table) => {
    let serialice = JSON.stringify(control_array_config_service);
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceMigracion.asmx/Service_consulta_documentos_migrados', {
                data: "{" + "'parameter':'" + serialice + "','" + "tipo_consulta':'" + tipo_consulta + "','" + "valor_consulta':'" + valor_consulta + "'}",
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
                            document.getElementById("state_migracion").innerText = class_stru_row_Gabinete_Generic.length + " Registro (s) de Migrados  " ;
                        }
                        init_row_feld_table_boostrap_table(name_table, data.d[0].Obj_ilist_fileds_generic, class_stru_row_Gabinete_Generic, name_parent_table, "", "table-borderless");
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
//-------Solicita datos registro de migracion
const Service_REST_solicita_estructura_registro_migracion_documento = async (id_registro_migracion) => {
    SEARCH_MIG_ARRAY_REGISTRO_MIGRACION = new Array();
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceMigracion.asmx/Service_solicita_estructura_registro_migracion_documento', {
                data: "{" + "'id_registro_migracion':'" + id_registro_migracion  + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_result !== "YES") {
                        resolve(data.d[0].Error_result);
                    } else {
                        SEARCH_MIG_ARRAY_REGISTRO_MIGRACION  = data.d[0];
                        SEARCH_MIG_ID_REG_VERSION_MIGRA_FUENTE= data.d[0].id_registro_version_anterior;
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
//-------Solicita los datos de visualización del documento version
const Service_REST_solicita_documentos_version = async (id_registro_version, name_frame) => {
    let myPromise = new Promise(function (resolve) {
        document.getElementById(name_frame).src = "";
        try {
            $.ajax('../webservice/WebServiceVersionDocumento.asmx/Service_solicita_documentos_version', {
                data: "{" + "'id_registro_version':'" + id_registro_version + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_result !== "YES") {
                        resolve(data.d[0].Error_result);
                    } else {
                        document.getElementById(name_frame).src = data.d[0].url_iframe;
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
//-------Detalle version de documento
const Service_REST_detalle_version_documento_coonsulta_migracion = async (id_registro_version) => {
    let myPromise = new Promise(function (resolve) {

        try {
            $.ajax('../webservice/WebServiceVersionDocumento.asmx/Service_solicita_detalle_version_documento', {
                data: "{" + "'id_registro_version':'" + id_registro_version + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_result !== "YES") {
                        resolve(data.d[0].Error_result);
                    } else {
                        document.getElementById("spn_id_registro_version").innerText = data.d[0].id_registro_version;
                        document.getElementById("spn_id_version_doc").innerText = data.d[0].id_version_doc;
                        document.getElementById("spn_fecha_registro_version").innerText = data.d[0].fecha_registro_version;
                        document.getElementById("spn_tipo_archivo").innerText = data.d[0].TIPO_ARCHIVO;
                        document.getElementById("spn_peso_documento").innerText = data.d[0].PESO_DOCUMENTO;
                        document.getElementById("spn_paginas_document").innerText = data.d[0].PAGINAS_DOCUMENT;
                        $("#modal_detalle_version_documento").modal("show");
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
//-------Elimina version archivo  fuente de  migración
const Service_REST_elimina_version_documento_migrado = async (id_registro_migracion) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceMigracion.asmx/Service_eliminar_documento_migrado', {
                data: "{" + "'id_registro_migracion':'" + id_registro_migracion + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_result !== "YES") {
                        resolve(data.d[0].Error_result);
                    } else {
                        updateCelByUniqueId("table_consulta_migracion", "id_registro_version_anterior", id_registro_migracion,"0")
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
const Service_REST_auto_complete_registro_migracion = async (name_control, name_dbs_auto, name_table_auto, name_campo_auto) => {
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
                    url: "../webservice/WebServiceMigracion.asmx/Service_auto_complete_registro_migracion",
                    data: "{'parameter':'" + serialice + "','value':'" + document.getElementById(name_control).value + "'}",
                    dataType: "json",
                    type: "POST",
                    traditional: true,
                    processData: false,
                    contentType: "application/json; charset=utf-8", 
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
                search_auto_complete_registro_migracion(document.getElementById(name_control).value);

            }

            , minLength: 4, max: 10, scroll: true
        });
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

        auto_zise_consulta();
        //auto_zise_version_document();

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
        $("#contenido_controles_consulta").css("height", (document.getElementById("Contentizquierdo").clientHeight) - (document.getElementById('contenido_titulo_controles_consulta').clientHeight  + document.getElementById('contenido_controles_buton_consulta').clientHeight) + "px");
        $("#contenido_consulta_gabinetes_migracion").css("height", (document.getElementById("Contentizquierdo").clientHeight) - (document.getElementById('contenido_titulo_controles_consulta').clientHeight + document.getElementById('contenido_controles_buton_consulta').clientHeight) + "px");
        $('#Contenedorderecho').css("height", ((espacio_iframe - 1) - 1) + "px");
        $('#contenido_table_boot_migracion').css("height", (document.getElementById("Contenedorderecho").clientHeight - (document.getElementById("contenido_icon_boton_migra").clientHeight + 5 + document.getElementById("contenido_footer_migracion").clientHeight)) + "px");
        let heig_table = document.getElementById('contenido_table_boot_migracion').clientHeight - 5;
        table_reize_heigth("table_consulta_migracion", heig_table, "", "table-borderless");
        
    } catch (ex) { alert("Funcion auto_zise_consulta " + ex.message); }

}