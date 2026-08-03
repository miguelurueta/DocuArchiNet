
$(document).ready(function () {
    $.fn.inicio = function () {
        auto_zise_popup_lista_niveles_ocultos();
        auto_zise();
        auto_zise_popup_visor_externo();       
        auto_zise_popup_compartir_documento();
        actuo_zise_popup_compartir_correo_electronico();
        auto_zise_popup_radicador();      
        auto_zise_add_expediente();
        auto_zise_edit_expediente();
        auto_zise_editar_expediente();
        auto_zise_add_nivel();
        auto_zise_edit_nivel();
        auto_zise_permisos_nivel();
        auto_zise_lista_permisos_nivel();
        auto_zise_permisos_usuario_nivel();
        auto_zise_reasigna_expe_unidad();
        auto_zise_ubicacion_toponimica();
        auto_zise_popup_indice_expediente();
        auto_zise_popup_gestion_meta_dato_iframe();
        auto_zise_popup_adjunta_documento_workflow();
        auto_zise_popup_lista_form_control_person("actualiza_indice_batch_wf");
        //Agrega la selección a la lista selecionda en el data gred
        $('#data_grid_listado_permisos tr[id]').click(function () {
            $('#data_grid_listado_permisos tr[id]').css({ "background": "White", "color": "Black" });
            $(this).css({ "background-color": "#e8e8f7", "color": "Black" });      
        });    
    }
    $("#ocultaleft").click(function (e) {
        if ($("#Contentizquierdo").is(":hidden")) {
            $("#Contenedorderecho").css("width", "74%");
            $("#Contentizquierdo").css("width", "25%");
            $("#Contentizquierdo").show();
            return true;
        } else {
            $("#Contentizquierdo").hide();
            $("#Contentizquierdo").css("width", "1%");
            $("#Contenedorderecho").css("width", "99%");
            return true;
        }
    });

    $('#Paneltreview').contextMenu('context-menu', {

        'Salir del menú': {
            click: function (element) { },
            klass: "fad fa-times"
        },

        'Agrega Expediente': {
            click: function (element) {
                document.getElementById("Button_nueva_carpeta").click();
            },
            klass: "fas fa-folder-plus"
        },
        'Agrega Nivel ': {
        click: function (element) {
            document.getElementById("Button_activa_nuevo_nivel").click();
        },
        klass: "far fa-sign-in"
        },
        'Eliminar ': {
            click: function (element) {
                document.getElementById("Button_eliminar_carpeta").click();
            },
            klass: "fal fa-trash-alt"
        },
        'Editar': {
            click: function (element) {
                document.getElementById("Button_activa_actualizar_carpeta").click();
            },
            klass: "fal fa-pen-square"
        },
        'Cortar expediente': {
            click: function (element) {
               
                Get_cortar_expediente();
            },
            klass: "far fa-cut"
        },
        'Pegar expediente': {
            click: function (element) {
           
                Get_pegar_expediente();
            },
            klass: "fal fa-paste"
        },
        'Pegar archivos': {
        click: function (element) {
            event_multiple_row(event, 'data_grid', 'cop_file_service');
        },
        klass: "fad fa-paste"
        },
        'Compartir': {
            click: function (element) {
                document.getElementById("Button_activa_compartir_nivel").click();
            },
            klass: "fal fa-share"
        },
        'Listar permisos': {
            click: function (element) {
                document.getElementById("Button_activa_lista_permiso_compartidos_nivel").click();
            },
            klass: "fal fa-list-alt"
        },
        'Limpiar selección': {
            click: function (element) {
                //clear_seleccion_treview();
                search_padre_nodo();
            },
            klass: "fad fa-broom"
        },
        'Ocultar nivel': {
            click: function (element) {
                document.getElementById("Button_ocultar_nivel").click();
            },
            klass: "fal fa-minus-square"
        }
    });

    $('#Panel_unidad_treview_unidad').contextMenu('context-menu', {

        'Salir del menú': {
            click: function (element) { },
            klass: "fad fa-times"
        },

        'Cambiar nombre': {
            click: function (element) {
                document.getElementById("ButtonButtonEditar").click();
            },
            klass: "fal fa-file-edit"
        },
        'Eliminar': {
            click: function (element) {
                event_multiple_row(event, 'data_grid', 'el_service');
            },
            klass: "fad fa-times"
        },
        'Radicar': {
            click: function (element) {
                document.getElementById("ButtonRadicar").click();
            },
            klass: "fal fa-file-check"
        },
        'Visualizar': {
            click: function (element) {
                document.getElementById("ButtonVerDocumento").click();
            },
            klass: "fal fa-file"
        },
        'Copiar': {
            click: function (element) {
                //document.getElementById("Button_activa_copia_archivo").click();
                activa_copia_archivo('data_grid');
            },
            klass: "fal fa-paste"
        },
        'Pegar': {
            click: function (element) {
                event_multiple_row(event, 'data_grid', 'cop_file_service');
            },
            klass: "fad fa-paste"
        },
        'Ubicación': {
            click: function (element) {
                document.getElementById("Button_activa_ubicacion_archivo").click();
            },
            klass: "fad fa-search-location"
        }


    });
});
var ITEMS_DATOS_TOKENIZE_2 = new Array();  //GUARDA LOS ITEM SELECIONANDO TOKENIZE  SELECTOR
var NIVEL_JERARQUIA=0;
var EXPDIENTE_JERARQUIA = 0;
var ITEMS_DATOS = new Array();  //GUARDA LOS DATOS DE BUSQUEDA DEL TREVIEW
var SELECCION_TREVIEW_ID = "";
var SELECCION_TREVIEW_ID_SEL = "";
var SELECCION_TREVIEW_EXPEDIENTE_ID_SEL = ""; //GUARDA EL VALOR DEL NODO COPIADO QUE HACE REFERENCIA AL EXPEIDENTE A MOVER
var PARAMETER_COPIA_EXPEDIENTE = new Array();
var CONTADOR_NODE_TREVIEW_ID = 90000000000000;
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
        ini_event_page();
        window.addEventListener("resize", rezize_event);
        ShowModalPopup("ModalPopupExtender_mensaje_personalizado_backgroundElement", "Panel_mensaje_personalizado", 100001);
        ShowModalPopup("ModalPopupExtende_agregar_unidad_conservacion_popup_backgroundElement", "Panel_agregar_unidad_conservacion_popup", 100001);
        ShowModalPopup("ModalPopupExtender_edition_pro_gres_bar_backgroundElement", "Panel_pro_gres_bar", 100001);
        ShowModalPopup("ModalPopupExtender_edition_actualiza_indice_batch_wf_backgroundElement", "Panel_actualiza_indice_batch_wf", 100001);
        CargarDatos();
        document.getElementById("pit").innerHTML = "Expedientes " + EXPDIENTE_JERARQUIA + "  Niveles " + NIVEL_JERARQUIA;
        precarga_datos_busqueda();
        inicia_tokenize("tokenize-callable-demo1");
        $('.tokenize-callable-demo1').on('tokenize:tokens:added', function (e, value, text) {    
            ITEMS_DATOS_TOKENIZE_2.push({ text: text, value: value });
        });
        $('.tokenize-callable-demo1').on('tokenize:tokens:remove', function (e, value) {
            delete_array_tokenize(value);
        });   
        service_archivos_produccion("TextBox_buequeda_general");
        if (document.getElementById("Hidden0003").value != "") {
            document.getElementById("file_externo_copy").style.display = "block";
        }
    } catch (e) {
        alert(" funcion load " + e.message);
    }

});
const ini_event_page = () => {
    //Active process
    let array_element = new Array;
    array_element.push({ id: "a_update_idnex_btach" }, { id: "boton_event_actualiza_indice_batch_wf" }, { id: "btn_add_firmante" },
        { id: "Btn_solicitud_firma" }, { id: "a_exp_excel" }, { id: "a_load_file" }, { id: "ma_load_file"}
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
        delete_alert_boot(); 
        let name_ID = e.currentTarget.id;
        let result = "";
        switch (name_ID) {
            case "a_load_file":
                event_element_click_promise(e);
                break;
            case "ma_load_file":
                event_element_click_promise(e);
                break;
            case "a_update_idnex_btach":
                result = handler_show_form_control("data_grid", "dummychkstyle", "C-DW-ACTU-INDICE");
                if (result !== "YES") {
                    alert(result);
                }
                break;
            case "boton_event_actualiza_indice_batch_wf":
                result = handler_active_update_idex_batch(e);
                break;
            case "btn_add_firmante":
                event_element_click_promise(e);
                break;
            case "Btn_solicitud_firma":
                event_element_click_promise(e);
                break;
            case "a_exp_excel":
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
        e.currentTarget.disabled = true;
        posicion_update_pogres('progres_bar');
        if (name_control == "a_load_file" || name_control == "ma_load_file") {
            result = await ActivaCargaArchivos();
            if (result != "YES") {
                alert_bot(result, 'warning', "div_error_content_rad");
            }
        }
        if (name_control == "btn_add_firmante") {
            result = await InsertaFirmanteSolicitudFirma("table_list_firmantes_table");
            if (result != "YES") {
                alert_bot(result, 'warning', "error_content_solicitud_firma");
            }
        }
        if (name_control == "Btn_solicitud_firma") {
            result = await SolicitudFirmaElectronica();
            if (result != "YES") {
                alert_bot(result, 'warning', "error_content_solicitud_firma");
            }
        }
        if (name_control == "a_exp_excel") {
            result = await ActivaexportAspnetReporte("ExportAspNet", "data_grid","0","Documentos del Expediente");
            if (result != "YES") {
                alert_bot(result, 'warning', "div_error_content_rad");
            }
        }
    }
    catch (ex) {
        alert_bot(ex.message, 'warning', "div_error_content_rad");
    } finally {
        document.getElementById(name_control).disabled = false;
        progres_hiden('progres_bar');
    }
}
const ActivaexportAspnetReporte = async (NameService, NameTable, IndexHiden,NameReporte) =>  {
    try {
        let Result = "";
        /*** Dependendencia del archivo JSEsportaReportes.js */
        let _OPtionExportaReporte = ({
            NameService: NameService,
            NameTable: NameTable, IndexHiden: IndexHiden, NameReporte: NameReporte
        });
        /**** Exporta a excel el contenido de la tabla */
        Result = await JsExport(_OPtionExportaReporte);
        return Result;
        
    }
    catch (err) {
        return " Funcion ActivaexportAspnetReporte (" + err.message + ")" ;
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
function rezize_event() {
    try {
        auto_zise_popup_lista_niveles_ocultos();
        auto_zise();
        auto_zise_popup_compartir_documento();
        auto_zise_popup_visor_externo();
        actuo_zise_popup_compartir_correo_electronico();
        auto_zise_popup_radicador();
        auto_zise_add_expediente();
        auto_zise_edit_expediente();
        auto_zise_editar_expediente();
        auto_zise_add_nivel();
        auto_zise_edit_nivel();
        auto_zise_permisos_nivel();
        auto_zise_lista_permisos_nivel();
        auto_zise_permisos_usuario_nivel();
        auto_zise_reasigna_expe_unidad();
        auto_zise_ubicacion_toponimica();
        auto_zise_popup_indice_expediente();
        auto_zise_popup_gestion_meta_dato_iframe();
        auto_zise_popup_adjunta_documento_workflow();
        auto_zise_popup_lista_form_control_person("actualiza_indice_batch_wf");
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

function inicia_tokenize(name_tokenize) {
    try {
        $('.' + name_tokenize).tokenize2({
            placeholder: "Para relacionar los usuarios puede digitar el nombre del usuario o el cargo del usuario...",
            tokensMaxItems: 1,
            dataSource: function (search, object) {
                $.ajax('../webservice/WebServiceRadicacion.asmx/GetLista_usuarios_gestion_tokenize', {
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

        })
    } catch (ex) { alert("Funcion inicia_tokenize " + ex.message); }
}
function Compartir_nivel_tokenize() {
    try {
        if (ITEMS_DATOS_TOKENIZE_2.length == 0) {
            alert("Debe selecionar los usuarios a compartir");
            return false;
        }
        var valParam = ITEMS_DATOS_TOKENIZE_2[0].value;
        var para_meter_ca = new Array();
        var carga_archivo_ = 0;
        if (document.getElementById('CheckBox_cargar_archivo').checked == true) {
            carga_archivo_ = 1;
        };
        var descarga_archivo_ = 0;
        if (document.getElementById('CheckBox_descargar_archivo').checked == true) { descarga_archivo_ = 1; };
        var compartir_archivo_ = 0;
        if (document.getElementById('CheckBox_compartir_archivo').checked == true) { compartir_archivo_ = 1; };
        var elimiminar_archivo_ = 0;
        if (document.getElementById('CheckBox_eliminar_archivos').checked == true) { elimiminar_archivo_ = 1; };
        var radicar_archivo_ = 0;
        if (document.getElementById('CheckBox_radicar_archivo').checked == true) { radicar_archivo_ = 1; };
        var visualizar_archivo_ = 0;
        if (document.getElementById('CheckBox_visualizar_archivos').checked == true) { visualizar_archivo_ = 1; };
        var editar_expediente_ = 0;
        if (document.getElementById('CheckBox_cambia_nombre_expediente').checked == true) { editar_expediente_ = 1; };
        var eliminar_expediente_ = 0;
        if (document.getElementById('CheckBox_eliminar_expediente').checked == true) { eliminar_expediente_ = 1; };
        var agregar_expediente_ = 0;
        if (document.getElementById('CheckBox_agregar_expediente').checked == true) { agregar_expediente_ = 1; };
        var cambiar_nombre_archivo_ = 0;
        if (document.getElementById('CheckBox_cambiar_nombre_archivos').checked == true) { cambiar_nombre_archivo_ = 1; };
        var mover_expediente_ = 0;
        if (document.getElementById('CheckBox_mover_expediente').checked == true) { mover_expediente_ = 1; };
        var copiar_archivo_ = 0;
        if (document.getElementById('CheckBox_copiar_archivo').checked == true) { copiar_archivo_ = 1; };
        para_meter_ca.push({
            carga_archivo: carga_archivo_, descarga_archivo: descarga_archivo_, compartir_archivo: compartir_archivo_, elimiminar_archivo: elimiminar_archivo_,
            radicar_archivo: radicar_archivo_, visualizar_archivo: visualizar_archivo_, editar_expediente: editar_expediente_, eliminar_expediente: eliminar_expediente_
            , agregar_expediente: agregar_expediente_, cambiar_nombre_archivo: cambiar_nombre_archivo_, mover_expediente: mover_expediente_, copiar_archivo: copiar_archivo_});
        var serialice = JSON.stringify(para_meter_ca);
        $.ajax('../webservice/WebServiceRadicacion.asmx/Set_compartir_nivel', {
            data: "{'item_user':'" + valParam + "'," + "'parameter':'" + serialice + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d !== "YES") {
                    alert(data.d);
                } else { actualiza_node_treview("", "", "TreeViewArchivo", "../Gestion/imagenes/share-light.png"); document.getElementById("Button_cerrar_compartir_nivel").click(); }
            }
        });
    } catch (ex) { alert(ex.message + " funcion Compartir_documentos_tokenize"); }
}
function Get_cortar_expediente() {
    try {
        if (SELECCION_TREVIEW_ID === "") {
            alert("Debe seleccionar el expediente a cortar");
            return true;
        }
        SELECCION_TREVIEW_EXPEDIENTE_ID_SEL = "";
        $.ajax('../webservice/WebServiceProducion.asmx/Get_cortar_expediente', {
            data: "{'item_user':'" + "'" +  "}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d !== "YES") {
                    alert(data.d);
                    SELECCION_TREVIEW_EXPEDIENTE_ID_SEL = "";
                } else {
                    SELECCION_TREVIEW_EXPEDIENTE_ID_SEL = SELECCION_TREVIEW_ID;
                }
            }
        });
    } catch (e) {
        alert("funcion Get_cortar_expediente " + e.message);
    }
}
function Get_pegar_expediente() {
    try {
        if (SELECCION_TREVIEW_EXPEDIENTE_ID_SEL === "") {
            alert("Debe seleccionar el expediente a mover");
            return true
        }
        if (SELECCION_TREVIEW_ID === SELECCION_TREVIEW_EXPEDIENTE_ID_SEL) {
            alert("El nivel no puede ser el mismo a copiar");
            return true;
        }
        var request = $.ajax('../webservice/WebServiceProducion.asmx/Get_pegar_expediente', {
            data: "{'item_user':'" + "'" + "}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d) {
                    var d = JSON.parse(data.d);
                    if (d.result_ !== "YES") {
                        SELECCION_TREVIEW_EXPEDIENTE_ID_SEL = "";
                        alert(d.result_);
                    } else {
                        copia_parameter_node_trreeview_java();
                        eliminar_node_tree_view_java_select("TreeViewArchivo");
                        create_trenode_treview(d.valor_, PARAMETER_COPIA_EXPEDIENTE[0].nomb_ctrl_texto_, "trenode", "TreeViewArchivo", PARAMETER_COPIA_EXPEDIENTE[0].nomb_ctrl_ur_, "1", "");
                        SELECCION_TREVIEW_EXPEDIENTE_ID_SEL = "";
                    }
                }
            }
        });
        request.fail(function (jqXHR, textStatus) {
            alert("Request failed Get_pegar_expediente : " + textStatus);
        });
    } catch (e) {
        return null;
        alert("funcion Get_pegar_expediente " + e.message);
    } 
}
function preven_event_search_keypres_enter_search(e, sender) {
    try {

        tecla = (document.all) ? e.keyCode : e.which;
        if (tecla == 13) {
            //document.getElementById("Button_busca_general_archivo").click();
            e.preventDefault();

        }


    } catch (err) {
        alert(err.message + " funcion preven_event_search_keypres_enter " + err.message);
    }
}
function preven_event_search_keypres_enter(e, sender) {
    try {

        tecla = (document.all) ? e.keyCode : e.which;
        if (tecla == 13) {
            document.getElementById("Button_busca_general_archivo").click();
            e.preventDefault();

        }


    } catch (err) {
        alert(err.message + " funcion preven_event_search_keypres_enter " + err.message);
    }
}
function preven_event_search(event, e) {
    try {
        document.getElementById("Button_busca_general_archivo").click();
        event.preventDefault();
    }
    catch (err) {
        alert(err.message + " Funcion preven_event_search");
    }
}
/** Eventos carga de archivos  */
let CDproduccion;
const ActivaCargaArchivos = async () => {
    try {
        let Result = "";
        Result = await ServiceRESTSolicitaCargarDocumentoExpediente();
        if (Result != "YES") {
            return Result;
        }
        let _OPtionFileLoad = ({
            NameLoadProceso: "PRODUCCION",
            NameContenedorError: "error_content_adjunta_documeto_load_documento_006",
            funcion_name: "insert_row_producion_documental", evento_adjunta: "PRODUCCION",
            IdRespuestaIdExpediente: CDproduccion.CDexpedienteSeleccionado[0].IdExpediente,
            NameContendorLoadDocumento: "Contenedorderecho", ModalWidth: 75, CargaTipologia: 1,
            CargaFecha: 1, CargaPreview: 1, multi_select: "multiple",
            element_parent: "modal_adjunta_documeto_load_documento_006", TipoFormulario:1
        });
        Result = await IniLoadPerson(_OPtionFileLoad);
        return Result;
    } catch (ex) {
        return "Inconsistencia general funcion ActivaCargaArchivos " + ex.mensaje;
    }
}
/** */
const ServiceRESTSolicitaCargarDocumentoExpediente = async () => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceProducion.asmx/ServiceSolicitaCargarDocumentoExpediente', {
                data: "{" + "'parameter':'" + 0 + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].AppError !== "YES") {
                        resolve(data.d[0].AppError);
                    } else {
                        CDproduccion = data.d[0];
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
function operateFormatterRegistro() {
    return [
        '<a class="remove" href="javascript:void(0)" title="Eliminar firmante">',
        '<i class="far fa-user-times"></i>',
        '</a>'
    ].join('')
}
window.operateEventsRegistro = {
    'click .remove'(e, value, row) {
        delete_row_table("table_list_firmantes_table", "", row.Ident);
    }
}
const ShowInterfazSolicitudFirma = () => {
    try {
        delete_alert_boot();
        document.getElementById("NombreFirmante").value="";
        document.getElementById("EmailFirmante").value="";
        document.getElementById("IdenficacionFirmante").value="";
        let row = [];
        init_row_constant_table_boostrap_table("table_list_firmantes_table", row,
             "div_solicitud_firma_", "", "table-borderless","");
        $("#modal_content_solicitud_firma").modal("show");
    } catch (ex) {
        return ex.mensaje;
    }
}
const InsertaFirmanteSolicitudFirma = async (NameTable) => {
    try {
        let RowFirmante = {};
        let NameFirmante = document.getElementById("NombreFirmante").value;
        let EmailFirmante = document.getElementById("EmailFirmante").value;
        let IndetiFirmante = document.getElementById("IdenficacionFirmante").value;
        if (NameFirmante == "") {
            return "Debe infromar el nombre del firmante.";
        }
        if (EmailFirmante == "") {
            return "Debe infromar el email del firmante.";
        }
        if (IndetiFirmante == "") {
            return "Debe infromar la identificación del firmante.";
        }
        RowFirmante = new Object();
        RowFirmante["Nombre"] = NameFirmante;
        RowFirmante["Email"] = EmailFirmante;
        RowFirmante["Ident"] = IndetiFirmante;
        insert_row_table(NameTable, RowFirmante);
        document.getElementById("NombreFirmante").value = "";
        document.getElementById("EmailFirmante").value = "";
        document.getElementById("IdenficacionFirmante").value = "";
        return "YES";
    } catch (ex) {
        return ex.mensaje;
    }
}
const SolicitudFirmaElectronica = async () => {
    try {
        let totalRow = 0;
        totalRow = total_row_table("table_list_firmantes_table");
        if (totalRow == 0) {
            return "Seleccione los usuarios firmantes para la solicitud de firma electrónica";
        }
        return "El servicio de firma electrónica no está disponible para la cuenta de usuario. Por favor, verifique el convenio y el número de firmas disponibles."
    } catch (ex) {
        return ex.mensaje;
    }
}
function preven_event_restor_search(event, e) {
    try {

        var element_table = document.getElementById("Panelactividad_documentos");
        if (element_table) {
            element_table.style.display = "none";
        }
        var element_espiner = document.getElementById("id_tag_spiner");
        if (element_espiner) {
            element_espiner.style.display = "block";
        }
        document.getElementById(e.id).blur();
        if (SELECCION_TREVIEW_ID == "") { return true; }
        document.getElementById("Label_title_selecion").innerHTML = "Expediente (" + document.getElementById("HiddenField_rest_text_node").value + ")";
        document.getElementById("titulo_label_grid").innerHTML = "Cargando.....";
        document.getElementById("Button_Restaura_busqueda").click();      
        event.preventDefault();
    }
    catch (err) {
        alert(err.message + " Funcion preven_event_restor_search");
    } finally {
        if (element_espiner) {
            element_espiner.style.display = "none";
        }
    }
}
function activa_menu(clave) {
    try {
        if (clave == "a_c_f_500") {
            ShowInterfazSolicitudFirma();
        }
        if (clave == "c_a_exte_011") {
            event_multiple_row(event, 'data_grid', 'cop_file_service_expediente_produccion');
        }
        if (clave == "e_l_i_001") {      
            event_multiple_row(event, 'data_grid', 'el_service');       
        }
        if (clave == "r_d_i_002") {
            document.getElementById("ButtonRadicar").click();
        }

        if (clave == "b_a_a_003") {
            document.getElementById("Button_Activa_Agregar_archivo").click();
        }

        if (clave == "a_d_d_004") {
            document.getElementById("Button_Digitaliza").click();
        }

        if (clave == "c_n_a_005") {
            document.getElementById("ButtonButtonEditar").click();
        }

        if (clave == "d_a_s_005") {
            document.getElementById("ButtonDescarga").click();
        }
        if (clave == "a_c_d_006") {
            document.getElementById("Button_activa_compartir_documento").click();
        }
        if (clave == "n_c_e_007") {
            document.getElementById("Button_nueva_carpeta").click();
        }
        if (clave == "e_l_c_008") {
            document.getElementById("Button_eliminar_carpeta").click();
        }
        if (clave == "a_c_e_009") {
            document.getElementById("Button_activa_actualizar_carpeta").click();
        }
        if (clave == "v_d_s_010") {
            document.getElementById("ButtonVerDocumento").click();
        }
        if (clave == "r_b_s_011") {
            document.getElementById("Button_Restaura_busqueda").click();
        }
        if (clave == "a_d_p_012") {
            document.getElementById("Button_activa_descarga_dcoumento_plantila").click();
        }

        if (clave == "c_c_e_012") {
            asigna_id_seleccionados_cheked_general_hiden('data_grid', 'Hidden_sele_docu');
            document.getElementById("Button_notificar_envio").click();
        }
        if (clave == "n_n_o_009") {
            document.getElementById("Button_activa_nuevo_nivel").click();
        }
        if (clave == "n_n_d_009") {
            document.getElementById("Button_activa_eliminar_nivel").click();
        }
        if (clave == "n_n_u_009") {
            document.getElementById("Button_activa_lista_permiso_compartidos_nivel").click();
        }
        if (clave == "n_n_c_009") {
            document.getElementById("Button_activa_compartir_nivel").click();
        }
        if (clave == "e_l_x_008") {
            //document.getElementById("Button_cortar_elemento").click();
            Get_cortar_expediente();
        }
        if (clave == "e_l_p_008") {
            //document.getElementById("Button_mover_elemento").click();
            Get_pegar_expediente();
        }
        if (clave == "c_a_s_010") {
            //document.getElementById("Button_activa_copia_archivo").click();
            activa_copia_archivo('data_grid');
        }
        //Descargar todos los archivos seleccionados  
        if (clave == "w_f_m_p_012") {
            event_multiple_row(event, 'data_grid', 'dowload_produccion_file_exp');
        }
        //Pegar archivos a copiar
        if (clave == "p_a_s_010") {       
            event_multiple_row(event, 'data_grid', 'cop_file_service');
        }
        if (clave == "m_g_m_011") {
            document.getElementById("Button_activa_gestion_meta_dato").click();
            
        }
        if (clave == "n_l_n_010") {
            clear_seleccion_treview();
            return true;
        }
    } catch (err) {
        alert(err.message + " funcion activa_menu " + err.message);
    }
}
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
                    agrega_meta_dato_documento(ID_IMAGEN_META_DATO, GABINETE_META_DATO, RADICADO_META_DATO, ID_TAREA_META_DATO, 2, 1, 1, ID_BOTON_META_DATO);
                    return true;
                }
                //Actualiza indice batch
                if (e.id == "boton_event_actualiza_indice_batch_wf") {
                    ITEM_GENERAL_CONTROL_ARRAY_DIFERENT = new Array();
                    ITEM_GENERAL_CONTROL_ARRAY_DIFERENT = Detec_chanque_valor_campo();
                    if (ITEM_GENERAL_CONTROL_ARRAY_DIFERENT.length == 0) {
                        progres_hiden('progres_bar');
                        e.disabled = false;
                        clearInterval(INTERVAL_EVENT_GENERAL);
                        ESTADO_EVENT_GENERAL = "";
                        return true;
                    }
                    event_multiple_row("ModalPopupExtender_edition_actualiza_indice_batch_wf", "", "actualiza_indice_batch_production");
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
                //sube documentos en la lista
                if (evento == "C-DW-LISTA") {
                    inicializa_upload_file_client(tip_event);
                    parameter_upload(ESTADO_EVENT_GENERAL, "PRODUCCION", "Button_Activa_Agregar_archivo", "multiple", tip_event);
                    return true;
                }
              
                //--Crea meta dato y firma documento
                if (evento == "firma_doc_selecion_rad_") {
                    var spliter = tip_event.split("|");
                    ITEMS_DATOS_SIST_META_ARCHIVO = new Array();
                    ID_IMAGEN_META_DATO = spliter[1];
                    GABINETE_META_DATO = spliter[0];
                    RADICADO_META_DATO = spliter[2];
                    ID_TAREA_META_DATO = spliter[5];
                    ID_BOTON_META_DATO = spliter[6];
                    service_crea_interface_registro_meta_dato(spliter[1], spliter[0], spliter[4]);
                    return true;
                }
                //Crea interface actualiza indice
                if (evento == "C-DW-ACTU-INDICE") {
                    Service_interface_form_control(ITEMS_IMAGE_LIST_WF[0].id_item, "WebServiceDocuarchi.asmx", "Service_crea_interface_indice_produccion", "ModalPopupExtender_edition_actualiza_indice_batch_wf", "div_actualiza_indice_batch_wf", 1, "actualiza_indice_batch_wf", 1);
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
        //Adjunta documento lista con event interval
        if (value_sel == "C-DW-LISTA") {
            event_element_menu("C-DW-LISTA", "adjunto_doc_visor");
        }
  
    }
    catch (err) {
        alert(err.message + " Funcion inicializa_tipo_adjunto_documento");
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
        var nombre_tipo_documento = "" // Guarda nombre del tipo documento
        var estado_adjunto = 0; //Determina si el documento sube como adjunto 
        var element_parent = "";  //Guarda el nombre del modal que contiene el control upload
        var numero_documento_relacionado = 0;
        var element_isert_table = "wf";
        if (document.getElementById("Hidden_tip_adjunt")) {
            element_isert_table = document.getElementById("Hidden_tip_adjunt").value;
        }
        var imp_load = document.getElementById('file_element_' + CONTEN_NAME_UPLOAD_FILE);
        if (CONTEN_NAME_UPLOAD_FILE == "adjunto_doc_visor") {
            funcion_name = "insert_row_producion_documental";
            evento_adjunta = "GESTION_RESPUESTA";
            element_html_actuliza = "";
            element_update_panel = "Button_update_update_adjunto_doc_visor";
            drow_tipo = document.getElementById("DropDownList_adjunta_documento");
            if (drow_tipo.value != "") {
                var spl = drow_tipo.value.split("|")
                id_tipo_docuental = spl[0];
                nombre_tipo_documento = spl[4];
            }
            element_parent = "ModalPopupExtender_sube_documento_adjunto";
            star_copy_interval_file_Upload(estado_adjunto, estado_relacion, id_tipo_docuental, funcion_name, element_parent, evento_adjunta,
                numero_documento_relacionado, element_html_actuliza, element_update_panel, id_respuesta, tipo_adjunta, element_isert_table, nombre_tipo_documento, "", 0);
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
function create_trenode_treview(title, texto, tipo_node, nombre_tree, ur_img_node, move, node_parent_id) {
    try {
        var element_a_eleccion; //Referencia al elemento A seleccioando
        var element_table_parent;//Elemento que referencia la tabla padre del elemento A seleccionado
        var td_t;//Elemento que referencia el TD al que pertenece el elemento A sleccionado
        var element_div;//Elemento que referencia el div que contiene los nodos del elemento seleccionado
        var tempo_seleccion_treview_id = SELECCION_TREVIEW_ID;
        if (node_parent_id !== "") {
            SELECCION_TREVIEW_ID = node_parent_id;
        }
        if (SELECCION_TREVIEW_ID !== "") {

        //SELECCIONA EN EL DOM EL ELEMENTO A SELECCIONADO
         element_a_eleccion = document.getElementById(SELECCION_TREVIEW_ID);
         if (element_a_eleccion) {
         } else {
             alert("Imposible entrarl el nodo " + SELECCION_TREVIEW_ID + " funcion create_trenode_treview");
             return true;
         }
        //INCREMENTA EN CONTADOR DE NODOS
        CONTADOR_NODE_TREVIEW_ID = CONTADOR_NODE_TREVIEW_ID + 1;
        var element_temp = element_a_eleccion.parentElement;
        //SELECCIONA LA TABLA PADRE DEL ELEMENTO A SELECCIONADO
        while (element_temp.parentElement) {
            if (element_temp.parentElement.nodeName == "TABLE") {
                element_table_parent = element_temp.parentElement;         
                break;
            }
            element_temp = element_temp.parentElement;
        }
        if (element_table_parent===null) {
            alert("Impsoble encontrar la tabla padre, función create_trenode_treview");
            return true;
        }
        //UBICA EL TD DENTRO LA TABLA PADRE QUE TIENE EL ELEMENTO SELECCIONADO
        for (var i = 0; i < element_table_parent.rows.length; i++) {
            if (element_table_parent.rows[i].cells.length > 2) {
                td_t = element_table_parent.rows[i].cells;
                i = element_table_parent.rows.length;
                break;
            }
        }
        
        //CREA LA TABLA QUE ANIDA EL NUEVO NODO
        var element_table = document.createElement("TABLE");
        element_table.cellSpacing = 0;
        element_table.cellPadding = 0;
        element_table.style.borderWidth = 0;
        //CONSTRUYE LA IDENTIFICACIÓN DEL DIV DEL ELEMENTO SELECICONADO
        var id_numero_selccion_ = SELECCION_TREVIEW_ID.replace(nombre_tree + "t", "");
        var indet_elment_div = nombre_tree + "n" + id_numero_selccion_;
        indet_elment_div = indet_elment_div + "Nodes";
        element_div = document.getElementById(indet_elment_div);
        //CASO CREAR DIV CUANDO NO EXISTE EL DIV PARA EL NUEVO ELEMENTO 
        if (element_div) {

        } else {
            
            element_div = document.createElement("DIV");
            //ASIGNA LA IDENTIFICACION AL DIV A CREAR
            element_div.id = indet_elment_div;
            //AGREGA PRIMERA TABLA DE SEPARACION INICIAL AL DIV
            var element_table_separa = document.createElement("TABLE");
            element_table_separa.style.height = 0;
            var element_row_t_separe = element_table_separa.insertRow(0);
            var element_cel_t_separe = element_row_t_separe.insertCell(0);
            element_div.appendChild(element_table_separa);
            //AGREGA EL PRIMER TR A LA NUEVA TABLA QUE ANIDA EL NUEVO NODO
            var element_row = element_table.insertRow(0);
            var element_td_expan = element_row.insertCell(0);
            element_row.style.height = 0;
            element_div.appendChild(element_table);
            var td_ = td_t;
            var img_;
            var a_font;
            var pk = 0;
            //UBICA EL OBJETO IMG DONDE SE UBICA LA IMAGEN DE AXPAND
            element_table_parent.id = "hhhhhhhhhh";
            var element_img_expand = $("#" + element_table_parent.id + " img ");
            element_table_parent.id = "";
            if (element_img_expand) {
                var element_parent_img_expand = element_img_expand[0].parentElement;
                if (element_parent_img_expand.localName === "td") {
                    var element_a = document.createElement("A");
                    var id_numero_selccion = SELECCION_TREVIEW_ID.replace(nombre_tree + "t", "");
                    var identificador_element = nombre_tree + "n" + id_numero_selccion;
                    element_a.id = identificador_element;
                    element_a.href = "javascript:TreeView_ToggleNode(" + nombre_tree + "_Data," + id_numero_selccion + "," + "document.getElementById('" + identificador_element + "')," + "' '," + "document.getElementById('" + identificador_element + "Nodes'))";
                    element_img_expand[0].alt = "Contraer ";
                    element_img_expand[0].src = "../imagera/minus-square-light_1.png";
                    element_a.appendChild(element_img_expand[0]);
                    element_parent_img_expand.appendChild(element_a);
                } else {
                    element_div.style.display = "block";
                    element_img_expand[0].alt = "Contraer ";
                    element_img_expand[0].src = "../imagera/minus-square-light_1.png";
                }
            }
           
            //AGREGA SEGUNDA TABLA DE SEPARACION FINAL
            var element_table_separa_final = document.createElement("TABLE");
            element_table_separa_final.style.height = 0;
            var element_row_t_separe_final = element_table_separa_final.insertRow(0);
            var element_cel_t_separe_final = element_row_t_separe_final.insertCell(0);
            //AGREGA LA TABLA DE SEPARACION AL DIV QUE SE ESTA CREANDO
            element_div.appendChild(element_table_separa_final);
            //AGREGA EL DIV DESPUES DE LA TABLA PADRE
            var div_parent_table_parent = element_table_parent.parentElement;
            div_parent_table_parent.insertBefore(element_div, element_table_parent.nextSibling);
        }
        //CREA NUEVO ELEMENTO ANIDADO DENTRO DEL DIV
        if (element_div) {
            //Agrega la primera fila con una celda vacia
            var element_row = element_table.insertRow(0);
            var element_td_expan = element_row.insertCell(0);
            element_row.style.height = 0;
            //Agrega la segunda fila
            element_row = element_table.insertRow(1);
            //Agrega td de separacion
            var element_div_;
            var numero_td_arbol = td_t.length;
            numero_td_arbol = numero_td_arbol + 1
            if (numero_td_arbol > 3) {
                numero_td_arbol = numero_td_arbol - 3;
            }
            var conta_td;
            for (var i = 0; i < numero_td_arbol; i++) {
                element_td_expan = element_row.insertCell(i);
                element_div_ = document.createElement("DIV");
                element_div_.style.width = 10 + "px";
                element_div_.style.height = 1 + "px";
                element_td_expan.appendChild(element_div_);
                conta_td = i;
            }
            
            //Agrega td de link
            var ref_href = location.href;
            ref_href = ref_href.replace(location.origin, "");
            var split_location = ref_href.split("/");
            var tempo_rewsd = "/" + split_location[1] + "/WebResource.axd?d=70tBAniWvlvWV5u5hxF89H8ScIHzGwAwVA9LRsmYqPtfEM_U7U3yLxFGIGMrn5tt7XXDkPSzqFBKSkF2QdkuNiPm9wAs4JgBFnKeVYByIwe3tZFf4Y_2PGCju0aDo4wJ0&t=637075857357612364";
            conta_td = conta_td + 1;
            var element_td_expan = element_row.insertCell(conta_td);
            var element_img = document.createElement("IMG");
            element_img.src = "../imagera/plus_black.gif";
            element_td_expan.appendChild(element_img);
            //Agrega elemento td nodo  .     
            if (ur_img_node !== "") {
                conta_td = conta_td + 1;
                element_td_expan = element_row.insertCell(conta_td);
                element_img = document.createElement("IMG");
                element_img.src = ur_img_node
                element_td_expan.appendChild(element_img);    
            }
            //agreg td  title expan
            conta_td = conta_td + 1;
            element_td_expan = element_row.insertCell(conta_td);
            element_td_expan.classList.add(nombre_tree + "_2", nombre_tree + "_8", "nav-link_", "mt-1", "mb-1", "pl-1");
            element_td_expan.onmouseover = "TreeView_HoverNode(" + nombre_tree + "_Data, this)";
            element_td_expan.onmouseover = "TreeView_UnhoverNode(this)";
            element_a = document.createElement("A");
            element_a.classList.add(nombre_tree + "_0", nombre_tree + "_1", nombre_tree + "_2", "nav-link_", "mt-1", "mb-1", "pl-1");
            CONTADOR_NODE_TREVIEW_ID = CONTADOR_NODE_TREVIEW_ID + 1;
            element_a.href = "javascript:OnTreeNodeClicked('" + CONTADOR_NODE_TREVIEW_ID + "')";
            element_a.id = nombre_tree + "t" + CONTADOR_NODE_TREVIEW_ID;
            element_a.title = title;
            element_a.text = texto;
            element_td_expan.appendChild(element_a);         
            //AGREGA LA TABLA CON EL CONTENIDO DEL NODO ANTES DE LA ULTIMA TABLA DE SEPARACION
            if (move == 0) {
                var elemet_fisrt_child = element_div.lastChild;
                if (elemet_fisrt_child) {
                    element_div.insertBefore(element_table, elemet_fisrt_child)
                } else {
                    element_div.appendChild(element_table);
                }
            }
            if (move == 1) {
                if (element_div.firstChild) {
                    element_div.insertBefore(element_table, element_div.firstChild);
                } else {
                    element_div.appendChild(element_table);
                }
                //HABRE EL DIV CUANDO SE PEGA UN NUEVO ELEMENTO
                element_table_parent.id = "hhhhhhhhhh";
                var element_img_expand = $("#" + element_table_parent.id + " img ");
                element_table_parent.id = "";
                if (element_img_expand) {       
                    element_div.style.display = "block";
                    element_img_expand[0].alt = "Contraer ";
                    element_img_expand[0].src = "../imagera/minus-square-light_1.png";
                }
                
            }

            ITEMS_DATOS.push({ text: texto, id: element_a.id, value: texto });
            if (title.indexOf("|") !== -1) {
                EXPDIENTE_JERARQUIA++;
            } else {
                NIVEL_JERARQUIA++;
            }
            document.getElementById("pit").innerHTML = "Expedientes " + EXPDIENTE_JERARQUIA + "  Niveles " + NIVEL_JERARQUIA;         
            precarga_datos_busqueda();
            return true;
        }
       

    } else {
        //AGREGA UN NUEVO NIVEL
        element_a_eleccion = document.getElementById(nombre_tree);
        if (element_a_eleccion) {
            CONTADOR_NODE_TREVIEW_ID = CONTADOR_NODE_TREVIEW_ID + 1;
            var element_table = document.createElement("TABLE");
            element_table.cellSpacing = 0;
            element_table.cellPadding = 0;
            element_table.style.borderWidth = 0;
            //1- Agrega la primera fila con una celda vacia
            var element_row = element_table.insertRow(0);
            var element_td_expan = element_row.insertCell(0);
            element_row.style.height = 0;
            //2 -Agrega la segunda fila
            element_row = element_table.insertRow(1);
            element_td_expan = element_row.insertCell(0);
            var element_img = document.createElement("IMG");
            var ref_href = location.href;
            ref_href = ref_href.replace(location.origin, "");
            var split_location = ref_href.split("/");
            //var tempo_rewsd = "/" + split_location[1] + "/WebResource.axd?d=70tBAniWvlvWV5u5hxF89H8ScIHzGwAwVA9LRsmYqPtfEM_U7U3yLxFGIGMrn5tt7XXDkPSzqFBKSkF2QdkuNiPm9wAs4JgBFnKeVYByIwe3tZFf4Y_2PGCju0aDo4wJ0&t=637075857357612364";
            element_img.src = "../imagera/plus_black.gif";
            element_td_expan.appendChild(element_img);
            //Crea el segundo td
            element_td_expan = element_row.insertCell(1);
            var element_a = document.createElement("A");
            element_a.classList.add(nombre_tree + "_0", nombre_tree + "_1", nombre_tree + "_2");    
            element_a.href = "javascript:OnTreeNodeClicked('" + CONTADOR_NODE_TREVIEW_ID + "')";
            element_a.id = nombre_tree + "t" + CONTADOR_NODE_TREVIEW_ID + 'i';
            element_img = document.createElement("IMG");
            element_img.src = ur_img_node;
            element_img.style.borderWidth = 0;
            element_a.appendChild(element_img);
            element_td_expan.appendChild(element_a);
            //3- Crea el tercer td
            element_td_expan = element_row.insertCell(2);
            element_td_expan.classList.add(nombre_tree + "_2", nombre_tree + "_4", "nav-link_", "mt-1", "mb-1", "pl-1");
            element_td_expan.onmouseover = "TreeView_HoverNode(" + nombre_tree + "_Data, this)";
            element_td_expan.onmouseover = "TreeView_UnhoverNode(this)";
            element_a = document.createElement("A");
            element_a.classList.add(nombre_tree + "_0", nombre_tree + "_1", nombre_tree + "_3", "nav-link_", "mt-1", "mb-1", "pl-1");
            element_a.href = "javascript:OnTreeNodeClicked('" + CONTADOR_NODE_TREVIEW_ID + "')";
            element_a.id = nombre_tree + "t" + CONTADOR_NODE_TREVIEW_ID;
            element_a.title = title;
            element_a.text = texto;
            element_td_expan.appendChild(element_a);
            //Agrega ultIma fila de separacion
            element_row = element_table.insertRow(2);
            element_td_expan = element_row.insertCell(0);
            element_a_eleccion.appendChild(element_table);
            ITEMS_DATOS.push({ text: texto, id: element_a.id, value: texto });
            if (title.indexOf("|") !== -1) {
                EXPDIENTE_JERARQUIA++;
            } else {
                NIVEL_JERARQUIA++;
            }
            document.getElementById("pit").innerHTML = "Expedientes " + EXPDIENTE_JERARQUIA + "  Niveles " + NIVEL_JERARQUIA;
            precarga_datos_busqueda();
            return true;
        }
    }
    }
    catch (err) {
        alert(err.message + " Funcion create_trenode_treview ");
    } finally { SELECCION_TREVIEW_ID = tempo_seleccion_treview_id; }
}

function copia_parameter_node_trreeview_java() {
    try {
        var nomb_ctrl_title;
        var nomb_ctrl_texto;
        var nomb_ctrl_ur;
        var nomb_ctrl_id;
        var nombre_tree;
        if (SELECCION_TREVIEW_EXPEDIENTE_ID_SEL !== "") {
            var element_node_select = document.getElementById(SELECCION_TREVIEW_EXPEDIENTE_ID_SEL);
            if (element_node_select) {         
                    nomb_ctrl_title = element_node_select.title;
                    nomb_ctrl_texto = element_node_select.text;      
            }
            var element_parent_img = document.getElementById(SELECCION_TREVIEW_EXPEDIENTE_ID_SEL + "i");
            if (element_parent_img) {
                for (var i = 0; i < element_parent_img.childNodes.length; i++) {
                    if (element_parent_img.childNodes[i].nodeName == "IMG") {
                            nomb_ctrl_ur = element_parent_img.childNodes[i].src;  
                        break;

                    }
                }
            } else {
                var element_temp = document.getElementById(SELECCION_TREVIEW_EXPEDIENTE_ID_SEL);
                var element_parent;
                while (element_temp.parentElement) {
                    if (element_temp.parentElement.nodeName == "TABLE") {
                        element_parent = element_temp.parentElement;
                        break;
                    }
                    element_temp = element_temp.parentElement;
                }
                if (element_parent.firstChild) {
                    for (var i = 0; i < element_parent.firstChild.childNodes.length; i++) {
                        element_temp = element_parent.firstChild.childNodes[i];
                        if (element_temp.childNodes.length > 1) {
                            for (var z = 0; z < element_temp.childNodes.length; z++) {
                                var element_img = element_temp.childNodes[z];
                                for (var k = 0; k < element_img.childNodes.length; k++) {
                                    if (element_img.childNodes[k].nodeName == "IMG") {        
                                            nomb_ctrl_ur = element_img.childNodes[k].src;      
                                    }
                                }

                            }
                        }

                    }
                }

            }
            var nomb_ctrl_title;
            var nomb_ctrl_texto;
            var nomb_ctrl_ur;
            var nomb_ctrl_id;
            var nombre_tree;
            PARAMETER_COPIA_EXPEDIENTE.splice(0, 1);
            PARAMETER_COPIA_EXPEDIENTE.push({
                nomb_ctrl_title_: nomb_ctrl_title, nomb_ctrl_texto_: nomb_ctrl_texto, nomb_ctrl_ur_: nomb_ctrl_ur, nomb_ctrl_id_: nomb_ctrl_id,
                nombre_tree_: nombre_tree
            });
        }
    }
    catch (err) {
        alert(err.message + " copia_parameter_node_trreeview_java");
    }
}

function copia_parameter_node_trreeview(nomb_ctrl_title, nomb_ctrl_texto, nomb_ctrl_ur,nomb_ctrl_id, nombre_tree) {
    try {
        if (SELECCION_TREVIEW_ID !== "") {
            SELECCION_TREVIEW_ID_SEL = SELECCION_TREVIEW_ID;
            if (document.getElementById(nomb_ctrl_id)) {
                document.getElementById(nomb_ctrl_id).value = SELECCION_TREVIEW_ID;
            }
            var element_node_select = document.getElementById(SELECCION_TREVIEW_ID);
            if (element_node_select) {
          
                    if (document.getElementById(nomb_ctrl_title)) {
                        document.getElementById(nomb_ctrl_title).value = element_node_select.title;
                    }                               
                    if (document.getElementById(nomb_ctrl_texto)) {
                        document.getElementById(nomb_ctrl_texto).value = element_node_select.text;
                        
                    }    
            }
            
                var element_parent_img = document.getElementById(SELECCION_TREVIEW_ID + "i");
                if (element_parent_img) {
                    for (var i = 0; i < element_parent_img.childNodes.length; i++) {
                        if (element_parent_img.childNodes[i].nodeName == "IMG") {
                            if (document.getElementById(nomb_ctrl_ur)) {
                                document.getElementById(nomb_ctrl_ur).value = element_parent_img.childNodes[i].src;
                            }
                            break;
                           
                        }
                    }
                } else {
                    var element_temp = document.getElementById(SELECCION_TREVIEW_ID);
                    var element_parent;
                    while (element_temp.parentElement) {
                        if (element_temp.parentElement.nodeName == "TABLE") {
                            element_parent = element_temp.parentElement;
                            break;
                        }
                        element_temp = element_temp.parentElement;
                    }
                    if (element_parent.firstChild) {
                        for (var i = 0; i < element_parent.firstChild.childNodes.length; i++) {
                            element_temp = element_parent.firstChild.childNodes[i];
                            if (element_temp.childNodes.length > 1) {
                                for (var z = 0; z < element_temp.childNodes.length; z++) {
                                    var element_img = element_temp.childNodes[z];     
                                    for (var k = 0; k < element_img.childNodes.length; k++) {
                                        if (element_img.childNodes[k].nodeName == "IMG") {
                                            if (document.getElementById(nomb_ctrl_ur)) {
                                                document.getElementById(nomb_ctrl_ur).value = element_img.childNodes[k].src;
                                            }
                                        }
                                    }

                                }
                            }
                           
                        }
                    }
                    
                }
        }
    } 
    catch (err) {
        alert(err.message + " copia_parameter_node_trreeview");
    }
}
function actualiza_node_treview(title, texto, nombre_tree, ur_img_node) {
    try {
        if (SELECCION_TREVIEW_ID !== "") {
            var element_node_select = document.getElementById(SELECCION_TREVIEW_ID);
            if (element_node_select) {
                
                if (title !== "") {
                    element_node_select.title = title;
                }
                if (texto !== "") {
                    element_node_select.text = texto;
                    actualiza_array(SELECCION_TREVIEW_ID, texto);
                    precarga_datos_busqueda();
                }
            }
            if (ur_img_node !== "") {
                var element_parent_img = document.getElementById(SELECCION_TREVIEW_ID + "i");
                if (element_parent_img) {
                    for (var i = 0; i < element_parent_img.childNodes.length; i++) {
                        if (element_parent_img.childNodes[i].nodeName = "IMG") {
                            element_parent_img.childNodes[i].src = ur_img_node;
                            break;
                            return true;
                        }
                    }
                }
            }
        }
    }
    catch (err) {
        alert(err.message + " actualiza_node_treview");
    }
}
//SELECCION_TREVIEW_ID
function eliminar_node_tree_view_java(nombre_tree,nombre_treview, numero_expediente, numero_nivel) {
    try {
        if (SELECCION_TREVIEW_ID !== "") {
            var element_a_select = document.getElementById(SELECCION_TREVIEW_ID);
            //1. BUSCA LA TABLA PADRE DEL ELEMENTO (A) SELECCIONADO
            var element_table_padre_a;
            var element_temp = element_a_select.parentElement;
            while (element_temp.parentElement) {
                if (element_temp.nodeName == "TABLE") {
                    element_table_padre_a = element_temp;
                    break;
                } else {
                    if (element_temp.parentElement) {
                        element_temp = element_temp.parentElement;
                    } else { break; }
                }
            }

            //2. UBICA EL ELEMENTO DIV QUE PRECEDE AL ELEMENTO A SELECCIONADO
            var element_div_padre_tabla_padre = element_table_padre_a.parentElement;

            //3. ELIMINAR LA TABLA PADRE DEL ELEMENTO SELECICNADO
            element_div_padre_tabla_padre.removeChild(element_table_padre_a);

            //4. EL SIGUIENTE CODIGO SE EJECUTA SI EL CONTENDOR DIV DE LA TABLA PADRE NO TIENE HIJOS
            var tables_parent = $("#" + element_div_padre_tabla_padre.id + " table a");
            if (tables_parent.length === 0) {
                //4.1 UBICA EL ELEMENTO (A) DE LA TABLA PADRE DEL ELEMENTO DIV      
                var id_numero_selccion_ = element_div_padre_tabla_padre.id.replace("TreeViewArchivon", "TreeViewArchivot");
                id_numero_selccion_ = id_numero_selccion_.replace("Nodes", "");
                element_parent_a = document.getElementById(id_numero_selccion_);
                //4.2 ELIMINA EL ELEMENTO  DIV PADRE DE LA TABLA DEL NODO SELECCIONADO
                var element_parent_div_padre = element_div_padre_tabla_padre.parentElement;
                element_parent_div_padre.removeChild(element_div_padre_tabla_padre);
                if (element_parent_a) {
                    var element_temp = element_parent_a.parentElement;
                    var element_parent_a_table_parent;
                    while (element_temp.parentElement) {
                        if (element_temp.nodeName == "TABLE") {
                            element_parent_a_table_parent = element_temp;
                            break;
                        } else {
                            if (element_temp.parentElement) {
                                element_temp = element_temp.parentElement;
                            } else { break; }
                        }
                    }
                    //4.3 ELIMINA EL NODO PLUSS
                    if (element_parent_a_table_parent) {
                        element_parent_a_table_parent.id = "xxxxxxxxxxxxxxxxxxxxx";
                        var img_matri = $("#" + element_parent_a_table_parent.id + " img");
                        img_matri[0].src = "../imagera/plus_black.gif";
                        img_matri[0].alt = "";
                        element_parent_a_table_parent.id = "";
                    }
                }
            } else {
                if (numero_expediente > 0) {
                    EXPDIENTE_JERARQUIA = EXPDIENTE_JERARQUIA - numero_expediente;
                    EXPDIENTE_JERARQUIA++;
                }
                if (numero_nivel > 0) {
                    NIVEL_JERARQUIA = NIVEL_JERARQUIA - numero_nivel
                    NIVEL_JERARQUIA++;
                }
                var div_padre_ref = document.getElementById(SELECCION_TREVIEW_ID + "Nodes");
                if (div_padre_ref) {
                    parent_div_padre = div_padre_ref.parentElement;
                    parent_div_padre.removeChild(div_padre_ref);
                      
                }
            }
            delete_array(SELECCION_TREVIEW_ID);
            SELECCION_TREVIEW_ID = "";
            if (element_a_select.title.indexOf("|") !== -1) {
                EXPDIENTE_JERARQUIA--;
            } else {
                NIVEL_JERARQUIA--;
            }
            document.getElementById("pit").innerHTML = "Expedientes " + EXPDIENTE_JERARQUIA + "  Niveles " + NIVEL_JERARQUIA;
            document.getElementById("HiddenField_0003").value = "";
            document.getElementById("HiddenField_rest_text_node").value = "";
            document.getElementById("Label_title_selecion").innerHTML = "";
            document.getElementById("HiddenField_rest_0004").value = "";
            precarga_datos_busqueda();
            return true;
        }
    }
    catch (err) {
        alert(err.message + " Funcion eliminar_node_tree_view_java");
    }
}
function eliminar_node_tree_view_java_select(nombre_tree) {
    try {
        if (SELECCION_TREVIEW_EXPEDIENTE_ID_SEL !== "") {
            var element_a_select = document.getElementById(SELECCION_TREVIEW_EXPEDIENTE_ID_SEL);
            //1. BUSCA LA TABLA PADRE DEL ELEMENTO (A) SELECCIONADO
            var element_table_padre_a;
            var element_temp = element_a_select.parentElement;
            while (element_temp.parentElement) {
                if (element_temp.nodeName == "TABLE") {
                    element_table_padre_a = element_temp;
                    break;
                } else {
                    if (element_temp.parentElement) {
                        element_temp = element_temp.parentElement;
                    } else { break; }
                }
            }
            //2. UBICA EL ELEMENTO DIV QUE PRECEDE AL ELEMENTO A SELECCIONADO
            var element_div_padre_tabla_padre = element_table_padre_a.parentElement;

            //3. ELIMINAR LA TABLA PADRE DEL ELEMENTO SELECIONADO
            element_div_padre_tabla_padre.removeChild(element_table_padre_a);

            //4. EL SIGUIENTE CODIGO SE EJECUTA SI EL CONTENDOR DIV DE LA TABLA PADRE NO TIENE HIJOS
            var tables_parent = $("#" + element_div_padre_tabla_padre.id + " table a");
            if (tables_parent.length === 0) {
                //4.1 UBICA EL ELEMENTO (A) DE LA TABLA PADRE DEL ELEMENTO DIV      
                var id_numero_selccion_ = element_div_padre_tabla_padre.id.replace("TreeViewArchivon", "TreeViewArchivot");
                id_numero_selccion_ = id_numero_selccion_.replace("Nodes", "");
                element_parent_a = document.getElementById(id_numero_selccion_);
                //4.2 ELIMINA  EL ELEMENTO DIV PADRE DE LA TABLA DEL NODO SELECCIONADO
                var element_parent_div_padre = element_div_padre_tabla_padre.parentElement;
                element_parent_div_padre.removeChild(element_div_padre_tabla_padre);
                if (element_parent_a) {
                    var element_temp = element_parent_a.parentElement;
                    var element_parent_a_table_parent;
                    while (element_temp.parentElement) {
                        if (element_temp.nodeName == "TABLE") {
                            element_parent_a_table_parent = element_temp;
                            break;
                        } else {
                            if (element_temp.parentElement) {
                                element_temp = element_temp.parentElement;
                            } else { break; }
                        }
                    }
                    //4.3 ELIMINA EL NODO PLUSS
                    if (element_parent_a_table_parent) {
                        element_parent_a_table_parent.id = "xxxxxxxxxxxxxxxxxxxxx";
                        var img_matri = $("#" + element_parent_a_table_parent.id + " img");
                        img_matri[0].src = "../imagera/plus_black.gif";
                        img_matri[0].alt = "";
                        element_parent_a_table_parent.id = "";
                    }
                }
            }
            delete_array(SELECCION_TREVIEW_EXPEDIENTE_ID_SEL);
            SELECCION_TREVIEW_ID_SEL = "";
            if (element_a_select.title.indexOf("|") !== -1) {
                EXPDIENTE_JERARQUIA--;
            } else {
                NIVEL_JERARQUIA--;
            }
            document.getElementById("pit").innerHTML = "Expedientes " + EXPDIENTE_JERARQUIA + "  Niveles " + NIVEL_JERARQUIA;
            precarga_datos_busqueda();
            return true;
        } else {
            alert("funcion eliminar_node_tree_view_java_select imposible encontrar el nodo" + SELECCION_TREVIEW_EXPEDIENTE_ID_SEL);
        }
    } catch (e) {
        alert("funcion eliminar_node_tree_view_java_select " + e.message);
    }
}
function OnTreeNodeClicked(treenode) {
  
}
//--------------------------------ZONE UPDATE IDENX BATCH---------------------------------------------
let ITEMS_IMAGE_LIST_WF;
const handler_show_form_control = (name_table, name_class, event_name) => {
    try {
        let result = asing_chek_idex_batch(name_table);
        if (result !== "YES") {
            return result;
        }
        if (ITEMS_IMAGE_LIST_WF.length > 0) {
            event_element_menu(event_name, name_table);
            return "YES";
        } else {
            return "Debe seleccionar los elmentos para actuaizar indice";
        }
              
    } catch (ex) {
        return ex.mensaje;
    }
}
const asing_chek_idex_batch = (name_table) => {
    try {
        var exist;
        ITEMS_IMAGE_LIST_WF = new Array;
        $('#' + name_table + ' .dummychkstyle').each(function () {
            var nod = $(this);
            if (nod[0].children[0].checked == true) {
                var cel = $(this).parent().parent().parent();
                var atri = $(this).parent().parent().parent().attr("id");
                if (atri == undefined) {
                    atri = $(this).parent().parent().attr("id");
                    cel = $(this).parent().parent();
                    ITEMS_IMAGE_LIST_WF.push({
                        id_item: atri
                    });
                    
                }

                if (atri !== undefined && cel[0].display !== "none") {
                    atri = $(this).parent().parent().parent().attr("id");
                    ITEMS_IMAGE_LIST_WF.push({
                        id_item: atri
                    });
                    
                }
            }

        });
        return "YES";
    }
    catch (err) {
        return err.message;    
    }
}
const handler_active_update_idex_batch = (e) => {
    let vresult = valida_solicita_datos_control_general("form_control_indice_docuarchi");
    if (vresult != "YES") {
        alert_bot(vresult, 'warning', 'modal_content_actualiza_indice_batch_wf');
        return "YES";
    } else {
        event_element_clic("", e.currentTarget);
        return "YES";
    }
}
function verifi_existencia_array(id_sleccion) {
    try {
        if (ITEMS_IMAGE_LIST_WF.length == 0) {
            return "NO";
        }
        for (var i = 0; i <= ITEMS_IMAGE_LIST_WF.length; i++) {
            if (ITEMS_IMAGE_LIST_WF[i] == id_sleccion) {
                return "YES";
                break;
            }
        }
        return "NO";
    } catch (err) {
        alert(err.message + " Funcion verifi_existencia_array");
    }
}
//Asigna tamaño ventana editar expediente
function tamano_ventana_editar_expediente() {
    try {
        $("#Hiddenheigpagina").val(($("#contendor_principal").height()));
        var label = $('#Label_agregar_expdiente_popup');
        label[0].innerText = "Editar unidad documental";
        // $("#Iframe_agregar_expdiente_popup_").attr("src", "../gestion/FormGaAgregarExpediente.aspx")
    }
    catch (err) {
        alert(err.message + " Funcion tamano_ventana_editar_expediente");
    }
}
function auto_zise_editar_expediente() {
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

        $('#Panel_agregar_expdiente_popup').css("height", (espacio_iframe - 5) + "px");
        $('#Contenido_agregar_expdiente_popup').css("height", (espacio_iframe - 5) + "px");
        $('#Iframe_agregar_expdiente_popup_').css("height", (espacio_iframe - 5) + "px");
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_editar_expediente");
    }
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
function clear_seleccion_treview() {
    try {
        if (SELECCION_TREVIEW_ID !== "") {
            var old_element_selecion = document.getElementById(SELECCION_TREVIEW_ID);
            var old_element_selecion_parent = old_element_selecion.parentElement;
            var terms = ["0"];
            terms.push("0");
            old_element_selecion.classList.remove("node_select_java");
            old_element_selecion_parent.classList.remove("node_select_parent");
            old_element_selecion.style.color = "Black";
            document.getElementById("HiddenField_0003").value = "";
            document.getElementById("HiddenField_rest_text_node").vaule = "";
            SELECCION_TREVIEW_ID = "";
        }
    }
    catch (err) {
        alert(err.message + " Funcion clear_seleccion_treview");
    }
}
function OnkeyDown(event) {
    try {
    
        if (event.which == "27") {
            clear_seleccion_treview();
        }
    }
    catch (err) {
        alert(err.message + " Funcion OnkeyDown");
    }
}
function getOffset(el) {
  var rect = el.getBoundingClientRect();
    return {
        left: rect.left + window.scrollX,
        top: rect.top + window.scrollY
    };
}
function AuTospam(id_seleccion, nombre_tree_panel) {
    try {
        var element_select = document.getElementById(id_seleccion);
        while (element_select.parentElement) {
            var element_temp = element_select.parentElement;
            var id_element = 0;
            if (element_temp.nodeName == "DIV") {
                element_temp.style.display = "block";
                id_element = element_temp.id;
            }
            if (id_element !== 0) {
                id_element = id_element.replace("Nodes", "");
                var element_espam = document.getElementById(id_element);
                if (element_espam) {
                    var element_img = element_espam.firstChild;
                    if (element_img) {
                        element_img.src = "../imagera/minus-square-light_1.png";
                        element_img.alt = "Contraer ";
                    }
                }
            }
            
            element_select = element_temp;
        }
    } catch (err) { alert(err.message + " funcion AuTospam"); }
}

function retorna_tabla_padre(id_seleccion) {
    try {
    var element_select = document.getElementById(id_seleccion);
    while (element_select.parentElement) {
        var element_temp = element_select.parentElement;
        if (element_temp.nodeName == "TABLE") {
            return element_temp;
            break;
        }
        element_select = element_temp;
    }
  } catch (err) {
      alert(err.message + " Fucion retorna_tabla_padre")
  }
}
function search_parent_nodo() {
    try {
        if (SELECCION_TREVIEW_ID !== "") {
            var element_parent = retorna_tabla_padre(SELECCION_TREVIEW_ID);
            if (element_parent) {
                element_parent = element_parent.parentElement;
                var id_tre_padre = element_parent.id.replace("Nodes", "");
                id_tre_padre = id_tre_padre.replace("TreeViewArchivon", "TreeViewArchivot");
                return id_tre_padre;
            } else {
                alert("Imposible encontrar la tabla padre del nodo " + SELECCION_TREVIEW_ID);
                return "";
            }
        } else { return ""; }
    } catch (e) { alert("Funcion search_parent_nodo " + e.mensaje); }
}
function OnNodeClicked(event) {
    try {
        if (event.target.tagName === "A") {
            var matri_img = $("#" + event.target.id + " img");
            if (matri_img.length > 0) {
                return true;
            }
            var element_select = document.getElementById(event.target.id);
            var seleccion_unidad;
            if (element_select.title.indexOf("\\") !== -1) {
                seleccion_unidad = "Nivel";
                /*var id_padre = event.target.id.replace("TreeViewArchivot", "TreeViewArchivon");
                id_padre = id_padre + "Nodes";
                if (document.getElementById(id_padre)) {
                    var tabla_padre = retorna_tabla_padre(event.target.id);
                    if (tabla_padre) {
                        tabla_padre.id = id_padre + "kkkk";
                        var imagen;
                        var img_matri = $("#" + tabla_padre.id + " img");
                        if (img_matri) {
                            var src_ref = img_matri[0].src;
                            if (img_matri[0].alt.indexOf("Contraer") !== -1) {
                                src_ref = src_ref.replace("minus-square-light_1.png", "plus-square-light_1.png");
                                img_matri[0].alt = img_matri[0].alt.replace("Contraer", "Expandir");
                            } else {
                                src_ref = src_ref.replace("plus-square-light_1.png", "minus-square-light_1.png");
                                img_matri[0].alt = img_matri[0].alt.replace("Expandir", "Contraer");
                            }
                            img_matri[0].src = src_ref;
                        }
                        tabla_padre.id = "";
                    }
                    if (document.getElementById(id_padre).style.display === "block") {
                        document.getElementById(id_padre).style.display = "none";
                    } else {
                        document.getElementById(id_padre).style.display = "block";
                    }
                    
                }*/
              
            } else { seleccion_unidad = "Expediente"; }
        if (SELECCION_TREVIEW_ID !== "") {
            var old_element_selecion = document.getElementById(SELECCION_TREVIEW_ID);
            var old_element_selecion_parent;
            old_element_selecion_parent = retorna_parent_tr(old_element_selecion);     
            old_element_selecion.parentElement.classList.remove("node_select_parent");
            old_element_selecion_parent.classList.remove("node_select_parent_table");
        }
        var parentId = event.target.parentElement.id;
        var parent_element_select;
        parent_element_select = retorna_parent_tr(element_select);
        element_select.parentElement.classList.add("node_select_parent");
        parent_element_select.classList.add("node_select_parent_table");
        element_select.style.outline = 0;
        SELECCION_TREVIEW_ID = event.target.id;
        document.getElementById("HiddenField_0003").value = element_select.title;
        document.getElementById("HiddenField_rest_text_node").value = element_select.text;
        document.getElementById("Label_title_selecion").innerHTML = seleccion_unidad + " (" + element_select.text + ")";
        var element_espiner = document.getElementById("id_tag_spiner");
        if (element_espiner) {
            element_espiner.style.display = "block";
        }
        document.getElementById("titulo_label_grid").innerHTML = "Cargando   "  ;
        var element_table = document.getElementById("Panelactividad_documentos");
        if (element_table) {
            element_table.style.display = "none";
        }
        document.getElementById("Button_inline_trevie").click();
       
    } 
    return true;
    }
    catch (err) {
        alert(err.message + " Funcion OnNodeClicked");
       
    }
}
function OnSearchClick(id_seleccion, nombre_tree_panel) {
    try {    
        var element_select = document.getElementById(id_seleccion);
        var seleccion_unidad;
        if (element_select.title.indexOf("\\") !== -1) {
            seleccion_unidad = "Nivel";
            /*var id_padre = id_seleccion.replace("TreeViewArchivot", "TreeViewArchivon");
            id_padre = id_padre + "Nodes";
            if (document.getElementById(id_padre)) {
                var tabla_padre = retorna_tabla_padre(id_seleccion);
                if (tabla_padre) {
                    tabla_padre.id = id_padre + "kkkk";
                    var imagen;
                    var img_matri = $("#" + tabla_padre.id + " img");
                    if (img_matri) {
                        var src_ref = img_matri[0].src;
                        if (img_matri[0].alt.indexOf("Contraer") !== -1) {
                            src_ref = src_ref.replace("minus-square-light_1.png", "plus-square-light_1.png");
                            img_matri[0].alt = img_matri[0].alt.replace("Contraer", "Expandir");
                        } else {
                            src_ref = src_ref.replace("plus-square-light_1.png", "minus-square-light_1.png");
                            img_matri[0].alt = img_matri[0].alt.replace("Expandir", "Contraer");
                        }
                        img_matri[0].src = src_ref;
                    }
                    tabla_padre.id = "";
                }
                if (document.getElementById(id_padre).style.display === "block") {
                    document.getElementById(id_padre).style.display = "none";
                } else {
                    document.getElementById(id_padre).style.display = "block";
                }

            }*/

        } else { seleccion_unidad = "Expediente"; }
        if (SELECCION_TREVIEW_ID !== "") {
            var old_element_selecion = document.getElementById(SELECCION_TREVIEW_ID);
            var old_element_selecion_parent;
            old_element_selecion_parent = retorna_parent_tr(old_element_selecion);
            old_element_selecion.parentElement.classList.remove("node_select_parent");
            old_element_selecion_parent.classList.remove("node_select_parent_table");
        }
        var element_select = document.getElementById(id_seleccion);
        var parent_element_select;
        parent_element_select = retorna_parent_tr(element_select);
        element_select.parentElement.classList.add("node_select_parent");
        parent_element_select.classList.add("node_select_parent_table");
        element_select.style.outline = 0;
        SELECCION_TREVIEW_ID = id_seleccion;
        document.getElementById("HiddenField_0003").value = element_select.title;
        document.getElementById("HiddenField_rest_text_node").value = element_select.text;
        document.getElementById("Label_title_selecion").innerHTML = seleccion_unidad + " (" + element_select.text + ")";
        var element_espiner = document.getElementById("id_tag_spiner");
        if (element_espiner) {
            element_espiner.style.display = "block";
        }
        document.getElementById("titulo_label_grid").innerHTML = "Cargando   ";
        AuTospam(id_seleccion, nombre_tree_panel);
        var element_panel = document.getElementsByName(nombre_tree_panel);
        if (element_panel) {
            var tabla_padre = retorna_tabla_padre(SELECCION_TREVIEW_ID);
            if (tabla_padre) {
                var x = tabla_padre.offsetTop;
                var tope = x - (document.getElementById("menu_var").clientHeight + document.getElementById("box_ide").clientHeight + document.getElementById("title_treview").clientHeight);
                $("#" + nombre_tree_panel).scrollTop((tope));
                
            }
        }
        document.getElementById("Button_inline_trevie").click();
    } catch (err) {
        alert(err.message + " Fucion OnSearchClick")
    }
}
function retorna_parent_tr(element_select) {
    try {
        while (element_select.parentElement) {
            var element_temp = element_select.parentElement;
            if (element_temp.nodeName == "TABLE") {
                return element_temp;
                break;
            }
            element_select = element_temp;
        }
    } catch (err) {
        alert(err.message + " Fucion retorna_parent_tr " )
    }
}
//Función que permite que el boton que se agrega a la lista no envie el formulario  Button_activa_copia_archivo Button_copia_archivo
function prevent(event, element) {
    try {
        //Evita el posback del boton
        event.preventDefault();
        // Marca la liena seleccionada
        $('#data_grid tr[id]').css({ "background": "White", "color": "Black" });
        $('#data_grid tr[id]').each(function () {
            $(this).css({ "background-color": "#e8e8f7", "color": "Black" });
        });
        var g = element;
        var fer = $(element).attr("idd");
        //Asigna el parametro al hiden relacionado
        $('#hdnEmailID').val(fer);
        //Boton que ejecuta la acción del lado del servidor
        document.getElementById("Button_agrega_actividad_flujo_trabajo").click();
        element.focus();
    }
    catch (err) {
        alert(err.message + " Funcion prevent");
    }
}

function prevent_lista_nivel(event, element) {
    try {  
        event.preventDefault();  
        var fer = $(element).attr("idd");
        $('#Hidden_lista_niveles_ocultos').val(fer);
        document.getElementById("Button_muestra_nivel_oculto").click();
        element.focus();
    }
    catch (err) {
        alert(err.message + " Funcion prevent_lista_nivel");
    }
}
function prevent_lista_permisos(event, element) {
        try {

            var fer = $(element).attr("idd");
            var DAcampoCompara_ = $(element).attr("DAcampoCompara");
            var DAcampoActualiza_ = $(element).attr("DAcampoActualiza");
            var DNtable_ = $(element).attr("DNtable");
            var DNcamponivel_ = $(element).attr("DAcaponivel");
            var DNvalues_;
            var tip_event = $(element).attr("tip_event");
            if (tip_event == "edit") {
                $('#Hidden_sel').val(fer);
                document.getElementById("Button_activa_config_registro").click();
                event.preventDefault();
                element.focus();
            }
            if (tip_event == "elimina_registro") {
                $('#Hidden_sel').val(fer);
                document.getElementById("Button_eliminar_regi_permiso").click();
                event.preventDefault();
                element.focus();
            }
            if (tip_event == "edita_registro_event") {
                DNvalues_compara_ = fer;
                if (element.checked == true) {
                    DNvalues_ = 1;
                } else {
                    DNvalues_ = 0;
                }
                var obj = {};
                var jsonData = JSON.stringify(obj);
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "../webservice/WebServiceRadicacion.asmx/Getactualiza_service",
                    data: "{'DAcampoCompara':'" + DAcampoCompara_ + "'," + "'DAcampoActualiza':'" + DAcampoActualiza_ + "'," + "'DNtable':'" + DNtable_ + "'," + "'DNvalues':'" + DNvalues_ + "'," + "'DNvalues_compara':'" + DNvalues_compara_ + "'," + "'DAcaponivel':'" + DNcamponivel_ + "'}",
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
       
        }
        catch (err) {
            alert(err.message + " Funcion prevent");
        }
    }

    function preven_scrol_onmouseover(event, e) {
        try {
            e.style.cursor = "hand";
            e.style.cursor = "pointer";
        }
        catch (err) {
            alert(err.message + " Funcion preven_scrol_onmouseover");
        }
    }
    function prevent_scrol_ondblclick(event, e) {
        try {
       
            if (document.getElementById("hdnEmailID").value !== "-1") {
                document.getElementById("ButtonVerDocumento").click();
            }
        
            event.preventDefault();
        }
        catch (err) {
            alert(err.message + " Funcion prevent_scrol_ondblclick");
        }
    }
    function prevent_scrol_onclick_vi_image(event, e) {
        try {

            if (document.getElementById("hdnEmailID").value !== "-1") {
                document.getElementById("ButtonVerDocumento").click();
            }

            event.preventDefault();
        }
        catch (err) {
            alert(err.message + " Funcion prevent_scrol_onclick_vi_image");
        }
    }
    function prevent_scrol(event, e, val_element) {
        try {
        
            if (document.getElementById("hdnEmailID").value !== -1) {
                var tr_after = document.getElementById(document.getElementById("hdnEmailID").value);
                if (tr_after) {
                    tr_after.style.background = "White";
                    tr_after.style.color = "Black";
                }         
            }
            if (e.nodeName == "TR") {
                document.getElementById("hdnEmailID").value = e.id;
                e.style.background = "#e8e8f7";
                e.style.color = "Black";
            }
       
            if (e.nodeName == "TD") {
                document.getElementById("hdnEmailID").value = e.parentNode.id;
                e.parentNode.style.background = "#e8e8f7";
                e.parentNode.style.color = "Black";
            }
            if (e.nodeName == "A") {
                if (val_element == "vis") {
                    document.getElementById("hdnEmailID").value = $(e).attr("idd");
                    e.parentNode.parentNode.parentNode.style.background = "#e8e8f7";
                    e.parentNode.parentNode.parentNode.style.color = "Black";
                    document.getElementById("ButtonVerDocumento").click();
                }
                if (val_element == "dow") {
                    document.getElementById("hdnEmailID").value = $(e).attr("idd");
                    e.parentNode.parentNode.parentNode.style.background = "#e8e8f7";
                    e.parentNode.parentNode.parentNode.style.color = "Black";
                    document.getElementById("ButtonDescarga").click();
                }
                if (val_element == "del") {
                    document.getElementById("hdnEmailID").value = $(e).attr("idd");
                    e.parentNode.parentNode.parentNode.style.background = "#e8e8f7";
                    e.parentNode.parentNode.parentNode.style.color = "Black";
                    eliminar_file_unico_row_multiple(event, 'data_grid', document.getElementById("hdnEmailID").value);
                    
                }
                if (val_element == "fir") {
                    document.getElementById("hdnEmailID").value = $(e).attr("idd");
                    e.parentNode.parentNode.parentNode.style.background = "#e8e8f7";
                    e.parentNode.parentNode.parentNode.style.color = "Black";
                    var ref_id = $(e).attr("idd_rad") + "|" + e.id;
                    if (ref_id != "") {
                        var spliter = ref_id.split("|");
                        let confi = confirm("¿Desea firmar el documento (" + spliter[4] + ")?");
                        if (confi == false) {
                            return true;
                        }
                        if (spliter.length > 3) {
                            stamp_file_doument_genral(spliter[1], "aspnettable", spliter[0], e.id, e.id, "div_error_content_rad", "2", "fa-lock-alt");
                        } else {
                            alert("Inconsistencia en el evento, spliter incompleto (" + spliter.length + ")");
                        }
                    }
                   
                }
            }
            if (e.nodeName !== "svg") {
                if (e.className == "GridviewScrollItem_line_cort_tr_flex") {
                    e.classList.remove("GridviewScrollItem_line_cort_tr_flex");
                    e.classList.toggle("GridviewScrollItem_line_corte_tr_flex_scrol");
                } else {
                    e.classList.remove("GridviewScrollItem_line_corte_tr_flex_scrol");
                    e.classList.toggle("GridviewScrollItem_line_cort_tr_flex");
                }
            }
              
            event.preventDefault();
        }
        catch (err) {
            alert(err.message + " Funcion prevent_scrol");
        }
    }
    function asigna_id_seleccionados_cheked() {
        try {
            var fer = "0";
            $('#hdnEmailID_sel').val("0");
            $('#data_grid .dummychkstyle').each(function () {
                var nod = $(this);
                if (nod[0].children[0].checked == true) {
                    var cel = $(this).parent().parent().parent();
                    var atri = $(this).parent().parent().parent().attr("id");
                    if (atri == undefined) {
                        atri = $(this).parent().parent().attr("id");
                        cel = $(this).parent().parent();
                    }

                    if (atri !== undefined && cel[0].display !== "none") {
                        if (fer == "0") {
                            fer = atri;
                        } else {
                            fer = fer + "|" + atri;
                        }
                    }
                }

            });
            $('#hdnEmailID_sel').val(fer);
        }
        catch (err) {
            alert(err.message + " funcion asigna_usuario_grupos_cheked " + err.message);
        }
    }
    function desactiva_chek() {
        try {
            var x = document.getElementsByClassName("dummychkstyle");
            for (i = 0; i < x.length; i++) {
                var z = x[i];
                if (z !== null) {
                    z.checked = false;
                }

            }
        }
        catch (err) {
            alert(err.message + " funcion desactiva_chek " + err.message);
        }
    }
    function inactiva_chek() {
        //document.getElementById("hdnEmailID_VAL").value == "-1";
        //xd5("GridView_val_radicacion", "hdnEmailID_VAL");
    }
    function service_usuarios_gestion_text(name_texbox) {
        function split(val) {
            return val.split(/,\s*/);
        }
        function extractLast(term) {
            return split(term).pop();
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
                        url: "../webservice/WebServiceRadicacion.asmx/GetLista_usuarios_gestion",
                        data: "{'DName':'" + document.getElementById(name_texbox).value + "'}",
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
                    var terms = split(this.value);
                    // remove the current input
                    terms.pop();
                    // add the selected item
                    terms.push(ui.item.value);
                    // add placeholder to get the comma-and-space at the end
                    terms.push("");
                    this.value = terms.join("");
                    return false;
                }

                , minLength: 3, max: 10, scroll: true
            });
    }
    function service_archivos_produccion(name_texbox) {
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
                        url: "../webservice/WebServiceRadicacion.asmx/GetLista_documentos_produccion",
                        data: "{'DName':'" + document.getElementById(name_texbox).value + "'}",
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
                    //var terms = split(this.value);
                    // remove the current input
                    //terms.pop();
                    // add the selected item
                    //terms.push(ui.item.value);
                    // add placeholder to get the comma-and-space at the end
                    //terms.push("");
                    this.value = ui.item.value;
                    document.getElementById("TextBox_buequeda_general").value = ui.item.label;
                    document.getElementById("Button_busca_general_archivo").click();
                    return false;
                }

                , minLength: 3, max: 10, scroll: true
            });
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
                var scrollableDiv = $("#" + "Panel_unidad_treview_unidad");
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
    function Insert_row(name_table,nombre_documento, tipo_documento,fecha,id) {
        try
        {
            if ($('#' + name_table + ' td').children.length > 0 && $('#' + name_table + ' tr:visible').length > 0) {
                var table = document.getElementById(name_table);
                var tr = document.getElementById(name_table).getElementsByTagName("tr");
                var row = table.insertRow(table.rows.length);
                row.className = "GridviewScrollItem_line"
                row.setAttribute('id', id);
                var cell1 = row.insertCell(0);
                var cell2 = row.insertCell(1);
                var cell3 = row.insertCell(2);
                cell1.innerHTML = nombre_documento;
                cell2.innerHTML = fecha;
                cell3.innerHTML = tipo_documento;
                document.getElementById("Hidden0001").value = "";          
                $('#data_grid tr[id=' + id + ']').css({ "background-color": "#e8e8f7", "color": "Black" });
                document.getElementById("hdnEmailID").value ="-1";
                document.getElementById("titulo_label_grid").innerHTML = (table.rows.length - 1) + " archivo(s) encontrado(s)";
                document.getElementById("Button_actualiza_add_archivo").click();
                document.getElementById("Button_actualiza_table").click();
            }
        }
        catch (err) {
            alert(err.message + " Funcion Insert_row");
        }
    }
    function pront_confirmacion(mensaje) {
        try {
            var men = confirm(mensaje);
            if (men) {
                document.getElementById("HiddenField_botones_respuesta").value = "1";
            } else {
                document.getElementById("HiddenField_botones_respuesta").value = "0";
            }
        }
        catch (err) {
            alert(err.message + " funcion pront_confirmacion " + err.message);
        }
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
    function buton_click(buton) {
        try {
            document.getElementById(buton).click();
        }
        catch (err) {
            alert(err.message + " funcion buton_click " + err.message);
        }
    }
    function auto_zise_reasigna_expe_unidad() {
        try {
            var espacio_iframe;
            var hidenpadre;
            var with_frame;
            if (window.innerHeight) {
                //navegadores basados en mozilla 
                espacio_iframe = window.innerHeight
            } else {
                if (document.body.clientHeight) {
                    //Navegadores basados en IExplorer, es que no tengo innerheight 
                    with_frame = window.innerWidth;
                    espacio_iframe = window.innerHeight;
                } else {
                    //otros navegadores y iframe
                    //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val();

                    espacio_iframe = document.body.clientHeight;
                    with_frame = document.body.clientWidth;

                }
            }
            $("#Panel_reubicar_unidad_expediente_popup").css("height", (espacio_iframe - 40) + "px");
            $("#Contenido_reubicar_unidad_expediente_popup").css("height", (espacio_iframe - 40) + "px");
            var heigconetedor = 0;
            $("#drowlist_r_u_e").css("height", $("#DropDownListEntidadEmpresa_r_u_e").height() + "px");
            $("#contendor_botones_unidad_r_u_e").css("height", ($("#Button_reubicar").height() + 5) + "px");
            heigconetedor = $("#Panel_reubicar_unidad_expediente_popup").height() - ($("#drowlist_r_u_e").height() + $("#contendor_botones_unidad_r_u_e").height());
            $("#div_treview_archivo_r_u_e").css("height", (heigconetedor) + "px");
            $("#Paneltreview_r_u_e").css("height", (heigconetedor) + "px");

       
        }
        catch (err) {
            alert(err.message + " Funcion auto_zise_reasigna_expe_unidad");
        }
    }

    function acti_busq_lista(e, sender) {
        try {
       
            tecla = (document.all) ? e.keyCode : e.which;
            if (tecla == 13) {
                //document.getElementById("Button_buscar_lista").click();
                busqueda_gred('hdnEmailID', 'data_grid', 'TextBox_busqueda', 'CheckBox_busqueda');
           
                e.preventDefault();
                //return false;
            }
        

        } catch (err) {
            alert(err.message + " funcion acti_busq_lista " + err.message);
        

        }
    
    }
    function acti_busq_general_archivo(e, sender) {
        try {

            tecla = (document.all) ? e.keyCode : e.which;
            if (tecla == 13) {
                document.getElementById("Button_busca_general_archivo").click();      
                e.preventDefault();       
            }
        } catch (err) {
            alert(err.message + " funcion acti_busq_general_archivo " + err.message);
        }
    }
    function acti_busq_general_archivo_boton(e, sender) {
        try {

            document.getElementById("Button_busca_general_archivo").click();
            e.preventDefault();

        } catch (err) {
            alert(err.message + " funcion acti_busq_general_archivo " + err.message);
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

    function precarga_datos_busqueda() {
        try {
            $("#TextBox_busqueda_tre").autocomplete({
                maxResults: 20,
                source: function(request, response) {
                    var results = $.ui.autocomplete.filter(ITEMS_DATOS, request.term);
                    response(results.slice(0, this.options.maxResults));
                },
                select: function (event, ui) {          
                    OnSearchClick(ui.item.id, 'TreeViewArchivo');
                    return ui.item.id;        
                }, minLength: 1
            })
        }
        catch (err) {
            alert(err.message + " Funcion precarga_datos_busqueda");
        }
    }

    function delete_array(value_id) {
        try {
            for (var i = 0; i < ITEMS_DATOS.length; i++) {
                if (ITEMS_DATOS[i].id === value_id) {
                    ITEMS_DATOS.splice(i, 1);
                    return true;
                }
            }
        } catch (err) {
            alert(err.message + " Funcion delete_array");
        }
    }
    function actualiza_array(value_id, texto) {
        try {
            for (var i = 0; i < ITEMS_DATOS.length; i++) {
                if (ITEMS_DATOS[i].id == value_id) {
                    ITEMS_DATOS[i].text = texto;
                    ITEMS_DATOS[i].value = texto;
                    i = ITEMS_DATOS.length;
                }
            }
        } catch (err) {
            alert(err.message + " Funcion actualiza_array");
        }
    }
    function CargarDatos() {
        try {
            var treeN = $(".TreeN");
            if (treeN.length) {
                var nodo = $("#" + treeN[0].id + " a");
                var i=0;
                for (var index = 0; index < nodo.length; index++) {
                    if (nodo[index].innerText !== "") {
                        ITEMS_DATOS.push({ text: nodo[index].innerText, id: nodo[index].id, value: nodo[index].innerText });
                        if (nodo[index].title.indexOf("|") !== -1) {
                            EXPDIENTE_JERARQUIA++ ;
                        } else {
                            NIVEL_JERARQUIA++;
                        }
                        i++;
                    }
                   
                }
            }
            return ITEMS_DATOS;
        }
        catch (err) {
            alert(err.message + " Funcion CargarDatos");
        }
    }
    function actualiza_gre_campo(nombre_grid, id, valor_campo, nombre_campo) {
        try {
            $("#" + nombre_grid + " tr[id=" + id + "]").each(function () {
                var idex = -1;
                var name = nombre_campo;
                idex = colum_index(name, nombre_grid);
                if (idex != -1) {
                    if (valor_campo == "") {
                        var sas = $(this)[0].cells[idex];     
                        var trfirst = $('#' + nombre_grid + ' tr:first').next();
                        if (sas.childElementCount == 0) {
                            $(this)[0].cells[idex].innerText = "\u00a0";
                           
                        }
                        if (sas.childElementCount >= 1) {
                            sas.firstChild.innerHTML = "&nbsp;";        
                        }
                    }
                    if (valor_campo !== "") {
                        var trfirst = $('#' + nombre_grid + ' tr:first').next();
                        var sas = $(this)[0].cells[idex];
                        if (sas.childElementCount <= 0) {
                            $(this)[0].cells[idex].innerText = valor_campo;
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
    function eliminar_fila_data_gred_simple(gred, nombre_hiden, seter) {
        try {
       
            $("#" + gred + " tr[id=" + $("#" + nombre_hiden).val() + "]").remove();
            $('#' + nombre_hiden).val(seter);
            //actualiza el hiden seleccionado en el servidor
            document.getElementById("Button_actualiza_add_archivo").click();
        }
        catch (err) {
            alert(err.message + " Funcion eliminar_fila_data_gred_simple");
        }

    }
    function eliminar_fila_data_gred_simple_(gred, nombre_hiden, seter) {
        try {

            $("#" + gred + " tr[id=" + $("#" + nombre_hiden).val() + "]").remove();
            $('#' + nombre_hiden).val(seter);
        }
        catch (err) {
            alert(err.message + " Funcion eliminar_fila_data_gred_simple_");
        }

    }
    function auto_zise_popup_lista_niveles_ocultos() {
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
            $('#Panel_lista_niveles_ocultos').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
            $('#modal_content_lista_niveles_ocultos').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
            //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
            $('#contenido_procesa_lista_niveles_ocultos').css("height", (document.getElementById("modal_content_lista_niveles_ocultos").clientHeight - (document.getElementById("diver_cabcera_lista_niveles_ocultos").clientHeight)) + "px");
            //Para los modal que contiene gred
            $('#content_data_grid_lista_niveles_ocultos').css("height", (document.getElementById("contenido_procesa_lista_niveles_ocultos").clientHeight - (document.getElementById("contenido_titulo_lista_niveles_ocultos").clientHeight + 40)) + "px");
        }
        catch (err) {
            alert(err.message + " funcion auto_zise_popup_lista_niveles_ocultos " + err.message);
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

            $('#Panel_notifica_gestion').css("height", "50%");
            $('#contenido_procesa_notifica_gestion').css("height", (document.getElementById("Panel_notifica_gestion").clientHeight - 20) + "px");
            $('#Iframe_comparte_coreo').css("height", (document.getElementById("contenido_procesa_notifica_gestion").clientHeight - 5) + "px");
        }
        catch (ex) {
            alert("Incosistencia general función actuo_zise_popup_compartir_correo_electronico " + ex)
        }
    }
    function auto_zise_popup_lista_tareas(value_lista_general) {
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




            var gridwith = with_frame - 20;
            //var gridheihg = (espacio_iframe - 35) - suma_div;  contenedor_unidad_treview_unidad
            var gridheihg = document.getElementById("contenedor_unidad_treview_unidad").clientHeight + document.getElementById("Button_buscar_lista").clientHeight + 10 - 30
            //LLAMA PLUGIN FIJA HIDER O TITULOS   
            if (document.getElementById("Hidden0002").value == "1") {
                if ($('#data_grid td').children.length > 0 && $('#data_grid tr:visible').length > 0) {
                    // $(document).ready(function () { $('#data_grid').gridviewScroll({ width: gridwith, height: gridheihg }); })
                    //document.getElementById("data_grid").deleteRow(0);
                    document.getElementById("Hidden0002").value = "";
                }
            }
        }
        catch (err) {
            alert(err.message + " funcion auto_zise_popup_lista_tareas " + err.message);
        }
    }
    function auto_zise() {
        try {
            var espacio_iframe;
            var hidenpadre;
            var with_frame;
            if (window.innerHeight) {
                //navegadores basados en mozilla 
                espacio_iframe = window.innerHeight
            } else {
                if (document.body.clientHeight) {
                    //Navegadores basados en IExplorer, es que no tengo innerheight 
                    with_frame = window.innerWidth;
                    espacio_iframe = window.innerHeight;
                } else {
                    //otros navegadores y iframe
                    //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val();
                    espacio_iframe = document.body.clientHeight;
                    with_frame = document.body.clientWidth;

                }
            }

            $("#Contenedorderecho").css("height", ((espacio_iframe - document.getElementById("menu_var").clientHeight) - 20) + "px");
            $("#Contentizquierdo").css("height", ((espacio_iframe - document.getElementById("menu_var").clientHeight) - 20) + "px");
            $("#Panel_unidad_treview_unidad").css("height", (document.getElementById("Contenedorderecho").clientHeight - (document.getElementById("foter_estado").clientHeight + document.getElementById("foter_estado").clientHeight)) + "px");
            $("#div_treview_archivo").css("height", (document.getElementById("Contentizquierdo").clientHeight - (document.getElementById("title_treview").clientHeight + document.getElementById("bar_table").clientHeight + document.getElementById("contenido_pie").clientHeight)) + "px");
        
        }
        catch (err) {
            alert(err.message + " Funcion auto_zise");
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
            $('#Panel_visor_externo').css("height", (espacio_iframe - 5) + "px");
            $('#Cotenedorpendiente_visor_externo').css("height", (espacio_iframe - 5) + "px");
            $('#Iframe_visor_externo_da_').css("height", (espacio_iframe - 5) + "px");


        }
        catch (err) {
            alert(err.message + " Funcion actualiza_gre_campos_dinamicos");
        }
    }
    function auto_zise_popup_radicador() {
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
            $('#Panel_radica_interno').css("height", (espacio_iframe - 5) + "px");
            $('#Cotenedorpendiente_radica_interno').css("height", (espacio_iframe - 5) + "px");
            $('#Iframe_radica_interno_da_').css("height", (espacio_iframe - 5) + "px");

        }
        catch (err) {
            alert(err.message + " Funcion actualiza_gre_campos_dinamicos");
        }
    }
    function auto_zise_popup_compartir_documento() {
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
            $('#Panel_autoriza_compartir_documento').css("height", (espacio_iframe - 40) + "px");
            $('#contenido_procesa_autoriza_compartir_documento').css("height", (espacio_iframe - 40) + "px");
            $('#Iframe_compartir_documento_').css("height", (espacio_iframe - 50) + "px");

        }
        catch (err) {
            alert(err.message + " Funcion auto_zise_popup_compartir_documento");
        }
    }
    function auto_zise_add_expediente() {
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
            var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);
            $('#Panel_agregar_expediente_carpeta').css("height", (heig_porcent) + "px");
            $('#content_add_expediente').css("height", (heig_porcent - 10) + "px");
            $('#contenido_procesa_agregar_expediente_carpeta').css("height", (document.getElementById("content_add_expediente").clientHeight - (document.getElementById("divcabecer2_agregar_expediente_carpeta").clientHeight + document.getElementById("content_boton_add_expediente").clientHeight)) + "px");
        }
        catch (err) {
            alert(err.message + " Funcion auto_zise_add_expediente");
        }
    }
    function auto_zise_edit_expediente() {
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
                    //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val();  UpdatePanel_actualizar_expediente_carpeta

                }
            }
            //corrige el posicionamiento horizontal del modalpopuextender, la propiedad width del panel debe estar en auto
            var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);
            //var widtth_procent_left_rigth = (with_frame - document.getElementById("Panel_actualizar_expediente_carpeta").clientHeight) / 2;
            //$('#Panel_actualizar_expediente_carpeta').css("left", (Math.round(widtth_procent_left_rigth) ) + "px");
            $('#Panel_actualizar_expediente_carpeta').css("height", (heig_porcent) + "px");
            $('#modal_content_edita_expediente').css("height", (heig_porcent - 10) + "px");
            $('#contenido_procesa_actualizar_expediente_carpeta').css("height", (document.getElementById("modal_content_edita_expediente").clientHeight - (document.getElementById("divcabecer2_actualizar_expediente_carpeta").clientHeight + document.getElementById("modal-footer_edita_expediente").clientHeight)) + "px");
        }
        catch (err) {
            alert(err.message + " Funcion auto_zise_edit_expediente");
        }
    }
    function auto_zise_popup_indice_expediente() {
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
            $('#Panel_indice').css("height", (espacio_iframe - 40) + "px");
            $('#Cotenedorpendiente_indice').css("height", (espacio_iframe - 40) + "px");
            $('#Iframe_indice_').css("height", (espacio_iframe - 45) + "px");
        }
        catch (err) {
            alert(err.message + " Funcion auto_zise_popup_indice_expediente");
        }
    }
    function auto_zise_add_nivel() {
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
                    //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val();  UpdatePanel_actualizar_expediente_carpeta

                }
            }
            //corrige el posicionamiento horizontal del modalpopuextender, la propiedad width del panel debe estar en auto
            //var heig_porcent = espacio_iframe - ((espacio_iframe * 40) / 100);
            var widtth_procent_left_rigth = (with_frame - document.getElementById("Panel_agregar_nivel").clientHeight) / 2;
            $('#Panel_agregar_nivel').css("left", (Math.round(widtth_procent_left_rigth)) + "px");
       
        }
        catch (err) {
            alert(err.message + " Funcion auto_zise_add_nivel");
        }
    }
    function auto_zise_edit_nivel() {
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
                    //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val();  UpdatePanel_actualizar_expediente_carpeta

                }
            }
            //corrige el posicionamiento horizontal del modalpopuextender, la propiedad width del panel debe estar en auto
            //var heig_porcent = espacio_iframe - ((espacio_iframe * 40) / 100);
            var widtth_procent_left_rigth = (with_frame - document.getElementById("Panel_editar_nivel").clientHeight) / 2;
            $('#Panel_editar_nivel').css("left", (Math.round(widtth_procent_left_rigth)) + "px");

        }
        catch (err) {
            alert(err.message + " Funcion auto_zise_add_nivel");
        }
    }
    function auto_zise_permisos_nivel() {
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
                    //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val();  UpdatePanel_actualizar_expediente_carpeta

                }
            }
            //corrige el posicionamiento horizontal del modalpopuextender, la propiedad width del panel debe estar en auto
            var heig_porcent = espacio_iframe - ((espacio_iframe * 10) / 100);
            //var widtth_procent_left_rigth = (with_frame - document.getElementById("Panel_compartir_nivel").clientHeight) / 2;
            //$('#Panel_compartir_nivel').css("left", (Math.round(widtth_procent_left_rigth)) + "px");
            $('#Panel_compartir_nivel').css("height", (heig_porcent) + "px");
            $('#modal_content_compartir_nivel').css("height", (heig_porcent - 10) + "px");
            $('#contenido_procesa_compartir_nivel').css("height", (document.getElementById("modal_content_compartir_nivel").clientHeight - (document.getElementById("divcabecer2_compartir_nivel").clientHeight + document.getElementById("modal-footer_compartir_nivel").clientHeight)) + "px");
       
        }
        catch (err) {
            alert(err.message + " Funcion auto_zise_add_nivel");
        }
    }
    function auto_zise_permisos_usuario_nivel() {
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
                    //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val();  UpdatePanel_actualizar_expediente_carpeta

                }
            }
            //corrige el posicionamiento horizontal del modalpopuextender, la propiedad width del panel debe estar en auto
            var heig_porcent = espacio_iframe - ((espacio_iframe * 10) / 100);
            //var widtth_procent_left_rigth = (with_frame - document.getElementById("Panel_compartir_nivel").clientHeight) / 2;
            //$('#Panel_compartir_nivel').css("left", (Math.round(widtth_procent_left_rigth)) + "px");
            $('#Panel_lista_permisos_nivel').css("height", (heig_porcent) + "px");
            $('#modal_content_lista_permisos_nivel').css("height", (heig_porcent - 10) + "px");
            $('#contenido_procesa_lista_permisos_nivel').css("height", (document.getElementById("modal_content_lista_permisos_nivel").clientHeight - (document.getElementById("divcabecer2_lista_permisos_nivel").clientHeight )) + "px");

        }
        catch (err) {
            alert(err.message + " Funcion auto_zise_permisos_usuario_nivel");
        }
    }
    function auto_zise_lista_permisos_nivel() {
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
                    //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val();  UpdatePanel_actualizar_expediente_carpeta

                }
            }
            //corrige el posicionamiento horizontal del modalpopuextender, la propiedad width del panel debe estar en auto
            var heig_porcent = espacio_iframe - ((espacio_iframe * 10) / 100);
            //var widtth_procent_left_rigth = (with_frame - document.getElementById("Panel_compartir_nivel").clientHeight) / 2;
            //$('#Panel_compartir_nivel').css("left", (Math.round(widtth_procent_left_rigth)) + "px");  
            var pag = 50;
            var tr_paginacion = document.getElementsByClassName("id_sele_pagi");
            if (tr_paginacion.length > 0) {
                pag = tr_paginacion[0].clientHeight;
            }
            $('#Panel_listar_permisos_niveles').css("height", (heig_porcent) + "px");
            $('#modal_content_lista_permisos_niveles').css("height", (heig_porcent - 10) + "px");
            $('#contenido_procesa_listar_permisos_niveles').css("height", (document.getElementById("modal_content_lista_permisos_niveles").clientHeight - (document.getElementById("divcabecer2_listar_permisos_niveles").clientHeight )) + "px");
            $('#Panel_principal').css("height", (document.getElementById("contenido_procesa_listar_permisos_niveles").clientHeight - (document.getElementById("title_permisos").clientHeight + pag)) + "px");
       
        }
        catch (err) {
            alert(err.message + " Funcion auto_zise_lista_permisos_nivel");
        }
    }
    function auto_zise_reasigna_expe_unidad() {
        try {
            var espacio_iframe;
            var hidenpadre;
            var with_frame;
            if (window.innerHeight) {
                //navegadores basados en mozilla 
                espacio_iframe = window.innerHeight
            } else {
                if (document.body.clientHeight) {
                    //Navegadores basados en IExplorer, es que no tengo innerheight 
                    with_frame = window.innerWidth;
                    espacio_iframe = window.innerHeight;
                } else {
                    //otros navegadores y iframe
                    //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val();

                    espacio_iframe = document.body.clientHeight;
                    with_frame = document.body.clientWidth;

                }
            }




            //corrige el posicionamiento horizontal del modalpopuextender, la propiedad width del panel debe estar en auto
            //var widtth_procent_left_rigth = (with_frame - document.getElementById("Panel_agregar_expediente_carpeta").clientHeight) / 2;
            //$('#Panel_agregar_expediente_carpeta').css("left", (Math.round(widtth_procent_left_rigth)) + "px");
            var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
            $('#Panel_reubicar_unidad_expediente_popup').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
            $('#modal_content_reubicar_unidad_expediente_popup').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
            //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
            $('#Contenido_reubicar_unidad_expediente_popup').css("height", (document.getElementById("modal_content_reubicar_unidad_expediente_popup").clientHeight - (document.getElementById("divcabecer_reubicar_unidad_expediente_popup").clientHeight + document.getElementById("contendor_botones_unidad_r_u_e").clientHeight)) + "px");
            //Para los modal que contiene gred
            $('#div_treview_archivo_r_u_e').css("height", (document.getElementById("Contenido_reubicar_unidad_expediente_popup").clientHeight - document.getElementById("drowlist_r_u_e").clientHeight) + "px");
        }
        catch (err) {
            alert(err.message + " Funcion auto_zise_reasigna_expe_unidad");
        }
    }
    function auto_zise_ubicacion_toponimica() {
        try {
            var espacio_iframe;
            var hidenpadre;
            var with_frame;
            if (window.innerHeight) {
                //navegadores basados en mozilla 
                espacio_iframe = window.innerHeight
            } else {
                if (document.body.clientHeight) {
                    //Navegadores basados en IExplorer, es que no tengo innerheight 
                    with_frame = window.innerWidth;
                    espacio_iframe = window.innerHeight;
                } else {
                    //otros navegadores y iframe
                    //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val();

                    espacio_iframe = document.body.clientHeight;
                    with_frame = document.body.clientWidth;

                }
            }



            //corrige el posicionamiento horizontal del modalpopuextender, la propiedad width del panel debe estar en auto
            //var widtth_procent_left_rigth = (with_frame - document.getElementById("Panel_agregar_expediente_carpeta").clientHeight) / 2;
            //$('#Panel_agregar_expediente_carpeta').css("left", (Math.round(widtth_procent_left_rigth)) + "px");
            var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
            $('#Panel_ubicacion_toponimica_expediente_popup').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
            $('#modal_content_ubicacion_toponimica_expediente_popup').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
            //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
            $('#Contenido_ubicacion_toponimica_expediente').css("height", (document.getElementById("modal_content_ubicacion_toponimica_expediente_popup").clientHeight - (document.getElementById("divcabecer_ubicacion_toponimica_expediente_popup").clientHeight + document.getElementById("contendor_botones_unidad_u_b_t").clientHeight)) + "px");
            //Para los modal que contiene gred
            //$('#content_data_grid').css("height", (document.getElementById("contenido_procesa_usu_rel_solicitud").clientHeight - document.getElementById("contenido_titulo_val_radicacion_documentos").clientHeight) + "px");
        }
        catch (err) {
            alert(err.message + " Funcion auto_zise_reasigna_expe_unidad");
        }
    }
    function auto_zise_popup_gestion_meta_datos() {
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
            $('#Panel_gestion_meta_datos').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
            $('#modal_content_modal_meta_data').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
            //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
            $('#contenido_procesa_gestion_meta_datos').css("height", (document.getElementById("modal_content_modal_meta_data").clientHeight - (document.getElementById("divcabecer2_gestion_meta_datos_").clientHeight + document.getElementById("content_boton_gestion_meta_datos").clientHeight)) + "px");
            //Para los modal que contiene gred
            //$('#content_data_grid').css("height", (document.getElementById("contenido_procesa_usu_rel_solicitud").clientHeight - document.getElementById("contenido_titulo_val_radicacion_documentos").clientHeight) + "px");
        }
        catch (err) {
            alert(err.message + " funcion auto_zise_popup_gestion_meta_datos " + err.message);
        }
    }
    function auto_zise_popup_gestion_meta_dato_iframe() {
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
            $('#Panel_gestion_meta_data_archivo').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
            $('#modal_content_gestion_meta_data_archivo').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
            //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
            $('#Contenido_gestion_meta_data_archivo').css("height", (document.getElementById("modal_content_gestion_meta_data_archivo").clientHeight - (document.getElementById("divcabecer_gestion_meta_data_archivo").clientHeight + 5)) + "px");
            $('#Iframe_gestion_meta_data_archivo_').css("height", (document.getElementById("modal_content_gestion_meta_data_archivo").clientHeight - (document.getElementById("divcabecer_gestion_meta_data_archivo").clientHeight + 10)) + "px");
            //Para los modal que contiene gred
            //$('#content_data_grid').css("height", (document.getElementById("contenido_procesa_usu_rel_solicitud").clientHeight - document.getElementById("contenido_titulo_val_radicacion_documentos").clientHeight) + "px");
        }
        catch (err) {
            alert(err.message + " funcion auto_zise_popup_gestion_meta_dato_iframe " + err.message);
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