<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebForm_gestion_confirma_recibido_usuario.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebForm_gestion_confirma_recibido_usuario" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
       <script src="../js/ui/jquery-3.4.1.min.js"></script>  
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
    <link href="../ccs/themes/base/jquery.ui.all.css" rel="stylesheet" type="text/css" />   
    <script src="../js/gestion/WebForm_gestion_confirma_recibido_usuario.js"></script>
     <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <script defer src="../Awesome/js/all.js"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet">
  <link href="../Awesome/css/brands.css" rel="stylesheet">
  <link href="../Awesome/css/solid.css" rel="stylesheet">
    <script defer src="../Awesome/js/brands.js"></script>
  <script defer src="../Awesome/js/solid.js"></script>
  <script defer src="../Awesome/js/fontawesome.js"></script>
</head>
<body style="background-color:gray">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"      
        EnableScriptGlobalization="True">
    </asp:ScriptManager>  
         <script accesskey="javascript" type="text/javascript">
             Sys.WebForms.PageRequestManager.getInstance().add_initializeRequest(InitializeRequest);
             Sys.Application.add_load(ApplicationLoadHandler)
             var elment_postbak;
             var value_element;
             function ApplicationLoadHandler(sender, args) {
                 Sys.WebForms.PageRequestManager.getInstance().add_endRequest(CheckStatus);
             }
             function InitializeRequest(sender, args) {
                 posicion_update_pogres("progres_bar");
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
                     progres_hiden("progres_bar");
                 } catch (ex) {
                     alert("Funcion CheckStatus dice " + ex.message)
                 }
                 finally {
                     progres_hiden("progres_bar");
                 }
             }
            </script>
    <div >
        <div id="general" style="text-align: left; width: 30%; background-color: #ffffff; margin: auto; border-radius: 25px; padding: 20px; background: #ffffff; height: auto; margin-top: 2%; margin-bottom: 2%">
            <div style="text-align: center">
                <asp:Image ID="Imagesecion" runat="server" Height="50px"
                    ImageUrl="~/imagera/logo_trd.png" Style="height: 70px" />
                <div class="row mt-3">
                    <div class="col-12">
                        <asp:Label ID="Label_mensaje" runat="server" Text="Label" Style="font-size: 12px" CssClass="h6"></asp:Label>
                    </div>
                </div>       
                <div class="row mt-3">
                    <div class="col-12">
                        <asp:UpdatePanel ID="UpdatePanel_confir" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Button ID="Button_confirma" runat="server" Text="Confirmar" CssClass="btn btn-success" />
                                <br />
                                <asp:Label ID="Label_estado_error" runat="server" Text="" Style="font-family: Arial; font-size: 12px"></asp:Label>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
                
            </div>
        </div>
    </div>
        <!--mensaje_progreso evento-->
            <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 200px">
                <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
                Processing ...
            </div> 
    </form>
</body>
</html>
