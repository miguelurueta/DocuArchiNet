$(document).ready(function () {
    $.fn.inicio = function () {
        auto_zise_popup_validacion_radicados();
        auto_zise_popup_gestion_externo();
        auto_zise_popup_visor_externo();
        auto_zise_popup_detalle_trazabilidad();
        auto_zise_popup_detalle_transacciones();
        auto_zise_popup_detalle_respuesta();
        auto_zise_popup_imagen_respuesta();
        service_get_Lista_respuestas_radicado("TextBox_buequeda_general");
        //****************************************VALIDACION RADICACION**********************************************************************************
        //FUNCION ACTIVA SELECCION CLIK EN EL DATAGREDVIEW DE VALIDACION RADICACION
        $('#GridView_val_radicacion tr[id]').click(function () {
            $('#GridView_val_radicacion tr[id]').css({ "background": "White", "color": "Black" });
            $(this).css({ "background-color": "#E7EDF5", "color": "Black" });
            var fer = $(this).attr("id");
            $('#hdnEmailID_VAL').val(fer);
            var recordgred = $('#GridView_val_radicacion tr[id=' + $('#hdnEmailID_VAL').val() + ']');
            var idex = colum_index('RADICADO', 'GridView_val_radicacion');
            if (idex != -1) {
                document.getElementById("Hidden_id_tarea_sel").value = recordgred[0].cells[idex].innerText;
                document.getElementById("Hidden_tipo_visor").value = "VISOR RADICADOR";
                //document.getElementById("Button_visor_emergente").click();
                return false;
            }
            

        });

        $('#GridView_val_radicacion tr[id]').dblclick(function () {

            if ($('#hdnEmailID_VAL').val() != "-1") {
                var recordgred = $('#GridView_val_radicacion tr[id=' + $('#hdnEmailID_VAL').val() + ']');
                var idex = colum_index('RADICADO', 'GridView_val_radicacion');
                if (idex != -1) {
                    document.getElementById("Hidden_id_tarea_sel").value = recordgred[0].cells[idex].innerText;
                    document.getElementById("Hidden_tipo_visor").value = "VISOR RADICADOR";
                    document.getElementById("Button_visor_emergente").click();
                    return false;
                }
            }

        });
        //ASIGNA EL CURSOR DE SELECCION CUANDO PASA EL CURSOR EN EL DATAGREDVIEW DE VALIDACION RADICACION
        $('#GridView_val_radicacion tr[id]').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });
        //INICIA INTERFACE POPUP VALIDACION RADICADOS
        var tempo = document.getElementById("idente_chekbi_actyive");
        if (tempo === null) {
           // $("#GridView_val_radicacion th:nth-child(1)").append(" <input id='idente_chekbi_actyive' type='checkbox' name='activa_deativa_chek' onchange=desactiva_ch_data_grid('idente_chekbi_actyive') class='mmmjjjkkkuuu'  />");
        }
       
        //******************************************FIN****************************************************************************************************
    }

    $('#contenido_datagrid_val_radicacion').contextMenu('context-menu-2', {

        'Ver documentos': {
            click: function (element) {  // element is the jquery obj clicked on when context menu launched
                //$('#HiddenSeleccion').val("-1");


                //document.getElementById("Buttond_Filtro").click();
                if ($('#hdnEmailID_VAL').val() != "-1") {
                    var recordgred = $('#GridView_val_radicacion tr[id=' + $('#hdnEmailID_VAL').val() + ']');
                    var idex = colum_index('RADICADO', 'GridView_val_radicacion');
                    if (idex != -1) {
                        document.getElementById("Hidden_id_tarea_sel").value = recordgred[0].cells[idex].innerText;
                        document.getElementById("Hidden_tipo_visor").value = "VISOR RADICADOR";
                        document.getElementById("Button_visor_emergente").click();
                        return false;
                    }
                }


            }
        },
        'Salir del Menu': {
            click: function (element) { }
        }
    }


    );
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
        service_get_Lista_respuestas_radicado("TextBox_buequeda_general");
    } catch (e) {
        alert(" funcion load " + e.message);
    }
});
const ini_event_page = () => {
    let array_element = new Array;
    array_element.push({ id: "bot_activa_archiva_resp" }, { id : "Btn_Archiva_tramite"});
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
        delete_alert_boot();
        switch (name_ID) {
            case "bot_activa_archiva_resp":
                event_element_click_promise(e);
                break;
            case "Btn_Archiva_tramite":
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
        document.getElementById(name_control).disabled = true;
        let result = "";
        posicion_update_pogres('progres_bar');
        delete_alert_boot();  
        if (name_control == "bot_activa_archiva_resp") {
            result = await ActivaPopuVerArchivaRespuesta();
            if (result != "YES") {
                alert_bot(result, 'warning', "errorgeneralhistorico");
                return true;
            }
        }
        if (name_control == "Btn_Archiva_tramite") {
            result = await ArchivaTramiteRespuesta();
            if (result != "YES") {
                alert_bot(result, 'warning', "error_content_Archiva_tramite");
                return true;
            }
        }
    }
    catch (ex) {
        alert_bot(ex.message, 'warning', "errorgeneralhistorico");
    } finally {
        progres_hiden('progres_bar');
        document.getElementById(name_control).disabled = false;
    }
}
let IdTramiteRespuesta = 0;
const ActivaPopuVerArchivaRespuesta = async () => {
    try {
        let htmlHiden = document.getElementById("hdnEmailID_VAL");
        if (htmlHiden.value == "0" || htmlHiden.value == "-1") {
            return "Por favor, seleccione un trámite de la lista para poder archivar la respuesta.";
        }
        IdTramiteRespuesta = htmlHiden.value;
        $("#modal_content_Archiva_tramite").modal("show");
        return "YES";
    } catch (ex) {
        return ex.mensaje;
    }
}
const ArchivaTramiteRespuesta = async () => {
    try {
        let Result = "";
        let htmloption = document.getElementById("OptionArchiva");
        let OtionArchiva = htmloption.options[htmloption.selectedIndex].text;
        if (OtionArchiva == "" || OtionArchiva == "Seleccione el motivo") {
            return "Por favor, seleccione el motivo por el cual se archivará la respuesta."
        }
        let htmlinput = document.getElementById("NotaArchivo");
        if (htmlinput.value == "") {
            return "Por favor, registre la nota justificativa correspondiente al archivo de la respuesta."
        }
        let NotaArchivadoTramite = OtionArchiva + ". Justificación : ("  + htmlinput.value + ")";
        let RadicadoTramite = "";
        let IdRespuestaRadicado = IdTramiteRespuesta;
        Result = await ServiceRESTarchivaTramiteHistoricoRadicado(RadicadoTramite, IdRespuestaRadicado, NotaArchivadoTramite);
        return Result;
    } catch (ex) {
        return ex.mensaje
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
        auto_zise_popup_validacion_radicados();
        auto_zise_popup_gestion_externo();
        auto_zise_popup_visor_externo();
        auto_zise_popup_detalle_trazabilidad();
        auto_zise_popup_detalle_transacciones();
        auto_zise_popup_detalle_respuesta();
        auto_zise_popup_imagen_respuesta();
    } catch (ex) {
        alert(ex.message + " Función rezize_event")
    }
}
//-------Realiza el archivo de la respuesta
const ServiceRESTarchivaTramiteHistoricoRadicado = async(RadicadoTramite,IdRespuestaRadicado,NotaArchivadoTramite) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceHistoricoCorrespondencia.asmx/ServiceArchivaTramiteHistoricoRadicado', {
                data: "{" + "'RadicadoTramite':'" + RadicadoTramite + "','" + "IdRespuestaRadicado':'" + IdRespuestaRadicado + "','NotaArchivadoTramite':'" + NotaArchivadoTramite + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].AppError !== "YES") {
                        resolve(data.d[0].AppError);
                    } else {
                        $("#modal_content_Archiva_tramite").modal("hide");
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
function acti_busq_general_archivo_boton(e, sender) {
    try {
        document.getElementById("Button_consulta_like").click();
        e.preventDefault();
    } catch (ex) {
        alert("Inconistencia general funcion acti_busq_general_archivo_boton " + ex.message);
    }
}
function restore_acti_busq_general_archivo_boton(e, sender) {
    try {
        document.getElementById("Button_consulta_val_radicacion").click();
        e.preventDefault();
    } catch (ex) {
        alert("Inconistencia general funcion restore_acti_busq_general_archivo_boton " + ex.message);
    }
}
function service_get_Lista_respuestas_radicado(name_texbox) {
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
                    url: "../webservice/WebServiceRadicacion.asmx/GetLista_respuestas_radicado",
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
                document.getElementById("Button_consulta_like").click();
                this.value = ui.item.value;
                return false;
            }

            , minLength: 1, max: 10, scroll: true
        });
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
function activa_descarga_resutados(event, e, event_name) {
    try {
        if (event_name !== "") {
            document.getElementById("Button_Exportar_Radicados").click();
        }

        event.preventDefault();
    }
    catch (ex) {
        alert("Inconsistencia general function activa_descarga_resutados " + ex.message)
    }
}
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
        document.getElementById("Hidden_height").value = espacio_iframe - 20;
        document.getElementById("Hidden_width").value = with_frame - 20;

    }
    catch (err) {
        alert(err.message + " funcion asigna_datos_heig_with " + err.message);
    }
}
function fnexcelcurrier() {
    try {
        var campo_autotizados = "ID|TRAMITE_DOCUMENTO|RADICADO|RADICADO_RESPUESTA|FECHA_VENCE|FECHA_RESPUETA|DESTINATARIO|USUARIO_RESPONSABLE|ASUNTO";
        var matri_campo = campo_autotizados.split("|")
        var tab_text = "<table border='2px'><tr bgcolor='#87AFC6'>";
        var textRange; var j = 0;
        tab = document.getElementById('GridView_val_radicacion'); // id of table
        var tempo = tab.outerHTML;
        for (j = 0 ; j < tab.rows.length ; j++) {
            var tdth;
            var reftabtex = "";
            if (j == 0) {
                tdth = tab.rows[j].getElementsByTagName('th');
            } else {
                tdth = tab.rows[j].getElementsByTagName('td');
            }
            for (k = 0; k < tdth.length ; k++) {
                var nombre_colum;
                nombre_colum = colum_name_index(k)
                var sutch = matri_campo.indexOf(nombre_colum);
                if (sutch !== -1) {
                    reftabtex = reftabtex + tdth[k].outerHTML;

                }

            }
            tab_text = tab_text + reftabtex + "</tr>";

        }
        tab_text = tab_text + "</table>";
        var ua = window.navigator.userAgent;
        var msie = ua.indexOf("MSIE ");

        if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./))      // If Internet Explorer
        {
            txtArea1.document.open("txt/html", "replace");
            txtArea1.document.write(tab_text);
            txtArea1.document.close();
            txtArea1.focus();
            sa = txtArea1.document.execCommand("SaveAs", false, "salida_export.xls");

        }
        else //other browser not tested on IE 11
            sa = window.open('data:application/vnd.ms-excel,' + encodeURIComponent(tab_text), "myWindow", "width=600,height=100");

        return (sa);

    }
    catch (err) {
        alert(err.message + " Funcion fnExcelReport ");
    }
}
function colum_name_index(index_colum) {
    try {
    var x = $('#GridView_val_radicacion th');
    var txt = "";
    var i;
    for (i = 0; i < x.length; i++) {
        if (i == index_colum) {
            txt = x[i].innerText.toUpperCase();
            return txt;
        }

    }
    return txt;
}
    catch (err) {
        alert(err.message + " Funcion colum_name_index ");
}
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
            var scrollableDiv = grid.parent();
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
function retorna_colum_mtriz(hiden_name) {
    try {
    var hiden = document.getElementById(hiden_name);
    var x = $('#GridView_val_radicacion th');
    var txt = "";
    var i;
    for (i = 1; i < x.length; i++) {
        txt = txt + x[i].innerText.toUpperCase() + "|";
    }
    hiden.value = txt;
    return txt;
}
    catch (err) {
        alert(err.message + " funcion retorna_colum_mtriz " + err.message);
}
}
//AUTO SIZE POPUP VALIDACION RADICADOS
function auto_zise_popup_validacion_radicados() {
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
   
    /*$("#contenido_titulo_controles_consulta").css("height", $("#Label_busqueda_panel").height() + 5 + "px");
    $("#contenido_controles_buton_consulta").css("height", ($("#Button_lipiar_val_radicacion").height() + 10) + "px");
    $("#contenido_controles_consulta").css("height", (espacio_iframe - 20) - ($("#contenido_titulo_controles_consulta").height() + $("#contenido_controles_buton_consulta").height() + $('#menucab').height()) + "px");
    $("#_Panelvalidacion_val_radicacion").css("height", (espacio_iframe - 20) - ($("#contenido_titulo_controles_consulta").height() + $("#contenido_controles_buton_consulta").height() + $('#menucab').height()) + "px");
    $("#contenido_titulo_val_radicacion").css("height", ($("#titulo_label_val_radicacion").height() + 10) + "px");
    $("#Contenido_botones_tipo_radicado").css("height", $("#Button_Exportar_Radicados").height() + 10 + "px");
    $("#contenido_datagrid_val_radicacion").css("height", (espacio_iframe - 20) - ($("#contenido_titulo_val_radicacion").height() + $("#Contenido_botones_tipo_radicado").height() + $('#menucab').height()) + "px");*/
    $('#Contentizquierdo').css("height", ((espacio_iframe - 5) - document.getElementById("menucab").clientHeight) + "px");
    $('#sidebar_').css("height", ((espacio_iframe - 5) - document.getElementById("menucab").clientHeight) + "px");
    $("#contenido_controles_consulta").css("height", (document.getElementById("Contentizquierdo").clientHeight) - (document.getElementById('contenido_titulo_controles_consulta').clientHeight + document.getElementById('contenido_controles_buton_consulta').clientHeight) + "px");
    $("#_Panelvalidacion_val_radicacion").css("height", (document.getElementById("Contentizquierdo").clientHeight) - (document.getElementById('contenido_titulo_controles_consulta').clientHeight + document.getElementById('contenido_controles_buton_consulta').clientHeight) + "px");
    $('#Contenedorderecho').css("height", ((espacio_iframe - 5) - document.getElementById("menucab").clientHeight) + "px");
    $("#contenido_datagrid_val_radicacion").css("height", (document.getElementById("Contenedorderecho").clientHeight - document.getElementById('contenido_titulo_val_radicacion').clientHeight) + "px");
}
    catch (ex) {
        alert("Function auto_zise_popup_validacion_radicados " + ex.message)
}
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
  
        //corrige el posicionamiento horizontal del modalpopuextender, la propiedad width del panel debe estar en auto
        //var widtth_procent_left_rigth = (with_frame - document.getElementById("Panel_agregar_expediente_carpeta").clientHeight) / 2;
        //$('#Panel_agregar_expediente_carpeta').css("left", (Math.round(widtth_procent_left_rigth)) + "px");
    var heig_porcent = espacio_iframe - ((espacio_iframe * 10) / 100);  // Indica el porcentaje de espacio vertical del elemento
    $('#Panel_trazabilidad').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
    $('#modal_content_Panel_trazabilidad').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
    $('#Cotenedorpendiente_trazabilidad').css("height", (document.getElementById("modal_content_Panel_trazabilidad").clientHeight - (document.getElementById("Cabecerapendiente_trazabilidad").clientHeight)) + "px");
        //Para los modal que contiene gred
    $('#Iframe_trazabilidad_').css("height", (document.getElementById("Cotenedorpendiente_trazabilidad").clientHeight - 1) + "px");
}
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_detalle_trazabilidad");
}
}
function auto_zise_popup_detalle_respuesta() {
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
        /*$('#Panel_detalle_respuesta').css("height", (espacio_iframe - 40) + "px");
        $('#Cotenedorpendiente_detalle_respuesta').css("height", (espacio_iframe - 40) + "px");
        $('#Iframe_detalle_respuesta_').css("height", (espacio_iframe - 40) + "px");*/
        //corrige el posicionamiento horizontal del modalpopuextender, la propiedad width del panel debe estar en auto
        //var widtth_procent_left_rigth = (with_frame - document.getElementById("Panel_agregar_expediente_carpeta").clientHeight) / 2;
        //$('#Panel_agregar_expediente_carpeta').css("left", (Math.round(widtth_procent_left_rigth)) + "px");
        var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_detalle_respuesta').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_Panel_detalle_respuesta').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#Cotenedorpendiente_detalle_respuesta').css("height", (document.getElementById("modal_content_Panel_detalle_respuesta").clientHeight - (document.getElementById("Cabecerapendiente_detalle_respuesta").clientHeight)) + "px");
        //Para los modal que contiene gred
        $('#Iframe_detalle_respuesta_').css("height", (document.getElementById("Cotenedorpendiente_detalle_respuesta").clientHeight - 1) + "px");
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_detalle_respuesta");
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
        /*$('#Panel_transacciones').css("height", (espacio_iframe - 40) + "px");
        $('#Cotenedorpendiente_transacciones').css("height", (espacio_iframe - 40) + "px");
        $('#Iframe_transacciones').css("height", (espacio_iframe - 40) + "px");*/

        //corrige el posicionamiento horizontal del modalpopuextender, la propiedad width del panel debe estar en auto
        //var widtth_procent_left_rigth = (with_frame - document.getElementById("Panel_agregar_expediente_carpeta").clientHeight) / 2;
        //$('#Panel_agregar_expediente_carpeta').css("left", (Math.round(widtth_procent_left_rigth)) + "px");
        var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_transacciones').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_Panel_transacciones').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#Cotenedorpendiente_transacciones').css("height", (document.getElementById("modal_content_Panel_transacciones").clientHeight - (document.getElementById("Cabecerapendiente_transacciones").clientHeight)) + "px");
        //Para los modal que contiene gred
        $('#Iframe_transacciones_').css("height", (document.getElementById("Cotenedorpendiente_transacciones").clientHeight - 1) + "px");
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_detalle_transacciones");
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
        var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_imagen_respuesta').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_Panel_imagen_respuesta').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#Cotenedorpendiente_imagen_respuesta').css("height", (document.getElementById("modal_content_Panel_imagen_respuesta").clientHeight - (document.getElementById("Cabecerapendiente_imagen_respuesta").clientHeight )) + "px");
        //Para los modal que contiene gred
        $('#Iframe_imagen_respuesta_').css("height", (document.getElementById("Cotenedorpendiente_imagen_respuesta").clientHeight - 5) + "px");
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_imagen_respuesta");
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
function prevent(event, element) {
    try {
        var fer = $(element).attr("idd");
        var tip_event = $(element).attr("tip_event");
        document.getElementById("Hidden_menu_var_event_dive").value = tip_event;
        $('#hdnEmailID_VAL').val(fer);
        document.getElementById("Button_me_active_men_dive").click();
        event.preventDefault();
        element.focus();
    }
    catch (err) {
        alert(err.message + " Funcion prevent");
    }
}
//  AUTOSIZE DATA GREVIEW
function plugin_grwedview() {
    try {
    var gridwith = $('#contenido_datagrid_val_radicacion').width();
    var gridheihg = $('#contenido_datagrid_val_radicacion').height();
    //LLAMA PLUGIN FIJA HIDER O TITULOS   
    if ($('#GridView_val_radicacion td').children.length > 0) {
        $(document).ready(function () { $('#GridView_val_radicacion').gridviewScroll({ width: gridwith, height: gridheihg }); })
    }
    }
    catch (err) {
        alert(err.message + " funcion plugin_grwedview " + err.message);
    }
}

