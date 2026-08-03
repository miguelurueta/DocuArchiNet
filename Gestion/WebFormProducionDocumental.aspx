<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormProducionDocumental.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormProducionDocumental" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
       <script src="../js/ui/jquery-3.4.1.min.js"></script> 
     <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
      <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
     <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <link href="../tokenzize2/tokenize2.min.css" rel="stylesheet" />
    <script src="../tokenzize2/tokenize2.1.min.js"></script>
   <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" />
     <link href="../Styles/Menu3.css" rel="stylesheet" />
    <link href="../Styles/styleMenu.css" rel="stylesheet" type="text/css" /> 
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <script src="../js/MyJavaScriptFile.js"></script> 
    <script src="../js/Reportes/JSEsportaReportes.js"></script>
    <script src="../js/gestion/WebFormProducionDocumental.js"></script>  
    <script src="../js/validate_campos.js"></script>
    <script src="../generic_control/FileUploadHandler.js" type="text/javascript"></script>
    <script src="../js/java_general/JSProgresBar.js"></script>
    <link href="../generic_control/UploadFile.css" rel="stylesheet" />
    <script src="../js/java_general/general_code_java.js"></script>
    <script src="../js/java_general/general_control_java.js"></script>
    <script src="../js/java_general/general_config.js"></script>
    <script src="../js/java_general/row_multiple_gred.js"></script>
    <script src="../js/java_general/JS_firma_digital.js"></script>
    <script src="../js/java_general/gestion_meta_dato.js"></script>
      <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.11.0/umd/popper.min.js" type="text/javascript"></script>
    <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />   
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-table@1.23.2/dist/bootstrap-table.min.css"/>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap-table@1.23.2/dist/bootstrap-table.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/tableexport.jquery.plugin@1.29.0/tableExport.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/tableexport.jquery.plugin@1.29.0/libs/jsPDF/jspdf.umd.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap-table@1.23.2/dist/extensions/export/bootstrap-table-export.min.js"></script>
   
    <script src="../js/table_boo/table_boot_config.js"></script>
    <script src="../js/java_general/BootstrapTable.js"></script>
    <script  src="../Awesome/js/all.js"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet"/>
  <link href="../Awesome/css/brands.css" rel="stylesheet"/>
  <link href="../Awesome/css/solid.css" rel="stylesheet"/>
    <script  src="../Awesome/js/brands.js"></script>
  <script  src="../Awesome/js/solid.js"></script>
  <script  src="../Awesome/js/fontawesome.js"></script>
    
    <style type="text/css">
        .auto-style1 {
            height: 29px;
        }
    </style>
    <script  accesskey="javascript" type="text/javascript">
     

