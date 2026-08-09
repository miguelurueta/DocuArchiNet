<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormDocumentosCompartidosRevision.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormDocumentosCompartidosRevision" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
  <title>Documentos compartidos</title>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/> 
     <script src="../js/ui/jquery-3.4.1.min.js"></script>
     <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />  
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <link href="../ccs/themes/base/jquery.ui.all.css" rel="stylesheet" type="text/css" />
    <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
    <script src="../js/Filtrar.js"></script>
      <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <script src="../js/gestion/WebFormDocumentosCompartidosRevision.js"></script>
    <script src="../js/validate_campos.js"></script>
    <script src="../js/java_general/general_code_java.js"></script>
    <script src="../js/java_general/general_config.js"></script>
    <script src="../js/java_general/general_control_java.js"></script>
     <script src="../js/java_general/JS_firma_digital.js"></script>
    <script  src="../Awesome/js/all.js"></script>
    <link href="../Awesome/css/fontawesome.css" rel="stylesheet">
    <link href="../Awesome/css/brands.css" rel="stylesheet">
    <link href="../Awesome/css/solid.css" rel="stylesheet">
    <script defer src="../Awesome/js/brands.js"></script>
    <script defer src="../Awesome/js/solid.js"></script>
    <script defer src="../Awesome/js/fontawesome.js"></script>
