<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormRespuestaFormalRadicado.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormRespuestaFormalRadicado" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <script src="../js/ui/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
      <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <script src="../js/radicacion/WebFormRespuestaFormalRadicado.js"></script>
    
    <script>
       
    </script>
    <style type="text/css">
        .auto-style1 {
            height: 26px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="True">
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
                 posicion_update_pogres('progres_bar');
                 elment_postbak = args.get_postBackElement();
                 var elmen = document.getElementById(elment_postbak.id)
                 if (elmen.type == "button" || elmen.type == "submit") {
                     value_element = elmen.value;
                     elmen.value = "Espere..."
                     elmen.disabled = true;
                 }
             }
             function CheckStatus(sender, args) {
                 progres_hiden('progres_bar');
                 if (elment_postbak.type == "button" || elment_postbak.type == "submit") {
                     elment_postbak.value = value_element;
                     elment_postbak.disabled = false;
                 }
                 if (document.getElementById("Hidden_estado_update").value == "-1") {
                     document.getElementById("Hidden_estado_update").value = "";
                     document.getElementById("Button_inicio_respuesta").click();
                 }
                 if (elment_postbak.id == "Button_examinar_destinatario") {
                     redimenciona_popup_gestion_externo();
                 }
             }

            </script>
        <div id="conten_general" style="float: initial; width:100%; height:100%">
            <div id="izq" style="float: left; width: 2%; height: 100%; left: auto; position: static; height: 150px">
                <input id="Hidden_estado_update" type="hidden" value="-1" runat="server">
                <input id="Hidden_height" type="hidden" value="0" runat="server">
                <input id="Hidden_width" type="hidden" value="0" runat="server">
                <asp:UpdatePanel ID="UpdatePanel_hiden_resp" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <input id="Hidden_radicado" type="hidden" value="" runat="server">
                        <input id="Hidden_id_respuesta" type="hidden" value="-1" runat="server">
                        <input id="Hidden_tipo_respuesta" type="hidden" value="-1" runat="server">
                        <asp:Button ID="Button_hident" runat="server" Text="" style="display:none"/>
                        <asp:Button ID="Button_inicio_respuesta" runat="server" Text="" style="display:none"/>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div id="derecho" style="float: right; width: 2%; height: 100%; position: static; height: 150px">
            </div>
            <asp:UpdatePanel ID="UpdatePanel_respuesta_documento" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div id="center" style="left: auto; width: 95%; height: 98%; background-color: White; float: right">
                        
                        <div>
                            
                            <table style="width: 100%">
                                <tr>
                                    <td colspan="2">
                                        <asp:UpdatePanel ID="UpdatePanel_image_semaforo" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <asp:Image ID="Image_estado_resp" runat="server"  style="height:60px; width:350px" />
                                            </ContentTemplate>

                                        </asp:UpdatePanel>
                                        
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <div id="title_resp" style="background-color: #b0c4de; height: 30px; align-content: center; text-align: center">
                                            <asp:Label ID="Label_text_title" runat="server" Text="Para elaborar una respuesta siga los siguientes pasos" Style="text-align: center; font-family: Arial; font-size: 20px"></asp:Label>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label_descarga_plantilla" runat="server" Text="1.Por favor descargue el documento plantilla con el protocolo de respuesta de la entidad aqui." Style="text-align: center; font-family: Arial; font-size: 16px"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Button ID="Button_descarga_plantilla" runat="server" Text="1.Descarga plantilla respuesta" Style="background-color: white; border-color: #b0c4de; height: 25px; width: 200px; text-align: center" CssClass="boton" />
                                    </td>


                                </tr>
                           
                                <tr>
                                    <td>
                                        <asp:Label ID="Label_carga_plantilla" runat="server" Text="2.Carque el documento de respuesta elaborado por usted con base a la plantilla del sistema." Style="text-align: center; font-family: Arial; font-size: 16px"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Button ID="Button_carga_plantilla" runat="server" Text="2.Carga plantilla respuesta" Style="background-color: white; border-color: #b0c4de; height: 30px; width: 200px; height: 25px; text-align: center" CssClass="boton"  OnClientClick="eliminar_ajaxtolkit();" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:UpdatePanel ID="UpdatePanel_combo_plantillas" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <asp:Label ID="Label_tile_plantillas" runat="server" Text="Lista plantillas" Style="text-align: center; font-family: Arial; font-size: 16px"></asp:Label>
                                                <asp:DropDownList ID="DropDownList_lista_plantillas" runat="server" Style="width: 700px"></asp:DropDownList>
                                            </ContentTemplate>

                                        </asp:UpdatePanel>
                                        
                                    </td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td colspan="2" style="border-color: #b0c4de;">

                                        <asp:Button ID="Button_descarga" runat="server" Text="Descargar" Style="background-color: white; border-color: #b0c4de; height: 25px; font-size: 16px" CssClass="boton" />
                                        &nbsp 
                            <asp:Button ID="Button_eliminar" runat="server" Text="Eliminar" Style="background-color: white; border-color: #b0c4de; height: 25px; font-size: 16px" CssClass="boton" />
                                        &nbsp 
                            <asp:Button ID="Button_solicita_autorizacion" runat="server" Text="Solicitar autorización" Style="background-color: white; border-color: #b0c4de; height: 25px; font-size: 16px" CssClass="boton" />
                                        &nbsp 
                            <asp:Button ID="Button_estado_autorizacion" runat="server" Text="Estado autorización" Style="background-color: white; border-color: #b0c4de; height: 25px; font-size: 16px" CssClass="boton" />
                                    </td>

                                </tr>
                                <tr>
                                    <td colspan="2"></td>
                                </tr>
                                
                                
                                
                                
                                <tr>
                                    <td>
                                        <asp:Label ID="Label_radcar_tramite" runat="server" Text="3. Para radicar y confirmar la respuesta, por favor presione aquí." Style="text-align: center; font-family: Arial; font-size: 16px"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Button ID="Button_radicar_tramite" runat="server" Text="3.Radicar y confirmar respuesta" Style="background-color: white; border-color: #b0c4de; height: 30px; width: 200px; height: 25px; text-align: center" CssClass="boton" />
                                    </td>
                                </tr>
                                
                               
                                <tr>
                                    <td>
                                        <asp:Label ID="Label_text_confirma" runat="server" Text="4.Para públicar  la respuesta al peticionario,  por favor presione aquí." Style="text-align: center; font-family: Arial; font-size: 16px"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Button ID="Button_confirmar_tramite" runat="server" Text="4.Publicar documento respuesta" Style="background-color: white; border-color: #b0c4de; height: 30px; width: 200px; height: 25px; text-align: center" CssClass="boton" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <div id="Div1" style="background-color: #b0c4de; height: 40px; align-content: center; text-align: center"/>
                                        <asp:Label ID="Label_estado_resultado" runat="server" Text="Estado" Style="font-size:11px; font-family:Arial"></asp:Label>
                                        </div>
                                    </td>

                                </tr>
                            </table>

                        </div>
                    </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        
       <div id="notifica_respuesta_edition_html">
            <asp:Panel ID="Panel_edition_html" runat="server" Style="display:none; color: White; width: 90%; height: 90%">

                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_html" runat="server" BehaviorID="Panel_edition_html_ModalPopupExtender" TargetControlID="ButtonSalir_edition_html"
                    CancelControlID="Button_cerrar_edition_html" PopupControlID="Panel_edition_html"></asp:ModalPopupExtender>
                <div id="divcabecer2_edition_html" class="cabecera2">
                    <asp:Button ID="Button_edition_html" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_edition_html" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Label ID="Label_edition_html" runat="server" Text="Edicion plantilla" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton2_edition_html" style="float: right">
                        <asp:Button ID="Button_cerrar_edition_html" runat="Server" Text="X"
                            ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_edition_html" style="background-color: white; width: 100%; height: 100%">
                    
                      <div>
                        <CKEditor:CKEditorControl ID="htmlEditor" runat="server">Hello &lt;b&gt;World!&lt;/b&gt;</CKEditor:CKEditorControl>
                    </div>
            
                    <input id="Button1" type="button" value="button" onclick="cargar();" />
                    <asp:UpdatePanel ID="UpdatePanel_edition_html" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Button ID="Button_cargar" runat="server" Text="Cargar"  />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                   
                </div>
            </asp:Panel>
        </div>
        <div id="sube_documento_respuesta">
            <asp:Panel ID="Panel_sube_documento_respuesta" runat="server" Style="display:none; color: White; width: 622px; height: 222px">

                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_sube_documento_respuesta" runat="server" BehaviorID="Panel_sube_documento_respuesta_ModalPopupExtender" TargetControlID="ButtonSalir_sube_documento_respuesta" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_sube_documento_respuesta" PopupControlID="Panel_sube_documento_respuesta"></asp:ModalPopupExtender>
                <div id="divcabecer2_sube_documento" class="cabecera2">
                    <asp:Button ID="Button_sube_documento_respuesta" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_sube_documento_respuesta" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Label ID="Label_sube_documento_respuesta" runat="server" Text="Sube respuesta" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton2_sube_documento_respuesta" style="float: right">
                        <asp:Button ID="Button_cerrar_sube_documento_respuesta" runat="Server" Text="X"
                            ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_sube_documento_respuesta" style="background-color: white; width: 100%; height: 100%;border: thin double #000080; color: black; background-color: #FFFFFF;">
                                
                    <div id="drop_zone_" style="width: 619px; height: 220px; overflow:auto">
                        <asp:UpdatePanel ID="UpdatePanel_descarga" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:AjaxFileUpload ID="AjaxFileUpload_dowload" runat="server" ThrobberID="drop_zone_"
                            ContextKeys="fred"
                            AllowedFileTypes="docx"
                            MaximumNumberOfFiles="1" OnClientUploadComplete="activa_boton_dowload" />
                                <asp:Button ID="Button_sube_documento" runat="server" Text="Button" style="display:none" />
                                
                                 &nbsp   <asp:Label ID="Label_estado_carga" runat="server" Text="Estado" style="font-family:Arial; font-size:10px"></asp:Label>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>       
                </div>
            </asp:Panel>
        </div>
        <div id="radica_documento_respuesta">
            <asp:Panel ID="Panel_radica_documento_respuesta" runat="server" Style="display:none; color: White; width: 800px; height: 300px">

                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_radica_documento_respuesta" runat="server" BehaviorID="Panel_radica_documento_respuesta_ModalPopupExtender" TargetControlID="ButtonSalir_radica_documento_respuesta" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_radica_documento_respuesta" PopupControlID="Panel_radica_documento_respuesta" y="5"></asp:ModalPopupExtender>
                <div id="divcabecer2_radica_documento" class="cabecera2">
                    <asp:Button ID="Button_radica_documento_respuesta" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_radica_documento_respuesta" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Label ID="Label_radica_documento_respuesta" runat="server" Text="Confirma y radica respuesta" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton2_radica_documento_respuesta" style="float: right">
                        <asp:Button ID="Button_cerrar_radica_documento_respuesta" runat="Server" Text="X"
                            ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_radica_documento_respuesta" style="background-color: white; width: 100%; height: 99%;border: thin double #000080; color: black; background-color: #FFFFFF;">
                                
                    
                        <asp:UpdatePanel ID="UpdatePanel_contenido_radica_documento" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <input id="Hidden_remitente_destinatario" type="hidden" value="-1" runat="server" checked="checked">
                                <asp:Button ID="Button_Asigana_datos_validacion_edicion" runat="server" Text="Button" style="display:none" />
                                <table style="width: 100%;">
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label1" runat="server" Text="Tramite*" Style="text-align: center; font-family: Arial; font-size: 14px"></asp:Label>

                                        </td>
                                         <td>
                                             <asp:DropDownList ID="RE_Descripcion_Documento" runat="server" style="width:600px"></asp:DropDownList>
                                         </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label3" runat="server" Text="Anexo*" Style="text-align: center; font-family: Arial; font-size: 14px"></asp:Label>
                                        </td>
                                        <td><asp:TextBox ID="TextBoxanexo" runat="server" Style="width:600px">NO</asp:TextBox></td>
                                       
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label_destinatario" runat="server" Text="Destinatario*" Style="text-align: center; font-family: Arial; font-size: 14px"></asp:Label>

                                        </td>
                                        <td>
                                            <asp:TextBox ID="RE_REMITENTE_COR_REMITENTE_COR_VARCHAR" runat="server" Style="background-color:yellow;width:600px" Enabled="False"></asp:TextBox> 
                                            <asp:UpdatePanel ID="UpdatePanel_bton" runat="server" RenderMode="Inline">
                                                <ContentTemplate>
                                                    <asp:Button ID="Button_examinar_destinatario" runat="server" Text="gestion" style="font-size:12px; display:none" CssClass="boton" OnClientClick="asigna_datos_heig_with()"/> 
                                                </ContentTemplate>
                                            </asp:UpdatePanel>

                                        </td>                           
                                    </tr>
                                    <tr>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" style=" align-content: center; margin-left:30px">
                                            <asp:CheckBox ID="CheckBox_envio_correo" runat="server" Style="text-align: center; font-family: Arial; font-size: 15px; margin-left:20px;color:red" Text="Confirmar respuesta al correo electrónico del peticionario" />
                                        </td>
                                      
                                    </tr>
                                    <tr style="align-content:center">
                                          <td colspan="2" style=" align-content: center;">
                                            <asp:CheckBox ID="CheckBox_envia_ventanilla" runat="server" Style="text-align: center; font-family: Arial; font-size: 15px; margin-left:20px;color:red" Text="Solicita al centro de envío de correspondencia el envío de la respuesta" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td><asp:Label ID="Label2" runat="server" Text="Por favor separe por comas(,) los correos electronicos, ejemplo pepito@gmail.com,juan@hotmail.com" Style="text-align: center; font-family: Arial; font-size: 12px"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td><asp:Label ID="Label4" runat="server" Text="Correos electronicos" Style="text-align: center; font-family: Arial; font-size: 14px"></asp:Label></td>
                                         <td>
                                            <asp:TextBox ID="TextBox_correo_electronico" runat="server" Style="width:600px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    
                                    <tr>
                                        <td>

                                        </td>
                                        <td style="float:left"><asp:Button ID="Button_confirmar" runat="server" Text="Aceptar" Style="background-color: white; border-color: #b0c4de; height: 30px; width: 200px; height: 25px; text-align: center" CssClass="boton" /> &nbsp &nbsp
                                                         
                                        </td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td>
                                             <asp:Label ID="Label5" runat="server" Text="Si produjo algún error y no se pudo notificar al correo electrónico intenta aquí" style=" font-family: Arial; font-size: 14px; color:red"></asp:Label> &nbsp &nbsp
                                            <asp:Button ID="Button_notificar_correo" runat="server" Text="Reintentar" Style="background-color: white; border-color: #b0c4de; height: 30px; width: 100px; height: 25px; text-align: center" CssClass="boton" />
                                            
                                        </td>
                                    </tr>
                                    
                                </table>
                                                         
                            </ContentTemplate>
                        </asp:UpdatePanel>
                         
                </div>
            </asp:Panel>
        </div>
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
                            <iframe width="100%" height="100%" id="Iframe_validacion_plantilla_" runat="server"  ></iframe>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
             </asp:Panel>
        </div>
        <div id="inferior_bajo_boton" style="width: 100%; height: 20%; background-color: #E7EDF5; display: none">
            <asp:UpdatePanel ID="updatapanel_iframe" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Label ID="Label_result" runat="server" Text="Estado" Style="font-size: 8px; font-family: Arial; float: right"></asp:Label>
                    <iframe runat="server" id="ifmExcel_" width="0" height="0" marginheight="0" marginwidth="0"
                        frameborder="0" />
                    <input id="Hidden_ruta_archivo" type="hidden" value="" runat="server" />
                </ContentTemplate>

            </asp:UpdatePanel>
        </div>
        <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 200px">
                <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
                Processing ...
            </div>
    </form>
</body>
</html>
