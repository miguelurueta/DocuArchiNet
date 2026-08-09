$(document).ready(function () {
    $.fn.inicio = function () {
        if (ESTATE == 0) {
            ESTATE = 1;
            auto_zise_popup_paginas_externas_libres();
            auto_size_page();
            auto_zise_popup_editar_instrumento();
            auto_zise_popup_agregar_instrumento();
            auto_zise_popup_agregar_editar_serie();
            auto_zise_popup_agregar_editar_sub_serie();
            
        }
        CargarDatos();
        precarga_datos_busqueda();
        //precarga_datos_busqueda();      
        $('.table_tre_row ').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });
        $('.table_tre_row ').click(function () {
            $('.table_tre_row ').css({ "background": "White", "color": "Black" });
            $('.table_tre_row tr[id]').css({ "background": "White", "color": "Black" });
            $(this).css({ "background-color": "#E7EDF5", "color": "Black" });
            var fer = $(this).attr("id");
            $('#Hidden_sel').val(fer);
        });
        $('#Paneltreview').contextMenu('context-menu', {

            'Salir del menú': {
                click: function (element) {
                },
                klass: "fad fa-times"
            },

            'Agregar un nuevo elemento en la tabla': {
                click: function (element) {
                    document.getElementById("Hidden_menu_var_event_dive").value = "IAC-ADD-TABLA";
                    document.getElementById("Button_me_active_men_dive").click();
                },
                klass: "fal fa-plus"
            },
            'Eliminar el elemento seleccionado de la tabla': {
                click: function (element) {
                    document.getElementById("Hidden_menu_var_event_dive").value = "IAC-ELIM-TABLA";
                    document.getElementById("Button_me_active_men_dive").click();
                },
                klass: "fal fa-times"
            },
            'Edita el elemento seleccionado de la tabla': {
                click: function (element) {
                    document.getElementById("Hidden_menu_var_event_dive").value = "IAC-EDIDA-TABLA";
                    document.getElementById("Button_me_active_men_dive").click();
                },
                klass: "fal fa-pencil"
            },
            'Cambia estado del elemento seleccionado de la tabla': {
                click: function (element) {
                    document.getElementById("Hidden_menu_var_event_dive").value = "IAC-ACTIVA-TABLA";
                    document.getElementById("Button_me_active_men_dive").click();
                },
                klass: "fad fa-exchange-alt"
            }
        });
    }


});
var ESTATE = 0;
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
        auto_zise_popup_paginas_externas_libres();
        auto_size_page();
        auto_zise_popup_editar_instrumento();
        auto_zise_popup_agregar_instrumento();
        auto_zise_popup_agregar_editar_serie();
        auto_zise_popup_agregar_editar_sub_serie();
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
function precarga_datos_busqueda_() {
    var data = CargarDatos_ini();
    $("#TextBox_busqueda").autocomplete({

        source: data,
        select: function (event, ui) {
            Seleccionar(ui.item.value);
            return ui.item.value;
        }
       
    })

}
function CargarDatos_ini_() {
    var treeN = $(".TreeN");
    var items = new Array();
    if (treeN.length) {
        var nodo = $("#" + treeN[0].id + " td:a");
        for (var index = 0; index < nodo.length; index++) {

            items[index] = nodo[index].innerText;

        }

    }

    return items;

}
function Seleccionar(treeID) {
    var treeN = $(".TreeN");
    if (treeN.length) {
        var nodo = $("#" + treeN[0].id + " td: a");
        for (var index = 0; index < nodo.length; index++) {
            var nodval = nodo[index].innerText;
            if (nodval == treeID) {      
                document.getElementById("Hidden_texto_buequeda").value = nodo[index].pathname;
                alert(nodo[index].pathname);
                document.getElementById("Button_activa_busqueda_treview").click();
                return true;
            }
        }
    }
}
function CargarDatos_() {
    var treeN = $(".TreeN");
    var items = new Array();
    if (treeN.length) {
        var nodo = $("#" + treeN[0].id + " td: a");
        for (var index = 0; index < nodo.length; index++) {
            var nod = nodo[index];
            items[index] = { text: nodo[index].innerText, id: nodo[index].id, value: nodo[index].nodeValue };
        }
    }
    return items;
}
function precarga_datos_busqueda() {
    try {
        $("#TextBox_busqueda").autocomplete({
            maxResults: 20,
            source: function (request, response) {
                var results = $.ui.autocomplete.filter(ITEMS_DATOS, request.term);
                response(results.slice(0, this.options.maxResults));
            },
            select: function (event, ui) {
                document.getElementById("Hidden_texto_buequeda").value = ui.item.pathname;
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
        ITEMS_DATOS = new Array();
        var treeN = $(".TreeN");
        if (treeN.length) {
            var nodo = $("#" + treeN[0].id + " a");
            var i = 0;
            for (var index = 0; index < nodo.length; index++) {
                if (nodo[index].innerText !== "") {
                    ITEMS_DATOS.push({ text: nodo[index].innerText, id: nodo[index].id, value: nodo[index].innerText, pathname: nodo[index].pathname });
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
function auto_size_page() {
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
              
            }
        }
        var gridheihg = document.getElementById("menu_var").clientHeight + document.getElementById("div_instrumentos").clientHeight + document.getElementById("TextBox_busqueda").clientHeight + document.getElementById("div_estado").clientHeight;
        $('#div_treview_archivo').css("height", (( espacio_iframe - 30) - gridheihg) + "px");
    }
    catch (err) {
        alert(err.message + " Funcion auto_size_page");
    }
}
function auto_zise_popup_paginas_externas_libres() {
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

        $('#Panel_paginas_externas_popou').css("height", (espacio_iframe) + "px");
        $('#contenido_procesa_paginas_externas_popou').css("height", (espacio_iframe - 20) + "px");
        $('#Iframe_paginas_externas_popup_').css("height", (espacio_iframe - 20) + "px");
       
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_paginas_externas_libres");
    }

}
function auto_zise_popup_agregar_instrumento() {
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
        $('#Panel_agregar_instrumento').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_Panel_agregar_instrumento').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera 
        $('#Contenido_agregar_instrumento').css("height", (document.getElementById("modal_content_Panel_agregar_instrumento").clientHeight - (document.getElementById("Divcerrarbuton2_agregar_instrumento").clientHeight + document.getElementById("modal-footer_panel_agregar_instrumento").clientHeight)) + "px");
        //Para los modal que contiene gred
        //$('#content_data_grid').css("height", (document.getElementById("contenido_procesa_usu_rel_solicitud").clientHeight - document.getElementById("contenido_titulo_val_radicacion_documentos").clientHeight) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_agregar_instrumento " + err.message);
    }
}
function auto_zise_popup_editar_instrumento() {
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
        $('#Panel_editar_instrumento').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_Panel_editar_instrumento').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
        $('#Contenido_editar_instrumento').css("height", (document.getElementById("modal_content_Panel_editar_instrumento").clientHeight - (document.getElementById("Divcerrarbuton2_editar_instrumento").clientHeight + document.getElementById("modal-footer_panel_editar_instrumento").clientHeight)) + "px");
        //Para los modal que contiene gred
        //$('#content_data_grid').css("height", (document.getElementById("contenido_procesa_usu_rel_solicitud").clientHeight - document.getElementById("contenido_titulo_val_radicacion_documentos").clientHeight) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_editar_instrumento " + err.message);
    }
}
function auto_zise_popup_agregar_editar_serie() {
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
        $('#Panel_agregar_serie').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_Panel_agregar_serie').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
        $('#Contenido_agregar_serie').css("height", (document.getElementById("modal_content_Panel_agregar_serie").clientHeight - (document.getElementById("Divcerrarbuton2_agregar_serie").clientHeight + document.getElementById("modal-footer_Panel_agregar_serie").clientHeight)) + "px");
        //Para los modal que contiene gred
        //$('#content_data_grid').css("height", (document.getElementById("contenido_procesa_usu_rel_solicitud").clientHeight - document.getElementById("contenido_titulo_val_radicacion_documentos").clientHeight) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_agregar_editar_serie " + err.message);
    }
}
function auto_zise_popup_agregar_editar_sub_serie() {
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
        $('#Panel_agregar_sub_serie').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_Panel_agregar_sub_serie').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
        $('#Contenido_agregar_sub_serie').css("height", (document.getElementById("modal_content_Panel_agregar_sub_serie").clientHeight - (document.getElementById("Divcerrarbuton2_agregar_sub_serie").clientHeight + document.getElementById("modal-footer_Panel_agregar_sub_serie").clientHeight)) + "px");
        //Para los modal que contiene gred
        //$('#content_data_grid').css("height", (document.getElementById("contenido_procesa_usu_rel_solicitud").clientHeight - document.getElementById("contenido_titulo_val_radicacion_documentos").clientHeight) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_agregar_editar_sub_serie " + err.message);
    }
}
