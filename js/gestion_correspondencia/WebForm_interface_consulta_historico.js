$(document).ready(function () {
    $.fn.inicio = function () {
        auto_zise_popup_lista_tramites(1, 1);
        auto_zise_popup_detalle_respuesta();
        auto_zise_popup_detalle_transacciones();
        auto_zise_popup_detalle_trazabilidad();
        service_posibles_datos_tramites();
        auto_zise_popup_visor_externo();
        
        $('#data_grid_listado_solicitudes tr[id]').click(function () {
            $('#data_grid_listado_solicitudes tr[id]').css({ "background": "White" });
            $(this).css({ "background-color": "#e8e8f7" });
            var fer = $(this).attr("id");
            $('#hdnEmailID').val(fer);
        });

        $('#data_grid_listado_solicitudes tr[id]').dblclick(function () {
            var fer = $(this).attr("id");
            $('#hdnEmailID').val(fer);
            document.getElementById("Hidden_tipo_visor").value = "VISOR WORKFLOW";
            document.getElementById("Button_visor_emergente").click();
            return false;
        });
        $('#data_grid_listado_solicitudes tr[id]').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });
       
    }


});
$(window).on("load", function () {
    window.addEventListener("resize", rezize_event);
    inicia_data_piker_boot('TextBox_fecha_fin_final_tramite');
    inicia_data_piker_boot('TextBox_fecha_ini_final_tramite');
    inicia_data_piker_boot('TextBox_fecha_ini_asigna');
    inicia_data_piker_boot('TextBox_fecha_fin_asigna');
    ShowModalPopup("ModalPopupExtender_mensaje_personalizado_backgroundElement", "Panel_mensaje_personalizado", 100001);

});
function inicia_data_piker_boot(name){
    try {
        $('#' + name).datepicker({
            uiLibrary: 'bootstrap4',
            header: true,  format: 'yyyy-mm-dd', iconsLibrary: 'fontawesome'
        });
        $('.gj-picker').css('zIndex', 99999999);
        var element = document.getElementById(name);
        element.classList.remove("form-control");
    } catch (ex) {alert(ex.mensaje + " funcion  inicia_data_piker_boot")}

}
function rezize_event() {
    try {
        auto_zise_popup_lista_tramites(1, 1);
        auto_zise_popup_detalle_respuesta();
        auto_zise_popup_detalle_transacciones();
        auto_zise_popup_detalle_trazabilidad();
        service_posibles_datos_tramites();
        auto_zise_popup_visor_externo();
        
    } catch (ex) {
        alert(ex.message + " Función rezize_event")
    }
}
function ShowModalPopup(modalPopupId_,name_panel, zIndex) {
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
    function actualiza_selecion() {
        try {
            document.getElementById("Label_anunciado_filtro").innerHTML = "Todas";
            document.getElementById("Hidden_lik_service_boton").value = "1";
        }
        catch (err) {
            alert(err.message + " Funcion actualiza_selecion");
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

            $('#Panel_visor_externo').css("height", (espacio_iframe - 20) + "px");
            var heig = document.getElementById("Panel_visor_externo").clientHeight - document.getElementById("Cabecerapendiente_visor_externo").clientHeight;
            $('#Cotenedorpendiente_visor_externo').css("height", (heig) + "px");
            $('#Iframe_visor_externo__').css("height", (heig) + "px");
        }
        catch (err) {
            alert(err.message + " Funcion actualiza_gre_campos_dinamicos");
        }
    }
    function auto_zise_popup_detalle_trazabilidad() {
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

        $('#Panel_trazabilidad').css("height", (espacio_iframe - 40) + "px");
        var heig = document.getElementById("Panel_trazabilidad").clientHeight - document.getElementById("Cabecerapendiente_trazabilidad").clientHeight;
        $('#Cotenedorpendiente_trazabilidad').css("height", (heig) + "px");
        $('#Iframe_trazabilidad_').css("height", (heig) + "px");

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
            $('#Panel_transacciones').css("height", (espacio_iframe - 1) + "px");
            var heig = document.getElementById("Panel_transacciones").clientHeight - document.getElementById("Cabecerapendiente_transacciones").clientHeight;
            $('#Cotenedorpendiente_transacciones').css("height", (heig) + "px");
            $('#Iframe_transacciones_historial_').css("height", (heig - 3) + "px");
        }
        catch (err) {
            alert(err.message + " Funcion auto_zise_popup_detalle_transacciones");
        }
    }
    function auto_zise_popup_detalle_respuesta() {
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

        $('#Panel_detalle_respuesta').css("height", (espacio_iframe - 1) + "px");
        var heig = document.getElementById("Panel_detalle_respuesta").clientHeight - document.getElementById("Cabecerapendiente_detalle_respuesta").clientHeight;
        $('#Cotenedorpendiente_detalle_respuesta').css("height", (heig) + "px");
        $('#Iframe_visor_externo__').css("height", (heig - 5 ) + "px");

    }
    function Decrementa_contador_tramites() {
        try {
            if (document.getElementById("Hidden_content").value !== 0) {
                document.getElementById("Hidden_content").value = document.getElementById("Hidden_content").value - 1;
            }
        }
        catch (err) {
            alert(err.message + " Decrementa_contador_tramites");
        }
    }
    function Actualiza_cantidad_barr_estado() {
        try {

            document.getElementById("Label_titulo_listado_solicitudes").innerHTML = "Se encontraron " + document.getElementById("Hidden_content").value + " registro(s) ";
        }
        catch (err) {
            alert(err.message + " Actualiza_cantidad_barr_estado");
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
                        url: "../webservice/WebServiceWorkflow.asmx/GetLista_usuarios_workflow_",
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
                    this.value = terms.join("");
                    return false;
                }

                , minLength: 3, max: 10, scroll: true
            });
    }
    function service_posibles_datos_tramites() {
        function split(val) {
            return val.split(/,\s*/);
        }
        function extractLast(term) {
            return split(term).pop();
        }
        $("#auto_complex")
            .on("keydown", function (event) {
                if (event.keyCode === $.ui.keyCode.TAB &&
                    $(this).autocomplete("instance").menu.active) {
                    event.preventDefault();
                }
            })
            .autocomplete({
                source: function (request, response) {
                
                    $.ajax({
                        url: "../webservice/WebServiceWorkflow.asmx/GetPosiblesDatos_Tramites_historico",
                        data: "{'DName':'" + document.getElementById('auto_complex').value + "'}",
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
                    //this.value = terms.join("");
                    document.getElementById("auto_complex").value = ui.item.label;
                    document.getElementById("ImageButton_buscar").click();
                    return false;
                }

                , minLength: 3, max: 10, scroll: true
            });
    }
    function prevent(event, element) {
        try {
            var fer = $(element).attr("idd");
            var tip_event = $(element).attr("tip_event");
            if (tip_event == "documento_solic_tramite") {
                $('#hdnEmailID').val(fer);
                document.getElementById("Hidden_tipo_visor").value = "VISOR WORKFLOW";
                document.getElementById("Button_visor_emergente").click();
            }
            if (tip_event == "trasa_solic_tramite") {
                $('#hdnEmailID').val(fer);
                document.getElementById("Button_traza_solic").click();

            }
            if (tip_event == "detalle_solic_tramite") {
                $('#hdnEmailID').val(fer);
                document.getElementById("Button_deta_solic").click();

            }
            if (tip_event == "transac_solic_tramite") {
                $('#hdnEmailID').val(fer);
                document.getElementById("Button_lo_solic").click();
            }
       
            event.preventDefault();
            element.focus();
        }
        catch (err) {
            alert(err.message + " Funcion prevent");
        }
    }
    function auto_zise_popup_respuesta() {
        try{
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
            $('#Iframe_respuesta_radicado_').css("height", (espacio_iframe - 60) + "px");

        }
        catch (err) {
            alert(err.message + " Funcion auto_zise_popup_respuesta");
        }

    }
    function activa_boton_cerrar() {
        document.getElementById("Button_valida_Cerrar_respuesta_radicado").click();
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
            //alert(err.message + " Funcion eliminar_fila_data_gred");
        }

    }
    function actualiza_estado_tramite() {
        try {
            actualiza_gre_campo("data_grid_listado_solicitudes", document.getElementById('hdnEmailID').value, document.getElementById('Hidden_estado_tramite').value, "ESTADO")
        }
        catch (err) {
            alert(err.message + " funcion actualiza_estado_tramite " + err.message);
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
                        //var nodetext = document.getElementById("ocultop");
                        //var trfirst = $('#' + nombre_grid + ' tr:first').next();
                        if (sas.childElementCount == 0) {
                            $(this)[0].cells[idex].innerText = "\u00a0";
                            //nodetext.innerText = "\u00a0";
                        }
                        if (sas.childElementCount >= 1) {
                            sas.firstChild.innerHTML = "&nbsp;";
                            //nodetext.innerText = "\u00a0";
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

    function auto_zise_popup_lista_tramites(value_lista_general, value_lista_usuario) {
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
            $('#div_contendor_principal').css("height", (espacio_iframe - 10) + "px");
            $('#div_contendor_principal').css("wdth", (with_frame - 20) + "px");
            $('#div_contendor_filtro_listado').css("height", ((document.getElementById("div_filtro__fil").clientHeight + 5)) + "px");
            var total = document.getElementById("div_contendor_filtro_listado").clientHeight + document.getElementById("navar_barra").clientHeight + document.getElementById("contenido_titulo_listado_solicitudes").clientHeight;
            var gridheihg_ = ((espacio_iframe - 10) - total);
            $('#content_grid').css("height", (gridheihg_ ) + "px");
            $('#Panel_principal').css("height", (gridheihg_ ) + "px");

        }
        catch (err) {
            alert(err.message + " funcion auto_zise_popup_lista_tramites " + err.message);
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
   
    function auto_zise_popup_solicitud_aprobacion() {
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

        $('#Panel_solicitud_aprobacion').css("height", (espacio_iframe - 40) + "px");
        $('#contenido_procesa_solicitud_aprobacion').css("height", (espacio_iframe - 40) + "px");
        $('#Iframe_solicitud_aprobacion').css("height", (espacio_iframe - 40) + "px");
        
    }
    function activa_export_lista(hiden_name, nombre_gred) {
        try {
            var hiden = document.getElementById(hiden_name);
            var nombre_gred;
            var x = $('#' + nombre_gred + ' th');
            var txt = "";
            var i;
            for (i = 1; i < x.length; i++) {
                txt = txt + x[i].innerText.toUpperCase() + "|";
            }

            hiden.value = txt;
            document.getElementById("Button_export_lista_event").click();
            return txt;
        }
        catch (err) {
            alert(err.message + " Funcion activa_export_lista");
        }
    }
    $(document).on('keydown', function (e) {
        if (e.which == 9) {
            
            var salidadato;
            if (e.target.id == "TextBox_fecha_ini_asigna" || e.target.id == "TextBox_fecha_fin_asigna" || e.target.id == "TextBox_fecha_ini_final_tramite" || e.target.id == "TextBox_fecha_fin_final_tramite") {
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