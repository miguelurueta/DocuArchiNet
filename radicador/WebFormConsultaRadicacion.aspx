 <%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormConsultaRadicacion.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormConsultaRadicacion" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    
    <title> Consulta radicados gestión</title>
       <script src="../js/ui/jquery-3.4.1.min.js"></script>   
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.11.0/umd/popper.min.js" type="text/javascript"></script
    <link href="../bootstrap/css/bootstrap.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
     <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
    <script src="../js/java_general/gestion_meta_dato.js" type="text/javascript"></script>  
     <script src="../js/radicacion/WebFormConsultaRadicacion.js"></script>
    <script src="../js/java_general/general_control_java.js"></script>
    <script src="../generic_control/FileUploadHandler.js" type="text/javascript"></script>
    <link href="../generic_control/UploadFile.css" rel="stylesheet" />
     <script src="../js/java_general/GredviewControl.js"></script>
     <script src="../js/java_general/JS_firma_digital.js" type="text/javascript"></script>
    <script src="../js/java_general/row_multiple_gred.js" type="text/javascript"></script>
    <script src="../js/java_general/JSProgresBar.js"></script>
    <script src="../js/java_general/JSReplaceScanFile.js"></script>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-table@1.23.1/dist/bootstrap-table.min.css"/>
    <script src="https://cdn.jsdelivr.net/npm/tableexport.jquery.plugin@1.29.0/tableExport.min.js" type="text/javascript"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap-table@1.23.1/dist/bootstrap-table.min.js" type="text/javascript"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap-table@1.23.1/dist/bootstrap-table-locale-all.min.js" type="text/javascript"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap-table@1.23.1/dist/extensions/export/bootstrap-table-export.min.js" type="text/javascript"></script>
    <script src="../js/versiondocumento/gestion_version_documento.js"></script>
    <script src="../js/table_boo/table_boot_config.js" type="text/javascript"></script>
    <script src="../js/java_general/BootstrapTable.js" type="text/javascript"></script>
    <script src="../js/MyJavaScriptFile.js"></script> 
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <script src="../js/validate_campos.js"></script> 
     <script src="../Awesome/js/all.js"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet"/>
  <link href="../Awesome/css/brands.css" rel="stylesheet"/>
  <link href="../Awesome/css/solid.css" rel="stylesheet"/>
    <script  src="../Awesome/js/brands.js"></script>
  <script  src="../Awesome/js/solid.js"></script>
  <script  src="../Awesome/js/fontawesome.js"></script>
