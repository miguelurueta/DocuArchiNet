<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormConsultaRadicadoPublico.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormConsultaRadicadoPublico" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Consulta radicado publico</title>
    <script src="../js/ui/jquery-3.4.1.min.js"></script>  
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
    <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
    <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <script src="../js/Publico/WebFormConsultaRadicadoPublico.js"></script>
    <script  accesskey="javascript" type="text/javascript">  </script>
    <script src="../js/java_general/general_code_java.js"></script>
    <script  src="../Awesome/js/all.js"></script>
    <link href="../Awesome/css/fontawesome.css" rel="stylesheet"/>
    <link href="../Awesome/css/brands.css" rel="stylesheet"/>
    <link href="../Awesome/css/solid.css" rel="stylesheet"/>
    <script  src="../Awesome/js/brands.js"></script>
    <script  src="../Awesome/js/solid.js"></script>
    <script  src="../Awesome/js/fontawesome.js"></script>
  <!--===============================================================================================-->
	<link rel="stylesheet" type="text/css" href="../colorlib/vendor/bootstrap/css/bootstrap.min.css"/>
 <!--===============================================================================================-->
	<link rel="stylesheet" type="text/css" href="../colorlib/fonts/font-awesome-4.7.0/css/font-awesome.min.css"/>
<!--===============================================================================================-->
	<link rel="stylesheet" type="text/css" href="../colorlib/vendor/animate/animate.css"/>
<!--===============================================================================================-->	
	<link rel="stylesheet" type="text/css" href="../colorlib/vendor/css-hamburgers/hamburgers.min.css"/>
<!--===============================================================================================-->
	<link rel="stylesheet" type="text/css" href="../colorlib/vendor/animsition/css/animsition.min.css"/>
<!--===============================================================================================-->
	<link rel="stylesheet" type="text/css" href="../colorlib/vendor/select2/select2.min.css"/>
<!--===============================================================================================-->	
	<link rel="stylesheet" type="text/css" href="../colorlib/vendor/daterangepicker/daterangepicker.css"/>
<!--===============================================================================================-->
	<link rel="stylesheet" type="text/css" href="../colorlib/css/util.css"/>
	<link rel="stylesheet" type="text/css" href="../colorlib/css/main.css"/>
