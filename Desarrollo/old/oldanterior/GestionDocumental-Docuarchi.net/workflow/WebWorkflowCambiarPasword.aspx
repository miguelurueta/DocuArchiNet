<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebWorkflowCambiarPasword.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebWorkflowCambiarPasword" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
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
<body style="height:215px; width:270px">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"
            EnableScriptGlobalization="True" EnablePageMethods="True">
        </asp:ScriptManager> 
        <div id="contenedorgenral" style="height: 211px">
            <div id="contenido_titulo" style="width:100%; height:10%; background-color: #E7EDF5" >
                <asp:Label ID="Label3" runat="server" Text="Cambiar pasword usuario" Font-Size="12" Font-Names="arial" Style="float: left; margin-left:10px; color:black"></asp:Label>
            </div>
                <br />
                <asp:Label ID="Label1" runat="server" Text="Pasword Usuario" Font-Size="10" Font-Names="arial" Style="float: left; margin-left:10px"></asp:Label>
                <br />
                <asp:TextBox ID="TextBoxPaswuno" runat="server" TextMode="Password"
                    Style="float: left; margin-left: 10px"></asp:TextBox>
            
                <br />
                <br />
                <asp:Label ID="Label2" runat="server" Text="Confirma Pasword" Font-Size="10" Font-Names="arial" Style="float:left; margin-left:10px "></asp:Label>
                <br />
                <asp:TextBox ID="TextBoxPaswdos" runat="server" TextMode="Password" Style="float: left; margin-left:10px"></asp:TextBox>
            
            <br />
            <div id="contenido_boton" style="margin-top: 20px; width:100%; background-color: #E7EDF5">
                <asp:UpdatePanel ID="Updatepanel_Boton" runat="server" UpdateMode="Conditional" >
                    <ContentTemplate>
                        <asp:Button ID="ButtonAceptar" runat="server" Text="Aceptar"
                    Style="float: left; margin-left: 200px;" Font-Size="Smaller" Height="26px" CssClass="boton" />
                    </ContentTemplate>
                </asp:UpdatePanel>
                
            </div>
        </div>
    </form>
</body>
</html>
