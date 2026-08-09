<%@ Page Language="vb" AutoEventWireup="false" Title="DocuArchi Gestor" CodeBehind="WebFormInicioDocuarchiGestion.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormInicioDocuarchiGestion" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <link href="https://fonts.googleapis.com/css?family=Open+Sans:300,400,400i,600,700,800" rel="stylesheet"/>
    <link href="../menu_scoope/assets/css/font-awesome.min.css" rel="stylesheet" />	
    <link href="../menu_scoope/assets/css/linearicons.css" rel="stylesheet"/>
	<link href="../menu_scoope/assets/css/simple-line-icons.css" rel="stylesheet"/>
	<link href="../menu_scoope/assets/css/ionicons.css" rel="stylesheet"/>
	<link href="../menu_scoope/assets/css/flag-icon.min.css" rel="stylesheet"/>
	<link href="../menu_scoope/assets/css/fakeLoader.css" rel="stylesheet"/>	
    <link href="../menu_scoope/assets/css/scoop-vertical.css" rel="stylesheet"/>
    <script src="../js/ui/jquery-3.4.1.min.js"></script>  
  <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
  <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
    <link href="../menu_scoope/assets/css/jquery.mCustomScrollbar.css" rel="stylesheet"/>
	<script src="../menu_scoope/assets/js/jquery.1.11.3.min.js"></script>
	<script src="../menu_scoope/assets/js/lib/fakeLoader.js"></script>
     <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js" type="text/javascript"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <script src="../menu_scoope/assets/js/scoop.min.js"></script>
	<script src="../menu_scoope/assets/js/lib/sparkline.min.js"></script>
	<script src="../menu_scoope/assets/js/lib/jquery.mCustomScrollbar.concat.min.js"></script> 
	<script src="../menu_scoope/assets/js/lib/jquery.mousewheel.min.js"></script>    
    <script src="../js/inicio/WebFormInicioDocuarchiGestion.js"></script>  
    <script src="../js/sesion/js_sesion_gestor.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />  
    <link href="../Awesome/css/fontawesome.css" rel="stylesheet" />
    <link href="../Awesome/css/brands.css" rel="stylesheet" />
    <link href="../Awesome/css/solid.css" rel="stylesheet" />
    <script  src="../Awesome/js/all.js" type="text/javascript"></script>
    <script  src="../Awesome/js/brands.js" type="text/javascript"></script>
    <script  src="../Awesome/js/solid.js" type="text/javascript"></script>
    <script src="../js/java_general/general_code_java.js"></script>

<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>

    <title>DocuArchi SGEDA </title>
