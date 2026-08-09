$(document).ready(function () {
    $.fn.inicio = function () {
        auto_zise_registro();
        auto_zise_popup_panel_mensaje_contactos();
        auto_zise_popup_registro_usuario();
        auto_zise_popup_adjunta_anexo_respuesta();
    }
});
$(window).on("load", function () {
    try {
        var elment = document.getElementsByClassName("da_event_captive");
        if (elment) {
            for (var i = 0; i < elment.length; i++) {
                elment[i].addEventListener("click", event_click, false);
            }
        }
        window.addEventListener("resize", rezize_event);
        ShowModalPopup("ModalPopupExtender_mensaje_personalizado_backgroundElement", "Panel_mensaje_personalizado", 100001);
        ShowModalPopup("ModalPopupExtender_edition_sube_anexo_respuesta_backgroundElement", "Panel_sube_anexo_respuesta", 100002);
        
    } catch (e) {
        alert(" funcion load " + e.message);
    }

});

function activa_retroceso_principal() {
    try {
        if (window.parent.document.getElementById("element_a_inicio")) {
            window.parent.document.getElementById("element_a_inicio").click();
        }

    }
    catch (err) {
        alert(err.message + " Funcion activa_retroceso_pagina");
    }
}
function rezize_event() {
    try {
        auto_zise_registro();
        auto_zise_popup_panel_mensaje_contactos();
        auto_zise_popup_registro_usuario();
        auto_zise_popup_adjunta_anexo_respuesta();
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

var ESTADO_EVENT_GENERAL = "";
var INTERVAL_EVENT_GENERAL;
var ESTADO_VALIDA_SOLICITANTE = 1;
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
var FILE_ARCHIVO_DONWLOAD;
function inicializa_tipo_adjunto_documento(event, element, value_sel) {
    try {

        //Adjunta documento anexo documento respuesta
        if (value_sel == "S-D-A") {
            event_element_menu("S-D-A", "adjunto_anexo_respuesta");
        }
        if (value_sel == "R-R-P") {
            event_element_menu("R-R-P", "");
        }
    }
    catch (err) {
        alert(err.message + " Funcion inicializa_tipo_adjunto_documento");
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
        var imp_load = document.getElementById('file_element_' + CONTEN_NAME_UPLOAD_FILE);
        //Adjnta anexo radicado
        if (CONTEN_NAME_UPLOAD_FILE == "adjunto_anexo_respuesta") {
            funcion_name = "actualiza_drowp_pqrs";
            evento_adjunta = "GESTION_PQRS";
            element_html_actuliza = "DropDownList_anexos_respuesta";
            tipo_adjunta = -1;
            id_respuesta = 0;
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
function event_element_clic(event, e) {
    try {
        ESTADO_EVENT_GENERAL = "intro";
        posicion_update_pogres('progres_bar');
        e.disabled = true;
        INTERVAL_EVENT_GENERAL = setInterval(fx_funcion, 400);
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
                if (e.id == "registro_actualiza_dext_externo_001") {
                    valida_datos_solcitante();
                    return true;
                }
                if (e.id == "registro_actualiza_dext_externo_003") {
                    valida_contacto_solicitante();
                    return true;
                }
                if (e.id == "registro_actualiza_dext_externo_004") {
                    var confir = confirm("Desea registar su solicitud de PQRSF ?");
                    if (confir == false) {
                        ESTADO_EVENT_GENERAL = "out";
                        return true;
                    }
                    Service_radicacion_pqrsd();
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
                //Sube documento anexo r
                if (evento == "S-D-A") {
                    inicializa_upload_file_client(tip_event);
                    parameter_upload_modal(ESTADO_EVENT_GENERAL, "PQR", "ModalPopupExtender_edition_sube_anexo_respuesta", "", tip_event);
                   
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
//----------ARCHIVO JAVA VALIDA EXTERNO------------////
var ITEMS_DATOS_DROW = [];
var ITEMS_CONTROL_DEST_EXTERNO = [];
function creaal_file() {
    try {
        $("#DropDownList_anexos_respuesta").empty();
    } catch (ex) {
        alert("Funcion  creaal_file " + ex.mensaje);
    }
   
}
function estado_diaplay_boton(evet,e) {
    try {
        if (e.id == "ex3_tab_1_") {
            document.getElementById("registro_actualiza_dext_externo_001").style.display = "block";
            document.getElementById("registro_actualiza_dext_externo_003").style.display = "none";
            document.getElementById("registro_actualiza_dext_externo_004").style.display = "none";
            return true;
        }
        if (e.id == "ex3_tab_2_") {
            document.getElementById("registro_actualiza_dext_externo_001").style.display = "none";
            if (ESTADO_VALIDA_SOLICITANTE == 2 || ESTADO_VALIDA_SOLICITANTE == 3) {
                document.getElementById("registro_actualiza_dext_externo_003").style.display = "block";
            } else {
                document.getElementById("registro_actualiza_dext_externo_003").style.display = "none";
            }
            document.getElementById("registro_actualiza_dext_externo_004").style.display = "none";
            return true;
        }
        if (e.id == "ex3_tab_3_") {
            document.getElementById("registro_actualiza_dext_externo_001").style.display = "none";
            document.getElementById("registro_actualiza_dext_externo_003").style.display = "none";
            if (ESTADO_VALIDA_SOLICITANTE == 3) {
                document.getElementById("registro_actualiza_dext_externo_004").style.display = "block";
            } else {
                document.getElementById("registro_actualiza_dext_externo_004").style.display = "none";
            }
            
            return true;
        }
    } catch (ex) {
        alert("Funcion estado_diaplay_boton dice " + ex.message);
    }
}
function valida_datos_solcitante() {
    try {
        var atrib_campo_aleas = "";
        var atrib_campo_obliga = "";
        var valor_campo = "";
        //--------------------Valida datos del solicitante-----------------------------------       
        if (document.getElementById("ex3_tabs_1").classList.contains("active") === true) {
            var control_element_ = document.getElementsByClassName("rel-campo-solicitante");
            for (var i = 0; i < control_element_.length; i++) {
                atrib_campo_aleas = control_element_[i].attributes["ref_aleas_campo"].value;
                atrib_campo_obliga = control_element_[i].attributes["ref_campo_obligatorio"].value;
                valor_campo = control_element_[i].value;
                if (atrib_campo_obliga == "*" && valor_campo == "") {
                    alert("El campo (" + atrib_campo_aleas + ") es obligatorio");
                    control_element_[i].focus();
                    ESTADO_EVENT_GENERAL = "out";
                    return true;
                }
                var atrib_correo = control_element_[i].attributes["ref_tipo_campo_correo"];
                var nombre_campo_correo = "";
                if (atrib_correo) {
                    nombre_campo_correo = control_element_[i].id;
                    var resultado = "";
                    if (control_element_[i].value !== "") {
                        resultado = validateEmail(nombre_campo_correo);
                        if (resultado !== "") {
                            alert(resultado);
                            control_element_[i].focus();
                            ESTADO_EVENT_GENERAL = "out";
                            return true;
                        }
                    }
                }
            }
            //valida campo correo electrónico

            ITEMS_CONTROL_DEST_EXTERNO = new Array();
            control_element_ = document.getElementsByClassName("rel-campo-solicitante");
            for (var i = 0; i < control_element_.length; i++) {
                 valor_campo = "";
                if (control_element_[i].tagName == "INPUT") {
                    valor_campo = control_element_[i].value;
                }
                if (control_element_[i].tagName == "SELECT") {  
                    valor_campo = control_element_[i].options[control_element_[i].selectedIndex].text;
                   
                }
                ITEMS_CONTROL_DEST_EXTERNO.push({
                    TEXTO_CAMPO: valor_campo.replace("'", ""), Tipo_Campo: "", Nombre_Campo: control_element_[i].id
                });
            } 
            //---------Valida existencia peticionario
            Service_Valida_exitencia_usuario_peticionario();      
            ESTADO_EVENT_GENERAL = "out";
            return true;
        }
       
    } catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        alert("Funcion valida_datos_solcitante " + ex.message);
    }
}
function valida_contacto_solicitante() {
    try {
        var atrib_campo_aleas = "";
        var atrib_campo_obliga = "";
        var valor_campo = "";    
        if (document.getElementById("ex3_tabs_2").classList.contains("active") === true) {
            var control_element_ = document.getElementsByClassName("rel-campo-contacto");
            for (var i = 0; i < control_element_.length; i++) {
                atrib_campo_aleas = control_element_[i].attributes["ref_aleas_campo"].value;
                atrib_campo_obliga = control_element_[i].attributes["ref_campo_obligatorio"].value;
                valor_campo = control_element_[i].value;
                if (atrib_campo_obliga == "*" && valor_campo == "") {
                    alert("El campo (" + atrib_campo_aleas + ") es obligatorio");
                    control_element_[i].focus();
                    ESTADO_EVENT_GENERAL = "out";
                    return true;
                }
                var atrib_correo = control_element_[i].attributes["ref_tipo_campo_correo"];
                var nombre_campo_correo = "";
                if (atrib_correo) {
                    nombre_campo_correo = control_element_[i].id;
                    var resultado = "";
                    resultado = validateEmail(nombre_campo_correo);
                    if (resultado !== "") {
                        alert(resultado);
                        control_element_[i].focus();
                        ESTADO_EVENT_GENERAL = "out";
                        return true;
                    }
                }
            }  
            
        }
        ITEMS_CONTROL_DEST_EXTERNO = new Array();
        control_element_ = document.getElementsByClassName("rel-campo-solicitante");
        for (var i = 0; i < control_element_.length; i++) {
             valor_campo = "";
            if (control_element_[i].tagName == "INPUT") {
                valor_campo = control_element_[i].value;
            }
            if (control_element_[i].tagName == "SELECT") {
                valor_campo = control_element_[i].options[control_element_[i].selectedIndex].text;
            }
            ITEMS_CONTROL_DEST_EXTERNO.push({
                TEXTO_CAMPO: valor_campo.replace("'", ""), Tipo_Campo: "", Nombre_Campo: control_element_[i].id
            });
        } 
        control_element_ = document.getElementsByClassName("rel-campo-contacto");
        for (var i = 0; i < control_element_.length; i++) {
             valor_campo = "";
            if (control_element_[i].tagName == "INPUT") {
                valor_campo = control_element_[i].value;
            }
            if (control_element_[i].tagName == "SELECT") {  
                valor_campo = control_element_[i].options[control_element_[i].selectedIndex].text;
            }
            ITEMS_CONTROL_DEST_EXTERNO.push({
                TEXTO_CAMPO: valor_campo.replace("'", ""), Tipo_Campo: "", Nombre_Campo: control_element_[i].id
            });
        } 
        if (ITEMS_CONTROL_DEST_EXTERNO.length > 0) {
            Service_registra_actualiza_plantilla_usuario_externo();
        } else {
            alert("No se registran valores en los campos para agregar el peticionario");
            ESTADO_EVENT_GENERAL = "out";
            return true;
        }
        
       
    } catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        alert("Error funcion evento_registra_peticionario " + ex.message)
    }
}
function inicializa_control_registro() {
    try {
        ESTADO_VALIDA_SOLICITANTE = 1;
    } catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        alert("Funcion inicializa_control_registro ")
    }
}
function event_add_departamento(name_campo) {
    try {
        const newLocal = document.getElementById(name_campo);
        if (document.getElementById(name_campo)) {
            var value_drow = $("#" + name_campo).val();
            $("#departemento").empty();
            $("#municipio").empty();
            service_source_list_item(value_drow, "service_solicita_lista_departamentos", "departemento");
        }   
    } catch (ex) {
        alert("Error funcion event_add_departamento " + ex.message);
    }
}


function Service_Valida_exitencia_usuario_peticionario() {
    try {
        var serialice = JSON.stringify(ITEMS_CONTROL_DEST_EXTERNO);
        $.ajax('../webservice/WebServiceRadicacion.asmx/Service_Valida_exitencia_usuario_peticionario', {
            data: "{" + "'parameter':'" + serialice + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].Error_valida !== "YES") {
                    ESTADO_EVENT_GENERAL = "out";
                    if (data.d[0].nombre_campo_error !== "") {
                        var r = confirm(data.d[0].Error_valida);
                        if (r == true) {
                            var control_campo = document.getElementById(data.d[0].nombre_campo_error);
                            if (control_campo) {
                                control_campo.focus();
                                if (control_campo.tagName == "INPUT") {
                                    control_campo.value = data.d[0].valor_campo_error;
                                }
                                if (control_campo.tagName == "SELECT") {
                                    control_campo.text = data.d[0].valor_campo_error;
                                    control_campo.value = data.d[0].valor_campo_error;
                                }
                            }
                        } else {
                            var control_campo = document.getElementById(data.d[0].nombre_campo_error);
                            if (control_campo) {
                                control_campo.focus();
                            }
                        }

                    } else {
                        alert(data.d[0].Error_valida);
                    }
                   
                } else {
                    ESTADO_VALIDA_SOLICITANTE = 2;
                    document.getElementById("registro_actualiza_dext_externo_001").style.display = "none";
                    document.getElementById("registro_actualiza_dext_externo_003").style.display = "block";
                    document.getElementById("registro_actualiza_dext_externo_004").style.display = "none";
                    document.getElementById("ex3_tabs_1").classList.remove("active");
                    document.getElementById("ex3_tab_1_").classList.remove("active");
                    document.getElementById("ex3_tabs_2").classList.toggle("active");
                    document.getElementById("ex3_tab_2_").classList.toggle("active");
                    ESTADO_EVENT_GENERAL = "out";
                }
            },
            error: function (result) {
                alert("Estatus : " + result.status + " Error " + result.responseJSON.Message + "  ")
                ESTADO_EVENT_GENERAL = "out";
            }, compelete: function () {

            }
        });
    } catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        alert(ex.message + " funcion Service_Valida_exitencia_usuario_peticionario");
    }
}
function Service_registra_actualiza_plantilla_usuario_externo() {
    try {      
        var serialice = JSON.stringify(ITEMS_CONTROL_DEST_EXTERNO);
        $.ajax('../webservice/WebServiceRadicacion.asmx/Service_registra_actualiza_plantilla_usuario_externo', {
            data: "{" + "'parameter':'" + serialice + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d !== "YES") {
                    alert(data.d);
                    ESTADO_EVENT_GENERAL = "out";
                } else {
                    document.getElementById("registro_actualiza_dext_externo_001").style.display = "none";
                    document.getElementById("registro_actualiza_dext_externo_003").style.display = "none";
                    document.getElementById("registro_actualiza_dext_externo_004").style.display = "block";
                    document.getElementById("ex3_tabs_1").classList.remove("active");
                    document.getElementById("ex3_tab_1_").classList.remove("active");
                    document.getElementById("ex3_tabs_2").classList.remove("active");
                    document.getElementById("ex3_tab_2_").classList.remove("active");
                    document.getElementById("ex3_tabs_3").classList.toggle("active");
                    document.getElementById("ex3_tab_3_").classList.toggle("active");
                    ESTADO_VALIDA_SOLICITANTE = 3;
                    ESTADO_EVENT_GENERAL = "out";
                }
            },
            error: function (result) {
                alert("Estatus : " + result.status + " Error " + result.responseJSON.Message + "  ")
                ESTADO_EVENT_GENERAL = "out";
            }, compelete: function () {

            }
        });
    } catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        alert(ex.message + " funcion Service_registra_actualiza_plantilla_usuario_externo");
    }
}
function Service_radicacion_pqrsd() {
    try {
        var tramite_ = document.getElementById("DropDownList_tipo_tramite").value;
        if (tramite_ == "" || tramite_ == "SELECCIONE") {
            alert("Debe seleccionar el tipo de solicitud");
            ESTADO_EVENT_GENERAL = "out";
            document.getElementById("DropDownList_tipo_tramite").focus();
            return true;
        }
        var area_ = document.getElementById("DropDownList_area_dependencia").value;
        if (area_ == "") {
            alert("Debe seleccionar el área o dependencia correspondiente a la solicitud");
            ESTADO_EVENT_GENERAL = "out";
            document.getElementById("DropDownList_area_dependencia").focus();
            return true;
        }
        var asunto_ = document.getElementById("TextBox_asunto").value;
        if (asunto_ == "") {
            alert("Debe digitar el asunto de la solicitud");
            ESTADO_EVENT_GENERAL = "out";
            document.getElementById("TextBox_asunto").focus();
            return true;
        }
        var descripcion_ = document.getElementById("TextBox_descripcion").value;
        if (descripcion_ == "") {
            alert("Debe digitar la descripción de la solicitud");
            ESTADO_EVENT_GENERAL = "out";
            document.getElementById("TextBox_descripcion").focus();
            return true;
        }
        var documento_anexo_ = document.getElementById("DropDownList_anexos_respuesta").value;
        var control_element_ = document.getElementsByClassName("rel-campo-contacto");
        var valor_campo_correo = "";
        var atrib_correo;
        for (var i = 0; i < control_element_.length; i++) {
            atrib_correo = control_element_[i].attributes["ref_tipo_campo_correo"];
            if (atrib_correo) {
                valor_campo_correo = control_element_[i].value;
            }

        }
        var control_element_ = document.getElementsByClassName("rel-campo-solicitante");
        for (var i = 0; i < control_element_.length; i++) {
            atrib_correo = control_element_[i].attributes["ref_tipo_campo_correo"];
            if (atrib_correo) {
                valor_campo_correo = control_element_[i].value;
            }
        }   
        if (valor_campo_correo == "") {
            alert("Correo electrónico sin identifcar");
            ESTADO_EVENT_GENERAL = "out";
            return true;
        } 
        $.ajax('../webservice/WebServiceRadicacion.asmx/Service_radicacion_pqrsd', {
            data: "{" + "'tramite':'" + tramite_ + "','" + "documento_anexo':'" + documento_anexo_ + "','" + "area':'" + area_ + "','" + "asunto':'" + asunto_ + "','" + "descripcion':'" + descripcion_ + "','" + "correo_copia" + "':'" + valor_campo_correo + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_sistema !== "YES") {
                    alert(data.d[0].error_sistema);
                    ESTADO_EVENT_GENERAL = "out";
                } else {
                    document.getElementById("TextBox_asunto").value = "";
                    document.getElementById("TextBox_descripcion").value = "";
                    $("#DropDownList_anexos_respuesta").empty();   
                    if (data.d[0].url_documento !== "") {
                        dowload_file(data.d[0].url_documento, data.d[0].radicado_documento);
                    }
                    ESTADO_EVENT_GENERAL = "out";
                    alert("Su solicitud se radico bajo el consecutivo : " + data.d[0].radicado_documento);    
                }
            },
            error: function (result) {
                alert("Estatus : " + result.status + " Error ");
                ESTADO_EVENT_GENERAL = "out";
            }, compelete: function () {

            }
        });
    } catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        alert(ex.message + " funcion Service_radicacion_pqrsd");
    }
}
//---------TERMINA FORMULARIO
function auto_zise_registro() {
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
        var sum_heigh =  document.getElementById("info_reg_solict_pqrs").clientHeight + document.getElementById("_Panelvalidacion").clientHeight + document.getElementById("contenido_controles_buton_registro").clientHeight;
        //$('#Panel_registro_usuario').css("heigh", sum_heigh + "px");
       
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_registro " + err.message);
    }
}
function auto_zise_popup_panel_mensaje_contactos() {
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
        $('#Panel_mensaje_contactos').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_Panel_mensaje_contactos').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#Cotenedor_mensaje_contactos').css("height", (document.getElementById("modal_content_Panel_mensaje_contactos").clientHeight - (document.getElementById("div_title_mensaje_contactos").clientHeight + 1)) + "px");
        //Para los modal que contiene gred
        $('#div_mesaje_panel_mensaje_contactos').css("height", (document.getElementById("Cotenedor_mensaje_contactos").clientHeight - document.getElementById("div_label_panel_mensaje_contactos").clientHeight) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_panel_mensaje_contactos " + err.message);
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
function resize_form_principal() {
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
        document.getElementById("Hidden_height").value = espacio_iframe - 20;
        document.getElementById("Hidden_width").value = with_frame - 20;

    }
    catch (err) {
        alert(err.message + " funcion asigna_datos_heig_with " + err.message);
    }
}
function auto_zise_popup_registro_usuario() {
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
        $('#Panel_registro_usuario').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_Panel_registro_usuario').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#Cotenedor_registro_usuario').css("height", (document.getElementById("modal_content_Panel_registro_usuario").clientHeight - (document.getElementById("divcabecer2_radica_documento").clientHeight + document.getElementById("contenido_controles_buton_registro").clientHeight)) + "px");
        //Para los modal que contiene gred
        $('#_Panelvalidacion').css("height", ((document.getElementById("Cotenedor_registro_usuario").clientHeight) - (document.getElementById("info_reg_solict_pqrs").clientHeight + document.getElementById("title_reg_solict_pqrs").clientHeight + 30) ) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_usuarios_relacionados " + err.message);
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