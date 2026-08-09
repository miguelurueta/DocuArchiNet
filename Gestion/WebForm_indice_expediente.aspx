<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebForm_indice_expediente.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebForm_indice_expediente" %>
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
        <script src="../js/gestion/WebForm_indice_expediente.js"></script>
        <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
        <script src="../bootstrap/js/bootstrap.min.js"></script>
        <link href="../Styles/bootra-person.css" rel="stylesheet" />
        <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
        <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
        <link href="../Styles/Aplicaction.css" rel="stylesheet" />
        <script src="../js/java_general/general_code_java.js"></script>
        <script  src="../Awesome/js/all.js"></script>
        <link href="../Awesome/css/fontawesome.css" rel="stylesheet"/>
       <link href="../Awesome/css/brands.css" rel="stylesheet"/>
  <link href="../Awesome/css/solid.css" rel="stylesheet"/>
    <script  src="../Awesome/js/brands.js"></script>
  <script  src="../Awesome/js/solid.js"></script>
  <script  src="../Awesome/js/fontawesome.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="True">
        </asp:ScriptManager>
        <script type="text/javascript" language="javascript">
            Sys.WebForms.PageRequestManager.getInstance().add_initializeRequest(InitializeRequest);
            Sys.Application.add_load(ApplicationLoadHandler)
            var elment_postbak;
            var value_element;
            function ApplicationLoadHandler(sender, args) {

                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(CheckStatus);

            }
            function InitializeRequest(sender, args) {
                //
                posicion_update_pogres('progres_bar');
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
                    if (elment_postbak.type == "button" || elment_postbak.type == "submit") {
                        elment_postbak.value = value_element;
                        elment_postbak.disabled = false;
                    }
                    
                    
                }
                catch (err) {
                    alert(" Funcion CheckStatus asincrona WebForm_indice_expediente.aspx" + err.message);
                }
                finally {
                    progres_hiden('progres_bar');
                }
            }
            </script>
        <div id="div_contendor_principal">
            <nav id="navar_barra" class="navbar navbar-expand-sm nav_botota_person modal_content_no_back_inferior" >
                <button class="navbar-toggler" type="button" style="background-color: #6d7fcc" data-toggle="collapse" data-target="#navbarNavDropdown">
                    <span class="navbar-toggler-icon_"><i style="color: white" class="fad fa-th-list"></i></span>
                </button>
                <div class="collapse navbar-collapse row" id="navbarNavDropdown">
                    <ul class="navbar-nav col-md-8">
                        <li class="nav-item dropdown active ml-2 active_">
                            <a class="nav-link dropdown-toggle bot_hover_person" style="color: #6d7fcc" href="#" id="navbarDropdownMenuLink" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc" class="fad fa-line-columns"></i> Detalle
                            </a>
                            <div class="dropdown-menu " aria-labelledby="navbarDropdownMenuLink"> 
                                <a class="dropdown-item" href="#" onclick="activa_boton_client_server('Button_descarga_listado');"> Descarga lista indice expediente</a>
                                <a class="dropdown-item" href="#" onclick="activa_boton_client_server('Button_descargar_archivo');"> Descarga archivo xml indice expediente</a>        
                            </div>
                        </li>
                        <li class="nav-item active ml-2 active_">
                            <a class="nav-link" style="color: #6d7fcc" title="Actualizar lista indice expediente" href="#" onclick="activa_boton_client_server('Button_actualiza_lista_indice');"><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-sync-alt"></i> Actualizar  </a>
                        </li>
                    </ul>

                    <div class=" float-md-right col-md-4 float-sm-left">
                        <div class="input-group " style="display:none">
                            <button id="td-boton" class="btn btn-outline-secondary border-right-2 " style="border-top-right-radius: 0px; border-bottom-right-radius: 0px" onclick="activa_boton_client_server('ImageButtonactualizar');" type="button">
                                <i class="fal fa-long-arrow-left"></i>
                            </button>
                            <asp:TextBox ID="auto_complex" runat="server" class="form-control form-control-sm complex " placeholder="Busqueda...." onkeypress="preven_event_search_keypres_enter(event,this);"></asp:TextBox>
                            <div class="input-group-append">
                                <button class="btn btn-outline-secondary" onclick="preven_event_search(event,this)" type="button">
                                    <i class="fal fa-search"></i>
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </nav>
            <div id="Contenedorgrid_listado_solicitud" style="width: auto;   margin-right: 10px; margin-left: 10px; height:auto">
                <asp:UpdatePanel ID="UpdateGeneral" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <input id="hdnEmailID" type="hidden" value="0" runat="server"/>      
                            <asp:Panel ID="Panel_principal" runat="server" ScrollBars="Auto"
                                Width="100%" Style="">
                                <asp:GridView ID="data_grid_listado_solicitudes" runat="server"   EnableViewState="true"
                                    PageSize="5" PagerSettings-Position="Top" Style="width: 100%; font-family: Segoe UI"
                                    AutoGenerateSelectButton="False" CssClass="table font-weight-light" GridLines="None" Font-Size="14px">
                                    <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                    <RowStyle CssClass="" />
                                    <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
                                          <HeaderStyle CssClass="GridviewScrollHeader_line_boot" BorderStyle="None" />
                                          <RowStyle CssClass="GridviewScrollItem_line_cort" />
                                          
                                    <Columns>
                                        <asp:BoundField HeaderText="OPCIONES" />
                                    </Columns>
                                </asp:GridView>
                            </asp:Panel>
                        
                    </ContentTemplate>
                    <Triggers>
                    </Triggers>
                </asp:UpdatePanel>
                <asp:UpdatePanel ID="UpdatePanel_title" runat="server" UpdateMode="Conditional" >
                    <ContentTemplate>
                        <div id="contenido_titulo_listado_solicitudes" style="width: 100%; position: inherit" class="border_inferior_radius_" >
                            <asp:Label ID="Label_estado" runat="server" ForeColor="Black" Font-Size="9px" Style="float: left"></asp:Label>
                            <asp:Label ID="Label_titulo_listado_solicitudes" runat="server" ForeColor="Black" Font-Size="12px" Style="float: left">Resultados busqueda</asp:Label>
                            <asp:Label ID="Label_titulo_listado" runat="server" Text="Gestión de tramites y respuestas" Style="font-family: Arial; font-size: 12px; float: right"></asp:Label>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div id="contenedor_opciones_solictitud_general" style="width: 100%; text-align: left; font-family: Arial; background-color: #E7EDF5; margin-top: 1px; border-color: #b0c4de; border-style: ridge; border-width: 1px; display: none">
                <asp:UpdatePanel ID="update_botonoes_opciones_solicitud_general" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <asp:Button ID="Button_descargar_archivo" runat="server" Text="" CssClass=""  />
                        <asp:Button ID="Button_descarga_listado" runat="server" Text="" CssClass=""  OnClientClick="activa_export_lista('Hidden_colum_header','data_grid_listado_solicitudes')"/>
                        <asp:Button ID="Button_actualiza_lista_indice" runat="server" Text="" CssClass=" btn btn-success"  />           
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
         <!--mensaje_progreso evento-->
        <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 200px">
            <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
            Processing ...
        </div>
         <div style="display: none">
            <asp:UpdatePanel ID="updatapanel_iframe" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                     <input id="Hidden_ruta_archivo" type="hidden" value="" runat="server"/>
                    <input id="Hidden_colum_header" type="hidden" value="" runat="server"/>
                    <iframe runat="server" style="float: left" id="ifmExcel_" width="0" height="0" marginheight="0" marginwidth="0"
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