</head>
<body onload="display_unload()" class="overflow_none_scoop" >
    <form id="form1" runat="server" style="background-color:lightgray">
         <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" EnablePageMethods="true" AsyncPostBackTimeout="1500">
        </asp:ScriptManager>
        <script type="text/javascript" >
            Sys.WebForms.PageRequestManager.getInstance().add_initializeRequest(InitializeRequest);
            Sys.Application.add_load(ApplicationLoadHandler)
            var elment_postbak;
            function ApplicationLoadHandler(sender, args) {
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(CheckStatus);
            }
            function InitializeRequest(sender, args) {
                elment_postbak = args.get_postBackElement();
                if (elment_postbak.id = "Button_notificar") {

                    document.getElementById("Button_notificar").value = "Cargado..";
                    document.getElementById("Button_notificar").disabled = true;


                }
                posicion_update_pogres('progres_bar');
            }
            function CheckStatus(sender, args) {
                try {
                    if (elment_postbak.id = "Button_notificar") {
                        document.getElementById("Button_notificar").value = "Enviar correo";
                        document.getElementById("Button_notificar").disabled = false;
                        if (document.getElementById("Hidden00001").value == "YES") {
                            document.getElementById("Hidden00001").value = "";
                            hiden_cierra_ventana_principal();
                        }
                    }
                } catch (e) {
                    alert("Funcion esstatus WebFormNotificar " + e.message);
                } finally { progres_hiden('progres_bar'); }

            }

        </script>
    	<div id="fakeLoader"></div>
        <input id="Hidden_selecion_url" type="hidden" value="" runat="server"/>
        <input id="Hidden_url_service" type="hidden" value="" runat="server" />
        <input id="Hidden_tipo_contenido_content" type="hidden" value="" runat="server" />
        <div id="scoop" class="scoop overflow_none_scoop" >
            <div class="scoop-overlay-box"></div>
            <div class="scoop-container">
                <header class="scoop-header" id="header_coop" style="opacity:0">
                    <div class="scoop-wrapper">
                        <div class="scoop-left-header d-none" id="hader_logo" >
                           <div class="scoop-logo">
                                <span class="logo-icon"><i class="ion-stats-bars"></i></span>
                                <span class="logo-text">DocuArchi SGDEA<span class="hide-in-smallsize light" ></span></span>
                            </div>   
                        </div>
                        <div class="scoop-right-header">
                            <div class="sidebar_toggle"><a href="javascript:void(0)"><i class="icon-menu"></i></a></div>
                            <div class="scoop-rl-header " id="id_scoop_item" >
                                <ul>
                                    <li class="icons hide-small-device"  >
                                       <span id="id_selecion_opcion_trea" > </span>
                                    </li>
                                    <li class="icons d-none" id="CR-RP-03_" onclick="event_menu_prinicipal(this, event);" title="Respuestas pendientes por aprobar" >
                                        <a href="javascript:void(0)"><i class="icon-envelope-open" aria-hidden="true"></i>
                                            <span id="id_resp_aprobar" style="display:none" class="scoop-badge badge-success">0</span>
                                        </a>
                                    </li>
                                    <li class="icons d-none" id="CR-GT-01_" onclick="event_menu_prinicipal(this, event);" title="Gestión de correspondencia">
                                        <a href="javascript:void(0)"><i class="icon-envelope-open" aria-hidden="true"></i>
                                            <span id="id_task_asignado" style="display:none" class="scoop-badge badge-warning" >0</span>
                                        </a>
                                    </li>
                                    <li class="icons d-none" id="GD-DC-19_" onclick="event_menu_prinicipal(this, event);" title="Documentos pendientes por revisar">
                                        <a href="javascript:void(0)"><i class="icon-folder-alt" aria-hidden="true" ></i>
                                            <span id="id_docu_aprobacion" style="display:none" class="scoop-badge badge-warning">0</span>
                                        </a>
                                    </li>
                                    <li class="icons d-none" id="WF-CL-01_" onclick="event_menu_prinicipal(this, event);" title="Flujos y tareas">
                                        <a href="javascript:void(0)"><i class="icon-organization" aria-hidden="true" ></i>
                                            <span id="id_task_workflow" style="display:none" class="scoop-badge badge-warning">0</span>
                                        </a>
                                    </li>
                                </ul>
                            </div>
                            <div class="scoop-rr-header" id="id_scoop_item_rr" style="height:40px">                 
                                <ul>
                                    <li class="icons" title="" >
                                       <span id="id_user_loguin" class="h6 font-weight-light"> </span>
                                    </li>
                                    <li class="icons" id="CI-AP-001_00050" title="Inicio" onclick="even_diplay_ini();">
                                        <a href="javascript:void(0)">
                                            <i  class="fal fa-arrow-left fa-2x" aria-hidden="true"></i>
                                        </a>
                                    </li>     
                                    <li class="icons" title="Descargar componente de impresión versión 3" >
                                         <a href="http://www.neodynamic.com/downloads/wcpp/" target="_blank">
                                            <i  class="fal fa-cloud-download fa-2x" aria-hidden="true"></i>
                                        </a>
                                    </li>
                                    <li class="icons" id="boton_sesion_end_active"  title="Cerrar sesión">
                                        <a href="javascript:void(0)"  >
                                            <i style="color:darkred" class="fal fa-power-off fa-2x" aria-hidden="true" title="Cerrar sesión"></i>
                                        </a>
                                    </li>
                                </ul>
                            </div>
                        </div>
                    </div>
                </header>
                <div class="scoop-main-container" id="main_container" style="margin-top:10px;opacity:0">
                    <div class="scoop-wrapper">
                        <nav class="scoop-navbar" id="nav_scooper">
                            <div class="sidebar_toggle"><a href="#"><i class="icon-close icons"></i></a></div>
                            <div class="scoop-inner-navbar" id="nav_menu">
                                <div class="scoop-search" style="display:none">
                                    <span class="searchbar-toggle"></span>
                                    <div class="scoop-search-box ">
                                        <input type="text" placeholder="Search"/>
                                        <span class="search-icon"><i class="fa fa-search" aria-hidden="true"></i></span>
                                    </div>
                                </div>

                                <ul class="scoop-item scoop-left-item">
                                    <!--Menu workflow-->
                                    <li class="scoop-hasmenu" id="WF-WF-01">
                                        <a href="javascript:void(0)"  title="Workflow (Gestión de flujos y tareas)">
                                            <span class="scoop-micon"><i class="icon-organization"></i></span>
                                            <span class="scoop-mtext">Workflow</span>
                                            <span id="id_task_workflow_" class="scoop-badge badge-warning">0</span>
                                            <span class="scoop-mcaret"></span>
                                        </a>
                                        <ul class="scoop-submenu">
                                            <li class=" " id="WF-CL-01"  onclick="event_menu_prinicipal(this, event);">
                                                <a href="javascript:void(0)" >
                                                    <span class="scoop-micon"><i class="icon-link"></i></span>
                                                    <span id="id_task_workflow__" class="scoop-badge badge-warning">0</span>
                                                    <span class="scoop-mtext">Flujos y tareas</span>
                                                    <span class="scoop-mcaret"></span>
                                                </a>
                                            </li>
                                            <li class="  " id="WF-TR-02" onclick="event_menu_prinicipal(this, event);">
                                                <a href="javascript:void(0)">
                                                    <span class="scoop-micon"><i class="icon-link"></i></span>
                                                    <span class="scoop-mtext">Consulta de flujos y tareas</span>

                                                </a>
                                            </li>
                                            <li class="  " id="WF-RW-03" onclick="event_menu_prinicipal(this, event);">
                                                <a href="javascript:void(0)">
                                                    <span class="scoop-micon"><i class="icon-link"></i></span>
                                                    <span class="scoop-mtext">Rerportes de flujos y tareas</span>

                                                </a>
                                            </li>
                                            <li class=" " id="WF-GF-04" onclick="event_menu_prinicipal(this, event);">
                                                <a href="javascript:void(0)">
                                                    <span class="scoop-micon"><i class="icon-link"></i></span>
                                                    <span class="scoop-mtext">Gestión de flujos y tareas</span>
                                                    <span class="scoop-mcaret"></span>
                                                </a>
                                            </li>
                                             <li class=" " id="WF-GF-05" onclick="event_menu_prinicipal(this, event);">
                                                <a href="javascript:void(0)">
                                                    <span class="scoop-micon"><i class="icon-link"></i></span>
                                                    <span class="scoop-mtext">Migración SII</span>
                                                    <span class="scoop-mcaret"></span>
                                                </a>
                                            </li>
                                            <li class=" " id="WF-CD-05" onclick="event_menu_prinicipal(this, event);">
                                                <a href="javascript:void(0)">
                                                    <span class="scoop-micon"><i class="icon-link"></i></span>
                                                    <span class="scoop-mtext">Consulta de documentos</span>
                                                    <span class="scoop-mcaret"></span>
                                                </a>
                                            </li>
                                            <li class=" " id="WF-DR-06" onclick="event_menu_prinicipal(this, event);">
                                                <a href="javascript:void(0)">
                                                    <span class="scoop-micon"><i class="icon-link"></i></span>
                                                    <span class="scoop-mtext">Administración de rutas</span>
                                                    <span class="scoop-mcaret"></span>
                                                </a>
                                            </li>
                                            <li class=" " id="WF-DF-07" onclick="event_menu_prinicipal(this, event);">
                                                <a href="javascript:void(0)">
                                                    <span class="scoop-micon"><i class="icon-link"></i></span>
                                                    <span class="scoop-mtext">Administración de flujos</span>
                                                    <span class="scoop-mcaret"></span>
                                                </a>
                                            </li>
                                            
                                            <li class=" " id="WF-PC-09" onclick="event_menu_prinicipal(this, event);">
                                                <a href="javascript:void(0)">
                                                    <span class="scoop-micon"><i class="icon-link"></i></span>
                                                    <span class="scoop-mtext">Autenticación</span>

                                                </a>
                                            </li>


                                        </ul>
                                    </li>
                                    <!--Menu docuarchi-->
                                    <li class="scoop-hasmenu" id="DA-PR-00">
                                        <a href="javascript:void(0)" title="Docuarchi (Consulta y almacenamiento de documentos)">
                                            <span class="scoop-micon"><i class="icon-notebook"></i></span>
                                            <span class="scoop-mtext">DocuArchi Contenedor</span>
                                            <span class="scoop-mcaret"></span>
                                        </a>
                                        <ul class="scoop-submenu">
                                            <li class=" " id="DA-CLI-01" onclick="event_menu_prinicipal(this, event);">
                                                <a href="javascript:void(0)">
                                                    <span class="scoop-micon"><i class="icon-mustache"></i></span>
                                                    <span class="scoop-mtext">Contenedor de documentos</span>
                                                    <span class="scoop-mcaret"></span>
                                                </a>
                                            </li>

                                            <li class=" " id="DA-CLI-02">
                                                <a href="javascript:void(0)" onclick="event_menu_prinicipal(this, event);">
                                                    <span class="scoop-micon"><i class="icon-mustache"></i></span>
                                                    <span class="scoop-mtext">Autenticación</span>
                                                    <span class="scoop-mcaret"></span>
                                                </a>
                                            </li>

                                        </ul>
                                    </li>
                                    <!--Menu correspondencia-->
                                    <li class="scoop-hasmenu" id="CR-PR-00">
                                        <a href="javascript:void(0)" title="Correspondencia (Gestión y trámite de la correspondencia)">
                                            <span class="scoop-micon"><i class="icon-envelope-open"></i></span>
                                            <span class="scoop-mtext">Correspondencia</span>
                                            <span id="id_resp_aprobar_" class="scoop-badge badge-success" style="display:none">New</span>
                                            <span class="scoop-mcaret"></span>
                                        </a>
                                        <ul class="scoop-submenu">
                                            <li class=" " id="CR-GT-01" onclick="event_menu_prinicipal(this, event);">
                                                <a href="javascript:void(0)">
                                                    <span class="scoop-micon"><i class="icon-cursor "></i></span>
                                                    <span class="scoop-mtext">Gestión de correspondencia</span>
                                                    <span id="id_task_asignado_" class="scoop-badge badge-warning" >0</span>
                                                    <span class="scoop-mcaret"></span>
                                                </a>
                                            </li>
                                            <li class="  " id="CR-HR-02" onclick="event_menu_prinicipal(this, event);">
                                                <a href="javascript:void(0)">
                                                    <span class="scoop-micon"><i class="icon-cursor "></i></span>
                                                    <span class="scoop-mtext">Historial de correspondencia</span>
                                                    <span class="scoop-mcaret"></span>
                                                </a>
                                            </li>
                                            <li class=" " id="CR-HR-300" onclick="event_menu_prinicipal(this, event);">
                                                <a href="javascript:void(0)">
                                                    <span class="scoop-micon"><i class="icon-cursor "></i></span>
                                                    <span class="scoop-mtext">Radicación simplificada</span>
                                                    <span id="id_task_asignado___" class="scoop-badge badge-warning">0</span>
                                                    <span class="scoop-mcaret"></span>
                                                </a>
                                            </li>
                                            <li class=" " id="CR-RP-03" onclick="event_menu_prinicipal(this, event);">
                                                <a href="javascript:void(0)" title="Respuesta pendientes por aprobación">
                                                    <span id="id_resp_aprobar__" class="scoop-badge badge-success" style="display:none">New</span>
                                                    <span class="scoop-micon"><i class="icon-cursor "></i></span> 
                                                    <span class="scoop-mtext" style="align-content:flex-start">Por aprobación</span>        
                                                    <span class="scoop-mcaret"></span>          
                                                </a>
                                            </li>
                                            <li class=" " id="CR-CR-07" onclick="event_menu_prinicipal(this, event);">
                                                <a href="javascript:void(0)">
                                                    <span class="scoop-micon"><i class="icon-cursor"></i></span>
                                                    <span class="scoop-mtext">Remisión interna de correspondencia</span>
                                                    <span class="scoop-mcaret"></span>
                                                </a>
                                            </li>
                                            <li class=" " id="CR-RE-04" onclick="event_menu_prinicipal(this, event);">
                                                <a href="javascript:void(0)">
                                                    <span class="scoop-micon"><i class="icon-cursor"></i></span>
                                                    <span class="scoop-mtext">Respuestas a correo pendientes por confirmar</span>
                                                    <span class="scoop-mcaret"></span>
                                                </a>
                                            </li>
                                             <li class=" scoop-hasmenu" id="CR-PR-11">
                                                <a href="javascript:void(0)">
                                                    <span class="scoop-micon"><i class="icon-cursor"></i></span>
                                                    <span class="scoop-mtext">Radicación</span>
                                                    <span class="scoop-mcaret"></span>
                                                </a>
                                                <ul class="scoop-submenu" id="class_rad">
                                                    
                                                </ul>
                                              
                                            </li>
                                            <li class=" scoop-hasmenu" id="CR-PR-12">
                                                <a href="javascript:void(0)">
                                                    <span class="scoop-micon"><i class="icon-cursor"></i></span>
                                                    <span class="scoop-mtext">Consulta de radicación</span>
                                                    <span class="scoop-mcaret"></span>
                                                </a>
                                               <ul class="scoop-submenu" id="class_rad_consulta">
                                                    
                                               </ul>
                                            </li>     
                                            <li class=" scoop-hasmenu" id="CR-PR-02" >
                                                <a href="javascript:void(0)">
                                                    <span class="scoop-micon"><i class="icon-bulb"></i></span>
                                                    <span class="scoop-mtext">Gestión de respuestas físicas</span>
                                                    <span class="scoop-mcaret"></span>
                                                </a>
                                                <ul class="scoop-submenu">
                                                    <li class=" " id="CR-RE-05" onclick="event_menu_prinicipal(this, event);">
                                                        <a href="javascript:void(0)">
                                                            <span class="scoop-micon"><i class="icon-cursor"></i></span>
                                                            <span class="scoop-mtext">Respuestas físicas pendientes por enviar</span>
                                                            <span class="scoop-mcaret"></span>
                                                        </a>
                                                    </li>
                                                </ul>
                                                <ul class="scoop-submenu">
                                                    <li class=" " id="CR-RC-06" onclick="event_menu_prinicipal(this, event);">
                                                        <a href="javascript:void(0)">
                                                            <span class="scoop-micon"><i class="icon-cursor"></i></span>
                                                            <span class="scoop-mtext">Respuesta físicas pendiente por confirmar</span>
                                                            <span class="scoop-mcaret"></span>
                                                        </a>
                                                    </li>
                                                </ul>
                                            </li>

                                            <li class=" scoop-hasmenu" id="CR-PR-03">
                                                <a href="javascript:void(0)">
                                                    <span class="scoop-micon"><i class="icon-cursor"></i></span>
                                                    <span class="scoop-mtext">Gestión de guías</span>
                                                    <span class="scoop-mcaret"></span>
                                                </a>
                                                <ul class="scoop-submenu">
                                                    <li class=" " id="CR-RG-08" onclick="event_menu_prinicipal(this, event);">
                                                        <a href="javascript:void(0)">
                                                            <span class="scoop-micon"><i class="icon-cursor"></i></span>
                                                            <span class="scoop-mtext">Registrar guías de envío</span>
                                                            <span class="scoop-mcaret"></span>
                                                        </a>
                                                    </li>
                                                    <li class=" " id="CR-GG-09" onclick="event_menu_prinicipal(this, event);">
                                                        <a href="javascript:void(0)">
                                                            <span class="scoop-micon"><i class="icon-cursor"></i></span>
                                                            <span class="scoop-mtext">Gestionar guías de envío</span>
                                                            <span class="scoop-mcaret"></span>
                                                        </a>
                                                    </li>
                                                    <li class=" " id="CR-CG-10" onclick="event_menu_prinicipal(this, event);">
                                                        <a href="javascript:void(0)">
                                                            <span class="scoop-micon"><i class="icon-cursor"></i></span>
                                                            <span class="scoop-mtext">Consultar guías de envío</span>
                                                            <span class="scoop-mcaret"></span>
                                                        </a>
                                                    </li>
                                                </ul>
                                            </li>


                                           
                                            <li class=" " id="CR-PR-13">
                                                <a href="javascript:void(0)">
                                                    <span class="scoop-micon"><i class="icon-cursor"></i></span>
                                                    <span class="scoop-mtext">Autenticación</span>
                                                    <span class="scoop-mcaret"></span>
                                                </a>
                                            </li>
                                        </ul>
                                    

                                    <!--Menu gestion-->
                                    <li class="scoop-hasmenu" id="GD-PR-00">
                                        <a href="javascript:void(0)" title="Gestión documental (Gestión de procesos documentales)">
                                            <span class="scoop-micon"><i class="icon-folder-alt"></i></span>
                                            <span class="scoop-mtext">Gestión documental</span>
                                              <span id="id_docu_aprobacion_" class="scoop-badge badge-warning" style="display:none">New</span>
                                            <span class="scoop-mcaret"></span>
                                        </a>
                                        <ul class="scoop-submenu" >
                                             <li class=" scoop-hasmenu" id="GD-PR-13">
                                                <a href="javascript:void(0)">
                                                    <span class="scoop-micon"><i class="icon-cursor"></i></span>
                                                    <span class="scoop-mtext">Gestión de documentos</span>
                                                  
                                                    <span class="scoop-mcaret"></span>
                                                </a>
                                                <ul class="scoop-submenu" title="Gestión de documentos electrónicos y expedientes">
                                                    <li class=" " id="GD-GD-14" onclick="event_menu_prinicipal(this, event);">
                                                        <a href="javascript:void(0)">
                                                            <span class="scoop-micon"><i class="icon-chart"></i></span>
                                                            <span class="scoop-mtext">Gestión de documentos electrónicos</span>
                                                            <span class="scoop-mcaret"></span>
                                                        </a>
                                                    </li>
                                                </ul>
                                                <ul class="scoop-submenu">
                                                    <li class=" " id="GD-MD-15" onclick="event_menu_prinicipal(this, event);">
                                                        <a href="javascript:void(0)">
                                                            <span class="scoop-micon"><i class="icon-chart"></i></span>
                                                            <span class="scoop-mtext">Mis documentos compartidos</span>
                                                            
                                                            <span class="scoop-mcaret"></span>
                                                        </a>
                                                    </li>
                                                </ul>
                                                <ul class="scoop-submenu">
                                                    <li class=" " id="GD-MR-16" onclick="event_menu_prinicipal(this, event);">
                                                        <a href="javascript:void(0)">
                                                            <span class="scoop-micon"><i class="icon-chart"></i></span>
                                                            <span class="scoop-mtext">Consulta radicaciones internas</span>
                                                            <span class="scoop-mcaret"></span>
                                                        </a>
                                                    </li>
                                                </ul>
                                                <ul class="scoop-submenu">
                                                    <li class=" " id="GD-MP-17" onclick="event_menu_prinicipal(this, event);">
                                                        <a href="javascript:void(0)">
                                                            <span class="scoop-micon"><i class="icon-chart"></i></span>
                                                            <span class="scoop-mtext">Radicaciones pendientes por asignar</span>
                                                            <span class="scoop-mcaret"></span>
                                                        </a>
                                                    </li>
                                                </ul>
                                            </li>
                                            <li class=" scoop-hasmenu" id="GD-UD-01">
                                                <a href="javascript:void(0)">
                                                    <span class="scoop-micon"><i class="icon-cursor"></i></span>
                                                    <span class="scoop-mtext">Gestión de expedientes</span>
                                                    <span class="scoop-mcaret"></span>
                                                </a>
                                                <ul class="scoop-submenu">
                                                    <li class=" " id="GD-RU-02" onclick="event_menu_prinicipal(this, event);">
                                                        <a href="javascript:void(0)">
                                                            <span class="scoop-micon"><i class="icon-chart"></i></span>
                                                            <span class="scoop-mtext">Registro de expedientes</span>
                                                            <span class="scoop-mcaret"></span>
                                                        </a>
                                                    </li>
                                                </ul>
                                                <ul class="scoop-submenu">
                                                    <li class=" " id="GD-CE-03" onclick="event_menu_prinicipal(this, event);">
                                                        <a href="javascript:void(0)">
                                                            <span class="scoop-micon"><i class="icon-chart"></i></span>
                                                            <span class="scoop-mtext">Consulta de expedientes</span>
                                                            <span class="scoop-mcaret"></span>
                                                        </a>
                                                    </li>
                                                </ul>
                                               
                                            </li>

                                            <li class=" scoop-hasmenu" id="GD-MR-15">
                                                <a href="javascript:void(0)">
                                                    <span class="scoop-micon"><i class="icon-cursor"></i></span>
                                                    <span class="scoop-mtext">Gestión y migración</span>
                                                    <span class="scoop-mcaret"></span>
                                                </a>
                                                <ul class="scoop-submenu">
                                                    <li class=" " id="GD-MR-99" onclick="event_menu_prinicipal(this, event);">
                                                        <a href="javascript:void(0)">
                                                            <span class="scoop-micon"><i class="icon-chart"></i></span>
                                                            <span class="scoop-mtext">Migración de documentos</span>
                                                            <span class="scoop-mcaret"></span>
                                                        </a>
                                                    </li>
                                                </ul>
                                                <ul class="scoop-submenu">
                                                    <li class=" " id="GD-MR-17" onclick="event_menu_prinicipal(this, event);">
                                                        <a href="javascript:void(0)">
                                                            <span class="scoop-micon"><i class="icon-chart"></i></span>
                                                            <span class="scoop-mtext">Consulta documentos migrados</span>
                                                            <span class="scoop-mcaret"></span>
                                                        </a>
                                                    </li>
                                                </ul>        
                                            </li>
                                            <li class=" scoop-hasmenu" id="GD-ALM-03">
                                                <a href="javascript:void(0)">
                                                    <span class="scoop-micon"><i class="icon-cursor"></i></span>
                                                    <span class="scoop-mtext">Gestión física</span>
                                                    <span class="scoop-mcaret"></span>
                                                </a>
                                                 <ul class="scoop-submenu" title="Gestión de unidades de conservación">
                                                    <li class=" " id="GD-CU-04" onclick="event_menu_prinicipal(this, event);">
                                                        <a href="javascript:void(0)">
                                                            <span class="scoop-micon"><i class="icon-chart"></i></span>
                                                            <span class="scoop-mtext">Gestión de unidades</span>
                                                            <span class="scoop-mcaret"></span>
                                                        </a>
                                                    </li>
                                                </ul>
                                                <ul class="scoop-submenu" title="Gestión topografica o física de la estructura de archivo">
                                                    <li class=" " id="GD-GT-05" onclick="event_menu_prinicipal(this, event);">
                                                        <a href="javascript:void(0)">
                                                            <span class="scoop-micon"><i class="icon-chart"></i></span>
                                                            <span class="scoop-mtext">Gestión topografica</span>
                                                            <span class="scoop-mcaret"></span>
                                                        </a>
                                                    </li>
                                                </ul>
                                            </li>
                                             <li class=" scoop-hasmenu" id="GD-GI-06">
                                                <a href="javascript:void(0)">
                                                    <span class="scoop-micon"><i class="icon-cursor"></i></span>
                                                    <span class="scoop-mtext">Gestión de instrumentos</span>
                                                    <span class="scoop-mcaret"></span>
                                                </a>
                                                <ul class="scoop-submenu">
                                                    <li class=" " id="GD-AC-06" onclick="event_menu_prinicipal(this, event);">
                                                        <a href="javascript:void(0)">
                                                            <span class="scoop-micon"><i class="icon-chart"></i></span>
                                                            <span class="scoop-mtext">Gestión de cuadros de clasificación</span>
                                                            <span class="scoop-mcaret"></span>
                                                        </a>
                                                    </li>
                                                </ul>
                                                <ul class="scoop-submenu">
                                                    <li class=" " id="GD-AI-08" onclick="event_menu_prinicipal(this, event);">
                                                        <a href="javascript:void(0)">
                                                            <span class="scoop-micon"><i class="icon-chart"></i></span>
                                                            <span class="scoop-mtext">Gestión de tablas de retención y valoración  </span>
                                                            <span class="scoop-mcaret"></span>
                                                        </a>
                                                    </li>
                                                </ul>
                                                <ul class="scoop-submenu">
                                                    <li class=" " id="GD-CC-07" onclick="event_menu_prinicipal(this, event);">
                                                        <a href="javascript:void(0)">
                                                            <span class="scoop-micon"><i class="icon-chart"></i></span>
                                                            <span class="scoop-mtext">Consulta de cuadros de clasificación</span>
                                                            <span class="scoop-mcaret"></span>
                                                        </a>
                                                    </li>
                                                </ul>
                                                
                                                <ul class="scoop-submenu">
                                                    <li class=" " id="GD-CR-09" onclick="event_menu_prinicipal(this, event);">
                                                        <a href="javascript:void(0)">
                                                            <span class="scoop-micon"><i class="icon-chart"></i></span>
                                                            <span class="scoop-mtext">Consulta de tablas de retención</span>
                                                            <span class="scoop-mcaret"></span>
                                                        </a>
                                                    </li>
                                                </ul>
                                                
                                                <ul class="scoop-submenu" style="display:none">
                                                    <li class=" " id="Li3" onclick="event_menu_prinicipal(this, event);">
                                                        <a href="javascript:void(0)">
                                                            <span class="scoop-micon"><i class="icon-chart"></i></span>
                                                            <span class="scoop-mtext">Consulta tablas de valoración</span>
                                                            <span class="scoop-mcaret"></span>
                                                        </a>
                                                    </li>
                                                </ul>
                                            </li>
                                            <li class=" scoop-hasmenu" id="GD-PR-06" >
                                                <a href="javascript:void(0)">
                                                    <span class="scoop-micon"><i class="icon-cursor"></i></span>
                                                    <span class="scoop-mtext">Reportes de gestión</span>
                                                    <span class="scoop-mcaret"></span>
                                                </a>
                                                <ul class="scoop-submenu">
                                                    <li class=" " id="GD-RG-07" onclick="event_menu_prinicipal(this, event);">
                                                        <a href="javascript:void(0)">
                                                            <span class="scoop-micon"><i class="icon-chart"></i></span>
                                                            <span class="scoop-mtext">Reportes de gestión</span>
                                                            <span class="scoop-mcaret"></span>
                                                        </a>
                                                    </li>
                                                </ul>
                                            </li>
                                           
                                            <li class=" scoop-hasmenu" id="GD-PR-11">
                                                <a href="javascript:void(0)">
                                                    <span class="scoop-micon"><i class="icon-cursor"></i></span>
                                                    <span class="scoop-mtext">Estructura Orgánica</span>
                                                    <span class="scoop-mcaret"></span>
                                                </a>
                                                <ul class="scoop-submenu">
                                                    <li class=" " id="GD-AE-12" onclick="event_menu_prinicipal(this, event);">
                                                        <a href="javascript:void(0)">
                                                            <span class="scoop-micon"><i class="icon-chart"></i></span>
                                                            <span class="scoop-mtext">Gestión de estructura</span>
                                                            <span class="scoop-mcaret"></span>
                                                        </a>
                                                    </li>
                                                </ul>
                                            </li>
                                           
                                            <li class="  " id="GD-DC-19" onclick="event_menu_prinicipal(this, event);">
                                                <a href="javascript:void(0)" title="Documentos pendientes por revisar">
                                                    <span class="scoop-micon"><i class="icon-link"></i></span>
                                                    <span id="id_docu_aprobacion__" class="scoop-badge badge-warning" style="display:none">New</span>
                                                    <span class="scoop-mtext">Por revisar</span>

                                                </a>
                                            </li>
                                        </ul>
                                    </li>
                                    <!--Menu autenticacion-->
                                    <li class="scoop-submenu" id="GD-PR-20" onclick="event_menu_prinicipal(this, event);">        
                                            <a href="javascript:void(0)" title="Gestión de contraseña">
                                                <span class="scoop-micon"><i class="icon-lock-open"></i></span>
                                                <span class="scoop-mtext">Autenticación</span>
                                                <span class="scoop-mcaret"></span>
                                            </a>          
                                    </li>

                                </ul>

                            </div>

                        </nav>
                        <div class="scoop-content" id="coop_content">
                            <div class="row" id="content_iframe_ds" style="overflow: hidden; display: none">
                                <div class="col-12" style="overflow: hidden">
                                   
                                    <iframe class="embed-responsive-item_" runat="server" style="width: 100%; overflow: no-display" id="ContentPlacenter_ifrm_ds_"
                                        frameborder="0" scrolling="no"></iframe>
                                </div>
                            </div>
                            <div id="card_general_ini_text" class="row_" style="overflow:auto; background-color:white">
                                <div class="col-12 m-1 p-2">
                                    <div class="row_" id="content_card">
                                        <div class="row">
                                            <div class="col-12">
                                                <a class="ml-4 mt-2 coll_sap_active"  style="color:#fecd33;text-decoration:none" data-toggle="collapse" href="#collpase_group_card_chek_0001"  aria-expanded="false" ><i class="fas fa-caret-down fa-2x  "></i>            
                                                </a>
                                                <span  class="ml-2 mt-2 logo-text h6"> Accesos directos</span>
                                                <span id="content_card_count" class="ml-2 mt-2 d-none" > </span>
                                                <hr />
                                            </div>
                                        </div>
                                        <div class="row collapse show" id="collpase_group_card_chek_0001">
                                            <div class="col-12">
                                                <div id="group_card_chek_0001" class="card-columns m-1">
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row_" id="content_card_wf">
                                        <div class="row">
                                            <div class="col-12">
                                                <a class="ml-4 mt-2 coll_sap_active collapsed"  style="color:#fecd33;text-decoration:none" data-toggle="collapse" href="#collpase_group_card_chek_0001_wf"  aria-expanded="false" ><i class="fas fa-caret-down fa-2x "></i>
                                                </a>
                                                <span  class="ml-2 mt-2 h6"> Workflow </span>
                                                <h7 id="content_card_wf_count" class="ml-2 mt-2 d-none"> </h7>
                                                <hr />
                                            </div>
                                        </div>
                                        <div class="row collapse  " id="collpase_group_card_chek_0001_wf">
                                            <div class="col-12">
                                                <div id="group_card_chek_0001_wf" class="card-columns m-1">
                                                </div>
                                            </div>

                                        </div>
                                    </div>
                                    <div class="row_" id="content_card_da">
                                        <div class="row ">
                                            <div class="col-12">
                                                <a class="ml-4 mt-2 coll_sap_active collapsed" style="color:#fecd33;text-decoration:none" data-toggle="collapse" href="#collpase_group_card_chek_0001_da"  aria-expanded="false" ><i class="fas fa-caret-down fa-2x "></i>
                                                </a>
                                                <span  class="ml-2 mt-2 h6"> DocuArchi contendor </span>
                                                <h7 id="content_card_da_count" class="ml-2 mt-2 d-none"> </h7>
                                                <hr />
                                            </div>
                                        </div>
                                        <div class="row collapse  " id="collpase_group_card_chek_0001_da">
                                            <div class="col-12">
                                                <div id="group_card_chek_0001_da" class="card-columns m-1">
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row_" id="content_card_rd">
                                        <div class="row ">
                                            <div class="col-12">
                                                <a class="ml-4 mt-2 coll_sap_active collapsed" style="color:#fecd33;text-decoration:none" data-toggle="collapse" href="#collpase_group_card_chek_0001_rd"  aria-expanded="false" ><i class="fas fa-caret-down fa-2x "></i>
                                                </a>
                                                <span  class="ml-2 mt-2 h6"> Correspondencia </span>
                                                <h7 id="content_card_rd_count" class="ml-2 mt-2 d-none"> </h7>
                                                <hr />
                                            </div>
                                        </div>
                                        <div class="row collapse  " id="collpase_group_card_chek_0001_rd">
                                            <div class="col-12">
                                                <div id="group_card_chek_0001_rd" class="card-columns m-1">
                                                </div>
                                            </div>
                                            <div class="col-12">
                                                <div id="group_card_chek_0001_rd_1" class="card-columns m-1">
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row_" id="content_card_gd">
                                        <div class="row ">
                                            <div class="col-12">
                                                <a class="ml-4 mt-2 coll_sap_active collapsed" style="color:#fecd33; text-decoration:none" data-toggle="collapse" href="#collpase_group_card_chek_0001_gd"  aria-expanded="false" ><i class="fas fa-caret-down fa-2x "></i>
                                                </a>
                                                <span  class="ml-2 mt-2 h6"> Gestion documental </span>
                                                <h7 id="content_card_gd_count" class="ml-2 mt-2 d-none"> </h7>
                                                <hr />
                                            </div>
                                        </div>
                                        <div class="row collapse  " id="collpase_group_card_chek_0001_gd">
                                            <div class="col-12">
                                                <div id="group_card_chek_0001_gd" class="card-columns m-1">
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div id="icono" style="float: initial; display: none">
                            <asp:ImageButton ID="ImageButtonSesion" runat="server"
                                AlternateText=" Cerrar Sesion Gestor" ToolTip=""
                                Style="background-color: white; color: #053061" />

                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="da_loading" id="container_loading_iframe" style="display: none">
            <div class="da_loading_center">Cargando</div>
            <div class="da_loader"></div>
        </div>
         <!--Popup redirect -->
         <div class="modal fade modal_opacity" id="modal_alert_sesion_time_out" role="dialog" >
             <div class="modal-dialog modal-dialog-scrollable">
                 <div class="modal-content" >
                     <div class="modal-header">
                        <h4 style="color: black" class="modal-title"></h4>     
                    </div>
                     <div class="modal-body">
                          <div class="row "> 
                              <div class="col-2">
                                  <i id="i_lert_sesion_time_out" class="fad fa-portal-exit fa-3x"></i>
                              </div>
                              <div class="col-10">
                                  <span class="h6 font-weight-light" id="title_lert_sesion_time_out"> Su sesión a caducado por inactividad, lo vamos a direccionar a la página principal.</span>       
                              </div>    
                          </div>      
                    </div>
                     
                     <div class=" modal-footer">
                        <button type="button" id="Button_rdedirect_pag" class="btn  btn-primary"  title="">Aceptar</button>
                     </div>
                 </div>
             </div>
         </div>     
        <!--Trmina Popup -->
        <!--Popup redirect -->
         <div class="modal fade modal_opacity" id="modal_sesion_end" role="dialog" >
             <div class="modal-dialog modal-dialog-scrollable">
                 <div class="modal-content" >
                     <div class="modal-header">
                        <h4 style="color: black" class="modal-title"></h4>     
                    </div>
                     <div class="modal-body">
                          <div class="row "> 
                              <div class="col-2">
                                  <i id="i_sesion_end" class="fad fa-portal-exit fa-3x"></i>
                              </div>
                              <div class="col-10">
                                  <span class="h6 font-weight-light" id="title_sesion_end">Desea cerrar la sesión actual?</span>       
                              </div>    
                          </div>      
                    </div>
                     
                     <div class=" modal-footer">
                        <button type="button" id="Button_sesion_end_cancelar" class="btn  btn-light"  title="">Cancelar</button>
                        <button type="button" id="Button_sesion_end" class="btn  btn-primary"  title="">Aceptar</button>
                     </div>
                 </div>
             </div>
         </div>     
        <!--Trmina Popup -->
        <!--mensaje_progreso evento-->
        <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 200px">
            <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
            Processing ...
        </div>
       <!--mensaje_personalizado-->
    </form>
    <script accesskey="javascript" type="text/javascript">
        $("#fakeLoader").fakeLoader();
        function MantenSesion() {
            try {
                var CONTROLADOR = "../workflow/refresh_session.ashx";
                var head = document.getElementsByTagName('body').item(0);
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
    </script>
</body>
</html>
