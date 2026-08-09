<%@ Page Language="vb" AutoEventWireup="false" EnableEventValidation="false" CodeBehind="WebFormPqrsGestion.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormPqrsGestion" %>
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
      <link href="../Styles/styleMenu.css" rel="stylesheet" />
    <link href="../Styles/tabccs.css" rel="stylesheet" />
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
    <script src="../js/Publico/WebFormPqrsGestion.js"></script>
    <script src="../generic_control/FileUploadHandler.js"></script>
    <link href="../generic_control/UploadFile.css" rel="stylesheet" />
     <script src="../js/validate_campos.js"></script>
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
           <asp:ScriptManager ID="ScriptManager1" runat="server"
              EnableScriptGlobalization="True" EnablePageMethods="True" AsyncPostBackTimeout="1900" >
            </asp:ScriptManager>
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
                           elmen.disabled = true;
                       }
                   }
                   function CheckStatus(sender, args) {
                       try {
                       progres_hiden('progres_bar');
                       var elmen = document.getElementById(elment_postbak.id)
                       if (elmen.type == "button" || elmen.type == "image" || elmen.type == "submit") {
                           elmen.disabled = false;
                           //elmen.value = value_element;
                       }
                       if (elment_postbak.id == "Button_Log_respuesta") {
                           auto_zise_popup_log_transacciones();
                       }
                       } catch (e) { alert("Fucion CheckStatus WebFormPqrsGestion" + e.message); }
                   }

            </script>
         <div class="limiter">
             <div class="container-login100">
             <div  class="wrap-login100_person">
                 <div class="validate-form p-l-35 p-r-35 p-t-17">
                     <div id="div_general_content" class="mb-4" >
                         <div id="div_logo_pqrs" class="w-100">
                             <div class="row w-100">
                                 <div class="col-7">    
                                     <span class="ml-2 h4" style="color: #57b846">PQRSD</span>  
                                     <asp:Label ID="Label_tipo_pqrsd" runat="server" Text="" style="color: #57b846" CssClass="h6 ml-1"></asp:Label>
                                      <span  class="h7 ml-2" > formular y consultar </span> 
                                 </div>
                                 <div class="col-1">
                                     
                                 </div>
                                 <div class="col-4">
                                      <a href="javascript:void(0)" title="Atrás" class="float-right" onclick="activa_retroceso_pagina();">
                                         <i style="color: #57b846" class="far fa-arrow-left fa-2x float-left"></i>
                                     </a>
                                 </div>
                             </div>
                             <hr />
                            
                         </div>
                         <div class="row  pt-1">
                           
                             <div class="col-12">
                                 <asp:Label ID="Label_login_uusario" runat="server" Text="na" CssClass="h6  float-right"></asp:Label>
                             </div>
                         </div>
                         <div class="row  mt-1">
                             
                             <div class="col-12">
                                 <asp:Label ID="Label_identifcacion" runat="server" Text="na" CssClass="h6 float-right"></asp:Label>
                             </div>
                         </div>
                         <div class="row mt-1">
                             
                             <div class="col-12">
                                 <asp:Label ID="Label_anualidad" runat="server" Text="na" CssClass="h6 float-right"></asp:Label>
                             </div>
                         </div>
                         <div class="mt-1">
                             <ul class="tab" style="background-color: white">
                                 <li><a href="javascript:void(0)" class="tablinks__" onclick="openCity__(event, 'registro_pqr')" id="defaultOpen"><i class="fas fa-edit"></i> Formular</a></li>
                                 <li><a href="javascript:void(0)" class="tablinks__" onclick="openCity__(event, 'historico_pqr')"><i class="fas fa-search"></i> Consultar</a></li>
                             </ul>
                         </div>
                         <div id="registro_pqr" class="tabcontent___boot_da mb-4" style="border: none">
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
                                     <asp:Label ID="Label1" runat="server" Text="Dependencia/Área *" CssClass="h6 font-weight-light"></asp:Label>
                                 </div>
                                 <div class="col-8">
                                     <asp:DropDownList ID="DropDownList_area_dependencia" runat="server" CssClass="w-60 custom-select"></asp:DropDownList>
                                 </div>
                             </div>
                             <div class="row w-100 mt-1">
                                 <div class="col-4">
                                     <asp:Label ID="Label5" runat="server" Text="Asunto Solicitud *" CssClass="h6 font-weight-light"></asp:Label>
                                 </div>
                                 <div class="col-8">
                                     <asp:UpdatePanel ID="UpdatePanel_asunto_solicitud" UpdateMode="Conditional" runat="server" RenderMode="Inline">
                                         <ContentTemplate>
                                             <asp:TextBox ID="TextBox_asunto" runat="server" Style="width: 100%" CssClass="form-control" TextMode="MultiLine" MaxLength="50"></asp:TextBox>
                                         </ContentTemplate>
                                     </asp:UpdatePanel>
                                 </div>
                             </div>
                              <div class="row w-100 mt-1">
                                 <div class="col-4">
                                     <asp:Label ID="Label2" runat="server" Text="Correo electrónico" CssClass="h6 font-weight-light"></asp:Label>
                                 </div>
                                 <div class="col-8">
                                     <asp:UpdatePanel ID="UpdatePanel_correo_anonimo" UpdateMode="Conditional" runat="server" RenderMode="Inline">
                                         <ContentTemplate>
                                             <asp:TextBox ID="TextBox_correo_anonimo" runat="server" Style="width: 100%" CssClass="form-control"  MaxLength="50"></asp:TextBox>
                                         </ContentTemplate>
                                     </asp:UpdatePanel>
                                 </div>
                             </div>
                             <div class="row w-100 mt-1">
                                 <div class="col-4">
                                     <asp:Label ID="Label3" runat="server" Text="Descripción solicitud*" CssClass="h6 font-weight-light"></asp:Label>
                                 </div>
                                 <div class="col-8">
                                     <asp:UpdatePanel ID="UpdatePanel_descripcion_solicitud" UpdateMode="Conditional" runat="server" RenderMode="Inline">
                                         <ContentTemplate>
                                             <asp:TextBox ID="TextBox_descripcion" CssClass="form-control" runat="server" Style="width: 100%; height: 100%; min-height:100px" TextMode="MultiLine" Rows="5" MaxLength="250"></asp:TextBox>
                                         </ContentTemplate>
                                     </asp:UpdatePanel>
                                 </div>
                             </div>
                              <div class="row w-100 p-2">
                                     <div class="col-12">
                                         <p style="font-size: 8px; text-size-adjust: auto">
                                             Tenga en cuenta para la aceptación de radicados de PQRSF de manera anónima por parte de la CAMARA DE COMERCIO DE VILLAVICENCIO, está sujeto a las condiciones expuestas en el artículo 38 de la Ley 190 de 1995; artículo 81 de la Ley 962 de 2005 y artículo 86 de la Ley 1952 de 2019.
                                         </p>
                                     </div>
                              </div>
                             <div class="row w-100 p-2">
                                 <div class="col-8">
                                     <div class="row w-100">
                                         <div class="col-6">
                                             <asp:UpdatePanel ID="UpdatePanel_anexos_respuesta" UpdateMode="Conditional" runat="server" RenderMode="Inline">
                                                 <ContentTemplate>
                                                     <asp:DropDownList ID="DropDownList_anexos_respuesta" runat="server" Style="" CssClass="custom-select"></asp:DropDownList>
                                                     <asp:Button ID="Button_anexo_cargar" runat="server" Text="" Style="width: 0px; height: 0px; display: none" CssClass="boton" />
                                                     <asp:Button ID="Button_descargar_anexo" runat="server" Text="" Style="width: 0px; height: 0px; display: none" CssClass="boton" />
                                                     <asp:Button ID="Button_anexo_eliminar" runat="server" Text="" Style="width: 0px; height: 0px; display: none" CssClass="boton" />
                                                 </ContentTemplate>
                                             </asp:UpdatePanel>
                                         </div>
                                         <div class="col-6">
                                             <button type="button" title="Adjuntar prueba" value="Button_anexo_cargar" onclick="inicializa_tipo_adjunto_documento(event,this, 'S-D-A');" class="btn  btn-warning  mt-1"><i class="fad fa-arrow-from-bottom"></i></button>
                                             <button type="button" title="Eliminar documento prueba" value="Button_anexo_eliminar" style="color: white" class="btn  btn-danger da_event_captive mt-1"><i class="fal fa-times"></i></button>
                                         </div>
                                     </div>
                                 </div>
                                
                                 <div class="col-4">
                                     <a class="btn btn-success" style="text-align: center; width: auto; margin: 1px; float: right" title="Guardar la solicitud" href="#" onclick="inicializa_tipo_adjunto_documento(event,this, 'R-R-P');"><i class="fas fa-check-circle"></i>Aceptar </a>
                                 </div>

                             </div>
                         </div>
                         <div id="historico_pqr" class="tabcontent___boot_da" style="border: none">
                             <div style="width: 100%; background-color: #F5F5F5; display: none">
                                 <asp:UpdatePanel ID="UpdatePanel_busqueda" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                                     <ContentTemplate>
                                         <asp:TextBox ID="TextBox_busqueda" runat="server" placeholder="Buscar..." Style="width: 270px; float: right; margin-right: 1px; margin-top: 0px"></asp:TextBox>
                                         <asp:ImageButton ID="ImageButton_buscar" runat="server" ToolTip="Consultar" Style="margin-top: 4px; float: right; margin-right: 4px; height: 20px;" ImageUrl="../radicador/imagenes/cbxs0-vnnbp.png" />
                                     </ContentTemplate>
                                 </asp:UpdatePanel>
                             </div>
                             <asp:UpdatePanel ID="UpdateGeneral" runat="server" UpdateMode="Conditional">
                                 <ContentTemplate>
                                     <input id="hdnEmailID" type="hidden" value="0" runat="server" />
                                     <input id="hdnEmailID_VAL" type="hidden" value="0" runat="server" />
                                     <input id="HiddenEmailconsulta" type="hidden" value="" runat="server" />
                                     <asp:Panel ID="Panelactividad" runat="server" Wrap="False"
                                         ScrollBars="Auto" Style="width: 100%; height: 100%">
                                         <asp:GridView ID="data_grid" runat="server" Style="position: inherit; width: 100%"
                                             PageSize="3" PagerSettings-Position="Top" Font-Names="arial" AllowSorting="true" AllowPaging="true"
                                             AutoGenerateSelectButton="False" CssClass="table  font-weight-light" GridLines="None" Font-Size="12px">
                                             <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                             <HeaderStyle CssClass="GridviewScrollHeader_line_boot_gren" BorderStyle="None" />
                                             <RowStyle CssClass="GridviewScrollItem_line_cort" />
                                             <PagerStyle CssClass="pagination-ys" />
                                             <Columns>
                                                 <asp:BoundField HeaderText="OPCIONES   " />
                                             </Columns>
                                         </asp:GridView>
                                     </asp:Panel>
                                     <div id="contenido_titulo_val_radicacion" style="height: 5%; width: 100%; ">
                                         <asp:Label ID="titulo_label_expedientes" runat="server" Style="font-family: Arial; font-size: 12px">Resultados busqueda</asp:Label>
                                     </div>
                                     <div id="botones_accion_postback" style="display: none">
                                         <asp:Button ID="Button_consulta_pqrs_registrados" runat="server" />
                                     </div>
                                 </ContentTemplate>

                                 <Triggers>
                                 </Triggers>
                             </asp:UpdatePanel>

                             <div id="contenido_botonoes" style="width: 100%; position: inherit; height: 30px; background-color: #E7EDF5; display: none">
                                 <asp:UpdatePanel ID="UpdatePanel_botones_radicacion" runat="server" UpdateMode="Conditional">
                                     <ContentTemplate>
                                         <asp:Button ID="Button_Trazabilidad" runat="server" Text="Estados del radicado" Width="140px" ToolTip="Estados del radicado" CssClass="boton" Style="font-family: arial; font-size: 10px" />
                                         <asp:Button ID="Button_Log_respuesta" runat="server" Text="Transacciones de la respuesta" Width="160px" ToolTip="Transacciones realizadas para la respuesta" CssClass="boton" Style="font-family: arial; font-size: 10px" />
                                         <asp:Button ID="Button_detalle_radicado" Text="Detalle del radicado" runat="server" Width="120px" ToolTip="Muestra los detalles del radicado" CssClass="boton" Style="font-family: arial; font-size: 10px" />
                                         <input id="Hidden_colum_header" type="hidden" value="" runat="server" />
                                         <input id="Hidden_ruta_archivo" type="hidden" value="" runat="server" />
                                     </ContentTemplate>
                                 </asp:UpdatePanel>
                             </div>
                         </div>
                         <div id="div_estado_pqrs">
                             <asp:Label ID="Label_estado" runat="server" Text="" Style="font-family: Arial; font-size: 10px; float: right"></asp:Label>
                         </div>
                         <div style="display: none">
                             <asp:UpdatePanel ID="UpdatePanel_sube_documento_respuesta" runat="server" UpdateMode="Conditional">
                                 <ContentTemplate>
                                     <asp:Button ID="Button_sube_documento" runat="server" Text="Button" Style="display: none" />

                                 </ContentTemplate>
                             </asp:UpdatePanel>

                         </div>
                     </div>
                 </div>
             </div>
             </div>
         </div>
         <!--mensaje_personalizado-->
        <asp:Panel ID="Panel_mensaje_personalizado" runat="server" Style="display: none; color: black; width: auto; height: auto">
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
        
         
    
        <!--Popup visor externo-->
               <asp:Panel ID="Panel_visor_externo" runat="server" ForeColor="White" Width="900px" Height="300px" Style="float:left;text-align: left; width: 900px; margin: auto; display:none" >
                  <asp:ModalPopupExtender ID="ModalPopupExtender_visor_externo" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_visor_externo"
                      PopupControlID="Panel_visor_externo" Y="5" CancelControlID="ButtonSalir_visor_externo">
                  </asp:ModalPopupExtender>
                 
                   <asp:Button ID="Button_visor_externo"  runat="server" Text="Button" Height="20px" Width="20px" Style="display:none" />
                  <div id="Cotenedorpendiente_visor_externo" style="border: thin double #000080; color: Black; background-color: #FFFFFF; height: 90%; width: 100%; margin: auto;border-radius: 25px;padding: 10px;background: #ffffff"> 
                      <div id="Div_visor_externo" style="float: right">
                          <asp:Button ID="ButtonSalir_visor_externo" runat="Server" Text="X"
                              ForeColor="#000066" Height="21px" />
                      </div> 
                      <asp:UpdatePanel ID="UpdatePanel_visor_externo" runat="server" UpdateMode="Conditional">
                          <ContentTemplate>
                              <iframe id="Iframe_visor_externo_" runat="server" frameborder="0"  style="width:100%; height:100%; overflow:hidden"></iframe>
                          </ContentTemplate>

                      </asp:UpdatePanel>
                           
                  </div>
                  
              </asp:Panel>
        <!--detalle trazabilidad-->
           <asp:Panel ID="Panel_trazabilidad" runat="server" Style="display:none; overflow:hidden; width:70%; height:100%"  CssClass="modal_content_general" >
                  <asp:ModalPopupExtender ID="ModalPopupExtender_trazabilidad" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_trazabilidad_dos"
                      PopupControlID="Panel_trazabilidad"  CancelControlID="ButtonSalir_trazabilidad">
                  </asp:ModalPopupExtender>
                  <div id="modal_content_Panel_trazabilidad" class="modal-content">  
                  <div id="Cabecerapendiente_trazabilidad" class="modal_title_superior_ modal-header"> 
                       <h6 class="modal-title d-inline ml-1">Ventana de trazabilidad</h6>
                       <button type="button" value="ButtonSalir_trazabilidad" class="close da_event_captive">&times;</button>                      
                  </div>
                  <div id="Cotenedorpendiente_trazabilidad" style="height: 100%; width: 100%; border-top:none" class="modal_content_back modal-body">          
                      <asp:UpdatePanel ID="UpdatePanel_trazabilidad" runat="server" UpdateMode="Conditional">
                          <ContentTemplate>
                              <iframe id="Iframe_trazabilidad_" runat="server" frameborder="0"  style="width:100%; height:100%; overflow:hidden"></iframe>
                          </ContentTemplate>
                      </asp:UpdatePanel>              
                  </div>
                      <div style="display: none; height: 1px">
                          <asp:Button ID="Button_trazabilidad_dos" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                          <asp:Button ID="ButtonSalir_trazabilidad" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                      </div>
                  </div>
              </asp:Panel>
        <!--detalle transacciones-->
           <asp:Panel ID="Panel_transacciones" runat="server" Style="display:none; overflow:hidden; width:70%; height:100%" CssClass="modal_content_general" >
                  <asp:ModalPopupExtender ID="ModalPopupExtender_transacciones" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_transacciones_dos"
                      PopupControlID="Panel_transacciones"  CancelControlID="ButtonSalir_transacciones">
                  </asp:ModalPopupExtender>
                  <div id="modal_content_Panel_transacciones" class="modal-content">  
                  <div id="Cabecerapendiente_transacciones" class="modal_title_superior_ modal-header" >  
                       <h6 class="modal-title d-inline ml-1">Detalle de transacciones</h6>
                       <button type="button" value="ButtonSalir_transacciones" class="close da_event_captive">&times;</button>                      
                  </div>
                  <div id="Cotenedorpendiente_transacciones" style="height: 100%; width: 100%; overflow:hidden; border-top:none" class="modal_content_back modal-body">
                      <asp:UpdatePanel ID="UpdatePanel_transacciones" runat="server" UpdateMode="Conditional">
                          <ContentTemplate>
                              <iframe id="Iframe_transacciones_" runat="server" frameborder="0"  style="width:100%; height:100%; overflow:hidden"></iframe>
                          </ContentTemplate>
                      </asp:UpdatePanel>         
                  </div>
                  </div>
               <div style="display: none; height: 1px">
                   <asp:Button ID="Button_transacciones_dos" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                   <asp:Button ID="ButtonSalir_transacciones" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
               </div>
              </asp:Panel>
        <!--detalle log transacciones-->
        <asp:Panel ID="Panel_log_transacciones" runat="server" Style="display: none; overflow: hidden; width: 70%; height: 100%" CssClass="modal_content_general">
            <asp:ModalPopupExtender ID="ModalPopupExtender_log_transacciones" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_log_transacciones_dos"
                PopupControlID="Panel_log_transacciones" CancelControlID="ButtonSalir_log_transacciones">
            </asp:ModalPopupExtender>
            <div id="modal_content_Panel_log_transacciones" class="modal-content">
                <div id="Cabecerapendiente_log_transacciones" class="modal_title_superior_ modal-header">
                    <h6 class="modal-title d-inline ml-1">Detalle de transacciones</h6>
                    <button type="button" value="ButtonSalir_log_transacciones" class="close da_event_captive">&times;</button>
                </div>
                <div id="Cotenedorpendiente_log_transacciones" style="height: 100%; width: 100%; overflow: hidden; border-top:none" class="modal_content_back modal-body">
                    <asp:UpdatePanel ID="UpdatePanel_log_transacciones" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <iframe id="Iframe_log_transacciones_" runat="server" frameborder="0" style="width: 100%; height: 100%; overflow: hidden"></iframe>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
            <div style="display: none; height: 1px">
                <asp:Button ID="Button_log_transacciones_dos" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                <asp:Button ID="ButtonSalir_log_transacciones" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
            </div>
        </asp:Panel>
       
        <div id="sube_documento_respuesta">
            <asp:Panel ID="Panel_sube_documento_respuesta" runat="server" Style="display:none;  width: 50%; height: auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_sube_documento_respuesta" runat="server" 
                    TargetControlID="ButtonSalir_sube_documento_respuesta" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_sube_documento_respuesta" PopupControlID="Panel_sube_documento_respuesta"></asp:ModalPopupExtender>
                <div id="modal_content_Panel_sube_documento_respuesta" class="modal-content">
                    <div id="divcabecer2_sube_documento" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title d-inline ml-1">Sube anexo</h6>
                        <button type="button" value="Button_cerrar_sube_documento_respuesta" class="close da_event_captive">&times;</button> 
                    </div>
                    <div id="contenido_procesa_sube_documento_respuesta" style="width: 100%; height: 100%; border-top:none" class="modal-body">
                        <div id="drop_zone_2" style="width: 100%; height: auto; overflow: auto">
                            <asp:UpdatePanel ID="UpdatePanel_descarga" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:AjaxFileUpload ID="AjaxFileUpload_dowload" runat="server" ThrobberID="drop_zone_2"
                                        ContextKeys="fred_1"
                                        AllowedFileTypes="pdf"
                                        MaximumNumberOfFiles="1" OnClientUploadComplete="activa_boton_dowload_adjunto"  />      
                                      <asp:Label ID="Label_estado_carga" runat="server" Text="Estado" Style="font-family: Arial; font-size: 10px"></asp:Label>
                                    <asp:Button ID="Button_sube_documento_adjunto_respuesta" CssClass="invisible" runat="server" Text="" Style=" width: 0px; height: 0px;  display: none" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                </div>    
                <div style="display: none; height: 1px">
                    <asp:Button ID="ButtonSalir_sube_documento_respuesta" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="Button_cerrar_sube_documento_respuesta" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px"/>
                </div>
            </asp:Panel>
        </div>
          <!--imagen_respuesta-->
           <asp:Panel ID="Panel_imagen_respuesta" runat="server" Style="display:none; overflow:hidden; width:99%; height:100% "  CssClass="modal_content_general" >
                  <asp:ModalPopupExtender ID="ModalPopupExtender_imagen_respuesta" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_imagen_respuesta_dos"
                      PopupControlID="Panel_imagen_respuesta"  CancelControlID="ButtonSalir_imagen_respuesta">
                  </asp:ModalPopupExtender>
               <div id="modal_content_Panel_imagen_respuesta" class="modal-content">
                   <div id="Cabecerapendiente_imagen_respuesta" class="modal_title_superior_ modal-header" >
                         <h6 class="modal-title d-inline ml-1">Visor de documentos</h6>
                         <button type="button" value="ButtonSalir_imagen_respuesta" class="close da_event_captive">&times;</button>   
                   </div>
                   <div id="Cotenedorpendiente_imagen_respuesta" style="height: 100%; width: 100%; overflow: hidden; border-top:none" class="modal_content_back modal-body">
                       <asp:UpdatePanel ID="UpdatePanel_imagen_respuesta" runat="server" UpdateMode="Conditional">
                           <ContentTemplate>
                               <iframe id="Iframe_imagen_respuesta_" runat="server" frameborder="0" style="width: 100%; height: 100%; overflow: hidden"></iframe>
                           </ContentTemplate>
                       </asp:UpdatePanel>
                   </div>
                   <div style="display: none; height: 1px">
                       <asp:Button ID="Button_imagen_respuesta_dos" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                       <asp:Button ID="ButtonSalir_imagen_respuesta" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                   </div>
                  
               </div>
              </asp:Panel>

        <asp:UpdatePanel ID="Updatepanel_botones_visor_emergente" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:Button ID="Button_visor_emergente" runat="server" Text="Button" Style="display: none" />
            </ContentTemplate>
        </asp:UpdatePanel>
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
    <script>
        /*function openCity(evt, cityName) {
            var i, tabcontent, tablinks;
            tabcontent = document.getElementsByClassName("tabcontent");
            for (i = 0; i < tabcontent.length; i++) {
                tabcontent[i].style.display = "none";
            }
            tablinks = document.getElementsByClassName("tablinks");
            for (i = 0; i < tablinks.length; i++) {
                tablinks[i].className = tablinks[i].className.replace(" active", "");
            }
            document.getElementById(cityName).style.display = "block";
            evt.currentTarget.className += " active";
            //Button_consulta_pqrs_registrados
            if (cityName == "historico_pqr") {
                document.getElementById("Button_consulta_pqrs_registrados").click();
            }
        }*/
        function openCity(evt, cityName) {
            var i, tabcontent, tablinks;
            tabcontent = document.getElementsByClassName("tabcontent_boot_da");
            for (i = 0; i < tabcontent.length; i++) {
                tabcontent[i].style.display = "none";
            }
            tablinks = document.getElementsByClassName("tablinks");
            for (i = 0; i < tablinks.length; i++) {
                tablinks[i].className = tablinks[i].className.replace(" active_boot_da", "");
            }
            document.getElementById(cityName).style.display = "block";
            document.getElementById("Hidden_select_tab").value = cityName;
            evt.currentTarget.className += " active_boot_da";

        }

        function openCity_(evt, cityName) {
            var i, tabcontent, tablinks;
            tabcontent = document.getElementsByClassName("tabcontent__boot_da");
            for (i = 0; i < tabcontent.length; i++) {
                tabcontent[i].style.display = "none";
            }
            tablinks = document.getElementsByClassName("tablinks_");
            for (i = 0; i < tablinks.length; i++) {
                tablinks[i].className = tablinks[i].className.replace(" active_vis__boot_da_gren", "");
            }
            document.getElementById(cityName).style.display = "block";
            document.getElementById("Hidden_select_tab_").value = cityName;
            evt.currentTarget.className += " active_vis_boot_da";

        }
        function openCity__(evt, cityName) {
            var i, tabcontent, tablinks;
            tabcontent = document.getElementsByClassName("tabcontent___boot_da");
            for (i = 0; i < tabcontent.length; i++) {
                tabcontent[i].style.display = "none";
            }
            tablinks = document.getElementsByClassName("tablinks__");
            for (i = 0; i < tablinks.length; i++) {
                tablinks[i].className = tablinks[i].className.replace(" active_vis__boot_da_gren", "");
            }
            document.getElementById(cityName).style.display = "block";
            evt.currentTarget.className += " active_vis__boot_da_gren";
            if (cityName == "historico_pqr") {
                document.getElementById("Button_consulta_pqrs_registrados").click();
            }

        }
        // Get the element with id="defaultOpen" and click on it
        document.getElementById("defaultOpen").click();
        AjaxFileUpload_change_text();
    </script>
</body>
</html>
