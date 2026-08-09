$(document).ready(function () {
    $.fn.inicio = function () {
        //****************************************VALIDACION RADICACION**********************************************************************************
        //FUNCION ACTIVA SELECCION CLIK EN EL DATAGREDVIEW DE VALIDACION RADICACION
        if (document.getElementById("Hidden_seleccion").value == "YES") {
            document.getElementById("Hidden_seleccion").value = "";
            auto_zise_popup_pendinetes();
        }
        $('#GridViewlista tr[id]').click(function () {
            $('#GridViewlista tr[id]').css({ "background": "White", "color": "Black" });
            $(this).css({ "background-color": "#E7EDF5", "color": "Black" });
            var fer = $(this).attr("id");
            $('#hdnEmailID_VAL').val(fer);
            $('#Hidden_id').val(fer);

        });
        $('#GridViewlista tr[id]').dblclick(function () {
           
            if ($('#Hidden_id').val() != "-1") {
                var split = $('#Hidden_id').val().split("-");
                if (split.length > 0) {
                    document.getElementById("Hidden_id_tarea_sel").value = split[1];
                    document.getElementById("Hidden_tipo_visor").value = "VISOR WORKFLOW";
                    document.getElementById("Button_visor_emergente").click();
                    return false;
                }
                

            }

        });
        //FUNCION NEVIAR TAREA A USUARIO 
        $(window.parent).resize(bodyResize);
        $(window).resize(bodyResize);
        function bodyResize() {
            auto_zise_popup_lista_tareas("1");
            auto_zise_popup_lista_tareas_ruta("1");
            auto_zise_popup_pendinetes();
            auto_zise_popup_visor_externo();
            auto_zise_popup_envia_usuario_grupo();
        }
        $('#data_grid_actividad tr[id]').click(function () {
            //$('#data_grid_actividad tr[id]').css({ "background": "White", "color": "Black" });
           // $(this).css({ "background-color": "#E7EDF5", "color": "Black" });
        });
        $('#data_grid tr[id]').click(function () {
            //$('#data_grid tr[id]').css({ "background": "White", "color": "Black" });
            //$(this).css({ "background-color": "#E7EDF5", "color": "Black" });
        });
        //ASIGNA EL CURSOR DE SELECCION CUANDO PASA EL CURSOR EN EL DATAGREDVIEW DE VALIDACION RADICACION
        $('#GridViewlista tr[id]').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });
        //INICIA INTERFACE POPUP VALIDACION RADICADOS
        var tempo = document.getElementById("idente_chekbi_actyive");
        if (tempo === null) {
            //$("#GridViewlista th:nth-child(1)").append(" <input id='idente_chekbi_actyive' type='checkbox' name='activa_deativa_chek' onchange=desactiva_ch_data_grid('idente_chekbi_actyive') class='mmmjjjkkkuuu'  />");
        }
        //document.getElementById("Button_actualiza_lista_pendiente").click();
       
        
        if ($("#" + 'GridViewlista' + " tr:visible").length > 0) {
            //mueve_scroll_data_gred('GridViewlista', 'Hidden_id');
        }
        auto_zise_popup_visor_externo();
        //******************************************FIN****************************************************************************************************
        $('#Lista').contextMenu('context-menu-1', {
            'Ver Documentos': {
                click: function (element) {  // element is the jquery obj clicked on when context menu launched
                    var RowID = $('#Hidden_id').val();
                    if (RowID == "-1") {
                        alert("Por favor seleccione el documento");
                    }
                    else {
                        if ($('#Hidden_id').val() != "-1" && $('#Hidden_id').val() != "0" && $('#Hidden_id').val() != "") {
                            var split = $('#Hidden_id').val().split("-");
                            if (split.length > 0) {
                                document.getElementById("Hidden_id_tarea_sel").value = split[1];
                                document.getElementById("Hidden_tipo_visor").value = "VISOR WORKFLOW";
                                document.getElementById("Button_visor_emergente").click();
                                return false;
                            }

                        }
                    }
                }
            },
            'Salir del Menu': {
                click: function (element) { }
            }
        })

        $('.alterna_image').hover(function () {
            //$(this).animate({ opacity: 0 });
            var boton = $(this);
            if (boton[0].alt == "Actualizar lista") {
                var sr = boton[0].src;
                sr = sr.replace("actualizar.jpg", "actualizar-1.jpg");
                boton[0].src = sr;
            }
            if (boton[0].alt == "Seleccionar de la lista") {
                var sr = boton[0].src;
                sr = sr.replace("seleccionar.jpg", "seleccionar-2.jpg");
                boton[0].src = sr;
            }
            if (boton[0].alt == "Renviar a Usuario") {
                var sr = boton[0].src;
                sr = sr.replace("envia_usuario.jpg", "envia_usuario-2.jpg");
                boton[0].src = sr;
            }
            if (boton[0].alt == "Renviar a Grupo") {
                var sr = boton[0].src;
                sr = sr.replace("enviar_actividad.jpg", "enviar_actividad-2.jpg");
                boton[0].src = sr;
            }
            if (boton[0].alt == "Sin tareas pendientes") {
                var sr = boton[0].src;
                sr = sr.replace("pendiente.jpg", "pendiente2.jpg");
                boton[0].src = sr;
            }
            if (boton[0].alt == "tareas pendientes") {
                var sr = boton[0].src;
                sr = sr.replace("pendiente.jpg", "pendiente2.jpg");
                boton[0].src = sr;
            }
            if (boton[0].alt == "Anotacion a tarea actual") {
                var sr = boton[0].src;
                sr = sr.replace("notas.jpg", "notas-2.jpg");
                boton[0].src = sr;
            }
            if (boton[0].alt == "Enviar tarea a") {
                var sr = boton[0].src;
                sr = sr.replace("terminar.jpg", "terminar-2.jpg");
                boton[0].src = sr;
            }
            if (boton[0].alt == "El sistema decide el envío por usted") {
                var sr = boton[0].src;
                sr = sr.replace("autoterminar.jpg", "autoterminar-2.jpg");
                boton[0].src = sr;
            }
        }, function () {
            //$(this).animate({ opacity: 1 });
            var boton = $(this);
            if (boton[0].alt == "Actualizar lista") {
                var sr = boton[0].src;
                sr = sr.replace("actualizar-1.jpg", "actualizar.jpg");
                boton[0].src = sr;
            }
            if (boton[0].alt == "Seleccionar de la lista") {
                var sr = boton[0].src;
                sr = sr.replace("seleccionar-2.jpg", "seleccionar.jpg");
                boton[0].src = sr;
            }
            if (boton[0].alt == "Renviar a Usuario") {
                var sr = boton[0].src;
                sr = sr.replace("envia_usuario-2.jpg", "envia_usuario.jpg");
                boton[0].src = sr;
            }
            if (boton[0].alt == "Renviar a Grupo") {
                var sr = boton[0].src;
                sr = sr.replace("enviar_actividad-2.jpg", "enviar_actividad.jpg");
                boton[0].src = sr;
            }
            if (boton[0].alt == "Sin tareas pendientes") {
                var sr = boton[0].src;
                sr = sr.replace("pendiente2.jpg", "pendiente.jpg");
                boton[0].src = sr;
            }
            if (boton[0].alt == "tareas pendientes") {
                var sr = boton[0].src;
                sr = sr.replace("pendiente2.jpg", "pendiente.jpg");
                boton[0].src = sr;
            }
            if (boton[0].alt == "Anotacion a tarea actual") {
                var sr = boton[0].src;
                sr = sr.replace("notas-2.jpg", "notas.jpg");
                boton[0].src = sr;
            }
            if (boton[0].alt == "Enviar tarea a") {
                var sr = boton[0].src;
                sr = sr.replace("terminar-2.jpg", "terminar.jpg");
                boton[0].src = sr;
            }
            if (boton[0].alt == "El sistema decide el envío por usted") {
                var sr = boton[0].src;
                sr = sr.replace("autoterminar-2.jpg", "autoterminar.jpg");
                boton[0].src = sr;
            }
        });
    }
    

    $("#ButtonFiltro").click(function (e) {
        var check = true;
        var s = $("#contenidobusqueda_filtro").val().toLowerCase();
        $("#HiddenFiltro").val(s);
    });
    $("#contenidobusqueda").keypress(function (e) {
        var check = document.getElementById("CheckboxBusqueda").checked;
        var s = $("#contenidobusqueda").val().toLowerCase();
        if (e.which == 13) {
            $('#GridViewlista').Buscar(
               {
                   contenido: s,
                   chekd: check,
                   id_selecion: $('#Hidden_id').val()
               });
            event.preventDefault();
        }
    })
    $("#TextBoxdatos").keypress(function (e) {
        if (e.which == 13) {
            event.preventDefault();
        }
    })
    $("#contenidobusqueda_filtro").keypress(function (e) {
        if (e.which == 13) {
            event.preventDefault();
        }
    })

});
function cambia_estado_boton_reasignar(estado) {
    if (estado == "VISIBLE") {
        $("#ButtonReasignarTerminar").css("display", "block")
    } else {
        $("#ButtonReasignarTerminar").css("display", "none")
    }
}

