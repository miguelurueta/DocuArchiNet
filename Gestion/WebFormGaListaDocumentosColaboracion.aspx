<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormGaListaDocumentosColaboracion.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormGaListaDocumentosColaboracion" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Documentos de colaboración</title>
     <link href="../ccs/themes/base/jquery.ui.all.css" rel="stylesheet" type="text/css" />
    <link href="../Styles/styleMenu.css" rel="stylesheet" type="text/css" /> 
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <link href="../Styles/Menu3.css" rel="stylesheet" />
     <script src="../js/ui/jquery-3.4.1.min.js"></script>
    <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
    <script src="../js/Filtrar.js"></script>
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <script src="../js/gestion/WebFormGaListaDocumentosColaboracion.js"></script>
    <script defer src="../Awesome/js/all.js"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet">
  <link href="../Awesome/css/brands.css" rel="stylesheet">
  <link href="../Awesome/css/solid.css" rel="stylesheet">
    <script defer src="../Awesome/js/brands.js"></script>
  <script defer src="../Awesome/js/solid.js"></script>
  <script defer src="../Awesome/js/fontawesome.js"></script>
</head>
<body style="margin-top:0px">
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
                progres_hiden('progres_bar');
                //$("#Menu1").show();
                if (elment_postbak.type == "button" || elment_postbak.type == "submit") {
                    elment_postbak.value = value_element;
                    elment_postbak.disabled = false;
                }

                if (elment_postbak.id == "Button_ver_documentos_relacionados") {
                    //auto_zise_popup_compartir_documento();
                }
                if (elment_postbak.id == "Button_lista_filtro") {
                    Lista_tareas_estados("data_grid_listado_solicitudes", "id_estado", "Red");
                    Lista_tareas_lectura("data_grid_listado_solicitudes", "id_estado_visto", "700");
                }

                if (elment_postbak.id == "Button_lista_filtro") {
                    auto_zise_popup_lista_solicitudes("1", "");
                }
                if (elment_postbak.id == "ImageButton_buscar") {
                       auto_zise_popup_lista_solicitudes("1", "");
                 }
                if (elment_postbak.id == "Button_ver_documentos_relacionados") {
                    if (document.getElementById("Hidden_resultado_ver_documento").value == "YES") {
                        actualiza_estado_boton_seleccion("data_grid_listado_solicitudes", "hdnEmailID", "100", "1");
                        document.getElementById("Hidden_resultado_ver_documento").value == "";
                    }
                }
                

            }

            </script>
    
    <div id="div_contendor_principal" class="container-fluid">
            <div id="div_titulo_listado" style="width: 100%; position: inherit; margin-top: 1px" class="navbar navbar-expand-sm nav_botota_person modal_content_no_back_inferior_">         
                   <button class="navbar-toggler" type="button" style="background-color: #6d7fcc" data-toggle="collapse" data-target="#navbarNavDropdown">
                      <span class="navbar-toggler-icon_"><i style="color: white" class="fad fa-th-list"></i></span>
                  </button>
                  <div class="collapse navbar-collapse row" id="navbarNavDropdown">
                      <div class="navbar-nav col-md-8">
                          <a href="#" class="navbar-brand ml-2" style="color: #0062cc">Registros de colaboración </a>
                      </div>
                      <div class=" float-md-right col-md-4 float-sm-left">
                          <div class="input-group ">
                              <asp:TextBox ID="TextBox_busqueda" runat="server" class="form-control form-control-sm complex " placeholder="Busqueda...." onkeypress="preven_event_search_keypres_enter(event,this);"></asp:TextBox>
                              <div class="input-group-append">
                                  <button class="btn btn-outline-secondary da_event_captive" id="boton_search" value="ImageButton_buscar" onclick="preven_event_search(event,this)" type="button">
                                      <i class="fal fa-search"></i>
                                  </button>
                              </div>
                          </div>
                      </div>
                  </div>                
            </div>     
              
            <div id="Contenedorgrid_listado_solicitud" style="width: 100%; position: inherit" class="mt-2">           
                <asp:UpdatePanel ID="UpdateGeneral" runat="server" UpdateMode="Conditional" >
                    <ContentTemplate>
                        <input id="hdnEmailID" type="hidden" value="0" runat="server">
                        <input id="hdnEmailID_VAL" type="hidden" value="0" runat="server">
                        <input id="HiddenEmailconsulta" type="hidden" value="" runat="server">
                        <input id="Hidden_control_lista" type="hidden" value="" runat="server"> 
                        <div id="content_grid" class="border_general_blanco_" style="width: 100%; overflow:auto">
                                <asp:GridView ID="data_grid_listado_solicitudes" runat="server" Style="position: inherit; width: 100%; font-family: Segoe UI" PageSize="7"
                                    PagerSettings-Position="Top" Font-Names="arial" AutoGenerateSelectButton="False"
                                    CssClass="filtrar table font-weight-light" GridLines="None" Font-Size="14px">
                                    <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                    <HeaderStyle CssClass="GridviewScrollHeader_line_boot" BorderStyle="None" />
                                    <PagerStyle CssClass="pagination-ys" />
                                    <Columns>
                                        <asp:BoundField HeaderText="OPCIONES   " />
                                    </Columns>
                                </asp:GridView>
                            
                        </div>                             
                    </ContentTemplate>

                    <Triggers>
                    </Triggers>
                </asp:UpdatePanel>
                 
            </div>
        <div id="contenido_titulo_listado_solicitudes" style="width: 100%" class="border_inferior_radius_ mt-2">
            <asp:UpdatePanel ID="UpdatePanel_title" runat="server" UpdateMode="Conditional" >
                <ContentTemplate>
                    <asp:Label ID="Label_estado" runat="server" CssClass="h6 font-weight-light" Style="font-family: 'Segoe UI Emoji'; font-size: 14px; float: left"></asp:Label>
                    <asp:Label ID="Label_titulo_listado_solicitudes" runat="server" CssClass="h6 font-weight-light" Style="font-family: Segoe UI; font-size: 14px; float: left">Resultados busqueda</asp:Label>
                    <asp:Label ID="Label_titulo_listado" runat="server" Text="Registros de colaboración" CssClass="h6 font-weight-light" Style="font-family: Segoe UI; font-size: 14px; float: right"></asp:Label>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
            <div id="contenedor_opciones_solictitud_general" style="width: 100%; text-align: left; font-family: Arial; background-color:white;margin-top: 1px; display:none">
                <asp:UpdatePanel ID="update_botonoes_opciones_solicitud_general" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <input id="Hidden_ruta_archivo" type="hidden" value="" runat="server">
                        <input id="Hidden_solicitud_compartido" type="hidden" value="-1" runat="server">
                        <input id="Hidden_result_eliminar" type="hidden" value="" runat="server">
                        <input id="Hidden_resultado_ver_documento" type="hidden" value="" runat="server">
                        <asp:Button ID="Button_lista_filtro" runat="server" Text="Gestionar solicitud"  CssClass="boton" Style="display:none" />
                        <asp:Button ID="Button_ver_documentos_relacionados" runat="server" Text="Ver documentos de colaboración" ToolTip="Lista los documentos de colaboración relacionados al registro"  CssClass="boton_azul" Style="float:right; margin-top:2px"  />
                        <asp:Button ID="Button_ver_nota_colaboracion" runat="server" Text="Ver nota colaboracion" ToolTip="Muestra el contenido de la nota de colaboración"  CssClass="boton_azul" Style="float:right; margin-top:2px" />
                    </ContentTemplate>
                </asp:UpdatePanel>

            </div>
        <div style="display:none">
                 <asp:UpdatePanel ID="UpdatePanel_busqueda" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                      <ContentTemplate>                 
                          <asp:ImageButton ID="ImageButton_buscar" runat="server" Style="margin-top: 4px; float: right; margin-right: 4px; height: 20px;" ImageUrl="../radicador/imagenes/cbxs0-vnnbp.png" />
                      </ContentTemplate>
                  </asp:UpdatePanel>  
            </div> 
        </div>      
      
    
        <!--lista_documentos_colaboracion-->
        <div id="lista_documentos_colaboracion">
            <asp:Panel ID="Panel_lista_documentos_colaboracion" runat="server" Style="display:none; color: White; width: 50%; height: 400px" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_lista_documentos_colaboracion" runat="server" BehaviorID="Panel_lista_documentos_colaboracion" TargetControlID="ButtonSalir_lista_documentos_colaboracion" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_lista_documentos_colaboracion" PopupControlID="Panel_lista_documentos_colaboracion">
                </asp:ModalPopupExtender>
                <div id="divcabecer2_lista_documentos_colaboracion" class="modal_title_superior">
                    <asp:Label ID="Label_lista_documentos_colaboracion" runat="server" Text="Documentos relacionados" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton2_lista_documentos_colaboracion" style="float: right">
                        <asp:Button ID="Button_cerrar_lista_documentos_colaboracion" runat="Server" Text="X" CssClass="modal_boton_hiden"
                             ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_lista_documentos_colaboracion" style="background-color: white; width: 100%; height: 380px; overflow: auto" class="modal_content_back">
                    <asp:UpdatePanel ID="UpdatePanel_seleccion_documento" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Panel ID="Panel_seleccion_documento" runat="server" Style="overflow: auto" ScrollBars="Auto">
                                <asp:Table ID="Table_seleccion_documento" runat="server" Style="text-align: left; overflow: auto; height: 99%; width: 90%; font-size: 12px; margin-left: 5%; margin-right: 5%" EnableViewState="false"></asp:Table>
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div id="div_status_bar" style="height: 20px; color: black; text-align: left; font-family: Arial; background-color:white;margin-top: 1px; border-color: #b0c4de; border-style: ridge; border-width: 1px">
                    <asp:UpdatePanel ID="UpdatePanel_estado_doc_colaboracion" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Label ID="Label_estado_doc_colaboracion" runat="server" Text="Estado" Style="font-family: Arial; font-size: 11px"></asp:Label>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <asp:UpdatePanel ID="UpdatePanel_descraga_documento" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Button ID="Button_descarga_documento" runat="server" Text="Button" Style="display: none" />
                        <input id="Hidden_documento_descarga" type="hidden" value="" runat="server">
                    </ContentTemplate>
                </asp:UpdatePanel>
                <asp:Button ID="Button_lista_documentos_colaboracion" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                <asp:Button ID="ButtonSalir_lista_documentos_colaboracion" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
            </asp:Panel>
        </div>
        <!--nota_solicitud_colaboracion-->
          <div id="nota_solicitud_colaboracion">
            <asp:Panel ID="Panel_nota_solicitud_colaboracion" runat="server" Style="display:none; width:60%; height:auto" CssClass="modal_content_general_">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_nota_solicitud_colaboracion" runat="server" BehaviorID="Panel_nota_solicitud_colaboracion" TargetControlID="ButtonSalir_nota_solicitud_colaboracion" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_nota_solicitud_colaboracion" PopupControlID="Panel_nota_solicitud_colaboracion" ></asp:ModalPopupExtender>
                <div id="modal_content_nota_solicitud_colaboracion" class="modal-content">
                    <div id="divcabecer2_nota_solicitud_colaboracion" class="modal_title_superior_ modal-header ">
                         <h6 class="modal-title d-inline ">Nota de colaboración</h6>
                          <button type="button" value="Button_cerrar_nota_solicitud_colaboracion" class="close da_event_captive ">&times;</button>                      
                    </div>
                    <div id="contenido_procesa_nota_solicitud_colaboracion" style="background-color: white; width: 100%; height: 99%; border-top:none; overflow:auto" class="modal_content_back modal-body p-1">
                        <asp:UpdatePanel ID="UpdatePanel_nota_solicitud_colaboracion" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:TextBox ID="TextBox_nota_colaboracion" runat="server" TextMode="MultiLine" CssClass="" Style="width: 100%; height: 100%; border:none"></asp:TextBox>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div  class="modal-footer justify-content-end" id="content_boton_nota_solicitud_colaboracion">  
                        <button type="button" value="Button_cerrar_nota_solicitud_colaboracion" class="da_event_captive  btn btn-success"> Cancelar </button>
                    </div>
                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button_nota_solicitud_colaboracion" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                        <asp:Button ID="ButtonSalir_nota_solicitud_colaboracion" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                        <asp:Button ID="Button_cerrar_nota_solicitud_colaboracion" runat="Server" Text="" CssClass="invisible" />
                    </div>
                </div>
            </asp:Panel>
        </div>
          <div id="tol_pie" style=" float:right;  background-color:#E7EDF5; width:100%; height:3%;border-style: ridge; border-bottom-width: 0.5px; border-left-width: 1px; border-right-width: 1px; border-top-width: 1px;text-align:center; display:none">
                 <asp:Label ID="Label2" runat="server" Text="Estado" style="font-family:Arial;font-size:11px"></asp:Label>
                    <asp:UpdatePanel ID="updatapanel_iframe" runat="server" UpdateMode="Conditional" >
                        <ContentTemplate>
                            <iframe runat="server" style="float: left" id="ifmExcel_" width="0" height="0" marginheight="0" marginwidth="0"
                                frameborder="0" />
                        </ContentTemplate>
                           
                 </asp:UpdatePanel>
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
