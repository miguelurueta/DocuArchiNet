<%@ Page Language="vb" AutoEventWireup="false" EnableEventValidation="false" CodeBehind="WebFormGestionPlantillasvalidacion.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormGestionPlantillasvalidacion" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" tagprefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Gestión plantillas</title>
       <script src="../js/ui/jquery-3.4.1.min.js"></script>  
       <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
   <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
     <script src="../js/java_general/general_code_java.js" type="text/javascript"></script>
    <script src="../js/java_general/ubicacion_code_java.js" type="text/javascript"></script>   
    <script src="../js/radicacion/WebFormGestionPlantillasvalidacion.js"></script>
    <script src="../js/java_general/general_control_java.js"></script>
    <script src="../js/java_general/general_code_java.js"></script>
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <script src="../js/java_general/ubicacion_code_java.js"></script>
    <script src="../js/java_general/row_multiple_gred.js"></script>
    <script src="../js/MyJavaScriptFile.js"></script>
    <script src="../js/validate_campos.js"></script>
       <script  src="../Awesome/js/all.js"></script>    
    <link href="../Awesome/css/fontawesome.css" rel="stylesheet"/>
    <link href="../Awesome/css/brands.css" rel="stylesheet"/>
    <link href="../Awesome/css/solid.css" rel="stylesheet"/>
    <script  src="../Awesome/js/brands.js"></script>
    <script  src="../Awesome/js/solid.js"></script>
    <script  src="../Awesome/js/fontawesome.js"></script> 

