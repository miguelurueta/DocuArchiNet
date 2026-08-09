$(document).ready(function () { 
    $.fn.inicio = function (ident) {  
    auto_zise_popup_lista_tramites(1, 1);
    auto_zise_popup_respuesta();   
    auto_zise_popup_detalle_respuesta();
    auto_zise_popup_detalle_transacciones();
    auto_zise_popup_detalle_trazabilidad();
    auto_zise_popup_visor_externo();
    auto_zise_popup_solicitud_aprobacion();
    auto_zise_popup_historico_tramite();
    service_posibles_datos_tramites_();
    auto_zise_popup_detalle_radicado();
    auto_size_content_anotacion();
        auto_zise_popup_lista_imagenes_gestion();
        auto_zise_popup_impresion();
        auto_zise_popup_guardar_documento();
        auto_zise_popup_consulta_meta_dato();
        auto_zise_popup_adjunta_documento_workflow();
        auto_zise_popup_adjunta_documento_respuesta();
        auto_zise_popup_adjunta_anexo_respuesta();
        asigna_datos_heig_with();
        auto_zise_popup_list_gestion_solicitud();
    $('#data_grid_listado_solicitudes tr[id]').on("click", function () {
        $('#data_grid_listado_solicitudes tr[id]').css({ "background": "White" });
        $(this).css({ "background-color": "#e8e8f7" });
        var fer = $(this).attr("id");
        $('#hdnEmailID').val(fer);
    });
    $('#data_grid_listado_solicitudes tr[id]').on("dblclick", function () {
        var fer = $(this).attr("id");
        $('#hdnEmailID').val(fer);
        document.getElementById("Hidden_tipo_visor").value = "VISOR WORKFLOW";
        document.getElementById("Button_visor_emergente").click();
        return false;
    });
        $('#GridView_list_documento_relacion_wf tr[idd_wf] a[idd_wf]').click(function () {
            try {
                var attrib_tip_event = $(this).attr("tip_event");
                if (attrib_tip_event == "vis_doc_selecion_wf") {
                    $('#GridView_list_documento_relacion_wf tr[idd_wf]').css({ "background": "White", "color": "Black" });
                    $(this).parent().parent().parent().css({ "background-color": "#e8e8f7", "color": "Black" });
                    var tag_split = $(this).attr("idd_wf").split("|");
                    Set_documento_seleccionado(tag_split[1], tag_split[0]);
                }
               
               
            }
            catch (err) {
                alert(err.message + " Funcion clik");
            }
        });
        $('#GridView_list_documento_relacion_wf tr[id_wf]').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });
    }
    $("#noaming").bind("contextmenu", function (e) {
        e.preventDefault();
    });
    var left;
    var top;
    $("#draggable").css({ opacity: 0.5 });
    left = $("#draggable").position.left;
    top = $("#draggable").position.top;
    var heigimage, withimage;
    var top_ = $("#zona").position.top + $("#zona").height();
    var bottom_ = $("#zona").position.top - $("#zona").height();
    $("#draggable").draggable({
        containment: $("#noaming"),
        stop: function (event, ui) {
            var elemento = $("#draggable");
            var posicion = elemento.position();
            left = posicion.left;
            top = posicion.top;
            var dragab = $("#draggable");
            var contenido = $("#content");
            var scr = contenido.scrollTop();
            var scrolleft = contenido.scrollLeft();
            left = (scrolleft) + left;
            var posicfinal = (top + scr) - 10;
            $("#Hiddenintercambio").val(top + "-" + left + "-" + dragab.height() + "-" + dragab.width() + "-" + dragab.height() + "-" + scr + "-" + posicfinal + "-" + heigimage + "-" + withimage);
        }
    }
    );

    $("#draggable").resizable({
        maxHeight: 80, maxWidth: 100, minWidth: 50, minHeight: 50,
        start: function (event, ui) {
            //$("#draggable").offset({ top: top, left: left });
        },
        stop: function (event, ui) {
            var conta = $("#draggable");
            $("#draggable").css('position', 'relative');
            var dragab = $("#draggable");
            var contenido = $("#content");
            var scr = contenido.scrollTop();
            var scrolleft = contenido.scrollLeft();
            left = left - scrolleft;
            var posicfinal = (top + scr) - 10;
            //$("#draggable").offset({ top: top, left: left });
            $("#Hiddenintercambio").val(top + "-" + left + "-" + dragab.height() + "-" + dragab.width() + "-" + dragab.height() + "-" + scr + "-" + posicfinal);

        },
        resize: function (event, ui) {
            $("#img").imageResize();
            var contenido = $("#content");
            var scroltop = contenido.scrollTop();
            var scroleft = contenido.scrollLeft();
            $("#draggable").offset({ top: top, left: left - scroleft });
        }
    }
    );

    $('#draggable').contextMenu('context-menu-1', {
        'Guardar': {
            click: function (element) {
                document.getElementById('ImageButtonguardar').click();
                $("#draggable").css("display", "none");
            }
        },
        'Limpiar': {
            click: function (element) {  // element is the jquery obj clicked on when context menu launched

                $("#draggable").css("display", "none");
            }
        },
        'Cancelar': {
            click: function (element) {  // element is the jquery obj clicked on when context menu launched

                //$(element).css("display", "none");
            }
        }
    }


    );
});

$(window).on("load", function () {
    
    $('#data_grid_listado_solicitudes tr[id]').on("mouseover", function () {
        $(this).css({ cursor: "hand", cursor: "pointer" });
    });
   
    var elment = document.getElementsByClassName("da_event_captive");
    if (elment) {
        for (var i = 0; i < elment.length; i++) {
            elment[i].addEventListener("click", event_click, false);
        }
    }
    let array_element = new Array;
    array_element.push(
        { id: "Button_actualizar_nota" }, { id: "Button_Show_Guardar" }, { id: "Button_duardar_nota" }
    );
    for (let i = 0; i < array_element.length; i++) {
        let elment_a_document_production = document.getElementById(array_element[i].id);
        if (elment_a_document_production) {
            elment_a_document_production.addEventListener("click", handler_element_event, false);
        }
    }
    window.addEventListener("resize", rezize_event);
    ShowModalPopup("ModalPopupExtender_mensaje_personalizado_backgroundElement", "Panel_mensaje_personalizado", 1000040);
    ShowModalPopup("ModalPopupExtender_guardar_backgroundElement", "Panel_guardar", 100007);
    ShowModalPopup("ModalPopupExtenderimpre_post_backgroundElement", "Panelimpresionpost", 100006);
    ShowModalPopup("ModalPopupExtender_edition_actualiza_tipologia_documental_workflow_backgroundElement", "Panel_actualiza_tipologia_documental_workflow", 100009);
    ShowModalPopup("ModalPopupExtender_sube_documento_adjunto_backgroundElement", "Panel_sube_documento_adjunto", 100005); 
    ShowModalPopup("ModalPopupExtender_transacciones_backgroundElement", "Panel_transacciones", 100001);
    ShowModalPopup("ModalPopupExtender_trazabilidad_backgroundElement", "Panel_trazabilidad", 100001);
    ShowModalPopup("ModalPopupExtender_edition_reasigna_responsable_tramite_backgroundElement", "Panel_reasigna_responsable_tramite", 100001);
    ShowModalPopup("ModalPopupExtender_edition_confirma_reversa_respuesta_backgroundElement", "Panel_confirma_reversa_respuesta", 100001);
    ShowModalPopup("ModalPopupExtender_edition_reasigna_tramite_usuario_backgroundElement", "Panel_reasigna_tramite_usuario", 100001);
    ShowModalPopup("ModalPopupExtender_edition_confirma_respuesta_backgroundElement", "Panel_confirma_respuesta", 100001);
    ShowModalPopup("ModalPopupExtender_edition_autoriza_reasignacion_tarea_backgroundElement", "Panel_autoriza_reasignacion_tarea", 100001);
    ShowModalPopup("ModalPopup_respuesta_radicado_backgroundElement", "Panel_respuesta_radicado", 100001);
    ShowModalPopup("ModalPopupExtender_solicitud_aprobacion_backgroundElement", "Panel_solicitud_aprobacion", 100001);
    ShowModalPopup("ModalPopupExtender_detalle_respuesta_backgroundElement", "Panel_detalle_respuesta", 100001);
    ShowModalPopup("ModalPopupExtender_edition_nota_respuesta_backgroundElement", "Panel_nota_respuesta", 100001);
    ShowModalPopup("ModalPopupExtender_edition_lista_imagenes_gestion_backgroundElement", "Panel_lista_imagenes_gestion", 100001);
    ShowModalPopup("ModalPopupExtender_edition_interface_regitra_meta_dato_backgroundElement", "Panel_interface_regitra_meta_dato", 100002);
    ShowModalPopup("ModalPopupExtender_edition_interface_consulta_meta_dato_backgroundElement", "Panel_interface_consulta_meta_dato", 1000009);
    ShowModalPopup("ModalPopupExtender_edition_descarga_plantilla_radicada_backgroundElement", "Panel_descarga_plantilla_radicada", 1000010);
    ShowModalPopup("ModalPopupExtender_descarga_formato_backgroundElement", "Panel_descarga_formato", 1000010);
    ShowModalPopup("ModalPopupExtender_edition_sube_documento_respuesta_backgroundElement", "Panel_sube_documento_respuesta", 100005);
    ShowModalPopup("ModalPopupExtender_edition_sube_anexo_respuesta_backgroundElement", "Panel_sube_anexo_respuesta", 100005);
    ShowModalPopup("ModalPopupExtender_edition_radica_documento_respuesta_backgroundElement", "Panel_radica_documento_respuesta", 100005);
    ShowModalPopup("ModalPopupExtender_edition_confirma_envio_respuesta_backgroundElement", "Panel_confirma_envio_respuesta", 100005);
    ShowModalPopup("ModalPopupExtender_edition_reversa_respuesta_backgroundElement", "Panel_reversa_respuesta", 100005);
    ShowModalPopup("ModalPopupExtender_edition_confirma_reversa_respuesta_backgroundElement", "Panel_confirma_reversa_respuesta", 100005);
    ShowModalPopup("ModalPopupExtender_edition_asigna_dest_externo_backgroundElement", "Panel_asigna_dest_externo", 100005);
    ShowModalPopup("ModalPopupExtender_valiacion_plantilla_backgroundElement", "Panel_valiacion_plantilla", 100006);
    ShowModalPopup("ModalPopupExtender_edition_notifica_correo_respuesta_backgroundElement", "Panel_notifica_correo_respuesta", 100005);
    ShowModalPopup("ModalPopupExtender_actualizacion_anualidad_backgroundElement", "Panel_actualizacion_anualidad", 100005);
    ShowModalPopup("ModalPopupExtender_solicitud_aprobacion_backgroundElement", "Panel_solicitud_aprobacion", 100005);
    ShowModalPopup("ModalPopupExtender_detalle_respuesta_backgroundElement", "Panel_detalle_respuesta", 100005);
    ShowModalPopup("ModalPopupExtender_transacciones_backgroundElement", "Panel_transacciones", 100005);
    ShowModalPopup("ModalPopupExtender_opcion_descarga_respuesta_backgroundElement", "Panel_opcion_descarga_respuesta", 100005);
    ShowModalPopup("ModalPopupExtenderimpre_backgroundElement", "Panelimpresion", 100005);
    ShowModalPopup("ModalPopupExtender_edition_redirecciona_entidad_externa_backgroundElement", "Panel_redirecciona_entidad_externa", 100006);
    ShowModalPopup("ModalPopupExtender_edition_gestion_respuesta_solicitud_backgroundElement", "Panel_gestion_respuesta_solicitud", 100006);
    ShowModalPopup("ModalPopupExtender_edition_list_gestion_solicitud_backgroundElement", "Panel_list_gestion_solicitud", 100006);
    ShowModalPopup("ModalPopupExtender_edition_editar_gestion_solicitud_backgroundElement", "Panel_editar_gestion_solicitud", 100007);
    //inicializar token reasigna tramite
    inicia_token_usuario_wf('tokenize-callable-demo_respuesta');
    $('.tokenize-callable-demo_respuesta').on('tokenize:tokens:added', function (e, value, text) {
        ITEMS_DATOS_TOKENIZE_2.push({ text: text, value: value });
    });
    //agrega el correo al cahe tokenize
    if (COORREO_INI !== "") {
        add_regitro_correo_service(COORREO_INI);
    }
    
   
    //agrega evento para eliminar token para reasignacion
    set_tokenize_delete_event('tokenize-callable-demo_respuesta');
    
    //RESPONDER LA SOLICITUD
    toke_ini('tokenize-callable-demo_respuesta_');
    set_tokenize_add_event_valid('tokenize-callable-demo_respuesta_');
    //agrega evento para eliminar token
    set_tokenize_delete_event('tokenize-callable-demo_respuesta_');

    //Inicializar el token de archivar tramite
    toke_ini('tokenize-callable-demo_respuesta_k');
    set_tokenize_add_event_valid('tokenize-callable-demo_respuesta_k');
    //agrega evento para eliminar token
    set_tokenize_delete_event('tokenize-callable-demo_respuesta_k');

    //agrega evento para agregar nuevos token personalizados
    toke_ini('tokenize-callable-demo_respuesta_simple');
    set_tokenize_add_event_valid('tokenize-callable-demo_respuesta_simple');
    //agrega evento para eliminar token  
    set_tokenize_delete_event('tokenize-callable-demo_respuesta_simple'); 
    
    
    
    //Inicializar el token renviar notificacion
    toke_ini('tokenize-callable-demo_respuesta__');
    //agrega evento para agregar nuevos token personalizados
    set_tokenize_add_event_valid('tokenize-callable-demo_respuesta__');
    //agrega evento para eliminar token
    set_tokenize_delete_event('tokenize-callable-demo_respuesta__');

    
    toke_ini_solicitud('tokenize-callable-demo_respuesta___');
    $('.tokenize-callable-demo_respuesta___').on('tokenize:tokens:added', function (e, value, text) {
        ITEMS_DATOS_TOKENIZE_5.push({ text: text, value: value });
    });
    $('.tokenize-callable-demo_respuesta___').on('tokenize:tokens:remove', function (e, value) {
        delete_array_tokenize_solicitud(value);
    });
});
function event_click(e) {
    try {
        var nombre_buton = e.currentTarget.value;
        var element_ = document.getElementById(nombre_buton);  
        element_.click();    
    }
    catch (err) {
        alert(err.message + " Funcion event_click");
    }
}

