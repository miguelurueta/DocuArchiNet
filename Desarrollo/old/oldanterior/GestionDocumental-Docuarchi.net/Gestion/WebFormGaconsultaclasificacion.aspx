<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormGaconsultaclasificacion.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormGaconsultaclasificacion" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Administración clasificación</title>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <script src="../js/ui/jquery-3.4.1.min.js"></script>  
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
     <link href="../ccs/themes/base/jquery.ui.all.css" rel="stylesheet" type="text/css" />
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
     <script defer src="../Awesome/js/all.js"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet">
     <link href="../Awesome/css/brands.css" rel="stylesheet">
     <link href="../Awesome/css/solid.css" rel="stylesheet">
     <script defer src="../Awesome/js/brands.js"></script>
     <script defer src="../Awesome/js/solid.js"></script>
     <script defer src="../Awesome/js/fontawesome.js"></script>   
      <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />  
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
      <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <script src="../js/gestion/WebFormGaconsultaclasificacion.js"></script>
    <script src="../js/java_general/general_code_java.js"></script>
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
                progres_hiden('progres_bar');
                //$("#Menu1").show();
                if (elment_postbak.type == "button" || elment_postbak.type == "submit") {
                    elment_postbak.value = value_element;
                    elment_postbak.disabled = false;
                }
                Auto_zise_marco_principal();
                if (elment_postbak.id == "TreeViewEstructura") {
                    if (document.getElementById("Hidden_result_selecion").value == "YES") {
                        document.getElementById("Hidden_result_selecion").value = "";
                        tab_sow("home-expediente");
                    }
                }
                //documentos_expediente   
                if (elment_postbak.id == "Button_ver_documentos_relacionados") {
                    if (document.getElementById("Hidden_estado_relacion").value == "YES") {
                        document.getElementById("Hidden_estado_relacion").value = "";
                        tab_sow("home_documentos_expediente");
                    }
                }
                if (elment_postbak.id == "Button_ver_documento") {
                    auto_zise_popup_visor_externo();
                }
               
            }

            </script>
        <div id="conte_waper" class="container-fluid_ mr-0 ml-0 pl-0 pr-0 d-flex" style="border-top: 1px solid #e9ecef">
            <a id="da_show-sidebar_" class="btn btn-sm   hide_da_sidebar " href="#" data-target="#sidebar">
                <i style="color: white" class="fas fa-bars"></i>
            </a>  
             <div id="sidebar_person" class=" bg-light_ pl-0 pr-0   modal_content_no_back_rigth" style="width: 30%; float:left">
                <div id="div_contenedor_izquierdo" style="width: 100%; height: 100%;  position: relative">
                    <div id="div_title_clasificacion" class="modal-header" style="border-top-left-radius: initial; border-top-right-radius: initial">
                        <h6 class="modal-title font-weight-normal" style="color:#0062cc">Cuadros de clasificación</h6>
                        <button id="sidebarCollapse" type="button" class="close" style="display:none">&times;</button>
                    </div>
                    <div id="div_cuadro_clasficacion" style="clear: both">
                        <asp:UpdatePanel ID="UpdatePanel_estructura_clasificacion" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:DropDownList ID="DropDownList_nivel_clasficacion" runat="server" Style="height: 100%; color:#0062cc" AutoPostBack="True" CssClass="custom-select"></asp:DropDownList>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div id="div_treview">
                        <asp:Panel ID="Paneltreview" runat="server" ScrollBars="Auto"
                            Height="100%" Width="100%" Style="margin-top: 10px">
                            <asp:UpdatePanel ID="update_tre_principal" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:TreeView ID="TreeViewEstructura" Style="text-align: left; padding-left: 0px; font-size: 12px; margin-top: 2px; font-family:'Segoe UI'" runat="server" CssClass="TreeN pl-0 pt-1"  NodeWrap="true"
                                        PopulateNodesFromClient="False" EnableViewState="true"
                                        LeafNodeStyle-CssClass="LeafNodeStyle_2_  mb-1 pl-1" Font-Size="12px" NodeIndent="5" ExpandDepth="0" SkipLinkText="">
                                        <HoverNodeStyle Font-Underline="False" />
                                        <LeafNodeStyle CssClass="LeafNodeStyle" HorizontalPadding="10px" NodeSpacing="0px" VerticalPadding="5px" />
                                        <NodeStyle ChildNodesPadding="5px" HorizontalPadding="0px" NodeSpacing="5px" VerticalPadding="5px"  CssClass="mt-2 mb-2 pl-2 font-weight-light" ForeColor="#0062cc" />
                                        <ParentNodeStyle ChildNodesPadding="0px" CssClass="ParentNodeStyle   font-weight-bold"  HorizontalPadding="5px" NodeSpacing="0px" VerticalPadding="5px" />
                                        <RootNodeStyle ChildNodesPadding="0px" CssClass="RootNodeStyle font-weight-bold"  NodeSpacing="0px" VerticalPadding="5px" HorizontalPadding="5px" />
                                        <SelectedNodeStyle CssClass="select_treview_boottra font-weight-normal nav-link-treview" ImageUrl="../workflow/imageneswf/iten_list_select.png" />
                                    </asp:TreeView>
                                    <input id="Hidden_id_cuadro" type="hidden" value="0" runat="server">
                                    <input id="Hidden_result_selecion" type="hidden" value="" runat="server">
                                </ContentTemplate>

                            </asp:UpdatePanel>
                        </asp:Panel>

                    </div>
                    <div id="pie_cuerpo_left" style="width: 100%; height: auto" class="modal-footer  justify-content-start">
                        <asp:Label ID="LabelEstado" runat="server" Text=" " CssClass="h6 font-weight-light" ForeColor="#0062cc"></asp:Label>
                    </div>
                </div>
            </div>
            <div id="div_contenedor_drecho" class=" mr-0 ml-0 pl-1 pr-1 pb-0 pt-0  " style="width: 70%; float:right">
                <div class="nav-person-da" id="item_nav_tab">
                    <ul class="nav nav-tabs mt-2" id="myTab" role="tablist">

                        <li class="nav-item" onclick="Auto_zise_marco_principal();">
                            <a class="nav-link nav-link-person " id="home-expediente" data-toggle="tab" href="#home_expediente" role="tab" aria-controls="home_radic" aria-selected="true"><i style="color: #0062cc" id="home-radicadori" class="fad fa-folders "></i> Expedientes</a>
                        </li>
                        <li class="nav-item" onclick="Auto_zise_marco_principal();">
                            <a class="nav-link nav-link-person " id="home_documentos_expediente" data-toggle="tab" href="#documentos_expediente" role="tab" aria-controls="profile" aria-selected="false"><i style="color: #0062cc" id="soporte-envio_navi" class="fad fa-copy"></i> Documentos </a>
                        </li>

                    </ul>
                </div>
                <div class="tab-content" id="item_tab_content">
                    <div class="tab-pane  p-2 active show" id="home_expediente" role="tabpanel" aria-labelledby="home-tab">
                        <div id="Contenedorgrid" style="width: 100%; position: inherit; left: 0px; height: auto">
                            <input id="hdnEmailID_VAL" type="hidden" value="0" runat="server"/>
                            <input id="HiddenEmailconsulta" type="hidden" value="" runat="server"/>
                            <div id="contenido_titulo_val_radicacion" style="width: auto" class="row">
                                <div class="col-6">
                                    <asp:UpdatePanel ID="UpdatePanel_title_expediente" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <asp:Label ID="titulo_label_expedientes" runat="server" ForeColor="#0062cc" CssClass="h6 font-weight-light"></asp:Label>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>
                                <div class="input-group  col-6 mb-2">
                                    <button id="Button_retaura_relacionados" class="btn btn-outline-secondary border-right-2 " title="Restaura expedientes relacionados" style="border-top-right-radius: 0px; border-bottom-right-radius: 0px" onclick="activa_restore_search_exp(event,this)" type="button">
                                        <i class="fal fa-long-arrow-left"></i>
                                    </button>
                                    <asp:TextBox ID="TextBox_busqueda" runat="server" class="form-control form-control-sm complex  border-left-0" placeholder="Buscar expedientes...." onkeypress=""></asp:TextBox>
                                    <div class="input-group-append">
                                        <button class="btn btn-outline-secondary" onclick="activa_boton_client(event, this)" type="button">
                                            <i class="fal fa-search"></i>
                                        </button>
                                    </div>
                                </div>
                            </div>
                            <asp:Panel ID="Panelactividad" runat="server" Wrap="False"
                                Height="90%" Style="width: 100%" ScrollBars="Auto">
                                <asp:UpdatePanel ID="UpdateGeneral" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:GridView ID="data_grid" runat="server" AllowSorting="true" AllowPaging="true" EnableViewState="true"
                                            PageSize="8" PagerSettings-Position="Top"
                                            AutoGenerateSelectButton="False" CssClass="filtrar table font-weight-light" GridLines="None" Font-Size="12px">
                                            <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                            <HeaderStyle CssClass="GridviewScrollHeader_line_boot" BorderStyle="None" />
                                            <RowStyle CssClass="GridviewScrollItem_line_cort" />
                                            <PagerStyle CssClass="pagination-ys" />
                                            <Columns>
                                                <asp:BoundField HeaderText="OPCIONES   " />
                                            </Columns>
                                        </asp:GridView>
                                        <div id="botones_accion_postback" style="display: none">
                                            <asp:Button ID="Button_actualiza_expdientes_agregados" runat="server" />
                                            <asp:Label ID="Label_estado" runat="server" ForeColor="#0062cc" Font-Size="9px" Style="display: none" CssClass="h6"></asp:Label>
                                        </div>
                                    </ContentTemplate>
                                    <Triggers>
                                    </Triggers>
                                </asp:UpdatePanel>
                            </asp:Panel>
                            <asp:UpdatePanel ID="UpdatePanel_documentos_exp_title" runat="server" UpdateMode="Conditional">
                                  <ContentTemplate>
                                         </ContentTemplate>      
                              </asp:UpdatePanel>
                        </div>
                    </div>
                    <div class="tab-pane  p-2" id="documentos_expediente" role="tabpanel" aria-labelledby="profile-tab">
                        <div id="Contenedorgrid_documentos" style="width: 100%; position: inherit; height: 40%">
                                    <input id="hdnEmailID_documentos" type="hidden" value="0" runat="server"/>
                                    <input id="hdnEmailID_VAL_documentos" type="hidden" value="0" runat="server"/>
                                    <input id="Hidden_gabienete" type="hidden" value="0" runat="server"/>
                                    <input id="HiddenEmailconsulta_documentos" type="hidden" value="" runat="server"/>
                                   <div id="contenido_titulo_documentos" style="width: auto" class="row">
                                        <div class="col-6">
                                            <asp:UpdatePanel ID="UpdatePanel_expediente_seleccionado" runat="server" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <asp:Label ID="Label_expediente_seleccionado" runat="server" Text="" ForeColor="#0062cc" CssClass="h6 font-weight-light"></asp:Label>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>   
                                        </div>
                                        <div class="input-group  col-6 mb-2">
                                            <button id="Button_clik" class="btn btn-outline-secondary border-right-2 " onclick="restore_activa_boton_client_documento(event, this)" title="Restaura documentos relacionados" style="border-top-right-radius: 0px; border-bottom-right-radius: 0px" onclick="" type="button">
                                                <i class="fal fa-long-arrow-left"></i>
                                            </button>
                                            <asp:TextBox ID="TextBox_busqueda_documento" runat="server" class="form-control form-control-sm complex  border-left-0" placeholder="Buscar documentos...." ></asp:TextBox>
                                            <div class="input-group-append">
                                                <button class="btn btn-outline-secondary" onclick="activa_boton_client_documento(event, this)" type="button">
                                                    <i class="fal fa-search"></i>
                                                </button>
                                            </div>
                                        </div>
                                    </div>
                              <asp:UpdatePanel ID="UpdateGeneral_documentos" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Panel ID="Panelactividad_documentos" runat="server" Wrap="False"
                                        ScrollBars="Auto" Height="90%"  Style="width:auto">
                                        <asp:GridView ID="data_grid_documentos" runat="server" AllowSorting="true" AllowPaging="true" EnableViewState="true"
                                            PageSize="7" PagerSettings-Position="Top"  Style="width: 100%"
                                            AutoGenerateSelectButton="False" CssClass="filtrar table font-weight-light" GridLines="None" Font-Size="12px">
                                            <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                            <HeaderStyle CssClass="GridviewScrollHeader_line_boot" BorderStyle="None" />
                                            <RowStyle CssClass="GridviewScrollItem_line_cort" />
                                            <PagerStyle CssClass="pagination-ys" />
                                            <Columns>
                                                <asp:BoundField HeaderText="OPCIONES   " />
                                            </Columns>

                                        </asp:GridView>
                                    </asp:Panel>
                                    <div id="contenido_titulo_val_radicacion_documentos" style="width: 100%"  class="p-1">
                                        <asp:Label ID="titulo_label_expedientes_documentos" runat="server" ForeColor="#0062cc"  Style="float: left" CssClass="h6 font-weight-light"></asp:Label>
                                    </div>
                                </ContentTemplate>

                                <Triggers>
                                </Triggers>
                            </asp:UpdatePanel>

                        </div>
                        <div style="display: none">
                            <asp:UpdatePanel ID="UpdatePanel_boton_documento" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Button ID="Button_ver_documento" runat="server" />
                                    <asp:Button ID="Button_busqueda_documento" runat="server" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                        <div id="div_expediente_seleccionado" style="width: 100%; position: inherit; left: auto; height: 10%; font-family: Arial; font-size: 11px; display: none">
                          

                        </div>
                    </div>
                </div>
            </div>
          
        </div>
          <div id="Div1" style="display: none">
                <asp:UpdatePanel ID="UpdatePanel_boton_Event" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <input id="hdnEmailID" type="hidden" value="0" runat="server"/>
                        <input id="Hidden_estado_relacion" type="hidden" value="" runat="server"/>
                        <asp:Button ID="Button_ver_documentos_relacionados" runat="server" />
                        <asp:Button ID="Button_busqueda_expediente" runat="server" />
                         <asp:Button ID="Button_restore_busqueda_expediente" runat="server" />
                    </ContentTemplate>
                </asp:UpdatePanel>

            </div>
        <!--Popup visor externo-->
               <asp:Panel ID="Panel_visor_externo" runat="server" Style="display:none; clear:both" ForeColor="White" Width="95%" Height="100%" CssClass="modal_content_general">
                  <asp:ModalPopupExtender ID="ModalPopupExtender_visor_externo" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_visor_externo"
                      PopupControlID="Panel_visor_externo"  CancelControlID="ButtonSalir_visor_externo">
                  </asp:ModalPopupExtender>
                   <div id="modal_content_Panel_visor_externo" class="modal-content">
                       <div id="Cabecerapendiente_visor_externo" class="modal_title_superior_ modal-header">
                              <h6 class="modal-title d-inline ml-1"></h6>
                              <button type="button" value="ButtonSalir_visor_externo" class="close da_event_captive">&times;</button>     
                       </div>
                       <div id="Cotenedorpendiente_visor_externo" style="background-color: white; width: 100%; height: 99% ; border-top:none; overflow:hidden" class="modal_content_back modal-body">
                           <asp:UpdatePanel ID="UpdatePanel_visor_externo" runat="server" UpdateMode="Conditional">
                               <ContentTemplate>
                                   <iframe id="Iframe_visor_externo__" runat="server" frameborder="0" style="width: 100%; height: 100%; overflow: hidden"></iframe>
                               </ContentTemplate>
                           </asp:UpdatePanel>

                       </div>
                   </div>
                   <div style="display:none; height:1px">
                       <asp:Button ID="Button_visor_externo" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                       <asp:Button ID="ButtonSalir_visor_externo" runat="Server" Text="" Height="0px" Width="0px"/>
                   </div>
                  
              </asp:Panel>
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
      <script type="text/javascript">
          $(document).ready(function () {
              $('#sidebarCollapse').on('click', function () {
                  $('#sidebar_person').toggleClass('active_da_slider');
                  $(this).toggleClass('active_da_slider');
                  $('#da_show-sidebar_').toggleClass('show_da_slide');
                  $('#da_show-sidebar_').toggleClass('hide_da_sidebar');
                  $('#div_contenedor_drecho').css("width", "70%");
              });
              $('#da_show-sidebar_').on('click', function () {
                  $('#sidebar_person').toggleClass('active_da_slider');
                  $(this).toggleClass('show_da_slide');
                  $(this).toggleClass('hide_da_sidebar');
                  $('#div_contenedor_drecho').css("width", "100%");
              });
          });
    </script>
</body>
</html>
