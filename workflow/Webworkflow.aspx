<%@ Page Language="vb" AutoEventWireup="false" EnableEventValidation="false" CodeBehind="Webworkflow.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.Webworkflow" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta id="workflowCentroTrabajoModernViewport" runat="server" name="viewport" content="width=device-width, initial-scale=1" visible="false" />

     <script src="../js/ui/jquery-3.4.1.min.js" type="text/javascript"></script>
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
     <script src="../js/jquery-ui-1.12.1.custom/jquery-ui.min.js" type="text/javascript"></script>
    <link href="../js/jquery-ui-1.12.1.custom/jquery-ui.min.css" rel="stylesheet" />
    <link href="../ccs/themes/base/jquery.ui.all.css" rel="stylesheet" type="text/css" />
    <script src="../js/sizeimagejquery.js" type="text/javascript"></script> 
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.11.0/umd/popper.min.js" type="text/javascript"></script>
    <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js" type="text/javascript"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />  
    <script src="../js/table_boo/table_boot_config.js?v=20260807-table-resize1" type="text/javascript"></script>
    <script src="../js/java_general/BootstrapTable.js" type="text/javascript"></script>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-table@1.23.1/dist/bootstrap-table.min.css"/>
    <script src="https://cdn.jsdelivr.net/npm/tableexport.jquery.plugin@1.29.0/tableExport.min.js" type="text/javascript"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap-table@1.23.1/dist/bootstrap-table.min.js" type="text/javascript"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap-table@1.23.1/dist/bootstrap-table-locale-all.min.js" type="text/javascript"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap-table@1.23.1/dist/extensions/export/bootstrap-table-export.min.js" type="text/javascript"></script>
    <link href="../Styles/styleMenu.css" rel="stylesheet" type="text/css" /> 
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <!-- Línea base visual aprobada antes de DOC-2: permanece activa para todos los usuarios. -->
    <link href="../Styles/gridview-moderno.css?v=20260807-phase2-6" rel="stylesheet" />
    <link href="../Styles/workflow-tareas-modernas.css?v=20260811-grid44" rel="stylesheet" />
    <link href="../Styles/workflow-documentos-relacionados-modernos.css?v=20260812-docrel23" rel="stylesheet" />
    <link href="../Styles/workflow-documentos-relacionados-titulo.css?v=20260808-title6" rel="stylesheet" />
    <script src="../js/workflow/workflow-paginacion-visual.js?v=20260807-pager6" type="text/javascript"></script>
    <script src="../js/workflow/documentos-relacionados-visual.js?v=20260812-docrel5" type="text/javascript"></script>
    <script src="../js/workflow/documentos-relacionados-titulo-visual.js?v=20260810-title3" type="text/javascript"></script>
    <script src="../js/validate_campos.js" type="text/javascript"></script>
    <script src="../js/java_general/general_config.js" type="text/javascript"></script>
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <link href="../Styles/Menu3.css" rel="stylesheet" />
    <link href="../Styles/tabs.css" rel="stylesheet" />    
    <script src="../js/java_general/JSReplaceScanFile.js" type="text/javascript"></script>
    <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <script src="../js/workflow/Webworkflow_2.js" type="text/javascript"></script>
     <script src="../js/java_general/GredviewControl.js?v=20260811-selector1" type="text/javascript"></script>
    <script src="../js/java_general/general_code_java.js" type="text/javascript"></script>
    <script src="../generic_control/FileUploadHandler.js" type="text/javascript"></script>
    <script src="../js/java_general/ASMXClient.js?v=20260807-asmxclient1" type="text/javascript"></script>
    <script src="../js/java_general/JSProgresBar.js" type="text/javascript"></script>
    <link href="../generic_control/UploadFile.css" rel="stylesheet" />
    <link href="../Awesome/css/fontawesome.css" rel="stylesheet" />
    <link href="../Awesome/css/brands.css" rel="stylesheet" />
    <link href="../Awesome/css/solid.css" rel="stylesheet" />
    <script  src="../Awesome/js/all.js" type="text/javascript"></script>
    <script  src="../Awesome/js/brands.js" type="text/javascript"></script>
    <script  src="../Awesome/js/solid.js" type="text/javascript"></script>
    <script src="../Awesome/js/fontawesome.js" type="text/javascript"></script>
    <link href="../tokenzize2/tokenize2.min.css" rel="stylesheet" />
    <script src="../tokenzize2/tokenize2.1.min.js" type="text/javascript"></script>
    <style type="text/css">
  #draggable {
    width: 100px;
    height: 100px;
    background: #ccc;
    
  }
 </style>
    <title>Workflow Documental</title>
