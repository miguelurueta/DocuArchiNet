<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormDescargaRadicado.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormDescargaRadicado" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
     <script src="../js/ui/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
    <script src="../js/radicacion/WebFormDescargaRadicado.js"></script>
   <script  accesskey="javascript" type="text/javascript">
      
       
     </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
         <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" EnablePageMethods="true">
            </asp:ScriptManager>
          <input id="Hidden_ruta_archivo" type="hidden" value="" runat="server">
        <asp:UpdatePanel ID="update_panel_ruta_archivo" runat="server" >
            <ContentTemplate>
                <asp:Button ID="Button_descarga" runat="server" Text="Button" />
            </ContentTemplate>
            <Triggers>
                <asp:PostBackTrigger ControlID="Button_descarga" />
            </Triggers>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
