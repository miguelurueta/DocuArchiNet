$(document).ready(function () {
    $.fn.inicio = function () {    
        $('#GridView_val_radicacion tr[id]').click(function () {
            $('#GridView_val_radicacion tr[id]').css({ "background": "White", "color": "Black" });
            $(this).css({ "background-color": "#e8e8f7", "color": "Black" });
            var fer = $(this).attr("id");
            $('#hdnEmailID_VAL').val(fer);
        });         
        $('#GridView_val_radicacion tr[id]').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });        
        auto_zise_popup_validacion_radicados();
        auto_zise_edit_remitente();
        //auto_zise_popup_lista_form_control_person("registro_validacion_externo");
        auto_zise_popup_lista_form_control_person_procent('div_registro_validacion_externo', 30);
    }
})
$(window).on("load", function () {
    try {
        ini_event_page();
        var elment = document.getElementsByClassName("da_event_captive");
        if (elment) {
            for (var i = 0; i < elment.length; i++) {
                elment[i].addEventListener("click", event_click, false);
            }
        }
        window.addEventListener("resize", rezize_event);
        ShowModalPopup("ModalPopupExtender_mensaje_personalizado_backgroundElement", "Panel_mensaje_personalizado", 100001);
        ShowModalPopup("ModalPopupExtender_edition_registro_validacion_externo_backgroundElement", "Panel_registro_validacion_externo", 100001);
    } catch (e) {
        alert(" funcion load " + e.message);
    }

});
const ini_event_page = () => {
    //Active event update activities workflow
    let array_element = new Array;
    array_element.push({ id: "boton_event_registro_validacion_externo" }, { id: "boton_event_update_validacion_externo"});
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
            //Evento registra tercero o remitente externo
            case "boton_event_registro_validacion_externo":          
                result = valida_solicita_datos_control_general("registro_validacion_externo");
                if (result != "YES") {
                    alert_bot(result, 'warning', "modal_content_registro_validacion_externo");
                } else {
                    event_element_clic("", e)
                }
                break;
            //Evento actualiza tercero o remitente externo
            case "boton_event_update_validacion_externo" :
                  result = valida_solicita_datos_control_general("registro_validacion_externo");
                if (result != "YES") {
                    alert_bot(result, 'warning', "modal_content_registro_validacion_externo");
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
        INTERVAL_EVENT_GENERAL = setInterval(fx_funcion, 30);
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
                //Evento registra tercero o remitente externo
                if (e.target.id == "boton_event_registro_validacion_externo") {
                    service_insert_row(ITEM_GENERAL_CONTROL_ARRAY, "WebService_plantilla_validacion.asmx",
                        "Service_registra_tercero_plantilla_validacion",
                        "ModalPopupExtender_edition_registro_validacion_externo", "modal_content_registro_validacion_externo","GridView_val_radicacion");
                    return true;
                }
                //Evento actualiza tercero o remitente externo
                if (e.target.id == "boton_event_update_validacion_externo") {
                    service_update_row(ITEM_GENERAL_CONTROL_ARRAY, "WebService_plantilla_validacion.asmx",
                        "Service_update_tercero_plantilla_validacion",
                        "ModalPopupExtender_edition_registro_validacion_externo", "modal_content_registro_validacion_externo", "GridView_val_radicacion");
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
const event_element_menu = (evento, value) => {
    try {
        ESTADO_EVENT_GENERAL = "intro";
        posicion_update_pogres('progres_bar');
        INTERVAL_EVENT_GENERAL = setInterval(fx_funcion, 30);
        function fx_funcion() {
            //--Sale del evento  
            if (ESTADO_EVENT_GENERAL == "out") {
                progres_hiden('progres_bar');
                clearInterval(INTERVAL_EVENT_GENERAL);
                ESTADO_EVENT_GENERAL = "";

            }
            //--Entra al evento
            if (ESTADO_EVENT_GENERAL == "intro") {
                ESTADO_EVENT_GENERAL = "";
                
                if (evento == "elimina_reg_usuario") {
                    let array_config_delete = new Array;
                    array_config_delete.push({ table_html: "GridView_val_radicacion" ,  identy_row: value ,  control_seting: "hdnEmailID_VAL" ,
                         control_title: "titulo_label_validacion" });
                    service_delete_row(array_config_delete, "WebService_plantilla_validacion.asmx", "Service_delete_tercero_plantilla_validacion", "","content_error");
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
        auto_zise_popup_validacion_radicados();
        auto_zise_edit_remitente();
        //auto_zise_popup_lista_form_control_person("registro_validacion_externo");
        auto_zise_popup_lista_form_control_person_procent('div_registro_validacion_externo', 30);
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
function activa_menu(clave) {
    try {
        if (clave == "i_r_r_001") {
           
            document.getElementById("boton_event_registro_validacion_externo").style.display = "block";
            document.getElementById("boton_event_update_validacion_externo").style.display = "none";
            html_form_ontrol(0, "WebService_plantilla_validacion.asmx", "Service_solicita_estructura_formulario_registro_validacion_externo",
               "ModalPopupExtender_edition_registro_validacion_externo", "div_registro_validacion_externo", 0, "registro_validacion_externo",
                0, "content_error","label_title_registro_validacion_externo","Registro de remitente",30);
        }
    } catch (err) {
        alert(err.message + " funcion activa_menu " + err.message);
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
        if (tip_event == "asigna_registro_usuario") {
            $('#hdnEmailID_VAL').val(fer);
            document.getElementById("Button_Asigna").click();
        }
       
        if (tip_event == "edita_reg_usuario") {
            $('#hdnEmailID_VAL').val(fer);
            document.getElementById("boton_event_registro_validacion_externo").style.display = "none";
            document.getElementById("boton_event_update_validacion_externo").style.display = "block";
            html_form_ontrol(fer, "WebService_plantilla_validacion.asmx", "Service_solicita_estructura_formulario_registro_validacion_externo",
               "ModalPopupExtender_edition_registro_validacion_externo", "div_registro_validacion_externo", 1, "registro_validacion_externo",
               0, "content_error", "label_title_registro_validacion_externo", "Editar remitente", 30);
            //document.getElementById("Editar_pre").click();
        }
        if (tip_event == "elimina_reg_usuario") {
            var r = confirm("Desea eiminar el registro");
            if (r == true) {
                $('#hdnEmailID_VAL').val(fer);
                event_element_menu("elimina_reg_usuario", fer);
            }
            
            //document.getElementById("Eliminar").click();
        }
        event.preventDefault();
        element.focus();
    }
    catch (err) {
        alert(err.message + " Funcion prevent");
    }
}
//para eliminar
function eliminar_fila_data_gred() {
    try {
        $('#GridView_val_radicacion tr[id=' + $('#hdnEmailID_VAL').val() + ']').remove();
        $('#hdnEmailID_VAL').val("-1");
        var chid = $('#GridView_val_radicacion >tbody >tr').length;
        if (chid >= 1) {
            chid = chid - 1;
        }
        var iff = document.forms.item(0).id;
        if (document.forms.item(0).id == "form1") {
            document.getElementById("titulo_label_validacion").innerHTML = "Se encontraron " + chid + " registro(s) en la plantilla ";
        } else {
            document.getElementById("titulo_label_validacion").innerHTML = "Se encontraron " + chid + " registro(s) en la plantilla ";
        }
    }
    catch (err) {
        alert(err.message + " Funcion eliminar_fila_data_gred");
    }

}
function ejecuta_ecript_consulta() {
    try {
        document.getElementById("Button_ejecutar_consulta").click();
    }
   
    catch (err) {
        alert(err.message + " Funcion ejecuta_ecript_consulta");
    }
}
function selecciona_item_registrado() {
    try {
    $('#GridView_val_radicacion tr[id]').css({ "background": "White", "color": "Black" });
    $('#GridView_val_radicacion tr[id=' + $('#hdnEmailID_VAL').val() + ']').css({ "background-color": "#e8e8f7", "color": "Black" });
}
    catch (err) {
        alert(err.message + " Funcion selecciona_item_registrado");
}
}
function display_edit() {
    try {
        document.getElementById("Button_edita_campos_dinamicos").style.display = "block";
        document.getElementById("Button_registrar").style.display = "none";

    }
    catch (err) {
        alert(err.message + " Funcion display_edit_registrar");
        }
}
function display_agregar() {
    try {
        document.getElementById("Button_edita_campos_dinamicos").style.display = "none";
        document.getElementById("Button_registrar").style.display = "block";

    }
    catch (err) {
        alert(err.message + " Funcion display_edit_registrar");
    }
}
function ConfirmMensajeEliminar(mensaje) {
    try {
        var x = 1;
        document.getElementById("HiddenPROMP").value = x;
        var t = document.getElementById("hdnEmailID_VAL").value;
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
function retorna_campo_valor_nombre_destinatario() {
    try {
        var text_box_nombre = $('#TextBox_dext_externo', window.parent.document);
    $("#GridView_val_radicacion tr[id=" + $("#hdnEmailID_VAL").val() + "]").each(function () {
        idex = colum_index("Nombre_Remitente");
        if (idex != -1) {
            if ($(this)[0].cells[idex].children.length > 0) {
                text_box_nombre[0].value = $(this)[0].cells[idex].children[0].innerText;
                //return nombre_val;
            } else {
                text_box_nombre[0].value = $(this)[0].cells[idex].innerText;
                //return nombre_val;

            }
        }
    })
}
    catch (err) {
        alert(err.message + " Funcion retorna_campo_valor_nombre_destinatario");
}
}
function actualiza_datos_data_gredview(values_campos,values_datos) {
    try {
        var ref_values_campos = values_campos.split("¬");
        var re_values_datos = values_datos.split("¬");
        $("#GridView_val_radicacion tr[id=" + $("#hdnEmailID_VAL").val() + "]").each(function () {
            for (i = 0; i <= ref_values_campos.length - 1 ; i++) {
                var idex = -1;
                var text = re_values_datos[i];
                idex = colum_index(ref_values_campos[i]);
                if (idex != -1) {
                    $(this)[0].cells[idex].innerText = text;                  
                }
            }

        })
        values_campos="";
        values_datos="";
    }
    catch (err) {
        alert(err.message + " Funcion actualiza_datos_data_gredview");
    }
}
function colum_index(colum_name) {
    try {
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
    catch (err) {
        alert(err.message + " Funcion colum_index");
    }
}
function value_scrool() {
    document.getElementById("Hidden_scroll").value = $("#Panel_campos_consuta").scrollTop();
    
    //Hidden_scroll_gred
    //alert(document.getElementById("Hidden_scroll").value);
}
function mueve_scroll_value(valor, panel) {
    try {

        $("#" + panel).scrollTop(document.getElementById("Hidden_scroll").value);

    }
    catch (err) {
        alert(err.message + " Funcion mueve_scroll_value");
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
        //DETERMINA SI EL FORMULARIO PADRE ES EDITAR DE RADICADO
        var heig = 0;
        var wit = 0;
        var refheiht = $('#Hidden_height', window.parent.document);
        if (refheiht !== null) {
            if (refheiht.length == 0) {
                document.getElementById("Hidden_asig_").value = "NO";
            } else {
                document.getElementById("Hidden_asig_").value = "YES";       
            }
        }
    
        $('#Contentizquierdo').css("height", ((espacio_iframe - 5) - document.getElementById("menu_var").clientHeight) + "px");
        $('#sidebar_').css("height", ((espacio_iframe - 5) - document.getElementById("menu_var").clientHeight) + "px");
        $("#contenido_controles_consulta").css("height", (document.getElementById("Contentizquierdo").clientHeight - (document.getElementById('contenido_titulo_controles_consulta').clientHeight + document.getElementById('contenido_controles_buton_consulta').clientHeight)) + "px");
        $("#_Panelvalidacion").css("height", (document.getElementById("Contentizquierdo").clientHeight - (document.getElementById('contenido_titulo_controles_consulta').clientHeight + document.getElementById('contenido_controles_buton_consulta').clientHeight)) + "px");
        $('#Contenedorderecho').css("height", ((espacio_iframe - 5) - document.getElementById("menu_var").clientHeight) + "px");
        $("#contenido_datagrid_val_radicacion").css("height", (document.getElementById("Contenedorderecho").clientHeight - document.getElementById('contenido_titulo_val_radicacion').clientHeight) + "px");
   
    } catch (ex) { alert("funcion auto_zise_popup_validacion_radicados " + ex.mensaje); }
}
function auto_zise_edit_remitente() {
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
        var heig_porcent = espacio_iframe - 10;  // Indica el porcentaje de espacio vertical del elemento
        $('#panel_edita_campos_dinamicos').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_edita_campos_dinamicos').css("height", (heig_porcent - 5) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#content_general').css("height", (document.getElementById("modal_content_edita_campos_dinamicos").clientHeight - (document.getElementById("divcabecer2_edita_campos_dinamicos").clientHeight + document.getElementById("botones_edita_campos_dinamicos").clientHeight)) + "px");
       
        $('#Panel_dinamico_edita_campos_dinamicos').css("height", (document.getElementById("content_general").clientHeight - 5) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_usuarios_relacionados " + err.message);
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
function onDataShown(sender, args) {
    sender._popupBehavior._element.style.zIndex = 1000001;

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
function llenardepartamento_edit() {
    try {
    var drowplist = document.getElementById("EDIT_PAIS");
    var idsel = document.getElementById("Hiddenselecionpais_EDIT");
    if (drowplist.selectedIndex != -1) {
        idsel.value = drowplist.options[drowplist.selectedIndex].text;
        var boton = document.getElementById("Buttonllenardepartamento_edit");
        var idsel2 = document.getElementById("Hiddenseleciondepartamento_EDIT");
        idsel2.value = "";
        boton.click();
    }
    }
    catch (err) {
        alert(err.message + " Funcion llenardepartamento_edit");
    }
}
function llenarciudad_edit() {
    try {
    var drowplist = document.getElementById("EDIT_DEPARTEMENTO");
    var idsel = document.getElementById("Hiddenseleciondepartamento_EDIT");
    if (drowplist.selectedIndex != -1) {
        idsel.value = drowplist.options[drowplist.selectedIndex].text;
        var boton = document.getElementById("Buttonllenarciudad_edit");
        boton.click();
    }
    }
    catch (err) {
        alert(err.message + " Funcion llenarciudad_edit");
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
function seleccionmuicipio_edit() {
    try {
    var drowplist = document.getElementById("EDIT_MUNICIPIO");
    var idsel = document.getElementById("Hiddenmunicipio_EDIT");
    if (drowplist.selectedIndex != -1) {
        idsel.value = drowplist.options[drowplist.selectedIndex].text;
    }
    }
    catch (err) {
        alert(err.message + " Funcion seleccionmuicipio_edit");
    }
}
function seleccionmuicipio() {
    try {
    var drowplist = document.getElementById("MUNICIPIO");
    var  idsel = document.getElementById("Hiddenmunicipio");
    if (drowplist.selectedIndex != -1) {
        idsel.value = drowplist.options[drowplist.selectedIndex].text;
    }
    }
    catch (err) {
        alert(err.message + " Funcion seleccionmuicipio");
    }
}
function asigna_registro_validacion() {
    try {
        var hiden_parent = $('#Hidden_remitente_destinatario', window.parent.document);
        var text_box_nombre = $('#TextBox_dext_externo', window.parent.document);
        var boton = $('#Button_Asigana_datos_validacion_edicion', window.parent.document);
        var boton_asigna = $('#asignar', window.parent.document);
        var hiden_parent_asigna = $('#hdnEmailID', window.parent.document);
        if (boton_asigna != undefined) {
            if (hiden_parent_asigna === undefined) {
                alert("Imposible econtrar el control parent hdnEmailID");
                return false;
            } else {
                if ($('#hdnEmailID_VAL').val() == "-1" || $('#hdnEmailID_VAL').val() == "") {
                    alert("debe seleccionar el registro a asignar");
                    return false;
                } else {
                    hiden_parent_asigna.val($('#hdnEmailID_VAL').val());              
                    boton_asigna.click();
                }
            }
        }
    if (boton != undefined) {
        //alert("Imposible econtrar el control parent Button_Asigana_datos_validacion_edicion");
        //return false;
        if (hiden_parent === undefined) {
            alert("Imposible econtrar el control parent Hidden_remitente_destinatario o TextBox_dext_externo");
            return false;
        } else {
            if ($('#hdnEmailID_VAL').val() == "-1" || $('#hdnEmailID_VAL').val() == "") {
                alert("debe seleccionar el registro a asignar");
                return false;
            } else {
                hiden_parent.val($('#hdnEmailID_VAL').val());
                if (text_box_nombre[0] !== undefined) {
                    retorna_campo_valor_nombre_destinatario();

                }
                boton.click();
            }
        }
    }
   
    }
    catch (err) {
        alert(err.message + " Funcion asigna_registro_validacion");
    }
}
