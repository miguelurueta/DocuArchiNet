<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormDaConsultaDocumento.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormDaConsultaDocumento" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
     <style>
      table#Table_campos_consulta,   th, td {
               padding: 1px;
               width:5px;
          }
    </style>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Consulta documentos</title>
      <script src="../js/ui/jquery-3.4.1.min.js"></script>  
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
    <link href="../ccs/themes/base/jquery.ui.all.css" rel="stylesheet" type="text/css" />
     <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <link rel="stylesheet" href="../Styles/style.css" />
   <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
    <script src="../js/validate_campos.js"></script> 
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
   <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <script src="../js/Docuarchi/WebFormDaConsultaDocumento.js" type="text/javascript"></script>
    <script src="../js/java_general/general_code_java.js"></script>
    <script defer src="../Awesome/js/all.js"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet">
  <link href="../Awesome/css/brands.css" rel="stylesheet">
  <link href="../Awesome/css/solid.css" rel="stylesheet">
    <script defer src="../Awesome/js/brands.js"></script>
  <script defer src="../Awesome/js/solid.js"></script>
  <script defer src="../Awesome/js/fontawesome.js"></script>
    <link href="../Styles/w3.css" rel="stylesheet" />
</head>
   
<body style="background-color:white; margin:0px">
    <form id="form1" runat="server" style="align-content:initial">
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
                    //
                    elment_postbak = args.get_postBackElement();
                    if (args.get_postBackElement().id == 'Button_consulta') {
                        document.getElementById("Button_consulta").disabled = true;
                        document.getElementById("Button_consulta").value="Espere...."
                    }
                    if (args.get_postBackElement().id == 'Button_limpiar_campos') {
                        document.getElementById("Button_limpiar_campos").disabled = true;
                        document.getElementById("Button_limpiar_campos").value = "Espere...."
                    }
                    posicion_update_pogres('progres_bar');
                  
                    
                }
                function CheckStatus(sender, args) {
                    try {
                        auto_zise_consulta();
                        GetLista_consulta_gabinetes('TextBox_buequeda_general');
                    if (elment_postbak.id == "GridView_val_radicacion") {
                            limpia_sleccion();
                    }
                    if (elment_postbak.id == "boton_trd_restore") {
                        if (document.getElementById("Hidden_resultado").value == "YES") {
                            setear_trd_documento();
                            document.getElementById("Hidden_resultado").value = "";

                        }
                        //mueve_scroll_data_gred('boton_trd_restore', 'Panel_campos_consuta');
                    }
                    if (elment_postbak.id == "boton_clase_documento") {
                        auto_zise_tipo_documento();
                        if (document.getElementById("Hidden_resultado").value == "YES") {
                            document.getElementById("Hidden_resultado").value = "";
                        }         
                    }
                    //comman_expediente_restore
                    if (elment_postbak.id == "boton_expediente_restore") {
                        if (document.getElementById("Hidden_resultado").value == "YES") {
                            setear_expdiente();
                            document.getElementById("Hidden_resultado").value = "";
                        }    
                    }
                    //boton_fecha_elaboracion_restore
                    if (elment_postbak.id == "boton_fecha_elaboracion_restore") {
                        if (document.getElementById("Hidden_resultado").value == "YES") {
                            setear_fecha_elaboracion();
                            document.getElementById("Hidden_resultado").value = "";
                        }     
                    }
                    //boton_clase_documento_restore
                    if (elment_postbak.id == "boton_clase_documento_restore") {
                        if (document.getElementById("Hidden_resultado").value == "YES") {
                            setear_clase_documento();
                            document.getElementById("Hidden_resultado").value = "";
                        }       
                    }
                    if (elment_postbak.id == "Button_visor_emergente") {
                            auto_zise_popup_visor_externo();
                    }
                   
                    if (elment_postbak.id == 'Button_consulta') {
                        
                        document.getElementById("Button_consulta").disabled = false;
                        document.getElementById("Button_consulta").value = "Consultar";
                    }
                    if (elment_postbak.id == 'Button_consulta_general') {
                        
                    }
                    if (elment_postbak.id == 'Button_limpiar_campos') {
                        
                        document.getElementById("Button_limpiar_campos").disabled = false;
                        document.getElementById("Button_limpiar_campos").value = "Restaurar";
                    }         
                    if (elment_postbak.id == "ImageButtonindice") {
                        auto_zise_panel_indice_documento();
                    }
                    if (elment_postbak.id == "ImageButton_exportar_archivo") {
                        auto_zise_descarga_documento();
                    }
                    if (elment_postbak.id == "ImageButton_toponimica") {
                        auto_zise_ubicacion_topografica();
                    }
                    //mueve_scroll_value(document.getElementById("Hidden_scroll").value, 'Panel_campos_consuta');
                    progres_hiden('progres_bar');
                    if (elment_postbak.id == 'Button_actualiza_indice') {
                        if (document.getElementById("Hidden_campos_dinamicos_edita").value !== "") {
                            actualiza_gre_campos_dinamicos();
                            document.getElementById("Hidden_campos_dinamicos_edita").value = "";

                        }
                    }
                    
                    }
                      catch (err) {
                          alert(err.message + " Funcion CheckStatus");
                    }
                }
                
            </script>
     <a id="da_show-sidebar_" class="btn btn-sm   hide_da_sidebar " href="#" data-target="#sidebar_">
                <i style="color: white" class="fas fa-bars"></i>
            </a>
   <div id="da_content_wraper" class="ml-0 mr-2  d-flex " style="padding-left: 1px; padding-right: 1px">    
       <div id="Contentizquierdo" style="width: 25%; float: left">
           <nav id="sidebar_" class=" bg-light_ pl-0 pr-0 " style="width: 100%">         
               <div id="contenido_titulo_controles_consulta" class="modal-header modal_title_superior bg-light_ p-2" style="background: #6d7fcc; border-top-left-radius: initial; border-top-right-radius: initial">
                    <asp:UpdatePanel ID="UpdatePanel_consulta_title_gabinete" runat="server" UpdateMode="Conditional">
                       <ContentTemplate>  
                               <asp:Image ID="Image1" runat="server" ImageUrl="../Docuarchi/imagenes/negro.png" Width="40px" CssClass="mt-1 mb-1" Style="display:none" />
                               <asp:Label ID="LabelGabinte" runat="server" Text="Gabinete" style="color: white; font-family: 'Segoe UI'" class=" mt-2 mb-2 ml-2 font-weight-light h6"></asp:Label>
                          
                       </ContentTemplate>
                    </asp:UpdatePanel> 
                   <h6 class=" mt-2 mb-2 ml-2 font-weight-light" id="pit_" style="color: white; float: left; font-family: 'Segoe UI'; display:none">Campos de busqueda </h6>
                   <a id="sidebarCollapse" class="close_ mr-1" style="float: right;   color:white">&times;</a>       
               </div>
               <div id="contenido_controles_consulta" style="width: 100%">
                   <asp:UpdatePanel ID="UpdatePanel_consulta" runat="server" UpdateMode="Conditional">
                       <ContentTemplate>     
                           <asp:Panel ID="Panel_campos_consuta" runat="server" ScrollBars="Both" Style="float: left; width: 100%; align-content: flex-start; background-color:white" CssClass="pl-1 pr-1" DefaultButton="Button_consulta">
                               <asp:Table ID="Table_campos_consulta" runat="server" Style="text-align: left; width: 95%" EnableViewState="false"></asp:Table>
                           </asp:Panel>
                       </ContentTemplate>
                   </asp:UpdatePanel>
               </div>
               <div id="contenido_controles_buton_consulta" style=" background-color:white; border-bottom:none" class="modal-header  justify-content-start">
                   <asp:UpdatePanel ID="UpdatePanel_botones_consulta" runat="server" UpdateMode="Conditional">
                       <ContentTemplate>
                               <asp:Panel ID="Panel_botones_consulta" runat="server">
                                   <asp:Button ID="Button_consulta" runat="server" Text="Aceptar" ToolTip="consultar"  OnClientClick="limpia_order();" CssClass="btn  btn-success" />
                                   <asp:Button ID="Button_limpiar_campos" runat="server" Text="Restaurar"  ToolTip="Restaurar valores campos" CssClass="btn  btn-success" />
                                   <asp:Button ID="Button_visor_emergente" runat="server" Text="" Style="display: none" />
                                   <asp:Button ID="Button_consulta_general" runat="server" Text="" Style="display: none"/>
                               </asp:Panel>   
                       </ContentTemplate>
                   </asp:UpdatePanel>
               </div>
           </nav>
       </div>
       <div id="Contenedorderecho" class=" mr-0 ml-0 pl-1 pr-1 pb-0 pt-0  " style="width: 75%; float: right">     
            <div id="contenido_titulo_val_radicacion" class="navbar navbar-expand-sm nav_botota_person modal_content_no_back_inferior"> 
                <button id="nav_togle_display" class="navbar-toggler" type="button" style="background-color: #6d7fcc" data-toggle="collapse" data-target="#navbarNavDropdown">
                    <span class="navbar-toggler-icon_"><i style="color: white" class="fad fa-th-list"></i></span>
                </button>
                <div class="nav col-md-6">    
                    <div class="nav-item active_">
                        <a class="nav-link active ml-1 " title="Ubicación topografica" style="color: #6d7fcc" href="#"  onclick="activa_boton_client_server('ImageButton_toponimica')"><i style="color: #0062cc" class="fal fa-location-circle fa-lg"></i>
                        </a>
                    </div>
                    <div class="nav-item active_">
                        <a class="nav-link active ml-1 " title="Eliminar registros" style="color: #6d7fcc" href="#"  onclick="Elimina_registro(event)"><i style="color: #0062cc" class="fal fa-times fa-lg"></i>
                        </a>
                    </div>
                </div> 
                <div class=" float-md-right col-md-6 float-sm-left">
                        <div class="input-group ">
                            <button id="td-boton" class="btn btn-outline-secondary border-right-2 " title="Restaura busqueda por campos" style="border-top-right-radius: 0px; border-bottom-right-radius: 0px" onclick="acti_busq_general_restore(event,this)" type="button">
                                <i class="fal fa-long-arrow-left"></i>
                            </button>
                            <asp:TextBox ID="TextBox_buequeda_general" runat="server"  CssClass="form-control form-control-sm complex  border-left-0" placeholder="Busqueda...." ></asp:TextBox>
                            <div class="input-group-append">
                                <button class="btn btn-outline-secondary" onclick="acti_busq_general_archivo_boton(event, this)" type="button">
                                    <i class="fal fa-search"></i>
                                </button>
                            </div>
                        </div>
                    </div>        
                        
            </div>
             <div id="contenido_datagrid_val_radicacion" style="height: 60%; width: 100%; position: relative; margin-top: 1px; overflow: auto">
                    <asp:UpdatePanel ID="UpdatePanel_conenido_grid_val_radicacion" runat="server" UpdateMode="Conditional" RenderMode="Block" style="width: 100%; height: 100%">
                        <ContentTemplate>
                            <asp:Panel ID="Panel_principal" runat="server" ScrollBars="Auto"
                                Width="100%" Style="">
                                <asp:GridView ID="GridView_val_radicacion" runat="server" Width="100%" EnableViewState="true" style=""
                                    AutoGenerateSelectButton="False" AllowPaging="true" Font-Size="13px" CssClass="table font-weight-light  " GridLines="None" 
                                     PagerSettings-Position="Top" AllowSorting="true" PageSize="8" >
                                    <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                    <HeaderStyle CssClass="GridviewScrollHeader_line_boot" BorderStyle="None" />
                                    <RowStyle CssClass="" />
                                    <PagerStyle CssClass="pagination-ys" />
                                     <Columns>
                                           <asp:TemplateField HeaderText="OPCIONES" >
                                         
                                     </asp:TemplateField>
                                                 
                                      </Columns>
                                </asp:GridView>
                            </asp:Panel>
                           
                        </ContentTemplate>
                        <Triggers>
                        </Triggers>
                    </asp:UpdatePanel>
                </div>        
            <div id="Contenido_botones_tipo_radicado"  class="navbar navbar-expand-sm nav_botota_person  modal_content_no_back_superior p-0">
                <asp:UpdatePanel ID="UpdatePanelabel_val_radicacion" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <input id="hdnEmailID_VAL" type="hidden" value="-1" runat="server" />
                        <input id="Hidden_consecutivo_radicado" type="hidden" value="-1" runat="server" />
                        <input id="Hidden_gabinete" type="hidden" value="" runat="server" />
                        <input id="Hidden_nureg" type="hidden" value="" runat="server" />
                        <input id="Hidden_nugab_sele" type="hidden" value="" runat="server" />
                        <asp:Label ID="titulo_label_val_radicacion" runat="server" ForeColor="black" Font-Size="10"  Style=" width: 100%" CssClass="h6 font-weight-light ml-2">Resultados busqueda</asp:Label>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
        
    </div>
        <div style="display:none">
            <asp:UpdatePanel ID="UpdatePanelButon" runat="server"
                    UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <asp:ImageButton ID="ImageButtonVisualiza_Documento" runat="server" ImageUrl="../Docuarchi/imagenes/visor-imagen.png" ToolTip="Visualiza documento" Style="margin-top: 0px; margin-left:5px; display:none" />
                        <asp:ImageButton ID="ImageButton_actualiza_batch" runat="server" ImageUrl="../Docuarchi/imagenes/actualiza-bach.png" ToolTip="Actualiza multiplex registros" Style="margin-top: 0px; display:none" />
                        <asp:ImageButton ID="ImageButton_Eliminar_Documento" runat="server" ImageUrl="../Docuarchi/imagenes/elimina_imagen.png" ToolTip="Eliminar documento" Style="margin-top: 0px; display:none" />
                        <asp:ImageButton ID="ImageButton_agregar_documento" runat="server" ImageUrl="../Docuarchi/imagenes/importargabinete2.png" ToolTip="Añadir documento al registro" Style="margin-top: 0px; display:none" />
                        <asp:ImageButton ID="ImageButton_exportar_carpeta" runat="server" ImageUrl="../Docuarchi/imagenes/Exportar_carpeta.png" ToolTip="Exportar documento a carpeta" Style="margin-top: 0px; display:none"  />
                        <asp:ImageButton ID="ImageButton_exportar_correo" runat="server" ImageUrl="../Docuarchi/imagenes/envia-correo-imagen.png" ToolTip="Exportar a correo electronico" Style="margin-top: 0px; display:none" />
                        <asp:ImageButton ID="ImageButton_exportar_archivo" runat="server" ImageUrl="../Docuarchi/imagenes/guardarimagen.png" ToolTip="Exportar a sistema de archivo" Style="margin-top: 0px; display:none" />          
                        <asp:ImageButton ID="ImageButton_exportar_gabinete" runat="server" ImageUrl="../Docuarchi/imagenes/importargabinete2.png" ToolTip="Exportar a gabinete" Style="margin-top: 0px; display:none" />
                        <asp:ImageButton ID="ImageButtonindice" runat="server" ToolTip="Visualiza indice documento" ImageUrl="../Docuarchi/imagenes/indice-imagen.png" Visible="true" Style="margin-top: 0px; display:none" />
                        <asp:ImageButton ID="ImageButton_toponimica" runat="server" ToolTip="Ubicación topografica" ImageUrl="../Docuarchi/imagenes/infoimagen.png"  Visible="true" Style="margin-top: 0px; display:none" />      
                    </ContentTemplate>
                    <Triggers>
                    </Triggers>
                </asp:UpdatePanel>
        </div>
         <!--Popup visor externo-->    
            <asp:Panel ID="Panel_visor_externo" runat="server" Style="display:none; overflow: hidden; width:100%; width:100%" ForeColor="Black"  CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_visor_externo" Y="1" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_visor_externo"
                    PopupControlID="Panel_visor_externo"  CancelControlID="ButtonSalir_visor_externo">
                </asp:ModalPopupExtender>
                <div id="modal_content_Panel_visor_externo" class="modal-content_" style="width:100%">
                    <div id="Cabecerapendiente_visor_externo" class="modal_title_superior_ modal-header">
                         <h6 class="modal-title d-inline ml-1"></h6>
                         <button type="button" value="ButtonSalir_visor_externo" class="close da_event_captive">&times;</button>   
                    </div>
                    <div id="Cotenedorpendiente_visor_externo" style="color: Black; height: 100%; width: 100%; overflow: hidden; border-top:none" class="modal_content_back modal-body_">
                        <asp:UpdatePanel ID="UpdatePanel_visor_externo" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <iframe id="Iframe_visor_externo_" runat="server" frameborder="0" style="width: 100%; height: 100%; overflow: hidden" scrolling="no"></iframe>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button_visor_externo" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" Style="display: none" />
                        <asp:Button ID="ButtonSalir_visor_externo" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" Style="display: none" />
                    </div>   
                </div>
            </asp:Panel>
        
        <!--POPUP QUE GUARDA EL POPUP CON EL CONTENEDOR DE APLICACION TRD-->  
            <input id="Hiddennameasigna" type="hidden" value="DOCUARCHI_NET" runat="server" />
            <input id="Hidden_id_serie" type="hidden" value="0" runat="server" />
            <input id="Hidden_id_sub_serie" type="hidden" value="0" runat="server" />
            <input id="Hidden_id_documento" type="hidden" value="0" runat="server" />
            <input id="Hidden_id_area" type="hidden" value="0" runat="server" />
            <asp:Panel ID="Panel_trd_popup" runat="server" Style="display: none; color: White; width: 100%; height: 100%">
                <asp:DragPanelExtender ID="DragPanelExtender_trd_popup" runat="server" TargetControlID="Panel_trd_popup" />
                <asp:ModalPopupExtender ID="ModalPopupExtende_trd_popup" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_trd_popup"
                    PopupControlID="Panel_trd_popup" CancelControlID="Buttoncerrar_trd_popup">
                </asp:ModalPopupExtender>
                <div id="divcabecer_trd_popup" class="cabecera2" style="width: 97%">
                    <asp:Button ID="Button_trd_popup" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_trd_popup" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Label ID="Label_trd_popup" runat="server" Text="Aplicar tabla de retención" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton_trd_popup" style="float: right">
                        <asp:Button ID="Buttoncerrar_trd_popup" runat="Server" Text="X"
                            ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana"  OnClientClick="mueve_scroll_data_gred('NOMBRESERIE', 'Panel_campos_consuta');"/>
                    </div>
                </div>
                <div id="Contenido_trd_popup" style="border: thin double #000080; color: black; background-color: #FFFFFF; height: 97%; width: 97%; float: left">
                    <asp:UpdatePanel ID="UpdatePanel_trd_popup" runat="server" UpdateMode="Conditional" style="height: 100%" RenderMode="Inline">
                        <ContentTemplate>
                            <iframe id="Iframe_trd_popup_" runat="server" style="width: 100%; height: 100%"></iframe>
                        </ContentTemplate>
                    </asp:UpdatePanel>

                </div>
            </asp:Panel>
         <!--POPUP QUE GUARDA EL POPUP CON EL CONTENEDOR DE LOS EXPEDIENTE-->  
            <asp:Panel ID="Panel_expdiente_popup" runat="server" Style="display: none; color: White; width: 100%; height: 100%">
                <asp:DragPanelExtender ID="DragPanelExtender_expdiente_popup" runat="server" TargetControlID="Panel_expdiente_popup" />
                <asp:ModalPopupExtender ID="ModalPopupExtende_expdiente_popup" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_expdiente_popup"
                    PopupControlID="Panel_expdiente_popup" CancelControlID="Buttoncerrar_expdiente_popup">
                </asp:ModalPopupExtender>
                <div id="divcabecer_expdiente_popup" class="cabecera2" style="width: 97%">
                    <asp:Button ID="Button_expdiente_popup" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_expdiente_popup" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Label ID="Label_expdiente_popup" runat="server" Text="Gestión expedientes" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton_expdiente_popup" style="float: right">
                        <asp:Button ID="Buttoncerrar_expdiente_popup" runat="Server" Text="X"
                            ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana"  OnClientClick="mueve_scroll_data_gred('CLASEDOCUMENTO', 'Panel_campos_consuta');"/>
                    </div>
                </div>
                <div id="Contenido_expdiente_popup" style="border: thin double #000080; color: black; background-color: #FFFFFF; height: 97%; width: 97%; float: left">
                    <asp:UpdatePanel ID="UpdatePanel_expdiente_popup" runat="server" UpdateMode="Conditional" style="height: 100%" RenderMode="Inline">
                        <ContentTemplate>
                            <iframe id="Iframe_expdiente_popup_" runat="server" style="width: 100%; height: 100%"></iframe>
                        </ContentTemplate>
                    </asp:UpdatePanel>

                </div>
            </asp:Panel>

         <!--POPUP QUE APLICA EL TIPO DE DOCUMENTO -->  
            
        <input id="Hidden_valor_seleccion" type="hidden" value="" runat="server">     
        <asp:Panel ID="Panel_tipo_popup" runat="server" Style="display: none; color: White; width: 100%; height: 100%">
            
            <asp:ModalPopupExtender ID="ModalPopupExtende_tipo_popup" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_tipo_popup"
                PopupControlID="Panel_tipo_popup" CancelControlID="Buttoncerrar_tipo_popup"></asp:ModalPopupExtender>
            <div id="divcabecer_tipo_popup" class="cabecera2" style="width: 97%">
                <asp:Button ID="Button_tipo_popup" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                <asp:Button ID="ButtonSalir_tipo_popup" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                <asp:Label ID="Label_tipo_popup" runat="server" Text="Aplicar tabla de retención" Font-Size="10" Style="float: left">
                </asp:Label>
                <div id="Divcerrarbuton_tipo_popup" style="float: right">
                    <asp:Button ID="Buttoncerrar_tipo_popup" runat="Server" Text="X"
                        ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" OnClientClick="mueve_scroll_data_gred('NOMBRESERIE', 'Panel_campos_consuta');"   />
                </div>
            </div>
            <div id="Contenido_tipo_popup" style="border: thin double #000080; color: black; background-color: #FFFFFF; height: 97%; width: 97%; float: left">
                <div id="Contenido_superior" style="width: 100%; height: 10%">
                    <asp:UpdatePanel ID="update_panel_drowlist" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:DropDownList ID="ComboBoxtipo" runat="server" EnableViewState="true" onchange="valor_tipo_documento();" Style="width: 100%; margin-bottom: 1px"></asp:DropDownList>
                        </ContentTemplate>
                    </asp:UpdatePanel>

                </div>
                <div id="contenido_gred" style="width: 100%; height: 80%">
                    <asp:UpdatePanel ID="UpdatePanelmensaje" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:TextBox ID="TextBoxinfotipo" runat="server" TextMode="MultiLine" Style="margin-bottom: 0px; margin-top: 0px; margin-left: 0px; width:95%; height:300px"></asp:TextBox>
                            <asp:Button ID="Button_lista_ayuda_tipo" runat="server" Text="Buscar" CssClass="boton" Style="margin-bottom: 5px; margin-top: 5px; margin-left: 5px; display: none" />
                        </ContentTemplate>
                        <Triggers>
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
                <div id="contenido_inferior" style="width: 100%; height: 10%; background-color: #E7EDF5">
                    <asp:UpdatePanel ID="Updatepanel_botones" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <input id="Button_aplicar_tipo" type="button" value="Aplicar" onclick="asignar_clase_documento();" style="margin-bottom: 5px; margin-top: 2px; margin-left: 5px;" class="boton" />
                            <asp:Button ID="Button_listar_tipos" runat="server" Text="Buscar" CssClass="boton" Style="margin-bottom: 5px; margin-top: 2px; margin-left: 5px; display: none" />
                            <input id="Hidden_id_tipo" type="hidden" value="0" runat="server" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>

            </div>
        </asp:Panel>
        <!--POPUP EXPORTAR DOCUMENTO-->  
        <asp:Panel ID="Panel_descarga_anexo_respuesta" runat="server" Style="display: none;  width: 50%; height: 100%" CssClass="modal_content_general">
            <asp:ModalPopupExtender ID="ModalPopupExtender_edition_descarga_anexo_respuesta" runat="server"
                TargetControlID="ButtonSalir_descarga_anexo_respuesta" BackgroundCssClass="FondoAplicacion"
                CancelControlID="Button_cerrar_descarga_anexo_respuesta" PopupControlID="Panel_descarga_anexo_respuesta">
            </asp:ModalPopupExtender>
            <div id="div_driver_anexo" class="modal_title_superior_ modal-header">
                <h6 class="modal-title d-inline ml-1">Descarga documento</h6>
                <button type="button" value="Button_cerrar_descarga_anexo_respuesta" class="close da_event_captive">&times;</button>
            </div>
            <div id="contenido_procesa_descarga_anexo_respuesta" style="width: 100%; height: 100%" class="modal_content_back">
                <asp:UpdatePanel ID="UpdatePanel_descarga_anexo_respuesta" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <iframe id="ifimpre_descarga_anexo_respuesta_" runat="server" width="100%" height="100%" style="border: none"></iframe>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div style="display: none; height: 1px">
                <asp:Button ID="ButtonSalir_descarga_anexo_respuesta" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                <asp:Button ID="Button_cerrar_descarga_anexo_respuesta" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
            </div>
        </asp:Panel>
         <!--POPUP INDICE DE DOCUMENTO--> 
            <asp:Panel ID="Panel_indice" runat="server" Style="display: none;  width: 60%; height: 93%" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtenderimpre_indice" runat="Server" BackgroundCssClass="FondoAplicacion" Y="1" 
                    TargetControlID="ButtonSalir_indice"
                    PopupControlID="Panel_indice" CancelControlID="Buttoncerrarimpre_indice">
                </asp:ModalPopupExtender>
                <div id="divcabecer2__indice" class="modal_title_superior_ modal-header">
                    <h6 class="modal-title d-inline ml-1">Indice documento</h6>
                    <button type="button" value="Buttoncerrarimpre_indice" class="close da_event_captive">&times;</button> 
                </div>
                <div id="content_Panel_indice" width: 100%; height: 96%" class="modal_content_back">
                    <asp:UpdatePanel ID="UpdatePanelindice" runat="server" UpdateMode="Conditional"
                        RenderMode="Inline">
                        <ContentTemplate>
                            <iframe id="ifrm_indice_" runat="server" style="border-style: none; width: 100%; height: 96%; position: relative; top: 1px; background-color: white; left: 0px;"
                                frameborder="0" scrolling="no"></iframe>
                            <input id="Hidden_result_indice" type="hidden" value="0" runat="server"/>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div style="display: none; height: 1px">
                    <asp:Button ID="Button1_indice" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="ButtonSalir_indice" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="Buttoncerrarimpre_indice" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                </div>
            </asp:Panel>
         <!--ubicacion toponimica-->
            <asp:Panel ID="Panel_ubicacion_toponimica_expediente_popup" runat="server" Style="display: none;  width: 50%; height: 99%" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtende_ubicacion_toponimica_expediente_popup" runat="Server" BackgroundCssClass="FondoAplicacion" 
                     TargetControlID="ButtonSalir_ubicacion_toponimica_expediente_popup"
                    PopupControlID="Panel_ubicacion_toponimica_expediente_popup" CancelControlID="Buttoncerrar_ubicacion_toponimica_expediente_popup" ></asp:ModalPopupExtender>
                <div id="divcabecer_ubicacion_toponimica_expediente_popup" class="modal-header">
                    <h6 class="modal-title d-inline ml-1">Ubicación topografica</h6>
                    <button type="button" value="Buttoncerrar_ubicacion_toponimica_expediente_popup" class="close da_event_captive">&times;</button>
                </div>
                <div id="Contenido_ubicacion_toponimica_expediente" style="  background-color: #FFFFFF; height: 97%; width: 99%; float: left">
                    <div id="div_treview_archivo_u_b_t" style="height: 99%">
                        <asp:Panel ID="Paneltreview_u_b_t" runat="server" ScrollBars="Both"
                            Height="100%" Width="100%" Style="position: inherit">
                            <asp:UpdatePanel ID="UpdatePanelViewArchivo_u_b_t" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:TreeView ID="TreeViewArchivo_u_b_t" runat="server" BackColor="white"
                                        PopulateNodesFromClient="False" RootNodeStyle-CssClass="RootNodeStyle"
                                        ParentNodeStyle-CssClass="ParentNodeStyle"
                                        LeafNodeStyle-CssClass="LeafNodeStyle" ForeColor="Black" Font-Size="11px" NodeIndent="1" ExpandDepth="0" SkipLinkText="">
                                        <HoverNodeStyle Font-Underline="False" />
                                        <LeafNodeStyle CssClass="LeafNodeStyle" HorizontalPadding="0px" NodeSpacing="0px" VerticalPadding="0px" />
                                        <NodeStyle ChildNodesPadding="0px" HorizontalPadding="0px" NodeSpacing="0px" VerticalPadding="0px" />
                                        <ParentNodeStyle ChildNodesPadding="0px" CssClass="ParentNodeStyle" HorizontalPadding="0px" NodeSpacing="0px" VerticalPadding="0px" />
                                        <RootNodeStyle ChildNodesPadding="0px" CssClass="RootNodeStyle" NodeSpacing="0px" VerticalPadding="0px" HorizontalPadding="0px" />
                                        <SelectedNodeStyle ForeColor="Red" />
                                    </asp:TreeView>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </asp:Panel>
                    </div>        
                </div>
                <div id="contendor_botones_unidad_u_b_t" class="modal-footer justify-content-end">
                    <a class="btn  btn-success  font-weight-light" style="height: auto; min-width: 140px; font-size: 14px; padding: 5px" title="Imprimir" href="#" onclick="fnExcelTre('TreeViewArchivo_u_b_t')"> Imprimir</a>
                </div>
                <div style="display: none; height: 1px">
                    <asp:Button ID="Button_ubicacion_toponimica_expediente_popup" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="ButtonSalir_ubicacion_toponimica_expediente_popup" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="Buttoncerrar_ubicacion_toponimica_expediente_popup" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                </div>
               
            </asp:Panel>
       
       <!--PROGRES-->
        <div id="Divpro_gres_bar">
            <asp:Panel ID="Panel_pro_gres_bar" runat="server" Style="display:none; color: White; width:40%; height:auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_pro_gres_bar" runat="server" BehaviorID="Panel_pro_gres_bar_ModalPopupExtender" TargetControlID="ButtonSalir_pro_gres_bar" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_pro_gres_bar" PopupControlID="Panel_pro_gres_bar"></asp:ModalPopupExtender>
                <div id="div1" class="modal_title_superior">                
                    <asp:Label ID="Label_pro_gres_bar" runat="server" Text=""  Style="">
                    </asp:Label>
                    <div id="Divcerrarbuton2_pro_gres_bar" style="float: right">
                        <asp:Button ID="Button_cerrar_pro_gres_bar" runat="Server" Text="X" style="display:none" CssClass="modal_boton_hiden"
                             ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_pro_gres_bar" style="width:100%; height:100%" class="modal_content_back"> 
                    <br />
                       <asp:Label ID="Label_progres_bar" runat="server" Text="Progreso de la tarea"></asp:Label>
                        <div id="myProgress" style="width:99%; align-content:center; margin-left:1px">
                            <div id="myBar" style="width:100%; text-align:center"></div>
                        </div>
                        <div id="myProgress_porcent" style="text-align: center">
                            0 %
                        </div>
                        <div id="myProgress_contador" style="text-align: center">
                            0 
                        </div>
                        <br>
                        <button class="boton_azul" onclick="myStopFunction(event)">Cancelar</button>      
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
        <input id="Hidden_colum_name_order" type="hidden" value=""/>
       <input id="Hidden_colum_value_order" type="hidden" value="0"/>
        <input id="Hidden_colum_value_order_ult" type="hidden" value="0"/>
       <input id="Hidden_estado_update" type="hidden" value="0"/>
       <asp:UpdatePanel ID="Updatepanel_actualiza" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <input id="Hidden_resultado" type="hidden" value="" runat="server"/> 
                    <input id="Hidden_id_expediente" type="hidden" value="0" runat="server"/>
                    <input id="Hidden_id_tipo_expediente" type="hidden" value="0" runat="server"/>
                    <input id="Hidden_id_unidad_conservacion" type="hidden" value="0" runat="server"/>
                    <input id="Hidden_id_tipo_unidad_conservacion" type="hidden" value="0" runat="server"/>
                    <input id="Hidden_id_inventario" type="hidden" value="0" runat="server"/>
                    <asp:Button ID="Button_actualiza_indice" runat="server" Text="Button" style="display:none" />
                     <input id="Hidden_campos_dinamicos_edita" type="hidden" value="" runat="server"/>
                    <input id="hidden_campos_dinamicos_aleas" type="hidden" value="" runat="server"/>
                     <input id="hidden_valore_campos" type="hidden" value="" runat="server"/>
                </ContentTemplate>
            </asp:UpdatePanel>
        <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 200px">
                <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
                Processing ...
            </div>
    </form>
      <script  accesskey="javascript" type="text/javascript">
         
          $(document).ready(function () {
              $('#sidebarCollapse').on('click', function () {
                  $('#sidebar_').toggleClass('active_da_slider');
                  $('#Contenedorderecho').toggleClass('active_content_rigth');
                  $('#Contentizquierdo').toggleClass('active_content_left');
                  $(this).toggleClass('active_da_slider');
                  $('#da_show-sidebar_').toggleClass('show_da_slide');
                  $('#da_show-sidebar_').toggleClass('hide_da_sidebar');

              });
              $('#da_show-sidebar_').on('click', function () {
                  $('#sidebar_').toggleClass('active_da_slider');
                  $('#Contenedorderecho').toggleClass('active_content_rigth');
                  $('#Contentizquierdo').toggleClass('active_content_left');
                  $(this).toggleClass('show_da_slide');
                  $(this).toggleClass('hide_da_sidebar');
                  //auto_zise_popup_validacion_radicados();

              });
          });
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
          });
</script>
</body>
    
</html>
