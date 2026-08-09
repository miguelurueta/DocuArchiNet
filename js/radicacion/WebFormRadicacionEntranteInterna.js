$(document).ready(function () {

    
    $.fn.cligred = function () {

        //****************************************VALIDACION DESTINATARIOS EXTERNOS*********************************************************************
        $('#data_grid tr[id]').click(function () {
            try {

                $('#data_grid tr[id]').css({ "background": "White", "color": "Black" });
                $(this).css({ "background-color": "#E7EDF5", "color": "Black" });
                var fer = $(this).attr("id");
                $('#hdnEmailID').val(fer);
                //Mantiene inactivo el boton editar de la gestion de destinatarios externos
                var estadoedi = document.getElementById('#Hiddenestadoedicion');
               
            }
            catch (err) {
                alert(err.message + " Funcion clik");
            }
        });
        //Mantiene el documents seleccionado en el datagrid de consulta del destinatario externo validación
        $('#data_grid tr[id=' + $('#hdnEmailID').val() + ']').css({ "background-color": "LightSkyBlue", "color": "Black" });
        $('#data_grid tr[id]').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });
        
        //****************************************VALIDACION RADICACION**********************************************************************************
        //FUNCION ACTIVA SELECCION CLIK EN EL DATAGREDVIEW DE VALIDACION RADICACION
        $('#GridView_val_radicacion tr[id]').click(function () {
            $('#GridView_val_radicacion tr[id]').css({ "background": "White", "color": "Black" });
            $(this).css({ "background-color": "#E7EDF5", "color": "Black" });
            var fer = $(this).attr("id");
            $('#hdnEmailID_VAL').val(fer);

        });


        //ASIGNA EL CURSOR DE SELECCION CUANDO PASA EL CURSOR EN EL DATAGREDVIEW DE VALIDACION RADICACION
        $('#GridView_val_radicacion tr[id]').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });
        //INICIA INTERFACE POPUP VALIDACION RADICADOS
        var tempo = document.getElementById("idente_chekbi_actyive");
        if (tempo === null) {
            $("#GridView_val_radicacion th:nth-child(1)").append(" <input id='idente_chekbi_actyive' type='checkbox' name='activa_deativa_chek' onchange=desactiva_ch_data_grid('idente_chekbi_actyive') class='mmmjjjkkkuuu'  />");
        }

    
        //********************************DESTINATARIO INTERNO*********************************************************************************************
        //FUNCION QUE CONTROLA EL CLIK SOBRE EL GREVIEW DE DESTINATARIO INTERNO
        $('#data_grid_auxiliar_lista tr[id]').click(function () {
            $('#data_grid_auxiliar_lista tr[id]').css({ "background-color": "transparent", "color": "Black" });
            $(this).css({ "background-color": "LightSkyBlue", "color": "Black" });
            var fer = $(this).attr("id");
            $('#Hidden_auxiliar_id').val(fer);

        });

        //MANTIENE EL ID SELECCIONADO DEL DESTINATARIO INTERNO
        $('#data_grid_auxiliar_lista tr[id=' + $('#Hidden_auxiliar_id').val() + ']').css({ "background-color": "LightSkyBlue", "color": "Black" });

        //OCULTA EL CAMPO ID DEL DESTINATARIO INTERNO
        $("#data_grid_auxiliar_lista td:nth-child(1), #data_grid_auxiliar_lista th:nth-child(1)").hide();
        //ASIGNA EL CURSOR DE SELECCION CUANDO PASA EL CURSOR EN EL DATAGREDVIEW DEL DESTINATARIO INTERNO
        $('#data_grid_auxiliar_lista tr[id]').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });

        //INICIA INTERFACE POPUP DESTINATARIO INTERNO
        //service_usuarios_gestion_text("RE_USUARIO-DESTINATARIO-VARCHAR")
        auto_resize_radicacion();
        auto_zise_popup_internos();
        auto_zise_popup_usuarios_externos();
        auto_zise_popup_trace_grafic();
        mueve_scroll_data_gred('data_grid_auxiliar_lista', 'Hidden_auxiliar_id');
        
        //******************************************************FIN******************************************************************************************
      
        
    }

    $(document).on('keypress', function (e) {
        var id_element = e.target.id;
        var id_element_spli = id_element.split("-")

        if (id_element_spli.length > 2) {
            if (id_element_spli[2] == "INT" || id_element_spli[2] == "DATE") {
                if (e.charCode < 48 || e.charCode > 57) return false;
            }
        }
    });


});
$(window).on("load", function () {
    var elment = document.getElementsByClassName("da_event_captive");
    if (elment) {
        for (var i = 0; i < elment.length; i++) {
            elment[i].addEventListener("click", event_click, false);
        }
    }
    window.addEventListener("resize", rezize_event);
    toke_ini('tokenize-callable-demo1');
    $('.tokenize-callable-demo1').on('tokenize:tokens:added', function (e, value, text) {
        document.getElementById("Hidden_005_sel_dest").value = value;

    });
    $('.tokenize-callable-demo1').on('tokenize:tokens:remove', function (e, value) {
        document.getElementById("Hidden_005_sel_dest").value = "0";

    });
    ShowModalPopup("ModalPopupExtender_mensaje_personalizado_backgroundElement", "Panel_mensaje_personalizado", 100001);
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
        auto_resize_radicacion();
        auto_zise_popup_usuarios_externos();
        auto_zise_popup_internos();
        auto_zise_popup_trace_grafic();
    } catch (ex) {
        alert(ex.message + " Función rezize_event")
    }
}

