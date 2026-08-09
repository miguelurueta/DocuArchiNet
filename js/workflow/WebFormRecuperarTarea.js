function limpiar_filtro() {
    $("#contenidobusqueda_filtro").val("");
}
$(document).ready(function () { 
    $.fn.presionBoton = function (ident) {
        var semilla = " ";
        var datos = ident.split("|");
        var arc1 = datos[1];
    }
    $("#ocultaleft").click(function (e) {
        /*alert("X: " + e.pageX + " - Y: " + e.pageY)*/

        var d = $("#Content").width.valueOf
        if ($("#Content").is(":hidden")) {

            $("#Contenedorgrid").css("width", "60%");
            $("#contenido_botonoes").css("width", "60%");
            $("#contenido_titulo_resultado").css("width", "60%");
            $("#contenido_general").show(1)
            //auto_zise_recuperar(34);
            dibuja_gred();
            mueve_scroll_data_gred('GridViewlista', 'hdnEmailID');

        } else {
            $("#contenido_general").hide(1);
            $("#Contenedorgrid").css("width", "98%");
            $("#contenido_botonoes").css("width", "98%");
            $("#contenido_titulo_resultado").css("width", "98%");
            //auto_zise_recuperar(3);
            dibuja_gred();
            mueve_scroll_data_gred('GridViewlista', 'hdnEmailID');
  
        }

    });

    $.fn.cligred = function () {
        $('#GridViewlista tr[id]').click(function () {
            $('#GridViewlista tr[id]').css({ "background-color": "White", "color": "Black" });
            $(this).css({ "background-color": "#e8e8f7", "color": "Black" });
            var fer = $(this).attr("id");
            $('#hdnEmailID').val(fer);
            $('#hdnEmailID', window.parent.document).val(fer);

        });
        $('#GridViewlista tr[id]').dblclick(function () {
            if ($('#hdnEmailID').val() != "-1") {
                var split = $('#hdnEmailID').val().split("-");
                if (split.length > 0) {
                    document.getElementById("Hidden_id_tarea_sel").value = split[0];
                    document.getElementById("Hidden_tipo_visor").value = "VISOR WORKFLOW";
                    document.getElementById("Button_visor_emergente").click();
                    return false;
                }

            }

        });
        $('#GridViewlista tr[id]').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });
        auto_zise_recuperar(34);
        auto_zise_popup_visor_externo();
        auto_zise_recuperar(34);
        auto_zise_popup_visor_externo();
        dibuja_gred();
       
    }
    $('#Contenedorgrid').contextMenu('context-menu-1', {
        'Ver Documentos': {
            click: function (element) {  // element is the jquery obj clicked on when context menu launched
                var RowID = $('#hdnEmailID').val();
                if (RowID == "-1") {
                    alert("Por favor seleccione el documento");
                }
                else {
                    if ($('#hdnEmailID').val() != "-1" && $('#hdnEmailID').val() != "0" && $('#hdnEmailID').val() != "") {
                        var split = $('#hdnEmailID').val().split("-");
                        if (split.length > 0) {
                            document.getElementById("Hidden_id_tarea_sel").value = split[0];
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


    } catch (e) {
        alert(" funcion load " + e.message);
    }

});
function rezize_event() {
    try {
        auto_zise_recuperar(34);
        auto_zise_popup_visor_externo();
        auto_zise_recuperar(34);
        auto_zise_popup_visor_externo();
        dibuja_gred();
    } catch (ex) {
        alert(ex.message + " Función rezize_event")
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
        var fer = $(element).attr("id");
        var tip_event = $(element).attr("tip_event");
        if (tip_event == "ver_docu") {
            $('#hdnEmailID').val(fer);
            var split = $('#hdnEmailID').val().split("-");
            if (split.length > 0) {
                document.getElementById("Hidden_id_tarea_sel").value = split[0];
                document.getElementById("Hidden_tipo_visor").value = "VISOR WORKFLOW";
                document.getElementById("Button_visor_emergente").click();
            }
        }
        if (tip_event == "asig_flujo") {
           $('#hdnEmailID').val(fer);
           document.getElementById("ButtonRecuperar").click();
        }
        event.preventDefault();
        element.focus();
    }
    catch (err) {
        alert(err.message + " Funcion prevent");
    }
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
            $("#" + data_grid + " tr[id=" + $("#" + HiddenSeleccion).val() + "]").css({ "background-color": "#E7EDF5", "color": "Black" });
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
//RECUPERA TAREA
function recupera_tarea() {
    try {
        window.parent.document.getElementById("ButtonRecuperar").click();
    }
    catch (err) {
        alert(err.message + " Funcion recupera_tarea");
    }
}
//RECUPERA TAREA 
function recupera_reasigna_tarea() {
    try {
       
        if (parent.document.getElementById("Hidden_usuario_autoriza")) {
            parent.document.getElementById("Hidden_usuario_autoriza").value = document.getElementById("Hidden_usuario_autoriza").value;
        }
        if (parent.document.getElementById("Hidden_usuario_autoriza_id")) {
            parent.document.getElementById("Hidden_usuario_autoriza_id").value = document.getElementById("Hidden_usuario_autoriza_id").value;
        }
        parent.document.getElementById("ButtonRecuperarReasignar").click();
    }
    catch (err) {
        alert(err.message + " Funcion recupera_reasigna_tarea");
    }
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
function progres_hiden(progres) {
    $("#progres_bar").css("display", "none");
}
function activa_busqueda() {
    try {
        busqueda_gred('hdnEmailID', 'GridViewlista', 'contenidobusqueda', 'checkbox');
    }
    catch (err) {
        alert(err.message + " funcion activa_busqueda " + err.message);
    }
}
function busqueda_gred(HiddenSeleccion, data_grid, contenido_busqueda, CheckboxBusqueda) {
    try {
        if ($("#" + contenido_busqueda).val() == "") {
            return false;
        }
        $("#" + HiddenSeleccion).val("0");
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
                            $(this).parent().css({ "background-color": "LightSkyBlue", "color": "orange" });
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
                            $(this).parent().css({ "background-color": "LightSkyBlue", "color": "orange" });
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
        alert(err.message + " funcion busqueda_gred " + err.message);
    }
}
function filtro_gred(HiddenSeleccion, data_grid, contenido_busqueda, CheckboxBusqueda) {
    try {
        if (document.getElementById(data_grid).rows.length == 1) {
            return true;
        }
        $("#" + HiddenSeleccion).val("-1");
        var refgrid;
        var filtro;
        var ito = 0;
        var confirma_hidem_fila = 0;
        var showtr;
        //$("#" + data_grid + " tr[id]").css({ "background-color": "transparent", "color": "Black" });
        var s = $("#" + contenido_busqueda).val().toLowerCase();
        //var grid = $("#" + data_grid);
        //$("#" + data_grid + " tr").hide();
        var acierto = -1;     
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
                            return false;
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
                            return false;
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
function dibuja_gred() {
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
            //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val();Contenedorgrid

        }
    }
    var heigconetedor = (espacio_iframe - 40) - (((espacio_iframe - 40) * 20) / 100);
    var gridwith = (with_frame - 10) - (((with_frame - 10) * 34) / 100);
    var gridheihg = heigconetedor;
    gridheihg = gridheihg - 5;
    gridwith = document.getElementById("Contenedorgrid").offsetWidth - 3;
    //LLAMA PLUGIN FIJA HIDER O TITULOS      
    if ($('#GridViewlista td').children.length > 0 && $('#GridViewlista tr:visible').length > 0) {

        //$(document).ready(function () { $('#GridViewlista').gridviewScroll({ width: gridwith, height: gridheihg }); })
    }
}
    catch (err) {
        alert(err.message + " funcion dibuja_gred " + err.message);
}
}
function auto_zise_recuperar(porcentaje) {
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
                //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val(); contenido_parametro_consulta

            }
        }
        var heigconetedor = (espacio_iframe - 5);    
        $('#contenido_general').css("height", (heigconetedor) + "px");
        $('#Content_consulta').css("height", (heigconetedor - (document.getElementById("Content").clientHeight)) + "px");
        $('#contenido_consulta_historico').css("height", (heigconetedor - (document.getElementById("Content").clientHeight)) + "px");
        $('#Contenedorgrid').css("height", (heigconetedor - (document.getElementById("contenido_titulo_resultado").clientHeight)) + "px");
        $('#Panelactividad').css("height", (heigconetedor - (document.getElementById("contenido_titulo_resultado").clientHeight)) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_pendinetes " + err.message);
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

    /*$('#Panel_visor_externo').css("height", (espacio_iframe - 10) + "px");
    $('#Cotenedorpendiente_visor_externo').css("height", (espacio_iframe - 10) + "px");
    $('#Iframe_visor_externo').css("height", (espacio_iframe - 15) + "px");*/
    //corrige el posicionamiento horizontal del modalpopuextender, la propiedad width del panel debe estar en auto
    //var widtth_procent_left_rigth = (with_frame - document.getElementById("Panel_agregar_expediente_carpeta").clientHeight) / 2;
    //$('#Panel_agregar_expediente_carpeta').css("left", (Math.round(widtth_procent_left_rigth)) + "px");
    var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
    $('#Panel_visor_externo').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
    $('#modal_content_visor_externo').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
    //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
    $('#Contenedor_visor_externo').css("height", (document.getElementById("modal_content_visor_externo").clientHeight - (document.getElementById("Cabecerapendiente_visor_externo").clientHeight + 5)) + "px");
    $('#Iframe_visor_externo_').css("height", (document.getElementById("modal_content_visor_externo").clientHeight - (document.getElementById("Cabecerapendiente_visor_externo").clientHeight + 10)) + "px");
    //Para los modal que contiene gred
    //$('#content_data_grid').css("height", (document.getElementById("contenido_procesa_usu_rel_solicitud").clientHeight - document.getElementById("contenido_titulo_val_radicacion_documentos").clientHeight) + "px");
}
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_visor_externo " + err.message);
}
}
$(document).on('keydown', function (e) {
    if (e.which == 9) {
        var id_element = e.srcElement.className;
        
        var salidadato;
        if (id_element == "DATE" || id_element == "date_2") {
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