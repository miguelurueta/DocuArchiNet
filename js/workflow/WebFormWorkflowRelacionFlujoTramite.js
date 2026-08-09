$(document).ready(function () {
    $.fn.inicio = function () {
        var STATE_INI = 1;
        if (STATE_INI == 1) {
            STATE_INI = 0;
            auto_zise_ventana_relacion();
            //precarga_datos_busqueda();
            auto_zise_ventana_relacion();
            auto_zise_popup_lista_tareas("1");
            service_posibles_tramites();
        }
        auto_zise_ventana_relacion();
        //Agrega la selección a la lista selecionda en el data gred
        $('#data_grid tr[id]').click(function () {
            $('#data_grid tr[id]').css({ "background": "White", "color": "Black" });
            $(this).css({ "background-color": "#e8e8f7", "color": "Black" });
        });

    }
});
var ITEMS_DATOS = new Array();  //GUARDA LOS DATOS DE BUSQUEDA DEL TREVIEW
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
        CargarDatos();
        precarga_datos_busqueda();

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
        auto_zise_ventana_relacion();
        auto_zise_popup_lista_tareas("1");
        auto_zise_ventana_relacion();
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
function precarga_datos_busqueda() {
    try {
        $("#TextBox_busqueda_tre").autocomplete({
            maxResults: 20,
            source: function (request, response) {
                var results = $.ui.autocomplete.filter(ITEMS_DATOS, request.term);
                response(results.slice(0, this.options.maxResults));
            },
            select: function (event, ui) {
                document.getElementById("Hidden_texto_buequeda").value = ui.item.text;
                document.getElementById("Button_activa_busqueda_treview").click();
                return true;
                //OnSearchClick(ui.item.id, 'TreeViewArchivo');
                //return ui.item.id;
            }, minLength: 1
        })
    }
    catch (err) {
        alert(err.message + " Funcion precarga_datos_busqueda");
    }
}

function delete_array(value_id) {
    try {
        for (var i = 0; i < ITEMS_DATOS.length; i++) {
            if (ITEMS_DATOS[i].id === value_id) {
                ITEMS_DATOS.splice(i, 1);
                return true;
            }
        }
    } catch (err) {
        alert(err.message + " Funcion delete_array");
    }
}
function actualiza_array(value_id, texto) {
    try {
        for (var i = 0; i < ITEMS_DATOS.length; i++) {
            if (ITEMS_DATOS[i].id == value_id) {
                ITEMS_DATOS[i].text = texto;
                ITEMS_DATOS[i].value = texto;
                i = ITEMS_DATOS.length;
            }
        }
    } catch (err) {
        alert(err.message + " Funcion actualiza_array");
    }
}
function CargarDatos() {
    try {
        var treeN = $(".TreeN");
        if (treeN.length) {
            var nodo = $("#" + treeN[0].id + " a");
            var i = 0;
            for (var index = 0; index < nodo.length; index++) {
                if (nodo[index].innerText !== "") {
                    ITEMS_DATOS.push({ text: nodo[index].innerText, id: nodo[index].id, value: nodo[index].innerText });
                    if (nodo[index].title.indexOf("|") !== -1) {
                        //EXPDIENTE_JERARQUIA++;
                    } else {
                        //NIVEL_JERARQUIA++;
                    }
                    i++;
                }

            }
        }
        return ITEMS_DATOS;
    }
    catch (err) {
        alert(err.message + " Funcion CargarDatos");
    }
}
function service_posibles_tramites() {
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
                    url: "../webservice/WebServiceRadicacion.asmx/Get_lista_Tramites",
                    data: "{'DName':'" + document.getElementById("auto_complex").value + "'}",
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
                document.getElementById("auto_complex").value = ui.item.label;
                document.getElementById("Button_busqueda").click();
            }

            , minLength: 3, max: 10, scroll: true
        });
}
function preven_event_search(event, e) {
    try {
        document.getElementById("Button_busqueda").click();
        event.preventDefault();
    }
    catch (err) {
        alert(err.message + " Funcion preven_event_search");
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
function preven_event_restor_search(event, e) {
    try {
        document.getElementById("Button_activa_relacion").click();
        event.preventDefault();
    }
    catch (err) {
        alert(err.message + " Funcion preven_event_search");
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
function auto_zise_ventana_relacion() {
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

        var suma_div = document.getElementById("div_busqueda").clientHeight + document.getElementById("div_botones").clientHeight;
        $('#div_treview').css("height", (espacio_iframe) - (suma_div) + "px");
        
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_ventana_relacion");
    }

}
function prevent(event, element) {
    try {
        //Evita el posback del boton
        event.preventDefault();
        // Marca la liena seleccionada
        $('#data_grid tr[id]').css({ "background": "White", "color": "Black" });
        $('#data_grid tr[id]').each(function () {
            $(this).css({ "background-color": "#E7EDF5", "color": "Black" });
        });

        //document.getElementById("HiddenFi").value = "YES";
        //Captura el atributo del boton
        var g = element;
        var fer = $(element).attr("idd");
        //Asigna el parametro al hiden relacionado
        $('#hdnEmailID').val(fer);
        //Boton que ejecuta la acción del lado del servidor
        document.getElementById("Button_relaciona_tramite_flujo").click();
        element.focus();
    }
    catch (err) {
        alert(err.message + " Funcion prevent");
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
        //corrige el posicionamiento horizontal del modalpopuextender, la propiedad width del panel debe estar en auto
        //var widtth_procent_left_rigth = (with_frame - document.getElementById("Panel_agregar_expediente_carpeta").clientHeight) / 2;
        //$('#Panel_agregar_expediente_carpeta').css("left", (Math.round(widtth_procent_left_rigth)) + "px");
        var heig_porcent = espacio_iframe - ((espacio_iframe * 10) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_lista_tramites').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_Panel_lista_tramites').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#contenido_procesa_lista_actividades_workflow').css("height", (document.getElementById("modal_content_Panel_lista_tramites").clientHeight - (document.getElementById("divcabecer2_lista_tramites").clientHeight )) + "px");
        //Para los modal que contiene gred
        $('#panel_conten_gred').css("height", (document.getElementById("contenido_procesa_lista_actividades_workflow").clientHeight - (document.getElementById("contenido_titulo_data_grid_dos_title").clientHeight + 50)) + "px");
    }
    catch (err) {
        alert("funcion auto_zise_popup_lista_tareas " + err.message);
    }
}