//Retorna el idex de una columna en una tabla
function colum_index(colum_name) {
    try {
    var x = $('#GridView_val_radicacion th');
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
function progres_hiden(progres) {
    try {
        $("#progres_bar").css("display", "none");
    }
    catch (err) {
        alert(err.message + " Funcion progres_hiden");
    }
}
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
        alert(err.message + " Funcion posicion_update_pogres");
    }

}

function onDataShown(sender, args) {
    sender._popupBehavior._element.style.zIndex = 1000001;

}
function auto_zise_popup_gestion_externo() {
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
    $('#ModalPopupExtender_valiacion_plantilla').css("height", (espacio_iframe - 40) + "px");
    $('#Iframe_validacion_plantilla_').css("height", (espacio_iframe - 42) + "px");
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
$(document).on('keydown', function (e) {
    if (e.which == 9) {
        var id_element = e.srcElement.className;
        var salidadato;
        if (id_element == "date_2") {
            var dato = e.srcElement.value;


            if (dato == "") {

                return false;
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
                salidadato = Año_F + "-" + Mes_f + "-" + Dia_f;
                e.srcElement.value = salidadato;
            }

            if (numerocaracter == 10) {
                salidadato = Año_F + "-" + Mes_f + "-" + Dia_f;
                e.srcElement.value = salidadato;
            }

        }
    }
});
