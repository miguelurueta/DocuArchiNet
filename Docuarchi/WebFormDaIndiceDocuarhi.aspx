<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormDaIndiceDocuarhi.aspx.vb" EnableEventValidation="false" Inherits="GestionDocumental_Docuarchi.net.WebFormDaIndiceDocuarhi" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
     
    <title></title>
    <script src="../js/ui/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
    <script src="../js/Docuarchi/WebFormDaIndiceDocuarhi.js" type="text/javascript"></script>
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js" type="text/javascript"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <link href="../Awesome/css/fontawesome.css" rel="stylesheet"/>
    <link href="../Awesome/css/brands.css" rel="stylesheet"/>
    <link href="../Awesome/css/solid.css" rel="stylesheet"/>
    <script  src="../Awesome/js/brands.js" type="text/javascript"></script>
    <script  src="../Awesome/js/solid.js" type="text/javascript"></script>
    <script  src="../Awesome/js/fontawesome.js" type="text/javascript"></script>
</head>

<script  accesskey="javascript" type="text/javascript">
    
    //CODIGO AJUSTA EL ALTO DE LA PAGINA DEBE ESTAR EL FORM 100% ALTURA
    var espacio_iframe;
    if (window.innerHeight) {
        //navegadores basados en mozilla 
        espacio_iframe = window.innerHeight
    } else {
        if (document.hidden == true) {
            if (document.body.clientHeight != undefined) {
                //Navegadores basados en IExplorer, es que no tengo innerheight 
                espacio_iframe = document.body.clientHeight
            } else {
                //otros navegadores 
                espacio_iframe = 478
            }
        }
    }
    $(document).ready(bodyResize);
    $(window).resize(bodyResize);
    function bodyResize() {
        $("body").css("height", (espacio_iframe - 50) + "px");
        $("#Panel1").css("height", (espacio_iframe - 50) + "px");
        document.getElementById("Hiddenheih").value = (espacio_iframe - 50);
        
    }
   
