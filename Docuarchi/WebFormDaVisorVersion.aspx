<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormDaVisorVersion.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormDaVisorVersion" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Neodynamic.WebControls.ImageDraw" Namespace="Neodynamic.WebControls.ImageDraw" TagPrefix="neoimg" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Visor docuarchi.net</title>
     <script src="../js/ui/jquery-3.4.1.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.11.0/umd/popper.min.js" type="text/javascript"></script>
    <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
    <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>  
   <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <script src="../js/Docuarchi/WebFormDaVisorDocuarchi.js"></script>
     <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <script src="../js/java_general/general_code_java.js"></script>
    <script  src="../Awesome/js/all.js"></script>
     <script src="../js/java_general/gestion_meta_dato.js" type="text/javascript"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet"/>
  <link href="../Awesome/css/brands.css" rel="stylesheet"/>
  <link href="../Awesome/css/solid.css" rel="stylesheet"/>
    <script  src="../Awesome/js/brands.js"></script>
  <script  src="../Awesome/js/solid.js"></script>
  <script  src="../Awesome/js/fontawesome.js"></script>
     <link href="../bootstrap/table/dist/bootstrap-table.min.css" rel="stylesheet" />
    <script src="../bootstrap/table/dist/bootstrap-table.min.js" type="text/javascript"></script>
    <script src="../bootstrap/table/dist/bootstrap-table-locale-all.js" type="text/javascript"></script>   
    <script src="../bootstrap/table/dist/extensions/export/bootstrap-table-export.min.js" type="text/javascript"></script>
    <script src="../bootstrap/table/dist/extensions/export/bootstrap-table-export.js" type="text/javascript"></script>     
    <script src="https://unpkg.com/tableexport.jquery.plugin/tableExport.min.js" type="text/javascript"></script>
     <script  accesskey="javascript" type="text/javascript">

</script>
    <style>
 
