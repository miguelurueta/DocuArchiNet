$(document).ready(function () {   
    $.fn.inicio = function () {       
        //CURSOR Y SELECCION LISTA CHEQUEO ADJUNTA DOCUMENTO 
        $('#data_grid_chequeo tr[id]').click(function () {
            $('#data_grid_chequeo tr[id]').css({ "background": "White", "color": "Black" });
            $(this).css({ "background-color": "#E7EDF5", "color": "Black" });
            var fer = $(this).attr("id");
            $('#Hidden_0001').val(fer);
        });
        //ASIGNA EL CURSOR DE SELECCION CUANDO PASA EL CURSOR
        $('#data_grid_chequeo tr[id]').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });
        resize_page_visor();
    };

    $("#noaming").bind("contextmenu", function (e) {
        e.preventDefault();
    });
  
    resize_page_visor();
    controla_botones_permiso_visor();
    resize_adjunta_documento();
    auto_zise_popup_impresion();
    auto_zise_popup_guardar_documento();
    var left;
    var top;
    $("#draggable").css({ opacity: 0.5 });
    left = $("#draggable").position.left;
    top = $("#draggable").position.top;
    var heigimage, withimage;
    $("#draggable").draggable({
        containment: $("#zona"),
        stop: function (event, ui) {
            var elemento = $("#draggable");
            var posicion = elemento.position();
            left = posicion.left;
            top = posicion.top;
            var dragab = $("#draggable");
            var contenido = $("#content");
            var scr = contenido.scrollTop();
            var scrolleft = contenido.scrollLeft();
            left = (scrolleft) + left;
            var posicfinal = (top + scr) - 10;
            $("#Hiddenintercambio").val(top + "-" + left + "-" + dragab.height() + "-" + dragab.width() + "-" + dragab.height() + "-" + scr + "-" + posicfinal + "-" + heigimage + "-" + withimage);
        }


    }

 );

    $("#draggable").resizable({
        maxHeight: 80, maxWidth: 100, minWidth: 50, minHeight: 50,
        start: function (event, ui) {
            //$("#draggable").offset({ top: top, left: left });
        },
        stop: function (event, ui) {
            var conta = $("#draggable");
            //$("#img").imageResize();
            //var im = $("#img");
            $("#draggable").css('position', 'relative');
            var dragab = $("#draggable");
            var contenido = $("#content");
            var scr = contenido.scrollTop();
            var scrolleft = contenido.scrollLeft();
            left = left - scrolleft;
            var posicfinal = (top + scr) - 10;
            //$("#draggable").offset({ top: top, left: left });
            $("#Hiddenintercambio").val(top + "-" + left + "-" + dragab.height() + "-" + dragab.width() + "-" + dragab.height() + "-" + scr + "-" + posicfinal);

        },
        resize: function (event, ui) {

            $("#img").imageResize();
            var contenido = $("#content");
            var scroltop = contenido.scrollTop();
            var scroleft = contenido.scrollLeft();
            $("#draggable").offset({ top: top, left: left - scroleft });

        }
    }
    );

    $('#draggable').contextMenu('context-menu-1', {
        'Guardar': {
            click: function (element) {  // element is the jquery obj clicked on when context menu launched
                //__doPostBack('#ImageButtonguardar.ClientID', 'to');
                document.getElementById('ImageButtonguardar').click();
                $("#draggable").css("display", "none");
            }
        },
        'Limpiar': {
            click: function (element) {  // element is the jquery obj clicked on when context menu launched

                $("#draggable").css("display", "none");
            }
        },
        'Cancelar': {
            click: function (element) {  // element is the jquery obj clicked on when context menu launched

                //$(element).css("display", "none");
            }
        }
    }


    );
    
})
//CODIGO AJUSTA EL ALTO DE LA PAGINA DEBE ESTAR EL FORM 100% ALTURA
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
        resize_page_visor();
        controla_botones_permiso_visor();
        resize_adjunta_documento();
        auto_zise_popup_impresion();
        auto_zise_popup_guardar_documento();
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
function firma_mecanica() {
    try {
        var heig_porcent = $("#noaming").attr("zon_heig");
        if (heig_porcent == 10) {
            alert("Debe aumentar el zoom de la imagen para agregar la firma");
            return;
        }
        $("#img").attr("src", $("#Hiddenintercambio2").val());
        $("#draggable").css("with", "100");
        $("#draggable").css("height", "70");
        $("#draggable").css("display", "block");
        $("#img").imageResize();
        var contenido = $("#content");
        var topconten = contenido.scrollTop();
        var lefconten = contenido.scrollLeft();
        if (topconten == 0) {
            var x = $("#zona").offset();
            topconten = x.top + 5;
            lefconten = x.left + 5;
        } else {
            var elmnt = document.getElementById("content");
            elmnt.scrollLeft = 1;
            elmnt.scrollTop = 1;
            var x = $("#zona").offset();
            topconten = x.top;
            lefconten= x.left;
        }
        $("#draggable").offset({ top: topconten, left: lefconten });      
    }
    catch (err) {
        alert(err.message + " funcion firma_mecanica " + err.message);
    }
}
function limpiar_firma() {
    $("#draggable").css("display", "none");
}


