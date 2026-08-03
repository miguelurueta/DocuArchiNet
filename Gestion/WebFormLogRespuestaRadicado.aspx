<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormLogRespuestaRadicado.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormLogRespuestaRadicado" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title> Consulta log respuesta</title>
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
      <script src="../js/ui/jquery-3.4.1.min.js"></script>
    <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
     <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
  
   <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
   <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <script src="../js/gestion/WebFormLogRespuestaRadicado.js"></script>
    <script  accesskey="javascript" type="text/javascript">
      
    </script>
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
                 
                 try {
                     elment_postbak = args.get_postBackElement();

                     if (elment_postbak.id == "Button_Exportar_Radicados") {
                         plugin_grwedview();
                        
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
        <div id="Contenedorderecho" style="width: 100%; position: inherit; left: auto; float: right; height:100%; float: right">
            <div id="contenido_titulo_val_radicacion" style="height: 5%; width: 96%; margin-left:5px" class="title_sup_redon_ ">
                <asp:UpdatePanel ID="UpdatePanelabel_val_radicacion" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <input id="hdnEmailID_VAL" type="hidden" value="-1" runat="server">
                        <input id="Hidden_consecutivo_radicado" type="hidden" value="-1" runat="server">
                        <asp:Label ID="titulo_label_val_radicacion" runat="server" ForeColor="Black"  style="margin-left:5px">Trazabilidad disponible</asp:Label>
                        &nbsp 
                            <asp:Label ID="Label_estado_transac" runat="server" Text="" Style="font-size: 8px; font-family: Arial"></asp:Label>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>

            <div id="contenido_datagrid_val_radicacion" style="height: 60%; width: 96%; position: relative; overflow:auto; margin-left:10px; margin-right:10px" class="conten_gred_border_  ">
                <asp:UpdatePanel ID="UpdatePanel_conenido_grid_val_radicacion" runat="server" UpdateMode="Conditional" >
                    <ContentTemplate>
                        <div style="">
                            <asp:GridView ID="GridView_val_radicacion" runat="server" Width="90%" EnableViewState="true" GridLines="None" CssClass="table"
                                AutoGenerateSelectButton="False" AllowPaging="false" Font-Size="12px" PagerSettings-Position="Top" AllowSorting="false">
                                <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                <FooterStyle BorderStyle="None" />
                                <HeaderStyle CssClass="" BorderStyle="None" />
                                <RowStyle CssClass="" />
                                <PagerStyle CssClass="" />

                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                    </Triggers>
                </asp:UpdatePanel>
            </div>
            <div id="Contenido_botones_tipo_radicado" style="height: auto; width: 96%;  " class="title_inf_redon_">    
                <div id="contennido_buton" style="width: 100%; height: auto">
                    <asp:UpdatePanel ID="UpdatePanel_botones_radicacion" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Button ID="Button_Exportar_Radicados" Text="Exportar" runat="server"  ToolTip="Exportar lista" OnClientClick="retorna_colum_mtriz('Hidden_colum_header');" CssClass="btn  btn-info btn-sm"  Style="margin-top:5px; margin-bottom:5px; float:right; margin-right: 1px"  />
                            <asp:Button ID="Button1" Text="Actualizar" runat="server"  ToolTip="Actualizar trazabilidad" CssClass="btn btn-success btn-sm" Style="margin-top:5px; margin-bottom:5px; float:right; margin-right: 10px" />
                            <input id="Hidden_colum_header" type="hidden" value="" runat="server">
                            <input id="Hidden_ruta_archivo" type="hidden" value="" runat="server">
                            <input id="Hidden_postbak" type="hidden" value="" runat="server">
                        </ContentTemplate>

                    </asp:UpdatePanel>
                </div>
             
            </div>
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
