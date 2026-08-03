<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebForm_interface_consulta_historico.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebForm_interface_consulta_historico" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Historico de correspondencia</title>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
     <link href="../ccs/themes/base/jquery.ui.all.css" rel="stylesheet" type="text/css" />
     <script src="../js/ui/jquery-3.4.1.min.js"></script>
    <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
     <link href="../Styles/bootra-person.css" rel="stylesheet" />
     <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
    <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
      <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <script src="../js/gestion_correspondencia/WebForm_interface_consulta_historico.js"></script>
    <script src="../js/java_general/general_code_java.js"></script>
      <script  src="../Awesome/js/all.js"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet"/>
     <link href="../Awesome/css/brands.css" rel="stylesheet"/>
     <link href="../Awesome/css/solid.css" rel="stylesheet"/>
     <script  src="../Awesome/js/brands.js"></script>
     <script  src="../Awesome/js/solid.js"></script>
     <script  src="../Awesome/js/fontawesome.js"></script>   
     <script src="../js/ScrollableGridViewPlugin_ASP.NetAJAXmin.js" type="text/javascript"></script>
     <script src="../Fixed-Header-Table-master/gridviewScroll.min.js"></script>
    <script src="../js/Filtrar.js"></script>
      <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <script src="../js/validate_campos.js"></script>
    <link href="../bootstrap/date-piker/gijgo.min.css" rel="stylesheet" />
    <script src="../bootstrap/date-piker/gijgo.min.js"></script>
