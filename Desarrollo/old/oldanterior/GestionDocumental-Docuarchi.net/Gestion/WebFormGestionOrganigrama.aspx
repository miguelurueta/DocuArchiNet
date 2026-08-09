<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormGestionOrganigrama.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormGestionOrganigrama" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register Assembly="MindFusion.Diagramming.WebForms" Namespace="MindFusion.Diagramming.WebForms"
    TagPrefix="ndiag" %>
<%@ Register Assembly="MindFusion.Extenders" Namespace="MindFusion.Diagramming.WebForms"
    TagPrefix="ndiag" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">

<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
     
<link href="../Styles/styleMenu.css" rel="stylesheet" type="text/css" /> 
    <title>Diagramador de rutas</title>
     <script src="../js/ui/jquery-3.4.1.min.js"></script>  
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
    <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
    <link href="../ccs/themes/base/jquery.ui.all.css" rel="stylesheet" type="text/css" />
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script> 
      <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <link href="../Styles/samples.css" rel="stylesheet" />
     <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
     <script defer src="../Awesome/js/all.js"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet">
     <link href="../Awesome/css/brands.css" rel="stylesheet">
     <link href="../Awesome/css/solid.css" rel="stylesheet">
     <script defer src="../Awesome/js/brands.js"></script>
     <script defer src="../Awesome/js/solid.js"></script>
     <script defer src="../Awesome/js/fontawesome.js"></script>
    <script src="../js/gestion/WebFormGestionOrganigrama.js"></script>
    <script src="../js/java_general/general_code_java.js"></script>
    <script src="../js/validate_campos.js"></script>
    <style type="text/css">
        .auto-style1 {
            height: 26px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="True">
            <Scripts>
                <asp:ScriptReference Path="../Scripts/CustomNode.js" />
                <asp:ScriptReference Path="../Scripts/IconNode.js" />
            </Scripts>
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
                   
                    
                }
                catch (err) {
                    alert(" Funcion CheckStatus asincrona WebFormGestionOrganigrama.aspx" + err.message);
                }
                finally {
                    progres_hiden('progres_bar');

                }
            }

            </script>
    
       <nav id="menucab" class="navbar navbar-expand-sm nav_botota_person_gray modal_content_no_back_inferior">
            <button id="nav_togle_display" class="navbar-toggler" type="button" style="background-color: #6d7fcc" data-toggle="collapse" data-target="#navbarNavDropdown">
                <span class="navbar-toggler-icon_"><i style="color: white" class="fad fa-th-list"></i></span>
            </button>
            <div class="collapse navbar-collapse row" id="navbarNavDropdown">
                <ul class="navbar-nav col-md-12" >
                     <li class="nav-item dropdown active ml-2 mr-0 active_">
                        <a class="nav-link  dropdown-toggle " style="color: #6d7fcc" href="#" id="A5" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc" class="fad fa-bars"></i> Inicio
                        </a>
                         <ul class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">
                             <li><a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" onclick="activa_menu_general_diference(event,this,'ORG-ADD-ORG')"><i class="fal fa-plus"></i> Nuevo organigrama</a> </li>
                             <li><a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" onclick="activa_menu_general_diference(event,this,'ORG-ELIM-ORG')"><i class="fal fa-times"></i> Eliminar organigrama</a> </li>
                             <li><a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" onclick="activa_menu_general_diference(event,this,'ORG-EDIDA-ORG')"><i class="fal fa-pencil"></i> Edita el organigrama actual</a> </li>
                             <li><a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" onclick="activa_menu_general_diference(event,this,'ORG-ACTIVA-ORG')"><i class="fad fa-exchange-alt"></i> Cambiar estado organigrama</a> </li>
                             <li><a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" onclick="activa_menu_general_diference(event,this,'ORG-EXP-ORG')"><i class="fal fa-file-download"></i> Exportar organigrama actual</a> </li>
                         </ul>
                    </li> 
                    <li class="nav-item dropdown active ml-2 mr-0 active_">
                        <a class="nav-link  dropdown-toggle " style="color: #6d7fcc" href="#" id="A1" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc" class="fad fa-edit"></i> Edición
                        </a>
                         <ul class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">
                            <li><a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" onclick="activa_menu_general_diference(event,this,'ORG-CAMBIA-ESTADO-ORG')"><i class="fad fa-exchange-alt"></i> Cambiar estado área departamento</a></li> 
                         </ul>
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
         
            <div id="Menutol" style="height:auto; width: 100%; border: 0.2px none Black; top: 0px; left: 0px" >
                <asp:UpdatePanel ID="updatemenu" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <nav id="nav_menu" class="navbar navbar-expand-sm nav_botota_person modal_content_no_back_inferior">
                            <button class="navbar-toggler" type="button" style="background-color: #6d7fcc" data-toggle="collapse" data-target="#navbarNavDropdown_">
                                <span class="navbar-toggler-icon_"><i style="color: white" class="fad fa-th-list"></i></span>
                            </button>
                            <div class="collapse navbar-collapse row" id="navbarNavDropdown_">
                                <ul class="navbar-nav">
                                    <li class="nav-item active ml-2 active_">
                                        <a class="nav-link font-weight-light" style="color: #6d7fcc" title="Guardar cambios del organigrama" href="#" onclick="activa_boton_client_server('ImageButtonGuardar');"><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fas fa-save fa-lg"></i></a>
                                    </li>
                                </ul>
                                <asp:DropDownList ID="DropDownList_organigramas_disponibles" CssClass="custom-select w-25"
                                    runat="server" Width="200px"
                                    Style="margin-left: 10px;" AutoPostBack="True">
                                </asp:DropDownList>
                                <ul class="navbar-nav">
                                    <li class="nav-item active ml-2 active_">
                                        <a class="nav-link font-weight-light" style="color: #6d7fcc" title="Agregar área departamento" href="#" onclick="activa_boton_client_server('ImageButtonActivaCrearArea');"><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-users-medical fa-lg"></i></a>
                                    </li>
                                </ul>
                                <ul class="navbar-nav">
                                    <li class="nav-item active ml-2 active_">
                                        <a class="nav-link font-weight-light" style="color: #6d7fcc" title="Editar área o departamento" href="#" onclick="activa_boton_client_server('ImageButtonEditarArea');"><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fas fa-user-edit fa-lg"></i></a>
                                    </li>
                                </ul>
                                <ul class="navbar-nav">
                                    <li class="nav-item active ml-2 active_">
                                        <a class="nav-link font-weight-light" style="color: #6d7fcc" title="Elimina elemento seleccionado" href="#" onclick="activa_boton_client_server('ImageButtonActivaEliminarElemento');"><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-times fa-lg"></i></a>
                                    </li>
                                </ul>
                                <ul class="navbar-nav">
                                    <li class="nav-item active ml-2 active_">
                                        <a class="nav-link font-weight-light" style="color: #6d7fcc" title="Agregar relación de jerarquia" href="#" onclick="activa_boton_client_server('ImageButton_conectar_actividades');"><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="far fa-arrow-down fa-lg"></i></a>
                                    </li>
                                </ul>
                                <ul class="navbar-nav">
                                    <li class="nav-item active ml-2 active_">
                                        <a class="nav-link font-weight-light" style="color: #6d7fcc" title="Agregar relación  sub área o departamento" href="#" onclick="activa_boton_client_server('ImageButton_conectar_sub_area');"><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-arrow-to-bottom fa-lg"></i></a>
                                    </li>
                                </ul>
                                <asp:DropDownList ID="DropDownZonFactor" CssClass="custom-select ml-4 mr-4" 
                                    runat="server"  Width="70px"
                                    Style="margin-left: 3px" AutoPostBack="True">
                                </asp:DropDownList>
                                <asp:CheckBox ID="CheckBox_Grid_alineamiento" runat="server" Text=""  Checked="false" AutoPostBack="true" class="mr-0 d-none"/>
                                <span style="color: #6d7fcc" class="ml-2 d-none"> Visible grid </span>
                            </div>
                        </nav>
                        <div style="display: none">
                        <asp:ImageButton ID="ImageButtonGuardar" runat="server" />
                        <asp:ImageButton ID="ImageButtonActivaCrearArea" runat="server"  />
                        <asp:ImageButton ID="ImageButtonEditarArea" runat="server"  />
                        <asp:ImageButton ID="ImageButtonActivaEliminarElemento" runat="server"/>
                        <asp:ImageButton ID="ImageButton_conectar_actividades" runat="server" />
                        <asp:ImageButton ID="ImageButton_conectar_sub_area" runat="server"/>
                        <asp:Button ID="Button_visor_emergente" runat="server" Text="Button" Style="display: none" />
                        <asp:HiddenField ID="HiddenField_tipo_operacion" runat="server" Value="" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div> 
         <asp:UpdatePanel ID="UpdatePanel_diagran_view" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div id="content" style="background-color: #EEEEEE; width:100%">
                    <ndiag:InteractivityExtender runat="server" ID="interactivityExtender" TargetControlID="diagramView" />
                    <ndiag:DiagramView runat="server" ID="diagramView" ClientSideMode="Canvas" Behavior="Custom"
                        AllowInplaceEdit="false" JsLibraryLocation="../Scripts/MindFusion.Diagramming.js" Diagram-ShowGrid="false"
                        Style="position: absolute; left: 0px; top: 0px; right: 0px; bottom: 0px"  
                          NodeSelectedScript="onNodeSelected"  LinkSelectedScript="onLinkSelected"  EnableViewState="true" >
                    </ndiag:DiagramView>
                    <asp:HiddenField ID="HiddenField_value_selecion" runat="server"  />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <div id="footer" style="height: 24px;">
            <asp:Label ID="Label_Estado_documento" runat="server" Text="Estado" style="margin-left:5px"></asp:Label>
            
        </div>
   
        <!--paginas_externas_popou-->
          <div id="paginas_externas_popou">
            <asp:Panel ID="Panel_paginas_externas_popou" runat="server" Style="display:none; color: White; width: 100%; height: 100%">

                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_paginas_externas_popou" runat="server" BehaviorID="Panel_paginas_externas_popou" TargetControlID="ButtonSalir_paginas_externas_popou" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_paginas_externas_popou" PopupControlID="Panel_paginas_externas_popou" ></asp:ModalPopupExtender>
                <div id="divcabecer2_paginas_externas_popou"  class="cabecera2">
                   
                    <asp:Label ID="Label_paginas_externas_popou" runat="server" Text="Configuración campos de la ruta" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton2_paginas_externas_popou" style="float: right">
                        <asp:Button ID="Button_cerrar_paginas_externas_popou" runat="Server" Text="X"
                            ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_paginas_externas_popou" style="background-color: white; width: 100%; height: 99%;border: thin double #000080; color: black; background-color: #FFFFFF;">
                                
                    
                        <asp:UpdatePanel ID="UpdatePanel_paginas_externas_popou" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                              <iframe id="Iframe_paginas_externas_popup_" runat="server" frameborder="0"  style="width:100%; height:100%; overflow:hidden"></iframe>                           
                            </ContentTemplate>
                        </asp:UpdatePanel>
                         
                </div>
                 <asp:Button ID="Button_paginas_externas_popou" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_paginas_externas_popou" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
            </asp:Panel>
        </div>
       
        <!--abrir_rutas_disponibles-->
          <div id="abrir_rutas_disponibles" style="">
            <asp:Panel ID="Panel_abrir_rutas_disponibles" runat="server" Style="display:none; color: White; width: 410px; height: 120px">

                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_abrir_rutas_disponibles" runat="server" BehaviorID="Panel_abrir_rutas_disponibles" TargetControlID="ButtonSalir_abrir_rutas_disponibles" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_abrir_rutas_disponibles" PopupControlID="Panel_abrir_rutas_disponibles" ></asp:ModalPopupExtender>
                <div id="divcabecer2_abrir_rutas_disponibles"  class="cabecera2">
                   
                    <asp:Label ID="Label_abrir_rutas_disponibles" runat="server" Text="Rutas disponibles" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton2_abrir_rutas_disponibles" style="float: right">
                        <asp:Button ID="Button_cerrar_abrir_rutas_disponibles" runat="Server" Text="X"
                            ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_abrir_rutas_disponibles" style="background-color: white; width: 100%; height: 99%;border: thin double #000080; color: black; background-color: #FFFFFF;">
                                
                         <br />
                        <asp:UpdatePanel ID="UpdatePanel_abrir_rutas_disponibles" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:DropDownList ID="DropDownListrutasdisponibles" runat="server" style="width:400px; margin-left:5px"></asp:DropDownList>          
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    <div style="text-align:center; margin-top:40px">
                        <asp:UpdatePanel ID="UpdatePanel_buton_abrir_rutas_disponibles" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Button ID="Button_abrir_ruta" runat="server" Text="Aceptar" CssClass="boton" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
                
                 <asp:Button ID="Button_abrir_rutas_disponibles" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_abrir_rutas_disponibles" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
            </asp:Panel>
        </div>
        <!--cargar documento!-->
          <div id="contenido_procesa_sube_documento_adjunto" >
            <asp:Panel ID="Panel_sube_documento_adjunto" runat="server" Style="display:none" Width="619px" Height="220px">
                <asp:ModalPopupExtender ID="ModalPopupExtender_sube_documento_adjunto" runat="Server" BackgroundCssClass="ModalBackgroud_gorund" TargetControlID="Button_sube_documento_adjunto"
                    PopupControlID="Panel_sube_documento_adjunto" CancelControlID="Button3_cerrar_adjunta" ></asp:ModalPopupExtender>
                <div id="Div_cabecera" class="cabecera2">
                    
                    <asp:Label ID="Label11" runat="server" Text="Adjuntar" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Div_title_sube_documento_adjunto"" style="float: right">
                        <asp:Button ID="Button3_cerrar_adjunta" runat="Server" Text="X"
                            ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />
                    </div>
                </div>
               
                 <div id="Div_contenido_adjunta" style="border: thin double #000080; color: White; background-color: #FFFFFF; height: 100%; width: 100%">

                    <div id="Div_contenedor_file" style="background-color: white; width: 100%; height: 100%; border: thin double #000080; color: black; background-color: #FFFFFF">
                        
                        <div id="drop_zone_" style="width: 619px; height: 220px; overflow: auto">
                            <asp:UpdatePanel ID="UpdatePanel_descarga" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    
                                    <asp:AjaxFileUpload ID="AjaxFileUpload_dowload" runat="server" ThrobberID="drop_zone_"
                                        ContextKeys="fred"
                                        AllowedFileTypes="vxd"
                                        MaximumNumberOfFiles="1" OnClientUploadComplete="activa_boton_dowload" />
                                    <asp:Button ID="Button_guardar_desicion" runat="server" Text="Button" Style="display: none" />
                                    
                                    &nbsp  
                                    <asp:Label ID="Label_estado_carga" runat="server" Text="Estado" Style="font-family: Arial; font-size: 10px"></asp:Label>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                      <asp:Button ID="Button_sube_documento_adjunto" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                </div>


            </asp:Panel>
          
        </div>
       
       <!--crear_area_departamento!-->
         <div id="crear_area_departamento">
             <asp:Panel ID="Panel_crear_area_departamento" runat="server" Style="display: none; color: black; height: auto; width: 50%">
                 <asp:ModalPopupExtender ID="ModalPopupExtender_crear_area_departamento" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_crear_area_departamento"
                     PopupControlID="Panel_crear_area_departamento" CancelControlID="Buttoncerrarimpre_crear_area_departamento">
                 </asp:ModalPopupExtender>
                 <div id="modal_content_Panel_crear_area_departamento" class="modal-content">
                     <div id="Divcerrarbuton2_crear_area_departamento"  class="modal_title_superior_ modal-header">
                         <h6 class="modal-title d-inline ml-1">Agregar área o departamento</h6>
                         <button type="button" value="Buttoncerrarimpre_crear_area_departamento" class="close da_event_captive">&times;</button>
                     </div>
                     <div id="Contenido_crear_area_departamento" style="background-color: #FFFFFF; height: auto; width: 100%; border: none; overflow:auto" class="modal_content_back modal-body">
                         <asp:UpdatePanel ID="UpdatePanel_crear_area_departamento" runat="server" UpdateMode="Conditional">
                             <ContentTemplate>
                                 <div class="row w-100 mt-2">
                                     <div class="col-6">
                                         <asp:Label ID="Label_nombre_area_dependencia" runat="server" Text="Nombre área dependencia *" CssClass="h6 font-weight-light"></asp:Label>
                                     </div>
                                     <div class="col-6">
                                         <asp:TextBox ID="TextBox_nombre_area_dependencia" runat="server" Style="width: 100%; text-transform: uppercase" MaxLength="100" CssClass="form-control"></asp:TextBox>
                                     </div>
                                 </div>
                                 <div class="row w-100 mt-2">
                                     <div class="col-6">
                                         <asp:Label ID="Label_codigo_arbitrario" runat="server" Text="Código arbitrario *" CssClass="h6 font-weight-light"></asp:Label>
                                     </div>
                                     <div class="col-6">
                                         <asp:TextBox ID="TextBox_codigo_arbitrario" runat="server" Style="width: 50%" MaxLength="45" CssClass="form-control"></asp:TextBox>
                                     </div>
                                 </div>
                                 <div class="row w-100 mt-2">
                                     <div class="col-6">
                                         <asp:Label ID="Label_descripcion_area" runat="server" Text="Descripción área departamento *" CssClass="h6 font-weight-light"></asp:Label>
                                     </div>
                                     <div class="col-6">
                                         <asp:TextBox ID="TextBox_descripcion_area" runat="server" Style="width: 50%" MaxLength="45" CssClass="form-control"></asp:TextBox>
                                     </div>
                                 </div>
                                 <div class="row w-100 mt-2">
                                     <div class="col-6">
                                     </div>
                                     <div class="col-6">
                                         <asp:CheckBox ID="CheckBox_activa_pqrs" runat="server" Text="" />
                                         <span class="ml-2">Área departamento activa para PQRS</span>
                                     </div>
                                 </div>
                                 <div class="row w-100 mt-2">
                                     <div class="col-6">
                                     </div>
                                     <div class="col-6">
                                         <asp:CheckBox ID="CheckBox_activa_publica" runat="server" Text="" />
                                         <span class="ml-2">Área departamento activa para consulta pública</span>
                                     </div>
                                 </div>

                             </ContentTemplate>
                         </asp:UpdatePanel>
                     </div>
                     <div class="modal-footer justify-content-end" id="modal-footer_Panel_crear_area_departamento">
                         <asp:UpdatePanel ID="UpdatePane_crear_area_departamento" runat="server" UpdateMode="Conditional">
                             <ContentTemplate>
                                 <asp:Button ID="Button_agregar_area" runat="server" Text="Aceptar" CssClass="btn btn-success" />
                                 <asp:HiddenField ID="HiddenField_estado_guarda" runat="server" Value="" />
                             </ContentTemplate>
                         </asp:UpdatePanel>
                     </div>
                 </div>
                 <div style="display: none; height: 1px">
                     <asp:Button ID="Button1__crear_area_departamento" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                     <asp:Button ID="ButtonSalir_crear_area_departamento" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                     <asp:Button ID="Buttoncerrarimpre_crear_area_departamento" runat="Server" Text="" Height="0px" Width="0px" />
                 </div>

             </asp:Panel>
        </div>
         <!--editar_area_departamento!-->
         <div id="editar_area_departamento">
             <asp:Panel ID="Panel_editar_area_departamento" runat="server" Style="display: none; color: black; height: auto; width: 50%">
                 <asp:ModalPopupExtender ID="ModalPopupExtender_editar_area_departamento" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_editar_area_departamento"
                     PopupControlID="Panel_editar_area_departamento" CancelControlID="Buttoncerrarimpre_editar_area_departamento">
                 </asp:ModalPopupExtender>
                 <div id="modal_content_Panel_editar_area_departamento" class="modal-content">
                     <div id="Divcerrarbuton2_editar_area_departamento" class="modal_title_superior_ modal-header">
                         <h6 class="modal-title d-inline ml-1">Editar área o departamento</h6>
                         <button type="button" value="Buttoncerrarimpre_editar_area_departamento" class="close da_event_captive">&times;</button>
                     </div>
                     <div id="Contenido_editar_area_departamento" style="background-color: #FFFFFF; height: auto; width: 100%; border-top: none; overflow:auto" class="modal_content_back modal-body">
                         <asp:UpdatePanel ID="UpdatePanel_editar_area_departamento" runat="server" UpdateMode="Conditional">
                             <ContentTemplate>
                                 <div class="row w-100 mt-2">
                                     <div class="col-6">
                                         <asp:Label ID="Label_editar_nombre_area_dependencia" runat="server" Text="Nombre área dependencia *" CssClass="h6 font-weight-light"></asp:Label>
                                     </div>
                                     <div class="col-6">
                                         <asp:TextBox ID="TextBox_editar_nombre_area_dependencia" runat="server" Style="width: 100%; text-transform: uppercase" MaxLength="100" CssClass="form-control"></asp:TextBox>
                                     </div>
                                 </div>
                                 <div class="row w-100 mt-2">
                                     <div class="col-6">
                                         <asp:Label ID="Label_editar_codigo_arbitrario" runat="server" Text="Código arbitrario *" CssClass="h6 font-weight-light"></asp:Label>
                                     </div>
                                     <div class="col-6">
                                         <asp:TextBox ID="TextBox_editar_codigo_arbitrario" runat="server" Style="width: 50%" MaxLength="45" CssClass="form-control"></asp:TextBox>
                                     </div>
                                 </div>
                                 <div class="row w-100 mt-2">
                                     <div class="col-6">
                                         <asp:Label ID="Label_editar_descripcion_area" runat="server" Text="Descripción área departamento *" CssClass="h6 font-weight-light"></asp:Label>
                                     </div>
                                     <div class="col-6">
                                         <asp:TextBox ID="TextBox_editar_descripcion_area" runat="server" Style="width: 50%" MaxLength="45" CssClass="form-control"></asp:TextBox>
                                     </div>
                                 </div>
                                 <div class="row w-100 mt-2">
                                     <div class="col-6">
                                     </div>
                                     <div class="col-6">
                                         <asp:CheckBox ID="CheckBox_editar_activa_pqrs" runat="server" Text="" />
                                         <span class="ml-2">Área departamento activa para PQRS</span>
                                     </div>
                                 </div>
                                 <div class="row w-100 mt-2">
                                     <div class="col-6">
                                     </div>
                                     <div class="col-6">
                                         <asp:CheckBox ID="CheckBox_editar_activa_publica" runat="server" Text="" />
                                         <span class="ml-2">Área departamento activa para consulta pública</span>
                                     </div>
                                 </div>
                             </ContentTemplate>
                         </asp:UpdatePanel>
                     </div>
                     <div class="modal-footer justify-content-end" id="modal-footer_Panel_editar_area_departamento">
                         <asp:UpdatePanel ID="UpdatePane_editar_area_departamento" runat="server" UpdateMode="Conditional">
                             <ContentTemplate>
                                 <asp:Button ID="Button_edita_area" runat="server" Text="Aceptar" CssClass="btn btn-success" />
                                 <asp:HiddenField ID="HiddenField_editar_estado_guarda" runat="server" Value="" />
                             </ContentTemplate>
                         </asp:UpdatePanel>
                     </div>
                 </div>
                 <div style="display: none; height: 1px">
                     <asp:Button ID="Button1__editar_area_departamento" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                     <asp:Button ID="ButtonSalir_editar_area_departamento" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                     <asp:Button ID="Buttoncerrarimpre_editar_area_departamento" runat="Server" Text="" Height="0px" Width="0px" />
                 </div>

             </asp:Panel>
        </div>
        <!--confirmar_eliminar!-->
         <div id="confirmar_eliminar">
             <asp:Panel ID="Panel_confirmar_eliminar" runat="server" Style="display: none; color:black; height: auto; width: 50%">
                 <asp:ModalPopupExtender ID="ModalPopupExtender_confirmar_eliminar" runat="Server" BackgroundCssClass="FondoAplicacion" 
                      TargetControlID="ButtonSalir_confirmar_eliminar"
                     PopupControlID="Panel_confirmar_eliminar" CancelControlID="Buttoncerrarimpre_confirmar_eliminar">
                 </asp:ModalPopupExtender>
                 <div id="modal_content_Panel_confirmar_eliminar" class="modal-content">
                     <div id="Divcerrarbuton2_confirmar_eliminar" class="modal_title_superior_ modal-header">
                         <h6 class="modal-title d-inline ml-1">Confirmar</h6>
                         <button type="button" value="Buttoncerrarimpre_confirmar_eliminar" class="close da_event_captive">&times;</button>
                     </div>
                     <div id="Contenido_confirmar_eliminar" style="background-color: #FFFFFF; height: 100%; width: 100%" class="modal_content_back modal-body">
                         <asp:UpdatePanel ID="UpdatePanel_confirmar_eliminar" runat="server" UpdateMode="Conditional">
                             <ContentTemplate>
                                 <div class="row w-100 mt-2">
                                     <div class="col-12 pl-2">
                                         <asp:Label ID="Label_Confirmado" runat="server" Text="" CssClass="h6  font-weight-light"></asp:Label>
                                     </div>
                                 </div>
                             </ContentTemplate>
                         </asp:UpdatePanel>
                     </div>
                     <div class="modal-footer justify-content-end">
                         <asp:UpdatePanel ID="UpdatePane_confirmar_eliminar" runat="server" UpdateMode="Conditional">
                             <ContentTemplate>
                                  <asp:Button ID="Button_confirmar_eliminar" runat="server" Text="Aceptar"  CssClass="btn btn-success" />
                                     <asp:Button ID="Button_cancelar_eliminar" runat="server" Text="Cancelar"  CssClass="btn btn-light" />
                                     <asp:HiddenField ID="HiddenField_estado_operacion" runat="server" Value="" />
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
        <!--activar_inactivar!-->
         <div id="activar_inactivar">
             <asp:Panel ID="Panel_activar_inactivar" runat="server" Style="display: none; color: black; height: auto; width: 40%">
                 <asp:ModalPopupExtender ID="ModalPopupExtender_activar_inactivar" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_activar_inactivar"
                     PopupControlID="Panel_activar_inactivar" CancelControlID="Buttoncerrarimpre_activar_inactivar">
                 </asp:ModalPopupExtender>
                 <div id="modal_content_Panel_activar_inactivar" class="modal-content">
                     <div id="Divcerrarbuton2_activar_inactivar" class="modal_title_superior_ modal-header">
                         <h6 class="modal-title d-inline ml-1">Cambio de estado área o departamento</h6>
                         <button type="button" value="Buttoncerrarimpre_activar_inactivar" class="close da_event_captive">&times;</button>
                     </div>
                     <div id="Contenido_activar_inactivar" style="background-color: #FFFFFF; height: auto; width: 100%; border-top: none" class="modal_content_back modal-body">
                         <asp:UpdatePanel ID="UpdatePanel_activar_inactivar" runat="server" UpdateMode="Conditional">
                             <ContentTemplate>
                                 <div class="row w-100 mt-2">
                                     <div class="col-12">
                                         <asp:CheckBox ID="Check_activa_area" runat="server" Text="" Checked="true" />
                                         <span class="ml-2">Area departamento activa</spa>
                                     </div>
                                 </div>
                                 <div class="row w-100 mt-2">
                                     <div class="col-12">
                                         <asp:CheckBox ID="CheckBox_inactiva_area" runat="server" Text="" Checked="false" />
                                         <span class="ml-2">Area departamento inactiva</spa>
                                     </div>
                                 </div>
                                 <asp:MutuallyExclusiveCheckBoxExtender ID="MutuallyExclusiveCheckBoxExtender1" runat="server" TargetControlID="Check_activa_area"
                                     Key="radicado_"></asp:MutuallyExclusiveCheckBoxExtender>
                                 <asp:MutuallyExclusiveCheckBoxExtender ID="Mutuallyexclusivecheckboxextender2" runat="server" TargetControlID="CheckBox_inactiva_area"
                                     Key="radicado_"></asp:MutuallyExclusiveCheckBoxExtender>
                             </ContentTemplate>
                         </asp:UpdatePanel>
                     </div>
                     <div class="modal-footer justify-content-end">
                         <asp:UpdatePanel ID="UpdatePane_activar_inactivar" runat="server" UpdateMode="Conditional">
                             <ContentTemplate>
                                 <asp:Button ID="Button_activar_inactivar" runat="server" Text="Aceptar" CssClass="btn btn-success" />
                                 <asp:Button ID="Button_cancelar_inactivar_activar" runat="server" Text="Cancelar" CssClass="btn btn-light" />
                             </ContentTemplate>
                         </asp:UpdatePanel>
                     </div>
                 </div>
                 <div style="display: none; height: 1px">
                     <asp:Button ID="Button1__activar_inactivar" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                     <asp:Button ID="ButtonSalir_activar_inactivar" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                     <asp:Button ID="Buttoncerrarimpre_activar_inactivar" runat="Server" Text="" Height="0px" Width="0px" />
                 </div>

             </asp:Panel>
        </div>
        <!--crear_area_workflow-->
          <div id="crear_actividad_workflow" style="">
            <asp:Panel ID="Panel_crear_actividad_workflow" runat="server" Style="display:none; color: White; width: 410px; height: 170px">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_crear_actividad_workflow" runat="server" BehaviorID="Panel_crear_actividad_workflow" TargetControlID="ButtonSalir_crear_actividad_workflow" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_crear_actividad_workflow" PopupControlID="Panel_crear_actividad_workflow" ></asp:ModalPopupExtender>
                <div id="divcabecer2_crear_actividad_workflow"  class="cabecera2">                  
                    <asp:Label ID="Label_crear_actividad_workflow" runat="server" Text="Crear actividad" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton2_crear_actividad_workflow" style="float: right">
                        <asp:Button ID="Button_cerrar_crear_actividad_workflow" runat="Server" Text="X"
                            ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_crear_actividad_workflow" style="background-color: white; width: 100%; height: 99%;border: thin double #000080; color: black; background-color: #FFFFFF">                                
                         <br />
                        <asp:UpdatePanel ID="UpdatePanel_crear_actividad_workflow" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                 <asp:HiddenField ID="HiddenField_tipo_actividad_seleccion" runat="server"  Value="" />
                                <table style="width: 100%;">
                                    <tr>
                                        <td> <asp:Label ID="Label_crear_actividad" runat="server" Text="Nombre actividad *" style="margin-left:5px"></asp:Label></td>
                                        <td><asp:TextBox ID="TextBox_nombre_actvidad_crear_actividad" runat="server" style="margin-left:5px; width:200px; text-transform: uppercase" MaxLength="45" ></asp:TextBox></td>                           
                                    </tr>
                                    <tr>
                                        <td><asp:Label ID="Label_descripcion_crear_actividad" runat="server" Text="Descripcíon actividad" style="margin-left:5px"></asp:Label></td>
                                        <td><asp:TextBox ID="TextBox_descripcion_crear_actividad" runat="server" style="margin-left:5px; width:250px" MaxLength="45"></asp:TextBox></td>                                      
                                    </tr>
                                    <tr >
                                       
                                        <td colspan="2">
                                            <asp:CheckBox ID="CheckBox_option_crea_grupo_workflow" Text="Crea y relaciona grupo workflow a la actividad" Checked="true" runat="server" /></td>
                                    </tr>                         
                                </table>
                              
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    <div style="text-align:center; margin-top:40px">
                        <asp:UpdatePanel ID="UpdatePanel_buton_crear_actividad_workflow" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Button ID="Button_crear_actividad_workflow_confirmar" runat="server" Text="Aceptar" CssClass="boton" />
                               
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
                
                 <asp:Button ID="Button_crear_actividad_workflow" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_crear_actividad_workflow" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
            </asp:Panel>
        </div>
       <!--agregar_organigrama!-->
         <div id="agregar_organigrama">
             <asp:Panel ID="Panel_agregar_organigrama" runat="server" Style="display: none; color: black; height: auto; width: 50%" CssClass="modal_content_general_">
                 <asp:ModalPopupExtender ID="ModalPopupExtender_agregar_organigrama" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_agregar_organigrama"
                     PopupControlID="Panel_agregar_organigrama" CancelControlID="Buttoncerrarimpre_agregar_organigrama">
                 </asp:ModalPopupExtender>
                 <div id="modal_content_Panel_agregar_organigrama" class="modal-content">
                     <div id="Divcerrarbuton2_agregar_organigrama" class="modal_title_superior_ modal-header">
                         <h6 class="modal-title d-inline ml-1">Agregar nuevo organigrama</h6>
                         <button type="button" value="Buttoncerrarimpre_agregar_organigrama" class="close da_event_captive">&times;</button>
                     </div>
                     <div id="Contenido_agregar_organigrama" style="background-color: #FFFFFF; height: 100%; width: 100%; overflow: auto; border-top: none" class="modal_content_back modal-body">
                         <asp:UpdatePanel ID="UpdatePanel_agregar_organigrama" runat="server" UpdateMode="Conditional">
                             <ContentTemplate>
                                 <div class="row w-100 mt-2">
                                     <div class="col-6">
                                         <asp:Label ID="Label_nombre_organigrama" runat="server" Text="Nombre organigrama *" CssClass="h6 font-weight-light"></asp:Label>
                                     </div>
                                     <div class="col-6">
                                         <asp:TextBox ID="TextBox_nombre_organigrama" runat="server" Style="width: 100%; text-transform: uppercase" MaxLength="100" CssClass="form-control"></asp:TextBox>
                                     </div>
                                 </div>
                                 <div class="row w-100 mt-2">
                                     <div class="col-6">
                                         <asp:Label ID="Label_codigo_resolucion" runat="server" Text="Número de resolución o acto admon *" CssClass="h6 font-weight-light"></asp:Label>
                                     </div>
                                     <div class="col-6">
                                         <asp:TextBox ID="TextBox_codigo_resulucion" runat="server" Style="width: 100%" MaxLength="45" CssClass="form-control"></asp:TextBox>
                                     </div>
                                 </div>
                                 <div class="row w-100 mt-2">
                                     <div class="col-6">
                                         <asp:Label ID="Label_fecha_organigrama" runat="server" Text="Fecha creación del organigrama *" CssClass="h6 font-weight-light"></asp:Label>
                                     </div>
                                     <div class="col-6">
                                         <div class="row p-0">
                                             <div class="col-6 p-0_">
                                                 <asp:TextBox ID="TextBox_fecha_organigrama" runat="server" Width="100%" onkeypress="return validate_fecha(event,this)" placeholder="0000 00 00"></asp:TextBox>
                                                 <asp:CalendarExtender ID="TextBox_fecha_instrumento_CalendarExtender" runat="server" BehaviorID="TextBox_fecha_instrumento_CalendarExtender" TargetControlID="TextBox_fecha_organigrama" Format='yyyy-MM-dd' PopupButtonID="ImageButtonCreacionIni" />

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
                                         <asp:Label ID="Label_detalle_resolucion" runat="server" Text="Detalle de resolución o acto admon  *" CssClass="h6 font-weight-light"></asp:Label>
                                     </div>
                                     <div class="col-6">
                                         <asp:TextBox ID="TextBox_descripcion_resolucion" runat="server" Style="width: 100%" MaxLength="120" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                                     </div>
                                 </div>
                                 <div class="row w-100 mt-2">
                                     <div class="col-6">
                                         <asp:Label ID="Label_version_organigrama" runat="server" Text="Versión del organigrama  " CssClass="h6 font-weight-light"></asp:Label>
                                     </div>
                                     <div class="col-6">
                                         <asp:TextBox ID="TextBox_version_organigrama" runat="server" Style="width: 50%" MaxLength="45" CssClass="form-control"></asp:TextBox>
                                     </div>
                                 </div>
                                 <div class="row w-100 mt-2">
                                     <div class="col-6">
                                         <asp:Label ID="Label_codigo_norma" runat="server" Text="Código norma  " CssClass="h6 font-weight-light"></asp:Label>
                                     </div>
                                     <div class="col-6">
                                         <asp:TextBox ID="TextBox_codigo_norma" runat="server" Style="width: 50%" MaxLength="45" CssClass="form-control"></asp:TextBox></td>
                                     </div>
                                 </div>
                             </ContentTemplate>
                         </asp:UpdatePanel>
                     </div>
                     <div class="modal-footer justify-content-end" id="modal-footer_Panel_agregar_organigrama">
                         <asp:UpdatePanel ID="UpdatePane_agregar_organigrama" runat="server" UpdateMode="Conditional">
                             <ContentTemplate>
                                 <asp:Button ID="Button_agregar_organigrama" runat="server" Text="Aceptar" CssClass="btn btn-success" />
                             </ContentTemplate>
                         </asp:UpdatePanel>
                     </div>
                 </div>
                
                 <div style="display: none; height: 1px">
                     <asp:Button ID="Button1__agregar_organigrama" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                     <asp:Button ID="ButtonSalir_agregar_organigrama" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                     <asp:Button ID="Buttoncerrarimpre_agregar_organigrama" runat="Server" Text="" Height="0px" Width="0px" />
                 </div>

             </asp:Panel>
        </div>
       <!--editar_organigrama!-->
         <div id="editar_organigrama">
             <asp:Panel ID="Panel_editar_organigrama" runat="server" Style="display: none; color: black; height: auto; width: 50%">
                 <asp:ModalPopupExtender ID="ModalPopupExtender_editar_organigrama" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_editar_organigrama"
                     PopupControlID="Panel_editar_organigrama" CancelControlID="Buttoncerrarimpre_editar_organigrama">
                 </asp:ModalPopupExtender>
                 <div id="modal_content_Panel_editar_organigrama" class="modal-content">
                     <div id="Divcerrarbuton2_editar_organigrama" class="modal_title_superior_ modal-header">
                         <h6 class="modal-title d-inline ml-1">Editar organigrama</h6>
                         <button type="button" value="Buttoncerrarimpre_editar_organigrama" class="close da_event_captive">&times;</button>
                     </div>
                     <div id="Contenido_editar_organigrama" style=" background-color: #FFFFFF; height:100%; width: 100%; border-top:none; overflow:auto"  class="modal_content_back modal-body">
                         <asp:UpdatePanel ID="UpdatePanel_editar_organigrama" runat="server" UpdateMode="Conditional">
                             <ContentTemplate>
                                 <div class="row w-100 mt-2">
                                     <div class="col-6">
                                         <asp:Label ID="Label_nombre_organigrama_editar" runat="server" Text="Nombre organigrama *" CssClass="h6 font-weight-light"></asp:Label>
                                     </div>
                                     <div class="col-6">
                                         <asp:TextBox ID="TextBox_nombre_organigrama_editar" runat="server" Style="width: 100%; text-transform: uppercase" MaxLength="100" CssClass="form-control"></asp:TextBox>
                                     </div>
                                 </div>
                                 <div class="row w-100 mt-2">
                                     <div class="col-6">
                                         <asp:Label ID="Label_codigo_resolucion_editar" runat="server" Text="Número de resolución o acto admon *" CssClass="h6 font-weight-light"></asp:Label>
                                     </div>
                                     <div class="col-6">
                                         <asp:TextBox ID="TextBox_codigo_resulucion_editar" runat="server" Style="width: 100%" MaxLength="45" CssClass="form-control"></asp:TextBox>
                                     </div>
                                 </div>
                                 <div class="row w-100 mt-2">
                                     <div class="col-6">
                                         <asp:Label ID="Label_fecha_organigrama_editar" runat="server" Text="Fecha creación del organigrama *" CssClass="h6 font-weight-light"></asp:Label>
                                     </div>
                                     <div class="col-6">
                                         <div class="row p-0">
                                             <div class="col-6 p-0_">
                                                 <asp:TextBox ID="TextBox_fecha_organigrama_editar" runat="server" Width="100%" onkeypress="return validate_fecha(event,this)" placeholder="0000 00 00"></asp:TextBox>
                                                 <asp:CalendarExtender ID="TextBox_fecha_instrumento_CalendarExtender_editar" runat="server" BehaviorID="TextBox_fecha_instrumento_CalendarExtender_editar" TargetControlID="TextBox_fecha_organigrama_editar" Format='yyyy-MM-dd' PopupButtonID="ImageButtonCreacionIni_editar" />

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
                                         <asp:Label ID="Label_detalle_resolucion_editar" runat="server" Text="Detalle de resolución o acto admon  *" CssClass="h6 font-weight-light"></asp:Label>
                                     </div>
                                     <div class="col-6">
                                         <asp:TextBox ID="TextBox_descripcion_resolucion_editar" runat="server" Style="width: 100%" MaxLength="120" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                                     </div>
                                 </div>
                                 <div class="row w-100 mt-2">
                                     <div class="col-6">
                                         <asp:Label ID="Label_version_organigrama_editar" runat="server" Text="Versión del organigrama  " CssClass="h6 font-weight-light"></asp:Label>
                                     </div>
                                     <div class="col-6">
                                         <asp:TextBox ID="TextBox_version_organigrama_editar" runat="server" Style="width: 50%" MaxLength="45" CssClass="form-control"></asp:TextBox>
                                     </div>
                                 </div>
                                 <div class="row w-100 mt-2">
                                     <div class="col-6">
                                         <asp:Label ID="Label_codigo_norma_editar" runat="server" Text="Código norma  " CssClass="h6 font-weight-light"></asp:Label>
                                     </div>
                                     <div class="col-6">
                                         <asp:TextBox ID="TextBox_codigo_norma_editar" runat="server" Style="width: 50%" MaxLength="45" CssClass="form-control"></asp:TextBox></td>
                                     </div>
                                 </div>
                             </ContentTemplate>
                         </asp:UpdatePanel>
                     </div>
                     <div class="modal-footer justify-content-end" id="modal-footer_Panel_editar_organigrama">
                         <asp:UpdatePanel ID="UpdatePane_editar_organigrama" runat="server" UpdateMode="Conditional">
                             <ContentTemplate>
                                 <asp:Button ID="Button_editar_organigrama_editar" runat="server" Text="Aceptar"  CssClass="btn btn-success" />
                             </ContentTemplate>
                         </asp:UpdatePanel>
                     </div>
                 </div>
                 <div style="display: none; height: 1px">
                     <asp:Button ID="Button1__editar_organigrama" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                     <asp:Button ID="ButtonSalir_editar_organigrama" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                     <asp:Button ID="Buttoncerrarimpre_editar_organigrama" runat="Server" Text="" Height="0px" Width="0px" />
                 </div>

             </asp:Panel>
        </div>
        <!--cambia_estado_organigrama!-->
         <div id="cambia_estado_organigrama">
             <asp:Panel ID="Panel_cambia_estado_organigrama" runat="server" Style="display: none; color: black; height: auto; width: 40%">
                 <asp:ModalPopupExtender ID="ModalPopupExtender_cambia_estado_organigrama" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_cambia_estado_organigrama"
                     PopupControlID="Panel_cambia_estado_organigrama" CancelControlID="Buttoncerrarimpre_cambia_estado_organigrama">
                 </asp:ModalPopupExtender>
                 <div id="modal_content_Panel_cambia_estado_organigrama" class="modal-content">
                     <div id="Divcerrarbuton2_cambia_estado_organigrama" class="modal_title_superior_ modal-header">
                         <h6 class="modal-title d-inline ml-1">Cambia de estado organigrama</h6>
                         <button type="button" value="Buttoncerrarimpre_cambia_estado_organigrama" class="close da_event_captive">&times;</button>
                     </div>
                     <div id="Contenido_cambia_estado_organigrama" style="background-color: #FFFFFF; height: auto; width: 100%; border-top: none" class="modal_content_back modal-body">
                         <asp:UpdatePanel ID="UpdatePanel_cambia_estado_organigrama" runat="server" UpdateMode="Conditional">
                             <ContentTemplate>
                                 <div class="row w-100 mt-2">
                                     <div class="col-12">
                                         <asp:CheckBox ID="Check_activa_organigrama" runat="server" Text="" Checked="true" />
                                         <span class="ml-2">Organigrama activo</span>
                                     </div>
                                 </div>
                                 <div class="row w-100 mt-2 mb-4">
                                     <div class="col-12">
                                         <asp:CheckBox ID="CheckBox_inactiva_organigrama" runat="server" Text="" Checked="false" />
                                         <span class="ml-2">Organigrama inactivo </span>
                                     </div>
                                 </div>
                                 <asp:MutuallyExclusiveCheckBoxExtender ID="MutuallyExclusiveCheckBoxExtender3" runat="server" TargetControlID="Check_activa_organigrama"
                                     Key="radicados_"></asp:MutuallyExclusiveCheckBoxExtender>
                                 <asp:MutuallyExclusiveCheckBoxExtender ID="Mutuallyexclusivecheckboxextender4" runat="server" TargetControlID="CheckBox_inactiva_organigrama"
                                     Key="radicados_"></asp:MutuallyExclusiveCheckBoxExtender>
                                 <div id="div_cambia_estado_organigrama">
                             </ContentTemplate>
                         </asp:UpdatePanel>
                     </div>
                     <asp:UpdatePanel ID="UpdatePane_cambia_estado_organigrama" runat="server" UpdateMode="Conditional">
                         <ContentTemplate>
                             <div class="modal-footer justify-content-end">
                                 <asp:Button ID="Button_cambia_estado_organigrama" runat="server" Text="Aceptar" CssClass="btn btn-success" />
                                 <asp:Button ID="Button_cancelar_cambio_estado_organigrama" runat="server" Text="Cancelar" CssClass="btn  btn-light" />
                             </div>
                         </ContentTemplate>
                     </asp:UpdatePanel>
                 </div>
                 <div style="display: none; height: 1px">
                     <asp:Button ID="Button1__cambia_estado_organigrama" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                     <asp:Button ID="ButtonSalir_cambia_estado_organigrama" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                     <asp:Button ID="Buttoncerrarimpre_cambia_estado_organigrama" runat="Server" Text="" Height="0px" Width="0px" />
                 </div>

             </asp:Panel>
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
        <!--guarda archivo-->
         <asp:UpdatePanel ID="updatapanel_iframe" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <input id="Hidden_ruta_archivo" type="hidden" value="" runat="server">
                <iframe runat="server" id="ifmExcel_" width="0" height="0" marginheight="0" marginwidth="0" 
                    frameborder="0"  />
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
    <script accesskey="javascript" type="text/javascript">
       
    </script>
</body>
</html>