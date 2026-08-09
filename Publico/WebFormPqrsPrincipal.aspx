<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormPqrsPrincipal.aspx.vb" enableEventValidation="false" Inherits="GestionDocumental_Docuarchi.net.WebFormPqrsPrincipal" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
     <script src="../js/ui/jquery-3.4.1.min.js"></script>  
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
      <link href="../Styles/Aplicaction.css" rel="stylesheet" /> 
    <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
      <script  src="../Awesome/js/all.js"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet"/>
  <link href="../Awesome/css/brands.css" rel="stylesheet"/>
  <link href="../Awesome/css/solid.css" rel="stylesheet"/>
    <script  src="../Awesome/js/brands.js"></script>
  <script  src="../Awesome/js/solid.js"></script>
  <script  src="../Awesome/js/fontawesome.js"></script>
    <script src="../js/java_general/general_code_java.js"></script>
    <script src="../js/Publico/WebFormPqrsPrincipal.js"></script>
     <script src="../generic_control/FileUploadHandler.js"></script>
    <link href="../generic_control/UploadFile.css" rel="stylesheet" />
    <script src="../js/validate_campos.js"></script>
    <script src="../js/java_general/ubicacion_code_java.js"></script>
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
        <asp:ScriptManager ID="ScriptManager1" runat="server"
        EnableScriptGlobalization="True">
    </asp:ScriptManager>  
             <!-- <script type="text/javascript" src="../jsUpdateProgress.js"></script>	-->
        <script accesskey="javascript" type="text/javascript">
                   Sys.WebForms.PageRequestManager.getInstance().add_initializeRequest(InitializeRequest);
                   Sys.Application.add_load(ApplicationLoadHandler)
                   var elment_postbak;
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
                           //value_element = elmen.value;
                           //elmen.value = "Espere..."
                           //elmen.disabled = true;
                       }
                   }
                   function CheckStatus(sender, args) {
                       progres_hiden('progres_bar');
                       var elmen = document.getElementById(elment_postbak.id)
                       if (elmen.type == "button" || elmen.type == "image" || elmen.type == "submit") {
                           elmen.disabled = false;
                           //elmen.value = value_element;
                       }
                       if (elment_postbak.id == "Button_buscar" || elment_postbak.id == "Button_anonimo") {
                           auto_zise_registro();
                           if (document.getElementById('Hidden_resultado_buscar').value == "YES") {
                               window.parent.document.getElementById('Button_carga_pqr_gestion').click();
                               document.getElementById('Hidden_resultado_buscar').value = "";
                           }
                       }
                       if (elment_postbak.id == "Button_registra_usuario") {
                           if (document.getElementById('Hidden_resultado_registro').value == "YES") {
                               window.parent.document.getElementById('Button_carga_pqr_gestion').click();
                               document.getElementById('Hidden_resultado_registro').value = "";
                           }
                       }
                       if (elment_postbak.id == "Button_actualizar_anualidad") {
                           if (document.getElementById('Hidden_resultado_actualizar').value == "YES") {
                               window.parent.document.getElementById('Button_carga_pqr_gestion').click();
                               document.getElementById('Hidden_resultado_actualizar').value = "";
                           }
                       }

                       if (elment_postbak.id == "Button_buscar") {
                           auto_zise_registro();
                       }
                   }

            </script>
        <div class="limiter">
            <div class="container-login100">
                <div id="div_center_" class="wrap-login100_person">
                    <div class="validate-form p-l-35 p-r-35 p-t-17">
                        <div id="div_center" >
                            <div id="div_logo_pqrs">
                                <div class="row">
                                    <div class="col-6">
                                        <h4 style="color: #57b846">PQRSF</h4>     
                                    </div>
                                    <div class="col-6 ">
                                        <a href="javascript:void(0)" title="Atrás" class="float-right" onclick="activa_retroceso_principal();">
                                            <i style="color: #57b846" class="far fa-arrow-left fa-2x float-left"> </i>
                                        </a>
                                    </div>
                                </div>
                                <hr />
                                <div class="row">
                                    <div class="col-12">
                                        <h6>Procedimiento de Peticiones, Quejas, Reclamos, Sugerencias y Denuncias</h6>
                                    </div>    
                                </div>   
                            </div>
                            <br />
                            <div id="div_contenido_general" class="content_text_pqrs">
                                <p style="font-size: 14px; font-family: Arial; font-weight: bold">Estimado Usuario:</p>
                                <p style="text-align: justify; font-size: 14px; font-family: Arial">Con el fin de brindarle una mejor atención, ponemos a su disposición los siguientes Canales de Comunicación, a través de los cuales usted podrá registrar sus solicitudes, quejas, reclamos y/o sugerencias sobre temas de nuestra competencia e información relacionada con nuestra gestión institucional:</p>
                                <div class="row_tres_option_pqr_ card-columns mt-4">
                                    <div class="card wrap-login100_person">
                                            <div class="card-content">    
                                                <div class="card-body car_cursor_person" title="formule y consulte pqrsd anónima, presione aquí " id="WF-CL-01_card_boton" onclick="activa_boton_client_server('Button_anonimo');">
                                                    <div class="media d-flex">
                                                        <div class="align-self-center">
                                                            <i style="color: #57b846" class="far fa-user-secret fa-2x float-left "></i>
                                                        </div>
                                                        <div class="media-body text-right">
                                                            <h4 id="WF-CL-01_card_content">PQRSF</h4>
                                                            <span>Formular y consultar</span>
                                                            <br />
                                                            <span style="color: #57b846">Anónima</span>
                                                        </div>
                                                    </div>
                                                </div>
                                              
                                            </div>
                                        </div>
                                     <div class="card wrap-login100_person">
                                            <div class="card-content" >
                                                <div class="card-body car_cursor_person" id="CR-GT-01_card_boton" title="formule y consulte su pqrsd personal, presione aquí " onclick="activa_boton_client_server('Button_registra_usuario_peticionario'); inicializa_control_registro();">
                                                    <div class="media d-flex">
                                                        <div class="align-self-center">
                                                            <i style="color: #57b846" class="far fa-pen fa-2x float-left "></i>
                                                        </div>
                                                        <div class="media-body text-right">
                                                            <h4 id="CR-GT-01_card_content">PQRSF</h4>
                                                            <span>Formular y consultar</span>
                                                             <br />
                                                            <span  style="color: #57b846"> Personal</span> 
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>              
                                </div>
                                <div id="div_botones" style="background-color: #E7EDF5; text-align: center; display: none">
                                    <asp:UpdatePanel ID="UpdatePanel_botones" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <asp:Button ID="Button_pagina_web" runat="server" Text="Pqrs a través de Página Web" CssClass="boton_blanco" ToolTip="Registre su PQRS a través de página web" OnClientClick="auto_zise_registro();" />
                                            &nbsp
                                        <asp:Button ID="Button_atencion_personal" runat="server" Text="Atención personalizada" CssClass="boton_blanco" ToolTip="Registre su PQRS en el centro de atención" />
                                            &nbsp
                                        <asp:Button ID="Button_atencion_telefonica" runat="server" Text="Atención telefónica" CssClass="boton_blanco" ToolTip="Registre su PQRS por medio de una llamada telefónica" />
                                            &nbsp
                                        <asp:Button ID="Button_anonimo" runat="server" Text="Usuario anónimo" CssClass="boton_blanco" Style="display: none" />
                                            <asp:Button ID="Button_activa_correo_info" runat="server" Text="Usuario anónimo" CssClass="boton_blanco" Style="display: none" />
                                        </ContentTemplate>

                                    </asp:UpdatePanel>
                                </div>

                                <p>A través del sistema de PQRSD; el interesado puede enviar<sup style="box-sizing: inherit;">1</sup>:</p>
                                <ul style="box-sizing: inherit; margin: 0px 0px 1.25rem 1.1rem; padding-right: 0px; padding-left: 0px; font-family: Arial; font-size: 14.4px; line-height: 1.3; list-style-position: outside; color: rgb(0, 0, 0); text-align: justify;">
                                    <li style="box-sizing: inherit; margin: 0px; padding: 0px; list-style-type: square;"><strong style="box-sizing: inherit; line-height: inherit;">Petición:</strong>&nbsp;Es el derecho fundamental que tiene toda persona a presentar solicitudes respetuosas a las autoridades por motivos de interés general o particular y a obtener su pronta resolución.<br>
                                        &nbsp;</li>
                                    <li style="box-sizing: inherit; margin: 0px; padding: 0px; list-style-type: square;"><strong style="box-sizing: inherit; line-height: inherit;">Queja:</strong>&nbsp;Es la manifestación de protesta, censura, descontento o inconformidad que formula una persona en relación con una conducta que considera irregular de uno o varios servidores públicos en desarrollo de sus funciones.<br>
                                        &nbsp;</li>
                                    <li style="box-sizing: inherit; margin: 0px; padding: 0px; list-style-type: square;"><strong style="box-sizing: inherit; line-height: inherit;">Reclamo:</strong>&nbsp;Es el derecho que tiene toda persona de exigir, reivindicar o demandar una solución, ya sea por motivo general o particular, referente a la prestación indebida de un servicio o a la falta de atención a una solicitud.<br>
                                        &nbsp;</li>
                                    <li style="box-sizing: inherit; margin: 0px; padding: 0px; list-style-type: square;"><strong style="box-sizing: inherit; line-height: inherit;">Sugerencia:</strong>&nbsp;Es la manifestación de una idea o propuesta para mejorar el servicio o la gestión de la entidad.</li>
                                </ul>
                                <hr/>

                                <ol>
                                    <li><strong>Sistema PQRSD (virtual): &nbsp;</strong>El sistema de PQRSF <span class="notranslate">(Peticiones, Quejas, Reclamos, Sugerencias y Denuncias) </span>permite radicar la denuncia, realizar seguimiento al trámite y visualizar la respuesta desde cualquier computador con acceso a internet. Ingrese al sistema siguiendo el siguiente enlace:  <a style="font-family: Arial; font-size: 14px; font-weight: 600; color: #57b846" href="#" onclick="activa_boton_client_server('Button_pagina_web')">Ingrese aquí </a>.</li>
                                </ol>
                                <ol>
                                    <li value="2"><strong>Sistema PQRSD (físico):</strong>&nbsp;Las denuncias pueden ser entregadas directamente en la recepción de las siguientes direciones en el link a continuación
                            <a style="font-family: Arial; font-size: 14px; font-weight: 600; color: #57b846" href="#" onclick="activa_boton_client_server('Button_atencion_personal')">Ingrese aquí </a>
                                        <br/>
                                        &nbsp;</li>


                                    <li value="3"><strong>Comunicación telefónica: </strong>En el siguiente link puede consultar los teléfonos disponibles.
                            <a style="font-family: Arial; font-size: 14px; font-weight: 600; color: #57b846" href="#" onclick="activa_boton_client_server('Button_atencion_telefonica')">Ingrese aquí </a>
                                        <br/>
                                        &nbsp;</li>
                                    <li value="4"><strong>Correo electrónico: </strong>En el siguiente link puede consutar la información del correo electrónico oficial
                            <a style="font-family: Arial; font-size: 14px; font-weight: 600; color: #57b846" href="#" onclick="activa_boton_client_server('Button_activa_correo_info')">Ingrese aquí </a>
                                    </li>
                                </ol>
                            </div>
                            <div id="div_estado" style="background-color: white; text-align: right; padding: 1px">
                                <asp:Label ID="Label_estado_inicio" runat="server" Text="" Style="font-family: Arial; font-size: 10px"></asp:Label>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
            <!--Popup registro_usuario-->
            <asp:Panel ID="Panel_registro_usuario" runat="server"   Style="width:70%; height:100%;  display:none" CssClass="modal_content_general_ wrap-login100_person">
                <asp:ModalPopupExtender ID="ModalPopupExtender_registro_usuario" runat="Server" BackgroundCssClass="FondoAplicacion_" TargetControlID="ButtonSalir_registro_usuario"
                    PopupControlID="Panel_registro_usuario" CancelControlID="ButtonCerrar_registro_usuario"></asp:ModalPopupExtender>
                <div id="modal_content_Panel_registro_usuario" class="modal-content_">
                    <div id="divcabecer2_radica_documento" class="modal_title_superior_ modal-header">
                        <h4 style="color: #57b846" class="modal-title d-inline ml-1">REGISTRO PQRSF</h4>
                        <button type="button"  value="ButtonCerrar_registro_usuario" title="Atrás" class="close da_event_captive"><i style="color: #57b846" class="far fa-arrow-left fa-1x float-left"> </i></button>          
                    </div>
                    <div id="Cotenedor_registro_usuario" style="background-color: white; width: 100%; height: 100%; border-top: none" class="modal_content_back modal-body">
                        <div id="title_reg_solict_pqrs" style="text-align: center" class="row m-100 mt-1">
                        </div>
                        <div class="row m-100 mt-1">
                            <div id="info_reg_solict_pqrs" class="col-12 d-none">
                                <p style="text-align: justify; margin-top: 2px; font-size: 13px; font-family: Arial"><strong>Información: &nbsp;</strong> A continuación digite su número de documento de identificación, seguido seleccione su año de nacimiento si es un ciudadano (Persona natural) o año de creación si es una Persona jurídica (entidad, empresa, asociación entre otras).</p>
                            </div>
                        </div>
                        <asp:UpdatePanel ID="UpdatePanelContenido" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                 <asp:Panel ID="_Panelvalidacion" runat="server" Style="width: auto; height: auto; overflow: auto" CssClass="pl-3 pr-3 mt-1 mb-1" >
                                    <!-- Tabs navs -->
                                    <ul class="nav nav-pills  justify-content-around" id="ex1" role="tablist">
                                        <li class="nav-item" >
                                            <a
                                                class="nav-link active"  onclick="estado_diaplay_boton(event,this)"
                                                id="ex3_tab_1_"
                                                data-toggle="tab"
                                                href="#ex3_tabs_1" style="color: #57b846"> Paso 1
                                                
                                            </a>
                                        </li>
                                        <li class="nav-item " >
                                            <a
                                                class="nav-link"  onclick="estado_diaplay_boton(event,this)"
                                                id="ex3_tab_2_"    
                                                href="#ex3_tabs_2"
                                                data-toggle="tab" style="color: #57b846"> Paso 2
                                                 
                                            </a>
                                            
                                        </li>
                                         <li class="nav-item " >
                                            <a
                                                class="nav-link"  onclick="estado_diaplay_boton(event,this)"
                                                id="ex3_tab_3_"    
                                                href="#ex3_tabs_3"
                                                data-toggle="tab" style="color: #57b846"> Paso 3
                                                 
                                            </a>
                                            
                                        </li>
                                    </ul>
                                   
                                    <!-- Tabs navs -->

                                    <!-- Tabs content -->
                                    <div class="tab-content pt-2" id="ex2-content">
                                    <div   class="tab-pane active" 
                                            id="ex3_tabs_1"> 
                                            <div class=" row p-2  title_pqrs" >
                                                <div class="col-4  ">
                                                   
                                                </div>
                                                <div class="col-4  justify-content-center font-weight-bold" style="color: #57b846">
                                                    DATOS DEL SOLICITANTE 
                                                </div>
                                                <div class="col-4">
                                                  
                                                </div>
                                            </div> 
                                            <asp:Panel ID="Panel_ex3_tabs_1" runat="server" Style="width: auto; height: auto; overflow: auto" CssClass="pl-3 pr-3 mt-1 mb-1" >
                                            </asp:Panel>
                                       </div>
                                        
                                     <div
                                            class="tab-pane" 
                                            id="ex3_tabs_2">
                                            <div class=" row p-2 title_pqrs" >
                                                <div class="col-4  ">
                                                   
                                                </div>
                                                <div class="col-4  justify-content-center font-weight-bold" style="color: #57b846">
                                                    DATOS DEL CONTACTO 
                                                </div>
                                                <div class="col-4  ">
                                                  
                                                </div>
                                            </div> 
                                            <asp:Panel ID="Panel_ex3_tabs_2" runat="server" Style="width: auto; height: auto; overflow: auto" CssClass="pl-3 pr-3 mt-1 mb-1" >
                                            </asp:Panel>
                                        </div>
                                             <div
                                            class="tab-pane"
                                            id="ex3_tabs_3">
                                            <div class=" row p-2 title_pqrs">
                                                <div class="col-4  ">
                                                </div>
                                                <div class="col-4  justify-content-center font-weight-bold" style="color: #57b846">
                                                    REGISTRO SOLICITUD 
                                                </div>
                                                <div class="col-4  ">
                                                </div>
                                            </div>
                                            <asp:Panel ID="Panel_ex3_tabs_3" runat="server" Style="width: auto; height: auto; overflow: auto" CssClass="pl-3 pr-3 mt-1 mb-1">
                                                <div id="registro_pqr" class=" mb-4" style="border: none">
                                                    <div class="row w-100 mt-1">
                                                        <div class="col-4">
                                                            <asp:Label ID="Label_solicitud" runat="server" Text="Tipo de solicitud *" CssClass="h6 font-weight-light"></asp:Label>
                                                        </div>
                                                        <div class="col-8">
                                                            <asp:DropDownList ID="DropDownList_tipo_tramite" runat="server" CssClass="w-50 custom-select"></asp:DropDownList>
                                                        </div>
                                                    </div>
                                                    <div class="row w-100 mt-1">
                                                        <div class="col-4">
                                                            <asp:Label ID="Label2" runat="server" Text="Dependencia/Área *" CssClass="h6 font-weight-light"></asp:Label>
                                                        </div>
                                                        <div class="col-8">
                                                            <asp:DropDownList ID="DropDownList_area_dependencia" runat="server" CssClass="w-60 custom-select"></asp:DropDownList>
                                                        </div>
                                                    </div>
                                                    <div class="row w-100 mt-1">
                                                        <div class="col-4">
                                                            <asp:Label ID="Label7" runat="server" Text="Asunto Solicitud *" CssClass="h6 font-weight-light"></asp:Label>
                                                        </div>
                                                        <div class="col-8">

                                                            <asp:TextBox ID="TextBox_asunto" runat="server" Style="width: 100%" CssClass="form-control" TextMode="MultiLine" Rows="1" MaxLength="50"></asp:TextBox>

                                                        </div>
                                                    </div>
                                                    <div class="row w-100 mt-1">
                                                        <div class="col-4">
                                                            <asp:Label ID="Label9" runat="server" Text="Descripción solicitud*" CssClass="h6 font-weight-light"></asp:Label>
                                                        </div>
                                                        <div class="col-8">

                                                            <asp:TextBox ID="TextBox_descripcion" CssClass="form-control" runat="server" Style="width: 100%; height: 100%; min-height: 100px" TextMode="MultiLine" Rows="3" MaxLength="250"></asp:TextBox>

                                                        </div>
                                                    </div>
                                                    <div class="row w-100 p-2">
                                                        <div class="col-4">
                                                            <asp:DropDownList ID="DropDownList_anexos_respuesta" runat="server" Style="" CssClass="custom-select"></asp:DropDownList>

                                                        </div>
                                                        <div class="col-2">
                                                            <a title="Adjuntar prueba" onclick="inicializa_tipo_adjunto_documento(event,this, 'S-D-A');" class="btn  btn-warning  mt-1"><i class="fad fa-arrow-from-bottom"></i></a>
                                                            <a title="Eliminar documento" onclick="creaal_file()" style="color: white" class="btn  btn-danger  mt-1"><i class="fal fa-times"></i></a>
                                                        </div>
                                                        <div class="col-6">
                                                            <div class="form-check form-check-inline">
                                                                <input class="form-check-input" type="radio" name="flexRadioDefault" id="flexRadioDefault1" checked>
                                                                <label class="form-check-label" for="flexRadioDefault1">
                                                                    Notificación a correo electrónico
                                                                </label>
                                                            </div>
                                                            <div class="form-check form-check-inline">
                                                                <input class="form-check-input" type="radio" name="flexRadioDefault" id="flexRadioDefault2" >
                                                                <label class="form-check-label" for="flexRadioDefault2">
                                                                    Notificación a correo físico
                                                                </label>
                                                            </div>
                                                        </div>

                                                    </div>
                                                    <div class="row w-100 p-2">
                                                         <div class="col-12">
                                                              <p   style="font-size:9px; text-size-adjust:auto">
                                                                "Al hacer clic el botón enviar, usted acepta la remisión de la PQRS a la CAMARA DE COMERCIO DE VILLAVICENCIO. Sus datos serán recolectados y tratados conforme con la Política de Tratamiento de Datos. Para consultar el estado de su respuesta lo podrá hacer a través en este link.  http://localhost/GestionDocumental-Docuarchi.net/Publico/WebFormDefaultPublico.aspx opción CONSULTA RADICADO </p>      
                                                               <p   style="font-size:8px; text-size-adjust:auto">      
                                                                En caso que la solicitud de información sea de naturaleza de identidad reservada, deberá efectuar el respectivo trámite ante la Procuraduría General de la Nación, haciendo clic en el siguiente link: https://www.procuraduria.gov.co/portal/solicitud_informacion_identificacion_reservada.page
                                                               Se deberá indicar los términos que aplican en la presentación de quejas anónimas, para lo cual, se deben indicar las condiciones para aceptarlas conforme con la siguiente normativa: artículo 38 de la Ley 190 de 1995; artículo 69; de la Ley 734 de 2002 y artículo 81 de la Ley 962 de 2005".
                                                            </p>
                                                         </div>
                                                    </div>
                                                </div>
                                            </asp:Panel>
                                        </div>
                                    </div>
                                    <!-- Tabs content -->
                                    <asp:Table ID="_ValidacionConsulta" runat="server" BackColor="White" ForeColor="Black" ViewStateMode="Enabled" Wrap="false">
                                    </asp:Table>

                                </asp:Panel>
                                 </ContentTemplate>
                               <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="Buttonllenardepartamento" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="Buttonllenarciudad" EventName="Click" />
                            </Triggers>
                        </asp:UpdatePanel>                 
                                <div id="_campos_seleccion" style="height: 0px; display: none">
                                    <asp:TextBox ID="TextBoxEditNombreDestRem" runat="server"></asp:TextBox>
                                    <input id="Hiddenselecionpais" runat="server" type="hidden" value="COLOMBIA" />
                                    <input id="Hiddenseleciondepartamento" runat="server" type="hidden" value="" />
                                    <input id="Hiddenvalidacion" runat="server" type="hidden" value="" />
                                    <input id="Hiddenmunicipio" runat="server" type="hidden" value="" />
                                    <input id="Hiddenestadoedicion" runat="server" type="hidden" value="0" />
                                    <input id="Hiddenrelacionvalidacion" runat="server" type="hidden" value="-1" />                
                                </div>          
                    </div>
                    <div id="contenido_controles_buton_registro" class="modal-footer   justify-content-end" style="min-height:75px"> 
                      
                         <a class="btn btn-success" id="registro_actualiza_dext_externo_001" style="margin-left: 10px; margin: 5px; width: 150px; float: right; margin-right: 15px" title="Guardar datos de usuario" href="#" onclick="event_element_clic(event, this);"><i class="fas fa-check-circle"></i>  Siguiente </a>
                         <a class="btn btn-success" id="registro_actualiza_dext_externo_003" style="margin-left: 10px; margin: 5px; width: 150px; float: right; margin-right: 15px; display:none" title="Guardar datos de contacto" href="#" onclick="event_element_clic(event, this);"><i class="fas fa-check-circle"></i>  Siguiente </a>
                         <a class="btn btn-success" id="registro_actualiza_dext_externo_004" style="margin-left: 10px; margin: 5px; width: 150px; float: right; margin-right: 15px; display:none" title="Guardar datos de usuario" href="#" onclick="event_element_clic(event, this);"><i class="fas fa-check-circle"></i>  Aceptar </a>    
                    </div>
                    <div style="display: none; height: 1px">
                        <asp:Button ID="ButtonSalir_registro_usuario" runat="server" Height="0px" Style="display: none" Text="Button" Width="0px" />
                        <asp:Button ID="ButtonCerrar_registro_usuario" runat="Server" Text="X" Height="0px" Style="display: none" Width="0px"
                            ToolTip="Cerrar ventana" />
                        <asp:UpdatePanel ID="UpdatePanel_botones_validacion" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>    
                                 
                                <asp:Button ID="Button_registra_usuario" runat="server" CssClass="boton_azul" Text="Guardar" ToolTip="Guardar datos de usuario" Style="width: 100px; float: right; margin: 5px; display: none" />  
                                <asp:Button ID="Button_regresar_registro" runat="server" Text="Regresar" ToolTip="Regresar" Style="width: 100px; float: left; margin: 5px; display: none" />
                                <asp:Button ID="Button_ejecutar_consulta" runat="server" CssClass="boton" Style="display: none" Text="Limpiar" ToolTip="Limpiar campos radicacion" Width="100px" />
                                <input id="hdnEmailID_VAL" runat="server" type="hidden" value="-1" />
                                <input id="Hidden_resultado_registro" runat="server" type="hidden" value="" />
                                </input>
                                   </input>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>

                </div>  
            </asp:Panel>
        <asp:UpdatePanel ID="UpdatePanel_botones_configuracion" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div id="botones_control_autollenar" style="height: 0px; display: none">
                    <asp:Button ID="Buttonllenardepartamento" runat="server" Text="Button" BackColor="Silver" />
                    <asp:Button ID="Buttonllenarciudad" runat="server" Text="Button" />
                    <asp:Button ID="Button_anexo_cargar" runat="server" Text="" Style="width: 0px; height: 0px; display: none" CssClass="boton" />
                    <asp:Button ID="Button_descargar_anexo" runat="server" Text="" Style="width: 0px; height: 0px; display: none" CssClass="boton" />
                    <asp:Button ID="Button_anexo_eliminar" runat="server" Text="" Style="width: 0px; height: 0px; display: none" CssClass="boton" />
                    <asp:Button ID="Button_registra_usuario_peticionario" runat="server" Text="Button" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
            <!--Popup validacion_usuario-->
            <asp:Panel ID="Panel_validacion_usuario" runat="server" ForeColor="White"  Style=" width: 50%;  display:none; height:auto">
                <asp:ModalPopupExtender ID="ModalPopupExtender_validacion_usuario" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_validacion_usuario"
                    PopupControlID="Panel_validacion_usuario" CancelControlID="ButtonCerrar_validacion_usuario">
                </asp:ModalPopupExtender>
                <div id="modal_content_Panel_validacion_usuario" class="modal-content">
                    <div id="divcabecer2_validacion_usuario" class="modal_title_superiorr_ modal-header">
                         <h6 class="modal-title d-inline ml-1">Archivar</h6>
                         <button type="button" value="ButtonCerrar_validacion_usuario" class="close da_event_captive">&times;</button>     
                    </div>
                    <div id="Cotenedor_validacion_usuario" style="background-color: #FFFFFF; height: auto; width: auto;  color: black; overflow:auto ; border-top:none" class="modal_content_back modal-body">
                        <asp:UpdatePanel ID="UpdatePanel_buton_ingresar" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row">
                                    <div style="text-align: center" class="col-12 mt-2">
                                        <a><i class="fal fa-edit "></i></a>
                                        <asp:Label ID="Label3" runat="server" Text="Registro PQRSD (Virtual)" Style="font-weight: 600" CssClass="h6"></asp:Label>
                                    </div>
                                </div>   
                                <div class="row mt-2">
                                    <div  class="col-12">
                                        <p style="text-align: justify; margin-top: 2px; font-size: 13px"><strong>Información: &nbsp;</strong>A continuación digite su número de documento de identificación, seguido seleccione su año de nacimiento si es un ciudadano (Persona natural) o año de creación si es una Persona jurídica (entidad, empresa, asociación entre otras).</p>
                                    </div>
                                </div> 
                                <div class="row mt-2 w-100">
                                    <div class="col-6">
                                         <asp:Label ID="Label1" runat="server" Text="Numero documento de identificación *"   CssClass="h6 font-weight-light"></asp:Label>
                                    </div>
                                    <div class="col-6">
                                         <asp:TextBox ID="TextBox_nit_identificacion" runat="server" CssClass="form-control" Style="width: 100%"></asp:TextBox>
                                    </div>
                                </div>
                                 <div class="row mt-2 w-100">
                                    <div class="col-6">
                                        <asp:Label ID="Label8" runat="server" Text="Seleccione el año en que nació/Anualidad *" CssClass="h6 font-weight-light"></asp:Label>
                                    </div>
                                    <div class="col-6">
                                         <asp:DropDownList ID="DropDownList_anualidad" runat="server" CssClass="custom-select w-50"></asp:DropDownList>
                                    </div>
                                </div>
                                
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="modal-footer justify-content-end" id="modal-footer_Panel_validacion_usuario">  
                         <asp:UpdatePanel ID="UpdatePanel_boton_Panel_validacion_usuario" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                             <ContentTemplate>
                                 <input id="Hidden_resultado_buscar" runat="server" type="hidden" value=""/>         
                                 <asp:Button ID="Button_buscar" runat="server" Text="Aceptar" CssClass="btn btn-success"  />
                             </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div style="display: none; height: 0px">
                        <asp:Button ID="ButtonSalir_validacion_usuario" runat="server" Text="" Height="0px" Width="0px" Style="display: none" />
                        <asp:Button ID="ButtonCerrar_validacion_usuario" runat="Server" Text="" Height="0px" Width="0px" Style="display: none" />
                    </div>
                </div>
            </asp:Panel>
            <!--Popup actualizacion_anualidad-->
            <asp:Panel ID="Panel_actualizacion_anualidad" runat="server"   Style=" width: 50%;  display:none">
                <asp:ModalPopupExtender ID="ModalPopupExtender_actualizacion_anualidad" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_actualizacion_anualidad"
                    PopupControlID="Panel_actualizacion_anualidad" CancelControlID="ButtonCerrar_actualizacion_anualidad">
                </asp:ModalPopupExtender>   
                <div id="modal_content_actualizacion_anualidad" class="modal-content">
                    <div id="divcabecera_ctualizacion_anualidad" class="modal_title_superiorr_ modal-header">
                        <h6 class="modal-title d-inline ml-1">Registro PQRS página web</h6>
                        <button type="button" value="ButtonCerrar_actualizacion_anualidad" class="close da_event_captive">&times;</button>
                    </div>
                    <div id="Cotenedor_actualizacion_anualidad" style="background-color: white; width: 100%; height: 100%; border-top: none" class="modal_content_back modal-body">
                            <asp:UpdatePanel ID="UpdatePanel_actualizacion_anualidad" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div class="row m-100 mt-1">
                                        <div style=" text-align: center" class="col-12">
                                            <asp:Label ID="Label4" runat="server" Text="" CssClass="h6" >Actualiza anualidad</asp:Label>
                                        </div>
                                    </div>
                                    <div class="row m-100 mt-1">
                                        <div class="col-12" style="text-align: center">
                                            <asp:Label ID="Label5" runat="server" Text="Seleccione su nombre" CssClass="h6 font-weight-light"></asp:Label>
                                        </div>
                                    </div>
                                    <div class="row m-100 mt-1">
                                        <div class="col-12" style="text-align: center">
                                            <asp:DropDownList ID="DropDownList_usuarios_registro" runat="server" Style="width: 100%"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div style="text-align: center; display:none">
                                        <asp:Button ID="Button_actualizar_anualidad" runat="server" Text="Aceptar" CssClass="boton" />    
                                        <asp:Button ID="Button_regresar" runat="server" Text="Regresar" CssClass="boton" />
                                        <input id="Hidden_resultado_actualizar" runat="server" type="hidden" value=""/>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                      
                    </div>
                     <div class="modal-footer justify-content-end" >  
                          <a class="btn btn-success" style="margin-left: 10px; margin: 5px; width: 100px; float: right" title="Regresar pagina anterior" href="#" onclick="activa_boton_client_server('Button_regresar')"><i class="fas fa-arrow-circle-left"></i> Regresar </a>
                          <a class="btn btn-info" style="margin-left: 10px; margin: 5px; width: 100px; float: right" title="Actualiza anualidad" href="#" onclick="activa_boton_client_server('Button_actualizar_anualidad')"><i class="fas fa-check-circle"></i> Aceptar </a>
                     </div>
                </div>
                <div style="display: none; height: 1px">
                    <asp:Button ID="ButtonSalir_actualizacion_anualidad" runat="server" Text="" Height="0px" Width="0px" Style="display: none" />
                    <asp:Button ID="ButtonCerrar_actualizacion_anualidad" runat="Server" Text="" Height="0px" Width="0px" Style="display: none" />
                </div>
                 
            </asp:Panel>
        <!--Popup recuperar_anualidad-->
              <asp:Panel ID="Panel_recuperar_anualidad" runat="server" ForeColor="White"  Style=" width: 40%; margin: auto; display:none; height:auto" CssClass="modal_content_general">
                  <asp:ModalPopupExtender ID="ModalPopupExtender_recuperar_anualidad" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_recuperar_anualidad"
                      PopupControlID="Panel_recuperar_anualidad" CancelControlID="ButtonCerrar_recuperar_anualidad">
                  </asp:ModalPopupExtender>
                  <div id="modal_content_Panel_recuperar_anualidad" class="modal-content">
                      <div id="div_title_recuperar_anualidad" class="modal_title_superior_ modal-header">
                             <h6 class="modal-title d-inline ml-1"></h6>
                             <button type="button" value="ButtonCerrar_recuperar_anualidad" class="close da_event_captive">&times;</button>          
                      </div>
                      <div id="Cotenedor_recuperar_anualidad" style="height: 100%; width: 100%; margin: auto; color: black; border-top:none; overflow:auto" class="modal_content_back_ modal-body">
                          <asp:UpdatePanel ID="UpdatePanel_recuperar_anualidad" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                              <ContentTemplate>
                                  <div class="row w-100 mt-2">
                                      <div style="text-align: center" class="col-12">
                                          <i class="fas fa-user-lock"></i>
                                          <asp:Label ID="Label6" runat="server" Text="Recuperación año de nacimiento o anualidad" CssClass="h6"></asp:Label>
                                      </div>
                                  </div>
                                  <div class="row w-100 mt-1">
                                      <div class="col-12">
                                          <p style="text-align: justify;  font-size: 13px; font-family: Arial"><strong>Información: &nbsp;</strong>El año de nacimiento no corresponde a la identificación, digite su correo electrónico para recuperar el año de nacimiento o anualidad y poder ingresar a su buzon.</p>
                                      </div>
                                  </div>
                                  <div class="row w-100 mt-1">
                                      <div class="col-4">
                                          <asp:Label ID="Label10" runat="server" Text="Correo electrónico"  CssClass="h6 font-weight-light"></asp:Label>
                                      </div>
                                      <div class="col-8">
                                           <asp:TextBox ID="TextBox_correo_electronico_recuperacion" runat="server" Style="width: 100%" placeholder="mi_corroe@dominio.com " CssClass="form-control"></asp:TextBox>
                                      </div>
                                  </div>      
                                  <div style="text-align: center">
                                       <asp:Button ID="ButtonRegresar_Recuperacion" runat="server" Text="Anterior" CssClass="boton_blanco" Style="display: none" ToolTip="Recuperar a ventana anterior" />
                                      <asp:Button ID="ButtonRecuperar" runat="server" Text="Aceptar" CssClass="boton_blanco" Style="display: none" ToolTip="Recuperar año de nacimiento o anualidad" />         
                                  </div>
                              </ContentTemplate>
                          </asp:UpdatePanel>
                          
                      </div>
                      <div class="modal-footer justify-content-end" id="modal-footer">
                          <a class="btn btn-success" style="margin-left: 10px; margin: 5px; width: 100px; float: right" title="Regresar pagina anterior" href="#" onclick="activa_boton_client_server('ButtonRegresar_Recuperacion')"><i class="fas fa-arrow-circle-left"></i> Regresar </a>
                          <a class="btn btn-info" style="margin-left: 10px; margin: 5px; width: 100px; float: right" title="Recuperar año de nacimiento o anualidad" href="#" onclick="activa_boton_client_server('ButtonRecuperar')"><i class="fas fa-check-circle"></i> Aceptar </a>
                      </div>
                      <div style="display:none; height:1px">
                          <asp:Button ID="ButtonCerrar_recuperar_anualidad" runat="Server" Text="" CssClass="modal_boton_hiden" />
                          <asp:Button ID="ButtonSalir_recuperar_anualidad" runat="server" Text="Button" Height="20px" Width="20px" Style="display: none" />
                     </div>
                       
                  </div>
            </asp:Panel>
       
            <!--Popup mensaje_contactos-->
             <asp:Panel ID="Panel_mensaje_contactos" runat="server"   Style=" width:40%;  display:none; height:auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_mensaje_contactos" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_mensaje_contactos"
                    PopupControlID="Panel_mensaje_contactos" CancelControlID="ButtonCerrar_mensaje_contactos">
                </asp:ModalPopupExtender>
                 <div id="modal_content_Panel_mensaje_contactos" class="modal-content">
                     <div id="div_title_mensaje_contactos" class="modal_title_superior_ modal-header">
                          <h6 class="modal-title d-inline ml-1"></h6>
                          <button type="button" value="ButtonCerrar_mensaje_contactos" class="close da_event_captive">&times;</button>              
                     </div>
                     <div id="Cotenedor_mensaje_contactos" style="height: 90%; width: 100%; color: black; border-top:none" class="modal_content_back">     
                             <asp:UpdatePanel ID="UpdatePanel_mensaje_contactos" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                                 <ContentTemplate>
                                     <div id="div_label_panel_mensaje_contactos" style="background-color: #dcdcdc; text-align: center">
                                         <asp:Label ID="Label_detalle_info" runat="server" Text="Información" Style="font-family: Arial"></asp:Label>
                                     </div>
                                     <div id="div_mesaje_panel_mensaje_contactos" style="text-align: justify; overflow: auto; padding: 3px; background-color: white" class=" modal_content_general_border">
                                         <asp:Label ID="Label_info_contacto" runat="server" Text="Información" Style="text-align: justify; font-family: Arial; font-size: 12px"></asp:Label>
                                     </div>
                                 </ContentTemplate>
                             </asp:UpdatePanel>
                     </div>
                     <div style="display: none; height: 1px">
                         <asp:Button ID="ButtonSalir_mensaje_contactos" runat="server" Text="" Height="0px" Width="0px" Style="display: none" />
                         <asp:Button ID="ButtonCerrar_mensaje_contactos" runat="server" Text="" Height="0px" Width="0px" Style="display: none" />
                     </div>
                     
                 </div>
            </asp:Panel>
            <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 200px">
                <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
                Processing ...
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
        <asp:Panel ID="Panel_sube_anexo_respuesta" runat="server" Style="display:none;  width: 60%; height: auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_sube_anexo_respuesta" runat="server"  TargetControlID="ButtonSalir_sube_anexo_respuesta" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_sube_anexo_respuesta" PopupControlID="Panel_sube_anexo_respuesta"></asp:ModalPopupExtender>
                <div class="modal_content" id="modal_content_sube_anexo_respuesta">
                    <div id="divcabecer2_sube_anexo_respuesta" class="modal_title_superior_ modal-header">
                        <h6 id="Label_sube_anexo_respuesta" class="modal-title"> Adjuntar enexos </h6>
                        <button type="button" value="Button_cerrar_sube_anexo_respuesta_" onclick="hide_upload_content('ModalPopupExtender_edition_sube_anexo_respuesta');" class="close da_event_captive_">&times;</button>
                    </div>
                    <div id="contenido_procesa_sube_anexo_respuesta" style="width: 100%; height: 100%; border-top:none" class="modal_content_back p-2">
                        <div class="p-2">
                        <div class="row p-2" id="content_boton_adjunto_anexo_respuesta">
                                <div class="col-12 p-0">
                                    <div class="file-select " id="src-file__">
                                        <input id="file_element_adjunto_anexo_respuesta" type="file" multiple="multiple" accept="" style="width: 100px; height: 40px" name="src-file" class="p-1" contente_file="ModalPopupExtender_sube_documento_adjunto" aria-label="Archivo" />
                                    </div>
                                    <a id="save_file_element_adjunto_anexo_respuesta" title="Guardar todos los archivos" class="btn  btn-success" style="opacity: 0; color:white"><i style="color: white" class="fas fa-save "></i> Guardar </a>
                                    <a id="delete_file_element_adjunto_anexo_respuesta" title="Elminar todos los archivos cargados" class="btn  btn-danger " style="opacity: 0; color:white"><i style="color: white" class="fal fa-trash-alt "></i> Eliminar </a>
                                    <a id="cancel_file_element_adjunto_anexo_respuesta" title="Cancelar guardar archivos" class="btn  btn-warning" style="opacity: 0; color:white"><i style="color: white" class="fas fa-window-close "></i> Cancelar </a>
                                </div>
                        </div>
                        <div class="paren_element background_upload" id="conten_file_element_adjunto_anexo_respuesta" style="overflow: auto; height: 100%">

                                <div id="content_drop_element_adjunto_anexo_respuesta" claas="">
                                </div>
                                <table id="table_file_element_adjunto_anexo_respuesta" class="table table-striped">
                                </table>
                            </div>
                        <div class="row border pt-2" id="content_pie_title_adjunto_anexo_respuesta">
                                <div class="col-8">
                                    <div class="row p-2">
                                        <div class="col-4 p-0">
                                            <div>
                                                <asp:Label ID="Label_progres_bar_file_element_adjunto_anexo_respuesta" runat="server" Text="" Style="font-family: Arial; text-align: center; font-size: 20px"></asp:Label>
                                            </div>
                                            <div id="pogres_file_element_contador_adjunto_anexo_respuesta" style="text-align: center; font-family: Arial; font-size: 14px">
                                            </div>
                                            <div id="pogres_file_element_porcent_adjunto_anexo_respuesta" style="text-align: center; font-family: Arial; font-size: 14px">
                                            </div>
                                        </div>
                                        <div class="col-5 p-0">
                                            <div>
                                                <div id="myProgress_file_element_adjunto_anexo_respuesta">
                                                    <div id="myBar_file_element_adjunto_anexo_respuesta" class="file-select-bar"></div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-3 p-0 pl-1">
                                             <p id="count_byte_file_element_adjunto_anexo_respuesta"></p>
                                        </div>

                                    </div>
                                   
                                </div>
                                <div class="col-4 justify-content-end pt-2">
                                    <p id="count_file_element_adjunto_anexo_respuesta" class="font-weight-light" style="float: right">Estado </p>
                                </div>
                            </div>
                       
                        </div>
                        <div style="display: none; height: 1px">
                            <asp:Button ID="Button_cerrar_sube_anexo_respuesta" runat="Server" Text="" CssClass="invisible" />
                            <asp:Button ID="ButtonSalir_sube_anexo_respuesta" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                        </div>

                    </div>
                   
                </div>
            </asp:Panel>
        <div class="da_loading" id="container_loading_iframe" style="display: none">
            <div class="da_loading_center">Cargando</div>
            <div class="da_loader"></div>
        </div>
    </form>
</body>
</html>
