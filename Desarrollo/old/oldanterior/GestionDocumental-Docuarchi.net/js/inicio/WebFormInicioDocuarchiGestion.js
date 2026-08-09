

$(document).ready(function () {
    $.fn.inicio = function () {
        auto_zise_popup_inicio_docuarchi_inicio();        
    }
    
});
$(window).on("load", function () {
    ini_event_page();
    var elment = document.getElementsByClassName("da_event_captive");
    if (elment) {
        for (var i = 0; i < elment.length; i++) {
            elment[i].addEventListener("click", event_click, false);
        }
    }
    var elment_ = document.getElementsByClassName("coll_sap_active");
    if (elment_) {
        for (var i = 0; i < elment_.length; i++) {
            elment_[i].addEventListener("click", event_onclick_colapse_card, false);
        }
    }
    window.addEventListener("resize", rezize_event); 
    inicializa_menu_scoope();
    auto_zise_popup_inicio_docuarchi_inicio();
    web_service_sesion();
    web_service_sesion_lguin();  
});
var INTERVAL_LOG_SESION;
var INTERVAL_REMPLAZA_PENDIETES_APROBACION;
var INTERVAL_REMPLADATOS_COMPARTIDOS;
var INTERVAL_REMPLAZA_LISTA_TRAMITES;
var INTERVAL_REMPLAZA_DATOS_LISTA_TAREAS;
var INTERVAL_SESION_ITEM_MANTENT;
var INTERVALO_ACTUALIZACION_CONTADORES = 30000;

function detener_polling_contadores_inicio() {
    clearInterval(INTERVAL_REMPLAZA_PENDIETES_APROBACION);
    clearInterval(INTERVAL_REMPLADATOS_COMPARTIDOS);
    clearInterval(INTERVAL_REMPLAZA_LISTA_TRAMITES);
    clearInterval(INTERVAL_REMPLAZA_DATOS_LISTA_TAREAS);
    clearInterval(INTERVAL_SESION_ITEM_MANTENT_SSION_GESTOR);
    clearInterval(INTERVAL_LOG_SESION);
    INTERVAL_REMPLAZA_PENDIETES_APROBACION = null;
    INTERVAL_REMPLADATOS_COMPARTIDOS = null;
    INTERVAL_REMPLAZA_LISTA_TRAMITES = null;
    INTERVAL_REMPLAZA_DATOS_LISTA_TAREAS = null;
    INTERVAL_SESION_ITEM_MANTENT_SSION_GESTOR = null;
    INTERVAL_LOG_SESION = null;
}

window.addEventListener("pagehide", detener_polling_contadores_inicio);
function event_onclick_colapse_card(e) {
    var element = e.currentTarget;
    if (element.classList.contains("collapsed")) {
        element.firstChild.classList.remove("fa-caret-up");
        element.firstChild.classList.add("fa-caret-down");
    } else {
        element.firstChild.classList.add("fa-caret-up");
        element.firstChild.classList.remove("fa-caret-down");
    }
}
const ini_event_page = () => {
    let array_element = new Array;
    array_element.push({ id: "Button_rdedirect_pag" }, { id: "Button_sesion_end_cancelar" }, { id: "Button_sesion_end" }, { id: "boton_sesion_end_active" });
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
        switch (name_ID) {
            case "Button_rdedirect_pag":
                hide_modal_redirect();
                break;
            case "boton_sesion_end_active":
                $("#modal_sesion_end").modal("show");
                break;
            case "Button_sesion_end":
                hide_modal_sesion_end();
                break;
            case "Button_sesion_end_cancelar":
                $("#modal_sesion_end").modal("hide");
                break;
        }
    } catch (ex) {
        alert(ex.mensaje);
    }
}
const hide_modal_redirect = () => {
    try {  
        document.getElementById("container_loading_iframe").style.display = "flex";
        document.getElementById("title_lert_sesion_time_out").style.display = "none";
        document.getElementById("Button_rdedirect_pag").style.display = "none";
        document.getElementById("i_lert_sesion_time_out").style.display = "none";
        document.getElementById("scoop").style.opacity = 0;
        $("#modal_alert_sesion_time_out").modal("hide");
        window.location.assign("../gestor.aspx");     
    }
    catch (ex) {
        
    } finally {
        
    }
}
const hide_modal_sesion_end = async () => {
    try {
        let result = "";
        result = await Service_REST_sesion_end_gestor_async();
        if (result == "YES") {
            document.getElementById("container_loading_iframe").style.display = "flex";
            document.getElementById("scoop").style.opacity = 0;
            $("#modal_sesion_end").modal("hide");
            window.location.assign("../gestor.aspx");
        }  
    }
    catch (ex) {

    } finally {

    }
}
/*----------------------------------------------
 * Carga eventos de servicios web una vez 
 * cargado el formuario
 * 
------------------------------------------------*/
function display_unload() {
    detener_polling_contadores_inicio();
    auto_zise_popup_inicio_docuarchi_inicio();
    INTERVAL_SESION_ITEM_MANTENT_SSION_GESTOR = setInterval('Service_REST_validate_sesion_gestor();', '6030');
    //INTERVAL_SESION_ITEM_MANTENT=setInterval('web_service_sesion_mantener("refresh_session.ashx");', '6030');
    INTERVAL_LOG_SESION = setInterval('set_actualiza_log_sesion_usuario_gestion_documental();', '31200');
    web_service_solicitudes_usuario("Handler_lista_numero_solicitudes_dbase.ashx");
    INTERVAL_REMPLAZA_PENDIETES_APROBACION = setInterval(function () { remplaza_datos_respuestas_pendientes_por_aprobacion("Respuestas pendientes por mi aprobación", "Handler_lista_numero_solicitudes_dbase.ashx"); }, INTERVALO_ACTUALIZACION_CONTADORES);
    web_service_solicitudes_documentos("Handler_Lista_compartidos_por_revision_db.ashx");
    INTERVAL_REMPLADATOS_COMPARTIDOS = setInterval(function () { remplaza_datos_doucumentos_compartidos("", "Handler_Lista_compartidos_por_revision_db.ashx"); }, INTERVALO_ACTUALIZACION_CONTADORES);
    web_service_lista_tramites_asignados("Handler_lista_tramites_wf_asignados_db.ashx");
    INTERVAL_REMPLAZA_LISTA_TRAMITES = setInterval(function () { remplaza_datos_lista_tramites_asignados("", "Handler_lista_tramites_wf_asignados_db.ashx"); }, INTERVALO_ACTUALIZACION_CONTADORES);
    web_service_lista_tareas_asignadas_workflow("Handler_lista_tareas_asignadas_workflow_db.ashx");
    INTERVAL_REMPLAZA_DATOS_LISTA_TAREAS = setInterval(function () { remplaza_datos_lista_tareas_asignadas_workflow("", "Handler_lista_tareas_asignadas_workflow_db.ashx"); }, INTERVALO_ACTUALIZACION_CONTADORES);
    document.getElementById("main_container").style.opacity = 1;
    document.getElementById("header_coop").style.opacity = 1;
    document.getElementById("hader_logo").classList.remove("d-none");
   
}
function rezize_event() {
    try {
        
        auto_zise_popup_inicio_docuarchi_inicio();
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

function sesion_cli() {
    var botonSesion = document.getElementById("ImageButtonSesion");
    if (botonSesion == null) {
        alert("Desde esta pagina auxiliar no puede cerrar sesión");
        return;
    }

    botonSesion.click();
    window.location.assign("../gestor.aspx");
}

var ITEMS_DATOS_TOKENIZE_2 = new Array();
var ESTADO_RESULTADO = "";
var stat_item = 0;
var VALOR_CONSULTA_RADIC;
var URL_NODE;
var INTERBALO;
var VALOR_SOLICITUD_SERVICE = 0;
var VALOR_DOCUMENTOS_SERVICE = 0;
var VALOR_TRAMITES_ASIGNADOS = 0;
var VALOR_TAREAS_ASIGNADAS = 0;
var VALOR_SESION = "";
/*-------------------------------------------------------
 * ------------------------------------------------------
 * Zona de servicios web
 * -------------------------------------------------------
 --------------------------------------------------------*/
function set_actualiza_log_sesion_usuario_gestion_documental() {
    try {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "../webservice/WebServiceGestorDocumental.asmx/Service_actualiza_log_sesion_usuario_gestion_documental",
            data: "{'name':'" + name + "'}",
            dataType: "json",
            success: function (data) {
                //response(data.d);
                if (data.d !== "YES") {
                    //alert(data.d);
                }
            },
            error: function (result) {
                clearInterval(INTERVAL_LOG_SESION);

            }
        });
    } catch (ex) {
    }
}
function web_service_solicitudes_usuario(datas) {
    try {
        var obj = {};
        var jsonData = JSON.stringify(obj);
        $.ajax({
            url: '../radicador/' + datas,
            type: 'POST',
            data: jsonData,
            success: function (data) {
                VALOR_SOLICITUD_SERVICE = data;
            },
            error: function (errorText) {
                clearInterval(INTERVAL_REMPLAZA_PENDIETES_APROBACION);
                
            }
        });
    }
    catch (err) {
        //alert(err.message + " Funcion web_service_solicitudes_usuario");
    }
}
function web_service_solicitudes_documentos(dat) {
    try {
        var obj = {};
        var jsonData = JSON.stringify(obj);
        $.ajax({
            url: '../radicador/' + dat,
            type: 'POST',
            data: jsonData,
            success: function (data) {
                VALOR_DOCUMENTOS_SERVICE = data;
            },
            error: function (errorText) {
                clearInterval(INTERVAL_REMPLADATOS_COMPARTIDOS);
            }
        });
    }
    catch (err) {
        //alert(err.message + " Funcion web_service_solicitudes_documentos");
    }
}
function web_service_lista_tramites_asignados(dat) {
    try {
        var obj = {};
        var jsonData = JSON.stringify(obj);
        $.ajax({
            url: '../radicador/' + dat,
            type: 'POST',
            data: jsonData,
            success: function (data) {
                VALOR_TRAMITES_ASIGNADOS = data;
            },
            error: function (errorText) {
                clearInterval(INTERVAL_REMPLAZA_LISTA_TRAMITES);
            }
        });
    }
    catch (err) {
        alert(err.message + " Funcion web_service_lista_tramites_asignados");
    }
}
function web_service_lista_tareas_asignadas_workflow(dat) {
    try {
        var obj = {};
        var jsonData = JSON.stringify(obj);
        $.ajax({
            url: '../radicador/' + dat,
            type: 'POST',
            data: jsonData,
            success: function (data) {
                VALOR_TAREAS_ASIGNADAS = data;
                pintar_numero_tareas_asignadas_workflow(data);
            },
            error: function (errorText) {   
                clearInterval(INTERVAL_REMPLAZA_DATOS_LISTA_TAREAS);
            }
        });
    }
    catch (err) {
        //alert(err.message + " Funcion web_service_lista_tareas_asignadas_workflow");
    }
}

