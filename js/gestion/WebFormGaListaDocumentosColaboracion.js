$(document).ready(function () {
    $.fn.inicio = function () {
        $('#data_grid_listado_solicitudes tr[id]').click(function () {
            $('#data_grid_listado_solicitudes tr[id]').css({ "background": "White" });
            $(this).css({ "background-color": " #e8e8f7" });
            var fer = $(this).attr("id");
            $('#hdnEmailID').val(fer);
            var id_docu = $(this).attr("id_doc_compatido");
            $('#hdnEmailID_VAL').val(id_docu);
            
        });
        //Ver listadado de documentos compartidos
        $('#data_grid_listado_solicitudes tr[id]').dblclick(function () {
            var fer = $(this).attr("id");
            $('#hdnEmailID').val(fer);
            var id_docu = $(this).attr("id_doc_compatido");
            $('#hdnEmailID_VAL').val(id_docu);
            document.getElementById("Button_ver_documentos_relacionados").click();          
        });
        $('#data_grid_listado_solicitudes tr[id]').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });    
        if (document.getElementById("Hidden_control_lista").value == "Segunda") {
            document.getElementById("Hidden_control_lista").value = "inhabilitado";
            auto_zise_popup_lista_solicitudes("0", "1");
        }
        if (document.getElementById("Hidden_control_lista").value == "") {
            document.getElementById("Hidden_control_lista").value = "Segunda";
            auto_zise_popup_lista_solicitudes("1", "1");
        }
        active_enter_buton();
        auto_zise_popup_nota_solicitud();
    }

    $('#Panel_principal').contextMenu('context-menu-1', {

        'Salir del menú': {
            click: function (element) { }
        },
        'Ver documentos relacionados': {
            click: function (element) {
                document.getElementById("Button_ver_documentos_relacionados").click();
            }
        }
    });
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
        auto_zise_popup_lista_solicitudes("1", "1");
        auto_zise_popup_nota_solicitud();
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
function preven_event_search_keypres_enter(e, sender) {
    try {

        tecla = (document.all) ? e.keyCode : e.which;
        if (tecla == 13) {
            document.getElementById("ImageButton_buscar").click();
            e.preventDefault();
        }
    } catch (err) {
        alert(err.message + " funcion preven_event_search_keypres_enter " + err.message);
    }
}

function dercaga_documento(elmen) {
    document.getElementById("Hidden_documento_descarga").value = elmen.id;
    document.getElementById("Button_descarga_documento").click();
}
function active_enter_buton() {
    try {
        $("#TextBox_busqueda").on('keyup', function (e) {
            if (e.keyCode == 13) {
                document.getElementById("ImageButton_buscar").click();
            }
        });
    }
    catch (err) {
        alert(err.message + " Función active_enter_buton");
    }
}
function busqueda_gred(HiddenSeleccion, data_grid, contenido_busqueda, CheckboxBusqueda, color_ref) {
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
                            $(this).parent().css({ "background-color": "LightSkyBlue", "color": color_ref });
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
                            $(this).parent().css({ "background-color": "LightSkyBlue", "color": color_ref });
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
function auto_zise_popup_lista_solicitudes(value_lista_general, value_lista_usuario) {
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

        var total = document.getElementById("boton_search").clientHeight + document.getElementById("Label_titulo_listado_solicitudes").clientHeight;
        var gridwith = with_frame - 1;
        var gridheihg_ = (espacio_iframe - (total + 50));
        $('#content_grid').css("height", (gridheihg_ - 10) + "px");
       
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_lista_solicitudes " + err.message);
    }
}
function auto_zise_popup_nota_solicitud() {
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
        $('#Panel_nota_solicitud_colaboracion').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_nota_solicitud_colaboracion').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#contenido_procesa_nota_solicitud_colaboracion').css("height", (document.getElementById("modal_content_nota_solicitud_colaboracion").clientHeight - (document.getElementById("divcabecer2_nota_solicitud_colaboracion").clientHeight + document.getElementById("content_boton_nota_solicitud_colaboracion").clientHeight)) + "px");
        //Para 
        $('#TextBox_nota_colaboracion').css("height", (document.getElementById("contenido_procesa_nota_solicitud_colaboracion").clientHeight - 15) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_nota_solicitud " + err.message);
    }
}
function prevent(event, element) {
    try {

        var fer = $(element).attr("idd");
        var fer_ = $(element).attr("id_doc");
        var tip_event = $(element).attr("tip_event");
        if (tip_event == "ver_doc_col") {
            $('#hdnEmailID').val(fer);
            $('#hdnEmailID_VAL').val(fer_);
            document.getElementById("Button_ver_documentos_relacionados").click();
        }
        if (tip_event == "ver_not_comp") {
            $('#hdnEmailID').val(fer);
            $('#hdnEmailID_VAL').val(fer_);
            document.getElementById("Button_ver_nota_colaboracion").click();
        }
      

        event.preventDefault();
        element.focus();
    }
    catch (err) {
        alert(err.message + " Funcion prevent");
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