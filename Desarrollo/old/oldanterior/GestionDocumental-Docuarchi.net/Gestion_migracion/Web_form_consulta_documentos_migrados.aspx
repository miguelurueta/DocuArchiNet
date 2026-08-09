<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Web_form_consulta_documentos_migrados.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.Web_form_consulta_documentos_migrados" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Consulta migración</title>
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
    <script src="../js/java_general/general_code_java.js"></script>
    <script src="../js/java_general/general_config.js"></script>
    <script src="../js/java_general/general_control_java.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/popper.js@1.12.9/dist/umd/popper.min.js" type="text/javascript"></script> 
    <link href="../Styles/bootra-person.css" rel="stylesheet" /> 
    <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <script src="../js/java_general/row_multiple_gred.js"></script>
    
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-table@1.23.1/dist/bootstrap-table.min.css"/>
    <script src="https://cdn.jsdelivr.net/npm/tableexport.jquery.plugin@1.29.0/tableExport.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap-table@1.23.1/dist/bootstrap-table.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap-table@1.23.1/dist/bootstrap-table-locale-all.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap-table@1.23.1/dist/extensions/export/bootstrap-table-export.min.js"></script>
    <script src="../js/table_boo/table_boot_config.js"></script>
    <script src="../js/java_general/BootstrapTable.js"></script>
    <script src="../js/Gestion_migracion/Web_form_consulta_documentos_migrados.js" ></script>
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
</head>
<body style="background-color: white " class="pt-2">
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
        <div id="da_content_wraper" class="ml-0 mr-2  d-flex " style="padding-left: 1px; padding-right: 1px"> 
           <div id="Contentizquierdo" style="width: 28%; float: left">
               <nav id="sidebar_" class=" bg-light_ pl-0 pr-0 " style="width: 100%">
                   <div id="contenido_titulo_controles_consulta" class="modal-header modal_title_superior bg-light_ p-2 " style="border-top-left-radius: initial; border-top-right-radius: initial">
                       <span class="h5">Campos de consulta</span>
                       <h6 class=" mt-2 mb-2 ml-2 font-weight-light" id="pit_" style="color: white; float: left; font-family: 'Segoe UI'; display: none">Campos de busqueda </h6>
                       <a id="sidebarCollapse" class="close_ mr-1 d-none" style="float: right; color: black">&times;</a>
                   </div>
                   <div id="contenido_controles_consulta" style="width: 100%">
                       <div id="contenido_consulta_gabinetes_migracion" style="width: auto; height: auto; border-top: none; overflow: auto" class="modal_content_back modal-body">
                           <div id="div_consulta_gabinetes_migracion" style="height: 100%">
                           </div>
                       </div>
                   </div>
                   <div id="contenido_controles_buton_consulta" style="background-color: white; border-bottom: none" class="modal-header  justify-content-end">
                       <input id="Button_restore_consulta" type="button" value="Restaurar"  class="btn  btn-secondary mr-2"   />
                        <input id="Button_search_registro_migracion" type="button" value="Aceptar"  class="btn btn-success "   />      
                   </div>
               </nav>
           </div>      
            <div id="Contenedorderecho" class=" mr-0 ml-0 pl-1 pr-1 pb-0 pt-0  " style="width: 72%; float: right">    
                <div id="contenido_icon_boton_migra" class="navbar navbar-expand-sm nav_botota_person modal_content_no_back_inferior">
                    <button id="nav_togle_display" class="navbar-toggler" type="button" style="background-color: black" data-toggle="collapse" data-target="#navbarNavDropdown">
                        <span class="navbar-toggler-icon_"><i style="color: white" class="fad fa-th-list"></i></span>
                    </button>
                    <div class="nav col-md-6">
                         <div class="nav-item active_ d-none">
                            <a class="nav-link active ml-1  d-none" id="Button_active_update_index_bacth" title="Actualiza multiplex indices" style="color: black" href="#" ><i style="color: #0062cc" class="fal fa-info fa-lg"></i>
                                Indice
                            </a>
                        </div>
                        
                    </div>
                    <div class=" float-md-right col-md-6 float-sm-left">
                        <div class="input-group ">
                            <input id="textBox_buequeda_general_migra" type="text" class="form-control form-control-sm complex  border-left-1" placeholder="Busqueda general gabinete...." />
                            <div class="input-group-append">
                                <button class="btn btn-outline-secondary" id="Button_search_registro_migracion_lik" type="button">
                                    <i class="fal fa-search"></i>
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
                <div id="contenido_table_boot_migracion" class="p-1" style="height: 100%; width: 100%; position: relative; margin-top: 1px; background-color: white">
                    <table class="table-not-border_person" style="background-color: white" 
                        id="table_consulta_migracion"
                        data-pagination="false"
                        data-page-list="[10, 25, 50, 100, all]"
                        data-show-export="true"
                        data-show-refresh="false"
                        data-cache="false"
                        data-toggle="table"
                        data-id-field="id_registro_migracion"
                        data-unique-id="id_registro_migracion"
                        data-click-to-select="true"
                        data-search="false"
                        data-locale="es-SP">
                         <thead class="GridviewScrollHeader_line_blue_wite">
                        </thead>
                    </table>
                </div>
                <div id="error_content_migracion" style="position: relative; width:100%"></div>
                <div id="contenido_footer_migracion" style="width: 100%; position: relative; margin-top: 1px; background-color: white">
                    <div class="modal-header justify-content-start">
                        <h6 style="color: black" id="state_migracion">Estado </h6>

                    </div>

                </div>

            </div>
        </div>
         <!--popout documentos version migración--> 
        <div class="modal fade" id="modal_migracion" role="dialog">
            <div class="modal-dialog modal-fullscreen-sm-down">
                <div class="modal-content-fullscreen">
                    <div class="modal-header">
                        <h4 style="color: black" class="modal-title">Documentos registro migracion</h4>
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>
                    <div class="modal-body-fullscreen">
                        <div class="row row-body-fullscreen">
                            <div class="col-6  modal-body-fullscreen pr-0">              
                                <div class="conten_gred_border" style=" height:100%">
                                    <iframe id="IframeVisor_" runat="server" loading="lazy" frameborder="0" width="100%" scrolling="no" height="100%"></iframe>
                                </div>
                            </div>
                            <div class="col-6 modal-body-fullscreen pl-0" >
                                <div class="conten_gred_border" style="height:100%">
                                    <iframe id="Iframe_visor_pdf" runat="server" loading="lazy" frameborder="0" width="100%" scrolling="no" height="100%"></iframe>
                                </div>
                            </div>
                        </div>
                        
                    </div>
                    <div id="error_content_popup_migracion" style="position: relative; width:100%"></div>
                    <div class=" modal-footer">
                     <div class="row row-body-fullscreen">
                        <div class="col-6">
                            <div class="row">
                                <div class="col-4 justify-content-start">
                                    <div class="nav col-md-12">
                                        <div class="nav-item active_">
                                            <a class="nav-link active ml-1 " title="Actualiza carga del documento fuente" id="Button_update_reload_docuent_fuente" style="color: black" href="#" ><i style="color: black" class="fas fa-sync-alt"></i>
                                            </a>
                                        </div>
                                        <div class="nav-item active_">
                                            <a class="nav-link active ml-1 " title="Detalle versión documento fuente" id="Button_dtalle_docuent_fuente" style="color: black" href="#" ><i style="color: black" class="fal fa-list"></i>
                                            </a>
                                        </div>
                                    </div>    
                                </div>
                                 <div  class="col-8 pt-2  justify-content-end ">
                                        <span style="color: black" id="h_gabibete_imagen"> Documento fuente </span>
                                 </div>
                               
                            </div>
                        </div>
                        <div class="col-6">
                            <div class="row">
                                <div class="col-4 justify-content-start">
                                    <div class="nav">
                                        <div class="nav-item active_">
                                            <a class="nav-link active ml-1 " title="Actualiza carga del documento migrado" id="Button_update_reload_docuent_destino" style="color: black" href="#" ><i style="color: black" class="fas fa-sync-alt"></i>
                                            </a>
                                        </div>
                                        <div class="nav-item active_">
                                              <a class="nav-link active ml-1 " title="Detalle versión documento fuente" id="Button_dtalle_docuent_migrado" style="color: black" href="#" ><i style="color: black" class="fal fa-list"></i>
                                            </a>
                                        </div>
                                        
                                    </div>
                                </div>
                                <div class="col-8 pt-2  justify-content-end ">
                                    <span style="color: black" id="h_gabibete_imagen_dest">Documento migrado </span>
                                </div>
                            </div>
                            
                        </div>
                     </div>   
                    </div>
                </div>

            </div>
        </div> 
        <!--Termina popout migracion-->
         <!--Popup detalle version documento -->
         <div class="modal fade modal_opacity" id="modal_detalle_version_documento" role="dialog" >
             <div class="modal-dialog modal-dialog-scrollable">
                 <div class="modal-content" >
                     <div class="modal-header">
                        <h4 class="modal-title">Detalle versión documento</h4>
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>
                     <div class="modal-body" style="overflow:auto">
                          <div class="row ">
                              <div class="col-6">
                                  <span>Identificador de versión</span>
                              </div>
                              <div class="col-6">
                                  <span id="spn_id_registro_version"></span>
                              </div>
                          </div>
                          <div class="row ">
                              <div class="col-6">
                                  <span>Versión del documento</span>
                              </div>
                              <div class="col-6">
                                  <span id="spn_id_version_doc"></span>
                              </div>
                          </div>
                          <div class="row ">
                               <div class="col-6">
                                  <span>Fecha versión</span>
                              </div>
                              <div class="col-6">
                                  <span id="spn_fecha_registro_version"></span>
                              </div>
                          </div>
                          <div class="row ">
                              <div class="col-6">
                                  <span>Tipo archivo</span>
                              </div>
                              <div class="col-6">
                                  <span id="spn_tipo_archivo"></span>
                              </div>
                          </div>
                           <div class="row ">
                               <div class="col-6">
                                  <span>Tamaño archivo</span>
                              </div>
                              <div class="col-6">
                                  <span id="spn_peso_documento"></span>
                              </div>
                          </div>
                           <div class="row ">
                              <div class="col-6">
                                  <span>Paginas</span>
                              </div>
                              <div class="col-6">
                                  <span id="spn_paginas_document"></span>
                              </div>
                          </div>
                         
                    </div>
                     <div id="error_detalle_version_documento" style="position: relative; width:100%"></div>
                     <div class=" modal-footer">
                     </div>
                 </div>
             </div>
         </div>     
        <!--Trmina Popup detalle version documento-->
        <!--Popup detalle registro migracion -->
         <div class="modal fade modal_opacity" id="modal_detalle_registro_migracion" role="dialog" >
             <div class="modal-dialog modal-dialog-scrollable">
                 <div class="modal-content" >
                     <div class="modal-header">
                        <h4 class="modal-title">Detalle registro migración</h4>
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>
                     <div class="modal-body">
                          <div class="row border_general_blanco">
                              <div class="col-6">
                                  <span>Identificador de registro</span>
                              </div>
                              <div class="col-6">
                                  <span id="spn_id_registro_migracion"></span>
                              </div>
                          </div>
                          <div class="row border_general_blanco">
                              <div class="col-6">
                                  <span>Fecha registro</span>
                              </div>
                              <div class="col-6">
                                  <span id="spn_fecha_registro"></span>
                              </div>
                          </div>
                          <div class="row border_general_blanco">
                               <div class="col-6">
                                  <span>Propietario versión</span>
                              </div>
                              <div class="col-6">
                                  <span id="spn_user_loguin"></span>
                              </div>
                          </div>
                          <div class="row border_general_blanco">
                              <div class="col-6">
                                  <span>Gabinete</span>
                              </div>
                              <div class="col-6">
                                  <span id="spn_nombre_gabinete"></span>
                              </div>
                          </div>
                           <div class="row border_general_blanco">
                               <div class="col-6">
                                  <span>Id documento</span>
                              </div>
                              <div class="col-6">
                                  <span id="spn_id_imagen"></span>
                              </div>
                          </div>
                           <div class="row border_general_blanco">
                              <div class="col-6">
                                  <span>Aplica OCR</span>
                              </div>
                              <div class="col-6">
                                  <span id="spn_aplica_ocr"></span>
                              </div>
                          </div>
                         
                          <div class="row border_general_blanco">
		                                     <div class="col-6">
			  			           <span>Aplica compresión</span>
			  			       </div>
			  			       <div class="col-6">
			  			           <span id="spn_aplica_compresion"></span>
			  			       </div>
                          </div>
                           <div class="row border_general_blanco">
			  			       <div class="col-6">
			  			           <span>Versión documento</span>
			  			       </div>
			  			       <div class="col-6">
			  			           <span id="spn_version_pdf"></span>
			  			       </div>
                          </div>
                         <div class="row border_general_blanco">
			  			       <div class="col-6">
			  			           <span>Campo clave</span>
			  			       </div>
			  			       <div class="col-6">
			  			           <span id="spn_valor_campo_gabinete"></span>
			  			       </div>
                          </div>
                          <div class="row border_general_blanco">
			  			       <div class="col-6">
			  			           <span>Nombre archivo</span>
			  			       </div>
			  			       <div class="col-6">
			  			           <span id="spn_nombre_archivo"></span>
			  			       </div>
                          </div>
                        
                         <div class="row border_general_blanco">
			  			       <div class="col-6">
			  			           <span>Extensión de archivo migrado</span>
			  			       </div>
			  			       <div class="col-6">
			  			           <span id="spn_Extension_doc_migrado"></span>
			  			       </div>
                          </div>
                         <div class="row border_general_blanco">
			  			       <div class="col-6">
			  			           <span>Tamaño</span>
			  			       </div>
			  			       <div class="col-6">
			  			           <span id="spn_leng_file"></span>
			  			       </div>
                          </div>
                         <div class="row border_general_blanco">
			  			       <div class="col-6">
			  			           <span>Numero paginas documento fuente de migrado</span>
			  			       </div>
			  			       <div class="col-6">
			  			           <span id="spn_num_page_anterior"></span>
			  			       </div>
                          </div>
                          <div class="row border_general_blanco">
			  			       <div class="col-6">
			  			           <span>Numero paginas documento migrado</span>
			  			       </div>
			  			       <div class="col-6">
			  			           <span id="spn_num_page_nuevo"></span>
			  			       </div>
                          </div>
                          <div class="row border_general_blanco">
                              <div class="col-6">
                                  <span>Fecha eliminación documento fuente</span>
                              </div>
                              <div class="col-6">
                                  <span id="spn_fecha_registro_elimina_doc_fuente"></span>
                              </div>
                          </div>
                           <div class="row  border_general_blanco">
                              <div class="col-6">
                                  <span>Usuario elimina documento fuente</span>
                              </div>
                              <div class="col-6">
                                  <span id="spn_user_loguin_elimina_doc_fuente"></span>
                              </div>
                          </div>
                    </div>
                     <div id="error_detalle_registro_migracion" style="position: relative; width:100%"></div>
                     <div class=" modal-footer">
                     </div>
                 </div>
             </div>
         </div>  
        
         <!--Popup actualiza indice batch -->
        <div class="modal fade modal_opacity" id="modal_actualiza_indice_batch_mig" role="dialog">
            <div class="modal-dialog  modal-mediunscreen-sm-down modal-dialog-scrollable">
                <div class="modal-content-fullscreen">
                    <div class="modal-header">
                        <h4 style="color: black" class="modal-title">Indice documento</h4>
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>
                    <div class="modal-body-fullscreen modal-body" style="overflow:auto">
                        <div id="div_actualiza_indice_batch_mig" style="height: 100%">
                        </div>
                    </div>
                    <div id="error_actualiza_indice_batch_mig" style="position: relative; width: 100%"></div>
                    <div class=" modal-footer">
                       
                    </div>
                </div>
            </div>
        </div>   
         <!--Termina actualiza indice batch -->
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