function pintar_numero_tareas_asignadas_workflow(numero_tareas) {
    try {
        if (!document.getElementById("id_task_workflow_")) {
            return;
        }

        var hayTareas = Number(numero_tareas) > 0;
        var visualizacion = hayTareas ? "block" : "none";

        document.getElementById("id_task_workflow").style.display = visualizacion;
        document.getElementById("id_task_workflow").textContent = numero_tareas;
        document.getElementById("id_task_workflow_").style.display = visualizacion;
        document.getElementById("id_task_workflow_").textContent = numero_tareas;
        document.getElementById("id_task_workflow__").style.display = visualizacion;
        document.getElementById("id_task_workflow__").textContent = numero_tareas;

        if (document.getElementById("WF-CL-01_card_content")) {
            document.getElementById("WF-CL-01_card_content").textContent = numero_tareas;
        }
    }
    catch (err) {
        //La actualización visual no debe interrumpir el siguiente ciclo de consulta.
    }
}

function remplaza_datos_respuestas_pendientes_por_aprobacion(clave_busqueda, url_service) {
    try {
        if (document.getElementById("id_resp_aprobar_")) {
            web_service_solicitudes_usuario(url_service);
            if (VALOR_SOLICITUD_SERVICE == 0) {
                document.getElementById("id_resp_aprobar").style.display = "none";
                document.getElementById("id_resp_aprobar").textContent = VALOR_SOLICITUD_SERVICE;
                //document.getElementById("id_resp_aprobar_").style.display = "none";
                //document.getElementById("id_resp_aprobar_").textContent = VALOR_SOLICITUD_SERVICE;
                document.getElementById("id_resp_aprobar__").style.display = "none";
                document.getElementById("id_resp_aprobar__").textContent = VALOR_SOLICITUD_SERVICE;
            } else {
                document.getElementById("id_resp_aprobar").style.display = "block";
                document.getElementById("id_resp_aprobar").textContent = VALOR_SOLICITUD_SERVICE;
                document.getElementById("id_resp_aprobar_").style.display = "block";
                document.getElementById("id_resp_aprobar_").textContent = "task";
                document.getElementById("id_resp_aprobar__").style.display = "block";
                document.getElementById("id_resp_aprobar__").textContent = VALOR_SOLICITUD_SERVICE;
            }
            if (document.getElementById("CR-RP-03_card_content")) {
                document.getElementById("CR-RP-03_card_content").textContent = VALOR_SOLICITUD_SERVICE;
            }
        }
    }
    catch (err) {
        //alert(err.message + " Funcion remplaza_datos_respuestas_pendientes_por_aprobacion");
    }
}
function remplaza_datos_lista_tramites_asignados(clave_busqueda, url_service) {
    try {
        if (document.getElementById("id_task_asignado_")) {
            web_service_lista_tramites_asignados(url_service);
            if (VALOR_TRAMITES_ASIGNADOS == 0) {
                document.getElementById("id_task_asignado").style.display = "none";
                document.getElementById("id_task_asignado").textContent = VALOR_TRAMITES_ASIGNADOS;
                document.getElementById("id_task_asignado_").style.display = "none";
                document.getElementById("id_task_asignado_").textContent = VALOR_TRAMITES_ASIGNADOS;
               
            } else {
                document.getElementById("id_task_asignado").style.display = "block";
                document.getElementById("id_task_asignado").textContent = VALOR_TRAMITES_ASIGNADOS;
                document.getElementById("id_task_asignado_").style.display = "block";
                document.getElementById("id_task_asignado_").textContent = VALOR_TRAMITES_ASIGNADOS;
                document.getElementById("id_resp_aprobar_").style.display = "block";
                document.getElementById("id_resp_aprobar_").textContent = "task";
               
            }
            if (document.getElementById("CR-GT-01_card_content")) {
                document.getElementById("CR-GT-01_card_content").textContent = VALOR_TRAMITES_ASIGNADOS;
            }
        }
    }
    catch (err) {
        //alert(err.message + " Funcion remplaza_datos_lista_tramites_asignados");
    }
}
function remplaza_datos_doucumentos_compartidos(clave_busqueda, url_service) {
    try {    
        if (document.getElementById("id_docu_aprobacion_")) {
            web_service_solicitudes_documentos(url_service);
            if (VALOR_DOCUMENTOS_SERVICE == 0) {
                document.getElementById("id_docu_aprobacion").style.display = "none";
                document.getElementById("id_docu_aprobacion").textContent = VALOR_DOCUMENTOS_SERVICE;
                document.getElementById("id_docu_aprobacion_").style.display = "none";
                document.getElementById("id_docu_aprobacion_").textContent = VALOR_DOCUMENTOS_SERVICE;
                document.getElementById("id_docu_aprobacion__").style.display = "none";
                document.getElementById("id_docu_aprobacion__").textContent = VALOR_DOCUMENTOS_SERVICE;
            } else {
                document.getElementById("id_docu_aprobacion").style.display = "block";
                document.getElementById("id_docu_aprobacion").textContent = VALOR_DOCUMENTOS_SERVICE;
                document.getElementById("id_docu_aprobacion_").style.display = "block";
                document.getElementById("id_docu_aprobacion_").textContent = VALOR_DOCUMENTOS_SERVICE;
                document.getElementById("id_docu_aprobacion__").style.display = "block";
                document.getElementById("id_docu_aprobacion__").textContent = VALOR_DOCUMENTOS_SERVICE;
            }
            if (document.getElementById("GD-DC-19_card_content")) {
                document.getElementById("GD-DC-19_card_content").textContent = VALOR_DOCUMENTOS_SERVICE;
            }
        }
    }
    catch (err) {
        //alert(err.message + " Funcion remplaza_datos_doucumentos_compartidos");
    }
}
function remplaza_datos_lista_tareas_asignadas_workflow(clave_busqueda, url_service) {
    try {
        if (document.getElementById("id_task_workflow_")) {
            web_service_lista_tareas_asignadas_workflow(url_service);
        }

    }
    catch (err) {
        //alert(err.message + " Funcion remplaza_datos_lista_tareas_asignadas_workflow");
    }
}
/**Inicializa el perfil del radicador en ls variables sesión y lista las plantilla permitidas no funcional */
function web_service_inicializa_menu_principal() {
    try {
        var search = "";
        $.ajax('../webservice/WebServiceInicioGestor.asmx/web_service_inicializa_menu_principal', {
            data: "{'DName':'" + search + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d !== "YES") {
                    ESTADO_RESULTADO = data.d;
                    alert("Error funcion  web_service_inicializa_menu_principal " + data.d)
                } else {

                }
            },
            error: function (errorText) {
                ESTADO_RESULTADO = data.d;
                alert("Error web_service_inicializa_menu_principal : " + errorText.responseText);

            }
        });

    } catch (ex) {
        alert("Funcion web_service_inicializa_menu_principal " + ex.message);
        //clear_proces();

    }
}
function web_service_lista_item_menu() {
    try {
        var search = "";
        $.ajax('../webservice/WebServiceInicioGestor.asmx/web_service_lista_item_menu', {
            data: "{'DName':'" + search + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_funcion !== "YES") {
                    ESTADO_RESULTADO = data.d[0].error_funcion;
                    hide_div();
                    alert("Error funcion  web_service_lista_item_menu " + data.d[0].error_funcion)
                } else {
                    ITEMS_DATOS_TOKENIZE_2 = new Array();
                    $.each(data.d, function (k, v) {
                        ITEMS_DATOS_TOKENIZE_2.push(v);
                    });
                    config_items_menu();
                }
            },
            error: function (errorText) {
                ESTADO_RESULTADO = data.d[0].error_funcion;
                hide_div();
                alert("Error web_service_lista_item_menu : " + errorText.responseText);
            }
        });

    } catch (ex) {
        alert("Funcion web_service_lista_item_menu " + ex.message);

    }
}
function web_service_update_radicado_consulta() {
    try {
        var search = "";
        $.ajax('../webservice/WebServiceInicioGestor.asmx/web_service_update_radicado_consulta', {
            data: "{'DName':'" + VALOR_CONSULTA_RADIC + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d !== "YES") {
                    ESTADO_RESULTADO = "";
                    alert("Error funcion  web_service_update_radicado_consulta " + data.d);
                } else {
                    ESTADO_RESULTADO = "FIN";
                }
            },
            error: function (errorText) {
                ESTADO_RESULTADO = data.d;
                alert("Error web_service_update_radicado_consulta : " + errorText.responseText);

            }
        });

    } catch (ex) {
        alert("Funcion web_service_update_radicado_consulta " + ex.message);
    }
}
function web_service_sesion() {
    try {
        var search = "";
        $.ajax('../webservice/WebServiceInicioGestor.asmx/web_service_sesion_user', {
            data: "{'DName':'" + "" + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                // Conserva la estructura visual del control: título/módulo y login.
                document.getElementById("id_user_loguin").title = data.d;
            },
            error: function (errorText) {
                document.getElementById("id_user_loguin").title = "";
            }
        });

    } catch (ex) {
        alert("Funcion web_service_update_radicado_consulta " + ex.message);

    }
}
function web_service_sesion_lguin() {
    try {
       
        $.ajax('../webservice/WebServiceInicioGestor.asmx/web_service_loguin_user', {
            data: "{'DName':'" + "" + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                // No reemplazar el <li>: el CSS presenta este span como el login.
                document.getElementById("user_usuario_loguin").textContent = data.d;
               
            },
            error: function (errorText) {
                document.getElementById("user_usuario_loguin").textContent = "";
                

            }
        });

    } catch (ex) {
        alert("Funcion web_service_sesion_lguin " + ex.message);

    }
}

