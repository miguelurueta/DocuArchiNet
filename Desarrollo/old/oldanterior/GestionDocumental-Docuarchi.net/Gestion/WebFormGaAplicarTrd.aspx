<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormGaAplicarTrd.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormGaAplicarTrd" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Aplicar tabla de Retención</title>
      <script src="../js/ui/jquery-3.4.1.min.js"></script>  
     <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
     <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
     <link href="../ccs/themes/base/jquery.ui.all.css" rel="stylesheet" type="text/css" />
     <link href="../Styles/styleMenu.css" rel="stylesheet" type="text/css" /> 
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
    <script src="../js/gestion/WebFormGaAplicarTrd.js"></script>
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
                   //if (args.get_postBackElement().id == 'Button_actualiza_tipo_tramite') {
                   //    var doc = document.getElementById("Hidden_id_control_postback");
                   //    doc.value = "Button_actualiza_tipo_tramite";

                   //}
                   posicion_update_pogres('progres_bar');
               }
               function CheckStatus(sender, args) {
                   progres_hiden('progres_bar')
                   if (elment_postbak.id == "ComboBoxArea") {
                       plugin_grwedview();
                   }
                   
                   if (elment_postbak.id == "Button_asignar") {
                       //importa_dato_trd();
                       //return false;
                   }
                   auto_zise_aplicar_trd();
               }

            </script>
        <div id="contenido_general">
            <input id="Hidden_resultado" type="hidden" value="-1" runat="server">
            <input id="Hidden_id_organigrama" type="hidden" value="-1" runat="server">
            <input id="Hidden_0001" type="hidden" value="-1" runat="server">
            <input id="Hidden_id" type="hidden" value="-1" runat="server">
            <input id="Hidden_000_serie" type="hidden" value="-1" runat="server">
            <input id="Hidden_000_sub_serie" type="hidden" value="-1" runat="server">
            <div id="Contenido_superior" style="width: 100%" class="row p-1">
                <div class="col-6 mt-1">
                    <div class="row">              
                        <div class="col-12">
                            <asp:UpdatePanel ID="update_panel_drowlist" runat="server">
                                <ContentTemplate>
                                    <asp:DropDownList ID="ComboBoxArea" runat="server" AutoPostBack="true" CssClass="custom-select w-75"></asp:DropDownList>
                                    <div style="display: none">
                                        <input id="Hidden_id_area" type="hidden" value="0" runat="server">
                                        <asp:Button ID="Button_busqueda_tipo_general" runat="server" Text="" Style="display: none; width: 0px; height: 0px" />
                                        <asp:Button ID="Button_busqueda_tipo_area" runat="server" Text="" Style="display: none; width: 0px; height: 0px" />
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                </div>
                <div class="input-group  col-6 d-none">
                    <button id="Button_retaura_relacionados" class="btn btn-outline-secondary border-right-2 " title="Restaura expedientes relacionados" style="border-top-right-radius: 0px; border-bottom-right-radius: 0px" onclick="activa_menu('r_b_s_011')" type="button">
                        <i class="fal fa-long-arrow-left"></i>
                    </button>
                    <asp:TextBox ID="TextBox_buequeda_general" runat="server" class="form-control form-control-sm complex  border-left-0 w-50" placeholder="Buscar...." onkeypress=""></asp:TextBox>
                    <div class="input-group-append">
                        <button class="btn btn-outline-secondary" onclick="acti_busq_general_archivo_boton(event, this);" type="button">
                            <i class="fal fa-search"></i>
                        </button>
                    </div>
                </div>
            </div>
            <div id="contenido_gred" style="width: 100%; height: 80%; margin-top: 1px; overflow:auto" class="modal_content_back_inferior_superior modal-body pl-2">
                <asp:UpdatePanel ID="UpdatePanelmensaje" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:GridView ID="GridViewlista" runat="server"  EnableViewState="true" Width="99%"
                            AutoGenerateSelectButton="False" CssClass="table  font-weight-light" GridLines="None">
                            <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                            <HeaderStyle CssClass="GridviewScrollHeader_line_boot" />
                            <PagerStyle CssClass="pagination-ys" />
                            <Columns>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>

                        </asp:GridView>

                    </ContentTemplate>
                    <Triggers>
                    </Triggers>
                </asp:UpdatePanel>
            </div>
            <div id="contenido_inferior" style="width: 100%" class="modal-footer justify-content-end">
                <asp:UpdatePanel ID="Updatepanel_botones" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Button ID="Button_asignar" runat="server" Text="Aplicar" OnClientClick="importa_dato_trd();" CssClass="btn btn-success"  ToolTip="Asignar tabla de retención" />
                        <asp:Button ID="Button_buascar" runat="server" Text="Buscar" CssClass="boton_azul" Style="margin-bottom: 5px; margin-top: 2px; margin-left: 5px; display: none" />
                        <asp:TextBox ID="TextBox_busqueda" runat="server" Style="margin-left: 10px; margin-bottom: 5px; display: none" placeholder="Buscar en la lista"></asp:TextBox>
                        <asp:CheckBox ID="CheckBox_busqueda" runat="server" Text="Palabra commpleta" Font-Size="8px" Font-Names="arial" Style="display: none" />
                        <asp:Button ID="Button_Exportar_Lista" Text="Exportar" runat="server" ToolTip="Exportar lista"
                            OnClientClick="retorna_colum_mtriz('Hidden_colum_header');"  CssClass="btn btn-success" />
                        <asp:Label ID="Label_estado" runat="server" Text="" Style="font-family: Arial; font-size: 12px; margin-left: 12px; float: right"></asp:Label>
                        <input id="Hidden_colum_header" type="hidden" value="0" runat="server">
                        <input id="Hidden_ruta_archivo" type="hidden" value="" runat="server">
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
          <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 50px">
            <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
            Processing ...
        </div>
         <div id="inferior_bajo_boton" style="width: 0%; height: 0%; background-color: #E7EDF5; display: none">
            <asp:UpdatePanel ID="updatapanel_iframe" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <iframe runat="server" id="ifmExcel_" width="0" height="0" marginheight="0" marginwidth="0"
                        frameborder="0" />
                </ContentTemplate>
            </asp:UpdatePanel>
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
