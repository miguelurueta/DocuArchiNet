<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormVisorDescarga.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormVisorDescarga" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Visor docuarchi.net</title>
    <script src="../js/ui/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
    <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
   
   <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <script src="../js/Docuarchi/WebFormDaVisorExternoDescarga.js"></script>
     <link href="../Styles/Aplicaction.css" rel="stylesheet" />
     <script  accesskey="javascript" type="text/javascript">
        
   </script>
</head>
<body style="background-color:ActiveCaption" >
    <form id="form1" runat="server">
          <asp:ScriptManager ID="ScriptManager1" runat="server"
            EnableScriptGlobalization="True" EnablePageMethods="True">
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
                      posicion_update_pogres('progres_bar');
                  }
                  catch (err) {
                      alert(err.message + " funcion InitializeRequest " + err.message);
                  }
              }
              function CheckStatus(sender, args) {
                  try {

                  }
                  catch (err) {
                      alert(err.message + " funcion InitializeRequest " + err.message);
                  }
                  finally {
                      progres_hiden('progres_bar');
                  }
              }

        </script>
    <div style="text-align:center; margin-top:20%;  " >
        <asp:UpdatePanel ID="UpdatePanel_Descarga" runat="server">
            <ContentTemplate>
                <div>
                    <asp:ImageButton ID="ImageButton_descarga" runat="server" ImageUrl="../Docuarchi/imagenes/descarga-boton.png" Width="200" />
                </div>
                 
            </ContentTemplate>
        </asp:UpdatePanel>

       
    </div>
        <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 50px">
            <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
            Processing ...
        </div>
        <asp:UpdatePanel ID="updatapanel_iframe" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <iframe runat="server" id="ifmExcel_" width="0" height="0" marginheight="0" marginwidth="0"
                    frameborder="0" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>

