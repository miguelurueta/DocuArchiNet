<%@ Page Language="vb" AutoEventWireup="false" enableEventValidation="false"  CodeBehind="WebFormRadicacionEntrante.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormRadicacionEntrante" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
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
    <script src="../js/radicacion/WebFormRadicacionEntrante.js"></script>
    <script src="../js/java_general/general_control_java.js"></script>
    <script src="../js/java_general/general_code_java.js"></script>
    <script src="../generic_control/FileUploadHandler.js" type="text/javascript"></script>
    <link href="../generic_control/UploadFile.css" rel="stylesheet" />
     <script src="../js/java_general/GredviewControl.js"></script>
     <script src="../js/java_general/JS_firma_digital.js" type="text/javascript"></script>
    <script src="../js/java_general/row_multiple_gred.js" type="text/javascript"></script>
    <script src="../js/java_general/JSProgresBar.js"></script>
    <script src="../js/java_general/JSReplaceScanFile.js"></script>
    <script src="../js/java_general/ASMXClient.js"></script>
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
<body style="background-color:white">       
    <form id="form1" runat="server">    
       <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="True"  AsyncPostBackTimeout="19900">
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
                try {
                    elment_postbak = args.get_postBackElement();
                    posicion_update_pogres('progres_bar');
                }
                catch (err) {
                    alert(err.message + " Funcion InitializeRequest");
                }
            }
            function CheckStatus(sender, args) {
                try {
                    if (elment_postbak.id == 'Button_nuevo_radicado') {             
                        if (document.getElementById("Hidden_resultado_radic").value == "YES") {
                            document.getElementById("Hidden_resultado_radic").value = "";
                            nuevo_radicado_tab();
                        }
                    }
                    if (elment_postbak.id == 'Button_tool_asigna_radicados_pendientes') {
                        if (document.getElementById("Hidden_result_boton_tool").value == "YES") {
                            document.getElementById("Hidden_result_boton_tool").value = "";
                            asig_radicado_tab();
                        }
                    }
                    if (elment_postbak.id == 'Buttonradicar_entrante') {
                        if (document.getElementById("Hidden_result_asig_radic").value == "YES") {
                            document.getElementById("Hidden_result_asig_radic").value = "";
                            asig_radicado_tab();
                        }
                    }
                    if (elment_postbak.id == 'btnOkay_autoterminar') {
                        if (document.getElementById("Hidden_rest_auto_terinar").value == "YES") {
                            document.getElementById("Hidden_rest_auto_terinar").value = "";
                            terminar_radicado_tab();
                        }
                    }
                    
                    if (elment_postbak.id == 'Button_activa_enviar_actividad_flujo_trabajo') {
                        if (document.getElementById("Hidden_resul_eviar_actividad").value == "YES") {
                            document.getElementById("Hidden_resul_eviar_actividad").value = "";
                            terminar_radicado_tab();
                        }
                    }
                   
                    if (elment_postbak.id == 'Button_tool_enviar_ruta') {
                        if (document.getElementById("Hidden_result_boton_tool").value == "YES") {
                            document.getElementById("Hidden_result_boton_tool").value = "";
                            terminar_radicado_tab();
                        }
                    }
                    if (elment_postbak.id == 'Button_tool_enviar_actividad') {
                        if (document.getElementById("Hidden_result_boton_tool").value == "YES") {
                            document.getElementById("Hidden_result_boton_tool").value = "";
                            terminar_radicado_tab();
                        }
                    }
                    if (elment_postbak.id == 'Button_tool_enviar_usuario') {
                        if (document.getElementById("Hidden_result_boton_tool").value == "YES") {
                            document.getElementById("Hidden_result_boton_tool").value = "";
                            terminar_radicado_tab();
                        }
                    }
                    if (elment_postbak.id == 'Button_tool_termitar_radicado') {
                        if (document.getElementById("Hidden_result_boton_tool").value == "YES") {
                            document.getElementById("Hidden_result_boton_tool").value = "";
                            terminar_radicado_tab();
                        }
                    }
                    if (elment_postbak.id == "Button_tool_visualiza_documento") {
                        if (document.getElementById("Hidden_result_boton_tool").value == "YES") {
                            document.getElementById("Hidden_result_boton_tool").value = "";
                            dispalyVisorEmergente();
                        }          
                    }           
                    if (elment_postbak.id == 'Button_tool_elimina_documento') {
                        if (document.getElementById("Hidden_result_boton_tool").value == "YES") {
                            document.getElementById("Hidden_result_boton_tool").value = "";
                            dispalyInterfaceEscaner();
                            eliminar_fila_data_gred_simple_('GridView_list_documento_relacion', 'hiden_seleccion_documento_id', 'hiden_seleccion_documento','-1','')
                            //decrementa_documento_relacion_estado();
                        }
                    }
                    if (elment_postbak.id == 'Button_actualiza_tipologia_documental') {
                        if (document.getElementById("Hidden_resulta_botno_tipologia_documental").value != "") {         
                            update_Cell_AspNetGred('GridView_list_documento_relacion', document.getElementById("hiden_seleccion_documento_id").value, document.getElementById("Hidden_resulta_botno_tipologia_documental").value, 'DOCUMENTO','id_rad');
                            document.getElementById("Hidden_resulta_botno_tipologia_documental").value = "";
                        }
                    }
                    //ButtonAlmacenar
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
                            insert_row_documento_relacionado(document.getElementById("Hidden_date_row_").value,"rad");
                            document.getElementById("Hidden_date_row_").value = "";
                        }
                    }
                    progres_hiden('progres_bar');     
                }
                catch (err) {
                    alert(err.message + " Funcion CheckStatus");
                }
            }

        </script>
        <div id="contenguia" class="contenguia" style="" >
            <div id="div_error_content_general" style="position: relative; width: 100%"></div>
            <div class="container_">
                <div class="nav-person-da" id="tab_content_item">
                    <ul class="nav nav-tabs mt-2" id="myTab" role="tablist">
                        <li class="nav-item"  onclick="show_tab_boton_content_radicado();">
                            <a class="nav-link nav-link-person "  id="home-radicador" data-toggle="tab" href="#home_radic" role="tab" aria-controls="home_radic" aria-selected="true"><i id="home-radicadori" class="fal fa-lock d-none"></i> Recepción y radicación</a>
                        </li>
                        <li class="nav-item" onclick="auto_resize_radicacion(); show_tab_boton_content_gestion_radicado();">
                            <a class="nav-link nav-link-person "  id="soporte-envio_nav" data-toggle="tab" href="#soporte_envio" role="tab" aria-controls="profile" aria-selected="false"><i id="soporte-envio_navi" class="fal fa-lock d-none"></i> Envío y soporte documental</a>
                        </li>
                        <div style="float: right" class="div_link_pend ml-auto pd-2">
                            <i id="i_radicado_pendiente" style="color: darkorange; display: none" class="fas fa-bell fa-spin"></i>
                            <a class="nav-link_ float-right " id="num_rad_pendiente" onclick="asigna_radicado_pendiente('R00001')" href="#settings"></a>
                        </div>
                    </ul>
                </div>
                <div class="tab-content" id="tab_content" style="overflow:auto">
                    <div id="div_error_content_rad" style="position: relative; width: 100%"></div>
                    <div class="tab-pane  p-2" id="home_radic" role="tabpanel" aria-labelledby="home-tab">
                        <div id="content_radicado_body" >
                        <asp:Panel ID="PanelTitulo" ForeColor="Black" runat="server" ScrollBars="None" EnableViewState="true" Style="font-family: Arial; font-size: 14px; font-weight: 600; margin-right: 3px; background-color: #6d7fcc" CssClass=" modal-header ml-1 mt-1 d-none">
                            <asp:Table ID="TableTitle" CssClass="mt-2 mb-2" runat="server" ForeColor="#E7EDF5" ViewStateMode="Enabled">
                            </asp:Table>
                        </asp:Panel>

                        <asp:Panel ID="Panel_modo_radicado"  runat="server" EnableViewState="true" CssClass="p-2" style="border-bottom: 1px solid #dee2e6">
                            <asp:UpdatePanel ID="UpdatePanel_modo_radicado" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <input id="Hiddenid_expediente" type="hidden" value="0" runat="server"/>
                                    <asp:MutuallyExclusiveCheckBoxExtender ID="MutuallyExclusive_anexo_radicado" runat="server" TargetControlID="Check_anexo_radicado"
                                        Key="radicado"></asp:MutuallyExclusiveCheckBoxExtender>
                                    <asp:MutuallyExclusiveCheckBoxExtender ID="MutuallyExclusive_relacionado_radicado" runat="server" TargetControlID="CheckBox_relacionado_radicado"
                                        Key="radicado"></asp:MutuallyExclusiveCheckBoxExtender>
                                    <asp:MutuallyExclusiveCheckBoxExtender ID="MutuallyExclusive_nuevo_radicado" runat="server" TargetControlID="check_nuevo_radicado"
                                        Key="radicado"></asp:MutuallyExclusiveCheckBoxExtender>
                                    <div class="row  container mt-2 mb-4">
                                        <div class="col-4">
                                            <asp:CheckBox ID="Check_anexo_radicado" runat="server" Text="" Checked="false"  />
                                            <span >RADICAR COMO ANEXO</span>
                                        </div>
                                        <div class="col-4">
                                            <asp:CheckBox ID="CheckBox_relacionado_radicado" runat="server" Text="" Checked="false"  />
                                             <span>RADICAR COMO RELACIONADO</span>
                                        </div>
                                        <div class="col-4">
                                            <asp:CheckBox ID="check_nuevo_radicado"  runat="server" Text="" Checked="true"  />
                                             <span>NUEVO RADICADO</span>
                                        </div>
                                    </div>
                                    <div class="row mt-1 mb-3" >
                                        <div class="col-4">
                                            <span class="mb-2 font-weight-light"  >Expediente</span> 
                                            <asp:TextBox ID="Textbox_expediente_val_radicacion" runat="server" disabled="disabled"></asp:TextBox>
                                            <asp:Button ID="Button_Eliminar_Expediente" runat="server" Text="X" ToolTip="Eliminar expediente seleccionado" style="font-size:10px" CssClass="btn btn-success" />
                                            <asp:Button ID="Button_Edit_Expediente" runat="server" Text="S" ToolTip="Seleccionar expediente" style="font-size:10px" CssClass="btn btn-success" OnClientClick="tamano_ventana_expediente();" />
                                            <input id="Hidden_id_expediente" type="hidden" value="0" runat="server"/>
                                        </div>
                                         <div class="col-4">
                                             <span class="font-weight-light" >Radicados relacionados</span> 
                                              <asp:DropDownList ID="Dropdowlis_sel_val_radciacion" runat="server"  style="min-width:150px"></asp:DropDownList>
                                              <asp:Button ID="Button_Eliminar_Rel_Radicados" runat="server" Text="X" ToolTip="Eliminar radicado enlazado" style="font-size:10px" CssClass="btn btn-success" />                                 
                                        </div>
                                         <div class="col-4">                            
                                              <asp:Button ID="Buttonvalidar_radciado" Text="Consulta previa" runat="server"  ToolTip="Consulta radicados y asigna  tipo radicado" CssClass=" btn btn-success" />
                                        </div>
                                    </div>                         
                                    <div id="cierra_popup_expediente" style="display: none">
                                        <asp:Button ID="Button_cierra_popup_expediente" runat="server" Text="Cierrapopup" />
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </asp:Panel>
                        <asp:UpdatePanel ID="UpdatePnaelcontrolesradicacion" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Panel ID="PanelRadicacion" runat="server" CssClass="mt-3"  Style="margin-left: 5px"
                                    EnableViewState="true">
                                    <div id="separator_control" style="width: 100%; height: 5px; background-color: white; display: none"></div>                              
                                    <asp:Table ID="TableControles" runat="server" style="overflow:auto; width:100%" CssClass="table_radic"  ViewStateMode="Enabled">
                                    </asp:Table>
                                    <asp:Table ID="Tableseparacion" runat="server" CssClass="mt-2 mb-2"  style="display:none" ViewStateMode="Enabled">
                                    </asp:Table>
                                    <asp:Table ID="Tableremitente" runat="server"  style="overflow:auto; width:100%" CssClass="table_radic" ViewStateMode="Enabled">
                                    </asp:Table>
                                    <asp:Table ID="Tableseparador_documento" runat="server" CssClass="mt-2 mb-2" style="display:none" ViewStateMode="Enabled">
                                    </asp:Table>
                                    <asp:Table ID="tablecontrolesdinamicos" runat="server" style="overflow:auto; width:100%" CssClass="table_radic" ViewStateMode="Enabled">
                                    </asp:Table>
                                    <input id="Hiddenareagestion" type="hidden" value="" runat="server"/>
                                    <input id="Hiddendestinatario" type="hidden" value="" runat="server"/>
                                </asp:Panel>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="asignar" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="Buttontramitevence" EventName="Click" />
                            </Triggers>

                        </asp:UpdatePanel>
                        </div>
                        
                        <asp:UpdatePanel ID="UpdatePanelbotonesradicado" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div id="butonverdestinario" style="display: none">
                                    <asp:Button ID="Button_ra_destinatario" runat="server" />
                                    <asp:Button ID="Buttonllenardestinatario" runat="server" />
                                    <asp:Button ID="Buttontramitevence" runat="server" />
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div id="butonloftfocus" style="display: none">
                            <asp:Button ID="focusremitente" runat="server" />
                            <asp:Button ID="Buttonrefasignar" runat="server" />

                        </div>
                    </div>

                    <div class="tab-pane  p-2" id="soporte_envio" role="tabpanel" aria-labelledby="profile-tab">  
                        <asp:UpdatePanel ID="updatemenu" CssClass="" style="width: 100%" runat="server" UpdateMode="Conditional" RenderMode="Block">
                            <ContentTemplate>
                                <nav id="navar_barra" class="navbar navbar-expand-sm nav_botota_person modal_content_no_back_inferior">
                                    <button class="navbar-toggler" type="button" style="background-color: #6d7fcc" data-toggle="collapse" data-target="#navbarNavDropdown">
                                        <span class="navbar-toggler-icon_"><i style="color: white" class="fad fa-th-list"></i></span>
                                    </button>                                   
                                    <div class="collapse navbar-collapse row" id="navbarNavDropdown">
                                        <asp:Panel ID="Panel_imprime_rotulo" CssClass="navbar-nav " runat="server">
                                        <ul class="navbar-nav">
                                            <li class="nav-item active ml-2 active_">
                                                <a class="nav-link  font-weight-light" style="color: #6d7fcc" title="Imprimir rotulo radicado" href="#" onclick="activa_boton_client_server('Button_tool_imprime_rotulo');"><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-print"></i> Imprimir rotulo  </a>
                                            </li>
                                              <li class="nav-item active ml-2 active_">
                                                <a class="nav-link font-weight-light" style="color: #6d7fcc" title="Guarda rotulo radicado" href="#" onclick="inicializa_tipo_adjunto_documento(event,this,'I-GD-RD');"><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-save"></i> Guardar rotulo  </a>
                                            </li>
                                            <li class="nav-item active ml-2 active_">
                                                <a class="nav-link font-weight-light" style="color: #6d7fcc" title="Detalle radicado" href="#" onclick="activa_boton_client_server('Button_tool_activa_detalle_radicado');"><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-ellipsis-v"></i> Detalle radicado  </a>
                                            </li>
                                            <li class="nav-item active ml-2 active_">
                                                <a class="nav-link" href="#">
                                                  <i style="margin-left: 1px; margin-top: 7px; color: #6d7fcc" class="fad fa-sticky-note"></i>
                                                  <span id="nota_db" class="font-weight-light" style="color: darkorange;" title="Notas" onclick="activa_boton_client_server('ImageButtonanotacion');"> Notas  </span>                
                                              </a>
                                            </li>
                                        </ul>
                                        </asp:Panel>
                                        <asp:Panel ID="Panel_cargar_archivo" CssClass="navbar-nav d-none " runat="server">
                                            <ul class="navbar-nav">
                                                <li class="nav-item active ml-2 active_">
                                                    <a id="a_load_file" class="nav-link font-weight-light" style="color: #6d7fcc" title="Cargar archivo a la lista" href="#" ><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-file-upload"></i> Cargar archivo  </a>
                                                </li>
                                            </ul>
                                        </asp:Panel>
                                        <asp:Panel ID="Panel_EnviarUsuario" CssClass="navbar-nav " runat="server">
                                            <ul class="navbar-nav">
                                                <li class="nav-item active ml-2 active_">
                                                    <a class="nav-link font-weight-light" style="color: #6d7fcc" title="Envía el radicado a usuario" href="#" onclick="activa_boton_client_server('Button_tool_activa_enviar_usuario');"><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-user"></i> Enviar a usuario  </a>
                                                </li>
                                            </ul>
                                        </asp:Panel>
                                        <asp:Panel ID="Panel_EnviaActividad" CssClass="navbar-nav " runat="server">
                                            <ul class="navbar-nav">
                                                <li class="nav-item active ml-2 active_">
                                                    <a class="nav-link font-weight-light" style="color: #6d7fcc" title="Envía el radicado a grupo de usuarios" href="#" onclick="activa_boton_client_server('Button_tool_activa_enviar_actividad');"><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-user-friends"></i> Enviar a grupo  </a>
                                                </li>
                                            </ul>
                                        </asp:Panel>
                                        <asp:Panel ID="Panel_enviar_flujo" CssClass="navbar-nav " runat="server">
                                            <ul class="navbar-nav">
                                                <li class="nav-item active ml-2 active_">
                                                    <a class="nav-link" style="color: #6d7fcc" title="Envía el radicado por ruta o flujo de trabajo" href="#" onclick="activa_boton_client_server('Button_tool_terminar');"><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-project-diagram"></i> Enviar a flujo  </a>
                                                </li>
                                            </ul>
                                        </asp:Panel>
                                        <asp:Panel ID="Panel_auto_terminar" CssClass="navbar-nav " runat="server">
                                            <ul class="navbar-nav">
                                                <li class="nav-item active ml-2 active_">
                                                    <a class="nav-link" style="color: #6d7fcc" title="Envía el radicado a gestión" href="#" onclick="activa_boton_client_server('Button_tool_auto_terminar');"><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-check"></i> Enviar a gestión  </a>
                                                </li>
                                            </ul>
                                        </asp:Panel>
                                        <asp:Panel ID="Panel_terminar_radicado" CssClass="navbar-nav " runat="server">
                                            <ul class="navbar-nav">
                                                <li class="nav-item active ml-2 active_">
                                                    <a class="nav-link" style="color: #6d7fcc" title="Terminar radicado" href="#" onclick="prevent_terminar_radicado(event,this);"><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-check"></i> Terminar radicado  </a>
                                                </li>
                                            </ul>
                                        </asp:Panel>
                                    </div>
                                    <div style="float:right" class="div_link_pend ml-auto pd-2">
                                        <asp:Panel ID="Panel_pendiente_radicado" CssClass="navbar-nav " runat="server">    
                                            <a class="nav-link_ float-right" title="lista pendientes" id="A1" onclick="activa_boton_client_server('Button_tool_lista_pendientes_radicados');" href="#settings">  <i id="i1" style="color: darkorange" class="fas fa-bell "></i> Pendientes : </a>       
                                            <asp:Label ID="Label_numero_item" runat="server" Text="" CssClass="h6 font-weight-light ml-1" style=""></asp:Label>                
                                        </asp:Panel> 
                                        <asp:Label ID="Label_estado_selecion" runat="server" Text="" CssClass="h6 font-weight-light" style=" color: #6d7fcc;  font-size:10px; display:block"></asp:Label>               
                                    </div>
                                </nav>
                                
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div id="conte_waper" class="container-fluid mr-0 ml-0 pl-0 pr-0" style="border-top: 1px solid #e9ecef">
                            <a id="da_show-sidebar_" class="btn btn-sm   hide_da_sidebar " href="#" data-target="#sidebar_">
                                <i style="color: white" class="fas fa-bars"></i>
                            </a>
                            <div id="da_content_wraper" class="wrapper_ ml-0 mr-0  d-flex  justify-content-between_" style="padding-left: 1px; padding-right: 1px">
                                <div id="Contentizquierdo" class="bg-light_ " style="width: 22%; float: left">
                                    <nav id="sidebar_" class=" bg-light_ pl-0 pr-0">
                                        <div id="title_treview" class="modal-header_ modal_title_superior " style="border-top-left-radius: initial; border-top-right-radius: initial;border-bottom: 1px solid #e9ecef;  border-right: 1px solid #e9ecef"> 
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
                                                        <a id="sidebarCollapse" class="nav-link pr-2 pl-2" style="float: right; color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600" title="Cerrar lista"><i class="fad fa-bars"></i></a>
                                                        <a id="sing_multiple_file" class="nav-link pr-2 pl-2" style="color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600; float: right" title="Firmar documentos seleccionados " href="#" ><i style="" class="fad fa-file-signature"></i></a>
                                                        <a id="delete_row_several_rad" class="nav-link pr-2 pl-2" style="color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600; float: right" title="Eliminar documentos" href="#" ><i style="" class="fad fa-trash-alt"></i></a>
                                                        <a id="a_load_file_nav" class="nav-link pr-2 pl-2" style="color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600; float: right" title="Adjuntar documento" href="#" ><i style="" class="fas fa-upload "></i></a>    
                                                    </div>   
                                                </div>    
                                            </div>
                                        </div>
                                        <div id="div_treview_archivo" style="width: 100%; border-right: 1px solid #e9ecef" >
                                            <asp:UpdatePanel ID="UpdatePanelseleccion_digitalizado" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                                <ContentTemplate>
                                                    <input id="hiden_seleccion_documento" type="hidden" value="" runat="server"/> 
                                                    <input id="hiden_seleccion_documento_id" type="hidden" value="-1" runat="server"/> 
                                                    <input id="Hidden_numero_doc_rel" type="hidden" value="0" runat="server"/> 
                                                    <asp:Panel ID="Paneltreview" runat="server"
                                                        Height="100%" Width="100%" Style="position: inherit; overflow:auto">
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
                                        <div id="contenido_pie" style=" border-top-left-radius: initial; border-top-right-radius: initial; display:none" class="modal-header pt-1 pb-1   justify-content-start">
                                            <h6 class="modal-title_ mt-2 mb-2 ml-2   font-weight-light" id="pit" style="color: white"></h6>
                                        </div>
                                    </nav>
                                </div>
                                <div id="Contenedorderecho" class="page-content mr-0 ml-0 pl-1 pr-1 pb-0 pt-0  " style="width: 78%">                              
                                    <div id="Are_Digitalizacion" style="width: 100%; height: 100%; float: right; display: none; margin-left: 1px" class="modal_content_back_">
                                        <iframe id="IframeDitaliza_" runat="server" frameborder="0" src="" width="100%" scrolling="no" height="100%"></iframe>
                                        <asp:UpdatePanel ID="UpdatePanel_iframe_digitaliza" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                            <ContentTemplate>
                                                
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>                          
                                    <div id="Area_Visor" style="width: 100%; height: 100%; display: none" class="modal_content_back_">
                                        <div id="div_cerrar" class="modal-header_ modal_title_superior " style="border-top-left-radius: initial; border-top-right-radius: initial;border-bottom: 1px solid #e9ecef">
                                            <h6 id="titel_visor" class="mt-2 mb-2 ml-2  h6 font-weight-light" style="color:#6d7fcc; font-family: 'Segoe UI'; float:left">Visor externo</h6>
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
                </div>
                <div class="modal-footer  justify-content-between p-1" id="tab_content_boton">
                    <div class=" d-inline-flex">
                        <asp:UpdatePanel ID="UpdatePanel_title_radicado" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <asp:Label ID="h_radicado_title" runat="server" Text="Radicado :" class="mt-1 ml-1  h6 font-weight-light" style="color:#6d7fcc"></asp:Label>
                            </ContentTemplate>
                        </asp:UpdatePanel>        
                        <asp:Label ID="Label_estado_transac" runat="server" Text="" Style="font-size: 8px; font-family: Arial; float: left"></asp:Label>
                    </div>   
                    <div id="content_radicado_boton" class="float-right" style="display:none">
                        <div id="separator_control_2" style="width: 100%; height: 1%; background-color: #E7EDF5; display: none"></div>
                        <asp:Panel ID="Panelbotonesradcacion" runat="server" ScrollBars="Auto" Style="height: 10%; margin-left: 5px; padding-top: 10px; text-align: right"
                            EnableViewState="true">
                            <asp:UpdatePanel ID="UpdatePanelradciacionbotones" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Button ID="Buttonradicar_entrante" runat="server" Style="position: relative; top: -3px; left: 0px; margin-right: 5px" Text="Radicar" Width="100px" ToolTip="Radicar documento" CssClass="btn btn-success" />
                                    <asp:Button ID="Buttonlimpiar_entrante" Text="Limpiar" runat="server" Style="position: relative; top: -3px; left: 0px; margin-right: 5px" Width="100px" ToolTip="Limpiar campos radicacion" CssClass="btn btn-success" />
                                    <asp:Button ID="Button_muestra_dias_horas_habil" Text="Mostrar" runat="server" Style="position: relative; top: -3px; left: 0px; margin-right: 5px" Width="100px" ToolTip="Muestra los días y horas hábiles permitidos para radicar" CssClass="btn btn-success" />
                                    <input id="Hidden_result_asig_radic" type="hidden" value="" runat="server" />
                                </ContentTemplate>
                            </asp:UpdatePanel>

                        </asp:Panel>
                    </div>
                    <div class="float-right" id="tab_nuevo_radicado">
                        <asp:UpdatePanel ID="UpdatePanel_boton_nuevo_radicado" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <asp:Button ID="Button_nuevo_radicado" runat="server" Visible="false" Text="Nuevo radicado"  CssClass="btn btn-success m-2"/>
                                <input id="Hidden_resultado_radic" type="hidden" value="0" runat="server"/>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                    </div>
                </div>
            </div>
            
        </div>
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
                    <asp:Button ID="Button_tool_lista_pendientes_radicados" runat="server" Text="" />
                    <asp:Button ID="Button_tool_asigna_radicados_pendientes" runat="server" Text="" />
                    <asp:Button ID="Button_tool_imprime_rotulo" runat="server" Text="" />
                    <asp:Button ID="Button_tool_visualiza_documento" runat="server" Text="" />
                    <asp:Button ID="Button_tool_elimina_documento" runat="server" Text="" />
                    <asp:Button ID="Button_tool_activa_cambia_tipologia" runat="server" Text="" />
                    <asp:Button ID="Button_tool_activa_sube_documento" runat="server" Text="" />
                    <asp:Button ID="Button_tool_actualiza_lista_relacionados" runat="server" Text="" />
                    <asp:Button ID="Button_tool_auto_terminar" runat="server" Text="" />
                    <asp:Button ID="Button_tool_terminar" runat="server" Text="" />
                    <asp:Button ID="Button_tool_activa_enviar_actividad" runat="server" Text="" />
                    <asp:Button ID="Button_tool_enviar_actividad" runat="server" Text="" />
                    <asp:Button ID="Button_tool_busqueda_enviar_actividad" runat="server" Text="" />
                    <asp:Button ID="Button_tool_restore_busqueda_enviar_actividad" runat="server" Text="" />
                    <asp:Button ID="Button_tool_activa_enviar_usuario" runat="server" Text="" />
                    <asp:Button ID="Button_tool_enviar_usuario" runat="server" Text="" />
                    <asp:Button ID="Button_tool_busqueda_enviar_usuario" runat="server" Text="" />
                    <asp:Button ID="Button_tool_enviar_ruta" runat="server" Text="" />
                    <asp:Button ID="Button_tool_restore_busqueda_enviar_usuario" runat="server" Text="" />
                    <asp:Button ID="Button_tool_activa_detalle_radicado" runat="server" Text="" />
                    <asp:Button ID="ButtonAlmacenar" Text="" runat="server" /> 
                    <asp:Button ID="Button_tool_termitar_radicado" runat="server" Text="" />           
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <input id="HiddenPROMP" type="hidden" value="1" runat="server"/>
            <input id="HiddenPlantilla" type="hidden" value="" runat="server"/>           
            <input id="Hiddenscript" type="hidden" value="" runat="server"/>
            <input id="Hidden_Estado_opcion_fecha" type="hidden" value="0" runat="server"/>
            <input id="Hidden_Estado_opcion_cita_respuesta" type="hidden" value="0" runat="server"/>
            <input id="Hidden_Estado_opcion_radicado_general" type="hidden" value="0" runat="server"/>
            <input id="Hiddentramiteseleccion" type="hidden" value="" runat="server"/>
            <input id="Hiddenheigpaginapopup" type="hidden" value="475" runat="server"/>
            <input id="Hiddennameasigna" type="hidden" value="RADICACION_ENTRANTE" runat="server"/>
            <input id="Hidden_radicado_seleccion" type="hidden" value="" runat="server"/>
            <input id="Hidden_numero_rad_pend" type="hidden" value="0" runat="server"/>
             <div id="cler" style="clear: both"></div>      
            <asp:Panel ID="Panel_lista_actividades_worflow_ruta" runat="server" Style="display:none;  width: 80%; height:100%" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_lista_actividades_worflow_ruta"  runat="server"   
                    TargetControlID="ButtonSalir_lista_actividades_worflow_ruta" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_lista_actividades_worflow_ruta" PopupControlID="Panel_lista_actividades_worflow_ruta" ></asp:ModalPopupExtender>
                <div id="modal_content_lista_actividades_worflow_ruta" class="modal-content">
                    <div id="divcabecer2_lista_actividades_worflow_ruta" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title d-inline ">Enviar tarea</h6>
                        <button type="button" value="Button_cerrar_lista_actividades_worflow_ruta" class="close da_event_captive ">&times;</button>
                    </div>
                    <div id="contenido_procesa_lista_actividades_workflow" style="background-color: white; width: 100%; height: 99%" class="p-1">
                        <asp:UpdatePanel ID="UpdateGeneral_documentos" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <div id="contenido_titulo_data_grid_dos_title" style="width: auto; height: auto; border-bottom: 1px solid #e9ecef"  class="border_superior_radius_   row p-2 ml-1 mr-1">
                                    <div class="col-6 ">
                                         <asp:Label ID="titulo_label_grid" runat="server"  CssClass="h6 font-weight-light "  > Resultados busqueda</asp:Label>
                                    </div>
                                    <div class="col-6  text-right">
                                         <asp:Label ID="Label_nombre_flujo" runat="server" CssClass="h6 font-weight-light "></asp:Label>
                                    </div>
                                     
                                </div>
                                <input id="HiddenEstado" type="hidden" value="1" runat="server"/>
                                <div id="div_gred" style="  overflow: auto">
                                    <asp:GridView ID="GridView_envia_flujo" runat="server" Style="width:100%"
                                        AutoGenerateSelectButton="False" CssClass="filtrar table font-weight-light" GridLines="None" >
                                        <SelectedRowStyle BackColor="LightSkyBlue"  />
                                        <HeaderStyle CssClass="GridviewScrollHeader_line_boot" />
                                        <Columns>
                                            <asp:BoundField HeaderText="DETALLE" />
                                        </Columns>
                                    </asp:GridView>

                                </div>
                            </ContentTemplate>

                        </asp:UpdatePanel>
                        <div id="div_contenido_procesa_lista_actividades_worflow_ruta_botones_desicion" class=" modal-footer" >
                          
                            
                        </div>
                    </div>
                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button_lista_actividades_worflow_ruta" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="ButtonSalir_lista_actividades_worflow_ruta" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="Button_cerrar_lista_actividades_worflow_ruta" runat="Server" Text="" CssClass="invisible" />
                        
                    </div>
                </div>
            </asp:Panel>
            <!--evniar actividad-->
            <asp:Panel ID="Panel_lista_actividades_ruta" runat="server" Style="display:none;  width: 80%; height:100%" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_lista_actividades_ruta"  runat="server"   
                    TargetControlID="ButtonSalir_lista_actividades_ruta" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_lista_actividades_ruta" PopupControlID="Panel_lista_actividades_ruta" ></asp:ModalPopupExtender>
                <div id="modal_content_lista_actividades_ruta" class="modal-content">
                    <div id="divcabecer2_lista_actividades_ruta" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title d-inline ">Enviar a grupo</h6>
                        <button type="button" value="Button_cerrar_lista_actividades_ruta" class="close da_event_captive ">&times;</button>
                    </div>
                    <div id="contenido_procesa_lista_actividades_ruta" style="background-color: white; width: 100%; height: 99%" class="p-1">
                        <asp:UpdatePanel ID="UpdateGeneral_lista_actividades_ruta" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <div id="contenido_titulo_data_grid_lista_actividades_ruta" style="width: auto; height: auto; border-bottom: 1px solid #e9ecef"  class="border_superior_radius_   row p-2 ml-1 mr-1">
                                    <div class="col-6 ">
                                         <asp:Label ID="titulo_label_lista_actividad_ruta" runat="server"  CssClass="h6 font-weight-light "  ></asp:Label>
                                    </div>
                                    <div class="col-6  text-right">
                                        <div class="input-group ">
                                            <button id="Button_restore_lista_actividades" class="btn btn-outline-secondary border-right-2 " title="Restaura lista" style="border-top-right-radius: 0px; border-bottom-right-radius: 0px" onclick="preven_event_restor_search_lista_actividad(event,this)" type="button">
                                                <i class="fal fa-long-arrow-left"></i>
                                            </button>
                                            <asp:TextBox ID="TextBox_buequeda_general_lista_actividades" runat="server" class="form-control form-control-sm complex  border-left-0" placeholder="Busqueda...." onkeypress="preven_event_search_keypres_enter_lista_actividad(event,this);"></asp:TextBox>
                                            <div class="input-group-append">
                                                <button class="btn btn-outline-secondary" title="Consultar" onclick="preven_event_search_lista_actividad(event,this)" type="button">
                                                    <i class="fal fa-search"></i>
                                                </button>
                                            </div>
                                        </div>
                                    </div>                      
                                </div>
                                <input id="Hidden_sel_actividad" type="hidden" value="-1" runat="server"/>
                                <div id="div_gred_actividades" style="  overflow: auto">
                                    <asp:GridView ID="GridView_envia_actividades" runat="server" Style="width:100%"
                                        AutoGenerateSelectButton="False" CssClass="filtrar table font-weight-light" GridLines="None" >
                                        <SelectedRowStyle BackColor="LightSkyBlue"  />
                                        <HeaderStyle CssClass="GridviewScrollHeader_line_boot" />
                                        <Columns>
                                            <asp:BoundField HeaderText="DETALLE" />
                                        </Columns>
                                    </asp:GridView>

                                </div>
                            </ContentTemplate>

                        </asp:UpdatePanel>
                        <div id="div_contenido_procesa_lista_actividades_ruta_botones_desicion" class=" modal-footer" >
                          
                            
                        </div>
                    </div>
                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button_lista_actividades_ruta" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="ButtonSalir_lista_actividades_ruta" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="Button_cerrar_lista_actividades_ruta" runat="Server" Text="" CssClass="invisible" />
                        
                    </div>
                </div>
            </asp:Panel>
        <asp:Panel ID="Panel_sube_documento_content_general" runat="server" Style="display:none;  width: 50%; height: auto" CssClass="modal_content_general_">
                <asp:ModalPopupExtender ID="ModalPopupExtender_sube_documento_content_general" runat="Server" BackgroundCssClass="FondoAplicacion" 
                    TargetControlID="Button_sube_documento_content_general"
                    PopupControlID="Panel_sube_documento_content_general" CancelControlID="Button3_sube_documento_content_general" ></asp:ModalPopupExtender>
                <div id="modal_content_sube_documento_content_general" class="modal-content">  
                    <div id="divcabecer2_sube_documento_content_general" class="modal_title_superior_ modal-header"> 
                        <h6 class="modal-title d-inline ml-1">Guardar rotulo radicado</h6>  
                        <button type="button" value="Button3_sube_documento_content_general" class="close da_event_captive ">&times;</button>                   
                    </div>            
                    <div id="Div_content_sube_documento_content_general" style="height: auto; width: 100%; border-top: none" class="modal_content_back p-2">
                         <asp:DropDownList ID="DropDownList_documento_sube_documento_content_general" Style="width: 100%" CssClass="custom-select mr-sm-2" runat="server"></asp:DropDownList>       
                    </div>
                    <div id="content_boton_sube_documento_content_general" class="modal-footer justify-content-end">
                       <input type="submit" name="Button_rotulo_guarda" value="Aceptar" id="Button_guarda_rotulo" class="btn btn-success" onclick="event_element_clic(event,this);" tabindex="0"/>
                   </div>
                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button_sube_documento_content_general" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                        <asp:Button ID="Button3_sube_documento_content_general" runat="Server" Text="" CssClass="invisible" />
                    </div>
                </div>
            </asp:Panel>    
         <!--enviar usuario-->
            <asp:Panel ID="Panel_lista_usuarios_ruta" runat="server" Style="display:none;  width: 80%; height:100%" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_lista_usuarios_ruta"  runat="server"   
                    TargetControlID="ButtonSalir_lista_usuarios_ruta" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_lista_usuarios_ruta" PopupControlID="Panel_lista_usuarios_ruta" ></asp:ModalPopupExtender>
                <div id="modal_content_lista_usuarios_ruta" class="modal-content">
                    <div id="divcabecer2_lista_usuarios_ruta" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title d-inline ">Enviar a usuario</h6>
                        <button type="button" value="Button_cerrar_lista_usuarios_ruta" class="close da_event_captive ">&times;</button>
                    </div>
                    <div id="contenido_procesa_lista_usuarios_ruta" style="background-color: white; width: 100%; height: 99%" class="p-1">
                        <asp:UpdatePanel ID="UpdateGeneral_lista_usuarios_ruta" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <div id="contenido_titulo_data_grid_lista_usuarios_ruta" style="width: auto; height: auto; border-bottom: 1px solid #e9ecef"  class="border_superior_radius_   row p-2 ml-1 mr-1">
                                    <div class="col-6 ">
                                         <asp:Label ID="titulo_label_lista_usuario_ruta" runat="server"  CssClass="h6 font-weight-light "  ></asp:Label>
                                    </div>
                                    <div class="col-6  text-right">
                                        <div class="input-group ">
                                            <button id="Button_restore_lista_usuarios" class="btn btn-outline-secondary border-right-2 " title="Restaura lista" style="border-top-right-radius: 0px; border-bottom-right-radius: 0px" onclick="preven_event_restor_search_lista_usuario(event,this)" type="button">
                                                <i class="fal fa-long-arrow-left"></i>
                                            </button>
                                            <asp:TextBox ID="TextBox_buequeda_general_lista_usuarios" runat="server" class="form-control form-control-sm complex  border-left-0" placeholder="Busqueda...." onkeypress="preven_event_search_keypres_enter_lista_usuario(event,this);"></asp:TextBox>
                                            <div class="input-group-append">
                                                <button class="btn btn-outline-secondary" title="Consultar" onclick="preven_event_search_lista_usuario(event,this)" type="button">
                                                    <i class="fal fa-search"></i>
                                                </button>
                                            </div>
                                        </div>
                                    </div>
                                     
                                </div>
                                <div id="div_gred_usuarios" style="  overflow: auto">
                                    <asp:GridView ID="GridView_envia_usuario" runat="server" Style="width:100%"
                                        AutoGenerateSelectButton="False" CssClass="filtrar table font-weight-light" GridLines="None" >
                                        <SelectedRowStyle BackColor="LightSkyBlue"  />
                                        <HeaderStyle CssClass="GridviewScrollHeader_line_boot" />
                                        <Columns>
                                            <asp:BoundField HeaderText="DETALLE" />
                                        </Columns>
                                    </asp:GridView>

                                </div>
                            </ContentTemplate>

                        </asp:UpdatePanel>
                        <div id="div_contenido_procesa_lista_usuarios_ruta_botones_desicion" class=" modal-footer" >
                          
                            
                        </div>
                    </div>
                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button_lista_usuarios_ruta" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="ButtonSalir_lista_usuarios_ruta" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="Button_cerrar_lista_usuarios_ruta" runat="Server" Text="" CssClass="invisible" />
                        
                    </div>
                </div>
            </asp:Panel>
            <!--detalle_actividad_flujo-->     
            <asp:Panel ID="Panel_detalle_actividad_flujo" runat="server" Style="display:none;  width:50%; height:auto" >
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_detalle_actividad_flujo"  runat="server"  TargetControlID="ButtonSalir_detalle_actividad_flujo" 
                     BackgroundCssClass="FondoAplicacion" 
                    CancelControlID="Button_cerrar_detalle_actividad_flujo" PopupControlID="Panel_detalle_actividad_flujo"   ></asp:ModalPopupExtender>
                <div id="modal_content_actividad_flujo" class="modal-content">
                    <div id="divcabecer2_detalle_actividad_flujo" class="modal-header">
                         <h6 class="modal-title d-inline ">Info</h6>
                         <button type="button" value="Button_cerrar_detalle_actividad_flujo" class="close da_event_captive">&times;</button>   
                    </div>
                    <div id="contenido_procesa_detalle_actividad_flujo" style="overflow:auto " class="pl-1 pr-1">
                        <asp:UpdatePanel ID="UpdatePanel_detalle_actividad_flujo" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div style="">
                                    <table style="" class="table ">
                                        <tr>
                                            <td style="background-color: #E7EDF5">
                                                <span class="h6"> Nombre actividad </span>
                                               
                                            </td>
                                            <td style="">
                                                <asp:Label ID="Label_nombre_actividad" runat="server" Text=""  CssClass="h6 font-weight-light"></asp:Label>
                                            </td>

                                        </tr>
                                        <tr>
                                            <td style="background-color: #E7EDF5">
                                                <span class="h6"> Descripción actividad </span>
                                                
                                            </td>
                                            <td style="">
                                                <asp:Label ID="Label_descripcion" runat="server" Text="" CssClass="h6 font-weight-light"></asp:Label>
                                            </td>

                                        </tr>
                                        <tr>
                                            <td style="background-color: #E7EDF5">
                                                <span class="h6">Tipo actividad</span>
                                                
                                            </td>
                                            <td style="">
                                                <asp:Label ID="Label_tipo_actividad" runat="server" Text="" CssClass="h6 font-weight-light"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="background-color: #E7EDF5">
                                                <span class="h6">
                                                    Usuarios relacionados
                                                </span>
                                                
                                            </td>
                                            <td style="overflow: auto; max-height:200px;">
                                                <div style="overflow: auto">
                                                    <asp:Label ID="Label_usuario_relacionados" runat="server" Text="" CssClass="h6 font-weight-light"></asp:Label>
                                                </div>


                                            </td>

                                        </tr>

                                    </table>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                    </div>
                </div>
                <div style="display:none; height:1px">
                    <asp:Button ID="Button_detalle_actividad_flujo" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="ButtonSalir_detalle_actividad_flujo" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="Button_cerrar_detalle_actividad_flujo" runat="Server" Text="" CssClass="invisible"/>
                 </div>
            </asp:Panel>
            <div style="display:none">
                  <asp:UpdatePanel ID="UpdatePanel_enviar_actividad" runat="server" UpdateMode="Conditional">
                  <ContentTemplate>
                      <asp:Button ID="Button_activa_enviar_actividad_flujo_trabajo" runat="server" Text="" style="display:none"   />
                      <asp:Button ID="Button_detalle_enviar_actividad_flujo_trabajo" runat="server" Text="" style="display:none"   />
                            <input id="Hidden_id_actividad_flujo" type="hidden" value="0" runat="server"/>
                            <input id="Hidden_id_flujo_trabjo" type="hidden" value="0" runat="server"/>
                            <input id="Hidden_id_actividad_destino" type="hidden" value="0" runat="server"/>
                            <input id="Hidden_id_usuario_workflow" type="hidden" value="0" runat="server"/>
                            <input id="Hidden_id_conector" type="hidden" value="0" runat="server"/>
                            <input id="Hidden_resul_eviar_actividad" type="hidden" value="" runat="server"/>
                  </ContentTemplate>
              </asp:UpdatePanel>
              </div>
             <!--detalle_actividad_flujo_user-->   
            <asp:Panel ID="Panel_detalle_actividad_flujo_user" runat="server" Style="display:none; width:50%; height:auto" >
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_detalle_actividad_flujo_user"  runat="server"  TargetControlID="ButtonSalir_detalle_actividad_flujo_user" BackgroundCssClass="FondoAplicacion" 
                    CancelControlID="Button_cerrar_detalle_actividad_flujo_user" PopupControlID="Panel_detalle_actividad_flujo_user"   ></asp:ModalPopupExtender>
                <div id="modal_content_detalle_actividad_flujo_user" class="modal-content">
                    <div id="divcabecer2_detalle_actividad_flujo_user" class="modal-header">
                        <h6 class="modal-title d-inline">Info</h6>
                        <button type="button" value="Button_cerrar_detalle_actividad_flujo_user" class="close da_event_captive">&times;</button> 
                    </div>
                    <div id="contenido_procesa_detalle_actividad_flujo_user" style="overflow:auto " class="pl-1 pr-1 table">
                        <asp:UpdatePanel ID="UpdatePanel_detalle_actividad_flujo_user" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div style="">
                                    <table style="width:100%">
                                        <tr>
                                            <td style="background-color: #E7EDF5">
                                                <span class="h6">Nombre usuario</span>
                                            </td>
                                            <td >
                                                <asp:Label ID="Label_nombre_usuario" runat="server" Text="" CssClass="h6 font-weight-light"></asp:Label>
                                            </td>

                                        </tr>
                                        <tr>
                                            <td style="background-color: #E7EDF5">
                                                <span class="h6">Cargo usuario</span>         
                                            </td>
                                            <td >
                                                <asp:Label ID="Label_cargo" runat="server" Text="" CssClass="h6 font-weight-light"></asp:Label>
                                            </td>

                                        </tr>
                                        <tr>
                                            <td style="background-color: #E7EDF5">
                                                <span class="h6">Correo electrónico</spa>
                                            </td>
                                            <td>
                                                <asp:Label ID="Label_correo" runat="server" Text="" CssClass="h6 font-weight-light"></asp:Label>
                                            </td>

                                        </tr>
                                        <tr>
                                            <td style="background-color: #E7EDF5">
                                                <span class="h6">Grupo usuario</span>
                                            </td>
                                            <td >
                                                <asp:Label ID="Label_nombre_grupo" runat="server" Text="" CssClass="h6 font-weight-light"></asp:Label>
                                            </td>

                                        </tr>
                                    </table>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                    </div>
                    <div style="display:none; height:1px">
                    <asp:Button ID="Button_detalle_actividad_flujo_user" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="ButtonSalir_detalle_actividad_flujo_user" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="Button_cerrar_detalle_actividad_flujo_user" CssClass="invisible" runat="Server" Text=""/>
                    </div>
                </div>
            </asp:Panel>
        
         
        <!--popup de auto terminar documento-->
             <asp:Panel ID="Panelmensaj_autoterminar" runat="server" Style="display:none; width:50%; height:auto" CssClass="modal_content_general">
                  <asp:ModalPopupExtender ID="ModalPopupExtendermensaje_autoterminar" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_auto" 
                      PopupControlID="Panelmensaj_autoterminar" CancelControlID="Button_auto">
                  </asp:ModalPopupExtender>
                 <div id="modal_content_autoterminar" class="modal-content">
                     <div id="Div2_autoterminar"  class="modal_title_superior_ modal-header">
                         <h6 class="modal-title d-inline ">Enviar a gestión</h6>
                         <button type="button" value="Button_auto" class="close da_event_captive ">&times;</button>                     
                     </div>
                     <div id="container_autoterminar" style="height: auto; width: auto" class="modal_content_back">
                         <asp:UpdatePanel ID="updatepanel_mensaje_extender_autoterminar" runat="server" UpdateMode="Conditional">
                             <ContentTemplate>
                                 <div id="Contenido_autoterminar" style="text-align:center" class="text-justify p-2">
                                     <br />
                                     <asp:Label ID="LabelMensaje_autoterminar" runat="server" Text="No mensage"   class="h6 font-weight-light " />
                                     <br />
                                     <br />
                                     <asp:CheckBox ID="CheckBox_autoterminar" runat="server" ForeColor="Black" Checked="true" Text="Enviar correo de notificación" class="h6" style="display:none" />
                                 </div>
                                 <input id="Hidden_id_actividad" type="hidden" value="0" runat="server"/>
                                 <input id="Hidden_id_usuario" type="hidden" value="0" runat="server"/>
                             </ContentTemplate>
                         </asp:UpdatePanel>
                     </div>
                     <div class="modal-footer justify-content-end">
                         <asp:UpdatePanel ID="UpdatePanel_boton_auto_terminar" runat="server" UpdateMode="Conditional">
                             <ContentTemplate>
                                 <asp:Button ID="btnOkay_autoterminar" runat="server" Text="Aceptar " CssClass="btn btn-success" />
                                     <asp:Button ID="btnCancel_autoterminar" runat="server" Text="Cancelar " CssClass="btn btn-secondary" />
                                     <input id="Hidden_rest_auto_terinar" type="hidden" value="" runat="server"/>
                             </ContentTemplate>
                         </asp:UpdatePanel>
                     </div>
                     <div style="display:none; height:1px">
                         <asp:Button ID="Button_auto" Style="display: none" runat="server" Text="Button" Height="0px" Width="0px" />
                     </div>
                 </div>
              </asp:Panel>
        <asp:Panel ID="Panel_list_registro_rad" runat="server" Style="display: none; width: 70%; height: 100%" CssClass="modal_content_general_">
            <asp:ModalPopupExtender ID="ModalPopupExtender_edition_list_registro_rad" runat="server"
                TargetControlID="ButtonSalir_list_registro_rad" BackgroundCssClass="FondoAplicacion"
                CancelControlID="Button_cerrar_list_registro_rad" PopupControlID="Panel_list_registro_rad">
            </asp:ModalPopupExtender>
            <div id="modal_content_list_registro_rad" class="modal-content">
                <div id="diver_cabcera_list_registro_rad" class="modal_title_superior_ modal-header">
                    <h6 class="modal-title d-inline ">Registros</h6>
                    <button type="button" value="Button_cerrar_list_registro_rad" class="close da_event_captive ">&times;</button>
                </div>
                <div id="contenido_procesa_list_registro_rad" style="background-color: white; width: 100%; height: 100%; border-top: none" class="modal_content_back modal-body">
                    <asp:UpdatePanel ID="Update_list_registro_rad" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div id="contenido_titulo_list_registro_rad" class="mb-2">
                                <input id="Hidden_list_registro_rad" type="hidden" value="-1" runat="server"/>
                                <asp:Label ID="titulo_label_list_registro_rad" runat="server" class="h6 font-weight-light">Resultados busqueda</asp:Label>
                            </div>
                            <div id="content_data_grid_list_registro_rad" class="conten_gred_border_" style="overflow: auto; width: 100%">
                                <asp:GridView ID="GridView_list_registro_rad" runat="server" Style="position: inherit; width: 100%; font-size: 14px"
                                    AutoGenerateSelectButton="False" AllowSorting="true" AllowPaging="true" PageSize="6" PagerSettings-Position="Top" CssClass="table  font-weight-light" GridLines="None"
                                    EnableViewState="true">
                                    <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                    <HeaderStyle CssClass="GridviewScrollHeader_line_boot" />
                                    <PagerStyle CssClass="pagination-ys" />
                                    <Columns>
                                        <asp:BoundField HeaderText="OPCIONES" />
                                    </Columns>
                                </asp:GridView>
                            </div>
                             
                                 
                        </ContentTemplate>

                        <Triggers>
                        </Triggers>
                    </asp:UpdatePanel>

                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button_list_registro_rad" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                        <asp:Button ID="ButtonSalir_list_registro_rad" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                        <asp:Button ID="Button_cerrar_list_registro_rad" runat="Server" Text="X" CssClass="invisible" />
                    </div>
                </div>

            </div>
        </asp:Panel>
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

                    </div>
                </div>
            </asp:Panel>     
        </div>
        <!--POPUP DE VALIDACION DE RADICADOS-->   
            <asp:Panel ID="Panel_Val_Radicacion" runat="server" Style="display:none; width: 100%; height: auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_Val_Radicado" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_Abrir_Val_Radicacion"
                    PopupControlID="Panel_Val_Radicacion" CancelControlID="Buttoncacerrar_Val_Radicacion">
                </asp:ModalPopupExtender>
                <div id="divcabecera_val_radicacion" class="modal_title_superior_ modal-header">
                    <h6 class="modal-title d-inline ml-1">Busqueda radicados</h6>
                    <button type="button" value="Buttoncacerrar_Val_Radicacion" class="close" onclick="close_popup_valida();">&times;</button>
                </div>
                <div id="Diupdate_val_radciacion" style="height: auto; width: 100%; " class="modal_content_back ">
                    <div id="contenido_izquierdo_val_radicacion" style="width: 30%; height: 100%; float: left; margin-left: 3px">
                        <div id="contenido_titulo_campos_consulta" style=" border-top-left-radius: initial; border-top-right-radius: initial" class="modal-header modal_title_superior bg-light p-2">
                            <h6 class="  font-weight-light" id="_titulo_campos_consulta" style="font-family: 'Segoe UI'">Campos de busqueda </h6>
                        </div>
                        <div id="contenido_consulta_val_radicacion" style="height: 80%; width: 100%; border-top: 1px solid #dee2e6" >
                            <asp:UpdatePanel ID="UpdatePanelContenido_val_radicacion" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Panel ID="_Panelvalidacion_val_radicacion" runat="server" Style="overflow: auto; height: auto" CssClass="p-1">
                                        <asp:Table ID="_ValidacionConsulta_val_radicacion" runat="server"  ViewStateMode="Enabled" Wrap="false" Width="100%" >
                                        </asp:Table>
                                    </asp:Panel>
                                    <input id="Hiddentramiteseleccionvalue" type="hidden" value="" runat="server"/>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                        <div id="contenido_botones_val_radicacion" style="border-top-left-radius: initial; border-top-right-radius: initial" class="p-2 bg-light">
                            <asp:UpdatePanel ID="UpdatePanel_botones_val_radicacion" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <input id="Hidden_resultado_consulta_previa" type="hidden" value="" runat="server"/>
                                    <asp:Button ID="Button_consulta_val_radicacion" runat="server" Text="Consultar" ToolTip="Consultar radicados" CssClass="btn btn-success" />
                                    <asp:Button ID="Button_lipiar_val_radicacion" Text="Limpiar" runat="server" ToolTip="Limpiar campos radicacion" CssClass="btn btn-success" />
                                </ContentTemplate>
                            </asp:UpdatePanel>

                        </div>
                    </div>
                    <div id="contenido_derecho_validacion_radicados" style="width: 69%; float: right; height: 100%; margin-right: 2px">
                        <div id="contenido_titulo_val_radicacion" style=" width: 100%" class=" p-2">
                            <asp:UpdatePanel ID="UpdatePanelabel_val_radicacion" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <input id="hdnEmailID_VAL" type="hidden" value="-1" runat="server"/>
                                    <asp:Label ID="titulo_label_val_radicacion" runat="server" CssClass="h6 font-weight-light p-1" Style="font-family:'Segoe UI'">Resultados busqueda</asp:Label>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                        <div id="contenido_datagrid_val_radicacion" style=" width: 100%; position: relative; overflow:auto; border-top: 1px solid #dee2e6">
                            <asp:UpdatePanel ID="UpdatePanel_conenido_grid_val_radicacion" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:GridView ID="GridView_val_radicacion" runat="server" AllowSorting="true"  AllowPaging="true"  EnableViewState="true"
                                    PageSize  ="7" PagerSettings-Position="Top"  style=" font-family:Segoe UI"
                                    AutoGenerateSelectButton="False" CssClass="table font-weight-light  " GridLines="None"  >
                                    <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                    <HeaderStyle CssClass="GridviewScrollHeader_line_boot" BorderStyle="None" />
                                    <RowStyle CssClass=""  />
                                    <PagerStyle CssClass="pagination-ys" />
                                    <Columns>  
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                                <asp:Panel ID="Panel_che_box_aling" runat="server" Style="text-align: center">
                                                    
                                                </asp:Panel>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkSelection" runat="server" onclick="inactiva_chek();" CssClass="jjjjjjjjjjj btn btn-light btn-sm border-0 bg-transparent" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                                </ContentTemplate>

                            </asp:UpdatePanel>
                        </div>
                        <div id="Contenido_botones_tipo_radicado" style=" width: 100%" class="modal-footer">
                            <asp:UpdatePanel ID="UpdatePanelabel_buton_asignacion_val_radicacion" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Button ID="Button_cerrar_ventana_date" runat="server"   Text="Copia datos"   ToolTip="Copia los datos del radicado seleccionado en la lista para un nuevo radicado" CssClass="btn btn-success d-none" />
                                    <asp:Button ID="Button_Asignar_nuevo_radicado" runat="server"   Text="Copia datos"   ToolTip="Copia los datos del radicado seleccionado en la lista para un nuevo radicado" CssClass="btn btn-success d-none" />
                                    <asp:Button ID="Button_Asignar_relacionado_expediente" Text="Anexo a expediente " runat="server"  ToolTip="Anexa la radicación al expediente del radicado seleccionado en la lista" CssClass="btn btn-success d-none" />
                                    <input id="Hidden_selecion_radicado" type="hidden" value="" runat="server"/>
                                    <asp:Button ID="Button_Asignar_radicado_relacionado" Text="Relacionar" runat="server" 
                                        ToolTip="Relaciona el nuevo radicado con los radicados chekeados en la lista" CssClass="btn btn-success mr-2" OnClientClick="retorna_check_radicados_gred();" />
                                    <asp:CheckBox ID="CheckBox_val_remplaza" runat="server" Style="position: relative" CssClass="custom-checkbox" Text="" Checked="true" ForeColor="Red" Font-Size="10" Font-Names="Arial" />
                                    <span>Remplaza</span>
                                    <asp:CheckBox ID="CheckBox_val_agrega" runat="server" Style="position: relative" CssClass="custom-checkbox" Text="" Checked="false" ForeColor="Red" Font-Size="10" Font-Names="Arial" />
                                    <span>Agrega</span>
                                    <asp:MutuallyExclusiveCheckBoxExtender ID="MutuallyExclusiveCheckBoxExtender1" runat="server" TargetControlID="CheckBox_val_remplaza"
                                        Key="radicado_plus"></asp:MutuallyExclusiveCheckBoxExtender>
                                    <asp:MutuallyExclusiveCheckBoxExtender ID="MutuallyExclusiveCheckBoxExtender2" runat="server" TargetControlID="CheckBox_val_agrega"
                                        Key="radicado_plus"></asp:MutuallyExclusiveCheckBoxExtender>
                                    <input id="Hidden_resultado_asignacion_radicado" type="hidden" value="" runat="server"/>
                                </ContentTemplate>

                            </asp:UpdatePanel>

                        </div>
                    </div>

                </div>
                <div style="display: none; height: 1px">
                    <asp:Button ID="Button1_val_radicacion" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" Style="display: none" />
                    <asp:Button ID="Button_Abrir_Val_Radicacion" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" Style="display: none" />
                    <asp:Button ID="Buttoncacerrar_Val_Radicacion" runat="Server" Text="" CssClass="invisible" Height="1px" Width="1px" Style="display: none"
                        OnClientClick="hiden_popup_resize_popup_validacion_radicados();" />
                </div>
            </asp:Panel>   
        <div id="Destinatarioguia">
            <asp:Panel ID="Paneldestinatario" runat="server"  Style="display:none; color: White; width: auto; height: auto">     
                <asp:ModalPopupExtender ID="ModalPopupExtenderdestinatario" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonDAPCERRAR"
                    PopupControlID="Paneldestinatario" CancelControlID="Buttoncacerrar"></asp:ModalPopupExtender>
                <div id="divcabecer" class="cabecera2">
                    <asp:Button ID="Buttond2" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonDAPCERRAR" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Label ID="Label4" runat="server" Text="Gestón externos" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton" style="float: right">
                        <asp:Button ID="Buttoncacerrar" runat="Server" Text="X"
                            ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="Diupdate" style="border: thin double #000080; color: White; background-color: #FFFFFF; height: auto; width: auto">      
                         <div id="contenido_general" style="height: 469px; width: 99%">
                                        <div id="contenido_consulta" style="height: 184px; width: 99%; float: left; margin-top:10px; margin-left:5px">
                                             
                                            <asp:UpdatePanel ID="UpdatePanelContenido" runat="server" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <asp:Panel ID="_Panelvalidacion" runat="server" ScrollBars="Vertical" Height="183px" Wrap="false">       
                                                        <asp:Table ID="_ValidacionConsulta" runat="server" ForeColor="White" BackColor="White" ViewStateMode="Enabled" Height="50" Wrap="false">
                                                        </asp:Table>
                                                    </asp:Panel>                                
                                                    <div id="Divsepara4" style="height: 1px; display: none">
                                                         <asp:TextBox ID="TextBoxEditNombreDestRem" runat="server"></asp:TextBox>
                                                            <input id="Hiddenselecionpais" type="hidden" runat="server"/>
                                                            <input id="Hiddenseleciondepartamento" runat="server" value="" type="hidden"/>
                                                            <input id="Hiddenvalidacion" type="hidden" value="" runat="server"/> 
                                                            <input id="Hiddenmunicipio" runat="server" value="" type="hidden"/>
                                                            <input id="Hiddenestadoedicion" runat="server" value="0" type="hidden"/>
                                                            <input id="Hiddenrelacionvalidacion" runat="server" type="hidden" value="-1"/>                   
                                                             <input id="Hidden_height" type="hidden" value="0" runat="server"/>
                                                             <input id="Hidden_width" type="hidden" value="0" runat="server"/>
                                                   </div>
                                                </ContentTemplate>
                                                <Triggers>
                                                    <asp:AsyncPostBackTrigger ControlID="Buttonllenardepartamento" EventName="Click" />
                                                    <asp:AsyncPostBackTrigger ControlID="Buttonllenarciudad" EventName="Click" />
                                                    <asp:AsyncPostBackTrigger ControlID="asignar" EventName="Click" />
                                                    <asp:AsyncPostBackTrigger ControlID="Buttonactualizar_ra_val" EventName="Click" />
                                                    <asp:AsyncPostBackTrigger ControlID="Button_Asignar_nuevo_radicado" EventName="Click" />
                                                </Triggers>
                                            </asp:UpdatePanel>

                                        </div>
                                        <div id="Divsepara2" style="height: 1px; display: none">
                                            <asp:Button ID="Buttonllenardepartamento" runat="server" Text="Button" BackColor="Silver" />
                                            <asp:Button ID="Buttonllenarciudad" runat="server" Text="Button" />
                                            <asp:Button ID="Buttonactualizar_ra_val" runat="server" Text="Button" />
                                        </div>
                                        <div id="contenido_titulo" style="height: 20px; width: 100%; background-color: #E7EDF5; float: left">
                                            <asp:UpdatePanel ID="UpdatePanelabel" runat="server" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <asp:Label ID="titulo_label" runat="server" ForeColor="Black" Font-Size="small">Resultados busqueda</asp:Label>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>

                                        </div>
                                        <div id="contenido_datagrid" style="height: 220px; width: 100%; position: relative; float: right;color: black;">
                                            <asp:Panel ID="Cosulta_valid" runat="server" ScrollBars="Horizontal" >
                                                <asp:UpdatePanel ID="UpdateGeneral" runat="server" UpdateMode="Conditional">
                                                    <ContentTemplate>
                                                        <asp:GridView ID="data_grid" runat="server"
                                                           AutoGenerateSelectButton="False" AllowPaging="true" PageSize="5" Font-Size="11px" PagerSettings-Position="Top" AllowSorting="false" style="width:100%">
                                                             <RowStyle VerticalAlign="Middle" />
                                                            <FooterStyle BackColor="White" ForeColor="#000066" />
                                                            <PagerSettings />
                                                            <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" Font-Size="10px" />
                                                            <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />       
                                                            <HeaderStyle CssClass="GridviewScrollHeader_line" /> 
                                                             <RowStyle CssClass="GridviewScrollItem_line" /> 
                                                             <PagerStyle CssClass="GridviewScrollPager_line" /> 
                                                        </asp:GridView>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </asp:Panel>
                                        </div>

                                        
                                        <div id="tolbalboton" style="float: left; height: 30px; width: 100%; background-color: #E7EDF5">
                                            
                                            <asp:UpdatePanel ID="UpdatePanelbotones" runat="server" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                     
                                                                 <input id="Hiddenrespuesta" type="hidden" value="" runat="server"/>
                                                                 <asp:Button ID="asignar" runat="server" Width="80px" Text="Asignar" OnClientClick="asignar_validacion();" ToolTip="Asigna los datos sleccionados al registro radicado" CssClass="boton" />
                                                                   &nbsp
                                                                 <asp:Button ID="Editar_" runat="server" Width="80px" Text="Editar" ToolTip="Activa el boton editar datos" CssClass="boton" />
                                                                   &nbsp     
                                                                 <asp:Button ID="Eliminar" runat="server" Text="Eliminar" Width="80px" ToolTip="Eliminar registro seleccionado" OnClientClick="ConfirmMensajeEliminar(&quot;Desea Eliminar el registro &quot;);" CssClass="boton" />
                                   
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </div>
                                    </div>
                     

                    <div id="Divsepara" style="height: 1px; display: none">
                        <asp:Button ID="Buttonllenardepartamento1" runat="server" Text="Button" BackColor="Silver"  />
                        <asp:Button ID="Buttonllenarciudad1" runat="server" Text="Button" />
                        <input id="HiddenIDdestinatario" type="hidden" value="" runat="server"/>                                                
                        <input id="Hiddensel" type="hidden" value="0" runat="server"/>
                        <asp:UpdatePanel ID="updatepanel_detinatario_radicacion_exntrante_hiden" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <input id="hdnEmailID" type="hidden" value="-1" runat="server"/>
                            </ContentTemplate>   
                        </asp:UpdatePanel>
                    </div>

                </div>

            </asp:Panel>

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
                <asp:DragPanelExtender ID="DragPanelExtenderimpre_post" runat="server" TargetControlID="Panelimpresionpost" />
                <asp:ModalPopupExtender ID="ModalPopupExtenderimpre_post" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_post"
                    PopupControlID="Panelimpresionpost" CancelControlID="Buttoncerrarimpre_post">
                </asp:ModalPopupExtender>
                <div id="divcabecer2_post" class="cabecera2">
                    <asp:Button ID="Button1_post" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_post" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Label ID="Label2" runat="server" Text="Menu Impresion" Font-Size="10" Style="float: left">
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
        <!--POPUP QUE GUARDA EL POPUP CON EL CONTENEDOR DE LOS EXPEDIENTE-->    
            <asp:Panel ID="Panel_expdiente_popup" runat="server" Style="display:none;  width: 100%; height: 100%" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtende_expdiente_popup" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_expdiente_popup"
                    PopupControlID="Panel_expdiente_popup" CancelControlID="Buttoncerrar_expdiente_popup">
                </asp:ModalPopupExtender>
                <div id="modal_content_expdiente_popup" class="modal-content_">
                    <div id="divcabecer_expdiente_popup" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title">Gestión expedientes</h6>
                        <button type="button" value="Buttoncerrar_expdiente_popup" class="close da_event_captive ">&times;</button>
                    </div>
                    <div id="Contenido_expdiente_popup" style="color: black; height: 100%; width: 100%; border-top: none" class="modal_content_back pl-3 pr-3">
                        <asp:UpdatePanel ID="UpdatePanel_expdiente_popup" runat="server" UpdateMode="Conditional" style="height: 100%" RenderMode="Inline">
                            <ContentTemplate>
                                <iframe id="Iframe_expdiente_popup_" runat="server" style="width: 100%; height: 99%; overflow:no-content" frameborder="0"></iframe>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                    </div>
                    <div style="display: none; height: 1px">
                        <asp:Button ID="Buttoncerrar_expdiente_popup" runat="Server" Text="" Height="0px" Width="0px" Style="display: none" />
                        <asp:Button ID="Button_expdiente_popup" CssClass="invisible" runat="server" Text="" Height="0px" Width="00px" Style="display: none" />
                        <asp:Button ID="ButtonSalir_expdiente_popup" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" Style="display: none" />
                    </div>
                </div>
            </asp:Panel>
        
        <!--UPDATEPANEL QUE CONTIENE EL POPUP DE DESTINATARIOS INTERNOS -->
        <div id="ventana_auxiliar_destinatarios_internos_popup">
                    <asp:Panel ID="Panel_auxiliar_destinatarios_internos_popup" runat="server" Style="display:none; width: 99%; height: 100%" CssClass="border_superior_inferior_radius_blanco">
                        <asp:ModalPopupExtender ID="ModalPopupExtender_auxiliar_destinatarios_internos_popup" runat="Server"  BackgroundCssClass="FondoAplicacion"
                            TargetControlID="Button_abrir_auxiliar_destinatarios_internos_popup" 
                            PopupControlID="Panel_auxiliar_destinatarios_internos_popup" CancelControlID="Buttoncerrar_auxiliar_destinatarios_internos_popup"></asp:ModalPopupExtender>
                        <div id="divcabecer_auxiliar_destinatarios_internos_popup" class="modal_title_superior_ modal-header" >  
                            <h6 class="modal-title d-inline ml-1">Auxiliar destinatarios</h6>
                            <button type="button" value="Buttoncerrar_auxiliar_destinatarios_internos_popup" class="close da_event_captive">&times;</button>            
                        </div>
                         <div id="contedor_botones_auxiliar_destinatarios_internos_popup" class="modal-footer ">
                            <div class="input-group justify-content-end">
                                <button id="td-boton" class="btn btn-outline-secondary border-right-2 " title="Restaura lista destinatarios" style="border-top-right-radius: 0px; border-bottom-right-radius: 0px" onclick="preven_event_search_remit_interno_restore(event,this)" type="button">
                                    <i class="fal fa-long-arrow-left"></i>
                                </button>
                                <asp:TextBox ID="TextBoxcontenidobusqueda" runat="server" class="complex  border-left-0" style="min-width:300px"  placeholder="Buscar destinatario...." ></asp:TextBox>
                                <div class="input-group-append">
                                    <button class="btn btn-outline-secondary" title="Busqueda destinatarios" onclick="preven_event_search_remit_interno(event, this)" type="button">
                                        <i class="fal fa-search"></i>
                                    </button>
                                </div>
                            </div>

                        </div>         
                        <div id="Contenido_auxiliar_destinatarios_internos_popup" style="width: 99%; height:100%; overflow:auto" class="modal_content_back  pl-3 pr-3 pb-3">
                                <asp:UpdatePanel ID="UpdatePanel_auxiliar_destinatarios_internos_popup" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:GridView ID="data_grid_auxiliar_lista" runat="server" AllowSorting="true" EnableViewState="true" PageSize="7" 
                                            Style="font-family: Segoe UI" AllowPaging="true" PagerSettings-Position="Top"
                                            AutoGenerateSelectButton="False" CssClass="table font-weight-light" GridLines="None">
                                            <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                            <HeaderStyle CssClass="GridviewScrollHeader_line_boot" BorderStyle="None" />
                                            <RowStyle CssClass="" />
                                            <PagerStyle CssClass="pagination-ys" />
                                            <Columns>
                                                <asp:BoundField HeaderText="OPCIONES" />
                                            </Columns>
                                        </asp:GridView>
                                        <input id="Hidden_destinatario_interno" type="hidden" value="" runat="server"/>
                                        <input id="Hidden_auxiliar_id" type="hidden" value="-1" runat="server"/>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                           
                        </div>
                                    
                       
                        <div style="display:none; height:1px">
                             <asp:Button ID="Button_abrir_auxiliar_destinatarios_internos_popup" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px"  OnClientClick="auto_zise_popup_internos()" style="display:none" />
                              <asp:Button ID="Buttoncerrar_auxiliar_destinatarios_internos_popup" runat="Server" Text="X" CssClass="invisible"/>
                             <asp:UpdatePanel ID="UpdatePanel_botones_popup_interno" runat="server" UpdateMode="Conditional" >
                                <ContentTemplate>
                                    
                                    <input ID="Button_consulta_busqueda_auxiliar_destinatarios_internos_popup" value="Filtrar" type="button" class="boton_azul" />  &nbsp    
                                    <input type='checkbox' id='CheckboxBusqueda' />
                                    <label style="font:100; font-family:Arial; font-size:12px">Buscar sólo palabra completa</label>  &nbsp                                   
                                    <asp:Button ID="Button_asignar_auxiliar_destinatarios_internos_popup" Text="Asignar" runat="server"  CssClass="boton_azul" Style="display:none" />
                                    <asp:Button ID="Button_consulta_destinatario_interno" Text="Asignar" runat="server"  CssClass="boton_azul" Style="display:none" />
                                    <asp:Button ID="Button_consulta_destinatario_interno_restore" Text="Asignar" runat="server"  CssClass="boton_azul" Style="display:none" />
                                    
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                        
                    </asp:Panel>
        </div>
         
          <!--lista_actividades_worflow_ruta-->
          <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 200px">
                <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
                Processing ...
            </div>
     
        <!--mostrar dias horas habiles-->
        <div id="modal_dias_horas_habiles">
            <asp:Panel ID="Panel_dias_horas_habiles_popup" runat="server"  Style="display:none; width: 50%; height: 99%" CssClass="modal_content_general">              
                <asp:ModalPopupExtender ID="ModalPopupExtende_dias_horas_habiles_popup" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_dias_horas_habiles_popup"
                    PopupControlID="Panel_dias_horas_habiles_popup" CancelControlID="Buttoncerrar_dias_horas_habiles_popup" ></asp:ModalPopupExtender>
                <div id="divcabecer_dias_horas_habiles_popup" class="modal_title_superior_ modal-header" > 
                     <h6 class="modal-title d-inline ml-1">Días y horas hábiles para radicación</h6>
                    <button type="button" value="Buttoncerrar_dias_horas_habiles_popup" class="close da_event_captive">&times;</button>   
                </div>  
                 <div id="Contenido_dias_horas_habiles_popup" style="color: black; background-color: #FFFFFF; height: 95%; width: 99%" >                 
                     <div id="div_treview_archivo_r_u_e" style="height: 90%">
                         <asp:Panel ID="Paneltreview_r_u_e" runat="server" 
                              Style="position: inherit; height:99%; width:100%; overflow:auto">
                             <asp:UpdatePanel ID="UpdatePanelViewArchivo_r_u_e" runat="server" UpdateMode="Conditional">
                                 <ContentTemplate>
                                     <asp:TreeView ID="TreeViewArchivo_r_u_e" Style="text-align: left; padding-left: 1px; font-size: 10px; margin-top: 0px" runat="server" CssClass="TreeN" NodeWrap="true"
                                            PopulateNodesFromClient="False" EnableViewState="true"
                                            LeafNodeStyle-CssClass="LeafNodeStyle" Font-Size="11px" NodeIndent="10" ExpandDepth="0" SkipLinkText="">
                                            <HoverNodeStyle Font-Underline="False" />
                                            <LeafNodeStyle CssClass="LeafNodeStyle" HorizontalPadding="10px" NodeSpacing="0px" VerticalPadding="5px" />
                                            <NodeStyle ChildNodesPadding="5px" HorizontalPadding="0px" NodeSpacing="5px" VerticalPadding="5px" ForeColor="Black" />
                                            <ParentNodeStyle ChildNodesPadding="0px" ForeColor="#313131" Font-Bold="true" HorizontalPadding="5px" NodeSpacing="0px" VerticalPadding="5px" />
                                            <RootNodeStyle ChildNodesPadding="0px" ForeColor="#313131" Font-Bold="true" NodeSpacing="0px" VerticalPadding="5px" HorizontalPadding="5px" />
                                            <SelectedNodeStyle ForeColor="White" CssClass="node_select_" Font-Size="10px" ImageUrl="../workflow/imageneswf/iten_list_select.png" />
                                        </asp:TreeView>
                                 </ContentTemplate>
                             </asp:UpdatePanel>

                         </asp:Panel>
                     </div>         
              </div>
                 <div id="contendor_botones_unidad_u_b_t" class="border_inferior_radius_blanco modal-footer">
                         <asp:UpdatePanel ID="UpdatePanel_botones_unidad_r_u_e" runat="server" UpdateMode="Conditional">
                             <ContentTemplate>
                                 <asp:Button ID="Button_exportar" runat="server" Text="Exportar" CssClass="btn btn-success"  OnClientClick="fnExcelTre('TreeViewArchivo_r_u_e')" />
                             </ContentTemplate>
                         </asp:UpdatePanel>
                     </div>
                <div style="display: none; height: 1px">
                    <asp:Button ID="Button_dias_horas_habiles_popup" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" Style="display: none" />
                    <asp:Button ID="ButtonSalir_dias_horas_habiles_popup" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" Style="display: none" />
                    <asp:Button ID="Buttoncerrar_dias_horas_habiles_popup" runat="Server" TText="" Height="1px" Width="1px" Style="display: none" />
                </div>
                 
            </asp:Panel>
            
        </div>
         <input id="Hidden_id_tarea_sel" type="hidden" value="-1" runat="server"/>
           <input id="Hidden_tipo_visor" type="hidden" value="" runat="server"/>
               <!--Popup visor externo-->
               <asp:Panel ID="Panel_visor_externo" runat="server" Style="display:none; overflow:hidden" ForeColor="White" Width="100%" Height="100% " CssClass="modal_content_general" >
                  <asp:ModalPopupExtender ID="ModalPopupExtender_visor_externo" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_visor_externo"
                      PopupControlID="Panel_visor_externo"  CancelControlID="ButtonSalir_visor_externo">
                  </asp:ModalPopupExtender>
                  <div id="Cabecerapendiente_visor_externo" class="modal_title_superior_ modal-header" > 
                       <h6 class="modal-title d-inline ml-1">Documentos Relacionados</h6>
                       <button type="button" value="ButtonSalir_visor_externo" class="close da_event_captive">&times;</button>                        
                  </div>
                  <div id="Cotenedorpendiente_visor_externo" style="height: 100%; width: 100%; overflow:hidden" class="modal_content_back">  
                      <asp:UpdatePanel ID="UpdatePanel_visor_externo" runat="server" UpdateMode="Conditional">
                          <ContentTemplate>
                              <iframe id="Iframe_visor_externo_wf_" runat="server" frameborder="0"  style="width:100%; height:100%; overflow:hidden"></iframe>
                          </ContentTemplate>

                      </asp:UpdatePanel>
                      <div style="display: none; height: 0px">
                          <asp:Button ID="ButtonSalir_visor_externo" runat="Server" Text="X" CssClass="d-none"
                               />
                          <asp:Button ID="Button_visor_externo" Style="display: none" runat="server" Text="Button" Height="1px" Width="1px" />
                          <asp:UpdatePanel ID="UpdatePanel_visor_externo_boton" runat="server" UpdateMode="Conditional">
                              <ContentTemplate>
                                  <asp:Button ID="Button_visor_emergente" runat="server" Text="Button" Style="display: none" />
                              </ContentTemplate>
                          </asp:UpdatePanel>
                      </div>  
                  </div>

                 
              </asp:Panel>
              <!--lista_chequeo_actualiza-->         
            <asp:Panel ID="Panel_lista_chequeo_actualiza" runat="server" Style="display:none; width: 70%; height: 99%" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_lista_chequeo_actualiza" runat="server"  TargetControlID="ButtonSalir_lista_chequeo_actualiza" 
                     BackgroundCssClass="ModalBackgroud_gorund"
                    CancelControlID="Button_cerrar_lista_chequeo_actualiza" PopupControlID="Panel_lista_chequeo_actualiza" ></asp:ModalPopupExtender>
                <div id="divcabecer2_lista_chequeo_actualiza"  class="modal_title_superior_ modal-header"> 
                     <h6 class="modal-title d-inline ml-1">Tipos documentales relacionados al trámite</h6>
                     <button type="button" value="Button_cerrar_lista_chequeo_actualiza" class="close da_event_captive">&times;</button>   
                </div>
                <div id="contenido_procesa_lista_chequeo_actualiza" style="background-color: white; width: auto; height: 90%" class="modal_content_back pl-1 pr-1">
                    <input id="Hidden_0003" type="hidden" value="-1" runat="server"/>
                    <input id="Hidden_0004" type="hidden" value="1" runat="server"/>
                    <div id="contenido_titulo_data_grid_title_actualiza" style="width: 100%; margin-top: 1px; border-color: #b0c4de; border-width: 1px; border-style: ridge; display: none">
                        <asp:Label ID="Label16" runat="server" ForeColor="Black" Font-Size="12px" Style="font-weight: 600">Seleccione el tipo documento</asp:Label>
                    </div>
                    <div id="Contenedorgrid_edita" style="width: 99%; height: 100%">
                        <asp:UpdatePanel ID="UpdateGeneral_actualiza" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <asp:Panel ID="Panel_principal_actualiza" runat="server"
                                    Style="overflow: auto; width: 100%; width: 100%">
                                    <asp:GridView ID="data_grid_chequeo_actualiza" runat="server" AllowSorting="true" AllowPaging="false" EnableViewState="true"
                                        PageSize="7" PagerSettings-Position="Top" Style="font-family: Segoe UI"
                                        AutoGenerateSelectButton="False" CssClass="table font-weight-light  " GridLines="None">
                                        <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                        <HeaderStyle CssClass="GridviewScrollHeader_line_boot" BorderStyle="None" />
                                        <RowStyle CssClass="" />
                                        <PagerStyle CssClass="pagination-ys" />
                                        <Columns>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <asp:Panel ID="Panel_che_box_aling" runat="server" Style="text-align: center">
                                                    </asp:Panel>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="chkSelection" runat="server" CssClass="btn btn-light btn-sm border-0 bg-transparent" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </asp:Panel>

                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
                 <div style="height: auto" id="modal-footer_boton_lista_chequeo_actualiza" class="modal-footer justify-content-end">
                        <asp:UpdatePanel ID="UpdatePanel_lista_chequeo_estado_actualiza" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <asp:Label ID="Label_estado_lista_chequeo_actualiza" runat="server" Text="Estado" Style="font-size: 12px; display: none"></asp:Label>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <asp:UpdatePanel ID="UpdatePanel_lista_chequeo_actualiza" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <asp:Button ID="Button_Actualizar_item_lista" runat="server" Text="Actualizar" Style="float: right; margin-top: 5px; margin-right: 5px; display: none" CssClass="boton_azul" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <button id="buton_validate_chek" type="button" class="btn btn-success">Validar</button>
                    </div>
                <div style="display: none; height: 1px">
                    <asp:Button ID="Button_lista_chequeo_actualiza" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                    <asp:Button ID="ButtonSalir_lista_chequeo_actualiza" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                    <asp:Button ID="Button_cerrar_lista_chequeo_actualiza" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                </div>
            </asp:Panel>
          
         <!--validacion externo-->
          <div id="validacion_plantilla">
            <asp:Panel ID="Panel_valiacion_plantilla" runat="server"  Style="display:none; width: 100%; height: 100%" CssClass="modal_content_general">
                 <asp:ModalPopupExtender ID="ModalPopupExtender_valiacion_plantilla" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_valiacion_plantilla"
                    PopupControlID="Panel_valiacion_plantilla" CancelControlID="Button_cerrar_validacion_plantilla">
                </asp:ModalPopupExtender>
                <div id="divcabecer2_validacion_plantilla" class="modal_title_superior_ modal-header"> 
                     <h6 class="modal-title d-inline ml-1">Gestión externos</h6>
                     <button type="button" value="Button_cerrar_validacion_plantilla" class="close da_event_captive">&times;</button>                     
                </div>
                <asp:UpdatePanel ID="UpdatePanel_validacion_plantilla" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div id="Contenido_validacion_plantilla" style=" height: 90%; width: 100%" class="modal_content_back">
                            <iframe width="100%" height="90%" id="Iframe_validacion_plantilla_" runat="server"  frameborder="0" ></iframe>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                    <div style="display:none; height:1px">
                         <asp:Button ID="ButtonSalir_valiacion_plantilla" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" style="display:none"/>
                         <asp:Button ID="Button_cerrar_validacion_plantilla" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" style="display:none"/>
                    </div>
                   
             </asp:Panel>
        </div>
        <!--popup traza_graficas-->       
          <div style="clear: both">
              <input id="Hidden_id_flu" type="hidden" value="0" runat="server"/> 
              <input id="Hidden_nom_flu" type="hidden" value="" runat="server"/>
              <asp:Panel ID="Paneltraza_grafica" runat="server" Style="display:none; color: White; width: 100%; height: auto" CssClass="modal_content_general">
                  <asp:ModalPopupExtender ID="ModalPopupExtendertraza_grafica" runat="Server"  Y="1" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonD_trace_grafic"
                      PopupControlID="Paneltraza_grafica" CancelControlID="Buttoncabcel_trace_grafic">
                  </asp:ModalPopupExtender>
                  <div id="div_trace_grafic" class="modal_title_superior">                             
                         <button type="button" value="Buttoncabcel_trace_grafic" class="close da_event_captive mr-2">&times;</button>
                  </div>
                  <div id="div_content_trace_grafic" style="color: White; background-color: #FFFFFF; height: auto; width: 100%" class="modal_content_back">
                      <asp:UpdatePanel ID="UpdatePaneltraza_grafica" runat="server" UpdateMode="Conditional">
                          <ContentTemplate>
                              <iframe id="Iframetraza_grafica_" runat="server" frameborder="0"  scrolling="no" style="width:100%" ></iframe>
                          </ContentTemplate>
                      </asp:UpdatePanel>
                  </div>
                  <div style="display:none; height:1px">
                      <asp:Button ID="ButtonD_trace_grafic" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" style="display:none" />
                        <asp:Button ID="Buttoncabcel_trace_grafic" runat="Server" Text="X"   CssClass="invisible"
                              Height="1px" Width="1px" style="display:none" />
                  </div>
                  
              </asp:Panel>
          </div>
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
                                            <asp:Label ID="Label1" runat="server" Text="RADICADO DEL TRAMITE"></asp:Label>
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
                                            <asp:Label ID="Label3" runat="server" Text="SEDE USUARIO RADICADOR"></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:Label ID="Label_SEDE_USUARIO" runat="server" Text=""></asp:Label>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                            <asp:Label ID="Label5" runat="server" Text="USUARIO ASIGNADO"></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:Label ID="LabelASIGNADO" runat="server" Text=""></asp:Label>
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
        <!--nota_flujo-->  
           <asp:Panel ID="Panel_content_anotacion" runat="server" Style="display: none; width: 90%; height: 100%" CssClass="modal_content_general_">
                  <asp:ModalPopupExtender ID="ModalPopupExtender_edition_content_anotacion" runat="server"
                      TargetControlID="ButtonSalir_content_anotacion_" BackgroundCssClass="FondoAplicacion"
                      CancelControlID="Button_cerrar_content_anotacion" PopupControlID="Panel_content_anotacion">
                  </asp:ModalPopupExtender>
                  <div id="modal_content_anotacion" class="modal-content">
                      <div id="diver_cabcera_content_anotacion" class="modal_title_superior_ modal-header">
                          <asp:Label ID="LabelTitulo" runat="server" Text="Anotaciones de la Tarea" CssClass="h6 font-weight-light"> </asp:Label>
                          <button type="button" value="Button_cerrar_content_anotacion" class="close da_event_captive ">&times;</button>
                      </div>
                      <div id="contenido_procesa_content_anotacion" style="height: auto; width: 100%; overflow: auto; border-top:none" class="pl-3 pr-3">
                          <asp:UpdatePanel ID="UpdatePanelanotacion" runat="server" UpdateMode="Conditional">
                              <ContentTemplate>
                                  <asp:Panel ID="Panel_content_anotacion_gred" runat="server"
                                      Enabled="true" Style="margin-bottom: 5px">
                                      <asp:GridView ID="GridView_lista_notas" runat="server" Style="width: 100%"
                                          AutoGenerateSelectButton="False" CssClass="filtrar table  font-weight-light" GridLines="None">
                                          <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                          <HeaderStyle CssClass="GridviewScrollHeader_line_boot" />
                                          <Columns>
                                              <asp:BoundField HeaderText="OPCIONES" />
                                          </Columns>
                                      </asp:GridView>
                                  </asp:Panel>
                                  <input id="hdnidlista" type="hidden" value="-1" runat="server"/>
                              </ContentTemplate>
                              <Triggers>
                              </Triggers>
                          </asp:UpdatePanel>
                      </div>
                       <div id="content_boton" class="modal-footer justify-content-end">
                           <input id="Button_Show_Guardar" type="button" class="btn btn-success" value="Nueva nota"  />        
                  </div>
                  </div>
                 
                  <div style="display: none; height: 1px">
                      <asp:Button ID="Button_content_anotacion" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                      <asp:Button ID="ButtonSalir_content_anotacion_" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                      <asp:Button ID="Button_cerrar_content_anotacion" runat="Server" Text="X" CssClass="invisible" />
                  </div>
              </asp:Panel>
                <div id="nota_respuesta">
                  <asp:Panel ID="Panel_nota_respuesta" runat="server" Style="display: none; width: 66%; height: auto" CssClass="modal_content_general_">
                      <asp:ModalPopupExtender ID="ModalPopupExtender_edition_nota_respuesta" runat="server" TargetControlID="ButtonSalir_nota_respuesta" BackgroundCssClass="FondoAplicacion"
                          CancelControlID="Button_cerrar_nota_respuesta" PopupControlID="Panel_nota_respuesta">
                      </asp:ModalPopupExtender>
                      <div id="modal_content_nota_respuesta" class="modal-content">
                          <div id="divcabecer_nota_respuesta" class="modal_title_superior_ modal-header">
                              <asp:Label ID="Label_nota_respuesta" class="modal-title d-inline ml-1 h6" runat="server" Text="Nota">
                              </asp:Label>
                              <button type="button" value="Button_cerrar_nota_respuesta" class="close da_event_captive">&times;</button>
                          </div>
                         
                          <div id="contenido_procesa_nota_respuesta" style="border-top: none; overflow: hidden" class="p-1">    
                               <textarea id="TextBox_nota" rows="10" style="width: 100%; height: 100%" cols="50">Write something here</textarea>
                          </div>
                          <div id="content_boton_nota" class="modal-footer">
                              <input id="Button_actualizar_nota" type="button" class="btn btn-success" value="Aceptar" style="display:none" />
                              <input id="Button_duardar_nota" type="button" class="btn btn-success" value="Aceptar" style="display:none"/>
                          </div>
                          <div style="display: none; height: 1px">
                              <asp:Button ID="Button_nota_respuesta" CssClass="invisible" runat="server" Text="Button" Height="0px" Width="0px" Style="display: none" />
                              <asp:Button ID="ButtonSalir_nota_respuesta" CssClass="invisible" runat="server" Text="Button" Height="0px" Width="0px" Style="display: none" />
                              <asp:Button ID="Button_cerrar_nota_respuesta" runat="Server" Text="X" CssClass="modal_boton_hiden" />
                          </div>

                      </div>
                  </asp:Panel>
              </div>  
              <div style="display:none">   
                  <asp:UpdatePanel ID="UpdatePanel_tool_menu" runat="server" UpdateMode="Conditional">
                      <ContentTemplate>                     
                           <asp:ImageButton ID="ImageButtonanotacion" runat="server"   Width="0px" Height="0px" style="margin-left:0px; display:none" />       
                           <input id="Hidden_estado_anotacion" type="hidden" value="NO" runat="server" />
                         
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
                     

                 });
             });
            
</script>
</body>

</html>
