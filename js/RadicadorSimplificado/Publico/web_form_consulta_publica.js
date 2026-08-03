
$(document).ready(function () {
    $.fn.inicio = function () {
        
    }
});
let CONS_PUB_ID_REGISTRO_USUARIO = 0;
let CONS_PUB_ID_REGISTRO_PUBLICO = 0;
let CONS_PUB_MATRICULA_REGISTRO_PUBLICO = "";
let CONS_PUB_URL_SESION = "";
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
        load_page_consulta_public();
        let INTERVAL_SESION_ITEM_MANTENT_PUBLIC = setInterval('Service_REST_validate_sesion_consulta_public("");', '6030');
    } catch (e) {
        alert(" funcion load " + e.message);
    }

});
const ini_event_page = () => {
    let array_element = new Array;
    array_element.push({ id: "Button_div_datos_ingreso" }, { id: "Button_search_matriculado" }, { id: "Button_back" }, { id: "Button_back_docu_actos" }
        , { id: "Button_back_lista_documentos" }, { id: "Button_back_lista_documentos_matriculados" }, { id: "Button_back_docu_matriculado" },
        { id: "Button_sesion" }, { id: "help_sear_documente"}    );
    for (let i = 0; i < array_element.length; i++) {
        let elment_a_document_production = document.getElementById(array_element[i].id);
        if (elment_a_document_production) {
            elment_a_document_production.addEventListener("click", handler_element_event, false);
        }
    }
}
const load_page_consulta_public = async () => {
    try {
        posicion_update_pogres('progres_bar');
        let result = "";
        let value_empresa = "";
        value_empresa = await load_url_page_consulta_publica();
        if (value_empresa == "") {
            alert_bot("Empres de inicialización no detectada", 'warning', "error_div_container_general");
            return true;
        }
        result = await Service_REST_inicializa_conexion_consulta_publica(value_empresa);
        if (result !== "YES") {
            alert_bot(result, 'warning', "error_div_container_general");
            return true;
        }
        result = await load_interface_consulta_publica_expediente();
        if (result !== "YES") {
            alert_bot(result, 'warning', "error_div_container_general");
            return true;
        }
        document.getElementById("home_datos_ingreso").classList.add("active");
        show_popup_general();
    } catch (ex) {  
        alert_bot(ex.message, 'warning', "error_div_container_general");
    } finally {
        progres_hiden('progres_bar');
    }
}
const load_url_page_consulta_publica = async () => {
    let myPromise = new Promise(function (resolve) {
        const querystring = window.location.search;
        CONS_PUB_URL_SESION = window.location.href;
        if (querystring !== "") {
            let params = new URLSearchParams(querystring)
            let empresaOuner = params.get('empresa');
            if (empresaOuner !== null) {
                resolve(empresaOuner);
            } else {
                resolve("");
            }
        } else {
            resolve("");
        }
    })
    let result = await myPromise;
    return result;
}
const handler_element_event = (e) => {
    try {
        let name_ID = e.currentTarget.id;
        let result = "";
        let name_espace_class
        delete_alert_boot();
        switch (name_ID) {
            case "help_sear_documente":
                show_help_search_public();
                break;
            case "Button_sesion":
                Service_REST_sesion_end("");
                break;
            //Activa el registro de usurio de consulta publica
            case "Button_div_datos_ingreso":
                event_element_click_promise(e);
                break;
            //Activa la consulta del matricualdo
            case "Button_search_matriculado" :
                 event_element_click_promise(e);
                break;
            case "Button_back":
                document.getElementById("consulta_actos_registro_matriculado").classList.remove("active");
                document.getElementById("consulta_expedientes_matricualados").classList.add("active");
                break;

            case "Button_back_docu_actos":
                document.getElementById("documentos_actos_registro_matriculado").classList.remove("active");
                document.getElementById("consulta_actos_registro_matriculado").classList.add("active");
                break;
            case "Button_back_lista_documentos":
                document.getElementById("visor_consulta_publica").classList.remove("active");
                document.getElementById("documentos_actos_registro_matriculado").classList.add("active");
                break;
            case "Button_back_lista_documentos_matriculados":
                document.getElementById("visor_consulta_publica_matricuado").classList.remove("active");
                document.getElementById("documentos_matriculado").classList.add("active");
                break;
            case "Button_back_docu_matriculado" :
                document.getElementById("documentos_matriculado").classList.remove("active");
                document.getElementById("consulta_expedientes_matricualados").classList.add("active");
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
        //Registra el registro del usuario de consulta de expediente
        if (name_control == "Button_div_datos_ingreso") {
            e.currentTarget.disabled = true;
            posicion_update_pogres('progres_bar');
            result = await valida_solicita_datos_control_general_async("datos_ingreso");
            if (result != "YES") {
                alert_bot(result, 'warning', "error_div_datos_ingreso");
                return true;
            }
            result = await Service_REST_registra_usuario_consulta_publica(ITEM_GENERAL_CONTROL_ARRAY);
            if (result != "YES") {
                alert_bot(result, 'warning', "error_div_datos_ingreso");
                return true;
            }
            result = await Service_REST_solicita_lista_tipo_consulta_publica();
            if (result != "YES") {
                alert_bot(result, 'warning', "error_div_datos_ingreso");
                return true;
            }
            document.getElementById("home_datos_ingreso").classList.remove("active");
            document.getElementById("consulta_expedientes_matricualados").classList.add("active");
        }
        //Consulta matriculado gabinete
        if (name_control == "Button_search_matriculado") {
            e.currentTarget.disabled = true;
            posicion_update_pogres('progres_bar');
            let ElmentDrow = document.getElementById("option_registro_expediente");
            let value_e = ElmentDrow.options[ElmentDrow.selectedIndex].value;
            let text_e = ElmentDrow.options[ElmentDrow.selectedIndex].label;
            if (text_e == "Seleccione el registro de consulta pública" || text_e == "") {
                alert_bot("Debe selecionar el tipo de registro a consultar", 'warning', "error_div_consulta_matriculado");
                return true;
            }
            ITEM_GENERAL_CONTROL_ARRAY = new Array();
            let text_search = document.getElementById("textBox_buequeda_matricualdo_gabinete").value;
            if (text_search == "") {
                alert_bot("Informe el criterio de busqueda", 'warning', "error_div_consulta_matriculado");
                return true;
            }
            result = await Service_REST_consulta_publica_matriculado_gabinete(ITEM_GENERAL_CONTROL_ARRAY, 2, text_search, "table_consulta_matriculado", value_e, "");
            if (result !== "YES") {
                alert_bot(result, 'warning', "error_div_consulta_matriculado");
                return true;
            }
            CONS_PUB_ID_REGISTRO_PUBLICO = value_e;
        }
    }
    catch (ex) {
        alert_bot(ex.message, 'warning', "error_div_container_general");
    } finally {
        progres_hiden('progres_bar');
        document.getElementById(name_control).disabled = false;
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
//------inicializa interface consulta publica expediente
const load_interface_consulta_publica_expediente = async () => {
    let myPromise = new Promise(function (resolve) {
    try {
       
        let parameter_Service = new Array();
        let result;
        parameter_Service.push({
            id_registro: 0, class_service: "WebServiceConsultaPublica.asmx", name_service: "Service_solicita_datos_interface_registro_consulta_publica",
            name_container: "div_datos_ingreso", name_control_padre: "",
            asigna_valor: 0, apost_name_content: "datos_ingreso", add_check: 0, name_table: "ra_con_usuario_consulta_publica", class_name_control: "datos_ingreso"
        });
        result =  Service_REST_interface_form_clontrol_bootStrap(parameter_Service);
        resolve(result);
    }
    catch (ex) {
        resolve(ex.message);
        }
    })
    let result = await myPromise;
    return result;
}
const show_popup_general = () => {
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl)
    })
}
let windowObjectReference;
const show_help_search_public = () => {
    windowObjectReference = window.open(
        "../help/help_searh_public_document.pdf",
        "Ayuda consulta publica",
        "resizable,scrollbars,status",
    );
}
const show_lista_actos_expediente = async (id_registro_publico, matricula, row) => {
    try {
        let result = "";
        delete_alert_boot();
        posicion_update_pogres('progres_bar');
        ITEM_GENERAL_CONTROL_ARRAY = new Array();
        result = await Service_REST_solicita_lista_actos_expediente(ITEM_GENERAL_CONTROL_ARRAY, 3, matricula, "table_consulta_actos_registro_matriculado", id_registro_publico, "");
        if (result !== "YES") {
            alert_bot(result, 'warning', "error_div_consulta_matriculado");
            return true;
        }
        let iconT = 1;
        for (const [key, value] of Object.entries(row)) {
            let idntifique = "r_" + iconT + "_label";
            if (document.getElementById(idntifique) !== null) {
                document.getElementById(idntifique).textContent = key;
            }
            idntifique = "r_" + iconT + "_value";
            if (document.getElementById(idntifique) !== null) {
                document.getElementById(idntifique).textContent = value;
            }
            iconT++;
        }
        document.getElementById("consulta_expedientes_matricualados").classList.remove("active");
        document.getElementById("consulta_actos_registro_matriculado").classList.add("active");
        
    }
    catch (ex) {
        alert_bot(ex.message, 'warning', "error_div_consulta_matriculado");
        } finally {
            progres_hiden('progres_bar');
            
        }
}
const show_lista_documentos_relacionados_actos = async (id_registro_publico, row_jonson, row) => {
    try {
        let result = "";
        delete_alert_boot();
        posicion_update_pogres('progres_bar');
        ITEM_GENERAL_CONTROL_ARRAY = new Array();
        let ident = table_boot_return_objet_jonson(row);  
        ITEM_GENERAL_CONTROL_ARRAY.push({ libro: ident.LIBRO, inscripcion: ident.INSCRIPCION, enlace: ident.ENLASE})
        result = await Service_REST_lista_documentos_relacionados_acto(ITEM_GENERAL_CONTROL_ARRAY, 3, "", "table_documentos_actos_registro_matriculado", id_registro_publico, "");
        if (result !== "YES") {
            alert_bot(result, 'warning', "error_consulta_actos_registro_matriculado");
            return true;
        }
        let iconT = 1;
        for (const [key, value] of Object.entries(row)) {
            let idntifique = "a_" + iconT + "_label";
            if (document.getElementById(idntifique) !== null) {
                document.getElementById(idntifique).textContent = key;
            }
            idntifique = "a_" + iconT + "_value";
            if (document.getElementById(idntifique) !== null) {
                document.getElementById(idntifique).textContent = value;
            }
            iconT++;
        }
        document.getElementById("consulta_actos_registro_matriculado").classList.remove("active");
        document.getElementById("documentos_actos_registro_matriculado").classList.add("active");

    }
    catch (ex) {
        alert_bot(ex.message, 'warning', "error_consulta_actos_registro_matriculado");
    } finally {
        progres_hiden('progres_bar');

    }
}
const Show_visor_documento_consulta_publica_acto = async (row) => {
    try {
        let result = "";
        delete_alert_boot();
        posicion_update_pogres('progres_bar');
        let ident = table_boot_return_objet_jonson(row);
        let Title_visor = "*DCUMENTO : " + ident.ID + "  *MATRICULA : " + CONS_PUB_MATRICULA_REGISTRO_PUBLICO;
        for (const [key, value] of Object.entries(row)) {
            let value_ = "";
            if (value == null || value == "") {
                value_ = "Na";
            } else {
                value_ = value;
            }       
            Title_visor = Title_visor + "  *" + key.toUpperCase() + " : " + value_;
        }
        ITEM_GENERAL_CONTROL_ARRAY = new Array();
        //CONS_PUB_ID_REGISTRO_USUARIO = 1;
        ITEM_GENERAL_CONTROL_ARRAY.push({
            id_imagen: ident.ID, id_registro_publico: CONS_PUB_ID_REGISTRO_PUBLICO, id_usuario_registro_publico: CONS_PUB_ID_REGISTRO_USUARIO,
            matricula: CONS_PUB_MATRICULA_REGISTRO_PUBLICO})
        result = await Service_REST_Lista_documento_consulta_publica_expediente(ITEM_GENERAL_CONTROL_ARRAY, Title_visor);
        if (result !== "YES") {
            alert_bot(result, 'warning', "error_documentos_actos_registro_matriculado");
            return true;
        }
        document.getElementById("documentos_actos_registro_matriculado").classList.remove("active");
        document.getElementById("visor_consulta_publica").classList.add("active");
    }
    catch (ex) {
        alert_bot(ex.message, 'warning', "error_documentos_actos_registro_matriculado");
    } finally {
        progres_hiden('progres_bar');

    }
}
const show_lista_documentos_matriculado = async (id_registro_publico, row_jonson, row) => {
    try {
        let result = "";
        delete_alert_boot();
        posicion_update_pogres('progres_bar');
        ITEM_GENERAL_CONTROL_ARRAY = new Array();
        let ident = table_boot_return_objet_jonson(row);
        result = await Service_REST_ista_documentos_relacionados_matriculado(ITEM_GENERAL_CONTROL_ARRAY, 3, ident.MATRICULA, "table_documentos_matriculado_registro", id_registro_publico, "");
        if (result !== "YES") {
            alert_bot(result, 'warning', "error_div_consulta_matriculado");
            return true;
        }
        let iconT = 1;
        for (const [key, value] of Object.entries(row)) {
            let idntifique = "b_" + iconT + "_label";
            if (document.getElementById(idntifique) !== null) {
                document.getElementById(idntifique).textContent = key;
            }
            idntifique = "b_" + iconT + "_value";
            if (document.getElementById(idntifique) !== null) {
                document.getElementById(idntifique).textContent = value;
            }
            iconT++;
        }
        document.getElementById("consulta_expedientes_matricualados").classList.remove("active");
        document.getElementById("documentos_matriculado").classList.add("active");

    }
    catch (ex) {
        alert_bot(ex.message, 'warning', "error_div_consulta_matriculado");
    } finally {
        progres_hiden('progres_bar');

    }
}
const Show_visor_documento_consulta_publica_matriculado = async (row) => {
    try {
        let result = "";
        delete_alert_boot();
        posicion_update_pogres('progres_bar');
        let ident = table_boot_return_objet_jonson(row);
        let Title_visor = "*DCUMENTO : " + ident.ID + "  *MATRICULA : " + CONS_PUB_MATRICULA_REGISTRO_PUBLICO;
        for (const [key, value] of Object.entries(row)) {
            let value_ = "";
            if (value == null || value == "") {
                value_ = "Na";
            } else {
                value_ = value;
            }
            Title_visor = Title_visor + "  *" + key.toUpperCase() + " : " + value_;
        }
        ITEM_GENERAL_CONTROL_ARRAY = new Array();
        //CONS_PUB_ID_REGISTRO_USUARIO = 1;
        ITEM_GENERAL_CONTROL_ARRAY.push({
            id_imagen: ident.ID, id_registro_publico: CONS_PUB_ID_REGISTRO_PUBLICO, id_usuario_registro_publico: CONS_PUB_ID_REGISTRO_USUARIO,
            matricula: CONS_PUB_MATRICULA_REGISTRO_PUBLICO
        })
        result = await Service_REST_Lista_documento_consulta_publica_matriculado(ITEM_GENERAL_CONTROL_ARRAY, Title_visor);
        if (result !== "YES") {
            alert_bot(result, 'warning', "error_documentos_matriculado");
            return true;
        }
        document.getElementById("documentos_matriculado").classList.remove("active");
        document.getElementById("visor_consulta_publica_matricuado").classList.add("active");
    }
    catch (ex) {
        alert_bot(ex.message, 'warning', "error_documentos_matriculado");
    } finally {
        progres_hiden('progres_bar');

    }
}
//-------------------ZONA EVENTOS TABLE BOOT---------------------------
function operateFormattertablebootmig(value, row, index) {
    return [
        '<a class="nav-item dropdown active w-100">',
            '<a class="nav-link  dropdown-toggle " style="color: black" href="#" id="A5" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: black; display:none" class="fad fa-th-list"></i> DETALLE ',
            '</a>',
            '<div class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">',
                '<a style="color: black" href="#" class="dropdown-item font-weight-light active_show_actos" ><i class="fad fa-bars"></i> Lista actos del matriculado </a>',
                '<a style="color: black" href="#" class="dropdown-item font-weight-light active_show_documentos" "><i class="fad fa-file-alt"></i> Lista documentos del matriculado</a>',
                '<a style="color: black" href="#" class="dropdown-item font-weight-light"><i class="far fa-sign-out"></i> Salir del menu</a>',
            '</div>',
        '</a>'    
    ].join('')
}
window.operateEvents = {
    'click .active_show_actos': (e, value, row, index) => {
        delete_alert_boot();
        let ident = table_boot_return_objet_jonson(row);
        CONS_PUB_MATRICULA_REGISTRO_PUBLICO = ident.MATRICULA;
        show_lista_actos_expediente(CONS_PUB_ID_REGISTRO_PUBLICO, ident.MATRICULA, row);

    },
    'click .active_show_documentos': (e, value, row, index) => {
        delete_alert_boot();
        let ident = table_boot_return_objet_jonson(row);
        CONS_PUB_MATRICULA_REGISTRO_PUBLICO = ident.MATRICULA;
        show_lista_documentos_matriculado(CONS_PUB_ID_REGISTRO_PUBLICO, ident, row);

    }
}
//Interface para evento de lista de actos de expediente
function operateFormattertablebootmigactos(value, row, index) {
    return [
        '<a style="color: black" title="Lista documentos relacionados al acto " href="#" class="dropdown-item font-weight-light active_show_lista_document_actos" ><i class="fad fa-folder-open"></i>  </a>',
    ].join('')
}
//Interface para evento de lista de actos de expediente
window.operateEventsActos = {
    'click .active_show_lista_document_actos': (e, value, row, index) => {
        delete_alert_boot();
        let ident = table_boot_return_objet_jonson(row);
        show_lista_documentos_relacionados_actos(CONS_PUB_ID_REGISTRO_PUBLICO, ident, row);
    }
}

