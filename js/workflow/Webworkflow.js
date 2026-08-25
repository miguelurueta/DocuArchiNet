
$(document).ready(function () {
    $.fn.inicio = function () {
   
    }
    //REGISTRA EVENTOS GREDVIEW GRUPO
    $.fn.auto_postback = function () {
        //actualiza_treview_seleccion_dos();
        auto_zise_popup_envia_usuario_grupo();
        cli_node_tre();
        timer();   
    };
    function cli_node_tre() {
        try {
            $("#tre .LeafNodeStyle_2 a").click(function () {
                var url = $(this).text();
            });
        }
        catch (err) {
            alert(err.message + " Funcion cli_node_tre");
        }
    };
    $.fn.clired_user = function () {
        try {
               
            $('#GridView2 tr[id]').click(function () {
                $('#GridView2 tr[id]').css({ "background-color": "White", "color": "Black" });
                $(this).css({ "background-color": "#e8e8f7", "color": "Black" });
                 fer = $(this).attr("id");
                 $('#HiddenSeleccion').val(fer);
                 document.getElementById("Hidden_id_tarea_sel").value = fer;
               
            });
            $('#GridView2 tr[id]').dblclick(function () {
                //document.getElementById("Hidden_tipo_visor").value = "VISOR WORKFLOW";
                //document.getElementById("Button_visor_emergente").click(); 
               //let fer = $(this).attr("id");
               // document.getElementById("Hidden_id_tarea_sel").value = fer;
                //if (document.getElementById("Hidden_id_tarea_sel").value == "-1") {
               //     alert("Debe selecionar la tarea para asignar "); 
               // } else {
                //    dispalyInterfaceEscaner();
                //    document.getElementById("ButtonSeleccionGrupo").click();
                //}
            });
            $('#GridView2 tr[id]').mouseover(function () {
                $(this).css({ cursor: "hand", cursor: "pointer" });
            });
           
            $('#TreeViewseleccion_digitalizado').dblclick(function () {
                document.getElementById("ButtonVisua").click();
            });
           
            $('#GridView_list_documento_relacion tr[id_rad]').mouseover(function () {
                $(this).css({ cursor: "hand", cursor: "pointer" });
            });   
            $('#GridView_list_documento_relacion_wf tr[id_wf]').mouseover(function () {
                $(this).css({ cursor: "hand", cursor: "pointer" });
            });
            $('#GridView_list_imagenes_sii tr[id]').click(function () {
                try {

                    $('#GridView_list_imagenes_sii tr[id]').css({ "background": "White", "color": "Black" });
                    $(this).css({ "background-color": "#e8e8f7", "color": "Black" });
                }
                catch (err) {
                    alert(err.message + " Funcion clik");
                }
            });
            $('#GridView_list_imagenes_sii tr[id]').mouseover(function () {
                $(this).css({ cursor: "hand", cursor: "pointer" });
            });
            $('#GridView_list_inscripciones_sii tr[id]').click(function () {
                try {

                    $('#GridView_list_inscripciones_sii tr[id]').css({ "background": "White", "color": "Black" });
                    $(this).css({ "background-color": "#e8e8f7", "color": "Black" });
                }
                catch (err) {
                    alert(err.message + " Funcion clik");
                }
            });
            $('#GridView_list_inscripciones_sii tr[id]').mouseover(function () {
                $(this).css({ cursor: "hand", cursor: "pointer" });
            });
           
            GetLista_lista_actividades_ruta("TextBox_buequeda_general_lista_actividades");
            $('#data_grid_actividad tr[id]').click(function () {
                $('#data_grid_actividad tr[id]').css({ "background": "White", "color": "Black" });
                $(this).css({ "background-color": "#e8e8f7", "color": "Black" });
            });
            $('#data_grid tr[id]').click(function () {
                $('#data_grid tr[id]').css({ "background": "White", "color": "Black" });
                $(this).css({ "background-color": "#e8e8f7", "color": "Black" });
            });

            //CURSOR Y SELECCION LISTA CHEQUEO ADJUNTA DOCUMENTO 
            $('#data_grid_chequeo tr[id]').click(function () {
                $('#data_grid_chequeo tr[id]').css({ "background": "White", "color": "Black" });
                $(this).css({ "background-color": "#e8e8f7", "color": "Black" });
                var fer = $(this).attr("id");
                $('#Hidden_0001').val(fer);
            });
            //ASIGNA EL CURSOR DE SELECCION CUANDO PASA EL CURSOR
            $('#data_grid_chequeo tr[id]').mouseover(function () {
                $(this).css({ cursor: "hand", cursor: "pointer" });
            });
            //CURSOR Y SELECCION LISTA CHEQUEO ACTUALIZA TIPO DOCUMENTO
            $('#data_grid_chequeo_actualiza tr[id]').click(function () {
                $('#data_grid_chequeo_actualiza tr[id]').css({ "background": "White", "color": "Black" });
                $(this).css({ "background-color": "#e8e8f7", "color": "Black" });
                var fer = $(this).attr("id");
                $('#Hidden_0003').val(fer);
            });
            //ASIGNA EL CURSOR DE SELECCION CUANDO PASA EL CURSOR
            $('#data_grid_chequeo_actualiza tr[id]').mouseover(function () {
                $(this).css({ cursor: "hand", cursor: "pointer" });
            });
            //ASIGNA CURSOR PARA LA LISTA DE NOTAS
            $('#GridView_lista_notas tr[id]').mouseover(function () {
                $(this).css({ cursor: "hand", cursor: "pointer" });
            });
            $('#GridView_lista_notas tr[id]').click(function () {
                $('#GridView_lista_notas tr[id]').css({ "background-color": "White", "color": "Black" });
                $(this).css({ "background-color": "#e8e8f7", "color": "Black" });
                var fer = $(this).attr("id");
                $('#hdnidlista').val(fer.toString());

            });
            //ASIGNA CURSOR PARA LISTA TAREAS PENDIENTES
            $('#data_grid_lista_pendientes tr[id]').click(function () {
                try {
                    $('#data_grid_lista_pendientes tr[id]').css({ "background": "White", "color": "Black" });
                    $(this).css({ "background-color": "#e8e8f7", "color": "Black" });
                    $('#Hidden_id_list_id_task').val($(this).attr("id_tarea"));
                    $('#Hidden_id_list_pent').val($(this).attr("id"));
                }
                catch (err) {
                    alert(err.message + " Funcion clik");
                }
            });
            $('#data_grid_lista_pendientes tr[id]').mouseover(function () {
                $(this).css({ cursor: "hand", cursor: "pointer" });
            });
           
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
                if (boton[0].alt == "Con pendientes aprobacion") {
                    var sr = boton[0].src;
                    sr = sr.replace("poraprobacion1.jpg", "poraprobacion2.jpg");
                    boton[0].src = sr;
                }
                if (boton[0].alt == "Sin pendientes aprobacion") {
                    var sr = boton[0].src;
                    sr = sr.replace("poraprobacion1.jpg", "poraprobacion2.jpg");
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
                if (boton[0].alt == "Con pendientes aprobacion") {
                    var sr = boton[0].src;
                    sr = sr.replace("poraprobacion2.jpg", "poraprobacion1.jpg");
                    boton[0].src = sr;
                }
                if (boton[0].alt == "Sin pendientes aprobacion") {
                    var sr = boton[0].src;
                    sr = sr.replace("poraprobacion2.jpg", "poraprobacion1.jpg");
                    boton[0].src = sr;
                }
            });
            $("#draggable").css({ opacity: 0.5 });
            left = $("#draggable").position.left;
            top = $("#draggable").position.top;
            var heigimage, withimage;
            var top_ = $("#zona").position.top + $("#zona").height();
            var bottom_ = $("#zona").position.top - $("#zona").height();
            $("#draggable").draggable({
                containment: $("#noaming"),
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
                    click: function (element) {
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
                inicializa_estado_pendiente();
                auto_zise_popup_workflow(0);
                auto_zise_popup_lista_actividades_ruta();
                auto_zise_popup_copiar_estructura();
                auto_zise_popup_recuperar_tarea();
                auto_zise_popup_ventana_externa();
                auto_zise_popup_paginas_externas_libres();
                auto_zise_popup_respuesta();
                auto_zise_popup_detalle_respuesta();
                auto_zise_popup_detalle_trazabilidad();
                auto_zise_popup_compartir_documento();
                auto_zise_popup_documentos_radicados_relacionados();
                auto_zise_popup_detalle_grupo_usuario();
                auto_zise_popup_detalle_transacciones();
                selection_event();
                auto_zise_popup_trace_grafic();
                actuo_zise_popup_compartir_correo_electronico();
                auto_zise_popup_visor_externo();
                detecte_boton_tool_visible();
                auto_zise_popup_detalle_sesion_workflow();
                auto_zise_popup_detalle_tarea_workflow();
                auto_zise_popup_adjunta_imagen_digitalizada();
                service_posibles_datos_tramites_();
                service_GetPosiblesDatos_lista_tareas_pendientes();
                auto_zise_popup_detalle_radicado();
                auto_zise_popup_autorizados();
                auto_size_content_anotacion();
                auto_zise_popup_visor_tarea_pendiente();
                auto_zise_popup_estado_paginacion();
                auto_zise_popup_guardar_documento();
                auto_zise_popup_impresion();
                auto_zise_popup_consulta_meta_dato();
                auto_zise_popup_adjunta_documento_workflow();
                auto_zise_popup_list_imagenes_sii();
                auto_zise_popup_list_inscripciones_sii();
                auto_zise_popup_lista_form_control_person("actualiza_indice_batch_wf");
                auto_zise_popup_lista_form_control_person("actualiza_indice_batch_wf_enlace");
                auto_zise_popup_modal_conten_procesing_image_worflow("div_content_tabla_procesa_detail_document_proces_workflow", "contenido_procesa_detail_document_proces_workflow");
                auto_zise_popup_modal_conten_copy_document_expediente("div_content_tabla_procesa_detail_copy_document_expediente_wf", "div_content_tabla_procesa_detail_copy_document_expediente_wf");
        }
        catch (err) {
            alert(err.message + " Funcion clired_user");
        }
      }
      
       
       
        $.fn.redienciona = function () {

            var valor;
            if ($('#Hidden1') != null) {
                valor = $('#Hidden1').val();
            }

            if (valor == "ENVIARUSUARIO") {
                //$('#DivColorPagina').css("width", "720px");
                //$('#Contenidopagina').css("width", "720px");
                //$('#Divcab').css("width", "720px");
                $('#Labeletiqueta').html("Enviar tarea a usuario");
                //$('#<%=frameeditexpanse_.ClientID%>//').attr("width", "520");
                //$('#Iframelibre').css("width", "100%");

            }

            if (valor == "ENVIARACTIVIDAD") {
                //$('#DivColorPagina').css("width", "230px");
                //$('#Contenidopagina').css("width", "230px");
                //$('#Divcab').css("width", "232px");
                $('#Labeletiqueta').html("Enviar tarea a grupo");
                //$('#<%=frameeditexpanse_.ClientID%>//').attr("width", "229");

            }

           

            if (valor == "PRIORIDAD") {
                $('#Iframelibre_').css("Height", "216px");
                $('#PanelLibre').css("width", "250px");
                $('#PanelLibre').css("Height", "218px");
                $('#Div9').css("width", "249px");
                $('#Div9').css("height", "217px");
                $('#Labeladver').html("Prioridad ");
                //$('#Div10').css("width", "350px");
                // $('#Div10').css("height", "350px");

            }

            if (valor == "PASWORD") {
                $('#Iframelibre_').css("Height", "150px");
                $('#PanelLibre').css("width", "320px");
                $('#PanelLibre').css("Height", "218px");
                $('#Div9').css("width", "320px");
                $('#Div9').css("height", "150px");
                $('#Labeladver').html("Cambiar Contraseña ");
               

            }

            if (valor == "ALARMA") {
                $('#Iframelibre_').css("Height", "50px");
                $('#PanelLibre').css("width", "320px");
                $('#PanelLibre').css("Height", "50px");
                $('#Div9').css("width", "320px");
                $('#Div9').css("height", "50px");
                $('#Labeladver').html("Actualiza Intervalo de Alarma ");
               

            }

            if (valor == "ACTUALIZACION") {
                $('#Iframelibre_').css("Height", "50px");
                $('#PanelLibre').css("width", "320px");
                $('#PanelLibre').css("Height", "50px");
                $('#Div9').css("width", "320px");
                $('#Div9').css("height", "50px");
                $('#Labeladver').html("Actualiza Intervalo de Actualización ");
                //$('#Div10').css("width", "350px");
                // $('#Div10').css("height", "350px");

            }
            //DETALLE  DETALLETAREA
            if (valor == "DETALLE") {
                $('#Iframelibre_').css("Height", "400px");
                $('#PanelLibre').css("width", "250px");
                $('#PanelLibre').css("Height", "400px");
                $('#Div9').css("width", "249px");
                $('#Div9').css("height", "400px");
                $('#Labeladver').html("Detalle Sesión ");
                //$('#Div10').css("width", "350px");
                // $('#Div10').css("height", "350px");

            }

            if (valor == "DETALLETAREA") {
                $('#Iframelibre_').css("Height", "400px");
                $('#PanelLibre').css("width", "250px");
                $('#PanelLibre').css("Height", "400px");
                $('#Div9').css("width", "249px");
                $('#Div9').css("height", "400px");
                $('#Labeladver').html("Detalle Tarea ");
                //$('#Div10').css("width", "350px");
                // $('#Div10').css("height", "350px");

            }

            var valordato;
            var tex;
            if ($('#mesjbox') != null) {
                valordato = $('#mesjbox').val();
            }
            if ($('#mesjbox') != null) {
                tex = $('#hdnEmailID').val();
            }
            //alert(valordato)
            if (valordato == "TERMINAR") {

                $('#Lableme').html(tex);

            }

        };
        
        $('#seleccion').contextMenu('context-menu', {
            
            'Salir del menú': {
                click: function (element) { },
                klass: "fad fa-times"
            },
            'Respuesta al tramite': {
                click: function (element) {
                    document.getElementById("Button_activa_respuesta_radicado").click();              
                },
                klass: "fa fa-reply"
            },
            'Eliminar documento adjunto': {
                click: function (element) {
                    prevent_elimina_adjunto();
                },
                klass: "fad fa-file-times"
            },
            'Cambiar tipologia documental': {
                click: function (element) {
                    document.getElementById("Button_clasficar_documento").click();
                },
                klass: "fa fa-file-edit"
            },
            'Adjuntar documento digitalizado': {
                click: function (element) {
                    menu_context_treview("D-DNDT");
                    
                },
                klass: "fal fa-scanner-image"
            },
            'Adjuntar documento': {
                click: function (element) {
                    //document.getElementById("Hidden_tip_adjunt").value = "wf";
                   //document.getElementById("Button_tool_adjunta_documento_relacionado").click();
                    inicializa_tipo_adjunto_documento(event, this, 'C-DW-ENL');
                },
                klass: "fad fa-file-upload"
            },
            'Compartir documentos a usuario': {
                click: function (element) {
                    menu_context_treview("D-CDW");
                },
                klass: "fad fa-share-square"
            },
            'Compartir documentos a correo electrónico': {
                click: function (element) {
                    menu_context_treview("D-CEDTS");
                },
                klass: "fad fa-envelope"
            }
        });
   
   
        $('#contenido_lista_tareas').contextMenu('context-menu', {
            'Seleccionar Tarea': {
                click: function (element) {  
                    if (document.getElementById("Hidden_id_tarea_sel").value == "-1") {
                        alert("Debe selecionar la tarea para asignar ");
                    } else {
                        document.getElementById("ButtonSeleccionGrupo").click();
                    }
                }, klass: "fad fa-arrow-to-bottom"
            },
            'Limpiar Seleccion': {
                click: function (element) {  // element is the jquery obj clicked on when context menu launched
                    $('#HiddenSeleccion').val("-1");
                    $('#GridView2 tr[id]').css({ "background-color": "White", "color": "Black" });
                }, klass: "fad fa-stream"
            },
            'Exportar lista de tareas': {
                click: function (element) {  // element is the jquery obj clicked on when context menu launched
                    activa_export_lista();
                    
                }, klass: "fad fa-file-export"
            },
            'Actualizar lista de tareas': {
                click: function (element) {  // element is the jquery obj clicked on when context menu launched
                    activa_boton_client_server('ImageButtonactualizar');
                }, klass: "fad fa-sync-alt"
            },
            'Ver documentos': {
                click: function (element) {  
                    if (document.getElementById("Hidden_id_tarea_sel").value == "-1") {
                        alert("Debe selecionar la tarea para visualizar los documentos ");
                    } else {
                        document.getElementById("Hidden_tipo_visor").value = "VISOR WORKFLOW";
                        document.getElementById("Button_visor_emergente").click();
                    }
                    
                }, klass: "fal fa-folder-open"
            },               
            'Notas del registro': {
                click: function (element) { document.getElementById("ImageButtonanotacion_").click(); }, klass: "fad fa-sticky-note"
            },
            'Lista de autorizaciones': {
                click: function (element) { document.getElementById("ImageButton_ista_autorizacio_").click(); }, klass: "fal fa-list-ul"
            },
            'Salir del Menu': {
                click: function (element) { },klass: "fad fa-times"
            }
        }
          );
        
    $("#noaming").bind("contextmenu", function (e) {
        e.preventDefault();
    });
    var left;
    var top;
   
})

var WF_ESTATUS_SERVICE;
var ESTADO_INICIALIZACION = -4;
var ESTADO_EVENT_GENERAL = "ini";
var INTERVAL_EVENT_GENERAL;
var ITEMS_IMAGE_LIST_WF;
let SEND_ENVIO_MAIL = 0;  //Determina el envio de correo para la gestio del usuario
let GESTION_USUARIO_WF_ARRAY = new Array(); //Copia array de datos gestio del usuario
let RADICADO_WORKFLOW = ""; // save settled document
let asmxClient;  //-------wapper de consumo de asmx
$(window).on("load", function () {
    try {
        var elment = document.getElementsByClassName("da_event_captive");
        if (elment) {
            for (var i = 0; i < elment.length; i++) {
                elment[i].addEventListener("click", event_click, false);
            }
        }
        document.getElementById("boton_event_actualiza_indice_batch_wf").addEventListener("click", botom_boton_event_actualiza_indice_batch_wf, false);
        document.getElementById("boton_event_actualiza_indice_batch_wf_enlace").addEventListener("click", botom_boton_event_actualiza_indice_batch_wf_enlace, false);
        document.getElementById("auto_complex").addEventListener("keypress", search_list_lista_wf, false);
        ini_event_page();
        inicia_seleccion_workflow();
        window.addEventListener("resize", rezize_event);
        ShowModalPopup("ModalPopupExtender_mensaje_personalizado_backgroundElement", "Panel_mensaje_personalizado", 100009);
        ShowModalPopup("ModalPopupExtender_edition_pro_gres_bar_backgroundElement", "Panel_pro_gres_bar", 100008);
        ShowModalPopup("ModalPopupExtender_sube_documento_adjunto_backgroundElement", "Panel_sube_documento_adjunto", 100001);
        ShowModalPopup("ModalPopupExtender_edition_actualiza_tipologia_documental_backgroundElement", "Panel_actualiza_tipologia_documental", 100001);
        ShowModalPopup("ModalPopupExtender_edition_detalle_actividad_flujo_backgroundElement", "Panel_detalle_actividad_flujo", 100001);
        ShowModalPopup("ModalPopupExtender_edition_autoriza_reasignacion_tarea_recuperada_enlazada_backgroundElement", "Panel_autoriza_reasignacion_tarea_recuperada_enlazada", 100001);
        ShowModalPopup("ModalPopupExtender_edition_detalle_actividad_flujo_user_backgroundElement", "Panel_detalle_actividad_flujo_user", 100001);
        ShowModalPopup("ModalPopupExtender_edition_nota_respuesta_backgroundElement", "Panel_nota_respuesta", 100004);
        ShowModalPopup("ModalPopupExtender_detalle_respuesta_backgroundElement", "Panel_detalle_respuesta", 100001);
        ShowModalPopup("ModalPopupExtender_visor_tareas_pendiente_backgroundElement", "Panel_visor_tareas_pendiente", 100001);
        ShowModalPopup("ModalPopupExtender_edition_list_imagenes_sii_backgroundElement", "Panel_list_imagenes_sii", 100001);
        ShowModalPopup("ModalPopupExtender_edition_list_inscripciones_sii_backgroundElement", "Panel_list_inscripciones_sii", 100001);
        ShowModalPopup("ModalPopupExtender_sube_documento_integra_sii_backgroundElement", "Panel_sube_documento_integra_sii", 100002);
        ShowModalPopup("ModalPopupExtender_copiar_estructura_backgroundElement", "Panel_copiar_estructura", 100001);
        ShowModalPopup("ModalPopupExtender_edition_interface_consulta_meta_dato_backgroundElement", "Panel_interface_consulta_meta_dato", 100002);
        ShowModalPopup("ModalPopupExtender_edition_interface_regitra_meta_dato_backgroundElement", "Panel_interface_regitra_meta_dato", 100002);
        ShowModalPopup("ModalPopupExtender_edition_consulta_avanzada_ruta_workflow_backgroundElement", "Panel_consulta_avanzada_ruta_workflow", 100002);
        ShowModalPopup("ModalPopupExtender_edition_exporta_gabinete_workflow_backgroundElement", "Panel_exporta_gabinete_workflow", 100002);
        ShowModalPopup("ModalPopupExtender_edition_actualiza_indice_batch_wf_backgroundElement", "Panel_actualiza_indice_batch_wf", 100002);
        ShowModalPopup("ModalPopupExtender_edition_actualiza_indice_batch_wf_enlace_backgroundElement", "Panel_actualiza_indice_batch_wf_enlace", 100002);
        ShowModalPopup("ModalPopupExtender_edition_detail_document_proces_workflow_backgroundElement", "Panel_detail_document_proces_workflow", 100001);
        ShowModalPopup("ModalPopupExtender_edition_detail_copy_document_expediente_wf_backgroundElement", "Panel_detail_copy_document_expediente_wf", 100001);
        Tokenize2_token_initial("tokenize-gestion-gestion-user");
       
        
       
    } catch (e) {
        alert(" funcion load " + e.message);
    }

});
const search_list_lista_wf = () => {
    if (event.key === "Enter") {
        event.preventDefault();
        document.getElementById("Button_tool_search_lista_tareas").click();
    }
}

const ini_event_page = () => {
    asmxClient = new ASMXClient(AsmxServicesConfig);
    //Active copy proceedings
    let array_element = new Array;
    array_element.push({ id: "a_copy_document_production_proceedings" }, { id: "a_copy_document_proceedings" },
        { id: "a_link_document_proceedings" }, { id: "a_auto_link_document_proceedings" },
        { id: "Button_actualizar_nota" }, { id: "a_copy_document_production_proceedings_" }, { id: "a_copy_document_proceedings_" },
        { id: "a_link_document_proceedings_" }, { id: "a_auto_link_document_proceedings_" }, { id: "a_list_copy_document_expedient" },
        { id: "btn_nav_registra_gestion_user" }, { id: "btn_nav_lista_gestion_user" }, { id: "Button_registra_gestion" }, { id: "Button_actualiza_registra_gestion" },
        { id: "boton_menu_stamp_firm" }, { id: "btnloadservice" }, { id: "Button_Activa_guardar_Multiplex_Constancias_sii" }, { id: "Button_guarda_inscipciones_sii" },
        { id: "a_adj_service_web" }, { id: "Button_guarda_anexos_sii" }, { id: "btnLoadFile" }, { id: "btnLoadFileEnlace"}
    );
    for (let i = 0; i < array_element.length; i++) {
        let elment_a_document_production = document.getElementById(array_element[i].id);
        if (elment_a_document_production) {
            elment_a_document_production.addEventListener("click", handler_element_event, false);
        }
    }
    //active note procesing
    array_element = new Array;
    array_element.push(
        { id: "Button_actualizar_nota" }, { id: "Button_Show_Guardar" }, { id: "Button_duardar_nota" }
    );
    for (let i = 0; i < array_element.length; i++) {
        let elment_a_document_production = document.getElementById(array_element[i].id);
        if (elment_a_document_production) {
            elment_a_document_production.addEventListener("click", handler_element_event, false);
        }
    }
    //active detalil procesing image workflow
    array_element = new Array;
    array_element.push(
        { id: "a_list_operation_document" }
    );
    for (let i = 0; i < array_element.length; i++) {
        let elment_a_document_production = document.getElementById(array_element[i].id);
        if (elment_a_document_production) {
            elment_a_document_production.addEventListener("click", handler_element_event, false);
        }
    }
}
function rezize_event() {
    try {
        auto_zise_popup_list_imagenes_sii();
        auto_zise_popup_workflow(1);
        auto_zise_popup_lista_actividades_ruta();
        auto_zise_popup_documentos_radicados_relacionados();
        auto_zise_popup_ventana_externa();
        auto_zise_popup_envia_usuario_grupo();
        auto_zise_popup_copiar_estructura();
        selection_event();
        auto_zise_popup_detalle_transacciones();
        auto_zise_popup_pendinetes();
        auto_zise_popup_recuperar_tarea();
        auto_zise_popup_detalle_trazabilidad();
        auto_zise_popup_trace_grafic();
        auto_zise_popup_respuesta();
        auto_zise_popup_detalle_respuesta();
        auto_zise_popup_compartir_documento();
        actuo_zise_popup_compartir_correo_electronico();
        auto_zise_popup_visor_externo();
        auto_zise_popup_detalle_sesion_workflow();
        auto_zise_popup_detalle_tarea_workflow();
        auto_zise_popup_adjunta_imagen_digitalizada();
        auto_zise_popup_estado_paginacion();
        service_posibles_datos_tramites_();
        service_GetPosiblesDatos_lista_tareas_pendientes();
        auto_zise_popup_detalle_radicado();
        auto_zise_popup_lista_usuario_flujo();
        auto_zise_popup_autorizados();
        auto_size_content_anotacion();
        auto_zise_popup_visor_tarea_pendiente();
        auto_zise_nota_tarea();
        auto_zise_tareas_pendientes();
        auto_zise_popup_consulta_meta_dato();
        auto_zise_popup_detalle_grupo_usuario();
        auto_zise_popup_adjunta_documento_workflow();
        auto_zise_popup_list_inscripciones_sii();
        GetLista_lista_actividades_ruta("TextBox_buequeda_general_lista_actividades");
        $.fn.redienciona();
        auto_zise_popup_lista_form_control_person("actualiza_indice_batch_wf");
        auto_zise_popup_lista_form_control_person("actualiza_indice_batch_wf_enlace");
        auto_zise_popup_modal_conten_procesing_image_worflow("div_content_tabla_procesa_detail_document_proces_workflow", "contenido_procesa_detail_document_proces_workflow");
        auto_zise_popup_modal_conten_copy_document_expediente("div_content_tabla_procesa_detail_copy_document_expediente_wf", "div_content_tabla_procesa_detail_copy_document_expediente_wf");
        
        
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
        alert('Exception in ShowModalPopup: ' + name_panel + modalPopupId_ + ex.message);
    }
}
function event_click(e) {
    try {
        var k = e.currentTarget.value;
        var d = document.getElementById(k);
        //Caso setea iframe copia y exporta a documentos
        if (d.id == "ButtonSalir_copiar_estructura") {
            let element_iframe = document.getElementById("Iframe_copiar_estructura_");
            if (element_iframe) {
                element_iframe.setAttribute("src", "");
            }     
        }
        d.click();
        e.preventDefault();
    }
    catch (err) {
        alert(err.message + " Funcion event_click");
    }
}
const handler_element_event = (e) => {
    try {
        delete_alert_boot();
        let name_ID = e.currentTarget.id;
        let result = "";
        switch (name_ID) {
            case "btnLoadFileEnlace":  //Activa la carga de archivos desde dispotivos cuando el documento esta en enlace 
                event_element_click_promise(e);
                break;
            case "btnLoadFile":  //Activa la carga de archivos desde dispotivos cuando el documento esta asignado
                event_element_click_promise(e);
                break;
            case "Button_guarda_anexos_sii" :
                 event_element_click_promise(e);
                break;
            case "a_adj_service_web":
                event_element_click_promise(e);
                break;
            case "Button_Activa_guardar_Multiplex_Constancias_sii":
                event_element_click_promise(e);
                break;
            case "Button_guarda_inscipciones_sii":
                event_element_click_promise(e);
                break;
            case "btnloadservice":
                event_element_click_promise(e);
                break;
            case "boton_menu_stamp_firm":
                event_element_click_promise(e);
                break;    
            case "Button_registra_gestion":
                event_element_click_promise(e);
                break;
            case "btn_nav_registra_gestion_user":
                event_element_click_promise(e);
                break;
            case "btn_nav_lista_gestion_user":
                event_element_click_promise(e);
                break;
            case "Button_actualiza_registra_gestion":
                event_element_click_promise(e);
                break;
            case "a_copy_document_production_proceedings":
                result = Event_document_proceesings("CP-DC-EXP-PRO", "", "GridView_list_documento_relacion_wf", "chek_selecion_list_wf");
                if (result !== "YES") {
                    alert(result);
                }
                break;
            case "a_copy_document_proceedings":
                result = Event_document_proceesings("CP-DC-EXP", "", "GridView_list_documento_relacion_wf", "chek_selecion_list_wf");
                if (result !== "YES") {
                    alert(result);
                }
                break;
            case "a_link_document_proceedings":
                result = Event_document_proceesings("C-DW-VIN", "", "GridView_list_documento_relacion_wf", "chek_selecion_list_wf");
                if (result !== "YES") {
                    alert(result);
                }
                break;
            case "a_auto_link_document_proceedings":
                result = Event_document_proceesings("C-DW-AUTO-VIN", "crear un expediente automatico y vincular a un expediente ", "GridView_list_documento_relacion_wf", "chek_selecion_list_wf");
                if (result !== "YES") {
                    alert(result);
                }
                break; 
            case "a_copy_document_production_proceedings_":
                result = Event_document_proceesings("CP-DC-EXP-PRO", "", "GridView_list_documento_relacion_wf", "chek_selecion_list_wf");
                if (result !== "YES") {
                    alert(result);
                }
                break;
            case "a_copy_document_proceedings_":
                result = Event_document_proceesings("CP-DC-EXP", "", "GridView_list_documento_relacion_wf", "chek_selecion_list_wf");
                if (result !== "YES") {
                    alert(result);
                }
                break;
            case "a_link_document_proceedings_":
                result = Event_document_proceesings("C-DW-VIN", "", "GridView_list_documento_relacion_wf", "chek_selecion_list_wf");
                if (result !== "YES") {
                    alert(result);
                }
                break;
            case "a_auto_link_document_proceedings_":
                result = Event_document_proceesings("C-DW-AUTO-VIN", "", "GridView_list_documento_relacion_wf", "chek_selecion_list_wf");
                if (result !== "YES") {
                    alert(result);
                }
                break;
            case "Button_actualizar_nota":
                result = Event_note_workflow(document.getElementById("hdnidlista").value, document.getElementById("TextBox_nota").value, "Button_actualizar_nota");
                if (result !== "YES") {
                    alert(result);
                }
                break;
            case "Button_Show_Guardar":
                result = Event_note_workflow("", "", "Button_Show_Guardar");
                if (result !== "YES") {
                    alert(result);
                }
                break;   
            case "Button_duardar_nota":
                result = Event_note_workflow("", document.getElementById("TextBox_nota").value, "Button_duardar_nota");
                if (result !== "YES") {
                    alert(result);
                }
                break;
            case "a_list_operation_document":
                let resutl = handler_show_detail_document_procesing("table_boot_detail_document", "", "C-DW-DETAIL-DOCUMENT");
                if (resutl !== "YES") {
                    alert(resutl);
                }
                break;
            case "a_list_copy_document_expedient":
                result = handler_show_detail_document_procesing("table_boot_detail_copy_document_expdient", "", "C-DW-DETAIL-COPY-DOCUMENT");
                if (result !== "YES") {
                    alert(result);
                }
                break;

  }
    } catch (ex) {
        alert(ex.mensaje);
    }
}
//ZONA EVENTOS 
//-----Activa los evento asincronos
let OptionItem = new Array();
const event_element_click_promise = async (e) => {
    let name_control = e.currentTarget.id;
    try {
        let result = "";
        delete_alert_boot();
        e.currentTarget.disabled = true;
        posicion_update_pogres('progres_bar');
        //----Activa la carga de archivos desde dispotivos cuando el documento esta en estado de enlace------//
        if (name_control == "btnLoadFileEnlace") {
            result = await ActivaAdjuntarDocumentoWorkflowEnlace();
            if (result != "YES") {
                alert_bot(result, 'warning', "contenido_procesa_admon_documentos");
                return true;
            }
        }
         //----Activa la carga de archivos desde dispotivos cuando el documento esta asignado------//
        if (name_control == "btnLoadFile") {
            result = await ActivaAdjuntarDocumentoWorkflow();
            if (result != "YES") {
                alert_bot(result, 'warning', "div_error_content_wf");
                return true;
            }
        }
        //------Activa lista amexos recibo SII---------//
        if (name_control == "a_adj_service_web") {
            result = await ActivaListaAnexosIntegracionSII();
            if (result != "YES") {
                alert_bot(result, 'warning', "error_div_selecion_tarea_rad");
                return true;
            }
        }
        //------Guarda anexos SII------////
        if (name_control == "Button_guarda_anexos_sii") {
            result = await GuardaDocumentoAnexoSII();
            if (result != "YES") {
                alert_bot(result, 'warning', "error_tipo_guarda_anexo");
                return true;
            }
        }
        //------Activa subir archivo desde servicios web de integración -----///
        //-----Listas inscripciones siii Ingración SII---------////
        if (name_control == "btnloadservice") {
            result = await ActivaServicioIntegracionAdjuntaDocumentosSistemasExternos();
        }
        //----Activa guardar multiplex registros de inscripcion SII y abre la ventana y lista las tipologias-------//
        if (name_control == "Button_Activa_guardar_Multiplex_Constancias_sii") {
            result = await ActivaGuardarMultiplexConstanciasInscription();
            if (result != "YES") {
                alert_bot(result, 'warning', "error_content_sii_constancias_inscripcion");
                return true;
            }
        }   
        //----Guardar inscripcion SII-------//
        if (name_control == "Button_guarda_inscipciones_sii") {
            result = await GuardarConstanciaIncripcionSII(0);
        }
        //Activa la interface 
        if (name_control == "btn_nav_registra_gestion_user") {        
            result = await Service_REST_crea_interface_registro_gestion(0);
            if (result != "YES") {
                alert_bot(result, 'warning', "conten_error_seleccion_task");
                return true;
            } else {
                Tokenize2_token_initial("tokenize-gestion-gestion-user");
            }
          
        }
       //Registra gestion al usuario
        if (name_control == "Button_registra_gestion") {
            let ElmentSelect = document.getElementById("option_tipos_gestion");
            let value_tipo_gestion = ElmentSelect.options[ElmentSelect.selectedIndex].value;
            if (value_tipo_gestion == 0) {
                alert_bot("Debe seleccionar el tipo de gestión", 'warning', "error_registro_gestion_usuario");
                ElmentSelect.focus();
                return true;
            }
            let ElementTextContent = document.getElementById("ContestTextarea");
            if (ElementTextContent.value == "") {
                alert_bot("Debe informar la gestión", 'warning', "error_registro_gestion_usuario");
                ElementTextContent.focus();
                return true;
            }
            let array_class_wf_gestion = new Array();
            array_class_wf_gestion.push({wf_gestion_tipos_id_tipo_gestion : value_tipo_gestion, content_gestion: ElementTextContent.value });
            result = await Service_REST_registra_gestion_al_usuario(array_class_wf_gestion);
            if (result != "YES") {
                alert_bot(result, 'warning', "error_registro_gestion_usuario");
            }
        }
        //Activa la lista de la gestion  
        if (name_control == "btn_nav_lista_gestion_user") {
            result = await Service_REST_lista_gestion_al_usuario();
            if (result != "YES") {
                alert_bot(result, 'warning', "div_error_content_wf");
            }
        }
        //Actualiza la gestion del usuario 
        if (name_control == "Button_actualiza_registra_gestion") {
            
            let ElementTextContent = document.getElementById("ContestTextareaActualiza");
            if (ElementTextContent.value == "") {
                alert_bot("Debe informar la gestión", 'warning', "error_edicion_registro_gestion_usuario");
                ElementTextContent.focus();
                return true;
            }
            GESTION_USUARIO_WF_ARRAY[0].content_gestion=ElementTextContent.value;
            result = await Service_REST_actualiza_registro_gestion_al_usuario(GESTION_USUARIO_WF_ARRAY);
            if (result != "YES") {
                alert_bot(result, 'warning', "error_edicion_registro_gestion_usuario");
                return true;
            }
        }
        //Activa firmas digitales multiplex en función de asignacion  
        if (name_control == "boton_menu_stamp_firm") {
            progres_hiden('progres_bar');
            let option = ({
                module: "1", valida_firma: "1", 
                name_campo_estado_firma: "", name_tipo_table: "aspnettable",
                name_table: "GridView_list_documento_relacion_wf",
                AtributeSingAspNet: "idd_wf",
                NameControlParent: "div_content_general_wf",
                content_error: "error_div_selecion_tarea_wf"
            });
            result = await LoadStampMultipleSing(option);
            if (result !== "YES") {
                alert_bot(result, 'warning', "error_div_selecion_tarea_wf");
            }
        }
    }
    catch (ex) {
        alert_bot(ex.message, 'warning', "div_error_content_wf");
    } finally {
        progres_hiden('progres_bar');
        document.getElementById(name_control).disabled = false;
    }
}
const Event_document_proceesings = (ident_proceesing, description_proceesing, name_table_gred, name_class_item) => {
    try {
        ITEMS_IMAGE_LIST_WF = new Array;
        ITEMS_IMAGE_LIST_WF = table_gred_select_check_item(name_table_gred, name_class_item);
        if (ITEMS_IMAGE_LIST_WF.length == 0) {
            return "Debe seleccionar los documentos de la lista de documentos relacionados";
        }
        let mensaje = "";
        switch (ident_proceesing) {
            case "CP-DC-EXP-PRO":
                mensaje = "Desea copiar " + "(" + ITEMS_IMAGE_LIST_WF.length + ") documento (s) a un expediente de su producción documental";
                break;
            case "CP-DC-EXP":
                mensaje = "Desea copiar " + "(" + ITEMS_IMAGE_LIST_WF.length + ") documento (s) a un expediente";
                break;
            case "C-DW-VIN":
                mensaje = "Desea vincular " + "(" + ITEMS_IMAGE_LIST_WF.length + ") documento (s) a un expediente";
                break;
            case "C-DW-AUTO-VIN":
                mensaje = "Desea crear un expediente automatico y vincular " + "(" + ITEMS_IMAGE_LIST_WF.length + ") documento (s) al expediente";
                break;
        }
        var resp = confirm(mensaje);
        if (resp == false) {
            return "YES";
        } else {
            event_element_menu(ident_proceesing, "");
            return "YES";
        }

    } catch (ex) {
        return "function Event_document_proceesings error : " + ex.mensaje;
    }   
}
const Event_note_workflow = (ident_note, date_note,ident_booton ) => {
    try {
        if (ident_booton == "Button_actualizar_nota") {
            if (ident_note== - 1 || ident_note == 1) {
                return "Debe selecionar la nota";
            }
            if (date_note == "" ) {
                return "Debe informar la nota";
            }
            event_element_menu(ident_booton, "");
            return "YES"
        }
        //Activa ventana guardar nota
        if (ident_booton == "Button_Show_Guardar") {
            let result = Show_new_note_workflow();
            return result;
        }
        //Guarda la nota workflow
        if (ident_booton == "Button_duardar_nota") {
            event_element_menu(ident_booton, "");
            return "YES"
        }
        return "YES"
    } catch (ex) {
        return "Error funcion Event_note_workflow " & ex.mensaje
    }
}
//Eventos detail procesing document workflow 
//--------Activa el show de las transaciones de la tarea con documentos
let ID_TAREA_WORKFLOW_WF = 0;  
const handler_show_detail_document_procesing = (name_table, name_class, event_name) => {
    try {
        if (document.getElementById("Hidden_id_tarea_selecionada").value == 1 || document.getElementById("Hidden_id_tarea_selecionada").value == -1) {
            return "Debe selecionar la tarea";
        }
        ID_TAREA_WORKFLOW_WF = document.getElementById("Hidden_id_tarea_selecionada").value;
        event_element_menu(event_name, name_table);
        return "YES";
    } catch (ex) {
        return ex.mensaje;
    }
}
//----EVENTOS CONTROL SELECT-----------
//evento seleccion tipo ggestion usuario workflow

const event_change_drowslisi_lista_tipos_gestion_usuario = async (e) => {
    try {
        delete_alert_boot();
        let ecourrent = e.currentTarget;
        let value_e = ecourrent.value;
        let text_e = ecourrent.options[ecourrent.selectedIndex].label;
        if (value_e == 0) { return true; }
        let estado_send = 0;
        posicion_update_pogres('progres_bar');
        let result = await Service_REST_solicita_estado_envio_correo_gestion_usuario(value_e, estado_send);
        if (result !== "YES") {
            alert_bot(result, 'warning', "error_registro_gestion_usuario");
            return true
        } else {
            if (SEND_ENVIO_MAIL == 1) {
                document.getElementById("content_send_mail").classList.remove("d-none");    
            } else {
                document.getElementById("content_send_mail").classList.add("d-none");
            }
        }
             
    } catch (ex) {
        alert_bot(ex.mensaje, 'warning', "error_registro_gestion_usuario");
    } finally {
        progres_hiden('progres_bar');
    }
}
//Active ventana add new note task workflow
const Show_new_note_workflow = () => {
    try {
        document.getElementById("TextBox_nota").value = "";
        document.getElementById("Button_actualizar_nota").style.display = "none";
        document.getElementById("Button_duardar_nota").style.display = "flex";
        document.getElementById("Label_nota_respuesta").innerHTML = "Nueva nota";
        $find("ModalPopupExtender_edition_nota_respuesta").show();
        auto_zise_nota_tarea();
        return "YES";
    }
    catch (ex) {
    return "Error funcion Event_note_workflow " & ex.mensaje
    }
}
function botom_boton_event_actualiza_indice_batch_wf(e) {
    try {
        let vresult = valida_solicita_datos_control_general("form_control_indice_docuarchi");
        if (vresult != "YES") {
            alert_bot(vresult, 'warning', 'modal_content_actualiza_indice_batch_wf');
            return true;
        } else {
            event_element_clic("", e.currentTarget);
            return true;
        }
    } catch (ex) {
        alert("funcion botom_boton_event_actualiza_indice_batch_wf " + ex.mensaje);
    }
}
function botom_boton_event_actualiza_indice_batch_wf_enlace(e) {
    try {
        let vresult = valida_solicita_datos_control_general("form_control_indice_docuarchi");
        if (vresult != "YES") {
            alert_bot(vresult, 'warning', 'modal_content_actualiza_indice_batch_wf_enlace');
            return true;
        } else {
            event_element_clic("", e.currentTarget);
            return true;
        }
    } catch (ex) {
        alert("funcion botom_boton_event_actualiza_indice_batch_wf_enlace " + ex.mensaje);
    }
}
function timer() {
    if (document.getElementById("Hidden_00020_4001").value == 1) {
        setInterval('alarma_actividad_pendiente()', '600');
    }  
    setInterval('alarma_nota_actividad()', '600');
    if (document.getElementById("Hidden_intervalo_search").value != -1) {
        setInterval('activa_busqueda_tareas_wf()', document.getElementById("Hidden_intervalo_search").value);
    }
   
}
//ZONA CHEK DATA GREED
function table_gred_on_click_check(elemen, table,name_class_check) {
    try {
        var value_ = true;
        if (elemen.checked == true) {
            value_ = true;
        } else {
            value_ = false;
        }
        $('#' + table + ' .' + name_class_check).each(function () {
            var nod = $(this);
            nod[0].checked = value_;
        });
       
    } catch (ex) {
        alert("Inconsistencia funcion table_gred_on_click_check " + ex.mensaje);
    }
}
function table_gred_select_check_item(table, name_class_check) {
    try {
        let items_check_id = new Array;
        $('#' + table + ' .' + name_class_check).each(function () {
            var nod = $(this);
            if (nod[0].checked == true) {
                if (nod[0].getAttribute("chek_id")) {
                    items_check_id.push({
                        id_item: nod[0].getAttribute("chek_id")
                    });
                }    
            }
        });
        return items_check_id;
    } catch (ex) {
        alert("Inconsistencia funcion table_gred_select_check_item " + ex.mensaje);
    }
}
//-------------------ZONA EVENTOS TABLE BOOT---------------------------
//eventos tabla lista gestión al usuario
function operateFormattertablebootListaGestionUser(value, row, index) {
    return [
        '<div class="row pl-2">',
        '<div class="col-12 p-0">',
        '<a class="active_view_gestion_usuario nav-link pl-5 justify-content-end font-weight-light" style="color: black" href="javascript:void(0)" title="Ir a la gestión del usuario">  <i style="color: black" class="fal fa-file-image"></i>  </a>',
        '</div>',
        '</div>',
    ].join('')
}
window.operateEventsListaGestionUser = {
    'click .active_view_gestion_usuario': (e, value, row, index) => {
        delete_alert_boot();
        let ident = table_boot_return_objet_jonson(row);
        active_interfaz_edicion_gestion_al_usuario(ident.id_gestion_tarea_usuario);

    }
}
function operateFormattertablebootListaConstancia(value, row, index) {
    return [
        '<div class="row">',
        '<div class="col-12">',
        '<a class="active_save_constancia nav-link  font-weight-light" style="" href="javascript:void(0)" title="Guardar contancia de inscripción">  <i style="" class="fad fa-save"></i>  </a>',
        '</div>',
        '</div>',
    ].join('')
}
window.operateEventsConstanciasInscripcion = {
    'click .active_save_constancia': (e, value, row, index) => {
        delete_alert_boot();
        let ident = table_boot_return_objet_jonson(row);
        ActivaGuardarContasnciaInscripcion(ident);
    }
}
window.operateEventsAnexosSII = {
    'click .active_save_anexo': (e, value, row, index) => {
        delete_alert_boot();
        let ident = table_boot_return_objet_jonson(row);
        ActivaVentanaGuardarAnexosSII(ident);
    }
}
function operateFormattertablebootListaAnexosSII(value, row, index) {
    return [
        '<div class="row">',
        '<div class="col-12">',
        '<a class="active_save_anexo nav-link  font-weight-light" style="" href="javascript:void(0)" title="Guardar anexo SII">  <i style="" class="fad fa-save"></i>  </a>',
        '</div>',
        '</div>',
    ].join('')
}
//Activa la interfaz de edición de la gestión del usuario 
const active_interfaz_edicion_gestion_al_usuario = async (id_registro_gestion) => {
    try {
        posicion_update_pogres('progres_bar');
        let result = "";
        result = await Service_REST_crea_interfaz_gestion_al_usuario(id_registro_gestion);
        if (result !== "YES") {
            alert_bot(result, 'warning', "error_lista_gestion_usuario");
            return true;
        } else {
            if (GESTION_USUARIO_WF_ARRAY[0].Estado_envio_correo == 1) {
                document.getElementById("content_actualiza_send_mail").classList.remove("d-none");
            } else {
                document.getElementById("content_actualiza_send_mail").classList.add("d-none");
            }
            document.getElementById("ContestTextareaActualiza").value = GESTION_USUARIO_WF_ARRAY[0].content_gestion;
            $("#modal_actualiza_registro_gestion_usuario_wf").modal("show");
        }
       
    } catch (ex) {
        alert_bot(ex.mensaje, 'warning', "error_lista_gestion_usuario");
    } finally {
        progres_hiden('progres_bar');
    }
}
//-------------TERMINA ZONA EVENTOS TABLE BOOT--------------------
function prevent_tool_menucab(event, element, value) {
    try {
        document.getElementById("Hidden_menucab").value = value;
        document.getElementById("Hidden_tip_adjunt").value = "wf";
        event.preventDefault();
        document.getElementById("Button_tool_menucab").click();
        element.focus();
    }
    catch (err) {
        alert(err.message + " Funcion prevent_tool_menucab");
    }
}
//CONTROLA LOS EVENTOS DE LOS INPUT
function event_element_clic(event, e) {
    try {
        ESTADO_EVENT_GENERAL = "intro";
        posicion_update_pogres('progres_bar');
        e.disabled = true;
        INTERVAL_EVENT_GENERAL = setInterval(fx_funcion, 50);
        function fx_funcion() {
            //--Sale del evento
            if (ESTADO_EVENT_GENERAL == "out") {
                progres_hiden('progres_bar');
                e.disabled = false;
                clearInterval(INTERVAL_EVENT_GENERAL);
                ESTADO_EVENT_GENERAL = "";
            }
            //--Entra al evento
            if (ESTADO_EVENT_GENERAL == "intro") {
                ESTADO_EVENT_GENERAL = "";
                if (e.id == "Button_registra_meta") {
                    if (ars_sele.length == 0) {
                        agrega_meta_dato_documento(ID_IMAGEN_META_DATO, GABINETE_META_DATO, RADICADO_META_DATO, ID_TAREA_META_DATO, 1, 1, 1, ID_BOTON_META_DATO);
                    } else {
                        progres_hiden('progres_bar');
                        e.disabled = false;
                        clearInterval(INTERVAL_EVENT_GENERAL);
                        ESTADO_EVENT_GENERAL = "";
                        $find("ModalPopupExtender_edition_interface_regitra_meta_dato").hide();
                        event_multiple_row(event, GridView_list_documento_relacion_wf, "firma_multiple_digital_documento");
                       
                    }           
                    return true;
                }
                if (e.id == "id_indice_wf_pdf") {
                    Service_Solicita_listar_meta_datos_Archivo(ID_IMAGEN_VIS_WF, GABIENTE_VIS_WF);
                    return true;
                }
                if (e.id == "id_indice_wf_pdf_draw") {
                    Service_Solicita_listar_meta_datos_Archivo(ID_IMAGEN_VIS_WF, GABIENTE_VIS_WF);
                    return true;
                }
                if (e.id == "a_lement_actualiza_index") {
                    service_actualiza_indice_workflow(1);
                    return true;
                } 
                if (e.id == "bnt_search_avance") {
                    Service_interface_form_control(0, 'WebServiceWorkflow.asmx', 'Service_solicita_structucre_consulta_ruta',
                        'ModalPopupExtender_edition_consulta_avanzada_ruta_workflow','div_consulta_avanzada',0,"");
                    return true;
                }
                if (e.id == "bnt_eval_event_default") {
                    Service_Eval_tarea_default_workflow();
                    return true;
                }
                //Exporta lista documentos enlace a gabinete  
                if (e.id == "Button_exporta_gabinete_workflow") {
                    var value_gabinete = "";
                    var control_element_ = document.getElementById("DropDownList_exporta_gabinete_workflow");
                    if (control_element_.options[control_element_.selectedIndex].text == "") {       
                        alert("Debe seleccionar el gabinete a exportar");
                    } else {
                        value_gabinete = control_element_.options[control_element_.selectedIndex].text;
                        event_multiple_row(event, value_gabinete, "exporta_gabinete_workflow_enlace");
                        $find('ModalPopupExtender_edition_exporta_gabinete_workflow').hide();      
                    }      
                }
                //Actualiza indice batch
                if (e.id == "boton_event_actualiza_indice_batch_wf") {
                    ITEM_GENERAL_CONTROL_ARRAY_DIFERENT = new Array();
                    ITEM_GENERAL_CONTROL_ARRAY_DIFERENT = Detec_chanque_valor_campo();
                    if (ITEM_GENERAL_CONTROL_ARRAY_DIFERENT.length == 0) {
                        progres_hiden('progres_bar');
                        e.disabled = false;
                        clearInterval(INTERVAL_EVENT_GENERAL);
                        ESTADO_EVENT_GENERAL = "";
                        return true;
                    }
                    event_multiple_row("ModalPopupExtender_edition_actualiza_indice_batch_wf", "", "actualiza_indice_batch_wf");
                }
                //Update index  batch workflow link
                if (e.id == "boton_event_actualiza_indice_batch_wf_enlace") {
                    ITEM_GENERAL_CONTROL_ARRAY_DIFERENT = new Array();
                    ITEM_GENERAL_CONTROL_ARRAY_DIFERENT = Detec_chanque_valor_campo();
                    if (ITEM_GENERAL_CONTROL_ARRAY_DIFERENT.length == 0) {
                        progres_hiden('progres_bar');
                        e.disabled = false;
                        clearInterval(INTERVAL_EVENT_GENERAL);
                        ESTADO_EVENT_GENERAL = "";
                        return true;
                    }
                    event_multiple_row("ModalPopupExtender_edition_actualiza_indice_batch_wf_enlace", "", "actualiza_indice_batch_wf_enlace");
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
        if (ESTADO_EVENT_GENERAL == "execute") {
            return true;
        }
        ESTADO_EVENT_GENERAL = "intro";
        posicion_update_pogres('progres_bar');
        INTERVAL_EVENT_GENERAL = setInterval(fx_funcion, 50);
        function fx_funcion() {
            //--Sale del evento  
            if (ESTADO_EVENT_GENERAL == "out") {
                ESTADO_EVENT_GENERAL = "";
                progres_hiden('progres_bar');
                clearInterval(INTERVAL_EVENT_GENERAL);     
            }
            //--Entra al evento
            if (ESTADO_EVENT_GENERAL == "intro") {
                ESTADO_EVENT_GENERAL = "execute";
                //Auto vincula documento a expediente
                if (evento == "A-VIN-DE") {
                    Service_auto_vincula_documentos_a_expediente();
                    return true;
                }
                 //sube documentos en la lista
                if (evento == "C-DW-LISTA") {
                    inicializa_upload_file_client(tip_event);
                    document.getElementById("Hidden_tip_adjunt").value = "wf";
                    parameter_upload(ESTADO_EVENT_GENERAL, "WORKFLOW", "Button_tool_activa_sube_documento_lista", "multiple", tip_event);
                    return true;
                }
                //sube documento visor
                if (evento == "C-DW-VIS") {
                    inicializa_upload_file_client(tip_event);
                    document.getElementById("Hidden_tip_adjunt").value = "wf";
                    parameter_upload(ESTADO_EVENT_GENERAL, "WORKFLOW", "Button_tool_activa_sube_documento", "", tip_event);
                    return true;
                }
                //sube documento enlace workflow
                if (evento == "C-DW-RD") {
                    inicializa_upload_file_client(tip_event);
                    document.getElementById("Hidden_tip_adjunt").value = "rad";
                    parameter_upload(ESTADO_EVENT_GENERAL, "WORKFLOW", "Button_tool_activa_sube_documento_enlace", "multiple", tip_event);
                    return true;
                }
                //--Crea interface multiple meta dato documento signado
                if (evento == "firma_multiple_digital_documento") {
                    var spliter = tip_event.split("|");
                    ITEMS_DATOS_SIST_META_ARCHIVO = new Array();
                    ID_IMAGEN_META_DATO = spliter[1];
                    GABINETE_META_DATO = spliter[0];
                    RADICADO_META_DATO = spliter[2];
                    ID_TAREA_META_DATO = spliter[5];
                    ID_BOTON_META_DATO = spliter[8];
                    ROW_ELEMENT_PRENT = "ModalPopupExtender_edition_interface_regitra_meta_dato";
                    service_crea_interface_registro_meta_dato(spliter[1], spliter[0], "multiplex");
                    return true;
                }
                //--Crea interface meta dato documento signado
                if (evento == "firma_doc_selecion_wf") {
                    var spliter = tip_event.split("|");
                    ITEMS_DATOS_SIST_META_ARCHIVO = new Array();
                    ID_IMAGEN_META_DATO = spliter[1];
                    GABINETE_META_DATO = spliter[0];
                    RADICADO_META_DATO = spliter[2];
                    ID_TAREA_META_DATO = spliter[5];
                    ID_BOTON_META_DATO = spliter[8];
                    service_crea_interface_registro_meta_dato(spliter[1], spliter[0], spliter[4]);
                    return true;
                }
                //--Crea interface meta dato documento enlace
                if (evento == "firma_doc_selecion_rad") {
                    var spliter = tip_event.split("|");
                    ITEMS_DATOS_SIST_META_ARCHIVO = new Array();
                    ID_IMAGEN_META_DATO = spliter[1];
                    GABINETE_META_DATO = spliter[0];
                    RADICADO_META_DATO = spliter[2];
                    ID_TAREA_META_DATO = spliter[5];
                    ID_BOTON_META_DATO = spliter[8];
                    service_crea_interface_registro_meta_dato(spliter[1], spliter[0], spliter[4]);
                    return true;
                }
                //-Activa la copia de documentos a expediente 
                if (evento == "CP-DC-EXP") {   
                    Service_activa_copia_documeento_a_expediente(ITEMS_IMAGE_LIST_WF);
                    return true;
                }
                //-Activa copia para la produccion docunental  
                if (evento == "CP-DC-EXP-PRO") {
                    Service_activa_copia_documento_a_produccion_expediente(ITEMS_IMAGE_LIST_WF);
                    return true;
                }
                //-Activa vinculacion documento a expdiente    
                if (evento == "C-DW-VIN") {
                    Service_vincula_documento_a_expediente(ITEMS_IMAGE_LIST_WF);
                    return true;
                }
                //-Auto vinculacion de documentos a expediente
                if (evento == "C-DW-AUTO-VIN") {
                    Service_auto_vincula_documentos_seleccionados_a_expediente(ITEMS_IMAGE_LIST_WF);
                    return true;
                }
                //Crea interface actualiza indice
                if (evento == "C-DW-ACTU-INDICE") {
                    Service_interface_form_control(ITEMS_IMAGE_LIST_WF[0].id_item, "WebServiceDocuarchi.asmx", "Service_crea_interface_indice_workflow", "ModalPopupExtender_edition_actualiza_indice_batch_wf", "div_actualiza_indice_batch_wf", 1,"actualiza_indice_batch_wf",1);
                    return true;
                }
                //Add interface update batch idex link on workflow
                if (evento == "C-DW-ACTU-INDICE-ENLACE") {
                    Service_interface_form_control(ITEMS_IMAGE_LIST_WF[0].id_item, "WebServiceDocuarchi.asmx", "Service_crea_interface_indice_workflow_enlace", "ModalPopupExtender_edition_actualiza_indice_batch_wf_enlace", "div_actualiza_indice_batch_wf_enlace", 1, "actualiza_indice_batch_wf_enlace",1);
                    return true;
                }
                //Elimina documentos relacionados workflow
                if (evento == "C-DW-DEL-IMAGE") {
                    event_multiple_row("", "GridView_list_documento_relacion_wf", "elimina_doc_relacionado_wf");
                    return true;
                    
                }
                //Delete severals images on workflow link
                if (evento == "C-DW-DEL-IMAGE-ENLACE") {
                    event_multiple_row("", "GridView_list_documento_relacion", "elimina_doc_enlace_wf");
                    return true;
                }
                if (evento == "Button_actualizar_nota") {      
                    Service_actualiza_nota_tarea_workflow(document.getElementById("hdnidlista").value, document.getElementById("TextBox_nota").value);
                    return true;
                }
                if (evento == "Button_duardar_nota") {
                    Service_add_nota_tarea_workflow(document.getElementById("TextBox_nota").value);
                    return true;
                }
                if (evento == "delete_note_workflow") {
                    Service_delete_nota_tarea_workflow(tip_event,document.getElementById("TextBox_nota").value);
                    return true;
                }
                if (evento == "C-DW-DETAIL-DOCUMENT") {
                    Service_lista_log_procesing_image_workflow(tip_event, ID_TAREA_WORKFLOW_WF);
                    return true;
                }
                if (evento == "C-DW-DETAIL-COPY-DOCUMENT") {
                    Service_lista_copia_documento_expediente(tip_event, ID_TAREA_WORKFLOW_WF);
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
function inicializa_estado_pendiente() {
    try {
        if (document.getElementById("Hidden_00020_4001").value == 1) {
            document.getElementById("span_pendiente_selec_tarea").innerText = "Enviar a pendientes";
            document.getElementById("span_pendiente_selec_tarea").title = "Enviar la tarea seleccionada a pendiente";
            document.getElementById("Panel_tareas_estado_pendiente").style.display = "block";
        } else {
            document.getElementById("span_pendiente_selec_tarea").innerText = "Cerrar tarea";
            document.getElementById("span_pendiente_selec_tarea").title = "Cerrar la tarea seleccionada";
            document.getElementById("hide_selec_tarea").style.display = "none";
            document.getElementById("Panel_tareas_estado_pendiente").style.display = "none";
        }
    } catch (ex) {
        alert("Funcion inicializa_estado_pendiente " + ex.mensaje);
    }
}
function show_windows_pendiente(evalua_show) {
    try {
        if (document.getElementById("Hidden_00020_4001").value == 1) {
            document.getElementById("div_content_span_pent").classList.remove("col-12");
            document.getElementById("div_content_span_pent").classList.add("col-4");
            document.getElementById("div_content_text_pent").style.display = "block";
            document.getElementById("span_nota_pent").textContent = "Nota de la tarea"
            document.getElementById("h6_envia_documento_pendiente_apro").textContent = "Enviar a pendiente";
            $find("ModalPopupExtender_edition_envia_documento_pendiente_apro").show();
        } else {
            document.getElementById("Button_aceptar_envia_documento_pendiente_apro").click();
        }
       
    } catch (ex) {
        alert("Funcion show_windows_pendiente " + ex.mensaje);
    }
}
function actualiza_titulo_lista_actividades_workflow(titulo) {
    var modalTitle = document.getElementById("ctw-workflow-route-modal-title");

    if (modalTitle && titulo) {
        modalTitle.textContent = titulo;
    }
}

function inicializa_tipo_adjunto_documento(event, element, value_sel) {
    try {
        //Evento activa ventana migra gabinete
        if (value_sel == "M-DWG-WS") {
            Solicita_lista_documentos_recuperar_enlace("GridView_list_documento_relacion", "id_rad");
            if (ars_sele.length == 0) {
                alert("Debe seleccionar los documentos de la lista a exportar");
            } else {
                Service_solicita_lista_gabinetes_permitidos_js("0");
            }
        }
        //Evento activa devolver al usuario anterior
        if (value_sel == "D-TWU-ANT") {
            var title_promp = "La tarea se devolverá al usuario anterior y saldrá de su bandeja. ¿Desea continuar?";
            var r = confirm(title_promp);
            if (r == true) {              
                document.getElementById("Button_tool_devolver_a_usuario").click();
            } else {
                if (event && event.preventDefault) {
                    event.preventDefault();
                }
                return false;
            }
        }
        //Activa la ventana de enviar a pediente tarea 
        if (value_sel == "E-ETP") {
            show_windows_pendiente(1);
        }
        //Auto vincula documento a expediente
        if (value_sel == "A-VIN-DE") {   
                var res = confirm("Desea vincular los  archivos(s) seleccionado(s)");
                if (res == true) {
                    event_element_menu("A-VIN-DE", "auto_vincula_documento_expediente");
                } else {
                    return false;
                }     
        }
        //Guarda documento sello sii ccv
        if (value_sel == "C-DW-SII") {
            event_element_menu("C-DW-SII", "adjunto_doc_sii_sello");
            return true;
        }
        
        //Adjunta documento lista con event interval
        if (value_sel == "C-DW-LISTA") {
            event_element_menu("C-DW-LISTA", "adjunto_doc_visor");
            return true;
        }
        //Adjunta documento enlace 
        if (value_sel == "C-DW-ENL") {
            event_element_menu("C-DW-LISTA", "adjunto_doc_visor");
            return true;
        }
        //Activa subir el documento desde el visor 
        if (value_sel == "C-DW-VIS") {
            event_element_menu("C-DW-VIS", "adjunto_doc_visor");
            return true;
        }
        //Activa subir el documento desde el visor automatico  
        if (value_sel == "C-DW-AUTO") {
            document.getElementById("Hidden_tip_adjunt").value = "wf";
            //document.getElementById("Button_tool_activa_sube_documento_automatico").click();
            document.getElementById("Button_tool_activa_sube_imagen_inscripcion_web_service").click();
        }
        //Ajunta documento
        if (value_sel == "C-DW-RD") {
            event_element_menu("C-DW-RD", "adjunto_doc_visor");
            return true;
        }
         //Ajunta documento desde servicio web
        if (value_sel == "C-DW-WS") {
            document.getElementById("Hidden_tip_adjunt").value = "rad";
            document.getElementById("Button_tool_activa_sube_documento_web_service").click();
        }  
        //Crea interface actualiza indice
        if (value_sel == "C-DW-ACTU-INDICE") {
            ITEMS_IMAGE_LIST_WF = new Array;
            ITEMS_IMAGE_LIST_WF = table_gred_select_check_item("GridView_list_documento_relacion_wf", "chek_selecion_list_wf");
            if (ITEMS_IMAGE_LIST_WF.length == 0) {
                alert("Debe seleccionar los documentos de la lista");
                return true;
            }
            event_element_menu("C-DW-ACTU-INDICE", "");
            return true;
        }
        //add inrerface update several index  link on workflow
        if (value_sel == "C-DW-ACTU-INDICE-ENLACE") {
            ITEMS_IMAGE_LIST_WF = new Array;
            ITEMS_IMAGE_LIST_WF = table_gred_select_check_item("GridView_list_documento_relacion", "chek_selecion_list_rad");
            if (ITEMS_IMAGE_LIST_WF.length == 0) {
                alert("Debe seleccionar los documentos de la lista");
                return true;
            }
            event_element_menu("C-DW-ACTU-INDICE-ENLACE", "");
            return true;
        }
        //Activa eliminar docunmentos relacionados
        if (value_sel == "C-DW-DEL-IMAGE") {
            ITEMS_IMAGE_LIST_WF = new Array;
            ITEMS_IMAGE_LIST_WF = table_gred_select_check_item("GridView_list_documento_relacion_wf", "chek_selecion_list_wf");
            if (ITEMS_IMAGE_LIST_WF.length == 0) {
                alert("Debe seleccionar los documentos de la lista");
                return true;
            }
            event_element_menu("C-DW-DEL-IMAGE", "");
            return true;
        }
        //Activa delete several image of link
        if (value_sel == "C-DW-DEL-IMAGE-ENLACE") {
            ITEMS_IMAGE_LIST_WF = new Array;
            ITEMS_IMAGE_LIST_WF = table_gred_select_check_item("GridView_list_documento_relacion", "chek_selecion_list_rad");
            if (ITEMS_IMAGE_LIST_WF.length == 0) {
                alert("Debe seleccionar los documentos de la lista");
                return true;
            }
            event_element_menu("C-DW-DEL-IMAGE-ENLACE", "");
            return true;
        }
    }
    catch (err) {
        alert(err.message + " Funcion inicializa_tipo_adjunto_documento");
    }
}
//------------ZONA EVENTOS REMPLAZA VERSION DOCUMENTO-------//



//ZONA EVENT GRED
//Activa el evento del listado de documentos relacionados
function prevent(event, element) {
    try {
        delete_alert_boot();
        var fer_id = $(element).attr("id");
        var fer = $(element).attr("idd");
        var tip_event = $(element).attr("tip_event");
        //-----------------------------------------
        //ZONA EVENTOS VENTA ENLACE WORFLOW
        //-----------------------------------------
        //Evento Visualiza el documento de la lista de enlace workflow
        if (tip_event == "vis_doc_selecion_rad") {
            var ref_idd = $(element).attr("idd_rad");
            var ref_id_rad = $(element).attr("id_rad");
            $('#hiden_seleccion_documento_id').val(ref_id_rad);
            $('#hiden_seleccion_documento').val(ref_idd);
            var spliter;
            var text_content = document.getElementById("hiden_seleccion_documento").value;
            if (text_content != "") {
                spliter = text_content.split("|");
                if (spliter.length > 3) {
                    ID_IMAGEN_VIS_WF = spliter[1];
                    GABIENTE_VIS_WF = spliter[0];
                    RADICADO_WORKFLOW = spliter[2];
                    Set_documento_seleccionado(spliter[1], spliter[0]);
                }
            }
            $('#GridView_list_documento_relacion tr[id_rad]').css({ "background": "White", "color": "Black" });
            $('#GridView_list_documento_relacion tr[id_rad=' + ref_id_rad + ']').css({ "background-color": "#e8e8f7", "color": "Black" });
            var value;
            var valor_documento = "";
            valor_documento = busca_campo_rad_seleccion("GridView_list_documento_relacion", ref_id_rad, "DOCUMENTO");
            if (valor_documento != "") {
                value = reemplazarAcentos(valor_documento);
            }
            document.getElementById("titel_visor").innerHTML = value;
            document.getElementById("Button_tool_visualiza_documento").click();
        }
        //Evento firmar documento documento de la lista de enlace workflow
        if (tip_event == "firma_doc_selecion_rad") {
            ars_sele = [];
            var ref_id = $(element).attr("idd_rad") + "|" + element.id;
            if (ref_id != "") {
                var spliter = ref_id.split("|");
                let confi = confirm("¿Desea firmar el documento (" + spliter[4] + ")?");
                if (confi == false) {
                    return true;
                }
                if (spliter.length > 3) {
                    stamp_file_doument_genral(spliter[1], "aspnettable", spliter[0], element.id, spliter[8], "div_error_content_rad", "1", "fa-file-certificate");
                } else {
                    alert("Inconsistencia en el evento, spliter incompleto (" + spliter.length + ")");
                }
            }
          
        }
        //Evento Cambia tipologia documento de la lista de enlace workflow
        if (tip_event == "cambia_doc_selecion_rad") {
            var ref_idd = $(element).attr("idd_rad");
            var ref_id_rad = $(element).attr("id_rad");
            $('#Hidden_selccion_documento_cambia_tipo_split_rad').val(ref_idd);
            $('#Hidden_selccion_documento_cambia_tipo_rad').val(ref_id_rad);
            //$('#hiden_seleccion_documento').val(ref_idd);
            //$('#hiden_seleccion_documento_id').val(ref_id_rad);
            document.getElementById("Button_tool_activa_cambia_tipologia").click();

        }
        //Evento Elimina documento  de la lista de enlace workflow
        if (tip_event == "elim_doc_selecion_rad") {
            ITEMS_IMAGE_LIST_WF = new Array;
            ITEMS_IMAGE_LIST_WF.push({
                id_item: $(element).attr("id_rad")
            });
            event_multiple_row("", "GridView_list_documento_relacion", "elimina_doc_enlace_wf");
           
        }
        //Activa remplazo versión documento asignado
        if (tip_event == "remplaza_ver_doc_selecion_rad") {
            let ref_idd = $(element).attr("idd_rad");
            let spliter = ref_idd.split("|");
            let id_imagen = spliter[1];
            let gabinete = spliter[0];
            let name_class_element_icono_aspnet = spliter[8];
            let DocumentoTilte = spliter[4];
            let option =
                ({
                    IdImagen: id_imagen, Gabinete: gabinete, name_class_element_icono_aspnet: name_class_element_icono_aspnet,
                    DocumentoTilte: DocumentoTilte, OptionRemPlazo: "RAD", ContentError: "error_div_selecion_tarea_rad",
                    NameControlParent: "div_content_general_wf"
                })
            ShowActivaOpcionRemplazo(option);
        }
        //------------------------------------------------
        //ZONA LISTA DOCUMENTOS ASIGNADOS WORKFLOW
        //-----------------------------------------------
        //Evento firmar documento de la lista documentos asignados workflow
        if (tip_event == "firma_doc_selecion_wf") {
            ars_sele = [];
            var ref_id = $(element).attr("idd_wf") + "|" + element.id;  
            if (ref_id != "") {
                var spliter = ref_id.split("|");
                let confi = confirm("¿Desea firmar el documento (" + spliter[4] + ")?");
                if (confi == false) {
                    return true;
                }  
                if (spliter.length > 3) {
                    stamp_file_doument_genral(spliter[1], "aspnettable", spliter[0], element.id, spliter[8], "error_div_selecion_tarea_wf", "1","fa-file-certificate");
                } else {
                    alert("Inconsistencia en el evento, spliter incompleto (" + spliter.length + ")");
                }
            }
        }
        //Evento visualiza documento de la lista documentos asignados workflow
        if (tip_event == "vis_doc_selecion_wf") {
            var ref_idd = $(element).attr("idd_wf");
            var ref_id =  $(element).attr("id_wf");
            $('#hiden_seleccion_documento_wf').val(ref_idd);
            $('#hiden_seleccion_documento_id_wf').val(ref_id);
            var spliter;
            var text_content = document.getElementById("hiden_seleccion_documento_wf").value;
            if (text_content != "") {
                spliter = text_content.split("|");
                if (spliter.length > 3) {
                    ID_IMAGEN_VIS_WF = spliter[1];
                    GABIENTE_VIS_WF = spliter[0];
                    RADICADO_WORKFLOW = spliter[2];
                    Set_documento_seleccionado(spliter[1], spliter[0]);
                }
            }
            $('#GridView_list_documento_relacion_wf tr[idd_wf]').css({ "background": "White", "color": "Black" });
            $('#GridView_list_documento_relacion_wf tr[id_wf=' + ref_id + ']').css({ "background-color": "#e8e8f7", "color": "Black" });
            var value="";
            var valor_documento = "";
            valor_documento = busca_campo_rad_seleccion("GridView_list_documento_relacion_wf", ref_id, "DOCUMENTO");
            if (valor_documento != "") {
                value = reemplazarAcentos(valor_documento);
            }
            document.getElementById("Button_selecion_treview_documento").click();
        }

        //Evento cambia tipologia documento de la lista documentos asignados workflow
        if (tip_event == "cambia_doc_selecion_wf") {
            var ref_idd = $(element).attr("idd_wf");
            var ref_id = $(element).attr("id_wf");
            $('#Hidden_selccion_documento_cambia_tipo_split_wf').val(ref_idd);
            $('#Hidden_selccion_documento_cambia_tipo_wf').val(ref_id);
            document.getElementById("Button_clasficar_documento").click();

        }
        if (tip_event == "elim_doc_selecion_wf") {
            ITEMS_IMAGE_LIST_WF = new Array;
            ITEMS_IMAGE_LIST_WF.push({
                id_item: $(element).attr("id_wf")
            });
            event_multiple_row("", "GridView_list_documento_relacion_wf", "elimina_doc_relacionado_wf");
        }
        //Versionamiento de documento
        if (tip_event == "lista_ver_doc_selecion_rad") {
            let ref_idd = $(element).attr("idd_rad");
            let spliter = ref_idd.split("|");
            let id_imagen = spliter[1];
            let gabinete = spliter[0];
            NAME_GABINETE_VERSION = gabinete;
            ID_IMAGEN_VERSION = id_imagen;
            let name_class_element_icono_aspnet = spliter[8];
            let DocumentoTilte = spliter[4];
            let option = ({
                IdImagen: id_imagen, Gabinete: gabinete, TipoModulo: 2,
                ContentError: "div_error_content_rad", name_class_element_icono_aspnet: name_class_element_icono_aspnet,
                DocumentoTilte: DocumentoTilte, NameModulo: "WORKFLOW", NameControlParent: "div_content_general_wf"
            });
            ShowListVersionDocumento(option);

        }
        //Versionamiento de documento
        if (tip_event == "lista_ver_doc_selecion_wf") {
            let ref_idd = $(element).attr("idd_wf");
            let spliter = ref_idd.split("|");
            let id_imagen = spliter[1];
            let gabinete = spliter[0];
            NAME_GABINETE_VERSION = gabinete;
            ID_IMAGEN_VERSION = id_imagen;
            let name_class_element_icono_aspnet = spliter[8];
            let DocumentoTilte = spliter[4];
            let option = ({
                IdImagen: id_imagen, Gabinete: gabinete, TipoModulo: 2,
                ContentError: "div_error_content_wf", name_class_element_icono_aspnet: name_class_element_icono_aspnet,
                DocumentoTilte: DocumentoTilte, NameModulo: "WORKFLOW", NameControlParent: "div_content_general_wf"
            });
            ShowListVersionDocumento(option);
        }
        //Activa remplazo versión documento asignado
        if (tip_event == "remplaza_ver_doc_selecion_wf") {
            let ref_idd = $(element).attr("idd_wf");
            let spliter = ref_idd.split("|");
            let id_imagen = spliter[1];
            let gabinete = spliter[0];
            let name_class_element_icono_aspnet = spliter[8];
            let DocumentoTilte = spliter[4];
            let option = 
            ({
                IdImagen: id_imagen, Gabinete: gabinete, name_class_element_icono_aspnet: name_class_element_icono_aspnet,
                DocumentoTilte: DocumentoTilte, OptionRemPlazo: "WF", ContentError: "error_div_selecion_tarea_wf",
                NameControlParent: "div_content_general_wf"
            })
            ShowActivaOpcionRemplazo(option);
        }
       
        event.preventDefault();
        element.focus();
    }
    catch (err) {
        alert(err.message + " Funcion prevent");
    }
}
//---------------Zona integración para adjuntar documentos----------////
const ActivaAdjuntarDocumentoWorkflowEnlace = async () => {
    try {
        let resp1 = await asmxClient
            .use("Workflow")
            .call("ServiceSolicitaEstructuraTramiteEnlaceWorkflowLoadFile", { Parameter: 0 });
        if (resp1.error) {
            return resp1.message;
        }
        let Data = resp1.data[0];
        resp1 = await asmxClient
            .use("ConfigDigitaliacion")
            .call("ServiceSolicitaEstructuraConfiguracion", { IdTipoTramite: Data.CDParameterFileLoadworkflow[0].IdTipoTramite });
        if (resp1.error) {
            return resp1.message;
        }
        Data = resp1.data[0];
        let _OPtionFileLoad = ({
            NameLoadProceso: "WORKFLOWENLACE",
            NameContenedorError: "error_content_adjunta_documeto_load_documento_006",
            funcion_name: "InsertRowWorkflowSeleccion", evento_adjunta: "WORKFLOWENLACE",
            IdRespuestaIdExpediente: 0,
            NameContendorLoadDocumento: "modal_content_admon_documentos", ModalWidth: 75, CargaTipologia: 1,
            CargaFecha: 1, CargaPreview: 1, multi_select: "multiple",
            element_parent: "modal_adjunta_documeto_load_documento_006", TipoFormulario: 1,
            name_serivce_list: "service_source_list_item_control_general_documento_radicado",
            name_class_serivce_list: "WebServiceRadicacion.asmx",
            element_html_table: "GridView_list_documento_relacion", element_html_lab_conteo: "Label_documentos", apost_html_lab_conteo: "Documentos",
            setioption_obliga_tipologia: Data.Obliga_Lista_Chequeo, TipoApost:"rad"
        });
        let result = await IniLoadPerson(_OPtionFileLoad);
        if (result != "YES") {
            return result;
        }
        document.getElementById("Hidden_tip_adjunt").value = "rad";
        return "YES"
    } catch (ex) {
        return "Inconsistencia funcion ActivaAdjuntarDocumentoRadicacion " + ex.mensaje;
    }
}
const ActivaAdjuntarDocumentoWorkflow = async () => {
    try {
        let resp1 = await asmxClient
            .use("Workflow")
            .call("ServiceSolicitaEstructuraTramiteAsignadWorkflowLoadFile", { Parameter: 0 });
        if (resp1.error) {
            return resp1.message;
        }
        let Data = resp1.data[0];
        resp1 = await asmxClient
            .use("ConfigDigitaliacion")
            .call("ServiceSolicitaEstructuraConfiguracion", { IdTipoTramite: Data.CDParameterFileLoadworkflow[0].IdTipoTramite });
        if (resp1.error) {
            return resp1.message;
        }
        Data = resp1.data[0];
        let _OPtionFileLoad = ({
            NameLoadProceso: "WORKFLOWSELECCION",
            NameContenedorError: "error_content_adjunta_documeto_load_documento_006",
            funcion_name: "InsertRowWorkflowSeleccion", evento_adjunta: "WORKFLOWSELECCION",
            IdRespuestaIdExpediente: 0,
            NameContendorLoadDocumento: "content_selecion_tarea", ModalWidth: 75, CargaTipologia: 1,
            CargaFecha: 1, CargaPreview: 1, multi_select: "multiple",
            element_parent: "modal_adjunta_documeto_load_documento_006", TipoFormulario: 1,
            name_serivce_list: "service_source_list_item_control_general_documento_radicado",
            name_class_serivce_list: "WebServiceRadicacion.asmx",
            element_html_table: "GridView_list_documento_relacion_wf", element_html_lab_conteo: "Label_docu_relacionado_wf", apost_html_lab_conteo: "Documentos",
            setioption_obliga_tipologia: Data.Obliga_Lista_Chequeo, TipoApost: "wf"
        });
        let result = await IniLoadPerson(_OPtionFileLoad);
        if (result != "YES") {
            return result;
        }
        document.getElementById("Hidden_tip_adjunt").value = "wf";
        return "YES"
    } catch (ex) {
        return "Inconsistencia funcion ActivaAdjuntarDocumentoRadicacion " + ex.mensaje;
    }
}
let IListCservicioIntegracionAdjuntaDocumento = new Array();  //Lista estructura de servicio interación documentos adjuntos
const ActivaServicioIntegracionAdjuntaDocumentosSistemasExternos = async () => {
    try {
        let result = "";
        IListCservicioIntegracionAdjuntaDocumento = new Array();
        result = await ServiceRESTActivaAdjuntaDocumentoServicioIntegracion(0);
        if (result != "YES") {
            alert_bot(result, 'warning', "div_error_content_wf");
        }
        if (IListCservicioIntegracionAdjuntaDocumento[0].NameService == "") {
            alert_bot("No se detecta ningún servicio de integración habilitado para adjuntar documentos externos. Por favor, contacte al área de soporte para validar la configuración del servicio.", 'warning', "div_error_content_wf");
            return "YES";
        }
        //***Lista Inscripciones SII--------///
        if (IListCservicioIntegracionAdjuntaDocumento[0].NameService == "INTEGRACIONSII") {
            result = await ServiceRESTServiceSolicitaListaConstanciaInscripcionSII("", "tabl_lista_sii_constancias_inscripcion", "");
            if (result != "YES") {
                alert_bot(result, 'warning', "div_error_content_wf");
                return "YES";
            }
            return "YES";
        }
        return "YES";
    } catch (ex) {
        return ex.mensaje;
    }
}
//---------------ANEXOS SISTEMA SII-------------------------//
let CIanexosSII = new Array();           //Guarda las estructura de los datos de alamacenamiento de los anexos SII
let CDParameterAnexosSII = new Array();  //Guarda los parametros generales de almacenamiento (CODIGO BARRAS, RECIBO Y GABINETE)
const ActivaListaAnexosIntegracionSII = async () => {
    try {
        let result = "";
        IListCservicioIntegracionAdjuntaDocumento = new Array();
        result = await ServiceRESTActivaAdjuntaDocumentoServicioIntegracionEnlace(0);
        if (result != "YES") {
            return result;
        }
        if (IListCservicioIntegracionAdjuntaDocumento[0].NameService == "") {
            alert_bot("No se detecta ningún servicio de integración habilitado para adjuntar documentos externos. Por favor, contacte al área de soporte para validar la configuración del servicio.", 'warning', "div_error_content_rad");
            return "YES";
        }
        let IdTramite = IListCservicioIntegracionAdjuntaDocumento[0].CTipoDocEntrante[0].id_Tipo_Doc_Entrante;
        /*Solicita la configuración de digitialziación de un tramite*/
        result = await ServiceRESTsolicitaEstructuraConfiguracion(IdTramite);
        if (result != "YES") {
            alert_bot(result, 'warning', "div_error_content_rad");
            return true;
        }
        result = await ServiceRESTsolicitaArchivosAnexosrelacionadosRadicadoSII("", "tabl_lista_sii_anexos_recibo","content_tabl_lista_sii_anexos_recibo");
        return result;
    } catch (ex) {
    return ex.mensaje;
}
}

const ActivaVentanaGuardarAnexosSII = async (element) => {
    try {
        posicion_update_pogres('progres_bar');
        let result = "";
        let ResultadoFiltro = [];
        let observaciones = element.observaciones;
        ResultadoFiltro = FiltroJonsonStrinstringify(element.observaciones);
        if (ResultadoFiltro[0].Error != "YES") {
            alert_bot(ResultadoFiltro[0].Error, 'warning', "error_content_sii_anexos_recibo");
            return true;
        }
        observaciones = ResultadoFiltro[0].ContentJonson;
        let nombre = element.nombre;
        ResultadoFiltro = FiltroJonsonStrinstringify(element.nombre);
        if (ResultadoFiltro[0].Error != "YES") {
            alert_bot(ResultadoFiltro[0].Error, 'warning', "error_content_sii_anexos_recibo");
            return true;
        }
        nombre = ResultadoFiltro[0].ContentJonson;
        CIanexosSII = new Array();
        CIanexosSII.push({
            idanexo: element.idanexo, formato: element.formato, tipo: element.tipo, url: element.url,
            observaciones: observaciones, url: element.url, tiposirep: element.tiposirep, tipodigitalizacion: element.tipodigitalizacion,
            identificador: element.identificador, nombre: nombre, matricula: element.matricula, proponent: element.proponent,
            fechadocumento: element.fechadocumento, origen: element.origen, identificacion: element.identificacion
        });
        let IdTramite = IListCservicioIntegracionAdjuntaDocumento[0].CTipoDocEntrante[0].id_Tipo_Doc_Entrante;
        result = await ServiceRESTsolicitaListaTiposDocumentalesTramiteListaAdjuntaAnexo(IdTramite, "");
        if (result != "YES") {
            alert_bot(result, 'warning', "error_content_sii_anexos_recibo");
            return true;
        }
        return true;
    } catch (ex) {
        alert_bot("Inconsistencia general función ActivaVentanaGuardarAnexosSII " + ex.mensaje, 'warning', "error_content_sii_anexos_recibo");
        return true;
    } finally {
        progres_hiden('progres_bar');
    }
}
const GuardaDocumentoAnexoSII = async () => {
    try {
        let result = "";
        let IdTramite = IListCservicioIntegracionAdjuntaDocumento[0].CTipoDocEntrante[0].id_Tipo_Doc_Entrante;
        let SelectOption = document.getElementById("option_lista_tipologia_anexo");
        let IdTipologiaDocumental = 0;
        let DescripcionTipos = "";
        if (SelectOption.options.length != 0) {
            IdTipologiaDocumental = SelectOption.options[SelectOption.selectedIndex].value;
            DescripcionTipos = SelectOption.options[SelectOption.selectedIndex].text;
        }
        CDParameterAnexosSII[0].IdTipoTaramite = IdTramite;
        CDParameterAnexosSII[0].IdTipoChekLista = IdTipologiaDocumental;
        CDParameterAnexosSII[0].DescripcionTipo = DescripcionTipos;
        CDParameterAnexosSII[0].MultiAnexos = 0;
        /*Valida la obligatoriedad de aplicar la tipologia para guardar el anexo*/
        if (CAinterfaceConfigDigitaliza.Obliga_Lista_Chequeo == 1 && (IdTipologiaDocumental == -1 || IdTipologiaDocumental == 0)) {
            return "Por favor, indique el tipo documental correspondiente al anexo.​";
        }
        result = await ServiceRESTGuardaDocumentoAnexoSII(CDParameterAnexosSII, CIanexosSII);
        if (result == "YES") {
            $("#modal_guarda_archivos_anexos_sii").modal("hide");
        }
        return result;
    } catch (ex) {
        return "Inconsistencia funcion GuardaDocumentoAnexoSII " + ex.mensaje;
    }
}

//---------------CONSTANCIAS DE INSCRIPCION SII-------------------------//
let CIncripcionSII = new Array();   //Guarda las constancias de inscripcion
let MULTIPLE_SII=0;
//-----------Activa guardar multiplex constancias de inscripción y muestra la ventana con la tipologia de las constancias------///
const ActivaGuardarMultiplexConstanciasInscription = async () => {
    try {
        let result = await ServiceRESTsolicitaRegistrosSellosSII(0);
        if (result != "YES") {
            return result;
        }
        //--------------------Solicita cache inscripción--------------------------------/////
        result = await ServiceRESTsolicitaEstructuraCacheInscripcionRadicado(CIncripcionSII[0].RADICADO_SII)
        if (result != "YES") {
            return result;
        }
        let IdTramite = IListCservicioIntegracionAdjuntaDocumento[0].CTipoDocEntrante[0].id_Tipo_Doc_Entrante;
        MULTIPLE_SII = 1;
        result = await ServiceRESTsolicitaListaTiposDocumentalesTramiteListaAdjunta(IdTramite, ""); 
        return result;
    } catch (ex) {
        return "Inconsistencia general función activa guardar multiplex constancias de incripción " + ex.mensaje;
    }
}
//-----------Activa guardar una constancia de inscripción especifica y muestra la ventana con la tipologia de las constancias------///
const ActivaGuardarContasnciaInscripcion = async (element) => {
    try {
        posicion_update_pogres('progres_bar');
        let ResultadoFiltro = [];
        let Noticia = element.noticia;
        ResultadoFiltro = FiltroJonsonStrinstringify(element.noticia);
        if (ResultadoFiltro[0].Error != "YES") {
            alert_bot(ResultadoFiltro[0].Error, 'warning', "error_content_sii_constancias_inscripcion");
            return true;
        }
        Noticia = ResultadoFiltro[0].ContentJonson;
        let RsocialSII = element.nombre;
        ResultadoFiltro = FiltroJonsonStrinstringify(element.nombre);
        if (ResultadoFiltro[0].Error != "YES") {
            alert_bot(ResultadoFiltro[0].Error, 'warning', "error_content_sii_constancias_inscripcion");
            return true;
        }
        RsocialSII = ResultadoFiltro[0].ContentJonson;
        CIncripcionSII = new Array();
        CIncripcionSII.push({
            LIBRO_SII: element.libro, REGISTRO_SII: element.registro, FECHA_SII: element.fecha, MATRICULA_SII: element.matricula,
            PROPONENTE_SII: element.proponente, NIT_SII: element.identificacion, RSOCIAL_SII: RsocialSII, ACTO_SII: element.acto,
            NOTICIA_SII: Noticia, RADICADO_SII: element.Recibo, COD_BARRA_SII: element.CodigoBarras, URL_SII: element.url,
            NACTO_SII: element.nacto
        });
        MULTIPLE_SII = 0;
        let result = "";
        //--------------------Solicita cache inscripción--------------------------------/////
        result = await ServiceRESTsolicitaEstructuraCacheInscripcionRadicado(CIncripcionSII[0].RADICADO_SII)
        if (result != "YES") {
            alert_bot(result, 'warning', "error_content_sii_constancias_inscripcion");
            return true;
        }
        if (CacheInscripcion[0].CahcheInscripcion[0] == null && MULTIPLE_SII == 0) {
            result = "Se recomienda utilizar la opción ‘Guardar todas las inscripciones’ antes de guardar un registro de inscripción individual.";
            alert_bot(result, 'warning', "error_content_sii_constancias_inscripcion");
            return true;
        }
        let IdTramite = IListCservicioIntegracionAdjuntaDocumento[0].CTipoDocEntrante[0].id_Tipo_Doc_Entrante;
        result = await ServiceRESTsolicitaListaTiposDocumentalesTramiteListaAdjunta(IdTramite, "");
        if (result != "YES") {
            alert_bot(result, 'warning', "error_content_sii_constancias_inscripcion");
            return true;
        }
        return true;
    } catch (ex) {
        alert_bot("Inconsistencia general función activa guardar constancias de incripción " + ex.mensaje, 'warning', "error_content_sii_constancias_inscripcion");
        return true;
    } finally {
         progres_hiden('progres_bar');
    }
}

let CAinterfaceConfigDigitaliza = new Array();
//----Inicializa las tipologias default de sellos de inscripción//
let DafaultTramiteSelloSII  = new Array();
DafaultTramiteSelloSII.push({ Name: "Constancia De Inscripción" }, { Name: "Constancia De Inscripcion" }, { Name: "Sello De Inscripcion" }, { Name: "Sello De Inscripción" });
/**
 * Function que adjunta sellos de incricpion y vincula documentos y crea expediente
 * Integración con el siistema SII
 * @param {any} CTipoDocEntrante
 */
let CacheInscripcion = new Array();
const GuardarConstanciaIncripcionSII = async (CTipoDocEntrante) => {
    try {
        let result_;
        let IdTramite = IListCservicioIntegracionAdjuntaDocumento[0].CTipoDocEntrante[0].id_Tipo_Doc_Entrante;
        /*Solicita la configuración de digitialziación de un tramite*/
        result_ = await ServiceRESTsolicitaEstructuraConfiguracion(IdTramite);
        if (result_ != "YES") {
            alert_bot(result_, 'warning', "error_tipo_tramite_vinculacion");
            return "YES";
        }
        let IdTipologiaDocumental = 0;
        var SelectOption = document.getElementById("option_lista_tipologia");
        if (SelectOption.options.length != 0) {
            IdTipologiaDocumental = SelectOption.options[SelectOption.selectedIndex].value;
        }
        /*Valida la obligatoriedad de aplicar la tiologia*/
        if (CAinterfaceConfigDigitaliza.Obliga_Lista_Chequeo == 1 && (IdTipologiaDocumental == -1 || IdTipologiaDocumental == 0)) {
            alert_bot("Por favor, indique el tipo documental correspondiente al sello de inscripción.​", 'warning', "error_tipo_tramite_vinculacion");
            return "YES";
        }
        //------------------Valida si crea expediente y vincula documentos a expediente para tramites marcados como de nuevos matriculados------------///
        if (IListCservicioIntegracionAdjuntaDocumento[0].CTipoDocEntrante[0].util_Estado_Crea_ExpedienteSII == 1) { 
            let OptionItem = Object;
            let OPtionProgresBar = ({
                name_service: "ServiceRegistraExpeidenteSIIVincula",
                OptionItemSelect: OptionItem, NameControlPadreProgres: "modal_guarda_archivos_inscripcion_sii", NameProceso: "Vinculando documentos"
            });
            let OptionParameterExpediente = ({
                OptionExpediente: CIncripcionSII, OptionProgres: OPtionProgresBar, NameService: "ServiceRegistraExpeidenteSIIVincula",
                IdTramite: IdTramite
            });
            result_ = await JSExpdiente(OptionParameterExpediente);
            if (result_.ErrorResuladoExpediente != "YES") {
                alert_bot(result_.ErrorResuladoExpediente, 'warning', "error_tipo_tramite_vinculacion");
                return result_.ErrorResuladoExpediente;
            } 
        }

        //---------------------Vicula documentos a expedientes con un registro previo de expediente -------------///
        if (IListCservicioIntegracionAdjuntaDocumento[0].CTipoDocEntrante[0].util_Estado_Crea_ExpedienteSII != 1) {
            let OptionItem = Object;
            let OPtionProgresBar = ({
                name_service: "ServiceVinculaDocumentoSII",
                OptionItemSelect: OptionItem, NameControlPadreProgres: "modal_guarda_archivos_inscripcion_sii", NameProceso: "Vinculando documentos"
            });
            let OptionParameterExpediente = ({
                OptionExpediente: CIncripcionSII, OptionProgres: OPtionProgresBar, NameService: "ServiceVinculaDocumentoSII",
                IdTramite: IdTramite
            });
            result_ = await JSExpdiente(OptionParameterExpediente);
            if (result_.ErrorResuladoExpediente != "YES") {
                alert_bot(result_.ErrorResuladoExpediente, 'warning', "error_tipo_tramite_vinculacion");
                return result_.ErrorResuladoExpediente;
            }
        }
        //---------------------Actualizar indices de Doumentos---------------------------////
        result_ = await ServiceRESTactualizaIndiceDocumentosSII(CIncripcionSII, IdTramite, CIncripcionSII[0].RADICADO_SII);
        if (result_ != "YES") {
            alert_bot(result_, 'warning', "error_tipo_tramite_vinculacion");
            return result_;
        }
        //---------------------Guarda constancias de inscripción-------------------------////
        if (CIncripcionSII.length == 1) {
            result_ = await ServiceRESTGuardaConstanciaInscripcionSII(CIncripcionSII[0], IdTipologiaDocumental, IdTramite)
            if (result_ != "YES") {
                alert_bot(result_, 'warning', "error_tipo_tramite_vinculacion");
                return result_;
            } 
        } else {
            /*** Dependendencia del archivo JSPressBar */
            let _OPtionProgresBar = ({
                name_service: "ServiceGuardaSelloSII",
                OptionItemSelect: CIncripcionSII, NameControlPadreProgres: "modal_guarda_archivos_inscripcion_sii", NameProceso: "Guardando la inscripción",
                IdtipoDocumentalTrd: IdTipologiaDocumental, IdTramite: IdTramite
            });
            result_ = await JSProgresBarBoot(_OPtionProgresBar);
            if (result_ != "YES") {
                alert_bot(result_, 'warning', "error_tipo_tramite_vinculacion");
                return result_;
            } 
        }
        //--------------------Registra cache de inscripción----------////
        if (CacheInscripcion[0].CahcheInscripcion[0] == null) {
            result_ = await ServiceRESTregistraCacheInscripcionRadicadoSII(CIncripcionSII, IdTramite);
            if (result_ != "YES") {
                alert_bot(result_, 'warning', "error_tipo_tramite_vinculacion");
                return result_;
            }    
        }
        $("#modal_guarda_archivos_inscripcion_sii").modal("hide");
        if (MULTIPLE_SII == 1) {
            $("#modal_sii_constancias_inscripcion").modal("hide");
        }
      
        return "YES";
    } catch (ex) {
        alert_bot(ex.mensaje, 'warning', "error_tipo_tramite_vinculacion");
        return ex.mensaje;
    }
}
//ZONA EXPORTAR GABINETE WORKFLOW ENLACE TAREA
//----Asigna documentos relacionados a las lista
function Solicita_lista_documentos_recuperar_enlace(table, clave_sel) {
    try {
        ars_sele = [];
        $('#' + table + ' tr[' + clave_sel + ']').each(function () {
            let nod_value = $(this).attr("idd_rad");
            let chek_item = $(this).find("[type=checkbox]");
            if (chek_item[0].checked == true) {
                let split_value = nod_value.split("|");
                ars_sele.push(split_value[0] + "|" + split_value[1]);
            }      
        });

    }
    catch (err) {
        alert(err.message + " funcion asigna_doc_seleccionados_multi_firma " + err.message);
    }
}
//--Sevice lista gabinetes permitidos
function Service_solicita_lista_gabinetes_permitidos_js(id_) {
    try {
        $.ajax('../webservice/WebServiceWorkflow.asmx/Service_solicita_lista_gabinetes_permitidos', {
            data: "{'id':" + "'" + id_ + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_sistema !== "YES") {
                    alert(data.d[0].error_sistema);
                } else {
                    ITEMS_DATOS_DROW = new Array();
                    $.each(data.d[0].item_sistema, function (k, v) {
                        ITEMS_DATOS_DROW.push(v);
                    });
                    if (document.getElementById('DropDownList_exporta_gabinete_workflow')) {
                        var element_drow = document.getElementById('DropDownList_exporta_gabinete_workflow');
                        $("#DropDownList_exporta_gabinete_workflow").empty();
                        for (var i = 0; i < ITEMS_DATOS_DROW.length; i++) {
                            element_drow[i] = new Option(ITEMS_DATOS_DROW[i].text, ITEMS_DATOS_DROW[i].value);
                        }
                    }
                    $find('ModalPopupExtender_edition_exporta_gabinete_workflow').show();
                }
            }, error: function (xception, textStatus, errorThrown) {

                if (xception.status === 0) {

                    alert('Not connect: Verify Network.');

                } else if (xception.status == 404) {

                    alert('Requested page not found [404]');

                } else if (xception.status == 500) {

                    alert('Internal Server Error [500].' + xception.responseText);

                } else if (textStatus === 'parsererror') {

                    alert('Requested JSON parse failed.');

                } else if (textStatus === 'timeout') {

                    alert('Time out error.');

                } else if (textStatus === 'abort') {

                    alert('Ajax request aborted.');

                } else {

                    alert('Uncaught Error: ' + xception.responseText);

                }
            }
        });
    }
    catch (ex) {

        alert('Service_solicita_lista_gabinetes_permitidos_js ' + ex.message);
    }
}
//Service exporta documentos a gabinete
function Service_exporta_documento_gabinete_workflow_java(nombre_gabinete_, nombre_gabinete_destino_, id_imagen_) {
    try {
        $.ajax('../webservice/WebServiceWorkflow.asmx/Service_exporta_dcoumento_gabinete_workflow', {
            data: "{'nombre_gabinete':" + "'" + nombre_gabinete_ + "','nombre_gabinete_destino':'" + nombre_gabinete_destino_ + "','id_imagen':'" + id_imagen_ + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_gestion !== "YES") {
                    myStopFunction_Event(data.d[0].error_gestion);         
                }
                if (data.d[0].result_service_control !== "") {          
                    myStopFunction_cancel(data.d[0].result_service_control);
                }
                ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
            }, error: function (xception, textStatus, errorThrown) {
                ESTADO_EVENT_GENERAL = "out";
                if (xception.status === 0) {
                    myStopFunction_Event('Not connect: Verify Network.');
                } else if (xception.status == 404) {
                    myStopFunction_Event('Requested page not found [404]');
                } else if (xception.status == 500) {
                    myStopFunction_Event('Internal Server Error [500].' + xception.responseText);
                } else if (textStatus === 'parsererror') {
                    myStopFunction_Event('Requested JSON parse failed.');
                } else if (textStatus === 'timeout') {
                    myStopFunction_Event('Time out error.');
                   

                } else if (textStatus === 'abort') {
                    myStopFunction_Event('Ajax request aborted.');
                   

                } else {
                    myStopFunction_Event('Uncaught Error: ' + xception.responseText);
                    

                }
            }
        });
    }
    catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        myStopFunction_Event('Service_solicita_lista_gabinetes_permitidos_js ' + ex.message);
    }
}
//TERMINA EXPORTAR GABINETE WORKFLOW ENLACE TAREA
//---------------////--------------------------------
//ZONA COPIA DOCUMENTOS A EXPEDIENTE
//---------------////--------------------------------
function Service_activa_copia_documeento_a_expediente(para_meter_ca) {
    try {   
        var serialice = JSON.stringify(para_meter_ca);
        $.ajax('../webservice/WebServiceWorkflow.asmx/Service_activa_copia_documeento_a_expediente', {
            data: "{" + "'parameter':'" + serialice + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0] !== "YES") {
                    alert(data.d[0]);
                    ESTADO_EVENT_GENERAL = "out";
                } else {  
                    let element_iframe = document.getElementById("Iframe_copiar_estructura_");
                    element_iframe.setAttribute("src", "../Gestion/WebFormGaGestionExpediente.aspx");
                    $find('ModalPopupExtender_copiar_estructura').show();
                    auto_zise_popup_copiar_estructura();
                    document.getElementById("title_copiar_estructura").innerHTML = "Copiar a expediente";
                    ESTADO_EVENT_GENERAL = "out";
                }
            }, error: function (xception, textStatus, errorThrown) {
                ESTADO_EVENT_GENERAL = "out";
                if (xception.status === 0) {
                    myStopFunction_Event('Not connect: Verify Network.');


                } else if (xception.status == 404) {
                    myStopFunction_Event('Requested page not found [404]');


                } else if (xception.status == 500) {
                    myStopFunction_Event('Internal Server Error [500].' + xception.responseText);

                } else if (textStatus === 'parsererror') {
                    myStopFunction_Event('Requested JSON parse failed.');

                } else if (textStatus === 'timeout') {
                    myStopFunction_Event('Time out error.');

                } else if (textStatus === 'abort') {
                    myStopFunction_Event('Ajax request aborted.');

                } else {
                    myStopFunction_Event('Uncaught Error: ' + xception.responseText);

                }
            }
        });
       
    } catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        alert(ex.message + " funcion Service_activa_copia_documeento_a_expediente");
    }
}
//---------------////--------------------------------
//ZONA COPIA DOCUMENTOS A PRODUCCION EXPEDIENTE
//---------------////--------------------------------
function Service_vincula_documento_a_expediente(para_meter_ca) {
    try {
        var serialice = JSON.stringify(para_meter_ca);
        $.ajax('../webservice/WebServiceWorkflow.asmx/Service_activa_vincula_documento_a_expediente', {
            data: "{" + "'parameter':'" + serialice + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0] !== "YES") {
                    alert(data.d[0]);
                    ESTADO_EVENT_GENERAL = "out";
                } else {
                    let element_iframe = document.getElementById("Iframe_copiar_estructura_");
                    element_iframe.setAttribute("src", "../Gestion/WebFormGaGestionExpediente.aspx");
                    $find('ModalPopupExtender_copiar_estructura').show();
                    auto_zise_popup_copiar_estructura();
                    document.getElementById("title_copiar_estructura").innerHTML = "Vincula documento a expediente";
                    ESTADO_EVENT_GENERAL = "out";
                }
            }, error: function (xception, textStatus, errorThrown) {
                ESTADO_EVENT_GENERAL = "out";
                if (xception.status === 0) {
                    myStopFunction_Event('Not connect: Verify Network.');


                } else if (xception.status == 404) {
                    myStopFunction_Event('Requested page not found [404]');


                } else if (xception.status == 500) {
                    myStopFunction_Event('Internal Server Error [500].' + xception.responseText);

                } else if (textStatus === 'parsererror') {
                    myStopFunction_Event('Requested JSON parse failed.');

                } else if (textStatus === 'timeout') {
                    myStopFunction_Event('Time out error.');

                } else if (textStatus === 'abort') {
                    myStopFunction_Event('Ajax request aborted.');

                } else {
                    myStopFunction_Event('Uncaught Error: ' + xception.responseText);

                }
            }
        });

    } catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        alert(ex.message + " funcion Service_vincula_documento_a_expediente");
    }
}
function Service_activa_copia_documento_a_produccion_expediente(para_meter_ca) {
    try {
        var serialice = JSON.stringify(para_meter_ca);
        $.ajax('../webservice/WebServiceWorkflow.asmx/Service_activa_copia_documento_a_produccion_expediente', {
            data: "{" + "'parameter':'" + serialice + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0] !== "YES") {
                    alert(data.d[0]);
                    ESTADO_EVENT_GENERAL = "out";
                } else {
                    let element_iframe = document.getElementById("Iframe_copiar_estructura_");
                    element_iframe.setAttribute("src", "../Gestion/WebFormProducionDocumental.aspx");
                    $find('ModalPopupExtender_copiar_estructura').show();
                    auto_zise_popup_copiar_estructura();
                    document.getElementById("title_copiar_estructura").innerHTML = "Copiar a produccion documental";
                    ESTADO_EVENT_GENERAL = "out";
                }
            }, error: function (xception, textStatus, errorThrown) {
                ESTADO_EVENT_GENERAL = "out";
                if (xception.status === 0) {
                    myStopFunction_Event('Not connect: Verify Network.');


                } else if (xception.status == 404) {
                    myStopFunction_Event('Requested page not found [404]');


                } else if (xception.status == 500) {
                    myStopFunction_Event('Internal Server Error [500].' + xception.responseText);

                } else if (textStatus === 'parsererror') {
                    myStopFunction_Event('Requested JSON parse failed.');

                } else if (textStatus === 'timeout') {
                    myStopFunction_Event('Time out error.');

                } else if (textStatus === 'abort') {
                    myStopFunction_Event('Ajax request aborted.');

                } else {
                    myStopFunction_Event('Uncaught Error: ' + xception.responseText);

                }
            }
        });

    } catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        alert(ex.message + " funcion Service_activa_copia_documento_a_produccion_expediente");
    }
}

