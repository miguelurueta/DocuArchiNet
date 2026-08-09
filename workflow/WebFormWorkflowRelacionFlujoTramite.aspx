<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormWorkflowRelacionFlujoTramite.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormWorkflowRelacionFlujoTramite" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">

<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
 
    <title>Administración clasificación</title>
     <script src="../js/ui/jquery-3.4.1.min.js"></script>  
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
      <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
     <link href="../ccs/themes/base/jquery.ui.all.css" rel="stylesheet" type="text/css" />
     <link href="../Styles/styleMenu.css" rel="stylesheet" type="text/css" /> 
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <link href="../Styles/Menu3.css" rel="stylesheet" />
     <script src="../js/ScrollableGrid.js" type="text/javascript"></script>
     <script src="../js/ScrollableGridViewPlugin_ASP.NetAJAXmin.js" type="text/javascript"></script>
    <script src="../Fixed-Header-Table-master/gridviewScroll.min.js"></script>  
    <script src="../js/workflow/WebFormWorkflowRelacionFlujoTramite.js"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet">
    <link href="../Awesome/css/brands.css" rel="stylesheet">
    <link href="../Awesome/css/solid.css" rel="stylesheet">
    <script defer src="../Awesome/js/all.js"></script>
    <script defer src="../Awesome/js/brands.js"></script>
    <script defer src="../Awesome/js/solid.js"></script>
    <script defer src="../Awesome/js/fontawesome.js"></script>
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
                if (elment_postbak.id == "Button_activa_relacion") {
                    auto_zise_popup_lista_tareas("1");
                }
                if (elment_postbak.id == "Button_buscar_lista") {
                    //busqueda_gred('hdnEmailID', 'data_grid', 'TextBox_busqueda', 'CheckBox_busqueda');
                }


            }

            </script>
       
            <div id="div_busqueda" style="width: 100%; display:none" class="mb-2">
                <asp:UpdatePanel ID="Update_busqueda" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="TextBox_busqueda_tre" runat="server" Style="width: 100%" CssClass="form-control" placeholder="Busqueda.."></asp:TextBox>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div id="div_treview" style="height: 96%; background-color: white; width: 100%;   border-top:none; overflow:auto" class="modal-body_">
                <asp:UpdatePanel ID="update_tre_principal" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TreeView ID="TreeView_lista_flujos" Style="text-align: left" runat="server" CssClass="TreeN" BackColor="white" NodeWrap="true"
                            PopulateNodesFromClient="False" 
                            LeafNodeStyle-CssClass="LeafNodeStyle" ForeColor="Black" HorizontalPadding="10px" Font-Size="12px" NodeIndent="5" ExpandDepth="0" SkipLinkText="">
                            <HoverNodeStyle Font-Underline="False" />
                            <LeafNodeStyle CssClass="LeafNodeStyle" HorizontalPadding="0px" NodeSpacing="0px" VerticalPadding="0px" />
                            <NodeStyle  HorizontalPadding="10px" NodeSpacing="5px" VerticalPadding="5px" ForeColor="Black"  CssClass="nav-link-treview mt-2 mb-2 pl-2" />
                            <ParentNodeStyle ChildNodesPadding="0px" CssClass="ParentNodeStyle" HorizontalPadding="5px" NodeSpacing="0px" VerticalPadding="0px" Font-Bold="true" />
                            <RootNodeStyle ChildNodesPadding="0px" CssClass="RootNodeStyle" NodeSpacing="0px" VerticalPadding="1px" HorizontalPadding="10px" Font-Bold="true" />
                            <SelectedNodeStyle  HorizontalPadding="10px" CssClass="select_treview_boottra_ajustado  nav-link-treview"  ImageUrl="~/workflow/imageneswf/iten_list_select.png" />
                        </asp:TreeView>
                        <asp:Button ID="Button_activa_busqueda_treview" runat="server" Text="Button" style="display:none" />
                        <input id="Hidden_texto_buequeda" type="hidden" value="" runat="server"/>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div id="div_botones" style="width: 100%;" class=" modal_content_back_no_radio modal-footer justify-content-end">
                <asp:UpdatePanel ID="UpdatePanel_botones" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Button ID="Button_activa_relacion" runat="server" Text="Relacionar" ToolTip="Relaciona trámite a flujo de trabajo seleccionado" CssClass="btn  btn-success"  />
                         <asp:Button ID="Button_activa_eliminar" runat="server" Text="Eliminar" CssClass="btn  btn-success" ToolTip="Eliminar relación de un trámite con el flujo de trabajo" />
                        <asp:Label ID="Label_estado" runat="server" Text="" Style="font-size: 10px; float: right; color: red"></asp:Label>
                    </ContentTemplate>
                </asp:UpdatePanel>

            </div>
        
        <div style="display:none">
            <asp:UpdatePanel ID="UpdatePanel_busqueda" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Button ID="Button_busqueda" runat="server" Text="" ToolTip="" Style="display: none" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <!--lista_tramites-->
          <div id="lista_tramites">
            <asp:Panel ID="Panel_lista_tramites" runat="server" Style="display:none; color:black; width: 70%; height:auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_lista_tramites" runat="server" BehaviorID="Panel_lista_tramites" TargetControlID="ButtonSalir_lista_tramites" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_lista_tramites" PopupControlID="Panel_lista_tramites"  ></asp:ModalPopupExtender>
                <div id="modal_content_Panel_lista_tramites" class="modal-content">
                <div id="divcabecer2_lista_tramites"  class="modal_title_superior_ modal-header"> 
                    <h6 class="modal-title d-inline ml-1">Lista trámites para relación</h6>
                    <button type="button" value="Button_cerrar_lista_tramites" class="close da_event_captive">&times;</button>                   
                </div>
                <div id="contenido_procesa_lista_actividades_workflow" style="background-color: white; width: 100%; height: 99%; color: black; background-color: #FFFFFF" class="modal-body">                   
                    <asp:UpdatePanel ID="UpdateGeneral_documentos" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div id="contenido_titulo_data_grid_dos_title" style="width: 100%" class="row">
                                <div class="col-5">
                                     <asp:Label ID="titulo_label_grid" runat="server"  Font-Size="12px" CssClass="h6">Resultados busqueda</asp:Label>
                                </div>
                                <div class="col-7">
                                    <div class="input-group ">
                                        <button id="td-boton" class="btn btn-outline-secondary border-right-2 " style="border-top-right-radius: 0px; border-bottom-right-radius: 0px" title="Actualizar lista de tareas" onclick="preven_event_restor_search(event,this)" type="button">
                                            <i class="fal fa-sync-alt"></i>
                                        </button>
                                        <asp:TextBox ID="auto_complex" runat="server" class="form-control form-control-sm complex " placeholder="Busqueda...."></asp:TextBox>
                                        <div class="input-group-append">
                                            <button class="btn btn-outline-secondary" onclick="preven_event_search(event,this)" title="consultar lista" type="button">
                                                <i class="fal fa-search"></i>
                                            </button>
                                        </div>
                                    </div>
                                </div>            
                            </div>
                            <input id="hdnEmailID" type="hidden" value="0" runat="server">
                            <input id="HiddenEstado" type="hidden" value="1" runat="server">
                                <asp:Panel ID="panel_conten_gred" runat="server" Wrap="False"
                                    Width="100%" style="overflow:auto" CssClass="mt-1">
                                    <asp:GridView ID="data_grid" runat="server" Style="position:inherit"
                                        AutoGenerateSelectButton="False" CssClass="filtrar table" GridLines="None" Font-Size="12px">
                                        <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                        <HeaderStyle CssClass="GridviewScrollHeader_line_boot" />
                                        <Columns>
                                            <asp:BoundField HeaderText="OPCIONES" />
                                        </Columns>
                                    </asp:GridView>
                                </asp:Panel>           
                        </ContentTemplate>
                        <Triggers>
                        </Triggers>
                    </asp:UpdatePanel>
                    <div id="div_contenido_procesa_lista_tramites_botones_desicion" style="width: auto; margin-top: 1px; border-color: #b0c4de; border-width: 1px; border-style: ridge; position: relative; display:none">
                        <asp:UpdatePanel ID="UpdatePanel_contendor_botones_desicion" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Button ID="Button_relaciona_tramite_flujo" runat="server" Text="Relacionar tramite flujo" CssClass="boton_azul" Style="margin-left: 0px; display:none" />               
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
                </div>
                <div style="display: none; height: 0px">
                    <asp:Button ID="Button_lista_tramites" CssClass="invisible" runat="server" Text="Button" Height="0px" Width="0px" />
                    <asp:Button ID="ButtonSalir_lista_tramites" CssClass="invisible" runat="server" Text="Button" Height="0px" Width="0px" />
                    <asp:Button ID="Button_cerrar_lista_tramites" runat="Server" Text="X" CssClass="modal_boton_hiden" />
                </div>
            </asp:Panel>
        </div>
        <!--confirma_eliminar-->
          <div id="confirma_eliminar">
            <asp:Panel ID="Panel_confirma_eliminar" runat="server" Style="display:none; color:black; width:40%; height: auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_confirma_eliminar" runat="server"  TargetControlID="ButtonSalir_confirma_eliminar" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_confirma_eliminar" PopupControlID="Panel_confirma_eliminar" ></asp:ModalPopupExtender>
                <div id="modal_content_Panel_confirma_eliminar" class="modal-content">
                    <div id="divcabecer2_confirma_eliminar" class="modal_title_superior_ modal-header">
                         <h6 class="modal-title d-inline ml-1"></h6>
                         <button type="button" value="Button_cerrar_confirma_eliminar" class="close da_event_captive">&times;</button>                  
                    </div>
                    <div id="contenido_procesa_confirma_eliminar" style="background-color: white; width: 100%; height: 99%; color: black; background-color: #FFFFFF; border-top:none; overflow:auto" class="modal_content_back modal-footer">
                         <div class="row w-100 p-2">
                             <div style="text-align: center">              
                                 <asp:Label ID="Label_title_comfirma_eliminar" runat="server" Text="Desea eliminar la relación con el tramite ?" CssClass="h6"></asp:Label>
                             </div>              
                         </div>
                       

                    </div>
                     <div class="modal-footer justify-content-end" id="modal-footer_Panel_confirma_eliminar">  
                           <asp:UpdatePanel ID="UpdatePanel_confirma_eliminar" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <input id="Hidden_estado_eliminar" type="hidden" value="" runat="server">
                                    <asp:Button ID="Button_aceptar_confirmacion" runat="server" Text="Aceptar" CssClass="btn btn-success" />  
                                    <asp:Button ID="Button_cancelar_confirmacion" runat="server" Text="Cancelar" CssClass="btn btn-light" />     
                            </ContentTemplate>
                        </asp:UpdatePanel>
                     </div>
                </div>
                <div style="display:none; height:1px">
                 <asp:Button ID="Button_confirma_eliminar" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                 <asp:Button ID="ButtonSalir_confirma_eliminar" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                 <asp:Button ID="Button_cerrar_confirma_eliminar" runat="Server" Text="" Height="0px" Width="0px" CssClass="invisible"/>
                </div>
            </asp:Panel>
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