</head>
<body style="margin-top:0px; overflow:hidden">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="True">
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
                try {


                    if (elment_postbak.type == "button" || elment_postbak.type == "submit") {
                        elment_postbak.value = value_element;
                        elment_postbak.disabled = false;
                    }
                   
                    
                    if (elment_postbak.id == "Button_visor_emergente") {
                        auto_zise_popup_visor_externo();
                    }
                    if (elment_postbak.id == "Button_confirma_reversar") {
                        if (document.getElementById("Hidden_con_ref").value == "YES") {
                            document.getElementById('Hidden_estado_tramite').value = "Por tramitar";
                            actualiza_estado_tramite();
                            document.getElementById("Hidden_con_ref").value = "";
                        }
                    }
                        
                    Actualiza_cantidad_barr_estado();
                }
                catch (err) {
                    alert(" Funcion CheckStatus asincrona workflow.aspx" + err.message);
                }
                finally {
                    progres_hiden('progres_bar');
                }
            }
            </script>
    <div id="div_contendor_principal" >
              <nav id="navar_barra" class="navbar navbar-expand-sm nav_botota_person modal_content_no_back_inferior" style="padding-top:initial">  
                  <button type="button"  style=" background-color:#6d7fcc" class=" navbar-toggler" data-toggle="collapse" data-target="#_histori_drop">
                       <span class="navbar-toggler-icon_"><i style="color:white" class="fad fa-th-list"></i></span>
                  </button>    
                <div class="collapse navbar-collapse row" id="_histori_drop">
                    
                    <div class="navbar-nav col-md-8 ">
                        <a href="#" class="navbar-brand ml-2"  style="color:#0062cc" > Histórico de tramites </a>   
                        <ul class="navbar-nav  ml-2" >
                          <li class="nav-item active active_">
                              <a  class="nav-link"  title="Descarga lista de resultados"  href="#" onclick="activa_boton_client_server('ImageButton_guarda_lista');"><i   style="margin-left: 1px; margin-top: 7px; color:#0062cc; font-size:18px  " class="fad fa-arrow-to-bottom "></i>  Descargar resultados  </a>
                          </li>   
                          <li class="nav-item active active_">
                              <a  class="nav-link"  title="Consultar por fechas de asignación y culminación"  href="#" onclick="activa_boton_client_server('ImageButton_filter');"><i   style="margin-left: 1px; margin-top: 8px; color:#0062cc; font-size:17px  " class="fad fa-calendar  fa-xs"></i>  Consultar por fechas  </a>
                          </li>
                        </ul>
                    </div>
                     <div class=" float-md-right col-md-4 float-sm-left">
                      <div class="input-group ">
                          <asp:TextBox ID="auto_complex" runat="server" class="form-control form-control-sm complex " placeholder="Busqueda...." onkeypress="preven_event_search_keypres_enter(event,this);"></asp:TextBox>
                          <div class="input-group-append">
                              <button class="btn btn-outline-secondary" onclick="preven_event_search(event,this)" type="button">
                                  <i class="fal fa-search"></i>
                              </button>
                          </div>
                      </div>
                  </div>   
                </div>                        
            </nav>
           
            <div id="Menutol" class="navbar_gray" style="overflow:auto;background-color:#f5f5f5;display:none">
              <asp:UpdatePanel ID="updatemenu" runat="server" UpdateMode="Conditional">
                  <ContentTemplate>    
                      <asp:UpdatePanel ID="UpdatePanel_busqueda" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                          <ContentTemplate>
                              <div class="box">
                                  <asp:ImageButton ID="ImageButton_filter" runat="server" ToolTip="Filtro avanzado" Style="margin-top: 4px; margin-right: 4px; margin-top: 5px; display: none" ImageUrl="../workflow/imageneswf/filter-solid.png" />
                                  <asp:ImageButton ID="ImageButton_guarda_lista" runat="server" ToolTip="Descargar lista de resultado" OnClientClick="activa_export_lista('Hidden_colum_header','data_grid_listado_solicitudes')" Style="margin-right: 4px; float: right; height: 20px; margin-top: 5px; display: none" ImageUrl="../workflow/imageneswf/download-solid.png" />
                                  <asp:ImageButton ID="ImageButton_buscar" runat="server" Style="margin-top: 4px; margin-right: 4px; margin-top: 5px; display: none" ImageUrl="../radicador/imagenes/cbxs0-vnnbp.png" />
                              </div>              
                          </ContentTemplate>
                      </asp:UpdatePanel>
                      <asp:Button ID="Button_visor_emergente" runat="server" Text="Button" style="display:none" />
                       <input id="Hidden_estado_tareas_pendiente" type="hidden" value="NO" runat="server">
                       <input id="Hidden_estado_anotacion" type="hidden" value="NO" runat="server">
                      <input id="Hidden_estado_pendiente_aprobacion" type="hidden" value="NO" runat="server">
                       <input id="Hidden_activa_popup" type="hidden" value="" runat="server">
                      <input id="Hidden_lista_ruta_flujo" type="hidden" value="" runat="server">
                      <input id="Hidden_vi_reasigna" type="hidden" value="" runat="server">
                      <input id="Hidden_men_result" type="hidden" value="" runat="server">
                  </ContentTemplate>
              </asp:UpdatePanel>
              
                <asp:UpdatePanel ID="UpdatePanel_menu_var_event" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <input id="Hidden_menu_var_event_dive" type="hidden" value="" runat="server"/>
                        <asp:Button ID="Button_me_active_men_dive" runat="server" Text="" Style="display:none; width:1px; height:1px" />
                    </ContentTemplate>
                </asp:UpdatePanel>
          </div>
            <div id="div_contendor_filtro_listado" class="modal_content_no_back_inferior" style="width:100%; height:auto" >   
                <asp:Label ID="Label_anunciado_filtro"  runat="server" Text="Todos" CssClass="p2" style="font-size:12px; font-family:Arial; float:left; margin-top:5px; float:right; margin-right:2px; margin-right: 15px; color:#0062cc"></asp:Label> 
                 <div id="div_filtro__fil" class="dropdown_filter" style="margin-left:10px"  >
                    <button id="boton__filtro_ver" onclick="myFunction(event,this)" data-toggle="dropdown" class="dropbtn_filter dropdown-toggle " style="color:#0062cc"> Filtrar
                          
                    </button>
                    <div id="myDropdown" class="dropdown-content_filter" onkeyup="hiden_keys(event, thiss)">
                        <input type="text" placeholder="Search.." id="myInput" onkeyup="filterFunction()">
                        <a href="#about" onclick="event_elemento(event,'1',this)" class="e_list_marc">Todos</a>
                        <a href="#base" onclick="event_elemento(event,'2',this)" class="e_list_marc">Por tramitar</a>
                        <a href="#blog" onclick="event_elemento(event,'3',this)" class="e_list_marc">En tramite</a>  
                        <a href="#blog" onclick="event_elemento(event,'4',this)" class="e_list_marc">Tramitado</a> 
                        <a href="#blog" onclick="event_elemento(event,'10',this)" class="e_list_marc">Tramitado archivado</a> 
                        <a href="#blog" onclick="event_elemento(event,'5',this)" class="e_list_marc">Solicitud por aprobación</a> 
                        <a href="#blog" onclick="event_elemento(event,'6',this)" class="e_list_marc">Solicitud aprobada</a> 
                        <a href="#blog" onclick="event_elemento(event,'7',this)" class="e_list_marc">Solicitud desaprobada</a>  
                        <a href="#blog" onclick="event_elemento(event,'8',this)" class="e_list_marc">Solicitud archivada</a>  
                        <a href="#blog" onclick="event_elemento(event,'9',this)" class="e_list_marc">Solicitud anulada</a>   
                    </div>
                    
                </div>
             
            </div>
           
            <div style="display:none">
                <asp:UpdatePanel ID="UpdatePanel_menu_boton" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                            <ContentTemplate>
                                 <asp:Button ID="Button_lik_service_boton" runat="server" Text="Button"  />
                                <input id="Hidden_lik_service_boton" type="hidden" value="0" runat="server">
                                </ContentTemplate>
                    </asp:UpdatePanel>
            </div>
        
            <div id="Contenedorgrid_listado_solicitud" style="width: auto; position: inherit; left: 0px; top: 0px;margin-top: 1px;margin-right:10px; margin-left:10px">              
                <asp:UpdatePanel ID="UpdateGeneral" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <input id="hdnEmailID" type="hidden" value="0" runat="server">
                        <input id="hdnEmailID_VAL" type="hidden" value="0" runat="server">
                        <input id="Hidden_content" type="hidden" value= 0 runat="server">
                        <input id="HiddenEmailconsulta" type="hidden" value="" runat="server">
                        <input id="Hidden_control_lista" type="hidden" value="" runat="server">  
                        <div id="content_grid" class="border_general_blanco_" style="width: 100%">
                             <asp:Panel ID="Panel_principal" runat="server"  ScrollBars="Auto"
                            Width="100%" Style="">
                            <asp:GridView ID="data_grid_listado_solicitudes" runat="server" AllowSorting="true"  AllowPaging="true"  EnableViewState="true"
                                  PageSize="7" PagerSettings-Position="Top" Font-Names="arial" style="width:100%;  font-family:Segoe UI"
                                    AutoGenerateSelectButton="False" CssClass="table  font-weight-light" GridLines="None" Font-Size="14px" >
                                    <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                    <HeaderStyle CssClass="GridviewScrollHeader_line_boot" BorderStyle="None" />
                                    <RowStyle CssClass=""  />
                                    <PagerStyle CssClass="pagination-ys" />
                                    <Columns>
                                        <asp:BoundField HeaderText="OPCIONES"   />
                                    </Columns>

                                </asp:GridView>
                        </asp:Panel>
                            
                        </div>    
                       
                        
                    </ContentTemplate>

                    <Triggers>
                    </Triggers>
                </asp:UpdatePanel>
                 
            </div>
        <div id="contenido_titulo_listado_solicitudes" style="width: auto; position: inherit; margin-left: 10px; margin-right: 10px" class="border_inferior_radius_">
            <asp:UpdatePanel ID="UpdatePanel_title" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                <ContentTemplate>
                    <div class="row">
                         <div class="col-sm-8">
                             <asp:Label ID="Label_estado" runat="server" ForeColor="Black" Font-Size="9px" Style="float: left"></asp:Label>
                             <asp:Label ID="Label_titulo_listado_solicitudes" CssClass="p2" runat="server"  Style="float: left">Resultados busqueda</asp:Label>
                         </div>
                        <div class="col-sm-4">
                            <asp:Label ID="Label_titulo_listado" runat="server" Text="Consulta historico de tramites y respuestas" Style=" font-size: 12px; float: right; display:none"></asp:Label>
                        </div>
                    </div>
                    
                    

                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
            <div id="contenedor_opciones_solictitud_general" style="width: 100%; text-align: left; font-family: Arial; background-color: #E7EDF5;margin-top: 1px; border-color: #b0c4de; border-style: ridge; border-width: 1px; display:none">
                <asp:UpdatePanel ID="update_botonoes_opciones_solicitud_general" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <input id="Hidden_ruta_archivo" type="hidden" value="" runat="server">
                        <input id="Hidden_solicitud_compartido" type="hidden" value="-1" runat="server">
                        <input id="Hidden_result_eliminar" type="hidden" value="" runat="server">
                        <input id="Hidden_resultado_ver_documento" type="hidden" value="" runat="server">
                        <input id="Hiddenxxxxxx" type="hidden" value="" runat="server">
                        <asp:Button ID="Button_ver_documento_solicitud" runat="server" Enabled="false" Text="Ver documento solicitud" ToolTip="Visualiza el documento de solicitud de aprobación" CssClass="boton" Style="display:none"  />
                        <asp:Button ID="Button_ver_documento_respuesta_solicitud" runat="server"  Text="Ver documento respuesta" ToolTip="Visualiza el documento respuesta de solicitud de aprobación" CssClass="boton" Style="display:none" />
                        <asp:Button ID="Button_activa_desicion_aprobacion" runat="server" Text="Gestionar solicitud"  ToolTip="Agrega una nueva solicitud de aprobación" CssClass="boton" Style="display:none" />
                        <asp:Button ID="Button_lista_filtro" runat="server"  Text="Gestionar solicitud"  CssClass="boton" Style="display:none" />
                        <asp:Button ID="Button_ver_documentos_relacionados" runat="server"  Text="Ver documentos" ToolTip="Lista los documentos compartidos relacionados con el registro seleccionado"  CssClass="boton"  />
                        <asp:Button ID="Button_eliminar_registro" runat="server" Text="Eliminar"   CssClass="boton"  ToolTip="Elimina el registro seleccionado" OnClientClick="ConfirmMensajeGeneral('Desea eliminar el registro','Hiddenxxxxxx')"   />                  
                        <asp:Button ID="Button_traza_solic" runat="server" Text=""  CssClass="boton"  ToolTip=""   />
                        <asp:Button ID="Button_deta_solic" runat="server" Text=""  CssClass="boton"  ToolTip=""   />
                        <asp:Button ID="Button_lo_solic" runat="server" Text=""  CssClass="boton"  ToolTip=""   />
                    </ContentTemplate>
                </asp:UpdatePanel>

            </div>
        </div>
        <!--Popup visor externo-->             
                  <asp:Panel ID="Panel_visor_externo" runat="server" Style="display:none;  height:100%; width:99%" Width="100%" Height="100% "  CssClass="modal_content_general">
                      <asp:ModalPopupExtender ID="ModalPopupExtender_visor_externo" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_visor_externo"
                          PopupControlID="Panel_visor_externo"  CancelControlID="ButtonSalir_visor_externo">
                      </asp:ModalPopupExtender>
                      <div id="Cabecerapendiente_visor_externo" class="modal_title_superior">              
                          <button style="margin-right:10px" type="button" onclick="activa_boton_client_server('ButtonSalir_visor_externo');" class="close">&times;</button>
                      </div>
                      <div id="Cotenedorpendiente_visor_externo" style=" height: 90%; width: 100%; overflow:hidden" class="modal_content_back">        
                          <asp:UpdatePanel ID="UpdatePanel_visor_externo" runat="server" UpdateMode="Conditional">
                              <ContentTemplate>
                                  <iframe id="Iframe_visor_externo_" runat="server" frameborder="0"  style="width:100%; height:100%; overflow:hidden"></iframe>
                              </ContentTemplate>
                          </asp:UpdatePanel>                          
                      </div>
                       <div style="display: none; height: 1px">
                           <asp:Button ID="Button_visor_externo" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" Style="display: none" />
                           <asp:Button ID="ButtonSalir_visor_externo" runat="Server" Text="" CssClass="invisible" />
                            <input id="Hidden_tipo_visor" type="hidden" value="" runat="server"/>
                       </div>
                      
              </asp:Panel>
             <!--Popup visor externo-->
       
      
        <!--detalle respuesta-->
           <asp:Panel ID="Panel_detalle_respuesta" runat="server" Style="display:none; overflow:hidden" ForeColor="White" Width="95%" Height="100% " CssClass="modal_content_general">
                  <asp:ModalPopupExtender ID="ModalPopupExtender_detalle_respuesta" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_detalle_respuesta"
                      PopupControlID="Panel_detalle_respuesta"  CancelControlID="ButtonSalir_detalle_respuesta">
                  </asp:ModalPopupExtender>
                  <div id="Cabecerapendiente_detalle_respuesta" class="modal_title_superior_  modal-header">                
                         <button type="button" onclick="activa_boton_client_server('ButtonSalir_detalle_respuesta');" class="close">&times;</button>
                  </div>
                  <div id="Cotenedorpendiente_detalle_respuesta" style="color: Black; background-color: #FFFFFF; height: 90%; width: 100%; overflow:hidden" class="modal_content_back">       
                      <asp:UpdatePanel ID="UpdatePanel_detalle_respuesta" runat="server" UpdateMode="Conditional">
                          <ContentTemplate>
                              <iframe id="Iframe_visor_externo__" runat="server" frameborder="0"  style="width:100%; height:100%; overflow:hidden"></iframe>
                          </ContentTemplate>
                      </asp:UpdatePanel>                    
                  </div>
               <div style="display: none; height: 1px">
                   <asp:Button ID="ButtonSalir_detalle_respuesta" runat="Server" Text="" CssClass="invisible" />
                   <asp:Button ID="Button_detalle_respuesta" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
               </div>
                    
              </asp:Panel>
         <!--detalle transacciones-->
	               <asp:Panel ID="Panel_transacciones" runat="server" Style="display:none; width:90%; height:100%" CssClass="modal_content_general" >
	                      <asp:ModalPopupExtender ID="ModalPopupExtender_transacciones" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_transacciones_dos"
	                          PopupControlID="Panel_transacciones"  CancelControlID="ButtonSalir_transacciones">
	                      </asp:ModalPopupExtender>
	                      <div id="Cabecerapendiente_transacciones" class="modal_title_superior_ modal-header" >  
                                     <h6 class="modal-title">transacciones</h6>                                 
                                  <button type="button" onclick="activa_boton_client_server('ButtonSalir_transacciones');" class="close">&times;</button>
	                      </div>
	                      <div id="Cotenedorpendiente_transacciones" style="height: 90%; width: 100%; overflow:hidden" class="modal_content_back">         
	                          <asp:UpdatePanel ID="UpdatePanel_transacciones" runat="server" UpdateMode="Conditional">
	                              <ContentTemplate>
	                                  <iframe id="Iframe_transacciones_historial_" runat="server" frameborder="0"  style="width:100%; height:100%; overflow:hidden"></iframe>
	                              </ContentTemplate>
	    
	                          </asp:UpdatePanel>
	                               
	                      </div>
                       <div style="display: none; height: 1px">
                           <asp:Button ID="Button_transacciones_dos" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                           <asp:Label ID="Label13" runat="server" Text="Ventana detalle de transacciones"></asp:Label>
                           <asp:Button ID="ButtonSalir_transacciones" runat="Server" Text=""  CssClass="invisible" />
                       </div>
	                       
              </asp:Panel>
         <!--detalle trazabilidad-->
	               <asp:Panel ID="Panel_trazabilidad" runat="server" Style="display:none; overflow:hidden; width:70%; height:100%"  CssClass="modal_content_general" >
	                      <asp:ModalPopupExtender ID="ModalPopupExtender_trazabilidad" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_trazabilidad_dos"
	                          PopupControlID="Panel_trazabilidad"  CancelControlID="ButtonSalir_trazabilidad">
	                      </asp:ModalPopupExtender>
	                      <div id="Cabecerapendiente_trazabilidad" class="modal_title_superior_ modal-header">                     
	                           <h6 class="modal-title">Trazabilidad</h6>
                               <button type="button" onclick="activa_boton_client_server('ButtonSalir_trazabilidad');" class="close">&times;</button>
	                      </div>
	                      <div id="Cotenedorpendiente_trazabilidad" style="height: 90%; width: 100%; border-top:none" class="modal_content_back">          
	                          <asp:UpdatePanel ID="UpdatePanel_trazabilidad" runat="server" UpdateMode="Conditional">
	                              <ContentTemplate>
	                                  <iframe id="Iframe_trazabilidad_" runat="server" frameborder="0"  style="width:100%; height:100%; overflow:hidden"></iframe>
	                              </ContentTemplate>
	    
	                          </asp:UpdatePanel>
	                               
	                      </div>
                       <div style="display: none; height: 1px">
                           <asp:Button ID="Button_trazabilidad_dos" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                           <asp:Button ID="ButtonSalir_trazabilidad" runat="Server" Text="" CssClass="modal_boton_hiden"
                                />
                       </div>
	                      
              </asp:Panel>
                   
                   <asp:Panel ID="Panel_historico_tramite" runat="server" Style="display:none; overflow:hidden" ForeColor="White" Width="95%" Height="100% " CssClass="modal_content_general">
                      <asp:ModalPopupExtender ID="ModalPopupExtender_historico_tramite" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_historico_tramite"
                          PopupControlID="Panel_historico_tramite" Y="1" CancelControlID="ButtonSalir_historico_tramite">
                      </asp:ModalPopupExtender>
                      <div id="Cabecerapendiente_historico_tramite" class="modal_title_superior">             
                          <asp:Label ID="Label1" runat="server" Text="" Font-Size="10"></asp:Label>
                          <div id="Div_historico_tramite" style="float: right">
                              <asp:Button ID="ButtonSalir_historico_tramite" runat="Server" Text="X" CssClass="modal_boton_hiden"
                                  ForeColor="#000066" Height="21px" />
                          </div>
                      </div>
                      <div id="Cotenedorpendiente_historico_tramite" style="color: Black; background-color: #FFFFFF; height: 90%; width: 100%; overflow:hidden" class="modal_content_back">        
                          <asp:UpdatePanel ID="UpdatePanel_historico_tramite" runat="server" UpdateMode="Conditional">
                              <ContentTemplate>
                                  <iframe id="Iframe_historico_tramite_" runat="server" frameborder="0"  style="width:100%; height:100%; overflow:hidden"></iframe>
                              </ContentTemplate>
                          </asp:UpdatePanel>                          
                      </div>
                      <asp:Button ID="Button_historico_tramite" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" style="display:none" />
              </asp:Panel>
             <!--detalle transacciones-->
           <asp:Panel ID="Panel1" runat="server" Style="display:none; overflow:hidden; width:70%; height:100%" CssClass="modal_content_general" >
                  <asp:ModalPopupExtender ID="ModalPopupExtender1" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_transacciones_dos"
                      PopupControlID="Panel_transacciones"  CancelControlID="ButtonSalir_transacciones">
                  </asp:ModalPopupExtender>
                  <div id="Div3" class="modal_title_superior" >                     
                      <asp:Label ID="Label14" runat="server" Text="Ventana detalle de transacciones" ></asp:Label>
                      <div id="Div4" style="float: right">
                          <asp:Button ID="Button1" runat="Server" Text="X" ToolTip="Cerrar ventana detalle de transacciones" CssClass="modal_boton_hiden" />

                      </div>
                  </div>
                  <div id="Div6" style="height: 90%; width: 100%; overflow:hidden" class="modal_content_back">
                  
                      <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                          <ContentTemplate>
                              <iframe id="Iframe_log_transacciones_" runat="server" frameborder="0"  style="width:100%; height:100%; overflow:hidden"></iframe>
                          </ContentTemplate>

                      </asp:UpdatePanel>
                           
                  </div>
                   <asp:Button ID="Button2" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" style="display:none" />
              </asp:Panel>
        <!--filtro-->
          <div id="filtro_historico">
            <asp:Panel ID="Panel_filtro_historico" runat="server" Style="display:none; color: White; width: auto; height: auto; color:black" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_filtro_historico"  runat="server" BackgroundCssClass="FondoAplicacion" BehaviorID="Panel_filtro_historico" TargetControlID="ButtonSalir_filtro_historico" 
                    CancelControlID="Button_cerrar_filtro_historico" PopupControlID="Panel_filtro_historico" ></asp:ModalPopupExtender>
                <div class="modal-content">
                    <div id="divcabecer2_filtro_historico_" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title">Filtrar por fechas</h6>
                        <button type="button" onclick="activa_boton_client_server('Button_cerrar_filtro_historico');" class="close">&times;</button>
                    </div>
                    <div id="contenido_procesa_filtro_historico_" style="width: 100%; height: auto;" class="modal-body">
                        <div class=" container">
                            <div class=" d-flex ">
                                <h6>Fecha de asignación</h6>

                            </div>
                            <div class="d-flex ">
                                <div class="align-self-center">
                                    <p class="p-2">Desde </p>
                                </div>
                                <div class="">
                                    <asp:TextBox ID="TextBox_fecha_ini_asigna" CssClass="form-control" Style="width: 100px" runat="server"></asp:TextBox>
                                </div>
                                <div class="align-self-center">
                                    <p class="p-2">Hasta </p>
                                </div>
                                <div class="">
                                    <asp:TextBox ID="TextBox_fecha_fin_asigna" CssClass=" form-control" Style="width: 100px" runat="server"></asp:TextBox>
                                </div>

                            </div>
                        </div>

                        <div class=" container" style="margin-top: 5px">
                            <div class="d-flex">
                                <h6 class="h6">Fecha de culminación</h6>
                            </div>
                            <div class="d-flex ">

                                <div class="">
                                    <p class="p-2">Desde</p>
                                </div>
                                <div class="">
                                    <asp:TextBox ID="TextBox_fecha_ini_final_tramite" CssClass="form-control" Style="width: 100px" runat="server"></asp:TextBox>
                                </div>
                                <div class="">
                                    <p class="p-2">Hasta </p>
                                </div>
                                <div class="">
                                    <asp:TextBox ID="TextBox_fecha_fin_final_tramite" CssClass="form-control" Style="width: 100px" runat="server"></asp:TextBox>
                                </div>

                            </div>

                        </div>


                        <div style="display: none; height: 1px">
                            <asp:UpdatePanel ID="UpdatePanel_filtro_historico" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Button ID="Button_consultar" runat="server" Style="display: none" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <asp:Button ID="Button_filtro_historico" CssClass="invisible bg-transparent" runat="server" Text="" Height="1px" Width="1px" />
                            <asp:Button ID="ButtonSalir_filtro_historico" CssClass="invisible bg-transparent" runat="server" Text="" Height="1px" Width="1px" />
                            <asp:Button ID="Button_cerrar_filtro_historico" runat="Server" Text="" CssClass="invisible bg-transparent" />
                        </div>

                    </div>
                    <div class="modal-footer ">
                        <button type="button" class="btn btn-light  float-right" style="margin-right: 5px" onclick="activa_boton_client_server('Button_cerrar_filtro_historico')">Cancelar </button>
                        <button type="button" class="btn btn-success   float-right" style="margin-right: 5px" onclick="activa_boton_client_server('Button_consultar')">Aceptar </button>

                    </div>
                </div>
                
            </asp:Panel>
        </div>
        <input id="Hidden_name_event" type="hidden" value="" runat="server"/>     
        <input id="Hidden_colum_header" type="hidden" value="" runat="server"/>        
        <div id="inferior_bajo_boton" style="width: 0%; height: 0%; background-color: #E7EDF5; display: none">
             
            <asp:UpdatePanel ID="updatapanel_iframe" runat="server" UpdateMode="Conditional">
                <ContentTemplate>   
                     <iframe runat="server" id="ifmExcel_" width="0" height="0" marginheight="0" marginwidth="0"
                        frameborder="0" />
                   <asp:Button ID="Button_export_lista_event" runat="server" Text="Exportar" style="margin-top:5px; display:none" CssClass="boton_azul"   /> 
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

