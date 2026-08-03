<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormConsultaPublico.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormConsultaPublico" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <script src="../js/ui/jquery-3.4.1.min.js"></script>  
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
    <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
    <link href="../ccs/themes/base/jquery.ui.all.css" rel="stylesheet" type="text/css" />
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <script  src="../Awesome/js/all.js"></script>
    <script  src="../Awesome/js/all.js"></script>
    <link href="../Awesome/css/fontawesome.css" rel="stylesheet"/>
    <link href="../Awesome/css/brands.css" rel="stylesheet"/>
    <link href="../Awesome/css/solid.css" rel="stylesheet"/>
    <script  src="../Awesome/js/brands.js"></script>
    <script  src="../Awesome/js/solid.js"></script>
    <script  src="../Awesome/js/fontawesome.js"></script>
    <script src="../js/java_general/general_code_java.js"></script>
    <script src="../js/Publico/WebFormConsultaPublico.js"></script>
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
    <form id="form1" runat="server">
         <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
    
     <div class="limiter">
         <div class="container-login100">
             <div class="wrap-login100_person">
                 <div class="validate-form p-l-35 p-r-35 p-t-17">
                     <div id="area_trabjo" >        
             <div id="div_gabinetes" style=" background-color:white; width:99%; height:auto">
                 <div class="row">
                     <div class="col-6">
                         <h5 style="color: #57b846">CONSULTA DOCUMENTOS</h5>
                     </div>
                     <div class="col-6 ">
                         <a href="javascript:void(0)" title="Atrás" class="float-right" onclick="activa_retroceso_principal();">
                             <i style="color: #57b846" class="far fa-arrow-left fa-2x float-left"></i>
                         </a>
                     </div>
                 </div>
                 <hr />
                 <div class="mt-2">
                      <asp:UpdatePanel ID="UpdatePanel_gabinetes" runat="server" UpdateMode="Conditional">
                     <ContentTemplate>       
                         <asp:Panel ID="Panel_gabinetes" runat="server" Style="height: 100%">
                             <p style="font-size:14px; font-family:Arial; font-weight:bold">Estimado Usuario:</p>
                             <p style="text-align: justify;font-size:16px; font-family:Arial">Los documentos están organizados por temas o gabinetes virtuales, estos a su vez están conformados por llaves de búsquedas o índices para recuperar los documentos, usted tendrá que informar la llave de búsqueda al sistema para recuperar el documento de su interés. .  A continuación seleccione el GABINETE.</p>     
                             <div class="row  mt-2">
                                 <div class="col-12">
                                     <asp:DropDownList ID="DropDownList_gabinetes" runat="server" CssClass="custom-select  w-100"></asp:DropDownList>
                                 </div>
                             </div>
                             <div class="row  mt-2">
                                 <div style="text-align: right" class="col-12 mb-4 mt-2">
                                     <a class="btn btn-success" style="width: 100px" title="Recuperar año de nacimiento o anualidad" href="#" onclick="activa_boton_client_server('ButtonConsultar')"> Aceptar </a>
                                     <asp:Button ID="ButtonConsultar" runat="server" Text="Consulta" Style="display: none" OnClientClick=" auto_zise_popup_consulta();" />
                                 </div>
                             </div>  
                         </asp:Panel>
                     </ContentTemplate>
                 </asp:UpdatePanel>
                 </div>
                
                <div id="tol_pie" style=" background-color:white; width:100%; height:5%; font-family:Arial"">
                 <asp:Label ID="Label_estado" runat="server" Text=""></asp:Label>
                </div>
             </div>           
        </div>
                 </div>
             </div>
         </div>
     </div>
         <div id="ventana_consulta">
                         <asp:Panel ID="Panel_consulta_documento" runat="server" Style="display: none; width: 100%; height: 100%" CssClass="modal_content_general">
                             <asp:ModalPopupExtender ID="ModalPopup_consulta_documento" runat="Server" Y="1" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_consulta_documento"
                                 PopupControlID="Panel_consulta_documento" CancelControlID="Buttoncerrar_consulta_documento">
                             </asp:ModalPopupExtender>
                             <div id="modal_content_Panel_consulta_documento" class="modal-content">
                                 <div id="divcabecer_consulta_documento" class="modal_title_superior_ modal-header">
                                     <h6 class="modal-title d-inline ml-1">Consulta de documentos públicos</h6>
                                     <button type="button" value="Buttoncerrar_consulta_documento" class="close da_event_captive">&times;</button>
                                 </div>
                                 <div id="Contenido_consulta_documento" style="background-color: #FFFFFF; height: 100%; width: 100%; overflow: hidden" class="modal_content_back">
                                     <asp:UpdatePanel ID="UpdatePaneliframe_consulta_documento" runat="server" UpdateMode="Conditional">
                                         <ContentTemplate>
                                             <iframe id="ifimpre_consulta_documento_" runat="server" style="overflow-x: hidden; width: 100%" scrolling="no"></iframe>
                                         </ContentTemplate>
                                     </asp:UpdatePanel>
                                 </div>
                                 <div style="display: none; height: 1px">
                                     <asp:Button ID="Button_consulta_documento" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                                     <asp:Button ID="ButtonSalir_consulta_documento" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                                     <asp:Button ID="Buttoncerrar_consulta_documento" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                                 </div>
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
    </form>
</body>
</html>
