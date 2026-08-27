<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormRecuperarTarea.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormRecuperarTarea" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<title></title>
    <script src="../js/ui/jquery-3.4.1.min.js"></script>  
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>   
   <script src="../js/Filtrar.js"></script>
    <script src="../js/java_general/general_code_java.js?v=20260827-compatible-events5"></script>
    <script src="../js/workflow/WebFormRecuperarTarea.js"></script>
    <script src="../js/validate_campos.js"></script> 
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <link href="../Styles/tabccs.css" rel="stylesheet" />
      <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <script defer src="../Awesome/js/all.js"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet">
  <link href="../Awesome/css/brands.css" rel="stylesheet">
  <link href="../Awesome/css/solid.css" rel="stylesheet">
    <script defer src="../Awesome/js/brands.js"></script>
  <script defer src="../Awesome/js/solid.js"></script>
  <script defer src="../Awesome/js/fontawesome.js"></script>
</head>
<body >
    <form id="form1" runat="server">
    
        <asp:ScriptManager ID="ScriptManager1" runat="server"
            EnableScriptGlobalization="True" EnablePageMethods="True" AsyncPostBackTimeout="1200">
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
                    var heg = 0;
                    if ($("#Content").is(":hidden")) {
                        heg = 34;
             
                    } else {      
                    
                        heg = 3;
                    }
               
                    if (elment_postbak.id == "ButtonRecuperarReasignar") {
                        if (document.getElementById("Hidden_resultado_reasigna").value == "YES") {
                            document.getElementById("Hidden_resultado_reasigna").value = "";
                            $('#hdnEmailID', window.parent.document).val(document.getElementById("hdnEmailID").value);
                            recupera_reasigna_tarea();
                        }
                    }
                    if (elment_postbak.id == "ButtonRecuperar") {
                        if (document.getElementById("Hidden_resultado_reasigna").value == "YES") {
                            document.getElementById("Hidden_resultado_reasigna").value = "";
                            $('#hdnEmailID', window.parent.document).val(document.getElementById("hdnEmailID").value);
                            recupera_tarea();
                        }
                    }
                    if (elment_postbak.id == "Button_autoriza_reasignacion") {
                        if (document.getElementById("Hidden_resultado_reasigna_acpetacion").value == "YES") {
                            document.getElementById("Hidden_resultado_reasigna_acpetacion").value = "";
                            recupera_reasigna_tarea();
                        }
                    }    
                    if (elment_postbak.id == "Consultar" || elment_postbak.id == "Consultar_por_actividad" || elment_postbak.id == "Button_consulta_envios_workflow") {
                        dibuja_gred();
                    }
                        
                    if (elment_postbak.type == "button" || elment_postbak.type == "submit") {
                        elment_postbak.value = value_element;
                        elment_postbak.disabled = false;
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
            <input id="Hidden_id_tarea_sel" type="hidden" value="-1" runat="server">
            <input id="Hidden_tipo_visor" type="hidden" value="" runat="server">
            <div id="contenido_general" style="width: 38%; float: left; position: inherit">
                <div id="Content" style="height:auto">
                    <ul class="tab"  style="background-color:white; height:auto">
                        <li><a style="font-family:Arial" href="javascript:void(0)" class="tablinks" onclick="openCity(event, 'Content_consulta')" id="defaultOpen"> <i class="far fa-search"></i> Consulta tareas</a></li>
                        <li><a style="font-family:Arial" href="javascript:void(0)" class="tablinks" onclick="openCity(event, 'contenido_consulta_historico')"> <i class="far fa-search"></i> Tareas terminadas</a></li>
                    </ul>
                </div>
                <div id="Content_consulta" style="border-right: 1px solid rgb(233, 236, 239); border-bottom:none; border-left:none" class="tabcontent">
                    <div id="contenido_titulo_campos" style="overflow:auto;border-radius: 5px 5px 0px 0px; width:100%;  height:10px; margin-top:1px; margin-left:0px; display:none" class="border_superior_radius">
                        <asp:Label ID="Label3" runat="server" Text="Campos de Busqueda" ForeColor="Black" Font-Size="10" Font-Names="arial" style="margin-top:10px; display:none"></asp:Label>
                    </div>
                    <asp:Panel ID="Panel1" runat="server" 
                         Style="width:100%; height:100%; overflow:auto ">
                        <asp:Table ID="TableControles" runat="server" Style="">
                        </asp:Table>
                    </asp:Panel>
                    <div id="contenido_pie_campos" style="background-color: #E7EDF5; height: 5%; display:none">
                        <asp:Label ID="Label5" runat="server" Text="Recuperar" ForeColor="White" Font-Size="10" Font-Names="arial"></asp:Label>
                    </div>
                </div> 
                 <div id="contenido_consulta_historico" class="tabcontent" style="float:right; width:100%; border-right: 1px solid rgb(233, 236, 239); border-bottom:none; border-left:none" >  
                    
                         <div class="modal-header">
                              <h6 class="modal-title d-inline ml-1">Consulta tareas terminadas</h6>
                         </div>
                         <div id="contenido_procesa_usu_rel_solicitud" style=" width: 100%; height: 100%; border-top: none" class="modal_content_back modal-body">
                             <div class="row pb-3">
                              <div class="col-4 p-0">
                                  <asp:TextBox ID="TextBoxFECHA_ENVIO_INI" runat="server" style="width:95%"  onkeypress="return validate_fecha(event,this)" placeholder="0000 00 00"></asp:TextBox>
                                  <asp:CalendarExtender ID="FECHA_ENVIO_INI_CalendarExtender" runat="server" BehaviorID="TextBoxFECHA_ENVIO_INI_CalendarExtender" TargetControlID="TextBoxFECHA_ENVIO_INI" Format='yyyy-MM-dd'                                   PopupButtonID="ImageButton_FECHA_ENVIO_INI" />
                              </div>
                              <div class="col-2 p-0">
                                    <button class="ml-1 btn border-0" id="ImageButton_FECHA_ENVIO_INI" type="button">
                                      <i class="fad fa-calendar-alt fa-1x"></i>
                                    </button>
                              </div>
                              <div class="col-4 p-0">
                                   <asp:TextBox ID="TextBoxFECHA_ENVIO_FIN" runat="server" style="width:95%"  onkeypress="return validate_fecha(event,this)" placeholder="0000 00 00"></asp:TextBox>
                                   <asp:CalendarExtender ID="TextBoxFECHA_ENVIO_FIN_CalendarExtender" runat="server" BehaviorID="TextBoxFECHA_ENVIO_FIN_CalendarExtender" TargetControlID="TextBoxFECHA_ENVIO_FIN"                                                          Format='yyyy-MM-dd' PopupButtonID="ImageButton_FECHA_ENVIO_FIN" />
                              </div>
                              <div class="col-2 p-0">
                                    <button class="ml-1 btn border-0" id="ImageButton_FECHA_ENVIO_FIN" type="button">
                                      <i class="fad fa-calendar-alt fa-1x"></i>
                                  </button>
                              </div>
                         </div>
                             <div class="modal-footer" >
                             <asp:UpdatePanel ID="UpdateGenera_botones_consulta" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                 <ContentTemplate>
                                     <asp:Button ID="Button_consulta_envios_workflow" runat="server" Text="Consultar" CssClass=" btn btn-success" />
                                 </ContentTemplate>
                             </asp:UpdatePanel>
                         </div>
                        </div>
                         
                    
                </div>
            </div>
          
        <div id="ocultaleft"
            style="position: inherit; width: 5px; float: left; height: 20px; background-color: #053061; margin-left: 1px; margin-right: 1px; margin-top: 1px">
        </div> 
          <div id="contenido_titulo_resultado" style=" width: 60% ; float: right" class="p-1">
              <asp:UpdatePanel ID="UpdatePanel_labelresultado" runat="server" UpdateMode="Conditional">
                  <ContentTemplate>
                      <asp:Label ID="Label_resultado" runat="server" Text="Resultado Busqueda"   CssClass="mr-2 font-weight-light" ></asp:Label>
                  </ContentTemplate>
                  </asp:UpdatePanel>
          </div> 
        <div id="Contenedorgrid" style="width: 60%; position: inherit; left: auto; float: right; height: 340px; border-top: 1px solid rgb(233, 236, 239)">
            <asp:UpdatePanel ID="UpdatePanel_hiden" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <input id="hdnEmailID" type="hidden" value="0" runat="server">
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel ID="UpdateGeneral" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Panel ID="Panelactividad" runat="server" Style="overflow: auto"
                        Width="100%">
                        <asp:GridView ID="GridViewlista" runat="server"  EnableViewState="false" GridLines="None"
                            AutoGenerateSelectButton="False" CssClass="filtrar table font-weight-light">
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

        </div>   
        <div id="contenido_botonoes" style="width: 60%; position: inherit; left: auto; float: right; height: 30px; display:none">
            <asp:UpdatePanel ID="Updatepanel_botones" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Button ID="Button_buscar" runat="server" Text="Buscar en lista " Style="margin-top: 15px; margin-left: 5px" CssClass="boton_azul"  />           
                     <asp:Button ID="ButtonRecuperar" runat="server" Text="Recuperar" ToolTip="Recupera la Tarea" Style="margin-top: 5px; margin-left: 5px" CssClass="boton_azul"   />
                    <asp:Button ID="ButtonRecuperarReasignar" runat="server" Text="Asignar" ToolTip="Recupera la tarea y reasigna la respuesta al usuario que la recupera" CssClass="boton" Style="margin-top: 5px; margin-left: 5px; background-color:yellow; display:none "  />
                     <asp:Button ID="Button_visor_emergente" runat="server" Text="Button" style="display:none" />
                    <input id="Hidden_resultado_reasigna" type="hidden" value="" runat="server"> 
                </ContentTemplate>
            </asp:UpdatePanel>
       </div>      
         <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 50px">
            <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
            Processing ...
        </div>   
        <input id="HiddenFiltro" type="hidden" value="" runat="server">
       <!--codigo cuadro de dialogo-->
        <div id="framemensaje">
            <input id="hdnConsult" type="hidden" value="0" runat="server">
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
           <!--autoriza reasignacion-->
          <div id="autoriza_reasignacion_tarea">
            <asp:Panel ID="Panel_autoriza_reasignacion_tarea" runat="server" Style="display:none; color: White; width: 600px; height: 200px" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_autoriza_reasignacion_tarea" runat="server" BehaviorID="Panel_autoriza_reasignacion_tarea" TargetControlID="ButtonSalir_autoriza_reasignacion_tarea" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_autoriza_reasignacion_tarea" PopupControlID="Panel_autoriza_reasignacion_tarea" ></asp:ModalPopupExtender>
                <div id="divcabecer2_autoriza_reasignacion_tarea" class="modal_title_superior">
                    <asp:Label ID="Label_autoriza_reasignacion_tarea" runat="server" Text="Autoriza reasignación" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton2_autoriza_reasignacion_tarea" style="float: right">
                        <asp:Button ID="Button_cerrar_autoriza_reasignacion_tarea" runat="Server" Text="X" CssClass="modal_boton_hiden"
                             ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_autoriza_reasignacion_tarea" style="background-color: white; width: 100%; height: 99%; color: black; background-color: #FFFFFF" class="modal_content_back">
                        <asp:UpdatePanel ID="UpdatePanel_autoriza_reasignacion_tarea" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                               <br />
                                <table style="width: 100%;">
                                   
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label_user_autoriza_reasignacion_tarea" runat="server" Text="Usuario autorizado*" Style="text-align: center; font-family: Arial; font-size: 14px"></asp:Label>
                                        </td>
                                        <td><asp:TextBox ID="TextBox_login_autoriza_reasignacion_tarea" runat="server" Style="width:300px"></asp:TextBox></td>
                                       
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label_dest_autoriza_reasignacion_tarea" runat="server" Text="Contraseña usuario*" Style="text-align: center; font-family: Arial; font-size: 14px"></asp:Label>

                                        </td>
                                        <td>
                                            <asp:TextBox ID="TextBox_pasw_autoriza_reasignacion_tarea" runat="server" Style="width:300px"  TextMode="Password"></asp:TextBox> 
                                           

                                        </td>                           
                                    </tr>
                                    <tr>
                                        <td></td>
                                    </tr>
                                    
                                    <tr>
                                        <td>

                                        </td>
                                        <td style="float:left"><asp:Button ID="Button_autoriza_reasignacion" runat="server" Text="Reasignar" Style="border-color: #b0c4de; height: 30px; width: 200px; height: 25px; text-align: center" CssClass="boton_azul" /> &nbsp &nbsp
                                                         
                                        </td>
                                    </tr>
                                    
                                    
                                </table>
                                  <input id="Hidden_resultado_reasigna_acpetacion" type="hidden" value="" runat="server"> 
                                  <input id="Hidden_usuario_autoriza" type="hidden" value="" runat="server">
                                  <input id="Hidden_usuario_autoriza_id" type="hidden" value="0" runat="server">                       
                            </ContentTemplate>
                        </asp:UpdatePanel>
                         
                </div>
                <asp:Button ID="Button_autoriza_reasignacion_tarea" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" style="display:none" />
                    <asp:Button ID="ButtonSalir_autoriza_reasignacion_tarea" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" style="display:none" />
            </asp:Panel>
        </div>
        <!--Popub busqueda !-->
          <div id="botonbusqueda" style="display: none">
              <asp:Button ID="ButtonActivarBusqueda" runat="server" Text="Button" />
          </div>
          <div id="busquda">
              <asp:Panel ID="Panelbusqueda" runat="server" Style="display:none; color: White; width: auto; height: auto" CssClass="modal_content_general">        
                  <asp:ModalPopupExtender ID="ModalPopupExtenderbusqueda" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Buttond_busqueda"
                      PopupControlID="Panelbusqueda" CancelControlID="Buttoncacerrar">
                  </asp:ModalPopupExtender>
                  <div id="divcabecer" class="modal_title_superior">  
                      <asp:Label ID="Label4" runat="server" Text="Busqueda" Font-Size="10" Style="float: left">
                      </asp:Label>
                      <div id="Divcerrarbuton" style="float: right">
                          <asp:Button ID="Buttoncacerrar" runat="Server" Text="X" CssClass="modal_boton_hiden"
                               ToolTip="Cerrar ventana" />
                      </div>
                  </div>
                  <div id="Diupdate" class="fond_contextual" style="color: White;  height: auto; width: auto" >
                      <div id="Contenidopaginabusqueda"  style="height: 140px; width: 450px; overflow: no-display; color: black; margin-left: 15px">
                          <asp:UpdatePanel ID="UpadatePanel_busqueda" runat="server" UpdateMode="Conditional">
                              <ContentTemplate>
                                  <br />
                                  <label style="font-family: Arial; font-size: 12px; display:none">Busqueda Tareas en la lista </label>
                                  <br />
                                  <label style="font-family: Arial; font-size: 12px">Texto busqueda </label>
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
                  <asp:Button ID="Buttond_busqueda" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" style="display:none"/>
              </asp:Panel>
          </div>
      <!--Popup visor externo-->
        <asp:Panel ID="Panel_visor_externo" runat="server" Style="display: none; overflow: hidden" ForeColor="White" Width="95%" Height="100% " CssClass="modal_content_general">
            <asp:ModalPopupExtender ID="ModalPopupExtender_visor_externo" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_visor_externo"
                PopupControlID="Panel_visor_externo" CancelControlID="ButtonSalir_visor_externo">
            </asp:ModalPopupExtender>
            <div id="modal_content_visor_externo" class="modal-content">
                <div id="Cabecerapendiente_visor_externo" class="modal_title_superior_ modal-header">
                       <h6 class="modal-title d-inline ml-1">Archivar</h6>
                       <button type="button" value="ButtonSalir_visor_externo" class="close da_event_captive">&times;</button>   
                </div>
                <div id="Contenedor_visor_externo" style="height: 100%; width: 100%; overflow: hidden" class="modal_content_back">
                    <asp:UpdatePanel ID="UpdatePanel_visor_externo" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <iframe id="Iframe_visor_externo_" runat="server" frameborder="0" style="width: 100%; height: 100%; overflow: hidden"></iframe>
                        </ContentTemplate>
                    </asp:UpdatePanel>

                </div>
               
            </div>
            <div style="display: none; height: 1px">
                <asp:Button ID="Button_visor_externo" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" Style="display: none" />
                <asp:Button ID="ButtonSalir_visor_externo" runat="Server" Text="" CssClass="invisible" Height="0px" Width="0px" Style="display: none"/>
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
     </form>
     <script>
         function openCity(evt, cityName) {
             var i, tabcontent, tablinks;
             tabcontent = document.getElementsByClassName("tabcontent");
             for (i = 0; i < tabcontent.length; i++) {
                 tabcontent[i].style.display = "none";
             }
             tablinks = document.getElementsByClassName("tablinks");
             for (i = 0; i < tablinks.length; i++) {
                 tablinks[i].className = tablinks[i].className.replace(" active", "");
             }
             document.getElementById(cityName).style.display = "block";
             evt.currentTarget.className += " active";
             //Button_consulta_pqrs_registrados
             if (cityName == "historico_pqr") {
                 //document.getElementById("Button_consulta_pqrs_registrados").click();
             }
         }

         // Get the element with id="defaultOpen" and click on it
         document.getElementById("defaultOpen").click();
    </script>
</body>
</html>
