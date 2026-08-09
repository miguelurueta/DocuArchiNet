<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormGagregarunidadconservacionexpediente.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormGagregarunidadconservacionexpediente" %>
<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Agregar expediente</title>
     <script src="../js/ui/jquery-3.4.1.min.js"></script>  
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
   <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
     <script src="../js/gestion/WebFormGagregarunidadconservacionexpediente.js"></script>
    <script src="../js/MyJavaScriptFile.js"></script> 
     <script src="../js/validate_campos.js"></script> 
     <script  src="../Awesome/js/all.js"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet"/>
  <link href="../Awesome/css/brands.css" rel="stylesheet"/>
  <link href="../Awesome/css/solid.css" rel="stylesheet"/>
    <script  src="../Awesome/js/brands.js"></script>
  <script  src="../Awesome/js/solid.js"></script>
  <script  src="../Awesome/js/fontawesome.js"></script>
</head>
<body style="background-color:transparent; margin-top:0px">
    <form id="formagregarexepediente" runat="server">
  
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
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
                elment_postbak = args.get_postBackElement();
                posicion_update_pogres('progres_bar');
                var elmen = document.getElementById(elment_postbak.id)
                if (elmen.type == "button" || elmen.type == "image" || elmen.type == "submit") {
                    //value_element = elmen.value;
                    //elmen.value = "Espere..."
                    elmen.disabled = true;
                }
            }
            function CheckStatus(sender, args) {
                progres_hiden('progres_bar');
                //Button_Lista_Radicados Button_actualizar_guia Button_anular_guia
                var elmen = document.getElementById(elment_postbak.id)
                if (elmen.type == "button" || elmen.type == "image" || elmen.type == "submit") {
                    elmen.disabled = false;
                    //elmen.value = value_element;
                }
                if (elment_postbak.id == "ButtonAceptar") {
                    if (document.getElementById("Hidden_resultado").value == "YES") {
                        document.getElementById("Hidden_resultado").value = "";
                        activa_agrega_unidad_interface();
                    }
                }

            }

            </script>
        <input id="Hiddenname_empresagestion" type="hidden" value="0" runat="server"/>
         <asp:HiddenField ID="Hidden_tipo_unidad_seleccion"  value="0" runat="server" />
        <div id="contenedorcontroles" style="height: 100%; width: 99%; background-color: white" class="modal_content_general">
            <div id="titulo"  class="modal-header">
                <asp:Label ID="Label_estado" runat="server" Text="" class="modal-title d-inline ml-1"></asp:Label>
                <asp:Label ID="Labelresultado" runat="server" Text="Agregar unidad contenedora" class="modal-title d-inline ml-1"></asp:Label>
                <button id="Button_cerrar_principal" type="button" value="x" style="display: none" onclick="cerra_modal_expediente();" title="Cerrar ventana" class="close">&times;</button>
            </div>
            <div id="contenido_campos" style="overflow: auto" class="modal_content_back">
                <asp:UpdatePanel ID="update_panel_controles" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <input id="Hidden_id_empresa" type="hidden" value="0" runat="server" />
                        <asp:Panel ID="panel_controles" CssClass="p-2" runat="server">
                            <asp:Table ID="table_controles" runat="server">
                                <asp:TableRow>
                                    <asp:TableCell>
                                        <span>Tipo unidad contenedora</span>
                                    </asp:TableCell>
                                    <asp:TableCell>
                                        <asp:DropDownList ID="DropDownList_tipo_unidad_contenedora" Width="99%" runat="server" CssClass="custom-select mt-1" onchange="changue_option_manual();"></asp:DropDownList>
                                        <asp:TextBox ID="TextBox_ayuda_conetedora" runat="server" Style="width: 98%; background-color: yellow" Enabled="false" TextMode="MultiLine"></asp:TextBox>
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell>
                                        <asp:CheckBox ID="CheckBoxActivaCodigomanual" runat="server" Text=" " onchange="changue_option_manual();" />
                                        <span style="color: red" class="custom-checkbox">Consecutivo unidad contendora</span>
                                    </asp:TableCell>
                                    <asp:TableCell>
                                        <asp:TextBox ID="TextBoxCodigoManual" runat="server" Style="width: 220px; font-family: Arial" ReadOnly="true" BackColor="Gray" CssClass="mt-1"></asp:TextBox>
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell>
                                        <span>Fechas extremas</span>
                                    </asp:TableCell>
                                    <asp:TableCell>
                                        <asp:TextBox ID="TextBoxFECHA_EXTREMA_INICIAL" runat="server" Width="26%" CssClass="mt-1" onkeypress="return validate_fecha(event,this)" placeholder="0000 00 00"></asp:TextBox>
                                        <asp:CalendarExtender ID="TextBoxFECHA_EXTREMA_INICIAL_CalendarExtender" runat="server" BehaviorID="TextBoxFECHA_EXTREMA_INICIAL_CalendarExtender" TargetControlID="TextBoxFECHA_EXTREMA_INICIAL" Format='yyyy-MM-dd' PopupButtonID="ImageButtonfechaextremaini" />
                                        <button class="ml-1 btn border-0" id="ImageButtonfechaextremaini" type="button">
                                            <i class="fad fa-calendar-alt fa-1x"></i>
                                        </button>
                                        &nbsp;&nbsp;<asp:TextBox ID="TextBoxFECHA_EXTREMA_FINAL" runat="server" Width="26%" CssClass="mt-1" onkeypress="return validate_fecha(event,this)" placeholder="0000 00 00"></asp:TextBox>
                                        <asp:CalendarExtender ID="TextBoxFECHA_EXTREMA_FINAL_CalendarExtender" runat="server" BehaviorID="TextBoxFECHA_EXTREMA_FINAL_CalendarExtender" TargetControlID="TextBoxFECHA_EXTREMA_FINAL" Format='yyyy-MM-dd' PopupButtonID="ImageButtonfechaextremafin" />
                                        <button class="ml-1 btn border-0" id="ImageButtonfechaextremafin" type="button">
                                            <i class="fad fa-calendar-alt fa-1x"></i>
                                        </button>
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell>
                                        <span>Rangos extremos</span>
                                    </asp:TableCell>
                                    <asp:TableCell>
                                        <asp:TextBox ID="TextBoxRANGO_EXTREMO_INICIAL" runat="server" Width="27%" CssClass="mt-1 mr-1" placeholder="rango inicial"></asp:TextBox>
                                        <asp:TextBox ID="TextBoxRANGO_EXTREMO_FINAL" runat="server" Width="27%" CssClass="mt-1" placeholder="rango final"></asp:TextBox>
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell>
                                        <span>Tema unidad contendora</span>
                                    </asp:TableCell>
                                    <asp:TableCell>
                                        <asp:TextBox ID="TextBoxTEMA_EXPEDIENTE" runat="server" Width="98%" CssClass="mt-1"></asp:TextBox>
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell>
                                        <span>Descripción unidad contendora</span>
                                    </asp:TableCell>
                                    <asp:TableCell>
                                        <asp:TextBox ID="TextBoxASUNTO_EXPEDIENTE" runat="server" Width="98%" TextMode="MultiLine" CssClass="mt-1"></asp:TextBox>
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell>
                                        <span style="color:red">Organigrama (*)</span>
                                    </asp:TableCell>
                                    <asp:TableCell>
                                        <asp:DropDownList ID="DropDownListorganigrama" Width="99%" runat="server" CssClass="mt-1 custom-select" onchange="selecion_change_organigrama();"></asp:DropDownList>
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell>
                                        <span style="color:red">Area/proceso/sección(*)</span>
                                    </asp:TableCell>
                                    <asp:TableCell>
                                        <asp:DropDownList ID="DropDownListArea" Width="99%" runat="server" CssClass="mt-1 custom-select" onchange="selecion_change_area();"></asp:DropDownList>
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell>   
                                   <span>Instrumento</span>                                                                   
                                    </asp:TableCell>
                                    <asp:TableCell>
                                        <asp:DropDownList ID="DropDownList_instrumento" Width="99%" runat="server" CssClass="mt-1 custom-select" AutoPostBack="true"></asp:DropDownList>
                                        <asp:DropDownList ID="DropDownListsub_seccion" Width="99%" runat="server" CssClass="mt-1 custom-select" Style="display: none"></asp:DropDownList>
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell>
                                        <span>Serie documental</span> 
                                    </asp:TableCell>
                                    <asp:TableCell>
                                        <asp:DropDownList ID="DropDownListSerie" Width="99%" runat="server" CssClass="mt-1 custom-select" onchange="selecion_change_serie();"></asp:DropDownList>
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell>
                                        <span>Sub serie documental</span> 
                                    </asp:TableCell>
                                    <asp:TableCell>
                                        <asp:DropDownList ID="DropDownListSubserie" Width="99%" runat="server" CssClass="mt-1 custom-select"></asp:DropDownList>
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell>
                                        <asp:Label ID="Label6" runat="server" Text="Unidad contenedora" Style="font-family: Arial; font-size: 10pt; display: none"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell Style="">
                                        <asp:TextBox ID="TextBox_id_archivo" runat="server" MaxLength="9" ReadOnly="true" BackColor="Gray" Style="display: none" CssClass="mt-1"></asp:TextBox>
                                        <asp:Button ID="Button_activa_archivar_unidad" runat="server" Text="Archivar" CssClass="btn btn-success" Style="display: none" />
                                    </asp:TableCell>
                                </asp:TableRow>
                            </asp:Table>
                        </asp:Panel>

                        <div id="contenedor_botones_postbak" style="display: none">
                            <asp:Button ID="Button_lista_ayuda_tipo_unidad" runat="server" Text="" Style="height: 1%" />
                            <asp:Button ID="Button_selecion_organigrama" runat="server" Text="" Style="height: 1%" />
                            <asp:Button ID="Button_selecion_area" runat="server" Text="" Style="height: 1%" />
                            <asp:Button ID="Button_selecion_serie" runat="server" Text="" Style="height: 1%" />
                            <asp:Button ID="Button1_seleccion_expediente_manual" runat="server" Text="" Style="height: 1%" />
                        </div>
                    </ContentTemplate>
                    <Triggers>
                    </Triggers>
                </asp:UpdatePanel>
            </div>

            <asp:Panel ID="panel_botones" runat="server" class="border_titulo_inferior modal-footer">
                <asp:UpdatePanel ID="Updatepanel_botones" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <asp:Button ID="ButtonAceptar" runat="server" Text="Registrar" ToolTip="Agregar unidad conservacion contenedora" CssClass="btn btn-success" />
                        <asp:Button ID="ButtonRestaurar" runat="server" Text="Restaurar" ToolTip="Restaurar unidad conservacion contenedora" CssClass="btn btn-success" />
                        <input id="Hidden_resultado" type="hidden" value="" runat="server"/>
                    </ContentTemplate>
                </asp:UpdatePanel>


            </asp:Panel>
            <div id="Impresion_post">
                <asp:Panel ID="Panelimpresionpost" runat="server" Style="display: none; color: White; width: auto; height: auto">
                    <asp:DragPanelExtender ID="DragPanelExtenderimpre_post" runat="server" TargetControlID="Panelimpresionpost" />
                    <asp:ModalPopupExtender ID="ModalPopupExtenderimpre_post" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_post"
                        PopupControlID="Panelimpresionpost" CancelControlID="Buttoncerrarimpre_post">
                    </asp:ModalPopupExtender>
                    <div id="divcabecer2_post" class="cabecera2">
                        <asp:Button ID="Button1_post" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                        <asp:Button ID="ButtonSalir_post" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                        <asp:Label ID="Label15" runat="server" Text="Menu Impresion" Font-Size="10" Style="float: left">
                        </asp:Label>
                        <div id="Divcerrarbuton2_post" style="float: right">
                            <asp:Button ID="Buttoncerrarimpre_post" runat="Server" Text="X"
                                ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />

                        </div>
                    </div>
                    <asp:UpdatePanel ID="UpdatePaneliframe_post" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div id="ContenidoImpresion_post" style="border: thin double #000080; color: black; background-color: #FFFFFF; height: 280px; width: 500px">
                                <iframe  style="width:100%; height:100%" id="ifimpre_post_" runat="server"></iframe>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </asp:Panel>
            </div>


        </div>
        <!--archiva expediente-->
        <div id="modal_reubicar_unidad_expediente">
            <asp:Panel ID="Panel_reubicar_unidad_expediente_popup" runat="server"  Style="display:none; color: White; width: 100%; height: 99%">
               
                <asp:ModalPopupExtender ID="ModalPopupExtende_reubicar_unidad_expediente_popup" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_reubicar_unidad_expediente_popup"
                    PopupControlID="Panel_reubicar_unidad_expediente_popup" CancelControlID="Buttoncerrar_reubicar_unidad_expediente_popup" Y="0"></asp:ModalPopupExtender>
                <div id="divcabecer_reubicar_unidad_expediente_popup" class="cabecera2" style="width:99%">
                    <asp:Button ID="Button_reubicar_unidad_expediente_popup" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_reubicar_unidad_expediente_popup" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Label ID="Label_reubicar_unidad_expediente_popup" runat="server" Text="Gestión unidad contendora" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton_reubicar_unidad_expediente_popup" style="float: right">
                         
                                 <asp:Button ID="Buttoncerrar_reubicar_unidad_expediente_popup" runat="Server" Text="X"
                                     ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana"  />
                             
                    </div>
                </div>  
                 <div id="Contenido_reubicar_unidad_expediente_popup" style="border: thin double #000080; color: black; background-color: #FFFFFF; height: 97%; width: 99%; float:left">
                       <div id="drowlist_r_u_e" style="height:5%" >
		                 <asp:UpdatePanel ID="UpdatePanelEntidad_r_u_e" runat="server" UpdateMode="Conditional">
		                     <ContentTemplate>
		                          <asp:DropDownList ID="DropDownListEntidadEmpresa_r_u_e" runat="server"  Width="100%" onchange="buton_click('Button_listar_edificio');" ></asp:DropDownList>
		                     </ContentTemplate>
		                 </asp:UpdatePanel>
		                
                      </div>
                      <div id="div_treview_archivo_r_u_e" style="height:80%; margin-top:5px">
		                   <asp:Panel ID="Paneltreview_r_u_e" runat="server" ScrollBars="Both"
		                       Height="100%" Width="100%" Style="position: inherit">
		                       <asp:UpdatePanel ID="UpdatePanelViewArchivo_r_u_e" runat="server" UpdateMode="Conditional">
		                           <ContentTemplate>
		                                 <asp:TreeView ID="TreeViewArchivo_r_u_e" runat="server" BackColor="white"
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
                      <div id="contendor_botones_unidad_r_u_e" style="height:10%; background-color: #E7EDF5;border-color: #b0c4de; border-width:1px; border-style:ridge">
		                     <asp:UpdatePanel ID="UpdatePanel_botones_unidad_r_u_e" runat="server" UpdateMode="Conditional">
		                         <ContentTemplate>
                                     <asp:Button ID="Button_actualizar_unidad" runat="server" Text="Editar" CssClass="boton" Style="display:none"   />
                                     <asp:Button ID="Button_agrega_unidad_conservacion_interface" runat="server" Text="Archivar" ToolTip="Archivar unidad documental en unidad contenedora"  CssClass="boton" Style="width:70px; display:none" />
		                             <asp:Button ID="Button_archivar" runat="server" Text="Archivar" CssClass="boton" style=" margin-left:10px"/>
                                     <asp:Button ID="Button_agrega_unidad_contenedora" runat="server" Text="Agregar" ToolTip="Agregar una unidad contenedora de unidad documental" CssClass="boton" style="margin-top:3px; margin-left:10px"/>
                                     <asp:Button ID="ButtonButtonEditar" runat="server" Text="Editar" CssClass="boton" ToolTip="Editar unidad contendora de unidad documental"  />
                                     <asp:Button ID="ButtonEliminar_unidad_contendora" runat="server" Text="Eliminar" ToolTip="Eliminar unidad contendora de unidad documental"  CssClass="boton" OnClientClick="ConfirmMensajeGeneral('Desea eliminar la unidad contenedora ?','Hidden_result_eliminar');" />
                                     <input id="Hidden_result_eliminar" type="hidden" value="0" runat="server"/>
		                         </ContentTemplate>
		                     </asp:UpdatePanel>
                     </div>
              </div>
            </asp:Panel>
            
        </div>
        <!--agregar unidad de conservacion-->
        <div id="modal_agregar_unidad_conservacion">
    
                <asp:Panel ID="Panel_agregar_unidad_conservacion_popup" runat="server"  Style="display: none; color: White; width: 52%; height: 99%">
                   
                    <asp:ModalPopupExtender ID="ModalPopupExtende_agregar_unidad_conservacion_popup" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_agregar_unidad_conservacion_popup"
                        PopupControlID="Panel_agregar_unidad_conservacion_popup" CancelControlID="Buttoncerrar_agregar_unidad_conservacion_popup" Y="1">
                    </asp:ModalPopupExtender>
                    <div id="divcabecer_agregar_unidad_conservacion_popup" class="cabecera2" style="width:97%">
                        <asp:Button ID="Button_agregar_unidad_conservacion_popup" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                        <asp:Button ID="ButtonSalir_agregar_unidad_conservacion_popup" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                        <asp:Label ID="Label_agregar_unidad_conservacion_popup" runat="server" Text="Gestión unidad contendora" Font-Size="10" Style="float: left">
                        </asp:Label>
                        <div id="Divcerrarbuton_agregar_unidad_conservacion_popup" style="float: right">
                             
                                     <asp:Button ID="Buttoncerrar_agregar_unidad_conservacion_popup" runat="Server" Text="X"
                                         ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana"  />
                                 
                        </div>
                    </div>  
                     <div id="Contenido_agregar_unidad_conservacion_popup" style="border: thin double #000080; color: black; background-color: #FFFFFF; height: 97%; width: 97%; float:left">
                         <asp:UpdatePanel ID="UpdatePanel_agregar_unidad_conservacion_popup" runat="server" UpdateMode="Conditional" style="height:100%" RenderMode="Inline">
                             <ContentTemplate>
                            <iframe  id="Iframe_agregar_unidad_conservacion_popup"  runat="server"  style="width:100%; height:100%"></iframe>                
                                 <asp:HiddenField ID="HiddenField1" runat="server" value="0"/>
                                 </ContentTemplate>
                             </asp:UpdatePanel>
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