function menu_context_treview(value) {
    try {
        document.getElementById("Hidden_menucab").value = value;
        document.getElementById("Button_tool_menucab").click();
    }
    catch (err) {
        alert(err.message + " Funcion menu_context_treview");
    }
}
function prevent_tool_active_adjunta(event, element, value) {
    try {
        event.preventDefault();
        document.getElementById("ImageButtonadjunta").click();
        element.focus();
    }
    catch (err) {
        alert(err.message + " Funcion revent_tool_active_adjunta");
    }
}
function selection_event() {
    var valor;
    if ($('#Hidden1') != null) {
        valor = $('#Hidden1').val();
    }
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
    if (valor == "ANOTACION") {
        $('#PanelLibre').css("height", (espacio_iframe - 40) + "px");
        $('#Div9').css("height", (espacio_iframe - 40) + "px");
        $('#Iframelibre_').css("height", (espacio_iframe - 41) + "px");
        $('#Iframelibre_').css("width", "100%");
        $('#PanelLibre').css("width", "50%");
        $('#Labeladver').html("Lista de Anotaciones ");

    }
}
const prevent_list_archivo = async(event, element) => {
    try {
        delete_alert_boot();
        var fer = $(element).attr("idd");
        var tip_event = $(element).attr("tip_event");
        if (tip_event == "des_car_archivo") {
            var ur = $(element).attr("ur");
            window.open(ur, '_blank');
        }
        if (tip_event == "guar_dar_archivo") {
            document.getElementById("Button_tool_activa_sube_documento_enlace_integra_sii").click();    
            document.getElementById("Hidden_url").value = $(element).attr("ur");
            document.getElementById("Hidden_extension").value = $(element).attr("ext"); 
        }
        if (tip_event == "guar_dar_archivo_inscripcion") {  
            let result = await ActivaGuardarContasnciaInscripcion(element);
            if (result != "YES") {
                alert_bot(result, 'warning', "div_error_content_adjunta_sello_sii");
                return true;
            }
        }
        event.preventDefault();
        element.focus();
    }
    catch (err) {
        alert(err.message + " Funcion prevent_list_archivo");
    }
}
function acti_busq_general_lista(e, sender) {
    try {
        
        tecla = (document.all) ? e.keyCode : e.which;
       if (tecla == 13) {
           document.getElementById("Button_busqueda").click();
           document.getElementById(sender.id).focus();
            e.preventDefault();

        }

    } catch (err) {
        alert(err.message + " funcion acti_busq_general_lista " + err.message);
    }
}
function activa_load_file_seleccion() {
    try {
        document.getElementById("Hidden_tip_adjunt").value = "wf";
        document.getElementById("Button_tool_adjunta_documento_relacionado").click();

    } catch (err) {
        alert(err.message + " funcion activa_load_file_seleccion " + err.message);
    }
}

