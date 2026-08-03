<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormRaNotasSolicitudesAprobacion.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormRaNotasSolicitudesAprobacion" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1"  runat="server">
    <title></title> 
     <script src="../js/ui/jquery-3.4.1.min.js"></script>
      <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
    <script src="../js/radicacion/WebFormRaNotasSolicitudesAprobacion.js"></script>
      <script src="../js/ScrollableGridViewPlugin_ASP.NetAJAXmin.js" type="text/javascript"></script>
    <script src="../Fixed-Header-Table-master/gridviewScroll.min.js"></script>
    <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
     <script defer src="../Awesome/js/all.js"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet">
  <link href="../Awesome/css/brands.css" rel="stylesheet">
  <link href="../Awesome/css/solid.css" rel="stylesheet">
    <script defer src="../Awesome/js/brands.js"></script>
  <script defer src="../Awesome/js/solid.js"></script>
  <script defer src="../Awesome/js/fontawesome.js"></script>
    <script src="../js/Filtrar.js"></script>
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
   <style type="text/css">
  
        .invisible { 
            visibility: hidden; 
        } 
    </style>
   
       
</head>

<body style="width:99%">
    <form id="form1" runat="server" >
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
                  progres_hiden('progres_bar');
              }

            </script>    
    
        <div id="Listnotacion" style="height: 100%; width: 100%; overflow: hidden; margin-top: 10px" class="container-fluid">
            <asp:UpdatePanel ID="UpdatePanelanotacion" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Panel ID="Panelactividad" runat="server" Wrap="False"
                        Enabled="true" Style="overflow: auto; width: 100%; height: 99%">
                        <asp:GridView ID="GridViewlista" runat="server"   EnableViewState="true"
                                   PagerSettings-Position="Top"  style="width:100%; font-family:Segoe UI"
                                    AutoGenerateSelectButton="False" CssClass="table font-weight-light  " GridLines="None"  >
                                    <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                    <HeaderStyle CssClass="GridviewScrollHeader_line_boot" BorderStyle="None" />
                                    <RowStyle CssClass=""  />
                                    <PagerStyle CssClass="pagination-ys" />
                                    <Columns>
                                        <asp:BoundField HeaderText="OPCIONES"   />
                                    </Columns>

                                </asp:GridView>
                    </asp:Panel>
                </ContentTemplate>
                <Triggers>
                </Triggers>
            </asp:UpdatePanel>
        </div>
          
        <div id="buton" style="width: 100%; float: left; height: 10%; margin: 0px 2px 1px 1px; overflow: hidden; display:none">        
            <asp:UpdatePanel ID="Updateboton" runat="server" UpdateMode="Conditional">
                <ContentTemplate> 
                    <asp:Label ID="Label_Estado" runat="server" Text="" style="font-size:11px; font-family:Arial; float:left"></asp:Label>
                     <INPUT id="Hidden_resultado_eliminar_guia" type="hidden" value="" runat="server" >  
                    <asp:Button ID="Buttonclidatos" runat="server" Text="Ver nota" ToolTip="Ver anotacion"  style="margin-top:5px; float:right; margin-right:5px; display:none"
                          CssClass="btn btn-primary" />        
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:Button ID="Buttonhide" runat="server" Text="Button" style="display:none" />
             <INPUT id="hdnEmailID" type="hidden" value="0" runat="server" >
             <INPUT id="HiddenPROMP" type="hidden" value="0" runat="server" >
             <INPUT id="Hidden_event_page" type="hidden" value="" runat="server" >   
        </div>
        <div id="nota_respuesta">
            <asp:Panel ID="Panel_nota_respuesta" runat="server" Style="display: none; width: 66%; height: auto" CssClass="modal_content_general_">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_nota_respuesta" runat="server" BehaviorID="Panel_nota_respuesta_ModalPopupExtender"
                    TargetControlID="ButtonSalir_nota_respuesta" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_nota_respuesta" PopupControlID="Panel_nota_respuesta">
                </asp:ModalPopupExtender>
                <div class="modal-content">
                    <div id="divcabecer2_radica_documento" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title">Nota</h6>
                        <button type="button" value="Button_cerrar_nota_respuesta" class="close da_event_captive">&times;</button>
                    </div>
                    <div id="contenido_procesa_nota_respuesta" style="background-color: white; width: 100%; height: 99%; border-top: none" class="modal_content_back  modal-body">
                        <div id="anotacion" style="height: auto; width: 100%">
                            <asp:UpdatePanel ID="UpdatePaneltextbos" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:TextBox ID="TextBoxdatos" runat="server" TextMode="MultiLine" CssClass="form-control"
                                        Columnas="50"
                                        Filas="10"
                                        Text="" Style="height: 150px; width: 98%; margin-left: 3px; margin-top: 3px" placeholder="digita la nota aquí">
                                    </asp:TextBox>
                                    <asp:TextBoxWatermarkExtender ID="TextBoxdatos_TextBoxWatermarkExtender" runat="server" BehaviorID="TextBoxdatos_TextBoxWatermarkExtender" TargetControlID="TextBoxdatos" WatermarkCssClass="watermark" WatermarkText="digita la nota aquí" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>

                        <div style="display: none; height: 1px">
                            <asp:Button ID="Button_cerrar_nota_respuesta" runat="Server" Text="" CssClass="invisible" />
                            <asp:Button ID="Button_nota_respuesta" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                            <asp:Button ID="ButtonSalir_nota_respuesta" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                        </div>

                    </div>
                    <div class="modal-footer">
                        <asp:UpdatePanel ID="UpdatePanel_guardar_nota" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <input id="hidden_campos_dinamicos_aleas" type="hidden" value="" runat="server">
                                <input id="hidden_valore_campos" type="hidden" value="" runat="server">
                                <input id="Hidden_resultado_nota_add_update" type="hidden" value="" runat="server">
                                <asp:Button ID="ButtonGuardar" runat="server" Text="Guardar " ToolTip="Guardar nueva anotacion" CssClass=" btn  btn-success" Style="margin-left: 5px; display: none"
                                    OnClientClick="ConfirmMensaje(&quot;Desea guardar la nueva anotacion&quot;);" />
                                <asp:Button ID="ButtonActualizar" runat="server" Text="Actualizar" ToolTip="Actualizar anotacion" CssClass="btn btn-success" Style="margin-left: 5px; display: none"
                                    OnClientClick="ConfirmMensaje(&quot;Desea actualizar la anotacion anotacion&quot;);" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>

                </div>

            </asp:Panel>
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
        <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 50px">
            <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
            Processing ...
        </div>
    </form>
</body>
</html>
