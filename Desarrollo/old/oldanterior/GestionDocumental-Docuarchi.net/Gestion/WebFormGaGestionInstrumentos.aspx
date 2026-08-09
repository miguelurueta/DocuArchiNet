<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormGaGestionInstrumentos.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormGaGestionInstrumentos"  ViewStateMode="Inherit"  %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">

<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
     <link href="../ccs/themes/base/jquery.ui.all.css" rel="stylesheet" type="text/css" />
    
 

    <title>Administración clasificación</title>
     <script src="../js/ui/jquery-3.4.1.min.js"></script>  
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>  
    <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
      <link href="../Styles/Aplicaction.css" rel="stylesheet" />
     <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <link href="../Styles/Menu3.css" rel="stylesheet" />
     <link href="../Styles/styleMenu.css" rel="stylesheet" type="text/css" /> 
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
     <script  src="../Awesome/js/all.js"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet"/>
     <link href="../Awesome/css/brands.css" rel="stylesheet"/>
     <link href="../Awesome/css/solid.css" rel="stylesheet"/>
     <script  src="../Awesome/js/brands.js"></script>
     <script  src="../Awesome/js/solid.js"></script>
     <script  src="../Awesome/js/fontawesome.js"></script>
     <script src="../js/java_general/general_code_java.js"></script>
    <script src="../js/gestion/WebFormGaGestionInstrumentos.js"></script>
    <script src="../js/validate_campos.js"></script>