function web_service_inicializa_gestion_expediente(ob) {
    try {

        $.ajax('../webservice/WebServiceGaExpediente.asmx/Service_inicializa_gestion_expediente', {
            data: "{ 'parameter':'" + 0 + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d !== "YES") {
                    alert(data.d);
                   
                } else {
                    if (ob) {
                        document.getElementById("id_selecion_opcion_trea").textContent = ob.Text_node.toUpperCase();
                        loading_iframe('ContentPlacenter_ifrm_ds_', ob.url_node);
                    }  
                    document.getElementById("content_iframe_ds").style.display = "flex";
                    document.getElementById("id_selecion_opcion_trea").style.display = "flex";
                    document.getElementById("card_general_ini_text").style.display = "none";
                    
                }
                
            }, error: function (xception, textStatus, errorThrown) {
               
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
        alert('web_service_inicializa_gestion_expediente  ' + ex.message);



    }
}

function scrope() {
    $(document).ready(function () {   
        $("#scoop").scoopmenu({
            themelayout: 'vertical',
            verticalMenuplacement: 'left',		// value should be left/right
            verticalMenulayout: 'wide',   		// value should be wide/box/widebox
            MenuTrigger: 'click',
            SubMenuTrigger: 'click',
            activeMenuClass: 'active',
            ThemeBackgroundPattern: 'pattern6',
            HeaderBackground: 'theme2',
            LHeaderBackground: 'theme2',
            NavbarBackground: 'theme2',
            ActiveItemBackground: 'theme8',
            SubItemBackground: 'theme8',
            ActiveItemStyle: 'style2',
            ItemBorder: true,
            ItemBorderStyle: 'solid',
            SubItemBorder: true,
            DropDownIconStyle: 'style3', // Value should be style1,style2,style3
            FixedNavbarPosition: true,
            FixedHeaderPosition: true,
            collapseVerticalLeftHeader: true,
            VerticalSubMenuItemIconStyle: 'style6',  // value should be style1,style2,style3,style4,style5,style6
            VerticalNavigationView: 'view1',
            verticalMenueffect: {
                desktop: "shrink",
                tablet: "push",
                phone: "overlay",
            },
            defaultVerticalMenu: {
                desktop: "expanded",	// value should be offcanvas/collapsed/expanded/compact/compact-acc/fullpage/ex-popover/sub-expanded
                tablet: "collapsed",		// value should be offcanvas/collapsed/expanded/compact/fullpage/ex-popover/sub-expanded
                phone: "offcanvas",		// value should be offcanvas/collapsed/expanded/compact/fullpage/ex-popover/sub-expanded
            },
            onToggleVerticalMenu: {
                desktop: "collapsed",		// value should be offcanvas/collapsed/expanded/compact/fullpage/ex-popover/sub-expanded
                tablet: "expanded",		// value should be offcanvas/collapsed/expanded/compact/fullpage/ex-popover/sub-expanded
                phone: "expanded",			// value should be offcanvas/collapsed/expanded/compact/fullpage/ex-popover/sub-expanded
            },

        });


        /* Left header Theme Change function Start */
        function handleleftheadertheme() {
            $('.theme-color > a.leftheader-theme').on("click", function () {
                var lheadertheme = $(this).attr("lheader-theme");
                $('.scoop-header .scoop-left-header').attr("lheader-theme", lheadertheme);
            });
        };

        handleleftheadertheme();
        /* Left header Theme Change function Close */
        /* header Theme Change function Start */
        function handleheadertheme() {
            $('.theme-color > a.header-theme').on("click", function () {
                var headertheme = $(this).attr("header-theme");
                $('.scoop-header').attr("header-theme", headertheme);
            });
        };
        handleheadertheme();
        /* header Theme Change function Close */
        /* Navbar Theme Change function Start */
        function handlenavbartheme() {
            $('.theme-color > a.navbar-theme').on("click", function () {
                var navbartheme = $(this).attr("navbar-theme");
                $('.scoop-navbar').attr("navbar-theme", navbartheme);
            });
        };

        handlenavbartheme();
        /* Navbar Theme Change function Close */
        /* Active Item Theme Change function Start */
        function handleactiveitemtheme() {
            $('.theme-color > a.active-item-theme').on("click", function () {
                var activeitemtheme = $(this).attr("active-item-theme");
                $('.scoop-navbar').attr("active-item-theme", activeitemtheme);
            });
        };

        handleactiveitemtheme();
        /* Active Item Theme Change function Close */
        /* SubItem Theme Change function Start */
        function handlesubitemtheme() {
            $('.theme-color > a.sub-item-theme').on("click", function () {
                var subitemtheme = $(this).attr("sub-item-theme");
                $('.scoop-navbar').attr("sub-item-theme", subitemtheme);
            });
        };

        handlesubitemtheme();
        /* SubItem Theme Change function Close */
        /* Theme background pattren Change function Start */
        function handlethemebgpattern() {
            $('.theme-color > a.themebg-pattern').on("click", function () {
                var themebgpattern = $(this).attr("themebg-pattern");
                $('body').attr("themebg-pattern", themebgpattern);
            });
        };

        handlethemebgpattern();
        /* Theme background pattren Change function Close */
        /* Vertical Navigation View Change function start*/
        function handleVerticalNavigationViewChange() {
            $('#navigation-view').val('view1').on('change', function (get_value) {
                get_value = $(this).val();
                $('.scoop').attr('vnavigation-view', get_value);
            });
        };

        handleVerticalNavigationViewChange();
        /* Theme Layout Change function Close*/
        /* Theme Layout Change function start*/
        function handlethemeverticallayout() {
            $('#theme-layout').val('wide').on('change', function (get_value) {
                get_value = $(this).val();
                $('.scoop').attr('vertical-layout', get_value);
            });
        };

        handlethemeverticallayout();
        /* Theme Layout Change function Close*/
        /* Menu effect change function start*/
        function handleverticalMenueffect() {
            $('#vertical-menu-effect').val('shrink').on('change', function (get_value) {
                get_value = $(this).val();
                $('.scoop').attr('vertical-effect', get_value);
            });
        };

        handleverticalMenueffect();
        /* Menu effect change function Close*/
        /* Vertical Menu Placement change function start*/
        function handleverticalMenuplacement() {
            $('#vertical-navbar-placement').val('left').on('change', function (get_value) {
                get_value = $(this).val();
                $('.scoop').attr('vertical-placement', get_value);
                $('.scoop-navbar').attr("scoop-navbar-position", 'absolute');
                $('.scoop-header .scoop-left-header').attr("scoop-lheader-position", 'relative');
            });
        };

        handleverticalMenuplacement();
        /* Vertical Menu Placement change function Close*/
        /* Vertical Active Item Style change function Start*/
        function handleverticalActiveItemStyle() {
            $('#vertical-activeitem-style').val('style1').on('change', function (get_value) {
                get_value = $(this).val();
                $('.scoop-navbar').attr('active-item-style', get_value);
            });
        };

        handleverticalActiveItemStyle();
        /* Vertical Active Item Style change function Close*/
        /* Vertical Item border change function Start*/
        function handleVerticalIItemBorder() {
            $('#vertical-item-border').change(function () {
                if ($(this).is(":checked")) {
                    $('.scoop-navbar .scoop-item').attr('item-border', 'false');
                } else {
                    $('.scoop-navbar .scoop-item').attr('item-border', 'true');
                }
            });
        };

        handleVerticalIItemBorder();
        /* Vertical Item border change function Close*/
        /* Vertical SubItem border change function Start*/
        function handleVerticalSubIItemBorder() {
            $('#vertical-subitem-border').change(function () {
                if ($(this).is(":checked")) {
                    $('.scoop-navbar .scoop-item').attr('subitem-border', 'false');
                } else {
                    $('.scoop-navbar .scoop-item').attr('subitem-border', 'true');
                }
            });
        };

        handleVerticalSubIItemBorder();
        /* Vertical SubItem border change function Close*/
        /* Vertical Item border Style change function Start*/
        function handleverticalboderstyle() {
            $('#vertical-border-style').val('solid').on('change', function (get_value) {
                get_value = $(this).val();
                $('.scoop-navbar .scoop-item').attr('item-border-style', get_value);
            });
        };

        handleverticalboderstyle();
        /* Vertical Item border Style change function Close*/
        /* Vertical Dropdown Icon change function Start*/
        function handleVerticalDropDownIconStyle() {
            $('#vertical-dropdown-icon').val('style1').on('change', function (get_value) {
                get_value = $(this).val();
                $('.scoop-navbar .scoop-hasmenu').attr('dropdown-icon', get_value);
            });
        };

        handleVerticalDropDownIconStyle();
        /* Vertical Dropdown Icon change function Close*/
        /* Vertical SubItem Icon change function Start*/

        function handleVerticalSubMenuItemIconStyle() {
            $('#vertical-subitem-icon').val('style5').on('change', function (get_value) {
                get_value = $(this).val();
                $('.scoop-navbar .scoop-hasmenu').attr('subitem-icon', get_value);
            });
        };

        handleVerticalSubMenuItemIconStyle();
        /* Vertical SubItem Icon change function Close*/
        /* Vertical Navbar Position change function Start*/
        function handlesidebarposition() {
            $('#sidebar-position').change(function () {
                if ($(this).is(":checked")) {
                    $('.scoop-navbar').attr("scoop-navbar-position", 'fixed');
                    $('.scoop-header .scoop-left-header').attr("scoop-lheader-position", 'fixed');
                } else {
                    $('.scoop-navbar').attr("scoop-navbar-position", 'absolute');
                    $('.scoop-header .scoop-left-header').attr("scoop-lheader-position", 'relative');
                }
            });
        };

        handlesidebarposition();
        /* Vertical Navbar Position change function Close*/
        /* Vertical Header Position change function Start*/
        function handleheaderposition() {
            $('#header-position').change(function () {
                if ($(this).is(":checked")) {
                    $('.scoop-header').attr("scoop-header-position", 'fixed');
                    $('.scoop-main-container').css('margin-top', $(".scoop-header").outerHeight());
                } else {
                    $('.scoop-header').attr("scoop-header-position", 'relative');
                    $('.scoop-main-container').css('margin-top', '0px');
                }
            });
        };

        handleheaderposition();
        /* Vertical Header Position change function Close*/


        /*  collapseable Left Header Change Function Start here*/
        function handlecollapseLeftHeader() {
            $('#collapse-left-header').change(function () {
                if ($(this).is(":checked")) {
                    $('.scoop-header, .scoop ').removeClass('iscollapsed');
                    $('.scoop-header, .scoop').addClass('nocollapsed');
                } else {
                    $('.scoop-header, .scoop').addClass('iscollapsed');
                    $('.scoop-header, .scoop').removeClass('nocollapsed');
                }
            });
        };

        handlecollapseLeftHeader();


        /*  collapseable Left Header Change Function Close here*/


        $(function () {
            var values = [3, 4, 5, 8, 6, 10, 3, 6, 9, 12, 5, 6, 10, 8, 9, 15, 14, 10, 9, 20, 16, 14, 10, 12, 9, 5, 6, 8, 6, 10, 6];
            $('.rsa').sparkline(values, {
                type: "bar",
                tooltipSuffix: " widgets",
                height: '50px',
                barSpacing: 1,
                barWidth: 4,
                barColor: '#70ca63',
                tooltipFormat: "{{value:val}}",
                tooltipValueLookups: { "val": { "-1": "N/A" } }
            });
        });
        $(function () {
            var values = [3, 4, 5, 8, 6, 10, 3, 6, 9, 12, 5, 6, 10, 8, 9, 15, 14, 10, 9, 20, 16, 14, 10, 12, 9, 5, 6, 8, 6, 10, 6];
            $('.tsa').sparkline(values, {
                type: "bar",
                tooltipSuffix: " widgets",
                height: '50px',
                barSpacing: 1,
                barWidth: 4,
                barColor: '#f9ab49',
            });
        });
        $(function () {
            var values = [3, 4, 5, 8, 6, 10, 3, 6, 9, 12, 5, 6, 10, 8, 9, 15, 14, 10, 9, 20, 16, 14, 10, 12, 9, 5, 6, 8, 6, 10, 6];
            $('.isa').sparkline(values, {
                type: "bar",
                tooltipSuffix: " widgets",
                height: '50px',
                barSpacing: 1,
                barWidth: 4,
                barColor: '#24b4b7',
            });
        });
        $(function () {
            var values = [3, 4, 5, 8, 6, 10, 3, 6, 9, 12, 5, 6, 10, 8, 9, 15, 14, 10, 9, 20, 16, 14, 10, 12, 9, 5, 6, 8, 6, 10, 6];
            $('.ssa').sparkline(values, {
                type: "bar",
                tooltipSuffix: " widgets",
                height: '50px',
                barSpacing: 1,
                barWidth: 4,
                barColor: '#25726e',

            });
        });


    });
}
function inicializa_menu_scoope() {
    try {
        if (stat_item == 0) {
            stat_item = 1;
            web_service_lista_item_menu();        
        }
    } catch (ex) {
        alert(ex.message);
    }
}

function pre_interval() {
    INTERBALO = setInterval(actualiza_valor_radicado_consulta(), 50);
}
function actualiza_valor_radicado_consulta() {
    try {
        
        if (ESTADO_RESULTADO == "YES") {
            ESTADO_RESULTADO = "";
            web_service_update_radicado_consulta();
            loading_iframe('ContentPlacenter_ifrm_ds_', URL_NODE);
            
        }
        if (ESTADO_RESULTADO == "FIN") {
            loading_iframe('ContentPlacenter_ifrm_ds_', URL_NODE);
            clearInterval(INTERBALO);       
        }
       
    }
    catch (ex) {
        alert("Funcion actualiza_valor_radicado_consulta " + ex.message);
    }
}

function config_items_menu() {
    try {
        
        //Agrega los item de plantilla de radicacion dinamica
        if (document.getElementById("CR-PR-11")) {
            for (i = 1; i < ITEMS_DATOS_TOKENIZE_2.length; i++) {
                if (ITEMS_DATOS_TOKENIZE_2[i].nodo_plantilla_radicado === "yes") {
                    //crea el elemento ul del menu
                    //var element_ul = document.createElement("ul");
                    //element_ul.classList.add("scoop-submenu");
                    //crea elemento li
                    var element_li = document.createElement("li");
                    var na = "scoop-trigger";
                    element_li.id = ITEMS_DATOS_TOKENIZE_2[i].value_node;
                    element_li.classList.add(".");
                    element_li.classList.add(na);
                    //Agrega elemento li
                    //element_ul.appendChild(element_li);
                    //Agrega elemento a
                    var element_a = document.createElement("A");
                    //element_a.href="javascript:void(0)";
                    element_a.href = "javascript:void(0)";
                    //Agrega elemento span contenedor del elemento i del icono
                    var element_span = document.createElement("span");
                    element_span.classList.add("scoop-micon");
                    //Agrega elemento de i del icono
                    var element_i = document.createElement("I");
                    element_i.classList.add("icon-chart");
                    element_span.appendChild(element_i);
                    element_a.appendChild(element_span);
                    //Agrega elemento span con el nombre del menu
                    element_span = document.createElement("span");
                    element_span.classList.add("scoop-mtext");
                    element_span.textContent = ITEMS_DATOS_TOKENIZE_2[i].Text_node;
                    element_a.appendChild(element_span);
                    //Agrega el elemento span del marcado
                    element_span = document.createElement("span");
                    element_span.classList.add("scoop-mcaret");
                    element_a.appendChild(element_span);
                    element_li.appendChild(element_a);
                    var element_ = document.getElementById("class_rad");
                    element_.appendChild(element_li);
                    $(element_li).on('click', function () { 
                        event_menu_prinicipal(this, event)
                    });

                }
            }
        }
        if (document.getElementById("CR-PR-12")) {
            for (i = 1; i < ITEMS_DATOS_TOKENIZE_2.length; i++) {
                if (ITEMS_DATOS_TOKENIZE_2[i].nodo_plantilla_radicado == "consulta") {
                    //crea el elemento ul del menu
                    //var element_ul = document.createElement("ul");
                    //element_ul.classList.add("scoop-submenu");
                    //crea elemento li
                    var element_li = document.createElement("li");
                    var na = "scoop-trigger";
                    element_li.id = ITEMS_DATOS_TOKENIZE_2[i].value_node;

                    element_li.classList.add(".");
                    element_li.classList.add(na);
                    //Agrega elemento li
                    //element_ul.appendChild(element_li);
                    //Agrega elemento a
                    var element_a = document.createElement("A");
                    //element_a.href="javascript:void(0)";
                    element_a.href = "javascript:void(0)";
                    //Agrega elemento span contenedor del elemento i del icono
                    var element_span = document.createElement("span");
                    element_span.classList.add("scoop-micon");
                    //Agrega elemento de i del icono
                    var element_i = document.createElement("I");
                    element_i.classList.add("icon-chart");
                    element_span.appendChild(element_i);
                    element_a.appendChild(element_span);
                    //Agrega elemento span con el nombre del menu
                    element_span = document.createElement("span");
                    element_span.classList.add("scoop-mtext");
                    element_span.textContent = ITEMS_DATOS_TOKENIZE_2[i].Text_node;
                    element_a.appendChild(element_span);
                    //Agrega el elemento span del marcado
                    element_span = document.createElement("span");
                    element_span.classList.add("scoop-mcaret");
                    element_a.appendChild(element_span);
                    element_li.appendChild(element_a);
                    var element_ = document.getElementById("class_rad_consulta");
                    element_.appendChild(element_li);
                    $(element_li).on('click', function () { //esta función se ejecutará en todos los casos
                        event_menu_prinicipal(this, event)
                    });

                }
            }

        }
        //-----------------------------------------------------
        //Crea las card de acceso directo
        //-----------------------------------------------------
        var contador_direct = 0;
        for (i = 0; i < ITEMS_DATOS_TOKENIZE_2.length; i++) {
            if (ITEMS_DATOS_TOKENIZE_2[i].visible_node == 1 && ITEMS_DATOS_TOKENIZE_2[i].value_card_conten !== "" && ITEMS_DATOS_TOKENIZE_2[i].url_node !== "") {
                var mensaje = add_card_inicio(ITEMS_DATOS_TOKENIZE_2[i].value_card, ITEMS_DATOS_TOKENIZE_2[i].value_card_conten, ITEMS_DATOS_TOKENIZE_2[i].Text_node, ITEMS_DATOS_TOKENIZE_2[i].tipo_modulo, "group_card_chek_0001","");
                if (mensaje !== "YES") {
                    alert("Error cargando card de inicio " + message);
                    return true;
                }
                contador_direct++;
            }
            
        }
        if (document.getElementById("content_card_count")) {
            document.getElementById("content_card_count").textContent = contador_direct;
        }
        //-----------------------------------------------------
        //Crea las card de acceso workflow
        //-----------------------------------------------------
        contador_direct = 0;
        for (i = 0; i < ITEMS_DATOS_TOKENIZE_2.length; i++) {
            if (ITEMS_DATOS_TOKENIZE_2[i].visible_node == 1 && ITEMS_DATOS_TOKENIZE_2[i].value_card !== "" && ITEMS_DATOS_TOKENIZE_2[i].url_node !== "" && ITEMS_DATOS_TOKENIZE_2[i].tipo_modulo == "WF" ) {
                var mensaje = add_card_inicio(ITEMS_DATOS_TOKENIZE_2[i].value_card, ITEMS_DATOS_TOKENIZE_2[i].value_card_conten, ITEMS_DATOS_TOKENIZE_2[i].Text_node, ITEMS_DATOS_TOKENIZE_2[i].tipo_modulo, "group_card_chek_0001_wf","");
                if (mensaje !== "YES") {
                    alert("Error cargando card de inicio " + message);
                    return true;
                }
                contador_direct++;
            }
        }
        if (document.getElementById("content_card_wf_count")) {
            document.getElementById("content_card_wf_count").textContent = contador_direct;
        }
        if (contador_direct == 0) {
            document.getElementById("content_card_wf").style.display = "none";
        }
        //-----------------------------------------------------
        //Crea las card de acceso docuarchi
        //-----------------------------------------------------
        contador_direct = 0;
        for (i = 0; i < ITEMS_DATOS_TOKENIZE_2.length; i++) {
            if (ITEMS_DATOS_TOKENIZE_2[i].visible_node == 1 && ITEMS_DATOS_TOKENIZE_2[i].value_card !== "" && ITEMS_DATOS_TOKENIZE_2[i].url_node !== "" && ITEMS_DATOS_TOKENIZE_2[i].tipo_modulo == "DA") {
                var mensaje = add_card_inicio(ITEMS_DATOS_TOKENIZE_2[i].value_card, ITEMS_DATOS_TOKENIZE_2[i].value_card_conten, ITEMS_DATOS_TOKENIZE_2[i].Text_node, ITEMS_DATOS_TOKENIZE_2[i].tipo_modulo, "group_card_chek_0001_da","");
                if (mensaje !== "YES") {
                    alert("Error cargando card de inicio " + message);
                    return true;
                }
                contador_direct++;
            }
        }
        if (document.getElementById("content_card_da_count")) {
            document.getElementById("content_card_da_count").textContent = contador_direct;
        }
        if (contador_direct == 0) {
            document.getElementById("content_card_da").style.display = "none";
        }
        //-----------------------------------------------------
        //Crea las card de acceso correspondencia
        //-----------------------------------------------------
        contador_direct = 0;
        for (i = 0; i < ITEMS_DATOS_TOKENIZE_2.length; i++) {
            if (ITEMS_DATOS_TOKENIZE_2[i].visible_node == 1 && ITEMS_DATOS_TOKENIZE_2[i].value_card !== "" && ITEMS_DATOS_TOKENIZE_2[i].url_node !== "" && ITEMS_DATOS_TOKENIZE_2[i].tipo_modulo == "RD" ) {
                var valor_node = ""
                var valor_plantilla = "group_card_chek_0001_rd";
                if (ITEMS_DATOS_TOKENIZE_2[i].nodo_plantilla_radicado == "yes") {
                    valor_node = "radica";
                    valor_plantilla = "group_card_chek_0001_rd_1";
                }
                if (ITEMS_DATOS_TOKENIZE_2[i].nodo_plantilla_radicado == "consulta") {
                    valor_node = ITEMS_DATOS_TOKENIZE_2[i].nodo_plantilla_radicado;
                    valor_plantilla = "group_card_chek_0001_rd_1";
                }
                var mensaje = add_card_inicio(ITEMS_DATOS_TOKENIZE_2[i].value_card, ITEMS_DATOS_TOKENIZE_2[i].value_card_conten, ITEMS_DATOS_TOKENIZE_2[i].Text_node, ITEMS_DATOS_TOKENIZE_2[i].tipo_modulo, valor_plantilla, valor_node);
                if (mensaje !== "YES") {
                    alert("Error cargando card de inicio " + message);
                    return true;
                }
                contador_direct++;
            }
        }
        if (document.getElementById("content_card_rd_count")) {
            document.getElementById("content_card_rd_count").textContent = contador_direct;
        }
        if (contador_direct == 0) {
            document.getElementById("content_card_rd").style.display = "none";
        }
        //-----------------------------------------------------
        //Crea las card de acceso gestion
        //-----------------------------------------------------
        contador_direct = 0;
        for (i = 0; i < ITEMS_DATOS_TOKENIZE_2.length; i++) {
            if (ITEMS_DATOS_TOKENIZE_2[i].visible_node === 1  && ITEMS_DATOS_TOKENIZE_2[i].value_card !== "" && ITEMS_DATOS_TOKENIZE_2[i].url_node !== "" && ITEMS_DATOS_TOKENIZE_2[i].tipo_modulo == "GD") {
                var mensaje = add_card_inicio(ITEMS_DATOS_TOKENIZE_2[i].value_card, ITEMS_DATOS_TOKENIZE_2[i].value_card_conten, ITEMS_DATOS_TOKENIZE_2[i].Text_node, ITEMS_DATOS_TOKENIZE_2[i].tipo_modulo, "group_card_chek_0001_gd","");
                if (mensaje !== "YES") {
                    alert("Error cargando card de inicio " + message);
                    return true;
                }
                contador_direct++;
            }
        }
        if (document.getElementById("content_card_gd_count")) {
            document.getElementById("content_card_gd_count").textContent = contador_direct;
        }
        if (contador_direct == 0) {
            document.getElementById("content_card_gd").style.display = "none";
        }
        for (i = 0; i < ITEMS_DATOS_TOKENIZE_2.length; i++) {
            if (ITEMS_DATOS_TOKENIZE_2[i].visible_node == 0) {
                if (document.getElementById(ITEMS_DATOS_TOKENIZE_2[i].value_node)) {
                    var ob_remo = document.getElementById(ITEMS_DATOS_TOKENIZE_2[i].value_node);
                    if (ob_remo) {
                        $(ob_remo).remove();
                    }
                }
            }
        }
    } catch (ex) {
        alert("Error funcion config_items_menu " + ex.message)
    } finally {
        scrope();
    }
}
function add_card_inicio(id_card, id_card_count, text_card, tipo_modulo,name_card_parent, card_title) {
    try {
        var div_tml_goup_card = document.getElementById(name_card_parent);
        //Agrega html de la card
        var divhtml = document.createElement("div");
        divhtml.classList.add("card");
        divhtml.classList.add("wrap-login100_person");
        divhtml.classList.add("m-1");
        div_tml_goup_card.appendChild(divhtml);
        //Agrega div contenido de la card
        var divhtml_content = document.createElement("div");
        divhtml_content.classList.add("card-body");
        divhtml_content.classList.add("car_cursor_person");
        divhtml_content.id = id_card;
        divhtml_content.setAttribute("onclick", "event_menu_prinicipal(this, event);")
        divhtml.appendChild(divhtml_content);
        //agrega el card flex de la card
        var divhtml_content_flex = document.createElement("div");
        divhtml_content_flex.classList.add("media");
        divhtml_content_flex.classList.add("d-flex");
        divhtml_content.appendChild(divhtml_content_flex);
        //Agrega div del icono de la card
        var divhtml_content_icono = document.createElement("div");
        divhtml_content_icono.classList.add("align-self-center");
        divhtml_content_flex.appendChild(divhtml_content_icono);
        //Agrega icono li de la card
        var ihtml_icono = document.createElement("i");
        if (tipo_modulo == "WF") {
            ihtml_icono.classList.add("fal");
            ihtml_icono.classList.add("fa-sitemap");
        }
        if (tipo_modulo == "DA") {
            ihtml_icono.classList.add("fal");
            ihtml_icono.classList.add("fa-books");
        }
        if (tipo_modulo == "RD") {
            ihtml_icono.classList.add("fal");
            ihtml_icono.classList.add("fa-envelope-open");
        }
        if (tipo_modulo == "GD") {
            ihtml_icono.classList.add("fal");
            ihtml_icono.classList.add("fa-folder");
        }
        ihtml_icono.classList.add("fa-2x");
        ihtml_icono.classList.add("float-left");
        ihtml_icono.style.color = "#57b846";
        if (tipo_modulo == "WF") {
            ihtml_icono.style.color = "#57b846";
        }
        if (tipo_modulo == "RD") {
            ihtml_icono.style.color = "#007bff";
        }
        if (tipo_modulo == "DA") {
            ihtml_icono.style.color = "#a65157";
        }
        if (tipo_modulo == "GD") {
            ihtml_icono.style.color = "#fecd33";
        }
        divhtml_content_icono.appendChild(ihtml_icono);
        //Agrega el body tex del card an div felx
        var divhtml_body_text = document.createElement("div");
        divhtml_body_text.classList.add("media-body");
        divhtml_body_text.classList.add("text-right");
        divhtml_content_flex.appendChild(divhtml_body_text);
        //Agrega contador card text
        var h3tml_body_text_contador = document.createElement("h6");
        if (id_card_count !== "") {
            h3tml_body_text_contador.id = id_card_count;
           
        }
        h3tml_body_text_contador.innerText = card_title;
        divhtml_body_text.appendChild(h3tml_body_text_contador);
        //Agrega el espan con el titulo de la card
        var spanhtml = document.createElement("SPAN");
        spanhtml.innerText = text_card;
        divhtml_body_text.appendChild(spanhtml);
        //Agrega spnan modulo card
        var spanhtml_modulo = document.createElement("SPAN");
        spanhtml_modulo.classList.add("ml-2");
        spanhtml_modulo.innerText = tipo_modulo;
        spanhtml_modulo.style.color = "#57b846";
        if (tipo_modulo == "WF") {
            spanhtml_modulo.style.color = "#57b846";
        }
        if (tipo_modulo == "RD") {
            spanhtml_modulo.style.color = "#007bff";
        }
        if (tipo_modulo == "DA") {
            spanhtml_modulo.style.color = "#a65157";
        }
        if (tipo_modulo == "GD") {
            spanhtml_modulo.style.color = "#fecd33";
        }
        divhtml_body_text.appendChild(spanhtml_modulo);
        return "YES";
    } catch (ex) {
        return ex.message;
        
    }
}
function hide_div() {   
    if (document.getElementById("nav_menu")) {
        document.getElementById("nav_menu").style.display = "none";
       
    }
}
function event_menu_prinicipal(element, event) {
    try {
        detener_polling_contadores_inicio();
        for (i = 0; i < ITEMS_DATOS_TOKENIZE_2.length; i++) {
            if (ITEMS_DATOS_TOKENIZE_2[i].value_node == element.id || ITEMS_DATOS_TOKENIZE_2[i].value_card == element.id) {
                if (ITEMS_DATOS_TOKENIZE_2[i].url_node === "") {
                   
                    return true;
                }
                if (ITEMS_DATOS_TOKENIZE_2[i].url_externa === "YES") {
                    document.getElementById("Hidden_selecion_url").value = ITEMS_DATOS_TOKENIZE_2[i].url_node;
                    document.getElementById("Hidden_tipo_contenido_content").value = ITEMS_DATOS_TOKENIZE_2[i].value_content;
                    var w = window.open(ITEMS_DATOS_TOKENIZE_2[i].url_content, '_blank');
                    w.focus();
                    return true;
                }
                if (document.getElementById(ITEMS_DATOS_TOKENIZE_2[i].value_card)) {
                    switch (ITEMS_DATOS_TOKENIZE_2[i].value_node) {
                        case "GD-MR-16":
                            VALOR_CONSULTA_RADIC = "PRODUCCION|";
                            URL_NODE = ITEMS_DATOS_TOKENIZE_2[i].url_node;
                            ESTADO_RESULTADO = "YES";
                            document.getElementById("content_iframe_ds").style.display = "flex";
                            document.getElementById("id_selecion_opcion_trea").style.display = "flex";
                            document.getElementById("card_general_ini_text").style.display = "none";
                            pre_interval();
                            break;
                        //Caso inicializa expediente
                        case "GD-CE-03" :
                            web_service_inicializa_gestion_expediente(ITEMS_DATOS_TOKENIZE_2[i]);
                            break;
                        default:
                            if (ITEMS_DATOS_TOKENIZE_2[i].nodo_plantilla_radicado == "yes") {
                                VALOR_CONSULTA_RADIC = "RADICACION|" + ITEMS_DATOS_TOKENIZE_2[i].id_plantilla + "|" + ITEMS_DATOS_TOKENIZE_2[i].tipo_plantilla + "|0|" + ITEMS_DATOS_TOKENIZE_2[i].Text_node + "|" + ITEMS_DATOS_TOKENIZE_2[i].tipo_plantilla;
                                URL_NODE = ITEMS_DATOS_TOKENIZE_2[i].url_node;
                                document.getElementById("id_selecion_opcion_trea").textContent = ITEMS_DATOS_TOKENIZE_2[i].Text_node.toUpperCase();
                                ESTADO_RESULTADO = "YES";
                                document.getElementById("content_iframe_ds").style.display = "flex";
                                document.getElementById("id_selecion_opcion_trea").style.display = "flex";
                                document.getElementById("card_general_ini_text").style.display = "none";
                                pre_interval();
                                break;
                            }
                            if (ITEMS_DATOS_TOKENIZE_2[i].nodo_plantilla_radicado == "consulta") {
                                VALOR_CONSULTA_RADIC = "CONSULTA|" + ITEMS_DATOS_TOKENIZE_2[i].id_plantilla + "|" + ITEMS_DATOS_TOKENIZE_2[i].tipo_plantilla + "|0|" + ITEMS_DATOS_TOKENIZE_2[i].Text_node + "|" + ITEMS_DATOS_TOKENIZE_2[i].tipo_plantilla;
                                URL_NODE = ITEMS_DATOS_TOKENIZE_2[i].url_node;
                                document.getElementById("id_selecion_opcion_trea").textContent = ITEMS_DATOS_TOKENIZE_2[i].Text_node.toUpperCase();
                                ESTADO_RESULTADO = "YES";
                                document.getElementById("content_iframe_ds").style.display = "flex";
                                document.getElementById("id_selecion_opcion_trea").style.display = "flex";
                                document.getElementById("card_general_ini_text").style.display = "none";
                                pre_interval();
                                break;
                            }
                            if (ITEMS_DATOS_TOKENIZE_2[i].nodo_plantilla_radicado != "yes" || ITEMS_DATOS_TOKENIZE_2[i].nodo_plantilla_radicado != "consulta") {             
                                document.getElementById("id_selecion_opcion_trea").textContent = ITEMS_DATOS_TOKENIZE_2[i].Text_node.toUpperCase();
                                document.getElementById("content_iframe_ds").style.display = "flex";
                                document.getElementById("id_selecion_opcion_trea").style.display = "flex";
                                document.getElementById("card_general_ini_text").style.display = "none";
                                loading_iframe('ContentPlacenter_ifrm_ds_', ITEMS_DATOS_TOKENIZE_2[i].url_node);
                                return true;
                                    
                            }
                    }

                }
                if (document.getElementById(ITEMS_DATOS_TOKENIZE_2[i].value_node)) {
                    switch (ITEMS_DATOS_TOKENIZE_2[i].value_node) {
                        case "GD-MR-16":
                            VALOR_CONSULTA_RADIC = "PRODUCCION|";
                            URL_NODE = ITEMS_DATOS_TOKENIZE_2[i].url_node;
                            ESTADO_RESULTADO = "YES";
                            document.getElementById("content_iframe_ds").style.display = "flex";
                            document.getElementById("id_selecion_opcion_trea").style.display = "flex";
                            document.getElementById("card_general_ini_text").style.display = "none";
                            pre_interval();
                            break;
                        //Caso inicializa expediente
                        case "GD-CE-03":
                            web_service_inicializa_gestion_expediente(ITEMS_DATOS_TOKENIZE_2[i]);
                            break;
                        default:
                            if (ITEMS_DATOS_TOKENIZE_2[i].nodo_plantilla_radicado == "yes") {
                                VALOR_CONSULTA_RADIC = "RADICACION|" + ITEMS_DATOS_TOKENIZE_2[i].id_plantilla + "|" + ITEMS_DATOS_TOKENIZE_2[i].tipo_plantilla + "|0|" + ITEMS_DATOS_TOKENIZE_2[i].Text_node + "|" + ITEMS_DATOS_TOKENIZE_2[i].tipo_plantilla;
                                URL_NODE = ITEMS_DATOS_TOKENIZE_2[i].url_node;
                                document.getElementById("id_selecion_opcion_trea").textContent = ITEMS_DATOS_TOKENIZE_2[i].Text_node.toUpperCase();
                                ESTADO_RESULTADO = "YES";
                                document.getElementById("content_iframe_ds").style.display = "flex";
                                document.getElementById("id_selecion_opcion_trea").style.display = "flex";
                                document.getElementById("card_general_ini_text").style.display = "none";
                                pre_interval();
                                break;
                            }
                            if (ITEMS_DATOS_TOKENIZE_2[i].nodo_plantilla_radicado == "consulta") {
                                VALOR_CONSULTA_RADIC = "CONSULTA|" + ITEMS_DATOS_TOKENIZE_2[i].id_plantilla + "|" + ITEMS_DATOS_TOKENIZE_2[i].tipo_plantilla + "|0|" + ITEMS_DATOS_TOKENIZE_2[i].Text_node + "|" + ITEMS_DATOS_TOKENIZE_2[i].tipo_plantilla;
                                URL_NODE = ITEMS_DATOS_TOKENIZE_2[i].url_node;
                                document.getElementById("id_selecion_opcion_trea").textContent = ITEMS_DATOS_TOKENIZE_2[i].Text_node.toUpperCase();
                                ESTADO_RESULTADO = "YES";
                                document.getElementById("content_iframe_ds").style.display = "flex";
                                document.getElementById("id_selecion_opcion_trea").style.display = "flex";
                                document.getElementById("card_general_ini_text").style.display = "none";
                                pre_interval();
                                break;
                            }
                            if (ITEMS_DATOS_TOKENIZE_2[i].nodo_plantilla_radicado != "yes" || ITEMS_DATOS_TOKENIZE_2[i].nodo_plantilla_radicado != "consulta") {
                                web_service_inicializa_gestion_expediente();
                                document.getElementById("id_selecion_opcion_trea").textContent = ITEMS_DATOS_TOKENIZE_2[i].Text_node.toUpperCase();
                                document.getElementById("content_iframe_ds").style.display = "flex";
                                document.getElementById("id_selecion_opcion_trea").style.display = "flex";
                                document.getElementById("card_general_ini_text").style.display = "none";
                                loading_iframe('ContentPlacenter_ifrm_ds_', ITEMS_DATOS_TOKENIZE_2[i].url_node);
                                return true;
                            }

                    }


                }
            } 
        }
       
    } catch (ex) {
        alert("Funcion event_menu_prinicipal : " + ex.message)
    } finally { auto_zise_popup_inicio_docuarchi_inicio(); }
}
function even_diplay_ini() {
    document.getElementById("content_iframe_ds").style.display = "none";
    document.getElementById("id_selecion_opcion_trea").style.display = "none";
    document.getElementById("card_general_ini_text").style.display = "flex";
    display_unload();
}
function auto_zise_popup_inicio_docuarchi_inicio() {
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
        //$('#Panel_agregar_expediente_carpeta').css("left", (Math.round(widtth_procent_left_rigth)) + "px");  id_scoop_item
        var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento card_general_ini_text
        $('#ContentPlacenter_ifrm_ds_').css("height", (heig_porcent - document.getElementById("header_coop").clientHeight) + "px");
        $('#card_general_ini_text').css("height", (espacio_iframe - 60) + "px");
       
       
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_inicio_docuarchi_inicio " + err.message);
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
