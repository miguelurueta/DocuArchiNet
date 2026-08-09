
$(document).ready(function () {
    $.fn.inicio = function () {
        clired();
        auto_zise_popup_visor_externo();
        auto_zise_gestion_pqrs();
        auto_zise_popup_detalle_trazabilidad();
        auto_zise_popup_detalle_transacciones();
        auto_zise_popup_log_transacciones();
        auto_zise_popup_imagen_respuesta();
        auto_zise_popup_adjunta_anexo_respuesta();
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
        window.addEventListener("resize", rezize_event);
        ShowModalPopup("ModalPopupExtender_mensaje_personalizado_backgroundElement", "Panel_mensaje_personalizado", 100002);
        
    } catch (e) {
        alert(" funcion load " + e.message);
    }

});
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
        auto_zise_popup_visor_externo();
        auto_zise_gestion_pqrs();
        auto_zise_popup_detalle_trazabilidad();
        auto_zise_popup_detalle_transacciones();
        auto_zise_popup_log_transacciones();
        auto_zise_popup_imagen_respuesta();
        auto_zise_popup_adjunta_anexo_respuesta();
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
var ESTADO_EVENT_GENERAL = "";
var INTERVAL_EVENT_GENERAL;
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
                    parameter_upload(ESTADO_EVENT_GENERAL, "PQR", "Button_anexo_cargar", "", tip_event);
                    return true;
                }
                if (evento == "R-R-P") {
                    var confir = confirm("Desea registar su solicitud de PQRSF y recibir su respuesta por correo electrónico o en la dirección de correspondencia?");
                    if (confir == false) {
                        ESTADO_EVENT_GENERAL = "out";
                        return true;
                    }
                    Service_radicacion_pqrsd();
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
        var valor_campo_correo = document.getElementById("TextBox_correo_anonimo").value;
        var resultado = "";
        if (valor_campo_correo !=="") {
            resultado = validateEmail("TextBox_correo_anonimo");
            if (resultado !== "") {
                alert(resultado);
                control_element_[i].focus();
                ESTADO_EVENT_GENERAL = "out";
                return true;
            }
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
function clired() {
    $('#data_grid tr[id]').click(function () {
        var fer = $(this).attr("id");
        $('#hdnEmailID').val(fer);
    });

    $('#data_grid tr[id]').mouseover(function () {
        $(this).css({ cursor: "hand", cursor: "pointer" });
    });
   
}
function prevent(event, element) {
    try {

        var fer = $(element).attr("idd");
        var tip_event = $(element).attr("tip_event");
        if (tip_event == "est_solic") {
            $('#hdnEmailID').val(fer);
            document.getElementById("Button_Trazabilidad").click();
        }
        if (tip_event == "trans_sac_solic") {
            $('#hdnEmailID').val(fer);
            document.getElementById("Button_Log_respuesta").click();
        }
        if (tip_event == "detale_solic") {
            $('#hdnEmailID').val(fer);
            document.getElementById("Button_detalle_radicado").click();
        }
        if (tip_event == "doc_rel_solic") {
            $('#hdnEmailID').val(fer);
            document.getElementById("Button_visor_emergente").click();
        }
        event.preventDefault();
        element.focus();
    }
    catch (err) {
        alert(err.message + " Funcion prevent");
    }
}
function activa_retroceso_pagina() {
    try {
        if (window.parent.document.getElementById("ifrm_ds_")) {
            window.parent.document.getElementById("ifrm_ds_").src ="../Publico/WebFormPqrsPrincipal.aspx";
        }

        }
    catch (err) {
        alert(err.message + " Funcion activa_retroceso_pagina");
        }
}


function active_enter_buton() {
    try {
        $("#TextBox_busqueda").on('keyup', function (e) {
            if (e.keyCode == 13) {
                document.getElementById("ImageButton_buscar").click();
            }
        });
        event.preventDefault();
    }
    catch (err) {
        alert(err.message + " Función active_enter_buton");
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

        //document.getElementById("Button_sube_documento_adjunto_respuesta").click();
    }
    catch (err) {
        alert(err.message + " funcion activa_boton_dowload " + err.message);
    }
}
function showUploadError_(sender, args) {
    try {
        alert(args.get_errorMessage());
    }
    catch (err) {
        alert(err.message + " funcion showUploadError " + err.message);
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
function auto_zise_gestion_pqrs() {
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

}
function AjaxFileUpload_change_text() {
    try {
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
    Sys.Extended.UI.Resources.AjaxFileUpload_SelectFileToUpload = "archivos(s) para cargar";
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
    catch (err) {
    alert(err.message + " funcion activa_boton_dowload_adjunto " + err.message);
}
}
function onDataShown(sender, args) {
    sender._popupBehavior._element.style.zIndex = 1000001;

}
function auto_zise_popup_detalle_trazabilidad() {
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
        /*$('#Panel_trazabilidad').css("height", (espacio_iframe - 40) + "px");
        $('#Cotenedorpendiente_trazabilidad').css("height", (espacio_iframe - 40) + "px");
        $('#Iframe_trazabilidad_').css("height", (espacio_iframe - 40) + "px");*/
        //corrige el posicionamiento horizontal del modalpopuextender, la propiedad width del panel debe estar en auto
        //var widtth_procent_left_rigth = (with_frame - document.getElementById("Panel_agregar_expediente_carpeta").clientHeight) / 2;
        //$('#Panel_agregar_expediente_carpeta').css("left", (Math.round(widtth_procent_left_rigth)) + "px");
        var heig_porcent = espacio_iframe - ((espacio_iframe * 5) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_trazabilidad').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_Panel_trazabilidad').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#Cotenedorpendiente_trazabilidad').css("height", (document.getElementById("modal_content_Panel_trazabilidad").clientHeight - (document.getElementById("Cabecerapendiente_trazabilidad").clientHeight + 1)) + "px");
        //Para los modal que contiene gred
        $('#Iframe_trazabilidad_').css("height", (document.getElementById("Cotenedorpendiente_trazabilidad").clientHeight - 1) + "px");
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_detalle_trazabilidad");
    }
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
        
        //corrige el posicionamiento horizontal del modalpopuextender, la propiedad width del panel debe estar en auto
        //var widtth_procent_left_rigth = (with_frame - document.getElementById("Panel_agregar_expediente_carpeta").clientHeight) / 2;
        //$('#Panel_agregar_expediente_carpeta').css("left", (Math.round(widtth_procent_left_rigth)) + "px");
        var heig_porcent = espacio_iframe - ((espacio_iframe * 5) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_transacciones').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_Panel_transacciones').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#Cotenedorpendiente_transacciones').css("height", (document.getElementById("modal_content_Panel_transacciones").clientHeight - (document.getElementById("Cabecerapendiente_transacciones").clientHeight + 1)) + "px");
        //Para los modal que contiene gred
        $('#Iframe_transacciones_').css("height", (document.getElementById("Cotenedorpendiente_transacciones").clientHeight - 1) + "px");
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_detalle_transacciones");
    }
}

function auto_zise_popup_log_transacciones() {
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
        /*$('#Panel_log_transacciones').css("height", (espacio_iframe - 40) + "px");
        $('#Cotenedorpendiente_log_transacciones').css("height", (espacio_iframe - 40) + "px");
        $('#Iframe_log_transacciones_').css("height", (espacio_iframe - 40) + "px");*/
        //corrige el posicionamiento horizontal del modalpopuextender, la propiedad width del panel debe estar en auto
        //var widtth_procent_left_rigth = (with_frame - document.getElementById("Panel_agregar_expediente_carpeta").clientHeight) / 2;
        //$('#Panel_agregar_expediente_carpeta').css("left", (Math.round(widtth_procent_left_rigth)) + "px");
        var heig_porcent = espacio_iframe - ((espacio_iframe * 5) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_log_transacciones').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_Panel_log_transacciones').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#Cotenedorpendiente_log_transacciones').css("height", (document.getElementById("modal_content_Panel_log_transacciones").clientHeight - (document.getElementById("Cabecerapendiente_log_transacciones").clientHeight + 1)) + "px");
        //Para los modal que contiene gred
        $('#Iframe_log_transacciones_').css("height", (document.getElementById("Cotenedorpendiente_log_transacciones").clientHeight - 1) + "px");
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_log_transacciones");
    }
}
function auto_zise_popup_imagen_respuesta() {
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
        $('#Panel_imagen_respuesta').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_Panel_imagen_respuesta').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#Cotenedorpendiente_imagen_respuesta').css("height", (document.getElementById("modal_content_Panel_imagen_respuesta").clientHeight - (document.getElementById("Cabecerapendiente_imagen_respuesta").clientHeight + 5)) + "px");
        //Para los modal que contiene gred
        $('#Iframe_imagen_respuesta_').css("height", (document.getElementById("Cotenedorpendiente_imagen_respuesta").clientHeight - 1) + "px");
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_imagen_respuesta");
    }
}