//Interface para evento de lista de actos de expediente
function operateFormattertablebootmigactos(value, row, index) {
    return [
        '<a style="color: black" title="Lista documentos relacionados al acto " href="#" class="dropdown-item font-weight-light active_show_lista_document_actos" ><i class="fad fa-folder-open"></i>  </a>',
    ].join('')
}
//Interface para evento de lista de actos de expediente
window.operateEventsActos = {
    'click .active_show_lista_document_actos': (e, value, row, index) => {
        delete_alert_boot();
        let ident = table_boot_return_objet_jonson(row);
        show_lista_documentos_relacionados_actos(CONS_PUB_ID_REGISTRO_PUBLICO, ident, row);
    }
}
//Interface para evento de lista documentos actos
function operateFormattertablebootmigactosdocumentos(value, row, index) {
    return [
        '<a style="color: black" href="#" title="Visualiza documento soporte" class="dropdown-item font-weight-light active_show_visualiza_documento" ><i class="fas fa-file-image"></i>  </a>',
      
    ].join('')
}
//Interface para evento de lista documentos actos
window.operateEventsActosDocumentos = {
    'click .active_show_visualiza_documento': (e, value, row, index) => {
        delete_alert_boot();
        Show_visor_documento_consulta_publica_acto(row);
        //show_lista_documentos_relacionados_actos(CONS_PUB_ID_REGISTRO_PUBLICO, ident, row);
    }
}

