/// <reference path="../../webservice/WebServiceRadicacion.asmx" />
$(document).ready(function () {
   
    $.fn.inicio = function () {
        auto_zise_popup_visor_externo();
        auto_zise_colaboracion();
    } 
    $('.onclik_item').click(function () {
        var t = $(this)[0].id.split("_");
        document.getElementById("Hidden_value_documento").value = t;
        document.getElementById("Button_activa_visor_documento").click();
    });
    $('.onclik_item').mouseover(function () {

        $(this).css({ cursor: "hand", cursor: "pointer" });
    });
    $('.close').mouseover(function () {

        $(this).css({ cursor: "hand", cursor: "pointer" });
    });
    
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
        ShowModalPopup("ModalPopupExtender_mensaje_personalizado_backgroundElement", "Panel_mensaje_personalizado", 100001);
        ShowModalPopup("ModalPopupExtender_sube_documento_adjunto_backgroundElement", "Panel_sube_documento_adjunto", 100001);
        inicia_tokenize("tokenize-callable-demo1");
        $('.tokenize-callable-demo1').on('tokenize:tokens:added', function (e, value, text) {
            ITEMS_DATOS_TOKENIZE_2.push({ text: text, value: value });
        });
        $('.tokenize-callable-demo1').on('tokenize:tokens:remove', function (e, value) {
            delete_array_tokenize(value);
        });
       
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
        auto_zise_colaboracion()
        auto_zise_popup_visor_externo();
        
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
var ITEMS_DATOS_TOKENIZE_2 = new Array();  //GUARDA LOS ITEM SELECIONANDO EN TEX SELECTOR
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
function asig_array_tokenize() {
    try {
        document.getElementsByName("Hidden_text_user").value = "";
        for (var i = 0; i < ITEMS_DATOS_TOKENIZE_2.length; i++) {
            if (i == 0) {
                document.getElementsByName("Hidden_text_user").value = '|' + ITEMS_DATOS_TOKENIZE_2[i].value + '|| ' + ITEMS_DATOS_TOKENIZE_2[i].text + ',';
            } else {
                document.getElementsByName("Hidden_text_user").value = document.getElementsByName("Hidden_text_user").value + '|' + ITEMS_DATOS_TOKENIZE_2[i].value + '|| ' + ITEMS_DATOS_TOKENIZE_2[i].text + ',';
            }
        }
} catch (err) {
    alert(err.message + " Funcion asig_array_tokenize");
}
}
function inicia_tokenize(name_tokenize) {
    try {
        $('.' + name_tokenize).tokenize2({
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

        })
    } catch (ex) { alert("Funcion inicia_tokenize " + ex.message); }
}
function Compartir_documentos_tokenize() {
    try {
        if (ITEMS_DATOS_TOKENIZE_2.length == 0) {
            alert("Debe selecionar los usuarios a compartir");
            return false;
        }
        var value_file;
        var i = 0;
        $('.onclik_item_chek').each(function () {
            var d = this;
            if (this.checked === true) {
                if (i == 0) {
                    value_file = this.id;
                    i = i + 1;
                } else {
                    value_file = value_file + "|" + this.id;
                    i = i + 1;
                }
            }
        });
        
        var valParam = JSON.stringify(ITEMS_DATOS_TOKENIZE_2);
        var para_meter_ca = new Array();
        var asunto_ = document.getElementsByName('TextBox_asunto_documento');
        var nota_ = document.getElementsByName('TextBox_nota_documento');
        var nivel_urgencia_solicitud_ = document.getElementsByName('DropDownList_prioridad_solicitud');
        var tipo_solicitud_ = document.getElementsByName('DropDownList_tipo_documento_compartir');
        var fecha_limite_ = document.getElementsByName("TextBox_fecha_limite_solicitud");
        var radicado_relacionado_ = "";
        var id_usuario_propietario_= 0;
        var matri_documentos_ = value_file;
        para_meter_ca.push({ asunto_: asunto_[0].value, nota_: nota_[0].value, nivel_urgencia_solicitud_: nivel_urgencia_solicitud_[0].value, tipo_solicitud_: tipo_solicitud_[0].value, radicado_relacionado_: radicado_relacionado_, id_usuario_propietario_: id_usuario_propietario_, matri_documentos_: matri_documentos_, fecha_limite_: fecha_limite_[0].value });
        var serialice = JSON.stringify(para_meter_ca);
        $.ajax('../webservice/WebServiceWorkflow.asmx/Set_compartir_documentos', {
            data: "{'item_user':'"  + valParam + "'," + "'parameter':'"  + serialice + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d !== "YES") {
                    alert(data.d);
                } else { cerrar_ventana('Button_cerrar_autoriza_compartir_documento'); }
            }
        });
    } catch (ex) { alert(ex.message + " funcion Compartir_documentos_tokenize"); }
}
function selcion_item() {
    try {
        var i = 0;
    $('.onclik_item_chek').each(function () {
        var d = this;
        
        if (this.checked === true) {
            if (i == 0) {
                document.getElementById("Hidden_iten_ckek").value = this.id;
                i = i + 1;
            } else {
                document.getElementById("Hidden_iten_ckek").value = document.getElementById("Hidden_iten_ckek").value + "|" + this.id;
                i = i + 1;
            }
            
        }
    })
    }
    catch (err) {
        alert(err.message + " Funcion selcion_item");
    }
}
function cerrar_ventana(nombre_buton) {
    var buton = parent.document.getElementById(nombre_buton)
    if (buton !== 'undefined') {
        buton.click();
    }
}

