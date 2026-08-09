$(document).ready(function () {
    $.fn.inicio = function () {
        //****************************************VALIDACION RADICACION**********************************************************************************
        //FUNCION ACTIVA SELECCION CLIK EN EL DATAGREDVIEW DE VALIDACION RADICACION
        $('#GridView_val_radicacion tr[id]').click(function () {
            $('#GridView_val_radicacion tr[id]').css({ "background": "White", "color": "Black" });
            $(this).css({ "background-color": "#E7EDF5", "color": "Black" });
            var fer = $(this).attr("id");
            $('#hdnEmailID_VAL').val(fer);
            

        });
        $('#GridView_val_radicacion tr[id]').dblclick(function () {
            var fer = $(this).attr("id");
            if (fer !== "-1") {
                //window.document.getElementById("Button_ver_documento").click();
            }
        })

        //ASIGNA EL CURSOR DE SELECCION CUANDO PASA EL CURSOR EN EL DATAGREDVIEW DE VALIDACION RADICACION
        $('#GridView_val_radicacion tr[id]').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });
        //INICIA INTERFACE POPUP VALIDACION RADICADOS
        //var tempo = document.getElementById("idente_chekbi_actyive");
       // if (tempo === null) {
       //     $("#GridView_val_radicacion th:nth-child(1)").append(" <input id='idente_chekbi_actyive' type='checkbox' name='activa_deativa_chek' onchange=desactiva_ch_data_grid('idente_chekbi_actyive') class='dummychkstyle_'  />");
       // }

        auto_zise_popup_validacion_radicados();
        auto_size_popup_procesa_tramite();
        $(document).ready(bodyResize);
        $(window).resize(bodyResize);
        function bodyResize() {
            auto_zise_popup_validacion_radicados();
            auto_size_popup_procesa_tramite();
            plugin_grwedview();
            mueve_scroll_data_gred('GridView_val_radicacion', 'hdnEmailID_VAL');
        }
        
        //MUEVE SCROLL DATA GRED (VALIDACION DE RADICADOS SE REALIZA EN PAGUE REGUEST
        // mueve_scroll_data_gred('GridView_val_radicacion', 'hdnEmailID_VAL');
        //OCULTA LA CLAVE PRINCIPAL
        //$("#GridView_val_radicacion th:nth-child(1)").hide();
        //$("#GridView_val_radicacion td:nth-child(1)").hide();
        //$("#GridView_val_radicacion th:nth-child(2)").hide();
        //$("#GridView_val_radicacion td:nth-child(2)").hide();

        //******************************************FIN****************************************************************************************************
    }


})

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
function colum_name_index(index_colum) {

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
function fnExcelReport() {
    try {
        var tab_text = "<table border='2px'><tr bgcolor='#87AFC6'>";
        var textRange; var j = 0;
        tab = document.getElementById('GridView_val_radicacion'); // id of table
        for (j = 0 ; j < tab.rows.length ; j++) {
            var tdth;
            var reftabtex = "";
            if (j == 0) {
                tdth = tab.rows[j].getElementsByTagName('th');
            } else {
                tdth = tab.rows[j].getElementsByTagName('td');
            }
            for (k = 0; k < tdth.length ; k++) {
                var suitch = 1;
                var redisp = tdth[k].style.display;
                if (redisp == "none") {
                    suitch = -1;

                }
                if (k == 0) {
                    if (j == 0) {
                       /* var t = tdth[k].getElementsByClassName("dummychkstyle_");
                        if (t[0].className == "dummychkstyle_") {
                            suitch = -1;

                        }*/
                    } else {
                        /*var t = tdth[k].getElementsByClassName("dummychkstyle");
                        if (t[0].className == "dummychkstyle") {
                            suitch = -1;

                        }*/
                    }
                }
                if (suitch == 1) {
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
            chid = chid - 1;
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
        var heigconetedor;
        $('#Contenedorderecho').css("height", (espacio_iframe - 40) + "px");
        heigconetedor = document.getElementById("Button_descargar_guia").clientHeight + $("#inferior_bajo_boton").height() + 5;
        $("#Contenido_botones_tipo_radicado").css("height", (heigconetedor) + "px");
        heigconetedor = document.getElementById("TextBox_busqueda").clientHeight + 5;
        $("#contenido_titulo_val_radicacion").css("height", (heigconetedor) + "px");
        heigconetedor = $('#Contenedorderecho').height() - ($("#Contenido_botones_tipo_radicado").height() + ($("#contenido_titulo_val_radicacion").height()));
        $("#contenido_datagrid_val_radicacion").css("height", (heigconetedor + 10) + "px");
        $('#Contentizquierdo').css("height", (espacio_iframe - 40) + "px");
        heigconetedor = $("#Button_consulta_pendientes_procesar").height() + 10;
        $("#contenido_controles_buton_consulta").css("height", (heigconetedor) + "px");
        heigconetedor = document.getElementById("TextBox_busqueda").clientHeight + 5;
        $("#contenido_titulo_controles_consulta").css("height", (heigconetedor) + "px");
        heigconetedor = $('#Contentizquierdo').height() - ($("#contenido_controles_buton_consulta").height() + ($("#contenido_titulo_controles_consulta").height()));
        $("#contenido_controles_consulta").css("height", (heigconetedor + 10) + "px");
        $("#_Panelvalidacion_val_radicacion").css("height", (heigconetedor + 10) + "px");
        /*$('#Contenedorderecho').css("height", (espacio_iframe - 40) + "px");
        var heigconetedor = $("#Contenedorderecho").height() - (($("#Contenedorderecho").height() * 9) / 100);
        $("#contenido_datagrid_val_radicacion").css("height", (heigconetedor) + "px");
        heigconetedor = $("#Contenedorderecho").height() - (($("#Contenedorderecho").height() * 92) / 100);
        $("#Contenido_botones_tipo_radicado").css("height", (heigconetedor) + "px");
        heigconetedor = $("#Contenedorderecho").height() - (($("#Contenedorderecho").height() * 96) / 100);
        $("#contenido_titulo_val_radicacion").css("height", (heigconetedor) + "px");
        $('#Contentizquierdo').css("height", (espacio_iframe - 40) + "px");
        heigconetedor = $("#Contentizquierdo").height() - (($("#Contentizquierdo").height() * 9) / 100);
        $("#contenido_controles_consulta").css("height", (heigconetedor) + "px");
        $("#_Panelvalidacion_val_radicacion").css("height", (heigconetedor) + "px");
        heigconetedor = $("#Contentizquierdo").height() - (($("#Contentizquierdo").height() * 92) / 100);
        $("#contenido_controles_buton_consulta").css("height", (heigconetedor) + "px");
        heigconetedor = $("#Contentizquierdo").height() - (($("#Contentizquierdo").height() * 96) / 100);
        $("#contenido_titulo_controles_consulta").css("height", (heigconetedor) + "px");*/
    }
    catch (ex) {
        alert(ex.message + " function java auto_zise_popup_validacion_radicados")
    }
}
function auto_size_popup_procesa_tramite() {
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
    catch (err) {
        alert(err.message + " funcion busqueda_gred_por_enviar " + err.message);
    }
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