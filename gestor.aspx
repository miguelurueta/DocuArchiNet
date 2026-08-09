<%@ Page Title="" Language="vb" AutoEventWireup="true" MasterPageFile="~/Site1.Master" CodeBehind="gestor.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.gestor" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" >  
    <link href="https://fonts.googleapis.com/css?family=Open+Sans:300,400,400i,600,700,800" rel="stylesheet"/>
    <link href="menu_scoope/assets/css/font-awesome.min.css" rel="stylesheet" />	
    <link href="menu_scoope/assets/css/linearicons.css" rel="stylesheet"/>
	<link href="menu_scoope/assets/css/simple-line-icons.css" rel="stylesheet"/>
	<link href="menu_scoope/assets/css/ionicons.css" rel="stylesheet"/>
	<link href="menu_scoope/assets/css/flag-icon.min.css" rel="stylesheet"/>
	<link href="menu_scoope/assets/css/fakeLoader.css" rel="stylesheet"/>	
    <link href="menu_scoope/assets/css/scoop-vertical.css" rel="stylesheet"/>
    <link href="menu_scoope/assets/css/jquery.mCustomScrollbar.css" rel="stylesheet"/>
    <link href="bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="bootstrap/js/bootstrap.min.js"type="text/javascript"></script>
    <link href="Styles/bootra-person.css" rel="stylesheet" /> 
      <link href="Styles/style.css" rel="stylesheet" />
    <link href="Styles/Aplicaction.css" rel="stylesheet" /> 
    <script src="js/java_general/general_code_java.js"type="text/javascript"></script>
    <script  src="Awesome/js/all.js" type="text/javascript"></script>
    <link href="Awesome/css/fontawesome.css" rel="stylesheet"/>
    <link href="Awesome/css/brands.css" rel="stylesheet"/>
    <link href="Awesome/css/solid.css" rel="stylesheet"/>
    <script  src="Awesome/js/brands.js"type="text/javascript"></script>
    <script  src="Awesome/js/solid.js"type="text/javascript"></script>
    <script  src="Awesome/js/fontawesome.js"type="text/javascript"></script>  
    <script src="js/workflow/gestor.js"type="text/javascript"></script> 
    
<!--===============================================================================================-->
	<link rel="stylesheet" type="text/css" href="colorlib/vendor/bootstrap/css/bootstrap.min.css"/>
<!--===============================================================================================-->
	<link rel="stylesheet" type="text/css" href="colorlib/fonts/font-awesome-4.7.0/css/font-awesome.min.css"/>
<!--===============================================================================================-->
	<link rel="stylesheet" type="text/css" href="colorlib/vendor/animate/animate.css"/>
<!--===============================================================================================-->	
	<link rel="stylesheet" type="text/css" href="colorlib/vendor/css-hamburgers/hamburgers.min.css"/>
<!--===============================================================================================-->
	<link rel="stylesheet" type="text/css" href="colorlib/vendor/animsition/css/animsition.min.css"/>
<!--===============================================================================================-->
	<link rel="stylesheet" type="text/css" href="colorlib/vendor/select2/select2.min.css"/>
<!--===============================================================================================-->	
	<link rel="stylesheet" type="text/css" href="colorlib/vendor/daterangepicker/daterangepicker.css"/>
<!--===============================================================================================-->
	<link rel="stylesheet" type="text/css" href="colorlib/css/util.css"/>
	<link rel="stylesheet" type="text/css" href="colorlib/css/main.css"/>