function prevent_autoriza_tarea(event, element) {
    try {
        var DNvalues_;
        if (element.checked == true) {
            DNvalues_ = 1;
        } else {
            DNvalues_ = 0;
        }
        Get_autoriza_tarea_workflow(DNvalues_, element.id)
    } catch (ex) {
        alert("Fucion prevent_autoriza_tarea " + ex.message);
    }
}
function Get_autoriza_tarea_workflow(DNvalues_, DName) {
    try {
        $.ajax({
            async: true,
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            url: "../webservice/WebServiceWorkflow.asmx/Get_autoriza_tarea_workflow",
            data: "{'DName':'" + DNvalues_ + "'}",
            dataType: "json",
            success: function (data) {
                if (data.d == "YES") {
                    WF_ESTATUS_SERVICE = "yes";
                } else {
                    WF_ESTATUS_SERVICE = "yes";
                    alert(data.d);
                    var ch = document.getElementById(DName).checked;
                    if (document.getElementById(DName).checked == false) {
                        document.getElementById(DName).checked = true;
                    } else {
                        document.getElementById(DName).checked = false;
                    }
                    
                }
            },
            error: function (result) {
                if (DNvalues_ == 1) {
                    document.getElementById(DName).checked = false;
                } else {
                    document.getElementById(DName).checked = true;
                }
                //alert("Error.... " + result)
            }, compelete: function () {
                WF_ESTATUS_SERVICE = "yes";
            }
        })

    } catch (e) {
        alert("Inconsistencia general funcíon elimina_regitro_producion_service " + e.mensaje);
    }
}
function GetLista_lista_actividades_ruta(name_texbox) {
    function extractLast(term) {
        return term;
    }
    $("#" + name_texbox)
        .on("keydown", function (event) {
            if (event.keyCode === $.ui.keyCode.TAB &&
                $(this).autocomplete("instance").menu.active) {
                event.preventDefault();
            }
        })
        .autocomplete({
            source: function (request, response) {
                var param = { keyword: $('#' + name_texbox).val() };
                $.ajax({
                    url: "../webservice/WebServiceWorkflow.asmx/GetLista_lista_actividades_ruta",
                    data: "{'DName':'" + document.getElementById(name_texbox).value + "'}",
                    dataType: "json",
                    type: "POST",
                    traditional: true,
                    processData: false,
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        term: extractLast(request.term)
                        response($.ui.autocomplete.filter(
                         data.d, extractLast(request.term)));
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert(textStatus);
                    }
                });
            }, select: function (event, ui) {
                document.getElementById(name_texbox).value = ui.item.label;
                document.getElementById("Button_tool_busqueda_enviar_actividad").click();
            }, minLength: 3, max: 10, scroll: true
        });
}
function service_posibles_datos_tramites_() {
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
                    url: "../webservice/WebServiceWorkflow.asmx/GetPosiblesDatos_lista_tareas_workflow",
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
               document.getElementById("Button_tool_search_lista_tareas").click();
            }

            , minLength: 1, max: 10, scroll: true
        });
}
function service_GetPosiblesDatos_lista_tareas_pendientes() {
    function split(val) {
        return val.split(/,\s*/);
    }
    function extractLast(term) {
        return split(term).pop();
    }
    $("#busqueda_lista_pendiente")
         .on("keydown", function (event) {
             if (event.keyCode === $.ui.keyCode.TAB &&
                 $(this).autocomplete("instance").menu.active) {
                 event.preventDefault();
             }
         })
        .autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: "../webservice/WebServiceWorkflow.asmx/GetPosiblesDatos_lista_tareas_pendientes",
                    data: "{'DName':'" + document.getElementById("busqueda_lista_pendiente").value + "'}",
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
                document.getElementById("busqueda_lista_pendiente").value = ui.item.label;
                document.getElementById("Button_tool_consulta_lista_tareas_pendiente").click();
            }

            , minLength: 3, max: 10, scroll: true
        });
}
function alarma_pendiente_aprobacion() {
    try {
        var doc = document.getElementById("Hidden_estado_pendiente_aprobacion");
        if (doc === null) { return false; }
        if (document.getElementById("Hidden_estado_pendiente_aprobacion").value == "YES") {
            if (document.getElementById("ImageButton_pendiente_aprobacion_").className === "boton_tool") {
                document.getElementById("ImageButton_pendiente_aprobacion_").className.replace("boton_tool", "");
                document.getElementById("ImageButton_pendiente_aprobacion_").className = "boton_tool_inactive";
                return false;
            }
            if (document.getElementById("ImageButton_pendiente_aprobacion_").className === "boton_tool_inactive") {
                document.getElementById("ImageButton_pendiente_aprobacion_").className.replace("boton_tool_inactive", "");
                document.getElementById("ImageButton_pendiente_aprobacion_").className = "boton_tool";
                return false;
            }
        }
        if (document.getElementById("Hidden_estado_pendiente_aprobacion").value == "NO" && document.getElementById("ImageButton_pendiente_aprobacion_").className === "boton_tool_inactive") {
            document.getElementById("ImageButton_pendiente_aprobacion_").className.replace("boton_tool_inactive", "boton_tool");
        }
       
    }
    catch (err) {
        //alert(err.message + " Funcion alarma_pendiente_aprobacion");
    }
}
function alarma_actividad_pendiente() {
    try {
        try {
            $.ajax({
                async: true,
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                url: "../webservice/WebServiceWorkflow.asmx/Get_numero_tareas_pendientes",
                data: "{'DName':'" + "" + "'}",
                dataType: "json",
                success: function (data) {
                    if (data.d) {
                        document.getElementById("pendiente_db").textContent = "Pendientes " + data.d;
                    }
                },
                error: function (result) {
                    //alert("Error.... " + result)

                }, compelete: function () {

                }
            })

        } catch (e) {
            //alert("Inconsistencia general funcíon alarma_actividad_pendiente " + e.mensaje);
        }


    }
    catch (err) {
        //alert(err.message + " Funcion alarma_actividad_pendiente");
    }
}
function activa_busqueda_tareas_wf() {
    try {
       
        document.getElementById("Button_activa_search").click();
    }
    catch (err) {
        //alert(err.message + " Funcion alarma_actividad_pendiente");
    }
}
function service_tarea_seleccionada_workflow() {
    try {
        try {
            $.ajax({
                async: true,
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                url: "../webservice/WebServiceWorkflow.asmx/Get_id_tarea_seleccionada",
                data: "{'DName':'" + "" + "'}",
                dataType: "json",
                success: function (data) {
                    if (data.d) {
                        ESTADO_INICIALIZACION = data.d;
                    }
                },
                error: function (result) {
                    //alert("Error.... " + result)
                   
                }, compelete: function () {

                }
            })

        } catch (e) {
            //alert("Inconsistencia general funcíon alarma_actividad_pendiente " + e.mensaje);
        }


    }
    catch (err) {
        //alert(err.message + " Funcion alarma_actividad_pendiente");
    }
}
function alarma_nota_actividad() {
    try {
        $.ajax({
            async: true,
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            url: "../webservice/WebServiceWorkflow.asmx/Get_numero_nota_tarea",
            data: "{'DName':'" + "" + "'}",
            dataType: "json",
            success: function (data) {
                if (data) {
                    if (document.getElementById("nota_db")) {
                        document.getElementById("nota_db").textContent = "Notas " + data.d;
                    }

                }
            },
            error: function (result) {
                //alert("Error.... " + result)
            }, compelete: function () {

            }
        })

    } catch (e) {
        //alert("Inconsistencia general funcíon alarma_nota_actividad " + e.mensaje);
    }

}
function service_actualiza_indice_workflow(tipo_indice_actualiza) {
    try {
        var para_meter_ca = new Array();
        var input_tag = $('#raw_some_table :input');
        for (var i = 0; i < input_tag.length; i++) {
            para_meter_ca.push({
                nombre_campo: input_tag[i].id, valor_campo: input_tag[i].value.replace("'",""), tipo_campo: ""
            });
        }
        var hident_tag = $('.dec_000_21_000');
        for (var i = 0; i < hident_tag.length; i++) {
            para_meter_ca.push({
                nombre_campo: hident_tag[i].id, valor_campo: hident_tag[i].value.replace("'", ""), tipo_campo: ""
            });
        }
        var serialice = JSON.stringify(para_meter_ca);
        $.ajax('../webservice/WebServiceDocuarchi.asmx/Set_actualiza_indice_docuarchi', {
            data: "{" + "'parameter':'" + serialice + "','tipo_indice_actualiza':'" + tipo_indice_actualiza + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d !== "YES") {
                    alert(data.d);
                    ESTADO_EVENT_GENERAL = "out";
                } else { alert("Se actualizo correctamente"); ESTADO_EVENT_GENERAL = "out";}
            },
            error: function (result) {
                alert("Estatus : " + result.status + " Error " + result.responseJSON.Message + "  ")
                ESTADO_EVENT_GENERAL = "out";
            }, compelete: function () {

            }
        });
    } catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        alert(ex.message + " funcion service_actualiza_indice_workflow");
    }
}
///WEB SERVICE DETALLE PROCESOS IMAGNES
function Service_lista_log_procesing_image_workflow(table, id_tarea) {
    try {
        $.ajax('../webservice/WebServiceWorkflow.asmx/Service_lista_log_procesing_image_workflow', {
            data: "{'parameter':'" + id_tarea + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].result !== "YES") {
                    mystopfunction_update_table_boot_manager(table, data.d[0].result, data.d);
                    $find("ModalPopupExtender_edition_detail_document_proces_workflow").show();
                    auto_zise_popup_modal_conten_procesing_image_worflow("div_content_tabla_procesa_detail_document_proces_workflow", "div_content_tabla_procesa_detail_document_proces_workflow");
                    ESTADO_EVENT_GENERAL = "out";
                } else {
                    if (data.d[0].Id_log_docuarchi == -1) {
                        mystopfunction_update_table_boot_manager(table, "ZERO", data.d);
                        $find("ModalPopupExtender_edition_detail_document_proces_workflow").show();
                        auto_zise_popup_modal_conten_procesing_image_worflow("div_content_tabla_procesa_detail_document_proces_workflow", "div_content_tabla_procesa_detail_document_proces_workflow");
                        ESTADO_EVENT_GENERAL = "out";
                    } else {
                        mystopfunction_update_table_boot_manager(table, data.d[0].result, data.d);
                        $find("ModalPopupExtender_edition_detail_document_proces_workflow").show();
                        auto_zise_popup_modal_conten_procesing_image_worflow("div_content_tabla_procesa_detail_document_proces_workflow", "div_content_tabla_procesa_detail_document_proces_workflow");
                        ESTADO_EVENT_GENERAL = "out";
                    }

                }
            },
            error: function (result) {
                mystopfunction_update_table_boot_manager(table, result.responseText, "");
                ESTADO_EVENT_GENERAL = "out";
            }, compelete: function () {

            }
        });
    } catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        alert(ex.message + " funcion Service_lista_log_procesing_image_workflow");
    }
}
function Service_lista_copia_documento_expediente(table, id_tarea) {
    try {
        $.ajax('../webservice/WebServiceWorkflow.asmx/Service_lista_copia_documento_expediente', {
            data: "{'parameter':'" + id_tarea + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].result !== "YES") {
                    mystopfunction_update_table_boot_manager(table, data.d[0].result, data.d);
                    $find("ModalPopupExtender_edition_detail_copy_document_expediente_wf").show();
                    auto_zise_popup_modal_conten_copy_document_expediente("div_content_tabla_procesa_detail_copy_document_expediente_wf", "div_content_tabla_procesa_detail_copy_document_expediente_wf");
                    ESTADO_EVENT_GENERAL = "out";
                } else {
                    if (data.d[0].id_relacion_wf_produccion == -1) {
                        mystopfunction_update_table_boot_manager(table, "ZERO", data.d);
                        $find("ModalPopupExtender_edition_detail_copy_document_expediente_wf").show();
                        auto_zise_popup_modal_conten_copy_document_expediente("div_content_tabla_procesa_detail_copy_document_expediente_wf", "div_content_tabla_procesa_detail_copy_document_expediente_wf");
                        ESTADO_EVENT_GENERAL = "out";
                    } else {
                        mystopfunction_update_table_boot_manager(table, data.d[0].result, data.d);
                        $find("ModalPopupExtender_edition_detail_copy_document_expediente_wf").show();
                        auto_zise_popup_modal_conten_copy_document_expediente("div_content_tabla_procesa_detail_copy_document_expediente_wf", "div_content_tabla_procesa_detail_copy_document_expediente_wf");
                        ESTADO_EVENT_GENERAL = "out";
                    }

                }
            },
            error: function (result) {
                mystopfunction_update_table_boot_manager(table, result.responseText, "");
                ESTADO_EVENT_GENERAL = "out";
            }, compelete: function () {

            }
        });
    } catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        alert(ex.message + " funcion Service_lista_copia_documento_expediente");
    }
}
////WEB SERVICE ACTUALIZA NOTA TAREA
function Service_actualiza_nota_tarea_workflow(id_nota, value_nota) {
    try {
        $.ajax('../webservice/WebServiceWorkflow.asmx/Service_actualiza_nota_tarea_workflow', {
            data: "{" + "'parameter':'" + id_nota + "','value_nota':'" + value_nota + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_result !== "YES") {
                    alert(data.d[0].error_result);
                    ESTADO_EVENT_GENERAL = "out";
                } else {   
                    actualiza_gre_campo_wf_lista('GridView_lista_notas', data.d[0].identificador, data.d[0].value, 'NOTA');
                    $find("ModalPopupExtender_edition_nota_respuesta").hide();
                    ESTADO_EVENT_GENERAL = "out";
                }
            }, error: function (xception, textStatus, errorThrown) {
                ESTADO_EVENT_GENERAL = "out";
                if (xception.status === 0) {
                    alert('Not connect: Verify Network.');

                } else if (xception.status == 404) {
                    alert('Requested page not found [404]');

                } else if (xception.status == 500) {
                    alert('Internal Server Error [500].' + xception.responseText);


                } else if (textStatus === 'parsererror') {
                    alert('Requested JSON parse failed.');


                } else if (textStatus === 'timeout') {
                    alert('Time out error.');


                } else if (textStatus === 'abort') {
                    alert('Ajax request aborted.');

                } else {
                    alert('Uncaught Error: ' + xception.responseText);

                }
            }
        });
    }
    catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        alert(ex.message);
    }
}
//WEB SERVICE ELIMINA ANOTAION
function Service_delete_nota_tarea_workflow(id_nota, value_nota) {
    try {
        $.ajax('../webservice/WebServiceWorkflow.asmx/Service_delete_nota_tarea_workflow', {
            data: "{" + "'parameter':'" + id_nota + "','value_nota':'" + value_nota + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_result !== "YES") {
                    alert(data.d[0].error_result);
                    ESTADO_EVENT_GENERAL = "out";
                } else {
                    let result = eliminar_fila_data_gred_nota(data.d[0].identificador, 'GridView_lista_notas');
                    if (result !== "YES") {
                        alert(result);
                    }
                    ESTADO_EVENT_GENERAL = "out";
                }
            }, error: function (xception, textStatus, errorThrown) {
                ESTADO_EVENT_GENERAL = "out";
                if (xception.status === 0) {
                    alert('Not connect: Verify Network.');

                } else if (xception.status == 404) {
                    alert('Requested page not found [404]');

                } else if (xception.status == 500) {
                    alert('Internal Server Error [500].' + xception.responseText);


                } else if (textStatus === 'parsererror') {
                    alert('Requested JSON parse failed.');


                } else if (textStatus === 'timeout') {
                    alert('Time out error.');


                } else if (textStatus === 'abort') {
                    alert('Ajax request aborted.');

                } else {
                    alert('Uncaught Error: ' + xception.responseText);

                }
            }
        });
    }
    catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        alert(ex.message);
    }
}
//WEB SERVICE ADD ANOTACION
function Service_add_nota_tarea_workflow(value_nota) {
    try {
        $.ajax('../webservice/WebServiceWorkflow.asmx/Service_add_nota_tarea_workflow', {
            data: "{'value_nota':'" + value_nota + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_result !== "YES") {
                    alert(data.d[0].error_result);
                    ESTADO_EVENT_GENERAL = "out";
                } else {
                    let result = insert_row_list_anotation(data, "GridView_lista_notas");
                    if (result !== "YES") {
                        alert(result);
                    }
                    $find("ModalPopupExtender_edition_nota_respuesta").hide();
                    ESTADO_EVENT_GENERAL = "out";
                }
            }, error: function (xception, textStatus, errorThrown) {
                ESTADO_EVENT_GENERAL = "out";
                if (xception.status === 0) {
                    alert('Not connect: Verify Network.');

                } else if (xception.status == 404) {
                    alert('Requested page not found [404]');

                } else if (xception.status == 500) {
                    alert('Internal Server Error [500].' + xception.responseText);


                } else if (textStatus === 'parsererror') {
                    alert('Requested JSON parse failed.');


                } else if (textStatus === 'timeout') {
                    alert('Time out error.');


                } else if (textStatus === 'abort') {
                    alert('Ajax request aborted.');

                } else {
                    alert('Uncaught Error: ' + xception.responseText);

                }
            }
        });
    }
    catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        alert(ex.message);
    }
}
//WEB SERVICE SOLICITA CONTENIDO NOTA
function Service_contenido_nota_tarea_workflow(id_nota, element_name) {
    try {
        $.ajax('../webservice/WebServiceWorkflow.asmx/Service_contenido_nota_tarea_workflow', {
            data: "{" + "'parameter':'" + id_nota + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_result !== "YES") {
                    alert(data.d[0].error_result);
                    ESTADO_EVENT_GENERAL = "out";
                } else {
                    document.getElementById(element_name).value = data.d[0].value;
                    document.getElementById("Button_actualizar_nota").style.display = "flex";
                    document.getElementById("Button_duardar_nota").style.display = "none";
                    document.getElementById("Label_nota_respuesta").innerHTML = "Nota " + data.d[0].identificador;
                    $find("ModalPopupExtender_edition_nota_respuesta").show();
                    auto_zise_nota_tarea();
                    ESTADO_EVENT_GENERAL = "out";
                }
            }, error: function (xception, textStatus, errorThrown) {
                ESTADO_EVENT_GENERAL = "out";
                if (xception.status === 0) {
                    alert('Not connect: Verify Network.');

                } else if (xception.status == 404) {
                    alert('Requested page not found [404]');

                } else if (xception.status == 500) {
                    alert('Internal Server Error [500].' + xception.responseText);


                } else if (textStatus === 'parsererror') {
                    alert('Requested JSON parse failed.');


                } else if (textStatus === 'timeout') {
                    alert('Time out error.');


                } else if (textStatus === 'abort') {
                    alert('Ajax request aborted.');

                } else {
                    alert('Uncaught Error: ' + xception.responseText);

                }
            }
        });
    }
    catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        alert(ex.message);
    }
}
function Service_Eval_tarea_default_workflow() {
    try {
        var id = 0;
        $.ajax('../webservice/WebServiceWorkflow.asmx/Service_Eval_tarea_default_workflow', {
            data: "{" + "'parameter':'" + id + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_gestion !== "YES") {
                    alert(data.d[0].error_gestion);
                    ESTADO_EVENT_GENERAL = "out";
                } else {
                    alert(data.d[0].result_service_control);
                    ESTADO_EVENT_GENERAL = "out";

                }
            }, error: function (xception, textStatus, errorThrown) {
                ESTADO_EVENT_GENERAL = "out";
                if (xception.status === 0) {

                    alert('Not connect: Verify Network.');

                } else if (xception.status == 404) {

                    alert('Requested page not found [404]');

                } else if (xception.status == 500) {

                    alert('Internal Server Error [500].' + xception.responseText);

                } else if (textStatus === 'parsererror') {

                    alert('Requested JSON parse failed.');

                } else if (textStatus === 'timeout') {

                    alert('Time out error.');

                } else if (textStatus === 'abort') {

                    alert('Ajax request aborted.');

                } else {

                    alert('Uncaught Error: ' + xception.responseText);

                }
            }, compelete: function () {

            }
        });
    } catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        alert(ex.message + " funcion Service_Eval_tarea_default_workflow");
    }
}
//-----------------SERVCI0S WEB GESTION AL USUARIO
//-------Solicita lista tipo de gestion al usuario
const Service_REST_crea_interface_registro_gestion = async (id_) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceWorkflow.asmx/Service_crea_interface_registro_gestion', {
                data: "{'id':" + "'" + id_ + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].error_sistema !== "YES") {
                        resolve(data.d[0].error_sistema);
                    } else {
                        ITEMS_DATOS_DROW = new Array();
                        $.each(data.d[0].item_sistema, function (k, v) {
                            ITEMS_DATOS_DROW.push(v);
                        });
                        if (document.getElementById('option_tipos_gestion')) {
                            var element_drow = document.getElementById('option_tipos_gestion');
                            $("#option_tipos_gestion").empty();
                            for (var i = 0; i < ITEMS_DATOS_DROW.length; i++) {
                                element_drow[i] = new Option(ITEMS_DATOS_DROW[i].text, ITEMS_DATOS_DROW[i].value);
                            }
                            element_drow.addEventListener("change", event_change_drowslisi_lista_tipos_gestion_usuario);
                        }
                        $("#modal_registro_gestion_usuario_wf").modal("show");
                        resolve("YES");
                    }
                }, error: function (xception, textStatus, errorThrown) {

                    if (xception.status === 0) {
                        resolve("Not connect: Verify Network.");


                    } else if (xception.status == 404) {
                        resolve("Requested page not found [404]");


                    } else if (xception.status == 500) {
                        resolve("Internal Server Error [500]." + xception.responseText);


                    } else if (textStatus === 'parsererror') {
                        resolve("Requested JSON parse failed.");


                    } else if (textStatus === 'timeout') {
                        return "Time out error.";


                    } else if (textStatus === 'abort') {
                        resolve("Ajax request aborted.");


                    } else {
                        resolve("Ajax request aborted." + xception.responseText);


                    }
                }
            });
        }
        catch (ex) {
            resolve(ex.message);
        }
    })
    let result = myPromise;
    return result;
}
//-------Solicita estado envio correo electrónico del tipo de gestion
const Service_REST_solicita_estado_envio_correo_gestion_usuario = async (id_, estado_send) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceWorkflow.asmx/Service_Solicita_estado_envio_correo_gestion_usuario', {
                data: "{'id':" + "'" + id_ + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].error_result !== "YES") {
                        resolve(data.d[0].error_result);
                    } else {
                        SEND_ENVIO_MAIL = data.d[0].estado_envio_correo;
                        resolve(data.d[0].error_result);
                    }
                }, error: function (xception, textStatus, errorThrown) {

                    if (xception.status === 0) {
                        resolve("Not connect: Verify Network.");


                    } else if (xception.status == 404) {
                        resolve("Requested page not found [404]");


                    } else if (xception.status == 500) {
                        resolve("Internal Server Error [500]." + xception.responseText);


                    } else if (textStatus === 'parsererror') {
                        resolve("Requested JSON parse failed.");


                    } else if (textStatus === 'timeout') {
                        return "Time out error.";


                    } else if (textStatus === 'abort') {
                        resolve("Ajax request aborted.");


                    } else {
                        resolve("Ajax request aborted." + xception.responseText);


                    }
                }
            });
        }
        catch (ex) {
            resolve(ex.message);
        }
    })
    let result = myPromise;
    return result;
}
//-------Registra gestion al usuario 
const Service_REST_registra_gestion_al_usuario = async (parameter) => {
    var serialice = JSON.stringify(parameter);
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceWorkflow.asmx/Service_registra_gestion_al_usuario', {
                data: "{'Parameter':" + "'" + serialice + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].error_result !== "YES") {
                        resolve(data.d[0].error_result);
                    } else {
                        $("#modal_registro_gestion_usuario_wf").modal("hide");
                        resolve(data.d[0].error_result);
                    }
                }, error: function (xception, textStatus, errorThrown) {

                    if (xception.status === 0) {
                        resolve("Not connect: Verify Network.");


                    } else if (xception.status == 404) {
                        resolve("Requested page not found [404]");


                    } else if (xception.status == 500) {
                        resolve("Internal Server Error [500]." + xception.responseText);


                    } else if (textStatus === 'parsererror') {
                        resolve("Requested JSON parse failed.");


                    } else if (textStatus === 'timeout') {
                        return "Time out error.";


                    } else if (textStatus === 'abort') {
                        resolve("Ajax request aborted.");


                    } else {
                        resolve("Ajax request aborted." + xception.responseText);


                    }
                }
            });
        }
        catch (ex) {
            resolve(ex.message);
        }
    })
    let result = myPromise;
    return result;
}
//-------Realiza la consulta de la gestion al usuario
const Service_REST_lista_gestion_al_usuario = async () => { 
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceWorkflow.asmx/Service_lista_gestion_al_usuario', {
                data: "{" + "'parameter':'" + "1" + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_result !== "YES") {
                        resolve(data.d[0].Error_result);
                    } else {
                        let class_stru_row_Gabinete_Generic = JSON.parse(data.d[0].Obj_ilist_row_generic);
                        init_row_feld_table_boostrap_table("table_lista_gestion_usuario", data.d[0].Obj_ilist_fileds_generic, class_stru_row_Gabinete_Generic, "", "table-bordered", "table-borderless");
                        $("#modal_lista_gestion_usuario_wf").modal("show");
                        
                        resolve("YES");
                    }
                }, error: function (xception, textStatus, errorThrown) {

                    if (xception.status === 0) {
                        resolve("Not connect: Verify Network.");


                    } else if (xception.status == 404) {
                        resolve("Requested page not found [404]");


                    } else if (xception.status == 500) {
                        resolve("Internal Server Error [500]." + xception.responseText);


                    } else if (textStatus === 'parsererror') {
                        resolve("Requested JSON parse failed.");


                    } else if (textStatus === 'timeout') {
                        return "Time out error.";


                    } else if (textStatus === 'abort') {
                        resolve("Ajax request aborted.");


                    } else {
                        resolve("Ajax request aborted." + xception.responseText);


                    }
                }
            });
        }
        catch (ex) {
            resolve(ex.message);
        }
    })
    let result = await myPromise;
    return result;

}
//-------Registra gestion al usuario 
const Service_REST_actualiza_registro_gestion_al_usuario = async (parameter) => {
    var serialice = JSON.stringify(parameter);
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceWorkflow.asmx/Service_actualiza_registro_gestion_al_usuario', {
                data: "{'Parameter':" + "'" + serialice + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].error_result !== "YES") {
                        resolve(data.d[0].error_result);
                    } else {
                        $("#modal_actualiza_registro_gestion_usuario_wf").modal("hide");
                        resolve(data.d[0].error_result);
                    }
                }, error: function (xception, textStatus, errorThrown) {

                    if (xception.status === 0) {
                        resolve("Not connect: Verify Network.");


                    } else if (xception.status == 404) {
                        resolve("Requested page not found [404]");


                    } else if (xception.status == 500) {
                        resolve("Internal Server Error [500]." + xception.responseText);


                    } else if (textStatus === 'parsererror') {
                        resolve("Requested JSON parse failed.");


                    } else if (textStatus === 'timeout') {
                        return "Time out error.";


                    } else if (textStatus === 'abort') {
                        resolve("Ajax request aborted.");


                    } else {
                        resolve("Ajax request aborted." + xception.responseText);


                    }
                }
            });
        }
        catch (ex) {
            resolve(ex.message);
        }
    })
    let result = myPromise;
    return result;
}
//-------Crea la interfaz de edición de la gestión al usuario
const Service_REST_crea_interfaz_gestion_al_usuario = async (id_registro_gestion) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceWorkflow.asmx/Service_crea_interfaz_gestion_al_usuario', {
                data: "{" + "'parameter':'" + id_registro_gestion + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].error_result !== "YES") {
                        resolve(data.d[0].error_result);
                    } else {
                        GESTION_USUARIO_WF_ARRAY = new Array();
                        $.each(data.d, function (k, v) {
                            GESTION_USUARIO_WF_ARRAY.push(v);
                        });
                        resolve("YES");
                    }
                }, error: function (xception, textStatus, errorThrown) {
                    if (xception.status === 0) {
                        resolve("Not connect: Verify Network.");
                    } else if (xception.status == 404) {
                        resolve("Requested page not found [404]");

                    } else if (xception.status == 500) {
                        resolve("Internal Server Error [500]." + xception.responseText);

                    } else if (textStatus === 'parsererror') {
                        resolve("Requested JSON parse failed.");

                    } else if (textStatus === 'timeout') {
                        return "Time out error.";

                    } else if (textStatus === 'abort') {
                        resolve("Ajax request aborted.");

                    } else {
                        resolve("Ajax request aborted." + xception.responseText);
                    }
                }
            });
        }
        catch (ex) {
            resolve(ex.message);
        }
    })
    let result = await myPromise;
    return result;

}
//-------Activa el servicio de integración para subir archivos enlace sii
const ServiceRESTActivaAdjuntaDocumentoServicioIntegracionEnlace = async (parameter) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceAdjuntaDocumentoServicioIntegracion.asmx/ServiceAdjuntaDocumentoServicioIntegracionEnlace', {
                data: "{" + "'parameter':'" + parameter + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].ErrorService !== "YES") {
                        resolve(data.d[0].ErrorService);
                    } else {
                        IListCservicioIntegracionAdjuntaDocumento = new Array();
                        $.each(data.d, function (k, v) {
                            IListCservicioIntegracionAdjuntaDocumento.push(v);
                        });
                        resolve("YES");
                    }
                }, error: function (xception, textStatus, errorThrown) {
                    if (xception.status === 0) {
                        resolve("Not connect: Verify Network.");
                    } else if (xception.status == 404) {
                        resolve("Requested page not found [404]");

                    } else if (xception.status == 500) {
                        resolve("Internal Server Error [500]." + xception.responseText);

                    } else if (textStatus === 'parsererror') {
                        resolve("Requested JSON parse failed.");

                    } else if (textStatus === 'timeout') {
                        return "Time out error.";

                    } else if (textStatus === 'abort') {
                        resolve("Ajax request aborted.");

                    } else {
                        resolve("Ajax request aborted." + xception.responseText);
                    }
                }
            });
        }
        catch (ex) {
            resolve(ex.message);
        }
    })
    let result = await myPromise;
    return result;

}
//-------Activa el servicio de integración para subir archivos
const ServiceRESTActivaAdjuntaDocumentoServicioIntegracion = async (parameter) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceAdjuntaDocumentoServicioIntegracion.asmx/ServiceAdjuntaDocumentoServicioIntegracion', {
                data: "{" + "'parameter':'" + parameter + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].ErrorService !== "YES") {
                        resolve(data.d[0].ErrorService);
                    } else {
                        IListCservicioIntegracionAdjuntaDocumento = new Array();
                        $.each(data.d, function (k, v) {
                            IListCservicioIntegracionAdjuntaDocumento.push(v);
                        });
                        resolve("YES");
                    }
                }, error: function (xception, textStatus, errorThrown) {
                    if (xception.status === 0) {
                        resolve("Not connect: Verify Network.");
                    } else if (xception.status == 404) {
                        resolve("Requested page not found [404]");

                    } else if (xception.status == 500) {
                        resolve("Internal Server Error [500]." + xception.responseText);

                    } else if (textStatus === 'parsererror') {
                        resolve("Requested JSON parse failed.");

                    } else if (textStatus === 'timeout') {
                        return "Time out error.";

                    } else if (textStatus === 'abort') {
                        resolve("Ajax request aborted.");

                    } else {
                        resolve("Ajax request aborted." + xception.responseText);
                    }
                }
            });
        }
        catch (ex) {
            resolve(ex.message);
        }
    })
    let result = await myPromise;
    return result;

}
//-------Solicita lista de archivos relacionados a un recibo SII
const ServiceRESTServiceSolicitaListaConstanciaInscripcionSII = async (CodigoBarraSII, name_table, name_parent_table) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebService_integracion_sii.asmx/ServiceSolicitaListaConstanciaInscripcionSII', {
                data: "{'CodigoBarraSII':" + "'" + CodigoBarraSII + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_result !== "YES") {
                        resolve(data.d[0].Error_result);
                    } else {
                        let class_stru_row_Gabinete_Generic = JSON.parse(data.d[0].row_table_boot);
                        init_row_feld_table_boostrap_table(name_table, data.d[0].field_table_boot, class_stru_row_Gabinete_Generic, name_parent_table, "table-bordered", "table-borderless");
                        console.log(class_stru_row_Gabinete_Generic);
                        $("#modal_sii_constancias_inscripcion").modal("show");
                        AutoSizeListaConstaciasSII();
                        resolve("YES");
                    }
                }, error: function (xception, textStatus, errorThrown) {

                    if (xception.status === 0) {
                        resolve("Not connect: Verify Network.");


                    } else if (xception.status == 404) {
                        resolve("Requested page not found [404]");


                    } else if (xception.status == 500) {
                        return ("Internal Server Error [500]." + xception.responseText);


                    } else if (textStatus === 'parsererror') {
                        resolve("Requested JSON parse failed.");


                    } else if (textStatus === 'timeout') {
                        resolve("Time out error.");


                    } else if (textStatus === 'abort') {
                        resolve("Ajax request aborted.");


                    } else {
                        resolve("Ajax request aborted." + xception.responseText);


                    }
                }
            });
        }
        catch (ex) {
            resolve(ex.message);
        }
    })
    let result = await myPromise;
    return result;
}
/**
 * Servicio que solicita la estructura de configuración de un tramite para digitalización
 * @param {any} IdTipoTramite
 */
