<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormRadicacionEntranteInterna.aspx.vb"  EnableEventValidation="false" Inherits="GestionDocumental_Docuarchi.net.WebFormRadicacionEntranteInterna" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
     <script src="../js/ui/jquery-3.4.1.min.js"></script>
    <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <link href="../tokenzize2/tokenize2.min.css" rel="stylesheet" />
    <script src="../tokenzize2/tokenize2.1.min.js"></script>
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
     <script src="../js/ScrollableGridPlugin.js"></script>
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
    <script src="../Fixed-Header-Table-master/gridviewScroll.min.js"></script>
    <script src="../js/radicacion/WebFormRadicacionEntranteInterna.js"></script>
    <script src="../generic_control/FileUploadHandler.js" type="text/javascript"></script>
    <link href="../generic_control/UploadFile.css" rel="stylesheet" />
    <script src="../js/java_general/JSProgresBar.js"></script>
    <script src="../js/java_general/JSReplaceScanFile.js"></script>
    <link rel="stylesheet"href="https://cdn.jsdelivr.net/npm/bootstraptable@1.23.1/dist/bootstrap-table.min.css"/>
    <script src="https://cdn.jsdelivr.net/npm/tableexport.jquery.plugin@1.29.0/tableExport.min.js" type="text/javascript"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap-table@1.23.1/dist/bootstrap-table.min.js" type="text/javascript"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap-table@1.23.1/dist/bootstrap-table-locale-all.min.js" type="text/javascript"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap-table@1.23.1/dist/extensions/export/bootstrap-table-export.min.js" type="text/javascript"></script>
    <script src="../js/versiondocumento/gestion_version_documento.js"></script>
    <script src="../js/table_boo/table_boot_config.js" type="text/javascript"></script>
    <script src="../js/java_general/BootstrapTable.js" type="text/javascript"></script>
    <script src="../js/MyJavaScriptFile.js"></script> 
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
   <script src="../js/Filtrar.js"></script>
      <script src="../js/ui/jquery-3.4.1.min.js"></script>
    <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <link href="../tokenzize2/tokenize2.min.css" rel="stylesheet" />
    <script src="../tokenzize2/tokenize2.1.min.js"></script>
    <script  src="../Awesome/js/all.js"></script>
    <link href="../Awesome/css/fontawesome.css" rel="stylesheet"/>
    <link href="../Awesome/css/brands.css" rel="stylesheet"/>
    <link href="../Awesome/css/solid.css" rel="stylesheet"/>
    <script  src="../Awesome/js/brands.js"></script>
    <script  src="../Awesome/js/solid.js"></script>
    <script  src="../Awesome/js/fontawesome.js"></script>
    
