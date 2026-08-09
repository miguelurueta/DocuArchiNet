<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormRaConsultaGuias.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormRaConsultaGuias" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Consulta guías</title>
    <script src="../js/ui/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
     <script src="../js/ScrollableGridPlugin.js"></script>   
    <script src="../js/ScrollableGridViewPlugin_ASP.NetAJAXmin.js" type="text/javascript"></script>
    <script src="../Fixed-Header-Table-master/gridviewScroll.min.js"></script>
   <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />   
    <script src="../js/radicacion/WebFormRaConsultaGuias.js"></script>
    <script defer src="../Awesome/js/all.js"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet">
  <link href="../Awesome/css/brands.css" rel="stylesheet">
  <link href="../Awesome/css/solid.css" rel="stylesheet">
    <script defer src="../Awesome/js/brands.js"></script>
  <script defer src="../Awesome/js/solid.js"></script>
  <script defer src="../Awesome/js/fontawesome.js"></script>
    <script  accesskey="javascript" type="text/javascript">
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div id="contendor_principal" style="height: 100%; width: 100%">
            <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" EnablePageMethods="true" AsyncPostBackTimeout="900">
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
                    elment_postbak = args.get_postBackElement();

                    posicion_update_pogres('progres_bar');
                    var elmen = document.getElementById(elment_postbak.id)
                    if (elmen.type == "button" || elmen.type == "image" || elmen.type == "submit") {
                        value_element = elmen.value;
                        elmen.value = "Espere..."
                        elmen.disabled = true;
                    }
                }
                function CheckStatus(sender, args) {
                    progres_hiden('progres_bar');
                    //Button_Lista_Radicados Button_actualizar_guia Button_anular_guia
                    if (elment_postbak.id == "Button_procesa_tramite_envio") {
                        if (document.getElementById("Hidden_procesa_tramite_envio").value == "YES") {

                            actualiza_gred_guia_respuesta();

                        }
                    }
                    if (elment_postbak.id == "Button_actualizar_guia") {
                        if (document.getElementById("Hidden_procesa_tramite_envio").value == "YES") {

                            actualiza_gred_guia_respuesta();

                        }
                    }
                    if (elment_postbak.id == "Button_anular_guia") {
                        if (document.getElementById("Hidden_procesa_tramite_envio").value == "YES") {

                            actualiza_gred_guia_respuesta();

                        }
                    }
                    if (elment_postbak.id == "Button_procesar_envio") {
                        elimina_registro_gred_pendiente();
                        document.getElementById("Hidden_alert_respuesta").value = "NO";
                        plugin_grwedview();
                    }
                    //mueve_scroll_data_gred('GridView_val_radicacion', 'hdnEmailID_VAL');  Button_consulta_pendientes_procesar
                    if (elment_postbak.id == "Button_consulta_pendientes_procesar") {
                        plugin_grwedview();
                    }
                    if (elment_postbak.id == "Button_Lista_Radicados") {
                        fnExcelReport();
                    }
                    if (elment_postbak.id == "Button_asigna_guia") {
                        auto_size_popup_procesa_tramite();
                    }
                    var elmen = document.getElementById(elment_postbak.id)
                    if (elmen.type == "button" || elmen.type == "image" || elmen.type == "submit") {
                        elmen.disabled = false;
                        elmen.value = value_element;
                    }
                }

            </script>
             <input id="Hidden_resultado_web_service" type="hidden" value="YES" runat="server">
            <input id="Hidden_alert_respuesta" type="hidden" value="YES" runat="server">
            <INPUT id="hdnEmailID_sel" type="hidden" value="0" runat="server" >
            <div id="Contenedorderecho" style="width: 100%; position: inherit; left: auto; height: 99.5%; float:right">
                <div id="contenido_titulo_val_radicacion" style="height: 5%; width: 75%; float:right; overflow:auto" class="border_superior_radius">
                    <asp:UpdatePanel ID="UpdatePanelabel_val_radicacion" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <input id="hdnEmailID_VAL" type="hidden" value="-1" runat="server">
                            <input id="Hidden_consecutivo_radicado" type="hidden" value="-1" runat="server">
                            <asp:Label ID="titulo_label_val_radicacion" runat="server" ForeColor="Black" Style="float: left; margin-top:3px; margin-left:5px; font-weight:600; text-overflow:ellipsis; font-family:Arial; font-size:13px ">Guías resultantes</asp:Label>                        
                            &nbsp <asp:CheckBox ID="CheckBox_busqueda" runat="server" Text="Sólo palabras completas" Font-Names="arial" Font-Size="6" Style="float: right" />
                            &nbsp <asp:TextBox ID="TextBox_busqueda" runat="server" Width="130px" Style="float:right"></asp:TextBox>
                            <a  style="margin-right:5px; float:right; color:black; height:35px; margin-top:5px" title="Buscar" href="#"  onclick="activa_boton_client_server('Button_buscar_lista')"><i class="fas fa-search fa-1x" ></i></a>
                            &nbsp <asp:Button ID="Button_buscar_lista" runat="server" Width="65px" Text="Buscar" Style="float: right; display:none" OnClientClick="busqueda_gred_por_enviar('hdnEmailID_VAL','GridView_val_radicacion','TextBox_busqueda','CheckBox_busqueda');" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                 <div id="Contentizquierdo" style="width: 24%; height: 99.5%; float: left; position: relative">
                    <div id="contenido_titulo_controles_consulta" style="height: 5%; width: 100%; text-overflow:ellipsis; white-space: nowrap;overflow:hidden" class="border_superior_radius">
                        <asp:Label ID="Label7" runat="server" style="margin-left:5px; font-weight:600; text-overflow:ellipsis; font-family:Arial; font-size:13px">Campos de busqueda</asp:Label>
                    </div>
                    <div id="contenido_controles_consulta" style="width: 100%; height: 75%; border:solid 1px #ccc;">
                    <asp:UpdatePanel ID="UpdatePanelContenido_val_radicacion" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            
                            <asp:Panel ID="_Panelvalidacion_val_radicacion" runat="server" ScrollBars="Vertical" Height="100%" Width="100%" Wrap="false" style="padding-left:1px"  DefaultButton="Button_consulta_pendientes_procesar">
                                <asp:Label ID="Label4" runat="server" Text="Areas/Dependencias" Style="font-size:12px; font-family:Arial"></asp:Label> <br />
                                <asp:DropDownList ID="DropDownList_areas_depart" runat="server" Width="250px"></asp:DropDownList> <br />
                                  <asp:Label ID="Label24" runat="server" Text="Nombre remitente" Style="font-size:12px; font-family:Arial"></asp:Label> <br />
                                <asp:DropDownList ID="DropDownList_nombre_remitente" runat="server" Width="250px"></asp:DropDownList>  <br />
                                 <asp:Label ID="Label1" runat="server" Text="Empresa Envío/currier" Style="font-size:12px; font-family:Arial"></asp:Label> <br />
                                <asp:DropDownList ID="DropDownList_empresa_envio" runat="server" Width="250px"></asp:DropDownList>  <br />
                                <asp:Label ID="Label25" runat="server" Text="Operadores de mensajería interna" Font-Size="10" Font-Names="arial" ForeColor="black" />
                                <br />
                                <asp:DropDownList ID="DropDownList_mensajero_inerno" runat="server" Font-Size="10" Font-Names="arial" Style="width: 250px"></asp:DropDownList>
                                <br />
                                <asp:Label ID="Label23" runat="server" Text="Codigo único de guía" Style="font-size:12px; font-family:Arial"></asp:Label> <br />
                                <asp:TextBox ID="TextBox_Id_guia_envio" runat="server" Width="250px" CssClass="date_2"></asp:TextBox> <br />  
                                <asp:Label ID="Label18" runat="server" Text="Consecutivo guía de envío" Style="font-size:12px; font-family:Arial"></asp:Label> <br />
                                <asp:TextBox ID="TextBox_Concecutivo_Guia" runat="server" Width="250px" CssClass="date_2"></asp:TextBox> <br />  
                                <asp:Label ID="Label5" runat="server" Text="Fecha registro guía" Style="font-size:12px; font-family:Arial"></asp:Label> <br />                          
                                <asp:TextBox ID="TextBox_Fecha_Registro_Guia_ini" runat="server" Width="80px" CssClass="date_2">
                                </asp:TextBox>             
                                <asp:CalendarExtender ID="TextBox_fecha_ini_CalendarExtender" runat="server" TargetControlID="TextBox_Fecha_Registro_Guia_ini"  PopupButtonID="ImageButton_Fecha_Registro_Guia_ini" Format = "yyyy-MM-dd"/>
                                <asp:ImageButton ID="ImageButton_Fecha_Registro_Guia_ini" runat="server"  ImageUrl="../imagera/Calendar.png"/> &nbsp
                                <asp:TextBox ID="TextBox_fecha_Registro_Guia_fin" runat="server" Width="80px" CssClass="date_2"></asp:TextBox>
                                <asp:CalendarExtender ID="TextBox_fecha_fin_CalendarExtender" runat="server" TargetControlID="TextBox_fecha_Registro_Guia_fin" PopupButtonID="ImageButton_Fecha_Registro_Guia_fin" Format = "yyyy-MM-dd" />
                                <asp:ImageButton ID="ImageButton_Fecha_Registro_Guia_fin" runat="server" ImageUrl="../imagera/Calendar.png"/> <br />
                                <asp:Label ID="Label6" runat="server" Text="Fecha envío guía" Style="font-size:12px; font-family:Arial"></asp:Label> <br />
                                <asp:TextBox ID="TextBox_FECHA_ENVIO_GUIA_ini" runat="server" Width="80px" CssClass="date_2">
                                </asp:TextBox>
                                <asp:CalendarExtender ID="TextBox_fecha_resp_ini_CalendarExtender" runat="server" TargetControlID="TextBox_FECHA_ENVIO_GUIA_ini" PopupButtonID="ImageButton_fecha_resp_ini" Format = "yyyy-MM-dd"/>
                                <asp:ImageButton ID="ImageButton_fecha_resp_ini" runat="server"  ImageUrl="../imagera/Calendar.png"/> &nbsp
                                <asp:TextBox ID="TextBox_FECHA_ENVIO_GUIA_fin" runat="server" Width="80px" CssClass="date_2"></asp:TextBox>
                                <asp:CalendarExtender ID="TextBox_fecha_resp_fin_CalendarExtender" runat="server" TargetControlID="TextBox_FECHA_ENVIO_GUIA_fin" PopupButtonID="ImageButton_resp_fin" Format = "yyyy-MM-dd" />
                                <asp:ImageButton ID="ImageButton_resp_fin" runat="server" ImageUrl="../imagera/Calendar.png"/> <abbr></abbr> <br />
                                <asp:Label ID="Label27" runat="server" Text="Fecha Recibido guía" Style="font-size:12px; font-family:Arial"></asp:Label> <br />
                                <asp:TextBox ID="TextBox_RECIBIDO_GUIA_ini" runat="server" Width="80px" CssClass="date_2">
                                </asp:TextBox>
                                <asp:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="TextBox_RECIBIDO_GUIA_ini" PopupButtonID="ImageButton_RECIBIDO_GUIA_ini" Format = "yyyy-MM-dd"/>
                                <asp:ImageButton ID="ImageButton_RECIBIDO_GUIA_ini" runat="server"  ImageUrl="../imagera/Calendar.png"/> &nbsp
                                <asp:TextBox ID="TextBox_FECHA_RECIBIDO_GUIA_fin" runat="server" Width="80px" CssClass="date_2"></asp:TextBox>
                                <asp:CalendarExtender ID="TextBox_RECIBIDO_GUIA_fin_CalendarExtender" runat="server" TargetControlID="TextBox_FECHA_RECIBIDO_GUIA_fin" PopupButtonID="ImageButton_RECIBIDO_GUIA_fin" Format = "yyyy-MM-dd" />
                                <asp:ImageButton ID="ImageButton_RECIBIDO_GUIA_fin" runat="server" ImageUrl="../imagera/Calendar.png"/> <abbr></abbr> <br />
                                <asp:Label ID="Label10" runat="server" Text="Nombre/rsocial destinatario" Style="font-size:12px; font-family:Arial"></asp:Label> <br />
                                <asp:TextBox ID="TextBox_NOMBRE_RAZON_SOCIA" runat="server" Width="250px" CssClass="date_2"></asp:TextBox> <br />
                                <asp:Label ID="Label11" runat="server" Text="Nit/identificacion destinatario" Style="font-size:12px; font-family:Arial"></asp:Label> <br />
                                <asp:TextBox ID="TextBox_NIT_IDENTIFICACION_2" runat="server" Width="250px"></asp:TextBox> <br />
                                <asp:Label ID="Label13" runat="server" Text="Estado confirmación guía" Style="font-size:12px; font-family:Arial"></asp:Label> <br />
                                <asp:DropDownList ID="DropDownList_ESTADO_CONFIRMACION_GUIA" runat="server" Width="250px">
                                    <asp:ListItem></asp:ListItem>
                                    <asp:ListItem>Guías pendientes por enviar</asp:ListItem>
                                    <asp:ListItem>Guías enviadas</asp:ListItem>
                                    <asp:ListItem>Guías archivadas</asp:ListItem>
                                    <asp:ListItem>Devolución Permanente</asp:ListItem>
                                </asp:DropDownList> <br />
                                <asp:Label ID="Label12" runat="server" Text="Estado de la guía" Style="font-size:12px; font-family:Arial"></asp:Label> <br />
                                <asp:DropDownList ID="DropDownList_ESTADO_GUIA" runat="server" Width="250px">
                                    <asp:ListItem></asp:ListItem>
                                    <asp:ListItem>Guías anuladas</asp:ListItem>
                                    <asp:ListItem>Guías activas</asp:ListItem>
                                </asp:DropDownList> <br />
                                <asp:Label ID="Label26" runat="server" Text="Tipo guía" Style="font-size:12px; font-family:Arial"></asp:Label> <br />
                                <asp:DropDownList ID="DropDownList_tipo_guia" runat="server" Width="250px">
                                    <asp:ListItem></asp:ListItem>
                                    <asp:ListItem>Manual</asp:ListItem>
                                    <asp:ListItem>Automatica</asp:ListItem>
                                </asp:DropDownList> <br />
                            </asp:Panel>
                        </ContentTemplate>

                    </asp:UpdatePanel>

                </div>
                     <div id="contenido_controles_buton_consulta" style="width: 100%; height: 20%; background-color: white; border:solid 1px #ccc; overflow:auto">
                        
                         <div id="contenido_botones" style="height: 60%; width: 100%; background-color: white;">
                             <asp:UpdatePanel ID="UpdatePanel_botones_validacion" runat="server" UpdateMode="Conditional">
                                 <ContentTemplate>
                                     <input id="Hidden_resultado_consulta" type="hidden" value="" runat="server">
                                     &nbsp
                        <asp:Button ID="Button_consulta_pendientes_procesar" Text="Consultar" runat="server" Width="70px" Style="margin-top:3px" ToolTip="Actualizar lista" CssClass="boton_azul" />
                                     &nbsp 
                        <asp:Button ID="Button_lipiar_val_radicacion" Text="Limpiar" runat="server" Width="70px" Style="margin-top:3px" ToolTip="Limpiar campos radicacion" CssClass="boton_azul" />

                                 </ContentTemplate>

                             </asp:UpdatePanel>
                         </div>
                        
                     </div>
                </div>
                <div id="contenido_datagrid_val_radicacion" style="height: 60%; width: 75%; position: relative;float:right; border:solid 1px #ccc;">
                    <asp:UpdatePanel ID="UpdatePanel_conenido_grid_val_radicacion" runat="server" UpdateMode="Conditional" RenderMode="Block" style="width: 100%; height: 100%">
                        <ContentTemplate>

                            <asp:GridView ID="GridView_val_radicacion" runat="server" Width="100%" EnableViewState="false" GridLines="None"
                                AutoGenerateSelectButton="False" AllowPaging="false" Font-Size="11px" PagerSettings-Position="Top" AllowSorting="false" >

                                 <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                        <HeaderStyle CssClass="GridviewScrollHeader_line_blanco" />
                                        <RowStyle CssClass="GridviewScrollItem_line" />
                                        <PagerStyle CssClass="GridviewScrollPager_line" />
                                 
                                
                            </asp:GridView>

                        </ContentTemplate>
                        <Triggers>
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
                <div id="Contenido_botones_tipo_radicado" style="height: 10%; width: 75%; background-color:white; float:right; border:solid 1px #ccc; overflow:auto">
                    <div id="superior_alto_boton" style="width: 100%; height: 20%; background-color: white"></div>
                    <asp:UpdatePanel ID="UpdatePanel_botones_radicacion" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                             &nbsp
                            <asp:Button ID="Button_descargar_guia" runat="server" Text="Descargar guía" Width="100px" ToolTip="Gestionar guía envío" CssClass="boton_azul"  />
                            &nbsp 
                       
                            <input id="Button_exportar" type="button" value="Exportar" style="width:70px; " class="boton_azul" onclick="fnExcelReport();" />
                            <input id="Hidden_colum_header" type="hidden" value="" runat="server">
                            <input id="Hidden_ruta_archivo" type="hidden" value="" runat="server">
                            <input id="Hidden_resultado" type="hidden" value="" runat="server">
                            <input id="Hidden_lista_eliminar_tarea" type="hidden" value="0" runat="server">
                                
                        </ContentTemplate>
                        <Triggers>
                            
                        </Triggers>
                    </asp:UpdatePanel>
                    
                    <div id="inferior_bajo_boton" style="width: 100%; height: 20%; background-color: white">
                        <asp:UpdatePanel ID="updatapanel_iframe" runat="server" UpdateMode="Conditional" >
                            <ContentTemplate>
                                
                                <asp:Label ID="Label_result" runat="server" Text="" Style="font-size:8px; font-family:Arial; float:right"></asp:Label>
                                  <iframe runat="server" id="ifmExcel_" width="0" height="0" marginheight="0" marginwidth="0"
                            frameborder="0" />
                                
                            </ContentTemplate>
                           
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
        </div>
        <div id="contenido_documentos">
            <asp:Panel ID="Panel_contenido_documentos" runat="server" Style="display: none; color: White; width: 470px; height: 380px">
                 <div id="divcabecer2_contenido_documentos" class="cabecera2">
                    <asp:Button ID="Button3" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Label ID="Label3" runat="server" Text="Notifica gestión" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton2_contenido_documentos" style="float: right">
                        <asp:Button ID="Button5" runat="Server" Text="X"
                            ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />
                    </div>
                </div>
            </asp:Panel>
        </div>
        <div id="notifica_gestion">
            <asp:Panel ID="Panel_notifica_gestion" runat="server" Style="display:none; color: White; width: 470px; height: 380px">

                <asp:ModalPopupExtender ID="ModalPopupExtender_notifica_gestion" runat="server" BehaviorID="Panel_notifica_gestion_ModalPopupExtender" TargetControlID="ButtonSalir_notifica_gestion"
                    CancelControlID="Button_cerrar_notifica_gestion" PopupControlID="Panel_notifica_gestion">
                </asp:ModalPopupExtender>
                <div id="divcabecer2_notifica_gestion" class="cabecera2">
                    <asp:Button ID="Button1" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_notifica_gestion" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Label ID="Label2" runat="server" Text="Notifica gestión" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton2_notifica_gestion" style="float: right">
                        <asp:Button ID="Button_cerrar_notifica_gestion" runat="Server" Text="X"
                            ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_notifica_gestion" style="background-color:white; width:470px; height: 380px">
                    <asp:UpdatePanel ID="UpdatePaneliframe" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>

                            <iframe Style="color: White; width: 470px; background-color:white; height: 380px; overflow:hidden" id="ifimpre" runat="server"  ></iframe>
                             <input id="Hidden_cuenta_correo_envio" type="hidden" value="" runat="server">
                             <input id="Hidden_correo_envio_default" type="hidden" value="" runat="server">
                             <input id="Hidden_imagen_adjunta" type="hidden" value="" runat="server">
                             <input id="Hidden_asunto_notificacion" type="hidden" value="" runat="server">
                             <input id="Hidden_convierte_pdf" type="hidden" value="" runat="server">
                            <input id="Hidden_tipo_notificacion" type="hidden" value="ENVIO CORESPONDENCIA" runat="server">
                             <input id="Hidden_ruta_tempo" type="hidden" value="" runat="server">
                             <input id="Hidden_id_plantilla_radicado" type="hidden" value="" runat="server">
                        </ContentTemplate>
                    </asp:UpdatePanel>
                   
                </div>
            </asp:Panel>
        </div>
        <div id="procesa_tramite_envio">
            <asp:Panel ID="Panel_procesa_tramite_envio" runat="server" Style="display:none; color: White; width: 550px; height: 100%">
                <asp:ModalPopupExtender ID="ModalPopupExtender_procesa_tramite_envio" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_procesa_tramite_envio"
                    PopupControlID="Panel_procesa_tramite_envio" CancelControlID="Button_cerrar_procesa_tramite_envio">
                </asp:ModalPopupExtender>
                <div id="divcabecer2_edita_tipo_tramite" class="cabecera2">
                    <asp:Button ID="Button2" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_procesa_tramite_envio" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Label ID="Label8" runat="server" Text="Procesa guía de envío" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton2_procesa_tramite_envio" style="float: right">
                        <asp:Button ID="Button_cerrar_procesa_tramite_envio" runat="Server" Text="X"
                            ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />
                    </div>

                </div>
                <asp:UpdatePanel ID="UpdatePanel_procesa_tramite_envio" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div id="contenido_procesa_tramite_envio" style="width: 549px; height: 100%; background-color: white">
                            <div id="contenido_campos_procesa_tramite_envio" style="width: 549px; height: 80%; background-color: white; overflow:auto">
                                <br />
                                &nbsp
                                <asp:Label ID="Label_edita_tipo_tramite" runat="server" Text="Empresa envío trámite" ForeColor="Black" Font-Names="arial" Font-Size="10" Style="" />
                                <br />
                                &nbsp
                                <asp:DropDownList ID="DropDownList_procesa_tramite_envio" runat="server" Font-Size="10" Font-Names="arial"  Style="width: 350px" onChange="on_clik('Button_tipo_empresa_guia');"  ></asp:DropDownList>
                                <br />
                                
                                 &nbsp
                                <asp:Label ID="Label_tramite" runat="server" Text="Tipo de operador" ForeColor="Red" Font-Names="arial" Font-Size="9" width="350px" Style="" />
                                <br />
                                &nbsp
                                <asp:Label ID="Label9" runat="server" Text="Código guía envío (*)" Font-Size="10" Font-Names="arial" ForeColor="black" />
                                <br />
                                &nbsp 
                                <asp:TextBox ID="TextBox_codigo_guia_envio" runat="server" Style=" width: 200px"></asp:TextBox>
                                <asp:AutoCompleteExtender ID="TextBox_codigo_guia_envio_AutoCompleteExtender" runat="server" BehaviorID="TextBox_codigo_guia_envio_AutoCompleteExtender" DelimiterCharacters=""
                                    MinimumPrefixLength="2"
                                    EnableCaching="True"
                                    CompletionSetCount="10"
                                    CompletionInterval="50"
                                    ServiceMethod="GetGuiaRadicaconasp"
                                    ServicePath="../webservice/WebServiceRadicacion.asmx"
                                    ContextKey="GUIA_ENVIO|ra_respuesta_radicado"
                                    UseContextKey="True"
                                    OnClientShown="onDataShown"
                                    CompletionListCssClass="completionList" TargetControlID="TextBox_codigo_guia_envio">
                                </asp:AutoCompleteExtender>
                                <br />
                                
                                &nbsp
                                <asp:Label ID="Label14" runat="server" Text="Operadores de mensajería interna" Font-Size="10" Font-Names="arial" ForeColor="black" />
                                <br />
                                  &nbsp <asp:DropDownList ID="DropDownList_mensajero_interno" runat="server" Font-Size="10" Font-Names="arial" Style="width: 350px"></asp:DropDownList>
                                <br />
                                <br />
                                &nbsp <asp:Label ID="Label21" runat="server" Text="Caracterización destinatario" Font-Size="10" Font-Names="arial" ForeColor="red" />
                                <br />
                                 &nbsp <asp:Label ID="Label15" runat="server" Text="Destinatario Correspondencia" Font-Size="10" Font-Names="arial" ForeColor="black" />
                                <br />
                                &nbsp 
                                <asp:TextBox ID="TextBox_NOMBRE_RAZON_SOCIAL" runat="server" Style=" width: 350px"></asp:TextBox>
                               
                                <br />
                                 &nbsp <asp:Label ID="Label17" runat="server" Text="Direccion Destinatario" Font-Size="10" Font-Names="arial" ForeColor="black" />
                                <br />
                                &nbsp <asp:TextBox ID="TextBox_DIRECCION" runat="server" Style=" width: 350px" TextMode="MultiLine" MaxLength="50"></asp:TextBox>
                                 <br />
                                 &nbsp <asp:Label ID="Label16" runat="server" Text="Nit/identificación Destinatario" Font-Size="10" Font-Names="arial" ForeColor="black" />
                                <br />
                                &nbsp <asp:TextBox ID="TextBox_NIT_IDENTIFICACION" runat="server" Style=" width: 350px"></asp:TextBox>
                                <br />
                                 &nbsp <asp:Label ID="Label19" runat="server" Text="Telefono Destinatario" Font-Size="10" Font-Names="arial" ForeColor="black" />
                                 <br />
                                &nbsp <asp:TextBox ID="TextBox_TELEFONO" runat="server" Style=" width: 350px"></asp:TextBox>
                                <br />
                                &nbsp <asp:Label ID="Label20" runat="server" Text="Correo electrónico Destinatario" Font-Size="10" Font-Names="arial" ForeColor="black" />
                                 <br />
                                &nbsp <asp:TextBox ID="TextBox_CORREO_ELECTRONICO" runat="server" Style=" width: 350px"></asp:TextBox>
                                <br />
                                 &nbsp <asp:Label ID="Label22" runat="server" Text="Anexo guía" Font-Size="10" Font-Names="arial" ForeColor="black" />
                                <br />
                                &nbsp <asp:TextBox ID="TextBox_ANEXO" runat="server" Style=" width: 350px" TextMode="MultiLine" MaxLength="50"></asp:TextBox>
                                 <br />
                            </div>
                            <div id="contenido_botones_edita_tipo_tramite" style="width: 549px; height: 20%; background-color: #E7EDF5">
                               
                                &nbsp 
                                <asp:Button ID="Button_procesa_tramite_envio" runat="server" style="margin-top:5px; margin-bottom:5px" Text="Asignar guía" ToolTip="Asigna guía de envío" CssClass="boton"  />
                               &nbsp <asp:Button ID="Button_actualizar_guia" runat="server" Text="Actualizar guía" style="margin-top:5px; margin-bottom:5px" ToolTip="Actualiuza  guía de envío" CssClass="boton"  />
                                &nbsp <asp:Button ID="Button_anular_guia" runat="server" Text="Anular guía" style="margin-top:5px; margin-bottom:5px" ToolTip="Anular  guía de envío" CssClass="boton"  />
                                &nbsp <asp:Button ID="Button_descarga_guia" runat="server" Text="Descarga guía" style="margin-top:5px; margin-bottom:5px" ToolTip="Descargar guía de envío" CssClass="boton"  />
                                <input id="Hidden_procesa_tramite_envio" type="hidden" value="FALSE" runat="server">
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <asp:UpdatePanel ID="UpdatePanel_bonones_ocultos" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                         <asp:Button ID="Button_tipo_empresa_guia" runat="server" style="margin-top:5px; margin-bottom:5px; display:none" Text="Procesar" ToolTip="Procesa envío tramite correspondencia" CssClass="boton"  />
                    </ContentTemplate>
                    </asp:UpdatePanel>
            </asp:Panel>
        </div>
       <!--mensaje_personalizado-->
	            <asp:Panel ID="Panel_mensaje_personalizado" runat="server" Style="display:none; color: White; width: 400px; height: 150px">
                    <asp:ModalPopupExtender ID="ModalPopupExtender_mensaje_personalizado" runat="server"
                        TargetControlID="Button_mensaje_personalizado" BackgroundCssClass="FondoAplicacion"
                        CancelControlID="Button_cerrar_mensaje_personalizado" PopupControlID="Panel_mensaje_personalizado">
                    </asp:ModalPopupExtender>
                    <div id="div_persoanlizado" class="cabecera2">
                        <asp:Label ID="Label_mensaje_personalizado_" runat="server" Text="Mensaje de servidor" Font-Size="10" Style="float: left; font-family: Arial; margin-left: 5px; margin-top: 2px">
                        </asp:Label>
                        <div id="Divcerrarbuton2_mensaje_personalizado" style="float: right">
                            <asp:Button ID="Button_cerrar_mensaje_personalizado" runat="Server" Text="X"
                                ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />
                        </div>
                    </div>
                    <div id="contenido_procesa_mensaje_personalizado" style="background-color: white; width: 100%; height: 99%; border: thin double #000080; color: black; background-color: #FFFFFF">
                        <br />
                        <div style="height: 60%; float: left; width: 50px">
                            <asp:Label ID="Label_estil" runat="server" Text="&#9888;" Style="font-family: Arial; font-size: 40px; color: black; margin-top: 60px; margin-left: 10px"></asp:Label>
                        </div>
                        <div style="height: 60%; overflow: auto; float: right; width: 330px; margin-right: 10px; text-align: center">
                            <br />
                            <asp:Label ID="Label_mensaje_personalizado" runat="server" Text="Detalle" Style="font-family: Arial; font-size: 11px; color: black; padding-top: 30px; padding-left: 1px; padding-right: 10px; margin-right: 30px; font-weight: 500"></asp:Label>
                        </div>
                        <asp:Button ID="Button_mensaje_personalizado" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                        <asp:Button ID="ButtonSalir_mensaje_personalizado" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    </div>
	            </asp:Panel>
        <!--Termina mensaje_personalizado-->
        <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 200px">
            <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
            Processing ...
        </div>
        <iframe id="txtArea1" style="display:none"></iframe>
    </form>
</body>
</html>
