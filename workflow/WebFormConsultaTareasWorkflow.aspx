<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormConsultaTareasWorkflow.aspx.vb" enableEventValidation="false" Inherits="GestionDocumental_Docuarchi.net.WebFormConsultaTareasWorkflow" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<title></title>
 <script src="../js/ui/jquery-3.4.1.min.js" type="text/javascript"></script>  
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.11.0/umd/popper.min.js" type="text/javascript"></script>
    <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js" type="text/javascript"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <link href="../bootstrap/table/dist/bootstrap-table.min.css" rel="stylesheet" />
    <script src="../bootstrap/table/dist/bootstrap-table.min.js" type="text/javascript"></script>
    <script src="../bootstrap/table/dist/bootstrap-table-locale-all.js" type="text/javascript"></script>   
     <script src="../bootstrap/table/dist/extensions/export/bootstrap-table-export.min.js" type="text/javascript"></script>
    <script src="../bootstrap/table/dist/extensions/export/bootstrap-table-export.js" type="text/javascript"></script>     
    <script src="../js/table_boo/table_boot_config.js" type="text/javascript"></script>
    <script src="https://unpkg.com/tableexport.jquery.plugin/tableExport.min.js" type="text/javascript"></script>
   <script src="../js/Filtrar.js" type="text/javascript"></script>
    <script src="../js/validate_campos.js" type="text/javascript"></script>
    <script src="../js/workflow/WebFormConsultaTareasWorkflow.js" type="text/javascript"></script>
    <script src="../js/java_general/general_config.js" type="text/javascript"></script>
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
     <script src="../js/java_general/general_code_java.js" type="text/javascript"></script>
     <script  src="../Awesome/js/all.js" type="text/javascript"></script>
    <script  src="../Awesome/js/brands.js" type="text/javascript"></script>
    <script  src="../Awesome/js/solid.js" type="text/javascript"></script>
    <script  src="../Awesome/js/fontawesome.js" type="text/javascript"></script> 
