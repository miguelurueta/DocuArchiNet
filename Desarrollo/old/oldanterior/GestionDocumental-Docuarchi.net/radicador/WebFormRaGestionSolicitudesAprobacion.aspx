<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormRaGestionSolicitudesAprobacion.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormRaGestionSolicitudesAprobacion" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">

<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
     <link href="../ccs/themes/base/jquery.ui.all.css" rel="stylesheet" type="text/css" />
<link href="../Styles/styleMenu.css" rel="stylesheet" type="text/css" /> 
 <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
<link href="../Styles/Menu3.css" rel="stylesheet" />
    <title>Gestión aprobación</title>
     <script src="../js/ui/jquery-3.4.1.min.js"></script>
      <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <link href="../tokenzize2/tokenize2.min.css" rel="stylesheet" />
    <script src="../tokenzize2/tokenize2.1.min.js"></script>
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
    <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
        <script src="../js/ScrollableGridViewPlugin_ASP.NetAJAXmin.js" type="text/javascript"></script>
     <script src="../Fixed-Header-Table-master/gridviewScroll.min.js"></script>   
      <script  src="../Awesome/js/all.js"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet"/>
  <link href="../Awesome/css/brands.css" rel="stylesheet"/>
  <link href="../Awesome/css/solid.css" rel="stylesheet"/>
    <script  src="../Awesome/js/brands.js"></script>
  <script  src="../Awesome/js/solid.js"></script>
  <script  src="../Awesome/js/fontawesome.js"></script>
    <script src="../js/Filtrar.js"></script>
    <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
      <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <link href="../Styles/tumb.css" rel="stylesheet" />  
    <link href="../js/jquery-ui-1.12.1.custom/style.css" rel="stylesheet" />   
    <script src="../js/radicacion/WebFormRaGestionSolicitudesAprobacion.js"></script>
