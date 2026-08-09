<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormDaPrincipal.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormDaPrincipal" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Docuarchi.net</title>
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
    <script src="../js/table_boo/table_boot_config.js" type="text/javascript"></script>
    <script src="../js/java_general/BootstrapTable.js" type="text/javascript"></script>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-table@1.23.1/dist/bootstrap-table.min.css"/>
    <script src="https://cdn.jsdelivr.net/npm/tableexport.jquery.plugin@1.29.0/tableExport.min.js" type="text/javascript"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap-table@1.23.1/dist/bootstrap-table.min.js" type="text/javascript"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap-table@1.23.1/dist/bootstrap-table-locale-all.min.js" type="text/javascript"></script>
    <!-- Cargar primero la librería XLSX -->
    <script src="https://cdnjs.cloudflare.com/ajax/libs/xlsx/0.17.1/xlsx.full.min.js" type="text/javascript"></script>
    <link href="https://fonts.googleapis.com/css2?family=Roboto:wght@400;500&display=swap" rel="stylesheet"/>

    <!-- Luego, carga el script de exportación de bootstrap-table -->
    <script src="https://cdn.jsdelivr.net/npm/bootstrap-table@1.23.1/dist/extensions/export/bootstrap-table-export.min.js" type="text/javascript"></script>
    <link href="../Styles/styleMenu.css" rel="stylesheet" type="text/css" /> 
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <script src="../js/validate_campos.js" type="text/javascript"></script>
    <script src="../js/java_general/general_config.js" type="text/javascript"></script>
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <script src="../js/java_general/general_control_java.js"></script>
    <script src="../js/java_general/general_code_java.js"></script>
    <link href="../Styles/Menu3.css" rel="stylesheet" />
    <link href="../Styles/tabs.css" rel="stylesheet" />    
    <script src="../js/java_general/JSReplaceScanFile.js" type="text/javascript"></script>
    <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <script src="../js/java_general/GredviewControl.js" type="text/javascript"></script>
    <script src="../js/java_general/general_code_java.js" type="text/javascript"></script>
    <script src="../generic_control/FileUploadHandler.js" type="text/javascript"></script>
    <script src="../js/java_general/ASMXClient.js" type="text/javascript"></script>
    <script src="../js/java_general/JSProgresBar.js" type="text/javascript"></script>
    <script src="../js/versiondocumento/gestion_version_documento.js"></script>
    <script src="../js/Docuarchi/JSOptionConsultaGabinete.js"></script>
    <script src="../js/Docuarchi/JSIndiceGabinete.js"></script>
    <link href="../generic_control/UploadFile.css" rel="stylesheet" />
    <link href="../Awesome/css/fontawesome.css" rel="stylesheet" />
    <link href="../Awesome/css/brands.css" rel="stylesheet" />
    <link href="../Awesome/css/solid.css" rel="stylesheet" />
    <script  src="../Awesome/js/all.js" type="text/javascript"></script>
    <script  src="../Awesome/js/brands.js" type="text/javascript"></script>
    <script  src="../Awesome/js/solid.js" type="text/javascript"></script>
    <script src="../Awesome/js/fontawesome.js" type="text/javascript"></script>
    <script src="../js/Docuarchi/WebFormDaPrincipal.js"></script>