const ServiceRESTsolicitaEstructuraConfiguracion = async (IdTipoTramite) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebService_Config_Digitalizacion.asmx/ServiceSolicitaEstructuraConfiguracion', {
                data: "{'IdTipoTramite':'" + IdTipoTramite  + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].error_gestion !== "YES") {
                        resolve(data.d[0].error_gestion);
                    } else {
                        CAinterfaceConfigDigitaliza = new Array();
                        CAinterfaceConfigDigitaliza = data.d[0];
                        resolve("YES");
                      
                    }
                }, error: function (xception, textStatus, errorThrown) {

                    if (xception.status === 0) {
                        resolve('Not connect: Verify Network.');
                    } else if (xception.status == 404) {
                        resolve('Requested page not found [404]');
                    } else if (xception.status == 500) {
                        resolve('Internal Server Error [500].' + xception.responseText);
                    } else if (textStatus === 'parsererror') {
                        resolve('Requested JSON parse failed.');
                    } else if (textStatus === 'timeout') {
                        resolve('Time out error.');
                    } else if (textStatus === 'abort') {
                        resolve('Ajax request aborted.');
                    } else {
                        resolve('Uncaught Error: ' + xception.responseText);
                    }
                }
            });
        }
        catch (ex) {
            resolve('ServiceRESTsolicitaEstructuraConfiguracion  ' + ex.message);
        }
    })
    let result = await myPromise;
    return result;
}
//-------Servicio web que solicita la lista de los sellos de inscripcion sii -------//
const ServiceRESTsolicitaRegistrosSellosSII = async () => {
    let myPromise = new Promise(function (resolve) {
    try {
        $.ajax('../webservice/WebService_integracion_sii.asmx/Service_solicita_registros_sellos_sii', {
            data: "{'na':" + "'1'" + "}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_gestion !== "YES") {
                    resolve(data.d[0].error_gestion);
                } else {
                    CIncripcionSII = [];
                    if (data.d[0].structure_lis.length > 0) {  
                        for (i = 0; i < data.d[0].structure_lis.length; i++) {
                             CIncripcionSII.push({
                                LIBRO_SII: data.d[0].structure_lis[i].libro, REGISTRO_SII: data.d[0].structure_lis[i].registro, FECHA_SII: data.d[0].structure_lis[i].fecha,
                                MATRICULA_SII: data.d[0].structure_lis[i].matricula,
                                PROPONENTE_SII: data.d[0].structure_lis[i].proponente, NIT_SII: data.d[0].structure_lis[i].identificacion, RSOCIAL_SII: data.d[0].structure_lis[i].nombre,
                                ACTO_SII: data.d[0].structure_lis[i].acto,
                                NOTICIA_SII: data.d[0].structure_lis[i].noticia, RADICADO_SII: data.d[0].recibo, COD_BARRA_SII: data.d[0].codigo, URL_SII: data.d[0].structure_lis[i].url,
                                NACTO_SII: data.d[0].structure_lis[i].nacto
                             });
                        }
                    }
                    resolve("YES");
                }
            }, error: function (xception, textStatus, errorThrown) {
                if (xception.status === 0) {
                    resolve("Not connect: Verify Network.");
                } else if (xception.status == 404) {
                    resolve("Requested page not found [404]");

                } else if (xception.status == 500) {
                    resolve("Internal Server Error [500]." + xception.responseText);

                } else if (textStatus === 'parsererror') {
                    resolve("Requested JSON parse failed.");

                } else if (textStatus === 'timeout') {
                    return "Time out error.";

                } else if (textStatus === 'abort') {
                    resolve("Ajax request aborted.");

                } else {
                    resolve("Ajax request aborted." + xception.responseText);
                }
            }
        });
    }
    catch (ex) {
        resolve('Service_solicita_registros_sellos_sii  ' + ex.message);
        }
    })
    let result = await myPromise;
    return result;
}
//-------Servicio que solicita la estructura del cache de inscripción SII-----///             
const ServiceRESTsolicitaEstructuraCacheInscripcionRadicado = async (ReciboSII) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebService_integracion_sii.asmx/ServiceSolicitaEstructuraCacheInscripcionRadicado', {
                data: "{" + "'ReciboSII':'" + ReciboSII + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].AppError !== "YES") {
                        resolve(data.d[0].AppError);
                    } else {
                        CacheInscripcion = new Array();
                        $.each(data.d, function (k, v) {
                            CacheInscripcion.push(v);
                        });
                        resolve("YES");
                    }
                }, error: function (xception, textStatus, errorThrown) {
                    if (xception.status === 0) {
                        resolve("Not connect: Verify Network.");
                    } else if (xception.status == 404) {
                        resolve("Requested page not found [404]");

                    } else if (xception.status == 500) {
                        resolve("Internal Server Error [500]." + xception.responseText);

                    } else if (textStatus === 'parsererror') {
                        resolve("Requested JSON parse failed.");

                    } else if (textStatus === 'timeout') {
                        resolve("Time out error.");

                    } else if (textStatus === 'abort') {
                        resolve("Ajax request aborted.");

                    } else {
                        resolve("Ajax request aborted." + xception.responseText);
                    }
                }
            });
        }
        catch (ex) {
            resolve(ex.message);
        }
    })
    let result = await myPromise;
    return result;
}
//-------Servicio que solicita la estructura del cache de creación expediente SII-----///             
const ServiceRESTregistraCacheInscripcionRadicadoSII = async (CIncripcionSII, IdTramite) => {
    var serialice = JSON.stringify(CIncripcionSII);
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebService_integracion_sii.asmx/ServiceRegistraCacheInscripcionRadicadoSII', {
                data: "{" + "'CIncripcionSII':'" + serialice + "','IdTramite':'" + IdTramite + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].AppError !== "YES") {
                        resolve(data.d[0].AppError);
                    } else {
                        CacheInscripcion = new Array();
                        $.each(data.d, function (k, v) {
                            CacheInscripcion.push(v);
                        });
                        resolve("YES");
                    }
                }, error: function (xception, textStatus, errorThrown) {
                    if (xception.status === 0) {
                        resolve("Not connect: Verify Network.");
                    } else if (xception.status == 404) {
                        resolve("Requested page not found [404]");

                    } else if (xception.status == 500) {
                        resolve("Internal Server Error [500]." + xception.responseText);

                    } else if (textStatus === 'parsererror') {
                        resolve("Requested JSON parse failed.");

                    } else if (textStatus === 'timeout') {
                        resolve("Time out error.");

                    } else if (textStatus === 'abort') {
                        resolve("Ajax request aborted.");

                    } else {
                        resolve("Ajax request aborted." + xception.responseText);
                    }
                }
            });
        }
        catch (ex) {
            resolve(ex.message);
        }
    })
    let result = await myPromise;
    return result;
}
//-------Servicio acualiza indice SII-----///             
const ServiceRESTactualizaIndiceDocumentosSII = async (CIncripcionSII, IdTramite, ReciboSII) => {
    var serialice = JSON.stringify(CIncripcionSII);
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebService_integracion_sii.asmx/ServiceActualizaIndiceDocumentosSII', {
                data: "{" + "'CIncripcionSII':'" + serialice + "','IdTramite':'" + IdTramite + "','ReciboSII':'" + ReciboSII + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].AppError !== "YES") {
                        resolve(data.d[0].AppError);
                    } else {
                        resolve("YES");
                    }
                }, error: function (xception, textStatus, errorThrown) {
                    if (xception.status === 0) {
                        resolve("Not connect: Verify Network.");
                    } else if (xception.status == 404) {
                        resolve("Requested page not found [404]");

                    } else if (xception.status == 500) {
                        resolve("Internal Server Error [500]." + xception.responseText);

                    } else if (textStatus === 'parsererror') {
                        resolve("Requested JSON parse failed.");

                    } else if (textStatus === 'timeout') {
                        resolve("Time out error.");

                    } else if (textStatus === 'abort') {
                        resolve("Ajax request aborted.");

                    } else {
                        resolve("Ajax request aborted." + xception.responseText);
                    }
                }
            });
        }
        catch (ex) {
            resolve(ex.message);
        }
    })
    let result = await myPromise;
    return result;
}


