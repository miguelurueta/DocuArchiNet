<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormVisorExterno.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormVisorExterno" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Visor de documentos</title>
     <script src="../js/ui/jquery-3.4.1.min.js"></script>
    <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
    <script src="../js/workflow/WebFormVisorExterno.js"></script>
     <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <script src="../js/java_general/general_code_java.js?v=20260827-compatible-events5"></script>
    <script defer src="../Awesome/js/all.js"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet">
  <link href="../Awesome/css/brands.css" rel="stylesheet">
  <link href="../Awesome/css/solid.css" rel="stylesheet">
    <script defer src="../Awesome/js/brands.js"></script>
  <script defer src="../Awesome/js/solid.js"></script>
  <script defer src="../Awesome/js/fontawesome.js"></script>
    
</head>
<body style="overflow:hidden">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <form id="form_visor_externo" runat="server">
         <asp:ScriptManager ID="ScriptManager1" runat="server"
            EnableScriptGlobalization="True" EnablePageMethods="True">
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
                posicion_update_pogres('progres_bar');
            }
            function CheckStatus(sender, args) {
                progres_hiden('progres_bar');
                //mantiene_oculta_lef();
                $("#noaming").bind("contextmenu", function (e) {
                    e.preventDefault();
                });
              

                if (elment_postbak.id == "TreeViewseleccion") {
                    if (document.getElementById("Hidden_tipo_visor_externo").value == "0") {
                        document.getElementById("content").style.display = "block";
                        document.getElementById("tollimage").style.display = "block";
                        document.getElementById("div_contendor_externo").style.display = "none";
                    } else {
                        document.getElementById("content").style.display = "none";
                        document.getElementById("tollimage").style.display = "none";
                        document.getElementById("div_contendor_externo").style.display = "block";
                    }
                    auto_zise_visor();
                }
            }

        </script>
        <div id="conte_waper" class="container-fluid mr-0 ml-0 pl-0 pr-0" style="border-top: 1px solid #e9ecef"> 
            <a id="da_show-sidebar_" class="btn btn-sm   hide_da_sidebar " href="#" data-target="#sidebar">
                <i style="color:white" class="fas fa-bars"></i>
            </a>       
            <div id="da_content_wraper" class="wrapper ml-0 mr-0 " style="padding-left: 1px; padding-right: 1px;">
                <nav id="sidebar" class=" bg-light pl-0 pr-0   ">
                    <div id="title_table" class="modal-header" style="background: #6d7fcc; border-top-left-radius: initial; border-top-right-radius: initial">
                        <h6 style="color: white" class="modal-title">Documentos</h6>
                        <button id="sidebarCollapse" type="button" class="close">&times;</button>
                    </div>
                    <div id="contenido_treeview" style="height: auto" class="modal-body  pl-1 pr-1 pt-1 pb-1 bg-light">
                        <asp:Panel ID="Panel_scroll" runat="server" ScrollBars="Auto" Style="height: 100%">
                            <asp:UpdatePanel ID="UpdatePanelseleccion" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                <ContentTemplate>
                                    <asp:TreeView ID="TreeViewseleccion" runat="server" NodeWrap="true" CssClass="font-weight-light h6 pl-0" Style="font-family:'Segoe UI'"
                                        NodeStyle-NodeSpacing="0.1px" LeafNodeStyle-CssClass="LeafNodeStyle_2_  mb-1 pl-1  " ExpandDepth="0" NodeIndent="1" CollapseImageUrl="../workflow/imageneswf/folder-open-light.png" ExpandImageUrl="../workflow/imageneswf/folder-light.png" PopulateNodesFromClient="False" SkipLinkText="">
                                        <HoverNodeStyle Font-Underline="True" />
                                        <SelectedNodeStyle CssClass="select_treview_boottra" />
                                        <ParentNodeStyle Font-Bold="False" />
                                        <HoverNodeStyle Font-Underline="True" ForeColor="Purple" />
                                        <NodeStyle CssClass="nav-link-treview mt-2 mb-2 pl-2  " ForeColor="#0062cc"
                                            VerticalPadding="0px" />
                                    </asp:TreeView>

                                    <input id="Hiddenint" type="hidden" value="0" runat="server">
                                </ContentTemplate>
                                <Triggers>
                                </Triggers>
                            </asp:UpdatePanel>
                        </asp:Panel>

                    </div>
                    <div id="contenido_pie" style="width: 100%; background: #6d7fcc; height: auto" class="modal-footer  justify-content-start">
                        <asp:Label ID="Label1" runat="server" Text="Estado" ForeColor="white"></asp:Label>
                    </div>

                </nav>
                <div id="content_" class="page-content mr-0 ml-0 pl-1 pr-1 pb-0 pt-0 " style="width: 100%; overflow: hidden">
                    <div id="div_contendor_externo" style="height: 100%; display: none" class="pt-0 px-0">
                        <asp:UpdatePanel ID="UpdatePanel_ifr_visor" runat="server" UpdateMode="Conditional"
                            RenderMode="Inline">
                            <ContentTemplate>
                                <input id="Hidden_tipo_visor_externo" type="hidden" value="0" runat="server"/>
                                <input id="Hidden_estado_visor" type="hidden" value="0" runat="server"/>
                                <iframe id="ifrm_visor_" runat="server" style="border-style: none; left: 3px; width: 99%; height: 99%; position: relative; top: 0px"
                                    frameborder="0" scrolling="no"></iframe>

                            </ContentTemplate>
                            <Triggers>
                            </Triggers>
                        </asp:UpdatePanel>
                    </div>
                    <div id="content" class="">
                        <div id="tollimage" style="width: 100%; border-bottom: 1px solid #ddd; background: #6d7fcc; display: inline-flexbox" class="navbar navbar-expand-sm   pb-0 pl-0 pt-1">
                            <button class="navbar-toggler btn btn-light " style="padding-bottom: 2px" type="button" data-toggle="collapse" data-target="#navbarNavDropdown">
                                <span class="pb-1"><i style="color: white" class="fas fa-bars"></i></span>
                            </button>
                            <div class="collapse navbar-collapse   pb-1 pt-0" id="navbarNavDropdown">
                                <div class="nav  ">
                                    <div class="nav-item active active_azul">
                                        <a class="nav-link active_azul" title="Primera  imagen" href="#" onclick="activa_boton_client_server('ImageButtonInicio')"><i style="font-size: 20px; color: white" class="fad fa-arrow-alt-to-left  active_azul"></i></a>
                                    </div>
                                    <div class="nav-item active active_azul">
                                        <a class="nav-link  active_azul" style="margin-left: 2px; margin: 2px; width: auto; color: black" title="Anterior imagen" href="#" onclick="activa_boton_client_server('ImageButtonAnterior')"><i style="color: white; font-size: 20px" class="fad fa-arrow-alt-left fa-1x"></i></a>
                                    </div>
                                    <asp:UpdatePanel ID="UpdatePanel_conte_bot" runat="server"
                                        UpdateMode="Conditional" RenderMode="Inline">
                                        <ContentTemplate>
                                            <div class="nav-item active ">
                                                <asp:TextBox ID="LabelConteo" runat="server" Style="margin-left: 5px; margin-right: 5px; text-align: center; margin-top: 3px; font-size: 12px; width: 50px; font-family: 'Segoe UI Emoji'" onkeypress="preven_event_search_keypres_enter(event,this);"></asp:TextBox>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                    <div class="nav-item active  active_azul">
                                        <a class="nav-link active_azul" style="" title="Siguiente imagen" href="#" onclick="activa_boton_client_server('ImageButtonSiguiente')"><i style="color: white; font-size: 20px" class="fad fa-arrow-alt-right fa-1x"></i></a>
                                    </div>

                                    <div class="nav-item active active_azul">
                                        <a class="nav-link active_azul" style="color: white" title="Ultima  imagen" href="#" onclick="activa_boton_client_server('ImageButtonFinal')"><i style="color: white; font-size: 20px" class="fad fa-arrow-alt-to-right fa-1x"></i></a>
                                    </div>

                                    <div class="nav-item active active_azul">
                                        <a class="nav-link active_azul" style="color: white" title="Alejar Imagen" href="#" onclick="activa_boton_client_server('ImageMenos')"><i style="color: white; font-size: 20px" class="fad fa-minus-circle fa-1x"></i></a>
                                    </div>

                                    <div class="nav-item active active_azul">
                                        <a class="nav-link active_azul" style="color: white" title="Acercar Imagen" href="#" onclick="activa_boton_client_server('ImageMas') "><i style="color: white; font-size: 20px" class="fad fa-plus-circle fa-1x"></i></a>
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
                                    <div class="nav-item_ active active_azul">
                                        <a class="nav-link active_azul" id="ImageButtonguardardocumento_" style="" title="Descargar imagenes" href="#" onclick="activa_boton_client_server('ImageButtonguardardocumento')"><i style="color: white; font-size: 20px" class="fad fa-arrow-to-bottom fa-1x"></i></a>
                                    </div>
                                    <div class="nav-item_ active active_azul">
                                        <a class="nav-link active_azul" style="color: white; font-family: Arial; text-decoration: none; font-weight: 600" title="Rotar 90 grados a la izquierda" href="#" onclick="activa_boton_client_server('ImageRotate45')"><i style="color: white" class="fad fa-undo fa-1x"></i></a>
                                    </div>
                                    <asp:TextBox ID="TextBox_ir_documento" Style="display: none" class=" mr-0 ml-0" Width="74px" runat="server" placeholder="" onkeypress="preven_event_search_keypres_enter(event,this);"></asp:TextBox>
                                    <div class="nav-item_ active active_azul" style="display: none">
                                        <a class="nav-link active_azul" style="" title="Ir a imagen" href="#" onclick="activa_boton_client_server('ImageButton_ir_pagina')"><i style="color: white; font-size: 20px" class="fas fa-search fa-1x"></i></a>
                                    </div>
                                </div>
                                <input id="Hidden_ruta_archivo" type="hidden" value="" runat="server" />
                            </div>
                            <asp:UpdatePanel ID="UpdatePanelButon" runat="server"
                                UpdateMode="Conditional" RenderMode="Inline">
                                <ContentTemplate>
                                    <div style="display: none">
                                        <asp:ImageButton ID="ImageButtonInicio" runat="server" Style="margin-left: 5px; float: left; display: none" ImageUrl="#" />
                                        <asp:ImageButton ID="ImageButtonAnterior" runat="server" ImageUrl="#" Style="display: none" />
                                        <asp:ImageButton ID="ImageButtonSiguiente" runat="server" ImageUrl="#" Style="display: none" />
                                        <asp:ImageButton ID="ImageButtonFinal" runat="server" ToolTip="Ultima  imagen" ImageUrl="#" Style="display: none" />
                                        <asp:ImageButton ID="ImageMenos" runat="server" ImageUrl="#" Style="display: none" />
                                        <asp:ImageButton ID="ImageMas" runat="server" ImageUrl="#" Style="display: none" />
                                        <asp:ImageButton ID="ImageRotate45" runat="server" ImageUrl="#" Style="display: none" />
                                        <asp:ImageButton ID="ImageRotate180" runat="server" ImageUrl="#" Style="display: none" />
                                        <asp:ImageButton ID="ImageRotate270" runat="server" ImageUrl="#" Style="display: none" />
                                        <asp:ImageButton ID="ImageButtonguardardocumento" runat="server" ImageUrl="#" Style="display: none" />
                                        <asp:ImageButton ID="ImageButtonindice" runat="server" ImageUrl="#" Style="display: none" />
                                        <asp:ImageButton ID="ImageButton_ir_pagina" runat="server" ImageUrl="#" Style="display: none" />
                                    </div>

                                </ContentTemplate>
                                <Triggers>
                                </Triggers>
                            </asp:UpdatePanel>


                        </div>
                        <div id="content_image" style="width: 100%; height: 380px; filter: alpha(opacity=70); opacity: 50; overflow: auto" class=" bg-secondary  pl-0">
                            <div id="zona" style="width: auto; height: auto;">
                                <asp:UpdatePanel ID="UpdatePanelvisor" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                    <ContentTemplate>
                                        <neoimg:ImageDraw ID="noaming" runat="server" Style="position: relative" RenderingMethod="HttpHandler" HttpHandlerName="ImageGenerator.axd">
                                        </neoimg:ImageDraw>
                                    </ContentTemplate>
                                    <Triggers>
                                    </Triggers>
                                </asp:UpdatePanel>
                            </div>
                            <input id="Hiddenintercambio" type="hidden" value="0" runat="server">
                            <input id="Hiddenintercambio2" type="hidden" value="0" runat="server">
                            <input id="Hidden_id_tarea_sel" type="hidden" value="-1" runat="server">
                            <input id="Hidden_tipo_visor" type="hidden" value="" runat="server">
                        </div>
                    </div>
                </div>
            </div>
        </div>
        
            <asp:Panel ID="Panel_guardar" runat="server"  Style="display:none;  width: 50%; height: auto" CssClass="modal_content_general_">              
                <asp:ModalPopupExtender ID="ModalPopupExtender_guardar" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_post_guardar"
                    PopupControlID="Panel_guardar" CancelControlID="Buttoncerrarimpre_post_guardar">
                </asp:ModalPopupExtender>
                <div id="modal_content_guardar" class="modal-content">  
                    <div id="divcabecer2_post_guardar" class="modal_title_superior_ modal-header" >
                     <h6 class="modal-title d-inline">Descargar</h6>
                     <button type="button" value="Buttoncerrarimpre_post_guardar" class="close da_event_captive ">&times;</button>                    
                </div>
                <div id="ContenidoImpresion_guardar" style="height: 100%; width: 100%; border-top:none; overflow:auto" class="modal_content_back pl-1 pr-1">
                    <asp:UpdatePanel ID="UpdatePane_guardar" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <iframe  style="height:100%; width:99%" id="Iframe_guardar" runat="server" class="modal_iframe"></iframe>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <div style="display:none; height:1px">
                         <asp:Button ID="Button1_guardar" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                         <asp:Button ID="ButtonSalir_post_guardar" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                        <div id="Divcerrarbuton2_post_guardar" style="float: right">
                            <asp:Button ID="Buttoncerrarimpre_post_guardar" runat="Server" Text=""  />
                        </div>
                     </div>
                </div>
                  <div id="content_boton_guardar" class="modal-footer justify-content-end">
                   </div>
                </div>
                
            </asp:Panel>
           
       
         <!--POPUP INDICE DE DOCUMENTO-->
        <asp:Panel ID="Panel_indice" runat="server"  Style="display:none; color: White; width: 100%; height: 97%; background-color:white" CssClass="modal_content_general">
            <asp:ModalPopupExtender ID="ModalPopupExtenderimpre_indice" runat="Server" Y="1" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_indice"
                    PopupControlID="Panel_indice" CancelControlID="Buttoncerrarimpre_indice">
                </asp:ModalPopupExtender>
                <div id="divcabecer2__indice" class="modal_title_superior" >       
                    <asp:Label ID="Label_indice_visor_externo" runat="server" Text="Indice documento" Font-Size="10" Style="color:black; font-family:Arial; margin-left:10px">
                    </asp:Label>
                    <div id="Divcerrarbuton2_indice" style="float: right">
                        <asp:Button ID="Buttoncerrarimpre_indice" runat="Server" Text="X" CssClass="modal_boton_hiden"
                             ToolTip="Cerrar ventana indice" style="margin-top:0px; margin-right:5px" />

                    </div>
                </div>
            <div id="Cotenedor_indice" style="height: 100%; width: 100%; overflow:hidden" class="modal_content_back" >
                 <asp:UpdatePanel ID="UpdatePanelindice" runat="server" UpdateMode="Conditional" 
                  RenderMode="Inline" >
                  <ContentTemplate>
                      <iframe id="ifrm_indice_" runat="server" style="border-style: none; left: 1px; width: 100%; height:100%; position: relative; top: 1px; background-color:white; float: right"
                          frameborder="0" scrolling="no" ></iframe>
                          <input id="Hidden_result_indice" type="hidden" value="0" runat="server"/>
                  </ContentTemplate>
                  <Triggers>
                      
                  </Triggers>
              </asp:UpdatePanel>
            </div>
             
            <asp:Button ID="Button1_indice" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
            <asp:Button ID="ButtonSalir_indice" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
             </asp:Panel>
        <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 50px; color:red">
            <img src="../workflow/loading.gif"  style="vertical-align: middle" alt="Processing"  />
            Processing ...
        </div>   
    </form>
    <script type="text/javascript">
        $(document).ready(function () {
            $('#sidebarCollapse').on('click', function () {
                $('#sidebar').toggleClass('active_da_slider');
                $(this).toggleClass('active_da_slider');
                $('#da_show-sidebar_').toggleClass('show_da_slide');
                $('#da_show-sidebar_').toggleClass('hide_da_sidebar');
            });
            $('#da_show-sidebar_').on('click', function () {
                $('#sidebar').toggleClass('active_da_slider');
                $(this).toggleClass('show_da_slide');
                $(this).toggleClass('hide_da_sidebar');
            });
        });
    </script>
</body>
</html>