function res_body() {
    try {
        var espacio_iframe;
        if (window.innerHeight) {
            //navegadores basados en mozilla 
            espacio_iframe = window.innerHeight
        } else {
            if (document.hidden == true) {
                if (document.body.clientHeight != undefined) {
                    //Navegadores basados en IExplorer, es que no tengo innerheight 
                    espacio_iframe = document.body.clientHeight
                } else {
                    //otros navegadores 
                    espacio_iframe = 478
                }
            }
        }
        $("body").css("height", (espacio_iframe - 50) + "px");
        var toptoll = ($("#content").height());
        $("#Pietolbar").css("top", toptoll + "px");
    } catch (ex) {
        alert("Funcion res_body " + ex.message)
    }
}
function preven_event_search_keypres_enter(e, sender) {
    try {
        tecla = (document.all) ? e.keyCode : e.which;
        if (tecla == 13) {
            document.getElementById("ImageButton_ir_pagina").click();
            e.preventDefault();
        }
    } catch (err) {
        alert(err.message + " funcion preven_event_search_keypres_enter " + err.message);
    }
}
function resize_adjunta_documento() {
    try {
        var espacio_iframe;
        if (window.innerHeight) {
            //navegadores basados en mozilla 
            espacio_iframe = window.innerHeight
        } else {
            if (document.hidden == true) {
                if (document.body.clientHeight != undefined) {
                    //Navegadores basados en IExplorer, es que no tengo innerheight 
                    espacio_iframe = document.body.clientHeight
                } else {
                    //otros navegadores 
                    espacio_iframe = 478
                }
            }
        }
        //if (document.getElementById("Panel_seleccion_tipo_adjunto").style.display == "Block") { }
        //var heig_ = document.getElementById("title_desicion").clientHeight + document.getElementById("campo_adjunta").clientHeight + document.getElementById("Div_inferior").clientHeight;
        //$("#Contenido_seleccion_tipo_adjunto").css("height", heig_ + 50 + "px");
        //$("#Panel_seleccion_tipo_adjunto").css("height", heig_ + 50 + "px");


    }
    catch (err) {
        alert(err.message + " funcion resize_adjunta_documento " + err.message);
    }
}
function resize_page_visor() {
    try {
        var espacio_iframe = 420;
        if (window.innerHeight) {
            //navegadores basados en mozilla 
            espacio_iframe = window.innerHeight
        } else {
            if (document.hidden == true) {
                if (document.body.clientHeight != undefined) {
                    //Navegadores basados en IExplorer, es que no tengo innerheight 
                    espacio_iframe = document.body.clientHeight
                } else {
                    //otros navegadores 
                    espacio_iframe = 478
                }
            }
        }
        if (parent.document.getElementById("ifrm_visor_")) {
            espacio_iframe = parent.document.getElementById("ifrm_visor_").clientHeight;
        }
        
        $("#ContentGeneral").css("height", (espacio_iframe - 1) + "px");
        $('#content').css("height", ((document.getElementById("ContentGeneral").clientHeight - 1) - document.getElementById("tollimage").clientHeight) + "px");
    }
    catch (err) {
        alert(err.message + " funcion resize_page_visor " + err.message);
    }
}
function auto_zise_popup_impresion() {
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
        $('#Panelimpresionpost').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_Panelimpresionpost').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#ContenidoImpresion_post').css("height", (document.getElementById("modal_content_Panelimpresionpost").clientHeight - (document.getElementById("divcabecer2_post").clientHeight )) + "px");
        //Para los modal que contiene gred
        $('#ifimpre_post_').css("height", (document.getElementById("ContenidoImpresion_post").clientHeight - 5) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_impresion " + err.message);
    }
}
function auto_zise_popup_guardar_documento() {
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
        $('#Panel_guardar').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_Panel_guardar').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#Content_guardar_documento').css("height", (document.getElementById("modal_content_Panel_guardar").clientHeight - (document.getElementById("divcabecer2_post_guardar").clientHeight)) + "px");
        //Para los modal que contiene gred
        $('#Iframe_guardar').css("height", (document.getElementById("Content_guardar_documento").clientHeight - 5) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_guardar_documento " + err.message);
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
        alert(err.message + " funcion eliminar_ajaxtolkit " + err.message);
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
        prog.style.zIndex = "4000009";
        $("#progres_bar").css("display", "block");
        prog.style.position = "fixed";

    }
    catch (err) {
        alert(err.message + " Funcion posicion_update_pogres");
    }

}
function posicion_update_pogres_modal(progres) {
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
        $("#progres_bar").addClass("overlay_");
        $("#progres_bar").css("width", "100%");
        $("#progres_bar").css("heigth", "100%");
        $("#progres_bar").css("display", "block");
        var prog = document.getElementById("imgr_modal");
        var widtop = (espacio_iframe / 2);
        var heitop = (with_frame / 2);
        prog.style.top = widtop + "px";
        prog.style.left = heitop + "px";
        //prog.style.zIndex = "4000009";
        prog.style.position = "fixed";
        var progres = document.getElementById("progres_bar");
        progres.style.top = "0px";
        progres.style.left = "0px";


    }
    catch (err) {
        alert(err.message + " Funcion posicion_update_pogres");
    }

}
function auto_zise_popup_lista_chequeo(value_lista_general) {
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


        $(document).ready(bodyResize);
        $(window).resize(bodyResize);
        function bodyResize() {

        }

        var gridwith = document.getElementById("Contenedorgrid").clientWidth;
        var gridheihg = document.getElementById("Contenedorgrid").clientHeight;
        //$('#data_grid_chequeo').css("height", (gridheihg - 5) + "px");
        //LLAMA PLUGIN FIJA HIDER O TITULOS   
        if (value_lista_general == "1") {
            if ($('#data_grid_chequeo td').children.length > 0 && $('#data_grid_chequeo tr:visible').length > 0) {
                $('#data_grid_chequeo th').hide();

            }
        }
        document.getElementById("Hidden_0001").value = "-1";
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_lista_chequeo " + err.message);
    }
}
function acti_busq_lista_cheq(e, sender) {
    try {

        tecla = (document.all) ? e.keyCode : e.which;
        if (tecla == 13) {
            //document.getElementById("Button_busca_general_archivo").click();
            busqueda_gred('Hidden_0001', 'data_grid_chequeo', 'TextBox_contenido_busqueda_lista_cheq', 'CheckBox_busqueda_list_cheq');
            e.preventDefault();
            //return false;
        }


    } catch (err) {
        alert(err.message + " funcion acti_busq_general_archivo " + err.message);
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
            var scrollableDiv = $("#" + "Panel_principal");
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
    finally {
        document.getElementById(contenido_busqueda).focus();
    }

}
function AjaxFileUpload_change_text() {

    Sys.Extended.UI.Resources.AjaxFileUpload_SelectFile = "Adjuntar";
    Sys.Extended.UI.Resources.AjaxFileUpload_DropFiles = "Soltar y arrastrar archivos aquí";
    Sys.Extended.UI.Resources.AjaxFileUpload_Pending = "Pendiente";
    Sys.Extended.UI.Resources.AjaxFileUpload_Remove = "Eliminar";
    Sys.Extended.UI.Resources.AjaxFileUpload_Upload = "Guardar";
    Sys.Extended.UI.Resources.AjaxFileUpload_Uploaded = "Cargando";
    Sys.Extended.UI.Resources.AjaxFileUpload_UploadedPercentage = "Cargando {0} %";
    Sys.Extended.UI.Resources.AjaxFileUpload_Uploading = "Cargando";
    Sys.Extended.UI.Resources.AjaxFileUpload_FileInQueue = "{0} archivos(s) de .";
    Sys.Extended.UI.Resources.AjaxFileUpload_AllFilesUploaded = "All Files Uploaded.";
    Sys.Extended.UI.Resources.AjaxFileUpload_FileList = "Lista de archivos a cargar:";
    Sys.Extended.UI.Resources.AjaxFileUpload_SelectFileToUpload = "Archivos(s) para cargar.";
    Sys.Extended.UI.Resources.AjaxFileUpload_Cancelling = "Cancelando...";
    Sys.Extended.UI.Resources.AjaxFileUpload_UploadError = "Ocurrio un error cargando el archivo.";
    Sys.Extended.UI.Resources.AjaxFileUpload_CancellingUpload = "Cancelando carga...";
    Sys.Extended.UI.Resources.AjaxFileUpload_UploadingInputFile = "Cargando archivos: {0}.";
    Sys.Extended.UI.Resources.AjaxFileUpload_Cancel = "Cancelar";
    Sys.Extended.UI.Resources.AjaxFileUpload_Canceled = "cancelando";
    Sys.Extended.UI.Resources.AjaxFileUpload_UploadCanceled = "Carga de archivo cancelada";
    Sys.Extended.UI.Resources.AjaxFileUpload_DefaultError = "Error cargando archivo";
    Sys.Extended.UI.Resources.AjaxFileUpload_UploadingHtml5File = "Cargando archivo: {0} of size {1} bytes.";
    Sys.Extended.UI.Resources.AjaxFileUpload_error = "error";
}
function controla_botones_permiso_visor() {
    try {
        if (document.getElementById("ImageFirma")) {
           
        } else {
            document.getElementById("ImageFirma_").style.display = "none";
        }
        if (document.getElementById("ImageButtonadjunta")) {
           
        } else {
            document.getElementById("ImageButtonadjunta_").style.display = "none";
        }
    }
    catch (ex)
    {
        alert("Error general funcion controla_botones_permiso_visor " + ex.message)
    }
}