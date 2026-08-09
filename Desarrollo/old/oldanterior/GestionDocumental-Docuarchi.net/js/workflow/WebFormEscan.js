
$(document).ready(function () {
    $.fn.inicio = function () {
        Service_solicita_estructura_interface_digitalizacion("-1");
        Auto_zise_marco_principal();
        auto_zise_popup_lista_chequeo(document.getElementById("Hidden_0001").value);     
        $(":checkbox").change(function () {
            SaveScanSettings();
        });
        $(":radio").change(function () {
            SaveScanSettings();
        });       
        $('#data_grid tr[id]').click(function () {
            $('#data_grid tr[id]').css({ "background": "White", "color": "Black" });
            $(this).css({ "background-color": "#E7EDF5", "color": "Black" });
            var fer = $(this).attr("id");
            $('#hdnEmailID').val(fer);
            
        });
        //ASIGNA EL CURSOR DE SELECCION CUANDO PASA EL CURSOR EN EL DATAGREDVIEW DE VALIDACION RADICACION
        $('#data_grid tr[id]').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });
       
    }
    
    
})
$(window).on("load", function () {
    try {
        var loca = window.location.hostname;
        //sitio web unicentro
        if (loca == '34.66.86.137') {
            const otMeta = document.createElement('meta');
            otMeta.httpEquiv = 'origin-trial';
            otMeta.content = 'Ai1hnB2A2OcBgXG5x1HTsjSnY9HJ4qNFl4NkNQIyHm2WsrsVldsbLmGbhKoxaR5RttG0iohmDQie5fT18NXGzQgAAABweyJvcmlnaW4iOiJodHRwOi8vMzQuNjYuODYuMTM3OjgwIiwiZmVhdHVyZSI6IlByaXZhdGVOZXR3b3JrQWNjZXNzTm9uU2VjdXJlQ29udGV4dHNBbGxvd2VkIiwiZXhwaXJ5IjoxNjc1MjA5NTk5fQ==';
            document.head.append(otMeta);
        }
        //sitio web contasoft
        if (loca == '35.237.60.119') {
            const otMeta = document.createElement('meta');
            otMeta.httpEquiv = 'origin-trial';
            otMeta.content = 'Aqtiz3I7pPTPp6+a3Dt8QwhgUCaiqUM0mNbTydcMO7TeuL3W8nCtg3ZPXdXe3j8bi1leuEUodgrZpJB91KqT4gQAAABxeyJvcmlnaW4iOiJodHRwOi8vMzUuMjM3LjYwLjExOTo4MCIsImZlYXR1cmUiOiJQcml2YXRlTmV0d29ya0FjY2Vzc05vblNlY3VyZUNvbnRleHRzQWxsb3dlZCIsImV4cGlyeSI6MTY3NTIwOTU5OX0=';
            document.head.append(otMeta);
        }
        //34.66.86.137
        
        var elment = document.getElementsByClassName("da_event_captive");
        if (elment) {
            for (var i = 0; i < elment.length; i++) {
                elment[i].addEventListener("click", event_click, false);
            }
        }
        window.addEventListener("resize", rezize_event);
        ShowModalPopup("ModalPopupExtender_mensaje_personalizado_backgroundElement", "Panel_mensaje_personalizado", 100001);
        setInterval('SaveScanSettings();', '600');
        
    } catch (e) {
        alert(" funcion load " + e.message);
    }

});
$(window).on("unload", function () {
    try {
        
        //SaveScanSettings();
    } catch (e) {
        //alert(" funcion unload " + e.message);
    }

});
//import { ExpModMig } from '../js/versiondocumento/gestion_version_documento.js';
//CONFIGURACION DE INTERFACE DE DIGITALIZACION
var ITEMS_DATOS_CONFIG_INTERFACE_DIG = new Array();
var ID_RA_CONFIG_DIG = -1;
var OBLIGA_LISTA_CHEQUEO_DIG = 0;
var TIPO_DIGITALIZACION_DIG = 0;
var TIPO_ARCHIVO_DIGITALIZA_DIG = "PDF";
var ACTIVA_OCR_DIG = 0;
var ACTIVA_COMPRESION_DIG = 0;
var RESOLUCION_DIG = "0.28242953648100017";
var TONALIDAD_DIGITALIZACION_DIG = 1;
var TONALIDAD_DIGITALIZACION_BLACK = 1;
var TONALIDAD_DIGITALIZACION_GRAY = 0;
var TONALIDAD_DIGITALIZACION_COLOR = 0;
var EXTENSION_ARCHIVO_DIG = ".PDF";
var ESTADO_FORMATO_PDF = 1;
var ESTADO_FORMATO_PDF_A = 0;
var ESTADO_FORMATO_TIF = 0;