</head>
<body>
    <form id="form1" runat="server" onkeypress="return caracter_especial(event,this)">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="True"  AsyncPostBackTimeout="1200">
        </asp:ScriptManager>
        <script type="text/javascript" language="javascript">
            Sys.WebForms.PageRequestManager.getInstance().add_initializeRequest(InitializeRequest);
            Sys.Application.add_load(ApplicationLoadHandler)
            var elment_postbak;
            var value_element;
            function ApplicationLoadHandler(sender, args) {

                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(CheckStatus);

            }
            function InitializeRequest(sender, args) {
                //
                posicion_update_pogres('progres_bar');
                elment_postbak = args.get_postBackElement();
                var elmen = document.getElementById(elment_postbak.id)
                if (elmen.type == "button" || elmen.type == "submit") {
                    value_element = elmen.value;
                    elmen.value = "Espere..."
                    elmen.disabled = true;
                }
            }
            function CheckStatus(sender, args) {
                progres_hiden('progres_bar');
                //$("#Menu1").show();
                if (elment_postbak.type == "button" || elment_postbak.type == "submit") {
                    elment_postbak.value = value_element;
                    elment_postbak.disabled = false;
                }

                if (elment_postbak.id == "Button_ver_documentos_relacionados") {
                    auto_zise_popup_compartir_documento();
                }
                if (elment_postbak.id == "Button_lista_filtro") {
                    Lista_tareas_estados("data_grid_listado_solicitudes", "id_estado", "Red");
                    Lista_tareas_lectura("data_grid_listado_solicitudes", "id_estado_visto", "700");
                }

                if (elment_postbak.id == "Button_lista_filtro") {
                    auto_zise_popup_lista_solicitudes("1", "");
                }
                if (elment_postbak.id == "Button_lik_service_boton") {
                    auto_zise_popup_lista_solicitudes("1", "");
                }
                if (elment_postbak.id == "Button_eliminar_registro") {
                    if (document.getElementById("Hidden_result_eliminar").value == "YES") {
                        eliminar_fila_data_gred('data_grid_listado_solicitudes', 'hdnEmailID');
                        document.getElementById("Hidden_result_eliminar").value = "";
                    }
                }
                if (elment_postbak.id == "Button_ver_documentos_relacionados") {
                    if (document.getElementById("Hidden_resultado_ver_documento").value == "YES") {
                        actualiza_estado_boton_seleccion("data_grid_listado_solicitudes", "hdnEmailID", "100","1");
                        document.getElementById("Hidden_resultado_ver_documento").value == "";
                    }
                }
                if (elment_postbak.id == "Button_activa_visto") {
                    actualiza_estado_boton_seleccion("data_grid_listado_solicitudes", "hdnEmailID", "700", "0");
                }
              
                if (elment_postbak.id == "Button_ver_registro_colaboracion") {
                    auto_zise_popup_usuarios_relacionados();
                }
            }

            </script>
    
    <div id="div_contendor_principal">
            <nav id="div_titulo_listado"  style="width: 100%;  height:auto" class="navbar navbar-expand-sm nav_botota_person modal_content_no_back_inferior_">   
                 <button class="navbar-toggler" type="button" style="background-color: #6d7fcc" data-toggle="collapse" data-target="#navbarNavDropdown">
                      <span class="navbar-toggler-icon_"><i style="color: white" class="fad fa-th-list"></i></span>
                  </button>        
                  <div class="collapse navbar-collapse row" id="navbarNavDropdown">
                      <div class="navbar-nav col-md-8">
                          <a href="#" class="navbar-brand ml-3" style="color: #0062cc"> Documentos compartidos para revisión </a>
                      </div>
                      <div class=" float-md-right col-md-4 float-sm-left">
                          <div class="input-group ">
                              <button id="td-boton" class="btn btn-outline-secondary border-right-2 " title="Restaura la lista de documentos compartidos" style="border-top-right-radius: 0px; border-bottom-right-radius: 0px" onclick="preven_event_restor_search(event,this)" type="button">
                                 <i class="fal fa-long-arrow-left"></i>
                               </button>
                              <asp:TextBox ID="TextBox_busqueda" runat="server" class="form-control form-control-sm complex " placeholder="Busqueda...." onkeypress="preven_event_search_keypres_enter(event,this);"></asp:TextBox>
                              <div class="input-group-append">
                                  <button class="btn btn-outline-secondary" onclick="preven_event_search(event,this)" type="button">
                                      <i class="fal fa-search"></i>
                                  </button>
                              </div>
                          </div>
                      </div>
                  </div>            
                
            </nav>
        <div style="display:none">
             <asp:UpdatePanel ID="UpdatePanel_busqueda" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                    <ContentTemplate>            
                        <asp:ImageButton ID="ImageButton_buscar" runat="server" Style="margin-top: 4px; float: right; margin-right: 4px; display:none" ImageUrl="../radicador/imagenes/cbxs0-vnnbp.png" />                    
                    </ContentTemplate>
                </asp:UpdatePanel>
        </div>
        <div id="error_content_compartido" style="position: relative; width:100%"></div>
            <div id="div_contendor_filtro_listado" class="modal_content_no_back_inferior_  container-fluid" style="width:100%; height:auto" >   
                <asp:Label ID="Label_anunciado_filtro" runat="server" Text="Todos los compartidos" style="font-size:12px; font-family:Arial; float:left; margin-top:5px; float:right; margin-right:2px"></asp:Label> 
                 <div id="div_filtro__fil" class="dropdown_filter" >
                    <button id="boton__filtro_ver" onclick="myFunction(event,this)" class="dropbtn_filter">Filtrar</button>
                    <div id="myDropdown" class="dropdown-content_filter" onkeyup="hiden_keys(event, thiss)">
                        <input type="text" placeholder="Search.." id="myInput" onkeyup="filterFunction()">
                        <a href="#about" onclick="event_elemento(event,'1',this)" class="e_list_marc">Todos los compartidos</a>
                        <a href="#base" onclick="event_elemento(event,'2',this)" class="e_list_marc">Informativo</a>
                        <a href="#blog" onclick="event_elemento(event,'3',this)" class="e_list_marc">Para colaboración</a>  
                        <a href="#blog" onclick="event_elemento(event,'4',this)" class="e_list_marc">Para aprobación</a> 
                        <a href="#blog" onclick="event_elemento(event,'5',this)" class="e_list_marc">Eliminados</a>      
                    </div>
                </div>               
            </div>          
            <div style="display:none">
                <asp:UpdatePanel ID="UpdatePanel_menu_boton" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                            <ContentTemplate>
                                 <asp:Button ID="Button_lik_service_boton" runat="server" Text="Button"  />
                                <asp:Button ID="Button_link_acualiza_lista" runat="server" Text="Button"  />
                                <input id="Hidden_lik_service_boton" type="hidden" value="0" runat="server">
                                </ContentTemplate>
                    </asp:UpdatePanel>
            </div>
        
            <div id="Contenedorgrid_listado_solicitud" style="width: 100%; position: inherit; height:auto; overflow:auto" class="container-fluid">             
                <asp:UpdatePanel ID="UpdateGeneral" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <input id="hdnEmailID" type="hidden" value="0" runat="server">
                        <input id="hdnEmailID_VAL" type="hidden" value="0" runat="server">
                        <input id="HiddenEmailconsulta" type="hidden" value="" runat="server">
                        <input id="Hidden_control_lista" type="hidden" value="" runat="server">                             
                            <asp:GridView ID="data_grid_listado_solicitudes" runat="server" AllowSorting="true"  AllowPaging="true"  EnableViewState="true"
                                  PageSize="7" PagerSettings-Position="Top"  style="width:100%; font-family: Segoe UI; font-size:16px"
                                    AutoGenerateSelectButton="False" CssClass="table  font-weight-light" GridLines="None" >
                                    <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                    <HeaderStyle CssClass="GridviewScrollHeader_line_boot" BorderStyle="None" />
                                    <PagerStyle CssClass="pagination-ys" />
                                    <Columns>
                                        <asp:BoundField HeaderText="OPCIONES   " />
                                    </Columns>
                                </asp:GridView>                    
                    </ContentTemplate>
                    <Triggers>
                    </Triggers>
                </asp:UpdatePanel>
                
            </div>
        <div id="contenido_titulo_listado_solicitudes"  class="container-fluid mt-1">
              <asp:UpdatePanel ID="UpdatePanel_title" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>     
                           <asp:Label ID="Label_titulo_listado_solicitudes" runat="server" CssClass=" h6 font-weight-light float-left" Style="font-family: Segoe UI; font-size:16px">Resultados busqueda</asp:Label>
                            <asp:Label ID="Label_estado" runat="server" CssClass="h6 font-weight-light " Style="font-family: Segoe UI ; float:right"></asp:Label>                        
                        </ContentTemplate>
                    </asp:UpdatePanel>
             </div>
            <div id="contenedor_opciones_solictitud_general" style="width: 100%; text-align: left; font-family: Arial; background-color: #E7EDF5;margin-top: 1px; border-color: #b0c4de; border-style: ridge; border-width: 1px; display:none">
                <asp:UpdatePanel ID="update_botonoes_opciones_solicitud_general" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <input id="Hidden_ruta_archivo" type="hidden" value="" runat="server"/>
                        <input id="Hidden_solicitud_compartido" type="hidden" value="-1" runat="server"/>
                        <input id="Hidden_result_eliminar" type="hidden" value="" runat="server"/>
                        <input id="Hidden_resultado_ver_documento" type="hidden" value="" runat="server"/>
                        <input id="Hiddenxxxxxx" type="hidden" value="" runat="server"/>
                        <asp:Button ID="Button_ver_documento_solicitud" runat="server" Enabled="false" Text="Ver documento solicitud" ToolTip="Visualiza el documento de solicitud de aprobación" CssClass="boton" Style="display:none"  />
                        <asp:Button ID="Button_ver_documento_respuesta_solicitud" runat="server"  Text="Ver documento respuesta" ToolTip="Visualiza el documento respuesta de solicitud de aprobación" CssClass="boton" Style="display:none" />
                        <asp:Button ID="Button_activa_desicion_aprobacion" runat="server" Text="Gestionar solicitud"  ToolTip="Agrega una nueva solicitud de aprobación" CssClass="boton" Style="display:none" />
                        <asp:Button ID="Button_lista_filtro" runat="server"  Text="Gestionar solicitud"  CssClass="boton" Style="display:none" />
                        <asp:Button ID="Button_ver_documentos_relacionados" runat="server"  Text="Ver documentos" ToolTip="Lista los documentos compartidos relacionados con el registro seleccionado"  CssClass="boton"  />
                        <asp:Button ID="Button_eliminar_registro" runat="server" Text="Eliminar"   CssClass="boton"  ToolTip="Elimina el registro seleccionado" OnClientClick="ConfirmMensajeGeneral('Desea eliminar el registro','Hiddenxxxxxx')"   />
                        <asp:Button ID="Button_activa_visto" runat="server" Text="Eliminar"   CssClass="boton"  style="display:none"   />
                        <asp:Button ID="Button_ver_registro_colaboracion" runat="server" Text="Ver registro colaboración"  CssClass="boton"  ToolTip="Ver los registro de colaboración asociados al registro"   />
                    </ContentTemplate>
                </asp:UpdatePanel>

            </div>
        </div>
        <!--compartir documento-->
          <div id="autoriza_compartir_documento">
            <asp:Panel ID="Panel_autoriza_compartir_documento" runat="server" Style="display:none; width: 98%; height: auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_autoriza_compartir_documento" runat="server"  TargetControlID="ButtonSalir_autoriza_compartir_documento" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_autoriza_compartir_documento" PopupControlID="Panel_autoriza_compartir_documento" ></asp:ModalPopupExtender>
                <div id="divcabecer2_autoriza_compartir_documento"  class="modal_title_superior_ modal-header pt-1 pb-1">
                    <h6 class="modal-title d-inline ">Documentos compartidos</h6>
                      <button type="button" value="Button_cerrar_autoriza_compartir_documento" class="close da_event_captive ">&times;</button>   
                </div>
                <div id="contenido_procesa_autoriza_compartir_documento" style="background-color: white; width: 99%; height: 99%; border-top:none" class="modal_content_back">
                        <asp:UpdatePanel ID="UpdatePanel_autoriza_compartir_documento" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                              <iframe id="Iframe_compartir_documento_" runat="server" frameborder="0"  style="width:100%; height:100%; overflow:hidden"></iframe>                           
                            </ContentTemplate>
                        </asp:UpdatePanel>
                         
                </div>
                <div style="display:none; height:1px">
                    <asp:Button ID="Button_autoriza_compartir_documento" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                    <asp:Button ID="ButtonSalir_autoriza_compartir_documento" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                   <asp:Button ID="Button_cerrar_autoriza_compartir_documento" runat="Server" Text="" CssClass="invisible" />
                </div>
                
            </asp:Panel>
        </div>
        
         <!--Popup decisión solicitud-->
            <asp:Panel ID="Panel_desicion_solicitud" runat="server" Style="text-align: left; display:none; width:70%; height:auto; border-top:none; overflow:auto" CssClass="modal_content_general_" >
                <asp:ModalPopupExtender ID="ModalPopupExtender_desicion_solicitud" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_desicion_solicitud"
                    PopupControlID="Panel_desicion_solicitud" CancelControlID="ButtonCerrar_desicion_solicitud">
                </asp:ModalPopupExtender>
                <div id="modal_content_desicion_solicitud" class="modal-content">
                    <div id="divcabecer_desicion_documento" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title d-inline ">Decisión de la solicitud</h6>
                        <button type="button" value="ButtonCerrar_desicion_solicitud" class="close da_event_captive ">&times;</button>           
                    </div>
                    <div id="Cotenedor_desicion_solicitud" style="background-color: #FFFFFF; height: 100%; width: auto; border-top:none; overflow:auto" class="modal_content_back modal-body">
                        <div class="row">
                            <div class="col-4">
                                <span class="h6 font-weight-light" style="font-family:'Segoe UI'"> Decisión de la solicitud (*) </span>
                            </div>
                            <div class="col-8">
                                <asp:DropDownList ID="DropDownList_estado_aprobacion" CssClass="form-control" runat="server" ></asp:DropDownList>
                            </div>
                        </div>
                        <div class="row mt-1">
                            <div class="col-4">
                                <span class="h6 font-weight-light" style="font-family:'Segoe UI'"> Nota de decisión (*) </span>
                            </div>
                            <div class="col-8">
                                <asp:TextBox ID="TextBox_nota_solicitud" runat="server" TextMode="MultiLine"  CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div style="display:none; height:1px">
                              <asp:Button ID="ButtonSalir_desicion_solicitud" runat="server" Text="" Height="1px" Width="1px" Style="display: none" />
                              <asp:Button ID="ButtonCerrar_desicion_solicitud" runat="Server" Text="" Style="display: none"/> 
                        </div>
                       
                    </div>
                     <div id="error_content_decision" style="position: relative; width:100%"></div>
                     <div class="modal-footer justify-content-end" id="modal-footer_desicion_solicitud"> 
                         <button type="button" id="ButtonGuadarRegistroDesicion" class="btn btn-success">Aceptar</button>     
                     </div>
                </div>            
            </asp:Panel>
        <asp:Panel ID="Panel_usu_rel_solicitud" runat="server" Style="display:none; width: 70%; height: 100%" CssClass="modal_content_general_">
            <asp:ModalPopupExtender ID="ModalPopupExtender_edition_usu_rel_solicitud" runat="server" 
                TargetControlID="ButtonSalir_usu_rel_solicitud" BackgroundCssClass="FondoAplicacion"
                CancelControlID="Button_cerrar_usu_rel_solicitud" PopupControlID="Panel_usu_rel_solicitud">
            </asp:ModalPopupExtender>
            <div id="modal_content_usu_rel_solicitud" class="modal-content"> 
                <div id="diver_cabcera_user_rel" class="modal_title_superior_ modal-header">
                     <h6 class="modal-title d-inline ">Usuarios relacionados</h6>
                    <button type="button" value="Button_cerrar_usu_rel_solicitud" class="close da_event_captive ">&times;</button>
                 </div>
            <div id="contenido_procesa_usu_rel_solicitud" style="background-color: white; width: 100%; height: 100%; border-top: none" class="modal_content_back modal-body_ pl-3 pr-3">               
                <asp:UpdatePanel ID="UpdateGeneral_documentos" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Button ID="Button_listar_usuarios_relacionados_solicitud" runat="server" Text="Button" Style="display: none" />
                        <input id="hdnEmailID_documentos" type="hidden" value="0" runat="server">
                        <input id="hdnEmailID_VAL_documentos" type="hidden" value="0" runat="server">
                        <input id="Hidden_id_usuarios_sel" type="hidden" value="0" runat="server">
                        <input id="HiddenEmailconsulta_documentos" type="hidden" value="" runat="server">
                        <div id="contenido_titulo_val_radicacion_documentos" class="mb-2">
                            <asp:Label ID="Label_estado_documentos" runat="server" ForeColor="Black" Font-Size="9px" Style="float: right; display: none"></asp:Label>
                            <asp:Label ID="titulo_label_expedientes_documentos" runat="server" class="h6 font-weight-light">Resultados busqueda</asp:Label>
                        </div>
                        <div id="content_data_grid" class="conten_gred_border_" style="overflow: auto; width: 100%">
                               <asp:GridView ID="data_grid_documentos" runat="server" Style="position: inherit; width: 100%; font-size: 14px; font-family: 'Segoe UI Emoji'"
                                    AutoGenerateSelectButton="False" CssClass="filtrar table  font-weight-light" GridLines="None"
                                    Font-Size="12px" EnableViewState="true">
                                    <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                    <HeaderStyle CssClass="GridviewScrollHeader_line_boot" />
                                    <Columns>
                                    </Columns>
                                </asp:GridView>              
                        </div>

                    </ContentTemplate>

                    <Triggers>
                    </Triggers>
                </asp:UpdatePanel>       
                <div style="display: none; height: 1px">
                    <asp:Button ID="Button_usu_rel_solicitud" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                    <asp:Button ID="ButtonSalir_usu_rel_solicitud" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                    <asp:Button ID="Button_cerrar_usu_rel_solicitud" runat="Server" Text="X" CssClass="modal_boton_hiden" />
                </div>
               
            </div>
                 <div id="content_boton_user_rel" class="modal-footer justify-content-end">
                    <asp:UpdatePanel ID="UpdatePanel_expediente_seleccionado" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <input id="Hidden_colum_header" type="hidden" value="" runat="server">
                            <asp:Button ID="Button_export_lista" runat="server" Text="Exportar lista" ToolTip="Exportar lista" CssClass="btn btn-success"  OnClientClick="activa_export_lista('Hidden_colum_header','')" />
                        </ContentTemplate>
                    </asp:UpdatePanel>

                </div>
            </div>
           
        </asp:Panel>
        <asp:UpdatePanel ID="UpdatePanel_iframe_reporte" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <input id="Hidden_ruta_archivo_" type="hidden" value="" runat="server"/>
                <iframe runat="server" style="float: left" id="ifmExcel_reporte_" width="0" height="0" marginheight="0" marginwidth="0"
                    frameborder="0" />
                <asp:Button ID="Button_export_lista_event" runat="server" Text="Exportar" Style="margin-top: 5px; display: none" CssClass="boton_azul" />
            </ContentTemplate>

        </asp:UpdatePanel>  
        <div id="tol_pie" style="float: right; background-color: #E7EDF5; width: 100%; height: 3%; border-style: ridge; border-bottom-width: 0.5px; border-left-width: 1px; border-right-width: 1px; border-top-width: 1px; text-align: center; display: none">
            <asp:Label ID="Label2" runat="server" Text="Estado" Style="font-family: Arial; font-size: 11px"></asp:Label>
            <asp:UpdatePanel ID="updatapanel_iframe" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <iframe runat="server" style="float: left" id="ifmExcel_" width="0" height="0" marginheight="0" marginwidth="0"
                        frameborder="0" />
                </ContentTemplate>

            </asp:UpdatePanel>
        </div>
   

        <!--mensaje_progreso evento-->
        <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 200px">
            <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
            Processing ...
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
</body>
</html>