</head>
<body style="overflow:hidden">
    <form id="form1" runat="server" defaultbutton="Button_default_buton">
    <div>
    
      <asp:ScriptManager ID="ScriptManager1" runat="server">
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
                    //
                    elment_postbak = args.get_postBackElement();
                   
                    if (args.get_postBackElement().id == 'Button_consulta_val_radicacion') {
                        document.getElementById("Button_consulta_val_radicacion").disabled = true;
                        document.getElementById("Button_consulta_val_radicacion").value = "Espere....";
                    }
                    if (args.get_postBackElement().id == 'Button_edita_campos_dinamicos') {
                        document.getElementById("Button_edita_campos_dinamicos").disabled = true;
                        document.getElementById("Button_edita_campos_dinamicos").value = "Espere....";
                        //document.getElementById("Button_registrar").style.display = "none";
                    }
                    if (args.get_postBackElement().id == 'Button_registrar') {
                        document.getElementById("Button_registrar").disabled = true;
                        document.getElementById("Button_registrar").value = "Espere....";
                        //document.getElementById("Button_edita_campos_dinamicos").style.display = "none";
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
                    if (elment_postbak.id == 'Button_consulta_val_radicacion') {
                        document.getElementById("Button_consulta_val_radicacion").disabled = false;
                        document.getElementById("Button_consulta_val_radicacion").value = "Consultar";
                        
                    }
                    if (elment_postbak.id == "Button_edita_campos_dinamicos") {
                        document.getElementById("Button_edita_campos_dinamicos").disabled = false;
                        document.getElementById("Button_edita_campos_dinamicos").value = "Actualizar";
                        if (document.getElementById("Hidden_campos_validacion").value !== "") {
                            actualiza_datos_data_gredview(document.getElementById("Hidden_campos_validacion").value, document.getElementById("Hidden_valores_validacion").value);
                        }
                        
                        
                    }
                    if (elment_postbak.id == 'Button_registrar') {
                        document.getElementById("Button_registrar").disabled = false;
                        document.getElementById("Button_registrar").value = "Registrar";
                        if (document.getElementById("Hidden_resultado_registrar").value == "-1") {
                            
                            selecciona_item_registrado();
                            document.getElementById("Hidden_resultado_registrar").value = "";
                        }
                        
                    }
                    if (elment_postbak.id == "Editar_pre") {
                        document.getElementById("Label_edit_campo_title").innerText = "Edita remitente";
                    }
                    if (elment_postbak.id == "Button_pre_agregar") {
                        document.getElementById("Label_edit_campo_title").innerText = "Registra remitente";
                    }
                    
                    //Eliminar
                    if (elment_postbak.id == "Eliminar") {
                        document.getElementById("Eliminar").disabled = false;
                        document.getElementById("Eliminar").value = "Eliminar";
                        if (document.getElementById("Hidden_resultado_eliminar").value == "1") {
                            eliminar_fila_data_gred();
                            document.getElementById("Hidden_resultado_eliminar").value = "1";
                        }

                    }

                    
                    }
                    catch (err) {
                        alert(err.message + " Funcion InitializeRequest");
                    }

                }

            </script>
         <div id="content_error"></div>
         <div id="contendor_principal" style="height: 100%; width:auto">
          <nav id="menu_var" class="navbar navbar-expand-sm nav_botota_person modal_content_no_back_inferior">
                <button id="nav_togle_display" class="navbar-toggler" type="button" style=" background-color:#6d7fcc" data-toggle="collapse" data-target="#navbarNavDropdown">
                   <span class="navbar-toggler-icon_"><i style="color:white" class="fad fa-th-list"></i></span>
               </button>
                <div class="collapse navbar-collapse row" id="navbarNavDropdown">
                    <ul class="navbar-nav col-md-8"> 
                         <li class="nav-item dropdown active ml-2 active_">                  
                            <a class="nav-link dropdown-toggle bot_hover_person" style="color:#6d7fcc" href="#" id="navbarDropdownMenuLink" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"> <i style="color:#0062cc" class="fad fa-bars"></i> Menú
                            </a>
                            <div class="dropdown-menu " aria-labelledby="navbarDropdownMenuLink">  
                                 <a href="#" class="dropdown-item" onclick="activa_menu('i_r_r_001')"><i class="fas fa-user-plus"></i> Nuevo registro de usuario</a>                   
                            </div>
                        </li>          
                    </ul>   
                </div>
            </nav>
         <a id="da_show-sidebar_" class="btn btn-sm   hide_da_sidebar " href="#" data-target="#sidebar_">
                <i style="color: white" class="fas fa-bars"></i>
         </a>
         <div id="da_content_wraper" class=" ml-0 mr-2  d-flex " style="padding-left: 1px; padding-right: 1px">
             <div id="Contentizquierdo" style="width:25%; float:left">
                 <nav id="sidebar_" class=" bg-light pl-0 pr-0 " >
                     <div id="contenido_titulo_controles_consulta" class="modal-header modal_title_superior bg-light" style=" border-top-left-radius: initial; border-top-right-radius: initial">
                          <h6 class=" mt-2 mb-2 ml-2 font-weight-light" id="pit_" style="color:black; float: left; font-family:'Segoe UI'">Campos de busqueda </h6>
                        <a id="sidebarCollapse" class="close_ mr-1" style="float: right; color:black; height: 10px">&times;</a>
                     </div>
                     <div id="contenido_controles_consulta"  style="width: 100%">
                         <asp:UpdatePanel ID="UpdatePanelContenido" runat="server" UpdateMode="Conditional" >
                             <ContentTemplate>
                                 <asp:Panel ID="_Panelvalidacion" runat="server"  Height="100%" Width="98%" Wrap="false" style="overflow:auto; background-color:white" DefaultButton="Button_consulta_val_radicacion" CssClass="pl-2 pb-2 mr-1 ml-1" >
                                     <asp:Table ID="_ValidacionConsulta" runat="server" ForeColor="Black" BackColor="White" ViewStateMode="Enabled" Wrap="false" Width="100%">
                                     </asp:Table>

                                 </asp:Panel>
                                 <div id="_campos_seleccion" style="height: 0px; display: none">
                                     <asp:TextBox ID="TextBoxEditNombreDestRem" runat="server"></asp:TextBox>
                                     <input id="Hiddenselecionpais" type="hidden" value="COLOMBIA" runat="server">
                                     <input id="Hiddenseleciondepartamento" runat="server" value="" type="hidden">
                                     <input id="Hiddenvalidacion" type="hidden" value="" runat="server">
                                     <input id="Hiddenmunicipio" runat="server" value="" type="hidden">
                                     <input id="Hiddenestadoedicion" runat="server" value="0" type="hidden">
                                     <input id="Hiddenrelacionvalidacion" runat="server" type="hidden" value="-1">
                                 </div>
                             </ContentTemplate>
                             <Triggers>
                                 <asp:AsyncPostBackTrigger ControlID="Buttonllenardepartamento" EventName="Click" />
                                 <asp:AsyncPostBackTrigger ControlID="Buttonllenarciudad" EventName="Click" />
                             </Triggers>
                         </asp:UpdatePanel>
                     </div>
                   
                     <div id="contenido_controles_buton_consulta" style="border-top-left-radius: initial; border-top-right-radius: initial" class="modal-header justify-content-start">
                         <asp:UpdatePanel ID="UpdatePanel_botones_validacion" runat="server" UpdateMode="Conditional">
                             <ContentTemplate>
                                 &nbsp
                                     <asp:Button ID="Button_consulta_val_radicacion" runat="server" Text="Consultar" ToolTip="Consultar radicados" CssClass="btn btn-success" />
                                 &nbsp 
                                     <asp:Button ID="Button_lipiar_val_radicacion" Text="Limpiar" runat="server"  Style="margin-top: 3px" ToolTip="Limpiar campos radicacion" CssClass="btn btn-success" />
                                 &nbsp 
                                     <asp:Button ID="Button_ejecutar_consulta" Text="Limpiar" runat="server" Width="100px" ToolTip="Limpiar campos radicacion" CssClass="boton" Style="display: none" />
                             </ContentTemplate>
                         </asp:UpdatePanel>
                     </div>
                 </nav>
             </div>
               <asp:UpdatePanel ID="UpdatePanel_botones_configuracion" runat="server" UpdateMode="Conditional">
                         <ContentTemplate>
                             <div id="botones_control_autollenar" style="height: 0px; display: none">
                                 <asp:Button ID="Buttonllenardepartamento" runat="server" Text="Button" BackColor="Silver" />
                                 <asp:Button ID="Buttonllenarciudad" runat="server" Text="Button" />
                             </div>

                         </ContentTemplate>
                     </asp:UpdatePanel>
             <input id="Hidden_asig_" type="hidden" value="" runat="server">
             <div id="Contenedorderecho" class=" mr-0 ml-0 pl-1 pr-1 pb-0 pt-0 " style="width: 75%; float: right">
                 <div id="contenido_titulo_val_radicacion" class=" p-2">
                     <asp:UpdatePanel ID="UpdatePanelabel_validacion" runat="server" UpdateMode="Conditional">
                         <ContentTemplate>
                             <input id="hdnEmailID_VAL" type="hidden" value="-1" runat="server">
                              
                             <input id="Hidden_consecutivo_radicado" type="hidden" value="-1" runat="server">
                             <asp:Label ID="titulo_label_validacion" runat="server" CssClass="h6 font-weight-light p-1" Style="font-family:'Segoe UI'" >Resultados busqueda</asp:Label>
                         </ContentTemplate>
                     </asp:UpdatePanel>
                 </div>

                 <div id="contenido_datagrid_val_radicacion" style="height: 57%; width: 100%; position: relative; background-color: white; overflow:auto">
                     <asp:UpdatePanel ID="UpdatePanel_conenido_grid_val_radicacion" runat="server" UpdateMode="Conditional" >
                         <ContentTemplate>
                              <asp:GridView ID="GridView_val_radicacion" runat="server" AllowSorting="true"  AllowPaging="false"  EnableViewState="true"
                                  PageSize="7" PagerSettings-Position="Top"  style=" font-family:Segoe UI"
                                    AutoGenerateSelectButton="False" CssClass="table font-weight-light  " GridLines="None"  >
                                    <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                    <HeaderStyle CssClass="GridviewScrollHeader_line_boot" BorderStyle="None" />
                                    <RowStyle CssClass=""  />
                                    <PagerStyle CssClass="pagination-ys" />
                                    <Columns>
                                        <asp:BoundField HeaderText="OPCIONES"   />
                                    </Columns>

                                </asp:GridView>

                         </ContentTemplate>

                     </asp:UpdatePanel>
                 </div>
                 <div id="Contenido_botones_tipo_radicado" style="height: 10%; width: 100%; float: left; overflow: auto; background-color: white; display:none">
                     <!-- <div id="superior_alto_boton" style="width: 100%; height: 5%; background-color: #E7EDF5" ></div>-->
                     <div id="contennido_buton" style="width: 100%; height: 100%">
                         <asp:UpdatePanel ID="UpdatePanel_botones_radicacion" runat="server" UpdateMode="Conditional">
                             <ContentTemplate>
                                 <asp:Label ID="Label_resultado" runat="server" Text="" Style="float: right; font-size: 10px"></asp:Label>
                                 &nbsp
                        <asp:Button ID="Button_Asigna" runat="server" Text="Asignar" Width="100px" Style="margin-top: 3px; display:none" ToolTip="Asigna registro validación" CssClass="boton_azul" OnClientClick="asigna_registro_validacion();" />
                                 &nbsp
                            <asp:Button ID="Editar_pre" runat="server" Width="80px" Style="margin-top: 3px; display:none" Text="Editar" ToolTip="Editar registro" CssClass="boton_azul" />
                                 &nbsp     
                            <asp:Button ID="Eliminar" runat="server" Text="Eliminar" Width="80px" Style="margin-top: 3px; display:none" ToolTip="Eliminar registro seleccionado" OnClientClick="ConfirmMensajeEliminar(&quot;Desea Eliminar el registro &quot;);" CssClass="boton_azul" />
                                 &nbsp     
                            <asp:Button ID="Button_pre_agregar" runat="server" Text="Agregar" Width="80px" Style="margin-top: 3px; display:none" ToolTip="Agregar nuevo registro" CssClass="boton_azul" />
                                 <input id="Hidden_resultado_eliminar" runat="server" type="hidden" value="-1"/>
                             </ContentTemplate>

                         </asp:UpdatePanel>
                     </div>
                    
                 </div>
             </div>
         </div>
        

         </div>     
          <div id="edita_campos_dinamicos" >
            <asp:Panel ID="panel_edita_campos_dinamicos" runat="server" style="display:none; height:auto;  width:60%" CssClass="modal_content_general_">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edita_campos_dinamicos" runat="server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_edita_campos_dinamicos"
                    PopupControlID="panel_edita_campos_dinamicos" CancelControlID="Button_cerrar_edita_campos_dinamicos"></asp:ModalPopupExtender>
                <div id="modal_content_edita_campos_dinamicos" class="modal-content">
                    <div id="divcabecer2_edita_campos_dinamicos" class="modal_title_superior_ modal-header">
                        <h6 id="Label_edit_campo_title" class="modal-title d-inline">Edita campos dinamicos</h6>
                        <button  type="button" value="Button_cerrar_edita_campos_dinamicos" class="close da_event_captive">&times;</button>   
                    </div>
                    <div id="content_general" style="color: white; width: 100%; height: auto ; border-top:none" class="modal_content_back modal-body">
                       
                            <asp:UpdatePanel ID="UpdatePanel_edita_campos_dinamicos" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Panel ID="Panel_dinamico_edita_campos_dinamicos" runat="server" Style="color: white; overflow:auto"  Height="100%" BackColor="White" Width="100%" Wrap="false">
                                        <asp:Table ID="Table_edita_campos_dinamicos" runat="server" ForeColor="Black" BackColor="White" ViewStateMode="Enabled" Wrap="false" Width="100%">
                                        </asp:Table>
                                    </asp:Panel>
                                    <div id="_campos_seleccion_EDIT" style="height: 0px; display: none">
                                        <asp:TextBox ID="TextBoxEditNombreDestRem_EDIT" runat="server"></asp:TextBox>
                                        <input id="Hiddenselecionpais_EDIT" type="hidden" value="COLOMBIA" runat="server">
                                        <input id="Hiddenseleciondepartamento_EDIT" runat="server" value="" type="hidden">
                                        <input id="Hiddenvalidacion_EDIT" type="hidden" value="" runat="server">
                                        <input id="Hiddenmunicipio_EDIT" runat="server" value="" type="hidden">
                                        <input id="Hiddenestadoedicion_EDIT" runat="server" value="0" type="hidden">
                                        <input id="Hiddenrelacionvalidacion_EDIT" runat="server" type="hidden" value="-1">
                                    </div>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="Buttonllenardepartamento_edit" EventName="Click" />
                                    <asp:AsyncPostBackTrigger ControlID="Buttonllenarciudad_edit" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>
                                       
                    </div>
                     <div id="botones_edita_campos_dinamicos" class="modal-footer justify-content-end" >
                            <asp:UpdatePanel ID="UpdatePanel_edita_campos_dinamicos_actualiza" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    &nbsp
                                <asp:Button ID="Button_edita_campos_dinamicos" runat="server" Text="Acutalizar" ToolTip="Actualiza campos dinamicos" CssClass="btn btn-success" />
                                    <asp:Button ID="Button_registrar" runat="server" Text="Registrar" ToolTip="Registrar nuevo iten de plantilla" CssClass="btn btn-success" Style="margin-left: 3px" />
                                    <asp:Button ID="Button_limpiar_campos_edicion" runat="server" Text="Restaurar" ToolTip="Limpiar campos de edición y registro" CssClass="btn btn-success" Style="margin-left: 3px" />
                                    <input id="Hidden_campos_validacion" runat="server" type="hidden" value="">
                                    <input id="Hidden_valores_validacion" runat="server" type="hidden" value="">
                                    <input id="Hidden_resultado_registrar" runat="server" type="hidden" value="">
                                </ContentTemplate>
                            </asp:UpdatePanel>

                        </div>
                    <div style="display: none; height: 1px">
                        <asp:Button ID="ButtonSalir_edita_campos_dinamicos" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" Style="display: none" />
                        <asp:Button ID="Button_cerrar_edita_campos_dinamicos" runat="Server" Text="" CssClass="invisible" Height="1px" Width="1px" Style="display: none" />
                        <asp:UpdatePanel ID="UpdatePanel_botones_configuracion_edit" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div id="botones_control_autollenar_edit" style="height: 0px; display: none">
                                    <asp:Button ID="Buttonllenardepartamento_edit" runat="server" Text="Button" BackColor="Silver" />
                                    <asp:Button ID="Buttonllenarciudad_edit" runat="server" Text="Button" />

                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    
                </div>
            </asp:Panel>
        </div>

        <asp:Panel ID="Panel_registro_validacion_externo" runat="server" Style="display: none; width: 80%; height: auto" CssClass="modal_content_general">
            <asp:ModalPopupExtender ID="ModalPopupExtender_edition_registro_validacion_externo" runat="server" TargetControlID="ButtonSalir_registro_validacion_externo" BackgroundCssClass="FondoAplicacion"
                CancelControlID="Button_cerrar_registro_validacion_externo" PopupControlID="Panel_registro_validacion_externo">
            </asp:ModalPopupExtender>
            <div class="modal-content_" id="modal_content_registro_validacion_externo">
                <div id="title_registro_validacion_externo" class="modal_title_superior_ modal-header">
                    <h6 id="label_title_registro_validacion_externo" class="modal-title">Gestion de terceros</h6>
                    <button type="button" value="Button_cerrar_registro_validacion_externo" class="close da_event_captive">&times;</button>
                </div>
                <div id="contenido_procesa_registro_validacion_externo" style="width: auto; height: auto; border-top: none; overflow:auto" class="modal_content_back modal-body">
                    <div id="div_registro_validacion_externo" style="height:100%">       
                         
                    </div>
                </div>
            </div>
            <div class="modal-footer align-content-end" id="modal_foter_registro_validacion_externo">
                <button type="button" id="boton_event_registro_validacion_externo" title="Registro destinatario externo"  style="display:block" class="btn btn-success   mt-1"> Aceptar</button>
                <button type="button" id="boton_event_update_validacion_externo" title="Edita destinatario externo " style="display:block" class="btn btn-success   mt-1"> Aceptar</button>
                <button type="button" title="" value="Button_cerrar_registro_validacion_externo" class="btn btn-light da_event_captive  mt-1"> Cancelar </button>  
            </div>
            <div style="display: none; height: 1px">
                <asp:Button ID="Button_cerrar_registro_validacion_externo" runat="Server" Text="X" CssClass="invisible" />
                <asp:Button ID="Button_registro_validacion_externo" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                <asp:Button ID="ButtonSalir_registro_validacion_externo" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
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

          
    </div>

         <input id="HiddenPROMP" type="hidden" value="1" runat="server">
         <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 200px">
                <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
                Processing ...
            </div>
        <asp:UpdatePanel ID="UpdatePanel_default" runat="server" UpdateMode="Conditional" >
            <ContentTemplate>
                 <asp:Button ID="Button_default_buton" runat="server" Text="Button" Style="display:none" />
            </ContentTemplate>
        </asp:UpdatePanel>
       
    </form>
     <script  accesskey="javascript" type="text/javascript">
        

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
</script>
</body>
</html>