function toke_ini(name_token) {
    try {
        $('.' + name_token).tokenize2({
            placeholder: "digita nombre usuario o cargo",
            tokensMaxItems: 1,
            tokensAllowCustom: true,
            zIndexMargin: 10001,
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
                        try {
                            object.trigger('tokenize:dropdown:fill', [$items]);

                        }
                        catch (ex) { alert(ex + " Funcion toke_ini"); }
                    }
                });

            }

        });
    } catch (ex) {
        alert(ex.message);
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
function retorna_check_radicados_gred() {
    try {
        var x = document.getElementsByClassName("jjjjjjjjjjj");
        var ref_hode = document.getElementById("Hidden_selecion_radicado");
        ref_hode.value = "";
        for (i = 0; i < x.length; i++) {
            var z = x[i].firstChild;
            if (z.checked == true) {
                if (x[i].parentNode.parentNode.attributes.length > 0) {
                    var valrad = x[i].parentNode.parentNode.attributes.getNamedItem("id").value;
                    if (ref_hode.value == "") {
                        ref_hode.value = valrad;
                    }
                    else {
                        ref_hode.value = ref_hode.value + "-" + valrad;
                    }
                } else {
                    var valrad = x[i].parentNode.parentNode.parentNode.attributes.getNamedItem("id").value;
                    if (ref_hode.value == "") {
                        ref_hode.value = valrad;
                    }
                    else {
                        ref_hode.value = ref_hode.value + "-" + valrad;
                    }
                }
            }

        }

    }
    catch (err) {
        alert(err.message + " Funcion retorna_check_radicados_gred");
    }
}
function auto_zise_popup_trace_grafic() {
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

        $('#Paneltraza_grafica').css("height", (espacio_iframe) + "px");
        $('#div_content_trace_grafic').css("height", (espacio_iframe) + "px");
        $('#Iframetraza_grafica_').css("height", (espacio_iframe) + "px");
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_trace_grafic");
    }

}
//FUNCION ACTIVA Y DEACTIVA LOS CAMPOS CHEKEADOS EN UNA TABLA
function desactiva_ch_data_grid(idente_chekbi_actyive) {
    try {
        var e = $("#" + idente_chekbi_actyive);
        if ($(e).is(':checked')) {
            var x = document.getElementsByClassName("jjjjjjjjjjj");
            for (i = 0; i < x.length; i++) {
                var z = x[i].firstChild;
                z.checked = false;

            }

        }
        else {

            var x = document.getElementsByClassName("jjjjjjjjjjj");
            for (i = 0; i < x.length; i++) {
                var z = x[i].firstChild;
                z.checked = true;

            }


        }
    }
    catch (err) {
        alert(err.message + " Funcion desactiva_ch_data_grid");
    }
}

