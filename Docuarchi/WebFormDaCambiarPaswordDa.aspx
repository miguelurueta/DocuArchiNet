<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormDaCambiarPaswordDa.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormDaCambiarPaswordDa" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Cambio pasword</title>
    <script src="../js/ui/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
    <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
    <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
    <script src="../js/radicacion/WebFormPaswordRadicacion.js"></script>
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
</head>
<body>
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
                    posicion_update_pogres('progres_bar');
                }
                function CheckStatus(sender, args) {
                    progres_hiden('progres_bar');
                }

            </script>
    <div>
       <div id="superior" style="width:100%; height:30%;background-color: #E7EDF5; margin-bottom:10px" >
             <asp:Label ID="Label5" runat="server" Text="Cambiar contraseña DocuArchi.web" Style="font-family:Arial; font-size:20px;color:black"></asp:Label>
         </div>
        <div id="contenido" style="width:100%; height:60%" >
            <asp:UpdatePanel ID="update_contenido" runat="server" UpdateMode="Conditional">
                <ContentTemplate >
                    <asp:Label ID="Label2" runat="server" Text="Las contraseñas deben tener como mínimo ocho caracteres" Style="font-family: Arial; font-size:10px; margin-left:5px"></asp:Label>
                    <br /> <br />
                    <asp:Label ID="Label4" runat="server" Text="Nueva Contraseña" Style="font-family: Arial; margin-left:5px; font-size:12px"></asp:Label>
                    &nbsp &nbsp &nbsp &nbsp
                    <asp:TextBox ID="TextBox_pasword" runat="server" Style="width: 230px"  TextMode="Password" EnableViewState="True" ></asp:TextBox>
                    <br /> <br />
                    <asp:Label ID="Label1" runat="server" Text="Confirma Contraseña " Style="font-family: Arial; margin-left:5px; font-size:12px" ></asp:Label>          
                    &nbsp &nbsp
                    <asp:TextBox ID="TextBox_pasword_2" runat="server" Style="width: 230px"  TextMode="Password" EnableViewState="True"></asp:TextBox>   
                    <br />
                    <br />
                </ContentTemplate>
            </asp:UpdatePanel>                    
                   
            <asp:UpdatePanel ID="update_general" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Button ID="Button_Cambiar" runat="server" Text="Aceptar" Style="margin-left: 300px" CssClass="boton" ToolTip="Cambiar contraseña" />
                    <br />
                    
                </ContentTemplate>
            </asp:UpdatePanel>
         <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 200px">
                <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
                Processing ...
            </div>
        <div id="pie_tol" style="width:100%; height:10%; background-color: #E7EDF5; margin-top:20px" >
             
         </div>
    </div>
    </div>
    </form>
</body>
</html>