</head>
<body style="background-color:white">
    <form id="form1" runat="server" style="background-color:white">
    
        <asp:ScriptManager ID="ScriptManager1" runat="server"
            EnableScriptGlobalization="True" EnablePageMethods="True" AsyncPostBackTimeout="900">
        </asp:ScriptManager> 
        <script accesskey="javascript" type="text/javascript">
            Sys.WebForms.PageRequestManager.getInstance().add_initializeRequest(InitializeRequest);
            Sys.Application.add_load(ApplicationLoadHandler)
            var elment_postbak;
            var value_element;
            function ApplicationLoadHandler(sender, args) {

                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(CheckStatus);

            }
            function InitializeRequest(sender, args) {
                //
                elment_postbak = args.get_postBackElement();
                var elmen = document.getElementById(elment_postbak.id)
                if (elmen.type == "button" || elmen.type == "submit") {
                    value_element = elmen.value;
                    elmen.value = "Espere..."
                    elmen.disabled = true;
                }
                posicion_update_pogres('progres_bar');
            }
            function CheckStatus(sender, args) {
                try {
                      
                    if (elment_postbak.type == "button" || elment_postbak.type == "submit") {
                        elment_postbak.value = value_element;
                        elment_postbak.disabled = false;
                    }
                    if (elment_postbak.id == "Button_Trazabilidad") {
                        auto_zise_popup_detalle_trazabilidad();
                    }
                    if (elment_postbak.id == "Button_Trazabilidad") {
                        auto_zise_popup_detalle_trazabilidad();
                    }
                    if (elment_postbak.id == "Button_trazabilidad_grafica") {
                        auto_zise_popup_paginas_externas_libres();
                    }
                    if (elment_postbak.id == "ImageButton_ista_autorizacio") {
                        auto_zise_popup_paginas_externas_libres();
                    }
                    if (elment_postbak.id == "Button_visor_emergente") {
                        auto_zise_popup_imagen_respuesta();
                    }
                    if (elment_postbak.id == "Button_tool_activa_detalle_radicado") {
                        auto_zise_popup_detalle_radicado();
                    }
                    if (elment_postbak.id == "Button_consulta") {
                        document.getElementById("hdnEmailID").value = "-1"
                        document.getElementById("Hidden_id_tarea_sel").value = "-1"
                        auto_zise_consulta_tarea(34);
                    }
                   
                }
                catch (ex) {
                    alert("Funcion asincrona error " + ex.message);
                }
                finally {
                    progres_hiden('progres_bar');
                }
            }

            </script>
            <input id="Hidden_id_tarea_sel" type="hidden" value="-1" runat="server"/>
            <input id="Hidden_tipo_visor" type="hidden" value="" runat="server"/>
        <div id="conte_waper" class="container-fluid mr-0 ml-0 pl-0 pr-0" style="border-top: 1px solid #e9ecef">
            <nav id="menu_var" class="navbar navbar-expand-sm nav_botota_person_gray modal_content_no_back_inferior">
                <button id="nav_togle_display" class="navbar-toggler" type="button" style="background-color: #6d7fcc" data-toggle="collapse" data-target="#navbarNavDropdown">
                    <span class="navbar-toggler-icon_"><i style="color: white" class="fad fa-th-list"></i></span>
                </button>
                <div class="collapse navbar-collapse row" id="navbarNavDropdown">
                    <ul class="navbar-nav col-md-12">
                        <li class="nav-item dropdown active ml-2 mr-0 active_">
                            <a class="nav-link  dropdown-toggle " style="color: #6d7fcc" href="#" id="A5" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc; display: none" class="fad fa-th-list"></i>Menú 
                            </a>
                            <div class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">
                                <a style="color: #6d7fcc" href="#" class="dropdown-item font-weight-light" onclick="activa_menu('e_r_r_002')"><i class="fal fa-file-export"></i> Exportar los resultados</a>
                            </div>
                        </li>
                        <li class="nav-item dropdown active ml-2 mr-0 active_">
                            <a class="nav-link  dropdown-toggle " style="color: #6d7fcc" href="#" id="A1" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc; display: none" class="fad fa-th-list"></i> Detalle 
                            </a>
                            <div class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">
                                <a style="color: #6d7fcc" href="#" class="dropdown-item font-weight-light" onclick="activa_menu('e_c_b_004')"><i class="fal fa-list-ol"></i> Lista trazabilidad de la tarea seleccionada</a>
                                <a style="color: #6d7fcc" href="#" class="dropdown-item font-weight-light" onclick="activa_menu('e_c_d_005')"><i class="fal fa-project-diagram"></i> Lista trazabilidad en modo grafico a la tarea seleccioanda</a>
                                <a style="color: #6d7fcc" href="#" class="dropdown-item font-weight-light" onclick="activa_menu('e_c_d_006')"><i class="fal fa-table"></i> Lista autorizaciones de la tarea seleccionada</a>
                                <a id ="a_list_operation_document" style="color: #6d7fcc" href="#" class="dropdown-item font-weight-light"><i class="far fa-info-square"></i> Lista detalle operaciones de documentos</a>
                                <a id ="a_list_notes_task" style="color: #6d7fcc" href="#"   class="dropdown-item font-weight-light" ><i class="fal fa-table"></i>  Lista notas tarea</a>
                                <a id ="a_list_copy_document_expedient" style="color: #6d7fcc" href="#"      class="dropdown-item font-weight-light" ><i class="fal fa-table"></i>  Detalle documentos copiados a expediente</a>
                            </div>
                        </li>
                         <li class="nav-item dropdown active ml-2 mr-0 active_">
                            <a class="nav-link  dropdown-toggle " style="color: #6d7fcc" href="#" id="A6" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc; display: none" class="fad fa-th-list"></i> Opciones 
                            </a>
                            <div class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">
                                <a style="color: #6d7fcc" href="#" class="dropdown-item font-weight-light" onclick="activa_menu('A-REA-SII')"><i class="fal fa-file-export"></i> Reasignar tarea</a>
                            </div>
                        </li>
                    </ul>
                </div>
                <input id="Button_activa_bus" type="submit" value="Buscar" style="background: white; color: black; float: right; margin: 5px 6px 1px 2px; display: none" class="boton_azul" onclick="acti_busq_general_archivo_boton(event, this);" />
                <asp:TextBox ID="TextBox_buequeda_general" runat="server" Style="float: right; padding: 2px 2px; margin: 6px 4px 4px 4px; display: none" placeholder="Busqueda general de tareas.."
                    onkeypress="acti_busq_general_archivo(event,this)"></asp:TextBox>
                <a href="#home" title="Regresar a la contenido de la carpeta seleccionada" onclick="activa_menu('r_b_s_011')" style="float: right; font-size: 16px; font-weight: 900; display: none">&#x21e6;</a>
            </nav>
          <div id="da_content_wraper" class="wrapper ml-0 mr-0 " style="padding-left: 1px; padding-right: 1px;">       
            <a id="da_show-sidebar_" class="btn btn-sm   hide_da_sidebar " href="#" data-target="#sidebar">
                <i style="color: white" class="fas fa-bars"></i>
            </a>
            <nav id="contenido_general" class=" pl-0 pr-0" style="width: 40%; float: left; position: inherit; border-right: 1px solid #F2F2F2">
                <div id="contenido_titulo_campos" class="modal-header" style="border-top-left-radius: initial; border-top-right-radius: initial">
                    <h6 style="color: #6d7fcc" class="modal-title">Campos de busqueda</h6>
                    <button id="sidebarCollapse" style="color: #6d7fcc" type="button" class="close">&times;</button>
                </div>
                <div id="Content_consulta" class="modal-body  pl-0 pr-0 pt-0 pb-0 ">
                    <div id="opciones_busqueda" style="text-align: center; margin-top: 1px; margin-left: 1px; margin-right: 1px">
                        <asp:MutuallyExclusiveCheckBoxExtender ID="MutuallyExclusive_anexo_radicado" runat="server" TargetControlID="CheckBox_and"
                            Key="radicado_un"></asp:MutuallyExclusiveCheckBoxExtender>
                        <asp:MutuallyExclusiveCheckBoxExtender ID="MutuallyExclusive_relacionado_radicado" runat="server" TargetControlID="CheckBox_on"
                            Key="radicado_un"></asp:MutuallyExclusiveCheckBoxExtender>
                        <asp:CheckBox ID="CheckBox_and" runat="server" Text="(Y)" Checked="true" ForeColor="Black" Font-Size="10" Font-Names="Arial" Style="margin-left: 5px; display: block" Enabled="true" />
                        <asp:CheckBox ID="CheckBox_on" runat="server" Text="(O)" Checked="false" ForeColor="Black" Font-Size="10" Font-Names="Arial" Style="margin-left: 5px" Enabled="true" />
                    </div>
                    <div id="separator_div" style="width: 100%">
                        <asp:Panel ID="Panel1" runat="server"
                            Style="overflow: auto; width: 99%;color: #6d7fcc">
                            <asp:Table ID="TableControles" runat="server" Style="width: 99%" CssClass="table table-borderless">
                            </asp:Table>
                        </asp:Panel>
                    </div>
                    <div id="contenido_consulta" style="width: 100%; background: white" class="modal-footer  justify-content-end">
                        <asp:UpdatePanel ID="Updatepanel_botones_consulta" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Button ID="Button_consulta" runat="server" Text="Consultar" Style="margin-top: 5px; margin-right: 5px; float: right" CssClass="btn btn-success" />
                                <asp:Button ID="Button_limpiar_campo" runat="server" Text="Restaurar" ToolTip="Restaurar campos de consulta" Style="margin-top: 5px; margin-left: 5px; display: none" CssClass="boton_azul" />
                                <asp:Button ID="Button_activa_busqueda_general" runat="server" Text="Consultar general" Style="margin-top: 5px; margin-left: 5px; display: none" CssClass="boton_azul" />
                                <asp:Button ID="Button_trazabilidad_grafica" runat="server" Text="Consultar general" Style="margin-top: 5px; margin-left: 5px; display: none" CssClass="boton_azul" />
                                <asp:Button ID="Button_visor_emergente" runat="server" Text="Button" Style="display: none" />
                                <asp:Button ID="Button_lista_autoriza" runat="server" Text="Button" Style="display: none" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>

            </nav>

            <div id="content_" class="page-content mr-0 ml-0 pl-1 pr-1 pb-0 pt-0 " style="width: 100%; overflow: hidden">
                <div id="contenido_titulo_resultado" class="modal-header justify-content-between" style="border-top-left-radius: initial; border-top-right-radius: initial" >
                    <asp:UpdatePanel ID="UpdatePanel_labelresultado" runat="server" UpdateMode="Conditional" style="width:100%" >
                        <ContentTemplate>
                            <div class="row " style="width:100%">
                                <div class="col-9 justify-content-start">
                                    <asp:Label ID="Label_resultado" runat="server" Text="Resultado Busqueda" Style="color: #6d7fcc" CssClass="h6"></asp:Label>
                                </div>
                                <div class="col-3 justify-content-end" >
                                    <asp:DropDownList ID="DropDownList_limite_rows"  CssClass="form-select mr-1" style="float:right" runat="server" ></asp:DropDownList>
                                </div>
                            </div> 
                               
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div id="Contenedorgrid" style="width: 100%; height: 340px">
                    <asp:UpdatePanel ID="UpdatePanel_hiden" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <input id="hdnEmailID" type="hidden" value="0" runat="server"/>
                        </ContentTemplate>
                    </asp:UpdatePanel>

                    <asp:UpdatePanel ID="UpdateGeneral" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Panel ID="Panelactividad" runat="server" Wrap="False" ScrollBars="Auto"
                                Width="100%" Height="98%">
                                <asp:GridView ID="GridViewlista" runat="server"  EnableViewState="true" GridLines="None"
                                    AutoGenerateSelectButton="False" CssClass="filtrar table font-weight-light">
                                    <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                    <HeaderStyle CssClass="GridviewScrollHeader_line_boot" />
                                     <Columns>
                                         <asp:BoundField HeaderText="OPCIONES"   />
                                    </Columns>

                                </asp:GridView>

                            </asp:Panel>
                        </ContentTemplate>
                        <Triggers>
                        </Triggers>
                    </asp:UpdatePanel>

                </div>
                <div id="contenido_botonoes" style="width: 69%; position: inherit; left: auto; float: right; height: 30px; background: white; display: none">
                    <asp:UpdatePanel ID="Updatepanel_botones" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Button ID="Button_buscar" runat="server" Text="Buscar en lista " Style="margin-top: 5px; margin-left: 5px" CssClass="boton_azul" />
                            <asp:Button ID="Button_Trazabilidad" runat="server" Text="Estados del radicado" ToolTip="Estados del radicado" Style="" CssClass="boton_azul" />
                            <asp:Button ID="Button_Exportar_Radicados" Text="Exportar" runat="server" ToolTip="Exportar lista" OnClientClick="retorna_colum_mtriz('Hidden_colum_header');"
                                Style="margin-left: 5px" CssClass="boton_azul" />
                            <input id="Hidden_colum_header" type="hidden" value="" runat="server"/>
                            <input id="Hidden_ruta_archivo" type="hidden" value="" runat="server"/>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
         </div>
        </div>
         <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 50px">
            <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
            Processing ...
        </div>   
       
        <input id="HiddenFiltro" type="hidden" value="" runat="server"/>
       <!--codigo cuadro de dialogo-->
        <div id="framemensaje">
            <input id="hdnConsult" type="hidden" value="0" runat="server"/>
            <asp:UpdatePanel ID="UpdatePaneMensaje" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Panel ID="Panelmensaj" runat="server" Style="display:none" ForeColor="White" Width="250px" Height="160px" HorizontalAlign="Center">
                        <asp:ModalPopupExtender ID="ModalPopupTexto" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button4"
                            PopupControlID="Panelmensaj" CancelControlID="btnCancel">
                        </asp:ModalPopupExtender>
                        <div id="Div2" class="cabecera3">

                            <asp:Label ID="Label2" runat="server" Text="Advertencia" Font-Size="10">
                            </asp:Label>
                        </div>

                        <div id="container" style="border: thin double #000080; color: White; background-color: #FFFFFF; height: 87%; width: 247px">
                            <div id="Contenido" style="height: 60%">
                                <br />
                                <label id="Lableme" style="font-size: 14px; color: #000000" title="" />
                                <asp:Label ID="LabelMensaje" runat="server" Text="Posibles Datos" ForeColor="Black" Font-Size="9" Visible="True" />

                                <asp:DropDownList ID="Droupdatos" runat="server" Width="200px">
                                </asp:DropDownList>

                            </div>
                            <div id="Contenidbuton" style="height: 29%; color: White; background-color: #FFFFFF;">


                                <asp:Button ID="btnOkay" runat="server" Text="Aceptar " CssClass="boton" />
                                <asp:Button ID="btnCancel" runat="server" Text="Cancelar" CssClass="boton"  />
                                <br />
                                <asp:Button ID="Button4" CssClass="invisible" runat="server" Text="Button" Height="0px" Width="0px" />
                                <asp:Button ID="Button5" CssClass="invisible" runat="server" Text="Button" Height="0px" Width="0px" />

                            </div>
                            <div id="Div3" style="height: 10%; color: White; background-color:black">
                            </div>
                        </div>

                    </asp:Panel>

                </ContentTemplate>

            </asp:UpdatePanel>
        </div>    
         
        <!--Popub busqueda !-->
          <div id="botonbusqueda" style="display: none">
              <asp:Button ID="ButtonActivarBusqueda" runat="server" Text="Button" />
          </div>
          <div id="busquda">
              <asp:Panel ID="Panelbusqueda" runat="server" Style="display:none; color: White; width: auto; height: auto">        
                  <asp:ModalPopupExtender ID="ModalPopupExtenderbusqueda" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Buttond_busqueda"
                      PopupControlID="Panelbusqueda" CancelControlID="Buttoncacerrar">
                  </asp:ModalPopupExtender>
                  <div id="divcabecer" class="cabecera2">
                      <asp:Button ID="Buttond_busqueda" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                      <asp:Label ID="Label4" runat="server" Text="Busqueda" Font-Size="10" Style="float: left">
                      </asp:Label>
                      <div id="Divcerrarbuton" style="float: right">
                          <asp:Button ID="Buttoncacerrar" runat="Server" Text="X"
                              ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />
                      </div>
                  </div>

                  <div id="Diupdate" class="fond_contextual" style="color: White;  height: auto; width: auto">
                      <div id="Contenidopaginabusqueda"  style="height: 140px; width: 450px; overflow: no-display; color: black; margin-left: 15px">
                          <asp:UpdatePanel ID="UpadatePanel_busqueda" runat="server" UpdateMode="Conditional">
                              <ContentTemplate>
                                  <br />
                                  <label style="font-family: Arial; font-size: 12px">Busqueda Tareas en la lista </label>
                                  <br />
                                  <label style="font-family: Arial; font-size: 12px">Digita Texto </label>
                                  <asp:TextBox ID="contenidobusqueda" runat="server"></asp:TextBox>
                                  <asp:Button ID="Buttonbuscar" runat="server" Text="Buscar" CssClass="boton_azul" OnClientClick="activa_busqueda();" />
                                  <br />
                                  <br />
                                  <br />
                                  <asp:CheckBox ID="checkbox" runat="server" />
                                  <!-- <input id="CheckboxBusqueda" type="checkbox" title="Palabra completa"  />!-->
                                  <label style="font-family: Arial; font-size: 10px">Buscar sólo palabra completa</label>
                                  <br />
                              </ContentTemplate>
                          </asp:UpdatePanel>
                      </div>
             
                    
                     
                  </div>
                  
              </asp:Panel>
          </div>
         <!--Popup visor externo-->
	     <asp:Panel ID="Panel_visor_externo" runat="server" Style="display:none; overflow:hidden" ForeColor="White" Width="95%" Height="100% " >
	                     <asp:ModalPopupExtender ID="ModalPopupExtender_visor_externo" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_visor_externo"
	                         PopupControlID="Panel_visor_externo" Y="1" CancelControlID="ButtonSalir_visor_externo">
	                     </asp:ModalPopupExtender>
	                     <div id="Cabecerapendiente_visor_externo" class="cabecera2">
	                         <asp:Button ID="Button_visor_externo" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
	                         <asp:Label ID="Label9" runat="server" Text="Visor documentos Workflow" Font-Size="10"></asp:Label>
	                         <div id="Div_visor_externo" style="float: right">
	                             <asp:Button ID="ButtonSalir_visor_externo" runat="Server" Text="X"
	                                 ForeColor="#000066" Height="21px" />
	   
	                         </div>
	                     </div>
	                     <div id="Cotenedorpendiente_visor_externo" style="border: thin double #000080; color: Black; background-color: #FFFFFF; height: 90%; width: 100%; overflow:hidden">
	                     
	                         <asp:UpdatePanel ID="UpdatePanel_visor_externo" runat="server" UpdateMode="Conditional">
	                             <ContentTemplate>
	                                 <iframe id="Iframe_visor_externo_" runat="server" frameborder="0"  style="width:100%; height:100%; overflow:hidden"></iframe>
	                             </ContentTemplate>
	   
	                         </asp:UpdatePanel>
	                              
	                     </div>
	                     
              </asp:Panel>
         <div id="Filtro">
              <asp:Panel ID="Panel_filtro" runat="server" Style="display: none; color: White; width: auto; height: auto">
                 
                  <asp:ModalPopupExtender ID="ModalPopupExtender_Filtro" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Buttond_Filtro"
                      PopupControlID="Panel_filtro" CancelControlID="Button_Filtro_Cerrar">
                  </asp:ModalPopupExtender>
                  <div id="divcabecer_filtro" class="cabecera2">
                      <asp:Button ID="Buttond_Filtro" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                      <asp:Label ID="Label1" runat="server" Text="Filtrar Lista" Font-Size="10" Style="float: left">
                      </asp:Label>
                      <div id="Divcerrarbuton_filtro" style="float: right">
                          <asp:Button ID="Button_Filtro_Cerrar" runat="Server" Text="X"
                              ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />
                      </div>
                  </div>

                  <div id="Diupdate_filtro" style="border: thin double #000080; color: White; background-color: #FFFFFF; height: auto; width: auto">
                      <div id="Contenidopagina_filtro" style="height: 140px; width: 450px; overflow: no-display; color: black; margin-left: 15px">
                          <asp:UpdatePanel ID="Updatepanel_filtro" runat="server" UpdateMode="Conditional">
                              <ContentTemplate>
                                  <br />
                                  <label style="font-family: Arial; font-size: 12px">Busqueda Tareas a filtar </label>
                                  <br />
                                  <label style="font-family: Arial; font-size: 12px">Digita Texto </label>
                                  <asp:TextBox ID="contenidobusqueda_filtro" runat="server"></asp:TextBox>
                          
                                  <asp:Button ID="ButtonFiltro" type="button" Text="Aceptar" runat="server" class="boton" />
                                  <asp:CheckBox ID="CheckBox_filtro" runat="server" Text="Sólo palabra completa" Font-Size="10" Font-Names="arial" />
                                  <br />

                              </ContentTemplate>
                          </asp:UpdatePanel>
                      </div>
                     
                  </div>
                   <div id="border_filtro" style=" color: white; font-size: small; background-color: #053061; width: 470px; height:10px">
                         
                      </div>
                 
              </asp:Panel>
          </div>
         <!--Mensaje popup para paginas externas libres-->
          <div style="clear: both">
              <asp:Panel ID="PanelLibre" runat="server" Style="display: none; color: black; width: 99%; height: auto" CssClass="modal_content_general_">
                  <asp:ModalPopupExtender ID="ModalPopupExtenderLibre" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonD"
                      PopupControlID="PanelLibre" CancelControlID="Buttoncabcel">
                  </asp:ModalPopupExtender>
                  <div id="modal_content_PanelLibre" class="modal-content">
                      <div id="Div_cabecera_PanelLibre" class="modal_title_superior_ modal-header">
                          <h6 class="modal-title d-inline ml-1">Trazabilidad grafica</h6>
                          <button type="button" value="Buttoncabcel" class="close da_event_captive">&times;</button>
                      </div>
                      <div id="Div_contenedor_PanelLibre" style="height: auto; width: 100%" class="modal_content_general">
                          <asp:UpdatePanel ID="UpdatePanelLibre" runat="server" UpdateMode="Conditional">
                              <ContentTemplate>
                                  <iframe id="Iframelibre_" runat="server" frameborder="0" scrolling="no" style="width: 100%"></iframe>
                              </ContentTemplate>
                          </asp:UpdatePanel>
                      </div>
                  </div>
              </asp:Panel>
              <div style="display:none; height:0px">
                   <asp:Button ID="ButtonD" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                   <asp:Button ID="Buttoncabcel" runat="Server" Text="" CssClass="modal_boton_hiden" />
              </div>       
          </div>
        <!--detalle trazabilidad-->
           <asp:Panel ID="Panel_trazabilidad" runat="server" Style="display:none; overflow:hidden; width:810px; height:100%"  CssClass="modal_content_general_" >
                  <asp:ModalPopupExtender ID="ModalPopupExtender_trazabilidad" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_trazabilidad_dos"
                      PopupControlID="Panel_trazabilidad"  CancelControlID="ButtonSalir_trazabilidad">
                  </asp:ModalPopupExtender>
                  <div id="modal_content_Panel_trazabilidad" class="modal-content">  
                  <div id="Cabecerapendiente_trazabilidad" class="modal_title_superior_ modal-header">  
                        <h6 class="modal-title d-inline ml-1">Ventana de trazabilidad</h6>
                        <button type="button" value="ButtonSalir_trazabilidad" class="close da_event_captive">&times;</button>                      
                  </div>
                  <div id="Cotenedorpendiente_trazabilidad" style="height: 90%; width: 100%; border-top:none; overflow:auto" class="modal_content_back">          
                      <asp:UpdatePanel ID="UpdatePanel_trazabilidad" runat="server" UpdateMode="Conditional">
                          <ContentTemplate>
                              <iframe id="Iframe_trazabilidad_" runat="server" frameborder="0"  style="width:100%; height:100%; overflow:hidden"></iframe>
                          </ContentTemplate>
                      </asp:UpdatePanel>            
                  </div>
                  </div>
                   <div style="display:none; height:0px">
                       <asp:Button ID="Button_trazabilidad_dos" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="ButtonSalir_trazabilidad" runat="Server" Text="" CssClass="modal_boton_hiden"/>
                   </div>  
              </asp:Panel>
           <div id="lista_autorizacion">
            <asp:Panel ID="Panel_lista_autorizacion" runat="server" Style="display:none; color: White; width:90%; height:auto" CssClass="modal_content_general_" >
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_lista_autorizacion" runat="server" BehaviorID="Panel_lista_autorizacion_ModalPopupExtender" TargetControlID="ButtonSalir_lista_autorizacion" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_lista_autorizacion" PopupControlID="Panel_lista_autorizacion" ></asp:ModalPopupExtender>
                <div id="modal_content_lista_autorizacion" class="modal-content">
                    <div id="divcabecer_lista_autorizacion" class="modal_title_superior_ modal-header">
                         <h6 class="modal-title d-inline ml-1">Autorizantes</h6>
                         <button type="button" value="Button_cerrar_lista_autorizacion" class="close da_event_captive">&times;</button>                        
                    </div>
                    <div id="contenido_procesa_lista_autorizacion" style="background-color: white; width: 100%; height: 99%" class="modal_content_back">
                       
                        <asp:UpdatePanel ID="UpdatePanel_contenido_lista_autorizacion" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div id="div_label_lista_autorizacion" class="p-2">
                                    <asp:Label ID="Label_title_listado_autorizaciones" runat="server" Text="" CssClass=" h6"></asp:Label>
                                </div>
                                <asp:Panel ID="Panel_lista_autorizacion_2" runat="server" ScrollBars="Auto"
                                    Width="100%" Style="min-height: 250px">
                                    <asp:GridView ID="data_grid_listado_solicitudes" runat="server" AllowSorting="true" AllowPaging="true" EnableViewState="true"
                                        PageSize="7" PagerSettings-Position="Top"  Style="width: 100%"
                                        AutoGenerateSelectButton="False" CssClass="filtrar table font-weight-light" GridLines="None" >
                                        <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                        <HeaderStyle CssClass="GridviewScrollHeader_line_boot" BorderStyle="None" />
                                        <PagerStyle CssClass="pagination-ys" />
                                        <Columns>
                                            <asp:BoundField HeaderText="OPCIONES   " />
                                        </Columns>

                                    </asp:GridView>
                                </asp:Panel>

                            </ContentTemplate>
                        </asp:UpdatePanel>
                        

                    </div>
                     <div class="modal-footer justify-content-end" id="conter_boton_footer_lista_autorizacion">
                          <asp:UpdatePanel ID="updatemenu_lista_autorizacion" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Button ID="Button_descarga_consolidado_aprobacion" CssClass="btn btn-success" runat="server" ToolTip="Descarga consolidado aprobaciones" Text="Descargar" />
                                   
                                </ContentTemplate>
                            </asp:UpdatePanel>
                    </div>
                    <div style="display:none; height:1px">
                        <asp:Button ID="Button_lista_autorizacion" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="ButtonSalir_lista_autorizacion" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="Button_cerrar_lista_autorizacion" runat="Server" Text="" CssClass="invisible" />
                    </div>
                </div>
            </asp:Panel>
              <asp:UpdatePanel ID="UpdatePanel_boton_lista" runat="server" UpdateMode="Conditional">
                  <ContentTemplate>
                      <input id="Hidden_selec_list" type="hidden" value="-1" runat="server">
                      <asp:Button ID="Button_dowload_xml" runat="server" Text="Button" Style="display: none" />

                  </ContentTemplate>

              </asp:UpdatePanel>
                    <div id="descaga_xml_autoriza" style="width: 0%; height: 0%; background-color: #E7EDF5; display: none">
                        <asp:UpdatePanel ID="updatapanel_iframe_xml_autoriza" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <iframe runat="server" id="ifmExcel_xml_autoriza" width="0" height="0" marginheight="0" marginwidth="0"
                                    frameborder="0" />
                                <asp:Button ID="Button_export_lista_event_xml_autoriza" runat="server" Text="Exportar" Style="margin-top: 5px; display: none" CssClass="boton_azul" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>  
                </div>
              <div style="display:none">   
                  <asp:UpdatePanel ID="UpdatePanel_tool_menu" runat="server" UpdateMode="Conditional">
                      <ContentTemplate>        
                           <asp:ImageButton ID="ImageButton_ista_autorizacio" runat="server" style="display:none"    />  
                          <asp:Button ID="Button_tool_activa_detalle_radicado" runat="server" Text="" style="display:none"/>  
                      </ContentTemplate>
                  </asp:UpdatePanel>  
              </div>
             <!--imagen_respuesta-->
         <asp:Panel ID="Panel_imagen_respuesta" runat="server" Style="display: none; overflow: hidden; width: 99%; color:black"  Height="100% " CssClass="modal_content_general_">
            <asp:ModalPopupExtender ID="ModalPopupExtender_imagen_respuesta" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_imagen_respuesta_dos"
                PopupControlID="Panel_imagen_respuesta" CancelControlID="ButtonSalir_imagen_respuesta">
            </asp:ModalPopupExtender>
            <div id="modal_content_Panel_imagen_respuesta" class="modal-content">
                <div id="Cabecerapendiente_imagen_respuesta" class="modal_title_superior_ modal-header" >
                    <h6 class="modal-title d-inline ml-1">Documentos</h6>
                    <button type="button" value="ButtonSalir_imagen_respuesta" class="close da_event_captive">&times;</button>               
                </div>
                <div id="Cotenedorpendiente_imagen_respuesta" style="height: 100%; width: 100%;  border-top:none; overflow:auto" class="modal_content_back p-0">
                    <asp:UpdatePanel ID="UpdatePanel_imagen_respuesta" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <iframe id="Iframe_imagen_respuesta_" runat="server" frameborder="0" style="width: 100%; height: 100%; overflow: hidden"></iframe>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
            <div style="display: none; height: 0px">
                <asp:Button ID="Button_imagen_respuesta_dos" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                <asp:Button ID="ButtonSalir_imagen_respuesta" runat="Server" Text="" CssClass="modal_boton_hiden" />
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
                                            <asp:Label ID="Label3" runat="server" Text="RADICADO DEL TRAMITE"></asp:Label>
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
                                            <asp:Label ID="Label7" runat="server" Text="RADICADO A NOMBRE DE "></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:Label ID="LabelASIGNADO" runat="server" Text=""></asp:Label>
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
                                            <asp:Label ID="Label5" runat="server" Text="FECHA VENCIMIENTO DEL TRAMITE"></asp:Label>
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
                                            <asp:Label ID="Label11" runat="server" Text="FLUJO RADICADO"></asp:Label>
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
                                            <asp:Label ID="Label12" runat="server" Text="CARGO USUARIO RADICADOR"></asp:Label>
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
         <asp:Panel ID="Panel_reasigna_tarea_workflow_sii" runat="server" Style="display:none; width: 40%; height: auto" CssClass="modal_content_general_">
            <asp:ModalPopupExtender ID="ModalPopupExtender_edition_reasigna_tarea_workflow_sii" runat="server"
                TargetControlID="ButtonSalir_reasigna_tarea_workflow_sii" BackgroundCssClass="FondoAplicacion"
                CancelControlID="Button_cerrar_reasigna_tarea_workflow_sii" PopupControlID="Panel_reasigna_tarea_workflow_sii">
            </asp:ModalPopupExtender>     
             <div id="modal_content_reasigna_tarea_workflow_sii" class="modal-content">
                 <div id="diver_cabcera_reasigna_tarea_workflow_sii" class="modal_title_superior_ modal-header">
                     <h6 class="modal-title d-inline ">Reasignar</h6>
                     <button type="button" value="Button_cerrar_reasigna_tarea_workflow_sii" class="close da_event_captive ">&times;</button>
                 </div>
                 <div id="contenido_procesa_reasigna_tarea_workflow_sii" style="background-color: white; width: 100%; height: 100%; border-top: none" class="modal_content_back modal-body">
                     <div id="content_data_grid_reasigna_tarea_workflow_sii" class="conten_gred_border_" style="width: 100%">
                         <div class="row ">
                             <div class="col-4">
                                 <span> Actividades</span>
                             </div>
                             <div class="col-8">
                                 <asp:DropDownList ID="DropDownList_list_actividad_workflow_sii" name_event="L-USER-WF" value_event="na" Style="width: 100%" CssClass="custom-select mr-sm-2 bt_sys_event_element_option" runat="server"></asp:DropDownList>
                             </div>
                         </div>
                         <div class="row pt-2">
                             <div class="col-4">
                                 <span>Usuario</span>
                             </div>
                             <div class="col-8">
                                 <asp:DropDownList ID="DropDownList_list_usuario_workflow_sii" Style="width: 100%" CssClass="custom-select mr-sm-2" runat="server"></asp:DropDownList>
                             </div>
                         </div>
                         
                     </div>
                     <div style="display: none; height: 1px">
                         <asp:Button ID="ButtonSalir_reasigna_tarea_workflow_sii" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                         <asp:Button ID="Button_cerrar_reasigna_tarea_workflow_sii" runat="Server" Text="X" CssClass="invisible" />
                     </div>
                 </div>
                 <div class="modal-footer justify-content-end" id="modal-footer_list_inscripciones_sii">
                     <input id="Button_reasigna_sii" type="button" value="Aceptar" name_event="R-REA-SII" value_event="na" class="btn btn-success bt_sys_event_element" />

                 </div>

             </div>
        </asp:Panel>
      <div id="inferior_bajo_boton" style="width: 100%; height: 20%; background-color: #E7EDF5; display: none">
            <asp:UpdatePanel ID="updatapanel_iframe" runat="server" UpdateMode="Conditional">
                <ContentTemplate>

                    <asp:Label ID="Label17" runat="server" Text="Estado" Style="font-size: 8px; font-family: Arial; float: right"></asp:Label>
                    <iframe runat="server" id="ifmExcel_" width="0" height="0" marginheight="0" marginwidth="0"
                        frameborder="0" />
                    <input id="Hidden_ruta_archivo_descarga" type="hidden" value="" runat="server"/>
                </ContentTemplate>

            </asp:UpdatePanel>
        </div>
        <asp:Panel ID="Panel_detail_document_proces_workflow" runat="server" Style="display:none; width: 100%; height: auto" CssClass="modal_content_general_">
            <asp:ModalPopupExtender ID="ModalPopupExtender_edition_detail_document_proces_workflow" runat="server"
                TargetControlID="ButtonSalir_detail_document_proces_workflow" BackgroundCssClass="FondoAplicacion"
                CancelControlID="Button_cerrar_detail_document_proces_workflow" PopupControlID="Panel_detail_document_proces_workflow">
            </asp:ModalPopupExtender>     
             <div id="modal_content_detail_document_proces_workflow" class="modal-content">
                 <div id="diver_cabcera_detail_document_proces_workflow" class="modal_title_superior_ modal-header">
                     <h6 class="modal-title d-inline ">Detalle procesos de imagenes en workflow</h6>
                     <button type="button" value="Button_cerrar_detail_document_proces_workflow" class="close da_event_captive ">&times;</button>
                 </div>
                 <div id="contenido_procesa_detail_document_proces_workflow" style="background-color: white; width: 100%; height: 100%; border-top: none" class="modal_content_back modal-body">
                     <div id="div_content_tabla_procesa_detail_document_proces_workflow" class="table-responsive">
                         <table
                             id="table_boot_detail_document"
                             data-pagination="true"
                             data-page-list="[5,10,15,20,25, 50, 100, all]"
                             data-show-export="true"
                             data-toggle="table"
                             data-id-field="Id_log_docuarchi"
                             data-search="true"
                             data-locale="es-SP">
                             <thead>
                                 <tr>
                                     <th data-field="Id_log_docuarchi" data-visible="false" style="display: none">Id_log_docuarchi</th>
                                     <th data-field="desc_op" data-sortable="true" data-sort-name="desc_op" data-sort-order="desc">OPERACION</th>
                                     <th data-field="USER_OPER">USUARIO DE LA OPERACION</th>
                                     <th data-field="DATE_TRANS">FECHA</th>
                                     <th data-field="HORA_REGISTRO">HORA</th>
                                     <th data-field="id_tran">DOCUMENTO</th>
                                     <th data-field="USER_PROPIETARIO">PROPIETARIO DEL DOCUMENTO</th>
                                     <th data-field="TIPOLOGIA_DOCUMENTAL">TIPOLOGIA</th>
                                     <th data-field="RUT_DOCU">RUTA</th>
                                     <th data-field="GABINETE">GABINETE</th>
                                     <th data-field="CAMPOS">DATOS IDENTIFICACION</th>
                                     <th data-field="RADICADO">RADICADO</th>
                                    
                                 </tr>
                             </thead>
                         </table>
                     </div>

                     <div style="display: none; height: 1px">
                         <asp:Button ID="ButtonSalir_detail_document_proces_workflow" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                         <asp:Button ID="Button_cerrar_detail_document_proces_workflow" runat="Server" Text="X" CssClass="invisible" />
                     </div>
                 </div>
                 <div class="modal-footer justify-content-end" id="modal-footer_detail_document_proces_workflow">
                     

                 </div>

             </div>
        </asp:Panel>
        <asp:Panel ID="Panel_detail_notes_task_workflow" runat="server" Style="display:none; width: 100%; height: auto" CssClass="modal_content_general_">
            <asp:ModalPopupExtender ID="ModalPopupExtender_edition_detail_notes_task_workflow" runat="server"
                TargetControlID="ButtonSalir_detail_notes_task_workflow" BackgroundCssClass="FondoAplicacion"
                CancelControlID="Button_cerrar_detail_notes_task_workflow" PopupControlID="Panel_detail_notes_task_workflow">
            </asp:ModalPopupExtender>     
             <div id="modal_content_detail_notes_task_workflow" class="modal-content">
                 <div id="diver_cabcera_detail_notes_task_workflow" class="modal_title_superior_ modal-header">
                     <h6 class="modal-title d-inline " id="title_detail_notes_task_workflow">Detalle notas</h6>
                     <button type="button" value="Button_cerrar_detail_notes_task_workflow" class="close da_event_captive ">&times;</button>
                 </div>
                 <div id="contenido_procesa_detail_notes_task_workflow" style="background-color: white; width: 100%; height: 100%; border-top: none" class="modal_content_back modal-body">
                     <div id="div_content_tabla_procesa_detail_notes_task_workflow" class="table-responsive">
                         <table
                             id="table_boot_detail_notes_task_workflow"
                             data-pagination="true"
                             data-page-list="[5,10,15,20,25, 50, 100, all]"
                             data-show-export="true"
                             data-toggle="table"
                             data-id-field="Id_Anotacion"
                             data-search="true"
                             data-locale="es-SP">
                             <thead>
                                 <tr>
                                     <th data-field="Id_Anotacion" data-visible="false" style="display: none">Id_Anotacion</th>
                                     <th data-field="fecha_anotacion">FECHA</th>
                                     <th data-field="nombre_actividad"> ACTIVIDAD</th>
                                     <th data-field="nombre_usuario">USUARIO</th>
                                     <th data-field="dato_anotacion">ANOTACION</th>
                                 </tr>
                             </thead>
                         </table>
                     </div>

                     <div style="display: none; height: 1px">
                         <asp:Button ID="ButtonSalir_detail_notes_task_workflow" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                         <asp:Button ID="Button_cerrar_detail_notes_task_workflow" runat="Server" Text="X" CssClass="invisible" />
                     </div>
                 </div>
                 <div class="modal-footer justify-content-end" id="modal-footer_detail_notes_task_workflow">
                     
                 </div>
             </div>
        </asp:Panel>
        <asp:Panel ID="Panel_detail_copy_document_expediente_wf" runat="server" Style="display:none; width: 100%; height: auto" CssClass="modal_content_general_">
            <asp:ModalPopupExtender ID="ModalPopupExtender_edition_detail_copy_document_expediente_wf" runat="server"
                TargetControlID="ButtonSalir_detail_copy_document_expediente_wf" BackgroundCssClass="FondoAplicacion"
                CancelControlID="Button_cerrar_detail_copy_document_expediente_wf" PopupControlID="Panel_detail_copy_document_expediente_wf">
            </asp:ModalPopupExtender>     
             <div id="modal_content_detail_copy_document_expediente_wf" class="modal-content">
                 <div id="diver_cabcera_detail_copy_document_expediente_wf" class="modal_title_superior_ modal-header">
                     <h6 class="modal-title d-inline ">Detalle copia de documentos a expediente</h6>
                     <button type="button" value="Button_cerrar_detail_copy_document_expediente_wf" class="close da_event_captive ">&times;</button>
                 </div>
                 <div id="contenido_procesa_detail_copy_document_expediente_wf" style="background-color: white; width: 100%; height: 100%; border-top: none" class="modal_content_back modal-body">
                     <div id="div_content_tabla_procesa_detail_copy_document_expediente_wf" class="table-responsive">
                         <table
                             id="table_boot_detail_copy_document_expdient"
                             data-pagination="true"
                             data-page-list="[5,10,15,20,25, 50, 100, all]"
                             data-show-export="true"
                             data-toggle="table"
                             data-id-field="id_relacion_wf_produccion"
                             data-search="true"
                             data-locale="es-SP">
                             <thead>
                                 <tr>
                                     <th data-field="id_relacion_wf_produccion" data-visible="false" style="display: none">Id_log_docuarchi</th>
                                     <th data-field="estado_copia_vincula" data-sortable="true" data-sort-name="estado_copia_vincula" data-sort-order="desc">OPERACION</th>
                                     <th data-field="date_registro_trans">FECHA</th>
                                     <th data-field="Nombre_Remitente">USUARIO</th>          
                                     <th data-field="Cargo_Remite">CARGO</th>
                                     <th data-field="DESCRIPCION_TIPO_DOCUMENTO">DOCUMENTO</th>
                                     <th data-field="ID_EXPEDIENTE">CODIGO EXPEDIENTE</th>
                                     <th data-field="codigo_unico">EXPEDIENTE DESTINO</th> 
                                 </tr>
                             </thead>
                         </table>
                     </div>

                     <div style="display: none; height: 1px">
                         <asp:Button ID="ButtonSalir_detail_copy_document_expediente_wf" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                         <asp:Button ID="Button_cerrar_detail_copy_document_expediente_wf" runat="Server" Text="X" CssClass="invisible" />
                     </div>
                 </div>
                 <div class="modal-footer justify-content-end" id="modal-footer_detail_copy_document_expediente_wf">
                     

                 </div>

             </div>
        </asp:Panel>

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
                  $('#contenido_general').toggleClass('hide_da_sidebar');
                  $(this).toggleClass('active_da_slider');
                  $('#da_show-sidebar_').toggleClass('show_da_slide');
                  $('#da_show-sidebar_').toggleClass('hide_da_sidebar');
              });
              $('#da_show-sidebar_').on('click', function () {
                  $('#contenido_general').toggleClass('active_da_slider');
                  $('#contenido_general').toggleClass('hide_da_sidebar');
                  $(this).toggleClass('show_da_slide');
                  $(this).toggleClass('hide_da_sidebar');
              });
          });
    </script>
</body>
</html>