var ID_CONFIG_DIGITALIZACION_DA = -1;
var ERROR_GESTION_DA = "";
var ZOON_VISOR_DA = "40";
var TUMBAIL_VISOR_DA = 0;
var VISTA_CONFIGURACION_ESCANER_DA = 0;
var DUPLEX_CONFIGURACION_DA = 0;
var DESC_PAG_BLANCO_CONFIGURACION_DA = 0;
var DETECT_BORDE_CONFIGURACION_DA = 0;
var CONTROLADOR_PROPIO_CONFIGURACION_DA = 0;
var ADF_CONFIGURACION = 1;
var DESK_CONFIGURACION = 0;
const reemplazarAcentos = function (cadena) {
    var chars = {
        "á": "a", "é": "e", "í": "i", "ó": "o", "ú": "u",
        "à": "a", "è": "e", "ì": "i", "ò": "o", "ù": "u", "ñ": "n",
        "Á": "A", "É": "E", "Í": "I", "Ó": "O", "Ú": "U",
        "À": "A", "È": "E", "Ì": "I", "Ò": "O", "Ù": "U", "Ñ": "N"
    }
    var expr = /[áàéèíìóòúùñ]/ig;
    //var res = cadena.replace(expr, function (e) { return chars[e] });
    var res = cadena;
    res = res.replace(/"/g, "");
    res = res.replace(/'/g, "");
    res = res.replace(/;/g, "");
    res = res.replace("/", "");
    return res;
}
function Service_solicita_estructura_interface_digitalizacion(radicado_) {
    try {
        $.ajax('../webservice/WebService_Config_Digitalizacion.asmx/Service_solicita_estructura_interface_digitalizacion', {
            data: "{'radicado':" + "'" + radicado_ + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_gestion !== "YES") {
                    alert(data.d[0].error_gestion);
                } else {
                    
                    if (data.d[0].Id_Ra_Config !== "-1") {
                        ID_RA_CONFIG_DIG = data.d[0].Id_Ra_Config;
                        OBLIGA_LISTA_CHEQUEO_DIG = data.d[0].Obliga_Lista_Chequeo;
                        TIPO_DIGITALIZACION_DIG = data.d[0].Tipo_Digitalizacion;
                        TIPO_ARCHIVO_DIGITALIZA_DIG = data.d[0].Tipo_Archivo_Digitaliza;
                        ACTIVA_OCR_DIG = data.d[0].Activa_Ocr;
                        ACTIVA_COMPRESION_DIG = data.d[0].Activa_Compresion;
                        RESOLUCION_DIG = data.d[0].Resolucion;
                        TONALIDAD_DIGITALIZACION_DIG = data.d[0].Tonalidad_Digitalizacion;
                        TONALIDAD_DIGITALIZACION_BLACK = data.d[0].Tonalidad_Digitalizacion_black;
                        TONALIDAD_DIGITALIZACION_GRAY = data.d[0].Tonalidad_Digitalizacion_gray;
                        TONALIDAD_DIGITALIZACION_COLOR = data.d[0].Tonalidad_Digitalizacion_color;
                        EXTENSION_ARCHIVO_DIG = data.d[0].Extension_Archivo;
                        ESTADO_FORMATO_PDF = data.d[0].Estado_formato_pdf;
                        ESTADO_FORMATO_PDF_A = data.d[0].Estado_formato_pdf_a;
                        ESTADO_FORMATO_TIF = data.d[0].Estado_formato_tif;   
                        document.getElementById("TIF").disabled = false;
                        document.getElementById("TIF").checked = true;
                        document.getElementById("PDFA").checked = true;
                        document.getElementById("PDFA").disabled = false;
                        if (EXTENSION_ARCHIVO_DIG == ".PDF") {
                            document.getElementById("PDFA").disabled = false;
                            document.getElementById("TIF").disabled = true;
                        }

                    }
                    ID_CONFIG_DIGITALIZACION_DA = data.d[0].id_config_digitalizacion;
                    ERROR_GESTION_DA = data.d[0].error_gestion;
                    ZOON_VISOR_DA = data.d[0].zoon_visor;
                    TUMBAIL_VISOR_DA = data.d[0].tumbail_visor;
                    VISTA_CONFIGURACION_ESCANER_DA = data.d[0].vista_configuracion_escaner;
                    DUPLEX_CONFIGURACION_DA = data.d[0].duplex_configuracion;
                    DESC_PAG_BLANCO_CONFIGURACION_DA = data.d[0].desc_pag_blanco_configuracion;
                    DETECT_BORDE_CONFIGURACION_DA = data.d[0].detect_borde_configuracion;
                    CONTROLADOR_PROPIO_CONFIGURACION_DA = data.d[0].controlador_propio_configuracion;
                    ADF_CONFIGURACION = data.d[0].adf_configuracion;
                    DESK_CONFIGURACION = data.d[0].desk_configuracion;
                    Configura_iterface_digitalizacion();
                    if (ID_CONFIG_DIGITALIZACION_DA !== -1) {
                        config_menu_config_escnaer();       
                    }
                    
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
        ESTADO_EVENT_GENERAL = "out";
        alert('Service_solicita_estructura_interface_digitalizacion  ' + ex.message);
    }
}
function Service_actualiza_estructura_interface_digitalizacion() {
    try {
        var serialice = JSON.stringify(ITEMS_DATOS_CONFIG_INTERFACE_DIG);
        $.ajax('../webservice/WebService_Config_Digitalizacion.asmx/Service_actualiza_estructura_interface_digitalizacion', {
            data: "{'parameter':" + "'" + serialice + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_gestion !== "YES") {
                    //alert(data.d[0].error_gestion);
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
        ESTADO_EVENT_GENERAL = "out";
        alert('Service_actualiza_estructura_interface_digitalizacion  ' + ex.message);
    }
}
function Configura_iterface_digitalizacion() {
    try {
        if (ID_RA_CONFIG_DIG !== -1) {
            //Configura tipo de formato de digitalización
            document.getElementById("PDFA").style.display = "none";
            document.getElementById("PDF").style.display = "none";
            document.getElementById("TIF").style.display = "none";
            document.getElementById("PDFAC").style.display = "none";
            document.getElementById("PDFC").style.display = "none";
            document.getElementById("TIFC").style.display = "none";
            if (ESTADO_FORMATO_TIF == 1) {
                document.getElementById("TIF").checked = true;
                document.getElementById("TIF").style.display = "flex";   
                document.getElementById("TIFC").style.display = "flex";
            }
            if (ESTADO_FORMATO_PDF == 1) {
                document.getElementById("PDF").checked = true;
                document.getElementById("PDF").style.display = "flex";
                document.getElementById("PDFC").style.display = "flex";
            }
            if (ESTADO_FORMATO_PDF_A == 1) {
                document.getElementById("PDFA").checked = true; 
                document.getElementById("PDFA").style.display = "flex";
                document.getElementById("PDFAC").style.display = "flex";
            }
            //Configura tipo de tonalidad del archivo
           
           
            if (TONALIDAD_DIGITALIZACION_COLOR == 1) {
                document.getElementById("RGB").checked = true;
                document.getElementById("RGB").style.display = "flex";
                document.getElementById("RGBL").style.display = "flex";
            } else {
                document.getElementById("RGB").checked = false;
                document.getElementById("RGB").style.display = "none";
                document.getElementById("RGBL").style.display = "none";
            }
     
            if (TONALIDAD_DIGITALIZACION_GRAY == 1) {
                document.getElementById("Gray").checked = true;
                document.getElementById("Gray").style.display = "flex";
                document.getElementById("GrayL").style.display = "flex";
            } else {
                document.getElementById("Gray").checked = false;
                document.getElementById("Gray").style.display = "none";
                document.getElementById("GrayL").style.display = "none";
            }
            if (TONALIDAD_DIGITALIZACION_BLACK == 1) {
                document.getElementById("BW").checked = true;
                document.getElementById("BW").style.display = "flex";
                document.getElementById("BWL").style.display = "flex";
            } else {
                document.getElementById("BW").checked = false;
                document.getElementById("BW").style.display = "none";
                document.getElementById("BWL").style.display = "none";
            }
            $("#Resolution").empty();
            var option = document.createElement("option");
            option.value = RESOLUCION_DIG.toString();
            option.text = RESOLUCION_DIG.toString();
            document.getElementById("Resolution").add(option);

        } else {
            document.getElementById("BW").style.display = "flex";
            document.getElementById("BWL").style.display = "flex";
            document.getElementById("Gray").style.display = "flex";
            document.getElementById("GrayL").style.display = "flex";
            document.getElementById("RGB").style.display = "flex";
            document.getElementById("RGBL").style.display = "flex";
        }
    } catch (ex) {
        alert("funcion Configura_iterface_digitalizacion " + ex.mensaje);
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
function rezize_event() {
    try {
        Auto_zise_marco_principal();
        auto_zise_popup_lista_chequeo(document.getElementById("Hidden_0001").value);
    } catch (ex) {
        alert(ex.message + " Función rezize_event")
    }
}
function Auto_zise_marco_principal() {
    //CODIGO AJUSTA EL ALTO DE LA PAGINA DEBE ESTAR EL FORM 100% ALTURA
    try {
        var espacio_iframe;
        var with_frame = 420;
        if (window.innerHeight) {
            //navegadores basados en mozilla 
            espacio_iframe = window.innerHeight;
            with_frame = window.innerWidth;
        } else {
            if (document.hidden == true) {
                if (document.body.clientHeight != undefined) {
                    //Navegadores basados en IExplorer, es que no tengo innerheight 
                    espacio_iframe = document.body.clientHeight
                    with_frame = document.body.clientWidth;
                } else {
                    //otros navegadores 
                    espacio_iframe = 478
                }
            }
        }
        /*
        if (window.parent.document.getElementById("IframeDitaliza_adjunto_")) {      
            if (window.parent.document.getElementById("Panel_digitaliza_documento_adjunto").style.display !== "none") {
                espacio_iframe = window.parent.document.getElementById("IframeDitaliza_adjunto_").clientHeight - 20;
            }
        }
       
        //IframeDitaliza_remplaza
        if (window.parent.document.getElementById("IframeDitaliza_")) {
            if (window.parent.document.getElementById("PanelPopupExt")) {
                if (window.parent.document.getElementById("PanelPopupExt").style.display !== "none") {
                    espacio_iframe = window.parent.document.getElementById("IframeDitaliza_").clientHeight + 20;
                   
                } 
            }
            if (window.parent.document.getElementById("Panel_admon_documentos")) {
                if (window.parent.document.getElementById("Panel_admon_documentos").style.display !== "none") {
                    espacio_iframe = window.parent.document.getElementById("IframeDitaliza_").clientHeight + 20;
                    
                }
            }
            if (window.parent.document.getElementById("tab_content")) {
                if (window.parent.document.getElementById("tab_content").style.display !== "none") {
                    espacio_iframe = window.parent.document.getElementById("tab_content").clientHeight;

                }
            }
        }
        //IframeDitaliza_remplaza
        if (window.parent.document.getElementById("IframeDitaliza_remplaza")) {
            //modal_digitaliza_remplaza_documento
            if (window.parent.document.getElementById("modal_digitaliza_remplaza_documento").style.display !== "none") {
                espacio_iframe = (window.parent.document.getElementById("modal_digitaliza_remplaza_documento").clientHeight - 30);
            }
        }*/
        $("#conte_visor").css("height", (espacio_iframe - 5) - ($("#controles").height()) + "px");
        $("#rigt").css("height", $("#conte_visor").height() + "px");
        Dynamsoft.DWT.Containers[0].Width = "100%";
        Dynamsoft.DWT.Containers[0].Height = "100%";
        //let DvsViwer = document.getElementsByClassName("dvs-WebViewer");
        //if (DvsViwer(0).clientHeight == 0) {
        //    DvsViwer(0).clientHeight = $("#conte_visor").height() - 5
        //    DvsViwer(0).clientHeight = $("#conte_visor").height() - 5
        //}
        //Dynamsoft.DWT.Containers[0].Width = "100%";
        //Dynamsoft.DWT.Containers[0].Height = ($("#conte_visor").height() - 5);      
    }
    catch (ex) {
        alert("Inconsistencia función Auto_zise_marco_principal " + ex.message)
    }
    
}
function config_menu_config_escnaer() {
    try {
        if (VISTA_CONFIGURACION_ESCANER_DA == 1) {
            $("#indice_title").toggleClass("fad");
            $("#indice_title").toggleClass("fa-bars");
            document.getElementById("a_title").title = "Oculta configuración digitalización ";
            $("#rigt").css("display", "flex");
            document.getElementById("container_visor").classList.remove("col-12");
            document.getElementById("container_visor").classList.add("col-8");
            document.getElementById("rigt").classList.add("col-4");
            var DWObject = Dynamsoft.DWT.GetWebTwain('dwtcontrolContainer');
            if (DWObject) {
                DWObject.Width = '100%';
                DWObject.Height = '100%';
            }
        } else {
            $("#indice_title").toggleClass("fad");
            $("#indice_title").toggleClass("fa-tools");
            document.getElementById("a_title").title = "Muestra configuración digitalización ";
            $("#rigt").css("display", "none");
            document.getElementById("container_visor").classList.remove("col-8");
            document.getElementById("container_visor").classList.add("col-12");
            document.getElementById("rigt").classList.remove("col-4");
            var DWObject = Dynamsoft.DWT.GetWebTwain('dwtcontrolContainer');
            if (DWObject) {
                DWObject.Width = '100%';
                DWObject.Height = '100%';
            }
           
        }
    }
    catch (err) {
        alert(err.message + " Funcion config_menu_config_escnaer");
    }
}
function event_click_indice() {
    try {    
        if (document.getElementById("rigt").style.display == "none") {
            $("#indice_title").toggleClass("fad");
            $("#indice_title").toggleClass("fa-bars");
            document.getElementById("a_title").title = "Oculta configuración digitalización ";
            $("#rigt").css("display", "flex");
            //$("#rigt").css("width", "30%");
            //$("#container_visor").css("width", "70%");
            document.getElementById("container_visor").classList.remove("col-12");
            document.getElementById("container_visor").classList.add("col-8");
            document.getElementById("rigt").classList.add("col-4");
            var DWObject = Dynamsoft.DWT.GetWebTwain('dwtcontrolContainer');
            DWObject.Width = '100%';
            DWObject.Height = '100%';
        } else {
            $("#indice_title").toggleClass("fad");
            $("#indice_title").toggleClass("fa-tools");
            document.getElementById("a_title").title = "Muestra configuración digitalización ";
            $("#rigt").css("display", "none");
            //$("#rigt").css("width", "0%");
            //$("#container_visor").css("width", "100%");
            document.getElementById("container_visor").classList.remove("col-8");
            document.getElementById("container_visor").classList.add("col-12");
            document.getElementById("rigt").classList.remove("col-4");
            var DWObject = Dynamsoft.DWT.GetWebTwain('dwtcontrolContainer');
            DWObject.Width = '100%';
            DWObject.Height = '100%';      
        }
    }
    catch (err) {
        alert(err.message + " Funcion event_click_indice");
    }
}
function prevent(event, element) {
    try {
        //Evita el posback del boton
        event.preventDefault();
        var panel = element.nextElementSibling;
        element.classList.toggle("active_dive");
        if (panel.style.display === "none") {
            panel.style.display = "block";
        } else {
            panel.style.display = "none";
        }

    }
    catch (err) {
        alert(err.message + " Funcion prevent ");
    }
}
function auto_zise_popup_lista_chequeo(value_lista_general) {
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
        $('#Panel_guarda_servidor').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_guarda_servidor').css("height", (heig_porcent) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#contenido_guarda_servidor').css("height", (document.getElementById("modal_content_guarda_servidor").clientHeight - (document.getElementById("divcabecer2__guarda_servidor").clientHeight + document.getElementById("content_boton_guarda_servidor").clientHeight)) + "px");
        //Para los modal que contiene gred
        $('#Panel_principal').css("height", (document.getElementById("contenido_guarda_servidor").clientHeight - (document.getElementById("contenido_text_nombre").clientHeight + document.getElementById("div_opciones_formato").clientHeight)) + "px");
            if ($('#data_grid td').children.length > 0 && $('#data_grid tr:visible').length > 0) {
                $('#data_grid th').hide();
            } 
        document.getElementById("Hidden_0001").value = "-1";
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_lista_chequeo " + err.message);
    }
}
function ver_modal() {
    var modal = document.getElementById('myModal');
    modal.style.display = "block";
}
//-------////guarda el archivo en el servidor
function activa_document_save() {
    //console.log(document.getElementById("Hidden21").value);
    if (document.getElementById("Hidden21").value == "1") {
        var t = window.parent.document.getElementById('ButtonAlmacenar');
        if (t == undefined) {
            alert("Imposible guardar el documento, falta el formulario padre");

        } else {
            window.parent.document.getElementById('ButtonAlmacenar').click();
        }
    }
    if (document.getElementById("Hidden21").value == "2") {
        document.getElementById("Button_añade_documento").click();
    }
    //Almacena documento para migración  save_document_scan
    if (document.getElementById("Hidden21").value == "3") {
        window.parent.document.getElementById('save_document_scan').click();;
    }
    //Almacena documento para radicación simple
    if (document.getElementById("Hidden21").value == "4") {
        window.parent.document.getElementById('save_document_scan').click();;
    }
    //Remplaza versión de documento digitalizado
    if (document.getElementById("Hidden21").value == "5") {
        window.parent.document.getElementById('Button_save_replace_dig').click();;
    }
}

function progres_hiden(progres) {
    $("#progres_bar").css("display", "none");
}
function load() {
    window.onload = function () {
        window.addEventListener('click', avisarUsuario);
        function avisarUsuario(evObject) {
            evObject.preventDefault();
            var botones = document.querySelectorAll('.boton');
            for (var i = 0; i < botones.length; i++) {
                //botones[i].disabled = true;
            }
            posicion_update_pogres("progres_bar");          
        }
    }
    
}
function deunload() {
    window.onunload = function () {
        window.addEventListener('click', avisarUsuario);
        function avisarUsuario(evObject) {
            evObject.preventDefault();
            var botones = document.querySelectorAll('.boton');
            for (var i = 0; i < botones.length; i++) {
                //botones[i].disabled = false;
            }
            progres_hiden("progres_bar");
        }
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
function activa_salvar_documento() {
    try {
        if (DWObject.HowManyImagesInBuffer == 0) {
            alert("No hay documento para guardar en el servidor");
            return true;
        }
        document.getElementById("Button_guardar_documento").click();
    }
    catch (err) {
        alert(err.message + " activa_salvar_documento");
    }
}
function activa_add_documento() {
    try {
        if (DWObject.HowManyImagesInBuffer == 0) {
            alert("No hay documento para añadir al documento seleccionado");
            return true;
        }
        document.getElementById("Button_adjuntar").click();
    }
    catch (err) {
        alert(err.message + " activa_add_documento");
    }
}
function hiden_modal() {
    $find('ModalPopupExtenderimpre_guarda_servidor').hide();
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