<!--===============================================================================================-->
</head>
<body >
    <form id="formconsultaradicacion" runat="server">
        
            <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" EnablePageMethods="true">
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
                        
                        if (elment_postbak.id == 'Button_consulta_val_radicacion') {
                            document.getElementById("Button_consulta_val_radicacion").disabled = false;
                            document.getElementById("Button_consulta_val_radicacion").value = "Consultar";
                            if (document.getElementById("Hidden_resultado_consulta").value == "YES") {
                                document.getElementById("Hidden_resultado_consulta").value == "";
                                plugin_grwedview();
                            }

                        }

                       
                        if (elment_postbak.id == "Button_visor_emergente") {
                            //document.getElementById("Label12").innerHTML = "Visor externo de documentos";
                            //auto_zise_popup_visor_externo();
                        }
                        if (elment_postbak.id == "Button_Trazabilidad") {
                            //document.getElementById("Label12").innerHTML = "Trazabilidad de radicados";
                            //auto_zise_popup_visor_externo();
                        }
                        if (elment_postbak.id == "Button_Log_respuesta") {
                            //document.getElementById("Label12").innerHTML = "Transacciones de respuestas";
                            //auto_zise_popup_visor_externo();
                        }
                        if (elment_postbak.id == "Button_detalle_radicado") {
                            //document.getElementById("Label12").innerHTML = "Detalle radicado";
                            //auto_zise_popup_visor_externo();
                        }
                        //Button_Log_respuesta Button_detalle_radicado
                    }
                    catch (err) {
                        alert(err.message + " Funcion CheckStatus");
                    }
                }

            </script>
        <div class="limiter">
            <div class="container-login100">
                <div class="wrap-login100_person">
                    <div class="validate-form p-l-35 p-r-35 p-t-17">
                        <div id="contendor_principal" style="">
                            <div id="div_logo_pqrs">
                                <div class="row">
                                    <div class="col-6">
                                        <h5 style="color: #57b846">CONSULTA RADICADO</h5>     
                                    </div>
                                    <div class="col-6 ">
                                        <a href="javascript:void(0)" title="Atrás" class="float-right" onclick="activa_retroceso_principal();">
                                            <i style="color: #57b846" class="far fa-arrow-left fa-2x float-left"> </i> 
                                        </a>
                                    </div>
                                </div> 
                            </div>
                            <hr />
                            <div id="Contentizquierdo" style="width: 100%; height: auto; position: relative">
                                <div id="contenido_titulo_controles_consulta" style="height: 5%; width: 100%">
                                    <p style="font-size: 14px; font-family: Arial; font-weight: bold; margin: 0px">Estimado Usuario:</p>
                                    <p style="text-align: justify; font-size: 14px; font-family: Arial; margin: 0px">Con el fin de brindarle una mejor atención, a continuación puede consultar el estado de sus radicados, respuestas, transaciones, detalles de la respuesta y los documentos relacionados.</p>
                                </div>
                                <hr/>
                                <div id="contenido_controles_consulta" style="width: 99%; height: auto">
                                    <asp:UpdatePanel ID="UpdatePanelContenido_val_radicacion" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <asp:Panel ID="_Panelvalidacion_val_radicacion" runat="server" Height="100%" Width="100%" Wrap="false" DefaultButton="Button_consulta_val_radicacion">
                                                <div class="row w-100 mt-2">
                                                    <div class="col-6">
                                                        <span>Plantilla radicacion</span>
                                                    </div>
                                                    <div class="col-6">
                                                        <asp:DropDownList ID="DropDownList_Pantilla" runat="server" CssClass="custom-select"></asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="row w-100 mt-1">
                                                    <div class="col-6">
                                                        <span>Consecutivo radicado</span>
                                                    </div>
                                                    <div class="col-6">
                                                        <asp:TextBox ID="TextBox_radicado" runat="server" Style="width: auto; max-width: 167px" CssClass="form-control"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="row w-100 mt-1">
                                                    <div class="col-6">
                                                    </div>
                                                    <div class="col-6">
                                                        <asp:UpdatePanel ID="UpdatePanel_botones_validacion" runat="server" UpdateMode="Conditional">
                                                            <ContentTemplate>
                                                                <input id="Hidden_resultado_consulta" type="hidden" value="" runat="server"/>
                                                                <a class="btn btn-success" style="margin-left: 10px; margin: 5px; width: 100px; float: right" title="Consultar radicado" href="#" onclick="activa_boton_client_server('Button_consulta_val_radicacion')"> Aceptar </a>
                                                                <asp:Button ID="Button_consulta_val_radicacion" runat="server" Text="Consultar" Width="0px" EnableTheming="True" Style="display: none" />
                                                                <asp:Button ID="Button_lipiar_val_radicacion" Text="Limpiar" runat="server" Width="0px" ToolTip="Limpiar campos radicacion" CssClass="boton" Style="display: none" />
                                                            </ContentTemplate>
                                                        </asp:UpdatePanel>
                                                    </div>
                                                </div>

                                            </asp:Panel>
                                        </ContentTemplate>

                                    </asp:UpdatePanel>

                                </div>

                            </div>
                            <div id="Contenedorderecho" style="width: 100%; position: inherit; left: auto; height: 99.5%">
                                <div id="contenido_datagrid_val_radicacion" style="height: auto; width: 100%; position: relative;">
                                    <asp:UpdatePanel ID="UpdatePanel_conenido_grid_val_radicacion" runat="server" UpdateMode="Conditional" RenderMode="Block" style="width: 100%; height: 100%">
                                        <ContentTemplate>
                                            <asp:Panel ID="Panel_principal" runat="server" ScrollBars="Auto"
                                                Width="100%" Style="">
                                                <asp:GridView ID="GridView_val_radicacion" runat="server" Width="100%" EnableViewState="true"
                                                    AutoGenerateSelectButton="False" AllowPaging="true" PageSize="4" Font-Size="14px"
                                                    PagerSettings-Position="Top" AllowSorting="true" CssClass="table font-weight-light" GridLines="None">
                                                    <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                                    <HeaderStyle CssClass="GridviewScrollHeader_line_boot" BorderStyle="None" />
                                                    <PagerStyle CssClass="pagination-ys" />
                                                    <Columns>
                                                        <asp:BoundField HeaderText="OPCIONES   " />
                                                    </Columns>
                                                </asp:GridView>
                                            </asp:Panel>
                                        </ContentTemplate>
                                        <Triggers>
                                        </Triggers>
                                    </asp:UpdatePanel>
                                </div>
                                <div id="contenido_titulo_val_radicacion" style="height: 5%; width: 100%" class="mt-2 mb-2">
                                    <div style="width: auto" class="expant_texto_recort">
                                        <asp:UpdatePanel ID="UpdatePanelabel_val_radicacion" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <input id="hdnEmailID_VAL" type="hidden" value="-1" runat="server"/>
                                                <input id="Hidden_consecutivo_radicado" type="hidden" value="-1" runat="server"/>
                                                <asp:Label ID="titulo_label_val_radicacion" runat="server" Style="font-family: Arial; font-size: 12px; margin-left: 5px">Resultados busqueda</asp:Label>
                                                &nbsp 
                            <asp:Label ID="Label_estado_transac" runat="server" Text="" Style="font-size: 8px; font-family: Arial"></asp:Label>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>

                                </div>
                                <div id="Contenido_botones_tipo_radicado" style="height: 10%; width: 100%; background-color: #E7EDF5; display: none">
                                    <div id="contennido_buton" style="width: 100%; height: 90%">
                                        <asp:UpdatePanel ID="UpdatePanel_botones_radicacion" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                &nbsp
                        <asp:Button ID="Button_Trazabilidad" runat="server" Text="Estados del radicado" Width="140px" ToolTip="Estados del radicado" CssClass="boton" Style="font-family: arial; font-size: 10px" />
                                                &nbsp 
                         <asp:Button ID="Button_Log_respuesta" runat="server" Text="Transacciones de la respuesta" Width="160px" ToolTip="Transacciones realizadas para la respuesta" CssClass="boton" Style="font-family: arial; font-size: 10px" />
                                                &nbsp 
                        <asp:Button ID="Button_Exportar_Radicados" Text="Exporta lista" runat="server" Width="90px" ToolTip="Exportar lista" OnClientClick="retorna_colum_mtriz('Hidden_colum_header');" Style="font-family: arial; font-size: 10px" CssClass="boton" />
                                                &nbsp
                             
                        <asp:Button ID="Button_detalle_radicado" Text="Detalle del radicado" runat="server" Width="120px" ToolTip="Muestra los detalles del radicado" CssClass="boton" Style="font-family: arial; font-size: 10px" />
                                                <input id="Hidden_colum_header" type="hidden" value="" runat="server"/>
                                                <input id="Hidden_ruta_archivo" type="hidden" value="" runat="server"/>
                                                <asp:Button ID="Button_visor_emergente" runat="server" Text="Button" Style="display: none" />
                                            </ContentTemplate>

                                        </asp:UpdatePanel>
                                    </div>


                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!--detalle transacciones-->
           <asp:Panel ID="Panel_transacciones" runat="server" Style="display:none; overflow:hidden; width:70%; height:100%" CssClass="modal_content_general" >
                  <asp:ModalPopupExtender ID="ModalPopupExtender_transacciones" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_transacciones_dos"
                      PopupControlID="Panel_transacciones"  CancelControlID="ButtonSalir_transacciones">
                  </asp:ModalPopupExtender>
                  <div id="modal_content_Panel_transacciones" class="modal-content">  
                  <div id="Cabecerapendiente_transacciones" class="modal_title_superior_ modal-header" >  
                       <h6 class="modal-title d-inline ml-1"></h6>
                       <button type="button" value="ButtonSalir_transacciones" class="close da_event_captive">&times;</button>                      
                  </div>
                  <div id="Cotenedorpendiente_transacciones" style="height: 100%; width: 100%; overflow:hidden; border-top:none" class="modal_content_back modal-body">
                      <asp:UpdatePanel ID="UpdatePanel_transacciones" runat="server" UpdateMode="Conditional">
                          <ContentTemplate>
                              <iframe id="Iframe_transacciones_" runat="server" frameborder="0"  style="width:100%; height:100%; overflow:hidden"></iframe>
                          </ContentTemplate>
                      </asp:UpdatePanel>         
                  </div>
                  </div>
               <div style="display: none; height: 1px">
                   <asp:Button ID="Button_transacciones_dos" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                   <asp:Button ID="ButtonSalir_transacciones" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
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
           <asp:UpdatePanel ID="Updatepanel_actualiza" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Button ID="Button_actualiza_indice" runat="server" Text="Button" Style="display: none" />
                    <input id="Hidden_buton_seleccion_edita_dinamico" type="hidden" value="" runat="server">
                    <input id="Hidden_campos_dinamicos_edita" type="hidden" value="" runat="server">
                    <input id="hidden_campos_dinamicos_aleas" type="hidden" value="" runat="server">
                    <input id="hidden_valore_campos" type="hidden" value="" runat="server" />                  
                </ContentTemplate>
            </asp:UpdatePanel>
            <input id="Hidden_resultado_web_service" type="hidden" value="YES" runat="server">
            <input id="Hidden_alert_respuesta" type="hidden" value="YES" runat="server">
            <input id="Hidden_height" type="hidden" value="0" runat="server">
            <input id="Hidden_width" type="hidden" value="0" runat="server">
          <input id="Hidden_id_tarea_sel" type="hidden" value="-1" runat="server">
           <input id="Hidden_tipo_visor" type="hidden" value="" runat="server">
              
          <!--imagen_respuesta-->
           <asp:Panel ID="Panel_imagen_respuesta" runat="server" Style="display:none; overflow:hidden; width:99%; height:100% "  CssClass="modal_content_general" >
                  <asp:ModalPopupExtender ID="ModalPopupExtender_imagen_respuesta" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_imagen_respuesta_dos"
                      PopupControlID="Panel_imagen_respuesta"  CancelControlID="ButtonSalir_imagen_respuesta">
                  </asp:ModalPopupExtender>
               <div id="modal_content_Panel_imagen_respuesta" class="modal-content">
                   <div id="Cabecerapendiente_imagen_respuesta" class="modal_title_superior_ modal-header" >
                         <h6 class="modal-title d-inline ml-1" id="Label10">Visor de documentos</h6>
                         <button type="button" value="ButtonSalir_imagen_respuesta" class="close da_event_captive">&times;</button>   
                   </div>
                   <div id="Cotenedorpendiente_imagen_respuesta" style="height: 100%; width: 100%; overflow: hidden; border-top:none" class="modal_content_back modal-body">
                       <asp:UpdatePanel ID="UpdatePanel_imagen_respuesta" runat="server" UpdateMode="Conditional">
                           <ContentTemplate>
                               <iframe id="Iframe_imagen_respuesta_" runat="server" frameborder="0" style="width: 100%; height: 100%; overflow: hidden"></iframe>
                           </ContentTemplate>
                       </asp:UpdatePanel>
                   </div>
                   <div style="display: none; height: 1px">
                       <asp:Button ID="Button_imagen_respuesta_dos" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                       <asp:Button ID="ButtonSalir_imagen_respuesta" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                   </div>        
               </div>
              </asp:Panel>
          <!--detalle trazabilidad-->
           <asp:Panel ID="Panel_trazabilidad" runat="server" Style="display:none; overflow:hidden; width:70%; height:100%"  CssClass="modal_content_general" >
                  <asp:ModalPopupExtender ID="ModalPopupExtender_trazabilidad" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_trazabilidad_dos"
                      PopupControlID="Panel_trazabilidad"  CancelControlID="ButtonSalir_trazabilidad">
                  </asp:ModalPopupExtender>
                  <div id="modal_content_Panel_trazabilidad" class="modal-content">  
                  <div id="Cabecerapendiente_trazabilidad" class="modal_title_superior_ modal-header"> 
                       <h6 class="modal-title d-inline ml-1">Ventana de trazabilidad</h6>
                       <button type="button" value="ButtonSalir_trazabilidad" class="close da_event_captive">&times;</button>                      
                  </div>
                  <div id="Cotenedorpendiente_trazabilidad" style="height: 100%; width: 100%; border-top:none" class="modal_content_back modal-body">          
                      <asp:UpdatePanel ID="UpdatePanel_trazabilidad" runat="server" UpdateMode="Conditional">
                          <ContentTemplate>
                              <iframe id="Iframe_trazabilidad_" runat="server" frameborder="0"  style="width:100%; height:100%; overflow:hidden"></iframe>
                          </ContentTemplate>
                      </asp:UpdatePanel>              
                  </div>
                      <div style="display: none; height: 1px">
                          <asp:Button ID="Button_trazabilidad_dos" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                          <asp:Button ID="ButtonSalir_trazabilidad" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
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
</body>
</html>
