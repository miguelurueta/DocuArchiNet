$(document).ready(function () {
    $.fn.inicio = function () {
        oculta_panel();
        auto_zise();
        clired();
        control_visualiza_boton_asignacion_expediente();
        auto_zise_ubicacion_toponimica();
        auto_zise_popup_visor_externo();
        auto_zise_reasigna_expe_unidad();
        auto_zise_popup_padres_relacionados();
        auto_zise_popup_padres_relacionados();
        auto_zise_popup_volumenes_relacionados();
        auto_zise_popup_add_edit_expediente();
        service_documentos_clasificacion("TextBox_busqueda_documento");
        service_get_lista_expediente("TextBox_buequeda_general");
        service_get_lista_expediente_relacion("TextBox_busqueda_padres");
        service_get_lista_expediente_relacion_padre("TextBox_busqueda_padres_volumen");
        auto_zise_popup_indice_expediente();
        ini_auto_complete_form_add_edit_expeidente();
        $('.close').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });
        $('.def').click(function (event) {
            event.preventDefault();
        });       
    };
});
$(window).on("load", function () {
    try {
        var elment = document.getElementsByClassName("da_event_captive");
        if (elment) {
            for (var i = 0; i < elment.length; i++) {
                elment[i].addEventListener("click", event_click, false);
            }
        }
        ini_auto_complete_form_add_edit_expeidente();
        tab_sow("home-expediente");
        window.addEventListener("resize", rezize_event);        
        ShowModalPopup("ModalPopupExtender_mensaje_personalizado_backgroundElement", "Panel_mensaje_personalizado", 100001);
        ShowModalPopup("ModalPopupExtende_agregar_unidad_conservacion_popup_backgroundElement", "Panel_agregar_unidad_conservacion_popup", 100001);
        ShowModalPopup("ModalPopupExtender_edition_add_edit_expediente_backgroundElement", "Panel_add_edit_expediente", 100001);
        ShowModalPopup("ModalPopupExtender_edition_pro_gres_bar_backgroundElement", "Panel_pro_gres_bar", 100008);
        ShowModalPopup("ModalPopupExtenderimpre_post_backgroundElement", "Panelimpresionpost", 100002);
        service_documentos_clasificacion("TextBox_busqueda_documento");
        service_get_lista_expediente("TextBox_buequeda_general");
        service_get_lista_expediente_relacion("TextBox_busqueda_padres");
        service_get_lista_expediente_relacion_padre("TextBox_busqueda_padres_volumen");
        
    } catch (e) {
        alert(" funcion load " + e.message);
    }
})
function ini_auto_complete_form_add_edit_expeidente() {
    try {
       var elment = document.getElementsByClassName("event_auto_source_expe");
        if (elment) {
            for (var i = 0; i < elment.length; i++) {
                //elment[i].addEventListener("click", event_click, false);
                service_get_lista_auto_complete_expediente(elment[i].id, elment[i].getAttribute("cap_name"))
            }
        }
    }
    catch (err) {
        alert(err.message + " Funcion ini_auto_complete_form_add_edit_expeidente");
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
            auto_zise();
            auto_zise_agregar_expediente();
            auto_zise_editar_expediente();
            auto_zise_ubicacion_toponimica();
            auto_zise_popup_visor_externo();
            auto_zise_reasigna_expe_unidad();
            auto_zise_popup_padres_relacionados();
            auto_zise_popup_padres_relacionados();
            auto_zise_popup_volumenes_relacionados();
            auto_zise_popup_add_edit_expediente();
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
    function tab_sow(name) {
        $('#' + name).tab('show');
    }
//zona agregar expediente
var ESTADO_EVENT_GENERAL = "";
var INTERVAL_EVENT_GENERAL;
function event_element_clic(event, e) {
    try {
        ESTADO_EVENT_GENERAL = "intro";
        e.disabled = true;
        INTERVAL_EVENT_GENERAL = setInterval(fx_funcion, 400);
        function fx_funcion() {
            //--Sale del evento
            if (ESTADO_EVENT_GENERAL == "out") {           
                e.disabled = false;
                clearInterval(INTERVAL_EVENT_GENERAL);
                ESTADO_EVENT_GENERAL = "";
            }
            //--Entra al evento
            if (ESTADO_EVENT_GENERAL == "intro") {
                ESTADO_EVENT_GENERAL = "";
                if (e.id == "Button_aceptar_java" && e.tip_event =="add") {
                    service_registra_expediente();
                    return true;
                }
                if (e.id == "Button_aceptar_java" && e.tip_event == "edit") {
                    service_actualiza_expediente(document.getElementById("hdnEmailID").value);
                    return true;
                }
                if (e.id == "Button_aceptar_java" && e.tip_event == "vol") {
                    service_registra_expediente_volumen();
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
        INTERVAL_EVENT_GENERAL = setInterval(fx_funcion, 400);
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
                if (evento == "elimina_rel_exp" && tip_event == "add") {
                    service_registra_expediente();
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
function asig_add_edit_expediente(value_add_edit) {
    try {
        if (value_add_edit == "add") {
            document.getElementById("title_aad_edit_expediente").innerHTML = "Registrar expediente";
            document.getElementById("Button_aceptar_java").tip_event = value_add_edit;
        }
        if (value_add_edit == "edit") {
            document.getElementById("Button_aceptar_java").tip_event = value_add_edit;
            document.getElementById("title_aad_edit_expediente").innerHTML = "Editar expediente (" + document.getElementById("hdnEmailID").value + ")";          
        }
        if (value_add_edit == "vol") {
            document.getElementById("Button_aceptar_java").tip_event = value_add_edit;
            document.getElementById("title_aad_edit_expediente").innerHTML = "Crear y extender un volumen del expediente (" + document.getElementById("hdnEmailID").value + ")";
        }
    }
    catch (ex) {
        alert('asig_add_edit_expediente  ' + ex.message);
    }
}
function lista_ayuda_expediente() {
    var drowplist = $('#DropDownListBoxtipoexpediente');
    if (drowplist === undefined) {
        alert("Imposible encontrar DropDownListBoxtipoexpediente");
        return false;
    }
    var buton = $('#Button_lista_ayuda_expediente');
    if (buton === undefined) {
        alert("Imposible encontrar Button_lista_ayuda_expediente");
        return false;
    }
    buton.click();
}
function selecion_change_organigrama() {
    var drowplist = $('#DropDownListorganigrama');
    if (drowplist == undefined) {
        alert("Imposible encontrar DropDownListorganigrama");
        return false;
    }
    var buton = $('#Button_selecion_organigrama');
    if (buton == undefined) {
        alert("Imposible encontrar Button_selecion_organigrama");
        return false;
    }
    buton.click();
}
function selecion_change_area() {
    var drowplist = $('#DropDownListArea');
    if (drowplist == undefined) {
        alert("Imposible encontrar DropDownListArea");
        return false;
    }
    var buton = $('#Button_selecion_area');
    if (buton == undefined) {
        alert("Imposible encontrar Button_selecion_area");
        return false;
    }
    buton.click();
}

function selecion_change_serie() {
    var drowplist = $('#DropDownListSerie');
    if (drowplist == undefined) {
        alert("Imposible encontrar DropDownListSerie");
        return false;
    }
    var buton = $('#Button_selecion_serie');
    if (buton == undefined) {
        alert("Imposible encontrar Button_selecion_serie");
        return false;
    }
    buton.click();
}

function changue_option_manual() {
    $('#Button_lista_ayuda_tipo_unidad').click();


}
function changue_manual_consecutivo() {
    try {
        
    if (document.getElementById("CheckBoxActivaCodigomanual").checked == false) {
        document.getElementById("TextBoxCodigoManual").readOnly = false;
        document.getElementById("TextBoxCodigoManual").style.backgroundColor = "white";
    } else {
        document.getElementById("TextBoxCodigoManual").readOnly = true;
        document.getElementById("TextBoxCodigoManual").style.backgroundColor = "gray";
        }
    }
    catch (ex) {
        alert('changue_manual_consecutivo  ' + ex.message);
    }
}
var ITEMS_DATOS_TOKENIZE_2 = [];
function Service_relacionar_como_expediente_volumen(id_expediente_padre, id_expediente_volumen) {
    try {
        $.ajax('../webservice/WebServiceGaExpediente.asmx/Service_relacionar_como_expediente_volumen', {
            data: "{'id_expediente_padre':'" + id_expediente_padre + "','id_expediente_volumen':'" + id_expediente_volumen + "'}",
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
                    cambia_estado_expediente_volumen(id_expediente_padre, id_expediente_volumen);
                    cambia_expediente_padre_volumen(id_expediente_padre);
                    document.getElementById("Button_cerrar_padres_relacionados").click();
                    ESTADO_EVENT_GENERAL = "out";
                }
            }
        });
    }
    catch (ex) {
        alert('Service_des_registrar_expediente_volumen  ' + ex.message);
        ESTADO_EVENT_GENERAL = "out";
    }
}
function Service_des_registrar_expediente_volumen(id_expediente, cierra_ventana) {
    try {
        $.ajax('../webservice/WebServiceGaExpediente.asmx/Service_des_registrar_expediente_volumen', {
            data: "{'id_expediente':'" + id_expediente + "'}",
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
                    cambia_estado_expediente_volumen_a_expediente(id_expediente);
                    if (data.d[0].NUMERO_REL_VOLUMEN == "0") {
                        cambia_estado_expediente_padre_a_expediente(data.d[0].ID_EXPEDIENTE_PADRE);
                    }
                    if (cierra_ventana == 1) {
                        document.getElementById("Button_cerrar_volumenes_relacionados").click();
                    }
                    ESTADO_EVENT_GENERAL = "out";
                }
            }
        });
    }
    catch (ex) {
        alert('Service_des_registrar_expediente_volumen  ' + ex.message);
        ESTADO_EVENT_GENERAL = "out";
    }
}
function service_actualiza_expediente(id_expediente) {
    try {
        var estado_codigo_unico = "0";
        document.getElementById("result").textContent = "";
        if (document.getElementById("CheckBoxActivaCodigomanual").checked == true) {
            estado_codigo_unico = "1";
        } else { estado_codigo_unico = "2"; }
        if (document.getElementById("TextBoxCodigoManual").value == "" && document.getElementById("CheckBoxActivaCodigomanual").checked == false) {
            ESTADO_EVENT_GENERAL = "out";
            //alert("Por favor digite el codigo de identificacion  del expediente"); 
            document.getElementById("result").textContent = "Por favor digite el codigo de identificacion  del expediente";
            document.getElementById("TextBoxCodigoManual").focus();
            return true;
        }
        if (document.getElementById("TextBoxFECHA_EXTREMA_INICIAL").value == "") {
            //alert("Por favor seleccione la fecha del expediente");
            document.getElementById("result").textContent = "Por favor seleccione la fecha del expediente";
            ESTADO_EVENT_GENERAL = "out";
            document.getElementById("TextBoxFECHA_EXTREMA_INICIAL").focus();
            return true;
        }

        if (document.getElementById("DropDownListNOMBRE_TIPO_UNIDAD_DOCUMENTAL").value == "") {
            //alert("Por favor seleccione el tipo unidad documental al que pertenecerá el expediente");
            document.getElementById("result").textContent = "Por favor seleccione el tipo unidad documental";
            ESTADO_EVENT_GENERAL = "out";
            document.getElementById("DropDownListNOMBRE_TIPO_UNIDAD_DOCUMENTAL").focus();
            return true;
        }
        if (document.getElementById("DropDownListorganigrama").value == "") {
            //alert("Por favor seleccione el organigrama al que pertencerá el expediente");
            document.getElementById("result").textContent = "Por favor seleccione el organigrama";
            ESTADO_EVENT_GENERAL = "out";
            document.getElementById("DropDownListorganigrama").focus();
            return true;
        }
        if (document.getElementById("DropDownListBoxtipoexpediente").value == "") {
            //alert("Por favor seleccione el tipo de expediente");
            document.getElementById("result").textContent = "Por favor seleccione el tipo de expediente";
            document.getElementById("DropDownListBoxtipoexpediente").focus();
            return true;
        }
        if (document.getElementById("DropDownListArea").value == "") {
            //alert("Por favor seleccione el area o departamento al que pertenecerá el expediente");
            document.getElementById("result").textContent = "Por favor seleccione el area o departamento";
            ESTADO_EVENT_GENERAL = "out";
            document.getElementById("DropDownListArea").focus();
            return true;
        }
        if (document.getElementById("DropDownListBoxtipoexpediente").value == "") {
            //alert("Por favor seleccione el tipo de expediente");
            document.getElementById("result").textContent = "Por favor seleccione el tipo de expediente";
            ESTADO_EVENT_GENERAL = "out";
            document.getElementById("DropDownListBoxtipoexpediente").focus();
            return true;
        }
        if (document.getElementById("TextBoxNUMERO_ELECTRONICO_CONTENIDO").value == "") {
            //alert("Por favor digite el número de documentos electronicos");
            document.getElementById("result").textContent = "Por favor digite el número de documentos electronicos";
            ESTADO_EVENT_GENERAL = "out";
            document.getElementById("TextBoxNUMERO_ELECTRONICO_CONTENIDO").focus();
            return true;
        }
        if (document.getElementById("TextBoxNUMERO_FOLIOS_CONTENIDOS").value == "") {
            //alert("Por favor digite el número de folios físicos");
            document.getElementById("result").textContent = "Por favor digite el número de folios físicos";
            ESTADO_EVENT_GENERAL = "out";
            document.getElementById("TextBoxNUMERO_FOLIOS_CONTENIDOS").focus();
            return true;
        }
        if (document.getElementById("TextBoxNUMERO_FOLIOS_CONTENIDOS").value == "") {
            //alert("Por favor digite el número de folios físicos");
            document.getElementById("result").textContent = "Por favor digite el número de folios físicos";
            ESTADO_EVENT_GENERAL = "out";
            document.getElementById("TextBoxNUMERO_FOLIOS_CONTENIDOS").focus();
            return true;
        }
        if (document.getElementById("TextBoxNUMERO_DIGITALIZADO_CONTENIDO").value == "") {
            //alert("Por favor digite el número de documentos digitalizados");
            document.getElementById("result").textContent = "Por favor digite el número de documentos digitalizados";
            ESTADO_EVENT_GENERAL = "out";
            document.getElementById("TextBoxNUMERO_DIGITALIZADO_CONTENIDO").focus();
            return true;
        }
        var para_meter_ca = new Array();
        var id_expediente = "0";
        var option_obliga_archivo_unidad = "0";
        var id_unidad_contenedora = "0";
        var aleas_expediente = "";
        var expediente_relacion = "0";
        var id_expediente_registrado = "0";
        var serie_documental = "";
        var sub_serie_documental = "";
        var id_istrumento = "0";
        var ciclo_archivo = "";
        var nombre_tipo_unidad_documental = "";
        var organigrama = "";
        var tipo_expdiente = "";
        var codigo_unico_ = document.getElementById("TextBoxCodigoManual").value;
        var id_empresa_ = document.getElementById("Hidden_id_empresa").value;
        var fecha_expediente = document.getElementById("TextBoxFECHA_EXTREMA_INICIAL").value;
        var fecha_final_expediente = document.getElementById("TextBoxFECHA_EXTREMA_FINAL").value;
        var rango_extremo_inicial = document.getElementById("TextBoxRANGO_EXTREMO_INICIAL").value;
        var rango_extremo_final = document.getElementById("TextBoxRANGO_EXTREMO_FINAL").value;
        var tema_expediente = document.getElementById("TextBoxTEMA_EXPEDIENTE").value;
        var registro_organigrama = document.getElementById("DropDownListorganigrama").value;
        var nombre_area = document.getElementById("DropDownListArea").value;
        var tipo_expediente = document.getElementById("DropDownListBoxtipoexpediente").value;
        var numero_digitalizado = document.getElementById("TextBoxNUMERO_DIGITALIZADO_CONTENIDO").value;
        var numero_folio = document.getElementById("TextBoxNUMERO_FOLIOS_CONTENIDOS").value;
        var numero_electronico = document.getElementById("TextBoxNUMERO_ELECTRONICO_CONTENIDO").value;
        var asunto_expediente = document.getElementById("TextBoxASUNTO_EXPEDIENTE").value;
        var tipo_unidad_conservacion = document.getElementById("DropDownList_tipo_unidad_conservacion").value;
        var observacion_expediente = document.getElementById("TextBoxOBSERVACION_EXPEDIENTE").value;
        var nombre_sub_area = document.getElementById("DropDownListsub_seccion").value;
        var nombre_fondo = document.getElementById("DropDownListNOMBRE_FONDO").value;
        var nombre_persona_expediente = document.getElementById("TextBoxNOMBRE_PERSONA_EXPEDIENTE").value;
        var identificacion_persona_expediente = document.getElementById("TextBoxIDENTIFICACION_PERSONA_EXPEDIENTE").value;
        var nombre_responsable_expediente = document.getElementById("TextBoxNOMBRE_RESPONSABLE_EXPEDIENTE").value;
        var identificacion_responsable_expediente = document.getElementById("TextBoxIDENFICACION_RESPONSABLE_EXPEDIENTE").value;
        var aleas_expediente = "";
        var expediente_padre = "0";
        var gabinete_produccion = "PRODUCIONDOC";
        var id_nivel_padre = "0";
        var id_registro_relacion = "0";
        if (document.getElementById("DropDownListSerie").selectedIndex > -1) {
            serie_documental = document.getElementById("DropDownListSerie").options[document.getElementById("DropDownListSerie").selectedIndex].text;
        }
        if (document.getElementById("DropDownListSubserie").selectedIndex > -1) {
            sub_serie_documental = document.getElementById("DropDownListSubserie").options[document.getElementById("DropDownListSubserie").selectedIndex].text;
        }
        id_istrumento = document.getElementById("DropDownList_instrumento").value;
        if (document.getElementById("DropDownListNOMBRE_CICLO_ARCHIVO").selectedIndex > -1) {
            ciclo_archivo = document.getElementById("DropDownListNOMBRE_CICLO_ARCHIVO").options[document.getElementById("DropDownListNOMBRE_CICLO_ARCHIVO").selectedIndex].text;
        }

        nombre_tipo_unidad_documental = document.getElementById("DropDownListNOMBRE_TIPO_UNIDAD_DOCUMENTAL").value;
        organigrama = document.getElementById("DropDownListorganigrama").value;
        tipo_expdiente = document.getElementById("DropDownListBoxtipoexpediente").value;
        para_meter_ca.push({
            CODIGO_UNICO: codigo_unico_, ESTADO_CODIGO_UNICO: estado_codigo_unico, ID_EMPRESA_GESTION: id_empresa_, FECHA_INICIAL_EXPEDICION: fecha_expediente
            , FECHA_FINAL_TERMINACION: fecha_final_expediente, RANGO_EXTREMO_INICIAL: rango_extremo_inicial, RANGO_EXTREMO_FINAL: rango_extremo_final, TEMA: tema_expediente
            , REGISTRO_ORGANIGRAMA: registro_organigrama, NOMBRE_AREA: nombre_area, NOMBRE_SERIE: serie_documental, NOMBRE_SUBSERIE: sub_serie_documental
            , TIPO_EXPEDIENTE: tipo_expediente, FOLIO_DIGITALIZADO: numero_digitalizado, FOLIO_FISICO: numero_folio, FOLIO_ELECTRONICO: numero_electronico, ASUNTO: asunto_expediente
            , TIPO_UNIDAD_DOCUMENTAL: nombre_tipo_unidad_documental, OBSERVACION_EXPEDIENTE: observacion_expediente, NOMBRE_SUB_AREA: nombre_sub_area, NOMBRE_CICLO_ARCHIVO: ciclo_archivo
            , NOMBRE_FONDO: nombre_fondo, NOMBRE_SOLICITANTE: nombre_persona_expediente, IDENTIFICACION_SOLICITANTE: identificacion_persona_expediente, RESPONSABLE_EXPEDIENTE: nombre_responsable_expediente
            , IDENFICACION_RESPONSABLE: identificacion_responsable_expediente, ALEAS_EXPEDIENTE: aleas_expediente, EXPEDIENTE_PADRE: expediente_padre, ID_INSTRUMENTO: id_istrumento
            , GABINETE_PRODUCION: gabinete_produccion, ID_NIVEL_PADRE: id_nivel_padre, ID_REGISTRO_RELACION: id_registro_relacion, TIPO_UNIDAD_CONSERVACION: tipo_unidad_conservacion
        });
        var serialice = JSON.stringify(para_meter_ca);
        $.ajax('../webservice/WebServiceGaExpediente.asmx/Service_actualiza_expediente', {
            data: "{ 'parameter':'" + serialice + "'," + "'id_expediente':'" + document.getElementById("hdnEmailID").value + "'}",
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
                    ITEMS_DATOS_TOKENIZE_2 = [];
                    $.each(data.d, function (k, v) {
                        ITEMS_DATOS_TOKENIZE_2.push(v);
                    });      
                    actualiza_gredview_expediente(document.getElementById("hdnEmailID").value);
                    document.getElementById("Button_cerrar_add_edit_expediente").click();
                    ESTADO_EVENT_GENERAL = "out";
                    
                }
            }
        });
    }
    catch (ex) {
        alert('service_actualiza_expediente  ' + ex.message);
        ESTADO_EVENT_GENERAL = "out";
    }
}
function actualiza_gredview_expediente(id_expediente) {
    try {  
        Object.entries(ITEMS_DATOS_TOKENIZE_2[0]).forEach(([key, value]) => {
            if (key == "CODIGO_UNICO") {
                actualiza_gre_campo("data_grid", id_expediente, value, "CONSECUTIVO");
            } else {
                actualiza_gre_campo("data_grid", id_expediente, value, key);
            }
        });
      }
    catch (ex) {
        alert('actualiza_gredview_expediente  ' + ex.message);
    E
}
}
function service_registra_expediente() {
    try {
        var estado_codigo_unico = "0";
        document.getElementById("result").textContent = "";
        if (document.getElementById("CheckBoxActivaCodigomanual").checked == true) {
            estado_codigo_unico = "1";
        } else { estado_codigo_unico = "2"; }
        if (document.getElementById("TextBoxCodigoManual").value == "" && document.getElementById("CheckBoxActivaCodigomanual").checked == false) {
            ESTADO_EVENT_GENERAL = "out";
            //alert("Por favor digite el codigo de identificacion  del expediente"); 
            document.getElementById("result").textContent = "Por favor digite el codigo de identificacion  del expediente";
            document.getElementById("TextBoxCodigoManual").focus();
            return true;
        }
        if (document.getElementById("TextBoxFECHA_EXTREMA_INICIAL").value == "") {
            //alert("Por favor seleccione la fecha del expediente");
            document.getElementById("result").textContent = "Por favor seleccione la fecha del expediente";
            ESTADO_EVENT_GENERAL = "out";
            document.getElementById("TextBoxFECHA_EXTREMA_INICIAL").focus();
            return true;
        }

        if (document.getElementById("DropDownListNOMBRE_TIPO_UNIDAD_DOCUMENTAL").value == "") {
            //alert("Por favor seleccione el tipo unidad documental al que pertenecerá el expediente");
            document.getElementById("result").textContent = "Por favor seleccione el tipo unidad documental";
            ESTADO_EVENT_GENERAL = "out";
            document.getElementById("DropDownListNOMBRE_TIPO_UNIDAD_DOCUMENTAL").focus();
            return true;
        }
        if (document.getElementById("DropDownListorganigrama").value == "") {
            //alert("Por favor seleccione el organigrama al que pertencerá el expediente");
            document.getElementById("result").textContent = "Por favor seleccione el organigrama";
            ESTADO_EVENT_GENERAL = "out";
            document.getElementById("DropDownListorganigrama").focus();
            return true;
        }
        if (document.getElementById("DropDownListBoxtipoexpediente").value == "") {
            //alert("Por favor seleccione el tipo de expediente");
            document.getElementById("result").textContent = "Por favor seleccione el tipo de expediente";
            document.getElementById("DropDownListBoxtipoexpediente").focus();
            return true;
        }
        if (document.getElementById("DropDownListArea").value == "") {
            //alert("Por favor seleccione el area o departamento al que pertenecerá el expediente");
            document.getElementById("result").textContent = "Por favor seleccione el area o departamento";
            ESTADO_EVENT_GENERAL = "out";
            document.getElementById("DropDownListArea").focus();
            return true;
        }
        if (document.getElementById("DropDownListBoxtipoexpediente").value == "") {
            //alert("Por favor seleccione el tipo de expediente");
            document.getElementById("result").textContent = "Por favor seleccione el tipo de expediente";
            ESTADO_EVENT_GENERAL = "out";
            document.getElementById("DropDownListBoxtipoexpediente").focus();
            return true;
        }
        if (document.getElementById("TextBoxNUMERO_ELECTRONICO_CONTENIDO").value == "") {
            //alert("Por favor digite el número de documentos electronicos");
            document.getElementById("result").textContent = "Por favor digite el número de documentos electronicos";
            ESTADO_EVENT_GENERAL = "out";
            document.getElementById("TextBoxNUMERO_ELECTRONICO_CONTENIDO").focus();
            return true;
        }
        if (document.getElementById("TextBoxNUMERO_FOLIOS_CONTENIDOS").value == "") {
            //alert("Por favor digite el número de folios físicos");
            document.getElementById("result").textContent = "Por favor digite el número de folios físicos";
            ESTADO_EVENT_GENERAL = "out";
            document.getElementById("TextBoxNUMERO_FOLIOS_CONTENIDOS").focus();
            return true;
        }
        if (document.getElementById("TextBoxNUMERO_FOLIOS_CONTENIDOS").value == "") {
            //alert("Por favor digite el número de folios físicos");
            document.getElementById("result").textContent = "Por favor digite el número de folios físicos";
            ESTADO_EVENT_GENERAL = "out";
            document.getElementById("TextBoxNUMERO_FOLIOS_CONTENIDOS").focus();
            return true;
        }
        if (document.getElementById("TextBoxNUMERO_DIGITALIZADO_CONTENIDO").value == "") {
            //alert("Por favor digite el número de documentos digitalizados");
            document.getElementById("result").textContent = "Por favor digite el número de documentos digitalizados";
            ESTADO_EVENT_GENERAL = "out";
            document.getElementById("TextBoxNUMERO_DIGITALIZADO_CONTENIDO").focus();
            return true;
        }
        var para_meter_ca = new Array();
        var id_expediente = "0";
        var option_obliga_archivo_unidad = "0";
        var id_unidad_contenedora = "0";
        var aleas_expediente = "";
        var expediente_relacion = "0";
        var id_expediente_registrado = "0";
        var serie_documental = "";
        var sub_serie_documental = "";
        var id_istrumento = "0";
        var ciclo_archivo = "";
        var nombre_tipo_unidad_documental = "";
        var organigrama = "";
        var tipo_expdiente = "";
        var codigo_unico_ = document.getElementById("TextBoxCodigoManual").value;
        var id_empresa_ = document.getElementById("Hidden_id_empresa").value;
        var fecha_expediente = document.getElementById("TextBoxFECHA_EXTREMA_INICIAL").value;
        var fecha_final_expediente = document.getElementById("TextBoxFECHA_EXTREMA_FINAL").value;
        var rango_extremo_inicial = document.getElementById("TextBoxRANGO_EXTREMO_INICIAL").value;
        var rango_extremo_final = document.getElementById("TextBoxRANGO_EXTREMO_FINAL").value;
        var tema_expediente = document.getElementById("TextBoxTEMA_EXPEDIENTE").value;
        var registro_organigrama = document.getElementById("DropDownListorganigrama").value;
        var nombre_area = document.getElementById("DropDownListArea").value;
        var tipo_expediente = document.getElementById("DropDownListBoxtipoexpediente").value;
        var numero_digitalizado = document.getElementById("TextBoxNUMERO_DIGITALIZADO_CONTENIDO").value;
        var numero_folio = document.getElementById("TextBoxNUMERO_FOLIOS_CONTENIDOS").value;
        var numero_electronico = document.getElementById("TextBoxNUMERO_ELECTRONICO_CONTENIDO").value;
        var asunto_expediente = document.getElementById("TextBoxASUNTO_EXPEDIENTE").value;
        var tipo_unidad_conservacion = document.getElementById("DropDownList_tipo_unidad_conservacion").value;
        var observacion_expediente = document.getElementById("TextBoxOBSERVACION_EXPEDIENTE").value;
        var nombre_sub_area = document.getElementById("DropDownListsub_seccion").value;
        var nombre_fondo = document.getElementById("DropDownListNOMBRE_FONDO").value;
        var nombre_persona_expediente = document.getElementById("TextBoxNOMBRE_PERSONA_EXPEDIENTE").value;
        var identificacion_persona_expediente = document.getElementById("TextBoxIDENTIFICACION_PERSONA_EXPEDIENTE").value;
        var nombre_responsable_expediente = document.getElementById("TextBoxNOMBRE_RESPONSABLE_EXPEDIENTE").value;
        var identificacion_responsable_expediente = document.getElementById("TextBoxIDENFICACION_RESPONSABLE_EXPEDIENTE").value;
        var aleas_expediente = "";
        var expediente_padre = "0";
        var gabinete_produccion = "PRODUCIONDOC";
        var id_nivel_padre = "0";
        var id_registro_relacion = "0";
        if (document.getElementById("DropDownListSerie").selectedIndex > -1) {
            serie_documental = document.getElementById("DropDownListSerie").options[document.getElementById("DropDownListSerie").selectedIndex].text;
        }
        if (document.getElementById("DropDownListSubserie").selectedIndex > -1) {
            sub_serie_documental = document.getElementById("DropDownListSubserie").options[document.getElementById("DropDownListSubserie").selectedIndex].text;
        }
        id_istrumento = document.getElementById("DropDownList_instrumento").value;
        if (document.getElementById("DropDownListNOMBRE_CICLO_ARCHIVO").selectedIndex > -1) {
            ciclo_archivo = document.getElementById("DropDownListNOMBRE_CICLO_ARCHIVO").options[document.getElementById("DropDownListNOMBRE_CICLO_ARCHIVO").selectedIndex].text;
        }

        nombre_tipo_unidad_documental = document.getElementById("DropDownListNOMBRE_TIPO_UNIDAD_DOCUMENTAL").value;
        organigrama = document.getElementById("DropDownListorganigrama").value;
        tipo_expdiente = document.getElementById("DropDownListBoxtipoexpediente").value;
        para_meter_ca.push({
            CODIGO_UNICO: codigo_unico_, ESTADO_CODIGO_UNICO: estado_codigo_unico, ID_EMPRESA_GESTION: id_empresa_, FECHA_INICIAL_EXPEDICION: fecha_expediente
            , FECHA_FINAL_TERMINACION: fecha_final_expediente, RANGO_EXTREMO_INICIAL: rango_extremo_inicial, RANGO_EXTREMO_FINAL: rango_extremo_final, TEMA: tema_expediente
            , REGISTRO_ORGANIGRAMA: registro_organigrama, NOMBRE_AREA: nombre_area, NOMBRE_SERIE: serie_documental, NOMBRE_SUBSERIE: sub_serie_documental
            , TIPO_EXPEDIENTE: tipo_expediente, FOLIO_DIGITALIZADO: numero_digitalizado, FOLIO_FISICO: numero_folio, FOLIO_ELECTRONICO: numero_electronico, ASUNTO: asunto_expediente
            , TIPO_UNIDAD_DOCUMENTAL: nombre_tipo_unidad_documental, OBSERVACION_EXPEDIENTE: observacion_expediente, NOMBRE_SUB_AREA: nombre_sub_area, NOMBRE_CICLO_ARCHIVO: ciclo_archivo
            , NOMBRE_FONDO: nombre_fondo, NOMBRE_SOLICITANTE: nombre_persona_expediente, IDENTIFICACION_SOLICITANTE: identificacion_persona_expediente, RESPONSABLE_EXPEDIENTE: nombre_responsable_expediente
            , IDENFICACION_RESPONSABLE: identificacion_responsable_expediente, ALEAS_EXPEDIENTE: aleas_expediente, EXPEDIENTE_PADRE: expediente_padre, ID_INSTRUMENTO: id_istrumento
            , GABINETE_PRODUCION: gabinete_produccion, ID_NIVEL_PADRE: id_nivel_padre, ID_REGISTRO_RELACION: id_registro_relacion, TIPO_UNIDAD_CONSERVACION: tipo_unidad_conservacion
        });
        var serialice = JSON.stringify(para_meter_ca);
        $.ajax('../webservice/WebServiceGaExpediente.asmx/Service_registra_expediente', {
            data: "{ 'parameter':'" + serialice + "'}",
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
                    ITEMS_DATOS_TOKENIZE_2 = new Array();
                    $.each(data.d, function (k, v) {
                        ITEMS_DATOS_TOKENIZE_2.push(v);
                    });
                    ESTADO_EVENT_GENERAL = "out";
                    insert_row_registro_expediente();
                    document.getElementById("Button_show_printer").click();
                    //actualiza_node_treview("", "", "TreeViewArchivo", "../Gestion/imagenes/share-light.png");
                }
            }
        });
    }
    catch (ex) {
        alert('service_registra_expediente  ' + ex.message);
        ESTADO_EVENT_GENERAL = "out";


    }
}
function service_registra_expediente_volumen() {
    try {
        var estado_codigo_unico = "0";
        document.getElementById("result").textContent = "";
        if (document.getElementById("CheckBoxActivaCodigomanual").checked == true) {
            estado_codigo_unico = "1";
        } else { estado_codigo_unico = "2"; }
        if (document.getElementById("TextBoxCodigoManual").value == "" && document.getElementById("CheckBoxActivaCodigomanual").checked == false) {
            ESTADO_EVENT_GENERAL = "out";
            //alert("Por favor digite el codigo de identificacion  del expediente"); 
            document.getElementById("result").textContent = "Por favor digite el codigo de identificacion  del expediente";
            document.getElementById("TextBoxCodigoManual").focus();
            return true;
        }
        if (document.getElementById("TextBoxFECHA_EXTREMA_INICIAL").value == "") {
            //alert("Por favor seleccione la fecha del expediente");
            document.getElementById("result").textContent = "Por favor seleccione la fecha del expediente";
            ESTADO_EVENT_GENERAL = "out";
            document.getElementById("TextBoxFECHA_EXTREMA_INICIAL").focus();
            return true;
        }

        if (document.getElementById("DropDownListNOMBRE_TIPO_UNIDAD_DOCUMENTAL").value == "") {
            //alert("Por favor seleccione el tipo unidad documental al que pertenecerá el expediente");
            document.getElementById("result").textContent = "Por favor seleccione el tipo unidad documental";
            ESTADO_EVENT_GENERAL = "out";
            document.getElementById("DropDownListNOMBRE_TIPO_UNIDAD_DOCUMENTAL").focus();
            return true;
        }
        if (document.getElementById("DropDownListorganigrama").value == "") {
            //alert("Por favor seleccione el organigrama al que pertencerá el expediente");
            document.getElementById("result").textContent = "Por favor seleccione el organigrama";
            ESTADO_EVENT_GENERAL = "out";
            document.getElementById("DropDownListorganigrama").focus();
            return true;
        }
        if (document.getElementById("DropDownListBoxtipoexpediente").value == "") {
            //alert("Por favor seleccione el tipo de expediente");
            document.getElementById("result").textContent = "Por favor seleccione el tipo de expediente";
            document.getElementById("DropDownListBoxtipoexpediente").focus();
            return true;
        }
        if (document.getElementById("DropDownListArea").value == "") {
            //alert("Por favor seleccione el area o departamento al que pertenecerá el expediente");
            document.getElementById("result").textContent = "Por favor seleccione el area o departamento";
            ESTADO_EVENT_GENERAL = "out";
            document.getElementById("DropDownListArea").focus();
            return true;
        }
        if (document.getElementById("DropDownListBoxtipoexpediente").value == "") {
            //alert("Por favor seleccione el tipo de expediente");
            document.getElementById("result").textContent = "Por favor seleccione el tipo de expediente";
            ESTADO_EVENT_GENERAL = "out";
            document.getElementById("DropDownListBoxtipoexpediente").focus();
            return true;
        }
        if (document.getElementById("TextBoxNUMERO_ELECTRONICO_CONTENIDO").value == "") {
            //alert("Por favor digite el número de documentos electronicos");
            document.getElementById("result").textContent = "Por favor digite el número de documentos electronicos";
            ESTADO_EVENT_GENERAL = "out";
            document.getElementById("TextBoxNUMERO_ELECTRONICO_CONTENIDO").focus();
            return true;
        }
        if (document.getElementById("TextBoxNUMERO_FOLIOS_CONTENIDOS").value == "") {
            //alert("Por favor digite el número de folios físicos");
            document.getElementById("result").textContent = "Por favor digite el número de folios físicos";
            ESTADO_EVENT_GENERAL = "out";
            document.getElementById("TextBoxNUMERO_FOLIOS_CONTENIDOS").focus();
            return true;
        }
        if (document.getElementById("TextBoxNUMERO_FOLIOS_CONTENIDOS").value == "") {
            //alert("Por favor digite el número de folios físicos");
            document.getElementById("result").textContent = "Por favor digite el número de folios físicos";
            ESTADO_EVENT_GENERAL = "out";
            document.getElementById("TextBoxNUMERO_FOLIOS_CONTENIDOS").focus();
            return true;
        }
        if (document.getElementById("TextBoxNUMERO_DIGITALIZADO_CONTENIDO").value == "") {
            //alert("Por favor digite el número de documentos digitalizados");
            document.getElementById("result").textContent = "Por favor digite el número de documentos digitalizados";
            ESTADO_EVENT_GENERAL = "out";
            document.getElementById("TextBoxNUMERO_DIGITALIZADO_CONTENIDO").focus();
            return true;
        }
        var para_meter_ca = new Array();
        var id_expediente = "0";
        var option_obliga_archivo_unidad = "0";
        var id_unidad_contenedora = "0";
        var aleas_expediente = "";
        var expediente_relacion = "0";
        var id_expediente_registrado = "0";
        var serie_documental = "";
        var sub_serie_documental = "";
        var id_istrumento = "0";
        var ciclo_archivo = "";
        var nombre_tipo_unidad_documental = "";
        var organigrama = "";
        var tipo_expdiente = "";
        var codigo_unico_ = document.getElementById("TextBoxCodigoManual").value;
        var id_empresa_ = document.getElementById("Hidden_id_empresa").value;
        var fecha_expediente = document.getElementById("TextBoxFECHA_EXTREMA_INICIAL").value;
        var fecha_final_expediente = document.getElementById("TextBoxFECHA_EXTREMA_FINAL").value;
        var rango_extremo_inicial = document.getElementById("TextBoxRANGO_EXTREMO_INICIAL").value;
        var rango_extremo_final = document.getElementById("TextBoxRANGO_EXTREMO_FINAL").value;
        var tema_expediente = document.getElementById("TextBoxTEMA_EXPEDIENTE").value;
        var registro_organigrama = document.getElementById("DropDownListorganigrama").value;
        var nombre_area = document.getElementById("DropDownListArea").value;
        var tipo_expediente = document.getElementById("DropDownListBoxtipoexpediente").value;
        var numero_digitalizado = document.getElementById("TextBoxNUMERO_DIGITALIZADO_CONTENIDO").value;
        var numero_folio = document.getElementById("TextBoxNUMERO_FOLIOS_CONTENIDOS").value;
        var numero_electronico = document.getElementById("TextBoxNUMERO_ELECTRONICO_CONTENIDO").value;
        var asunto_expediente = document.getElementById("TextBoxASUNTO_EXPEDIENTE").value;
        var tipo_unidad_conservacion = document.getElementById("DropDownList_tipo_unidad_conservacion").value;
        var observacion_expediente = document.getElementById("TextBoxOBSERVACION_EXPEDIENTE").value;
        var nombre_sub_area = document.getElementById("DropDownListsub_seccion").value;
        var nombre_fondo = document.getElementById("DropDownListNOMBRE_FONDO").value;
        var nombre_persona_expediente = document.getElementById("TextBoxNOMBRE_PERSONA_EXPEDIENTE").value;
        var identificacion_persona_expediente = document.getElementById("TextBoxIDENTIFICACION_PERSONA_EXPEDIENTE").value;
        var nombre_responsable_expediente = document.getElementById("TextBoxNOMBRE_RESPONSABLE_EXPEDIENTE").value;
        var identificacion_responsable_expediente = document.getElementById("TextBoxIDENFICACION_RESPONSABLE_EXPEDIENTE").value;
        var aleas_expediente = "";
        var expediente_padre = "0";
        var gabinete_produccion = "PRODUCIONDOC";
        var id_nivel_padre = "0";
        var id_registro_relacion = "0";
        if (document.getElementById("DropDownListSerie").selectedIndex > -1) {
            serie_documental = document.getElementById("DropDownListSerie").options[document.getElementById("DropDownListSerie").selectedIndex].text;
        }
        if (document.getElementById("DropDownListSubserie").selectedIndex > -1) {
            sub_serie_documental = document.getElementById("DropDownListSubserie").options[document.getElementById("DropDownListSubserie").selectedIndex].text;
        }
        id_istrumento = document.getElementById("DropDownList_instrumento").value;
        if (document.getElementById("DropDownListNOMBRE_CICLO_ARCHIVO").selectedIndex > -1) {
            ciclo_archivo = document.getElementById("DropDownListNOMBRE_CICLO_ARCHIVO").options[document.getElementById("DropDownListNOMBRE_CICLO_ARCHIVO").selectedIndex].text;
        }

        nombre_tipo_unidad_documental = document.getElementById("DropDownListNOMBRE_TIPO_UNIDAD_DOCUMENTAL").value;
        organigrama = document.getElementById("DropDownListorganigrama").value;
        tipo_expdiente = document.getElementById("DropDownListBoxtipoexpediente").value;
        para_meter_ca.push({
            CODIGO_UNICO: codigo_unico_, ESTADO_CODIGO_UNICO: estado_codigo_unico, ID_EMPRESA_GESTION: id_empresa_, FECHA_INICIAL_EXPEDICION: fecha_expediente
            , FECHA_FINAL_TERMINACION: fecha_final_expediente, RANGO_EXTREMO_INICIAL: rango_extremo_inicial, RANGO_EXTREMO_FINAL: rango_extremo_final, TEMA: tema_expediente
            , REGISTRO_ORGANIGRAMA: registro_organigrama, NOMBRE_AREA: nombre_area, NOMBRE_SERIE: serie_documental, NOMBRE_SUBSERIE: sub_serie_documental
            , TIPO_EXPEDIENTE: tipo_expediente, FOLIO_DIGITALIZADO: numero_digitalizado, FOLIO_FISICO: numero_folio, FOLIO_ELECTRONICO: numero_electronico, ASUNTO: asunto_expediente
            , TIPO_UNIDAD_DOCUMENTAL: nombre_tipo_unidad_documental, OBSERVACION_EXPEDIENTE: observacion_expediente, NOMBRE_SUB_AREA: nombre_sub_area, NOMBRE_CICLO_ARCHIVO: ciclo_archivo
            , NOMBRE_FONDO: nombre_fondo, NOMBRE_SOLICITANTE: nombre_persona_expediente, IDENTIFICACION_SOLICITANTE: identificacion_persona_expediente, RESPONSABLE_EXPEDIENTE: nombre_responsable_expediente
            , IDENFICACION_RESPONSABLE: identificacion_responsable_expediente, ALEAS_EXPEDIENTE: aleas_expediente, EXPEDIENTE_PADRE: expediente_padre, ID_INSTRUMENTO: id_istrumento
            , GABINETE_PRODUCION: gabinete_produccion, ID_NIVEL_PADRE: id_nivel_padre, ID_REGISTRO_RELACION: id_registro_relacion, TIPO_UNIDAD_CONSERVACION: tipo_unidad_conservacion, ID_EXPEDIENTE: document.getElementById("hdnEmailID").value
        });
        var serialice = JSON.stringify(para_meter_ca);
        $.ajax('../webservice/WebServiceGaExpediente.asmx/Service_registra_expediente_volumen', {
            data: "{ 'parameter':'" + serialice + "'}",
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
                    ITEMS_DATOS_TOKENIZE_2 = new Array();
                    $.each(data.d, function (k, v) {
                        ITEMS_DATOS_TOKENIZE_2.push(v);
                    });
                    ESTADO_EVENT_GENERAL = "out";
                    insert_row_registro_expediente();
                    document.getElementById("Button_show_printer").click();
                    cambia_expediente_padre_volumen(document.getElementById("hdnEmailID").value);
                    //actualiza_node_treview("", "", "TreeViewArchivo", "../Gestion/imagenes/share-light.png");
                }
            }
        });
    }
    catch (ex) {
        alert('service_registra_expediente_volumen  ' + ex.message);
        ESTADO_EVENT_GENERAL = "out";


    }
}
function insert_row_registro_expediente() {
    try {
        var element_table = document.getElementById("data_grid");
        if (element_table) {
        } else {
            return true;
        }
        if (ITEMS_DATOS_TOKENIZE_2.length == 0) {
            return true;
        }
        var value_insert = 1;
        var element_class_row_pag = element_table.rows.item(0).className;
        if (element_class_row_pag == "pagination-ys") {
            value_insert = 2;
        }
        var conta_td = 0;
        var element_row = element_table.insertRow(value_insert);
        var element_td = element_row.insertCell(conta_td);
        element_row.id = ITEMS_DATOS_TOKENIZE_2[0].ID_EXPEDIENTE;
        element_row.style.cursor = "pointer";
        element_row.style.background = "white";
        element_row.style.color = "black";
        element_row.classList.add("GridviewScrollItem_line_cort");
        var divhtml = document.createElement("div");
        var ihtml = document.createElement("i");
        ihtml.style.color = "white";
        if (ITEMS_DATOS_TOKENIZE_2[0].NUMERO_ELECTRONICO_CONTENIDO > 0) {
            ihtml.classList.add("fal");
            ihtml.classList.add("fa-folder-open");
            ihtml.classList.add("fa-lg");
        } else {
            ihtml.classList.add("fad");
            ihtml.classList.add("fa-folder-open");
            ihtml.classList.add("fa-lg");
        }
        var ahtml = document.createElement("a");
        ahtml.classList.add("btn");
        ahtml.classList.add("btn-success");
        ahtml.classList.add("btn-sm");
        ahtml.setAttribute("onclick", "prevent(event,this)");
        ahtml.setAttribute("title", "Ver documentos relacionados");
        ahtml.setAttribute("id", ITEMS_DATOS_TOKENIZE_2[0].ID_EXPEDIENTE);
        ahtml.setAttribute("tip_event", "ver_doc_col");
        ahtml.style.marginLeft = "3px";
        ahtml.appendChild(ihtml);
        divhtml.appendChild(ahtml);

        ihtml = document.createElement("i");
        ihtml.style.color = "white";
        ahtml = document.createElement("a");
        ahtml.classList.add("btn");
        ahtml.classList.add("btn-danger");
        ahtml.classList.add("btn-sm");
        if (ITEMS_DATOS_TOKENIZE_2[0].CONSECUTIVO_EXPEDIENTE_2 > 1) {
            ihtml.classList.add("fal");
            ihtml.classList.add("fa-database");
            ihtml.classList.add("fa-lg");
            ahtml.setAttribute("onclick", "prevent(event,this)");
            ahtml.setAttribute("title", "Expediente padre, click para listar expedientes relacionados");
            ahtml.setAttribute("id_list_rel_", ITEMS_DATOS_TOKENIZE_2[0].ID_EXPEDIENTE);
            ahtml.setAttribute("tip_event", "display_exp");
            ahtml.style.marginLeft = "3px";
            ahtml.appendChild(ihtml);
            divhtml.appendChild(ahtml);
        }
        if (ITEMS_DATOS_TOKENIZE_2[0].VOLUMEN_EXPEDIENTE > 1) {
            ihtml.classList.add("fal");
            ihtml.classList.add("fa-coins");
            ihtml.classList.add("fa-lg");
            ahtml.setAttribute("onclick", "prevent(event,this)");
            ahtml.setAttribute("title", "Expediente volumen del expediente (" + ITEMS_DATOS_TOKENIZE_2[0].EXPEDIENTE_PADRE + "), presione click para desvincular");
            ahtml.setAttribute("idd_image_rel", ITEMS_DATOS_TOKENIZE_2[0].ID_EXPEDIENTE);
            ahtml.setAttribute("idd_expediente_rel_padre", ITEMS_DATOS_TOKENIZE_2[0].EXPEDIENTE_PADRE);
            ahtml.setAttribute("tip_event", "elimina_rel_exp");
            ahtml.style.marginLeft = "3px";
            ahtml.appendChild(ihtml);
            divhtml.appendChild(ahtml);
        }
        
        if (ITEMS_DATOS_TOKENIZE_2[0].VOLUMEN_EXPEDIENTE == 1 && ITEMS_DATOS_TOKENIZE_2[0].VOLUMEN_EXPEDIENTE == 1) {
            ihtml.classList.add("fal");
            ihtml.classList.add("fa-folder-plus");
            ihtml.classList.add("fa-lg");
            ahtml.setAttribute("onclick", "prevent(event,this)");
            ahtml.setAttribute("title", "Expediente, click para relacionar como voulmen ");
            ahtml.setAttribute("id_list_rel", ITEMS_DATOS_TOKENIZE_2[0].ID_EXPEDIENTE);
            ahtml.setAttribute("tip_event", "activa_rel_exp");
            ahtml.style.marginLeft = "3px";
            ahtml.appendChild(ihtml);
            divhtml.appendChild(ahtml);
        }
        ihtml = document.createElement("i");
        ihtml.style.color = "white";
        ahtml = document.createElement("a");
        ahtml.classList.add("btn");
        ahtml.classList.add("btn-dark");
        ahtml.classList.add("btn-sm");
        ahtml.style.marginLeft = "3px";
        if (ITEMS_DATOS_TOKENIZE_2[0].ESTADO_GESTION_EXPEDIENTE == 1) {
            ihtml.classList.add("fal");
            ihtml.classList.add("fa-paste");
            ihtml.classList.add("fa-lg");
            ahtml.setAttribute("onclick", "prevent(event,this)");
            ahtml.setAttribute("title", "Copiar documentos al expediente");
            ahtml.setAttribute("id_expediente_", ITEMS_DATOS_TOKENIZE_2[0].ID_EXPEDIENTE);
            ahtml.setAttribute("tip_event", "copia_documento_expediente");
            ahtml.appendChild(ihtml);
            divhtml.appendChild(ahtml);
        }
        if (ITEMS_DATOS_TOKENIZE_2[0].ESTADO_GESTION_EXPEDIENTE == 2) {
            ihtml.classList.add("fal");
            ihtml.classList.add("fa-folder-download");
            ihtml.classList.add("fa-lg");
            ahtml.setAttribute("onclick", "prevent(event,this)");
            ahtml.setAttribute("title", "Vincular documentos al expediente");
            ahtml.setAttribute("id_expediente_", ITEMS_DATOS_TOKENIZE_2[0].ID_EXPEDIENTE);
            ahtml.setAttribute("tip_event", "vincula_documento_expediente");
            ahtml.appendChild(ihtml);
            divhtml.appendChild(ahtml);
        }

        if (ITEMS_DATOS_TOKENIZE_2[0].ESTADO_GESTION_EXPEDIENTE == 3) {
            ihtml.classList.add("fal");
            ihtml.classList.add("fa-folder-upload");
            ihtml.classList.add("fa-lg");
            ahtml.setAttribute("onclick", "prevent(event,this)");
            ahtml.setAttribute("title", "Asigna expediente al nuevo radicado");
            ahtml.setAttribute("idd", ITEMS_DATOS_TOKENIZE_2[0].ID_EXPEDIENTE);
            ahtml.setAttribute("tip_event", "asig_exp");
            ahtml.appendChild(ihtml);
            divhtml.appendChild(ahtml);
        }
       
        divhtml.style.display = "inline-flex";
        element_td.appendChild(divhtml);
        conta_td++;
        element_td = element_row.insertCell(conta_td);
        element_td.innerHTML = ITEMS_DATOS_TOKENIZE_2[0].ID_EXPEDIENTE;
        element_td.classList.add("GridviewScrollItem_line_cort_tr_flex");
        element_td.setAttribute("onclick", "prevent_scrol(event,this,'')");
            //element_td.setAttribute("ondblclick", "prevent_scrol_ondblclick(event,this)");
        conta_td++;
        element_td = element_row.insertCell(conta_td);
        element_td.innerHTML = ITEMS_DATOS_TOKENIZE_2[0].CODIGO_UNICO;
        element_td.classList.add("GridviewScrollItem_line_cort_tr_flex");
        element_td.setAttribute("onclick", "prevent_scrol(event,this,'')");
            //element_td.setAttribute("ondblclick", "prevent_scrol_ondblclick(event,this)");
        conta_td++;
        element_td = element_row.insertCell(conta_td);
        element_td.innerHTML = ITEMS_DATOS_TOKENIZE_2[0].NOMBRE_SERIE_TRD;
        element_td.classList.add("GridviewScrollItem_line_cort_tr_flex");
        element_td.setAttribute("onclick", "prevent_scrol(event,this,'')");
            //element_td.setAttribute("ondblclick", "prevent_scrol_ondblclick(event,this)");
        conta_td++;
        element_td = element_row.insertCell(conta_td);
        element_td.innerHTML = ITEMS_DATOS_TOKENIZE_2[0].NOMBRE_SUBSERIE_TRD;
        element_td.classList.add("GridviewScrollItem_line_cort_tr_flex");
        element_td.setAttribute("onclick", "prevent_scrol(event,this,'')");
            //element_td.setAttribute("ondblclick", "prevent_scrol_ondblclick(event,this)");
        conta_td++;
        element_td = element_row.insertCell(conta_td);
        element_td.innerHTML = ITEMS_DATOS_TOKENIZE_2[0].TEMA;
        element_td.classList.add("GridviewScrollItem_line_cort_tr_flex");
        element_td.setAttribute("onclick", "prevent_scrol(event,this,'')");
            //element_td.setAttribute("ondblclick", "prevent_scrol_ondblclick(event,this)");
        conta_td++;
        element_td = element_row.insertCell(conta_td);
        element_td.innerHTML = ITEMS_DATOS_TOKENIZE_2[0].ASUNTO;
        element_td.classList.add("GridviewScrollItem_line_cort_tr_flex");
        element_td.setAttribute("onclick", "prevent_scrol(event,this,'')");
            //element_td.setAttribute("ondblclick", "prevent_scrol_ondblclick(event,this)");
        conta_td++;
        element_td = element_row.insertCell(conta_td);
        element_td.innerHTML = ITEMS_DATOS_TOKENIZE_2[0].FECHA_CREACION;
        element_td.classList.add("GridviewScrollItem_line_cort_tr_flex");
        element_td.setAttribute("onclick", "prevent_scrol(event,this,'')");
            //element_td.setAttribute("ondblclick", "prevent_scrol_ondblclick(event,this)");
        conta_td++;
        element_td = element_row.insertCell(conta_td);
        element_td.innerHTML = ITEMS_DATOS_TOKENIZE_2[0].CODIGO_AREA_TRD;
        element_td.classList.add("GridviewScrollItem_line_cort_tr_flex");
        element_td.setAttribute("onclick", "prevent_scrol(event,this,'')");
            //element_td.setAttribute("ondblclick", "prevent_scrol_ondblclick(event,this)");
        conta_td++;
        element_td = element_row.insertCell(conta_td);
        element_td.innerHTML = ITEMS_DATOS_TOKENIZE_2[0].NOMBRE_AREA_TRD;
        element_td.classList.add("GridviewScrollItem_line_cort_tr_flex");
        element_td.setAttribute("onclick", "prevent_scrol(event,this,'')");
        conta_td++;
        element_td = element_row.insertCell(conta_td);
        element_td.innerHTML = ITEMS_DATOS_TOKENIZE_2[0].CODIGO_SERIE_TRD;
        element_td.classList.add("GridviewScrollItem_line_cort_tr_flex");
        element_td.setAttribute("onclick", "prevent_scrol(event,this,'')");
            //element_td.setAttribute("ondblclick", "prevent_scrol_ondblclick(event,this)");
        conta_td++;
        element_td = element_row.insertCell(conta_td);
        element_td.innerHTML = ITEMS_DATOS_TOKENIZE_2[0].CODIGO_SUB_SERIE_TRD;
        element_td.classList.add("GridviewScrollItem_line_cort_tr_flex");
        element_td.setAttribute("onclick", "prevent_scrol(event,this,'')");
            //element_td.setAttribute("ondblclick", "prevent_scrol_ondblclick(event,this)");
        conta_td++;
        element_td = element_row.insertCell(conta_td);
        element_td.innerHTML = ITEMS_DATOS_TOKENIZE_2[0].TIPO_UNIDAD_DOCUMENTAL;
        element_td.classList.add("GridviewScrollItem_line_cort_tr_flex");
        element_td.setAttribute("onclick", "prevent_scrol(event,this,'')");
            //element_td.setAttribute("ondblclick", "prevent_scrol_ondblclick(event,this)");
        conta_td++;
        element_td = element_row.insertCell(conta_td);
        element_td.innerHTML = ITEMS_DATOS_TOKENIZE_2[0].COMPOSICION_EXPEDIENTE;
        element_td.classList.add("GridviewScrollItem_line_cort_tr_flex");
        element_td.setAttribute("onclick", "prevent_scrol(event,this,'')");
            //element_td.setAttribute("ondblclick", "prevent_scrol_ondblclick(event,this)");
        conta_td++;
        element_td = element_row.insertCell(conta_td);
        element_td.innerHTML = ITEMS_DATOS_TOKENIZE_2[0].FECHA_INICIAL_EXPEDICION;
        element_td.classList.add("GridviewScrollItem_line_cort_tr_flex");
        element_td.setAttribute("onclick", "prevent_scrol(event,this,'')");
            //element_td.setAttribute("ondblclick", "prevent_scrol_ondblclick(event,this)");
        conta_td++;
        element_td = element_row.insertCell(conta_td);
        element_td.innerHTML = ITEMS_DATOS_TOKENIZE_2[0].FECHA_FINAL_TERMINACION;
        element_td.classList.add("GridviewScrollItem_line_cort_tr_flex");
        element_td.setAttribute("onclick", "prevent_scrol(event,this,'')");
            //element_td.setAttribute("ondblclick", "prevent_scrol_ondblclick(event,this)");
        conta_td++;
        element_td = element_row.insertCell(conta_td);
        element_td.innerHTML = ITEMS_DATOS_TOKENIZE_2[0].RANGO_EXTREMO_INICIAL;
        element_td.classList.add("GridviewScrollItem_line_cort_tr_flex");
        element_td.setAttribute("onclick", "prevent_scrol(event,this,'')");
            //element_td.setAttribute("ondblclick", "prevent_scrol_ondblclick(event,this)");
        conta_td++;
        element_td = element_row.insertCell(conta_td);
        element_td.innerHTML = ITEMS_DATOS_TOKENIZE_2[0].RANGO_EXTREMO_FINAL;
        element_td.classList.add("GridviewScrollItem_line_cort_tr_flex");
        element_td.setAttribute("onclick", "prevent_scrol(event,this,'')");
            //element_td.setAttribute("ondblclick", "prevent_scrol_ondblclick(event,this)");
        conta_td++;
        element_td = element_row.insertCell(conta_td);
        element_td.innerHTML = ITEMS_DATOS_TOKENIZE_2[0].ESTADO_EXPEDIENTE;
        element_td.classList.add("GridviewScrollItem_line_cort_tr_flex");
        element_td.setAttribute("onclick", "prevent_scrol(event,this,'')");
            //element_td.setAttribute("ondblclick", "prevent_scrol_ondblclick(event,this)");
        conta_td++;
        element_td = element_row.insertCell(conta_td);
        element_td.innerHTML = ITEMS_DATOS_TOKENIZE_2[0].NOMBRE_SOLICITANTE;
        element_td.classList.add("GridviewScrollItem_line_cort_tr_flex");
        element_td.setAttribute("onclick", "prevent_scrol(event,this,'')");
            //element_td.setAttribute("ondblclick", "prevent_scrol_ondblclick(event,this)");
        conta_td++;
        element_td = element_row.insertCell(conta_td);
        element_td.innerHTML = ITEMS_DATOS_TOKENIZE_2[0].IDENTIFICACION_SOLICITANTE;
        element_td.classList.add("GridviewScrollItem_line_cort_tr_flex");
        element_td.setAttribute("onclick", "prevent_scrol(event,this,'')");
            //element_td.setAttribute("ondblclick", "prevent_scrol_ondblclick(event,this)");
        conta_td++;
        element_td = element_row.insertCell(conta_td);
        element_td.innerHTML = ITEMS_DATOS_TOKENIZE_2[0].RESPONSABLE_EXPEDIENTE;
        element_td.classList.add("GridviewScrollItem_line_cort_tr_flex");
        element_td.setAttribute("onclick", "prevent_scrol(event,this,'')");
            //element_td.setAttribute("ondblclick", "prevent_scrol_ondblclick(event,this)");
        conta_td++;
        element_td = element_row.insertCell(conta_td);
        element_td.innerHTML = ITEMS_DATOS_TOKENIZE_2[0].IDENFICACION_RESPONSABLE;
        element_td.classList.add("GridviewScrollItem_line_cort_tr_flex");
        element_td.setAttribute("onclick", "prevent_scrol(event,this,'')");
            //element_td.setAttribute("ondblclick", "prevent_scrol_ondblclick(event,this)");
        conta_td++;
        element_td = element_row.insertCell(conta_td);
        element_td.innerHTML = ITEMS_DATOS_TOKENIZE_2[0].NOMBRE_FONDO;
        element_td.classList.add("GridviewScrollItem_line_cort_tr_flex");
        element_td.setAttribute("onclick", "prevent_scrol(event,this,'')");
            //element_td.setAttribute("ondblclick", "prevent_scrol_ondblclick(event,this)");
        conta_td++;
        element_td = element_row.insertCell(conta_td);
        element_td.innerHTML = ITEMS_DATOS_TOKENIZE_2[0].NOMBRE_CICLO_ARCHIVO;
        element_td.classList.add("GridviewScrollItem_line_cort_tr_flex");
        element_td.setAttribute("onclick", "prevent_scrol(event,this,'')");
            //element_td.setAttribute("ondblclick", "prevent_scrol_ondblclick(event,this)");

        conta_td++;
        element_td = element_row.insertCell(conta_td);
        element_td.innerHTML = ITEMS_DATOS_TOKENIZE_2[0].FOLIO_FISICO;
        element_td.classList.add("GridviewScrollItem_line_cort_tr_flex");
        element_td.setAttribute("onclick", "prevent_scrol(event,this,'')");
            //element_td.setAttribute("ondblclick", "prevent_scrol_ondblclick(event,this)");

        conta_td++;
        element_td = element_row.insertCell(conta_td);
        element_td.innerHTML = ITEMS_DATOS_TOKENIZE_2[0].FOLIO_DIGITALIZADO;
        element_td.classList.add("GridviewScrollItem_line_cort_tr_flex");
        element_td.setAttribute("onclick", "prevent_scrol(event,this,'')");
            //element_td.setAttribute("ondblclick", "prevent_scrol_ondblclick(event,this)");
        conta_td++;
        element_td = element_row.insertCell(conta_td);
        element_td.innerHTML = ITEMS_DATOS_TOKENIZE_2[0].FOLIO_ELECTRONICO;
        element_td.classList.add("GridviewScrollItem_line_cort_tr_flex");
        element_td.setAttribute("onclick", "prevent_scrol(event,this,'')");
            //element_td.setAttribute("ondblclick", "prevent_scrol_ondblclick(event,this)");
        var atri_pagi = parseInt($(element_table).attr("pag_util"));
        if (element_class_row_pag == "pagination-ys" ) {
            if ((element_table.rows.length - 2) >= atri_pagi) {
                element_table.deleteRow(element_table.rows.length-1);
            }           
        }
        var cant_record = parseInt(document.getElementById("Hidden_0001").value) + 1;
        document.getElementById("titulo_label_expedientes").innerHTML = cant_record + " registro(s) de expediente (s)";
        document.getElementById("Hidden_0001").value = cant_record;

    } catch (err) {
        alert(err.message + " Funcion insert_row_registro_expediente");
    }
}
function service_documentos_clasificacion(name_texbox) {
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
                    url: "../webservice/WebServiceRadicacion.asmx/GetLista_documentos_clasificacion",
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
            },


            focus: function () {
                // prevent value inserted on focus
                return false;
            },
            select: function (event, ui) {
                document.getElementById(name_texbox).value = ui.item.label;
                document.getElementById("Button_busqueda_documento").click();
            }

            , minLength: 3, max: 10, scroll: true
        });
}
function service_get_lista_expediente(name_texbox) {
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
                    url: "../webservice/WebServiceGaExpediente.asmx/GetLista_expedientes_gestion",
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
            },


            focus: function () {
                // prevent value inserted on focus
                return false;
            },
            select: function (event, ui) {
                document.getElementById(name_texbox).value = ui.item.label;
                document.getElementById("ButtonConsultaLike").click();
                //this.value = ui.item.value;
                //return false;
            }

            , minLength: 1, max: 10, scroll: true
        });
}
function service_get_lista_expediente_relacion(name_texbox) {
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
                    url: "../webservice/WebServiceGaExpediente.asmx/GetLista_expedientes_gestion",
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
            },
            focus: function () {
                // prevent value inserted on focus
                return false;
            },
            select: function (event, ui) {
                document.getElementById(name_texbox).value = ui.item.label;
                document.getElementById("ImageButton_buscar").click();
               
            }
            , minLength: 1, max: 10, scroll: true
        });
}
function service_get_lista_expediente_relacion_padre(name_texbox) {
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
                    url: "../webservice/WebServiceGaExpediente.asmx/GetLista_expedientes_gestion",
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
            },
            focus: function () {
                // prevent value inserted on focus
                return false;
            },
            select: function (event, ui) {
                document.getElementById(name_texbox).value = ui.item.label;
                document.getElementById("ImageButton_buscar_volumen").click();

            }
            , minLength: 1, max: 10, scroll: true
        });
}
function service_get_lista_auto_complete_expediente(name_texbox, campo) {
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
                    url: "../webservice/WebServiceGaExpediente.asmx/GetLista_campos_expedientes_gestion",
                    data: "{'DName':'" + document.getElementById(name_texbox).value + "','CAmpo':'" + campo + "'}",
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
            },
            focus: function () {
                // prevent value inserted on focus
                return false;
            },
            select: function (event, ui) {
                document.getElementById(name_texbox).value = ui.item.label;
                //document.getElementById("ImageButton_buscar").click();

            }
            , minLength: 1, max: 10, scroll: true
        });
}
function acti_busq_general_documento(e, sender) {
    try {

        tecla = (document.all) ? e.keyCode : e.which;
        if (tecla == 13) {
            document.getElementById("Button_busqueda_documento").click();
            e.preventDefault();
        }
    } catch (err) {
        alert(err.message + " funcion acti_busq_general_documento " + err.message);
    }
}
function activa_boton_client_documento(e, sender) {
    try {      
        document.getElementById("Button_busqueda_documento").click();
        e.preventDefault();
    } catch (ex) {
        alert("Inconistencia general funcion activa_boton_client_documento " + ex.message);
    }
}
function acti_busq_general_archivo_boton(e, sender) {
    try {
        document.getElementById("ButtonConsultaLike").click();
        e.preventDefault();
    } catch (ex) {
        alert("Inconistencia general funcion acti_busq_general_archivo_boton " + ex.message);
    }
}
function restore_acti_busq_general_archivo_boton(e, sender) {
    try {
        document.getElementById("ButtonConsulta").click();
        e.preventDefault();
    } catch (ex) {
        alert("Inconistencia general funcion restore_acti_busq_general_archivo_boton " + ex.message);
    }
}
function restore_lista_documentos_relacionados(e, sender) {
    try {
        document.getElementById("Button_documentos_relacionado").click();
    e.preventDefault();
    } catch (ex) {
        alert("Inconistencia general funcion restore_lista_documentos_relacionados " + ex.message);
    }
}
function activa_busqueda_volumenes_relacionados(e, sender) {
    try {
        document.getElementById("ImageButton_buscar").click();
        e.preventDefault();
    } catch (ex) {
        alert("Inconistencia general funcion restore_lista_documentos_relacionados " + ex.message);
    }
}
function activa_busqueda_volumenes_relacionados_padre(e, sender) {
    try {
        document.getElementById("ImageButton_buscar_volumen").click();
        e.preventDefault();
    } catch (ex) {
        alert("Inconistencia general funcion activa_busqueda_volumenes_relacionados_padre " + ex.message);
    }
}
function activa_busqueda_volumenes_relacionados_enter(e, sender) {
    try {
        tecla = (document.all) ? e.keyCode : e.which;
        if (tecla == 13) {
            document.getElementById("ImageButton_buscar").click();
            e.preventDefault();
        }
        document.getElementById("Button_documentos_relacionado").click();
        e.preventDefault();
    } catch (ex) {
        alert("Inconistencia general funcion activa_busqueda_volumenes_relacionados_enter " + ex.message);
    }
}
function prevent_scrol_(event, e) {
    try {

        if (e.className == "GridviewScrollItem_line_corte_tr") {
            e.classList.remove("GridviewScrollItem_line_corte_tr");
            e.classList.toggle("GridviewScrollItem_line_corte_tr_scrol");
        } else {
            e.classList.remove("GridviewScrollItem_line_corte_tr_scrol");
            e.classList.toggle("GridviewScrollItem_line_corte_tr");
        }
        event.preventDefault();
    }
    catch (err) {
        alert(err.message + " Funcion prevent_scrol_");
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
function eliminar_fila_data_gred_lista(gred, name_hide) {
    try {
       
        $("#" + gred + " tr[id=" + $("#" + name_hide).val() + "]").remove();  
        $("#" + name_hide).val("-1");
        
    }
    catch (err) {
        alert(err.message + " Funcion eliminar_fila_data_gred_lista");
    }

}
function cambia_icono_gred(gred) {
    try {
        var t = $("input[id_list_rel=" + $("#hdnEmailID").val() + "]");
        var src_;
        if (t[0]) {
            src_ = t[0].src.replace("folder-light-vol.png", "folder-light.png");
            $(t).attr("title", "Expediente, click para relacionar como voulmen");
            $(t).attr("tip_event", "activa_rel_exp");
            $(t).attr('src', src_);
        }
       
    }
    catch (err) {
        alert(err.message + " Funcion cambia_icono_gred");
    }

}
function cambia_icono_gred_volumen() {
    try {
        var t = $("input[id_list_rel=" + $("#hdnEmailID").val() + "]");
        var src_;
        if (t[0]) {
            src_ = t[0].src.replace("folder-light.png", "folder-light-vol.png");
            $(t).attr('src', src_);
            $(t).attr("tip_event", "elimina_rel_exp");
            $(t).attr("title", "Expediente volumen de expediente ");
        }
      
    }
    catch (err) {
        alert(err.message + " Funcion cambia_icono_gred_volumen");
    }

}
function cambia_expediente_padre_volumen(id_expeidente) {
    try {
        var t = $("a[id_list_rel=" + id_expeidente + "]");
        if (t[0]) {
            var parent_ = t[0].parentElement;
            var ahtml = document.createElement("a");
            var ihtml = document.createElement("i");
            ihtml.style.color = "white";
            ahtml.classList.add("btn");
            ahtml.classList.add("btn-danger");
            ahtml.classList.add("btn-sm");
            ihtml.classList.add("fal");
            ihtml.classList.add("fa-database");
            ihtml.classList.add("fa-lg");
            ahtml.setAttribute("onclick", "prevent(event,this)");
            ahtml.setAttribute("title", "Expediente padre, click para listar expedientes relacionados");
            ahtml.setAttribute("id_list_rel_", id_expeidente);
            ahtml.setAttribute("tip_event", "display_exp");
            ahtml.style.marginLeft = "3px";
            ahtml.appendChild(ihtml);
            parent_.appendChild(ahtml);
            var ob = t[0];
            ob.remove();
            
        }
    }
    catch (err) {
        alert(err.message + " Funcion cambia_icono_gred_volumen");
    }
}
function cambia_estado_expediente_padre_a_expediente(id_expeidente) {
    try {
        var t = $("a[id_list_rel_=" + id_expeidente + "]");
        if (t[0]) {
            var parent_ = t[0].parentElement;
            var ahtml = document.createElement("a");
            var ihtml = document.createElement("i");
            ihtml.style.color = "white";
            ahtml.classList.add("btn");
            ahtml.classList.add("btn-danger");
            ahtml.classList.add("btn-sm");
            ihtml.classList.add("fal");
            ihtml.classList.add("fa-folder-plus");
            ihtml.classList.add("fa-lg");
            ahtml.setAttribute("onclick", "prevent(event,this)");
            ahtml.setAttribute("title", "Expediente, click para relacionar como voulmen");
            ahtml.setAttribute("id_list_rel", id_expeidente);
            ahtml.setAttribute("tip_event", "activa_rel_exp");
            ahtml.style.marginLeft = "3px";
            ahtml.appendChild(ihtml);
            parent_.appendChild(ahtml);
            var ob = t[0];
            ob.remove();

        }
    }
    catch (err) {
        alert(err.message + " Funcion cambia_estado_expediente_padre_a_expediente");
    }
}
function cambia_estado_expediente_volumen_a_expediente(id_expeidente) {
    try {
        var t = $("a[idd_image_rel=" + id_expeidente + "]");
        if (t[0]) {
            var parent_ = t[0].parentElement;
            var ahtml = document.createElement("a");
            var ihtml = document.createElement("i");
            ihtml.style.color = "white";
            ahtml.classList.add("btn");
            ahtml.classList.add("btn-danger");
            ahtml.classList.add("btn-sm");
            ihtml.classList.add("fal");
            ihtml.classList.add("fa-folder-plus");
            ihtml.classList.add("fa-lg");
            ahtml.setAttribute("onclick", "prevent(event,this)");
            ahtml.setAttribute("title", "Expediente, click para relacionar como voulmen");
            ahtml.setAttribute("id_list_rel", id_expeidente);
            ahtml.setAttribute("tip_event", "activa_rel_exp");
            ahtml.style.marginLeft = "3px";
            ahtml.appendChild(ihtml);
            parent_.appendChild(ahtml);
            var ob = t[0];
            ob.remove();

        }
    }
    catch (err) {
        alert(err.message + " Funcion cambia_estado_expediente_padre_a_expediente");
    }
}
function cambia_estado_expediente_volumen(id_expediente_padre, id_expediente_volumen) {
    try {
        var t = $("a[id_list_rel=" + id_expediente_volumen + "]");
        if (t[0]) {
            var parent_ = t[0].parentElement;
            var ahtml = document.createElement("a");
            var ihtml = document.createElement("i");
            ihtml.style.color = "white";
            ahtml.classList.add("btn");
            ahtml.classList.add("btn-danger");
            ahtml.classList.add("btn-sm");
            ihtml.classList.add("fal");
            ihtml.classList.add("fa-coins");
            ihtml.classList.add("fa-lg");
            ahtml.setAttribute("onclick", "prevent(event,this)");
            ahtml.setAttribute("title", "Expediente volumen del expediente (" + id_expediente_padre + "), presione click para desvincular");
            ahtml.setAttribute("idd_image_rel", id_expediente_volumen);
            ahtml.setAttribute("idd_expediente_rel_padre", id_expediente_padre);
            ahtml.setAttribute("tip_event", "elimina_rel_exp");
            ahtml.style.marginLeft = "3px";
            ahtml.appendChild(ihtml);
            parent_.appendChild(ahtml);
            var ob = t[0];
            ob.remove();


        }
    }
    catch (err) {
        alert(err.message + " Funcion cambia_estado_expediente_padre_a_expediente");
    }
}
function prevent_menu_element(event, value) {
    try {
       
        if (value == "elimina_rel_exp") {
            $('#Hidden_eli_rel').val($('#hdnEmailID').val());
            document.getElementById("Button_active_eli_rel").click();
        }
      
        event.preventDefault();
        
    }
    catch (err) {
        alert(err.message + " Funcion prevent_menu_element");
    }
}
function prevent(event, element) {
    try {
        var fer = $(element).attr("idd_image");
        var tip_event = $(element).attr("tip_event");
        if (tip_event == "asig_exp" && document.getElementById("Button_asigna_expediente_gestion")) {
            $('#hdnEmailID').val($(element).attr("idd"));
            document.getElementById("Button_asigna_expediente_gestion").click();
        }
        if (tip_event == "display_exp") {
            $('#hdnEmailID').val($(element).attr("id_list_rel_"));
            document.getElementById("Button_listar_volumenes_relacionados").click();
            document.getElementById("Label_title_volumenes_relacionados").innerHTML = "Volúmunes relacionados al expediente " + $('#hdnEmailID').val();
            auto_zise_popup_volumenes_relacionados();
        }
        //activa eliminar relacion expeidente padre volumen 
        if (tip_event == "elimina_rel_exp") {
            var id_expediente_padre = $(element).attr("idd_expediente_rel_padre");
            var id_expediente = $(element).attr("idd_image_rel");
            var x = "";
            var r = confirm("Desea desvincular el expediente (" + id_expediente + ") como  expediente volumen del expediente (" + id_expediente_padre + ")");
            if (r == false) {
                return true;
            }            
            Service_des_registrar_expediente_volumen(id_expediente, 0);
        }
        //activa eliminar relacion expeidente padre volumen  desde ventana auxiliar
        if (tip_event == "elimina_rel_exp_") {
            var id_expediente_padre = $(element).attr("idd_expediente_rel_padre_");
            var id_expediente = $(element).attr("idd_image_rel_");
            var x = "";
            var r = confirm("Desea desvincular el expediente (" + id_expediente + ") como  expediente volumen del expediente (" + id_expediente_padre + ")");
            if (r == false) {
                return true;
            }
            Service_des_registrar_expediente_volumen(id_expediente, 1);
        }
        //activa ventana relacionar volumen
        if (tip_event == "activa_rel_exp") {
            $('#hdnEmailID').val($(element).attr("id_list_rel"));
            document.getElementById("Label_title_padres_relacionados").innerHTML = "Seleccione expediente padre o principal a relacionar con el volumen " + $(element).attr("id_list_rel");
            document.getElementById("Button_activa_ventana_rel_volumen").click();
            
        }      
        if (tip_event == "relacion_rel_exp") {
            var id_expediente_padre = $(element).attr("idd_image_rel");
            var id_expediente_volumen = $('#hdnEmailID').val();
            var x = "";
            var r = confirm("Desea vincular el expediente (" + id_expediente_volumen + ") como  expediente volumen del expediente (" + id_expediente_padre + ")");
            if (r == false) {
                return true;
            }
            Service_relacionar_como_expediente_volumen(id_expediente_padre, id_expediente_volumen);
           
        }
        if (tip_event == "relacion_rel_exp_") {
            var id_expediente_padre = $(element).attr("idd_image_rel_");
            var id_expediente_volumen = $('#hdnEmailID').val();
            var x = "";
            var r = confirm("Desea vincular el expediente (" + id_expediente_volumen + ") como  expediente volumen del expediente (" + id_expediente_padre + ")");
            if (r == false) {
                return true;
            }
            Service_relacionar_como_expediente_volumen(id_expediente_padre, id_expediente_volumen);

        }
        if (tip_event == "relacion_rel_exp_padre") {
            var id_expediente_volumen = $(element).attr("idd_image_rel_padre");
            var id_expediente_padre = $('#hdnEmailID').val();
            var x = "";
            var r = confirm("Desea vincular el expediente (" + id_expediente_volumen + ") como  expediente volumen del expediente (" + id_expediente_padre + ")");
            if (r == false) {
                return true;
            }
            Service_relacionar_como_expediente_volumen(id_expediente_padre, id_expediente_volumen);

        }
        if (tip_event == "ver_doc_col") {
            var fer = $(element).attr("id");
            $('#hdnEmailID').val(fer);
            document.getElementById("Button_documentos_relacionado").click();
        }
        if (tip_event == "ver_doc") {
            var fer_ = $(element).attr("id");
            var gab = $(element).attr("idd");
            $('#hdnEmailID_documentos').val(fer_);
            $('#Hidden_gabienete').val(gab);
            document.getElementById("Button_ver_documento").click();
        }
        if (tip_event == "copia_documento_expediente") {
            var id_exp_cop_doc = $(element).attr("id_expediente_");
            $('#Hidden0008').val(id_exp_cop_doc);
            event_multiple_row(event, 'data_grid', 'cop_file_service_expediente');         
        }
        if (tip_event == "vincula_documento_expediente") {
            var id_exp_cop_doc = $(element).attr("id_expediente_");
            $('#Hidden0008').val(id_exp_cop_doc);
            event_multiple_row(event, 'data_grid', 'vincula_file_service_expediente');
        }
        event.preventDefault();
    }
    catch (err) {
        alert(err.message + " Funcion prevent");
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
function auto_zise_popup_padres_relacionados() {
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
        var gridwith = with_frame - 2;
        var gridheihg = 250;
        $('#content_data_grid_padres_relacionados').css("height", (gridheihg) + "px");
        $('#Panel_grid_padres_relacionados').css("height", (gridheihg) + "px");
        gridwith = document.getElementById("Panel_grid_padres_relacionados").offsetWidth - 3;
        if ($('#data_grid_padres_relacionados td').children.length > 0 && $('#data_grid_padres_relacionados tr:visible').length > 0) {
            // $(document).ready(function () { $('#data_grid_padres_relacionados').gridviewScroll({ width: gridwith, height: (gridheihg - 5) }); })

        }

    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_padres_relacionados " + err.message);
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
function auto_zise_popup_padres_relacionados() {
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
        $('#Panel_padres_relacionados').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_padres_relacionados').css("height", (heig_porcent ) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#contenido_procesa_padres_relacionados').css("height", (document.getElementById("modal_content_padres_relacionados").clientHeight - (document.getElementById("div_mod_title_padres_relacionados").clientHeight)) + "px");
        //Para los modal que contiene gred  
        $('#Panel_grid_padres_relacionados').css("height", (document.getElementById("contenido_procesa_padres_relacionados").clientHeight - (document.getElementById("div_label_title").clientHeight + document.getElementById("div_rel_padre").clientHeight + document.getElementById("Div_title_resultados_relacionados").clientHeight + 30)) + "px");
        $('#content_data_grid_padres_relacionados').css("height", (document.getElementById("contenido_procesa_padres_relacionados").clientHeight - (document.getElementById("div_label_title").clientHeight + document.getElementById("div_rel_padre").clientHeight + document.getElementById("Div_title_resultados_relacionados").clientHeight + 30)) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_padres_relacionados " + err.message);
    }
}
function togle_volumen_padre() {
    $('#home_relacion_volumen_content').toggleClass('active');
    $('#home_relacion_volumen_content').toggleClass('show');
}
function auto_zise_popup_volumenes_relacionados() {
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
        $('#Panel_volumenes_relacionados').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_volumenes_relacionados').css("height", (heig_porcent) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#contenido_procesa_volumenes_relacionados').css("height", (document.getElementById("modal_content_volumenes_relacionados").clientHeight - (document.getElementById("div_mod_title_volumenes_relacionados").clientHeight + document.getElementById("item_nav_tab_volumen").clientHeight)) + "px");
        //Para los modal que contiene gred  
      
        if (document.getElementById("home_relacion_volumen_content").className.indexOf("active") > 0) {
            
            $('#Panel_grid_volumenes_relacionados').css("height", (document.getElementById("contenido_procesa_volumenes_relacionados").clientHeight - (document.getElementById("contenido_titulo_val_radicacion_documentos").clientHeight + document.getElementById("div_contenedor_titulo_documentos_relacionados").clientHeight + 30)) + "px");
            $('#content_data_grid_volumenes_relacionados').css("height", (document.getElementById("contenido_procesa_volumenes_relacionados").clientHeight - (document.getElementById("contenido_titulo_val_radicacion_documentos").clientHeight + document.getElementById("div_contenedor_titulo_documentos_relacionados").clientHeight + 30)) + "px");
        }
        if (document.getElementById("home_relacionar_volumen_content").className.indexOf("active") > 0) {
           
            $('#Panel_grid_relacionar_volumen').css("height", (document.getElementById("contenido_procesa_volumenes_relacionados").clientHeight - (document.getElementById("div_rel_padre_volumen").clientHeight + document.getElementById("Div_title_resultados_relacionados_volumen").clientHeight + 30)) + "px");
            $('#content_data_grid_relacionar_volumen').css("height", (document.getElementById("contenido_procesa_volumenes_relacionados").clientHeight - (document.getElementById("div_rel_padre_volumen").clientHeight + document.getElementById("Div_title_resultados_relacionados_volumen").clientHeight + 30)) + "px");     
        }
       
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_volumenes_relacionados " + err.message);
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
//Funcion que asigna los expedintes creados desde el popup agreagar expediete
function cerrar_popup_agregar_expedinte() {
   $('#Button_actualiza_expdientes_agregados').click();
}
//Realiza consulta cuando cambia la empresa
function cambio_empresa_gestion_consulta() {
    $('#ButtonConsulta').click();
}
function clired() {
    $('#data_grid tr[id]').click(function () {
        $('#data_grid tr[id]').css({ "background-color": "White", "color": "Black" });
        $(this).css({ "background-color": "#e8e8f7", "color": "Black",  });
        var fer = $(this).attr("id");
        $('#hdnEmailID').val(fer);

    });
    $('#data_grid_documentos_exp tr[id]').click(function () {
        $('#data_grid_documentos_exp tr[id]').css({ "background-color": "White", "color": "Black" });
        $(this).css({ "background-color": "#e8e8f7", "color": "Black" });
       
    });
    //Mantenie el cursor de seleccion
    $('#data_grid tr[id]').mouseover(function () {
        $(this).css({ cursor: "hand", cursor: "pointer" });
    });
    //Mantiene seleccinado en todos los postback la celda del data grid de lado del cliente
    $('#data_grid tr[id=' + $('#hdnEmailID').val() + ']').css({ "background-color": "#e8e8f7", "color": "Black" });

    $('#data_grid tr[id]').dblclick(function () {

        if ($('#hdnEmailID_VAL').val() != "-1") {
            document.getElementById("Button_documentos_relacionado").click();
            return false;
        }

    });
}
function oculta_panel() {

}
/* funcion CONTROLA QUE EL BOTON DE ASIGNACION DE EXPEDIENTE SE VEAN SI ESTA EN UN PAREN O NO, requiere control
Hiddenheigpaginapopup,Hiddennameasigna,Hiddenid_expediente en el foumulario fuente
*/
function control_visualiza_boton_asignacion_expediente() {
  try{
    $('#Button_asigna_expediente_gestion').hide();
    var hidenvalue = $('#Hiddennameasigna', window.parent.document).val();
    if (hidenvalue != undefined ) {
        $('#Hiddennameasigna').val($('#Hiddennameasigna', window.parent.document).val());
        if (hidenvalue == "") {
            $('#Button_asigna_expediente_gestion').hide();
        } else {
            $('#Button_asigna_expediente_gestion').show();
        }     
       
    } else {
        $('#Button_asigna_expediente_gestion').hide();
        
    }
}
    catch (err) {
        alert(err.message + " Funcion control_visualiza_boton_asignacion_expediente");
}
}
/* funcion que determina a que pagina asigna el id del expediente, requiere control
Hiddenheigpaginapopup,Hiddennameasigna,Hiddenid_expediente en el foumulario fuente
*/ 
function importa_dato_expediente() {
    try {
    var namepaginapopup = $('#Hiddennameasigna', window.parent.document);
    if (namepaginapopup == undefined) {
        alert("Imposible encontrar el tipo seleccion en en la pagina fuente fal el control Hiddenheigpaginapopup");
        return false;
    }
    if (namepaginapopup.val() == "RADICACION_ENTRANTE") {
        asigna_expdiente_radicacion_entrante();
        return false;
    }

    if (namepaginapopup.val() == "EXPEDIENTE_WORKFLOW") {
        asigna_expediente_workflow_indice();
        //asigna_expdiente_radicacion_entrante();
        //return false;
    }
    if (namepaginapopup.val() == "DOCUARCHI_NET") {
        asigna_expediente_workflow_indice();
        return false;
    }
    if (namepaginapopup.val() == "DOCUARCHI.VISOR") {
        alert("No se puede asignar expediente desde la ventana de produción documental");
        return false;
    }
    if (namepaginapopup.val() == "RADICACION_SALIENTE") {
        //asigna_expdiente_radicacion_entrante();
        //return false;
    }

   }
    catch (err) {
        alert(err.message + " Funcion importa_dato_expediente");
  }
}
function asigna_expediente_workflow_indice() {
    try {
    var modal_popup = $('#Buttoncerrar_expdiente_popup', window.parent.document);
    var hiden_expediente = $('#Hidden_id_expediente', window.parent.document);
    var text_box_expediente = $('#EXPEDIENTE', window.parent.document);
    if (hiden_expediente == undefined) {
        alert("Imposible encontrar el control Hidden_id_expediente en el indice workflow");
        return false;
    }

    if (text_box_expediente == undefined) {
        alert("Imposible encontrar el control EXPEDIENTE en el indice workflow");
        return false;
    }

    if ($('#hdnEmailID').val() == "0" || $('#hdnEmailID').val() == "-1") {
        alert("Debe seleccionar un expediente");
        return false;
    }
    var sele_row = $('#data_grid tr[id=' + $('#hdnEmailID').val() + ']');
    var columindex = colum_index("ESTADO_EXPEDIENTE");
    if (columindex == -1) {
        alert("Imposible encontrar el index de la columna ESTADO_EXPEDIENTE");
        return false;
    }

    if (sele_row[0].cells[columindex].innerText != "1") {
        alert("El expediente esta cerrado, no se puede asignar");
        return false;
    }
    hiden_expediente.val($('#hdnEmailID').val());
    text_box_expediente.val(sele_row[0].cells[1].innerText);
    modal_popup.click();
}
    catch (err) {
        alert(err.message + " Funcion asigna_expediente_workflow_indice");
}
}

function asigna_expdiente_radicacion_entrante() {
    try {
    var modal_popup = $('#Button_cierra_popup_expediente', window.parent.document);
    var textbox_expediente = $('#Textbox_expediente_val_radicacion', window.parent.document);
    var hiden_id_expediente = $('#Hiddenid_expediente', window.parent.document);
    var Check_anexo_radicado = $('#Check_anexo_radicado', window.parent.document)
    var CheckBox_relacionado_radicado = $('#CheckBox_relacionado_radicado', window.parent.document)
    var check_nuevo_radicado = $('#check_nuevo_radicado', window.parent.document)
    if (hiden_id_expediente == undefined) {
        alert("Imposible encontrar el hiden del del expediente en la pagina radicación entrante");
        return false;
    }
    if (Check_anexo_radicado == undefined) {
        alert("Imposible encontrar Check_anexo_radicado en la pagina radicación entrante");
        return false;
    }
    if (textbox_expediente == undefined) {
        alert("Imposible encontrar el textbox del expediente en la pagina radicación entrante");
        return false;
    }
    if (modal_popup != undefined) {
        if ($('#hdnEmailID').val() == "0" || $('#hdnEmailID').val() == "-1" || $('#hdnEmailID').val() == "") {
            alert("Debe seleccionar un expediente");
            return false;
        }
        var h = $('#hdnEmailID').val();
        var sele_row = $('#data_grid tr[id=' + $('#hdnEmailID').val() + ']');
        var columindex = colum_index("ESTADO_EXPEDIENTE");
        if (columindex == -1) {
            alert("Imposible encontrar el index de la columna ESTADO_EXPEDIENTE");
            return false;
        }
       
        if (sele_row[0].cells[columindex].innerText !="1") {
            alert("El expediente esta cerrado, no se puede asignar" );
            return false;
        }
        hiden_id_expediente.val($('#hdnEmailID').val());
        textbox_expediente.val(sele_row[0].cells[2].innerText);
        modal_popup.click();

    }

}
    catch (err) {
        alert(err.message + " Funcion asigna_expdiente_radicacion_entrante");
}
}
//Retorna el idex de una columna en una tabla
function colum_index(colum_name) {
   try {
    var x = $('#data_grid th');
    var txt = "";
    var i;
    for (i = 0; i < x.length; i++) {
        if (x[i].innerText == colum_name) {

            return i;
        }
        
    }
    return -1;
}
    catch (err) {
        alert(err.message + " Funcion colum_index");
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
//Asigna tamaño ventana agregar expediente
function tamano_ventana_agregar_expediente() {
    try {
   
    $("#Hiddenheigpagina").val(($("#contendor_principal").height()));
    var label = $('#Label_agregar_expdiente_popup');
    label[0].innerText = "Agregar unidad";
   // $("#Iframe_agregar_expdiente_popup_").attr("src", "../gestion/FormGaAgregarExpediente.aspx")
}
    catch (err) {
        alert(err.message + " Funcion tamano_ventana_agregar_expediente");
}
}
//Asigna tamaño ventana editar expediente
function tamano_ventana_editar_expediente() {
    try {
    $("#Hiddenheigpagina").val(($("#contendor_principal").height()));
    var label = $('#Label_agregar_expdiente_popup');
    label[0].innerText="Editar unidad documental";
    // $("#Iframe_agregar_expdiente_popup_").attr("src", "../gestion/FormGaAgregarExpediente.aspx")
}
    catch (err) {
        alert(err.message + " Funcion tamano_ventana_editar_expediente");
}
}
//Asigna tamaño ventana nuevo volumen expediente
function tamano_ventana_nuevo_volumen_expediente() {
    try {
    $("#Hiddenheigpagina").val(($("#contendor_principal").height()));
    var label = $('#Label_agregar_expdiente_popup');
    label[0].innerText = "Nuevo volumen unidad documental";
    // $("#Iframe_agregar_expdiente_popup_").attr("src", "../gestion/FormGaAgregarExpediente.aspx")
}
    catch (err) {
        alert(err.message + " Funcion tamano_ventana_nuevo_volumen_expediente");
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
function auto_zise(){
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


     hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val();
     if (hidenpadre != undefined) {
        var tempo = $('#Hiddenheigpaginapopup', window.parent.document).val();
        if (tempo != "0") {
            //espacio_iframe = (parseInt(tempo) - 1);
        } 
     } 

if (window.parent.document.getElementById("Divcerrarbuton_expdiente_popup_idice_visor")) {
    espacio_iframe = (window.parent.document.getElementById("Panel_expdiente_popup").clientHeight - window.parent.document.getElementById("Divcerrarbuton_expdiente_popup_idice_visor").clientHeight) - 40;
}
if (window.parent.document.getElementById("divcabecer_expdiente_popup") && window.parent.document.getElementById("Iframe_expdiente_popup_")) {
    espacio_iframe = (window.parent.document.getElementById("Iframe_expdiente_popup_").clientHeight - window.parent.document.getElementById("divcabecer_expdiente_popup").clientHeight + 10);
}
espacio_iframe = espacio_iframe - document.getElementById("menu_var").clientHeight;
$("body").css("height", (espacio_iframe ) + "px");
$("#contendor_principal").css("height", (espacio_iframe ) + "px");
$("#Contentizquierdo").css("height", (espacio_iframe ) - ($("#menu_var").height()) + "px");
$("#Contenedorderecho").css("height", (espacio_iframe ) - ($("#menu_var").height()) + "px");
$("#Panelcampos").css("height", ($("#Contentizquierdo").height()) - ($("#Panelbuton").height() + $("#DropDownListEntidadEmpresa").height() + 10) + "px");
//$("#item_tab_content").css("height", ($("#Contenedorderecho").height()) - ($("#item_nav_tab").height() + 10) + "px");
//$("#home_expediente").css("height", ($("#item_tab_content").height() - 5) + "px");
//$("#Contenedorgrid").css("height", ($("#item_tab_content").height() - 10) + "px");
$("#Panelactividad").css("height", ($("#Contenedorderecho").height() - 70) + "px");
$("#contenido_botonoes").css("height", $("#Button_nuevo_expediente_gestion").height() + 10 + "px");
$("#Panelactividad_documentos_exp").css("height", ($("#Contenedorderecho").height() - (100 + $("#contenido_titulo_val_radicacion_documentos_exp").height())) + "px");
var heig = (espacio_iframe) - $("#contenido_titulo_val_radicacion").height() + $("#contenido_botonoes").height() + $("#div_contenedor_titulo_documentos_relacionados_exp").height() + $("#TextBox_busqueda_documento").height() + $("#UpdatePanel_expediente_seleccionado_exp").height();
$("#contendor_principal").css("width", (with_frame - 5) + "px");
$("#Contenedorderecho").css("width", (with_frame - 5) + "px");
$("#formGaGestionExpediente").css("width", (with_frame - 5) + "px");
$("#Panelcampos").css("width", (with_frame - 5) + "px");
$("#Panelactividad").css("width", (with_frame - 5) + "px");
$("#Panelbuton").css("width", (with_frame - 5) + "px");
}
    catch (err) {
        alert(err.message + " Funcion auto_zise ");
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

        $('#Panel_visor_externo').css("height", (espacio_iframe - 10) + "px");
        $('#Cotenedorpendiente_visor_externo').css("height", ((espacio_iframe - 10) - $("#Cabecerapendiente_visor_externo").height()) + "px");
        $('#Iframe_visor_externo_clasficacion_').css("height", ((espacio_iframe - 10) - $("#Cabecerapendiente_visor_externo").height()) + "px");
    }
    catch (err) {
        alert(err.message + " Funcion actualiza_gre_campos_dinamicos");
    }
}
function auto_zise_agregar_expediente() {
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

        $('#Panel_agregar_expdiente_popup_trabajo').css("height", (espacio_iframe - 5) + "px");
        $('#Contenido_agregar_expdiente_popup_trabajo').css("height", (espacio_iframe - 5) + "px");
        $('#Iframe_agregar_expdiente_popup_trabajo_').css("height", (espacio_iframe - 5) + "px");
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_agregar_expediente");
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
function auto_zise_popup_add_edit_expediente() {
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
        var heig_porcent = espacio_iframe - ((espacio_iframe * 5) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_add_edit_expediente').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_add_edit_expediente').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#contenido_procesa_add_edit_expediente').css("height", (document.getElementById("modal_content_add_edit_expediente").clientHeight - (document.getElementById("diver_cabcera_add_edit_expediente").clientHeight + document.getElementById("content_boton_add_edit_expediente").clientHeight)) + "px");
        //Para los modal que contiene gred
        //$('#content_data_grid').css("height", (document.getElementById("contenido_procesa_add_edit_expediente").clientHeight ) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_add_edit_expediente " + err.message);
    }
}
function eliminar_fila_data_gred(gred) {
    try {
        
        $("#" + gred + " tr[id=" + $("#hdnEmailID").val() + "]").remove();
        $('#hdnEmailID').val("-1");
        var cant_record = parseInt(document.getElementById("Hidden_0001").value) - 1;
        document.getElementById("titulo_label_expedientes").innerHTML = (cant_record) + " registro(s) de expediente (s)";
        document.getElementById("Hidden_0001").value = cant_record;
    }
    catch (err) {
        alert(err.message + " Funcion eliminar_fila_data_gred");
    }

}
function onDataShown(sender, args) {
    sender._popupBehavior._element.style.zIndex = 1000001;
    //sender._popupBehavior._element.style.left = "54px";//set positions according to your requriment.
    //sender._popupBehavior._element.style.top = "50px";//set top postion accorind to you requirement.

    //you can either use left,top or right,bottom or any combination u want to set ur divlist.            
}
function progres_hiden(progres) {
    $("#progres_bar").css("display", "none");
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
function create_element_popup(elemento_posicion, element_padre_everflow) {
    try {

        var document_posicion = document.getElementById(elemento_posicion);
        var documento = document.getElementById("myModal");
        $('#myModal').css("width", "400px");
        $('#myModal').css("height", "150px");
        $('#mytexto_').css("width", "395px");
        $('#mytexto_').css("height", "145px");
        //document.getElementById("tex_modal").innerHTML = texto_popup;
        var pagy = 1;
        if (document.getElementById(element_padre_everflow).scrollTop) {
            pagy = document.getElementById(element_padre_everflow).scrollTop;
        }
        if (document_posicion.offsetParent) {

            if (document_posicion.offsetParent.offsetTop > document_posicion.offsetTop) {

                documento.style.top = (document_posicion.offsetParent.offsetTop - pagy) + 45 + "px";
            } else {

                documento.style.top = (document_posicion.offsetTop - pagy) + 45 + "px";
            }

        } else {
            documento.style.top = (document_posicion.offsetTop - pagy) + 45 + "px";
        }
        documento.style.left = document_posicion.offsetLeft + document_posicion.offsetWidth + "px";
        documento.style.display = "block";
        $('#myModal').show();
    }
    catch (err) {
        alert(err.message + " Función create_element_popup");
    }
}

function hide_autonomo() {
    document.getElementById("myModal").style.display = "none";
}
//3. La funcion a continuacion es la encargda de asignar el texto de ayuda de la función 
function ayuda_compartir(nombre_boton_ayuda, element_padre_everflow) {
    try {
        web_service_solicitudes_ayuda(nombre_boton_ayuda, 'tex_modal');
        create_element_popup(nombre_boton_ayuda, element_padre_everflow);
    }
    catch (err) {
        alert(err.message + " Funcion web_service_solicitudes_ayuda");
    }
   
}
function web_service_solicitudes_ayuda(datas, hidemodal) {
    try {
        var obj = { "nombre_ayuda": datas, "nombre_modulo": "modulo" };
        var jsonData = JSON.stringify(obj);
        $.ajax({
            url: '../radicador/' + 'HandlerListaAyuda.ashx',
            type: 'POST',
            data: jsonData,
            success: function (data) {
                //alert(data);
                //web_service_solicitudes_ayuda = data;
                document.getElementById(hidemodal).innerHTML = data;
                //return web_service_solicitudes_ayuda;
            },
            error: function (errorText) {
                //alert("Error general funcion axion_script !" + errorText);
            }
        });
    }
    catch (err) {
        alert(err.message + " Funcion web_service_solicitudes_ayuda");
    }
}



