<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="web_form_consulta_publica.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.web_form_consulta_publica" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Consulta publica expedientes</title>
    <script src="../js/ui/jquery-3.4.1.min.js"></script>  
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
    <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
    <link href="../ccs/themes/base/jquery.ui.all.css" rel="stylesheet" type="text/css" />   
    <link rel="stylesheet" href="../Styles/style.css" />
    <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
    <script src="../js/validate_campos.js"></script> 
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <script src="../js/Publico/web_form_consulta_publica.js"></script>
    <script src="../js/java_general/general_code_java.js"></script>
    <script src="../js/java_general/general_config.js"></script>
    <script src="../js/java_general/general_control_java.js"></script>
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
        <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeout="900">
            
        </asp:ScriptManager>
        <script accesskey="javascript" type="text/javascript">
                Sys.WebForms.PageRequestManager.getInstance().add_initializeRequest(InitializeRequest);
                Sys.Application.add_load(ApplicationLoadHandler)
                var elment_postbak;
                function ApplicationLoadHandler(sender, args) {
                    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(CheckStatus);
                }
                function InitializeRequest(sender, args) {    
                    elment_postbak = args.get_postBackElement();                
                    posicion_update_pogres('progres_bar');       
                }
                function CheckStatus(sender, args) {
                    try {     
                    progres_hiden('progres_bar');
                      
                    }
                      catch (err) {
                          alert(err.message + " Funcion CheckStatus");
                    }
                }
                
        </script>   
        <header>
            <div style="margin-left:13%; margin-right:13%">
                <div class="row  pt-2 pb-2">
                <div class="col-6 justify-content-end" >
                    <img src="../imagera/logo_trd.png" alt="Girl in a jacket" style="height: 50px; float:left"/>            
                    <div class="rotulo-general-servicio-virtual hidden-xs hidden-sm">Servicios Virtuales</div>
                </div>
                    <div class="col-1  justify-content-start">
                        <a id="help_sear_documente" class="rotulo-general-servicio-virtual" style="border-left:none" data-bs-toggle="tooltip" data-bs-placement="top" title="Ayuda módulo de consulta">
                            <i class="fal fa-info-circle"></i> 
                        </a>
                    </div>
                    <div class="col-5 justify-content-start">     
                        <span class="rotulo-general-servicio-virtual hidden-xs hidden-sm" style="border-left: none; float: right">Consulta de expedientes</span>
                        <span id="tor_ico"></span>
                    </div>
            </div>
            </div>
            
        </header>
        <div class="modal-body-fullscreen ">
            <ul class="nav nav-tabs" id="myTab" role="tablist">
                <li class="nav-item" role="presentation">
                    <button class="nav-link active d-none" id="home-tab" data-bs-toggle="tab" data-bs-target="#home_datos_ingreso" type="button" role="tab" aria-controls="home" aria-selected="true">Home</button>
                </li>
                <li class="nav-item" role="presentation">
                    <button class="nav-link d-none" id="matri-tab" data-bs-toggle="tab" data-bs-target="#consulta_expedientes_matricualados" type="button" role="tab" aria-controls="profile" aria-selected="false">Profile</button>
                </li>
                <li class="nav-item d-none" role="presentation">
                    <button class="nav-link" id="actos-tab" data-bs-toggle="tab" data-bs-target="#consulta_actos_registro_matriculado" type="button" role="tab" aria-controls="messages" aria-selected="false">Messages</button>
                </li>
                <li class="nav-item d-none" role="presentation">
                    <button class="nav-link" id="settings-tab" data-bs-toggle="tab" data-bs-target="#documentos_actos_registro_matriculado" type="button" role="tab" aria-controls="settings" aria-selected="false">Settings</button>
                </li>
            </ul>

            <div class="tab-content container body-content" >
                <div class="tab-pane  pt-3 col-xs-12" style="height:100%" id="home_datos_ingreso" role="tabpanel" aria-labelledby="home-tab">
                    <h3 class="rotulo-general_title"> Datos de Ingreso </h3>
                    <div id="error_div_datos_ingreso" style="position: relative; width: 100%"></div>
                    <div id="div_datos_ingreso" style="height: 100%">

                    </div>
                    
                    <div class=" modal-footer">
                        <button type="button" id="Button_div_datos_ingreso" class="btn  btn-success" title="">Aceptar</button>
                        <button type="button" id="Button_sesion" class="btn  btn-success d-none" title="">Actualizar</button>
                    </div>
                </div>
                <div class="tab-pane " id="consulta_expedientes_matricualados" role="tabpanel" aria-labelledby="profile-tab">
                    <h3 class="rotulo-general_title"> Consulta matriculados </h3>
                    <div id="error_div_consulta_matriculado" style="position: relative; width: 100%"></div>   
                    <div id="div_consulta_expedientes_matricualados" style="height: 100%">
                        <div class="row w-100 ml-0 pt-3 pb-4">
                            <div class="">
                                <a class="ml-1 mr-1" data-bs-toggle="tooltip" data-bs-placement="top" title="Debe seleccionar el tipo de registro público sobre el cual necesita generar la consulta, los registros habilitados actualmente por la entidad son Registro mercantil (Registro de todos los comerciantes matriculados), Registro Esal (Registros de todas las entidades sin ánimo de lucro)  Registro rup (Registro de todos los proponentes adscritos para contratar con el estado)">
                                    <i class="fal fa-info-circle"></i>
                                </a>
                            </div>
                            <div class="col-4  ml-0 mr-0">
                                <select style=" font-weight:600" id="option_registro_expediente" class="form-select form-select-lg form-control" >
                                    <option></option>
                                </select>
                            </div>
                            <div class="input-group col-7  pl-0 ">
                                <input id="textBox_buequeda_matricualdo_gabinete" type="text" class="form-control form-control-sm complex  border-left-1" placeholder="Consulte los datos del matriculado (nit, matricula y razón social)" />
                                <div class="input-group-append">
                                    <button class="btn btn-outline-secondary" id="Button_search_matriculado" title="Consulta matricualdos y sus expedientes" type="button">
                                        <i class="fal fa-search"></i>
                                    </button>
                                    <div class="">
                                        <a class="ml-1 mr-1" data-bs-toggle="tooltip" data-bs-placement="top" title="La consulta se realiza sobre el tipo de registro público seleccionado, la consulta se puede realizar por los campos número de matrícula del matriculado, NIT o identificación del matriculado, o la razón social del matriculado">
                                            <i class="fal fa-info-circle"></i>
                                        </a>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div id="contenido_table_boot_consulta_matriculado" class="p-1" style="height: 100%; width: 100%; position: relative; margin-top: 1px; background-color: white">
                            <table class="table-not-border_person" style="background-color: white"
                                id="table_consulta_matriculado"
                                data-pagination="true"
                                data-page-list="[5,10, 25, 50, 100, all]"
                                data-page-size="5"
                                data-show-export="false"
                                data-show-refresh="false"
                                data-cache="false"
                                data-toggle="table"
                                data-id-field="MATRICULA"
                                data-unique-id="MATRICULA"
                                data-click-to-select="true"
                                data-search="false"
                                data-locale="es-SP">
                                <thead class="GridviewScrollHeader_line_boot_gray">
                                </thead>
                            </table>
                        </div>
                        
                    </div>            
                    
                    <div class=" modal-footer">
                        
                    </div>
                </div>
                <div class="tab-pane " id="consulta_actos_registro_matriculado" role="tabpanel" aria-labelledby="messages-tab">
                    <h3 class="rotulo-general_title"> Actos del matriculado </h3>
                    <div id="error_consulta_actos_registro_matriculado" style="position: relative; width: 100%"></div>   
                    <div id="div_consulta_actos_registro_matriculado" style="height: 100%">
                        <div class="row m-0 p-0">
                            <div class="col-8">
                                <div class="row w-100 ml-0">
                                    <div class="col-4">
                                        <sopan class="h6" id="r_1_label"></sopan>
                                    </div>
                                    <div class="col-8">
                                        <sopan id="r_1_value"></sopan>
                                    </div>
                                </div>
                                <div class="row w-100 ml-0">
                                    <div class="col-4">
                                        <sopan class="h6" id="r_2_label"></sopan>
                                    </div>
                                    <div class="col-8">
                                        <sopan id="r_2_value"></sopan>
                                    </div>
                                </div>
                                <div class="row w-100 ml-0">
                                    <div class="col-4">
                                        <sopan class="h6" id="r_3_label"></sopan>
                                    </div>
                                    <div class="col-8">
                                        <sopan id="r_3_value"></sopan>
                                    </div>
                                </div>
                            </div>
                            <div class="col-4">
                                <div class="row  justify-content-end">
                                    <div class="col-5 justify-content-end">
                                        <div class="nav-item active_">
                                            <a class="nav-link active ml-1 h6" id="Button_back" title="Regresar a la consulta de matriculados" style="color: black" href="#"><i style="color: black" class="fal fa-arrow-left"></i>
                                                Regresar
                                            </a>
                                        </div>
                                    </div>

                                </div>
                            </div>
                        </div>  
                        <div id="contenido_consulta_actos_registro_matriculado" class="p-1" style="height: 100%; width: 100%; position: relative; margin-top: 1px; background-color: white">
                            <table class="table-not-border_person" style="background-color: white"
                                id="table_consulta_actos_registro_matriculado"
                                data-pagination="true"
                                data-page-list="[5,10, 25, 50, 100, all]"
                                data-page-size="5"
                                data-show-export="false"
                                data-show-refresh="false"
                                data-cache="false"
                                data-toggle="table"
                                data-id-field="ENLACE"
                                data-unique-id="ENLACE"
                                data-click-to-select="true"
                                data-search="true"
                                data-locale="es-SP">
                                <thead class="GridviewScrollHeader_line_boot_gray">
                                </thead>
                            </table>
                        </div>
                    </div>
                    
                     <div class=" modal-footer">
                        
                    </div>
                </div>
                <div class="tab-pane " id="documentos_actos_registro_matriculado" role="tabpanel" aria-labelledby="settings-tab">
                    <h3 class="rotulo-general_title"> Documentos actos del matriculado </h3>
                    <div id="error_documentos_actos_registro_matriculado" style="position: relative; width: 100%"></div>   
                    <div id="div_documentos_actos_registro_matriculado" style="height: 100%">
                        <div class="row m-0 p-0">
                            <div class="col-8">
                                <div class="row w-100 ml-0">
                            <div class="col-4">
                                <sopan class="h6" id= "a_1_label" ></sopan>
                            </div>
                            <div class="col-8">
                                <sopan id="a_1_value"></sopan>
                            </div>
                        </div>
                        <div class="row w-100 ml-0">
                            <div class="col-4">
                                <sopan class="h6" id= "a_2_label"></sopan>
                            </div>
                            <div class="col-8">
                                <sopan id="a_2_value"></sopan>
                            </div>
                        </div>
                        <div class="row w-100 ml-0">
                            <div class="col-4">
                                <sopan  class="h6" id= "a_3_label"></sopan>
                            </div>
                            <div class="col-8">
                                <sopan id="a_3_value"></sopan>
                            </div>
                        </div>
                            </div>
                            <div class="col-4">
                                <div class="row  justify-content-end">
                                    <div class="col-5 justify-content-end">
                                        <div class="nav-item active_">
                                            <a class="nav-link active ml-1 h6" id="Button_back_docu_actos" title="Regresar a consulta de actos del matriculado" style="color: black" href="#"><i style="color: black" class="fal fa-arrow-left"></i>
                                                Regresar
                                            </a>
                                        </div>
                                    </div>

                                </div>
                            </div>
                        </div>      
                        <div id="contenido_documentos_actos_registro_matriculado" class="p-1" style="height: 370px; width: 100%; position: relative; margin-top: 1px; background-color: white">
                            <table class="table-not-border_person" style="background-color: white"
                                id="table_documentos_actos_registro_matriculado"
                                data-pagination="true"
                                data-page-list="[5,10,25, 50, 100, all]"
                                data-page-size="5"
                                data-show-export="false"
                                data-show-refresh="false"
                                data-cache="false"
                                data-toggle="table"
                                data-id-field=""
                                data-unique-id=""
                                data-click-to-select="true"
                                data-search="true"
                                data-locale="es-SP">
                                <thead class="GridviewScrollHeader_line_boot_gray">
                                </thead>
                            </table>
                        </div>
                    </div>
                    
                     <div class=" modal-footer">
                        
                    </div>
                </div>
                <div class="tab-pane " id="visor_consulta_publica" role="tabpanel" aria-labelledby="settings-tab">
                    <div class="row m-0 p-0">
                        <div class="col-8 justify-content-start pt-3">
                            <span class="h6">Visor público de documentos DocuArchi SGDEA </span>
                        </div>
                        <div class="col-4">
                            <div class="row  justify-content-end">
                                <div class="col-5 justify-content-end pt-2">
                                    <div class="nav-item active_">
                                        <a class="nav-link active ml-1 h6" id="Button_back_lista_documentos" title="Regresar a la lista" style="color: black" href="#"><i style="color: black" class="fal fa-arrow-left"></i>
                                            Regresar
                                        </a>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>        
                    <div class="modal-body-fullscreen">
                        <div class="row row-body-fullscreen">
                            <div class="col-12  modal-body-fullscreen pr-0">              
                                <div class="conten_gred_border" style=" height:100%">
                                    <iframe id="IframeVisor_" runat="server" loading="lazy" frameborder="0" width="100%" scrolling="no" height="450px"></iframe>
                                </div>
                            </div>      
                        </div>       
                    </div>
                    <div class=" modal-footer">
                        <div class="col-12 pt-2 justify-content-start ">
                            <span style="" id="h_gabibete_imagen"></span>
                        </div>
                    </div>
                </div>
                <div class="tab-pane " id="documentos_matriculado" role="tabpanel" aria-labelledby="settings-tab">
                    <h3 class="rotulo-general_title"> Documentos del matriculado </h3>
                    <div id="error_documentos_matriculado" style="position: relative; width: 100%"></div>   
                    <div id="div_documentos_matriculado" style="height: 100%">
                        <div class="row m-0 p-0">
                            <div class="col-8">
                                <div class="row w-100 ml-0">
                            <div class="col-4">
                                <sopan class="h6" id= "b_1_label" ></sopan>
                            </div>
                            <div class="col-8">
                                <sopan id="b_1_value"></sopan>
                            </div>
                        </div>
                        <div class="row w-100 ml-0">
                            <div class="col-4">
                                <sopan class="h6" id= "b_2_label"></sopan>
                            </div>
                            <div class="col-8">
                                <sopan id="b_2_value"></sopan>
                            </div>
                        </div>
                        <div class="row w-100 ml-0">
                            <div class="col-4">
                                <sopan  class="h6" id= "b_3_label"></sopan>
                            </div>
                            <div class="col-8">
                                <sopan id="b_3_value"></sopan>
                            </div>
                        </div>
                            </div>
                            <div class="col-4">
                                <div class="row  justify-content-end">
                                    <div class="col-5 justify-content-end">
                                        <div class="nav-item active_">
                                            <a class="nav-link active ml-1 h6" id="Button_back_docu_matriculado" title="Regresar a consulta de matriculados" style="color: black" href="#"><i style="color: black" class="fal fa-arrow-left"></i>
                                                Regresar
                                            </a>
                                        </div>
                                    </div>

                                </div>
                            </div>
                        </div>      
                        <div id="contenido_documentos_matriculado" class="p-1" style="height: 370px; width: 100%; position: relative; margin-top: 1px; background-color: white">
                            <table class="table-not-border_person" style="background-color: white"
                                id="table_documentos_matriculado_registro"
                                data-pagination="true"
                                data-page-list="[5,10,25, 50, 100, all]"
                                data-page-size="5"
                                data-show-export="false"
                                data-show-refresh="false"
                                data-cache="false"
                                data-toggle="table"
                                data-id-field=""
                                data-unique-id=""
                                data-click-to-select="true"
                                data-search="true"
                                data-locale="es-SP">
                                <thead class="GridviewScrollHeader_line_boot_gray">
                                </thead>
                            </table>
                        </div>
                    </div>
                    
                     <div class=" modal-footer">
                        
                    </div>
                </div>
                <div class="tab-pane " id="visor_consulta_publica_matricuado" role="tabpanel" aria-labelledby="settings-tab">
                    <div class="row m-0 p-0">
                        <div class="col-8 justify-content-start pt-3">
                            <span class="h6">Visor público de documentos DocuArchi SGDEA </span>
                        </div>
                        <div class="col-4">
                            <div class="row  justify-content-end">
                                <div class="col-5 justify-content-end pt-2">
                                    <div class="nav-item active_">
                                        <a class="nav-link active ml-1 h6" id="Button_back_lista_documentos_matriculados" title="Regresar a la lista" style="color: black" href="#"><i style="color: black" class="fal fa-arrow-left"></i>
                                            Regresar
                                        </a>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>        
                    <div class="modal-body-fullscreen">
                        <div class="row row-body-fullscreen">
                            <div class="col-12  modal-body-fullscreen pr-0">              
                                <div class="conten_gred_border" style=" height:100%">
                                    <iframe id="Iframe_doc_maticulado" runat="server" loading="lazy" frameborder="0" width="100%" scrolling="no" height="450px"></iframe>
                                </div>
                            </div>      
                        </div>       
                    </div>
                    <div class=" modal-footer">
                        <div class="col-12 pt-2 justify-content-start ">
                            <span style="" id="h_gabibete_imagen_matricula"></span>
                        </div>
                    </div>
                </div>
                <div id="error_div_container_general" style="position: relative; width: 100%"></div>
            </div>   
        </div>
        <footer>

        </footer>
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
    </form>
</body>
</html>
