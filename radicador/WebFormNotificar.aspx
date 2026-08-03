<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormNotificar.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormNotificar" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Notificar</title>
      <script src="../js/ui/jquery-3.4.1.min.js"></script>
    <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <link href="../tokenzize2/tokenize2.min.css" rel="stylesheet" />
    <script src="../tokenzize2/tokenize2.1.min.js"></script>
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
   <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />   
    <script src="../js/validate_campos.js"></script>
    <script src="../js/radicacion/WebFormNotificar.js"></script>
    
    
</head>
<body style="height:100%; width:98%">
    <form id="form1" runat="server" onkeypress="return caracter_especial(event,this)">
    
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" EnablePageMethods="true" AsyncPostBackTimeout="1500">
        </asp:ScriptManager>
        <script type="text/javascript" language="javascript">
            Sys.WebForms.PageRequestManager.getInstance().add_initializeRequest(InitializeRequest);
            Sys.Application.add_load(ApplicationLoadHandler)
            var elment_postbak;
            function ApplicationLoadHandler(sender, args) {
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(CheckStatus);
            }
            function InitializeRequest(sender, args) {              
                elment_postbak = args.get_postBackElement();
                if (elment_postbak.id = "Button_notificar") {
                    
                    document.getElementById("Button_notificar").value = "Cargado..";
                    document.getElementById("Button_notificar").disabled = true;
                    
                    
                }
                posicion_update_pogres('progres_bar');
            }
            function CheckStatus(sender, args) {
                try {
                    if (elment_postbak.id = "Button_notificar") {
                        document.getElementById("Button_notificar").value = "Enviar correo";
                        document.getElementById("Button_notificar").disabled = false;
                        if (document.getElementById("Hidden00001").value == "YES") {
                            document.getElementById("Hidden00001").value = "";
                            hiden_cierra_ventana_principal();
                        }
                    }
                } catch (e) {
                    alert("Funcion esstatus WebFormNotificar " + e.message);
                } finally { progres_hiden('progres_bar'); }
                
            }

        </script>
        <div class="container-fluid p-2">
            <div class="row pl-3 pr-3 pt-2">
                <div class="col-sm-12 pl-1 pr-1 pb-2 pt-2" style="background-color:#6d7fcc">
                    <div id="contenedor_titulo" class="panel-body " style="height: auto; ">
                        <asp:Label ID="Label_notificar" runat="server" Text="Notificar gestión" ForeColor="Black" Font-Names="arial" Font-Size="11" Style="margin-left: 1px; display: none"></asp:Label>
                        <asp:Label ID="Label9" runat="server" Text=" &#x2709; Notificar a correo electrónico" ForeColor="White" Font-Names="arial" Font-Size="11" Style="margin-left: 2px; font-weight: 600"></asp:Label>
                    </div>
                </div>
            </div>
            <div class="row mt-2" >
                <div class="col-sm-1">
                    <asp:Label ID="Label3" runat="server" Text="Para"  Style="margin-left: 1px" CssClass="h6 font-weight-light"></asp:Label>
                </div>
                <div class="col-sm-11">
                    <select class="tokenize-callable-demo" multiple style="width: 100%">
                    </select>
                </div>

            </div>
            <div class="row mt-2">
                <div class="col-sm-1">
                    <asp:Label ID="Label4" runat="server" Text="Asunto "  Style="margin-left: 1px" CssClass="h6 font-weight-light"></asp:Label>
                </div>
                <div class="col-sm-11">
                    <asp:TextBox ID="TextBox_asunto_notificacion" runat="server" CssClass="tokens-container form-control" Style="width: 100%"></asp:TextBox>
                </div>
            </div>
            <div class="row mt-2">
                <div class="col-sm-12">
                    <asp:TextBox ID="TextBox_nota_noti_ficacion" runat="server" Style="width: 100%; height: auto; overflow: auto; margin-top: 5px; margin-left: 1px" CssClass="tokens-container form-control"></asp:TextBox>
                </div>
            </div>
            <div class="row mt-2">
                <div class="col-sm-12" style="margin-top: 5px"> 
                    <asp:UpdatePanel ID="Updatepanel_contenido_botones" runat="server">
                        <ContentTemplate>
                            <input id="Hidden00001" type="hidden" value="" runat="server">
                             <input id="Hidden_text_user_correo" type="hidden" value="-1" runat="server">
                            <asp:Button ID="Button_notificar" runat="server" Text="Enviar correo" CssClass="btn btn-primary" ToolTip="Envía correo de notificación" Style="float: right; background-color:#6d7fcc" OnClientClick="asig_array_tokenize();" />
                            <asp:Button ID="Button_confgura_cuenta" runat="server" Text="Configurar" CssClass="boton" ToolTip="Configura cuenta de correo de envío" Style="margin-left: 10px; margin-top: 5px; display: none" />
                            <asp:CheckBox ID="CheckBox_tipo_envio_correo" Text="" Checked="false" runat="server" Style="float: left; margin-right: 5px" CssClass="h6 font-weight-light" />
                            <span class="ml-2 h6 font-weight-light">Envia solo el link de los archivos a compartir</span>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
        <div style="display:none">
                    <input id="Hidden_cuenta_correo_envio" type="hidden" value="" runat="server">
                        <input id="Hidden_id_tipo_cuenta" type="hidden" value="-1" runat="server">
                        <input id="Hidden_tipo_cuenta_correo" type="hidden" value="" runat="server">
                        <input id="Hidden_correo_envio_default" type="hidden" value="" runat="server">
                        <input id="Hidden_imagen_adjunta" type="hidden" value="" runat="server">
                        <input id="Hidden_asunto_notificacion" type="hidden" value="" runat="server">
                        <input id="Hidden_convierte_pdf" type="hidden" value="" runat="server">
                        <input id="Hidden_tipo_notificacion" type="hidden" value="" runat="server">
                        <input id="hdnEmailID_VAL" type="hidden" value="-1" runat="server">
                        <input id="Hidden_id_plantilla_radicado" type="hidden" value="" runat="server">
            </div>
            <asp:UpdatePanel ID="Updatepanel_contenedor_controles" runat="server">
                <ContentTemplate>
                    <br />
                    <asp:Label ID="Label2" runat="server" Text="Correo " Font-Names="arial" Font-Size="10" Style="margin-left: 10px; display:none"></asp:Label>
                    <asp:TextBox ID="TextBox_busca_correo" runat="server" Style="width: 250px; display:none"></asp:TextBox>
                    <asp:AutoCompleteExtender ID="TextBox_busca_correo_AutoCompleteExtender" runat="server" BehaviorID="TextBox_busca_correo_AutoCompleteExtender"  TargetControlID="TextBox_busca_correo"
                        MinimumPrefixLength="2"
                        EnableCaching="True"
                        CompletionSetCount="10"
                        CompletionInterval="50"
                        ServiceMethod="GetGuiaRadicaconasp"
                        ServicePath="../webservice/WebServiceRadicacion.asmx"
                        ContextKey="Correo_Electronico|remit_dest_interno"
                        UseContextKey="True"
                        OnClientShown="onDataShown"
                        CompletionListCssClass="completionList">
                    </asp:AutoCompleteExtender>
                    <asp:Button ID="Button_Agregar" runat="server" Text="Agregar" ToolTip="Agregar correo a la lista" CssClass="boton"  style="display:none"/>                          
                </ContentTemplate>
               
            </asp:UpdatePanel>
           
       
        <div id="div_opciones" style="display:none">
             <asp:CheckBox ID="CheckBox_imagen_adjunta" runat="server"  Text="Imagen adjunta" Font-Size="10" Font-Names="arial" Checked="true"/>
            &nbsp <asp:CheckBox ID="CheckBox_pdf" runat="server"  Text="Convierte pdf" Font-Size="10" Checked="true" Font-Names="arial" />
            &nbsp <asp:CheckBox ID="CheckBox_lectura" runat="server"  Text="Pdf sólo lectura" Font-Size="10" Checked="true" Font-Names="arial"/>
            &nbsp <asp:CheckBox ID="CheckBox_pasw" runat="server"  Text="Pdf pasword" Font-Size="10" Font-Names="arial"/>
        </div>
        
        <div id="configura_cuenta">
            <asp:Panel ID="Panel_configura_cuenta" runat="server" Style="display:none; color: White; width: 400px; height: 250px">

                <asp:ModalPopupExtender ID="ModalPopupExtender_configura_cuenta" runat="server" BehaviorID="Panel_configura_cuenta_ModalPopupExtender" TargetControlID="ButtonSalir_configura_cuenta"
                    CancelControlID="Button_cerrar_configura_cuenta" PopupControlID="Panel_configura_cuenta">
                </asp:ModalPopupExtender>
                <div id="divcabecer2_notifica_gestion" class="cabecera2">
                    <asp:Button ID="Button1" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_configura_cuenta" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Label ID="Label5" runat="server" Text="Configura cuenta correo" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton2_configura_cuenta" style="float: right">
                        <asp:Button ID="Button_cerrar_configura_cuenta" runat="Server" Text="X"
                            ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_configura_cuenta" style="background-color:white; border: thin double #000080;">
                    <asp:UpdatePanel ID="UpdatePane_configura_cuenta" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            &nbsp <asp:Label ID="Label6" runat="server" Text="Tipo de cuenta correo" ForeColor="Black" Font-Names="arial" Font-Size="10"></asp:Label>
                            <br />
                           &nbsp <asp:DropDownList ID="DropDownList_tipo_cuentas" runat="server" style="width:200px"></asp:DropDownList>
                            <br />                         
                            &nbsp <asp:Label ID="Label7" runat="server" Text="Cuenta correo" ForeColor="Black" Font-Names="arial" Font-Size="10"></asp:Label> <br />
                            &nbsp <asp:TextBox ID="TextBox_cuenta_correo" runat="server" style="width:250px; background-color:yellow" ReadOnly="true"></asp:TextBox>
                            <br />                         
                            &nbsp <asp:Label ID="Label8" runat="server" Text="Contraseña Cuenta" ForeColor="Black" Font-Names="arial" Font-Size="10"></asp:Label> <br />
                            &nbsp <asp:TextBox ID="TextBox_pasword" runat="server" style="width:200px"  TextMode="Password"></asp:TextBox>
                            <br />
                            <br />
                            
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <asp:UpdatePanel id="Updatepanel_configura_cuenta_botones" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            &nbsp <asp:Button ID="Button_actualiza_cuenta" runat="server" Text="Actualizar" CssClass="boton" style=" margin-bottom:10px" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                   
                </div>
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
         <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 200px">
            <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
              Processing ...
        </div>
   
    </form>
</body>
</html>
