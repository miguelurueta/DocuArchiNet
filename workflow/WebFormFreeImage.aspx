<%@ Page Language="vb" AutoEventWireup="false" EnableEventValidation="false" CodeBehind="WebFormFreeImage.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormFreeImage" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Neodynamic.WebControls.ImageDraw" Namespace="Neodynamic.WebControls.ImageDraw"
    TagPrefix="neoimg" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Gestor Documental visor Freeimage</title>
    <style type="text/css"> 
  #draggable {
    width: 100px;
    height: 100px;
    background: #ccc;
    
  }
   .invisible { 
            visibility: hidden; 
        } 
        
          .cabecera2{
    height : 20px;
    /*position : static;*/
    margin: 0px;
    padding: 0px;
    background: #053061;
    /*width: 100%;*/
    color:White;
    text-align:left;
	top: 0px;
	left: 0px;
     }
     .cabecera{
    height : 7%;
    /*position : static;*/
    margin: 0px;
    padding: 0px;
    background: #053061;
    width: 100%;
    color:White;
    text-align:left;
	top: 0px;
	left: 0px;
}
  </style>
     
     <script src="../js/ui/jquery-3.4.1.min.js"></script>  
    <script src="../js/jquery-ui-1.12.1.custom/jquery-ui.min.js"></script>
    <link href="../js/jquery-ui-1.12.1.custom/jquery-ui.min.css" rel="stylesheet" />
   <script src="../js/sizeimagejquery.js" type="text/javascript"></script> 
   <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
        <link href="../Styles/Aplicaction.css" rel="stylesheet" />
      <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <script src="../js/workflow/WebFormFreeImage.js"></script>
    <script src="../js/java_general/general_code_java.js?v=20260827-compatible-events5"></script>
    <script defer src="../Awesome/js/all.js"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet">
  <link href="../Awesome/css/brands.css" rel="stylesheet">
  <link href="../Awesome/css/solid.css" rel="stylesheet">
    <script defer src="../Awesome/js/brands.js"></script>
  <script defer src="../Awesome/js/solid.js"></script>
  <script defer src="../Awesome/js/fontawesome.js"></script>
   
