$(document).ready(function () {
    $.fn.inicio = function () {
        
        $('#data_grid_listado_solicitudes tr[id]').click(function () {
            $('#data_grid_listado_solicitudes tr[id]').css({ "background": "White" });
            $(this).css({ "background-color": "#e8e8f7" });
            var fer = $(this).attr("id");
            $('#hdnEmailID').val(fer);
            var fero = $(this).attr("id_documento_general");
            $('#Hidden_solicitud_compartido').val(fero);         
        });
        
        $('#data_grid_listado_solicitudes tr[id]').dblclick(function () {
            var fer = $(this).attr("id_documento_general");
            $('#Hidden_solicitud_compartido').val(fer);
            $(this).css({ "font-weight": "100" });
            document.getElementById("Button_ver_documentos_relacionados").click();
            var estado_visto = $(this).attr("id_estado_visto")
            if (estado_visto = "0") {
                //pre_actualiza_web_service_estado_visto(fer);
                //$(this).css({ "font-weight": "100" });
            }
        });
        $('#data_grid_listado_solicitudes tr[id]').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });
        if (document.getElementById("Hidden_control_lista").value == "Segunda") {
            document.getElementById("Hidden_control_lista").value = "inhabilitado";
            auto_zise_popup_lista_solicitudes("0", "1");

        }
        if (document.getElementById("Hidden_control_lista").value == "") {
            document.getElementById("Hidden_control_lista").value = "Segunda";
            auto_zise_popup_lista_solicitudes("1", "1");
        }
       
        auto_zise_popup_compartir_documento();
        auto_zise_popup_lista_solicitudes("1", "1");
        auto_zise_popup_usuarios_relacionados();
      
        $('#Contenedorgrid_listado_solicitud').contextMenu('context-menu-1', {

            'Salir del menú': {
                click: function (element) { },
                klass: "fad fa-times"
            },
            'Ver documentos relacionados': {
                click: function (element) {
                    document.getElementById("Button_ver_documentos_relacionados").click();
                },
                klass: "fal fa-folder-open"
            },
            'Eliminar registro': {
                click: function (element) {
                    document.getElementById("Button_eliminar_registro").click();
                },
                klass: "fal fa-times"
            },
            'Marcar como no visto': {
                click: function (element) {
                    document.getElementById("Button_activa_visto").click();
                },
                klass: "fal fa-eye-slash"
            }
        });
    }

  
})
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
        GetLista_documentos_compartidos_revision('TextBox_busqueda');

    } catch (e) {
        alert(" funcion load " + e.message);
    }

});
let IdRgistroDocumentoUusario = 0;
let IdDocumentoGeneral = 0;
let CdCompartirDocunento = new Array();
const ini_event_page = () => {
    let array_element = new Array;
    array_element.push({ id: "ButtonGuadarRegistroDesicion"});
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
            //---Registra decisión documento compartido
            case "ButtonGuadarRegistroDesicion":
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
        if (name_control == "ButtonGuadarRegistroDesicion") {
            result = await RegistroDecisionSolitudAprobacion();
            if (result != "YES") {
                alert_bot(result, 'warning', "error_content_decision");
            }
        }
    }
    catch (ex) {
        alert_bot(ex.message, 'warning', "error_content_compartido");
    } finally {
        document.getElementById(name_control).disabled = false;
        progres_hiden('progres_bar');
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
        auto_zise_popup_compartir_documento();
        auto_zise_popup_lista_solicitudes("1", "1");
        auto_zise_popup_usuarios_relacionados();
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
const RegistroDecisionSolitudAprobacion = async () => {
    try {
        let Result = "";
        let NotaRegistroDecision = "";
        let DescripcionDecision = "";
        let ControlDrowp = document.getElementById("DropDownList_estado_aprobacion");
        let ControlText = document.getElementById("TextBox_nota_solicitud");
        DescripcionDecision = ControlDrowp.options[ControlDrowp.selectedIndex].text;
        NotaRegistroDecision = ControlText.value;
        if (DescripcionDecision == "") {
            return "Debe seleccionar el tipo de decisión";
        }
        if (NotaRegistroDecision == "") {
            return "Debe informar la nota de su desición sobre el documento compartido.";
        }
        Result = await ServiceRESTregistraDecisionSolicitudAprobacion(IdRgistroDocumentoUusario, NotaRegistroDecision, DescripcionDecision);
        if (Result != "YES") {
            return Result;
        }
        actualiza_gre_campo('data_grid_listado_solicitudes', IdRgistroDocumentoUusario, DescripcionDecision, 'ESTADO');
        if (CdCompartirDocunento[0].IdDcoumento == 0) {
            return "YES";
        }
        let Option = {
            id_imagen: CdCompartirDocunento[0].IdDcoumento, name_gabinete: CdCompartirDocunento[0].Gabinete, module: "0",
            valida_firma: "0", name_table: "", name_campo_estado_firma: "",
            name_tipo_table: "", name_element_table_aspnet: ""
        };
        result = await LoadStampFile(Option);
        if (result != "YES") {
            return result;
        }
        return "YES";
    } catch (ex) {
        return ex.mensaje;

    }
}
function activa_boton_interface(nombre_buton) {
    try {
        document.getElementById(nombre_buton).click();
    }
    catch (err) {
        alert(err.message + " Función activa_boton_interface ");
    }

}
function GetLista_documentos_compartidos_revision(name_texbox) {
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
                    url: "../webservice/WebServiceRadicacion.asmx/GetLista_documentos_compartidos_revision",
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
                document.getElementById("TextBox_busqueda").value = ui.item.label;
                document.getElementById("ImageButton_buscar").click();
                return false;
            }, minLength: 3, max: 10, scroll: true
        });
}
function preven_event_search(event, e) {
    try {
        document.getElementById("ImageButton_buscar").click();
        event.preventDefault();
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
            e.preventDefault();
        }

    } catch (err) {
        alert(err.message + " funcion preven_event_search_keypres_enter " + err.message);
    }
}
function preven_event_restor_search(e, sender) {
    try {
        document.getElementById("Button_link_acualiza_lista").click();
        event.preventDefault();
    } catch (err) {
        alert(err.message + " funcion preven_event_restor_search " + err.message);
    }
}
function prevent(event, element) {
    try {
        //Hidden_solicitud_compartido
        var fer = $(element).attr("idd");
        var fer_ = $(element).attr("id_documento_general_");
        var tip_event = $(element).attr("tip_event");
        if (tip_event == "ver_doc_comp") {
            $('#hdnEmailID').val(fer);
            $('#Hidden_solicitud_compartido').val(fer_);
            document.getElementById("Button_ver_documentos_relacionados").click();
        }
        if (tip_event == "elimina_doc_comp") {
            $('#hdnEmailID').val(fer);
            $('#Hidden_solicitud_compartido').val(fer_);
            document.getElementById("Button_eliminar_registro").click();

        }
        if (tip_event == "desicion_doc_resp") {
            $('#hdnEmailID').val(fer);
            $('#Hidden_solicitud_compartido').val(fer_);
            IdDocumentoGeneral = fer_;
            IdRgistroDocumentoUusario = fer;
            document.getElementById("Button_activa_desicion_aprobacion").click();

        }
        if (tip_event == "ver_reg_colab") {
            $('#hdnEmailID').val(fer);
            $('#Hidden_solicitud_compartido').val(fer_);
            document.getElementById("Button_ver_registro_colaboracion").click();

        }
        event.preventDefault();
        element.focus();
    }
    catch (err) {
        alert(err.message + " Funcion prevent");
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
        event.preventDefault();
    }
    catch (err) {
        alert(err.message + " Funcion prevent_scrol");
    }
}
function actualiza_estado_boton_seleccion(gred, nombre_hiden, valor_weight,estado) {
    try {
        $("#" + gred + " tr[id=" + $("#" + nombre_hiden).val() + "]").each(function () {
            var fer = $(this).attr("id");
            var estado_visto = $(this).attr("id_estado_visto");
            if (estado_visto !== estado) {
                pre_actualiza_web_service_estado_visto(fer, 'data_grid_listado_solicitudes', 'id_estado', 'Red', estado);
                $(this).css({ "font-weight": valor_weight });
                $(this).attr("id_estado_visto", estado);
            }
        });
    }
    catch (err) {
        alert("Funcion actualiza_estado_boton_seleccion dice " + err.message);
    }
}
function pre_actualiza_web_service_estado_visto(id_solicitud, nombre_grid, nombre_campo, color_estado,estado) {
    try {

        var update = "Update ra_cd_usuarios_documentos_compartidos set ESTADO_VISTO_SOLICITANTE=" + estado + " where ID_USUARIOS_DOCUMENTOS_COMPARTIDOS=" + id_solicitud;
        web_service_actualiza(update);     
        $("#" + nombre_grid + " tr[" + nombre_campo + "]").each(function () {
            var fer = $(this).attr(nombre_campo);
            if (fer == "2") {
                $(this).css({ "color": color_estado });

            }
        });

    }
    catch (err) {
        alert(err.message + " Funcion pre_actualiza_web_service_estado_visto");
    }
}
function web_service_actualiza(data) {
    try {
        $.ajax({
            type: 'POST',
            url: '../webservice/WebServiceRadicacion.asmx/update_radic_plantilla_radicado',
            data: "{'update':'" + data + "'}",
            contentType: 'application/json; utf-8',
            dataType: 'json',
            success: function (data) {
                if (data.d != null) {
                    $("#Hidden_resultado_web_service").val(data.d);
                    //alert($("#Hidden_resultado_web_service").val());
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                $("#Hidden_resultado_web_service").val(jqXHR.responseText);
                //alert($("#Hidden_resultado_web_service").val());
                alert(jqXHR.responseText);
            }

        });
    }
    catch (err) {
        alert(err.message + " Funcion web_service_actualiza");
    }
}
function auto_zise_popup_usuarios_relacionados() {
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
        $('#Panel_usu_rel_solicitud').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_usu_rel_solicitud').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#contenido_procesa_usu_rel_solicitud').css("height", (document.getElementById("modal_content_usu_rel_solicitud").clientHeight - (document.getElementById("diver_cabcera_user_rel").clientHeight + document.getElementById("content_boton_user_rel").clientHeight )) + "px");
        //Para los modal que contiene gred
        $('#content_data_grid').css("height", (document.getElementById("contenido_procesa_usu_rel_solicitud").clientHeight - document.getElementById("contenido_titulo_val_radicacion_documentos").clientHeight) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_usuarios_relacionados " + err.message);
    }
}
function Lista_tareas_estados(nombre_grid, nombre_campo, color_estado) {
    try {
        $("#" + nombre_grid + " tr[" + nombre_campo + "]").each(function () {
            var fer = $(this).attr(nombre_campo);
            if (fer == "2") {
                $(this).css({ "color": color_estado });

            }
        });
    }
    catch (err) {
        alert(err.message + " Función Lista_tareas_estados ");
    }
}
function Lista_tareas_lectura(nombre_grid, nombre_campo, color_estado) {
    try {
        $("#" + nombre_grid + " tr[" + nombre_campo + "]").each(function () {
            var fer = $(this).attr(nombre_campo);
            if (fer == "0") {
                $(this).css({ "font-weight": color_estado });
            }
        });
    }
    catch (err) {
        alert(err.message + " Función Lista_tareas_lectura ");
    }
}
function activa_boton_interface(nombre_buton) {
    try {
        document.getElementById(nombre_buton).click();
    }
    catch (err) {
        alert(err.message + " Función activa_boton_interface ");
    }

}
//-------Servicio que registra la decisión de aprobación de un documento-----///             
const ServiceRESTregistraDecisionSolicitudAprobacion = async (IdDocumentoCompartidoUsuario, NotaRegistroDecision, DescripcionDecision) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceCompartirDocumento.asmx/ServiceRegistraDecisionSolicitudAprobacion', {
                data: "{" + "'IdDocumentoCompartidoUsuario':'" + IdDocumentoCompartidoUsuario +
                    "','NotaRegistroDecision':'" + NotaRegistroDecision + "','DescripcionDecision':'" + DescripcionDecision + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].AppError !== "YES") {
                        resolve(data.d[0].AppError);
                    } else {
                        CdCompartirDocunento = new Array();
                        $.each(data.d, function (k, v) {
                            CdCompartirDocunento.push(v);
                        });
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
        }
        catch (ex) {
            resolve(ex.message);
        }
    })
    let result = await myPromise;
    return result;
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
           
            }
        }
        $('#Panel_autoriza_compartir_documento').css("height", (espacio_iframe - 5) + "px");
        $('#contenido_procesa_autoriza_compartir_documento').css("height", ((espacio_iframe - 10) - document.getElementById("divcabecer2_autoriza_compartir_documento").clientHeight) + "px");
        $('#Iframe_compartir_documento_').css("height", ((espacio_iframe - 10) - document.getElementById("divcabecer2_autoriza_compartir_documento").clientHeight) + "px");
    } catch (ex) { alert(" funcion auto_zise_popup_compartir_documento " + ex.message) }
}
function activa_export_lista(hiden_name, name_evento) {
    try {
        var hiden = document.getElementById(hiden_name);
        var nombre_gred = "data_grid_documentos";
        var x = $('#' + nombre_gred + ' th');
        var txt = "";
        var i;
        for (i = 0; i < x.length; i++) {
            txt = txt + x[i].innerText.toUpperCase() + "|";
        }

        hiden.value = txt;
        //document.getElementById("Hidden_name_event").value = name_evento;
        //document.getElementById("Button_export_lista_event").click();
        return txt;
    }
    catch (err) {
        alert(err.message + " Funcion activa_export_lista");
    }
}
function auto_zise_popup_visor_externo() {
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

    $('#Panel_visor_externo').css("height", (espacio_iframe - 40) + "px");
    $('#Cotenedorpendiente_visor_externo').css("height", (espacio_iframe - 40) + "px");
    $('#Iframe_visor_externo_').css("height", (espacio_iframe - 40) + "px");
    
   


}
function eliminar_fila_data_gred(gred, nombre_hiden) {
    try {
        var idex = 0;
        $("#" + gred + " tr[id=" + $("#" + nombre_hiden).val() + "]").each(function () {
            idex = $(this)[0].rowIndex;
        })
        $("#" + gred + " tr[id=" + $("#" + nombre_hiden).val() + "]").remove();
        $('#' + nombre_hiden).val("-1");
        //recorre el titulo de la tabla fija

        if (idex == 1) {
            auto_zise_popup_lista_solicitudes("1", "");
        } else {
           
        }
    }
    catch (err) {
        alert(err.message + " Funcion eliminar_fila_data_gred");
    }

}

