<%@ Page Title="Reportes Workflow" Language="vb" AutoEventWireup="false"   CodeBehind="WebFormReportesWorkflow.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormReportesWorkflow" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
   
    <title> Reportes workflow</title>
     <script src="../js/ui/jquery-3.4.1.min.js"></script>  
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
      <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
     <script defer src="../Awesome/js/all.js"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet">
  <link href="../Awesome/css/brands.css" rel="stylesheet">
  <link href="../Awesome/css/solid.css" rel="stylesheet">
    <script defer src="../Awesome/js/brands.js"></script>
  <script defer src="../Awesome/js/solid.js"></script>
  <script defer src="../Awesome/js/fontawesome.js"></script>
   <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
     <link rel="stylesheet" href="../Styles/style.css" />
   <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <script src="../js/workflow/WebFormReportesWorkflow.js"></script>
    <script src="../js/validate_campos.js"></script> 
    
</head>   
    <body id="pricnipal" style="background-color:white; margin:0px">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" EnablePageMethods="true" AsyncPostBackTimeout="900">
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
                try {
                    elment_postbak = args.get_postBackElement();
                    posicion_update_pogres('progres_bar');

                }
                catch (err) {
                    alert(err.message + " Funcion InitializeRequest");
                }
            }
            function CheckStatus(sender, args) {
                try {

                    if (elment_postbak.id == "Button1") {
                        
                        if (document.getElementById("Hidden_ruta_archivo").value !== "") {
                            //plugin_grwedview();
                            document.getElementById('ifmExcel_').src = "../radicador/WebFormDescargaRadicado.aspx";
                        }
                    }
                   
                    progres_hiden('progres_bar');

                }
                catch (err) {
                    alert(err.message + " Funcion CheckStatus");
                }
            }

            </script>
        <div id="contendor_reporte" style="width: 100%; margin: 0px auto; height: 100%"  class="row">
            <div id="derecha" style="height: 100%; background-color:white; border-right:1px solid #F2F2F2" class="col-4 p-0">
                <div id="reportes" style=" height: 60%" class="d-block">
                    <div style="text-align:left" class="nav_botota_person_gray modal_content_no_back_inferior p-3" id="div_title_reportes">
                        <asp:Label ID="Label1" runat="server"   CssClass="h6"
                        Text="Lista de reportes" Width="100%" ></asp:Label>
                    </div>             
                    <asp:Panel ID="Panel_reportes" runat="server" BackColor="white" ScrollBars="Auto"
                        Style="width: 100%; height: 90%">
                        <asp:UpdatePanel ID="update_tre_principal" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                 <asp:TreeView ID="TreeView1" runat="server" NodeWrap="true"  CssClass="font-weight-light h6 pl-0 " Style="font-family:'Segoe UI'" 
                                      NodeStyle-NodeSpacing="0.1px" 
                                      LeafNodeStyle-CssClass="LeafNodeStyle_2_  mb-1 pl-1  " ExpandDepth="0" NodeIndent="1"   PopulateNodesFromClient="False">
                                      <HoverNodeStyle Font-Underline="True" />
                                      <SelectedNodeStyle CssClass="select_treview_boottra font-weight-normal" />
                                      <ParentNodeStyle Font-Bold="False" />
                                      <HoverNodeStyle Font-Underline="True" ForeColor="Purple" />
                                     <NodeStyle CssClass="nav-link-treview mt-1 mb-1 pl-1  " ForeColor="black"
                                            VerticalPadding="0px" />
                                     
                                      <RootNodeStyle />
                                  </asp:TreeView>
                             
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </asp:Panel>
                </div>
                <div id="parametros" style="width:100%; height:39%; overflow:auto; top: 0px; left: 0px; border-top:1px solid #F2F2F2" >
                    <asp:UpdatePanel ID="UpdatePanel_parametros" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <input id="Hidden_parametro_sel" type="hidden" value="" runat="server">  
                            <asp:Panel ID="Panel_parametros_consulta" runat="server" ScrollBars="Auto"
                                Style="width: 100%; float: left; height: 100%;" BackColor="White" CssClass="p-0">
                                <div style="text-align: left; width:100%" class="nav_botota_person_gray modal_content_no_back_inferior p-3">
                                    <asp:Label ID="Label3" runat="server"  CssClass="h6"
                                        Text="Parametros Consulta" Width="100%"></asp:Label>
                                </div>
                                <div class="pt-3">
                                    <asp:Table ID="Tableparametro" runat="server" Style="width:100%" >
                                        <asp:TableRow ID="TableRow1" runat="server" Style="width:100%">
                                        </asp:TableRow>
                                    </asp:Table>
                                </div>
                                <div>
                                    <asp:Button ID="Button_reporte" runat="server" Text="Consulta" Visible="false" style="margin-left:5px; margin-top:5px" CssClass=" btn btn-success mt-2" />
                                </div>                     
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                
            </div>
            <div id="resultados" style=" height: 90%; background-color:white;
             border:initial" class="col-8 p-0">
                <asp:UpdatePanel ID="UpdatePanel_conenido" runat="server" UpdateMode="Conditional"  style="width: 100%; height: 100%">
                    <ContentTemplate>
                         <div  style="text-align:left" id="div_resultado" class="nav_botota_person_gray modal_content_no_back_inferior p-3">
                             <asp:Label ID="Label_resultado" runat="server" 
                             ForeColor="Black"  CssClass="h6"
                            Text="Resultados" Width="100%" ></asp:Label>
                         </div>
                        <div id="div_pan_radicacion" style="overflow:auto">
                             <asp:GridView ID="GridView_val_radicacion" runat="server" Width="100%" EnableViewState="True" GridLines="None"  style=" font-family:Segoe UI"
                            AutoGenerateSelectButton="False" CssClass="table font-weight-light"  PagerSettings-Position="Top" AllowSorting="false" >
                            <SelectedRowStyle BackColor="LightSkyBlue"  />
                           <HeaderStyle CssClass="GridviewScrollHeader_line_boot" />
                            <Columns>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        </div>
                       
                    </ContentTemplate>
                    <Triggers>
                    </Triggers>
                </asp:UpdatePanel>
                <asp:UpdatePanel ID="UpdatePanel_consulta" runat="server" UpdateMode="Conditional"  RenderMode="Inline">
                <ContentTemplate>            
                    <div id="Opciones" style="width: 100%; height: 10%; float: right; 
                      border:initial; 
                     text-align:right">
                        <asp:Label ID="label_result" runat="server" Text="" Font-Size="8px" Style="margin-left:20px"></asp:Label>
                        <input id="btnPrint" type="button" value="Imprimir" style="margin-top:5px; display:none" />
                        <asp:Button ID="Button1" runat="server" Text="Exportar" CssClass="btn btn-success " style="margin-top:5px; margin-right:5px "  OnClientClick="retorna_colum_mtriz('Hidden_colum_header');"   />                   
                        <input id="Hidden_ruta_archivo" type="hidden" value="" runat="server">  
                        
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            </div>
            
        </div>
        <div style=" clear:both"></div>
         <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 200px">
                <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
                Processing ...
            </div>
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
        <input id="Hidden_colum_header" type="hidden" value="" runat="server">        
        <div id="inferior_bajo_boton" style="width: 0%; height: 0%; background-color: #E7EDF5; display: none">
             
            <asp:UpdatePanel ID="updatapanel_iframe" runat="server" UpdateMode="Conditional">
                <ContentTemplate>   
                     <iframe runat="server" id="ifmExcel_" width="0" height="0" marginheight="0" marginwidth="0"
                        frameborder="0" />
                   
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </form>
</body>
</html>