</head>
<body style="margin:0px">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="True" AsyncPostBackTimeout="900">
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
                    progres_hiden('progres_bar');
                    //auto_zise_popup_lista_solicitudes();
                    //$("#Menu1").show();
                    if (elment_postbak.type == "button" || elment_postbak.type == "submit") {
                        elment_postbak.value = value_element;
                        elment_postbak.disabled = false;
                    }
                    if (elment_postbak.id == "Button_activa_usuario_relacion") {
                        auto_zise_popup_lista_usuarios();
                    }
                
                    if (elment_postbak.id == "Buttonbuscar") {
                        activa_busqueda();
                        //busqueda_gred('hdnEmailID', 'GridViewlista', 'contenidobusqueda', 'CheckboxBusqueda');
                    }
                    if (elment_postbak.id == "Button_listar_usuarios_relacionados_solicitud")
                    {
                        auto_zise_popup_usuarios_relacionados();
                    }
                
                    if (elment_postbak.id == "Button_registrar_solicitud_aprobacion") {
                        if (document.getElementById("HiddenEmailconsulta").value == "YES") {
                            document.getElementById("HiddenEmailconsulta").value = "YES";
                            auto_zise_popup_lista_solicitudes("1", "1");
                            ejecuta_boton_formulario_padre("Button_lista_filtro");
                           
                        }
                    
                    }
                    if (elment_postbak.id == "Button_registrar_solicitud_aprobacion_usuario") {
                        if (document.getElementById("Hidden_resultado_actualizar_usuario").value == "YES") {
                            document.getElementById("Hidden_resultado_actualizar_usuario").value = "";
                            auto_zise_popup_usuarios_relacionados();
                        }

                    }

                    //
                    if (elment_postbak.id == "Button_nula_solicitud_aprobacion") {
                        if (document.getElementById("Hidden_resultado_actualizar_usuario").value == "YES") {
                            document.getElementById("Hidden_resultado_actualizar_usuario").value = "";
                            //auto_zise_popup_lista_solicitudes("0", "1");
                            actualiza_gre_campo('data_grid', document.getElementById("hdnEmailID_documentos").value,'Anulado', 'ESTADO');
                        }

                    }
               
                    if (elment_postbak.id == "Button_archiva_solicitud") {
                        if (document.getElementById("Hidden_resultado_aprobacion").value == "YES") {
                            document.getElementById("Hidden_resultado_aprobacion").value == "";
                            if (document.getElementById("Hidden_actualizacion_general").value !== "") {
                                actualiza_gre_campo('data_grid', document.getElementById("hdnEmailID_documentos").value, document.getElementById("Hidden_actualizacion_general").value, 'ESTADO');
                                document.getElementById("Hidden_actualizacion_general").value = "";
                            }
                            if (document.getElementById("Hidden_actualizacion_usuario").value !== "") {
                                actualiza_gre_campo('data_grid_documentos', document.getElementById("Hidden_id_usuarios_sel").value, document.getElementById("Hidden_actualizacion_usuario").value, 'ESTADO');
                                document.getElementById("Hidden_actualizacion_usuario").value = "";
                            }
                        }
                    }
                } catch (e) { alert("Funcion CheckStatus " + e.message) }
            }
            </script>
        <div>
            <div id="div_contenedor_drecho" style="left: 0px; width: 100%; height: 100%; position: relative; margin-top: 1px; top: 0px;" class="container-fluid" >
                <div id="div_unidades_documentales" style="width: 100%; position: inherit;  text-align:left;  text-align:center; top: 0px; left: 0px;">
                    <asp:Label CssClass="h6" ID="Label_solicitudes_relacionadas" runat="server" Text="Solicitudes de aprobación" Style=" text-align:center"></asp:Label>
                </div>
                <div id="Contenedorgrid" style="width: 100%" >
                    <asp:UpdatePanel ID="UpdateGeneral" runat="server" UpdateMode="Conditional"  RenderMode="Inline">
                        <ContentTemplate>
                            <input id="hdnEmailID" type="hidden" value="0" runat="server">
                            <input id="hdnEmailID_VAL" type="hidden" value="0" runat="server">
                            <input id="HiddenEmailconsulta" type="hidden" value="" runat="server">
                            <input id="Hidden_id_respuesta" type="hidden" value="" runat="server">                  
                            <div id="contenido_titulo_val_radicacion" style="width: 100%;margin-top:1px"  >
                                <asp:Label ID="Label_estado" CssClass="h6" runat="server" ForeColor="Black" Font-Size="9px" ></asp:Label>
                                <asp:Label ID="Label3" runat="server" CssClass="h6"  style=" margin-left:5px; display:none">Relación solicitudes de aprobación</asp:Label> &nbsp
                                <asp:Label ID="titulo_label_expedientes" CssClass="h6" runat="server"   >Resultados busqueda</asp:Label>
                            </div>
                             <asp:Panel ID="Panel_principal" runat="server" Wrap="False"
                                  Width="100%" Style="overflow:auto">
                                <asp:GridView ID="data_grid" runat="server" Style="position: inherit; width:100%; font-family:Segoe UI" AutoGenerateSelectButton="False" 
                                                AllowPaging="true" PageSize="6" PagerSettings-Position="Top" EnableViewState="true"
                                                CssClass="table font-weight-light" GridLines="None" Font-Size="14px">
				                                <SelectedRowStyle BackColor="LightSkyBlue"   />
				                                <HeaderStyle CssClass="GridviewScrollHeader_line_boot" BorderStyle="None" />
                                                <RowStyle CssClass="" />
                                                <PagerStyle CssClass="pagination-ys" />
                                   
                                    <Columns>
                                        <asp:BoundField HeaderText="OPCIONES" />                               
                                    </Columns>
                                </asp:GridView>
                            </asp:Panel>
                            <div id="botones_accion_postback" style="display: none">
                                <asp:Button ID="Button_actualiza_expdientes_agregados" runat="server" />
                            </div>
                        </ContentTemplate>

                        <Triggers>
                        </Triggers>
                    </asp:UpdatePanel>

                </div>
                <div id="contenedor_opciones_solictitud_general" style="width: 100%;  text-align:left; font-family: Arial;margin-top:1px">
                    <asp:UpdatePanel ID="update_botonoes_opciones_solicitud_general" runat="server" UpdateMode="Conditional"   RenderMode="Inline"  >
                        <ContentTemplate>
                            <asp:Button ID="Button_activa_registro_solicitud" runat="server" Text="Nueva solicitud" ToolTip="Registra una nueva solicitud de aprobación para el documento de respuesta" CssClass="btn  btn-success" style="float:right; margin-right:5px; margin-top:7px" />     
                            <asp:Button ID="Button_envia_correo_notificacion" runat="server" Text="Notificar al correo electrónico" ToolTip="Notificar al correo electrónico a todos los usuarios de la solcitud de aprobación que usted seleccione en la lista superior" CssClass="boton_azul" style="display:none" />
                            <asp:Button ID="Button_activa_anulacion_solicitud" runat="server" Text="Anula solicitud" ToolTip="Anula la solicitud de aprobación de respuesta que usted seleccione en la lista superior" CssClass="boton_azul" style="display:none" />
                            <asp:Button ID="Button_activa_ver_nota_general" runat="server" Text="Ver todas las notas" ToolTip="Lista todas las notas relacionadas a la solicitud de aprobación de respuesta que usted seleccione en la lista superior" CssClass="boton_azul" style="display:none"  OnClientClick="auto_zise_popup_paginas_externas_libres();" /> &nbsp
                            <asp:Button ID="Button_todos_documentos_correccion" runat="server" Text="Ver todos los anexos de corrección" CssClass="boton_azul" ToolTip="Listar documentos de corrección relacionados a la solicitud de aprobación que usted seleccione en la lista superior" style="display:none" />
                            
                        </ContentTemplate>
                    </asp:UpdatePanel>
                   
                </div>
               
            </div>
        </div>
       
       <asp:Panel ID="Panel_usu_rel_solicitud" runat="server" Style="display:none;  width:70%; height:100%" CssClass="modal_content_general" >
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_usu_rel_solicitud" runat="server" BehaviorID="Panel_usu_rel_solicitud_ModalPopupExtender" 
                     TargetControlID="ButtonSalir_usu_rel_solicitud" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_usu_rel_solicitud" PopupControlID="Panel_usu_rel_solicitud" ></asp:ModalPopupExtender>
                <div id="div_content_titulo" class="modal_title_superior">              
                     <button id="boton_hide_sol" type="button" value="Button_cerrar_usu_rel_solicitud" class="close da_event_captive mr-2">&times;</button>
                </div>
                <div id="contenido_procesa_usu_rel_solicitud" style="background-color: white; width: 100%; height:100%" class="modal_content_back container-fluid">         
                    <div id="div_contenedor_titulo_documentos_relacionados" style="width: 100%; position: inherit; left: auto; 
                     text-align: left; font-family: Arial; font-size: 16px; font-weight: 600; text-align: center; ">
                        <asp:Label ID="Label_relacion_solicitudes" runat="server" Text="Usuarios relacionados a la solicitud"></asp:Label>
                    </div>
                    <asp:UpdatePanel ID="UpdateGeneral_documentos" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                             <asp:Button ID="Button_listar_usuarios_relacionados_solicitud" runat="server" Text="Button" Style="display:none" />
                            <input id="hdnEmailID_documentos" type="hidden" value="0" runat="server">
                            <input id="hdnEmailID_VAL_documentos" type="hidden" value="0" runat="server">
                            <input id="Hidden_id_usuarios_sel" type="hidden" value="0" runat="server">
                            <input id="HiddenEmailconsulta_documentos" type="hidden" value="" runat="server">
                            <div id="contenido_titulo_val_radicacion_documentos" style="height:auto; width: 99%;margin-top:1px; margin-left:2px">
                                <asp:Label ID="Label_estado_documentos" runat="server" ForeColor="Black" Font-Size="9px" Style="float: right; display:none"></asp:Label>
                                <asp:Label ID="titulo_label_expedientes_documentos" runat="server" ForeColor="Black" Font-Size="12px">Resultados busqueda</asp:Label>
                            </div>
                            <div id="content_data_grid" class="conten_gred_border_">
                                <asp:Panel ID="Panelactividad_documentos" runat="server" Wrap="False"
                                  style="height:98%; overflow:auto" >
                                <asp:GridView ID="data_grid_documentos" runat="server"   AllowPaging="true" PageSize="5"  EnableViewState="true"
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
                            </div>
                            
                        </ContentTemplate>

                    </asp:UpdatePanel>    
                   
                      <div style="display:none; height:1px">
                             <asp:Button ID="Button_usu_rel_solicitud" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                             <asp:Button ID="ButtonSalir_usu_rel_solicitud" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                              <asp:Button ID="Button_cerrar_usu_rel_solicitud" runat="Server" Text="" CssClass="invisible"
                              />
                      </div>
                         
                </div>
            <div id="div_expediente_seleccionado" style="width: 100%; font-family: Arial; font-size: 11px" class="container-fluid modal-footer">
                    <asp:UpdatePanel ID="UpdatePanel_expediente_seleccionado" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>                      
                            <input id="Hidden_resultado_aprobacion" type="hidden" value="" runat="server">
                            <input id="Hidden_actualizacion_general" type="hidden" value="" runat="server">
                            <input id="Hidden_actualizacion_usuario" type="hidden" value="" runat="server">
                            <asp:Button ID="Button_estado_solicitud" runat="server" Text="Listar notas del usuario" ToolTip="listar las notas a la solicitud de aprobación del usuario seleccionado en la lista"  CssClass="boton_azul" style="margin-top:5px; display:none"/> 
                            <asp:Button ID="Button_documentos_correccion" runat="server" Text="Listar anexos de corrección" CssClass="boton_azul" ToolTip="Listar los anexos de corrección del usuario seleccionado en la lista" style="margin-top:5px ; display:none" />  
                            <asp:Button ID="Button_archiva_solicitud" runat="server" Text="Archiva la solictud" ToolTip="Archiva la solictud de aprobación del usuario que seleccione en la lista"  CssClass="boton_azul" style="margin-top:5px ; display:none"/> 
                            <asp:Button ID="Button_notifica_solicitud_usuario_correo" runat="server" Text="Notificar al correo electrónico" ToolTip="Notificar al correo electrónico la solcitud de aprobación al usuario que seleccione en la lista"  CssClass="boton_azul" style="display:none" />
                            <asp:Button ID="Button_nuevo_integrante" runat="server" Text="Nuevo usuario" ToolTip="Relacionar un nuevo integrante a la solicitud de aprobación" CssClass="btn  btn-success" />
                           
                        </ContentTemplate>
                    </asp:UpdatePanel>

                </div>
            </asp:Panel>
           <!--Mensaje popup para paginas externas libres-->       
              <asp:Panel ID="PanelLibre" runat="server" Style="display:none; color: White; width: 70%; height: 100%" CssClass="modal_content_general">
                  <asp:ModalPopupExtender ID="ModalPopupExtenderLibre" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonD"
                      PopupControlID="PanelLibre" CancelControlID="Buttoncabcel">
                  </asp:ModalPopupExtender>
                  <div id="title_bot" class="modal_title_superior"> 
                       <h6 class="modal-title d-inline ml-2">Notas</h6>
                       <button type="button" value="Buttoncabcel" class="close da_event_captive mr-2">&times;</button>                 
                  </div>
                  <div id="conten_iframe" style=" background-color: #FFFFFF; height:auto; width: auto" class="modal_content_back">
                      <asp:UpdatePanel ID="UpdatePanelLibre" runat="server" UpdateMode="Conditional">
                          <ContentTemplate>
                              <iframe id="Iframelibre_notas_general_" class="bg-transparent" runat="server" frameborder="0"  scrolling="no" style="width:100%; height:100%" ></iframe>
                          </ContentTemplate>
                      </asp:UpdatePanel>
                  </div>
                  <div style="display:none; height:1px">
                       <asp:Button ID="Buttoncabcel" runat="Server" Text="" style="display:none"
                                />
                       <asp:Button ID="ButtonD"  runat="server" Text="" Height="1px" Width="1px" style="display:none" />
                  </div>
                  
              </asp:Panel>
       
         <!--Popup registrar nuevo usuario a la solicitud -->
        <asp:Panel ID="Panel_registro_solicitud_usuario" runat="server"  Style="width: 60%; height: auto; display:none" CssClass="modal_content_general">
            <asp:ModalPopupExtender ID="ModalPopupExtender_registro_solicitud_usuario" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_registro_solicitud_usuario"
                PopupControlID="Panel_registro_solicitud_usuario" CancelControlID="ButtonCerrar_registro_solicitud_usuario">
            </asp:ModalPopupExtender>
            <div class="modal-content">
                <div id="div_content_title_nueva_sol" class="modal_title_superior_ modal-header">
                      <h6 class="modal-title">Relaciona nuevo usuario a la solicitud</h6>
                      <button type="button" value="ButtonCerrar_registro_solicitud_usuario" class="close da_event_captive">&times;</button>
                </div>
                <div id="Cotenedor_registro_solicitud_usuario" style="color: none; height: auto; width: 100%; border-top:none" class="modal_content_back modal-body">
                    
                        <div class="col-sm-12">
                            <select class="tokenize-callable-demo_respuesta____" style="width: 100%;" multiple>
                            </select>
                        </div>
                   
                    
                    <div style="display: none; height: 1px">
                        <asp:Button ID="ButtonSalir_registro_solicitud_usuario" runat="server" Text="" Height="1px" Width="1px" Style="display: none" />
                        <asp:Button ID="ButtonCerrar_registro_solicitud_usuario" runat="Server" Text=""
                            Style="display: none" />
                    </div>

                </div>
                 <div class="modal-footer">
                       <asp:UpdatePanel ID="UpdatePanel_registro_solicitud_usuario" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Button ID="Button_activa_relacion_usuarios" runat="server" Text="Relacionar" CssClass="boton" Style="display: none" />
                                    <input id="Hidden_resultado_actualizar_usuario" runat="server" type="hidden" value=""/>
                                    <asp:Button ID="Button_registrar_solicitud_aprobacion_usuario" runat="server" Text="Aceptar" CssClass="btn btn-success" Style="margin-bottom: 5px; margin-top: 5px; margin-right: 5px; float: right" OnClientClick="Agrega_usuario_a_la_solicitud_aprobacion_();" />
                                    <asp:Button ID="Button_cancela_registro_solictud" runat="server" Text="Cancelar" CssClass="btn btn-default" Style="margin-bottom: 5px; margin-top: 5px; margin-right: 5px; float: right" />
                                    <asp:Button ID="Button_actualiza_registro_solicitud" runat="server" Text="Cancelar" CssClass="btn btn-default" Style="margin-bottom: 5px; margin-top: 5px; margin-right: 5px; float: right; display: none" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                 </div>

            </div>
            

        </asp:Panel>
        <!--Popup registrar nueva solicitud-->
            <asp:Panel ID="Panel_actualizacion_anualidad" runat="server"   Style=" width: 90%; height:auto; display:none" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_actualizacion_anualidad" runat="Server" BackgroundCssClass="FondoAplicacion" 
                     TargetControlID="ButtonSalir_actualizacion_anualidad"
                    PopupControlID="Panel_actualizacion_anualidad" CancelControlID="ButtonCerrar_actualizacion_anualidad"></asp:ModalPopupExtender>     
                <div class="modal-content">
                    <div id="div3" class="modal_title_superior_ modal-header">
                           <h6 class="modal-title">Solicitud de aprobación</h6>
                           <button type="button" value="ButtonCerrar_actualizacion_anualidad" class="close da_event_captive">&times;</button>
                    </div>
                    <div id="Cotenedor_actualizacion_anualidad" style="color: Black; background-color: #FFFFFF; height: 100%; width: 100%; border-top:none" class="modal_content_back modal-body">
                        <div style="margin-left: 10px; margin-right: 10px; margin-top: 10px">
                            <div class="row">
                                <div class="col-sm-12">
                                    <asp:UpdatePanel ID="UpdatePanel_registro_solicitud" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <div style="margin-top: 1px; background-color: white">
                                                <div class="form-group-sm">
                                                    <h6 class="h6">Prioridad de la solicitud</h6>
                                                    <asp:DropDownList ID="DropDownList_prioridad_solicitud" CssClass="form-control" runat="server" Style="width: 200px"></asp:DropDownList>
                                                </div>
                                                <div class="form-group-sm">
                                                    <h6 class="h6 mt-2">Fecha límite de aprobación</h6>
                                                    <div class="row">
                                                        <div class="col-sm-2">
                                                            <asp:TextBox ID="TextBox_fecha_limite_solicitud" CssClass="form-control" runat="server" Width="100px"></asp:TextBox>
                                                        </div>
                                                        <div class="col-sm-1">
                                                            <asp:CalendarExtender ID="TextBoxFECHA_EXTREMA_INICIAL_CalendarExtender" runat="server" BehaviorID="TextBoxFECHA_EXTREMA_INICIAL_CalendarExtender" TargetControlID="TextBox_fecha_limite_solicitud" Format='yyyy-MM-dd' PopupButtonID="A1" />
                                                            <a id="A1" class="" style="" title="Examinar el calendario" href="#"><i style="margin-left: 1px" class="fal fa-calendar-alt fa-2x"></i></a>
                                                        </div>
                                                    </div>

                                                </div>

                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>
                            </div>
                            <div class="row "  style="margin-top: 10px">
                                <div   class="col-sm-12" style="overflow:auto">
                                    <select class="tokenize-callable-demo_respuesta___" style="width: 99%;" multiple>
                                    </select>
                                </div>
                            </div>
                            <div>
                                <asp:UpdatePanel ID="UpdatePanel_boton_nota_active" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div class="row">
                                            <div class="col-sm-12">
                                                <asp:TextBox ID="TextBox_nota_aprobacion" runat="server" CssClass="form-control" TextMode="MultiLine" Style="width: 99.5%; font-size: 12px; margin-top: 3px; margin-left: 3px" Rows="2" placeholder="Digita nota solicitud.."></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="col-sm-12">
                                                <asp:Button ID="Button_registrar_solicitud_aprobacion" runat="server" Text="Aceptar" CssClass="btn  btn-success" Style="float: right; margin-bottom: 10px; margin-top: 10px; margin-right: 5px" OnClientClick="Solicitud_aprobacion_tokenize();" />
                                                <asp:Button ID="Button_cancelar_registro" runat="server" Text="Cancelar" CssClass="btn btn-default" Style="float: right; margin-bottom: 10px; margin-top: 10px; margin-right: 5px;" />
                                                <input id="Hidden_resultado_actualizar" runat="server" type="hidden" value="">
                                                 <asp:Button ID="Button_actualiza" runat="server" Text="Cancelar" CssClass="btn btn-default" Style="float: right; margin-bottom: 10px; margin-top: 10px; margin-right: 5px; display: none" />
                                            </div>

                                        </div>
                                        <asp:Label ID="Label30" runat="server" Text="Usuarios relacionados a la solicitud de aprobación" Style="font-size: 14px; text-wrap: normal; display: none"></asp:Label>
                                        <asp:Button ID="Button_activa_usuario_relacion" runat="server" Text="Ver usuarios relacionados" CssClass="boton" Style="width: 96%; display: none" />
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                        <div style="display: none; height: 1px">
                            <asp:Button ID="ButtonCerrar_actualizacion_anualidad" runat="Server" Text="" Height="1px" Width="1px" Style="display: none" />
                            <asp:Button ID="ButtonSalir_actualizacion_anualidad" runat="server" Text="" Height="1px" Width="1px" Style="display: none" />
                        </div>

                    </div>
                </div>
            </asp:Panel>
      
        <!--Popup listado usuarios disponibles -->
            <asp:Panel ID="Panel_lista_usuarios_solicitud" runat="server" ForeColor="White"  Height="98%" Style="text-align: left; width: 90%; margin: auto; display:none">
                <asp:ModalPopupExtender ID="ModalPopupExtender_lista_usuarios_solicitud" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_lista_usuarios_solicitud"
                    PopupControlID="Panel_lista_usuarios_solicitud" CancelControlID="ButtonCerrar_lista_usuarios_solicitud" Y="1"></asp:ModalPopupExtender>
               
                <asp:Button ID="ButtonSalir_lista_usuarios_solicitud" runat="server" Text="Button" Height="20px" Width="20px" Style="display: none" />
                <div id="Cotenedor_lista_usuarios_solicitud" style="border: thin double #000080; color: Black; background-color: #FFFFFF; height: 100%; width: 100%; margin: auto;border-radius: 25px;padding: 10px;background: #ffffff"">
                    <asp:Button ID="ButtonCerrar_lista_usuarios_solicitud" runat="Server" Text="X"
                            ForeColor="#000066" Height="21px" style="float:right; margin-right:10px" />
                    <div id="div_lista_usuarios_solicitud" style="margin-top: 1px; border-color: #b0c4de; border-width: 1px; border-style: ridge; width:99%; height:100%">
                    <div>
                       
                        <input id="hdnEmailID_sel" type="hidden" value="" runat="server">
				        <input id="hdnEmailID_user" type="hidden" value="0" runat="server">
				        <input id="Hidden_resultado_gred" type="hidden" value="YES" runat="server">
                        <input id="Hidden_result_detalle" type="hidden" value="YES" runat="server">
                        <input id="Hidden_correos_electronico" type="hidden" value="YES" runat="server">
				        <div id="contenido_label" style="width:100%; height:20%; background-color: #E7EDF5; text-align:left" >
				            <asp:Label ID="Label_titulo" runat="server" Text="Usuarios disponibles para solicitud de aprobación" style="font-family:Arial; color:black; font-size:14px"></asp:Label> &nbsp
                            <asp:Label ID="Label_help" runat="server" Text="(Debe chequear los usuarios que requiere para aprobación |x|)" style="font-family:Arial; color:black; font-size:14px; color:red"></asp:Label> &nbsp &nbsp
                            <asp:Label ID="Label_totales_usuario" runat="server" Text="" style="font-family:Arial; color:black; font-size:14px; color:red"></asp:Label>
				        </div>
				        <div id="Lista" style="width: 100%; position: inherit; height:60%;  margin-top: 1px; border-color: #b0c4de; border-style: ridge; border-width: 1px">
				            <asp:UpdatePanel ID="UpdatePanelgred" runat="server" UpdateMode="Conditional"  RenderMode="Inline">
				                <ContentTemplate>
				                   <asp:GridView ID="GridViewlista" runat="server" Font-Size="10pt" EnableViewState="true"
				                            AutoGenerateSelectButton="False" CssClass="filtrar" GridLines="None">
				                              <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
				                                <HeaderStyle CssClass="GridviewScrollHeader_line" />
				                                <RowStyle CssClass="GridviewScrollItem_line" />
				                                <PagerStyle CssClass="GridviewScrollPager_line" />
                                       <Columns>
                                           <asp:TemplateField>
                                               <ItemTemplate>
                                                   <input type="checkbox" id="chkSelection" runat="server" class="dummychkstyle" />
                                               </ItemTemplate>
                                           </asp:TemplateField>
                                       </Columns>
				                           
				                        </asp:GridView>
				
				                    
				                </ContentTemplate>
				                <Triggers>
				               
				                </Triggers>
				            </asp:UpdatePanel>
				            </div>
				            <div id="contenido_botonoes" style="width: 100%; position: inherit; left: auto; float: right; height: 20%; background-color: #E7EDF5">
				                <asp:UpdatePanel ID="updatepnael_botones" runat="server" UpdateMode="Conditional"  >
				                    <ContentTemplate>
				                         <asp:TextBox ID="contenidobusqueda" runat="server" style="left:30px; width:250px; margin-left:20px; margin-top:3px"></asp:TextBox>
				                          <asp:Button ID="Buttonbuscar" runat="server" Text="Buscar" class="boton_blanco"  />
				                          <asp:CheckBox ID="CheckboxBusqueda" runat="server" />
				                          <label style="font-family:Arial ; font-size:10px"   >Buscar sólo palabra completa</label>
                                        <asp:Button ID="Button_asignar_usuarios" runat="server" Text="Asignar usuarios seleccionados" class="boton_blanco"  OnClientClick="asigna_usuario_grupos_cheked()" />
				                    </ContentTemplate>
				                </asp:UpdatePanel>                       
				            </div>     
    </div>
                           

                    </div>


                </div>

            </asp:Panel>
        <!--lista_documentos_colaboracion-->
        
            <asp:Panel ID="Panel_lista_documentos_colaboracion" runat="server" Style="display:none; width: 50%; height:auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_lista_documentos_colaboracion" runat="server" 
                      TargetControlID="ButtonSalir_lista_documentos_colaboracion" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_lista_documentos_colaboracion" PopupControlID="Panel_lista_documentos_colaboracion"></asp:ModalPopupExtender>
                <div class="modal-content">
                    <div id="divcabecer2_lista_documentos_colaboracion" class="modal_title_superior_ modal-header">
                         <h6 class="modal-title">Anexos relacionados</h6>
                         <button type="button" value="Button_cerrar_lista_documentos_colaboracion" class="close da_event_captive">&times;</button> 
                    </div>
                    <div id="contenido_procesa_lista_documentos_colaboracion" style="background-color: white; width: 100%; height: auto; overflow: auto; border-top:none" class="modal_content_back modal-body">
                        <div id="div_status_bar" style="height: auto; color: black; text-align: left; font-family: Arial; background-color: white; margin-top: 0px;">
                            <asp:UpdatePanel ID="UpdatePanel_estado_doc_colaboracion" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Label ID="Label_estado_doc_colaboracion" CssClass="h6" runat="server" Text="Estado" Style=""></asp:Label>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                        <asp:UpdatePanel ID="UpdatePanel_seleccion_documento" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Panel ID="Panel_seleccion_documento" runat="server" Style="overflow: auto; width: 100%; height: 200px" ScrollBars="Auto">
                                    <asp:Table ID="Table_seleccion_documento"  runat="server" Style="text-align: left; width: 99%; font-size: 12px; margin-left: 2px;" EnableViewState="false" CssClass="table table-hover"></asp:Table>
                                </asp:Panel>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
               
                
                <asp:UpdatePanel ID="UpdatePanel_descraga_documento" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Button ID="Button_descarga_documento" runat="server" Text="Button" Style="display: none" />
                        <input id="Hidden_documento_descarga" type="hidden" value="" runat="server">
                    </ContentTemplate>
                </asp:UpdatePanel>
                <div style="display: none; height: 1px">
                    <asp:Button ID="Button_lista_documentos_colaboracion" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                    <asp:Button ID="ButtonSalir_lista_documentos_colaboracion" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                    <asp:Button ID="Button_cerrar_lista_documentos_colaboracion" CssClass="invisible" runat="Server" Text="" class="invisible" />
                </div>
               
            </asp:Panel>
        
        <!--anula_solictud_aprobacion-->        
            <asp:Panel ID="Panel_anula_solictud_aprobacion" runat="server" Style="display:none;  width: 50%; height:auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_anula_solictud_aprobacion" runat="server" BehaviorID="Panel_anula_solictud_aprobacion" 
                     TargetControlID="ButtonSalir_anula_solictud_aprobacion" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_anula_solictud_aprobacion" PopupControlID="Panel_anula_solictud_aprobacion" ></asp:ModalPopupExtender>
                <div class="modal-content">                
                <div id="divcabecer2_anula_solictud_aprobacion"  class="modal_title_superior_ modal-header">
                     <h6 class="modal-title">Anular solicitud</h6>
                     <button type="button" value="Button_cerrar_anula_solictud_aprobacion" class="close da_event_captive">&times;</button>                       
                </div>
                <div id="contenido_procesa_anula_solictud_aprobacion" style="background-color: white; width: 100%; height:auto ; border-top:none" class="modal_content_back  modal-body">                            
                    <div>
                         <asp:UpdatePanel ID="UpdatePanel_anula_solictud_aprobacion" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Label ID="Label_nota_anulado" CssClass="h6" runat="server" Text="Nota de anulación" style=" margin-left:3px"></asp:Label> <br />
                                <asp:TextBox ID="TextBox_nota_anulado"  CssClass="form-control" runat="server" TextMode="MultiLine" style="margin-left:3px; margin-right:3px; width:98%" Rows="3"></asp:TextBox>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    
                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button_cerrar_anula_solictud_aprobacion" runat="Server" Text="X" CssClass="invisible" />
                        <asp:Button ID="Button_anula_solictud_aprobacion" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                        <asp:Button ID="ButtonSalir_anula_solictud_aprobacion" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    </div>
                      
                </div>
                 <div class="modal-footer justify-content-end">
                     <asp:UpdatePanel ID="UpdatePanel_buton_anula" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>           
                                 <asp:Button ID="Button_cancela_nula_solicitud_aprobacion" runat="server" Text="Cancelar"  CssClass="btn  btn-light" style="margin-bottom:5px; margin-right:5px; margin-top:5px"/>
                                <asp:Button ID="Button_nula_solicitud_aprobacion" runat="server" Text="Aceptar"  CssClass="btn  btn-success" style="margin-bottom:5px; margin-right:5px; margin-top:5px"/>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                </div>
                 </div>
            </asp:Panel>    
         <div id="tol_pie" style=" float:right;  background-color:#E7EDF5; width:100%; height:3%;border-style: ridge; border-bottom-width: 0.5px; border-left-width: 1px; border-right-width: 1px; border-top-width: 1px;text-align:center; display:none">
                 <asp:Label ID="Label10" runat="server" Text="Estado" style="font-family:Arial;font-size:11px"></asp:Label>
                    <asp:UpdatePanel ID="updatapanel_iframe" runat="server" UpdateMode="Conditional" >
                            <ContentTemplate>
                                <input id="Hidden_ruta_archivo" type="hidden" value="" runat="server"/>
                                <iframe runat="server" style="float:left" id="ifmExcel_" width="0" height="0" marginheight="0" marginwidth="0"
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