function busqueda_gred(HiddenSeleccion, data_grid, contenido_busqueda, CheckboxBusqueda,color_ref) {
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
            var scrollableDiv = grid.parent();
            var rowtd = $(this);
            $(this).children("td").each(function (idex) {
                var tempotd = $(this).text().toLowerCase()
                var check = document.getElementById(CheckboxBusqueda).checked;
                if (check == true) {

                    if (idex >= 0) {
                        if (s == tempotd) {
                            $(this).parent().css({ "background-color": "LightSkyBlue", "color": color_ref });
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
                            $(this).parent().css({ "background-color": "LightSkyBlue", "color": color_ref });
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
function auto_zise_popup_lista_solicitudes(value_lista_general, value_lista_usuario) {
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
        $('#div_contendor_principal').css("height", (espacio_iframe - 5) + "px");
        var total = document.getElementById("div_titulo_listado").clientHeight + document.getElementById("div_filtro__fil").clientHeight + document.getElementById("contenido_titulo_listado_solicitudes").clientHeight;
        var gridwith = with_frame - 10;
        var gridheihg_ = ((espacio_iframe - 5) - (total + 20));
        $('#Contenedorgrid_listado_solicitud').css("height", gridheihg_ + "px");
        if (value_lista_general == "1") {
            Lista_tareas_estados("data_grid_listado_solicitudes", "id_estado", "Red");
            Lista_tareas_lectura("data_grid_listado_solicitudes", "id_estado_visto", "700");
        }
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_lista_solicitudes " + err.message);
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
function actualiza_gre_campo(nombre_grid, id, valor_campo, nombre_campo) {
    try {
        $("#" + nombre_grid + " tr[id=" + id + "]").each(function () {
            var idex = -1;
            var name = nombre_campo;
            idex = colum_index(name, nombre_grid);
            if (idex != -1) {
                if (valor_campo == "") {
                    var sas = $(this)[0].cells[idex];
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