/**
 * Servicio web que guarda la constancia de isncripción integraciÓn SII
 * @param {any} CIncripcionSII
 * @param {any} IdTipoTramite
 */
const ServiceRESTGuardaConstanciaInscripcionSII = async (CIncripcionSII, IdTipoTramite, IdTramite) => {
    let serialice = JSON.stringify(CIncripcionSII);
    let myPromise = new Promise(function (resolve) {
    try {
        $.ajax('../webservice/WebService_integracion_sii.asmx/SeviceGuardaConstanciaInscripcionSII', {
            data: "{'CIncripcionSII':'[" + serialice + "]','IdTipoTramite':'" + IdTipoTramite + "','IdTramite':'" + IdTramite + "'}" ,
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_gestion !== "YES") {
                    resolve(data.d[0].error_gestion);
                } else {
                    insert_row_documento_relacionado(data.d[0].dato_lista, "wf", 1);
                    resolve("YES");
                }
            }, error: function (xception, textStatus, errorThrown) {
                
                if (xception.status === 0) {
                    resolve('Not connect: Verify Network.');

                } else if (xception.status == 404) {
                    resolve('Requested page not found [404]');

                } else if (xception.status == 500) {
                    resolve('Internal Server Error [500].' + xception.responseText);

                } else if (textStatus === 'parsererror') {
                    resolve('Requested JSON parse failed.');

                } else if (textStatus === 'timeout') {
                    resolve('Time out error.');


                } else if (textStatus === 'abort') {
                    resolve('Ajax request aborted.');


                } else {
                    resolve('Uncaught Error: ' + xception.responseText);


                }
            }
        });
    }
    catch (ex) {
        resolve('ServiceRESTGuardaConstanciaInscripcionSII  ' + ex.message);

        }
    })
    let result = await myPromise;
    return result;
}

//-------Solicita estructura para clasificación tipologia documental para inscripcion SII-----///
const ServiceRESTsolicitaListaTiposDocumentalesTramiteListaAdjunta = async (IdTramite,TramiteDefault) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceWorkflow.asmx/ServiceSolicitaListaTiposDocumentalesTramiteListaAdjunta', {
                data: "{" + "'IdTramite':'" + IdTramite + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].error_sistema != "YES") {
                        resolve(data.d[0].error_sistema);
                    } else {
                        
                        $("#option_lista_tipologia").empty();
                        var element_drow = document.getElementById('option_lista_tipologia');
                        for (var i = 0; i < data.d[0].item_sistema.length; i++) {
                            element_drow[i] = new Option(data.d[0].item_sistema[i].text, data.d[0].item_sistema[i].value);
                        }
                        for (const option of element_drow.options) {
                            for (var i2 = 0; i2 < (DafaultTramiteSelloSII.length - 1) ; i2++) {    
                                if (DafaultTramiteSelloSII[i2].Name == option.text) {
                                    option.selected = true;
                                }
                            }
                        }
                        

                        $("#modal_guarda_archivos_inscripcion_sii").modal("show");
                        resolve("YES");

                    }
                }, error: function (xception, textStatus, errorThrown) {

                    if (xception.status === 0) {
                        resolve("Not connect: Verify Network.");


                    } else if (xception.status == 404) {
                        resolve("Requested page not found [404]");


                    } else if (xception.status == 500) {
                        resolve("Internal Server Error [500]." + xception.responseText);


                    } else if (textStatus === 'parsererror') {
                        resolve("Requested JSON parse failed.");


                    } else if (textStatus === 'timeout') {
                        return "Time out error.";


                    } else if (textStatus === 'abort') {
                        resolve("Ajax request aborted.");


                    } else {
                        resolve("Ajax request aborted." + xception.responseText);


                    }
                }
            });
        }
        catch (ex) {
            resolve(ex.message);
        }
    })
    let result = await myPromise;
    return result;
}
//-------Solicita lista de archivos anexos relacionados a un recibo SII----------///
/**
 * /
 * @param {any} RadicadoSII
 * @param {any} name_table
 * @param {any} name_parent_table
 */