</head>
<body >
    <form id="form1" runat="server" style="height:100%" >
         <asp:ScriptManager ID="ScriptManager1" runat="server"
              EnableScriptGlobalization="True" EnablePageMethods="True" AsyncPostBackTimeout="900">
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
                   if (elment_postbak.id == "Button_guardar_desicion") {

                       posicion_update_pogres_modal('progres_bar');

                   } else {
                       posicion_update_pogres('progres_bar');
                   }
                  
               }
               function CheckStatus(sender, args) {
                   $("#noaming").bind("contextmenu", function (e) {
                       e.preventDefault();
                   });
                   if (elment_postbak.type == "button" || elment_postbak.type == "submit") {
                       elment_postbak.value = value_element;
                       elment_postbak.disabled = false;
                   }
                   if (elment_postbak.id == "ImageButtonadjunta") {
                       resize_adjunta_documento();
                   }
                   
                   
                   
                   resize_adjunta_documento();
                   if (elment_postbak.id == "Button_guardar_desicion") {
                       if (document.getElementById("HiddenField_estado_guarda").value == "YES") {
                           document.getElementById("HiddenField_estado_guarda").value = "";
                           if (window.parent.document.getElementById("Button_guardar_desicion_fre_image")) {
                               window.parent.document.getElementById("Hidden_date_row").value = document.getElementById("Hidden_date_row_").value;
                               window.parent.document.getElementById("Hidden_tip_adjunt").value = document.getElementById("Hidden_tip_adjunt_").value;
                               window.parent.document.getElementById("Button_guardar_desicion_fre_image").click();
                           }

                       }
                   }
                   if (elment_postbak.id == "Button_guardar_automatico") {
                       if (document.getElementById("HiddenField_estado_guarda").value == "YES") {
                           document.getElementById("HiddenField_estado_guarda").value = "";
                           //if (window.parent.document.getElementById("Button_actualiza_trevie_seleccion")) {
                           //    window.parent.document.getElementById("Button_actualiza_trevie_seleccion").click();
                           //}

                       }
                   }
                   if (elment_postbak.id == "Button_examinar_archivo_lista_chequeo") {
                       if (document.getElementById("Hidden_list_cheo_acepta").value == "YES") {
                           document.getElementById("Hidden_list_cheo_acepta").value = "";
                           if (window.parent.document.getElementById("Button_actualiza_trevie_seleccion")) {
                               window.parent.document.getElementById("Button_actualiza_trevie_seleccion").click();
                           }

                       }

                   }
                  
                   if (elment_postbak.id == "ImageButtonimprimir") {
                       auto_zise_popup_impresion();
                   }
                  
                   if (elment_postbak.id == "ImageButtonguardardocumento") {
                       auto_zise_popup_guardar_documento();
                   }
                   if (elment_postbak.id == "Button_guardar_automatico") {
                       if (document.getElementById("Hidden_0002").value == "1") {
                           document.getElementById("Hidden_0002").value = "0";
                           auto_zise_popup_lista_chequeo("1");
                       }
                   }
                   if (elment_postbak.id == "Button_examinar") {
                       if (document.getElementById("Hidden_0002").value == "1") {
                           document.getElementById("Hidden_0002").value = "0";
                           auto_zise_popup_lista_chequeo("1");
                       }
                   }
                  
                   if (elment_postbak.id == "Button_Actualizar_Lista_chequeo") {
                       if (document.getElementById("Hidden_0002").value == "1") {
                           document.getElementById("Hidden_0002").value = "0";
                           auto_zise_popup_lista_chequeo("1");
                       }
                   }
                   if (elment_postbak.id == "Button_guardar_desicion") {
                       $("#progres_bar").removeClass("overlay_");
                       progres_hiden('progres_bar');
                   } else {
                       progres_hiden('progres_bar');
                   }
               }

           </script>
      <!--  <script type="text/javascript" src="../jsUpdateProgress.js"></script>-->
     <div id="ContentGeneral" style="width:100%; height:100%; position:absolute">
         <div id="tollimage" style="border-bottom: 1px solid #ddd; display: inline-flexbox; width:100%" class="navbar navbar-expand-sm   pb-0 pl-0 pt-1">
                            <button class="navbar-toggler btn btn-light " style="padding-bottom: 2px" type="button" data-toggle="collapse" data-target="#navbarNavDropdown">
                                <span class="pb-1"><i style="color: white" class="fas fa-bars"></i></span>
                            </button>
                            <div class="collapse navbar-collapse   pb-1 pt-0" id="navbarNavDropdown">
                                <div class="nav  ml-1">
                                    <div class="nav-item active active_ ">
                                        <a class="nav-link" style="color: #6d7fcc" title="Primera  imagen" href="#" onclick="activa_boton_client_server('ImageButtonInicio')"><i style="font-size: 20px" class="fad fa-arrow-alt-to-left  "></i></a>
                                    </div>
                                    <div class="nav-item active active_ ">
                                        <a class="nav-link  " style="margin-left: 2px; margin: 2px; width: auto; color: #6d7fcc" title="Anterior imagen" href="#" onclick="activa_boton_client_server('ImageButtonAnterior')"><i style=" font-size: 20px" class="fad fa-arrow-alt-left "></i></a>
                                    </div>
                                    <asp:UpdatePanel ID="UpdatePanel_conte_bot" runat="server"
                                        UpdateMode="Conditional" RenderMode="Inline">
                                        <ContentTemplate>
                                            <div class="nav-item active ">
                                                <asp:TextBox ID="LabelConteo" runat="server" Style="margin-left: 5px; margin-right: 5px; text-align: center; margin-top: 3px; font-size: 12px; width: 50px; font-family: 'Segoe UI Emoji'" onkeypress="preven_event_search_keypres_enter(event,this);"></asp:TextBox>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                    <div class="nav-item active  active_">
                                        <a class="nav-link " style="color: #6d7fcc" title="Siguiente imagen" href="#" onclick="activa_boton_client_server('ImageButtonSiguiente')"><i style=" font-size: 20px" class="fad fa-arrow-alt-right "></i></a>
                                    </div>

                                    <div class="nav-item active active">
                                        <a class="nav-link " style="color: #6d7fcc" title="Ultima  imagen" href="#" onclick="activa_boton_client_server('ImageButtonFinal')"><i style=" font-size: 20px" class="fad fa-arrow-alt-to-right "></i></a>
                                    </div>

                                    <div class="nav-item active active_">
                                        <a class="nav-link " style="color: #6d7fcc" title="Alejar Imagen" href="#" onclick="activa_boton_client_server('ImageMenos')"><i style=" font-size: 20px" class="fad fa-minus-circle "></i></a>
                                    </div>

                                    <div class="nav-item active active_">
                                        <a class="nav-link " style="color: #6d7fcc" title="Acercar Imagen" href="#" onclick="activa_boton_client_server('ImageMas') "><i style=" font-size: 20px" class="fad fa-plus-circle "></i></a>
                                    </div>
                                    <asp:UpdatePanel ID="UpdatePanel_drows_bot" runat="server"
                                        UpdateMode="Conditional" RenderMode="Inline">
                                        <ContentTemplate>
                                            <asp:DropDownList ID="DropDownList_zom" runat="server" AutoPostBack="True" class=" mr-1 ml-1">
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
                                   
                                    <div class="nav-item_ active active_">
                                        <a class="nav-link " style="color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600" title="Rotar 90 grados a la izquierda" href="#" onclick="activa_boton_client_server('ImageRotate45')"><i style="" class="fad fa-undo "></i></a>
                                    </div>
                                    <asp:TextBox ID="TextBox2" Style="display: none" class=" mr-0 ml-0" Width="74px" runat="server" placeholder="" onkeypress="preven_event_search_keypres_enter(event,this);"></asp:TextBox>
                                    <div class="nav-item_ active active_" style="display: none">
                                        <a class="nav-link " style="color: #6d7fcc" title="Ir a imagen" href="#" onclick="activa_boton_client_server('ImageButton_ir_pagina')"><i style="color: white; font-size: 20px" class="fas fa-search "></i></a>
                                    </div>
                                     <div class="nav-item_ active active_">
                                        <a class="nav-link " id="ImageButtonguardardocumento_" style="color: #6d7fcc" title="Descargar imagenes" href="#" onclick="activa_boton_client_server('ImageButtonguardardocumento')"><i style=" font-size: 20px" class="fad fa-arrow-to-bottom "></i></a>
                                    </div>
                                    <div class="nav-item_ active active_">
                                        <a class="nav-link " style="color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600" title="Imprimir documento" href="#" onclick="activa_boton_client_server('ImageButtonimprimir')"><i style="" class="fad fa-print "></i></a>
                                    </div>
                                    <div id="ImageFirma_" class="nav-item_ active active_">
                                        <a  class="nav-link " style="color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600" title="Firma Imagen" href="#" onclick="activa_boton_client_server('ImageFirma')"><i style="" class="fas fa-file-signature "></i></a>
                                    </div>
                                    <div id="ImageButtonadjunta_" class="nav-item_ active active_">
                                        <a  class="nav-link " style="color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600" title="Adjuntar documento" href="#" onclick="activa_boton_client_server('Button_tool_activa_sube_documento')"><i style="" class="fas fa-upload "></i></a>
                                    </div>
                                    <div class="nav-item_ active active_">
                                        <a id="A1" class="nav-link" style="color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600" title="Adjuntar documento desde servicio web" href="#" onclick="activa_boton_client_server('Button_tool_activa_sube_documento_automatico')"><i style="" class="fas fa-page-break"></i></a>
                                    </div>
                                </div>
                                <input id="Hidden_ruta_archivo" type="hidden" value="" runat="server" />
                            </div>
              </div>
            <asp:UpdatePanel ID="Updatepanel_boton_content" runat="server" UpdateMode="Conditional">
             <ContentTemplate>
                 <asp:ImageButton ID="ImageButtonInicio" runat="server" ToolTip="Primera  imagen" ImageUrl="../imagewf/inicio14.png" Style="display: none" />
                  <asp:ImageButton ID="ImageButtonAnterior" runat="server" ToolTip="Anterior imagen" ImageUrl="../imagewf/anterior15.png" Style="display: none" />
                  <asp:ImageButton ID="ImageButtonguardar" runat="server" ToolTip="Guardar firma" Style="display: none" />
                 <asp:ImageButton ID="ImageButtonSiguiente" runat="server" ToolTip="Siguiente imagen" ImageUrl="../imagewf/siguiente15.png" ImageAlign="NotSet" Style="display: none" />
                 <asp:ImageButton ID="ImageButtonFinal" runat="server" ToolTip="Ultima  imagen" ImageUrl="../imagewf/final15.png" Style="display: none" />
                 <asp:ImageButton ID="ImageButton_ir_pagina" runat="server" ToolTip="Ir a imagen" ImageUrl="../Docuarchi/imagenes/busca_pagina.png" Visible="true" Style="display: none" />
                 <asp:ImageButton ID="ImageMenos" runat="server" ToolTip="Alejar Imagen" ImageUrl="../imagewf/alejarimagen.png" Style="display: none" />
                 <asp:ImageButton ID="ImageMas" runat="server" ToolTip="Acercar Imagen" ImageUrl="../imagewf/acercarimagen.png" Style="display: none" />
                 <asp:ImageButton ID="ImageRotate45" runat="server" ToolTip="Rotar 90 grados" ImageUrl="../Docuarchi/imagenes/rotar90.png" Style="display: none" />
                  <asp:ImageButton ID="ImageRotate180" runat="server" ToolTip="Rotar 180 grados" ImageUrl="../Docuarchi/imagenes/rotar180.png" Style="display: none" />
                 <asp:ImageButton ID="ImageRotate270" runat="server" ToolTip="Rotar 270 grados" ImageUrl="../Docuarchi/imagenes/rotar270.png" Style="display: none" />
                 <asp:ImageButton ID="ImageButtonimprimir" runat="server" ToolTip="Imprimir documento" ImageUrl="../Docuarchi/imagenes/imprimir30.png" Visible="true" Style="display: none" />
                  <asp:ImageButton ID="ImageButtonguardardocumento" runat="server" ToolTip="Guadar documento" Style="display: none" ImageUrl="../Docuarchi/imagenes/guardarimagen.png" Visible="true" />
                 <asp:ImageButton ID="ImageButtoninfo" runat="server" ToolTip="Información Documento" ImageUrl="../Docuarchi/imagenes/infoimagen.png" Style="display: none" Visible="true" />
                 <asp:ImageButton ID="ImageFirma" runat="server" ToolTip="Firma Imagen" ImageUrl="../imagewf/firma.png" Style="display: none" OnClientClick="firma_mecanica();" />
                 <asp:ImageButton ID="ImageButtonadjunta" runat="server" ImageUrl="../imagewf/adjunta_image.png" ToolTip="Adjunta imagen a documento" Style="display: none" />
                 
             </ContentTemplate>
         </asp:UpdatePanel>     
         <div id="content" style="width: 100%; height: 88%; position: absolute; background-color: Gray; filter: alpha(opacity=70); opacity: 50; overflow: scroll; border-bottom-width: 0.5px; border-left-width: 1px; border-right-width: 1px; border-top-width: 2px; left: 0px; display: block">
             <div id="zona" style="width: auto; height: auto; position: absolute;">
                 <asp:UpdatePanel ID="UpdatePanelvisor" runat="server" UpdateMode="Conditional">
                     <ContentTemplate>
                         <neoimg:ImageDraw ID="noaming" runat="server" Style="position: relative" RenderingMethod="HttpHandler" HttpHandlerName="ImageGenerator.axd">
                         </neoimg:ImageDraw>
                     </ContentTemplate>
                     <Triggers>
                     </Triggers>
                 </asp:UpdatePanel>
             </div>

             <div id="draggable" class="ui-widget-content" style="background-color: Gray; display: none; position: absolute">

                 <img id="img" alt="Firma Mecanica"
                     align="bottom" style="border-style: none" />
             </div>
             <input id="Hiddenintercambio" type="hidden" value="0" runat="server"/>
             <input id="Hiddenintercambio2" type="hidden" value="0" runat="server"/>
         </div>
        
         <div id="Pietolbar" style="width:99%; height:7%; position:relative; margin-top:10px; display:none">
             <asp:UpdateProgress ID="UpdateProg1" DisplayAfter="0" runat="server">
                 <ProgressTemplate>
                     <div id="loadind" style="position:relative; float:right; text-align: center;">
                         <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
                         Processing ...
                     </div>
                 </ProgressTemplate>
             </asp:UpdateProgress>           
         </div>  
    </div>
         <div style="display:none">
             <asp:UpdatePanel ID="UpdatePanel_boton_tool" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                <ContentTemplate>
                    <asp:Button ID="Button_tool_adjunta_documento_relacionado" runat="server" Text="" />
                    <asp:Button ID="Button_tool_activa_sube_documento" runat="server" Text="" /> 
                    <asp:Button ID="Button_tool_activa_sube_documento_automatico" runat="server" Text="" /> 
                </ContentTemplate>
            </asp:UpdatePanel>
         </div>
        <div id="Impresion_post">
            <asp:Panel ID="Panelimpresionpost" runat="server"  Style="display:none;  width:80%; height: auto" CssClass="modal_content_general_">
                <asp:ModalPopupExtender ID="ModalPopupExtenderimpre_post" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_post"
                    PopupControlID="Panelimpresionpost" CancelControlID="Buttoncerrarimpre_post">
                </asp:ModalPopupExtender>
                <div id="modal_content_Panelimpresionpost" class="modal-content">
                    <div id="divcabecer2_post" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title d-inline ml-1">Menú Impresión</h6>
                        <button type="button" value="Buttoncerrarimpre_post" class="close da_event_captive">&times;</button>
                    </div>
                    <div id="ContenidoImpresion_post" style="border-top:none; overflow:auto; height: auto; width: 100%" class="modal_content_back_">
                        <asp:UpdatePanel ID="UpdatePaneliframe_post" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>

                                <iframe width="100%" height="100%" id="ifimpre_post_" frameborder="0" runat="server" src="../Radicador/WebFormDaImprimir.aspx"></iframe>

                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </asp:Panel>
            <div style="display:none; height:0px">
                 <asp:Button ID="Button1_post" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                 <asp:Button ID="ButtonSalir_post" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                 <asp:Button ID="Buttoncerrarimpre_post" runat="Server" Text="" CssClass="invisible"/>
            </div>
            
        </div>
        <div id="guardar_post">
            <asp:Panel ID="Panel_guardar" runat="server"  Style="display:none; width:80%; height: auto" CssClass="modal_content_general_">
                
                <asp:ModalPopupExtender ID="ModalPopupExtender_guardar" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_post_guardar"
                    PopupControlID="Panel_guardar" CancelControlID="Buttoncerrarimpre_post_guardar">
                </asp:ModalPopupExtender>
                <div id="modal_content_Panel_guardar" class="modal-content">
                    <div id="divcabecer2_post_guardar" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title d-inline ml-1">Guardar documento</h6>
                        <button type="button" value="Buttoncerrarimpre_post_guardar" class="close da_event_captive">&times;</button>   
                    </div>
                    <div id="Content_guardar_documento" style=" width: 100% ; border-top:none; overflow:auto" class="modal_content_back">
                        <asp:UpdatePanel ID="UpdatePane_guardar" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>

                                <iframe width="100%" height="100%" id="Iframe_guardar" runat="server" frameborder="0"></iframe>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div style="display: none; height: 1px">
                            <asp:Button ID="Button1_guardar" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                            <asp:Button ID="ButtonSalir_post_guardar" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                            <asp:Button ID="Buttoncerrarimpre_post_guardar" runat="Server" Text="X" CssClass="modal_boton_hiden" />
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </div>
        <div id="info_post">
            <asp:Panel ID="Panel_info" runat="server"  Style="display:none; color: White; width: 80%; height: auto; background-color: #FFFFFF" CssClass="modal_content_general">              
                <asp:ModalPopupExtender ID="ModalPopupExtender_info" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_post_info"
                    PopupControlID="Panel_info" CancelControlID="Buttoncerrarimpre_post_info">
                </asp:ModalPopupExtender>                 
                    <div style="background-color: #FFFFFF" class="modal_title_superior">
                         <asp:Label ID="Label_div" runat="server" Text="Info documento" Font-Size="10" Style="float: left ">
                        </asp:Label>
                         <asp:Button ID="Buttoncerrarimpre_post_info" runat="Server" Text="X" CssClass="modal_boton_hiden" style="float:right"
                             ToolTip="Cerrar ventana" />
                    </div>
                <asp:UpdatePanel ID="UpdatePane_info" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div id="ContenidoImpresion_info"  style="color: black; background-color: #FFFFFF; height:auto; width: 100%" class="modal_content_back">                  
                            <asp:TextBox ID="TextBox_info" class="content_option_selecion" runat="server" Style="width:98%; height:auto; font-family:Arial; font-size:12px; margin:2px;min-height:120px; overflow:scroll; background-color:white" TextMode="MultiLine"></asp:TextBox>         
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                  <asp:Button ID="Button1_info" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_post_info" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
            </asp:Panel>
        </div>
        <asp:UpdatePanel ID="updatapanel_iframe" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <iframe runat="server" id="ifmExcel_" width="0" height="0" marginheight="0" marginwidth="0"
                    frameborder="0" />
            </ContentTemplate>
        </asp:UpdatePanel>
         <!--Adjunta documento!-->
         <div id="seleccion_tipo_adjunto">
            <asp:Panel ID="Panel_seleccion_tipo_adjunto" runat="server"  Style="display:none; color: White;  height:auto; width:50%; margin:auto" CssClass="modal_content_general">              
                <asp:ModalPopupExtender ID="ModalPopupExtender_seleccion_tipo_adjunto" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_seleccion_tipo_adjunto"
                    PopupControlID="Panel_seleccion_tipo_adjunto" CancelControlID="Buttoncerrarimpre_seleccion_tipo_adjunto">
                </asp:ModalPopupExtender>
                <div id="title_desicion" class="modal_title_superior" style="text-align: left">
                    <asp:Label ID="Label4" runat="server" Text="Opciones para adjuntar el archivo" Style="font-family: Arial; font-size: 14px; display: none"></asp:Label>
                    <div id="Divcerrarbuton2_seleccion_tipo_adjunto" style="float: right">
                        <asp:Button ID="Buttoncerrarimpre_seleccion_tipo_adjunto" runat="Server" Text="X" CssClass="modal_boton_hiden"
                            ToolTip="Cerrar ventana" />
                    </div>
                </div>          
                <asp:UpdatePanel ID="UpdatePane_seleccion_tipo_adjunto" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div >
                            <div id="Contenido_seleccion_tipo_adjunto" style="color: black; background-color: #FFFFFF; height: auto; width:100%" class="modal_content_back">
                                <asp:MutuallyExclusiveCheckBoxExtender ID="MutuallyExclusive_anexo_radicado" runat="server" TargetControlID="Check_anexo_radicado"
                                    Key="radicado_"></asp:MutuallyExclusiveCheckBoxExtender>
                                <asp:MutuallyExclusiveCheckBoxExtender ID="MutuallyExclusive_relacionado_radicado" runat="server" TargetControlID="CheckBox_relacionado_radicado"
                                    Key="radicado_"></asp:MutuallyExclusiveCheckBoxExtender>
                                

                                <div id="campo_adjunta" Class="content_option_selecion" style="margin-left: 0px; margin: 5px; background-color:white">
                                    <asp:CheckBox ID="Check_anexo_radicado" runat="server" Text="Adjunto como parte del documento seleccionado" Checked="false" ForeColor="Black" Font-Size="10" Font-Names="Arial" Style="margin-left: 5px" Enabled="true" />
                                    <br />
                                    <asp:CheckBox ID="CheckBox_relacionado_radicado" runat="server" Text="Adjunto como documento relacionado " Checked="true" ForeColor="Black" Font-Size="10" Font-Names="Arial" Style="margin-left: 5px" />
                                </div>

                                <div id="Div_inferior" style="text-align: right; margin-top: 10px">
                                    <asp:Button ID="Button_examinar" runat="server" Text="Ajuntar" CssClass="boton_azul" Style="margin-right: 10px; margin-bottom:10px" />
                                    <asp:Button ID="Button_guardar_desicion" runat="server" Text="Guardar" CssClass="boton" Style="display: none" />
                                    <asp:HiddenField ID="HiddenField_estado_guarda" runat="server" Value="" />
                                    <asp:HiddenField ID="Hidden_date_row_" runat="server" Value="" />
                                    <asp:HiddenField ID="Hidden_tip_adjunt_" runat="server" Value="" />
                                    
                                </div>
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                 <asp:Button ID="Button1__seleccion_tipo_adjunto" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_seleccion_tipo_adjunto" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
            </asp:Panel>
        </div>
         <!--Adjunta documento automatico!-->      
            <asp:Panel ID="Panel_adjunta_autamatico_documento" runat="server" Style="display:none;  height:auto; width:80%" CssClass="modal_content_general_">              
                <asp:ModalPopupExtender ID="ModalPopupExtender_adjunta_autamatico_documento" runat="Server" BackgroundCssClass="FondoAplicacion" 
                     TargetControlID="ButtonSalir_adjunta_autamatico_documento"
                    PopupControlID="Panel_adjunta_autamatico_documento" CancelControlID="Buttoncerrarimpre_adjunta_autamatico_documento">
                </asp:ModalPopupExtender>    
                 <div id="modal_content_adjunta_autamatico_documento" class="modal-content">        
                    <div id="Divcerrarbuton2_adjunta_autamatico_title"  class="modal_title_superior_ modal-header" >
                        <h6 class="modal-title d-inline ml-1">Adjuntar documento desde servicio web</h6>  
                        <button type="button" value="Buttoncerrarimpre_adjunta_autamatico_documento" class="close da_event_captive ">&times;</button>                  
                    </div>                   
                        <div id="Contenido_adjunta_autamatico_documento" style=" height:auto; width: 100%; border-top: none" class="modal_content_back p-2">
                            <asp:MutuallyExclusiveCheckBoxExtender ID="MutuallyExclusiveCheckBoxExtender1" runat="server" TargetControlID="Check_anexo_radicado_automatico"
                            Key="radicado"></asp:MutuallyExclusiveCheckBoxExtender>
                        <asp:mutuallyexclusivecheckboxextender id="Mutuallyexclusivecheckboxextender2" runat="server" targetcontrolid="CheckBox_relacionado_radicado_automatico"
                            key="radicado"></asp:mutuallyexclusivecheckboxextender>
                             <div class=" row pl-1 pr-1"> 
                                 <div class="col-6">
                                      <asp:CheckBox ID="Check_anexo_radicado_automatico" runat="server" Text="Guardar como parte del documento" Checked="true"  Font-Size="11"  Enabled="true" CssClass="h6 font-weight-light"  />
                                 </div>
                                 <div class="col-6">
                                      <asp:CheckBox ID="CheckBox_relacionado_radicado_automatico" runat="server" Text="Guardar como documento relacionado " Checked="false" Font-Size="11"  Enabled="true" CssClass="h6 font-weight-light" />
                                 </div>
                             </div>
                            <div id="content_data_grid_adjunta_documento_automatico" class="conten_gred_border_" style="width: 100%">
                                <asp:UpdatePanel ID="UpdatePanel_actualiza_adjunta_documento_automatico" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:DropDownList ID="DropDownList_adjunta_documento_automatico" Style="width: 100%" CssClass="custom-select mr-sm-2" runat="server"></asp:DropDownList>
                                    </ContentTemplate>
                                </asp:UpdatePanel>

                            </div>
                        </div>
                     <div class="modal-footer">
                         <asp:UpdatePanel ID="UpdatePane_adjunta_autamatico_documento" runat="server" UpdateMode="Conditional">
                             <ContentTemplate>
                                  <asp:Button ID="Button_guardar_automatico" runat="server" Text="Guardar"  CssClass="btn btn-success"  />
                                 <asp:HiddenField ID="HiddenField_estado_guarda_automatico" runat="server" Value="" />
                             </ContentTemplate>
                         </asp:UpdatePanel>
                     </div>
                   
                     <div style="display: none; height: 1px">
                         <asp:Button ID="Button1__adjunta_autamatico_documento" CssClass="invisible" runat="server" Text="Button" Height="0px" Width="0px" />
                         <asp:Button ID="ButtonSalir_adjunta_autamatico_documento" CssClass="invisible" runat="server" Text="Button" Height="0px" Width="0px" />
                         <asp:Button ID="Buttoncerrarimpre_adjunta_autamatico_documento" runat="Server" Text="X" CssClass="modal_boton_hiden" />
                     </div>
                
                 </div>   
            </asp:Panel>
       <!--cargar documento!-->
          <div id="contenido_procesa_sube_documento_adjunto" >
            <asp:Panel ID="Panel_sube_documento_adjunto" runat="server" Style="display:none;  width: 80%; height: auto" CssClass="modal_content_general_">
                <asp:ModalPopupExtender ID="ModalPopupExtender_sube_documento_adjunto" runat="Server" BackgroundCssClass="FondoAplicacion" 
                    TargetControlID="Button_sube_documento_adjunto"
                    PopupControlID="Panel_sube_documento_adjunto" CancelControlID="Button3_cerrar_adjunta" ></asp:ModalPopupExtender>
                <div id="modal_content_sube_documento_adjunto" class="modal-content">  
                    <div id="Div_cabecera" class="modal_title_superior_ modal-header"> 
                        <h6 class="modal-title d-inline ml-1">Adjuntar</h6>  
                        <button type="button" value="Button3_cerrar_adjunta" class="close da_event_captive ">&times;</button>                   
                </div>            
                    <div id="Div_contenido_adjunta" style="height: auto; width: 100%; border-top: none" class="modal_content_back p-2">
                        <asp:UpdatePanel ID="Update_actualiza_adjunta_documento" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:MutuallyExclusiveCheckBoxExtender ID="MutuallyExclusiveCheckBoxExtender1_" runat="server" TargetControlID="Check_anexo_radicado_adj"
                                     Key ="radicado_"></asp:MutuallyExclusiveCheckBoxExtender>
                                    <asp:mutuallyexclusivecheckboxextender id="Mutuallyexclusivecheckboxextender2_" runat="server" targetcontrolid="CheckBox_relacionado_radicado_adj"
                                     Key ="radicado_"></asp:mutuallyexclusivecheckboxextender>  
                                <div class=" row pl-1 pr-1">         
                                    <div class="col-6">
                                        <asp:CheckBox ID="Check_anexo_radicado_adj" runat="server" Text="Adjuntar como parte del documento" Checked="true"  Font-Size="11"  Enabled="true" CssClass="h6 font-weight-light"  />
                                    </div>
                                     <div class="col-6">
                                         <asp:CheckBox ID="CheckBox_relacionado_radicado_adj" runat="server" Text="Adjuntar como documento relacionado" Checked="false"  Font-Size="11" CssClass="h6 font-weight-light" Enabled="true" />
                                    </div>
                                </div>                  
                                <div id="content_data_grid_adjunta_documento" class="conten_gred_border_" style="width: 100%">
                                    <asp:DropDownList ID="DropDownList_adjunta_documento" Style="width: 100%" CssClass="custom-select mr-sm-2" runat="server"></asp:DropDownList>
                                </div>
                            </ContentTemplate>
                            <Triggers>
                            </Triggers>
                        </asp:UpdatePanel>
                        <asp:Panel ID="Panel_descarga_ajax" runat="server">
                            <div id="drop_zone_" style="width: 100%; height: auto">
                                <asp:UpdatePanel ID="UpdatePanel_descarga" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:AjaxFileUpload ID="AjaxFileUpload_dowload" runat="server" ThrobberID="drop_zone_"
                                            ContextKeys="fred"
                                            AllowedFileTypes="tif,jpg,tiff,bmp,pdf"
                                            MaximumNumberOfFiles="1" OnClientUploadComplete="activa_boton_dowload"  />
                                            <asp:Button ID="Button1" runat="server" Text="Button" Style="display: none" />
                                        &nbsp  
                                    <asp:Label ID="Label_estado_carga" runat="server" Text="Estado" Style="font-size: 10px" CssClass="font-weight-light h6"></asp:Label>
                                        <input id="Hidden_result_load" type="hidden" value="" runat="server"/>
                                        <input id="Hidden_date_row" type="hidden" value="" runat="server"/>
                                        
                                        
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </asp:Panel>
                        <div style="display: none; height: 1px">
                            <asp:Button ID="Button_sube_documento_adjunto" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                            <asp:Button ID="Button3_cerrar_adjunta" runat="Server" Text="" CssClass="invisible" />
                        </div>

                    </div>
                </div>
            </asp:Panel>     
        </div>
       
          <!--lista_chequeo_tramite-->
        <div id="lista_chequeo_tramite">
            <asp:Panel ID="Panel_lista_chequeo_tramite" runat="server" Style="display:none; color: White; width: 50%; height: auto; margin: auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_lista_chequeo_tramite" runat="server" BehaviorID="Panel_lista_chequeo_tramite" TargetControlID="ButtonSalir_lista_chequeo_tramite" BackgroundCssClass="ModalBackgroud_gorund"
                    CancelControlID="Button_cerrar_lista_chequeo_tramite" PopupControlID="Panel_lista_chequeo_tramite">
                </asp:ModalPopupExtender>
                <div id="divcabecer2_lista_chequeo_tramite" class="modal_title_superior">
                    <asp:Label ID="Label_lista_chequeo_tramite" runat="server" Text="Tipo documental que desea Adjuntar" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton2_lista_chequeo_tramite" style="float: right">
                        <asp:Button ID="Button_cerrar_lista_chequeo_tramite" runat="Server" Text="X" CssClass="modal_boton_hiden"
                             ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_lista_chequeo_tramite" style="background-color: white; width: auto; height: auto; 
                  color: black; background-color: #FFFFFF" class="modal_content_back">
                    <div id="Contenedorgrid" style="width: 99%; position: inherit; left: 0px; top: 0px; text-align: left; height: auto; margin-top: 1px; border-color: #b0c4de; border-width: 1px; border-style: ridge">
                        <asp:UpdatePanel ID="UpdateGeneral" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <input id="Hidden_0002" type="hidden" value="0" runat="server"/>
                                <input id="Hidden_0001" type="hidden" value="-1" runat="server"/>
                                <asp:Panel ID="Panel_principal" runat="server"
                                    Style="overflow: auto; width:100%; min-height:150px; max-height:150px">
                                    <asp:GridView ID="data_grid_chequeo" runat="server" Style="position: inherit; font-family:Arial" AutoGenerateSelectButton="False" CssClass="filtrar" GridLines="None" Font-Size="12px" Width="100%">
                                        <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                        <HeaderStyle CssClass="GridviewScrollHeader_line_blanco" />
                                        <RowStyle CssClass="GridviewScrollItem_line" />
                                        <PagerStyle CssClass="GridviewScrollPager_line" />
                                    </asp:GridView>
                                </asp:Panel>

                            </ContentTemplate>

                        </asp:UpdatePanel>

                    </div>

                    <div style="margin-top: 1px; height: auto; text-align: right">
                        <asp:UpdatePanel ID="UpdatePanel_lista_chequeo" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <asp:TextBox ID="TextBox_contenido_busqueda_lista_cheq" runat="server" Style="width: auto; margin-left:3px;  margin-top: 5px" placeholder="Busqueda.." onkeypress="acti_busq_lista_cheq(event,this)"></asp:TextBox>
                                <input id="Hidden_list_cheo_acepta" type="hidden" value="" runat="server"/>
                                <asp:Button ID="Button_examinar_archivo_lista_chequeo" runat="server" Text="Aceptar" Style="margin-left: 5px; margin-top: 5px" CssClass="boton_azul" />
                                <asp:Button ID="Button_Actualizar_Lista_chequeo" runat="server" Text="Actualizar" Style="margin-top: 5px; display:none" CssClass="boton_azul" />
                                <asp:CheckBox ID="CheckBox_busqueda_list_cheq" runat="server" Style="display: none" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>

                    <div style="overflow: auto">
                        <asp:UpdatePanel ID="UpdatePanel_lista_chequeo_estado" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <asp:Label ID="Label_estado_lista_chequeo" runat="server" Text="Estado" Style="font-size: 12px; display: none"></asp:Label>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
                <asp:Button ID="Button_lista_chequeo_tramite" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                <asp:Button ID="ButtonSalir_lista_chequeo_tramite" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
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
        <div id="progres_bar" style="position:absolute; text-align: center; display: none; width: 200px">
                <img id="imgr_modal" src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
               
            </div>
    </form>
</body>
   <script  accesskey="javascript" type="text/javascript">
       AjaxFileUpload_change_text();
</script>
</html>
