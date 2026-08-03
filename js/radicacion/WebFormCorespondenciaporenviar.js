$(document).ready(function () {
    $.fn.inicio = function () {
        //****************************************VALIDACION RADICACION**********************************************************************************
        //FUNCION ACTIVA SELECCION CLIK EN EL DATAGREDVIEW DE VALIDACION RADICACION
        $('#GridView_val_radicacion tr[id]').click(function () {
            $('#GridView_val_radicacion tr[id]').css({ "background": "White", "color": "Black" });
            $(this).css({ "background-color": "#E7EDF5", "color": "Black" });
            var fer = $(this).attr("id");
            $('#hdnEmailID_VAL').val(fer);
            var sele_row = $('#GridView_val_radicacion tr[id=' + $('#hdnEmailID_VAL').val() + ']');
            var columindex = colum_index("ID_RESPUESTA_RADICADO");
            if (columindex == -1) {
                alert("Imposible encontrar el index de la columna ID_RESPUESTA_RADICADO");
                return false;
            }

            if (sele_row[0].cells[columindex].innerText != "1") {
                //alert("El expediente esta cerrado, no se puede asignar");
                //return false;
            }
            // $('#Hidden_consecutivo_radicado').val(sele_row[0].cells[columindex].innerText);

        });
        $('#GridView_val_radicacion tr[id]').dblclick(function () {
            var fer = $(this).attr("id");
            if (fer !== "-1") {
                window.document.getElementById("Button_ver_documento").click();
            }
        })

        //ASIGNA EL CURSOR DE SELECCION CUANDO PASA EL CURSOR EN EL DATAGREDVIEW DE VALIDACION RADICACION
        $('#GridView_val_radicacion tr[id]').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });
        //INICIA INTERFACE POPUP VALIDACION RADICADOS
        var tempo = document.getElementById("idente_chekbi_actyive");
        if (tempo === null) {
            $("#GridView_val_radicacion th:nth-child(1)").append(" <input id='idente_chekbi_actyive' type='checkbox' name='activa_deativa_chek' onchange=desactiva_ch_data_grid('idente_chekbi_actyive') class='dummychkstyle_'  />");
        }
        auto_zise_popup_validacion_radicados();
        auto_size_popup_procesa_tramite();
        actuo_zise_popup_compartir_correo_electronico();
        auto_size_control_documentos();
        $(window).resize(bodyResize);
        function bodyResize() {
            auto_zise_popup_validacion_radicados();
            auto_size_popup_procesa_tramite();
            resize_opcion_descarga_respuesta();
            plugin_grwedview();
            mueve_scroll_data_gred('GridView_val_radicacion', 'hdnEmailID_VAL');
            actuo_zise_popup_compartir_correo_electronico();
            auto_size_control_documentos();
            if (document.getElementById('Area_Visor').style.display == 'block') {
                dispalyVisorEmergente();
            }
            if (document.getElementById('Are_Digitalizacion').style.display == 'block') {
                dispalyInterfaceEscaner();
            }
        }
        
        $('#TreeViewseleccion_digitalizado').dblclick(function () {
            document.getElementById("ButtonVisua").click();
        });


        //CURSOR Y SELECCION LISTA CHEQUEO ADJUNTA DOCUMENTO 
        $('#data_grid_chequeo tr[id]').click(function () {
            $('#data_grid_chequeo tr[id]').css({ "background": "White", "color": "Black" });
            $(this).css({ "background-color": "#E7EDF5", "color": "Black" });
            var fer = $(this).attr("id");
            $('#Hidden_0001').val(fer);
        });
        //ASIGNA EL CURSOR DE SELECCION CUANDO PASA EL CURSOR
        $('#data_grid_chequeo tr[id]').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });

        //CURSOR Y SELECCION LISTA CHEQUEO ACTUALIZA TIPO DOCUMENTO
        $('#data_grid_chequeo_actualiza tr[id]').click(function () {
            $('#data_grid_chequeo_actualiza tr[id]').css({ "background": "White", "color": "Black" });
            $(this).css({ "background-color": "#E7EDF5", "color": "Black" });
            var fer = $(this).attr("id");
            $('#Hidden_0003').val(fer);
        });
        //ASIGNA EL CURSOR DE SELECCION CUANDO PASA EL CURSOR
        $('#data_grid_chequeo_actualiza tr[id]').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });

      
        
        //MUEVE SCROLL DATA GRED (VALIDACION DE RADICADOS SE REALIZA EN PAGUE REGUEST
        // mueve_scroll_data_gred('GridView_val_radicacion', 'hdnEmailID_VAL');
        //OCULTA LA CLAVE PRINCIPAL
        //$("#GridView_val_radicacion th:nth-child(1)").hide();
        //$("#GridView_val_radicacion td:nth-child(1)").hide();
        $("#GridView_val_radicacion th:nth-child(2)").hide();
        $("#GridView_val_radicacion td:nth-child(2)").hide();

        //******************************************FIN****************************************************************************************************
    }


})
function auto_zise_popup_plantilla_validacion() {
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

        $('#Contenido_validacion_plantilla').css("zIndex", 100030)
        $('#Panel_valiacion_plantilla').css("zIndex", 100030)
        $('#Panel_valiacion_plantilla').css("width", (with_frame) + "px");
        $('#Panel_valiacion_plantilla').css("height", (espacio_iframe - 5) + "px");
        $('#Iframe_validacion_plantilla_').css("height", (espacio_iframe - 5) + "px");

    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_plantilla_validacion");
    }
}
function auto_zise_popup_editar_radicados() {
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
        document.getElementById("Hidden_height").value = espacio_iframe - 5;
        document.getElementById("Hidden_width").value = with_frame;

    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_editar_radicados");
    }
}
function auto_size_control_documentos() {
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
        document.getElementById("Area_Enlace").style.height = (espacio_iframe - 40) + "px";
        document.getElementById("Datos_Digitalizacion_botones").style.height = (30) + "px";
        var total_altura = document.getElementById("Separacio2n").clientHeight + document.getElementById("Datos_Enlace").clientHeight + document.getElementById("Separacion").clientHeight + document.getElementById("Datos_Digitalizacion_botones").clientHeight + 30;
        //var total_altura = document.getElementById("Labeltext").clientHeight + document.getElementById("Button_actualiza_enlace").clientHeight + document.getElementById("Buttonaceptar").clientHeight + document.getElementById("Label_relacion_documentos").clientHeight + document.getElementById("ImagebutonActualizarA").clientHeight + document.getElementById("Label_estado_lista").clientHeight;
        document.getElementById("seleccion_documentos_digitalizados").style.height = ((document.getElementById("Area_Enlace").clientHeight - total_altura) - 20) + "px";
        document.getElementById("TreeViewseleccion_digitalizado").style.height = ((document.getElementById("Area_Enlace").clientHeight - total_altura) - 30) + "px";
        //TreeViewseleccion_digitalizado
    }
    catch (err) {
        alert(err.message + " Funcion auto_size_control_documentos");
    }
}
function dispalyInterfaceEscaner() {
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
        document.getElementById("div_cerrar").style.display = "none";
        document.getElementById('Are_Digitalizacion').style.width = (with_frame - document.getElementById("Area_Enlace").clientWidth) - 5 + "px";
        document.getElementById('Are_Digitalizacion').style.height = "100%";
        document.getElementById('Are_Digitalizacion').style.display = 'block';
        document.getElementById('Area_Visor').style.display = 'none';
        document.getElementById('Area_Visor').style.width = "0%";
        document.getElementById('Area_Visor').style.height = "0%";

    }
    catch (err) {
        alert(err.message + " Funcion dispalyInterfaceEscaner");
    }
    
}
function dispalyVisorEmergente() {
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
        document.getElementById("div_cerrar").style.display = "block";
        document.getElementById('Are_Digitalizacion').style.display = 'none';
        document.getElementById('Are_Digitalizacion').style.width = "0%";
        document.getElementById('Are_Digitalizacion').style.height = "0%";
        document.getElementById('Area_Visor').style.display = 'block';
        document.getElementById('Area_Visor').style.width = (with_frame - document.getElementById("Area_Enlace").clientWidth) - 5 + "px";
        document.getElementById('Area_Visor').style.height = ((espacio_iframe - document.getElementById("div_cerrar").clientHeight) - 25) + "px";


    }
    catch (err) {
        alert(err.message + " Funcion dispalyVisorEmergente");
    }
}
function prevent_cerrar(event, element) {
    try {
        //Evita el posback del boton
        event.preventDefault();
        dispalyInterfaceEscaner();

    }
    catch (err) {
        alert(err.message + " Funcion prevent ");
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



        var gridwith = document.getElementById("Contenedorgrid").clientWidth;
        var gridheihg = document.getElementById("Contenedorgrid").clientHeight;
        //$('#data_grid_chequeo').css("height", (gridheihg - 5) + "px");
        //LLAMA PLUGIN FIJA HIDER O TITULOS   
        if (value_lista_general == "1") {
            if ($('#data_grid_chequeo td').children.length > 0 && $('#data_grid_chequeo tr:visible').length > 0) {
                $('#data_grid_chequeo th').hide();


            }
        }

        document.getElementById("Hidden_0001").value = "-1";
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_lista_chequeo " + err.message);
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
function activa_boton_dowload() {
    try {

        document.getElementById("Button_guardar_desicion").click();
    }
    catch (err) {
        alert(err.message + " funcion activa_boton_dowload " + err.message);
    }
}
function acti_busq_lista_cheq(e, sender) {
    try {

        tecla = (document.all) ? e.keyCode : e.which;
        if (tecla == 13) {
            //document.getElementById("Button_busca_general_archivo").click();
            busqueda_gred_lista_cheq('Hidden_0001', 'data_grid_chequeo', 'TextBox_contenido_busqueda_lista_cheq', 'CheckBox_busqueda_list_cheq');
            e.preventDefault();
            //return false;
        }


    } catch (err) {
        alert(err.message + " funcion acti_busq_general_archivo " + err.message);
    }
}
function busqueda_gred_lista_cheq(HiddenSeleccion, data_grid, contenido_busqueda, CheckboxBusqueda) {
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
            var scrollableDiv = $("#" + "Panel_principal");
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
        alert(err.message + " funcion busqueda_gred_lista " + err.message);
    }
    finally {
        document.getElementById(contenido_busqueda).focus();
    }

}
function auto_zise_popup_lista_chequeo_edita(value_lista_general) {
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




        var gridwith = document.getElementById("Contenedorgrid_edita").clientWidth;
        var gridheihg = document.getElementById("Contenedorgrid_edita").clientHeight;
        //$('#data_grid_chequeo_actualiza').css("height", (gridheihg - 5) + "px");
        //LLAMA PLUGIN FIJA HIDER O TITULOS   
        if (value_lista_general == "1") {
            if ($('#data_grid_chequeo_actualiza td').children.length > 0 && $('#data_grid_chequeo_actualiza tr:visible').length > 0) {
                $('#data_grid_chequeo_actualiza th').hide();

            }
        }
        document.getElementById("Hidden_0003").value = "-1";
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_lista_chequeo_edita " + err.message);
    }
}
function ConfirmMensaje(mensaje) {
    try {
        var x;
        var r = confirm(mensaje);
        if (r == true) {
            x = "0";
        }
        else {
            x = "1";
        }
        document.getElementById("HiddenPROMP").value = x;
    }
    catch (err) {
        alert(err.message + " Funcion ConfirmMensaje");
    }
}
function fnexcelcurrier(campo_autotizados) {
    try {
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
function activa_menu_general_diference_(event, e, event_name) {
    try {
        if (event_name !== "") {
            document.getElementById("Hidden_menu_var_event_dive").value = event_name;
            if (event_name == "O-CER") {
                document.getElementById("Button_procesar_envio").click();
            }
            if (event_name == "R-DDR") {
                document.getElementById("Button_ver_documento").click();
            }
            if (event_name == "R-GDR") {
                document.getElementById("Button_digitaliza_documento").click();
            }
            if (event_name == "E-ERC") {
                document.getElementById("Button_reporte_currier").click();
            }
            if (event_name == "E-GDR") {
                document.getElementById("Button_exportar").click();
            }
            if (event_name == "R-ENC") {
                document.getElementById("Button_notificar_envio").click();
            }
            if (event_name == "G-GGG") {
                document.getElementById("Button_asigna_guia").click();
            }
            if (event_name == "G-IPG") {
                document.getElementById("Button_imprimir_guia_").click();
            }
            if (event_name == "G-GDR") {
                document.getElementById("Button_descarga_guia_").click();
            }
            if (event_name == "G-GAG") {
                document.getElementById("Button_anular_guia_").click();
            }
        }

        event.preventDefault();
    }
    catch (ex) {
        alert("Inconsistencia general function activa_menu_general_diference " + ex.message)
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

        $('#Panel_notifica_gestion').css("height", "60%");
        $('#contenido_procesa_notifica_gestion').css("height", (document.getElementById("Panel_notifica_gestion").clientHeight - 20) + "px");
        $('#Iframe_comparte_coreo').css("height", (document.getElementById("contenido_procesa_notifica_gestion").clientHeight - 5) + "px");
    }
    catch (ex) {
        alert("Incosistencia general función actuo_zise_popup_compartir_correo_electronico " + ex)
    }
}
function auto_zise_popup_plantilla_validacion() {
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

        $('#Contenido_validacion_plantilla').css("zIndex", 100030)
        $('#Panel_valiacion_plantilla').css("zIndex", 100030)
        $('#Panel_valiacion_plantilla').css("width", (with_frame) + "px");
        $('#Panel_valiacion_plantilla').css("height", (espacio_iframe - 5) + "px");
        $('#Iframe_validacion_plantilla_').css("height", (espacio_iframe - 5) + "px");
        
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_plantilla_validacion");
    }
}
function auto_zise_popup_editar_radicados() {
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
        document.getElementById("Hidden_height").value = espacio_iframe - 5;
        document.getElementById("Hidden_width").value = with_frame;
        
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_editar_radicados");
    }
}
function fnexcelcurrier(campo_autotizados) {
    try {
        var matri_campo = campo_autotizados.split("|")
        var tab_text = "<table border='2px'><tr bgcolor='#87AFC6'>";
        var textRange; var j = 0;
        tab = document.getElementById('GridView_val_radicacion'); // id of table
        var tempo = tab.outerHTML;
        for (j = 0 ; j < tab.rows.length ; j++) {
            var tdth;
            var reftabtex="";
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
function fnExcelReport()
{
    try {
    var tab_text="<table border='2px'><tr bgcolor='#87AFC6'>";
    var textRange; var j=0;
    tab = document.getElementById('GridView_val_radicacion'); // id of table
    for(j = 0 ; j < tab.rows.length ; j++)
    {
        var tdth;
        var reftabtex = "";
        if (j == 0) {
            tdth = tab.rows[j].getElementsByTagName('th');
        } else {
            tdth = tab.rows[j].getElementsByTagName('td');
        }
        for (k = 0; k < tdth.length ; k++) {
            var suitch=1;
            var redisp = tdth[k].style.display;
            if (redisp == "none") {
                suitch = -1;
               
            }
            if (k == 0) {
                if (j == 0) {
                    var t = tdth[k].getElementsByClassName("dummychkstyle_");
                    if (t[0].className == "dummychkstyle_") {
                        suitch = -1;
                       
                    }
                } else {
                    var t = tdth[k].getElementsByClassName("dummychkstyle");
                    if (t[0].className == "dummychkstyle") {
                        suitch = -1;
                      
                    }
                }
            }
            if (suitch == 1) {
                reftabtex = reftabtex + tdth[k].outerHTML;
            }

        }
        tab_text = tab_text + reftabtex + "</tr>";
       
    }
    tab_text=tab_text+"</table>";
    var ua = window.navigator.userAgent;
    var msie = ua.indexOf("MSIE ");
  
    if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./))      // If Internet Explorer
    {
        txtArea1.document.open("txt/html","replace");
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
  
function descargarExcel() {
    //Creamos un Elemento Temporal en forma de enlace
    var tmpElemento = document.createElement('a');
    // obtenemos la información desde el div que lo contiene en el html
    // Obtenemos la información de la tabla

    var data_type = 'data:application/vnd.ms-excel';
    var tabla_div = document.getElementById('GridView_val_radicacion');
    var tr = $('#GridView_val_radicacion tr');
    tr[0].display = "block";
    var tabla_html = tabla_div.outerHTML.replace(/ /g, '%20');
    tmpElemento.href = data_type + ', ' + tabla_html;
    //Asignamos el nombre a nuestro EXCEL
    tmpElemento.download = 'Nombre_De_Mi_Excel.xls';
    // Simulamos el click al elemento creado para descargarlo
    tmpElemento.click();
    tr[0].display = "none";
}
function resize_opcion_descarga_respuesta() {
    try {
        var espacio_iframe;
        if (window.innerHeight) {
            //navegadores basados en mozilla 
            espacio_iframe = window.innerHeight
        } else {
            if (document.hidden == true) {
                if (document.body.clientHeight != undefined) {
                    //Navegadores basados en IExplorer, es que no tengo innerheight 
                    espacio_iframe = document.body.clientHeight
                } else {
                    //otros navegadores 
                    espacio_iframe = 478
                }
            }
        }
       
        var heig_ = document.getElementById("div_title_opcion_descarga_respuesta").clientHeight + document.getElementById("div_opcion_descarga_respuesta").clientHeight + document.getElementById("div_inferior_opcion_descarga_respuesta").clientHeight;
        $("#Contenido_opcion_descarga_respuesta").css("height", heig_ + document.getElementById("Divcerrarbuton2_opcion_descarga_respuesta").clientHeight + "px");
        $("#Panel_opcion_descarga_respuesta").css("height", heig_ + 80 + "px");


    }
    catch (err) {
        alert(err.message + " funcion resize_opcion_descarga_respuesta " + err.message);
    }
}
//Actualiza el campo fecha limite de respuesta del documento
function actualiza_gred_guia_respuesta() {
    try {
        $("#GridView_val_radicacion tr[id=" + $("#hdnEmailID_VAL").val() + "]").each(function () {
            var idex = -1;
            var drop = document.getElementById("DropDownList_procesa_tramite_envio");
            var text = document.getElementById("TextBox_codigo_guia_envio");
            idex = colum_index("GUIA_ENVIO");
            if (idex != -1) {
                
                if ($(this)[0].cells[idex].children.length > 0) {
                    $(this)[0].cells[idex].children[0].innerText = text.value;
                    $(this)[0].cells[idex].children[0].style.overflow = "auto";
                } else {
                    $(this)[0].cells[idex].innerText = "";
                    var heade = $("#GridView_val_radicacionCopy th");
                    var div = document.createElement("div");
                    div.innerText = text.value;
                    div.style.overflow = "auto";
                    //heade[idex].children[0].style.width = div.style.width
                    div.style.width = heade[idex].children[0].style.width;
                    $(this)[0].cells[idex].appendChild(div);

                }
            }
            idex = colum_index("EMPRESA_ENVIO");
            if (idex != -1) {
                
                if ($(this)[0].cells[idex].children.length > 0) {
                    $(this)[0].cells[idex].children[0].innerText = drop.value;
                    $(this)[0].cells[idex].children[0].style.overflow = "auto";
                } else {
                   
                    $(this)[0].cells[idex].innerText = "";
                    var heade = $("#GridView_val_radicacionCopy th");
                    var div = document.createElement("div");
                    div.innerText = drop.value;
                    div.style.overflow = "auto";
                    //heade[idex].children[0].style.width = div.style.width
                    div.style.width = heade[idex].children[0].style.width;
                    $(this)[0].cells[idex].appendChild(div);

                }     
            }
            
        })
        
    }
    catch (err) {
        alert(err.message + " Funcion actualiza_gred_limite_respuesta");
    }
}
//SELECCIONA LAS TAREAS CON CHECK
function asigna_usuario_grupos_cheked() {
    try {
        var fer = "0";
        $('#hdnEmailID_sel').val("0");
        $('#GridView_val_radicacion .dummychkstyle').each(function () {
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
                        fer = fer + "." + atri;
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
//ELIMINA REGISTROS GRED PENDIENTES
function elimina_registro_gred_pendiente() {
    try {
        if (document.getElementById("Hidden_lista_eliminar_tarea") == "0") {
            return false;
        }
        var spli = document.getElementById("Hidden_lista_eliminar_tarea").value.split(".");
        for (i = 0; i <= spli.length - 1 ; i++) {
            $('#GridView_val_radicacion .dummychkstyle').each(function () {
                var nod = $(this);
                if (nod[0].children[0].checked == true) {
                    var cel = $(this).parent().parent().parent();
                    var atri = $(this).parent().parent().parent().attr("id");
                    if (atri == undefined) {
                        cel = $(this).parent().parent();
                        atri = $(this).parent().parent().attr("id");
                    }
                    if (atri == spli[i]) {
                        document.getElementById("GridView_val_radicacion").deleteRow(cel[0].rowIndex);
                    }
                }
            })
        }

    }
    catch (err) {
        alert(err.message);
    }
    finally {
        document.getElementById("Hidden_lista_eliminar_tarea").value = "0";
    }
}
function actualiza_gre_campos_dinamicos() {
    var hidendcampos = document.getElementById("Hidden_campos_dinamicos_edita").value;
    var spli_campos = hidendcampos.split("|");
    $("#GridView_val_radicacion tr[id=" + $("#hdnEmailID_VAL").val() + "]").each(function () {
        var idex = -1;
        //cargo_destinatario
        for (i = 0; i <= (spli_campos.length - 1) ; i++) {
            var control = document.getElementById(spli_campos[i]);
            var name = spli_campos[i].split("-");
            if (control != undefined) {
                idex = colum_index(name[1]);
                if (idex != -1) {

                    $(this)[0].cells[idex].innerText = control.value;

                }
            }
        }
    })
}
function eliminar_fila_data_gred() {
    try {
    $('#GridView_val_radicacion tr[id=' + $('#hdnEmailID_VAL').val() + ']').remove();
    $('#hdnEmailID_VAL').val("-1");
    var chid = $('#GridView_val_radicacion >tbody >tr').length;
    if (chid >= 1) {
        chid = chid -1 ;
    }
    var iff = document.forms.item(0).id;
    if (document.forms.item(0).id == "form_archiva") {
        document.getElementById("titulo_label_val_radicacion").innerHTML = "Se encontraron " + chid + " registro(s) enviados por archivar ";
    } else {
        document.getElementById("titulo_label_val_radicacion").innerHTML = "Se encontraron " + chid + " registro(s) por enviar ";
    }
}
    catch (err) {
        alert(err.message + " Funcion eliminar_fila_data_gred");
}
   
}
function confirma_respuesta(mensaje) {
    try {
    var res = confirm(mensaje);
    if (res == true) {
        document.getElementById("Hidden_alert_respuesta").value = "YES";
    } else {
        document.getElementById("Hidden_alert_respuesta").value = "NO";
    }
}
    catch (err) {
        alert(err.message + " Funcion confirma_respuesta");
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

function onDataShown(sender, args) {
    sender._popupBehavior._element.style.zIndex = 1000001;

}
//MUEVE EL SCCROL AL ID SELECCIONADO
function mueve_scroll_data_gred(data_grid, HiddenSeleccion) {
    if ($("#" + HiddenSeleccion).val() != "-1") {
        var scrollableDiv = $("#" + data_grid).parent();
        //limpia todos los seleccionados
        $("#" + data_grid + " tr[id]").css({ "background-color": "transparent", "color": "Black" });
        $("#" + data_grid + " tr[id=" + $("#" + HiddenSeleccion).val() + "]").css({ "background-color": "#E7EDF5", "color": "Black" });
        $("#" + data_grid + " tr[id= " + $("#" + HiddenSeleccion).val() + "]").each(function () {
            $(scrollableDiv).scrollTop(70);
            $(scrollableDiv).scrollTop(($(this).offset().top));
            return true;
        });
    }
}
function inactiva_chek() {
    //document.getElementById("hdnEmailID_VAL").value == "-1";
    //xd5("GridView_val_radicacion", "hdnEmailID_VAL");
}
//DESACTIVA CHEK
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
//FUNCION ACTIVA Y DEACTIVA LOS CAMPOS CHEKEADOS EN UNA TABLA
function desactiva_ch_data_grid(idente_chekbi_actyive) {
    var e = $("#" + idente_chekbi_actyive);

    if ($(e).is(':checked')) {
        var x = document.getElementsByClassName("dummychkstyle");
        for (i = 0; i < x.length; i++) {
            var z = x[i].firstChild;
            z.checked = false;

        }

    }
    else {

        var x = document.getElementsByClassName("dummychkstyle");
        for (i = 0; i < x.length; i++) {
            var z = x[i].firstChild;
            z.checked = true;

        }


    }
}
//AUTO SIZE POPUP VALIDACION RADICADOS
function auto_zise_popup_validacion_radicados() {
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
    var heigconetedor;
    $('#Contenedorderecho').css("height", (espacio_iframe - 40) + "px");
    heigconetedor = document.getElementById("Button_consulta_pendientes_procesar").clientHeight + $("#inferior_bajo_boton").height() + 5;
    $("#Contenido_botones_tipo_radicado").css("height", (heigconetedor) + "px");
    heigconetedor = document.getElementById("TextBox_busqueda").clientHeight + 5;
    $("#contenido_titulo_val_radicacion").css("height", (heigconetedor) + "px");
    heigconetedor = $('#Contenedorderecho').height() - ($("#Contenido_botones_tipo_radicado").height() + ($("#contenido_titulo_val_radicacion").height() + $("#menu_var").height()));
    $("#contenido_datagrid_val_radicacion").css("height", (heigconetedor + 10) + "px");
    $('#Contentizquierdo').css("height", (espacio_iframe - (40 + $("#menu_var").height()) + "px"));
    heigconetedor = $("#Button_consulta_pendientes_procesar").height() + 10;
    $("#contenido_controles_buton_consulta").css("height", (heigconetedor) + "px");
    heigconetedor = document.getElementById("TextBox_busqueda").clientHeight + 5;
    $("#contenido_titulo_controles_consulta").css("height", (heigconetedor) + "px");
    heigconetedor = $('#Contentizquierdo').height() - ($("#contenido_controles_buton_consulta").height() + ($("#contenido_titulo_controles_consulta").height()));
    $("#contenido_controles_consulta").css("height", (heigconetedor + 10) + "px");
    $("#_Panelvalidacion_val_radicacion").css("height", (heigconetedor + 10) + "px");

}
function auto_size_popup_procesa_tramite()
{
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
    $('#contenido_procesa_tramite_envio').css("height", (espacio_iframe - 50) + "px");

   
}
function on_clik(buton) {
    document.getElementById(buton).click();
}
function plugin_grwedview() {
    try {
        var gridwith = $('#contenido_datagrid_val_radicacion').width();
        var gridheihg = $('#contenido_datagrid_val_radicacion').height();
        //LLAMA PLUGIN FIJA HIDER O TITULOS   
        if ($('#GridView_val_radicacion td').children.length > 0) {
            $(document).ready(function () { $('#GridView_val_radicacion').gridviewScroll({ width: gridwith, height: gridheihg }); })
        }
        //var x2 = $('#GridView_val_radicacionCopy th');
        var x2 = $('#GridView_val_radicacionHeaderCopy th').first();
        if (x2.length > 0) {
            x2[0].firstChild.style.textAlign = "center";

        }
    }
    catch (err) {
        alert(err.message + " Funcion plugin_grwedview");
    }
}
//  AUTOZISE POPUP EDITA RADICADOS ENTRANTES

//Retorna el idex de una columna en una tabla
function retorna_colum_mtriz(hiden_name) {
    var hiden = document.getElementById(hiden_name);
    var x = $('#GridView_val_radicacion th');
    var txt = "";
    var i;
    for (i = 0; i < x.length; i++) {
        txt = txt + x[i].innerText.toUpperCase() + "|";
    }
    hiden.value = txt;
    return txt;
}
function colum_index(colum_name) {

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

function busqueda_gred_por_enviar(HiddenSeleccion, data_grid, contenido_busqueda, CheckboxBusqueda) {
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
                        //var id_ref = $(this).parent();
                        //$(scrollableDiv).scrollTop($(id_ref).offset().top);
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
                        //$(scrollableDiv).scrollTop(70);
                        //var id_ref = $(this).parent();
                        //$(scrollableDiv).scrollTop($(id_ref).offset().top);
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
$(document).on('keydown', function (e) {
    try {
    if (e.which == 9) {
        var id_element = e.srcElement.className;
        if (id_element == "evendocument") {
            if (e.srcElement.value !== "") {
                document.getElementById("Button_Asigana_datos_validacion_edicion_manual").click();
            }
           
        }
    }
    }
    catch (err) {
        alert(err.message + " Funcion event");
    }
})
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