const ServiceRESTsolicitaArchivosAnexosrelacionadosRadicadoSII = async (RadicadoSII, name_table, name_parent_table) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebService_integracion_sii.asmx/ServiceSolicitaArchivosAnexosrelacionadosRadicadoSII', {
                data: "{'RadicadoSII':" + "'" + RadicadoSII + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_result !== "YES") {
                        resolve(data.d[0].Error_result);
                    } else {
                        let class_stru_row_Gabinete_Generic = JSON.parse(data.d[0].row_table_boot);
                        init_row_feld_table_boostrap_table(name_table, data.d[0].field_table_boot, class_stru_row_Gabinete_Generic, name_parent_table, "table-bordered", "table-borderless");
                        //if (document.getElementById("h_title_radicado_sii")) {
                       //     document.getElementById("h_title_radicado_sii").innerText = "RADICADO " + radicado_sii;
                        //}
                        CDParameterAnexosSII = new Array();
                        CDParameterAnexosSII.push({ ReciboSII: data.d[0].ReciboSII, CodigoBarras: data.d[0].CodigoBarras, Gabinete: data.d[0].Gabinete });
                        $("#modal_sii_anexos_recibo").modal("show");
                        AutoSizeListaAnexosSII();
                        resolve("YES");
                    }
                }, error: function (xception, textStatus, errorThrown) {

                    if (xception.status === 0) {
                        resolve("Not connect: Verify Network.");


                    } else if (xception.status == 404) {
                        resolve("Requested page not found [404]");


                    } else if (xception.status == 500) {
                        resolve ("Internal Server Error [500]." + xception.responseText);


                    } else if (textStatus === 'parsererror') {
                        resolve("Requested JSON parse failed.");


                    } else if (textStatus === 'timeout') {
                        resolve("Time out error.");


                    } else if (textStatus === 'abort') {
                        resolve("Ajax request aborted.");


                    } else {
                        resolve("Ajax request aborted." + xception.responseText);


                    }
                }
            });
        }
        catch (ex) {
            resolve(ex.message);
        }
    })
    let result = await myPromise;
    return result;
}
//-------Solicita estructura para clasificación tipologia documental para anexos SII-----///
const ServiceRESTsolicitaListaTiposDocumentalesTramiteListaAdjuntaAnexo = async (IdTramite, TramiteDefault) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceWorkflow.asmx/ServiceSolicitaListaTiposDocumentalesTramiteListaAdjunta', {
                data: "{" + "'IdTramite':'" + IdTramite + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].error_sistema != "YES") {
                        resolve(data.d[0].error_sistema);
                    } else {
                        $("#option_lista_tipologia_anexo").empty();
                        var element_drow = document.getElementById('option_lista_tipologia_anexo');
                        for (var i = 0; i < data.d[0].item_sistema.length; i++) {
                            element_drow[i] = new Option(data.d[0].item_sistema[i].text, data.d[0].item_sistema[i].value);
                        }
                        for (const option of element_drow.options) {
                            for (var i2 = 0; i2 < (DafaultTramiteSelloSII.length - 1); i2++) {
                                if (DafaultTramiteSelloSII[i2].Name == option.text) {
                                    option.selected = true;
                                }
                            }
                        }
                        $("#modal_guarda_archivos_anexos_sii").modal("show");
                        resolve("YES");
                    }
                }, error: function (xception, textStatus, errorThrown) {

                    if (xception.status === 0) {
                        resolve("Not connect: Verify Network.");


                    } else if (xception.status == 404) {
                        resolve("Requested page not found [404]");


                    } else if (xception.status == 500) {
                        resolve("Internal Server Error [500]." + xception.responseText);


                    } else if (textStatus === 'parsererror') {
                        resolve("Requested JSON parse failed.");


                    } else if (textStatus === 'timeout') {
                        return "Time out error.";


                    } else if (textStatus === 'abort') {
                        resolve("Ajax request aborted.");


                    } else {
                        resolve("Ajax request aborted." + xception.responseText);


                    }
                }
            });
        }
        catch (ex) {
            resolve(ex.message);
        }
    })
    let result = await myPromise;
    return result;
}
function Set_documento_seleccionado(id_imagen_, nombre_gabinete_) {
    try {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "../webservice/WebServiceDocuarchi.asmx/Set_documento_seleccionado",
            data: "{'id_imagen':'" + id_imagen_ + "'," + "'nombre_gabinete':'" + nombre_gabinete_ + "'}",
            dataType: "json",
            success: function (data) {
                //response(data.d);
                if (data.d !== "YES") {
                    alert(data.d);
                    element.checked = false;
                }
            },
            error: function (result) {
                //alert("Error......" + result);
                event.preventDefault();
            }
        });
    }
    catch (err) {
        console.log(err);
        //alert(err.message + " Funcion Set_documento_seleccionado");
    }
}
/**
 * 
 * @param {any} CDParameterAnexosSII
 * @param {any} CDlistaAnexosSII
 */
const ServiceRESTGuardaDocumentoAnexoSII = async (CDParameterAnexosSII, CDlistaAnexosSII) => {
    let serialice = JSON.stringify(CDParameterAnexosSII);
    let serialiceList = JSON.stringify(CDlistaAnexosSII);
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebService_integracion_sii.asmx/SeviceGuardaDocumentoAnexoSII', {
                data: "{'CDParameterAnexosSII':'" + serialice + "','CDlistaAnexosSII':'" + serialiceList +  "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].error_gestion !== "YES") {
                        resolve(data.d[0].error_gestion);
                    } else {
                        insert_row_documento_relacionado(data.d[0].dato_lista, "rad", 1);
                        resolve("YES");
                    }
                }, error: function (xception, textStatus, errorThrown) {

                    if (xception.status === 0) {
                        resolve('Not connect: Verify Network.');

                    } else if (xception.status == 404) {
                        resolve('Requested page not found [404]');

                    } else if (xception.status == 500) {
                        resolve('Internal Server Error [500].' + xception.responseText);

                    } else if (textStatus === 'parsererror') {
                        resolve('Requested JSON parse failed.');

                    } else if (textStatus === 'timeout') {
                        resolve('Time out error.');
                    } else if (textStatus === 'abort') {
                        resolve('Ajax request aborted.');
                    } else {
                        resolve('Uncaught Error: ' + xception.responseText);
                    }
                }
            });
        }
        catch (ex) {
            resolve('Error general ServiceRESTGuardaDocumentoAnexoSII  ' + ex.message);
        }
    })
    let result = await myPromise;
    return result;
}
function preven_event_search(event, e) {
    try {
        document.getElementById("Button_tool_search_lista_tareas").click();
        event.preventDefault();
    }
    catch (err) {
        alert(err.message + " Funcion preven_event_search");
    }
}
function preven_event_restor_search(event, e) {
    try {
        document.getElementById("Button_tool_restore_lista_tareas").click();
        event.preventDefault();
    }
    catch (err) {
        alert(err.message + " Funcion preven_event_search");
    }
}
function preven_event_search_new_task(event, e) {
    try {
        document.getElementById("Button_tool_searh_new_task").click();
        event.preventDefault();
    }
    catch (err) {
        alert(err.message + " Funcion preven_event_search_new_task");
    }
}
function preven_event_search_especial(event, e) {
    try {
        var result = search_valor_campo_form_control("form_control_consul_campos_dat_adit");
        if (result == "YES") {
            for (i = 0; i < ITEM_GENERAL_CONTROL_ARRAY.length; i++) {
                if (ITEM_GENERAL_CONTROL_ARRAY[i].value_campo !== "") {
                    document.getElementById("Hidden_value_search_especial").value = ITEM_GENERAL_CONTROL_ARRAY[i].value_campo;
                    document.getElementById("Button_tool_search_especial").click();
                    event.preventDefault();
                    return true;
                }
            }
            
           
        } else {
            alert(result);
        }
        
    }
    catch (err) {
        alert(err.message + " Funcion preven_event_search_new_task");
    }
}
function preven_event_search_pendientes(event, e) {
    try {
        document.getElementById("Button_tool_consulta_lista_tareas_pendiente").click();
        event.preventDefault();
    }
    catch (err) {
        alert(err.message + " Funcion preven_event_search_pendientes");
    }
}
function preven_event_restor_search_pendientes(event, e) {
    try {
        document.getElementById("Button_tool_restore_lista_tareas_pendiente").click();
        event.preventDefault();
    }
    catch (err) {
        alert(err.message + " Funcion preven_event_search");
    }
}
function preven_event_restor_search_pendientes(event, e) {
    try {
        document.getElementById("Button_tool_restore_lista_tareas_pendiente").click();
        event.preventDefault();
    }
    catch (err) {
        alert(err.message + " Funcion preven_event_search");
    }
}
function prevent_tarea_pendiente(event, element) {
    try {
        var fer = $(element).attr("idd");
        var tip_event = $(element).attr("tip_event");
        if (tip_event == "documentos_tarea_list") {
            $('#Hidden_id_list_id_task').val($(element).attr("id_tarea"));
            document.getElementById("Button_tool_visor_emergente_tareas_pendiente").click();
        }
        if (tip_event == "asig_tarea_pendiente") {
            $('#Hidden_id_list_id_task').val($(element).attr("id_tarea"));
            $('#Hidden_id_list_pent').val($(element).attr("idd"));
            document.getElementById("ButtonAsignar").click();
        }
        event.preventDefault();
        element.focus();
    }
    catch (err) {
        alert(err.message + " Funcion prevent_tarea_pendiente");
    }
}
function prevent_cerrar(event, element) {
    try {
        //Evita el posback del boton
        event.preventDefault();
        dispalyInterfaceEscaner();

    }
    catch (err) {
        alert(err.message + " Funcion prevent_cerrar ");
    }
}
function prevent_event(event, element) {
    try {
        var fer = $(element).attr("idd");
        var tip_event = $(element).attr("tip_event");
        if (tip_event == "eli_nota") {
            var r = confirm("Desea eliminar la nota");
            if (r == false) {
                return false;
            }
            $('#hdnidlista').val(fer);
            event_element_menu("delete_note_workflow", fer);
        }
        if (tip_event == "ver_nota") {
            $('#hdnidlista').val(fer);
            Service_contenido_nota_tarea_workflow(fer, "TextBox_nota");
            
            //document.getElementById("Button_ver_nota").click();
        }
        event.preventDefault();
        element.focus();
    }
    catch (err) {
        alert(err.message + " Funcion prevent_event");
    }
}
function preven_event_search_lista_actividad(event, e) {
    try {
        document.getElementById("Button_tool_busqueda_enviar_actividad").click();
        event.preventDefault();
    }
    catch (err) {
        alert(err.message + " Funcion preven_event_search_keypres_enter_lista_actividad");
    }
}
function prevent_detalle_usuario(event, element) {
    try {
        event.preventDefault();
        var fer = $(element).attr("id");
        $('#Hidden_id_usuario_workflow').val(fer);
        document.getElementById("Button_detalle_enviar_actividad_flujo_trabajo").click();
        element.focus();
    }
    catch (err) {
        alert(err.message + " Funcion prevent_detalle_usuario");
    }
}
function preven_event_restor_search_lista_actividad(event, e) {
    try {
        document.getElementById("Button_tool_restore_busqueda_enviar_actividad").click();
        event.preventDefault();
    }
    catch (err) {
        alert(err.message + " Funcion preven_event_search_keypres_enter_lista_actividad");
    } finally {

    }
}
function preven_event_search_keypres_enter_lista_actividad(e, sender) {
    try {

        tecla = (document.all) ? e.keyCode : e.which;
        if (tecla == 13) {
            document.getElementById("Button_tool_busqueda_enviar_actividad").click();
            e.preventDefault();
        }
    } catch (err) {
        alert(err.message + " funcion preven_event_search_keypres_enter_lista_actividad " + err.message);
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
        //event.preventDefault();
    }
    catch (err) {
        alert(err.message + " Funcion prevent_scrol");
    }
}
function prevent_lista_tareas(event, element) {
    try {
        var fer = $(element).attr("idd");
        var tip_event = $(element).attr("tip_event");
        if (tip_event == "documentos_tarea_list") {
            document.getElementById("Hidden_id_tarea_sel").value = fer;
            document.getElementById("Hidden_tipo_visor").value = "VISOR WORKFLOW";
            document.getElementById("Button_visor_emergente").click();
        }

        if (tip_event == "detalle_radicado_tarea") {
            $('#hiden_seleccion_documento').val(fer);
            document.getElementById("Hidden_id_tarea_sel").value = fer;
            document.getElementById("Button_tool_activa_detalle_radicado").click();

        }
        if (tip_event == "seleccion_tarea_wf") {      
            document.getElementById("Hidden_id_tarea_sel").value = fer;   
            if (document.getElementById("Hidden_id_tarea_sel").value == "-1") {
                alert("Debe selecionar la tarea para asignar ");
            } else {
                dispalyInterfaceEscaner();
                document.getElementById("ButtonSeleccionGrupo").click();
            }
        }
        //event.preventDefault();
        element.focus();
    }
    catch (err) {
        alert(err.message + " Funcion prevent_lista_tareas");
    }
}
function prevent_elimina_adjunto() {
    try {
        var text_content = document.getElementById("hiden_seleccion_documento_wf").value;
        if (text_content != "" && text_content != "-1") {
            spliter = text_content.split("|");
            if (spliter.length > 3) {
                value = reemplazarAcentos(spliter[4]);
            }
        } else {
            alert("Seleccione el doumento de la lista");
            return true;
        }
        confirma_eliminar_documento_relacion("Desea elimnar el documento adjunto del documento (" + value + ") ?", "hiden_seleccion_documento_wf");
        if (document.getElementById("HiddenPROMP").value == 0) {
            document.getElementById("Button_eliminar_documento_adjunto").click();
        }
    }

    catch (err) {
        alert(err.message + " Funcion prevent_elimina_adjunto");
    }
}
//ZONA LOAD FILE
function start_file_save_UploadFile() {
    try {
        var funcion_name = ""; //Nombre de la funcion java que actualiza el elemento
        var evento_adjunta = ""; //Nombre del evento que adunta el documento
        var tipo_adjunta = 0; // Guarda si tipo documento de respueta se adunta formal o libre   1. formal  2.Libre
        var element_html_actuliza = ""; //Guarda el nombre del elemento que se actualiza
        var element_update_panel = ""; //Guarda el nombre del boton que actualiza el update panel
        var id_respuesta = 0; //Guarda el id respuesta
        var estado_relacion = 0; //Determina si el documento sube como relacionado
        var id_tipo_docuental = 0; //Guarda el tipo documental que se envia para guardar el documento
        var estado_adjunto = 0; //Determina si el documento sube como adjunto 
        var element_parent = "";  //Guarda el nombre del modal que contiene el control upload
        var numero_documento_relacionado = 0;
        var element_isert_table = "wf";
        let nombre_tipo_documental = "";
        if (document.getElementById("Hidden_tip_adjunt")) {
            element_isert_table = document.getElementById("Hidden_tip_adjunt").value;
        }
        var imp_load = document.getElementById('file_element_' + CONTEN_NAME_UPLOAD_FILE);
        if (CONTEN_NAME_UPLOAD_FILE == "adjunto_doc_visor") {
            var chek_relacion = document.getElementById("CheckBox_relacionado_radicado_adj");
            var chek_adjunto = document.getElementById("Check_anexo_radicado_adj");
            if (chek_relacion) {
                if (chek_relacion.checked == true) {
                    estado_relacion = 1;
                } else {
                    estado_relacion = 0;
                }
                funcion_name = "insert_row_documento_relacionado";
                evento_adjunta = "GESTION_RESPUESTA";
                element_html_actuliza = "";
            }

            if (chek_adjunto) {
                if (chek_adjunto.checked == true) {
                    estado_adjunto = 1;
                    funcion_name = "actualiza_contador_imagen";
                    evento_adjunta = "GESTION_RESPUESTA";
                    element_html_actuliza = "LabelConteo";

                } else {
                    estado_adjunto = 0;
                }

            }
           element_update_panel = "Button_update_update_adjunto_doc_visor";
           var drow_tipo = document.getElementById("DropDownList_adjunta_documento");
            
            if (drow_tipo.options.length != 0) {
                id_tipo_docuental = drow_tipo.options[drow_tipo.selectedIndex].value;
                nombre_tipo_documental = drow_tipo.options[drow_tipo.selectedIndex].text;
            }
            element_parent = "ModalPopupExtender_sube_documento_adjunto";
            if (element_isert_table == "wf") {
                numero_documento_relacionado = document.getElementById("GridView_list_documento_relacion_wf").rows.length - 1;
            } else {
                numero_documento_relacionado = document.getElementById("GridView_list_documento_relacion").rows.length - 1;
            } 
            star_copy_interval_file_Upload(estado_adjunto, estado_relacion, id_tipo_docuental, funcion_name, element_parent, evento_adjunta,
                numero_documento_relacionado, element_html_actuliza, element_update_panel, id_respuesta, tipo_adjunta, element_isert_table, nombre_tipo_documental,0);
        }
   
    } catch (err) {
        alert(err.mensaje + " function start_file_save_UploadFile")
    }
}
function auto_zise_popup_adjunta_documento_workflow() {
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

        var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_sube_documento_adjunto').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_sube_documento_adjunto').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#Div_contenido_adjunta').css("height", (document.getElementById("modal_content_sube_documento_adjunto").clientHeight - (document.getElementById("Div_cabecera").clientHeight)) + "px");
        //Para los modal que contiene gred
        var elment_heig = document.getElementById("content_option_chek_adjunto_doc_visor").clientHeight + document.getElementById("content_boton_adjunto_doc_visor").clientHeight + document.getElementById("content_pie_title_adjunto_doc_visor").clientHeight + 20;
        $('#conten_file_element_adjunto_doc_visor').css("height", (document.getElementById("Div_contenido_adjunta").clientHeight - elment_heig) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_adjunta_documento_workflow " + err.message);
    }
}
//TERMINA ZONA LOAD FILE
function prevent_greed(event, element) {
    try {
        //Evita el posback del boton
        event.preventDefault();
        // Marca la liena seleccionada
        $('#data_grid tr[id]').css({ "background": "White", "color": "Black" });
        $('#data_grid tr[id]').each(function () {
            $(this).css({ "background-color": "#E7EDF5", "color": "Black" });
        });
        var fer = $(element).attr("id");
        $('#Hidden_id_actividad_flujo').val(fer);
        $('#Hidden_id_flujo_trabjo').val($(element).attr("id_flujo_trabjo"));
        $('#Hidden_id_actividad_destino').val($(element).attr("id_actividad_destino"));
        $('#Hidden_id_usuario_workflow').val($(element).attr("id_usuario_workflow"));
        $('#Hidden_id_conector').val($(element).attr("id_conector"));
        var x;
        var r = confirm("Desea enviar la tarea a la actividad seleccionada del flujo documental");
        if (r == true) {
            document.getElementById("Button_activa_enviar_actividad_flujo_trabajo").click();
        }
        else {

        }
        element.focus();
    }
    catch (err) {
        alert(err.message + " Funcion prevent_greed");
    }
}
//Envio de actividad por flujo de trabajo
function prevent_envio_actividad_flujo(event, element) {
    try {
        var fer = $(element).attr("id");
        var titl = $(element).attr("title");
        $('#Hidden_id_actividad_flujo').val(fer);
        var x;
        var r = confirm(titl);
        if (r == true) {
            document.getElementById("Button_activa_enviar_actividad_flujo_trabajo").click();
        }
        element.focus();
    }
    catch (err) {
        alert(err.message + " Funcion prevent_envio_actividad_flujo");
    }
}
//Envio de actividad por flujo de trabajo anterior back
function prevent_envio_actividad_flujo_anterior(event, element) {
    try {
        
        var fer = $(element).attr("id");
        var titl = $(element).attr("title");
        $('#Hidden_id_actividad_flujo').val(fer);
        var x;
        var r = confirm(titl);
        if (r == true) {
            document.getElementById("Button_activa_enviar_actividad_flujo_trabajo_anterior").click();
        }
        element.focus();
    }
    catch (err) {
        alert(err.message + " Funcion prevent_envio_actividad_flujo");
    }
}
//Envio de actividad por grupo 
function prevent_envio_actividad_tarea(event, element) {
    try {
        event.preventDefault();
        var fer = $(element).attr("id");
        $('#Hidden_id_tarea').val(fer);
        var x;
        var r = confirm("Desea " + element.title + " ?");
        if (r == true) {
            document.getElementById("Button_tool_enviar_actividad").click();
        }
        element.focus();
    }
    catch (err) {
        alert(err.message + " Funcion prevent_envio_actividad");
    }
}
//Envio de actividad por conector
function prevent_envio_ruta_actividad(event, element) {
    try {

        var r = confirm("Desea " + element.title + " ?");
        if (r == true) {
            $('#Hidden_id_actividad_envio').val($(element).attr("id"));
            $('#Hidden_id_actividad_disp_envio').val($(element).attr("idd"));
            document.getElementById("Button_activa_enviar_actividad_ruta").click();
        } else {
            $('#Hidden_id_actividad_disp_envio').val(0);
        }
        element.focus();
        event.preventDefault();
    }
    catch (err) {
        alert(err.message + " Funcion prevent_envio_ruta_actividad");
    }
}
function prevent_detalle(event, element) {
    try {
        //event.preventDefault();
        $('#data_grid tr[id]').css({ "background": "White", "color": "Black" });
        $('#data_grid tr[id]').each(function () {
            $(this).css({ "background-color": "#E7EDF5", "color": "Black" });
        });
        var fer = $(element).attr("id");
        $('#Hidden_id_actividad_flujo').val(fer);
        $('#Hidden_id_flujo_trabjo').val($(element).attr("id_flujo_trabjo"));
        $('#Hidden_id_actividad_destino').val($(element).attr("id_actividad_destino"));
        $('#Hidden_id_usuario_workflow').val($(element).attr("id_usuario_workflow"));
        document.getElementById("Button_detalle_enviar_actividad_flujo_trabajo").click();
        element.focus();
    }
    catch (err) {
        alert(err.message + " Funcion prevent_detalle");
    }
}
function prevent_detalle_actividad(event, element) {
    try {
        event.preventDefault();
        var fer = $(element).attr("id");
        $('#Hidden_id_actividad_destino').val(fer);
        $('#Hidden_id_usuario_workflow').val(0);
        document.getElementById("Button_detalle_enviar_actividad_flujo_trabajo").click();
        element.focus();
    }
    catch (err) {
        alert(err.message + " Funcion prevent_detalle_actividad");
    }
}
function eliminar_fila_data_gred_lista(gred, nombre_hiden) {
    try {
        
        $("#" + gred + " tr[id=" + $("#" + nombre_hiden).val() + "]").remove();
        $('#' + nombre_hiden).val("-1");
       
    }
    catch (err) {
        alert(err.message + " Funcion eliminar_fila_data_gred_lista");
    }

}
function eliminar_fila_data_gred_lista_sin_set(gred, nombre_hiden) {
    try {

        $("#" + gred + " tr[id=" + $("#" + nombre_hiden).val() + "]").remove();
       

    }
    catch (err) {
        alert(err.message + " Funcion eliminar_fila_data_gred_lista");
    }

}
function changue_boton_color(id_value) {
    try {
        var nombre_grid = "GridView2";
        $("#" + nombre_grid + " tr[id=" + id_value + "]").each(function () {
            var elment_row = $(this)[0];
            if (elment_row) {
                elment_row.classList.remove("font-weight-bold");
                elment_row.classList.add("font-weight-light");
            }
            var element_ = $(this)[0].cells[0];
            if (element_) {
                const list = element_.getElementsByClassName("dmt_sel_imput");
                if (list[0]) {
                    list[0].classList.remove("btn-success");
                    list[0].classList.add("btn-warning");
                }             
            }     
        })
       
    } catch (ex) {
        alert("Error funcion changue_boton_color " + ex.mensaje);
    }
}
function changue_boton(id_value) {
    try {
        var nombre_grid = "GridView2";
        $("#" + nombre_grid + " tr[id=" + id_value + "]").each(function () {         
            var element_ = $(this)[0].cells[0];
            if (element_) {
                const list = element_.getElementsByClassName("dmt_sel_imput");
                if (list[0]) {
                    while (list[0].firstChild) {
                        list[0].removeChild(list[0].firstChild);
                    }
                   var ihtml = document.createElement("i");
                    ihtml.style.color = "white";
                    ihtml.classList.add("far");
                    ihtml.classList.add("fa-user");
                    ihtml.classList.add("fa-lg");
                    list[0].appendChild(ihtml);
                }
            }
        })

    } catch (ex) {
        alert("Error funcion changue_boton " + ex.mensaje);
    }
}
function onDataShown(sender, args) {
    sender._popupBehavior._element.style.zIndex = 1000001;

}
function activa_export_lista() {
    try {
        var x = $('#' + 'GridView2' + ' th');
        var txt = "";
        var i;
        for (i = 1; i < x.length; i++) {
            txt = txt + x[i].innerText.toUpperCase() + "|";
        }
        document.getElementById("Hidden_colum_header").value = txt;
        document.getElementById("Button_export_lista_event").click();
        return txt;
    }
    catch (err) {
        alert(err.message + " Funcion activa_export_lista");
    }
}
function detecte_boton_tool_visible() {
    try {
        if (document.getElementById("ImageButtonautoterminar")) {
            $("#ImageButtonautoterminar_").css("display", "block");
            $("#ImageButtonautoterminar").css("display", "none");
        } else {
            $("#ImageButtonautoterminar_").css("display", "none");
        }
    }
    catch (ex) {
        alert("Inconsistencia función java detecte_boton_tool_visible " + ex.message);
    }
}
function AutoSizeListaConstaciasSII() {
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
        let height_header = document.getElementById('header_modal_sii_constancias_inscripcion').clientHeight;
        let height_footer = document.getElementById('footer_modal_sii_constancias_inscripcion').clientHeight;
        espacio_iframe = espacio_iframe - 100;

        $('#content_tabl_lista_sii_constancias_inscripcion').css("height", ((espacio_iframe - (height_header + height_header))) + "px");
        let heig_table = (espacio_iframe - (height_header + height_header));
        table_reize_heigth("tabl_lista_sii_constancias_inscripcion", heig_table, "", "table-borderless");

    } catch (ex) { alert("Funcion AutoSizeListaConstaciasSII " + ex.message); }

}
function AutoSizeListaAnexosSII() {
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
        let height_header = document.getElementById('header_modal_sii_anexos_recibo').clientHeight;
        let height_footer = document.getElementById('footer_modal_sii_anexos_recibo').clientHeight;
        espacio_iframe = espacio_iframe - 100;
        
        $('#content_tabl_lista_sii_anexos_recibo').css("height", ((espacio_iframe - (height_header + height_header))) + "px");
        let heig_table = (espacio_iframe - (height_header + height_header));
        table_reize_heigth("tabl_lista_sii_anexos_recibo", heig_table, "", "table-borderless");

    } catch (ex) { alert("Funcion AutoSizeListaAnexosSII " + ex.message); }

}
//----------Debijua y redimenciona la tabla que muestra la lista de versiones
function auto_zise_popup_consulta_meta_dato() {
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
        var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_interface_consulta_meta_dato').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_consulta_meta_dato').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#contenido_procesa_interface_consulta_meta_dato').css("height", (document.getElementById("modal_content_consulta_meta_dato").clientHeight - (document.getElementById("divcabecer2_interface_consulta_meta_dato").clientHeight)) + "px");
        $('#div_content_tabla').css("height", (document.getElementById("modal_content_consulta_meta_dato").clientHeight - (document.getElementById("divcabecer2_interface_consulta_meta_dato").clientHeight)) + "px");
        $('#table_meta_row').bootstrapTable('resetView', { height: (document.getElementById("contenido_procesa_interface_consulta_meta_dato").clientHeight - 30) });
        
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_consulta_meta_dato " + err.message);
    }
}
function auto_zise_popup_autorizados() {
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
        $('#Panel_lista_autorizacion').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_lista_autorizacion').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#contenido_procesa_lista_autorizacion').css("height", (document.getElementById("modal_content_lista_autorizacion").clientHeight - (document.getElementById("divcabecer_lista_autorizacion").clientHeight + document.getElementById("conter_boton_footer_lista_autorizacion").clientHeight)) + "px");
        //Para los modal que contiene gred
        $('#Panel_lista_autorizacion_2').css("height", (document.getElementById("contenido_procesa_lista_autorizacion").clientHeight - document.getElementById("div_label_lista_autorizacion").clientHeight) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_autorizados " + err.message);
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
function auto_zise_popup_lista_chequeo_edita(value_lista_general) {
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
        var gridwith = document.getElementById("Contenedorgrid_edita").clientWidth;
        var gridheihg = document.getElementById("Contenedorgrid_edita").clientHeight;
        //$('#data_grid_chequeo_actualiza').css("height", (gridheihg - 5) + "px");
        //LLAMA PLUGIN FIJA HIDER O TITULOS   
        if (value_lista_general == "1") {
            if ($('#data_grid_chequeo_actualiza td').children.length > 0 && $('#data_grid_chequeo_actualiza tr:visible').length > 0) {
                $('#data_grid_chequeo_actualiza th').hide();

            }
        }
        document.getElementById("Hidden_0003").value = "-1";
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_lista_chequeo_edita " + err.message);
    }
}
function auto_zise_popup_adjunta_imagen_digitalizada() {
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
        var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_digitaliza_documento_adjunto').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_digitaliza_documento_adjunto').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#contenido_procesa_digitaliza_documento_adjunto').css("height", (document.getElementById("modal_content_digitaliza_documento_adjunto").clientHeight - (document.getElementById("divcabecer2_digitaliza_documento_adjunto").clientHeight )) + "px");
        //Para los modal que contiene gred
        $('#IframeDitaliza_adjunto_').css("height", (document.getElementById("contenido_procesa_digitaliza_documento_adjunto").clientHeight) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_adjunta_imagen_digitalizada " + err.message);
    }
}
//ImageButton_pendiente_aprobacion

