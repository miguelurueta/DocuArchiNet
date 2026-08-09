<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormAnotacion.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormAnotacion" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head  runat="server">
    <title></title>
    
     <script src="../js/ui/jquery-3.4.1.min.js"></script>  
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
    <script src="../js/workflow/WebFormAnotacion.js"></script>
      <script src="../js/ScrollableGridViewPlugin_ASP.NetAJAXmin.js" type="text/javascript"></script>
    <script src="../Fixed-Header-Table-master/gridviewScroll.min.js"></script>
    <script src="../js/Filtrar.js"></script>
    <script src="../js/validate_campos.js"></script>
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
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
   <style type="text/css">
  
        .invisible { 
            visibility: hidden; 
        } 
    </style>
   
       
</head>

<body >
    <form id="form1" runat="server" onkeypress="return caracter_especial(event,this)">
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
                  if (elment_postbak.id == "ButtonGuardar") {
                      if (document.getElementById("Hidden_resultado_nota_add_update").value == "YES") {
                          document.getElementById("Hidden_resultado_nota_add_update").value = "";
                          auto_zise_anotacion(0);
                          auto_size_gredview();
                      }
                  }
                  if (elment_postbak.id == "ButtonActualizar") {
                      if (document.getElementById("Hidden_resultado_nota_add_update").value == "YES") {
                          document.getElementById("Hidden_resultado_nota_add_update").value = "";
                          actualiza_gre_campos_dinamicos();
                         
                      }
                  }
                  if (elment_postbak.id == "ButtonEliminar") {
                      if (document.getElementById("Hidden_resultado_eliminar_guia").value == "YES") {
                          document.getElementById("Hidden_resultado_eliminar_guia").value = "";
                          eliminar_fila_data_gred("GridViewlista");
                          
                      }
                  }
                  progres_hiden('progres_bar');
                  

              }

            </script>    
        <div id="modal_content_anotacion" class="modal-content_ p-1">
            <div id="contenido_procesa_content_anotacion" style="width: auto; height: auto" class="modal_content_back_">
                <div id="contenido_titulo" style="width: 100%" class="p-2">
                    <asp:Label ID="LabelTitulo" runat="server" Text="Anotaciones de la Tarea" CssClass="h6"> </asp:Label>
                </div>
                <div id="contenido_gred_anotacion" style="height: auto; width: 100%; overflow: auto">
                    <asp:UpdatePanel ID="UpdatePanelanotacion" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Panel ID="Panelactividad" runat="server"
                                Enabled="true" Style="margin-bottom: 5px">
                                <asp:GridView ID="GridViewlista" runat="server" Style="width: 100%"
                                    AutoGenerateSelectButton="False" CssClass="filtrar table" GridLines="None">
                                    <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                    <HeaderStyle CssClass="GridviewScrollHeader_line_boot" />
                                    <Columns>
                                        <asp:BoundField HeaderText="OPCIONES" />
                                    </Columns>
                                </asp:GridView>
                            </asp:Panel>
                            <input id="hdnEmailID" type="hidden" value="-1" runat="server">
                        </ContentTemplate>
                        <Triggers>
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
                <div id="content_boton" style="width: 100%; border-bottom:none"  class="modal-footer justify-content-end">
                    <asp:UpdatePanel ID="Updateboton" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <input id="Hidden_resultado_eliminar_guia" type="hidden" value="" runat="server">
                            <asp:Button ID="Button_Show_Guardar" runat="server" Text="Nueva nota" ToolTip="Nueva anotacion" CssClass="btn  btn-success" Style="margin-left: 5px; margin-top: 5px" OnClientClick="labe_texto_modal_nota('Guardar nota')" />
                            <asp:Button ID="ButtonEliminar" runat="server" Text="Eliminar" ToolTip="Eliminar anotacion" CssClass="boton_azul" Style="display:none" />                  
                            <asp:Button ID="Buttonclidatos" runat="server" Text="Ver nota" ToolTip="Ver y editar anotacion" Style="display:none"
                                OnClientClick="labe_texto_modal_nota('Contenido nota')" CssClass="boton_azul" />
                             
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <asp:Button ID="Buttonhide" runat="server" Text="Button" Style="display: none" />
                </div>
            </div>
        </div>   
         <div style="display:none">
            
             <input id="HiddenPROMP" type="hidden" value="0" runat="server">
             <input id="Hidden_event_page" type="hidden" value="" runat="server">
         </div>
         <div id="nota_respuesta">
                <asp:Panel ID="Panel_nota_respuesta" runat="server" Style="display: none; width: 66%; height: auto; margin: auto" CssClass="modal_content_general_">
                    <asp:ModalPopupExtender ID="ModalPopupExtender_edition_nota_respuesta" runat="server"  TargetControlID="ButtonSalir_nota_respuesta" BackgroundCssClass="FondoAplicacion"
                        CancelControlID="Button_cerrar_nota_respuesta" PopupControlID="Panel_nota_respuesta">
                    </asp:ModalPopupExtender>
                    <div id="modal_content_nota_respuesta" class="modal-content">  
                    <div id="divcabecer2_radica_documento" class="modal_title_superior_ modal-header">
                        <asp:Label ID="Label_nota_respuesta" class="modal-title d-inline ml-1 h6" runat="server" Text="Guardar nota" >
                        </asp:Label>
                         <button type="button" value="Button_cerrar_nota_respuesta" class="close da_event_captive">&times;</button>                   
                    </div>
                    <div id="contenido_procesa_nota_respuesta" style=" border-top:none; overflow:hidden" class="p-1">
                            <asp:UpdatePanel ID="UpdatePaneltextbos" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:TextBox ID="TextBoxdatos" runat="server" placeholder="digita la nota aquí" CssClass="form-control" TextMode="MultiLine"
                                        Columnas="50"
                                        Filas="10"
                                        Text="" Style="width: 100%">
                                    </asp:TextBox>                    
                                </ContentTemplate>
                            </asp:UpdatePanel>      
                    </div>
                    <div id="content_boton_nota" class="modal-footer">
                            <asp:UpdatePanel ID="UpdatePanel_guardar_nota" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <input id="hidden_campos_dinamicos_aleas" type="hidden" value="" runat="server">
                                    <input id="hidden_valore_campos" type="hidden" value="" runat="server">
                                    <input id="Hidden_resultado_nota_add_update" type="hidden" value="" runat="server">
                                    <asp:Button ID="ButtonGuardar" runat="server" Text="Guardar " ToolTip="Guardar nueva anotacion" CssClass="btn btn-success" Style="margin-left: 5px"
                                        OnClientClick="ConfirmMensaje(&quot;Desea guardar la nueva anotacion&quot;);" />
                                    <asp:Button ID="ButtonActualizar" runat="server" Text="Actualizar" ToolTip="Actualizar anotacion" CssClass="btn btn-success" Style="margin-left: 5px"
                                        OnClientClick="ConfirmMensaje(&quot;Desea actualizar la anotacion anotacion&quot;);" />
                                </ContentTemplate>
                            </asp:UpdatePanel>

                        </div>
                        <div style="display: none; height: 1px">
                            <asp:Button ID="Button_nota_respuesta" CssClass="invisible" runat="server" Text="Button" Height="0px" Width="0px" Style="display: none" />
                            <asp:Button ID="ButtonSalir_nota_respuesta" CssClass="invisible" runat="server" Text="Button" Height="0px" Width="0px" Style="display: none" />
                            <asp:Button ID="Button_cerrar_nota_respuesta" runat="Server" Text="X" CssClass="modal_boton_hiden" />
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