//ACTUALIZA GRED VIEW PENDIENTE
function actualiza_gred_pendiente(contenido) {
    try {
    $("#GridViewlista tr[id=" + $("#Hidden_id").val() + "]").each(function () {
        var idex = -1;
        idex = colum_index("DETALLEPENDIENTE");
        if (idex != -1) {
            $(this)[0].cells[idex].innerText = contenido;
        }

    })
    }
    catch (err) {
        alert(err.message + " funcion filtro_gred " + err.message);
    }
}
function colum_index(colum_name) {
    try {
    var x = $('#GridViewlista th');
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
//Actualiza detalle pendiente
function actualiza_gred(columname,valor) {
    try {
        $("#GridViewlista tr[id=" + $("#Hidden_id").val() + "]").each(function () {
            var idex = -1;
            idex = colum_index(columname);
            if (idex != -1) {
                if ($(this)[0].cells[idex].children.length > 0) {
                    $(this)[0].cells[idex].children[0].innerText = valor;
                    $(this)[0].cells[idex].children[0].style.overflow = "auto";
                } else {
                    $(this)[0].cells[idex].innerText = "";
                    var heade = $("#GridViewlistaCopy th");
                    var div = document.createElement("div");
                    div.innerText = valor;
                    div.style.overflow = "auto";
                    //heade[idex].children[0].style.width = div.style.width
                    div.style.width = heade[idex].children[0].style.width;
                    $(this)[0].cells[idex].appendChild(div);

                }
            }
            

        })

    }
    catch (err) {
        alert(err.message + " Funcion actualiza_gred");
    }
}
//Retorna el idex de una columna en una tabla
function colum_index(colum_name) {
    try {
    var x = $('#GridViewlista th');
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

//ELIMINA REGISTROS GRED PENDIENTES
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
            auto_zise_popup_pendinetes();
        } else {
            if (document.getElementById(gred).clientHeight < document.getElementById(gred + "PanelItemContent").clientHeight) {
                //document.getElementById(gred + "VerticalRail").style.display = "none"; .style.visibility = "hidden";VerticalBar

                if (document.getElementById(gred + "VerticalRail") !== undefined) {
                    document.getElementById(gred + "VerticalRail").style.display = "none";
                    document.getElementById(gred + "VerticalRail").style.visibility = "hidden";
                    document.getElementById(gred + "VerticalBar").style.display = "none";
                    document.getElementById(gred + "VerticalBar").style.visibility = "hidden";
                }

            }
        }
    }
    catch (err) {
        alert(err.message + " Funcion eliminar_fila_data_gred");
    }

}
function elimina_registro_gred_pendiente() {
    try {
        if (document.getElementById("Hidden_lista_eliminar_tarea") == "0") {
            return false;
        }
        var spli = document.getElementById("Hidden_lista_eliminar_tarea").value.split(".");
        for (i = 0; i <= spli.length - 1 ; i++) {
            $('#GridViewlista .dummychkstyle').each(function () {
                if (this.checked == true) {
                    var cel = $(this).parent().parent().parent();
                    var atri = $(this).parent().parent().parent().attr("id");
                    if (atri == undefined) {
                        cel = $(this).parent().parent();
                        atri = $(this).parent().parent().attr("id");
                    }
                    if (atri == spli[i]) {
                        document.getElementById("GridViewlista").deleteRow(cel[0].rowIndex);
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
    //SELECCIONA LAS TAREAS CON CHECK
function asigna_usuario_grupos_cheked() {
    try {
        var fer = "0";
        var i = 0;
        $('#hdnEmailID_sel').val("0");
        $('#GridViewlista .dummychkstyle').each(function () {
            if (this.checked == true) {
                var cel = $(this).parent().parent().parent();          
                var atri = $(this).parent().parent().parent().attr("id");
                if (atri == undefined) {
                    atri = $(this).parent().parent().attr("id");
                    cel = $(this).parent().parent();
                  
                }
                var index = -1;
                index = colum_index("RADICADO");           
                atri = atri + "-" + cel[0].cells[index].innerText;
                if (atri !== undefined && cel[0].display !== "none") {
                    if (fer == "0") {
                        fer = atri;
                    } else {
                        fer = fer + "." + atri;
                    }
                }
                i = i + 1;
            }

            //$('#Hiddenseltareas', window.parent.document).val(fer)
        });
        $('#hdnEmailID_sel').val(fer);
    }
    catch (err) {
        alert(err.message + " funcion asigna_usuario_grupos_cheked " + err.message);
    }
    }
    //ACTIVA EL BOTON ASIGNAR TAREA
    function asignar_tarea_pendiente() {
        try {
        if ($("#Hidden_id").val() == "-1" || $("#Hidden_id").val() == "0" || $("#Hidden_id").val() == "") {
            alert("Debe seleccionar una tarea para asignar");
            return false;
        }
        var bto = window.parent.document.getElementById("ButtonAsignar");
        if (bto !== null) {
            $('#hdnEmailID', window.parent.document).val($("#Hidden_id").val());
            bto.click();
        }
        }
        catch (err) {
            alert(err.message + " funcion asignar_tarea_pendiente " + err.message);
        }
    }
    //ACTIVA BOTON SUBIR A PEDIENTE
    function sube_tarea_a_pendiente() {
        try {
        if (document.getElementById("Hidden_resultado").value == "YES") {
            var bto = window.parent.document.getElementById("Button_sube_pediente");
            if (bto !== null) {        
                bto.click();
            }
        }
        }
        catch (err) {
            alert(err.message + " funcion sube_tarea_a_pendiente " + err.message);
        }
    }

    function onDataShown(sender, args) {
        sender._popupBehavior._element.style.zIndex = 1000001;
   
    }
    //MUEVE EL SCCROL AL ID SELECCIONADO
    function mueve_scroll_data_gred(data_grid, HiddenSeleccion) {
        try {
        if ($("#" + data_grid + " td").children.length == 0 && $("#" + data_grid + " tr:visible").length == 0) {
            return true;
        }
        if ($("#" + HiddenSeleccion).val() != "-1" && $("#" + HiddenSeleccion).val() != "0") {
            var scrollableDiv = $("#" + data_grid).parent();
            var index = $("#" + data_grid + " tr[id= " + $("#" + HiddenSeleccion).val() + "]");
            //limpia todos los seleccionados
            $("#" + data_grid + " tr[id]").css({ "background-color": "transparent", "color": "Black" });
            $("#" + data_grid + " tr[id=" + $("#" + HiddenSeleccion).val() + "]").css({ "background-color": "LightSkyBlue", "color": "Red" });
            $("#" + data_grid + " tr[id= " + $("#" + HiddenSeleccion).val() + "]").each(function () {
                if (index[0].rowIndex > 1) {
                    $(scrollableDiv).scrollTop(70);
                    $(scrollableDiv).scrollTop(($(this).offset().top));
                    return true;
                }
               
            });
        }
        }
        catch (err) {
            alert(err.message + " Funcion mueve_scroll_data_gred");
        }
    }
    function confirma_respuesta(mensaje) {
        try {
        if (document.getElementById("hdnEmailID").value == "0" || document.getElementById("hdnEmailID").value == "-1") {
            alert("Por favor seleccione un usuario o grupo para enviar la tarea")
            return false;
        }
        var res = confirm(mensaje);
        if (res == true) {
            document.getElementById("HiddenPROMP").value = "1";
        } else {
            document.getElementById("HiddenPROMP").value = "0";
        }
    }
    catch (err) {
        alert("Funcion confirma_respuesta" + err.message);
        }
    }
    function confirma_respuesta_terminar(mensaje) {
        try {
        if (document.getElementById("hdnEmailID_sel").value == "0" || document.getElementById("hdnEmailID_sel").value == "-1") {
            alert("Por favor seleccione un usuario o grupo para enviar la tarea")
            return false;
        }
        var res = confirm(mensaje + " " + document.getElementById("DropDownActividades").value);
        if (res == true) {
            document.getElementById("HiddenPROMP").value = "1";
        } else {
            document.getElementById("HiddenPROMP").value = "0";
        }
        }
        catch (err) {
            alert(err.message + " funcion confirma_respuesta_terminar");
        }
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
        try {
        var e = $("#" + idente_chekbi_actyive);
        if ($(e).is(':checked')) {
            var x = document.getElementsByClassName("dummychkstyle");
            for (i = 0; i < x.length; i++) {
                var z = x[i];
                if (z !== null) {
                    var parnt = z.parentNode.parentNode.parentNode;
                    if (z.parentNode.parentNode.parentNode.hasAttribute("id") == false) {
                        parnt = z.parentNode.parentNode;
                    }
                    if (parnt.style.display != "none") {
                        z.checked = false;
                    }                  
                }
            }
        }
        else {

            var x = document.getElementsByClassName("dummychkstyle");
            for (i = 0; i < x.length; i++) {
                var z = x[i];
                if ( z !== null){
                    var parnt = z.parentNode.parentNode.parentNode;
                    if (z.parentNode.parentNode.parentNode.hasAttribute("id") == false) {
                        parnt = z.parentNode.parentNode;                      
                    }                  
                    if (parnt.style.display != "none") {
                        z.checked = true;
                    }
                }
            }
        }
        }
        catch (err) {
            alert(err.message + " funcion desactiva_ch_data_grid " + err.message);
        }
    }
    function auto_zise_popup_pendinetes() {
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
       

       
        if (window.parent.document.getElementById("Iframependiente_")) {
            espacio_iframe = window.parent.document.getElementById("Iframependiente_").clientHeight;
            with_frame = window.parent.document.getElementById("Iframependiente_").clientWidth - 10;
        }
        $('#contenido_desicion').css("height", (document.getElementById("boton_heig").clientHeight + 10) + "px");
        $('#buton').css("height", (document.getElementById("heig_bot").clientHeight + 10) + "px");
        var total = document.getElementById("buton").clientHeight + document.getElementById("contenido_desicion").clientHeight;
        var theigt = (espacio_iframe - (total + 20));
        $('#Lista').css("height", (espacio_iframe - (total + 40)) + "px");     
        var gridwith = with_frame - 20;
        var gridheihg = document.getElementById("Lista").clientHeight;
        gridheihg = gridheihg - 5;
        //LLAMA PLUGIN FIJA HIDER O TITULOS      
        if ($('#GridViewlista td').children.length > 0 && $('#GridViewlista tr:visible').length > 0) {            
            $(document).ready(function () { $('#GridViewlista').gridviewScroll({ width: gridwith, height: gridheihg }); })
        }
        }
        catch (err) {
            alert(err.message + " funcion auto_zise_popup_pendinetes " + err.message);
        }
    }
    function auto_zise_popup_envia_usuario_grupo() {
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



            $('#DivBotones').css("height", (document.getElementById("btnCancelpagina").clientHeight + 5) + "px");
            var total = document.getElementById("DivBotones").clientHeight + document.getElementById("Divcab").clientHeight;
            $('#DivColorPagina').css("height", ((espacio_iframe - 1) - (total + 5)) + "px");
            $('#frameeditexpanse_').css("height", ((espacio_iframe - 1) - (total + 5)) + "px");
           
        }
        catch (err) {
            alert(err.message + " funcion auto_zise_popup_envia_usuario_grupo " + err.message);
        }
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



        $('#Panel_visor_externo').css("height", (espacio_iframe - 5) + "px");
        $('#Cotenedorpendiente_visor_externo').css("height", (espacio_iframe - 5) + "px");
        $('#Iframe_visor_externo_').css("height", (espacio_iframe - 10) + "px");

    }

    function progres_hiden(progres) {
        $("#progres_bar").css("display", "none");
    }
//ACTIVA EL GIF DE PROGRESO DE UN EVENTO
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
            alert(err.message + " funcion posicion_update_pogres " + err.message);
        }

    }
    function busqueda_gred__(HiddenSeleccion, data_grid, contenido_busqueda, CheckboxBusqueda) {
        try {
            if ($("#" + contenido_busqueda).val() == "") {
                $("#" + data_grid + " tr[id]").css({ "background-color": "transparent", "color": "Black" });
                $("#" + HiddenSeleccion).val("0");
                return false;
            }
            $("#" + HiddenSeleccion).val("0");
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
                                $(this).parent().css({ "background-color": "LightSkyBlue", "color": "orange" });
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
                                $(this).parent().css({ "background-color": "LightSkyBlue", "color": "orange" });
                                //$(scrollableDiv).scrollTop(70);
                                //var id_ref = $(this).parent();
                                //$(scrollableDiv).scrollTop($(id_ref).offset().top var id_ref = $(this).parent();
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
    function busqueda_gred(HiddenSeleccion, data_grid, contenido_busqueda, CheckboxBusqueda) {
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
    function filtro_gred(HiddenSeleccion, data_grid, contenido_busqueda, CheckboxBusqueda) {
        try {
            if (document.getElementById(data_grid).rows.length == 1) {
                return true;
            }
            var ro = document.getElementById(data_grid).rows.length;
            //desactiva_chek();
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
            var acierto = -1;
            var check = document.getElementById(CheckboxBusqueda).checked;
            if (check == true && s == "") {
                return true;
            }
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
                                refdif.show();
                                confirm = 1;
                                acierto = 1;
                            } else {

                                //refdif.hide();
                            }
                        }
                    }

                    if (check == false) {
                        if (idex >= 0) {
                            var compare = tempotd;
                            var strcompre = compare.indexOf(s);
                            if (strcompre >= 0) {
                                refdif.show();
                                acierto = 1;
                                confirm = 1;
                            } else {
                                //refdif.hide();
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
                if (acierto == -1) {
                    $("#" + data_grid + " tr:hidden").show();
                }
            });
        }
        catch (err) {
            alert(err.message + " funcion filtro_gred " + err.message);
        }
    }
    function filtro_gred_1(HiddenSeleccion, data_grid, contenido_busqueda, CheckboxBusqueda) {
        try {
        if (document.getElementById(data_grid).rows.length == 1) {
            return true;
        }
        var ro = document.getElementById(data_grid).rows.length;
        desactiva_chek();
        $("#" + HiddenSeleccion).val("-1");
        $("#Hidden_id").val("0");
        var refgrid;
        var filtro;
        var ito = 0;
        var confirma_hidem_fila = 0;
        var showtr;
        $("#" + data_grid + " tr[id]").css({ "background-color": "transparent", "color": "Black" });
        var s = $("#" + contenido_busqueda).val().toLowerCase();
        var grid = $("#" + data_grid);
        $("#" + data_grid + " tr:hidden").show();
        var acierto = -1;
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

                                acierto = 1;
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
                                acierto = 1;
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
                if (acierto == -1) {
                    $("#" + data_grid + " tr:hidden").show();
                    
                }
            });
        }
        catch (err) {
            alert(err.message + " funcion filtro_gred " + err.message);
        }
    }
    //Función que permite que el boton que se agrega a la lista no envie el formulario
    function prevent(event, element) {
        try {
            //Evita el posback del boton
            event.preventDefault();
            // Marca la liena seleccionada
            $('#data_grid tr[id]').css({ "background": "White", "color": "Black" });
            $('#data_grid tr[id]').each(function () {
                $(this).css({ "background-color": "White", "color": "Black" });
            });
            //Captura el atributo del boton
            var fer = $(element).attr("id");
            //Asigna el parametro al hiden relacionado 
            $('#Hidden_id_actividad_flujo').val(fer);
            $('#Hidden_id_flujo_trabjo').val($(element).attr("id_flujo_trabjo"));
            $('#Hidden_id_actividad_destino').val($(element).attr("id_actividad_destino"));
            $('#Hidden_id_usuario_workflow').val($(element).attr("id_usuario_workflow"));
            $('#Hidden_id_conector').val($(element).attr("id_conector"));
            var x;
            var r = confirm("Desea enviar la tarea a la actividad seleccionada del flujo de trabajo");
            if (r == true) {
                document.getElementById("Button_activa_enviar_actividad_flujo_trabajo").click();
            }
            else {

            }
            element.focus();
        }
        catch (err) {
            alert(err.message + " Funcion prevent");
        }
    }
    function prevent_ruta(event, element) {
        try {
            //Evita el posback del boton
            event.preventDefault();
            //Captura el atributo del boton
            var fer = $(element).attr("id");
            $('#Hidden_id_actividad_ruta').val(fer);
            $('#Hidden_nombre_actividad').val($(element).attr("nombre_actividad"));
            $('#Hidden_id_tar_sel').val($(element).attr("id_tar_sel"));
            var x;
            var r = confirm("Desea enviar la tarea a la actividad " + document.getElementById('Hidden_nombre_actividad').value + " de la ruta de trabajo");
            if (r == true) {
                document.getElementById("Button_activa_enviar_actividad_ruta").click();
            }
            else {

            }
            element.focus();
        }
        catch (err) {
            alert(err.message + " Funcion prevent");
        }
    }
    function prevent_detalle(event, element) {
        try {
            //Evita el posback del boton
            event.preventDefault();
            // Marca la liena seleccionada
            $('#data_grid tr[id]').css({ "background": "White", "color": "Black" });
            $('#data_grid tr[id]').each(function () {
                $(this).css({ "background-color": "white", "color": "Black" });
            });
            //Captura el atributo del boton
            var fer = $(element).attr("id");
            //Asigna el parametro al hiden relacionado
            $('#Hidden_id_actividad_flujo').val(fer);
            $('#Hidden_id_flujo_trabjo').val($(element).attr("id_flujo_trabjo"));
            $('#Hidden_id_actividad_destino').val($(element).attr("id_actividad_destino"));
            $('#Hidden_id_usuario_workflow').val($(element).attr("id_usuario_workflow"));
            //Boton que ejecuta la acción del lado del servidor Hidden_id_actividad_ruta
            document.getElementById("Button_detalle_enviar_actividad_flujo_trabajo").click();
            element.focus();
        }
        catch (err) {
            alert(err.message + " Funcion prevent_detalle");
        }
    }
    function prevent_detalle_actividad_ruta(event, element) {
        try {
            //Evita el posback del boton
            event.preventDefault();
            // Marca la liena seleccionada
            $('#div_gred_actividad tr[id]').css({ "background": "White", "color": "Black" });
            $('#div_gred_actividad tr[id]').each(function () {
                $(this).css({ "background-color": "White", "color": "Black" });
            });
            //Captura el atributo del boton
            var fer = $(element).attr("id");
            //Asigna el parametro al hiden relacionado
            $('#Hidden_id_actividad_ruta').val(fer);
            //Boton que ejecuta la acción del lado del servidor Hidden_id_actividad_ruta
            document.getElementById("Button_detalle_enviar_actividad_ruta").click();
            element.focus();
        }
        catch (err) {
            alert(err.message + " Funcion prevent_detalle_actividad_ruta");
        }
    }
    function prevent_blank(event, element) {
        try {
            //Evita el posback del boton
            event.preventDefault();
            element.focus();
        }
        catch (err) {
            alert(err.message + " Funcion prevent_blank");
        }
    }

    function auto_zise_popup_lista_tareas(value_lista_general) {
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

            var suma_div = document.getElementById("div_contenido_procesa_lista_actividades_worflow_ruta_botones_desicion").clientHeight + document.getElementById("contenido_titulo_data_grid_dos_title").clientHeight + document.getElementById("div_actividades_disponibles_flujo").clientHeight;
            var gridheihg_ = document.getElementById("div_actividades_disponibles_flujo").clientHeight - suma_div;
            var gridwith = document.getElementById("div_gred").clientWidth;
            $('#data_grid').css("width", (gridwith - 30) + "px");
            var gridheihg = document.getElementById('Panel_lista_actividades_worflow_ruta').clientHeight - suma_div;
            $('#div_gred').css("height", (gridheihg - 25) + "px");
            if (value_lista_general == "1") {
                if ($('#data_grid td').children.length > 0 && $('#data_grid tr:visible').length > 0) {

                    $('#data_grid th').each(function () {

                        $(this).css("display", "none");

                    });
                }
            }
        }
        catch (err) {
            alert(err.message + " funcion auto_zise_popup_lista_tareas " + err.message);
        }
    }
    function auto_zise_popup_lista_tareas_ruta(value_lista_general) {
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


            var suma_div = document.getElementById("div_contenido_procesa_lista_actividades_ruta_botones_desicion_actividad").clientHeight + document.getElementById("contenido_titulo_data_grid_dos_title_actividad").clientHeight + document.getElementById("div_actividades_diponibles").clientHeight;
            var gridwith = document.getElementById("Panel_lista_actividades_ruta").clientWidth - 5;
            var gridheihg_ = document.getElementById("contenido_procesa_lista_actividades_ruta").clientHeight - suma_div;
            $('#div_gred_actividad').css("height", (gridheihg_ - 19) + "px");
            if (value_lista_general == "1") {
                if ($('#data_grid_actividad td').children.length > 0 && $('#data_grid_actividad tr:visible').length > 0) {

                    $('#data_grid_actividad th').each(function () {

                        $(this).css("display", "none");

                    });

                }
            }
        }
        catch (err) {
            alert(err.message + " funcion auto_zise_popup_lista_tareas_ruta " + err.message);
        }
    }
   