function rezize_event() {
    try {
        auto_zise_popup_lista_tramites(1, 1);
        auto_zise_popup_respuesta();
        auto_zise_popup_detalle_respuesta();
        auto_zise_popup_detalle_transacciones();
        auto_zise_popup_detalle_trazabilidad();
        auto_zise_popup_visor_externo();
        auto_zise_popup_solicitud_aprobacion();
        auto_zise_popup_historico_tramite();
        auto_zise_popup_detalle_radicado();
        auto_size_content_anotacion();
        auto_zise_popup_lista_imagenes_gestion();
        auto_zise_popup_impresion();
        auto_zise_popup_guardar_documento();
        auto_zise_popup_consulta_meta_dato();
        auto_zise_popup_adjunta_documento_workflow();
        auto_zise_popup_adjunta_documento_respuesta();
        auto_zise_popup_adjunta_anexo_respuesta();
        asigna_datos_heig_with();
        auto_zise_popup_list_gestion_solicitud();
    } catch (ex) {
        alert(ex.message + " Función rezize_event")
    }
}
var COORREO_INI = "";
var CORREO_GESTION = "";
var ITEMS_DATOS_TOKENIZE_2 = new Array();  //GUARDA LOS ITEM SELECIONANDO EN TEX SELECTOR
var ITEMS_DATOS_TOKENIZE_3 = new Array();  //GUARDA LOS ITEM SELECIONANDO CORREO
var ITEMS_DATOS_TOKENIZE_4 = new Array();  //GUARDA LOS ITEM SELECIONANDO EN TEX SELECTOR
var ITEMS_DATOS_TOKENIZE_5 = new Array();  //GUARDA LOS ITEM SELECIONANDO EN TEX SELECTOR
var ITEMS_DATOS_TOKENIZE_6 = new Array();  //GUARDA LOS ITEM SELECIONANDO EN TEX SELECTOR
var ITEMS_DATOS_TOKENIZE_8 = new Array();  //GUARDA LOS ITEM SELECIONANDO EN TEX SELECTOR
var ITEMS_DATOS_LISTA_RAMITE = new Array(); //GUARDA LOS ITEM DEL LOS CAMPOS DE LISTA DE TRAMITE
let TIPO_ENVIO_RESPUESTA = 0; //GUARDA EL TIPO ENVIO DEL TRAMITE
var WF_ESTATUS_SERVICE;
var ESTADO_INICIALIZACION = -4;
var ESTADO_EVENT_GENERAL = "";
var INTERVAL_EVENT_GENERAL;
var FILE_ARCHIVO_DONWLOAD;
//CONTROLA LOS EVENTOS PARA FUNCINES AJAX
function event_element_clic(event, e) {
    try {
        ESTADO_EVENT_GENERAL = "intro";
        posicion_update_pogres('progres_bar');
        e.disabled = true;
        INTERVAL_EVENT_GENERAL = setInterval(fx_funcion, 50);
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
                if (e.id == "id_indice_wf_pdf") {
                    Service_Solicita_listar_meta_datos_Archivo(ID_IMAGEN_VIS_WF, GABIENTE_VIS_WF);
                    return true;
                }
                if (e.id == "id_indice_wf_pdf_draw") {
                    Service_Solicita_listar_meta_datos_Archivo(ID_IMAGEN_VIS_WF, GABIENTE_VIS_WF);
                    return true;
                }
                if (e.id == "a_lement_actualiza_index") {
                    service_actualiza_indice_workflow(1);
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
        ESTADO_EVENT_GENERAL = "intro";
        posicion_update_pogres('progres_bar');
        INTERVAL_EVENT_GENERAL = setInterval(fx_funcion, 50);
        function fx_funcion() {
            //--Sale del evento
            if (ESTADO_EVENT_GENERAL == "out") {
                progres_hiden('progres_bar');
                clearInterval(INTERVAL_EVENT_GENERAL);
                ESTADO_EVENT_GENERAL = "";
            }
            //--Entra al evento
            if (ESTADO_EVENT_GENERAL == "intro") {
                ESTADO_EVENT_GENERAL = "";
                //--Crea meta dato y firma documento
                if (evento == "firma_doc_selecion_wf") {
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
                //Reasigna tramite
                if (evento == "R-ASIG-LISTA") {     
                    Service_reasignar_tramite(document.getElementById("hdnEmailID").value,tip_event);
                    return true;
                }
                //finaliza tramite y elimina de la lista
                if (evento == "finaliza_solic_tramite") {
                    Service_finalizar_tramite(tip_event);
                    return true;
                }
                if (evento == "documento_solic_tramite") {
                    Service_solicita_valor_nombre_campo_radicado_beneficiario(tip_event);
                    return true;
                }
                //Activa ventana renviar notificacion tramite
                if (evento == "A-RENVIA-COR_TR") {
                    Service_start_notifica_correo(tip_event);
                    return true;
                }
                //Notifica gestión correo electronico
                if (evento == "N-R-C-E") {
                    event_notifica_gestion_correo(tip_event);
                    return true;
                }
                //Activa ventana archivar tramite  
                if (evento == "archiva_solic_tramite") {
                    Service_start_archiva_tramite(tip_event);
                    return true;
                }
                //Archiva tramite
                if (evento == "A-AH-GES") {      
                    Service_archiva_tramite_solicitud(document.getElementById("Hidden_token_tokenize_k").value, tip_event);
                    return true;
                }
                if (evento == "C-DW-LISTA") {
                    inicializa_upload_file_client(tip_event);
                    document.getElementById("Hidden_tip_adjunt").value = "wf";
                    parameter_upload(ESTADO_EVENT_GENERAL, "CORRESPO", "Button_tool_activa_sube_documento_lista", "multiple", tip_event);
                    return true;
                }
                //sube documento visor
                if (evento == "C-DW-VIS") {
                    inicializa_upload_file_client(tip_event);
                    document.getElementById("Hidden_tip_adjunt").value = "wf";
                    parameter_upload(ESTADO_EVENT_GENERAL, "CORRESPO", "Button_tool_activa_sube_documento", "", tip_event);
                    return true;
                }
                //Sube documento respuesta
                if (evento == "S-D-R") {
                    inicializa_upload_file_client(tip_event);
                    document.getElementById("Hidden_tip_adjunt").value = "";
                    parameter_upload(ESTADO_EVENT_GENERAL, "CORRESPO", "Button_carga_plantilla", "", tip_event);
                    return true;
                }
                //Sube documento anexo respuesta
                if (evento == "S-D-A") {
                    inicializa_upload_file_client(tip_event);
                    document.getElementById("Hidden_tip_adjunt").value = "";
                    parameter_upload(ESTADO_EVENT_GENERAL, "CORRESPO", "Button_anexo_cargar", "multiple", tip_event);
                    return true;
                }
               
                //Activa interface respuesta
                if (evento == "responder_solic_tramite") {      
                    Service_start_inicio_gestio_correspondencia(tip_event);
                    lista_tipos_respuesta_simple();
                    return true;
                }
                //Elimina documento respuesta
                if (evento == "E-D-R") {
                    Service_elimina_documento_respuesta(document.getElementById("Hidden_id_respuesta").value);
                    return true;
                }
                //Elimina anexo respuesta
                if (evento == "E-A-R") {
                    Service_elimina_anexo_respuesta(document.getElementById("Hidden_id_respuesta").value, tip_event);
                    return true;
                }
                //Elimina anexo respuesta  simple
                if (evento == "E-A-R-S") {
                    Service_elimina_anexo_respuesta(document.getElementById("Hidden_id_respuesta").value, tip_event);
                    return true;
                }
                //Descarga anexo simple y formal
                if (evento == "D-A-R-S" || evento == "D-A-R-F") {
                    Service_url_anexo_respuesta(tip_event, FILE_ARCHIVO_DONWLOAD);
                    return true;
                }
                //Respuesta formal tramite 
                if (evento == "R-R-R-F") {
                    event_responder_solicitud(tip_event);
                    return true;
                }
                //Respuesta simple tramite 
                if (evento == "R-R-R-S") {
                    event_cofirmar_solicitud(tip_event);
                    return true;
                }
                //Reversa gestion tramite con autorización 
                if (evento == "E-R-R-A") {
                    var login_usuario_ = document.getElementById("TextBox_login_usuario_val").value;
                    var pasword_usuario_ = document.getElementById("TextBox_pasw_usuario_val").value;
                    Service_Reversa_gestion_tramite_usuario_autorizado(tip_event, login_usuario_, pasword_usuario_,1);
                    return true;
                }
                if (evento == "E-R-R-C") {
                    var login_usuario_ = "";
                    var pasword_usuario_ = "-1";
                    Service_Reversa_gestion_tramite_usuario_autorizado(tip_event, login_usuario_, pasword_usuario_, 0);
                    return true;
                }
                
                
                //Activa solicitudes de aprobación
                if (evento == "A-P-S-A") {
                    Service_activa_soicitud_aprobacion(document.getElementById("Hidden_id_respuesta").value);
                    return true;
                }
                //Registra solicitud de aprobación
                if (evento == "R-S-A-D") {
                    Set_Registra_solicitud_aprobacion(document.getElementById("Hidden_id_respuesta").value);
                    return true;
                }
                //Actualiza estado solicitud de aprobación listado
                if (evento == "A-E-S-A") {
                    Service_solicita_estado_tramite_tarea_workflow(tip_event);
                    return true;
                }
                //Descarga documento respuesta
                if (evento == "D-D-R-S") {
                    var formato_ = document.getElementById("DropDownList_tipo_archivo").value;
                    var estado_firma_ = 0;
                    if (document.getElementById("CheckBox_opcion_descarga_respuesta_con_firma").checked == true) {
                        estado_firma_ = 1;
                    }
                    Service_url_documento_respuesta(tip_event, formato_, estado_firma_);
                    return true;
                }
                //Traslada petición a entidad externa
                if (evento == "E-ENTIDAD-EXTERNA") {
                    var mensaje = confirm("Desea confirmar el traslado de la solicitud");
                    //Detectamos si el usuario acepto el mensaje
                    if (mensaje) {
                        Service_redirecciona_solicitud_a_entidades();
                        return true;
                    }
                   
                }
                //Registra gestión solicitud
                if (evento == "R-GESTION-SOLICITUD") {
                    Service_registra_gestion_respuesta();
                    return true;
                }
                //Elimina registro gestion solicitud
                if (evento == "d_g_r_s") {
                    var mensaje = confirm("Desea eliminar el registro de gestión");  
                    if (mensaje) {
                        Service_elimina_registro_gestion_respuesta(tip_event);
                        return true;
                    }
                    
                }
                //Activa la lista de la descripcion del registro
                if (evento == "e_g_r_s") {
                    Service_solicita_descripcion_gestion_respuesta(tip_event);
                    return true;
                }
                //Edita el registro de gestion del tramite
                if (evento == "e_e_g_r_s") {
                    Service_Actualiza_datos_gestion_solicitud(tip_event);
                    return true;
                }
                //Actualiza nota
                if (evento == "Button_actualizar_nota") {
                    Service_actualiza_nota_tarea_workflow(document.getElementById("hdnidlista").value, document.getElementById("TextBox_nota").value);
                    return true;
                }
                //guarda nota
                if (evento == "Button_duardar_nota") {
                    Service_add_nota_tarea_workflow(document.getElementById("TextBox_nota").value);
                    return true;
                }
                //elimina nota
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
//EVENTOS NOTA 
const handler_element_event = (e) => {
    try {
        let name_ID = e.currentTarget.id;
        let result = "";
        switch (name_ID) {
            case "Button_Show_Guardar":
                result = Event_note_workflow("", "", "Button_Show_Guardar");
                if (result !== "YES") {
                    alert(result);
                }
                break;
            case "Button_actualizar_nota":
                result = Event_note_workflow(document.getElementById("hdnidlista").value, document.getElementById("TextBox_nota").value, "Button_actualizar_nota");
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
//ActIve venata add new note task workflow
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

//Funcion : Captura los evento de los botonos de los greview
function prevent_lista_gestion(event, element) {
    try {
        var fer = $(element).attr("id");
        var tip_event = $(element).attr("tip_event");
        if (tip_event == "d_g_r_s") {      
            event_element_menu("d_g_r_s", fer);
        }
        if (tip_event == "e_g_r_s") {
            event_element_menu("e_g_r_s", fer);
        }
        var atri_id = $(element).attr("idd");
        if (tip_event == "e_e_g_r_s") {
            event_element_menu("e_e_g_r_s", atri_id);
        }
        element.focus();
    }
    catch (err) {
        alert(err.message + " Funcion prevent_lista_gestion");
    }
}
function prevent(event, element) {
    try {
        var fer = $(element).attr("idd");
        var tip_event = $(element).attr("tip_event");
        if (tip_event == "documento_solic_tramite") {
            $('#hdnEmailID').val(fer);
            event_element_menu("documento_solic_tramite", fer);
        }
        if (tip_event == "lista_solic_tramite") {
            $('#hdnEmailID').val(fer);
            document.getElementById("Button_activa_lista_solicitudes_aprobacion").click();
        }
        if (tip_event == "responder_solic_tramite") {
            $('#hdnEmailID').val(fer);
            event_element_menu("responder_solic_tramite", fer);
         }
        //Activa reasignar el tramite a otro usuario 
        if (tip_event == "reasignar_solic_tramite") {
            $('#hdnEmailID').val(fer);  
            if (document.getElementById("hdnEmailID").value == "" || document.getElementById("hdnEmailID").value == "-1") {
                alert("Debe seleccionar el registro del tramite hacer la reasignación");
            } else {   
                $find("ModalPopupExtender_edition_reasigna_tramite_usuario").show();
                $('.tokenize-callable-demo_respuesta').tokenize2().trigger('tokenize:clear');
                document.getElementById("Hidden_token_tokenize2").value = "-1";
            }
        }
        //Activa archivar tramite
        if (tip_event == "archiva_solic_tramite") {
            $('#hdnEmailID').val(fer);
            if (document.getElementById("hdnEmailID").value == "" || document.getElementById("hdnEmailID").value == "-1") {
                alert("Debe seleccionar el registro del tramite para archivar");
            } else {
                event_element_menu("archiva_solic_tramite", fer);
            }
            
        }
        if (tip_event == "finaliza_solic_tramite") {
            $('#hdnEmailID').val(fer);
            var r = confirm('Desea finalizar el tramite');
            if (r == true) {
                event_element_menu('finaliza_solic_tramite', fer);
            }
           
        }
        if (tip_event == "nota_solic_tramite") {
            $('#hdnEmailID').val(fer);
            document.getElementById("ImageButtonanotacion").click();
           
        }
        //Evento firmar documento
        if (tip_event == "firma_doc_selecion_wf") {
            var ref_id = $(element).attr("idd_wf") + "|" + element.id;
            if (ref_id != "") {
                var spliter = ref_id.split("|");
                if (spliter.length > 3) {
                    event_element_menu(tip_event, ref_id);
                } else {
                    alert("Inconsistencia en el evento, spliter incompleto (" + spliter.length + ")");
                }
            }
        }
        //Visualiza documento wf
        if (tip_event == "vis_doc_selecion_wf") {
            var ref_idd = $(element).attr("idd_wf");
            var ref_id = $(element).attr("id_wf");
            $('#hiden_seleccion_documento_wf').val(ref_idd);
            $('#hiden_seleccion_documento_id_wf').val(ref_id);
            var spliter;
            var text_content = document.getElementById("hiden_seleccion_documento_wf").value;
            if (text_content != "") {
                spliter = text_content.split("|");
                if (spliter.length > 3) {
                    ID_IMAGEN_VIS_WF = spliter[1];
                    GABIENTE_VIS_WF = spliter[0];
                }
            }
            var valor_documento = "";
            valor_documento = busca_campo_wf_seleccion("GridView_list_documento_relacion_wf", ref_id, "DOCUMENTO");
            $('#GridView_list_documento_relacion_wf tr[idd_wf]').css({ "background": "White", "color": "Black" });
            $('#GridView_list_documento_relacion_wf tr[id_wf=' + ref_id + ']').css({ "background-color": "#e8e8f7", "color": "Black" });
            if (valor_documento != "") {
                valor_documento = reemplazarAcentos(valor_documento);
            }
            document.getElementById("Label_estado_selecion").innerHTML = valor_documento;
            document.getElementById("Button_selecion_treview_documento").click();
        }
        //cambia tipologia documento enlzado a flujo
        if (tip_event == "cambia_doc_selecion_wf") {
            var ref_idd = $(element).attr("idd_wf");
            var ref_id = $(element).attr("id_wf");
            $('#Hidden_selccion_documento_cambia_tipo_split_wf').val(ref_idd);
            $('#Hidden_selccion_documento_cambia_tipo_wf').val(ref_id);
            document.getElementById("Button_clasficar_documento").click();

        }
        //Elimina documento lista doumentos workflow
        if (tip_event == "elim_doc_selecion_wf") {
            var ref_idd = $(element).attr("idd_wf");
            var ref_id = $(element).attr("id_wf");
            $('#Hidden_selccion_documento_eliminar_split_wf').val(ref_idd);
            $('#Hidden_selccion_documento_eliminar_wf').val(ref_id);
            var value;
            var valor_documento = "";
            valor_documento = busca_campo_wf_seleccion("GridView_list_documento_relacion_wf", ref_id, "DOCUMENTO");
            if (valor_documento != "") {
                value = reemplazarAcentos(valor_documento);
            }

            confirma_eliminar_documento_relacion("Desea eliminar el documento ?", "Hidden_selccion_documento_eliminar_split_wf");
            if (document.getElementById("HiddenPROMP").value == 0) {
                document.getElementById("Button_eliminar_documento").click();
            }

        }
        //event.preventDefault();
        element.focus();
    }
    catch (err) {
        alert(err.message + " Funcion prevent");
    }
}
//ZONA LISTA DOCUMENTOS GESTION
function Service_solicita_valor_nombre_campo_radicado_beneficiario(id_tarea_) {
    try {
       
        $.ajax('../webservice/WebServiceWorkflow.asmx/Service_solicita_valor_nombre_campo_radicado_beneficiario', {
            data: "{'id_tarea':" + "'" + id_tarea_  + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].ERROR_SERVICE !== "YES") {
                    alert(data.d[0].ERROR_SERVICE);
                    ESTADO_EVENT_GENERAL = "out";
                } else {
                    ITEMS_DATOS_LISTA_RAMITE = new Array();
                    $.each(data.d, function (k, v) {
                        ITEMS_DATOS_LISTA_RAMITE.push(v);
                    });
                    inicia_set_lista_imagenes();
                    document.getElementById("Button_activa_lista_imagenes_gestion_corresponencia").click();
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
        alert('Service_Solicita_listar_meta_datos_Archivo  ' + ex.message);
    }
}
function inicia_set_lista_imagenes() {
    try {
        if (document.getElementById("div_buton")) {
            document.getElementById("div_buton").style.display = "None";
        }
        if (document.getElementById("Panel_indice")) {
            document.getElementById("Panel_indice").style.display = "None";
        }
        if (document.getElementById("panel_content_iframe")) {
            document.getElementById("panel_content_iframe").style.display = "None";
        }
        if (document.getElementById("panel_content_image_draw")) {
            document.getElementById("panel_content_image_draw").style.display = "None";
        }
        document.getElementById("Label_estado_selecion").innerHTML = "";   
        document.getElementById("Label_estado_tarea_selecion").innerHTML ="Radicado : " + ITEMS_DATOS_LISTA_RAMITE[0].valor_campo_ruta + "     Beneficiario : " + ITEMS_DATOS_LISTA_RAMITE[1].valor_campo_ruta;
        document.getElementById("Label_lista_imagenes_gestion").innerHTML = "Tramite : " + ITEMS_DATOS_LISTA_RAMITE[2].valor_campo_ruta;
       
    } catch (err) {
        alert(err.message + " Funcion inicia_set_lista_imagenes");
    }
}
function confirma_eliminar_documento_relacion(mensaje, hiden_sel) {
    try {
        if (document.getElementById(hiden_sel).value == "") {
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
                //event.preventDefault();
            }
        });
    }
    catch (err) {
        alert(err.message + " Funcion Set_documento_seleccionado");
    }
}

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
            numero_documento_relacionado = document.getElementById("GridView_list_documento_relacion_wf").rows.length - 1;
            star_copy_interval_file_Upload(estado_adjunto, estado_relacion, id_tipo_docuental, funcion_name, element_parent, evento_adjunta,
                numero_documento_relacionado, element_html_actuliza, element_update_panel, id_respuesta, tipo_adjunta, element_isert_table, "", "", 0);
        }
        if (CONTEN_NAME_UPLOAD_FILE == "adjunto_doc_respuesta") {
            var chek_adjunto = document.getElementById("Check_adjunta_formato");
            if (chek_adjunto) {
                if (chek_adjunto.checked == true) {
                    
                    tipo_adjunta = 1;
                    funcion_name = "actualiza_semaforo_respuesta";
                    evento_adjunta = "SUBE_RESPUESTA";
                    element_html_actuliza = "Image_estado_resp";
                } else {
                    var element_html_actuliza = ""; //Guarda el nombre del elemento que se actualiza
                    funcion_name = "actualiza_semaforo_respuesta";
                    evento_adjunta = "SUBE_RESPUESTA";
                    element_html_actuliza = "Image_estado_resp";
                    tipo_adjunta = 2;
                }

            }
            id_respuesta = document.getElementById("Hidden_id_respuesta").value;
            element_update_panel = "Button_update_semaforo";
            id_tipo_docuental = -1;
            element_parent = "ModalPopupExtender_edition_sube_documento_respuesta";
            numero_documento_relacionado = - 1;
            star_copy_interval_file_Upload(estado_adjunto, estado_relacion, id_tipo_docuental, funcion_name, element_parent, evento_adjunta,
                numero_documento_relacionado, element_html_actuliza, element_update_panel, id_respuesta, tipo_adjunta, element_isert_table, "", "", 0);
        }
        //Adjnta anexo respuesta
        if (CONTEN_NAME_UPLOAD_FILE == "adjunto_anexo_respuesta") {    
            funcion_name = "actualiza_drowp_respuesta";
            evento_adjunta = "SUBE_ANEXO";
            element_html_actuliza = "DropDownList_anexos_respuesta";
            tipo_adjunta = -1;
            id_respuesta = document.getElementById("Hidden_id_respuesta").value;
            element_update_panel = "";
            id_tipo_docuental = -1;
            element_parent = "ModalPopupExtender_edition_sube_anexo_respuesta";
            numero_documento_relacionado = - 1;
            star_copy_interval_file_Upload(estado_adjunto, estado_relacion, id_tipo_docuental, funcion_name, element_parent, evento_adjunta,
                numero_documento_relacionado, element_html_actuliza, element_update_panel, id_respuesta, tipo_adjunta, element_isert_table, "", "", 0);
        }
    } catch (err) {
        alert(err.mensaje + " function start_file_save_UploadFile")
    }
}
//ACTIVA LOS EVENTOS DESDE LOS BOTONES
function inicializa_tipo_adjunto_documento(event, element, value_sel) {
    try {
        //Activa lista gestion
        if (value_sel == "R-LISTA-GESTION") {
            document.getElementById("Button_tool_activa_lista_gestion_solicitud").click();
        }
        //Activa registro gestión
        if (value_sel == "R-GESTION-SOLICITUD") {
            $find("ModalPopupExtender_edition_gestion_respuesta_solicitud").show();
        }
        //Activa redireccionar petición a entidad externa
        if (value_sel == "R-ENTIDAD-EXTERNA") {
            document.getElementById("TextBox_nombre_externo").value = "";
            document.getElementById("TextBox_identificacion").value = "";
            document.getElementById("TextBox_correo_electronico").value = "";
            document.getElementById("TextBox_direccion").value = "";
            document.getElementById("TextBox_direccion").value = "";
            $("#departamento").empty();
            $("#municipio").empty();
            $("#pais").empty();
            service_source_list_item(0, "service_lista_paises", "pais");
            $find('ModalPopupExtender_edition_redirecciona_entidad_externa').show();
        }
        //Traslada solictud de petición a entidad externa
        if (value_sel == "E-ENTIDAD-EXTERNA") {
            event_element_menu("E-ENTIDAD-EXTERNA", "");
        }
        //Activa renviar respuesta al correo
        if (value_sel == "A-RENVIA-COR_TR") {
            if (document.getElementById("hdnEmailID").value == "" || document.getElementById("hdnEmailID").value == "-1") {
                alert("Debe seleccionar el registro del tramite enviar la notificacion del tramite");
            } else {
                event_element_menu("A-RENVIA-COR_TR", document.getElementById("hdnEmailID").value);
            }         
        }
        //Reasigna tramite
        if (value_sel == "R-ASIG-LISTA") {
            asig_array_tokenize_user_asig();
            if (document.getElementById("Hidden_token_tokenize2").value == "-1") {
                alert("Debe digitar el usuario para hacer la reasignación");
            } else {
                event_element_menu("R-ASIG-LISTA", document.getElementById("Hidden_token_tokenize2").value);
            }
            
        }
        //Archiva tramite gestion
        if (value_sel == "A-AH-GES") {
            asig_array_tokenize_archiva_user_correo();
            var envio_correo = 0;
            if (document.getElementById("CheckBox_envio_correo_solo_confirmar").checked == true) {
                envio_correo = 1;
                if (document.getElementById("Hidden_token_tokenize_k").value == "") {
                    alert("Debe informar el correo electronico para notificar el archivado del tramite");
                    return true;
                }
            } else {
                envio_correo = 0;     
            }
            event_element_menu("A-AH-GES", envio_correo);
        }
        //Adjunta documento enlace 
        if (value_sel == "C-DW-ENL") {
            document.getElementById("Hidden_tip_adjunt").value = "wf";
            document.getElementById("Button_tool_adjunta_documento_relacionado").click();
        }
        //Adjunta documento lista con event interval
        if (value_sel == "C-DW-LISTA") {
            event_element_menu("C-DW-LISTA", "adjunto_doc_visor");
        }
        //Adjunta documento lista con event interval Activa subir el documento desde el visor
        if (value_sel == "C-DW-VIS") {
            event_element_menu("C-DW-VIS", "adjunto_doc_visor");
          
        }
        //Adjunta documento lista con event interval Activa subir el documemento respuesta
        if (value_sel == "S-D-R") {
            event_element_menu("S-D-R", "adjunto_doc_respuesta");

        }
        //Adjunta documento anexo documento respuesta
        if (value_sel == "S-D-A") {
            event_element_menu("S-D-A", "adjunto_anexo_respuesta");
        }
        //Elimina documento respuesta
        if (value_sel == "E-D-R") {
            var mensaje = confirm("Desea eliminar el documento respuesta");     
            //Detectamos si el usuario acepto el mensaje
            if (mensaje) {
                event_element_menu("E-D-R", "");
            }
           
        }
        //Elimina anexo respuesta
        if (value_sel == "E-A-R") {
            var dorop_list = document.getElementById("DropDownList_anexos_respuesta");
            if (dorop_list.length == 0) {
                return true;
            }
            if (dorop_list.value == "") {
                return true;
            }
            var x = dorop_list.selectedIndex;
            var mensaje = confirm("Desea eliminar el anexo respuesta (" + dorop_list.innerText + ")");
            //Detectamos si el usuario acepto el mensaje
            if (mensaje) {
                event_element_menu("E-A-R", dorop_list.value);
            }

        }
        //Elimina anexo respuesta simple
        if (value_sel == "E-A-R-S") {
            var dorop_list = document.getElementById("DropDownList_anexos_respuesta_simple");
            if (dorop_list.length == 0) {
                return true;
            }
            if (dorop_list.value == "") {
                return true;
            }
            var x = dorop_list.selectedIndex;
            //var mensaje = confirm("Desea eliminar el anexo respuesta (" + document.getElementsByTagName("option")[x].text + ")");
            var mensaje = confirm("Desea eliminar el anexo respuesta (" + dorop_list.innerText + ")");
            //Detectamos si el usuario acepto el mensaje
            if (mensaje) {
                event_element_menu("E-A-R-S", dorop_list.value);
            }

        }
        //Descarga anexo simple
        if (value_sel == "D-A-R-S") {
            var dorop_list = document.getElementById("DropDownList_anexos_respuesta_simple");
            if (dorop_list.length == 0) {
                return true;
            }
            if (dorop_list.value == "") {
                return true;
            }
            var x = dorop_list.selectedIndex;
            FILE_ARCHIVO_DONWLOAD = dorop_list.innerText;
            event_element_menu("D-A-R-S", dorop_list.value);

        }
        //Descarga anexo formal
        if (value_sel == "D-A-R-F") {
            var dorop_list = document.getElementById("DropDownList_anexos_respuesta");
            if (dorop_list.length == 0) {
                return true;
            }
            if (dorop_list.value == "") {
                return true;
            }
            var x = dorop_list.selectedIndex;
            FILE_ARCHIVO_DONWLOAD = dorop_list.innerText;
            event_element_menu("D-A-R-F", dorop_list.value);

        }
        //Respuesta formal
        if (value_sel == "R-R-R-F") {
            var dorop_list = document.getElementById("DropDownList_lista_firmas_confirma_respuesta");
            if (dorop_list.length == 0) {
                alert("Debe seleccionar un firmante");
                dorop_list.focus();
                return true;
            }
            if (dorop_list.value == "") {
                alert("Debe seleccionar un firmante");
                dorop_list.focus();
                return true;
            }
            if (document.getElementById("Hidden_id_respuesta").value == "" || document.getElementById("Hidden_id_respuesta").value == "-1") {
                alert("Debe seleccionar una respuesta");
                return true;
            }
           
            var mensaje = confirm("Desea terminar su gestión y enviar la respuesta ?" );
            //Detectamos si el usuario acepto el mensaje
            if (mensaje) {
                event_element_menu("R-R-R-F", document.getElementById("Hidden_id_respuesta").value);
            }
            
        }
        //Respuesta simple solo confirmación
        if (value_sel == "R-R-R-S") {     
            var mensaje = confirm("Desea terminar su gestión y enviar mensaje de confirmación ?");
            //Detectamos si el usuario acepto el mensaje
            if (mensaje) {
                event_element_menu("R-R-R-S", document.getElementById("Hidden_id_respuesta").value);
            }
           
        }
        //Rerversa gestion respuesta autorizacion
        if (value_sel == "E-R-R-A") {
            if (document.getElementById("TextBox_login_usuario_val").value == "") {
                alert("Debe informar el usuario de autorización ");
                return true;
            }
            if (document.getElementById("TextBox_pasw_usuario_val").value == "") {
                alert("Debe informar el pasword de autorización ");
                return true;
            }
            event_element_menu("E-R-R-A", document.getElementById("Hidden_id_respuesta").value);
        }
        //Rerversa gestion respuesta
        if (value_sel == "E-R-R-C") {  
            event_element_menu("E-R-R-C", document.getElementById("Hidden_id_respuesta").value);
        }
        //Notifica gestión correo electronico
        if (value_sel == "N-R-C-E") {
            event_element_menu("N-R-C-E",document.getElementById("Hidden_id_respuesta").value);
        }
        //Activa solicitudes de aprobación
        if (value_sel == "A-P-S-A") {
            event_element_menu("A-P-S-A", document.getElementById("Hidden_id_respuesta").value);
        }
        //Registra solicitud de aprobación
        if (value_sel == "R-S-A-D") {
            event_element_menu("R-S-A-D", document.getElementById("Hidden_id_respuesta").value);
        }
        //Actualiza estado solicitud de aprobación listado
        if (value_sel == "A-E-S-A") {
            Service_solicita_estado_tramite_tarea_workflow(document.getElementById("hdnEmailID").value);
            //event_element_menu("A-E-S-A", document.getElementById("hdnEmailID").value);
        }
        //Descarga documento respuesta
        if (value_sel == "D-D-R-S") {
            event_element_menu("D-D-R-S", document.getElementById("Hidden_id_respuesta").value);
        }
        //Activa subir el documento desde el visor automatico
        if (value_sel == "C-DW-AUTO") {
            document.getElementById("Hidden_tip_adjunt").value = "wf";
            document.getElementById("Button_tool_activa_sube_documento_automatico").click();
        }
        if (value_sel == "C-DW-RD") {
            document.getElementById("Hidden_tip_adjunt").value = "rad";
            document.getElementById("Button_tool_activa_sube_documento_enlace").click();
        }
        if (value_sel == "C-DW-WS") {
            document.getElementById("Hidden_tip_adjunt").value = "rad";
            document.getElementById("Button_tool_activa_sube_documento_web_service").click();
        }

    }
    catch (err) {
        alert(err.message + " Funcion inicializa_tipo_adjunto_documento");
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
//Agrega link solicitudes aprobación
function insert_link_lista_solicitudes(nombre_grid, id, valor_campo, nombre_campo) {
    try {
        $("#" + nombre_grid + " tr[id=" + id + "]").each(function () {
            var idex = -1;
            var name = nombre_campo;
            idex = colum_index_(name, nombre_grid);
            if (idex != -1) {          
                if (valor_campo !== "") {       
                    $(this)[0].cells[idex].innerText = "";
                    $(this)[0].cells[idex].innerHTML = "";
                    var ahtml = document.createElement("a");
                   
                    if (valor_campo == "Solicitud por aprobación") {
                        ahtml.classList.add("btn_wrrap");
                        ahtml.classList.add("btn-primary");
                        ahtml.classList.add("btn-sm");
                    }
                    if (valor_campo == "Solicitud aprobada") {
                        ahtml.classList.add("btn_wrrap");
                        ahtml.classList.add("btn-success");
                        ahtml.classList.add("btn-sm");
                    }
                    if (valor_campo == "Solicitud desaprobada") {
                        ahtml.classList.add("btn_wrrap");
                        ahtml.classList.add("btn-danger");
                        ahtml.classList.add("btn-sm");
                    }
                    if (valor_campo == "Solicitud archivada") {
                        ahtml.classList.add("btn_wrrap");
                        ahtml.classList.add("btn-warning");
                        ahtml.classList.add("btn-sm");
                    }
                    if (valor_campo == "Solicitud anulada") {
                        ahtml.classList.add("btn_wrrap");
                        ahtml.classList.add("btn-warning");
                        ahtml.classList.add("btn-sm");
                    }
                    ahtml.classList.add("text-wrap");
                    ahtml.setAttribute("onclick", "prevent(event,this);");
                    ahtml.setAttribute("title", "Lista solictudes de aprobación de respuesta");
                    ahtml.setAttribute("idd" , id);
                    ahtml.setAttribute("tip_event", "lista_solic_tramite");
                    ahtml.innerHTML = valor_campo;
                    $(this)[0].cells[idex].appendChild(ahtml);
                }
            }
        })
    } catch (err) {
        alert("función insert_link_lista_solicitudes " + err.mensaje);
    }
}
function changue_row_font_weigh_light(name_gred_,id_value) {
    try {
        var nombre_grid = name_gred_;
        $("#" + nombre_grid + " tr[id=" + id_value + "]").each(function () {
            var elment_row = $(this)[0];
            if (elment_row) {
                elment_row.classList.remove("font-weight-bold");
                elment_row.classList.add("font-weight-light");
            }
           
        })

    } catch (ex) {
        alert("Error funcion changue_row_font_weigh_light " + ex.mensaje);
    }
}
function changue_row_font_weigh_bold(name_gred_,id_value) {
    try {
        var nombre_grid = name_gred_;
        $("#" + nombre_grid + " tr[id=" + id_value + "]").each(function () {
            var elment_row = $(this)[0];
            if (elment_row) {
                elment_row.classList.remove("font-weight-light");
                elment_row.classList.add("font-weight-bold");
            }


        })

    } catch (ex) {
        alert("Error funcion changue_row_font_weigh_light " + ex.mensaje);
    }
}
//Agrega item de documento relacionado en workflow
function insert_row_documento_relacionado(date_campo, selecion, activa_registro_versionado) {
    try {
        var element_atrit;
        var element_sel;
        if (selecion == "wf") {
            element_sel = "GridView_list_documento_relacion_wf";
            element_atrit = "_wf";
        } else {
            selecion = "rad";
            element_sel = "GridView_list_documento_relacion";
            element_atrit = "_rad";
        }
        var element_table = document.getElementById(element_sel);
        if (element_table) {
        } else {
            return true;
        }
        if (date_campo == "") {
            return true;
        }
        let split = date_campo.split("|");
        let IconoFle = "";
        if (split.length >= 8) {
            IconoFle = "fal " + split[7];
        } else {
            IconoFle = "fal fa-file"
        }
        let iconFirma = "";
        switch (split[6]) {
            case 1:
                iconFirma = "fal fa-lock-alt";
                break;
            case 2:
                iconFirma = "fal fa-lock-alt";
                break;
            default:
                iconFirma = "fal fa-file-signature";
        }
        let html_menu;
        if (selecion == "wf") {
            html_menu = [
                '<div class="row pl-1 w-100" style="display:inline-flex; margin-right:0px">',
                '<div class="w-100 col-10 pl-2 row" style="margin-right:0px;" onclick="prevent(event,this);" title="Ver documento" id_wf="' + split[1] + ' " idd_wf="' + date_campo + ' " tip_event=vis_doc_selecion_' + selecion + '>',
                '<div class="col-2 pt-2 ">',
                '<a  class=" font-weight-light" style="color: #0062cc;" aria-hidden="true" focusable="false"> ',
                '<i class="' + IconoFle + '" style="color:#0062cc;"></i>',
                '</a>',
                '</div>',
                '<div class="col-10 pl-1 pt-1">',
                '<spam class="pl-0 GridviewSpanOverFlow" style="color:black;">' + split[4] + '',
                '</spam>',
                '</div>',
                '</div>',
                '<div class="col-2 p-0 nav-item dropdown active">',
                '<a class="nav-link dropdown-toggle justify-content-start btn-lg mt-1" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false" href="#"></a>',
                '<div class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink">',
                '<a  class="dropdown-item pl-3 font-weight-light" onclick="prevent(event,this);" title="Eliminar documento"',
                'id' + element_atrit + '="' + split[1] + '" idd' + element_atrit + '="' + date_campo + '" tip_event="elim_doc_selecion_' + selecion + '" class="dropdown-item font-weight-light" style="margin-left:1px; aria-hidden="true" focusable="false"  > <i style="color: #0062cc;" class="fal fa-trash-alt"></i> <spam class="pl-1 font-weight-light"> Eliminar documento</spam></a>',
                '<a  class="dropdown-item pl-3 font-weight-light" onclick="prevent(event,this);" title="Cambiar tipología documental"',
                'id' + element_atrit + '="' + split[1] + '" idd' + element_atrit + '="' + date_campo + '" tip_event="cambia_doc_selecion_' + selecion + '" class="dropdown-item font-weight-light" style="margin-left:1px; aria-hidden="true" focusable="false"  > <i style="color: #0062cc;" class="fal fa-file-edit"></i> <spam class="pl-1 font-weight-light"> Cambiar tipología</spam></a>',
                '<a  class="dropdown-item pl-3 font-weight-light" onclick="prevent(event,this);" title="Firmar y agregar meta dato"',
                'id' + element_atrit + '="' + split[1] + '" idd' + element_atrit + '="' + date_campo + '" tip_event="firma_doc_selecion_' + selecion + '" class="dropdown-item font-weight-light" style="margin-left:1px; aria-hidden="true" focusable="false" > <i style="color: #0062cc;" class="' + iconFirma + '"></i> <spam class="pl-1 font-weight-light"> Firma digital</spam></a>',
                '</div>',
                '</div>',
                '</div>'
            ].join('')
        } else {
            let option_activa_registro_versionado = "";
            if (activa_registro_versionado == 1) {
                let option_activa_registro_versionado = '<a  class="dropdown-item pl - 3 font - weight - light" onclick="prevent(event, this); " title="Versiones del documento" ' +
                    'id' + element_atrit + '="' + split[1] + '" idd' + element_atrit + '="' + date_campo + '" tip_event="lista_ver_doc_selecion_' + selecion + '" class="dropdown-item font-weight-light" style="margin-left:1px; aria-hidden="true" focusable="false" > <i style="color: #0062cc;" class="far fa-folder-open"></i> <spam class="pl-1 font-weight-light"> Versiones del documento</spam></a> '
                '<a  class="dropdown-item pl-3 font-weight-light" onclick="prevent(event,this);" title="Remplazar documento" ' +
                    'id' + element_atrit + '="' + split[1] + '" idd' + element_atrit + '="' + date_campo + '" tip_event="remplaza_ver_doc_selecion_' + selecion + '" class="dropdown-item font-weight-light" style="margin-left:1px; aria-hidden="true" focusable="false" > <i style="color: #0062cc;" class="far fa-clone"></i> <spam class="pl-1 font-weight-light"> Remplazar documento</spam></a>';
            }
            html_menu = [
                '<div class="row pl-1 w-100" style="display:inline-flex; margin-right:0px">',
                '<div class="w-100 col-10 pl-2 row" style="margin-right:0px;" onclick="prevent(event,this);" title="Ver documento" id_rad="' + split[1] + ' " idd_rad="' + date_campo + ' " tip_event=vis_doc_selecion_' + selecion + '>',
                '<div class="col-2 pt-2 ">',
                '<a  class=" font-weight-light" style="color: #0062cc;" aria-hidden="true" focusable="false"> ',
                '<i class="' + IconoFle + '" style="color:#0062cc;"></i>',
                '</a>',
                '</div>',
                '<div class="col-10 pl-1 pt-1">',
                '<spam class="pl-0 GridviewSpanOverFlow" style="color:black;">' + split[4] + '',
                '</spam>',
                '</div>',
                '</div>',
                '<div class="col-2 p-0 nav-item dropdown active">',
                '<a class="nav-link dropdown-toggle justify-content-start btn-lg mt-1" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false" href="#"></a>',
                '<div class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink">',
                '<a  class="dropdown-item pl-3 font-weight-light" onclick="prevent(event,this);" title="Eliminar documento"',
                'id' + element_atrit + '="' + split[1] + '" idd' + element_atrit + '="' + date_campo + '" tip_event="elim_doc_selecion_' + selecion + '" class="dropdown-item font-weight-light" style="margin-left:1px; aria-hidden="true" focusable="false" data-prefix="fal" data-icon="trash-alt" role="img" > <i style="color: #0062cc;" class="fal fa-trash-alt"></i> <spam class="pl-1 font-weight-light"> Eliminar documento</spam></a>',
                '<a  class="dropdown-item pl-3 font-weight-light" onclick="prevent(event,this);" title="Cambiar tipología documental"',
                'id' + element_atrit + '="' + split[1] + '" idd' + element_atrit + '="' + date_campo + '" tip_event="cambia_doc_selecion_' + selecion + '" class="dropdown-item font-weight-light" style="margin-left:1px; aria-hidden="true" focusable="false" data-prefix="fal" data-icon="file-edit" role="img" > <i style="color: #0062cc;" class="fal fa-file-edit"></i> <spam class="pl-1 font-weight-light"> Cambiar tipología</spam></a>',
                '<a  class="dropdown-item pl-3 font-weight-light" onclick="prevent(event,this);" title="Firmar y agregar meta dato"',
                'id' + element_atrit + '="' + split[1] + '" idd' + element_atrit + '="' + date_campo + '" tip_event="firma_doc_selecion_' + selecion + '" class="dropdown-item font-weight-light" style="margin-left:1px; aria-hidden="true" focusable="false" > <i style="color: #0062cc;" class="' + iconFirma + '"></i> <spam class="pl-1 font-weight-light"> Firma digital</spam></a>',
                option_activa_registro_versionado,
                '</div>',
                '</div>',
                '</div>'
            ].join('')
        }
        var conta_td = 0;
        //Agrega el row a la tabla
        var element_row = element_table.insertRow(element_table.rows.length);
        element_row.setAttribute("id" + element_atrit, split[1]);
        element_row.setAttribute("idd" + element_atrit, date_campo);
        element_row.style.cursor = "pointer";
        element_row.style.background = "white";
        element_row.style.color = "black";
        //Agregar el check en la celda 0
        let element_td = element_row.insertCell(0);
        let htmlCheck;
        if (selecion == "wf") {
            htmlCheck = ['<div class="pl-0 pt-2">',
                '<input type="checkbox" class="ml-0 chek_selecion_list_wf" chek_id="' + split[1] + '">',
                '</div>'
            ].join('')
        } else {
            htmlCheck = ['<div class="pl-0 pt-2">',
                '<input type="checkbox" class="ml-0 chek_selecion_list_wf" chek_id="' + split[1] + '">',
                '</div>'
            ].join('')
        }
        element_td.insertAdjacentHTML("beforeend", htmlCheck);
        //Agrega la celda del menú
        element_td = element_row.insertCell(1);
        //Agrega el menu a la celda
        element_td.insertAdjacentHTML("beforeend", html_menu);
        var numero_fila = element_table.rows.length - 1;
        if (selecion == "wf") {
            document.getElementById("Hidden_numero_doc_rel_wf").value = numero_fila;
            document.getElementById("Label_docu_relacionado_wf").innerHTML = "Documentos " + numero_fila;
        } else {
            document.getElementById("Hidden_numero_doc_rel").value = numero_fila;
            document.getElementById("Label_documentos").innerHTML = "Documentos " + numero_fila;
        }



    } catch (err) {
        alert(err.message + " Funcion insert_row_producion_documental");
    }
}
//Functión que actualiza el campo del muevo modelo de tabla aspNET con menu
const update_Cell_AspNetGred = (nombre_grid, id, valor_campo, nombre_campo, atr) => {
    try {
        $("#" + nombre_grid + " tr[" + atr + "=" + id + "]").each(function () {
            let idex = -1;
            idex = colum_index_(nombre_campo, nombre_grid);
            if (idex != -1) {
                let htmlCel = $(this)[0].cells[idex];
                let htmlSpan = htmlCel.firstChild.firstChild.lastChild.firstChild;
                if (valor_campo !== "") {
                    if (htmlSpan !== null) {
                        htmlSpan.innerText = valor_campo;
                    }
                } else {
                    if (htmlSpan !== null) {
                        htmlSpan.innerText = "\u00a0";
                    }
                }
            }
        })
    }
    catch (err) {
        alert(err.message + " update_Cell_AspNetGred");
    }
}
function eliminar_fila_data_gred_simple_wf(gred, nombre_hiden, nombre_hiden_, seter, seter_) {
    try {
        var id_dent = $("#" + gred + " tr[id_wf=" + $("#" + nombre_hiden).val() + "]");
        if (id_dent.length > 0) {
            $("#" + gred + " tr[id_wf=" + $("#" + nombre_hiden).val() + "]").remove();
            if (seter !== "") {
                $('#' + nombre_hiden).val(seter);
                $('#' + nombre_hiden_).val(seter_);
                
            } else { document.getElementById("Label_estado_selecion").innerHTML = ""; }
            decrementa_documento_relacion_estado_wf();
        }

    }
    catch (err) {
        alert(err.message + " Funcion eliminar_fila_data_gred_simple_wf");
    }

}

function decrementa_documento_relacion_estado_wf() {
    try {
        var element_table = document.getElementById("GridView_list_documento_relacion_wf");
        var numero_fila = element_table.rows.length - 1;
        document.getElementById("Hidden_numero_doc_rel_wf").value = numero_fila;
        document.getElementById("Label_docu_relacionado_wf").innerHTML = "Documentos " + numero_fila;
    }
    catch (err) {
        alert(err.message + " Funcion decrementa_documento_relacion_estado");
    }
}
function actualiza_gre_campo_wf_seleccion(nombre_grid, id, valor_campo, nombre_campo,id_visor) {
    try {
        $("#" + nombre_grid + " tr[id_wf=" + id + "]").each(function () {
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
                    $(this)[0].cells[idex].innerText = valor_campo;
                    $(this)[0].cells[idex].innerHTML = valor_campo;
                    if (id_visor == id) {                
                        if (valor_campo != "") {
                            valor_campo = reemplazarAcentos(valor_campo);
                            document.getElementById("Label_estado_selecion").innerHTML = valor_campo;
                        }
                    }
                }
            }
        })
        return true;
    }
    catch (err) {
        alert(err.message + " Funcion actualiza_gre_campo");
    }
}
function busca_campo_wf_seleccion(nombre_grid, id_, nombre_campo) {
    try {
        var valor_retorn = "";
        $("#" + nombre_grid + " tr[id_wf=" + id_ + "]").each(function () {
            var idex = -1;
            var name = nombre_campo;
            var d = id_;
            idex = colum_index_(name, nombre_grid);
            if (idex != -1) {
                var k = $(this)[0].cells[idex];
                var valor = $(this)[0].cells[idex].innerText;
                if (valor == "") {
                    $(this)[0].cells[idex].innerHTML = valor;
                }
                valor_retorn = valor; 
            } else {
                valor_retorn = "";
            }
        })
        return valor_retorn;
    }
    catch (err) {
        alert(err.message + " Funcion busca_campo_wf_seleccion");
    }
}
function onDataShown(sender, args) {
    sender._popupBehavior._element.style.zIndex = 1000001;

}
function eliminar_ajaxtolkit() {
    try {
        var ele = document.getElementsByClassName("ajax__fileupload_fileItemInfo");
        for (var i = 0; i < ele.length; i++) {
            ele[i].parentNode.removeChild(ele[i]);
        }
    } catch (err) {
        alert(err.message + " funcion eliminar_ajaxtolkit " + err.message);
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
//---WEB SERVICE INDICE
function service_actualiza_indice_workflow(tipo_indice_actualiza) {
    try {
        var para_meter_ca = new Array();
        var input_tag = $('#raw_some_table :input');
        for (var i = 0; i < input_tag.length; i++) {
            para_meter_ca.push({
                nombre_campo: input_tag[i].id, valor_campo: input_tag[i].value.replace("'", ""), tipo_campo: ""
            });
        }
        var hident_tag = $('.dec_000_21_000');
        for (var i = 0; i < hident_tag.length; i++) {
            para_meter_ca.push({
                nombre_campo: hident_tag[i].id, valor_campo: hident_tag[i].value.replace("'", ""), tipo_campo: ""
            });
        }
        var serialice = JSON.stringify(para_meter_ca);
        $.ajax('../webservice/WebServiceDocuarchi.asmx/Set_actualiza_indice_docuarchi', {
            data: "{" + "'parameter':'" + serialice + "','tipo_indice_actualiza':'" + tipo_indice_actualiza + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d !== "YES") {
                    alert(data.d);
                    ESTADO_EVENT_GENERAL = "out";
                } else { alert("Se actualizo correctamente"); ESTADO_EVENT_GENERAL = "out";}
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
            }, compelete: function () {

            }
        });
    } catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        alert(ex.message + " funcion service_actualiza_indice_workflow");
    }
}

//--TERMINA ZONA
//-----------------------------------------------------------------------ZONA NOTA WORKFLOW --------------------------------
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

//------------CONFIGURACION------------------------------------------
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

            //document.getElementById("Button_ver_nota").click();
        }
        event.preventDefault();
        element.focus();
    }
    catch (err) {
        alert(err.message + " Funcion prevent_event");
    }
}
//Elimina fila
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
//-----------------------------------------------------------------------FIN ZONA NOTA WORKFLOW-----------------------------
//ZONA TOKENIZE
function inicia_token_usuario_wf(dw_name_token) {
    try {
        $('.' + dw_name_token).tokenize2({
            placeholder: "Relacione usuario o cargo ",
            tokensAllowCustom: false,
            tokensMaxItems: 1,
            dataSource: function (search, object) {
                $.ajax('../webservice/WebServiceWorkflow.asmx/GetLista_usuarios_workflow_tokenize', {
                    data: "{'DName':'" + search + "'}",
                    dataType: 'json',
                    type: "POST",
                    traditional: true,
                    processData: false,
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        var $items = [];
                        $.each(data.d, function (k, v) {
                            $items.push(v);
                        });
                        object.trigger('tokenize:dropdown:fill', [$items]);

                    }
                });
            }

        });
    } catch (ex) {

    }
}

