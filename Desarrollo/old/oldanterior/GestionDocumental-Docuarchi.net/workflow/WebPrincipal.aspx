<%@ Page Title="" Language="vb" AutoEventWireup="true" MasterPageFile="~/Masterpage/Principal.Master" CodeBehind="WebPrincipal.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WorkflowPrincipal" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
 
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
 <script src="../js/ui/jquery-3.4.1.min.js"></script>  
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>    
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
    <link href="../ccs/themes/base/jquery.ui.all.css" rel="stylesheet" type="text/css" />
    <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <script src="../js/splitter.js"></script>
     <script src="../js/workflow/WebPrincipal.js"></script>
    <link rel="stylesheet" href="../Styles/style.css" />
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <script defer src="../Awesome/js/all.js"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet">
  <link href="../Awesome/css/brands.css" rel="stylesheet">
  <link href="../Awesome/css/solid.css" rel="stylesheet">
    <script defer src="../Awesome/js/brands.js"></script>
  <script defer src="../Awesome/js/solid.js"></script>
  <script defer src="../Awesome/js/fontawesome.js"></script>
    <style type="text/css">
.splitbarV {
float:left;width:5px;height:100%;
line-height:0px;font-size:0px;
border-left:solid 1px #9cbdff;border-right:solid 1px #9cbdff;
background:#cbe1fb url(../imagewf/panev.gif) 0% 50%;
}
.splitbarH {
height:6px;text-align:left;line-height:0px;font-size:0px;
border-top:solid 1px #9cbdff;border-bottom:solid 1px #9cbdff;
background:#cbe1fb url(../imagewf/paneh.gif) 50% 0%;
}
.ocultaleft{
margin-top:-41px;margin-left:-4px;top:50%;position:relative;
height:83px;width:5px;
background:transparent url(../imagewf/panevc.gif) 10px 50%;
}
.splitbuttonV{
margin-top:-41px;margin-left:-4px;top:50%;position:relative;
height:83px;width:5px;
background:transparent url(../imagewf/panevc.gif) 10px 50%;
}
.splitbuttonV.invert{
margin-left:0px;background:transparent url(../imagewf/panevc.gif) 0px 50%;
}
.splitbuttonH{
margin-left:-41px;left:50%;position:relative;
height:10px !important;width:83px;
background:transparent url(../imagewf/panehc.gif) 50% 0px;
}
.splitbuttonH.invert{
margin-top:-4px;background:transparent url(../imagewf/panehc.gif) 50% -10px;
}
.splitbarV.working,.splitbarH.working,.splitbuttonV.working,.splitbuttonH.working{
 -moz-opacity:.50; filter:alpha(opacity=50); opacity:.50;
}
    </style>
    <style type="text/css">
   
    </style>

        <script accesskey="javascript" type="text/javascript">
            var ModalProgress = '<%= ModalProgress.ClientID %>';
            function MantenSesion() {
                try {
                    var CONTROLADOR = "refresh_session.ashx";
                    var head = document.getElementsByTagName('head').item(0);
                    script = document.createElement('script');
                    script.src = CONTROLADOR;
                    script.setAttribute('type', 'text/javascript');
                    script.defer = true;
                    head.appendChild(script);
                }
                catch (err) {
                    alert(err.message + " Funcion MantenSesion");
                }
             }
            function Manten_service() {
                try {
                       if (document.getElementById("ContentPlacenter_Hidden_url_service").value !== "") {
                           web_service_test_db(document.getElementById("ContentPlacenter_Hidden_url_service").value);
                    }
                }
                catch (err) {
                    alert(err.message + " Funcion Manten_service");
                }
            }
            function monitera_solicitudes_pendientes() {
                //Button_activa_servicio_solicitudes_aprobacion
                document.getElementById("ContentPlacenter_Button_activa_servicio_solicitudes_aprobacion").click();
            }
            var sesion ='<%=Session.Timeout %>';
            var obje;
            setInterval('MantenSesion()', '<%= (0.9 * (Session.Timeout * 60000)) %>');  
            //setInterval('Manten_service()', '60000');
            //Remplaza el valor en la interface de treview sin crear la alarma
            setInterval('remplaza_datos_solicitudes_usuario("Respuestas pendientes por mi aprobación","Handler_lista_numero_solicitudes.ashx","","ContentPlacenter_Hidden_resultado_web_service");', '1200');
            //Remplaza el valor en la interface de treview creando la interface de alarma
            setInterval('remplaza_datos_solicitudes_usuario("Respuestas pendientes por mi aprobación","Handler_lista_numero_solicitudes_dbase.ashx","Tiene varias solicitudes pendientes por su aprobación   ","ContentPlacenter_Hidden_resultado_web_service");', '1200000');
            //Lista el numero de solicitudes sin remplazar el item del tree y genera interface de larma
            setInterval('listas_solicitudes_pendientes_por_aprobacion("","HandlerListaSolicitudesPendientesAprobacion.ashx","Tiene varias solicitudes de aprobación en pendiente, en total :   ","ContentPlacenter_Hidden_resultado_pendiente_por_aprobacion");', '1800000');
            //Remplaza el valor en la interface de treview de documentos compartidos pendientes por revisar Documentos compartidos a otros usuarios
            //setInterval('remplaza_datos_documentos_compartidos("Documentos compartidos pendientes por mi revisión","Handler_Lista_compartidos_por_revision.ashx","","ContentPlacenter_Hidden_resultado_compartido_por_revision");', '1300');
            //setInterval('listas_documentos_compartidos_por_revision("","Handler_Lista_compartidos_por_revision_db.ashx","Tiene varios documentos compartidos para revisión  ","ContentPlacenter_Hidden_resultado_compartido_por_revision");', '1300000');
            //setInterval('remplaza_datos_documentos_compartidos("Mis documentos compartidos","Handler_Lista_compartidos_para_otros_usuarios.ashx","","ContentPlacenter_Hidden_resultado_compartido_por_revision_db");', '1400');
            setInterval('set_actualiza_log_sesion_usuario_gestion_documental();', '31200');
            //setInterval('Manten_service()', '<%= (0.9 * 6000)%>');
   
    </script>
  
