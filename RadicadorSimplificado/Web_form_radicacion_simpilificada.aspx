<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Web_form_radicacion_simpilificada.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.Web_form_radicacion_simpilificada" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
     <title>Radicación simplificada</title>
    <script src="../js/ui/jquery-3.4.1.min.js"></script>  
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
    <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
    <link href="../ccs/themes/base/jquery.ui.all.css" rel="stylesheet" type="text/css" />   
    <link rel="stylesheet" href="../Styles/style.css" />
    <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
    <script src="../js/validate_campos.js"></script> 
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <script src="../js/RadicadorSimplificado/Web_form_radicacion_simpilificada.js"></script>
    <script src="../js/java_general/general_code_java.js"></script>
    <script src="../js/java_general/general_config.js"></script>
    <script src="../js/java_general/general_control_java.js"></script>
    <script src="../js/java_general/JSProgresBar.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.11.0/umd/popper.min.js" type="text/javascript"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />  
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-table@1.23.1/dist/bootstrap-table.min.css"/>
    <script src="https://cdn.jsdelivr.net/npm/tableexport.jquery.plugin@1.29.0/tableExport.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap-table@1.23.1/dist/bootstrap-table.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap-table@1.23.1/dist/bootstrap-table-locale-all.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap-table@1.23.1/dist/extensions/export/bootstrap-table-export.min.js"></script>
    <script src="../js/table_boo/table_boot_config.js"></script>
    <script src="../js/java_general/BootstrapTable.js"></script>
    <script src="../js/java_general/JS_firma_digital.js"></script>
    <script src="../js/java_general/gestion_meta_dato.js"></script>
    <script src="../js/java_general/JSReplaceScanFile.js"></script>
    <script src="../js/versiondocumento/gestion_version_documento.js"></script>
    <script  src="../Awesome/js/all.js"></script>
    <script src="../js/java_general/ubicacion_code_java.js" type="text/javascript"></script>   
    <script src="../generic_control/FileUploadHandler.js" type="text/javascript"></script>
    <link href="../generic_control/UploadFile.css" rel="stylesheet" />
    <link href="../Awesome/css/fontawesome.css" rel="stylesheet"/>
    <link href="../Awesome/css/brands.css" rel="stylesheet"/>
    <link href="../Awesome/css/solid.css" rel="stylesheet"/>
    <script  src="../Awesome/js/brands.js"></script>
    <script  src="../Awesome/js/solid.js"></script>
    <script  src="../Awesome/js/fontawesome.js"></script>
    <link href="../Styles/w3.css" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/tom-select@2.3.1/dist/css/tom-select.css" rel="stylesheet"/>
    <script src="https://cdn.jsdelivr.net/npm/tom-select@2.3.1/dist/js/tom-select.complete.min.js"></script>
    <script src="../js/java_general/TomSelectComplent.js"></script>
    
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeout="900">
            
        </asp:ScriptManager>
        <script accesskey="javascript" type="text/javascript">
                Sys.WebForms.PageRequestManager.getInstance().add_initializeRequest(InitializeRequest);
                Sys.Application.add_load(ApplicationLoadHandler)
                var elment_postbak;
                function ApplicationLoadHandler(sender, args) {
                    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(CheckStatus);
                }
                function InitializeRequest(sender, args) {    
                    elment_postbak = args.get_postBackElement();                
                    posicion_update_pogres('progres_bar');       
                }
                function CheckStatus(sender, args) {
                    try {     
                    progres_hiden('progres_bar');
                      
                    }
                      catch (err) {
                          alert(err.message + " Funcion CheckStatus");
                    }
                }
                
        </script>   
        <div id="div_content_general_radicacion_simpleficada" style="width: auto; height: 100%">
            <div id="div_error_content_content_general_rad_simp" style="position: relative; width: 100%"></div>
            <div id="menu_content_radicacion_simplificada" class="navbar navbar-expand-sm nav_botota_person_gray modal_content_no_back_inferior">
                <div style="float: right" class="div_link_pend ml-auto pd-2">
                    <div id="Panel_pendiente_radicado" style="display: none" class="navbar-nav ">
                        <a class="nav-link_ float-right mr-2" title="Subir radicado a pendiente" id="boton_rad_send_pend" href="#settings"><i style="color: darkorange" class="fad fa-arrow-up "></i> </a>
                        <a class="nav-link_ float-right" title="lista pendientes" id="boton_rad_list_task" href="#settings"><i style="color: darkorange" class="fas fa-bell "></i> Pendientes : </a>
                        <span id="Label_numero_item" runat="server" class="h6 font-weight-light ml-1 pt-1" style="color: #6d7fcc; font-family: 'Segoe UI'"></span>
                    </div>   
                </div>
            </div>
            <div class="tab-content container_ body-content m-1">
                <div class="tab-pane  pt-0 col-xs-12 tab_rad_simple" style="height: 100%" id="home_datos_registro_tramite" role="tabpanel" aria-labelledby="home-tab">
                    <h5 class="rotulo-general_title_gestor d-none">Registro del tramite </h5>
                    <div id="error_div_datos_ingreso" style="position: relative; width: 100%"></div>
                    <div id="div_datos_registro_tramite" style="height: 100%; overflow:auto" class="p-3">
                    </div>
                    <div class="modal-footer" id="foter_registro_tramite">
                        <button type="button" id="Button_div_limpiaar_campo" class="btn  btn-secondary" title="">Limpiar</button>
                        <button type="button" id="Button_div_registro_tramite" class="btn  btn-primary" title="">Aceptar</button>
                    </div>
                </div>
                <div class="tab-pane  pt-0 col-xs-12 tab_rad_simple" style="height: 100%" id="home_soporte_documento" role="tabpanel" aria-labelledby="home-tab">          
                    <div id="error_div_soporte_documento" style="position: relative; width: 100%"></div>
                    <div id="div_soporte_documento" style="height: 100%">
                        <div id="div_title_soporte_documental">
                            <h5 class="rotulo-general_title_gestor d-none">Soporte documental y asignación</h5>
                        </div> 
                        <nav id="navar_rad_simple_barra" class="navbar navbar-expand-sm nav_botota_person modal_content_no_back_inferior">
                            <button class="navbar-toggler" type="button" style="background-color: #6d7fcc" data-toggle="collapse" data-target="#navbarNavDropdown">
                                <span class="navbar-toggler-icon_"><i style="color: white" class="fad fa-th-list"></i></span>
                            </button>
                            <div class="collapse navbar-collapse row" id="navbarNavDropdown">
                                <div id="Panel_imprime_rotulo" style="display:none" class="navbar-nav  d-none">
                                    <ul class="navbar-nav">
                                        <li class="nav-item active ml-2 active_">
                                            <a class="nav-link" id="boton_rad_simpl_printer_rot" style="color: #6d7fcc" title="Imprimir rotulo radicado" href="#" ><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-print"></i> Imprimir rotulo  </a>
                                        </li>
                                        <li class="nav-item active ml-2 active_">
                                            <a class="nav-link" id="boton_rad_simpl_save_rot" style="color: #6d7fcc" title="Guarda rotulo radicado" href="#" ><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-save"></i> Guardar rotulo  </a>
                                        </li>
                                        <li class="nav-item active ml-2 active_">
                                            <a class="nav-link" id="boton_rad_simpl_detail_rad" style="color: #6d7fcc" title="Detalle radicado" href="#" ><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-ellipsis-v"></i> Detalle radicado  </a>
                                        </li>
                                        <li class="nav-item active ml-2 active_">
                                            <a class="nav-link" href="#">
                                                <i style="margin-left: 1px; margin-top: 7px; color: #6d7fcc" class="fad fa-sticky-note"></i>
                                                <span id="boton_rad_simpl_gestion_not_rad" class="font-weight-light" style="color: darkorange" title="Notas" > Notas  </span>
                                            </a>
                                        </li>
                                    </ul>
                                </div>
                                <div id="Panel_cargar_archivo" style="display:none" class="navbar-nav ">
                                    <ul class="navbar-nav">
                                        <li class="nav-item active ml-2 active_">
                                            <a class="nav-link" id="boton_rad_simpl_load_file" style="color: #6d7fcc" title="Cargar archivo a la lista" href="#" ><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-file-upload"></i> Cargar archivo  </a>
                                        </li>
                                    </ul>
                                </div>
                                <div id="Panel_auto_vincular" style="display:none" class="navbar-nav " runat="server">
                                    <ul class="navbar-nav">
                                        <li class="nav-item active ml-2 active_">
                                            <a class="nav-link" id="boton_rad_simpl_auto_vincula" style="color: #6d7fcc" title="Vincula documentos al expediente" href="#" ><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-folder-download"></i> Vincular  </a>
                                        </li>
                                    </ul>
                                </div>
                                <div id="Panel_EnviarUsuario" style="display:none" class="navbar-nav ">
                                    <ul class="navbar-nav">
                                        <li class="nav-item active ml-2 active_">
                                            <a class="nav-link" id="boton_rad_simpl_send_user_task" style="color: #6d7fcc" title="Envía el radicado a usuario" href="#" ><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-user"></i> Enviar a usuario  </a>
                                        </li>
                                    </ul>
                                </div>
                                <div id="Panel_EnviaActividad" style="display:none" class="navbar-nav ">
                                    <ul class="navbar-nav">
                                        <li class="nav-item active ml-2 active_">
                                            <a class="nav-link" id="boton_rad_simpl_send_task_gorup" style="color: #6d7fcc" title="Asignar tarea de radicado" href="#" ><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-user-friends"></i> Enviar a grupo  </a>
                                        </li>
                                    </ul>
                                </div>
                                <div id="Panel_enviar_flujo" class="navbar-nav " style="display:none" runat="server">
                                    <ul class="navbar-nav">
                                        <li class="nav-item active ml-2 active_">
                                            <a class="nav-link" id="boton_rad_simpl_send_task_flow" style="color: #6d7fcc" title="Enviar y terminar" href="#" ><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fas fa-user"></i> Terminar   </a>
                                        </li>
                                    </ul>
                                </div>
                                <div id="Panel_auto_terminar" style="display:none" class="navbar-nav ">
                                    <ul class="navbar-nav">
                                        <li class="nav-item active ml-2 active_">
                                            <a class="nav-link" id="boton_rad_simpl_send_task_rad_gestion" style="color: #6d7fcc" title="Envía el radicado a gestión" href="#" ><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-check"></i> Enviar a gestión  </a>
                                        </li>
                                    </ul>
                                </div>
                                <div id="Panel_terminar_radicado" style="display:none" class="navbar-nav " runat="server">
                                    <ul class="navbar-nav">
                                        <li class="nav-item active ml-2 active_">
                                            <a class="nav-link" id="boton_rad_simpl_end_rad" style="color: #6d7fcc" title="Terminar radicado" href="#" ><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-check"></i> Terminar radicado  </a>
                                        </li>
                                    </ul>
                                </div>
                                <div style="float: right" class="div_link_pend ml-auto pd-2">
                                    <span id="Label_estado_selecion" class="h6 font-weight-light" style="color: #6d7fcc; font-size: 10px; display: block"></span>
                                </div>
                            </div>
                        </nav>
                        <div id="conte_rad_simpe_table_waper" class="container-fluid mr-0 ml-0 pl-0 pr-0" style="border-top: 1px solid #e9ecef">
                            <a id="da_show-sidebar_" class="btn btn-sm   hide_da_sidebar d-none" href="#" data-target="#sidebar_">
                                <i style="color: white" class="fas fa-bars"></i>
                            </a>
                            <div id="rad_simple_content_wraper" class="wrapper_ ml-0 mr-0  d-flex  justify-content-between_" style="padding-left: 1px; padding-right: 1px">
                                <div id="rad_simple_contentizquierdo" class="bg-light_ " style="width: 22%; float: left">
                                    <nav id="sidebar_" class=" bg-light_ pl-0 pr-0">
                                        <div id="div_rad_simple_title" class="modal-header_ modal_title_superior " style="border-top-left-radius: initial; border-top-right-radius: initial; border-bottom: 1px solid #e9ecef; border-right: 1px solid #e9ecef">
                                            <div class="row">
                                                <div class="col-4">
                                                    <span id="Label_documentos" class=" mt-2 mb-2 ml-2  h8 f font-weight-light" style="color: #6d7fcc; float: left; font-family: 'Segoe UI'" runat="server">Documentos</span>
                                                </div>
                                                <div class="col-8">
                                                    <div class="nav-item_ active active_">
                                                        <a id="sidebarCollapse" class="nav-link pr-2 pl-2 d-none" style="float: right; color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600" title="Cerrar lista"><i class="fad fa-bars"></i></a>
                                                        <a id="delete_row_several_rad" class="nav-link pr-2 pl-2 d-none" style="color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600; float: right" title="Eliminar documentos" href="#"><i style="" class="fad fa-trash-alt"></i></a>
                                                        <a id="delete_wodloa_file_rad" class="nav-link pr-2 pl-2" style="color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600; float: right" title="Adjuntar documento" href="#" ><i style="" class="fas fa-upload "></i></a>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div id="div_rad_simple_content_table" style="width: 100%; border-right: 1px solid #e9ecef">
                                            <table class="table-not-border_person_ table  table-no-borders" style="background-color: white"
                                                id="table_doc_flow_select"
                                                data-pagination="false"
                                                data-page-list="[5,10, 25, 50, 100, all]"
                                                data-page-size="5"
                                                data-show-export="false"
                                                data-show-refresh="false"
                                                data-cache="true"
                                                data-toggle="table"
                                                data-id-field="ID"
                                                data-unique-id="ID"
                                                data-single-select="true"
                                                data-search="false"
                                                data-locale="es-SP">
                                                <thead class="GridviewScrollHeader_line_blue_wite d-none" >
                                                </thead>
                                            </table>
                                        </div>
                                        <div id="div_rad_simple_contenido_pie" style="border-top-left-radius: initial; border-top-right-radius: initial; display: none" class="modal-header pt-1 pb-1   justify-content-start">
                                            <h6 class="modal-title_ mt-2 mb-2 ml-2   font-weight-light" id="pit" style="color: white"></h6>
                                        </div>
                                    </nav>
                                </div>
                                <div id="rad_simple_contenedorderecho" class="page-content mr-0 ml-0 pl-1 pr-1 pb-0 pt-0  " style="width: 78%">
                                    <div id="Are_Digitalizacion" style="width: 100%; height: 100%; float: right; display: none; margin-left: 1px" class="modal_content_back_">
                                        <iframe id="IframeDitaliza_" runat="server" frameborder="0" src="" width="100%" scrolling="no" height="100%"></iframe>

                                    </div>
                                    <div class="d-none">
                                        <input id="HiddenIdFlujo" type="hidden" value="0" runat="server" />
                                        <button class="d-none" id="save_document_scan"></button>
                                    </div>
                                    <div id="Area_Visor" style="width: 100%; height: 100%; display: none" class="modal_content_back_">
                                        <div id="div_cerrar_rad_simple" class="modal-header_ modal_title_superior " style="border-top-left-radius: initial; border-top-right-radius: initial; border-bottom: 1px solid #e9ecef">
                                            <h6 id="titel_visor" class="mt-2 mb-2 ml-2  h6 font-weight-light" style="color: #6d7fcc; font-family: 'Segoe UI'; float: left">Visor externo</h6>
                                            <button type="button" title="Cerrar ventana visualizador" onclick="prevent_cerrar(event,this);" class="close mr-1" style="float: right">&times;</button>
                                        </div>
                                        
                                        <iframe id="IframeVisor_" runat="server" frameborder="0" width="100%" scrolling="no" height="100%"></iframe>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div id="div_rad_simple_footer" class=" modal-footer justify-content-between p-1">
                            <h6 id="Label_estado_transac" style="  float: left; color: #6d7fcc; font-family: 'Segoe UI'" class="h6 font-weight-light mt-2" ></h6>
                        </div>
                    </div>        
                </div>        
            </div>
        </div>
        <!--Popup lista radicados pendietes -->
        <div class="modal fade modal_opacity" id="modal_content_lista_radicados_pendientes" role="dialog">
            <div class="modal-dialog  modal-mediunscreen-sm-down modal-dialog-scrollable">
                <div class="modal-content-fullscreen">
                    <div class="modal-header">
                        <h4 id="label_title_lista_radicados_pendientes" style="color: black" class="modal-title">Radicados pendientes por enviar</h4>
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>
                    <div class="modal-body-fullscreen modal-body" style="overflow:auto">
                        <div id="div_lista_radicados_pendientes" style="height: 100%">
                            <table class="table-not-border_person" style="background-color: white"
                                id="table_list_radicados_table"
                                data-pagination="false"
                                data-page-list="[5,10, 25, 50, 100, all]"
                                data-page-size="5"
                                data-show-export="false"
                                data-show-refresh="false"
                                data-cache="false"
                                data-toggle="table"
                                data-id-field="id_estado_radicado"
                                data-click-to-select="true"
                                data-search="true"
                                data-locale="es-SP">
                                <thead class="GridviewScrollHeader_line_blue_wite">
                                </thead>
                            </table>
                        </div>
                    </div>
                    <div id="error_content_lista_radicados_pendientes" style="position: relative; width: 100%"></div>
                    <div class="modal-footer align-content-end" id="modal_foter_lista_radicados_pendientes">
                        
                       
                    </div>
                </div>
            </div>
        </div>   
         <!--Termina Popup lista radicados pendietes -->
        <!--Popup envio tarea flujo -->
        <div class="modal fade modal_opacity" id="modal_content_enviar_tarea_flujo" role="dialog">
            <div class="modal-dialog  modal-mediunscreen-sm-down modal-dialog-scrollable">
                <div class="modal-content-fullscreen">
                    <div class="modal-header">
                        <h4 id="label_title_enviar_tarea_flujo" style="color: black" class="modal-title"></h4>
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>
                    <div class="modal-body-fullscreen modal-body" style="overflow:auto">
                        <div id="div_enviar_tarea_flujo" style="height: 100%">
                            <table class="table-not-border_person" style="background-color: white"
                                id="table_send_task_flow_table"
                                data-pagination="false"
                                data-page-list="[5,10, 25, 50, 100, all]"
                                data-page-size="5"
                                data-show-export="false"
                                data-show-refresh="false"
                                data-cache="false"
                                data-toggle="table"
                                data-id-field="ID_REGISTRO_ACTIVIDAD_ENVIO"
                                data-click-to-select="true"
                                data-search="false"
                                data-locale="es-SP">
                                <thead class="GridviewScrollHeader_line_blue_wite">
                                </thead>
                            </table>
                        </div>
                    </div>
                    <div id="error_content_enviar_tarea_flujo" style="position: relative; width: 100%"></div>
                    <div class="modal-footer align-content-end" id="modal_foter_enviar_tarea_flujo">
                        
                       
                    </div>
                </div>
            </div>
        </div>   
         <!--Termina Popup envio tarea flujo -->
           <!--Popup cambia tipo documental -->
         <div class="modal fade modal_opacity" id="modal_cambiar_tipologia_documento" role="dialog" >
             <div class="modal-dialog modal-dialog-scrollable">
                 <div class="modal-content" >
                     <div class="modal-header">
                        <h4 style="color: black" class="modal-title">Cambia tipo documental</h4>
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>
                     <div class="modal-body">
                         <div class="row " id="row_cambia_tipo_documental">
                             <div class="col-4">
                                 <span style="color: black" class="font-weight-light">Tipo documental  </span>
                             </div>
                             <div class="col-8">
                                 <select style="color: black; float: left" id="option_cambia_tipo_documental" class="form-select form-control w-100" >
                                     <option></option>
                                 </select>
                             </div>
                         </div>
                     </div>
                     <div id="error_cambiar_tipologia_documento" style="position: relative; width:100%"></div>
                     <div class=" modal-footer">
                        <button type="button" id="Button_cambia_tipologia_documental" class="btn  btn-success"  title="Cambia tipologia documental ">Aceptar</button>
                     </div>
                 </div>
             </div>
         </div>     
        <!--Trmina Popup cambia tipo documental-->
         <!--Popup adjunta documento version -->
         <div class="modal fade modal_opacity" id="modal_adjunta_documeto_version_document" role="dialog">
             <div class="modal-dialog  modal-mediunscreen-sm-down ">
                 <div class="modal-content-fullscreen">
                     <div class="modal-header">
                        <h4 style="color: black" class="modal-title">Adjuntar documentos</h4>
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>
                     <div class="modal-body-fullscreen modal-body">
                          <div class="row row-body-fullscreen">
                              
                              <div class="p-4 w-100">
                                  <div id="Div_content_sube_documento_content_general" style="height: auto; width: 100%; border-top: none" class="modal_content_back">
                                     <select id="DropDownList_adjunta_documeto_version_document" style="width: 100%" class="custom-select mr-sm-2" ></select>       
                                  </div>
                                  <div class="row p-2" id="content_boton_adjunta_documeto_version_document">
                                      <div class="col-12 p-0 pl-1">
                                          <div class="file-select " id="src-file_">
                                              <input id="file_element_adjunta_documeto_version_document" type="file" multiple="multiple" accept="" style="width: 100px; height: 40px" name="src-file" class="p-1" contente_file="ModalPopupExtender_sube_documento_adjunto" aria-label="Archivo" />
                                          </div>  
                                          <a id="save_file_element_adjunta_documeto_version_document" title="Guardar todos los archivos" class="btn  btn-success ml-1" style="opacity: 0; color: white"><i style="color: white" class="fas fa-save "> </i> Guardar </a>
                                          <a id="delete_file_element_adjunta_documeto_version_document" title="Elminar todos los archivos cargados" class="btn  btn-danger " style="opacity: 0; color: white"><i style="color: white" class="fal fa-trash-alt "> </i> Eliminar </a>
                                          <a id="cancel_file_element_adjunta_documeto_version_document" title="Cancelar guardar archivos" class="btn  btn-warning" style="opacity: 0; color: white"><i style="color: white" class="fas fa-window-close "> </i> Cancelar </a>
                                      </div>
                                  </div>
                                  <div class="paren_element background_upload" id="conten_file_element_adjunta_documeto_version_document" style="overflow: auto; height: 80%">

                                      <div id="content_drop_element_adjunta_documeto_version_document" claas="">
                                      </div>
                                      <table id="table_file_element_adjunta_documeto_version_document" class="table table-striped">
                                      </table>
                                  </div>
                                  
                              </div>
                          </div>
                    </div>
                     <div id="error_content_adjunta_documeto_version_document" style="position: relative; width:100%" class="pl-4 pr-4"></div>
                     <div class=" modal-footer_">
                         <div class="row border_ pt-2 w-100" id="content_pie_title_adjunta_documeto_version_document" >
                                      <div class="col-8 justify-content-start">
                                          <div class="row p-2">
                                              <div class="col-4 p-0">
                                                  <div>
                                                      <asp:Label ID="Label_progres_bar_file_element_adjunta_documeto_version_document" runat="server" Text="" Style="font-family: Arial; text-align: center; font-size: 20px"></asp:Label>
                                                  </div>
                                                  <div id="pogres_file_element_contador_adjunta_documeto_version_document" style="text-align: center; font-family: Arial; font-size: 14px">
                                                  </div>
                                                  <div id="pogres_file_element_porcent_adjunta_documeto_version_document" style="text-align: center; font-family: Arial; font-size: 14px">
                                                  </div>
                                              </div>
                                              <div class="col-5 p-0">
                                                  <div>
                                                      <div id="myProgress_file_element_adjunta_documeto_version_document">
                                                          <div id="myBar_file_element_adjunta_documeto_version_document" class="file-select-bar"></div>
                                                      </div>
                                                  </div>
                                              </div>
                                              <div class="col-3 p-0 pl-3">
                                                  <p id="count_byte_file_element_adjunta_documeto_version_document"></p>
                                              </div>
                                          </div>

                                      </div>
                                      <div class="col-4 justify-content-end pt-2">
                                          <p id="count_file_element_adjunta_documeto_version_document" class="font-weight-light" style="float: right">Estado </p>
                                      </div>
                                  </div>
                     </div>
                 </div>
             </div>
         </div>
        <!--Termina popup adjunta documento version -->
         <!--Popup registro solicititante -->
        <div class="modal fade modal_opacity" id="modal_content_registro_validacion_externo" data-backdrop="static" data-keyboard="false" tabindex="-1" aria-labelledby="staticBackdropLabel" aria-hidden="true">
            <div class="modal-dialog  modal-mediunscreen-sm-down modal-dialog-scrollable">
                <div class="modal-content-fullscreen">
                    <div class="modal-header">
                        <h4 id="label_title_registro_validacion_externo" style="color: black" class="modal-title"></h4>
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>
                    <div class="modal-body-fullscreen modal-body" style="overflow:auto">
                        <div id="div_registro_validacion_externo" style="height: 100%">
                        </div>
                    </div>
                    <div id="error_content_registro_validacion_externo" style="position: relative; width: 100%"></div>
                    <div class="modal-footer align-content-end" id="modal_foter_registro_validacion_externo">
                        <button type="button" id="Boton_event_registro_validacion_externo" title="Registro del solicitante" style="display: block" class="btn btn-success   mt-1">Aceptar</button>
                       
                    </div>
                </div>
            </div>
        </div>   
         <!--Termina Popup registro solicititante -->
         <!--Popup actualización solicititante -->
        <div class="modal fade modal_opacity" id="modal_content_actualizacion_validacion_externo" data-backdrop="static" data-keyboard="false" tabindex="-1" aria-labelledby="staticBackdropLabel" aria-hidden="true">
            <div class="modal-dialog  modal-mediunscreen-sm-down modal-dialog-scrollable">
                <div class="modal-content-fullscreen">
                    <div class="modal-header">
                        <h4 id="label_title_actualizacion_validacion_externo" style="color: black" class="modal-title"></h4>
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>
                    <div class="modal-body-fullscreen modal-body" style="overflow:auto">
                        <div id="div_actualizacion_validacion_externo" style="height: 100%">
                        </div>
                    </div>
                    <div id="error_content_actualizacion_validacion_externo" style="position: relative; width: 100%"></div>
                    <div class="modal-footer align-content-end" id="modal_foter_actualizacion_validacion_externo">
                        <button type="button" id="Boton_event_actualizacion_validacion_externo" title="Editar el solicitante" style="display: block" class="btn btn-success   mt-1">Aceptar</button>
                    </div>
                </div>
            </div>
        </div>   
        <!--Termina Popup actualización solicititante -->
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
        <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 200px">
            <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
            Processing ...
        </div>
    </form>
</body>
</html>
