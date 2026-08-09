$(document).ready(function () {
    $.fn.inicio = function () {
        auto_zise_ventana_diagrama();
        auto_zise_popup_paginas_externas_libres();
        auto_zise_popup_lista_form_control_person_procent("adm_flujo_update_activity_description",70);
        diagranview_bloqued();
        var ESTATE = 1;
        if (ESTATE == 1) {
            ESTATE = 0;
            auto_zise_popup_lista_tareas("1");
            service_posibles_actividades();
        }
        //Agrega la selección a la lista selecionda
        $('#data_grid tr[id]').click(function () {
            $('#data_grid tr[id]').css({ "background": "White", "color": "Black" });
            $(this).css({ "background-color": "#e8e8f7", "color": "Black" });
        });
    }
});
let ESTADO_EVENT_GENERAL = "";
let IDENT_ACTVITIE_WF = 0;
let NAME_ACTVITIE_WF = "";
//inicializa los eventos
const ini_event_page = () => {
    //Active event update activities workflow
    let array_element = new Array;
    array_element.push({ id: "a_descriptive_activities" }, { id: "boton_event_adm_flujo_update_activity_description"});
    for (let i = 0; i < array_element.length; i++) {
        let elment_a_document_production = document.getElementById(array_element[i].id);
        if (elment_a_document_production) {
            elment_a_document_production.addEventListener("click", handler_element_event, false);
        }
    } 
}
//active event menu
const handler_element_event = (e) => {
    try {
        let name_ID = e.currentTarget.id;
        let result = "";
        switch (name_ID) {
            case "a_descriptive_activities":
                if (IDENT_ACTVITIE_WF != 0) {
                    event_element_menu("ME-UP-ACT-WF", "");
                } else {
                    alert_bot("Debe selecionar la actividad en el diagrama", 'warning', "form_parent_content");
                }
                break;
            case "boton_event_adm_flujo_update_activity_description":
                let result = valida_solicita_datos_control_general("adm_flujo_update_activity_description");
                if (result != "YES") {
                    alert_bot(result, 'warning', "modal_content_adm_flujo_update_activity_description");
                } else {
                    event_element_clic("", e)
                }
               break;
        }
    } catch (ex) {
        alert(ex.mensaje);
    }
}
function event_element_clic(event, e) {
    try {
        ESTADO_EVENT_GENERAL = "intro";
        posicion_update_pogres('progres_bar');
        RESULT_EVENT_GENERAL = "YES";
        delete_alert_boot();
        e.disabled = true;
        INTERVAL_EVENT_GENERAL = setInterval(fx_funcion, 50);
        function fx_funcion() {
            //--Sale del evento
            if (ESTADO_EVENT_GENERAL == "out") {
                progres_hiden('progres_bar');
                e.disabled = false;
                if (RESULT_EVENT_GENERAL != "YES" && CONTROL_EVENT_GENERAL != "") {
                    alert_bot(RESULT_EVENT_GENERAL, 'warning', CONTROL_EVENT_GENERAL);
                }
                clearInterval(INTERVAL_EVENT_GENERAL);
                ESTADO_EVENT_GENERAL = "";
            }
            //--Entra al evento
            if (ESTADO_EVENT_GENERAL == "intro") {
                ESTADO_EVENT_GENERAL = "";
                if (e.target.id == "boton_event_adm_flujo_update_activity_description") {
                    service_update_row(ITEM_GENERAL_CONTROL_ARRAY, "WebServiceWorkflow.asmx",
                        "Service_Actualiza_descripcion_actividad_flujo_trabajo",
                        "ModalPopupExtender_edition_adm_flujo_update_activity_description", "modal_content_adm_flujo_update_activity_description","");
                    return true;
                }     
                progres_hiden('progres_bar');
                e.disabled = false;
                clearInterval(INTERVAL_EVENT_GENERAL);
                ESTADO_EVENT_GENERAL = "";
            }
        }
    }
    catch (ex) {
        alert('event_element_clic  ' + ex.message);
    }
}

