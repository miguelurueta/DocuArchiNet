$(document).ready(function () {
    $.fn.inicio = function () {
        ini_tempo();
    }
})
var result_fuciones;
var contador_registro_gabi;
var response_sevice_java="YES";
var id_sel_elemnt_;
var elem = document.getElementById("myBar");
var elment_progres = document.getElementById("myProgress_porcent");
var elment_conta = document.getElementById("myProgress_contador");
var lastSelectedRow;
var trs;
var ars_sele=[];
function ini_tempo() {
    //****************************************VALIDACION RADICACION**********************************************************************************
    //FUNCION ACTIVA SELECCION CLIK EN EL DATAGREDVIEW DE VALIDACION RADICACION
    $('#GridView_val_radicacion tr[id]').click(function (e) {
      
           var fer = $(this).attr("id");
           $('#hdnEmailID_VAL').val(fer);
              
    });
   
    $('#GridView_val_radicacion tr[id]').dblclick(function (e) {

        if ($('#hdnEmailID_VAL').val() != "-1") {
            document.getElementById("Button_visor_emergente").click();
            return false;

        }

    });
    //ASIGNA EL CURSOR DE SELECCION CUANDO PASA EL CURSOR EN EL DATAGREDVIEW DE VALIDACION RADICACION
    $('#GridView_val_radicacion tr[id]').mouseover(function () {
        $(this).css({ cursor: "hand", cursor: "pointer" });
    });

    //INICIA INTERFACE POPUP VALIDACION RADICADOS
    var tempo = document.getElementById("idente_chekbi_actyive");
    if (tempo === null) {
        //$("#GridView_val_radicacion th:nth-child(1)").append(" <input id='idente_chekbi_actyive' type='checkbox' name='activa_deativa_chek' onchange=desactiva_ch_data_grid('idente_chekbi_actyive') class='mmmjjjkkkuuu'  />");
        
    }
    GetLista_consulta_gabinetes('TextBox_buequeda_general');
    //auto_zise_consulta();
    auto_zise_tipo_documento();
    ajuta_tabla_consulta();
    $('#contenido_datagrid_val_radicacion').contextMenu('context-menu-2', {

        'Ver documentos': {
            click: function (element) {  
                if ($('#hdnEmailID_VAL').val() != "-1") {                               
                    document.getElementById("Button_visor_emergente").click();
                    return false;   
                }

            }
        },
        'Seleccionar todos': {
            click: function (element) {
                inicializa_tr();
                selectRowsBetweenIndexes();

            }
        },
        'Salir del Menu': {
            click: function (element) { }
        }
    });
    //******************************************FIN****************************************************************************************************
}
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
        auto_zise_descarga_documento();
        auto_zise_ubicacion_topografica();
        GetLista_consulta_gabinetes('TextBox_buequeda_general');
        auto_zise_consulta();
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
        auto_zise_consulta();
        auto_zise_popup_visor_externo();
        auto_zise_tipo_documento();
        ajuta_tabla_consulta();
        auto_zise_panel_indice_documento();
        auto_zise_descarga_documento();
        auto_zise_ubicacion_topografica();
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
function inicializa_tr() {
    trs = document.getElementById('GridView_val_radicacion').tBodies[0].getElementsByTagName('tr');
}
function RowClick(currenttr, lock) {
    inicializa_tr();
    if (window.event.ctrlKey) {
        toggleRow(currenttr);
        var exist;
        exist = verifi_existencia_array(currenttr.id);
        if (exist == "NO") {
            ars_sele.push(currenttr.id);
        } else {
            var pos = ars_sele.indexOf(currenttr.id);
            ars_sele.splice(pos,1);
        }
    }

    if (window.event.button === 0) {
        if (!window.event.ctrlKey && !window.event.shiftKey) {
            ars_sele = [];
            clearAll();
            toggleRow(currenttr);
            ars_sele.push(currenttr.id)
        }

        if (window.event.shiftKey) {
            selectRowsBetweenIndexes()
           
        }
    }
}
function toggleRow(row) {
    row.className = row.className == 'GridviewScrollItem_line_cort_select' ? '' : 'GridviewScrollItem_line_cort_select';
    lastSelectedRow = row;
}
function selectRowsBetweenIndexes() {
    ars_sele = [];
    for (var i = 0; i <= trs.length - 1; i++) {
        if (trs[i].className == "GridviewScrollItem_line_cort" || trs[i].className == "GridviewScrollItem_line_cort_select") {
            trs[i].className = 'GridviewScrollItem_line_cort_select';
            var exist;
            exist = verifi_existencia_array(trs[i].id);
            if (exist == "NO") {
                ars_sele.push(trs[i].id);
            }
        }
       
    }

}
function Elimina_registro(event) {
    if (ars_sele.length == 0) {
        alert("Debe seleccionar los registros a eliminar")
        return false;
    }
    var res = confirm("Desea eliminar (" + ars_sele.length + ") registro(s) selecconidado(s)");
    if (res == true) {
       
    } else {
        return false;
    }
    if (ars_sele.length == 1) {
        elimina_regitro_service(ars_sele[0]);
        if (response_sevice_java !== "YES") {
            alert(response_sevice_java);
        } else {
            result_fuciones = eliminar_fila_data_gred_service("GridView_val_radicacion", ars_sele[0]);
            if (result_fuciones !== "YES") {
                response_sevice_java = result_fuciones;
            } else {
                ars_sele = [];
                document.getElementById("Hidden_nureg").value = (document.getElementById("Hidden_nureg").value - 1);
                document.getElementById("titulo_label_val_radicacion").innerHTML = "Se encontro " + document.getElementById("Hidden_nureg").value + " registro(s) en el gabinete " + document.getElementById("Hidden_nugab_sele").value;
            }

        }
    } else {
        document.getElementById("Label_progres_bar").innerHTML = "Eliminado registros";
        document.getElementById("Button_pogres_show").click();
        move(0, ars_sele.length, "elimina_regitro_service");
        event.preventDefault();
    }
   
}
function elimina_regitro_service(id_imagen_) {
    var obj = {};
    var jsonData = JSON.stringify(obj);
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "../webservice/WebServiceDocuarchi.asmx/Get_elimina_registro_service",
        data: "{'id_imagen':'" + id_imagen_ + "'}",
        dataType: "json",
        success: function (data) {
            //response(data.d);
            if (data.d) {
                response_sevice_java = data.d;
            } else {
                response_sevice_java = "YES";
            }       
        },
        error: function (result) {
            alert("Error......" + result);
            event.preventDefault();
        }
    });
}
function eliminar_fila_data_gred_service(gred, id) {
    try {

        $("#" + gred + " tr[id=" + id + "]").remove();
        return 'YES';
    }
    catch (err) {
        return err.message;
    }

}
function verifi_existencia_array(id_sleccion) {
    if (ars_sele.length == 0) {
        return "NO";
    }
    for (var i = 0; i <= ars_sele.length; i++) {
        if (ars_sele[i] == id_sleccion) {
            return "YES";
            break;
        }
    }
    return "NO";
}
function clearAll() {
    for (var i = 0; i < trs.length; i++) {
        if (trs[i].className == "GridviewScrollItem_line_cort" || trs[i].className == "GridviewScrollItem_line_cort_select") {
            trs[i].className = 'GridviewScrollItem_line_cort';
        }
        
    }
}
function myStopFunction(event) {
    //
    var con = confirm("Desea cancelar el proceso");
    if (con == true) {
        elem.style.width = 0 + '%';
        elment_progres.innerHTML = 0 + '%';
        clearInterval(id_sel_elemnt_);
        Restaura_array();
        document.getElementById('Button_cerrar_pro_gres_bar').click();
        event.preventDefault();
    }
   
}
function Restaura_array() {
    var copi_ars_sele = [];
    for (var i = 0; i <= ars_sele.length - 1; i++) {
        if (ars_sele[i] !== "") {
            copi_ars_sele.push(ars_sele[i])
           
        }
    }
    ars_sele = copi_ars_sele.slice(0, copi_ars_sele.length);
}
function move(numero_ini, numero_fin,fuction_name) {
    var width_ = numero_ini;
    elem = document.getElementById("myBar");
    elment_progres = document.getElementById("myProgress_porcent");
    elment_conta = document.getElementById("myProgress_contador");
    id_sel_elemnt_ = setInterval(frame, 100);
    function frame() {
        if (width_ >= numero_fin) {
            clearInterval(id_sel_elemnt_);
            ars_sele = [];
            document.getElementById('Button_cerrar_pro_gres_bar').click();
        } else {
            if (fuction_name == "elimina_regitro_service") {
                elimina_regitro_service(ars_sele[width_]);
                if (response_sevice_java !== "YES") {
                    alert(response_sevice_java);
                    clearInterval(id_sel_elemnt_);
                    document.getElementById('Button_cerrar_pro_gres_bar').click();
                } else {                 
                        result_fuciones = eliminar_fila_data_gred_service("GridView_val_radicacion", ars_sele[width_]);
                        if (result_fuciones !== "YES") {
                            response_sevice_java = result_fuciones;
                        } else {
                            ars_sele[width_] = "";
                            document.getElementById("Hidden_nureg").value = (document.getElementById("Hidden_nureg").value - 1);
                            document.getElementById("titulo_label_val_radicacion").innerHTML = "Se encontro " + document.getElementById("Hidden_nureg").value + " registro(s) en el gabinete " + document.getElementById("Hidden_nugab_sele").value;
                           
                        }
                    
                }
            }
            width_++;
            var porcent = (100 * width_) / numero_fin;
            porcent = Math.round(porcent)
            elem.style.width = porcent + '%';
            elment_progres.innerHTML = porcent + '% ';
            elment_conta.innerHTML = width_ + ' de ' + numero_fin;
        }
    }
}
function limpia_sleccion() {
    ars_sele = [];
}
function clik_oculta() {
    if ($("#area_consulta").is(":hidden")) {
        $("#area_resultado_documentos").css("width", "69%");
        $("#area_consulta").css("width", "30%");
        $("#area_consulta").show();
        document.getElementById("oculta_resultado").title = "Oculta campos de busqueda";
        return true;
    } else {
        $("#area_consulta").hide();
        $("#area_consulta").css("width", "1%");
        $("#area_resultado_documentos").css("width", "99%");
        document.getElementById("oculta_resultado").title = "Muestra campos de busqueda";
        return true;
    }
}
function GetLista_consulta_gabinetes(name_texbox) {
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
                    url: "../webservice/WebServiceDocuarchi.asmx/GetLista_consulta_gabinetes",
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
                //var terms = split(this.value);
                // remove the current input
                //terms.pop();
                // add the selected item
                //terms.push(ui.item.value);
                // add placeholder to get the comma-and-space at the end
                //terms.push("");
                this.value = ui.item.value;
                document.getElementById("TextBox_buequeda_general").value = ui.item.label;
                document.getElementById("Button_consulta_general").click();
                return false;
            }

                , minLength: 3, max: 10, scroll: true
        });
}
function prevent(event, element) {
    try {

        var fer = $(element).attr("id");
        var tip_event = $(element).attr("tip_event");
        if (tip_event == "visualiza_documento") {
            $('#hdnEmailID').val(fer);
            $('#hdnEmailID_VAL').val(fer);
            document.getElementById("Button_visor_emergente").click();
        }
        if (tip_event == "indice_documento") {
            $('#hdnEmailID').val(fer);
            $('#hdnEmailID_VAL').val(fer);
            document.getElementById("ImageButtonindice").click();
        }
        if (tip_event == "descarga_documento") {
            $('#hdnEmailID').val(fer);
            $('#hdnEmailID_VAL').val(fer);
            document.getElementById("ImageButton_exportar_archivo").click();
        }

        event.preventDefault();
        element.focus();
    }
    catch (err) {
        alert(err.message + " Funcion prevent");
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
function acti_busq_general_archivo_boton(e, sender) {
    try {

        document.getElementById("Button_consulta_general").click();
        e.preventDefault();

    } catch (err) {
        alert(err.message + " funcion acti_busq_general_archivo_boton " + err.message);
    }
}
function acti_busq_general_restore(e, sender) {
    try {

        document.getElementById("Button_consulta").click();
        e.preventDefault();

    } catch (err) {
        alert(err.message + " funcion acti_busq_general_restore " + err.message);
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

function retorna_colum_mtriz(hiden_name) {
    try {
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
    catch (err) {
        alert(err.message + " Funcion retorna_colum_mtriz");
}
}
function fixGridView(tableEl) {
    //Funcion que habilita el tbody en la tabla del gredview
    try {
    var jTbl = $(tableEl);
    if (jTbl.find("tbody>tr>th").length > 0) {
        jTbl.find("tbody").before("<thead><tr></tr></thead>");
        jTbl.find("thead tr").append(jTbl.find("th"));
        jTbl.find("tbody tr:first").remove();
       
    }
}
    catch (err) {
        alert(err.message + " Funcion fixGridView");
}
};

function actualiza_gre_campos_dinamicos() {
    try {
        var hidendcampos = document.getElementById("Hidden_campos_dinamicos_edita").value;
        var hidencamposaleas = document.getElementById("hidden_campos_dinamicos_aleas").value;
        var hidenvalores = document.getElementById("hidden_valore_campos").value;
        var spli_campos = hidendcampos.split("|");   
        var valores = hidenvalores.split("|||||");
        $("#GridView_val_radicacion tr[id=" + $("#hdnEmailID_VAL").val() + "]").each(function () {
            var idex = -1;          
            for (i = 0; i <= (spli_campos.length - 1) ; i++) {
                var control = document.getElementById(spli_campos[i]);
                var name = spli_campos[i];
                if (valores[i] != undefined) {
                    idex = colum_index(name);
                    if (idex != -1) {
                        if (valores[i] == "") {
                            var sas = $(this)[0].cells[idex];
                            var nodetext = document.getElementById("ocultop");
                            var trfirst = $('#GridView_val_radicacion tr:first').next();
                            if (sas.childElementCount == 0) {
                                $(this)[0].cells[idex].innerText = "\u00a0";
                                nodetext.innerText = "\u00a0";                              
                            }
                            if (sas.childElementCount >= 1) {
                                sas.firstChild.innerHTML = "&nbsp;";
                                nodetext.innerText = "\u00a0";                             
                            }                            
                        }
                        if (valores[i] !== "") {                        
                            var trfirst = $('#GridView_val_radicacion tr:first').next();
                            var sas = $(this)[0].cells[idex];       
                            if (sas.childElementCount <= 0) {
                                //var clinet_widt_old = sas.firstChild.clientWidth;
                                //var div_element = document.createElement("div");
                                //var p_element = document.createElement("p");
                                //p_element.innerHTML = valores[i];
                                //div_element.appendChild(p_element);
                                //$(this)[0].appendChild(div_element);
                                $(this)[0].cells[idex].innerText = valores[i];
                                //var clinet_widt_new = p_element.clientWidth;
                                //verifcar que la fila uno tenga childs
                               // if (($(this)[0].cells[idex].clientWidth - 10) > trfirst[0].cells[idex].firstChild.clientWidth) {
                               //     trfirst[0].cells[idex].firstChild.style.width = $(this)[0].cells[idex].clientWidth + "px";
                               //     var x2 = $('#GridView_val_radicacionCopy th');
                               //     x2[idex].firstChild.style.width = ($(this)[0].cells[idex].clientWidth-10) + "px";
                               //     x2[idex].clientWidth = ($(this)[0].cells[idex].clientWidth-10);
                                //}
                                //$(this)[0].removeChild(div_element);
                            }
                            //Opcion para actualizar la primera fila de la tabla que se le agrega un div, cuado trae mas de un elemento
                            /*if (sas.childElementCount >= 1) {
                                var clinet_widt_old = sas.firstChild.clientWidth;
                                var div_element = document.createElement("div");
                                var p_element = document.createElement("p");
                                p_element.innerHTML = valores[i];
                                div_element.appendChild(p_element);
                                sas.firstChild.innerHTML = valores[i];
                                sas.appendChild(div_element);
                                var clinet_widt_new = p_element.clientWidth;
                                if (clinet_widt_new > trfirst[0].cells[idex].firstChild.clientWidth) {
                                    if (trfirst[0].cells[idex].firstChild.childElementCount > 0) {
                                        trfirst[0].cells[idex].firstChild[0].style.width = clinet_widt_new + "px";
                                    }
                                    else {
                                        trfirst[0].cells[idex].firstChild.style.width = clinet_widt_new + "px";                                    
                                    }
                                    //var x2 = $('#GridView_val_radicacionCopy th');
                                    //x2[idex].firstChild.style.width = clinet_widt_new + "px";
                                    //x2[idex].clientWidth = clinet_widt_new;
                                }
                                //sas.removeChild(div_element);
                            }*/
                            
                        }
                                             
                    }
                }
            }
        })
       
    }
    catch (err) {
        alert(err.message + " Funcion actualiza_gre_campos_dinamicos");
    }
}
function mueve_scroll_data_gred_datos(data_grid, HiddenSeleccion) {
    try {
    if ($("#" + HiddenSeleccion).val() != "-1") {
        var scrollableDiv = $("#" + data_grid).parent();
        //limpia todos los seleccionados
        $("#" + data_grid + " tr[id]").css({ "background-color": "transparent", "color": "Black" });
        $("#" + data_grid + " tr[id=" + $("#" + HiddenSeleccion).val() + "]").css({ "background-color": "LightSkyBlue", "color": "Black" });
        $("#" + data_grid + " tr[id= " + $("#" + HiddenSeleccion).val() + "]").each(function () {
            //$(scrollableDiv).scrollTop(70);
            $(scrollableDiv).scrollTop(document.getElementById("Hidden_scroll_gred").value);
            return true;
        });
    }
}
    catch (err) {
        alert(err.message + " Funcion mueve_scroll_data_gred_datos");
}
}

function ajuta_tabla_consulta() {
    try {
        
        var heigts = document.getElementById("Panel_campos_consuta").style.width;
        //window.document.getElementsByClassName("date_indice_text_box").style.width = heigts + "px";
        var x = document.getElementsByClassName("date_indice_text_box");
        var i;
        for (i = 0; i < x.length; i++) {
            x[i].style.width = heigts ;
        }
    }
    catch (err) {
        alert(err.message + " Funcion ajuta_tabla_consulta");
    }
}
function setear_fecha_elaboracion() {
    try {
        var conf = confirm("Desea limpiar la fecha de elaboración");
        if (conf == false) {
            return false;
        }

        if (document.getElementById("FECHAELABORACION") != undefined) {
            document.getElementById("FECHAELABORACION").value = "";
        }
    }
    catch (err) {
        alert(err.message + " Funcion setear_fecha_elaboracion");
    }
}
function setear_expdiente() {
    try {
        var conf = confirm("Desea limpiar datos del expediente");
        if (conf == false) {
            return false;
        }
        document.getElementById("Hidden_id_expediente").value = "0";
        document.getElementById("Hidden_id_tipo_expediente").value = "0";
        if (document.getElementById("EXPEDIENTE") != undefined) {
            document.getElementById("EXPEDIENTE").value = "";
        }

    }
    catch (err) {
        alert(err.message + " Funcion setear_expdiente");
    }
}
function setear_clase_documento() {
    try {
        var conf = confirm("Desea limpiar la clase de documento");
        if (conf == false) {
            return false;
        }
        document.getElementById("Hidden_id_tipo").value = "0";
        if (document.getElementById("CLASEDOCUMENTO") != undefined) {
            document.getElementById("CLASEDOCUMENTO").value = "";
        }
    }
    catch (err) {
        alert(err.message + " Funcion setear_clase_documento");
    }
}
function setear_trd_documento() {
    try {
        var conf = confirm("Desea limpiar datos tabla de retención");
        if (conf == false) {
            return false;
        }
        document.getElementById("Hidden_id_serie").value = "0";
        document.getElementById("Hidden_id_sub_serie").value = "0";
        document.getElementById("Hidden_id_documento").value = "0";
        document.getElementById("Hidden_id_area").value = "0";
        if (document.getElementById("NOMBRESERIE") != undefined) {
            document.getElementById("NOMBRESERIE").value = "";
        }
        if (document.getElementById("NOMBRESUBSERIE") != undefined) {
            document.getElementById("NOMBRESUBSERIE").value = "";
        }

        if (document.getElementById("TIPODOCUMENTO") != undefined) {
            document.getElementById("TIPODOCUMENTO").value = "";
        }
    }
    catch (err) {
        alert(err.message + " Funcion setear_trd_documento");
    }
}

//funcion inicializa lo valores para ordenacion de tabla por consulta
function limpia_order() {
    ars_sele = [];
    document.getElementById("Hidden_colum_value_order").value=0;
    document.getElementById("Hidden_colum_value_order_ult").value=0;
    document.getElementById("Hidden_colum_name_order").value="";
    if (document.getElementById("Hidden_estado_update").value==0) {
        document.getElementById("Hidden_estado_update").value=1;
    } else {
        document.getElementById("Hidden_estado_update").value=0;
    }
}
function mueve_scroll_value(valor, panel) {
    try {
       
        $("#" + panel).scrollTop(document.getElementById("Hidden_scroll").value);
       
    }
    catch (err) {
        alert(err.message + " Funcion mueve_scroll_value");
    }
}
function mueve_scroll_data_gred(buton, panel) {
    try {
        $("#" + panel).scrollTop(70);
        $("#" + panel).scrollTop(($("#" + buton).offset().top));
    }
    catch (err) {
        alert(err.message + " Funcion mueve_scroll_data_gred");
    }
}
    function valor_tipo_documento() {
        try {

            if ($('#ComboBoxtipo')[0].value !== "SELECCIONE" && $('#ComboBoxtipo')[0].value !== "") {
                document.getElementById("Hidden_valor_seleccion").value = $('#ComboBoxtipo')[0].value;
                $('#Button_lista_ayuda_tipo').click();
            } else {
                document.getElementById("Hidden_valor_seleccion").value = "";
                document.getElementById("TextBoxinfotipo").value = "";
            }

        }
        catch (err) {
            alert(err.message + " funcion valor_tipo_documento " + err.message);
        }
    }
    function asignar_clase_documento() {
        try {
            if (document.getElementById("Hidden_id_tipo").value == "0") {
                alert("Debe seleccionar la clase de documento");
                return false;
            }
            var text_clase_documento = document.getElementById("CLASEDOCUMENTO");
            if (text_clase_documento == undefined) {
                alert("Imposible encontrar el control CLASEDOCUMENTO");
                return false;
            }
            text_clase_documento.value = document.getElementById("Hidden_valor_seleccion").value
            document.getElementById("Buttoncerrar_tipo_popup").click();
            
        }
        catch (err) {
            alert(err.message + " funcion asignar_clase_documento " + err.message);
        }


    }
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

function auto_zise_tipo_documento() {
    var witht = document.getElementById("contenido_controles_consulta").style.width;
    var heigts = document.getElementById("contenido_controles_consulta").style.height;
    $('#Panel_tipo_popup').css("height", heigts + "px");
    $('#Panel_tipo_popup').css("width", witht + "px");
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

function GetChar(event) {
    try {
        var chCode = ('charCode' in event) ? event.charCode : event.keyCode;
        if (chCode == 13) {

        }
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_editar_radicados");
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
function inactiva_chek() {
    //document.getElementById("hdnEmailID_VAL").value == "-1";
    //xd5("GridView_val_radicacion", "hdnEmailID_VAL");
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
        /*$('#Panel_visor_externo').css("height", (espacio_iframe - 40) + "px");
        $('#Cotenedorpendiente_visor_externo').css("height", (espacio_iframe - 40) + "px");
        $('#Iframe_visor_externo').css("height", (espacio_iframe - 70) + "px");*/
        //corrige el posicionamiento horizontal del modalpopuextender, la propiedad width del panel debe estar en auto
        //var widtth_procent_left_rigth = (with_frame - document.getElementById("Panel_agregar_expediente_carpeta").clientHeight) / 2;
        //$('#Panel_agregar_expediente_carpeta').css("left", (Math.round(widtth_procent_left_rigth)) + "px");
        var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_visor_externo').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_Panel_visor_externo').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#Cotenedorpendiente_visor_externo').css("height", (document.getElementById("modal_content_Panel_visor_externo").clientHeight - (document.getElementById("Cabecerapendiente_visor_externo").clientHeight + 1)) + "px");
        //Para los modal que contiene gred
        $('#Iframe_visor_externo_').css("height", (document.getElementById("Cotenedorpendiente_visor_externo").clientHeight - 1) + "px");
    } catch (ex) {
        alert("Error funcion auto_zise_popup_visor_externo " + ex.message)
    }
}
function auto_zise_panel_indice_documento() {
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
        $('#Panel_indice').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        //$('#modal_content_user_rel').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#content_Panel_indice').css("height", (document.getElementById("Panel_indice").clientHeight - (document.getElementById("divcabecer2__indice").clientHeight )) + "px");
        //Para los modal que contiene gred
        $('#ifrm_indice_').css("height", (document.getElementById("content_Panel_indice").clientHeight - 1) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_panel_indice_documento " + err.message);
    }
}
function auto_zise_descarga_documento() {
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
        $('#Panel_descarga_anexo_respuesta').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        //$('#modal_content_user_rel').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#contenido_procesa_descarga_anexo_respuesta').css("height", (document.getElementById("Panel_descarga_anexo_respuesta").clientHeight - (document.getElementById("div_driver_anexo").clientHeight + 20)) + "px");
        //Para los modal que contiene gred
        $('#ifimpre_descarga_anexo_respuesta_').css("height", (document.getElementById("contenido_procesa_descarga_anexo_respuesta").clientHeight - 1) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_descarga_documento " + err.message);
    }
}
function auto_zise_descarga_documento() {
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
        $('#Panel_descarga_anexo_respuesta').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        //$('#modal_content_user_rel').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#contenido_procesa_descarga_anexo_respuesta').css("height", (document.getElementById("Panel_descarga_anexo_respuesta").clientHeight - (document.getElementById("div_driver_anexo").clientHeight + 20)) + "px");
        //Para los modal que contiene gred
        $('#ifimpre_descarga_anexo_respuesta_').css("height", (document.getElementById("contenido_procesa_descarga_anexo_respuesta").clientHeight - 1) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_descarga_documento " + err.message);
    }
}
function auto_zise_ubicacion_topografica() {
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
        $('#Panel_ubicacion_toponimica_expediente_popup').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        //$('#modal_content_user_rel').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#Contenido_ubicacion_toponimica_expediente').css("height", (document.getElementById("Panel_ubicacion_toponimica_expediente_popup").clientHeight - (document.getElementById("divcabecer_ubicacion_toponimica_expediente_popup").clientHeight + document.getElementById("contendor_botones_unidad_u_b_t").clientHeight)) + "px");
        //Para los modal que contiene gred
        $('#div_treview_archivo_u_b_t').css("height", (document.getElementById("Contenido_ubicacion_toponimica_expediente").clientHeight - 1) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_ubicacion_topografica " + err.message);
    }
}
function auto_zise_consulta() {
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
   
        $('#Contentizquierdo').css("height", ((espacio_iframe - 1) - 1) + "px");
        $('#sidebar_').css("height", ((espacio_iframe - 1) - 1) + "px");
        $("#contenido_controles_consulta").css("height", (document.getElementById("Contentizquierdo").clientHeight) - (document.getElementById('contenido_titulo_controles_consulta').clientHeight + document.getElementById('contenido_controles_buton_consulta').clientHeight ) + "px");
        $("#Panel_campos_consuta").css("height", (document.getElementById("Contentizquierdo").clientHeight) - (document.getElementById('contenido_titulo_controles_consulta').clientHeight + document.getElementById('contenido_controles_buton_consulta').clientHeight ) + "px");
        $('#Contenedorderecho').css("height", ((espacio_iframe - 1) - 1) + "px");
        $("#contenido_datagrid_val_radicacion").css("height", (document.getElementById("Contenedorderecho").clientHeight - (document.getElementById('contenido_titulo_val_radicacion').clientHeight + document.getElementById('contenido_controles_buton_consulta').clientHeight)) + "px");
        $("#Panel_principal").css("height", (document.getElementById("contenido_datagrid_val_radicacion").clientHeight ) + "px");
    } catch (ex) { alert("Funcion auto_zise_consulta " + ex.message); }
   
}


function onDataShown(sender, args) {
    sender._popupBehavior._element.style.zIndex = 1000001;

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


//Retorna el idex de una columna en una tabla
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

$(document).on('keydown', function (e) {
    if (e.which == 9) {
        var id_element = e.srcElement.className;

        var salidadato;
        if (id_element == "date_indice" && e.srcElement.value != "") {
            var dato = e.srcElement.value;


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
                e.srcElement.value = salidadato;
            }

            if (numerocaracter == 10) {
                salidadato = Año_F + "/" + Mes_f + "/" + Dia_f;
                e.srcElement.value = salidadato;
            }

        }
    }
});
