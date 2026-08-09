$(document).ready(function () {
    $.fn.inicio = function () {
        auto_zise_ventana_diagrama();
        auto_zise_popup_paginas_externas_libres();
        diagranview_bloqued();
        auto_zise_popup_agregar_organigrama();
        auto_zise_popup_editar_organigrama();
        auto_zise_popup_agregar_area_departamento();
        auto_zise_popup_editar_area_departamento();
    }


});
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
        auto_zise_ventana_diagrama();
        auto_zise_popup_paginas_externas_libres();
        auto_zise_popup_agregar_organigrama();
        auto_zise_popup_editar_organigrama();
        auto_zise_popup_agregar_area_departamento();
        auto_zise_popup_editar_area_departamento();
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
function auto_zise_popup_agregar_organigrama() {
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
        $('#Panel_agregar_organigrama').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_Panel_agregar_organigrama').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#Contenido_agregar_organigrama').css("height", (document.getElementById("modal_content_Panel_agregar_organigrama").clientHeight - (document.getElementById("Divcerrarbuton2_agregar_organigrama").clientHeight + document.getElementById("modal-footer_Panel_agregar_organigrama").clientHeight)) + "px");
        //Para los modal que contiene gred
        //$('#content_data_grid').css("height", (document.getElementById("contenido_procesa_usu_rel_solicitud").clientHeight - document.getElementById("contenido_titulo_val_radicacion_documentos").clientHeight) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_agregar_organigrama " + err.message);
    }
}
function auto_zise_popup_editar_organigrama() {
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
        $('#Panel_editar_organigrama').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_Panel_editar_organigrama').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#Contenido_editar_organigrama').css("height", (document.getElementById("modal_content_Panel_editar_organigrama").clientHeight - (document.getElementById("Divcerrarbuton2_editar_organigrama").clientHeight + document.getElementById("modal-footer_Panel_editar_organigrama").clientHeight)) + "px");
        //Para los modal que contiene gred
        //$('#content_data_grid').css("height", (document.getElementById("contenido_procesa_usu_rel_solicitud").clientHeight - document.getElementById("contenido_titulo_val_radicacion_documentos").clientHeight) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_agregar_organigrama " + err.message);
    }
}
function auto_zise_popup_agregar_area_departamento() {
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
        var heig_porcent = espacio_iframe - ((espacio_iframe * 40) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_crear_area_departamento').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_Panel_crear_area_departamento').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#Contenido_crear_area_departamento').css("height", (document.getElementById("modal_content_Panel_crear_area_departamento").clientHeight - (document.getElementById("Divcerrarbuton2_crear_area_departamento").clientHeight + document.getElementById("modal-footer_Panel_crear_area_departamento").clientHeight)) + "px");
        //Para los modal que contiene gred
        //$('#content_data_grid').css("height", (document.getElementById("contenido_procesa_usu_rel_solicitud").clientHeight - document.getElementById("contenido_titulo_val_radicacion_documentos").clientHeight) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_agregar_area_departamento " + err.message);
    }
}
function auto_zise_popup_editar_area_departamento() {
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
        var heig_porcent = espacio_iframe - ((espacio_iframe * 40) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_editar_area_departamento').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_Panel_editar_area_departamento').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#Contenido_editar_area_departamento').css("height", (document.getElementById("modal_content_Panel_editar_area_departamento").clientHeight - (document.getElementById("Divcerrarbuton2_editar_area_departamento").clientHeight + document.getElementById("modal-footer_Panel_editar_area_departamento").clientHeight)) + "px");
        //Para los modal que contiene gred
        //$('#content_data_grid').css("height", (document.getElementById("contenido_procesa_usu_rel_solicitud").clientHeight - document.getElementById("contenido_titulo_val_radicacion_documentos").clientHeight) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_editar_area_departamento " + err.message);
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
        $('#paginas_externas_popou').css("height", (espacio_iframe) + "px");
        $('#Iframe_paginas_externas_popup_').css("height", (espacio_iframe) + "px");
       
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_paginas_externas_libres");
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

       // $('#div_diagran').css("height", (espacio_iframe - 100) + "px");
        //$('#content').css("height", (espacio_iframe - 100) + "px");
        var client_keigth = document.getElementById("Menutol").clientHeight + document.getElementById("menucab").clientHeight + document.getElementById("footer").clientHeight;
        $('#content').css("height", (espacio_iframe - (client_keigth)) + "px");
        
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_ventana_diagrama");
    }

}

function eliminar_ajaxtolkit() {
    try {
        var ele = document.getElementsByClassName("ajax__fileupload_fileItemInfo");
        for (var i = 0; i < ele.length; i++) {
            ele[i].parentNode.removeChild(ele[i]);
        }
    } catch (err) {
        alert(err.message + " funcion eliminar_ajaxtolkit " + err.message);
    }
}
function activa_boton_dowload() {
    try {

        document.getElementById("Button_guardar_desicion").click();
    }
    catch (err) {
        alert(err.message + " funcion activa_boton_dowload " + err.message);
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
            //document.getElementById("ImageButtonGuardar").src = "../workflow/imageneswf/Guardar_actividad.png";
        } else {
            document.getElementById("HiddenField_value_selecion").value = document.getElementById("HiddenField_value_selecion").value + "|" + args.getNode().id;
            //document.getElementById("ImageButtonGuardar").src = "../workflow/imageneswf/Guardar_actividad.png";
        }

    } else {
        document.getElementById("HiddenField_value_selecion").value = args.getNode().id;
        //document.getElementById("ImageButtonGuardar").src = "../workflow/imageneswf/Guardar_actividad.png";
        //var f = args.getNode();
        //alert(f.id);

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