function verifi_token_array(token, value) {
    try {
        for (var i = 0; i < token.length; i++) {
            if (token[i].text == value) {
                return "YES";
            }
        }
        return "";
    } catch (ex) { alert(ex.message); }
}
//------------------------------------------------
//EVENTO ADD MATRIZ TOKENIZE ITEMS_DATOS_TOKENIZE
//------------------------------------------------
function set_tokenize_add_event_valid(name_token) {
    try {
        $('.' + name_token).on('tokenize:tokens:added', function (e, value, text) {
            if (value == "" || value == null) {
                return true;
            }
            var ident = validator_cuenta_correo(value);
            if (ident) {
                var token = document.getElementsByClassName("token");
                if (token) {
                    for (i = 0; i < token.length; i++) {
                        var value_token = token[i].getAttribute("data-value");
                        if (value_token == value) {
                            token[i].style.color = "black";
                        }
                    }
                }
                if (name_token == "tokenize-callable-demo_respuesta") {
                    ITEMS_DATOS_TOKENIZE_2.push({ text: text, value: value });
                }
                if (name_token == "tokenize-callable-demo_respuesta_simple") {
                    ITEMS_DATOS_TOKENIZE_6.push({ text: text, value: value });
                }
                if (name_token == "tokenize-callable-demo_respuesta_") {
                    ITEMS_DATOS_TOKENIZE_3.push({ text: text, value: value });
                }
                if (name_token == "tokenize-callable-demo_respuesta__") {
                    ITEMS_DATOS_TOKENIZE_4.push({ text: text, value: value });
                }
                if (name_token == "tokenize-callable-demo_respuesta_k") {
                    ITEMS_DATOS_TOKENIZE_8.push({ text: text, value: value });
                }

            } else {
                var token = document.getElementsByClassName("token");
                if (token) {
                    for (i = 0; i < token.length; i++) {
                        var value_token = token[i].getAttribute("data-value");
                        if (value_token == value) {
                            token[i].style.color = "red";
                            token[i].style.textDecoration = "line-through";
                        }
                    }    
                    if (name_token == "tokenize-callable-demo_respuesta") {
                        ITEMS_DATOS_TOKENIZE_2.push({ text: text, value: value });
                    }
                    if (name_token == "tokenize-callable-demo_respuesta_") {
                        ITEMS_DATOS_TOKENIZE_3.push({ text: text, value: value });
                    }
                    if (name_token == "tokenize-callable-demo_respuesta__") {
                        ITEMS_DATOS_TOKENIZE_4.push({ text: text, value: value });
                    }
                    if (name_token == "tokenize-callable-demo_respuesta_k") {
                        ITEMS_DATOS_TOKENIZE_8.push({ text: text, value: value });
                    }
                    if (name_token == "tokenize-callable-demo_respuesta_simple") {
                        ITEMS_DATOS_TOKENIZE_6.push({ text: text, value: value });
                    }
                   

                }

            }

        });
    } catch (ex) { alert(ex + " funcion set_tokenize_add_event_valid") }
}
//--------------------------------------------------
//EVENTO ADD TOKEN A LA INTERFACE TOKENICE
//-------------------------------------------------
function lista_tipos_respuesta() {
    $("#DropDownList_tipo_respuesta").empty();
    service_source_list_item(0, "Service_lista_tipo_respuesta", "DropDownList_tipo_respuesta");
}
function lista_tipos_respuesta_simple() {
    $("#Drop_tipo_respuesta").empty();
    service_source_list_item(0, "Service_lista_tipo_respuesta", "Drop_tipo_respuesta");
}
function asig_correo_token_respuesta(name_token) {
    try {
       
        if (CORREO_GESTION == "") {

        } else {
            var exit = "";
            if (name_token == "tokenize-callable-demo_respuesta_simple") {     
                $('.' + name_token).tokenize2().trigger('tokenize:tokens:add', [CORREO_GESTION, CORREO_GESTION, true]);
            }
            if (name_token == "tokenize-callable-demo_respuesta_") {
               
                $('.' + name_token).tokenize2().trigger('tokenize:tokens:add', [CORREO_GESTION, CORREO_GESTION, true]);
            }
            if (name_token == "tokenize-callable-demo_respuesta_k") {
               
                $('.' + name_token).tokenize2().trigger('tokenize:tokens:add', [CORREO_GESTION, CORREO_GESTION, true]);
            }
            if (name_token == "tokenize-callable-demo_respuesta__") {
              
                $('.' + name_token).tokenize2().trigger('tokenize:tokens:add', [CORREO_GESTION, CORREO_GESTION, true]);
            }
        }

    } catch (ex) {
        alert("funcion asig_correo_token_respuesta" + ex.mensaje);
    }
}
//------------------------------------------------
//EVENTO QUE ELIMINA TOKEN DE TOKENIZE
//------------------------------------------------
function set_tokenize_delete_event(name_token) {
    try {
        $('.' + name_token).on('tokenize:tokens:remove', function (e, value) {
            delete_array_tokenize(value, name_token);
        });
    } catch (ex) { alert(ex + " funcion set_nize_delete_event") }
}
function delete_array_tokenize(value_id) {
    try {
        for (var i = 0; i < ITEMS_DATOS_TOKENIZE_2.length; i++) {
            if (ITEMS_DATOS_TOKENIZE_2[i].value == value_id) {
                ITEMS_DATOS_TOKENIZE_2.splice(i, 1);
                i = ITEMS_DATOS_TOKENIZE_2.length;
            }
        }
    } catch (err) {
        alert(err.message + " Funcion delete_array_tokenize");
    }
}
function delete_array_tokenize(value_id, name_token) {
    try {
        if (name_token == "tokenize-callable-demo_respuesta") {
            document.getElementById("Hidden_token_tokenize2").value = "-1";
            for (var i = 0; i < ITEMS_DATOS_TOKENIZE_2.length; i++) {
                if (ITEMS_DATOS_TOKENIZE_2[i].value == value_id) {
                    ITEMS_DATOS_TOKENIZE_2.splice(i, 1);
                    i = ITEMS_DATOS_TOKENIZE_2.length;
                }
            }
        }
        if (name_token == "tokenize-callable-demo_respuesta_simple") {
            for (var i = 0; i < ITEMS_DATOS_TOKENIZE_6.length; i++) {
                if (ITEMS_DATOS_TOKENIZE_6[i].value == value_id) {
                    ITEMS_DATOS_TOKENIZE_6.splice(i, 1);
                }
            }
        }
        if (name_token == "tokenize-callable-demo_respuesta_") {
            for (var i = 0; i < ITEMS_DATOS_TOKENIZE_3.length; i++) {
                if (ITEMS_DATOS_TOKENIZE_3[i].value == value_id) {
                    ITEMS_DATOS_TOKENIZE_3.splice(i, 1);
                    i = ITEMS_DATOS_TOKENIZE_3.length;
                }
            }
        }
        if (name_token == "tokenize-callable-demo_respuesta_k") {
            for (var i = 0; i < ITEMS_DATOS_TOKENIZE_8.length; i++) {
                if (ITEMS_DATOS_TOKENIZE_8[i].value == value_id) {
                    ITEMS_DATOS_TOKENIZE_8.splice(i, 1);
                    i = ITEMS_DATOS_TOKENIZE_8.length;
                }
            }
        }
        if (name_token == "tokenize-callable-demo_respuesta__") {
            for (var i = 0; i < ITEMS_DATOS_TOKENIZE_4.length; i++) {
                if (ITEMS_DATOS_TOKENIZE_4[i].value == value_id) {
                    ITEMS_DATOS_TOKENIZE_4.splice(i, 1);

                }
            }
        }

    } catch (err) {
        alert(err.message + " Funcion delete_array_tokenize");
    }
}
function asig_array_tokenize_user_asig() {
    try {
        document.getElementById("Hidden_token_tokenize2").value = "-1";
        if (ITEMS_DATOS_TOKENIZE_2.length > 0) {
            document.getElementById("Hidden_token_tokenize2").value = ITEMS_DATOS_TOKENIZE_2[0].value;
            
        }
    } catch (err) {
        alert(err.message + " Funcion asig_array_tokenize_user_asig");
    }
}
//--------------------------------------------------------
//INCIALIZA TOKENIZE PARA ASIGNAR SERVICIO WEB DE BUSQUEDA
//---------------------------------------------------------
function toke_ini(name_token) {
    try {
        $('.' + name_token).tokenize2({
            placeholder: "digita correos electrónicos y presiona enter",
            tokensAllowCustom: true,
            zIndexMargin: 10001,
            dataSource: function (search, object) {
                $.ajax('../webservice/WebServiceRadicacion.asmx/GetLista_correos_usuarios_gestion_tokenize', {
                    data: "{'DName':'" + search + "'}",
                    dataType: 'json',
                    type: "POST",
                    traditional: true,
                    processData: false,
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        var $items = [];
                        $.each(data.d, function (k, v) {
                            $items.push(v);
                        });
                        try {
                            object.trigger('tokenize:dropdown:fill', [$items]);

                        }
                        catch (ex) { alert(ex + " Funcion toke_ini"); }
                    }
                });

            }

        });
    } catch (ex) {
        alert(ex.message);
    }
}
function asig_array_tokenize_user_correo() {
    try {
        document.getElementById("Hidden_token_tokenize2_").value = "";
        if (ITEMS_DATOS_TOKENIZE_3.length > 0) {
            for (var i = 0; i < ITEMS_DATOS_TOKENIZE_3.length; i++) {
                if (i == 0) {
                    document.getElementById("Hidden_token_tokenize2_").value = ITEMS_DATOS_TOKENIZE_3[i].text;
                } else {
                    document.getElementById("Hidden_token_tokenize2_").value = document.getElementById("Hidden_token_tokenize2_").value + "," + ITEMS_DATOS_TOKENIZE_3[i].text;
                }
            }
           
        }
    } catch (err) {
        alert(err.message + " Funcion asig_array_tokenize_user_asig");
    }
}
//Asigna los correos electronicos  token para archivar tramite
function asig_array_tokenize_archiva_user_correo() {
    try {
        document.getElementById("Hidden_token_tokenize_k").value = "";
        if (ITEMS_DATOS_TOKENIZE_8.length > 0) {
            for (var i = 0; i < ITEMS_DATOS_TOKENIZE_8.length; i++) {
                if (i == 0) {
                    document.getElementById("Hidden_token_tokenize_k").value = ITEMS_DATOS_TOKENIZE_8[i].text;
                } else {
                    document.getElementById("Hidden_token_tokenize_k").value = document.getElementById("Hidden_token_tokenize_k").value + "," + ITEMS_DATOS_TOKENIZE_8[i].text;
                }
            }

        }
    } catch (err) {
        alert(err.message + " Funcion asig_array_tokenize_archiva_user_correo");
    }
}
//Asigna el item token del correo electronico del usuario peticionario
function asig_correo_token(name_token) {
    try {
        if (COORREO_INI == "") {
            var id_respuest = document.getElementById("Hidden_id_respuesta_").value;
            solicita_correorespuesta_documento_tokenize(id_respuest);
            if (COORREO_INI !== "") {
                $('.' + name_token).tokenize2().trigger('tokenize:tokens:add', [COORREO_INI, COORREO_INI, true]);            
                if (name_token == "tokenize-callable-demo_respuesta_") {
                    ITEMS_DATOS_TOKENIZE_3.push({ text: COORREO_INI, value: COORREO_INI });
                }
               
            }
        } else {
            $('.' + name_token).tokenize2().trigger('tokenize:tokens:add', [COORREO_INI, COORREO_INI, true]);
            var exit = "";
            if (name_token == "tokenize-callable-demo_respuesta_") {
                exit = verifi_token_array(ITEMS_DATOS_TOKENIZE_3, COORREO_INI);
                if (exit !== "YES") {
                    ITEMS_DATOS_TOKENIZE_3.push({ text: COORREO_INI, value: COORREO_INI });
                }
            }
            
        }

    } catch (ex) { alert(ex + " funcion asig_correo_token") }
}

