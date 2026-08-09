<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormGaCompartirDocumento.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormGaCompartirDocumento" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Administración clasificación</title>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>    
     <script src="../js/ui/jquery-3.4.1.min.js"></script>
    <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <link href="../tokenzize2/tokenize2.min.css" rel="stylesheet" />
    <script src="../tokenzize2/tokenize2.1.min.js"></script>
     <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
     <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script> 
    <link href="../Styles/styleMenu.css" rel="stylesheet" type="text/css" />   
    <script src="../js/validate_campos.js"></script>   
     <link href="../Styles/Menu3.css" rel="stylesheet" />
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />   
    <link href="../Styles/tumb.css" rel="stylesheet" />      
    <script src="../js/gestion/WebFormGaCompartirDocumento.js"></script>    
    <script  src="../Awesome/js/all.js"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet"/>
  <link href="../Awesome/css/brands.css" rel="stylesheet"/>
  <link href="../Awesome/css/solid.css" rel="stylesheet"/>
    <script  src="../Awesome/js/brands.js"></script>
  <script  src="../Awesome/js/solid.js"></script>
  <script  src="../Awesome/js/fontawesome.js"></script>
    
</head>
<body style="">
    <form id="form1" runat="server" onkeypress="return caracter_especial(event,this)">
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
                //$("#Menu1").show();
                if (elment_postbak.type == "button" || elment_postbak.type == "submit") {
                    elment_postbak.value = value_element;
                    elment_postbak.disabled = false;
                }
                if (elment_postbak.id == "Button_compartir_documento") {
                    if (document.getElementById("Hidden_resultado_compartir").value == "YES") {
                        cerrar_ventana('Button_cerrar_autoriza_compartir_documento');
                    }
                    
                }
                if (elment_postbak.id == "Button_anexar_aportes_colaboracion") {
                    //auto_size_popup_colaboracion_documento_compartido();

                }      
                auto_zise_popup_visor_externo();
            }

            </script>
    <div id="contenido_general" style="overflow:auto" class="container-fluid" >
        <div id="title_compartir" class="p-2 mt-1" style="margin-right:2px; background-color:#6d7fcc">
            <asp:Label ID="Label_title" runat="server" Text="Compartir documentos con otros usuarios" Style="color:white"></asp:Label>
        </div>
        <asp:Panel ID="Panel_selecion_user" runat="server">
             <div id="div_seleccion_usuario" class="row mt-2" >
           <div class="col-2 col-sm-1 align-self-center">
               <span class="h6 font-weight-light">Usuarios</span>
           </div> 
           <div class="col-10 col-sm-11 pl-0">
               <select class="tokenize-callable-demo1 w-100" multiple>
               </select>
           </div>
       </div>
        </asp:Panel>
      
          
        <div class = "row mt-2 mr-1">
             <div class="col-2 col-sm-1 align-self-center">
               <span class="h6 font-weight-light">Asunto</span>
           </div> 
            <div id="div_asunto_documento"  class="col-10 col-sm-11 border-bottom p-0">
                <asp:TextBox ID="TextBox_asunto_documento" runat="server" CssClass="form-control_ imput_text_line_none_focus_none" style=" width:100%; "   maxlength="30"  ></asp:TextBox>
           </div>
        </div>     
        <div class="row mt-4 p-3 ">
            <div id="div_nota_documento" class="col-12 border-bottom border-top p-0">
                <asp:TextBox ID="TextBox_nota_documento" CssClass="imput_text_line_none" runat="server" Style="width:100%" TextMode="MultiLine" Rows="4" placeholder=""></asp:TextBox>
        </div>
        </div>
         <div   class="row mt-0">          
            <div id="div_seleccion_documento" class="col-12" >
                <asp:UpdatePanel ID="UpdatePanel_seleccion_documento" runat="server" UpdateMode="Conditional" RenderMode="Inline" >
                    <ContentTemplate>
                        <asp:Panel ID="Panel_seleccion_documento" runat="server" Style="overflow: auto" >
                            <asp:Table ID="Table_seleccion_documento" runat="server" Style="text-align: left; width: auto; height: 110px; font-size: 12px" class="form-control_" EnableViewState="false"></asp:Table>
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>           
        </div>
        
        <div style="background-color:#E7EDF5; display:none" >
            <asp:Label ID="Label_resultado" runat="server" Text="Estado" style="font-size:11px"></asp:Label>
        </div>
        <div id="div_tipo_documento" class = "mt-2" >       
            <asp:UpdatePanel ID="UpdatePanel_tipo_documento_compartido" runat="server" UpdateMode="Conditional"  class="row w-100">
                <ContentTemplate>
                    <div class="col-12 col-sm-4 mt-1 pl-0">
                        <div class="col-6">
                             <asp:Label ID="Label_prioridad" runat="server" Text="Prioridad(*)"  CssClass="h6 font-weight-light"></asp:Label>
                        </div>
                        <div class="col-6">
                            <asp:DropDownList ID="DropDownList_prioridad_solicitud" runat="server" Style="width: 150px" class="custom-select "></asp:DropDownList>
                        </div>
                    </div>
                    <div class="col-12 col-sm-4 mt-1 pl-0">
                        <div class="col-6">
                            <asp:Label ID="Label_tipo_documento" runat="server" Text="Compartido como (*)" CssClass="h6 font-weight-light"></asp:Label>
                        </div>
                        <div class="col-6">
                             <asp:DropDownList ID="DropDownList_tipo_documento_compartir" runat="server" class="custom-select " Style="width: 200px"></asp:DropDownList>
                        </div>
                    </div>
                    <div class="col-12 col-sm-4 mt-1 pl-0">
                        <div class="col-6">
                             <asp:Label ID="Label_fecha_limite" runat="server" Text="Fecha límite (*)" CssClass="h6 font-weight-light"></asp:Label>
                        </div>
                        <div class="col-6">
                            <asp:TextBox ID="TextBox_fecha_limite_solicitud" runat="server" Width="120px" style="" CssClass="date_indice" placeholder="YYYY-MM-DD"></asp:TextBox>
                            <asp:CalendarExtender ID="TextBoxFECHA_EXTREMA_INICIAL_CalendarExtender" runat="server" BehaviorID="TextBoxFECHA_EXTREMA_INICIAL_CalendarExtender" TargetControlID="TextBox_fecha_limite_solicitud" Format='yyyy-MM-dd' PopupButtonID="ImageButtonfechaextremaini" />
                            <asp:ImageButton ID="ImageButtonfechaextremaini" runat="server" ImageUrl="../imagera/Calendar.png" Height="20px" Width="20px" CssClass="mt-1"/>
                        </div>
                    </div>
                   
                    
                </ContentTemplate>
            </asp:UpdatePanel>
              
        </div>
          <div id="botom_option"  >
            <asp:UpdatePanel ID="UpdatePanel_boton_documento" runat="server" UpdateMode="Conditional" class = "row mt-2 justify-content-end pb-3">
                <ContentTemplate>
                    <input id="Hidden_value_documento" type="hidden" value="0" runat="server"/>
                    <input id="Hidden_ruta_archivo" type="hidden" value="0" runat="server"/>
                    <input id="Hidden_id_actividad" type="hidden" value="0" runat="server"/>
                    <input id="Hidden_resultado_compartir" type="hidden" value="" runat="server"/>
                    <input id="Hidden_iten_ckek" type="hidden" value="" runat="server"/>
                    <input id="Hidden_text_user" type="hidden" value="" runat="server"/>
                    <asp:Button ID="Button_compartir_documento" runat="server" Text="Compartir" CssClass="btn  btn-light mr-3" ToolTip="Compartir los documentos relacionados" Style="background-color:#6d7fcc; color:white" OnClientClick="Compartir_documentos_tokenize(); " />
                    <asp:Button ID="Button_anexar_aportes_colaboracion" runat="server" Text="Adjuntar aportes" ToolTip="Adjunta aportes de colaboración" CssClass="btn btn-light mr-2" Style="background-color:#6d7fcc; color:white"  OnClientClick="eliminar_ajaxtolkit(); "/>
                    <asp:Button ID="Button_responder" runat="server" Text="Responder" CssClass="boton_azul"  ToolTip="Responder a quien compartio los documentos"   Style="display: none" />
                    <asp:Button ID="Button_activa_visor_documento" runat="server" Text="" Style="display: none" />             
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
        
      
         <!--Popup visor externo-->
               <asp:Panel ID="Panel_visor_externo" runat="server" Style="display:none; overflow:hidden" ForeColor="White" Width="100%" Height="100% " CssClass="modal_content_general">
                  <asp:ModalPopupExtender ID="ModalPopupExtender_visor_externo" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_visor_externo"
                      PopupControlID="Panel_visor_externo" Y="1"  CancelControlID="ButtonSalir_visor_externo">
                  </asp:ModalPopupExtender>
                  <div id="Cabecerapendiente_visor_externo" class="modal_title_superior">
                       <h6 class="modal-title d-inline ml-1">Documento</h6>
                       <button type="button" value="ButtonSalir_visor_externo" class="close da_event_captive mr-2">&times;</button>               
                  </div>
                  <div id="Cotenedorpendiente_visor_externo" style="height: 90%; width: 100%; overflow:hidden" class="modal_content_back">
                  
                      <asp:UpdatePanel ID="UpdatePanel_visor_externo" runat="server" UpdateMode="Conditional">
                          <ContentTemplate>
                              <iframe id="Iframe_visor_externo_clasficacion_" runat="server" frameborder="0"  style="width:100%; height:100%; overflow:hidden"></iframe>
                          </ContentTemplate>

                      </asp:UpdatePanel>
                           
                  </div>
                   <div style="display: none; height: 1px">
                       <asp:Button ID="Button_visor_externo" runat="server" Text="" Height="1px" Width="1px" Style="display: none" />
                       <asp:Button ID="ButtonSalir_visor_externo" runat="Server" Text="" Height="1px" Width="1px" Style="display: none" />
                   </div>
              </asp:Panel>
        <div id="tol_pie" style=" float:right;  background-color:#E7EDF5; width:100%; height:3%;border-style: ridge; border-bottom-width: 0.5px; border-left-width: 1px; border-right-width: 1px; border-top-width: 1px;text-align:center; display:none"">
                 <asp:Label ID="Label5" runat="server" Text="Estado" style="font-family:Arial;font-size:11px; display:none"></asp:Label>
                    <asp:UpdatePanel ID="updatapanel_iframe" runat="server" UpdateMode="Conditional" >
                            <ContentTemplate>
                                  <iframe runat="server" style="float:left" id="ifmExcel_" width="0" height="0" marginheight="0" marginwidth="0"
                            frameborder="0" />
                            </ContentTemplate>
                           
                 </asp:UpdatePanel>
             </div>
           <!--cargar documento!-->
         
            <asp:Panel ID="Panel_sube_documento_adjunto" runat="server" Style="display:none ; color: White; width: 50%; height: auto"  CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_sube_documento_adjunto" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_sube_documento_adjunto"
                    PopupControlID="Panel_sube_documento_adjunto" CancelControlID="Button3_cerrar_adjunta" ></asp:ModalPopupExtender>
                <div id="Div_cabecera" class="modal_title_superior"> 
                       <h6 class="modal-title d-inline ml-1">Adjuntar</h6>
                       <button type="button" value="Button3_cerrar_adjunta" class="close da_event_captive mr-2">&times;</button>   
                   
                    
                       
                    
                </div>
                 <div id="Div_contenido_adjunta" style=" height: 100%; width: 100%" class="modal_content_back">            
                        <div id="drop_zone_" style="width: 100%; height:auto; overflow: auto">
                            <asp:UpdatePanel ID="UpdatePanel_descarga" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>                 
                                    <asp:AjaxFileUpload ID="AjaxFileUpload_dowload" runat="server" ThrobberID="drop_zone_"
                                        ContextKeys="fred"
                                        AllowedFileTypes="tif,jpg,tiff,bmp,pdf,docx,xls,xlsx,doc"
                                        MaximumNumberOfFiles="1" OnClientUploadComplete="activa_boton_dowload" />
                                    <asp:Button ID="Button_guardar_desicion" runat="server" Text="Button" Style="display: none" />        
                                    &nbsp  
                                    <asp:Label ID="Label_estado_carga" runat="server" Text="Estado" Style="font-family: Arial; font-size: 10px"></asp:Label>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                      <div style="display:none; height:1px">
                             <asp:Button ID="Button_sube_documento_adjunto" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                       <asp:Button ID="Button3_cerrar_adjunta" runat="Server" Text="X" CssClass="invisible"
                              />
                      </div>
                     
                </div>


            </asp:Panel>
          
        
        <!--colaboracion_documento_compartido-->       
            <asp:Panel ID="Panel_colaboracion_documento_compartido" runat="server" Style="display:none;  width: 50%; height:auto" CssClass="modal_content_general_">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_colaboracion_documento_compartido" runat="server"  TargetControlID="ButtonSalir_colaboracion_documento_compartido" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_colaboracion_documento_compartido" PopupControlID="Panel_colaboracion_documento_compartido">            
                </asp:ModalPopupExtender>
                <div class="modal-content">
                    <div id="divcabecer2_colaboracion_documento_compartido" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title">Colaboración documentos compartidos</h6>
                        <button id="Button_cerrar_ventana" type="button" onclick="activa_boton_cerrar();" class="close" title="Cerrar ventana">&times;</button>
                    </div>
                   
                    <div id="contenido_procesa_colaboracion_documento_compartido" style="background-color: white; width: 100%; height: 100%; overflow:auto" class="modal_content_back modal-body">
                        <div id="div_contenedor_nota">
                            <asp:UpdatePanel ID="UpdatePanel_colaboracion_documento_compartido" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Label ID="Label_nota_colaboracion" runat="server" Text="Nota para la colaboración" Style="margin-left: 5px" CssClass="h6 font-weight-light"></asp:Label>
                                    <br />
                                    <asp:TextBox ID="TextBox_nota_colaboracion" runat="server" Style="width: 99%; height: 100px; margin-left: 2px" TextMode="MultiLine"></asp:TextBox>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                        <div id="div_contendor_boton_guardar_nota" class="row justify-content-end pr-3">
                            <button type="button" title="Guardar nota" value="Button_guarda_actualiza_nota" style="color: white" class="btn  btn-success da_event_captive "><i class="fal fa-save"></i></button>
                        </div>
                        <div id="adjunta_documento" class="mt-2 row  " >
                            <asp:UpdatePanel ID="UpdatePanel_adjunto_documento_colaboracion" runat="server" UpdateMode="Conditional" class="col-12 col-sm-6 mt-2  d-sm-inline-flex d-block">
                                <ContentTemplate>
                                    <asp:Label ID="Label_soporte_colaboracion" runat="server" Text="Anexos" Style="" CssClass="h6 font-weight-light mr-1"></asp:Label>
                                    <asp:DropDownList ID="DropDownList_docuentos_colaboracion" CssClass="form-control " runat="server" Style="width: 200px"></asp:DropDownList>
                                    <asp:ImageButton ID="ImageButton__adjuntar_archivo" runat="server" ImageUrl="#" OnClientClick="eliminar_ajaxtolkit();" ToolTip="Subir archivo colaboración" Style="display: none" />
                                    <asp:ImageButton ID="ImageButton_eliminar_archivo" runat="server" ImageUrl="#" Style="width: 20px; margin-left: 5px; display: none" ToolTip="Eliminar archivo colaboración" />
                                </ContentTemplate>
                            </asp:UpdatePanel>

                            <div class="col-12 col-sm-6 mt-1 d-sm-inline-flex d-block">
                                <button type="button" title="Subir archivo colaboración" value="ImageButton__adjuntar_archivo" style="color: white" class="btn  btn-success da_event_captive m-1 "><i class="fad fa-arrow-from-bottom"></i></button>
                                <button type="button" title="Eliminar archivo colaboración" value="ImageButton_eliminar_archivo" style="color: white" class="btn  btn-danger da_event_captive m-1 "><i class="fal fa-times"></i></button>
                            </div>

                        </div>

                        <div style="display: none; height: 1px">
                            <asp:Button ID="Button_colaboracion_documento_compartido" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                            <asp:Button ID="ButtonSalir_colaboracion_documento_compartido" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                            <asp:Button ID="Button_cerrar_colaboracion_documento_compartido" runat="Server" Text=""
                                Style="display: none" Class="invisible" />
                        </div>
                        <div style="display: none">
                            <asp:UpdatePanel ID="UpdatePanel_guarda_actualiza_nota" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <input id="Hidden_id_nota_document" type="hidden" value="0" runat="server">
                                    <asp:Button ID="Button_guarda_actualiza_nota" runat="server" Text="Guardar nota" CssClass="boton_azul" Style="margin-right: 5px" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                    <div id="div_contenedor_confirma_colaboracion" class="modal-footer">
                        <asp:UpdatePanel ID="UpdatePanel_confirmar_colaboracion" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Button ID="Button_cerrar_colaboracion_validacion" runat="Server" Text="Cancelar colaboración" Style="display: none" CssClass="boton" ToolTip="Cerrar ventana" />
                                <asp:Button ID="Button_confirmar_colaboracion" runat="server" Text="Confirmar" CssClass="btn btn-success" Style="margin-right: 5px; float: right; margin-bottom: 10px" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>

                </div>
            </asp:Panel>
       
       <!---mensaje_progreso evento-->
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

    </form>
    <script  accesskey="javascript" type="text/javascript">
        AjaxFileUpload_change_text();
        

       
    </script>
</body>
</html>
