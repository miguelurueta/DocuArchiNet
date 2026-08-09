<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebWorkflowIntervaloAlarma.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebWorkflowIntervaloAlarma" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     <script src="../js/ui/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
    <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
</head>
<body style="height:215px; width:370px">
    <form id="form1" runat="server">
   
        <asp:ScriptManager ID="ScriptManager1" runat="server"
            EnableScriptGlobalization="True" EnablePageMethods="True">
        </asp:ScriptManager> 
        <div id="contenido_titulo" style="width: 100%; height: 20px; background-color: #E7EDF5">
            <asp:Label ID="Label3" runat="server" Text="Cambiar intervalo de alarma workflow" Font-Size="11" Font-Names="arial" Style="float: left; margin-left: 10px; color:black"></asp:Label>
        </div>
        <br />
        <asp:Label ID="Label1" runat="server" Text="Selecciona intervalo" Font-Names="arial" Font-Size="10"></asp:Label>
        <br />
        <asp:DropDownList ID="DropDownListIntervalo" runat="server">
        </asp:DropDownList>
        <br />
        <br />
         <div id="contenido_boton" style="margin-top: 20px; width:100%">
             <asp:UpdatePanel ID="Updatepanel_Boton" runat="server" UpdateMode="Conditional">
                 <ContentTemplate>
                     <asp:Button ID="ButtonAceptar" runat="server" Text="Aceptar" CssClass="boton" Style="margin-left: 300px" />
                 </ContentTemplate>
             </asp:UpdatePanel>
        <br />
        <asp:Label ID="Label2" runat="server" Text="Recuerda iniciar nueva sesión al aplicar este cambio" Font-Size="11px" ForeColor="Blue"></asp:Label>
    </div>
    </form>
</body>
</html>
