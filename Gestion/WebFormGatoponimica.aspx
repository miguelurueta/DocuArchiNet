<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormGatoponimica.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormGatoponimica" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <script src="../js/ui/jquery-3.4.1.min.js"></script>  
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script> 
    <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
    <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <script src="../js/MyJavaScriptFile.js"></script>  
    <script src="../js/gestion/WebFormGatoponimica.js"></script> 
     <script defer src="../Awesome/js/all.js"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet">
     <link href="../Awesome/css/brands.css" rel="stylesheet">
     <link href="../Awesome/css/solid.css" rel="stylesheet">
     <script defer src="../Awesome/js/brands.js"></script>
     <script defer src="../Awesome/js/solid.js"></script>
     <script defer src="../Awesome/js/fontawesome.js"></script>
      <script src="../js/java_general/general_code_java.js"></script>
</head>
<body>
    <form id="form1" runat="server">
         <asp:ScriptManager ID="ScriptManager1" runat="server">
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
                if (elment_postbak.id == "ButtonReubicar") {
                    auto_zise_reasigna_expe_unidad();
                }

            }

            </script>
         <nav id="menu_var" class="navbar navbar-expand-sm nav_botota_person_gray modal_content_no_back_inferior">
             <button id="nav_togle_display" class="navbar-toggler" type="button" style="background-color: #6d7fcc" data-toggle="collapse" data-target="#navbarNavDropdown">
                 <span class="navbar-toggler-icon_"><i style="color: white" class="fad fa-th-list"></i></span>
             </button>
             <div class="collapse navbar-collapse row" id="navbarNavDropdown"> 
                 <ul class="navbar-nav col-md-12" >
                      <li class="nav-item dropdown active ml-2 mr-0 active_">
                        <a class="nav-link  dropdown-toggle " style="color: #6d7fcc" href="#" id="A5" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc" class="fad fa-tools"></i> Configurar Estructura 
                        </a>
                        <ul class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">
                            <li> <a style="color: #6d7fcc" href="#" class="dropdown-item font-weight-light" onclick="activa_menu_general_diference_event(event,this,'AGRE-ED','ADD')"><i class="fal fa-building"></i> Agregar nuevo edificio a la estrucura</a></li>
                            <li><a style="color: #6d7fcc" href="#" class="dropdown-item font-weight-light" onclick="activa_menu_general_diference_event(event,this,'AGRE-EL','ADD')"><i class="fal fa-plus"></i> Agregar nuevo elemento</a></li>
                            <li><a style="color: #6d7fcc" href="#" class="dropdown-item font-weight-light" onclick="activa_menu_general_diference_event(event,this,'DELT-EL','DELETE')"><i class="fal fa-times"></i> Eliminar elemento seleccionado</a></li>
                            <li><a style="color: #6d7fcc" href="#" class="dropdown-item font-weight-light" onclick="activa_menu_general_diference_event(event,this,'EDIT-EL','EDIT')"><i class="fal fa-pencil"></i> Editar elemento seleccionado</a></li>      
                        </ul>
                    </li>
                     <li class="nav-item dropdown active ml-2 mr-0 active_">
                        <a class="nav-link  dropdown-toggle " style="color: #6d7fcc" href="#" id="A1" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc" class="fad fa-archive"></i> Unidad de conservación
                        </a>
                        <ul class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">
                            <li> <a style="color: #6d7fcc" href="#" class="dropdown-item font-weight-light" onclick="activa_menu_general_diference_boton(event,this,'A-UC')"><i class="fal fa-archive"></i> Agregar unidad de conservación</a></li>
                            <li><a style="color: #6d7fcc" href="#" class="dropdown-item font-weight-light" onclick="activa_menu_general_diference_boton(event,this,'E-UC')"><i class="fal fa-edit"></i> Editar unidad de conservación</a></li>
                            <li><a style="color: #6d7fcc" href="#" class="dropdown-item font-weight-light" onclick="activa_menu_general_diference_boton(event,this,'D-UC')"><i class="fad fa-times"></i> Eliminar unidad de conservación</a></li>              
                        </ul>
                    </li>
                      <li class="nav-item dropdown active ml-2 mr-0 active_">
                        <a class="nav-link  dropdown-toggle " style="color: #6d7fcc" href="#" id="A2" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc" class="fad fa-folder-tree"></i> Ubicación
                        </a>
                        <ul class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">
                            <li> <a style="color: #6d7fcc" href="#" class="dropdown-item font-weight-light" onclick="activa_menu_general_diference_boton(event,this,'DUA-UC')"><i class="fal fa-folder-tree"></i> Desarchivar unidad</a></li>
                            <li><a style="color: #6d7fcc" href="#" class="dropdown-item font-weight-light" onclick="activa_menu_general_diference_boton(event,this,'RUR-UC')"><i class="fal fa-sitemap"></i> Reubicar unidad</a></li>
                            
                        </ul>
                    </li>
                      <li class="nav-item dropdown active ml-2 mr-0 active_">
                        <a class="nav-link  dropdown-toggle " style="color: #6d7fcc" href="#" id="A3" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc" class="fad fa-stamp"></i> Rotulación
                        </a>
                        <ul class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">
                            <li> <a style="color: #6d7fcc" href="#" class="dropdown-item font-weight-light" onclick="activa_menu_general_diference_boton(event,this,'IMPR-UC')"><i class="fad fa-print"></i> Imprimir rotulo</a></li>
                            <li> <a style="color: #6d7fcc" href="#" class="dropdown-item font-weight-light" onclick="activa_menu_general_diference_boton(event,this,'DESR-UC')"><i class="fal fa-file-download"></i> Descargar rotulo</a></li>
                            <li> <a style="color: #6d7fcc" href="#" class="dropdown-item font-weight-light" onclick="activa_menu_general_diference_boton(event,this,'CONR-UC')"><i class="fad fa-tools"></i> Configurar rotulo</a> </li>
                        </ul>
                    </li>
                 </ul>
             </div>
         </nav>
        <asp:UpdatePanel ID="UpdatePanel_menu_var_event" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <input id="Hidden_menu_var_event_dive" type="hidden" value="" runat="server" />
                <input id="Hidden_edita_red_event" type="hidden" value="" runat="server" />
                <asp:Button ID="Button_me_active_men_dive" runat="server" Text="" Style="display: none; width: 1px; height: 1px" />
            </ContentTemplate>
        </asp:UpdatePanel>
      
        <asp:HiddenField ID="HiddenField_botones_respuesta" runat="server" value="-1"/>
    <div id="Contentizquierdo" style="width: 25%; height: 100%; float: left; position:relative" class=" modal_content_no_back_left"> 
        <div id="Div_title_estrucutura" style="text-align: center" class="row w-100 p-2">
            <div class="col-12">
                <h6 class="font-weight-light" style="color: #0062cc">Estructura</h6>
            </div>
        </div>
            <asp:UpdatePanel ID="UpdatePanelEntidadEmpresa" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div id="drowlist" class="">
                        <div class="">
                            <asp:DropDownList ID="DropDownListEntidadEmpresa" runat="server" Width="100%" style="color:#0062cc" onchange="buton_click('Button_listar_edificio');" CssClass="custom-select"></asp:DropDownList>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>   
         <div id="div_treview_archivo" >
             <asp:Panel ID="Paneltreview" runat="server" ScrollBars="Both"
                 Height="100%" Width="100%" Style="position: inherit">
                 <asp:UpdatePanel ID="UpdatePanelViewArchivo" runat="server" UpdateMode="Conditional">
                     <ContentTemplate>
                         <asp:TreeView ID="TreeViewArchivo" Style="text-align: left; padding-left: 1px; font-size: 13px; margin-top:0px" runat="server" CssClass="TreeN" NodeWrap="true"
                                    PopulateNodesFromClient="False" EnableViewState="true"
                                    LeafNodeStyle-CssClass="LeafNodeStyle" Font-Size="13px" NodeIndent="10" ExpandDepth="0"  SkipLinkText="">
                                    <HoverNodeStyle Font-Underline="False" />
                                    <LeafNodeStyle CssClass="LeafNodeStyle" HorizontalPadding="10px" NodeSpacing="0px" VerticalPadding="5px" />
                                    <NodeStyle ChildNodesPadding="5px" HorizontalPadding="0px" NodeSpacing="5px" VerticalPadding="5px"  CssClass="mt-2 mb-2 pl-2  font-weight-normal" ForeColor="#0062cc" />
                                    <ParentNodeStyle ChildNodesPadding="0px"     Font-Bold="true" HorizontalPadding="5px" NodeSpacing="0px" VerticalPadding="5px" CssClass="font-weight-bold"/>
                                    <RootNodeStyle ChildNodesPadding="0px"  Font-Bold="true" NodeSpacing="0px" VerticalPadding="5px" HorizontalPadding="5px" CssClass="font-weight-bold"/>
                                    <SelectedNodeStyle  CssClass="select_treview_boottra font-weight-normal nav-link-treview"  ImageUrl="../workflow/imageneswf/iten_list_select.png" />
                                </asp:TreeView>
                     </ContentTemplate>
                 </asp:UpdatePanel>
               
             </asp:Panel>
         </div>
        
    </div>    
        <!--contenedor derecho-->
        <div id="Contenedorderecho" style="width: 75%; position: inherit; left: auto; float: right; height: 100%">
            <asp:UpdatePanel ID="UpdatePanel_titulo_unidad_conservacion" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div id="titulo_unidad_conservacion" style=" text-align: center" class="row w-100 p-2  modal_content_back_inferior">
                        <div class="col-6">
                            <h6 class=" font-weight-light" style="color:#0062cc">Entrepaños (contenedor de unidades de conservación)</h6>
                        </div>
                        <div class="col-6">
                            <asp:Label ID="Label_estado" runat="server"  Text="" Style="font-size:9px" CssClass="h6"></asp:Label>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>

            <div id="contenedor_unidad_treview_unidad" style="height: 79%" class="modal_content_back_inferior_superior_">
                <asp:Panel ID="Panel_unidad_treview_unidad" runat="server" ScrollBars="Both"
                    Height="100%" Width="100%" Style="position: inherit">
                    <asp:UpdatePanel ID="UpdatePanel_unidad_treview_unidad" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:TreeView ID="TreeViewunidad" Style="text-align: left; padding-left: 5px; font-size: 13px; margin-top: 2px" runat="server" CssClass="TreeN" NodeWrap="true"
                                PopulateNodesFromClient="False" EnableViewState="true"
                                LeafNodeStyle-CssClass="LeafNodeStyle" Font-Size="13px" NodeIndent="5" ExpandDepth="0" SkipLinkText="">
                                <HoverNodeStyle Font-Underline="False" />
                                <LeafNodeStyle CssClass="LeafNodeStyle" HorizontalPadding="10px" NodeSpacing="0px" VerticalPadding="5px" />
                                <NodeStyle ChildNodesPadding="5px" HorizontalPadding="0px" NodeSpacing="5px" VerticalPadding="5px" CssClass="mt-2 mb-2 pl-2 font-weight-light nav-link-treview" ForeColor="#0062cc" />
                                <ParentNodeStyle ChildNodesPadding="0px"  Font-Bold="true" HorizontalPadding="5px" NodeSpacing="0px" VerticalPadding="5px" CssClass="font-weight-bold"/>
                                <RootNodeStyle ChildNodesPadding="0px"  Font-Bold="true" NodeSpacing="0px" VerticalPadding="5px" HorizontalPadding="5px" CssClass="font-weight-bold" />
                                <SelectedNodeStyle  CssClass="select_treview_boottra font-weight-normal nav-link-treview"  ImageUrl="../workflow/imageneswf/iten_list_select.png" />
                            </asp:TreeView>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </asp:Panel>
            </div>
            <div id="contendor_botones_unidad" style="height: 10%; overflow: auto; display:none">
                <asp:UpdatePanel ID="UpdatePanel_botones_unidad" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Button ID="ButtonAgregar" runat="server" Text="Agregar" CssClass="boton_azul" Style="margin-top: 3px; margin-left: 10px; display: none" />
                        <asp:Button ID="ButtonButtonEditar" runat="server" Text="Editar" CssClass="boton_azul" Style="display: none" />
                        <asp:Button ID="ButtonEliminar" runat="server" Text="Eliminar" CssClass="boton_azul" Style="display: none" OnClientClick="pront_confirmacion('Desea eliminar la unidad contenedora ?');" />
                        <asp:Button ID="ButtonRotulo" runat="server" Text="Descarga Rotulo" CssClass="boton_azul" Style="display: none" />
                        <asp:Button ID="ButtonImprimirRotulo" runat="server" Text="Imprimir rotulo" CssClass="boton_azul" Style="display: none" />
                        <asp:Button ID="Button_configura_rotulo" runat="server" Text="Configura rotulo " CssClass="boton_azul" Style="display: none" ToolTip="Selecciona el rotulo de impresión expediente o carpeta" />
                        <asp:Button ID="ButtonReubicar" runat="server" Text="Reubicar" CssClass="boton_azul" Style="display: none" />
                        <asp:Button ID="Buttondesarchivar" runat="server" Text="Desarchivar" CssClass="boton_azul" Style="display: none" OnClientClick="pront_confirmacion('Desea desachivar ?');" />
                        <asp:Button ID="ButtonArchivar" runat="server" Text="Archivar" CssClass="boton_azul" Style="width: 70px; display: none" />
                        <asp:Button ID="Button_agrega_unidad_conservacion_interface" runat="server" Text="Archivar" CssClass="boton" Style="width: 70px; display: none" />
                        <asp:HiddenField ID="Hidden_id_unidad" runat="server" Value="0" />
                        <asp:HiddenField ID="HiddenField_empresa" runat="server" Value="0" />
                        <asp:HiddenField ID="HiddenField_estado_ubicacion" runat="server" Value="" />
                        <input id="Hidden_ruta_archivo" type="hidden" value="" runat="server">
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
        <div id="botones" style="display:none">
            <asp:UpdatePanel ID="UpdatePanel_botones_comandos" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Button ID="Button_listar_edificio" runat="server" Text="Button" />
                    <asp:Button ID="Button_actualizar_unidad" runat="server" Text="Button" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
  
       
    <div id="modal_agregar_expediente">

            <asp:Panel ID="Panel_agregar_expdiente_popup" runat="server"  Style="display:none; color: White; width: 50%; height: 100%" CssClass="modal_content_general">
               
                <asp:ModalPopupExtender ID="ModalPopupExtende_agregar_expdiente_popup" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_agregar_expdiente_popup"
                    PopupControlID="Panel_agregar_expdiente_popup" CancelControlID="Buttoncerrar_agregar_expdiente_popup" Y="0">
                </asp:ModalPopupExtender>
                <div id="divcabecer_agregar_expdiente_popup" class="cabecera2" style="width:97%; display:none">
                    <asp:Button ID="Button_agregar_expdiente_popup" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_agregar_expdiente_popup" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Label ID="Label_agregar_expdiente_popup" runat="server" Text="Gestión unidad contendora" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton_agregar_expdiente_popup" style="float: right; display:none">
                         
                                 <asp:Button ID="Buttoncerrar_agregar_expdiente_popup" runat="Server" Text="X"
                                     ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana"  />
                             
                    </div>
                </div>  
                 <div id="Contenido_agregar_expdiente_popup" style=" color: black; background-color:transparent; height: 100%; width: 100%">
                     <asp:UpdatePanel ID="UpdatePanel_agregar_expdiente_popup" runat="server" UpdateMode="Conditional" style="height:100%" RenderMode="Inline">
                         <ContentTemplate>
                        <iframe  id="Iframe_agregar_expdiente_popup_"  runat="server"  style="width:100%; height:100%; background-color:transparent; border:0px" scrolling="no"></iframe>                
                             <asp:HiddenField ID="Hidden_tipo_unidad_seleccion" runat="server" value="0"/>
                             </ContentTemplate>
                         </asp:UpdatePanel>
                    </div>  
               
            </asp:Panel>
            
        </div>
        <div id="Impresion_post">
            <asp:Panel ID="Panelimpresionpost" runat="server" Style="display: none; color: black; width: auto; height: auto" CssClass="modal_content_general">
                <asp:DragPanelExtender ID="DragPanelExtenderimpre_post" runat="server" TargetControlID="Panelimpresionpost" />
                <asp:ModalPopupExtender ID="ModalPopupExtenderimpre_post" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_post"
                    PopupControlID="Panelimpresionpost" CancelControlID="Buttoncerrarimpre_post">
                </asp:ModalPopupExtender>
                <div id="modal_content_Panelimpresionpost" class="modal-content">
                    <div id="divcabecer2_post" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title d-inline ml-1">Menú Impresión</h6>
                        <button type="button" value="Buttoncerrarimpre_post" class="close da_event_captive">&times;</button>
                    </div>
                    <asp:UpdatePanel ID="UpdatePaneliframe_post" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div id="ContenidoImpresion_post" style="color: black; background-color: #FFFFFF; height: auto; width: auto" class="modal_content_back modal-body">
                                <iframe width="100%" height="100%" id="ifimpre_post_" runat="server" frameborder="0"></iframe>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button1_post" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" Style="display: none" />
                        <asp:Button ID="ButtonSalir_post" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" Style="display: none" />
                        <asp:Button ID="Buttoncerrarimpre_post" runat="Server" Text="" CssClass="modal_boton_hiden" Height="0px" Width="0px" Style="display: none" />
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
                                            PopulateNodesFromClient="False"
                                            LeafNodeStyle-CssClass="LeafNodeStyle" ForeColor="Black" HorizontalPadding="10px" Font-Size="12px" NodeIndent="5" ExpandDepth="0" SkipLinkText="">
                                            <HoverNodeStyle Font-Underline="False" />
                                            <LeafNodeStyle CssClass="LeafNodeStyle" HorizontalPadding="0px" NodeSpacing="0px" VerticalPadding="0px" />
                                            <NodeStyle HorizontalPadding="10px" NodeSpacing="5px" VerticalPadding="5px" ForeColor="Black" CssClass="nav-link-treview mt-2 mb-2 pl-2" />
                                            <ParentNodeStyle ChildNodesPadding="0px" CssClass="ParentNodeStyle" HorizontalPadding="5px" NodeSpacing="0px" VerticalPadding="0px" Font-Bold="true" />
                                            <RootNodeStyle ChildNodesPadding="0px" CssClass="RootNodeStyle" NodeSpacing="0px" VerticalPadding="1px" HorizontalPadding="10px" Font-Bold="true" />
                                            <SelectedNodeStyle HorizontalPadding="10px" CssClass="select_treview_boottra_ajustado  nav-link-treview" ImageUrl="~/workflow/imageneswf/iten_list_select.png" />
                                        </asp:TreeView>
                                    </ContentTemplate>
                                </asp:UpdatePanel>

                            </asp:Panel>
                        </div>         
                    </div>
                    <div id="contendor_botones_unidad_r_u_e" style="border-top:none" class="border_inferior_radius_blanco_ modal-footer">
                            <asp:UpdatePanel ID="UpdatePanel_botones_unidad_r_u_e" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                   <asp:Button ID="Button_reubicar" runat="server" Text="Reubicar" CssClass="btn btn-success" />
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
          <div id="configura_plantilla_rotulo">
              <asp:Panel ID="Panel_configura_plantilla_rotulo" runat="server" Style="display: none; color: black; width: 40%; height: auto" CssClass="modal_content_general">
                  <asp:ModalPopupExtender ID="ModalPopupExtender_edition_configura_plantilla_rotulo" runat="server" TargetControlID="ButtonSalir_configura_plantilla_rotulo" BackgroundCssClass="FondoAplicacion"
                      CancelControlID="Button_cerrar_configura_plantilla_rotulo" PopupControlID="Panel_configura_plantilla_rotulo">
                  </asp:ModalPopupExtender>
                  <div id="modal_content_Panel_configura_plantilla_rotulo" class="modal-content">
                      <div id="divcabecer2_configura_plantilla_rotulo" class="modal_title_superior_ modal-header">
                           <h6 class="modal-title d-inline ml-1"></h6>
                           <button type="button" value="Button_cerrar_configura_plantilla_rotulo" class="close da_event_captive">&times;</button>   
                          
                      </div>
                      <div id="contenido_procesa_configura_plantilla_rotulo" style="background-color: white; width: 100%; height: 99%; color: black; background-color: #FFFFFF" class="modal_content_back modal-body">
                          <asp:UpdatePanel ID="UpdatePanel_configura_plantilla_rotulo" runat="server" UpdateMode="Conditional">
                              <ContentTemplate>
                                  <div class="row-12 w-100 mt-2">
                                   <div class="col-12">
                                       <span>Selecciona plantilla</span>
                                   </div>
                                   <div class="col-12">
                                       <asp:DropDownList ID="DropDownList_configura_plantilla_rotulo" runat="server" Style="width: 100%" CssClass="custom-select"></asp:DropDownList>
                                   </div>
                               </div>

                              </ContentTemplate>
                          </asp:UpdatePanel>

                      </div>
                      <div class="modal-footer justify-content-end" >  
                          <asp:UpdatePanel ID="UpdatePanel_boton_config_rotulo" runat="server" UpdateMode="Conditional">
                              <ContentTemplate>
                                   <asp:Button ID="Button_aceptar_configura_plantilla_rotulo" runat="server" Text="Aceptar" CssClass="btn btn-success" />
                            </ContentTemplate>
                         </asp:UpdatePanel>
                      </div>
                      <div style="display: none; height: 1px">
                          <asp:Button ID="Button_configura_plantilla_rotulo" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                          <asp:Button ID="ButtonSalir_configura_plantilla_rotulo" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                          <asp:Button ID="Button_cerrar_configura_plantilla_rotulo" runat="Server" Text="" CssClass="invisible" Height="0px" Width="0px" />
                      </div>

                  </div>
              </asp:Panel>
        </div>
        <div id="reg_edit_edificio_archivo" style="clear:both">
            <asp:Panel ID="Panel_reg_edit_edificio_archivo" runat="server" Style="display:none; color: black; width:60%; height:auto" CssClass="modal_content_general" >
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_reg_edit_edificio_archivo" runat="server"  TargetControlID="ButtonSalir_reg_edit_edificio_archivo" 
                     BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_reg_edit_edificio_archivo" PopupControlID="Panel_reg_edit_edificio_archivo" ></asp:ModalPopupExtender>
                <div id="modal_content_Panel_reg_edit_edificio_archivo" class="modal-content">  
                <div id="divcabecer2_radica_documento" class="modal_title_superior_ modal-header">   
                       <h6 class="modal-title d-inline ml-1" id="Label_reg_edit_edificio_archivo"></h6>
                       <button type="button" value="Button_cerrar_reg_edit_edificio_archivo" class="close da_event_captive">&times;</button>                       
                </div>
                <div id="contenido_procesa_reg_edit_edificio_archivo" style="background-color: white; width: 100%; height: 99%; overflow:auto" class="modal_content_back modal-body">         
                        <asp:UpdatePanel ID="UpdatePanel_reg_edit_edificio_archivo" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row w-100 mt-1">
                                    <div class="col-12" style="text-align:center">
                                          <asp:Label ID="Label_reg_edit_title" runat="server" Text="Regitra edificio archivo" class="h6"></asp:Label>
                                    </div>
                                </div>
                                 <div class="row w-100 mt-3">
                                     <div class="col-6" >      
                                         <span>País Edificio</span>
                                     </div>
                                     <div class="col-6" >
                                         <asp:DropDownList ID="DropDownList_reg_edit_pais" runat="server" Style="width:100%" AutoPostBack="true" CssClass="custom-select" ></asp:DropDownList>
                                     </div>
                                </div>
                                 <div class="row w-100 mt-2">
                                     <div class="col-6" >
                                         <span>Departamento Edificio</span>
                                     </div>
                                     <div class="col-6" >
                                         <asp:DropDownList ID="DropDownList_reg_edit_departamento" runat="server" Style="width:100%" AutoPostBack="true" CssClass="custom-select" ></asp:DropDownList>
                                     </div>
                                </div>
                                 <div class="row w-100 mt-2">
                                     <div class="col-6" >
                                       
                                         <span>Cidudad/Municipio Edificio</span>
                                     </div>
                                     <div class="col-6" >
                                         <asp:DropDownList ID="DropDownList_reg_edit_munici_depart" runat="server" Style="width:100%" AutoPostBack="true"  CssClass="custom-select"></asp:DropDownList>
                                     </div>
                                </div>
                                 <div class="row w-100 mt-2">
                                     <div class="col-6" >        
                                         <span>Dirección Edificio</span>
                                     </div>
                                     <div class="col-6" >
                                         <asp:TextBox ID="TextBox_reg_edit_direcion" runat="server" MaxLength="120" Style="width:100%" CssClass="form-control"></asp:TextBox>
                                     </div>
                                </div>
                                 <div class="row w-100 mt-2">
                                     <div class="col-6" >          
                                         <span>Telefono Edificio *</span>
                                     </div>
                                     <div class="col-6" >
                                          <asp:TextBox ID="TextBox_reg_edit_telefono" runat="server" MaxLength="45" Style="width:100%" CssClass="form-control"></asp:TextBox>
                                     </div>
                                </div>
                                 <div class="row w-100 mt-2">
                                     <div class="col-6" >
                                         <span>Nombre responsable Edificio *</span>
                                     </div>
                                     <div class="col-6" >
                                         <asp:TextBox ID="TextBox_reg_edit_responsable" runat="server" MaxLength="45" Style="width:100%" CssClass="form-control"></asp:TextBox>
                                     </div>
                                </div>
                                 <div class="row w-100 mt-2">
                                     <div class="col-6" >               
                                         <span>Nombre  Edificio *</span>
                                     </div>
                                     <div class="col-6" >
                                        <asp:TextBox ID="TextBox_reg_edit_edificio_nombre" runat="server" MaxLength="45" Style="width:100%"></asp:TextBox>
                                     </div>
                                </div>
                              
                            </ContentTemplate>
                        </asp:UpdatePanel>
                          
                </div>
                    <div class="modal-footer justify-content-end" id="modal-footer_Panel_reg_edit_edificio_archivo">  
                         <asp:UpdatePanel ID="UpdatePanel_boton_edit_add" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Button ID="Button_reg_edit_aceptar" runat="server" Text="Aceptar" CssClass="btn btn-success"  />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button_reg_edit_edificio_archivo" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="ButtonSalir_reg_edit_edificio_archivo" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="Button_cerrar_reg_edit_edificio_archivo" runat="Server" Text="" Style="display: none" />
                    </div>
                 
                </div>
            </asp:Panel>
           
        </div>
        <div id="reg_edit_piso_archivo" style="clear:both">
            <asp:Panel ID="Panel_reg_edit_piso_archivo" runat="server" Style="display:none; color: black; width:50%; height:auto" CssClass="modal_content_general" >
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_reg_edit_piso_archivo" runat="server"  TargetControlID="ButtonSalir_reg_edit_piso_archivo" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_reg_edit_piso_archivo" PopupControlID="Panel_reg_edit_piso_archivo" ></asp:ModalPopupExtender>
                <div id="modal_content_Panel_reg_edit_piso_archivo" class="modal-content">  
                <div id="div_cabecera_title" class="modal_title_superior_ modal-header">  
                    <h6 id="Label_reg_edit_piso_archivo" class="modal-title d-inline ml-1"></h6>
                    <button type="button" value="Button_cerrar_reg_edit_piso_archivo" class="close da_event_captive">&times;</button>               
                   
                </div>
                <div id="contenido_procesa_reg_edit_piso_archivo" style="background-color: white; width: 100%; height: 99%; overflow:auto" class="modal_content_back modal-body">         
                        <asp:UpdatePanel ID="UpdatePanel_reg_edit_piso_archivo" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row w-100 mt-2">
                                   <div class="col-12" style="text-align:center">
                                        <asp:Label ID="Label_reg_edit_piso_title" runat="server" Text="Regitra piso edificio"  CssClass="h6"></asp:Label>
                                   </div>         
                               </div>
                               <div class="row w-100 mt-2">
                                   <div class="col-6">
                                       <span>Telefono piso</span>
                                   </div>
                                   <div class="col-6">
                                       <asp:TextBox ID="TextBox_reg_edit_piso_telefono" runat="server" MaxLength="45" Style="width:100%" CssClass="form-control"></asp:TextBox>
                                   </div>
                               </div>
                                 <div class="row w-100 mt-2">
                                   <div class="col-6">
                                       <span>Nombre responsable</span>
                                   </div>
                                   <div class="col-6">
                                       <asp:TextBox ID="TextBox_reg_edit_piso_responsable" runat="server" MaxLength="45" Style="width:100%" CssClass="form-control"></asp:TextBox>
                                   </div>
                               </div>
                                 <div class="row w-100 mt-2">
                                   <div class="col-6">
                                       <span>Nombre piso *</span>
                                   </div>
                                   <div class="col-6">
                                       <asp:TextBox ID="TextBox_reg_edit_piso_nombre" runat="server" MaxLength="45" Style="width:100%" CssClass="form-control"></asp:TextBox>
                                   </div>
                               </div>
                                <table style="width: 100%;">
                                  
                                      <tr>
                                         <td> </td>
                                        <td></td>
                                    </tr>
                                    <tr >
                                        <td colspan="2" style="text-align:right">   </td>
                                        
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                   
                          
                </div>
                     <div class="modal-footer justify-content-end" id="modal-footer_Panel_reg_edit_piso_archivo">  
                         <asp:UpdatePanel ID="UpdatePanel_boton_reg_edit_piso" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                   <asp:Button ID="Button_reg_edit_piso_aceptar" runat="server" Text="Aceptar" CssClass="btn btn-success" />
                            </ContentTemplate>
                         </asp:UpdatePanel>
                     </div>
                     <div style="display:none; height:1px">
                        <asp:Button ID="Button_reg_edit_piso_archivo" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                          <asp:Button ID="ButtonSalir_reg_edit_piso_archivo" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                         <asp:Button ID="Button_cerrar_reg_edit_piso_archivo" runat="Server" Text="" CssClass="invisible" Height="0px" Width="0px"/>
                    </div>
                </div>
            </asp:Panel>
           
        </div>
        <div id="reg_edit_area_piso" style="clear:both">
            <asp:Panel ID="Panel_reg_edit_area_piso" runat="server" Style="display: none; color:black; width: 50%; height: auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_reg_edit_area_piso" runat="server" TargetControlID="ButtonSalir_reg_edit_area_piso" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_reg_edit_area_piso" PopupControlID="Panel_reg_edit_area_piso">
                </asp:ModalPopupExtender>
                <div id="modal_content_Panel_reg_edit_area_piso" class="modal-content">
                    <div id="div_edit_title_area_piso" class="modal_title_superior_ modal-header">
                        <h6 id="Label_reg_edit_area_piso" class="modal-title d-inline ml-1"></h6>
                        <button type="button" value="Button_cerrar_reg_edit_area_piso" class="close da_event_captive">&times;</button>
                    </div>
                    <div id="contenido_procesa_reg_edit_area_piso" style="background-color: white; width: 100%; height: 99%; overflow: auto" class="modal_content_back modal-body">
                        <asp:UpdatePanel ID="UpdatePanel_reg_edit_area_piso" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row w-100 mt-2">
                                    <div class="col-12" style="text-align: center">
                                        <asp:Label ID="Label_titulo_area_piso" runat="server" Text="Regitra área" CssClass="h6"></asp:Label>
                                    </div>

                                </div>
                                <div class="row w-100 mt-2">
                                    <div class="col-6">
                                        <span>Tipo de archivo del área *</span>
                                    </div>
                                    <div class="col-6">
                                        <asp:DropDownList ID="DropDownList_tipo_archivo_area_piso" runat="server" CssClass="custom-select">
                                            <asp:ListItem>GESTION</asp:ListItem>
                                            <asp:ListItem>CENTRAL</asp:ListItem>
                                            <asp:ListItem>HISTORICO</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row w-100 mt-2">
                                    <div class="col-6">
                                        <span>Telefono del área *</span>
                                    </div>
                                    <div class="col-6">
                                        <asp:TextBox ID="TextBox_telefono_area_piso" runat="server" MaxLength="45" Style="width: 100%" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row w-100 mt-2">
                                    <div class="col-6">
                                        <span>Nombre del responsable del área *</span>
                                    </div>
                                    <div class="col-6">
                                        <asp:TextBox ID="TextBox_responsable_area_piso" runat="server" MaxLength="45" Style="width: 100%" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row w-100 mt-2">
                                    <div class="col-6">
                                        <span>Nombre del área *</span>
                                    </div>
                                    <div class="col-6">
                                        <asp:TextBox ID="TextBox_nombre_area_piso" runat="server" MaxLength="45" Style="width: 100%" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>

                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="modal-footer justify-content-end" id="modal-footer_Panel_reg_edit_area_piso">
                        <asp:UpdatePanel ID="UpdatePanel_boton_reg_edit_piso_tool" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Button ID="Button_registrar_editar_area_piso" runat="server" Text="Aceptar" CssClass="btn btn-success" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button_reg_edit_area_piso" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="ButtonSalir_reg_edit_area_piso" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="Button_cerrar_reg_edit_area_piso" runat="Server" Text="" CssClass="invisible" Height="0px" Width="0px" />
                    </div>
                </div>
            </asp:Panel>
           
        </div>
        <div id="reg_edit_modulo_area" style="clear:both">
            <asp:Panel ID="Panel_reg_edit_modulo_area" runat="server" Style="display:none; color:black; width:50%; height:auto" CssClass="modal_content_general" >
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_reg_edit_modulo_area" runat="server"  TargetControlID="ButtonSalir_reg_edit_modulo_area" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_reg_edit_modulo_area" PopupControlID="Panel_reg_edit_modulo_area" ></asp:ModalPopupExtender>
                <div id="modal_content_Panel_reg_edit_modulo_area" class="modal-content">  
                <div id="div_edit_title_area_modulo" class="modal_title_superior_ modal-header"> 
                      <h6 id="Label_reg_edit_modulo_area" class="modal-title d-inline ml-1"></h6>
                      <button  type="button" value="Button_cerrar_reg_edit_modulo_area" class="close da_event_captive">&times;</button>                     
                </div>
                    <div id="contenido_procesa_reg_edit_modulo_area" style="background-color: white; width: 100%; height: 99%; overflow: auto" class="modal_content_back modal-body">
                        <asp:UpdatePanel ID="UpdatePanel_reg_edit_modulo_area" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row w-100 mt-2">
                                    <div class="col-12" style="text-align: center">
                                        <asp:Label ID="Label_title_reg_edit_modulo" runat="server" Text="Registra modulo" CssClass="h6"></asp:Label>
                                    </div>
                                </div>
                                <div class="row w-100 mt-2">
                                    <div class="col-6">
                                        <span>Nombre modulo *</span>
                                    </div>
                                    <div class="col-6">
                                        <asp:TextBox ID="TextBox_reg_edit_modulo_area_nombre" runat="server" MaxLength="45" Style="width: 100%" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row w-100 mt-2">
                                    <div class="col-6">
                                        <span>Descripción modulo *</span>
                                    </div>
                                    <div class="col-6">
                                        <asp:TextBox ID="TextBox_reg_edit_modulo_area_descripcion" runat="server" MaxLength="120" Style="width: 100%" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row w-100 mt-2">
                                    <div class="col-6">
                                        <span>Sección modulo *</span>
                                    </div>
                                    <div class="col-6">
                                        <asp:TextBox ID="TextBox_reg_edit_modulo_area_seccion" runat="server" MaxLength="45" Style="width: 100%" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="modal-footer justify-content-end" id="modal-footer">  
                          <asp:UpdatePanel ID="UpdatePanel_boton_reg_edit_modulo_area" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                 <asp:Button ID="Button_reg_edit_modulo_aceptar" runat="server" Text="Aceptar" CssClass="btn btn-success"  />
                            </ContentTemplate>
                        </asp:UpdatePanel>      
                    </div>  
                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button_reg_edit_modulo_area" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="ButtonSalir_reg_edit_modulo_area" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="Button_cerrar_reg_edit_modulo_area" runat="Server" Text="" CssClass="modal_boton_hiden" Height="0px" Width="0px" />
                    </div>
                </div>
            </asp:Panel>
           
        </div>
        <div id="reg_edit_estante" style="clear:both">
            <asp:Panel ID="Panel_reg_edit_estante" runat="server" Style="display:none; color: black; width:40%; height:auto" CssClass="modal_content_general" >
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_reg_edit_estante" runat="server"  TargetControlID="ButtonSalir_reg_edit_estante" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_reg_edit_estante" PopupControlID="Panel_reg_edit_estante" ></asp:ModalPopupExtender>
                <div id="modal_content_Panel_reg_edit_estante" class="modal-content">  
                <div id="div2" class="modal_title_superior_ modal-header">   
                    <h6 id="Label_reg_edit_estante" class="modal-title d-inline ml-1"></h6>
                    <button type="button" value="Button_cerrar_reg_edit_estante" class="close da_event_captive">&times;</button>                     
                </div>
                <div id="contenido_procesa_reg_edit_estante" style="background-color: white; width: 100%; height: 99%; overflow:auto" class="modal_content_back modal-body">         
                        <asp:UpdatePanel ID="UpdatePanel_reg_edit_estante" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row w-100 mt-2">
                                   <div class="col-12" style="text-align:center">
                                       <asp:Label ID="Label_title_reg_edit_estante" runat="server" Text="Registra estante" Style=""></asp:Label>
                                   </div>
                                 
                               </div>
                                <div class="row w-100 mt-2">
                                   <div class="col-6">      
                                       <span>Número de estantes * </span>
                                   </div>
                                   <div class="col-6">
                                       <asp:DropDownList ID="DropDownList_reg_edit_estante_numero" style="min-width:100px" runat="server" CssClass="custom-select">
                                                <asp:ListItem>1</asp:ListItem>
                                                <asp:ListItem>2</asp:ListItem>
                                                <asp:ListItem>3</asp:ListItem>
                                                <asp:ListItem>4</asp:ListItem>
                                                <asp:ListItem>5</asp:ListItem>
                                                <asp:ListItem>6</asp:ListItem>
                                                <asp:ListItem>7</asp:ListItem>
                                                <asp:ListItem>8</asp:ListItem>
                                                <asp:ListItem>9</asp:ListItem>
                                                <asp:ListItem>10</asp:ListItem>
                                                <asp:ListItem>11</asp:ListItem>
                                                <asp:ListItem>12</asp:ListItem>
                                                <asp:ListItem>13</asp:ListItem>
                                                <asp:ListItem>14</asp:ListItem>
                                                <asp:ListItem>15</asp:ListItem>
                                                <asp:ListItem>16</asp:ListItem>
                                                <asp:ListItem>17</asp:ListItem>
                                                <asp:ListItem>18</asp:ListItem>
                                                <asp:ListItem>19</asp:ListItem>
                                                <asp:ListItem>20</asp:ListItem>
                                            </asp:DropDownList>
                                   </div>
                               </div>
                               
                            </ContentTemplate>
                        </asp:UpdatePanel>
                   
                       
                </div>
                     <div style="display: none; height: 1px">
                        <asp:Button ID="Button_reg_edit_estante" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="ButtonSalir_reg_edit_estante" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="Button_cerrar_reg_edit_estante" runat="Server" Text="" CssClass="invisible" Height="0px" Width="0px" />
                    </div>
                     <div class="modal-footer justify-content-end" >  
                           <asp:UpdatePanel ID="UpdatePanel_boton_edit_reg_estante" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Button ID="Button_reg_edit_estante_aceptar" runat="server" Text="Aceptar" CssClass="btn  btn-success"  />
                            </ContentTemplate>
                           </asp:UpdatePanel>       
                     </div>

                </div>
            </asp:Panel>
           
        </div>
        <div id="reg_edit_entrepano" style="clear:both">
            <asp:Panel ID="Panel_reg_edit_entrepano" runat="server" Style="display:none; color: black; width:40%; height:auto" CssClass="modal_content_general" >
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_reg_edit_entrepano" runat="server"  TargetControlID="ButtonSalir_reg_edit_entrepano" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_reg_edit_entrepano" PopupControlID="Panel_reg_edit_entrepano" ></asp:ModalPopupExtender>
                <div id="modal_content_Panel_reg_edit_entrepano" class="modal-content">  
                <div id="div3" class="modal_title_superior_ modal-header">
                      <h6 ID="Label_reg_edit_entrepano" class="modal-title d-inline ml-1"></h6>
                      <button type="button" value="Button_cerrar_reg_edit_entrepano" class="close da_event_captive">&times;</button>                              
                </div>
                <div id="contenido_procesa_reg_edit_entrepano" style="background-color: white; width: 100%; height: 99%; overflow:auto" class="modal_content_back modal-body">         
                        <asp:UpdatePanel ID="UpdatePanel_reg_edit_entrepano" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row w-100 mt-2">
                                   <div class="col-12" style="text-align:center">
                                        <asp:Label ID="Label_title_reg_edit_entrepano" runat="server" Text="Registra entrepaño" CssClass="h6"  ></asp:Label>
                                   </div>
                                  
                               </div>
                                <div class="row w-100 mt-2">
                                   <div class="col-6">
                                       <asp:Label ID="Label_reg_edit_entrepano_numero" runat="server" Text="Número de entrepaños *" CssClass="h6"  ></asp:Label>
                                      
                                   </div>
                                   <div class="col-6">
                                        <asp:DropDownList ID="DropDownList_reg_edit_entrepano_numero" style="min-width:100px" runat="server" CssClass="custom-select">
                                                <asp:ListItem>1</asp:ListItem>
                                                <asp:ListItem>2</asp:ListItem>
                                                <asp:ListItem>3</asp:ListItem>
                                                <asp:ListItem>4</asp:ListItem>
                                                <asp:ListItem>5</asp:ListItem>
                                                <asp:ListItem>6</asp:ListItem>
                                                <asp:ListItem>7</asp:ListItem>
                                                <asp:ListItem>8</asp:ListItem>
                                                <asp:ListItem>9</asp:ListItem>
                                                <asp:ListItem>10</asp:ListItem>
                                                <asp:ListItem>11</asp:ListItem>
                                                <asp:ListItem>12</asp:ListItem>
                                                <asp:ListItem>13</asp:ListItem>
                                                <asp:ListItem>14</asp:ListItem>
                                                <asp:ListItem>15</asp:ListItem>
                                                <asp:ListItem>16</asp:ListItem>
                                                <asp:ListItem>17</asp:ListItem>
                                                <asp:ListItem>18</asp:ListItem>
                                                <asp:ListItem>19</asp:ListItem>
                                                <asp:ListItem>20</asp:ListItem>
                                            </asp:DropDownList>
                                   </div>
                               </div>
                                <div class="row w-100 mt-2">
                                   <div class="col-6">     
                                       <asp:Label ID="Label_reg_edit_entrepano_numero_unidades" runat="server" Text="Número de unidades de conservación permitidas *" CssClass="h6"  ></asp:Label>
                                      
                                   </div>
                                   <div class="col-6">
                                         <asp:DropDownList ID="DropDownList_reg_edit_entrepano_numero_unidades" Style="min-width: 100px" runat="server" CssClass="custom-select">
                                                <asp:ListItem>1</asp:ListItem>
                                                <asp:ListItem>2</asp:ListItem>
                                                <asp:ListItem>3</asp:ListItem>
                                                <asp:ListItem>4</asp:ListItem>
                                                <asp:ListItem>5</asp:ListItem>
                                                <asp:ListItem>6</asp:ListItem>
                                                <asp:ListItem>7</asp:ListItem>
                                                <asp:ListItem>8</asp:ListItem>
                                                <asp:ListItem>9</asp:ListItem>
                                                <asp:ListItem>10</asp:ListItem>
                                                <asp:ListItem>11</asp:ListItem>
                                                <asp:ListItem>12</asp:ListItem>
                                                <asp:ListItem>13</asp:ListItem>
                                                <asp:ListItem>14</asp:ListItem>
                                                <asp:ListItem>15</asp:ListItem>
                                                <asp:ListItem>16</asp:ListItem>
                                                <asp:ListItem>17</asp:ListItem>
                                                <asp:ListItem>18</asp:ListItem>
                                                <asp:ListItem>19</asp:ListItem>
                                                <asp:ListItem>20</asp:ListItem>
                                                <asp:ListItem>22</asp:ListItem>
                                                <asp:ListItem>23</asp:ListItem>
                                                <asp:ListItem>24</asp:ListItem>
                                                <asp:ListItem>25</asp:ListItem>
                                                <asp:ListItem>26</asp:ListItem>
                                                <asp:ListItem>27</asp:ListItem>
                                                <asp:ListItem>28</asp:ListItem>
                                                <asp:ListItem>29</asp:ListItem>
                                                <asp:ListItem>30</asp:ListItem>
                                                <asp:ListItem>31</asp:ListItem>
                                                <asp:ListItem>32</asp:ListItem>
                                                <asp:ListItem>33</asp:ListItem>
                                                <asp:ListItem>34</asp:ListItem>
                                                <asp:ListItem>35</asp:ListItem>
                                                <asp:ListItem>36</asp:ListItem>
                                                <asp:ListItem>37</asp:ListItem>
                                                <asp:ListItem>38</asp:ListItem>
                                                <asp:ListItem>39</asp:ListItem>
                                                <asp:ListItem>40</asp:ListItem>
                                            </asp:DropDownList>
                                   </div>
                               </div>
                               
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    
                          
                </div>
                    <div class="modal-footer justify-content-end" >  
                         <asp:UpdatePanel ID="UpdatePanel_boton_reg_edit_entrepano" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Button ID="Button_reg_edit_entrepano_aceptar" runat="server" Text="Aceptar" CssClass="btn btn-success" />
                            </ContentTemplate>
                         </asp:UpdatePanel>
                    </div>
                    <div style="display:none; height:1px">
                        <asp:Button ID="Button_reg_edit_entrepano" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                          <asp:Button ID="ButtonSalir_reg_edit_entrepano" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                          <asp:Button ID="Button_cerrar_reg_edit_entrepano" runat="Server" Text="" CssClass="modal_boton_hiden" Height="0px" Width="0px"/>
                    </div>
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
        <div style="display:none">
            <asp:UpdatePanel ID="updatapanel_iframe" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <iframe runat="server" style="float: left" id="ifmExcel_" width="0" height="0" marginheight="0" marginwidth="0"
                        frameborder="0" />
                </ContentTemplate>
            </asp:UpdatePanel>
            </div>
    </form>
</body>
</html>