//activa auto size workflow desde la pagina webformindice con el boton  Button_red_parent de la pagina web workflow
function auto_zise_popup_detalle_sesion_workflow() {
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
        var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_detalle_sesion').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_detalle_sesion').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#contenido_procesa_detalle_sesion').css("height", (document.getElementById("modal_content_detalle_sesion").clientHeight - (document.getElementById("foter_detalle_sesion").clientHeight + document.getElementById("divcabecer_detalle_sesion").clientHeight)) + "px");
        //Para los modal que contiene gred
        $('#Panel_detalle_session').css("height", (document.getElementById("contenido_procesa_detalle_sesion").clientHeight - 5) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_detalle_sesion_workflow " + err.message);
    }
}
function auto_zise_popup_detalle_tarea_workflow() {
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
        var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_detalle_flujon').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_detalle_flujo').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#contenido_procesa_detalle_flujo').css("height", (document.getElementById("modal_content_detalle_flujo").clientHeight - (document.getElementById("foter_detalle_flujo").clientHeight + document.getElementById("divcabecer_detalle_flujo").clientHeight)) + "px");
        //Para los modal que contiene gred
        $('#Panel_content_detalle_flujo').css("height", (document.getElementById("contenido_procesa_detalle_flujo").clientHeight - 5) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_detalle_tarea_workflow " + err.message);
    }
}
function auto_zise_popup_workflow(estado_lista_tarea) {
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
                //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val();div_buton

            }
        }

        if (parent.document.getElementById("ContentPlacenter_ifrm_ds_")) {
            espacio_iframe = parent.document.getElementById("ContentPlacenter_ifrm_ds_").clientHeight;
        }
        var heig_Menutol = 0;
        if (document.getElementById("Menutol")) {
            heig_Menutol = document.getElementById("Menutol").clientHeight;
        }
        $('#div_content_general_wf').css("height", (espacio_iframe) + "px");
        var he_content = document.getElementById("div_content_general_wf").clientHeight - (document.getElementById("menucab").clientHeight + heig_Menutol);
        if (document.getElementById("contenido_lista_tareas")) {
            $('#contenido_lista_tareas').css("height", (he_content) + "px");
        }
        var heing = 1;
        if (document.getElementById("div_buton")) {
            heing = document.getElementById("div_buton").clientHeight;
        }
        if (document.getElementById("content_selecion_tarea")) {         
            $('#content_selecion_tarea').css("height", (he_content) + "px");
            $('#contenido_imagen').css("height", (he_content - document.getElementById("content_pie_seleccion_tarea").clientHeight) + "px");
            $('#contenido_indice').css("height", (he_content - (document.getElementById("content_pie_seleccion_tarea").clientHeight + heing)) + "px");
            $('#div_conent_indice').css("height", (he_content - (document.getElementById("content_pie_seleccion_tarea").clientHeight + heing)) + "px");
            $('#content_seleccion_documentos').css("height", (he_content - document.getElementById("content_pie_seleccion_tarea").clientHeight) + "px");
            $('#seleccion').css("height", (he_content - (document.getElementById("content_pie_seleccion_tarea").clientHeight + document.getElementById("div_label").clientHeight + document.getElementById("content_boton_gestion").clientHeight)) + "px");
            $('#Panel_scroll').css("height", (he_content - (document.getElementById("content_pie_seleccion_tarea").clientHeight + document.getElementById("div_label").clientHeight + document.getElementById("content_boton_gestion").clientHeight)) + "px");
        }
        if (document.getElementById("contenido_lista_tareas")) {
            $('#contenido_lista_tareas').css("height", (he_content) + "px"); 
            $('#contenedor_tab').css("height", (he_content - (document.getElementById("div_label_title_tareas").clientHeight + document.getElementById("contenido_botonoes").clientHeight)) + "px");
            $('#Panelactividad').css("height", (he_content - (document.getElementById("div_label_title_tareas").clientHeight + document.getElementById("contenido_botonoes").clientHeight)) + "px");
        }
               
        $('#Panel_indice').css("height", (document.getElementById("div_conent_indice").clientHeight - document.getElementById("title_indice").clientHeight) + "px");
        autozize_iframe_visor();
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_workflow " + err.message);
    }
}
function inicia_seleccion_workflow() {
    try {
        if (ESTADO_INICIALIZACION == -4) {
            ESTADO_INICIALIZACION = 0;
            //service_tarea_seleccionada_workflow();
            if (document.getElementById("Hidden_id_tarea_selecionada").value !== "0" && document.getElementById("Hidden_id_tarea_selecionada").value !== "-1") {
                show_area_workflow_seleccion();
            } else {
                hide_area_workflow_seleccion();
            }
        }
    }
    catch (err) {
        alert(err.message + " funcion inicia_seleccion_workflow " + err.message);
    }
}
function show_area_workflow_seleccion() {
    try {
        
        if (document.getElementById("Hidden_id_tarea_selecionada").value !== "0" && document.getElementById("Hidden_id_tarea_selecionada").value !== "-1") {
            document.getElementById("Menutol").style.display = "block";        
        } else {
            document.getElementById("show_selec_tarea").style.display = "none";
        }
        document.getElementById("contenido_lista_tareas").style.display = "none";
        document.getElementById("content_selecion_tarea").style.display = "block";
        if (document.getElementById("Hidden_00020_4001").value == 1) {
            document.getElementById("hide_selec_tarea").style.display = "block";
        } else {
            document.getElementById("hide_selec_tarea").style.display = "none";
        }
        document.getElementById("show_selec_tarea").style.display = "none";
        if (document.getElementById("Panel_EnviaActividad")) {
            document.getElementById("Panel_EnviaActividad").style.display = "block";
        }
       
        if (document.getElementById("Panel_info_tarea")) {
            document.getElementById("Panel_info_tarea").style.display = "block";
        }
        if (document.getElementById("Panel_EnviarUsuario")) {
            document.getElementById("Panel_EnviarUsuario").style.display = "block";
        }
        if (document.getElementById("Panel_devolver_tarea")) {
            document.getElementById("Panel_devolver_tarea").style.display = "block";
        }
        if (document.getElementById("Panel_Buttonanotacion")) {
            document.getElementById("Panel_Buttonanotacion").style.display = "block";
        }
        if (document.getElementById("Panel_autoriza")) {
            document.getElementById("Panel_autoriza").style.display = "block";
        }
        if (document.getElementById("Panel_autoterminar")) {
            document.getElementById("Panel_autoterminar").style.display = "block";
        }    
        if (document.getElementById("Panel_enviar_flujo")) {
            document.getElementById("Panel_enviar_flujo").style.display = "block";
        }
        if (document.getElementById("pendiente_selec_tarea")) {
            document.getElementById("pendiente_selec_tarea").style.display = "block";
        }
        if (document.getElementById("Panel_detalle_tarea")) {
            document.getElementById("Panel_detalle_tarea").style.display = "block";
        }
        if (document.getElementById("Panel_tramitar_tarea")) {
            document.getElementById("Panel_tramitar_tarea").style.display = "block";
        }
        if (document.getElementById("Panel_documentos_tarea")) {
            document.getElementById("Panel_documentos_tarea").style.display = "block";
        }
        auto_zise_popup_workflow("1");
        ini_event_page();
    }
    catch (err) {
        alert(err.message + " funcion show_area_workflow_seleccion " + err.message);
    }
}
function hide_area_workflow_seleccion() {
    try {
        
        if (document.getElementById("Hidden_id_tarea_selecionada").value !== "0" && document.getElementById("Hidden_id_tarea_selecionada").value !== "-1") {
            document.getElementById("Menutol").style.display = "block";
            document.getElementById("show_selec_tarea").style.display = "block";
        } else {
            document.getElementById("show_selec_tarea").style.display = "none";
            document.getElementById("Menutol").style.display = "none";
        }
        document.getElementById("contenido_lista_tareas").style.display = "block";
        document.getElementById("content_selecion_tarea").style.display = "none";
        document.getElementById("hide_selec_tarea").style.display = "none";
        if (document.getElementById("Panel_info_tarea")) {
            document.getElementById("Panel_info_tarea").style.display = "none";
        }
        if (document.getElementById("Panel_enviar_flujo")) {
            document.getElementById("Panel_enviar_flujo").style.display = "none";
        }
        if (document.getElementById("Panel_EnviaActividad")) {
            document.getElementById("Panel_EnviaActividad").style.display = "none";
        }
        if (document.getElementById("Panel_EnviarUsuario")) {
            document.getElementById("Panel_EnviarUsuario").style.display = "none";
        }
        if (document.getElementById("Panel_devolver_tarea")) {
            document.getElementById("Panel_devolver_tarea").style.display = "none";
        }
        if (document.getElementById("Panel_autoriza")) {
            document.getElementById("Panel_autoriza").style.display = "none";
        }
        if (document.getElementById("Panel_Buttonanotacion")) {
            document.getElementById("Panel_Buttonanotacion").style.display = "none";
        }
        if (document.getElementById("Panel_autoterminar")) {
            document.getElementById("Panel_autoterminar").style.display = "none";
        }            
        if (document.getElementById("pendiente_selec_tarea")) {
            document.getElementById("pendiente_selec_tarea").style.display = "none";
        }
        if (document.getElementById("Panel_detalle_tarea")) {
            document.getElementById("Panel_detalle_tarea").style.display = "none";
        }
        if (document.getElementById("Panel_tramitar_tarea")) {
            document.getElementById("Panel_tramitar_tarea").style.display = "none";
        }
        if (document.getElementById("Panel_documentos_tarea")) {
            document.getElementById("Panel_documentos_tarea").style.display = "none";
        }
        auto_zise_popup_workflow("1");
        ini_event_page();
    }
    catch (err) {
        alert(err.message + " funcion hide_area_workflow_seleccion " + err.message);
    }
}
function auto_size_content_anotacion() {
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
        var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#modal_content_anotacion').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#contenido_procesa_content_anotacion').css("height", (document.getElementById("modal_content_anotacion").clientHeight - (document.getElementById("content_boton").clientHeight + document.getElementById("diver_cabcera_content_anotacion").clientHeight)) + "px");
        //Para los modal que contiene gred
        $('#Panel_content_anotacion_gred').css("height", (document.getElementById("contenido_procesa_content_anotacion").clientHeight - 5) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_size_content_anotacion " + err.message);
    }
}
function auto_zise_nota_tarea() {
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
        var heig_porcent = espacio_iframe - ((espacio_iframe * 50) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_nota_respuesta').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_nota_respuesta').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#contenido_procesa_nota_respuesta').css("height", (document.getElementById("modal_content_nota_respuesta").clientHeight - (document.getElementById("divcabecer_nota_respuesta").clientHeight + document.getElementById("content_boton_nota").clientHeight)) + "px");
        //Para los modal que contiene gred
        //$('#TextBox_nota').css("height", (document.getElementById("contenido_procesa_nota_respuesta").clientHeight - 5) + "px");

    }
    catch (err) {
        alert(err.message + " funcion auto_zise_nota_tarea " + err.message);
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



        $('#Panelpagina').css("height", (espacio_iframe - 1) + "px");
        $('#DivBotones').css("height", (document.getElementById("btnCancelpagina").clientHeight + 5) + "px");
        var total = document.getElementById("DivBotones").clientHeight + document.getElementById("Divcab").clientHeight;
        $('#DivColorPagina').css("height", ((espacio_iframe - 1) - (total + 5)) + "px");
        $('#UpdatePanelpagina').css("height", ((espacio_iframe - 1) - (total + 5)) + "px");
        $('#frameeditexpanse_').css("height", ((espacio_iframe - 1) - (total + 5)) + "px");
        
            //Panelpagina
        
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_envia_usuario_grupo " + err.message);
    }
}
function auto_zise_popup_detalle_transacciones() {
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
        var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_transacciones').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_transacciones').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#Cotenedorpendiente_transacciones').css("height", (document.getElementById("modal_content_transacciones").clientHeight - (document.getElementById("Cabecerapendiente_transacciones").clientHeight + 5)) + "px");
        $('#Iframe_transacciones').css("height", (document.getElementById("modal_content_transacciones").clientHeight - (document.getElementById("Cabecerapendiente_transacciones").clientHeight + 5)) + "px");
        //Para los modal que contiene gred
        //$('#content_data_grid').css("height", (document.getElementById("contenido_procesa_usu_rel_solicitud").clientHeight - document.getElementById("contenido_titulo_val_radicacion_documentos").clientHeight) + "px");
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_detalle_transacciones");
    }
}
function auto_zise_popup_detalle_respuesta() {
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
    var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
    $('#Panel_detalle_respuesta').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
    $('#modal_content_detalle_respuesta').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
    //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
    $('#Cotenedorpendiente_detalle_respuesta').css("height", (document.getElementById("modal_content_detalle_respuesta").clientHeight - (document.getElementById("Cabecerapendiente_detalle_respuesta").clientHeight + 5)) + "px");
    $('#Iframe_visor_externo_').css("height", (document.getElementById("modal_content_detalle_respuesta").clientHeight - (document.getElementById("Cabecerapendiente_detalle_respuesta").clientHeight + 5)) + "px");
    //Para los modal que contiene gred
    //$('#content_data_grid').css("height", (document.getElementById("contenido_procesa_usu_rel_solicitud").clientHeight - document.getElementById("contenido_titulo_val_radicacion_documentos").clientHeight) + "px");

}
function auto_zise_popup_detalle_trazabilidad() {
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

    /*$('#Panel_trazabilidad').css("height", (espacio_iframe - 40) + "px");
    $('#Cotenedorpendiente_trazabilidad').css("height", (espacio_iframe - 40) + "px");
    $('#Iframe_trazabilidad_').css("height", (espacio_iframe - 40) + "px");*/

    //corrige el posicionamiento horizontal del modalpopuextender, la propiedad width del panel debe estar en auto
    //var widtth_procent_left_rigth = (with_frame - document.getElementById("Panel_agregar_expediente_carpeta").clientHeight) / 2;
    //$('#Panel_agregar_expediente_carpeta').css("left", (Math.round(widtth_procent_left_rigth)) + "px");
    var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
    $('#Panel_trazabilidad').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
    $('#modal_content_trazabilidad').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
    //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
    $('#Cotenedorpendiente_trazabilidad').css("height", (document.getElementById("modal_content_trazabilidad").clientHeight - (document.getElementById("Cabecerapendiente_trazabilidad").clientHeight + 5)) + "px");
    $('#Iframe_trazabilidad_').css("height", (document.getElementById("modal_content_trazabilidad").clientHeight - (document.getElementById("Cabecerapendiente_trazabilidad").clientHeight + 5)) + "px");
    //Para los modal que contiene gred
    //$('#content_data_grid').css("height", (document.getElementById("contenido_procesa_usu_rel_solicitud").clientHeight - document.getElementById("contenido_titulo_val_radicacion_documentos").clientHeight) + "px");
}
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_detalle_trazabilidad");
}
}
function actuo_zise_popup_compartir_correo_electronico() {
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

        $('#Panel_notifica_gestion').css("height", "60%");
        $('#contenido_procesa_notifica_gestion').css("height", (document.getElementById("Panel_notifica_gestion").clientHeight - 20) + "px");
        $('#Iframe_comparte_coreo').css("height", (document.getElementById("contenido_procesa_notifica_gestion").clientHeight - 5) + "px" );
    }
    catch (ex) {
        alert("Incosistencia general función actuo_zise_popup_compartir_correo_electronico " + ex)
    }
}
function auto_zise_popup_compartir_documento() {
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
    /*$('#Panel_autoriza_compartir_documento').css("height", (espacio_iframe - 30) + "px");
    $('#contenido_procesa_autoriza_compartir_documento').css("height", (espacio_iframe - 30) + "px");
    $('#Iframe_compartir_documento_').css("height", (espacio_iframe - 40) + "px");*/
    //corrige el posicionamiento horizontal del modalpopuextender, la propiedad width del panel debe estar en auto
    //var widtth_procent_left_rigth = (with_frame - document.getElementById("Panel_agregar_expediente_carpeta").clientHeight) / 2;
    //$('#Panel_agregar_expediente_carpeta').css("left", (Math.round(widtth_procent_left_rigth)) + "px");
    var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
    $('#Panel_autoriza_compartir_documento').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
    $('#modal_content_compartir_documento').css("height", (heig_porcent - 2) + "px"); // Asigna altura del contenedor bootstraf
    //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
    $('#contenido_procesa_autoriza_compartir_documento').css("height", (document.getElementById("modal_content_compartir_documento").clientHeight - (document.getElementById("divcabecer2_autoriza_compartir_documento").clientHeight)) + "px");
    //Para los modal que contiene gred
    $('#Iframe_compartir_documento_').css("height", (document.getElementById("contenido_procesa_autoriza_compartir_documento").clientHeight - 5) + "px");

}
function auto_zise_popup_detalle_radicado() {
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
        var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_detalle_radicado').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_detalle_radicado').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#contenido_procesa_detalle_radicado').css("height", (document.getElementById("modal_content_detalle_radicado").clientHeight - (document.getElementById("diver_cabcera_detalle_radicado").clientHeight + document.getElementById("modal-footer_detalle_radicado").clientHeight)) + "px");
        //Para los modal que contiene gred
        $('#Panel_detalle_radicado_user').css("height", (document.getElementById("contenido_procesa_detalle_radicado").clientHeight - 5) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_detalle_radicado " + err.message);
    }
}
function auto_zise_tareas_pendientes() {
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
        var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_tareas_pendientes').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_tareas_pendientes').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#contenido_procesa_tareas_pendientes').css("height", (document.getElementById("modal_content_tareas_pendientes").clientHeight - (document.getElementById("diver_cabcera_tareas_pendientes").clientHeight + document.getElementById("content_boton_tareas_pendientes").clientHeight)) + "px");
        //Para los modal que contiene gred
        $('#content_data_grid_tareas_pendientes').css("height", (document.getElementById("contenido_procesa_tareas_pendientes").clientHeight - (document.getElementById("contenido_titulo_tareas_pendientes").clientHeight + 5)) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_tareas_pendientes " + err.message);
    }
}
function autozize_iframe_visor() {
    try {
        var espacio_iframe = document.getElementById("contenido_imagen").clientHeight;
        var heigconetedor;
        if (document.getElementById("Panel_tolbar_pdf")) {
            heigconetedor = (espacio_iframe - document.getElementById("Panel_tolbar_pdf").clientHeight);
            $('#ifrm_visor_').css("height", (heigconetedor - 10) + "px");
        } else {
            heigconetedor = espacio_iframe;
            $('#ifrm_visor_').css("height", (heigconetedor - 2) + "px");
        }
        if (document.getElementById("panel_content_image_draw")) {
            heigconetedor = (espacio_iframe - document.getElementById("tollimage").clientHeight);
            $('#content').css("height", (heigconetedor - 2) + "px");
        } else {
            $('#content').css("height", (heigconetedor - 2) + "px");
        }
    }
    catch (err) {
        alert(err.message + " Funcion autozize_iframe_visor");
    }
}
function auto_zise_popup_copiar_estructura() {
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
        var heig_porcent = espacio_iframe - ((espacio_iframe * 5) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_copiar_estructura').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_copiar_estructura').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#Cotenedorpendiente_copiar_estructura').css("height", (document.getElementById("modal_content_copiar_estructura").clientHeight - (document.getElementById("Cabecerapendiente_copiar_estructura").clientHeight + 5)) + "px");
        //Para los modal que contiene gred
        $('#Iframe_copiar_estructura_').css("height", (document.getElementById("Cotenedorpendiente_copiar_estructura").clientHeight - 5) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_copiar_estructura " + err.message);
    }
}
function auto_zise_popup_visor_tarea_pendiente() {
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
        var heig_porcent = espacio_iframe - ((espacio_iframe * 5) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_visor_tareas_pendiente').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_visor_tareas_pendiente').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#Cotenedorpendiente_visor_tareas_pendiente').css("height", (document.getElementById("modal_content_visor_tareas_pendiente").clientHeight - (document.getElementById("Cabecerapendiente_visor_tareas_pendiente").clientHeight + 5)) + "px");
        //Para los modal que contiene gred
        $('#Iframe_visor_tareas_pendiente_').css("height", (document.getElementById("Cotenedorpendiente_visor_tareas_pendiente").clientHeight - 5) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_visor_tarea_pendiente " + err.message);
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
        $('#ContenidoImpresion_post').css("height", (document.getElementById("modal_content_Panelimpresionpost").clientHeight - (document.getElementById("divcabecer2_post").clientHeight)) + "px");
        //Para los modal que contiene gred
        $('#ifimpre_post_').css("height", (document.getElementById("ContenidoImpresion_post").clientHeight - 5) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_impresion " + err.message);
    }
}
function auto_zise_popup_modal_conten_procesing_image_worflow(name_content_table, name_content) {
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
        var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_detail_document_proces_workflow').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_detail_document_proces_workflow').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#div_content_trace_grafic').css("height", (document.getElementById("modal_content_detail_document_proces_workflow").clientHeight - (document.getElementById("diver_cabcera_detail_document_proces_workflow").clientHeight + document.getElementById("modal-footer_detail_document_proces_workflow").clientHeight + 5)) + "px");
        $('#' + name_content).css("height", (document.getElementById("modal_content_detail_document_proces_workflow").clientHeight - (document.getElementById("diver_cabcera_detail_document_proces_workflow").clientHeight + document.getElementById("modal-footer_detail_document_proces_workflow").clientHeight + 5)) + "px");
        let heig_table = document.getElementById(name_content).clientHeight - 5;
        table_reize_heigth("table_boot_detail_document", heig_table, "", "table-borderless");
        //let result = resize_table_boot_manager(name_content_table, name_content);
        //if (result !== "YES") {
        //    alert(result);
        //}

    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_modal_conten_procesing_image_worflow");
    }

}
function auto_zise_popup_modal_conten_copy_document_expediente(name_content_table, name_content) {
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
        var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_detail_copy_document_expediente_wf').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_detail_copy_document_expediente_wf').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        //$('#div_content_trace_grafic').css("height", (document.getElementById("modal_content_detail_copy_document_expediente_wf").clientHeight - (document.getElementById("diver_cabcera_detail_document_proces_workflow").clientHeight + document.getElementById("modal-footer_detail_document_proces_workflow").clientHeight + 5)) + "px");
        $('#' + name_content).css("height", (document.getElementById("modal_content_detail_copy_document_expediente_wf").clientHeight - (document.getElementById("diver_cabcera_detail_copy_document_expediente_wf").clientHeight + document.getElementById("modal-footer_detail_copy_document_expediente_wf").clientHeight + 5)) + "px");
        let result = resize_table_boot_manager(name_content_table, name_content);
        if (result !== "YES") {
            alert(result);
        }

    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_modal_conten_copy_document_expediente");
    }

}