//ACTIVA LOS EVENTOS PENDID CON INTERVAL
function event_element_menu(evento, tip_event) {
    try {
        ESTADO_EVENT_GENERAL = "intro";
        RESULT_EVENT_GENERAL = "YES";
        delete_alert_boot();
        posicion_update_pogres('progres_bar');
        INTERVAL_EVENT_GENERAL = setInterval(fx_funcion, 50);
        function fx_funcion() {
            //--Sale del evento  
            if (ESTADO_EVENT_GENERAL == "out") {      
                progres_hiden('progres_bar');
                if (RESULT_EVENT_GENERAL != "YES" && CONTROL_EVENT_GENERAL != "") {
                    alert_bot(RESULT_EVENT_GENERAL, 'warning', CONTROL_EVENT_GENERAL);
                }
                clearInterval(INTERVAL_EVENT_GENERAL);
                ESTADO_EVENT_GENERAL = "";        
            }
            //--Entra al evento
            if (ESTADO_EVENT_GENERAL == "intro") {
                ESTADO_EVENT_GENERAL = "";
                CONTROL_EVENT_GENERAL = "";
                RESULT_EVENT_GENERAL = "YES";
                //Ativa windows update activities workflow
                if (evento == "ME-UP-ACT-WF") {
                    CONTROL_EVENT_GENERAL = "modal_content_adm_flujo_update_activity_description";
                     html_form_ontrol(IDENT_ACTVITIE_WF, "WebServiceWorkflow.asmx", "Service_solicita_descripcion_tarea_actividad_flujo",
                        "ModalPopupExtender_edition_adm_flujo_update_activity_description",
                         "div_adm_flujo_update_activity_description", 1, "adm_flujo_update_activity_description", 0, "form_parent_content",
                         "id_modal_title", "Descripcion actividad " + NAME_ACTVITIE_WF,50);
                    return true;
                }  
                progres_hiden('progres_bar');
                clearInterval(INTERVAL_EVENT_GENERAL);
                ESTADO_EVENT_GENERAL = "";
            }
        }
    }
    catch (ex) {
        alert('event_element_menu  ' + ex.message);
    }
}
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
        ShowModalPopup("ModalPopupExtender_edition_registra_respon_flujo_backgroundElement", "Panel_registra_respon_flujo", 100001);
        ShowModalPopup("ModalPopupExtender_edition_adm_flujo_update_activity_description_backgroundElement", "Panel_adm_flujo_update_activity_description", 100001);
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
        auto_zise_ventana_diagrama();
        auto_zise_popup_lista_tareas("1");
        auto_zise_popup_paginas_externas_libres();
        auto_zise_popup_lista_form_control_person_procent("adm_flujo_update_activity_description", 70);
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
function preven_event_search(event, e) {
    try {
        document.getElementById("Button_buscar_lista").click();
        event.preventDefault();
    }
    catch (err) {
        alert(err.message + " Funcion preven_event_search");
    }
}
function preven_event_restor_search(event, e) {
    try {
        document.getElementById("Button_restore_lista_actividad").click();
        event.preventDefault();
    }
    catch (err) {
        alert(err.message + " Funcion preven_event_search");
    }
}
function ConfirmMensajeEliminar_user_resp() {
    try {
        var drowplist = document.getElementById("DropDownList_user_respon_flujo");
        document.getElementById("Hidden_res").value = 0;
        if (drowplist.selectedIndex == -1) {
            return;
        } else {
            var x = "";
            var r = confirm("Desea eliminar el usuario responsable ?");
            if (r == true) {
                x = "1";
            }
            else {
                x = "0";
            }
            document.getElementById("Hidden_res").value = x;
        }

    }
    catch (err) {
        alert(err.message + " ConfirmMensajeEliminar_user_resp");
    }
}
function diagranview_bloqued(sender, args) {
    try {
        if (args) {
            var link = args.getLink();
            var index = args.getAdjustmentHandle();
            if (index == 0 || index == link.getControlPoints().length - 1) {
                args.setCancel(true);
                args.cancelDrag();
            }
        }

        diagramView.addEventListener(MindFusion.Diagramming.Events.linkModifying, function (sender, args) {
            var link = args.getLink();
            var index = args.getAdjustmentHandle();
            if (index == 0 || index == link.getControlPoints().length - 1) {
                args.setCancel(true);
                args.cancelDrag();
            }
        });
    }
    catch (err) {
        alert(err.message + " Funcion diagranview_bloqued");
    }
}
function prevent(event, element) {
    try {      
        event.preventDefault();
        $('#data_grid tr[id]').css({ "background": "White", "color": "Black" });
        $('#data_grid tr[id]').each(function () {
            $(this).css({ "background-color": "#e8e8f7", "color": "Black" });
        });
        var g = element;
        var fer = $(element).attr("idd");
        $('#hdnEmailID').val(fer);
        document.getElementById("Button_agrega_actividad_flujo_trabajo").click();
        element.focus();
    }
    catch (err) {
        alert(err.message + " Funcion prevent");
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
function auto_zise_ventana_diagrama() {
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

        //$('#div_diagran').css("height", (espacio_iframe - 90) + "px");
        //$('#content').css("height", (espacio_iframe - 90) + "px");
        //var client_keigth = document.getElementById("header").clientHeight + document.getElementById("menucab").clientHeight;
        //$('#content').css("height", (espacio_iframe - (client_keigth - 40)) + "px");
        var client_keigth = document.getElementById("Menutol").clientHeight + document.getElementById("menucab").clientHeight + document.getElementById("footer").clientHeight;
        $('#content').css("height", (espacio_iframe - (client_keigth)) + "px");
        
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_ventana_diagrama");
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
        /* $('#Panel_paginas_externas_popou').css("height", (espacio_iframe) + "px");
         $('#paginas_externas_popou').css("height", (espacio_iframe) + "px");
         $('#Iframe_paginas_externas_popup_').css("height", (espacio_iframe) + "px");*/

        //corrige el posicionamiento horizontal del modalpopuextender, la propiedad width del panel debe estar en auto
        //var widtth_procent_left_rigth = (with_frame - document.getElementById("Panel_agregar_expediente_carpeta").clientHeight) / 2;
        //$('#Panel_agregar_expediente_carpeta').css("left", (Math.round(widtth_procent_left_rigth)) + "px");
        var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_paginas_externas_popou').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_Panel_paginas_externas_popou').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#contenido_procesa_paginas_externas_popou').css("height", (document.getElementById("modal_content_Panel_paginas_externas_popou").clientHeight - (document.getElementById("divcabecer2_paginas_externas_popou").clientHeight)) + "px");
        //Para los modal que contiene gred
        $('#Iframe_paginas_externas_popup_').css("height", (document.getElementById("contenido_procesa_paginas_externas_popou").clientHeight) + "px");

    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_paginas_externas_libres");
    }

}
function service_posibles_actividades() {
    function split(val) {
        return val.split(/,\s*/);
    }
    function extractLast(term) {
        return split(term).pop();
    }
    $("#TextBox_busqueda")
         .on("keydown", function (event) {
             if (event.keyCode === $.ui.keyCode.TAB &&
                 $(this).autocomplete("instance").menu.active) {
                 event.preventDefault();
             }
         })
        .autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: "../webservice/WebServiceWorkflow.asmx/Get_lista_actividades",
                    data: "{'DName':'" + document.getElementById("TextBox_busqueda").value + "'}",
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
                document.getElementById("TextBox_busqueda").value = ui.item.label;
                document.getElementById("Button_buscar_lista").click();
            }

            , minLength: 3, max: 10, scroll: true
        });
}
function nodeClicked(sender, args) {
    if (args.node) {
       // alert(args.node.id);
        IDENT_ACTVITIE_WF = args.node.id;
        NAME_ACTVITIE_WF = args.node.text.text;
    }
    
}
function onNodeSelected(sender, args) {
    try {
        if (event.ctrlKey) {
            if (document.getElementById("HiddenField_value_selecion").value == "") {
                document.getElementById("HiddenField_value_selecion").value = args.getNode().getText();
            } else {
                document.getElementById("HiddenField_value_selecion").value = document.getElementById("HiddenField_value_selecion").value + "|" + args.getNode().getText();
            }

        } else {
            //document.getElementById("HiddenField_value_selecion").value = args.getNode().getText();
            //var f = args.getNode();
            //alert(f.id);
        }
    }
    catch (err) {
        alert(err.message + " Funcion onNodeSelected");
    }
}
function onLinkSelected(sender, args) {
    try {
        if (event.ctrlKey) {
            if (document.getElementById("HiddenField_value_selecion").value == "") {
                document.getElementById("HiddenField_value_selecion").value = args.getNode().getText();
            } else {
                document.getElementById("HiddenField_value_selecion").value = document.getElementById("HiddenField_value_selecion").value + "|" + args.getNode().getText();
            }

        } else {
            //document.getElementById("HiddenField_value_selecion").value = args.getNode().getText();
            //var f = args.link;
            //alert(f.id);
        }
    }
    catch (err) {
        alert(err.message + " Funcion onLinkSelected");
    }
}
$("#content").keypress(function (event) {
    event.preventDefault();
});
function onNodeClicked(sender, args) {
    if (sender == null)
        return false;

    var node = args.getNode();
    var button = node.getButtonAtPoint(args.getMousePosition());
    if (button == 'switch') {
        node.setNodeType(node.getNodeType() < 3 ? node.getNodeType() + 1 : 1);
    }
    else if (button == 'close') {
        node.parent.removeItem(node);
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
function onNodeSelected(sender, args) {
    //console.log(args.getNode().getText() + " selected");
    if (event.ctrlKey) {
        if (document.getElementById("HiddenField_value_selecion").value == "") {
            document.getElementById("HiddenField_value_selecion").value = args.getNode().id;
            document.getElementById("ImageButtonGuardar").src = "../workflow/imageneswf/Guardar_actividad.png";
        } else {
            document.getElementById("HiddenField_value_selecion").value = document.getElementById("HiddenField_value_selecion").value + "|" + args.getNode().id;
            document.getElementById("ImageButtonGuardar").src = "../workflow/imageneswf/Guardar_actividad.png";
        }

        //alert(event.ctrlKey);
    } else {
        document.getElementById("HiddenField_value_selecion").value = args.getNode().id;
        document.getElementById("ImageButtonGuardar").src = "../workflow/imageneswf/Guardar_actividad.png";
        var f = args.getNode();
        //alert(f.id);

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
        $('#Panel_lista_actividades_worflow').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_Panel_lista_actividades_worflow').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#contenido_procesa_lista_actividades_workflow').css("height", (document.getElementById("modal_content_Panel_lista_actividades_worflow").clientHeight - (document.getElementById("divcabecer2_lista_actividades_worflow").clientHeight)) + "px");
        //Para los modal que contiene gred
        $('#panel_conten_gred').css("height", (document.getElementById("contenido_procesa_lista_actividades_workflow").clientHeight - (document.getElementById("contenido_titulo_data_grid_dos_title").clientHeight + 50)) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_lista_tareas " + err.message);
    }
}