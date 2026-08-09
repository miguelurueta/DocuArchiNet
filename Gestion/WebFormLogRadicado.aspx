<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormLogRadicado.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormLogRadicado" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title> Consulta log radicado</title>
   
    <script src="../js/ui/jquery-3.4.1.min.js"></script> 
    <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
     <script src="../js/ScrollableGridPlugin.js"></script>   
    <script src="../js/ScrollableGridViewPlugin_ASP.NetAJAXmin.js" type="text/javascript"></script>
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
   <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
   <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <script src="../js/gestion/WebFormLogRadicado.js"></script>
    <script defer src="../Awesome/js/all.js"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet">
  <link href="../Awesome/css/brands.css" rel="stylesheet">
  <link href="../Awesome/css/solid.css" rel="stylesheet">
    <script defer src="../Awesome/js/brands.js"></script>
  <script defer src="../Awesome/js/solid.js"></script>
  <script defer src="../Awesome/js/fontawesome.js"></script>

</head>
<body style="overflow:hidden">
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

                     if (elment_postbak.id == "Button_Exportar_Radicados") {
                         plugin_grwedview();
                         if (document.getElementById("Hidden_ruta_archivo").value !== "") {
                             //plugin_grwedview();
                             //document.getElementById('ifmExcel_').src = "../radicador/WebFormDescargaRadicado.aspx";
                         }
                     }
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
        <div id="Contenedorderecho" style="width: 100%; position: inherit; left: auto; float: right; height: 99.5%">
            <div id="contenido_titulo_val_radicacion" class="modal-header">
                <asp:UpdatePanel ID="UpdatePanelabel_val_radicacion" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <input id="hdnEmailID_VAL" type="hidden" value="-1" runat="server">
                        <input id="Hidden_consecutivo_radicado" type="hidden" value="-1" runat="server">
                        <asp:Label ID="titulo_label_val_radicacion" runat="server" ForeColor="Black" CssClass="h6 font-weight-light p-1" Style="font-family:'Segoe UI'; margin-left:3px">Trazabilidad disponible</asp:Label>
                        &nbsp 
                            <asp:Label ID="Label_estado_transac" runat="server" Text="" CssClass="h6 font-weight-light p-1" Style="font-family:'Segoe UI';  font-size:8px"></asp:Label>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>

            <div id="contenido_datagrid_val_radicacion" style="height: 60%; width: 100%; position: relative; margin-top: 1px; overflow:auto " class="p-3">
                <asp:UpdatePanel ID="UpdatePanel_conenido_grid_val_radicacion" runat="server" UpdateMode="Conditional" >
                    <ContentTemplate>
                        <asp:GridView ID="GridView_val_radicacion" runat="server"     EnableViewState="true"
                                   PagerSettings-Position="Top"  style="width:100%; font-family:Segoe UI"
                                    AutoGenerateSelectButton="False" CssClass="table font-weight-light  " GridLines="None"  >
                                    <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                    <HeaderStyle CssClass="GridviewScrollHeader_line_boot" BorderStyle="None" />
                                    <RowStyle CssClass=""  />
                                    <PagerStyle CssClass="pagination-ys" />
                                </asp:GridView>
                    </ContentTemplate>
                    <Triggers>
                    </Triggers>
                </asp:UpdatePanel>
            </div>
            <div id="Contenido_botones_tipo_radicado" style="width: 100%" class="modal-footer justify-content-end" >      
                    <asp:UpdatePanel ID="UpdatePanel_botones_radicacion" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Button ID="Button_Exportar_Radicados" Text="Exportar" runat="server"  ToolTip="Exportar lista" OnClientClick="retorna_colum_mtriz('Hidden_colum_header');" CssClass="btn btn-success" Style="margin-left: 10px" />
                            <asp:Button ID="Button1" Text="Actualizar" runat="server"  ToolTip="Actualizar registros de log" CssClass="btn btn-success" Style="margin-left: 10px" />
                            <input id="Hidden_colum_header" type="hidden" value="" runat="server">
                            <input id="Hidden_ruta_archivo" type="hidden" value="" runat="server">
                        </ContentTemplate>
                    </asp:UpdatePanel>
            </div>
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