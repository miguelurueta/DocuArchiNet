<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormDaExportArchivo.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormDaExportArchivo" EnableViewState="False" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <script src="../js/ui/jquery-3.4.1.min.js"></script>
    <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
     <link href="../Styles/bootra-person.css" rel="stylesheet" />     
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
    <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
     <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <script src="../js/Docuarchi/WebFormDaExportArchivo.js"></script>
     <script  src="../Awesome/js/all.js"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet"/>
     <link href="../Awesome/css/brands.css" rel="stylesheet"/>
     <link href="../Awesome/css/solid.css" rel="stylesheet"/>
     <script  src="../Awesome/js/brands.js"></script>
     <script  src="../Awesome/js/solid.js"></script>
     <script  src="../Awesome/js/fontawesome.js"></script> 
    <style>
 
.waterMark
{  height: 16px;
           width: 168px;
           padding: 2px 2 2 2px;
           border: 1px solid #BEBEBE;
           background-color: #F0F8FF;
           color: gray;
           font-size: 8pt;
           text-align: center;
}
</style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"
            EnableScriptGlobalization="True" EnablePageMethods="True">
        </asp:ScriptManager> 
       <script accesskey="javascript" type="text/javascript">
          
        </script>
           <script accesskey="javascript" type="text/javascript">

            Sys.WebForms.PageRequestManager.getInstance().add_initializeRequest(InitializeRequest);
            Sys.Application.add_load(ApplicationLoadHandler)
            var elment_postbak;
            var value_element;
            function ApplicationLoadHandler(sender, args) {

                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(CheckStatus);

            }
            function InitializeRequest(sender, args) {
                //
                try {
                elment_postbak = args.get_postBackElement();
                posicion_update_pogres('progres_bar');
              
                var elmen = document.getElementById(elment_postbak.id)
                if (elmen.type == "button" || elmen.type == "submit") {
                    value_element = elmen.value;
                    elmen.value = "Espere..."
                    elmen.disabled = true;
                }
            }
              catch (err) {
                  alert(err.message + " Funcion InitializeRequest");
               }
            }

            function CheckStatus(sender, args) {
                try {
                progres_hiden('progres_bar');
                if (elment_postbak.id == 'Button_exportar') {
                    if (document.getElementById("Hidden_respuesta").value == "YES") {
                        document.getElementById("Hidden_respuesta").value == "NO";
                        hiden_marco_padre_descarga();
                    }
                    
                }
                if (elment_postbak.type == "button" || elment_postbak.type == "submit") {
                    elment_postbak.value = value_element;
                    elment_postbak.disabled = false;
                }
                //redimenciona_marco_descarga();
                }
                catch (err) {
                    alert(err.message + " Funcion CheckStatus");
                }
            }

        </script>
        <div id="ContenidoImpresion" style="color: black; background-color: #FFFFFF; height: auto; width: auto" class="m-2">
            <div style="margin: 15px">
                <div id="Table1">
                    <asp:Panel ID="Table1v" runat="server" Visible="false">
                        <div class="row" style="">
                            <div class="col-sm-12 pl-0 pr-0" style="border-bottom: 1px ridge #b0c4de">
                                <label style="color: black; font-family: Arial" class="h6 font-weight-light">Selección de páginas a exportar  </label>
                            </div>
                        </div>

                        <div class="row " style="background:#6d7fcc; border-bottom: 1px ridge white">
                            <div class="col-sm-6">
                                <asp:RadioButton ID="RadioButton_todo" runat="server" Checked="true" GroupName="data" />
                                <label id="tod_1" style="font-family:'Segoe UI Emoji'; font-size: 14px; color:white">Todas las páginas </label>
                            </div>

                        </div>
                        <div class="row pt-2" style="background:#6d7fcc; border-bottom: 1px ridge white" >
                            <div class="col-sm-6">
                                <asp:RadioButton ID="RadioButton_rango" runat="server" GroupName="data" />
                                <label id="barr" style="font-family: 'Segoe UI Emoji'; font-size: 14px; color:white">Rangos de páginas</label>
                            </div>
                            <div class="col-sm-6">
                                <asp:TextBox ID="TextBox_ini" runat="server" Style="width: 200px" placeholder="Seleción 1,2,3.. Rangos 1-5" CssClass="solo-numero form-control"></asp:TextBox>
                                <label id="barr1_2" style="color: red; font-family: Arial; font-size: 11px; color:white">Las paginas individuales se separan con  comas (Ej; 1,5..) </label>
                                <label id="barr1_3" style="color: red; font-family: Arial; font-size: 11px; color:white">los rangos de paginas se definen separados por un guion (ej; 1-5) </label>
                            </div>
                        </div>
                        <div class="row">
                            <div class=" col-sm-12" style="border-top: 1px ridge #b0c4de; border-bottom: 1px ridge #b0c4de; display:none">
                                <label style="color: black; font-family:'Segoe UI Emoji'; font-weight: 600" class="font-weight-light">Formato de Exportación de documentos </label>
                            </div>
                        </div>
                        <div class="row mt-1">
                            <div class="col-sm-4 pl-0 ">
                                <asp:RadioButton ID="RadioButton_docuarchi" runat="server" GroupName="data_1" Checked="true" />
                                <label id="barr_1" style="font-family: Arial; font-size: 11px">Exporta Formato Docuarchi</label>
                            </div>
                            <div class="col-sm-4 pl-0 pr-0">
                                <asp:RadioButton ID="RadioButton_pdf" runat="server" GroupName="data_1" />
                                <label id="barr_2" style="font-family: Arial; font-size: 11px">Exporta Formato Pdf</label>
                            </div>
                            <div class="col-sm-4 pl-0 pr-0">
                                <asp:CheckBox ID="CheckBoxEporta" runat="server" Checked="false" Text=" Estamp Seguridad pdf" Style="font-family: Arial; font-size: 11px" />
                            </div>
                        </div>
                    </asp:Panel>
                    <asp:Panel ID="Panel_stamp_pdf" runat="server" Visible="false">
                        <div class="row mt-1">     
                            <div class="col-sm-4 pl-0 pr-0">
                                <asp:CheckBox ID="CheckBox_stmp_pdf" CssClass="ml-2" runat="server" Checked="false" Text=" Estamp Seguridad para pdf" Style="font-family: Arial; font-size: 14px" />
                            </div>
                        </div>
                    </asp:Panel>
                    <div id="conten_boton" class="row  justify-content-end">
                        <asp:UpdatePanel ID="updatepanel_imprimir" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Button ID="Button_exportar" runat="server" Text="Descargar" class="btn btn-primary" Style="float: right; margin-top: 3px; margin-right: 1px; background:#6d7fcc" />
                                <input type="hidden" id="Hidden_respuesta" name="Hidden_respuesta" runat="server" value="NO" />
                                <input type="hidden" id="sid2" name="sid2" runat="server" value="" />
                                <input type="hidden" id="hideextension" name="hideextension" runat="server" value="" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>

                </div>

            </div>

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
        <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 50px">
            <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
            Processing ...
        </div>
        <asp:UpdatePanel ID="updatapanel_iframe" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <input id="Hidden_ruta_archivo" type="hidden" value="" runat="server">
                <iframe runat="server" id="ifmExcel_" width="0" height="0" marginheight="0" marginwidth="0" 
                    frameborder="0"  />
            </ContentTemplate>
        </asp:UpdatePanel>
       
    </form>
</body>
</html>
