<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormTrazabilidadWorkflow.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormTrazabilidadWorkflow" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    
    <title> Consulta trazabilidad</title>
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
     <script src="../js/ui/jquery-3.4.1.min.js"></script>
    <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
     <script src="../js/ScrollableGridPlugin.js"></script>   
    <script src="../js/ScrollableGridViewPlugin_ASP.NetAJAXmin.js" type="text/javascript"></script>
    <script src="../Fixed-Header-Table-master/gridviewScroll.min.js"></script>
   <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
   <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <script src="../js/workflow/WebFormTrazabilidadWorkflow.js"></script>
      <script defer src="../Awesome/js/all.js"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet">
     <link href="../Awesome/css/brands.css" rel="stylesheet">
     <link href="../Awesome/css/solid.css" rel="stylesheet">
     <script defer src="../Awesome/js/brands.js"></script>
     <script defer src="../Awesome/js/solid.js"></script>
     <script defer src="../Awesome/js/fontawesome.js"></script>
    <script  accesskey="javascript" type="text/javascript">
      
    </script>
</head>
<body  style="overflow:hidden">
    <form id="form1" runat="server">
         <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" EnablePageMethods="true">
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
                 try {
                     elment_postbak = args.get_postBackElement();
                     posicion_update_pogres('progres_bar');
                 }
                 catch (err) {
                     alert(err.message + " Funcion InitializeRequest");
                 }
             }
             function CheckStatus(sender, args) {
                 try {
                  
                 
                     progres_hiden('progres_bar');
                  
                 }
                 catch (err) {
                     alert(err.message + " Funcion CheckStatus");
                 }
             }

            </script>
    <div>
        <input id="Hidden_estado_lista" type="hidden" value="" runat="server">
        <div id="Contenedorderecho"  style="text-align: left; width: auto; padding: 1px; background: white; height: 90%" class="container-fluid pl-2 pr-2">
            <div id="contenido_titulo_val_radicacion" style="height: auto; margin-bottom: 10px; margin-left: 10px; margin-right: 10px" class="border_superior_radius_">
                <asp:UpdatePanel ID="UpdatePanelabel_val_radicacion" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <input id="Hidden_consecutivo_radicado" type="hidden" value="-1" runat="server">
                        <asp:Label ID="titulo_label_val_radicacion" runat="server" ForeColor="Black" Font-Size="10" Font-Names="Arial" Style="margin-left: 3px; display: none">Trazabilidad disponible</asp:Label>
                        &nbsp 
                            <asp:Label ID="Label_estado_transac" runat="server" Text="" Style="font-size: 8px; font-family: Arial"></asp:Label>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>

            <div id="contenido_datagrid_val_radicacion" style="height: auto; width: auto; position: relative; margin-left: 10px; margin-right: 10px">
                <asp:UpdatePanel ID="UpdatePanel_conenido_grid_val_radicacion" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Panel ID="Panel_gred" runat="server" Style="width: 100%; overflow: auto; height: 100%">
                            <div class=" container align-content-center ">
                                <asp:GridView ID="GridView_val_radicacion" runat="server" EnableViewState="true" Style="width: 100%; height: 99%; display: inline-flex" ViewStateMode="Enabled"
                                    AutoGenerateSelectButton="False" CssClass="table table-hover " GridLines="None" Font-Names="Arial">
                                    <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                    <FooterStyle BorderStyle="None" />
                                    <HeaderStyle CssClass="" BorderStyle="None" />
                                    <RowStyle CssClass="" />
                                    <PagerStyle CssClass="GridviewScrollPager_line" />

                                    <Columns>
                                        <asp:BoundField HeaderText="OPCIONES" />
                                    </Columns>
                                </asp:GridView>
                            </div>

                        </asp:Panel>
                    </ContentTemplate>
                    <Triggers>
                    </Triggers>
                </asp:UpdatePanel>
            </div>
            <div id="Contenido_botones_tipo_radicado" style="height: auto" class="border_inferior_radius_ modal-footer">

                <asp:UpdatePanel ID="UpdatePanel_botones_radicacion" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Button ID="Button_Exportar_Radicados" Text="Exportar" runat="server" ToolTip="Exportar lista" OnClientClick="retorna_colum_mtriz('Hidden_colum_header');" CssClass="btn  btn-info btn-sm" Style="margin-left: 10px; float: right; margin-right: 10px; margin-top: 5px" />
                        <asp:Button ID="Button1" Text="Actualizar" runat="server" ToolTip="Actualizar trazabilidad" CssClass="btn  btn-outline-success btn-sm" Style="margin-left: 10px; float: right; margin-top: 5px" />
                        <asp:Button ID="Button_detalle" Text="Actualizar" runat="server" Width="70px" ToolTip="Actualizar trazabilidad" CssClass="boton_azul" Style="margin-left: 10px; display: none" />
                        <input id="Hidden_colum_header" type="hidden" value="" runat="server">
                        <input id="Hidden_ruta_archivo" type="hidden" value="" runat="server">

                        <input id="hdnEmailID_VAL" type="hidden" value="-1" runat="server">
                    </ContentTemplate>

                </asp:UpdatePanel>
            </div>
        </div>
          <!--detalle_conector_trazabilidad!-->
         <div id="detalle_conector_trazabilidad">
            <asp:Panel ID="Panel_detalle_conector_trazabilidad" runat="server"  Style="display:none; color: White; width:100%; height:100% " CssClass="modal_content_general">              
                <asp:ModalPopupExtender ID="ModalPopupExtender_detalle_conector_trazabilidad" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_detalle_conector_trazabilidad"
                    PopupControlID="Panel_detalle_conector_trazabilidad"  CancelControlID="Buttoncerrarimpre_detalle_conector_trazabilidad"></asp:ModalPopupExtender>              
                <div id="Divcerrarbuton2_detalle_conector_trazabilidad" style="" class="modal_title_superior_"> 
                     <button type="button" style="margin-right:10px" onclick="document.getElementById('Buttoncerrarimpre_detalle_conector_trazabilidad').click();" class="close">&times;</button>
                </div>                         
                <asp:UpdatePanel ID="UpdatePane_detalle_conector_trazabilidad" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div id="Contenido_detalle_conector_trazabilidad" style="background-color: #FFFFFF; width:100%; height:auto" class="modal_content_back">
                            <div id="conten_detalle_traza" class="container-fluid text-center">
                                <h5> Detalle registro trazabilidad </h5>
                            </div>                                          
                            <div id="content_detalle" class="container-fluid" style="overflow:auto; width:100%" >
                                <table id="table_detalle" class=" table-responsive table-hover " style="width:100%">
                                    
                                     <tr>
                                         <td >
                                            <p> Fecha de asignación de la tarea</p>
                                         </td>    
                                          <td > 
                                             <asp:Label ID="TextBox_fecha_inici" runat="server" Text=""  ></asp:Label> 
                                         </td>    
                                     </tr>
                                     <tr>
                                         <td >
                                            <p>Fecha inicio de la tarea</p>
                                         </td>    
                                          <td >
                                             <asp:Label ID="TextBox_Fecha_Seleccion_" runat="server" Text="" ></asp:Label> 
                                         </td>    
                                     </tr>
                                     <tr>
                                          <td >
                                              <p>Fecha terminación de la tarea</p>
                                         </td>    
                                          <td >
                                              <asp:Label ID="TextBox_Fecha_Fin_" runat="server" Text="" ></asp:Label> 
                                         </td>    
                                     </tr>
                                     <tr>
                                         <td >
                                            <p>Tiempo de espera para seleccionar la tarea</p>
                                         </td>    
                                          <td >
                                             <asp:Label ID="TextBox_Duracion_Inicio_Seleccion_" runat="server" Text="" ></asp:Label>
                                         </td>    
                                     </tr>
                                     <tr>
                                          <td >
                                             <p>Tiempo estimado para terminar la tarea</p>
                                         </td>    
                                          <td >
                                              <asp:Label ID="TextBox_Duracion_Seleccion_Fin_" runat="server" Text="" ></asp:Label>
                                         </td>    
                                     </tr>
                                     <tr>
                                         <td >
                                             <p>Usuario asignado</p>
                                         </td>    
                                          <td >
                                              <asp:Label ID="TextBox_usuario_asignado_" runat="server" Text="" ></asp:Label>
                                         </td>    
                                     </tr>                                     <tr>
                                         <td >
                                             <p>Cargo usuario asignado</p>
                                         </td>    
                                          <td >
                                             <asp:Label ID="TextBox_cargo_usuario_asignado_" runat="server" Text="" ></asp:Label>
                                         </td>    
                                     </tr>
                                     <tr>
                                         <td >
                                             <p>Indentificador del estado</p>
                                         </td>    
                                          <td >
                                              <asp:Label ID="TextBox_id_Estado_" runat="server" Text="" ></asp:Label>
                                         </td>    
                                     </tr>
                                 </table>
                               
                            </div>                         
                           
                            <div id="Div5" style="float:right; text-align:right; margin-top:10px; display:none" >
                                <asp:Button ID="Button_detalle_conector_trazabilidad" runat="server" Text="Aceptar" Style="margin-right:10px"  CssClass="boton_azul"/>                             
                            </div>                         
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <div style="display: none; height: 1px">
                        <asp:Button ID="Button1__detalle_conector_trazabilidad" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                    <asp:Button ID="ButtonSalir_detalle_conector_trazabilidad" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                         <asp:Button ID="Buttoncerrarimpre_detalle_conector_trazabilidad" runat="Server" Text="" CssClass="invisible"
                         ToolTip=""/>
                </div>
                
                    
            </asp:Panel>
        </div>   
        <div id="inferior_bajo_boton" style="width: 0%; height: 0%; background-color: #E7EDF5; display: none">
        
            <asp:UpdatePanel ID="updatapanel_iframe" runat="server" UpdateMode="Conditional">
                <ContentTemplate>   
                     <iframe runat="server" id="ifmExcel_" width="0" height="0" marginheight="0" marginwidth="0"
                        frameborder="0"  />
                   
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
         <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 200px">
                <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
                Processing ...
            </div>
    </div>
    </form>
</body>
</html>