function solicita_correorespuesta_documento_tokenize(id_respuesta) {
    $.ajax({
        async: false,
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "../webservice/WebServiceRadicacion.asmx/GetLista_correos_respuesta_documento_tokenize",
        data: "{'DName':'" + id_respuesta + "'}",
        dataType: "json",
        success: function (data) {
            if (data.d) {
                COORREO_INI = data.d;
                return data.d;
                //add_regitro_correo_service = data.d;
            } else {
                return "";
                COORREO_INI = "";
                //add_regitro_correo_service = "YES";
            }
        },
        error: function (result) {
            //alert("Error......" + result);
            return "";
        }
    });
}

function add_regitro_correo_service(correo) {
    $.ajax({
        async: false,
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "../webservice/WebServiceRadicacion.asmx/Add_correos_cache_tokenize",
        data: "{'DName':'" + correo + "'}",
        dataType: "json",
        success: function (data) {
            if (data.d) {
                //add_regitro_correo_service = data.d;
            } else {
                //add_regitro_correo_service = "YES";
            }
        },
        error: function (result) {
            alert("Error......" + result);

        }
    });
}
function validator_cuenta_correo(value_correo) {
    try {
        if (value_correo == "") {
            return false;
        }
        //valida que se incluya el (@) en la direccion de correo electrónico
        if (value_correo.search("@") == -1) {
            return false;
        }
        //valida que se incluya el (.) en la direccion de correo electrónico
        if (value_correo.search(".") == -1) {
            return false;
        }
        //valida que el nombre del dominio este presente (co, com etc.....)
        let spl = value_correo.split(".");
        let value_ = spl[spl.length - 1];
        if (value_ == "") {
            return false;
        }
        if (value_ == ".") {
            return false;
        }
        //Valida nombre del servidor de dominio
        spl = value_correo.split("@");
        value_ = spl[1];
        let indexOF_punto = value_.indexOf(".");
        if (indexOF_punto == 0) {
            return false;
        }
        return true;
       
    } catch (ex) {
        return false;
    }
}
function toke_ini_solicitud(name_token) {
    try {
        $('.' + name_token).tokenize2({
            placeholder: "Para relacionar los usuarios puede digitar el nombre del usuario o el cargo del usuario...",
            dataSource: function (search, object) {
                $.ajax('../webservice/WebServiceWorkflow.asmx/GetLista_usuarios_workflow_z2', {
                    data: "{'DName':'" + search + "'}",
                    dataType: 'json',
                    type: "POST",
                    traditional: true,
                    processData: false,
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        var $items = [];
                        $.each(data.d, function (k, v) {
                            $items.push(v);
                        });
                        object.trigger('tokenize:dropdown:fill', [$items]);

                    }
                });
            }

        });
    } catch (ex) { alert(ex.message + " funcion toke_ini_solicitud ") }
}
function delete_array_tokenize_solicitud(value_id) {
    try {
        for (var i = 0; i < ITEMS_DATOS_TOKENIZE_5.length; i++) {
            if (ITEMS_DATOS_TOKENIZE_5[i].value == value_id) {
                ITEMS_DATOS_TOKENIZE_5.splice(i, 1);
                i = ITEMS_DATOS_TOKENIZE_5.length;
            }
        }
    } catch (err) {
        alert(err.message + " Funcion delete_array_tokenize_solicitud");
    }
}
function asig_array_tokenize_solicitud() {
    try {
        document.getElementsByName("Hidden_text_user").value = "";
        for (var i = 0; i < ITEMS_DATOS_TOKENIZE_4.length; i++) {
            if (i == 0) {
                document.getElementsByName("Hidden_text_user").value = '|' + ITEMS_DATOS_TOKENIZE_4[i].value + '|| ' + ITEMS_DATOS_TOKENIZE_4[i].text + ',';
            } else {
                document.getElementsByName("Hidden_text_user").value = document.getElementsByName("Hidden_text_user").value + '|' + ITEMS_DATOS_TOKENIZE_4[i].value + '|| ' + ITEMS_DATOS_TOKENIZE_4[i].text + ',';
            }
        }
    } catch (err) {
        alert(err.message + " Funcion asig_array_tokenize");
    }
}
function Solicitud_aprobacion_tokenize() {
    try {
        if (ITEMS_DATOS_TOKENIZE_5.length == 0) {
            alert("Debe selecionar los usuarios a compartir");
            return false;
        }
        var valParam = JSON.stringify(ITEMS_DATOS_TOKENIZE_5);
        var para_meter_ca = new Array();
        var asunto_ = "";
        var nota_ = document.getElementsByName('TextBox_nota_aprobacion');
        var nivel_urgencia_solicitud_ = document.getElementsByName('DropDownList_prioridad_solicitud');
        var tipo_solicitud_ = "";
        var fecha_limite_ = document.getElementsByName("TextBox_fecha_limite_solicitud");
        var radicado_relacionado_ = "";
        var id_usuario_propietario_ = document.getElementById("Hidden_id_respuesta").value;
        var matri_documentos_ = "";
        para_meter_ca.push({ asunto_: asunto_, nota_: nota_[0].value, nivel_urgencia_solicitud_: nivel_urgencia_solicitud_[0].value, tipo_solicitud_: tipo_solicitud_, radicado_relacionado_: radicado_relacionado_, id_usuario_propietario_: id_usuario_propietario_, matri_documentos_: matri_documentos_, fecha_limite_: fecha_limite_[0].value });
        var serialice = JSON.stringify(para_meter_ca);
        $.ajax('../webservice/WebServiceWorkflow.asmx/Set_Registra_solicitud_aprobacion', {
            data: "{'item_user':'" + valParam + "'," + "'parameter':'" + serialice + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d) {
                    var split = data.d.split("|");
                    if (split[0] !== "YES") {
                        alert(data.d);
                    } else {

                        document.getElementById("Button_cancelar_registro").click();
                    }
                }
            }
        });
    } catch (ex) { alert(ex.message + " funcion Solicitud_aprobacion_tokenize"); }
}
function asig_array_tokenize(name_token) {
    try {
        if (name_token == "tokenize-callable-demo_respuesta_simple") {
            document.getElementById("Hidden_text_user_correo").value = "";
            for (var i = 0; i < ITEMS_DATOS_TOKENIZE_6.length; i++) {
                if (i == 0) {
                    document.getElementById("Hidden_text_user_correo").value = ITEMS_DATOS_TOKENIZE_6[i].text;
                } else {
                    document.getElementById("Hidden_text_user_correo").value = document.getElementById("Hidden_text_user_correo").value + ',' + ITEMS_DATOS_TOKENIZE_6[i].text;
                }
            }

        }
        if (name_token == "tokenize-callable-demo_respuesta") {
            document.getElementById("Hidden_text_user_correo").value = "";
            for (var i = 0; i < ITEMS_DATOS_TOKENIZE_2.length; i++) {
                if (i == 0) {
                    document.getElementById("Hidden_text_user_correo").value = ITEMS_DATOS_TOKENIZE_2[i].text;
                } else {
                    document.getElementById("Hidden_text_user_correo").value = document.getElementById("Hidden_text_user_correo").value + ',' + ITEMS_DATOS_TOKENIZE_2[i].text;
                }
            }

        }
        //Asigna el token de respuesta formal   
        if (name_token == "tokenize-callable-demo_respuesta_") {
            document.getElementById("Hidden_text_user_correo").value = "";
            for (var i = 0; i < ITEMS_DATOS_TOKENIZE_3.length; i++) {
                if (i == 0) {
                    document.getElementById("Hidden_text_user_correo").value = ITEMS_DATOS_TOKENIZE_3[i].text;
                } else {
                    document.getElementById("Hidden_text_user_correo").value = document.getElementById("Hidden_text_user_correo").value + ',' + ITEMS_DATOS_TOKENIZE_3[i].text;
                }
            }

        }
        
        if (name_token == "tokenize-callable-demo_respuesta__") {
            document.getElementById("Hidden_text_user_correo").value = "";
            for (var i = 0; i < ITEMS_DATOS_TOKENIZE_4.length; i++) {
                if (i == 0) {
                    document.getElementById("Hidden_text_user_correo").value = ITEMS_DATOS_TOKENIZE_4[i].text;
                } else {
                    document.getElementById("Hidden_text_user_correo").value = document.getElementById("Hidden_text_user_correo").value + ',' + ITEMS_DATOS_TOKENIZE_4[i].text;
                }
            }

        }

    } catch (err) {
        alert(err.message + " Funcion asig_array_tokenize");
    }
}
//TERMINA ZONA TOKENICE
function actualiza_selecion() {
    try {
        document.getElementById("Label_anunciado_filtro").innerHTML = "Todas";
        document.getElementById("Hidden_lik_service_boton").value = "1";
    }
    catch (err) {
        alert(err.message + " Funcion actualiza_selecion");
    }
}
function preven_event_search(event, e) {
    try {
        document.getElementById("ImageButton_buscar").click();
        //event.preventDefault();
    }
    catch (err) {
        alert(err.message + " Funcion preven_event_search");
    }
}
function preven_event_search_keypres_enter(e, sender) {
    try {

        tecla = (document.all) ? e.keyCode : e.which;
        if (tecla == 13) {
            document.getElementById("ImageButton_buscar").click();
            //e.preventDefault();

        }


    } catch (err) {
        alert(err.message + " funcion preven_event_search_keypres_enter " + err.message);
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
        //event.preventDefault();
    }
    catch (err) {
        alert(err.message + " Funcion prevent_scrol");
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


        $('#Panel_visor_externo').css("height", (espacio_iframe) + "px");
        $('#Cotenedorpendiente_visor_externo').css("height", (espacio_iframe - 5) + "px");
        var heicontendor = document.getElementById("Cotenedorpendiente_visor_externo").clientHeight - document.getElementById("nav_visor").clientHeight;
        $('#Iframe_visor_externo_').css("height", (heicontendor - 10) + "px");
    }
    catch (err) {
        alert(err.message + " Funcion actualiza_gre_campos_dinamicos");
    }
}
function auto_zise_popup_lista_imagenes_gestion() {
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
        $('#Panel_lista_imagenes_gestion').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_lista_imagenes_gestion').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#contenido_procesa_lista_imagenes_gestion').css("height", (document.getElementById("modal_lista_imagenes_gestion").clientHeight - (document.getElementById("diver_cabcera_lista_imagenes_gestion").clientHeight)) + "px");
        $('#content_selecion_tarea').css("height", (document.getElementById("contenido_procesa_lista_imagenes_gestion").clientHeight) + "px");
        var he_content = document.getElementById("content_selecion_tarea").clientHeight;
        var heing = 1;
        if (document.getElementById("Panel_tolbar_pdf")) {
            heing = document.getElementById("Panel_tolbar_pdf").clientHeight;
        }
        var heing_tool_image = 1;
        if (document.getElementById("tollimage")) {
            heing_tool_image = document.getElementById("tollimage").clientHeight;
        }
        if (document.getElementById("content_selecion_tarea")) {
            $('#content_selecion_tarea').css("height", (he_content) + "px");
            $('#content').css("height", (he_content - (document.getElementById("content_pie_seleccion_tarea").clientHeight + heing_tool_image + 2)) + "px");
            $('#ifrm_visor_').css("height", (he_content - (document.getElementById("content_pie_seleccion_tarea").clientHeight + heing + 2)) + "px");
            $('#contenido_imagen').css("height", ((he_content - 10) - document.getElementById("content_pie_seleccion_tarea").clientHeight) + "px");
            $('#contenido_indice').css("height", (he_content - (document.getElementById("content_pie_seleccion_tarea").clientHeight )) + "px");
            $('#div_conent_indice').css("height", (he_content - (document.getElementById("content_pie_seleccion_tarea").clientHeight )) + "px");
            $('#content_seleccion_documentos').css("height", (he_content - document.getElementById("content_pie_seleccion_tarea").clientHeight) + "px");
            $('#seleccion').css("height", (he_content - (document.getElementById("content_pie_seleccion_tarea").clientHeight + document.getElementById("div_label").clientHeight)) + "px");
            $('#Panel_scroll').css("height", (he_content - (document.getElementById("content_pie_seleccion_tarea").clientHeight + document.getElementById("div_label").clientHeight)) + "px");
            $('#Panel_indice').css("height", (document.getElementById("div_conent_indice").clientHeight - (document.getElementById("title_indice").clientHeight + document.getElementById("title_indice").clientHeight)) + "px");
        }
       
       
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_lista_imagenes_gestion " + err.message);
    }
}
function auto_zise_popup_impresion() {
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
        $('#Panelimpresionpost').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_Panelimpresionpost').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#ContenidoImpresion_post').css("height", (document.getElementById("modal_content_Panelimpresionpost").clientHeight - (document.getElementById("divcabecer2_post").clientHeight)) + "px");
        //Para los modal que contiene gred
        $('#ifimpre_post_').css("height", (document.getElementById("ContenidoImpresion_post").clientHeight - 5) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_impresion " + err.message);
    }
}
function auto_zise_popup_guardar_documento() {
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
        $('#Panel_guardar').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_Panel_guardar').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#Content_guardar_documento').css("height", (document.getElementById("modal_content_Panel_guardar").clientHeight - (document.getElementById("divcabecer2_post_guardar").clientHeight)) + "px");
        //Para los modal que contiene gred
        $('#Iframe_guardar').css("height", (document.getElementById("Content_guardar_documento").clientHeight - 5) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_guardar_documento " + err.message);
    }
}
function auto_zise_popup_detalle_trazabilidad() {
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

    $('#Panel_trazabilidad').css("height", (espacio_iframe - 40) + "px");
    $('#Cotenedorpendiente_trazabilidad').css("height", (espacio_iframe - 40) + "px");
    $('#Iframe_trazabilidad_').css("height", (espacio_iframe - 40) + "px");

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
        $('#Panel_transacciones').css("height", (espacio_iframe - 40) + "px");
        $('#Cotenedorpendiente_transacciones').css("height", (espacio_iframe - 40) + "px");
        $('#Iframe_transacciones_').css("height", (espacio_iframe - 45) + "px");
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_detalle_transacciones");
    }
}
function auto_zise_popup_detalle_respuesta() {
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

    $('#Panel_detalle_respuesta').css("height", (espacio_iframe - 40) + "px");
    $('#Cotenedorpendiente_detalle_respuesta').css("height", (espacio_iframe - 40) + "px");
    $('#Iframe_visor_externo__').css("height", (espacio_iframe - 40) + "px");

}
function auto_zise_popup_consulta_meta_dato() {
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
        $('#Panel_interface_consulta_meta_dato').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_consulta_meta_dato').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#contenido_procesa_interface_consulta_meta_dato').css("height", (document.getElementById("modal_content_consulta_meta_dato").clientHeight - (document.getElementById("divcabecer2_interface_consulta_meta_dato").clientHeight)) + "px");
        $('#div_content_tabla').css("height", (document.getElementById("modal_content_consulta_meta_dato").clientHeight - (document.getElementById("divcabecer2_interface_consulta_meta_dato").clientHeight)) + "px");
        $('#table_meta_row').bootstrapTable('resetView', { height: (document.getElementById("contenido_procesa_interface_consulta_meta_dato").clientHeight - 30) });

    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_consulta_meta_dato " + err.message);
    }
}
function Decrementa_contador_tramites() {
    try {
         if (document.getElementById("Hidden_content").value !== 0) {
             document.getElementById("Hidden_content").value = document.getElementById("Hidden_content").value - 1;
         }
    }
    catch (err) {
        alert(err.message + " Decrementa_contador_tramites");
    }
}
function Actualiza_cantidad_barr_estado() {
    try {
         
        document.getElementById("Label_titulo_listado_solicitudes").innerHTML =  document.getElementById("Hidden_content").value + " registro(s) ";
    }
    catch (err) {
        alert(err.message + " Actualiza_cantidad_barr_estado");
    }
}
function service_posibles_datos_tramites_() {
    function split(val) {
        return val.split(/,\s*/);
    }
    function extractLast(term) {
        return split(term).pop();
    }
    $("#auto_complex")
         .on("keydown", function (event) {
             if (event.keyCode === $.ui.keyCode.TAB &&
                 $(this).autocomplete("instance").menu.active) {
                 //event.preventDefault();
             }
         })
        .autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: "../webservice/WebServiceWorkflow.asmx/GetPosiblesDatos_Tramites",
                    data: "{'DName':'" + document.getElementById("auto_complex").value + "'}",
                    dataType: "json",
                    type: "POST",
                    traditional: true,
                    processData: false,
                    contentType: "application/json; charset=utf-8",
                    //dataFilter: function (data) { return data; },
                    success: function (data) {
                        term: extractLast(request.term)
                        response($.ui.autocomplete.filter(
                        data.d, extractLast(request.term)));

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
            },

            focus: function () {
                // prevent value inserted on focus
                return false;
            },
            select: function (event, ui) {
                //var terms = split(this.value);           
                //terms.pop();
                //terms.push(ui.item.value);
                //terms.push("");
                //this.value = terms.join("");
                //return false;
                document.getElementById("auto_complex").value = ui.item.label;
                document.getElementById("ImageButton_buscar").click();
            }

            , minLength: 3, max: 10, scroll: true
        });
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

