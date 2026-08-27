<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormDiagramaEstadoFlujoTrabajo.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormDiagramaEstadoFlujoTrabajo" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register Assembly="MindFusion.Diagramming.WebForms" Namespace="MindFusion.Diagramming.WebForms"
    TagPrefix="ndiag" %>
<%@ Register Assembly="MindFusion.Extenders" Namespace="MindFusion.Diagramming.WebForms"
    TagPrefix="ndiag" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">

<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>

    <title>Diagramador de flujos</title>
    <script src="../js/ui/jquery-3.4.1.min.js"></script>  
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
     <link href="../ccs/themes/base/jquery.ui.all.css" rel="stylesheet" type="text/css" />
     <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
     <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />  
      <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <link href="../Styles/samples.css" rel="stylesheet" />
    <script src="../js/workflow/WebFormDiagramaEstadoFlujoTrabajo.js"></script>
    <script src="../js/java_general/general_code_java.js?v=20260827-compatible-events4"></script>
    <script src="../js/validate_campos.js"></script>
     <script  src="../Awesome/js/all.js"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet"/>
  <link href="../Awesome/css/brands.css" rel="stylesheet"/>
  <link href="../Awesome/css/solid.css" rel="stylesheet"/>
    <script  src="../Awesome/js/brands.js"></script>
  <script  src="../Awesome/js/solid.js"></script>
  <script  src="../Awesome/js/fontawesome.js"></script>
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
                    alert(" Funcion CheckStatus asincrona WebFormDiagramaEstadoFlujoTrabajo.aspx" + err.message);
                }
                finally {
                    progres_hiden('progres_bar');
                }
            }
            </script>
    <div>
     <div id="menucab" class="modal_content_no_back_inferior_" style="height:auto; width: 100%; border: 0.2px none Black; top: 0px; left: 0px">
            <asp:UpdatePanel ID="UpdatePanel_menu_principal" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <nav id="nav1" class="navbar navbar-expand-sm nav_botota_person modal_content_no_back_inferior">
                        <div class="collapse navbar-collapse row" id="Div1">
                            <ul class="navbar-nav  pl-2 pr-2">
                                <asp:Label ID="Label_nombre_flujo_trabjo" runat="server" Text="" Style="color: #6d7fcc; " CssClass="h6" ></asp:Label>
                            </ul>
                        </div>
                    </nav>           
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
       <div id="header" class="modal_content_no_back_inferior_ border_general_" style="background-color:white" >        
                <asp:UpdatePanel ID="updatemenu" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <nav id="nav_menu" class="navbar navbar-expand-sm nav_botota_person modal_content_no_back_inferior_">
                            <button class="navbar-toggler" type="button" style="background-color: #6d7fcc" data-toggle="collapse" data-target="#navbarNavDropdown_">
                                <span class="navbar-toggler-icon_"><i style="color: white" class="fad fa-th-list"></i></span>
                            </button>
                            <div class="collapse navbar-collapse row" id="navbarNavDropdown_">
                                <ul class="navbar-nav">
                                    <li class="nav-item active ml-2 active_">
                                        <a class="nav-link font-weight-light" style="color: #6d7fcc" title="Guardar cambios de la ruta" href="#" onclick="activa_boton_client_server('ImageButtonGuardar');"><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fas fa-save fa-lg"></i>  </a>
                                    </li>
                                </ul>
                                 <asp:DropDownList ID="DropDownList_vistas_disponibles_workflow"
                                    runat="server" Style="margin-left: 10px; color: #6d7fcc" CssClass="custom-select  w-50"
                                    AutoPostBack="True"> 
                                </asp:DropDownList>
                                <ul class="navbar-nav">
                                    <li class="nav-item active ml-2 active_">
                                        <asp:CheckBox ID="CheckBox_Grid_alineamiento" runat="server" Text="" Checked="false" AutoPostBack="true" />
                                        <span style="color: #6d7fcc">Visible grid </span>
                                    </li>
                                </ul>
                                <asp:DropDownList ID="DropDownZonFactor" CssClass="custom-select ml-4 mr-4" 
                                    runat="server"  Width="70px"
                                    Style="margin-left: 3px; float: left; color: #6d7fcc" AutoPostBack="True">
                                </asp:DropDownList>
                                <ul class="navbar-nav">
                                    <li class="nav-item active ml-2 active_">
                                        <a class="nav-link font-weight-light" style="color: #6d7fcc" title="Mostrar detalle elemento seleccionado en el diagrama" href="#" onclick="activa_boton_client_server('ImageButton_detalle');"><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-ballot fa-lg"></i>  </a>
                                    </li>
                                </ul>
                            </div>
                        </nav>
                        <div class="row"  style="display:none">      
                             <asp:ImageButton ID="ImageButtonGuardar" runat="server" ImageUrl="../workflow/imageneswf/Guardar_actividad_inactive.png"
                                    ToolTip="Descargar diagrama"
                                    ImageAlign="Left" Width="0px" Height="0px" CssClass="alterna_image" />
                                 <asp:ImageButton ID="ImageButton_detalle" runat="server" ImageUrl="../workflow/imageneswf/detalle.png"
                                    ToolTip="Mostrar detalle elemento seleccionado en el diagrama"
                                    ImageAlign="Left" Width="0px" Height="0px" Style="margin-left: 3px; display: none;" CssClass="alterna_image" />    
                        </div>               
                    </ContentTemplate>
                </asp:UpdatePanel>

            
        </div>   
       
      
         <asp:UpdatePanel ID="UpdatePanel_diagran_view" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div id="content" style=" width:100%">
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
        <div id="footer" class="pd-2">
            <asp:Label ID="Label_Estado_documento" runat="server" Text="Estado" Style="color: #6d7fcc" CssClass="h6"></asp:Label>    
        </div>
    </div>
       <!--detalle_conector_trazabilidad!-->
         <div id="detalle_conector_trazabilidad">
            <asp:Panel ID="Panel_detalle_conector_trazabilidad" runat="server"  Style="display:none;  height:auto; width:auto" CssClass="modal_content_general">              
                <asp:ModalPopupExtender ID="ModalPopupExtender_detalle_conector_trazabilidad" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_detalle_conector_trazabilidad"
                    PopupControlID="Panel_detalle_conector_trazabilidad" CancelControlID="Buttoncerrarimpre_detalle_conector_trazabilidad">
                </asp:ModalPopupExtender>    
                <div id="Divcerrarbuton2_detalle_conector_trazabilidad"  class="modal-header">     
                      <h6 id="Label_title_detalle_conector_trazabilidad" class="modal-title">Detalle envío tarea</h6>
                     <button type="button" value="Buttoncerrarimpre_detalle_conector_trazabilidad" class="close da_event_captive">&times;</button>                
                </div>     
                <div id="Contenido_detalle_conector_trazabilidad" style=" height: auto; width: auto; overflow:auto" class="modal-body">
                    <asp:UpdatePanel ID="UpdatePane_detalle_conector_trazabilidad" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:UpdatePanel ID="UpdatePanel_detalle_conector_trazabilidad" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div class="row  mt-1">
                                        <div class="col-6">
                                            <span>Fecha de asignación de la tarea</span>
                                        </div>
                                        <div class="col-6">
                                            <asp:TextBox ID="TextBox_fecha_inicio" runat="server" Enabled="false" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row mt-1">
                                        <div class="col-6">
                                            <span>Fecha inicio de la tarea</span>
                                        </div>
                                        <div class="col-6">
                                            <asp:TextBox ID="TextBox_Fecha_Seleccion" runat="server" Enabled="false" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row mt-1">
                                        <div class="col-6">
                                            <span>Fecha terminación de la tarea</span>
                                        </div>
                                        <div class="col-6">
                                            <asp:TextBox ID="TextBox_Fecha_Fin" runat="server" Enabled="false" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row mt-1">
                                        <div class="col-6">
                                            <span>Tiempo de espera para seleccionar la tarea</span>
                                        </div>
                                        <div class="col-6">
                                            <asp:TextBox ID="TextBox_Duracion_Inicio_Seleccion" runat="server"  style="width:100%" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row mt-1">
                                        <div class="col-6">
                                            <span>Tiempo estimado para terminar la tarea</span>
                                        </div>
                                        <div class="col-6">
                                            <asp:TextBox ID="TextBox_Duracion_Seleccion_Fin" runat="server"  style="width:100%" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row mt-1">
                                        <div class="col-6">
                                            <span>Usuario asignado</span>
                                        </div>
                                        <div class="col-6">
                                            <asp:TextBox ID="TextBox_usuario_asignado" runat="server" Enabled="false" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row mt-1">
                                        <div class="col-6">
                                            <span>Cargo usuario asignado</span>
                                        </div>
                                        <div class="col-6">
                                            <asp:TextBox ID="TextBox_cargo_usuario_asignado" runat="server" Enabled="false" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row mt-1">
                                        <div class="col-6">
                                            <span>Indentificador del estado</span>
                                        </div>
                                        <div class="col-6">
                                            <asp:TextBox ID="TextBox_id_Estado" runat="server" Enabled="false"></asp:TextBox>
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <div id="Div5" style="float: right; text-align: right; margin-top: 10px; display: none">
                                <asp:Button ID="Button_detalle_conector_trazabilidad" runat="server" Text="Aceptar" Style="margin-right: 10px" CssClass="boton_azul" />
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div style="display:none; height:1px">
                    <asp:Button ID="Button1__detalle_conector_trazabilidad" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                 <asp:Button ID="ButtonSalir_detalle_conector_trazabilidad" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                 <asp:Button ID="Buttoncerrarimpre_detalle_conector_trazabilidad" runat="Server" Text=""  Height="0px" Width="0px"/>
                </div>
                 
            </asp:Panel>
        </div>
         <!--guarda archivo-->
         <asp:UpdatePanel ID="updatapanel_iframe" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <input id="Hidden_ruta_archivo" type="hidden" value="" runat="server"/>
                <iframe runat="server" id="ifmExcel_" width="0" height="0" marginheight="0" marginwidth="0" 
                    frameborder="0"  />
            </ContentTemplate>
        </asp:UpdatePanel>
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
