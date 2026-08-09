<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormRemisionCorrespondencia.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormRemisionCorrespondencia" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Remisión Corresponencia</title>
     <script src="../js/ui/jquery-3.4.1.min.js"></script>  
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
    <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
   <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
     <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />   
    <script src="../js/radicacion/WebFormRemisionCorrespondencia.js"></script>
    <script src="../js/validate_campos.js"></script>
     <script defer src="../Awesome/js/all.js"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet">
  <link href="../Awesome/css/brands.css" rel="stylesheet">
  <link href="../Awesome/css/solid.css" rel="stylesheet">
    <script defer src="../Awesome/js/brands.js"></script>
  <script defer src="../Awesome/js/solid.js"></script>
  <script defer src="../Awesome/js/fontawesome.js"></script> 
    
</head>
<body style="background-color:#A4A4A4">
    <form id="form1" runat="server">
   
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" EnablePageMethods="true">
            </asp:ScriptManager>
            <script accesskey="javascript" type="text/javascript">
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
                    if (elmen.type == "button" || elmen.type == "submit") {
                        value_element = elmen.value;
                        elmen.value = "Espere..."
                        elmen.disabled = true;
                    }
                }
                function CheckStatus(sender, args) {
                    try {
                        if (elment_postbak.type == "button" || elment_postbak.type == "submit") {
                            elment_postbak.value = value_element;
                            elment_postbak.disabled = false;
                        }
                    }
                    catch (err) {
                        alert(" Funcion CheckStatus asincrona WebFormRemisionCorrespondencia.aspx" + err.message);
                    }
                    finally {
                        progres_hiden('progres_bar');
                    }
                }
            </script>
        <div style="text-align: left; width: 50%; background-color: #FAFAFA; margin: auto; border-radius: 5px; padding: 20px; background: white; height: 100%" class="mt-4">
            <div id="modal_content_" class="modal-content_">
                <div id="superior" class="modal_title_superior_ modal-header">
                    <asp:Label ID="Label5" runat="server" Text="Lista remisión correspondencia" class="modal-title d-inline ml-1 h6"></asp:Label>
                </div>
                <div id="contenido" style="width: 100%; height: 100%; overflow: auto; border-top: none" class="modal-body">
                    <asp:UpdatePanel ID="update_general" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="row mt-2">
                                <div class="col-5">
                                    <span>Plantilla radicación *</span>
                                </div>
                                <div class="col-6">
                                    <asp:DropDownList ID="DropDownList_plantilla" runat="server" Style="width: 100%" CssClass="custom-select"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="row mt-2">
                                <div class="col-5">
                                    <span>Area-Dependencia *</span>
                                </div>
                                <div class="col-6">
                                    <asp:DropDownList ID="DropDownList_area" runat="server" Style="width: 100%" AutoPostBack="true" CssClass="custom-select"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="row mt-2">
                                <div class="col-5">
                                    <span>Destinatario-Remitente *</span>
                                </div>
                                <div class="col-6">
                                    <asp:DropDownList ID="DropDownList_dest_tremit" runat="server" Style="width: 100%" CssClass="custom-select"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="row mt-2">
                                <div class="col-5">
                                    <span>Rango Inicial *</span>
                                </div>
                                <div class="col-6">
                                    <div class="row w-100 p-0">
                                        <div class="col-8">
                                            <div class="row">
                                                <div class="col-8">
                                                    <asp:TextBox ID="TextBox_fecha_ini" runat="server" Width="100%" onkeypress="return validate_fecha(event,this)" placeholder="0000 00 00"></asp:TextBox>
                                                    <asp:CalendarExtender ID="CalendarExtender_ini" runat="server" PopupButtonID="ImageButton_ini" TargetControlID="TextBox_fecha_ini" Format="yyyy-MM-dd" />
                                                </div>
                                                <div class="col-2 p-0">
                                                    <button class="ml-1 btn border-0" id="ImageButton_ini" type="button">
                                                        <i class="fad fa-calendar-alt fa-1x"></i>
                                                    </button>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-4">
                                            <asp:DropDownList ID="DropDownList_hora_ini" runat="server" Width="48px"></asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row mt-2">
                                <div class="col-5">
                                    <span>Rango Final *</span>
                                </div>
                                <div class="col-6">
                                    <div class="row w-100 p-0">
                                        <div class="col-8">
                                            <div class="row">
                                                <div class="col-8">
                                                    <asp:TextBox ID="TextBox_fin" runat="server" Width="100%" onkeypress="return validate_fecha(event,this)" placeholder="0000 00 00"></asp:TextBox>
                                                    <asp:CalendarExtender ID="CalendarExtender_fin" runat="server" PopupButtonID="ImageButton_fin" TargetControlID="TextBox_fin" Format="yyyy-MM-dd" />
                                                </div>
                                                <div class="col-2 p-0">
                                                    <button class="ml-1 btn border-0" id="ImageButton_fin" type="button">
                                                        <i class="fad fa-calendar-alt fa-1x"></i>
                                                    </button>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-4">
                                            <asp:DropDownList ID="DropDownList_hora_fin" runat="server" Width="48px"></asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                           
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div class="modal-footer justify-content-end" id="modal-footer">  
                    <asp:UpdatePanel ID="UpdatePanel_boton_generar" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                             <asp:Button ID="Button_generar" runat="server" Text="Aceptar"  CssClass="btn btn-success" />
                             <input id="Hidden_ruta_archivo" type="hidden" value="" runat="server">
                        </ContentTemplate>
                    </asp:UpdatePanel>
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
                <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 200px">
                    <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
                    Processing ...
                </div>
                <div id="pie_tol" style="display:none">
                    <asp:UpdatePanel ID="updatapanel_iframe" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <iframe runat="server" id="ifmExcel_" width="0" height="0" marginheight="0" marginwidth="0"
                                frameborder="0" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
        </div>
    </form>
</body>
</html>
