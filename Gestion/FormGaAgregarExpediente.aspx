<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="FormGaAgregarExpediente.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.FormGaAgregarExpediente" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
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
     <script src="../js/gestion/FormGaAgregarExpediente.js"></script>
    <script src="../js/validate_campos.js"></script> 
    <script src="../js/MyJavaScriptFile.js"></script> 
     <script defer src="../Awesome/js/all.js"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet">
  <link href="../Awesome/css/brands.css" rel="stylesheet">
  <link href="../Awesome/css/solid.css" rel="stylesheet">
    <script defer src="../Awesome/js/brands.js"></script>
  <script defer src="../Awesome/js/solid.js"></script>
  <script defer src="../Awesome/js/fontawesome.js"></script>
</head>
<body style="background-color:#A4A4A4; margin-top:0px">
    <form id="formagregarexepediente" runat="server">
  
        <asp:ScriptManager runat="server"></asp:ScriptManager>
        <script type="text/javascript" language="javascript">
            Sys.WebForms.PageRequestManager.getInstance().add_initializeRequest(InitializeRequest);
            Sys.Application.add_load(ApplicationLoadHandler)
            var elment_postbak;
            var value_element;
            function ApplicationLoadHandler(sender, args) {

                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(CheckStatus);

            }
            function InitializeRequest(sender, args) {
              
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
                try {
                   
                    if (elment_postbak.id == "Button_activa_archivar_unidad") {
                        auto_zise_reasigna_expe_unidad();
                    }
                } catch (ex) {
                    alert("Inconsistencia función CheckStatus page FormGaAgregarExpediente " + ex.message)  
                } finally {
                    var elmen = document.getElementById(elment_postbak.id)
                    if (elmen.type == "button" || elmen.type == "image" || elmen.type == "submit") {
                        elmen.disabled = false;
                    }
                    progres_hiden('progres_bar');
                }
            }

            </script>
        <input id="Hiddenname_empresagestion" type="hidden" value="0" runat="server">
      <div id="contenedorcontroles" style="height:100%; width:100%; margin:auto; background-color:white" Class="modal_content_general">
         <div id="titulo" Style="" class="modal_title_superior_ modal-header">
              <h6 class="modal-title d-inline ml-1 mt-1">Registrar unidad documental</h6>    
             <button id="Button_cerrar_principal" type="button" value="x" style="display:none"  onclick="cerra_modal_expediente();" title="Cerrar ventana" class="close"> &times;</button>             
         </div>
          <div id="contenido_campos" style="overflow:scroll" class="modal_content_back">
          <asp:UpdatePanel ID="update_panel_controles" runat="server" UpdateMode="Conditional" >
              <ContentTemplate>
                  <input id="Hidden_id_empresa" type="hidden" value="0" runat="server"/>
                  <asp:Panel ID="panel_controles" runat="server" CssClass="p-2" >
                      <asp:Table ID="table_controles" runat="server">
                          
                          <asp:TableRow>
                              <asp:TableCell>
                                  <asp:CheckBox ID="CheckBoxActivaCodigomanual" runat="server" Text="" ForeColor="Red" Style="font-family: Arial; font-size: 10pt" onchange="changue_option_manual();" />
                                   <span style=" color:red" class="ml-1">Consecutivo unidad</span>
                              </asp:TableCell>
                              <asp:TableCell>
                                  <asp:TextBox ID="TextBoxCodigoManual" runat="server" Style="width: 220px; font-family: Arial" ReadOnly="true" BackColor="Gray"></asp:TextBox>
                              </asp:TableCell>
                          </asp:TableRow>
                          <asp:TableRow>
                              <asp:TableCell>
                                  <span style=" color:red">Fecha inicial (Expedición) *</span> 
                              </asp:TableCell>
                              <asp:TableCell>
                                  <asp:TextBox ID="TextBoxFECHA_EXTREMA_INICIAL" runat="server" MaxLength="10" CssClass="mt-1" Width="26%" onkeypress="return validate_fecha(event,this)" placeholder="0000 00 00"></asp:TextBox>
                                  <asp:CalendarExtender ID="TextBoxFECHA_EXTREMA_INICIAL_CalendarExtender" runat="server" BehaviorID="TextBoxFECHA_EXTREMA_INICIAL_CalendarExtender" TargetControlID="TextBoxFECHA_EXTREMA_INICIAL" Format='yyyy-MM-dd' PopupButtonID="ImageButtonfechaextremaini" />
                                   <button class="ml-1 btn border-0" id="ImageButtonfechaextremaini" type="button">
                                      <i class="fad fa-calendar-alt fa-1x"></i>
                                    </button>
                                
                              </asp:TableCell>
                          </asp:TableRow>
                          <asp:TableRow>
                              <asp:TableCell>
                                  <span >Fecha final (Terminación)</span>         
                              </asp:TableCell>
                              <asp:TableCell>
                                  <asp:TextBox ID="TextBoxFECHA_EXTREMA_FINAL" runat="server" MaxLength="10" Width="26%" CssClass="mt-1" onkeypress="return validate_fecha(event,this)" placeholder="0000 00 00"></asp:TextBox>
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
                                  <asp:TextBox ID="TextBoxRANGO_EXTREMO_INICIAL" MaxLength="9" runat="server" Width="27%" CssClass="mt-1 " placeholder="rango inicial" ></asp:TextBox>
                                  &nbsp;&nbsp;
                                  <asp:TextBox ID="TextBoxRANGO_EXTREMO_FINAL" runat="server" MaxLength="9" Width="27%" CssClass="mt-1 " placeholder="rango final" ></asp:TextBox>               
                              </asp:TableCell>
                          </asp:TableRow>
                          <asp:TableRow>
                              <asp:TableCell>
                                   <span>Tema expediente</span>
                              </asp:TableCell>
                              <asp:TableCell>
                                  <asp:TextBox ID="TextBoxTEMA_EXPEDIENTE" runat="server" CssClass="mt-1"  Width="98%"></asp:TextBox>
                              </asp:TableCell>
                          </asp:TableRow>
                          <asp:TableRow>
                              <asp:TableCell>
                                  <span>Asunto expediente</span>
                              </asp:TableCell>
                              <asp:TableCell>
                                  <asp:TextBox ID="TextBoxASUNTO_EXPEDIENTE" runat="server" Width="98%" CssClass="mt-1" TextMode="MultiLine"></asp:TextBox>
                              </asp:TableCell>
                          </asp:TableRow>
                          <asp:TableRow>
                              <asp:TableCell>            
                                  <span>Observación expediente</span>
                              </asp:TableCell>
                              <asp:TableCell>
                                  <asp:TextBox ID="TextBoxOBSERVACION_EXPEDIENTE" runat="server" Width="98%" CssClass="mt-1" TextMode="MultiLine"></asp:TextBox>
                              </asp:TableCell>
                          </asp:TableRow>
                           <asp:TableRow>
                              <asp:TableCell>
                                   <span>Nombre Solicitante</span>
                                  <input type="image" id="image_ayuda_solicitante" src="../workflow/imageneswf/ayuda.png" style="height:20px" class="def" onclick="ayuda_compartir('image_ayuda_solicitante', 'contenido_campos');" >
                              </asp:TableCell>
                              <asp:TableCell>
                                 <asp:TextBox ID="TextBoxNOMBRE_PERSONA_EXPEDIENTE" runat="server" Width="98%" CssClass="mt-1"></asp:TextBox>
                                  
                              </asp:TableCell>
                          </asp:TableRow>
                          <asp:TableRow>
                              <asp:TableCell>
                                  <span>Nit/Identificación solicitante</span>
                              </asp:TableCell>
                              <asp:TableCell>
                                 <asp:TextBox ID="TextBoxIDENTIFICACION_PERSONA_EXPEDIENTE" runat="server" Width="98%" MaxLength="120" CssClass="mt-1"></asp:TextBox>
                              </asp:TableCell>
                          </asp:TableRow>
                          <asp:TableRow>
                              <asp:TableCell>
                                  <span>Nombre Responsable</span>
                                  <input type="image" id="image_ayuda_responsable" src="../workflow/imageneswf/ayuda.png" style="height: 20px" class="def" onclick="ayuda_compartir('image_ayuda_responsable', 'contenido_campos');">
                              </asp:TableCell>
                              <asp:TableCell>
                                 <asp:TextBox ID="TextBoxNOMBRE_RESPONSABLE_EXPEDIENTE" runat="server" Width="98%" CssClass="mt-1"></asp:TextBox>
                              </asp:TableCell>
                          </asp:TableRow>
                          <asp:TableRow>
                              <asp:TableCell>
                                  <span>Nit/Identificación responsable</span>
                              </asp:TableCell>
                              <asp:TableCell>
                                 <asp:TextBox ID="TextBoxIDENFICACION_RESPONSABLE_EXPEDIENTE" runat="server" Width="98%" MaxLength="120" CssClass="mt-1"></asp:TextBox>
                              </asp:TableCell>
                          </asp:TableRow>
                           <asp:TableRow>
                              <asp:TableCell ColumnSpan="2" style="text-align:center; width:100%" CssClass="mt-2">
                                   <span class="h5">CLASIFICACION DOCUMENTAL</span>
                              </asp:TableCell>
                          </asp:TableRow>
                          <asp:TableRow>
                              <asp:TableCell>
                                  <span style=" color:red">Tipo unidad documental(*)</span>
                              </asp:TableCell>
                              <asp:TableCell>
                                  <asp:DropDownList ID="DropDownListNOMBRE_TIPO_UNIDAD_DOCUMENTAL" CssClass="mt-1 custom-select" Width="99%" runat="server" ></asp:DropDownList>
                              </asp:TableCell>
                          </asp:TableRow>
                           <asp:TableRow>
                              <asp:TableCell>
                                  <span>Fondo documental</span>
                              </asp:TableCell>
                              <asp:TableCell>
                                  <asp:DropDownList ID="DropDownListNOMBRE_FONDO" Width="99%" CssClass="mt-1 custom-select" runat="server" onchange="selecion_change_organigrama();"></asp:DropDownList>
                              </asp:TableCell>
                          </asp:TableRow>             
                          <asp:TableRow>
                              <asp:TableCell>
                                   <span style=" color:red">"Organigrama (*)</span>
                              </asp:TableCell>
                              <asp:TableCell>
                                  <asp:DropDownList ID="DropDownListorganigrama" Width="99%" runat="server" CssClass="mt-1 custom-select" onchange="selecion_change_organigrama();"></asp:DropDownList>
                              </asp:TableCell>
                          </asp:TableRow>
                          <asp:TableRow>
                              <asp:TableCell>
                                  <span style=" color:red">Area/departamento(*)</span>
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
                                  <asp:DropDownList ID="DropDownList_instrumento" Width="99%" runat="server" CssClass="mt-1 custom-select"  AutoPostBack="true" ></asp:DropDownList>
                                  <asp:DropDownList ID="DropDownListsub_seccion" Width="99%" runat="server" onchange="selecion_change_sub_area();" style="display:none" ></asp:DropDownList>
                              </asp:TableCell>
                          </asp:TableRow>
                          <asp:TableRow>
                              <asp:TableCell>
                                  <span>Ciclo expediente archivo</span>
                              </asp:TableCell>
                              <asp:TableCell>
                                  <asp:DropDownList ID="DropDownListNOMBRE_CICLO_ARCHIVO" Width="99%" runat="server" CssClass="mt-1 custom-select"></asp:DropDownList>
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
                                  <span  style=" color:red">Medio de la unidad documental(*)</span>
                              </asp:TableCell>
                              <asp:TableCell>
                                  <asp:DropDownList ID="DropDownListBoxtipoexpediente" Width="99%" runat="server" CssClass="mt-1 custom-select" onchange="lista_ayuda_expediente();"></asp:DropDownList>
                              </asp:TableCell>
                          </asp:TableRow>
                          <asp:TableRow>
                              <asp:TableCell>
                                 
                              </asp:TableCell>
                              <asp:TableCell>
                                  <asp:TextBox ID="TextBoxayuda" runat="server" Width="98%" Style="height: 30px" ReadOnly="True" CssClass="mt-1" TextMode="MultiLine" BackColor="Yellow"></asp:TextBox>
                              </asp:TableCell>
                          </asp:TableRow>
                          <asp:TableRow>
                              <asp:TableCell>
                                  <span>Tipo unidad de conservación</span>
                              </asp:TableCell>
                              <asp:TableCell>
                                  <asp:DropDownList ID="DropDownList_tipo_unidad_conservacion" Width="98%" runat="server" CssClass="mt-1 custom-select"></asp:DropDownList>
                              </asp:TableCell>
                          </asp:TableRow>
                          <asp:TableRow>
                              <asp:TableCell>
                                  <span>Paginas digitalizadas</span>
                              </asp:TableCell>
                              <asp:TableCell>
                                  <asp:TextBox ID="TextBoxNUMERO_DIGITALIZADO_CONTENIDO" runat="server" Width="50%" ReadOnly="true" Text="0" class="solo-numero mt-1 "></asp:TextBox>
                              </asp:TableCell>
                          </asp:TableRow>
                          <asp:TableRow>
                              <asp:TableCell>
                                   <span>folios físicos</span>
                              </asp:TableCell>
                              <asp:TableCell>
                                  <asp:TextBox ID="TextBoxNUMERO_FOLIOS_CONTENIDOS" runat="server" Width="50%" ReadOnly="true" Text="0" class="solo-numero mt-1 "></asp:TextBox>
                              </asp:TableCell>
                          </asp:TableRow>
                          <asp:TableRow>
                              <asp:TableCell>
                                  <span>Documentos electrónicos</span>
                              </asp:TableCell>
                              <asp:TableCell>
                                  <asp:TextBox ID="TextBoxNUMERO_ELECTRONICO_CONTENIDO" runat="server" Width="50%" ReadOnly="true" Text="0" class="solo-numero mt-1 "></asp:TextBox>
                              </asp:TableCell>
                          </asp:TableRow>

                          <asp:TableRow>
                              <asp:TableCell>
                                  <asp:CheckBox ID="CheckBox_unidad_contenedora" runat="server" Text="" ForeColor="Red" Checked="true"   />
                                   <span Style=" color:red" class="ml-1">Obliga unidad contenedora</span>
                              </asp:TableCell>
                              <asp:TableCell Style="">
                                  <asp:TextBox ID="TextBox_id_archivo" runat="server" MaxLength="9" ReadOnly="true" BackColor="Gray" class="mt-1"></asp:TextBox>
                                  <asp:Button ID="Button_activa_archivar_unidad" runat="server" Text="Archivar" ToolTip="Archiva unidad documental en unidad contenedora" CssClass="btn btn-success m-1" style="font-size:10px"  OnClientClick="auto_zise_reasigna_expe_unidad();" /> 
                                  <asp:Button ID="Button_des_archivar" runat="server" Text="Desarchivar" CssClass="btn btn-success m-1" style="font-size:10px" ToolTip="Desarchiva unidad documental de unidad contenedora"/>  
                                  <input id="HiddenField_estado_ubicacion" type="hidden" value="0" runat="server">
                                  <input id="Hidden_tipo_unidad" type="hidden" value="0" runat="server">
                              </asp:TableCell>
                          </asp:TableRow>
                      </asp:Table>

                  </asp:Panel>

                  <div id="contenedor_botones_postbak" style="display: none">
                      <asp:Button ID="Button_lista_ayuda_expediente" runat="server" Text="" Style="height: 1%" />
                      <asp:Button ID="Button_selecion_organigrama" runat="server" Text="" Style="height: 1%" />
                      <asp:Button ID="Button_selecion_area" runat="server" Text="" Style="height: 1%" />
                      <asp:Button ID="Button_seleccion_sub_area" runat="server" Text="" Style="height: 1%" />
                      <asp:Button ID="Button_selecion_serie" runat="server" Text="" Style="height: 1%" />
                      <asp:Button ID="Button1_seleccion_expediente_manual" runat="server" Text="" Style="height: 1%" />
                  </div>

              </ContentTemplate>
              <Triggers>
              </Triggers>
          </asp:UpdatePanel>
              <asp:Label ID="Labelresultado" runat="server" Text="" Style="float: right; font-size: 11px"></asp:Label>
          </div>
          <div id="contenido_botonoes" style="" class="border_inferior_radius_blanco modal-footer">
              <asp:UpdatePanel ID="Updatepanel_botones" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                  <ContentTemplate>
                      <asp:Button ID="ButtonAceptar" runat="server" Text="Registrar" ToolTip="Agregar expediente" CssClass="btn btn-success"  />
                      <asp:Button ID="ButtonRestaurar" runat="server" Text="Restaurar" ToolTip="Restaurar"  CssClass="btn btn-success" />
                      <asp:Button ID="Button_configurar_rotulo" runat="server" Text="Plantilla" ToolTip="Selecciona la plantilla para la impresión del rotulo"  CssClass="btn btn-success" />
                  </ContentTemplate>
              </asp:UpdatePanel>
          </div>
       </div>
       <div id="Impresion_post">
            <asp:Panel ID="Panelimpresionpost" runat="server" Style="display: none; width: auto; height: auto" CssClass="modal_content_general">
                <asp:DragPanelExtender ID="DragPanelExtenderimpre_post" runat="server" TargetControlID="Panelimpresionpost" />
                <asp:ModalPopupExtender ID="ModalPopupExtenderimpre_post" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_post"
                    PopupControlID="Panelimpresionpost" CancelControlID="Buttoncerrarimpre_post">
                </asp:ModalPopupExtender>
                <div id="modal_content_impre_post" class="modal-content">
                    <div id="divcabecer2_post" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title d-inline ml-1">Menú Impresión</h6>
                         <button type="button" value="Buttoncerrarimpre_post" class="close da_event_captive">&times;</button>   
                    </div>
                    <asp:UpdatePanel ID="UpdatePaneliframe_post" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div id="ContenidoImpresion_post" style="height: auto; width: auto ; border-top:none; overflow:auto" class="modal_content_back  pl-3 pr-3">
                                <iframe width="100%" height="100%" id="ifimpre_post_" runat="server" frameborder="0"></iframe>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button1_post" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" Style="display: none" />
                        <asp:Button ID="ButtonSalir_post" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" Style="display: none" />
                        <asp:Button ID="Buttoncerrarimpre_post" runat="Server" Text="X" CssClass="invisible" Style="display: none" />
                    </div>
                </div>
            </asp:Panel>
        </div>
        <!--archiva expediente-->
        <div id="modal_reubicar_unidad_expediente">
            <asp:Panel ID="Panel_reubicar_unidad_expediente_popup" runat="server"  Style="display:none; width: 100%; height: 99%" CssClass="modal_content_general">             
                <asp:ModalPopupExtender ID="ModalPopupExtende_reubicar_unidad_expediente_popup" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_reubicar_unidad_expediente_popup"
                    PopupControlID="Panel_reubicar_unidad_expediente_popup" CancelControlID="Buttoncerrar_reubicar_unidad_expediente_popup" ></asp:ModalPopupExtender>
                <div id="modal_content_reubicar_unidad_expediente_popup" class="modal-content_">
                    <div id="divcabecer_reubicar_unidad_expediente_popup" class="modal_title_superior_ modal-header">
                         <h6 class="modal-title d-inline ml-1">Archivar unidad documental</h6>
                         <button type="button" value="Buttoncerrar_reubicar_unidad_expediente_popup" class="close da_event_captive">&times;</button>       
                    </div>
                    <div id="Contenido_reubicar_unidad_expediente_popup" style=" height: 97%; width: 100%; border-top:none" class="modal_content_back pl-1 pr-1 pt-1">
                        <div id="drowlist_r_u_e" style="">
                            <asp:UpdatePanel ID="UpdatePanelEntidad_r_u_e" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:DropDownList ID="DropDownListEntidadEmpresa_r_u_e" runat="server" Width="100%" CssClass="custom-select" onchange="buton_click('Button_listar_edificio');"></asp:DropDownList>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                         <div id="div_treview_archivo_r_u_e" style="height: 80%; margin-top: 5px">
                            <asp:Panel ID="Paneltreview_r_u_e" runat="server" ScrollBars="Both"
                                Height="100%" Width="100%" Style="position: inherit">
                                <asp:UpdatePanel ID="UpdatePanelViewArchivo_r_u_e" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:TreeView ID="TreeViewArchivo_r_u_e" Style="text-align: left; padding-left: 1px; font-size: 10px; margin-top: 0px" runat="server" CssClass="TreeN" NodeWrap="true"
                                            PopulateNodesFromClient="False" EnableViewState="true"
                                            LeafNodeStyle-CssClass="LeafNodeStyle" Font-Size="11px" NodeIndent="10" ExpandDepth="0" SkipLinkText="">
                                            <HoverNodeStyle Font-Underline="False" />
                                            <LeafNodeStyle CssClass="LeafNodeStyle" HorizontalPadding="10px" NodeSpacing="0px" VerticalPadding="5px" />
                                            <NodeStyle ChildNodesPadding="5px" HorizontalPadding="0px" NodeSpacing="5px" VerticalPadding="5px" ForeColor="Black" />
                                            <ParentNodeStyle ChildNodesPadding="0px" ForeColor="#313131" Font-Bold="true" HorizontalPadding="5px" NodeSpacing="0px" VerticalPadding="5px" />
                                            <RootNodeStyle ChildNodesPadding="0px" ForeColor="#313131" Font-Bold="true" NodeSpacing="0px" VerticalPadding="5px" HorizontalPadding="5px" />
                                            <SelectedNodeStyle ForeColor="White" CssClass="node_select_" Font-Size="10px" ImageUrl="../workflow/imageneswf/iten_list_select.png" />
                                        </asp:TreeView>
                                    </ContentTemplate>
                                </asp:UpdatePanel>

                            </asp:Panel>
                        </div>         
                    </div>
                    <div id="contendor_botones_unidad_r_u_e" style="border-top:none" class="border_inferior_radius_blanco_ modal-footer">
                            <asp:UpdatePanel ID="UpdatePanel_botones_unidad_r_u_e" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Button ID="Button_actualizar_unidad" runat="server" Text="Editar" CssClass="btn btn-success" Style="display: none" />
                                    <asp:Button ID="Button_agrega_unidad_conservacion_interface" runat="server" Text="Archivar" ToolTip="Archivar unidad documental en unidad contenedora" CssClass="btn btn-success" Style=" display: none" />
                                    <asp:Button ID="Button_archivar" runat="server" Text="Archivar" CssClass="btn btn-success" Style="margin-left: 10px" />
                                    <asp:Button ID="Button_agrega_unidad_contenedora" runat="server" Text="Agregar" ToolTip="Agregar una unidad contenedora de unidad documental" CssClass="btn btn-success" Style=" margin-left: 10px" />
                                    <asp:Button ID="ButtonButtonEditar" runat="server" Text="Editar" CssClass="btn btn-success" ToolTip="Editar unidad contendora de unidad documental" />
                                    <asp:Button ID="ButtonEliminar_unidad_contendora" runat="server" Text="Eliminar" ToolTip="Eliminar unidad contendora de unidad documental" CssClass="btn btn-success" OnClientClick="ConfirmMensajeGeneral('Desea eliminar la unidad contenedora ?','Hidden_result_eliminar');" />
                                    <input id="Hidden_result_eliminar" type="hidden" value="0" runat="server">
                                    <input id="Hidden_tipo_unidad_seleccion" type="hidden" value="0" runat="server">
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                </div>
                <div style="display: none; height: 1px">
                    <asp:Button ID="Button_reubicar_unidad_expediente_popup" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                    <asp:Button ID="ButtonSalir_reubicar_unidad_expediente_popup" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                    <asp:Button ID="Buttoncerrar_reubicar_unidad_expediente_popup" runat="Server" Text="" CssClass="invisible" />
                </div>
            </asp:Panel>
            
        </div>
        <!--agregar unidad de conservacion-->
        <div id="modal_agregar_unidad_conservacion">
                <asp:Panel ID="Panel_agregar_unidad_conservacion_popup" runat="server"  Style="display:none; color: White; width: 100%; height: 98%" >       
                    <asp:ModalPopupExtender ID="ModalPopupExtende_agregar_unidad_conservacion_popup" runat="Server" Y="0" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_agregar_unidad_conservacion_popup"
                        PopupControlID="Panel_agregar_unidad_conservacion_popup" CancelControlID="Buttoncerrar_agregar_unidad_conservacion_popup" >
                    </asp:ModalPopupExtender>
                    <div id="divcabecer_agregar_unidad_conservacion_popup" class="modal_title_superior" style="display:none">
                        <asp:Label ID="Label_agregar_unidad_conservacion_popup" runat="server" Text="Gestión unidad contendora" Font-Size="10" Style="float: left">
                        </asp:Label>
                        <div id="Divcerrarbuton_agregar_unidad_conservacion_popup" style="float: right">
                             
                                     <asp:Button ID="Buttoncerrar_agregar_unidad_conservacion_popup" runat="Server" Text="X" CssClass="modal_boton_hiden"
                                         ToolTip="Cerrar ventana"  />
                                 
                        </div>
                    </div>  
                     <div id="Contenido_agregar_unidad_conservacion_popup" style=" color: black; background-color:white; height: 100%; width: 100%; padding-top:10px" class="modal_content_general">
                         <asp:UpdatePanel ID="UpdatePanel_agregar_unidad_conservacion_popup" runat="server" UpdateMode="Conditional" style="height:100%" RenderMode="Inline">
                             <ContentTemplate>
                            <iframe  id="Iframe_agregar_unidad_conservacion_popup"  runat="server"  style="width:100%; height:100%" frameborder="0"></iframe>                
                                 <asp:HiddenField ID="HiddenField1" runat="server" value="0"/>
                                 </ContentTemplate>
                             </asp:UpdatePanel>
                        </div>  
                   
                </asp:Panel>
                <asp:Button ID="Button_agregar_unidad_conservacion_popup" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                        <asp:Button ID="ButtonSalir_agregar_unidad_conservacion_popup" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
        </div>
        <!--configura_plantilla_rotulo-->
          <div id="configura_plantilla_rotulo">
            <asp:Panel ID="Panel_configura_plantilla_rotulo" runat="server" Style="display:none; width:40%; height:auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_configura_plantilla_rotulo" runat="server" BehaviorID="Panel_configura_plantilla_rotulo" TargetControlID="ButtonSalir_configura_plantilla_rotulo" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_configura_plantilla_rotulo" PopupControlID="Panel_configura_plantilla_rotulo" ></asp:ModalPopupExtender>
                <div id="modal_content_configura_plantilla_rotulo" class="modal-content">
                    <div id="divcabecer2_configura_plantilla_rotulo" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title d-inline ml-1">Seleccionar plantilla</h6>
                        <button type="button" value="Button_cerrar_configura_plantilla_rotulo" class="close da_event_captive">&times;</button>  
                    </div>
                    <div id="contenido_procesa_configura_plantilla_rotulo" style="border-top:none; overflow:auto" class="modal_content_back pl-3 pr-3">
                        <asp:UpdatePanel ID="UpdatePanel_configura_plantilla_rotulo" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div style="text-align: center">
                                    <br />
                                    <span></span>                           
                                    <br />
                                    <asp:DropDownList ID="DropDownList_configura_plantilla_rotulo" runat="server" CssClass="mt-1 custom-select"></asp:DropDownList>
                                    <br />
                                    <br />
                                    <asp:Button ID="Button_aceptar_configura_plantilla_rotulo" runat="server" Text="Aceptar" CssClass="btn btn-success mb-4" />
                                 
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                    </div>
                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button_configura_plantilla_rotulo" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                        <asp:Button ID="ButtonSalir_configura_plantilla_rotulo" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                        <asp:Button ID="Button_cerrar_configura_plantilla_rotulo" runat="Server" Text="X" CssClass="invisible" />
                    </div>
                </div>
            </asp:Panel>
        </div>
        <!-- Modal ayuda -->
        <div id="myModal" class="modal-ayuda" style="display: none">
            <div id="mytexto_" class="modal-content-ayuda" style="overflow:auto">
                <span class="close" onclick="hide_autonomo();">&times;</span>
                <p id="tex_modal" style="justify-content: center; font-family: Arial; font-size: 11px; text-align: justify">Some text in the Modal..</p>
            </div>

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
