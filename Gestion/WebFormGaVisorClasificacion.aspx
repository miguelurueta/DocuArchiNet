<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormGaVisorClasificacion.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormGaVisorClasificacion" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
 <title>Administración clasificación</title>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>   
      <script src="../js/ui/jquery-3.4.1.min.js"></script>
    <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
     <link href="../ccs/themes/base/jquery.ui.all.css" rel="stylesheet" type="text/css" />
     <link href="../Styles/styleMenu.css" rel="stylesheet" type="text/css" /> 
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
     <link href="../Styles/Menu3.css" rel="stylesheet" /> 
      <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <script src="../js/gestion/WebFormGaVisorClasificacion.js"></script>
    <script src="../js/java_general/general_code_java.js"></script>
    <script defer src="../Awesome/js/all.js"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet">
  <link href="../Awesome/css/brands.css" rel="stylesheet">
  <link href="../Awesome/css/solid.css" rel="stylesheet">
    <script defer src="../Awesome/js/brands.js"></script>
  <script defer src="../Awesome/js/solid.js"></script>
  <script defer src="../Awesome/js/fontawesome.js"></script>
</head>
<body style="margin-top:0px">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="True" 
            EnableScriptGlobalization="True"  AsyncPostBackTimeout="900">
        </asp:ScriptManager>
         <script accesskey="javascript" type="text/javascript">
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
                 var elmen = document.getElementById(elment_postbak.id)
                 if (elmen.type == "button" || elmen.type == "submit") {
                     value_element = elmen.value;
                     elmen.value = "Espere..."
                     elmen.disabled = true;
                 }
                 posicion_update_pogres('progres_bar');
                 

             }
             function CheckStatus(sender, args) {
                 try {
                     $("#noaming").bind("contextmenu", function (e) {
                         e.preventDefault();
                     });
                     if (elment_postbak.type == "button" || elment_postbak.type == "submit") {
                         elment_postbak.value = value_element;
                         elment_postbak.disabled = false;
                     }
                     
                     if (elment_postbak.id == "ImageButton_toponimica") {
                         auto_zise_ubicacion_toponimica();
                     }
                     if (elment_postbak.id == "ImageButtonindice") {
                        
                     }
                 } 
                 catch (ex) {
                     alert("Inconsistencia general funcion WebFormGaVisorClasificacion " + ex.message)
                 } finally {
                     progres_hiden('progres_bar');
                 }
             }
            
            </script>
        <div>
    
            <div id="Contendor_derecho" style="width:100%; height:100%; position:relative">
                <div id="tollimage" style="width: 100%; border-bottom: 1px solid #ddd; background: #6d7fcc; display: inline-flexbox; min-height:50px" class="navbar navbar-expand-sm   pb-0 pl-0 pt-1">
                    <button class="navbar-toggler btn btn-light mb-2 ml-2 mr-2" style="padding-bottom: 2px" type="button" data-toggle="collapse" data-target="#navbarNavDropdown">
                        <span class=""><i style="color: white" class="fas fa-bars"></i></span>
                    </button>
                    <div class="collapse navbar-collapse   pb-1 pt-0" id="navbarNavDropdown">
                        <div class="nav  ">
                            <div class="nav-item active active_azul">
                                <a class="nav-link active_azul ml-1" title="Primera  imagen (tecla D)" href="#" onclick="activa_boton_client_server('ImageButtonInicio')"><i style="font-size: 20px; color: white" class="fad fa-arrow-alt-to-left  active_azul"></i></a>
                            </div>
                            <div class="nav-item active active_azul">
                                <a class="nav-link  active_azul" style="margin-left: 2px; margin: 2px; width: auto; color: black" title="Anterior imagen (Tecla A)" href="#" onclick="activa_boton_client_server('ImageButtonAnterior')"><i style="color: white; font-size: 20px" class="fad fa-arrow-alt-left fa-1x"></i></a>
                            </div>
                            <asp:UpdatePanel ID="UpdatePanel_conte_bot" runat="server"
                                UpdateMode="Conditional" RenderMode="Inline">
                                <ContentTemplate>
                                    <div class="nav-item active ">
                                        <asp:TextBox ID="LabelConteo" runat="server" ToolTip="Para busqueda digite número y presione tecla enter" CssClass="mt-1" Style="margin-left: 5px; margin-right: 5px; text-align: center; margin-top: 3px; font-size: 12px; width: 50px; font-family: 'Segoe UI Emoji'" onkeypress="preven_event_search_keypres_enter(event,this);"></asp:TextBox>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <div class="nav-item active  active_azul">
                                <a class="nav-link active_azul" style="" title="Siguiente imagen (tecla S)" href="#" onclick="activa_boton_client_server('ImageButtonSiguiente')"><i style="color: white; font-size: 20px" class="fad fa-arrow-alt-right fa-1x"></i></a>
                            </div>

                            <div class="nav-item active active_azul">
                                <a class="nav-link active_azul" style="color: white" title="Ultima  imagen (tecla F)" href="#" onclick="activa_boton_client_server('ImageButtonFinal')"><i style="color: white; font-size: 20px" class="fad fa-arrow-alt-to-right fa-1x"></i></a>
                            </div>

                            <div class="nav-item active active_azul">
                                <a class="nav-link active_azul" style="color: white" title="Alejar Imagen (tecla -)" href="#" onclick="activa_boton_client_server('ImageMenos')"><i style="color: white; font-size: 20px" class="fad fa-minus-circle fa-1x"></i></a>
                            </div>

                            <div class="nav-item active active_azul">
                                <a class="nav-link active_azul" style="color: white" title="Acercar Imagen (tecla +) " href="#" onclick="activa_boton_client_server('ImageMas') "><i style="color: white; font-size: 20px" class="fad fa-plus-circle fa-1x"></i></a>
                            </div>
                            <asp:UpdatePanel ID="UpdatePanel_drows_bot" runat="server"
                                UpdateMode="Conditional" RenderMode="Inline">
                                <ContentTemplate>
                                    <asp:DropDownList ID="DropDownList_zom" runat="server" AutoPostBack="True" class="mt-1 mr-1 ml-1">
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
                            <div class="nav-item_ active active_azul">
                                <a class="nav-link active_azul" id="A1" style="" title="Descargar imagenes (tecla G)" href="#" onclick="activa_boton_client_server('ImageButtonguardardocumento')"><i style="color: white; font-size: 20px" class="fad fa-arrow-to-bottom fa-1x"></i></a>
                            </div>
                            <asp:TextBox ID="TextBox_ir_documento" Style="display: none" class=" mr-0 ml-0" Width="74px" runat="server" placeholder="" onkeypress="preven_event_search_keypres_enter(event,this);"></asp:TextBox>
                            <div class="nav-item_ active active_azul" style="display: none">
                                <a class="nav-link active_azul" style="" title="Ir a imagen" href="#" onclick="activa_boton_client_server('ImageButton_ir_pagina')"><i style="color: white; font-size: 20px" class="fas fa-search fa-1x"></i></a>
                            </div>
                            <div class="nav-item_ active active_azul">
                                <a class="nav-link active_azul" style="color: white; font-family: Arial; text-decoration: none; font-weight: 600" title="Imprimir documento (tecla P)" href="#" onclick="activa_boton_client_server('ImageButtonimprimir')"><i style="color: white" class="far fa-print fa-1x"></i></a>
                            </div>
                            <div class="nav-item_ active active_azul">
                                <a class="nav-link active_azul" style="color: white; font-family: Arial; text-decoration: none; font-weight: 600" title="Rotar 90 grados a la izquierda (tecla R)" href="#" onclick="activa_boton_client_server('ImageRotate45')"><i style="color: white" class="fad fa-undo fa-1x"></i></a>
                            </div>
                            <div class="nav-item_ active active_azul">
                                <a class="nav-link active_azul" style="color: white; font-family: Arial; text-decoration: none; font-weight: 600" title="Ubicación (tecla E)" href="#" onclick="activa_boton_client_server('ImageButton_toponimica')"><i style="color: white" class="far fa-info-circle fa-1x"></i></a>
                            </div>
                            <div class="nav-item_ active active_azul">
                                <a id="id_a_indice" class="nav-link active_azul" style="color: white; font-family: Arial; text-decoration: none; font-weight: 600" title="Visualiza indice documento (tecla I)" href="#" onclick="event_click_indice(event,this);"><i id="id_indice_image" style="color: white" class="far fa-info fa-1x"></i></a>
                            </div>
                        </div>
                    </div>    
                    <div style="display: none">
                        <asp:UpdatePanel ID="UpdatePanelButon" runat="server"
                            UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <asp:ImageButton ID="ImageButtonInicio" runat="server" ToolTip="Primera  imagen" ImageUrl="../imagewf/inicio14.png" Style="display: none" />     
                                <asp:ImageButton ID="ImageButtonAnterior" runat="server" ToolTip="Anterior imagen" ImageUrl="../imagewf/anterior15.png" Style="display: none" />                 
                                <asp:ImageButton ID="ImageButtonSiguiente" runat="server" ToolTip="Siguiente imagen" ImageUrl="../imagewf/siguiente15.png" ImageAlign="NotSet" Style="display: none" />     
                                <asp:ImageButton ID="ImageButtonFinal" runat="server" ToolTip="Ultima  imagen" ImageUrl="../imagewf/final15.png" Style="display: none" />   
                                <asp:ImageButton ID="ImageMenos" runat="server" ToolTip="Alejar Imagen" ImageUrl="../imagewf/alejarimagen.png" Style="display: none" />   
                                <asp:ImageButton ID="ImageMas" runat="server" ToolTip="Acercar Imagen" ImageUrl="../imagewf/acercarimagen.png" Style="display: none" />
                                <asp:ImageButton ID="ImageRotate45" runat="server" ToolTip="Rotar 90 grados" ImageUrl="../Docuarchi/imagenes/rotar90.png" Style="display: none" />
                                <asp:ImageButton ID="ImageRotate180" runat="server" ToolTip="Rotar 180 grados" ImageUrl="../Docuarchi/imagenes/rotar180.png" Style="display: none" />
                                <asp:ImageButton ID="ImageRotate270" runat="server" ToolTip="Rotar 270 grados" ImageUrl="../Docuarchi/imagenes/rotar270.png" Style="display: none" />
                                <asp:ImageButton ID="ImageButtonguardardocumento" runat="server" ToolTip="Guadar documento" ImageUrl="../Docuarchi/imagenes/guardarimagen.png" Visible="true" Style="display: none" />            
                                <asp:ImageButton ID="ImageButtonindice" runat="server" ToolTip="Visualiza indice documento" ImageUrl="../Docuarchi/imagenes/indice222.png" Visible="true" Style="display: none" OnClientClick=" visualiza_indice_documento(1);" />    
                                <asp:ImageButton ID="ImageButtonimprimir" runat="server" ToolTip="Imprimir documento" ImageUrl="../Docuarchi/imagenes/imprimir30.png" Style="display: none" />              
                                <asp:ImageButton ID="ImageButton_ir_pagina" runat="server" ToolTip="Ir a página" ImageUrl="../Docuarchi/imagenes/busca_pagina.png" Visible="true" Style="display: none" />                       
                                <asp:ImageButton ID="ImageButton_toponimica" runat="server" ToolTip="Ubicación toponimica" ImageUrl="../Docuarchi/imagenes/infoimagen.png" Visible="true" Style="margin-top: 0px; display: none" />
                                <input id="Hidden_ruta_archivo" type="hidden" value="" runat="server" />
                                <input id="Hidden_id_imagen" type="hidden" value="-1" runat="server" />
                                <input id="Hidden_gabinete" type="hidden" value="" runat="server" />                   
                            </ContentTemplate>
                            <Triggers>
                            </Triggers>
                        </asp:UpdatePanel>
                    </div>
                  
                </div>
                
                <div id="cuerpoindice"
                    style="width: 0%; left: auto; display: none; float:right; height: auto; background-color: white; width: 310px">
                    <asp:UpdatePanel ID="UpdatePanelindice_visor" runat="server" UpdateMode="Conditional"
                        RenderMode="Inline">
                        <ContentTemplate>
                            <iframe id="ifrm_indice_visor_docuarchi_" runat="server" style="border-style: none; left: 1px; width: 100%; height: 99%; position: relative; top: 1px; background-color: white; float: right; padding-left:5px"
                                frameborder="0" scrolling="no"></iframe>
                            <input id="Hidden_result_indice_" type="hidden" value="0" runat="server" />

                        </ContentTemplate>
                        <Triggers>
                        </Triggers>
                    </asp:UpdatePanel>
                    &nbsp;&nbsp;&nbsp; 
                </div>
                <div id="content" style="width:100%; height: 380px; position: absolute; background-color: Gray; filter: alpha(opacity=70); opacity: 50; overflow: scroll; border-style: ridge; border-bottom-width: 0.5px; border-left-width: 1px; border-right-width: 1px; border-top-width: 1px; float:left">
                    <div id="zona" style="width: 99.5%; height: auto; position: absolute;">
                        <asp:UpdatePanel ID="UpdatePanelvisor" runat="server" UpdateMode="Conditional" RenderMode="Inline" >
                            <ContentTemplate>

                                <neoimg:ImageDraw ID="noaming" runat="server" Style="position: relative" RenderingMethod="HttpHandler" HttpHandlerName="ImageGenerator.axd">
                                </neoimg:ImageDraw>

                            </ContentTemplate>
                            <Triggers>
                            </Triggers>
                        </asp:UpdatePanel>
                    </div>
                    <input id="Hiddenintercambio" type="hidden" value="0" runat="server"/>
                    <input id="Hiddenintercambio2" type="hidden" value="0" runat="server"/>
                    <input id="Hidden_id_tarea_sel" type="hidden" value="-1" runat="server"/>
                    <input id="Hidden_tipo_visor" type="hidden" value="" runat="server"/>
                </div>
               
                <div id="Pietolbar" style="width: 99%; height: 10px; position: absolute"></div>
               
            </div>
             <div id="div_contendor_externo" style="height: 100%; width: 100%; float: right; display:none">
             <asp:UpdatePanel ID="UpdatePanel_ifr_visor" runat="server" UpdateMode="Conditional"
                  RenderMode="Inline">
                  <ContentTemplate>
                      <input id="Hidden_tipo_visor_externo" type="hidden" value="0" runat="server"/>
                      <iframe id="ifrm_visor_" runat="server" style="border-style: none; left: 3px; width: 99%; height: 99%; position: relative; top: 0px"
                          frameborder="0" scrolling="no"></iframe>

                  </ContentTemplate>
                  <Triggers>
                      
                  </Triggers>

              </asp:UpdatePanel>
      </div>
    </div>
           <div id="Impresion_post">
            <asp:Panel ID="Panelimpresionpost" runat="server"  Style="display:none; color: White; width: 60%; height: 60%" CssClass="modal_content_general">
                <asp:DragPanelExtender ID="DragPanelExtenderimpre_post" runat="server" TargetControlID="Panelimpresionpost" />
                <asp:ModalPopupExtender ID="ModalPopupExtenderimpre_post" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_post"
                    PopupControlID="Panelimpresionpost" CancelControlID="Buttoncerrarimpre_post">
                </asp:ModalPopupExtender>
                <div id="divcabecer2_post" class="modal_title_superior">  
                     <h6 class="modal-title d-inline ml-1">Menú Impresión</h6>
                     <button type="button" value="Buttoncerrarimpre_post" class="close da_event_captive mr-2">&times;</button>                                
                </div>
                <asp:UpdatePanel ID="UpdatePaneliframe_post" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div id="ContenidoImpresion_post" style="color: black; background-color: #FFFFFF; border-bottom-right-radius:10px; border-bottom-left-radius:10px" class="modal_content_back pr-2 pl-2">
                            <iframe width="100%" height="100%" id="ifimpre_post_" runat="server" src="../Radicador/WebFormDaImprimir.aspx" frameborder="0"></iframe>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <div style="display:none; height:1px">
                     <asp:Button ID="Button1_post" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" style="display:none" />
                     <asp:Button ID="ButtonSalir_post" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" style="display:none"/>
                     <asp:Button ID="Buttoncerrarimpre_post" runat="Server" Text="X" CssClass="invisible" Height="1px" Width="1px" style="display:none"/>
                </div>
                    
            </asp:Panel>
        </div>
       <!--POPUP EXPORTAR DOCUMENTO-->
        <div id="Divdescarga_anexo_respuesta">
            <asp:Panel ID="Panel_descarga_anexo_respuesta" runat="server" Style="display:none; color: White; width:50%; height:auto" CssClass="modal_content_general_">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_descarga_anexo_respuesta" runat="server" BehaviorID="Panel_descarga_anexo_respuesta_ModalPopupExtender" TargetControlID="ButtonSalir_descarga_anexo_respuesta" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_descarga_anexo_respuesta" PopupControlID="Panel_descarga_anexo_respuesta"></asp:ModalPopupExtender>
                <div id="div_title_descarga" class="modal_title_superior">
                    <h6 class="modal-title d-inline ml-1">Descarga documento</h6>
                    <button type="button" value="Button_cerrar_descarga_anexo_respuesta" class="close da_event_captive mr-2">&times;</button>                                      
                </div>
                <div id="contenido_procesa_descarga_anexo_respuesta" style="width:100%; border-bottom-right-radius:10px; border-bottom-left-radius:10px" class="modal_content_back pr-2 pl-2">       
                    <asp:UpdatePanel ID="UpdatePanel_descarga_anexo_respuesta" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <iframe   id="ifimpre_descarga_anexo_respuesta_" runat="server" style="width :100%; height:100%" frameborder="0"   ></iframe>
                            
                        </ContentTemplate>
                    </asp:UpdatePanel>
            
                    <div style="display:none; height:1px">
                         <asp:Button ID="ButtonSalir_descarga_anexo_respuesta" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" style="display:none" /> 
                          <asp:Button ID="Button_cerrar_descarga_anexo_respuesta" runat="Server" Text="" CssClass="invisible"  Height="1px" Width="1px" style="display:none"
                              />  
                    </div>  
                    
                </div>
           </asp:Panel>
        </div>  
         <!--POPUP INDICE DE DOCUMENTO-->
        <asp:Panel ID="Panel_indice" runat="server"  Style="display:none; color: White; width: 100%; height: 98%" CssClass="modal_content_general">
            <asp:ModalPopupExtender ID="ModalPopupExtenderimpre_indice" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_indice"
                    PopupControlID="Panel_indice" CancelControlID="Buttoncerrarimpre_indice" Y="1">
                </asp:ModalPopupExtender>
                <div id="divcabecer2__indice" class="modal_title_superior">
                    <asp:Label ID="Label_indice" runat="server" Text="Indice documento" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton2_indice" style="float: right">
                        <asp:Button ID="Buttoncerrarimpre_indice" runat="Server" Text="X"  CssClass="modal_boton_hiden"
                             ToolTip="Cerrar ventana" />
                    </div>
                </div>
              <asp:UpdatePanel ID="UpdatePanelindice" runat="server" UpdateMode="Conditional"
                  RenderMode="Inline">
                  <ContentTemplate>
                      <div id="content_indice_imagen" class="modal_content_back" style=" width: 100%; height:100%; position: relative; background-color:white">
                            <iframe id="ifrm_indice_indice_doc_" runat="server" style=" width: 100%; height:97%; position: relative; background-color:white"
                          frameborder="0" scrolling="no" ></iframe>
                      </div> 
                      <input id="Hidden_result_indice" type="hidden" value="0" runat="server"/>
                  </ContentTemplate>
                  <Triggers>
                      
                  </Triggers>
              </asp:UpdatePanel>
                 <asp:Button ID="Button1_indice" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" style="display:none" />
                    <asp:Button ID="ButtonSalir_indice" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" style="display:none"/>
             </asp:Panel>
           <!--ubicacion toponimica-->
        <div id="modal_ubicacion_toponimica_expediente">
            <asp:Panel ID="Panel_ubicacion_toponimica_expediente_popup" runat="server" Style="display:none; color: White; width: 50%; height: 99%">
                <asp:ModalPopupExtender ID="ModalPopupExtende_ubicacion_toponimica_expediente_popup" runat="Server" BackgroundCssClass="FondoAplicacion" 
                     TargetControlID="ButtonSalir_ubicacion_toponimica_expediente_popup"
                    PopupControlID="Panel_ubicacion_toponimica_expediente_popup" CancelControlID="Buttoncerrar_ubicacion_toponimica_expediente_popup" Y="0"></asp:ModalPopupExtender>
                <div id="divcabecer_ubicacion_toponimica_expediente_popup" class="modal_title_superior" >
                      <h6 class="modal-title d-inline ml-1">Ubicación toponimica</h6>
                     <button type="button" value="Buttoncerrar_ubicacion_toponimica_expediente_popup" class="close da_event_captive mr-2">&times;</button>
                   
                  
                    
                </div>
                <div id="Contenido_ubicacion_toponimica_expediente" style=" color: black; background-color: #FFFFFF; height: 97%; width: 100%" class="modal_content_back">
                    <div id="div_treview_archivo_u_b_t" style="height: 100%">
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
                    <div id="contendor_botones_unidad_u_b_t" style="height: 10%; background-color:white; border:1px solid #ccc" class="border_inferior_radius_blanco">
                        <asp:UpdatePanel ID="UpdatePanel_botones_unidad_u_b_t" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Button ID="Button_exportar" runat="server" Text="Imprimir" CssClass="btn btn-success" Style="margin-left: 5px; margin-top:5px" OnClientClick="fnExcelTre('TreeViewArchivo_u_b_t')" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
                <div style="display: none; height: 1px">
                    <asp:Button ID="Button_ubicacion_toponimica_expediente_popup" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" Style="display: none" />
                    <asp:Button ID="ButtonSalir_ubicacion_toponimica_expediente_popup" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" Style="display: none" />
                    <asp:Button ID="Buttoncerrar_ubicacion_toponimica_expediente_popup" runat="Server" Text="" Height="1px" Width="1px" Style="display: none" />
                </div>
                
            </asp:Panel>
        </div>   
        <div id="tol_pie" style=" float:right;  background-color:#E7EDF5; width:100%; height:3%;border-style: ridge; border-bottom-width: 0.5px; border-left-width: 1px; border-right-width: 1px; border-top-width: 1px;text-align:center; display:none"">
                 <asp:Label ID="Label5" runat="server" Text="Estado" style="font-family:Arial;font-size:11px; display:none"></asp:Label>
                    <asp:UpdatePanel ID="updatapanel_iframe" runat="server" UpdateMode="Conditional" >
                            <ContentTemplate>
                                  <iframe runat="server" style="float:left" id="ifmExcel_" width="0" height="0" marginheight="0" marginwidth="0"
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