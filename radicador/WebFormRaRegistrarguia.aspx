<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormRaRegistrarguia.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormRaRegistrarguia" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Registrar guía</title>
     <script src="../js/ui/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
     <script src="../js/ScrollableGridPlugin.js"></script>   
    <script src="../js/ScrollableGridViewPlugin_ASP.NetAJAXmin.js" type="text/javascript"></script>
    <script src="../Fixed-Header-Table-master/gridviewScroll.min.js"></script>
   <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />   
    <script src="../js/radicacion/WebFormRegistraguia.js"></script>
    <script  accesskey="javascript" type="text/javascript">
    </script>
</head>
<body style="background-color:#A4A4A4; margin-top:0px">
    <form id="form1" runat="server">
    
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" EnablePageMethods="true">
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
                        //elmen.value = "Espere..."
                        elmen.disabled = true;
                    }
                }
                function CheckStatus(sender, args) {
                    try {
                  

                    if (elment_postbak.id = "Button_gestion_remitente_entrante") {
                        auto_zise_popup_plantilla_validacion();
                        //auto_zise_popup_editar_radicados();
                    }
                    }
                    catch (err) {
                        alert(err.message + " Funcion WebFormRaRegistrarguia ");
                    } finally {
                       
                        var elmen = document.getElementById(elment_postbak.id)
                        if (elmen.type == "button" || elmen.type == "image" || elmen.type == "submit") {
                            elmen.disabled = false;

                        }
                        progres_hiden('progres_bar');
                    }
                }

            </script>
        <div id="contendor_principal" style="height: 100%; width: 50%; background-color: #FAFAFA; margin: auto; border-radius: 25px; padding: 20px; background: white">          
            <div id="superior" style="width: 100%; height: auto" class="border_superior_radius">
                <asp:Label ID="Label5" runat="server" Text="Registra guía de envío de correspondencia" Style="font-family: Arial; font-size: 12px; color: black" CssClass="letra_titulo"></asp:Label>
            </div>
            <div id="contenido_registro" style="width: 100%; height: auto; overflow: auto; border: solid 1px #ccc">            
                <asp:UpdatePanel ID="UpdatePanel_procesa_tramite_envio" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Panel ID="_Panelvalidacion_val_radicacion" runat="server" Style="display: none">
                        </asp:Panel>
                        <input id="Hidden_height" type="hidden" value="0" runat="server">
                        <input id="Hidden_width" type="hidden" value="0" runat="server">
                        <table>
                            <tr>
                                <td>
                                    <asp:Label ID="Label_edita_tipo_tramite" runat="server" Text="Empresa envío trámite(*)" ForeColor="Black" Font-Names="arial" Font-Size="10" Style="" />
                                </td>
                                <td>
                                    <asp:DropDownList ID="DropDownList_procesa_tramite_envio" runat="server" Font-Size="10" Font-Names="arial" Style="width: 350px" onChange="on_clik('Button_tipo_empresa_guia');"></asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <asp:Label ID="Label_tramite" runat="server" Text="" Font-Size="9" Font-Names="arial" ForeColor="red" />
                                </td>

                            </tr>
                            <tr>

                                <td>
                                    <asp:Label ID="Label9" runat="server" Text="Código guía envío (*)" Font-Size="10" Font-Names="arial" ForeColor="black" />
                                </td>
                                <td>
                                    <asp:TextBox ID="TextBox_codigo_guia_envio" runat="server" Style="width: 200px"></asp:TextBox>
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

                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="Label4" runat="server" Text="Radicado relacionado(*)" Font-Size="10" Font-Names="arial" ForeColor="black" />
                                </td>
                                <td>
                                    <asp:TextBox ID="TextBox_RADICADO" runat="server" Style="width: 350px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="Label14" runat="server" Text="Operadores de mensajería interna" Font-Size="10" Font-Names="arial" ForeColor="black" />
                                </td>
                                <td>
                                    <asp:DropDownList ID="DropDownList_mensajero_interno" runat="server" Font-Size="10" Font-Names="arial" Style="width: 350px"></asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="Label2" runat="server" Text="Remitente de correspondencia(*)" Font-Size="10" Font-Names="arial" ForeColor="black" />
                                </td>
                                <td>
                                    <asp:DropDownList ID="DropDownList_remit_dest_interno" runat="server" Font-Size="10" Font-Names="arial" Style="width: 350px"></asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="Label15" runat="server" Text="Destinatario Correspondencia(*)" Font-Size="10" Font-Names="arial" ForeColor="black" />
                                </td>
                                <td style="border: solid 1px #ccc; padding: 3px 3px 3px 3px; background: #F5F5F5">
                                    <asp:TextBox ID="TextBox_NOMBRE_RAZON_SOCIAL" runat="server" Style="width: 96%; margin-right: 0px" CssClass="evendocument"></asp:TextBox>
                                    <br />
                                    <asp:Button ID="Button_Asigana_datos_validacion_edicion_manual_" Text="Validar" Style="margin-top: 5px; margin-left: 5px" ToolTip="Valida el nombre del destinatario" runat="server" CssClass="boton_azul" />
                                    &nbsp
                                    <asp:Button ID="Button_gestion_remitente_entrante" runat="server" Text="Gestionar" ToolTip="Gestiona el destinatario de la guía" CssClass="boton_azul" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="Label17" runat="server" Text="Dirección Destinatario(*)" Font-Size="10" Font-Names="arial" ForeColor="black" />
                                </td>
                                <td>
                                    <asp:TextBox ID="TextBox_DIRECCION" runat="server" Style="width: 350px" TextMode="MultiLine" MaxLength="50"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="Label16" runat="server" Text="Nit/identificación Destinatario" Font-Size="10" Font-Names="arial" ForeColor="black" />
                                </td>
                                <td>
                                    <asp:TextBox ID="TextBox_NIT_IDENTIFICACION" runat="server" Style="width: 350px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="Label19" runat="server" Text="Telefono Destinatario" Font-Size="10" Font-Names="arial" ForeColor="black" />
                                </td>
                                <td>
                                    <asp:TextBox ID="TextBox_TELEFONO" runat="server" Style="width: 350px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="Label20" runat="server" Text="Correo electrónico Destinatario" Font-Size="10" Font-Names="arial" ForeColor="black" />
                                </td>
                                <td>
                                    <asp:TextBox ID="TextBox_CORREO_ELECTRONICO" runat="server" Style="width: 350px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="Label22" runat="server" Text="Anexo guía" Font-Size="10" Font-Names="arial" ForeColor="black" />
                                </td>
                                <td>
                                    <asp:TextBox ID="TextBox_ANEXO" runat="server" Style="width: 350px" TextMode="MultiLine" MaxLength="50"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td></td>
                                <td style="text-align:right">
                                    <asp:UpdatePanel ID="UpdatePanel_botones" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <asp:Button ID="Button_restaurar" runat="server" Style="margin-left: 5px; margin-top: 5px" Text="Restaurar" ToolTip="Restaurar datos guía de envío" CssClass="boton_azul" />
                                            <asp:Button ID="Button_registrar" runat="server" Style="margin-top: 5px" Text="Registrar" ToolTip="Registrar guía de envío" CssClass="boton_azul" />
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                        </table>

                    </ContentTemplate>
                </asp:UpdatePanel>
                <div id="pie_tol" style="width: 100%; height: auto" class="border_titulo_inferior">
                    <asp:UpdatePanel ID="updatapanel_iframe" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Label ID="Label_result" runat="server" Text="" Style="font-size: 8px; font-family: Arial; float: right"></asp:Label>
                            <iframe runat="server" id="ifmExcel_" width="0" height="0" marginheight="0" marginwidth="0"
                                frameborder="0" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>

           
            <asp:UpdatePanel ID="UpdatePanel_bonones_ocultos" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Button ID="Button_tipo_empresa_guia" runat="server" Style="margin-top: 5px; margin-bottom: 5px; display: none" Text="Procesar" ToolTip="Procesa envío tramite correspondencia" CssClass="boton" />
                </ContentTemplate>
            </asp:UpdatePanel>
            <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 200px">
                <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
                Processing ...
            </div>
            <div id="botn_asignar" style="display: none">
                <asp:UpdatePanel ID="updatepanel_Asigana_datos_validacion_edicion" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Button ID="Button_Asigana_datos_validacion_edicion" Text="asignar" runat="server" />
                        <asp:Button ID="Button_Asigana_datos_validacion_edicion_manual" Text="Validar" ToolTip="Valida el nombre del destinatario" runat="server" CssClass="boton" />
                        &nbsp
                        <input id="Hidden_area_remitente_destinatario" type="hidden" value="-1" runat="server">
                        <input id="Hidden_remitente_destinario_interno" type="hidden" value="-1" runat="server">
                        <input id="Hidden_tipo_plantilla" type="hidden" value="" runat="server">
                        <input id="Hidden_nombre_plantilla_radicado" type="hidden" value="" runat="server">
                        <input id="Hidden_remitente_destinatario" type="hidden" value="-1" runat="server">
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
           
        </div>
         <div id="validacion_plantilla">
                <asp:Panel ID="Panel_valiacion_plantilla" runat="server" Style="display:none; color: White; width: 100%; height: 100%" CssClass="modal_content_general">
                    <asp:ModalPopupExtender ID="ModalPopupExtender_valiacion_plantilla" runat="Server" BackgroundCssClass="FondoAplicacion" Y="0" TargetControlID="ButtonSalir_valiacion_plantilla"
                        PopupControlID="Panel_valiacion_plantilla" CancelControlID="Button_cerrar_validacion_plantilla">
                    </asp:ModalPopupExtender>
                    <div id="divcabecer2_validacion_plantilla" class="modal_title_superior">                 
                        <asp:Label ID="Label7" runat="server" Text="Gestion externos" Font-Size="10" Style="float: left">
                        </asp:Label>
                        <div id="Divcerrarbuton2_validacion_plantilla" style="float: right">
                            <asp:Button ID="Button_cerrar_validacion_plantilla" runat="Server" Text="X" CssClass="modal_boton_hiden"
                                 ToolTip="Cerrar ventana" />
                        </div>
                    </div>
                    <asp:UpdatePanel ID="UpdatePanel_validacion_plantilla" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div id="Contenido_validacion_plantilla" style=" color: black; background-color: #FFFFFF; height: 90%; width: 100%" class="modal_content_back">
                                <iframe width="100%" height="100%" id="Iframe_validacion_plantilla_" runat="server" frameborder="0"></iframe>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <asp:Button ID="Button5" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                        <asp:Button ID="ButtonSalir_valiacion_plantilla" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                </asp:Panel>
            </div>
            <asp:UpdatePanel ID="UpdatePanel_imp_impresion" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <input id="Hiddendatoradicacion" type="hidden" value="" runat="server">
                    <input id="Hiddenruta" type="hidden" value="" runat="server">
                </ContentTemplate>
            </asp:UpdatePanel>
            <div id="ventanaimpreion">
                <asp:Panel ID="Panelimpresion" runat="server" Style="display: none; color: White; width: auto; height: auto" CssClass="modal_content_general">
                    <asp:DragPanelExtender ID="DragPanelExtenderimpre" runat="server" TargetControlID="Panelimpresion" />
                    <asp:ModalPopupExtender ID="ModalPopupExtenderimpre" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir"
                        PopupControlID="Panelimpresion" CancelControlID="Buttoncerrarimpre">
                    </asp:ModalPopupExtender>
                    <div id="divcabecer2" class="modal_title_superior">               
                        <asp:Label ID="Label3" runat="server" Text="Menú Impresión" Font-Size="10" Style="float: left">
                        </asp:Label>
                        <div id="Divcerrarbuton2" style="float: right">
                            <asp:Button ID="Buttoncerrarimpre" runat="Server" Text="X"  CssClass="modal_boton_hiden"
                                 ToolTip="Cerrar ventana" />

                        </div>
                    </div>

                    <asp:UpdatePanel ID="UpdatePaneliframe" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div id="ContenidoImpresion" style=" color: black; background-color: #FFFFFF; height: 150px; width:auto" class="modal_content_back">
                                <iframe width="100%" height="100%" id="ifimpre" frameborder="0" runat="server" src="../Gestion/WebFormimpresionfile.aspx"></iframe>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                     <asp:Button ID="Button1" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                        <asp:Button ID="ButtonSalir" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                </asp:Panel>
            </div>
    </form>
</body>
</html>
