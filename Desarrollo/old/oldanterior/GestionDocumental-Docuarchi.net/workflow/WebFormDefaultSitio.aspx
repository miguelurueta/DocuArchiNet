<%@ Page Language="vb" AutoEventWireup="false"   CodeBehind="WebFormDefaultSitio.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormDefaultSitio" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <script src="../js/ui/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
    <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
    <div style="align-content:center">
        <div id="msgInProgress" style="display: none;">
            <div id="mySpinner" style="width: 40px; height: 40px; "></div>
            <br />
            <asp:Label ID="Label_detalle_list" runat="server" Text="Estamos detectando el WCPP utilitario client para imprimir... " style="font-family:Arial; font-size:12px">
            </asp:Label>        
            <br />
            <asp:Label ID="Label1" runat="server" Text="Por favor espere seconds... " style="font-family:Arial; font-size:12px">
            </asp:Label>
            
            <br />
        </div>
      <div id="msgInstallWCPP" style="display: none;">
                  <br />
          <asp:Label ID="Label2" runat="server" Text="Instalación Componente de impresión para el cliente wccp.exe" Style="font-family: Arial; font-size: 25px">

          </asp:Label>
                <p>
                    <a href="http://www.neodynamic.com/downloads/wcpp/" Style="font-family: Arial; font-size: 18px" target="_blank">Descarga y instala este componente, es necesario para  imprimir cualquier documento.</a><br />
                </p>     
            </div>
    </div>
    </form>
  
<%-- Register the WebClientPrint script code --%>

   
</body>
</html>
