$(document).ready(function () {
    $.fn.inicio = function () {
               
    }
});
let asmxClient;  //-------wapper de consumo de asmx
let IdGabineteConsulta = 0;
let NombreGabineteConsulta = "";
let spinner;
let _JSIndiceGabinete;
let _JSPopupBusquedaGabinete;
let ContadorProgress = 0;
let LengProgress = 0;

$(window).on("load", function () {
    try {
        var elment = document.getElementsByClassName("da_event_captive");
        if (elment) {
            for (var i = 0; i < elment.length; i++) {
                elment[i].addEventListener("click", event_click, false);
            }
        }
       $('#table_consulta_gabinete').exportTableToExcelHybrid({
           buttonSelector: "#btn_dow_load_gabonete",  // Usar botón en toolbar
            spinnerSelector: null, // Spinner dentro de toolbar
            toolbarButton: false,    // Activar el botón en el tolbar de la tabla
            companyName: "DocuArchi",
            reportName: "Reporte gabinete",
            userName: ""
        });
        asmxClient = new ASMXClient(AsmxServicesConfig);
        spinner = new SpinnerManager();
        InitEventPage();
        LoadFormDocuarchi();
        window.addEventListener("resize", rezize_event);
        ShowModalPopup("ModalPopupExtender_mensaje_personalizado_backgroundElement", "Panel_mensaje_personalizado", 100001);
        auto_zise_docuarchi();
        auto_zise_popup_consulta();
    } catch (e) {
        console.log(" funcion load " + e.message);
    }

});
const LoadFormDocuarchi = async () => {
    try {
        let Result = await ActivaSolicitaListaGabinetesPermitidos();
        if (Result) {
        }
    } catch (ex) {
        return "Error funcion LoadFormDocuarchi " + ex.message;
    }
}
const InitEventPage = () => {
    let array_element = new Array;
    array_element.push({ id: "btnSearhGabinete" }, { id: "Button_search_gabinete" }, { id: "BtnsearchgabineteGeneral" },
        { id: "Button_active_update_index_bacth" }, { id: "Button_activa_elimina_registro_documento" }, { id: "b_opcion_busqueda" },
        { id:"Button_restore_gabinete"}
    );
    for (let i = 0; i < array_element.length; i++) {
        let elment_a_document_production = document.getElementById(array_element[i].id);
        if (elment_a_document_production) {
            elment_a_document_production.addEventListener("click", EventElementClickPromise, false);
        }
    }
}
const HandlerElementEvent = (e) => {
    try {
        let IdTarget = e.currentTarget.id;
        delete_alert_boot();
        switch (IdTarget) {
            //Guarda documento escaneado
            case "btnSearhGabinete":
                EventElementClickPromise(e);
                break;
            case "Button_search_gabinete":
                EventElementClickPromise(e);
                break;
            case "BtnsearchgabineteGeneral":
                EventElementClickPromise(e);
                break;
           
        }
    } catch (ex) {
        console.log("Funcion HandlerElementEvent " + ex.message);
    }
}
const EventElementClickPromise = async (e) => {
    let IdControl = e.currentTarget.id;
    try {
        let result = "";
        delete_alert_boot();
        e.currentTarget.disabled = true;
        spinner.mostrarProgresBar();
        spinner.showOnButton(IdControl, "circle");
        if (IdControl == "btnSearhGabinete") {
            result = await EventSolicitiaFormularioConsulta(e);
            if (result !== "YES") {
                alert_bot(result, 'warning', "error_content_general_docuarchi");
            }
        }
        if (IdControl == "Button_search_gabinete") {  
            result = await EventActivaConsultaAvanzadaGabinete(e);
            if (result !== "YES") {
                alert_bot(result, 'warning', "error_modal_option_search_gabinete");
            }
        }
        if (IdControl == "BtnsearchgabineteGeneral") {
            let HtmlSearh = document.getElementById("textBox_buequeda_general_gabinete");
            result = await EventSolicitaConsultaGeneralGabinete(HtmlSearh.value,2);
            if (result !== "YES") {
                alert_bot(result, 'warning', "error_content_consulta_gabinete");
            }
        }
        if (IdControl == "Button_active_update_index_bacth") { 
            result = await EventActivaMultplexIndexBatch();
            if (result !== "YES") {
                alert_bot(result, 'warning', "error_content_consulta_gabinete");
            }
        }
        if (IdControl == "Button_activa_elimina_registro_documento") {
            spinner.ocultarProgresBar();
            result = await EventActivaEliminarMultiplexRegistros();
            if (result !== "YES") {
                alert_bot(result, 'warning', "error_content_consulta_gabinete");
            }
        }
        if (IdControl == "b_opcion_busqueda") {
            $('#modal_option_search_gabinete').modal("show");
            //result = await openAndPositionModal("modal_option_search_gabinete", "b_opcion_busqueda");
            //if (result !== "YES") {
            //    alert_bot(result, 'warning', "error_content_consulta_gabinete");
            //}
            //AutoZiseModalOptionConsulta();
        }
        if (IdControl == "Button_restore_gabinete") {
            result = await EventRestoreControlOptionConsulta();
            if (result !== "YES") {
                alert_bot(result, 'warning', "error_modal_option_search_gabinete");
            }
        }
    }
    catch (ex) {
        alert_bot(ex.message, 'warning', "error_content_general_docuarchi");
    } finally {
        spinner.hideOnButton(IdControl);
        spinner.ocultarProgresBar();
        document.getElementById(IdControl).disabled = false;
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



//📕lISTA GABINETE PERMITIDOS
/**
 * Lista gabinetes permitidos
 * ActivaSolicitaListaGabinetesPermitidos()
 * ServiceSolicitaListaGabinetesPermitidos             BACK END
 * SolicitaGabinetesPermitidosGrupo()                  BACK END
 * SolicitagabinetesPermitidosUsuario()                BACK END
 * */
const ActivaSolicitaListaGabinetesPermitidos = async () => {
    try {
        let resp1 = await asmxClient
            .use("Docuarchi")
            .call("ServiceSolicitaListaGabinetesPermitidos", { Parameter: 0 });
        if (resp1.error) {
            return resp1.message;
        }
        let Data = resp1.data[0];
        let ItemsOption = new Array();
        $.each(Data.item_sistema, function (k, v) {
            ItemsOption.push(v);
        });
        if (document.getElementById('DropDownList_gabinetes')) {
            var element_drow = document.getElementById('DropDownList_gabinetes');
            $("#DropDownList_gabinetes").empty();
            for (var i = 0; i < ItemsOption.length; i++) {
                element_drow[i] = new Option(ItemsOption[i].text, ItemsOption[i].value);
            }
        }
        return "YES";
    } catch (ex) {
        return "Inconsistencia funcion ActivaSolicitaListaGabinetesPermitidos " + ex.message;
    }
}


/**👍👍👍 🚀🚀🚀-ACTIVA Y CONFIGURA EL FORMULARIO DE CONSULTA DE GBINETES Y 
 * SOLICITA PERMISOS DE USUARIO Y DE GRUPO DE USUARIO SOBRE GABINETE
Y CONFIGURA PARAMETROS DE BUSQUEDA AVANZADA 👍👍👍*/
/**
 * EventSolicitiaFormularioConsulta
 * ActivaSolicitaPermisosSessionGabinete
 * ServiceSolicitaPermisosSessionGabinete               BACK END
 * SolicitaPermisosSessionGabinete                      BACK END
 * _JSPopupBusquedaGabinete.LoadJSPopupBusquedaGabinete
 * ServiceCreaInterfazindiceGabinete                    BACK END
 * SolicitaEstructuraValoresCamposIndice                BACK END
 * @param {any} IdGabinete
 * @param {any} NombreGabinete
 */
const ActivaSolicitaPermisosSessionGabinete = async (IdGabinete, NombreGabinete) => {
    try {
        const CDParamenterGabinete = [{ IdGabinete: IdGabinete, NombreGabinete: NombreGabinete }];
        let resp1 = await asmxClient
            .use("Docuarchi")
            .call("ServiceSolicitaPermisosSessionGabinete", { Parameter: CDParamenterGabinete });
        if (resp1.error) {
            return resp1.message;
        }
        return "YES";
    } catch (ex) {
        return "Inconsistencia funcion ActivaSolicitaPermisosSessionGabinete " + ex.message;
    }
}
const EventSolicitiaFormularioConsulta = async (e) => {
    try {
        let Result = "";
        let ElmentOPtion = document.getElementById('DropDownList_gabinetes');
        let itemEelmentOPtion = ElmentOPtion.options[ElmentOPtion.selectedIndex];
        if (itemEelmentOPtion.value == 0) {
            return "Seleccione el gabinete a consultar";
        }
        Result = await ActivaSolicitaPermisosSessionGabinete(itemEelmentOPtion.value, itemEelmentOPtion.text);
        if (Result != "YES") {
            return Result;
        }
        IdGabineteConsulta = itemEelmentOPtion.value;
        NombreGabineteConsulta = itemEelmentOPtion.text;

        const CDParamenterGabinete = [{
            IdGabineteConsulta: IdGabineteConsulta,
            NombreGabineteConsulta: NombreGabineteConsulta,
            IdmagenGabinete: 0,
            NameEspaceControl: "",
            NameControlPadre: "div_consulta_gabinetes_migracion",
            NameClassControlGroup: "consulta_gabinetes_migracion"
        }];
        _JSPopupBusquedaGabinete = new JSPopupBusquedaGabinete(CDParamenterGabinete[0]);
        Result = await _JSPopupBusquedaGabinete.LoadJSPopupBusquedaGabinete();
        if (Result !== "YES") { return Result };
        asmxClient.autoCompleteNative(
            "Docuarchi",
            "textBox_buequeda_general_gabinete",
            "ServiceAutoCompleteConsultaGabinete",
            {
                NameDbsAuto: "",
                NameTableAuto: NombreGabineteConsulta,
                NameCampoAuto: "",
                IdTable: IdGabineteConsulta
            },
            async (Value, inputEl) => {
                try {
                    asmxClient.showSpinner(inputEl); // 🔹 spinner sigue activo durante tu lógica
                    let Rest = await EventSolicitaConsultaGeneralGabinete(Value, 2);
                } finally {
                    asmxClient.removeSpinner(inputEl); // 🔹 se quita solo al terminar
                }
            },
            { minChars: 4, maxResults: 15 }
        );
        $('#table_consulta_gabinete').bootstrapTable('destroy');
        $("#consulta_gabinete").modal("show");
        document.getElementById("gabinet_title").innerText = itemEelmentOPtion.text;
        AutoZiseConsultaModal();
        return "YES";
    } catch (ex) {
        return "Inconsistencia general funcion EventSolicitiaFormularioConsulta " + ex.message;
    }
}



//👍👍👍-----CONSULTA AVANZADA GABINETE 👍👍👍
/**
 * _JSPopupBusquedaGabinete._ConsultaOpcionesGabinete()
 * _JSPopupBusquedaGabinete._SolicitaDatosConsulta()
 * ServiceConsultaGabinete                                  BACK END
 * ConsultaGabinete                                         BACK END
 * SolicitaEstructuraCamposGabinetePorId                    BACK END
 * SolicitaEstructuraCamposConsultaGabineteBootStra         BACK END
 * SolicitaNombreGabinetePorId                              BACK END
 * SolicitasqlConsultaGabinete                              BACK END
 * SolicitaEstructuraConsultaGabinete                       BACK END
 * @param {any} e
 */
const EventActivaConsultaAvanzadaGabinete = async (e) => {
    try {
        let Result = await _JSPopupBusquedaGabinete._ConsultaOpcionesGabinete();
        if (Result != "YES") { return Result };

        $('#modal_option_search_gabinete').modal("hide");
        return Result;
    } catch (ex) {
        return "Inconsistencia funcion EventActivaConsultaAvanzadaGabinete " + ex.message;
    }
}
/**
 * Activa limpiar los controles del formulario de consulta avanzada
 * @param {any} e
 */
const EventRestoreControlOptionConsulta = async (e) => {
    try {
        let Result = await _JSPopupBusquedaGabinete._ElementTextClear();
        if (Result != "YES") { return Result };
        Result = await _JSPopupBusquedaGabinete._ElementDangerAlertClearClass();
        return Result;
    } catch (ex) {
        return "Inconsistencia funcion EventRestoreControlOptionConsulta " + ex.message;
    }
}



//👍👍👍-----CONSULTA GENERAL DE GABINETE 👍👍👍
/**
 * EventSolicitaConsultaGeneralGabinete
 * ServiceConsultaGabinete                                  BACK END
 * ConsultaGabinete                                         BACK END
 * SolicitaEstructuraCamposGabinetePorId                    BACK END
 * SolicitaEstructuraCamposConsultaGabineteBootStra         BACK END
 * SolicitaNombreGabinetePorId                              BACK END
 * SolicitasqlConsultaGabinete                              BACK END
 * SolicitaEstructuraConsultaGabinete                       BACK END
 * @param {any} ValorConsulta
 * @param {any} TipoConsulta
 */
const EventSolicitaConsultaGeneralGabinete = async (ValorConsulta, TipoConsulta) => {
    try {
        const CDParamenterGabinete = [{ ValorConsulta: ValorConsulta, TipoConsulta: TipoConsulta, IdGabinete: IdGabineteConsulta, NombreGabinete: NombreGabineteConsulta, ClassConfigGeneralService: ITEM_GENERAL_CONTROL_ARRAY }];
        let resp1 = await asmxClient
            .use("Docuarchi")
            .call("ServiceConsultaGabinete", { Parameter: CDParamenterGabinete });
        if (resp1.error) {
            return resp1.message;
        }
        let Data = resp1.data[0];
        let class_stru_row_Gabinete_Generic = JSON.parse(Data.Obj_ilist_row_generic);
        init_row_feld_table_boostrap_table("table_consulta_gabinete",
            Data.Obj_ilist_fileds_generic,
            class_stru_row_Gabinete_Generic,
            "contenido_table_boot_migracion",
            "table-bordered",
            "table-borderless",
            "multiple",
            "bt-selected",    // hex (#e7ebf6) o clase CSS
            false,
            false,      //quitar bordes de tr/td
            false,           // habilitar/deshabilitar table-hover
            true,        // bordes redondeados en seleccionados
            ["vis_doc_selecion_rad"], // SOLO estas clases disparan selección
            true);
        //AutoZiseConsultaModal();
        return "YES";
    } catch (ex) {
        return "Inconsistencia funcion EventActivaConsultaAvanzadaGabinete " + ex.message;
    }
}



//👍👍👍-----VISUALIZA DOCUMENTO GABINETE 👍👍👍
/**
 * EventActivaVisualizaDocumento
 * ServiceSolicitaUrlVisorConsulta               BACK END
 * SolicitaUrlVisorConsulta                      BACK END
 * SolicitaIdTipoImagen                          BACK END
 * SolicitaExtensionArchivoGabineteTipoImagen    BACK END
 * @param {any} Idmagen
 */
const EventActivaVisualizaDocumento = async (Idmagen) => {
    try {
        const CDParamenterGabinete = [{ IdGabinete: IdGabineteConsulta, NombreGabinete: NombreGabineteConsulta, IdImagen: Idmagen }];
        let resp1 = await asmxClient
            .use("Docuarchi")
            .call("ServiceSolicitaUrlVisorConsulta", { Parameter: CDParamenterGabinete });
        if (resp1.error) {
            return resp1.message + resp1.status;
        }
        if (resp1.message != "YES") {
            return resp1.message;
        }
        let Data = resp1.data[0];
        document.getElementById("Iframe_visor_migracion_documento").src = Data.url_iframe;
        document.getElementById("h_title_gabinete_image_migracion").innerText = "DOCUMENTO  : " + Idmagen + "  GABINETE : " + NombreGabineteConsulta;
        $("#modal_visor_migracion_documento").modal("show");
        return "YES";
    } catch (ex) {
        return "Inconsistencia funcion EventActivaVisualizaDocumento " + ex.message;
    }
}



//👍👍👍-----ACTUALIZA INDICE GABINETE 👍👍👍
/**
 * EventActivaIdiceBatch
 * _JSIndiceGabinete._LoadIndexGabinete
 * ServiceCreaInterfazindiceGabinete              BACK END
 * SolicitaEstructuraValoresCamposIndice          BACK END
 * SolicitaEstructuraCamposGabinete               BACK END
 * AsignaDatosIndiceDocumento                     BACK END
 * AsignaDatosEstructuraGeneralIndice             BACK END
 * _JSIndiceGabinete._EventActivaActualizaIndice
 * ServiceActualizaIndiceBatchGabinete            BACK END
 * ActualizaIndiceDocumentoGabinete               BACK END
 * Dependencia   JSIindiceGabinete
 * Fecha 2025-09-09 Miguel Urueta
 * @param {any} Idmagen
 */
const EventActivaIdiceBatch = async (Idmagen) => {
    try {
        let Option = [{
            IdGabineteConsulta: IdGabineteConsulta , NombreGabineteConsulta: NombreGabineteConsulta,
            IdmagenGabinete: Idmagen, NameParent: "parent_workflow", NameEspaceControl: "class_indice_batch_gabinete_001",
            NameModulo: "DOCUARCHI", NameTable: "table_consulta_gabinete", NameAsmxUpdaTeIndice: "ServiceActualizaIndiceBatchGabinete",
            OptionIndice:2
        }];
        _JSIndiceGabinete = new JSindiceGabinete(Option[0]);
        let Result = await _JSIndiceGabinete._LoadIndexGabinete();
        return Result;
    } catch (ex) {
        return "Inconsistencia EventActivaIdiceBatch " + ex.message;
    }
}


//👍👍👍-----ACTUALIZA MULTIPLEX INDICES GABINETE 👍👍👍
/**
 * _EventActivaActualizaIndiceBatch
 * _JSIndiceGabinete._LoadIndexGabinete
 * ServiceCreaInterfazindiceGabinete              BACK END
 * SolicitaEstructuraValoresCamposIndice          BACK END
 * SolicitaEstructuraCamposGabinete               BACK END
 * AsignaDatosIndiceDocumento                     BACK END
 * AsignaDatosEstructuraGeneralIndice             BACK END
 * JSProgresBarBoot(_OPtionProgresBar)            🚀Envia la progress los indices actualizar
 * ServiceActualizaIndiceBatchGabinete            BACK END
 * ActualizaIndiceDocumentoGabinete               BACK END
 * Dependencia   JSIindiceGabinete  JSProgresBar
 * Fecha 2025-09-09 Miguel Urueta
 */
const EventActivaMultplexIndexBatch = async () => {
    try {
        let SelectionTable = new Array();
        SelectionTable = $('#' + 'table_consulta_gabinete').bootstrapTable('getSelections');
        if (SelectionTable.length == 0) { return  "Debe seleccionar los ítems de la tabla que desea actualizar." }
        let Option = [{
            IdGabineteConsulta: IdGabineteConsulta, NombreGabineteConsulta: NombreGabineteConsulta,
            IdmagenGabinete: SelectionTable[0].ID, NameParent: "parent_workflow", NameEspaceControl: "class_indice_batch_gabinete_001",
            NameModulo: "DOCUARCHI", NameTable: "table_consulta_gabinete", NameAsmxUpdaTeIndice: "ServiceActualizaIndiceBatchGabinete"
        }];
        _JSIndiceGabinete = new JSindiceGabinete(Option[0]);
        let Result = await _JSIndiceGabinete._LoadIndexGabinete();
        return Result;
    } catch (ex) {
        return "Inconsistencia general funcion EventActivaMultplexIndexBatch " + ex.message
    }
}


//👍👍👍-----ELIMINAR REGISTRO DOCUMENTO 👍👍👍
/**
 * EventActivaEliminaRegistro
 * ElimnaRegistroDcoumento
 * ServiceEliminaDocumentoGabinete                BACK END
 * EliminarDocumentosGabinete                     BACK END
 * @param {any} Idmagen
 */
const EventActivaEliminaRegistro = async (Idmagen) => {
    try {
        let CDParamenterGabinete = [{
            IdGabinete: IdGabineteConsulta, NombreGabinete: NombreGabineteConsulta,
            IdImagen: Idmagen,
            NombreModulo: "DOCUARCHI"
        }];
      
       let Result = await ElimnaRegistroDcoumento(CDParamenterGabinete);
        return Result;
    } catch (ex) {
        return "Inconsistencia EventActivaIdiceBatch " + ex.message;
    }
}


//👍👍👍-----ELIMINAR MULTIPLEX REGISTROS DE  DOCUMENTOS 👍👍👍
/**
 * EventActivaEliminarMultiplexRegistros
 * JSProgresBarBoot(_OPtionProgresBar)            🚀Envia AL progress los registros a eliminar para iterar
 * ElimnaRegistroDcoumento
 * ServiceEliminaDocumentoGabinete                BACK END
 * EliminarDocumentosGabinete                     BACK END
 * Dependencia    JSProgresBar
 * Fecha 2025-09-09 Miguel Urueta
 * @param {any} Idmagen
 */
const EventActivaEliminarMultiplexRegistros = async () => {
    try {
        let SelectionTable = new Array();
        SelectionTable = $('#' + 'table_consulta_gabinete').bootstrapTable('getSelections');
        if (SelectionTable.length == 0) { return "Debe seleccionar los ítems de la tabla que desea eliminar." }
        let Result = await JSPopupConfirmInit({ TitlePopup: "Eliminar registro", MensajepPoput: "Desea eliminar " +SelectionTable.length + " documento selecionados?", NameContenPopup: "parent_workflow" });
        if (Result.ReSulTPopup == "NOT") { return "YES" };
        spinner.mostrarProgresBar();
        const CDParamenterGabinete = [];
        SelectionTable.forEach(r => {
            let ParamenterGabinete = [];
            ParamenterGabinete.push({
                IdGabinete: IdGabineteConsulta,
                NombreGabinete: NombreGabineteConsulta,
                IdImagen: r.ID,
                NombreModulo: "DOCUARCHI"
            })
            CDParamenterGabinete.push(ParamenterGabinete);
        });
        let _OPtionProgresBar = ({
            name_service: "EliminarDocumentoGabinete",
            OptionItemSelect: CDParamenterGabinete,
            NameControlPadreProgres: "parent_workflow", NameProceso: "Elminando documentos", ObjectComponente: null
        });
        Result = await JSProgresBarBoot(_OPtionProgresBar);
        return Result;
    } catch (ex) {
        return "Inconsistencia EventActivaIdiceBatch " + ex.message;
    }
}
const ElimnaRegistroDcoumento = async (CDParamenterGabinete) => {
    try {
        let resp1 = await asmxClient
            .use("Docuarchi")
            .call("ServiceEliminaDocumentoGabinete", { Parameter: CDParamenterGabinete });
        if (resp1.error) {
            return `${resp1.message} ${resp1.status}`;
        }
        if (resp1.message !== "YES") {
            return resp1.message;
        }
        let Result = "";
        Result = await deleteRecordById("table_consulta_gabinete", CDParamenterGabinete[0].IdImagen);
        if (Result !== "YES") { return Result; }
        return "YES";
    } catch (ex) {
        return "Incosistecia funcion ElimnaRegistroDcoumento " + ex.message;
    }
}

//👍👍👍-----GESTIONA VERSIONES DEl DOCUMENTO 👍👍👍
/**
 * EvenActivaVersionadoDocumento
 * ShowListVersionDocumento
 * @param {any} Idmagen
 */
const EvenActivaVersionadoDocumento = async (Idmagen) => {
    try {
        let Result = "";
        let option =
            ({
                IdImagen: Idmagen, Gabinete: NombreGabineteConsulta, name_class_element_icono_aspnet: "",
                DocumentoTilte: "Versiones del documento", OptionRemPlazo: "", ContentError: "error_content_consulta_gabinete",
                NameControlParent: "parent_workflow", NameModulo: "DOCUARCHI", TipoModulo:4
            })
        Result = await ShowListVersionDocumento(option);
        return "YES";
    } catch (ex) {
        return "Inconsistencia funcion EvenActivaVersionadoDocumento " + ex.message;
    }
}
function operateFormattertablebootmig(value, row, index) {
    let icono_font
    let ident = table_boot_return_objet_jonson(row);
    if (parseInt(ident.ESTADO_FIRMA_DIGITAL) === 1) {
        icono_font = "fal fa-file-certificate";
    } else {
        icono_font = "fal " + TableAgregaIconoAwesonGabinete(ident.DBT);
    }
    return [
        '<div class="row pl-2">',
        '<div class="col-8 p-0">',
        '<a class="active_view_document nav-link pl-5 justify-content-end font-weight-light" style="color: black" href="javascript:void(0)" title="Visualiza documento">  <i style="color: black" class="' + icono_font + '"></i>  </a>',
        '</div > ',
        '<div class="col-4 p-0">',
        '<a class="nav-link  dropdown-toggle justify-content-start" style="color: black" href="#"  data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: black; display:none" class="fad fa-th-list"></i>  ',
        '</a>',
        '<div class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">',
        '<a class="active_view_version dropdown-item font-weight-light" href="javascript:void(0)" title="Lista versiones del documento">  <i style="color: #black" class="far fa-folder-open"></i> Versiones del documento </a>',
        '<a class="active_view_detail_row dropdown-item font-weight-light" href="javascript:void(0)" title="Lista datos de registro de migración">  <i style="color: #black" class="fas fa-list"></i> Registro de migración del documento</a>', ,
        '<a class="active_indice_documento btn dropdown-item font-weight-light" href="javascript:void(0)" title="Indice del documento">  <i style="color: #black" class="fas fa-info-square"></i> Lista índice del documento </a>',
        '<a class="elimina_registro_documento dropdown-item font-weight-light" href="javascript:void(0)" title="Eliminar documento">  <i style="color: #black" class="fas fa-download"></i> Eliminar documento </a>',
        '<a style="color: black" href="#" class="dropdown-item font-weight-light"><i class="far fa-sign-out"></i> Salir del menu</a>',
        '</div>',
        '</a>',
        '</div>',
        '</div>',
    ].join('')
}
const EventExtenOperateEvent = async (e, value, row, index, ContentError, NameOperate) => {
    try {
        let result = "";
        let ident = table_boot_return_objet_jonson(row);
        delete_alert_boot();
        spinner.mostrarProgresBar();   
        switch (NameOperate) {
            case "active_view_document":
                result = await EventActivaVisualizaDocumento(ident.ID);
                if (result !== "YES") {
                    alert_bot(result, 'warning', ContentError);
                }
                break;
            case "active_indice_documento":

                result = await EventActivaIdiceBatch(ident.ID);
                if (result !== "YES") {
                    alert_bot(result, 'warning', ContentError);
                }
                break;
            case "elimina_registro_documento":
                spinner.ocultarProgresBar();
                let Result = await JSPopupConfirmInit({ TitlePopup: "Eliminar registro documento", MensajepPoput: "Desea eliminar el documento?", NameContenPopup: "parent_workflow" });
                if (Result.ReSulTPopup == "NOT") { return "YES" };
                spinner.mostrarProgresBar();
                result = await EventActivaEliminaRegistro(ident.ID);
                if (result !== "YES") {
                    alert_bot(result, 'warning', ContentError);
                }
                break;
            case "active_view_version":
                result = await EvenActivaVersionadoDocumento(ident.ID);
                if (result !== "YES") {
                    alert_bot(result, 'warning', ContentError);
                }
                break;
            default:
                alert_bot("El control (" + IdControl + ") no tiene evento registrado ", 'warning', ContentError);
        } 
    }
    catch (ex) {
        alert_bot(ex.message, 'warning', "error_content_general_docuarchi");
    } finally {
        spinner.ocultarProgresBar(); 
    }
}
window.operateEvents  =  {
    'click .elimina_registro_documento': (e, value, row, index) => {
        
        EventExtenOperateEvent(e, value, row, index, "error_content_consulta_gabinete", "elimina_registro_documento");

    }, 'click .active_indice_documento': (e, value, row, index) => {
        EventExtenOperateEvent(e, value, row, index, "error_content_consulta_gabinete", "active_indice_documento");

    }, 'click .active_view_version': (e, value, row, index) => {
        EventExtenOperateEvent(e, value, row, index, "error_content_consulta_gabinete", "active_view_version");
    }, 'click .active_view_document': (e, value, row, index) => {
        EventExtenOperateEvent(e, value, row, index, "error_content_consulta_gabinete", "active_view_document");
       

    }, 'click .active_view_detail_row': (e, value, row, index) => {
        delete_alert_boot();
        let ident = table_boot_return_objet_jonson(row);
        ID_IMAGEN_MIG = ident.ID;
        Show_detail_registro_migracion(ID_IMAGEN_MIG, GABINETE_MIG);
    }
}
function rezize_event() {
    try {
        auto_zise_docuarchi();
        auto_zise_popup_consulta();
        AutoZiseConsultaModal();
        //AutoZiseModalOptionConsulta();
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
function auto_zise_popup_consulta() {
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
                //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val(); Contenido_consulta_documento tol_pie

            }
        }

        /*var heigconetedor = (espacio_iframe - 1) - (((espacio_iframe - 1) * 1) / 100);
        var widthconetedor = (with_frame - 1) - (((with_frame - 1) * 1) / 100);
        $('#Panel_consulta_documento').css("height", heigconetedor + "px");
        $('#Contenido_consulta_documento').css("height", heigconetedor + "px");
        $('#ifimpre_consulta_documento').css("height", heigconetedor + "px");
        $('#Panel_consulta_documento').css("width", widthconetedor + "px");
        $('#Contenido_consulta_documento').css("width", widthconetedor + "px");
        $('#ifimpre_consulta_documento').css("width", widthconetedor + "px");*/
        //corrige el posicionamiento horizontal del modalpopuextender, la propiedad width del panel debe estar en auto
        //var widtth_procent_left_rigth = (with_frame - document.getElementById("Panel_agregar_expediente_carpeta").clientHeight) / 2;
        //$('#Panel_agregar_expediente_carpeta').css("left", (Math.round(widtth_procent_left_rigth)) + "px");
        var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_consulta_documento').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#Contenido_consulta_documento').css("height", (document.getElementById("Panel_consulta_documento").clientHeight - (document.getElementById("divcabecer_consulta_documento").clientHeight + 2)) + "px");
        //Para los modal que contiene gred
        $('#ifimpre_consulta_documento_').css("height", (document.getElementById("Contenido_consulta_documento").clientHeight - 2) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_docuarchi " + err.message);
    }
}

function auto_zise_docuarchi() {
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

        var heigconetedor = (espacio_iframe - 5) - (((espacio_iframe - 5) * 95) / 100);
        $('#menu_tulbar').css("height", heigconetedor + "px");
        $('#bar_herramineta').css("height", heigconetedor + "px");
        heigconetedor = (espacio_iframe - 5) - (((espacio_iframe - 5) * 10) / 100);
        $('#area_trabjo').css("height", heigconetedor + "px");
        heigconetedor = (espacio_iframe - 5) - (((espacio_iframe - 5) * 15) / 100);
        $('#div_carpetas').css("height", heigconetedor + "px");
        heigconetedor = (espacio_iframe - 5) - (((espacio_iframe - 5) * 95) / 100);
        $('#tol_pie').css("height", heigconetedor + "px");
       
       
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_docuarchi " + err.message);
    }
}
function AutoZiseConsultaModal() {
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

        //$('#Contentizquierdo').css("height", ((espacio_iframe - (60 +  document.getElementById("tool_bar_version_document").clientHeight) ) - 1) + "px");
        //$('#sidebar_').css("height", ((espacio_iframe - (60 + document.getElementById("tool_bar_version_document").clientHeight)) - 1) + "px");
        //$("#contenido_controles_consulta").css("height", (document.getElementById("Contentizquierdo").clientHeight) - (document.getElementById('contenido_controles_buton_consulta').clientHeight ) + "px");
        //$("#contenido_consulta_gabinetes_migracion").css("height", (document.getElementById("Contentizquierdo").clientHeight) - (document.getElementById('contenido_controles_buton_consulta').clientHeight) + "px");
        $('#Contenedorderecho').css("height", ((espacio_iframe - (60 + document.getElementById("tool_bar_version_document").clientHeight)) - 1) + "px");
        $('#contenido_table_boot_migracion').css("height", (document.getElementById("Contenedorderecho").clientHeight - (document.getElementById("contenido_icon_boton_migra").clientHeight + 5 )) + "px");
        let heig_table = document.getElementById('contenido_table_boot_migracion').clientHeight - 5;
        table_reize_heigth("table_consulta_gabinete", heig_table, "", "table-borderless",true);


    } catch (ex) { alert("Funcion auto_zise_consulta " + ex.message); }

}
function AutoZiseModalOptionConsulta() {
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

        $('#Contentizquierdo').css("height", ((espacio_iframe - (60 + document.getElementById("tool_bar_version_document").clientHeight)) - 1) + "px");
        //$('#sidebar_').css("height", ((espacio_iframe - (60 + document.getElementById("tool_bar_version_document").clientHeight)) - 1) + "px");
        $("#contenido_controles_consulta").css("height", (document.getElementById("Contentizquierdo").clientHeight) - (document.getElementById('contenido_controles_buton_consulta').clientHeight) + "px");
        //$("#contenido_consulta_gabinetes_migracion").css("height", (document.getElementById("Contentizquierdo").clientHeight) - (document.getElementById('contenido_controles_buton_consulta').clientHeight) + "px");
        


    } catch (ex) { alert("Funcion AutoZiseModalOptionConsulta " + ex.message); }

}
function progres_hiden(progres) {
    $("#progres_bar").css("display", "none");
    
    
}
//ACTIVA EL GIF DE PROGRESO DE UN EVENTO
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
        alert(err.message + " funcion posicion_update_pogres " + err.message);
    }

}