</head>
<body>
    <form id="formconsultaradicacion" runat="server" >
            <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" EnablePageMethods="true" AsyncPostBackTimeout="900">
            </asp:ScriptManager>
            <script accesskey="javascript" type="text/javascript">
                Sys.WebForms.PageRequestManager.getInstance().add_initializeRequest(InitializeRequest);
                Sys.Application.add_load(ApplicationLoadHandler)
                var elment_postbak;
                function ApplicationLoadHandler(sender, args) {

                    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(CheckStatus);

                }
                function InitializeRequest(sender, args) {
                    
                    try {
                    elment_postbak = args.get_postBackElement();
                    if (args.get_postBackElement().id == 'Button_actualiza_tipo_tramite') {
                        var doc = document.getElementById("Hidden_id_control_postback");
                        doc.value = "Button_actualiza_tipo_tramite";

                    }
                    if (args.get_postBackElement().id == 'Button_consulta_val_radicacion') {
                        document.getElementById("Button_consulta_val_radicacion").disabled = true;
                        document.getElementById("Button_consulta_val_radicacion").value = "Espere....";
                    }
                    posicion_update_pogres('progres_bar');
                    }
                    catch (err) {
                        alert(err.message + " Funcion InitializeRequest");
                    }
                }
                function CheckStatus(sender, args) {
                    try {
                    progres_hiden('progres_bar');
                    var doctra = document.getElementById("Hidden_resultado_tipo_tramite");
                    var id_buton = document.getElementById("Hidden_id_control_postback");
                    var hiden_dianmico = document.getElementById("Hidden_buton_seleccion_edita_dinamico");                   
                    if ( elment_postbak.id == "Button_actualiza_tipo_tramite") {                      
                       
                        if (document.getElementById("Hidden_campos_dinamicos_edita").value !== "") {
                            actualiza_gre_campos_dinamicos();
                            document.getElementById("Hidden_campos_dinamicos_edita").value = "";
                        }
                    }
                    if (elment_postbak.id == "Button_editar_campos_dinamicos_consulta") {
                        auto_zise_popup_campo_dinamico();
                    }
                    if (elment_postbak.id == 'Button_consulta_val_radicacion') {
                        document.getElementById("Button_consulta_val_radicacion").disabled = false;
                        document.getElementById("Button_consulta_val_radicacion").value = "Consultar";
                        if (document.getElementById("Hidden_resultado_consulta").value == "YES") {
                            document.getElementById("Hidden_resultado_consulta").value == "";
                            //plugin_grwedview();
                        }
                        
                    }
                    if (elment_postbak.id == 'Button_consulta_val_radicacion_general') {
                        document.getElementById("Button_consulta_val_radicacion_general").disabled = false;
                        document.getElementById("Button_consulta_val_radicacion_general").value = "Consultar";
                        if (document.getElementById("Hidden_resultado_consulta").value == "YES") {
                            document.getElementById("Hidden_resultado_consulta").value == "";
                            //plugin_grwedview();
                        }

                    }
                    if (hiden_dianmico.value == "Button_edita_campos_dinamicos") {
                      
                        if (document.getElementById("Hidden_campos_dinamicos_edita").value !== "") {
                            actualiza_gre_campos_dinamicos();
                            document.getElementById("Hidden_campos_dinamicos_edita").value = "";

                        }
                    }
                    if (elment_postbak.id == "Button_visor_emergente") {
                        document.getElementById("Label12").innerHTML = "Visor de documentos";
                        auto_zise_popup_visor_externo();
                    }
                    if (elment_postbak.id == "Button_log") {
                        document.getElementById("Label14").innerHTML = "Visor de documentos";
                        auto_zise_popup_detalle_transacciones();
                    }
                        //Button_log
                    
                    if (elment_postbak.id == "Button_Trazabilidad") {
                        document.getElementById("Label12").innerHTML = "Trazabilidad de radicados";
                        auto_zise_popup_visor_externo();
                    }
                    if (elment_postbak.id == "Button_Log_respuesta") {
                        document.getElementById("Label12").innerHTML = "Transacciones de respuestas";
                        auto_zise_popup_visor_externo();
                    }
                    if (elment_postbak.id == "Button_detalle_radicado") {
                        document.getElementById("Label12").innerHTML = "Detalle radicado";
                        auto_zise_popup_visor_externo();
                    }
                    if (elment_postbak.id == "Button_log") {
                        document.getElementById("Label14").innerHTML = "Transacciones del radicado";
                        auto_zise_popup_detalle_trazabilidad();
                    }
                    if (elment_postbak.id == "Button_Log_respuesta") {
                        document.getElementById("Label14").innerHTML = "Transacciones de la respuesta";
                        auto_zise_popup_detalle_trazabilidad();
                    }
                    if (elment_postbak.id == "Button_detalle_radicado") {
                        document.getElementById("Label14").innerHTML = "Detalle respuesta radicado";
                        auto_zise_popup_detalle_trazabilidad();
                    }
                    if (elment_postbak.id == "Button_notificar_envio") {
                        actuo_zise_popup_compartir_correo_electronico();
                    }
                       
                    if (elment_postbak.id == "Button_actualizar_entrantres" && document.getElementById("Hidden_resultado_campo_estatico").value == "TRUE") {
                        
                        if (document.getElementById("Hidden_campos_dinamicos_edita").value !== "") {
                            actualiza_gre_campos_dinamicos();
                            document.getElementById("Hidden_campos_dinamicos_edita").value = "";
                            document.getElementById("Hidden_resultado_campo_estatico").value = "FALSE";
                        }
                    }
                    if (elment_postbak.id == "Button_buscar_lista") {
                        busqueda_gred('hdnEmailID_VAL', 'GridView_val_radicacion', 'TextBox_busqueda', 'CheckBox_busqueda');
                    }
                    if (elment_postbak.id == "Button_Editar_radicados") {
                        auto_zise_popup_editar_radicados();
                    }
                    if (elment_postbak.id == "Button_actualiza_salientes" ) {
                        //actualiza_grid_campos_fijos();   
                        // document.getElementById("Hidden_resultado_campo_estatico").value = "FALSE";
                        // auto_zise_popup_validacion_radicados();
                        if (document.getElementById("Hidden_campos_dinamicos_edita").value !== "") {
                            document.getElementById("Hidden_campos_dinamicos_edita").value = "";
                            document.getElementById("Hidden_resultado_campo_estatico").value = "FALSE";
                            actualiza_gre_campos_dinamicos();
                            
                        }
                    }
                    if (elment_postbak.id == 'Button_actualiza_indice') {
                        if (document.getElementById("Hidden_campos_dinamicos_edita").value !== "") {
                            actualiza_gre_campos_dinamicos();
                            document.getElementById("Hidden_campos_dinamicos_edita").value = "";

                        }
                    }
                    if (elment_postbak.id == "Button_tool_visualiza_documento") {
                        if (document.getElementById("Hidden_result_boton_tool").value == "YES") {
                            document.getElementById("Hidden_result_boton_tool").value = "";
                            dispalyVisorEmergente();
                        }
                    }
                    if (elment_postbak.id == 'Button_guardar_desicion') {
                        if (document.getElementById("Hidden_result_load").value == "YES") {
                            document.getElementById("Hidden_result_load").value = "";
                            insert_row_documento_relacionado(document.getElementById("Hidden_date_row").value,"rad");
                            document.getElementById("Hidden_date_row").value = "";
                        }
                    }
                    if (elment_postbak.id == 'ButtonAlmacenar') {
                        if (document.getElementById("Hidden_result_load_").value == "YES") {
                            document.getElementById("Hidden_result_load_").value = "";
                            insert_row_documento_relacionado(document.getElementById("Hidden_date_row_").value, "rad");
                            document.getElementById("Hidden_date_row_").value = "";
                        }
                    }
                        if (elment_postbak.id == 'Button_tool_elimina_documento') {
                            
                        if (document.getElementById("Hidden_result_boton_tool").value == "YES") {
                            document.getElementById("Hidden_result_boton_tool").value = "";
                            dispalyInterfaceEscaner();
                            eliminar_fila_data_gred_simple_('GridView_list_documento_relacion', 'hiden_seleccion_documento_id', 'hiden_seleccion_documento', '-1', '');
                        }
                    }
                    if (elment_postbak.id == 'Button_actualiza_tipologia_documental') {
                        if (document.getElementById("Hidden_resulta_botno_tipologia_documental").value != "") {
                            update_Cell_AspNetGred('GridView_list_documento_relacion', document.getElementById("hiden_seleccion_documento_id").value, document.getElementById("Hidden_resulta_botno_tipologia_documental").value, 'DOCUMENTO', 'id_rad');
                            //actualiza_gre_campo_lista('GridView_list_documento_relacion', document.getElementById("hiden_seleccion_documento_id").value, document.getElementById("Hidden_resulta_botno_tipologia_documental").value, 'DOCUMENTO');
                            document.getElementById("Hidden_resulta_botno_tipologia_documental").value = "";
                        }
                    }
                }
                    catch (err) {
                        alert(err.message + " Funcion CheckStatus");
                }
                }

            </script>
           
            <asp:UpdatePanel ID="Updatepanel_actualiza" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Button ID="Button_actualiza_indice" runat="server" Text="Button" Style="display: none" />
                    <input id="Hidden_buton_seleccion_edita_dinamico" type="hidden" value="" runat="server"/>
                    <input id="Hidden_campos_dinamicos_edita" type="hidden" value="" runat="server"/>
                    <input id="hidden_campos_dinamicos_aleas" type="hidden" value="" runat="server"/>
                    <input id="hidden_valore_campos" type="hidden" value="" runat="server" />                  
                </ContentTemplate>
            </asp:UpdatePanel>
            <input id="Hidden_resultado_web_service" type="hidden" value="YES" runat="server"/>
            <input id="Hidden_alert_respuesta" type="hidden" value="YES" runat="server"/>
            <input id="Hidden_height" type="hidden" value="0" runat="server"/>
            <input id="Hidden_width" type="hidden" value="0" runat="server"/>
      <div id="contenguia">
           <div id="div_error_content_general" style="position: relative; width: 100%"></div>
            <nav id="menu_var" class="navbar navbar-expand-sm nav_botota_person modal_content_no_back_inferior">
                <button id="nav_togle_display" class="navbar-toggler" type="button" style=" background-color:#6d7fcc" data-toggle="collapse" data-target="#navbarNavDropdown">
                   <span class="navbar-toggler-icon_"><i style="color:white" class="fad fa-th-list"></i></span>
               </button>
                <div class="collapse navbar-collapse row" id="navbarNavDropdown">
                    <ul class="navbar-nav col-md-8"> 
                         <li class="nav-item dropdown active ml-2 active_">                  
                            <a class="nav-link dropdown-toggle bot_hover_person" style="color:#6d7fcc" href="#" id="navbarDropdownMenuLink" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"> <i style="color:#0062cc" class="fad fa-bars"></i> Menú
                            </a>
                            <div class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink">  
                                 <a href="#" class="dropdown-item" onclick="activa_menu('i_r_r_001')"><i class="fal fa-print"></i> Imprmir rótulo del radicado seleccionado</a>
                                 <a href="#" class="dropdown-item" onclick="activa_menu('e_r_r_002')"><i class="fal fa-file-export"></i> Exportar los resultados de la lista de radicados</a>    
                                 <a href="#" class="dropdown-item" onclick="activa_menu('d_d_r_003')"><i class="fal fa-arrow-down"></i> Descarga certificado del radicado seleccionado</a>                                                                   
                            </div>
                        </li>
                        <li class="nav-item dropdown active ml-2 mr-0 active_">
                           <a class="nav-link  dropdown-toggle" style="color: #6d7fcc" href="#" id="A3" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc" class="fal fa-edit "></i> Edición
                           </a>
                           <div class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">
                                <a href="#" class="dropdown-item" onclick="activa_menu('e_c_b_004')"><i class="fal fa-money-check-edit"></i> Editar campos  del radicado seleccionado</a>
                                <a href="#" class="dropdown-item" onclick="activa_menu('c_t_t_006')"><i class="fal fa-exchange"></i> Cambiar el tipo de trámite del radicado seleccionado </a>  
                                <a href="#" class="dropdown-item" onclick="activa_menu('d_r_r_010')"><i class="fad fa-folder-open"></i> Gestionar documentos del radicado seleccionado </a>                    
                           </div>
                       </li>
                        <li class="nav-item dropdown active ml-2 mr-0 active_  ">
                            <a class="nav-link  dropdown-toggle" style="color: #6d7fcc" href="#" id="A1" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc" class="fal fa-file-search"></i> Consultas
                           </a>
                            <div class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">
                                <a href="#" class="dropdown-item" onclick="activa_menu('t_r_s_006')"><i class="fal fa-list-alt"></i> Transacciones realizadas para dar respuesta al radicado seleccionado</a>
                                <a href="#" class="dropdown-item" onclick="activa_menu('t_r_s_020')"><i class="fal fa-list-ol"></i> Transacciones realizadas sobre el radicado seleccionado</a>
                                <a href="#" class="dropdown-item" onclick="activa_menu('e_r_s_007')"><i class="fal fa-project-diagram"></i> Estados del radicado seleccionado</a>
                                <a href="#" class="dropdown-item" onclick="activa_menu('d_r_s_008')"><i class="fal fa-table"></i> Detalles de la respuesta del radicado seleccionado</a>
                                <a href="#" class="dropdown-item" style="display: none" onclick="activa_menu('l_t_r_009')"> Detalle de las modificaciones realizadas al radicado seleccionado</a>
                               
                            </div>
                        </li> 
                         <li class="nav-item dropdown active ml-2 mr-0 active_">
                            <a class="nav-link  dropdown-toggle" style="color: #6d7fcc" href="#" id="A2" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc" class="fal fa-flag-alt"></i> Notificaciones 
                           </a>
                            <div class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">
                                  <a href="#" class="dropdown-item" style="display:none" onclick="activa_menu('n_u_a_010')"><i class="fal fa-flag-alt"></i> Notificar a otros usuarios el radicado seleccionado</a>
                                  <a href="#" class="dropdown-item" onclick="activa_menu('n_u_c_011')"><i class="fal fa-envelope-square"></i> Notificar a correo electrónico el radicado seleccionado</a>
                                  
                           </div>
                        </li>
                    </ul>
                    <div class=" float-md-right col-md-4 float-sm-left">
                        <div class="input-group ">
                            <button id="td-boton" class="btn btn-outline-secondary border-right-2 " title="Restaura busqueda por campos" style="border-top-right-radius: 0px; border-bottom-right-radius: 0px" onclick="preven_event_restor_search(event,this)" type="button">
                                <i class="fal fa-long-arrow-left"></i>
                            </button>
                            <asp:TextBox ID="TextBox_buequeda_general" runat="server"  CssClass="form-control form-control-sm complex  border-left-0" placeholder="Busqueda...." onkeypress="acti_busq_general_archivo(event,this)"></asp:TextBox>
                            <div class="input-group-append">
                                <button class="btn btn-outline-secondary" onclick="acti_busq_general_archivo_boton(event, this)" type="button">
                                    <i class="fal fa-search"></i>
                                </button>
                            </div>
                        </div>
                    </div>     
                </div>
            </nav>
            <a id="da_show-sidebar_" class="btn btn-sm   hide_da_sidebar " href="#" data-target="#sidebar_">
                <i style="color: white" class="fas fa-bars"></i>
            </a>
        <div id="da_content_wraper" class="ml-0 mr-2  d-flex " style="padding-left: 1px; padding-right: 1px">
            <div id="Contentizquierdo" style="width: 25%; float: left">
                <nav id="sidebar_" class=" bg-light pl-0 pr-0 " style="width: 100%">
                    <div id="contenido_titulo_controles_consulta" class="modal-header modal_title_superior bg-light" style="border-top-left-radius: initial; border-top-right-radius: initial">
                        <h6 class=" mt-2 mb-2 ml-2 font-weight-light" id="pit_" style="color: black; float: left; font-family: 'Segoe UI'">Campos de busqueda </h6>
                        <a id="sidebarCollapse" class="close_ mr-1" style="float: right; color: black; height: 10px">&times;</a>
                    </div>
                    <div id="contenido_controles_consulta" style="width: 100%">
                        <asp:UpdatePanel ID="UpdatePanelContenido_val_radicacion" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Panel ID="_Panelvalidacion_val_radicacion" runat="server" Height="100%" Width="100%" Wrap="false" Style="overflow: auto; background-color: white" DefaultButton="Button_consulta_val_radicacion" CssClass="pl-2 pb-2">
                                    <asp:Table ID="_ValidacionConsulta_val_radicacion" runat="server" ForeColor="White" BackColor="White" ViewStateMode="Enabled" Wrap="false" Width="100%" Style="background-color: white">
                                    </asp:Table>
                                </asp:Panel>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div id="contenido_controles_buton_consulta" style="border-top-left-radius: initial; border-top-right-radius: initial" class="modal-header    justify-content-start">
                        <asp:UpdatePanel ID="UpdatePanel_botones_validacion" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <input id="Hidden_resultado_consulta" type="hidden" value="" runat="server"/>
                                <asp:Button ID="Button_consulta_val_radicacion" runat="server" Text="Consultar" ToolTip="Consultar radicados" CssClass="btn btn-success" />
                                <asp:Button ID="Button_lipiar_val_radicacion" Text="Limpiar" runat="server" ToolTip="Limpiar campos radicacion" CssClass="btn btn-success" />
                                <asp:Button ID="Button_consulta_val_radicacion_general" runat="server" Text="Consultar" ToolTip="Consultar radicados" CssClass="btn btn-success" Style="display: none" />
                                <asp:Button ID="Button__consulta_val_radicacion_rest" runat="server" Text="Consultar" ToolTip="Consultar radicados" CssClass="btn btn-success" Style="display: none" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </nav>
            </div>
            <div id="Contenedorderecho" class=" mr-0 ml-0 pl-1 pr-1 pb-0 pt-0  " style="width: 75%; float: right">
                <div id="contenido_titulo_val_radicacion" class=" p-2">
                    <asp:UpdatePanel ID="UpdatePanelabel_val_radicacion" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Label ID="titulo_label_val_radicacion" runat="server" CssClass="h6 font-weight-light p-1" Style="font-family: 'Segoe UI'" Text="Resultados"></asp:Label>
                            <asp:Label ID="Label_estado_transac" runat="server" Text="" Style="font-size: 8px; font-family: Arial; float: right"></asp:Label>
                            <input id="hdnEmailID_VAL" type="hidden" value="-1" runat="server"/>
                            <input id="Hidden_consecutivo_radicado" type="hidden" value="-1" runat="server"/>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div id="contenido_datagrid_val_radicacion" style="height: 60%; width: 100%; position: relative; margin-top: 1px; overflow: auto">
                    <asp:UpdatePanel ID="UpdatePanel_conenido_grid_val_radicacion" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <input id="Hidden_offset" type="hidden" value="" runat="server"/>
                            <asp:GridView ID="GridView_val_radicacion" runat="server" AllowSorting="true" AllowPaging="true" EnableViewState="true"
                                PageSize="7" PagerSettings-Position="Top" Style="font-family: Segoe UI"
                                AutoGenerateSelectButton="False" CssClass="table font-weight-light  " GridLines="None">
                                <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                <HeaderStyle CssClass="GridviewScrollHeader_line_boot" BorderStyle="None" />
                                <RowStyle CssClass="" />
                                <PagerStyle CssClass="pagination-ys" />
                                <Columns>
                                    <asp:BoundField HeaderText="OPCIONES" />
                                </Columns>

                            </asp:GridView>

                        </ContentTemplate>
                        <Triggers>
                        </Triggers>
                    </asp:UpdatePanel>
                </div>

                <div id="Contenido_botones_tipo_radicado" style="height: 10%; width: 100%; background-color: #E7EDF5; float: left; overflow: auto; display: none">

                    <div id="contennido_buton" style="width: 100%; height: 100%; display: none">
                        <asp:UpdatePanel ID="UpdatePanel_botones_radicacion" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>

                                <asp:Button ID="Button_Reimpresion_radicado" runat="server" Text="Reimprime" Width="70px" ToolTip="Reimprime rotulo radicado" CssClass="boton_blanco" Style="font-size: 11px; background-color: white; height: 20px;" />

                                <asp:Button ID="Button_Editar_radicados" Text="Campos basicos" runat="server" ToolTip="Editar campos estandar de la plantilla" CssClass="boton_blanco" Style="font-size: 11px; background-color: white; width: auto" OnClientClick="auto_zise_popup_editar_radicados();" />

                                <asp:Button ID="Button_editar_campos_dinamicos_consulta" Text="Campos dinamicos" runat="server"
                                    ToolTip="Editar campos adicionales" CssClass="boton_blanco" Style="font-size: 11px; background-color: white; width: auto" />

                                <asp:Button ID="Button_editar_tipo_tramite" Text="Clase tramite" runat="server" ToolTip="Tipo tramite documento" CssClass="boton_blanco" Style="font-size: 11px; background-color: white; width: auto" />

                                <asp:Button ID="Button_Exportar_Radicados" Text="Exportar" runat="server" ToolTip="Exportar lista"
                                    OnClientClick="retorna_colum_mtriz('Hidden_colum_header');" Style="font-size: 11px; background-color: white;" CssClass="boton_blanco" />
                                
                                <asp:Button ID="Button_Trazabilidad" runat="server" Text="Button" />

                                <asp:Button ID="Button_notificar_envio" Text="Notificar" runat="server" Width="55px" ToolTip="Notificar al correo a usuario remitente" CssClass="boton_blanco" Style="font-size: 11px; background-color: white; height: 20px;" />

                                

                                <asp:Button ID="Button_Log_respuesta" runat="server" Text="Transacciones" ToolTip="Transacciones realizadas para la respuesta" CssClass="boton_blanco" Style="font-family: arial; font-size: 11px; background-color: white; width: auto" />

                                <asp:Button ID="Button_detalle_radicado" Text="Detalle" runat="server" Width="50px" ToolTip="Muestra los detalles del radicado" CssClass="boton_blanco" Style="font-family: arial; font-size: 11px; background-color: white" />

                                <asp:Button ID="Button_log" Text="Log" runat="server" Width="50px" ToolTip="Muestra del radicado" CssClass="boton_blanco" Style="font-family: arial; font-size: 11px; background-color: white" />
                                <asp:Button ID="Button_certificado_radicado" Text="Certificado radicado" runat="server" Width="120px" ToolTip="Certificado radicado" CssClass="boton_blanco" Style="font-family: arial; font-size: 11px; background-color: white" />
                                <asp:Button ID="Button_compartir" Text="Compartir" runat="server" Width="120px" ToolTip="Certificado radicado" CssClass="boton_blanco" Style="font-family: arial; font-size: 11px; background-color: white" />
                                <input id="Hidden_colum_header" type="hidden" value="" runat="server"/>
                                <input id="Hidden_ruta_archivo" type="hidden" value="" runat="server"/>
                                <asp:Button ID="Button_visor_emergente" runat="server" Text="Button" Style="display: none" />
                            </ContentTemplate>

                        </asp:UpdatePanel>
                    </div>


                </div>
            </div>
        </div>
        <div id="cler" style="clear: both"></div>   
        <asp:Panel ID="Panel_admon_documentos" runat="server" Style="display:none;  width:100%; height: auto" CssClass="modal_content_general">
            <div id="div_error_content_rad" style="position: relative; width: 100%"></div>
            <asp:ModalPopupExtender ID="ModalPopupExtender_edition_admon_documentos" runat="server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_admon_documentos"
                CancelControlID="Button_cerrar_admon_documentos" PopupControlID="Panel_admon_documentos">
            </asp:ModalPopupExtender>
                <div id="modal_content_admon_documentos" class="modal_content">
                    <div id="divcabecer2_admon_documentos" class="modal-header">
                        <h6 class="modal-title"> Documentos</h6>
                        <button type="button" value="Button_cerrar_admon_documentos"  class="close da_event_captive close">&times;</button>
                    </div>
                    <div id="contenido_procesa_admon_documentos" style="background-color: white; width: auto; height: auto; border-top: none" class="modal_content_back modal-body_ m-1">
                        <asp:UpdatePanel ID="updatemenu" style="width: 100%" runat="server" UpdateMode="Conditional" RenderMode="Block">
                            <ContentTemplate>
                                <nav id="navar_barra" class="navbar navbar-expand-sm nav_botota_person modal_content_no_back_inferior">
                                    <button class="navbar-toggler" type="button" style="background-color: #6d7fcc" data-toggle="collapse" data-target="#navbarNavDropdown">
                                        <span class="navbar-toggler-icon_"><i style="color: white" class="fad fa-th-list"></i></span>
                                    </button>
                                    <asp:Panel ID="Panel_cargar_archivo" CssClass="navbar-nav " runat="server">
                                        <ul class="navbar-nav">
                                            <li class="nav-item active ml-2 active_">
                                                <a class="nav-link" style="color: #6d7fcc" title="Cargar archivo a la lista" href="#" onclick="inicializa_tipo_adjunto_documento(event,this,'C-DW-RD')"><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-upload"></i> Cargar archivos  </a>
                                            </li>
                                            <li class="nav-item active ml-2 active_">
                                                <a class="nav-link" style="color: #6d7fcc" title="Detalle radicado" href="#" onclick="activa_boton_client_server('Button_tool_activa_detalle_radicado');"><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-ellipsis-v"></i> Detalle radicado  </a>
                                            </li>
                                        </ul>
                                    </asp:Panel>
                                </nav>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div id="conte_waper" class="container-fluid mr-0 ml-0 pl-0 pr-0" style="border-top: 1px solid #e9ecef">
                            <a id="da_show-sidebar__" class="btn btn-sm   hide_da_sidebar " href="#" data-target="#sidebar__">
                                <i style="color: white" class="fas fa-bars"></i>
                            </a>
                            <div id="da_content_wraper_" class="wrapper_ ml-0 mr-0  d-flex  justify-content-between_" style="padding-left: 1px; padding-right: 1px">
                                <div id="Contentizquierdo_" class="bg-light_ " style="width: 22%; float: left">
                                    <nav id="sidebar__" class=" bg-light_ pl-0 pr-0">
                                        <div id="title_treview" class="modal-header_ modal_title_superior " style="border-top-left-radius: initial; border-top-right-radius: initial; border-bottom: 1px solid #e9ecef; border-right: 1px solid #e9ecef">
                                             <div class="row">
                                                 <div class="col-1 pl-3 pt-2 ">
                                                    <asp:CheckBox ID="chk_selec" Text="" CssClass="btn   btn-light btn-sm border-0 bg-transparent" runat="server" onclick="table_gred_on_click_check(this,'GridView_list_documento_relacion','chek_selecion_list_rad');" />
                                                </div>
                                                <div class="col-3 d-flex justify-content-start">
                                                    <asp:UpdatePanel ID="UpdatePanelseleccion_label_documentos" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                                        <ContentTemplate>
                                                            <asp:Label ID="Label_documentos" class=" mt-2 mb-2 ml-2  h8 f font-weight-light" Style="color: #6d7fcc; float: left; font-family: 'Segoe UI'" runat="server" Text="Documentos"></asp:Label>
                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>
                                                </div>
                                                <div class="col-7 p-0 d-flex justify-content-end">
                                                    <div class="nav-item_ active active">
                                                        <a id="sidebarCollapse_" class="nav-link pr-2 pl-2" style="float: right; color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600" title="Cerrar lista"><i class="fad fa-bars"></i></a>
                                                        <a id="sing_multiple_file" class="nav-link pr-2 pl-2" style="color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600; float: right" title="Firmar documentos seleccionados " href="#" ><i style="" class="fad fa-file-signature"></i></a>
                                                        <a id="delete_row_several_rad" class="nav-link pr-2 pl-2" style="color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600; float: right" title="Eliminar documentos" href="#" ><i style="" class="fad fa-trash-alt"></i></a>
                                                        <a class="nav-link pr-2 pl-2" style="color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600; float: right" title="Adjuntar documento" href="#" onclick="inicializa_tipo_adjunto_documento(event,this,'C-DW-RD')"><i style="" class="fas fa-upload "></i></a>    
                                                    </div>   
                                                </div>    
                                            </div>
                                        </div>
                                        <div id="div_treview_archivo" style="width: 100%; border-right: 1px solid #e9ecef">
                                            <asp:UpdatePanel ID="UpdatePanelseleccion_digitalizado" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                                <ContentTemplate>
                                                    <input id="hiden_seleccion_documento" type="hidden" value="" runat="server"/>
                                                    <input id="hiden_seleccion_documento_id" type="hidden" value="-1" runat="server"/>
                                                    <input id="Hidden_numero_doc_rel" type="hidden" value="0" runat="server"/>
                                                    <asp:Panel ID="Paneltreview" runat="server"
                                                        Height="100%" Width="100%" Style="position: inherit; overflow: auto">
                                                        <asp:GridView ID="GridView_list_documento_relacion" runat="server" Style="position: inherit; width: 100%; font-size: 14px"
                                                            AutoGenerateSelectButton="False" AllowSorting="false" AllowPaging="false" PageSize="6" PagerSettings-Position="Top" CssClass="Gridviewtable  font-weight-light" GridLines="None"
                                                            EnableViewState="true">
                                                            <RowStyle CssClass="GridviewRow" />
                                                            <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                                            <HeaderStyle CssClass="GridviewScrollHeader_line_boot_none" />
                                                            <PagerStyle CssClass="pagination-ys" />
                                                            <Columns>
                                                                <asp:TemplateField>
                                                                    <HeaderTemplate>
                                                                        <asp:Panel ID="Panel_che_box_aling_" runat="server" Style="text-align: left">
                                                                           
                                                                            <asp:Label runat="server" Text="" Style="font-weight: 500; color: white" CssClass="ml-1">
                                                                            </asp:Label>
                                                                        </asp:Panel>
                                                                    </HeaderTemplate>
                                                                </asp:TemplateField>
                                                            </Columns>
                                                        </asp:GridView>
                                                        <asp:TreeView ID="TreeViewArchivo" runat="server" CssClass="TreeN  h6" NodeWrap="true" Style="overflow: auto; height: 100%; display: none" EnableViewState="false" EnableClientScript="True"
                                                            PopulateNodesFromClient="False"
                                                            LeafNodeStyle-CssClass="LeafNodeStyle" Font-Size="14px" NodeIndent="10" ExpandDepth="0" CollapseImageUrl="../imagera/minus-square-light_1.png" ExpandImageUrl="../imagera/plus-square-light_1.png" SkipLinkText="">
                                                            <HoverNodeStyle Font-Underline="False" />
                                                            <NodeStyle CssClass="nav-link_ mt-1 mb-1 pl-1  " ForeColor="black"
                                                                VerticalPadding="0px" />
                                                            <SelectedNodeStyle ForeColor="Black" CssClass="node_select_" VerticalPadding="5px" HorizontalPadding="5px" ImageUrl="../Gestion/imagenes/folder-open-regular.png" />
                                                        </asp:TreeView>

                                                    </asp:Panel>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </div>
                                        <div id="contenido_pie" style="border-top-left-radius: initial; border-top-right-radius: initial; display: none" class="modal-header pt-1 pb-1   justify-content-start">
                                            <h6 class="modal-title_ mt-2 mb-2 ml-2   font-weight-light" id="pit" style="color: white"></h6>
                                        </div>
                                    </nav>
                                </div>
                                <div id="Contenedorderecho_" class="page-content mr-0 ml-0 pl-1 pr-1 pb-0 pt-0  " style="width: 78%">
                                    <div id="Are_Digitalizacion" style="width: 100%; height: 100%; float: right; display: block; margin-left: 1px" class="modal_content_back_">
                                        <asp:UpdatePanel ID="UpdatePanel_iframe_digitaliza" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                            <ContentTemplate>
                                                <iframe id="IframeDitaliza_" runat="server" frameborder="0" src="../workflow/WebFormEscan.aspx" width="100%" scrolling="no" height="100%"></iframe>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                    <div id="Area_Visor" style="width: 100%; height: 100%; display: none" class="modal_content_back_">
                                        <div id="div_cerrar" class="modal-header_ modal_title_superior " style="border-top-left-radius: initial; border-top-right-radius: initial; border-bottom: 1px solid #e9ecef">
                                            <h6 id="titel_visor" class="mt-2 mb-2 ml-2  h6 font-weight-light" style="color: #6d7fcc; font-family: 'Segoe UI'; float: left">Visor externo</h6>
                                            <button type="button" title="Cerrar ventana visualizador" onclick="prevent_cerrar(event,this);" class="close mr-1" style="float: right">&times;</button>
                                        </div>
                                        <asp:UpdatePanel ID="UpdatePanelIframevisor" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                            <ContentTemplate>
                                                <iframe id="IframeVisor_" runat="server" frameborder="0" width="100%" scrolling="no" height="100%"></iframe>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer  justify-content-between p-1" id="tab_content_boton">
                            <div class=" d-inline-flex">
                                <asp:UpdatePanel ID="UpdatePanel_title_radicado" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                    <ContentTemplate>
                                        <asp:Label ID="h_radicado_title" runat="server" Text="Radicado :"  CssClass="mt-1 ml-1  h6 font-weight-light" Style="color: #6d7fcc"></asp:Label>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <asp:Label ID="Label1" runat="server" Text="" Style="font-size: 8px; font-family: Arial; float: left"></asp:Label>
                            </div>
                            <div class="float-right">
                                <asp:UpdatePanel ID="UpdatePanel_boton_nuevo_radicado" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                    <ContentTemplate>
                                        <asp:Button ID="Button_nuevo_radicado" runat="server" Visible="false" Text="Nuevo radicado" CssClass="btn btn-success" />
                                        <input id="Hidden_resultado_radic" type="hidden" value="0" runat="server"/>
                                    </ContentTemplate>
                                </asp:UpdatePanel>

                            </div>
                        </div>
                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button_cerrar_admon_documentos" runat="Server" Text="X"
                            ToolTip="Cerrar ventana" />
                        <asp:Button ID="Button_admon_documentos" runat="server" Text="Button" Height="1px" Width="1px" />
                        <asp:Button ID="ButtonSalir_admon_documentos" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                    </div>
                </div>
            </asp:Panel>
         <div style="display:none">
            <asp:UpdatePanel ID="UpdatePanel_boton_tool" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                <ContentTemplate>
                    <input id="Hidden_result_boton_tool" type="hidden" value="" runat="server"/> 
                     <input id="HiddenIdFlujo" type="hidden" value="0" runat="server"/>
                    <input id="HiddenRuta_" type="hidden" value="0" runat="server"/>
                     <input id="hide_ruta" type="hidden" value="" runat="server"/> 
                    <input id="Hidden_id_actividad_envio" type="hidden" value="0" runat="server"/> 
                    <input id="Hidden_id_actividad_disp_envio" type="hidden" value="0" runat="server"/> 
                    <input id="Hidden_id_usuario_envio" type="hidden" value="0" runat="server"/> 
                    <input id="Hidden_id_tarea" type="hidden" value="" runat="server"/>
                    <input id="Hidden_date_row_" type="hidden" value="" runat="server"/> 
                    <input id="Hidden_result_load_" type="hidden" value="" runat="server"/> 
                    <asp:Button ID="Button_tool_visualiza_documento" runat="server" Text="" />
                    <asp:Button ID="Button_tool_elimina_documento" runat="server" Text="" />
                    <asp:Button ID="Button_tool_activa_cambia_tipologia" runat="server" Text="" />
                    <asp:Button ID="Button_tool_activa_sube_documento" runat="server" Text="" />
                    <asp:Button ID="Button_tool_actualiza_lista_relacionados" runat="server" Text="" />  
                    <asp:Button ID="Button_tool_activa_lista_documentos" runat="server" Text="" />  
                    <asp:Button ID="Button_tool_activa_detalle_radicado" runat="server" Text="" /> 
                    <asp:Button ID="ButtonAlmacenar" Text="" runat="server" />          
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        </div>
         <input id="HiddenPROMP" type="hidden" value="1" runat="server"/>
           <!--cargar documento anexo!-->
          <div id="contenido_procesa_sube_documento_adjunto" >
            <asp:Panel ID="Panel_sube_documento_adjunto" runat="server" Style="display:none;  width: 70%; height: 100%" CssClass="modal_content_general_">
                <asp:ModalPopupExtender ID="ModalPopupExtender_sube_documento_adjunto" runat="Server" BackgroundCssClass="FondoAplicacion" 
                    TargetControlID="Button_sube_documento_adjunto"
                    PopupControlID="Panel_sube_documento_adjunto" CancelControlID="Button3_cerrar_adjunta" ></asp:ModalPopupExtender>
                <div id="modal_content_sube_documento_adjunto" class="modal-content">  
                    <div id="Div_cabecera" class="modal_title_superior_ modal-header"> 
                        <h6 class="modal-title d-inline ml-1">Adjuntar</h6>  
                        <button type="button" value="Button3_cerrar_adjunta_" onclick="hide_upload_content('ModalPopupExtender_sube_documento_adjunto');" class="close da_event_captive_ ">&times;</button>                   
                   </div>            
                    <div id="Div_contenido_adjunta" style="height: auto; width: 100%; border-top: none" class="modal_content_back p-2">
                        <div id="content_option_chek_adjunto_doc_visor"> 
                        <asp:UpdatePanel ID="Update_actualiza_adjunta_documento" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:MutuallyExclusiveCheckBoxExtender ID="MutuallyExclusiveCheckBoxExtender1_" runat="server" TargetControlID="Check_anexo_radicado_adj"
                                     Key ="radicado_"></asp:MutuallyExclusiveCheckBoxExtender>
                                    <asp:mutuallyexclusivecheckboxextender id="Mutuallyexclusivecheckboxextender2_" runat="server" targetcontrolid="CheckBox_relacionado_radicado_adj"
                                     Key ="radicado_"></asp:mutuallyexclusivecheckboxextender>  
                                <div class=" row pl-1 pr-1">         
                                    <div class="col-6">
                                       <div class="row">
                                            <div class="col-0 pl-3 pr-2 pb-2 pt-2">
                                                <asp:CheckBox ID="Check_anexo_radicado_adj" runat="server" Text="" Checked="false"  Font-Size="11"  onchange="upload_adjunto_doc_visor_event_cheked_adjunto(event)" style="display:none"  Enabled="true" CssClass="h6 font-weight-light"  />
                                            </div>
                                           <div class="col-8 pl-0  pr-0 pb-2 pt-2">
                                               <asp:Label ID="h_adjunto_adjunto_doc_visor" class="pl-0 font-weight-light h6"  runat="server" style="display:none" Text="Adjuntar como parte del documento"></asp:Label>
                                                   
                                            </div>
                                       </div>
                                        
                                    </div>
                                     <div class="col-6">
                                          <div class="row">
                                              <div class="col-0 pl-0 pr-2 pb-2 pt-2">
                                                   <asp:CheckBox ID="CheckBox_relacionado_radicado_adj" runat="server" Text="" Checked="true" onchange="upload_adjunto_doc_visor_event_cheked_relacion(event)"  Font-Size="11" CssClass="h6 font-weight-light" Enabled="true" />
                                              </div>
                                              <div class="col-8 pl-0  pr-0 pb-2 pt-2">
                                                  <asp:Label ID="h_relacion_adjunto_doc_visor" class="pl-0 font-weight-light h6"  runat="server" Text="Adjuntar como documento relacionado"></asp:Label>
                                                 
                                              </div>
                                          </div>
                                        
                                    </div>
                                </div>                  
                                <div id="content_data_grid_adjunta_documento" class="conten_gred_border_" style="width: 100%">
                                    <asp:DropDownList ID="DropDownList_adjunta_documento" Style="width: 100%" CssClass="custom-select mr-sm-2" runat="server"></asp:DropDownList>
                                </div>
                            </ContentTemplate>
                            <Triggers>
                            </Triggers>
                        </asp:UpdatePanel>
                        </div>
                        <div class="p-2">
                            <div class="row p-2" id="content_boton_adjunto_doc_visor">
                                <div class="col-12 p-0">
                                    <div class="file-select " id="src-file">
                                        <input id="file_element_adjunto_doc_visor" type="file" multiple="multiple"  accept=""  style="width: 100px; height: 40px" name="src-file" class="p-1" contente_file="ModalPopupExtender_sube_documento_adjunto" aria-label="Archivo"  />
                                    </div>
                                    <a id="save_file_element_adjunto_doc_visor" title="Guardar todos los archivos"  class="btn  btn-success" style="opacity: 0; color:white"><i style="color: white" class="fas fa-save "></i> Guardar </a>   
                                    <a id="delete_file_element_adjunto_doc_visor" title="Elminar todos los archivos cargados"  class="btn  btn-danger " style="opacity: 0; color:white"><i style="color: white" class="fal fa-trash-alt "></i> Eliminar </a>
                                     <a id="cancel_file_element_adjunto_doc_visor" title="Cancelar guardar archivos"  class="btn  btn-warning" style="opacity: 0; color:white"><i style="color: white" class="fas fa-window-close "></i> Cancelar </a>
                                </div>
                            </div>            
                            <div class="paren_element background_upload" id="conten_file_element_adjunto_doc_visor" style="overflow: auto; height: 100%">
                                
                                  <div id="content_drop_element_adjunto_doc_visor" claas="">
                                       
                                     
                                 </div>
                                 <table id="table_file_element_adjunto_doc_visor" class="table table-striped">
                                 </table>
                            </div>
                            <div class="row border pt-2" id="content_pie_title_adjunto_doc_visor">
                                <div class="col-8">
                                    <div class="row p-2">
                                        <div class="col-4 p-0">
                                            <div >
                                                <asp:Label ID="Label_progres_bar_file_element_adjunto_doc_visor" runat="server" Text="" Style="font-family: Arial; text-align: center; font-size: 20px"></asp:Label>
                                            </div>
                                            <div id="pogres_file_element_contador_adjunto_doc_visor" style="text-align: center; font-family: Arial; font-size: 14px">
                                            </div>
                                            <div id="pogres_file_element_porcent_adjunto_doc_visor" style="text-align: center; font-family: Arial; font-size: 14px">
                                            </div>
                                        </div>
                                        <div class="col-5 p-0">
                                            <div>
                                                <div id="myProgress_file_element_adjunto_doc_visor">
                                                    <div id="myBar_file_element_adjunto_doc_visor" class="file-select-bar"></div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-3 p-0 pl-3">
                                            <p id="count_byte_file_element_adjunto_doc_visor"></p>    
                                        </div>
                                    </div>
                                    
                                </div>
                                <div class="col-4 justify-content-end pt-2">
                                    <p id="count_file_element_adjunto_doc_visor" class="font-weight-light" style="float: right"> Estado </p>
                                </div>
                            </div>
                        </div>
                        
                </div>
                </div>
                 <asp:Panel ID="Panel_descarga_ajax" runat="server">               
                            <div id="drop_zone_" style="width: 100%; height: auto; display: none">
                                <asp:UpdatePanel ID="UpdatePanel_descarga" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                         <asp:Button ID="Button_update_update_adjunto_doc_visor" runat="server" Text="Button" />
                                        <asp:AjaxFileUpload ID="AjaxFileUpload_dowload" runat="server" ThrobberID="drop_zone_"
                                            ContextKeys="fred"
                                            AllowedFileTypes="tif,jpg,tiff,bmp,pdf"
                                            MaximumNumberOfFiles="1" OnClientUploadComplete="activa_boton_dowload" />
                                        <asp:Button ID="Button_guardar_desicion" runat="server" Text="Button" Style="display: none" />
                                        &nbsp  
                                        <asp:Label ID="Label_estado_carga" runat="server" Text="Estado" Style="font-size: 10px" CssClass="font-weight-light h6"></asp:Label>
                                        <input id="Hidden_result_load" type="hidden" value="" runat="server" />
                                        <input id="Hidden_tip_adjunt" type="hidden" value="" runat="server" />
                                        <input id="Hidden_date_row" type="hidden" value="" runat="server" />
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </asp:Panel>
                 <div style="display: none; height: 1px">
                            <asp:Button ID="Button_sube_documento_adjunto" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                            <asp:Button ID="Button3_cerrar_adjunta" runat="Server" Text="" CssClass="invisible" />
                        </div>
            </asp:Panel>     
        </div>
        <asp:Panel ID="Panel_actualiza_tipologia_documental" runat="server" Style="display:none; width: 40%; height: auto" CssClass="modal_content_general_">
            <asp:ModalPopupExtender ID="ModalPopupExtender_edition_actualiza_tipologia_documental" runat="server"
                TargetControlID="ButtonSalir_actualiza_tipologia_documental" BackgroundCssClass="FondoAplicacion"
                CancelControlID="Button_cerrar_actualiza_tipologia_documental" PopupControlID="Panel_actualiza_tipologia_documental">
            </asp:ModalPopupExtender>     
            <div id="modal_content_actualiza_tipologia_documental" class="modal-content">
                <div id="diver_cabcera_actualiza_tipologia_documental" class="modal_title_superior_ modal-header">
                    <h6 class="modal-title d-inline ">Tipologia documental</h6>
                    <button type="button" value="Button_cerrar_actualiza_tipologia_documental" class="close da_event_captive ">&times;</button>
                </div>
                <div id="contenido_procesa_actualiza_tipologia_documental" style="background-color: white; width: 100%; height: 100%; border-top: none" class="modal_content_back modal-body">
                    <asp:UpdatePanel ID="Update_actualiza_tipologia_documental" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>                          
                            <div id="content_data_grid_actualiza_tipologia_documental" class="conten_gred_border_" style="width: 100%">
                                <asp:DropDownList ID="DropDownList_tipologia_documental" style="width:100%" CssClass="custom-select mr-sm-2" runat="server"></asp:DropDownList>
                            </div>                           
                        </ContentTemplate>
                        <Triggers>
                        </Triggers>
                    </asp:UpdatePanel>

                    <div style="display: none; height: 1px">
                        <asp:Button ID="ButtonSalir_actualiza_tipologia_documental" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                        <asp:Button ID="Button_cerrar_actualiza_tipologia_documental" runat="Server" Text="X" CssClass="invisible" />
                    </div>
                </div>
                <div class="modal-footer justify-content-end" id="modal-footer_tipologia_documental">
                    <asp:UpdatePanel ID="UpdatePanel_boton_tipologia_documental" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Button ID="Button_actualiza_tipologia_documental" runat="server" Text="Aceptar" CssClass="btn btn-success" />
                            <input id="Hidden_resulta_botno_tipologia_documental" type="hidden" value="" runat="server"/>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
              
            </div>
        </asp:Panel>
        <asp:Panel ID="Panel_detalle_radicado" runat="server" Style="display:none; width: auto; height: auto" CssClass="modal_content_general_">
            <asp:ModalPopupExtender ID="ModalPopupExtender_edition_detalle_radicado" runat="server"
                TargetControlID="ButtonSalir_detalle_radicado" BackgroundCssClass="FondoAplicacion"
                CancelControlID="Button_cerrar_detalle_radicado" PopupControlID="Panel_detalle_radicado">
            </asp:ModalPopupExtender>     
            <div id="modal_content_detalle_radicado" class="modal-content">
                <div id="diver_cabcera_detalle_radicado" class="modal_title_superior_ modal-header">
                    <h6 class="modal-title d-inline ">Detalle radicado</h6>
                    <button type="button" value="Button_cerrar_detalle_radicado" class="close da_event_captive ">&times;</button>
                </div>
                <div id="contenido_procesa_detalle_radicado" style="background-color: white; width: 100%; height: 100%; border-top: none" class="modal_content_back modal-body">
                    <asp:UpdatePanel ID="Update_detalle_radicado" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Panel ID="Panel_detalle_radicado_user" runat="server" Style="height: 100%; width: 99.8%; margin-bottom: 1px; overflow:auto" CssClass="container">
                                <asp:Table ID="Table_detalle_radicado" runat="server" Style="height: 100%; width: 100%; font-size:12px" CssClass="table table-hover" >
                                    <asp:TableRow>
                                        <asp:TableCell>
                                            <asp:Label ID="Label2" runat="server" Text="RADICADO DEL TRAMITE"></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:Label ID="Label_RADICADO_TRAMITE" runat="server" Text=""></asp:Label>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                            <asp:Label ID="Label19_" runat="server" Text="PETICIONARIO O SOLICITANTE"></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:Label ID="LabelDESTINATARIO" runat="server" Text=""></asp:Label>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                            <asp:Label ID="Label8" runat="server" Text="TIPO TRÁMITE RADICADO"></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:Label ID="Label_TIPO_TRAMITE" runat="server" Text=""></asp:Label>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                            <asp:Label ID="Label6" runat="server" Text="FECHA DE RADICACION DEL TRAMITE"></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:Label ID="LabelFECHA_REGISTRO" runat="server" Text=""></asp:Label>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                            <asp:Label ID="Label17" runat="server" Text="FECHA VENCIMIENTO DEL TRAMITE"></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:Label ID="Label_FECHA_VENCE" runat="server" Text=""></asp:Label>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                            <asp:Label ID="Label_asunto_radicado" runat="server" Text="ASUNTO"></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:Label ID="Label_ASUNTO_RADICADO_" runat="server" Text=""></asp:Label>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                            <asp:Label ID="Label7" runat="server" Text="FLUJO RADICADO"></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:Label ID="Label_FLUJO_RADICADO" runat="server" Text=""></asp:Label>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                            <asp:Label ID="Label20" runat="server" Text="USUARIO RADICADOR "></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:Label ID="Label_radicador_usuario" runat="server" Text=""></asp:Label>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                            <asp:Label ID="Label11" runat="server" Text="CARGO USUARIO RADICADOR"></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:Label ID="Label_CARGO_USUARIO_RADICADOR" runat="server" Text=""></asp:Label>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                            <asp:Label ID="Label16" runat="server" Text="SEDE USUARIO RADICADOR"></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:Label ID="Label_SEDE_USUARIO" runat="server" Text=""></asp:Label>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                </asp:Table>
                            </asp:Panel>
                        </ContentTemplate>
                        <Triggers>
                        </Triggers>
                    </asp:UpdatePanel>

                    <div style="display: none; height: 1px">
                        <asp:Button ID="ButtonSalir_detalle_radicado" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                        <asp:Button ID="Button_cerrar_detalle_radicado" runat="Server" Text="X" CssClass="invisible" />
                    </div>
                </div>
                <div class="modal-footer justify-content-end" id="modal-footer_detalle_radicado">
                    <asp:UpdatePanel ID="UpdatePanel_boton_detalle_radicado" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                           
                            
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
              
            </div>
        </asp:Panel>
        <div id="inferior_bajo_boton" style="width: 0%; height: 0%; background-color: #E7EDF5; display: none">
            <asp:UpdatePanel ID="updatapanel_iframe" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <iframe runat="server" id="ifmExcel_" width="0" height="0" marginheight="0" marginwidth="0"
                        frameborder="0" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 200px">
            <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
            Processing ...
        </div>
         <asp:UpdatePanel ID="UpdatePanel_imp_impresion" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <input id="Hiddendatoradicacion" type="hidden" value="" runat="server"/>
                <input id="Hiddenruta" type="hidden" value="" runat="server"/>
            </ContentTemplate>
        </asp:UpdatePanel>
         <div id="ventanaimpreion">     
            <asp:Panel ID="Panelimpresion" runat="server"  Style="display:none; width: 70%; height: 50%" CssClass="modal_content_general_">
                 <asp:DragPanelExtender ID="DragPanelExtenderimpre" runat="server" TargetControlID="Panelimpresion" />
                 <asp:ModalPopupExtender ID="ModalPopupExtenderimpre" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir"
                     PopupControlID="Panelimpresion" CancelControlID="Buttoncerrarimpre">
                 </asp:ModalPopupExtender>
                <div id="modal_content_Panelimpresion" class="modal-content">
                    <div id="divcabecer2" class="modal_title_superior_ modal-header">
                         <h6 class="modal-title d-inline ">Impresiòn</h6>
                         <button type="button" value="Buttoncerrarimpre" class="close da_event_captive">&times;</button>   
                    </div>
                    <div id="ContenidoImpresion" style=" height: auto; width: auto; border-top:none" class="modal_content_back pl-3 pr-3">
                        <asp:UpdatePanel ID="UpdatePaneliframe" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <iframe style="width:100%; height:100%" id="ifimpre" runat="server" src="../radicador/WebFormImprimir.aspx" frameborder="0"></iframe>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button1" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" Style="display: none" />
                        <asp:Button ID="ButtonSalir" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" Style="display: none" />
                        <asp:Button ID="Buttoncerrarimpre" runat="Server" Text="" CssClass="invisible" Height="1px" Width="1px" Style="display: none" />
                    </div>
                </div>
                </asp:Panel>
             </div>

        <div id="Impresion_post">
            <asp:Panel ID="Panelimpresionpost" runat="server"  Style="display:none; color: White; width: auto; height: auto">
                <asp:ModalPopupExtender ID="ModalPopupExtenderimpre_post" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_post"
                    PopupControlID="Panelimpresionpost" CancelControlID="Buttoncerrarimpre_post">
                </asp:ModalPopupExtender>
                <div id="divcabecer2_post" class="cabecera2">
                    <asp:Button ID="Button1_post" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_post" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Label ID="Label3" runat="server" Text="Menu Impresion" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton2_post" style="float: right">
                        <asp:Button ID="Buttoncerrarimpre_post" runat="Server" Text="X"
                            ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />

                    </div>
                </div>
                <asp:UpdatePanel ID="UpdatePaneliframe_post" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div id="ContenidoImpresion_post" style="border: thin double #000080; color: black; background-color: #FFFFFF; height: 280px; width: 500px">
                            <iframe width="100%" height="100%" id="ifimpre_post_" runat="server" src="../radicador/WebFormImprimirfiles.aspx" ></iframe>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </asp:Panel>
        </div>
        <!--detalle transacciones-->
           <asp:Panel ID="Panel_transacciones" runat="server" Style="display:none; overflow:hidden; width:95%; height:100%" CssClass="modal_content_general_" >
                  <asp:ModalPopupExtender ID="ModalPopupExtender_transacciones" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_transacciones_dos"
                      PopupControlID="Panel_transacciones"  CancelControlID="ButtonSalir_transacciones">
                  </asp:ModalPopupExtender>
               <div id="modal_content_Panel_transacciones" class="modal-content">  
                  <div id="Cabecerapendiente_transacciones" class="modal_title_superior_ modal-header" > 
                        <h6 id="Label14" class="modal-title d-inline ">Ventana detalle de transacciones</h6>
                        <button type="button" value="ButtonSalir_transacciones" class="close da_event_captive ">&times;</button>                       
                    
                  </div>
                  <div id="Cotenedorpendiente_transacciones" style="height: 90%; width: 100%; overflow:hidden; border-top:none" class="modal_content_back  pl-3 pr-3">
                  
                      <asp:UpdatePanel ID="UpdatePanel_transacciones" runat="server" UpdateMode="Conditional">
                          <ContentTemplate>
                              <iframe id="Iframe_log_transacciones_" runat="server" frameborder="0"  style="width:100%; height:100%; overflow:hidden"></iframe>
                          </ContentTemplate>

                      </asp:UpdatePanel>
                           
                  </div>
                   <div style="display: none; height: 1px">
                       <asp:Button ID="Button_transacciones_dos" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" Style="display: none" />
                       <asp:Button ID="ButtonSalir_transacciones" runat="Server" Text="" ToolTip="" CssClass="invisible" Height="1px" Width="1px" Style="display: none" />
                   </div>
               </div>
              </asp:Panel>
        <div id="validacion_plantilla"> 
            <asp:Panel ID="Panel_valiacion_plantilla" runat="server"  Style="display:none; width: 100%; height: 100%" CssClass="modal_content_general_">
                 <asp:ModalPopupExtender ID="ModalPopupExtender_valiacion_plantilla" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_valiacion_plantilla"
                    PopupControlID="Panel_valiacion_plantilla" CancelControlID="Button_cerrar_validacion_plantilla">
                </asp:ModalPopupExtender>
                <div id="modal_content_validacion_plantilla" class="modal-content">  
                <div id="divcabecer2_validacion_plantilla" class="modal_title_superior_ modal-header"> 
                      <h6 class="modal-title d-inline">Gestion externos</h6>
                      <button type="button" value="Button_cerrar_validacion_plantilla" class="close da_event_captive ">&times;</button>                                    
                </div>
                    <div id="Contenido_validacion_plantilla" style=" height: auto; width: auto; border-top:none; overflow:auto" class="modal_content_back pl-3 pr-3 pt-1 pb-1">
                        <asp:UpdatePanel ID="UpdatePanel_validacion_plantilla" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <iframe width="100%" height="100%" id="Iframe_validacion_plantilla_" runat="server" frameborder="0"></iframe>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div style="display: none; height: 1px">
                        <asp:Button ID="ButtonSalir_valiacion_plantilla" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                        <asp:Button ID="Button_cerrar_validacion_plantilla" runat="Server" Text="" CssClass="invisible" />
                    </div>
                    <div class="modal-footer justify-content-end" id="modal-footer_validacion_plantilla">  </div>
                </div>
             </asp:Panel>
        </div>
        <div id="edita_campos_dinamicos" style="color:white">
            <asp:Panel ID="panel_edita_campos_dinamicos" runat="server" style="display:none; color:white; height:99%;  width:100%" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edita_campos_dinamicos" runat="server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_edita_campos_dinamicos"
                    PopupControlID="panel_edita_campos_dinamicos" CancelControlID="Button_cerrar_edita_campos_dinamicos" Y="1"></asp:ModalPopupExtender>
                 <div id="divcabecer2_edita_campos_dinamicos" class="modal_title_superior">
                    <asp:Label ID="Label10" runat="server" Text="Edita campos dinamicos" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton2_edita_campos_dinamicos" style="float: right">
                        <asp:Button ID="Button_cerrar_edita_campos_dinamicos" runat="Server" Text="X" CssClass="modal_boton_hiden"
                             ToolTip="Cerrar ventana" />
                    </div>
                </div>
                     
                    <div id="campos_edita_campos_dinamicos" style="color: white; width: 100%; height: 100%; background-color:white" class="modal_content_back">
                       
                    </div>
                    <div id="botones_edita_campos_dinamicos" style=" width:100%; height: 19%; margin-right:1px; border-top: 1px solid #ccc" >

                        <asp:UpdatePanel ID="UpdatePanel_edita_campos_dinamicos_actualiza" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                &nbsp
                                <asp:Button ID="Button_edita_campos_dinamicos" runat="server" Text="Acutalizar" CssClass="boton_azul" ToolTip="Actualiza campos dinamicos" style="float:right; margin-right:5px; margin-top:3px" OnClientClick="confirma_respuesta('Desea actualizar los campos dinamicos');" />

                            </ContentTemplate>
                        </asp:UpdatePanel>

                    </div>
                       <asp:Button ID="Button6" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" style="display:none"/>
                       <asp:Button ID="ButtonSalir_edita_campos_dinamicos" CssClass="invisible" runat="server" Text="Button" style="display:none" />
            </asp:Panel>
        </div>
        
        <div id="cambia_tipo_tramite">
            <asp:Panel ID="Panel_edita_tipo_tramite" runat="server" Style="display: none; width: 50%; height: 100%" CssClass="modal_content_general_">
                <asp:ModalPopupExtender ID="ModalPopupExtender_Panel_edita_tipo_tramite" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_edita_tipo_tramite"
                    PopupControlID="Panel_edita_tipo_tramite" CancelControlID="Button_cerrar_edita_tipo_tramite">
                </asp:ModalPopupExtender>
                <div id="modal_content_tipo_tramite" class="modal-content">
                    <div id="divcabecer2_edita_tipo_tramite" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title d-inline">Cambiar tipo tramite</h6>
                        <button type="button" value="Button_cerrar_edita_tipo_tramite" class="close da_event_captive">&times;</button>
                    </div>
                    <div id="contenido_edita_tipo_tramite" style="width: auto; height: 100%; border-top: none; overflow: auto" class="modal_content_back modal-body">
                       <div id="contenido_campos_edita_tipo_tramite" style="width: 100%; height: 100%; background-color: white; overflow: auto">
                        <asp:UpdatePanel ID="UpdatePanel_edita_tipo_tramite" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>       
                                    <asp:Label ID="Label13" runat="server" Text="Selecciona el tipo tramite" CssClass="font-weight-light h6" Style="font-family: 'Segoe UI'" />
                                    <br />
                                    <asp:DropDownList ID="DropDownList_edita_tipo_tramite" runat="server" CssClass="form-control" onchange="selecciona_fecha_limite_resp_tramite();"></asp:DropDownList>
                                    <br />
                                    <asp:Label ID="Label15" runat="server" Text="Selecciona flujo trabajo" CssClass="font-weight-light h6" Style="font-family: 'Segoe UI'" />
                                    <br />
                                    <asp:DropDownList ID="DropDownList_flujo_tramite" runat="server" CssClass="form-control" onchange="selecciona_lista_activididades_flujos();"></asp:DropDownList>
                                    <br />
                                    <asp:Label ID="Label9" runat="server" Text="Fecha limite de respuesta para el tramite" CssClass="font-weight-light h6" Style="font-family: 'Segoe UI'" />
                                    <asp:TextBox ID="TextBox_fecha_tramite_vence" runat="server" ReadOnly="true" CssClass="form-control"></asp:TextBox>
                               
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <asp:UpdatePanel ID="UpdatePanel_edita_reasigna_flujo" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                 <asp:Label ID="Label18" runat="server" Text="Selecciona actividad flujo" CssClass="font-weight-light h6" Style="font-family: 'Segoe UI'" />
                                    <br />
                                    <asp:DropDownList ID="DropDownList_lista_actividades_flujo" runat="server" CssClass="form-control" onchange="selecciona_lista_usuario_flujos();"></asp:DropDownList>
                                    <br />
                                    <asp:Label ID="Label19" runat="server" Text="Selecciona usuario flujo" CssClass="font-weight-light h6" Style="font-family: 'Segoe UI'" />
                                    <br />
                                    <asp:DropDownList ID="DropDownList_lista_usuarios_flujo" runat="server" CssClass="form-control"></asp:DropDownList>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                         </div>
                        <div style="display: none; height: 1px">
                            <asp:Button ID="ButtonSalir_edita_tipo_tramite" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                            <asp:Button ID="Button_cerrar_edita_tipo_tramite" runat="Server" Text="" CssClass="invisible" />
                        </div>
                    </div>
                    <div id="contenido_botones_edita_tipo_tramite" class="modal-footer justify-content-end">
                         <asp:UpdatePanel ID="UpdatePanel_boton_tipo_tramite" runat="server" UpdateMode="Conditional">
                             <ContentTemplate>
                                 <asp:Button ID="Button_actualiza_tipo_tramite" runat="server" Text="Actualizar" ToolTip="Actualiza el tipo de tramite" CssClass="btn btn-success"  OnClientClick="confirma_respuesta('Desea actualizar el tipo tramite, este procedimiento cambia la fecha limite de respuseta');" />
                                 <asp:Button ID="Button_actualiza_fecha_limte_respuesta" runat="server" Text="" Style="display: none" />
                                 <asp:Button ID="Button_actualiza_lista_actividades_flujo" runat="server" Text="Button" Style="display: none" />
                                 <asp:Button ID="Button_actualiza_lista_usuarios_actividades" runat="server" Text="Button" Style="display: none" />
                                 <input id="Hidden_resultado_tipo_tramite" type="hidden" value="FALSE" runat="server"/>
                                 <input id="Hidden_id_control_postback" type="hidden" value="FALSE" runat="server"/>
                             </ContentTemplate>
                             </asp:UpdatePanel>
                        
                    </div>

                </div>

            </asp:Panel>
        </div>
        <div id="editar_radicacion_entrante" >
            <asp:Panel ID="panel_editar_radicacion_entrante" runat="server" Style="display:none;  width: 50%; height: 100%" CssClass="modal_content_general_">
                <asp:ModalPopupExtender ID="ModalPopupExtender_editar_radicacion_entrante" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_salir_editar_radicacion_entrante"
                    PopupControlID="panel_editar_radicacion_entrante" CancelControlID="Button_cerrar_editar_radicacion_entrante" >
                </asp:ModalPopupExtender>
                <div id="modal_content_radicacion_entrante" class="modal-content">
                    <div id="divcabecer2_editar_radicacion_entrante" class="modal_title_superior_ modal-header">
                        <h6 id="Label4" class="modal-title d-inline ">Editar</h6>
                        <button type="button" value="Button_cerrar_editar_radicacion_entrante" class="close da_event_captive ">&times;</button>   
                       
                    </div>
                    <div id="content_ditar_radicacion_entrante"  style="height: 98%; width: 100%; border-top:none; overflow:auto" class="modal_content_back pl-3 pr-3 pb-3 pt-3">
                        <asp:UpdatePanel ID="UpdatePnaelcontrolesradicacion_entrante" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Panel ID="conenedor_controles_entrante" runat="server">
                                    <asp:Label ID="Label_remitente_entrante" runat="server" ForeColor="black" CssClass="h6 font-weight-light" Style="font-family: 'Segoe UI'; font-size: 16px">Remitente</asp:Label>
                                    <div class="row">
                                        <div class="col-9 pr-1">
                                            <asp:TextBox ID="TextBox_remitente_entrante" runat="server" CssClass="form-control bg-light" Style=""></asp:TextBox>
                                        </div>
                                        <div class="col-3 pl-0">
                                            <button type="button" value="Button_gestion_remitente_entrante"  title="Cambiar usuario" class="da_event_captive btn  btn-success" onclick="document.getElementById('Button_gestion_remitente_entrante').click();"> <i class="fal fa-user-plus"></i></button> 
                                            <asp:Button ID="Button_gestion_remitente_entrante" runat="server" Text="Gestión" CssClass="btn  btn-success d-none"  />
                                        </div>
                                    </div>
                                    <br />
                                    <asp:Label ID="Label_Identificion_remitente_entrante" runat="server" CssClass="h6 font-weight-light " Style="font-family: 'Segoe UI'; font-size: 16px">Identificación Remitente</asp:Label>
                                    <asp:TextBox ID="TextBoxIdentificacion_remitente" runat="server"  CssClass="form-control bg-light  w-50" MaxLength="20"></asp:TextBox>
                                    <br />
                                    <asp:Label ID="Label_area_destinatario_entrante" runat="server" CssClass="h6 font-weight-light" Style="font-family: 'Segoe UI'; font-size: 16px">Area Destinatario</asp:Label>
                                    <asp:DropDownList ID="DropDownList_area_destinatario_entrate" CssClass="form-control" runat="server" onchange="llenardestinatrio_entrante();" ></asp:DropDownList>
                                    <br />
                                    <asp:Label ID="Label_destinatario_entrante" runat="server" CssClass="h6 font-weight-light" Style="font-family: 'Segoe UI'; font-size: 16px">Destinatario</asp:Label>
                                    <asp:DropDownList ID="DropDownList_destinatario_entrante" CssClass="form-control" runat="server" onchange="listar_id_destintario_remitente();" ></asp:DropDownList>
                                    <br />
                                    <asp:Label ID="Label_asunto_entrante" runat="server" CssClass="h6 font-weight-light" Style="font-family: 'Segoe UI'; font-size: 16px">Asunto</asp:Label>
                                    <asp:TextBox ID="TextBox_asunto_entrante" runat="server" CssClass="form-control" MaxLength="200"></asp:TextBox>
                                    <br />
                                    <asp:Label ID="Label_cita_radicado_entrante" runat="server" CssClass="h6 font-weight-light" Style="font-family: 'Segoe UI'; font-size: 16px">Cita radicado</asp:Label>
                                    <asp:TextBox ID="TextBox_cita_radicado_entrante" runat="server" CssClass="form-control" MaxLength="40" Style="width: 40%"></asp:TextBox>
                                    <br />
                                    <asp:Label ID="Label_Numero_Folios_entrante" runat="server" CssClass="h6 font-weight-light" Style="font-family: 'Segoe UI'; font-size: 16px">Numero folios</asp:Label>
                                    <asp:TextBox ID="TextBox_Numero_Folios_entrante" runat="server" CssClass="form-control" MaxLength="9" Style="width: 20%"></asp:TextBox>
                                    <br />
                                    <asp:Label ID="Label_anexos_entrante" runat="server" CssClass="h6 font-weight-light" Style="font-family: 'Segoe UI'; font-size: 16px">Anexo</asp:Label>
                                    <asp:TextBox ID="TextBox_anexos_entrante" runat="server"  MaxLength="150" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                                    <br />
                                    <asp:Label ID="Label_Fecha_Documento_entrante" runat="server" CssClass="h6 font-weight-light" Style="font-family: 'Segoe UI'; font-size: 16px">Fecha documento</asp:Label>
                                    <div class="row">
                                        <div class="col-4">
                                            <asp:TextBox ID="TextBox_fecha_documento_entrante"  CssClass="form-control" runat="server"></asp:TextBox>
                                        </div>
                                        <div class="col-8 pl-0">
                                            <buton id="boton_calendar"  type="button"  > <i class="fad fa-calendar-alt fa-2x"></i> </buton>
                                            
                                        </div>
                                    </div>
                                    <asp:CalendarExtender ID="TextBox_fecha_documento_entrante_CalendarExtender" runat="server" BehaviorID="TextBox_fecha_documento_entrante_CalendarExtender" TargetControlID="TextBox_fecha_documento_entrante" PopupButtonID="boton_calendar" Format="yyyy-MM-dd" />
                                </asp:Panel>
                                <asp:UpdatePanel ID="UpdatePanel_edita_campos_dinamicos" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Panel ID="Panel_dinamico_edita_campos_dinamicos" runat="server" Style="color: white; overflow: auto" Height="100%" BackColor="White" Width="100%" Wrap="false">
                                            <asp:Table ID="Table_edita_campos_dinamicos" runat="server" ForeColor="White" BackColor="White" ViewStateMode="Enabled" Wrap="false" Width="100%">
                                            </asp:Table>
                                        </asp:Panel>

                                    </ContentTemplate>

                                </asp:UpdatePanel>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="modal-footer" style="justify-content: space-between" id="modal-footer_boton_edit">
                                <div class="modal-title d-inline ">
                                    <asp:CheckBox ID="ceked_actualiza_wf" Checked="true" runat="server" />
                                    <label class="" for="flexCheckChecked">
                                        Actualiza en workflow
                                    </label>
                                </div>    
                                <asp:UpdatePanel ID="UpdatePanel_edit_boton" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Button ID="Button_actualizar_entrantres" runat="server" Text="Actualizar" CssClass="btn btn-success" Style="margin-right: 5px; margin-top: 1px; margin-right: 2px" />
                                        <asp:Button ID="Button_listar_destinatarios_entrantes" runat="server" Text="listar" Style="display: none" />
                                        <asp:Button ID="Button_listar_id_destinatario" runat="server" Text="listar" Style="display: none" />
                                        <input id="Hidden_resultado_campo_estatico" type="hidden" value="FALSE" runat="server" />
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                     
                    </div>
                    <div style="display:none; height:1px">                   
                         <asp:Button ID="Button_salir_editar_radicacion_entrante" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" Style="display: none" />
                         <asp:Button ID="Button_cerrar_editar_radicacion_entrante" runat="Server" Text="X" CssClass="invisible" Height="1px" Width="1px" Style="display: none"/>
                    </div>
                    
                </div>
            </asp:Panel>
        </div>    
        
        <div id="botn_asignar" style="display:none">
            <asp:UpdatePanel ID="updatepanel_Asigana_datos_validacion_edicion" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Button ID="Button_Asigana_datos_validacion_edicion" runat="server" />
                    <input id="Hidden_area_remitente_destinatario" type="hidden" value="-1" runat="server"/>
                    <input id="Hidden_remitente_destinario_interno" type="hidden" value="-1" runat="server"/>
                    <input id="Hidden_tipo_plantilla" type="hidden" value="" runat="server"/>
                    <input id="Hidden_nombre_plantilla_radicado" type="hidden" value="" runat="server"/>
                    <input id="Hidden_remitente_destinatario" type="hidden" value="-1" runat="server"/>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div id="editar_radicacion_saliente"  >
            <asp:Panel ID="panel_editar_radicacion_saliente" runat="server" Style="display:none;  width: 50%; height: 100%" CssClass="modal_content_general_">
                <asp:ModalPopupExtender ID="ModalPopupExtender_editar_radicacion_saliente" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_salir_editar_radicacion_saliente"
                    PopupControlID="panel_editar_radicacion_saliente" CancelControlID="Button_cerrar_editar_radicacion_saliente" >
                </asp:ModalPopupExtender>
                <div id="modal_content_radicacion_radicacion_saliente" class="modal-content">
                <div id="divcabecer2_editar_radicacion_saliente" class="modal-header">             
                    <h6 id="Label5" class="modal-title d-inline ">Editar</h6>
                    <button type="button" value="Button_cerrar_editar_radicacion_saliente" class="close da_event_captive ">&times;</button>
                </div>
                    <div id="content_ditar_radicacion_saliente" style="height: 98%; width: 100%; border-top: none; overflow: auto" class="modal_content_back pl-3 pr-3 pb-3 pt-3">
                        <asp:UpdatePanel ID="UpdatePnaelcontrolesradicacion_saliente" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>

                                <asp:Panel ID="conenedor_controles_saliente" runat="server">
                                    
                            <asp:Label ID="Label_area_destinatario_saliente" runat="server" CssClass="h6 font-weight-light" Style="font-family: 'Segoe UI'; font-size: 16px">Area Remitente</asp:Label>   
                            <asp:DropDownList ID="DropDownList_area_remitente_saliente" runat="server" CssClass="form-control" onchange="llenar_remitente();"></asp:DropDownList>
                                    <br /> 
                            <asp:Label ID="Label_destinatario_saliente" runat="server" CssClass="h6 font-weight-light" Style="font-family: 'Segoe UI'; font-size: 16px">Remitente</asp:Label>
                            <asp:DropDownList ID="DropDownList_remitente_saliente" runat="server" CssClass="form-control" onchange="listar_id_destintario_remitente();"></asp:DropDownList>
                                    <br />
                                    
                            <asp:Label ID="Label_remitente_saliente" runat="server" ForeColor="black" CssClass="h6 font-weight-light" Style="font-family: 'Segoe UI'; font-size: 16px">Destinatario</asp:Label>
                                    <div class="row">
                                        <div class="col-9 pr-1">
                                            <asp:TextBox ID="TextBox_remitente_saliente" runat="server" CssClass="form-control bg-light" Style=""></asp:TextBox>
                                        </div>
                                        <div class="col-3 pl-0">
                                            <button type="button" value="Button_gestion_remitente_saliente" title="Cambiar usuario" class="da_event_captive btn  btn-success" onclick="document.getElementById('Button_gestion_remitente_saliente').click();"><i class="fal fa-user-plus"></i></button>
                                            <asp:Button ID="Button_gestion_remitente_saliente" runat="server" Text="Gestión" CssClass="btn  btn-success d-none" />
                                        </div>
                                    </div>
                                    <br />
                                    <asp:Label ID="Label_Identificion_remitente_saliente" runat="server" CssClass="h6 font-weight-light " Style="font-family: 'Segoe UI'; font-size: 16px">Identificación Destinatario</asp:Label>
                                    <asp:TextBox ID="TextBox_identificacion_destinatario" runat="server" CssClass="form-control bg-light  w-50" MaxLength="20"></asp:TextBox>
                                    <br />                   
                            <asp:Label ID="Label_asunto_saliente" runat="server" CssClass="h6 font-weight-light " Style="font-family: 'Segoe UI'; font-size: 16px">Asunto</asp:Label>                
                            <asp:TextBox ID="TextBox_asunto_saliente" runat="server" CssClass="form-control" MaxLength="200" ></asp:TextBox>
                                    <br />
                            <asp:Label ID="Label_cita_radicado_saliente" runat="server" CssClass="h6 font-weight-light " Style="font-family: 'Segoe UI'; font-size: 16px">Cita radicado</asp:Label>                    
                            <asp:TextBox ID="TextBox_cita_radicado_saliente" runat="server" CssClass="form-control" MaxLength="40" Style="width: 40%"></asp:TextBox>
                                     <br />
                            <asp:Label ID="Label_Numero_Folios_saliente" runat="server" CssClass="h6 font-weight-light " Style="font-family: 'Segoe UI'; font-size: 16px">Numero folios</asp:Label>
                 
                            <asp:TextBox ID="TextBox_Numero_Folios_saliente" runat="server" CssClass="form-control" MaxLength="9" Style="width: 20%"></asp:TextBox>
                                    <br />
                                    &nbsp  
                            <asp:Label ID="Label_anexos_saliente" runat="server" ForeColor="black" Font-Size="12" Font-Names="Arial">Anexo</asp:Label>
                            <asp:TextBox ID="TextBox_anexos_saliente" runat="server" MaxLength="150" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                                    <br />
                                    &nbsp 
                                 <br />
                                    <asp:Label ID="Label_Fecha_Documento_saliente" runat="server" CssClass="h6 font-weight-light" Style="font-family: 'Segoe UI'; font-size: 16px">Fecha documento</asp:Label>
                                    <div class="row">
                                        <div class="col-4">
                                            <asp:TextBox ID="TextBox_fecha_documento_saliente"  CssClass="form-control" runat="server"></asp:TextBox>
                                        </div>
                                        <div class="col-8 pl-0">
                                            <buton id="boton_calendar_"  type="button"  > <i class="fad fa-calendar-alt fa-2x"></i> </buton>
                                            
                                        </div>
                                    </div>
                                    <asp:CalendarExtender ID="TextBox_fecha_documento_saliente_CalendarExtender" runat="server" BehaviorID="TextBox_fecha_documento_saliente_CalendarExtender" TargetControlID="TextBox_fecha_documento_saliente" PopupButtonID="boton_calendar_" Format="yyyy-MM-dd" />
                                    <br />
                                </asp:Panel>
                               <asp:UpdatePanel ID="UpdatePanel_edita_campos_dinamicos_saliente" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Panel ID="Panel_dinamico_edita_campos_dinamicos_saliente" runat="server" Style="color: white; overflow: auto" Height="100%" BackColor="White" Width="100%" Wrap="false">
                                            <asp:Table ID="Table_edita_campos_dinamicos_saliente" runat="server" ForeColor="White" BackColor="White" ViewStateMode="Enabled" Wrap="false" Width="100%">
                                            </asp:Table>
                                        </asp:Panel>

                                    </ContentTemplate>

                                </asp:UpdatePanel>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="modal-footer justify-content-end" id="modal-footer_boton_edit_">
                        <asp:UpdatePanel ID="UpdatePanel_boton_edita_saliente" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Button ID="Button_actualiza_salientes" runat="server" Text="Actualizar" CssClass="btn btn-success"  OnClientClick="confirma_respuesta('Desea actualizar los campos estandar de la plantilla');" />
                                <asp:Button ID="Button_listar_destinatarios_salientes" runat="server" Text="listar" Style="display: none" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        
                    </div>
                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button_salir_editar_radicacion_saliente" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                        <asp:Button ID="Button_cerrar_editar_radicacion_saliente" runat="Server" CssClass="invisible" Text="" Height="1px" Width="1px" />
                    </div>
                </div>
            </asp:Panel>
        </div>
       <!--Notifica gestión-->
          <div id="notifica_gestion">
            <asp:Panel ID="Panel_notifica_gestion" runat="server" Style="display:none; width: 70%; height: 100%; margin:auto" CssClass="modal_content_general_">
                <asp:ModalPopupExtender ID="ModalPopupExtender_notifica_gestion" runat="server" BehaviorID="Panel_notifica_gestion_ModalPopupExtender" TargetControlID="ButtonSalir_notifica_gestion"
                    CancelControlID="Button_cerrar_notifica_gestion" PopupControlID="Panel_notifica_gestion" BackgroundCssClass="FondoAplicacion" >
                </asp:ModalPopupExtender>
                <div id="modal_content_notifica_gestion" class="modal-content">
                    <div id="divcabecer2_notifica_gestion" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title d-inline">Envío de correo electrónico</h6>
                        <button type="button" value="Button_cerrar_notifica_gestion" class="close da_event_captive">&times;</button>   
                    </div>
                    <div id="contenido_procesa_notifica_gestion" style="background-color: white; width: 100%; height: auto; border-top:none" class="modal_content_back pl-4 pr-4">
                        <asp:UpdatePanel ID="UpdatePanel_iframenotifica" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <iframe style="color: White; width: 100%; background-color: white; height: auto; overflow: auto" id="Iframe_comparte_coreo" runat="server" frameborder="0"></iframe>
                                <input id="Hidden_cuenta_correo_envio" type="hidden" value="" runat="server"/>
                                <input id="Hidden_correo_envio_default" type="hidden" value="" runat="server"/>
                                <input id="Hidden_imagen_adjunta" type="hidden" value="" runat="server"/>
                                <input id="Hidden_asunto_notificacion" type="hidden" value="" runat="server"/>
                                <input id="Hidden_convierte_pdf" type="hidden" value="" runat="server"/>
                                <input id="Hidden_tipo_notificacion" type="hidden" value="ENVIO CORREO WORKFLOW" runat="server"/>
                                <input id="Hidden_id_plantilla_radicado" type="hidden" value="" runat="server"/>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                    </div>
                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button7" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                        <asp:Button ID="ButtonSalir_notifica_gestion" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                        <asp:Button ID="Button_cerrar_notifica_gestion" runat="Server" Text="" CssClass="invisible" />
                    </div>
                </div>
            </asp:Panel>
        </div>
          
           <input id="Hidden_id_tarea_sel" type="hidden" value="-1" runat="server"/>
           <input id="Hidden_tipo_visor" type="hidden" value="" runat="server"/>
            <!--Popup visor externo-->
               <asp:Panel ID="Panel_visor_externo" runat="server" Style="display:none; overflow:hidden"  Width="100%" Height="100% " CssClass="modal_content_general_" >
                  <asp:ModalPopupExtender ID="ModalPopupExtender_visor_externo" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_visor_externo"
                      PopupControlID="Panel_visor_externo"  CancelControlID="ButtonSalir_visor_externo">
                  </asp:ModalPopupExtender>
                   <div id="modal_content_visor_externo" class="modal-content">
                       <div id="Cabecerapendiente_visor_externo" class="modal_title_superior_ modal-header">
                           <h6 id="Label12" class="modal-title d-inline "> Documentos Relacionados</h6>
                           <button type="button" value="ButtonSalir_visor_externo" class="close da_event_captive ">&times;</button>
                       </div>
                       <div id="Cotenedorpendiente_visor_externo" style="background-color: #FFFFFF; height: 100%; width: 100%; overflow:hidden; border-top: none" class="modal_content_back modal-body_">

                           <asp:UpdatePanel ID="UpdatePanel_visor_externo" runat="server" UpdateMode="Conditional">
                               <ContentTemplate>
                                   <iframe id="Iframe_visor_externo_wf_" runat="server" frameborder="0" style="width: 100%; height: 100%; overflow: hidden"></iframe>
                               </ContentTemplate>

                           </asp:UpdatePanel>

                       </div>
                       <div style="display: none; height: 1px">
                           <asp:Button ID="Button_visor_externo" Style="display: none" runat="server" Text="" Height="1px" Width="1px" />
                           <asp:Button ID="ButtonSalir_visor_externo" runat="Server" Style="display: none" Text="" Height="1px" Width="1px" />
                       </div>

                   </div>
              </asp:Panel>
                <!--detalle trazabilidad-->
           <asp:Panel ID="Panel_trazabilidad" runat="server" Style="display:none; overflow:hidden; width:70%; height:100%"  CssClass="modal_content_general_" >
                  <asp:ModalPopupExtender ID="ModalPopupExtender_trazabilidad" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_trazabilidad_dos"
                      PopupControlID="Panel_trazabilidad"  CancelControlID="ButtonSalir_trazabilidad">
                  </asp:ModalPopupExtender>
               <div id="modal_content_trazabilidad" class="modal-content">
                   <div id="Cabecerapendiente_trazabilidad" class="modal_title_superior_ modal-header">
                       <h6 class="modal-title d-inline">Trazabilidad</h6>
                       <button type="button" value="ButtonSalir_trazabilidad" class="close da_event_captive">&times;</button>   
                   </div>
                   <div id="Cotenedorpendiente_trazabilidad" style="height: 90%; width: 100%; border-top:none" class="modal_content_back pl-3 pr-3">
                       <asp:UpdatePanel ID="UpdatePanel_trazabilidad" runat="server" UpdateMode="Conditional">
                           <ContentTemplate>
                               <iframe id="Iframe_trazabilidad_" runat="server" frameborder="0" style="width: 100%; height: 100%; overflow: hidden"></iframe>
                           </ContentTemplate>

                       </asp:UpdatePanel>

                   </div>
                   <div style="display:none; height:1px">
                        <asp:Button ID="Button_trazabilidad_dos" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                    <asp:Button ID="ButtonSalir_trazabilidad" runat="Server" Text="X" CssClass="invisible"  Height="1px" Width="1px"/>
                   </div>
                  
               </div>
              </asp:Panel>
         <asp:Panel ID="Panel_interface_regitra_meta_dato" runat="server" Style="display:none;  width:50%; height: auto" CssClass="modal_content_general_">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_interface_regitra_meta_dato"  runat="server" BackgroundCssClass="FondoAplicacion"  TargetControlID="ButtonSalir_interface_regitra_meta_dato" 
                    CancelControlID="Button_cerrar_interface_regitra_meta_dato" PopupControlID="Panel_interface_regitra_meta_dato" ></asp:ModalPopupExtender>
                <div class="modal-content">
                    <div id="divcabecer2_interface_regitra_meta_dato" class="modal_title_superior_ modal-header">
                        <h6 id="label_interface_regitra_meta_dato" class="modal-title  ">Registra meta dato</h6>
                        <button type="button" value="Button_cerrar_interface_regitra_meta_dato" class="close da_event_captive">&times;</button>
                    </div>
                    <div id="contenido_procesa_interface_regitra_meta_dato" style="background-color: white; width: auto; height: auto; color: black; background-color: #FFFFFF; border-top: none; overflout: auto" class="modal_content_back modal-body">
                        <div id="conte_regitra_meta_dato_control" >
                         
                        </div>
                    </div>
                    <div class="modal-footer" id="modal_footer_regitra_meta_dato">
                        <input id="Button_registra_meta" type="button" value="Aceptar" onclick="event_element_clic(event,this);" class="btn btn-success"/>
                        <input id="Button_file_firma" type="button" value="Aceptar" style="display:none" class="btn btn-success"/>
                    </div>
                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button_cerrar_interface_regitra_meta_dato" runat="Server" Text="" />
                        <asp:Button ID="Button_interface_regitra_meta_dato" runat="server" Text="" Height="1px" Width="1px" />
                        <asp:Button ID="ButtonSalir_interface_regitra_meta_dato" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                    </div>
                </div>
            </asp:Panel>
             <!--PROGRES-->
        <div id="Divpro_gres_bar">
            <asp:Panel ID="Panel_pro_gres_bar" runat="server" Style="display:none; color: White; width:30%; height:auto" CssClass="border_superior_inferior_radius_blanco">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_pro_gres_bar" runat="server"  TargetControlID="ButtonSalir_pro_gres_bar" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_pro_gres_bar" PopupControlID="Panel_pro_gres_bar"></asp:ModalPopupExtender>
                <div id="div1" class="border_superior_radius_blanco" style="display:none">                
                    <asp:Label ID="Label_pro_gres_bar" runat="server" Text=""  Style="">
                    </asp:Label>
                    <div id="Divcerrarbuton2_pro_gres_bar" style="float: right">
                        <asp:Button ID="Button_cerrar_pro_gres_bar" runat="Server" Text="X" style="display:none" 
                             ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_pro_gres_bar" style="width:99%; height:99%" class="modal_content_back_no_radio" > 
                      <br />   
                    <div style="text-align:center">
                         <asp:Label ID="Label_progres_bar" runat="server" Text="Progreso de la tarea" style="font-family:Arial; text-align:center; font-size:20px"></asp:Label>
                    </div>
                    <br />  
                     <div id="myProgress_contador" style="text-align: center; font-family:Arial; font-size:14px">
                        
                             0 
                        </div>
                        <div id="myProgress_porcent" style="text-align: center; font-family:Arial; font-size:14px">
                            0 %
                        </div>                
                        <div style="margin-left:5%; margin-right:5%">
                            <div id="myProgress" >
                            <div id="myBar" ></div>
                        </div>
                        </div>         
                        <br/>
                        <div style="text-align: center">
                            <button class="boton_blanco" onclick="myStopFunction(event)" >Cancelar</button>
                        </div>
                              
                    <asp:UpdatePanel ID="UpdatePanel_pro_gres_bar" runat="server" UpdateMode="Conditional">
                        <ContentTemplate> 
                           
                            <asp:Button ID="Button_pogres_show" CssClass="invisible" runat="server" Text="Button" style="display:none" />   
                            
                        </ContentTemplate>
                    </asp:UpdatePanel>
                         
                    <asp:Button ID="ButtonSalir_pro_gres_bar" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />    
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
    </form>
    <script  accesskey="javascript" type="text/javascript">
        AjaxFileUpload_change_text();
        $(document).ready(function () {
            $('#sidebarCollapse').on('click', function () {
                $('#sidebar_').toggleClass('active_da_slider');
                $('#Contenedorderecho').toggleClass('active_content_rigth');
                $('#Contentizquierdo').toggleClass('active_content_left');
                $(this).toggleClass('active_da_slider');
                $('#da_show-sidebar_').toggleClass('show_da_slide');
                $('#da_show-sidebar_').toggleClass('hide_da_sidebar');

            });
            $('#da_show-sidebar_').on('click', function () {
                $('#sidebar_').toggleClass('active_da_slider');
                $('#Contenedorderecho').toggleClass('active_content_rigth');
                $('#Contentizquierdo').toggleClass('active_content_left');
                $(this).toggleClass('show_da_slide');
                $(this).toggleClass('hide_da_sidebar');
                auto_zise_popup_validacion_radicados();

            });
        });
        $(document).ready(function () {
            $('#sidebarCollapse_').on('click', function () {
                $('#sidebar__').toggleClass('active_da_slider');
                $('#Contenedorderecho_').toggleClass('active_content_rigth');
                $('#Contentizquierdo_').toggleClass('active_content_left');
                $(this).toggleClass('active_da_slider');
                $('#da_show-sidebar__').toggleClass('show_da_slide');
                $('#da_show-sidebar__').toggleClass('hide_da_sidebar');
            });
            $('#da_show-sidebar__').on('click', function () {
                $('#sidebar__').toggleClass('active_da_slider');
                $('#Contenedorderecho_').toggleClass('active_content_rigth');
                $('#Contentizquierdo_').toggleClass('active_content_left');
                $(this).toggleClass('show_da_slide');
                $(this).toggleClass('hide_da_sidebar');
            });
        });
</script>
</body>
</html>
