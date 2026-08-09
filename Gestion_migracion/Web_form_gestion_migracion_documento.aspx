<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Web_form_gestion_migracion_documento.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.Web_form_gestion_migracion_documento" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Gestión y migración de documentos</title>
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
    <script src="../js/Gestion_migracion/Web_form_gestion_migracion_documento.js" ></script>
    <script src="../js/java_general/general_code_java.js"></script>
    <script src="../js/java_general/general_config.js"></script>
    <script src="../js/java_general/general_control_java.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
      <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.11.0/umd/popper.min.js" type="text/javascript"></script>
    <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />   
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <script src="../js/java_general/row_multiple_gred.js"></script>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-table@1.23.2/dist/bootstrap-table.min.css"/>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap-table@1.23.2/dist/bootstrap-table.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/tableexport.jquery.plugin@1.29.0/tableExport.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/tableexport.jquery.plugin@1.29.0/libs/jsPDF/jspdf.umd.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap-table@1.23.2/dist/extensions/export/bootstrap-table-export.min.js"></script>
    <script src="../js/table_boo/table_boot_config.js"></script>
    <script src="../js/java_general/BootstrapTable.js"></script>
    <script  src="../js/versiondocumento/gestion_version_documento.js" ></script>
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
        
           <div id="Contentizquierdo" style="width: 28%; float: left" >
               <nav id="sidebar_" class=" bg-light_ pl-0 pr-0 " style="width: 100%">
                   <div id="contenido_titulo_controles_consulta" class="modal-header modal_title_superior bg-light_ p-2 " style="border-top-left-radius: initial; border-top-right-radius: initial">
                       <select style="color: black; float: left" id="option_gabinet" class="form-select form-control w-100" >
                           <option></option>
                       </select>
                       <h6 class=" mt-2 mb-2 ml-2 font-weight-light" id="pit_" style="color: white; float: left; font-family: 'Segoe UI'; display: none">Campos de busqueda </h6>
                   </div>
                   <div id="contenido_controles_consulta" style="width: 100%">
                       <div id="contenido_consulta_gabinetes_migracion" style="width: auto; height: auto; border-top: none; overflow: auto" class="modal_content_back modal-body">
                           <div id="div_consulta_gabinetes_migracion" style="height: 100%">
                           </div>
                       </div>
                   </div>
                   <div id="contenido_controles_buton_consulta" style="background-color: white; border-bottom: none" class="modal-header  justify-content-end t-5">
                       <input id="Button_restore_gabinete" type="button" value="Restaurar"  class="btn  btn-secondary mr-2"   />
                        <input id="Button_search_gabinete" type="button" value="Aceptar"  class="btn btn-success "   />
                        
                   </div>
               </nav>
           </div>     
            <div id="Contenedorderecho" class=" mr-0 ml-0 pl-0 pr-0 pb-0 pt-0  " style="width: 72%; float: right; background-color:white">           
                <div id="contenido_icon_boton_migra" class="navbar navbar-expand-sm nav_botota_person_black nav_no_border_peroson">
                    
                    <div class="nav col-md-6">
                         <div class="nav-item active_">
                            <a class="nav-link active ml-1 " id="Button_active_update_index_bacth" title="Actualiza multiplex indices" style="color: black" href="#" ><i style="color: black" class="fad fa-info"></i>
                                Indice
                            </a>
                        </div>
                        <div class="nav-item active_">
                            <a class="nav-link active ml-1 " id="Button_activa_migra_vincula_document" title="Vincula multiplex registros a expediente" style="color: black" href="#" ><i style="color: black" class="fad fa-folder-download"></i>
                                Vincula
                            </a>
                        </div> 
                        <div class="nav-item active_">
                           <a class="nav-link active ml-1 " id="Button_migra_remplaza_version_documento" title="Migra y remplaza multiplex documentos" style="color: black" href="#" ><i style="color: black" class="fal fal fa-clone"></i>
                               Migrar
                            </a>
                        </div>
                    </div>
                    <div class=" float-md-right col-md-6 float-sm-left">
                        <div class="input-group ">
                            <input id="textBox_buequeda_general_migra" type="text" class="form-control form-control-sm complex  border-left-1" placeholder="Busqueda general gabinete...." />
                            <div class="input-group-append">
                                <button class="btn btn-outline-secondary" id="Button_search_gabinete_general" type="button">
                                    <i class="fal fa-search"></i>
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
                <div id="contenido_table_boot_migracion" class="p-1 " style="height: 100%; width: 100%; position: relative; margin-top: 1px; background-color: white">
                    <table  class="table-not-border_person" style="background-color: white" 
                        id="table_consulta_migracion"
                        data-pagination="false"
                        data-page-list="[10, 25, 50, 100, all]"
                        data-show-export="false"
                        data-show-refresh="false"
                        data-cache="false"
                        data-toggle="table"
                        data-id-field="ID"
                        data-unique-id="ID"
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
        
        <!--modal versiones documento--->
        <div class="modal fade_person" id="modal_version_document" role="dialog" style="">
            <div class="modal-dialog modal-fullscreen-sm-down">
                <div class="modal-content-fullscreen">
                    <div id="header_modal_version_document" style="width: 100%">
                        <div class="modal-header" style="max-height:73px">
                            <h4 class="modal-title" style="color: black" >Versiones del documento</h4>
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                        </div>
                    </div>      
                    <div class="modal-body-fullscreen" >
                         <div id="tool_bar_version_document" class="navbar navbar-expand-sm  modal_content_no_back_inferior row">
                            <div class="nav col-md-6">
                                <div class="nav-item active_">
                                    <a class="nav-link active ml-1 " title="Adjunta nueva versión" id="Button_activa_adjunta_document_version" style="color:black" href="#">  <i style="color: black" class="fal fa-arrow-from-bottom"></i>
                                         Adjunta nueva versión
                                    </a>
                                </div>
                            </div>
                       </div>
                        <div class="row row-body-fullscreen">
                            <div class="col-4  modal-body-fullscreen_ pr-0">
                                <div class="conten_gred_border modal_bot_traf_table_" id="content_tabl_lista_version_documento" style="height:100%" >
                                    <table  style="background-color: white"
                                        id="tabl_lista_version_documento"
                                        data-unique-id="id_registro_version"       
                                        data-locale="es-SP">
                                        <thead class="GridviewScrollHeader_line_boot_black">
                                            <tr>
                                                <th data-field="ESTADO_ACTIVO_GABINETE" title="Versión activa en el gabinete" data-formatter="version_operateFormatter_asing" data-events="operateEventsVesrion" ></th> 
                                                <th data-field="operate" data-formatter="operate_list_version_document" data-events="operateEventsVesrion">OPCIONES</th>
                                                <th data-field="id_registro_version" data-visible="false">ID</th>
                                                <th data-field="id_version_doc" title="Versión del documento" data-visible="false">VERSION</th>
                                                <th data-field="fecha_registro_version" title="fecha versión del documemto">FECHA VERSION</th>    
                                            </tr>
                                        </thead>
                                    </table>
                                </div>
                            </div>
                            <div class="col-8 modal-body-fullscreen pl-0">
                                <div class="conten_gred_border" id="content_view_version_documento" style="height: 100%">
                                    <iframe id="Iframe_document_visor_version" runat="server" loading="lazy" frameborder="0" width="100%" scrolling="no" height="100%"></iframe>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div id="error_content_version_documento" style="position: relative; width:100%"></div>
                    <div id="footer_modal_version_document" >
                        <div class=" modal-footer" style="max-height:73px">
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
        <!--Termina popout version de documentos-->
        <!--Popup detalle version documento -->
         <div class="modal fade modal_opacity" id="modal_detalle_version_documento" role="dialog" >
             <div class="modal-dialog modal-dialog-scrollable">
                 <div class="modal-content" >
                     <div class="modal-header">
                        <h4 class="modal-title">Detalle versión documento</h4>
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>
                     <div class="modal-body">
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
        <!--Popup adjunta documento version -->
         <div class="modal fade modal_opacity" id="modal_adjunta_documeto_version_document" role="dialog">
             <div class="modal-dialog  modal-mediunscreen-sm-down ">
                 <div class="modal-content-fullscreen">
                     <div class="modal-header">
                        <h4 style="color: black" class="modal-title">Adjunta documento version</h4>
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>
                     <div class="modal-body-fullscreen modal-body">
                          <div class="row row-body-fullscreen">
                              <div class="p-4 w-100">
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
                         <div class="row border_ pt-2 w-100" id="content_pie_title_adjunta_documeto_version_document">
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
         <!--popout migracion--> 
        <div class="modal fade" id="modal_migracion" role="dialog">
            <div class="modal-dialog modal-fullscreen-sm-down">
                <div class="modal-content-fullscreen">
                    <div class="modal-header">
                        <h4 style="color: black" class="modal-title">Migrar y remplazar documento</h4>
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>
                    <div class="modal-body-fullscreen">
                        <div class="row row-body-fullscreen">
                            <div class="col-6  modal-body-fullscreen pr-0">              
                                <div class="conten_gred_border" style=" height:100%">
                                    <iframe id="IframeVisor_" runat="server"  frameborder="0" width="100%" scrolling="no" height="100%"></iframe>
                                </div>
                            </div>
                            <div class="col-6 modal-body-fullscreen pl-0" >
                                <div class="conten_gred_border" style="height:100%">
                                    <iframe id="Iframe_visor_pdf" runat="server"  frameborder="0" width="100%" scrolling="no" height="100%"></iframe>
                                </div>
                            </div>
                        </div>
                        
                    </div>
                    <div id="error_content_popup_migracion" style="position: relative; width:100%"></div>
                    <div class=" modal-footer">
                     <div class="row row-body-fullscreen">
                        <div class="col-6">
                            <div class="row">
                                <div class="col-2 justify-content-start">
                                    <div class="nav col-md-6">
                                        <div class="nav-item active_">
                                            <a class="nav-link active ml-1 " title="Actualiza carga del documento a migrar" id="Button_update_reload_docuent_migra" style="color: black" href="#" ><i style="color: black" class="fas fa-sync-alt"></i>
                                            </a>
                                        </div>
                                    </div>    
                                </div>
                                 <div  class="col-8 pt-2 justify-content-start ">
                                        <span style="color: black" id="h_gabibete_imagen"> </span>
                                 </div>
                               
                            </div>
                        </div>
                        <div class="col-6">
                            <div class="row">
                                <div class="col-9 justify-content-start">
                                    <div class="nav">
                                        <div class="nav-item active_">
                                            <a class="nav-link active ml-1 " title="Migrar documento de formato para remplazo de versión" id="Button_migra_formato_document" style="color: black" href="#"><i style="color: black" class="fas fa-file-import "></i>
                                                Migrar
                                            </a>
                                        </div>
                                        <div class="nav-item active_">
                                            <a class="nav-link active ml-1 " title="Adjunta documento para remplazo de versión" id="Button_activa_adjunta_document_remplazo" style="color: black" href="#" ><i style="color: black" class="fad fa-upload "></i>
                                                Adjuntar
                                            </a>
                                        </div>
                                        <div class="nav-item active_">
                                            <a class="nav-link active ml-1 " title="Digitalizar documento para remplazo de versión" id="Button_activa_digitaliza_document_remplazo" style="color: black" href="#" ><i style="color: black" class="fad fa-scanner-image "></i>
                                                Digitalizar
                                            </a>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-3 justify-content-end pr-1">
                                    <button type="button" id="Button_remplaza_version_documento" class="btn  btn-success"  title="Remplaza versión de documento">Remplazar</button>
                                </div>
                            </div>
                            
                        </div>
                     </div>   
                    </div>
                </div>

            </div>
        </div> 
        <!--Termina popout migracion-->
         <!--Popup adjunta documento para remplazo -->
         <div class="modal fade modal_opacity" id="modal_adjunta_documeto_migra" role="dialog">
             <div class="modal-dialog  modal-mediunscreen-sm-down ">
                 <div class="modal-content-fullscreen">
                     <div class="modal-header">
                        <h4 style="color: black" class="modal-title">Adjunta documento migracion</h4>
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>
                     <div class="modal-body-fullscreen modal-body">
                          <div class="row row-body-fullscreen">
                              <div class="p-4 w-100">
                                  <div class="row p-2" id="content_boton_adjunta_documeto_migra">
                                      <div class="col-12 p-0 pl-1">
                                          <div class="file-select " id="src-file">
                                              <input id="file_element_adjunta_documeto_migra" type="file" multiple="multiple" accept="" style="width: 100px; height: 40px" name="src-file" class="p-1" contente_file="ModalPopupExtender_sube_documento_adjunto" aria-label="Archivo" />
                                          </div>  
                                          <a id="save_file_element_adjunta_documeto_migra" title="Guardar todos los archivos" class="btn  btn-success ml-1" style="opacity: 0; color: white"><i style="color: white" class="fas fa-save "> </i> Guardar </a>
                                          <a id="delete_file_element_adjunta_documeto_migra" title="Elminar todos los archivos cargados" class="btn  btn-danger " style="opacity: 0; color: white"><i style="color: white" class="fal fa-trash-alt "> </i> Eliminar </a>
                                          <a id="cancel_file_element_adjunta_documeto_migra" title="Cancelar guardar archivos" class="btn  btn-warning" style="opacity: 0; color: white"><i style="color: white" class="fas fa-window-close "> </i> Cancelar </a>
                                      </div>
                                  </div>
                                  <div class="paren_element background_upload" id="conten_file_element_adjunta_documeto_migra" style="overflow: auto; height: 80%">

                                      <div id="content_drop_element_adjunta_documeto_migra" claas="">
                                      </div>
                                      <table id="table_file_element_adjunta_documeto_migra" class="table table-striped">
                                      </table>
                                  </div>
                                  
                              </div>
                          </div>
                    </div>
                     <div id="error_content_adjunta_documeto_migra" style="position: relative; width:100%"></div>
                     <div class=" modal-footer_">
                         <div class="row border_ pt-2 w-100" id="content_pie_title_adjunta_documeto_migra" >
                                      <div class="col-8 justify-content-start">
                                          <div class="row p-2">
                                              <div class="col-4 p-0">
                                                  <div>
                                                      <asp:Label ID="Label_progres_bar_file_element_adjunta_documeto_migra" runat="server" Text="" Style="font-family: Arial; text-align: center; font-size: 20px"></asp:Label>
                                                  </div>
                                                  <div id="pogres_file_element_contador_adjunta_documeto_migra" style="text-align: center; font-family: Arial; font-size: 14px">
                                                  </div>
                                                  <div id="pogres_file_element_porcent_adjunta_documeto_migra" style="text-align: center; font-family: Arial; font-size: 14px">
                                                  </div>
                                              </div>
                                              <div class="col-5 p-0">
                                                  <div>
                                                      <div id="myProgress_file_element_adjunta_documeto_migra">
                                                          <div id="myBar_file_element_adjunta_documeto_migra" class="file-select-bar"></div>
                                                      </div>
                                                  </div>
                                              </div>
                                              <div class="col-3 p-0 pl-3">
                                                  <p id="count_byte_file_element_adjunta_documeto_migra"></p>
                                              </div>
                                          </div>

                                      </div>
                                      <div class="col-4 justify-content-end pt-2">
                                          <p id="count_file_element_adjunta_documeto_migra" class="font-weight-light" style="float: right">Estado </p>
                                      </div>
                                  </div>
                     </div>
                 </div>
             </div>
         </div>
        <!--Trmina Popup adjunta documento para remplazo-->
        <!--Popup tipo tramite vinculacion -->
         <div class="modal fade modal_opacity" id="modal_tipo_tramite_vinculacion" role="dialog" >
             <div class="modal-dialog modal-dialog-scrollable">
                 <div class="modal-content" >
                     <div class="modal-header">
                        <h4 style="color: black" class="modal-title">Tipo tramite vinculación</h4>
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>
                     <div class="modal-body">
                          <div class="row ">
                              <div class="col-4">
                                  <span style="color: black">Proceso</span>
                              </div>
                              <div class="col-8">
                                  <select style="color: black" id="option_tramite_vincula" class="form-select form-control w-100" style="float: left">
				                             <option></option>
                                 </select>
                              </div>
                          </div>
                          
                           
                    </div>
                     <div id="error_tipo_tramite_vinculacion" style="position: relative; width:100%"></div>
                     <div class=" modal-footer">
                        <button type="button" id="Button_vincula_migra_documento" class="btn  btn-success"  title="Vincula documentos a expediente">Aceptar</button>
                     </div>
                 </div>
             </div>
         </div>     
        <!--Trmina Popup tipo tramite vinculacion-->
     
        <!--Popup actualiza indice batch -->
        <div class="modal fade modal_opacity" id="modal_actualiza_indice_batch_mig" role="dialog">
            <div class="modal-dialog  modal-mediunscreen-sm-down modal-dialog-scrollable">
                <div class="modal-content-fullscreen">
                    <div class="modal-header">
                        <h4 style="color: black" class="modal-title">Actualiza indice</h4>
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>
                    <div class="modal-body-fullscreen modal-body" style="overflow:auto">
                        <div id="div_actualiza_indice_batch_mig" style="height: 100%">
                        </div>
                    </div>
                    <div id="error_actualiza_indice_batch_mig" style="position: relative; width: 100%"></div>
                    <div class=" modal-footer">
                        <button type="button" id="Button_actualiza_indice_mig" class="btn  btn-success" title="Actualiza indice ">Aceptar</button>
                    </div>
                </div>
            </div>
        </div>   
         <!--Termina actualiza indice batch -->
        <!--modal visor migracion documento--->
        <div class="modal  fade_person " id="modal_visor_migracion_documento" role="dialog" style="">
            <div class="modal-dialog modal-fullscreen-sm-down">
                <div class="modal-content-fullscreen">
                    <div id="header_modal_visor_migracion_documento" style="width: 100%">
                        <div class="modal-header" style="max-height:73px">
                            <h4 class="modal-title">Visor de migración</h4>
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                        </div>
                    </div>      
                    <div class="modal-body-fullscreen" >        
                        <div class="row row-body-fullscreen">                  
                            <div class="col-12 modal-body-fullscreen pl-0">
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
                                    <h6 style="color: black" id="h_title_gabinete_image_migracion"></h6>
                                </div>
                                <div class="col-6 nav col-md-6">
                                    <div class="nav-item active_">
                                        <a class="nav-link active ml-1 " title="Cambiar tipología del documento" id="Button_activa_cambia_tipologia" style="color: black" href="#"><i style="color: black" class="fal fa-file-edit"></i>
                                            Tipología
                                        </a>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    
                </div>
                
            </div>
        </div>
        <!--Termina popout visor migracion documento-->
           <!--Popup cambia tipo documental -->
         <div class="modal fade modal_opacity" id="modal_cambiar_tipologia_documento" role="dialog" >
             <div class="modal-dialog modal-dialog-scrollable">
                 <div class="modal-content" >
                     <div class="modal-header">
                        <h4 style="color: black" class="modal-title">Cambia tipo documental</h4>
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>
                     <div class="modal-body">
                         <div class="row mt-1" id="row_tipo_tramite_migra">
                             <div class="col-4">
                                 <span style="color: black" class="font-weight-light">Proceso</span>
                             </div>
                             <div class="col-8">
                                 <select  id="option_cambia_tipo_tramite_migra" class="form-select form-control w-100" style="float: left">
                                     <option></option>
                                 </select>
                             </div>
                         </div>
                         <div class="row mt-1" id="row_cambia_tipo_serie_documental">
                             <div class="col-4">
                                 <span style="color: black" class="font-weight-light">Serie</span>
                             </div>
                             <div class="col-8">
                                 <select style="color: black" id="option_cambia_tipo_serie_documental" class="form-select form-control w-100" style="float: left">
                                     <option></option>
                                 </select>
                             </div>
                         </div>
                         <div class="row mt-1" id="row_cambia_tipo_sub_serie_documental">
                             <div class="col-4">
                                 <span style="color: black"  class="font-weight-light">Sub serie </span>
                             </div>
                             <div class="col-8">
                                 <select style="color: black" id="option_cambia_tipo_sub_serie_documental" class="form-select form-control w-100" style="float: left">
                                     <option></option>
                                 </select>
                             </div>
                         </div>
                         <div class="row " id="row_cambia_tipo_documental">
                             <div class="col-4">
                                 <span style="color: black" class="font-weight-light">Tipo documental  </span>
                             </div>
                             <div class="col-8">
                                 <select style="color: black; float: left" id="option_ambia_tipo_documental" class="form-select form-control w-100" >
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
        <!--modal scan document--->
        <div class="modal  fade_person " id="modal_scan_document_migracion" role="dialog" style="">
            <div class="modal-dialog modal-fullscreen-sm-down">
                <div class="modal-content-fullscreen">
                    <div id="header_mocdal_scan_document_migracion" style="width: 100%">
                        <div class="modal-header" style="max-height:73px">
                            <h4 class="modal-title">Digitalización</h4>
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                        </div>
                    </div>      
                    <div class="modal-body-fullscreen" >        
                        <div class="row row-body-fullscreen">                  
                            <div class="col-12 modal-body-fullscreen pl-0">
                                <div class="conten_gred_border" id="content_view_scan_document_migracion" style="height: 100%">
                                    <iframe id="Iframe_scan_document_migracion" class="pl-3" runat="server"  frameborder="0" width="100%" scrolling="no" height="100%"></iframe>
                                </div>
                            </div>
                        </div>         
                    </div>
                    <div id="error_content_scan_document_migracion" style="position: relative; width:100%"></div>
                    <div class="d-none">
                        <input id="HiddenIdFlujo" type="hidden" value="0" runat="server" />
                        <button class="d-none" id="save_document_scan"> </button>
                    </div>
                    
                </div>
                
            </div>
        </div>
        <!--termina modal scan document--->
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
