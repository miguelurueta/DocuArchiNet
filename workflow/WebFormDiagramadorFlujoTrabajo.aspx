<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormDiagramadorFlujoTrabajo.aspx.vb" enableEventValidation="false" Inherits="GestionDocumental_Docuarchi.net.WebFormDiagramadorFlujoTrabajo" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register Assembly="MindFusion.Diagramming.WebForms" Namespace="MindFusion.Diagramming.WebForms"
    TagPrefix="ndiag" %>
<%@ Register Assembly="MindFusion.Extenders" Namespace="MindFusion.Diagramming.WebForms"
    TagPrefix="ndiag" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">

<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
     <title>Diagramador de rutas</title>
     <script src="../js/ui/jquery-3.4.1.min.js"></script>  
     <link href="../ccs/themes/base/jquery.ui.all.css" rel="stylesheet" type="text/css" />
    <link href="../Styles/styleMenu.css" rel="stylesheet" type="text/css" />     
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
      <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <link href="../Styles/samples.css" rel="stylesheet" />
    <script src="../js/workflow/WebFormDiagramadorFlujoTrabajo.js"></script>
    <link href="../Styles/samples.css" rel="stylesheet" />
     <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <script src="../js/java_general/general_code_java.js" type="text/javascript"></script>
    <script src="../js/java_general/ubicacion_code_java.js" type="text/javascript"></script>   
   <script src="../js/java_general/general_control_java.js"></script>
    <script  src="../Awesome/js/all.js"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet"/>
  <link href="../Awesome/css/brands.css" rel="stylesheet"/>
  <link href="../Awesome/css/solid.css" rel="stylesheet"/>
    <script  src="../Awesome/js/brands.js"></script>
  <script  src="../Awesome/js/solid.js"></script>
  <script  src="../Awesome/js/fontawesome.js"></script>
    <script src="../js/validate_campos.js"></script>
    <script src="../js/java_general/general_code_java.js"></script>
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
                
                //$("#Menu1").show();
                    if (elment_postbak.id == "Button_me_active_men_dive") {
                    //document.getElementById("Label_edita_escript_evento").innerHTML = document.getElementById("HiddenField_result_edit_script").value;
                    //auto_size_popup_edit_escript();
                }
                if (elment_postbak.type == "button" || elment_postbak.type == "submit") {
                    elment_postbak.value = value_element;
                    elment_postbak.disabled = false;
                }
                
                if (elment_postbak.id == "ImageButtonGuardar") {
                    document.getElementById("ImageButtonGuardar").src = "../workflow/imageneswf/Guardar_actividad_inactive.png";
                }
                //ImageButtonCrearGrupoActividadUsuario
                if (elment_postbak.id == "ImageButtonCrearGrupoActividadUsuario") {
                    document.getElementById("Label_lista_actividades_worflow").innerHTML = "Agregar actividad";
                    auto_zise_popup_lista_tareas("1");
                }
                if (elment_postbak.id == "ImageButton_Crear_Actividad_usuario") {
                    document.getElementById("Label_lista_actividades_worflow").innerHTML = "Agregar actividad de usuario";
                    auto_zise_popup_lista_tareas("1");
                }
               
                }

                catch (err) {
                    alert(" Funcion CheckStatus asincrona WebFormDiagramadorFlujoTrabajo.aspx" + err.message);
                }
                finally {
                    progres_hiden('progres_bar');
                }
            }

            </script>
    <div id="form_parent_content">
        <nav id="menucab" class="navbar navbar-expand-sm nav_botota_person_gray modal_content_no_back_inferior">
            <button id="nav_togle_display" class="navbar-toggler" type="button" style="background-color: #6d7fcc" data-toggle="collapse" data-target="#navbarNavDropdown">
                <span class="navbar-toggler-icon_"><i style="color: white" class="fad fa-th-list"></i></span>
            </button>
            <div class="collapse navbar-collapse row" id="navbarNavDropdown">
                <ul class="navbar-nav col-md-12" >
                     <li class="nav-item dropdown active ml-2 mr-0 active_">
                        <a class="nav-link  dropdown-toggle " style="color: #6d7fcc" href="#" id="A5" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc; display: none" class="fad fa-project-diagram"></i> Flujos de trabajo 
                        </a>
                        <ul class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">
                            <li><a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Agregar nuevo flujo de trabajo a la ruta" onclick="activa_menu_general_diference(event,this,'N-F-TF')"><i class="fal fa-plus"></i> Agregar nuevo flujo de trabajo</a></li> 
                            <li><a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Relación trámites y flujos de trabajo" onclick="activa_menu_general_diference(event,this,'G-T-FW')"><i class="fal fa-wrench"></i> Relacion trámites y flujos de trabajo</a></li>  
                            <li><a  style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Exportar diagrama del flujo de trabajo seleccionado" onclick="activa_menu_general_diference(event,this,'F-EXP-FW')"><i class="fal fa-file-download"></i> Exportar diagrama del flujo de trabajo seleccionado</a> </li> 
                            <li> <a  style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Crear un nuevo flujo de trabajo a partir del flujo seleccionado" onclick="activa_menu_general_diference(event,this,'C-FTA-FW')"><i class="fal fa-clone"></i> Duplicar el flujo de trabajo seleccionado</a></li> 
                            <li> <a  style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Editar datos de caracterización flujo de trabajo seleccionado" onclick="activa_menu_general_diference(event,this,'E-DC-FT')"><i class="fal fa-edit"></i> Editar caracterización del flujo de trabajo seleccionado</a></li> 
                             <li> <a  style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Eliminar flujo de trabajo seleccionado" onclick="activa_boton_client_server('ImageButton_eliminar_flujo_trabajo');"><i class="fal fa-times"></i> Eliminar flujo de trabajo seleccionado</a></li> 
                            <li> <a  style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Usuario responsable flujo de trabajo seleccionado" onclick="activa_menu_general_diference(event,this,'E-RS-FT')"><i class="fal fa-user-plus"></i> Establece responsable flujo de trabajo seleccionado</a> </li> 
                            <li> <a  style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Cambiar eatado abierto o cerrado flujo de trabajo" onclick="activa_menu_general_diference(event,this,'C-F-TS')"><i class="fal fa-exchange"></i> Cambiar estado flujo de trabajo seleccionado</a>                      </li> 
                        </ul>
                    </li> 
                    <li class="nav-item dropdown active ml-2 mr-0 active_">
                        <a class="nav-link  dropdown-toggle " style="color: #6d7fcc" href="#" id="A1" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc; display: none" class="fad fa-project-diagram"></i> Configuración de actividades 
                        </a>
                        <ul class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">
                             <li><a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Configura actividad seleccionada como abierta o cerrada" onclick="activa_menu_general_diference(event,this,'C-A-SF')"><i class="fal fa-wrench"></i> Configuración actividad seleccionada</a></li>
                             <li><a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Activa actividad seleccionada como inicio de flujo de trabajo" onclick="activa_menu_general_diference(event,this,'C-A-IF')"><i class="fal fa-wrench"></i> Configuración estado de inicio de la actividad selecionada</a></li>
                             <li><a id="a_descriptive_activities" style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Actualiza decripción de actividad" ><i class="fal fa-wrench"></i> Actualiza decripción de actividad</a></li>
                        </ul>
                    </li> 
                     <li class="nav-item dropdown active ml-2 mr-0 active_">
                        <a class="nav-link  dropdown-toggle " style="color: #6d7fcc" href="#" id="A2" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc; display: none" class="fad fa-project-diagram"></i> Configuración de conectores 
                        </a>
                        <ul class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">
                            <li><a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" onclick="activa_menu_general_diference(event,this,'C-CONECTOR-WB')"><i class="fal fa-wrench"></i> Configurar conector</a></li>
                        </ul>
                    </li> 
                </ul>
            </div>
        </nav>
       
         <asp:UpdatePanel ID="UpdatePanel_menu_var_event" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <input id="Hidden_menu_var_event_dive" type="hidden" value="" runat="server"/>
                        <asp:Button ID="Button_me_active_men_dive" runat="server" Text="" Style="display:none; width:1px; height:1px" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            <div id="Menutol" style="height:auto; width: 100%; border: 0.2px none Black; top: 0px; left: 0px">
                <asp:UpdatePanel ID="updatemenu" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                         <nav id="nav_menu" class="navbar navbar-expand-sm nav_botota_person modal_content_no_back_inferior">
                             <button class="navbar-toggler" type="button" style="background-color: #6d7fcc" data-toggle="collapse" data-target="#navbarNavDropdown_">
                                <span class="navbar-toggler-icon_"><i style="color: white" class="fad fa-th-list"></i></span>
                            </button>
                            <div class="collapse navbar-collapse row" id="navbarNavDropdown_">
                                <ul class="navbar-nav">
                                    <li class="nav-item active ml-2 active_">
                                        <a class="nav-link font-weight-light" style="color: #6d7fcc" title="Guardar cambios al diagrama del flujo de trabajo" href="#" onclick="activa_boton_client_server('ImageButtonGuardar');"><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fas fa-save fa-lg"></i>  </a>
                                    </li>
                                </ul>
                                <asp:DropDownList ID="DropDownList_flujos_disponibles_workflow" CssClass="custom-select  w-25"
                                    runat="server"  Width="500px"
                                    Style=" margin-left: 10px;" AutoPostBack="True">
                                </asp:DropDownList>       
                                <ul class="navbar-nav">
                                    <li class="nav-item active ml-2 active_">
                                        <a class="nav-link font-weight-light" style="color: #6d7fcc" title="Nueva actividad de grupo" href="#" onclick="activa_boton_client_server('ImageButtonCrearGrupoActividadUsuario');"><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-users-medical fa-lg"></i>  </a>
                                    </li>
                                </ul>
                                 <ul class="navbar-nav" >
                                    <li class="nav-item active ml-2 active_">
                                        <a class="nav-link font-weight-light" style="color: #6d7fcc" title="Nueva actividad de usuario" href="#" onclick="activa_boton_client_server('ImageButton_Crear_Actividad_usuario');"><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-user-plus fa-lg"></i>  </a>
                                    </li>
                                </ul>
                                 <ul class="navbar-nav" >
                                    <li class="nav-item active ml-2 active_">
                                        <a class="nav-link font-weight-light" style="color: #6d7fcc" title="Elimina elemento seleccionado" href="#" onclick="activa_boton_client_server('ImageButtonEliminarActividades');"><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-times fa-lg"></i>  </a>
                                    </li>
                                </ul>
                                <ul class="navbar-nav">
                                    <li class="nav-item active ml-2 active_">
                                        <a class="nav-link font-weight-light" style="color: #6d7fcc" title="Conectar actividades para flujo de trabajo" href="#" onclick="activa_boton_client_server('ImageButton_conectar_actividades');"><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-long-arrow-alt-right fa-lg"></i>  </a>
                                    </li>
                                </ul>
                                <asp:DropDownList ID="DropDownZonFactor" CssClass="custom-select ml-4 mr-4" 
                                    runat="server"  Width="70px"
                                    Style="margin-left: 3px" AutoPostBack="True">
                                </asp:DropDownList>
                                <asp:CheckBox ID="CheckBox_Grid_alineamiento" runat="server" Text=""  Checked="false" AutoPostBack="true" class="mr-0"/>
                                <span style="color: #6d7fcc" class="ml-2"> Visible grid </span>
                            </div>
                         </nav>
                        <div style="display: none">
                             <asp:ImageButton ID="ImageButtonGuardar" runat="server" ImageUrl="../workflow/imageneswf/Guardar_actividad_inactive.png"
                            ToolTip="Guardar cambios al diagrama del flujo de trabajo"
                            ImageAlign="Left" Width="25px" Height="25px" Style="margin-left: 3px; display: block; margin-top:3px" CssClass="alterna_image" />
                        <asp:Label ID="Label_flujos_workflow" runat="server" Text="Flujos de trabajo" style="float:left; margin-left:10px; margin-top:5px"></asp:Label>  
                        <asp:ImageButton ID="ImageButton_eliminar_flujo_trabajo" runat="server"
                            ToolTip="Elimina flujo de trabajo seleccionado" AlternateText="Elimina flujo de trabajo seleccionado" CssClass="alterna_image"
                            ImageAlign="Bottom" Width="25px" Height="20px" Style="margin-left:3px; margin-bottom:5px;margin-top:5px; float:left" ImageUrl="../workflow/imageneswf/Eliminar_Actividad.png" />                     
                        &nbsp &nbsp  &nbsp  &nbsp 
                        
                            <asp:ImageButton ID="ImageButtonCrearGrupoActividadUsuario" runat="server" ImageUrl="../workflow/imageneswf/Actividad_grupo_usuario.png"
                            ToolTip="Agregar actividad al flujo de trabajo" AlternateText="Agregar actividad de grupo al flujo de trabajo" CssClass="alterna_image"
                            ImageAlign="Left" Width="40px" Height="30px" Style="margin-left:1px" />

                        <asp:ImageButton ID="ImageButton_Crear_Actividad_usuario" runat="server"
                            ToolTip="Agregar usuario al flujo documental" AlternateText="Agregar actividad de usuario al flujo documental" CssClass="alterna_image"
                            ImageAlign="Bottom" Width="30px" Height="30px"  Style="margin-left:1px; margin-bottom:5px; float:left; display:block" ImageUrl="../workflow/imageneswf/actividad_usuario.png" />

                        <asp:ImageButton ID="ImageButtonEliminarActividades" runat="server"
                            ToolTip="Elimina actividad seleccionada" AlternateText="Elimina actividad seleccionada" CssClass="alterna_image"
                            ImageAlign="Bottom" Width="25px" Height="20px" Style="margin-left:3px; margin-bottom:5px;margin-top:5px; float:left" ImageUrl="../workflow/imageneswf/Eliminar_Actividad.png" />
                        
                            <asp:ImageButton ID="ImageButton_conectar_actividades" runat="server"
                            ToolTip="Conectar actividades para flujo de trabajo" AlternateText="Conectar actividades para flujo de trabajo" CssClass="alterna_image"
                            ImageAlign="Bottom" Width="30px" Height="25px" Style="margin-left:1px; margin-bottom:5px; margin-top:1px; float:left"  ImageUrl="../workflow/imageneswf/Conectar_Actividades.png" />
                        <asp:Button ID="Button_visor_emergente" runat="server" Text="Button" Style="display: none" />

                       
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
                          NodeSelectedScript="onNodeSelected"   NodeClickedScript="nodeClicked" LinkSelectedScript="onLinkSelected"  EnableViewState="true" >
                    </ndiag:DiagramView>
                    <asp:HiddenField ID="HiddenField_value_selecion" runat="server"  />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <div id="footer" style="height: 24px;">
            <asp:UpdatePanel ID="UpdatePanel_label_estado" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Label ID="Label_Estado_documento" runat="server" Text="Estado" Style="margin-left: 5px"></asp:Label>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
       <!--paginas_externas_popou-->
          <div id="paginas_externas_popou">
            <asp:Panel ID="Panel_paginas_externas_popou" runat="server" Style="display:none; color: black; width: 100%; height: 100%" CssClass="modal_content_general_">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_paginas_externas_popou" runat="server"  
                     TargetControlID="ButtonSalir_paginas_externas_popou" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_paginas_externas_popou" PopupControlID="Panel_paginas_externas_popou" ></asp:ModalPopupExtender>
                <div id="modal_content_Panel_paginas_externas_popou" class="modal-content">
                    <div id="divcabecer2_paginas_externas_popou" class="modal_title_superior_ modal-header">
                           <h6 class="modal-title d-inline ml-1">Relación de trámites y flujos de trabajo</h6>
                           <button type="button" value="Button_cerrar_paginas_externas_popou" class="close da_event_captive">&times;</button>   
                        
                    </div>
                    <div id="contenido_procesa_paginas_externas_popou" style="background-color: white; width: 100%; height: 99%; color: black; background-color: #FFFFFF" class="modal_content_back">
                        <asp:UpdatePanel ID="UpdatePanel_paginas_externas_popou" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <iframe id="Iframe_paginas_externas_popup_" runat="server" frameborder="0" style="width: 100%; height: 100%; overflow: hidden" scrolling="no"></iframe>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
                <div style="display:none; height:1px">
                     <asp:Button ID="Button_paginas_externas_popou" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="ButtonSalir_paginas_externas_popou" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                     <asp:Button ID="Button_cerrar_paginas_externas_popou" runat="Server" Text="" CssClass="modal_boton_hiden"/>
                </div>
                
            </asp:Panel>
        </div>
        <!--nuevo flujo trabajo-->
          <div id="nuevo_flujo_trabajo">
            <asp:Panel ID="Panel_nuevo_flujo_trabajo" runat="server" Style="display:none; color:black; width: 60%; height: auto" CssClass="modal_content_general_">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_nuevo_flujo_trabajo" runat="server" 
                     TargetControlID="ButtonSalir_nuevo_flujo_trabajo" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_nuevo_flujo_trabajo" PopupControlID="Panel_nuevo_flujo_trabajo" ></asp:ModalPopupExtender>
                <div id="modal_content_Panel_nuevo_flujo_trabajo" class="modal-content">  
                <div id="divcabecer2_nuevo_flujo_trabajo"  class="modal_title_superior_ modal-header"> 
                    <h6 class="modal-title d-inline ml-1">Nuevo flujo de trabajo</h6>
                    <button type="button" value="Button_cerrar_nuevo_flujo_trabajo" class="close da_event_captive">&times;</button>                    
                </div>
                <div id="contenido_procesa_nuevo_flujo_trabajo" style="background-color: white; width: 100%; height: 99%; color: black; background-color: #FFFFFF; border-top:none; overflow:auto" class="modal_content_back modal-body">
                        <div class="row">
                            <div class="col-6 mt-1">
                                <span>Seleccione la ruta general (*)</span>
                            </div>
                            <div class="col-6 mt-1">
                                <asp:UpdatePanel ID="UpdatePanel_combo_rutas_disponibles" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:DropDownList ID="DropDownList_combo_rutas" runat="server" Style="width: 100%" CssClass="custom-select" ></asp:DropDownList>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                            <div class="col-6 mt-1">
                                <span>Nombre del flujo trabajo (*)</span>
                            </div>
                            <div class="col-6 mt-1">
                                <asp:TextBox ID="TextBox_flujo_trabajo" runat="server" Style="width:100%" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="col-6 mt-1">
                                 <span>Descripción del flujo de trabajo (*)</span>
                            </div>
                            <div class="col-6 mt-1">
                                <asp:TextBox ID="TextBox_descripcion_flujo_trabajo" runat="server" Style="width:100%" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="col-6 mt-1"> 
                                <span>Tipo flujo de trabajo (*)</span>
                            </div>
                            <div class="col-6 mt-1">
                                <asp:UpdatePanel ID="UpdatePanel_list_tipo_flujo" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:DropDownList ID="DropDownList_tipo_flujo" runat="server" Style="width: 100%" CssClass="custom-select"></asp:DropDownList>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>        
                        </div>                        
                </div>
                    <div class="modal-footer justify-content-end" id="modal-footer_Panel_nuevo_flujo_trabajo">  
                         <asp:UpdatePanel ID="UpdatePanel_nuevo_flujo_trabajo" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <table style="width: 100%;">
                                    <asp:Button ID="Button_agregar_flujo_trabjo" runat="server" Text="Aceptar"  CssClass="btn btn-success" />
                                </table>          
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
                <div style="display:none; height:0px">
                      <asp:Button ID="Button_nuevo_flujo_trabajo" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                      <asp:Button ID="ButtonSalir_nuevo_flujo_trabajo" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                     <asp:Button ID="Button_cerrar_nuevo_flujo_trabajo" runat="Server" Text="" CssClass="modal_boton_hiden" />
                </div>
                
            </asp:Panel>
        </div>
        <!--copia flujo trabajo-->
          <div id="copia_flujo_trabajo">
            <asp:Panel ID="Panel_copia_flujo_trabajo" runat="server" Style="display:none; color:black; width: 60%; height: auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_copia_flujo_trabajo" runat="server"  TargetControlID="ButtonSalir_copia_flujo_trabajo" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_copia_flujo_trabajo" PopupControlID="Panel_copia_flujo_trabajo" ></asp:ModalPopupExtender>
                <div id="modal_content_Panel_copia_flujo_trabajo" class="modal-content">  
                <div id="divcabecer2_copia_flujo_trabajo"  class="modal_title_superior_ modal-header">
                     <h6 class="modal-title d-inline ml-1">Copia flujo de trabajo</h6>
                     <button type="button" value="Button_cerrar_copia_flujo_trabajo" class="close da_event_captive">&times;</button>   
                   
                </div>
                    <div id="contenido_procesa_copia_flujo_trabajo" style="background-color: white; width: 100%; height: 99%; color: black; background-color: #FFFFFF; border-top: none; overflow: auto" class="modal_content_back modal-body">
                        <div class="row w-100 mt-1">
                            <div class="col-6">
                                <span>Seleccione la ruta general</span>
                            </div>
                            <div class="col-6">
                                <asp:UpdatePanel ID="UpdatePanel_combo_rutas_disponibles_copia" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:DropDownList ID="DropDownList_combo_rutas_copia" runat="server" Style="width: 100%" CssClass="custom-select"></asp:DropDownList>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                        <div class="row w-100 mt-1">
                            <div class="col-6">        
                                <span>Nombre del flujo trabajo (*)</span>
                            </div>
                            <div class="col-6">
                                <asp:TextBox ID="TextBox_flujo_trabajo_copia" runat="server" Style="width: 100%" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row w-100 mt-1">
                            <div class="col-6">
                                <span>Descripción del flujo de trabajo (*)</span>
                            </div>
                            <div class="col-6">
                                 <asp:TextBox ID="TextBox_descripcion_flujo_trabajo_copia" runat="server" Style="width: 100%" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row w-100 mt-1">
                            <div class="col-6">
                                <span>Tipo flujo de trabajo (*)</span>
                            </div>
                            <div class="col-6">
                                <asp:UpdatePanel ID="UpdatePanel_copia_flujo_trabajo" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:DropDownList ID="DropDownList_tipo_flujo_copia" runat="server" Style="width: 100%" CssClass="custom-select"></asp:DropDownList>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>                
                    </div>
                    <div class="modal-footer justify-content-end" >
                        <asp:UpdatePanel ID="UpdatePanel_botton_duplicar" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Button ID="Button_agregar_flujo_trabajo_copia" runat="server" Text="Aceptar"  CssClass="btn btn-success" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div style="display:none; height:0px">
                         <asp:Button ID="Button_copia_flujo_trabajo" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                         <asp:Button ID="ButtonSalir_copia_flujo_trabajo" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                         <asp:Button ID="Button_cerrar_copia_flujo_trabajo" runat="Server" Text="" CssClass="invisible" Height="0px" Width="0px"/>
                    </div>
                 
                </div>
            </asp:Panel>
        </div>
        <!--lista_actividades_worflow-->
            <div style="clear:both"></div>
            <asp:Panel ID="Panel_lista_actividades_worflow" runat="server" Style="display:none; color: black; width: 70%; height:auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_lista_actividades_worflow" runat="server" 
                     TargetControlID="ButtonSalir_lista_actividades_worflow" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_lista_actividades_worflow" PopupControlID="Panel_lista_actividades_worflow"  ></asp:ModalPopupExtender>
                <div id="modal_content_Panel_lista_actividades_worflow" class="modal-content">
                    <div id="divcabecer2_lista_actividades_worflow" class="modal_title_superior_ modal-header">
                        <asp:Label ID="Label_lista_actividades_worflow" runat="server" class="modal-title d-inline ml-1 h6"></asp:Label>
                        <button type="button" value="Button_cerrar_lista_actividades_worflow" class="close da_event_captive">&times;</button>
                    </div>
                    <div id="contenido_procesa_lista_actividades_workflow" style="background-color: white; width: 100%; height: 99%; color: black; background-color: #FFFFFF; border-top: none" class="modal_content_back modal-body">
                        <asp:UpdatePanel ID="UpdateGeneral_documentos" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div id="contenido_titulo_data_grid_dos_title" class="row w-100">
                                    <div class="col-5">
                                        <asp:Label ID="titulo_label_grid" runat="server" ForeColor="Black" CssClass="h6">Resultados busqueda</asp:Label>
                                    </div>
                                    <div class="col-7">
                                        <div class="input-group ">
                                            <button id="td-boton" class="btn btn-outline-secondary border-right-2 " style="border-top-right-radius: 0px; border-bottom-right-radius: 0px" title="Restaurar lista" onclick="preven_event_restor_search(event,this)" type="button">
                                                <i class="fal fa-undo-alt"></i>
                                            </button>
                                            <asp:TextBox ID="TextBox_busqueda" runat="server" class="form-control form-control-sm complex " placeholder="Busqueda...."></asp:TextBox>
                                            <div class="input-group-append">
                                                <button class="btn btn-outline-secondary" onclick="preven_event_search(event,this)" title="consultar lista" type="button">
                                                    <i class="fal fa-search"></i>
                                                </button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <input id="hdnEmailID" type="hidden" value="0" runat="server">
                                <input id="HiddenEstado" type="hidden" value="1" runat="server">
                                <asp:Panel ID="panel_conten_gred" runat="server" Wrap="False"
                                    Width="100%" Style="overflow: auto" CssClass="mt-1">
                                    <asp:GridView ID="data_grid" runat="server" Style="position: inherit"
                                        AutoGenerateSelectButton="False" CssClass="filtrar table" GridLines="None" Font-Size="12px">
                                        <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                        <HeaderStyle CssClass="GridviewScrollHeader_line_boot" />
                                        <Columns>
                                            <asp:BoundField HeaderText="OPCIONES" />
                                        </Columns>
                                    </asp:GridView>
                                </asp:Panel>
                            </ContentTemplate>
                            <Triggers>
                            </Triggers>
                        </asp:UpdatePanel>
                    </div>
                </div>
                <div style="display: none; height: 1px">
                    <asp:Button ID="Button_lista_actividades_worflow" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="ButtonSalir_lista_actividades_worflow" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="Button_cerrar_lista_actividades_worflow" runat="Server" Text="X" CssClass="" Height="0px" Width="0px"/>
                </div>
                 
                <div id="div_contenido_procesa_lista_actividades_worflow_botones_desicion" style="display:none">
                        <asp:UpdatePanel ID="UpdatePanel_contendor_botones_desicion" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Button ID="Button_agrega_actividad_flujo_trabajo" runat="server" Text="Agregar actividad" CssClass="boton_azul" Style="margin-left: 5px; display:none" />
                                <asp:Button ID="Button_buscar_lista" runat="server" Width="65px" Height="19px" Text="Buscar" style="float:right; font-size:11px"  />     
                                <asp:Button ID="Button_restore_lista_actividad" runat="server" Width="65px" Height="19px" Text="Buscar" style="float:right; font-size:11px"  />   
                            
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
            </asp:Panel>
       
         <!--confirma_eliminar_elmento_diagrama-->
          <div id="confirma_eliminar_elmento_diagrama">
            <asp:Panel ID="Panel_confirma_eliminar_elmento_diagrama" runat="server" Style="display:none; color: black; width:40%; height: auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_confirma_eliminar_elmento_diagrama" runat="server"  TargetControlID="ButtonSalir_confirma_eliminar_elmento_diagrama" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_confirma_eliminar_elmento_diagrama" PopupControlID="Panel_confirma_eliminar_elmento_diagrama" ></asp:ModalPopupExtender>
                <div id="modal_content_Panel_confirma_eliminar_elmento_diagrama" class="modal-content">  
                <div id="divcabecer2_confirma_eliminar_elmento_diagrama"  class="cabecera2_ modal-header">               
                    <h6 class="modal-title d-inline ml-1">Eliminar elemento</h6>
                    <button type="button" value="Button_cerrar_confirma_eliminar_elmento_diagrama" class="close da_event_captive">&times;</button>   
                </div>
                <div id="contenido_procesa_confirma_eliminar_elmento_diagrama" style="background-color: white; width: 100%; height: 99%; color: black; background-color: #FFFFFF;" class="modal-body">  
                        <div class="row w-100 mt-3 mb-3">
                            <div class="col-12">
                                <span>Desea eliminar el elemento seleccionado ?</span>
                            </div>
                        </div>                               
                </div>
                <div class="modal-footer justify-content-end" >  
                        <asp:UpdatePanel ID="UpdatePanel_confirma_eliminar_elmento_diagrama" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <input id="Hidden_estado_eliminar" type="hidden" value="" runat="server">
                                <asp:Button ID="Button_aceptar_confirmacion_eliminar_elmento_diagrama" runat="server" Text="Aceptar" CssClass="btn btn-success" />
                                <asp:Button ID="Button_cancelar_confirmacion_eliminar_elmento_diagrama" runat="server" Text="Cancelar" CssClass="btn btn-light" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                </div>
                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button_confirma_eliminar_elmento_diagrama" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="ButtonSalir_confirma_eliminar_elmento_diagrama" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="Button_cerrar_confirma_eliminar_elmento_diagrama" runat="Server" Text="" CssClass="invisible" Height="0px" Width="0px" />
                    </div>
                 
                </div>
            </asp:Panel>
        </div>
        <!--configura_tipo_actividad_flujo-->
          <div id="configura_tipo_actividad_flujo">
            <asp:Panel ID="Panel_configura_tipo_actividad_flujo" runat="server" Style="display:none; color: black; width:40%; height:auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_configura_tipo_actividad_flujo" runat="server"  TargetControlID="ButtonSalir_configura_tipo_actividad_flujo" 
                     BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_configura_tipo_actividad_flujo" PopupControlID="Panel_configura_tipo_actividad_flujo" ></asp:ModalPopupExtender>
                <div id="modal_content_Panel_configura_tipo_actividad_flujo" class="modal-content">
                    <div id="divcabecer2_configura_tipo_actividad_flujo" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title d-inline ml-1">Configurar actividad</h6>
                        <button type="button" value="Button_cerrar_configura_tipo_actividad_flujo" class="close da_event_captive">&times;</button>
                    </div>
                    <div id="contenido_procesa_configura_tipo_actividad_flujo" style="background-color: white; width: 100%; height: 100%" class="modal_content_back modal-body">
                        <asp:UpdatePanel ID="UpdatePanel_configura_tipo_actividad_flujo" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row-12 mt-2 w-100">
                                    <div class="col-12">
                                        <asp:CheckBox ID="Check_flujo_abierto_actividad" runat="server" Text="" Checked="false" />
                                        <span class="ml-2">Activa  la  actividad  para  flujo  de  trabajo abierto</span>
                                    </div>
                                    <div class="col-12">
                                        <asp:CheckBox ID="CheckBox_flujo_cerrado_actividad" runat="server" Text=" " Checked="true" />
                                        <span class="ml-2">Activa la actividad para flujo de trabajo cerrado</span>
                                    </div>
                                </div>

                                <asp:MutuallyExclusiveCheckBoxExtender ID="MutuallyExclusive_anexo_radicado" runat="server" TargetControlID="Check_flujo_abierto_actividad"
                                    Key="radicado"></asp:MutuallyExclusiveCheckBoxExtender>
                                <asp:MutuallyExclusiveCheckBoxExtender ID="MutuallyExclusive_relacionado_radicado" runat="server" TargetControlID="CheckBox_flujo_cerrado_actividad"
                                    Key="radicado"></asp:MutuallyExclusiveCheckBoxExtender>


                            </ContentTemplate>
                        </asp:UpdatePanel>

                    </div>
                    <div class="modal-footer justify-content-end">
                        <asp:UpdatePanel ID="UpdatePanel_cambia_estado_actividad" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Button ID="Button_cambia_estado_cerrado_abierto_actividad" runat="server" Text="Aceptar" CssClass="btn btn-success" />
                                <asp:Button ID="Button_Cancela_estado_cerrado_abierto_actividad" runat="server" Text="Cancelar" CssClass="btn btn-light" />
                            </ContentTemplate>
                        </asp:UpdatePanel>

                    </div>
                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button_configura_tipo_actividad_flujo" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="ButtonSalir_configura_tipo_actividad_flujo" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="Button_cerrar_configura_tipo_actividad_flujo" runat="Server" Text="" CssClass="modal_boton_hiden" Height="0px" Width="0px" />
                    </div>

                </div>
            </asp:Panel>
        </div>
        <!--configura_tipo_flujo_trabajo-->
          <div id="configura_tipo_flujo_trabajo">
            <asp:Panel ID="Panel_configura_tipo_flujo_trabajo" runat="server" Style="display:none; color:black; width:40%; height: auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_configura_tipo_flujo_trabajo" runat="server"  TargetControlID="ButtonSalir_configura_tipo_flujo_trabajo" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_configura_tipo_flujo_trabajo" PopupControlID="Panel_configura_tipo_flujo_trabajo" ></asp:ModalPopupExtender>
                <div id="modal_content_Panel_configura_tipo_flujo_trabajo" class="modal-content">  
                <div id="divcabecer2_configura_tipo_flujo_trabajo"  class="modal_title_superior_ modal-header">  
                    <h6 class="modal-title d-inline ml-1">Configura flujo de trabajo</h6>
                    <button type="button" value="Button_cerrar_configura_tipo_flujo_trabajo" class="close da_event_captive">&times;</button>                
                </div>
                <div id="contenido_procesa_configura_tipo_flujo_trabajo" style="background-color: white; width: 100%; height: 100%; color: black; background-color: #FFFFFF" class="modal_content_back modal-body">                  
                        <asp:UpdatePanel ID="UpdatePanel_configura_tipo_flujo_trabajo" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                               <div class="row-12 w-100 mt-2">
                                   <div class="col-12">
                                        <asp:CheckBox ID="Check_flujo_abierto" runat="server" Text="" Checked="false"   />
                                       <span class="ml-2">Activa flujo de trabajo como abierto</span>
				                             
                                   </div>
                                   <div class="col-12">
                                        <asp:CheckBox ID="CheckBox_flujo_cerrado" runat="server" Text="" Checked="true"  />
                                       <span class="ml-2">Activa flujo de trabajo como cerrado</span>
                                   </div>
                               </div>      
                                    <asp:MutuallyExclusiveCheckBoxExtender ID="MutuallyExclusiveCheckBoxExtender1" runat="server" TargetControlID="Check_flujo_abierto"
                                        Key="radicado_"></asp:MutuallyExclusiveCheckBoxExtender>
				                            <asp:mutuallyexclusivecheckboxextender id="Mutuallyexclusivecheckboxextender2" runat="server" targetcontrolid="CheckBox_flujo_cerrado"
				                                key="radicado_"></asp:mutuallyexclusivecheckboxextender>       
                            </ContentTemplate>
                        </asp:UpdatePanel>
                         
                </div>
                     <div class="modal-footer justify-content-end">  
                         <asp:UpdatePanel ID="UpdatePanel_estado_cerrado_abierto" runat="server" UpdateMode="Conditional">
                             <ContentTemplate>
                                  <asp:Button ID="Button_cambia_estado_cerrado_abierto_flujo" runat="server" Text="Aceptar" CssClass="btn btn-success"  /> 
                                  <asp:Button ID="Button_Cancela_estado_cerrado_abierto_flujo" runat="server" Text="Cancelar" CssClass="btn  btn-light"  />
                             </ContentTemplate>
                         </asp:UpdatePanel>
                     </div>
                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button_configura_tipo_flujo_trabajo" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="ButtonSalir_configura_tipo_flujo_trabajo" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="Button_cerrar_configura_tipo_flujo_trabajo" runat="Server" Text="" CssClass="modal_boton_hiden" />
                    </div>
                
                </div>
            </asp:Panel>
        </div>
        <!--confirma_eliminar_flujo_trabajo-->
          <div id="confirma_eliminar_flujo_trabajo">
            <asp:Panel ID="Panel_confirma_eliminar_flujo_trabajo" runat="server" Style="display:none; color: White; width:300px; height: 130px">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_confirma_eliminar_flujo_trabajo" runat="server" BehaviorID="Panel_confirma_eliminar_flujo_trabajo" TargetControlID="ButtonSalir_confirma_eliminar_flujo_trabajo" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_confirma_eliminar_flujo_trabajo" PopupControlID="Panel_confirma_eliminar_flujo_trabajo" ></asp:ModalPopupExtender>
                <div id="divcabecer2_confirma_eliminar_flujo_trabajo"  class="cabecera2">               
                    <asp:Label ID="Label_confirma_eliminar_flujo_trabajo" runat="server" Text="Mensaje" Font-Size="10" Style="float: left; font-family:Arial; margin-left:10px">
                    </asp:Label>
                    <div id="Divcerrarbuton2_confirma_eliminar_flujo_trabajo" style="float: right">
                        <asp:Button ID="Button_cerrar_confirma_eliminar_flujo_trabajo" runat="Server" Text="X"
                            ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_confirma_eliminar_flujo_trabajo" style="background-color: white; width: 100%; height: 99%;border: thin double #000080; color: black; background-color: #FFFFFF;">                  
                        <asp:UpdatePanel ID="UpdatePanel_confirma_eliminar_flujo_trabajo" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <input id="Hidden_estado_eliminar_flujo" type="hidden" value="" runat="server">
                                <div style="text-align: center">
                                    <br />
                                    <asp:Label ID="Label_title_comfirma_eliminar_flujo" runat="server" Text="Desea eliminar el flujo de trabajo ?" style="font-family:Arial; font-size:14px"></asp:Label>
                                    <br />
                                    <br />
                                    <asp:Button ID="Button_aceptar_confirmacion_eliminar_flujo_trabajo" runat="server" Text="Aceptar" CssClass="boton_azul" /> &nbsp
                                    <asp:Button ID="Button_cancelar_confirmacion_eliminar_flujo_trabajo" runat="server" Text="Cancelar" CssClass="boton_azul" />
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                         
                </div>
                 <asp:Button ID="Button_confirma_eliminar_flujo_trabajo" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_confirma_eliminar_flujo_trabajo" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
            </asp:Panel>
        </div>
        <!--confirma_eliminar_flujo-->
          <div id="confirma_eliminar_flujo">
            <asp:Panel ID="Panel_confirma_eliminar_flujo" runat="server" Style="display:none; color: White; width:320px; height: 130px">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_confirma_eliminar_flujo" runat="server" BehaviorID="Panel_confirma_eliminar_flujo" TargetControlID="ButtonSalir_confirma_eliminar_flujo" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_confirma_eliminar_flujo" PopupControlID="Panel_confirma_eliminar_flujo" ></asp:ModalPopupExtender>
                <div id="divcabecer2_confirma_eliminar_flujo"  class="cabecera2">               
                    <asp:Label ID="Label_confirma_eliminar_flujo" runat="server" Text="Mensaje" Font-Size="10" Style="float: left; font-family:Arial; margin-left:10px">
                    </asp:Label>
                    <div id="Divcerrarbuton2_confirma_eliminar_flujo" style="float: right">
                        <asp:Button ID="Button_cerrar_confirma_eliminar_flujo" runat="Server" Text="X"
                            ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_confirma_eliminar_flujo" style="background-color: white; width: 100%; height: 99%;border: thin double #000080; color: black; background-color: #FFFFFF;">                  
                        <asp:UpdatePanel ID="UpdatePanel_confirma_eliminar_flujo" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <input id="Hidden1" type="hidden" value="" runat="server">
                                <div style="text-align: center">
                                    <br />
                                    <asp:Label ID="Label2" runat="server" Text="Desea eliminar el flujo de trabajo seleccionado ?" style="font-family:Arial; font-size:14px"></asp:Label>
                                    <br />
                                    <br />
                                    <asp:Button ID="Button_aceptar_confirmacion" runat="server" Text="Aceptar" CssClass="boton_azul" /> &nbsp
                                    <asp:Button ID="Button_cancelar_confirmacion" runat="server" Text="Cancelar" CssClass="boton_azul" />
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                         
                </div>
                 <asp:Button ID="Button_confirma_eliminar_flujo" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_confirma_eliminar_flujo" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
            </asp:Panel>
        </div>
        <!--activa_actividad_inicio-->
          <div id="activa_actividad_inicio">
            <asp:Panel ID="Panel_activa_actividad_inicio" runat="server" Style="display:none; color: black; width:40%; height:auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_activa_actividad_inicio" runat="server" BehaviorID="Panel_activa_actividad_inicio" 
                     TargetControlID="ButtonSalir_activa_actividad_inicio" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_activa_actividad_inicio" PopupControlID="Panel_activa_actividad_inicio" ></asp:ModalPopupExtender>
                <div id="modal_content_Panel_activa_actividad_inicio" class="modal-content">
                    <div id="divcabecer2_activa_actividad_inicio" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title d-inline ml-1">Activa inicio de flujo de trabajo</h6>
                        <button type="button" value="Button_cerrar_activa_actividad_inicio" class="close da_event_captive">&times;</button>              
                    </div>
                    <div id="contenido_procesa_activa_actividad_inicio" style="background-color: white; width:100%; height: 100%; color: black; background-color: #FFFFFF" class="modal_content_back modal-body">
                        <asp:UpdatePanel ID="UpdatePanel_activa_actividad_inicio" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row-12 mt-2">
                                    <asp:CheckBox ID="Check_actividad_inicio" runat="server" Text="" Checked="false"  />
                                    <span class="ml-2">Activa como actividad de inicio del flujo de trabajo</span>
                                </div>          
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="modal-footer justify-content-end" >  
                         <asp:UpdatePanel ID="UpdatePanel_canbia_estado_actividad_ini" runat="server" UpdateMode="Conditional">
                             <ContentTemplate>
                                 <asp:Button ID="Button_cambia_estado_actividad_inicio" runat="server" Text="Aceptar" CssClass="btn btn-success"  />
                                 <asp:Button ID="Button_Cancela_estado_actividad_inicio" runat="server" Text="Cancelar" CssClass="btn btn-light"  />
                             </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button_activa_actividad_inicio" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="ButtonSalir_activa_actividad_inicio" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="Button_cerrar_activa_actividad_inicio" runat="Server" Text="" CssClass="invisible" Height="0px" Width="0px"/>
                    </div>
                </div>
            </asp:Panel>
        </div>
        <!--edita flujo trabajo-->
          <div id="edita_flujo_trabajo">
            <asp:Panel ID="Panel_edita_flujo_trabajo" runat="server" Style="display:none; color:black; width: 60%; height: auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_edita_flujo_trabajo" runat="server"  TargetControlID="ButtonSalir_edita_flujo_trabajo" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_edita_flujo_trabajo" PopupControlID="Panel_edita_flujo_trabajo" ></asp:ModalPopupExtender>
                <div id="modal_content_Panel_edita_flujo_trabajo" class="modal-content">  
                <div id="divcabecer2_edita_flujo_trabajo"  class="modal_title_superior_ modal-header">  
                     <h6 class="modal-title d-inline ml-1">Editar caracterización del flujo de trabajo</h6>
                     <button type="button" value="Button_cerrar_edita_flujo_trabajo" class="close da_event_captive">&times;</button>                   
                </div>
                <div id="contenido_procesa_edita_flujo_trabajo" style="background-color: white; width: 100%; height: 99%; color: black; background-color: #FFFFFF" class="modal_content_back modal-body">                 
                        <asp:UpdatePanel ID="UpdatePanel_edita_flujo_trabajo" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row w-100 mt-1">
                                    <div class="col-6">                         
                                        <span>Nombre del flujo  trabajo (*)</span>
                                    </div>
                                    <div class="col-6">
                                        <asp:TextBox ID="TextBox_Edita_nombre_flujo_trabajo" runat="server" Style="width:100%" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row w-100 mt-1">
                                    <div class="col-6">
                                        
                                        <span>Descripción del flujo de trabajo (*)</span>
                                    </div>
                                    <div class="col-6">
                                        <asp:TextBox ID="TextBox_Edita_descripcion_flujo_trabajo" runat="server" Style="width:100%" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                               
                            </ContentTemplate>
                        </asp:UpdatePanel>
                           
                </div>
                    <div class="modal-footer justify-content-end" id="modal-footer">  
                        <asp:UpdatePanel ID="UpdatePanel_boton_edita_flujo" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Button ID="Button_editar_flujo_trabjo" runat="server" Text="Aceptar" style="margin-right:10px" CssClass="btn btn-success" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div style="display:none; height:0px">
                        <asp:Button ID="Button_edita_flujo_trabajo" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="ButtonSalir_edita_flujo_trabajo" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="Button_cerrar_edita_flujo_trabajo" runat="Server" Text="" CssClass="invisible" Height="0px" Width="0px"/>
                    </div>
                 
                </div>
            </asp:Panel>
        </div>
        
        <!--edita flujo trabajo-->
          <div id="usuario_respon_flujo">
            <asp:Panel ID="Panel_usuario_respon_flujo" runat="server" Style="display:none; color:black; width:50%; height: auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_usuario_respon_flujo" runat="server"  TargetControlID="ButtonSalir_usuario_respon_flujo" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_usuario_respon_flujo" PopupControlID="Panel_usuario_respon_flujo" ></asp:ModalPopupExtender>
                <div id="modal_content_Panel_usuario_respon_flujo" class="modal-content">
                    <div id="divcabecer2_usuario_respon_flujo" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title d-inline ml-1">Usuario responsable flujo</h6>
                        <button type="button" value="Button_cerrar_usuario_respon_flujo" class="close da_event_captive">&times;</button>        
                    </div>
                    <div id="contenido_procesa_usuario_respon_flujo" style="background-color: white; width: 100%; height: 99%; color: black; background-color: #FFFFFF" class="modal_content_back modal-body">
                       
                        <asp:UpdatePanel ID="UpdatePanel_usuario_respon_flujo" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row w-100 mt-2">
                                    <div class="col-8">
                                        <asp:DropDownList ID="DropDownList_user_respon_flujo" runat="server" Style="width: 100%" CssClass="custom-select"></asp:DropDownList>
                                    </div>
                                    <div class="col-4">
                                        <a class="btn  btn-success" style="margin-left: 10px; margin: 5px; width: 50px" title="Registrar usuario responsable " href="#" onclick="activa_boton_client_server('Button_regi_resp')"><i class="fal fa-user-plus"></i></a>
                                            <asp:Button ID="Button_regi_resp" runat="server" Text="Button" Style="display: none" />
                                            <a class="btn  btn-success" style="margin-left: 10px; margin: 5px; width: 50px" title="Eliminar usuario responsable" href="#" onclick="activa_boton_client_server('Button_elimi_resp')"><i class="fal fa-user-times"></i></a>
                                            <asp:Button ID="Button_elimi_resp" runat="server" Text="Button" Style="display: none" OnClientClick="ConfirmMensajeEliminar_user_resp();" />
                                            <input id="Hidden_res" type="hidden" value="0" runat="server" />
                                    </div>
                                </div>
                               
                            </ContentTemplate>
                        </asp:UpdatePanel>

                    </div>
                    <div style="display: none; height: 0px">
                        <asp:Button ID="Button_usuario_respon_flujo" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="ButtonSalir_usuario_respon_flujo" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="Button_cerrar_usuario_respon_flujo" runat="Server" Text="" CssClass="invisible" Height="0px" Width="0px" />
                    </div>
                    
                </div>
            </asp:Panel>
        </div>
        <!--edita flujo trabajo-->
          <div id="registra_respon_flujo">
            <asp:Panel ID="Panel_registra_respon_flujo" runat="server" Style="display:none; color: black; width:50%; height: auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_registra_respon_flujo" runat="server"  TargetControlID="ButtonSalir_registra_respon_flujo" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_registra_respon_flujo" PopupControlID="Panel_registra_respon_flujo" ></asp:ModalPopupExtender>
                <div id="modal_content_Panel_registra_respon_flujo" class="modal-content"> 
                <div id="divcabecer2_registra_respon_flujo"  class="modal_title_superior_ modal-header">   
                    <h6 class="modal-title d-inline ml-1">Registra usuario responsable flujo</h6>
                    <button type="button" value="Button_cerrar_registra_respon_flujo" class="close da_event_captive">&times;</button>              
                   
                </div>
                <div id="contenido_procesa_registra_respon_flujo" style="background-color: white; width: 100%; height: 99%; color: black; background-color: #FFFFFF" class="modal_content_back modal-body">
                                                   
                        <asp:UpdatePanel ID="UpdatePanel_registra_respon_flujo" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row mt-1 w-100">
                                    <div class="col-6">              
                                        <span>Area/dependencia usuario</span>
                                    </div>
                                    <div class="col-6">
                                        <asp:DropDownList ID="DropDownList_grupo_respon_flujo" runat="server" AutoPostBack="true" style="width:100%" CssClass="custom-select"></asp:DropDownList> 
                                    </div>
                                </div>
                                 <div class="row mt-1 w-100">
                                    <div class="col-6">
                                        
                                        <span>Usuario responsable</span>
                                    </div>
                                    <div class="col-6">
                                        <asp:DropDownList ID="DropDownList_user_resp" runat="server" style="width:100%" CssClass="custom-select"></asp:DropDownList>
                                    </div>
                                </div>
                               
                              
                            </ContentTemplate>
                        </asp:UpdatePanel>
                         
                </div>
                    <div class="modal-footer justify-content-end" >  
                        <asp:UpdatePanel ID="UpdatePanel_activa_registra" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Button ID="Button_activa_registra" runat="server" Text="Aceptar"  CssClass="btn btn-success"  />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button_registra_respon_flujo" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="ButtonSalir_registra_respon_flujo" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="Button_cerrar_registra_respon_flujo" runat="Server" Text="" CssClass="modal_boton_hiden" Height="0px" Width="0px"/>
                    </div>
                 </div>
            </asp:Panel>
        </div>
         <!--configura_envi_correo_conector-->
        <div id="configura_envi_correo_conector" style="">
            <asp:Panel ID="Panel_configura_envi_correo_conector" runat="server" Style="display: none; color: black; width: 50%; height: auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_configura_envi_correo_conector" runat="server"
                    TargetControlID="ButtonSalir_configura_envi_correo_conector" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_configura_envi_correo_conector" PopupControlID="Panel_configura_envi_correo_conector">
                </asp:ModalPopupExtender>
                <div id="modal_content_Panel_configura_envi_correo_conector" class="modal-content">
                    <div id="divcabecer2_configura_envi_correo_conector" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title d-inline ml-1">Configurar conector</h6>
                        <button type="button" value="Button_cerrar_configura_envi_correo_conector" class="close da_event_captive">&times;</button>
                    </div>
                    <div id="contenido_procesa_configura_envi_correo_conector" style="background-color: white; width: 100%; height: 99%; color: black; background-color: #FFFFFF;  border-top:none" class="modal_content_back modal-body">

                        <asp:UpdatePanel ID="UpdatePanel_configura_envi_correo_conector" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row w-100 mt-2">
                                    <div class="col-12">
                                        <asp:CheckBox ID="CheckBox_estado_correo_conector" runat="server" Text="" />
                                        <span class="ml-2">Envía correo elecrónico al enviar la tarea</span>
                                    </div>
                                    
                                </div>
                                <div class="row w-100 mt-1">
                                    <div class="col-12">
                                        <asp:CheckBox ID="CheckBox_autoriza_tarea" runat="server" Text="" />
                                        <span class="ml-2">El usuario debe autorizar la tarea antes de enviarla</span>
                                    </div>
                                    
                                </div>
                                <div class="row w-100 mt-1">
                                    <div class="col-12">
                                        <asp:CheckBox ID="CheckBox_autoriza_tarea_firma_digital" runat="server" Text="" />
                                        <span class="ml-2">El usuario debe  firmar digtalmente la autorización</span>
                                    </div>
                                     
                                </div>
                                <div class="row w-100 mt-1">
                                    <div class="col-12">
                                        <asp:CheckBox ID="CheckBox_estado_copia_estructura" runat="server" Text="" />
                                        <span class="ml-2">El usuario debe  copiar algunos documentos a expediente</span>
                                    </div>
                                     
                                </div>
                                <div class="row w-100 mt-1">
                                    <div class="col-12">
                                        <asp:CheckBox ID="CheckBox_estado_copia_estructura_total" runat="server" Text="" />
                                        <span class="ml-2">El usuario debe  copiar todos los documentos a expediente</span>
                                    </div>
                                     
                                </div>
                                <div class="row w-100 mt-1">
                                    <div class="col-12">
                                        <asp:CheckBox ID="CheckBox_Estado_asigna_expediente" runat="server" Text="" />
                                        <span class="ml-2">El usuario debe asignar expediente a los documentos</span>
                                    </div>
                                     
                                </div>
                               <div class="row w-100 mt-1">
                                    <div class="col-12">
                                        <asp:CheckBox ID="CheckBox_estado_firma_digital" runat="server" Text="" />
                                        <span class="ml-2">El usuario debe firmar digitalmente todos los documentos</span>
                                    </div>
                                     
                                </div>
                                <div class="row w-100 mt-1">
                                    <div class="col-12">
                                        <asp:CheckBox ID="CheckBox_estado_valida_balanceo" runat="server" Text="" />
                                        <span class="ml-2">El sistema evalua cargas de trabajo para la asignación (Solo para actividades de grupo)</span>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>                 
                    </div>
                    <div class="modal-footer justify-content-end">
                        <asp:UpdatePanel ID="UpdatePanel_buton_configura_envi_correo_conector" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Button ID="Button_config_correo_conector" runat="server" Text="Aceptar" CssClass="btn btn-success" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div style="display:none; height:1px">
                    <asp:Button ID="Button_configura_envi_correo_conector" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="ButtonSalir_configura_envi_correo_conector" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="Button_cerrar_configura_envi_correo_conector" runat="Server" Text="" CssClass="invisible" Height="0px" Width="0px"/>
                    </div>
                </div>
            </asp:Panel>
        </div>
        <!---Edita descripion actividad-->
       <asp:Panel ID="Panel_adm_flujo_update_activity_description" runat="server" Style="display: none; width: 50%; height: auto" CssClass="modal_content_general">
            <asp:ModalPopupExtender ID="ModalPopupExtender_edition_adm_flujo_update_activity_description" runat="server" TargetControlID="ButtonSalir_adm_flujo_update_activity_description" BackgroundCssClass="FondoAplicacion"
                CancelControlID="Button_cerrar_adm_flujo_update_activity_description" PopupControlID="Panel_adm_flujo_update_activity_description">
            </asp:ModalPopupExtender>
            <div class="modal-content_" id="modal_content_adm_flujo_update_activity_description">
                <div id="title_adm_flujo_update_activity_description" class="modal_title_superior_ modal-header">
                    <h6 id="id_modal_title" class="modal-title"></h6>
                    <button type="button" value="Button_cerrar_adm_flujo_update_activity_description" class="close da_event_captive">&times;</button>
                </div>
                <div id="contenido_procesa_adm_flujo_update_activity_description" style="width: auto; height: auto; border-top: none" class="modal_content_back modal-body">
                    <div id="div_adm_flujo_update_activity_description"  style="width:95%; overflow:auto" >       
                         
                    </div>
                </div>
            </div>
            <div class="modal-footer align-content-end" id="modal_foter_adm_flujo_update_activity_description">
                <button type="button" id="boton_event_adm_flujo_update_activity_description" title=""  class="btn btn-success   mt-1"> Aceptar</button>
                <button type="button" title="" value="Button_cerrar_adm_flujo_update_activity_description" class="btn btn-light da_event_captive  mt-1"> Cancelar </button>  
            </div>
            <div style="display: none; height: 1px">
                <asp:Button ID="Button_cerrar_adm_flujo_update_activity_description" runat="Server" Text="X" CssClass="invisible" />
                <asp:Button ID="Button_adm_flujo_update_activity_description" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                <asp:Button ID="ButtonSalir_adm_flujo_update_activity_description" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
            </div>
        </asp:Panel>
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
                <input id="Hidden_ruta_archivo" type="hidden" value="" runat="server"/>
                <iframe runat="server" id="ifmExcel_" width="0" height="0" marginheight="0" marginwidth="0" 
                    frameborder="0"  />
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
    <script accesskey="javascript" type="text/javascript">
       // $("#Menu1").show();
    </script>
</body>
</html>
