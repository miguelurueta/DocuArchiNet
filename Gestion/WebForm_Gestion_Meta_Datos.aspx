<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebForm_Gestion_Meta_Datos.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebForm_Gestion_Meta_Datos" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Gestión meta datos</title>
     <link href="../Styles/Aplicaction.css" rel="stylesheet" />
      <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />    
     <script src="../js/ui/jquery-3.4.1.min.js"></script>  
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
     <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <script src="../js/gestion/WebForm_Gestion_Meta_Datos.js"></script>
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <script defer src="../Awesome/js/all.js"></script>
    <link href="../Awesome/css/fontawesome.css" rel="stylesheet">
    <link href="../Awesome/css/brands.css" rel="stylesheet">
    <link href="../Awesome/css/solid.css" rel="stylesheet">
    <script defer src="../Awesome/js/brands.js"></script>
    <script defer src="../Awesome/js/solid.js"></script>
    <script defer src="../Awesome/js/fontawesome.js"></script>
    <script src="../js/validate_campos.js"></script>
</head>
<body>
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
                   //
                   elment_postbak = args.get_postBackElement();
                   posicion_update_pogres('progres_bar');

               }
               function CheckStatus(sender, args) {
                   try {
                       if (elment_postbak.type == "button" || elment_postbak.type == "submit") {
                           elment_postbak.value = value_element;
                           elment_postbak.disabled = false;
                       }

                   }
                   catch (err) {
                       alert(" Funcion CheckStatus asincrona WebForm_Gestion_Meta_Datos.aspx" + err.message);
                   }
                   finally {
                       progres_hiden('progres_bar');
                   }
               }

            </script>

    <div>
        <div id="content_meta_data" class="modal-body">
            <asp:UpdatePanel ID="Update_control_meta_data" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                <ContentTemplate>
                    <asp:Panel ID="Panel_control_meta_data" runat="server" ScrollBars="Auto"
                        Height="100%" ViewStateMode="Enabled" Width="100%" >
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div id="content_boton" class="modal-footer">
            <asp:UpdatePanel ID="Update_control_boton_meta_data" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                <ContentTemplate>
                    <asp:Label ID="Label_estado" runat="server" Text="" style="font-size:8px; float:left"></asp:Label>          
                     <asp:Button ID="Button_aceptar_meta_data" runat="server" Text="Aceptar" Style="float: right" CssClass="btn btn-success"  />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
          <!--mensaje_progreso evento-->
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