</head>
<body>
   
    <form id="form1" runat="server">
    <div>
     
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
                elment_postbak = args.get_postBackElement();
                if (args.get_postBackElement().id == 'Button_consulta_val_radicacion') {
                    document.getElementById("Button_consulta_val_radicacion").disabled = true;
                    document.getElementById("Button_consulta_val_radicacion").value = "Espere...."
                }
                if (args.get_postBackElement().id == "Buttonradicar_entrante") {
                    document.getElementById("Buttonradicar_entrante").disabled = true;
                    document.getElementById("Buttonradicar_entrante").value = "Espere...."
                }
                posicion_update_pogres('progres_bar');
            }
            function CheckStatus(sender, args) {
                progres_hiden('progres_bar');
                if (elment_postbak.id == 'Button_consulta_val_radicacion') {
                    document.getElementById("Button_consulta_val_radicacion").disabled = false;
                    document.getElementById("Button_consulta_val_radicacion").value = "Consultar"
                    if (document.getElementById("Hidden_resultado_consulta_previa").value == "YES") {
                        plugin_grwedview();
                        document.getElementById("Hidden_resultado_consulta_previa").value = "";
                    }

                }
                if (elment_postbak.id == "Buttonradicar_entrante") {
                    document.getElementById("Buttonradicar_entrante").disabled = false;
                    document.getElementById("Buttonradicar_entrante").value = "Radicar"
                    if (document.getElementById("hiden000001").value == "YES") {
                        if (window.parent.document.getElementById("ButtonSalir_visor_externo")) {
                            window.parent.document.getElementById("ButtonSalir_visor_externo").click();
                            document.getElementById("hiden000001").value = "";
                        }
                    }
                }
                if (elment_postbak.id == "Button_Asignar_radicado_relacionado" || elment_postbak.id == "Button_Asignar_relacionado_expediente" ||
                    elment_postbak.id == "Button_Asignar_nuevo_radicado") {
                    if (document.getElementById("Hidden_resultado_asignacion_radicado").value == "YES") {
                        hiden_popup_resize_popup_validacion_radicados();
                        document.getElementById("Hidden_resultado_asignacion_radicado").value = ""
                        return true;
                    }

                }

                if (elment_postbak.id == "Button_Asignar_radicado_relacionado" || elment_postbak.id == "Button_Asignar_relacionado_expediente" ||
                    elment_postbak.id == "Button_Asignar_nuevo_radicado" || elment_postbak.id == "Button_lipiar_val_radicacion" ||
                    elment_postbak.id == "Button_consulta_val_radicacion" || elment_postbak.id == "Buttonvalidar_radciado") {
                    auto_zise_popup_validacion_radicados();
                    //mueve_scroll_data_gred('GridView_val_radicacion', 'hdnEmailID_VAL');

                }

            }

        </script>
       <input id="Hiddenheigpaginapopup" type="hidden" value="475" runat="server"/>
        <input id="Hiddennameasigna" type="hidden" value="RADICACION_ENTRANTE" runat="server"/> 
        <input id="Hiddentramiteseleccionvalue" type="hidden" value="RADICACION_ENTRANTE" runat="server"/> 
        <input id="Hidden_radi_inter" type="hidden" value="" runat="server"/> 
        <input id="Hidden_nom_flu" type="hidden" value="" runat="server"/> 
        <input id="Hidden_id_flu" type="hidden" value="0" runat="server"/> 
        <div id="contenguia" class="contenguia container-fluid" style="width:100%; float:left"" >
            <asp:Panel ID="PanelTitulo"   ForeColor="Black" runat="server" ScrollBars="None" EnableViewState="true" style=" font-family:Arial; font-size:14px; font-weight:600; margin-right:3px; background-color:#6d7fcc" CssClass=" modal-header ml-1 mt-1" >                
                <asp:Table ID="TableTitle" CssClass="mt-2 mb-2" runat="server" ForeColor="#E7EDF5" ViewStateMode="Enabled"   >
                   
                </asp:Table>

            </asp:Panel>
            <div id="separator_control_2" style="width: 100%; height: 1px; background-color: white"></div>
            <asp:Panel ID="Panel_modo_radicado" BorderStyle="Solid" BackColor="#E7EDF5" ForeColor="White" runat="server"  ScrollBars="Auto" EnableViewState="true" style="height:10%; margin-left:5px; display:none" >
                <asp:UpdatePanel ID="UpdatePanel_modo_radicado" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                         <input id="Hiddenid_expediente" type="hidden" value="0" runat="server"/>
                        <asp:MutuallyExclusiveCheckBoxExtender ID="MutuallyExclusive_anexo_radicado" runat="server" TargetControlID="Check_anexo_radicado"
                            Key="radicado"></asp:MutuallyExclusiveCheckBoxExtender>
                        <asp:mutuallyexclusivecheckboxextender id="MutuallyExclusive_relacionado_radicado" runat="server" targetcontrolid="CheckBox_relacionado_radicado"
                            key="radicado"></asp:mutuallyexclusivecheckboxextender>
                        <asp:MutuallyExclusiveCheckBoxExtender ID="MutuallyExclusive_nuevo_radicado" runat="server" TargetControlID="check_nuevo_radicado"
                            Key="radicado"></asp:MutuallyExclusiveCheckBoxExtender>
                        <asp:CheckBox ID="Check_anexo_radicado" runat="server" Text="RADICAR COMO ANEXO" Checked="false" ForeColor="Red" Font-Size="10" Font-Names="Arial" Font-Bold="true" />
                        &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp  
                        <asp:CheckBox ID="CheckBox_relacionado_radicado" runat="server" Text="RADICAR COMO RELACIONADO" Checked="false" ForeColor="Red" Font-Size="10" Font-Names="Arial" />
                        &nbsp &nbsp  &nbsp &nbsp &nbsp  &nbsp &nbsp &nbsp
                        <asp:CheckBox ID="check_nuevo_radicado" runat="server" Text="NUEVO RADICADO" Checked="true" ForeColor="Red" Font-Size="10" Font-Names="Arial" />
                        <br />
                        
                        <asp:label ID="Label_expeidente_val_radicacion" runat="server" Text="Expediente" ForeColor="Black" Font-Size="9" Font-Names="Arial"></asp:label>
                        <asp:TextBox ID="Textbox_expediente_val_radicacion" runat="server" disabled="disabled" ></asp:TextBox>
                        &nbsp 
                         <asp:Button ID="Button_Eliminar_Expediente" runat="server" Text="X" ToolTip="Eliminar expediente seleccionado" CssClass="boton" />
                        &nbsp
                         <asp:Button ID="Button_Edit_Expediente" runat="server" Text="S" ToolTip="Seleccionar expediente" CssClass="boton"  OnClientClick="tamano_ventana_expediente();" />
                        &nbsp
                        <asp:label ID="Label_radicados_sel_val_radicacion" runat="server" style="position:relative" Text="Radicados relacionados" ForeColor="Black" Font-Size="9" Font-Names="Arial"></asp:label>
                        <asp:DropDownList id="Dropdowlis_sel_val_radciacion" runat="server" Width="200" ></asp:DropDownList>
                         &nbsp 
                         <asp:Button ID="Button_Eliminar_Rel_Radicados" runat="server" Text="X" ToolTip="Eliminar radicado enlazado" CssClass="boton" />
                          &nbsp
                         <asp:Button ID="Buttonvalidar_radciado" Text="Consulta previa radicados " runat="server" Width="200px" ToolTip="Consulta radicados y asigna  tipo radicado" CssClass="boton"  />
                        <div id="separ_ident_boton" style="height:20px"></div>
                        <div id="cierra_popup_expediente" style="display:none">
                            <asp:Button ID="Button_cierra_popup_expediente" runat="server" Text="Cierrapopup" />
                        </div>
                        
                    </ContentTemplate>
                </asp:UpdatePanel>
                
            </asp:Panel> 
                 
                <asp:UpdatePanel ID="UpdatePanel_user_radica" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                          <div class="row tokens-container  ml-0 mr-0 border_superior_inferior_radius_blanco">
                            <div class="col-4 mt-2 mb-2">
                                <span>Usuario que radica</span>
                            </div>
                            <div class="col-8 mt-2 mb-2">
                                <asp:DropDownList ID="RE_DropDownList_user_radica"   AutoPostBack="true"  CssClass="custom-select  w-100"   runat="server"></asp:DropDownList>
                            </div>
                        </div>
                    </ContentTemplate>
                  </asp:UpdatePanel>
                <asp:UpdatePanel ID="UpdatePnaelcontrolesradicacion" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Panel ID="PanelRadicacion" runat="server" ScrollBars="Both" Style="height: 74%; overflow: auto; margin-left: 5px; margin-right: 3px"
                            EnableViewState="true">
                            <asp:Table ID="TableControles" runat="server" ForeColor="Black" BackColor="White" ViewStateMode="Enabled" class="tokens-container form-control" Style="height: auto; width: 100%; overflow-x: auto; margin-right: 5px">
                            </asp:Table>
                            <asp:Table ID="Tableseparacion" runat="server" ForeColor="Black" BackColor="White" ViewStateMode="Enabled">
                            </asp:Table>
                            <asp:Table ID="Tableseparador_documento" runat="server" ForeColor="White" BackColor="White" ViewStateMode="Enabled">
                            </asp:Table>
                            <asp:Table ID="tablecontrolesdinamicos" runat="server" ForeColor="White" BackColor="White" ViewStateMode="Enabled" CssClass="tokens-container form-control" Style="color: Black; background-color: White; height: auto; width: 100%; overflow-x: auto; margin-right: 5px">
                            </asp:Table>
                            <input id="Hiddenareagestion" type="hidden" value="" runat="server" />
                            <input id="Hiddendestinatario" type="hidden" value="" runat="server" />
                        </asp:Panel>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="asignar" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="Buttontramitevence" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="Button_llena_wf_flujo" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="Button_llena_actividad_flujo" EventName="Click" />
                    </Triggers>

                </asp:UpdatePanel>
           
              <asp:Table ID="Tableremitente" runat="server" ForeColor="Black" BackColor="White" ViewStateMode="Enabled" class = "tokens-container  mr-3 ml-1 mt-2" Style="height:auto; width:100%; overflow-x:auto; margin-right:5px; display:none">
                            </asp:Table>
            <asp:Panel ID="Panelbotonesradcacion" runat="server" ScrollBars="Auto" style="height:10%; margin-left:5px; text-align:right"
                EnableViewState="true" >
                <asp:UpdatePanel ID="UpdatePanelradciacionbotones" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <input id="hiden000001" type="hidden" value="" runat="server">
                        <input id="Hidden_005_sel_dest" type="hidden" value="0" runat="server">
                        <input id="Hidden_ruta_archivo" type="hidden" value="" runat="server">
                        <asp:Button ID="Buttonradicar_entrante" runat="server" Style="left: 0px; margin-top: 3px; width:100px; margin-right:3px" Text="Radicar" ToolTip="Radicar documento" CssClass="btn btn-success" />
                        <asp:Button ID="Buttonlimpiar_entrante" Text="Limpiar" runat="server" Style="top: 3px; left: 0px; margin-top: 3px; width:100px; margin-right:3px; display:none" ToolTip="Limpiar campos radicacion" CssClass="btn btn-primary" />
                        <asp:Label ID="Label_estado_transac" runat="server" Text="" Style="font-size: 8px; font-family: Arial; float: right"></asp:Label>
                    </ContentTemplate>
                </asp:UpdatePanel>
                
            </asp:Panel>
                 <asp:UpdatePanel ID="UpdatePanelbotonesradicado" runat="server" UpdateMode="Conditional">
                     <ContentTemplate>
                        <div id="butonverdestinario" style="display: none">
                             <asp:Button ID="Button_ra_destinatario" runat="server" />
                             <asp:Button ID="Buttonllenardestinatario" runat="server" />
                             <asp:Button ID="Buttontramitevence" runat="server" />
                             <asp:Button ID="Button_llena_wf_flujo" runat="server" />
                            <asp:Button ID="Button_llena_actividad_flujo" runat="server" />
                         </div>
                     </ContentTemplate>
                     </asp:UpdatePanel>
                <div id="butonloftfocus" style="display:none">
                    <asp:Button ID="focusremitente" runat="server" />
                   <asp:Button ID="Buttonrefasignar" runat="server"  />

                </div>
             
            <input id="HiddenPROMP" type="hidden" value="1" runat="server">
            <input id="HiddenPlantilla" type="hidden" value="" runat="server">           
            <input id="Hiddenscript" type="hidden" value="" runat="server">
            <input id="Hidden_Estado_opcion_fecha" type="hidden" value="0" runat="server">
            <input id="Hidden_Estado_opcion_cita_respuesta" type="hidden" value="0" runat="server">
            <input id="Hidden_Estado_opcion_radicado_general" type="hidden" value="0" runat="server">
            <input id="Hiddentramiteseleccion" type="hidden" value="" runat="server">
            <input id="Hidden_id_activividad" type="hidden" value="" runat="server">
            <input id="Hidden_id_flujo" type="hidden" value="" runat="server">
            <input id="Hidden_id_user_wf" type="hidden" value="" runat="server">
           
            <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 200px">
                <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
                Processing ...
            </div>
            
        </div>
    </div>
        <div id="cler" style="clear: both"></div>
        <!--POPUP DE VALIDACION DE RADICADOS-->
        <div id="Validaradicacion">
            <asp:Panel ID="Panel_Val_Radicacion" runat="server"   Style="display:none; color: White; width: 100%; height: 100%">
                <asp:ModalPopupExtender ID="ModalPopupExtender_Val_Radicado" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_Abrir_Val_Radicacion"
                    PopupControlID="Panel_Val_Radicacion" CancelControlID="Buttoncacerrar_Val_Radicacion"></asp:ModalPopupExtender>
                     <div id="divcabecera_val_radicacion" class="cabecera2">
                         <asp:Button ID="Button1_val_radicacion" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                         <asp:Button ID="Button_Abrir_Val_Radicacion" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                         <asp:Label ID="Label_val_radicacion" runat="server" Text="Busqueda Radicados" Font-Size="10" Style="float: left"> </asp:Label>
                         <div id="Divcerrarbuton_Val_Radicacion" style="float: right">
                             <asp:Button ID="Buttoncacerrar_Val_Radicacion" runat="Server" Text="X"
                                 ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" OnClientClick="hiden_popup_resize_popup_validacion_radicados();" />

                         </div>
                     </div>
                <div id="Diupdate_val_radciacion" style="border: thin double #000080;  height: 100%; width: 100%; background-color:white; display:none">
                    
                        <div id="contenido_izquierdo_val_radicacion" style="width: 30%;  height: 100%; float:left">
                            <div id="contenido_titulo_campos_consulta" style="height: 5%; width: 100%; background-color: #E7EDF5">
                                <asp:Label ID="_titulo_campos_consulta" runat="server" ForeColor="Black" Font-Size="12" Font-Names="Arial">Campos de busqueda</asp:Label>
                            </div>

                            <div id="contenido_consulta_val_radicacion" style="height: 80%; width: 100%">
                                <asp:UpdatePanel ID="UpdatePanelContenido_val_radicacion" runat="server" UpdateMode="Conditional" RenderMode="Inline" >
                                    <ContentTemplate>
                                        <asp:Panel ID="_Panelvalidacion_val_radicacion" runat="server" ScrollBars="Vertical" Height="100%" Width="100%" >
                                            
                                            <asp:Table ID="_ValidacionConsulta_val_radicacion" runat="server" ForeColor="White" BackColor="White" ViewStateMode="Enabled" Wrap="false"  Style="margin-left:10px; margin-top:5px">
                                            </asp:Table>

                                        </asp:Panel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                            <div id="contenido_botones_val_radicacion" style="height: 15%; width: 100%; background-color: #E7EDF5;">
                                <asp:UpdatePanel ID="UpdatePanel_botones_val_radicacion" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                    <ContentTemplate>
                                        <input id="Hidden_resultado_consulta_previa" type="hidden" value="" runat="server">
                                        
                                        &nbsp &nbsp
                                        <asp:Button ID="Button_consulta_val_radicacion" runat="server" Text="Consultar" Width="100px" ToolTip="Consultar radicados" CssClass="boton" Style="margin-top:5px" />
                                        &nbsp &nbsp
                                         <asp:Button ID="Button_lipiar_val_radicacion" Text="Limpiar" runat="server" Width="100px" ToolTip="Limpiar campos radicacion" CssClass="boton" Style="margin-top:5px"  />
                                        
                                    
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                               
                            </div>
                        </div>
                  <div id="contenido_derecho_validacion_radicados" style="width:69%; float:right; height:100%" >
                        <div id="contenido_titulo_val_radicacion" style="height: 5%; width: 100%; background-color: #E7EDF5; ">
                            <asp:UpdatePanel ID="UpdatePanelabel_val_radicacion" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <input id="hdnEmailID_VAL" type="hidden" value="-1" runat="server">
                                    <asp:Label ID="titulo_label_val_radicacion" runat="server" ForeColor="Black" Font-Size="12" Font-Names="Arial" >Resultados busqueda</asp:Label>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                       
                        <div id="contenido_datagrid_val_radicacion" style="height: 70%; width: 100%; position:relative">
                            <asp:UpdatePanel ID="UpdatePanel_conenido_grid_val_radicacion" runat="server" UpdateMode="Conditional" >
                                <ContentTemplate>

                                    <asp:GridView ID="GridView_val_radicacion" runat="server" Width="100%" EnableViewState="false"
                                        AutoGenerateSelectButton="False" AllowPaging="false" Font-Size="12px" PagerSettings-Position="Top" AllowSorting="false" ForeColor="Black">
                                        <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                        <HeaderStyle CssClass="GridviewScrollHeader" />
                                        <RowStyle CssClass="GridviewScrollItem" />
                                        <PagerStyle CssClass="GridviewScrollPager" />
                                        <Columns>
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="chkSelection" runat="server" onclick="inactiva_chek();"  CssClass="jjjjjjjjjjj" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>

                                </ContentTemplate>

                            </asp:UpdatePanel>
                        </div>
                      
                       
                        
                        <div id="Contenido_botones_tipo_radicado" style="height: 10%; width: 100%; background-color: #E7EDF5; float: left; display:block">

                            <asp:UpdatePanel ID="UpdatePanelabel_buton_asignacion_val_radicacion" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    &nbsp &nbsp 
                                         <asp:Button ID="Button_Asignar_nuevo_radicado" runat="server" Style="position: relative; top: 5px; left: 0px;" Text="Copia datos" Width="100px" ToolTip="Copia los datos del radicado seleccionado en la lista para un nuevo radicado" CssClass="boton"  />
                                    &nbsp 
                                         <asp:Button ID="Button_Asignar_relacionado_expediente" Text="Anexo a expediente " runat="server" Style="position: relative; top: 5px; left: 0px;" Width="130px" ToolTip="Anexa la radicación al expediente del radicado seleccionado en la lista" CssClass="boton"  />
                                    &nbsp
                                        <input id="Hidden_selecion_radicado" type="hidden" value="" runat="server">
                                         <asp:Button ID="Button_Asignar_radicado_relacionado" Text="Relacionar" runat="server" Style="position: relative; top: 5px; left: 0px;" Width="100px"
                                             ToolTip="Relaciona el nuevo radicado con los radicados chekeados en la lista" CssClass="boton" OnClientClick="retorna_check_radicados_gred();" />
                                    &nbsp

                                         <asp:CheckBox ID="CheckBox_val_remplaza" runat="server" Style="position: relative" Text="Remplaza" Checked="true" ForeColor="Red" Font-Size="10" Font-Names="Arial" />
                                    &nbsp
                                         <asp:CheckBox ID="CheckBox_val_agrega" runat="server" Style="position: relative" Text="Agrega" Checked="false" ForeColor="Red" Font-Size="10" Font-Names="Arial" />
                                    <asp:MutuallyExclusiveCheckBoxExtender ID="MutuallyExclusiveCheckBoxExtender1" runat="server" TargetControlID="CheckBox_val_remplaza"
                                        Key="radicado_plus"></asp:MutuallyExclusiveCheckBoxExtender>
                                    <asp:MutuallyExclusiveCheckBoxExtender ID="MutuallyExclusiveCheckBoxExtender2" runat="server" TargetControlID="CheckBox_val_agrega"
                                        Key="radicado_plus"></asp:MutuallyExclusiveCheckBoxExtender>
                                     <input id="Hidden_resultado_asignacion_radicado" type="hidden" value="" runat="server">
                                </ContentTemplate>

                            </asp:UpdatePanel>
                              
                        </div>
                      </div>
                    
                </div>
            </asp:Panel>
        </div>
       
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
                                                            <input id="Hiddenselecionpais" type="hidden" runat="server">
                                                            <input id="Hiddenseleciondepartamento" runat="server" value="" type="hidden">
                                                            <input id="Hiddenvalidacion" type="hidden" value="" runat="server"> 
                                                            <input id="Hiddenmunicipio" runat="server" value="" type="hidden">
                                                            <input id="Hiddenestadoedicion" runat="server" value="0" type="hidden">
                                                            <input id="Hiddenrelacionvalidacion" runat="server" type="hidden" value="-1">
                                                             
                                                            
                                                             <input id="Hidden_height" type="hidden" value="0" runat="server">
                                                             <input id="Hidden_width" type="hidden" value="0" runat="server">
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
                                                     
                                                                 <input id="Hiddenrespuesta" type="hidden" value="" runat="server">
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
                        <input id="HiddenIDdestinatario" type="hidden" value="" runat="server">                       
                                       
                        <input id="Hiddensel" type="hidden" value="0" runat="server">
                        <asp:UpdatePanel ID="updatepanel_detinatario_radicacion_exntrante_hiden" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <input id="hdnEmailID" type="hidden" value="-1" runat="server">
                            </ContentTemplate>   
                        </asp:UpdatePanel>
                    </div>

                </div>

            </asp:Panel>

        </div>
        <asp:UpdatePanel ID="UpdatePanel_imp_impresion" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <input id="Hiddendatoradicacion" type="hidden" value="" runat="server">
                <input id="Hiddenruta" type="hidden" value="" runat="server">
            </ContentTemplate>
        </asp:UpdatePanel>
         <div id="ventanaimpreion">     
            <asp:Panel ID="Panelimpresion" runat="server"  Style="display:none; color: White; width: auto; height: auto">
                 <asp:DragPanelExtender ID="DragPanelExtenderimpre" runat="server" TargetControlID="Panelimpresion" />
                 <asp:ModalPopupExtender ID="ModalPopupExtenderimpre" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir"
                     PopupControlID="Panelimpresion" CancelControlID="Buttoncerrarimpre">
                 </asp:ModalPopupExtender>
                 <div id="divcabecer2" class="cabecera2">
                     <asp:Button ID="Button1" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                     <asp:Button ID="ButtonSalir" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                     <asp:Label ID="Label1" runat="server" Text="Menu Impresion" Font-Size="10" Style="float: left">
                     </asp:Label>
                     <div id="Divcerrarbuton2" style="float: right">
                         <asp:Button ID="Buttoncerrarimpre" runat="Server" Text="X"
                             ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />

                     </div>
                   </div>
               
                <asp:UpdatePanel ID="UpdatePaneliframe" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div id="ContenidoImpresion" style="border: thin double #000080; color: black; background-color: #FFFFFF; height: 280px; width: 500px">
                            <iframe width="100%" height="100%" id="ifimpre" runat="server" src="../radicador/WebFormImprimir.aspx" ></iframe>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
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
        <div id="expediente_ventana_popup" style="height: 97%; width: 97% ">
            <asp:Panel ID="Panel_expdiente_popup" runat="server" Style="display: none; color: White; width: 97%; height: 97%">
                <asp:DragPanelExtender ID="DragPanelExtender_expdiente_popup" runat="server" TargetControlID="Panel_expdiente_popup" />
                <asp:ModalPopupExtender ID="ModalPopupExtende_expdiente_popup" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_expdiente_popup"
                    PopupControlID="Panel_expdiente_popup" CancelControlID="Buttoncerrar_expdiente_popup">
                </asp:ModalPopupExtender>
                <div id="divcabecer_expdiente_popup" class="cabecera2" style="width: 97%">
                    <asp:Button ID="Button_expdiente_popup" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_expdiente_popup" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Label ID="Label_expdiente_popup" runat="server" Text="Gestión expedientes" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton_expdiente_popup" style="float: right">
                        <asp:Button ID="Buttoncerrar_expdiente_popup" runat="Server" Text="X"
                            ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="Contenido_expdiente_popup" style="border: thin double #000080; color: black; background-color: #FFFFFF; height: 97%; width: 97%; float: left">
                    <asp:UpdatePanel ID="UpdatePanel_expdiente_popup" runat="server" UpdateMode="Conditional" style="height: 100%" RenderMode="Inline">
                        <ContentTemplate>
                            <iframe id="Iframe_expdiente_popup_" runat="server" style="width: 100%; height: 100%"></iframe>
                        </ContentTemplate>
                    </asp:UpdatePanel>

                </div>

            </asp:Panel>
        </div>
         <!--UPDATEPANEL QUE CONTIENE EL POPUP DE DESTINATARIOS INTERNOS -->
        <div id="ventana_auxiliar_destinatarios_internos_popup">
                    <asp:Panel ID="Panel_auxiliar_destinatarios_internos_popup" runat="server" Style="display:none; color: White; width: 99%; height: 100%" CssClass="border_superior_inferior_radius_blanco">
                        <asp:ModalPopupExtender ID="ModalPopupExtender_auxiliar_destinatarios_internos_popup" runat="Server"  BackgroundCssClass="FondoAplicacion"
                            TargetControlID="Button_abrir_auxiliar_destinatarios_internos_popup" Y="1"
                            PopupControlID="Panel_auxiliar_destinatarios_internos_popup" CancelControlID="Buttoncerrar_auxiliar_destinatarios_internos_popup"></asp:ModalPopupExtender>
                        <div id="divcabecer_auxiliar_destinatarios_internos_popup" class="modal_title_superior" style="width: 99%">
                           
                            <asp:Label ID="Label3" runat="server" Text="Auxiliar destinatarios" Font-Size="10" Style="float: left">
                            </asp:Label>
                            <div id="Divcerrarbuton_auxiliar_destinatarios_internos_popup" style="float: right">
                                <asp:Button ID="Buttoncerrar_auxiliar_destinatarios_internos_popup" runat="Server" Text="X" CssClass="modal_boton_hiden"
                                     ToolTip="Cerrar ventana" />
                            </div>
                        </div>
                        <asp:UpdatePanel ID="UpdatePanel_auxiliar_destinatarios_internos_popup" runat="server" UpdateMode="Conditional" >
                            <ContentTemplate>
                                <div id="Contenido_auxiliar_destinatarios_internos_popup" style="color: black; background-color: #FFFFFF; height: 45%; width: 99.5%;" class="modal_content_back">
                                    <asp:Panel ID="panel_data_grid_auxiliar_destinatarios_internos_popup" runat="server" ScrollBars="none" Style="height: 100%">
                                        <asp:GridView ID="data_grid_auxiliar_lista" runat="server" Width="100%" CssClass="filtrar" EnableViewState="false"
                                            AutoGenerateSelectButton="False" AllowPaging="false" PageSize="10" Font-Size="11px" Font-Names="arial" PagerSettings-Position="Top" AllowSorting="false">
                                            <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                            <HeaderStyle CssClass="GridviewScrollHeader_line_blanco" /> 
                                            <RowStyle CssClass="GridviewScrollItem_line" /> 
                                            <PagerStyle CssClass="GridviewScrollPager_line" /> 
                                        </asp:GridView>
                                    </asp:Panel>
                                    <input id="Hidden_destinatario_interno" type="hidden" value="" runat="server">
                                    <input id="Hidden_auxiliar_id" type="hidden" value="-1" runat="server">
                                </div>
                            </ContentTemplate>
                           
                        </asp:UpdatePanel>                     
                        <div id="contedor_botones_auxiliar_destinatarios_internos_popup" style="height: 30%;  width: 99%; float:left;  color: black; background-color: #FFFFFF">
                            <asp:UpdatePanel ID="UpdatePanel_botones_popup_interno" runat="server" UpdateMode="Conditional" >
                                <ContentTemplate>
                                    
                                    &nbsp   
                                    <input ID="TextBoxcontenidobusqueda" type="text"  style="width:200px; margin-top:3px" onkeypress="consulta_documentos_busqueda_keypres(event, this)" onkeydown="consulta_documentos_busqueda_keycode(event, this)" ></input>
                                    <input ID="Button_consulta_busqueda_auxiliar_destinatarios_internos_popup" value="Filtrar" Style="margin-top:3px" type="button" class="boton_azul" onclick="filtro_gred_destinatarios_internos('Hidden_auxiliar_id', 'data_grid_auxiliar_lista', 'TextBoxcontenidobusqueda', 'CheckboxBusqueda', 'panel_data_grid_auxiliar_destinatarios_internos_popup', 'Contenido_auxiliar_destinatarios_internos_popup', 'panel_data_grid_auxiliar_destinatarios_internos_popup')" />  &nbsp    
                                    <input type='checkbox' id='CheckboxBusqueda' />
                                    <label style="font:100; font-family:Arial; font-size:12px">Buscar sólo palabra completa</label>  &nbsp                                   
                                    <asp:Button ID="Button_asignar_auxiliar_destinatarios_internos_popup" Text="Asignar" runat="server"  CssClass="boton_azul" Style="margin-top:3px" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                         <asp:Button ID="Button_abrir_auxiliar_destinatarios_internos_popup" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px"  OnClientClick="auto_zise_popup_internos()" style="display:none" />
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
         <!--validacion externo-->
          <div id="validacion_plantilla">
            <asp:Panel ID="Panel_valiacion_plantilla" runat="server"  Style="display:none; color: White; width: 100%; height: 100%">
                 <asp:ModalPopupExtender ID="ModalPopupExtender_valiacion_plantilla" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_valiacion_plantilla"
                    PopupControlID="Panel_valiacion_plantilla" CancelControlID="Button_cerrar_validacion_plantilla">
                </asp:ModalPopupExtender>
                <div id="divcabecer2_validacion_plantilla" class="cabecera2">
                    <asp:Button ID="Button5" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_valiacion_plantilla" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Label ID="Label7" runat="server" Text="Gestion externos" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton2_validacion_plantilla" style="float: right">
                        <asp:Button ID="Button_cerrar_validacion_plantilla" runat="Server" Text="X"
                            ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <asp:UpdatePanel ID="UpdatePanel_validacion_plantilla" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div id="Contenido_validacion_plantilla" style="border: thin double #000080; color: black; background-color: #FFFFFF; height: 90%; width: 100%">
                            <iframe width="100%" height="90%" id="Iframe_validacion_plantilla_" runat="server"  ></iframe>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
             </asp:Panel>
        </div>
        <!--popup traza_graficas-->
          <div style="clear: both">
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
        <div id="inferior_bajo_boton" style="width: 0%; height: 0%; background-color: #E7EDF5; display: none">
            <asp:UpdatePanel ID="updatapanel_iframe" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <iframe runat="server" id="ifmExcel_" width="0" height="0" marginheight="0" marginwidth="0"
                        frameborder="0" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </form>
        
</body>

</html>