.waterMark
{  height: 16px;
           width: 168px;
          
           border: 1px solid #BEBEBE;
           background-color: #F0F8FF;
           color: gray;
           font-size: 8pt;
           text-align: center;
}
</style>
</head>
    <body style="margin:0px; overflow:hidden">
    <form id="form1" runat="server" >
     <asp:ScriptManager ID="ScriptManager1" runat="server"
            EnableScriptGlobalization="True" EnablePageMethods="True">
        </asp:ScriptManager> 
        <script accesskey="javascript" type="text/javascript">
            Sys.WebForms.PageRequestManager.getInstance().add_initializeRequest(InitializeRequest);
            Sys.Application.add_load(ApplicationLoadHandler)
            var elment_postbak;
            function ApplicationLoadHandler(sender, args) {

                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(CheckStatus);

            }
            function InitializeRequest(sender, args) {
                try {
                    //
                    posicion_update_pogres('progres_bar');
                    if (args.get_postBackElement().id == 'ImageButtonInicio') {
                        document.getElementById("ImageButtonInicio").disabled = true;
                    }
                    if (args.get_postBackElement().id == 'ImageButtonAnterior') {
                        document.getElementById("ImageButtonAnterior").disabled = true;
                    }
                    if (args.get_postBackElement().id == 'ImageButtonSiguiente') {
                        document.getElementById("ImageButtonSiguiente").disabled = true;
                    }
                    if (args.get_postBackElement().id == 'ImageButtonFinal') {
                        document.getElementById("ImageButtonFinal").disabled = true;
                    }
                    if (args.get_postBackElement().id == 'ImageMenos') {
                        document.getElementById("ImageMenos").disabled = true;
                    }
                    if (args.get_postBackElement().id == 'ImageMas') {
                        document.getElementById("ImageMas").disabled = true;
                    }
                    elment_postbak = args.get_postBackElement();

                }
                catch (err) {
                    alert(err.message + " funcion InitializeRequest " + err.message);
                }
            }
            function CheckStatus(sender, args) {
                try {
                    if (elment_postbak.id == 'ImageButtonInicio') {
                        document.getElementById("ImageButtonInicio").disabled = false;
                    }
                    if (elment_postbak.id == 'ImageButtonAnterior') {
                        document.getElementById("ImageButtonAnterior").disabled = false;
                    }
                    if (elment_postbak.id == 'ImageButtonSiguiente') {
                        document.getElementById("ImageButtonSiguiente").disabled = false;
                    }
                    if (elment_postbak.id == 'ImageButtonFinal') {
                        document.getElementById("ImageButtonFinal").disabled = false;
                    }
                    if (elment_postbak.id == 'ImageMenos') {
                        document.getElementById("ImageMenos").disabled = false;
                    }
                    if (elment_postbak.id == 'ImageMas') {
                        document.getElementById("ImageMas").disabled = false;
                    }

                    if (elment_postbak.id == "ImageButtonindice") {
                        visualiza_indice_documento(1);
                    }

                }
                catch (err) {
                    alert(err.message + " funcion InitializeRequest " + err.message);
                } finally { progres_hiden('progres_bar'); }
            }

        </script>
     <div id="ContentGeneral" style="width:100%; height:100%; position:relative" >
         <div id="tollimage" style="width: 100%; border-bottom: 1px solid #ddd; background: black; display: inline-flexbox; min-height:50px" class="navbar navbar-expand-sm   pb-0 pl-0 pt-1">        
             <button class="navbar-toggler btn btn-light mb-2 ml-2 mr-2" style="padding-bottom: 2px" type="button" data-toggle="collapse" data-target="#navbarNavDropdown">
                 <span class=""><i style="color: white" class="fas fa-bars"></i></span>
             </button>
             <div class="collapse navbar-collapse   pb-1 pt-0" id="navbarNavDropdown">
                 <div class="nav  ">
                     <div class="nav-item active active_azul">
                         <a class="nav-link active_azul ml-1" title="Primera  imagen (tecla D)" href="#" onclick="activa_boton_client_server('ImageButtonInicio')"><i style="font-size: 20px; color: white" class="fad fa-arrow-alt-to-left  active_azul"></i></a>
                     </div>
                     <div class="nav-item active active_azul">
                         <a class="nav-link  active_azul" style="margin-left: 2px; margin: 2px; width: auto; color: black" title="Anterior imagen (Tecla A)" href="#" onclick="activa_boton_client_server('ImageButtonAnterior')"><i style="color: white; font-size: 20px" class="fad fa-arrow-alt-left fa-1x"></i></a>
                     </div>
                     <asp:UpdatePanel ID="UpdatePanel_conte_bot" runat="server"
                         UpdateMode="Conditional" RenderMode="Inline">
                         <ContentTemplate>
                             <div class="nav-item active ">
                                 <asp:TextBox ID="LabelConteo" runat="server" ToolTip="Para busqueda digite número y presione tecla enter" CssClass="mt-1" Style="margin-left: 5px; margin-right: 5px; text-align: center; margin-top: 3px; font-size: 12px; width: 70px; font-family: 'Segoe UI Emoji'" onkeypress="preven_event_search_keypres_enter(event,this);"></asp:TextBox>
                             </div>
                         </ContentTemplate>
                     </asp:UpdatePanel>
                     <div class="nav-item active  active_azul">
                         <a class="nav-link active_azul" style="" title="Siguiente imagen (tecla S)" href="#" onclick="activa_boton_client_server('ImageButtonSiguiente')"><i style="color: white; font-size: 20px" class="fad fa-arrow-alt-right fa-1x"></i></a>
                     </div>

                     <div class="nav-item active active_azul">
                         <a class="nav-link active_azul" style="color: white" title="Ultima  imagen (tecla F)" href="#" onclick="activa_boton_client_server('ImageButtonFinal')"><i style="color: white; font-size: 20px" class="fad fa-arrow-alt-to-right fa-1x"></i></a>
                     </div>

                     <div class="nav-item active active_azul">
                         <a class="nav-link active_azul" style="color: white" title="Alejar Imagen (tecla -)" href="#" onclick="activa_boton_client_server('ImageMenos')"><i style="color: white; font-size: 20px" class="fad fa-minus-circle fa-1x"></i></a>
                     </div>

                     <div class="nav-item active active_azul">
                         <a class="nav-link active_azul" style="color: white" title="Acercar Imagen (tecla +) " href="#" onclick="activa_boton_client_server('ImageMas') "><i style="color: white; font-size: 20px" class="fad fa-plus-circle fa-1x"></i></a>
                     </div>
                     <asp:UpdatePanel ID="UpdatePanel_drows_bot" runat="server"
                                        UpdateMode="Conditional" RenderMode="Inline">
                                        <ContentTemplate>
                                            <asp:DropDownList ID="DropDownList_zom" runat="server" AutoPostBack="True" class="mt-1 mr-1 ml-1">
                                                <asp:ListItem Value="50"></asp:ListItem>
                                                <asp:ListItem>20</asp:ListItem>
                                                <asp:ListItem>30</asp:ListItem>
                                                <asp:ListItem>40</asp:ListItem>
                                                <asp:ListItem>50</asp:ListItem>
                                                <asp:ListItem>60</asp:ListItem>
                                                <asp:ListItem>70</asp:ListItem>
                                                <asp:ListItem>80</asp:ListItem>
                                                <asp:ListItem>90</asp:ListItem>
                                                <asp:ListItem>100</asp:ListItem>
                                            </asp:DropDownList>
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="ImageMenos" EventName="Click" />
                                            <asp:AsyncPostBackTrigger ControlID="ImageMenos" EventName="Click" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                                                   
                 </div>
             </div>
             <div style="display: none">
                 <asp:UpdatePanel ID="UpdatePanelButon" runat="server"
                     UpdateMode="Conditional" RenderMode="Inline">
                     <ContentTemplate>
                         <asp:Button ID="Button_actualiza_indice" runat="server" Text="Button" Style="display: none" OnClientClick="actualiza_indice_padre();" />
                         <asp:ImageButton ID="ImageButtonInicio" runat="server" ToolTip="Primera  imagen" ImageUrl="../imagewf/inicio14.png" Style="display: none" />
                         <asp:ImageButton ID="ImageButtonAnterior" runat="server" ToolTip="Anterior imagen" ImageUrl="../imagewf/anterior15.png" Style="display: none" />
                         <asp:ImageButton ID="ImageButtonSiguiente" runat="server" ToolTip="Siguiente imagen" ImageUrl="../imagewf/siguiente15.png" Style="display: none" />
                         <asp:ImageButton ID="ImageButtonFinal" runat="server" ToolTip="Ultima  imagen" ImageUrl="../imagewf/final15.png" Style="display: none" />
                         <asp:ImageButton ID="ImageMenos" runat="server" ToolTip="Alejar Imagen" ImageUrl="../imagewf/alejarimagen.png" Style="display: none" />
                         <asp:ImageButton ID="ImageMas" runat="server" ToolTip="Acercar Imagen" ImageUrl="../imagewf/acercarimagen.png" Style="display: none" />
                         <asp:ImageButton ID="ImageButtonguardardocumento" runat="server" ToolTip="Guadar documento" ImageUrl="../Docuarchi/imagenes/guardarimagen.png" Visible="true" Style="display: none" />
                         <input id="Hidden_ruta_archivo" type="hidden" value="" runat="server" />
                     </ContentTemplate>
                     <Triggers>
                     </Triggers>
                 </asp:UpdatePanel>
             </div>
            
         </div>
         <div id="cuerpoindice"
              style="width: 0%; left: auto; display:none; float:right; height: auto; margin: 0px 0px 0px 0px; background-color:white;  width:310px">
              <asp:UpdatePanel ID="UpdatePanelindice" runat="server" UpdateMode="Conditional"
                  RenderMode="Inline">
                  <ContentTemplate>
                      <iframe id="ifrm_indice_visor_docuarchi_" runat="server" style="border-style: none; left: 1px; width:100%; height:99%; position: relative; top: 1px; background-color:white; float: right"
                          frameborder="0" scrolling="no"  ></iframe>
                          <input id="Hidden_result_indice" type="hidden" value="0" runat="server"/>
                  </ContentTemplate>
                  <Triggers>                     
                  </Triggers>
              </asp:UpdatePanel>
              &nbsp;&nbsp;&nbsp;    
          </div>  
         <div id="ocultaleft"
             style="left: auto; width: 5px; float: right; height: 9.5%; background-color: #053061; display: none">
         </div>
         <div id="content" style="width: 100%; height: 380px; background-color: Gray; filter: alpha(opacity=70); opacity: 50; overflow:auto; border-style: ridge; border-bottom-width: 0.5px; border-left-width: 1px; border-right-width: 1px; border-top-width: 1px; " class="visor">
             <div id="zona" style="width: auto; height: auto">
                 <asp:UpdatePanel ID="UpdatePanelvisor" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                     <ContentTemplate>
                          <input id="Hidden_gabinete" type="hidden" value="" runat="server" />
                          <input id="Hidden_imagen" type="hidden" value="0" runat="server"/>
                         <neoimg:ImageDraw ID="noaming" runat="server" RenderingMethod="HttpHandler" HttpHandlerName="ImageGenerator.axd" Style="position: relative" CssClass="visor">
                         </neoimg:ImageDraw>
                         <asp:Label ID="Label_estado" runat="server" Text="" Style="font-family: Arial; font-size: 10px; float: left"></asp:Label>
                     </ContentTemplate>
                     <Triggers>
                     </Triggers>
                 </asp:UpdatePanel>
             </div>
             <div id="draggable" class="ui-widget-content" style="background-color: Gray; display: none; position: absolute">
                 <img id="img" alt="Firma Mecanica"
                   align="bottom" style="border-style: none" />
             </div>
            
         </div>
          <div id="Ocultaindice"
                  style="left: auto; width: 4px; float: left; height: 9.5%; background-color: #053061; display:none">
              </div>
         <div id="oculto" style=" visibility:hidden; position:absolute">
              <asp:ImageButton ID="ImageButtonguardar" runat="server"  ToolTip="Guardar firma" Visible="true" />
         </div>
  
    </div>
         <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 50px">
            <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
            Processing ...
        </div>
          <div id="Impresion_post">
            <asp:Panel ID="Panelimpresionpost" runat="server"  Style="display:none; color: White; width: 60%; height: 60%" CssClass="modal_content_general">
                <asp:DragPanelExtender ID="DragPanelExtenderimpre_post" runat="server" TargetControlID="Panelimpresionpost" />
                <asp:ModalPopupExtender ID="ModalPopupExtenderimpre_post" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_post"
                    PopupControlID="Panelimpresionpost" CancelControlID="Buttoncerrarimpre_post">
                </asp:ModalPopupExtender>
                <div id="divcabecer2_post" class="modal_title_superior">  
                     <h6 class="modal-title d-inline ml-1">Menú Impresión</h6>
                     <button type="button" value="Buttoncerrarimpre_post" class="close da_event_captive mr-2">&times;</button>                                
                </div>
                <asp:UpdatePanel ID="UpdatePaneliframe_post" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div id="ContenidoImpresion_post" style="color: black; background-color: #FFFFFF; border-bottom-right-radius:10px; border-bottom-left-radius:10px" class="modal_content_back pr-2 pl-2">
                            <iframe width="100%" height="100%" id="ifimpre_post_" runat="server" src="../Radicador/WebFormDaImprimir.aspx" frameborder="0"></iframe>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <div style="display:none; height:1px">
                     <asp:Button ID="Button1_post" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" style="display:none" />
                     <asp:Button ID="ButtonSalir_post" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" style="display:none"/>
                     <asp:Button ID="Buttoncerrarimpre_post" runat="Server" Text="X" CssClass="invisible" Height="1px" Width="1px" style="display:none"/>
                </div>
                    
            </asp:Panel>
        </div>
       <!--POPUP EXPORTAR DOCUMENTO-->
        <div id="Divdescarga_anexo_respuesta">
            <asp:Panel ID="Panel_descarga_anexo_respuesta" runat="server" Style="display:none; color: White; width:50%; height:auto" CssClass="modal_content_general_">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_descarga_anexo_respuesta" runat="server" BehaviorID="Panel_descarga_anexo_respuesta_ModalPopupExtender" TargetControlID="ButtonSalir_descarga_anexo_respuesta" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_descarga_anexo_respuesta" PopupControlID="Panel_descarga_anexo_respuesta"></asp:ModalPopupExtender>
                <div id="div_title_descarga" class="modal_title_superior">
                    <h6 class="modal-title d-inline ml-1">Descarga documento</h6>
                    <button type="button" value="Button_cerrar_descarga_anexo_respuesta" class="close da_event_captive mr-2">&times;</button>                                      
                </div>
                <div id="contenido_procesa_descarga_anexo_respuesta" style="width:100%; border-bottom-right-radius:10px; border-bottom-left-radius:10px" class="modal_content_back pr-2 pl-2">       
                    <asp:UpdatePanel ID="UpdatePanel_descarga_anexo_respuesta" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <iframe   id="ifimpre_descarga_anexo_respuesta_" runat="server" style="width :100%; height:100%" frameborder="0"   ></iframe>
                            
                        </ContentTemplate>
                    </asp:UpdatePanel>
            
                    <div style="display:none; height:1px">
                         <asp:Button ID="ButtonSalir_descarga_anexo_respuesta" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" style="display:none" /> 
                          <asp:Button ID="Button_cerrar_descarga_anexo_respuesta" runat="Server" Text="" CssClass="invisible"  Height="1px" Width="1px" style="display:none"
                              />  
                    </div>  
                    
                </div>
           </asp:Panel>
        </div>  
        <div id="info_post">
            <asp:Panel ID="Panel_info" runat="server"  Style="display:none; color: White; width: 60%; height: auto; background-color: #FFFFFF" CssClass="modal_content_general">              
                <asp:ModalPopupExtender ID="ModalPopupExtender_info" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_post_info"
                    PopupControlID="Panel_info" CancelControlID="Buttoncerrarimpre_post_info">
                </asp:ModalPopupExtender>                 
                    <div style="background-color: #FFFFFF" class="modal_title_superior">
                            <h6 class="modal-title ml-2 d-inline">Info documento</h6>
                            <button type="button" value="Buttoncerrarimpre_post_info" class="close da_event_captive mr-2">&times;</button>                     
                    </div>
                <asp:UpdatePanel ID="UpdatePane_info" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div id="ContenidoImpresion_info"  style="color: black; background-color: #FFFFFF; height:100%; width: 100%" class="modal_content_back pb-2">
                            <div class="container_">
                                <asp:TextBox ID="TextBox_info" class="content_option_selecion form-control h6 font-weight-light" runat="server" Style="width:99%; height:auto; min-height: 250px; overflow:auto; background-color:white" TextMode="MultiLine"></asp:TextBox>              
                            </div>    
                             
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <div style="display:none; height:1px">
                     <asp:Button ID="Button1_info" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" Style="display:none" />
                    <asp:Button ID="ButtonSalir_post_info" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" Style="display:none" />
                  <asp:Button ID="Buttoncerrarimpre_post_info" runat="Server" Text="" CssClass="invisible" style="float:right"
                             />
                </div>
                  
            </asp:Panel>
        </div>
        <asp:Panel ID="Panel_interface_consulta_meta_dato" runat="server" Style="display: none; width: 100%; height: auto" CssClass="modal_content_general_">
              <asp:ModalPopupExtender ID="ModalPopupExtender_edition_interface_consulta_meta_dato" runat="server" BackgroundCssClass="FondoAplicacion" 
                  TargetControlID="ButtonSalir_interface_consulta_meta_dato"
                  CancelControlID="Button_cerrar_interface_consulta_meta_dato" PopupControlID="Panel_interface_consulta_meta_dato">
              </asp:ModalPopupExtender>
              <div id="modal_content_consulta_meta_dato" class="modal-content">
                  <div id="divcabecer2_interface_consulta_meta_dato" class="modal_title_superior_ modal-header">
                      <h6 id="label_interface_consulta_meta_dato" class="modal-title  ">Meta datos</h6>
                      <button type="button" value="Button_cerrar_interface_consulta_meta_dato" class="close da_event_captive">&times;</button>
                  </div>
                  <div id="contenido_procesa_interface_consulta_meta_dato" style="background-color: white; width: auto; height: auto; color: black; background-color: #FFFFFF; border-top: none; overflout: auto" class="modal_content_back modal-body">
                      <div id="div_content_tabla">
                          <table
                              id="table_meta_row"
                              data-height="400"
                              data-pagination="false"
                              data-page-list="[10, 25, 50, 100, all]"
                              data-show-export="true"
                              data-toggle="table"
                              data-id-field="ra_m_id"
                              data-search="true"
                              data-locale="es-SP">
                              <thead>
                                  <tr>
                                      <th data-field="ra_m_id" data-visible="false" style="display: none">id_meta_dato</th>
                                      <th data-field="Meta_dato" data-sortable="true" data-sort-name="Meta_dato" data-sort-order="desc">CAMPO</th>
                                      <th data-field="Valor_meta_dato">VALOR</th>
                                      <th data-field="Estado_obligatorio">OBLIGATORIEDAD</th>
                                      <th data-field="Estandar_meta_dato" data-sortable="true" data-sort-name="Estandar_meta_dato" data-sort-order="desc">ESTANDAR</th>
                                      <th data-field="descripcion">DESCRIPCION</th>
                                      <th data-field="Tipo">CONTEXTO</th>
                                  </tr>
                              </thead>
                          </table>
                      </div>
                  </div>              
                  <div style="display: none; height: 1px">
                      <asp:Button ID="Button_cerrar_interface_consulta_meta_dato" runat="Server" Text="" />
                      <asp:Button ID="Button_interface_consulta_meta_dato" runat="server" Text="" Height="1px" Width="1px" />
                      <asp:Button ID="ButtonSalir_interface_consulta_meta_dato" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                  </div>
              </div>
          </asp:Panel>
        <asp:UpdatePanel ID="updatapanel_iframe" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <iframe runat="server" id="ifmExcel_" width="0" height="0" marginheight="0" marginwidth="0"
                    frameborder="0" />
            </ContentTemplate>
        </asp:UpdatePanel>
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
