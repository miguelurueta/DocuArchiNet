$(document).ready(function () {
   

    $.fn.auto_postback = function (data_grid_id, email_selector_id) {
        clired();
       
        if (ESTATE == 0) {
            ESTATE = 1;
            auto_zise();
            control_visualiza_boton_asignacion_expediente();
            auto_zise_reasigna_expe_unidad();
            auto_zise_ubicacion_toponimica();
            auto_zise_agregar_unidad_conservacion();
            auto_zise_agregar_expediente();
            auto_zise_ubicacion_toponimica();
        }
        
        
    };
});
var ESTATE = 0;
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
        auto_zise();
        auto_zise_agregar_unidad_conservacion();
        auto_zise_agregar_expediente();
        auto_zise_ubicacion_toponimica();
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
function auto_zise_ubicacion_toponimica() {
    try {
        var espacio_iframe;
        var hidenpadre;
        var with_frame;
        if (window.innerHeight) {
            //navegadores basados en mozilla 
            espacio_iframe = window.innerHeight
        } else {
            if (document.body.clientHeight) {
                //Navegadores basados en IExplorer, es que no tengo innerheight 
                with_frame = window.innerWidth;
                espacio_iframe = window.innerHeight;
            } else {
                //otros navegadores y iframe
                //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val();

                espacio_iframe = document.body.clientHeight;
                with_frame = document.body.clientWidth;

            }
        }

        /*$("#Panel_ubicacion_toponimica_expediente_popup").css("height", (espacio_iframe - 5) + "px");
        $("#Contenido_ubicacion_toponimica_expediente").css("height", (espacio_iframe - 5) + "px");
        var heigconetedor = 0;
        $("#contendor_botones_unidad_u_b_t").css("height", ($("#Button_exportar").height() + 15) + "px");
        heigconetedor = $("#Panel_ubicacion_toponimica_expediente_popup").height() - ($("#contendor_botones_unidad_u_b_t").height());
        $("#div_treview_archivo_u_b_t").css("height", (heigconetedor) + "px");
        $("#Paneltreview_u_b_t").css("height", (heigconetedor) + "px");*/

        //corrige el posicionamiento horizontal del modalpopuextender, la propiedad width del panel debe estar en auto
        //var widtth_procent_left_rigth = (with_frame - document.getElementById("Panel_agregar_expediente_carpeta").clientHeight) / 2;
        //$('#Panel_agregar_expediente_carpeta').css("left", (Math.round(widtth_procent_left_rigth)) + "px");
        var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_ubicacion_toponimica_expediente_popup').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_Panel_ubicacion').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#Contenido_ubicacion_toponimica_expediente').css("height", (document.getElementById("modal_content_Panel_ubicacion").clientHeight - (document.getElementById("divcabecer_ubicacion_toponimica_expediente_popup").clientHeight + document.getElementById("contendor_botones_unidad_u_b_t").clientHeight + 30)) + "px");
        //Para los modal que contiene gred
        $('#Paneltreview_u_b_t').css("height", (document.getElementById("Contenido_ubicacion_toponimica_expediente").clientHeight ) + "px");
        $('#TreeViewArchivo_u_b_t').css("height", (document.getElementById("Contenido_ubicacion_toponimica_expediente").clientHeight ) + "px");
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_reasigna_expe_unidad");
    }
}
function auto_zise_agregar_unidad_conservacion() {
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

        $('#Panel_agregar_unidad_conservacion').css("height", (espacio_iframe - 1) + "px");
        $('#Contenido_agregar_unidad_conservacion').css("height", (espacio_iframe - 1) + "px");
        $('#Iframe_agregar_unidad_conservacion_popup_').css("height", (espacio_iframe - 5) + "px");
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_agregar_unidad_conservacion");
    }
}
function auto_zise_agregar_expediente() {
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

        $('#Panel_agregar_expdiente_popup').css("height", (espacio_iframe  - 5) + "px");
        $('#Contenido_agregar_expdiente_popup').css("height", (espacio_iframe - 5  ) + "px");
        $('#Iframe_agregar_expdiente_popup_').css("height", (espacio_iframe - 7) + "px");
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_agregar_expediente");
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
function auto_zise_reasigna_expe_unidad() {
    try {
        var espacio_iframe;
        var hidenpadre;
        var with_frame;
        if (window.innerHeight) {
            //navegadores basados en mozilla 
            espacio_iframe = window.innerHeight
        } else {
            if (document.body.clientHeight) {
                //Navegadores basados en IExplorer, es que no tengo innerheight 
                with_frame = window.innerWidth;
                espacio_iframe = window.innerHeight;
            } else {
                //otros navegadores y iframe
                //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val();

                espacio_iframe = document.body.clientHeight;
                with_frame = document.body.clientWidth;

            }
        }


        $(document).ready(bodyResize);
        $(window).resize(bodyResize);
        function bodyResize() {
            $("#Panel_reubicar_unidad_expediente_popup").css("height", (espacio_iframe - 40) + "px");
            $("#Contenido_reubicar_unidad_expediente_popup").css("height", (espacio_iframe - 40) + "px");
            var heigconetedor = 0;
            $("#drowlist_r_u_e").css("height", $("#DropDownListEntidadEmpresa_r_u_e").height() + "px");
            $("#contendor_botones_unidad_r_u_e").css("height", ($("#Button_archivar").height() + 5) + "px");
            heigconetedor = $("#Panel_reubicar_unidad_expediente_popup").height() - ($("#drowlist_r_u_e").height() + $("#contendor_botones_unidad_r_u_e").height());
            $("#div_treview_archivo_r_u_e").css("height", (heigconetedor) + "px");
            $("#Paneltreview_r_u_e").css("height", (heigconetedor) + "px");

        }
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_reasigna_expe_unidad");
    }
}
//Funcion que asigna los expedintes creados desde el popup agreagar expediete
function cerrar_popup_agregar_expedinte() {
    $('#Button_actualiza_expdientes_agregados').click();
}
//Realiza consulta cuando cambia la empresa
function cambio_empresa_gestion_consulta() {
    $('#ButtonConsulta').click();
}
function clired() {
    $('#data_grid tr[id]').click(function () {
        $('#data_grid tr[id]').css({ "background-color": "White", "color": "Black" });
        $(this).css({ "background-color": "#E7EDF5", "color": "Black" });
        var fer = $(this).attr("id");
        $('#hdnEmailID').val(fer);

    });

    //Mantenie el cursor de seleccion
    $('#data_grid tr[id]').mouseover(function () {
        $(this).css({ cursor: "hand", cursor: "pointer" });
    });
    //Mantiene seleccinado en todos los postback la celda del data grid de lado del cliente
    $('#data_grid tr[id=' + $('#hdnEmailID').val() + ']').css({ "background-color": "#E7EDF5", "color": "Black" });

}

/* funcion CONTROLA QUE EL BOTON DE ASIGNACION DE EXPEDIENTE SE VEAN SI ESTA EN UN PAREN O NO, requiere control
Hiddenheigpaginapopup,Hiddennameasigna,Hiddenid_expediente en el foumulario fuente
*/
function control_visualiza_boton_asignacion_expediente() {
    try {
        $('#Button_asigna_expediente_gestion').hide();
        var hidenvalue = $('#Hiddennameasigna', window.parent.document).val();
        if (hidenvalue != undefined) {
            $('#Hiddennameasigna').val($('#Hiddennameasigna', window.parent.document).val());
            if (hidenvalue == "") {
                $('#Button_asigna_expediente_gestion').hide();
            } else {
                $('#Button_asigna_expediente_gestion').show();
            }

        } else {
            $('#Button_asigna_expediente_gestion').hide();

        }
    }
    catch (err) {
        alert(err.message + " Funcion control_visualiza_boton_asignacion_expediente");
    }
}
/* funcion que determina a que pagina asigna el id del expediente, requiere control
Hiddenheigpaginapopup,Hiddennameasigna,Hiddenid_expediente en el foumulario fuente
*/
function importa_dato_expediente() {
    try {
        var namepaginapopup = $('#Hiddennameasigna', window.parent.document);
        if (namepaginapopup == undefined) {
            alert("Imposible encontrar el tipo seleccion en en la pagina fuente fal el control Hiddenheigpaginapopup");
            return false;
        }
        if (namepaginapopup.val() == "RADICACION_ENTRANTE") {
            asigna_expdiente_radicacion_entrante();
            return false;
        }

        if (namepaginapopup.val() == "EXPEDIENTE_WORKFLOW") {
            asigna_expediente_workflow_indice();
            //asigna_expdiente_radicacion_entrante();
            //return false;
        }
        if (namepaginapopup.val() == "DOCUARCHI_NET") {
            asigna_expediente_workflow_indice();
            return false;
        }
        if (namepaginapopup.val() == "RADICACION_SALIENTE") {
            //asigna_expdiente_radicacion_entrante();
            //return false;
        }

    }
    catch (err) {
        alert(err.message + " Funcion importa_dato_expediente");
    }
}
function asigna_expediente_workflow_indice() {
    try {
        var modal_popup = $('#Buttoncerrar_expdiente_popup', window.parent.document);
        var hiden_expediente = $('#Hidden_id_expediente', window.parent.document);
        var text_box_expediente = $('#EXPEDIENTE', window.parent.document);
        if (hiden_expediente == undefined) {
            alert("Imposible encontrar el control Hidden_id_expediente en el indice workflow");
            return false;
        }

        if (text_box_expediente == undefined) {
            alert("Imposible encontrar el control EXPEDIENTE en el indice workflow");
            return false;
        }

        if ($('#hdnEmailID').val() == "0" || $('#hdnEmailID').val() == "-1") {
            alert("Debe seleccionar un expediente");
            return false;
        }
        var sele_row = $('#data_grid tr[id=' + $('#hdnEmailID').val() + ']');
        var columindex = colum_index("ESTADO_EXPEDIENTE");
        if (columindex == -1) {
            alert("Imposible encontrar el index de la columna ESTADO_EXPEDIENTE");
            return false;
        }

        if (sele_row[0].cells[columindex].innerText != "1") {
            alert("El expediente esta cerrado, no se puede asignar");
            return false;
        }
        hiden_expediente.val($('#hdnEmailID').val());
        text_box_expediente.val(sele_row[0].cells[1].innerText);
        modal_popup.click();
    }
    catch (err) {
        alert(err.message + " Funcion asigna_expediente_workflow_indice");
    }
}
function asigna_expdiente_radicacion_entrante() {
    try {
        var modal_popup = $('#Button_cierra_popup_expediente', window.parent.document);
        var textbox_expediente = $('#Textbox_expediente_val_radicacion', window.parent.document);
        var hiden_id_expediente = $('#Hiddenid_expediente', window.parent.document);
        var Check_anexo_radicado = $('#Check_anexo_radicado', window.parent.document)
        var CheckBox_relacionado_radicado = $('#CheckBox_relacionado_radicado', window.parent.document)
        var check_nuevo_radicado = $('#check_nuevo_radicado', window.parent.document)
        if (hiden_id_expediente == undefined) {
            alert("Imposible encontrar el hiden del del expediente en la pagina radicación entrante");
            return false;
        }
        if (Check_anexo_radicado == undefined) {
            alert("Imposible encontrar Check_anexo_radicado en la pagina radicación entrante");
            return false;
        }
        if (textbox_expediente == undefined) {
            alert("Imposible encontrar el textbox del expediente en la pagina radicación entrante");
            return false;
        }
        if (modal_popup != undefined) {
            if ($('#hdnEmailID').val() == "0" || $('#hdnEmailID').val() == "-1") {
                alert("Debe seleccionar un expediente");
                return false;
            }
            var sele_row = $('#data_grid tr[id=' + $('#hdnEmailID').val() + ']');
            var columindex = colum_index("ESTADO_EXPEDIENTE");
            if (columindex == -1) {
                alert("Imposible encontrar el index de la columna ESTADO_EXPEDIENTE");
                return false;
            }

            if (sele_row[0].cells[columindex].innerText != "1") {
                alert("El expediente esta cerrado, no se puede asignar");
                return false;
            }
            hiden_id_expediente.val($('#hdnEmailID').val());
            //Check_anexo_radicado[0].checked = true;
            //CheckBox_relacionado_radicado[0].checked = false;
            //check_nuevo_radicado[0].checked = false;
            textbox_expediente.val(sele_row[0].cells[1].innerText);
            modal_popup.click();

        }

    }
    catch (err) {
        alert(err.message + " Funcion asigna_expdiente_radicacion_entrante");
    }
}
//Retorna el idex de una columna en una tabla
function colum_index(colum_name) {
    try {
        var x = $('#data_grid th');
        var txt = "";
        var i;
        for (i = 0; i < x.length; i++) {
            if (x[i].innerText == colum_name) {

                return i;
            }

        }
        return -1;
    }
    catch (err) {
        alert(err.message + " Funcion colum_index");
    }
}
//Asigna tamaño ventana agregar expediente
function tamano_ventana_agregar_expediente() {
    try {
        $("#Hiddenheigpagina").val(($("#contendor_principal").height()));
        var label = $('#Label_agregar_expdiente_popup');
        label[0].innerText = "Agregar unidad";
        // $("#Iframe_agregar_expdiente_popup_").attr("src", "../gestion/FormGaAgregarExpediente.aspx")
    }
    catch (err) {
        alert(err.message + " Funcion tamano_ventana_agregar_expediente");
    }
}
//Asigna tamaño ventana editar expediente
function tamano_ventana_editar_expediente() {
    try {
        $("#Hiddenheigpagina").val(($("#contendor_principal").height()));
        var label = $('#Label_agregar_expdiente_popup');
        label[0].innerText = "Editar unidad documental";
        // $("#Iframe_agregar_expdiente_popup_").attr("src", "../gestion/FormGaAgregarExpediente.aspx")
    }
    catch (err) {
        alert(err.message + " Funcion tamano_ventana_editar_expediente");
    }
}
//Asigna tamaño ventana nuevo volumen expediente
function tamano_ventana_nuevo_volumen_expediente() {
    try {
        $("#Hiddenheigpagina").val(($("#contendor_principal").height()));
        var label = $('#Label_agregar_expdiente_popup');
        label[0].innerText = "Nuevo volumen unidad documental";
        // $("#Iframe_agregar_expdiente_popup_").attr("src", "../gestion/FormGaAgregarExpediente.aspx")
    }
    catch (err) {
        alert(err.message + " Funcion tamano_ventana_nuevo_volumen_expediente");
    }
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
function auto_zise() {
    try {
        var espacio_iframe;
        var hidenpadre;
        var with_frame;
        if (window.innerHeight) {
            //navegadores basados en mozilla 
            espacio_iframe = window.innerHeight
        } else {
            if (document.body.clientHeight) {
                //Navegadores basados en IExplorer, es que no tengo innerheight 
                with_frame = window.innerWidth;
                espacio_iframe = window.innerHeight;
            } else {
                //otros navegadores y iframe
                //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val();
                espacio_iframe = document.body.clientHeight;
                with_frame = document.body.clientWidth;

            }
        }


        hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val();
        if (hidenpadre != undefined) {
            var tempo = $('#Hiddenheigpaginapopup', window.parent.document).val();
            if (tempo != "0") {
                //espacio_iframe = (parseInt(tempo) - 1);
            }
        }
        var espacio_frame_genral = (espacio_iframe - 20) - $("#menu_var").height();
        $("body").css("height", (espacio_iframe - 20) + "px");
        $("#contendor_principal").css("height", (espacio_iframe - 20) + "px");
        $("#Contenedorderecho").css("height", (espacio_frame_genral) + "px");
        $("#formGaGestionExpediente").css("height", (espacio_iframe - 20) + "px");
        //$("#Panelbuton").css("height", $("#ButtonConsulta").height() + 10 + "px");
        $("#Panelcampos").css("height", (espacio_frame_genral) - ($("#Panelbuton").height() + document.getElementById("DropDownListEntidadEmpresa").clientHeight + 20) + "px");
        $("#contenido_botonoes").css("height", $("#Button_nuevo_expediente_gestion").height() + 10 + "px");
        $("#Contenedorgrid").css("height", (espacio_frame_genral)  + "px");
        $("#Panelactividad").css("height", (espacio_frame_genral) - ($("#contenido_titulo_val_radicacion").height() + 20) + "px");
       
        $("#contendor_principal").css("width", (with_frame - 5) + "px");
        $("#Contenedorderecho").css("width", (with_frame - 5) + "px");
        $("#formGaGestionExpediente").css("width", (with_frame - 5) + "px");
        $("#Panelcampos").css("width", (with_frame - 5) + "px");
        $("#Panelactividad").css("width", (with_frame - 5) + "px");
        $("#Panelbuton").css("width", (with_frame - 5) + "px");
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise");
    }
}

function eliminar_fila_data_gred(gred) {
    try {

        $("#" + gred + " tr[id=" + $("#hdnEmailID").val() + "]").remove();
        $('#hdnEmailID').val("-1");

    }
    catch (err) {
        alert(err.message + " Funcion eliminar_fila_data_gred");
    }

}
function onDataShown(sender, args) {
    sender._popupBehavior._element.style.zIndex = 1000001;
    //sender._popupBehavior._element.style.left = "54px";//set positions according to your requriment.
    //sender._popupBehavior._element.style.top = "50px";//set top postion accorind to you requirement.

    //you can either use left,top or right,bottom or any combination u want to set ur divlist.            
}
function progres_hiden(progres) {
    $("#progres_bar").css("display", "none");
}
function pront_confirmacion(mensaje) {
    try {
        var men = confirm(mensaje);
        if (men) {
            document.getElementById("HiddenField_botones_respuesta").value = "1";
        } else {
            document.getElementById("HiddenField_botones_respuesta").value = "0";
        }
    }
    catch (err) {
        alert(err.message + " funcion pront_confirmacion " + err.message);
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



