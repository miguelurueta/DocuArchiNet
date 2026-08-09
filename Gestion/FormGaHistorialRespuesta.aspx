<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="FormGaHistorialRespuesta.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.FormGaHistorialRespuesta" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    
    <title> Historial de respuestas</title>
    <script src="../js/ui/jquery-3.4.1.min.js"></script>  
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
    <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>   
    <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <link href="../Styles/styleMenu.css" rel="stylesheet" type="text/css" /> 
    <link href="../Styles/Menu3.css" rel="stylesheet" />
    <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />   
    <script  src="../Awesome/js/all.js"></script>
    <link href="../Awesome/css/fontawesome.css" rel="stylesheet"/>
    <link href="../Awesome/css/brands.css" rel="stylesheet"/>
    <link href="../Awesome/css/solid.css" rel="stylesheet"/>
    <script  src="../Awesome/js/brands.js"></script>
    <script  src="../Awesome/js/solid.js"></script>
    <script  src="../Awesome/js/fontawesome.js"></script>
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <script src="../js/gestion/FormGaHistorialRespuesta.js"></script>
    <script src="../js/java_general/general_code_java.js"></script>
    <script src="../js/validate_campos.js"></script> 
     <script src="../js/java_general/general_code_java.js"></script>
    <script src="../js/java_general/general_config.js"></script>
    <script src="../js/java_general/general_control_java.js"></script>