</head>
<body  style="padding:0px 0px 0px 0px; margin:0px 0px 0px 0px">
    <form id="form1" runat="server" style=" margin-top:0px" >
        <asp:ScriptManager ID="ScriptManager1" runat="server">
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
                      auto_zise_docuarchi();
                      if (elment_postbak.id == "ButtonConsultar") {
                          auto_zise_popup_consulta();
                      }
                  }
                  catch (err) {
                      alert(err.message + " Funcion CheckStatus");
                  } finally {
                      progres_hiden('progres_bar');
                  }
              }

            </script>
        <div id="parent_workflow">
        <div id="menu_tulbar" style="height:5%; margin-top:0px" class="modal_content_back_inferior_superior">

        </div>
         <div id="bar_herramineta" style="height:5%" class="modal_content_back_inferior_superior">

        </div>
         <div id="area_trabjo" style="height:89%; background-color:white">
             <div id="error_content_general_docuarchi" style="position: relative; width: 100%"></div>
             <div id="div_gabinetes" style="float: left; background-color: white; width: 15%; height: 95%; border-style: ridge; border-bottom-width: 0.5px; border-left-width: 1px; border-right-width: 1px; border-top-width: 1px" class="p-1">
                 <div id="Panel_gabinetes" style="text-align: center">
                     <span id="Label_gabinetes" style="text-align: center" class="h6 mt-2">Gabinetes</span>
                     <br />
                     <select style="color: black; float: left" id="DropDownList_gabinetes" class="form-select form-control w-100">
                         <option></option>
                     </select>

                     <br />
                     <br />
                     <input id="btnSearhGabinete" type="button" value="Consultar" class="btn  btn-primary " />

                 </div>

             </div>
             <div id="div_carpetas" style="float:right;  background-color:white; width:84.5%; height:95%">

             </div>
             <div id="tol_pie" style=" float:right;   width:100%; height:5%" class="modal_content_back_inferior_superior">
                 <asp:Label ID="Label_estado" runat="server" Text=""></asp:Label>
             </div>
        </div>
        </div>
        <div class="modal fade modal_opacity" id="consulta_gabinete" role="dialog" data-backdrop="false">
            <div class="modal-dialog modal-fullscreen-sm-down">
                <div class="modal-content-fullscreen modal-content-fullscreen-no-border">
                    <div id="header_consulta_gabinete" style="width: 100%">
                        <div class="modal-header" style="max-height: 50px">
                            <h7 class="modal-title" id="label_titel_consulta" style="color: black"></h7>
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                        </div>
                    </div>
                    <div class="modal-body-fullscreen">
                        <div id="tool_bar_version_document" class=" w-100 navbar-expand-sm   row">
                            <!-- Barra de navegación -->
                            <nav class="navbar navbar-expand-lg navbar-light w-100">
                                <div class="container-fluid">
                                    <a id="gabinet_title" class="navbar-brand " href="#"></a>
                                    <div class="icon-container ">
                                        <!-- Menú hamburguesa -->
                                        <div role="button" class="navbar-icon d-flex align-items-center" aria-expanded="true" aria-label="Menú principal">
                                            <img focusable="false" id="svg-icon" src="../Docuarchi/imagenes/gabinete_azul.svg" alt="Gabinete" width="300" height="300"/>
                                           
                                        </div>
        
                                    </div>
                                    
                                    <div class="d-flex ms-auto">
                                        <div class="search-container">
                                            <input class="form-control search-input" id="textBox_buequeda_general_gabinete" type="search" placeholder="Buscar general..." aria-label="Buscar"/>
                                            <span id="BtnsearchgabineteGeneral"  class="search-icon"><i class="fas fa-search"></i></span>
                                        </div>
                                          <button id="b_opcion_busqueda" title="Busqueda avanzada" class="btn menu-button ms-3"  type="button"><i class="fas fa-search-plus"></i> Búsqueda avanzada</button>
                                    </div>
                                    <!-- Menú de opciones -->
                                    <div class="ms-3" id="toolbar">
                                        <button class="btn menu-button" title="Eliminar los documentos seleccionados" type="button" id="Button_activa_elimina_registro_documento"><i class="fas fa-trash-alt"></i> Eliminar</button>
                                        <button class="btn menu-button" title="Actualizar el índice de los documentos seleccionados" type="button" id="Button_active_update_index_bacth"><i class="fas fa-info"></i> Índice</button>
                                        <button class="btn menu-button" title="Descargar el resultado de la consulta" type="button" id="btn_dow_load_gabonete"><i class="fas fa-download"></i></button>
                                    </div>
                                </div>
                            </nav>   
                        </div>
                        <div class="col-12 modal-body-fullscreen mr-0 ml-0 pl-0 pr-0 pb-0 pt-0 float-sm-right" id="Contenedorderecho">
                            <div id="error_content_consulta_gabinete" style="position: relative; width: 100%"></div>
                            <div id="contenido_icon_boton_migra" class=" nav_botota_person_black nav_no_border_peroson d-none">
                               
                            </div>
                            <div id="contenido_table_boot_migracion" class="p-1 " style="height: 100%; width: 100%; position: relative; margin-top: 1px; background-color: white">
                                <table class="table_over " style="background-color: white"
                                    id="table_consulta_gabinete"
                                    data-pagination="true"
                                    data-page-list="[10, 25, 50, 100]"
                                    data-show-export="false"
                                    data-show-refresh="false"
                                    data-cache="true"
                                    data-toggle="table"
                                    data-id-field="ID"
                                    data-unique-id="ID"
                                    data-sortable="true" 
                                    data-sort-name="ID"  
                                    data-sort-order="asc" 
                                    data-click-to-select="true"
                                    data-search="false"
                                    data-export-excel-file-name="ReporteGabinete"
                                    data-locale="es-SP">
                                    <thead class="GridviewScrollHeader_line_blue_wite">
                                        
                                    </thead>
                                </table>
                            </div>
                             
                            <div id="contenido_footer_migracion"  class="d-none" style="width: 100%; position: relative; margin-top: 1px; background-color: white">
                                <div class="modal-header justify-content-start">
                                    <h6 style="color: black" id="state_migracion">Estado </h6>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div id="footer_consulta_gabinete" class="d-none">
                        <div class=" modal-footer" style="max-height: 73px">
                            <div class="row row-body-fullscreen">
                                <div class="col-6 justify-content-start">
                                    <h6 style="color: black" id="h_title_gabinete_image"></h6>
                                </div>
                                <div class="col-6 justify-content-end">
                                </div>
                            </div>
                        </div>
                    </div>

                </div>

            </div>
        </div>
        <!--modal visor migracion documento--->
       
        <div class="modal fade modal_opacity" id="modal_visor_migracion_documento" role="dialog" data-backdrop="false">
            <div class="modal-dialog modal-fullscreen-sm-down">
                <div class="modal-content-fullscreen modal-content-fullscreen-no-border">
                    <div id="header_modal_visor_migracion_documento" style="width: 100%">
                        <div class="modal-header" style="max-height:50px">
                            <h7 class="modal-title">Visor gabinete</h7>
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                        </div>
                    </div>      
                    <div class="modal-body-fullscreen" >        
                        <div class="row row-body-fullscreen">                  
                            <div class="col-12 modal-body-fullscreen pl-3">
                                <div class="conten_gred_border" id="content_view_visor_migracion_documento" style="height: 100%">
                                    <iframe id="Iframe_visor_migracion_documento" runat="server"  frameborder="0" width="100%" scrolling="no" height="100%"></iframe>
                                </div>
                            </div>
                        </div>         
                    </div>
                    <div id="error_content_visor_migracion_documento" style="position: relative; width:100%"></div>
                    <div id="footer_modal_visor_migracion_documento" >
                        <div class=" modal-footer" style="max-height:73px">
                            <div class="row row-body-fullscreen">
                                <div class="col-6 justify-content-start">
                                    <h7 style="color: black" id="h_title_gabinete_image_migracion"></h7>
                                </div>
                                <div class="col-6 nav col-md-6">
                                    <div class="nav-item active_">
                                        
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    
                </div>
                
            </div>
        </div>
       
        <!--Termina popout visor migracion documento-->
        <div class="modal fade modal_opacity_transparent" style="z-index: 100061" id="modal_option_search_gabinete" role="dialog" data-backdrop="false">
            <div class="modal-dialog modal-dialog-centered modal-custom-size-75 modal-50w">
                <div class="modal-content-fullscreen" id="modal_content_load_documento_006">
                    <div class="modal-header" id="modal_header_load_documento_006">
                         <h4>Búsqueda Avanzada</h4>
                        <h4 style="color: black" class="modal-title d-none">Adjunta documento</h4>
                        <button type="button" class="close" data-dismiss="modal">×</button>
                    </div>
                    <div class="modal-body-fullscreen modal-body" style="max-height: 75vh; overflow-y: auto;">
                        <div class="row_ row-body-fullscreen ">
                            <div class="col-12 pr-0" id="Contentizquierdo">
                                <div id="contenido_controles_consulta" style="width: 100%">
                                    <div id="div_consulta_gabinetes_migracion" class="container_gabinete_search" style="height: 100%">
                                        <!-- Aquí va el contenido dinámico -->
                                    </div>
                                   
                                </div>
                            </div>
                        </div>
                    </div>
                    <div id="error_modal_option_search_gabinete" style="position: relative; width: 100%" class="pl-4 pr-4"></div>
                    <div class="modal-footer justify-content-end" id="modal_footer_option_search_gabinete">
                            <div id="contenido_controles_buton_consulta" style="background-color: white; border-bottom: none" class="">
                            <input type="button" value="Cerrar" title="Cerrar el formulario" data-dismiss="modal" class="btn btn-secondary mr-2" />
                            <input type="button" id="Button_restore_gabinete" title="Limpiar los valores del formulario"  value="Restaurar" class="btn btn-secondary mr-2" />
                            <input type="button" id="Button_search_gabinete"  title="Consultar en el gabinete" value="Consultar" class="btn btn-primary" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!--Termina popout version de documentos-->
        <div id="ventana_consulta" style="clear:both">     
            <asp:Panel ID="Panel_consulta_documento" runat="server" Style="display: none; width: 100%; height: 100%" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopup_consulta_documento" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_consulta_documento"
                    PopupControlID="Panel_consulta_documento" CancelControlID="Buttoncerrar_consulta_documento">
                </asp:ModalPopupExtender>
                <div id="divcabecer_consulta_documento" class="modal_title_superior_ modal-header">
                    <h6 class="modal-title d-inline ml-1">Consulta gabinete</h6>
                    <button type="button" value="Buttoncerrar_consulta_documento" class="close da_event_captive">&times;</button>
                </div>
                <div id="Contenido_consulta_documento" style="color: black; background-color: #FFFFFF; height: 100%; width: 100%; overflow: hidden" class="modal_content_back">
                    <asp:UpdatePanel ID="UpdatePaneliframe_consulta_documento" runat="server" UpdateMode="Conditional" style="overflow: hidden">
                        <ContentTemplate>
                            <iframe id="ifimpre_consulta_documento_" runat="server" style="overflow-x: hidden; width:100%" scrolling="no" frameborder="0"></iframe>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div style="display: none; height: 1px">
                    <asp:Button ID="Button_consulta_documento" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="ButtonSalir_consulta_documento" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="Buttoncerrar_consulta_documento" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
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
