<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormPaswordGestion.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormPaswordGestion" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Cambio pasword</title>
     <script src="../js/ui/jquery-3.4.1.min.js"></script>  
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
    <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
    <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
      <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <script defer src="../Awesome/js/all.js"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet">
  <link href="../Awesome/css/brands.css" rel="stylesheet">
  <link href="../Awesome/css/solid.css" rel="stylesheet">
    <script defer src="../Awesome/js/brands.js"></script>
  <script defer src="../Awesome/js/solid.js"></script>
  <script defer src="../Awesome/js/fontawesome.js"></script>
    <script src="../js/radicacion/WebFormPaswordRadicacion.js"></script>
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
</head>
<body style="background-color:#A4A4A4">
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
        <div style="text-align: left; width: 40%; background-color: #FAFAFA; margin: auto; border-radius: 5px; padding: 20px; background: white; height: 100%" class="mt-4">
            <div id="modal_content_" class="modal-content_">
                <div id="superior" class="modal_title_superior_ modal-header">
                    <h6 class="modal-title d-inline ml-1">Cambiar contraseña Getión Documental</h6>
                </div>
                <div id="contenido" style="width: 100%; height: auto" class="modal-body">
                    
                    <div class="row mt-2">
                        <div class="col-6">
                            <h6 class="font-weight-light">Nueva Contraseña</h6>
                        </div>
                        <div class="col-6">
                            <asp:TextBox ID="TextBox_pasword" runat="server" Style="width: 100%" TextMode="Password" EnableViewState="false" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row mt-2">
                        <div class="col-6">
                            <h6 class="font-weight-light">Confirma Contraseña</h6>
                        </div>
                        <div class="col-6">
                            <asp:TextBox ID="TextBox_pasword_2" runat="server" Style="width: 100%" TextMode="Password" EnableViewState="false" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div id="pie_tol" style="width: 100%" class="modal-footer justify-content-end" >
                    <asp:UpdatePanel ID="update_general" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Button ID="Button_Cambiar" runat="server" Text="Aceptar"  CssClass="btn btn-success" ToolTip="Cambiar contraseña" />

                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
        <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 200px">
                    <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
                    Processing ...
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
    </form>
</body>
</html>