</head>
<body style="height:100%">
    <form id="FormGaHistorialRespuesta" runat="server">
    
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" EnablePageMethods="true" AsyncPostBackTimeout="900">
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
                
                try {
                    elment_postbak = args.get_postBackElement();
                    var elmen = document.getElementById(elment_postbak.id)
                    if (elmen.type == "button" || elmen.type == "image" || elmen.type == "submit") {
                        value_element = elmen.value;
                        elmen.value = "Espere..."
                        elmen.disabled = true;
                    }
                    posicion_update_pogres('progres_bar');
                }
                catch (err) {
                    alert(err.message + " Funcion InitializeRequest");
                }
            }
            function CheckStatus(sender, args) {
                try {
                    progres_hiden('progres_bar');
                    var elmen = document.getElementById(elment_postbak.id)
                    if (elmen.type == "button" || elmen.type == "image" || elmen.type == "submit") {
                        elmen.disabled = false;
                        elmen.value = value_element;
                    }
                    if (elment_postbak.id == 'Button_consulta_val_radicacion') {
                        document.getElementById("Button_consulta_val_radicacion").disabled = false;
                        document.getElementById("Button_consulta_val_radicacion").value = "Consultar";
                        if (document.getElementById("Hidden_resultado_consulta").value == "YES") {
                            document.getElementById("Hidden_resultado_consulta").value == "";
                            //plugin_grwedview();
                        }

                    }       
                    if (elment_postbak.id == "Button_visor_emergente") {
                        document.getElementById("Label12").innerHTML = "Visor de documentos";
                        auto_zise_popup_visor_externo();
                    }
                    if (elment_postbak.id == "Button_notificar_envio") {
                        auto_zise_popup_notificar();
                    }
                    if (elment_postbak.id == "Button_Trazabilidad") {
                        document.getElementById("Label12").innerHTML = "Trazabilidad de radicados";
                        auto_zise_popup_visor_externo();
                    }
                    if (elment_postbak.id == "Button_Log_respuesta") {
                        document.getElementById("Label12").innerHTML = "Transacciones de respuestas";
                        auto_zise_popup_visor_externo();
                    }
                    if (elment_postbak.id == "Button_detalle_radicado") {
                        document.getElementById("Label12").innerHTML = "Detalle radicado";
                        auto_zise_popup_visor_externo();
                    }
                    if (elment_postbak.id == "Button_log") {
                        document.getElementById("Label12").innerHTML = "Log radicado";
                        auto_zise_popup_visor_externo();
                    }
                    
                    if (elment_postbak.id == "Button_buscar_lista") {
                        busqueda_gred('hdnEmailID_VAL', 'GridView_val_radicacion', 'TextBox_busqueda', 'CheckBox_busqueda');
                    }
                   
                }
                catch (err) {
                    alert(err.message + " Funcion CheckStatus");
                }
            }

            </script>
        <nav id="menucab" class="navbar navbar-expand-sm nav_botota_person_gray modal_content_no_back_inferior">
            <button id="nav_togle_display" class="navbar-toggler" type="button" style="background-color: #6d7fcc" data-toggle="collapse" data-target="#navbarNavDropdown">
                <span class="navbar-toggler-icon_"><i style="color: white" class="fad fa-th-list"></i></span>
            </button>
            <div class="collapse navbar-collapse row" id="navbarNavDropdown">
                <ul class="navbar-nav col-md-7">
                    <li class="nav-item dropdown active ml-2 mr-0 active_">
                        <a class="nav-link  dropdown-toggle " style="color: #6d7fcc" href="#" id="A5" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc; display: none" class="fal fa-bars"></i> Menú
                        </a>
                        <div class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">
                            <a style="color: #6d7fcc" href="#" class="dropdown-item font-weight-light" onclick="activa_descarga_resutados(event,this,'E-R-H-R');"><i class="fal fa-file-export"></i> Exportar los resultados de la lista de radicados </a>
                            <a style="color: #6d7fcc" href="#" class="dropdown-item font-weight-light" onclick="activa_menu_general_diference(event,this,'V-I-H-R')"><i class="fal fa-file-image"></i> Ver imagen soporte</a>
                            <a style="color: #6d7fcc" href="#" class="dropdown-item font-weight-light" onclick="activa_menu_general_diference(event,this,'D-I-H-R')"><i class="fal fa-file-download"></i> Descargar documento de respuesta</a>
                        </div>
                    </li>
                    <li class="nav-item dropdown active ml-2 mr-0 active_">
                        <a class="nav-link  dropdown-toggle " style="color: #6d7fcc" href="#" id="A1" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc; display: none" class="fal fa-ballot"></i> Opciones
                        </a>
                        <div class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">
                            <a style="color: #6d7fcc" href="#" class="dropdown-item font-weight-light" onclick="activa_menu_general_diference(event,this,'N-R-C')"><i class="fal fa-envelope-square"></i> Notificar respuesta al correo </a>
                            <a style="color: #6d7fcc" href="#" class="dropdown-item font-weight-light" onclick="activa_menu_general_diference(event,this,'R-R-D')"><i class="fal fa-undo"></i> Reversar respuesta</a>
                            <a style="color: #6d7fcc" href="#" class="dropdown-item font-weight-light" id="bot_activa_archiva_resp" ><i class="fal fa-archive"></i> Archiva respuesta</a>
                           
                        </div>
                    </li>
                    <li class="nav-item dropdown active ml-2 mr-0 active_">
                        <a class="nav-link  dropdown-toggle " style="color: #6d7fcc" href="#" id="A2" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc; display: none" class="fad fa-th-large"></i> Detalle
                        </a>
                        <div class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">
                            <a style="color: #6d7fcc" href="#" class="dropdown-item font-weight-light" onclick="activa_menu_general_diference(event,this,'D-D-R-R')"><i class="fal fa-table"></i> Detalle respuesta radicado </a>
                            <a style="color: #6d7fcc" href="#" class="dropdown-item font-weight-light" onclick="activa_menu_general_diference(event,this,'D-V-D-T')"><i class="fal fa-list-ol"></i> Transacciones de la respuesta</a>
                            <a style="color: #6d7fcc" href="#" class="dropdown-item font-weight-light" onclick="activa_menu_general_diference(event,this,'D-E-D-R')"><i class="fal fa-project-diagram"></i> Estados del radicado</a>
                        </div>
                    </li>
                </ul>
                <div class=" float-md-right col-md-4 float-sm-left mr-1">
                    <div class="input-group ">
                        <button id="td-boton" class="btn btn-outline-secondary border-right-2 " title="Restaura busqueda por campos" style="border-top-right-radius: 0px; border-bottom-right-radius: 0px; background-color:white" onclick="restore_acti_busq_general_archivo_boton(event,this)" type="button">
                            <i class="fal fa-long-arrow-left"></i>
                        </button>
                        <asp:TextBox ID="TextBox_buequeda_general" runat="server" class="form-control form-control-sm complex  border-left-0" placeholder="Busqueda...."></asp:TextBox>
                        <div class="input-group-append">
                            <button class="btn btn-outline-secondary" onclick="acti_busq_general_archivo_boton(event, this)" type="button" style="background-color:white">
                                <i class="fal fa-search"></i>
                            </button>
                        </div>
                    </div>
                </div> 
            </div>
        </nav>
        <div id="menucab_" class="navbar_gray" style="overflow: auto; display:none">         
            <asp:UpdatePanel ID="UpdatePanel_menu_var_event" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <input id="Hidden_menu_var_event_dive" type="hidden" value="" runat="server" />
                    <asp:Button ID="Button_me_active_men_dive" runat="server" Text="" Style="display: none; width: 1px; height: 1px" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div> 
         <div id="errorgeneralhistorico" style="position: relative; width: 100%"></div>   
        <a id="da_show-sidebar_" class="btn btn-sm   hide_da_sidebar " href="#" data-target="#sidebar_">
            <i style="color: white" class="fas fa-bars"></i>
        </a>
        <div id="da_content_wraper" class="ml-0 mr-2  d-flex " style="padding-left: 1px; padding-right: 1px">
            <div id="Contentizquierdo" style="width: 25%; float: left; border-right: 1px solid #F2F2F2">
                <nav id="sidebar_" class=" bg-light_ pl-0 pr-0 " style="width: 100%">
                    <div id="contenido_titulo_controles_consulta" class="modal-header modal_title_superior bg-light_" style="border-top-left-radius: initial; border-top-right-radius: initial">      
                        <h6 class=" mt-2 mb-2 ml-2 " id="pit_" style="color: #6d7fcc; float: left; font-family: 'Segoe UI'">Campos de busqueda </h6>
                        <a id="sidebarCollapse" class="close_ mr-1" style="float: right;  color: #6d7fcc">&times;</a>
                    </div>
                    <div id="contenido_controles_consulta" style="width: 100%; overflow: auto">
                        <asp:UpdatePanel ID="UpdatePanelContenido_val_radicacion" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Panel ID="_Panelvalidacion_val_radicacion" runat="server" ScrollBars="Vertical" Height="100%" Width="100%" Wrap="false" CssClass="p-2" DefaultButton="Button_consulta_val_radicacion">
                                    <div class="row w-100 mt-2 mr-0 ml-0 p-0">
                                        <div class="col-12 p-0">
                                            <span style="color: #6d7fcc">Nombre del solicitante o peticionario</span>
                                        </div>
                                        <div class="col-12 p-0">
                                            <asp:TextBox ID="TextBoxDESTINATARIO" runat="server" Style="width: 100%" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row w-100 mt-2 mr-0 ml-0 p-0">
                                        <div class="col-12 p-0">
                                            <span style="color: #6d7fcc">Consecutivo radicado</span>
                                        </div>
                                        <div class="col-12 p-0">
                                            <asp:TextBox ID="TextBoxRADICADO" runat="server" Style="width: 100%" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row w-100 mt-2 mr-0 ml-0 p-0">
                                        <div class="col-12 p-0">
                                            <span style="color: #6d7fcc">Nombre del responsable de la respuesta</span>
                                        </div>
                                        <div class="col-12 p-0">
                                            <asp:TextBox ID="TextBoxUSUARIO_RESPONSABLE" runat="server" Style="width: 100%" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row w-100 mt-2 mr-0 ml-0 p-0">
                                        <div class="col-12 p-0">
                                            <span style="color: #6d7fcc">Consecutivo radicado respuesta</span>
                                        </div>
                                        <div class="col-12 p-0">
                                            <asp:TextBox ID="TextBoxRADICADO_RESPUESTA" runat="server" Style="width: 100%" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row w-100 mt-2 mr-0 ml-0 p-0">
                                        <div class="col-12 p-0">
                                            <span style="color: #6d7fcc">Asunto</span>
                                        </div>
                                        <div class="col-12 p-0">
                                            <asp:TextBox ID="TextBoxASUNTO" runat="server" Style="width: 100%" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row w-100 mt-2 mr-0 ml-0 p-0">
                                        <div class="col-12 p-0">
                                            <span style="color: #6d7fcc">Area responsable respuesta</span>
                                        </div>
                                        <div class="col-12 p-0">
                                            <asp:DropDownList ID="DropDownListAREA_RESPONSABLE" runat="server" Style="width: 100%" CssClass="custom-select"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="row w-100 mt-2 mr-0 ml-0 p-0">
                                        <div class="col-12 p-0">
                                            <span style="color: #6d7fcc">Tipo tramite respuesta</span>
                                        </div>
                                        <div class="col-12 p-0">
                                            <asp:DropDownList ID="DropDownListTRAMITE_DOCUMENTO" runat="server" Style="width: 100%" CssClass="custom-select"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="row w-100 mt-2 mr-0 ml-0 p-0">
                                        <div class="col-12 p-0">
                                            <span style="color: #6d7fcc">Tipo respuesta</span>
                                        </div>
                                        <div class="col-12 p-0">
                                            <asp:DropDownList ID="DropDownListtiporespuesta" runat="server" Style="width: 100%" CssClass="custom-select"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="row w-100 mt-2 mr-0 ml-0 p-0">
                                        <div class="col-12 p-0">
                                            <span style="color: #6d7fcc">Estado respuesta</span>
                                        </div>
                                        <div class="col-12 p-0">
                                            <asp:DropDownList ID="DropDownListestadorespuesta" runat="server" Style="width: 100%" CssClass="custom-select"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="row w-100 mt-2 mr-0 ml-0 p-0">
                                        <div class="col-12 p-0">
                                            <span style="color: #6d7fcc">Consecutivo respuesta</span>
                                        </div>
                                        <div class="col-12 p-0">
                                            <div class="row">
                                                <div class="col-6">
                                                    <asp:TextBox ID="TextBoxID_RESPUESTA_RADICADO_INI" runat="server" Style="width: 100%" CssClass="form-control"></asp:TextBox>
                                                </div>
                                                <div class="col-6">
                                                    <asp:TextBox ID="TextBoxID_RESPUESTA_RADICADO_FIN" runat="server" Style="width: 100%" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row w-100 mt-2 mr-0 ml-0 p-0">
                                        <div class="col-12 p-0">
                                            <span style="color: #6d7fcc">Fecha registro solicitud</span>
                                        </div>
                                        <div class="col-12 p-0">
                                            <div class="row p-0">
                                                <div class="col-4 pr-0">
                                                    <asp:TextBox ID="TextBoxFECHA_REGISTRO_INI" runat="server" Width="100%" CssClass="form-control" onkeypress="return validate_fecha(event,this)" placeholder="0000 00 00"></asp:TextBox>
                                                    <asp:CalendarExtender ID="TextBoxFECHA_REGISTRO_INI_CalendarExtender" runat="server" TargetControlID="TextBoxFECHA_REGISTRO_INI" PopupButtonID="ImageButtonfechaextremaini_CUADRO" Format="yyyy-MM-dd" />
                                                </div>
                                                <div class="col-2 p-0">
                                                    <button class="ml-1 btn border-0" id="ImageButtonfechaextremaini_CUADRO" type="button">
                                                        <i class="fad fa-calendar-alt  fa"></i>
                                                    </button>
                                                </div>
                                                <div class="col-4 p-0">
                                                    <asp:TextBox ID="TextBoxFECHA_REGISTRO_FIN" runat="server" Width="100%" CssClass="form-control" onkeypress="return validate_fecha(event,this)" placeholder="0000 00 00"></asp:TextBox>
                                                    <asp:CalendarExtender ID="TextBoxFECHA_REGISTRO_FIN_CalendarExtender" runat="server" TargetControlID="TextBoxFECHA_REGISTRO_FIN" PopupButtonID="ImageButtonfechaextremafin_CUADRO" Format="yyyy-MM-dd" />

                                                </div>
                                                <div class="col-2 p-0">
                                                    <button class="ml-1 btn border-0" id="ImageButtonfechaextremafin_CUADRO" type="button">
                                                        <i class="fad fa-calendar-alt  fa"></i>
                                                    </button>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row w-100 mt-2 mr-0 ml-0 p-0">
                                        <div class="col-12 p-0">
                                            <span style="color: #6d7fcc">Fecha limite respuesta</span>
                                        </div>
                                        <div class="col-12 p-0">
                                            <div class="row p-0">
                                                <div class="col-4 pr-0">
                                                    <asp:TextBox ID="TextBoxFECHA_VENCE_INI" runat="server" Style="width: 100%" CssClass="form-control" onkeypress="return validate_fecha(event,this)" placeholder="0000 00 00"></asp:TextBox>
                                                    <asp:CalendarExtender ID="FECHA_VENCE_INI_CalendarExtender" runat="server" TargetControlID="TextBoxFECHA_VENCE_INI" PopupButtonID="ImageButton_FECHA_VENCE_INI" Format="yyyy-MM-dd" />
                                                </div>
                                                <div class="col-2 p-0">
                                                    <button class="ml-1 btn border-0" id="ImageButton_FECHA_VENCE_INI" type="button">
                                                        <i class="fad fa-calendar-alt  fa"></i>
                                                    </button>
                                                </div>
                                                <div class="col-4 p-0">
                                                    <asp:TextBox ID="TextBoxFECHA_VENCE_FIN" runat="server" Width="100%" CssClass="form-control" onkeypress="return validate_fecha(event,this)" placeholder="0000 00 00"></asp:TextBox>
                                                    <asp:CalendarExtender ID="TextBoxFECHA_VENCE_FIN_CalendarExtender" runat="server" TargetControlID="TextBoxFECHA_VENCE_FIN" PopupButtonID="ImageButton_FECHA_VENCE_FIN" Format="yyyy-MM-dd" />

                                                </div>
                                                <div class="col-2 p-0">
                                                    <button class="ml-1 btn border-0" id="ImageButton_FECHA_VENCE_FIN" type="button">
                                                        <i class="fad fa-calendar-alt  fa"></i>
                                                    </button>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row w-100 mt-2 mr-0 ml-0 p-0">
                                        <div class="col-12 p-0">
                                            <span style="color: #6d7fcc">Fecha respuesta del radicado</span>
                                        </div>
                                        <div class="col-12 p-0">
                                            <div class="row p-0">
                                                <div class="col-4 pr-0">
                                                    <asp:TextBox ID="TextBoxFECHA_RESPUETA_INI" runat="server" Width="100%" CssClass="form-control" onkeypress="return validate_fecha(event,this)" placeholder="0000 00 00"></asp:TextBox>
                                                    <asp:CalendarExtender ID="TextBoxFECHA_RESPUETA_INI_CalendarExtender" runat="server" TargetControlID="TextBoxFECHA_RESPUETA_INI" PopupButtonID="ImageButton_TextBoxFECHA_RESPUETA_INI" Format="yyyy-MM-dd" />
                                                </div>
                                                <div class="col-2 p-0">
                                                    <button class="ml-1 btn border-0" id="ImageButton_TextBoxFECHA_RESPUETA_INI" type="button">
                                                        <i class="fad fa-calendar-alt  fa"></i>
                                                    </button>
                                                </div>
                                                <div class="col-4 p-0">
                                                    <asp:TextBox ID="TextBoxFECHA_RESPUETA_FIN" runat="server" Width="100%" CssClass="form-control" onkeypress="return validate_fecha(event,this)" placeholder="0000 00 00"></asp:TextBox>
                                                    <asp:CalendarExtender ID="TextBoxFECHA_RESPUETA_FIN_CalendarExtender" runat="server" TargetControlID="TextBoxFECHA_RESPUETA_FIN" PopupButtonID="ImageButton_FECHA_RESPUETA_FIN" Format="yyyy-MM-dd" />

                                                </div>
                                                <div class="col-2 p-0">
                                                    <button class="ml-1 btn border-0" id="ImageButton_FECHA_RESPUETA_FIN" type="button">
                                                        <i class="fad fa-calendar-alt  fa"></i>
                                                    </button>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row w-100 mt-2 mr-0 ml-0 p-0">
                                        <div class="col-12 p-0">
                                            <span style="color: #6d7fcc">Fecha envío respuesta al usuario</span>
                                        </div>
                                        <div class="col-12 p-0">
                                            <div class="row p-0">
                                                <div class="col-4 pr-0">
                                                    <asp:TextBox ID="TextBoxFECHA_ENVIO_INI" runat="server" Width="100%" CssClass="form-control" onkeypress="return validate_fecha(event,this)" placeholder="0000 00 00"></asp:TextBox>
                                                    <asp:CalendarExtender ID="FECHA_ENVIO_INI_CalendarExtender" runat="server" TargetControlID="TextBoxFECHA_ENVIO_INI" PopupButtonID="ImageButton_TextBoxFECHA_RESPUETA_INI" Format="yyyy-MM-dd" />
                                                </div>
                                                <div class="col-2 p-0">
                                                    <button class="ml-1 btn border-0" id="Button2" type="button">
                                                        <i class="fad fa-calendar-alt  fa"></i>
                                                    </button>
                                                </div>
                                                <div class="col-4 p-0">
                                                    <asp:TextBox ID="TextBoxFECHA_ENVIO_FIN" runat="server" Width="100%" CssClass="form-control" onkeypress="return validate_fecha(event,this)" placeholder="0000 00 00"></asp:TextBox>
                                                    <asp:CalendarExtender ID="FECHA_ENVIO_FIN_CalendarExtender" runat="server" TargetControlID="TextBoxFECHA_ENVIO_FIN" PopupButtonID="ImageButton_FECHA_ENVIO_FIN" Format="yyyy-MM-dd" />

                                                </div>
                                                <div class="col-2 p-0">
                                                    <button class="ml-1 btn border-0" id="ImageButton_FECHA_ENVIO_FIN" type="button">
                                                        <i class="fad fa-calendar-alt  fa"></i>
                                                    </button>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </asp:Panel>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div id="contenido_controles_buton_consulta" style="border-top-left-radius: initial; border-top-right-radius: initial" class="modal-header    justify-content-start">
                        <asp:UpdatePanel ID="UpdatePanel_botones_validacion" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <input id="Hidden_resultado_consulta" type="hidden" value="" runat="server">
                                <asp:Button ID="Button_consulta_val_radicacion" runat="server" Text="Consultar" ToolTip="Consultar radicados" CssClass="btn  btn-success" EnableTheming="True" />
                                <asp:Button ID="Button_lipiar_val_radicacion" Text="Limpiar" runat="server" ToolTip="Limpiar campos radicacion" CssClass="btn  btn-success" />
                                <asp:Button ID="Button_consulta_like" runat="server" Text="" style="display:none"   />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </nav>
            </div>
            <div id="Contenedorderecho" class=" mr-0 ml-0 pl-1 pr-1 pb-0 pt-0  " style="width: 75%; float: right">
                <div id="contenido_titulo_val_radicacion" class=" p-2">
                    <asp:UpdatePanel ID="UpdatePanelabel_val_radicacion" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="row w-100">
                                <div class="col-6">
                                     <asp:Label ID="titulo_label_val_radicacion" runat="server" style="color: #6d7fcc" CssClass="h6" Text="Resultados busqueda"></asp:Label>
                                     <asp:Label ID="Label_estado_transac" runat="server" Text="" Style="font-size: 8px;color: #6d7fcc" CssClass="h6 font-weight-light"></asp:Label>
                                </div>
                                <div class="col-6">
                                    <div class="row">
                                        <div class="col-8">
                                            <span Style="font-size: 12px;  float: right; color: #6d7fcc" class="font-weight-light ml-2 mr-2"> Listar solo mis respuestas </span>
                                            <asp:CheckBox ID="CheckBoxtodosusuarios" runat="server" Checked="false" Text="" Style=" float: right" />    
                                        </div>
                                         <div class="col-4">
                                             <asp:DropDownList ID="DropDownList_record" runat="server" Style=" float: right" CssClass="custom-select " ></asp:DropDownList>
                                        </div>
                                    </div>
                                               
                                </div>
                            </div>          
                            <input id="hdnEmailID_VAL" type="hidden" value="-1" runat="server">
                            <input id="Hidden_consecutivo_radicado" type="hidden" value="-1" runat="server">
                            <input id="Hidden_max_registro" type="hidden" value="500" runat="server">
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div id="contenido_datagrid_val_radicacion" style="height: 60%; width: 100%; position: relative; margin-top: 1px; overflow: auto">
                    <asp:UpdatePanel ID="UpdatePanel_conenido_grid_val_radicacion" runat="server" UpdateMode="Conditional" RenderMode="Block" style="width: 100%; height: 100%">
                        <ContentTemplate>
                            <asp:GridView ID="GridView_val_radicacion" runat="server" Width="100%" EnableViewState="true" GridLines="None" CssClass="table font-weight-light"
                                AutoGenerateSelectButton="False"  AllowPaging="false" Font-Size="14px" PagerSettings-Position="Top" AllowSorting="false">
                                <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                <FooterStyle BorderStyle="None" />
                                <HeaderStyle CssClass="GridviewScrollHeader_line_boot" BorderStyle="None" />
                                <Columns>
                                    <asp:BoundField HeaderText="OPCIONES"   />
                                </Columns>
                            </asp:GridView>

                        </ContentTemplate>
                        <Triggers>
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
                <div id="Contenido_botones_tipo_radicado" style="height: 10%; width: 100%; float: left; overflow: auto; padding-top: 0.5%; padding-bottom: 0.5%; display: none">

                    <asp:UpdatePanel ID="UpdatePanel_botones_radicacion" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>

                            <asp:Button ID="Button_Exportar_Radicados" Text="Exportar" runat="server" ToolTip="Exportar lista" OnClientClick="retorna_colum_mtriz('Hidden_colum_header');"
                                Style="margin-left: 5px" CssClass="boton_azul" />
                            &nbsp
                             
                        <asp:Button ID="Button_notificar_envio" Text="Notificar" runat="server" Width="55px" ToolTip="Notificar al correo a usuario remitente" Style="background-color: white; border-color: #b0c4de; height: 25px; font-size: 12px; font-family: Arial; display: none" CssClass="boton_blanco" />
                            &nbsp
                        <asp:Button ID="Button_Trazabilidad" runat="server" Text="Estados del radicado" ToolTip="Estados del radicado" Style="" CssClass="boton_azul" />
                            &nbsp 
                         <asp:Button ID="Button_Log_respuesta" runat="server" Text="Transacciones" ToolTip="Transacciones realizadas para la respuesta" Style="" CssClass="boton_azul" />
                            &nbsp 
                        <asp:Button ID="Button_detalle_radicado" Text="Detalle respuesta" runat="server" ToolTip="Muestra los detalles del radicado" Style="" CssClass="boton_azul" />
                            &nbsp 
                         <asp:Button ID="Button_log" Text="Log" runat="server" Width="50px" ToolTip="Muestra del radicado" Style="font-family: arial; font-size: 9px; height: 17px; display: none" />
                            <asp:Button ID="Button_descarga" runat="server" Text="Descargar" Style="" CssClass="boton_azul" ToolTip="Descargar documento de respuesta" />
                            <input id="Hidden_colum_header" type="hidden" value="" runat="server">
                            <input id="Hidden_ruta_archivo" type="hidden" value="" runat="server">
                            <input id="Hidden_estado_plugin" type="hidden" value="" runat="server">
                            &nbsp 
                            <asp:Button ID="Button_visor_emergente" runat="server" Text="Imagen soporte" Style="" CssClass="boton_azul" ToolTip="Ver imagenes relacionadas al radicado" />
                            &nbsp 
                               <asp:Button ID="Button_imprimir" runat="server" Text="Impresión" Style="display: none" CssClass="boton_azul" ToolTip="Imprimir radicado" />
                        </ContentTemplate>

                    </asp:UpdatePanel>

                </div>
            </div>
        </div>
        <!--Asigna destinatario externo-->
        <div id="asigna_dest_externo">
            <asp:Panel ID="Panel_asigna_dest_externo" runat="server" Style="display:none; color: White; width: 600px; height: 250px">

                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_asigna_dest_externo" runat="server" BehaviorID="Panel_asigna_dest_externo_ModalPopupExtender" TargetControlID="ButtonSalir_asigna_dest_externo" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_asigna_dest_externo" PopupControlID="Panel_asigna_dest_externo" ></asp:ModalPopupExtender>
                <div id="div4" class="cabecera2">
                    <asp:Button ID="Button_asigna_dest_externo" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_asigna_dest_externo" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Label ID="Label_asigna_dest_externo" runat="server" Text="Reasigna peticionario con autorización" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton2_asigna_dest_externo" style="float: right">
                        <asp:Button ID="Button_cerrar_asigna_dest_externo" runat="Server" Text="X"
                            ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_asigna_dest_externo" style="background-color: white; width: 100%; height: 99%;border: thin double #000080; color: black; background-color: #FFFFFF;">
                                
                    
                        <asp:UpdatePanel ID="UpdatePanel_dest_externo" runat="server" UpdateMode="Conditional">
                            
                            <ContentTemplate>
                                <input id="Hidden_height" type="hidden" value="-1" runat="server" >
                                <input id="Hidden_width" type="hidden" value="-1" runat="server" >
                               <input id="Hidden_remitente_destinatario" type="hidden" value="-1" runat="server" checked="checked">
                                <input id="Hidden_remitente_nombre" type="hidden" value="-1" runat="server" checked="checked">
                                <asp:Button ID="Button_Asigana_datos_validacion_edicion" runat="server" Text="Button" style="display:none" />
                               <br />
                                <table style="width: 100%;">
                                    <tr>
                                        <td colspan="2">
                                            <asp:TextBox ID="TextBox_dext_externo" runat="server"   style="width:500px; background-color:yellow"  Enabled="false"></asp:TextBox> &nbsp
                                            <asp:Button ID="Button_examinar_dest_externo" runat="server" Text="Examinar" Style="background-color: white; border-color: #b0c4de; height: 30px; width: 70px; height: 25px; text-align: center" CssClass="boton" OnClientClick="asigna_datos_heig_with();" /> 
                                        </td>
                                    </tr>
                                    <tr  >
                                        <td  colspan="2" style="text-align:center">
                                            <asp:Label ID="Label_us" runat="server" Text="Usuario autorizado" style="font-family:Arial; text-align:center; font-size:12px "></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>     
                                        <td>
                                            <asp:Label ID="Label_usario_externo" runat="server" Text="Usuario autorizado*" Style="text-align: center; font-family: Arial; font-size: 14px"></asp:Label>
                                        </td>
                                        <td><asp:TextBox ID="TextBox_login_usuario_val_externo" runat="server" Style="width:300px"></asp:TextBox></td>
                                       
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label_destinatario_externo" runat="server" Text="Contraseña usuario*" Style="text-align: center; font-family: Arial; font-size: 14px"></asp:Label>

                                        </td>
                                        <td>
                                            <asp:TextBox ID="TextBox_pasw_usuario_val_externo" runat="server" Style="width:300px"  TextMode="Password"></asp:TextBox> 
                                           

                                        </td>                           
                                    </tr>
                                    <tr>
                                        <td></td>
                                    </tr>
                                    
                                    <tr>
                                        <td>

                                        </td>
                                        <td style="float:left"><asp:Button ID="Button_actualizar_peticionario" runat="server" Text="Actualizar" Style="background-color: white; border-color: #b0c4de; height: 30px; width: 200px; height: 25px; text-align: center" CssClass="boton" /> &nbsp &nbsp
                                                         
                                        </td>
                                    </tr>
                                    
                                    
                                </table>
                                                         
                            </ContentTemplate>
                        </asp:UpdatePanel>
                         
                </div>
            </asp:Panel>
        </div>
         <asp:Panel ID="Panel_nota_respuesta" runat="server" Style="display: none; overflow: hidden" ForeColor="White" Width="95%" Height="410px">
            <asp:ModalPopupExtender ID="ModalPopupExtender_nota_respuesta" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_nota_respuesta"
                PopupControlID="Panel_nota_respuesta" Y="1" CancelControlID="ButtonSalir_nota_respuesta"></asp:ModalPopupExtender>
            <div id="Cabecerapendiente_nota_respuesta" class="cabecera2">
                <asp:Button ID="Button_nota_respuesta" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                <asp:Label ID="Label_not" runat="server" Text="Anotacion a la respuesta" Font-Size="10"></asp:Label>
                <div id="Div_nota_respuesta" style="float: right">
                    <asp:Button ID="ButtonSalir_nota_respuesta" runat="Server" Text="X"
                        ForeColor="#000066" Height="21px" />
                </div>
            </div>
            <div id="Cotenedorpendiente_nota_respuesta" style="border: thin double #000080; color: Black; background-color: #FFFFFF; height: 400px; width: 100%; overflow: hidden">
                <div id="contenido_titulo_detalle_respuesta" style="height: 20px; width: 100%; background-color: #E7EDF5; text-align: center">
                    <asp:Label ID="Label_nota_respuesta" runat="server" ForeColor="White" Font-Size="12" Font-Names="Arial">NOTA RESPUESTA</asp:Label>
                </div>
                <div id="div_gabinetes" style="float: left; background-color: white; width: 100%; height: 330px; margin-top: 1px">
                    <asp:UpdatePanel ID="UpdatePanel_nota_respuesta" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:TextBox ID="TextBox_NOTA_RESPUESTA" runat="server" Style="font-family: arial; font-size: 12px; width: 99%; margin-right: 7px; height: 295px" TextMode="MultiLine"></asp:TextBox>
                        </ContentTemplate>

                    </asp:UpdatePanel>
                </div>
                <div id="Div2" style="float: left; background-color: white; width: 100%; height: 35px; margin-top: 1px; margin-bottom: 1px; text-align: center">
                    <asp:UpdatePanel ID="UpdatePanel_botones_registro" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>

                            <asp:Panel ID="Panel_botones" runat="server" Style="height: 34px">
                                <input id="Hidden_estado_nota" type="hidden" value="" runat="server">
                                <asp:Button ID="Button_guardar" Style="background-color: white; border-color: #b0c4de; height: 30px; width: 200px; height: 25px; text-align: center;margin-top:3px" runat="server" Text="Actualizar nota" CssClass="boton" />

                            </asp:Panel>
                            <asp:RoundedCornersExtender ID="Panel_botones_RoundedCornersExtender" runat="server" Enabled="True" BorderColor="Black" Color="Black" TargetControlID="Panel_botones" />
                        </ContentTemplate>
                    </asp:UpdatePanel>

                </div>


            </div>

        </asp:Panel>
        
         <!--Notifca respuesta al correo electronico-->
        <div id="notifica_correo_respuesta">
            <asp:Panel ID="Panel_notifica_correo_respuesta" runat="server" Style="display: none; width: auto; height: auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_notifica_correo_respuesta" runat="server" BehaviorID="Panel_notifica_correo_respuesta_ModalPopupExtender" TargetControlID="ButtonSalir_notifica_correo_respuesta" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_notifica_correo_respuesta" PopupControlID="Panel_notifica_correo_respuesta">
                </asp:ModalPopupExtender>
                <div id="cabecera_notifica_correo" class="modal_title_superior_ modal-header">
                    <h6 class="modal-title d-inline ml-1">Notifica respuesta al correo electrónico</h6>
                    <button type="button" value="Button_cerrar_notifica_correo_respuesta" class="close da_event_captive">&times;</button>
                </div>
                <div id="contenido_procesa_notifica_correo_respuesta" style="width: 100%; height: 99%; border-top:none; overflow:auto" class="modal_content_general modal-body">
                    <asp:UpdatePanel ID="UpdatePanel_notifica_correo" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="row w-100 mt-2">
                                <div class="col-4">
                                </div>
                                <div class="col-8">
                                    <span class="font-weight-light">Por favor separe por comas(,) los correos electronicos, ejemplo pepito@gmail.com,juan@hotmail.com</span>
                                </div>
                            </div>
                            <div class="row w-100 mt-2">
                                <div class="col-4">
                                    <span>Correos electronicos*</span>
                                </div>
                                <div class="col-8">
                                    <asp:TextBox ID="TextBox_correo_electronico" runat="server" Style="width: 100%" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row w-100 mt-2">
                                <div class="col-4">
                                </div>
                                <div class="col-8">
                                    <asp:CheckBox ID="CheckBox_anexa_anexos" runat="server" Checked="true" />
                                    <span class="ml-2">Adjunta al correo los anexos de la respuesta</span>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div class="modal-footer justify-content-end" id="modal-footer_notifica_correo_respuesta">
                    <asp:UpdatePanel ID="UpdatePanel_boton_notifica_correo" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Button ID="Button_notificar_correo" runat="server" Text="Notificar" CssClass="btn btn-success" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div style="display: none; height: 1px">
                    <asp:Button ID="ButtonSalir_notifica_correo_respuesta" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="Button_notifica_correo_respuesta" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="Button_cerrar_notifica_correo_respuesta" runat="Server" Text="" CssClass="invisible" Height="0px" Width="0px" />
                </div>

            </asp:Panel>
        </div>
        <!--reversa respuesta-->
         <div id="reversa_respuesta">
            <asp:Panel ID="Panel_reversa_respuesta" runat="server" Style="display:none; width: auto; height: auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_reversa_respuesta" runat="server"  TargetControlID="ButtonSalir_reversa_respuesta" 
                     BackgroundCssClass="FondoAplicacion" CancelControlID="Button_cerrar_reversa_respuesta" PopupControlID="Panel_reversa_respuesta" ></asp:ModalPopupExtender>
                <div id="modal_content_Panel_reversa_respuesta" class="modal-content">
                    <div id="divcabecer2_radica_documento" class="modal_title_superior_ modal-header" >
                          <h6 class="modal-title d-inline ml-1">Reversa respuesta</h6>
                          <button type="button" value="Button_cerrar_reversa_respuesta" class="close da_event_captive">&times;</button> 
                          
                    </div>
                    <div id="contenido_procesa_reversa_respuesta" style="width: 100%; height: 100% ; border-top:none; overflow:auto" class="modal_content_general modal-body">
                        <asp:UpdatePanel ID="UpdatePanel_contenido_radica_documento" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row  mt-2">
                                    <div class="col-5">
                                        <span class="font-weight-light">Usuario*</span>
                                    </div>
                                    <div class="col-7">
                                        <asp:TextBox ID="TextBox_login_usuario_val" runat="server" Style="width: 100%" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row  mt-2">
                                    <div class="col-5">
                                        <span class="font-weight-light">Contraseña*</span>
                                    </div>
                                    <div class="col-7">
                                        <asp:TextBox ID="TextBox_pasw_usuario_val" runat="server" Style="width: 100%" TextMode="Password" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                
                            </ContentTemplate>
                        </asp:UpdatePanel>
                       
                    </div>
                    <div class="modal-footer justify-content-end" id="modal-footer_Panel_reversa_respuesta">  
                         <asp:UpdatePanel ID="UpdatePanel_boton_Panel_reversa_respuesta" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                 <asp:Button ID="Button_reversar" runat="server" Text="Reversar" Style="" CssClass="btn btn-success" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
                <div style="display: none; height: 1px">
                    <asp:Button ID="Button_cerrar_reversa_respuesta" runat="Server" Text="" Height="0px" Width="0px" CssClass="invisible"/>
                    <asp:Button ID="Button_reversa_respuesta" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="ButtonSalir_reversa_respuesta" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                </div> 
            </asp:Panel>
        </div>
        <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 200px">
            <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
            Processing ...
        </div>
        <iframe id="txtArea1" style="display:none"></iframe>
        <input id="Hidden_id_tarea_sel" type="hidden" value="-1" runat="server"/>
           <input id="Hidden_tipo_visor" type="hidden" value="" runat="server"/>
               <!--Popup visor externo-->
               <asp:Panel ID="Panel_visor_externo" runat="server" Style="display:none; overflow:hidden" ForeColor="White" Width="95%" Height="100% " >
                  <asp:ModalPopupExtender ID="ModalPopupExtender_visor_externo" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_visor_externo"
                      PopupControlID="Panel_visor_externo" Y="1" CancelControlID="ButtonSalir_visor_externo">
                  </asp:ModalPopupExtender>
                  <div id="Cabecerapendiente_visor_externo" class="cabecera2">
                      <asp:Button ID="Button_visor_externo" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                      <asp:Label ID="Label12" runat="server" Text="Visor documentos Radicado" Font-Size="10"></asp:Label>
                      <div id="Div_visor_externo" style="float: right">
                          <asp:Button ID="ButtonSalir_visor_externo" runat="Server" Text="X"
                              ForeColor="#000066" Height="21px" />
                      </div>
                  </div>
                  <div id="Cotenedorpendiente_visor_externo" style="border: thin double #000080; color: Black; background-color: #FFFFFF; height: 90%; width: 100%; overflow:hidden">   
                      <asp:UpdatePanel ID="UpdatePanel_visor_externo" runat="server" UpdateMode="Conditional">
                          <ContentTemplate>
                             
                              <iframe id="Iframe_visor_externo_" runat="server" frameborder="0"  style="width:100%; height:100%; overflow:hidden"></iframe>
                          </ContentTemplate>
                      </asp:UpdatePanel>                    
                  </div>      
              </asp:Panel>
        <!--detalle trazabilidad-->
           <asp:Panel ID="Panel_trazabilidad" runat="server" Style="display:none; overflow:hidden; width:80%; height:100%"  CssClass="modal_content_general" >
                  <asp:ModalPopupExtender ID="ModalPopupExtender_trazabilidad" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_trazabilidad_dos"
                      PopupControlID="Panel_trazabilidad"  CancelControlID="ButtonSalir_trazabilidad">
                  </asp:ModalPopupExtender>
               <div id="modal_content_Panel_trazabilidad" class="modal-content">  
                  <div id="Cabecerapendiente_trazabilidad" class="modal_title_superior_ modal-header"> 
                      <h6 class="modal-title d-inline ml-1">Trazabilidad radicado</h6>
                      <button type="button" value="ButtonSalir_trazabilidad" class="close da_event_captive">&times;</button>
                  </div>
                  <div id="Cotenedorpendiente_trazabilidad" style="height: 90%; width: 100%" class="modal_content_back">          
                      <asp:UpdatePanel ID="UpdatePanel_trazabilidad" runat="server" UpdateMode="Conditional">
                          <ContentTemplate>
                              <iframe id="Iframe_trazabilidad_" runat="server" frameborder="0"  style="width:100%; height:100%; overflow:hidden"></iframe>
                          </ContentTemplate>
                      </asp:UpdatePanel>                
                  </div>
               </div>
               <div style="display:none; height:1px">
                    <asp:Button ID="Button_trazabilidad_dos" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                   <asp:Button ID="ButtonSalir_trazabilidad" runat="Server" Text="" CssClass="invisible" Height="0px" Width="0px"/>
               </div>      
              </asp:Panel>
        <!--detalle transacciones-->
           <asp:Panel ID="Panel_transacciones" runat="server" Style="display:none; overflow:hidden; width:90%; height:100%" CssClass="modal_content_general" >
                  <asp:ModalPopupExtender ID="ModalPopupExtender_transacciones" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_transacciones_dos"
                      PopupControlID="Panel_transacciones"  CancelControlID="ButtonSalir_transacciones">
                  </asp:ModalPopupExtender>
               <div id="modal_content_Panel_transacciones" class="modal-content">  
                  <div id="Cabecerapendiente_transacciones" class="modal_title_superior_ modal-header" >   
                       <h6 class="modal-title d-inline ml-1">Detalle de transacciones</h6>
                       <button type="button" value="ButtonSalir_transacciones" class="close da_event_captive">&times;</button>
                  </div>
                  <div id="Cotenedorpendiente_transacciones" style="height: 90%; width: 100%; overflow:hidden" class="modal_content_back">     
                      <asp:UpdatePanel ID="UpdatePanel_transacciones" runat="server" UpdateMode="Conditional">
                          <ContentTemplate>
                              <iframe id="Iframe_transacciones_" runat="server" frameborder="0"  style="width:100%; height:100%; overflow:hidden"></iframe>
                          </ContentTemplate>
                      </asp:UpdatePanel>           
                  </div>
              </div>
               <div style="display:none; height:1px">
                   <asp:Button ID="Button_transacciones_dos" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                   <asp:Button ID="ButtonSalir_transacciones" runat="Server" Text="X"  CssClass="invisible" Height="0px" Width="0px"/>
               </div>
                   
              </asp:Panel>
        <!--detalle_respuesta-->
        <asp:Panel ID="Panel_detalle_respuesta" runat="server" Style="display: none; overflow: hidden; width: 90%; height: 100%" CssClass="modal_content_general">
            <asp:ModalPopupExtender ID="ModalPopupExtender_detalle_respuesta" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_detalle_respuesta_dos"
                PopupControlID="Panel_detalle_respuesta" CancelControlID="ButtonSalir_detalle_respuesta">
            </asp:ModalPopupExtender>
            <div id="modal_content_Panel_detalle_respuesta" class="modal-content">
                <div id="Cabecerapendiente_detalle_respuesta" class="modal_title_superior_ modal-header">
                    <h6 class="modal-title d-inline ml-1">Detalle respuesta radicado</h6>
                    <button type="button" value="ButtonSalir_detalle_respuesta" class="close da_event_captive">&times;</button>
                </div>
                <div id="Cotenedorpendiente_detalle_respuesta" style="height: 90%; width: 100%; overflow: hidden">
                    <asp:UpdatePanel ID="UpdatePanel_detalle_respuesta" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <iframe id="Iframe_detalle_respuesta_" runat="server" frameborder="0" style="width: 100%; height: 100%; overflow: hidden"></iframe>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
            <div style="display: none; height: 1px">
                <asp:Button ID="Button_detalle_respuesta_dos" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                <asp:Button ID="ButtonSalir_detalle_respuesta" runat="Server" Text="" CssClass="invisible" Height="0px" Width="0px" />
            </div>
        </asp:Panel>
              <!--imagen_respuesta-->
        <asp:Panel ID="Panel_imagen_respuesta" runat="server" Style="display: none; overflow: hidden; width: 99%;"  Height="100% " CssClass="modal_content_general">
            <asp:ModalPopupExtender ID="ModalPopupExtender_imagen_respuesta" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_imagen_respuesta_dos"
                PopupControlID="Panel_imagen_respuesta" CancelControlID="ButtonSalir_imagen_respuesta">
            </asp:ModalPopupExtender>
            <div id="modal_content_Panel_imagen_respuesta" class="modal-content">
                <div id="Cabecerapendiente_imagen_respuesta" class="modal_title_superior_ modal-header">
                    <h6 class="modal-title d-inline ml-1">Visor de documentos</h6>
                    <button type="button" value="ButtonSalir_imagen_respuesta" class="close da_event_captive">&times;</button>
                </div>
                <div id="Cotenedorpendiente_imagen_respuesta" style="height: 100%; width: 100%; overflow: hidden" class="modal_content_back">
                    <asp:UpdatePanel ID="UpdatePanel_imagen_respuesta" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <iframe id="Iframe_imagen_respuesta_" runat="server" frameborder="0" style="width: 100%; height: 100%; overflow: hidden"></iframe>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
            <div style="display: none; height: 1px">
                <asp:Button ID="Button_imagen_respuesta_dos" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                <asp:Button ID="ButtonSalir_imagen_respuesta" runat="Server" Text="X" CssClass="modal_boton_hiden" />
            </div>
        </asp:Panel>
        <!--Popup actualización Archiva_tramite -->
        <div class="modal fade modal_opacity" id="modal_content_Archiva_tramite"  data-keyboard="false" tabindex="-1" style="z-index:100066" aria-labelledby="staticBackdropLabel"  aria-hidden="false" data-backdrop="false">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h4 id="label_title_Archiva_tramite" style="color: black" class="modal-title">Archiva respuesta</h4>
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>
                    <div class="modal-body" > 
                        <div id="div_Archiva_tramite" class="pb-1">  
                            <div class="row pt-1 pb-1">
                                <div class="col-12">
                                    <select id="OptionArchiva" class="form-control">
                                        <option value="1">Seleccione el motivo </option>
                                        <option value="2">Respuesta enviada y recibida por el destinatario; se archiva para cierre del trámite</option>
                                        <option value="3">No se recibió respuesta en el plazo establecido; se archiva por vencimiento de términos</option>
                                        <option value="4">Trámite resuelto por otro medio; se archiva para constancia</option>
                                        <option value="5">Se archiva por desistimiento del solicitante, según comunicación recibida</option>
                                    </select>
                                </div>
                            </div>
                            <div class="row  pb-1 pt-1">
                                <div class="col-12">
                                      <textarea id="NotaArchivo" rows="5" cols="30" placeholder="Nota relacionada al motivo" class="form-control" maxlength="200"></textarea>     
                                </div>  
                            </div>
                          
                        </div>
                    </div>
                    <div id="error_content_Archiva_tramite" style="position: relative; width: 100%"></div>
                    <div class="modal-footer align-content-end" id="modal_foter_actualizacion_Archiva_tramite">
                        <button type="button" id="Btn_Archiva_tramite" title="Archiva respuesta"  class="btn  btn-primary  mt-1">Aceptar</button>
                    </div>
                </div>
            </div>
        </div>   
        <!--Termina Popup archiva tramite -->
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
        <div id="inferior_bajo_boton" style="width: 100%; height: 20%; background-color: #E7EDF5; display: none">
            <asp:UpdatePanel ID="updatapanel_iframe" runat="server" UpdateMode="Conditional">
                <ContentTemplate>

                    <asp:Label ID="Label17" runat="server" Text="Estado" Style="font-size: 8px; font-family: Arial; float: right"></asp:Label>
                    <iframe runat="server" id="ifmExcel_" width="0" height="0" marginheight="0" marginwidth="0"
                        frameborder="0" />
                    <input id="Hidden_ruta_archivo_descarga" type="hidden" value="" runat="server"/>
                </ContentTemplate>

            </asp:UpdatePanel>
        </div>
        <div id="ventanaimpreion">     
            <asp:Panel ID="Panelimpresion" runat="server"  Style="display:none; color: White; width: auto; height: auto">
                 <asp:DragPanelExtender ID="DragPanelExtenderimpre" runat="server" TargetControlID="Panelimpresion" />
                 <asp:ModalPopupExtender ID="ModalPopupExtenderimpre" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir"
                     PopupControlID="Panelimpresion" CancelControlID="Buttoncerrarimpre">
                 </asp:ModalPopupExtender>
                 <div id="divcabecer2" class="cabecera2">
                     <asp:Button ID="Button1" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                     <asp:Button ID="ButtonSalir" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                     <asp:Label ID="Label6" runat="server" Text="Menu Impresion" Font-Size="10" Style="float: left">
                     </asp:Label>
                     <div id="Divcerrarbuton2" style="float: right">
                         <asp:Button ID="Buttoncerrarimpre" runat="Server" Text="X"
                             ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />

                     </div>
                   </div>
               
                <asp:UpdatePanel ID="UpdatePaneliframe" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div id="ContenidoImpresion" style="border: thin double #000080; color: black; background-color: #FFFFFF; height: 280px; width: 500px">
                            <iframe width="100%" height="100%" id="ifimpre" runat="server" src="../Gestion/WebFormimpresionfile.aspx" ></iframe>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                </asp:Panel>
        </div>
   
    </form>
     <script type="text/javascript">
         $(document).ready(function () {
             $('#sidebarCollapse').on('click', function () {
                 $('#sidebar_').toggleClass('active_da_slider');
                 $('#Contenedorderecho').toggleClass('active_content_rigth');
                 $('#Contentizquierdo').toggleClass('active_content_left');
                 $(this).toggleClass('active_da_slider');
                 $('#da_show-sidebar_').toggleClass('show_da_slide');
                 $('#da_show-sidebar_').toggleClass('hide_da_sidebar');

             });
             $('#da_show-sidebar_').on('click', function () {
                 $('#sidebar_').toggleClass('active_da_slider');
                 $('#Contenedorderecho').toggleClass('active_content_rigth');
                 $('#Contentizquierdo').toggleClass('active_content_left');
                 $(this).toggleClass('show_da_slide');
                 $(this).toggleClass('hide_da_sidebar');
                 auto_zise_popup_validacion_radicados();
             });
         });
    </script>
</body>
</html>
