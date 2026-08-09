<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormGaGestionUnidadConservacion.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormGaGestionUnidadConservacion" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
     <script src="../js/ui/jquery-3.4.1.min.js"></script>  
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
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
     <script src="../js/gestion/WebFormGaGestionUnidadConservacion.js"></script>
     <script src="../js/java_general/general_code_java.js"></script> 
    <script src="../js/validate_campos.js"></script>
    <script src="../js/MyJavaScriptFile.js"></script>    
    <style type="text/css">
        </style>
      <script accesskey="javascript" type="text/javascript">
                  
      </script>
</head>
<body>
    
    <form id="formGaGestionExpediente" runat="server">
    
     <asp:ScriptManager ID="ScriptManager1" runat="server">
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
                if (elmen.type == "button" || elmen.type == "image" || elmen.type == "submit") {
                    //value_element = elmen.value;
                    //elmen.value = "Espere..."
                    elmen.disabled = true;
                }
            }
            function CheckStatus(sender, args) {
                progres_hiden('progres_bar');
                //Button_Lista_Radicados Button_actualizar_guia Button_anular_guia
                var elmen = document.getElementById(elment_postbak.id)
                if (elmen.type == "button" || elmen.type == "image" || elmen.type == "submit") {
                    elmen.disabled = false;
                    //elmen.value = value_element;
                }
                
                if (elment_postbak.id == "Button_Editar_unidad_conservacion") {
                    auto_zise_agregar_expediente();
                }
                if (elment_postbak.id == "Button_archivar_expediente_gestion") {
                    auto_zise_reasigna_expe_unidad();
                }
                if (elment_postbak.id == "Button_ubicacio_unidad_conservacion") {
                    auto_zise_ubicacion_toponimica();
                }
                
                if (elment_postbak.id == "ButtonConsulta") {
                    auto_zise();
                }
                if (elment_postbak.id == "data_grid") {
                    auto_zise();
                }
                //Eliminar registro en java
                if (elment_postbak.id == "Button_eliminar_unidad_conservacion") {
                    if (document.getElementById("Hidden_result").value == "YES") {
                        eliminar_fila_data_gred('data_grid');
                        document.getElementById("Hidden_result").value = "NO";
                    }

                }
            }

       </script>
  <div id="contendor_principal" style="height: 100%; width:100%">
        <input id="Hiddenheigpagina" type="hidden" value="475" runat="server">
        <input id="Hiddennameasigna" type="hidden" value="" runat="server">
        <input id="HiddenPROMP" type="hidden" value="0" runat="server">
      <nav id="menu_var" class="navbar navbar-expand-sm nav_botota_person_gray modal_content_no_back_inferior">
          <button id="nav_togle_display" class="navbar-toggler" type="button" style="background-color: #6d7fcc" data-toggle="collapse" data-target="#navbarNavDropdown">
              <span class="navbar-toggler-icon_"><i style="color: white" class="fad fa-th-list"></i></span>
          </button>
           <div class="collapse navbar-collapse row" id="navbarNavDropdown">  
               <ul class="navbar-nav col-md-12" >
                    <li class="nav-item dropdown active ml-2 mr-0 active_">
                        <a class="nav-link  dropdown-toggle " style="color: #6d7fcc" href="#" id="A5" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc" class="fad fa-archive"></i> Unidad de conservación 
                        </a>
                        <ul class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">
                            <li> <a style="color: #6d7fcc" href="#" class="dropdown-item font-weight-light"  onclick="activa_boton_client_server('Button_Editar_unidad_conservacion')"><i class="fal fa-edit"></i> Editar unidad de conservación</a></li>
                            <li> <a style="color: #6d7fcc" href="#" class="dropdown-item font-weight-light"  onclick="activa_boton_client_server('Button_eliminar_unidad_conservacion')"><i class="fad fa-times"></i> Eliminar unidad de conservación</a></li> 
                          
                        </ul>
                    </li>
                    <li class="nav-item dropdown active ml-2 mr-0 active_">
                        <a class="nav-link  dropdown-toggle " style="color: #6d7fcc" href="#" id="A1" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc" class="fad fa-folder-tree"></i> Ubicación 
                        </a>
                        <ul class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">
                            <li>  <a color: "#6d7fcc" href="#" class="dropdown-item font-weight-light" onclick="activa_boton_client_server('Button_ubicacio_unidad_conservacion')"><i class="fal fa-folder-tree"></i> Ubicación unidad de conservación</a></li>
                        </ul>
                    </li>
                   <li class="nav-item dropdown active ml-2 mr-0 active_">
                        <a class="nav-link  dropdown-toggle " style="color: #6d7fcc" href="#" id="A2" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc" class="fad fa-stamp"></i> Rotulo 
                        </a>
                        <ul class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">
                            <li>   <a color: "#6d7fcc" href="#" class="dropdown-item font-weight-light" onclick="activa_boton_client_server('ButtonRotulo')"><i class="fal fa-file-download"></i> Descargar rótulo unidad de conservación</a></li>
                            <li>   <a color: "#6d7fcc" href="#" class="dropdown-item font-weight-light" onclick="activa_boton_client_server('ButtonImprimirRotulo')"><i class="fad fa-print"></i> Imprimir rótulo unidad de conservación</a></li>
                        </ul>
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
      <div id="menu_var_" class="navbar_gray" style="overflow: auto; display:none">
                <div class="dropdown_gray">
                    <div class="dropbtn_gray">
                        <i class="fad fa-archive"></i>
                        Unidad de conservación
                            <i class="fa fa-caret-down"></i>
                    </div>
                    <div class="dropdown-content_gray">
                       
                        <a href="#" onclick="activa_boton_client_server('Button_Editar_unidad_conservacion')"><i class="fal fa-edit"></i> Editar unidad de conservación</a>
                        <a href="#" onclick="activa_boton_client_server('Button_eliminar_unidad_conservacion')"><i class="fad fa-times"></i> Eliminar unidad de conservación</a>
                        
                    </div>
                </div>
                <div class="dropdown_gray">
                    <div class="dropbtn_gray">
                        <i class="fad fa-folder-tree"></i>
                        Ubicación 
                                <i class="fa fa-caret-down"></i>
                    </div>
                    <div class="dropdown-content_gray">
                       
                        <a href="#" onclick="activa_boton_client_server('Button_ubicacio_unidad_conservacion')"><i class="fal fa-folder-tree"></i> Ubicación unidad de conservación</a>

                    </div>
                </div>
                <div class="dropdown_gray">
                    <div class="dropbtn_gray">
                        <i class="fad fa-stamp"></i>
                        Rotulo
                                 <i class="fa fa-caret-down"></i>
                    </div>
                    <div class="dropdown-content_gray">
                        <a href="#" onclick="activa_boton_client_server('ButtonRotulo')"><i class="fal fa-file-download"></i> Descargar rótulo unidad de conservación</a>
                        <a href="#" onclick="activa_boton_client_server('ButtonImprimirRotulo')"><i class="fad fa-print"></i> Imprimir rótulo unidad de conservación</a>
                       
                    </div>
                </div>
               
                
            </div>
        <div id="Contentizquierdo" style="width: 25%; height: 100%; float: left; position:relative">
            <asp:UpdatePanel ID="UpdatePaneLconsulta" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                     <asp:DropDownList ID="DropDownListEntidadEmpresa" runat="server"  Width="99%" CssClass="custom-select" onchange="cambio_empresa_gestion_consulta()"></asp:DropDownList>
                    <asp:Panel ID="Panelcampos" runat="server" ScrollBars="Both"
                        Height="90%" Width="99%" Style="background-color:white; position: inherit; border:solid 1px #ccc" >
                       
                        <asp:CheckBox ID="CheckBoxsolo_expeidente_propio" runat="server" CssClass="ml-2"  />
                        <span class="ml-2" style="color:red">Solo ver mis unidades</span>
                         <br />
                        <div class=" row-12">
                            <div class="col-12">
                               <span class="">Código único</span> 
                            </div>
                             <div class="col-12">
                                  <asp:TextBox ID="TextBoxCODIGO_UNICO" runat="server" Width="100%"  CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class=" row-12 mt-1">
                            <div class="col-12">
                                 <span class="">Consecutivo unidad</span>
                            </div>
                             <div class="col-12">
                                  <asp:TextBox ID="TextBoxCONSECUTIVO_UNIDAD_CONSERVACION" runat="server" Width="100%" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>                 
                        <div class=" row-12 mt-2">
                            <div class="col-12">
                                <span class="">Fecha creación (yyyy mm dd)</span>
                            </div>
                             <div class="col-12">
                                 <div class="row ">
                                     <div class="col-6 p-3">
                                         <div class="row p-0">
                                             <div class=" col-9 ">
                                                 <asp:TextBox ID="TextBoxFECHA_CREACION_INI" runat="server" Width="100%" CssClass="form-control" onkeypress="return validate_fecha(event,this)"></asp:TextBox>
                                                 <asp:CalendarExtender ID="TextBoxFECHA_CREACION_INI_CalendarExtender" runat="server" BehaviorID="TextBoxFECHA_CREACION_INI_CalendarExtender" TargetControlID="TextBoxFECHA_CREACION_INI" DaysModeTitleFormat='yyyy-MM-dd' Format='yyyy-MM-dd' PopupButtonID="ImageButtonCreacionIni" />
                                             </div>
                                             <div class=" col-3 p-0">
                                                 <button id="ImageButtonCreacionIni" class="btn btn-success border-0" type="button" value=""><i class="fad fa-calendar-alt fa-1x"></i></button>
                                             </div>

                                         </div>
                                     </div>
                                     <div class="col-6 p-3">
                                         <div class="row p-0">
                                             <div class="col-9">
                                                 <asp:TextBox ID="TextBoxFECHA_CREACION_FINAL" runat="server" Width="100%" CssClass="form-control" onkeypress="return validate_fecha(event,this)"></asp:TextBox>
                                                 <asp:CalendarExtender ID="TextBoxFECHA_CREACION_FINAL_CalendarExtender" runat="server" BehaviorID="TextBoxFECHA_CREACION_FINAL_CalendarExtender" TargetControlID="TextBoxFECHA_CREACION_FINAL" DaysModeTitleFormat="yyyy-mm-dd" Format='yyyy-MM-dd' PopupButtonID="ImageButtonCreacionFin" />
                                             </div>
                                             <div class="col-3 p-0">
                                                 <button id="ImageButtonCreacionFin" class="btn btn-success border-0" type="button" value=""><i class="fad fa-calendar-alt fa-1x"></i></button>
                                             </div>
                                         </div> 
                                     </div>
                                 </div>
                            </div>
                        </div>
                         <div class=" row-12 mt-2">
                            <div class="col-12">
                                <asp:CheckBox ID="CheckBox_tema" runat="server" style="margin-left:5px" />
                                <span>Tema unidad conservación</span>
                                 
                            </div>
                             <div class="col-12">
                                  <asp:TextBox ID="TextBoxTEMA_UNIDAD_CONSERVACION" runat="server" Width="100%" CssClass="form-control" ></asp:TextBox>
                            </div>
                        </div>
                          <div class=" row-12 mt-2">
                            <div class="col-12">
                                <asp:CheckBox ID="CheckBox_Descripcion" runat="server" style="margin-left:5px" />
                                <span>Descripcion unidad conservación</span>
                            </div>
                             <div class="col-12">
                                  <asp:TextBox ID="TextBoxDESCRIPCION_UNIDAD_CONSERVACION" runat="server" Width="100%" CssClass="form-control" ></asp:TextBox>
                            </div>
                        </div>
                          <div class=" row-12 mt-2">
                            <div class="col-12">
                                <span>Nombre área gestión(sección)</span>
                            </div>
                             <div class="col-12">
                                 <asp:TextBox ID="TextBoxNOMBRE_AREA" runat="server" Width="100%"  CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                         <div class="row-12 mt-2">
                            <div class="col-12">
                                <span>Nombre sub área gestión(sub sección)</span>
                            </div>
                            <div class="col-12">
                                <asp:TextBox ID="TextBoxNOMBRE_SUB_AREA" runat="server" Width="100%" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                         <div class=" row-12 mt-2">
                            <div class="col-12">
                                <span>Nombre serie documental</span>
                            </div>
                            <div class="col-12">
                                <asp:TextBox ID="TextBoxNOMBRE_SERIE" runat="server" Width="100%"  CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class=" row-12 mt-2">
                            <div class="col-12">
                                <span>Nombre sub serie documental</span>
                            </div>
                            <div class="col-12">
                                 <asp:TextBox ID="TextBoxNOMBRE_SUBSERIE" runat="server" Width="100%"  CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                         <div class=" row-12 mt-2">
                            <div class="col-12">
                                <span>Fechas extremas (yyyy mm dd)</span>
                            </div>
                            <div class="col-12">
                                 <div class="row ">
                                     <div class="col-6 p-3">
                                         <div class="row p-0">
                                             <div class=" col-9 ">
                                                  <asp:TextBox ID="TextBoxFECHA_EXTREMA_INICIAL" runat="server" Width="100%" onkeypress="return validate_fecha(event,this)"></asp:TextBox>
                                                  <asp:CalendarExtender ID="TextBoxFECHA_EXTREMA_INICIAL_CalendarExtender" runat="server" BehaviorID="TextBoxFECHA_EXTREMA_INICIAL_CalendarExtender" TargetControlID="TextBoxFECHA_EXTREMA_INICIAL" Format='yyyy-MM-dd' PopupButtonID="ImageButtonfechaextremaini" />
                                             </div>
                                             <div class=" col-3 p-0">
                                                 <button id="ImageButtonfechaextremaini" class="btn btn-success border-0" type="button" value=""><i class="fad fa-calendar-alt fa-1x"></i></button>
                                             </div>
                                         </div>
                                     </div>
                                     <div class="col-6 p-3">
                                         <div class="row p-0">
                                             <div class=" col-9 ">
                                                 <asp:TextBox ID="TextBoxFECHA_EXTREMA_FINAL" runat="server" Width="100%" CssClass="form-control" onkeypress="return validate_fecha(event,this)"></asp:TextBox>
                                                 <asp:CalendarExtender ID="TextBoxFECHA_EXTREMA_FINAL_CalendarExtender" runat="server" BehaviorID="TextBoxFECHA_EXTREMA_FINAL_CalendarExtender" TargetControlID="TextBoxFECHA_EXTREMA_FINAL" Format='yyyy-MM-dd' PopupButtonID="ImageButtonfechaextremafin" />
                                             </div>
                                             <div class=" col-3 p-0">
                                                 <button id="ImageButtonfechaextremafin" class="btn btn-success border-0" type="button" value=""><i class="fad fa-calendar-alt fa-1x"></i></button>
                                             </div>
                                         </div>
                                     </div>
                                 </div>
                            </div>
                            
                        </div>
                         
                        <div class=" row-12 mt-2">
                            <div class="col-12">
                                <span>Rangos extremos</span>
                            </div>
                             <div class="col-12">
                                  <div class="row ">
                                      <div class="col-5 p-3">
                                          <asp:TextBox ID="TextBoxRANGO_EXTREMO_INICIAL" runat="server" Width="100%" CssClass="form-control" ></asp:TextBox>
                                      </div>
                                      <div class="col-2 p-3">
                                          <span>Hasta</span>
                                      </div>
                                      <div class="col-5 p-3">
                                           <asp:TextBox ID="TextBoxRANGO_EXTREMO_FINAL" runat="server" Width="100" CssClass="form-control"></asp:TextBox>
                                      </div>
                                  </div>
                             </div>
                        </div>
                        <div class=" row-12 mt-2">
                            <div class="col-12">
                                <span>Estado archivado unidad documental</span>
                            </div>
                            <div class="col-12">
                                <asp:DropDownList ID="DropDownListEstado_Expediente" runat="server"  Width="100%"  CssClass="custom-select" >
                                    <asp:ListItem>Todos</asp:ListItem>
                                    <asp:ListItem>Archivados</asp:ListItem>
                                    <asp:ListItem>Sin archivar</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                       <div class=" row-12 mt-2">
                           <div class="col-12">
                               <span>Creador por el usuario</span>
                           </div>
                           <div class="col-12">
                               <asp:DropDownList ID="DropDownListUusuariocreador" runat="server"  Width="100%" CssClass="custom-select" ></asp:DropDownList>
                           </div>
                       </div>
                       <div class=" row-12 mt-2">
                           <div class="col-12">
                               <span>Clase unidad conservación</span>
                           </div>
                           <div class="col-12">
                               <asp:DropDownList ID="DropDownListtipounidadconservacion" runat="server"  Width="100%"  CssClass="custom-select"></asp:DropDownList>
                           </div>
                       </div>
                       
                    </asp:Panel>
                    
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel ID="UpdatePanel_botones_val_radicacion" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Panel ID="Panelbuton" runat="server" ScrollBars="None"
                        Height="10%" Width="99%" Style="background-color: white; border:solid 1px #ccc" CssClass="p-1">
                       
                         <asp:Button ID="ButtonConsulta" runat="server" Text="Consulta" ToolTip="Consulta" Style="" CssClass="btn btn-success" />
                      
                         <asp:Button ID="ButtonRestaurar" runat="server" Text="Restaurar" ToolTip="Restaurar" Style="" CssClass="btn btn-success" />
                        
                        <asp:Button ID="Button_asigna_expediente_gestion" runat="server" Text="Asignar " Style="display:none"  ToolTip="Asigna unidad documental"  OnClientClick="importa_dato_expediente()"  CssClass="btn btn-success" />
                        
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>   
       
      <div id="Contenedorderecho" style="width: 74%; position: inherit; left: auto; float: right; height: 100%; float:right">
        <div id="Contenedorgrid" style="width: 100%; position: inherit; left: auto; float: right; height: 90%">                    
            <asp:UpdatePanel ID="UpdateGeneral" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                     <input id="hdnEmailID" type="hidden" value="0" runat="server">
                    <input id="hdnEmailID_VAL" type="hidden" value="0" runat="server">
                    <input id="HiddenEmailconsulta" type="hidden" value="" runat="server">   
                    <div id="contenido_titulo_val_radicacion" style=" width: 100%" class="row p-2">
                         <div class="col-6">
                            <asp:Label ID="titulo_label_expedientes" runat="server" CssClass="h6">Resultados busqueda</asp:Label>
                        </div>   
                        <div class="col-6">
                            <asp:Label ID="Label_estado" runat="server" Font-Size="9px" CssClass="h6"></asp:Label>
                        </div>
                               
                    </div>                
                    <asp:Panel ID="Panelactividad" runat="server" Wrap="False"
                        ScrollBars="Horizontal"  Width="100%"   style="overflow:auto" >
                        <asp:GridView ID="data_grid" runat="server" style=" position:inherit"  GridLines="None" EnableViewState="true"
                            AutoGenerateSelectButton="False" AllowPaging="true" PageSize="7"  PagerSettings-Position="Top" 
                             AllowSorting="false" Height="95%" ScrollBars="Horizontal" CssClass="table  font-weight-light">
                            <RowStyle VerticalAlign="Middle" />
                            <PagerSettings />
                            <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
                             <SelectedRowStyle BackColor="#E7EDF5"  ForeColor="Black" />
                               <HeaderStyle CssClass="GridviewScrollHeader_line_boot" />
                              <PagerStyle CssClass="pagination-ys" />
                        </asp:GridView>
                    </asp:Panel>
                    <div id="botones_accion_postback" style="display: none">
                        <asp:Button ID="Button_actualiza_expdientes_agregados" runat="server" />
                    </div>
                </ContentTemplate>
       
                <Triggers>
                          
                </Triggers>
            </asp:UpdatePanel>
          
        </div>   
        
          <asp:HiddenField ID="HiddenField_botones_respuesta" runat="server" value="-1"/>
        <div id="contenido_botonoes" style="width: 100%; position:inherit; left: auto; float: right; height: 10%; background-color:white; overflow:auto; border:solid 1px #ccc; display:none">
            <asp:UpdatePanel ID="UpdatePanel_botones_opcion" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Button ID="Button_nuevo_expediente_gestion" runat="server" Text="Nuevo " Style="margin-top: 2px; margin-left: 5px; display:none" CssClass="boton_azul" ToolTip="Agregar nueva unidad documental" OnClientClick="tamano_ventana_agregar_expediente()" />
                    <asp:Button ID="Button_Editar_unidad_conservacion" runat="server" Text="Editar " Style="margin-top: 2px; margin-left: 5px" CssClass="boton_azul" ToolTip="Editar unidad de conservación" OnClientClick="tamano_ventana_editar_expediente()" />
                    <asp:Button ID="Button_eliminar_unidad_conservacion" runat="server"  Text="Eliminar"  Style="margin-top: 2px; margin-left: 5px" CssClass="boton_azul" ToolTip="Eliminar unidad conservación" OnClientClick="ConfirmMensajeGeneral('Desea eliminar la unidad','Hidden_promp_mensaje')" />
                    <asp:Button ID="Button_archivar_expediente_gestion" runat="server" Text="Archivar " Style="margin-top: 2px; margin-left: 5px; display:none" CssClass="boton_azul" ToolTip="Archivar unidad documental" />
                    <asp:Button ID="Button_desachivar_expediente_gestion" runat="server" Text="Desarrchivar " Style="margin-top: 2px; margin-left: 5px; display:none" CssClass="boton_azul" ToolTip="Desarrchivar unidad documental" OnClientClick="pront_confirmacion('Desea desachivar ?');" />
                    <asp:Button ID="Button_ubicacio_unidad_conservacion" runat="server" Text="Ubicación " Style="margin-top: 2px; margin-left: 5px" CssClass="boton_azul" ToolTip="Muestra Ubicación unidad documental" />
                    <asp:Button ID="ButtonRotulo" runat="server" Text="Descarga rotulo" CssClass="boton_azul" ToolTip="Descarga rotulo unidad conservación" />
                    <asp:Button ID="ButtonImprimirRotulo" runat="server" Text="Imprimir" Style="margin-top: 2px; margin-left: 5px" CssClass="boton_azul" ToolTip="Imprimir rotulo unidad conservacion" />
                    <asp:HiddenField ID="HiddenField_estado_ubicacion" runat="server" value=""/>
                    <input id="Hidden_ruta_archivo" type="hidden" value="" runat="server">
                    <input id="Hidden_result" type="hidden" value="NO" runat="server">
                    <input id="Hidden_promp_mensaje" type="hidden" value="0" runat="server">
                    <asp:Button ID="Button_actualizar_unidad" runat="server" Text="Button" style="display:none" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>     
     </div>
    </div>
         <!--ubicacion toponimica-->
       
        <asp:Panel ID="Panel_ubicacion_toponimica_expediente_popup" runat="server" Style="display: none; color: black; width: 80%; height: auto" CssClass="modal_content_general">
            <asp:ModalPopupExtender ID="ModalPopupExtende_ubicacion_toponimica_expediente_popup" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_ubicacion_toponimica_expediente_popup"
                PopupControlID="Panel_ubicacion_toponimica_expediente_popup" CancelControlID="Buttoncerrar_ubicacion_toponimica_expediente_popup">
            </asp:ModalPopupExtender>
            <div id="modal_content_Panel_ubicacion" class="modal-content">
                <div id="divcabecer_ubicacion_toponimica_expediente_popup" class="modal_title_superior_ modal-header">
                    <h6 class="modal-title d-inline ml-1">Ubicación toponimica</h6>
                    <button type="button" value="Buttoncerrar_ubicacion_toponimica_expediente_popup" class="close da_event_captive">&times;</button>
                </div>
                <div id="Contenido_ubicacion_toponimica_expediente" style="color: black; background-color: #FFFFFF; height: 97%; width: 100%" class="modal_content_back modal-body pt-0 pb-0 pr-0">
                    <asp:Panel ID="Paneltreview_u_b_t" runat="server" ScrollBars="Both"
                        Height="100%" Width="100%">
                        <asp:UpdatePanel ID="UpdatePanelViewArchivo_u_b_t" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:TreeView ID="TreeViewArchivo_u_b_t" runat="server" BackColor="white"
                                    PopulateNodesFromClient="False"
                                    LeafNodeStyle-CssClass="LeafNodeStyle" ForeColor="Black" HorizontalPadding="10px" Font-Size="12px" NodeIndent="5" ExpandDepth="0" SkipLinkText="">
                                    <HoverNodeStyle Font-Underline="False" />
                                    <LeafNodeStyle CssClass="LeafNodeStyle" HorizontalPadding="0px" NodeSpacing="0px" VerticalPadding="0px" />
                                    <NodeStyle HorizontalPadding="10px" NodeSpacing="5px" VerticalPadding="5px" ForeColor="Black" CssClass="nav-link-treview mt-2 mb-2 pl-2" />
                                    <ParentNodeStyle ChildNodesPadding="0px" CssClass="ParentNodeStyle" HorizontalPadding="5px" NodeSpacing="0px" VerticalPadding="0px" Font-Bold="true" />
                                    <RootNodeStyle ChildNodesPadding="0px" CssClass="RootNodeStyle" NodeSpacing="0px" VerticalPadding="1px" HorizontalPadding="10px" Font-Bold="true" />
                                    <SelectedNodeStyle HorizontalPadding="10px" CssClass="select_treview_boottra_ajustado  nav-link-treview" ImageUrl="~/workflow/imageneswf/iten_list_select.png" />
                                </asp:TreeView>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </asp:Panel>
                </div>
                <div id="contendor_botones_unidad_u_b_t"   class="border_inferior_radius_blanco_ modal-footer justify-content-end">
                    <asp:UpdatePanel ID="UpdatePanel_botones_unidad_u_b_t" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Button ID="Button_exportar" runat="server" Text="Imprimir" CssClass="btn btn-success" OnClientClick="fnExcelTre('TreeViewArchivo_u_b_t')" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div style="display: none; height: 1px">
                    <asp:Button ID="Button_ubicacion_toponimica_expediente_popup" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" Style="display: none" />
                    <asp:Button ID="ButtonSalir_ubicacion_toponimica_expediente_popup" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" Style="display: none" />
                    <asp:Button ID="Buttoncerrar_ubicacion_toponimica_expediente_popup" runat="Server" Text="" CssClass="invisible" Height="0px" Width="0px" Style="display: none" />
                </div>

            </div>
        </asp:Panel>
       
         <div id="modal_agregar_expediente">

            <asp:Panel ID="Panel_agregar_expdiente_popup" runat="server"  Style="display:none; color: White; width: 50%; height: 100%" CssClass="modal_content_general">
               
                <asp:ModalPopupExtender ID="ModalPopupExtende_agregar_expdiente_popup" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_agregar_expdiente_popup"
                    PopupControlID="Panel_agregar_expdiente_popup" CancelControlID="Buttoncerrar_agregar_expdiente_popup" >
                </asp:ModalPopupExtender>
                <div id="divcabecer_agregar_expdiente_popup" class="modal_title_superior" style="width:97%; display:none">
                    
                    <asp:Label ID="Label_agregar_expdiente_popup" runat="server" Text="Gestión expedientes" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton_agregar_expdiente_popup" style="float: right; display:none">
                         
                                 <asp:Button ID="Buttoncerrar_agregar_expdiente_popup" runat="Server" Text="X"
                                     ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" OnClientClick="cerrar_popup_agregar_expedinte()" />
                             
                    </div>
                </div>  
                 <div id="Contenido_agregar_expdiente_popup" style=" color: black; background-color:transparent; height: 100%; width: 100%" >
                     <asp:UpdatePanel ID="UpdatePanel_agregar_expdiente_popup" runat="server" UpdateMode="Conditional" style="height:100%" RenderMode="Inline">
                         <ContentTemplate>
                        <iframe  id="Iframe_agregar_expdiente_popup_"  runat="server"  style="width:100%; height:97%; background-color:transparent; border:solid 0px  #ccc"></iframe>
                             <input id="Hidden_estado_editar" type="hidden" value="NO" runat="server">
                             </ContentTemplate>
                         </asp:UpdatePanel>
                    </div>  
                    <asp:Button ID="Button_agregar_expdiente_popup" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_agregar_expdiente_popup" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
            </asp:Panel>
            
        </div>       
        <div id="modal_unidad_conservacion">
            <asp:Panel ID="Panel_agregar_unidad_conservacion" runat="server"  Style="display:none; width: 50%; height: 100%; overflow:hidden; margin:auto" CssClass="modal_content_general">           
                <asp:ModalPopupExtender ID="ModalPopupExtende_agregar_unidad_conservacion" runat="Server" BackgroundCssClass="FondoAplicacion"  TargetControlID="ButtonSalir_agregar_unidad_conservacion"
                    PopupControlID="Panel_agregar_unidad_conservacion" CancelControlID="Buttoncerrar_agregar_unidad_conservacion" >
                </asp:ModalPopupExtender>
                <div id="divcabecer_agregar_unidad_conservacion" class="modal_title_superior" style="width:99%; display:none">
                    
                    <asp:Label ID="Label_agregar_unidad_conservacion" runat="server" Text="Editar unidad de comservación" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton_agregar_unidad_conservacion" style="float: right">
                         
                                 <asp:Button ID="Buttoncerrar_agregar_unidad_conservacion" runat="Server" Text="X" CssClass="modal_boton_hiden" style="display:none"
                                      ToolTip="Cerrar ventana"  />
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" >
                            <ContentTemplate>
                                <asp:Button ID="Button_cancelar_agregar_unidad_conservacion" runat="server" Text="X" CssClass="modal_boton_hiden" ToolTip="Cerrar ventana" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                             
                    </div>
                </div>  
                <div id="Contenido_agregar_unidad_conservacion" style="background-color:transparent; height:auto; width:auto; overflow: hidden" class="modal_content_back">
                    <asp:UpdatePanel ID="UpdatePanel_agregar_unidad_conservacion" runat="server" UpdateMode="Conditional" style="height: 100%" RenderMode="Inline">
                        <ContentTemplate>
                            <iframe id="Iframe_agregar_unidad_conservacion_popup_" runat="server" style="width: 100%; height: 100%; overflow: hidden" scrolling="no" frameborder="0"></iframe>
                            <input id="Hidden_estado_editar_unidad" type="hidden" value="YES" runat="server"/>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>  
                <asp:Button ID="Button_agregar_unidad_conservacion" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" style="display:none" />
                <asp:Button ID="ButtonSalir_agregar_unidad_conservacion" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" style="display:none"/>
            </asp:Panel>
            
        </div>
       
        <!--archiva expediente-->
        <div id="modal_reubicar_unidad_expediente">

            <asp:Panel ID="Panel_reubicar_unidad_expediente_popup" runat="server"  Style="display:none; color: White; width: 50%; height: 99%">
               
                <asp:ModalPopupExtender ID="ModalPopupExtende_reubicar_unidad_expediente_popup" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_reubicar_unidad_expediente_popup"
                    PopupControlID="Panel_reubicar_unidad_expediente_popup" CancelControlID="Buttoncerrar_reubicar_unidad_expediente_popup" Y="0"></asp:ModalPopupExtender>
                <div id="divcabecer_reubicar_unidad_expediente_popup" class="cabecera2" style="width:99%">
                    <asp:Button ID="Button_reubicar_unidad_expediente_popup" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_reubicar_unidad_expediente_popup" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Label ID="Label_reubicar_unidad_expediente_popup" runat="server" Text="Gestión unidad contendora" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton_reubicar_unidad_expediente_popup" style="float: right">
                         
                                 <asp:Button ID="Buttoncerrar_reubicar_unidad_expediente_popup" runat="Server" Text="X"
                                     ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana"  />
                             
                    </div>
                </div>  
                 <div id="Contenido_reubicar_unidad_expediente_popup" style="border: thin double #000080; color: black; background-color: #FFFFFF; height: 97%; width: 99%; float:left">
                       <div id="drowlist_r_u_e" style="height:5%" >
		                 <asp:UpdatePanel ID="UpdatePanelEntidad_r_u_e" runat="server" UpdateMode="Conditional">
		                     <ContentTemplate>
		                          <asp:DropDownList ID="DropDownListEntidadEmpresa_r_u_e" runat="server"  Width="100%" onchange="buton_click('Button_listar_edificio');" ></asp:DropDownList>
		                     </ContentTemplate>
		                 </asp:UpdatePanel>
		                
                      </div>
                      <div id="div_treview_archivo_r_u_e" style="height:80%">
		                   <asp:Panel ID="Paneltreview_r_u_e" runat="server" ScrollBars="Both"
		                       Height="100%" Width="100%" Style="position: inherit">
		                       <asp:UpdatePanel ID="UpdatePanelViewArchivo_r_u_e" runat="server" UpdateMode="Conditional">
		                           <ContentTemplate>
		                                 <asp:TreeView ID="TreeViewArchivo_r_u_e" runat="server" BackColor="white"
		                           PopulateNodesFromClient="False" RootNodeStyle-CssClass="RootNodeStyle"
		                           ParentNodeStyle-CssClass="ParentNodeStyle"
		                           LeafNodeStyle-CssClass="LeafNodeStyle" ForeColor="Black" Font-Size="11px" NodeIndent="1" ExpandDepth="0">
		                           <HoverNodeStyle Font-Underline="False" />
		                           <LeafNodeStyle CssClass="LeafNodeStyle" HorizontalPadding="0px" NodeSpacing="0px" VerticalPadding="0px" />
		                           <NodeStyle ChildNodesPadding="0px" HorizontalPadding="0px" NodeSpacing="0px" VerticalPadding="0px" />
		                           <ParentNodeStyle ChildNodesPadding="0px" CssClass="ParentNodeStyle" HorizontalPadding="0px" NodeSpacing="0px" VerticalPadding="0px" />
		                           <RootNodeStyle ChildNodesPadding="0px" CssClass="RootNodeStyle" NodeSpacing="0px" VerticalPadding="0px" HorizontalPadding="0px" />
		                           <SelectedNodeStyle ForeColor="Red" />
		                       </asp:TreeView>
		                           </ContentTemplate>
		                       </asp:UpdatePanel>
		                     
		                   </asp:Panel>
                     </div>
                      <div id="contendor_botones_unidad_r_u_e" style="height:10%; background-color: #E7EDF5;border-color: #b0c4de; border-width:1px; border-style:ridge">
		                     <asp:UpdatePanel ID="UpdatePanel_botones_unidad_r_u_e" runat="server" UpdateMode="Conditional">
		                         <ContentTemplate>
		                             <asp:Button ID="Button_archivar" runat="server" Text="Archivar" CssClass="boton" style=" margin-left:10px"/>
		                         </ContentTemplate>
		                     </asp:UpdatePanel>
                     </div>
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
          <div id="Impresion_post">
                   <asp:Panel ID="Panelimpresionpost" runat="server" Style="display: none; color:black; width: auto; height: auto" CssClass="modal_content_general">
                       <asp:ModalPopupExtender ID="ModalPopupExtenderimpre_post" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_post"
                           PopupControlID="Panelimpresionpost" CancelControlID="Buttoncerrarimpre_post">
                       </asp:ModalPopupExtender>
                       <div id="modal_content_Panelimpresionpost" class="modal-content">
                           <div id="divcabecer2_post" class="modal_title_superior_ modal-header">
                               <h6 class="modal-title d-inline ml-1">Menú Impresión</h6>
                                <button type="button" value="Buttoncerrarimpre_post" class="close da_event_captive">&times;</button>   
                           </div>
                           <asp:UpdatePanel ID="UpdatePaneliframe_post" runat="server" UpdateMode="Conditional">
                               <ContentTemplate>
                                   <div id="ContenidoImpresion_post" style="color: black; background-color: #FFFFFF; height: auto; width: auto" class="modal_content_back">
                                       <iframe width="100%" height="100%" id="ifimpre_post_" runat="server" frameborder="0"></iframe>
                                   </div>
                               </ContentTemplate>
                           </asp:UpdatePanel>
                           <div style="display: none; height: 1px">
                               <asp:Button ID="Button1_post" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" Style="display: none" />
                               <asp:Button ID="ButtonSalir_post" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" Style="display: none" />
                               <asp:Button ID="Buttoncerrarimpre_post" runat="Server" Text="X" CssClass="" Height="0px" Width="0px" Style="display: none" />
                           </div>
                         
                       </div>
                   </asp:Panel>
        </div>    
         <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 200px">
                <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
                Processing ...
            </div>
        <div style="display:none">
            <asp:UpdatePanel ID="updatapanel_iframe" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <iframe runat="server" style="float: left" id="ifmExcel_" width="0" height="0" marginheight="0" marginwidth="0"
                        frameborder="0" />
                </ContentTemplate>

            </asp:UpdatePanel>
            </div>
    </form>
    
</body>
   
</html>
