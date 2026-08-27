<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormWorkflowEditarCamposRuta.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormWorkflowEditarRuta" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">

<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
     <link href="../ccs/themes/base/jquery.ui.all.css" rel="stylesheet" type="text/css" />
<link href="../Styles/styleMenu.css" rel="stylesheet" type="text/css" /> 
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
 <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
<link href="../Styles/Menu3.css" rel="stylesheet" />
    <title>Administración clasificación</title>
   <script src="../js/ui/jquery-3.4.1.min.js"></script>  
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
    <script src="../js/ScrollableGridPlugin.js"></script>   
    <script src="../js/ScrollableGridViewPlugin_ASP.NetAJAXmin.js" type="text/javascript"></script>
    <script src="../Fixed-Header-Table-master/gridviewScroll.min.js"></script>
    <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <script src="../js/workflow/WebFormWorkflowEditarCamposRuta.js"></script>
    <script src="../js/validate_campos.js"></script>
     <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
      <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <link href="../Styles/samples.css" rel="stylesheet" />
    <script  src="../Awesome/js/all.js"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet"/>
  <link href="../Awesome/css/brands.css" rel="stylesheet"/>
  <link href="../Awesome/css/solid.css" rel="stylesheet"/>
    <script  src="../Awesome/js/brands.js"></script>
  <script  src="../Awesome/js/solid.js"></script>
  <script  src="../Awesome/js/fontawesome.js"></script>
   
    <script src="../js/validate_campos.js"></script>
    <script src="../js/java_general/general_code_java.js?v=20260827-compatible-events5"></script>
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
                if (elment_postbak.id == "DropDownList_rutas_workflow") {
                    auto_zise_popup_lista_campos_disponibles("1", "1");
                }
                //Lista los grid con la configuración de los campos disponibles
                if (elment_postbak.id == "Button_agregar_campo") {
                    if (document.getElementById("Hidden_resultado_gred").value == "YES") {
                        document.getElementById("Hidden_resultado_gred").value = "";
                        auto_zise_popup_lista_campos_disponibles("1", "1");
                    }
                    
                }
                if (elment_postbak.id == "Button_importar_campo") {
                    if (document.getElementById("Hidden_resultado_gred_dos").value == "YES") {
                        document.getElementById("Hidden_resultado_gred_dos").value = "";
                        auto_zise_popup_lista_campos_disponibles("0", "1");
                    }

                }
                if (elment_postbak.id == "Button_aceptar_confirmacion") {
                    
                    if (document.getElementById("Hidden_estado_eliminar").value == "YES") {
                        document.getElementById("Hidden_estado_eliminar").value = "";
                        eliminar_fila_data_gred("data_grid_dos", "hdnEmailID_dos");
                    }
                }
                if (elment_postbak.id == "Button_actualiza_campo_lista") {
                    if (document.getElementById("Hidden_estado_configura_campo_lista").value == "YES") {
                        document.getElementById("Hidden_estado_configura_campo_lista").value = "";
                        pre_actualiza_campos_lista("data_grid_dos", document.getElementById("hdnEmailID_dos").value)
                    }
                }
                //ImageButton_baja_item
                if (elment_postbak.id == "ImageButton_baja_item") {
                    if (document.getElementById("Hidden_resultado_aprobacion").value == "YES") {
                        document.getElementById("Hidden_resultado_aprobacion").value = "";
                        cambia_registro_gred("data_grid_dos", document.getElementById("hdnEmailID_dos").value, document.getElementById("Hidden_id_orden_seleccion").value, document.getElementById("Hidden_id_idex_config_siguiente").value, document.getElementById("Hidden_ide_orden_siguiente").value);
                    }
                    
                }
                //ImageButton_sube_item
                if (elment_postbak.id == "ImageButton_sube_item") {
                    if (document.getElementById("Hidden_resultado_aprobacion").value == "YES") {
                        document.getElementById("Hidden_resultado_aprobacion").value = "";
                        cambia_registro_gred("data_grid_dos", document.getElementById("hdnEmailID_dos").value, document.getElementById("Hidden_id_orden_seleccion").value, document.getElementById("Hidden_id_idex_config_siguiente").value, document.getElementById("Hidden_ide_orden_siguiente").value);
                    }

                }
                //Button_aceptar_confirmacion_actualiza_campo
                if (elment_postbak.id == "Button_aceptar_confirmacion_actualiza_campo") {
                    if (document.getElementById("Hidden_estado_actualizar").value == "1") {
                        document.getElementById("Hidden_estado_actualizar").value = "-1";
                        var nombre_campo = new Array(1);
                        nombre_campo[0] = document.getElementById("Hidden_nombre_campo").value;
                        seter_gre_campo('data_grid_dos', document.getElementById("hdnEmailID_dos").value, '0', nombre_campo);
                        actualiza_gre_campo('data_grid_dos', document.getElementById("hdnEmailID_dos").value, '1', nombre_campo);
                    }
                }
               
            }

            </script>
        <div>
            <div id="div_contenedor_drecho" style=" width: 100%; height: 100%; position: relative" class="p-2">
                
                 <input id="Hidden_result_detalle" type="hidden" value="YES" runat="server"/>

                <div id="div_unidades_title" style="width: 100%; position: inherit; text-align: left;  text-align:left; margin-top: 1px" class="border_superior_radius_">
                    <asp:UpdatePanel ID="UpdatePanel_droplist_rutas" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <div  class="row">
                                 <div class="col-2 align-content-center">
                                      <asp:Label ID="Label_title_principal"  CssClass="h6" runat="server" Text="Rutas disponibles" ></asp:Label>
                                 </div>
                                 <div class="col-10 align-content-start">
                                     <asp:DropDownList ID="DropDownList_rutas_workflow" CssClass="form-control" runat="server" AutoPostBack="true" ></asp:DropDownList>
                                 </div>
                            </div>  
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div id="Contenedorgrid" style="width: 100%">
                    <asp:UpdatePanel ID="UpdateGeneral" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <input id="Hidden_resultado_gred" type="hidden" value="YES" runat="server"/>
                            <input id="Hidden_resultado_gred_dos" type="hidden" value="YES" runat="server"/>
                            <input id="hdnEmailID" type="hidden" value="" runat="server"/>
                            <input id="HiddenType" type="hidden" value="0" runat="server"/>
                            <input id="hdnEmailID_VAL" type="hidden" value="0" runat="server"/>
                            <input id="HiddenEmailconsulta" type="hidden" value="" runat="server"/>

                            <div id="contenido_titulo_data_grid_title" style="width: 100%">
                                <asp:Label ID="Label_estado" runat="server" ForeColor="Black" Font-Size="9px"></asp:Label>
                                <asp:Label ID="Label3" runat="server" ForeColor="Black"  CssClass="h6" Style="font-weight: 600">Campos de la ruta</asp:Label>
                                &nbsp
                                <asp:Label ID="titulo_label_title" runat="server" ForeColor="Black" Font-Size="12px">Resultados busqueda</asp:Label>
                            </div>
                            <asp:Panel ID="Panel_principal" runat="server" Wrap="False" style="overflow:auto"
                                Width="100%" >
                                <asp:GridView ID="data_grid" runat="server" Style="position: inherit" AutoGenerateSelectButton="False" CssClass="filtrar_ table" GridLines="None" Font-Size="12px">
                                    <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                    <HeaderStyle CssClass="GridviewScrollHeader_line_boot" />
                                    <RowStyle CssClass="GridviewScrollItem_line_" />
                                    <PagerStyle CssClass="GridviewScrollPager_line_" />


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
                <div id="contenedor_opciones_data_gred_uno_general" class="modal-footer justify-content-end" style="border-color: #b0c4de; border-width: 1px; border-style: ridge">
                    <asp:UpdatePanel ID="Update_botones_opciones_solicitud_general" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <asp:Button ID="Button_agregar_campo_disponible" runat="server" Text="Nuevo campo" ToolTip="Registrar un nuevo campo" CssClass="btn btn-success" />
                            <asp:Button ID="Button_importar_campo" runat="server" Text="Importar campo" ToolTip="Importar campo seleccionado a la lista de tareas pendientes" CssClass="btn btn-success" />
                            <asp:Button ID="Button_editar_campo_disponible" runat="server" Text="Editar campo" style="display:none" ToolTip="Editar campo disponible seleccionado" CssClass="btn btn-success" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>

             

                <div id="Contenedorgrid_dos" style="width: 100%">
                    <asp:UpdatePanel ID="UpdateGeneral_documentos" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Button ID="Button_listar_usuarios_relacionados_solicitud" runat="server" Text="Button" Style="display: none" />
                            <input id="hdnEmailID_dos" type="hidden" value="-1" runat="server"/>
                            <div id="contenido_titulo_data_grid_dos_title" class="row" style=" width: 100%">
                                <div class="col-6">
                                    <asp:Label ID="Label_estado_documentos" runat="server" ForeColor="Black" Font-Size="9px" Style="float: right; display: none"></asp:Label>
                                    <asp:Label ID="Label9" runat="server" ForeColor="Black" CssClass="h6" Style="font-weight: 600">Campos para listar en la ruta </asp:Label>
                                    &nbsp
                                    <asp:Label ID="titulo_label_title_dos" runat="server" ForeColor="Black" Font-Size="12px">Resultados busqueda</asp:Label>
                                </div>
                                <div class="col-6  justify-content-end">
                                    <nav id="menucab" class="navbar navbar-expand-sm nav_botota_person_gray modal_content_no_back_inferior">
                                        <button id="nav_togle_display" class="navbar-toggler" type="button" style="background-color: #6d7fcc" data-toggle="collapse" data-target="#navbarNavDropdown">
                                            <span class="navbar-toggler-icon_"><i style="color: white" class="fad fa-th-list"></i></span>
                                        </button>
                                        <div class="collapse navbar-collapse row" id="navbarNavDropdown">
                                            <ul class="navbar-nav col-md-12">
                                                <li class="nav-item dropdown active ml-2 mr-0 active_">
                                                    <a class="nav-link  dropdown-toggle " style="color: #6d7fcc" href="#" id="A6" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc" class="fal fa-list-ol"></i> Opciones
                                                    </a>
                                                    <div class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">
                                                        <a style="color: #6d7fcc" href="#" class="dropdown-item font-weight-light" onclick="activa_boton_client_server('Button_editar_campo_lista');"><i class="far fa-minus"></i> Editar campo </a>
                                                        <a style="color: #6d7fcc" href="#" class="dropdown-item font-weight-light" onclick="activa_boton_client_server('Button_activa_configura_orden');"><i class="far fa-minus"></i> Orden de lista</a>
                                                        <a style="color: #6d7fcc" href="#" class="dropdown-item font-weight-light" atr_adj="enlace_adjunt" onclick="activa_boton_client_server('Button_activa_campo_radicado');"><i class="far fa-minus"></i> Activa campo radicado</a>
                                                        <a style="color: #6d7fcc" href="#" class="dropdown-item font-weight-light" onclick="activa_boton_client_server('Button_activa_campo_tramite');"><i class="far fa-minus"></i> Activa campo trámite</a>
                                                        <a style="color: #6d7fcc" href="#" class="dropdown-item font-weight-light" onclick="activa_boton_client_server('Button_activa_campo_beneficiario');"><i class="far fa-minus"></i> Activa campo beneficiario</a>
                                                        <a style="color: #6d7fcc" href="#" class="dropdown-item font-weight-light" onclick="activa_boton_client_server('Button_activa_campo_fecha')"><i class="far fa-minus"></i> Activa campo fecha vence</a>
                                                      
                                                    </div>
                                                </li>
                                            </ul>
                                        </div>
                                    </nav>
                                </div>
                            </div>
                            <asp:Panel ID="Panelactividad_documentos" runat="server" Wrap="False" style="overflow:auto"
                                Width="100%" >
                                <asp:GridView ID="data_grid_dos" runat="server"   Style="position: inherit"
                                    AutoGenerateSelectButton="False" CssClass="filtrar_ table" GridLines="None" Font-Size="12px">
                                    <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                    <HeaderStyle CssClass="GridviewScrollHeader_line_boot" />
                                    <RowStyle CssClass="GridviewScrollItem_line_" />
                                    <PagerStyle CssClass="GridviewScrollPager_line_" />


                                </asp:GridView>
                            </asp:Panel>

                        </ContentTemplate>

                        <Triggers>
                        </Triggers>
                    </asp:UpdatePanel>

                </div>
                <div id="contenedor_opciones_data_gred_dos_general" class="modal-footer" style="width: 100%; border-color: #b0c4de; border-width: 1px; border-style: ridge">
                  <div class="col-6">
                     
                  </div>
                    <div class="col-6 justify-content-end">
                        <asp:Label ID="Label_indices_campos" runat="server" Text="Cambiar el orden de los campos"></asp:Label>
                        <asp:ImageButton ID="ImageButton_baja_item" runat="server" ImageUrl="../workflow/imageneswf/bajar_campo.png" Height="20" Width="25" ToolTip="Bajar campo seleccionado" />
                        <asp:ImageButton ID="ImageButton_sube_item" runat="server" Style="" ImageUrl="../workflow/imageneswf/subir_campo.png" Height="20" Width="25" ToolTip="Subir campo seleccionado" />

                        <asp:Label ID="Label_Estado_ruta" runat="server" Text="" Style="float: right; font-family: Arial; font-size: 10px"></asp:Label>
                    </div>

                </div>
                <div style="display:none">
                      <asp:UpdatePanel ID="UpdatePanel_expediente_seleccionado" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <input id="Hidden_resultado_aprobacion" type="hidden" value="" runat="server"/>
                            <input id="Hidden_actualizacion_general" type="hidden" value="" runat="server"/>
                            <input id="Hidden_actualizacion_usuario" type="hidden" value="" runat="server"/>
                            <input id="Hidden_id_idex_config_siguiente" type="hidden" value="-1" runat="server"/>
                            <input id="Hidden_ide_orden_siguiente" type="hidden" value="-1" runat="server"/>
                            <input id="Hidden_id_orden_seleccion" type="hidden" value="-1" runat="server"/>
                            <input id="Hidden_estado_actualiza_campo" type="hidden" value="-1" runat="server"/>
                            <input id="Hidden_resultado_promp" type="hidden" value="-1" runat="server"/>
                            <asp:Button ID="Button_eliminar" runat="server" Text="Eliminar campo" ToolTip="Eliminar campo seleccionado" CssClass="btn btn-success"  />
                            &nbsp
                            <asp:Button ID="Button_editar_campo_lista" runat="server" Text="Editar campo" CssClass="btn btn-success" ToolTip="Editar el campo seleccionado"  />
                             &nbsp
                            <asp:Button ID="Button_activa_configura_orden" runat="server" Text="Orden de lista" CssClass="btn btn-success" ToolTip="Seleccione el orden en el cual se mostrara la lista"  />
                            &nbsp
                            &nbsp
                            <asp:Button ID="Button_activa_campo_radicado" runat="server" Text="Activa campo radicado" CssClass="btn btn-success" ToolTip="Activa el campo seleccionado como campo radicado"   />
                            &nbsp
                            &nbsp
                             <asp:Button ID="Button_activa_campo_tramite" runat="server" Text="Activa campo trámite" CssClass="btn btn-success" ToolTip="Activa el campo seleccionado como campo tramite"  />
                            &nbsp
                            &nbsp
                             <asp:Button ID="Button_activa_campo_beneficiario" runat="server" Text="Activa campo beneficiario" CssClass="btn btn-success" ToolTip="Activa el campo seleccionado como campo tramite"  />
                            &nbsp
                            &nbsp
                             <asp:Button ID="Button_activa_campo_fecha" runat="server" Text="Activa campo fecha vence" CssClass="boton_azul" ToolTip="Activa el campo seleccionado como campo tramite" Style="margin-top: 5px" />
                            &nbsp
                            &nbsp
                              
                           
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
        <!--agregar_campo_ruta_workflow-->
          <div id="agregar_campo_ruta_workflow">
            <asp:Panel ID="Panel_agregar_campo_ruta_workflow" runat="server" Style="display:none; width: 70%; height:auto" CssClass="modal_content_general_">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_agregar_campo_ruta_workflow" runat="server" BehaviorID="Panel_agregar_campo_ruta_workflow" 
                     TargetControlID="ButtonSalir_agregar_campo_ruta_workflow" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_agregar_campo_ruta_workflow" PopupControlID="Panel_agregar_campo_ruta_workflow" ></asp:ModalPopupExtender>
                <div class="modal-content">
                <div id="divcabecer2_agregar_campo_ruta_workflow"  class="modal-header">              
                    <h6 class="modal-title d-inline ">Agregar campo</h6>
                    <button type="button" value="Button_cerrar_agregar_campo_ruta_workflow" class="close da_event_captive ">&times;</button>
                </div>
                <div id="contenido_procesa_agregar_campo_ruta_workflow" style="background-color: white; width: 100%; height: 100%; border-top: none" class="modal_content_back modal-body">
                    <div class="row p-1">
                        <div class="col-6">
                             <asp:Label ID="Label_nombre_campo" CssClass="h6" runat="server" Text="Nombre Campo (*)" ></asp:Label>
                        </div>
                         <div class="col-6">
                              <asp:TextBox ID="TextBox_nombre_campo" runat="server"  CssClass="form-control"     onkeypress="return validate_letra_numero(event,this)" MaxLength="20"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row p-1">
                        <div class="col-6">
                             <asp:Label ID="Label_tipo_campo" CssClass="h6" runat="server" Text="Tipo campo (*)" ></asp:Label>
                        </div>
                         <div class="col-6">
                              <asp:UpdatePanel ID="UpdatePanel_tipo_campo" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                    <ContentTemplate>
                                        <asp:DropDownList ID="DropDownList_tipo_campo"  CssClass="form-control" runat="server" ></asp:DropDownList>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                        </div>
                    </div>
                    <div class="row p-1">
                        <div class="col-6">
                            <asp:Label ID="Label_longitud_campo" CssClass="h6" runat="server" Text="Longitud campo"  ></asp:Label>
                        </div>
                         <div class="col-6">
                              <asp:UpdatePanel ID="UpdatePanel_longitud_campo" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                    <ContentTemplate>
                                        <asp:TextBox ID="TextBox_longitud_campo" runat="server" CssClass="form-control" onkeypress="return validate_numero(event,this)"></asp:TextBox>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                        </div>
                    </div>
                    <div class="row p-1">
                        <div class="col-6">
                        </div>
                        <div class="col-6">
                            <asp:CheckBox ID="CheckBox_option_obligatorio" runat="server" Text="Campo obligatorio"  />
                        </div>
                    </div>
                   
                </div>
                    <div id="content_boton_user_rel" class="modal-footer justify-content-end">
                        <asp:UpdatePanel ID="UpdatePanel_buton_agregar_campo" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <asp:Button ID="Button_agregar_campo" runat="server" Text="Aceptar" CssClass="btn btn-success" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                <div style="display:none">
                     <asp:Button ID="Button_agregar_campo_ruta_workflow" CssClass="invisible" runat="server" Text="Button" Height="0px" Width="0px" />
                    <asp:Button ID="ButtonSalir_agregar_campo_ruta_workflow" CssClass="invisible" runat="server" Text="Button" Height="0px" Width="0px" />
                     <asp:Button ID="Button_cerrar_agregar_campo_ruta_workflow" runat="Server" Text="X" CssClass="invisible"
                              />
                </div>
                
               </div>
            </asp:Panel>
        </div>
        <!--confirma_eliminar_campo_lista-->
          <div id="confirma_eliminar_campo_lista">
            <asp:Panel ID="Panel_confirma_eliminar_campo_lista" runat="server" Style="display:none; color: White; width:300px; height: 130px" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_confirma_eliminar_campo_lista" runat="server" BehaviorID="Panel_confirma_eliminar_campo_lista" TargetControlID="ButtonSalir_confirma_eliminar_campo_lista" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_confirma_eliminar_campo_lista" PopupControlID="Panel_confirma_eliminar_campo_lista" ></asp:ModalPopupExtender>
                <div id="divcabecer2_confirma_eliminar_campo_lista"  class="modal_title_superior">               
                    <asp:Label ID="Label_confirma_eliminar_campo_lista" runat="server" Text="Mensaje" Font-Size="10" Style="float: left; font-family:Arial; margin-left:10px">
                    </asp:Label>
                    <div id="Divcerrarbuton2_confirma_eliminar_campo_lista" style="float: right">
                        <asp:Button ID="Button_cerrar_confirma_eliminar_campo_lista" runat="Server" Text="X" CssClass="modal_boton_hiden"
                             ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_confirma_eliminar_campo_lista" style="background-color: white; width: 100%; height: 99%; color: black; background-color: #FFFFFF" class="modal_content_back">                  
                        <asp:UpdatePanel ID="UpdatePanel_confirma_eliminar_campo_lista" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <input id="Hidden_estado_eliminar" type="hidden" value="" runat="server"/>
                                <div style="text-align: center">
                                    <br />
                                    <asp:Label ID="Label_title_comfirma_eliminar" runat="server" Text="Desea eliminar el campo seleccionado de la lista ?" style="font-family:Arial; font-size:14px"></asp:Label>
                                    <br />
                                    <br />
                                    <asp:Button ID="Button_aceptar_confirmacion" runat="server" Text="Aceptar" CssClass="boton_azul" /> &nbsp
                                    <asp:Button ID="Button_cancelar_confirmacion" runat="server" Text="Cancelar" CssClass="boton_azul" />
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                         
                </div>
                 <asp:Button ID="Button_confirma_eliminar_campo_lista" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_confirma_eliminar_campo_lista" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
            </asp:Panel>
        </div>
        <!--digitaliza_actualizar_campo_lista-->
          <div id="digitaliza_actualizar_campo_lista">
            <asp:Panel ID="Panel_digitaliza_actualizar_campo_lista" runat="server" Style="display:none;  width:40%" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_digitaliza_actualizar_campo_lista" runat="server" 
                     TargetControlID="ButtonSalir_digitaliza_actualizar_campo_lista" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_digitaliza_actualizar_campo_lista" PopupControlID="Panel_digitaliza_actualizar_campo_lista" ></asp:ModalPopupExtender>
                <div class="modal-content">
                <div id="divcabecer2_digitaliza_actualizar_campo_lista"  class="modal_title_superior_ modal-header">               
                    <h6 class="modal-title d-inline "></h6>
                    <button type="button" value="Button_cerrar_digitaliza_actualizar_campo_lista" class="close da_event_captive ">&times;</button>
                </div>
                <div id="contenido_procesa_digitaliza_actualizar_campo_lista" style="background-color: white; width: 100%; height: 99%" class="modal_content_back modal-body">                  
                        <asp:UpdatePanel ID="UpdatePanel_digitaliza_actualizar_campo_lista" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <input id="Hidden_estado_actualizar" type="hidden" value="-1" runat="server"/>
                                <input id="Hidden_nombre_campo" type="hidden" value="" runat="server"/>
                                <div class="row w-100">
                                    <div class="col-12 p-4 justify-content-center">
                                         <asp:Label ID="Label_promp_campo" runat="server" Text="Desea actualizar campo ?"  CssClass="h6"></asp:Label>
                                    </div>
                                </div>
                                <div class="row w-100">
                                    <div class="col-6 p-4">
                                        <asp:Label ID="Label_clave_seguridad" runat="server" Text="Clave de seguridad"  CssClass="h6"></asp:Label>
                                    </div>
                                     <div class="col-6 p-4">
                                         <asp:TextBox ID="TextBox_clave_campo" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
                                    </div>
                                </div>
                                
                            </ContentTemplate>
                        </asp:UpdatePanel>
                         
                </div>
                <div class="modal-footer justify-content-end">
                    <asp:UpdatePanel ID="UpdatePanel_digitaliza_actualizar_campo_lista_boton" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                  <asp:Button ID="Button_aceptar_confirmacion_actualiza_campo" runat="server" Text="Aceptar" CssClass="btn btn-success" /> &nbsp
                                    <asp:Button ID="Button_cancelar_confirmacion_actualiza_campo" runat="server" Text="Cancelar" CssClass="btn btn-secondary" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                   
                </div>
                </div>

                <div style="display:none">
                      <asp:Button ID="Button_digitaliza_actualizar_campo_lista" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                      <asp:Button ID="ButtonSalir_digitaliza_actualizar_campo_lista" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                      <asp:Button ID="Button_cerrar_digitaliza_actualizar_campo_lista" runat="Server" Text="" CssClass="invisible" Height="0px" Width="0px"/>
                </div>
               
            </asp:Panel>
        </div>
         <!--configura_campo_lista-->
          <div id="configura_campo_lista">
            <asp:Panel ID="Panel_configura_campo_lista" runat="server" Style="display:none;  width:60%; height: auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_configura_campo_lista" runat="server" BehaviorID="Panel_configura_campo_lista" TargetControlID="ButtonSalir_configura_campo_lista" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_configura_campo_lista" PopupControlID="Panel_configura_campo_lista" ></asp:ModalPopupExtender>
                <div class="modal-content">
                <div id="divcabecer2_configura_campo_lista"  class="modal-header">               
                   <h6 class="modal-title d-inline ">Opciones configuración campo</h6>
                    <button type="button" value="Button_cerrar_configura_campo_lista" class="close da_event_captive ">&times;</button>
                </div>
                <div id="contenido_procesa_configura_campo_lista" style="background-color: white; width: 100%; height: 99%" class="modal_content_back modal-body">                  
                        <asp:UpdatePanel ID="UpdatePanel_configura_campo_lista" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row w-100">
                                    <div class="col-12 p-2">
                                         <asp:CheckBox ID="CheckBox_Lista_Campo_ruta" CssClass="h6" runat="server"  Text="" />
                                         <span class="ml-2">El campo se muestra en la lista de tareas en espera</span>
                                    </div>
                                </div>
                                <div class="row w-100">
                                    <div class="col-12 p-2">
                                        <asp:CheckBox ID="CheckBox_Ordena_La_lista" runat="server" CssClass="h6" Text=" " />
                                        <span class="ml-2">Campo por el cual se ordena la lista de tareas en espera</span>
                                    </div>
                                </div>
                                <div class="row w-100">
                                    <div class="col-12 p-2">
                                        <asp:CheckBox ID="CheckBox_Campo_Prioridad_Lista" runat="server" CssClass="h6" Text="" />
                                        <span class="ml-2">El campo es prioritario en la lista de tareas en espera</span>
                                    </div>
                                </div>
                                <input id="Hidden_estado_configura_campo_lista" type="hidden" value="" runat="server"/>
                                
                            </ContentTemplate>
                        </asp:UpdatePanel>
                         
                </div>
                <div class="modal-footer justify-content-end">
                     <asp:UpdatePanel ID="UpdatePanel_configura_campo_ruta_boton" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                 <asp:Button ID="Button_actualiza_campo_lista" runat="server" Text="Aceptar" CssClass="btn btn-success" />
                            </ContentTemplate>
                      </asp:UpdatePanel>     
                </div>
                </div>
                <div style="display: none">
                    <asp:Button ID="Button_configura_campo_lista" CssClass="invisible" runat="server" Text="Button" Height="0px" Width="0px" />
                    <asp:Button ID="ButtonSalir_configura_campo_lista" CssClass="invisible" runat="server" Text="Button" Height="0px" Width="0px" />
                    <asp:Button ID="Button_cerrar_configura_campo_lista" runat="Server" Text="X" CssClass="modal_boton_hiden" />
                </div>
                
                            
            </asp:Panel>
        </div>
        <!--configura_listado_ruta-->
          <div id="configura_listado_ruta">
            <asp:Panel ID="Panel_configura_listado_ruta" runat="server" Style="display:none; color: White; width:40%" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_configura_listado_ruta" runat="server" BehaviorID="Panel_configura_listado_ruta" TargetControlID="ButtonSalir_configura_listado_ruta" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_configura_listado_ruta" PopupControlID="Panel_configura_listado_ruta" ></asp:ModalPopupExtender>
                <div class="modal-content">
                <div id="divcabecer2_configura_listado_ruta"  class="modal_title_superior_ modal-header">               
                  <h6 class="modal-title d-inline ">Usuarios relacionados</h6>
                    <button type="button" value="Button_cerrar_configura_listado_ruta" class="close da_event_captive ">&times;</button>
                </div>
                <div id="contenido_procesa_configura_listado_ruta" style="background-color: white" class="modal_content_back modal-body">                  
                        <asp:UpdatePanel ID="UpdatePanel_configura_listado_ruta" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <input id="Hidden_estado_configura_listado_ruta" type="hidden" value="" runat="server"/>
                                <div class="row">
                                    <div class="col-6 p-2">
                                         <asp:Label ID="Label_configuracion_listado_ruta" CssClass="h6" runat="server" Text="Orden del listado de la ruta" ></asp:Label>
                                    </div>
                                    <div class="col-6 p-2">
                                        <asp:DropDownList ID="DropDownList_configuracion_listado_ruta"  CssClass="form-control" runat="server"></asp:DropDownList>
                                    </div>
                                </div>
                                
                            </ContentTemplate>
                        </asp:UpdatePanel>
                  
                </div>
                    <div class="modal-footer justify-content-end">
                        <asp:UpdatePanel ID="UpdatePanel_buton_procesa_configura_listado_ruta" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>

                                <asp:Button ID="Button_actualiza_configuracion_ruta" runat="server" Text="Aceptar" CssClass="btn btn-success" />

                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
                <div style="display:none">
                     <asp:Button ID="Button_configura_listado_ruta" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="ButtonSalir_configura_listado_ruta" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="Button_cerrar_configura_listado_ruta" runat="Server" Text="" CssClass="modal_boton_hiden" Height="0px" Width="0px"
                              />
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