</head>
<body>
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
                   
                    if (elment_postbak.id == "Button_me_active_men_dive") {
                        auto_zise_popup_editar_instrumento();
                        auto_zise_popup_agregar_instrumento();
                        auto_zise_popup_agregar_editar_serie();
                        auto_zise_popup_agregar_editar_sub_serie();
                    }
                    if (elment_postbak.id == "Button_seleccion_agregar" ) {
                        auto_zise_popup_agregar_editar_serie();
                        auto_zise_popup_agregar_editar_sub_serie();
                    }

                }
                catch (err) {
                    alert(" Funcion CheckStatus asincrona WebFormGaGestionInstrumentos" + err.message);
                }
                finally {
                    progres_hiden('progres_bar');
                }
            }
            </script>
        <div>
             <nav id="menu_var" class="navbar navbar-expand-sm nav_botota_person modal_content_no_back_inferior nav_botota_person_gray">
                 <button id="nav_togle_display" class="navbar-toggler" type="button" style="background-color: #6d7fcc" data-toggle="collapse" data-target="#navbarNavDropdown">
                     <span class="navbar-toggler-icon_"><i style="color: white" class="fad fa-th-list"></i></span>
                 </button>
                 <div class="collapse navbar-collapse row" id="navbarNavDropdown">
                     <ul class="navbar-nav col-md-12">
                         <li class="nav-item dropdown active ml-2 active_">
                             <a class="nav-link dropdown-toggle bot_hover_person" style="color: #6d7fcc" href="#" id="navbarDropdownMenuLink" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc" class="fad fa-edit"></i> Edición de instrumentos 
                             </a>
                             <div class="dropdown-menu " aria-labelledby="navbarDropdownMenuLink">
                                  <a href="#" class="dropdown-item" onclick="activa_menu_general_diference(event,this,'IAC-ADD-INSTRUMENTO')" title="Agregar nuevo instrumento archivistico"><i class="fal fa-plus"></i> Nuevo instrumento</a>
                                  <a href="#" class="dropdown-item" onclick="activa_menu_general_diference(event,this,'IAC-ELIM-INSTRUMENTO')" title="Elimina instrumento archivistico"><i class="fal fa-times"></i> Eliminar instrumento</a>
                                  <a href="#" class="dropdown-item" onclick="activa_menu_general_diference(event,this,'IAC-EDIDA-INSTRUMENTO')" title="Edita instrumento archivistico"><i class="fal fa-pencil"></i> Edita instrumento</a>
                                  <a href="#" class="dropdown-item" onclick="activa_menu_general_diference(event,this,'IAC-ACTIVA-INSTRUMENTO')" title="Active instrumento predeterminado para los usuarios"><i class="fad fa-exchange-alt"></i> Cambiar estado instrumento </a>
                                  <a href="#" class="dropdown-item" onclick="activa_menu_general_diference(event,this,'IAC-EXPORTA-INSTRUMENTO')" title="Exporta el área/Departamento selecionado"><i class="fal fa-file-download"></i> Exporta tabla área/Departamento</a>
                             </div>
                         </li>
                         <li class="nav-item dropdown active ml-2 active_">
                             <a class="nav-link dropdown-toggle bot_hover_person" style="color: #6d7fcc" href="#" id="A1" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc" class="far fa-edit"></i> Edición de elementos  
                             </a>
                             <div class="dropdown-menu " aria-labelledby="navbarDropdownMenuLink">
                                 <a href="#" class="dropdown-item" onclick="activa_menu_general_diference(event,this,'IAC-ADD-TABLA')" title="Agregar un nuevo elemento en la tabla"><i class="fal fa-plus"></i> Agregar elemento a la tabla</a>
                                 <a href="#" class="dropdown-item" onclick="activa_menu_general_diference(event,this,'IAC-ELIM-TABLA')" title="Eliminar el elemento seleccionado de la tabla"><i class="fal fa-times"></i> Eliminar elemento de la tabla</a>
                                 <a href="#" class="dropdown-item" onclick="activa_menu_general_diference(event,this,'IAC-EDIDA-TABLA')" title="Edita el elemento seleccionado de la tabla"><i class="fal fa-pencil"></i> Editar elemento de la tabla</a>
                                 <a href="#" class="dropdown-item" onclick="activa_menu_general_diference(event,this,'IAC-ACTIVA-TABLA')" title="Cambia estado del elemento seleccionado de la tabla"><i class="fad fa-exchange-alt"></i> Cambiar estado elemento de la tabla</a>
                             </div>
                         </li>
                     </ul>
                     
                 </div>
             </nav>
             <asp:UpdatePanel ID="UpdatePanel_menu_var_event" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <input id="Hidden_menu_var_event_dive" type="hidden" value="" runat="server" />
                        <asp:Button ID="Button_me_active_men_dive" runat="server" Text="" Style="display: none; width: 1px; height: 1px" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            
            <input id="Hidden_sel" type="hidden" value="0" runat="server"/>
            <div id="div_instrumentos" style="background-repeat: initial; height: auto; width: 100%; border: 1px solid #ccc" class="border_superior_radius_blanco_">
                <asp:UpdatePanel ID="UpdatePanel_instrumentos" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="row w-100 pt-1 pl-1 pr-1">
                            <div class="col-4">
                                <div class="row w-100 ">
                                     <div class="col-4 ">
                                         <h6 class="font-weight-normal" style="color: #6d7fcc">Organigramas</h6>
                                     </div>
                                    <div class="col-8 ">
                                        <asp:DropDownList ID="DropDownList_organigrama" runat="server" Style=" width: 100%; text-transform: uppercase; color:#0062cc" MaxLength="100" AutoPostBack="True" CssClass="custom-select"></asp:DropDownList>
                                     </div>
                                </div>
                            </div>
                              <div class="col-4">
                                <div class="row w-100">
                                     <div class="col-4">
                                         <h6 class="font-weight-normal" style="color: #6d7fcc">Instrumentos</h6>
                                     </div>
                                    <div class="col-8 ">
                                         <asp:DropDownList ID="DropDownList_instrumento" runat="server" Style="width: 100%; color:#0062cc" AutoPostBack="True" CssClass="custom-select"></asp:DropDownList>
                                     </div>
                                </div>
                            </div>
                              <div class="col-4">
                                <div class="row w-100 ">
                                     <div class="col-4">
                                         <h6 class="font-weight-normal" style="color: #6d7fcc">Áreas departamentos</h6>
                                     </div>
                                    <div class="col-8">
                                        <asp:DropDownList ID="DropDownList_areas_departamento" runat="server" Style="width: 100%; color:#0062cc" AutoPostBack="True" CssClass="custom-select"></asp:DropDownList>
                                     </div>
                                </div>
                            </div>
                        </div>
                       
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div style=" height: auto; border-left-width: 0.5px; width: 100%;  top: 0px; left: 0px">
                <asp:UpdatePanel ID="UpdatePanel_busqueda" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="TextBox_busqueda" runat="server" Style="width: 100%; color:#0062cc" placeholder="Busqueda.." CssClass="form-control"></asp:TextBox>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div id="div_treview_archivo" style=" height: auto; width: 100%" class="modal_content_back_inferior_superior">
                <asp:Panel ID="Paneltreview" runat="server" ScrollBars="Both"
                    Height="100%" Width="100%" Style="position: inherit">
                    <asp:UpdatePanel ID="UpdatePanel_treview_instrumento" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:TreeView ID="TreeViewInstrumento" runat="server" BackColor="white" EnableViewState="true"  CssClass="TreeN  pl-0 pt-1" Style="font-family:'Segoe UI'" ViewStateMode="Inherit"
                                PopulateNodesFromClient="False" RootNodeStyle-CssClass="RootNodeStyle"
                                ParentNodeStyle-CssClass="ParentNodeStyle"
                                LeafNodeStyle-CssClass="LeafNodeStyle_2_  mb-1 pl-1" ForeColor="Black"  Font-Size="14px" NodeIndent="1" ExpandDepth="0" SkipLinkText="">
                                <HoverNodeStyle Font-Underline="False" />
                                <LeafNodeStyle CssClass="LeafNodeStyle" HorizontalPadding="0px" NodeSpacing="0px" VerticalPadding="0px" />
                                <NodeStyle ChildNodesPadding="0px" HorizontalPadding="0px" NodeSpacing="0px" VerticalPadding="0px" CssClass="mt-2 mb-2 pl-2 font-weight-light" ForeColor="#0062cc" />
                                <ParentNodeStyle ChildNodesPadding="0px" CssClass="ParentNodeStyle   font-weight-bold"  HorizontalPadding="0px" NodeSpacing="0px" VerticalPadding="0px" />
                                <RootNodeStyle ChildNodesPadding="0px" CssClass="RootNodeStyle font-weight-bold"  NodeSpacing="0px" VerticalPadding="0px" HorizontalPadding="0px" />
                                <SelectedNodeStyle  CssClass="select_treview_boottra font-weight-normal nav-link-treview" />
                            </asp:TreeView>
                            <asp:Button ID="Button_activa_busqueda_treview" runat="server" Text="Button" Style="display: none" />
                            <input id="Hidden_texto_buequeda" type="hidden" value="" runat="server" />
                        </ContentTemplate>
                    </asp:UpdatePanel>

                </asp:Panel>
            </div>
            <div id="div_estado" style="text-align: right; background-repeat: initial; width: 100%; top: 0px; left: 0px" class="border_inferior_radius_blanco_">
               
            </div>
        </div>
        <!--agregar_instrumento!-->
        <div id="agregar_instrumento">
            <asp:Panel ID="Panel_agregar_instrumento" runat="server" Style="display: none; color: black; height: auto; width: 70%" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_agregar_instrumento" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_agregar_instrumento"
                    PopupControlID="Panel_agregar_instrumento" CancelControlID="Buttoncerrarimpre_agregar_instrumento">
                </asp:ModalPopupExtender>
                <div id="modal_content_Panel_agregar_instrumento" class="modal-content">
                    <div id="Divcerrarbuton2_agregar_instrumento" class="modal_title_superior_ modal-header" style="max-height:100px">
                        <h6 class="modal-title d-inline ml-1">Agregar nuevo instrumento</h6>
                        <button type="button" value="Buttoncerrarimpre_agregar_instrumento" class="close da_event_captive">&times;</button>
                    </div>
                    <div id="Contenido_agregar_instrumento" style="background-color: #FFFFFF; height: 100%; width: 100% ; border-top:none; overflow:auto" class="modal_content_back modal-body">
                        <asp:UpdatePanel ID="UpdatePane_agregar_instrumento" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row w-100 mt-2">
                                   <div class="col-6">
                                       <asp:Label ID="Label_tipo_instrumento" runat="server" Text="Tipo de instrumento *"  CssClass="h6 font-weight-light"></asp:Label>
                                   </div>
                                   <div class="col-6">
                                       <asp:DropDownList ID="DropDownList_tipo_instrumento" runat="server" Style="width: 100%; text-transform: uppercase" MaxLength="100" CssClass="custom-select"></asp:DropDownList></td>
                                   </div>
                               </div>
                                <div class="row w-100 mt-2">
                                   <div class="col-6">
                                         <asp:Label ID="Label_nombre_instrumento" runat="server" Text="Nombre del instrumento *"  CssClass="h6 font-weight-light"></asp:Label>
                                   </div>
                                   <div class="col-6">
                                        <asp:TextBox ID="TextBox_nombre_instrumento" runat="server" Style="width: 100%; text-transform: uppercase" MaxLength="100" CssClass="form-control"></asp:TextBox>
                                   </div>
                               </div>
                                <div class="row w-100 mt-2">
                                   <div class="col-6">
                                        <asp:Label ID="Label_fecha_instrumento" runat="server" Text="Fecha de creación del instrumento *"  CssClass="h6 font-weight-light"></asp:Label>
                                   </div>
                                   <div class="col-6">
                                       <div class="row p-0">
                                            <div class="col-6 p-0_">
                                                <asp:TextBox ID="TextBox_fecha_instrumento" runat="server" Width="100%" onkeypress="return validate_fecha(event,this)" placeholder="0000 00 00"></asp:TextBox>
                                                <asp:CalendarExtender ID="TextBox_fecha_instrumento_CalendarExtender" runat="server" BehaviorID="TextBox_fecha_instrumento_CalendarExtender" TargetControlID="TextBox_fecha_instrumento" Format='yyyy-MM-dd' PopupButtonID="ImageButtonCreacionIni" />
                                            </div>
                                            <div class="col-6 p-0">
                                                <button class="ml-1 btn border-0" id="ImageButtonCreacionIni" type="button">
                                                    <i class="fad fa-calendar-alt fa-1x"></i>
                                                </button>
                                            </div>
                                        </div>
                                   </div>

                               </div>
                                <div class="row w-100 mt-2">
                                   <div class="col-6">
                                        <asp:Label ID="Label_descripcion_instrumento" runat="server" Text="Descripción del Instrumento  *" CssClass="h6 font-weight-light"></asp:Label>
                                   </div>
                                   <div class="col-6">
                                       <asp:TextBox ID="TextBox_descripcion_instrumento" runat="server" Style="width: 100%" MaxLength="120" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                                   </div>
                               </div>
                                <div class="row w-100 mt-2">
                                   <div class="col-6">
                                       <asp:Label ID="Label_version_instrumento" runat="server" Text="Versión del instrumento  " CssClass="h6 font-weight-light" ></asp:Label>
                                   </div>
                                   <div class="col-6">
                                       <asp:TextBox ID="TextBox_version_instrumento" runat="server" Style="width: 50%" MaxLength="45" CssClass="form-control"></asp:TextBox>
                                   </div>
                               </div>
                                <div class="row w-100 mt-2">
                                   <div class="col-6">
                                       <asp:Label ID="Label_justificacion_instrumento" runat="server" Text="Justificación del instrumento  "  CssClass="h6 font-weight-light"></asp:Label>
                                   </div>
                                   <div class="col-6">
                                        <asp:TextBox ID="TextBox_Justificacion_instrumento" runat="server" Style="width: 100%" MaxLength="200" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                                   </div>
                               </div>
                              
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        
                    </div>
                    <div class="modal-footer justify-content-end" id="modal-footer_panel_agregar_instrumento" style="max-height:100px">
                        <asp:UpdatePanel ID="UpdatePanel_agregar_instrumento" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Button ID="Button_agregar_instrumento" runat="server" Text="Aceptar" CssClass="btn btn-success" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>   
                </div>
                <div style="display: none; height: 1px">
                    <asp:Button ID="Button1__agregar_instrumento" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="ButtonSalir_agregar_instrumento" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="Buttoncerrarimpre_agregar_instrumento" runat="Server" Text="" CssClass="modal_boton_hiden" Height="0px" Width="0px" />
                </div>
               
            </asp:Panel>
        </div>
         <!--editar_instrumento!-->
            <div style="clear:both"></div>
            <asp:Panel ID="Panel_editar_instrumento" runat="server"  Style="display:none; color: black;  height:100%; width:70%" CssClass="modal_content_general_">              
                <asp:ModalPopupExtender ID="ModalPopupExtender_editar_instrumento" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_editar_instrumento"
                    PopupControlID="Panel_editar_instrumento" CancelControlID="Buttoncerrarimpre_editar_instrumento">
                </asp:ModalPopupExtender>    
                <div id="modal_content_Panel_editar_instrumento" class="modal-content">
                    <div id="Divcerrarbuton2_editar_instrumento" class="modal_title_superior_ modal-header" style="max-height:100px">
                        <h6 class="modal-title d-inline ml-1">Editar instrumento</h6>
                        <button type="button" value="Buttoncerrarimpre_editar_instrumento" class="close da_event_captive">&times;</button>           
                    </div>
                    <div id="Contenido_editar_instrumento" style="background-color: #FFFFFF; height: 100%; width: 100%; border-top:none; overflow:auto" class="modal_content_back modal-body">
                        <asp:UpdatePanel ID="UpdatePanel_editar_instrumento" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row w-100 mt-2">
                                    <div class="col-6">
                                        <asp:Label ID="Label_tipo_instrumento_editar" runat="server" Text="Tipo de instrumento *" CssClass="h6  font-weight-light"></asp:Label>
                                    </div>
                                    <div class="col-6">
                                        <asp:DropDownList ID="DropDownList_tipo_instrumento_editar" runat="server" Style="width: 100%; text-transform: uppercase" MaxLength="100" CssClass="custom-select"></asp:DropDownList></td>
                                    </div>
                                </div>
                                <div class="row w-100 mt-2">
                                    <div class="col-6">
                                        <asp:Label ID="Label_nombre_instrumento_editar" runat="server" Text="Nombre del instrumento *" CssClass="h6 font-weight-light"></asp:Label>
                                    </div>
                                    <div class="col-6">
                                        <asp:TextBox ID="TextBox_nombre_instrumento_editar" runat="server" Style="width: 100%; text-transform: uppercase" MaxLength="100" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row w-100 mt-2">
                                    <div class="col-6">
                                        <asp:Label ID="Label_fecha_instrumento_editar" runat="server" Text="Fecha de creación del instrumento *" CssClass="h6 font-weight-light"></asp:Label>
                                    </div>
                                    <div class="col-6">
                                        <div class="row p-0">
                                            <div class="col-6 p-0_">
                                                <asp:TextBox ID="TextBox_fecha_instrumento_editar" runat="server" Width="100%" onkeypress="return validate_fecha(event,this)" placeholder="0000 00 00"></asp:TextBox>
                                                <asp:CalendarExtender ID="TextBox_fecha_instrumento_editar_CalendarExtender" runat="server" BehaviorID="TextBox_fecha_instrumento_editar_CalendarExtender" TargetControlID="TextBox_fecha_instrumento_editar" Format='yyyy-MM-dd' PopupButtonID="ImageButtonCreacionIni_editar" />
                                            </div>
                                            <div class="col-6 p-0">
                                                <button class="ml-1 btn border-0" id="ImageButtonCreacionIni_editar" type="button">
                                                    <i class="fad fa-calendar-alt fa-1x"></i>
                                                </button>
                                            </div>
                                        </div>
                                    </div>

                                </div>
                                <div class="row w-100 mt-2">
                                    <div class="col-6">
                                        <asp:Label ID="Label_descripcion_instrumento_editar" runat="server" Text="Descripción del Instrumento  *" CssClass="h6 font-weight-light"></asp:Label>
                                    </div>
                                    <div class="col-6">
                                        <asp:TextBox ID="TextBox_descripcion_instrumento_editar" runat="server" Style="width: 100%" MaxLength="120" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row w-100 mt-2">
                                    <div class="col-6">
                                        <asp:Label ID="Label_version_instrumento_editar" runat="server" Text="Versión del instrumento  " CssClass="h6 font-weight-light"></asp:Label>
                                    </div>
                                    <div class="col-6">
                                        <asp:TextBox ID="TextBox_version_instrumento_editar" runat="server" Style="width: 50%" MaxLength="45" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row w-100 mt-2">
                                    <div class="col-6">
                                        <asp:Label ID="Label_justificacion_instrumento_editar" runat="server" Text="Justificación del instrumento  " CssClass="h6 font-weight-light"></asp:Label>
                                    </div>
                                    <div class="col-6">
                                        <asp:TextBox ID="TextBox_Justificacion_instrumento_editar" runat="server" Style="width: 100%" MaxLength="200" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        
                    </div>
                    <div class="modal-footer justify-content-end" id="modal-footer_panel_editar_instrumento" style="max-height: 100px">
                            <asp:UpdatePanel ID="UpdatePane_editar_instrumento" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Button ID="Button_editar_instrumento" runat="server" Text="Aceptar" CssClass="btn btn-success" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div> 
                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button1__editar_instrumento" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="ButtonSalir_editar_instrumento" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="Buttoncerrarimpre_editar_instrumento" runat="Server" Text="X" CssClass="invisible" Height="0px" Width="0px" />
                    </div>
                </div>       
            </asp:Panel>
        
         <!--activar_inactivar!-->
         <div id="activar_inactivar">
            <asp:Panel ID="Panel_activar_inactivar" runat="server"  Style="display:none; color: black;  height:auto; width:40%" CssClass="modal_content_general">              
                <asp:ModalPopupExtender ID="ModalPopupExtender_activar_inactivar" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_activar_inactivar"
                    PopupControlID="Panel_activar_inactivar" CancelControlID="Buttoncerrarimpre_activar_inactivar">
                </asp:ModalPopupExtender>
                <div id="modal_content_Panel_activar_inactivar" class="modal-content">
                    <div id="Divcerrarbuton2_activar_inactivar" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title d-inline ml-1">Cambia estado instrumento</h6>
                        <button type="button" value="Buttoncerrarimpre_activar_inactivar" class="close da_event_captive">&times;</button>
                    </div>
                    <div id="Contenido_activar_inactivar" style="background-color: #FFFFFF; height: auto; width: 100%; border-top:none; overflow:auto" class="modal_content_back modal-body">
                        <asp:UpdatePanel ID="UpdatePanel_activar_inactivar" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row-12 w-100 mt-2">
                                    <div class="col-6">
                                        <asp:CheckBox ID="Check_activa_instrumento" runat="server" Text="" Checked="true" />
                                        <span class="ml-1">Instrumento activo</span>
                                    </div>
                                    <div class="col-6">
                                        <asp:CheckBox ID="CheckBox_inactiva_instrumento" runat="server" Text=" " Checked="false" />
                                        <span class="ml-1">Instrumento inactivo</span>
                                    </div>
                                </div>

                                <asp:MutuallyExclusiveCheckBoxExtender ID="MutuallyExclusiveCheckBoxExtender1" runat="server" TargetControlID="Check_activa_instrumento"
                                    Key="radicado_"></asp:MutuallyExclusiveCheckBoxExtender>
                                <asp:MutuallyExclusiveCheckBoxExtender ID="Mutuallyexclusivecheckboxextender2" runat="server" TargetControlID="CheckBox_inactiva_instrumento"
                                    Key="radicado_"></asp:MutuallyExclusiveCheckBoxExtender>
                                

                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="modal-footer justify-content-end">
                        <asp:UpdatePanel ID="UpdatePane_activar_inactivar" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                 <asp:Button ID="Button_activar_inactivar" runat="server" Text="Aceptar"  CssClass="btn btn-success" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>

                </div>
                <div style="display: none; height: 1px">
                    <asp:Button ID="Button1__activar_inactivar" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" Style="display: none" />
                    <asp:Button ID="ButtonSalir_activar_inactivar" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" Style="display: none" />
                    <asp:Button ID="Buttoncerrarimpre_activar_inactivar" runat="Server" Text="" Height="0px" Width="0px" Style="display: none" />
                </div>
                
            </asp:Panel>
        </div>
        <!--activar_inactivar_elemento!-->
        <div id="activar_inactivar_elemento">
            <asp:Panel ID="Panel_activar_inactivar_elemento" runat="server" Style="display: none; color: black; height: auto; width: 40%" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_activar_inactivar_elemento" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_activar_inactivar_elemento"
                    PopupControlID="Panel_activar_inactivar_elemento" CancelControlID="Buttoncerrarimpre_activar_inactivar_elemento">
                </asp:ModalPopupExtender>
                <div id="Divcerrarbuton2_activar_inactivar_elemento" class="modal_title_superior_ modal-header">
                    <h6 class="modal-title d-inline ml-1">Cambio de estado elemento</h6>
                    <button type="button" value="Buttoncerrarimpre_activar_inactivar_elemento" class="close da_event_captive">&times;</button>           
                </div>
                <div id="Contenido_activar_inactivar_elemento" style="background-color: #FFFFFF; height: auto; width: 100% ; border-top:none" class="modal_content_back modal-body">
                    <asp:UpdatePanel ID="UpdatePanel_activar_inactivar_elemento" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="row w-100 mt-2">
                                <div class="col-12">
                                     <asp:CheckBox ID="Check_activa_elemento" runat="server"  Checked="true" ForeColor="Black"  />
                                     <span class="h6 ml-2">Activo</span>
                                </div>

                            </div>
                            <div class="row w-100 mt-2">
                                <div class="col-12">
                                     <asp:CheckBox ID="CheckBox_inactiva_elemento" runat="server" Text=" " Checked="false"  />
                                     <span class="h6 ml-2">Inactivo</span>
                                </div>

                            </div>
                            <asp:MutuallyExclusiveCheckBoxExtender ID="MutuallyExclusiveCheckBoxExtender_activa_elemento" runat="server" TargetControlID="Check_activa_elemento"
                                Key="radicado_"></asp:MutuallyExclusiveCheckBoxExtender>
                            <asp:MutuallyExclusiveCheckBoxExtender ID="Mutuallyexclusivecheckboxextender_inactiva_elemento" runat="server" TargetControlID="CheckBox_inactiva_elemento"
                                Key="radicado_"></asp:MutuallyExclusiveCheckBoxExtender>
                           
                            <asp:HiddenField ID="HiddenField_oper" runat="server" Value="" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div class="modal-footer justify-content-end">
                    <asp:UpdatePanel ID="UpdatePanel_boton_activar_inactivar_elemento" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Button ID="Button_activar_inactivar_elemento" runat="server" Text="Aceptar" CssClass="btn btn-success" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div style="display: none; height: 1px">
                    <asp:Button ID="Button1__activar_inactivar_elemento" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="ButtonSalir_activar_inactivar_elemento" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="Buttoncerrarimpre_activar_inactivar_elemento" runat="Server" Text="" Height="0px" Width="0px" CssClass="invisible" />
                </div>
            </asp:Panel>
        </div>
         <!--confirmar_eliminar!-->
         <div id="confirmar_eliminar">
            <asp:Panel ID="Panel_confirmar_eliminar" runat="server"  Style="display:none; color: black;  height:auto; width:40%" CssClass="modal_content_general">              
                <asp:ModalPopupExtender ID="ModalPopupExtender_confirmar_eliminar" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_confirmar_eliminar"
                    PopupControlID="Panel_confirmar_eliminar" CancelControlID="Buttoncerrarimpre_confirmar_eliminar">
                </asp:ModalPopupExtender>
                <div id="modal_content_Panel_confirmar_eliminar" class="modal-content">
                    <div id="Divcerrarbuton2_confirmar_eliminar" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title d-inline ml-1">Confirmar</h6>
                        <button type="button" value="Buttoncerrarimpre_confirmar_eliminar" class="close da_event_captive">&times;</button>
                    </div>
                    <div id="Contenido_confirmar_eliminar" style="background-color: #FFFFFF; height: 100%; width: 100%; border-top:none" class="modal_content_back  modal-body">
                        <asp:UpdatePanel ID="UpdatePanel_confirmar_eliminar" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                               <div class="row w-100 mt-2">
                                   <div class="col-12">
                                         <asp:Label ID="Label_Confirmado" runat="server" Text=""  CssClass="h6 font-weight-light"></asp:Label>
                                          <asp:HiddenField ID="HiddenField_estado_operacion" runat="server" Value="" />
                                   </div>     
                               </div>
                              
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="modal-footer justify-content-end">
                        <asp:UpdatePanel ID="UpdatePanel_boton_confirmar_eliminar" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Button ID="Button_confirmar_eliminar" runat="server" Text="Aceptar"  CssClass="btn btn-success" />
                               
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>

                </div>
                <div style="display: none; height: 1px">
                    <asp:Button ID="Button1__confirmar_eliminar" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="ButtonSalir_confirmar_eliminar" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="Buttoncerrarimpre_confirmar_eliminar" runat="Server" Text="" Height="0px" Width="0px" />
                </div>
               
            </asp:Panel>
        </div>
        <!--agregar_serie!-->
         <div id="agregar_serie">
             <asp:Panel ID="Panel_agregar_serie" runat="server" Style="display: none; color: black; height: auto; width: 70%; overflow: auto" CssClass="modal_content_general">
                 <asp:ModalPopupExtender ID="ModalPopupExtender_agregar_serie" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_agregar_serie"
                     PopupControlID="Panel_agregar_serie" CancelControlID="Buttoncerrarimpre_agregar_serie">
                 </asp:ModalPopupExtender>
                 <div id="modal_content_Panel_agregar_serie" class="modal-content">
                     <div id="Divcerrarbuton2_agregar_serie" class="modal_title_superior_ modal-header">
                         <asp:UpdatePanel ID="UpdatePanel_title_agregar_serie" runat="server" UpdateMode="Conditional">
                             <ContentTemplate>
                                 <asp:Label ID="Label_title_agregar_serie" runat="server" class="modal-title d-inline ml-1 h6" Text="Agregar serie documental"></asp:Label>
                             </ContentTemplate>
                         </asp:UpdatePanel>
                         <button type="button" value="Buttoncerrarimpre_agregar_serie" class="close da_event_captive">&times;</button>
                     </div>
                     <div id="Contenido_agregar_serie" style="color: black; width: 100%; overflow: auto; border-top: none" class="modal_content_back modal-body">
                         <asp:UpdatePanel ID="UpdatePanel_agregar_serie" runat="server" UpdateMode="Conditional">
                             <ContentTemplate>
                                 <div class="row w-100 mt-2">
                                     <div class="col-6">
                                         <asp:Label ID="Label_agre_nmbre_serie" runat="server" Text="Nombre serie documental *" CssClass="h6 font-weight-light"></asp:Label>
                                     </div>
                                     <div class="col-6">
                                         <asp:TextBox ID="TextBox_nombre_serie" runat="server" Style="width: 100%" CssClass="form-control" MaxLength="250"></asp:TextBox>
                                     </div>
                                 </div>
                                 <div class="row w-100 mt-2">
                                     <div class="col-6">
                                         <asp:Label ID="Observaciones" runat="server" Text="Observaciones de la seriedocumental " CssClass="h6 font-weight-light"></asp:Label>
                                     </div>
                                     <div class="col-6">
                                         <asp:TextBox ID="TextBox_observaciones_serie" runat="server" Style="width: 100%" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                                     </div>
                                 </div>
                                 <div class="row w-100 mt-2">
                                     <div class="col-6">
                                         <asp:Label ID="LabelProceso" runat="server" Text="Proceso al que pertenece la serie " CssClass="h6 font-weight-light"></asp:Label>
                                     </div>
                                     <div class="col-6">
                                         <asp:TextBox ID="TextBoxProceso" runat="server" MaxLength="80" Style="width: 100%" CssClass="form-control"></asp:TextBox>
                                     </div>
                                 </div>
                                 <div class="row w-100 mt-2">
                                     <div class="col-6">
                                         <asp:Label ID="LabelProcedimiento" runat="server" Text="Procedimiento al que pertenece la serie " CssClass="h6 font-weight-light"></asp:Label>
                                     </div>
                                     <div class="col-6">
                                         <asp:TextBox ID="TextBoxProcedimiento" runat="server" MaxLength="80" Style="width: 100%" CssClass="form-control"></asp:TextBox>
                                     </div>
                                 </div>
                                 <div class="row w-100 mt-2">
                                     <div class="col-6">
                                         <asp:Label ID="Label_codigo_serie" runat="server" Text="Código serie  " CssClass="h6 font-weight-light"></asp:Label>
                                     </div>
                                     <div class="col-6">
                                         <asp:TextBox ID="TextBoxCodigoSerie" runat="server" Style="width: 70px" MaxLength="80"></asp:TextBox>
                                     </div>
                                 </div>
                                 <div class="row w-100 mt-2">
                                     <div class="col-12">
                                         <asp:CheckBox ID="CheckBox_public_serie" runat="server" Text="" Checked="True" AutoPostBack="True" />
                                         <span class="ml-1">La serie se publica en el cuadro de clasificación</span>
                                     </div>

                                 </div>
                                 <div class="row w-100 mt-2">
                                     <div class="col-12">
                                         <asp:CheckBox ID="CheckBoxDiposicion" runat="server" Text="" Checked="True" AutoPostBack="True" />
                                         <span class="ml-1">Tiene en cuenta disposición Final y Retención</span>
                                     </div>

                                 </div>
                                 <div class="row w-100 mt-2">
                                     <div class="col-12">
                                         <div style="text-align: center; background-color: #7098DD">
                                             <asp:Label ID="Label5" runat="server" Style="color: white" Text="Tiempos de retención  y medios de conservacion" CssClass="h6"></asp:Label>
                                         </div>
                                     </div>

                                 </div>
                                 <div class="row w-100 mt-2">
                                     <div class="col-6">
                                         <asp:Label ID="Label_Tiempo_retencion_archivo_gestion" runat="server" Text="Tiempo de retención archivo de gestión " CssClass="h6 font-weight-light"></asp:Label>
                                     </div>
                                     <div class="col-6">
                                         <asp:DropDownList ID="DropDownList_tiempo_retencion_gestion" runat="server" CssClass="custom-select w-25"></asp:DropDownList>
                                     </div>
                                 </div>
                                 <div class="row w-100 mt-2">
                                     <div class="col-6">
                                         <asp:Label ID="Label_Tiempo_retencion_archivo_central" runat="server" Text="Tiempo de retención archivo central " CssClass="h6 font-weight-light"></asp:Label>
                                     </div>
                                     <div class="col-6">
                                         <asp:DropDownList ID="DropDownList_tiempo_retencion_central" CssClass="custom-select w-25" runat="server"></asp:DropDownList></td>
                                     </div>
                                 </div>
                                 <div class="row w-100 mt-2">
                                     <div class="col-6">
                                         <asp:Label ID="LabelMedio" runat="server" Text="Medio de conservación  " CssClass="h6 font-weight-light"></asp:Label>
                                     </div>
                                     <div class="col-6">
                                         <asp:DropDownList ID="DropDownListMedio" runat="server" CssClass="custom-select w-45">
                                             <asp:ListItem Value=""></asp:ListItem>
                                             <asp:ListItem Value=Físico></asp:ListItem>
                                             <asp:ListItem Value=Digital></asp:ListItem>
                                             <asp:ListItem Value=físico-digital></asp:ListItem>
                                         </asp:DropDownList>
                                     </div>
                                 </div>
                                 <div class="row w-100 mt-2">
                                     <div class="col-12">
                                         <div style="text-align: center; background-color: #7098DD">
                                             <asp:Label ID="Label_disposicion_final" runat="server" Style="color: white" Text="Disposición final" CssClass="h6"></asp:Label>
                                         </div>
                                     </div>
                                 </div>
                                 <div class="row w-100 mt-2">
                                     <div class="col-3">
                                         <asp:CheckBox ID="CheckBoxConservTotal" runat="server" AutoPostBack="True" Text="" />
                                         <span class="ml-1" > Conservación total</span>
                                     </div>
                                     <div class="col-3">
                                         <asp:CheckBox ID="CheckBoxSerieEliminacion" runat="server" AutoPostBack="True" />
                                         <span class="ml-1" > Eliminación</span>
                                     </div>
                                     <div class="col-3">
                                         <asp:CheckBox ID="CheckBoxSerieDigitalizacion" runat="server" />
                                         <span class="ml-1" > Medio tecnológico</span>
                                     </div>
                                     <div class="col-3">
                                         <asp:CheckBox ID="CheckBoxSerieSeleccion" runat="server" AutoPostBack="True" Text="" />
                                         <span class="ml-1" > Selección</span>
                                     </div>
                                 </div>
                             </ContentTemplate>
                         </asp:UpdatePanel>
                     </div>
                     <div class="modal-footer justify-content-end" id="modal-footer_Panel_agregar_serie">
                         <asp:UpdatePanel ID="UpdatePane_agregar_serie" runat="server" UpdateMode="Conditional">
                             <ContentTemplate>
                                 <asp:CheckBox ID="CheckBox_ventana_visible" runat="server" Text="" Checked="true" />
                                 <span class="ml-1 mr-2">Mantiene visible ventana</span>
                                 <asp:Button ID="Button_agregar_serie" runat="server" Text="Aceptar" CssClass="btn btn-success" />
                             </ContentTemplate>
                         </asp:UpdatePanel>
                     </div>
                 </div>
                 <div style="display: none; height: 1px">
                     <asp:Button ID="Button1__agregar_serie" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                     <asp:Button ID="ButtonSalir_agregar_serie" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                     <asp:Button ID="Buttoncerrarimpre_agregar_serie" runat="Server" Text="" Height="0px" Width="0px" />
                 </div>

             </asp:Panel>
        </div>
        <!--seleccion_agregar!-->
         <div id="seleccion_agregar">
             <asp:Panel ID="Panel_seleccion_agregar" runat="server" Style="display: none; color: black; height: auto; width: 40%" CssClass="modal_content_general">
                 <asp:ModalPopupExtender ID="ModalPopupExtender_seleccion_agregar" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_seleccion_agregar"
                     PopupControlID="Panel_seleccion_agregar" CancelControlID="Buttoncerrarimpre_seleccion_agregar">
                 </asp:ModalPopupExtender>
                 <div id="modal_content_Panel_seleccion_agregar" class="modal-content">
                     <div id="Divcerrarbuton2_seleccion_agregar" class="modal_title_superior_ modal-header" >
                         <h6 class="modal-title d-inline ml-1">Selecciona el tipo elemento a agregar</h6>
                         <button type="button" value="Buttoncerrarimpre_seleccion_agregar" class="close da_event_captive">&times;</button>
                     </div>
                     <div id="Contenido_seleccion_agregar" style="background-color: #FFFFFF; height: auto; width: 100%; border-top:none" class="modal_content_back modal-body">
                         <div class="row w-100 mt-2">
                             <div class="col-12">
                                 <asp:CheckBox ID="Check_agrega_sub_serie" runat="server" Text="" Checked="true" />
                                 <span class="ml-1">Agrega sub serie</span>
                             </div>
                         </div>
                         <div class="row w-100 mt-2">
                             <div class="col-12">
                                 <asp:CheckBox ID="CheckBox_agrega_tipo" runat="server" Text=" " Checked="false" />
                                 <span class="ml-1">Agrega tipo documental</span>
                             </div>
                         </div>

                         <asp:MutuallyExclusiveCheckBoxExtender ID="MutuallyExclusiveCheckBoxExtender_agrega_sub_serie" runat="server" TargetControlID="Check_agrega_sub_serie"
                             Key="radicado__"></asp:MutuallyExclusiveCheckBoxExtender>
                         <asp:MutuallyExclusiveCheckBoxExtender ID="Mutuallyexclusivecheckboxextender_agrega_tipo" runat="server" TargetControlID="CheckBox_agrega_tipo"
                             Key="radicado__"></asp:MutuallyExclusiveCheckBoxExtender>

                     </div>
                     <div class="modal-footer justify-content-end" id="modal-footer_Panel_seleccion_agregar">
                         <asp:UpdatePanel ID="UpdatePanel_seleccion_agregar" runat="server" UpdateMode="Conditional">
                             <ContentTemplate>
                                 <asp:Button ID="Button_seleccion_agregar" runat="server" Text="Aceptar" CssClass="btn btn-success" />
                             </ContentTemplate>
                         </asp:UpdatePanel>
                     </div>
                 </div>
                 <div style="display: none; height: 1px">
                     <asp:Button ID="Button1__seleccion_agregar" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                     <asp:Button ID="ButtonSalir_seleccion_agregar" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                     <asp:Button ID="Buttoncerrarimpre_seleccion_agregar" runat="Server" Text="" CssClass="invisible" Height="0px" Width="0px" />
                 </div>

             </asp:Panel>
        </div>
        <!--agregar_sub_serie!-->
        <div id="agregar_sub_serie">
            <asp:Panel ID="Panel_agregar_sub_serie" runat="server" Style="display: none; color: black; height: 80%; width: 70%" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_agregar_sub_serie" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_agregar_sub_serie"
                    PopupControlID="Panel_agregar_sub_serie" CancelControlID="Buttoncerrarimpre_agregar_sub_serie">
                </asp:ModalPopupExtender>
                <div id="modal_content_Panel_agregar_sub_serie" class="modal-content">
                    <div id="Divcerrarbuton2_agregar_sub_serie" class="modal_title_superior_ modal-header">
                        <asp:UpdatePanel ID="UpdatePanel_title_agregar_sub_serie" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Label ID="Label_title_agregar_sub_serie" runat="server" class="modal-title d-inline ml-1" Text="Agregar serie documental"></asp:Label>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <button type="button" value="Buttoncerrarimpre_agregar_sub_serie" class="close da_event_captive">&times;</button>
                    </div>
                    <div id="Contenido_agregar_sub_serie" style="color: black; width: 100%; overflow: auto; border-top: none" class="modal_content_back modal-body">
                        <asp:UpdatePanel ID="UpdatePanel_agregar_sub_serie" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row w-100 mt-2">
                                    <div class="col-6">
                                        <asp:Label ID="Label_agre_nmbre_sub_serie" runat="server" Text="Nombre sub serie documental *" CssClass="h6 font-weight-light"></asp:Label>
                                    </div>
                                    <div class="col-6">
                                        <asp:TextBox ID="TextBox_nombre_sub_serie" runat="server" Style="width: 100%" CssClass="form-control" MaxLength="250"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row w-100 mt-2">
                                    <div class="col-6">
                                        <asp:Label ID="Observaciones_sub" runat="server" Text="Observaciones de la sub serie documental " CssClass="h6 font-weight-light"></asp:Label>
                                    </div>
                                    <div class="col-6">
                                        <asp:TextBox ID="TextBox_observaciones_sub_serie" runat="server" Style="width: 100%" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row w-100 mt-2">
                                    <div class="col-6">
                                        <asp:Label ID="LabelProceso_sub" runat="server" Text="Proceso al que pertenece la sub serie " CssClass="h6 font-weight-light"></asp:Label>
                                    </div>
                                    <div class="col-6">
                                        <asp:TextBox ID="TextBoxProceso_sub" runat="server" Style="width: 50%" MaxLength="80" CssClass="form-control"></asp:TextBox></td>
                                    </div>
                                </div>
                                <div class="row w-100 mt-2">
                                    <div class="col-6">
                                        <asp:Label ID="LabelProcedimiento_sub" runat="server" Text="Procedimiento al que pertenece la sub serie " CssClass="h6 font-weight-light"></asp:Label>
                                    </div>
                                    <div class="col-6">
                                        <asp:TextBox ID="TextBoxProcedimiento_sub" runat="server" Style="width: 50%" MaxLength="80" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row w-100 mt-2">
                                    <div class="col-6">
                                        <asp:Label ID="Label_codigo_sub_serie" runat="server" Text="Código sub serie  " CssClass="h6 font-weight-light"></asp:Label></td>
                                    </div>
                                    <div class="col-6">
                                        <asp:TextBox ID="TextBoxCodigo_sub_Serie" runat="server" Style="width: 50%" MaxLength="80" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row w-100 mt-2">
                                    <div class="col-12">
                                        <asp:CheckBox ID="CheckBox_public_sub_serie" runat="server" Text="" />
                                        <span class="ml-1 font-weight-light">La sub serie se publica en el cuadro de clasificación</span>
                                    </div>

                                </div>
                                <div class="row w-100 mt-2">
                                    <div class="col-12">
                                        <asp:CheckBox ID="CheckBoxDiposicion_sub_serie" runat="server" Text="" Checked="True" AutoPostBack="True" />
                                        <span class="ml-1 font-weight-light">Tiene en cuenta disposición Final y Retención</span>
                                    </div>

                                </div>
                                <div class="row w-100 mt-2">
                                    <div class="col-12">
                                        <div style="text-align: center; background-color: #7098DD">
                                            <asp:Label ID="Label_sub_serie" runat="server" Style="color: white" Text="Tiempos de retención  y medios de conservación" CssClass="h6"></asp:Label>
                                        </div>
                                    </div>
                                </div>
                                <div class="row w-100 mt-2">
                                    <div class="col-6">
                                        <asp:Label ID="Label_Tiempo_retencion_archivo_gestion_sub_serie" runat="server" Text="Tiempo de retención archivo de gestión " CssClass="h6 font-weight-light"></asp:Label>
                                    </div>
                                    <div class="col-6">
                                        <asp:DropDownList ID="DropDownList_tiempo_retencion_gestion_sub_serie" runat="server" CssClass="custom-select w-25"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row w-100 mt-2">
                                    <div class="col-6">
                                        <asp:Label ID="Label_Tiempo_retencion_archivo_central_sub_serie" runat="server" Text="Tiempo de retención archivo central " CssClass="h6 font-weight-light"></asp:Label>
                                    </div>
                                    <div class="col-6">
                                        <asp:DropDownList ID="DropDownList_tiempo_retencion_central_sub_serie" runat="server" CssClass="custom-select w-25"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row w-100 mt-2">
                                    <div class="col-6">
                                        <asp:Label ID="LabelMedio_sub_serie" runat="server" Text="Medio de conservación  " CssClass="h6 font-weight-light"></asp:Label>
                                    </div>
                                    <div class="col-6">
                                        <asp:DropDownList ID="DropDownListMedio_sub_serie" CssClass="custom-select w-45" runat="server">
                                            <asp:ListItem Value=""></asp:ListItem>
                                            <asp:ListItem Value="Físico"></asp:ListItem>
                                            <asp:ListItem Value="Digital"></asp:ListItem>
                                            <asp:ListItem Value="físico-digital"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row w-100 mt-2">
                                    <div class="col-12">
                                        <div style="text-align: center; background-color: #7098DD">
                                            <asp:Label ID="Label_disposicion_final_sub_serie" runat="server" Style="color: white" Text="Disposición final" CssClass="h6 font-weight-light"></asp:Label>
                                        </div>
                                    </div>
                                </div>
                                <div class="row w-100 mt-2">
                                    <div class="col-3">
                                        <asp:CheckBox ID="CheckBoxConservTotal_sub_serie" runat="server" AutoPostBack="True" Text="" />
                                        <span class="ml-1 font-weight-light">Conservación total</span>
                                    </div>
                                    <div class="col-3">
                                        <asp:CheckBox ID="CheckBoxSerieEliminacion_sub_serie" runat="server" AutoPostBack="True" Text="" />
                                        <span class="ml-1 font-weight-light">Eliminación</span>
                                    </div>
                                    <div class="col-3">
                                        <asp:CheckBox ID="CheckBoxSerieDigitalizacion_sub_serie" runat="server" Text="" />
                                        <span class="ml-1 font-weight-light">Medio tecnológico</span>
                                    </div>
                                    <div class="col-3">
                                        <asp:CheckBox ID="CheckBoxSerieSeleccion_sub_serie" runat="server" AutoPostBack="True" Text="" />
                                        <span class="ml-1 font-weight-light">Selección</span>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="modal-footer justify-content-end" id="modal-footer_Panel_agregar_sub_serie">
                        <asp:UpdatePanel ID="UpdatePane_agregar_sub_serie" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:CheckBox ID="CheckBox_visible_ventana_sub_Serie" runat="server" Text="" Checked="true" />
                                <span class="mr-2 font-weight-light">Mantiene ventana visible</span>
                                <asp:Button ID="Button_agregar_sub_serie" runat="server" Text="Aceptar" CssClass="btn btn-success" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
                <div style="display: none; height: 1px">
                    <asp:Button ID="Button1__agregar_sub_serie" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="ButtonSalir_agregar_sub_serie" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="Buttoncerrarimpre_agregar_sub_serie" runat="Server" Text="" CssClass="invisible" Height="0px" Width="0px" />
                </div>

            </asp:Panel>
        </div>
        <!--agregar_tipo_documento!-->
        <div id="agregar_tipo_documento">
            <asp:Panel ID="Panel_agregar_tipo_documento" runat="server" Style="display: none; color: black; height: auto; width: 50%" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_agregar_tipo_documento" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_agregar_tipo_documento"
                    PopupControlID="Panel_agregar_tipo_documento" CancelControlID="Buttoncerrarimpre_agregar_tipo_documento">
                </asp:ModalPopupExtender>
                <div id="modal_content_Panel_agregar_tipo_documento" class="modal-content">
                    <div id="Divcerrarbuton2_agregar_tipo_documento" class="modal_title_superior_ modal-header">
                        <asp:UpdatePanel ID="UpdatePanel_title_agregar_tipo_documento" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Label ID="Label_title_agregar_tipo_documento" runat="server" class="modal-title d-inline ml-1 h6" Text="Agregar tipo documento"></asp:Label>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <button type="button" value="Buttoncerrarimpre_agregar_tipo_documento" class="close da_event_captive">&times;</button>
                    </div>
                    <div id="Contenido_agregar_tipo_documento" style="background-color: #FFFFFF; height: auto; width: 100%; border-top: none" class="modal_content_back modal-body">
                        <asp:UpdatePanel ID="UpdatePanel_agregar_tipo_documento" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row w-100 mt-2">
                                    <div class="col-6">
                                        <asp:Label ID="Label_nombre_tipo_documento" runat="server" Text="Nombre documento *" CssClass="h6 font-weight-light"></asp:Label>
                                    </div>
                                    <div class="col-6">
                                        <asp:TextBox ID="TextBox_nombre_tipo_documento" runat="server" MaxLength="500" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row w-100 mt-2">
                                    <div class="col-6">
                                        <asp:Label ID="Label_ruta_docuemeto" runat="server" Text="Ruta documento " CssClass="h6 font-weight-light"></asp:Label>
                                    </div>
                                    <div class="col-6">
                                        <asp:TextBox ID="TextBox_ruta_documento" runat="server" Style="width: 100%" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row w-100 mt-2">
                                    <div class="col-6">
                                        <asp:Label ID="LabelCodigoDocumento" runat="server" Text="Código del documento " CssClass="h6 font-weight-light"></asp:Label>
                                    </div>
                                    <div class="col-6">
                                        <asp:TextBox ID="TextBoxCodigoDocumento" MaxLength="80" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row w-100 mt-2">
                                    <div class="col-12">
                                        <asp:CheckBox ID="CheckBox_trasv_serie" runat="server" />
                                        <span class="ml-1">Traversal a todas las series y sub series</span>
                                    </div>
                                </div>
                                <asp:HiddenField ID="HiddenField_agrega_tipo" runat="server" Value="" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="modal-footer justify-content-end" id="modal-footer">
                        <asp:UpdatePanel ID="UpdatePane_agregar_tipo_documento" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:CheckBox ID="CheckBox_visible_tipo_documento" runat="server" Text="" Checked="true" />
                                <span class="ml-2 mr-2">Mantiene ventana visible</span>
                                <asp:Button ID="Button_agregar_tipo_documento" runat="server" Text="Aceptar" CssClass="btn btn-success" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
                <div style="display: none; height: 1px">
                    <asp:Button ID="Button1__agregar_tipo_documento" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="ButtonSalir_agregar_tipo_documento" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="Buttoncerrarimpre_agregar_tipo_documento" runat="Server" Text="" CssClass="invisible" Height="0px" Width="0px" />
                </div>
            </asp:Panel>
        </div>
        <!--agregar_tipo_documento_sub_serie!-->
        <div id="agregar_tipo_documento_sub_serie">
            <asp:Panel ID="Panel_agregar_tipo_documento_sub_serie" runat="server" Style="display: none; color:black; height: auto; width: 50%" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_agregar_tipo_documento_sub_serie" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_agregar_tipo_documento_sub_serie"
                    PopupControlID="Panel_agregar_tipo_documento_sub_serie" CancelControlID="Buttoncerrarimpre_agregar_tipo_documento_sub_serie">
                </asp:ModalPopupExtender>
                <div id="modal_content_Panel_agregar_tipo_documento_sub_serie" class="modal-content">
                    <div id="Divcerrarbuton2_agregar_tipo_documento_sub_serie" class="modal_title_superior_ modal-header">
                        <asp:UpdatePanel ID="UpdatePanel_title_agregar_tipo_documento_sub_serie" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Label ID="Label_title_agregar_tipo_documento_sub_serie" runat="server" class="modal-title d-inline ml-1 h6" Text="Agregar tipo documento a sub serie documental"></asp:Label>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <button type="button" value="Buttoncerrarimpre_agregar_tipo_documento_sub_serie" class="close da_event_captive">&times;</button>
                    </div>
                    <div id="Contenido_agregar_tipo_documento_sub_serie" style="background-color: #FFFFFF; height: auto; width: 100% ; border-top:none" class="modal_content_back modal-body">
                        <asp:UpdatePanel ID="UpdatePanel_agregar_tipo_documento_sub_serie" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row w-100 mt-2">
                                    <div class="col-6">
                                        <asp:Label ID="Label_nombre_tipo_documento_sub_serie" runat="server" Text="Nombre documento *" CssClass="h6 font-weight-light"></asp:Label>
                                    </div>
                                    <div class="col-6">
                                        <asp:TextBox ID="TextBox_nombre_tipo_documento_sub_serie" runat="server" Style="width: 100%" MaxLength="100" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row w-100 mt-2">
                                    <div class="col-6">
                                        <asp:Label ID="Label_ruta_docuemeto_sub_serie" runat="server" Text="Ruta documento " CssClass="h6 font-weight-light"></asp:Label>
                                    </div>
                                    <div class="col-6">
                                        <asp:TextBox ID="TextBox_ruta_documento_sub_serie" runat="server" Style="width: 100%" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row w-100 mt-2">
                                    <div class="col-6">
                                        <asp:Label ID="LabelCodigoDocumento_sub_serie" runat="server" Text="Código del documento " CssClass="h6 font-weight-light"></asp:Label>
                                    </div>
                                    <div class="col-6">
                                        <asp:TextBox ID="TextBoxCodigoDocumento_sub_serie" runat="server" MaxLength="80" CssClass="form-control w-50"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row w-100 mt-2">
                                    <div class="col-12">
                                        <asp:CheckBox ID="CheckBox_trasv_sub_serie" runat="server" />
                                        <span class="ml-2">Traversal a todas las series y sub series</span>
                                    </div>
                                </div>
                                <asp:HiddenField ID="HiddenField_agrega_tipo_sub_serie" runat="server" Value="" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="modal-footer justify-content-end">
                        <asp:UpdatePanel ID="UpdatePane_agregar_tipo_documento_sub_serie" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:CheckBox ID="CheckBox_ventana_tipo_documental_sub_serie" runat="server" Text="" Checked="true" />
                                <span class="ml-1 mr-1">Mantiene ventan visible</span>
                                <asp:Button ID="Button_agregar_tipo_documento_sub_serie" runat="server" Text="Aceptar" CssClass="btn btn-success" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>

                </div>
                <div style="display: none; height: 1px">
                    <asp:Button ID="Button1__agregar_tipo_documento_sub_serie" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="ButtonSalir_agregar_tipo_documento_sub_serie" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="Buttoncerrarimpre_agregar_tipo_documento_sub_serie" runat="Server" Text="" CssClass="invisible" Height="0px" Width="0px" />
                </div>

            </asp:Panel>
        </div>
        <div id="inferior_bajo_boton" style="width: 0%; height: 0%; background-color: #E7EDF5; display: none">
            <asp:UpdatePanel ID="updatapanel_iframe" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <input id="Hidden_ruta_archivo" type="hidden" value="" runat="server"/>
                    <iframe runat="server" id="ifmExcel_" width="0" height="0" marginheight="0" marginwidth="0"
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
     <script accesskey="javascript" type="text/javascript">
         
    </script>
</body>
</html>
