<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormGaadmonclasificacion.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormGaadmonclasificacion" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
     <title>Administración clasificación</title>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
      <script src="../js/ui/jquery-3.4.1.min.js"></script>  
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
    <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>  
     <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
    <link href="../ccs/themes/base/jquery.ui.all.css" rel="stylesheet" type="text/css" />
      <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
   <script defer src="../Awesome/js/all.js"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet">
     <link href="../Awesome/css/brands.css" rel="stylesheet">
     <link href="../Awesome/css/solid.css" rel="stylesheet">
     <script defer src="../Awesome/js/brands.js"></script>
     <script defer src="../Awesome/js/solid.js"></script>
     <script defer src="../Awesome/js/fontawesome.js"></script>  
    <script src="../js/gestion/WebFormGaadmonclasificacion.js"></script>
    <script src="../js/MyJavaScriptFile.js"></script> 
     <script src="../js/java_general/general_code_java.js"></script>
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
                progres_hiden('progres_bar');
                //$("#Menu1").show();
                if (elment_postbak.type == "button" || elment_postbak.type == "submit") {
                    elment_postbak.value = value_element;
                    elment_postbak.disabled = false;
                }
                
               


            }

            </script>
    <div id="contenedor_general"  >
        <nav id="menu_var" class="navbar navbar-expand-sm nav_botota_person_gray modal_content_no_back_inferior">
             <button id="nav_togle_display" class="navbar-toggler" type="button" style="background-color: #6d7fcc" data-toggle="collapse" data-target="#navbarNavDropdown">
                <span class="navbar-toggler-icon_"><i style="color: white" class="fad fa-th-list"></i></span>
            </button>
            <div class="collapse navbar-collapse row" id="navbarNavDropdown">
                 <ul class="navbar-nav col-md-12" >
                     <li class="nav-item dropdown active ml-2 mr-0 active_">
                         <a class="nav-link  dropdown-toggle " style="color: #6d7fcc" href="#" id="A5" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc; display: none" class="fad fa-project-diagram"></i>Menú 
                         </a>
                         <ul class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">
                             <li><a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" onclick="activa_menu_general_diference_local(event,this,'A-CC')"><i class="fas fa-layer-plus"></i> Agregar cuadro de clasficación</a></li>
                             <li><a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" onclick="activa_menu_general_diference_local(event,this,'A-NC')"><i class="fas fa-layer-plus"></i> Agregar nivel cuadro de clasficación</a></li>
                             <li><a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" onclick="activa_menu_general_diference_local(event,this,'E-NCCC')"><i class="fal fa-edit"></i> Editar elemento</a></li>
                             <li><a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" onclick="activa_menu_general_diference_local(event,this,'D-NCCC')"><i class="fal fa-trash-alt"></i> Eliminar elemento</a></li>
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
            <asp:UpdatePanel ID="UpdatePanel_estructura_clasificacion" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                <ContentTemplate>
                    <div id="Estru_clasificacion" class="row_ w-100">
                        <div class=" ">
                            <asp:DropDownList ID="DropDownList_nivel_clasficacion" runat="server" Style="width: 100%" CssClass="custom-select" onchange="activa_boton_lista_cuadro();"></asp:DropDownList>
                        </div>
                        <asp:Label ID="Label_estructura" runat="server" Text="Estructura de clasificación documental" Style="text-align: center; font-family: Arial; font-size: 16px; margin-left: 5px; font-weight: 200; display: none"></asp:Label>
                        <asp:Button ID="Button_activa_agregar_cuadro_clasificacion" runat="server" Text="Agregar" ToolTip="Agregar la estructura de clasificación documental o cuadro de clasificación" Style="background-color: white; border-color: #b0c4de; height: 25px; font-size: 12px; font-family: Arial; display: none" CssClass="boton" />
                        <asp:Button ID="Button_activa_editar_cuadro_clasificacion" runat="server" Text="Editar" ToolTip="Edita la descripción de la estructura de clasificación documental o cuadro de clasificación" Style="background-color: white; border-color: #b0c4de; height: 25px; font-size: 12px; font-family: Arial; display: none" CssClass="boton" />
                        <asp:Button ID="Button_eliminar_cuadro_clasificacion" runat="server" Text="Eliminar" ToolTip="Elimina la descripción de la estructura de clasificación documental o cuadro de clasificación" Style="background-color: white; border-color: #b0c4de; height: 25px; font-size: 12px; font-family: Arial; display: none" CssClass="boton" OnClientClick="ConfirmMensajeGeneral('Desea eliminar la estructura de clasificación?','Hidden_result');" />
                        <div style="display: none">
                            <asp:Button ID="Button_lista_cuadro_clasficacion_treview" runat="server" Text="Eliminar" Style="background-color: white; border-color: #b0c4de; height: 25px; font-size: 12px; font-family: Arial" CssClass="boton" />
                        </div>
                        <input id="Hidden_tipo_trasac" type="hidden" value="0" runat="server">
                        <input id="Hidden_id_cuadro" type="hidden" value="0" runat="server">
                        <input id="Hidden_result" type="hidden" value="0" runat="server">
                    </div>
                </ContentTemplate>

            </asp:UpdatePanel>
        
        <div id="tre_claficacion" style="margin-top:0px; overflow:auto" class="modal_content_back_inferior_superior">   
                 <asp:UpdatePanel ID="UpdatePanelViewArchivo" runat="server" UpdateMode="Conditional"  RenderMode="Inline">
                     <ContentTemplate>
                         <asp:Panel ID="Paneltreview" runat="server" ScrollBars="Both"
                             Height="100%" Width="100%" Style="margin-top: 0px">
                             <asp:TreeView ID="TreeViewEstructura"  Style="text-align: left; padding-left: 5px; font-size: 12px; margin-top: 2px; font-family:'Segoe UI'" runat="server" CssClass="TreeN pl-0 pt-1" NodeWrap="true"
                                 PopulateNodesFromClient="False" EnableViewState="true"
                                 LeafNodeStyle-CssClass="LeafNodeStyle_2_  mb-1 pl-1" Font-Size="12px" NodeIndent="5" ExpandDepth="0" SkipLinkText="">
                                 <HoverNodeStyle Font-Underline="False" />
                                 <LeafNodeStyle CssClass="LeafNodeStyle" HorizontalPadding="10px" NodeSpacing="0px" VerticalPadding="5px" />
                                 <NodeStyle ChildNodesPadding="5px" HorizontalPadding="0px" NodeSpacing="5px" VerticalPadding="5px" CssClass=" mt-2 mb-2 pl-2 font-weight-light" ForeColor="#0062cc" />
                                 <ParentNodeStyle ChildNodesPadding="0px" CssClass="ParentNodeStyle   font-weight-bold "  HorizontalPadding="0px" NodeSpacing="0px" VerticalPadding="0px" />
                                 <RootNodeStyle ChildNodesPadding="0px" CssClass="ParentNodeStyle   font-weight-bold "  NodeSpacing="0px" VerticalPadding="5px" HorizontalPadding="5px" />
                                 <SelectedNodeStyle CssClass="select_treview_boottra font-weight-normal nav-link-treview" ImageUrl="../workflow/imageneswf/iten_list_select.png" />
                             </asp:TreeView>
                         </asp:Panel>
                     </ContentTemplate>
                 </asp:UpdatePanel>     
        </div>
        <div id="Div_opciones_multinivel" style="overflow:auto; text-align:center">
            <asp:UpdatePanel ID="UpdatePanel_opciones_nivel_clasificacion" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                <ContentTemplate>
                    <asp:Label ID="Label_estado" runat="server" Text="" Style=" float:right; font-family: Arial; font-size: 9px; margin-left: 5px; font-weight:200"></asp:Label>
                    <asp:Button ID="Button_activa_agregar_nivel" runat="server" Text="Agregar nivel" ToolTip="Agregar nivel de clasificación" Style="background-color: white; border-color: #b0c4de; height: 25px; font-size: 12px; font-family: Arial; display:none" CssClass="boton" />
                    &nbsp 
                    <asp:Button ID="Button_elimina_nivel" runat="server" Text="Eliminar nivel" ToolTip="Elimina el nivel de clasificación" Style="background-color: white; border-color: #b0c4de; height: 25px; font-size: 12px; font-family: Arial; display:none" CssClass="boton" OnClientClick="ConfirmMensajeGeneral('Desea eliminar el nivel de clasificación?','Hidden_result_eliminar');" />
                    &nbsp 
                    <asp:Button ID="Button_activa_edita_descripcion" runat="server" Text="Editar nivel" ToolTip="Edita la descripción del nivel de clasifición" Style="background-color: white; border-color: #b0c4de; height: 25px; font-size: 12px; font-family: Arial; display:none" CssClass="boton" />
                    <input id="Hidden_result_eliminar" type="hidden" value="0" runat="server">
                     
                </ContentTemplate>

            </asp:UpdatePanel>
        </div>
    </div>
         <!--Popup crear cuadro clasificacion-->
            <asp:Panel ID="Panel_crear_cuadro_clasificacion" runat="server"    Style=" width:50%; height:auto; display:none" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_crear_cuadro_clasificacion" runat="Server" BackgroundCssClass="FondoAplicacion" 
                     TargetControlID="ButtonSalir_crear_cuadro_clasificacion"
                    PopupControlID="Panel_crear_cuadro_clasificacion" CancelControlID="ButtonCerrar_crear_cuadro_clasificacion">
                </asp:ModalPopupExtender>
                <div id="modal_content_Panel_crear_cuadro_clasificacion" class="modal-content"> 
                <div id="divcabecer2_radica_documento" class="modal_title_superior_ modal-header">              
                    <asp:Label ID="Label_reversa_respuesta" class="modal-title d-inline ml-1 h6" runat="server" Text=""  >
                    </asp:Label>
                    <button type="button" value="ButtonCerrar_crear_cuadro_clasificacion" class="close da_event_captive">&times;</button>   
                </div>            
                    <div id="Cotenedor_crear_cuadro_clasificacion" style="background-color: white; width: 99%; height: 99%; margin-left: 1px; border-top:none" class="modal_content_back modal-body">
                        <asp:UpdatePanel ID="UpdatePanel_contenido_estructura_cuadro_editar_crear" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row w-100 mt-2">
                                    <div class="col-12" style="text-align: center">
                                        <asp:Label ID="Label_title_estructura" runat="server" Text="Crear estructura de clasificación documental " CssClass="h6"></asp:Label>
                                    </div>

                                </div>
                                <div class="row w-100 mt-2">
                                    <div class="col-6">
                                        <span>Entidad/empresa  (*)</span>
                                    </div>
                                    <div class="col-6">
                                        <asp:DropDownList ID="DropDownList_entidad_empresa_clasificacion" runat="server" Style="width: 100%" CssClass="custom-select"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row w-100 mt-2">
                                    <div class="col-6">
                                        <span>Estructura organica (*)</span>
                                    </div>
                                    <div class="col-6">
                                        <asp:DropDownList ID="DropDownList_organigrama" runat="server" Style="width: 100%" CssClass="custom-select"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row w-100 mt-2">
                                    <div class="col-6">
                                        <span>Código país (*)</span>
                                    </div>
                                    <div class="col-6">
                                        <asp:DropDownList ID="DropDownList_codigo_estructura" runat="server" Style="width: 100%" CssClass="custom-select"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row w-100 mt-2">
                                    <div class="col-6">
                                        <span>Fechas extremas YYYY MM DD (*)</span>
                                    </div>
                                    <div class="col-6">
                                        <div class="row p-0">
                                            <div class="col-4 p-0">
                                                <asp:TextBox ID="TextBoxFECHA_EXTREMA_INICIAL_CUADRO" runat="server" Width="100%" onkeypress="return validate_fecha(event,this)" placeholder="0000 00 00"></asp:TextBox>
                                                <asp:CalendarExtender ID="TextBoxFECHA_EXTREMA_INICIAL_CUADRO_CalendarExtender" runat="server" BehaviorID="TextBoxFECHA_EXTREMA_INICIAL_CUADRO_CalendarExtender" TargetControlID="TextBoxFECHA_EXTREMA_INICIAL_CUADRO" Format='yyyy-MM-dd' PopupButtonID="ImageButtonfechaextremaini_CUADRO" />

                                            </div>
                                            <div class="col-2 p-0">
                                                <button class="ml-1 btn border-0" id="ImageButtonfechaextremaini_CUADRO" type="button">
                                                    <i class="fad fa-calendar-alt fa-1x"></i>
                                                </button>
                                            </div>
                                            <div class="col-4 p-0">
                                                <asp:TextBox ID="TextBoxFECHA_EXTREMA_FINAL_CUADRO" runat="server" Width="100%" onkeypress="return validate_fecha(event,this)" placeholder="0000 00 00"></asp:TextBox>
                                                <asp:CalendarExtender ID="TextBoxFECHA_EXTREMA_FINAL_CUADRO_CalendarExtender" runat="server" BehaviorID="TextBoxFECHA_EXTREMA_FINAL_CUADRO_CalendarExtender" TargetControlID="TextBoxFECHA_EXTREMA_FINAL_CUADRO" Format='yyyy-MM-dd' PopupButtonID="ImageButtonfechaextremafin_CUADRO" />

                                            </div>
                                            <div class="col-2 p-0">
                                                <button class="ml-1 btn border-0" id="ImageButtonfechaextremafin_CUADRO" type="button">
                                                    <i class="fad fa-calendar-alt fa-1x"></i>
                                                </button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                               

                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="modal-footer justify-content-end" >  
                        <asp:UpdatePanel ID="UpdatePanel_boton_editar_agregar_cuadro_clasficacion" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                            <ContentTemplate>
                                <input id="Hidden_resultado_buscar" runat="server" type="hidden" value="">
                                <asp:Button ID="Button_editar_agregar_cuadro_clasficacion" runat="server" Text="Aceptar" CssClass="btn btn-success" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div style="display:none; height:1px">
                         <asp:Button ID="ButtonSalir_crear_cuadro_clasificacion" runat="server" Text="" Height="0px" Width="0px" Style="display: none" />
                         <asp:Button ID="ButtonCerrar_crear_cuadro_clasificacion" runat="Server" Text="" Height="0px" Width="0px" Style="display: none"/>
                    </div>
                 </div>
            </asp:Panel>
        <!--mensaje_progreso evento-->
        <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 200px">
            <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
            Processing ...
        </div>
       
         <!--Popup agregar nivel de clasificacion-->
        <asp:Panel ID="Panel_agregar_nivel_clasificacion" runat="server"  Style="width: 50%; height: auto; display: none" CssClass="modal_content_general">
            <asp:ModalPopupExtender ID="ModalPopupExtender_agregar_nivel_clasificacion" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_agregar_nivel_clasificacion"
                PopupControlID="Panel_agregar_nivel_clasificacion" CancelControlID="ButtonCerrar_agregar_nivel_clasificacion">
            </asp:ModalPopupExtender>
            <div id="modal_content_Panel_agregar_nivel_clasificacion" class="modal-content">
                <div id="div_title_agregar_nivel" class="modal_title_superior_ modal-header">
                    <h6 class="modal-title d-inline ml-1"></h6>
                    <button type="button" value="ButtonCerrar_agregar_nivel_clasificacion" class="close da_event_captive">&times;</button>
                </div>
                <div id="Cotenedor_agregar_nivel_clasificacion" style="background-color: white; width: 100%; height: 99% ; border-top:none" class="modal_content_back modal-body">
                    <asp:UpdatePanel ID="UpdatePanel_contenido_estructura_nivel_clasificacion_crear" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="row w-100 mt-2">
                                <div class="col-12" style="text-align: center">
                                    <asp:Label ID="Label_title_nivel_clasificacion" runat="server" Text="Agregar nivel de clasificación documental "  CssClass="h6" ></asp:Label>
                                </div>
                            </div>
                            <div class="row w-100 mt-2">
                                <div class="col-6">
                                    <span>Nivel de clasificación</span>
                                </div>
                                <div class="col-6">
                                    <asp:DropDownList ID="DropDownList_nivel_clasificacion" runat="server" Style="width: 100%" AutoPostBack="True" CssClass="custom-select"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="row w-100 mt-2">
                                <div class="col-12">
                                    <asp:TextBox ID="TextBox_ayuda_nivel" runat="server" Style="width: 100%; height: 60px" BackColor="Yellow" ReadOnly="True" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row w-100 mt-2">
                                <div class="col-6">
                                    <span>Titulo nivel de clasificación</span>
                                </div>
                                <div class="col-6">
                                    <asp:TextBox ID="TextBox_titulo_nivel_clasificacion" runat="server" Style="width: 100%" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row w-100 mt-2">
                                   <div class="col-6">
                                       <span>Signatura del nivel de calificación</span>
                                   </div>
                                   <div class="col-6">
                                       <asp:TextBox ID="TextBox_signatura_nivel_clasificacion" runat="server" Style="width: 100%" CssClass="form-control"></asp:TextBox>
                                   </div>
                               </div>

                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div class="modal-footer justify-content-end" >  
                    <asp:UpdatePanel ID="UpdatePanel_boton_agregar_nivel_clasificacion" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                        <ContentTemplate>
                             <asp:Button ID="Button_agregar_nivel_clasificacion" runat="server" Text="Aceptar" CssClass="btn btn-success"  />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div style="display: none; height: 1px">
                    <asp:Button ID="ButtonSalir_agregar_nivel_clasificacion" runat="server" Text="" Height="0px" Width="0px" Style="display: none" />
                    <asp:Button ID="ButtonCerrar_agregar_nivel_clasificacion" runat="Server" Text="" Height="0px" Width="0px" Style="display: none" />
                </div>
            </div>
        </asp:Panel>
        <!--Popup editar nivel de clasificacion-->
        <asp:Panel ID="Panel_editar_nivel_clasificacion" runat="server" ForeColor="White" Style="width: 50%; height: auto; display: none" CssClass="modal_content_general">
            <asp:ModalPopupExtender ID="ModalPopupExtender_editar_nivel_clasificacion" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_editar_nivel_clasificacion"
                PopupControlID="Panel_editar_nivel_clasificacion" CancelControlID="ButtonCerrar_editar_nivel_clasificacion">
            </asp:ModalPopupExtender>
            <div id="modal_content_Panel_editar_nivel_clasificacion" class="modal-content">
                <div id="div_title_editar_nivel" class="modal_title_superior_ modal-header">
                    <h6 class="modal-title d-inline ml-1"></h6>
                    <button type="button" value="ButtonCerrar_editar_nivel_clasificacion" class="close da_event_captive">&times;</button>

                </div>
                <div id="Cotenedor_editar_nivel_clasificacion" style="background-color: white; width: 100%; height: 99%; border-top: none" class="modal_content_back modal-body">

                    <asp:UpdatePanel ID="UpdatePanel_editar_nivel_clasficacion" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="row w-100 mt-2">
                                <div class="col-12" style="text-align: center">
                                    <asp:Label ID="Label_title_nivel_clasificacion_editar" runat="server" Text="Editar nivel de clasificación documental " CssClass="h6"></asp:Label>
                                </div>
                            </div>
                            <div class="row w-100 mt-2">
                                <div class="col-6">
                                    <span>Título del nivel de calificación</span>
                                </div>
                                <div class="col-6">
                                    <asp:TextBox ID="TextBox_titulo_nivel_clasificacion_editar" runat="server" Style="width: 100%" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row w-100 mt-2">
                                <div class="col-6">
                                    <span>Signatura del nivel de calificación</span>
                                </div>
                                <div class="col-6">
                                    <asp:TextBox ID="TextBox_signatura_nivel_clasificacion_editar" runat="server" Style="width: 100%" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div style="float: left; width: 100%">
                                <table style="width: 100%">


                                    <tr>
                                        <td colspan="2"></td>
                                    </tr>
                                </table>
                            </div>

                        </ContentTemplate>
                    </asp:UpdatePanel>

                </div>
                <div class="modal-footer justify-content-end">
                    <asp:UpdatePanel ID="UpdatePanel_boton_editar_nivel_clasificacion" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Button ID="Button_editar_nivel_clasificacion" runat="server" Text="Aceptar" CssClass="btn btn-success" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div style="display: none; height: 1px">
                    <asp:Button ID="ButtonSalir_editar_nivel_clasificacion" runat="server" Text="" Height="0px" Width="0px" Style="display: none" />
                    <asp:Button ID="ButtonCerrar_editar_nivel_clasificacion" runat="Server" Text="" Height="0px" Width="0px" Style="display: none" />
                </div>

            </div>
        </asp:Panel>
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
