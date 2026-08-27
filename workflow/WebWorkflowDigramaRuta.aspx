<%@ Page Language="vb" AutoEventWireup="true" CodeBehind="WebWorkflowDigramaRuta.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebWorkflowDigramaRuta" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register Assembly="MindFusion.Diagramming.WebForms" Namespace="MindFusion.Diagramming.WebForms"
    TagPrefix="ndiag" %>
<%@ Register Assembly="MindFusion.Extenders" Namespace="MindFusion.Diagramming.WebForms"
    TagPrefix="ndiag" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">

<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Diagramador de rutas</title>
    <script src="../js/ui/jquery-3.4.1.min.js"></script>  
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
     <link href="../ccs/themes/base/jquery.ui.all.css" rel="stylesheet" type="text/css" />
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
     <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
      <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <link href="../Styles/samples.css" rel="stylesheet" />
    <script  src="../Awesome/js/all.js"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet"/>
  <link href="../Awesome/css/brands.css" rel="stylesheet"/>
  <link href="../Awesome/css/solid.css" rel="stylesheet"/>
    <script  src="../Awesome/js/brands.js"></script>
  <script  src="../Awesome/js/solid.js"></script>
  <script  src="../Awesome/js/fontawesome.js"></script>
    <script src="../js/workflow/WebWorkflowDigramaRuta.js"></script>
    <script src="../js/validate_campos.js"></script>
    <script src="../js/java_general/general_code_java.js?v=20260827-compatible-events4"></script>
    <style type="text/css">
        .auto-style1 {
            height: 26px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="True">
            <Scripts>
                <asp:ScriptReference Path="../Scripts/CustomNode.js" />
                <asp:ScriptReference Path="../Scripts/IconNode.js" />
            </Scripts>
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
                //
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
                       
                    if (elment_postbak.id == "Button_me_active_men_dive") {
                        document.getElementById("Label_edita_escript_evento").innerHTML = document.getElementById("HiddenField_result_edit_script").value;
                        auto_size_popup_edit_escript();
                    }
                if (elment_postbak.type == "button" || elment_postbak.type == "submit") {
                    elment_postbak.value = value_element;
                    elment_postbak.disabled = false;
                }
               
                if (elment_postbak.id == "ImageButtonGuardar") {
                   
                }
                }
                catch (err) {
                    alert(" Funcion CheckStatus asincrona WebWorkflowDigramaRuta.aspx" + err.message);
                }
                finally {
                    progres_hiden('progres_bar');                
                }
            }

            </script>
   
        <nav id="menucab" class="navbar navbar-expand-sm nav_botota_person_gray modal_content_no_back_inferior">       
            <button id="nav_togle_display" class="navbar-toggler" type="button" style="background-color: #6d7fcc" data-toggle="collapse" data-target="#navbarNavDropdown">
                <span class="navbar-toggler-icon_"><i style="color: white" class="fad fa-th-list"></i></span>
            </button>
            <div class="collapse navbar-collapse row" id="navbarNavDropdown">             
                <ul class="navbar-nav col-md-12" >
                    <li class="nav-item dropdown active ml-2 mr-0 active_">
                        <a class="nav-link  dropdown-toggle " style="color: #6d7fcc" href="#" id="A5" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc; display: none" class="fad fa-th-list"></i> Rutas 
                        </a>
                        <ul class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">
                           <li><a style="color: #6d7fcc" href="#" class="dropdown-item font-weight-light" onclick="activa_menu_general_diference(event,this,'I-R-WF')"><i class="fal fa-file-upload"></i> Importar ruta </a></li> 
                           <li><a style="color: #6d7fcc; display:none" href="#" class="dropdown-item font-weight-light" onclick="activa_menu_general_diference(event,this,'A-RD-WF')"><i class="fal fa-folder-open"></i> Abrir rutas disponibles</a></li> 
                           <li><a style="color: #6d7fcc" href="#" class="dropdown-item font-weight-light" onclick="activa_menu_general_diference(event,this,'A-EXP-WF')"><i class="fal fa-file-download"></i> Exportar diagrama ruta actual</a></li> 
                        </ul>
                    </li>
                    <li class="nav-item dropdown active ml-2 mr-0 active_">
                        <a class="nav-link  dropdown-toggle " style="color: #6d7fcc" href="#" id="A2" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc; display: none" class="fad fa-th-list"></i> Configuración
                        </a>
                        <div class="dropdown-menu ">
                            <div class="dropdown-submenu ">
                                <a class="dropdown-item font-weight-light  dropdown-toggle" style="color: #6d7fcc" href="#" id="A1" data-toggle="dropdown" aria-haspopup="true" aria-expanded="true"><i style="color: #0062cc; display: none" class="fad fa-th-list"></i> Rutas
                                </a>
                                <div class="dropdown-menu ">
                                    <a style="color: #6d7fcc" title="Configuración de espacios de nombres" href="#" class="dropdown-item font-weight-light" onclick="activa_menu_general_diference(event,this,'A-E-N-W')"><i class="fal fa-wrench"></i> Espacios de nombres </a>
                                    <a style="color: #6d7fcc" title="Administra los campos que se muestran ne la lista de espera" href="#" class="dropdown-item font-weight-light" onclick="activa_menu_general_diference(event,this,'S-GAU')"><i class="fal fa-wrench"></i> Campos de la ruta </a>
                                </div>

                            </div>
                            <div class="dropdown-submenu ">
                                <a class="dropdown-item font-weight-light  dropdown-toggle" style="color: #6d7fcc" href="#" id="A4" data-toggle="dropdown" aria-haspopup="true" aria-expanded="true"><i style="color: #0062cc; display: none" class="fad fa-th-list"></i> Gabinetes 
                                </a>
                                <div class="dropdown-menu">
                                    <a style="color: #6d7fcc" title="Agrega un nuevo gabinete de almacenamiento" href="#" class="dropdown-item font-weight-light" onclick="activa_menu_general_diference(event,this,'A-GAW')"><i class="fal fa-plus"></i> Agregar un nuevo gabinete de almacenamiento </a>
                                    <a style="color: #6d7fcc" title="Edita configuración gabinetes de almacenamiento" href="#" class="dropdown-item font-weight-light" onclick="activa_menu_general_diference(event,this,'A-EDGW')"><i class="fal fa-edit"></i> Edita configuración de gabinetes </a>
                                </div>
                            </div>
                             <div class="dropdown-submenu ">
                                <a class="dropdown-item font-weight-light  dropdown-toggle" style="color: #6d7fcc" href="#" id="A8" data-toggle="dropdown" aria-haspopup="true" aria-expanded="true"><i style="color: #0062cc; display: none" class="fad fa-th-list"></i> Actividades 
                                </a>
                                <div class="dropdown-menu">
                                    <a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" onclick="activa_menu_general_diference(event,this,'R-GRUPO-WB')"><i class="fal fa-object-group"></i> Relacionar actividad a grupo workflow</a>
                                    <a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" onclick="activa_menu_general_diference(event,this,'L-GRUPO-WB')"><i class="fal fa-users"></i> Lista grupo workflow relacionado a la actividad</a>
                                </div>
                            </div>
                            <div class="dropdown-submenu ">
                                <a class="dropdown-item font-weight-light  dropdown-toggle" style="color: #6d7fcc" href="#" id="A9" data-toggle="dropdown" aria-haspopup="true" aria-expanded="true"><i style="color: #0062cc; display: none" class="fad fa-th-list"></i> Conectores 
                                </a>
                                <div class="dropdown-menu">
                                    <a class="dropdown-item font-weight-light" style="color: #6d7fcc" href="#" onclick="activa_menu_general_diference(event,this,'C-CONECTOR-WB')"><i class="fal fa-wrench"></i> Configurar conector</a>
                                </div>
                            </div>
                        </div>
                       
                    </li>
                    <li class="nav-item dropdown active ml-2 mr-0 active_">
                        <a class="nav-link  dropdown-toggle " style="color: #6d7fcc" href="#" id="A3" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc; display: none" class="fad fa-th-list"></i> Eventos 
                        </a>
                         <div class="dropdown-menu ">
                            <div class="dropdown-submenu ">
                                <a class="dropdown-item font-weight-light  dropdown-toggle" style="color: #6d7fcc" href="#" id="A6" data-toggle="dropdown" aria-haspopup="true" aria-expanded="true"><i style="color: #0062cc; display: none" class="fad fa-th-list"></i>  Eventos de escritorio
                                </a>
                                <div class="dropdown-menu ">
                                    <a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Editar código fuente para evento worokflow escritorio" onclick="activa_menu_general_diference(event,this,'M-INICIO-WE')"><i class="fal fa-wrench"></i> INICIO</a>
                                    <a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Editar código fuente para evento worokflow escritorio" onclick="activa_menu_general_diference(event,this,'M-PREINICIO-WE')"><i class="fal fa-wrench"></i> PREINICIO</a>
                                    <a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Editar código fuente para evento worokflow escritorio" onclick="activa_menu_general_diference(event,this,'M-TOMARTAREA-WE')"><i class="fal fa-wrench"></i> TOMARTAREA</a>
                                    <a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Editar código fuente para evento worokflow escritorio" onclick="activa_menu_general_diference(event,this,'M-ENLACE-WE')"><i class="fal fa-wrench"></i> ENLACE</a>
                                    <a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Editar código fuente para evento worokflow escritorio" onclick="activa_menu_general_diference(event,this,'M-PRETERMINARACTIVIDAD-WE')"><i class="fal fa-wrench"></i> PRETERMINARACTIVIDAD</a>
                                    <a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Editar código fuente para evento worokflow escritorio" onclick="activa_menu_general_diference(event,this,'M-TERMINARACTIVIDAD-WE')"><i class="fal fa-wrench"></i> TERMINARACTIVIDAD</a>
                                    <a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Editar código fuente para evento worokflow escritorio" onclick="activa_menu_general_diference(event,this,'M-PENDIENTE-WE')"><i class="fal fa-wrench"></i> PENDIENTE</a>
                                    <a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Editar código fuente para evento worokflow escritorio" onclick="activa_menu_general_diference(event,this,'M-ADJUNTOS-WE')"><i class="fal fa-wrench"></i> ADJUNTOS</a>
                                    <a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Editar código fuente para evento worokflow escritorio" onclick="activa_menu_general_diference(event,this,'M-ADJUNTARIMAGENES-WE')"><i class="fal fa-wrench"></i> ADJUNTARIMAGENES</a>
                                    <a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Editar código fuente para evento worokflow escritorio" onclick="activa_menu_general_diference(event,this,'M-CREARIMAGENES-WE')"><i class="fal fa-wrench"></i> CREARIMAGENES</a>
                                    <a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Editar código fuente para evento worokflow escritorio" onclick="activa_menu_general_diference(event,this,'M-DEFAULTESCRIPT-WE')"><i class="fal fa-wrench"></i> DEFAULTESCRIPT</a>
                                    <a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Editar código fuente para evento worokflow escritorio" onclick="activa_menu_general_diference(event,this,'M-PREACTUALIZAR-WE')"><i class="fal fa-wrench"></i> PREACTUALIZAR</a>
                                    <a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Editar código fuente para evento worokflow escritorio" onclick="activa_menu_general_diference(event,this,'M-ACTUALIZAR-WE')"><i class="fal fa-wrench"></i> ACTUALIZAR</a>
                                    <a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Editar código fuente para evento worokflow escritorio" onclick="activa_menu_general_diference(event,this,'M-FINALIZAR-WE')"><i class="fal fa-wrench"></i> FINALIZAR</a>
                                    <a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Editar código fuente para evento worokflow escritorio" onclick="activa_menu_general_diference(event,this,'M-ADJUNTARIMAGENES_SISTEMA-WE')"><i class="fal fa-wrench"></i> ADJUNTARIMAGENES_SISTEMA</a>
                                    <a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Editar código fuente para evento worokflow escritorio" onclick="activa_menu_general_diference(event,this,'M-CREARIMAGENES_SISTEMA-WE')"><i class="fal fa-wrench"></i> CREARIMAGENES_SISTEMA</a>
                                    <a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Editar código fuente para evento worokflow escritorio" onclick="activa_menu_general_diference(event,this,'M-DEFAULTSCRIPT_SISTEMA-WE')"><i class="fal fa-wrench"></i> DEFAULTSCRIPT_SISTEMA</a>
                                </div>

                            </div>
                            <div class="dropdown-submenu ">
                                <a class="dropdown-item font-weight-light  dropdown-toggle" style="color: #6d7fcc" href="#" id="A7" data-toggle="dropdown" aria-haspopup="true" aria-expanded="true"><i style="color: #0062cc; display: none" class="fad fa-th-list"></i> Eventos web 
                                </a>
                                <div class="dropdown-menu">
                                    <a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Editar código fuente para evento worokflow web" onclick="activa_menu_general_diference(event,this,'M-INICIO-WEB')"><i class="fal fa-wrench"></i> INICIO</a>
                                    <a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Editar código fuente para evento worokflow web" onclick="activa_menu_general_diference(event,this,'M-PREINICIO-WEB')"><i class="fal fa-wrench"></i> PREINICIO</a>
                                    <a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Editar código fuente para evento worokflow web" onclick="activa_menu_general_diference(event,this,'M-TOMARTAREA-WEB')"><i class="fal fa-wrench"></i> TOMARTAREA</a>
                                    <a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Editar código fuente para evento worokflow web" onclick="activa_menu_general_diference(event,this,'M-ENLACE-WEB')"><i class="fal fa-wrench"></i> ENLACE</a>
                                    <a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Editar código fuente para evento worokflow web" onclick="activa_menu_general_diference(event,this,'M-PRETERMINARACTIVIDAD-WEB')"><i class="fal fa-wrench"></i> PRETERMINARACTIVIDAD</a>
                                    <a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Editar código fuente para evento worokflow web" onclick="activa_menu_general_diference(event,this,'M-TERMINARACTIVIDAD-WEB')"><i class="fal fa-wrench"></i> TERMINARACTIVIDAD</a>
                                    <a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Editar código fuente para evento worokflow web" onclick="activa_menu_general_diference(event,this,'M-PENDIENTE-WEB')"><i class="fal fa-wrench"></i> PENDIENTE</a>
                                    <a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Editar código fuente para evento worokflow web" onclick="activa_menu_general_diference(event,this,'M-ADJUNTOS-WEB')"><i class="fal fa-wrench"></i> ADJUNTOS</a>
                                    <a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Editar código fuente para evento worokflow web" onclick="activa_menu_general_diference(event,this,'M-ADJUNTARIMAGENES-WEB')"><i class="fal fa-wrench"></i> ADJUNTARIMAGENES</a>
                                    <a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Editar código fuente para evento worokflow web" onclick="activa_menu_general_diference(event,this,'M-CREARIMAGENES-WEB')"><i class="fal fa-wrench"></i> CREARIMAGENES</a>
                                    <a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Editar código fuente para evento worokflow web" onclick="activa_menu_general_diference(event,this,'M-DEFAULTESCRIPT-WEB')"><i class="fal fa-wrench"></i> DEFAULTESCRIPT</a>
                                    <a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Editar código fuente para evento worokflow web" onclick="activa_menu_general_diference(event,this,'M-PREACTUALIZAR-WEB')"><i class="fal fa-wrench"></i> PREACTUALIZAR</a>
                                    <a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Editar código fuente para evento worokflow web" onclick="activa_menu_general_diference(event,this,'M-ACTUALIZAR-WEB')"><i class="fal fa-wrench"></i> ACTUALIZAR</a>
                                    <a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Editar código fuente para evento worokflow web" onclick="activa_menu_general_diference(event,this,'M-FINALIZAR-WEB')"><i class="fal fa-wrench"></i> FINALIZAR</a>
                                    <a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Editar código fuente para evento worokflow web" onclick="activa_menu_general_diference(event,this,'M-ADJUNTARIMAGENES_SISTEMA-WEB')"><i class="fal fa-wrench"></i> ADJUNTARIMAGENES_SISTEMA</a>
                                    <a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Editar código fuente para evento worokflow web" onclick="activa_menu_general_diference(event,this,'M-CREARIMAGENES_SISTEMA-WEB')"><i class="fal fa-wrench"></i> CREARIMAGENES_SISTEMA</a>
                                    <a style="color: #6d7fcc" class="dropdown-item font-weight-light" href="#" title="Editar código fuente para evento worokflow web" onclick="activa_menu_general_diference(event,this,'M-DEFAULTSCRIPT_SISTEMA-WEB')"><i class="fal fa-wrench"></i> DEFAULTSCRIPT_SISTEMA</a>
                                </div>
                            </div>

                        </div>
                    </li>
                   

                </ul>
            </div>
        </nav>
         <asp:UpdatePanel ID="UpdatePanel_menu_var_event" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <input id="Hidden_menu_var_event_dive" type="hidden" value="" runat="server"/>
                        <asp:Button ID="Button_me_active_men_dive" runat="server" Text="" Style="display:none; width:1px; height:1px" />
                    </ContentTemplate>
                </asp:UpdatePanel>  
       
         
        <div id="Menutol" class="modal_content_no_back_inferior_" style="height:auto; width: 100%; border: 0.2px none Black; top: 0px; left: 0px">            
                <asp:UpdatePanel ID="updatemenu" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <nav id="nav_menu" class="navbar navbar-expand-sm nav_botota_person modal_content_no_back_inferior">
                            <button class="navbar-toggler" type="button" style="background-color: #6d7fcc" data-toggle="collapse" data-target="#navbarNavDropdown_">
                                <span class="navbar-toggler-icon_"><i style="color: white" class="fad fa-th-list"></i></span>
                            </button>
                            <div class="collapse navbar-collapse row" id="navbarNavDropdown_">
                                <ul class="navbar-nav">
                                    <li class="nav-item active ml-2 active_">
                                        <a class="nav-link font-weight-light" style="color: #6d7fcc" title="Guardar cambios de la ruta" href="#" onclick="activa_boton_client_server('ImageButtonGuardar');"><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fas fa-save fa-lg"></i>  </a>
                                    </li>
                                </ul>
                                <asp:DropDownList ID="DropDownList_rutas_disponibles_workflow" CssClass="custom-select  w-25"
                                    runat="server" 
                                    Style="margin-left: 10px; min-width: 200px" AutoPostBack="True">
                                </asp:DropDownList>
                                 <ul class="navbar-nav">
                                    <li class="nav-item active ml-2 active_">
                                        <a class="nav-link font-weight-light" style="color: #6d7fcc" title="Nueva actividad de grupos de usuarios" href="#" onclick="activa_boton_client_server('ImageButtonCrearGrupoActividadUsuario');"><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-users-medical fa-lg"></i>  </a>
                                    </li>
                                </ul>
                                 <ul class="navbar-nav" style="display:none">
                                    <li class="nav-item active ml-2 active_">
                                        <a class="nav-link font-weight-light" style="color: #6d7fcc" title="Nueva actividad de usuario" href="#" onclick="activa_boton_client_server('ImageButton_Crear_Actividad_usuario');"><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-user-plus fa-lg"></i>  </a>
                                    </li>
                                </ul>
                                 <div class="navbar-nav">              
                                    <li class="nav-item active ml-2 active_ ">
                                        <a class="nav-link font-weight-light" style="color: #6d7fcc" title="Nueva actividad de enlace y digitalización" href="#" onclick="activa_boton_client_server('ImageButtonCrearActividadEnlaceDocumento');"><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-scanner-image fa-lg"></i>  </a>
                                    </li>
                                </div>
                                <ul class="navbar-nav">
                                    <li class="nav-item active ml-2 active_">
                                        <a class="nav-link font-weight-light" style="color: #6d7fcc" title="Elimina elemento seleccionado" href="#" onclick="activa_boton_client_server('ImageButtonEliminarActividades');"><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-times fa-lg"></i>  </a>
                                    </li>
                                </ul>
                                <ul class="navbar-nav">
                                    <li class="nav-item active ml-2 active_">
                                        <a class="nav-link font-weight-light" style="color: #6d7fcc" title="Conectar actividades para flujo de trabajo" href="#" onclick="activa_boton_client_server('ImageButton_conectar_actividades');"><i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-long-arrow-alt-right fa-lg"></i>  </a>
                                    </li>
                                </ul>
                                <asp:DropDownList ID="DropDownZonFactor" CssClass="custom-select ml-4 mr-4" 
                                    runat="server"  Width="70px"
                                    Style="margin-left: 3px; float: left" AutoPostBack="True">
                                </asp:DropDownList>
                                <ul class="navbar-nav">
                                    <li class="nav-item active ml-2 active_">
                                        <asp:CheckBox ID="CheckBox_Grid_alineamiento" runat="server"  Checked="false" AutoPostBack="true" class="mr-0" />
                                        <span style="color: #6d7fcc"> Visible grid </span>
                                    </li>
                                </ul>
                            </div>
                            
                        </nav>
                        <div style="display: none">
                            <asp:ImageButton ID="ImageButtonGuardar" runat="server" ImageUrl="../workflow/imageneswf/Guardar_actividad_inactive.png"
                                ToolTip="Guardar cambios de la ruta"
                                ImageAlign="Left" Width="25px" Height="25px" Style="margin-left: 3px; display: block; margin-top: 3px" CssClass="alterna_image" />
                            <asp:Label ID="Label_rutas_workflow" runat="server" Text="Rutas disponibles" Style="float: left; margin-left: 10px; margin-top: 5px"></asp:Label>
                            
                            <asp:ImageButton ID="ImageButtonCrearGrupoActividadUsuario" runat="server" ImageUrl="../workflow/imageneswf/Actividad_grupo_usuario.png"
                                ToolTip="Nueva actividad de grupos de usuarios" AlternateText="Nueva actividad de grupos de usuarios" CssClass="alterna_image"
                                ImageAlign="Left" Width="40px" Height="30px" Style="margin-left: 1px" />
                            <asp:ImageButton ID="ImageButton_Crear_Actividad_usuario" runat="server"
                                ToolTip="Nueva actividad de usuario" AlternateText="Crear nueva actividad de usuario" CssClass="alterna_image"
                                ImageAlign="Bottom" Width="30px" Height="30px" Style="margin-left: 1px; margin-bottom: 5px; float: left; display: none" ImageUrl="../workflow/imageneswf/actividad_usuario.png" />
                            
                            <asp:ImageButton ID="ImageButtonCrearActividadEnlaceDocumento" runat="server"
                                ToolTip="Nueva actividad de enlace y  digitalización" AlternateText="Nueva actividad de enlace y  digitialización" CssClass="alterna_image"
                                ImageAlign="Bottom" Width="40px" Height="30px" Style="margin-left: 1px; margin-bottom: 5px; float: left" ImageUrl="../workflow/imageneswf/Actividad_Enlace_Digitalizacion_dos.png" />
                            
                            <asp:ImageButton ID="ImageButton_Crear_Actividad_Sistema" runat="server"
                                ToolTip="Nueva actividad de sistema" AlternateText="Crear nueva actividad de sistema" CssClass="alterna_image"
                                ImageAlign="Bottom" Width="30px" Height="30px" Style="margin-left: 1px; margin-bottom: 5px; float: left" ImageUrl="../workflow/imageneswf/ActividadSistema.png" />
                            
                            <asp:ImageButton ID="ImageButtonEliminarActividades" runat="server"
                                ToolTip="Elimina elemento seleccionado" AlternateText="Elimina actividad seleccionada" CssClass="alterna_image"
                                ImageAlign="Bottom" Width="25px" Height="20px" Style="margin-left: 3px; margin-bottom: 5px; margin-top: 5px; float: left" ImageUrl="../workflow/imageneswf/Eliminar_Actividad.png" />
                            
                            <asp:ImageButton ID="ImageButton_conectar_actividades" runat="server"
                                ToolTip="Conectar actividades para flujo de trabajo" AlternateText="Conectar actividades para flujo de trabajo" CssClass="alterna_image"
                                ImageAlign="Bottom" Width="30px" Height="25px" Style="margin-left: 1px; margin-bottom: 5px; margin-top: 1px; float: left" ImageUrl="../workflow/imageneswf/Conectar_Actividades.png" />
                            <asp:Button ID="Button_visor_emergente" runat="server" Text="Button" Style="display: none" />
                            <asp:Label ID="Label_zon" runat="server" Text="Zoon" Style="float: left; margin-left: 10px; margin-top: 5px"></asp:Label>
                           
                           
                        </div>

                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>      
            
         <asp:UpdatePanel ID="UpdatePanel_diagran_view" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div id="content" style="background-color: #EEEEEE; width:100%">
                    <ndiag:InteractivityExtender runat="server" ID="interactivityExtender" TargetControlID="diagramView" />
                    <ndiag:DiagramView runat="server" ID="diagramView" ClientSideMode="Canvas" Behavior="Custom"
                        AllowInplaceEdit="false" JsLibraryLocation="../Scripts/MindFusion.Diagramming.js" Diagram-ShowGrid="false"
                        Style="position:absolute; left: 0px; top: 0px; right: 0px; bottom: 0px"  
                          NodeSelectedScript="onNodeSelected"  LinkSelectedScript="onLinkSelected"  EnableViewState="true" >
                    </ndiag:DiagramView>
                    <asp:HiddenField ID="HiddenField_value_selecion" runat="server"  />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <div id="footer" style="height: 24px;">
            <asp:Label ID="Label_Estado_documento" runat="server" Text="Estado" style="margin-left:5px"></asp:Label>
            
        </div>
    
        <!--paginas_externas_popou-->
          <div id="paginas_externas_popou">
            <asp:Panel ID="Panel_paginas_externas_popou" runat="server" Style="display:none; color: black; width: 100%; height: 100%" CssClass="modal_content_general_">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_paginas_externas_popou" runat="server"  
                     TargetControlID="ButtonSalir_paginas_externas_popou" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_paginas_externas_popou" PopupControlID="Panel_paginas_externas_popou" ></asp:ModalPopupExtender>
                <div id="modal_content_Panel_paginas_externas_popou" class="modal-content">
                    <div id="divcabecer2_paginas_externas_popou" class="modal_title_superior_ modal-header">
                           <h6 class="modal-title d-inline ml-1">Configuración campos de la ruta</h6>
                           <button type="button" value="Button_cerrar_paginas_externas_popou" class="close da_event_captive">&times;</button>   
                        
                    </div>
                    <div id="contenido_procesa_paginas_externas_popou" style="background-color: white; width: 100%; height: 99%; color: black; background-color: #FFFFFF" class="modal_content_back">
                        <asp:UpdatePanel ID="UpdatePanel_paginas_externas_popou" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <iframe id="Iframe_paginas_externas_popup__" runat="server" frameborder="0" style="width: 100%; height: 100%; overflow: hidden"></iframe>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
                <div style="display:none; height:1px">
                     <asp:Button ID="Button_paginas_externas_popou" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="ButtonSalir_paginas_externas_popou" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                     <asp:Button ID="Button_cerrar_paginas_externas_popou" runat="Server" Text="" CssClass="modal_boton_hiden"/>
                </div>
                
            </asp:Panel>
        </div>
        <!--importa_ruta-->
          <div id="importa_ruta">
            <asp:Panel ID="Panel_importa_ruta" runat="server" Style="display:none; color: black; width: 60%; height: auto" CssClass="modal_content_general_">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_importa_ruta" runat="server" BehaviorID="Panel_importa_ruta" 
                     TargetControlID="ButtonSalir_importa_ruta" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_importa_ruta" PopupControlID="Panel_importa_ruta" ></asp:ModalPopupExtender>
                <div id="modal_content_Panel_importa_ruta" class="modal-content">
                    <div id="divcabecer2_importa_ruta" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title d-inline ml-1">Importa ruta general con actividades</h6>
                        <button type="button" value="Button_cerrar_importa_ruta" class="close da_event_captive">&times;</button>

                    </div>
                    <div id="contenido_procesa_importa_ruta" style="background-color: white; width: 100%; height: 99%; color: black; background-color: #FFFFFF; border-top:none; overflow:auto" class="modal_content_back modal-body">

                        <div id="div_contenido_titulo" style="text-align: center" class="p-1">               
                            <span>Rutas disponibles</span>      
                        </div>
                        <asp:UpdatePanel ID="UpdatePanel_importa_ruta" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row">          
                                    <div class="col-12">
                                         <asp:DropDownList ID="DropDownList_rutas_disponibles" CssClass="custom-select" runat="server" Style="width: 100%"></asp:DropDownList>
                                    </div>
                                </div>
                                
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <asp:UpdatePanel ID="UpdatePanel_adunta_archivo_ruta" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row mt-2">
                                    <div class="col-12">
                                         <span>Archivo extensión .vdx</span>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-9">
                                        <asp:TextBox ID="TextBox_archivo_import" Enabled="false" runat="server" Style=" width: 100%"></asp:TextBox>
                                    </div>
                                    <div class="col-3 justify-content-end">
                                         <asp:Button ID="Button_seleccion_archivo" runat="server" Text="Examinar"  CssClass="btn btn-success" OnClientClick="eliminar_ajaxtolkit();" />
                                    </div>
                                </div>
                                
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        
                    </div>
                     <div class="modal-footer justify-content-end">
                         <asp:UpdatePanel ID="UpdatePanel_buton_importa_ruta" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>          
                                <asp:Button ID="Button_importa_ruta_archivo" runat="server" Text="Importar" CssClass="btn btn-success"  />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                     </div>
                </div>
                <div style="display:none; height:1px">
                    <asp:Button ID="Button_importa_ruta" CssClass="invisible" runat="server" Text="Button" Height="0px" Width="0px" />
                    <asp:Button ID="ButtonSalir_importa_ruta" CssClass="invisible" runat="server" Text="Button" Height="0px" Width="0px" />
                     <asp:Button ID="Button_cerrar_importa_ruta" runat="Server" Text="" CssClass="modal_boton_hiden"/>
                </div>
                 
            </asp:Panel>
        </div>
        <!--abrir_rutas_disponibles-->
          <div id="abrir_rutas_disponibles" style="">
              <asp:Panel ID="Panel_abrir_rutas_disponibles" runat="server" Style="display: none; color: White; width: 50%; height: auto" CssClass="modal_content_general_">
                  <asp:ModalPopupExtender ID="ModalPopupExtender_edition_abrir_rutas_disponibles" runat="server"
                      TargetControlID="ButtonSalir_abrir_rutas_disponibles" BackgroundCssClass="FondoAplicacion"
                      CancelControlID="Button_cerrar_abrir_rutas_disponibles" PopupControlID="Panel_abrir_rutas_disponibles">
                  </asp:ModalPopupExtender>
                  <div id="modal_content_Panel_abrir_rutas_disponibles" class="modal-content">
                      <div id="divcabecer2_abrir_rutas_disponibles" class="modal_title_superior_ modal-header">
                          <h6 class="modal-title d-inline ml-1"></h6>
                          <button type="button" value="Button_cerrar_abrir_rutas_disponibles" class="close da_event_captive">&times;</button>
                      </div>
                      <div id="contenido_procesa_abrir_rutas_disponibles" style="background-color: white; width: 100%; height: 99%; color: black; background-color: #FFFFFF; border-top: none; overflow: auto" class="modal_content_back modal-body">
                          <asp:UpdatePanel ID="UpdatePanel_abrir_rutas_disponibles" runat="server" UpdateMode="Conditional">
                              <ContentTemplate>
                                  <div class="row">
                                      <div class="col-12 text-center">
                                          <span>Rutas disponibles</span>
                                      </div>
                                  </div>
                                  <div class="row">
                                      <div class="col-12 mt-2">
                                          <asp:DropDownList ID="DropDownListrutasdisponibles" runat="server" Style="width: 100%" CssClass="custom-select"></asp:DropDownList>
                                      </div>
                                  </div>
                              </ContentTemplate>
                          </asp:UpdatePanel>
                      </div>
                      <div class="modal-footer justify-content-end">
                          <asp:UpdatePanel ID="UpdatePanel_buton_abrir_rutas_disponibles" runat="server" UpdateMode="Conditional">
                              <ContentTemplate>
                                  <asp:Button ID="Button_abrir_ruta" runat="server" Text="Aceptar" CssClass="btn btn-success" />
                              </ContentTemplate>
                          </asp:UpdatePanel>
                      </div>
                  </div>

                  <div style="display: none; height: 1px">
                      <asp:Button ID="Button_abrir_rutas_disponibles" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                      <asp:Button ID="ButtonSalir_abrir_rutas_disponibles" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                      <asp:Button ID="Button_cerrar_abrir_rutas_disponibles" runat="Server" Text="" CssClass="modal_boton_hiden" />
                  </div>

              </asp:Panel>
        </div>
        <!--cargar documento!-->
          <div id="contenido_procesa_sube_documento_adjunto" >
            <asp:Panel ID="Panel_sube_documento_adjunto" runat="server" Style="display:none; color: White; width: 50%; height: auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_sube_documento_adjunto" runat="Server" BackgroundCssClass="ModalBackgroud_gorund" TargetControlID="Button_sube_documento_adjunto"
                    PopupControlID="Panel_sube_documento_adjunto" CancelControlID="Button3_cerrar_adjunta" ></asp:ModalPopupExtender>
                <div id="Div_cabecera" class="modal_title_superior">                
                    <asp:Label ID="Label11" runat="server" Text="Adjuntar" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Div_title_sube_documento_adjunto"" style="float: right">
                        <asp:Button ID="Button3_cerrar_adjunta" runat="Server" Text="X" CssClass="modal_boton_hiden"
                             ToolTip="Cerrar ventana" />
                    </div>
                </div>
               
                 <div id="contenido_procesa_sube_documento_respuesta" style="width: 100%; height: 100%" class="modal_content_back">           
                            <asp:UpdatePanel ID="UpdatePanel_descarga" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>  
                                    <asp:Panel ID="Panel_descarga_ajax" runat="server">
                                        <div id="drop_zone_" style="width: 100%; height:auto; overflow: auto">
                                            <asp:AjaxFileUpload ID="AjaxFileUpload_dowload" runat="server" ThrobberID="drop_zone_"
                                                ContextKeys="fred"
                                                AllowedFileTypes="vxd"
                                                MaximumNumberOfFiles="1" OnClientUploadComplete="activa_boton_dowload" />
                                        </div>
                                    </asp:Panel>
                                         <asp:Button ID="Button_guardar_desicion" runat="server" Text="Button" Style="display: none" />                           
                                    &nbsp  
                                    <asp:Label ID="Label_estado_carga" runat="server" Text="Estado" Style="font-family: Arial; font-size: 10px"></asp:Label>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        
                   
                      <asp:Button ID="Button_sube_documento_adjunto" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                </div>


            </asp:Panel>
          
        </div>
        <!--agrega_nuevo_gabinete-->
          <div id="agrega_nuevo_gabinete">
            <asp:Panel ID="Panel_agrega_nuevo_gabinete" runat="server" Style="display:none; color:black; width: 60%; height:auto" CssClass="modal_content_general_">
                <asp:ModalPopupExtender ID="ModalPopupExtender_agrega_nuevo_gabinete" runat="server" BehaviorID="Panel_agrega_nuevo_gabinete" TargetControlID="ButtonSalir_agrega_nuevo_gabinete" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_agrega_nuevo_gabinete" PopupControlID="Panel_agrega_nuevo_gabinete" ></asp:ModalPopupExtender>
                <div id="modal_content_Panel_agrega_nuevo_gabinete" class="modal-content">  
                <div id="divcabecer2_agrega_nuevo_gabinete"  class="modal_title_superior_ modal-header"> 
                      <h6 class="modal-title d-inline ml-1">Agregar gabinete</h6>
                      <button type="button" value="Button_cerrar_agrega_nuevo_gabinete" class="close da_event_captive">&times;</button>       
                </div>
                    <div id="contenido_procesa_agrega_nuevo_gabinete" style="background-color: white; width: 100%; height: 99%; color: black; background-color: #FFFFFF; border-top: none; overflow: auto" class="modal_content_back modal-body">
                        <div class="row mt-1">
                            <div class="col-6">
                                <span>Seleccione el gabinete</span>
                            </div>
                            <div class="col-6">
                                <asp:UpdatePanel ID="UpdatePanel_nombre_gabinete_agrega" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:DropDownList ID="DropDownList_Nombre_Gabinete_Agrega" runat="server" Style="width: 100%" CssClass="custom-select" AutoPostBack="true"></asp:DropDownList>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                        <asp:UpdatePanel ID="UpdatePanel_parametros_gabinete_agrega" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row mt-1">
                                    <div class="col-6">
                                        <span>Ruta física gabinete</span>

                                    </div>
                                    <div class="col-6">
                                        <asp:TextBox ID="TextBox_ruta_fisica_gab_agrega" runat="server" Style="width: 100%" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row mt-1">
                                    <div class="col-6">
                                        <span>Ruta busqueda gabinete</span>
                                    </div>
                                    <div class="col-6">
                                        <asp:TextBox ID="TextBox_ruta_almacena_gab_agrega" runat="server" Style="width: 100%" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row mt-1">
                                    <div class="col-6">
                                        <span>Ruta almacena gabinete</span>
                                    </div>
                                    <div class="col-6">
                                        <asp:TextBox ID="TextBox_ruta_busqueda_gab_agrega" runat="server" Style="width: 100%" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row mt-1">
                                    <div class="col-6">
                                        <span>Base de datos gabinete</span>
                                    </div>
                                    <div class="col-6">
                                        <asp:DropDownList ID="DropDownList_base_datos_gabinete_agrega" runat="server" Style="width: 100%" CssClass="custom-select"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row mt-1">
                                    <div class="col-6">
                                        <span>DBMS</span>
                                    </div>
                                    <div class="col-6">
                                        <asp:DropDownList ID="DropDownList_dbms_gabinete_agrega" runat="server"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row mt-1">
                                    <div class="col-6">
                                        <span>UNC-SERVIDOR</span>
                                    </div>
                                    <div class="col-6">
                                        <asp:TextBox ID="TextBox_unc_gabinete_agrega" runat="server" Style="width: 100%" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row mt-1">
                                    <div class="col-6">
                                        <span>Usuario base de datos</span>
                                    </div>
                                    <div class="col-6">
                                        <asp:TextBox ID="TextBox_usuario_db_gabinete_agrega" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row mt-1">
                                    <div class="col-6">
                                        <span>Pasword usuario base de datos</span>

                                    </div>
                                    <div class="col-6">
                                        <asp:TextBox ID="TextBox_pasword_db_gabinete_agrega" runat="server" TextMode="Password" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                
                            </ContentTemplate>
                        </asp:UpdatePanel>

                    </div>
                    <div class="modal-footer justify-content-end">
                        <asp:UpdatePanel ID="UpdatePanel_buton_opciones_agrega_gabinete" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Button ID="Button_agrega_gabinete" runat="server" Text="Aceptar" CssClass="btn btn-success" ToolTip="Agrega un nuevo gabinete a la configuración de workflow" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
                <div style="display:none; height:1px">
                     <asp:Button ID="Button_agrega_nuevo_gabinete" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                     <asp:Button ID="ButtonSalir_agrega_nuevo_gabinete" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                     <asp:Button ID="Button_cerrar_agrega_nuevo_gabinete" runat="Server" Text=""  CssClass="modal_boton_hiden"/>
                </div>           
            </asp:Panel>
        </div>
        <!--edita_configuracion_gabinete-->
          <div id="edita_configuracion_gabinete">
            <asp:Panel ID="Panel_edita_configuracion_gabinete" runat="server" Style="display:none; color: black; width: 60%; height:auto" CssClass="modal_content_general_">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edita_configuracion_gabinete" runat="server"  TargetControlID="ButtonSalir_edita_configuracion_gabinete" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_edita_configuracion_gabinete" PopupControlID="Panel_edita_configuracion_gabinete" ></asp:ModalPopupExtender>
                <div id="modal_content_Panel_edita_configuracion_gabinete" class="modal-content">  
                <div id="divcabecer2_edita_configuracion_gabinete"  class="modal_title_superior_ modal-header"> 
                      <h6 class="modal-title d-inline ml-1">Edita configuración gabinete</h6>
                     <button type="button" value="Button_cerrar_edita_configuracion_gabinete" class="close da_event_captive">&times;</button>       
                </div>
                    <div id="contenido_procesa_edita_configuracion_gabinete" style="background-color: white; width: 100%; height: 99%; color: black; background-color: #FFFFFF; border-top: none; overflow: auto" class="modal_content_back modal-body">
                        <div class="row mt-1">
                            <div class="col-6">
                                <span>Seleccione el gabinete</span>
                            </div>
                            <div class="col-6">
                                <asp:UpdatePanel ID="UpdatePanel_nombre_gabinete_edita" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:DropDownList ID="DropDownList_Nombre_Gabinete_edita" runat="server" Style="width: 100%" AutoPostBack="true" CssClass="custom-select"></asp:DropDownList>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                        <asp:UpdatePanel ID="UpdatePanel_parametros_gabinete_edita" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row mt-1">
                                    <div class="col-6">
                                        <span > Ruta física gabinete </span> 
                                    </div>
                                    <div class="col-6">
                                         <asp:TextBox ID="TextBox_ruta_fisica_gab_edita" runat="server" Style="width: 100%" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row mt-1">
                                    <div class="col-6">
                                        <span>Ruta busqueda gabinete</span>
                                    </div>
                                    <div class="col-6">
                                        <asp:TextBox ID="TextBox_ruta_almacena_gab_edita" runat="server" Style="width: 100%" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row mt-1">
                                    <div class="col-6">
                                         <span>Ruta almacena gabinete</span>
                                    </div>
                                    <div class="col-6">
                                        <asp:TextBox ID="TextBox_ruta_busqueda_gab_edita" runat="server" Style="width: 100%" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row mt-1">
                                    <div class="col-6">
                                        <span>Base de datos gabinete</span>
                                    </div>
                                    <div class="col-6">
                                         <asp:DropDownList ID="DropDownList_base_datos_gabinete_edita" runat="server"  CssClass="custom-select"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row mt-1">
                                    <div class="col-6">
                                        <span>DBMS</span>        
                                    </div>
                                    <div class="col-6">
                                        <asp:DropDownList ID="DropDownList_dbms_gabinete_edita" runat="server" CssClass="custom-select"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row mt-1">
                                    <div class="col-6">
                                        <span>UNC-SERVIDOR</span>
                                    </div>
                                    <div class="col-6">
                                         <asp:TextBox ID="TextBox_unc_gabinete_edita" runat="server" Style="width: 100%" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                 <div class="row mt-1">
                                    <div class="col-6">
                                        <span>Usuario base de datos</span>
                                    </div>
                                    <div class="col-6">
                                        <asp:TextBox ID="TextBox_usuario_db_gabinete_edita" runat="server"></asp:TextBox>
                                    </div>
                                </div>
                                 <div class="row mt-1">
                                    <div class="col-6">
                                         <span>Pasword usuario base de datos</span>                            
                                    </div>
                                    <div class="col-6">
                                        <asp:TextBox ID="TextBox_pasword_db_gabinete_edita" runat="server" TextMode="Password"></asp:TextBox>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>          
                    </div>
                    <div class="modal-footer">
                        <asp:UpdatePanel ID="UpdatePanel_buton_opciones_edita_gabinete" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Button ID="Button_edita_gabinete" runat="server" Text="Aceptar" CssClass="btn btn-success" ToolTip="Agrega un nuevo gabinete a la configuración de workflow" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
                <div style="display:none; height:1px">
                     <asp:Button ID="Button_edita_configuracion_gabinete" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="ButtonSalir_edita_configuracion_gabinete" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                     <asp:Button ID="Button_cerrar_edita_configuracion_gabinete" runat="Server" Text="" CssClass="modal_boton_hiden"/>
                </div>
                
            </asp:Panel>
        </div>
        <!--edita_escript_evento-->
          <div id="edita_escript_evento">
            <asp:Panel ID="Panel_edita_escript_evento" runat="server" Style="display:none; color: black; width: 100%; height: 100%" CssClass="modal_content_general_">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_edita_escript_evento" runat="server" BehaviorID="Panel_edita_escript_evento" 
                     TargetControlID="ButtonSalir_edita_escript_evento" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_edita_escript_evento" PopupControlID="Panel_edita_escript_evento" ></asp:ModalPopupExtender>
                <div id="modal_content_Panel_edita_escript_evento" class="modal-content">
                    <div id="divcabecer2_edita_escript_evento" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title d-inline ml-1" id="Label_edita_escript_evento"></h6>
                        <button type="button" value="Button_cerrar_edita_escript_evento" class="close da_event_captive">&times;</button>                   
                    </div>
                    <div id="contenido_procesa_edita_escript_evento" style="background-color: white; width: 100%; height: 99%; color: black; background-color: #FFFFFF; border-top:none; overflow:hidden" class="modal-body_">
                        <asp:UpdatePanel ID="UpdatePanel_contenido_edita_escript_evento" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:HiddenField ID="HiddenField_result_edit_script" runat="server" Value="" />
                                <asp:HiddenField ID="HiddenField_nom_event" runat="server" Value="" />                  
                                    <asp:TextBox ID="TextBox_contenido_edita_escript_evento" runat="server" spellcheck="false" TextMode="MultiLine" Style="width: 99.5%; height: 100%"></asp:TextBox>               
                            </ContentTemplate>
                        </asp:UpdatePanel>   
                    </div>
                    <div class="modal-footer justify-content-end" id="modal-footer_Panel_edita_escript_evento">
                        <asp:UpdatePanel ID="UpdatePanel_botones_contenido_edita_escrip_evento" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Button ID="Button_compilar_evento_escript" runat="server" Text="Compilar" CssClass="btn btn-success" />
                                <asp:Button ID="Button_actualiza_evento_escript" runat="server" Text="Guardar" CssClass="btn btn-success" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
                <div style="display:none; height:1px">
                      <asp:Button ID="Button_edita_escript_evento" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                      <asp:Button ID="ButtonSalir_edita_escript_evento" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                      <asp:Button ID="Button_cerrar_edita_escript_evento" runat="Server" Text="" CssClass="modal_boton_hiden" />
                </div>       
            </asp:Panel>
        </div>
        <!--crear_actividad_workflow-->
          <div id="crear_actividad_workflow" style="">
            <asp:Panel ID="Panel_crear_actividad_workflow" runat="server" Style="display:none; color:black; width: 50%; height:auto" CssClass="modal_content_general_">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_crear_actividad_workflow" runat="server" 
                     TargetControlID="ButtonSalir_crear_actividad_workflow" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_crear_actividad_workflow" PopupControlID="Panel_crear_actividad_workflow" ></asp:ModalPopupExtender>
                <div id="modal_content_Panel_crear_actividad_workflow" class="modal-content">
                    <div id="divcabecer2_crear_actividad_workflow" class="modal_title_superior_ modal-header">
                          <h6 class="modal-title d-inline ml-1">Crear actividad</h6>
                          <button type="button" value="Button_cerrar_crear_actividad_workflow" class="close da_event_captive">&times;</button>              
                    </div>
                    <div id="contenido_procesa_crear_actividad_workflow" style="background-color: white; width: 100%; height: 99%; color: black; background-color: #FFFFFF" class="modal_content_back">         
                        <asp:UpdatePanel ID="UpdatePanel_crear_actividad_workflow" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:HiddenField ID="HiddenField_tipo_actividad_seleccion" runat="server" Value="" />
                                <div class="p-2">
                                    <div class="row mt-2">
                                        <div class="col-6">
                                            <span>Nombre actividad *</span>
                                        </div>
                                        <div class="col-6 p-0">
                                             <asp:TextBox ID="TextBox_nombre_actvidad_crear_actividad"  runat="server" style="width:80%"   MaxLength="45"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row mt-2">
                                        <div class="col-6">
                                            <span>Descripcíon actividad</span>
                                        </div>
                                        <div class="col-6 p-0">
                                             <asp:TextBox ID="TextBox_descripcion_crear_actividad" runat="server"  style="width:80%"   MaxLength="45"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row  mt-2">
                                        <div class="col-6">
                                            <spna> Tipo actividad </spna>
                                        </div>
                                        <div class="col-6 pl-0">
                                            <asp:DropDownList ID="DropDownList_tipo_actividad" CssClass="custom-select" runat="server"></asp:DropDownList>
                                        </div>
                                    </div> 
                                    <div class="row mt-4">
                                        <div class="col-12 justify-content-end">
                                            <asp:CheckBox ID="CheckBox_option_crea_grupo_workflow" Text="" Checked="true" runat="server" />
                                            <span> Crea y relaciona grupo workflow </span>
                                        </div>       
                                    </div>
                                    
                                    
                                </div>
                             
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div  class="modal-footer align-content-end">
                            <asp:UpdatePanel ID="UpdatePanel_buton_crear_actividad_workflow" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Button ID="Button_crear_actividad_workflow_confirmar" runat="server" Text="Aceptar" CssClass="btn btn-success"  />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                </div>
                <div style="display: none; height: 1px">
                    <asp:Button ID="Button_crear_actividad_workflow" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="ButtonSalir_crear_actividad_workflow" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="Button_cerrar_crear_actividad_workflow" runat="Server" Text="" CssClass="modal_boton_hiden" />
                </div>
            </asp:Panel>
        </div>
        <!--confirma_eliminar_elmento_diagrama-->
          
            <asp:Panel ID="Panel_confirma_eliminar_elmento_diagrama" runat="server" Style="display:none; color:black; width:auto; height: auto" CssClass="">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_confirma_eliminar_elmento_diagrama" runat="server"  TargetControlID="ButtonSalir_confirma_eliminar_elmento_diagrama" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_confirma_eliminar_elmento_diagrama" PopupControlID="Panel_confirma_eliminar_elmento_diagrama" ></asp:ModalPopupExtender>
                <div id="modal_content_Panel_confirma_eliminar_elmento_diagrama" class="modal-content">
                    <div id="divcabecer2_confirma_eliminar_elmento_diagrama" class="cabecera2_ modal-header">
                        <h6 class="modal-title d-inline ml-1"></h6>
                        <button type="button" value="Button_cerrar_confirma_eliminar_elmento_diagrama" class="close da_event_captive">&times;</button>  
                       
                    </div>
                    <div id="contenido_procesa_confirma_eliminar_elmento_diagrama" style="background-color: white; width: 100%; height: 99%; color: black; background-color: #FFFFFF;" class="modal-body">
                        <asp:UpdatePanel ID="UpdatePanel_confirma_eliminar_elmento_diagrama" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <input id="Hidden_estado_eliminar" type="hidden" value="" runat="server">
                                <div style="text-align: center" class="p-2">       
                                    <asp:Label ID="Label_title_comfirma_eliminar" runat="server" Text="Desea eliminar el elemento seleccionado ?"  CssClass="h6 "></asp:Label>            
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                    </div>
                    <div class="modal-footer">
                            <asp:UpdatePanel ID="UpdatePanel_confirma" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                     <asp:Button ID="Button_aceptar_confirmacion_eliminar_elmento_diagrama" runat="server" Text="Aceptar" CssClass="btn btn-success" />           
                                    <asp:Button ID="Button_cancelar_confirmacion_eliminar_elmento_diagrama" runat="server" Text="Cancelar" CssClass="btn btn-light " />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                    </div>
                </div>
                    <div style="display:none; height:0px">
                        <asp:Button ID="Button_cerrar_confirma_eliminar_elmento_diagrama" runat="Server" Text=""
                                ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />
                    <asp:Button ID="Button_confirma_eliminar_elmento_diagrama" CssClass="invisible" runat="server" Text="Button" Height="0px" Width="0px" />
                    <asp:Button ID="ButtonSalir_confirma_eliminar_elmento_diagrama" CssClass="invisible" runat="server" Text="Button" Height="0px" Width="0px" />
                    </div>
               
            </asp:Panel>
       
        <!--actividades_disponibles_asignacion-->
        <div id="grupos_disponibles_asignacion" style="">
              <asp:Panel ID="Panel_grupos_disponibles_asignacion" runat="server" Style="display:none; color: black; width: 40%; height: auto" CssClass="modal_content_general_">
                  <asp:ModalPopupExtender ID="ModalPopupExtender_edition_grupos_disponibles_asignacion" runat="server" 
                       TargetControlID="ButtonSalir_grupos_disponibles_asignacion" BackgroundCssClass="FondoAplicacion"
                      CancelControlID="Button_cerrar_grupos_disponibles_asignacion" PopupControlID="Panel_grupos_disponibles_asignacion" ></asp:ModalPopupExtender>
                  <div id="modal_content_Panel_grupos_disponibles_asignacion" class="modal-content">
                      <div id="divcabecer2_grupos_disponibles_asignacion" class="modal_title_superior_ modal-header">
                           <h6 class="modal-title d-inline ml-1">Grupos disponibles para relacionar</h6>
                           <button type="button" value="Button_cerrar_grupos_disponibles_asignacion" class="close da_event_captive">&times;</button>   
                      </div>
                      <div id="contenido_procesa_grupos_disponibles_asignacion" style="background-color: white; width: 100%; height: 99%; color: black; background-color: #FFFFFF; border-top:none; overflow:auto" class="modal_content_back modal-body">
                          <asp:UpdatePanel ID="UpdatePanel_grupos_disponibles_asignacion" runat="server" UpdateMode="Conditional">
                              <ContentTemplate>
                                  <div class="row -mt-1">
                                      <div class="col-12">
                                           <asp:DropDownList ID="DropDownList_grupos_disponibles_asignacion" runat="server" Style="width: 100%" CssClass="custom-select"></asp:DropDownList>
                                      </div>
                                  </div>
                                 
                              </ContentTemplate>
                          </asp:UpdatePanel>
                          
                      </div>
                       <div class="modal-footer justify-content-end">
                            <asp:UpdatePanel ID="UpdatePanel_buton_grupos_disponibles_asignacion" runat="server" UpdateMode="Conditional">
                                  <ContentTemplate>
                                      <asp:Button ID="Button_asigna_grupo_workflow" runat="server" Text="Aceptar" CssClass="btn btn-success" Style="margin-bottom: 5px" />
                                  </ContentTemplate>
                              </asp:UpdatePanel>
                       </div>
                  </div>
                  <div style="display: none; height: 0px">
                      <asp:Button ID="Button_grupos_disponibles_asignacion" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                      <asp:Button ID="ButtonSalir_grupos_disponibles_asignacion" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                      <asp:Button ID="Button_cerrar_grupos_disponibles_asignacion" runat="Server" Text="X" CssClass="modal_boton_hiden" />
                  </div>       
              </asp:Panel>
        </div>
        <!--lista_grupo_workflow_relacion-->
        <div id="lista_grupo_workflow_relacion" style="">
            <asp:Panel ID="Panel_lista_grupo_workflow_relacion" runat="server" Style="display: none; color: black; width: 50%; height: auto" CssClass="modal_content_general_">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_lista_grupo_workflow_relacion" runat="server"
                    TargetControlID="ButtonSalir_lista_grupo_workflow_relacion" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_lista_grupo_workflow_relacion" PopupControlID="Panel_lista_grupo_workflow_relacion">
                </asp:ModalPopupExtender>
                <div id="modal_content_Panel_lista_grupo_workflow_relacion" class="modal-content">
                    <div id="divcabecer2_lista_grupo_workflow_relacion" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title d-inline ml-1">Grupos relacionados a la actividad</h6>
                        <button type="button" value="Button_cerrar_lista_grupo_workflow_relacion" class="close da_event_captive">&times;</button>
                    </div>
                    <div id="contenido_procesa_lista_grupo_workflow_relacion" style="background-color: white; width: 100%; height: 99%; color: black; background-color: #FFFFFF; border-top: none; overflow: auto" class="modal_content_back modal-body">
                        <asp:UpdatePanel ID="UpdatePanel_lista_grupo_workflow_relacion" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>          
                                <div class="row-12 mt-1 mb-1">        
                                    <div class="colo-12">
                                        <asp:DropDownList ID="DropDownList_lista_grupo_workflow_relacion" runat="server" Style="width: 100%" CssClass="custom-select"></asp:DropDownList>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="modal-footer justify-content-end">
                        <asp:UpdatePanel ID="UpdatePanel_buton_lista_grupo_workflow_relacion" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Button ID="Button_elimina_relacion_grupo_workflow" runat="server" Text="Eliminar relación" CssClass="btn btn-success" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>

                </div>
                <div style="display: none; height: 1px">
                    <asp:Button ID="Button_lista_grupo_workflow_relacion" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="ButtonSalir_lista_grupo_workflow_relacion" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                    <asp:Button ID="Button_cerrar_lista_grupo_workflow_relacion" runat="Server" Text="X" CssClass="invisible" />
                </div>

            </asp:Panel>
        </div>
        <!--mensaje_progreso evento-->
        <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 200px">
            <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
            Processing ...
        </div>
       <!--configura_envi_correo_conector-->
        <div id="configura_envi_correo_conector" style="">
              <asp:Panel ID="Panel_configura_envi_correo_conector" runat="server" Style="display:none; color: black; width: 60%; height: auto" CssClass="modal_content_general_">
                  <asp:ModalPopupExtender ID="ModalPopupExtender_edition_configura_envi_correo_conector" runat="server" 
                       TargetControlID="ButtonSalir_configura_envi_correo_conector" BackgroundCssClass="FondoAplicacion"
                      CancelControlID="Button_cerrar_configura_envi_correo_conector" PopupControlID="Panel_configura_envi_correo_conector" ></asp:ModalPopupExtender>
                  <div id="modal_content_Panel_configura_envi_correo_conector" class="modal-content">
                      <div id="divcabecer2_configura_envi_correo_conector" class="modal_title_superior_ modal-header">
                            <h6 class="modal-title d-inline ml-1">Configuración del conector</h6>
                            <button type="button" value="Button_cerrar_configura_envi_correo_conector" class="close da_event_captive">&times;</button>            
                      </div>
                      <div id="contenido_procesa_configura_envi_correo_conector" style="background-color: white; width: 100%; height: 99%; color: black; background-color: #FFFFFF; border-top:none; overflow:auto" class="modal_content_back modal-body">
                          <asp:UpdatePanel ID="UpdatePanel_configura_envi_correo_conector" runat="server" UpdateMode="Conditional">
                              <ContentTemplate>
                                  <div class="row w-100 mt-1">
                                      <div class="col-12">
                                           <asp:CheckBox ID="CheckBox_estado_correo_conector" runat="server" Text="" />
                                           <span class="ml-2">Envía correo elecrónico al enviar la tarea</span>
                                      </div>  
                                  </div>
                                  <div class="row w-100 mt-1">
                                      <div class="col-12">
                                          <asp:CheckBox ID="CheckBox_autoriza_tarea" runat="server" Text="" />
                                          <span class="ml-2">El usuario debe autorizar la tarea antes de enviarla</span>
                                      </div>
                                      
                                  </div>
                                  <div class="row w-100 mt-1">
                                      <div class="col-12">
                                           <asp:CheckBox ID="CheckBox_autoriza_tarea_firma_digital" runat="server" Text="" />
                                           <span class="ml-2">El usuario debe  firmar digtalmente la autorización</span>
                                      </div>
                                      
                                  </div> 
                                   <div class="row w-100 mt-1">
                                    <div class="col-12">
                                        <asp:CheckBox ID="CheckBox_estado_copia_estructura" runat="server" Text="" />
                                        <span class="ml-2">El usuario debe  copiar documentos a expediente</span>
                                    </div>
                                     
                                </div>
                                <div class="row w-100 mt-1">
                                    <div class="col-12">
                                        <asp:CheckBox ID="CheckBox_Estado_asigna_expediente" runat="server" Text="" />
                                        <span class="ml-2">El usuario debe asignar expediente a los documentos</span>
                                    </div>
                                     
                                </div>
                                <div class="row w-100 mt-1">
                                    <div class="col-12">
                                        <asp:CheckBox ID="CheckBox_estado_firma_digital" runat="server" Text="" />
                                        <span class="ml-2">El usuario debe firmar digitalmente todos los documentos</span>
                                    </div>
                                     
                                </div>
                                  <div class="row w-100 mt-1">
                                      <div class="col-12">
                                          <asp:CheckBox ID="CheckBox_estado_valida_balanceo" runat="server" Text="" />
                                          <span class="ml-2">El sistema evalua cargas de trabajo para la asignación (Solo para actividades de grupo)</span>
                                      </div>

                                  </div>
                              </ContentTemplate>
                          </asp:UpdatePanel>          
                      </div>
                      <div class="modal-footer justify-content-end" id="modal-footer_Panel_configura_envi_correo_conector">
                          <asp:UpdatePanel ID="UpdatePanel_buton_configura_envi_correo_conector" runat="server" UpdateMode="Conditional">
                              <ContentTemplate>
                                  <asp:Button ID="Button_config_correo_conector" runat="server" Text="Aceptar" CssClass="btn btn-success" />
                              </ContentTemplate>
                          </asp:UpdatePanel>
                      </div>
                  </div>
                  <div style="display:none; height:1px">
                       <asp:Button ID="Button_configura_envi_correo_conector" CssClass="invisible" runat="server" Text="Button" Height="0px" Width="0px" />
                      <asp:Button ID="ButtonSalir_configura_envi_correo_conector" CssClass="invisible" runat="server" Text="Button" Height="0px" Width="0px" />
                      <asp:Button ID="Button_cerrar_configura_envi_correo_conector" runat="Server" Text="" CssClass="modal_boton_hiden" />
                  </div>
                  
              </asp:Panel>
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
        <!--guarda archivo-->
         <asp:UpdatePanel ID="updatapanel_iframe" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <input id="Hidden_ruta_archivo" type="hidden" value="" runat="server"/>
                <iframe runat="server" id="ifmExcel_" width="0" height="0" marginheight="0" marginwidth="0" 
                    frameborder="0"  />
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
    <script accesskey="javascript" type="text/javascript">
        AjaxFileUpload_change_text();
    </script>
</body>
</html>
