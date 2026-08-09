<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormListaSolicitudesPorMiAprobacion.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormListaSolicitudesPorMiAprobacion" %>
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
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
    <link href="../ccs/themes/base/jquery.ui.all.css" rel="stylesheet" type="text/css" />
   <link href="../Styles/styleMenu.css" rel="stylesheet" type="text/css" /> 
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <script src="../js/Filtrar.js"></script>
      <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <script src="../js/radicacion/WebFormListaSolicitudesPorMiAprobacion.js"></script>
     <script src="../js/java_general/general_code_java.js"></script>
    <script src="../js/validate_campos.js"></script>
     <script defer src="../Awesome/js/all.js"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet">
  <link href="../Awesome/css/brands.css" rel="stylesheet">
  <link href="../Awesome/css/solid.css" rel="stylesheet">
    <script defer src="../Awesome/js/brands.js"></script>
  <script defer src="../Awesome/js/solid.js"></script>
  <script defer src="../Awesome/js/fontawesome.js"></script>
</head>
<body>
    <form id="form1" runat="server" onkeypress="return caracter_especial(event,this)">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="True" AsyncPostBackTimeout="900">
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
                if (elment_postbak.id == "Button_lik_service_boton") {
                    auto_zise_popup_lista_solicitudes("1", "1")
                }
                if (elment_postbak.id == "Button_guadar_registro_desicion") {
                    if (document.getElementById("Hidden_resultado_aprobacion").value == "YES") {
                        document.getElementById("Hidden_resultado_aprobacion").value == "";

                        if (document.getElementById("Hidden_actualizacion_usuario").value !== "") {
                            actualiza_gre_campo('data_grid_listado_solicitudes', document.getElementById("hdnEmailID").value, document.getElementById("Hidden_actualizacion_usuario").value, 'ESTADO');
                            document.getElementById("Hidden_actualizacion_usuario").value = "";
                        }
                    }
                }
                if (elment_postbak.id == "Button_ver_documento_solicitud") {
                    //document.getElementById("Label12").innerHTML = "Visor de documentos";
                    auto_zise_popup_visor_externo();
                }

            }

            </script>
        <div id="div_contendor_principal">
            <nav id="div_titulo_listado" style="width: 100%; height:auto" class="navbar navbar-expand-sm nav_botota_person modal_content_no_back_inferior_">
                  <button class="navbar-toggler" type="button" style="background-color: #6d7fcc" data-toggle="collapse" data-target="#navbarNavDropdown">
                      <span class="navbar-toggler-icon_"><i style="color: white" class="fad fa-th-list"></i></span>
                  </button>        
                  <div class="collapse navbar-collapse row" id="navbarNavDropdown">
                      <div class="navbar-nav col-md-8">
                          <a href="#" class="navbar-brand ml-3" style="color: #0062cc"> Respuestas pendientes por mi aprobación </a>
                      </div>
                      <div class=" float-md-right col-md-4 float-sm-left">
                          <div class="input-group ">
                               <button id="td-boton" class="btn btn-outline-secondary border-right-2 " title="Restaura la lista de respuestas pendientes por mi aprobación" style="border-top-right-radius: 0px; border-bottom-right-radius: 0px" onclick="preven_event_restor_search(event,this)" type="button">
                                 <i class="fal fa-long-arrow-left"></i>
                               </button>
                              <asp:TextBox ID="TextBox_busqueda" runat="server" class="form-control form-control-sm complex " placeholder="Busqueda historico...." onkeypress="preven_event_search_keypres_enter(event,this);"></asp:TextBox>
                              <div class="input-group-append">
                                  <button class="btn btn-outline-secondary" title="Consulta historios de aprobaciones" onclick="preven_event_search(event,this)" type="button">
                                      <i class="fal fa-search"></i>
                                  </button>
                              </div>
                          </div>
                      </div>
                  </div>            
            </nav>            
            <div id="div_contendor_filtro_listado" class="modal_content_no_back_inferior_  container-fluid" style="width:100%; height:auto" >   
                <asp:Label ID="Label_anunciado_filtro" runat="server" Text="✓ Solicitudes pendientes" style="font-size:12px; font-family:Arial; float:left; margin-top:5px; float:right; margin-right:2px"></asp:Label> 
                 <div id="div_filtro__fil" class="dropdown_filter" >
                    <button id="boton__filtro_ver" onclick="myFunction(event,this)" class="dropbtn_filter">Filtrar</button>
                    <div id="myDropdown" class="dropdown-content_filter" onkeyup="hiden_keys(event, thiss)">
                        <input type="text" placeholder="Search.." id="myInput" onkeyup="filterFunction()">
                        <a href="#about" onclick="event_elemento(event,'1',this)" class="e_list_marc"> ✓ Solicitudes pendientes </a>
                        <a href="#base" onclick="event_elemento(event,'2',this)" class="e_list_marc">Solicitudes archivadas</a>
                        <a href="#blog" onclick="event_elemento(event,'3',this)" class="e_list_marc">Solicitudes aprobadas</a>
                        <a href="#blog" onclick="event_elemento(event,'4',this)" class="e_list_marc">Solicitudes Desaprobadas</a>
                        
                    </div>
                </div>
                
            </div>     
            <div id="Contenedorgrid_listado_solicitud" style="width: 100%; position: inherit; height:auto; overflow:auto" class="container-fluid">
                <asp:UpdatePanel ID="UpdateGeneral" runat="server" UpdateMode="Conditional" >
                    <ContentTemplate>
                        <input id="hdnEmailID" type="hidden" value="0" runat="server">
                        <input id="hdnEmailID_VAL" type="hidden" value="0" runat="server">
                        <input id="Hidden_id_solicitud" type="hidden" value="0" runat="server">
                        <input id="HiddenEmailconsulta" type="hidden" value="" runat="server">
                        <input id="Hidden_control_lista" type="hidden" value="" runat="server">              
                                <asp:GridView ID="data_grid_listado_solicitudes" runat="server" AllowSorting="true"  AllowPaging="true"  EnableViewState="true"
                                  PageSize="7" PagerSettings-Position="Top"  style="width:100%; font-family: Segoe UI; font-size:16px"
                                    AutoGenerateSelectButton="False" CssClass="table  font-weight-light" GridLines="None" >
                                    <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                    <HeaderStyle CssClass="GridviewScrollHeader_line_boot" BorderStyle="None" />
                                    <PagerStyle CssClass="pagination-ys" />
                                    <Columns>
                                        <asp:BoundField HeaderText="OPCIONES   " />
                                    </Columns>
                                </asp:GridView>                           
                      
                        
                    </ContentTemplate>

                    <Triggers>
                    </Triggers>
                </asp:UpdatePanel>

            </div>
            <div id="contenido_titulo_listado_solicitudes" class="container-fluid mt-1">
                <asp:UpdatePanel ID="UpdatePanel_estado_listado_solicitud" runat="server" UpdateMode="Conditional" >
                    <ContentTemplate>
                        <asp:Label ID="Label_titulo_listado_solicitudes" runat="server" CssClass=" h6 font-weight-light float-left" Style="font-family: Segoe UI; font-size: 16px">Resultados busqueda</asp:Label>
                        <asp:Label ID="Label_estado" runat="server" CssClass="h6 font-weight-light " Style="font-family: Segoe UI; float: right"></asp:Label>
                    </ContentTemplate>
                   </asp:UpdatePanel>
                
            </div>
            <div id="contenedor_opciones_solictitud_general" style=" display:none">
                <asp:UpdatePanel ID="update_botonoes_opciones_solicitud_general" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <input id="Hidden_ruta_archivo" type="hidden" value="" runat="server">
                        <asp:Button ID="Button_ver_documento_solicitud" runat="server" Text="Ver documento del peticionario" ToolTip="Visualiza el documento radicado por el peticionario" CssClass="boton" />
                        <asp:Button ID="Button_ver_documento_respuesta_solicitud" runat="server" Text="Ver documento respuesta al peticionario" ToolTip="Visualiza el documento borrador postulado para responder al peticionario" CssClass="boton" />
                        <asp:Button ID="Button_activa_desicion_aprobacion" runat="server" Text="Gestionar solicitud" ToolTip="Agrega una nueva solicitud de aprobación" CssClass="boton" />
                        <asp:Button ID="Button_lista_filtro" runat="server" Text="Gestionar solicitud"  CssClass="boton" Style="display:none" />
                        
                    </ContentTemplate>
                </asp:UpdatePanel>

            </div>
            <div style="display: none">
                <asp:UpdatePanel ID="UpdatePanel_menu_boton" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Button ID="Button_lik_service_boton" runat="server" Text="Button" />
                        <asp:Button ID="Button_restore_lista_service" runat="server" Text="Button" />
                        <input id="Hidden_lik_service_boton" type="hidden" value="0" runat="server">
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div style="display:none">
                  <asp:UpdatePanel ID="UpdatePanel_busqueda" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                    <ContentTemplate>           
                        <asp:ImageButton ID="ImageButton_buscar" runat="server" Style="margin-top: 4px; float: right; margin-right: 4px; display:none" ImageUrl="../radicador/imagenes/cbxs0-vnnbp.png" />
                        
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            
        </div>
        <div id="tol_pie" style=" float:right;  background-color:#E7EDF5; width:100%; height:3%;border-style: ridge; border-bottom-width: 0.5px; 
                 border-left-width: 1px; border-right-width: 1px; border-top-width: 1px;text-align:center; display:none">
                 <asp:Label ID="Label2" runat="server" Text="Estado" style="font-family:Arial;font-size:11px"></asp:Label>
                    <asp:UpdatePanel ID="updatapanel_iframe" runat="server" UpdateMode="Conditional" >
                            <ContentTemplate>
                                  <iframe runat="server" style="float:left" id="ifmExcel_" width="0" height="0" marginheight="0" marginwidth="0"
                            frameborder="0" />
                            </ContentTemplate>
                           
                 </asp:UpdatePanel>
             </div>
        <!--Popup decisión solicitud-->
            <asp:Panel ID="Panel_desicion_solicitud" runat="server"   Style="display:none; width:70%; height:auto" CssClass="modal_content_general_" >
                <asp:ModalPopupExtender ID="ModalPopupExtender_desicion_solicitud" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_desicion_solicitud"
                    PopupControlID="Panel_desicion_solicitud" CancelControlID="ButtonCerrar_desicion_solicitud">
                </asp:ModalPopupExtender>
                <div id="modal_content_desicion_solicitud" class="modal-content">
                    <div id="divcabecer_desicion_documento" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title d-inline ">Decisión de la solicitud</h6>
                        <button type="button" value="ButtonCerrar_desicion_solicitud" class="close da_event_captive ">&times;</button>                     
                    </div>
                    <div id="Cotenedor_desicion_solicitud" style="background-color: #FFFFFF; height: 100%; width: auto; border-top:none; overflow:auto" class="modal_content_back modal-body">
                        <div class="row">
                            <div class="col-4">
                                <span class="h6 font-weight-light" style="font-family:'Segoe UI'">Decisión de la solicitud (*)</span>
                            </div>
                            <div class="col-8">
                                <asp:DropDownList ID="DropDownList_estado_aprobacion" runat="server"  CssClass="form-control"></asp:DropDownList>
                            </div>
                        </div>
                        <div class="row mt-2">
                            <div class="col-4">
                                <span class="h6 font-weight-light" style="font-family:'Segoe UI'">Nota de decisión (*)</span>
                            </div>
                            <div class="col-8">
                                <asp:TextBox ID="TextBox_nota_solicitud" runat="server" TextMode="MultiLine"  CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row mt-2">
                            <div class="col-4">
                                <span class="h6 font-weight-light" style="font-family:'Segoe UI'">Archivo de correción</span>
                            </div>
                            <div class="col-4">
                                <asp:UpdatePanel ID="UpdatePanel_adjunto_documento_colaboracion" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:DropDownList ID="DropDownList_docuentos_colaboracion" runat="server"  CssClass="form-control"></asp:DropDownList>
                                        <div class="border_superior_inferior_radius_blanco" style="display: none">
                                            <asp:ImageButton ID="ImageButton_adjunta_archivo" runat="server" />
                                            <asp:ImageButton ID="ImageButton_elimina_archivo" runat="server" OnClientClick="eliminar_ajaxtolkit();" />
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                            <div class="col-4 float-lg-right">
                                <button type="button" value="ImageButton_adjunta_archivo" title="Adjunta archivo a la solicitud" class="btn btn-success da_event_captive"><i class="fal fa-long-arrow-up"></i></button>  
                                <button type="button" value="ImageButton_elimina_archivo" title="Elimina archivo adjunto" class="btn btn-danger da_event_captive"><i class="fal fa-times"></i></button>  
                            </div>
                        </div>
                        <div class="row mt-1">
                            <div class="col-12 float-right">
                                  <asp:CheckBox ID="CheckBox_autoriza_firma" runat="server"   Checked="true" />
                                  <span class="h6 font-weight-light ml-1" Style="font-family:'Segoe UI'; font-size: 14px" > Autorizo mi firma para documento respuesta </span>
                            </div>
                           
                        </div>
                       
                        <div style="display:none; height:1px">
                             <asp:Button ID="ButtonSalir_desicion_solicitud" runat="server" Text="Button" Height="20px" Width="20px" Style="display: none" />
                             <asp:Button ID="ButtonCerrar_desicion_solicitud" runat="server" Text="Button" Height="20px" Width="20px" Style="display: none" />
                        </div>
                    </div>
                    <div class="modal-footer justify-content-end" id="modal-footer_desicion_solicitud">  
                        <asp:UpdatePanel ID="UpdatePanel_desicion_solicitud" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Button ID="Button_guadar_registro_desicion" runat="server" Text="Aceptar" CssClass=" btn btn-success" />
                                <input id="Hidden_actualizacion_general" type="hidden" value="0" runat="server">
                                <input id="Hidden_actualizacion_usuario" type="hidden" value="0" runat="server">
                                <input id="Hidden_resultado_aprobacion" type="hidden" value="0" runat="server">
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </asp:Panel>
          <!--cargar documento!-->
          <div id="contenido_procesa_sube_documento_adjunto" >
            <asp:Panel ID="Panel_sube_documento_adjunto" runat="server" Style="display:none ; color: White; width: 50%; height: auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_sube_documento_adjunto" runat="Server" BackgroundCssClass="ModalBackgroud_gorund" TargetControlID="Button_sube_documento_adjunto"
                    PopupControlID="Panel_sube_documento_adjunto" CancelControlID="Button3_cerrar_adjunta" ></asp:ModalPopupExtender>
                <div id="Div_cabecera" class="modal_title_superior">                   
                  <asp:Label ID="Label11" runat="server" Text="Adjuntar" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Div_title_sube_documento_adjunto"" style="float: right">
                        <asp:Button ID="Button3_cerrar_adjunta" runat="Server" Text="X" CssClass="modal_boton_hiden"
                             ToolTip="Cerrar ventana" />
                    </div>
                </div>               
                <div id="Div_contenido_adjunta" style="height: 100%; width: 100%" class="modal_content_back">
                        <asp:UpdatePanel ID="UpdatePanel_descarga" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:AjaxFileUpload ID="AjaxFileUpload_dowload" runat="server" ThrobberID="drop_zone_"
                                    ContextKeys="fred"
                                    AllowedFileTypes="DOCX"
                                    MaximumNumberOfFiles="1" OnClientUploadComplete="activa_boton_dowload" />
                                    <asp:Button ID="Button_guardar_desicion" runat="server" Text="Button" Style="display: none" />
                              
                                    <asp:Label ID="Label_estado_carga" runat="server" Text="Estado" Style="font-family: Arial; font-size: 10px"></asp:Label>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                    
                    <asp:Button ID="Button_sube_documento_adjunto" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                </div>
            </asp:Panel>
          
        </div>
        <input id="Hidden_id_tarea_sel" type="hidden" value="-1" runat="server">
           <input id="Hidden_tipo_visor" type="hidden" value="" runat="server">
               <!--Popup visor externo-->
               <asp:Panel ID="Panel_visor_externo" runat="server" Style="display:none; overflow:hidden"  Width="100%" Height="100% " CssClass="modal_content_general_" >
                  <asp:ModalPopupExtender ID="ModalPopupExtender_visor_externo" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_visor_externo"
                      PopupControlID="Panel_visor_externo"  CancelControlID="ButtonSalir_visor_externo">
                  </asp:ModalPopupExtender>
                   <div id="modal_content_visor_externo" class="modal-content">
                       <div id="Cabecerapendiente_visor_externo" class="modal_title_superior_ modal-header">
                           <h6 class="modal-title d-inline ">Documentos Relacionados</h6>
                           <button type="button" value="ButtonSalir_visor_externo" class="close da_event_captive ">&times;</button>
                       </div>
                       <div id="Cotenedorpendiente_visor_externo" style="background-color: #FFFFFF; height: 100%; width: 100%; overflow:hidden; border-top: none" class="modal_content_back modal-body_">

                           <asp:UpdatePanel ID="UpdatePanel_visor_externo" runat="server" UpdateMode="Conditional">
                               <ContentTemplate>
                                   <iframe id="Iframe_visor_externo_wf_" runat="server" frameborder="0" style="width: 100%; height: 100%; overflow: hidden"></iframe>
                               </ContentTemplate>

                           </asp:UpdatePanel>

                       </div>
                       <div style="display: none; height: 1px">
                           <asp:Button ID="Button_visor_externo" Style="display: none" runat="server" Text="" Height="1px" Width="1px" />
                           <asp:Button ID="ButtonSalir_visor_externo" runat="Server" Style="display: none" Text="" Height="1px" Width="1px" />
                       </div>

                   </div>
              </asp:Panel>
        <!--mensaje_progreso evento-->
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
