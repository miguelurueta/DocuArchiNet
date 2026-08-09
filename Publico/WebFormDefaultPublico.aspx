<%@ Page Language="vb" AutoEventWireup="false"  CodeBehind="WebFormDefaultPublico.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormDefaultPublico" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>    
   <script src="../js/ui/jquery-3.4.1.min.js"></script>  
      <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />   
    <script src="../js/Publico/WebFormDefaultPublico.js"></script>
    <link href="../ccs/themes/base/jquery.ui.all.css" rel="stylesheet" type="text/css" />
     <script  src="../Awesome/js/all.js"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet"/>
  <link href="../Awesome/css/brands.css" rel="stylesheet"/>
  <link href="../Awesome/css/solid.css" rel="stylesheet"/>
    <script  src="../Awesome/js/brands.js"></script>
  <script  src="../Awesome/js/solid.js"></script>
  <script  src="../Awesome/js/fontawesome.js"></script> 
    <script src="../js/MyJavaScriptFile.js"></script>  
    <script src="../js/java_general/general_code_java.js"></script>
     <link href="https://fonts.googleapis.com/css?family=Open+Sans:300,400,400i,600,700,800" rel="stylesheet"/>
    <link href="../menu_scoope/assets/css/font-awesome.min.css" rel="stylesheet" />	
    <link href="../menu_scoope/assets/css/linearicons.css" rel="stylesheet"/>
	<link href="../menu_scoope/assets/css/simple-line-icons.css" rel="stylesheet"/>
	<link href="../menu_scoope/assets/css/ionicons.css" rel="stylesheet"/>
	<link href="../menu_scoope/assets/css/flag-icon.min.css" rel="stylesheet"/>
	<link href="../menu_scoope/assets/css/fakeLoader.css" rel="stylesheet"/>
	<link href="../menu_scoope/assets/css/bootstrap.min.css" rel="stylesheet"/> 
    <link href="../menu_scoope/assets/css/scoop-vertical.css" rel="stylesheet"/>
    <link href="../menu_scoope/assets/css/jquery.mCustomScrollbar.css" rel="stylesheet"/>
	<script src="../menu_scoope/assets/js/scoop.min.js"></script>	
	<script src="../menu_scoope/assets/js/lib/sparkline.min.js"></script>
	<script src="../menu_scoope/assets/js/lib/jquery.mCustomScrollbar.concat.min.js"></script> 
	<script src="../menu_scoope/assets/js/lib/jquery.mousewheel.min.js"></script> 
    <script src="../menu_scoope/assets/js/vertical-demo.js"></script>
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
<body style="margin-top:0px; width:100%; margin-left:0px; height:100%; overflow:hidden; font-size:inherit">
    <form id="form1" runat="server" onkeypress="return caracter_especial(event,this)">
         <asp:ScriptManager ID="ScriptManager1" runat="server"
        EnableScriptGlobalization="True">
    </asp:ScriptManager>  
             
               <script accesskey="javascript" type="text/javascript">
                   Sys.WebForms.PageRequestManager.getInstance().add_initializeRequest(InitializeRequest);
                   Sys.Application.add_load(ApplicationLoadHandler)
                   var elment_postbak;
                   function ApplicationLoadHandler(sender, args) {

                       Sys.WebForms.PageRequestManager.getInstance().add_endRequest(CheckStatus);

                   }
                   function InitializeRequest(sender, args) {
                       //
                       elment_postbak = args.get_postBackElement();


                   }
                   function CheckStatus(sender, args) {
                      
                       
                   }

            </script>
        <div id="fakeLoader"></div>
        <div id="scoop" class="scoop">
            <div class="scoop-overlay-box"></div>
            <div class="scoop-container">
                <header class="scoop-header d-none" id="header_coop" style="position:relative">
                    <div class="scoop-wrapper">
                        <div class="scoop-left-header">
                            <div class="scoop-logo">
                                <a href="#"><span class="logo-icon"><i class="ion-stats-bars"></i></span>
                                    <span class="logo-text"><span class="hide-in-smallsize"></span></span></a>
                            </div>
                        </div>
                        <div class="scoop-right-header">
                            <div class="sidebar_toggle" style="display: none"><a href="javascript:void(0)"><i class="icon-menu"></i></a></div>
                            <div class="scoop-rl-header">
                                <ul>
                                    <li class="icons" style="display: none">
                                        <a href="javascript:void(0)"><i class="fa fa-envelope" aria-hidden="true"></i>
                                            <span class="scoop-badge badge-success">2</span>
                                        </a>
                                    </li>
                                    <li class="icons" style="display: none">
                                        <a href="javascript:void(0)"><i class="fa fa-bell" aria-hidden="true"></i>
                                            <span class="scoop-badge badge-danger">8</span>
                                        </a>
                                    </li>
                                    <li class="icons" style="display: none">
                                        <a href="javascript:void(0)"><i class="fa fa-tasks" aria-hidden="true"></i>
                                            <span class="scoop-badge badge-warning">8</span>
                                        </a>
                                    </li>
                                    <li class="icons hide-small-device" style="display: none">
                                        <a href="javascript:void(0)">
                                            <i class="fa fa-rss" aria-hidden="true"></i>
                                        </a>
                                    </li>
                                </ul>
                            </div>
                            <div class="scoop-rr-header">
                                <ul>
                                    <li class="icons" style="display: none">
                                        <a href="javascript:void(0)">
                                            <i class="fa fa-user" aria-hidden="true"></i>
                                        </a>
                                    </li>
                                    <li class="icons"  onclick="sesion_cli();" title="Cerrar sesión" style="display: none">
                                        <a href="javascript:void(0)">
                                            <i class="fa fa-sign-out" aria-hidden="true" title="Cerrar sesión"></i>
                                        </a>
                                    </li>

                                </ul>
                            </div>
                        </div>
                    </div>
                </header>
                <div id="opciones_seleccion" style="height: auto">
                   
                    <nav id="menu_var" class="navbar navbar-expand-sm nav_botota_person_gray modal_content_no_back_inferior" style="margin-bottom:1px">     
                        <div class=" row w-100" >     
                            <div class=" col-2  navbar-nav p-4  justify-content-end">
                                
                            </div>
                             <div class="col-6 navbar-nav p-4">
                                  <a href="javascript:void(0)" id="element_a_inicio" title="inicio (Acceso público)" onclick="even_diplay_ini();">
                                    <i style="color: #57b846" class="fad fa-house fa-2x float-left"></i>
                                  </a>
                                    <asp:Label ID="Label_title_seleccion" runat="server" Text="" CssClass="font-weight-light h6 d-none" Style="font-size: 13px; color: #6d7fcc"></asp:Label>
                              </div>
                            <div class="col-4">
                                <asp:Image ID="Image_empresa" runat="server" ImageUrl="../imagera/logo_trd.png" Style="float:right; height: 50px" />
                            </div>
                        </div>
                    </nav>
                </div>

                <div id="div_titulo_form" style="text-align: right; background-color: #dcdcdc; display:none" class="row-12 p-1">
                   
                </div>

                <div class="limiter" id="content_opcion_public" >
                    <div class="container-login100" style="min-height:auto; font-size:inherit">
                        <div id="div_center_" class="wrap-login100_person" style="overflow:auto">
                            <div class="validate-form p-l-25 p-r-25 p-t-17" >
                                <div id="card_general_ini_text_public"  style=" background-color: white; font-size:inherit">
                                    <div  class="row w-100">
                                        <div class="col-6">
                                            <h4 style="color: #57b846"> ACCESO PUBLICO </h4>
                                        </div>
                                        <div class="col-6">
                                            <h4 class="float-lg-right" style="color: #57b846"> Transparencia </h4>
                                        </div>
                                    </div>
                                    <hr />
                                    <p style="font-size: 14px; font-family: Arial; font-weight: bold">Estimado Usuario:</p>
                                    <p style="text-align: justify; font-size: 14px; font-family: Arial">Con el fin de brindarle una mejor atención, ponemos a su disposición las siguientes opciones públicas, a través de los cuales usted podrá acceder a servicios relacionados con nuestra gestión institucional:</p>
                                    <div class="row">
                                            <div class="col-12">
                                                <a class="ml-4 mt-2 coll_sap_active"  style="color:#fecd33;text-decoration:none" data-toggle="collapse" href="#collpase_group_card_chek_0001"  aria-expanded="false" ><i class="fas fa-caret-down fa-2x  "></i>            
                                                </a>
                                                <span  class="ml-2 mt-2 logo-text h6"> Opciones públicas</span>
                                                <span id="content_card_count" class="ml-2 mt-2" > 4 </span>
                                                <hr />
                                            </div>
                                     </div>
                                    <div class="row_tres_option_pqr_ card-columns collapse show" style="font-size:inherit" id="collpase_group_card_chek_0001">
                                        <div class="card wrap-login100_person">
                                            <div class="card-content">
                                                <div class="card-body car_cursor_person" title="Gestione su pqrsf, presione aquí " id="WF-CL-01_card_boton" onclick="menu_public_general('G-PQ');">
                                                    <div class="media d-flex">
                                                        <div class="align-self-center">
                                                            <i style="color: #57b846" class="far fa-mailbox fa-4x float-left "></i>
                                                        </div>
                                                        <div class="media-body text-right">
                                                            <h4 id="WF-CL-01_card_content">PQRSF</h4>
                                                            <span style="font-size: 12px">Gestionar pqrsf</span>
                                                            <br />
                                                            <span style="color: #57b846;font-size: 12px">Opción pública</span>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="card wrap-login100_person">
                                            <div class="card-content">
                                                <div class="card-body car_cursor_person" id="CR-RP-03_card_boton" title="Para consultar el estado de su radicado, presione aquí" onclick="menu_public_general('G-CER');">
                                                    <div class="media d-flex">
                                                        <div class="align-self-center">
                                                            <i style="color: #57b846" class="far fa-search fa-4x float-left "></i>
                                                        </div>
                                                        <div class="media-body text-right">
                                                            <h4 id="CR-RP-03_card_content">CONSULTA RADICADO</h4>
                                                            <span style="font-size: 12px">Consultar consecutivo radicado</span>
                                                            <br />
                                                            <span style="color: #57b846; font-size: 12px">Opción pública</span>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="card wrap-login100_person">
                                            <div class="card-content">
                                                <div class="card-body car_cursor_person" id="CR-GT-01_card_boton" title="formule su pqrsd personal, presione aquí " onclick="menu_public_general('G-CDP');">
                                                    <div class="media d-flex">
                                                        <div class="align-self-center">
                                                            <i style="color: #57b846" class="far fa-file-search fa-4x float-left "></i>
                                                        </div>
                                                        <div class="media-body text-right">
                                                            <h4 id="CR-GT-01_card_content">CONSULTA DOCUMENTOS</h4>
                                                            <span style="font-size: 12px">Consulta documentos públicos</span>
                                                            <br />
                                                            <span style="color: #57b846; font-size: 12px">Opción pública</span>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="card wrap-login100_person">
                                            <div class="card-content">
                                                <div class="card-body car_cursor_person" id="CR-GT-01_card_boton_" title="Consulte su pqrsd personal, presione aquí" onclick="menu_public_general('G-CEO');">
                                                    <div class="media d-flex">
                                                        <div class="align-self-center">
                                                            <i style="color: #57b846" class="far fa-info fa-4x float-left "></i>
                                                        </div>
                                                        <div class="media-body text-right">
                                                            <h4 id="CR-GT-01_card_content_">ENTIDADES OFICIALES</h4>
                                                            <span style="font-size: 12px">Consulta entidades oficiales</span>
                                                            <br />
                                                            <span style="color: #57b846; font-size: 12px">Opción pública</span>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div id="div_iframe" style="height: auto; margin-top: 0px; width: 100%; background-color:white">
                    <asp:UpdatePanel ID="UpdatePanel_iframe" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <iframe id="ifrm_ds_" runat="server" style="width: 100%; float: left" frameborder="0"></iframe>
                            <asp:Button ID="Button_carga_pqr_gestion_" runat="server" Text="Button" Style="display: none" />
                            <a href="#" id="Button_carga_pqr_gestion" onclick="menu_public_general('P-QR-G');"></a>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
        <div id="div_oculto" style="display:none">
            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Button ID="Button_diplay" runat="server" Text="Button"  OnClientClick="auto_zise_publico();"/>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
         <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 200px">
                <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
                Processing ...
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
         <div class="da_loading" id="container_loading_iframe" style="display: none">
            <div class="da_loading_center">Cargando</div>
            <div class="da_loader"></div>
        </div>
    </form>
    
</body>
</html>