function service_usuarios_gestion() {
    function split(val) {
        return val.split(/,\s*/);
    }
    function extractLast(term) {
        return split(term).pop();
    }
    $("#TextBox_user_seleccionado")
        .on("keydown", function (event) {
            if (event.keyCode === $.ui.keyCode.TAB &&
                $(this).autocomplete("instance").menu.active) {
                event.preventDefault();
            }
        })
        .autocomplete({
        source: function (request, response) {
            var param = { keyword: $('#TextBox_user_seleccionado').val() };
            $.ajax({
                url: "../webservice/WebServiceWorkflow.asmx/GetLista_usuarios_workflow",
                data: "{'DName':'" + document.getElementById('TextBox_user_seleccionado').value + "'}",
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
            this.value = terms.join(", ");
            return false;
        }
       
            ,minLength:3,max:10,scroll:true});
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
        $('#Cotenedorpendiente_visor_externo').css("height", (espacio_iframe - 10) + "px");
        $('#Iframe_visor_externo_clasficacion_').css("height", (espacio_iframe - 15) + "px");
    }
    catch (err) {
        alert(err.message + " Funcion actualiza_gre_campos_dinamicos");
    }
}
function auto_zise_colaboracion() {
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
        var heig_porcent = espacio_iframe - ((espacio_iframe * 30) / 100);
        $('#Panel_colaboracion_documento_compartido').css("height", (heig_porcent - 10) + "px");
        $('#contenido_procesa_colaboracion_documento_compartido').css("height", (document.getElementById("Panel_colaboracion_documento_compartido").clientHeight - (document.getElementById("divcabecer2_colaboracion_documento_compartido").clientHeight + document.getElementById("div_contenedor_confirma_colaboracion").clientHeight)) + "px");
    }
    catch (err) {
        alert(err.message + " Funcion actualiza_gre_campos_dinamicos");
    }
}