</head>    
     <script src="../js/java_general/row_multiple_gred.js" type="text/javascript"></script>
     <script src="../js/java_general/JSExpediente.js" type="text/javascript"></script>
     <script src="../js/java_general/gestion_meta_dato.js" type="text/javascript"></script>
     <script src="../js/workflow/Webworkflow.js?v=20260812-taskclose53" type="text/javascript"></script>
     <script src="../js/sesion/js_sesion_gestor.js" type="text/javascript"></script>
     <script src="../js/versiondocumento/gestion_version_documento.js" type="text/javascript"></script>
     <script src="../js/java_general/JS_firma_digital.js" type="text/javascript"></script>
     <script src="../js/java_general/general_control_java.js" type="text/javascript"></script>
     <script src="../js/java_general/ubicacion_code_java.js" type="text/javascript"></script>  
     <% If WorkflowCentroTrabajoModernActive Then %>
      <link href="../Styles/workflow-centro-trabajo-moderno.css?v=20260813-mobileframe46" rel="stylesheet" type="text/css" />
      <script src="../js/workflow/centro-trabajo-visual.js?v=20260819-doc15grouporder1" type="text/javascript"></script>
     <% End If %>
 <body  style="margin: 0;
    background-color : #ffffff" >
      <form id="form1" style="height:100%" runat="server">
          <asp:ScriptManager ID="ScriptManager1" runat="server"
              EnableScriptGlobalization="True" EnablePageMethods="True" AsyncPostBackTimeout="1900" >
              
          </asp:ScriptManager>
           <script accesskey="javascript" type="text/javascript">
               Sys.WebForms.PageRequestManager.getInstance().add_initializeRequest(InitializeRequest);
               Sys.Application.add_load(ApplicationLoadHandler)
               var elment_postbak;
               function ApplicationLoadHandler(sender, args) {
                   Sys.WebForms.PageRequestManager.getInstance().add_endRequest(CheckStatus);
               }
               function InitializeRequest(sender, args) {
                   //
                   elment_postbak = args.get_postBackElement();
                   if (args.get_postBackElement().id == 'Buttonaceptar') {
                       document.getElementById("Buttonaceptar").disabled = true;
                       document.getElementById("Buttonaceptar").value = "Espere....";
                   }
                   //Inactiva el boton reasignar
                   if (args.get_postBackElement().id == 'ButtonReasignarTerminar') {
                       document.getElementById("ButtonReasignarTerminar").disabled = true;
                       document.getElementById("ButtonReasignarTerminar").value = "Espere....";
                   }
                   //Inactiva enviar a actividad
                   if (args.get_postBackElement().id == 'btnOkpagina') {
                       document.getElementById("btnOkpagina").disabled = true;
                       document.getElementById("btnOkpagina").value = "Espere....";
                   }
                   //Inactiva boton auto terminar overlay
                   if (args.get_postBackElement().id == 'btnOkay_autoterminar') {
                       document.getElementById("btnOkay_autoterminar").disabled = true;
                       document.getElementById("btnOkay_autoterminar").value = "Espere....";
                   }
                   //Button_tool_activa_sube_imagen_inscripcion_web_service
                   if (elment_postbak.id == "Button_tool_activa_sube_imagen_inscripcion_web_service") {
                       posicion_update_pogres('progres_bar');
                   }
    
                   if (elment_postbak.id == "ButtonAlmacenar" || elment_postbak.id == "Button_guardar_desicion") {

                       posicion_update_pogres_modal('progres_bar');
                       
                   } else {
                       posicion_update_pogres('progres_bar');
                   }
                  
               }
               function CheckStatus(sender, args) {
                   try {
                       //auto_size_control_documentos();
                       actuo_zise_popup_compartir_correo_electronico();
                       //Button_activa_respuesta_radicado_tag
                       if (elment_postbak.id == 'Button_activa_respuesta_radicado_tag') {
                           if (document.getElementById("Hidden_id_respuesta").value !== -1) {
                               if (document.getElementById("Hidden_id_tarea_selecionada").value !== "0") {
                                   var w = window.open('../radicador/WebFormContendorPage.aspx', '_blank', "", "true");
                                   //w.focus();
                                   //return false;
                               }
                           }

                       }
  
                   if (elment_postbak.id == 'Buttonaceptar') {
                       document.getElementById("Buttonaceptar").disabled = false;
                       document.getElementById("Buttonaceptar").value = "Enlazar Documentos";
                       
                   }
                       //activa el boton reasignar
                   if (elment_postbak.id == 'ButtonReasignarTerminar') {
                       document.getElementById("ButtonReasignarTerminar").disabled = false;
                       document.getElementById("ButtonReasignarTerminar").value = "Reasignar";
                       document.getElementById("Hidden_gredv_lista").value = "YES";
                       auto_zise_popup_envia_usuario_grupo();
                   }
                       //activa enviar a actividad
                   if (elment_postbak.id == 'btnOkpagina') {
                       document.getElementById("btnOkpagina").disabled = false;
                       document.getElementById("btnOkpagina").value = "Aceptar";
                       document.getElementById("Hidden_gredv_lista").value = "YES";
                       auto_zise_popup_envia_usuario_grupo();
                   }
                      
                 
                   if (elment_postbak.id == "ImageButtonpendiente") {
                       document.getElementById("Labelpendiente").innerHTML = "Tareas pendientes";
                       auto_zise_popup_pendinetes();
                   }
                   if (elment_postbak.id == "ImageButton_pendiente_aprobacion") {
                       document.getElementById("Labelpendiente").innerHTML = "Solicitudes de aprobación";
                       auto_zise_popup_pendinetes();
                   }                   
                  
                   if (elment_postbak.id == "ButtonAlmacenar" || elment_postbak.id == "Button_guardar_desicion") {
                       $("#progres_bar").removeClass("overlay_");             
                   }   
                       
                   if (elment_postbak.id == "ImageButtonEnviarUsuario") {
                       if (document.getElementById("Hidden_vi_reasigna").value == "1") {

                           if (document.getElementById("btnOkpagina")) {
                               document.getElementById("btnOkpagina").style.visibility = 'hidden';
                           }
                           if (document.getElementById("ButtonReasignarTerminar")) {
                               document.getElementById("ButtonReasignarTerminar").style.visibility = 'visible';
                           }

                       } else {
                           if (document.getElementById("btnOkpagina")) { document.getElementById("btnOkpagina").style.visibility = 'visible'; }
                           if (document.getElementById("ButtonReasignarTerminar")) { document.getElementById("ButtonReasignarTerminar").style.visibility = 'hidden'; }

                       }
                       document.getElementById("Labeletiqueta").innerHTML = "Enviar la tarea seleccionada al usuario a seleccionar en la lista";
                       document.getElementById("Hidden_gredv_lista").value = "YES";
                       auto_zise_popup_envia_usuario_grupo();
                   }
                   if (elment_postbak.id == "ImageButtonEnviaActividad") {
                       document.getElementById("Labeletiqueta").innerHTML = "Enviar la tarea seleccionada al grupo o actividad a seleccionar en la lista";
                       document.getElementById("Hidden_gredv_lista").value = "YES";
                       auto_zise_popup_envia_usuario_grupo();
                   }
                     
                   if (elment_postbak.id == "Buttonactividad" ) {          
                        auto_zise_tareas_grupos();
                   
                   }
                   if (elment_postbak.id == "Buttonuser") {
                       auto_zise_tareas_usuario();
                   }
                  
                  
                   if (elment_postbak.id == "Button_visor_emergente") {
                       auto_zise_popup_visor_externo();
                   }
                   if (elment_postbak.id == "ButtonVisua") {
                       dispalyVisorEmergente();
                   }
                   
                       //ENVIAR ACTIVIDAD FLUJO 
                       if (elment_postbak.id == "ImageButtonterminar" || elment_postbak.id == "Button_tool_devolver_a_usuario" || elment_postbak.id == "Button_tool_devolver_a_actividades_anterior") {
                       if (document.getElementById("Hidden_lista_ruta_flujo").value == "F") {
                           auto_zise_popup_lista_usuario_flujo();
                          
                       }
                       if (document.getElementById("Hidden_lista_ruta_flujo").value == "R") {
                           auto_zise_popup_lista_usuario_flujo();
                           
                       }
                   }
                   
                       //ImageButton_adjunt
                   if (elment_postbak.id == "ImageButton_adjunt") {
                       if (document.getElementById("Hidden_0002").value == "1") {
                           document.getElementById("Hidden_0002").value = "0";
                           auto_zise_popup_lista_chequeo("1");
                       }
                   }
                   if (elment_postbak.id == "ImageButtonActivaClasifica") {
                       if (document.getElementById("Hidden_0004").value == "1") {
                           document.getElementById("Hidden_0004").value = "0";
                           //auto_zise_popup_lista_chequeo_edita("1");
                       }
                   }
                  
                     
                   if (elment_postbak.id == "Button_clasficar_documento") {
                       if (document.getElementById("Hidden_0004").value == "1") {
                           document.getElementById("Hidden_0004").value = "0";
                           auto_zise_popup_lista_chequeo_edita("1");
                       }
                   }
                       
                   if (elment_postbak.id == "Button_Actualizar_item_lista") {
                       if (document.getElementById("hidden_estado_seleccion").value == "1") {
                           document.getElementById("hidden_estado_seleccion").value = "";
                           actualiza_treview_seleccion();
                       }
                       
                   }
                   if (elment_postbak.id == "Button_Actualizar_seleccion_indice_wf") {
                           actualiza_treview_seleccion();
                   }
                   if (elment_postbak.id == "Button_actualizar_seleccion_digitalizacion") {
                       actualiza_treview_seleccion_escaner();
                   }
                   if (elment_postbak.id == "Button_activa_copiar_estructura") {
                       auto_zise_popup_copiar_estructura();
                       document.getElementById("title_copiar_estructura").innerHTML = "Copiar a producción de expedientes";
                   }
                   if (elment_postbak.id == "Button_activa_copiar_expediente") {
                       auto_zise_popup_copiar_estructura();
                       document.getElementById("title_copiar_estructura").innerHTML = "Copiar a expediente";
                    }
                   if (elment_postbak.id == "Button_activa_incorpora_expediente") {
                       auto_zise_popup_copiar_estructura();
                       document.getElementById("title_copiar_estructura").innerHTML = "Incorporar a expediente";
                   }
                      
                   if (elment_postbak.id == "Button_export_lista") {
                       if (document.getElementById("Hidden_ruta_archivo").value !== "") {
                           //plugin_grwedview();
                           document.getElementById('ifmExcel_reporte_').src = "../radicador/WebFormDescargaRadicado.aspx";
                       }
                   }
                   if (elment_postbak.id == "Button_tool_visualiza_documento") {
                       if (document.getElementById("Hidden_result_boton_tool").value == "YES") {
                           document.getElementById("Hidden_result_boton_tool").value = "";
                           dispalyVisorEmergente();
                       }
                   }
                   if (elment_postbak.id == "ButtonAsignar") {
                       if (document.getElementById("hidden_000_aceptacion").value == "YES") {
                           document.getElementById("hidden_000_aceptacion").value = "";
                           show_area_workflow_seleccion();
                       }
                   }
                   //Elimina documentos relacionados a la lista de documentos en nelace
                   if (elment_postbak.id == "Button_tool_elimina_documento") {
                       if (document.getElementById("Hidden_result_boton_tool").value == "YES") {
                           document.getElementById("Hidden_result_boton_tool").value = "";
                           var seter = "1";
                           if (document.getElementById("Hidden_selccion_documento_eliminar_rad").value == document.getElementById("hiden_seleccion_documento_id").value) {
                               seter = "";
                               dispalyInterfaceEscaner();
                           }
                         
                           eliminar_fila_data_gred_simple_('GridView_list_documento_relacion', 'Hidden_selccion_documento_eliminar_rad', 'Hidden_selccion_documento_eliminar_split_rad', seter, seter);
                       }
                       }
                       //Elimina documntos en la lista de documentos relacionados workflow
                   if (elment_postbak.id == "Button_eliminar_documento") {
                           if (document.getElementById("Hidden_confir_elimina").value == "YES") {
                               document.getElementById("Hidden_confir_elimina").value = "";
                               eliminar_fila_data_gred_simple_wf('GridView_list_documento_relacion_wf', 'Hidden_selccion_documento_eliminar_wf', 'Hidden_selccion_documento_eliminar_split_wf', '', '');
                           }
                   }
                   if (elment_postbak.id == "Button_actualiza_tipologia_documental") {
                       if (document.getElementById("Hidden_resulta_botno_tipologia_documental").value != "") {           
                           update_Cell_AspNetGred('GridView_list_documento_relacion', document.getElementById("Hidden_selccion_documento_cambia_tipo_rad").value, document.getElementById("Hidden_resulta_botno_tipologia_documental").value, 'DOCUMENTO',"id_rad");
                           document.getElementById("Hidden_resulta_botno_tipologia_documental").value = "";
                       }
                   }
                       if (elment_postbak.id == "Button_actualiza_tipologia_documental_workflow") {
                           if (document.getElementById("Hidden_resulta_botno_tipologia_documental_workflow").value != "") {
                               update_Cell_AspNetGred('GridView_list_documento_relacion_wf', document.getElementById("Hidden_selccion_documento_cambia_tipo_wf").value, document.getElementById("Hidden_resulta_botno_tipologia_documental_workflow").value, 'DOCUMENTO','id_wf');
                               document.getElementById("Hidden_resulta_botno_tipologia_documental_workflow").value = "";
                           }
                       }
                       //
                   if (elment_postbak.id == "Button_guardar_desicion" || elment_postbak.id == "Button_acepta_sube_documento_integra_sii") {
                       if (document.getElementById("Hidden_result_load").value == "YES") {
                           document.getElementById("Hidden_result_load").value = "";
                           insert_row_documento_relacionado(document.getElementById("Hidden_date_row").value, document.getElementById("Hidden_tip_adjunt").value,1);
                           document.getElementById("Hidden_date_row").value = "";
                           document.getElementById("Hidden_tip_adjunt").value = "";
                       }
                   }
                       if (elment_postbak.id == "Button_guardar_automatico" ) {
                           if (document.getElementById("HiddenField_estado_guarda_automatico").value == "YES") {
                               document.getElementById("HiddenField_estado_guarda_automatico").value = "";
                               insert_row_documento_relacionado(document.getElementById("Hidden_date_row_auto").value, document.getElementById("Hidden_tip_adjunt_auto").value,1);
                               document.getElementById("Hidden_date_row_auto").value = "";
                               document.getElementById("Hidden_tip_adjunt_auto").value = "";
                           }
                       }
                    if (elment_postbak.id == "Button_guardar_desicion_fre_image") {
                      if (document.getElementById("Hidden_date_row").value != "") { 
                               insert_row_documento_relacionado(document.getElementById("Hidden_date_row").value, document.getElementById("Hidden_tip_adjunt").value,1);
                               document.getElementById("Hidden_date_row").value = "";
                               document.getElementById("Hidden_tip_adjunt").value = "";
                      }
                    }
                  
                   
                   if (elment_postbak.id == "ButtonAlmacenar") {
                       if (document.getElementById("Hidden_result_load_").value == "YES") {
                           document.getElementById("Hidden_result_load_").value = "";
                           insert_row_documento_relacionado(document.getElementById("Hidden_date_row_").value, document.getElementById("Hidden_tip_adjunt").value,1);
                           document.getElementById("Hidden_date_row_").value = "";
                           document.getElementById("Hidden_tip_adjunt").value = "";
                       }
                   }
                   
                  
                       //Boton recupera la tarea
                   if (elment_postbak.id == "ButtonRecuperar") {
                       if (document.getElementById("Hidden_resultado_selecion").value == "YES") {
                           document.getElementById("Hidden_resultado_selecion").value = "NO";
                           show_area_workflow_seleccion();
                           if (document.getElementById("Hidden_00020_4001").value == 0) {
                               if (document.getElementById("Hidden_00021_row").value !== "") {
                                   insert_row_lista_workflow(document.getElementById("Hidden_id_tarea_sel").value, 'GridView2', document.getElementById("Hidden_00021_row").value);
                                   document.getElementById("Hidden_00021_row").value = "";
                               }
                               actualiza_gre_campo_wf_lista('GridView2', document.getElementById("Hidden_id_tarea_sel").value, 'En proceso', 'ESTADO');
                               //Cambia el boton del registro
                               changue_boton(document.getElementById("Hidden_id_tarea_sel").value);
                               //asigna el tipo adjunto para los documentos enlazados
                               document.getElementById("Hidden_tip_adjunt").value = "wf";
                           } else {
                               //asigna el tipo adjunto para los documentos enlazados
                               document.getElementById("Hidden_tip_adjunt").value = "wf";
                               eliminar_fila_data_gred_lista('GridView2', 'Hidden_id_tarea_sel');
                           }

                       } else {
                           //asigna el tipo adjunto para los documentos enlazados
                           document.getElementById("Hidden_tip_adjunt").value = "rad";
                       }
                       }
                       //Actival asignacion de la tarea desde la lista de tareas, desde enlace o asignacion directa
                       if (elment_postbak.id == "ButtonSeleccionGrupo") {
                           //asigna la tarea directamente
                           if (document.getElementById("Hidden_resultado_selecion").value == "YES") {
                               document.getElementById("Hidden_resultado_selecion").value = "NO";
                               show_area_workflow_seleccion();
                               if (document.getElementById("Hidden_00020_4001").value == 0) {
                                   changue_boton_color(document.getElementById("Hidden_id_tarea_sel").value);
                                   changue_boton(document.getElementById("Hidden_id_tarea_sel").value);
                                   actualiza_gre_campo_wf_lista('GridView2', document.getElementById("Hidden_id_tarea_sel").value, 'En proceso', 'ESTADO');
                               } else {
                                   eliminar_fila_data_gred_lista('GridView2', 'Hidden_id_tarea_sel');
                               }
                               document.getElementById("Hidden_tip_adjunt").value = "wf";
                           } else {
                               //asigna el tipo adjunto para el enlace para la ventana de enlace
                               document.getElementById("Hidden_tip_adjunt").value = "rad";
                           }
                       }
                       //Asigna tarea desde la opción recuperar tarea 
                       if (elment_postbak.id == "Buttonaceptar") {
                           if (document.getElementById("Hidden_resultado_selecion_enlace").value == "YES") {
                               document.getElementById("Hidden_resultado_selecion_enlace").value = "NO";
                               dispalyInterfaceEscaner();
                               show_area_workflow_seleccion();
                               if (document.getElementById("Hidden_00020_4001").value == 0) {
                                   //Asigna la tarea desde la actividad enlace     
                                   if (document.getElementById("Hidden_00022_row").value !== "") {
                                       //Elimina el registro si existe en la lista de espera del usuario
                                       eliminar_fila_data_gred_lista_sin_set('GridView2', 'Hidden_id_tarea_sel');
                                       //Inserta el registro el documento recuperado
                                       insert_row_lista_workflow(document.getElementById("Hidden_id_tarea_sel").value, 'GridView2', document.getElementById("Hidden_00022_row").value);
                                       document.getElementById("Hidden_00022_row").value = "";
                                       //Actualiza el estado del registro
                                       actualiza_gre_campo_wf_lista('GridView2', document.getElementById("Hidden_id_tarea_sel").value, 'En proceso', 'ESTADO');
                                       //Cambia el boton del registro
                                       changue_boton(document.getElementById("Hidden_id_tarea_sel").value);
                                       //asigna el tipo adjunto para los documentos enlazados
                                       document.getElementById("Hidden_tip_adjunt").value = "wf";
                                   } else {
                                       //asigna el tipo adjunto para los documentos enlazados
                                       document.getElementById("Hidden_tip_adjunt").value = "wf";
                                       changue_boton(document.getElementById("Hidden_id_tarea_sel").value);
                                   }

                               } else {
                                   //asigna el tipo adjunto para los documentos enlazados
                                   document.getElementById("Hidden_tip_adjunt").value = "wf";
                                   eliminar_fila_data_gred_lista('GridView2', 'Hidden_id_tarea_sel');
                               }

                           }
                       }
                   if (elment_postbak.id == "Button_aceptar_reasignacion_tarea_recuperada_enlazada") {
                       if (document.getElementById("Hidden_resp_reasignacion_tarea_recuperada_enlazada").value == "YES") {
                           document.getElementById("Hidden_resp_reasignacion_tarea_recuperada_enlazada").value = "";
                           show_area_workflow_seleccion();
                           if (document.getElementById("Hidden_00020_4001").value == 0) {
                               //changue_boton_color(document.getElementById("Hidden_id_tarea_sel").value);
                               actualiza_gre_campo_wf_lista('GridView2', document.getElementById("Hidden_id_tarea_sel").value, 'En proceso', 'ESTADO');
                           } else {
                               eliminar_fila_data_gred_lista('GridView2', 'Hidden_id_tarea_sel');
                           }
                       }
                       }
                       //Envia la tarea a pendiente
                   if (elment_postbak.id == "Button_aceptar_envia_documento_pendiente_apro") {
                       if (document.getElementById("Hidden_000_estado").value == "YES") {
                           document.getElementById("Hidden_000_estado").value = "";
                           hide_area_workflow_seleccion();
                           if (document.getElementById("Hidden_00020_4001").value == 0) {
                               changue_boton_color(document.getElementById("Hidden_id_tarea_sel").value);  
                               actualiza_gre_campo_wf_lista('GridView2', document.getElementById("Hidden_id_tarea_sel").value, 'En proceso', 'ESTADO');
                               document.getElementById("Hidden_id_tarea_sel").value = "-1";
                           } else {
                               eliminar_fila_data_gred_lista('GridView2', 'Hidden_id_tarea_sel');
                               document.getElementById("Hidden_id_tarea_sel").value = "-1";
                           }
                          
                       }
                   }
                  
                   if (elment_postbak.id == "Button_tool_enviar_usuario") {
                       if (document.getElementById("Hidden_result_boton_tool").value == "YES") {
                           document.getElementById("Hidden_result_boton_tool").value = "";
                           eliminar_fila_data_gred_lista('GridView2', 'Hidden_00005_2222');
                           hide_area_workflow_seleccion();
                       }
                   }
                       if (elment_postbak.id == "Button_tool_devolver_a_usuario") {
                           if (document.getElementById("Hidden_result_boton_tool").value == "YES") {
                               document.getElementById("Hidden_result_boton_tool").value = "";
                               eliminar_fila_data_gred_lista('GridView2', 'Hidden_00005_2222');
                               hide_area_workflow_seleccion();
                           }
                       }
                   if (elment_postbak.id == "Button_tool_enviar_actividad") {
                       if (document.getElementById("Hidden_result_boton_tool").value == "YES") {
                           document.getElementById("Hidden_result_boton_tool").value = "";
                           eliminar_fila_data_gred_lista('GridView2', 'Hidden_00005_2222');
                           hide_area_workflow_seleccion();
                       }
                   }

                   if (elment_postbak.id == "Button_activa_enviar_actividad_ruta") {
                       if (document.getElementById("Hidden_result_actividad_ruta").value == "YES") {
                           document.getElementById("Hidden_result_actividad_ruta").value = "";
                           eliminar_fila_data_gred_lista('GridView2', 'Hidden_00005_2222');
                           hide_area_workflow_seleccion();
                       }
                   }
                   if (elment_postbak.id == "Button_activa_enviar_actividad_flujo_trabajo") {
                       if (document.getElementById("Hidden_resultado_enviar_activdad_flujo").value == "YES") {
                           document.getElementById("Hidden_resultado_enviar_activdad_flujo").value = "";
                           eliminar_fila_data_gred_lista('GridView2', 'Hidden_00005_2222');
                           hide_area_workflow_seleccion();
                       }
                       }
                       if (elment_postbak.id == "Button_activa_enviar_actividad_flujo_trabajo_anterior") {
                           if (document.getElementById("Hidden_resultado_enviar_activdad_flujo").value == "YES") {
                               document.getElementById("Hidden_resultado_enviar_activdad_flujo").value = "";
                               eliminar_fila_data_gred_lista('GridView2', 'Hidden_00005_2222');
                               hide_area_workflow_seleccion();
                           }
                       }
                       
                       
                       if (elment_postbak.id == "btnOkay_autoterminar") {
                       document.getElementById("btnOkay_autoterminar").disabled = false;
                       document.getElementById("btnOkay_autoterminar").value = "Aceptar";
                       if (document.getElementById("Hidden_result_auto_termnar").value = "YES") {
                           document.getElementById("Hidden_result_auto_termnar").value = "";
                           eliminar_fila_data_gred_lista('GridView2', 'Hidden_00005_2222');
                           hide_area_workflow_seleccion();
                       }
                   }
                   if (elment_postbak.id == "ImageButtonpendiente") {
                       auto_zise_tareas_pendientes();
                   }
                   if (elment_postbak.id == "TreeViewseleccion") {
                       ver_visor();
                       autozize_iframe_visor();
                   }
                   }
                   catch (err) {
                       alert(" Funcion CheckStatus asincrona workflow.aspx error : " + err.message);
                   }
                   finally {
                       progres_hiden('progres_bar');
                       resize_adjunta_documento();
                       resize_adjunta_documento_automatico();
                       auto_zise_popup_envia_usuario_grupo();
                   }
               }

           </script>
          <div id="div_content_general_wf"<%= WorkflowCentroTrabajoModernCssAttribute %> style="width:auto; height:100%">
              <div id="div_error_content_wf" style="position: relative; width: 100%"></div>
              <asp:UpdatePanel ID="UpdatePanel_menu_cab" runat="server" UpdateMode="Conditional">
                  <ContentTemplate>
                      <nav id="menucab" class="navbar navbar-expand-sm nav_botota_person_gray_ modal_content_no_back_inferior" >
                          <button id="nav_togle_display" class="navbar-toggler" type="button" style="background-color: #6d7fcc" data-toggle="collapse" data-target="#navbarNavDropdown">
                              <span class="navbar-toggler-icon_"><i style="color: white" class="fad fa-th-list"></i></span>
                          </button>
                          <div class="collapse navbar-collapse row" id="navbarNavDropdown">
                              <ul class="navbar-nav col-md-12">
                                  <li class="nav-item dropdown active ml-2 mr-0 ">
                                      <a class="nav-link  dropdown-toggle " style="color: #6d7fcc" href="#"  data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc" class="fad fa-th-list"></i> Opciones
                                      </a>
                                      <div class="dropdown-menu" role="group" aria-label="Opciones de la tarea">
                                          <a  href="#" class="dropdown-item font-weight-light" onclick="prevent_tool_menucab(event,this,'T-RTW')"><i style="color: #6d7fcc" class="fad fa-search-plus"></i> <span class="font-weight-light"> Recuperar tarea </span></a>
                                          <% If WorkflowCentroTrabajoModernActive Then %>
                                          <a href="#" class="dropdown-item font-weight-light" id="bnt_eval_event_default" onclick="event_element_clic(event,this)"><i style="color: #6d7fcc" class="fad fa-sync-alt"></i><span class="font-weight-light"> Servicio default</span></a>
                                          <% End If %>
                                          <% If Not WorkflowCentroTrabajoModernActive Then %>
                                          <a  href="#" class="dropdown-item font-weight-light" onclick="prevent_tool_menucab(event,this,'S-DDS')"><i style="color: #6d7fcc" class="fad fa-bars"></i><span class="font-weight-light"> Detalle de la sesión  </span></a>
                                          <a  href="#" class="dropdown-item font-weight-light" onclick="prevent_tool_menucab(event,this,'S-GAU')"><i style="color: #6d7fcc" class="fad fa-user-friends"></i><span class="font-weight-light"> Grupo relacionado </span></a>
                                          <a  href="#" class="dropdown-item font-weight-light" onclick="activa_boton_client_server('Button_activa_estado_paginacion');"><i style="color: #6d7fcc" class="fad fa-browser"></i><span class="font-weight-light"> Estado de paginación </span></a>
                                          <% End If %>
                                      </div>
                                  </li>  
                                  <asp:Panel ID="Panel_detalle_tarea" CssClass="navbar-nav " runat="server" data-workflow-task-action="true">
                                      <li class="nav-item dropdown active ml-2 mr-0 ">
                                          <a class="nav-link  dropdown-toggle " style="color: #6d7fcc" href="#" id="A8" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc" class="fad fa-tasks"></i> Detalle
                                          </a>
                                          <div class="dropdown-menu ctw-detail-menu" role="group" aria-label="Consultas de la tarea">
                                              <% If WorkflowCentroTrabajoModernActive Then %>
                                              <div class="ctw-detail-menu__section" role="group" aria-label="Información">
                                                  <span class="ctw-detail-menu__section-label">Información</span>
                                              <% End If %>
                                              <a href="#" class="dropdown-item font-weight-light ctw-detail-menu__item" onclick="prevent_tool_menucab(event,this,'S-DTS')"><i style="color: #6d7fcc" class="fad <% If WorkflowCentroTrabajoModernActive Then %>fa-info-circle<% Else %>fa-bars<% End If %>"></i><span class="font-weight-light"> Información de la tarea </span></a>
                                              <a href="#" class="dropdown-item font-weight-light ctw-detail-menu__item" onclick="prevent_tool_menucab(event,this,'G-DRR')"><i style="color: #6d7fcc" class="fad <% If WorkflowCentroTrabajoModernActive Then %>fa-file-alt<% Else %>fa-bars<% End If %>"></i><span class="font-weight-light"> Detalle del trámite  </span></a>
                                              <a href="#" class="dropdown-item font-weight-light ctw-detail-menu__item" onclick="prevent_tool_menucab(event,this,'G-TDR') "><i style="color: #6d7fcc" class="fad fa-list-ol"></i><span class="font-weight-light"> Transacciones del trámite </span></a>
                                              <% If WorkflowCentroTrabajoModernActive Then %>
                                              </div>
                                              <div class="ctw-detail-menu__section" role="group" aria-label="Trazabilidad">
                                                  <span class="ctw-detail-menu__section-label">Trazabilidad</span>
                                              <% End If %>
                                              <a href="#" class="dropdown-item font-weight-light ctw-detail-menu__item" onclick="prevent_tool_menucab(event,this,'G-TDW')"><i style="color: #6d7fcc" class="fad <% If WorkflowCentroTrabajoModernActive Then %>fa-history<% Else %>fa-table<% End If %>"></i><span class="font-weight-light"> Trazabilidad de la tarea </span></a>
                                              <a href="#" class="dropdown-item font-weight-light ctw-detail-menu__item" onclick="prevent_tool_menucab(event,this,'G-TDWG')"><i style="color: #6d7fcc" class="fad fa-project-diagram"></i><span class="font-weight-light"> Trazabilidad grafica de la tarea </span></a>
                                              <% If WorkflowCentroTrabajoModernActive Then %>
                                              </div>
                                              <div class="ctw-detail-menu__section" role="group" aria-label="Documentos">
                                                  <span class="ctw-detail-menu__section-label">Documentos</span>
                                              <% End If %>
                                              <a href="#" class="dropdown-item font-weight-light ctw-detail-menu__item" id ="a_list_operation_document" ><i style="color: #6d7fcc" class="fad <% If WorkflowCentroTrabajoModernActive Then %>fa-cogs<% Else %>fa-info-square<% End If %>"></i><span class="font-weight-light">  Detalle de operaciones con documentos </span></a>
                                              <a href="#" class="dropdown-item font-weight-light ctw-detail-menu__item" id ="a_list_copy_document_expedient"><i style="color: #6d7fcc"  class="fad <% If WorkflowCentroTrabajoModernActive Then %>fa-copy<% Else %>fa-table<% End If %>"></i><span class="font-weight-light">  Detalle documentos copiados a expediente </span></a>
                                              <% If WorkflowCentroTrabajoModernActive Then %>
                                              </div>
                                              <% End If %>
                                          </div>
                                      </li>
                                  </asp:Panel>
                                  <asp:Panel ID="Panel_tramitar_tarea" CssClass="navbar-nav " runat="server" data-workflow-task-action="true">
                                      <% If Not WorkflowCentroTrabajoModernActive Then %>
                                       <li class="nav-item dropdown active ml-2 mr-0 ">
                                          <a class="nav-link  dropdown-toggle " style="color: #6d7fcc" href="#" id="A7" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc" class="fad fa-list"></i> Servicios
                                          </a>
                                          <div class="dropdown-menu" role="group" aria-label="Servicios de la tarea">
                                               <a  href="#" class="dropdown-item font-weight-light" id="bnt_eval_event_default" onclick="event_element_clic(event,this)"><i style="color: #6d7fcc" class="fad fa-sync-alt"></i><span class="font-weight-light"> Servicio default</span>  </a>
                                             
                                          </div>
                                       </li>
                                      <% End If %>
                                  </asp:Panel>
                                  <asp:Panel ID="Panel_documentos_tarea" CssClass="navbar-nav " runat="server" data-workflow-task-action="true">
                                      <% If Not WorkflowCentroTrabajoModernActive Then %>
                                      <li class="nav-item dropdown active ml-2 mr-0 ">
                                          <a class="nav-link  dropdown-toggle " style="color: #6d7fcc" href="#" id="A6" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc" class="fad fa-folder-open"></i> Documentos
                                          </a>
                                          <div class="dropdown-menu" role="group" aria-label="Acciones sobre los documentos">
                                              <a  href="#" class="dropdown-item font-weight-light" onclick="prevent_elimina_adjunto();"><i style="color: #6d7fcc" class="fad fa-file-times"></i><span class="font-weight-light"> Eliminar documento adjunto </span></a>
                                              <a  href="#" class="dropdown-item font-weight-light" onclick="prevent_tool_menucab(event,this,'D-DNDT')"><i style="color: #6d7fcc" class="fad fa-scanner-image"></i><span class="font-weight-light"> Adjuntar documento digitalizado </span></a>
                                              <a  href="#" class="dropdown-item font-weight-light" atr_adj="enlace_adjunt" onclick="inicializa_tipo_adjunto_documento(event,this,'C-DW-ENL')"><i style="color: #6d7fcc" class="fad fa-upload"></i><span class="font-weight-light"> Adjuntar documento </span></a>
                                              <div class="dropdown-divider"></div> 
                                              <a  href="#" class="dropdown-item font-weight-light" onclick="prevent_tool_menucab(event,this,'D-CDW')"><i style="color: #6d7fcc" class="fad fa-share-square"></i><span class="font-weight-light"> Compartir documentos a usuarios </span></a>
                                              <a  href="#" class="dropdown-item font-weight-light" onclick="prevent_tool_menucab(event,this,'D-CEDTS')"><i style="color: #6d7fcc" class="fad fa-envelope"></i><span class="font-weight-light"> Compartir documentos a correo  electrónico</span></a>
                                              <div class="dropdown-divider"></div> 
                                              <a id="a_copy_document_production_proceedings_"  href="#" class="dropdown-item font-weight-light"><i style="color: #6d7fcc" class="fad fa-folder-tree"></i><span class="font-weight-light"> Copiar los documentos seleccionados a su archivo de producción documental</span></a>
                                              <a id="a_copy_document_proceedings_"  href="#" class="dropdown-item font-weight-light" ><i style="color: #6d7fcc" class="fad fa-copy"></i><span class="font-weight-light"> Copiar los documentos seleccionados al expediente </span></a>
                                              <a id="a_link_document_proceedings_"  href="#" class="dropdown-item font-weight-light" ><i style="color: #6d7fcc" class="fad fa-folders"></i><span class="font-weight-light"> Vincular los documentos seleccionados al expediente </span></a>
                                              <a id="a_auto_link_document_proceedings_"  href="#" class="dropdown-item font-weight-light" ><i style="color: #6d7fcc" class="fad fa-folder-download"></i><span class="font-weight-light"> Crea un expediente de manera automática y vincula los documentos seleccionados </span></a>
                                          </div>
                                      </li>
                                      <% End If %>
                                  </asp:Panel>
                                  <asp:Panel ID="Panel_tareas_estado_pendiente" CssClass="navbar-nav " runat="server">
                                      <li class="nav-item dropdown active ml-2 mr-0 ">
                                          <a class="nav-link" href="#">
                                              <i style="color: #0062cc" class="fad fa-tasks-alt"></i>
                                              <span class="dropdown-toggle " style="color: #6d7fcc" id="pendiente_db" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"> Pendientes
                                              </span>
                                              <div class="dropdown-menu" role="group" aria-label="Tareas pendientes">
                                                  <span class="dropdown-item font-weight-light" style="color: #6d7fcc" onclick="activa_boton_client_server('ImageButtonpendiente');"><i class="fad fa-list-ul"></i> Listas tarea pendientes </span>
                                              </div>
                                          </a>
                                      </li>
                                  </asp:Panel>
                              </ul>
                          </div>
                      </nav>
                  </ContentTemplate>
              </asp:UpdatePanel>
              <div id="Menutol" class="modal_content_no_back_inferior" style="border-bottom-width: 0.5px; border-left-width: 0.5px; border-right-width: 0.5px; border-top-width: 0.5px;height: auto; width: 100%; position: relative; margin: 0px 0px 0px 0px; top: 0px; left: 0px">
              <asp:UpdatePanel ID="updatemenu" runat="server" UpdateMode="Conditional">
                  <ContentTemplate>
                      <nav id="nav_menu" class="navbar navbar-expand-sm nav_botota_person_" >
                          <button class="navbar-toggler" type="button" style="background-color: #6d7fcc" data-toggle="collapse" data-target="#navbarNavDropdown_" aria-label="Mostrar acciones de tarea" aria-controls="navbarNavDropdown_">
                              <span class="navbar-toggler-icon_"><i style="color: white" class="fad fa-th-list"></i></span>
                          </button> 
                          <div class="collapse navbar-collapse row" id="navbarNavDropdown_">
                              <div  class="col-md-9 navbar-nav">
                                  <asp:Panel ID="Panel_devolver_tarea" CssClass="navbar-nav " runat="server" Visible="false" data-workflow-task-action="true">
                                     <li class="nav-item dropdown active ml-2 mr-0">  
                                         <a class="nav-link dropdown-toggle" style="color: #6d7fcc" title="Devuelve la tarea" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false" href="#">
                                             <i style="color: #0062cc" class="fad fa-arrow-alt-left"></i> Devolver
                                         </a>
                                         <div class="dropdown-menu" role="group" aria-label="Opciones para devolver la tarea">
                                             <a class="dropdown-item" title="Devolver la tarea directamente al usuario anterior" href="#" onclick="inicializa_tipo_adjunto_documento(event,this,'D-TWU-ANT');"><i style="color: #0062cc" class="fad fa-arrow-alt-left"></i><span>Usuario anterior</span></a>
                                             <a class="dropdown-item" title="Elegir la actividad anterior de destino" href="#" onclick="inicializa_tipo_adjunto_documento(event,this,'D-TASK-ANT');"><i style="color: #0062cc" class="fad fa-arrow-alt-to-left"></i><span>Elegir actividad anterior</span></a>
                                         </div>
                                      </li>            
                                  </asp:Panel>
                                  <asp:Panel ID="Panel_EnviarUsuario" CssClass="navbar-nav " runat="server" data-workflow-task-action="true">
                                      <ul class="navbar-nav">
                                          <li class="nav-item active ml-2">
                                              <a class="nav-link font-weight-light" style="color: #6d7fcc" title="Envía la tarea a usuario" href="#" onclick="activa_boton_client_server('ImageButtonEnviarUsuario');"><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-user"></i> Enviar a usuario  </a>
                                          </li>
                                      </ul>
                                  </asp:Panel>
                                  <asp:Panel ID="Panel_EnviaActividad" CssClass="navbar-nav " runat="server" data-workflow-task-action="true">
                                      <ul class="navbar-nav">
                                          <li class="nav-item active ml-2">
                                              <% If WorkflowCentroTrabajoModernActive Then %>
                                              <a id="workflow-group-send-trigger" class="nav-link font-weight-light" style="color: #6d7fcc" title="Envía la tarea a grupo de usuarios" aria-label="Enviar tarea a grupo" href="#"><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-user-friends"></i> Enviar a grupo</a>
                                              <% Else %>
                                              <a class="nav-link font-weight-light" style="color: #6d7fcc" title="Envía la tarea a grupo de usuarios" href="#" onclick="activa_boton_client_server('ImageButtonEnviaActividad');"><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-user-friends"></i> Enviar a grupo  </a>
                                              <% End If %>
                                          </li>
                                      </ul>
                                  </asp:Panel>
                                  <asp:Panel ID="Panel_enviar_flujo" CssClass="navbar-nav " runat="server" data-workflow-task-action="true">
                                      <ul class="navbar-nav">
                                          <li class="nav-item active ml-2 ">
                                              <a id="workflow-transition-trigger" class="nav-link font-weight-light ctw-btn" style="color: #6d7fcc" title="Continuar la tarea por ruta o flujo de trabajo" href="#" onclick="actualiza_titulo_lista_actividades_workflow('Enviar tarea'); activa_boton_client_server('ImageButtonterminar');" tabindex="0"><i class="fad fa-arrow-alt-right"></i><span>Continuar flujo</span></a>
                                          </li>
                                      </ul>
                                  </asp:Panel>
                                  <asp:Panel ID="Panel_autoterminar" CssClass="navbar-nav " runat="server" Visible="false" data-workflow-task-action="true">
                                      <ul class="navbar-nav">
                                          <li class="nav-item active ml-2">
                                              <a class="nav-link font-weight-light"  style="color: #6d7fcc" title="Envía la tarea a gestión de correspondencia" href="#" onclick="activa_boton_client_server('ImageButtonautoterminar');"><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-check"></i> Enviar a gestión  </a>
                                          </li>
                                      </ul>
                                  </asp:Panel>
                                  <asp:Panel ID="Panel_Buttonanotacion" CssClass="navbar-nav " runat="server" data-workflow-task-action="true">
                                      <ul class="navbar-nav">
                                          <li class="nav-item active ml-2 ">
                                              <a class="nav-link" href="#" title="Notas" aria-label="Abrir notas" onclick="activa_boton_client_server('ImageButtonanotacion');">
                                                  <i style="color: #0062cc" class="fad fa-sticky-note"></i>
                                                  <span id="nota_db">Notas</span>
                                              </a>
                                             
                                          </li>
                                      </ul>
                                  </asp:Panel>
                                  <asp:Panel ID="Panel_autoriza" CssClass="navbar-nav " runat="server" data-workflow-task-action="true">
                                      <li class="nav-item dropdown active ml-2 mr-0 ctw-authorize-menu">
                                          <div class="ctw-authorize-control" role="group" aria-label="Autorización de la tarea">
                                              <span class="ctw-authorize-check"><asp:CheckBox ID="CheckBox_auturiza" runat="server" aria-label="<% If WorkflowCentroTrabajoModernActive Then %>Cambiar estado de autorización de la tarea<% Else %>Marcar tarea como autorizada<% End If %>" onclick="prevent_autoriza_tarea(event, this);" /></span>
                                              <% If WorkflowCentroTrabajoModernActive Then %><span class="ctw-authorize-state-label">Autorizada</span><% End If %>
                                              <a class="nav-link dropdown-toggle" style="color: #6d7fcc" id="A11" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false" aria-controls="ctw-authorize-options" href="#"><% If WorkflowCentroTrabajoModernActive Then %>Historial<% Else %>Autorizar<% End If %></a>
                                              <div id="ctw-authorize-options" class="dropdown-menu" role="group" aria-label="<% If WorkflowCentroTrabajoModernActive Then %>Historial de autorizaciones<% Else %>Opciones de autorización<% End If %>">
                                                  <a href="#" class="dropdown-item" onclick="activa_boton_client_server('ImageButton_ista_autorizacio');"><i style="color: #0062cc" class="fad fa-list-ul"></i><span>Lista de autorizaciones</span></a>
                                              </div>
                                          </div>
                                      </li>          
                                  </asp:Panel>
                                  <asp:Panel ID="Panel_info_tarea" CssClass="navbar-nav " runat="server" data-workflow-task-action="true">
                                      <ul class="navbar-nav">
                                          <li class="nav-item active ml-1">

                                              <a class="nav-link font-weight-light" href="#" title="Detalle del radicado" aria-label="Abrir detalle del radicado" onclick=" activa_boton_client_server('Button_tool_activa_detalle_radicado_seleccion');">
                                                   <i style="margin-left: 1px; margin-top: 7px; color: #6d7fcc" title="Detalle del radicado" class="fad fa-info-square"></i>
                                              </a>       
                                          </li>
                                      </ul>
                                 </asp:Panel>                
                              </div>
                              <div class="col-md-3  navbar-nav justify-content-end">
                                  <asp:Panel ID="Panel_show_hide" CssClass="navbar-nav " Style="float:right" runat="server">
                                       <ul class="navbar-nav">
                                           
                                          <li id="hide_selec_tarea" style="display:none" class="nav-item active ml-0" data-workflow-task-toggle="true">
                                              <a class="nav-link" href="#">
                                              <span class=" font-weight-bold" style=" margin-left:0px; margin:2px; width:auto; color:#0062cc;" title="Mostrar la lista de tareas en espera"  onclick="hide_area_workflow_seleccion();"><i id="indice_title" style="margin-left: 1px; margin-top: 7px; color: #0062cc"  class="fad fa-list fa-1x"></i> Tareas</span> 
                                              </a>
                                          </li>
                                           <li id="show_selec_tarea"  class="nav-item active ml-0" style="display:none" data-workflow-task-toggle="true">
                                               <a class="nav-link" href="#">
                                                 <span class="font-weight-bold"  style=" margin-left:0px; margin:2px; width:auto; color:#0062cc;" title="Mostrar la tarea seleccionada"   onclick="show_area_workflow_seleccion()"><i id="i1" style="margin-left: 1px; margin-top: 7px; color: #0062cc"  class="far fa-window-alt fa-1x"></i> Mostrar tarea </span> 
                                              </a>
                                          </li>
                                           <li id="pendiente_selec_tarea" style="display:none" class="nav-item active ml-0" data-workflow-task-action="true">
                                               <a class="nav-link ctw-btn" href="#" title="Cerrar tarea seleccionada" aria-label="Cerrar tarea seleccionada" onclick="inicializa_tipo_adjunto_documento(event,this,'E-ETP');" tabindex="0">
                                                 <i id="i2" class="fad fa-check-circle" aria-hidden="true"></i><span id="span_pendiente_selec_tarea">Cerrar tarea</span>
                                               </a>
                                          </li>
                                      </ul>
                                  </asp:Panel>
                                  
                              </div>
                          </div>  
                      </nav>    
                  </ContentTemplate>
              </asp:UpdatePanel>
              <div style="display:none">   
                  <asp:UpdatePanel ID="UpdatePanel_tool_menu" runat="server" UpdateMode="Conditional">
                      <ContentTemplate>
                           <asp:Button ID="Button_activa_estado_paginacion" runat="server" Text="Button" />
                           <asp:Button ID="Button_activa_search" runat="server" Text="Button" />
                           <asp:Button ID="Button_activa_copiar_estructura" runat="server" Text="Button" style="display:none"   />
                           <asp:Button ID="Button_activa_copiar_expediente" runat="server" Text="Button" style="display:none"   />
                           <asp:Button ID="Button_activa_incorpora_expediente" runat="server" Text="Button" style="display:none"   />
                           <asp:ImageButton ID="ImageButtonactualizar" runat="server"  Width="0px" Height="0px"   />
                           <asp:ImageButton ID="ImageButtonpendiente" runat="server" style="display:none"/>
                           <asp:ImageButton ID="ImageButtonanotacion" runat="server"   Width="0px" Height="0px" style="margin-left:0px; display:none" />
                           <asp:ImageButton ID="ImageButtonanotacion_" runat="server" />
                           <asp:ImageButton ID="ImageButtonautoterminar" runat="server"  Width="0px" Height="0px" style="display:none" />
                           <asp:ImageButton ID="ImageButtonestadograficotrazabilida" runat="server"  Width="0px" Height="0px"   style="display:none"  />
                           <asp:ImageButton ID="ImageButtonterminar" runat="server"  Width="0px" Height="0px"   style="display:none"  />
                           <asp:ImageButton ID="ImageButtonEnviaActividad" runat="server"  Width="0px" Height="0px" style="display:none" CssClass="alterna_image"  OnClientClick="cambia_estado_boton_reasignar('FALSE');" />
                           <asp:ImageButton ID="ImageButtonEnviarUsuario" runat="server"  Width="0px" Height="0px" style="display:none"  OnClientClick="cambia_estado_boton_reasignar('VISIBLE');"/>    
                           <asp:ImageButton ID="ImageButtonseleccionar" runat="server"  Width="0px" Height="0px" style="display:none" CssClass="alterna_image" /> 
                           <asp:ImageButton ID="ImageButton_pendiente_aprobacion" runat="server" Width="0px" Height="0px" style="display:none"    /> 
                           <asp:ImageButton ID="ImageButton_autorizar" runat="server" Style="display: none" />
                           <asp:ImageButton ID="ImageButton_desautoriza" runat="server" Style="display: none" />
                           <asp:ImageButton ID="ImageButton_ista_autorizacio" runat="server" style="display:none"    />
                           <asp:ImageButton ID="ImageButton_ista_autorizacio_" runat="server" />
                          <asp:Button ID="Button_guardar_desicion_fre_image" runat="server" Text="Button" Style="display: none" />
                           <input id="Hidden_lista_ruta_flujo" type="hidden" value="" runat="server"/>
                           <input id="Hidden_activa_popup" type="hidden" value="" runat="server" />
                           <input id="Hidden_vi_reasigna" type="hidden" value="" runat="server" />
                            <input id="Hidden_estado_tareas_pendiente" type="hidden" value="NO" runat="server" />
                           <input id="Hidden_estado_anotacion" type="hidden" value="NO" runat="server" />
                           <input id="Hidden_intervalo_search" type="hidden" value="-1" runat="server" />
                           <input id="Hidden_estado_pendiente_aprobacion" type="hidden" value="NO" runat="server" />
                      </ContentTemplate>
                  </asp:UpdatePanel>  
              </div>
             
          </div>
              <div id="contenido_lista_tareas" data-workflow-task-list="true"
              style="position: inherit; left: auto; width: 100%; height: 99%; margin: 0px 0px 0px 0px; background-color:white; display:none">
                   <div id="div_label_title_tareas" class="row p-1" >
                       <div class="col-7">
                           <div class="row">
                           <div class="col3">
                                <asp:UpdatePanel ID="UpdatePanelnumeroespera" runat="server" UpdateMode="Conditional">
                               <ContentTemplate>
                                   <asp:Label ID="LabelEspera" runat="server" Text=""  CssClass="h6 ml-3" Style="color:#0062cc" data-workflow-task-count="true"></asp:Label>
                                   <input id="Hidden_00005_2222" type="hidden" value="" runat="server"/>
                               </ContentTemplate>
                           </asp:UpdatePanel>
                           </div>
                           <div class="col6">
                            <asp:UpdatePanel ID="UpdatePanelseleccionfiltro" runat="server" UpdateMode="Conditional">
                               <ContentTemplate>        
                                   <asp:DropDownList ID="DropDownListseleccionfiltro" runat="server" CssClass="dropdown ml-3" AutoPostBack="true"></asp:DropDownList>
                               </ContentTemplate>
                           </asp:UpdatePanel>
                           </div>
                           <div class="col-3">
                               <button class="btn btn-outline-secondary" onclick="preven_event_search_new_task(event,this)" title="Buscar nuevas tareas" type="button">
                                    <i class="fal fa-sync-alt"></i>
                                </button>
                               <button class="btn btn-outline-secondary" id="bnt_search_avance" onclick="event_element_clic(event,this)" title="Busqueda avanzada" type="button">
                                    <i class="fal fa-search-plus"></i>
                               </button>
                           </div>
                         </div>
                       </div>
                       <div class=" float-md-right col-md-5 float-sm-left">
                        <div class="input-group ">
                             <button id="td-boton" class="btn btn-outline-secondary border-right-2 " style="border-top-right-radius: 0px; border-bottom-right-radius: 0px" title="Restaurar lista de tareas" onclick="preven_event_restor_search(event,this)" type="button">
                                <i class="fal fa-long-arrow-left"></i>
                            </button>
                            <asp:TextBox ID="auto_complex" runat="server" class="form-control form-control-sm complex " placeholder="Busqueda...." ></asp:TextBox>
                            <div class="input-group-append">
                                <button class="btn btn-outline-secondary" onclick="preven_event_search(event,this)" title="consultar lista" type="button">
                                    <i class="fal fa-search"></i>
                                </button>
                            </div>
                        </div>
                    </div>      
                   </div>  
              <div id="contenedor_tab" style="height: 60%;" class="">
                  <asp:Panel ID="Panelactividad" runat="server" Style="overflow: auto; height: 400px"
                      Enabled="true">
                      <asp:UpdatePanel ID="UpdatePanel1" runat="server"
                          UpdateMode="Conditional">
                          <ContentTemplate>
                              <asp:GridView ID="GridView2" class="table font-weight-light ml-1 gridview-moderno" PagerSettings-Position="Top" AllowSorting="true" AllowPaging="true" PageSize="7" runat="server" Style="width: 99.5%; font-family: Segoe UI; font-size: 14px" EnableViewState="true"
                                  AutoGenerateSelectButton="False" GridLines="None">
                                  <RowStyle />
                                  <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                  <HeaderStyle CssClass="GridviewScrollHeader_line_boot" />
                                  <RowStyle CssClass="" />
                                  <PagerStyle CssClass="GridviewScrollPager_line" />
                                  <PagerStyle CssClass="pagination-ys" />
                                  <Columns>
                                      <asp:BoundField HeaderText="OPCIONES" />
                                  </Columns>
                              </asp:GridView>

                              <input id="Hidden_cantidad_registros" type="hidden" value="0" runat="server" />
                          </ContentTemplate>
                          <Triggers>
                              <asp:AsyncPostBackTrigger ControlID="Buttonactividad" EventName="Click" />

                              <asp:AsyncPostBackTrigger ControlID="ImageButtonactualizar" EventName="Click" />
                          </Triggers>
                      </asp:UpdatePanel>
                  </asp:Panel>
              </div>
                  <div id="contenido_botonoes" style="width: 100%" class="p-2  modal_content_no_back_superior">
                     
                  </div>                          
          </div>
              <div id="content_selecion_tarea" style="width:100%; display:none" data-workflow-task-context="true">
                  <div id="error_div_selecion_tarea_wf" style="position: relative; width: 100%"></div>
                  <div id="content_seleccion_documentos" style="width: 25%; position: relative; left: auto; float: left; height: 100%;" class="modal_content_no_back_rigth modal_content_no_back_inferior">
                      <div id="div_label" class="row p-0 m-0 pt-2 pb-2 modal_content_no_back_inferior documentos-relacionados-titulo">
                          <div class="col-1 pt-2 pl-l3" >
                              <input classs="btn   btn-light btn-sm border-0 bg-transparent" type="checkbox" aria-label="Seleccionar todos los documentos" title="Seleccionar todos los documentos" onclick="table_gred_on_click_check(this,'GridView_list_documento_relacion_wf','chek_selecion_list_wf');" />
                          </div>
                          <div class="col-5 pl-1">
                              <asp:UpdatePanel ID="UpdatePanel_label_seleccion" runat="server" UpdateMode="Conditional" class="col-12_" RenderMode="Inline">
                                  <ContentTemplate>
                                      <asp:Label ID="Label_docu_relacionado_wf" runat="server" Text="Documentos (0)" Style="color: #0062cc; float: left; font-family: 'Segoe UI'" CssClass="h8 mt-1 pl-1  font-weight-light"></asp:Label>
                                  </ContentTemplate>
                              </asp:UpdatePanel>
                          </div>
                          <div class="col-4 pr-1 pl-0">
                              <a class="nav-link pr-2 pl-2" style="color: #6d7fcc;  float: right" title="Actualiza indice batch" href="#" onclick="inicializa_tipo_adjunto_documento(event,this,'C-DW-ACTU-INDICE')"><i style="" class="fal fa-info "></i><% If WorkflowCentroTrabajoModernActive Then %><span class="ctw-document-action-label">Actualizar índice</span><% End If %></a>
                              <a class="nav-link pr-2 pl-2" id="btnLoadFile" style="color: #6d7fcc;  float: right" title="Adjuntar documento" href="#" ><i style="" class="fal fa-upload "></i><% If WorkflowCentroTrabajoModernActive Then %><span class="ctw-document-action-label">Cargar</span><% End If %></a>
                              <a class="nav-link pr-2 pl-2" id="btnloadservice" style="color: #6d7fcc;  float: right" title="Adjuntar documentos desde servicio web" href="#" ><i style="" class="fal fa-page-break "></i><% If WorkflowCentroTrabajoModernActive Then %><span class="ctw-document-action-label">Servicio</span><% End If %></a>
                          </div> 
                          <div class="col-1 pr-0 d-flex justify-content-end">
                              <div class="dropright">
                                  <button type="button" class="mt-1 btn btn-light dropdown-toggle ctw-document-more-toggle" data-toggle="dropdown" aria-expanded="false" aria-haspopup="true" aria-controls="ctw-document-actions-menu" aria-label="<% If WorkflowCentroTrabajoModernActive Then %>Acciones de documentos<% Else %>Acciones para documentos seleccionados<% End If %>" title="<% If WorkflowCentroTrabajoModernActive Then %>Acciones de documentos<% Else %>Acciones para documentos seleccionados<% End If %>">
                                      <% If WorkflowCentroTrabajoModernActive Then %>
                                      <span class="ctw-document-more-actions-label">Acciones</span>
                                      <% End If %>
                                      <span class="sr-only"><% If WorkflowCentroTrabajoModernActive Then %>Acciones de documentos<% Else %>Acciones para documentos seleccionados<% End If %></span>
                                  </button>
                                  <div id="ctw-document-actions-menu" class="dropdown-menu" role="group" aria-label="<% If WorkflowCentroTrabajoModernActive Then %>Acciones de documentos<% Else %>Acciones para documentos seleccionados<% End If %>">
                                      <% If WorkflowCentroTrabajoModernActive Then %>
                                      <span class="ctw-menu__section-label">Agregar documentos</span>
                                      <a id="ctw-document-action-attach-list" class="dropdown-item font-weight-light" href="#" title="Adjuntar documento a la lista" onclick="inicializa_tipo_adjunto_documento(event,this,'C-DW-ENL')"><i style="color: #6d7fcc" class="fad fa-upload"></i><span class="font-weight-light"> Adjuntar a la lista </span></a>
                                      <a id="ctw-document-action-service" class="dropdown-item font-weight-light" href="#" title="Adjuntar documento desde servicio web" onclick="inicializa_tipo_adjunto_documento(event,this,'C-DW-AUTO')"><i style="color: #6d7fcc" class="fad fa-page-break"></i><span class="font-weight-light"> Cargar desde servicio </span></a>
                                      <a id="ctw-document-action-digitize" class="dropdown-item font-weight-light" href="#" title="Adjuntar documento digitalizado" onclick="prevent_tool_menucab(event,this,'D-DNDT')"><i style="color: #6d7fcc" class="fad fa-scanner-image"></i><span class="font-weight-light"> Adjuntar digitalizado </span></a>
                                      <div class="dropdown-divider"></div>
                                      <span class="ctw-menu__section-label">Documento y selección</span>
                                      <% End If %>
                                      <a class="dropdown-item font-weight-light"   href="#" onclick="inicializa_tipo_adjunto_documento(event,this,'C-DW-DEL-IMAGE')"><i style="color: #6d7fcc" class="fad fa-trash-alt"></i><span class="font-weight-light"> Eliminar documentos seleccionados </span> </a>
                                      <a class="dropdown-item font-weight-light" id="boton_menu_stamp_firm"  href="#"><i style="color: #6d7fcc" class="fad fa-file-signature"></i><span class="font-weight-light"> Firmar documentos seleccionados </span> </a>
                                      <div class="dropdown-divider"></div> 
                                      <% If WorkflowCentroTrabajoModernActive Then %>
                                      <span class="ctw-menu__section-label">Compartir</span>
                                      <a id="ctw-document-action-share-users" class="dropdown-item font-weight-light" href="#" onclick="prevent_tool_menucab(event,this,'D-CDW')"><i style="color: #6d7fcc" class="fad fa-share-square"></i><span class="font-weight-light"> Compartir con usuarios </span></a>
                                      <a id="ctw-document-action-share-email" class="dropdown-item font-weight-light" href="#" onclick="prevent_tool_menucab(event,this,'D-CEDTS')"><i style="color: #6d7fcc" class="fad fa-envelope"></i><span class="font-weight-light"> Compartir por correo </span></a>
                                      <div class="dropdown-divider"></div>
                                      <span class="ctw-menu__section-label">Gestionar selección</span>
                                      <% End If %>
                                      <a id="a_copy_document_production_proceedings"  class="dropdown-item font-weight-light"  href="#" ><i style="color: #6d7fcc" class="fad fa-folder-tree"></i><span class=" font-weight-light"> Copiar los documentos seleccionados a su archivo de producción documental </span>  </a>
                                      <a id="a_copy_document_proceedings"  class="dropdown-item font-weight-light"  href="#" ><i style="color: #6d7fcc"  class="fad fa-copy"></i><span class=" font-weight-light"> Copiar los documentos seleccionados al expediente </span> </a>
                                      <a id="a_link_document_proceedings"  class="dropdown-item font-weight-light"  href="#" ><i style="color: #6d7fcc" class="fad fa-folders"></i><span class=" font-weight-light"> Vincular los documentos seleccionados al expediente </span> </a>
                                      <a id="a_auto_link_document_proceedings"  class="dropdown-item font-weight-light"  href="#" ><i style="color: #6d7fcc"  class="fad fa-folder-download"></i><span class=" font-weight-light"> Crea un expediente de manera automática y vincula los documentos seleccionados </span> </a>
                                  </div>
                                 
                              </div>

                          </div>
                      </div>
                   
                      <div id="seleccion" style="width: 100%; float: left; height: 15%; position:relative" class="bg-light">
                          <asp:Panel ID="Panel_scroll" runat="server" ScrollBars="Auto" Style="height: 150px; background-color: white" class="modal_content_no_back_inferior contenedor_scroll">
                              <asp:UpdatePanel ID="UpdatePanelseleccion" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                  <ContentTemplate>
                                          <input id="Hidden_selccion_documento_eliminar_wf" type="hidden" value="" runat="server" />
                                          <input id="Hidden_selccion_documento_eliminar_split_wf" type="hidden" value="" runat="server" />
                                          <input id="Hidden_selccion_documento_cambia_tipo_wf" type="hidden" value="" runat="server" />
                                          <input id="Hidden_selccion_documento_cambia_tipo_split_wf" type="hidden" value="" runat="server" />
                                          <input id="hiden_seleccion_documento_wf" type="hidden" value="" runat="server" />
                                          <input id="hiden_seleccion_documento_id_wf" type="hidden" value="" runat="server" />
                                          <input id="Hidden_numero_doc_rel_wf" type="hidden" value="0" runat="server" />
                                            <asp:GridView ID="GridView_list_documento_relacion_wf" runat="server" Style="position: inherit; width: 100%; font-size: 14px"
                                          AutoGenerateSelectButton="False" AllowSorting="false" AllowPaging="false" PageSize="6" PagerSettings-Position="Top" CssClass="Gridviewtable  font-weight-light" GridLines="None"
                                          EnableViewState="true">
                                          <RowStyle CssClass="GridviewRow" />
                                          <SelectedRowStyle  BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                          <HeaderStyle CssClass="GridviewScrollHeader_line_boot_none" />
                                          <PagerStyle CssClass="pagination-ys" />
                                         <AlternatingRowStyle CssClass="GridviewRow"/>
                                          <Columns>
                                              <asp:TemplateField>
                                                  <HeaderTemplate>
                                                      
                                                  </HeaderTemplate>
                                                  
                                              </asp:TemplateField>
                                          </Columns>
                                      </asp:GridView>
                                      <div style="display: none">
                                          <asp:TreeView ID="TreeViewseleccion" runat="server" NodeWrap="true" CssClass="font-weight-light h6 pl-0 pt-3" Style="font-family: 'Segoe UI'"
                                              NodeStyle-NodeSpacing="0.1px" Height="16px"
                                              LeafNodeStyle-CssClass="LeafNodeStyle_2_  mb-1 pl-1  " ExpandDepth="0" NodeIndent="1" CollapseImageUrl="../workflow/imageneswf/folder-open-light.png" ExpandImageUrl="../workflow/imageneswf/folder-light.png" PopulateNodesFromClient="False">
                                              <HoverNodeStyle Font-Underline="True" />
                                              <SelectedNodeStyle CssClass="select_treview_boottra font-weight-normal nav-link-treview" />
                                              <ParentNodeStyle Font-Bold="False" />
                                              <HoverNodeStyle Font-Underline="True" ForeColor="Purple" />
                                              <NodeStyle CssClass="nav-link-treview mt-2 mb-2 pl-2  " ForeColor="#0062cc"
                                                  VerticalPadding="0px" />
                                              <RootNodeStyle />
                                          </asp:TreeView>
                                      </div>
                                      <asp:Button ID="Button_actualiza_trevie_seleccion" runat="server" Text="Button" Style="display: none" />        
                                      <input id="Hiddenint" type="hidden" value="0" runat="server" />
                                  </ContentTemplate>
                              </asp:UpdatePanel>   
                          </asp:Panel>
                           
                      </div>
                       <div id="content_boton_gestion" style=" border-top-left-radius: initial; border-top-right-radius: initial" class="modal-header_ modal_title_superior   modal_content_no_back_inferior " >
                          
                       </div>
                  </div>
                 
                  <a id="da_show-sidebar_" class="btn btn-sm   show_da_slide_rigth  " title="Visualiza indice" style="top: 50%" href="#" data-target="#sidebar__">
                      <i style="color: white" class="fas fa-bars"></i>
                  </a>
                 <div id="contenido_indice"
                      style="width: 20%; position: inherit; left: auto; height: 100%; margin: 0px 0px 0px 0px; float: right; background-color: white; display:none" class="modal_content_no_back_left">
                      <div id="div_conent_indice">
                          <div id="title_indice" class="modal-header_ modal_title_superior  p-2 modal_content_no_back_inferior"  style=" border-top-left-radius: initial; border-top-right-radius: initial">
                              <h6 class=" mt-2 mb-2 ml-2 font-weight-normal" id="pit_" style="color: #0062cc; float: left; font-family: 'Segoe UI'">Indice </h6>
                              <a id="sidebarCollapse" class="close_ mr-2" title="Oculta indice" style="float: right; height: 10px; color: #0062cc"><i class="fal fa-times   font-weight-light"></i></a>
                          </div>
                          <asp:UpdatePanel ID="UpdatePanelindice" runat="server" UpdateMode="Conditional"
                              RenderMode="Inline">
                              <ContentTemplate>
                                  <asp:Panel ID="Panel_indice" runat="server" ScrollBars="Auto" CssClass="pl-1"
                                      Height="98%" EnableViewState="true" >
                                  </asp:Panel>
                                  <asp:Panel ID="div_buton" runat="server" Style="text-align: center; background-color: white" Visible="false"  CssClass="modal_content_no_back_inferior" >
                                      <ul class="navbar-nav">
                                          <li class="nav-item active  active_">
                                              <a class="nav-link" id="a_lement_actualiza_index" href="#" onclick="event_element_clic(event,this);">
                                                  <i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-save"></i>
                                                  <span id="Span1" style="color: #6d7fcc" title="Guardar los cambios" > Guardar  </span>
                                              </a>
                                          </li>
                                      </ul>
                                  </asp:Panel>
                                  <input id="Hiddenheih" type="hidden" value="0" runat="server" class="dec_000_21_000"/>
                                  <input id="Hidden_image_gabinete" type="hidden" value="0" runat="server" class="dec_000_21_000"/>
                                  <input id="Hidden_id_inventario" type="hidden" value="0" runat="server" class="dec_000_21_000"/>
                                  <input id="Hidden_id_serie" type="hidden" value="0" runat="server" class="dec_000_21_000"/>
                                  <input id="Hidden_id_sub_serie" type="hidden" value="0" runat="server" class="dec_000_21_000"/>
                                  <input id="Hidden_id_documento" type="hidden" value="0" runat="server" class="dec_000_21_000"/>
                                  <input id="Hidden_id_area" type="hidden" value="0" runat="server" class="dec_000_21_000"/>
                                  <input id="Hidden_id_tipo" type="hidden" value="0" runat="server" class="dec_000_21_000"/>
                                  <input id="Hidden_id_expediente" type="hidden" value="0" runat="server" class="dec_000_21_000"/>
                                  <input id="Hidden_id_tipo_expediente" type="hidden" value="0" runat="server" class="dec_000_21_000"/>
                                  <input id="Hidden_id_unidad_conservacion" type="hidden" value="0" runat="server" class="dec_000_21_000"/>
                                  <input id="Hidden_id_tipo_unidad_conservacion" type="hidden" value="0" runat="server" class="dec_000_21_000"/>
                              </ContentTemplate>
                              <Triggers>
                              </Triggers>
                          </asp:UpdatePanel>
                          <asp:UpdatePanel ID="Updatepanel_actualiza" runat="server" UpdateMode="Conditional">
                              <ContentTemplate>
                                  <asp:Button ID="Button_actualiza_indice_imagen" runat="server" Text="Actualiza Indice" CssClass="btn btn-success" ToolTip="Actualiza Indice documento" Style="display: none" />
                              </ContentTemplate>
                          </asp:UpdatePanel>

                      </div>
                  </div>
                 <div id="contenido_imagen"
                      style="width: 75%; position: relative; left: auto; float: left; height: 100%; margin: 0px 0px 0px 0px">
                      <asp:UpdatePanel ID="UpdatePanel_content_iframe" runat="server" UpdateMode="Conditional"
                          RenderMode="Inline">
                          <ContentTemplate>
                              <asp:Panel ID="panel_content_iframe" runat="server" Visible="false">
                                  <asp:UpdatePanel ID="UpdatePanel_panel_toll" runat="server" UpdateMode="Conditional"
                                      RenderMode="Inline">
                                      <ContentTemplate>
                                          <asp:Panel ID="Panel_tolbar_pdf" runat="server" Visible="false" Style="display: inline-flexbox; width: 100%; height: auto" class="navbar navbar-expand-sm  p-2  pb-1 pl-0 pt-1 modal_content_no_back_inferior">
                                              <div class="nav  ml-1">
                                                  <div id="Div5" class="nav-item_ active active_" style="display:none">
                                                      <a class="nav-link " style="color: #0062cc; font-family: Arial; text-decoration: none; font-weight: 600" title="Adjuntar documento" href="#" onclick="inicializa_tipo_adjunto_documento(event,this,'C-DW-VIS')"><i style="color: #0062cc" class="fas fa-upload "></i></a>
                                                  </div>
                                                  <div class="nav-item_ active active_" style="display:none">
                                                      <a id="A10" class="nav-link " style="color: #0062cc; font-family: Arial; text-decoration: none; font-weight: 600" title="Adjuntar documento desde servicio web" href="#" onclick="inicializa_tipo_adjunto_documento(event,this,'C-DW-AUTO')"><i style="color: #0062cc" class="fas fa-page-break"></i></a>
                                                  </div>
                                                  <div class="nav-item_ active active_">
                                                    <% If WorkflowCentroTrabajoModernActive AndAlso WorkflowCentroTrabajoSelectedDocumentAvailable Then %>
                                                    <span class="ctw-viewer-document-context">
                                                        <strong class="ctw-viewer-document-context-title"><%= WorkflowCentroTrabajoSelectedDocumentTitle %></strong>
                                                        <span class="ctw-badge ctw-viewer-document-context-format"><%= WorkflowCentroTrabajoSelectedDocumentFormat %></span>
                                                        <span class="ctw-viewer-document-actions">
                                                            <% If WorkflowCentroTrabajoSelectedDocumentMetadataAvailable Then %>
                                                            <a id="id_indice_wf_pdf" class="nav-link ctw-document-metadata-action" title="Ver metadatos del documento" href="#" onclick="event_element_clic(event,this);"><i class="fad fa-file-invoice"></i><span class="ctw-document-action-label">Metadatos</span></a>
                                                            <% End If %>
                                                        </span>
                                                    </span>
                                                    <% Else %>
                                                    <a id="id_indice_wf_pdf" class="nav-link d-none" style="color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600" title="Ver meta datos del documento" href="#" onclick="event_element_clic(event,this);"><i style="" class="fas fa-file-invoice"></i></a>
                                                    <% End If %>
                                                  </div>
                                              </div>
                                          </asp:Panel>
                                          <asp:ImageButton ID="ImageButtonadjunta" runat="server" Style="display: none" />
                                          <asp:ImageButton ID="ImagenAdjuntaAutomatico" runat="server" Style="display: none" />
                                      </ContentTemplate>
                                  </asp:UpdatePanel>
                                  <asp:UpdatePanel ID="UpdatePanelVisor" runat="server" UpdateMode="Conditional"
                                      RenderMode="Inline">
                                      <ContentTemplate>
                                          <iframe id="ifrm_visor_" runat="server" data-workflow-task-viewer="true" style="border-style: none; left: 0px; width: 100%; height: auto; position: relative; top: 1px; z-index: auto; right: 1px"
                                              frameborder="0" scrolling="no"></iframe>

                                      </ContentTemplate>
                                      <Triggers>
                                      </Triggers>
                                  </asp:UpdatePanel>
                                  <!--guarda el alto del ifrmviso-->
                                  <input id="HiddenHeigth" type="hidden" value="560" runat="server" />
                                  <div id="oculto" style="display: none; width: 5px;">
                                      <asp:UpdatePanel ID="UpdatePanel_actualiza_tab" runat="server" UpdateMode="Conditional"
                                          RenderMode="Inline">
                                          <ContentTemplate>
                                              <asp:Button ID="Buttonuser" runat="server" Height="0px" Visible="True"
                                                  Width="0px" />
                                              <asp:Button ID="Buttonactividad" runat="server" Height="0px" Visible="True"
                                                  Width="0px" />
                                              <asp:Button ID="ButtonTreeviewSeleccion" runat="server" Height="0px" Visible="True"
                                                  Width="0px" />
                                          </ContentTemplate>
                                      </asp:UpdatePanel>



                                  </div>
                              </asp:Panel>
                          </ContentTemplate>

                      </asp:UpdatePanel>
                      
                      <asp:UpdatePanel ID="UpdatePanel_content_image_draw" runat="server" UpdateMode="Conditional"
                          RenderMode="Inline">
                          <ContentTemplate>
                              <asp:Panel ID="panel_content_image_draw" runat="server" Visible="false" style="height:100%">
                                  <div id="tollimage" style="border-bottom: 1px solid #ddd; display: inline-flexbox; width: 100%" class="navbar navbar-expand-sm   pb-0 pl-0 pt-1">
                                      <button class="navbar-toggler btn btn-light " style="padding-bottom: 2px" type="button" data-toggle="collapse" data-target="#navbarNavDropdown">
                                          <span class="pb-1"><i style="color: white" class="fas fa-bars"></i></span>
                                      </button>
                                      <div class="collapse navbar-collapse   pb-1 pt-0" id="navbarNavDropdown__">
                                          <div class="nav  ml-1">
                                              <div class="nav-item active active_ ">
                                                  <a class="nav-link" style="color: #6d7fcc" title="Primera  imagen" href="#" onclick="activa_boton_client_server('ImageButtonInicio')"><i style="font-size: 20px" class="fad fa-arrow-alt-to-left  "></i></a>
                                              </div>
                                              <div class="nav-item active active_ ">
                                                  <a class="nav-link  " style="margin-left: 2px; margin: 2px; width: auto; color: #6d7fcc" title="Anterior imagen" href="#" onclick="activa_boton_client_server('ImageButtonAnterior')"><i style="font-size: 20px" class="fad fa-arrow-alt-left "></i></a>
                                              </div>
                                              <asp:UpdatePanel ID="UpdatePanel_conte_bot" runat="server"
                                                  UpdateMode="Conditional" RenderMode="Inline">
                                                  <ContentTemplate>
                                                      <div class="nav-item active ">
                                                          <asp:TextBox ID="LabelConteo" runat="server" Style="margin-left: 5px; margin-right: 5px; text-align: center; margin-top: 3px; font-size: 12px; width: 50px; font-family: 'Segoe UI Emoji'" onkeypress="preven_event_search_keypres_enter(event,this);"></asp:TextBox>
                                                      </div>
                                                  </ContentTemplate>
                                              </asp:UpdatePanel>
                                              <div class="nav-item active  active_">
                                                  <a class="nav-link " style="color: #6d7fcc" title="Siguiente imagen" href="#" onclick="activa_boton_client_server('ImageButtonSiguiente')"><i style="font-size: 20px" class="fad fa-arrow-alt-right "></i></a>
                                              </div>

                                              <div class="nav-item active active">
                                                  <a class="nav-link " style="color: #6d7fcc" title="Ultima  imagen" href="#" onclick="activa_boton_client_server('ImageButtonFinal')"><i style="font-size: 20px" class="fad fa-arrow-alt-to-right "></i></a>
                                              </div>

                                              <div class="nav-item active active_">
                                                  <a class="nav-link " style="color: #6d7fcc" title="Alejar Imagen" href="#" onclick="activa_boton_client_server('ImageMenos')"><i style="font-size: 20px" class="fad fa-minus-circle "></i></a>
                                              </div>

                                              <div class="nav-item active active_">
                                                  <a class="nav-link " style="color: #6d7fcc" title="Acercar Imagen" href="#" onclick="activa_boton_client_server('ImageMas') "><i style="font-size: 20px" class="fad fa-plus-circle "></i></a>
                                              </div>
                                              <asp:UpdatePanel ID="UpdatePanel_drows_bot" runat="server"
                                                  UpdateMode="Conditional" RenderMode="Inline">
                                                  <ContentTemplate>
                                                      <asp:DropDownList ID="DropDownList_zom" runat="server" AutoPostBack="True" class=" mr-1 ml-1">
                                                          <asp:ListItem Value="50"></asp:ListItem>
                                                          <asp:ListItem>20</asp:ListItem>
                                                          <asp:ListItem>30</asp:ListItem>
                                                          <asp:ListItem>40</asp:ListItem>
                                                          <asp:ListItem>50</asp:ListItem>
                                                          <asp:ListItem>60</asp:ListItem>
                                                          <asp:ListItem>70</asp:ListItem>
                                                          <asp:ListItem>80</asp:ListItem>
                                                          <asp:ListItem>90</asp:ListItem>
                                                          <asp:ListItem>100</asp:ListItem>
                                                      </asp:DropDownList>
                                                  </ContentTemplate>
                                                  <Triggers>
                                                      <asp:AsyncPostBackTrigger ControlID="ImageMenos" EventName="Click" />
                                                      <asp:AsyncPostBackTrigger ControlID="ImageMenos" EventName="Click" />
                                                  </Triggers>
                                              </asp:UpdatePanel>

                                              <div class="nav-item_ active active_">
                                                  <a class="nav-link " style="color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600" title="Rotar 45 grados a la izquierda" href="#" onclick="activa_boton_client_server('ImageRotate45')"><i style="" class="fad fa-undo "></i></a>
                                              </div>
                                              <asp:TextBox ID="TextBox2" Style="display: none" class=" mr-0 ml-0" Width="74px" runat="server" placeholder="" onkeypress="preven_event_search_keypres_enter(event,this);"></asp:TextBox>
                                              <div class="nav-item_ active active_" style="display: none">
                                                  <a class="nav-link " style="color: #6d7fcc" title="Ir a imagen" href="#" onclick="activa_boton_client_server('ImageButton_ir_pagina')"><i style="color: white; font-size: 20px" class="fas fa-search "></i></a>
                                              </div>
                                              <div class="nav-item_ active active_">
                                                  <a class="nav-link " id="ImageButtonguardardocumento_" style="color: #6d7fcc" title="Descargar imagenes" href="#" onclick="activa_boton_client_server('ImageButtonguardardocumento')"><i style="font-size: 20px" class="fad fa-arrow-to-bottom "></i></a>
                                              </div>
                                              <div class="nav-item_ active active_">
                                                  <a class="nav-link " style="color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600" title="Imprimir documento" href="#" onclick="activa_boton_client_server('ImageButtonimprimir')"><i style="" class="fad fa-print "></i></a>
                                              </div>
                                              <div id="ImageFirma_" class="nav-item_ active active_">
                                                  <a class="nav-link " style="color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600" title="Firma Imagen" href="#" onclick="firma_mecanica();"><i style="" class="fas fa-file-signature "></i></a>
                                              </div>
                                              <div id="ImageButtonadjunta_" class="nav-item_ active active_">
                                                  <a class="nav-link " style="color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600" title="Adjuntar documento" href="#" onclick="inicializa_tipo_adjunto_documento(event,this,'C-DW-VIS')"><i style="" class="fas fa-upload "></i></a>
                                              </div>         
                                               <div class="nav-item_ active active_">
                                                  <a id="id_indice_wf" class="nav-link" style="color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600" title="Adjuntar documento desde servicio web" href="#" onclick="inicializa_tipo_adjunto_documento(event,this,'C-DW-AUTO')"><i style="" class="fas fa-page-break"></i></a>
                                              </div>
                                               <div class="nav-item_ active active_">
                                                    <a id="id_indice_wf_pdf_draw" class="nav-link" style="color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600" title="Ver meta datos de documento" href="#" onclick="event_element_clic(event,this);"><i style="" class="fas fa-file-invoice"></i></a>
                                                  </div>
                                          </div>
                                          <input id="Hidden2" type="hidden" value="" runat="server" />
                                      </div>
                                  </div>
                                  <asp:UpdatePanel ID="Updatepanel_boton_content" runat="server" UpdateMode="Conditional">
                                      <ContentTemplate>
                                          <asp:ImageButton ID="ImageButtonInicio" runat="server" ToolTip="Primera  imagen" ImageUrl="../imagewf/inicio14.png" Style="display: none" />
                                          <asp:ImageButton ID="ImageButtonAnterior" runat="server" ToolTip="Anterior imagen" ImageUrl="../imagewf/anterior15.png" Style="display: none" />
                                          <asp:ImageButton ID="ImageButtonguardar" runat="server" ToolTip="Guardar firma" Style="display: none" />
                                          <asp:ImageButton ID="ImageButtonSiguiente" runat="server" ToolTip="Siguiente imagen" ImageUrl="../imagewf/siguiente15.png" ImageAlign="NotSet" Style="display: none" />
                                          <asp:ImageButton ID="ImageButtonFinal" runat="server" ToolTip="Ultima  imagen" ImageUrl="../imagewf/final15.png" Style="display: none" />
                                          <asp:ImageButton ID="ImageButton_ir_pagina" runat="server" ToolTip="Ir a imagen" ImageUrl="../Docuarchi/imagenes/busca_pagina.png" Visible="true" Style="display: none" />
                                          <asp:ImageButton ID="ImageMenos" runat="server" ToolTip="Alejar Imagen" ImageUrl="../imagewf/alejarimagen.png" Style="display: none" />
                                          <asp:ImageButton ID="ImageMas" runat="server" ToolTip="Acercar Imagen" ImageUrl="../imagewf/acercarimagen.png" Style="display: none" />
                                          <asp:ImageButton ID="ImageRotate45" runat="server" ToolTip="Rotar 90 grados" ImageUrl="../Docuarchi/imagenes/rotar90.png" Style="display: none" />
                                          <asp:ImageButton ID="ImageRotate180" runat="server" ToolTip="Rotar 180 grados" ImageUrl="../Docuarchi/imagenes/rotar180.png" Style="display: none" />
                                          <asp:ImageButton ID="ImageRotate270" runat="server" ToolTip="Rotar 270 grados" ImageUrl="../Docuarchi/imagenes/rotar270.png" Style="display: none" />
                                          <asp:ImageButton ID="ImageButtonimprimir" runat="server" ToolTip="Imprimir documento" ImageUrl="../Docuarchi/imagenes/imprimir30.png" Visible="true" Style="display: none" />
                                          <asp:ImageButton ID="ImageButtonguardardocumento" runat="server" ToolTip="Guadar documento" Style="display: none" ImageUrl="../Docuarchi/imagenes/guardarimagen.png" Visible="true" />
                                          <asp:ImageButton ID="ImageButtoninfo" runat="server" ToolTip="Información Documento" ImageUrl="../Docuarchi/imagenes/infoimagen.png" Style="display: none" Visible="true" />
                                          <asp:ImageButton ID="ImageFirma" runat="server" ToolTip="Firma Imagen" ImageUrl="../imagewf/firma.png" Style="display: none" OnClientClick="firma_mecanica();" />
                                          <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="../imagewf/adjunta_image.png" ToolTip="Adjunta imagen a documento" Style="display: none" />

                                      </ContentTemplate>
                                  </asp:UpdatePanel>
                                  <div id="content" style="width: 100%; height: 88%; position: absolute; background-color: Gray; filter: alpha(opacity=70); opacity: 50; overflow: scroll; border-bottom-width: 0.5px; border-left-width: 1px; border-right-width: 1px; border-top-width: 2px; left: 0px; display: block">
                                      <div id="zona" style="width: auto; height: auto; position: absolute;">
                                          <asp:UpdatePanel ID="UpdatePanel_noaming" runat="server" UpdateMode="Conditional">
                                              <ContentTemplate>
                                                  <neoimg:ImageDraw ID="noaming" runat="server" Style="position: relative" RenderingMethod="HttpHandler" HttpHandlerName="ImageGenerator.axd">
                                                  </neoimg:ImageDraw>
                                                  <div id="draggable" class="ui-widget-content" style="background-color: Gray; display: none; position: absolute">
                                                      <img id="img" alt="Firma Mecanica"
                                                          align="bottom" style="border-style: none" />
                                                  </div>
                                              </ContentTemplate>
                                              <Triggers>
                                              </Triggers>
                                          </asp:UpdatePanel>
                                      </div>
                                     
                                  </div>
                              </asp:Panel>
                          </ContentTemplate>
                      </asp:UpdatePanel>
                                      <input id="Hiddenintercambio" type="hidden" value="0" runat="server" />
                                      <input id="Hiddenintercambio2" type="hidden" value="0" runat="server" />     
              </div>  
                 <div id="conten_error_seleccion_task"></div>
                 <div id="content_pie_seleccion_tarea" style="width: 100%; clear: both" class="p-2  modal_content_no_back_superior nav_botota_person_gray hide_bar_wf hover">
                          <asp:UpdatePanel ID="UpdatePanel_estado_tarea" runat="server" UpdateMode="Conditional"
                              RenderMode="Inline">
                              <ContentTemplate>
                                  <% If WorkflowCentroTrabajoModernActive Then %>
                                  <div id="ctw-task-context" class="ctw-task-context" aria-label="Contexto de la tarea seleccionada">
                                      <div class="ctw-task-context__headline">
                                          <asp:Label ID="Label_contexto_tramite" runat="server" Text="Trámite" CssClass="ctw-task-context__title"></asp:Label>
                                          <asp:Label ID="Label_contexto_estado" runat="server" Text="Estado" CssClass="ctw-task-context__state"></asp:Label>
                                      </div>
                                  <% Else %>
                                  <div class="row">
                                      <div class="col-8">
                                  <% End If %>
                                  <asp:Label ID="Label_estado_tarea_selecion" runat="server" Text="Estado" Style="color: #6d7fcc" CssClass="h6 font-weight-light ctw-task-context__meta"></asp:Label>
                                  <% If Not WorkflowCentroTrabajoModernActive Then %>
                                      </div>
                                      <div class="col-4">
                                  <% End If %>
                                  <asp:Label ID="Label_estado_selecion" runat="server" Text="Estado ruta" Style="color: #6d7fcc; float: right" CssClass="font-weight-light h6 ctw-task-context__process"></asp:Label>
                                  <% If WorkflowCentroTrabajoModernActive Then %>
                                  </div>
                                  <% Else %>
                                      </div>
                                  </div>
                                  <% End If %>
                              </ContentTemplate>
                          </asp:UpdatePanel>
                      </div>
              </div>
         </div>
        <!--consulta_avanzada_ruta_workflow-->    
        <asp:Panel ID="Panel_consulta_avanzada_ruta_workflow" runat="server" Style="display: none; width: 80%; height: auto" CssClass="modal_content_general">
            <asp:ModalPopupExtender ID="ModalPopupExtender_edition_consulta_avanzada_ruta_workflow" runat="server" TargetControlID="ButtonSalir_consulta_avanzada_ruta_workflow" BackgroundCssClass="FondoAplicacion"
                CancelControlID="Button_cerrar_consulta_avanzada_ruta_workflow" PopupControlID="Panel_consulta_avanzada_ruta_workflow">
            </asp:ModalPopupExtender>
            <div class="modal-content_">
                <div id="div_consulta_avanzada_ruta_workflow" class="modal_title_superior_ modal-header">
                    <h6 class="modal-title">Consulta avanzada</h6>
                    <button type="button" value="Button_cerrar_consulta_avanzada_ruta_workflow" class="close da_event_captive">&times;</button>
                </div>
                <div id="contenido_procesa_consulta_avanzada_ruta_workflow" style="width: auto; height: auto; border-top: none" class="modal_content_back modal-body">
                    <div id="div_consulta_avanzada">       
                         
                    </div>
                </div>

            </div>
            <div class="modal-footer align-content-end">
                <button type="button"  title="Consulta" onclick="preven_event_search_especial(event, this);" class="btn btn-success   mt-1"> Aceptar</button>
                <button type="button" title="" value="Button_cerrar_consulta_avanzada_ruta_workflow" class="btn btn-light da_event_captive  mt-1"> Cancelar </button>
                
            </div>
            <div style="display: none; height: 1px">
                <asp:Button ID="Button_cerrar_consulta_avanzada_ruta_workflow" runat="Server" Text="X" CssClass="invisible" />
                <asp:Button ID="Button_consulta_avanzada_ruta_workflow" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                <asp:Button ID="ButtonSalir_consulta_avanzada_ruta_workflow" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
            </div>
        </asp:Panel>
           <!--actualiza indice bacth-->    
        <asp:Panel ID="Panel_actualiza_indice_batch_wf" runat="server" Style="display: none; width: 80%; height: auto" CssClass="modal_content_general">
            <asp:ModalPopupExtender ID="ModalPopupExtender_edition_actualiza_indice_batch_wf" runat="server" TargetControlID="ButtonSalir_actualiza_indice_batch_wf" BackgroundCssClass="FondoAplicacion"
                CancelControlID="Button_cerrar_actualiza_indice_batch_wf" PopupControlID="Panel_actualiza_indice_batch_wf">
            </asp:ModalPopupExtender>
            <div class="modal-content_" id="modal_content_actualiza_indice_batch_wf">
                <div id="title_actualiza_indice_batch_wf" class="modal_title_superior_ modal-header">
                    <h6 class="modal-title">Actualiza indice batch</h6>
                    <button type="button" value="Button_cerrar_actualiza_indice_batch_wf" class="close da_event_captive">&times;</button>
                </div>
                <div id="contenido_procesa_actualiza_indice_batch_wf" style="width: auto; height: auto; border-top: none; overflow:auto" class="modal_content_back modal-body">
                    <div id="div_actualiza_indice_batch_wf" style="height:100%">       
                         
                    </div>
                </div>
            </div>
            <div class="modal-footer align-content-end" id="modal_foter_actualiza_indice_batch_wf">
                <button type="button" id="boton_event_actualiza_indice_batch_wf" title=""  class="btn btn-success   mt-1"> Aceptar</button>
                <button type="button" title="" value="Button_cerrar_actualiza_indice_batch_wf" class="btn btn-light da_event_captive  mt-1"> Cancelar </button>  
            </div>
            <div style="display: none; height: 1px">
                <asp:Button ID="Button_cerrar_actualiza_indice_batch_wf" runat="Server" Text="X" CssClass="invisible" />
                <asp:Button ID="Button_actualiza_indice_batch_wf" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                <asp:Button ID="ButtonSalir_actualiza_indice_batch_wf" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
            </div>
        </asp:Panel>
        <!--Actualiza indice bacth enlace-->    
        <asp:Panel ID="Panel_actualiza_indice_batch_wf_enlace" runat="server" Style="display: none; width: 80%; height: auto" CssClass="modal_content_general">
            <asp:ModalPopupExtender ID="ModalPopupExtender_edition_actualiza_indice_batch_wf_enlace" runat="server" TargetControlID="ButtonSalir_actualiza_indice_batch_wf_enlace" BackgroundCssClass="FondoAplicacion"
                CancelControlID="Button_cerrar_actualiza_indice_batch_wf_enlace" PopupControlID="Panel_actualiza_indice_batch_wf_enlace">
            </asp:ModalPopupExtender>
            <div class="modal-content_" id="modal_content_actualiza_indice_batch_wf_enlace">
                <div id="title_actualiza_indice_batch_wf_enlace" class="modal_title_superior_ modal-header">
                    <h6 class="modal-title">Actualiza indice batch</h6>
                    <button type="button" value="Button_cerrar_actualiza_indice_batch_wf_enlace" class="close da_event_captive">&times;</button>
                </div>
                <div id="contenido_procesa_actualiza_indice_batch_wf_enlace" style="width: auto; height: auto; border-top: none; overflow:auto" class="modal_content_back modal-body">
                    <div id="div_actualiza_indice_batch_wf_enlace" style="height:100%">       
                         
                    </div>
                </div>
            </div>
            <div class="modal-footer align-content-end" id="modal_foter_actualiza_indice_batch_wf_enlace">
                <button type="button" id="boton_event_actualiza_indice_batch_wf_enlace" title=""  class="btn btn-success   mt-1"> Aceptar</button>
                <button type="button" title="" value="Button_cerrar_actualiza_indice_batch_wf_enlace" class="btn btn-light da_event_captive  mt-1"> Cancelar </button>  
            </div>
            <div style="display: none; height: 1px">
                <asp:Button ID="Button_cerrar_actualiza_indice_batch_wf_enlace" runat="Server" Text="X" CssClass="invisible" />
                <asp:Button ID="Button_actualiza_indice_batch_wf_enlace" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                <asp:Button ID="ButtonSalir_actualiza_indice_batch_wf_enlace" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
            </div>
        </asp:Panel>
        <asp:Panel ID="Panel_estado_paginacion" runat="server" Style="display: none; width: auto; height: 50%" CssClass="modal_content_general_">
              <asp:ModalPopupExtender ID="ModalPopupExtender_edition_estado_paginacion" runat="server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_estado_paginacion"
                  CancelControlID="Button_cerrar_estado_paginacion" PopupControlID="Panel_estado_paginacion">
              </asp:ModalPopupExtender>
              <div class="modal-content" id="modal_estado_paginacion">
                  <div id="divcabecer2_estado_paginacion_" class="modal_title_superior_ modal-header">
                      <h6 class="modal-title">Estado paginación</h6>
                      <button type="button" onclick="activa_boton_client_server('Button_cerrar_estado_paginacion');" class="close">&times;</button>
                  </div>
                  <div id="contenido_procesa_estado_paginacion" style="background-color: white; width: auto; height: auto; color: black; background-color: #FFFFFF; border-top: none; overflout: auto" class="modal_content_back modal-body">
                      <asp:UpdatePanel ID="UpdatePanel_estado_paginacion_chek" runat="server" UpdateMode="Conditional">
                          <ContentTemplate>
                              <asp:CheckBox ID="CheckBox_estado_paginacion" runat="server" />
                              <span>Estado paginación lista de tareas</span>
                          </ContentTemplate>
                      </asp:UpdatePanel>
                  </div>
                  <div class="modal-footer" id="contenido_buton_estado_paginacion">
                      <asp:UpdatePanel ID="UpdatePanel_estado_paginacion" runat="server" UpdateMode="Conditional">
                          <ContentTemplate>
                              <asp:Button ID="Button_cambiar_estado_paginacion" runat="server" Text="Aceptar" Style="float: right; margin-left: 10px" CssClass="btn btn-success" />
                          </ContentTemplate>
                      </asp:UpdatePanel>
                  </div>
              </div>
              <div style="display: none; height: 1px">
                  <asp:Button ID="Button_cerrar_estado_paginacion" runat="Server" Text="X"
                      ToolTip="Cerrar ventana" />
                  <asp:Button ID="Button_estado_paginacion" runat="server" Text="Button" Height="1px" Width="1px" />
                  <asp:Button ID="ButtonSalir_estado_paginacion" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
              </div>
          </asp:Panel>
              <asp:Panel ID="Panel_copiar_estructura" runat="server" Style="display:none; overflow:hidden"  Width="95%" Height="100% " CssClass="modal_content_general_">
                  <asp:ModalPopupExtender ID="ModalPopupExtender_copiar_estructura" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_copiar_estructura"
                      PopupControlID="Panel_copiar_estructura"  CancelControlID="ButtonSalir_copiar_estructura">
                  </asp:ModalPopupExtender>
               <div id="modal_content_copiar_estructura" class="modal-content">
                   <div id="Cabecerapendiente_copiar_estructura" class="modal_title_superior_ modal-header">
                       <h6 class="modal-title d-inline ml-1" id="title_copiar_estructura"></h6>
                       <button type="button" value="ButtonSalir_copiar_estructura" class="close da_event_captive">&times;</button>
                   </div>
                   <div id="Cotenedorpendiente_copiar_estructura" style="color: Black; background-color: #FFFFFF; height: 90%; width: 100%; overflow: hidden" class="modal_content_back">
                       <asp:UpdatePanel ID="UpdatePanel_copiar_estructura" runat="server" UpdateMode="Conditional">
                           <ContentTemplate>
                               <iframe id="Iframe_copiar_estructura_" runat="server" frameborder="0" style="width: 100%; height: 100%; overflow: hidden"></iframe>
                           </ContentTemplate>

                       </asp:UpdatePanel>

                   </div>
               </div>
                   <div style="display:none; height:1px">
                   <asp:Button ID="Button_copiar_estructura" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="ButtonSalir_copiar_estructura" runat="Server" Text="" CssClass="invisible"/>
                  </div>
              </asp:Panel>
              <asp:Panel ID="Panel_visor_tareas_pendiente" runat="server" Style="display:none; overflow:hidden"  Width="95%" Height="100% " CssClass="modal_content_general_">
                  <asp:ModalPopupExtender ID="ModalPopupExtender_visor_tareas_pendiente" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_visor_tareas_pendiente"
                      PopupControlID="Panel_visor_tareas_pendiente"  CancelControlID="ButtonSalir_visor_tareas_pendiente">
                  </asp:ModalPopupExtender>
               <div id="modal_content_visor_tareas_pendiente" class="modal-content">
                   <div id="Cabecerapendiente_visor_tareas_pendiente" class="modal_title_superior_ modal-header">
                       <h6 class="modal-title d-inline ml-1"></h6>
                       <button type="button" value="ButtonSalir_visor_tareas_pendiente" class="close da_event_captive">&times;</button>
                   </div>
                   <div id="Cotenedorpendiente_visor_tareas_pendiente" style="color: Black; background-color: #FFFFFF; height: 90%; width: 100%; overflow: hidden" class="modal_content_back">
                       <asp:UpdatePanel ID="UpdatePanel_visor_tareas_pendiente" runat="server" UpdateMode="Conditional">
                           <ContentTemplate>
                               <iframe id="Iframe_visor_tareas_pendiente_" runat="server" frameborder="0" style="width: 100%; height: 100%; overflow: hidden"></iframe>
                           </ContentTemplate>

                       </asp:UpdatePanel>

                   </div>
               </div>
                   <div style="display:none; height:1px">
                   <asp:Button ID="Button_visor_tareas_pendiente" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="ButtonSalir_visor_tareas_pendiente" runat="Server" Text="" CssClass="invisible"/>
                  </div>
              </asp:Panel>
              <asp:Panel ID="Panel_tareas_pendientes" runat="server" Style="display:none; width: 99%; height: 100%" CssClass="modal_content_general_">
                  <asp:ModalPopupExtender ID="ModalPopupExtender_edition_tareas_pendientes" runat="server"
                      TargetControlID="ButtonSalir_tareas_pendientes" BackgroundCssClass="FondoAplicacion"
                      CancelControlID="Button_cerrar_tareas_pendientes" PopupControlID="Panel_tareas_pendientes">
                  </asp:ModalPopupExtender>
                  <div id="modal_content_tareas_pendientes" class="modal-content">
                      <div id="diver_cabcera_tareas_pendientes" class="modal_title_superior_ modal-header">
                          <h6 class="modal-title d-inline ">Tareas pendientes</h6>
                          <button type="button" value="Button_cerrar_tareas_pendientes" class="close da_event_captive ">&times;</button>
                      </div>
                      <div id="contenido_procesa_tareas_pendientes" style="background-color: white; width: 100%; height: 100%; border-top: none" class="modal_content_back modal-body">
                          <div id="contenido_titulo_tareas_pendientes" class="row p-2">
                              <div class="col-7">
                                  <asp:UpdatePanel ID="UpdatePanel_title_tarea_pendiente" runat="server" UpdateMode="Conditional">
                                      <ContentTemplate>
                                          <asp:Label ID="titulo_label_tareas_pendientes" runat="server" class="h6 font-weight-light">Resultados busqueda</asp:Label>
                                      </ContentTemplate>
                                  </asp:UpdatePanel>
                              </div>
                              <div class=" float-md-right col-md-5 float-sm-left">
                                  <div class="input-group ">
                                      <button id="Button_restore_tareas_pendientes" class="btn btn-outline-secondary border-right-2 " style="border-top-right-radius: 0px; border-bottom-right-radius: 0px" title="Restaurar lista" onclick="preven_event_restor_search_pendientes(event,this)" type="button">
                                          <i class="fal fa-long-arrow-left"></i>
                                      </button>
                                      <asp:TextBox ID="busqueda_lista_pendiente" runat="server" class="form-control form-control-sm complex " placeholder="Busqueda...."></asp:TextBox>
                                      <div class="input-group-append">
                                          <button class="btn btn-outline-secondary" onclick="preven_event_search_pendientes(event,this)" title="consultar lista" type="button">
                                              <i class="fal fa-search"></i>
                                          </button>
                                      </div>
                                  </div>
                              </div>      
                          </div>
                          <div id="content_data_grid_tareas_pendientes" class="conten_gred_border_" style="overflow: auto; width: 100%">
                              <asp:UpdatePanel ID="UpdatePanel_tareas_pendientes" runat="server" UpdateMode="Conditional">
                                  <ContentTemplate>
                                      <asp:GridView ID="data_grid_lista_pendientes" runat="server" Style="position: inherit; width: 100% ; font-size: 14px"
                                          AutoGenerateSelectButton="False" CssClass="filtrar table  font-weight-light" GridLines="None"
                                           EnableViewState="true">
                                          <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                          <HeaderStyle CssClass="GridviewScrollHeader_line_boot" />
                                          <Columns>
                                              <asp:BoundField HeaderText="OPCIONES" />
                                          </Columns>
                                      </asp:GridView>
                                       <input id="Hidden_id_list_pent" type="hidden" value="-1" runat="server"/>
                                       <input id="Hidden_id_list_id_task" type="hidden" value="-1" runat="server"/>
                                       <input id="Hidden_count_reg" type="hidden" value="-1" runat="server"/>
                                  </ContentTemplate>
                                  <Triggers>
                                  </Triggers>
                              </asp:UpdatePanel>
                          </div>
                          <div style="display: none; height: 1px">
                              <asp:Button ID="Button_tareas_pendientes" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                              <asp:Button ID="ButtonSalir_tareas_pendientes" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                              <asp:Button ID="Button_cerrar_tareas_pendientes" runat="Server" Text="X" CssClass="invisible" />
                          </div>
                      </div>
                      <div id="content_boton_tareas_pendientes" class="modal-footer justify-content-end">
                          <asp:UpdatePanel ID="UpdatePanel_boton_tareas_pendientes" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                              <ContentTemplate>
                                  <asp:Button ID="Button_enviar_tarea_pendiente" runat="server" Text="Enviar tarea a pendiente" style="display:none" ToolTip="Enviar la tarea seleccionada a estado pendiente" CssClass="btn btn-success" />
                              </ContentTemplate>
                          </asp:UpdatePanel>

                      </div>
                  </div>
              </asp:Panel>
              <div style="display: none; height: 0px">
                  <asp:UpdatePanel ID="UpdatePanel_tool_tareas_pedientes" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                      <ContentTemplate>
                          <input id="Hidden_confir_elimina" type="hidden" value="" runat="server" />
                          <asp:Button ID="Button_tool_restore_lista_tareas_pendiente" runat="server" Text="" ToolTip="" Style="display: none" />
                          <asp:Button ID="Button_tool_consulta_lista_tareas_pendiente" runat="server" Text="" ToolTip="" Style="display: none" />
                          <asp:Button ID="Button_tool_visor_emergente_tareas_pendiente" runat="server" Text="" ToolTip="" Style="display: none" />
                          <asp:Button ID="Button_clasficar_documento" runat="server" Text="" Style="display: none" />
                          <asp:Button ID="Button_eliminar_documento" runat="server" Text="" Style="display: none"  />
                          <asp:Button ID="Button_eliminar_documento_adjunto" runat="server" Text="" Style="display: none"  />
                      </ContentTemplate>
                  </asp:UpdatePanel>
              </div>
           <!--update panel de intercambio de datos con java-->
             <asp:UpdatePanel ID="UpdatePanelintercambio" runat="server" UpdateMode="Conditional">
                          <ContentTemplate>
                              <input id="mesjbox" type="hidden" value="0" runat="server"/>
                              <input id="hdnEmailID" type="hidden" value="0" runat="server"/>
                              <input id="Hidden1" type="hidden" value="0" runat="server"/>
                          </ContentTemplate>
                      </asp:UpdatePanel>    
            
  
          <asp:UpdatePanel ID="UpdatePanel_seleccion_treview" runat="server" UpdateMode="Conditional" RenderMode="Inline">
              <ContentTemplate>
                  <asp:Button ID="Button_selecion_treview_documento" runat="server" Text="Button" Style="display: none" />
                  <input id="hidden_selecion_documento_treview" type="hidden" value="" runat="server"/>
                  <input id="hidden_selecion_actualiza_treview" type="hidden" value="" runat="server"/>
                  <input id="hidden_estado_seleccion" type="hidden" value="" runat="server"/>
                  <asp:Button ID="Button_Actualizar_seleccion_indice_wf" runat="server" Text="Actualizar" Style="display: none" OnClientClick="actualiza_treview_seleccion();" />
                  <asp:Button ID="Button_buton_actualiza_seleccion" runat="server" Text="Button" Style="display: none" OnClientClick="actualiza_treview_seleccion();" />
                  <input id="Button_buton_actualiza_seleccion__" type="button" value="button" style="display: none" onclick="actualiza_treview_seleccion();" />
              </ContentTemplate>
          </asp:UpdatePanel>
          <div style="display:none">
              <asp:UpdatePanel ID="UpdatePanel_tool_menucab" runat="server" UpdateMode="Conditional">
                  <ContentTemplate>
                      <input id="Hidden_menucab" type="hidden" value="" runat="server"/>
                      <asp:Button ID="Button_tool_menucab" runat="Server" Text=""   CssClass="modal_boton_hiden"
                                />
                  </ContentTemplate>
              </asp:UpdatePanel>
          </div>
           <!--nota_flujo-->  
           <asp:Panel ID="Panel_content_anotacion" runat="server" Style="display: none; width: 90%; height: 100%" CssClass="modal_content_general_">
                  <asp:ModalPopupExtender ID="ModalPopupExtender_edition_content_anotacion" runat="server"
                      TargetControlID="ButtonSalir_content_anotacion_" BackgroundCssClass="FondoAplicacion"
                      CancelControlID="Button_cerrar_content_anotacion" PopupControlID="Panel_content_anotacion">
                  </asp:ModalPopupExtender>
                  <div id="modal_content_anotacion" class="modal-content">
                      <div id="diver_cabcera_content_anotacion" class="modal_title_superior_ modal-header">
                          <asp:Label ID="LabelTitulo" runat="server" Text="Anotaciones de la Tarea" CssClass="h6 font-weight-light"> </asp:Label>
                          <button type="button" value="Button_cerrar_content_anotacion" class="close da_event_captive ">&times;</button>
                      </div>
                      <div id="contenido_procesa_content_anotacion" style="height: auto; width: 100%; overflow: auto; border-top:none" class="pl-3 pr-3">
                          <asp:UpdatePanel ID="UpdatePanelanotacion" runat="server" UpdateMode="Conditional">
                              <ContentTemplate>
                                  <asp:Panel ID="Panel_content_anotacion_gred" runat="server"
                                      Enabled="true" Style="margin-bottom: 5px">
                                      <asp:GridView ID="GridView_lista_notas" runat="server" Style="width: 100%"
                                          AutoGenerateSelectButton="False" CssClass="filtrar table  font-weight-light" GridLines="None">
                                          <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                          <HeaderStyle CssClass="GridviewScrollHeader_line_boot" />
                                          <Columns>
                                              <asp:BoundField HeaderText="OPCIONES" />
                                          </Columns>
                                      </asp:GridView>
                                  </asp:Panel>
                                  <input id="hdnidlista" type="hidden" value="-1" runat="server"/>
                              </ContentTemplate>
                              <Triggers>
                              </Triggers>
                          </asp:UpdatePanel>
                      </div>
                       <div id="content_boton" class="modal-footer justify-content-end">
                           <input id="Button_Show_Guardar" type="button" class="btn btn-success" value="Nueva nota"  />        
                  </div>
                  </div>
                 
                  <div style="display: none; height: 1px">
                      <asp:Button ID="Button_content_anotacion" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                      <asp:Button ID="ButtonSalir_content_anotacion_" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                      <asp:Button ID="Button_cerrar_content_anotacion" runat="Server" Text="X" CssClass="invisible" />
                  </div>
              </asp:Panel>
             <div id="nota_respuesta">
                  <asp:Panel ID="Panel_nota_respuesta" runat="server" Style="display: none; width: 66%; height: auto" CssClass="modal_content_general_">
                      <asp:ModalPopupExtender ID="ModalPopupExtender_edition_nota_respuesta" runat="server" TargetControlID="ButtonSalir_nota_respuesta" BackgroundCssClass="FondoAplicacion"
                          CancelControlID="Button_cerrar_nota_respuesta" PopupControlID="Panel_nota_respuesta">
                      </asp:ModalPopupExtender>
                      <div id="modal_content_nota_respuesta" class="modal-content">
                          <div id="divcabecer_nota_respuesta" class="modal_title_superior_ modal-header">
                              <asp:Label ID="Label_nota_respuesta" class="modal-title d-inline ml-1 h6" runat="server" Text="Nota">
                              </asp:Label>
                              <button type="button" value="Button_cerrar_nota_respuesta" class="close da_event_captive">&times;</button>
                          </div>
                         
                          <div id="contenido_procesa_nota_respuesta" style="border-top: none; overflow: hidden" class="p-1">    
                               <textarea id="TextBox_nota" rows="10" style="width: 100%; height: 100%" cols="50">Write something here</textarea>
                          </div>
                          <div id="content_boton_nota" class="modal-footer">
                              <input id="Button_actualizar_nota" type="button" class="btn btn-success" value="Aceptar" style="display:none" />
                              <input id="Button_duardar_nota" type="button" class="btn btn-success" value="Aceptar" style="display:none"/>
                          </div>
                          <div style="display: none; height: 1px">
                              <asp:Button ID="Button_nota_respuesta" CssClass="invisible" runat="server" Text="Button" Height="0px" Width="0px" Style="display: none" />
                              <asp:Button ID="ButtonSalir_nota_respuesta" CssClass="invisible" runat="server" Text="Button" Height="0px" Width="0px" Style="display: none" />
                              <asp:Button ID="Button_cerrar_nota_respuesta" runat="Server" Text="X" CssClass="modal_boton_hiden" />
                          </div>

                      </div>
                  </asp:Panel>
              </div>  
          <!--detalle_flujo-->     
            <asp:Panel ID="Panel_detalle_flujo" runat="server" Style="display:none;  width:auto; height:auto" >
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_detalle_flujo"  runat="server"  TargetControlID="ButtonSalir_detalle_flujo" 
                     BackgroundCssClass="FondoAplicacion" 
                    CancelControlID="Button_cerrar_detalle_flujo" PopupControlID="Panel_detalle_flujo"   ></asp:ModalPopupExtender>
                <div id="modal_content_detalle_flujo" class="modal-content">
                    <div id="divcabecer_detalle_flujo" class="modal-header">
                         <h6 class="modal-title d-inline ">Información tarea</h6>
                         <button type="button" value="Button_cerrar_detalle_flujo" class="close da_event_captive">&times;</button>   
                    </div>
                    <div id="contenido_procesa_detalle_flujo" style="overflow:auto " class="pl-1 pr-1">
                        <asp:UpdatePanel ID="UpdatePanel_detalle_flujo" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                 <asp:Panel ID="Panel_content_detalle_flujo" runat="server" CssClass="mt-3"  Style="margin-left: 5px"
                                    EnableViewState="true">
                                     <asp:Table ID="Table_detalle_flujo" runat="server" style="overflow:auto; width:100%; font-size:12px" CssClass="table font-weight-light"  ViewStateMode="Enabled">
                               </asp:Table>
                                </asp:Panel>
                              
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                     <div class="modal-footer" id="foter_detalle_flujo">
                    </div>
                </div>
                <div style="display:none; height:1px">
                    <asp:Button ID="Button_detalle_flujo" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="ButtonSalir_detalle_flujo" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="Button_cerrar_detalle_flujo" runat="Server" Text="" CssClass="invisible"/>
                 </div>
            </asp:Panel>
          <!--detalle_sesion-->     
            <asp:Panel ID="Panel_detalle_sesion" runat="server" Style="display:none;  width:auto; height:auto" >
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_detalle_sesion"  runat="server"  TargetControlID="ButtonSalir_detalle_sesion" 
                     BackgroundCssClass="FondoAplicacion" 
                    CancelControlID="Button_cerrar_detalle_sesion" PopupControlID="Panel_detalle_sesion"   ></asp:ModalPopupExtender>
                <div id="modal_content_detalle_sesion" class="modal-content">
                    <div id="divcabecer_detalle_sesion" class="modal-header">
                         <h6 class="modal-title d-inline ">Detalle sesión</h6>
                         <button type="button" value="Button_cerrar_detalle_sesion" class="close da_event_captive">&times;</button>   
                    </div>
                    <div id="contenido_procesa_detalle_sesion" style="overflow:auto " class="pl-1 pr-1">
                        <asp:UpdatePanel ID="UpdatePanel_detalle_sesion" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                 <asp:Panel ID="Panel_detalle_session" runat="server" CssClass="mt-3"  Style="margin-left: 5px"
                                    EnableViewState="true">
                                     <asp:Table ID="Table_detalle_session" runat="server" style="overflow:auto; width:100%; font-size:12px" CssClass="table font-weight-light"  ViewStateMode="Enabled">
                               </asp:Table>
                                </asp:Panel>
                              
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                     <div class="modal-footer" id="foter_detalle_sesion">
                    </div>
                </div>
                <div style="display:none; height:1px">
                    <asp:Button ID="Button_detalle_sesion" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="ButtonSalir_detalle_sesion" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="Button_cerrar_detalle_sesion" runat="Server" Text="" CssClass="invisible"/>
                 </div>
            </asp:Panel>
         <div id="porput" style="clear: both; width:95%"> </div>        
              <input id="HiddenGredview" type="hidden" value="Gredview2" runat="server"/>
              <!--Ppopup recuperar tareas-->
             <asp:Panel ID="Panelrecuperar" runat="server" Style="display: none; width: 99%; height: 99%" CssClass="modal_content_general_">
                 <asp:ModalPopupExtender ID="ModalPopupExtenderRecuperar" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Buttoneget"
                     PopupControlID="Panelrecuperar" CancelControlID="ButtonCerrarRecuperar">
                 </asp:ModalPopupExtender>
                 <div id="modal_content_recuperar" class="modal-content">
                     <div id="Cabecerarecuperar" class="modal_title_superior_ modal-header">
                         <h6 class="modal-title d-inline ml-1">Recuperar tareas</h6>
                         <button type="button" value="ButtonCerrarRecuperar" class="close da_event_captive">&times;</button>
                     </div>
                     <div id="ContendorRecuperar" style="height: 100%; width: 99%" class="modal_content_back_  m-1">
                         <asp:UpdatePanel ID="UpdatePanelRecuperar" runat="server" UpdateMode="Conditional">
                             <ContentTemplate>
                                 <iframe id="IframeRecuperar_" runat="server" frameborder="0" scrolling="no" style="width: 99%"></iframe>
                             </ContentTemplate>
                         </asp:UpdatePanel>
                     </div>
                     <div class="modal-footer" id="modal_contendor_recuperar">
                         
                     </div>
                     <div style="display: none; height: 1px">
                         <asp:Button ID="Buttoneget" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" Style="display: none" />
                         <asp:Button ID="ButtonCerrarRecuperar" runat="Server" Text="" CssClass="modal_boton_hiden"
                             Style="display: none" />
                     </div>
                 </div>
             </asp:Panel>
              <!--Popup visor externo-->
               <asp:Panel ID="Panel_visor_externo" runat="server" Style="display:none; overflow:hidden"  Width="95%" Height="100% " CssClass="modal_content_general_">
                  <asp:ModalPopupExtender ID="ModalPopupExtender_visor_externo" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_visor_externo"
                      PopupControlID="Panel_visor_externo"  CancelControlID="ButtonSalir_visor_externo">
                  </asp:ModalPopupExtender>
                   <div id="modal_content_visor_externo" class="modal-content">  
                  <div id="Cabecerapendiente_visor_externo" class="modal_title_superior_ modal-header">  
                          <h6 class="modal-title d-inline ml-1"></h6>
                          <button type="button" value="ButtonSalir_visor_externo" class="close da_event_captive">&times;</button>              
                  </div>
                  <div id="Cotenedor_visor_externo" style="height: 90%; width: 100%; border-top:none; overflow:hidden" class="modal_content_back_ modal-body">        
                      <asp:UpdatePanel ID="UpdatePanel_visor_externo" runat="server" UpdateMode="Conditional">
                          <ContentTemplate>
                              <iframe id="Iframe_visor_externo_" runat="server" frameborder="0"  style="width:100%; height:100%; overflow:hidden"></iframe>
                          </ContentTemplate>
                      </asp:UpdatePanel>                          
                  </div>
                       <div style="display: none; height: 0px">
                           <asp:Button ID="Button_visor_externo" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" Style="display: none" />
                           <asp:Button ID="ButtonSalir_visor_externo" runat="Server" Text="X" CssClass="invisible" Style="display: none"/>
                       </div>
                  </div>
              </asp:Panel>
            
              <!--Popup pendientes-->
              <asp:Panel ID="Panelpendientes" runat="server" Style="display:none; width:98%; height:100%" ForeColor="White"   CssClass="modal_content_general">
                  <asp:ModalPopupExtender ID="ModalPopupExtenderpendiente" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir"
                      PopupControlID="Panelpendientes" Y="1" CancelControlID="ButtonCerrarpendiente"></asp:ModalPopupExtender>
                  <div id="Cabecerapendiente" class="modal_title_superior">
                      
                      <asp:Label ID="Labelpendiente" runat="server" Text="Tareas en pendiente" Font-Size="10"></asp:Label>
                      <div id="Div1" style="float: right">
                          <asp:Button ID="ButtonCerrarpendiente" runat="Server" Text="X" CssClass="modal_boton_hiden"
                               />
                      </div>
                  </div>
                  <div id="Cotenedorpendiente" style="color: Black; background-color: #FFFFFF; height: 90%; width: 100%" class="modal_content_back">
                  
                      <asp:UpdatePanel ID="UpdatePanelpendiente" runat="server" UpdateMode="Conditional">
                          <ContentTemplate>
                              <iframe id="Iframependiente_" runat="server" frameborder="0"  style="width:100%; height:100%" scrooll="none" ></iframe>
                          </ContentTemplate>

                      </asp:UpdatePanel>
                      
                      <asp:Button ID="Buttontaeget" CssClass="invisible" runat="server" Text="Button" Height="0px" Width="0px" />
                      <asp:UpdatePanel ID="UpdatePanelpedieteboton" runat="server" UpdateMode="Conditional">
                          <ContentTemplate>
                              <asp:Button ID="ButtonAsignar" CssClass="invisible" runat="server" Text="Asignar Tarea" ToolTip="Asignar Tarea de pendiente"
                                  Style="margin-right: 5px" />
                               
                               <asp:Button ID="ButtonAsignar_Aprobacion" CssClass="invisible" runat="server" Text="Asignar Tarea" ToolTip="Asignar Tarea de pendiente"
                                  Style="margin-right: 5px" />
                              <asp:Button ID="Button_sube_pediente" CssClass="invisible" runat="server" Text="Asignar Tarea" ToolTip="Asignar Tarea de pendiente" 
                                  style="margin-right:5px" />
                               <input id="hidden_000_aceptacion" type="hidden" value="" runat="server"/>
                          </ContentTemplate>

                      </asp:UpdatePanel>
                       <asp:Button ID="ButtonSalir" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                  </div>
                  
              </asp:Panel>
          <asp:Panel ID="Panel_admon_documentos" runat="server" Style="display:none; width: 100%; height: auto" CssClass="modal_content_general">
              <asp:ModalPopupExtender ID="ModalPopupExtender_edition_admon_documentos" runat="server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_admon_documentos"
                  CancelControlID="Button_cerrar_admon_documentos" PopupControlID="Panel_admon_documentos">
              </asp:ModalPopupExtender>
              <div id="modal_content_admon_documentos" class="modal_content">
                  <div id="divcabecer2_admon_documentos" class="modal-header">
                      <h6 class="modal-title">Enlace de documentos</h6>
                      <button type="button" value="Button_cerrar_admon_documentos" class="close da_event_captive close">&times;</button>
                  </div>
                  <div id="error_div_selecion_tarea_rad" style="position: relative; width: 100%"></div>
                  <div id="contenido_procesa_admon_documentos" style="background-color: white; width: auto; height: auto; border-top: none" class="modal_content_back modal-body_ m-1">
                      <div id="div_error_content_rad" style="position: relative; width: 100%"></div>
                      <asp:UpdatePanel ID="UpdatePanel_tool_documentos" CssClass="" style="width: 100%" runat="server" UpdateMode="Conditional" RenderMode="Block">
                          <ContentTemplate>
                              <nav id="navar_barra" class="navbar navbar-expand-sm nav_botota_person modal_content_no_back_inferior pl-0">
                                  <button class="navbar-toggler" type="button" style="background-color: #6d7fcc" data-toggle="collapse" data-target="#navbarNavDropdown">
                                      <span class="navbar-toggler-icon_"><i style="color: white" class="fad fa-th-list"></i></span>
                                  </button>
                                  <asp:Panel ID="Panel_cargar_archivo" CssClass="navbar-nav  " runat="server">
                                      <ul class="navbar-nav">
                                          
                                          <li class="nav-item active ml-2 d-none">
                                               <a class="nav-link"  style="color: #6d7fcc" title="Adjuntar archivo desde el dispositivo" href="#" ><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-upload"></i> Ajuntar  </a>
                                          </li>
                                           <li class="nav-item active ml-2 d-none">
                                                <a class="nav-link"  style="color: #6d7fcc" title="Adjuntar archivo desde servicio web" href="#" ><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-page-break"></i> Servicio web  </a>               
                                          </li>
                                          
                                          <li class="nav-item active ml-2 ">
                                              <a class="nav-link" style="color: #6d7fcc" title="Actualizar indice documentos relacionados" href="#" onclick="activa_boton_client_server('Button_actualiza_enlace');"><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-sync"></i> Acualizar indices  </a>
                                          </li>
                                          <li class="nav-item active ml-2 ">
                                              <a class="nav-link" style="color: #6d7fcc" title="Exportar documentos relacionados a gabinete" href="#" onclick="inicializa_tipo_adjunto_documento(event,this,'M-DWG-WS');"><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-cabinet-filing"></i> Exportar a gabinete  </a>
                                          </li>
                                         
                                      </ul>
                                      <ul class="navbar-nav">
                                           <li class="nav-item active ml-2  ">
                                              <a class="nav-link" style="color: #6d7fcc" title="Asignar tarea" href="#" onclick="activa_boton_client_server('Buttonaceptar');"><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-arrow-down"></i> Asignar  </a>
                                          </li>
                                      </ul>
                                  </asp:Panel>
                              </nav>
                          </ContentTemplate>
                      </asp:UpdatePanel>
                      <div id="conte_waper" class="container-fluid mr-0 ml-0 pl-0 pr-0" style="border-top: 1px solid #e9ecef">
                          <a id="da_show-sidebar__" class="btn btn-sm   hide_da_sidebar " href="#" data-target="#sidebar__">
                              <i style="color: white" class="fas fa-bars"></i>
                          </a>
                          <div id="da_content_wraper_" class="wrapper_ ml-0 mr-0  d-flex  justify-content-between_" style="padding-left: 1px; padding-right: 1px">
                              <div id="Contentizquierdo_" class="bg-light_ " style="width: 22%; float: left">
                                  <nav id="sidebar__" class=" bg-light_ pl-0 pr-0">
                                      <div id="title_treview" class="modal-header_ modal_title_superior " style="border-top-left-radius: initial; border-top-right-radius: initial; border-bottom: 1px solid #e9ecef; border-right: 1px solid #e9ecef">
                                          <div class="row pt-2  pb-2">
                                              <div class="col-1 pl-3 pt-2 ">
                                                   <asp:CheckBox ID="chk_selec" Text="" CssClass="btn   btn-light btn-sm border-0 bg-transparent" runat="server" onclick="table_gred_on_click_check(this,'GridView_list_documento_relacion','chek_selecion_list_rad');" />
                                              </div>
                                              <div class="col-3 d-flex justify-content-start">
                                                  <asp:UpdatePanel ID="UpdatePanelseleccion_label_documentos" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                                      <ContentTemplate>
                                                          <asp:Label ID="Label_documentos" class=" mt-2 mb-2 ml-2  h8 f font-weight-light" Style="color: #6d7fcc; float: left; font-family: 'Segoe UI'" runat="server" Text="Documentos"></asp:Label>
                                                      </ContentTemplate>
                                                  </asp:UpdatePanel>
                                              </div>                 
                                              <div class="col-7 p-0 d-flex justify-content-end">   
                                                   <a class="nav-link pr-2 pl-2" style="color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600; float: right" title="Eliminar documentos" href="#" onclick="inicializa_tipo_adjunto_documento(event,this,'C-DW-DEL-IMAGE-ENLACE')"><i style="" class="fad fa-trash-alt"></i></a>
                                                   <a class="nav-link pr-2 pl-2" style="color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600; float: right" title="Actualiza indice batch" href="#" onclick="inicializa_tipo_adjunto_documento(event,this,'C-DW-ACTU-INDICE-ENLACE')"><i style="" class="fad fa-info "></i></a>
                                                   <a class="nav-link pr-2 pl-2" id="btnLoadFileEnlace" style="color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600; float: right" title="Adjuntar documento" href="#" ><i style="" class="fas fa-upload "></i></a>
				                                   <a id="a_adj_service_web" class="nav-link pr-2 pl-2" style="color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600; float: right" title="Adjuntar documento desde servicio web" href="#" ><i style="" class="fas fa-page-break "></i></a>
                                                   <a id="sidebarCollapse_" class="close__   nav-link pr-2 pl-2" style="float: right; color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600"  title="Cerrar lista"><i class="fad fa-bars"></i></a>
                                              </div>

                                          </div>
                                      </div>
                                      <div id="div_treview_archivo" style="width: 100%; border-right: 1px solid #e9ecef">
                                          <asp:UpdatePanel ID="UpdatePanelseleccion_digitalizado" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                              <ContentTemplate>
                                                  <input id="Hidden_selccion_documento_eliminar_rad" type="hidden" value="" runat="server" />
                                                   <input id="Hidden_selccion_documento_eliminar_split_rad" type="hidden" value="" runat="server" />
                                                  <input id="Hidden_selccion_documento_cambia_tipo_rad" type="hidden" value="" runat="server" />
                                                   <input id="Hidden_selccion_documento_cambia_tipo_split_rad" type="hidden" value="" runat="server" />
                                                  <input id="hiden_seleccion_documento" type="hidden" value="" runat="server"/>
                                                  <input id="hiden_seleccion_documento_id" type="hidden" value="-1" runat="server"/>
                                                  <input id="Hidden_numero_doc_rel" type="hidden" value="0" runat="server"/>
                                                  <asp:Panel ID="Paneltreview" runat="server"
                                                      Height="100%" Width="100%" Style="position: inherit; overflow: auto" CssClass="pb-2">
                                                      <asp:GridView ID="GridView_list_documento_relacion" runat="server" Style="position: inherit; width: 100%; font-size: 14px"
                                                          AutoGenerateSelectButton="False" AllowSorting="false" AllowPaging="false" PageSize="6" PagerSettings-Position="Top" CssClass="Gridviewtable  font-weight-light" GridLines="None"
                                                          EnableViewState="true">
                                                           <RowStyle CssClass="GridviewRow" />
                                                          <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                                          <HeaderStyle CssClass="GridviewScrollHeader_line_boot_none" />
                                                          <PagerStyle CssClass="pagination-ys" />
                                                          <Columns>
                                                              <asp:TemplateField>
                                                                  <HeaderTemplate>
                                                                      <asp:Panel ID="Panel_che_box_aling_" runat="server" Style="text-align: left">
                                                                         
                                                                          
                                                                      </asp:Panel>
                                                                  </HeaderTemplate>
                                                              </asp:TemplateField>
                                                          </Columns>
                                                      </asp:GridView>
                                                      <asp:TreeView ID="TreeViewArchivo" runat="server" CssClass="TreeN  h6" NodeWrap="true" Style="overflow: auto; height: 100%; display: none" EnableViewState="false" EnableClientScript="True"
                                                          PopulateNodesFromClient="False"
                                                          LeafNodeStyle-CssClass="LeafNodeStyle" Font-Size="14px" NodeIndent="10" ExpandDepth="0" CollapseImageUrl="../imagera/minus-square-light_1.png" ExpandImageUrl="../imagera/plus-square-light_1.png" SkipLinkText="">
                                                          <HoverNodeStyle Font-Underline="False" />
                                                          <NodeStyle CssClass="nav-link_ mt-1 mb-1 pl-1  " ForeColor="black"
                                                              VerticalPadding="0px" />
                                                          <SelectedNodeStyle ForeColor="Black" CssClass="node_select_" VerticalPadding="5px" HorizontalPadding="5px" ImageUrl="../Gestion/imagenes/folder-open-regular.png" />
                                                      </asp:TreeView>

                                                  </asp:Panel>
                                              </ContentTemplate>
                                          </asp:UpdatePanel>
                                      </div>
                                      
                                     
                                      <div id="contenido_pie" style="border-top-left-radius: initial; border-top-right-radius: initial; display: none" class="modal-header pt-1 pb-1   justify-content-start">
                                          <h6 class="modal-title_ mt-2 mb-2 ml-2   font-weight-light" id="pit" style="color: white"></h6>
                                      </div>
                                  </nav>
                              </div>
                              <div id="Contenedorderecho_" class="page-content mr-0 ml-0 pl-1 pr-1 pb-0 pt-0  " style="width: 78%">
                                  <div id="Are_Digitalizacion" style="width: 100%; height: 100%; float: right; display: block; margin-left: 1px" class="modal_content_back_">
                                      <asp:UpdatePanel ID="UpdatePanel_iframe_digitaliza" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                          <ContentTemplate>
                                              <iframe id="IframeDitaliza_"  runat="server" frameborder="0" src="" width="100%" scrolling="no" height="100%"></iframe>
                                          </ContentTemplate>
                                      </asp:UpdatePanel> 
                                  </div>
                                  <div id="Area_Visor" style="width: 100%; height: 100%; display: none" class="modal_content_back_">
                                      <div id="div_cerrar" class="modal-header_ modal_title_superior " style="border-top-left-radius: initial; border-top-right-radius: initial; border-bottom: 1px solid #e9ecef">
                                          <h6 id="titel_visor" class="mt-2 mb-2 ml-2  h6 font-weight-light" style="color: #6d7fcc; font-family: 'Segoe UI'; float: left">Visor externo</h6>
                                          <button type="button" title="Cerrar ventana visualizador" onclick="prevent_cerrar(event,this);" class="close mr-1" style="float: right">&times;</button>
                                      </div>
                                      <asp:UpdatePanel ID="UpdatePanelIframevisor" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                          <ContentTemplate>
                                              <iframe id="IframeVisor_" runat="server" frameborder="0" width="100%" scrolling="no" height="100%"></iframe>
                                          </ContentTemplate>
                                      </asp:UpdatePanel>
                                  </div>
                              </div>
                          </div>
                      </div>
                  </div>
                  <div class="modal-footer  justify-content-between p-3" id="tab_content_boton">
                      <asp:UpdatePanel ID="UpdatePanel_estado_enlace" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                          <ContentTemplate>
                              <asp:Label ID="Label_estado_enlace" runat="server" Text="Digitalización de documentos" class="mt-1 ml-1  h6 font-weight-light" Style="color: #6d7fcc"></asp:Label>
                          </ContentTemplate>
                      </asp:UpdatePanel>
                  </div>
                  <div style="display: none; height: 1px">
                      <asp:Button ID="Button_cerrar_admon_documentos" runat="Server" Text="X"
                          ToolTip="Cerrar ventana" />
                      <asp:Button ID="Button_admon_documentos" runat="server" Text="Button" Height="1px" Width="1px" />
                      <asp:Button ID="ButtonSalir_admon_documentos" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                  </div>
              </div>
          </asp:Panel>
             <div style="display: none; height: 1px">
                 <asp:UpdatePanel ID="UpdatePanelBotones" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                     <ContentTemplate>
                         <div id="Datos_Digitalizacion_botones" style="width: 98%; height: 30px; float: left; position: inherit; margin-left: 0px; margin-top: 1px; overflow-y: auto; overflow-x: auto; border-top: solid 1px #ccc">
                             <asp:ImageButton ID="ImagebutonActualizarA" runat="server" ToolTip="Actualizar documentos " ImageUrl="../workflow/imageneswf/NUEVA CONSULTA3.png" Style="display: none" />
                             <a style="margin-left: 2px; width: auto; color: black" title="Actualizar lista de documentos" href="#" onclick="activa_boton_client_server('ImagebutonActualizarA')"><i style="width: 25px; height: 25px" class="far fa-redo-alt fa-2x"></i></a>
                             <asp:ImageButton ID="ButtonElimina" runat="server" Text="(X)" ToolTip="Eliminar Documento" AlternateText="(X)" ImageUrl="../workflow/imageneswf/eliminar2.png" OnClientClick='ConfirmMensaje("Desea eliminar el documento seleccionado");' Style="display: none" />
                             <a style="margin-left: 2px; width: auto; color: black" title="Eliminar Documento" href="#" onclick="activa_boton_client_server('ButtonElimina')"><i style="width: 25px; height: 25px" class="fal fa-trash-alt fa-2x"></i></a>
                             <asp:ImageButton ID="ButtonPrincipa" runat="server" Text="(P)" ToolTip="Convertir en documento Principal" ImageUrl="../workflow/imageneswf/actualizarbach2.png" Width="24px" OnClientClick='ConfirmMensaje("Desea convertir el documento en  principal");' Style="display: none" />
                             <a style="margin-left: 2px; width: auto; color: black" title="Convertir en documento Principal" href="#" onclick="activa_boton_client_server('ButtonPrincipa')"><i style="width: 25px; height: 25px" class="far fa-images fa-2x"></i></a>
                             <asp:ImageButton ID="ImageButtonindice" runat="server" ToolTip="Visualiza indice documento" ImageUrl="../Docuarchi/imagenes/indice222.png" Visible="true" Height="24px" Style="display: none" />
                             <a style="margin-left: 2px; margin: 2px; width: auto; color: black" title="Indice documento" href="#" onclick="activa_boton_client_server('ImageButtonindice')"><i style="width: 25px; height: 25px" class="fal fa-info-square fa-2x"></i></a>
                             <asp:ImageButton ID="ImageButtonActivaClasifica" runat="server" ToolTip="Cambia tipo documento" ImageUrl="../workflow/imageneswf/Actualiza_indice.png" Visible="true" Height="24px" Style="display: none" />
                             <a style="margin-left: 2px; width: auto; color: black" title="Tipifica documento" href="#" onclick="activa_boton_client_server('ImageButtonActivaClasifica')"><i style="width: 25px; height: 25px" class="fal fa-sort-alpha-up fa-2x"></i></a>
                             <input id="Hidden_estado_display" type="hidden" value="0" runat="server"/>
                             <input id="Hidden_estado_visor" type="hidden" value="" runat="server"/>
                             <asp:ImageButton ID="ImageButton_adjunt" runat="server" Text="(V)" ToolTip="Adjuntar Documento" AlternateText="(V)" ImageUrl="../workflow/imageneswf/adjuntarcarpeta.png" Visible="True" OnClientClick="eliminar_ajaxtolkit();" Style="display: none" />
                             <a style="margin-left: 2px; width: auto; color: black" title="Cargar archivo Documento" href="#" onclick="activa_boton_client_server('ImageButton_adjunt')"><i style="width: 25px; height: 25px" class="far fa-cloud-upload fa-2x"></i></a>
                             <asp:ImageButton ID="ImageButtonVisibleEscaner" runat="server" Text="(V)" ToolTip="Interface de Digitalización" AlternateText="(V)" ImageUrl="../workflow/imageneswf/ESCANEAR INTERFACE ESCANER.png" OnClientClick="dispalyInterfaceEscaner(); " Visible="True" Style="display: none" />
                             <a style="margin-left: 2px; width: auto; color: black" title="Interface de Digitalización" href="#" onclick="dispalyInterfaceEscaner();"><i style="width: 25px; height: 25px" class="far fa-vote-yea fa-2x"></i></a>
                             <asp:ImageButton ID="ButtonVisua" runat="server" Text="(V)" ToolTip="Ver archivo" AlternateText="(V)" ImageUrl="../workflow/imageneswf/paginasola.png"
                                 Visible="True" Style="width: 18px; display: none" />
                             <a style="margin-left: 2px; width: auto; color: black" title="Visualizar documento seleccionado" href="#" onclick="activa_boton_client_server('ButtonVisua')"><i style="width: 25px; height: 25px" class="fal fa-image fa-2x"></i></a>
                         </div>
                     </ContentTemplate>
                 </asp:UpdatePanel>
                 <asp:UpdatePanel ID="UpdateDatos" runat="server" UpdateMode="Conditional"  RenderMode="Inline">
                              <ContentTemplate>
                                   <asp:TextBox ID="TextBoxDatos" runat="server" TextMode="MultiLine" Width="95%" Height="50px" ReadOnly="True" style="display:none"></asp:TextBox>
                                      <asp:Button ID="Button_actualiza_enlace" runat="server" style="display:block; margin:1px" Text="&#8634; Actualizar índices" Width="99%" ToolTip="Actualiza el índice de los nuevos documentos relacionados a la tarea"  CssClass="boton_azul"/>
                                      <asp:Button ID="Buttonaceptar" runat="server" Text="&#10004; Asignar tarea" Width="99%" Style="margin:1px" CssClass="boton_azul"/>
                                      <input id="Hidden_resultado_selecion_enlace" type="hidden" value="NO" runat="server"/>  
                                      <input id="Hidden_00022_row" type="hidden" value="" runat="server"/>   
                              </ContentTemplate>
                 </asp:UpdatePanel>
            </div>
          <div style="display: none">
              <asp:UpdatePanel ID="UpdatePanel_boton_tool" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                  <ContentTemplate>
                      <input id="Hidden_result_boton_tool" type="hidden" value="" runat="server" />
                      <input id="HiddenIdFlujo" type="hidden" value="0" runat="server" />
                      <input id="hide_ruta" type="hidden" value="" runat="server" />
                      <input id="HiddenRuta" type="hidden" value="0" runat="server" />
                      <input id="Hidden_id_actividad_envio" type="hidden" value="0" runat="server" />
                      <input id="Hidden_id_actividad_disp_envio" type="hidden" value="0" runat="server" />
                      <input id="Hidden_id_usuario_envio" type="hidden" value="0" runat="server" />
                      <input id="Hidden_id_tarea" type="hidden" value="" runat="server" />
                      <input id="Hidden_date_row_" type="hidden" value="" runat="server" />
                      <asp:HiddenField ID="Hidden_url_" runat="server" />
                      <asp:HiddenField ID="Hidden_extension_" runat="server" />
                      <input id="Hidden_result_load_" type="hidden" value="" runat="server" />
                      <asp:Button ID="Button_tool_visualiza_documento" runat="server" Text="" />
                      <asp:Button ID="Button_tool_elimina_documento" runat="server" Text="" />
                      <asp:Button ID="Button_tool_elimina_documento_enlace" runat="server" Text="" />
                      <asp:Button ID="Button_tool_activa_cambia_tipologia" runat="server" Text="" />
                      <asp:Button ID="Button_tool_activa_guardar_archivo_inscripcion_sii" runat="server" Text="Button" />
                      <asp:Button ID="Button_tool_activa_sube_documento" runat="server" Text="" />
                      <asp:Button ID="Button_tool_activa_sube_documento_enlace" runat="server" Text="" />
                      <asp:Button ID="Button_tool_activa_sube_documento_enlace_integra_sii" runat="server" Text="Button" />
                      <asp:Button ID="Button_tool_activa_sube_documento_web_service" runat="server" Text="" />
                      <asp:Button ID="Button_tool_activa_sube_imagen_inscripcion_web_service" runat="server" Text="" />
                      <asp:Button ID="Button_tool_actualiza_lista_relacionados" runat="server" Text="" />
                      <asp:Button ID="Button_tool_activa_lista_documentos" runat="server" Text="" />
                      <asp:Button ID="Button_tool_activa_detalle_radicado" runat="server" Text="" />
                      <asp:Button ID="Button_tool_activa_detalle_radicado_seleccion" runat="server" Text="" />
                      <asp:Button ID="Button_tool_adjunta_documento_relacionado" runat="server" Text="" />
                      <asp:Button ID="Button_tool_activa_sube_documento_automatico" runat="server" Text="" />
                      <asp:Button ID="Button_tool_search_lista_tareas" runat="server" Text="" />
                      <asp:Button ID="Button_tool_searh_new_task" runat="server" Text="" />
                      <asp:Button ID="Button_tool_search_especial" runat="server" Text="" />
                      <asp:Button ID="Button_tool_restore_lista_tareas" runat="server" Text="" />
                      <asp:Button ID="Button_tool_enviar_actividad" runat="server" Text="" />
                      <asp:Button ID="Button_tool_busqueda_enviar_actividad" runat="server" Text="" />
                      <asp:Button ID="Button_tool_restore_busqueda_enviar_actividad" runat="server" Text="" />
                      <asp:Button ID="Button_tool_activa_enviar_usuario" runat="server" Text="" />
                      <asp:Button ID="Button_tool_enviar_usuario" runat="server" Text="" />
                      <asp:Button ID="Button_tool_devolver_a_usuario" runat="server" Text="" />
                      <asp:Button ID="Button_tool_devolver_a_actividades_anterior" runat="server" Text="" />
                      <asp:Button ID="Button_tool_busqueda_enviar_usuario" runat="server" Text="" />
                      <asp:Button ID="Button_tool_enviar_ruta" runat="server" Text="" />
                      <asp:Button ID="Button_tool_restore_busqueda_enviar_usuario" runat="server" Text="" />
                      <asp:Button ID="ButtonAlmacenar" Text="" runat="server" />
                      <asp:Button ID="Button_tool_activa_sube_documento_lista" runat="server" Text="" />
                      <input id="HiddenSeleccion" type="hidden" value="-1" runat="server" />
                      <input id="Hiddenconfirmacion" type="hidden" value="-1" runat="server" />
                      <input id="Hidden_resultado_selecion" type="hidden" value="NO" runat="server" />
                      <asp:Button ID="ButtonSeleccionUsuario" runat="server" Height="0px" Visible="True"
                          Width="0px" />
                      <asp:Button ID="ButtonSeleccionGrupo" runat="server" Height="0px" Visible="True"
                          Width="0px" />
                      <asp:Button ID="Button_detalle_radicado" runat="server" Height="0px" Visible="True"
                          Width="0px" />
                      <asp:Button ID="ButtonRecuperar" runat="server" Text="Asignar Tarea" ToolTip="Recuperar y asignar Tarea" />
                      <asp:Button ID="ButtonRecuperarReasignar" runat="server" Text="Reasignar Tarea" ToolTip="Recuperar y reasignar Tarea" />
                      <input id="Hidden_usuario_autoriza" type="hidden" value="" runat="server" />
                      <input id="Hidden_usuario_autoriza_id" type="hidden" value="0" runat="server" />
                      <input id="Hidden_00021_row" type="hidden" value="" runat="server" />
                      <input id="Hidden_value_search_especial" type="hidden" value = "" runat="server" />
                      <input id="Hidden_estado_obliga_sii" type="hidden" value = "0" runat="server" />
                  </ContentTemplate>
              </asp:UpdatePanel>
          </div>
          <input id="Hidden_url" type="hidden" value="" runat="server"/> 
          <input id="Hidden_extension" type="hidden" value="" runat="server"/> 
          <input id="HiddenPROMP" type="hidden" value="0" runat="server"/>
          <div id="invisible" style="display:none">
	                              <!---Boton de enlace de documento para almacenamiento--->
	                              
	                              <asp:Button ID="Buttonactualizar" runat="server" Text="(...)" ToolTip="Actualizar" ViewStateMode="Enabled" Height="0px" Width="0px" />
	                              <!---Boton eliminar archivos digitalizados--->
	                              <asp:Button ID="ButtonEliminarArchivos" runat="server" Height="0px" Visible="True"
	                                  Width="0px" />
	                               <asp:UpdatePanel ID="UpdatePanel6" runat="server" UpdateMode="Conditional" RenderMode="Inline">
	                                  <ContentTemplate>
	                                     
	                                      </ContentTemplate>
	                                   </asp:UpdatePanel>
                      </div>
          <div style="display: none">
              <asp:UpdatePanel ID="UpdatePanel_actualizar_seleccion_digitalizacion" runat="server" UpdateMode="Conditional">
                  <ContentTemplate>
                      <asp:Button ID="Button_actualizar_seleccion_digitalizacion" runat="server" Text="Actualizar" Style="display: none" OnClientClick="actualiza_treview_seleccion_escaner();" />
                      <input id="hidden_selecion_actualiza_treview_digitalizacion" type="hidden" value="" runat="server" />
                  </ContentTemplate>
              </asp:UpdatePanel>
          </div>
           <!--modal lista anexos SII--->
        <div class="modal fade modal_opacity" id="modal_sii_anexos_recibo" style="z-index:100065" role="dialog" aria-hidden="false" data-backdrop="false">      
            <div class="modal-dialog  modal-lg">
                <div class="modal-content">
                    <div id="header_modal_sii_anexos_recibo" >
                        <div class="modal-header" >
                            <h6 class="modal-title" style="color: black" >Anexos SII</h6>
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                        </div>
                    </div>      
                    <div class="modal-body" >     
                        <div class="row">
                            <div class="col-12">
                                <div class="conten_gred_border_ modal_bot_traf_table_" id="content_tabl_lista_sii_anexos_recibo" >
                                    <table  style="background-color: white"
                                        id="tabl_lista_sii_anexos_recibo"
                                        data-unique-id="registro" 
                                         data-pagination="true"
                                         data-show-footer="true"
                                         data-page-list="[5, 10, 25, 35, all]"
                                         data-locale="es-SP">
                                         <thead class="GridviewScrollHeader_line_boot">
                                           
                                         </thead>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>
                   <div id="error_content_sii_anexos_recibo" style="position: relative; width:100%"></div>
                    <div id="footer_modal_sii_anexos_recibo" >
                        <div class=" modal-footer justify-content-end" >
                             <input id="Button_Activa_guardar_Multiplex_anexos_sii" type="button" value="Guardar todos los anexos" class="btn  btn-primary d-none" />
                        </div>
                    </div> 
                </div>  
            </div>
        </div>
        <!--Termina popout lista anexos SII-->
          <!--Popup tipo tramite anexo sii -->
         <div class="modal fade modal_opacity" id="modal_guarda_archivos_anexos_sii" style="z-index:100066" role="dialog" aria-hidden="false" data-backdrop="false"  >
             <div class="modal-dialog modal-dialog-centered">
                 <div class="modal-content" >
                     <div class="modal-header">
                        <h4 style="color: black" class="modal-title">Guardar anexos </h4>
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>
                     <div class="modal-body">
                          <div class="row ">
                              <div class="col-4">
                                  <span style="color: black">Tipo documento</span>
                              </div>
                              <div class="col-8">
                                  <select style="color: black" id="option_lista_tipologia_anexo" class="form-select form-control w-100" >
				                             <option></option>
                                 </select>
                              </div>
                          </div>
                    </div>
                     <div id="error_tipo_guarda_anexo" style="position: relative; width:100%"></div>
                     <div class=" modal-footer">
                        <button type="button" id="Button_guarda_anexos_sii" class="btn  btn-primary"  title="Guarda anexo SII">Aceptar</button>
                     </div>
                 </div>
             </div>
         </div>     
        <!--Trmina Popup tipo tramite anexo sii-->
          <asp:Panel ID="Panel_list_imagenes_sii" runat="server" Style="display:none; width: 90%; height: 100%" CssClass="modal_content_general_">
            <asp:ModalPopupExtender ID="ModalPopupExtender_edition_list_imagenes_sii" runat="server"
                TargetControlID="ButtonSalir_list_imagenes_sii" BackgroundCssClass="FondoAplicacion"
                CancelControlID="Button_cerrar_list_imagenes_sii" PopupControlID="Panel_list_imagenes_sii">
            </asp:ModalPopupExtender>
            <div id="modal_content_list_imagenes_sii" class="modal-content">
                <div id="diver_cabcera_list_imagenes_sii" class="modal_title_superior_ modal-header">
                    <h6 class="modal-title d-inline ">Registros</h6>
                    <button type="button" value="Button_cerrar_list_imagenes_sii" class="close da_event_captive ">&times;</button>
                </div>
                <div id="contenido_procesa_list_imagenes_sii" style="background-color: white; width: 100%; height: 100%; border-top: none" class="modal_content_back modal-body">
                    <asp:UpdatePanel ID="Update_list_imagenes_sii" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>      
                            <div id="contenido_titulo_list_imagenes_sii" class="mb-2">  
                                <input id="Hidden_list_imagenes_sii" type="hidden" value="-1" runat="server"/>          
                                <asp:Label ID="titulo_label_list_imagenes_sii" runat="server" class="h6 font-weight-light">Resultados busqueda</asp:Label>
                            </div>
                            <div id="content_data_grid_list_imagenes_sii" class="conten_gred_border_" style="overflow: auto; width: 100%">
                                <asp:GridView ID="GridView_list_imagenes_sii" runat="server"  Style="position: inherit; width: 100%; font-size: 14px"
                                    AutoGenerateSelectButton="False" AllowSorting="true"  AllowPaging="true" PageSize  ="6" PagerSettings-Position="Top"  CssClass="table  font-weight-light" GridLines="None"
                                     EnableViewState="true">
                                    <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                    <HeaderStyle CssClass="GridviewScrollHeader_line_boot" />
                                    <PagerStyle CssClass="pagination-ys" />
                                    <Columns>
                                         <asp:BoundField HeaderText="OPCIONES"   />
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </ContentTemplate>

                        <Triggers>
                        </Triggers>
                    </asp:UpdatePanel>

                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button_list_imagenes_sii" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                        <asp:Button ID="ButtonSalir_list_imagenes_sii" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                        <asp:Button ID="Button_cerrar_list_imagenes_sii" runat="Server" Text="X" CssClass="invisible" />
                    </div>
                </div>
              
            </div>
        </asp:Panel>
            <asp:Panel ID="Panel_sube_documento_integra_sii" runat="server" Style="display:none;  width: 50%; height: auto" CssClass="modal_content_general_">
                <asp:ModalPopupExtender ID="ModalPopupExtender_sube_documento_integra_sii" runat="Server" BackgroundCssClass="FondoAplicacion" 
                    TargetControlID="Button_sube_documento_integra_sii"
                    PopupControlID="Panel_sube_documento_integra_sii" CancelControlID="Button3_sube_documento_integra_sii" ></asp:ModalPopupExtender>
                <div id="modal_content_sube_documento_integra_sii" class="modal-content">  
                    <div id="divcabecer2_sube_documento_integra_sii" class="modal_title_superior_ modal-header"> 
                        <h6 class="modal-title d-inline ml-1">Guardar</h6>  
                        <button type="button" value="Button3_sube_documento_integra_sii" class="close da_event_captive ">&times;</button>                   
                    </div>            
                    <div id="Div_content_sube_documento_integra_sii" style="height: auto; width: 100%; border-top: none" class="modal_content_back p-2">
                        <asp:UpdatePanel ID="UpdatePanel_drowp_sube_documento_integra_sii" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:DropDownList ID="DropDownList_documento_integra_sii" Style="width: 100%" CssClass="custom-select mr-sm-2" runat="server"></asp:DropDownList>
                            </ContentTemplate>
                            <Triggers>
                            </Triggers>
                        </asp:UpdatePanel>                     
                    </div>
                    <div id="content_boton_guardar_sii" class="modal-footer justify-content-end">
                        <asp:UpdatePanel ID="UpdatePanel_boton_sube_documento_integra_sii" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Button ID="Button_acepta_sube_documento_integra_sii" runat="server" Text="Aceptar" CssClass="btn  btn-success" />
                            </ContentTemplate>
                            <Triggers>
                            </Triggers>
                        </asp:UpdatePanel> 
                   </div>
                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button_sube_documento_integra_sii" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                        <asp:Button ID="Button3_sube_documento_integra_sii" runat="Server" Text="" CssClass="invisible" />
                    </div>
                </div>
            </asp:Panel>    
          <asp:Panel ID="Panel_list_inscripciones_sii" runat="server" Style="display:none; width: 90%; height: 100%" CssClass="modal_content_general_">
            <asp:ModalPopupExtender ID="ModalPopupExtender_edition_list_inscripciones_sii" runat="server"
                TargetControlID="ButtonSalir_list_inscripciones_sii" BackgroundCssClass="FondoAplicacion"
                CancelControlID="Button_cerrar_list_inscripciones_sii" PopupControlID="Panel_list_inscripciones_sii">
            </asp:ModalPopupExtender>
            <div id="modal_content_list_inscripciones_sii" class="modal-content">
                <div id="diver_cabcera_list_inscripciones_sii" class="modal_title_superior_ modal-header">
                    <h6 class="modal-title d-inline ">Inscripciones SII</h6>
                    <button type="button" value="Button_cerrar_list_inscripciones_sii" class="close da_event_captive ">&times;</button>
                </div>
                
                <div id="contenido_procesa_list_inscripciones_sii" style="background-color: white; width: 100%; height: 100%; border-top: none" class="modal_content_back modal-body">
                    <asp:UpdatePanel ID="Update_list_inscripciones_sii" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>      
                            <div id="contenido_titulo_list_inscripciones_sii" class="mb-2">  
                                <input id="Hidden_list_inscripciones_sii" type="hidden" value="-1" runat="server"/>          
                                <asp:Label ID="titulo_label_list_inscripciones_sii" runat="server" class="h6 font-weight-light">Resultados busqueda</asp:Label>
                            </div>
                            <div id="content_data_grid_list_inscripciones_sii" class="conten_gred_border_" style="overflow: auto; width: 100%">
                                <asp:GridView ID="GridView_list_inscripciones_sii" runat="server"  Style="position: inherit; width: 100%; font-size: 14px"
                                    AutoGenerateSelectButton="False" AllowSorting="true"  AllowPaging="true" PageSize  ="6" PagerSettings-Position="Top"  CssClass="table  font-weight-light" GridLines="None"
                                     EnableViewState="true">
                                    <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                    <HeaderStyle CssClass="GridviewScrollHeader_line_boot" />
                                    <PagerStyle CssClass="pagination-ys" />
                                    <Columns>
                                         <asp:BoundField HeaderText="OPCIONES"   />
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </ContentTemplate>

                        <Triggers>
                        </Triggers>
                    </asp:UpdatePanel>
                    
                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button_list_inscripciones_sii" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                        <asp:Button ID="ButtonSalir_list_inscripciones_sii" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                        <asp:Button ID="Button_cerrar_list_inscripciones_sii" runat="Server" Text="X" CssClass="invisible" />
                    </div>
                    <div id="div_error_content_adjunta_sello_sii" style="position: relative; width: 100%"></div>
                </div>
                 <div class="modal-footer justify-content-end" id="modal-footer_list_inscripciones_sii_">
                    <input id="Button_Activa_guardar_Multiplex_Constancias_sii_" type="button" value="Guardar todas las inscripciones" class="btn btn-success" />
                   
                </div>
            </div>
        </asp:Panel>
          <!--modal lista constancias SII--->
        <div class="modal fade modal_opacity" id="modal_sii_constancias_inscripcion" style="z-index:100065; height:100%" role="dialog" aria-hidden="false" data-backdrop="false">      
            <div class="modal-dialog  modal-lg">
                <div class="modal-content" style="height:100%">
                    <div id="header_modal_sii_constancias_inscripcion" >
                        <div class="modal-header" >
                            <h6 class="modal-title" style="color: black" >Constancias de inscripciones SII</h6>
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                        </div>
                    </div>      
                    <div class="modal-body" >     
                        <div class="row">
                            <div class="col-12">
                                <div class="conten_gred_border_ modal_bot_traf_table_" id="content_tabl_lista_sii_constancias_inscripcion" >
                                    <table  style="background-color: white"
                                        id="tabl_lista_sii_constancias_inscripcion"
                                        data-unique-id="registro" 
                                         data-pagination="true"
                                         data-show-footer="true"
                                         data-page-list="[5, 10, 25, 35, all]"
                                         data-locale="es-SP">
                                         <thead class="GridviewScrollHeader_line_boot">
                                           
                                         </thead>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>
                   <div id="error_content_sii_constancias_inscripcion" style="position: relative; width:100%"></div>
                    <div id="footer_modal_sii_constancias_inscripcion" >
                        <div class=" modal-footer justify-content-end" >
                             <input id="Button_Activa_guardar_Multiplex_Constancias_sii" type="button" value="Guardar todas las inscripciones" class="btn  btn-primary" />
                        </div>
                    </div> 
                </div>  
            </div>
        </div>
        <!--Termina popout lista constancias SII-->
         <!--Popup tipo tramite vinculacion -->
         <div class="modal fade modal_opacity" id="modal_guarda_archivos_inscripcion_sii" style="z-index:100066" role="dialog" aria-hidden="false" data-backdrop="false"  >
             <div class="modal-dialog modal-dialog-centered">
                 <div class="modal-content" >
                     <div class="modal-header">
                        <h4 style="color: black" class="modal-title">Guardar inscripiciones </h4>
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>
                     <div class="modal-body">
                          <div class="row ">
                              <div class="col-4">
                                  <span style="color: black">Tipo documento</span>
                              </div>
                              <div class="col-8">
                                  <select style="color: black" id="option_lista_tipologia" class="form-select form-control w-100" >
				                             <option></option>
                                 </select>
                              </div>
                          </div>
                 
                    </div>
                     <div id="error_tipo_tramite_vinculacion" style="position: relative; width:100%"></div>
                     <div class=" modal-footer">
                        <button type="button" id="Button_guarda_inscipciones_sii" class="btn  btn-primary"  title="Guarda inscripciones SII">Aceptar</button>
                     </div>
                 </div>
             </div>
         </div>     
        <!--Trmina Popup tipo tramite vinculacion-->
          <div id="lista_chequeo_tramite">
            <asp:Panel ID="Panel_lista_chequeo_tramite" runat="server" Style="display:none; color: White; width: 50%; height:auto; margin: auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_lista_chequeo_tramite" runat="server" BehaviorID="Panel_lista_chequeo_tramite" TargetControlID="ButtonSalir_lista_chequeo_tramite" BackgroundCssClass="ModalBackgroud_gorund"
                    CancelControlID="Button_cerrar_lista_chequeo_tramite" PopupControlID="Panel_lista_chequeo_tramite" ></asp:ModalPopupExtender>
                <div id="divcabecer2_lista_chequeo_tramite" class="modal_title_superior">
                    <asp:Label ID="Label_lista_chequeo_tramite" runat="server" Text="Tipo documental que desea Adjuntar" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton2_lista_chequeo_tramite" style="float: right">
                        <asp:Button ID="Button_cerrar_lista_chequeo_tramite" runat="Server" Text="X" CssClass="modal_boton_hiden"
                            ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_lista_chequeo_tramite" style="background-color: white; width: auto; height: auto; 
                  color: black; background-color: #FFFFFF" class="modal_content_back">                   
                        <div id="Contenedorgrid" style="width: 99%; position: inherit; left: 0px; top: 0px; text-align: left; height: auto; margin-top: 1px; border-color: #b0c4de; border-width: 1px; border-style: ridge">
		                           <asp:UpdatePanel ID="UpdateGeneral" runat="server" UpdateMode="Conditional" RenderMode="Inline">
		                               <ContentTemplate>
		                                   <input id="Hidden_0002" type="hidden" value="0" runat="server"/>
		                                   <input id="Hidden_0001" type="hidden" value="-1" runat="server"/>                       
		                                   <asp:Panel ID="Panel_principal" runat="server"
                                     Style="overflow: auto; width:100%; min-height:150px; max-height:150px">
		                                       <asp:GridView ID="data_grid_chequeo" runat="server" Style="position: inherit; font-family:Arial" AutoGenerateSelectButton="False" CssClass="filtrar" GridLines="None" Font-Size="12px" Width="100%">
		                                         <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                                 <HeaderStyle CssClass="GridviewScrollHeader_line_blanco" />
                                                 <RowStyle CssClass="GridviewScrollItem_line" />
                                                 <PagerStyle CssClass="GridviewScrollPager_line" />
		                                       </asp:GridView>
		                                   </asp:Panel>
		                                   
		                               </ContentTemplate>
		       
		                           </asp:UpdatePanel>
		       
                </div>
                    
                     <div style="margin-top: 1px; height: auto; text-align: right">
                        <asp:UpdatePanel ID="UpdatePanel_lista_chequeo" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <asp:TextBox ID="TextBox_contenido_busqueda_lista_cheq" runat="server" Style="width: auto; margin-left:3px;  margin-top: 5px" placeholder="Busqueda.." onkeypress="acti_busq_lista_cheq(event,this)"></asp:TextBox>
                                <input id="Hidden_list_cheo_acepta" type="hidden" value="" runat="server"/>
                                <asp:Button ID="Button_examinar_archivo_lista_chequeo" runat="server" Text="Aceptar" Style="margin-left: 5px; margin-top: 5px" CssClass="boton_azul" />
                                <asp:Button ID="Button_Actualizar_Lista_chequeo" runat="server" Text="Actualizar" Style="margin-top: 5px; display:none" CssClass="boton_azul" />
                                <asp:CheckBox ID="CheckBox_busqueda_list_cheq" runat="server" Style="display: none" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                   
                    <div style="overflow:auto">
                        <asp:UpdatePanel ID="UpdatePanel_lista_chequeo_estado" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <asp:Label ID="Label_estado_lista_chequeo" runat="server" Text="Estado" style="font-size:12px; display:none"></asp:Label>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
                 <asp:Button ID="Button_lista_chequeo_tramite" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_lista_chequeo_tramite" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
            </asp:Panel>
        </div>
          <asp:Panel ID="Panel_detalle_radicado" runat="server" Style="display:none; width: auto; height: auto" CssClass="modal_content_general_">
            <asp:ModalPopupExtender ID="ModalPopupExtender_edition_detalle_radicado" runat="server"
                TargetControlID="ButtonSalir_detalle_radicado" BackgroundCssClass="FondoAplicacion"
                CancelControlID="Button_cerrar_detalle_radicado" PopupControlID="Panel_detalle_radicado">
            </asp:ModalPopupExtender>     
            <div id="modal_content_detalle_radicado" class="modal-content">
                <div id="diver_cabcera_detalle_radicado" class="modal_title_superior_ modal-header">
                    <h6 class="modal-title d-inline ">Detalle radicado</h6>
                    <button type="button" value="Button_cerrar_detalle_radicado" class="close da_event_captive ">&times;</button>
                </div>
                <div id="contenido_procesa_detalle_radicado" style="background-color: white; width: 100%; height: 100%; border-top: none" class="modal_content_back modal-body">
                    <asp:UpdatePanel ID="Update_detalle_radicado" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Panel ID="Panel_detalle_radicado_user" runat="server" Style="height: 100%; width: 99.8%; margin-bottom: 1px; overflow:auto" CssClass="container">
                                <asp:Table ID="Table_detalle_radicado" runat="server" Style="height: 100%; width: 100%; font-size:12px" CssClass="table table-hover" >
                                    <asp:TableRow>
                                        <asp:TableCell>
                                            <asp:Label ID="Label1" runat="server" Text="RADICADO DEL TRAMITE"></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:Label ID="Label_RADICADO_TRAMITE" runat="server" Text=""></asp:Label>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                            <asp:Label ID="Label19_" runat="server" Text="PETICIONARIO O SOLICITANTE"></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:Label ID="LabelDESTINATARIO" runat="server" Text=""></asp:Label>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                            <asp:Label ID="Label12" runat="server" Text="RADICADO A NOMBRE DE "></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:Label ID="LabelASIGNADO" runat="server" Text=""></asp:Label>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                            <asp:Label ID="Label3" runat="server" Text="TIPO TRÁMITE RADICADO"></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:Label ID="Label_TIPO_TRAMITE" runat="server" Text=""></asp:Label>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                            <asp:Label ID="Label6" runat="server" Text="FECHA DE RADICACION DEL TRAMITE"></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:Label ID="LabelFECHA_REGISTRO" runat="server" Text=""></asp:Label>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                            <asp:Label ID="Label17" runat="server" Text="FECHA VENCIMIENTO DEL TRAMITE"></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:Label ID="Label_FECHA_VENCE" runat="server" Text=""></asp:Label>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                            <asp:Label ID="Label_asunto_radicado" runat="server" Text="ASUNTO"></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:Label ID="Label_ASUNTO_RADICADO_" runat="server" Text=""></asp:Label>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                            <asp:Label ID="Label4" runat="server" Text="FLUJO RADICADO"></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:Label ID="Label_FLUJO_RADICADO" runat="server" Text=""></asp:Label>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                            <asp:Label ID="Label20" runat="server" Text="USUARIO RADICADOR "></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:Label ID="Label_radicador_usuario" runat="server" Text=""></asp:Label>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                            <asp:Label ID="Label11" runat="server" Text="CARGO USUARIO RADICADOR"></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:Label ID="Label_CARGO_USUARIO_RADICADOR" runat="server" Text=""></asp:Label>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                            <asp:Label ID="Label9" runat="server" Text="SEDE USUARIO RADICADOR"></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:Label ID="Label_SEDE_USUARIO" runat="server" Text=""></asp:Label>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                </asp:Table>
                            </asp:Panel>
                        </ContentTemplate>
                        <Triggers>
                        </Triggers>
                    </asp:UpdatePanel>

                    <div style="display: none; height: 1px">
                        <asp:Button ID="ButtonSalir_detalle_radicado" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                        <asp:Button ID="Button_cerrar_detalle_radicado" runat="Server" Text="X" CssClass="invisible" />
                    </div>
                </div>
                <div class="modal-footer justify-content-end" id="modal-footer_detalle_radicado">
                    <asp:UpdatePanel ID="UpdatePanel_boton_detalle_radicado" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                           
                            
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
              
            </div>
        </asp:Panel>
              <!--lista_chequeo_actualiza-->
          <div id="lista_chequeo_actualiza">
              <asp:Panel ID="Panel_lista_chequeo_actualiza" runat="server" Style="display: none; color: White; width: 50%; height: auto; margin: auto" CssClass="modal_content_general">

                  <asp:ModalPopupExtender ID="ModalPopupExtender_edition_lista_chequeo_actualiza" runat="server" BehaviorID="Panel_lista_chequeo_actualiza" TargetControlID="ButtonSalir_lista_chequeo_actualiza" BackgroundCssClass="ModalBackgroud_gorund"
                      CancelControlID="Button_cerrar_lista_chequeo_actualiza" PopupControlID="Panel_lista_chequeo_actualiza">
                  </asp:ModalPopupExtender>
                  <div id="divcabecer2_lista_chequeo_actualiza" class="modal_title_superior">

                      <asp:Label ID="Label_lista_chequeo_actualiza" runat="server" Text="Cambiar tipo documento" Font-Size="10" Style="float: left">
                      </asp:Label>
                      <div id="Divcerrarbuton2_lista_chequeo_actualiza" style="float: right">
                          <asp:Button ID="Button_cerrar_lista_chequeo_actualiza" runat="Server" Text="X" CssClass="modal_boton_hiden"
                              ToolTip="Cerrar ventana" />
                      </div>
                  </div>
                  <div id="contenido_procesa_lista_chequeo_actualiza" style="background-color: white; width: auto; height: auto; color: black; background-color: #FFFFFF"
                      class="modal_content_back">

                      <div id="Contenedorgrid_edita" style="width: 99%; position: inherit; left: 0px; top: 0px; text-align: left; height: auto; margin-top: 1px; border-color: #b0c4de; border-width: 1px; border-style: ridge">
                          <asp:UpdatePanel ID="UpdateGeneral_actualiza" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                              <ContentTemplate>
                                  <input id="Hidden_0003" type="hidden" value="-1" runat="server" />
                                  <input id="Hidden_0004" type="hidden" value="1" runat="server" />
                                  <div id="contenido_titulo_data_grid_title_actualiza" style="width: 100%; margin-top: 1px; border-color: #b0c4de; border-width: 1px; border-style: ridge; display: none">
                                      <asp:Label ID="Label16" runat="server" ForeColor="Black" Font-Size="12px" Style="font-weight: 600">Seleccione el tipo documento</asp:Label>
                                  </div>
                                  <asp:Panel ID="Panel_principal_actualiza" runat="server"
                                      Style="overflow: auto; width: 100%; min-height: 150px; max-height: 150px">
                                      <asp:GridView ID="data_grid_chequeo_actualiza" runat="server" Style="position: inherit; font-family: Arial" AutoGenerateSelectButton="False" CssClass="filtrar" GridLines="None" Font-Size="12px" Width="100%">
                                          <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                          <HeaderStyle CssClass="GridviewScrollHeader_line" />
                                          <RowStyle CssClass="GridviewScrollItem_line" />
                                          <PagerStyle CssClass="GridviewScrollPager_line" />
                                      </asp:GridView>
                                  </asp:Panel>

                              </ContentTemplate>

                          </asp:UpdatePanel>

                      </div>
                      <div style="margin-top: 1px; height: auto; text-align: right">
                          <asp:UpdatePanel ID="UpdatePanel_lista_chequeo_actualiza" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                              <ContentTemplate>
                                  <asp:Button ID="Button_Actualizar_item_lista" runat="server" Text="Actualizar" Style="float: right; margin-top: 5px; margin-right: 5px" CssClass="boton_azul" />
                              </ContentTemplate>
                          </asp:UpdatePanel>
                      </div>
                      <br />
                      <div style="overflow: auto">
                          <asp:UpdatePanel ID="UpdatePanel_lista_chequeo_estado_actualiza" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                              <ContentTemplate>
                                  <asp:Label ID="Label_estado_lista_chequeo_actualiza" runat="server" Text="Estado" Style="font-size: 12px; display: none"></asp:Label>
                              </ContentTemplate>
                          </asp:UpdatePanel>
                      </div>
                  </div>
                  <asp:Button ID="Button_lista_chequeo_actualiza" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                  <asp:Button ID="ButtonSalir_lista_chequeo_actualiza" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
              </asp:Panel>
          </div>  
              

              <!--codigo cuadro de dialogo-->
              <asp:Panel ID="Panelmensaj" runat="server" Style="display: none" ForeColor="White" Width="250px" Height="160px" HorizontalAlign="Center">
                  <asp:ModalPopupExtender ID="ModalPopupExtendermensaje" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button4" PopupControlID="Panelmensaj">
                  </asp:ModalPopupExtender>
                  <div id="Div2" class="cabecera3">
                      <asp:Label ID="Label2" runat="server" Text="Popup worflow" Font-Size="10"></asp:Label>
                      <asp:Button ID="Button4" CssClass="invisible" runat="server" Text="Button" Height="0px" Width="0px" />
                  </div>              
                  <div id="container" style="border: thin double #000080; color: White; background-color: #FFFFFF; height: 87%; width: 247px">
                      <asp:UpdatePanel ID="updatepanel_mensaje_extender" runat="server" UpdateMode="Conditional">
                          <ContentTemplate>
                              <div id="Contenido" style="height: 60%; align-content:center">
                                  <br />
                                  <asp:Label ID="Label10" runat="server" Text="Desea terminar y enviar la tarea a:" ForeColor="Black" Font-Size="11" Visible="True" />
                                  <br />
                                  <asp:Label ID="LabelMensaje" runat="server" Text="No mensage" ForeColor="Black" Font-Size="9" Visible="True" />
                              </div>
                              <div id="Contenidbuton" style="height: 29%; color: White; background-color: #FFFFFF;">     
                                   <br />                     
                                  <asp:Button ID="btnOkay" runat="server" Text="Aceptar " CssClass="boton" />
                                  <asp:Button ID="btnCancel" runat="server" Text="Cancelar " CssClass="boton" />
                              </div>
                              <div id="Div3" style="height: 10%; color: White; background-color: #FFFFFF;">
                              </div>
                             
                          </ContentTemplate>
                      </asp:UpdatePanel>
                  </div>
              </asp:Panel>
                     <!--popup de auto terminar documento-->
             <asp:Panel ID="Panelmensaj_autoterminar" runat="server" Style="display:none; width:50%; height:auto" CssClass="modal_content_general">
                  <asp:ModalPopupExtender ID="ModalPopupExtendermensaje_autoterminar" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_auto" 
                      PopupControlID="Panelmensaj_autoterminar" CancelControlID="Button_auto">
                  </asp:ModalPopupExtender>
                 <div id="modal_content_autoterminar" class="modal-content">
                     <div id="Div2_autoterminar"  class="modal_title_superior_ modal-header">
                         <h6 class="modal-title d-inline ">Enviar a gestión</h6>
                         <button type="button" value="Button_auto" class="close da_event_captive ">&times;</button>                     
                     </div>
                     <div id="container_autoterminar" style="height: auto; width: auto" class="modal_content_back">
                         <asp:UpdatePanel ID="updatepanel_mensaje_extender_autoterminar" runat="server" UpdateMode="Conditional">
                             <ContentTemplate>
                                 <div id="Contenido_autoterminar" style="text-align:center" class="text-justify p-2">
                                     <br />
                                     <asp:Label ID="LabelMensaje_autoterminar" runat="server" Text="No mensage"   class="h6 font-weight-light " />
                                     <br />
                                     <br />
                                     <asp:CheckBox ID="CheckBox_autoterminar" runat="server" ForeColor="Black" Checked="true" Text="Enviar correo de notificación" class="h6" style="display:none" />
                                 </div>
                                 <input id="Hidden_id_actividad" type="hidden" value="0" runat="server"/>
                                 <input id="Hidden_id_usuario" type="hidden" value="0" runat="server"/>
                             </ContentTemplate>
                         </asp:UpdatePanel>
                     </div>
                     <div class="modal-footer justify-content-end">
                         <asp:UpdatePanel ID="UpdatePanel_boton_auto_terminar" runat="server" UpdateMode="Conditional">
                             <ContentTemplate>
                                 <asp:Button ID="btnOkay_autoterminar" runat="server" Text="Aceptar " CssClass="btn btn-success" />
                                     <asp:Button ID="btnCancel_autoterminar" runat="server" Text="Cancelar " CssClass="btn btn-secondary" />
                                      <input id="Hidden_result_auto_termnar" type="hidden" value="0" runat="server"/>
                                    
                             </ContentTemplate>
                         </asp:UpdatePanel>
                     </div>
                     <div style="display:none; height:1px">
                         <asp:Button ID="Button_auto" Style="display: none" runat="server" Text="Button" Height="0px" Width="0px" />
                     </div>
                 </div>
              </asp:Panel>
              <asp:Panel ID="Panel_interface_consulta_meta_dato" runat="server" Style="display: none; width: 100%; height: auto" CssClass="modal_content_general_">
              <asp:ModalPopupExtender ID="ModalPopupExtender_edition_interface_consulta_meta_dato" runat="server" BackgroundCssClass="FondoAplicacion" 
                  TargetControlID="ButtonSalir_interface_consulta_meta_dato"
                  CancelControlID="Button_cerrar_interface_consulta_meta_dato" PopupControlID="Panel_interface_consulta_meta_dato">
              </asp:ModalPopupExtender>
              <div id="modal_content_consulta_meta_dato" class="modal-content">
                  <div id="divcabecer2_interface_consulta_meta_dato" class="modal_title_superior_ modal-header">
                      <h6 id="label_interface_consulta_meta_dato" class="modal-title  ">Meta datos</h6>
                      <button type="button" value="Button_cerrar_interface_consulta_meta_dato" class="close da_event_captive">&times;</button>
                  </div>
                  <div id="contenido_procesa_interface_consulta_meta_dato" style="background-color: white; width: auto; height: auto; color: black; background-color: #FFFFFF; border-top: none; overflout: auto" class="modal_content_back modal-body">
                      <div id="div_content_tabla">
                          <table
                              id="table_meta_row"
                              data-height="400"
                              data-pagination="false"
                              data-page-list="[10, 25, 50, 100, all]"
                              data-show-export="true"
                              data-toggle="table"
                              data-id-field="ra_m_id"
                              data-search="true"
                              data-locale="es-SP">
                              <thead>
                                  <tr>
                                      <th data-field="ra_m_id" data-visible="false" style="display: none">id_meta_dato</th>
                                      <th data-field="Meta_dato" data-sortable="true" data-sort-name="Meta_dato" data-sort-order="desc">CAMPO</th>
                                      <th data-field="Valor_meta_dato">VALOR</th>
                                      <th data-field="Estado_obligatorio">OBLIGATORIEDAD</th>
                                      <th data-field="Estandar_meta_dato" data-sortable="true" data-sort-name="Estandar_meta_dato" data-sort-order="desc">ESTANDAR</th>
                                      <th data-field="descripcion">DESCRIPCION</th>
                                      <th data-field="Tipo">CONTEXTO</th>
                                  </tr>
                              </thead>
                          </table>
                      </div>
                  </div>
                 
                  <div style="display: none; height: 1px">
                      <asp:Button ID="Button_cerrar_interface_consulta_meta_dato" runat="Server" Text="" />
                      <asp:Button ID="Button_interface_consulta_meta_dato" runat="server" Text="" Height="1px" Width="1px" />
                      <asp:Button ID="ButtonSalir_interface_consulta_meta_dato" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                  </div>
              </div>
          </asp:Panel>
         
        <asp:Panel ID="Panel_actualiza_tipologia_documental_workflow" runat="server" Style="display:none; width: 40%; height: auto" CssClass="modal_content_general_">
            <asp:ModalPopupExtender ID="ModalPopupExtender_edition_actualiza_tipologia_documental_workflow" runat="server"
                TargetControlID="ButtonSalir_actualiza_tipologia_documental_workflow" BackgroundCssClass="FondoAplicacion"
                CancelControlID="Button_cerrar_actualiza_tipologia_documental_workflow" PopupControlID="Panel_actualiza_tipologia_documental_workflow">
            </asp:ModalPopupExtender>     
            <div id="modal_content_actualiza_tipologia_documental_workflow" class="modal-content">
                <div id="diver_cabcera_actualiza_tipologia_documental_workflow" class="modal_title_superior_ modal-header">
                    <h6 class="modal-title d-inline ">Tipologia documental</h6>
                    <button type="button" value="Button_cerrar_actualiza_tipologia_documental_workflow" class="close da_event_captive ">&times;</button>
                </div>
                <div id="contenido_procesa_actualiza_tipologia_documental_workflow" style="background-color: white; width: 100%; height: 100%; border-top: none" class="modal_content_back modal-body">
                    <asp:UpdatePanel ID="Update_actualiza_tipologia_documental_workflow" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>                          
                            <div id="content_data_grid_actualiza_tipologia_documental_workflow" class="conten_gred_border_" style="width: 100%">
                                <asp:DropDownList ID="DropDownList_tipologia_documental_workflow" style="width:100%" CssClass="custom-select mr-sm-2" runat="server"></asp:DropDownList>
                            </div>                           
                        </ContentTemplate>
                        <Triggers>
                        </Triggers>
                    </asp:UpdatePanel>

                    <div style="display: none; height: 1px">
                        <asp:Button ID="ButtonSalir_actualiza_tipologia_documental_workflow" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                        <asp:Button ID="Button_cerrar_actualiza_tipologia_documental_workflow" runat="Server" Text="X" CssClass="invisible" />
                    </div>
                </div>
                <div class="modal-footer justify-content-end" id="modal-footer_tipologia_documental_workflow">
                    <asp:UpdatePanel ID="UpdatePanel_boton_tipologia_documental_workflow" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Button ID="Button_actualiza_tipologia_documental_workflow" runat="server" Text="Aceptar" CssClass="btn btn-success" />
                            <input id="Hidden_resulta_botno_tipologia_documental_workflow" type="hidden" value="" runat="server"/>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
              
            </div>
        </asp:Panel>
          <!--Mensaje popup para paginas externas libres-->
          <div style="clear: both">
              <asp:Panel ID="PanelLibre" runat="server" Style="display:none; color: White; width: 100%; height: auto" CssClass="modal_content_general">
                  <asp:ModalPopupExtender ID="ModalPopupExtenderLibre" runat="Server"  Y="1" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonD"
                      PopupControlID="PanelLibre" CancelControlID="Buttoncabcel">
                  </asp:ModalPopupExtender>
                  <div id="Div6" class="modal_title_superior">            
                      <asp:Label ID="Labeladver" runat="server" Text="Ventana emergente" Font-Size="10" Style="float: left">                   
                      </asp:Label>
                      <div id="Div7" style="float: right">
                          <asp:Button ID="Buttoncabcel" runat="Server" Text="X"   CssClass="modal_boton_hiden"
                               ToolTip="Cerrar ventana" />
                      </div>
                  </div>
                  <div id="Div9" style="color: White; background-color: #FFFFFF; height: auto; width: 100%" class="modal_content_back">
                      <asp:UpdatePanel ID="UpdatePanelLibre" runat="server" UpdateMode="Conditional">
                          <ContentTemplate>
                              <iframe id="Iframelibre_" runat="server" frameborder="0"  scrolling="no" style="width:100%" ></iframe>
                          </ContentTemplate>
                      </asp:UpdatePanel>
                  </div>
                  <asp:Button ID="ButtonD" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" style="display:none" />
              </asp:Panel>
          </div>
          <!--popup traza_graficas-->
          <div style="clear: both">
              <asp:Panel ID="Paneltraza_grafica" runat="server" Style="display:none; width: 100%; height: auto" CssClass="modal_content_general_">
                  <asp:ModalPopupExtender ID="ModalPopupExtendertraza_grafica" runat="Server"  Y="1" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonD_trace_grafic"
                      PopupControlID="Paneltraza_grafica" CancelControlID="Buttoncabcel_trace_grafic">
                  </asp:ModalPopupExtender>
                  <div id="modal_content_grafica" class="modal-content">  
                      <div id="div_trace_grafic" class="modal_title_superior_ modal-header">
                            <h6 class="modal-title d-inline ml-1">Trazabilidad gráfica</h6>
                            <button type="button" value="Buttoncabcel_trace_grafic" class="close da_event_captive">&times;</button>   
                      </div>
                  <div id="div_content_trace_grafic" style="color: White; background-color: #FFFFFF; height: auto; width: 100%" class="modal_content_back">
                      <asp:UpdatePanel ID="UpdatePaneltraza_grafica" runat="server" UpdateMode="Conditional">
                          <ContentTemplate>
                              <iframe id="Iframetraza_grafica_" runat="server" frameborder="0"  scrolling="no" style="width:100%" ></iframe>
                          </ContentTemplate>
                      </asp:UpdatePanel>
                  </div>
                  </div>
                  <div style="display:none; height:1px">
                      <asp:Button ID="ButtonD_trace_grafic" CssClass="invisible" runat="server" Text="Button" Height="0px" Width="0px" style="display:none" />
                      <asp:Button ID="Buttoncabcel_trace_grafic" runat="Server" Text="" CssClass="modal_boton_hiden" />
                  </div>
                  
              </asp:Panel>
          </div>
           <!--Mensaje popup para paginas auxiliares-->
          <div style="clear: both">
              <asp:Panel ID="Panel_paginas_auxiliares" runat="server" Style="display:none; color: White; width: 30%; height:auto; margin:auto" CssClass="modal_content_general">
                  <asp:DragPanelExtender ID="DragPanelExtender_auxiliares" runat="server" TargetControlID="Panel_paginas_auxiliares" />
                  <asp:ModalPopupExtender ID="ModalPopupExtender_auxiliares" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_target_auxiliares"
                      PopupControlID="Panel_paginas_auxiliares" CancelControlID="Button_cancelar_auxiliares">
                  </asp:ModalPopupExtender>
                  <div id="Div_cabecera_auxiliares" class="modal_title_superior">
                      
                      <asp:Label ID="Label8" runat="server" Text="" Font-Size="10" Style="float: left; font-family:Arial">
                      </asp:Label>
                      <div id="Div_cerrar_auxiliares" style="float: right">
                         <asp:Button ID="Button_cancelar_auxiliares" runat="Server" Text="X" CssClass="modal_boton_hiden"
                               ToolTip="Cerrar ventana" />
                      </div>
                  </div>
                  <div id="Div_contenido_auxiliares" style=" color: White; background-color: #FFFFFF; height: 230px; width: 100%" class="modal_content_back">
                      <asp:UpdatePanel ID="UpdatePanel_auxiliares" runat="server" UpdateMode="Conditional">
                          <ContentTemplate>
                              <iframe id="Iframe_auxiliares_" runat="server" frameborder="0"  scrolling="no" style="width:100%; height:230px" ></iframe>
                          </ContentTemplate>
                      </asp:UpdatePanel>
                  </div>
                  <asp:Button ID="Button_target_auxiliares" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" style="display:none" />
              </asp:Panel>
          </div>
          <!--Popub busqueda !-->
          <div id="botonbusqueda" style="display: none">
              <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional">
                          <ContentTemplate>
                              <asp:Button ID="ButtonActivarBusqueda" runat="server" Text="Button" />
                          </ContentTemplate>
                  </asp:UpdatePanel>
              
          </div>
          <div id="busquda">
              <asp:Panel ID="Panelbusqueda" runat="server" Style="display:none; color: White; height:auto; width:40%; margin:auto "  CssClass="modal_content_general">
                  <asp:DragPanelExtender ID="DragPanelExtenderbusqueda" runat="server" TargetControlID="Panelbusqueda" />
                  <asp:ModalPopupExtender ID="ModalPopupExtenderbusqueda" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Buttond2"
                      PopupControlID="Panelbusqueda" CancelControlID="Buttoncacerrar">
                  </asp:ModalPopupExtender>
                  <div id="div_title_busqueda" class="modal_title_superior">
                      <asp:Label ID="Label_busqueda_label" runat="server" Text="Busqueda" Font-Size="10" Style="float: left">
                    </asp:Label>
                      <div id="Divcerrar_butonbusqueda" style="float: right">
                          <asp:Button ID="Buttoncacerrar" runat="Server" Text="X" CssClass="modal_boton_hiden"
                              ToolTip="Cerrar ventana" />
                      </div>
                    </div>                
                    
                  <div id="Diupdate" style="color:black; background-color: #FFFFFF; height:auto; width:100%" class="modal_content_back">
                      <asp:Label ID="Label5" runat="server" Text="Buscar en la lista de tareas" Font-Size="10" ForeColor="Black" Font-Names="arial" style="font-weight:600; display:none"></asp:Label>   
                          <asp:UpdatePanel ID="updatepanel_busqueda" runat="server" UpdateMode="Conditional">
                              <ContentTemplate>
                                  <br />
                                  <br />
                                  <asp:TextBox ID="contenidobusqueda" runat="server" Style="width: 85%; margin-left: 5px" placeholder="Buscar en lista de tareas..."></asp:TextBox>
                                  <asp:Button ID="Buttonbuscar" runat="server" Text="Buscar" OnClientClick="activa_busqueda();" CssClass="boton_azul" style="margin-left:3px" />
                                  <br />    
                                  <asp:CheckBox ID="checkbox" runat="server" Text="Buscar sólo palabra completa" Font-Size="9" Font-Names="arial" Style="float: right; margin-right: 15px; margin-bottom:20px" />
                                  <br />
                              </ContentTemplate>
                          </asp:UpdatePanel>
                      
                      
                  </div>
                  <asp:Button ID="Buttond2" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" style="display:none" />
                  
              </asp:Panel>
          </div>
            <div id="Filtro">
              <asp:Panel ID="Panel_filtro" runat="server" Style="display: none; color: White; width: auto; height: auto">
                 
                  <asp:ModalPopupExtender ID="ModalPopupExtender_Filtro" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Buttond_Filtro"
                      PopupControlID="Panel_filtro" CancelControlID="Button_Filtro_Cerrar">
                  </asp:ModalPopupExtender>
                  <div id="divcabecer_filtro" class="cabecera2">
                      <asp:Button ID="Buttond_Filtro" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                      <asp:Label ID="Label7" runat="server" Text="Filtrar Lista" Font-Size="10" Style="float: left">
                      </asp:Label>
                      <div id="Divcerrarbuton_filtro" style="float: right">
                          <asp:Button ID="Button_Filtro_Cerrar" runat="Server" Text="X"
                              ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />
                      </div>
                  </div>
                
                  <div id="Diupdate_filtro" style="border: thin double #000080; color: White; background-color: #FFFFFF; height: auto; width: auto">
                      <div id="Contenidopagina_filtro" style="height: 140px; width: 450px; overflow: no-display; color: black; margin-left: 15px">
                          <asp:UpdatePanel ID="updata_panel_filtro" runat="server" UpdateMode="Conditional">
                              <ContentTemplate>
                                  <br />
                                  <label style="font-family: Arial; font-size: 11px">Busqueda Tareas a filtrar </label>
                                  <br />
                                  <label style="font-family: Arial; font-size: 11px">Digita Texto </label>
                                  <asp:TextBox ID="contenidobusqueda_filtro"  runat="server"></asp:TextBox>
                                  <asp:Button ID="ButtonFiltro"  Text="Aceptar" runat="server" class="boton" OnClientClick="activa_filtro();"/>
                                  <asp:CheckBox ID="CheckboxBusqueda_f" runat="server" Text="Sólo palabras completas" Font-Size="10" Font-Names="arial"  />
                                  <br />
                              </ContentTemplate>
                          </asp:UpdatePanel>

                      </div>

                  </div>
                  <div id="border_filtro" style="color: white; font-size: small; background-color: #053061; width: 470px; height: 10px">
                  </div>
                 
              </asp:Panel>
          </div>
        
           <!--Popup respuesta radicado--> 
          <div>
              <asp:Panel ID="Panel_respuesta_radicado" runat="server" Style="display:none; width:99%; height:auto"    CssClass="modal_content_general_">
                  <asp:ModalPopupExtender ID="ModalPopup_respuesta_radicado" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_respuesta_radicado"
                      PopupControlID="Panel_respuesta_radicado"  CancelControlID="ButtonCerrar_respuesta_radicado"></asp:ModalPopupExtender>
                  <div id="modal_content_respuesta_radicado" class="modal-content">
                      <div id="Cabecera_respuesta_radicado" class="modal_title_superior_ modal-header" style="">
                             <h6 class="modal-title d-inline ml-1">Repuesta tramite</h6>
                             <button id="Button_cerrar_ventana" type="button" onclick="activa_boton_cerrar();" class="close">&times;</button>   
                      </div>
                      <div id="contenido_procesa_respuesta_radicado" style="height: 100%; width: 100%" class="modal_content_back_">

                          <asp:UpdatePanel ID="UpdatePanel_respuesta_radicado" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                              <ContentTemplate>
                                  <iframe id="Iframe_respuesta_radicado_" runat="server" frameborder="0" style="width: 100%; height: 100%; overflow: auto"></iframe>
                                  <asp:Button ID="Button_activa_respuesta_radicado" runat="server" Text="Button" Style="display: none" OnClientClick="auto_zise_popup_respuesta();" />
                                  <asp:Button ID="Button_activa_respuesta_radicado_tag" runat="server" Text="Button" Style="display: none" />
                                  <input id="Hidden_radicado" type="hidden" value="" runat="server"/>
                                  <input id="Hidden_id_respuesta" type="hidden" value="-1" runat="server"/>
                              </ContentTemplate>

                          </asp:UpdatePanel>
                      </div>
                  </div>
                  <div style="display: none; height: 1px">
                      <asp:Button ID="ButtonSalir_respuesta_radicado" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                      <asp:Button ID="ButtonCerrar_respuesta_radicado" runat="Server" Text=""
                           Height="0px" Style="display: none" />
                      <asp:UpdatePanel ID="UpdatePanel_valida_cerra_respuesta_radicado" runat="server" UpdateMode="Conditional">
                          <ContentTemplate>
                              <asp:Button ID="Button_valida_Cerrar_respuesta_radicado" runat="Server" Text="hiden"
                                  Style="display: none" />
                          </ContentTemplate>
                      </asp:UpdatePanel>
                  </div>
              </asp:Panel>
          </div>             
              
          <!--envia_documento_pendiente_apro-->
          <div id="envia_documento_pendiente_apro" style="width:auto">
              <asp:Panel ID="Panel_envia_documento_pendiente_apro" runat="server" Style="display:none; width: 50%; height:auto" CssClass="modal_content_general">
                  <asp:ModalPopupExtender ID="ModalPopupExtender_edition_envia_documento_pendiente_apro" runat="server"  TargetControlID="ButtonSalir_envia_documento_pendiente_apro" BackgroundCssClass="FondoAplicacion"
                      CancelControlID="Button_cerrar_envia_documento_pendiente_apro" PopupControlID="Panel_envia_documento_pendiente_apro">
                  </asp:ModalPopupExtender>
                  <div id="modal_content_pendiente_apro" class="modal-content">  
                  <div id="divcabecer2_envia_documento_pendiente_apro" class="modal_title_superior_ modal-header">
                        <h6 id="h6_envia_documento_pendiente_apro" class="modal-title d-inline ml-1">Enviar a pendiente</h6>
                        <button type="button" value="Button_cerrar_envia_documento_pendiente_apro" class="close da_event_captive">&times;</button>   
                  </div>
                  <div id="contenido_procesa_envia_documento_pendiente_apro"  style="background-color: white; width:auto; height: 100%;   overflow:auto" class="modal_content_back modal-body" >
                      <asp:UpdatePanel ID="UpdatePanel_envia_documento_pendiente_apro" runat="server" UpdateMode="Conditional">
                          <ContentTemplate>
                              <div class="row">
                                  <div id="div_content_span_pent" class="col-4">
                                      <span id="span_nota_pent"> Nota de la tarea
                                      </span>
                                  </div>
                                  <div id="div_content_text_pent" class="col-8">
                                      <asp:TextBox ID="TextBox_texto_pendiente_aprobacion" runat="server" Style="width: 100%" CssClass="form-control"></asp:TextBox>
                                  </div>
                              </div>                 
                          </ContentTemplate>
                      </asp:UpdatePanel>    
                      
                  </div>
                      <div class="modal-footer">
                          <asp:UpdatePanel ID="UpdatePanel_buton_envia_documento_pendiente_apro" runat="server" UpdateMode="Conditional">
                              <ContentTemplate>     
                                      <asp:Button ID="Button_aceptar_envia_documento_pendiente_apro" runat="server" Text="Aceptar" CssClass="btn btn-success" />
                                      <asp:Button ID="Button_cancelar_envia_documento_pendiente_apro" runat="server" Text="Cancelar" style="display:none" CssClass="btn  btn-light" /> 
                                      <input id="Hidden_000_estado" type="hidden" value="" runat="server"/>
                                      <input id="Hidden_0001_estado" type="hidden" value="" runat="server"/>
                              </ContentTemplate>
                          </asp:UpdatePanel>
                          <button type="button" value="Button_cerrar_envia_documento_pendiente_apro" class=" da_event_captive btn  btn-light"> Cancelar </button>   
                      </div>  
                  </div>
                  <div style="display:none; height:1px">
                       <asp:Button ID="Button_envia_documento_pendiente_apro" style="display:none" runat="server" Text="" Height="0px" Width="0px" />
                       <asp:Button ID="ButtonSalir_envia_documento_pendiente_apro" style="display:none" runat="server" Text="" Height="0px" Width="0px" />
                      <asp:Button ID="Button_cerrar_envia_documento_pendiente_apro" runat="Server" Text=""  style="display:none"
                              />
                  </div>  
              </asp:Panel>
        </div>
          <!--autoriza reasignacion-->
          <div id="autoriza_reasignacion_tarea">
            <asp:Panel ID="Panel_autoriza_reasignacion_tarea" runat="server" Style="display:none; color: White; width: 600px; height: 200px">

                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_autoriza_reasignacion_tarea" runat="server" BehaviorID="Panel_autoriza_reasignacion_tarea" TargetControlID="ButtonSalir_autoriza_reasignacion_tarea" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_autoriza_reasignacion_tarea" PopupControlID="Panel_autoriza_reasignacion_tarea" ></asp:ModalPopupExtender>
                <div id="divcabecer2_autoriza_reasignacion_tarea" class="cabecera2">
                    <asp:Button ID="Button_autoriza_reasignacion_tarea" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_autoriza_reasignacion_tarea" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Label ID="Label_autoriza_reasignacion_tarea" runat="server" Text="Autoriza reasignación" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton2_autoriza_reasignacion_tarea" style="float: right">
                        <asp:Button ID="Button_cerrar_autoriza_reasignacion_tarea" runat="Server" Text="X"
                            ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_autoriza_reasignacion_tarea" style="background-color: white; width: 100%; height: 99%;border: thin double #000080; color: black; background-color: #FFFFFF;">
                                
                    
                        <asp:UpdatePanel ID="UpdatePanel_autoriza_reasignacion_tarea" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                               <br />
                                <table style="width: 100%;">
                                   
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label_user_autoriza_reasignacion_tarea" runat="server" Text="Usuario autorizado*" Style="text-align: center; font-family: Arial; font-size: 14px"></asp:Label>
                                        </td>
                                        <td><asp:TextBox ID="TextBox_login_autoriza_reasignacion_tarea" runat="server" Style="width:300px"></asp:TextBox></td>
                                       
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label_dest_autoriza_reasignacion_tarea" runat="server" Text="Contraseña usuario*" Style="text-align: center; font-family: Arial; font-size: 14px"></asp:Label>

                                        </td>
                                        <td>
                                            <asp:TextBox ID="TextBox_pasw_autoriza_reasignacion_tarea" runat="server" Style="width:300px"  TextMode="Password"></asp:TextBox> 
                                           

                                        </td>                           
                                    </tr>
                                    <tr>
                                        <td></td>
                                    </tr>
                                    
                                    <tr>
                                        <td>

                                        </td>
                                        <td style="float:left"><asp:Button ID="Button_autoriza_reasignacion" runat="server" Text="Reasignar" Style="background-color: white; border-color: #b0c4de; height: 30px; width: 200px; height: 25px; text-align: center" CssClass="boton" /> &nbsp &nbsp
                                                         
                                        </td>
                                    </tr>
                                    
                                    
                                </table>
                                                         
                            </ContentTemplate>
                        </asp:UpdatePanel>
                         
                </div>
            </asp:Panel>
        </div>
          <!--detalle respuesta-->
           <asp:Panel ID="Panel_detalle_respuesta" runat="server" Style="display:none; overflow:hidden"  Width="95%" Height="100% " CssClass="modal_content_general_">
                  <asp:ModalPopupExtender ID="ModalPopupExtender_detalle_respuesta" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_detalle_respuesta"
                      PopupControlID="Panel_detalle_respuesta"  CancelControlID="ButtonSalir_detalle_respuesta">
                  </asp:ModalPopupExtender>
               <div id="modal_content_detalle_respuesta" class="modal-content">
                   <div id="Cabecerapendiente_detalle_respuesta" class="modal_title_superior_ modal-header">
                       <h6 class="modal-title d-inline ml-1"></h6>
                       <button type="button" value="ButtonSalir_detalle_respuesta" class="close da_event_captive">&times;</button>
                   </div>
                   <div id="Cotenedorpendiente_detalle_respuesta" style="color: Black; background-color: #FFFFFF; height: 90%; width: 100%; overflow: hidden" class="modal_content_back">
                       <asp:UpdatePanel ID="UpdatePanel_detalle_respuesta" runat="server" UpdateMode="Conditional">
                           <ContentTemplate>
                               <iframe id="Iframe_visor_externo__" runat="server" frameborder="0" style="width: 100%; height: 100%; overflow: hidden"></iframe>
                           </ContentTemplate>

                       </asp:UpdatePanel>

                   </div>
               </div>
                   <div style="display:none; height:1px">
                   <asp:Button ID="Button_detalle_respuesta" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="ButtonSalir_detalle_respuesta" runat="Server" Text="" CssClass="invisible"/>
                  </div>
              </asp:Panel>
          <!--detalle trazabilidad-->
           <asp:Panel ID="Panel_trazabilidad" runat="server" Style="display:none; overflow:hidden; width:70%; height:100%"  CssClass="modal_content_general" >
                  <asp:ModalPopupExtender ID="ModalPopupExtender_trazabilidad" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_trazabilidad_dos"
                      PopupControlID="Panel_trazabilidad"  CancelControlID="ButtonSalir_trazabilidad">
                  </asp:ModalPopupExtender>
               <div id="modal_content_trazabilidad" class="modal-content_">  
                  <div id="Cabecerapendiente_trazabilidad" class="modal_title_superior_ modal-header">   
                        <h6 class="modal-title d-inline ml-1">Trazabilidad</h6>
                        <button type="button" value="ButtonSalir_trazabilidad" class="close da_event_captive">&times;</button>                     
                  </div>
                  <div id="Cotenedorpendiente_trazabilidad" style="height: 90%; width: 100%" class="modal_content_back_">          
                      <asp:UpdatePanel ID="UpdatePanel_trazabilidad" runat="server" UpdateMode="Conditional">
                          <ContentTemplate>
                              <iframe id="Iframe_trazabilidad_" runat="server" frameborder="0"  style="width:100%; height:100%; overflow:hidden"></iframe>
                          </ContentTemplate>
                      </asp:UpdatePanel>                    
                  </div>
               </div>
                   <div style="display:none; height:1px">
                       <asp:Button ID="Button_trazabilidad_dos" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px"/>
                       <asp:Button ID="ButtonSalir_trazabilidad" runat="Server" Text="" CssClass="modal_boton_hiden"  />
                   </div>
              </asp:Panel>
          <!--compartir documento-->
          <div id="autoriza_compartir_documento">
            <asp:Panel ID="Panel_autoriza_compartir_documento" runat="server" Style="display:none;  width: 100%; height: 100%" CssClass="modal_content_general_">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_autoriza_compartir_documento"  runat="server"   TargetControlID="ButtonSalir_autoriza_compartir_documento" 
                     BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_autoriza_compartir_documento" PopupControlID="Panel_autoriza_compartir_documento" ></asp:ModalPopupExtender>
                <div id="modal_content_compartir_documento" class="modal-content">
                    <div id="divcabecer2_autoriza_compartir_documento" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title d-inline ml-1">Compartir</h6>
                        <button type="button" value="Button_cerrar_autoriza_compartir_documento" class="close da_event_captive">&times;</button>     
                    </div>
                    <div id="contenido_procesa_autoriza_compartir_documento" style="width: 100%; height: 99%" class="modal_content_back">
                        <asp:UpdatePanel ID="UpdatePanel_autoriza_compartir_documento" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <iframe id="Iframe_compartir_documento_" runat="server" frameborder="0" style="width: 100%; height: 100%; overflow: hidden"></iframe>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                    </div>
                    <div style="display:none; height:1px">
                    <asp:Button ID="Button_autoriza_compartir_documento" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="ButtonSalir_autoriza_compartir_documento" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="Button_cerrar_autoriza_compartir_documento" runat="Server" Text="" CssClass="invisible" />
                    </div>
                </div>
            </asp:Panel>
        </div>
              <!--Confirma reasigna respuesta-->
        <div id="confirma_reasigna_responsable_tramite">
            <asp:Panel ID="Panel_confirma_reasigna_responsable_tramite" runat="server" Style="display:none; color: White; width: 400px; height: 130px" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_confirma_reasigna_responsable_tramite" runat="server" BehaviorID="Panel_confirma_reasigna_responsable_tramite_ModalPopupExtender" TargetControlID="ButtonSalir_confirma_reasigna_responsable_tramite" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_confirma_reasigna_responsable_tramite" PopupControlID="Panel_confirma_reasigna_responsable_tramite" ></asp:ModalPopupExtender>
                <div id="div8" class="modal_title_superior">                 
                    <asp:Label ID="Label_confirma_reasigna_responsable_tramite" runat="server" Text="Reasigna respuesta tramite" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton2_confirma_reasigna_responsable_tramite" style="float: right">
                        <asp:Button ID="Button_cerrar_confirma_reasigna_responsable_tramite" runat="Server" Text="X" CssClass="modal_boton_hiden"
                             ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_confirma_reasigna_responsable_tramite" style="background-color: white; width: 100%; height: 99%; color: black; background-color: #FFFFFF;" class="modal_content_back">        
                        <asp:UpdatePanel ID="UpdatePanel_contenido_confirma_reasigna_responsable_tramite" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                               <br />
                                <table style="width: 100%;">                    
                                    <tr >
                                        <td style=" width:300px; padding-left:30px; padding-bottom:20px">
                                            <asp:Label ID="Label_destinatario_confirma_reasgina" runat="server" Text="Desea reasignarse la responsabilidad de responder esta solicitud?" Style="text-align: center; font-family: Arial; font-size: 13px; font-weight:600"></asp:Label>

                                        </td>
                                                                  
                                    </tr>
                                    <tr>
                                        <td> </td>
                                    </tr>
                                    
                                    <tr>
                                        
                                        <td style=" text-align:right">
                                            <asp:Button ID="Button_autoriza_confirma_reasigna" runat="server" Text="Aceptar" Style="width: 100px" CssClass="boton_azul" /> 
                                             <asp:Button ID="Button_cancela_confirma_reasigna" runat="server" Text="Cancelar" Style="width: 100px" CssClass="boton_blanco" /> 
                                                         
                                        </td>
                                    </tr>
                                    
                                    
                                </table>
                                                         
                            </ContentTemplate>
                        </asp:UpdatePanel>
                         
                </div>
                <asp:Button ID="Button_confirma_reasigna_responsable_tramite" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_confirma_reasigna_responsable_tramite" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
            </asp:Panel>
           
        </div>
       <!--Modal reasigna respuesta-->
            <div id="reasigna_responsable_tramite">
                <asp:Panel ID="Panel_reasigna_responsable_tramite" runat="server" Style="display:none;  width: auto; height: auto" CssClass="modal_content_general_ ">
                    <asp:ModalPopupExtender ID="ModalPopupExtender_edition_reasigna_responsable_tramite" runat="server"  TargetControlID="ButtonSalir_reasigna_responsable_tramite" BackgroundCssClass="FondoAplicacion"
                        CancelControlID="Button_cerrar_reasigna_responsable_tramite" PopupControlID="Panel_reasigna_responsable_tramite" ></asp:ModalPopupExtender>
                    <div class="modal-content">
                        <div id="div10" class="modal_title_superior_ modal-header">
                             <h6 class="modal-title">Reasigna respuesta tramite</h6>
                            <button type="button" value="Button_cerrar_reasigna_responsable_tramite" class="close da_event_captive">&times;</button>
                        </div>
                        <div id="contenido_procesa_reasigna_responsable_tramite" style="background-color: white; width: auto; height: auto; color: black; background-color: #FFFFFF;" class="modal_content_back modal-body">
                            <div  class=" col-12">
                                <span>
                                    Usuario autorizado*
                                </span>       
                            </div>
                             <div  class=" col-6">
                                  <asp:TextBox ID="TextBox_login_autoriza_reasigna" runat="server" Style="width: 300px" CssClass="form-control"></asp:TextBox>
                            </div>
                            <br />
                            <div  class=" col-12">
                                  
                                <span>
                                    Contraseña usuario*
                                </span>
                            </div>
                             <div  class=" col-6">
                                 <asp:TextBox ID="TextBox_pasw_autoriza_reasigna" runat="server" Style="width: 300px" TextMode="Password"></asp:TextBox>
                            </div>
                            
                        </div>
                        <div class="modal-footer">
                            <asp:UpdatePanel ID="UpdatePanel_contenido_reasigna_responsable_tramite" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Button ID="Button_autoriza_reasigna" runat="server" Text="Aceptar" Style="float: right" CssClass="btn btn-success" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <button type="button" value="Button_cerrar_reasigna_responsable_tramite" class="btn  btn-light da_event_captive">Cancelar</button>
                        </div>
                        <div style="display: none; height: 1px">
                            <asp:Button ID="Button_reasigna_responsable_tramite" CssClass="invisible bg-transparent" runat="server" Text="" Height="1px" Width="1px" />
                            <asp:Button ID="ButtonSalir_reasigna_responsable_tramite" CssClass="invisible bg-transparent" runat="server" Text="" Height="1px" Width="1px" />
                            <asp:Button ID="Button_cerrar_reasigna_responsable_tramite" runat="Server" Text="" />
                        </div>         
                    </div>
                </asp:Panel>
               
        </div>   
          
          <!--cargar documento anexo!-->
          <div id="contenido_procesa_sube_documento_adjunto" >
            <asp:Panel ID="Panel_sube_documento_adjunto" runat="server" Style="display:none;  width: 70%; height: 100%" CssClass="modal_content_general_">
                <asp:ModalPopupExtender ID="ModalPopupExtender_sube_documento_adjunto" runat="Server" BackgroundCssClass="FondoAplicacion" 
                    TargetControlID="Button_sube_documento_adjunto"
                    PopupControlID="Panel_sube_documento_adjunto" CancelControlID="Button3_cerrar_adjunta" ></asp:ModalPopupExtender>
                <div id="modal_content_sube_documento_adjunto" class="modal-content">  
                    <div id="Div_cabecera" class="modal_title_superior_ modal-header"> 
                        <h6 class="modal-title d-inline ml-1">Adjuntar</h6>  
                        <button type="button" value="Button3_cerrar_adjunta_" onclick="hide_upload_content('ModalPopupExtender_sube_documento_adjunto');" class="close da_event_captive_ ">&times;</button>                   
                   </div>            
                    <div id="Div_contenido_adjunta" style="height: auto; width: 100%; border-top: none" class="modal_content_back p-2">
                        <div id="content_option_chek_adjunto_doc_visor"> 
                        <asp:UpdatePanel ID="Update_actualiza_adjunta_documento" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:MutuallyExclusiveCheckBoxExtender ID="MutuallyExclusiveCheckBoxExtender1_" runat="server" TargetControlID="Check_anexo_radicado_adj"
                                     Key ="radicado_"></asp:MutuallyExclusiveCheckBoxExtender>
                                    <asp:mutuallyexclusivecheckboxextender id="Mutuallyexclusivecheckboxextender2_" runat="server" targetcontrolid="CheckBox_relacionado_radicado_adj"
                                     Key ="radicado_"></asp:mutuallyexclusivecheckboxextender>  
                                <div class=" row pl-1 pr-1">         
                                    <div class="col-6">
                                       <div class="row">
                                            <div class="col-0 pl-3 pr-2 pb-2 pt-2">
                                                <asp:CheckBox ID="Check_anexo_radicado_adj" runat="server" Text="" Checked="false"  Font-Size="11"  onchange="upload_adjunto_doc_visor_event_cheked_adjunto(event)"  Enabled="true" CssClass="h6 font-weight-light"  />
                                            </div>
                                           <div class="col-8 pl-0  pr-0 pb-2 pt-2">
                                               <asp:Label ID="h_adjunto_adjunto_doc_visor" class="pl-0 font-weight-light h6"  runat="server" Text="Adjuntar como parte del documento"></asp:Label>
                                                   
                                            </div>
                                       </div>
                                        
                                    </div>
                                     <div class="col-6">
                                          <div class="row">
                                              <div class="col-0 pl-0 pr-2 pb-2 pt-2">
                                                   <asp:CheckBox ID="CheckBox_relacionado_radicado_adj" runat="server" Text="" Checked="true" onchange="upload_adjunto_doc_visor_event_cheked_relacion(event)"  Font-Size="11" CssClass="h6 font-weight-light" Enabled="true" />
                                              </div>
                                              <div class="col-8 pl-0  pr-0 pb-2 pt-2">
                                                  <asp:Label ID="h_relacion_adjunto_doc_visor" class="pl-0 font-weight-light h6"  runat="server" Text="Adjuntar como documento relacionado"></asp:Label>
                                                 
                                              </div>
                                          </div>
                                        
                                    </div>
                                </div>                  
                                <div id="content_data_grid_adjunta_documento" class="conten_gred_border_" style="width: 100%">
                                     <input id="HiddenField_sube_documento_adjunto" type="hidden" value="0" runat="server"/>
                                    <asp:DropDownList ID="DropDownList_adjunta_documento" Style="width: 100%" CssClass="custom-select mr-sm-2" runat="server"></asp:DropDownList>
                                </div>
                            </ContentTemplate>
                            <Triggers>
                            </Triggers>
                        </asp:UpdatePanel>
                        </div>
                        <div class="p-2">
                            <div class="row p-2" id="content_boton_adjunto_doc_visor">
                                <div class="col-12 p-0">
                                    <div class="file-select " id="src-file">
                                        <input id="file_element_adjunto_doc_visor" type="file" multiple="multiple"  accept=""  style="width: 100px; height: 40px" name="src-file" class="p-1" contente_file="ModalPopupExtender_sube_documento_adjunto" aria-label="Archivo"  />
                                    </div>
                                    <a id="save_file_element_adjunto_doc_visor" title="Guardar todos los archivos"  class="btn  btn-success" style="opacity: 0; color:white"><i style="color: white" class="fas fa-save "></i> Guardar </a>   
                                    <a id="delete_file_element_adjunto_doc_visor" title="Elminar todos los archivos cargados"  class="btn  btn-danger " style="opacity: 0; color:white"><i style="color: white" class="fal fa-trash-alt "></i> Eliminar </a>
                                     <a id="cancel_file_element_adjunto_doc_visor" title="Cancelar guardar archivos"  class="btn  btn-warning" style="opacity: 0; color:white"><i style="color: white" class="fas fa-window-close "></i> Cancelar </a>
                                </div>
                            </div>            
                            <div class="paren_element background_upload" id="conten_file_element_adjunto_doc_visor" style="overflow: auto; height: 100%">
                                
                                  <div id="content_drop_element_adjunto_doc_visor" claas="">
                                                    
                                 </div>
                                 <table id="table_file_element_adjunto_doc_visor" class="table table-striped">
                                 </table>
                            </div>
                            <div class="row border pt-2" id="content_pie_title_adjunto_doc_visor">
                                <div class="col-8">
                                    <div class="row p-2">
                                        <div class="col-4 p-0">
                                            <div >
                                                <asp:Label ID="Label_progres_bar_file_element_adjunto_doc_visor" runat="server" Text="" Style="font-family: Arial; text-align: center; font-size: 20px"></asp:Label>
                                            </div>
                                            <div id="pogres_file_element_contador_adjunto_doc_visor" style="text-align: center; font-family: Arial; font-size: 14px">
                                            </div>
                                            <div id="pogres_file_element_porcent_adjunto_doc_visor" style="text-align: center; font-family: Arial; font-size: 14px">
                                            </div>
                                        </div>
                                        <div class="col-5 p-0">
                                            <div>
                                                <div id="myProgress_file_element_adjunto_doc_visor">
                                                    <div id="myBar_file_element_adjunto_doc_visor" class="file-select-bar"></div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-3 p-0 pl-3">
                                            <p id="count_byte_file_element_adjunto_doc_visor"></p>    
                                        </div>
                                    </div>
                                    
                                </div>
                                <div class="col-4 justify-content-end pt-2">
                                    <p id="count_file_element_adjunto_doc_visor" class="font-weight-light" style="float: right"> Estado </p>
                                </div>
                            </div>
                        </div>
                        <asp:Panel ID="Panel_descarga_ajax" runat="server">               
                            <div id="drop_zone_" style="width: 100%; height: auto; display: none">
                                <asp:UpdatePanel ID="UpdatePanel_descarga" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                         <asp:Button ID="Button_update_update_adjunto_doc_visor" runat="server" Text="Button" />
                                        <asp:AjaxFileUpload ID="AjaxFileUpload_dowload" runat="server" ThrobberID="drop_zone_"
                                            ContextKeys="fred"
                                            AllowedFileTypes="tif,jpg,tiff,bmp,pdf"
                                            MaximumNumberOfFiles="1" OnClientUploadComplete="activa_boton_dowload" />
                                        <asp:Button ID="Button_guardar_desicion" runat="server" Text="Button" Style="display: none" />
                                        &nbsp  
                                        <asp:Label ID="Label_estado_carga" runat="server" Text="Estado" Style="font-size: 10px" CssClass="font-weight-light h6"></asp:Label>
                                        <input id="Hidden_result_load" type="hidden" value="" runat="server" />
                                        <input id="Hidden_tip_adjunt" type="hidden" value="" runat="server" />
                                        <input id="Hidden_date_row" type="hidden" value="" runat="server" />
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </asp:Panel>
                        <div style="display: none; height: 1px">
                            <asp:Button ID="Button_sube_documento_adjunto" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                            <asp:Button ID="Button3_cerrar_adjunta" runat="Server" Text="" CssClass="invisible" />
                        </div>

                    </div>
                </div>
            </asp:Panel>     
        </div>
         
         <asp:Panel ID="Panel_indice_enlace" runat="server"  Style="display:none; color: White; width: 100%; height: 98%; margin:auto" >
            <asp:ModalPopupExtender ID="ModalPopupExtenderimpre_indice_enlace" runat="Server" BackgroundCssClass="ModalBackgroud_gorund" TargetControlID="ButtonSalir_indice_enlace"
                    PopupControlID="Panel_indice_enlace" CancelControlID="Buttoncerrarimpre_indice_enlace">
                </asp:ModalPopupExtender>
                <div id="divcabecer2__indice_enlace" class="cabecera2" style="background-color:transparent">
                    
                    <asp:Label ID="Label_indice_enlace" runat="server" Text="indice_enlace documento" Font-Size="10" Style="float: left; display:none">
                    </asp:Label>
                    <div id="Divcerrarbuton2_indice_enlace" style="float: right">
                        <asp:Button ID="Buttoncerrarimpre_indice_enlace" runat="Server" Text="X"
                            ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" style="display:none"/>
                    </div>
                </div>
              <asp:UpdatePanel ID="UpdatePanelindice_enlace" runat="server" UpdateMode="Conditional"
                  RenderMode="Inline">
                  <ContentTemplate>
                      <iframe id="ifrm_indice_enlace_" runat="server" style="border-style: none; left: 1px; width: 100%; height:97%; position: relative; top: 1px; background-color:transparent;  margin:auto"
                          frameborder="0" scrolling="no" ></iframe>
                          <input id="Hidden_result_indice_enlace" type="hidden" value="0" runat="server"/>
                  </ContentTemplate>
                  <Triggers>
                      
                  </Triggers>
              </asp:UpdatePanel>
              <asp:Button ID="Button1_indice_enlace" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
              <asp:Button ID="ButtonSalir_indice_enlace" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
             </asp:Panel>
            <!--evnviar actividad-->
            <asp:Panel ID="Panel_lista_actividades_ruta" runat="server" Style="display:none;  width: 80%; height:100%" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_lista_actividades_ruta"  runat="server"   
                    TargetControlID="ButtonSalir_lista_actividades_ruta" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_lista_actividades_ruta" PopupControlID="Panel_lista_actividades_ruta" ></asp:ModalPopupExtender>
                <div id="modal_content_lista_actividades_ruta" class="modal-content">
                    <div id="divcabecer2_lista_actividades_ruta" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title d-inline ">Enviar a grupo</h6>
                        <button type="button" value="Button_cerrar_lista_actividades_ruta" class="close da_event_captive ">&times;</button>
                    </div>
                    <div id="contenido_procesa_lista_actividades_ruta" style="background-color: white; width: 100%; height: 99%" class="p-1">
                        <asp:UpdatePanel ID="UpdateGeneral_lista_actividades_ruta" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <div id="contenido_titulo_data_grid_lista_actividades_ruta" style="width: auto; height: auto; border-bottom: 1px solid #e9ecef"  class="border_superior_radius_   row p-2 ml-1 mr-1">
                                    <div class="col-6 ">
                                         <asp:Label ID="titulo_label_lista_actividad_ruta" runat="server"  CssClass="h6 font-weight-light "  ></asp:Label>
                                    </div>
                                    <div class="col-6  text-right">
                                        <div class="input-group ">
                                            <button id="Button_restore_lista_actividades" class="btn btn-outline-secondary border-right-2 " title="Restaura lista" style="border-top-right-radius: 0px; border-bottom-right-radius: 0px" onclick="preven_event_restor_search_lista_actividad(event,this)" type="button">
                                                <i class="fal fa-long-arrow-left"></i>
                                            </button>
                                            <asp:TextBox ID="TextBox_buequeda_general_lista_actividades" runat="server" class="form-control form-control-sm complex  border-left-0" placeholder="Busqueda...." onkeypress="preven_event_search_keypres_enter_lista_actividad(event,this);"></asp:TextBox>
                                            <div class="input-group-append">
                                                <button class="btn btn-outline-secondary" title="Consultar" onclick="preven_event_search_lista_actividad(event,this)" type="button">
                                                    <i class="fal fa-search"></i>
                                                </button>
                                            </div>
                                        </div>
                                    </div>                      
                                </div>
                                <input id="Hidden_sel_actividad" type="hidden" value="-1" runat="server"/>
                                <div id="div_gred_actividades" style="  overflow: auto">
                                    <asp:GridView ID="GridView_envia_actividades" runat="server" Style="width:100%"
                                        AutoGenerateSelectButton="False" CssClass="filtrar table font-weight-light" GridLines="None" >
                                        <SelectedRowStyle BackColor="LightSkyBlue"  />
                                        <HeaderStyle CssClass="GridviewScrollHeader_line_boot" />
                                        <Columns>
                                            <asp:BoundField HeaderText="DETALLE" />
                                        </Columns>
                                    </asp:GridView>

                                </div>
                            </ContentTemplate>

                        </asp:UpdatePanel>
                        <div id="div_contenido_procesa_lista_actividades_ruta_botones_desicion" class=" modal-footer" >
                             
                        </div>
                    </div>
                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button_lista_actividades_ruta" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="ButtonSalir_lista_actividades_ruta" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="Button_cerrar_lista_actividades_ruta" runat="Server" Text="" CssClass="invisible" />
                        
                    </div>
                </div>
            </asp:Panel>
              <asp:UpdatePanel ID="UpdatePanel_lista_actividades_ruta" runat="server" UpdateMode="Conditional">
                  <ContentTemplate>
                      <asp:Button ID="Button_activa_enviar_actividad_ruta" runat="server" Text="" style="display:none"   />
                      <asp:Button ID="Button_detalle_enviar_actividad_ruta" runat="server" Text="" style="display:none"   />
                      <input id="Hidden_result_actividad_ruta" type="hidden" value="" runat="server"/>
                  </ContentTemplate>
              </asp:UpdatePanel>
            <!--enviar usuario-->
            <asp:Panel ID="Panel_lista_usuarios_ruta" runat="server" Style="display:none;  width: 80%; height:100%" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_lista_usuarios_ruta"  runat="server"   
                    TargetControlID="ButtonSalir_lista_usuarios_ruta" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_lista_usuarios_ruta" PopupControlID="Panel_lista_usuarios_ruta" ></asp:ModalPopupExtender>
                <div id="modal_content_lista_usuarios_ruta" class="modal-content">
                    <div id="divcabecer2_lista_usuarios_ruta" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title d-inline ">Enviar a usuario</h6>
                        <button type="button" value="Button_cerrar_lista_usuarios_ruta" class="close da_event_captive ">&times;</button>
                    </div>
                    <div id="contenido_procesa_lista_usuarios_ruta" style="background-color: white; width: 100%; height: 99%" class="p-1">
                        <asp:UpdatePanel ID="UpdateGeneral_lista_usuarios_ruta" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <div id="contenido_titulo_data_grid_lista_usuarios_ruta" style="width: auto; height: auto; border-bottom: 1px solid #e9ecef"  class="border_superior_radius_   row p-2 ml-1 mr-1">
                                    <div class="col-6 ">
                                         <asp:Label ID="titulo_label_lista_usuario_ruta" runat="server"  CssClass="h6 font-weight-light "  ></asp:Label>
                                    </div>
                                    <div class="col-6  text-right">
                                        <div class="input-group ">
                                            <button id="Button_restore_lista_usuarios" class="btn btn-outline-secondary border-right-2 " title="Restaura lista" style="border-top-right-radius: 0px; border-bottom-right-radius: 0px" onclick="preven_event_restor_search_lista_usuario(event,this)" type="button">
                                                <i class="fal fa-long-arrow-left"></i>
                                            </button>
                                            <asp:TextBox ID="TextBox_buequeda_general_lista_usuarios" runat="server" class="form-control form-control-sm complex  border-left-0" placeholder="Busqueda...." onkeypress="preven_event_search_keypres_enter_lista_usuario(event,this);"></asp:TextBox>
                                            <div class="input-group-append">
                                                <button class="btn btn-outline-secondary" title="Consultar" onclick="preven_event_search_lista_usuario(event,this)" type="button">
                                                    <i class="fal fa-search"></i>
                                                </button>
                                            </div>
                                        </div>
                                    </div>
                                     
                                </div>
                                <div id="div_gred_usuarios" style="  overflow: auto">
                                    <asp:GridView ID="GridView_envia_usuario" runat="server" Style="width:100%"
                                        AutoGenerateSelectButton="False" CssClass="filtrar table font-weight-light" GridLines="None" >
                                        <SelectedRowStyle BackColor="LightSkyBlue"  />
                                        <HeaderStyle CssClass="GridviewScrollHeader_line_boot" />
                                        <Columns>
                                            <asp:BoundField HeaderText="DETALLE" />
                                        </Columns>
                                    </asp:GridView>

                                </div>
                            </ContentTemplate>

                        </asp:UpdatePanel>
                        <div id="div_contenido_procesa_lista_usuarios_ruta_botones_desicion" class=" modal-footer" >
                          
                            
                        </div>
                    </div>
                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button_lista_usuarios_ruta" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="ButtonSalir_lista_usuarios_ruta" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="Button_cerrar_lista_usuarios_ruta" runat="Server" Text="" CssClass="invisible" />
                        
                    </div>
                </div>
            </asp:Panel>
            <!--lista_actividades_worflow_ruta-->
           <asp:Panel ID="Panel_lista_actividades_worflow_ruta" runat="server" Style="display:none;  width: 50%; height:50%" CssClass="modal_content_general ctw-legacy-modal">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_lista_actividades_worflow_ruta"  runat="server"   
                    TargetControlID="ButtonSalir_lista_actividades_worflow_ruta" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_lista_actividades_worflow_ruta" PopupControlID="Panel_lista_actividades_worflow_ruta" ></asp:ModalPopupExtender>
                <div id="modal_content_lista_actividades_worflow_ruta" class="modal-content ctw-modal-content">
                    <div id="divcabecer2_lista_actividades_worflow_ruta" class="modal_title_superior_ modal-header ctw-modal-header">
                        <h6 id="ctw-workflow-route-modal-title" class="modal-title ctw-modal-title d-inline ">Enviar tarea</h6>
                        <button type="button" value="Button_cerrar_lista_actividades_worflow_ruta" class="close da_event_captive btn btn-light btn-sm" aria-label="Cerrar">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div id="contenido_procesa_lista_actividades_workflow" style="background-color: white; width: 100%; height: 99%; border-bottom:none" class="modal-body ctw-modal-body p-1">
                        <asp:UpdatePanel ID="UpdateGeneral_documentos" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <div id="contenido_titulo_data_grid_dos_title" class="ctw-workflow-modal-context-anchor" aria-hidden="true"></div>
                                <input id="HiddenEstado" type="hidden" value="1" runat="server"/>
                                <div id="div_gred" style="  overflow: auto">
                                    <asp:GridView ID="GridView_envia_flujo" runat="server" Style="width:100%" EnableViewState="true"
                                        AutoGenerateSelectButton="False" CssClass="filtrar table font-weight-light gridview-moderno" GridLines="None" >
                                        <SelectedRowStyle BackColor="LightSkyBlue"  />
                                        <HeaderStyle CssClass="GridviewScrollHeader_line_boot" />
                                        <Columns>
                                            <asp:BoundField HeaderText="DETALLE" />
                                        </Columns>
                                    </asp:GridView>

                                </div>
                                <div id="div_contenido_procesa_lista_actividades_worflow_ruta_botones_desicion" class="modal-footer ctw-modal-footer ctw-workflow-modal-context" aria-label="Resumen del flujo">
                                    <div class="container-fluid p-0">
                                        <div class="row m-0 align-items-center">
                                            <div class="col-5 px-0 ctw-workflow-modal-context__column">
                                                <span class="ctw-workflow-modal-context__item">
                                                    <b>Registros:</b>
                                                    <asp:Label ID="titulo_label_grid" runat="server" CssClass="h6 font-weight-light">Resultados busqueda</asp:Label>
                                                </span>
                                            </div>
                                            <div class="col-7 px-0 text-right ctw-workflow-modal-context__column ctw-workflow-modal-context__column--flow">
                                                <span class="ctw-workflow-modal-context__item ctw-workflow-modal-context__item--flow">
                                                    <b>Flujo:</b>
                                                    <asp:Label ID="Label_nombre_flujo" runat="server" CssClass="h6 font-weight-light"></asp:Label>
                                                </span>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </ContentTemplate>

                        </asp:UpdatePanel>
                    </div>
                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button_lista_actividades_worflow_ruta" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="ButtonSalir_lista_actividades_worflow_ruta" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="Button_cerrar_lista_actividades_worflow_ruta" runat="Server" Text="" CssClass="invisible" />
                        
                    </div>
                </div>
            </asp:Panel>

              <asp:UpdatePanel ID="UpdatePanel_enviar_actividad" runat="server" UpdateMode="Conditional">
                  <ContentTemplate>
                            <asp:Button ID="Button_activa_enviar_actividad_flujo_trabajo_anterior" runat="server" Text="" style="display:none"   />
                            <asp:Button ID="Button_activa_enviar_actividad_flujo_trabajo" runat="server" Text="" style="display:none"   />
                            <asp:Button ID="Button_detalle_enviar_actividad_flujo_trabajo" runat="server" Text="" style="display:none"   />
                            <input id="Hidden_resultado_enviar_activdad_flujo" type="hidden" value="" runat="server"/>
                            <input id="Hidden_id_actividad_flujo" type="hidden" value="0" runat="server"/>
                            <input id="Hidden_id_flujo_trabjo" type="hidden" value="0" runat="server"/>
                            <input id="Hidden_id_actividad_destino" type="hidden" value="0" runat="server"/>
                            <input id="Hidden_id_usuario_workflow" type="hidden" value="0" runat="server"/>
                            <input id="Hidden_id_conector" type="hidden" value="0" runat="server"/>
                  </ContentTemplate>
              </asp:UpdatePanel>
       
            <!--envia_actividad_flujo_trabajo-->
          <div id="envia_actividad_flujo_trabjo">
            <asp:Panel ID="Panel_envia_actividad_flujo_trabjo" runat="server" Style="display:none; color: White; width:300px; height: 130px" >
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_envia_actividad_flujo_trabjo"  runat="server" BehaviorID="Panel_envia_actividad_flujo_trabjo" TargetControlID="ButtonSalir_envia_actividad_flujo_trabjo" BackgroundCssClass="FondoAplicacion" 
                    CancelControlID="Button_cerrar_envia_actividad_flujo_trabjo" PopupControlID="Panel_envia_actividad_flujo_trabjo"   ></asp:ModalPopupExtender>
                <div id="divcabecer2_envia_actividad_flujo_trabjo"  class="cabecera2">               
                    <asp:Label ID="Label_envia_actividad_flujo_trabjo" runat="server" Text="Mensaje" Font-Size="10" Style="float: left; font-family:Arial; margin-left:10px">
                    </asp:Label>
                    <div id="Divcerrarbuton2_envia_actividad_flujo_trabjo" style="float: right">
                        <asp:Button ID="Button_cerrar_envia_actividad_flujo_trabjo" runat="Server" Text="X"
                            ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_envia_actividad_flujo_trabjo" style="background-color: white; width: 100%; height: 99%;border: thin double #000080; color: black; background-color: #FFFFFF;">                  
                        <asp:UpdatePanel ID="UpdatePanel_envia_actividad_flujo_trabjo" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <input id="Hidden_estado_eliminar" type="hidden" value="" runat="server"/>
                                <div style="text-align: center">
                                    <br />
                                    <asp:Label ID="Label_title_comfirma_eliminar" runat="server" Text="Desea enviar la tarea a la actividad selecionada ?" style="font-family:Arial; font-size:14px"></asp:Label>
                                    <br />
                                    <br />
                                    <asp:Button ID="Button_aceptar_confirmacion" runat="server" Text="Aceptar" CssClass="boton_azul" /> &nbsp
                                    <asp:Button ID="Button_cancelar_confirmacion" runat="server" Text="Cancelar" CssClass="boton_azul" />
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                         
                </div>
                 <asp:Button ID="Button_envia_actividad_flujo_trabjo" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_envia_actividad_flujo_trabjo" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
            </asp:Panel>
        </div>

          <!--detalle_actividad_flujo_user-->   
            <asp:Panel ID="Panel_detalle_actividad_flujo_user" runat="server" Style="display:none; width:50%; height:auto" >
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_detalle_actividad_flujo_user"  runat="server"  TargetControlID="ButtonSalir_detalle_actividad_flujo_user" BackgroundCssClass="FondoAplicacion" 
                    CancelControlID="Button_cerrar_detalle_actividad_flujo_user" PopupControlID="Panel_detalle_actividad_flujo_user"   ></asp:ModalPopupExtender>
                <div id="modal_content_detalle_actividad_flujo_user" class="modal-content">
                    <div id="divcabecer2_detalle_actividad_flujo_user" class="modal-header">
                        <h6 class="modal-title d-inline">Info</h6>
                        <button type="button" value="Button_cerrar_detalle_actividad_flujo_user" class="close da_event_captive">&times;</button> 
                    </div>
                    <div id="contenido_procesa_detalle_actividad_flujo_user" style="overflow:auto " class="pl-1 pr-1 table">
                        <asp:UpdatePanel ID="UpdatePanel_detalle_actividad_flujo_user" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div style="">
                                    <table style="width:100%">
                                        <tr>
                                            <td style="background-color: #E7EDF5">
                                                <span class="h6">Nombre usuario</span>
                                            </td>
                                            <td >
                                                <asp:Label ID="Label_nombre_usuario" runat="server" Text="" CssClass="h6 font-weight-light"></asp:Label>
                                            </td>

                                        </tr>
                                        <tr>
                                            <td style="background-color: #E7EDF5">
                                                <span class="h6">Cargo usuario</span>         
                                            </td>
                                            <td >
                                                <asp:Label ID="Label_cargo" runat="server" Text="" CssClass="h6 font-weight-light"></asp:Label>
                                            </td>

                                        </tr>
                                        <tr>
                                            <td style="background-color: #E7EDF5">
                                                <span class="h6">Correo electrónico</spa>
                                            </td>
                                            <td>
                                                <asp:Label ID="Label_correo" runat="server" Text="" CssClass="h6 font-weight-light"></asp:Label>
                                            </td>

                                        </tr>
                                        <tr>
                                            <td style="background-color: #E7EDF5">
                                                <span class="h6">Grupo usuario</span>
                                            </td>
                                            <td >
                                                <asp:Label ID="Label_nombre_grupo" runat="server" Text="" CssClass="h6 font-weight-light"></asp:Label>
                                            </td>

                                        </tr>
                                    </table>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                    </div>
                    <div style="display:none; height:1px">
                    <asp:Button ID="Button_detalle_actividad_flujo_user" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="ButtonSalir_detalle_actividad_flujo_user" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="Button_cerrar_detalle_actividad_flujo_user" CssClass="invisible" runat="Server" Text=""/>
                    </div>
                </div>
            </asp:Panel>

          <!--detalle_actividad_flujo-->     
            <asp:Panel ID="Panel_detalle_actividad_flujo" runat="server" Style="display:none;  width:50%; height:50%" >
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_detalle_actividad_flujo"  runat="server" Y="1"  TargetControlID="ButtonSalir_detalle_actividad_flujo" 
                     BackgroundCssClass="FondoAplicacion" 
                    CancelControlID="Button_cerrar_detalle_actividad_flujo" PopupControlID="Panel_detalle_actividad_flujo"   ></asp:ModalPopupExtender>
                <div id="modal_content_actividad_flujo" class="modal-content">
                    <div id="divcabecer2_detalle_actividad_flujo" class="modal-header">
                         <h6 class="modal-title d-inline ">Info</h6>
                         <button type="button" value="Button_cerrar_detalle_actividad_flujo" class="close da_event_captive">&times;</button>   
                    </div>
                    <div id="contenido_procesa_detalle_actividad_flujo" style="overflow:auto " class="pl-1 pr-1">
                        <asp:UpdatePanel ID="UpdatePanel_detalle_actividad_flujo" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div style="">
                                    <table style="" class="table ">
                                        <tr>
                                            <td style="background-color: #E7EDF5">
                                                <span class="h6"> Nombre actividad </span>
                                               
                                            </td>
                                            <td style="">
                                                <asp:Label ID="Label_nombre_actividad" runat="server" Text=""  CssClass="h6 font-weight-light"></asp:Label>
                                            </td>

                                        </tr>
                                        <tr>
                                            <td style="background-color: #E7EDF5">
                                                <span class="h6"> Descripción actividad </span>
                                                
                                            </td>
                                            <td style="">
                                                <asp:Label ID="Label_descripcion" runat="server" Text="" CssClass="h6 font-weight-light"></asp:Label>
                                            </td>

                                        </tr>
                                        <tr>
                                            <td style="background-color: #E7EDF5">
                                                <span class="h6">Tipo actividad</span>
                                                
                                            </td>
                                            <td style="">
                                                <asp:Label ID="Label_tipo_actividad" runat="server" Text="" CssClass="h6 font-weight-light"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="background-color: #E7EDF5">
                                                <span class="h6">
                                                    Usuarios relacionados
                                                </span>
                                                
                                            </td>
                                            <td style="overflow: auto; max-height:200px;">
                                                <div style="overflow: auto; max-height:200px">
                                                    <asp:Label ID="Label_usuario_relacionados" runat="server" Text="" CssClass="h6 font-weight-light"></asp:Label>
                                                </div>


                                            </td>

                                        </tr>

                                    </table>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                    </div>
                </div>
                <div style="display:none; height:1px">
                    <asp:Button ID="Button_detalle_actividad_flujo" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="ButtonSalir_detalle_actividad_flujo" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="Button_cerrar_detalle_actividad_flujo" runat="Server" Text="" CssClass="invisible"/>
                 </div>
            </asp:Panel>
          <!--autoriza reasignacion flujo-->
          <div id="lista_actividades_ruta_flujo">
            <asp:Panel ID="Panel_lista_actividades_ruta_flujo" runat="server" Style="display:none; color: White; width: 600px; height: 200px">

                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_lista_actividades_ruta_flujo" runat="server" BehaviorID="Panel_lista_actividades_ruta_flujo" TargetControlID="ButtonSalir_lista_actividades_ruta_flujo" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_lista_actividades_ruta_flujo" PopupControlID="Panel_lista_actividades_ruta_flujo" ></asp:ModalPopupExtender>
                <div id="divcabecer2_lista_actividades_ruta_flujo" class="cabecera2">
                    <asp:Button ID="Button_lista_actividades_ruta_flujo" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_lista_actividades_ruta_flujo" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Label ID="Label_lista_actividades_ruta_flujo" runat="server" Text="Autoriza reasignación" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton2_lista_actividades_ruta_flujo" style="float: right">
                        <asp:Button ID="Button_cerrar_lista_actividades_ruta_flujo" runat="Server" Text="X"
                            ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_lista_actividades_ruta_flujo" style="background-color: white; width: 100%; height: 99%;border: thin double #000080; color: black; background-color: #FFFFFF;">
                                
                    
                        <asp:UpdatePanel ID="UpdatePanel_lista_actividades_ruta_flujo" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                               <br />
                                <table style="width: 100%;">
                                   
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label_user_lista_actividades_ruta_flujo" runat="server" Text="Usuario autorizado*" Style="text-align: center; font-family: Arial; font-size: 14px"></asp:Label>
                                        </td>
                                        <td><asp:TextBox ID="TextBox_login_lista_actividades_ruta_flujo" runat="server" Style="width:300px"></asp:TextBox></td>
                                       
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label_dest_lista_actividades_ruta_flujo" runat="server" Text="Contraseña usuario*" Style="text-align: center; font-family: Arial; font-size: 14px"></asp:Label>

                                        </td>
                                        <td>
                                            <asp:TextBox ID="TextBox_pasw_lista_actividades_ruta_flujo" runat="server" Style="width:300px"  TextMode="Password"></asp:TextBox> 
                                           

                                        </td>                           
                                    </tr>
                                    <tr>
                                        <td></td>
                                    </tr>
                                    
                                    <tr>
                                        <td>

                                        </td>
                                        <td style="float:left"><asp:Button ID="Button_autoriza_reasignacion_flujo" runat="server" Text="Reasignar" Style="background-color: white; border-color: #b0c4de; height: 30px; width: 200px; height: 25px; text-align: center" CssClass="boton" /> &nbsp &nbsp
                                                     <input id="Hidden_resp_envio_flujo" type="hidden" value="" runat="server"/>    
                                        </td>
                                    </tr>
                                    
                                    
                                </table>
                                                         
                            </ContentTemplate>
                        </asp:UpdatePanel>
                         
                </div>
            </asp:Panel>
        </div>
          <!--autoriza reasignacion tarea recuperada-->
          <div id="autoriza_reasignacion_tarea_recuperada">
            <asp:Panel ID="Panel_autoriza_reasignacion_tarea_recuperada" runat="server" Style="display:none; width: auto; height: auto">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_autoriza_reasignacion_tarea_recuperada" runat="server"  TargetControlID="ButtonSalir_autoriza_reasignacion_tarea_recuperada" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_autoriza_reasignacion_tarea_recuperada" PopupControlID="Panel_autoriza_reasignacion_tarea_recuperada" ></asp:ModalPopupExtender>
                <div id="modal_content_autoriza_reasignacion_tarea_recuperada" class="modal-content">
                    <div id="divcabecer2_autoriza_reasignacion_tarea_recuperada" class="modal-header">
                          <h6 class="modal-title d-inline ml-1">Autoriza reasignación</h6>
                          <button type="button" value="Button_cerrar_autoriza_reasignacion_tarea_recuperada" class="close da_event_captive">&times;</button>   
                    </div>
                    <div id="contenido_procesa_autoriza_reasignacion_tarea_recuperada" style="background-color: white; width: 100%; height: 99%" class="modal_content_back modal-body">
                        <div class="row mb-2">
                            <div class="col-4">
                                <span>Usuario*</span>
                            </div>
                             <div class="col-8">
                                 <asp:TextBox ID="TextBox_login_autoriza_reasignacion_tarea_recuperada" runat="server" CssClass="form-control" ></asp:TextBox>
                            </div>
                        </div>
                         <div class="row">
                            <div class="col-4">
                                <span>Contraseña*</span>
                            </div>
                             <div class="col-8">
                                 <asp:TextBox ID="TextBox_pasw_autoriza_reasignacion_tarea_recuperada" runat="server" CssClass="form-control"  TextMode="Password"></asp:TextBox>
                            </div>
                        </div>
                      
                    </div>
                    <div class="modal-footer">
                        <asp:UpdatePanel ID="UpdatePanel_autoriza_reasignacion_tarea_recuperada" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                    <asp:Button ID="Button_aceptar_reasignacion_tarea_recuperada" runat="server" Text="Aceptar"  CssClass="btn btn-success" />
                                    <input id="Hidden_respreasignacion_tarea_recuperada" type="hidden" value="" runat="server"/>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div style="display:none; height:1px">
                        <asp:Button ID="ButtonSalir_autoriza_reasignacion_tarea_recuperada" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                         <asp:Button ID="Button_cerrar_autoriza_reasignacion_tarea_recuperada" runat="Server" Text="X"
                                  />
                    </div>
                    
                </div>
            </asp:Panel>
        </div>
        
          <!--autoriza reasignacion tarea recuperada enlazada-->
          <div id="autoriza_reasignacion_tarea_recuperada_enlazada">
            <asp:Panel ID="Panel_autoriza_reasignacion_tarea_recuperada_enlazada" runat="server" Style="display:none; width:auto; height: auto">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_autoriza_reasignacion_tarea_recuperada_enlazada" runat="server"  TargetControlID="ButtonSalir_autoriza_reasignacion_tarea_recuperada_enlazada" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_autoriza_reasignacion_tarea_recuperada_enlazada" PopupControlID="Panel_autoriza_reasignacion_tarea_recuperada_enlazada" ></asp:ModalPopupExtender>
                <div id="modal_content_autoriza_reasignacion_tarea_recuperada_enlazada" class="modal-content">
                    <div id="divcabecer2_autoriza_reasignacion_tarea_recuperada_enlazada" class="cabecera2_ modal-header">   
                          <h6 class="modal-title d-inline ml-1">Autoriza reasignación</h6>
                          <button type="button" value="Button_cerrar_autoriza_reasignacion_tarea_recuperada_enlazada" class="close da_event_captive">&times;</button>              
                    </div>
                    <div id="contenido_procesa_autoriza_reasignacion_tarea_recuperada_enlazada" style=" width: 100%; height: 99%" class="modal_content_back modal-body">
                        <div class="row mb-2">
                            <div class="col-4">
                                <span>Usuario * </span>
                            </div>
                            <div class="col-8">
                                <asp:TextBox ID="TextBox_login_autoriza_reasignacion_tarea_recuperada_enlazada" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                         <div class="row">
                             <div class="col-4">
                                 <span>
                                     Contraseña
                                 </span>
                             </div>
                             <div class="col-8">
                                 <asp:TextBox ID="TextBox_pasw_autoriza_reasignacion_tarea_recuperada_enlazada" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
                             </div>
                        </div>
                       
                    </div>
                    <div class="modal-footer">
                        <asp:UpdatePanel ID="UpdatePanel_autoriza_reasignacion_tarea_recuperada_enlazada" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Button ID="Button_aceptar_reasignacion_tarea_recuperada_enlazada" runat="server" Text="Reasignar"  CssClass="btn btn-success" />        
                                <input id="Hidden_resp_reasignacion_tarea_recuperada_enlazada" type="hidden" value="" runat="server"/>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
                <div style="display:none; height:1px">
                     <asp:Button ID="ButtonSalir_autoriza_reasignacion_tarea_recuperada_enlazada" CssClass="invisible" runat="server" Text="Button" Height="0px" Width="0px" />
                     <asp:Button ID="Button_cerrar_autoriza_reasignacion_tarea_recuperada_enlazada" runat="Server" Text="X"
                                 Height="0px"  />
                </div>
            </asp:Panel>
        </div>
           <!--digitaliza documento adjunto-->
          <div id="digitaliza_documento_adjunto"> 
            <asp:Panel ID="Panel_digitaliza_documento_adjunto" runat="server" Style="display:none; width: 100%; height:100%" CssClass="modal_content_general_">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_digitaliza_documento_adjunto" runat="server" BehaviorID="Panel_digitaliza_documento_adjunto" TargetControlID="ButtonSalir_digitaliza_documento_adjunto" BackgroundCssClass="ModalBackgroud_gorund"
                    CancelControlID="Button_cerrar_digitaliza_documento_adjunto" PopupControlID="Panel_digitaliza_documento_adjunto" ></asp:ModalPopupExtender>
                <div id="modal_content_digitaliza_documento_adjunto" class="modal-content">  
                <div id="divcabecer2_digitaliza_documento_adjunto"  class="modal_title_superior_ modal-header">  
                     <h6 class="modal-title d-inline ml-1">Digitalización</h6>
                    <button type="button" value="Button_cerrar_digitaliza_documento_adjunto" class="close da_event_captive">&times;</button>
                </div>
                    <div id="contenido_procesa_digitaliza_documento_adjunto" style=" width: 100%; height: 100%" class="modal_content_back">
                        <asp:UpdatePanel ID="UpdatePanel_iframe_digitaliza_adjunto" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <iframe id="IframeDitaliza_adjunto_" runat="server" frameborder="0" width="100%" scrolling="no" height="100%"></iframe>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div style="display:none; height:1px">
                        <asp:Button ID="Button_digitaliza_documento_adjunto" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="ButtonSalir_digitaliza_documento_adjunto" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="Button_cerrar_digitaliza_documento_adjunto" runat="Server" Text="" CssClass="invisible"/>
                    </div>
                 
                </div>
            </asp:Panel>
        </div>  
          <!--Notifica gestión-->
          <div id="notifica_gestion">
            <asp:Panel ID="Panel_notifica_gestion" runat="server" Style="display:none;  width: 70%; height: 100%" CssClass="modal_content_general_">
                <asp:ModalPopupExtender ID="ModalPopupExtender_notifica_gestion" runat="server" BehaviorID="Panel_notifica_gestion_ModalPopupExtender" TargetControlID="ButtonSalir_notifica_gestion"
                    CancelControlID="Button_cerrar_notifica_gestion" PopupControlID="Panel_notifica_gestion" BackgroundCssClass="FondoAplicacion" >
                </asp:ModalPopupExtender>
                <div id="modal_content_notifica_gestion" class="modal-content"> 
                <div id="divcabecer2_notifica_gestion" class="modal_title_superior_ modal-header">   
                    <h6 class="modal-title d-inline ml-1">Envío de correo electrónico</h6>
                    <button type="button" value="Button_cerrar_notifica_gestion" class="close da_event_captive">&times;</button>      
                </div>
                <div id="contenido_procesa_notifica_gestion" style=" width:100%; height:auto ; border-top:none" class="modal_content_back">
                    <asp:UpdatePanel ID="UpdatePanel_iframenotifica" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <iframe  style="color: White; width: 100%; background-color:white; height:auto; overflow:auto" id="Iframe_comparte_coreo" runat="server" frameborder="0"  ></iframe>
                             <input id="Hidden_cuenta_correo_envio" type="hidden" value="" runat="server"/>
                             <input id="Hidden_correo_envio_default" type="hidden" value="" runat="server"/>
                             <input id="Hidden_imagen_adjunta" type="hidden" value="" runat="server"/>
                             <input id="Hidden_asunto_notificacion" type="hidden" value="" runat="server"/>
                             <input id="Hidden_convierte_pdf" type="hidden" value="" runat="server"/>
                            <input id="Hidden_tipo_notificacion" type="hidden" value="ENVIO CORREO WORKFLOW" runat="server"/>
                            <input id="Hidden_ruta_tempo" type="hidden" value="" runat="server"/>
                             <input id="Hidden_id_plantilla_radicado" type="hidden" value="" runat="server"/>
                            <input id="hdnEmailID_VAL" type="hidden" value="" runat="server"/>
                            
                        </ContentTemplate>
                    </asp:UpdatePanel>
                   
                </div>
                    <div style="display:none; height:1px">
                    <asp:Button ID="Button7" CssClass="invisible" runat="server" Text="Button" Height="0px" Width="0px" />
                    <asp:Button ID="ButtonSalir_notifica_gestion" CssClass="invisible" runat="server" Text="Button" Height="0px" Width="0px" />
                    <asp:Button ID="Button_cerrar_notifica_gestion" runat="Server" Text="" CssClass="invisible" />
                    </div>
                 </div>
            </asp:Panel>
        </div>
          <!--Adjunta documento automatico!-->      
            <asp:Panel ID="Panel_adjunta_autamatico_documento" runat="server" Style="display:none;  height:auto; width:60%" CssClass="modal_content_general_">              
                <asp:ModalPopupExtender ID="ModalPopupExtender_adjunta_autamatico_documento" runat="Server" BackgroundCssClass="FondoAplicacion" 
                     TargetControlID="ButtonSalir_adjunta_autamatico_documento"
                    PopupControlID="Panel_adjunta_autamatico_documento" CancelControlID="Buttoncerrarimpre_adjunta_autamatico_documento">
                </asp:ModalPopupExtender>    
                 <div id="modal_content_adjunta_autamatico_documento" class="modal-content">        
                    <div id="Divcerrarbuton2_adjunta_autamatico_title"  class="modal_title_superior_ modal-header" >
                        <h6 class="modal-title d-inline ml-1">Adjuntar documento desde servicio web</h6>  
                        <button type="button" value="Buttoncerrarimpre_adjunta_autamatico_documento" class="close da_event_captive ">&times;</button>                  
                    </div>                   
                        <div id="Contenido_adjunta_autamatico_documento" style=" height:auto; width: 100%; border-top: none" class="modal_content_back p-2">
                            <asp:UpdatePanel ID="UpdatePanel_chek_adjunta_documento_automatico" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:MutuallyExclusiveCheckBoxExtender ID="MutuallyExclusiveCheckBoxExtender1" runat="server" TargetControlID="Check_anexo_radicado_automatico"
                                        Key="radicado"></asp:MutuallyExclusiveCheckBoxExtender>
                                    <asp:MutuallyExclusiveCheckBoxExtender ID="Mutuallyexclusivecheckboxextender2" runat="server" TargetControlID="CheckBox_relacionado_radicado_automatico"
                                        Key="radicado"></asp:MutuallyExclusiveCheckBoxExtender>
                                    <div class=" row pl-1 pr-1">
                                        <div class="col-6">
                                            <asp:CheckBox ID="Check_anexo_radicado_automatico" runat="server" Text="Guardar como parte del documento" Checked="false" Font-Size="11" Enabled="true" CssClass="h6 font-weight-light" />
                                        </div>
                                        <div class="col-6">
                                            <asp:CheckBox ID="CheckBox_relacionado_radicado_automatico" runat="server" Text="Guardar como documento relacionado " Checked="true" Font-Size="11" Enabled="true" CssClass="h6 font-weight-light" />
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <div id="content_data_grid_adjunta_documento_automatico" class="conten_gred_border_" style="width: 100%">
                                <asp:UpdatePanel ID="UpdatePanel_actualiza_adjunta_documento_automatico" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:DropDownList ID="DropDownList_adjunta_documento_automatico" Style="width: 100%" CssClass="custom-select mr-sm-2" runat="server"></asp:DropDownList>
                                    </ContentTemplate>
                                </asp:UpdatePanel>

                            </div>
                        </div>
                     <div class="modal-footer">
                         <asp:UpdatePanel ID="UpdatePane_adjunta_autamatico_documento" runat="server" UpdateMode="Conditional">
                             <ContentTemplate>
                                  <asp:Button ID="Button_guardar_automatico" runat="server" Text="Guardar"  CssClass="btn btn-success"  />
                                 <asp:HiddenField ID="HiddenField_estado_guarda_automatico" runat="server" Value="" />
                                 <input id="Hidden_tip_adjunt_auto" type="hidden" value="" runat="server"/>
                                 <input id="Hidden_date_row_auto" type="hidden" value="" runat="server"/>
                             </ContentTemplate>
                         </asp:UpdatePanel>
                     </div>
                   
                     <div style="display: none; height: 1px">
                         <asp:Button ID="Button1__adjunta_autamatico_documento" CssClass="invisible" runat="server" Text="Button" Height="0px" Width="0px" />
                         <asp:Button ID="ButtonSalir_adjunta_autamatico_documento" CssClass="invisible" runat="server" Text="Button" Height="0px" Width="0px" />
                         <asp:Button ID="Buttoncerrarimpre_adjunta_autamatico_documento" runat="Server" Text="X" CssClass="modal_boton_hiden" />
                     </div>
                
                 </div>   
            </asp:Panel>
          <!--detalle transacciones-->
           <asp:Panel ID="Panel_transacciones" runat="server" Style="display:none; overflow:hidden; width:70%; height:100%" CssClass="modal_content_general_" >
                  <asp:ModalPopupExtender ID="ModalPopupExtender_transacciones" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_transacciones_dos"
                      PopupControlID="Panel_transacciones"  CancelControlID="ButtonSalir_transacciones">
                  </asp:ModalPopupExtender>
               <div id="modal_content_transacciones" class="modal-content">  
                  <div id="Cabecerapendiente_transacciones" class="modal_title_superior_ modal-header" > 
                        <h6 class="modal-title d-inline ml-1">Detalle de transacciones</h6>
                       <button type="button" value="ButtonSalir_transacciones" class="close da_event_captive">&times;</button>                       
                  </div>
                  <div id="Cotenedorpendiente_transacciones" style="height: 90%; width: 100%; overflow:hidden" class="modal_content_back">     
                      <asp:UpdatePanel ID="UpdatePanel_transacciones" runat="server" UpdateMode="Conditional">
                          <ContentTemplate>
                              <iframe id="Iframe_transacciones_" runat="server" frameborder="0"  style="width:100%; height:100%; overflow:hidden"></iframe>
                          </ContentTemplate>

                      </asp:UpdatePanel>
                           
                  </div>
                    <div style="display:none; height:1px">
                        <asp:Button ID="Button_transacciones_dos" CssClass="invisible" runat="server" Text="Button" Height="0px" Width="0px" />
                        <asp:Button ID="ButtonSalir_transacciones" runat="Server" Text="" ToolTip="" CssClass="invisible" />
                    </div>
                  </div> 
              </asp:Panel>
          <input id="Hidden_seccion" type="hidden" value="" runat="server"/>
                
           <div id="Div_hierlink" style="position: fixed; text-align: center; display: none; width: 0px; width: 0px">
              <asp:UpdatePanel ID="UpdatePanel_general_variable" runat="server" UpdateMode="Conditional">
                  <ContentTemplate>
                       <input id="Hidden_id_tarea_sel" type="hidden" value="-1" runat="server"/>   
                      <input id="Hidden_id_tarea_selecionada" type="hidden" value="0" runat="server"/>
                      <input id="Hidden_00020_4001" type="hidden" value="0" runat="server"/>
                      <input id="Hidden_radic_select" type="hidden" value="-1" runat="server"/>
                      <asp:Button ID="Button_visor_emergente" runat="server" Text="Button" style="display:none" />
                  </ContentTemplate>
              </asp:UpdatePanel>
               <input id="Hidden_tipo_contenido" type="hidden" value="RESPUESTA" runat="server"/>
               <input id="Hidden_radicado_seleccion" type="hidden" value="-1" runat="server"/>
               <input id="Hidden_tipo_visor" type="hidden" value="" runat="server"/>
               <input id="Hidden_filtro_gred" type="hidden" value="" runat="server"/>
          </div>
          
         <div id="confirma_autoriza_tarea">
            <asp:Panel ID="Panel_confirma_autoriza_tarea" runat="server" Style="display:none; color: White; width:30%; height:auto" CssClass="modal_content_general" >
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_confirma_autoriza_tarea" runat="server" BehaviorID="Panel_confirma_autoriza_tarea_ModalPopupExtender" TargetControlID="ButtonSalir_confirma_autoriza_tarea" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_confirma_autoriza_tarea" PopupControlID="Panel_confirma_autoriza_tarea" ></asp:ModalPopupExtender>
                <div id="divcabecer2_radica_documento_" class="modal_title_superior">              
                    
                   
                    <div id="Divcerrarbuton2_confirma_autoriza_tarea" style="float: right">
                        <asp:Button ID="Button_cerrar_confirma_autoriza_tarea" runat="Server" Text="X" CssClass="modal_boton_hiden"
                             ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_confirma_autoriza_tarea" style="background-color: white; width: 100%; height: 99%" class="modal_content_back">         
                        <asp:UpdatePanel ID="UpdatePanel_autoriza_tarea" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <br />
                                <br />
                                <div class="emergente_barra" style="padding-top:20px; padding-bottom:20px">
                                    <a id="a1"   style="margin-left:1px; font-family: Arial; margin-left:1px; margin-top:10px; width:100%; font-size:14px;  ">Desea autorizar la tarea actual ? </a> 
                                </div>          
                                <br />
                                <br />
                                <asp:Button ID="Button_autoriza_tarea" runat="server" Text="Aceptar" Style="float:right; margin-right:5px; margin-top:10px; margin-bottom:10px" CssClass="boton_azul" />                         
                            </ContentTemplate>
                        </asp:UpdatePanel>
                          <asp:Button ID="Button_confirma_autoriza_tarea" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                          <asp:Button ID="ButtonSalir_confirma_autoriza_tarea" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                </div>
            </asp:Panel>
           
        </div>
          <div id="anula_autoriza_tarea">
            <asp:Panel ID="Panel_anula_autoriza_tarea" runat="server" Style="display:none; color: White; width:30%; height:auto" CssClass="modal_content_general" >
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_anula_autoriza_tarea" runat="server" BehaviorID="Panel_anula_autoriza_tarea_ModalPopupExtender" TargetControlID="ButtonSalir_anula_autoriza_tarea" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_anula_autoriza_tarea" PopupControlID="Panel_anula_autoriza_tarea" ></asp:ModalPopupExtender>
                <div id="div4" class="modal_title_superior">              
                    
                   
                    <div id="Divcerrarbuton2_anula_autoriza_tarea" style="float: right">
                        <asp:Button ID="Button_cerrar_anula_autoriza_tarea" runat="Server" Text="X" CssClass="modal_boton_hiden"
                             ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_anula_autoriza_tarea" style="background-color: white; width: 100%; height: 99%" class="modal_content_back">         
                        <asp:UpdatePanel ID="UpdatePanel_desautoriza" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <br />
                                <br />
                                <div class="emergente_barra" style="padding-top:20px; padding-bottom:20px">
                                    <a id="a4"   style="margin-left:1px; font-family: Arial; margin-left:1px; margin-top:10px; width:100%; font-size:14px;  ">Desea desautorizar la tarea actual ? </a> 
                                </div>          
                                <br />
                                <br />
                                <asp:Button ID="Button_anula_autorizacion_tarea" runat="server" Text="Aceptar" Style="float:right; margin-right:5px; margin-top:10px; margin-bottom:10px" CssClass="boton_azul" />                         
                            </ContentTemplate>
                        </asp:UpdatePanel>
                          <asp:Button ID="Button_anula_autoriza_tarea" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                          <asp:Button ID="ButtonSalir_anula_autoriza_tarea" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                </div>
            </asp:Panel>
           
        </div>
          <div id="lista_autorizacion">
            <asp:Panel ID="Panel_lista_autorizacion" runat="server" Style="display:none; color: White; width:90%; height:auto" CssClass="modal_content_general_" >
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_lista_autorizacion" runat="server" BehaviorID="Panel_lista_autorizacion_ModalPopupExtender" TargetControlID="ButtonSalir_lista_autorizacion" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_lista_autorizacion" PopupControlID="Panel_lista_autorizacion" ></asp:ModalPopupExtender>
                <div id="modal_content_lista_autorizacion" class="modal-content">
                    <div id="divcabecer_lista_autorizacion" class="modal_title_superior_ modal-header">
                         <h6 class="modal-title d-inline ml-1">Autorizantes</h6>
                         <button type="button" value="Button_cerrar_lista_autorizacion" class="close da_event_captive">&times;</button>                        
                    </div>
                    <div id="contenido_procesa_lista_autorizacion" style="background-color: white; width: 100%; height: 99%" class="modal_content_back">
                       
                        <asp:UpdatePanel ID="UpdatePanel_contenido_lista_autorizacion" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div id="div_label_lista_autorizacion" class="p-2">
                                    <asp:Label ID="Label_title_listado_autorizaciones" runat="server" Text="" CssClass="h6 font-weight-light"></asp:Label>
                                </div>
                                <asp:Panel ID="Panel_lista_autorizacion_2" runat="server" ScrollBars="Auto"
                                    Width="100%" Style="min-height: 250px">
                                    <asp:GridView ID="data_grid_listado_solicitudes" runat="server" AllowSorting="true" AllowPaging="true" EnableViewState="true"
                                        PageSize="7" PagerSettings-Position="Top"  Style="width: 100%"
                                        AutoGenerateSelectButton="False" CssClass="filtrar table font-weight-light" GridLines="None" >
                                        <SelectedRowStyle  BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                        <HeaderStyle CssClass="GridviewScrollHeader_line_boot" BorderStyle="None" />
                                        <PagerStyle CssClass="pagination-ys" />
                                        <Columns>
                                            <asp:BoundField HeaderText="OPCIONES   " />
                                        </Columns>

                                    </asp:GridView>
                                </asp:Panel>

                            </ContentTemplate>
                        </asp:UpdatePanel>
                        

                    </div>
                     <div class="modal-footer justify-content-end" id="conter_boton_footer_lista_autorizacion">
                          <asp:UpdatePanel ID="updatemenu_lista_autorizacion" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Button ID="Button_descarga_consolidado_aprobacion" CssClass="btn btn-success" runat="server" ToolTip="Descarga consolidado aprobaciones" Text="Descargar" />
                                   
                                </ContentTemplate>
                            </asp:UpdatePanel>
                    </div>
                    <div style="display:none; height:1px">
                        <asp:Button ID="Button_lista_autorizacion" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="ButtonSalir_lista_autorizacion" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="Button_cerrar_lista_autorizacion" runat="Server" Text="" CssClass="invisible" />
                    </div>
                </div>
            </asp:Panel>
              <asp:UpdatePanel ID="UpdatePanel_boton_lista" runat="server" UpdateMode="Conditional">
                  <ContentTemplate>
                      <input id="Hidden_selec_list" type="hidden" value="-1" runat="server"/>
                      <asp:Button ID="Button_dowload_xml" runat="server" Text="Button" Style="display: none" />

                  </ContentTemplate>

              </asp:UpdatePanel>
                    <div id="descaga_xml_autoriza" style="width: 0%; height: 0%; background-color: #E7EDF5; display: none">
                        <asp:UpdatePanel ID="updatapanel_iframe_xml_autoriza" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <iframe runat="server" id="ifmExcel_xml_autoriza" width="0" height="0" marginheight="0" marginwidth="0"
                                    frameborder="0" />
                                <asp:Button ID="Button_export_lista_event_xml_autoriza" runat="server" Text="Exportar" Style="margin-top: 5px; display: none" CssClass="boton_azul" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>  
                </div>
       
         <div id="guardar_post">
            <asp:Panel ID="Panel_guardar" runat="server"  Style="display:none; width:80%; height: auto" CssClass="modal_content_general_">
                
                <asp:ModalPopupExtender ID="ModalPopupExtender_guardar" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_post_guardar"
                    PopupControlID="Panel_guardar" CancelControlID="Buttoncerrarimpre_post_guardar">
                </asp:ModalPopupExtender>
                <div id="modal_content_Panel_guardar" class="modal-content">
                    <div id="divcabecer2_post_guardar" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title d-inline ml-1">Guardar documento</h6>
                        <button type="button" value="Buttoncerrarimpre_post_guardar" class="close da_event_captive">&times;</button>   
                    </div>
                    <div id="Content_guardar_documento" style=" width: 100% ; border-top:none; overflow:auto" class="modal_content_back">
                        <asp:UpdatePanel ID="UpdatePane_guardar" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>

                                <iframe width="100%" height="100%" id="Iframe_guardar" runat="server" frameborder="0"></iframe>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div style="display: none; height: 1px">
                            <asp:Button ID="Button1_guardar" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                            <asp:Button ID="ButtonSalir_post_guardar" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                            <asp:Button ID="Buttoncerrarimpre_post_guardar" runat="Server" Text="X" CssClass="modal_boton_hiden" />
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </div>
            <div id="Impresion_post">
            <asp:Panel ID="Panelimpresionpost" runat="server"  Style="display:none;  width:80%; height: auto" CssClass="modal_content_general_">
                <asp:ModalPopupExtender ID="ModalPopupExtenderimpre_post" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_post"
                    PopupControlID="Panelimpresionpost" CancelControlID="Buttoncerrarimpre_post">
                </asp:ModalPopupExtender>
                <div id="modal_content_Panelimpresionpost" class="modal-content">
                    <div id="divcabecer2_post" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title d-inline ml-1">Menú Impresión</h6>
                        <button type="button" value="Buttoncerrarimpre_post" class="close da_event_captive">&times;</button>
                    </div>
                    <div id="ContenidoImpresion_post" style="border-top:none; overflow:auto; height: auto; width: 100%" class="modal_content_back_">
                        <asp:UpdatePanel ID="UpdatePaneliframe_post" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>

                                <iframe width="100%" height="100%" id="ifimpre_post_" frameborder="0" runat="server" src="../Radicador/WebFormDaImprimir.aspx"></iframe>

                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </asp:Panel>
            <div style="display:none; height:0px">
                 <asp:Button ID="Button1_post" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                 <asp:Button ID="ButtonSalir_post" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                 <asp:Button ID="Buttoncerrarimpre_post" runat="Server" Text="" CssClass="invisible"/>
            </div>
            
        </div>
          <div >
              <div id="progres_bar"<% If WorkflowCentroTrabajoModernActive Then %> class="ctw-loading-indicator" role="status" aria-live="polite"<% End If %> style="position:absolute; text-align: center; display: none; width: 200px">
                <img id="imgr_modal" src="../workflow/loading.gif" style="vertical-align: middle" alt="<% If WorkflowCentroTrabajoModernActive Then %>Procesando<% End If %>" />
            </div>
              <input id="Hidden_gredv_lista" type="hidden" value="YES" runat="server"/>
              <asp:Panel ID="Panelpagina" runat="server" Style="display:none; color: White; width: 80%; height: 100%;  margin-top:1px; margin:auto" CssClass="modal_content_general">
                  <div id="Divcab" class="modal_title_superior">
                      <asp:ModalPopupExtender ID="ModalPopupExtendermesjpagina" runat="Server" Y="0" BackgroundCssClass="FondoAplicacion" TargetControlID="Buttonhide_full"
                          PopupControlID="Panelpagina" CancelControlID="Button_Cerrar" ></asp:ModalPopupExtender>
                      <asp:Label ID="Labeletiqueta" runat="server" Text="Enviar tarea" Font-Size="10"></asp:Label>
                      <div id="Divlabel" style="float: right">
                          <asp:Button ID="Button_Cerrar" runat="Server" Text="X" ToolTip="Cerrar ventana"  CssClass="modal_boton_hiden"/>
                      </div>
                  </div>
                  <div id="DivColorPagina" style="color: White; background-color: #FFFFFF; height: 80%; width: 100%" class="modal_content_back">
                      <asp:Button ID="Buttonhide_full" runat="server" Text="Button" Style="display: none" />
                      <asp:UpdatePanel ID="UpdatePanelpagina" runat="server" UpdateMode="Conditional" >
                          <ContentTemplate>
                               <iframe id="frameeditexpanse_" runat="server" frameborder="0"  scrolling="no" style="width:100%; height:100%"></iframe>
                          </ContentTemplate>
                      </asp:UpdatePanel>

                  </div>
                  <div id="DivBotones" style="height:20%; margin-top:1px; background-color:white; color:black; background-color: #FFFFFF" >
                      <asp:UpdatePanel ID="Updatecondiciona" runat="Server" UpdateMode="Conditional" RenderMode="Inline">
                          <ContentTemplate>              
                              <asp:Button ID="btnCancelpagina" runat="server" Text="Cancelar " Style="float: right; margin-right: 10px; margin-left: 10px; margin-top: 1px" CssClass="boton_blanco" />
                              &nbsp
                              <asp:Button ID="ButtonReasignarTerminar" runat="server" Text="Reasignar " Style="float: right; margin-left: 10px; margin-top: 1px; background-color: yellow" OnClientClick="ConfirmMensaje(&quot;Desea terminar y reasignar la tarea&quot;);" CssClass="boton_blanco" ToolTip="Reasigna y envia la tarea al usuario seleccionado" />
                              &nbsp
                              <asp:Button ID="btnOkpagina" runat="server" Text="Aceptar " Style="float: right; margin-left: 10px; margin-top: 1px" OnClientClick="ConfirmMensaje('Desea enviar las tareas seleccionadas');" CssClass="boton_azul" />
                              &nbsp <asp:CheckBox ID="CheckBox_noti_envio" runat="server" Text="Notifica envío a correo electrónico" Style="float: right; margin-right: 10px; margin-left: 10px; margin-top: 1px; font-family:Arial;font-size:11px" />
                              <input id="Hiddenseltareas" type="hidden" value="0" runat="server"/>
                              <input id="Hidden_res_envi" type="hidden" value="" runat="server"/>                           
                              <input id="Hidden_lista_eliminar_tarea" type="hidden" value="0" runat="server"/>
                          </ContentTemplate>
                          <Triggers>
                          </Triggers>
                      </asp:UpdatePanel>


                  </div>

              </asp:Panel>
          </div>

           <input id="Hidden_name_event" type="hidden" value="" runat="server"/>     
           <input id="Hidden_colum_header" type="hidden" value="" runat="server"/>      
          <asp:Panel ID="Panel_interface_regitra_meta_dato" runat="server" Style="display:none;  width:50%; height: auto" CssClass="modal_content_general_">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_interface_regitra_meta_dato"  runat="server" BackgroundCssClass="FondoAplicacion"  TargetControlID="ButtonSalir_interface_regitra_meta_dato" 
                    CancelControlID="Button_cerrar_interface_regitra_meta_dato" PopupControlID="Panel_interface_regitra_meta_dato" ></asp:ModalPopupExtender>
                <div class="modal-content">
                    <div id="divcabecer2_interface_regitra_meta_dato" class="modal_title_superior_ modal-header">
                        <h6 id="label_interface_regitra_meta_dato" class="modal-title  ">Registra meta dato</h6>
                        <button type="button" value="Button_cerrar_interface_regitra_meta_dato" class="close da_event_captive">&times;</button>
                    </div>
                    <div id="contenido_procesa_interface_regitra_meta_dato" style="background-color: white; width: auto; height: auto; color: black; background-color: #FFFFFF; border-top: none; overflow: auto" class="modal_content_back modal-body">
                        <div id="conte_regitra_meta_dato_control" >
                         
                        </div>
                    </div>
                    <div class="modal-footer" id="modal_footer_regitra_meta_dato">
                        <input id="Button_registra_meta" type="button" value="Aceptar" onclick="event_element_clic(event,this);" class="btn btn-success"/>
                        <input id="Button_file_firma" type="button" value="Aceptar" style="display:none" class="btn btn-success"/>
                    </div>
                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button_cerrar_interface_regitra_meta_dato" runat="Server" Text="" />
                        <asp:Button ID="Button_interface_regitra_meta_dato" runat="server" Text="" Height="1px" Width="1px" />
                        <asp:Button ID="ButtonSalir_interface_regitra_meta_dato" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                    </div>
                </div>
            </asp:Panel>
          
        <div id="inferior_bajo_boton" style="width: 0%; height: 0%; background-color: #E7EDF5; display: none">      
            <asp:UpdatePanel ID="updatapanel_iframe" runat="server" UpdateMode="Conditional">
                <ContentTemplate>   
                     <iframe runat="server" id="ifmExcel_reporte_" width="0" height="0" marginheight="0" marginwidth="0"
                        frameborder="0" />
                   <asp:Button ID="Button_export_lista_event" runat="server" Text="Exportar" style="margin-top:5px; display:none" CssClass="boton_azul"/> 
                  <input id="Hidden_ruta_archivo" type="hidden" value="" runat="server"/>  
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>   
          <asp:Panel ID="Panel_actualiza_tipologia_documental" runat="server" Style="display:none; width: 40%; height: auto" CssClass="modal_content_general_">
            <asp:ModalPopupExtender ID="ModalPopupExtender_edition_actualiza_tipologia_documental" runat="server"
                TargetControlID="ButtonSalir_actualiza_tipologia_documental" BackgroundCssClass="FondoAplicacion"
                CancelControlID="Button_cerrar_actualiza_tipologia_documental" PopupControlID="Panel_actualiza_tipologia_documental">
            </asp:ModalPopupExtender>     
            <div id="modal_content_actualiza_tipologia_documental" class="modal-content">
                <div id="diver_cabcera_actualiza_tipologia_documental" class="modal_title_superior_ modal-header">
                    <h6 class="modal-title d-inline ">Tipologia documental</h6>
                    <button type="button" value="Button_cerrar_actualiza_tipologia_documental" class="close da_event_captive ">&times;</button>
                </div>
                <div id="contenido_procesa_actualiza_tipologia_documental" style="background-color: white; width: 100%; height: 100%; border-top: none" class="modal_content_back modal-body">
                    <asp:UpdatePanel ID="Update_actualiza_tipologia_documental" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>                          
                            <div id="content_data_grid_actualiza_tipologia_documental" class="conten_gred_border_" style="width: 100%">
                                <asp:DropDownList ID="DropDownList_tipologia_documental" style="width:100%" CssClass="custom-select mr-sm-2" runat="server"></asp:DropDownList>
                            </div>                           
                        </ContentTemplate>
                        <Triggers>
                        </Triggers>
                    </asp:UpdatePanel>

                    <div style="display: none; height: 1px">
                        <asp:Button ID="ButtonSalir_actualiza_tipologia_documental" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                        <asp:Button ID="Button_cerrar_actualiza_tipologia_documental" runat="Server" Text="X" CssClass="invisible" />
                    </div>
                </div>
                <div class="modal-footer justify-content-end" id="modal-footer_tipologia_documental">
                    <asp:UpdatePanel ID="UpdatePanel_boton_tipologia_documental" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Button ID="Button_actualiza_tipologia_documental" runat="server" Text="Aceptar" CssClass="btn btn-success" />
                            <input id="Hidden_resulta_botno_tipologia_documental" type="hidden" value="" runat="server"/>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
              
            </div>
        </asp:Panel>
          <asp:Panel ID="Panel_exporta_gabinete_workflow" runat="server" Style="display:none; width: 40%; height: auto" CssClass="modal_content_general_">
            <asp:ModalPopupExtender ID="ModalPopupExtender_edition_exporta_gabinete_workflow" runat="server"
                TargetControlID="ButtonSalir_exporta_gabinete_workflow" BackgroundCssClass="FondoAplicacion"
                CancelControlID="Button_cerrar_exporta_gabinete_workflow" PopupControlID="Panel_exporta_gabinete_workflow">
            </asp:ModalPopupExtender>     
            <div id="modal_content_exporta_gabinete_workflow" class="modal-content">
                <div id="diver_cabcera_exporta_gabinete_workflow" class="modal_title_superior_ modal-header">
                    <h6 class="modal-title d-inline ">Exportar gabinete</h6>
                    <button type="button" value="Button_cerrar_exporta_gabinete_workflow" class="close da_event_captive ">&times;</button>
                </div>
                <div id="contenido_procesa_exporta_gabinete_workflow" style="background-color: white; width: 100%; height: 100%; border-top: none" class="modal_content_back modal-body">
                                         
                            <div id="content_data_grid_exporta_gabinete_workflow" class="conten_gred_border_" style="width: 100%">
                                <asp:DropDownList ID="DropDownList_exporta_gabinete_workflow" style="width:100%" CssClass="custom-select mr-sm-2" runat="server"></asp:DropDownList>
                            </div>                           
                    <div style="display: none; height: 1px">
                        <asp:Button ID="ButtonSalir_exporta_gabinete_workflow" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                        <asp:Button ID="Button_cerrar_exporta_gabinete_workflow" runat="Server" Text="X" CssClass="invisible" />
                    </div>
                </div>
                <div class="modal-footer justify-content-end" id="modal-footer_exporta_gabinete_workflow">
                       <input id="Button_exporta_gabinete_workflow" type="button" value="Aceptar" onclick="event_element_clic(event,this);" class="btn btn-success"/>
                       
                </div> 
            </div>
        </asp:Panel>
           <!--PROGRES-->
        <div id="Divpro_gres_bar">
            <asp:Panel ID="Panel_pro_gres_bar" runat="server" Style="display:none; color: White; width:30%; height:auto" CssClass="border_superior_inferior_radius_blanco">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_pro_gres_bar" runat="server"  TargetControlID="ButtonSalir_pro_gres_bar" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_pro_gres_bar" PopupControlID="Panel_pro_gres_bar"></asp:ModalPopupExtender>
                <div id="div1" class="border_superior_radius_blanco" style="display:none">                
                    <asp:Label ID="Label_pro_gres_bar" runat="server" Text=""  Style="">
                    </asp:Label>
                    <div id="Divcerrarbuton2_pro_gres_bar" style="float: right">
                        <asp:Button ID="Button_cerrar_pro_gres_bar" runat="Server" Text="X" style="display:none" 
                             ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_pro_gres_bar" style="width:99%; height:99%" class="modal_content_back_no_radio" > 
                      <br />   
                    <div style="text-align:center">
                         <asp:Label ID="Label_progres_bar" runat="server" Text="Progreso de la tarea" style="font-family:Arial; text-align:center; font-size:20px"></asp:Label>
                    </div>
                    <br />  
                     <div id="myProgress_contador" style="text-align: center; font-family:Arial; font-size:14px">
                        
                             0 
                        </div>
                        <div id="myProgress_porcent" style="text-align: center; font-family:Arial; font-size:14px">
                            0 %
                        </div>                
                        <div style="margin-left:5%; margin-right:5%">
                            <div id="myProgress" >
                            <div id="myBar" ></div>
                        </div>
                        </div>         
                        <br/>
                        <div style="text-align: center">
                            <button class="boton_blanco" onclick="myStopFunction(event)" >Cancelar</button>
                        </div>
                              
                    <asp:UpdatePanel ID="UpdatePanel_pro_gres_bar" runat="server" UpdateMode="Conditional">
                        <ContentTemplate> 
                           
                            <asp:Button ID="Button_pogres_show" CssClass="invisible" runat="server" Text="Button" style="display:none" />   
                            
                        </ContentTemplate>
                    </asp:UpdatePanel>
                         
                    <asp:Button ID="ButtonSalir_pro_gres_bar" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />    
                </div>
           </asp:Panel>
        </div>  

        <asp:Panel ID="Panel_detail_document_proces_workflow" runat="server" Style="display:none; width: 100%; height: auto" CssClass="modal_content_general_">
            <asp:ModalPopupExtender ID="ModalPopupExtender_edition_detail_document_proces_workflow" runat="server"
                TargetControlID="ButtonSalir_detail_document_proces_workflow" BackgroundCssClass="FondoAplicacion"
                CancelControlID="Button_cerrar_detail_document_proces_workflow" PopupControlID="Panel_detail_document_proces_workflow">
            </asp:ModalPopupExtender>     
             <div id="modal_content_detail_document_proces_workflow" class="modal-content">
                 <div id="diver_cabcera_detail_document_proces_workflow" class="modal_title_superior_ modal-header">
                     <h6 class="modal-title d-inline ">Detalle procesos de imagenes en workflow</h6>
                     <button type="button" value="Button_cerrar_detail_document_proces_workflow" class="close da_event_captive ">&times;</button>
                 </div>
                 <div id="contenido_procesa_detail_document_proces_workflow" style="background-color: white; width: 100%; height: 100%; border-top: none" class="modal_content_back modal-body">
                     <div id="div_content_tabla_procesa_detail_document_proces_workflow" class="table-responsive">
                         <table
                             id="table_boot_detail_document"
                             data-pagination="true"
                             data-page-list="[5,10,15,20,25, 50, 100, all]"
                             data-show-export="true"
                             data-toggle="table"
                             data-id-field="Id_log_docuarchi"
                             data-search="true"
                             data-locale="es-SP">
                             <thead>
                                 <tr>
                                     <th data-field="Id_log_docuarchi" data-visible="false" style="display: none">Id_log_docuarchi</th>
                                     <th data-field="desc_op" data-sortable="true" data-sort-name="desc_op" data-sort-order="desc">OPERACION</th>
                                     <th data-field="USER_OPER">USUARIO DE LA OPERACION</th>
                                     <th data-field="DATE_TRANS">FECHA</th>
                                     <th data-field="HORA_REGISTRO">HORA</th>
                                     <th data-field="id_tran">DOCUMENTO</th>
                                     <th data-field="USER_PROPIETARIO">PROPIETARIO DEL DOCUMENTO</th>
                                     <th data-field="TIPOLOGIA_DOCUMENTAL">TIPOLOGIA</th>
                                     <th data-field="RUT_DOCU">RUTA</th>
                                     <th data-field="GABINETE">GABINETE</th>
                                     <th data-field="CAMPOS">DATOS IDENTIFICACION</th>
                                     <th data-field="RADICADO">RADICADO</th>              
                                 </tr>
                             </thead>
                         </table>
                     </div>
                     <div style="display: none; height: 1px">
                         <asp:Button ID="ButtonSalir_detail_document_proces_workflow" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                         <asp:Button ID="Button_cerrar_detail_document_proces_workflow" runat="Server" Text="X" CssClass="invisible" />
                     </div>
                 </div>
                 <div class="modal-footer justify-content-end" id="modal-footer_detail_document_proces_workflow">
                 </div>
             </div>
        </asp:Panel>
          <!---Panel detail copy docuennt to expedient---->
          <asp:Panel ID="Panel_detail_copy_document_expediente_wf" runat="server" Style="display:none; width: 100%; height: auto" CssClass="modal_content_general_">
            <asp:ModalPopupExtender ID="ModalPopupExtender_edition_detail_copy_document_expediente_wf" runat="server"
                TargetControlID="ButtonSalir_detail_copy_document_expediente_wf" BackgroundCssClass="FondoAplicacion"
                CancelControlID="Button_cerrar_detail_copy_document_expediente_wf" PopupControlID="Panel_detail_copy_document_expediente_wf">
            </asp:ModalPopupExtender>     
             <div id="modal_content_detail_copy_document_expediente_wf" class="modal-content">
                 <div id="diver_cabcera_detail_copy_document_expediente_wf" class="modal_title_superior_ modal-header">
                     <h6 class="modal-title d-inline ">Detalle copia de documentos a expediente</h6>
                     <button type="button" value="Button_cerrar_detail_copy_document_expediente_wf" class="close da_event_captive ">&times;</button>
                 </div>
                 <div id="contenido_procesa_detail_copy_document_expediente_wf" style="background-color: white; width: 100%; height: 100%; border-top: none" class="modal_content_back modal-body">
                     <div id="div_content_tabla_procesa_detail_copy_document_expediente_wf" class="table-responsive">
                         <table
                             id="table_boot_detail_copy_document_expdient"
                             data-pagination="true"
                             data-page-list="[5,10,15,20,25, 50, 100, all]"
                             data-show-export="true"
                             data-toggle="table"
                             data-id-field="id_relacion_wf_produccion"
                             data-search="true"
                             data-locale="es-SP">
                             <thead>
                                 <tr>
                                     <th data-field="id_relacion_wf_produccion" data-visible="false" style="display: none">Id_log_docuarchi</th>
                                     <th data-field="estado_copia_vincula" data-sortable="true" data-sort-name="estado_copia_vincula" data-sort-order="desc">OPERACION</th>
                                     <th data-field="date_registro_trans">FECHA</th>
                                     <th data-field="Nombre_Remitente">USUARIO</th>          
                                     <th data-field="Cargo_Remite">CARGO</th>
                                     <th data-field="DESCRIPCION_TIPO_DOCUMENTO">DOCUMENTO</th>
                                     <th data-field="ID_EXPEDIENTE">CODIGO EXPEDIENTE</th>
                                     <th data-field="codigo_unico">EXPEDIENTE DESTINO</th> 
                                 </tr>
                             </thead>
                         </table>
                     </div>

                     <div style="display: none; height: 1px">
                         <asp:Button ID="ButtonSalir_detail_copy_document_expediente_wf" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                         <asp:Button ID="Button_cerrar_detail_copy_document_expediente_wf" runat="Server" Text="X" CssClass="invisible" />
                     </div>
                 </div>
                 <div class="modal-footer justify-content-end" id="modal-footer_detail_copy_document_expediente_wf">
                     

                 </div>

             </div>
        </asp:Panel>
        <!--Popup registro gestion usuario -->
        <div class="modal fade modal_opacity" id="modal_registro_gestion_usuario_wf" role="dialog" data-backdrop="false">
            <div class="modal-dialog  modal-mediunscreen-sm-down modal-dialog-scrollable">
                <div class="modal-content-fullscreen">
                    <div class="modal-header">
                        <h4 style="color: black" class="modal-title">Registro gestión al usuario</h4>
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>
                    <div class="modal-body" style="">
                       
                        <div id="div_registro_gestion_usuario_wf" style="height:85%"  >
                            <div class="row pb-1">
                                <div class="col-4">
                                    <span>Tipo de gestión</span>
                                </div>
                                <div class="col-8">
                                    <select style="color: black; float: left" id="option_tipos_gestion" class="form-select form-control w-100">
                                        <option></option>
                                    </select>
                                </div>
                            </div>
                            <div class="row pb-1">
                                <div id="content_send_mail" class="d-none w-100">
                                    <div class="col-12">
                                        <select id="tokenize-gestion-gestion-user" class="tokenize-gestion-gestion-user" multiple>
                                        </select>
                                    </div>
                                </div>
                            </div>    
                             <div class="row pb-1">
                                 <div class="col-12" style="overflow:auto">
                                    <textarea class="form-control" id="ContestTextarea" style="resize:none; width:100%" rows="9"></textarea>
                                 </div>
                            </div>
                             <div id="error_registro_gestion_usuario" style="position: relative; width: 100%"></div>
                        </div>
                        <div class="modal-footer justify-content-end " style="height:15%">
                                <input id="Button_registra_gestion" type="button" title="Registra gestión al usuario" value="Aceptar" class="btn btn-success " />
                            </div>
                        
                    </div>
                </div>
            </div>
        </div>   
         <!--Termina registro gestion usuario -->
        <!--Popup lista gestion usuario -->
          <div class="modal fade modal_opacity" id="modal_lista_gestion_usuario_wf" role="dialog" data-backdrop="false">
              <div class="modal-dialog  modal-mediunscreen-sm-down modal-dialog-scrollable">
                  <div class="modal-content-fullscreen">
                      <div class="modal-header" id="modal_header_gestion_usuario_wf">
                          <h4 style="color: black" class="modal-title">Lista gestión al usuario</h4>
                          <button type="button" class="close" data-dismiss="modal">&times;</button>
                      </div>
                      <div class="modal-body" style="">   
                              <div id="contenido_table_boot_lista_gestion_usuario" class="p-1 " style="min-height:450px; height:auto; width: 100%; position: relative; margin-top: 1px; background-color: white; overflow:auto">
                                  <table class="table-not-border_person" style="background-color: white"
                                      id="table_lista_gestion_usuario"
                                      data-pagination="false"
                                      data-page-list="[10, 25, 50, 100, all]"
                                      data-show-export="false"
                                      data-show-refresh="false"
                                      data-cache="false"
                                      data-toggle="table"
                                      data-id-field="id_gestion_tarea_usuario"
                                      data-unique-id="id_gestion_tarea_usuario"
                                      data-click-to-select="true"
                                      data-search="false"
                                      data-locale="es-SP">
                                      <thead class="GridviewScrollHeader_line_blue_wite">
                                      </thead>
                                  </table>
                              </div>    
                          <div id="error_lista_gestion_usuario" style="position: relative; width: 100%"></div>
                      </div>
                  </div>
              </div>
          </div>
         <!--Termina lista gestion usuario -->
          <!--Popup actualiza registro gestion usuario -->
            <div class="modal fade modal_opacity" id="modal_actualiza_registro_gestion_usuario_wf" role="dialog" data-backdrop="false">
                <div class="modal-dialog  modal-mediunscreen-sm-down modal-dialog-scrollable">
                    <div class="modal-content-fullscreen">
                        <div class="modal-header">
                            <h4 style="color: black" class="modal-title">Actualiza gestión al usuario</h4>
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                        </div>
                        <div class="modal-body" style="">
                            
                            <div id="div_actualiza_registro_gestion_usuario_wf" style="height:85%"  >   
                                <div class="row pb-1">
                                    <div id="content_actualiza_send_mail" class="d-none w-100">
                                        <div class="col-12">
                                            
                                        </div>
                                    </div>
                                </div>    
                                 <div class="row pb-1">
                                     <div class="col-12" style="overflow:auto">
                                        <textarea class="form-control" id="ContestTextareaActualiza" style="resize:none; width:100%" rows="9"></textarea>
                                     </div>
                                </div>
                                <div id="error_edicion_registro_gestion_usuario" style="position: relative; width: 100%"></div>
                            </div>
                            
                            <div class="modal-footer justify-content-end " style="height:15%">
                                    <input id="Button_actualiza_registra_gestion" type="button" title="Edita gestión al usuario" value="Aceptar" class="btn btn-success " />
                            </div>
                            
                        </div>
                    </div>
                </div>
            </div>    
        <!--Trmina Popup seleccion -->
        <div id="workflow-transition-modern-modal" class="workflow-transition-modal" hidden="hidden" aria-hidden="true" data-workflow-transition-state="cerrado">
            <div class="workflow-transition-modal__backdrop" data-workflow-transition-close="true"></div>
            <section id="workflow-transition-modern-dialog" class="workflow-transition-modal__dialog" role="dialog" aria-modal="true" aria-labelledby="workflow-transition-modern-title" tabindex="-1">
                <header class="workflow-transition-modal__header">
                    <h2 id="workflow-transition-modern-title" class="workflow-transition-modal__title">Seleccionar destino</h2>
                    <button id="workflow-transition-modern-close" class="workflow-transition-modal__close" type="button" aria-label="Cerrar lista de destinos">&times;</button>
                </header>
                <div class="workflow-transition-modal__body">
                    <div id="workflow-transition-modern-status" class="workflow-transition-modal__status" role="status" aria-live="polite"></div>
                    <dl id="workflow-transition-modern-context" class="workflow-transition-modal__context" aria-label="Contexto de la tarea"></dl>
                    <div id="workflow-transition-modern-table" class="workflow-transition-modal__desktop">
                        <table class="workflow-transition-modal__table">
                            <thead><tr><th scope="col">Destino</th><th scope="col">Destinatario o grupo</th><th scope="col">Tipo</th><th scope="col"><span class="sr-only">Acción</span></th></tr></thead>
                            <tbody id="workflow-transition-modern-table-body"></tbody>
                        </table>
                    </div>
                    <div id="workflow-transition-modern-cards" class="workflow-transition-modal__mobile" aria-label="Destinos disponibles"></div>
                </div>
            </section>
        </div>
        <div id="workflow-transition-success-message" class="workflow-transition-success-message" data-workflow-transition-success="true" role="status" aria-live="polite" hidden="hidden"></div>
        <div id="workflow-group-send-modern-modal" class="workflow-transition-modal" hidden="hidden" aria-hidden="true" data-workflow-transition-state="cerrado">
            <div class="workflow-transition-modal__backdrop" data-workflow-group-send-close="true"></div>
            <section id="workflow-group-send-modern-dialog" class="workflow-transition-modal__dialog" role="dialog" aria-modal="true" aria-labelledby="workflow-group-send-modern-title" tabindex="-1">
                <header class="workflow-transition-modal__header">
                    <h2 id="workflow-group-send-modern-title" class="workflow-transition-modal__title">Enviar a grupo</h2>
                    <button id="workflow-group-send-modern-close" class="workflow-transition-modal__close" type="button" aria-label="Cerrar lista de actividades">&times;</button>
                </header>
                <div class="workflow-transition-modal__body">
                    <div id="workflow-group-send-modern-status" class="workflow-transition-modal__status" role="status" aria-live="polite"></div>
                    <dl id="workflow-group-send-modern-context" class="workflow-transition-modal__context" aria-label="Contexto de la tarea"></dl>
                    <div id="workflow-group-send-modern-table" class="workflow-transition-modal__desktop">
                        <table class="workflow-transition-modal__table">
                            <thead><tr><th scope="col">Actividad destino</th><th scope="col">Grupo destino</th><th scope="col"><span class="sr-only">Acción</span></th></tr></thead>
                            <tbody id="workflow-group-send-modern-table-body"></tbody>
                        </table>
                    </div>
                    <div id="workflow-group-send-modern-cards" class="workflow-transition-modal__mobile" aria-label="Actividades de destino disponibles"></div>
                </div>
            </section>
        </div>
        <!--mensaje_personalizado-->
        <asp:Panel ID="Panel_mensaje_personalizado" runat="server" Style="display: none; color: black; width: auto; height: auto; z-index: 99999999999">
            <asp:ModalPopupExtender ID="ModalPopupExtender_mensaje_personalizado" runat="server"
                TargetControlID="Button_mensaje_personalizado" BackgroundCssClass="FondoAplicacion"
                CancelControlID="Button_cerrar_mensaje_personalizado" PopupControlID="Panel_mensaje_personalizado">
            </asp:ModalPopupExtender>
            <div class="modal-content">
                <div id="div_persoanlizado" class="modal-header">
                    <a class="modal-title h6 " href="#" style="color: orange"><i class="fas fa-exclamation-triangle"></i></a>
                    <br />
                    <button type="button" onclick="document.getElementById('Button_cerrar_mensaje_personalizado').click();" class="close">&times;</button>
                </div>
                <div id="contenido_procesa_mensaje_personalizado" style="max-width: 450px; max-height: 350px; background-color: white; color: black; overflow: auto" class="modal-body  text-justify">
                    <asp:Label ID="Label_mensaje_personalizado" runat="server" Text="Detalle" Style=""></asp:Label>
                    <asp:Button ID="Button_mensaje_personalizado" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_mensaje_personalizado" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="Button_cerrar_mensaje_personalizado" CssClass="invisible" runat="Server" />
                </div>
                <div class="modal-footer ">
                    <button type="button" class="btn  btn-light  float-right" style="margin-right: 5px; color: orange" onclick="document.getElementById('Button_cerrar_mensaje_personalizado').click();">Aceptar </button>
                </div>
            </div>
        </asp:Panel>
        <!--Termina mensaje_personalizado-->
        
      </form>
     
      <script  accesskey="javascript" type="text/javascript"> 
          
          AjaxFileUpload_change_text();
          
          $(document).ready(function () {
              $('#sidebarCollapse_').on('click', function () {
                  $('#sidebar__').toggleClass('active_da_slider');
                  $('#Contenedorderecho_').toggleClass('active_content_rigth');
                  $('#Contentizquierdo_').toggleClass('active_content_left');
                  $(this).toggleClass('active_da_slider');
                  $('#da_show-sidebar__').toggleClass('show_da_slide');
                  $('#da_show-sidebar__').toggleClass('hide_da_sidebar');
              });
              $('#da_show-sidebar__').on('click', function () {
                  $('#sidebar__').toggleClass('active_da_slider');
                  $('#Contenedorderecho_').toggleClass('active_content_rigth');
                  $('#Contentizquierdo_').toggleClass('active_content_left');
                  $(this).toggleClass('show_da_slide');
                  $(this).toggleClass('hide_da_sidebar');
              });
              $('#sidebarCollapse').on('click', function () {
                  $(this).toggleClass('active_da_slider_rigth');
                  $('#da_show-sidebar_').toggleClass('show_da_slide_rigth');
                  $('#da_show-sidebar_').toggleClass('hide_da_sidebar_rigth');
                  $("#contenido_indice").css("display", "none");
                  $("#contenido_imagen").css("width", "75%");
                  auto_zise_popup_workflow(1);
              });
              $('#da_show-sidebar_').on('click', function () {
                  $(this).toggleClass('show_da_slide_rigth');
                  $(this).toggleClass('hide_da_sidebar_rigth');
                  $("#contenido_indice").css("display", "block");
                  $("#contenido_indice").css("width", "20%");
                  $("#contenido_imagen").css("width", "55%");
                  auto_zise_popup_workflow(1);
              });
          });
   
      </script>
         <script  accesskey="javascript" type="text/javascript">         
             timer();
         </script>   
    </body> 

  </html>
  
  