//ZONA RESPUESTA CORRESPONDENCIA

function asigna_datos_heig_with() {
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
        document.getElementById("Hidden_height").value = espacio_iframe - 30;
        document.getElementById("Hidden_width").value = with_frame - 30;

    }
    catch (err) {
        alert(err.message + " funcion asigna_datos_heig_with " + err.message);
    }
}
function promp_respuesta(mensaje) {
    try {
        var mensaje = confirm(mensaje);
        document.getElementById("Hidden_resp").value = "0";
        //Detectamos si el usuario acepto el mensaje
        if (mensaje) {
            document.getElementById("Hidden_resp").value = "1";
        }
        //Detectamos si el usuario denegó el mensaje
        else {
            document.getElementById("Hidden_resp").value = "0";
        }
    } catch (err) {
        alert(err.message + " funcion promp_respuesta " + err.message);
    }
}
function activa_boton_dowload_sube_plantilla() {
    try {

        document.getElementById("Button_sube_documento").click();
    }
    catch (err) {
        alert(err.message + " funcion activa_boton_dowload_sube_plantilla " + err.message);
    }
}
function activa_boton_dowload_adjunto() {
    try {

        document.getElementById("Button_sube_documento_adjunto_respuesta").click();
    }
    catch (err) {
        alert(err.message + " funcion activa_boton_dowload_adjunto " + err.message);
    }
}
function activa_boton_dowload_adjunto_simple() {
    try {

        document.getElementById("Button_sube_documento_adjunto_respuesta_simple").click();
    }
    catch (err) {
        alert(err.message + " funcion activa_boton_dowload_adjunto_simple " + err.message);
    }
}
function auto_zise_popup_respuesta() {
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
        $('#center_contenedor_respuesta').css("height", (espacio_iframe - 60) + "px");
        
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_respuesta");
    }
}
function Service_Actualiza_datos_gestion_solicitud(id_parameter_) {
    try {
        var resultado = valida_solicita_datos_control_general("form_control_gestion_edition");
        if (resultado !== "YES") {
            ESTADO_EVENT_GENERAL = "out";
            alert(resultado);
            return true;
        }
        var serialice = JSON.stringify(ITEM_GENERAL_CONTROL_ARRAY);
        $.ajax('../webservice/WebService_gestion_correspondencia.asmx/Service_Actualiza_datos_gestion_solicitud', {
            data: "{" + "'parameter':'" + serialice + "','id_parameter':'" + id_parameter_ + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_gestion !== "YES") {
                    alert(data.d[0].error_gestion);
                    ESTADO_EVENT_GENERAL = "out";
                } else {
                    actualiza_gre_campo("GridView_list_gestion_solicitud", id_parameter_, document.getElementById('TextBox_nota_gestion_edita').value, "GESTION");
                    $find("ModalPopupExtender_edition_editar_gestion_solicitud").hide();
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
            }, compelete: function () {

            }
        });
    } catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        alert(ex.message + " funcion Service_redirecciona_solicitud_a_entidades");
    }
}
function Service_solicita_descripcion_gestion_respuesta(id_registro) {
    try {
        ITEM_GENERAL_CONTROL_ARRAY = new Array();
        $.ajax('../webservice/WebService_gestion_correspondencia.asmx/Service_solicita_descripcion_gestion_respuesta', {
            data: "{" + "'parameter':'" + id_registro + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_gestion !== "YES") {
                    alert(data.d[0].error_gestion);
                    ESTADO_EVENT_GENERAL = "out";
                } else {      
                    ESTADO_EVENT_GENERAL = "out";
                    $.each(data.d, function (k, v) {
                        ITEM_GENERAL_CONTROL_ARRAY.push(v);
                    });
                    var resultado = Asigna_datos_control_interface("form_control_gestion_edition");
                    if (resultado == "YES") {
                        $find("ModalPopupExtender_edition_editar_gestion_solicitud").show();
                    } else {
                        alert(resultado);
                    }
                    document.getElementById("edit_gestion_soclicitud").setAttribute("idd", id_registro);
                    document.getElementById("edit_gestion_soclicitud").setAttribute("onclick", "prevent_lista_gestion(event,this)");
                    document.getElementById("edit_gestion_soclicitud").setAttribute("tip_event", 'e_e_g_r_s');
                   
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
            }, compelete: function () {

            }
        });
    } catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        alert(ex.message + " funcion Service_redirecciona_solicitud_a_entidades");
    }
}
function Service_elimina_registro_gestion_respuesta(id_registro) {
    try {
        ITEM_GENERAL_CONTROL_ARRAY = new Array();
        ITEM_GENERAL_CONTROL_ARRAY.push({ "name_campo_condicion": "id_tran", "value_campo_condicion": id_registro });
        var serialice = JSON.stringify(ITEM_GENERAL_CONTROL_ARRAY);
        $.ajax('../webservice/WebService_gestion_correspondencia.asmx/Service_eliminar_registra_gestion_respuesta', {
            data: "{" + "'parameter':'" + serialice + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_gestion !== "YES") {
                    alert(data.d[0].error_gestion);
                    ESTADO_EVENT_GENERAL = "out";
                } else {
                    ESTADO_EVENT_GENERAL = "out";
                    $("#" + "GridView_list_gestion_solicitud" + " tr[id=" + id_registro + "]").remove();
                    var row_count = document.getElementById("GridView_list_gestion_solicitud").rows.length - 1;
                    document.getElementById("titulo_label_list_gestion_solicitud").innerText = "Se encontro  " + row_count + " registro(s)";
                    
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
            }, compelete: function () {

            }
        });
    } catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        alert(ex.message + " funcion Service_redirecciona_solicitud_a_entidades");
    }
}
function Service_registra_gestion_respuesta() {
    try {
        var resultado = valida_solicita_datos_control_general("form_control_gestion");
        if (resultado !== "YES") {
            ESTADO_EVENT_GENERAL = "out";
            alert(resultado);
            return true;
        }
        var serialice = JSON.stringify(ITEM_GENERAL_CONTROL_ARRAY);
        $.ajax('../webservice/WebService_gestion_correspondencia.asmx/Service_registra_gestion_respuesta', {
            data: "{" + "'parameter':'" + serialice + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_gestion !== "YES") {
                    alert(data.d[0].error_gestion);
                    ESTADO_EVENT_GENERAL = "out";
                } else {         
                    $find("ModalPopupExtender_edition_gestion_respuesta_solicitud").hide();    
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
            }, compelete: function () {

            }
        });
    } catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        alert(ex.message + " funcion Service_redirecciona_solicitud_a_entidades");
    }
}
function Service_redirecciona_solicitud_a_entidades() {
    try {
        var resultado = valida_solicita_datos_control_general("form_control_traslado");
        if (resultado !== "YES") {
            ESTADO_EVENT_GENERAL = "out";
            alert(resultado);
            return true;
        }
        var serialice = JSON.stringify(ITEM_GENERAL_CONTROL_ARRAY);
        $.ajax('../webservice/WebService_gestion_correspondencia.asmx/Service_redirecciona_solicitud_a_entidades', {
            data: "{" + "'parameter':'" + serialice + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_gestion !== "YES") {
                    alert(data.d[0].error_gestion);
                    ESTADO_EVENT_GENERAL = "out";
                } else {
                    if (data.d[0].resultado_terminar_tarea == "YES") {
                        eliminar_fila_data_gred('data_grid_listado_solicitudes', 'hdnEmailID');
                        Decrementa_contador_tramites();
                        Actualiza_cantidad_barr_estado();
                    } else {
                        var id = document.getElementById("hdnEmailID").value;
                        actualiza_gre_campo('data_grid_listado_solicitudes', id, 'Traslado', 'ESTADO');
                        alert("Se traslado el tramite pero no se pudo enviar la tarea  a la actividad final  error (" + data.d[0].resultado_terminar_tarea + ")");
                    }
                    if (data.d[0].result_envio_correo !== "YES") {
                        alert("Se traslado el tramite pero no se pudo notificar al correo electrónico error (" + data.d[0].result_envio_correo + ")");
                    }
                    $find("ModalPopupExtender_edition_redirecciona_entidad_externa").hide();
                    $find("ModalPopup_respuesta_radicado").hide();
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
            }, compelete: function () {

            }
        });
    } catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        alert(ex.message + " funcion Service_redirecciona_solicitud_a_entidades");
    }
}
function Service_start_archiva_tramite(id_tarea) {
    try {
        $.ajax('../webservice/WebService_gestion_correspondencia.asmx/Service_start_inicio_gestio_correspondencia', {
            data: "{" + "'id_tarea':'" + id_tarea + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_gestion !== "YES") {
                    alert(data.d[0].error_gestion);
                    ESTADO_EVENT_GENERAL = "out";
                } else {
                    ITEMS_DATOS_TOKENIZE_8 = new Array();
                    $('.tokenize-callable-demo_respuesta_k').tokenize2().trigger('tokenize:clear');
                    document.getElementById("Hidden_id_respuesta").value = data.d[0].id_respuesta;
                    CORREO_GESTION = data.d[0].correo_electronico_envio;
                    document.getElementById("Hidden_token_tokenize_k").value = "";
                    if (CORREO_GESTION !== "") {
                        asig_correo_token_respuesta('tokenize-callable-demo_respuesta_k');     
                    }
                    var element_parent = $find("ModalPopupExtender_edition_confirma_respuesta");   
                    if (element_parent) {
                        element_parent.show();
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
            }, compelete: function () {

            }
        });
    } catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        alert(ex.message + " funcion Service_start_archiva_tramite");
    }
}
function Service_start_notifica_correo(id_tarea) {
    try {
        $.ajax('../webservice/WebService_gestion_correspondencia.asmx/Service_start_inicio_gestio_correspondencia', {
            data: "{" + "'id_tarea':'" + id_tarea + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_gestion !== "YES") {
                    alert(data.d[0].error_gestion);
                    ESTADO_EVENT_GENERAL = "out";
                } else {
                    ITEMS_DATOS_TOKENIZE_4 = new Array();
                    $('.tokenize-callable-demo_respuesta__').tokenize2().trigger('tokenize:clear');
                    document.getElementById("Hidden_id_respuesta").value = data.d[0].id_respuesta;
                    CORREO_GESTION = data.d[0].correo_electronico_envio;
                    document.getElementById("Hidden_text_user_correo").value = "";
                    toke_ini('tokenize-callable-demo_respuesta__');
                    
                    if (CORREO_GESTION !== "") {
                        asig_correo_token_respuesta('tokenize-callable-demo_respuesta__');
                    }
                    var element_parent = $find("ModalPopupExtender_edition_notifica_correo_respuesta");
                    if (element_parent) {
                        element_parent.show();
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
            }, compelete: function () {

            }
        });
    } catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        alert(ex.message + " funcion Service_start_notifica_correo");
    }
}
//SERVICIO INICIALIZA INTERFACE GESTION RESPUESTA