function auto_zise_popup_compartir_documento() {
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
    if (window.parent.document.getElementById("Iframe_compartir_documento_")) {
        espacio_iframe = window.parent.document.getElementById("Iframe_compartir_documento_").clientHeight;
    }
    var espace = document.getElementById("botom_option").clientHeight + document.getElementById("div_tipo_documento").clientHeight;
    espace = espace + 40;
    $('#contenido_general').css("height", (espacio_iframe - espace) + "px");
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
function activa_boton_cerrar() {
    try {

        document.getElementById("Button_cerrar_colaboracion_validacion").click();
    }
    catch (err) {
        alert(err.message + " funcion activa_boton_cerrar " + err.message);
    }
}
$(document).on('keydown', function (e) {
    if (e.which == 9) {
        var id_element = this;

        var salidadato;
        if (e.target.className == "date_indice" && e.target.value != "") {
            var dato = e.target.value;


            if (dato == "") {

                return true;
            }


            if (salidadato == "Formato fecha no cumple") {
                alert(salidadato);
                e.preventDefault();
                return false;
            }
            var BisestA;
            var Año_F, Mes_f, Dia_f, tip;
            var numerocaracter = dato.length;
            if (numerocaracter == 10 || numerocaracter == 8) {

            }
            else {
                alert("Formato fecha no cumple");
                e.preventDefault();
                return false;
            }

            if (numerocaracter == 10) {

                Año_F = dato.substring(0, 4);
                Mes_f = dato.substring(0, 7);
                Mes_f = Mes_f.substring(7, 5);
                Dia_f = dato.substring(8, 10);
            }
            else {
                Año_F = dato.substring(0, 4);
                Mes_f = dato.substring(0, 6);
                Mes_f = Mes_f.substring(6, 4);
                Dia_f = dato.substring(6, 8);
            }

            //Verifica el formato del dia
            if (Dia_f > 31 || Dia_f == 0) {

                alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                e.preventDefault();
                return false;
            }

            //verifica el formato del mes
            if (Mes_f > 12 || Mes_f < 1) {
                alert("EM_" + Año_F + "(" + Mes_f + ")" + Dia_f);
                e.preventDefault();
                return false;
            }

            switch (Mes_f) {
                case "01":
                    if (Dia_f > 31) {
                        alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                        e.preventDefault();
                    }
                    break;

                case "02":
                    if (Dia_f % 4 == 0) {

                        BisestA = 29;
                    }
                    else {
                        BisestA = 28;
                    }
                    if (Dia_f > BisestA) {
                        alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                        e.preventDefault();
                    }
                    break;
                case "03":
                    if (Dia_f > 31) {
                        alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                        e.preventDefault();
                    }
                    break;

                case "04":
                    if (Dia_f > 30) {
                        alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                        e.preventDefault();
                    }
                    break;

                case "05":
                    if (Dia_f > 31) {
                        alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                        e.preventDefault();
                    }
                    break;

                case "06":
                    if (Dia_f > 30) {
                        alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                        e.preventDefault();
                    }
                    break;

                case "07":
                    if (Dia_f > 31) {
                        alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                        e.preventDefault();
                    }
                    break;

                case "08":
                    if (Dia_f > 31) {
                        alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                        e.preventDefault();
                    }
                    break;

                case "09":
                    if (Dia_f > 30) {
                        alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                        e.preventDefault();
                    }
                    break;

                case "10":
                    if (Dia_f > 31) {
                        alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                        e.preventDefault();
                    }
                    break;

                case "11":
                    if (Dia_f > 30) {
                        alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                        e.preventDefault();
                    }
                    break;

                case "12":
                    if (Dia_f > 31) {
                        alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                        e.preventDefault();
                    }
                    break;
            }

            if (numerocaracter == 8) {
                salidadato = Año_F + "/" + Mes_f + "/" + Dia_f;
                e.target.value = salidadato;
            }

            if (numerocaracter == 10) {
                salidadato = Año_F + "/" + Mes_f + "/" + Dia_f;
                e.target.value = salidadato;
            }

        }
    }
});
function ayuda_compartir() {
   
    var texto;
    texto = "A continuación los modos de como compartir un documento : <br />  Informativo : Sólo para informar a los usuarios de la existencia del documento. <br /> Para colaboración : Los usuario están obligados a colaborar con un concepto o anexar algún soporte.";
    create_element_popup(texto, "image_ayuda");
}
function create_element_popup(texto_popup,elemento_posicion) {
    try {
        
        var document_posicion = document.getElementById(elemento_posicion);
        var documento = document.getElementById("myModal");
        $('#myModal').css("width", "400px");
        $('#myModal').css("height", "150px");
        $('#mytexto_').css("width", "400px");
        $('#mytexto_').css("height", "150px");
        document.getElementById("tex_modal").innerHTML = texto_popup;
        documento.style.top = document_posicion.offsetTop + "px";
        documento.style.left = document_posicion.offsetLeft + "px";
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