function activa_buton_asignar_dest_iterno(idente_chekbi_actyive) {


}
function activa_dest_externo() {
    try {

        $('#Button_inicia_selecion_validacion').click();
    }
    catch (err) {
        alert(err.message + " Funcion activa_dest_externo");
    }
}
//ACTIVA VENTANA POPUP DESTINATARION INTERNO
function activa_ventana_auxiliar_dest_iterno() {
    try {
        var t = $('#Area_Destinatario_Cor')[0].value;
        if ($('#Area_Destinatario_Cor')[0].value != "SELECCIONE" && $('#Area_Destinatario_Cor')[0].value != "") {

            $('#Button_abrir_auxiliar_destinatarios_internos_popup').click();
        }
    }
    catch (err) {
        alert(err.message + " Funcion activa_ventana_auxiliar_dest_iterno");
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

        $('#Contenido_validacion_plantilla').css("zIndex", 100030);
        $('#Panel_valiacion_plantilla').css("zIndex", 100030);
        document.getElementById("Hidden_height").value = espacio_iframe - 20;
        document.getElementById("Hidden_width").value = with_frame;
        //$('#Contenido_validacion_plantilla').css("position", "fixed")
        $(document).ready(bodyResize);
        $(window).resize(bodyResize);
        function bodyResize() {
            $('#Panel_valiacion_plantilla').css("width", (with_frame) + "px");
            $('#Panel_valiacion_plantilla').css("height", (espacio_iframe - 5) + "px");
            $('#Iframe_validacion_plantilla_').css("height", (espacio_iframe - 5) + "px");
        }
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_plantilla_validacion");
    }
}
function GetChar(event) {
    try {
        var chCode = ('charCode' in event) ? event.charCode : event.keyCode;
        if (chCode == 13) {

        }
    }
    catch (err) {
        alert(err.message + " Funcion GetChar");
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
//FUNCION CONFIGURA POPUP VENTANA GESTION DE EXPEDIENTE
function tamano_ventana_expediente() {
    try {
        $("#Hiddenheigpaginapopup").val(($("#Panel_expdiente_popup").height()));
        //$("#Iframe_expdiente_popup_").attr("src", "../gestion/WebFormGaGestionExpediente.aspx")
        //alert("ojo");
    }
    catch (err) {
        alert(err.message + " Funcion tamano_ventana_expediente");
    }
}
function onDataShown(sender, args) {
    try {
        sender._popupBehavior._element.style.zIndex = 1000001;
    }
    catch (err) {
        alert(err.message + " Funcion onDataShown");
    }
}
function llenardepartamento() {
    try {
        var drowplist = document.getElementById("PAIS");
        var idsel = document.getElementById("Hiddenselecionpais");
        if (drowplist.selectedIndex != -1) {
            idsel.value = drowplist.options[drowplist.selectedIndex].text;
            var boton = document.getElementById("Buttonllenardepartamento");
            var idsel2 = document.getElementById("Hiddenseleciondepartamento");
            idsel2.value = "";
            boton.click();
        }
    }
    catch (err) {
        alert(err.message + " Funcion llenardepartamento");
    }
}
function llenarciudad() {
    try {
        var drowplist = document.getElementById("DEPARTEMENTO");
        var idsel = document.getElementById("Hiddenseleciondepartamento");
        if (drowplist.selectedIndex != -1) {
            idsel.value = drowplist.options[drowplist.selectedIndex].text;
            var boton = document.getElementById("Buttonllenarciudad");
            boton.click();
        }
    }
    catch (err) {
        alert(err.message + " Funcion llenarciudad");
    }
}

function seleccionmuicipio() {
    try {
        var drowplist = document.getElementById("MUNICIPIO");
        var idsel = document.getElementById("Hiddenmunicipio");
        if (drowplist.selectedIndex != -1) {
            idsel.value = drowplist.options[drowplist.selectedIndex].text;
        }
    }
    catch (err) {
        alert(err.message + " Funcion seleccionmuicipio");
    }
}

function llenardestinatario() {
    try {
        var drowplist = document.getElementById("Area_Destinatario_Cor");
        var idsel = document.getElementById("Hiddenareagestion");
        if (drowplist.selectedIndex != -1) {
            idsel.value = drowplist.options[drowplist.selectedIndex].text;
            var boton = document.getElementById("Buttonllenardestinatario");
            boton.click();
        }
    }
    catch (err) {
        alert(err.message + " Funcion llenardestinatario");
    }
}
function seleccionardestinatario() {
    try {
        var drowplist = document.getElementById("Destinatario_Cor");
        var idsel = document.getElementById("Hiddendestinatario");
        if (drowplist.selectedIndex != -1) {
            idsel.value = drowplist.options[drowplist.selectedIndex].text;
            var boton = document.getElementById("Button_ra_destinatario");
            boton.click();
        }
    }
    catch (err) {
        alert(err.message + " Funcion seleccionardestinatario");
    }
}
function seleccionardestinatario_evento() {
    try {
        var drowplist = document.getElementById("Destinatario_Cor");
        var idsel = document.getElementById("Hiddendestinatario");
        if (drowplist.selectedIndex != -1) {
            idsel.value = drowplist.options[drowplist.selectedIndex].text;
            //var boton = document.getElementById("Button_ra_destinatario");
            //boton.click();
        }
    }
    catch (err) {
        alert(err.message + " Funcion seleccionardestinatario_evento");
    }
}

function asignar_validacion() {
    try {
        var boton = document.getElementById("Buttonrefasignar");
        //boton.click();
    }
    catch (err) {
        alert(err.message + " Funcion asignar_validacion");
    }
}
function asignar_fecha_vence_tramite() {
    try {
        var boton = document.getElementById("Buttontramitevence");
        var drowplist = document.getElementById("RE_Descripcion_Documento");
        var idsel = document.getElementById("Hiddentramiteseleccion");
        if (drowplist.selectedIndex != -1) {
            document.getElementById("Hidden_radi_inter").value = drowplist.options[drowplist.selectedIndex].value;
            idsel.value = drowplist.options[drowplist.selectedIndex].text;
            boton.click();
        }
    }
    catch (err) {
        alert(err.message + " Funcion asignar_validacion");
    }
}
function asignar_usuarios_wf_tramite() {
    try {
        var boton = document.getElementById("Button_llena_wf_flujo");
        var drowplist = document.getElementById("RE_actividad_asignacion_trabajo_flujo");
        var idsel = document.getElementById("Hidden_id_activividad");
        if (drowplist.selectedIndex != -1) {
            idsel.value = drowplist.options[drowplist.selectedIndex].value;
            boton.click();
        }
    }
    catch (err) {
        alert(err.message + " Funcion asignar_usuarios_wf_tramite");
    }
}
function asignar_actividades_flujo() {
    try {
        var boton = document.getElementById("Button_llena_actividad_flujo");
        var drowplist = document.getElementById("RE_flujo_trabajo");
        var idsel = document.getElementById("Hidden_id_flujo");
        if (drowplist.selectedIndex != -1) {
            idsel.value = drowplist.options[drowplist.selectedIndex].value;
            boton.click();
        }
    }
    catch (err) {
        alert(err.message + " Funcion asignar_actividades_flujo");
    }
}
function selecion_usuario_wf_grupo() {
    try {
        var drowplist = document.getElementById("RE_asignacion_asignacion_trabajo_flujo");
        var idsel = document.getElementById("Hidden_id_user_wf");
        if (drowplist.selectedIndex != -1) {
            idsel.value = drowplist.options[drowplist.selectedIndex].value;

        }
    }
    catch (err) {
        alert(err.message + " Funcion selecion_usuario_wf_grupo");
    }
}
function asignar_tipo_flujo() {
    try {
        var drowplist = document.getElementById("RE_flujo_trabajo");
        var idsel = document.getElementById("Hidden_nom_flu");
        if (drowplist.selectedIndex != -1) {
            document.getElementById("Hidden_id_flu").value = drowplist.options[drowplist.selectedIndex].value;
            idsel.value = drowplist.options[drowplist.selectedIndex].text;
            
        }
    }
    catch (err) {
        alert(err.message + " Funcion asignar_tipo_flujo");
    }
}

//Funcion filtra campos enteros
function key_pres_entero(semilla) {
    try {
        valtexbox = document.getElementById(semilla);
        if (e.charCode < 48 || e.charCode > 57) return false;
    }
    catch (err) {
        alert(err.message + " Funcion key_pres_entero");
    }
}

function xd() {

    if (document.getElementById("hdnEmailID").value == "-1" && document.getElementById("data_grid").rows != undefined) {
        
    }

}
function xdlimpiar(datgrid, hdnemail) {
    try {

        if (document.getElementById(hdnemail).value == "-1" && document.getElementById(datgrid).rows != undefined) {
            var _dat_grid = document.getElementById(hdnemail);

        }
    }
    catch (err) {
        alert(err.message + " Funcion xdlimpiar");
    }

}
function ConfirmMensajeEliminar(mensaje) {
    try {
        var x = 1;
        document.getElementById("HiddenPROMP").value = x;
        var t = document.getElementById("hdnEmailID").value;
        if (t != -1) {
            var r = confirm(mensaje);
            if (r == true) {
                x = "0";
            }
            else {
                x = "1";
            }
            document.getElementById("HiddenPROMP").value = x;
        }
    }
    catch (err) {
        alert(err.message + " Funcion ConfirmMensajeEliminar");
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
function xd2() {
    //var t = document.getElementById("hdnEmailID").value();
    //alert(t);
    try {
        var saber = document.getElementById("data_grid");
        if (saber == null) {
            document.getElementById("hdnEmailID").value = "-1"
        }
        if (document.getElementById("hdnEmailID").value != "-1" && saber != null) {
            for (var i = 2; i < document.getElementById("data_grid").rows.length; i++) {
                var id = document.getElementById("hdnEmailID").value;
                if (id == document.getElementById("data_grid").rows[i].cells[0].innerText) {
                    for (var j = 0; j < document.getElementById("data_grid").rows[i].cells.length; j++) {
                        var rtr = document.getElementById("data_grid").rows[i].cells[j].style.color = "White";
                        var rtr = document.getElementById("data_grid").rows[i].cells[j].style.background = "Black";

                    }
                }
            }
        }
    }
    catch (err) {
        alert(err.message + " Funcion  xd2");
    }
}

function xd4(datgrid, hdnemail) {
    try {
        var t = document.getElementById(hdnemail).value;
        //alert(t);
        if (document.getElementById(hdnemail).value != "-1" && document.getElementById(datgrid).rows != undefined) {
            for (var i = 2; i < document.getElementById(datgrid).rows.length; i++) {
                var id = document.getElementById(hdnemail).value;

                if (id == document.getElementById(datgrid).rows[i].cells[1].innerText) {
                    for (var j = 0; j < document.getElementById(datgrid).rows[i].cells.length; j++) {

                        var rtr = document.getElementById(datgrid).rows[i].cells[j].style.color = "White";
                        var rtr = document.getElementById(datgrid).rows[i].cells[j].style.background = "Black";

                    }
                }
            }
        }
    }
    catch (err) {
        alert(err.message + " Funcion  xd4");
    }
}
function xd5(datgrid, hdnemail) {
    //Lispia los campos seleccionados por la funcion clidred
    try {
        if (document.getElementById(hdnemail).value != "-1" && document.getElementById(datgrid).rows != undefined) {
            for (var i = 2; i < document.getElementById(datgrid).rows.length; i++) {
                for (var j = 0; j < document.getElementById(datgrid).rows[i].cells.length; j++) {
                    var rtr = document.getElementById(datgrid).rows[i].cells[j].style.color = "Black";
                    var rtr = document.getElementById(datgrid).rows[i].cells[j].style.background = "White";
                    //alert("ojo");
                }
            }
        }

    }
    catch (err) {
        alert(err.message + " Funcion  xd5");
    }
}

function xd3() {
    //Lispia los campos seleccionados por la funcion clidred
    try {
        if (document.getElementById("hdnEmailID").value != "-1") {
            for (var i = 2; i < document.getElementById("data_grid").rows.length; i++) {
                for (var j = 0; j < document.getElementById("data_grid").rows[i].cells.length; j++) {
                    var rtr = document.getElementById("data_grid").rows[i].cells[j].style.color = "Black";
                    var rtr = document.getElementById("data_grid").rows[i].cells[j].style.background = "White";
                    //alert("ojo");
                }
            }
        }
    }
    catch (err) {
        alert(err.message + " Funcion  xd5");
    }

}


function inactiva_chek() {
    //document.getElementById("hdnEmailID_VAL").value == "-1";
    //xd5("GridView_val_radicacion", "hdnEmailID_VAL");
}

//AUTO SIZE DE POPUP DESTINATARIOS EXTERNOS
function auto_zise() {
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
        $(document).ready(bodyResize);
        $(window).resize(bodyResize);
        function bodyResize() {
            $('#Panel_Val_Radicacion').css("height", (espacio_iframe - 45) + "px");
            $('#Contenido_val_radicacion').css("height", (espacio_iframe - 45) + "px");
            $('#contenido_titulo_val_radicacion').css("height", "5%");
            $('#contenido_consulta_val_radicacion').css("height", "30%");
            $('#_ValidacionConsulta_val_radicacion').css("height", "100%");
            $('#_Panelvalidacion_val_radicacion').css("height", "100%");
            $('#UpdatePanelContenido_val_radicacion').css("height", "100%");
            $('#contenido_datagrid_val_radicacion').css("height", "40%");
            $('#UpdatePanel_conenido_grid_val_radicacion').css("height", "100%");
            $('#Panel_gred_val_radicacion').css("height", "100%");
            $('#recontenido_limpiar').css("height", "1%");
            $('#Contenido_botones_tipo_radicado').css("height", "10%");

            $('#Panel_Val_Radicacion').css("width", (with_frame - 45) + "px");
            $('#Contenido_val_radicacion').css("width", "100%");
            $('#contenido_titulo_val_radicacion').css("width", "99%");
            $('#contenido_datagrid_val_radicacion').css("width", (with_frame - 50) + "px");
            $('#UpdatePanel_conenido_grid_val_radicacion').css("width", (with_frame - 50) + "px");
            $('#UpdatePanelContenido_val_radicacion').css("width", (with_frame - 50) + "px");
            $('#Panel_gred_val_radicacion').css("width", (with_frame - 50) + "px");
            $('#recontenido_limpiar').css("width", "100%");
            $('#Contenido_botones_tipo_radicado').css("width", "99%");
            $('#_Panelvalidacion_val_radicacion').css("width", (with_frame - 50) + "px");
            $('#_ValidacionConsulta_val_radicacion').css("width", (with_frame - 50) + "px");
        }

        activa_ventan_validacion_radicados()
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise");
    }
}

//ACTIVA VENTANA POPOUP PREVALIDACION RADICADO
function activa_ventan_validacion_radicados() {
    
    $('#Button_Abrir_Val_Radicacion').click();
}
//CAPTURA FILTRADO DESTINATARIOS INTERNOS TECLA BORRAR
function consulta_documentos_busqueda_keycode(e, textarea) {

    if (e.keyCode == 8) {
        // if (textarea.value.length >= 0) {

        //filtro_gred_destinatarios_internos('Hidden_auxiliar_id', 'data_grid_auxiliar_lista', 'TextBoxcontenidobusqueda', 'CheckboxBusqueda', 'panel_data_grid_auxiliar_destinatarios_internos_popup', 'Contenido_auxiliar_destinatarios_internos_popup', 'panel_data_grid_auxiliar_destinatarios_internos_popup');
        //}


    }
}

//CAPTURA FILTRADO DESTINATARIOS INTERNOS TECLAS NORMALES
function consulta_documentos_busqueda_keypres(e, textarea) {
    var code = (e.keyCode ? e.keyCode : e.which);

}
//AUTO SIZE DESTINATARIOS INTERNOS
function auto_zise_popup_internos() {
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


       
        $('#Panel_auxiliar_destinatarios_internos_popup').css("height", (espacio_iframe - 40) + "px");
        $("#contedor_botones_auxiliar_destinatarios_internos_popup").css("height", ($("#Button_asignar_auxiliar_destinatarios_internos_popup").height() + 10) + "px");
        var gridwith = $('#panel_data_grid_auxiliar_destinatarios_internos_popup').width() - 5;
        var gridheihg = (espacio_iframe - 70) - $('#contedor_botones_auxiliar_destinatarios_internos_popup').height();
        $("#Contenido_auxiliar_destinatarios_internos_popup").css("height", (gridheihg - 10) + "px");
        //LLAMA PLUGIN FIJA HIDER O TITULOS
        gridheihg = gridheihg - 10;
        if ($('#data_grid_auxiliar_lista td').children.length > 0) {
            //$('#data_grid_auxiliar_lista').gridviewScroll({ width: gridwith, height: gridheihg });
        }
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_internos");
    }
}
//AUTO SIZE RADICACION
function auto_resize_radicacion() {
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

       
        $('#contenguia').css("height", (espacio_iframe - 5) + "px");
        /*var heigconetedor = $("#contenguia").height() - (($("#contenguia").height() * 93) / 100);
        //$("#PanelTitulo").css("height", (heigconetedor) + "px");
        heigconetedor = $("#contenguia").height() - (($("#contenguia").height() * 99) / 100);
        $("#separator_control_2").css("height", (heigconetedor) + "px");
        heigconetedor = $("#contenguia").height() - (($("#contenguia").height() * 90) / 100);
        $("#Panel_modo_radicado").css("height", (heigconetedor) + "px");
        heigconetedor = $("#contenguia").height() - (($("#contenguia").height() * 26) / 100);
        //$("#PanelRadicacion").css("height", (heigconetedor) + "px");
        heigconetedor = $("#contenguia").height() - (($("#contenguia").height() * 90) / 100);
        $("#Panelbotonesradcacion").css("height", (heigconetedor) + "px");*/
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_internos");
    }
}
//AUTO SIZE POPUP VALIDACION RADICADOS
function hiden_popup_resize_popup_validacion_radicados() {
    try {
        $("#Diupdate_val_radciacion").hide();
    }
    catch (err) {
        alert(err.message + " Funcion hiden_popup_resize_popup_validacion_radicados");
    }
}
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
        //var gridwith = $('#contenido_datagrid_val_radicacion').width();
        //var gridheihg = $('#contenido_datagrid_val_radicacion').height();


        $(document).ready(bodyResize);
        $(window).resize(bodyResize);
        function bodyResize() {
            $('#contenido_izquierdo_val_radicacion').css("height", (espacio_iframe - 10) + "px");
            $('#contenido_derecho_validacion_radicados').css("height", (espacio_iframe - 10) + "px");
            var heigconetedor = $("#contenido_derecho_validacion_radicados").height() - (($("#contenido_derecho_validacion_radicados").height() * 15) / 100);
            $("#contenido_datagrid_val_radicacion").css("height", (heigconetedor) + "px");
            $("#contenido_consulta_val_radicacion").css("height", (heigconetedor) + "px");
            $("#_Panelvalidacion_val_radicacion").css("height", (heigconetedor - 10) + "px");
            heigconetedor = $("#contenido_derecho_validacion_radicados").height() - (($("#contenido_derecho_validacion_radicados").height() * 89) / 100);
            $("#Contenido_botones_tipo_radicado").css("height", (heigconetedor) + "px");
            $("#contenido_botones_val_radicacion").css("height", (heigconetedor) + "px");
            heigconetedor = $("#contenido_derecho_validacion_radicados").height() - (($("#contenido_derecho_validacion_radicados").height() * 96) / 100);
            $("#contenido_titulo_campos_consulta").css("height", (heigconetedor) + "px");
            $("#contenido_titulo_val_radicacion").css("height", (heigconetedor) + "px");
            $("#Diupdate_val_radciacion").show();
        }
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_validacion_radicados");
    }
}
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
        alert(err.message + " Funcion plugin_grwedview");
    }
}
function auto_zise_popup_usuarios_externos() {
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

        $('#contenido_general').css("height", (espacio_iframe - 30) + "px");
        $('#contenido_general').css("width", (with_frame - 10) + "px");
        $('#Cosulta_valid').css("width", (with_frame - 15) + "px");
        var heigconetedor = $("#contenido_general").height() - (($("#contenido_general").height() * 60) / 100);
        $("#contenido_consulta").css("height", (heigconetedor) + "px");
        $("#_Panelvalidacion").css("height", (heigconetedor) + "px");
        heigconetedor = $("#contenido_general").height() - (($("#contenido_general").height() * 91) / 100);
        $("#contenido_titulo").css("height", (heigconetedor) + "px");
        heigconetedor = $("#contenido_general").height() - (($("#contenido_general").height() * 60) / 100);
        $("#contenido_datagrid").css("height", (heigconetedor) + "px");
        $("#Cosulta_valid").css("height", (heigconetedor) + "px");
        heigconetedor = $("#contenido_general").height() - (($("#contenido_general").height() * 91) / 100);
        $("#tolbalboton").css("height", (heigconetedor) + "px");

        /*$(document).ready(bodyResize);
        $(window).resize(bodyResize);
        function bodyResize() {
            $('#contenido_general').css("height", (espacio_iframe - 20) + "px");
            $('#contenido_general').css("width", (with_frame - 10) + "px");
            $('#Cosulta_valid').css("width", (with_frame - 15) + "px");
            var heigconetedor = $("#contenido_general").height() - (($("#contenido_general").height() * 60) / 100);
            $("#contenido_consulta").css("height", (heigconetedor) + "px");
            $("#_Panelvalidacion").css("height", (heigconetedor) + "px");
            heigconetedor = $("#contenido_general").height() - (($("#contenido_general").height() * 91) / 100);
            $("#contenido_titulo").css("height", (heigconetedor) + "px");
            heigconetedor = $("#contenido_general").height() - (($("#contenido_general").height() * 60) / 100);
            $("#contenido_datagrid").css("height", (heigconetedor) + "px");
            $("#Cosulta_valid").css("height", (heigconetedor) + "px");
            heigconetedor = $("#contenido_general").height() - (($("#contenido_general").height() * 91) / 100);
            $("#tolbalboton").css("height", (heigconetedor) + "px");
        }*/
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_usuarios_externos");
    }
}
//MUEVE EL SCCROL AL ID SELECCIONADO
function mueve_scroll_data_gred(data_grid, HiddenSeleccion) {
    try {
        if ($("#" + HiddenSeleccion).val() != "-1") {
            var scrollableDiv = $("#" + data_grid).parent();

            //limpia todos los seleccionados
            $("#" + data_grid + " tr[id]").css({ "background-color": "transparent", "color": "Black" });
            $("#" + data_grid + " tr[id=" + $("#" + HiddenSeleccion).val() + "]").css({ "background-color": "LightSkyBlue", "color": "Black" });
            $("#" + data_grid + " tr[id= " + $("#" + HiddenSeleccion).val() + "]").each(function () {
                $(scrollableDiv).scrollTop(70);
                $(scrollableDiv).scrollTop($(this).offset().top);
                return false;
            });


        }
    }
    catch (err) {
        alert(err.message + " Funcion mueve_scroll_data_gred");
    }
}