function Service_start_inicio_gestio_correspondencia(id_tarea) {
    try {
        $.ajax('../webservice/WebService_gestion_correspondencia.asmx/Service_start_inicio_gestio_correspondencia', {
            data: "{" + "'id_tarea':'" + id_tarea + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_gestion !== "YES") {
                    alert(data.d[0].error_gestion);
                    ESTADO_EVENT_GENERAL = "out";
                } else {
                    document.getElementById("Hidden_id_respuesta").value = data.d[0].id_respuesta;
                    document.getElementById("Hidden_radicado").value = data.d[0].radicado;
                    document.getElementById("label_title").innerText = data.d[0].title;
                    document.getElementById("Hidden_id_propietario_resp").value = data.d[0].id_remitente_interno;
                    document.getElementById("label_result").innerText = data.d[0].resultado_label;
                    document.getElementById("Hidden_obliga_rep").value = data.d[0].estado_obligatorio;
                    document.getElementById("Hidden_tipo_respuesta").value = data.d[0].estado_envio;
                    TIPO_ENVIO_RESPUESTA = data.d[0].estado_envio;
                    document.getElementById("Hidden_remitente_destinatario").value = data.d[0].id_remitente_externo;
                    document.getElementById("Image_estado_resp").src = data.d[0].url_image;
                    document.getElementById("Image_estado_resp_solo_confirm").src = data.d[0].url_image_electronica;
                    var dorop_list = document.getElementById("DropDownList_anexos_respuesta");
                    var dorop_list_simple = document.getElementById("DropDownList_anexos_respuesta_simple");
                    if (dorop_list) {
                        while (dorop_list.length > 0) {
                            dorop_list.remove(dorop_list.length - 1);
                        }
                    }
                    if (dorop_list_simple) {
                        while (dorop_list_simple.length > 0) {
                            dorop_list_simple.remove(dorop_list_simple.length - 1);
                        }
                    }
                    if (data.d[0].item_anexos) {
                    if (data.d[0].item_anexos.length !== 0) {
                        for (i = 0; i < data.d[0].item_anexos.length; i++) {
                            var option = document.createElement("option");
                            option.text = data.d[0].item_anexos[i].nombre_anexo;
                            option.value = data.d[0].item_anexos[i].id_anexo;
                            if (dorop_list) {
                                dorop_list.add(option);
                            }
                            var option_ = document.createElement("option");
                            option_.text = data.d[0].item_anexos[i].nombre_anexo;
                            option_.value = data.d[0].item_anexos[i].id_anexo;
                            if (dorop_list_simple) {
                                dorop_list_simple.add(option_);
                            }
                        }
                    }
                    }
                    if (data.d[0].estado_obligatorio == 1) {
                        document.getElementById("default_formal").click();
                       
                    } else {
                        if (data.d[0].radicado !== "") {
                            document.getElementById("default_formal").click();
                           
                        } else {
                            document.getElementById("default_confirmar").click();
                        }
                    }
                    
                    var element_parent = $find("ModalPopup_respuesta_radicado");
                    if (element_parent) {
                        element_parent.show();
                    }
                    CORREO_GESTION = data.d[0].correo_electronico_envio;        
                    //inicializa el control token respuesta simple
                    ITEMS_DATOS_TOKENIZE_6 = new Array();
                    $('.tokenize-callable-demo_respuesta_simple').tokenize2().trigger('tokenize:clear');
                    toke_ini('tokenize-callable-demo_respuesta_simple');
                     //asigna el correo eletronico del peticionarion al tokenize
                    asig_correo_token_respuesta('tokenize-callable-demo_respuesta_simple');
                    
                   //RESPONDE TRAMITE A CORREO ELECTRONICO PARA RESPUESTA FORMAL
                    ITEMS_DATOS_TOKENIZE_3 = new Array();
                    $('.tokenize-callable-demo_respuesta_').tokenize2().trigger('tokenize:clear');
                    toke_ini('tokenize-callable-demo_respuesta_');
                    //asigna el correo del usuario peticionario al token notifica correo eletronico.   
                    asig_correo_token_respuesta('tokenize-callable-demo_respuesta_');
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
            }, compelete: function () {

            }
        });
    } catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        alert(ex.message + " funcion Service_start_inicio_gestio_correspondencia");
    }
}
function Service_elimina_documento_respuesta(id_respuesta) {
    try {

        $.ajax('../webservice/WebService_gestion_correspondencia.asmx/Service_elimina_documento_respuesta', {
            data: "{" + "'id_respuesta':'" + id_respuesta + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_gestion !== "YES") {
                    alert(data.d[0].error_gestion);
                    ESTADO_EVENT_GENERAL = "out";
                } else {
                    document.getElementById("Image_estado_resp").src = data.d[0].url_image; 
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
            }, compelete: function () {

            }
        });
    } catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        alert(ex.message + " funcion Service_elimina_documento_respuesta");
    }
}
function Service_elimina_anexo_respuesta(id_respuesta, id_anexo) {
    try {

        $.ajax('../webservice/WebService_gestion_correspondencia.asmx/Service_elimina_anexo_respuesta', {
            data: "{" + "'id_respuesta':'" + id_respuesta + "','id_anexo':'" + id_anexo + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_gestion !== "YES") {
                    alert(data.d[0].error_gestion);
                    ESTADO_EVENT_GENERAL = "out";
                } else {
                    var dorop_list = document.getElementById("DropDownList_anexos_respuesta");
                    var dorop_list_simple = document.getElementById("DropDownList_anexos_respuesta_simple");
                    if (dorop_list) {
                        for (i = 0; i < dorop_list.length; i++) {
                            if (id_anexo == dorop_list[i].value) {
                                dorop_list.remove(i);
                            }
                        }            
                    }
                    if (dorop_list_simple) {
                        for (i = 0; i < dorop_list_simple.length; i++) {
                            if (id_anexo == dorop_list_simple[i].value) {
                                dorop_list_simple.remove(i);
                            }
                        }
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
            }, compelete: function () {

            }
        });
    } catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        alert(ex.message + " funcion Service_elimina_documento_respuesta");
    }
}
function Service_url_anexo_respuesta(id_anexo, name_file) {
    try {
        $.ajax('../webservice/WebService_gestion_correspondencia.asmx/Service_url_anexo_respuesta', {
            data: "{" + "'id_anexo':'" + id_anexo + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_gestion !== "YES") {
                    alert(data.d[0].error_gestion);
                    ESTADO_EVENT_GENERAL = "out";
                } else {
                    dowload_file(data.d[0].url_image, data.d[0].name_file);
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
            }, compelete: function () {

            }
        });
    } catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        alert(ex.message + " funcion Service_elimina_documento_respuesta");
    }
}
function Service_url_documento_respuesta(id_respuesta_radicado_, formato_, estado_firma_) {
    try {
        $.ajax('../webservice/WebService_gestion_correspondencia.asmx/Service_url_documento_respuesta', {
            data: "{" + "'id_respuesta_radicado':'" + id_respuesta_radicado_ + "','formato':'" + formato_ + "','estado_firma':'" + estado_firma_ + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_gestion !== "YES") {
                    alert(data.d[0].error_gestion);
                    ESTADO_EVENT_GENERAL = "out";
                } else {       
                    dowload_file(data.d[0].url_image, data.d[0].name_file);
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
            }, compelete: function () {

            }
        });
    } catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        alert(ex.message + " funcion Service_url_documento_respuesta");
    }
}
function event_responder_solicitud(id_respuesta_radicado_) {
    try {
        var estado_envia_correo_electronico_ = 0;
        if (document.getElementById("CheckBox_envio_correo_ra").checked == true) {
            estado_envia_correo_electronico_ = 1;
        }
        var estado_envia_ventanilla_ = 0;
        if (document.getElementById("CheckBox_envia_ventanilla_ra").checked == true) {
            estado_envia_ventanilla_ = 1;
        }
        var estado_firma_digital_ = 0;
        if (document.getElementById("CheckBox_firma_digital").checked == true) {
            estado_firma_digital_ = 1;
        }
        asig_array_tokenize("tokenize-callable-demo_respuesta_");
        var id_usuario_gestion_firma_ = document.getElementById("DropDownList_lista_firmas_confirma_respuesta").value;
        //var correo_electronico_envio_ = document.getElementById("Hidden_text_user_correo").value;
        var correo_electronico_envio_ = "";
        for (var i = 0; i < ITEMS_DATOS_TOKENIZE_3.length; i++) {
            if (i == 0) {
                correo_electronico_envio_ = ITEMS_DATOS_TOKENIZE_3[i].text;
            } else {
                correo_electronico_envio_ = correo_electronico_envio_ + "," + ITEMS_DATOS_TOKENIZE_3[i].text;
            }
        }
        if (estado_envia_correo_electronico_ == 1 && correo_electronico_envio_ == "") {
            alert("Debe informar el correo electrónico " );
            ESTADO_EVENT_GENERAL = "out";
            return true;   
        }
        //valida el correo elertrónico que tenga la estructura valida
        if (correo_electronico_envio_ != "") {
            for (var i = 0; i < ITEMS_DATOS_TOKENIZE_3.length; i++) {
                var ident = validator_cuenta_correo(ITEMS_DATOS_TOKENIZE_3[i].text);
                if (ident == false) {
                    alert("El correo electrónico  (" + ITEMS_DATOS_TOKENIZE_3[i].text + "), nos valido para envio de la respuesta");
                    ESTADO_EVENT_GENERAL = "out";
                    return true;
                }
            }
            //Valida la exitencia del correo inicial en el token
            let exitencia_token = 0;
            if (CORREO_GESTION != "") {
                for (var i = 0; i < ITEMS_DATOS_TOKENIZE_3.length; i++) {
                    if (CORREO_GESTION == ITEMS_DATOS_TOKENIZE_3[i].text) {
                        exitencia_token = 1;
                    }
                }
            }
            //Asigna correo pre registrado 
            if (CORREO_GESTION != "" && exitencia_token == 0) {
                var ident = validator_cuenta_correo(CORREO_GESTION);
                if (ident == true) {
                    correo_electronico_envio_ = correo_electronico_envio_ + "," + CORREO_GESTION;
                }
            }
        }
        if (document.getElementById("DropDownList_tipo_respuesta").value == "" || document.getElementById("DropDownList_tipo_respuesta").value == "0") {
            alert("Debe informar el tipo de respuesta ");
            document.getElementById("DropDownList_tipo_respuesta").focus();
            ESTADO_EVENT_GENERAL = "out";
            return true
        }
        var tipo_respuesta = "";
        var control_element_ = document.getElementById("DropDownList_tipo_respuesta");
        tipo_respuesta_ = control_element_.options[control_element_.selectedIndex].text;
        Service_Responder_a_la_solicitud(id_respuesta_radicado_, estado_envia_ventanilla_,
                                         estado_envia_correo_electronico_,
                                         estado_firma_digital_, id_usuario_gestion_firma_,
                                         correo_electronico_envio_, tipo_respuesta_);
    }
    catch (err) {
        alert("Funcion event_responder_solicitud " + err.mensaje);
        ESTADO_EVENT_GENERAL = "out";
    }
}
function Service_Responder_a_la_solicitud(id_respuesta_radicado_, estado_envia_ventanilla_, estado_envia_correo_electronico_,
                                          estado_firma_digital_, id_usuario_gestion_firma_,
    correo_electronico_envio_, tipo_respuesta_) {
    try {   
        $.ajax('../webservice/WebService_gestion_correspondencia.asmx/Service_Responder_a_la_solicitud', {
            data: "{'id_respuesta_radicado':" + "'" + id_respuesta_radicado_ + "'" + "," + "'estado_envia_ventanilla':'" + estado_envia_ventanilla_ + "','estado_envia_correo_electronico':'" + estado_envia_correo_electronico_ + "','estado_firma_digital':'" + estado_firma_digital_ + "','id_usuario_gestion_firma':'" + id_usuario_gestion_firma_ +
                "','correo_electronico_envio':'" + correo_electronico_envio_ + "','tipo_respuesta':'" + tipo_respuesta_ + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_gestion !== "YES") {
                    alert(data.d[0].error_gestion);
                    ESTADO_EVENT_GENERAL = "out";
                } else {
                    document.getElementById("Image_estado_resp").src = data.d[0].url_image;
                    document.getElementById("Image_estado_resp_solo_confirm").src = data.d[0].url_image_electronica;
                    var id = document.getElementById("hdnEmailID").value;
                    changue_row_font_weigh_light('data_grid_listado_solicitudes', document.getElementById("hdnEmailID").value);
                    actualiza_gre_campo('data_grid_listado_solicitudes', id, 'Tramitado', 'ESTADO');
                    if (data.d[0].result_envio_correo !== "YES") {
                        alert("El sistema registro la culminación de la gestión del tramite, pero no pudo noficar al correo electrónico por el siguiente error (" + data.d[0].result_envio_correo + ")");
                    }
                    $find("ModalPopupExtender_edition_confirma_envio_respuesta").hide();
                    $find("ModalPopup_respuesta_radicado").hide();
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
        alert('Service_Responder_a_la_solicitud  ' + ex.message);
    }
}
function Service_archiva_tramite_solicitud(correo_token_, confirma_correo_) {
    try {

        $.ajax('../webservice/WebService_gestion_correspondencia.asmx/Service_archiva_traite_solicitud', {
            data: "{'correo_token':" + "'" + correo_token_ + "'" + "," + "'confirma_correo':'" + confirma_correo_ + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_gestion !== "YES") {
                    alert(data.d[0].error_gestion);
                    ESTADO_EVENT_GENERAL = "out";
                } else {
                    if (data.d[0].resultado_terminar_tarea == "YES") {
                        eliminar_fila_data_gred('data_grid_listado_solicitudes', 'hdnEmailID');
                        Decrementa_contador_tramites();
                        Actualiza_cantidad_barr_estado();
                    } else {
                        var id = document.getElementById("hdnEmailID").value;
                        actualiza_gre_campo('data_grid_listado_solicitudes', id, 'Tramitado', 'ESTADO');
                        alert("Se archivo el tramite pero no se pudo enviar la tarea  a la actividad final  error (" + data.d[0].resultado_terminar_tarea + ")");
                    }
                    if (data.d[0].result_envio_correo !== "YES") {
                        alert("Se archivo el tramite pero no se pudo notificar al correo electrónico error (" + data.d[0].result_envio_correo + ")");
                    }
                    $find("ModalPopupExtender_edition_confirma_respuesta").hide();
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
        alert('Service_archiva_tramite_solicitud  ' + ex.message);
    }
}
function Service_finalizar_tramite(id_tarea_) {
    try {
        $.ajax('../webservice/WebService_gestion_correspondencia.asmx/Service_finalizar_tramite', {
            data: "{'id_tarea':" + "'" + id_tarea_ + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_gestion !== "YES") {
                    alert(data.d[0].error_gestion);
                    ESTADO_EVENT_GENERAL = "out";
                } else {
                    eliminar_fila_data_gred('data_grid_listado_solicitudes', 'hdnEmailID');
                    Decrementa_contador_tramites();
                    Actualiza_cantidad_barr_estado();
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
        alert('Service_finalizar_tramite  ' + ex.message);
    }
}
function Service_reasignar_tramite(id_tarea_, usuario_tokenize_) {
    try {
        $.ajax('../webservice/WebService_gestion_correspondencia.asmx/Service_reasigna_tramite', {
            data: "{'id_tarea':" + "'" + id_tarea_ + "','usuario_tokenize':'" + usuario_tokenize_ + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_gestion !== "YES") {
                    alert(data.d[0].error_gestion);
                    ESTADO_EVENT_GENERAL = "out";
                } else {
                    eliminar_fila_data_gred('data_grid_listado_solicitudes', 'hdnEmailID');
                    Decrementa_contador_tramites();
                    Actualiza_cantidad_barr_estado();
                    if (data.d[0].result_envio_correo !== "YES") {
                        alert("Se reasigno  el tramite pero no se pudo notificar al correo electrónico error (" + data.d[0].result_envio_correo + ")");
                    }
                    $find("ModalPopupExtender_edition_reasigna_tramite_usuario").hide();
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
        alert('Service_finalizar_tramite  ' + ex.message);
    }
}
//evento confirmar solicitud
function event_cofirmar_solicitud(id_respuesta_radicado_) {
    try {
        var estado_envia_correo_electronico_ = 0;
        if (document.getElementById("CheckBox_respuesta_confirmar").checked == true) {
            estado_envia_correo_electronico_ = 1;
        }
        //asig_array_tokenize('tokenize-callable-demo_respuesta_simple');
        var correo_electronico_envio_ = "";
        for (var i = 0; i < ITEMS_DATOS_TOKENIZE_6.length; i++) {
            if (i == 0) {
                correo_electronico_envio_ = ITEMS_DATOS_TOKENIZE_6[i].text;
            } else {
                correo_electronico_envio_ = correo_electronico_envio_ + "," + ITEMS_DATOS_TOKENIZE_6[i].text;
            }
        }
        if (estado_envia_correo_electronico_ == 1 && correo_electronico_envio_ == "") {
            alert("Debe informar el correo electrónico ");
            ESTADO_EVENT_GENERAL = "out";
            return true;
        }    
        var nota_confirma_ = document.getElementById("TextBox_nota_confirma").value;
        //valida el correo elertrónico que tenga la estructura valida
        if (correo_electronico_envio_ != "") {
            for (var i = 0; i < ITEMS_DATOS_TOKENIZE_6.length; i++) {
                var ident = validator_cuenta_correo(ITEMS_DATOS_TOKENIZE_6[i].text);
                if (ident == false) {
                    alert("El correo electrónico  (" + ITEMS_DATOS_TOKENIZE_6[i].text + "), nos valido para envio de la respuesta");
                    ESTADO_EVENT_GENERAL = "out";
                    return true;
                }
            }
            //Valida la exitencia del correo inicial en el token
            let exitencia_token = 0;
            if (CORREO_GESTION != "") {
                for (var i = 0; i < ITEMS_DATOS_TOKENIZE_6.length; i++) {
                    if (CORREO_GESTION == ITEMS_DATOS_TOKENIZE_6[i].text) {
                        exitencia_token = 1;
                    }
                }
            }
            //Asigna correo pre registrado 
            if (CORREO_GESTION != "" && exitencia_token == 0) {
                var ident = validator_cuenta_correo(CORREO_GESTION);
                if (ident == true) {
                    correo_electronico_envio_ = correo_electronico_envio_ + "," + CORREO_GESTION;
                }
            }
        }
       
        if (document.getElementById("Drop_tipo_respuesta").value == "" || document.getElementById("Drop_tipo_respuesta").value == "0") {
            alert("Debe informar el tipo de confirmación ");
            document.getElementById("Drop_tipo_respuesta").focus();
            ESTADO_EVENT_GENERAL = "out";
            return true
        }
        var tipo_respuesta = "";
        var control_element_ = document.getElementById("Drop_tipo_respuesta");
        tipo_respuesta_ = control_element_.options[control_element_.selectedIndex].text;
        Service_Confirma_recibido_de_la_solicitud(id_respuesta_radicado_, estado_envia_correo_electronico_,
            nota_confirma_, 
            correo_electronico_envio_, tipo_respuesta_);
    }
    catch (err) {
        alert("Funcion event_cofirmar_solicitud " + err.mensaje);
        ESTADO_EVENT_GENERAL = "out";
    }
}
function event_notifica_gestion_correo(id_respuesta_radicado_) {
    try {
        var estado_anexo_ = 0;
        if (document.getElementById("CheckBox_anexa_anexos").checked == true) {
            estado_anexo_ = 1;
        }
        asig_array_tokenize('tokenize-callable-demo_respuesta__');
        var correo_electronico_envio_ = document.getElementById("Hidden_text_user_correo").value;    
        if (correo_electronico_envio_ == "") {
            alert("Debe informar el correo electrónico ");
            ESTADO_EVENT_GENERAL = "out";
            return true
        }
        Service_notifica_gestion_correo_electronico(id_respuesta_radicado_, estado_anexo_,
            correo_electronico_envio_);
    }
    catch (err) {
        alert("Funcion event_cofirmar_solicitud " + err.mensaje);
        ESTADO_EVENT_GENERAL = "out";
    }
}
function Service_Confirma_recibido_de_la_solicitud(id_respuesta_radicado_, estado_envia_correo_electronico_, nota_confirma_, correo_electronico_envio_, tipo_respuesta_) {
    try {
        $.ajax('../webservice/WebService_gestion_correspondencia.asmx/Service_Confirma_recibido_de_la_solicitud', {
            data: "{'id_respuesta_radicado':" + "'" + id_respuesta_radicado_ + "','" + "estado_envia_correo_electronico':'" + estado_envia_correo_electronico_ + "','nota_confirma':'" + nota_confirma_ +
                "','correo_electronico_envio':'" + correo_electronico_envio_ + "','tipo_respuesta':'" + tipo_respuesta_ + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_gestion !== "YES") {
                    alert(data.d[0].error_gestion);
                    ESTADO_EVENT_GENERAL = "out";
                } else {
                    document.getElementById("Image_estado_resp").src = data.d[0].url_image;
                    document.getElementById("Image_estado_resp_solo_confirm").src = data.d[0].url_image_electronica;
                    var id = document.getElementById("hdnEmailID").value;
                    changue_row_font_weigh_light('data_grid_listado_solicitudes', document.getElementById("hdnEmailID").value);
                    actualiza_gre_campo('data_grid_listado_solicitudes', id, 'Tramitado', 'ESTADO');
                    if (data.d[0].result_envio_correo !== "YES") {
                        alert("El sistema registro la culminación de la gestión del tramite, pero no pudo noficar al correo electrónico por el siguiente error (" + data.d[0].result_envio_correo + ")");
                    }
                    $find("ModalPopup_respuesta_radicado").hide();
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
        alert('Service_Confirma_recibido_de_la_solicitud  ' + ex.message);
    }
}
function Set_Registra_solicitud_aprobacion(id_respuesta_radicado_) {
        try {
            if (ITEMS_DATOS_TOKENIZE_5.length == 0) {
                alert("Debe selecionar los usuarios a compartir");
                ESTADO_EVENT_GENERAL = "out";
                return false;
            }
            var valParam = JSON.stringify(ITEMS_DATOS_TOKENIZE_5);
            var para_meter_ca = new Array();
            var asunto_ = "";
            var nota_ = document.getElementsByName('TextBox_nota_aprobacion');
            var nivel_urgencia_solicitud_ = document.getElementsByName('DropDownList_prioridad_solicitud');
            var tipo_solicitud_ = "";
            var fecha_limite_ = document.getElementsByName("TextBox_fecha_limite_solicitud");
            var radicado_relacionado_ = "";
            var matri_documentos_ = "";
            para_meter_ca.push({ asunto_: asunto_, nota_: nota_[0].value, nivel_urgencia_solicitud_: nivel_urgencia_solicitud_[0].value, tipo_solicitud_: tipo_solicitud_, radicado_relacionado_: radicado_relacionado_, id_usuario_propietario_: id_respuesta_radicado_, matri_documentos_: matri_documentos_, fecha_limite_: fecha_limite_[0].value });
            var serialice = JSON.stringify(para_meter_ca);
            $.ajax('../webservice/WebServiceWorkflow.asmx/Set_Registra_solicitud_aprobacion', {
                data: "{'item_user':'" + valParam + "'," + "'parameter':'" + serialice + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d) {
                        var split = data.d.split("|");
                        if (split[0] !== "YES") {
                            alert(split[0]);
                            ESTADO_EVENT_GENERAL = "out";
                        } else {
                            var id = document.getElementById("hdnEmailID").value;
                            insert_link_lista_solicitudes('data_grid_listado_solicitudes', id, 'Solicitud por aprobación', 'ESTADO');
                            var element_parent = $find("ModalPopupExtender_actualizacion_anualidad");
                            if (element_parent) {
                                element_parent.hide();
                            }
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
        } catch (ex) {
            ESTADO_EVENT_GENERAL = "out";
            alert(ex.message + " funcion Set_Registra_solicitud_aprobacion");
        }
    }

function Service_notifica_gestion_correo_electronico(id_respuesta_radicado_, estado_anexo_,correo_electronico_envio_) {
    try {
        $.ajax('../webservice/WebService_gestion_correspondencia.asmx/Service_notifica_gestion_correo_electronico', {
            data: "{'id_respuesta_radicado':" + "'" + id_respuesta_radicado_ + "','" + "estado_anexo':'" + estado_anexo_ +
                "','correo_electronico_envio':'" + correo_electronico_envio_ + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_gestion !== "YES") {
                    alert(data.d[0].error_gestion);
                    ESTADO_EVENT_GENERAL = "out";
                } else {
                    document.getElementById("Image_estado_resp").src = data.d[0].url_image;
                    document.getElementById("Image_estado_resp_solo_confirm").src = data.d[0].url_image_electronica;
                    var element_parent = $find("ModalPopupExtender_edition_notifica_correo_respuesta");
                    if (element_parent) {
                        element_parent.hide();
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
        alert('Service_notifica_gestion_correo_electronico  ' + ex.message);
    }
}
function Service_activa_soicitud_aprobacion(id_respuesta_radicado_) {
    try {
        $.ajax('../webservice/WebService_gestion_correspondencia.asmx/Service_activa_soicitud_aprobacion', {
            data: "{'id_respuesta_radicado':" + "'" + id_respuesta_radicado_ + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_gestion !== "YES") {
                    alert(data.d[0].error_gestion);
                    ESTADO_EVENT_GENERAL = "out";
                } else {
                    var dorop_list = document.getElementById("DropDownList_prioridad_solicitud");
                    if (dorop_list) {
                        while (dorop_list.length > 0) {
                            dorop_list.remove(dorop_list.length - 1);
                        }
                    }
                    var option = document.createElement("option");
                    option.text = "Normal";
                    option.value = "1";
                    if (dorop_list) {
                        dorop_list.add(option);
                    }
                    var option_ = document.createElement("option");
                    option_.text = "Urgente";
                    option_.value = "2";
                    if (dorop_list) {
                        dorop_list.add(option_);
                    }
                    document.getElementById("TextBox_fecha_limite_solicitud").value = data.d[0].fecha_limite;
                    var element_parent = $find("ModalPopupExtender_actualizacion_anualidad");
                    if (element_parent) {
                        element_parent.show();
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
        alert('Service_notifica_gestion_correo_electronico  ' + ex.message);
    }
}
function Service_solicita_estado_tramite_tarea_workflow(id_tarea_) {
    try {
        $.ajax('../webservice/WebService_gestion_correspondencia.asmx/Service_solicita_estado_tramite_tarea_workflow', {
            data: "{'id_tarea':" + "'" + id_tarea_ + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_gestion !== "YES") {
                    alert(data.d[0].error_gestion);
                    ESTADO_EVENT_GENERAL = "out";
                } else {
                   
                    insert_link_lista_solicitudes('data_grid_listado_solicitudes', id_tarea_, data.d[0].estado_tramite, 'ESTADO');
                    var element_parent = $find("ModalPopupExtender_solicitud_aprobacion");
                    if (element_parent) {
                        element_parent.hide();
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
        alert('Service_solicita_estado_tramite_tarea_workflow  ' + ex.message);
    }
}
function Service_Reversa_gestion_tramite_usuario_autorizado(id_respuesta_radicado_, login_usuario_, pasword_usuario_, valid_) {
    try {
        $.ajax('../webservice/WebService_gestion_correspondencia.asmx/Service_Reversa_gestion_tramite_usuario_autorizado', {
            data: "{'id_respuesta_radicado':" + "'" + id_respuesta_radicado_ + "'" + "," + "'login_usuario':'" + login_usuario_ + "','pasword_usuario':'" + pasword_usuario_ + "','valid':'" + valid_ + "'}" ,
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_gestion !== "YES") {
                    alert(data.d[0].error_gestion);
                    ESTADO_EVENT_GENERAL = "out";
                } else {
                    document.getElementById("Image_estado_resp").src = data.d[0].url_image;
                    document.getElementById("Image_estado_resp_solo_confirm").src = data.d[0].url_image_electronica;
                    var id = document.getElementById("hdnEmailID").value;
                    changue_row_font_weigh_bold('data_grid_listado_solicitudes', document.getElementById("hdnEmailID").value);
                    actualiza_gre_campo('data_grid_listado_solicitudes', id, 'Por tramitar', 'ESTADO');
                    if (valid_ == 1) {
                        $find("ModalPopupExtender_edition_reversa_respuesta").hide();
                    }
                    if (valid_ == 0) {
                        $find("ModalPopupExtender_edition_confirma_reversa_respuesta").hide();
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
        alert('Service_Responder_a_la_solicitud  ' + ex.message);
    }
}

//--------------TERMINA ZONA RESPUESTA CORRESPONDENCIA
function activa_boton_cerrar() {
    document.getElementById("Button_valida_Cerrar_respuesta_radicado").click();
}
function eliminar_fila_data_gred(gred, nombre_hiden) {
    try {
        var idex = 0;
        $("#" + gred + " tr[id=" + $("#" + nombre_hiden).val() + "]").each(function () {
            idex = $(this)[0].rowIndex;
        })
        $("#" + gred + " tr[id=" + $("#" + nombre_hiden).val() + "]").remove();
        $('#' + nombre_hiden).val("-1");
        if (idex == 1) {
            auto_zise_popup_lista_solicitudes("1", "");
        } else {
    
        }
    }
    catch (err) {
        //alert(err.message + " Funcion eliminar_fila_data_gred");
    }

}
function actualiza_estado_tramite() {
    try {
        actualiza_gre_campo("data_grid_listado_solicitudes", document.getElementById('hdnEmailID').value, document.getElementById('Hidden_estado_tramite').value, "ESTADO")
    }
    catch (err) {
        alert(err.message + " funcion actualiza_estado_tramite " + err.message);
    }
}
function colum_index(colum_name, nombre_grid) {
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
        $('#TextBox_nota').css("height", (document.getElementById("contenido_procesa_nota_respuesta").clientHeight - 5) + "px");

    }
    catch (err) {
        alert(err.message + " funcion auto_zise_nota_tarea " + err.message);
    }
}
function auto_zise_popup_lista_tramites(value_lista_general, value_lista_usuario) {
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

        $('#div_contendor_principal').css("height", (espacio_iframe - 10) + "px");
        $('#div_contendor_principal').css("wdth", (with_frame - 10) + "px");
        $('#div_contendor_filtro_listado').css("height", ((document.getElementById("div_filtro__fil").clientHeight + 5)) + "px");
        var total = document.getElementById("div_contendor_filtro_listado").clientHeight + document.getElementById("navar_barra").clientHeight  ;
        var gridwith = with_frame - 10;
        var gridheihg_ = (espacio_iframe - (total + 20));
        $('#content_grid').css("height", gridheihg_ + "px");
        $('#Panel_principal').css("height", (gridheihg_) + "px");
      
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_lista_tramites " + err.message);
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
        var elment_heig = document.getElementById("content_option_chek_adjunto_doc_visor").clientHeight + document.getElementById("content_boton_adjunto_doc_visor").clientHeight  + document.getElementById("content_pie_title_adjunto_doc_visor").clientHeight + 20;
        $('#conten_file_element_adjunto_doc_visor').css("height", (document.getElementById("Div_contenido_adjunta").clientHeight - elment_heig) + "px");

    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_adjunta_documento_workflow " + err.message);
    }
}
function auto_zise_popup_adjunta_documento_respuesta() {
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
        $('#Panel_sube_documento_respuesta').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_sube_documento_respuesta').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        if (document.getElementById("modal_content_sube_documento_respuesta")) {
            $('#contenido_procesa_sube_documento_respuesta').css("height", (document.getElementById("modal_content_sube_documento_respuesta").clientHeight - (document.getElementById("divcabecer2_sube_documento_respuesta").clientHeight)) + "px");
        }
            //Para los modal que contiene gred
        var contenido_adjunta = 1;
        if (document.getElementById("Contenido_opcion_adjunta_respuesta")) {
            contenido_adjunta = document.getElementById("Contenido_opcion_adjunta_respuesta").clientHeight;
        }
        var elment_heig = contenido_adjunta + document.getElementById("content_boton_adjunto_doc_respuesta").clientHeight + document.getElementById("content_pie_title_adjunto_doc_respuesta").clientHeight + 20;
        $('#conten_file_element_adjunto_doc_respuesta').css("height", (document.getElementById("contenido_procesa_sube_documento_respuesta").clientHeight - elment_heig) + "px");

    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_adjunta_documento_respuesta " + err.message);
    }
}
function auto_zise_popup_adjunta_anexo_respuesta() {
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
        $('#Panel_sube_anexo_respuesta').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_sube_anexo_respuesta').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        if (document.getElementById("modal_content_sube_anexo_respuesta")) {
            $('#contenido_procesa_sube_anexo_respuesta').css("height", (document.getElementById("modal_content_sube_anexo_respuesta").clientHeight - (document.getElementById("divcabecer2_sube_anexo_respuesta").clientHeight)) + "px");
        }
        //Para los modal que contiene gred
        var contenido_adjunta = 1;
        if (document.getElementById("Contenido_opcion_adjunta_respuesta")) {
            contenido_adjunta = document.getElementById("Contenido_opcion_adjunta_respuesta").clientHeight;
        }
        var elment_heig = contenido_adjunta + document.getElementById("content_boton_adjunto_anexo_respuesta").clientHeight + document.getElementById("content_pie_title_adjunto_anexo_respuesta").clientHeight + 20;
        $('#conten_file_element_adjunto_anexo_respuesta').css("height", (document.getElementById("contenido_procesa_sube_anexo_respuesta").clientHeight - elment_heig) + "px");

    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_adjunta_anexo_respuesta " + err.message);
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
function auto_zise_popup_solicitud_aprobacion() {
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

    $('#Panel_solicitud_aprobacion').css("height", (espacio_iframe - 10) + "px");
    $('#contenido_procesa_solicitud_aprobacion').css("height", (espacio_iframe - 40) + "px");
    $('#Iframe_solicitud_aprobacion').css("height", (espacio_iframe - 45) + "px");
  
}
function auto_zise_popup_historico_tramite() {
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


        //$('#Panel_historico_tramite').css("height", (espacio_iframe - 10) + "px");
        //$('#Cotenedorpendiente_historico_tramite').css("height", (espacio_iframe - 15) + "px");
        $('#Iframe_historico_tramite_').css("height", (espacio_iframe - 40) + "px");
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_historico_tramite");
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
function auto_zise_popup_list_gestion_solicitud() {
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
        $('#Panel_list_gestion_solicitud').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_list_gestion_solicitud').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#contenido_procesa_list_gestion_solicitud').css("height", (document.getElementById("modal_content_list_gestion_solicitud").clientHeight - (document.getElementById("diver_cabcera_list_gestion_solicitud").clientHeight)) + "px");
        //Para los modal que contiene gred
        $('#content_data_grid_list_gestion_solicitud').css("height", (document.getElementById("contenido_procesa_list_gestion_solicitud").clientHeight - (document.getElementById("contenido_titulo_list_gestion_solicitud").clientHeight + 40)) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_list_gestion_solicitud " + err.message);
    }
}
function activa_export_lista(hiden_name, nombre_gred) {
    try {
        var hiden = document.getElementById(hiden_name);
        var nombre_gred;
        var x = $('#' + nombre_gred + ' th');
        var txt = "";
        var i;
        for (i = 1; i < x.length; i++) {
            txt = txt + x[i].innerText.toUpperCase() + "|";
        }
        hiden.value = txt;
        document.getElementById("Button_export_lista_event").click();
        return txt;
    }
    catch (err) {
        alert(err.message + " Funcion activa_export_lista");
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
    Sys.Extended.UI.Resources.AjaxFileUpload_FileList = "Lista de archivos a cargar:";
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
function firma_mecanica() {
    try {
        var heig_porcent = $("#noaming").attr("zon_heig");
        if (heig_porcent == 10) {
            alert("Debe aumentar el zoom de la imagen para agregar la firma");
            return;
        }
        $("#img").attr("src", $("#Hiddenintercambio2").val());
        $("#draggable").css("with", "100");
        $("#draggable").css("height", "70");
        $("#draggable").css("display", "block");
        $("#img").imageResize();
        var contenido = $("#content");
        var topconten = contenido.scrollTop();
        var lefconten = contenido.scrollLeft();
        if (topconten == 0) {
            var x = $("#zona").offset();
            topconten = x.top + 5;
            lefconten = x.left + 5;
        } else {
            var elmnt = document.getElementById("content");
            elmnt.scrollLeft = 1;
            elmnt.scrollTop = 1;
            var x = $("#zona").offset();
            topconten = x.top;
            lefconten = x.left;
        }
        $("#draggable").offset({ top: topconten, left: lefconten });
    }
    catch (err) {
        alert(err.message + " funcion firma_mecanica " + err.message);
    }
}
function limpiar_firma() {
    $("#draggable").css("display", "none");
}