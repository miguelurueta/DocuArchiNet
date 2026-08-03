<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormDetalleRadicado.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormDetalleRadicado" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
   <title>Detalle radicado publico</title>
      <script src="../js/ui/jquery-3.4.1.min.js"></script>
    <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
      <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
      <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
      <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
      <link href="../Styles/Aplicaction.css" rel="stylesheet" />
      <script accesskey="javascript" type="text/javascript">   </script>
       <script src="../js/radicacion/WebFormDetalleRadicado.js"></script>
    
</head>
<body style="margin-top:1px">
    <form id="form1" runat="server">
          <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" EnablePageMethods="true">
            </asp:ScriptManager>
          <asp:UpdatePanel ID="UpdatePanel1" runat="server">
          </asp:UpdatePanel>
        <script accesskey="javascript" type="text/javascript">
            Sys.WebForms.PageRequestManager.getInstance().add_initializeRequest(InitializeRequest);
            Sys.Application.add_load(ApplicationLoadHandler)
            var elment_postbak;
            var value_element;
            function ApplicationLoadHandler(sender, args) {

                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(CheckStatus);

            }
            function InitializeRequest(sender, args) {
              
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

            }

            </script>
        <div id="contenido_detallle_radicado">
            <div id="contenido_titulo_detalle_respuesta" style="height: auto; width: 100%;text-align:center" class="title_sup_redon_">     
                <asp:UpdatePanel ID="UpdatePanel_combo_detalle" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="container">
                            <div id="row-title" class="row">
                                <div class="col-12">
                                    <h1 id="de_tal">Detalle respuesta</h1>                        
                                </div>
                                <div class="col-8">
                                    <asp:DropDownList ID="DropDownList_detalle_respuesta" CssClass="form-control" runat="server" Style="width: 50%; display:none" onChange="on_clik('Button_activa_detalle_tramite');"></asp:DropDownList>
                                </div>
                            </div>     
                        </div>
                             
                    </ContentTemplate>
                </asp:UpdatePanel>        
                <asp:UpdatePanel ID="UpdatePanel_boton_activa_detalle" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>        
                        <asp:Button ID="Button_activa_detalle_tramite" runat="server" Text="Button" style="display:none" />
                    </ContentTemplate>
                </asp:UpdatePanel>

            </div>
            
            <div id="div_gabinetes" style="background-color: white; width: 100%; height: 75%; margin-top:1px; overflow:auto" class=" container " >      
                <asp:UpdatePanel ID="UpdatePanel_detalle" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Panel ID="Panel_gabinetes" runat="server" Style="height: 100%; width: 99.8%; margin-bottom: 1px" CssClass="container">
                            <asp:Table ID="Table_detalle_respuesta" runat="server" Style="height: 100%; width: 100%" CssClass="table table-hover">
                                <asp:TableRow>
                                    <asp:TableCell>
                                        <asp:Label ID="Label9" runat="server" Text="RADICADO DEL TRAMITE" ></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell>
                                        <asp:Label ID="Label_RADICADO_TRAMITE" runat="server" Text="" ></asp:Label>
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell>
                                        <asp:Label ID="Label19_" runat="server" Text="PETICIONARIO O SOLICITANTE" ></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell>
                                        <asp:Label ID="LabelDESTINATARIO" runat="server" Text="" ></asp:Label>
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell>
                                        <asp:Label ID="Label8" runat="server" Text="TIPO TRÁMITE RADICADO" ></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell>
                                        <asp:Label ID="Label_TIPO_TRAMITE" runat="server" Text="" ></asp:Label>
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell>
                                        <asp:Label ID="Label15" runat="server" Text="FECHA DE RADICACION DEL TRAMITE" ></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell>
                                        <asp:Label ID="LabelFECHA_REGISTRO" runat="server" Text="" ></asp:Label>
                                    </asp:TableCell>
                                </asp:TableRow>
       
                                <asp:TableRow>
                                    <asp:TableCell>
                                        <asp:Label ID="Label1" runat="server" Text="FECHA LÍMITE PARA RESPONDER EL TRAMITE" ></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell>
                                        <asp:Label ID="Label_FECHA_VENCE" runat="server" Text="" ></asp:Label>
                                    </asp:TableCell>
                                </asp:TableRow>
                                 <asp:TableRow>
                                    <asp:TableCell>
                                        <asp:Label ID="Label20" runat="server" Text="ESTADO DEL TRAMITE " ></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell>
                                        <asp:Label ID="Label_tipo_respuesta_usuario" runat="server" Text="" ></asp:Label>
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell>
                                        <asp:Label ID="Label4" runat="server" Text="TIEMPO ESTIMADO DE RESPUESTA DEL TRAMITE" ></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell>
                                        <asp:Label ID="Label_TIEMPO_ESTIMADO_RESPUESTA" runat="server" Text="" ></asp:Label>
                                    </asp:TableCell>
                                </asp:TableRow>
                                 <asp:TableRow>
                                    <asp:TableCell>
                                        <asp:Label ID="Label_fecha_resp" runat="server" Text="FECHA EN QUE SE LE DIO RESPUESTA TRAMITE" ></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell>
                                        <asp:Label ID="Label_FECHA_RESPUETA" runat="server" Text="" ></asp:Label>
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell>
                                        <asp:Label ID="Label5" runat="server" Text="RADICADO CON EL CUAL SE RESPONDIO EL TRAMITE" ></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell>
                                        <asp:Label ID="Label_RADICADO_RELACIONADO_RESPUESTA" runat="server" Text="" ></asp:Label>
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell>
                                        <asp:Label ID="Label6" runat="server" Text="USUARIO QUE ELABORA(O) LA RESPUESTA" ></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell>
                                        <asp:Label ID="Label_USUARIO_ELABORO_RESPUESTA" runat="server" Text="" ></asp:Label>
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell>
                                        <asp:Label ID="Label7" runat="server" Text="CARGO USUARIO QUE ELABORA(O) LA RESPUESTA" ></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell>
                                        <asp:Label ID="Label_CARGO_USUARIO_ELABORO_RESPUESTA" runat="server" Text="" ></asp:Label>
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell>
                                        <asp:Label ID="Label10" runat="server" Text="MEDIO DE ENVÍO DE LA RESPUESTA " ></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell>
                                        <asp:Label ID="Label_MEDIO_ENVIO_RESPUESTA" runat="server" Text="" ></asp:Label>
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell>
                                        <asp:Label ID="Label11" runat="server" Text="CURRIER DE ENVÍO DE LA RESPUESTA " ></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell>
                                        <asp:Label ID="Label_CURRIER_ENVIO_RESPUESTA" runat="server" Text="" ></asp:Label>
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell>
                                        <asp:Label ID="Label12" runat="server" Text="CONSECUTIVO GUÍA DE ENVÍO DE LA RESPUESTA " ></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell>
                                        <asp:Label ID="Label_GUIA_CURRIER_RESPUESTA" runat="server" Text="" ></asp:Label>
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell>
                                        <asp:Label ID="Label13" runat="server" Text="FECHA DE ENVÍO DE LA RESPUESTA " ></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell>
                                        <asp:Label ID="LabelFECHA_ENVIO_RESPUESTA" runat="server" Text="" ></asp:Label>
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell>
                                        <asp:Label ID="Label16" runat="server" Text="ASUNTO DEL TRAMITE" ></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell>
                                        <asp:Label ID="LabelASUNTO" runat="server" Text="" ></asp:Label>
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell>
                                        <asp:Label ID="Label17" runat="server" Text="TIPO ENVIO PREVISTO " ></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell>
                                        <asp:Label ID="Label_tipo_envio_respuesta" runat="server" Text="" ></asp:Label>
                                    </asp:TableCell>
                                </asp:TableRow>
                                 <asp:TableRow>
                                    <asp:TableCell>
                                        <asp:Label ID="Label2" runat="server" Text="TIPO DE RESPUESTA PREVISTO " ></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell>
                                        <asp:Label ID="Label_tipo_respuesta" runat="server" Text="" ></asp:Label>
                                    </asp:TableCell>
                                </asp:TableRow>
                               
                                <asp:TableRow Style="display:none">
                                    <asp:TableCell>
                                        <asp:Label ID="Label18" runat="server" Text="ESTADO GRAFICO DEL TRAMITE " Style="font-family: arial; font-size: 9px; display:none"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell>
                                        <asp:Image ID="Image_estado_resp" runat="server" Style="height: 40px; width: 600px; display:none" />
                                    </asp:TableCell>
                                </asp:TableRow>
                                    <asp:TableRow>
                                    <asp:TableCell>
                                        <asp:Label ID="Label_fecha_envio" runat="server" Text="FECHA DE ENVÍO DE CORREO ELECTRÓNICO CON LA RESPUESTA" ></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell>
                                         <asp:Label ID="Label_FECHA_REGISTRO_EVIO_CORREO" runat="server" Text="" ></asp:Label>
                                    </asp:TableCell>
                                </asp:TableRow>

                            <asp:TableRow>
                                    <asp:TableCell>
                                        <asp:Label ID="Label_correo_electronico" runat="server" Text="CORREO ELECTRÓNICO NOTIFICADO CON LA RESPUESTA" ></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell>
                                        <asp:Label ID="Label_CORREO_NOTIFICACION" runat="server" Text="" ></asp:Label>
                                    </asp:TableCell>
                                </asp:TableRow>
                             <asp:TableRow>
                                    <asp:TableCell>
                                        <asp:Label ID="Label_fecha_recibido" runat="server" Text="FECHA DE CONFIRMACIÓN DE RECIBIDO CORREO ELECTRÓNICO CON LA RESPUESTA" ></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell>
                                        <asp:Label ID="Label_FECHA_CONFIRMACION_CORREO_RECIBIDO" runat="server" Text="" ></asp:Label>
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell>
                                        <asp:Label ID="Label14" runat="server" Text="NOTA RESPUESTA " ></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell>
                                        <asp:TextBox ID="TextBox_NOTA_RESPUESTA" runat="server" Style=" width:600px" TextMode="MultiLine"></asp:TextBox>
                                    </asp:TableCell>
                                </asp:TableRow>
                            </asp:Table>
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
                
            </div>
            <div id="contenido_botones" style="background-color: white; height: auto; margin-top: 1px; margin-bottom: 1px" class="container  justify-content-start">
                <asp:UpdatePanel ID="UpdatePanel_botones_registro" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <input id="Hidden_ruta_archivo" type="hidden" value="" runat="server">
                        <asp:Button ID="Button_generar" Style="float: right; margin-left: 5px; margin-top: 3px; margin-bottom: 3px" runat="server" Text="Descargar detalle" ToolTip="Descargar detalle de la respuesta" CssClass="btn  btn-success btn-sm" />
                        <asp:Button ID="Button_descarga_respuesta" Style="float: right; margin-top: 3px; margin-bottom: 3px" runat="server" Text="Descargar documento" CssClass="btn btn-success btn-sm align-item-end" />
                    </ContentTemplate>
                </asp:UpdatePanel>

            </div>
             
             <div id="tol_pie" style=" float:right;   width:100%; height:3%;border-style: ridge; border-bottom-width: 0.5px; border-left-width: 1px; border-right-width: 1px; border-top-width: 1px;text-align:center; display:none">
                 <asp:Label ID="Label_estado" runat="server" Text="" style="font-family:Arial;font-size:11px"></asp:Label>
                    <asp:UpdatePanel ID="updatapanel_iframe" runat="server" UpdateMode="Conditional" >
                            <ContentTemplate>
                                  <iframe runat="server" style="float:left" id="ifmExcel_" width="0" height="0" marginheight="0" marginwidth="0"
                            frameborder="0" />
                            </ContentTemplate>
                           
                 </asp:UpdatePanel>
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
        </div>
    </form>
</body>
</html>
