<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormListaDiagramaFlujo.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormListaDiagramaFlujo" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register Assembly="MindFusion.Diagramming.WebForms" Namespace="MindFusion.Diagramming.WebForms"
    TagPrefix="ndiag" %>
<%@ Register Assembly="MindFusion.Extenders" Namespace="MindFusion.Diagramming.WebForms"
    TagPrefix="ndiag" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">

<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
     <link href="../ccs/themes/base/jquery.ui.all.css" rel="stylesheet" type="text/css" />
<link href="../Styles/styleMenu.css" rel="stylesheet" type="text/css" /> 
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
 <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
<link href="../Styles/Menu3.css" rel="stylesheet" />
    <title>Diagramador de rutas</title>
    <script src="../js/ui/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
      <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <link href="../Styles/samples.css" rel="stylesheet" />
     <script src="../js/ScrollableGrid.js" type="text/javascript"></script>
     <script src="../js/ScrollableGridViewPlugin_ASP.NetAJAXmin.js" type="text/javascript"></script>
    <script src="../Fixed-Header-Table-master/gridviewScroll.min.js"></script>  
    <script src="../js/workflow/WebFormListaDiagramaFlujo.js"></script>
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
                    alert(" Funcion CheckStatus asincrona WebFormDiagramaEstadoFlujoTrabajo.aspx" + err.message);
                }
                finally {
                    progres_hiden('progres_bar');
                }
            }
            </script>
    <div>
      <div id="menucab" style="height: 20px; width: auto; margin-left:1px; margin-right:1px" class="border_marron">
                <asp:UpdatePanel ID="UpdatePanel_menu_principal" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Label ID="Label_nombre_flujo_trabjo" runat="server" Text="" style="font-family:Arial; text-align:center"></asp:Label>
                    </ContentTemplate>
                </asp:UpdatePanel>

            </div>
        <div id="header" style="height: auto; overflow:auto; margin-left:1px; margin-right:1px" class="border_general">      
           <div id="Menutol" style=" height: auto; width: auto" >
                <asp:UpdatePanel ID="updatemenu" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:ImageButton ID="ImageButtonGuardar" runat="server" ImageUrl="../workflow/imageneswf/Guardar_actividad_inactive.png"
                            ToolTip="Descargar diagrama"
                            ImageAlign="Left" Width="25px" Height="25px" Style="margin-left: 3px; display: block; margin-top:3px" CssClass="alterna_image" />
                        <asp:Label ID="Label_flujos_workflow" runat="server" Text="Tipo diagrama" style="float:left; margin-left:10px; margin-top:5px; display:none"></asp:Label>
                        <asp:DropDownList ID="DropDownList_vistas_disponibles_workflow"
                            runat="server" Height="29px" Width="300px"
                            Style="float: left; margin-left: 10px; display:none"  AutoPostBack="True">
                        </asp:DropDownList> 
                        <asp:Label ID="Label_zon" runat="server" Text="Zoon" style="float:left; margin-left:10px; margin-top:5px"></asp:Label>
                        <asp:DropDownList ID="DropDownZonFactor"
                            runat="server" Height="29px" Width="50px"
                            Style="margin-left: 1px; float:left" AutoPostBack="True">
                        </asp:DropDownList>
                        <asp:CheckBox ID="CheckBox_Grid_alineamiento" runat="server" Text="Visible grid"  Checked="false" AutoPostBack="true"/>
                        <asp:ImageButton ID="ImageButton_detalle" runat="server" ImageUrl="../workflow/imageneswf/detalle.png"
                            ToolTip="Mostrar detalle elemento seleccionado en el diagrama"
                            ImageAlign="Left" Width="25px" Height="25px" Style="margin-left: 3px;  margin-top:3px; float:left; display:none" CssClass="alterna_image" />
                    </ContentTemplate>
                </asp:UpdatePanel>

            </div> 
        </div>
       
      
         <asp:UpdatePanel ID="UpdatePanel_diagran_view" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div id="content" style="top: 60px; bottom: 24px; background-color: #EEEEEE; width:100%">
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
    </div>
         <!--detalle_conector_trazabilidad!-->
         <div id="detalle_conector_trazabilidad">
            <asp:Panel ID="Panel_detalle_conector_trazabilidad" runat="server"  Style="display:none; color: White;  height:auto; width:auto">              
                <asp:ModalPopupExtender ID="ModalPopupExtender_detalle_conector_trazabilidad" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_detalle_conector_trazabilidad"
                    PopupControlID="Panel_detalle_conector_trazabilidad" CancelControlID="Buttoncerrarimpre_detalle_conector_trazabilidad">
                </asp:ModalPopupExtender>
                
                <div id="Divcerrarbuton2_detalle_conector_trazabilidad" style="float: right; margin-top: 5px; margin-right: 5px; margin-left: 5px; background-color: white; font-weight: 700; border: none; border-width: 0px 0px 0px 0px; width: 98%; background-color: #7098DD;">
                    <asp:UpdatePanel ID="UpdatePanel_title_detalle_conector_trazabilidad" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Label ID="Label_title_detalle_conector_trazabilidad" runat="server" Style="color: white; float: left; margin-left: 3px" Text="Detalle conector trazabilidad"></asp:Label>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <asp:Button ID="Buttoncerrarimpre_detalle_conector_trazabilidad" runat="Server" Text="X"
                        ForeColor="#000066" Height="21px" ToolTip="Cerrar ventana" Style="float: right" />

                </div>   
                               
                              
                <asp:UpdatePanel ID="UpdatePane_detalle_conector_trazabilidad" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div id="Contenido_detalle_conector_trazabilidad" style="border-radius: 5px 5px 5px 5px; color: black; border: 1px solid #ccc; background-color: #FFFFFF; height:auto; width: 100%">                 
                            <asp:UpdatePanel ID="UpdatePanel_detalle_conector_trazabilidad" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>                 
                                <table style="width: 100%;">
                                    <tr>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                     <tr>
                                        <td> <asp:Label ID="Label_fecha_inicio" runat="server" Text="Fecha de asignación de la tarea" style="margin-left:5px; font-family:arial"></asp:Label></td>
                                        <td> <asp:TextBox ID="TextBox_fecha_inicio" runat="server" Enabled="false" style="width:250px"></asp:TextBox> </td>                          
                                    </tr>
                                    <tr>
                                        <td> <asp:Label ID="Fecha_asignacion_tarea" runat="server" Text="Fecha inicio de la tarea " style="margin-left:5px; font-family:arial"></asp:Label></td>                             
                                        <td> <asp:TextBox ID="TextBox_Fecha_Seleccion" runat="server" Enabled="false" style="width:250px" ></asp:TextBox></td>           
                                    </tr>
                                    <tr>
                                        <td><asp:Label ID="Label_fecha_final" runat="server" Text="Fecha terminación de la tarea " style="margin-left:5px; font-family:arial"></asp:Label> </td>
                                        <td> <asp:TextBox ID="TextBox_Fecha_Fin" runat="server" Enabled="false" style="width:250px"></asp:TextBox></td>
                                    </tr>
                                   
                                    <tr>
                                        <td><asp:Label ID="Label_tiempo_seleccion_ta" runat="server" Text="Tiempo de espera para seleccionar la tarea " style="margin-left:5px; font-family:arial"></asp:Label> </td>
                                        <td> <asp:TextBox ID="TextBox_Duracion_Inicio_Seleccion" runat="server" Enabled="false" style="width:300px"></asp:TextBox> </td>
                                    </tr>
                                    <tr>
                                        <td><asp:Label ID="Label_tiempo_trabajo" runat="server" Text="Tiempo estimado para terminar la tarea " style="margin-left:5px; font-family:arial"></asp:Label> </td>
                                        <td> <asp:TextBox ID="TextBox_Duracion_Seleccion_Fin" runat="server" Enabled="false" style="width:300px"></asp:TextBox></td>
                                    </tr>
                                    <tr>
                                        <td><asp:Label ID="Label_usuario_asignado" runat="server" Text="Usuario asignado " style="margin-left:5px; font-family:arial"></asp:Label> </td>
                                        <td> <asp:TextBox ID="TextBox_usuario_asignado" runat="server" Enabled="false" style="width:300px"></asp:TextBox></td>
                                    </tr>
                                    <tr>
                                        <td><asp:Label ID="Label_cargo_usuario_asignado" runat="server" Text="Cargo usuario asignado " style="margin-left:5px; font-family:arial"></asp:Label> </td>
                                        <td> <asp:TextBox ID="TextBox_cargo_usuario_asignado" runat="server" Enabled="false" style="width:300px"></asp:TextBox></td>
                                    </tr>
                                    <tr>
                                        <td><asp:Label ID="Label_id_Estado" runat="server" Text="Indentificador del estado " style="margin-left:5px; font-family:arial"></asp:Label> </td>
                                        <td> <asp:TextBox ID="TextBox_id_Estado" runat="server" Enabled="false" ></asp:TextBox></td>
                                    </tr>
                                </table> 
                                                     
                            </ContentTemplate>
                        </asp:UpdatePanel>
                            <div id="Div5" style="float:right; text-align:right; margin-top:10px; display:none" >
                                <asp:Button ID="Button_detalle_conector_trazabilidad" runat="server" Text="Aceptar" Style="margin-right:10px"  CssClass="boton_azul"/>                             
                            </div>                         
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                 <asp:Button ID="Button1__detalle_conector_trazabilidad" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_detalle_conector_trazabilidad" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
            </asp:Panel>
        </div>
         <!--guarda archivo-->
        <asp:UpdatePanel ID="updatapanel_iframe" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <input id="Hidden_ruta_archivo" type="hidden" value="" runat="server" />
                <iframe runat="server" id="ifmExcel_" width="0" height="0" marginheight="0" marginwidth="0"
                    frameborder="0" />
            </ContentTemplate>
        </asp:UpdatePanel>
       
        <!--mensaje_progreso evento-->
        <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 200px">
            <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
            Processing ...
        </div>
        <!--mensaje_personalizado-->
	            <asp:Panel ID="Panel_mensaje_personalizado" runat="server" Style="display:none; color: White; width: 400px; height: 150px">
                    <asp:ModalPopupExtender ID="ModalPopupExtender_mensaje_personalizado" runat="server"
                        TargetControlID="Button_mensaje_personalizado" BackgroundCssClass="FondoAplicacion"
                        CancelControlID="Button_cerrar_mensaje_personalizado" PopupControlID="Panel_mensaje_personalizado">
                    </asp:ModalPopupExtender>
                    <div id="div_persoanlizado" class="cabecera2">
                        <asp:Label ID="Label_mensaje_personalizado_" runat="server" Text="Mensaje de servidor" Font-Size="10" Style="float: left; font-family: Arial; margin-left: 5px; margin-top: 2px">
                        </asp:Label>
                        <div id="Divcerrarbuton2_mensaje_personalizado" style="float: right">
                            <asp:Button ID="Button_cerrar_mensaje_personalizado" runat="Server" Text="X"
                                ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />
                        </div>
                    </div>
                    <div id="contenido_procesa_mensaje_personalizado" style="background-color: white; width: 100%; height: 99%; border: thin double #000080; color: black; background-color: #FFFFFF">
                        <br />
                        <div style="height: 60%; float: left; width: 50px">
                            <asp:Label ID="Label_estil" runat="server" Text="&#9888;" Style="font-family: Arial; font-size: 40px; color: black; margin-top: 60px; margin-left: 10px"></asp:Label>
                        </div>
                        <div style="height: 60%; overflow: auto; float: right; width: 330px; margin-right: 10px; text-align: center">
                            <br />
                            <asp:Label ID="Label_mensaje_personalizado" runat="server" Text="Detalle" Style="font-family: Arial; font-size: 11px; color: black; padding-top: 30px; padding-left: 1px; padding-right: 10px; margin-right: 30px; font-weight: 500"></asp:Label>
                        </div>
                        <asp:Button ID="Button_mensaje_personalizado" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                        <asp:Button ID="ButtonSalir_mensaje_personalizado" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    </div>
	            </asp:Panel>
        <!--Termina mensaje_personalizado-->

    </form>
    <script accesskey="javascript" type="text/javascript">
        $("#Menu1").show();
    </script>
</body>
</html>