//Interface para evento de lista documentos del matriculado
function operateFormattertablebootdocumentomatriculado(value, row, index) {
    return [
        '<a style="color: black" href="#" title="Visualiza documento soporte" class="dropdown-item font-weight-light active_show_visualiza_documento" ><i class="fas fa-file-image"></i>  </a>',

    ].join('')
}
//Interface para evento de lista documentos del matriculado
window.operateEventsDocumentoMatriculado = {
    'click .active_show_visualiza_documento': (e, value, row, index) => {
        delete_alert_boot();
        Show_visor_documento_consulta_publica_matriculado(row);
        
    }
}

//-----------------------------------Zona web service --------------------------------------
const Service_REST_registra_usuario_consulta_publica = async (array_data_form) => {
    var serialice = JSON.stringify(array_data_form);
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceConsultaPublica.asmx/Service_registra_usuario_consulta_publica', {
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
                        CONS_PUB_ID_REGISTRO_USUARIO = data.d[0].id_registro_usuario;
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
        }
        catch (ex) {
            resolve(ex.message);
        }
    })
    let result = myPromise;
    return result;
}
//-------Solicita lista gabinetes migracion
const Service_REST_solicita_lista_tipo_consulta_publica = async (id_) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceConsultaPublica.asmx/Service_solicita_lista_tipo_consulta_publica', {
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
                        if (document.getElementById('option_registro_expediente')) {
                            var element_drow = document.getElementById('option_registro_expediente');
                            $("#option_registro_expediente").empty();
                            for (var i = 0; i < ITEMS_DATOS_DROW.length; i++) {
                                element_drow[i] = new Option(ITEMS_DATOS_DROW[i].text, ITEMS_DATOS_DROW[i].value);
                            }
                            //element_drow.addEventListener("change", event_change_drowslis_gabinete_migracion);
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
    let result = myPromise;
    return result;
}
//-------Realiza la consulta de gabinetes para matricualdo y retorna los resultados
const Service_REST_consulta_publica_matriculado_gabinete = async (control_array_config_service, tipo_consulta, valor_consulta, name_table, id_tipo_registro, name_parent_table) => {
    let serialice = JSON.stringify(control_array_config_service);
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceConsultaPublica.asmx/Service_consulta_publica_matriculado_gabinete', {
                data: "{" + "'parameter':'" + serialice + "','" + "tipo_consulta':'" + tipo_consulta + "','" + "valor_consulta':'" + valor_consulta + "','" + "id_registro_publico':'" + id_tipo_registro + "'}",
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
//-------Lista la consuta de actos de una matricualdo
const Service_REST_solicita_lista_actos_expediente = async (control_array_config_service, tipo_consulta, valor_consulta, name_table, id_tipo_registro, name_parent_table) => {
    let serialice = JSON.stringify(control_array_config_service);
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceConsultaPublica.asmx/Service_solicita_lista_actos_expediente', {
                data: "{" + "'parameter':'" + serialice + "','" + "tipo_consulta':'" + tipo_consulta + "','" + "valor_consulta':'" + valor_consulta + "','" + "id_registro_publico':'" + id_tipo_registro + "'}",
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
                        //console.log(data.d[0].Obj_ilist_fileds_generic);
                        //console.log(row);
                        if (document.getElementById("state_migracion") !== null) {
                            document.getElementById("state_migracion").innerText = class_stru_row_Gabinete_Generic.length + " Registro (s) de Gabinete : " + GABINETE_MIG;
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
//-------Retorna lista documentos relacionados al acto
const Service_REST_lista_documentos_relacionados_acto = async (control_array_config_service, tipo_consulta, valor_consulta, name_table, id_tipo_registro, name_parent_table) => {
    let serialice = JSON.stringify(control_array_config_service);
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceConsultaPublica.asmx/Service_lista_documentos_relacionados_actos', {
                data: "{" + "'parameter':'" + serialice + "','" + "tipo_consulta':'" + tipo_consulta + "','" + "valor_consulta':'" + valor_consulta + "','" + "id_registro_publico':'" + id_tipo_registro + "'}",
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
//-------Retorna lista documentos relacionados al matriocualdo
const Service_REST_ista_documentos_relacionados_matriculado = async (control_array_config_service, tipo_consulta, valor_consulta, name_table, id_tipo_registro, name_parent_table) => {  
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceConsultaPublica.asmx/Service_lista_documentos_relacionados_matriculado', {
                data: "{" + "'parameter':'" + control_array_config_service + "','" + "tipo_consulta':'" + tipo_consulta + "','" + "valor_consulta':'" + valor_consulta + "','" + "id_registro_publico':'" + id_tipo_registro + "'}",
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
                        console.log(class_stru_row_Gabinete_Generic);
                        if (document.getElementById("state_migracion") !== null) {
                            document.getElementById("state_migracion").innerText = class_stru_row_Gabinete_Generic.length + " Registro (s) de Gabinete : " + GABINETE_MIG;
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
//-------Solicita la url del documento a visualizar
const Service_REST_Lista_documento_consulta_publica_expediente = async (parameter_, Title_visor) => {
    let serialice = JSON.stringify(parameter_); 
    document.getElementById("IframeVisor_").src = "";
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceDocuarchi.asmx/Service_Lista_documento_consulta_publica_expediente' , {
                data: "{" + "'parameter':'" + serialice  + "'}",
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
                        document.getElementById("IframeVisor_").src = data.d[0].url_iframe;
                        document.getElementById("h_gabibete_imagen").innerText = Title_visor;
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
//-------Solicita la url del documento a visualizar del matriculado
const Service_REST_Lista_documento_consulta_publica_matriculado = async (parameter_, Title_visor) => {
    let serialice = JSON.stringify(parameter_);
    document.getElementById("Iframe_doc_maticulado").src = "";
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceDocuarchi.asmx/Service_Lista_documento_matriculado', {
                data: "{" + "'parameter':'" + serialice + "'}",
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
                        document.getElementById("Iframe_doc_maticulado").src = data.d[0].url_iframe;
                        document.getElementById("h_gabibete_imagen_matricula").innerText = Title_visor;
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

const Service_REST_validate_sesion_consulta_public = (dat) => {
    try {
        $.ajax('../webservice/WebServiceInicioGestor.asmx/web_service_validate_sesion_active', {
            data: "{" + "'DName':'" + dat + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d !== "YES") {
                    alert("Su sesión caducó, lo vamos a direccionar a la página principal");
                    window.location.assign(CONS_PUB_URL_SESION);
                    clearInterval(INTERVAL_SESION_ITEM_MANTENT);
                } 
            }, error: function (xception, textStatus, errorThrown) {

                if (xception.status === 0) {
                    //resolve("Not connect: Verify Network.");


                } else if (xception.status == 404) {
                    //resolve("Requested page not found [404]");


                } else if (xception.status == 500) {
                    //resolve("Internal Server Error [500]." + xception.responseText);


                } else if (textStatus === 'parsererror') {
                    //resolve("Requested JSON parse failed.");


                } else if (textStatus === 'timeout') {
                    //return "Time out error.";


                } else if (textStatus === 'abort') {
                    //resolve("Ajax request aborted.");


                } else {
                    //resolve("Ajax request aborted." + xception.responseText);


                }
            }
        });
    }
    catch (ex) {
        //resolve(ex.message);
    }
}
const Service_REST_inicializa_conexion_consulta_publica = async (dat) => {
    let myPromise = new Promise(function (resolve) {
    try {
        $.ajax('../webservice/WebServiceInicioGestor.asmx/Service_inicializa_conexion_consulta_publica', {
            data: "{" + "'DName':'" + dat + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                resolve(data.d);
               
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
                    //return "Time out error.";
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
const Service_REST_sesion_end = (dat) => {
    try {
        $.ajax('../webservice/WebServiceInicioGestor.asmx/web_service_sesion_end', {
            data: "{" + "'DName':'" + dat + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d == "YES") {
                    alert("Sin sesion ");
                }
            }, error: function (xception, textStatus, errorThrown) {

                if (xception.status === 0) {
                    //resolve("Not connect: Verify Network.");


                } else if (xception.status == 404) {
                    //resolve("Requested page not found [404]");


                } else if (xception.status == 500) {
                    resolve("Internal Server Error [500]." + xception.responseText);


                } else if (textStatus === 'parsererror') {
                    //resolve("Requested JSON parse failed.");


                } else if (textStatus === 'timeout') {
                    //return "Time out error.";


                } else if (textStatus === 'abort') {
                    //resolve("Ajax request aborted.");


                } else {
                    //resolve("Ajax request aborted." + xception.responseText);


                }
            }
        });
    }
    catch (ex) {
        //resolve(ex.message);
    }
}
const Service_REST_service_sesion_return_timeout = (dat) => {
    try {
        $.ajax('../webservice/WebServiceInicioGestor.asmx/Service_sesion_return_timeout', {
            data: "{" + "'DName':'" + dat + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d) {
                    document.getElementById("tor_ico").textContent = data.d; 
                   
                }
            }, error: function (xception, textStatus, errorThrown) {

                if (xception.status === 0) {
                    //resolve("Not connect: Verify Network.");


                } else if (xception.status == 404) {
                    //resolve("Requested page not found [404]");


                } else if (xception.status == 500) {
                    resolve("Internal Server Error [500]." + xception.responseText);


                } else if (textStatus === 'parsererror') {
                    //resolve("Requested JSON parse failed.");


                } else if (textStatus === 'timeout') {
                    //return "Time out error.";


                } else if (textStatus === 'abort') {
                    //resolve("Ajax request aborted.");


                } else {
                    //resolve("Ajax request aborted." + xception.responseText);


                }
            }
        });
    }
    catch (ex) {
        //resolve(ex.message);
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

      

    } catch (ex) {
        alert(ex.message + " Función rezize_event")
    }
}