</asp:Content>
              
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlacenter" runat="server">
   
    <form id="form1" runat="server" style="height:105%">
        <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeout="900" >
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
                
                 posicion_update_pogres('progres_bar');
             }
             function CheckStatus(sender, args) {
                 progres_hiden('progres_bar');
                 remplaza_datos_solicitudes_usuario("Respuestas pendientes por mi aprobación", "Handler_lista_numero_solicitudes.ashx", "", "ContentPlacenter_Hidden_resultado_web_service");
                 remplaza_datos_documentos_compartidos("Documentos compartidos pendientes por mi revisión", "Handler_Lista_compartidos_por_revision.ashx", "", "ContentPlacenter_Hidden_resultado_compartido_por_revision");
                 remplaza_datos_documentos_compartidos("Mis documentos compartidos", "Handler_Lista_compartidos_para_otros_usuarios.ashx", "", "ContentPlacenter_Hidden_resultado_compartido_por_revision_db");
             }

            </script>
       
        
        <div id="splitterContainer" style="height: 100%; width: 100%; margin: 0; padding: 0;">
            <div id="cuerpoleft" style="left: auto; width: 16.5%; height: 94%; position: inherit; background-color:white; margin-top:3px" class="border_superior_radius_blanco">
                <div id="sesion" style="height: 4%; float: inherit; width: 95%; background-color:white">
                    <div id="icono" style="float: initial;display:none">
                        <asp:ImageButton ID="ImageButtonSesion_dos" runat="server" />
                        <asp:ImageButton ID="ImageButtonSesion" runat="server"
                            AlternateText=" Cerrar Sesion Gestor" ToolTip="Cerrar Sesion"
                            OnClientClick="window.location.href='../gestor.aspx'" Style="background-color: white; color: #053061" ImageUrl="~/workflow/imageneswf/cerrar.png" />

                    </div>
                    <asp:TextBox ID="TextBox_busqueda" runat="server" style="width:100%; font-size:11px; font-family:Arial; height:90% " placeholder="Busqueda.."></asp:TextBox>

                </div>

                <div id="ocultaleft" style="float: right; height:5%; width: 3%; background-color: #053061;">
                    <!--- style="left: auto; width: 5px; float:right; height: 9.5%; background-color:#053061;"!--->
                </div>

                <div id="tre" style="height: 94%; margin-top:5px; background-color: white; width: 98%;  float: left;  background-color:white; overflow:auto" >
                    <asp:UpdatePanel ID="update_tre_principal" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>                
                            
                                <asp:TreeView ID="TreeView1" Style="text-align: left; padding-left: 5px; font-size: 10px; margin-top:2px" runat="server" CssClass="TreeN_" NodeWrap="true"
                                    PopulateNodesFromClient="False" EnableViewState="true"
                                    LeafNodeStyle-CssClass="LeafNodeStyle" Font-Size="12px" NodeIndent="5" ExpandDepth="0" CollapseImageUrl="../imagera/minus-square-light.png" ExpandImageUrl="../imagera/plus-square-light.png" SkipLinkText="&quot;&quot;">
                                    <HoverNodeStyle Font-Underline="False" />
                                    <LeafNodeStyle CssClass="LeafNodeStyle" HorizontalPadding="10px" NodeSpacing="0px" VerticalPadding="5px" />
                                    <NodeStyle ChildNodesPadding="5px" HorizontalPadding="0px" NodeSpacing="5px" VerticalPadding="5px" ForeColor="Black" />
                                    <ParentNodeStyle ChildNodesPadding="0px" ForeColor="#313131" Font-Bold="true" HorizontalPadding="5px" NodeSpacing="0px" VerticalPadding="5px" />
                                    <RootNodeStyle ChildNodesPadding="0px" ForeColor="#313131" Font-Bold="true" NodeSpacing="0px" VerticalPadding="5px" HorizontalPadding="5px" />
                                    <SelectedNodeStyle ForeColor="White" CssClass="node_select_" Font-Size="10px" ImageUrl="../workflow/imageneswf/iten_list_select.png" />
                                </asp:TreeView>
                           
                            <asp:Button ID="Button_activa_servicio_solicitudes_aprobacion" runat="server" Text="Button" style="display:none" />
                            <asp:Button ID="Button_activa_busqueda_treview" runat="server" Text="Button" style="display:none" />
                            <input id="Hidden_texto_buequeda" type="hidden" value="" runat="server"/>
                             <input id="Hidden_selecion_url" type="hidden" value="" runat="server"/>
                            <input id="Hidden_url_service" type="hidden" value="" runat="server" />
                            <input id="HiddenHeigth" type="hidden" value="560" runat="server" />
                            <input id="Hidden_tipo_contenido_content" type="hidden" value="" runat="server"/>
                            <input id="Hiddenseleccion" type="hidden" value="" runat="server"/>
                            <input id="Hidden_resultado_web_service" type="hidden" value="" runat="server"/>
                            <input id="Hidden_resultado_pendiente_por_aprobacion" type="hidden" value="" runat="server"/>
                            <input id="Hidden_resultado_compartido_por_revision" type="hidden" value="" runat="server"/>
                            <input id="Hidden_resultado_compartido_por_revision_db" type="hidden" value="" runat="server"/>
                        </ContentTemplate>

                    </asp:UpdatePanel>


                </div>

            </div>

            <div id="cuerporigth" style="left: auto; width: 83.2%; height: 98%; position: relative">
                <div id="Ocultarigth"
                    style="left: auto; width: 0.4%; float: left; height: 83px; background-color: #053061; display: none; background-color: #053061">
                </div>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                         
                        <iframe id="ifrm_ds_" runat="server" style="border-style: none; border-color: #A4A4A4; border-left: solid; border-left-color:#A4A4A4; left: auto; width: 99.6%; height: 97%; float: left"
                            frameborder="0" scrolling="no" src="../workflow/WebFormDefaultSitio.aspx"> </iframe>
                        
                    </ContentTemplate>
                    <Triggers>


                        <asp:AsyncPostBackTrigger ControlID="ImageButtonSesion" />

                    </Triggers>
                </asp:UpdatePanel>
            </div>
            <div id="pie_cuerpo_left" style="width: 16.5%; height: 4%; float: left; background-color: white; text-align: left; text-overflow: clip; white-space: nowrap; overflow: hidden; min-width: 0px" class="border_inferior_radius_blanco">
                <asp:Label ID="LabelEstado" runat="server" Text="Estado " Style="color: black; font-family: Arial"></asp:Label>
            </div>
        </div>
       
        <!-- The Modal -->
        <div id="myModal" class="modal">

            <!-- Modal content -->
            <div class="modal-content">
                <span class="close" onclick="hide_autonomo();">&times;</span>
                <p id="tex_modal">Some text in the Modal..</p>
            </div>

        </div>
         <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 200px">
                <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
                Processing ...
            </div>
        <asp:Panel ID="panelUpdateProgress" runat="server" CssClass="updateProgress">
            <asp:UpdateProgress ID="UpdateProg1" DisplayAfter="0" runat="server">
                <ProgressTemplate>
                    <div style="position: relative; top: 30%; text-align: center; display:none">
                        <img src="loading.gif" style="vertical-align: middle" alt="Processing" />
                        Processing ...
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
        </asp:Panel>
        <asp:ModalPopupExtender ID="ModalProgress" runat="server" TargetControlID="panelUpdateProgress"
            BackgroundCssClass="modalBackground" PopupControlID="panelUpdateProgress" />
        <div id="Conrtenido_div" style="display:none">
             <asp:UpdatePanel ID="UpdatePanel_webservice" runat="server" UpdateMode="Conditional">
                 <ContentTemplate>
                     <asp:Button ID="Button_service" runat="server" Text="Button" />

                 </ContentTemplate>
             </asp:UpdatePanel>
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
     
      

</asp:Content>