</script>
<body  style="background-color:transparent; margin-top:0px; overflow:hidden">
    <form id="form1" runat="server" style="background-color: transparent; height:100%">
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
                   //
                   elment_postbak = args.get_postBackElement();
                   value_scrool();
               }
               function CheckStatus(sender, args) {
                   visualiza_boton_marco_padre();
                   if (elment_postbak.id == "boton_expediente") {
                       if (document.getElementById("Hidden_resultado").value == "YES") {
                           redimenciona_marco_indice();
                           document.getElementById("Hidden_resultado").value = "";
                           //mueve_scroll_data_gred('CLASEDOCUMENTO', 'Panel1');
                       }
                       //mueve_scroll_data_gred('boton_expediente', 'Panel1');
                   }
                   if (elment_postbak.id == "boton_trd") {           
                           redimenciona_marco_indice();
                   }
                   if (elment_postbak.id == "boton_clase_documento") {
                       if (document.getElementById("Hidden_resultado").value == "YES") {
                           document.getElementById("Button_listar_tipos").click();
                           document.getElementById("Hidden_resultado").value = "";
                           
                       }
                       //mueve_scroll_data_gred('boton_clase_documento', 'Panel1');
                   }
                   //comman_expediente_restore
                   if (elment_postbak.id == "boton_expediente_restore") {
                       if (document.getElementById("Hidden_resultado").value == "YES") {
                           setear_expdiente();
                           document.getElementById("Hidden_resultado").value = "";

                       }
                      // mueve_scroll_data_gred('boton_expediente_restore', 'Panel1');
                   }

                   //boton_trd_restore
                   if (elment_postbak.id == "boton_trd_restore") {
                       if (document.getElementById("Hidden_resultado").value == "YES") {
                           setear_trd_documento();
                           document.getElementById("Hidden_resultado").value = "";

                       }
                      // mueve_scroll_data_gred('boton_trd_restore', 'Panel1');
                   }
                   //boton_fecha_elaboracion_restore
                   if (elment_postbak.id == "boton_fecha_elaboracion_restore") {
                       if (document.getElementById("Hidden_resultado").value == "YES") {
                           setear_fecha_elaboracion();
                           document.getElementById("Hidden_resultado").value = "";

                       }
                       mueve_scroll_data_gred('FECHAELABORACION', 'Panel1');
                   }
                   //boton_clase_documento_restore
                   if (elment_postbak.id == "boton_clase_documento_restore") {
                       if (document.getElementById("Hidden_resultado").value == "YES") {
                           setear_clase_documento();
                           document.getElementById("Hidden_resultado").value = "";

                       }
                      // mueve_scroll_data_gred('boton_clase_documento_restore', 'Panel1');
                   }
                   if (elment_postbak.id == "Button_asignar_tipo") {
                       //mueve_scroll_data_gred('CLASEDOCUMENTO', 'Panel1');

                   }
                   if (elment_postbak.id == "Button_actualiza_indice_imagen") {
                       actualiza_indice_padre();
                       actualiza_documento_seleccion_workflow();
                   }
                   
                   mueve_scroll_value(document.getElementById("Hidden_scroll").value, 'Panel1')
                   //Buton_actualizar_indice_
                  
               }

            </script>
    
        <div id="div_central" style="background-color:white; background-color:transparent; margin: auto; border-radius: 0px; padding: 0px; background:transparent; width:300px">
           <div id="div_buton" style="text-align:center;border-radius: 5px 5px 0px 0px; border: 1px solid #ccc; background-color:white">
                <asp:UpdatePanel ID="Updatepanel_actualiza" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <input id="Hidden_resultado" type="hidden" value="" runat="server">
                        <input id="Hidden_id_expediente" type="hidden" value="0" runat="server">
                        <input id="Hidden_id_tipo_expediente" type="hidden" value="0" runat="server">
                        <input id="Hidden_id_unidad_conservacion" type="hidden" value="0" runat="server">
                        <input id="Hidden_id_tipo_unidad_conservacion" type="hidden" value="0" runat="server">
                        <input id="Hidden_id_inventario" type="hidden" value="0" runat="server">
                        <asp:Button ID="Button_actualiza_indice_imagen" runat="server" Text="Actualiza Indice" CssClass="boton_azul" OnClientClick="value_scrool(); ConfirmMensajeGeneral('Desea actualizar el indice','HiddenPROMP');" Font-Size="10" Font-Names="arial" ToolTip="Actualiza Indice documento" />
                        <asp:Button ID="Button_actualiza_hiden_Expediente" runat="server" Text="Button" Style="display: none" />
                        <asp:Button ID="Buttoncerrarimpre_indice_enlace" runat="Server" Text="X" CssClass="modal_boton_hiden"
                             ToolTip="Cerrar ventana" style="float:right; display:none" OnClientClick="cierra_marco_padre();"/>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div id="indice_imagen_div" style="background-color:white;border-radius: 0px 0px 5px 5px; border: 1px solid #ccc"">
                <asp:UpdatePanel ID="ActualizaindiceImage" runat="server" UpdateMode="Conditional" RenderMode="Inline" >
                    <ContentTemplate>
                        <asp:Panel ID="Panel1" runat="server" ScrollBars="Auto"
                            Height="100%" ViewStateMode="Enabled" style="background-color:white; margin-left:7px">
                            
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
                
            </div>
             
        </div>
       
                   <input id="Hidden_esta_hiden" type="hidden" value="" runat="server">
                 <input id="Hidden_image_gabinete" type="hidden" value="580" runat="server">
                 <input id="Hiddenheih" type="hidden" value="580" runat="server">
                 <input id="Hiddennameasigna" type="hidden" value="DOCUARCHI_NET" runat="server">
                 <input id="Hidden_scroll" type="hidden" value="0" runat="server">
                 <input id="HiddenPROMP" type="hidden" value="0" runat="server">
            <!--POPUP QUE GUARDA EL POPUP CON EL CONTENEDOR DE LOS EXPEDIENTE-->  
            <asp:Panel ID="Panel_expdiente_popup" runat="server" Style="display:none;  width: 100%; height:98%;background-color:white;" CssClass="modal_content_general">
                <asp:DragPanelExtender ID="DragPanelExtender_expdiente_popup" runat="server" TargetControlID="Panel_expdiente_popup" />
                <asp:ModalPopupExtender ID="ModalPopupExtende_expdiente_popup" runat="Server" Y="0" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_expdiente_popup"
                    PopupControlID="Panel_expdiente_popup" CancelControlID="Buttoncerrar_expdiente_popup">
                </asp:ModalPopupExtender>
                <div id="divcabecer_expdiente_popup" class="modal_title_superior"  style="width: 100%">
                    <asp:Label ID="Label_expdiente_popup" runat="server" Text="Gestión expedientes" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton_expdiente_popup_idice_visor" style="float: right" >
                        <asp:Button ID="Buttoncerrar_expdiente_popup" runat="Server" Text="X" class="modal_boton_hiden"
                              ToolTip="Cerrar ventana gestión expedientes"  OnClientClick="redimenciona_padre_marco(); mueve_scroll_data_gred('CLASEDOCUMENTO', 'Panel1');"/>
                    </div>
                </div>
                <div id="Contenido_expdiente_popup" style="height: 100%; width: 100%; background-color:white" class="modal_content_back">
                    <asp:UpdatePanel ID="UpdatePanel_expdiente_popup" runat="server" UpdateMode="Conditional" style="height: 100%; background-color:white" RenderMode="Inline">
                        <ContentTemplate>
                            <iframe id="Iframe_expdiente_popup_" runat="server" style="width: 100%; height: 100%" frameborder="0"></iframe>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <asp:Button ID="Button_expdiente_popup" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_expdiente_popup" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                </div>
            </asp:Panel>
           <!--POPUP QUE GUARDA EL POPUP CON EL CONTENEDOR DE APLICACION TRD Hiddennameasigna-->            
            <input id="Hidden_id_serie" type="hidden" value="0" runat="server"/>
            <input id="Hidden_id_sub_serie" type="hidden" value="0" runat="server"/>
            <input id="Hidden_id_documento" type="hidden" value="0" runat="server"/>
            <input id="Hidden_id_area" type="hidden" value="0" runat="server"/>
            <asp:Panel ID="Panel_trd_popup" runat="server" Style="display: none; color: White; width: 100%; height: 96%" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtende_trd_popup" runat="Server" Y="1" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_trd_popup"
                    PopupControlID="Panel_trd_popup" CancelControlID="Buttoncerrar_trd_popup">
                </asp:ModalPopupExtender>
                <div id="divcabecer_trd_popup" class="modal_title_superior" style="width: 100%">
                    <asp:Label ID="Label_trd_popup" runat="server" Text="Aplicar tabla de retención" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton_trd_popup_visor_indice" style="float: right">
                        <asp:Button ID="Buttoncerrar_trd_popup" runat="Server" Text="X"
                              ToolTip="Cerrar ventana aplicar tabla" CssClass="modal_boton_hiden"  OnClientClick="redimenciona_padre_marco(); mueve_scroll_data_gred('NOMBRESERIE', 'Panel1');" />
                    </div>
                </div>
                <div id="Contenido_trd_popup" style="height: 100%; width: 100%; float: left" class="modal_content_back">
                    <asp:UpdatePanel ID="UpdatePanel_trd_popup" runat="server" UpdateMode="Conditional" style="height: 100%" RenderMode="Inline">
                        <ContentTemplate>
                            <iframe id="Iframe_trd_popup_" runat="server" style="width: 100%; height: 100%" frameborder="0"></iframe>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <asp:Button ID="Button_trd_popup" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_trd_popup" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                </div>
            </asp:Panel>
            <!--POPUP QUE APLICA EL TIPO DE DOCUMENTO -->  
            
        <input id="Hidden_valor_seleccion" type="hidden" value="" runat="server"/>     
        <asp:Panel ID="Panel_tipo_popup" runat="server" Style="display:none; color: White; width: 100%; height: 96%" CssClass="modal_content_general">
            <asp:ModalPopupExtender ID="ModalPopupExtende_tipo_popup" runat="Server" Y="1" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_tipo_popup"
                PopupControlID="Panel_tipo_popup" CancelControlID="Buttoncerrar_tipo_popup">
            </asp:ModalPopupExtender>
            <div id="divcabecer_tipo_popup" class="modal_title_superior" style="width: 100%"  >
                <asp:Label ID="Label_tipo_popup" runat="server" Text="Clasificar tipo documento" Font-Size="10" Style="float: left; margin-left:3px">
                </asp:Label>
                <div id="Divcerrarbuton_tipo_popup" style="float: right">
                    <asp:Button ID="Buttoncerrar_tipo_popup" runat="Server" Text="X" CssClass="modal_boton_hiden"
                         ToolTip="Cerrar ventana clasificar tipo documento" OnClientClick="mueve_scroll_data_gred('NOMBRESERIE', 'Panel1');" />
                </div>
            </div>
            <div id="Contenido_tipo_popup" style=" height: 100%; width: 100%; float: left" class="modal_content_back">
                <div id="Contenido_superior" style="width: 100%; height: 10%">
                    <asp:UpdatePanel ID="update_panel_drowlist" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:DropDownList ID="ComboBoxtipo" runat="server" EnableViewState="true" onchange="valor_tipo_documento();" Style="width: 100%; margin-bottom: 1px; margin-left:2px"></asp:DropDownList>
                        </ContentTemplate>
                    </asp:UpdatePanel>

                </div>
                <div id="contenido_gred" style="width: 100%; height: 80%">
                    <asp:UpdatePanel ID="UpdatePanelmensaje" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:TextBox ID="TextBoxinfotipo" runat="server" TextMode="MultiLine" Style="margin-bottom: 1px; margin-top: 2px; margin-left: 2px"></asp:TextBox>
                            <asp:Button ID="Button_lista_ayuda_tipo" runat="server" Text="Buscar" CssClass="boton" Style="margin-bottom: 5px; margin-top: 2px; margin-left: 5px; display: none" />
                        </ContentTemplate>
                        <Triggers>
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
                <div id="contenido_inferior" style="width: 100%; height: 10%">
                    <asp:UpdatePanel ID="Updatepanel_botones" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <input id="Button_aplicar_tipo" type="button" value="Aplicar" onclick="asignar_clase_documento();" style="margin-bottom: 5px; margin-top: 2px; margin-right: 5px;float:right" class="boton_azul" />
                            <asp:Button ID="Button_listar_tipos" runat="server" Text="Buscar" CssClass="boton" Style="margin-bottom: 5px; margin-top: 2px; margin-left: 5px; display: none"  />
                            <input id="Hidden_id_tipo" type="hidden" value="0" runat="server">
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                 <asp:Button ID="Button_tipo_popup" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                <asp:Button ID="ButtonSalir_tipo_popup" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
            </div>
        </asp:Panel>
    </form>
    
</body>
</html>

