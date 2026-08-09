<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormDaVisorExterno.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormDaVisorExterno" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Visor docuarchi.net</title>
    <script src="../js/ui/jquery-3.4.1.min.js"></script>
    <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
    <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
   <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <script src="../js/Docuarchi/WebFormDaVisorExterno.js"></script>
     <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <script src="../js/java_general/general_code_java.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.11.0/umd/popper.min.js" type="text/javascript"></script>
    <script src="../js/java_general/gestion_meta_dato.js" type="text/javascript"></script>
    <link href="../bootstrap/table/dist/bootstrap-table.min.css" rel="stylesheet" />
    <script src="../bootstrap/table/dist/bootstrap-table.min.js" type="text/javascript"></script>
    <script src="../bootstrap/table/dist/bootstrap-table-locale-all.js" type="text/javascript"></script>   
	<script src="../bootstrap/table/dist/extensions/export/bootstrap-table-export.min.js" type="text/javascript"></script>
	<script src="../bootstrap/table/dist/extensions/export/bootstrap-table-export.js" type="text/javascript"></script>     
    <script src="https://unpkg.com/tableexport.jquery.plugin/tableExport.min.js" type="text/javascript"></script>
    <script  src="../Awesome/js/all.js"></script>
    <link href="../Awesome/css/fontawesome.css" rel="stylesheet"/>
    <link href="../Awesome/css/brands.css" rel="stylesheet"/>
    <link href="../Awesome/css/solid.css" rel="stylesheet"/>
    <script  src="../Awesome/js/brands.js"></script>
    <script  src="../Awesome/js/solid.js"></script>
    <script  src="../Awesome/js/fontawesome.js"></script>
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
                    posicion_update_pogres('progres_bar');
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
                        visualiza_indice_documento();
                    }
                    progres_hiden('progres_bar');
                }
                catch (err) {
                    alert(err.message + " funcion InitializeRequest " + err.message);
                }
            }

        </script>
     <div id="ContentGeneral" style="width:100%; height:100%; position:relative">
         <div id="tollimage" style="width: 95%; height: 40px; position: relative; margin-bottom: 0px; top: 0px; left: 0px;" class="p-2">
               <a  style=" margin-left:2px; margin:2px; width:auto; color:black" title="Información imagen" href="#"  onclick="activa_boton_client_server('ImageButtoninfo')"><i style="width:25px; height:25px" class="fad fa-file-alt fa-2x"></i></a>
               <a  style=" margin-left:2px; margin:2px; width:auto; color:black" title="Ver meta datos del documento" href="#"  onclick="event_click_indice_meta_dato(event,this);"><i  style="width:25px; height:25px" class="fas fa-file-invoice fa-2x"></i></a>          
                <a id="ImageButtonindice_" style=" margin-left:2px; margin:2px; width:auto; color:black" title="Visualiza indice documento" href="#"  onclick="event_click_indice(event,this);"><i id="indice_title" style="width:25px; height:25px" class="fad fa-info fa-2x"></i></a>          
             <div style="display:none">
                 <asp:UpdatePanel ID="UpdatePanelButon" runat="server"
                     UpdateMode="Conditional" RenderMode="Inline">
                     <ContentTemplate>
                         <asp:ImageButton ID="ImageButtoninfo" runat="server" ToolTip="Información Documento" ImageUrl="../Docuarchi/imagenes/infoimagen.png" Visible="true" Style="display: none" />

                         <asp:ImageButton ID="ImageButtonindice" runat="server" ToolTip="Visualiza indice documento" ImageUrl="../Docuarchi/imagenes/indice222.png" Visible="true" Style="display: none" />

                         <asp:ImageButton ID="ImageMas" runat="server" Style="display: none" ToolTip="Imagen completa" OnClientClick="resize_contenedor()" ImageUrl="../imagewf/acercarimagen.png" />
                         &nbsp
                     <input id="Hidden_ruta_archivo" type="hidden" value="" runat="server" />
                          <input id="Hidden_gabinete_" type="hidden" value="" runat="server" />
                          <input id="Hidden_imagen_" type="hidden" value="0" runat="server"/>
                     </ContentTemplate>
                     <Triggers>
                     </Triggers>
                 </asp:UpdatePanel>
             </div>
            
            
         </div>
         <div id="cuerpoindice"
               style="width: 0%; left: auto; display:none; float:right; height: auto; margin: 0px 0px 0px 0px; background-color:white;  width:320px">
              <asp:UpdatePanel ID="UpdatePanelindice" runat="server" UpdateMode="Conditional"
                  RenderMode="Inline">
                  <ContentTemplate>
                      <iframe id="ifrm_indice_visor_externo_" runat="server" style="border-style: none; left: 1px; width: 100%; height:100%; position: relative; top: 1px; background-color:white; float: right"
                          frameborder="0" scrolling="no" ></iframe>
                          <input id="Hidden_result_indice" type="hidden" value="0" runat="server"/>
                     
                  </ContentTemplate>
                  <Triggers>
                      
                  </Triggers>
              </asp:UpdatePanel>
             
          </div>     
         <div id="content" style="width: 99%; height: 380px; float:left; background-color: Gray; filter: alpha(opacity=70); opacity: 50; border-style: ridge; border-bottom-width: 0.5px; border-left-width: 1px; border-right-width: 1px; border-top-width: 1px; " >     
                 <asp:UpdatePanel ID="UpdatePanelvisor" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                     <ContentTemplate>
                          <input id="Hidden_gabinete" type="hidden" value="" runat="server" />
                          <input id="Hidden_imagen" type="hidden" value="0" runat="server"/>
                         <iframe id="ifrm_visor_" runat="server" style="border-style: none; left: 3px; width: 100%; height: 380px;  top: 0px"
                          frameborder="0" scrolling="no"></iframe>
                     </ContentTemplate>
                     <Triggers>
                     </Triggers>
                 </asp:UpdatePanel>                    
         </div>
          <div id="Ocultaindice"
             style="left: auto; width: 4px; float: left; height: 9.5%; background-color: #053061; display: none">
         </div>
         <div id="ocultaleft"
             style="left: auto; width: 5px; float: right; height: 9.5%; background-color: #053061; display: none">
         </div>
  
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
         <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 50px">
            <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
            Processing ...
        </div>
          <div id="Impresion_post">
            <asp:Panel ID="Panelimpresionpost" runat="server"  Style="display:none; color: White; width: auto; height: auto">
                <asp:DragPanelExtender ID="DragPanelExtenderimpre_post" runat="server" TargetControlID="Panelimpresionpost" />
                <asp:ModalPopupExtender ID="ModalPopupExtenderimpre_post" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_post"
                    PopupControlID="Panelimpresionpost" CancelControlID="Buttoncerrarimpre_post">
                </asp:ModalPopupExtender>
                <div id="divcabecer2_post" class="cabecera2">
                    <asp:Button ID="Button1_post" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_post" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Label ID="Label3" runat="server" Text="Menu Impresion" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton2_post" style="float: right">
                        <asp:Button ID="Buttoncerrarimpre_post" runat="Server" Text="X"
                            ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />

                    </div>
                </div>
                <asp:UpdatePanel ID="UpdatePaneliframe_post" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div id="ContenidoImpresion_post" style="border: thin double #000080; color: black; background-color: #FFFFFF; height: 280px; width: 500px">
                            <iframe width="100%" height="100%" id="ifimpre_post_" runat="server" src="../Radicador/WebFormDaImprimir.aspx" ></iframe>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </asp:Panel>
        </div>
        <div id="guardar_post">
            <asp:Panel ID="Panel_guardar" runat="server"  Style="display:none; color: White; width: auto; height: auto">
                
                <asp:ModalPopupExtender ID="ModalPopupExtender_guardar" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_post_guardar"
                    PopupControlID="Panel_guardar" CancelControlID="Buttoncerrarimpre_post_guardar">
                </asp:ModalPopupExtender>
                <div id="divcabecer2_post_guardar" class="cabecera2">
                    <asp:Button ID="Button1_guardar" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_post_guardar" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Label ID="Label1" runat="server" Text="Guardar documento" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton2_post_guardar" style="float: right">
                        <asp:Button ID="Buttoncerrarimpre_post_guardar" runat="Server" Text="X"
                            ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <asp:UpdatePanel ID="UpdatePane_guardar" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div id="ContenidoImpresion_guardar" style="border: thin double #000080; color: black; background-color: #FFFFFF; height: 280px; width: 550px">
                            <iframe width="100%" height="100%" id="Iframe_guardar" runat="server"  ></iframe>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </asp:Panel>
        </div>
        <div id="info_post">
            <asp:Panel ID="Panel_info" runat="server"  Style="display:none; color: White; width: 40%; height: auto; background-color: #FFFFFF" CssClass="modal_content_general">              
                <asp:ModalPopupExtender ID="ModalPopupExtender_info" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_post_info"
                    PopupControlID="Panel_info" CancelControlID="Buttoncerrarimpre_post_info">
                </asp:ModalPopupExtender>                 
                    <div style="background-color: #FFFFFF" class="modal_title_superior">
                         <h6 class="modal-title d-inline">Info documento</h6>
                         <button type="button" value="Buttoncerrarimpre_post_info" class="close da_event_captive mr-2">&times;</button>                       
                    </div>
                <asp:UpdatePanel ID="UpdatePane_info" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div id="ContenidoImpresion_info"  style="color: black;  height:auto; width: 100%" class="modal_content_back">            
                                <asp:TextBox ID="TextBox_info" class="content_option_selecion" runat="server" Style="width:98%; height:auto; font-family:Arial; font-size:12px; margin:2px;min-height:120px; overflow:scroll; background-color:white" TextMode="MultiLine"></asp:TextBox>           
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <div style="display:none; height:1px">
                   <asp:Button ID="Buttoncerrarimpre_post_info" runat="Server" Text="" CssClass="invisible" 
                            />
                  <asp:Button ID="Button1_info" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                    <asp:Button ID="ButtonSalir_post_info" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                </div>
            </asp:Panel>
        </div>
        <asp:UpdatePanel ID="updatapanel_iframe" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <iframe runat="server" id="ifmExcel_" width="0" height="0" marginheight="0" marginwidth="0"
                    frameborder="0" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
        
</body>

</html>

