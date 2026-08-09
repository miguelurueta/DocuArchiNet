<%@ Page Language="vb" AutoEventWireup="true" CodeBehind="WebFormindice.aspx.vb" EnableEventValidation="false" Inherits="GestionDocumental_Docuarchi.net.WebFormindice" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
     <link href="../Styles/Aplicaction.css" rel="stylesheet" />
      <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <title></title>
     <script src="../js/ui/jquery-3.4.1.min.js"></script>  
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
     <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <script src="../js/workflow/WebFormindice.js"></script>
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <script defer src="../Awesome/js/all.js"></script>
    <link href="../Awesome/css/fontawesome.css" rel="stylesheet">
    <link href="../Awesome/css/brands.css" rel="stylesheet">
    <link href="../Awesome/css/solid.css" rel="stylesheet">
    <script defer src="../Awesome/js/brands.js"></script>
    <script defer src="../Awesome/js/solid.js"></script>
    <script defer src="../Awesome/js/fontawesome.js"></script>
    <script src="../js/validate_campos.js"></script>
</head>

<script  accesskey="javascript" type="text/javascript">  
</script>
<body  style="background-color :white; margin-top:0px">
    <form id="form1" runat="server" style="background-color:white; height:100%; margin-top:0px">
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
                 
               }
               function CheckStatus(sender, args) {
                   if (elment_postbak.id == "boton_expediente") {
                       if (document.getElementById("Hidden_resultado").value == "YES") {
                           redimenciona_marco_indice();
                           document.getElementById("Hidden_resultado").value = "";
                           //mueve_scroll_data_gred('CLASEDOCUMENTO', 'Panel1');
                       }
                       mueve_scroll_data_gred('boton_expediente', 'Panel1');
                   }
                   if (elment_postbak.id == "Button_actualiza_indice_imagen") {
                       if (window.parent.document.getElementById("hidden_selecion_actualiza_treview")) {
                           window.parent.document.getElementById("hidden_selecion_actualiza_treview").value = document.getElementById("hidden_selecion_actualiza_treview").value;
                           window.parent.document.getElementById("Button_buton_actualiza_seleccion").click();
                       }
                   }
                   if (elment_postbak.id == "boton_clase_documento") {
                       if (document.getElementById("Hidden_resultado").value == "YES") {
                           redimenciona_marco_indice();
                           document.getElementById("Button_listar_tipos").click();
                           document.getElementById("Hidden_resultado").value = "";
                           
                       }
                       mueve_scroll_data_gred('boton_clase_documento', 'Panel1');
                   }
                   //comman_expediente_restore
                   if (elment_postbak.id == "boton_expediente_restore") {
                       if (document.getElementById("Hidden_resultado").value == "YES") {
                           setear_expdiente();
                           document.getElementById("Hidden_resultado").value = "";

                       }
                       mueve_scroll_data_gred('boton_expediente_restore', 'Panel1');
                   }

                   //boton_trd_restore
                   if (elment_postbak.id == "boton_trd_restore") {
                       if (document.getElementById("Hidden_resultado").value == "YES") {
                           setear_trd_documento();
                           document.getElementById("Hidden_resultado").value = "";

                       }
                       mueve_scroll_data_gred('boton_trd_restore', 'Panel1');
                   }
                   //boton_fecha_elaboracion_restore
                   if (elment_postbak.id == "boton_fecha_elaboracion_restore") {
                       if (document.getElementById("Hidden_resultado").value == "YES") {
                           setear_fecha_elaboracion();
                           document.getElementById("Hidden_resultado").value = "";

                       }
                       mueve_scroll_data_gred('FECHAELABORACION', 'Panel1');
                   }
                   if (elment_postbak.id == "boton_trd") {
                       
                       if (document.getElementById("Hidden_resultado").value == "YES") {
                           redimenciona_marco_indice();
                           document.getElementById("Hidden_resultado").value = "";

                       }

                   }
                   //boton_clase_documento_restore
                   if (elment_postbak.id == "boton_clase_documento_restore") {
                       if (document.getElementById("Hidden_resultado").value == "YES") {
                           setear_clase_documento();
                           document.getElementById("Hidden_resultado").value = "";

                       }
                       mueve_scroll_data_gred('boton_clase_documento_restore', 'Panel1');
                   }
                   if (elment_postbak.id == "Button_asignar_tipo") {
                       mueve_scroll_data_gred('CLASEDOCUMENTO', 'Panel1');

                   }
                   if (elment_postbak.id == "Button_actualiza_indice_imagen") {
                       //mueve_scroll_data_gred('CLASEDOCUMENTO', 'Panel1');
                       //actualizar_indice();
                       //mueve_scroll_data_gred('CLASEDOCUMENTO', 'Panel1');
                       mueve_scroll_value(document.getElementById("Hidden_scroll").value, 'Panel1')


                   }
                  
                   //Buton_actualizar_indice_
                  
               }

            </script>
        
        <div id="div_central"  style="background-color: white; width:100%">
            <div id="div_buton" style="text-align:center; background-color:white; padding-bottom:2px">
                <asp:UpdatePanel ID="Updatepanel_actualiza" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <input id="Hidden_resultado" type="hidden" value="" runat="server">
                        <input id="Hidden_id_expediente" type="hidden" value="0" runat="server">
                        <input id="Hidden_id_tipo_expediente" type="hidden" value="0" runat="server">
                        <input id="Hidden_id_unidad_conservacion" type="hidden" value="0" runat="server">
                        <input id="Hidden_id_tipo_unidad_conservacion" type="hidden" value="0" runat="server">
                        <input id="Hidden_id_inventario" type="hidden" value="0" runat="server">
                         <input id="hidden_selecion_actualiza_treview" type="hidden" value="" runat="server">
                        <asp:Button ID="Button_actualiza_indice_imagen" runat="server" Text="Actualiza Indice" CssClass="boton_azul" OnClientClick="value_scrool(); actualiza_documento_seleccion_workflow();" Font-Size="10" Font-Names="arial" ToolTip="Actualiza Indice documento" />
                        <asp:Button ID="Button_actualiza_hiden_Expediente" runat="server" Text="Button" Style="display: none"  />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div id="indice_imagen_div" >
                <asp:UpdatePanel ID="ActualizaindiceImage" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <asp:Panel ID="Panel1" runat="server" ScrollBars="Auto"
                            Height="98%" ViewStateMode="Enabled" style="border-top:solid 1px #ccc">
                            
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
               
            </div>
        </div>
                  <input id="Hidden_esta_hiden" type="hidden" value="1" runat="server"/>
                 <input id="Hidden_image_gabinete" type="hidden" value="580" runat="server"/>
                 <input id="Hiddenheih" type="hidden" value="580" runat="server"/>
                 <input id="Hiddennameasigna" type="hidden" value="EXPEDIENTE_WORKFLOW" runat="server"/>
                 <input id="Hidden_scroll" type="hidden" value="0" runat="server"/>
                       
            <!--POPUP QUE GUARDA EL POPUP CON EL CONTENEDOR DE LOS EXPEDIENTE-->  
            <asp:Panel ID="Panel_expdiente_popup" runat="server" Style="display: none; color: White; width: 100%; height: 100%; margin:auto" CssClass="modal_content_general" >
                <asp:DragPanelExtender ID="DragPanelExtender_expdiente_popup" runat="server" TargetControlID="Panel_expdiente_popup" />
                <asp:ModalPopupExtender ID="ModalPopupExtende_expdiente_popup" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_expdiente_popup"
                    PopupControlID="Panel_expdiente_popup" CancelControlID="Buttoncerrar_expdiente_popup">
                </asp:ModalPopupExtender>
                <div id="divcabecer_expdiente_popup" class="modal_title_superior">     
                    <asp:Label ID="Label_expdiente_popup" runat="server" Text="Gestión expedientes" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton_expdiente_popup" style="float: right">
                        <asp:Button ID="Buttoncerrar_expdiente_popup" runat="Server" Text="X" CssClass="modal_boton_hiden"
                             ToolTip="Cerrar ventana"  OnClientClick="redimenciona_padre_marco(); mueve_scroll_data_gred('CLASEDOCUMENTO', 'Panel1');"/>
                    </div>
                </div>
                <div id="Contenido_expdiente_popup" style="color: black; background-color: #FFFFFF; height:100%; width:100%" class="modal_content_back">
                    <asp:UpdatePanel ID="UpdatePanel_expdiente_popup" runat="server" UpdateMode="Conditional" >
                        <ContentTemplate>
                            <iframe id="Iframe_expdiente_popup_" runat="server" style="width: 100%; height: 100%" frameborder="0"></iframe>
                        </ContentTemplate>
                    </asp:UpdatePanel>

                </div>
                 <asp:Button ID="Button_expdiente_popup" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_expdiente_popup" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
            </asp:Panel>
           <!--POPUP QUE GUARDA EL POPUP CON EL CONTENEDOR DE APLICACION TRD-->  
            <input id="Hidden_id_serie" type="hidden" value="0" runat="server">
            <input id="Hidden_id_sub_serie" type="hidden" value="0" runat="server">
            <input id="Hidden_id_documento" type="hidden" value="0" runat="server">
            <input id="Hidden_id_area" type="hidden" value="0" runat="server">
            <asp:Panel ID="Panel_trd_popup" runat="server" Style="display:none; color: White; width:99.5%; height:100%; margin:auto" CssClass="modal_content_general">
                <asp:DragPanelExtender ID="DragPanelExtender_trd_popup" runat="server" TargetControlID="Panel_trd_popup" />
                <asp:ModalPopupExtender ID="ModalPopupExtende_trd_popup" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_trd_popup"
                    PopupControlID="Panel_trd_popup" CancelControlID="Buttoncerrar_trd_popup">
                </asp:ModalPopupExtender>
                <div id="divcabecer_trd_popup" class="modal_title_superior" >
                    
                    <asp:Label ID="Label_trd_popup" runat="server" Text="Aplicar tabla de retención" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton_trd_popup" style="float: right">
                        <asp:Button ID="Buttoncerrar_trd_popup" runat="Server" Text="X" CssClass="modal_boton_hiden"
                             ToolTip="Cerrar ventana"  OnClientClick="redimenciona_padre_marco(); mueve_scroll_data_gred('NOMBRESERIE', 'Panel1');"/>
                    </div>
                </div>
                <div id="Contenido_trd_popup" style="color: black; background-color: #FFFFFF; height:auto; width:auto" class="modal_content_back">
                    <asp:UpdatePanel ID="UpdatePanel_trd_popup" runat="server" UpdateMode="Conditional" >
                        <ContentTemplate>
                            <iframe id="Iframe_trd_popup_" runat="server" style="width: 100%; height: 100%" frameborder="0"></iframe>
                        </ContentTemplate>
                    </asp:UpdatePanel>

                </div>
                <asp:Button ID="Button_trd_popup" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_trd_popup" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
            </asp:Panel>
            <!--POPUP QUE APLICA EL TIPO DE DOCUMENTO -->     
        <input id="Hidden_valor_seleccion" type="hidden" value="" runat="server"/>        
        <asp:Panel ID="Panel_tipo_popup" runat="server" Style="display:block; color: White; width: 99%; height: 96%" CssClass="modal_content_general">
           
            <asp:ModalPopupExtender ID="ModalPopupExtende_tipo_popup" runat="Server" Y="1" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_tipo_popup"
                PopupControlID="Panel_tipo_popup" CancelControlID="Buttoncerrar_tipo_popup">
            </asp:ModalPopupExtender>
            <div id="divcabecer_tipo_popup" class="modal_title_superior" >
                <asp:Label ID="Label_tipo_popup" runat="server" Text="Tipificación" Font-Size="10" Style="float: left">
                </asp:Label>
                <div id="Divcerrarbuton_tipo_popup" style="float: right">
                    <asp:Button ID="Buttoncerrar_tipo_popup" runat="Server" Text="X" CssClass="modal_boton_hiden"
                         ToolTip="Cerrar ventana" OnClientClick="redimenciona_padre_marco(); mueve_scroll_data_gred('NOMBRESERIE', 'Panel1');" />
                </div>
            </div>
            <div id="Contenido_tipo_popup" style="color: black; background-color: #FFFFFF; height: 99%; width: 100%" class="modal_content_back">
                <div id="Contenido_superior" style="width: 100%; height:auto; text-align:center">
                    <asp:UpdatePanel ID="update_panel_drowlist" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:DropDownList ID="ComboBoxtipo" runat="server" EnableViewState="true" onchange="valor_tipo_documento();" Style="width:98%"></asp:DropDownList>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div id="contenido_gred" style="width: 100%; height:auto; text-align:center">
                    <asp:UpdatePanel ID="UpdatePanelmensaje" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:TextBox ID="TextBoxinfotipo" runat="server" TextMode="MultiLine" Style="margin-bottom: 1px; margin-top: 1px; width:98%"></asp:TextBox>
                            <asp:Button ID="Button_lista_ayuda_tipo" runat="server" Text="Buscar" CssClass="boton" Style="margin-bottom: 5px; margin-top: 5px; margin-left: 5px; display: none" />
                        </ContentTemplate>
                        <Triggers>
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
                <div id="contenido_inferior" style="width: 100%; height:auto; text-align:right">
                    <asp:UpdatePanel ID="Updatepanel_botones" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <input id="Button_aplicar_tipo" type="button" value="Aplicar" onclick="asignar_clase_documento();" style="margin-bottom: 2px; margin-top: 2px; margin-right: 2px;" class="boton_azul" />
                            <asp:Button ID="Button_listar_tipos" runat="server" Text="Buscar" CssClass="boton" Style="margin-bottom: 5px; margin-top: 2px; margin-left: 5px; display: none" />
                            <input id="Hidden_id_tipo" type="hidden" value="0" runat="server">
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>

            </div>
            <asp:Button ID="Button_tipo_popup" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" style="display:none" />
            <asp:Button ID="ButtonSalir_tipo_popup" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" style="display:none"/>
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
    </form>
    
</body>
</html>