//FUNCION QUE PERMITE LA BUSQUEDA DE REGISTROS
function busqueda_gred_destinatarios_internos(HiddenSeleccion, data_grid, contenido_busqueda, CheckboxBusqueda) {
    try {
        $("#" + HiddenSeleccion).val("-1");
        var refgrid;
        var filtro;
        $("#" + data_grid + " tr[id]").css({ "background-color": "transparent", "color": "Black" });
        var s = $("#" + contenido_busqueda).val().toLowerCase();
        var grid = $("#" + data_grid);

        $("#" + data_grid + " tr:has(td)").each(function () {
            var scrollableDiv = grid.parent();

            $(this).children("td").each(function (idex) {

                var tempotd = $(this).text().toLowerCase()
                var check = document.getElementById(CheckboxBusqueda).checked;
                if (check == true) {

                    if (idex >= 0) {
                        if (s == tempotd) {
                            $(this).parent().css({ "background-color": "LightSkyBlue", "color": "green" });
                            $(scrollableDiv).scrollTop(70);
                            var id_ref = $(this).parent();
                            $(scrollableDiv).scrollTop($(id_ref).offset().top);

                        }
                    }
                }

                if (check == false) {
                    if (idex >= 0) {
                        var compare = tempotd;
                        var strcompre = compare.indexOf(s);
                        if (strcompre >= 0) {
                            $(this).parent().css({ "background-color": "LightSkyBlue", "color": "green" });
                            $(scrollableDiv).scrollTop(70);
                            var id_ref = $(this).parent();
                            $(scrollableDiv).scrollTop($(id_ref).offset().top);

                        }
                    }
                }


            })
        });
    }
    catch (err) {
        alert(err.message + " Funcion busqueda_gred_destinatarios_internos");
    }
}

