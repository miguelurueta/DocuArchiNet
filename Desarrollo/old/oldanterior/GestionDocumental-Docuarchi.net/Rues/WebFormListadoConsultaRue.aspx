<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormListadoConsultaRue.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormListadoConsultaRue"      %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">

<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
  
    <title>Administración clasificación</title>
    <link href="../Styles/styleMenu.css" rel="stylesheet" type="text/css" /> 
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <link href="../Styles/Menu3.css" rel="stylesheet" />
    <script src="../js/ui/jquery-3.4.1.min.js"></script>  
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
    <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
    <link href="../ccs/themes/base/jquery.ui.all.css" rel="stylesheet" type="text/css" />   
    <link rel="stylesheet" href="../Styles/style.css" />
    <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
    <script src="../js/validate_campos.js"></script> 
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <script src="../js/ScrollableGridPlugin.js"></script>   
    <script src="../js/ScrollableGridViewPlugin_ASP.NetAJAXmin.js" type="text/javascript"></script>
    <script src="../Fixed-Header-Table-master/gridviewScroll.min.js"></script>
    <script src="../js/Rue/WebFormListadoConsultaRueNew.js"></script>
    <script src="../js/Rue/JSRue.js"></script>
    <script src="../js/java_general/general_code_java.js"></script>
    <script src="../js/java_general/general_config.js"></script>
    <script src="../js/java_general/general_control_java.js"></script>
    <script src="../js/sesion/js_sesion_gestor.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.11.0/umd/popper.min.js" type="text/javascript"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />  
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-table@1.23.1/dist/bootstrap-table.min.css"/>
    <script src="https://cdn.jsdelivr.net/npm/tableexport.jquery.plugin@1.29.0/tableExport.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap-table@1.23.1/dist/bootstrap-table.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap-table@1.23.1/dist/bootstrap-table-locale-all.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap-table@1.23.1/dist/extensions/export/bootstrap-table-export.min.js"></script>
    <script src="../js/table_boo/table_boot_config.js"></script>
    <script src="../js/java_general/BootstrapTable.js"></script>
    <script  src="../Awesome/js/all.js"></script>
    <script src="../js/java_general/ubicacion_code_java.js" type="text/javascript"></script>   
    <script src="../generic_control/FileUploadHandler.js" type="text/javascript"></script>
    <link href="../generic_control/UploadFile.css" rel="stylesheet" />
    <link href="../Awesome/css/fontawesome.css" rel="stylesheet"/>
    <link href="../Awesome/css/brands.css" rel="stylesheet"/>
    <link href="../Awesome/css/solid.css" rel="stylesheet"/>
    <script  src="../Awesome/js/brands.js"></script>
    <script  src="../Awesome/js/solid.js"></script>
    <script  src="../Awesome/js/fontawesome.js"></script>
    <link href="../Styles/w3.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
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
               
                posicion_update_pogres('progres_bar');
                elment_postbak = args.get_postBackElement();
                var elmen = document.getElementById(elment_postbak.id)
               
            }
            function CheckStatus(sender, args) {
                progres_hiden('progres_bar');
               
            }

        </script>
        
    <div id="div_parent_general"  class="p-1">    
       
        <input id="HiddenParramRue" type="hidden" value="-1" runat="server" />
        <input id="HiddenCamaraRue" type="hidden" value="-1" runat="server" />
        <div id="div_log_camara_rue" style="height: 100px">
            <img src="../imagera/logo_trd.png" alt="Girl in a jacket" class="p-1" style="height: 100px; float:left"/> 
            <img src="https://d1ubo22jqmjd7v.cloudfront.net/images/82205767e296e81508c339a8e093fda0-rues-logo.svg" alt="Girl in a jacket" class="p-1" style="height: 90px; float:right"/> 
            
        </div>
        <div id="div_title_pagina" class="row w-100 ml-0" >    
            <span class="h3" style="color:#004e82">Consulta Virtual de Expedientes</span>
        </div>
        <div id="errorgeneralrue" style="position: relative; width: 100%"></div>   
        <div id="div_table_descripcion">
            <div class="row m-0 p-0 ml-0">
                <div class="col-6 pl-0">
                    <div class="row w-100  pl-1">
                        <div class="col-4">
                            <span class="h6" id="r_2_n_label" style="color:#1883C2">Empresa</span>
                        </div>
                        <div class="col-8">
                            <span id="r_2_n_value"></span>
                        </div>
                    </div>
                    <div class="row w-100 pl-1">
                        <div class="col-4">
                            <span class="h6" id="r_1_e_label" style="color:#1883C2">Indentificación</span>
                        </div>
                        <div class="col-8">
                            <span id="r_1_e_value"></span>
                        </div>
                    </div>
                    <div class="row w-100 pl-1">
                        <div class="col-4">
                            <span class="h6" style="color:#1883C2" id="r_3_us_label">Usuario</span>
                        </div>
                        <div class="col-8">
                            <span id="r_3_us_value"></span>
                        </div>
                    </div>
                </div>
                <div class="col-6 justify-content-end">
                    <div class="row w-100 ml-0">
                        <div class="col-4">
                            <span class="h6" id="r_1_tr_label" style="color:#1883C2">Tipo registro</span>
                        </div>
                        <div class="col-8">
                            <span id="r_1_tr_value"></span>
                        </div>
                    </div>
                    <div class="row w-100 ml-0">
                        <div class="col-4">
                            <span class="h6" id="r_2_mt_label" style="color:#1883C2">Matricula</span>
                        </div>
                        <div class="col-8">
                            <span id="r_2_mt_value"></span>
                        </div>
                    </div>
                    <div class="row w-100 ml-0">
                        <div class="col-4">
                            <span class="h6" id="r_3_rs_label" style="color:#1883C2">Razón Social</span>
                        </div>
                        <div class="col-8">
                            <span id="r_3_rs_value"></span>
                        </div>
                    </div>
                </div>
            </div>
            
        </div>
        <div id="div_title_resultado" style="background-color:#006CB5; height:40px; text-align:center">
             <asp:Label ID="Label_resultado_title" runat="server" Text="Resultados" Style="font-family: Arial; font-size: 12px; color: white; margin-top: 100px"></asp:Label>
           
        </div>
        <div id="contenido_datagrid_val_radicacion" style="height: 400px; width: 100%; position: relative; border-style: ridge; border-bottom-width: 0.5px; border-left-width: 1px; border-right-width: 1px; border-top-width: 1px; top: 0px; left: 0px;">
            <table class="table-not-border_person" style="background-color: white"
                id="table_documentos_rue"
                data-pagination="true"
                data-page-list="[5,10,25, 50, 100, all]"
                data-page-size="5"
                data-show-export="false"
                data-show-refresh="false"
                data-cache="false"
                data-toggle="table"
                data-id-field="ID"
                data-unique-id="ID"
                data-click-to-select="true"
                data-search="false"
                data-locale="es-SP">
                <thead class="GridviewScrollHeader_line_blue_wite">
                </thead>
            </table>
           
        </div>
    </div>
         
        <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 200px">
            <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
            Processing ...
        </div>
        <!--mensaje_personalizado-->
           <asp:Panel ID="Panel_mensaje_personalizado" runat="server" Style="display:none; overflow:hidden; background-color: #FFFFFF" ForeColor="White" Width="400px" Height="200px"  >
                  <asp:ModalPopupExtender ID="ModalPopupExtender_mensaje_personalizado" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_mensaje_personalizado"
                      PopupControlID="Panel_mensaje_personalizado"  CancelControlID="ButtonSalir_mensaje_personalizado">
                  </asp:ModalPopupExtender>
                  <div id="Cabecerapendiente_mensaje_personalizado" class="cabecera2">
                     
                      <asp:Label ID="Label1" runat="server" Text="Mensaje del servidor" Font-Size="10" style="font-size:12px;font-family:Arial; margin-left:1px"></asp:Label>
                       <asp:Button ID="Button_mensaje_personalizado"  runat="server" style="display:none" Text="Button" Height="0px" Width="0px" />
                      <div id="Div_mensaje_personalizado" style="float: right">
                          <asp:Button ID="ButtonSalir_mensaje_personalizado" runat="Server"   Text="X"
                              ForeColor="#000066" Height="20px" style="font-size:10px" />

                      </div>
                  </div>
                  <div id="Cotenedorpendiente_mensaje_personalizado" style="border: thin double #000080; color: Black; background-color: #FFFFFF; height: 88%; width: 99%; overflow:hidden; text-align:center">   
                      <div style="height:30%"> 
                          <asp:Image ID="Image_error" runat="server"  ImageUrl="../workflow/imageneswf/1471560355_free-33.png" Width="50px" />
                      </div>  
                      <div style="height:40%"> 
                          <asp:Label ID="Label_mensaje_personalizado" runat="server" Text="Detalle"  style="font-family:Arial; font-size:11px; color:blue"></asp:Label>       
                      </div>  
                      <div style="height:25%"> </div>  
                      
                  </div>
                  
              </asp:Panel>

    </form>
    
</body>
</html>