function ver_visor() {
    try {
    $("#hide_image_indice").css("display", "block")
    $("#show_image_indice").css("display", "none")
    $("#label").css("width", "96%")
    $("#contenido_seleccion").css("width", "20%")
    $("#contenido_imagen").css("display", "block")
    }
    catch (err) {
        alert(err.message + " Funcion ver_visor");
    }
}
function cambia_estado_boton_reasignar(estado) {
    try {
     if (estado == "VISIBLE") {
        $("#ButtonReasignarTerminar").css("display", "block")
      } else
        {
        $("#ButtonReasignarTerminar").css("display", "none")
       }
    }
    catch (err) {
        alert(err.message + " Funcion cambia_estado_boton_reasignar");
    }
}
//Retorna el idex de una columna en una tabla
function colum_index(colum_name, gred) {
    try {
    var x = $("#" + gred + " th");
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
function solicita_id_tarea_workflow(gred) {
    try {
        
        $("#" + gred + " tr[id=" + $("#HiddenSeleccion").val() + "]").each(function () {
            document.getElementById("Hidden_id_tarea_sel").value = -1;
            var idex = -1;
            var idex = colum_index('id_tarea', gred);
            if (idex != -1) {
                document.getElementById("Hidden_id_tarea_sel").value = $(this)[0].cells[idex].innerText;
                
            }
            
        })
    }
    catch (err) {
        alert(err.message + " Funcion solicita_id_tarea_workflow");
    }
}
function solicita_radicado(gred) {
    try {

        $("#" + gred + " tr[id=" + $("#HiddenSeleccion").val() + "]").each(function () {
            document.getElementById("Hidden_radicado_seleccion").value = -1;
            var idex = -1;
            var idex = colum_index('RADICADO', gred);
            if (idex != -1) {
                document.getElementById("Hidden_radicado_seleccion").value = $(this)[0].cells[idex].innerText;
            }
            if (idex == -1) {
                idex = colum_index('DATOS_RECIBO', gred);
                if (idex != -1) {
                    document.getElementById("Hidden_radicado_seleccion").value = $(this)[0].cells[idex].innerText;
                }
            }
            

        })
    }
    catch (err) {
        alert(err.message + " Funcion solicita_radicado");
    }
}

function onclick(selcion_buton) {
    var boton=document.getElementById(selcion_buton)
    if (boton) {
        boton.click();
    }
    
}
function activa_boton_cerrar() {
    document.getElementById("Button_valida_Cerrar_respuesta_radicado").click();
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
        document.getElementById("Hidden_filtro_gred").value = $("#" + contenido_busqueda).val().toLowerCase();
        var grid = $("#" + data_grid);
        //$("#" + data_grid + " tr:hidden").show();
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
              //$("#" + data_grid + " tr:hidden").show();
            }
        });
    }
    catch (err) {
        alert(err.message + " funcion filtro_gred " + err.message);
    }
}
function busca_campo_wf_seleccion(nombre_grid, id_, nombre_campo) {
    try {
        var valor_retorn = "";
        $("#" + nombre_grid + " tr[id_wf=" + id_ + "]").each(function () {
            var idex = -1;
            var name = nombre_campo;
            var d = id_;
            idex = colum_index_(name, nombre_grid);
            if (idex != -1) {
                var k = $(this)[0].cells[idex];
                var valor = $(this)[0].cells[idex].innerText;
                if (valor == "") {
                    $(this)[0].cells[idex].innerHTML = valor;
                }
                valor_retorn = valor;
            } else {
                valor_retorn = "";
            }
        })
        return valor_retorn;
    }
    catch (err) {
        alert(err.message + " Funcion busca_campo_wf_seleccion");
    }
}
function busca_campo_rad_seleccion(nombre_grid, id_, nombre_campo) {
    try {
        var valor_retorn = "";
        $("#" + nombre_grid + " tr[id_rad=" + id_ + "]").each(function () {
            var idex = -1;
            var name = nombre_campo;
            var d = id_;
            idex = colum_index_(name, nombre_grid);
            if (idex != -1) {
                var k = $(this)[0].cells[idex];
                var valor = $(this)[0].cells[idex].innerText;
                if (valor == "") {
                    $(this)[0].cells[idex].innerHTML = valor;
                }
                valor_retorn = valor;
            } else {
                valor_retorn = "";
            }
        })
        return valor_retorn;
    }
    catch (err) {
        alert(err.message + " Funcion  busca_campo_rad_seleccion");
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
    finally {
        document.getElementById(contenido_busqueda).focus();
    }

}

//MUEVE EL SCCROL AL ID SELECCIONADO
function mueve_scroll_data_gred(data_grid, HiddenSeleccion) {
    try {
        if ($("#" + data_grid + " td").children.length == 0 && $("#" + data_grid + " tr:visible").length == 0) {
            return true;
        }
        var p = document.getElementById(HiddenSeleccion).value;
        var k = document.getElementById(data_grid);
        if (p != "-1" && p != "0") {           
            var scrollableDiv = $("#" + data_grid).parent();
            var index = $("#" + data_grid + " tr[id_sel= " + p + "]");
           
            //limpia todos los seleccionados
            //$("#" + data_grid + " tr[id_sel]").css({ "background-color": "transparent", "color": "Black" });
            $("#" + data_grid + " tr[id_sel=" + p + "]").css({"background-color": "#E7EDF5", "color": "Black" });
            $("#" + data_grid + " tr[id_sel= " + p + "]").each(function () {
                if (index[0].rowIndex > 1) {
                    //$(scrollableDiv).scrollTop(70);
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
function ConfirmMensajeGeneral_dos(mensaje, name_hiden) {
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
    function ConfirmMensaje(mensaje) {
        try {
            var x;
            var r = confirm(mensaje);
            if (r == true) {
                x = "0";
            }
            else {
                x = "1";
            }
            document.getElementById("HiddenPROMP").value = x;
        }
        catch (err) {
            alert(err.message + " Funcion ConfirmMensaje");
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
    function eliminar_fila_data_gred(gred) {
        try {
            $("#" + gred + " tr[id=" + $("#HiddenSeleccion").val() + "]").remove();
            $('#HiddenSeleccion').val("-1");
      
        }
        catch (err) {
            alert(err.message + " Funcion eliminar_fila_data_gred");
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
            $("#progres_bar").css("heigth",  "100%");
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
    function dispalyVisorEmergente() {
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
            document.getElementById("div_cerrar").style.display = "block";
            document.getElementById('Are_Digitalizacion').style.display = 'none';
            document.getElementById('Are_Digitalizacion').style.width = "0%";
            document.getElementById('Are_Digitalizacion').style.height = "0%";
            document.getElementById('Area_Visor').style.display = 'block';
            document.getElementById('Area_Visor').style.width = (with_frame - document.getElementById("Area_Enlace").clientWidth) - 5 + "px";
            document.getElementById('Area_Visor').style.height = ((espacio_iframe - document.getElementById("div_cerrar").clientHeight) - 25) + "px";        
        }
        catch (err) {
            alert(err.message + " Funcion dispalyVisorEmergente");
        }
    }
    
   
    function dispalyInterfaceEscaner() {
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
            document.getElementById("div_cerrar").style.display = "none";
            document.getElementById('Are_Digitalizacion').style.width = (with_frame - document.getElementById("Area_Enlace").clientWidth) - 5 + "px";
            document.getElementById('Are_Digitalizacion').style.height = "100%";
            document.getElementById('Are_Digitalizacion').style.display = 'block';
            document.getElementById('Area_Visor').style.display = 'none';
            document.getElementById('Area_Visor').style.width = "0%";
            document.getElementById('Area_Visor').style.height = "0%";
           
        }
        catch (err) {
            alert(err.message + " Funcion dispalyInterfaceEscaner");
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
            $('#Panelpendientes').css("height", (espacio_iframe - 40) + "px");
            $('#Cotenedorpendiente').css("height", (espacio_iframe - 40) + "px");
            $('#Iframependiente_').css("height", (espacio_iframe - 40) + "px");
        }
        catch (err) {
            alert(err.message + " Funcion auto_zise_popup_pendinetes");
        }
    }
    function auto_zise_popup_ventana_externa() {
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
            var heigt;
            $('#Panelpagina').css("height", (450) + "px");
            heigt = (450) - (((450) * 10) / 100);
            $('#Contenidopagina').css("height", heigt + "px");
            $('#frameeditexpanse_').css("height", heigt + "px");
            heigt = (450) - (((450) * 90) / 100);
            $('#DivBotones').css("height", heigt + "px");
        }
        catch (err) {
            alert(err.message + " Funcion auto_zise_popup_ventana_externa");
        }
  }
    function auto_zise_popup_trace_grafic() {
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
            var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
            $('#Paneltraza_grafica').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
            $('#modal_content_grafica').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
            //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
            $('#div_content_trace_grafic').css("height", (document.getElementById("modal_content_grafica").clientHeight - (document.getElementById("div_trace_grafic").clientHeight + 5)) + "px");
            $('#Iframetraza_grafica_').css("height", (document.getElementById("modal_content_grafica").clientHeight - (document.getElementById("div_trace_grafic").clientHeight + 5)) + "px");
            //Para los modal que contiene gred
            //$('#content_data_grid').css("height", (document.getElementById("contenido_procesa_usu_rel_solicitud").clientHeight - document.getElementById("contenido_titulo_val_radicacion_documentos").clientHeight) + "px");
        }
        catch (err) {
            alert(err.message + " Funcion auto_zise_popup_trace_grafic");
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

            $('#PanelLibre').css("height", (espacio_iframe) + "px");
            $('#Div9').css("height", (espacio_iframe) + "px");
            $('#Iframelibre_').css("height", (espacio_iframe) + "px");
        }
        catch (err) {
            alert(err.message + " Funcion auto_zise_popup_paginas_externas_libres");
        }

    }
    function auto_zise_popup_recuperar_tarea() {
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
            var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
            $('#Panelrecuperar').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
            $('#modal_content_recuperar').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
            //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
            $('#ContendorRecuperar').css("height", (document.getElementById("modal_content_recuperar").clientHeight - (document.getElementById("Cabecerarecuperar").clientHeight + 5)) + "px");
            $('#IframeRecuperar_').css("height", (document.getElementById("modal_content_recuperar").clientHeight - (document.getElementById("Cabecerarecuperar").clientHeight + 10)) + "px");
            //Para los modal que contiene gred
            //$('#content_data_grid').css("height", (document.getElementById("contenido_procesa_usu_rel_solicitud").clientHeight - document.getElementById("contenido_titulo_val_radicacion_documentos").clientHeight) + "px");
        }
        catch (err) {
            alert(err.message + " Funcion auto_zise_popup_recuperar_tarea");
        }

    }
    function auto_zise_popup_respuesta() {
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
            //$('#Iframe_respuesta_radicado_').css("height", (espacio_iframe - 60) + "px");
            //corrige el posicionamiento horizontal del modalpopuextender, la propiedad width del panel debe estar en auto
            //var widtth_procent_left_rigth = (with_frame - document.getElementById("Panel_agregar_expediente_carpeta").clientHeight) / 2;
            //$('#Panel_agregar_expediente_carpeta').css("left", (Math.round(widtth_procent_left_rigth)) + "px");
            var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
            $('#Panel_respuesta_radicado').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
            $('#modal_content_respuesta_radicado').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
            //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
            $('#contenido_procesa_respuesta_radicado').css("height", (document.getElementById("modal_content_respuesta_radicado").clientHeight - (document.getElementById("Cabecera_respuesta_radicado").clientHeight + 5)) + "px");
            $('#Iframe_respuesta_radicado_').css("height", (document.getElementById("modal_content_respuesta_radicado").clientHeight - (document.getElementById("Cabecera_respuesta_radicado").clientHeight + 5)) + "px");
            //Para los modal que contiene gred
            //$('#content_data_grid').css("height", (document.getElementById("contenido_procesa_usu_rel_solicitud").clientHeight - document.getElementById("contenido_titulo_val_radicacion_documentos").clientHeight) + "px");

        }
        catch (err) {
            alert(err.message + " Funcion auto_zise_popup_respuesta");
        }

    }
    function auto_zise_popup_detalle_grupo_usuario() {
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
            //$('#Iframe_respuesta_radicado_').css("height", (espacio_iframe - 60) + "px");
            //corrige el posicionamiento horizontal del modalpopuextender, la propiedad width del panel debe estar en auto
            //var widtth_procent_left_rigth = (with_frame - document.getElementById("Panel_agregar_expediente_carpeta").clientHeight) / 2;
            //$('#Panel_agregar_expediente_carpeta').css("left", (Math.round(widtth_procent_left_rigth)) + "px");
            var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
            $('#Panel_detalle_actividad_flujo').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
            $('#modal_content_actividad_flujo').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
            //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
            $('#contenido_procesa_detalle_actividad_flujo').css("height", (document.getElementById("modal_content_actividad_flujo").clientHeight - (document.getElementById("divcabecer2_detalle_actividad_flujo").clientHeight + 5)) + "px");
            //$('#Iframe_respuesta_radicado_').css("height", (document.getElementById("modal_content_respuesta_radicado").clientHeight - (document.getElementById("Cabecera_respuesta_radicado").clientHeight + 5)) + "px");
            //Para los modal que contiene gred
            //$('#content_data_grid').css("height", (document.getElementById("contenido_procesa_usu_rel_solicitud").clientHeight - document.getElementById("contenido_titulo_val_radicacion_documentos").clientHeight) + "px");

        }
        catch (err) {
            alert(err.message + " Funcion auto_zise_popup_detalle_grupo_usuario");
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

            var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
            $('#Panel_visor_externo').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
            $('#modal_content_visor_externo').css("height", (heig_porcent - 5) + "px"); // Asigna altura del contenedor bootstraf
            //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
            $('#Cotenedor_visor_externo').css("height", (document.getElementById("modal_content_visor_externo").clientHeight - (document.getElementById("Cabecerapendiente_visor_externo").clientHeight + 5)) + "px");
            //Para los modal que contiene gred
            $('#Iframe_visor_externo_').css("height", (document.getElementById("Cotenedor_visor_externo").clientHeight - 3) + "px");
        }
        catch (err) {
            alert(err.message + " Funcion auto_zise_popup_visor_externo");
        }
    }
    function auto_zise_popup_estado_paginacion() {
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
            $('#Panel_estado_paginacion').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
            $('#modal_estado_paginacion').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
            //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
            $('#contenido_procesa_estado_paginacion').css("height", (document.getElementById("modal_estado_paginacion").clientHeight - (document.getElementById("divcabecer2_estado_paginacion_").clientHeight + document.getElementById("contenido_buton_estado_paginacion").clientHeight)) + "px");
            //Para los modal que contiene gred
            //$('#content_data_grid').css("height", (document.getElementById("contenido_procesa_usu_rel_solicitud").clientHeight - document.getElementById("contenido_titulo_val_radicacion_documentos").clientHeight) + "px");
        }
        catch (err) {
            alert(err.message + " funcion auto_zise_popup_estado_paginacion " + err.message);
        }
    }
    function actualiza_treview_seleccion() {
        try {          
            var nodo = $("#" + "TreeViewseleccion" + " td: a");
            for (var index = 0; index < nodo.length; index ++) {
                var i = nodo[index].className;
                if (document.getElementById("hidden_selecion_documento_treview").value == nodo[index].title && nodo[index].className == "node_select_ LeafNodeStyle_2") {
                    //nodo[index].style.color = "Red";
                    if (document.getElementById("hidden_selecion_actualiza_treview").value !== "") {
                        nodo[index].innerText = document.getElementById("hidden_selecion_actualiza_treview").value;
                    } else {
                        nodo[index].innerText = "Documento(" + (index -1 ) + ")";
                    }
                    
                    return;
                }
            }

        }
        catch (err) {
            alert(err.message + " Funcion actualiza_treview_seleccion");
        }
    }
    function actualiza_treview_seleccion_escaner() {
        try {
            var nodo = $("#" + "TreeViewseleccion_digitalizado" + " td: a");
            for (var index = 0; index < nodo.length; index++) {
                var i = nodo[index].className;
                if (nodo[index].className == "node_select_ LeafNodeStyle_2") {
                    //nodo[index].style.color = "Red";
                    if (document.getElementById("hidden_selecion_actualiza_treview_digitalizacion").value !== "") {
                        nodo[index].innerText = document.getElementById("hidden_selecion_actualiza_treview_digitalizacion").value;
                    } else {
                        nodo[index].innerText = "Documento(" + (index - 1) + ")";
                    }

                    return;
                }
            }

        }
        catch (err) {
            alert(err.message + " Funcion actualiza_treview_seleccion");
        }
    }
    function auto_zise_popup_documentos_radicados_relacionados() {
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
            var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
            $('#Panel_admon_documentos').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
            $('#modal_content_admon_documentos').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
            //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
            $('#contenido_procesa_admon_documentos').css("height", ($("#modal_content_admon_documentos").height() - (document.getElementById("divcabecer2_admon_documentos").clientHeight + 60)) + "px");
            //Para los modal que contiene gred
            //$('#content_data_grid').css("height", (document.getElementById("contenido_procesa_usu_rel_solicitud").clientHeight - document.getElementById("contenido_titulo_val_radicacion_documentos").clientHeight) + "px");
            $("#conte_waper").css("height", (($("#contenido_procesa_admon_documentos").height() - 60)) + "px");
            $("#da_content_wraper_").css("height", (($("#contenido_procesa_admon_documentos").height() - 60)) + "px");
            $("#Contenedorderecho_").css("height", (($("#contenido_procesa_admon_documentos").height() - 60)) + "px");
            $("#Contentizquierdo_").css("height", (($("#contenido_procesa_admon_documentos").height() - 60)) + "px");
            $("#div_treview_archivo").css("height", ($("#Contentizquierdo_").height() - 30) + "px");
            $("#Paneltreview").css("height", ($("#Contentizquierdo_").height() - 50) + "px");
            document.getElementById('Area_Visor').style.height = ((document.getElementById("Contenedorderecho_").clientHeight - document.getElementById("div_cerrar").clientHeight)) + "px";
        }
        catch (err) {
            alert(err.message + " funcion auto_zise_popup_documentos_radicados_relacionados " + err.message);
        }
    }
    function dispalyInterfaceEscaner() {
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
            //document.getElementById('Are_Digitalizacion').style.width = (document.getElementById("Contenedorderecho").clientWidth - 5) + "px";
            document.getElementById('Are_Digitalizacion').style.display = 'block';
            document.getElementById('Area_Visor').style.display = 'none';
        }
        catch (err) {
            alert(err.message + " Funcion dispalyInterfaceEscaner");
        }

    }
    function dispalyVisorEmergente() {
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
            document.getElementById('Are_Digitalizacion').style.display = 'none';
            document.getElementById('Area_Visor').style.display = 'block';
            document.getElementById('Area_Visor').style.height = ((document.getElementById("Contenedorderecho_").clientHeight - document.getElementById("div_cerrar").clientHeight)) + "px";

        }
        catch (err) {
            alert(err.message + " Funcion dispalyVisorEmergente");
        }
    }
    function selecciona_treview_seleccion() {
        try {

            $("#" + "TreeViewseleccion" + " td: a").each(function () {
                $(this).css("color", "Black");
            })
            var nodo = $("#" + "TreeViewseleccion" + " td: a");
            for (var index = 0; index < nodo.length; index++) {
                if (document.getElementById("hidden_selecion_documento_treview").value == nodo[index].title) {
                    nodo[index].style.color = "Red";
                    return;
                }
            }
           
        }
        catch (err) {
            alert(err.message + " Funcion selecciona_treview_seleccion");
        }
    }   
    function retorna_treview_seleccion(datos, node) {
        //Remplaza los nuevos datos en el treview
        try {
          
            $("#" + "TreeViewseleccion" + " td: a").each(function () {
                $(this).css("color",  "Black");
            })
            if (node) {
                node.style.color = "Red";
                document.getElementById("hidden_selecion_documento_treview").value = node.title;
            }
        }
        catch (err) {
            alert(err.message + " Funcion RemplazarDatos_solicitudes_usuario");
        }
    }
    function auto_zise_popup_lista_usuario_flujo() {
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
            // Modal de envío de tarea: conserva una altura equivalente al 50% de la ventana disponible.
            var heig_porcent = Math.round(espacio_iframe * 0.5);
            $('#Panel_lista_actividades_worflow_ruta').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
            $('#modal_content_lista_actividades_worflow_ruta').css("height", (heig_porcent - 3) + "px"); // Asigna altura del contenedor bootstraf
            //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
            $('#contenido_procesa_lista_actividades_workflow').css("height", (document.getElementById("modal_content_lista_actividades_worflow_ruta").clientHeight - (document.getElementById("divcabecer2_lista_actividades_worflow_ruta").clientHeight)) + "px");
            //Para los modal que contiene gred
            $('#div_gred').css("height", (document.getElementById("contenido_procesa_lista_actividades_workflow").clientHeight - (document.getElementById("contenido_titulo_data_grid_dos_title").clientHeight + document.getElementById("div_contenido_procesa_lista_actividades_worflow_ruta_botones_desicion").clientHeight)) + "px");
        }
        catch (err) {
            alert(err.message + " funcion auto_zise_popup_lista_usuario_flujo " + err.message);
        }
    }
    function auto_zise_popup_lista_actividades_ruta() {
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
            var heig_porcent = espacio_iframe - ((espacio_iframe * 2) / 100);  // Indica el porcentaje de espacio vertical del elemento
            $('#Panel_lista_actividades_ruta').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
            $('#modal_content_lista_actividades_ruta').css("height", (heig_porcent - 3) + "px"); // Asigna altura del contenedor bootstraf
            //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
            $('#contenido_procesa_lista_actividades_ruta').css("height", (document.getElementById("modal_content_lista_actividades_ruta").clientHeight - (document.getElementById("divcabecer2_lista_actividades_ruta").clientHeight)) + "px");
            //Para los modal que contiene gred
            $('#div_gred_actividades').css("height", (document.getElementById("contenido_procesa_lista_actividades_ruta").clientHeight - (document.getElementById("contenido_titulo_data_grid_lista_actividades_ruta").clientHeight + document.getElementById("div_contenido_procesa_lista_actividades_ruta_botones_desicion").clientHeight)) + "px");
        }
        catch (err) {
            alert(err.message + " funcion auto_zise_popup_lista_actividades_ruta " + err.message);
        }
    }
    function auto_zise_popup_list_imagenes_sii() {
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
            $('#Panel_list_imagenes_sii').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
            $('#modal_content_list_imagenes_sii').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
            //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
            $('#contenido_procesa_list_imagenes_sii').css("height", (document.getElementById("modal_content_list_imagenes_sii").clientHeight - (document.getElementById("diver_cabcera_list_imagenes_sii").clientHeight)) + "px");
            //Para los modal que contiene gred
            $('#content_data_grid_list_imagenes_sii').css("height", (document.getElementById("contenido_procesa_list_imagenes_sii").clientHeight - (document.getElementById("contenido_titulo_list_imagenes_sii").clientHeight + 40)) + "px");
        }
        catch (err) {
            alert(err.message + " funcion auto_zise_popup_list_imagenes_sii " + err.message);
        }
    }
    function auto_zise_popup_list_inscripciones_sii() {
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
        $('#Panel_list_inscripciones_sii').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_list_inscripciones_sii').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#contenido_procesa_list_inscripciones_sii').css("height", (document.getElementById("modal_content_list_inscripciones_sii").clientHeight - (document.getElementById("diver_cabcera_list_inscripciones_sii").clientHeight + document.getElementById("modal-footer_list_inscripciones_sii_").clientHeight)) + "px");
        //Para los modal que contiene gred
        $('#content_data_grid_list_inscripciones_sii').css("height", (document.getElementById("contenido_procesa_list_inscripciones_sii").clientHeight - (document.getElementById("contenido_titulo_list_inscripciones_sii").clientHeight + 20)) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_list_inscripciones_sii " + err.message);
    }
}
    function confirma_eliminar_documento_relacion(mensaje, hiden_sel) {
        try {
            if (document.getElementById(hiden_sel).value == "") {
                document.getElementById("HiddenPROMP").value = 1;
                return false;
            }
            var x = 1;
            var r = confirm(mensaje);
            if (r == true) {
                x = "0";
            }
            else {
                x = "1";
            }
            document.getElementById("HiddenPROMP").value = x;
        } catch (err) {
            alert(err.message + " Funcion confirma_eliminar_documento_relacion");
        }
    }
    var reemplazarAcentos = function (cadena) {
        var chars = {
            "á": "a", "é": "e", "í": "i", "ó": "o", "ú": "u",
            "à": "a", "è": "e", "ì": "i", "ò": "o", "ù": "u", "ñ": "n",
            "Á": "A", "É": "E", "Í": "I", "Ó": "O", "Ú": "U",
            "À": "A", "È": "E", "Ì": "I", "Ò": "O", "Ù": "U", "Ñ": "N"
        }
        var expr = /[áàéèíìóòúùñ]/ig;
        var res = cadena.replace(expr, function (e) { return chars[e] });
        return res;
    }
    
const eliminar_fila_data_gred_nota =(id_nota, name_table)=> {
    try {
        $("#" + name_table + " tr[id=" + id_nota + "]").remove();
        $('#hdnidlista').val("-1");
        return "YES";
    }
    catch (err) {
        return "Funcion eliminar_fila_data_gred_nota error : " + err.mensaje;
    }

}
//Inserta row lista anotaciones
const insert_row_list_anotation = (array_date, data_table) => {
    try {
        var element_table = document.getElementById(data_table);
        var element_row;
        var element_td;
        var index_tr_title = -1;
        for (i = 0; i < element_table.rows.length; i++) {
            if (element_table.rows[i].className == "GridviewScrollHeader_line_boot") {
                index_tr_title = i + 1;
            }
        }
        element_row = element_table.insertRow(index_tr_title);
        //Agrega los atributos del row
        var conta_td = 0;
        element_td = element_row.insertCell(conta_td);
        element_row.setAttribute("id", array_date.d[0].detailt_note.id_anotacion);
        element_row.style.cursor = "pointer";
        element_row.style.background = "#e8e8f7";
        element_row.style.color = "black";
        //Agrega el boton de ver anotacion
        var divhtml = document.createElement("div");
        var ihtml = document.createElement("i");
        ihtml.style.color = "white";
        ihtml.classList.add("fas");
        ihtml.classList.add("fa-sticky-note");
        var ahtml = document.createElement("a");
        ahtml.classList.add("btn");
        ahtml.classList.add("btn-success");
        ahtml.classList.add("btn-sm");
        ahtml.setAttribute("onclick", "prevent_event(event,this);");
        ahtml.setAttribute("title", "nota");
        ahtml.setAttribute("idd", array_date.d[0].detailt_note.id_anotacion);
        ahtml.setAttribute("tip_event", "ver_nota");
        ahtml.style.marginLeft = "3px";
        ahtml.appendChild(ihtml);
        divhtml.appendChild(ahtml);
        //Agrega boton eliminar nota
        ihtml = document.createElement("i");
        ihtml.style.color = "white";
        ihtml.classList.add("far");
        ihtml.classList.add("fa-trash-alt");
        ihtml.classList.add("fa-lg");
        ahtml = document.createElement("a");
        ahtml.classList.add("btn");
        ahtml.classList.add("btn-danger");
        ahtml.classList.add("btn-sm");
        ahtml.setAttribute("onclick", "prevent_event(event,this);");
        ahtml.setAttribute("title", "Eliminar nota");
        ahtml.setAttribute("idd", array_date.d[0].detailt_note.id_anotacion);
        ahtml.setAttribute("tip_event", "eli_nota");
        ahtml.style.marginLeft = "3px";
        ahtml.appendChild(ihtml);
        divhtml.appendChild(ahtml);
        divhtml.style.display = "inline-flex";
        element_td.appendChild(divhtml);
        conta_td++;
        element_td = element_row.insertCell(conta_td);
        element_td.innerHTML = array_date.d[0].detailt_note.nombre_usuario;
        element_td.classList.add("GridviewScrollItem_line_cort_tr_flex");
        element_td.setAttribute("onclick", "prevent_scrol(event,this,'')");
        conta_td++;
        element_td = element_row.insertCell(conta_td);
        element_td.innerHTML = array_date.d[0].detailt_note.loguin_usuario;
        element_td.classList.add("GridviewScrollItem_line_cort_tr_flex");
        element_td.setAttribute("onclick", "prevent_scrol(event,this,'')");
        conta_td++;
        element_td = element_row.insertCell(conta_td);
        element_td.innerHTML = array_date.d[0].detailt_note.dato_anotacion;
        element_td.classList.add("GridviewScrollItem_line_cort_tr_flex");
        element_td.setAttribute("onclick", "prevent_scrol(event,this,'')");
        conta_td++;
        element_td = element_row.insertCell(conta_td);
        element_td.innerHTML = array_date.d[0].detailt_note.fecha_anotacion;
        element_td.classList.add("GridviewScrollItem_line_cort_tr_flex");
        element_td.setAttribute("onclick", "prevent_scrol(event,this,'')");
        return "YES";
    }
    catch (ex) {
        return "function insert_row_list_anotation error " + ex.mensaje;
    }
}
 //Inserta row lista workflow
function insert_row_lista_workflow(id_tarea, data_table, data_campos) {
    try {
        var element_table = document.getElementById(data_table);
        var element_row;
        var element_td;
        var split = data_campos.split("|");
        var index_tr_title = -1;
        for (i = 0; i < element_table.rows.length; i++) {
            if (element_table.rows[i].className == "GridviewScrollHeader_line_boot") {
                index_tr_title = i + 1;
            }   
        }
        element_row = element_table.insertRow(index_tr_title);     
        $('#' + data_table + ' tr[id]').css({ "background-color": "White", "color": "Black" });
        document.getElementById("Hidden_id_tarea_sel").value = id_tarea;      
        //Agrega los atributos del row
        var conta_td = 0;
        element_td = element_row.insertCell(conta_td);
        element_row.setAttribute("id", id_tarea);
        element_row.style.cursor = "pointer";
        element_row.style.background = "#e8e8f7";
        element_row.style.color = "black";
        //Agrega el boton de visualizar documentos
        var divhtml = document.createElement("div");
        var ihtml = document.createElement("i");
        ihtml.style.color = "white";
        ihtml.classList.add("fas");
        ihtml.classList.add("fa-folder-open");
        ihtml.classList.add("fa-lg");
        var ahtml = document.createElement("a");
        ahtml.classList.add("btn");
        ahtml.classList.add("btn-primary");
        ahtml.classList.add("btn-sm");
        ahtml.setAttribute("onclick", "prevent_lista_tareas(Event,this);");
        ahtml.setAttribute("title", "Ver documentos");
        ahtml.setAttribute("idd", id_tarea);
        ahtml.setAttribute("tip_event", "documentos_tarea_list");
        ahtml.style.marginLeft = "3px";
        ahtml.appendChild(ihtml);
        divhtml.appendChild(ahtml);
        //Agrega el boton del indice del radicado
        //if (split[4] == 1) {
        ihtml = document.createElement("i");
        ihtml.style.color = "white";
        ihtml.classList.add("far");
        ihtml.classList.add("fa-info");
        ihtml.classList.add("fa-lg");
        ahtml = document.createElement("a");
        ahtml.classList.add("btn");
        ahtml.classList.add("btn-info");
        ahtml.classList.add("btn-sm");
        ahtml.setAttribute("onclick", "prevent_lista_tareas(Event,this);");
        ahtml.setAttribute("title", "Detalle tarea");
        ahtml.setAttribute("idd", id_tarea);
        ahtml.setAttribute("tip_event", "detalle_radicado_tarea");
        ahtml.style.marginLeft = "3px";
        ahtml.appendChild(ihtml);
        divhtml.appendChild(ahtml);//}
        //Agrega boton asignar tarea
        ihtml = document.createElement("i");
        ihtml.style.color = "white";
        ihtml.classList.add("far");
        ihtml.classList.add("fa-user");
        ihtml.classList.add("fa-lg");
        ahtml = document.createElement("a");
        ahtml.classList.add("btn");
        ahtml.classList.add("btn-warning");
        ahtml.classList.add("btn-sm");
        ahtml.setAttribute("onclick", "prevent_lista_tareas(Event,this);");
        ahtml.setAttribute("title", "Retomar tarea de usario en proceso");
        ahtml.setAttribute("idd", id_tarea);
        ahtml.setAttribute("tip_event", "seleccion_tarea_wf");
        ahtml.style.marginLeft = "3px";
        ahtml.appendChild(ihtml);
        divhtml.appendChild(ahtml);
        divhtml.style.display = "inline-flex";
        element_td.appendChild(divhtml);
        for (i = 0; i < split.length; i++) {
            conta_td++;
            element_td = element_row.insertCell(conta_td);
            element_td.innerHTML = split[i];
            element_td.classList.add("GridviewScrollItem_line_cort_tr_flex");
            element_td.setAttribute("onclick", "prevent_scrol(event,this,'')");
        }
     
    } catch (ex) {
        alert("Error geeneral funcion insert_row_lista_workflow " + ex.mensaje);
    }
}

//Elimina registro de la lista de documentos enlazados
function eliminar_fila_data_gred_simple_(gred, nombre_hiden, nombre_hiden_, seter, seter_) {
        try {
            var id_dent = $("#" + gred + " tr[id_rad=" + $("#" + nombre_hiden).val() + "]");
            var k = $("#" + nombre_hiden).val();
            if (id_dent.length > 0) {
                $("#" + gred + " tr[id_rad=" + $("#" + nombre_hiden).val() + "]").remove();
                $('#' + nombre_hiden).val(seter);
                $('#' + nombre_hiden_).val("");
                if (seter == "") {
                    document.getElementById("titel_visor").innerHTML = "";
                } 
                decrementa_documento_relacion_estado();
            }

        }
        catch (err) {
            alert(err.message + " Funcion eliminar_fila_data_gred_simple_");
        }

}
function decrementa_documento_relacion_estado() {
    try {
        var element_table = document.getElementById("GridView_list_documento_relacion");
        var numero_fila = element_table.rows.length - 1;
        document.getElementById("Hidden_numero_doc_rel").value = numero_fila;
        document.getElementById("Label_documentos").innerHTML = "Documentos " + numero_fila;
    }
    catch (err) {
        alert(err.message + " Funcion decrementa_documento_relacion_estado");
    }
}
function eliminar_fila_data_gred_simple_wf(gred, nombre_hiden, nombre_hiden_, seter, seter_) {
    try {
        var id_dent = $("#" + gred + " tr[id_wf=" + $("#" + nombre_hiden).val() + "]");
        if (id_dent.length > 0) {
            $("#" + gred + " tr[id_wf=" + $("#" + nombre_hiden).val() + "]").remove();
            $('#' + nombre_hiden).val(seter);
            $('#' + nombre_hiden_).val(seter_);
            decrementa_documento_relacion_estado_wf();
        }

    }
    catch (err) {
        alert(err.message + " Funcion eliminar_fila_data_gred_simple_wf");
    }

}
//Delete row relation workflow
const elimina_row_gred_relacionado = (table_gred, atrib, value_item) => {
    try {
        let id_dent = $("#" + table_gred + " tr[" + atrib + "=" + value_item + "]");
        if (id_dent.length > 0) {
            $("#" + table_gred + " tr[" + atrib + "=" + value_item + "]").remove();
            decrementa_documento_relacion_estado_wf();
        }
        return "YES";
    } catch (ex) {
        return "Funcion name elimina_row_gred error " + ex.mensaje;
    }
}
//Delete row link workflow
const elimina_row_gred_enlace = (table_gred, atrib, value_item) => {
    try {
        let id_dent = $("#" + table_gred + " tr[" + atrib + "=" + value_item + "]");
        if (id_dent.length > 0) {
            $("#" + table_gred + " tr[" + atrib + "=" + value_item + "]").remove();
            decrementa_documento_relacion_estado();         
        }
        return "YES";
    } catch (ex) {
        return "Funcion name elimina_row_gred_enlace error " + ex.mensaje;  
    }
}

function decrementa_documento_relacion_estado_wf() {
    try {
        var element_table = document.getElementById("GridView_list_documento_relacion_wf");
        var numero_fila = element_table.rows.length - 1;
        document.getElementById("Hidden_numero_doc_rel_wf").value = numero_fila;
        document.getElementById("Label_docu_relacionado_wf").innerHTML = "Documentos " + numero_fila;
    }
    catch (err) {
        alert(err.message + " Funcion decrementa_documento_relacion_estado");
    }
}
function incrementa_documento_relacion_estado() {
        try {
            var element_table = document.getElementById("GridView_list_documento_relacion");
            var numero_fila = element_table.rows.length - 1;
            document.getElementById("Hidden_numero_doc_rel").value = numero_fila;
            document.getElementById("Label_documentos").innerHTML = "Documentos " + numero_fila;
        }
        catch (err) {
            alert(err.message + " Funcion incrementa_documento_relacion_estado");
        }
    }
function actualiza_gre_campo(nombre_grid, id, valor_campo, nombre_campo, id_visor) {
        try {
            $("#" + nombre_grid + " tr[id_rad=" + id + "]").each(function () {
                var idex = -1;
                var name = nombre_campo;
                idex = colum_index_(name, nombre_grid);
                if (idex != -1) {
                    if (valor_campo == "") {
                        var sas = $(this)[0].cells[idex];
                        //var nodetext = document.getElementById("ocultop");
                        var trfirst = $('#' + nombre_grid + ' tr:first').next();
                        if (sas.childElementCount == 0) {
                            $(this)[0].cells[idex].innerText = "\u00a0";
                            //nodetext.innerText = "\u00a0";
                        }
                        if (sas.childElementCount >= 1) {
                            sas.firstChild.innerHTML = "&nbsp;";
                            //nodetext.innerText = "\u00a0";
                        }
                    }
                    if (valor_campo !== "") {
                       
                        $(this)[0].cells[idex].innerText = valor_campo;
                        $(this)[0].cells[idex].innerHTML = valor_campo;
                        if (id_visor == id) {
                            if (valor_campo != "") {
                                valor_campo = reemplazarAcentos(valor_campo);
                                document.getElementById("titel_visor").innerHTML = valor_campo;
                            }
                        }
                    }
                }
            })
            return true;
        }
        catch (err) {
            alert(err.message + " Funcion actualiza_gre_campo");
        }
}
function actualiza_gre_campo_wf_seleccion(nombre_grid, id, valor_campo, nombre_campo) {
    try {
        $("#" + nombre_grid + " tr[id_wf=" + id + "]").each(function () {
            var idex = -1;
            var name = nombre_campo;
            idex = colum_index_(name, nombre_grid);
            if (idex != -1) {
                if (valor_campo == "") {
                    var sas = $(this)[0].cells[idex];
                    //var nodetext = document.getElementById("ocultop");
                    var trfirst = $('#' + nombre_grid + ' tr:first').next();
                    if (sas.childElementCount == 0) {
                        $(this)[0].cells[idex].innerText = "\u00a0";
                        //nodetext.innerText = "\u00a0";
                    }
                    if (sas.childElementCount >= 1) {
                        sas.firstChild.innerHTML = "&nbsp;";
                        //nodetext.innerText = "\u00a0";
                    }
                }
                if (valor_campo !== "") {
                    var k = $(this)[0].cells[idex];
                    $(this)[0].cells[idex].innerText = valor_campo;
                    $(this)[0].cells[idex].innerHTML = valor_campo;
                }
            }
        })
        return true;
    }
    catch (err) {
        alert(err.message + " Funcion actualiza_gre_campo");
    }
}

function actualiza_gre_campo_wf_lista(nombre_grid, id, valor_campo, nombre_campo) {
    try {
        $("#" + nombre_grid + " tr[id=" + id + "]").each(function () {
            var idex = -1;
            var name = nombre_campo;
            idex = colum_index_(name, nombre_grid);
            if (idex != -1) {
                if (valor_campo == "") {
                    var sas = $(this)[0].cells[idex];
                    //var nodetext = document.getElementById("ocultop");
                    var trfirst = $('#' + nombre_grid + ' tr:first').next();
                    if (sas.childElementCount == 0) {
                        $(this)[0].cells[idex].innerText = "\u00a0";
                        //nodetext.innerText = "\u00a0";
                    }
                    if (sas.childElementCount >= 1) {
                        sas.firstChild.innerHTML = "&nbsp;";
                        //nodetext.innerText = "\u00a0";
                    }
                }
                if (valor_campo !== "") {
                    var k = $(this)[0].cells[idex];
                    $(this)[0].cells[idex].innerText = valor_campo;
                    $(this)[0].cells[idex].innerHTML = valor_campo;
                }
            }
        })
        return true;
    }
    catch (err) {
        alert(err.message + " Funcion actualiza_gre_campo_wf_lista");
    }
}
function colum_index_(colum_name, nombre_grid) {
        try {
            var x = $('#' + nombre_grid + ' th');
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
    //Función que permite que el boton que se agrega a la lista no envie el formulario
   
function resize_adjunta_documento_automatico() {
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
            //if (document.getElementById("Panel_adjunta_autamatico_documento").style.display == "Block") { }
            //var heig_ = document.getElementById("div_title_automatico").clientHeight + document.getElementById("opcion_adjunt").clientHeight + document.getElementById("div_inferior_automatico").clientHeight;
            //$("#Contenido_adjunta_autamatico_documento").css("height", heig_ + 50 + "px");
            //$("#Panel_adjunta_autamatico_documento").css("height", heig_ + 50 + "px");


        }
        catch (err) {
            alert(err.message + " funcion resize_adjunta_documento_automatico " + err.message);
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
function acti_busq_lista_cheq(e, sender) {
        try {

            tecla = (document.all) ? e.keyCode : e.which;
            if (tecla == 13) {
                //document.getElementById("Button_busca_general_archivo").click();
                busqueda_gred_lista_cheq('Hidden_0001', 'data_grid_chequeo', 'TextBox_contenido_busqueda_lista_cheq', 'CheckBox_busqueda_list_cheq');
                e.preventDefault();
                //return false;
            }


        } catch (err) {
            alert(err.message + " funcion acti_busq_general_archivo " + err.message);
        }
    }
function busqueda_gred_lista_cheq(HiddenSeleccion, data_grid, contenido_busqueda, CheckboxBusqueda) {
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
            alert(err.message + " funcion busqueda_gred_lista " + err.message);
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
        Sys.Extended.UI.Resources.AjaxFileUpload_SelectFileToUpload = "archivos(s) para cargar.";
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
        document.getElementById("draggable").style.top = topconten + "px";
        document.getElementById("draggable").style.left = lefconten + "px";
       
        
    }
    catch (err) {
        alert(err.message + " funcion firma_mecanica " + err.message);
    }
}
function limpiar_firma() {
    $("#draggable").css("display", "none");
}
//ZONA TOKENIZE--/////
const Tokenize2_token_items = (name_control) => {
    let Control_token;
    let array_token = new Array();
    if (document.getElementById(name_control) !== null) {
        Control_token = document.getElementById(name_control);
        for (let i = 0; i < Control_token.length - 1; i++) {
            array_token.push({ value: Control_token.options[i].value, text: Control_token.options[i].text});
        }
    }
    return array_token;
}
const Tokenize2_token_initial = (name_tokenize) => {
    $('.' + name_tokenize).tokenize2({
        placeholder: "Digite la dirección de correo y presiones enter..."
    })
    $('.' + name_tokenize).on('tokenize:tokens:added', function (e, value, text) {

    });
    $('.' + name_tokenize).on('tokenize:tokens:remove', function (e, value) {

    });
    }
