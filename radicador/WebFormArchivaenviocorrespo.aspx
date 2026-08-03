<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormArchivaenviocorrespo.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormArchivaenviocorrespo" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Correspondencia enviadas por archivar</title>
     <script src="../js/ui/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
     <script src="../js/ScrollableGridPlugin.js"></script>   
    <script src="../js/ScrollableGridViewPlugin_ASP.NetAJAXmin.js" type="text/javascript"></script>
    <script src="../Fixed-Header-Table-master/gridviewScroll.min.js"></script>
   <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />   
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <link href="../Styles/Menu3.css" rel="stylesheet" />
    <link href="../Styles/tabs.css" rel="stylesheet" />
    <link href="../Styles/styleMenu.css" rel="stylesheet" type="text/css" /> 
    <script src="../js/radicacion/WebFormArchivaenviocorrespo.js"></script>
    <script src="../js/java_general/general_code_java.js"></script>
     <link href="../Styles/menu_lib.css" rel="stylesheet" />
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
    <form id="form_archiva" runat="server">
        <div id="contendor_principal" style="height: 100%; width: 100%">
            <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" EnablePageMethods="true">
            </asp:ScriptManager>
            <script  accesskey="javascript" type="text/javascript">
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
                   
                    try {
                    if (elment_postbak.id == "Button_procesar_archivo" || elment_postbak.id == "Button_devolver_envio") {
                        if (document.getElementById("Hidden_procesa_tramite_envio").value == "YES") {
                            eliminar_fila_data_gred()
                            plugin_grwedview();
                            //auto_zise_popup_validacion_radicados();
                        }
                    }
                    //mueve_scroll_data_gred('GridView_val_radicacion', 'hdnEmailID_VAL');
                    if (elment_postbak.id == "Button_consulta_pendientes_procesar") {
                        plugin_grwedview();
                    }
                    if (elment_postbak.id == "Button_digitaliza_documento") {
                        auto_size_control_documentos();
                    }
                    if (elment_postbak.id == "Button_ver_documento") {
                        resize_opcion_descarga_respuesta();
                    }
                    if (elment_postbak.id == "ButtonVisua") {
                        dispalyVisorEmergente();
                    }
                    if (elment_postbak.id == "ImageButton_adjunt") {
                        if (document.getElementById("Hidden_0002").value == "1") {
                            document.getElementById("Hidden_0002").value = "0";
                            auto_zise_popup_lista_chequeo("1");
                        }
                    }
                    if (elment_postbak.id == "ImageButtonActivaClasifica") {
                        if (document.getElementById("Hidden_0004").value == "1") {
                            document.getElementById("Hidden_0004").value = "0";
                            auto_zise_popup_lista_chequeo_edita("1");
                        }
                    }
                    if (elment_postbak.id == "Button_descarga_docmento_respuesta") {
                        resize_opcion_descarga_respuesta();
                    }
                      
                    if (elment_postbak.id == "Button_notificar_envio") {
                        actuo_zise_popup_compartir_correo_electronico();
                    }
                    }
                    catch (err) {
                        alert(" Funcion CheckStatus asincrona WebFormArchivaenviocorrespo.aspx" + err.message);
                    }
                    finally {
                        progres_hiden('progres_bar');
                        var elmen = document.getElementById(elment_postbak.id)
                        if (elmen.type == "button" || elmen.type == "image" || elmen.type == "submit") {
                            elmen.disabled = false;
                            elmen.value = value_element;
                        }
                    }
                }

            </script>
            <input id="Hidden_resultado_web_service" type="hidden" value="YES" runat="server">
            <input id="Hidden_alert_respuesta" type="hidden" value="YES" runat="server">  
            <div id="Contenedorderecho" style="width: 100%; position: inherit; left: auto; float: right; height: 99.5%; float: right">
                <div id="menu_var" class="navbar_gray" style="overflow: auto; width: auto">
                    <div class="dropdown_gray">
                        <div class="dropbtn_gray">
                            OPCIONES
                            <i class="fa fa-caret-down"></i>
                        </div>
                        <div class="dropdown-content_gray">
                            <a href="#" onclick="activa_menu_general_diference_(event,this,'R-ACH')">Finalizar proceso de respuesta</a>
                            <a href="#" onclick="activa_menu_general_diference_(event,this,'R-EPA')">Enviar a pendientes por enviar</a>
                            <a href="#" onclick="activa_menu_general_diference_(event,this,'R-ENC')">Notificar al correo electrónico</a>
                        </div>
                    </div>
                    <div class="dropdown_gray">
                        <div class="dropbtn_gray">
                            DOCUMENTOS 
                                <i class="fa fa-caret-down"></i>
                        </div>
                        <div class="dropdown-content_gray">
                            <a href="#" onclick="activa_menu_general_diference_(event,this,'R-DDR')">Descargar documento de respuesta</a>
                            <a href="#" onclick="activa_menu_general_diference_(event,this,'R-GDR')">Gestión documentos de respuesta</a>

                        </div>
                    </div>
                    <div class="dropdown_gray">
                        <div class="dropbtn_gray">
                            REPORTES 
                                <i class="fa fa-caret-down"></i>
                        </div>
                        <div class="dropdown-content_gray">
                            <a href="#" onclick="activa_menu_general_diference_(event,this,'E-ERC')">Exportar listado de respuesta para currier</a>
                            <a href="#" onclick="activa_menu_general_diference_(event,this,'E-GDR')">Exportar la lista actual</a>

                        </div>
                    </div>

                    <asp:UpdatePanel ID="UpdatePanel_menu_var_event" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <input id="Hidden_menu_var_event_dive" type="hidden" value="" runat="server" />
                            <asp:Button ID="Button_me_active_men_dive" runat="server" Text="" Style="display: none; width: 1px; height: 1px" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div id="contenido_titulo_val_radicacion" style="height: 5%; width: 75%; float:right; overflow:auto " class="border_superior_radius">
                    <asp:UpdatePanel ID="UpdatePanelabel_val_radicacion" runat="server" UpdateMode="Conditional" >
                        <ContentTemplate>
                            <input id="hdnEmailID_VAL" type="hidden" value="-1" runat="server">
                            <input id="Hidden_consecutivo_radicado" type="hidden" value="-1" runat="server">
                            <asp:Label ID="titulo_label_val_radicacion" runat="server"  style="float:left; font-weight:600; margin-top:5px; margin-left:5px; text-overflow:ellipsis; font-family:Arial; font-size:13px">Correspondencia por archivar</asp:Label>
                             &nbsp <asp:CheckBox ID="CheckBox_busqueda" runat="server"  Text="Sólo palabras completas" Font-Names="arial"  Font-Size="6" style="float:right"/>
                            &nbsp <asp:TextBox ID="TextBox_busqueda" runat="server" Width="130px" Style="float:right"></asp:TextBox>
                             <a  style="margin-right:5px; float:right; color:black; height:35px; margin-top:5px" title="Buscar" href="#"  onclick="activa_boton_client_server('Button_buscar_lista')"><i class="fas fa-search fa-1x" ></i></a> 
                             &nbsp <asp:Button ID="Button_buscar_lista" runat="server" Width="65px" Text="Buscar" style="float:right; display:none" OnClientClick="busqueda_gred_por_enviar('hdnEmailID_VAL','GridView_val_radicacion','TextBox_busqueda','CheckBox_busqueda');" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div id="Contentizquierdo" style="width: 24%; height: 99.5%; float: left; position: relative; top: 0px; left: 0px;">
                    <div id="contenido_titulo_controles_consulta" style="height: 5%; width: 100%;overflow: hidden;text-overflow:ellipsis" class="border_superior_radius">
                        <asp:Label ID="Label7" runat="server"  style="font-weight:600; margin-left:5px; margin-top:2px; font-family:Arial; font-size:13px">Campos de busqueda</asp:Label>
                    </div>
                    <div id="contenido_controles_consulta" style="width: 100%; height: 75%; border:1px solid #ccc">
                    <asp:UpdatePanel ID="UpdatePanelContenido_val_radicacion" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Panel ID="_Panelvalidacion_val_radicacion" runat="server" ScrollBars="Vertical" Height="100%" Width="100%" Wrap="false"  DefaultButton="Button_consulta_pendientes_procesar">
                                <asp:Label ID="Label10" runat="server" Text="Areas/Dependencias" Style="font-size:12px; font-family:Arial; margin-left:5px"></asp:Label> <br />
                                <asp:DropDownList ID="DropDownList_areas_depart" runat="server" Width="250px" style="margin-left:5px"></asp:DropDownList> <br />
                                 <asp:Label ID="Label11" runat="server" Text="Empresa Envío/currier" Style="font-size:12px; font-family:Arial; margin-left:5px"></asp:Label> <br />
                                <asp:DropDownList ID="DropDownList_empresa_envio" runat="server" Width="250px" style="margin-left:5px"></asp:DropDownList> <br />
                                 <asp:Label ID="Label18" runat="server" Text="Guía de envío" Style="font-size:12px; font-family:Arial; margin-left:5px"></asp:Label> <br />
                                <asp:TextBox ID="TextBox_GUIA_ENVIO" runat="server" Width="250px" CssClass="date_2" style="margin-left:5px"></asp:TextBox> <br />     
                                <asp:Label ID="Label12" runat="server" Text="Fecha vencimiento" Style="font-size:12px; font-family:Arial; margin-left:5px"></asp:Label> <br />
                                <asp:TextBox ID="TextBox_fecha_ini" runat="server" Width="100px" CssClass="date_2" style="margin-left:5px">
                                </asp:TextBox>        
                                <asp:CalendarExtender ID="TextBox_fecha_ini_CalendarExtender" runat="server" TargetControlID="TextBox_fecha_ini"  PopupButtonID="ImageButton_ini" Format = "yyyy-MM-dd"/>
                                <asp:ImageButton ID="ImageButton_ini" runat="server"  ImageUrl="../imagera/Calendar.png"/> &nbsp
                                <asp:TextBox ID="TextBox_fecha_fin" runat="server" Width="100px" CssClass="date_2"></asp:TextBox>
                                <asp:CalendarExtender ID="TextBox_fecha_fin_CalendarExtender" runat="server" TargetControlID="TextBox_fecha_fin" PopupButtonID="ImageButton_fin" Format = "yyyy-MM-dd" />
                                <asp:ImageButton ID="ImageButton_fin" runat="server" ImageUrl="../imagera/Calendar.png"/> <br />
                                <asp:Label ID="Label13" runat="server" Text="Fecha respuesta" Style="font-size:12px; font-family:Arial; margin-left:5px"></asp:Label> <br />
                                <asp:TextBox ID="TextBox_fecha_resp_ini" runat="server" Width="100px" CssClass="date_2" style="margin-left:5px">
                                </asp:TextBox>
                                <asp:CalendarExtender ID="TextBox_fecha_resp_ini_CalendarExtender" runat="server" TargetControlID="TextBox_fecha_resp_ini" PopupButtonID="ImageButton_fecha_resp_ini" Format = "yyyy-MM-dd"/>
                                <asp:ImageButton ID="ImageButton_fecha_resp_ini" runat="server"  ImageUrl="../imagera/Calendar.png"/> &nbsp
                                <asp:TextBox ID="TextBox_fecha_resp_fin" runat="server" Width="100px" CssClass="date_2"></asp:TextBox>
                                <asp:CalendarExtender ID="TextBox_fecha_resp_fin_CalendarExtender" runat="server" TargetControlID="TextBox_fecha_resp_fin" PopupButtonID="ImageButton_resp_fin" Format = "yyyy-MM-dd" />
                                <asp:ImageButton ID="ImageButton_resp_fin" runat="server" ImageUrl="../imagera/Calendar.png"/> <abbr></abbr> <br />
                                <asp:Label ID="Label14" runat="server" Text="Radicado Peticionario" Style="font-size:12px; font-family:Arial; margin-left:5px"></asp:Label> <br />
                                <asp:TextBox ID="TextBoxRadicado" runat="server" Width="250px" CssClass="date_2" style="margin-left:5px"></asp:TextBox> <br />
                                <asp:Label ID="Label15" runat="server" Text="Radicado respuesta" Style="font-size:12px; font-family:Arial; margin-left:5px"></asp:Label> <br />
                                <asp:TextBox ID="TextBoxRadicado_respuesta" runat="server" Width="250px" style="margin-left:5px"></asp:TextBox> <br />
                                <asp:Label ID="Label16" runat="server" Text="Nombre usario responsable de respuesta" Style="font-size:12px; font-family:Arial; margin-left:5px"></asp:Label> <br />
                                <asp:TextBox ID="TextBoxUSUARIO_RESPONSABLE" runat="server" Width="250px" CssClass="date_2" style="margin-left:5px"></asp:TextBox> <br />
                                <asp:Label ID="Label17" runat="server" Text="Destinatario de la respuesta" Style="font-size:12px; font-family:Arial; margin-left:5px"></asp:Label> <br />
                                <asp:TextBox ID="TextBoxDESTINATARIO" runat="server" Width="250px" CssClass="date_2" style="margin-left:5px"></asp:TextBox> <br />
                                <asp:Label ID="Label19" runat="server" Text="Fecha envío al destinatario" Style="font-size:12px; font-family:Arial; margin-left:5px"></asp:Label> <br />
                                <asp:TextBox ID="TextBox_FECHA_ENVIO_INI" runat="server" Width="100px" CssClass="date_2" style="margin-left:5px">
                                </asp:TextBox>        
                                <asp:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="TextBox_FECHA_ENVIO_INI"  PopupButtonID="ImageButton_FECHA_ENVIO_INI" Format = "yyyy-MM-dd"/>
                                <asp:ImageButton ID="ImageButton_FECHA_ENVIO_INI" runat="server"  ImageUrl="../imagera/Calendar.png"/> &nbsp
                                <asp:TextBox ID="TextBox_FECHA_ENVIO_FIN" runat="server" Width="100px" CssClass="date_2"></asp:TextBox> &nbsp
                                <asp:CalendarExtender ID="TextBox_FECHA_ENVIO_FIN_CalendarExtender" runat="server" TargetControlID="TextBox_FECHA_ENVIO_FIN" PopupButtonID="ImageButton_FECHA_ENVIO_FIN" Format = "yyyy-MM-dd"/>
                                 <asp:ImageButton ID="ImageButton_FECHA_ENVIO_FIN" runat="server"  ImageUrl="../imagera/Calendar.png"/> &nbsp
                              </asp:Panel>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                    <div id="contenido_controles_buton_consulta" style="width: 100%; height: 20%; background-color: white; border: 1px solid #ccc; overflow: auto">
                        <asp:UpdatePanel ID="UpdatePanel_botones_validacion" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <input id="Hidden_resultado_consulta" type="hidden" value="" runat="server">
                                &nbsp
                                <asp:Button ID="Button_consulta_pendientes_procesar" Text="Consultar" runat="server" Width="100px" Style="margin-top:3px" ToolTip="Consulta pendiente por archivar envios" CssClass="boton_azul" />
                                &nbsp 
                                <asp:Button ID="Button_lipiar_val_radicacion" Text="Limpiar" runat="server" Width="100px" Style="margin-top:3px" ToolTip="Limpiar campos consulta" CssClass="boton_azul" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
                <div id="contenido_datagrid_val_radicacion" style="height: 60%; width: 75%; position: relative; float:right; border:1px solid #ccc">
                    <asp:UpdatePanel ID="UpdatePanel_conenido_grid_val_radicacion" runat="server" UpdateMode="Conditional" RenderMode="Block" style="width: 100%; height: 100%">
                        <ContentTemplate>
                            <asp:GridView ID="GridView_val_radicacion" runat="server" Width="100%" EnableViewState="false"
                                 AutoGenerateSelectButton="False" CssClass="filtrar" GridLines="None" Font-Size="12px" ViewStateMode="Enabled" Font-Names="Arial">
                                        <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                        <HeaderStyle CssClass="GridviewScrollHeader_line_blanco" />
                                        <RowStyle CssClass="GridviewScrollItem_line" />
                                        <PagerStyle CssClass="GridviewScrollPager_line" />
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkSelection" runat="server" onclick="inactiva_chek();" CssClass="dummychkstyle" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>

                        </ContentTemplate>
                        <Triggers>
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
                <div id="Contenido_botones_tipo_radicado" style="height: 10%; width: 75%; background-color:white; float:right; border:1px solid #ccc; overflow:auto">
                    <div id="superior_alto_boton" style="width: 100%; height: 5%; background-color:white"></div>
                    <asp:UpdatePanel ID="UpdatePanel_botones_radicacion" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            &nbsp
                        <asp:Button ID="Button_procesar_envio" runat="server" Text="Archivar" Width="80px" ToolTip="Archivar proceso de envío" CssClass="boton_azul" Style="display:none" />
                            &nbsp 
                        <asp:Button ID="Button_devolver_envio" Text="A envío" runat="server" Width="80px" Style="display:none" ToolTip="Devolver al bandeja de envío" CssClass="boton_azul" OnClientClick="confirma_respuesta('Desea devolver a bandeja de envíos?');" />
                            &nbsp 
                        <asp:Button ID="Button_notificar_envio" Text="Notificar" runat="server" Width="80px" Style="display:none" ToolTip="Notificar al correo a usuario remitente" CssClass="boton_azul" />
                            &nbsp 
                        <asp:Button ID="Button_ver_documento" Text="Documento" runat="server" Width="100px" Style="display:none" ToolTip="Descarga documento respuesta" CssClass="boton_azul" />
                            &nbsp
                            <input id="Button_exportar" type="button" value="Exportar" style="width: 70px; display:none" title="Exportar resultados a axcel" class="boton_azul" onclick="fnExcelReport();" />
                            <input id="Button_reporte_currier" type="button" value="Reporte currier" title="Reporte de currier o empresa de mensajería" style="width: 110px; display:none" class="boton_azul" onclick="fnexcelcurrier('RADICADO_RESPUESTA|DESTINATARIO|FECHA_VENCE|FECHA_RESPUETA|NOMBRE_AREA|GUIA_ENVIO|EMPRESA_ENVIO');" />
                            <input id="Hidden_colum_header" type="hidden" value="" runat="server">
                            <input id="Hidden_ruta_archivo" type="hidden" value="" runat="server">
                            &nbsp  
                            <asp:Button ID="Button_digitaliza_documento" Text="Documentos" runat="server" Width="100px" Style="display:none" ToolTip="Documentos relacionados" CssClass="boton_azul"  />                           
                        </ContentTemplate>
                        <Triggers>
                            
                        </Triggers>
                    </asp:UpdatePanel>

                    <div id="inferior_bajo_boton" style="width: 100%; height: 5%; background-color: white">
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
        <!--Notifica gestión-->
          <div id="notifica_gestion">
            <asp:Panel ID="Panel_notifica_gestion" runat="server" Style="display:none; color: White; width: 70%; height: 100%; margin:auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_notifica_gestion" runat="server" BehaviorID="Panel_notifica_gestion_ModalPopupExtender"  TargetControlID="ButtonSalir_notifica_gestion"
                    CancelControlID="Button_cerrar_notifica_gestion" PopupControlID="Panel_notifica_gestion" BackgroundCssClass="FondoAplicacion" >
                </asp:ModalPopupExtender>
                <div id="divcabecer2_notifica_gestion" class="modal_title_superior">     
                    <asp:Label ID="Label2" runat="server" Text="Envío de correo electrónico" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton2_notifica_gestion" style="float: right">
                        <asp:Button ID="Button_cerrar_notifica_gestion" runat="Server" Text="X" CssClass="modal_boton_hiden"
                             ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_notifica_gestion" style="background-color:white; width:100%; height:auto" class="modal_content_back">
                    <asp:UpdatePanel ID="UpdatePaneliframe" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <iframe  Style="color: White; width: 100%; background-color:white; height:auto; overflow:auto" id="ifimpre" runat="server" frameborder="0"  ></iframe>
                             <input id="Hidden_cuenta_correo_envio" type="hidden" value="" runat="server">
                             <input id="Hidden_correo_envio_default" type="hidden" value="" runat="server">
                             <input id="Hidden_imagen_adjunta" type="hidden" value="" runat="server">
                             <input id="Hidden_asunto_notificacion" type="hidden" value="" runat="server">
                             <input id="Hidden_convierte_pdf" type="hidden" value="" runat="server">
                            <input id="Hidden_tipo_notificacion" type="hidden" value="ENVIO CORESPONDENCIA" runat="server">
                            
                             <input id="Hidden_id_plantilla_radicado" type="hidden" value="" runat="server">
                            
                        </ContentTemplate>
                    </asp:UpdatePanel>
                   
                </div>
                    <asp:Button ID="Button7" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_notifica_gestion" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
            </asp:Panel>
        </div>
        <div id="procesa_archivo_tramite">
            <asp:Panel ID="Panel_procesa_archivo_tramite" runat="server" Style="display:none; color: White; width: auto; height:auto" CssClass="modal_content_general" > 
                 <asp:ModalPopupExtender ID="ModalPopupExtender_procesa_archivo_tramite" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_procesa_archivo_tramite"
                    PopupControlID="Panel_procesa_archivo_tramite" CancelControlID="Button_cerrar_procesa_archivo_tramite">
                </asp:ModalPopupExtender>
                 <div id="divcabecer2_procesa_archivo_tramite" class="modal_title_superior">       
                    <asp:Label ID="Label4" runat="server" Text="Procesa tramite por archivar"  Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton2_procesa_archivo_tramite" style="float: right">
                        <asp:Button ID="Button_cerrar_procesa_archivo_tramite" runat="Server" Text="X" CssClass="modal_boton_hiden"
                            ToolTip="Cerrar ventana" />
                    </div>
                </div>
                
                <div id="contenido_procesa_archivo_tramite" style="width: 100%; height: 90%; background-color: white; overflow:auto" class="modal_content_back">
                    <asp:UpdatePanel ID="UpdatePanel_procesa_archivo_tramite" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            
                            <asp:Label ID="Label32" runat="server" Text="Estado tramite" ForeColor="Black" Font-Names="arial" Font-Size="10" Style="margin-top:10px; margin-left:5px" />
                            <br />
                            
                            <asp:DropDownList ID="DropDownListESTADO_CONFIRMACION_GUIA" runat="server" Width="350px" Style="margin-left:5px">
                                <asp:ListItem></asp:ListItem>
                                <asp:ListItem>Entregada</asp:ListItem> 
                                <asp:ListItem>Devolucion Permanente</asp:ListItem>
                              </asp:DropDownList>
                            <br />
                            
                            <asp:Label ID="Label5" runat="server" Text="Fecha recibo destinatario" ForeColor="Black" Font-Names="arial" Font-Size="10" Style="margin-left:5px" />
                            <br />
                           
                            <asp:TextBox ID="TextBox_fecha_envio" runat="server" Style="width: 100px; margin-left:5px" CssClass="date_2" ></asp:TextBox>
                            <asp:CalendarExtender ID="TextBox_fecha_envio_CalendarExtender" runat="server" BehaviorID="TextBox_fecha_envio_CalendarExtender" TargetControlID="TextBox_fecha_envio" Format="yyyy-MM-dd" PopupButtonID="Button_fecha" />
                            <asp:TextBoxWatermarkExtender ID="TextBox_fecha_envio_TextBoxWatermarkExtender" runat="server" BehaviorID="TextBox_fecha_envio_TextBoxWatermarkExtender" TargetControlID="TextBox_fecha_envio" WatermarkText="yyyy-mm-dd" WatermarkCssClass="watermark" />
                            &nbsp<asp:Button ID="Button_fecha" runat="server" Text="#" CssClass="boton" />
                            
                            <br />
                           
                            <asp:Label ID="Label6" runat="server" Text="Hora recibo destinatario" ForeColor="Black" Font-Names="arial" Font-Size="10" Style="margin-left:5px" />
                            <br />
                            
                            <asp:TextBox ID="TextBox_hora_envio" runat="server" Style="width: 100px; margin-left:5px"></asp:TextBox>
                            <br />
                             <asp:Label ID="Label20" runat="server" Text="Nota guía" ForeColor="Black" Font-Names="arial" Font-Size="10" Style="margin-left:5px" />
                            <br />
                          
                            <asp:TextBox ID="TextBox_NOTA_CLIENTE" runat="server" Style="width: 350px; margin-left:5px; margin-right:10px"  TextMode="MultiLine" MaxLength="255" ></asp:TextBox>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div id="contenido_botones_procesa_archivo_tramite" style="width: auto; height: 10%; background-color:white; overflow:auto" class="border_inferior_radius_blanco">
                    <asp:UpdatePanel ID="UpdatePanel_botones_procesa_archivo_tramite" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <input id="Hidden_procesa_tramite_envio" type="hidden" value="FALSE" runat="server">
                            &nbsp
                            <asp:Button ID="Button_procesar_archivo" runat="server" Text="Aceptar" CssClass="boton_azul" Style="margin-top: 5px; margin-bottom:5px; float:right; margin-right:10px" OnClientClick="confirma_respuesta('Desea Archivar el envíon del tramite?');" />
                        </ContentTemplate>
                    </asp:UpdatePanel>

                </div>
                     <asp:Button ID="Button4" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" Style="display:none" />
                    <asp:Button ID="ButtonSalir_procesa_archivo_tramite" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" Style="display:none"/>

            </asp:Panel>
        </div>
        <div id="procesa_tramite_envio">
            <asp:Panel ID="Panel_procesa_tramite_envio" runat="server" Style="display: none; color: White; width: 450px; height: 230px">
                <asp:ModalPopupExtender ID="ModalPopupExtender_procesa_tramite_envio" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_procesa_tramite_envio"
                    PopupControlID="Panel_procesa_tramite_envio" CancelControlID="Button_cerrar_procesa_tramite_envio">
                </asp:ModalPopupExtender>
                <div id="divcabecer2_edita_tipo_tramite" class="cabecera2">
                    <asp:Button ID="Button2" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_procesa_tramite_envio" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Label ID="Label8" runat="server" Text="Procesa envío" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton2_procesa_tramite_envio" style="float: right">
                        <asp:Button ID="Button_cerrar_procesa_tramite_envio" runat="Server" Text="X"
                            ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />
                    </div>

                </div>
                <asp:UpdatePanel ID="UpdatePanel_procesa_tramite_envio" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div id="contenido_procesa_tramite_envio" style="width: 349px; height: 200px; background-color: white">
                            <div id="contenido_campos_procesa_tramite_envio" style="width: 449px; height: 150px; background-color: white">
                                &nbsp
                                <asp:Label ID="Label_edita_tipo_tramite" runat="server" Text="Empresa envío trámite" ForeColor="Black" Font-Names="arial" Font-Size="11" Style="" />
                                <br />
                                &nbsp
                                <asp:DropDownList ID="DropDownList_procesa_tramite_envio" runat="server" Font-Size="10" Font-Names="arial" Style="width: 250px"></asp:DropDownList>
                                <br />
                                &nbsp
                                <asp:Label ID="Label9" runat="server" Text="Código guía envío (*)" Font-Size="10" Font-Names="arial" ForeColor="red" />
                                <br />
                                &nbsp 
                                <asp:TextBox ID="TextBox_codigo_guia_envio" runat="server" Style="background-color: yellow; width: 200px"></asp:TextBox>
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
                                <asp:Label ID="Label1" runat="server" Text="Consecutivo radicado" Font-Size="10" Font-Names="arial" ForeColor="Black" Style="width: 10px" />
                                &nbsp &nbsp 
                                <asp:CheckBox ID="RadioButton_valida_radicadores" runat="server" Text="Requiere radicado saliente" ForeColor="Black" Font-Names="arial" Font-Size="10" Checked="true" />
                                <br />
                                &nbsp 
                                <asp:TextBox ID="TextBox_consecutivo_radicado_saliente" runat="server" Style="width: 200px"></asp:TextBox>

                                <asp:AutoCompleteExtender ID="TextBox_consecutivo_radicado_saliente_AutoCompleteExtender" runat="server" BehaviorID="TextBox_consecutivo_radicado_saliente_AutoCompleteExtender"
                                    MinimumPrefixLength="2"
                                    EnableCaching="True"
                                    CompletionSetCount="10"
                                    CompletionInterval="50"
                                    ServiceMethod="GetGuiaRadicaconasp_flow"
                                    ServicePath="../webservice/WebServiceRadicacion.asmx"
                                    ContextKey="Consecutivo_Rad|ra_registro_general_radicacion"
                                    UseContextKey="True"
                                    OnClientShown="onDataShown"
                                    CompletionListCssClass="completionList" TargetControlID="TextBox_consecutivo_radicado_saliente">
                                </asp:AutoCompleteExtender>

                                <br />

                                <br />
                            </div>
                            <div id="contenido_botones_edita_tipo_tramite" style="width: 450px; height: 50px; background-color: #E7EDF5">
                                <br />
                                &nbsp 
                                <asp:Button ID="Button_procesa_tramite_envio" runat="server" Text="Procesar" ToolTip="Procesa envío tramite correspondencia" CssClass="boton" OnClientClick="confirma_respuesta('Desea procesar el envío tramite?');" />
                                
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </asp:Panel>
        </div>
         <!--opcion_descarga_respuesta!-->
         <div id="opcion_descarga_respuesta">
            <asp:Panel ID="Panel_opcion_descarga_respuesta" runat="server"  Style="display:none; color: White;  height:auto; width:auto; background-color:white"  CssClass="modal_content_general" >              
                <asp:ModalPopupExtender ID="ModalPopupExtender_opcion_descarga_respuesta" runat="Server" BackgroundCssClass="FondoAplicacion" 
                     TargetControlID="ButtonSalir_opcion_descarga_respuesta"
                    PopupControlID="Panel_opcion_descarga_respuesta" CancelControlID="Buttoncerrarimpre_opcion_descarga_respuesta">
                </asp:ModalPopupExtender>
                
                    <div id="Divcerrarbuton2_opcion_descarga_respuesta" style="float: right; margin-top:10px; margin-right:3px; 
                        background-color:white; font-weight:700; border:none; border-width: 0px 0px 0px 0px;">
                        <asp:Button ID="Buttoncerrarimpre_opcion_descarga_respuesta" runat="Server" Text="X" 
                            ForeColor="#000066" Height="21px" ToolTip="Cerrar ventana"  />
                    </div>
                
                <asp:UpdatePanel ID="UpdatePane_opcion_descarga_respuesta" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div id="Contenido_opcion_descarga_respuesta" style=" color: black; background-color: #FFFFFF; height:auto; width: 100%">
                            <asp:MutuallyExclusiveCheckBoxExtender ID="MutuallyExclusiveCheckBoxExtender1" runat="server" TargetControlID="Check_opcion_descarga_respuesta_sin_firma"
                            Key="radicado"></asp:MutuallyExclusiveCheckBoxExtender>
                        <asp:mutuallyexclusivecheckboxextender id="Mutuallyexclusivecheckboxextender2" runat="server" targetcontrolid="CheckBox_opcion_descarga_respuesta_con_firma"
                            key="radicado"></asp:mutuallyexclusivecheckboxextender>
                            <div id="div_title_opcion_descarga_respuesta"  class="_seleccion_title" style="text-align:left">
                                <asp:Label ID="Label25" runat="server" Text="Opciones para descargar el documento" Style="font-family:Arial; font-size:16px"></asp:Label>
                            </div>
                             
                            <div id="div_opcion_descarga_respuesta" class="_seleccion_campo_adjunta" style="" >
                                <asp:CheckBox ID="CheckBox_opcion_descarga_respuesta_con_firma" runat="server" Text="Guardar documento con firma " Checked="true" ForeColor="Black" Font-Size="10" Font-Names="Arial" Style="margin-left: 5px" Enabled="true" />
                                <asp:CheckBox ID="Check_opcion_descarga_respuesta_sin_firma" runat="server" Text="Guardar documento sin firma" Checked="false" ForeColor="Black" Font-Size="10" Font-Names="Arial" Style="margin-left: 5px; display:block" Enabled="true" />
                                
                            </div>          
                            <div>
                                <asp:Label ID="Label_formato_documento" runat="server" Text="Formato descarga" style="font-family:Arial; margin-left:5px; float:left"></asp:Label>
                                <asp:DropDownList ID="DropDownList_tipo_archivo" runat="server" ClientIDMode="Inherit">
                                    <asp:ListItem Selected="True">PDF</asp:ListItem>
                                    <asp:ListItem>DOCX</asp:ListItem>
                                </asp:DropDownList>
                                
                            </div>
                            <div id="div_inferior_opcion_descarga_respuesta" style="text-align:right; margin-top:10px" >
                               <asp:Button ID="Button_descarga_docmento_respuesta" runat="server" Text="Descargar"  CssClass="boton_azul" Style="margin-right:10px" />
                                 <asp:HiddenField ID="HiddenField_descarga_docmento_respuesta" runat="server" Value="" />
                            </div>                 
                            
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                    <asp:Button ID="ButtonSalir_opcion_descarga_respuesta" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
            </asp:Panel>
        </div>
         <!--CUADRO DE DIALOGO ENLACE DOCUMENTOS-->
              <asp:Panel ID="PanelPopupExt" runat="server" Style="display:none" ForeColor="White" Width="100%" Height="100%" CssClass="modal_content_general">
                  <div id="TABCPTION" class="modal_title_superior" style="width:100%">
                      <asp:Label ID="Label21" runat="server" Text="Digitalización de documentos" Font-Size="10" Style="float: left; font-family:Arial"></asp:Label>
                      <div id="Cerrar" style="float: right; height: 5px;">
                          <asp:Button ID="botonpopupshowExt" runat="Server" Text="X" CssClass="modal_boton_hiden"
                                OnClientClick="dispalyInterfaceEscaner();" ToolTip="Cerrar Ventana" />
                      </div>
                  </div>

                  <div id="text" class="fond_contextual"
                      style=" color: White; width: 100%; height: 95%" >

                      <div id="Area_Enlace" style="width: 20%; height: 100%; float: left;margin-top: 1px; border-color: #b0c4de; border-style: ridge; border-width: 1px">
                          <asp:UpdatePanel ID="UpdateDatos" runat="server" UpdateMode="Conditional"  RenderMode="Inline">
                              <ContentTemplate>
                                 
                                  <div id="Separacio2n" style="width: 100%;background-color: #f5f5f5">
                                      <asp:Label ID="Labeltext" runat="server" Text="Control de documetos digitalizados "  ForeColor="Black" style="font-family:Arial; font-size:12px; font-weight: 600"></asp:Label>
                                  </div>
                                  <div id="Datos_Enlace" style="width: 100%; margin-top: 1px; border-color: #b0c4de; border-style: ridge; border-width: 1px; overflow:auto; background-color:white">
                                      <asp:TextBox ID="TextBoxDatos" runat="server" TextMode="MultiLine" Width="95%" Height="50px" ReadOnly="True" style="display:none"></asp:TextBox>
                                      <asp:Button ID="Button_actualiza_enlace" runat="server" style="display:none; margin:1px" Text="&#8634; Actualizar índices" Width="99%" ToolTip="Actualiza el índice de los nuevos documentos relacionados a la tarea"  CssClass="boton_azul"/>
                                      <asp:Button ID="Buttonaceptar" runat="server" Text="&#10004; Asignar tarea" Width="99%" Style="margin:1px; display:none" CssClass="boton_azul"/>
                                  </div>
                                  
                                  <div id="Separacion" style="width: 100%; background-color: #f5f5f5"; margin-left: 3px">

                                      <asp:Label ID="Label_relacion_documentos" runat="server" Text="Documentos Digitalizados"  ForeColor="Black" Style="font-family:Arial; font-size:12px; font-weight: 600"></asp:Label>
                                  </div>

                                  <div id="Datos_Digitalizacion" style="width: 100%; height: 20%; border: thin double #000080; margin-left: 3px; display:none">
                                      <asp:DropDownList ID="DropDownListLista" runat="server" Width="94%">
                                      </asp:DropDownList>

                                  </div>
                                   
                                  <div id="seleccion_documentos_digitalizados" style="width:98%; float: left;  border: 5px; border-bottom-color: white; background-color: white; overflow: auto;  margin-top:5px; margin-left:0px; margin-right:0px; margin-bottom:5px">
                                      <asp:UpdatePanel ID="UpdatePanelseleccion_digitalizado" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                          <ContentTemplate>
                                             
                                                  <asp:TreeView ID="TreeViewseleccion_digitalizado" runat="server" NodeWrap="true"   style="width:99%; margin-top:5px; margin-left:0px; margin-right:0px; margin-bottom:5px"
                                                      NodeStyle-NodeSpacing="0.1px" Height="16px" LeafNodeStyle-CssClass="LeafNodeStyle_2" ExpandDepth="0" NodeIndent="1" Font-Size="9pt"  PopulateNodesFromClient="False" SkipLinkText="">
                                                      <HoverNodeStyle Font-Underline="True" />
                                                      <SelectedNodeStyle ForeColor="Red" />
                                                      <ParentNodeStyle Font-Bold="False" />
                                                      <HoverNodeStyle Font-Underline="True" ForeColor="Purple" />
                                                      <SelectedNodeStyle ForeColor="White" CssClass="node_select_" VerticalPadding="5px" HorizontalPadding="5px" />
                                                      <NodeStyle Font-Names="Tahoma" Font-Size="8pt" ForeColor="black" HorizontalPadding="1px"
                                                          NodeSpacing="1px" VerticalPadding="0px" />
                                                  </asp:TreeView>
                                              
                                          </ContentTemplate>
                                          <Triggers>
                                          </Triggers>
                                      </asp:UpdatePanel>

                                  </div>
                                  <input id="HiddenRuta" type="hidden" value="0" runat="server">
                                  <input id="HiddenIdFlujo" type="hidden" value="0" runat="server">
                              </ContentTemplate>
                              <Triggers>
                                  <asp:AsyncPostBackTrigger ControlID="ButtonAlmacenar" EventName="Click" />
                                  <asp:AsyncPostBackTrigger ControlID="ButtonEliminarArchivos" EventName="Click" />


                              </Triggers>
                          </asp:UpdatePanel>
                         <asp:UpdatePanel ID="UpdatePanelBotones" runat="server" UpdateMode="Conditional"  RenderMode="Inline" >
                              <ContentTemplate>

                                  <div id="Datos_Digitalizacion_botones" style="width: 98%; height:30px; float:left; position:inherit; margin-left: 0px;margin-top: 1px; overflow-y:auto; overflow-x:auto; border-top:solid 1px #ccc">
                                      <asp:ImageButton ID="ImagebutonActualizarA" runat="server" ToolTip="Actualizar documentos " ImageUrl="../workflow/imageneswf/NUEVA CONSULTA3.png" style="display:none"/>
                                      <a  style=" margin-left:2px; width:auto; color:black" title="Actualizar lista de documentos" href="#"  onclick="activa_boton_client_server('ImagebutonActualizarA')"><i style="width:25px; height:25px; display:none" class="far fa-redo-alt fa-2x"></i></a>
                                      <a  style=" margin-left:2px; width:auto; color:black" title="Eliminar Documento" href="#"  onclick="activa_boton_client_server('ButtonElimina')"><i style="width:25px; height:25px" class="fal fa-trash-alt fa-2x"></i></a>
                                      <asp:ImageButton ID="ButtonElimina" runat="server" Text="(X)" ToolTip="Eliminar Documento" AlternateText="(X)" ImageUrl="../workflow/imageneswf/eliminar2.png" OnClientClick='ConfirmMensaje("Desea eliminar el documento seleccionado");' style="display:none" />
                                      <a  style=" margin-left:2px; margin:2px; width:auto; color:black" title="Indice documento" href="#"  onclick="activa_boton_client_server('ImageButtonindice')"><i style="width:25px; height:25px; display:none" class="fal fa-info-square fa-2x"></i></a> 
                                      <asp:ImageButton ID="ImageButtonActivaClasifica" runat="server" ToolTip="Cambia tipo documento" ImageUrl="../workflow/imageneswf/Actualiza_indice.png"  Visible="true" Height="24px" style="display:none"/>
                                      <a  style=" margin-left:2px; width:auto; color:black" title="Tipifica documento" href="#"  onclick="activa_boton_client_server('ImageButtonActivaClasifica')"><i style="width:25px; height:25px" class="fal fa-sort-alpha-up fa-2x"></i></a>                                  
                                      <input id="Hidden_estado_display" type="hidden" value="0" runat="server">
                                      <input id="Hidden_estado_visor" type="hidden" value="" runat="server">
                                      <asp:ImageButton ID="ImageButton_adjunt" runat="server" Text="(V)" ToolTip="Adjuntar Documento" AlternateText="(V)" ImageUrl="../workflow/imageneswf/adjuntarcarpeta.png" Visible="True" OnClientClick="eliminar_ajaxtolkit();" style="display:none" />
                                       <a  style=" margin-left:2px; width:auto; color:black" title="Cargar archivo Documento" href="#"  onclick="activa_boton_client_server('ImageButton_adjunt')"><i style="width:25px; height:25px" class="far fa-cloud-upload fa-2x"></i></a> 
                                      <asp:ImageButton ID="ImageButtonVisibleEscaner" runat="server" Text="(V)" ToolTip="Interface de Digitalización" AlternateText="(V)" ImageUrl="../workflow/imageneswf/ESCANEAR INTERFACE ESCANER.png" OnClientClick="dispalyInterfaceEscaner(); " Visible="True" style="display:none" />
                                       <a  style=" margin-left:2px; width:auto; color:black" title="Interface de Digitalización" href="#"  onclick="dispalyInterfaceEscaner();"><i style="width:25px; height:25px" class="far fa-vote-yea fa-2x"></i></a> 
                                        <asp:ImageButton ID="ButtonVisua" runat="server" Text="(V)" ToolTip="Visualizar Documento" AlternateText="(V)" ImageUrl="../workflow/imageneswf/paginasola.png" 
                                          Visible="True"  style="width: 18px; display:none" />
                                      <a  style=" margin-left:2px; width:auto; color:black" title="Visualizar documento seleccionado" href="#"  onclick="activa_boton_client_server('ButtonVisua')"><i style="width:25px; height:25px" class="fal fa-image fa-2x"></i></a>
                                  </div>
                              </ContentTemplate>
                          </asp:UpdatePanel>
                                   <div id="div_estado" style="height: 20px; position:inherit; float:left; overflow-y:auto; overflow-x:auto">
                              <asp:UpdatePanel ID="UpdatePanel_estado_lista" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                  <ContentTemplate>
                                      <asp:Label ID="Label_estado_lista" runat="server" Text="" ForeColor="Black" Style="font-family: Arial; font-size: 11px"></asp:Label>
                                  </ContentTemplate>
                              </asp:UpdatePanel>
                          </div>
                      </div>
                      <input id="HiddenPROMP" type="hidden" value="1" runat="server"/>
                      <div id="div_cerrar" style="float: right; height: 20px; display:none; width:79%; margin-top:5px" class="modal_title_superior">
                           <asp:Button ID="Button_cerrar_visor" runat="Server" Text="X" CssClass="modal_boton_hiden"
                               Style="float: right" OnClientClick="prevent_cerrar(event,this);" ToolTip="Cerrar Ventana visualizador" />
                      </div>
                      <div id="Are_Digitalizacion" style="width: 79%; height: 100%; float: right; display:block; margin-left:1px" class="modal_content_back">
                           <asp:UpdatePanel ID="UpdatePanel_iframe_digitaliza" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                              <ContentTemplate>
                                   <iframe id="IframeDitaliza_" runat="server" frameborder="0" width="100%" scrolling="no" height="100%" ></iframe>
                                  </ContentTemplate>
                               </asp:UpdatePanel>
                         
                      </div>
                      <div id="Area_Visor" style="width: 50%; height: 100%; float: right; display: none" class="modal_content_back">
                          <asp:UpdatePanel ID="UpdatePanelIframevisor" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                              <ContentTemplate>                         
                                  <iframe id="IframeVisor_" runat="server" frameborder="0" width="100%" scrolling="no" height="100%"></iframe>
                              </ContentTemplate>
                          </asp:UpdatePanel>
                      </div>
                      <div id="invisible" style="display:none">
                          <!---Boton de enlace de documento para almacenamiento--->
                           <asp:Button ID="ButtonAlmacenar" Text="" runat="server" Height="20px" Visible="True"
                              Width="20px" />
                          <asp:Button ID="Buttonactualizar" runat="server" Text="(...)" ToolTip="Actualizar" ViewStateMode="Enabled" Height="0px" Width="0px" />
                          <!---Boton eliminar archivos digitalizados--->
                          <asp:Button ID="ButtonEliminarArchivos" runat="server" Height="0px" Visible="True"
                              Width="0px" />
                           <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                              <ContentTemplate>
                                 
                                  </ContentTemplate>
                               </asp:UpdatePanel>
                      </div>
                  </div>
              </asp:Panel>  
               <asp:UpdatePanel ID="UpdatePopupExt" runat="server" UpdateMode="Conditional">
                  <ContentTemplate>
                      <asp:ModalPopupExtender ID="ModalpopoenlaceExt" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button1" PopupControlID="PanelPopupExt"
                          CancelControlID="botonpopupshowExt">
                      </asp:ModalPopupExtender>
                      <div id="botactiva" style="display: none">
                          <asp:Button ID="Button1" runat="server" Text="Button" />

                          <asp:Button ID="Button6" runat="server" Text="show" />
                      </div>

                  </ContentTemplate>

              </asp:UpdatePanel>
                <!--lista_chequeo_tramite-->
        <div id="lista_chequeo_tramite">
            <asp:Panel ID="Panel_lista_chequeo_tramite" runat="server" Style="display: none; color: White; width: 50%; height: auto; margin: auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_lista_chequeo_tramite" runat="server" BehaviorID="Panel_lista_chequeo_tramite" TargetControlID="ButtonSalir_lista_chequeo_tramite" BackgroundCssClass="ModalBackgroud_gorund"
                    CancelControlID="Button_cerrar_lista_chequeo_tramite" PopupControlID="Panel_lista_chequeo_tramite">
                </asp:ModalPopupExtender>
                <div id="divcabecer2_lista_chequeo_tramite" class="modal_title_superior">
                    <asp:Label ID="Label_lista_chequeo_tramite" runat="server" Text="Tipo documental que desea Adjuntar" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton2_lista_chequeo_tramite" style="float: right">
                        <asp:Button ID="Button_cerrar_lista_chequeo_tramite" runat="Server" Text="X" CssClass="modal_boton_hiden"
                            ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_lista_chequeo_tramite" style="background-color: white; width: auto; height: auto; color: black; background-color: #FFFFFF"
                    class="modal_content_back">
                    <div id="Contenedorgrid" style="width: 99%; position: inherit; left: 0px; top: 0px; text-align: left; height: auto; margin-top: 1px; border-color: #b0c4de; border-width: 1px; border-style: ridge">
                        <asp:UpdatePanel ID="UpdateGeneral" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <input id="Hidden_0002" type="hidden" value="0" runat="server">
                                <input id="Hidden_0001" type="hidden" value="-1" runat="server">
                                <asp:Panel ID="Panel_principal" runat="server"
                                    Style="overflow: auto; width: 100%; min-height: 150px; max-height: 150px">
                                    <asp:GridView ID="data_grid_chequeo" runat="server" Style="position: inherit; font-family: Arial" AutoGenerateSelectButton="False" CssClass="filtrar" GridLines="None" Font-Size="12px" Width="100%">
                                        <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                        <HeaderStyle CssClass="GridviewScrollHeader_line_blanco" />
                                        <RowStyle CssClass="GridviewScrollItem_line" />
                                        <PagerStyle CssClass="GridviewScrollPager_line" />
                                    </asp:GridView>
                                </asp:Panel>

                            </ContentTemplate>

                        </asp:UpdatePanel>

                    </div>

                    <div style="margin-top: 1px; height: auto; text-align: right">
                        <asp:UpdatePanel ID="UpdatePanel_lista_chequeo" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <asp:TextBox ID="TextBox_contenido_busqueda_lista_cheq" runat="server" Style="width: auto; margin-left: 3px; margin-top: 5px" placeholder="Busqueda.." onkeypress="acti_busq_lista_cheq(event,this)"></asp:TextBox>
                                <input id="Hidden_list_cheo_acepta" type="hidden" value="" runat="server">
                                <asp:Button ID="Button_examinar_archivo_lista_chequeo" runat="server" Text="Aceptar" Style="margin-left: 5px; margin-top: 5px" CssClass="boton_azul" />
                                <asp:Button ID="Button_Actualizar_Lista_chequeo" runat="server" Text="Actualizar" Style="margin-top: 5px; display: none" CssClass="boton_azul" />
                                <asp:CheckBox ID="CheckBox_busqueda_list_cheq" runat="server" Style="display: none" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>

                    <div style="overflow: auto">
                        <asp:UpdatePanel ID="UpdatePanel_lista_chequeo_estado" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <asp:Label ID="Label_estado_lista_chequeo" runat="server" Text="Estado" Style="font-size: 12px; display: none"></asp:Label>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
                <asp:Button ID="Button_lista_chequeo_tramite" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                <asp:Button ID="ButtonSalir_lista_chequeo_tramite" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
            </asp:Panel>
        </div>
        <!--cargar documento!-->
          <div id="contenido_procesa_sube_documento_adjunto" style="clear:both" >
            <asp:Panel ID="Panel_sube_documento_adjunto" runat="server" Style="display:none; color: White; width: 50%; height: auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_sube_documento_adjunto" runat="Server" BackgroundCssClass="ModalBackgroud_gorund" TargetControlID="Button_sube_documento_adjunto"
                    PopupControlID="Panel_sube_documento_adjunto" CancelControlID="Button3_cerrar_adjunta" ></asp:ModalPopupExtender>
                <div id="Div_cabecera" class="modal_title_superior">           
                    <asp:Label ID="Label22" runat="server" Text="Adjuntar"  Style="float: left">
                    </asp:Label>
                    <div id="Div_title_sube_documento_adjunto"" style="float: right">
                        <asp:Button ID="Button3_cerrar_adjunta" runat="Server" Text="X" CssClass="modal_boton_hiden"
                             ToolTip="Cerrar ventana" />
                    </div>
                </div>            
                 <div id="Div_contenido_adjunta" style=" color: White; background-color: #FFFFFF; height: 100%; width: 100%" class="modal_content_back">
                       <asp:Panel ID="Panel_descarga_ajax" runat="server">                              
                        <div id="drop_zone_" style="width: 100%; height:auto; overflow: auto">
                            <asp:UpdatePanel ID="UpdatePanel_descarga" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>                              
                                    <asp:AjaxFileUpload ID="AjaxFileUpload_dowload" runat="server" ThrobberID="drop_zone_"
                                        ContextKeys="fred"
                                        AllowedFileTypes="tif,jpg,tiff,bmp,pdf"
                                        MaximumNumberOfFiles="1" OnClientUploadComplete="activa_boton_dowload" />
                                    <asp:Button ID="Button_guardar_desicion" runat="server" Text="Button" Style="display: none" />             
                                    &nbsp  
                                    <asp:Label ID="Label_estado_carga" runat="server" Text="Estado" Style="font-family: Arial; font-size: 10px"></asp:Label>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                   </asp:Panel>    
                      <asp:Button ID="Button_sube_documento_adjunto" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" style="display:none" />
                </div>
            </asp:Panel>     
        </div>
            <!--lista_chequeo_actualiza-->
          <div id="lista_chequeo_actualiza"> 
            <asp:Panel ID="Panel_lista_chequeo_actualiza" runat="server" Style="display:none; color: White; width: 50%; height: auto; margin: auto" CssClass="modal_content_general">

                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_lista_chequeo_actualiza" runat="server" BehaviorID="Panel_lista_chequeo_actualiza" TargetControlID="ButtonSalir_lista_chequeo_actualiza" BackgroundCssClass="ModalBackgroud_gorund"
                    CancelControlID="Button_cerrar_lista_chequeo_actualiza" PopupControlID="Panel_lista_chequeo_actualiza" ></asp:ModalPopupExtender>
                <div id="divcabecer2_lista_chequeo_actualiza"  class="modal_title_superior">
                   
                    <asp:Label ID="Label_lista_chequeo_actualiza" runat="server" Text="Cambiar tipo documento" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton2_lista_chequeo_actualiza" style="float: right">
                        <asp:Button ID="Button_cerrar_lista_chequeo_actualiza" runat="Server" Text="X" CssClass="modal_boton_hiden"
                             ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_lista_chequeo_actualiza" style="background-color: white; width: auto; height: auto; 
                  color: black; background-color: #FFFFFF" class="modal_content_back">  
                     
                        <div id="Contenedorgrid_edita" style="width: 99%; position: inherit; left: 0px; top: 0px; text-align: left; height: auto; margin-top: 1px; border-color: #b0c4de; border-width: 1px; border-style: ridge">
		                           <asp:UpdatePanel ID="UpdateGeneral_actualiza" runat="server" UpdateMode="Conditional" RenderMode="Inline">
		                               <ContentTemplate>
		                                   <input id="Hidden_0003" type="hidden" value="-1" runat="server">
		                                   <input id="Hidden_0004" type="hidden" value="1" runat="server">
		       
		                                   <div id="contenido_titulo_data_grid_title_actualiza" style="width: 100%; margin-top: 1px; border-color: #b0c4de; border-width: 1px; border-style: ridge;display:none">    
		                                       <asp:Label ID="Label23" runat="server" ForeColor="Black" Font-Size="12px" Style="font-weight: 600">Seleccione el tipo documento</asp:Label>
                                                   
		                                   </div>
		                                  
		                                   <asp:Panel ID="Panel_principal_actualiza" runat="server" 
		                                        Style="overflow: auto; width:100%; min-height:150px; max-height:150px">
		                                       <asp:GridView ID="data_grid_chequeo_actualiza" runat="server" Style="position: inherit; font-family:Arial" AutoGenerateSelectButton="False" CssClass="filtrar" GridLines="None" Font-Size="12px" Width="100%">
		                                           <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
		                                           <HeaderStyle CssClass="GridviewScrollHeader_line" />
		                                           <RowStyle CssClass="GridviewScrollItem_line" />
		                                           <PagerStyle CssClass="GridviewScrollPager_line" />  
		                                       </asp:GridView>
		                                   </asp:Panel>
		                                   
		                               </ContentTemplate>
		       
		                           </asp:UpdatePanel>
		       
                </div>
                    <div style="margin-top: 1px; height: auto; text-align: right">
                        <asp:UpdatePanel ID="UpdatePanel_lista_chequeo_actualiza" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <asp:Button ID="Button_Actualizar_item_lista" runat="server" Text="Actualizar" Style="float:right; margin-top:5px; margin-right:5px" CssClass="boton_azul"  />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <br />
                    <div style="overflow:auto">
                        <asp:UpdatePanel ID="UpdatePanel_lista_chequeo_estado_actualiza" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <asp:Label ID="Label_estado_lista_chequeo_actualiza" runat="server" Text="Estado" style="font-size:12px; display:none"></asp:Label>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
                 <asp:Button ID="Button_lista_chequeo_actualiza" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_lista_chequeo_actualiza" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
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
    <script  accesskey="javascript" type="text/javascript">

        AjaxFileUpload_change_text();

          </script>
</body>
</html>