</script>
</head>
<body style="margin:0px">
    <form id="form1" runat="server" onkeypress="return caracter_especial(event,this)">
         <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeout="9900">
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
                elment_postbak = args.get_postBackElement();
                posicion_update_pogres('progres_bar');
                var elmen = document.getElementById(elment_postbak.id)
                
              
            }
            function CheckStatus(sender, args) {
                progres_hiden('progres_bar');
                //Button_Lista_Radicados Button_actualizar_guia Button_anular_guia
                var elmen = document.getElementById(elment_postbak.id)
                //ImageButtonCrearGrupoActividadUsuario
                if (elment_postbak.id == "TreeViewArchivo") {
                    //Insert_row("data_grid", "nombre", "fecha", "tipo", "id");
                    document.getElementById("Hidden0002").value = "1";
                    auto_zise_popup_lista_tareas("1");
                }
                if (elment_postbak.id == "Button_buscar_lista") {
                    busqueda_gred('hdnEmailID', 'data_grid', 'TextBox_busqueda', 'CheckBox_busqueda');
                }
                if (elment_postbak.id == "Button_notificar_envio") {
                    actuo_zise_popup_compartir_correo_electronico();
                }
                //Agregar documento a la carpeta  
                if (elment_postbak.id == "Button_agregar_archivo_expediente") {
                    document.getElementById("hdnEmailID").value="-1"
                    if (document.getElementById("Hidden0001").value != "") {
                        document.getElementById("hdnEmailID").value = "-1"
                        //Insert_row("data_grid", document.getElementById("TextBox_nombre_documento").value, document.getElementById("DropDownList_tipo_documento").value, "", document.getElementById("Hidden0001").value);
                    }
                   
                }
                if (elment_postbak.id == "Button_Editar_expediente_gestion") {
                    auto_zise_editar_expediente();
                }
                if (elment_postbak.id == "Button_guardar_desicion") {
                    if (document.getElementById("Hidden0001").value != "") {
                        document.getElementById("hdnEmailID").value = "-1"
                        //Insert_row("data_grid", document.getElementById("TextBox_nombre_documento").value, document.getElementById("DropDownList_tipo_documento").value, "", document.getElementById("Hidden0001").value);
                    }

                }
                //Editar documento
                if (elment_postbak.id == "Button_editar_archivo_expediente") {
                    if (document.getElementById("Hidden0001_edita").value != "") {
                        actualiza_gre_campo("data_grid", document.getElementById("hdnEmailID").value, document.getElementById("TextBox_nombre_documento_edita").value, "NOMBRE");
                        actualiza_gre_campo("data_grid", document.getElementById("hdnEmailID").value, document.getElementById("Hidden0002_edita").value, "TIPODOCUMENTAL");
                        document.getElementById("Hidden0001_edita").value = "";
                        document.getElementById("Hidden0002_edita").value = "";
                    }

                }
                //Eliminar documento 
                if (elment_postbak.id == "ButtonEliminar") {
                    if (document.getElementById("Hidden0004").value != "") {
                        eliminar_fila_data_gred_simple("data_grid", "hdnEmailID", "-1");
                        document.getElementById("Hidden0004").value = "";
                    }

                }
                //Agregar nivel oculto
                if (elment_postbak.id == "Button_muestra_nivel_oculto") {
                    if (document.getElementById("HiddenField_res_muestra_nivel").value != "") {
                        eliminar_fila_data_gred_simple_("GridView_lista_niveles_ocultos", "Hidden_lista_niveles_ocultos", "-1");
                        document.getElementById("HiddenField_res_muestra_nivel").value = "";
                    }

                }
                if (elment_postbak.id == "ButtonRadicar") {
                    auto_zise_popup_radicador();
                }
                //Eliminar expediente o nivel
                if (elment_postbak.id == "Button_eliminar_carpeta") {
                    if (document.getElementById("HiddenField_rest_0004").value != "") {
                        document.getElementById("HiddenField_rest_0004").value = "";
                        eliminar_node_tree_view_java("TreeViewArchivo", SELECCION_TREVIEW_ID, 0, 0);
                    }
                }
                if (elment_postbak.id == "Button_ocultar_nivel") {
                    if (document.getElementById("HiddenField_rest_0004").value != "") {
                        document.getElementById("HiddenField_rest_0004").value = "";
                        eliminar_node_tree_view_java("TreeViewArchivo", SELECCION_TREVIEW_ID, document.getElementById("HiddenField_numero_expediente").value, document.getElementById("HiddenField_numero_nivel").value);
                        document.getElementById("HiddenField_numero_expediente").value = 0;
                        document.getElementById("HiddenField_numero_nivel").value = 0;

                    }
                }
                //Agregar nivel  
                if (elment_postbak.id == "Button_agregar_nivel") {
                    if (document.getElementById("HiddenField_rest_0005").value !== "") {
                        document.getElementById("HiddenField_rest_0005").value = "";
                        create_trenode_treview(document.getElementById("Hidden_rest_tit_0006").value, document.getElementById("TextBox_nombre_nivel").value, "trenode", "TreeViewArchivo", document.getElementById("Hidden_rest_ur_0007").value,"0","");
                        
                    }
                }
                //Editar nivel
                if (elment_postbak.id == "Button_editar_nivel") {
                    if (document.getElementById("Hidden_res_edita_nivel_0001").value !== "") {
                        document.getElementById("Hidden_res_edita_nivel_0001").value = "";
                        actualiza_node_treview("", document.getElementById("TextBox_nombre_nivel_editar").value, "TreeViewArchivo", "");

                    }
                }
                //Editar expediente
                if (elment_postbak.id == "Button_agregar_expediente_actualizar") {
                    if (document.getElementById("Hidden_rest_result_agre_expe_tit_011").value !== "") {
                        document.getElementById("Hidden_rest_result_agre_expe_tit_011").value = "";
                        actualiza_node_treview("", document.getElementById("TextBox_nombre_expediente_carpeta_actualizar").value, "TreeViewArchivo", "");

                    }
                }
                //Agregar expediente   
                if (elment_postbak.id == "Button_agregar_expediente") {
                    if (document.getElementById("Hidden_rest_agrre_exp_0011").value !== "") {
                        document.getElementById("Hidden_rest_agrre_exp_0011").value = "";
                        create_trenode_treview(document.getElementById("Hidden_rest_expe_tit_0009").value, document.getElementById("TextBox_nombre_expediente_carpeta").value, "trenode", "TreeViewArchivo", document.getElementById("Hidden_rest_ur_expe_tit_0010").value, "1", document.getElementById("Hidden_parent_node_id").value);
                    }
                }
               
                
                if (elment_postbak.id == "Button_eliminar_regi_permiso") {
                    if (document.getElementById("Hidden_00_09").value != "-1") {
                        eliminar_fila_data_gred_simple_("data_grid_listado_permisos", "Hidden_sel", "-1");
                        document.getElementById("Hidden_00_09").value = "-1";
                        actualiza_node_treview("", "", "TreeViewArchivo", document.getElementById("Hidden_rest_ur_permiso_elimina_0007").value);
                        document.getElementById("Hidden_rest_ur_permiso_elimina_0007").value = "";
                    }

                }
                //Agrega registro digitalizado
                if (elment_postbak.id == "ButtonAlmacenar") {
                    if (document.getElementById("Hidden_001_inst_row").value !== "") {
                        insert_row_producion_documental(document.getElementById("Hidden_001_inst_row").value);
                        document.getElementById("Hidden_001_inst_row").value = "";
                        
                    }

                }
                //Agrega registro documento cargado  
                if (elment_postbak.id == "Button_guardar_desicion") {
                    if (document.getElementById("HiddenField_rest_des").value !== "") {
                        insert_row_producion_documental(document.getElementById("HiddenField_rest_des").value);
                        document.getElementById("HiddenField_rest_des").value = "";

                    }

                }
                //Agrega registro documento  
                if (elment_postbak.id == "Button_agregar_archivo_expediente") {
                    if (document.getElementById("HiddenField_rest_insert_row").value !== "") {
                        insert_row_producion_documental(document.getElementById("HiddenField_rest_insert_row").value);
                        document.getElementById("HiddenField_rest_insert_row").value = "";

                    }

                }
                //Button_nueva_carpeta  Asigna nodo padre para agregar carpeta con pocesión en carpeta
                if (elment_postbak.id == "Button_nueva_carpeta") {
                    if (document.getElementById("Hidden_parent_node_id").value !== "") {
                        document.getElementById("Hidden_parent_node_id").value = search_parent_nodo();
                    }

                }
                if (elment_postbak.id == "Button_indice_expediente") {
                    auto_zise_popup_indice_expediente();

                }
               
            }

            </script>
          <div id="div_error_content_rad" style="position: relative; width: 100%"></div>
         <nav id="menu_var" class="navbar navbar-expand-sm nav_botota_person_gray  modal_content_no_back_inferior"   > 
               <button id="nav_togle_display" class="navbar-toggler" type="button" style=" background-color:#6d7fcc" data-toggle="collapse" data-target="#navbarNavDropdown">
                   <span class="navbar-toggler-icon_"><i style="color:white" class="fad fa-th-list"></i></span>
               </button>
              
               <div class="collapse navbar-collapse row" id="navbarNavDropdown">  
                   <ul class="navbar-nav col-md-8"> 
                       <li class="nav-item dropdown active ml-2 active_">                  
                            <a class="nav-link dropdown-toggle bot_hover_person" style="color:#6d7fcc" href="#" id="navbarDropdownMenuLink" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"> <i style="color:#0062cc" class="fad fa-folders"></i> Expedientes
                            </a>
                           <div class="dropdown-menu " aria-labelledby="navbarDropdownMenuLink">
                               <a href="#" class="dropdown-item" onclick="activa_menu('n_c_e_007')"><i class="fas fa-folder-plus"></i> Agrega Expediente</a>
                               <a href="#" class="dropdown-item" onclick="activa_menu('n_n_o_009')"><i class="fal fa-folders"></i> Agrega Nivel</a>
                               <a href="#" class="dropdown-item" onclick="activa_menu('e_l_c_008')"><i class="fal fa-trash-alt"></i> Eliminar</a>
                               <a href="#" class="dropdown-item" onclick="activa_menu('a_c_e_009')"><i class="fal fa-pen-square"></i> Editar</a>
                               <a href="#" class="dropdown-item" onclick="activa_menu('e_l_x_008')"><i class="far fa-cut"></i> Cortar</a>
                               <a href="#" class="dropdown-item" onclick="activa_menu('e_l_p_008')"><i class="fal fa-paste"></i> Pegar</a>
                               <a href="#" class="dropdown-item" onclick="activa_menu('n_n_c_009')"><i class="fal fa-share"></i> Compartir</a>
                               <a href="#" class="dropdown-item" onclick="activa_menu('n_n_u_009')"><i class="fal fa-list-alt"></i> Listar permisos </a>
                               <a href="#" class="dropdown-item" onclick="activa_menu('n_l_n_010')"><i class="fad fa-broom"></i> Limpiar selección </a>
                               <div class="dropdown-submenu ">
                                   <a class="dropdown-item font-weight-light  dropdown-toggle" style="color: #6d7fcc" href="#" id="A6" data-toggle="dropdown" aria-haspopup="true" aria-expanded="true"><i style="color: #0062cc" class="fad fa-folder-tree"></i> Ubicación
                                   </a>
                                   <div class="dropdown-menu ">
                                       <a href="#" class="dropdown-item" onclick="activa_boton_client_server('Button_archivar_expediente_gestion')"><i class="fad fa-archive"></i> Archivar unidad documental (Expediente)</a>
                                       <a href="#" class="dropdown-item" onclick="activa_boton_client_server('Button_desachivar_expediente_gestion')"><i class="fal fa-archive"></i> Desarchivar unidad documental (Expediente)</a>
                                       <a href="#" class="dropdown-item" onclick="activa_boton_client_server('Button_ubicacio_expediente_gestion')"><i class="fal fa-folder-tree"></i> Ubicación unidad documental (Expediente)</a>
                                   </div>
                               </div>
                               <div class="dropdown-submenu ">
                                   <a class="dropdown-item font-weight-light  dropdown-toggle" style="color: #6d7fcc" href="#" id="A7" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc" class="fad fa-stamp"></i> Rotulo
                                   </a>
                                   <div class="dropdown-menu" >
                                       <a href="#" class="dropdown-item" onclick="activa_boton_client_server('ButtonRotulo')"><i class="fal fa-file-download"></i> Descargar rótulo unidad documental (Expediente)</a>
                                       <a href="#" class="dropdown-item" onclick="activa_boton_client_server('Button_rotulo_expediente_gestion')"><i class="fad fa-print"></i> Imprimir rótulo unidad documental (Expediente)</a>
                                       <a href="#" class="dropdown-item" onclick="activa_boton_client_server('Button_configura_rotulo')"><i class="fad fa-tools"></i> Configuración rótulo unidad documental (Expediente)</a>
                                   </div>
                               </div>
                                <div class="dropdown-submenu ">
                                    <a class="dropdown-item font-weight-light  dropdown-toggle" style="color: #6d7fcc" href="#" id="A8" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc" class="fal fa-cog"></i> Gestión de expediente
                                   </a>
                                    <div class="dropdown-menu" >
                                        <a href="#" class="dropdown-item" onclick="activa_boton_client_server('Button_Editar_expediente_gestion')"><i class="fal fa-edit"></i> Editar expediente</a>
                                        <a href="#" class="dropdown-item" onclick="activa_boton_client_server('Button_estado_expediente_gestion')"><i class="fal fa-folder"></i> Estado expediente</a> 
                                        <a href="#" class="dropdown-item" onclick="activa_boton_client_server('Button_general_indice_expediente')"><i class="fal fa-info-square"></i> Crear indice de expediente</a> 
                                        <a href="#" class="dropdown-item" onclick="activa_boton_client_server('Button_indice_expediente')"><i class="far fa-list-alt"></i> Mostrar indice de expediente</a>         
                                    </div>
                                </div>
                               <div class="dropdown-submenu ">
                                    <a class="dropdown-item font-weight-light  dropdown-toggle" style="color: #6d7fcc" href="#" id="A9" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc" class="fal fa-cog"></i> Gestión de niveles
                                   </a>
                                    <div class="dropdown-menu" >
                                        <a href="#" class="dropdown-item" onclick="activa_boton_client_server('Button_ocultar_nivel')"><i class="fal fa-minus-square"></i> Ocultar nivel</a>
                                        <a href="#" class="dropdown-item" onclick="activa_boton_client_server('Button_activa_listar_niveles_ocultos')"><i class="fal fa-list-ul"></i> Lista niveles ocultos</a> 
                                        
                                    </div>
                                </div>
                           </div>
                        </li>
                       <li class="nav-item dropdown active ml-2 mr-0 active_">
                           <a class="nav-link  dropdown-toggle" style="color: #6d7fcc" href="#" id="A3" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc" class="fal fa-arrow-from-bottom"></i> Cargar
                           </a>
                           <div class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">
                               <a href="#" class="dropdown-item" id="ma_load_file" ><i class="fal fa-arrow-from-bottom"></i> Archivo</a>
                               <a href="#" class="dropdown-item" onclick="activa_menu('a_d_d_004')"><i class="fal fa-scanner-image"></i> Archivo digitalizado</a>
                           </div>
                       </li> 
                        <li class="nav-item dropdown active ml-2 mr-0 active_">
                            <a class="nav-link  dropdown-toggle" style="color: #6d7fcc" href="#" id="A1" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc" class="fal fa-arrow-to-bottom"></i> Descargar
                           </a>
                            <div class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">
                                 <a href="#" class="dropdown-item" onclick="activa_menu('w_f_m_p_012')"> <i class="fal fa-arrow-to-bottom" ></i> Descargar Archivo seleccionado</a>
                           </div>
                        </li> 
                       <li class="nav-item dropdown active ml-2 mr-0 active_">
                            <a class="nav-link  dropdown-toggle" style="color: #6d7fcc" href="#" id="A2" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc" class="fal fa-copy"></i> Documentos
                           </a>
                            <div class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">
                                  <a href="#" class="dropdown-item" onclick="activa_menu('a_d_p_012')"><i class="fal fa-arrow-to-bottom" ></i>  Descargar archivo plantilla para elaboración de oficios </a>
                                  <a href="#" class="dropdown-item" style="display:none"> Descargar archivos del sistema integrado de gestión documental</a>
                           </div>
                        </li> 
                       <li class="nav-item dropdown active ml-2 mr-0 active_">
                            <a class="nav-link  dropdown-toggle" style="color: #6d7fcc" href="#" id="A4" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc" class="fad fa-share-square"></i> Compartir
                           </a>
                            <div class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">
                                 <a href="#"  class="dropdown-item" onclick="activa_menu('a_c_d_006')"> <i class="fal fa-share-square"></i> Compartir archivo  con usuarios internos</a>
                                 <a href="#"  class="dropdown-item" onclick="activa_menu('a_c_f_500')"> <i class="fal fa-file-signature"></i> Compartir archivo  para firma electrónica</a>
                                 <a href="#"  class="dropdown-item" onclick="activa_menu('c_c_e_012')"> <i class="fal fa-envelope"></i> Compartir archivo  por correo electrónico</a> 
                           </div>
                        </li> 
                        <li class="nav-item dropdown active ml-2 mr-0 active_">
                            <a class="nav-link  dropdown-toggle" style="color: #6d7fcc" href="#" id="A5" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc" class="fal fa-file"></i> Archivos
                           </a>
                            <div class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">
                                  <a href="#home" class="dropdown-item" title="Cambiar nombre archivo seleccionado"  onclick="activa_menu('c_n_a_005')"><i class="fal fa-file-edit"></i> Cambiar nombre</a> 
                                  <a href="#home" class="dropdown-item" title="Eliminar archivo seleccionado"   onclick="activa_menu('e_l_i_001')"><i class="fad fa-times"></i> Eliminar</a> 
                                  <a href="#home" class="dropdown-item" title="Radicar archivo seleccionado"  onclick="activa_menu('r_d_i_002')"> <i class="fal fa-file-check"></i> Radicar</a>
                                  <a href="#home" class="dropdown-item" title="Visualiza archivo seleccionado"  onclick="activa_menu('v_d_s_010')"> <i class="fal fa-file"></i> Visualizar</a>
                                  <a href="#home" class="dropdown-item" title="Copia archivo seleccionado"  onclick="activa_menu('c_a_s_010')"> <i class="fal fa-paste"></i> Copiar</a>
                                  <a href="#home" class="dropdown-item" title="Pegar archivo seleccionado"  onclick="activa_menu('p_a_s_010')"> <i class="fad fa-paste"></i> Pegar</a>
                                  <a href="#home" class="dropdown-item" title="Gestion meta dato archivo seleccionado"  onclick="activa_menu('m_g_m_011')"> <i class="fad fa-paste"></i> Meta datos</a>
                           </div>
                        </li>           
                   </ul>
                   <div class=" float-md-right col-md-4 float-sm-left">
                        <div class="input-group ">
                            <button id="td-boton" class="btn btn-outline-secondary border-right-2 " style="border-top-right-radius: 0px; border-bottom-right-radius: 0px" onclick="preven_event_restor_search(event,this)" type="button">
                                <i class="fal fa-long-arrow-left"></i>
                            </button>
                            <asp:TextBox ID="TextBox_buequeda_general" runat="server" class="form-control form-control-sm complex  border-left-0" placeholder="Busqueda...." onkeypress="preven_event_search_keypres_enter(event,this);"></asp:TextBox>
                            <div class="input-group-append">
                                <button class="btn btn-outline-secondary" onclick="preven_event_search(event,this)" type="button">
                                    <i class="fal fa-search"></i>
                                </button>
                            </div>
                        </div>
                    </div>     
               </div>
        </nav>    
        
        <asp:HiddenField ID="HiddenField_botones_respuesta" runat="server" Value="-1" />          
        <div id="conte_waper" class="container-fluid mr-0 ml-0 pl-0 pr-0" style="border-top: 1px solid #e9ecef">
            <a id="da_show-sidebar_" class="btn btn-sm   d-none " href="#" data-target="#sidebar_">
                <i style="color: white" class="fas fa-bars"></i>
            </a>
            <div id="da_content_wraper" class="wrapper_ ml-0 mr-0  d-flex  justify-content-between_" style="padding-left: 1px; padding-right: 1px">
                <div id="Contentizquierdo" class="bg-light_  " style="width:25%; float:left" >
                  <nav id="sidebar_" class=" bg-light_ pl-0 pr-0" >
                    <div id="title_treview" class="modal-header_ modal_title_superior_ border-right" style="border-top-left-radius: initial; border-top-right-radius: initial" >   
                        <div class="row">
                        <div class="col-10 nav-item_ active active_">
                             <a class="nav-link pr-2 pl-2" style="float:left; color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600" title="Agregar nivel" href="#" onclick="activa_menu('n_n_o_009')"><i style="" class="fal fa-folders"></i></a>            
                             <a class="nav-link pr-2 pl-2" style="float:left; color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600" title="Agregar expediente" href="#" onclick="activa_menu('n_c_e_007');"><i style="" class="fal fa-folder-plus"></i></a>  
                             <a class="nav-link  pr-2 pl-2" style="float:left; color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600" title="Eliminar elemento " href="#" onclick="activa_menu('e_l_c_008');"><i style="" class="fal fa-trash-alt"></i></a>
                           
                        </div>
                        <div class="col-2">
                             <a id="sidebarCollapse"   class="close_ mr-2 d-none" style=" float:right;  height:10px; color: #6d7fcc"> <i class="fal fa-times  fa-1x font-weight-light"></i></a>
                        </div>
                        </div>
                    </div>
                    <div id="bar_table" style=" height: auto; padding: 1px" class="border-right">
                        <div id="box_ide" class="box">
                            <div id="box_container" class="container-1_" style="background: white">
                                <asp:TextBox ID="TextBox_busqueda_tre" Style="width: 100%; padding-left: 5px; border-top-left-radius: initial; border-top-right-radius: initial; border-bottom-left-radius:initial; border-bottom-right-radius:initial; border:1px solid #dee2e6" CssClass="search_asp_rect_ form-control form-control-sm complex" runat="server" onkeypress="preven_event_search_keypres_enter_search(event,this);" placeholder="Busqueda.."></asp:TextBox>
                            </div>
                        </div>
                       
                    </div>
                    <div id="div_treview_archivo" style="width:100%" class="border-right">
                        <asp:Panel ID="Paneltreview" runat="server"
                            Height="100%" Width="99%" Style="position: inherit" >
                             <asp:TreeView ID="TreeViewArchivo" runat="server" CssClass="TreeN  nav-link" NodeWrap="true" Style=" overflow:auto; height:100%" EnableViewState="false" EnableClientScript="True" 
                                        PopulateNodesFromClient="False"
                                        LeafNodeStyle-CssClass="LeafNodeStyle"   Font-Size="14px" NodeIndent="10" ExpandDepth="0" CollapseImageUrl="../imagera/minus-square-light_1.png" ExpandImageUrl="../imagera/plus-square-light_1.png"  SkipLinkText="">
                                         <HoverNodeStyle Font-Underline="False" /> 
                                         <NodeStyle CssClass="nav-link_ mt-1 mb-1 pl-1  " ForeColor="black"
                                            VerticalPadding="0px" />         
                                        <SelectedNodeStyle ForeColor="Black"  CssClass="node_select_" VerticalPadding="5px" HorizontalPadding="5px" ImageUrl="../Gestion/imagenes/folder-open-regular.png" />
                                    </asp:TreeView>
                            <asp:UpdatePanel ID="UpdatePanelViewArchivo" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Button ID="Button_activa_busqueda_treview" runat="server" Text="Button" Style="display: none" />
                                    <input id="Hidden_texto_buequeda" type="hidden" value="" runat="server" />
                                    <asp:HiddenField ID="Hidden0002" runat="server" Value="1" />
                                    <asp:HiddenField ID="Hidden0003" runat="server" Value="" />
                                    <asp:HiddenField ID="Hidden0005" runat="server" Value="1" />
                                    <asp:HiddenField ID="Hidden0006" runat="server" Value="0" />
                                    <asp:HiddenField ID="Hidden0007" runat="server" Value="" />
                                    <asp:HiddenField ID="Hidden0008" runat="server" Value="0" />
                                </ContentTemplate>
                            </asp:UpdatePanel>

                        </asp:Panel>
                    </div>
                       <div id="contenido_pie" style="  border-top-left-radius: initial; border-top-right-radius: initial" class="modal-header pt-1 pb-1   justify-content-start  border">  
                           <h6 class="modal-title_ mt-2 mb-2 ml-2   font-weight-light" id="pit" style="color: #6d7fcc" ></h6>
                    </div>
                    <div id="Divbotnones_raiz" style="height: 10%; background-color: #E7EDF5; text-align: center; display: none">
                        <asp:UpdatePanel ID="UpdatePanel_botones_carpeta" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Button ID="Button_activa_listar_niveles_ocultos" runat="server" Text="" />
                                <asp:Button ID="Button_muestra_nivel_oculto" runat="server" Text="Button" />
                                <asp:Button ID="Button_ocultar_nivel" runat="server" Text=""  />
                                <asp:Button ID="Button_nueva_carpeta" runat="server" Text="Nueva" CssClass="boton_blanco" ToolTip="Agregar nueva carpeta o expediente" />
                                <asp:Button ID="Button_eliminar_carpeta" runat="server" Text="Eliminar" CssClass="boton_blanco" ToolTip="Eliminar carpeta o expediente" OnClientClick="pront_confirmacion('Desea eliminar el elemento seleccionado de la estructura ?');" />
                                <asp:Button ID="Button_activa_actualizar_carpeta" runat="server" Text="Editar" CssClass="boton_blanco" ToolTip="Editar carpeta o expediente" />
                                <asp:Button ID="Button_activa_nuevo_nivel" runat="server" Text="Nueva" CssClass="boton_blanco" />
                                <asp:Button ID="Button_activa_eliminar_nivel" runat="server" Text="Eliminar" CssClass="boton_blanco" OnClientClick="pront_confirmacion('Desea eliminar el nivel ?');" />
                                <asp:Button ID="Button_activa_lista_permiso_compartidos_nivel" runat="server" Text="Editar" CssClass="boton_blanco" />
                                <asp:Button ID="Button_activa_compartir_nivel" runat="server" Text="Editar" CssClass="boton_blanco"   />
                                <asp:Button ID="Button_actualiza" runat="server" Text="Button" Style="display: none" />
                                <asp:Button ID="Button_Restaura_busqueda" runat="server" Text="Button" Style="display: none" />
                                <asp:Button ID="Button_inline_trevie" runat="server" Text="Editar" CssClass="boton_blanco" />
                                <asp:HiddenField ID="HiddenField_0003" runat="server" Value="" />
                                <asp:HiddenField ID="HiddenField_rest_text_node" runat="server" Value="" />
                                <asp:HiddenField ID="HiddenField_rest_0004" runat="server" Value="" />
                                <asp:HiddenField ID="Hidden_rest_result_cut_expe_tet_01" runat="server" Value="" />
                                <asp:HiddenField ID="Hidden_rest_cut_expe_tit_00" Value="" runat="server" />
                                <asp:HiddenField ID="Hidden_rest_cut_ur_expe_01" Value="" runat="server" />
                                <asp:HiddenField ID="Hidden_rest_cut_expe_text_01" Value="" runat="server" />
                                <asp:HiddenField ID="Hidden_rest_cut_expe_id_01" Value="" runat="server" />
                                <asp:HiddenField ID="Hidden_rest_result_paste_expe_01" Value="" runat="server" />
                                <asp:HiddenField ID="Hidden_parent_node_id" Value="" runat="server" />               
                                <asp:HiddenField ID="HiddenField_numero_expediente" runat="server" Value="0"/>
                                <asp:HiddenField ID="HiddenField_numero_nivel" runat="server" Value="0" />
                                <asp:HiddenField ID="HiddenField_res_muestra_nivel" runat="server" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                  </nav>
                </div>
                <!--contenedor derecho-->
                <div id="Contenedorderecho" class="page-content mr-0 ml-0 pl-1 pr-1 pb-0 pt-0 "  style="width:75%; float:right"> 
                         <div id="titlte_gred_lista"   class=" p-2" >
                            <div class="row">
                            <div class="col-4">
                                 <asp:UpdatePanel ID="UpdatePanel_label_resultado" runat="server" UpdateMode="Conditional"  >
                                <ContentTemplate>
                                    <asp:Label ID="titulo_label_grid" runat="server" CssClass="h6 font-weight-light p-1"   Style=" float: left; color: #6d7fcc">Archivos</asp:Label>
                                    <div id="id_tag_spiner" style="display: none; margin-left: 20px"><i class="fas fa-spinner fa-spin"></i></div>        
                                    <asp:Button ID="Button_buscar_lista" runat="server" Width="65px" Text="Buscar" Style="float: right; font-size: 11px; display: none" />
                                    <asp:Button ID="Button_busca_general_archivo" runat="server" Width="65px" Text="Buscar" Style="float: right; font-size: 11px; display: none" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            </div>
                                <div class="col-8 justify-content-end nav-item_ active active_" >
                                     <a  id="a_exp_excel" class="p-2" style="float:right; color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600" title="Exportar lista documentos expediente" href="#" ><i style="" class="fal fa-file-chart-line"></i></a>
                                     <a  id="a_update_idnex_btach" class="p-2" style="float:right; color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600" title="Actualizar indices de documentos" href="#" ><i style="" class="fal fa-info"></i></a>
                                     <a class="p-2" style="float:right; color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600" title="Eliminar archivos seleccionados" href="#" onclick="activa_menu('e_l_i_001')"><i style="" class="fal fa-trash-alt"></i></a>   
                                     <a class="p-2" style="float:right; color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600" title="Pegar archivos seleccionados " href="#" onclick="activa_menu('p_a_s_010');"><i style="" class="far fa-paste"></i></a>
                                     <a class="p-2" style="float:right; color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600" title="Copiar archivos seleccionados" href="#" onclick="activa_menu('c_a_s_010');"><i style="" class="fal fa-copy"></i></a>        
                                     <a class="p-2" style="float:right; color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600" title="Descargar Archivos seleccionados " href="#" onclick="activa_menu('w_f_m_p_012');"><i style="" class="fal fa-arrow-to-bottom"></i></a>
                                     <a class="p-2" style="float:right; color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600" title="Cargar Archivos" href="#" id="a_load_file"><i style="" class="fal fa-arrow-from-bottom"></i></a> 
                                     <a class="p-2" style="float:right; color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600" title="Cargar archivos digitalizados" href="#" onclick="activa_menu('a_d_d_004');"><i style="" class="fal fa-scanner-image"></i></a>                    
                                     <a id="file_externo_copy" class="p-2" style="float:right; color:red; font-family: Arial; text-decoration: none; font-weight: 600; display:none" title="Pegar archivos externos" href="#" onclick="activa_menu('c_a_exte_011');"><i style="" class="far fa-paste"></i></a>  
                                </div>
                           
                          </div>
                        </div>                                              
                        <asp:Panel ID="Panel_unidad_treview_unidad" runat="server"
                             Style=" overflow: auto; width:100%" CssClass=" border-top_ ">               
                                <asp:UpdatePanel ID="UpdateGeneral_documentos" runat="server" UpdateMode="Conditional" EnableViewState="false" ViewStateMode="Disabled">
                                    <ContentTemplate>  
                                            <asp:GridView ID="data_grid" runat="server" AllowSorting="false" PageSize="14" PagerSettings-Position="Top"
                                                EnableViewState="false" Style="font-family: Segoe UI;  font-weight:300; font-size:1em"
                                                AutoGenerateSelectButton="False" CssClass="table  " GridLines="None" 
                                                ViewStateMode="Disabled">
                                                <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                                <HeaderStyle CssClass="GridviewScrollHeader_line_boot" BorderStyle="None" />
                                                <RowStyle CssClass="" />
                                                <PagerStyle CssClass="pagination-ys" />
                                                <Columns>
                                                    <asp:TemplateField>
                                                        <HeaderTemplate>
                                                            <asp:Panel ID="Panel_che_box_aling" runat="server" Style="text-align: center">
                                                                <asp:CheckBox ID="chk_selec" CssClass="btn   btn-light btn-sm border-0 bg-transparent" runat="server" onclick="desactiva_activa_chek(this,'data_grid');" />
                                                            </asp:Panel>
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="chkSelection" runat="server" Class="dummychkstyle" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>               
                                        <div style="display:none">
                                             <asp:Button ID="Button_actualiza_table" runat="server" Text="Cargar" CssClass="boton" Style="display: none" />
                                             <input id="Hidden_nureg" type="hidden" value="" runat="server" />
                                             <input id="HiddenEstado" type="hidden" value="1" runat="server"/>                  
                                        </div>                                       
                                    </ContentTemplate>
                                </asp:UpdatePanel>                        
                        </asp:Panel>
                    <div id="foter_estado" style="height: 40px" class="p-1">
                        <input id="hdnEmailID_sel" type="hidden" value="0" runat="server"/>
                        <asp:Label ID="Label_estado" runat="server" Text="" CssClass="h6 font-weight-light p-1" Style="float: right; margin-right: 3px; color: #6d7fcc"></asp:Label>
                        <asp:Label ID="Label_title_selecion" runat="server" CssClass="h6 font-weight-light p-1" Style="margin-left: 1px; float: right; color: #6d7fcc"></asp:Label>
                    </div>                                  
                    <div id="contendor_botones_unidad" style="height: 10%; background-color: #E7EDF5; border-color: #b0c4de; border-width: 1px; border-style: ridge; display: none">
                        <asp:UpdatePanel ID="UpdatePanel_botones_unidad" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                
                                <iframe runat="server" id="ifmExcel_reporte_" width="0" height="0" marginheight="0" marginwidth="0"
                                frameborder="0" />
                                <asp:Button ID="Button_Activa_Agregar_archivo" runat="server" Text="Cargar" CssClass="boton" Style="margin-top: 3px; margin-left: 10px" ToolTip="Cargar archivo a la carpeta o expediente seleccionado" OnClientClick="eliminar_ajaxtolkit();" />                       
                                <asp:Button ID="Button_Digitaliza" runat="server" Text="Digitaliza" CssClass="boton" ToolTip="Digitaliza y djunta archivo a la carpeta expediente seleccionada" />
                                <asp:Button ID="ButtonButtonEditar" runat="server" Text="Cambiar nombre" CssClass="boton" ToolTip="Cambia nombre o tipo documental del archivo seleccionado" />
                                <asp:Button ID="ButtonDescarga" runat="server" Text="Descarga" CssClass="boton" ToolTip="Descarga el archivo seleccionado" />
                                <asp:Button ID="ButtonVerDocumento" runat="server" Text="Visualiza" CssClass="boton" ToolTip="Visualiza el documento seleccionado" />
                                <asp:Button ID="ButtonReubicar" runat="server" Text="Reubicar" Style="display: none" CssClass="boton" ToolTip="Reubica en otra carpeta el archivo seleccionado" />
                                <asp:Button ID="ButtonRadicar" runat="server" Text="Radicar" CssClass="boton" ToolTip="Radica los archivos seleccionados" OnClientClick="asigna_id_seleccionados_cheked();" />
                                <asp:Button ID="ButtonArchivar" runat="server" Text="Archivar" CssClass="boton" Style="width: 70px; display: none" />
                                <asp:Button ID="Button_activa_compartir_documento" runat="server" Text="Compartir" CssClass="boton" Style="width: 75px" OnClientClick="asigna_id_seleccionados_cheked()" />
                                <asp:Button ID="Button_activa_descarga_dcoumento_plantila" runat="server" Text="Compartir" CssClass="boton" Style="width: 0px; display: none" />
                                <asp:Button ID="Button_notificar_envio" runat="server" Text="Compartir" CssClass="boton" Style="width: 0px; display: none" />           
                                <asp:Button ID="Button_copia_update" runat="server" Width="65px" Text="Buscar" Style="float: right; font-size: 11px; display: none" />
                                <asp:Button ID="Button_activa_ubicacion_archivo" runat="server" Text="" CssClass="boton" Style="width: 0px; display: none" />
                                <asp:Button ID="Button_indice_expediente" runat="server" Text="" CssClass="boton" Style="width: 0px; display: none" />
                                <asp:Button ID="Button_activa_gestion_meta_dato" runat="server" Text="" CssClass="boton" Style="width: 0px; display: none" />
                                <asp:Button ID="Button1" runat="server" Text="Prueba" CssClass="boton" Style="width: 0px; display: none" />
                                <asp:HiddenField ID="Hidden_id_unidad" runat="server" Value="0" />
                                <asp:HiddenField ID="Hidden0004" runat="server" Value="" />
                                <asp:HiddenField ID="HiddenField_empresa" runat="server" Value="0" />
                                <asp:HiddenField ID="HiddenField_estado_ubicacion" runat="server" Value="" />
                                 <input id="Hidden_colum_header_reporte" type="hidden" value="-1" runat="server"/>
                                <input id="hdnEmailID" type="hidden" value="-1" runat="server"/>
                                <input id="Hidden_sele_docu" type="hidden" value="" runat="server"/>
                                <input id="Hidden_ruta_archivo" type="hidden" value="" runat="server"/>
                                <input id="Hidden_gabinete" type="hidden" value="" runat="server"/>
                                <input id="hdnEmailID_VAL" type="hidden" value="" runat="server"/>  
                                <asp:Button ID="Button_actualiza_add_archivo" runat="server" Text="Button" Style="display: none" />
                                <input id="HiddenRuta" type="hidden" value="0" runat="server"/>
                                <input id="HiddenIdFlujo" type="hidden" value="0" runat="server"/>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
        </div>
        <div id="botones" style="display: none">
            <asp:UpdatePanel ID="UpdatePanel_botones_comandos" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Button ID="ButtonAlmacenar" runat="server" Text="Button" />
                    <input id="Hidden_001_inst_row" type="hidden" value="" runat="server"/>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel ID="UpdatePanel_botones_opcion" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Button ID="Button_archivar_expediente_gestion" runat="server" Text="" Style="width: 0px; height: 0px" />
                    <asp:Button ID="Button_Editar_expediente_gestion" runat="server" Text="Editar " ToolTip="Editar unidad documental" OnClientClick="tamano_ventana_editar_expediente()" CssClass="boton_azul_normal" />
                    <asp:Button ID="Button_desachivar_expediente_gestion" runat="server" Text="Desarrchivar " ToolTip="Desarrchivar unidad documental" OnClientClick="pront_confirmacion('Desea desachivar ?');" CssClass="boton_azul_normal" />
                    <asp:Button ID="Button_ubicacio_expediente_gestion" runat="server" Text="Ubicación " ToolTip="Muestra Ubicación unidad documental" CssClass="boton_azul_normal" />
                    <asp:Button ID="ButtonRotulo" runat="server" Text="Descarga"  ToolTip="Descarga rotulo unidad documental" CssClass="boton_azul_normal" />
                    <asp:Button ID="Button_rotulo_expediente_gestion" runat="server" Text="Imprimir"   Style="width: 0px; height: 0px"/>
                    <asp:Button ID="Button_configura_rotulo" runat="server" Text="Configura rotulo "  Style="width: 0px; height: 0px" />
                    <asp:Button ID="Button_estado_expediente_gestion" runat="server" Text="Estado " Style="width: 0px; height: 0px" />
                    <asp:Button ID="Button_general_indice_expediente" runat="server" Text=""  Style="width: 0px; height: 0px"/>
                    <asp:Button ID="ButtonExportaListaExpediente" runat="server" Text="" Style="width: 0px; height: 0px" />
                   
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
         <asp:Panel ID="Panel_lista_niveles_ocultos" runat="server" Style="display:none; width: 70%; height: 100%" CssClass="modal_content_general_">
            <asp:ModalPopupExtender ID="ModalPopupExtender_edition_lista_niveles_ocultos" runat="server"
                TargetControlID="ButtonSalir_lista_niveles_ocultos" BackgroundCssClass="FondoAplicacion"
                CancelControlID="Button_cerrar_lista_niveles_ocultos" PopupControlID="Panel_lista_niveles_ocultos">
            </asp:ModalPopupExtender>
            <div id="modal_content_lista_niveles_ocultos" class="modal-content">
                <div id="diver_cabcera_lista_niveles_ocultos" class="modal_title_superior_ modal-header">
                    <h6 class="modal-title d-inline ">Registros</h6>
                    <button type="button" value="Button_cerrar_lista_niveles_ocultos" class="close da_event_captive ">&times;</button>
                </div>
                <div id="contenido_procesa_lista_niveles_ocultos" style="background-color: white; width: 100%; height: 100%; border-top: none" class="modal_content_back modal-body">
                    <asp:UpdatePanel ID="Update_lista_niveles_ocultos" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>      
                            <div id="contenido_titulo_lista_niveles_ocultos" class="mb-2">  
                                <input id="Hidden_lista_niveles_ocultos" type="hidden" value="-1" runat="server"/>          
                                <asp:Label ID="titulo_label_lista_niveles_ocultos" runat="server" class="h6 font-weight-light">Resultados busqueda</asp:Label>
                            </div>
                            <div id="content_data_grid_lista_niveles_ocultos" class="conten_gred_border_" style="overflow: auto; width: 100%">
                                <asp:GridView ID="GridView_lista_niveles_ocultos" runat="server"  Style="position: inherit; width: 100%; font-size: 14px"
                                    AutoGenerateSelectButton="False" AllowSorting="true"  AllowPaging="true" PageSize  ="6" PagerSettings-Position="Top"  CssClass="table  font-weight-light" GridLines="None"
                                     EnableViewState="true">
                                    <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                    <HeaderStyle CssClass="GridviewScrollHeader_line_boot" />
                                    <PagerStyle CssClass="pagination-ys" />
                                    <Columns>
                                         <asp:BoundField HeaderText="OPCIONES"   />
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </ContentTemplate>

                        <Triggers>
                        </Triggers>
                    </asp:UpdatePanel>

                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button_lista_niveles_ocultos" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                        <asp:Button ID="ButtonSalir_lista_niveles_ocultos" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                        <asp:Button ID="Button_cerrar_lista_niveles_ocultos" runat="Server" Text="X" CssClass="invisible" />
                    </div>
                </div>
              
            </div>
        </asp:Panel>
          <asp:Panel ID="Panel_cambia_estado_expediente_popup" runat="server" Style="display:none; width:40%; height:auto; overflow:auto" CssClass="modal_content_general">               
                 <asp:ModalPopupExtender ID="ModalPopupExtender_cambia_estado_expediente_popup" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_cambia_estado_expediente_popup"
                    PopupControlID="Panel_cambia_estado_expediente_popup" CancelControlID="Buttoncerrar_cambia_estado_expediente_popup"></asp:ModalPopupExtender>
                <div id="modal_content_cambia_estado_expediente_popup" class="modal-content">
                    <div id="divcabecer_cambia_estado_expediente_popup" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title d-inline ml-1">Cambia estado unidad documental</h6>
                         <button type="button" value="Buttoncerrar_cambia_estado_expediente_popup" class="close da_event_captive ">&times;</button>   
                    </div>
                    <div id="Contenido_cambia_estado_expediente_popup" style=" height: auto; width: auto; border-top:none; overflow:auto" class="modal_content_back pl-3 pr-3">
                        <asp:UpdatePanel ID="UpdatePanel_cambia_estado_expediente_popup" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:MutuallyExclusiveCheckBoxExtender ID="MutuallyExclusive_anexo_radicado" runat="server" TargetControlID="Check_ButtonAbierto"
                                    Key="radicado"></asp:MutuallyExclusiveCheckBoxExtender>
                                <asp:MutuallyExclusiveCheckBoxExtender ID="MutuallyExclusive_relacionado_radicado" runat="server" TargetControlID="CheckBox_ButtonSerrado"
                                    Key="radicado"></asp:MutuallyExclusiveCheckBoxExtender>
                                <div class="row mt-4 pl-2">
                                    <asp:CheckBox ID="Check_ButtonAbierto" runat="server" Text="" Checked="false"   CssClass="h7 custom-checkbox mr-1" />
                                    <span>Unidad documental en estado abierto</span>
                                </div>
                                <div class="row mt-1 pl-2">
                                    <asp:CheckBox ID="CheckBox_ButtonSerrado" runat="server" Text="" Checked="false" CssClass="h7 custom-checkbox mr-1" />
                                    <span>Unidad documental en estado cerrado</span>
                                </div>
                                 <div class="row mt-4 pl-2">
                                 <span>Motivo del cambio</span>
                                 </div>
                                <div class="row mt-1 mb-3 pl-2">
                                    <asp:TextBox ID="TextBox_cambia_estado_exp_popup" TextMode="MultiLine" runat="server" Style="width: 98%" CssClass="form-control"></asp:TextBox>
                                </div>
                                
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="Button_estado_expediente_gestion" EventName="Click" />
                            </Triggers>
                        </asp:UpdatePanel>         
                    </div>
                     <div id="contedor_botones_cambia_estado_expediente_popup" class="modal-footer ">
                            <asp:UpdatePanel ID="updatepanel_botones_cambia_estado_expediente_popup" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Button ID="Button_actualiza_estado_expediente_popup" runat="server" Text="Cambiar" ToolTip="Cambia estado unidad documental"  CssClass="btn btn-success"  />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button_cambia_estado_expediente_popup" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" Style="display: none" />
                        <asp:Button ID="ButtonSalir_cambia_estado_expediente_popup" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" Style="display: none" />
                        <asp:Button ID="Buttoncerrar_cambia_estado_expediente_popup" runat="Server" Text="X" CssClass="invisible" Height="1px" Width="1px" Style="display: none" />
                    </div>
                </div>
            </asp:Panel>
            <asp:Panel ID="Panel_gestion_meta_datos" runat="server" Style="display:none;  width:auto; height: auto" CssClass="modal_content_general_">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_gestion_meta_datos"  runat="server" BackgroundCssClass="FondoAplicacion"  TargetControlID="ButtonSalir_gestion_meta_datos" 
                    CancelControlID="Button_cerrar_gestion_meta_datos" PopupControlID="Panel_gestion_meta_datos" ></asp:ModalPopupExtender>
                <div class="modal-content" id="modal_content_modal_meta_data">
                    <div id="divcabecer2_gestion_meta_datos_" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title"> Gestión meta datos</h6>
                        <button type="button" onclick="activa_boton_client_server('Button_cerrar_gestion_meta_datos');" class="close">&times;</button>
                    </div>
                    <div id="contenido_procesa_gestion_meta_datos" style="background-color: white; width: auto; height: auto; color: black; background-color: #FFFFFF; border-top: none" class="modal_content_back modal-body">
                        <asp:UpdatePanel ID="UpdatePanel_gestion_meta_datos_up" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div style="text-align: center">
                                    <asp:DropDownList ID="DropDownList_meta_datos" runat="server" CssClass="mt-1 custom-select"></asp:DropDownList>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div style="display: none; height: 1px">
                            <asp:Button ID="Button_cerrar_gestion_meta_datos" runat="Server" Text="X"
                                ToolTip="Cerrar ventana" />
                            <asp:Button ID="Button_gestion_meta_datos" runat="server" Text="Button" Height="1px" Width="1px" />
                            <asp:Button ID="ButtonSalir_gestion_meta_datos" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                        </div>
                    </div>
                    <div class="modal-footer" id="content_boton_gestion_meta_datos">
                        <asp:UpdatePanel ID="UpdatePanel_gestion_meta_datos" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Button ID="Button_gestion_meta_dato" runat="server" Text="Aceptar" Style="float: right" CssClass="btn btn-success" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </asp:Panel>
            <!--gestion meta data-->
       
            <asp:Panel ID="Panel_gestion_meta_data_archivo" runat="server" Style="display: none; width: 50%; height: 100%">
                <asp:ModalPopupExtender ID="ModalPopupExtende_gestion_meta_data_archivo" runat="Server" BackgroundCssClass="FondoAplicacion" 
                     TargetControlID="ButtonSalir_gestion_meta_data_archivo"
                    PopupControlID="Panel_gestion_meta_data_archivo" CancelControlID="Buttoncerrar_gestion_meta_data_archivo" >
                </asp:ModalPopupExtender>
                <div class="modal-content" id="modal_content_gestion_meta_data_archivo">
                    <div id="divcabecer_gestion_meta_data_archivo" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title d-inline ml-1">Gestión meta dato</h6>
                        <button type="button" value="Buttoncerrar_gestion_meta_data_archivo" class="close da_event_captive">&times;</button>
                    </div>
                    <div id="Contenido_gestion_meta_data_archivo" style="color: black; background-color: #FFFFFF; height: 100%; width: 100%" class="modal_content_general_">
                        <asp:UpdatePanel ID="UpdatePanel_gestion_meta_data_archivo" runat="server" UpdateMode="Conditional" style="height: 100%" RenderMode="Inline">
                            <ContentTemplate>
                                <iframe id="Iframe_gestion_meta_data_archivo_" runat="server" style="width: 100%; height: 100%; padding-top: 5px" frameborder="0"></iframe>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button_gestion_meta_data_archivo" Style="display: none" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="ButtonSalir_gestion_meta_data_archivo" Style="display: none" runat="server" Text="" Height="0px" Width="0px" />
                        <asp:Button ID="Buttoncerrar_gestion_meta_data_archivo" runat="Server" Text="" Style="display: none" />
                    </div>
                </div>
            </asp:Panel>
        
         <!--configura_plantilla_rotulo-->
          <div id="configura_plantilla_rotulo">
            <asp:Panel ID="Panel_configura_plantilla_rotulo" runat="server" Style="display:none; width:40%; height:auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_configura_plantilla_rotulo" runat="server" BehaviorID="Panel_configura_plantilla_rotulo" TargetControlID="ButtonSalir_configura_plantilla_rotulo" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_configura_plantilla_rotulo" PopupControlID="Panel_configura_plantilla_rotulo" ></asp:ModalPopupExtender>
                <div id="modal_content_configura_plantilla_rotulo" class="modal-content">
                    <div id="divcabecer2_configura_plantilla_rotulo" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title d-inline ml-1">Seleccionar plantilla</h6>
                        <button type="button" value="Button_cerrar_configura_plantilla_rotulo" class="close da_event_captive">&times;</button>  
                    </div>
                    <div id="contenido_procesa_configura_plantilla_rotulo" style="border-top:none; overflow:auto" class="modal_content_back pl-3 pr-3">
                        <asp:UpdatePanel ID="UpdatePanel_configura_plantilla_rotulo" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div style="text-align: center">
                                    <br />
                                    <span></span>                           
                                    <br />
                                    <asp:DropDownList ID="DropDownList_configura_plantilla_rotulo" runat="server" CssClass="mt-1 custom-select"></asp:DropDownList>
                                    <br />
                                    <br />
                                    <asp:Button ID="Button_aceptar_configura_plantilla_rotulo" runat="server" Text="Aceptar" CssClass="btn btn-success mb-4" />
                                 
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                    </div>
                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button_configura_plantilla_rotulo" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                        <asp:Button ID="ButtonSalir_configura_plantilla_rotulo" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                        <asp:Button ID="Button_cerrar_configura_plantilla_rotulo" runat="Server" Text="X" CssClass="invisible" />
                    </div>
                </div>
            </asp:Panel>
        </div>
         <!--detalle indice-->
	               <asp:Panel ID="Panel_indice" runat="server" Style="display:none; overflow:hidden; width:95%; height:100%" CssClass="modal_content_general" >
	                      <asp:ModalPopupExtender ID="ModalPopupExtender_indice" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_indice_dos"
	                          PopupControlID="Panel_indice"  CancelControlID="ButtonSalir_indice">
	                      </asp:ModalPopupExtender>
	                      <div id="Cabecerapendiente_indice" class="modal_title_superior" >                     
	                         <button style="margin-right:10px" type="button" onclick="activa_boton_client_server('ButtonSalir_indice');" class="close">&times;</button>          
	                      </div>
	                      <div id="Cotenedorpendiente_indice" style="height: 90%; width: 100%; overflow:hidden" class="modal_content_back">                     
	                          <asp:UpdatePanel ID="UpdatePanel_indice" runat="server" UpdateMode="Conditional">
	                              <ContentTemplate>
	                                  <iframe id="Iframe_indice_" runat="server" frameborder="0"  style="width:100%; height:100%; overflow:hidden"></iframe>
	                              </ContentTemplate>
	                          </asp:UpdatePanel>           
	                      </div>
                       <div style="display: none; height: 1px">
                           <asp:Button ID="ButtonSalir_indice" runat="Server" Text=""  CssClass="invisible bg-transparent" />
	                       <asp:Button ID="Button_indice_dos" CssClass="invisible bg-transparent" runat="server" Text="" Height="1px" Width="1px" />
                       </div>           
              </asp:Panel>
            <!--Termina mensaje_personalizado-->
        <div id="Impresion_post">
            <asp:Panel ID="Panelimpresionpost" runat="server" Style="display: none; width: auto; height: auto" CssClass="modal_content_general">
                <asp:DragPanelExtender ID="DragPanelExtenderimpre_post" runat="server" TargetControlID="Panelimpresionpost" />
                <asp:ModalPopupExtender ID="ModalPopupExtenderimpre_post" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_post"
                    PopupControlID="Panelimpresionpost" CancelControlID="Buttoncerrarimpre_post">
                </asp:ModalPopupExtender>
                <div id="modal_content_impre_post" class="modal-content">
                    <div id="divcabecer2_post" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title d-inline ml-1">Menú Impresión</h6>
                         <button type="button" value="Buttoncerrarimpre_post" class="close da_event_captive">&times;</button>   
                    </div>
                    <asp:UpdatePanel ID="UpdatePaneliframe_post" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div id="ContenidoImpresion_post" style="height: auto; width: auto ; border-top:none; overflow:auto" class="modal_content_back  pl-3 pr-3">
                                <iframe width="100%" height="100%" id="ifimpre_post_" runat="server" frameborder="0"></iframe>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button1_post" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" Style="display: none" />
                        <asp:Button ID="ButtonSalir_post" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" Style="display: none" />
                        <asp:Button ID="Buttoncerrarimpre_post" runat="Server" Text="X" CssClass="invisible" Style="display: none" />
                    </div>
                </div>
            </asp:Panel>
        </div>
     <!--archiva expediente-->
        <div id="modal_reubicar_unidad_expediente">
            <asp:Panel ID="Panel_reubicar_unidad_expediente_popup" runat="server"  Style="display:none; width: 100%; height: 99%" CssClass="modal_content_general">             
                <asp:ModalPopupExtender ID="ModalPopupExtende_reubicar_unidad_expediente_popup" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_reubicar_unidad_expediente_popup"
                    PopupControlID="Panel_reubicar_unidad_expediente_popup" CancelControlID="Buttoncerrar_reubicar_unidad_expediente_popup" ></asp:ModalPopupExtender>
                <div id="modal_content_reubicar_unidad_expediente_popup" class="modal-content_">
                    <div id="divcabecer_reubicar_unidad_expediente_popup" class="modal_title_superior_ modal-header">
                         <h6 class="modal-title d-inline ml-1">Archivar unidad documental</h6>
                         <button type="button" value="Buttoncerrar_reubicar_unidad_expediente_popup" class="close da_event_captive">&times;</button>       
                    </div>
                    <div id="Contenido_reubicar_unidad_expediente_popup" style=" height: 97%; width: 100%; border-top:none" class="modal_content_back pl-1 pr-1 pt-1">
                        <div id="drowlist_r_u_e" style="">
                            <asp:UpdatePanel ID="UpdatePanelEntidad_r_u_e" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:DropDownList ID="DropDownListEntidadEmpresa_r_u_e" runat="server" Width="100%" CssClass="custom-select" onchange="buton_click('Button_listar_edificio');"></asp:DropDownList>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                         <div id="div_treview_archivo_r_u_e" style="height: 80%; margin-top: 5px">
                            <asp:Panel ID="Paneltreview_r_u_e" runat="server" ScrollBars="Both"
                                Height="100%" Width="100%" Style="position: inherit">
                                <asp:UpdatePanel ID="UpdatePanelViewArchivo_r_u_e" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:TreeView ID="TreeViewArchivo_r_u_e" Style="text-align: left; padding-left: 1px; font-size: 10px; margin-top: 0px" runat="server" CssClass="TreeN" NodeWrap="true"
                                            PopulateNodesFromClient="False" EnableViewState="true"
                                            LeafNodeStyle-CssClass="LeafNodeStyle" Font-Size="11px" NodeIndent="10" ExpandDepth="0" SkipLinkText="">
                                            <HoverNodeStyle Font-Underline="False" />
                                            <LeafNodeStyle CssClass="LeafNodeStyle" HorizontalPadding="10px" NodeSpacing="0px" VerticalPadding="5px" />
                                            <NodeStyle ChildNodesPadding="5px" HorizontalPadding="0px" NodeSpacing="5px" VerticalPadding="5px" ForeColor="Black" />
                                            <ParentNodeStyle ChildNodesPadding="0px" ForeColor="#313131" Font-Bold="true" HorizontalPadding="5px" NodeSpacing="0px" VerticalPadding="5px" />
                                            <RootNodeStyle ChildNodesPadding="0px" ForeColor="#313131" Font-Bold="true" NodeSpacing="0px" VerticalPadding="5px" HorizontalPadding="5px" />
                                            <SelectedNodeStyle ForeColor="White" CssClass="node_select_" Font-Size="10px" ImageUrl="../workflow/imageneswf/iten_list_select.png" />
                                        </asp:TreeView>
                                    </ContentTemplate>
                                </asp:UpdatePanel>

                            </asp:Panel>
                        </div>         
                    </div>
                    <div id="contendor_botones_unidad_r_u_e" style="border-top:none" class="border_inferior_radius_blanco_ modal-footer">
                            <asp:UpdatePanel ID="UpdatePanel_botones_unidad_r_u_e" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Button ID="Button_actualizar_unidad" runat="server" Text="Editar" CssClass="btn btn-success" Style="display: none" />
                                    <asp:Button ID="Button_agrega_unidad_conservacion_interface" runat="server" Text="Archivar" ToolTip="Archivar unidad documental en unidad contenedora" CssClass="btn btn-success" Style=" display: none" />
                                    <asp:Button ID="Button_archivar" runat="server" Text="Archivar" CssClass="btn btn-success" Style="margin-left: 10px" />
                                    <asp:Button ID="Button_agrega_unidad_contenedora" runat="server" Text="Agregar" ToolTip="Agregar una unidad contenedora de unidad documental" CssClass="btn btn-success" Style=" margin-left: 10px" />
                                    <asp:Button ID="Button_editar_unidad_contenedora" runat="server" Text="Editar" CssClass="btn btn-success" ToolTip="Editar unidad contendora de unidad documental" />
                                    <asp:Button ID="ButtonEliminar_unidad_contendora" runat="server" Text="Eliminar" ToolTip="Eliminar unidad contendora de unidad documental" CssClass="btn btn-success" OnClientClick="ConfirmMensajeGeneral('Desea eliminar la unidad contenedora ?','Hidden_result_eliminar');" />
                                    <input id="Hidden_result_eliminar" type="hidden" value="0" runat="server"/>
                                    <input id="Hidden3" type="hidden" value="0" runat="server"/>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                </div>
                <div style="display: none; height: 1px">
                    <asp:Button ID="Button_reubicar_unidad_expediente_popup" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                    <asp:Button ID="ButtonSalir_reubicar_unidad_expediente_popup" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                    <asp:Button ID="Buttoncerrar_reubicar_unidad_expediente_popup" runat="Server" Text="" CssClass="invisible" />
                </div>
            </asp:Panel>
            
        </div>
         <!--agregar unidad de conservacion-->
        <div id="modal_agregar_unidad_conservacion">  
                <asp:Panel ID="Panel_agregar_unidad_conservacion_popup" runat="server"  Style="display:none; color: White; width:50%; height: 99%; margin:auto" >                
                    <asp:ModalPopupExtender ID="ModalPopupExtende_agregar_unidad_conservacion_popup" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_agregar_unidad_conservacion_popup"
                        PopupControlID="Panel_agregar_unidad_conservacion_popup" CancelControlID="Buttoncerrar_agregar_unidad_conservacion_popup" Y="1">
                    </asp:ModalPopupExtender>
                    <div id="divcabecer_agregar_unidad_conservacion_popup" class="modal_title_superior" style="display:none">                        
                        <asp:Label ID="Label_agregar_unidad_conservacion_popup" runat="server" Text="Gestión unidad contendora" Font-Size="10" Style="float: left; display:none">
                        </asp:Label>
                        <div id="Divcerrarbuton_agregar_unidad_conservacion_popup" style="float: right">
                            <asp:Button ID="Buttoncerrar_agregar_unidad_conservacion_popup" runat="Server" Text="X" Style="display: none"
                                ToolTip="Cerrar ventana" />
                        </div>
                    </div>  
                     <div id="Contenido_agregar_unidad_conservacion_popup" style="color: black; background-color: #FFFFFF; height: 100%; width: 100%" class="modal_content_general">
                         <asp:UpdatePanel ID="UpdatePanel_agregar_unidad_conservacion_popup" runat="server" UpdateMode="Conditional" style="height:100%" RenderMode="Inline">
                             <ContentTemplate>
                            <iframe  id="Iframe_agregar_unidad_conservacion_popup"  runat="server"  style="width:100%; height:100%;padding-top:5px" frameborder="0"></iframe>                
                                 <asp:HiddenField ID="Hidden_tipo_unidad_seleccion" runat="server" value="0"/>
                                 </ContentTemplate>
                             </asp:UpdatePanel>
                        </div>  
                        <asp:Button ID="Button_agregar_unidad_conservacion_popup" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                        <asp:Button ID="ButtonSalir_agregar_unidad_conservacion_popup" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                </asp:Panel>      
        </div>  
        <div id="modal_agregar_expediente">
            <asp:Panel ID="Panel_agregar_expdiente_popup" runat="server" Style="display: none; width: 50%; height: 98%; overflow: hidden; margin: auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtende_agregar_expdiente_popup" runat="Server" BackgroundCssClass="FondoAplicacion" Y="1" TargetControlID="ButtonSalir_agregar_expdiente_popup"
                    PopupControlID="Panel_agregar_expdiente_popup" CancelControlID="Buttoncerrar_agregar_expdiente_popup">
                </asp:ModalPopupExtender>
                <div id="divcabecer_agregar_expdiente_popup" class="modal_title_superior" style="width: 99%; display: none">
                    <asp:Label ID="Label_agregar_expdiente_popup" runat="server" Text="Gestión expedientes" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton_agregar_expdiente_popup" style="float: right">
                        <asp:Button ID="Buttoncerrar_agregar_expdiente_popup" runat="Server" Text="X" CssClass="modal_boton_hiden"
                            ToolTip="Cerrar ventana"  />

                    </div>
                </div>
                <div id="Contenido_agregar_expdiente_popup" style="background-color: transparent; height: auto; width: auto; overflow: hidden" class="modal_content_back">
                    <asp:UpdatePanel ID="UpdatePanel_agregar_expdiente_popup" runat="server" UpdateMode="Conditional" style="height: 100%" RenderMode="Inline">
                        <ContentTemplate>
                            <iframe id="Iframe_agregar_expdiente_popup_" runat="server" style="width: 100%; height: 100%; overflow: hidden" scrolling="no" frameborder="0"></iframe>
                            <input id="Hidden_estado_editar" type="hidden" value="YES" runat="server"/>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <asp:Button ID="Button_agregar_expdiente_popup" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" Style="display: none" />
                <asp:Button ID="ButtonSalir_agregar_expdiente_popup" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" Style="display: none" />
            </asp:Panel>
            
        </div> 
          <!--ubicacion toponimica-->
        <div id="modal_ubicacion_toponimica_expediente">
            <asp:Panel ID="Panel_ubicacion_toponimica_expediente_popup" runat="server" Style="display:none;  width: 50%; height: 99%">
                <asp:ModalPopupExtender ID="ModalPopupExtende_ubicacion_toponimica_expediente_popup" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_ubicacion_toponimica_expediente_popup"
                    PopupControlID="Panel_ubicacion_toponimica_expediente_popup" CancelControlID="Buttoncerrar_ubicacion_toponimica_expediente_popup" Y="0"></asp:ModalPopupExtender>
                <div id="modal_content_ubicacion_toponimica_expediente_popup" class="modal-content">
                    <div id="divcabecer_ubicacion_toponimica_expediente_popup" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title d-inline ml-1">Ubicación topografica</h6>
                        <button type="button" value="Buttoncerrar_ubicacion_toponimica_expediente_popup" class="close da_event_captive">&times;</button>
                    </div>
                    <div id="Contenido_ubicacion_toponimica_expediente" style="height: 97%; width: 100%; border-top: none; overflow: auto" class="modal_content_back pl-1 pr-1">
                        <div id="div_treview_archivo_u_b_t" style="height: 100%">
                            <asp:Panel ID="Paneltreview_u_b_t" runat="server" ScrollBars="Both"
                                Height="100%" Width="100%" Style="position: inherit">
                                <asp:UpdatePanel ID="UpdatePanelViewArchivo_u_b_t" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:TreeView ID="TreeViewArchivo_u_b_t" Style="text-align: left; padding-left: 1px; font-size: 10px; margin-top: 0px" runat="server" CssClass="TreeN" NodeWrap="true"
                                            PopulateNodesFromClient="False" EnableViewState="true"
                                            LeafNodeStyle-CssClass="LeafNodeStyle" Font-Size="11px" NodeIndent="10" ExpandDepth="0" SkipLinkText="">
                                            <HoverNodeStyle Font-Underline="False" />
                                            <LeafNodeStyle CssClass="LeafNodeStyle" HorizontalPadding="10px" NodeSpacing="0px" VerticalPadding="5px" />
                                            <NodeStyle ChildNodesPadding="5px" HorizontalPadding="0px" NodeSpacing="5px" VerticalPadding="5px" ForeColor="Black" />
                                            <ParentNodeStyle ChildNodesPadding="0px" ForeColor="#313131" Font-Bold="true" HorizontalPadding="5px" NodeSpacing="0px" VerticalPadding="5px" />
                                            <RootNodeStyle ChildNodesPadding="0px" ForeColor="#313131" Font-Bold="true" NodeSpacing="0px" VerticalPadding="5px" HorizontalPadding="5px" />
                                            <SelectedNodeStyle ForeColor="White" CssClass="node_select_" Font-Size="10px" ImageUrl="../workflow/imageneswf/iten_list_select.png" />
                                        </asp:TreeView>
                                    </ContentTemplate>
                                </asp:UpdatePanel>

                            </asp:Panel>
                        </div>

                    </div>
                    <div id="contendor_botones_unidad_u_b_t" class="border_inferior_radius_blanco modal-footer">
                        <asp:UpdatePanel ID="UpdatePanel_botones_unidad_u_b_t" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Button ID="Button_exportar" runat="server" Text="Exportar" CssClass="btn btn-success" OnClientClick="fnExcelTre('TreeViewArchivo_u_b_t')" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button_ubicacion_toponimica_expediente_popup" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" Style="display: none" />
                        <asp:Button ID="ButtonSalir_ubicacion_toponimica_expediente_popup" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" Style="display: none" />
                        <asp:Button ID="Buttoncerrar_ubicacion_toponimica_expediente_popup" runat="Server" Text="" CssClass="invisible" />
                    </div>
                </div>
            </asp:Panel>
        </div>  
  <!--agregar_expediente_carpeta-->
          <div id="agregar_expediente_carpeta">
            <asp:Panel ID="Panel_agregar_expediente_carpeta" runat="server" Style="display:none;  width:60%; height: auto " CssClass="modal_content_general_  ">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_agregar_expediente_carpeta" runat="server" BehaviorID="Panel_agregar_expediente_carpeta" TargetControlID="ButtonSalir_agregar_expediente_carpeta" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_agregar_expediente_carpeta" PopupControlID="Panel_agregar_expediente_carpeta" ></asp:ModalPopupExtender>
                <div class="modal-content" id="content_add_expediente">
                    <div id="divcabecer2_agregar_expediente_carpeta" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title ">Agregar expediente</h6>
                        <button type="button" value="Button_cerrar_agregar_expediente_carpeta" class="close da_event_captive ">&times;</button>
                    </div>
                    <div id="contenido_procesa_agregar_expediente_carpeta" style="background-color: white; width:auto; height: 99%; color: black; overflow:auto; border-top:none" class="modal_content_back modal-body">
                        <asp:UpdatePanel ID="UpdatePanel_agregar_expediente_carpeta" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row mt-2">
                                    <div class="col-12 d-flex justify-content-center">
                                        <asp:Label ID="Label2" runat="server" Text="Descripción del expediente" CssClass="h6"></asp:Label>
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <div class="col-6 d-flex">
                                        <asp:Label ID="Label_nombre_carpeta" CssClass="h6 font-weight-light" runat="server" Text="Nombre del expediente * "></asp:Label>
                                    </div>
                                    <div class="col-6 d-flex">
                                        <asp:TextBox ID="TextBox_nombre_expediente_carpeta" runat="server" CssClass="form-control" MaxLength="70"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <div class="col-6 d-flex">
                                        <asp:Label ID="Label4" CssClass="h6 font-weight-light" runat="server" Text="Nombre del solicitante "></asp:Label>
                                    </div>
                                    <div class="col-6 d-flex">
                                        <asp:TextBox ID="TextBox_nombre_persona_expediente" runat="server" CssClass="form-control" MaxLength="40"></asp:TextBox>
                                    </div>
                                </div>
                                 <div class="row mt-2">
                                    <div class="col-6 d-flex">
                                        <asp:Label ID="Label5" CssClass="h6 font-weight-light" runat="server" Text="Nit/identificación del solicitante "></asp:Label>
                                    </div>
                                    <div class="col-6 d-flex">
                                        <asp:TextBox ID="TextBox_identificacion_persona_expediente" runat="server" CssClass="form-control" MaxLength="60"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <div class="col-6 d-flex">
                                        <asp:Label ID="Label6" CssClass="h6 font-weight-light" runat="server" Text="Asunto "></asp:Label>
                                    </div>
                                    <div class="col-6 d-flex">
                                        <asp:TextBox ID="TextBox_asunto_expediente" runat="server" CssClass="form-control" ></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <div class="col-6 d-flex">
                                        <asp:Label ID="Label7" CssClass="h6 font-weight-light" runat="server" Text="Tema "></asp:Label>
                                    </div>
                                    <div class="col-6 d-flex">
                                        <asp:TextBox ID="TextBox_tema_expediente" runat="server" CssClass="form-control" ></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <div class="col-6 d-flex">
                                        <asp:Label ID="Label8" CssClass="h6 font-weight-light" runat="server" Text="Observación "></asp:Label>
                                    </div>
                                    <div class="col-6 d-flex">
                                        <asp:TextBox ID="TextBox_observacion_expediente" runat="server" CssClass="form-control" ></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <div class="col-12 d-flex justify-content-center">
                                        <asp:Label ID="Label1" runat="server" Text="Clasificación del expediente" CssClass="h6"></asp:Label>
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <div class="col-6 d-flex">
                                        <asp:Label ID="Label_instrumento" runat="server" Text="Instrumento archivístico" CssClass="h6 font-weight-light"></asp:Label>
                                    </div>
                                    <div class="col-6 d-flex">
                                        <asp:DropDownList ID="DropDownList_instrumento" runat="server"  CssClass="form-control" AutoPostBack="True"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <div class="col-6 d-flex">
                                        <asp:Label ID="Label_serie_documental" runat="server" Text="Serie o asunto" CssClass="h6 font-weight-light"></asp:Label>
                                    </div>
                                    <div class="col-6 d-flex">
                                        <asp:DropDownList ID="DropDownList_serie_documental" runat="server" CssClass="form-control" AutoPostBack="True"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <div class="col-6 d-flex">
                                        <asp:Label ID="Label_sub_serie_documental" runat="server" Text="Sub serie o sub asunto" CssClass="h6 font-weight-light"></asp:Label>
                                    </div>
                                    <div class="col-6 d-flex">
                                        <asp:DropDownList ID="DropDownList_sub_serie_asunto" runat="server" CssClass="form-control"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <div class="col-6 d-flex">
                                        <asp:Label ID="Label_fondo_documental" runat="server" Text="Fondo documental" CssClass="h6 font-weight-light"></asp:Label>
                                    </div>
                                    <div class="col-6 d-flex">
                                        <asp:DropDownList ID="DropDownList_fondo" runat="server" CssClass="form-control"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <div class="col-6 d-flex">
                                        <asp:Label ID="Label_gabiente_prod" runat="server" Text="Gabinete almacenamiento" CssClass="h6 font-weight-light"></asp:Label>
                                    </div>
                                    <div class="col-6 d-flex">
                                        <asp:DropDownList ID="DropDownList_gabinete_producion" runat="server" CssClass="form-control"></asp:DropDownList>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div style="display: none; height: 1px">
                            <asp:Button ID="Button_cerrar_agregar_expediente_carpeta" runat="Server" Text="X" CssClass="invisible" />
                            <asp:Button ID="Button_agregar_expediente_carpeta" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                            <asp:Button ID="ButtonSalir_agregar_expediente_carpeta" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                        </div>
                    </div>
                    <div class="modal-footer  justify-content-between" id="content_boton_add_expediente">
                        <div class="modal-title d-inline ">
                            <asp:CheckBox ID="CheckBox_hide_form" runat="server"  Checked="true"/>
                            <label class="" for="flexCheckChecked">
                                Mantiene ventana abierta
                            </label>
                        </div>
                        <asp:UpdatePanel ID="UpdatePanel_boton_add_expediente" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Button ID="Button_agregar_expediente" runat="server" Text="Aceptar" CssClass="btn btn-success" />
                                <input id="Hidden_rest_expe_tit_0009" type="hidden" value="" runat="server" />
                                <input id="Hidden_rest_ur_expe_tit_0010" type="hidden" value="" runat="server" />
                                <input id="Hidden_rest_agrre_exp_0011" type="hidden" value="" runat="server" />
                            </ContentTemplate>
                        </asp:UpdatePanel>

                    </div>
                </div>                 
            </asp:Panel>
        </div>
       <!--actualizar_expediente_carpeta-->
            <asp:Panel ID="Panel_actualizar_expediente_carpeta" runat="server" Style="display:none; width:60%; height:auto">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_actualizar_expediente_carpeta" runat="server"  TargetControlID="ButtonSalir_actualizar_expediente_carpeta" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_actualizar_expediente_carpeta" PopupControlID="Panel_actualizar_expediente_carpeta" ></asp:ModalPopupExtender>
                <div id="modal_content_edita_expediente" class="modal-content">
                    <div id="divcabecer2_actualizar_expediente_carpeta" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title">Actualizar expediente</h6>
                        <button type="button" value="Button_cerrar_actualizar_expediente_carpeta" class="close da_event_captive">&times;</button>
                    </div>
                    <div id="contenido_procesa_actualizar_expediente_carpeta" style="background-color: white; width: auto; height: 99%; border-top: none; overflow:auto " class="modal_content_back modal-body">
                        <asp:UpdatePanel ID="UpdatePanel_actualizar_expediente_carpeta" runat="server" UpdateMode="Conditional"   >
                            <ContentTemplate>
                                 <div class="row mt-2">
                                    <div class="col-12 d-flex justify-content-center">
                                        <asp:Label ID="Label9" runat="server" Text="Descripción del expediente" CssClass="h6"></asp:Label>
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <div class="col-6 d-flex">
                                        <asp:Label ID="Label_nombre_carpeta_actualizar" runat="server" Text="Nombre del expediente *" CssClass="h6 font-weight-light"></asp:Label>
                                    </div>
                                    <div class="col-6 d-flex">
                                        <asp:TextBox ID="TextBox_nombre_expediente_carpeta_actualizar" runat="server"  MaxLength="70" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <div class="col-6 d-flex">
                                        <asp:Label ID="Label10" CssClass="h6 font-weight-light" runat="server" Text="Nombre del solicitante "></asp:Label>
                                    </div>
                                    <div class="col-6 d-flex">
                                        <asp:TextBox ID="TextBox_nombre_persona_expediente_actualizar" runat="server" CssClass="form-control" MaxLength="40"></asp:TextBox>
                                    </div>
                                </div>
                                 <div class="row mt-2">
                                    <div class="col-6 d-flex">
                                        <asp:Label ID="Label11" CssClass="h6 font-weight-light" runat="server" Text="Nit/identificación del solicitante "></asp:Label>
                                    </div>
                                    <div class="col-6 d-flex">
                                        <asp:TextBox ID="TextBox_identificacion_persona_expediente_actualizar" runat="server" CssClass="form-control" MaxLength="60"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <div class="col-6 d-flex">
                                        <asp:Label ID="Label12" CssClass="h6 font-weight-light" runat="server" Text="Asunto "></asp:Label>
                                    </div>
                                    <div class="col-6 d-flex">
                                        <asp:TextBox ID="TextBox_asunto_expediente_actualizar" runat="server" CssClass="form-control" ></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <div class="col-6 d-flex">
                                        <asp:Label ID="Label13" CssClass="h6 font-weight-light" runat="server" Text="Tema "></asp:Label>
                                    </div>
                                    <div class="col-6 d-flex">
                                        <asp:TextBox ID="TextBox_tema_expediente_actualizar" runat="server" CssClass="form-control" ></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <div class="col-6 d-flex">
                                        <asp:Label ID="Label14" CssClass="h6 font-weight-light" runat="server" Text="Observación "></asp:Label>
                                    </div>
                                    <div class="col-6 d-flex">
                                        <asp:TextBox ID="TextBox_observacion_expediente_actualizar" runat="server" CssClass="form-control" ></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <div class="col-12 d-flex justify-content-center">
                                        <asp:Label ID="Label_actualizar" runat="server" Text="Clasificación del expediente" CssClass="h6"></asp:Label>
                                    </div>

                                </div>

                                <div class="row mt-2">
                                    <div class="col-6 d-flex">
                                        <asp:Label ID="Label_instrumento_edita" runat="server" Text="Instrumento archivístico" CssClass="h6 font-weight-light"></asp:Label>
                                    </div>
                                    <div class="col-6 d-flex">
                                        <asp:DropDownList ID="DropDownList_instrumento_edita" runat="server"  AutoPostBack="True" CssClass="form-control"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <div class="col-6 d-flex">
                                        <asp:Label ID="Label_serie_documental_actualizar" runat="server" Text="Serie o asunto" CssClass="h6 font-weight-light"></asp:Label>
                                    </div>
                                    <div class="col-6 d-flex">
                                        <asp:DropDownList ID="DropDownList_serie_documental_actualizar" runat="server"  AutoPostBack="True" CssClass="form-control"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <div class="col-6 d-flex">
                                        <asp:Label ID="Label_sub_serie_documental_actualizar" runat="server" Text="Sub serie o sub asunto" CssClass="h6 font-weight-light"></asp:Label>
                                    </div>
                                    <div class="col-6 d-flex">
                                        <asp:DropDownList ID="DropDownList_sub_serie_asunto_actualizar" runat="server"  CssClass="form-control"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <div class="col-6 d-flex">
                                        <asp:Label ID="Label_fondo_documental_actualizar" runat="server" Text="Fondo documental " CssClass="h6 font-weight-light"></asp:Label>
                                    </div>
                                    <div class="col-6 d-flex">
                                        <asp:DropDownList ID="DropDownList_fondo_actualizar" runat="server"  CssClass="form-control"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <div class="col-6 d-flex">
                                        <asp:Label ID="Label_gabiente_prod_edit" runat="server" Text="Gabinete almacenamiento" CssClass="h6 font-weight-light"></asp:Label>
                                    </div>
                                    <div class="col-6 d-flex">
                                        <asp:DropDownList ID="DropDownList_gabinete_producion_edit" runat="server"  CssClass="form-control"></asp:DropDownList>
                                    </div>
                                </div>

                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div style="display: none; height: 1px">
                            <asp:Button ID="Button_actualizar_expediente_carpeta" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" Style="display: none" />
                            <asp:Button ID="ButtonSalir_actualizar_expediente_carpeta" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" Style="display: none" />
                            <asp:Button ID="Button_cerrar_actualizar_expediente_carpeta" runat="Server" Text="X" CssClass="invisible" Height="1px" Width="1px" Style="display: none" />
                        </div>
                    </div>
                    <div class="modal-footer justify-content-end" id="modal-footer_edita_expediente">
                        <asp:UpdatePanel ID="UpdatePanel_botones_actualiza_expediente" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <input id="Hidden_rest_result_agre_expe_tit_011" type="hidden" value="" runat="server"/>
                                <input id="Hidden_rest_edit_agre_expe_tit_009" type="hidden" value="" runat="server"/>
                                <input id="Hidden_rest_edit_agre_ur_expe_tit_010" type="hidden" value="" runat="server"/>
                                <asp:Button ID="Button_agregar_expediente_actualizar" runat="server" Text="Aceptar"  CssClass="btn btn-success" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
                 
            </asp:Panel>
       
      <!--agrega_documento_carpeta_expediente-->
          <div id="agrega_documento_carpeta_expediente" style="clear:both">
            <asp:Panel ID="Panel_agrega_documento_carpeta_expediente" runat="server" Style="display:none;  width: 50%; height:auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_agrega_documento_carpeta_expediente" runat="server" BehaviorID="Panel_agrega_documento_carpeta_expediente" TargetControlID="ButtonSalir_agrega_documento_carpeta_expediente" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_agrega_documento_carpeta_expediente" PopupControlID="Panel_agrega_documento_carpeta_expediente" ></asp:ModalPopupExtender>
                <div id="divcabecer2_agrega_documento_carpeta_expediente"  class="modal_title_superior">
                     <h6 class="modal-title d-inline ml-1">Adjuntar documento a expediente</h6>
                     <button type="button" value="Button_cerrar_agrega_documento_carpeta_expediente" class="close da_event_captive mr-2">&times;</button>               
                </div>
                <div id="contenido_procesa_agrega_documento_carpeta_expediente" style="background-color: white; width: 100%; height: 100%" class="modal_content_back  p-2">
                    <asp:UpdatePanel ID="UpdatePanel_agrega_documento_carpeta_expediente" runat="server" UpdateMode="Conditional" class="p-2">
                        <ContentTemplate>
                            <div class="col-11 col-sm-10 d-flex d-none">
                                <asp:TextBox ID="TextBox_nombre_archivo" runat="server" Style="width: 100%; display:none" MaxLength="40" ReadOnly="true"></asp:TextBox>
                                <asp:TextBox ID="TextBox_ruta_archivo" runat="server" Style="width: 550px; font-size: 9px; display: none" MaxLength="40" ReadOnly="true"></asp:TextBox>
                            </div>
                            <asp:DropDownList ID="DropDownList_tipo_documento" runat="server" Style="width: 100%"></asp:DropDownList></td>
                                       <asp:HiddenField ID="Hidden0001" runat="server" Value="" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <asp:Panel ID="Panel_descarga_ajax" runat="server">
                        <div id="drop_zone_" style="width: 100%; height: auto; overflow: auto">
                            <asp:UpdatePanel ID="UpdatePanel_descarga" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:AjaxFileUpload ID="AjaxFileUpload_dowload" runat="server" ThrobberID="drop_zone_"
                                        ContextKeys="fred"
                                        AllowedFileTypes="tif,jpg,tiff,bmp,pdf" Mode="Auto"
                                        MaximumNumberOfFiles="1" OnClientUploadComplete="activa_boton_dowload" />
                                    &nbsp  
                                    <div style="overflow: auto">
                                        <asp:Label ID="Label_estado_carga" runat="server" Text="Estado" Style="font-family: Arial; font-size: 10px"></asp:Label>
                                    </div>
                                    <asp:Button ID="Button_guardar_desicion" runat="server" Text="Button" Style="display: none" />
                                    <asp:HiddenField ID="HiddenField_rest_des" runat="server" Value="" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </asp:Panel>
                    <div class="row justify-content-end p-3" >
                        <asp:HiddenField ID="HiddenField_rest_insert_row" runat="server" Value="" />
                    </div>
                    <div style="display: none">
                        <asp:Label ID="Label_nombre_documento" runat="server" Text="Nombre documento" Style="font-family: Arial; font-size: 12px; display: none"></asp:Label>
                        <asp:TextBox ID="TextBox_nombre_documento" runat="server" Style="width: 240px; display: none" MaxLength="40"></asp:TextBox>
                    </div>
                </div>             
                <div style="display: none; height: 1px">
                    <asp:Button ID="Button_agrega_documento_carpeta_expediente" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                    <asp:Button ID="ButtonSalir_agrega_documento_carpeta_expediente" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                    <asp:Button ID="Button_cerrar_agrega_documento_carpeta_expediente" runat="Server" Text="" CssClass="invisible" />
                </div>
            </asp:Panel>
        </div>
        <!--edita_documento_carpeta_expediente-->
          <div id="edita_documento_carpeta_expediente">
            <asp:Panel ID="Panel_edita_documento_carpeta_expediente" runat="server" Style="display:none; width: 50%; height: auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_edita_documento_carpeta_expediente" runat="server" BehaviorID="Panel_edita_documento_carpeta_expediente" TargetControlID="ButtonSalir_edita_documento_carpeta_expediente" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_edita_documento_carpeta_expediente" PopupControlID="Panel_edita_documento_carpeta_expediente" ></asp:ModalPopupExtender>
                <div class="modal-content">
                    <div id="divcabecer2_edita_documento_carpeta_expediente" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title">Editar caracterización documento</h6>
                        <button type="button" value="Button_cerrar_edita_documento_carpeta_expediente" class="close da_event_captive">&times;</button>
                    </div>
                    <div id="contenido_procesa_edita_documento_carpeta_expediente" style="background-color: white; color: black; background-color: #FFFFFF; border-top: none" class="modal_content_back modal-body">
                        <asp:UpdatePanel ID="UpdatePanel_edita_documento_carpeta_expediente" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row">
                                    <div class="col-6">
                                        <asp:Label ID="Label_nombre_documento_edita" runat="server" Text="Nombre documento" CssClass=" h6 font-weight-light"></asp:Label>
                                    </div>
                                    <div class="col-6">
                                        <asp:TextBox ID="TextBox_nombre_documento_edita" runat="server" Style="" CssClass="form-control " MaxLength="40"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <div class="col-6">
                                        <asp:Label ID="Label_tipo_edita" runat="server" Text="Tipo documento" CssClass="h6 font-weight-light"></asp:Label>
                                    </div>
                                    <div class="col-6 d-flex">
                                        <asp:DropDownList ID="DropDownList_tipo_documento_edita" runat="server" Style="min-width: 150px" AppendDataBoundItems="true" EnableViewState="true"></asp:DropDownList></td>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div style="display: none; height: 1px">          
                            <asp:Button ID="Button_edita_documento_carpeta_expediente" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" Style="display: none" />
                            <asp:Button ID="ButtonSalir_edita_documento_carpeta_expediente" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" Style="display: none" />
                            <asp:Button ID="Button_cerrar_edita_documento_carpeta_expediente" runat="Server" Text="" CssClass="invisible" Style="display: none" />
                        </div>

                    </div>
                    <div class="modal-footer">
                        <button type="button" value="Button_cerrar_edita_documento_carpeta_expediente" class="btn btn-light da_event_captive mr-2">Cancelar</button>
                        <asp:UpdatePanel ID="UpdatePanel_edita_archivo_boton" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>       
                                <asp:Button ID="Button_editar_archivo_expediente" runat="server" Text="Aceptar" Style="margin-right: 5px" CssClass="btn  btn-success" />
                                <asp:HiddenField ID="Hidden0001_edita" runat="server" Value="" />
                                <asp:HiddenField ID="Hidden0002_edita" runat="server" Value="" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    

                </div>
              
            </asp:Panel>
        </div>
        
           <!--cargar documento anexo!-->
          <div id="contenido_procesa_sube_documento_adjunto" >
            <asp:Panel ID="Panel_sube_documento_adjunto" runat="server" Style="display:none;  width: 70%; height: 100%" CssClass="modal_content_general_">
                <asp:ModalPopupExtender ID="ModalPopupExtender_sube_documento_adjunto" runat="Server" BackgroundCssClass="FondoAplicacion" 
                    TargetControlID="Button_sube_documento_adjunto"
                    PopupControlID="Panel_sube_documento_adjunto" CancelControlID="Button3_cerrar_adjunta" ></asp:ModalPopupExtender>
                <div id="modal_content_sube_documento_adjunto" class="modal-content">  
                    <div id="Div_cabecera" class="modal_title_superior_ modal-header"> 
                        <h6 class="modal-title d-inline ml-1">Adjuntar</h6>  
                        <button type="button" value="Button3_cerrar_adjunta_" onclick="hide_upload_content('ModalPopupExtender_sube_documento_adjunto');" class="close da_event_captive_ ">&times;</button>                   
                   </div>            
                    <div id="Div_contenido_adjunta" style="height: auto; width: 100%; border-top: none" class="modal_content_back p-2">
                        <div id="content_option_chek_adjunto_doc_visor"> 
                        <asp:UpdatePanel ID="Update_actualiza_adjunta_documento" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                
                                <div id="content_data_grid_adjunta_documento" class="conten_gred_border_" style="width: 100%">
                                    <asp:DropDownList ID="DropDownList_adjunta_documento" Style="width: 100%" CssClass="custom-select mr-sm-2" runat="server"></asp:DropDownList>
                                </div>
                            </ContentTemplate>
                            <Triggers>
                            </Triggers>
                        </asp:UpdatePanel>
                        </div>
                        <div class="p-2">
                            <div class="row p-2" id="content_boton_adjunto_doc_visor">
                                <div class="col-12 p-0">
                                    <div class="file-select " id="src-file">
                                        <input id="file_element_adjunto_doc_visor" type="file" multiple="multiple"  accept=""  style="width: 100px; height: 40px" name="src-file" class="p-1" contente_file="ModalPopupExtender_sube_documento_adjunto" aria-label="Archivo"  />
                                    </div>
                                    <a id="save_file_element_adjunto_doc_visor" title="Guardar todos los archivos"  class="btn  btn-success" style="opacity: 0"><i style="color: white" class="fas fa-save "></i> Guardar </a>   
                                    <a id="delete_file_element_adjunto_doc_visor" title="Elminar todos los archivos cargados"  class="btn  btn-danger " style="opacity: 0"><i style="color: white" class="fal fa-trash-alt "></i> Eliminar </a>
                                    <a id="cancel_file_element_adjunto_doc_visor" title="Cancelar guardar archivos"  class="btn  btn-warning" style="opacity: 0"><i style="color: white" class="fas fa-window-close "></i> Cancelar </a>
                                </div>
                            </div>            
                            <div class="paren_element background_upload" id="conten_file_element_adjunto_doc_visor" style="overflow: auto; height: 100%">
                                
                                  <div id="content_drop_element_adjunto_doc_visor" claas="">
                                       
                                     
                                 </div>
                                 <table id="table_file_element_adjunto_doc_visor" class="table table-striped">
                                 </table>
                            </div>
                            <div class="row border pt-2" id="content_pie_title_adjunto_doc_visor">
                                <div class="col-8">
                                    <div class="row p-2">
                                        <div class="col-4 p-0">
                                            <div >
                                                <asp:Label ID="Label_progres_bar_file_element_adjunto_doc_visor" runat="server" Text="" Style="font-family: Arial; text-align: center; font-size: 20px"></asp:Label>
                                            </div>
                                            <div id="pogres_file_element_contador_adjunto_doc_visor" style="text-align: center; font-family: Arial; font-size: 14px">
                                            </div>
                                            <div id="pogres_file_element_porcent_adjunto_doc_visor" style="text-align: center; font-family: Arial; font-size: 14px">
                                            </div>
                                        </div>
                                        <div class="col-5 p-0">
                                            <div>
                                                <div id="myProgress_file_element_adjunto_doc_visor">
                                                    <div id="myBar_file_element_adjunto_doc_visor" class="file-select-bar"></div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-3 p-0 pl-3">
                                            <p id="count_byte_file_element_adjunto_doc_visor"></p>    
                                        </div>
                                    </div>
                                    
                                </div>
                                <div class="col-4 justify-content-end pt-2">
                                    <p id="count_file_element_adjunto_doc_visor" class="font-weight-light" style="float: right"> Estado </p>
                                </div>
                            </div>
                        </div>
                       <div style="display: none; height: 1px">
                            <asp:Button ID="Button_sube_documento_adjunto" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                            <asp:Button ID="Button3_cerrar_adjunta" runat="Server" Text="" CssClass="invisible" />
                        </div>

                    </div>
                </div>
            </asp:Panel>     
        </div>
        <!--Popup visor externo-->
               <asp:Panel ID="Panel_visor_externo" runat="server" Style="display:none; overflow:hidden" ForeColor="White" Width="98%" Height="100% " CssClass="modal_content_general">
                  <asp:ModalPopupExtender ID="ModalPopupExtender_visor_externo" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_visor_externo"
                      PopupControlID="Panel_visor_externo"  CancelControlID="ButtonSalir_visor_externo"></asp:ModalPopupExtender>
                  <div id="Cabecerapendiente_visor_externo" class="modal_title_superior">        
                       <button type="button" value="ButtonSalir_visor_externo" class="close da_event_captive mr-2">&times;</button>                     
                  </div>
                  <div id="Cotenedorpendiente_visor_externo" style="color: Black; background-color: #FFFFFF; height: 90%; width: 100%; overflow:hidden" class="modal_content_back">                 
                      <asp:UpdatePanel ID="UpdatePanel_visor_externo" runat="server" UpdateMode="Conditional">
                          <ContentTemplate>
                              <iframe id="Iframe_visor_externo_da_" runat="server" frameborder="0"  style="width:100%; height:100%; overflow:hidden"></iframe>
                          </ContentTemplate>
                      </asp:UpdatePanel>
                            <asp:Button ID="Button_visor_externo" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" style="display:none" />
                             <asp:Button ID="ButtonSalir_visor_externo" runat="Server" Text="" CssClass="invisible" 
                              Height="1px" Width="1px" style="display:none"  />
                  </div>
                  
              </asp:Panel>
        <!--Popup radica interno-->
               <asp:Panel ID="Panel_radica_interno" runat="server" Style="display:none; overflow:hidden" ForeColor="White" Width="98%" Height="100% " CssClass="modal_content_general">
                  <asp:ModalPopupExtender ID="ModalPopupExtender_radica_interno" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_radica_interno"
                      PopupControlID="Panel_radica_interno"  CancelControlID="ButtonSalir_radica_interno">
                  </asp:ModalPopupExtender>
                  <div id="Cabecerapendiente_radica_interno" class="modal_title_superior">  
                       <h6 class="modal-title d-inline">Radicación interna</h6>
                       <button type="button" value="ButtonSalir_radica_interno" class="close da_event_captive mr-2">&times;</button>      
                       
                  </div>
                  <div id="Cotenedorpendiente_radica_interno" style="color: Black; background-color: #FFFFFF; height: 90%; width: 100%; overflow:hidden" class="modal_content_back">                 
                      <asp:UpdatePanel ID="UpdatePanel_radica_interno" runat="server" UpdateMode="Conditional">
                          <ContentTemplate>
                              <iframe id="Iframe_radica_interno_da_" runat="server" frameborder="0"  style="width:100%; height:100%; overflow:hidden"></iframe>
                          </ContentTemplate>
                      </asp:UpdatePanel>
                       <div style="display:none; height:1px">
                           <asp:Button ID="Button_radica_interno" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" style="display:none" />
                             <asp:Button ID="ButtonSalir_radica_interno" runat="Server" Text="" Height="1px" Width="1px" style="display:none" CssClass="invisible" 
                               />
                       </div>
                            
                  </div>
                  
              </asp:Panel>
        <!--compartir documento-->
          <div id="autoriza_compartir_documento">
            <asp:Panel ID="Panel_autoriza_compartir_documento" runat="server" Style="display:none; color: White; width: 90%; height: 100%" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_autoriza_compartir_documento" runat="server" Y="0" BehaviorID="Panel_autoriza_compartir_documento" TargetControlID="ButtonSalir_autoriza_compartir_documento" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_autoriza_compartir_documento" PopupControlID="Panel_autoriza_compartir_documento" ></asp:ModalPopupExtender>
                <div id="divcabecer2_autoriza_compartir_documento"  class="modal_title_superior">
                    <h6 class="modal-title d-inline ml-1">Compartir documento</h6>
                    <button type="button" value="Button_cerrar_autoriza_compartir_documento" class="close da_event_captive mr-2">&times;</button>            
                </div>
                <div id="contenido_procesa_autoriza_compartir_documento" style="background-color: white; width: 100%; height: 99%; color: black; background-color: #FFFFFF" class="modal_content_back">   
                        <asp:UpdatePanel ID="UpdatePanel_autoriza_compartir_documento" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                              <iframe id="Iframe_compartir_documento_" runat="server" frameborder="0"  style="width:100%; height:100%; overflow:hidden"></iframe>                           
                            </ContentTemplate>
                        </asp:UpdatePanel>
                         
                </div>
                   <div style="display:none; height:1px">
                        <asp:Button ID="Button_autoriza_compartir_documento" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" style="display:none" />
                        <asp:Button ID="ButtonSalir_autoriza_compartir_documento" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" style="display:none" />
                         <asp:Button ID="Button_cerrar_autoriza_compartir_documento" runat="Server" Text="" CssClass="invisible" Height="1px" Width="1px" style="display:none"
                             ToolTip="Cerrar ventana" />
                   </div>
                    
            </asp:Panel>
        </div>
        <!--digitaliza documento adjunto-->
          <div id="digitaliza_documento_adjunto"> 
            <asp:Panel ID="Panel_digitaliza_documento_adjunto" runat="server" Style="display:none; color: White; width: 100%; height:100%" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_digitaliza_documento_adjunto" runat="server" BehaviorID="Panel_digitaliza_documento_adjunto" TargetControlID="ButtonSalir_digitaliza_documento_adjunto" BackgroundCssClass="ModalBackgroud_gorund"
                    CancelControlID="Button_cerrar_digitaliza_documento_adjunto" PopupControlID="Panel_digitaliza_documento_adjunto" ></asp:ModalPopupExtender>
                <div id="divcabecer2_digitaliza_documento_adjunto"  class="modal_title_superior">   
                     <asp:UpdatePanel ID="UpdatePanel_titule_digitaliza" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>      
                            <asp:Label ID="Label_digitaliza_documento_adjunto" style="color:#6d7fcc" runat="server" Text="Digitalización" 
                                  CssClass="modal-title d-inline ml-1">
                            </asp:Label>          
                            </ContentTemplate>
                         </asp:UpdatePanel>
                          <button type="button" value="Button_cerrar_digitaliza_documento_adjunto" class="close da_event_captive mr-2">&times;</button> 
                </div>
                <div id="contenido_procesa_digitaliza_documento_adjunto" style="background-color: white; width: 100%; height: 99%; color: black; background-color: #FFFFFF" class="modal_content_back">
                     
                    <asp:UpdatePanel ID="UpdatePanel_iframe_digitaliza_adjunto" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <iframe id="IframeDitaliza_adjunto_" runat="server" frameborder="0" width="100%" scrolling="no" height="100%"></iframe>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                   
                </div>
                <div style="display: none; height: 1px">
                    <asp:Button ID="Button_cerrar_digitaliza_documento_adjunto" runat="Server" Text="X" CssClass="invisible"
                         />
                    <asp:Button ID="Button_digitaliza_documento_adjunto" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                    <asp:Button ID="ButtonSalir_digitaliza_documento_adjunto" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                </div>
                  
            </asp:Panel>
        </div>  
        <!--descarga documento plantilla-->
        <div id="descarga_documento_plantilla" style="clear:both">
            <asp:Panel ID="Panel_descarga_documento_plantilla" runat="server" Style="display:none; color: White; width: 30%; height: auto; margin:auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_descarga_documento_plantilla" runat="server" 
                     BehaviorID="Panel_descarga_documento_plantilla_ModalPopupExtender" 
                     TargetControlID="ButtonSalir_descarga_documento_plantilla" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_descarga_documento_plantilla" PopupControlID="Panel_descarga_documento_plantilla" ></asp:ModalPopupExtender>
                <div id="div_title_descarga_documento_plantilla" class="modal_title_superior">                 
                    <asp:Label ID="Label_descarga_documento_plantilla" runat="server" Text=" Descarga plantilla"  Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton2_descarga_documento_plantilla" style="float: right">
                        <asp:Button ID="Button_cerrar_descarga_documento_plantilla" runat="Server" Text="X" CssClass="modal_boton_hiden"
                             ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_descarga_documento_plantilla" style="background-color: white;  color: black; background-color: #FFFFFF; clear:both" class="modal_content_back">           
                        <asp:UpdatePanel ID="UpdatePanel_descarga_documento_plantilla" runat="server" UpdateMode="Conditional" >
                            <ContentTemplate>                
                                <table style="width: 100%;">                                 
                                    <tr>
                                        <td>
                                            <br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="text-align:center; font-weight:700">
                                            <asp:Label ID="Label27" runat="server" Text="Tipos de plantilla para elaboración de documentos" Style="text-align: center; font-family: Arial; font-size: 14px; text-align:center"></asp:Label>
                                        </td>
                                    </tr>
                                    
                                    <tr>
                                       
                                       <td style="text-align:center;">
                                           <asp:DropDownList ID="DropDownList_lista_tipo_plantilla" runat="server" style="width:98%">
                                               <asp:ListItem Value="1">&#9870; Con membrete</asp:ListItem>
                                               <asp:ListItem Value="2">&#9866; Sin membrete</asp:ListItem>
                                           </asp:DropDownList>
                                       </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <br />
                                        </td>
                                    </tr>
                                     <tr>
                                        <td style="text-align:right">
                                            <asp:Button ID="Button_descarga_plantilla" runat="server" Text="Aceptar" style="margin-right:8px; margin-bottom:5px" CssClass="boton_azul" />
                                        </td>
                                    </tr>
                                    
                                </table>
                                                         
                            </ContentTemplate>
                        </asp:UpdatePanel>
                          <asp:Button ID="Button_descarga_documento_plantilla" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" style="display:none" />
                    <asp:Button ID="ButtonSalir_descarga_documento_plantilla" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" style="display:none"/>
                </div>
            </asp:Panel>
        </div>
        <!--Notifica gestión-->
          <div id="notifica_gestion">
            <asp:Panel ID="Panel_notifica_gestion" runat="server" Style="display:none; color: White; width: 70%; height:auto; margin:auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_notifica_gestion" runat="server" BehaviorID="Panel_notifica_gestion_ModalPopupExtender" TargetControlID="ButtonSalir_notifica_gestion"
                    CancelControlID="Button_cerrar_notifica_gestion" PopupControlID="Panel_notifica_gestion" BackgroundCssClass="FondoAplicacion" >
                </asp:ModalPopupExtender>
                <div id="divcabecer2_notifica_gestion" class="modal_title_superior">     
                    <button type="button" value="Button_cerrar_notifica_gestion" class="close da_event_captive mr-2">&times;</button>                      
                </div>
                <div id="contenido_procesa_notifica_gestion" style="background-color:white; width:100%; height:auto; overflow:hidden" class="modal_content_back">
                    <asp:UpdatePanel ID="UpdatePanel_iframenotifica" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <iframe   style="color: White; width: 100%; background-color:white; height:auto; overflow:hidden" id="Iframe_comparte_coreo" runat="server" frameborder="0" scroll="none"  ></iframe>
                             <input id="Hidden_cuenta_correo_envio" type="hidden" value="" runat="server"/>
                             <input id="Hidden_correo_envio_default" type="hidden" value="" runat="server"/>
                             <input id="Hidden_imagen_adjunta" type="hidden" value="" runat="server"/>
                             <input id="Hidden_asunto_notificacion" type="hidden" value="" runat="server"/>
                             <input id="Hidden_convierte_pdf" type="hidden" value="" runat="server"/>
                            <input id="Hidden_tipo_notificacion" type="hidden" value="ENVIO CORREO PRODUCCION" runat="server"/>
                             <input id="Hidden_id_plantilla_radicado" type="hidden" value="" runat="server"/>
                                  
                        </ContentTemplate>
                    </asp:UpdatePanel>
                   
                </div>
                <div style="display: none; height: 1px">
                    <asp:Button ID="Button7" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                    <asp:Button ID="ButtonSalir_notifica_gestion" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                    <asp:Button ID="Button_cerrar_notifica_gestion" runat="Server" Text="" CssClass="invisible" Height="1px" Width="1px"
                         />
                </div>
            </asp:Panel>
        </div>
         <!--agregar_nivel-->
          <div id="agregar_nivel">
            <asp:Panel ID="Panel_agregar_nivel" runat="server" Style="display:none; width: auto; height: auto" CssClass="modal_content_general_">
                <asp:ModalPopupExtender ID="ModalPopupExtender_agregar_nivel" runat="server" BehaviorID="Panel_agregar_nivel" TargetControlID="ButtonSalir_agregar_nivel" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_agregar_nivel" PopupControlID="Panel_agregar_nivel" ></asp:ModalPopupExtender>
                <div id="modal_content_agregar_nivel" class="modal-content">
                    <div id="divcabecer2_agregar_nivel" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title ">Agregar nivel de organización</h6>
                        <button type="button" value="Button_cerrar_agregar_nivel" class="close da_event_captive">&times;</button>
                    </div>
                    <div id="contenido_procesa_agregar_nivel" style="background-color: white; width: auto; height: 99%; border-top: none; overflow: auto; min-height:100px" class="modal_content_back modal-body">
                        <div class="row mt-2 d-flex">
                            <div class="col-6 d-flex">
                                <asp:Label ID="Label3" runat="server" Text="Nivel de organización  " CssClass="h6 font-weight-light"></asp:Label>
                            </div>
                            <div class="col-6 d-flex">
                                <asp:TextBox ID="TextBox_nombre_nivel" runat="server"  MaxLength="40" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div style="display: none; height: 1px">
                            <asp:Button ID="Button_cerrar_agregar_nivel" runat="Server" Text="X" CssClass="invisible"
                                 />
                            <asp:Button ID="Button2" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                            <asp:Button ID="ButtonSalir_agregar_nivel" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                        </div>

                    </div>
                    <div class="modal-footer justify-content-end" id="modal-footer_agregar_nivel">
                        <asp:UpdatePanel ID="UpdatePanel_agregar_nivel" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Button ID="Button_agregar_nivel" runat="server" Text="Aceptar"  CssClass="btn btn-success" />
                                <input id="HiddenField_rest_0005" type="hidden" value="" runat="server"/>
                                <input id="Hidden_rest_tit_0006" type="hidden" value="" runat="server"/>
                                <input id="Hidden_rest_ur_0007" type="hidden" value="" runat="server"/>
                                <input id="Hidden_rest_val_0008" type="hidden" value="" runat="server"/>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
                
                
            </asp:Panel>
        </div>
        <!--editar_nivel-->       
            <asp:Panel ID="Panel_editar_nivel" runat="server" Style="display:none; width:auto; height: auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_editar_nivel" runat="server"  TargetControlID="ButtonSalir_editar_nivel" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_editar_nivel" PopupControlID="Panel_editar_nivel" ></asp:ModalPopupExtender>
                <div id="modal_content_editar_nivel" class="modal-content">
                    <div id="divcabecer2_editar_nivel" class="modal_title_superior_ modal-header">
                         <h6 class="modal-title ">Editar nivel de organización</h6>
                         <button type="button" value="Button_cerrar_editar_nivel" class="close da_event_captive ">&times;</button>   
                    </div>
                    <div id="contenido_procesa_editar_nivel" style=" width: auto; height:auto ; border-top:none; overflow:auto; min-height:100px" class="modal_content_back modal-body">           
                        <asp:UpdatePanel ID="UpdatePanel_editar_nivel" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row mt-2 d-flex">
                                    <div class="col-6 d-flex">
                                        <asp:Label ID="Label3_" runat="server" Text="Nivel de organización  " CssClass="h6 font-weight-light"></asp:Label>
                                    </div>
                                    <div class="col-6 d-flex">
                                        <asp:TextBox ID="TextBox_nombre_nivel_editar" runat="server" MaxLength="40" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div style="display: none; height: 1px">
                            <asp:Button ID="Button3" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                            <asp:Button ID="ButtonSalir_editar_nivel" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                            <asp:Button ID="Button_cerrar_editar_nivel" runat="Server" Text="X" CssClass="invisible" />
                        </div>
                    </div>
                    <div class="modal-footer justify-content-end" id="modal-footer_editar_nivel">  
                          <asp:UpdatePanel ID="UpdatePanel_editar_nivel_boton" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                   <input id="Hidden_res_edita_nivel_0001" type="hidden" value="0" runat="server">           
                                   <asp:Button ID="Button_editar_nivel" runat="server" Text="Aceptar"  CssClass="btn btn-success" />                               
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div> 
               
                 
            </asp:Panel>     
         <!--compartir_nivel-->
          
            <asp:Panel ID="Panel_compartir_nivel" runat="server" Style="display:none; width: 70%; height: auto; margin:auto" CssClass="modal_content_general_">
                <asp:ModalPopupExtender ID="ModalPopupExtender_compartir_nivel" runat="server" BehaviorID="Panel_compartir_nivel" TargetControlID="ButtonSalir_compartir_nivel" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_compartir_nivel" PopupControlID="Panel_compartir_nivel" ></asp:ModalPopupExtender>
                <div id="modal_content_compartir_nivel" class="modal-content">
                    <div id="divcabecer2_compartir_nivel" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title">Compartir nivel de organización</h6>
                        <button type="button" value="Button_cerrar_compartir_nivel" class="close da_event_captive">&times;</button>
                    </div>
                    <div id="contenido_procesa_compartir_nivel" style="background-color: white; width: 100%; height: 99%; border-top: none; overflow: auto" class="modal_content_back modal-body">
                        <div class="row mt-2">
                            <div class="col-12">
                                <select class="tokenize-callable-demo1  w-100 " multiple="">
                                </select>
                            </div>
                        </div>                  
                        <div class="row mt-2">
                            <div class="col-12 mt2 border-top pb-2 pt-2">
                                <asp:CheckBox ID="CheckBox_cargar_archivo" runat="server" Text="  "   CssClass="h6 font-weight-light" /> <span style="margin-left:4px"> Cargar archivos en los expedientes </span>
                                <br />
                                <asp:CheckBox ID="CheckBox_descargar_archivo" runat="server" Text=" " CssClass="h6 font-weight-light" /> <span style="margin-left:4px"> Descargar archivos en los expedientes</span>
                                 <br />                              
                                <asp:CheckBox ID="CheckBox_compartir_archivo" runat="server" Text=" " CssClass="h6 font-weight-light" /> <span style="margin-left:4px"> Compartir archivos en los expedientes</span>
                                <br />
                                <asp:CheckBox ID="CheckBox_eliminar_archivos" runat="server" Text=" " CssClass="h6 font-weight-light" /> <span style="margin-left:4px"> Eliminar archivos en los expedientes</span>
                                <br />
                                <asp:CheckBox ID="CheckBox_cambiar_nombre_archivos" runat="server" Text=" " CssClass="h6 font-weight-light" />   <span style="margin-left:4px"> Cambiar nombre de archivos en los expedientes</span>                
                                <br />
                                <asp:CheckBox ID="CheckBox_radicar_archivo" runat="server" Text=" " CssClass="h6 font-weight-light" /> <span style="margin-left:4px"> Radicar archivos en los expedientes</span>
                                <br />
                                <asp:CheckBox ID="CheckBox_visualizar_archivos" runat="server" Text=" " CssClass="h6 font-weight-light" /> <span style="margin-left:4px"> Visualizar archivos en los expedientes</span>
                                <br />
                                <asp:CheckBox ID="CheckBox_cambia_nombre_expediente" runat="server" Text=" " CssClass="h6 font-weight-light" /> <span style="margin-left:4px"> Editar expedientes</span>
                                <br />
                                <asp:CheckBox ID="CheckBox_eliminar_expediente" runat="server" Text="  " CssClass="h6 font-weight-light" /> <span style="margin-left:4px"> Eliminar expedientes</span>
                                <br />
                                <asp:CheckBox ID="CheckBox_agregar_expediente" runat="server" Text=" " CssClass="h6 font-weight-light" /> <span style="margin-left:4px"> Agregar nuevos expedientes</span>      
                                <br />
                                <asp:CheckBox ID="CheckBox_mover_expediente" runat="server" Text=" " CssClass="h6 font-weight-light" /> <span style="margin-left:4px"> Mover expedientes</span>
                                 <br />
                                <asp:CheckBox ID="CheckBox_copiar_archivo" runat="server" Text=" " CssClass="h6 font-weight-light" /> <span style="margin-left:4px"> Mover archivos entre expedientes</span>
                                <br />
                            </div>
                        </div>
                        <div style="display: none; height: 1px">
                            <asp:Button ID="Button4" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                            <asp:Button ID="ButtonSalir_compartir_nivel" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                            <asp:Button ID="Button_cerrar_compartir_nivel" runat="Server" Text="" CssClass="invisible" />
                        </div>
                    </div>
                    <div class="modal-footer justify-content-end" id="modal-footer_compartir_nivel">
                         <button type="button" value="Button_cerrar_compartir_nivel" onclick="Compartir_nivel_tokenize() " class=" btn btn-success">Aceptar</button>            
                    </div>
                </div>
               
            </asp:Panel>
         <!--lista_permisos_nivel_para_usuario-->
          <div id="lista_permisos_nivel">
            <asp:Panel ID="Panel_lista_permisos_nivel" runat="server" Style="display:none; width: 70%; height: auto" CssClass="modal_content_general_">
                <asp:ModalPopupExtender ID="ModalPopupExtender_lista_permisos_nivel" runat="server"  TargetControlID="ButtonSalir_lista_permisos_nivel" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_lista_permisos_nivel" PopupControlID="Panel_lista_permisos_nivel" ></asp:ModalPopupExtender>
                <div id="modal_content_lista_permisos_nivel" class="modal-content">
                    <div id="divcabecer2_lista_permisos_nivel" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title">Permisos del usuario para el nivel</h6>
                        <button type="button" value="Button_cerrar_lista_permisos_nivel" class="close da_event_captive ">&times;</button>
                    </div>
                    <div id="contenido_procesa_lista_permisos_nivel" style="background-color: white; width: 100%; height: 99%; border-top: none; overflow: auto" class="modal_content_back modal-body">
                        <asp:UpdatePanel ID="UpdatePanel_lista_permisos_nivel" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row mt-2">
                                    <div class="col-12 d-flex justify-content-center">
                                        <asp:Label ID="Label_permisos_nivel_lista" runat="server" CssClass="h6" Text="Permisos del nivel a usuario compartido"></asp:Label>
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <div class="col-12 mt2 border-top pb-2 pt-2">
                                        <asp:CheckBox ID="CheckBox_cargar_archivo_lista" runat="server" Text="  "  Enabled="false" /> <span Class="h6 font-weight-light ml-2"> Cargar archivos en los expedientes</span>
                                        <br />
                                        <asp:CheckBox ID="CheckBox_descargar_archivo_lista" runat="server" Text=" "  Enabled="false" /> <span Class="h6 font-weight-light ml-2"> Descargar archivos en los expedientes</span>
                                        <br />
                                        <asp:CheckBox ID="CheckBox_compartir_archivo_lista" runat="server" Text=" " Enabled="false" /> <span Class="h6 font-weight-light ml-2"> Compartir archivos en los expedientes</span>
                                        <br />
                                        <asp:CheckBox ID="CheckBox_eliminar_archivos_lista" runat="server" Text=" " Enabled="false" /> <span Class="h6 font-weight-light ml-2"> Eliminar archivos en los expedientes</span>
                                        <br />
                                        <asp:CheckBox ID="CheckBox_cambiar_nombre_archivos_lista" runat="server" Text=" " Enabled="false" /> <span Class="h6 font-weight-light ml-2"> Cambiar nombre de archivos en los expedientes</span>
                                        <br />
                                        <asp:CheckBox ID="CheckBox_radicar_archivo_lista" runat="server" Text=" " Enabled="false"  /> <span Class="h6 font-weight-light ml-2"> Radicar archivos en los expedientes</span>
                                        <br />
                                        <asp:CheckBox ID="CheckBox_visualizar_archivos_lista" runat="server" Text=" " Enabled="false" /> <span Class="h6 font-weight-light ml-2"> Visualizar archivos en los expedientes</span>
                                        <br />
                                        <asp:CheckBox ID="CheckBox_cambia_nombre_expediente_lista" runat="server" Text=" " Enabled="false" /> <span Class="h6 font-weight-light ml-2"> Editar expedientes</span>
                                        <br />
                                        <asp:CheckBox ID="CheckBox_eliminar_expediente_lista" runat="server" Text="  " Enabled="false" /> <span Class="h6 font-weight-light ml-2"> Eliminar expedientes</span>
                                        <br />
                                        <asp:CheckBox ID="CheckBox_agregar_expediente_lista" runat="server" Text=" " Enabled="false" /> <span Class="h6 font-weight-light ml-2"> Agregar nuevos expedientes</span>
                                        <br />
                                        <asp:CheckBox ID="CheckBox_mover_expediente_lista" runat="server" Text=" " Enabled="false" /> <span Class="h6 font-weight-light ml-2"> Mover expedientes</span>
                                        <br />
                                        <asp:CheckBox ID="CheckBox_copiar_archivo_lista" runat="server" Text=" " Enabled="false" /> <span Class="h6 font-weight-light ml-2"> Mover archivos entre expedientes</span>
                                        <br />
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                    </div>
                    <div class="modal-footer_lista_permisos_nivel">
                    </div>
                </div>
                <div style="display:none; height:1px">
                     <asp:Button ID="Button_cerrar_lista_permisos_nivel" runat="Server" Text="" CssClass="invisible"/>
                     <asp:Button ID="Button_lista" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                     <asp:Button ID="ButtonSalir_lista_permisos_nivel" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                </div>
                   
            </asp:Panel>
        </div>
         <!--listar_permisos_niveles-->
          <div id="listar_permisos_niveles">
            <asp:Panel ID="Panel_listar_permisos_niveles" runat="server" Style="display:none; width: 100%; height:100%" CssClass="modal_content_general_">
                <asp:ModalPopupExtender ID="ModalPopupExtender_listar_permisos_niveles" runat="server" BehaviorID="Panel_listar_permisos_niveles" TargetControlID="ButtonSalir_listar_permisos_niveles" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_listar_permisos_niveles" PopupControlID="Panel_listar_permisos_niveles" ></asp:ModalPopupExtender>
                <div id="modal_content_lista_permisos_niveles" class="modal-content">
                    <div id="divcabecer2_listar_permisos_niveles" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title">Permisos nivel de organización</h6>
                        <button type="button" value="Button_cerrar_listar_permisos_niveles" class="close da_event_captive">&times;</button>   
                    </div>
                    <div id="contenido_procesa_listar_permisos_niveles" style="width: 100%; height: 99%; border-top:none; overflow:auto" class="modal_content_back modal-body">
                        
                                <div id="title_permisos" class="row" >
                                    <div class="col-6">
                                         <asp:UpdatePanel ID="UpdatePanel_title_permisos" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <asp:Label ID="Label_title_permisos" runat="server" Text="Label" Style="float: left; margin-top: 3px" CssClass="h6 font-weight-light"></asp:Label>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                    </div>
                                    <div class="col-6 pb-2">
                                        <div class="input-group ">
                                            <button id="Button6" class="btn btn-outline-secondary border-right-2 " title="Busqueda en la lista" style="border-top-right-radius: 0px; border-bottom-right-radius: 0px" onclick="activa_boton_client_server('Button_restaura_consulta')" type="button">
                                                <i class="fal fa-long-arrow-left"></i>
                                            </button>
                                            <asp:TextBox ID="TextBox_consulta_permisos" runat="server" class="form-control form-control-sm complex  border-left-0" placeholder="Busqueda...." ></asp:TextBox>
                                            <div class="input-group-append">
                                                <button class="btn btn-outline-secondary"title="restaura lista" onclick="activa_boton_client_server('Button_activa_busqueda')" type="button">
                                                    <i class="fal fa-search"></i>
                                                </button>
                                            </div>
                                            <asp:TextBox ID="TextBox_consulta_permisos_" runat="server" Style="float: right; margin-right: 3px; min-width: 250px; display: none" placeholder="Busqueda" Height="20px"></asp:TextBox>
                                            <a style="margin: 5px 6px 1px 2px; float: right; color: black; display: none" title="Buscar usuarios" href="#" onclick="activa_boton_client_server('Button_activa_busqueda')"><i class="fas fa-search fa-1x"></i></a>
                                            <a style="margin: 5px 6px 1px 2px; float: right; color: black; display: none" title="Restaurar resultados" href="#" onclick="activa_boton_client_server('Button_restaura_consulta')"><i class="fas fa-redo-alt fa-1x"></i></a>
                                        </div>
                                    </div>
                             </div>
                        <asp:UpdatePanel ID="UpdatePanel_listar_permisos_niveles" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                
                                    <asp:Panel ID="Panel_principal" runat="server" ScrollBars="Auto"
                                        Width="100%" Style="min-height: 300px">
                                        <asp:GridView ID="data_grid_listado_permisos" runat="server" AllowSorting="true" AllowPaging="true" EnableViewState="true"
                                            PageSize="4" PagerSettings-Position="Top"  Style="font-family: Segoe UI; width: 100%; margin-left: 2px; margin-right: 2px"
                                            AutoGenerateSelectButton="False" CssClass="filtrar table font-weight-light " GridLines="None" Font-Size="16px">
                                            <SelectedRowStyle BackColor="LightSkyBlue"  ForeColor="Red" />
                                            <HeaderStyle CssClass="GridviewScrollHeader_line_boot" BorderStyle="None" />
                                            <RowStyle CssClass="" />
                                            <PagerStyle CssClass="pagination-ys  id_sele_pagi"  />
                                            <Columns>
                                                <asp:BoundField HeaderText="OPCIONES   " />
                                            </Columns>

                                        </asp:GridView>
                                    </asp:Panel>
                                    <input id="Hidden_sel" type="hidden" value="-1" runat="server"/>
                               
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div id="boton_lista" style="display: none">
                            <asp:UpdatePanel ID="UpdatePanel_botones_lista" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <input id="Hidden_00_09" type="hidden" value="-1" runat="server"/>
                                    <input id="Hidden_rest_ur_permiso_elimina_0007" type="hidden" value="" runat="server"/>
                                    <asp:Button ID="Button_eliminar_regi_permiso" runat="server" Text="Button" OnClientClick="pront_confirmacion('Desea eliminar el registro del usuario comparitdo');" />
                                    <asp:Button ID="Button_activa_busqueda" runat="server" Text="Button" />
                                    <asp:Button ID="Button_restaura_consulta" runat="server" Text="Button" />
                                </ContentTemplate>
                            </asp:UpdatePanel>

                        </div>

                    </div>
                </div>
                <div style="display: none; height: 1px">
                    <asp:Button ID="Button5" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                    <asp:Button ID="ButtonSalir_listar_permisos_niveles" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                    <asp:Button ID="Button_cerrar_listar_permisos_niveles" runat="Server" Text="" CssClass="invisible" />
                </div>
              </asp:Panel>
        </div>
       
        <!--ubicacion_documento-->
          <div id="ubicacion_documento">
              <asp:Panel ID="Panel_ubicacion_documento" runat="server" Style="display: none; color: black; width: 50%; height: auto" CssClass="modal_content_general_">
                  <asp:ModalPopupExtender ID="ModalPopupExtender_ubicacion_documento" runat="server" TargetControlID="ButtonSalir_ubicacion_documento" BackgroundCssClass="FondoAplicacion"
                      CancelControlID="Button_cerrar_ubicacion_documento" PopupControlID="Panel_ubicacion_documento">
                  </asp:ModalPopupExtender>
                  <div id="modal_content_Panel_ubicacion_documento" class="modal-content">
                      <div id="divcabecer2_ubicacion_documento" class="modal_title_superior_ modal-header">
                          <h6 class="modal-title d-inline ml-1">Ubicación documento</h6>
                          <button type="button" value="Button_cerrar_ubicacion_documento" class="close da_event_captive">&times;</button>
                      </div>
                      <div id="contenido_procesa_ubicacion_documento" style="width: 100%; height: auto; color: black; border-top: none" class="modal_content_back_ modal-header">
                          <asp:UpdatePanel ID="UpdatePanel_ubicacion_documento" runat="server" UpdateMode="Conditional">
                              <ContentTemplate>
                                  <div class="row w-100 mt-2">
                                      <div class="col-6">
                                          <asp:Label ID="Label_ubicacion_expediente" runat="server" CssClass="h6 font-weight-light" Text="Expediente de ubicación"></asp:Label>
                                      </div>
                                      <div class="col-6">
                                          <asp:TextBox ID="TextBox_expediente_ubicacion" runat="server" Enabled="false" Style="width: 95%" CssClass="form-control"></asp:TextBox>
                                      </div>
                                  </div>
                                  <div class="row w-100 mt-2">
                                      <div class="col-6">
                                          <asp:Label ID="Label_nivel_ubicacion" runat="server" CssClass="h6 font-weight-light" Text="Nivel de ubicación"></asp:Label>
                                      </div>
                                      <div class="col-6">
                                          <asp:TextBox ID="TextBox_nivel_ubicacion" runat="server" Enabled="false" Style="width: 100%" CssClass="form-control"></asp:TextBox>
                                      </div>
                                  </div>
                                  <div class="row w-100 mt-2">
                                      <div class="col-6">
                                          <asp:Label ID="Label_propietario_nivel_ubicacion" runat="server" CssClass="h6 font-weight-light" Text="Propietario nivel de ubicación"></asp:Label>
                                      </div>
                                      <div class="col-6">
                                          <asp:TextBox ID="TextBox_propietario_nivel_ubicacion" runat="server" Enabled="false" Style="width: 100%" CssClass="form-control"></asp:TextBox>
                                      </div>
                                  </div>
                                  <div class="row w-100 mt-2">
                                      <div class="col-6">
                                          <asp:Label ID="Label_cargo_propietario_nivel" runat="server" CssClass="h6 font-weight-light" Text="Cargo propietario nivel de ubicación"></asp:Label>
                                      </div>
                                      <div class="col-6">
                                          <asp:TextBox ID="TextBox_cargo_propietario_nivel" runat="server" Enabled="false" Style="width: 95%" CssClass="form-control"></asp:TextBox>
                                      </div>
                                  </div>

                              </ContentTemplate>
                          </asp:UpdatePanel>

                      </div>
                  </div>
                  <div style="display: none; height: 1px">
                      <asp:Button ID="Button_lista_ubicacion" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                      <asp:Button ID="ButtonSalir_ubicacion_documento" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                      <asp:Button ID="Button_cerrar_ubicacion_documento" runat="Server" Text="" Height="0px" Width="0px" />
                  </div>

              </asp:Panel>
        </div>
           <!--actualiza indice bacth-->    
        <asp:Panel ID="Panel_actualiza_indice_batch_wf" runat="server" Style="display: none; width: 80%; height: auto" CssClass="modal_content_general">
            <asp:ModalPopupExtender ID="ModalPopupExtender_edition_actualiza_indice_batch_wf" runat="server" TargetControlID="ButtonSalir_actualiza_indice_batch_wf" BackgroundCssClass="FondoAplicacion"
                CancelControlID="Button_cerrar_actualiza_indice_batch_wf" PopupControlID="Panel_actualiza_indice_batch_wf">
            </asp:ModalPopupExtender>
            <div class="modal-content_" id="modal_content_actualiza_indice_batch_wf">
                <div id="title_actualiza_indice_batch_wf" class="modal_title_superior_ modal-header">
                    <h6 class="modal-title">Actualiza indice batch</h6>
                    <button type="button" value="Button_cerrar_actualiza_indice_batch_wf" class="close da_event_captive">&times;</button>
                </div>
                <div id="contenido_procesa_actualiza_indice_batch_wf" style="width: auto; height: auto; border-top: none; overflow:auto" class="modal_content_back modal-body">
                    <div id="div_actualiza_indice_batch_wf" style="height:100%">       
                         
                    </div>
                </div>
            </div>
            <div class="modal-footer align-content-end" id="modal_foter_actualiza_indice_batch_wf">
                <button type="button" id="boton_event_actualiza_indice_batch_wf" title=""  class="btn btn-success   mt-1"> Aceptar</button>
                <button type="button" title="" value="Button_cerrar_actualiza_indice_batch_wf" class="btn btn-light da_event_captive  mt-1"> Cancelar </button>  
            </div>
            <div style="display: none; height: 1px">
                <asp:Button ID="Button_cerrar_actualiza_indice_batch_wf" runat="Server" Text="X" CssClass="invisible" />
                <asp:Button ID="Button_actualiza_indice_batch_wf" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                <asp:Button ID="ButtonSalir_actualiza_indice_batch_wf" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
            </div>
        </asp:Panel>
         <input id="Hidden_name_event" type="hidden" value="" runat="server"/>     
           <input id="Hidden_colum_header" type="hidden" value="" runat="server"/>      
          <asp:Panel ID="Panel_interface_regitra_meta_dato" runat="server" Style="display:none;  width:50%; height: auto" CssClass="modal_content_general_">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_interface_regitra_meta_dato"  runat="server" BackgroundCssClass="FondoAplicacion"  TargetControlID="ButtonSalir_interface_regitra_meta_dato" 
                    CancelControlID="Button_cerrar_interface_regitra_meta_dato" PopupControlID="Panel_interface_regitra_meta_dato" ></asp:ModalPopupExtender>
                <div class="modal-content">
                    <div id="divcabecer2_interface_regitra_meta_dato" class="modal_title_superior_ modal-header">
                        <h6 id="label_interface_regitra_meta_dato" class="modal-title  ">Registra meta dato</h6>
                        <button type="button" value="Button_cerrar_interface_regitra_meta_dato" class="close da_event_captive">&times;</button>
                    </div>
                    <div id="contenido_procesa_interface_regitra_meta_dato" style="background-color: white; width: auto; height: auto; color: black; background-color: #FFFFFF; border-top: none; overflow: auto" class="modal_content_back modal-body">
                        <div id="conte_regitra_meta_dato_control" >
                         
                        </div>
                    </div>
                    <div class="modal-footer" id="modal_footer_regitra_meta_dato">
                        <input id="Button_registra_meta" type="button" value="Aceptar" onclick="event_element_clic(event,this);" class="btn btn-success"/>
                        <input id="Button_file_firma" type="button" value="Aceptar" style="display:none" class="btn btn-success"/>
                    </div>
                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button_cerrar_interface_regitra_meta_dato" runat="Server" Text="" />
                        <asp:Button ID="Button_interface_regitra_meta_dato" runat="server" Text="" Height="1px" Width="1px" />
                        <asp:Button ID="ButtonSalir_interface_regitra_meta_dato" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                    </div>
                </div>
            </asp:Panel>
        <!--Popup actualización solicitud_firma -->
        <div class="modal fade modal_opacity" id="modal_content_solicitud_firma" data-backdrop="static" data-keyboard="false" tabindex="-1" aria-labelledby="staticBackdropLabel" aria-hidden="true">
            <div class="modal-dialog  modal-mediunscreen-sm-down modal-dialog-scrollable">
                <div class="modal-content-fullscreen">
                    <div class="modal-header">
                        <h4 id="label_title_solicitud_firma" style="color: black" class="modal-title">Solicitud de firma electrónica</h4>
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>
                    <div class="modal-body-fullscreen modal-body" style="overflow:auto">
                        <div id="div_registro_firmante" class="pb-1">
                            <div class="row">
                                <div class="col-12">
                                    <h6>Relacione quiénes recibirán el documento para firma electrónica.</h6>
                                </div>
                            </div>
                            <div class="row  pb-1">
                                <div class="col-4">
                                    <input type="text" id="NombreFirmante" class="form-control"  placeholder="Nombre"/>
                                </div>
                                 <div class="col-4">
                                     <input type="email" id="EmailFirmante" class="form-control" placeholder="Email" />
                                </div>
                                 <div class="col-4">
                                     <input type="text" id="IdenficacionFirmante" class="form-control" placeholder="Indentificación"/>
                                </div>
                            </div>
                            <div class="row pb-2">
                                <div class="col-12">
                                <button id="btn_add_firmante" type="button" class="btn  btn-primary w-100">  Agregar firmante   </button>  
                                </div>
                            </div>
                        </div>
                        <div id="div_solicitud_firma" style="height: 100%">
                            <table class="table-not-border_person" style="background-color: white"
                                id="table_list_firmantes_table"
                                data-pagination="false"
                                data-page-list="[5,10, 25, 50, 100, all]"
                                data-page-size="5"
                                data-show-export="false"
                                data-show-refresh="false"
                                data-cache="false"
                                data-toggle="table"
                                data-id-field="Ident"
                                data-unique-id="Ident"
                                data-click-to-select="true"
                                data-search="false"
                                data-locale="es-SP">
                                <thead class="GridviewScrollHeader_line_blue_wite">
                                    <tr>  
                                        <th
                                            data-field="Nombre"> Nombre
                                        </th>
                                        <th
                                            data-field="Email">Email
                                        </th>
                                        <th
                                            data-field="Ident">Identificación
                                        </th>
                                        <th data-field="operate" data-align: "center" data-formatter="operateFormatterRegistro" data-events="window.operateEventsRegistro"></th>
                                    </tr>
                                </thead>

                            </table>
                        </div>
                    </div>
                    <div id="error_content_solicitud_firma" style="position: relative; width: 100%"></div>
                    <div class="modal-footer align-content-end" id="modal_foter_actualizacion_solicitud_firma">
                        <button type="button" id="Btn_solicitud_firma" title="Solicitar firma electrónica"  class="btn  btn-primary  mt-1">Aceptar</button>
                    </div>
                </div>
            </div>
        </div>   
        <!--Termina Popup solicitud firma -->
        <!--PROGRES-->
        <div id="Divpro_gres_bar">
            <asp:Panel ID="Panel_pro_gres_bar" runat="server" Style="display:none; color: White; width:30%; height:auto" CssClass="border_superior_inferior_radius_blanco">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_pro_gres_bar" runat="server"  TargetControlID="ButtonSalir_pro_gres_bar" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_pro_gres_bar" PopupControlID="Panel_pro_gres_bar"></asp:ModalPopupExtender>
                <div id="div1" class="border_superior_radius_blanco" style="display:none">                
                    <asp:Label ID="Label_pro_gres_bar" runat="server" Text=""  Style="">
                    </asp:Label>
                    <div id="Divcerrarbuton2_pro_gres_bar" style="float: right">
                        <asp:Button ID="Button_cerrar_pro_gres_bar" runat="Server" Text="X" style="display:none" 
                             ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_pro_gres_bar" style="width:99%; height:99%" class="modal_content_back_no_radio" > 
                      <br />   
                    <div style="text-align:center">
                         <asp:Label ID="Label_progres_bar" runat="server" Text="Progreso de la tarea" style="font-family:Arial; text-align:center; font-size:20px"></asp:Label>
                    </div>
                    <br />  
                     <div id="myProgress_contador" style="text-align: center; font-family:Arial; font-size:14px">
                        
                             0 
                        </div>
                    <div id="myProgress_porcent" style="text-align: center; font-family:Arial; font-size:14px">
                            0 %
                        </div>                
                        <div style="margin-left:5%; margin-right:5%">
                            <div id="myProgress" >
                            <div id="myBar" ></div>
                        </div>
                        </div>         
                        <br/>
                        <div style="text-align: center">
                            <button class="boton_blanco" onclick="myStopFunction(event)" >Cancelar</button>
                        </div>
                              
                    <asp:UpdatePanel ID="UpdatePanel_pro_gres_bar" runat="server" UpdateMode="Conditional">
                        <ContentTemplate> 
                           
                            <asp:Button ID="Button_pogres_show" CssClass="invisible" runat="server" Text="Button" style="display:none" />   
                            
                        </ContentTemplate>
                    </asp:UpdatePanel>
                         
                    <asp:Button ID="ButtonSalir_pro_gres_bar" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />    
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
         <div id="progres_bar" class="load_ding" style="position: fixed; text-align: center; display: none; width: 150px; width: 200px">
                <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
                Processing ...
            </div>
        <div style="display: none">
            <asp:UpdatePanel ID="updatapanel_iframe" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <iframe runat="server" style="float: left" id="ifmExcel_" width="0" height="0" marginheight="0" marginwidth="0"
                        frameborder="0" />
                </ContentTemplate>

            </asp:UpdatePanel>
        </div>
    </form>
    <script  accesskey="javascript" type="text/javascript">
        AjaxFileUpload_change_text();
        $("#TreeViewArchivo").on('click', this, OnNodeClicked);
        $("#TreeViewArchivo").on('keydown', this, OnkeyDown);
        $(document).ready(function () {
            $('#sidebarCollapse').on('click', function () {
                $('#sidebar_').toggleClass('active_da_slider');
                $('#Contenedorderecho').toggleClass('active_content_rigth');
                $('#Contentizquierdo').toggleClass('active_content_left');
                $(this).toggleClass('d-none');
                $('#da_show-sidebar_').toggleClass('d-none');
            });
            $('#da_show-sidebar_').on('click', function () {
                $('#sidebar_').toggleClass('active_da_slider');
                $('#Contenedorderecho').toggleClass('active_content_rigth');
                $('#Contentizquierdo').toggleClass('active_content_left');
                $(this).toggleClass('d-none');
                $('#sidebarCollapse').toggleClass('d-none');
                auto_zise();

            });
        });
    </script>
    
</body>
</html>