function filtro_gred_destinatarios_internos(HiddenSeleccion, data_grid, contenido_busqueda, CheckboxBusqueda, prent_with, parent_heiht, paren_) {
    try {
        $("#" + HiddenSeleccion).val("-1");
        var refgrid;
        var filtro;
        var ito = 0;
        var confirma_hidem_fila = 0;
        var showtr;
        $("#" + data_grid + " tr[id]").css({ "background-color": "transparent", "color": "Black" });
        var s = $("#" + contenido_busqueda).val().toLowerCase();
        var grid = $("#" + data_grid);
        $("#" + data_grid + " tr:hidden").show();
        //$('#data_grid_auxiliar_listaHeader').hide();
        $("#" + data_grid + " tr:has(td)").each(function () {

            var refdif = $(this);
            var confirm = -1;
            $(this).children("td").each(function (idex) {

                var tempotd = $(this).text().toLowerCase()
                var check = document.getElementById(CheckboxBusqueda).checked;
                if (check == true) {

                    if (idex >= 0) {
                        if (s == tempotd) {
                            (this).parent().show();


                            confirm = 1;
                        } else {


                        }
                    }
                }

                if (check == false) {
                    if (idex >= 0) {
                        var compare = tempotd;
                        var strcompre = compare.indexOf(s);
                        if (strcompre >= 0) {
                            refdif.show();
                            confirm = 1;
                        } else {

                        }
                    }
                }


            })
            ito++;
            if (confirm == -1 && ito != 1) {
                refdif.hide();
                $("#" + data_grid).append(refdif.clone());
                refdif.remove();
            }
            if (confirm == -1 && ito == 1) {
                refdif.hide();
                $("#" + data_grid).append(refdif.clone());
                refdif.remove();
            }
        });
        var gridwith = $("#" + prent_with).width() - 5;
        var gridheihg = $("#" + parent_heiht).height();
        if (gridheihg == 0) {
            var heigconetedor = 420 - (((420 * 15) / 100) + 10);
            gridheihg = heigconetedor;
        }

        if (showtr != undefined) {
            showtr.show();
        }

        $("#" + paren_).append($("#" + data_grid).clone());
        $("#" + data_grid + "Wrapper").remove();
        //carga el data gredi con el plugin
        if ($("#" + data_grid + " td:visible").children.length > 0) {
            $("#" + data_grid).gridviewScroll({ width: gridwith, height: gridheihg });
        }


        //FUNCION QUE CONTROLA EL CLIK SOBRE EL GREVIEW DE DESTINATARIO INTERNO
        $("#" + data_grid + " tr[id]").click(function () {
            $("#" + data_grid + " tr[id]").css({ "background-color": "transparent", "color": "Black" });
            $(this).css({ "background-color": "LightSkyBlue", "color": "Black" });
            var fer = $(this).attr("id");
            $("#" + HiddenSeleccion).val(fer);

        });

        //ASIGNA EL CURSOR DE SELECCION CUANDO PASA EL CURSOR EN EL DATAGREDVIEW DEL DESTINATARIO INTERNO
        $("#" + data_grid + " tr[id]").mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });
    }
    catch (err) {
        alert(err.message + " Funcion filtro_gred_destinatarios_internos");
    }
}
$(document).on('keydown', function (e) {
    if (e.which == 9) {
        var id_element = e.target.id;
        var matr_id = id_element.split("-");
        if (matr_id.length < 2) {
            return;
        }
        var salidadato;
        if (matr_id[2] == "DATE" || matr_id[2] == "DATE_2") {
            var dato = e.target.value;


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
                e.target.value = salidadato;
            }

            if (numerocaracter == 10) {
                salidadato = Año_F + "-" + Mes_f + "-" + Dia_f;
                e.target.value = salidadato;
            }

        }
    }
});