<!--===============================================================================================-->
    <script language="javascript" type="text/javascript">     
        if (document.documentMode) {
            if (document.documentMode <= 7) {
                alert("Estas en Internet Explorer con vista de compatibilidad es posible que no te funcionen algunas opciones del sistema");
            }
            
        } else {
           
        }
        
       
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlacenter" runat="server"  >
<form id="form1" runat="server" style="height:100%" defaultbutton="Buttonaceptar"  >
    <asp:ScriptManager ID="ScriptManager1" runat="server"      
        EnableScriptGlobalization="True">
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
                       posicion_update_pogres("progres_bar");
                       elment_postbak = args.get_postBackElement();
                       if (elment_postbak.id == "ContentPlacenter_Button_Aceptar") {
                           
                       }
                       var elmen = document.getElementById(elment_postbak.id)
                       if (elmen.type == "button" || elmen.type == "submit") {
                           value_element = elmen.value;
                           elmen.value = "Espere..."
                           elmen.disabled = true;
                       }
                   }
                   function CheckStatus(sender, args) {
                       try {
                           if (elment_postbak.type == "button" || elment_postbak.type == "submit") {
                               elment_postbak.value = value_element;
                               elment_postbak.disabled = false;
                           }
                          
                           if (elment_postbak.id == "ContentPlacenter_Button_sesion_publico" && document.getElementById("ContentPlacenter_Hidden_resul_ses_public").value == "YES") {
                               window.location.assign("Publico/WebFormDefaultPublico.aspx");
                           }
                       } catch (ex) {
                           alert("Funcion CheckStatus dice " + ex.message )
                       }
                       finally {
                           progres_hiden("progres_bar");
                       }
                   }
            </script>
        
    <div id="fakeLoader"></div>
    <div class="limiter">
        <div class="container-login100">
        <div id="div_center" class="wrap-login100" > 
            <div class="login100-form validate-form p-l-55 p-r-55 p-t-178">
            <asp:Panel ID="Panel1" runat="server"
                ViewStateMode="Enabled" Style="align-content: center; width: 99%">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <input id="Hiddenseleccion" type="hidden" value="" runat="server"/>
                        <input id="Hiddenempresagestion" type="hidden" value="" runat="server"/>
                        <input id="Hidden_resul_ses_public" type="hidden" value="" runat="server"/>       
                        <div class="login100-form-title modal_content_no_back_inferior_" style="background-color:white" >
                               <asp:Image ID="Imagesecion" runat="server" Height="70px"
                                ImageUrl="~/imagera/logo_trd.png" Style="height: 70px" />
                            
                        </div>
                        <div class="text-right p-t-13 p-b-23">
                            <span class="txt1">(PQRSD)
                            </span>
                            <a href="#" onclick="sesion_cli();" class="txt2"> Registre su petición, queja o reclamo
                            </a>
                        </div>                      
                        <asp:Button ID="Button_sesion_empresa" runat="server" Text="Aceptar" Style="display: none" />
                        <asp:Button ID="Button_sesion_publico" runat="server" CssClass="boton" Enabled="false" Text="Aceptar" Style="display: none" />
                        <asp:Panel ID="Panel_sesion_privado_login" runat="server" CssClass="login100-form validate-form p-l-5 p-r-5 p-t-7">
                            <div class="row mt-2">
                                <div class="col-12">
                                    <asp:DropDownList ID="DropDownListmodulos" runat="server" Style="width: 100%" CssClass="custom-select">
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="row mt-4" data-validate="Please enter username">
                                <div class="col-12">
                                    <asp:TextBox ID="TextBoxuser" runat="server" placeholder="Nombre de usuario" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row mt-4">
                                <div class="col-12">
                                    <div class="input-group">
                                        <asp:TextBox ID="TextBoxpasw" runat="server" placeholder="Contraseña" TextMode="Password" CssClass="form-control "></asp:TextBox>
                                        <div class="input-group-append">
                                            <button id="show_password" class="btn  btn-success" type="button" onclick="mostrarPassword()"><span class="fa fa-eye-slash icon"></span></button>
                                        </div>

                                    </div>
                                </div>
                            </div>
                            <div class="row mt-4">
                                <div class="col-12 modal-footer justify-content-center">
                                    <a class="btn  btn-success  font-weight-light" style="height: auto; min-width: 100%; font-size: 15px; padding: 14px" title="Ingresar a opción privada" href="#" onclick="activa_boton_client_server('ContentPlacenter_Buttonaceptar')"> INICIAR SESIÓN </a>
                                    <a class="btn btn-light  font-weight-light" style="height: auto; min-width: 140px; font-size: 14px; display: none" title="Actualizar conexión" href="#" onclick="activa_boton_client_server('ContentPlacenter_Buttonaceptar0')"><i class="fas fa-sync-alt fa-1x"></i>Actualizar </a>
                                    <asp:Button ID="Buttonaceptar" runat="server" Text="" ToolTip="" CssClass="boton" Style="display: none" />
                                </div>
                            </div>
                            <div>
                                <div class="flex-col-c p-t-17 p-b-40">
                                    <span class="txt1 p-b-9">Olvido su contraseña?
                                    </span>
                                    <div class="txt3">
                                        <asp:LinkButton ID="LinkButton_recupera_pw" runat="server" CssClass="txt3"> <i class="fas fa-key"></i> Recuperar</asp:LinkButton>
                                    </div>
                                </div>
                            </div>
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </asp:Panel>
            </div>
        </div>  
        </div>
    </div>
     <!--mensaje_progreso evento-->
            <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 200px">
                <img src="workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
                Processing ...
            </div> 
     <!--Popup recupera_pasw-->
    <asp:Panel ID="Panel_recupera_pasw" runat="server" Style="width: 40%; display: none; height: auto" CssClass="modal_content_general">
        <asp:ModalPopupExtender ID="ModalPopupExtender_recupera_pasw" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_recupera_pasw"
            PopupControlID="Panel_recupera_pasw" CancelControlID="ButtonCerrar_recupera_pasw">
        </asp:ModalPopupExtender>
        <div id="modal_content_Panel_recupera_pasw" class="modal-content_">
            <div id="divcabecer2_recupera_pasw" class="modal_title_superior_ modal-header">
                 <h6 class="modal-title d-inline ml-1 font-weight-bold">Recuperar contraseña</h6>
                <button type="button" value="ContentPlacenter_ButtonCerrar_recupera_pasw" class="close da_event_captive">&times;</button>
            </div>
            <div id="Cotenedor_recupera_pasw" style="background-color: #FFFFFF; height: 100%; width: 100%; border-top: none" class="modal_content_back modal-body">
                <div class="row mt-2">
                    <div style="text-align: center" class="col-12">
                        <p style="text-align: justify; margin-top: 2px" class="font-weight-light">Para recuperar su contraseña digite los datos a continuación, si estos datos son los correctos su contraseña será enviada al correo electrónico relacionado al usuario informado.</p>
                    </div>
                </div>
                <div class="row mt-2">
                    <div class="col-6" style="text-align:left">
                        <span class="font-weight-light ">Nombre de usuario (*)</span>
                    </div>
                    <div class="col-6">
                        <asp:TextBox ID="TextBox_loguin_usuario" CssClass="form-control" runat="server" Style="width: 100%"></asp:TextBox>
                    </div>
                </div>
                <div class="row mt-2">
                    <div class="col-6" style="text-align:left">
                        <span class="font-weight-light">Correo electrónico (*)</span>
                    </div>
                    <div class="col-6">
                        <asp:TextBox ID="TextBox_correo_electronico" CssClass="form-control" runat="server" Style="width: 100%"></asp:TextBox>
                    </div>
                </div>

            </div>
            <div class="modal-footer justify-content-end" id="modal-footer">
                <asp:UpdatePanel ID="UpdatePanel_recupera_pasw" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Button ID="Button_Cancelar" runat="server" Text="Cancelar" CssClass="boton_blanco" Style="float: right; display: none" />
                        <a class="btn   btn-primary" style="width: 200px;font-size:14px; padding:14px" title="Ingresar a opción privada" href="#" onclick="activa_boton_client_server('ContentPlacenter_Button_Aceptar')"><i class="fas fa-check-circle fa-1x"></i>Aceptar </a>
                        <asp:Button ID="Button_Aceptar" runat="server" Text="Aceptar" CssClass="boton_blanco" Style="float: right; display: none" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
        <div style="display: none; height: 1px">
            <asp:Button ID="ButtonCerrar_recupera_pasw" runat="Server" Text="" CssClass="" Style="display: none" />
            <asp:Button ID="ButtonSalir_recupera_pasw" runat="server" Text="" Height="0px" Width="0px" Style="display: none" />
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
    <script src="menu_scoope/assets/js/jquery.1.11.3.min.js" type="text/javascript"></script>
	<script src="menu_scoope/assets/js/lib/fakeLoader.js" type="text/javascript"></script>
    <script src="menu_scoope/assets/js/scoop.min.js" type="text/javascript"></script>
	<script src="menu_scoope/assets/js/lib/sparkline.min.js" type="text/javascript"></script>
	<script src="menu_scoope/assets/js/lib/jquery.mCustomScrollbar.concat.min.js" type="text/javascript"></script> 
	<script src="menu_scoope/assets/js/lib/jquery.mousewheel.min.js" type="text/javascript"></script>    
    <!--===============================================================================================-->
	<script src="colorlib/vendor/jquery/jquery-3.2.1.min.js" type="text/javascript"></script>
<!--===============================================================================================-->
	<script src="colorlib/vendor/animsition/js/animsition.min.js" type="text/javascript"></script>
<!--===============================================================================================-->
	<script src="colorlib/vendor/bootstrap/js/popper.js" type="text/javascript"></script>
	<script src="colorlib/vendor/bootstrap/js/bootstrap.min.js" type="text/javascript"></script>
<!--===============================================================================================-->
	<script src="colorlib/vendor/select2/select2.min.js" type="text/javascript"></script>
<!--===============================================================================================-->
	<script src="colorlib/vendor/daterangepicker/moment.min.js" type="text/javascript"></script>
	<script src="colorlib/vendor/daterangepicker/daterangepicker.js" type="text/javascript"></script>
<!--===============================================================================================-->
	<script src="colorlib/vendor/countdowntime/countdowntime.js" type="text/javascript"></script>
<!--===============================================================================================-->
	<script src="colorlib/js/main.js" type="text/javascript"></script>
    </form>
</asp:Content>
