<%@ Page Language="vb" AutoEventWireup="false" enableEventValidation="false" CodeBehind="WebFormEscan.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormEscan" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Scan and edit documentos</title>
    <meta http-equiv="Content-type" content="text/html;charset=UTF-8" />
    <meta http-equiv="Content-Language" content="en-us" />
    <meta http-equiv="X-UA-Compatible" content="requiresActiveX=true" />     
   <script src="../js/ui/jquery-3.4.1.min.js"></script>  
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
     <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
    <link href="../ccs/themes/base/jquery.ui.all.css" rel="stylesheet" type="text/css" />  
     <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <link href="../Styles/tabacordion.css" rel="stylesheet" />
    <script src="../js/java_general/general_code_java.js?v=20260827-compatible-events5"></script>
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>  
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />  
    <script src="../js/workflow/WebFormEscan.js" ></script>   
    <script type="text/javascript" src="../Resources/dynamsoft.webtwain.initiate.js"> </script>
    <script type="text/javascript" src="../Resources/dynamsoft.webtwain.config.js"> </script>
    <script src="../Resources/online_demo_operation.js"></script> 
    <script  src="../Resources/online_demo_initpage.js"></script>  
    <script src="../js/java_general/row_multiple_gred.js"></script>
    <script src="../js/validate_campos.js"></script>
    <script  src="../Awesome/js/all.js"></script>
    <link href="../Awesome/css/fontawesome.css" rel="stylesheet"/>
    <link href="../Awesome/css/brands.css" rel="stylesheet"/>
    <link href="../Awesome/css/solid.css" rel="stylesheet"/>
    <script  src="../Awesome/js/brands.js"></script>
    <script  src="../Awesome/js/solid.js"></script>
    <script  src="../Awesome/js/fontawesome.js"></script>
    <style>
    </style>
</head>
<body  style="background-color: white; overflow:hidden; height:auto"  >
       <form id="form1"  runat="server" onkeypress="return caracter_especial(event,this)"> 
         <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="True" AsyncPostBackTimeout="100000">
        </asp:ScriptManager>
        <script type="text/javascript" >
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
                progres_hiden('progres_bar');
                if (elment_postbak.type == "button" || elment_postbak.type == "submit") {
                    elment_postbak.value = value_element;
                    elment_postbak.disabled = false;
                }
                
                if (elment_postbak.id == "Button_guardar_documento") {
                    if (document.getElementById("Hidden_estado_guardar").value == "0") {
                        document.getElementById("Hidden_estado_guardar").value == "1";               
                    }
                    auto_zise_popup_lista_chequeo(document.getElementById("Hidden_0001").value);
                }
                if (elment_postbak.id == "Button_guardar_popup") {
                    if (document.getElementById("Hidden0001").value == "1") {
                        document.getElementById("Hidden0001").value = "-1";
                        //document.getElementById("Hidden21").value = "1";
                        document.getElementById("Hidden22").value = "";
                        Gurdar_documento_htpp_server(document.getElementById("Hidden22").value);
                    }
                }
                
                if (elment_postbak.id == "Button_añadir_popup") {
                    if (document.getElementById("Hidden00010").value == "1") {
                        document.getElementById("Hidden00010").value = "-1";
                        document.getElementById("Hidden21").value == "2";
                        Gurdar_documento_htpp_server(document.getElementById("Hidden22").value);                   
                    }
                }
                
                if (elment_postbak.id == "Button_Actualizar_Lista_chequeo") {
                    auto_zise_popup_lista_chequeo("1");
                }
                
            }

            </script>  
        <nav id="controles" class="navbar navbar-expand-sm nav_botota_person modal_content_no_back_inferior"  > 
               <button id="nav_togle_display" class="navbar-toggler" type="button" style=" background-color:#6d7fcc" data-toggle="collapse" data-target="#navbarNavDropdown">
                   <span class="navbar-toggler-icon_"><i style="color:white" class="fad fa-th-list"></i></span>
               </button>
         <div class="collapse navbar-collapse row" id="navbarNavDropdown" >
             <ul class="navbar-nav  col-md-12">
                 <li class="nav-item dropdown active ml-0 active_">
                     <a class="nav-link dropdown-toggle bot_hover_person" style="color: #6d7fcc" href="#" id="A1" data-toggle="dropdown" title="Opciones de digitalización" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc" class="fad fa-scanner-image fa-lg"></i>
                     </a>
                     <div class="dropdown-menu " aria-labelledby="navbarDropdownMenuLink">
                         <a class="dropdown-item " style="color: #6d7fcc" title="Digitalizar un nuevo documento"  href="#" onclick="acquireImage('1');"><span class="fa-stack">
                             <i style="color: #6d7fcc" class="fad fa-scanner-image  fa-stack-1x  "></i>
                             <i style="color: #6d7fcc; margin-left: -5px; float: right; margin-top: 5px" class="fal fa-file  fa-stack-1x "></i>
                         </span>Nuevo </a>
                         <a class="dropdown-item " style="color: #6d7fcc" title="Digitalizar y agregar página al final del documento" href="#" onclick="SaveScanSettings(); acquireImage('0');"><span class="fa-stack">
                             <i style="color: #6d7fcc" class="fad fa-scanner-image  fa-stack-1x  "></i>
                             <i style="color: #6d7fcc; margin-left: -5px; float: right; margin-top: 5px" class="fal fa-files-medical  fa-stack-1x "></i>
                         </span>Agregar </a>
                         <a class="dropdown-item " style="color: #6d7fcc" title="Digitalizar e insertar página antes de la página actual" href="#" onclick="acquireImage('2');"><span class="fa-stack">
                             <i style="color: #6d7fcc" class="fad fa-scanner-image  fa-stack-1x  "></i>
                             <i style="color: #6d7fcc; margin-left: -5px; float: right; margin-top: 5px" class="fad fa-copy  fa-stack-1x "></i>
                         </span>insertar </></a>
                         <a class="dropdown-item " style="color: #6d7fcc" title="Digitalizar y remplazar página actual" href="#" onclick="acquireImage('3');"><span class="fa-stack ">
                             <i style="color: #6d7fcc" class="fad fa-scanner-image  fa-stack-1x  "></i>
                             <i style="color: #6d7fcc; margin-left: -5px; float: right; margin-top: 5px" class="fal fa-file-minus fa-stack-1x "></i>
                         </span>Remplazar </></a>
                     </div>
                 </li>
                 <li class="nav-item dropdown active ml-0 active_">
                     <a class="nav-link nav-link-person " style="color: #6d7fcc" title="Guardar documento" href="#" onclick="activa_salvar_documento();"><i class="fad fa-save fa-lg"></i></a>
                 </li>
                 <li class="nav-item dropdown active ml-0 active_">
                     <a class="nav-link dropdown-toggle bot_hover_person" style="color: #6d7fcc" href="#" id="A2" data-toggle="dropdown" title="Opciones de rotación" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc" class="fad fa-sync-alt fa-lg"></i>
                     </a>
                     <div class="dropdown-menu " aria-labelledby="navbarDropdownMenuLink">
                         <a class="dropdown-item " style="color: #6d7fcc" title="Rotar 180" href="#" id="A8"  onclick="ini_rotate_paginas_miniaturas(event,'180');"><i class="fad fa-sync "></i> 180°</a>
                         <a class="dropdown-item " style="color: #6d7fcc" title="Rotar a la izquierda" href="#" onclick="ini_rotate_paginas_miniaturas(event,'90');"><i class="fad fa-undo "></i> 90°  </a>
                         <a class="dropdown-item " style="color: #6d7fcc" title="Rotar página a la derecha" href="#" onclick="ini_rotate_paginas_miniaturas(event,'-90');"><i class="fad fa-redo "></i> 90°  </></a>
                         
                     </div>
                 </li>
                 <li class="nav-item active ml-0 active_">
                     <a id="cursor_select" class="nav-link nav-link-person " style="color: #6d7fcc; border:inset" title="Cursor selección" href="#" onclick="btn_cursor(event,this);"><i class="fal fa-vector-square fa-lg"></i></a>
                 </li>
                 <li class="nav-item active ml-0 active_">
                     <a class="nav-link nav-link-person " style="color: #6d7fcc" title="Recorte de página" href="#" onclick="btnCrop_onclick(event,this);"><i class="fad fa-crop fa-lg"></i></a>
                 </li>
                  <li class="nav-item active ml-0 active_">
                     <a class="nav-link nav-link-person " style="color: #6d7fcc" title="Enderezar página" href="#" onclick="des_kew();  "><i class="fas fa-eraser fa-lg"></i></a>
                 </li>
                 <li class="nav-item active ml-0 active_">
                     <a class="nav-link nav-link-person " style="color: #6d7fcc" title="Acercar Imagen" href="#" onclick="mas_view_image(event,this);"><i class="fad fa-plus-circle fa-lg"></i></a>
                 </li>
                 <li class="nav-item active ml-0 active_">
                     <a class="nav-link nav-link-person " style="color: #6d7fcc" title="Alejar Imagen" href="#" onclick="menos_view_image(event,this);"><i class="fad fa-minus-circle fa-lg"></i></a>
                 </li>
                
                 <li class="nav-item active ml-0 active_">
                     <a class="nav-link nav-link-person " style="color: #6d7fcc" title="Primera  imagen" href="#" onclick="ini_lasImage_Image(event,this);"><i class="fad fa-arrow-alt-to-left fa-lg"></i></a>
                 </li>
                 <li class="nav-item active ml-0 active_">
                     <a class="nav-link nav-link-person " style="color: #6d7fcc" title="Anterior imagen" href="#" onclick="LasImage_Image(event,this);"><i class="fad fa-arrow-alt-left fa-lg"></i></a>
                 </li>
                 <li class="nav-item ">
                     <label id="Paginador" style="color: #000000; background: #FFFFFF; margin-top: 8px; margin-left: 4px; min-width: 30px; float: left"></label>
                     <label id="Contador" style="color: #000000; background: #FFFFFF; margin-top: 8px; margin-left: 2px; display: none; float: left"></label>
                 </li>
                 <li class="nav-item active  active_">
                     <a class="nav-link nav-link-person " style="color: #6d7fcc" title="Siguiente imagen" href="#" onclick="NextImage_Image(event,this);"><i class="fad fa-arrow-alt-right fa-lg"></i></a>
                 </li>
                 <li class="nav-item active ml-0 active_">
                     <a class="nav-link nav-link-person " style="color: #6d7fcc" title="Ultima  imagen" href="#" onclick="Fin_NextImage_Image(event,this);"><i class="fad fa-arrow-alt-to-right fa-lg"></i></a>
                 </li>
                
                 <li class="nav-item active ml-0 active_">
                     <a class="nav-link nav-link-person " style="color: #6d7fcc" title="Elimina imagen(es) seleccionada(s)" href="#" onclick="btnRemoveCurrentImage_onclick(event,this);"><i class="fad fa-file-times fa-lg text-warning"></i></a>
                 </li>
                 <li class="nav-item active ml-0 active_">
                     <input id="Text_buequeda" type="text" style="width: 60px" placeholder="ir a" onkeypress="return validate_numero(event,this)" />
                 </li>
                 <li class="nav-item active ml-0 active_">
                     <a class="nav-link nav-link-person " style="color: #6d7fcc" title="Ir a la imagen" href="#" onclick="Find_page_imagen(event,this);"><i id="I8" class="fad fa-search fa-lg"></i></a>
                 </li>
                 <li class="nav-item active ml-0 active_">
                     <a class="nav-link nav-link-person " style="color: #6d7fcc" title="Ver Thumbnail" href="#" onclick="Thumbnail_();"><i id="I2" class="fad fa-th-large fa-lg"></i></a>
                 </li>
                  <li class="nav-item dropdown active ml-0 active_">
                     <a class="nav-link dropdown-toggle bot_hover_person" style="color: #6d7fcc" href="#"  data-toggle="dropdown" title="Vista" aria-haspopup="true" aria-expanded="false"><i  style="color: #0062cc" class="" >  </i> <span id="a_tumb_pagina"> -1</span>
                      </a> 
                     <div class="dropdown-menu " aria-labelledby="navbarDropdownMenuLink">
                         <a class="dropdown-item " style="color: #6d7fcc" title="Vista libro" href="#"onclick="Sevalue_tumb_a(-1);"><i class="fal fa-book-open"></i> -1 x -1 </a>
                         <a class="dropdown-item " style="color: #6d7fcc" title="Vista 1 x 1" href="#" onclick="Sevalue_tumb_a(1);"><i class="fal fa-th-large "></i> 1 x 1  </a>
                         <a class="dropdown-item " style="color: #6d7fcc" title="Vista 2 x 2" href="#" onclick="Sevalue_tumb_a(2);"><i class="fal fa-th-large "></i> 2 x 2  </a>
                         <a class="dropdown-item " style="color: #6d7fcc" title="Vista 3 x 3" href="#" onclick="Sevalue_tumb_a(3);"><i class="fal fa-th-large "></i> 3 x 3  </a>
                         <a class="dropdown-item " style="color: #6d7fcc" title="Vista 4 x 4" href="#" onclick="Sevalue_tumb_a(4);"><i class="fal fa-th-large "></i> 4 x 4  </a>
                         <a class="dropdown-item " style="color: #6d7fcc" title="Vista 5 x 5" href="#" onclick="Sevalue_tumb_a(5);"><i class="fal fa-th-large "></i> 5 x 5  </a>
                         <a class="dropdown-item " style="color: #6d7fcc" title="Vista 6 x 6" href="#" onclick="Sevalue_tumb_a(6);"><i class="fal fa-th-large "></i> 6 x 6  </a>
                         <a class="dropdown-item " style="color: #6d7fcc" title="Vista 7 x 7" href="#" onclick="Sevalue_tumb_a(7);"><i class="fal fa-th-large "></i> 7 x 7  </a>
                         <a class="dropdown-item " style="color: #6d7fcc" title="Vista 8 x 8" href="#" onclick="Sevalue_tumb_a(8);"><i class="fal fa-th-large "></i> 8 x 8  </a>
                         <a class="dropdown-item " style="color: #6d7fcc" title="Vista 9 x 9" href="#" onclick="Sevalue_tumb_a(9);"><i class="fal fa-th-large "></i> 9 x 9  </a>
                         <a class="dropdown-item " style="color: #6d7fcc" title="Vista 10 x 10" href="#" onclick="Sevalue_tumb_a(10);"><i class="fal fa-th-large "></i> 10 x 10  </a>
                     </div>
                 </li>
                 <li class="nav-item active ml-2 active_  d-none">
                     <select id="Select_tumb" style="width: 50px; margin-left: 2px; margin: 2px; float: left" onchange="Sevalue_tumb(event,this);" title="numero de filas y columnas vista miniatura">
                         <option>-1</option>
                         <option>1</option>
                         <option>2</option>
                         <option>3</option>
                         <option>4</option>
                         <option>5</option>
                         <option>6</option>
                         <option>7</option>
                         <option>8</option>
                         <option>9</option>
                         <option>10</option>
                     </select>
                 </li>
                 <li class="nav-item dropdown active ml-0 mr-0 active_">
                     <a class="nav-link  dropdown-toggle" style="color: #6d7fcc" href="#" id="A7" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc" class="fal fa-link"></i>
                     </a>
                     <div class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">
                         <a class="dropdown-item " style="color: #6d7fcc" title="Descargar imagenes" href="#" onclick="save_file(event,this);"><i id="I9" class="fad fa-file-download fa-lg"></i> Descargar imagenes</a>
                         <a class="dropdown-item " style="color: #6d7fcc" title="Cargar imagen desde su dispositivo" href="#" onclick="load_file(event,this);"><i id="I10" class="fad fa-file-upload fa-lg"></i> Cargar imagenes</a>
                         <a class="dropdown-item " style="color: #6d7fcc" title="Cargar imagen seleccionada en el sistema" href="#" onclick="http_upload_server(event,this);"><i id="I11" class="fad fa-file-code fa-lg"></i> Cargar imagenes desde el sistema</a>
                         <a class="dropdown-item " style="color: #6d7fcc" title="Añadir imagenes al documento seleccionado" href="#" onclick="activa_add_documento();"><i id="I1" class="fad fa-file-plus fa-lg"></i> Añadir a la imagen seleccionada</a>
                     </div>
                 </li>
                 <li class="nav-item active ml-2 active_">
                     <a class="nav-link nav-link-person " id="a_title" style="color: #6d7fcc" title="Oculta configuración digitalización" href="#" onclick="event_click_indice();"><i id="indice_title" class="fad fa-bars fa-lg"></i></a>
                 </li>
             </ul>

         </div>
        </nav>           
         <div id="conte_visor" style="width: 100%; height: 100%; margin-left: 1px" class="p-0 row">
        <div id="container_visor" style="height: 100%; width: 100%; float: left" class="col-8 p-0">
            <div id="dwtcontrolContainer" style="width:100%; height:100%; padding-bottom:5px; border:none">
            </div>     
        </div>            
        <div id="rigt" class="col-4 p-0" style="height: 98%; width: 100%; float: right; background-color: white;margin-top: 1px; border-top: 1px solid #e9ecef; border-style: ridge; border-width: 1px; overflow:auto; position:inherit; display:none">
            <!---<div id="table" style="float: left"> !--->
            <div id="Separacion" style="width: 100%;  margin-left: 0px; display:none" >
                <asp:Label ID="Label3" runat="server" Text="Digitalización"  style="font-family:Arial; font-size:14px; font-weight:600"></asp:Label>
            </div>           
            <div id="div_ScanImage" style="width: 100%; background-color: white" class="p-2">                
                <button onclick="prevent(event,this);" style="" class="accordion "><i class="fad fa-tools fa-lg"></i> Configuración de digitalización</button>
                <div class="panel " style=" content: contents">
                   <div class="row">
                       <div class="col-12">
                           <select size="1" id="source" class="mt-1 custom-select mr-sm-2" style="position: relative; width: 100%;" onchange="source_onchange()">
                               <option value=""></option>
                           </select>
                       </div>
                   </div>
                    <div class="row " style="background-color:#f8f9fa">
                        <div class="col-12" style="text-align: center">
                            <b class="mr-1 mt-1 h6 font-weight-light">Dispositivo:</b>
                        </div>
                    </div>
                   <div class="row p-1">
                       <div class="col-4">
                           <label for='Interface escaner' class="h6 font-weight-light " title="Activa contrlolador propio de escaner">
                               <input type='checkbox' id='ShowUI' />
                               Driver</label>
                       </div>
                       <div class="col-4">
                           <label for='ADF' class="mr-1 h6 font-weight-light" title="Activa alimentador automatico de escaner">
                               <input type='checkbox' id='ADF' />
                               ADF</label>
                       </div>
                       <div class="col-4" >
                             <label for='Duplex' class="ml-1 h6 font-weight-light" title="Activa escaneo en ambas caras">
                               <input type='checkbox' id='Duplex' />
                               Duplex</label>
                       </div>
                   </div>
                   
                    <div class="row" style="background-color:#f8f9fa">
                        <div class="col-12" style="text-align: center">
                            <b class="mr-1 mt-1 h6 font-weight-light">Pixel Tipo:</b>
                        </div>
                    </div>
                    <div class="row p-1">
                        <div class="col-4">
                            <label for='BW' id="BWL" class="ml-1 font-weight-light" style="display:none" title="Activa escaneo mono cromatico ">
                                <input type='radio' id='BW' name='PixelType' class="mr-1"  />
                                Mono 
                            </label>
                        </div>
                        <div class="col-4">
                            <label for='Gray' id="GrayL" class="ml-1 font-weight-light" style="display:none" title="Activa escaneo escala de grises ">
                                <input type='radio' id='Gray' class="mr-1" name='PixelType' style="display:none"/>
                                Grises</label>
                        </div>
                         <div class="col-4" >
                            <label for='RGB' id="RGBL" class="ml-1 font-weight-light" style="display:none" title="Activa escaneo a color ">
                                <input type='radio' id='RGB' class="mr-1" name='PixelType' style="display:none"/>
                                Color</label>
                        </div>
                    </div>
                    <div class="row" style="background-color:#f8f9fa">
                        <div class="col-12" style="text-align: center">
                            <b class="mr-1 mt-1 h6 font-weight-light">Auto correción:</b>
                        </div>
                    </div>
                    <div class="row p-1">
                        <div class="col-4">
                            <label for='RGB' class="mt-1 h6 font-weight-light" title="Descarta página en blanco">
                                <input type='checkbox' id='Radio_pag_blank'  checked="checked" />
                                Page blank </label>
                        </div>
                        <div class="col-4">
                            <label for='DESK' class="mt-1 h6 font-weight-light" title="Corrige desviación">
                                    <input type='checkbox' id='des_kew' name='DeskType' /> DesKew  </label>
                        </div>
                        <div class="col-4">
                              <label for='BORDER' class="mt-1  h6 font-weight-light" title="Detección automática del borde">
                                    <input type='checkbox' id='border_detect' name='BorderType' /> Border </label>
                        </div>
                    </div>
                   
                    <div class="row">
                        <div class="col-12" style="text-align: center">
                             <span class="h6 font-weight-light">Resolución</span>
                                <select class="custom-select "  style="width: 70px" size='1' id='Resolution'>
                                    <option value='300'></option>
                                </select>
                        </div>
                    </div>
                </div>
                <button onclick="prevent(event,this);" class="accordion " ><i class="fas fa-scanner-image fa-1x"></i> Opciones de digitalización</button>
                <div class="panel">
                    <div id="div_opciones_digita" style="margin-top: 1px; border-color: #b0c4de; border-width: 1px; border-style: ridge">
                        <div style="overflow: auto" class="p-1">
                            <table style="width: 99%">
                                <tr>
                                    <td>
                                        <button type="button" value="" style="width: 100%; text-align:start" class="btn  btn-light  m-1 font-weight-light" title="Digitalizar un nuevo documento" onclick="acquireImage('1');">
                                            <span class="fa-stack">
                                                <i style="color: #6d7fcc" class="fas fa-scanner-image  fa-stack-1x  "></i>
                                                <i style="color: #6d7fcc; margin-left:-5px; float:right; margin-top:5px" class="fal fa-file  fa-stack-1x "></i>
                                            </span> Nuevo
                                        </button>
                                        
                                    </td>
                                    <td>
                                        <button type="button" value="" style="width: 100%; text-align:start" class="btn btn-light  m-1 font-weight-light" onclick="SaveScanSettings(); acquireImage('0');" title="Digitalizar y agregar página al final del documento">                                             
                                            <span class="fa-stack">
                                                <i style="color: #6d7fcc" class="fas fa-scanner-image  fa-stack-1x  "></i>
                                                <i style="color: #6d7fcc; margin-left:-5px; float:right; margin-top:5px" class="fal fa-files-medical  fa-stack-1x "></i>
                                            </span> Agregar </button>
                                        
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <button type="button" value="" style="width: 100%; text-align:start" class="btn btn-light  m-1 font-weight-light" onclick="acquireImage('2');" title="Digitalizar e insertar página antes de la página actual"> 
                                            <span class="fa-stack">
                                                <i style="color: #6d7fcc" class="fas fa-scanner-image  fa-stack-1x"></i>
                                                <i style="color: #6d7fcc; margin-left: -5px; float: right; margin-top: 5px" class="fad fa-copy  fa-stack-1x "></i>
                                            </span> Insertar 
                                         </button>
                                    </td>
                                    <td>
                                        <button type="button" value="" style="width: 100%; text-align: start" class="btn btn-light  m-1 font-weight-light" onclick="acquireImage('3');" title="Digitalizar y remplazar página actual">
                                            <span class="fa-stack ">
                                                <i style="color: #6d7fcc" class="fas fa-scanner-image  fa-stack-1x  "></i>
                                                <i style="color: #6d7fcc; margin-left: -5px; float: right; margin-top: 5px" class="fal fa-file-minus fa-stack-1x "></i>
                                            </span> Remplazar
                                        </button>
                                        
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </div>
                <button onclick="prevent(event,this);" class="accordion " style=""> <i class="far fa-hdd fa-1x"></i> Guardar</button>
                <div class="panel">
                    <div id="div_opciones_guardar" style="margin-top: 1px ;  text-align: center" >      
                        <div id="div_boton_guardar" style="width: 100%">
                            <asp:UpdatePanel ID="UpdatePanel_guadar" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div class="row_">
                                        <div class="col-12 p-0">
                                            <button type="button" value="" style="width: 98%; text-align: center" class="btn btn-light  m-1 font-weight-light" onclick="activa_salvar_documento();" title="Guardar documento en el servidor">
                                                <span class="fa-stack">
                                                    <i style="color: #6d7fcc" class="fal fa-save  fa-stack-1x"></i>
                                                    <i style="color: #6d7fcc; margin-left: 1px; float: right; margin-top: 3px" class="fad fa-file  fa-stack-1x "></i>
                                                </span> Guardar 
                                            </button>
                                        </div>
                                       
                                    </div>
                                    <input id="Hidden_estado_guardar" type="hidden" value="0" runat="server"/>
                                    <asp:Button ID="Button_guardar_documento" runat="server" Text="Guardar" ToolTip="Guardar documento" class="boton_azul" Style="margin-top: 3px; display:none" />
                                    <asp:Button ID="Button_adjuntar" runat="server" Text="Añadir" ToolTip="Añade a documento seleccionado" class="boton_azul" Visible="false" Style="margin-top: 3px; display:none" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                        <div id="div_contendor_estado" style="width: 100%; margin-left: 0px; text-align: left">
                            <asp:Label ID="Label_estado_lista" runat="server" Text="" Style="font-family: Arial; font-size: 10px"></asp:Label>
                        </div>
                    </div>
                </div>         
            </div>
        </div>
        
    </div>  
    <asp:Panel ID="Panel_guarda_servidor" runat="server" Style="display:none;  width: 50%; height: 100%" CssClass="modal_content_general">
               <asp:ModalPopupExtender ID="ModalPopupExtenderimpre_guarda_servidor" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_guarda_servidor"
                   PopupControlID="Panel_guarda_servidor" CancelControlID="Buttoncerrarimpre_guarda_servidor"  Y="1">
               </asp:ModalPopupExtender>
               <div id="modal_content_guarda_servidor" class="modal-content">
                   <div id="divcabecer2__guarda_servidor" class="modal_title_superior_ modal-header">
                       <h6 class="modal-title d-inline ml-1">Guardar como</h6>
                       <button type="button" value="Buttoncerrarimpre_guarda_servidor" class="close da_event_captive">&times;</button>
                   </div>
                   <div id="contenido_guarda_servidor" style="background-color: white; width: auto; height: auto;  border-top:none" class="modal_content_back pl-2 pr-2 pt-2">
                       <div id="contenido_text_nombre" style="width: 100%">
                            <asp:TextBox ID="TextBox_nombre" runat="server" Style="width:100%" CssClass="mb-2" placeholder="Digita nombre"></asp:TextBox>
                       </div>     
                        <asp:UpdatePanel ID="UpdateGeneral" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                <ContentTemplate>
                                    <input id="hdnTipoTramite" type="hidden" value="0" runat="server"/>
                                    <input id="hdnEmailID" type="hidden" value="0" runat="server"/>
                                    <input id="hdnEmailID_VAL" type="hidden" value="0" runat="server"/>
                                    <input id="HiddenEmailconsulta" type="hidden" value="" runat="server"/>
                                    <input id="Hidden_0001" type="hidden" value="1" runat="server"/>
                                    <asp:Button ID="Button_Actualizar_Lista_chequeo" runat="server" Text="&#8634; Actualizar" Style="float: right; height: 18px; font-size: 10px; background-color: white; font-weight: 500; display: none" ToolTip="Actualizar lista de chequeo" /> 
                                    <asp:Panel ID="Panel_principal" runat="server"
                                        Width="100%"  Style="overflow: auto; min-width:50px">
                                        <asp:GridView ID="data_grid" runat="server" Style="position: inherit" AutoGenerateSelectButton="False" CssClass="filtrar font-weight-light" GridLines="None"  Width="100%">
                                            <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                            <HeaderStyle CssClass="GridviewScrollHeader_line_boot" />
                                            <RowStyle CssClass="GridviewScrollItem_line" />
                                            <PagerStyle CssClass="GridviewScrollPager_line" />
                                        </asp:GridView>
                                    </asp:Panel>
                                </ContentTemplate>

                                <Triggers>
                                </Triggers>
                            </asp:UpdatePanel>
                       <div id="div_opciones_formato" style="width: 100%" class="mt-1 mb-1 text-center p-1">
                           <div class="row">
                               <div class="col-5">
                                   <b class="font-weight-light h6">Tipo formato:</b>
                               </div>
                               <div class="col-7">
                                   <div class="row">
                                       
                                       <div class="4">
                                           <label class="font-weight-light" id='PDFC' for='PDF'>
                                               <input type='radio' id='PDF' name='TypeFormato' value="PDF" class="m-2 mr-1" />PDF</label>
                                       </div>
                                       <div class="4">
                                           <label for='PDFA' id='PDFAC'>
                                               <input type='radio' id='PDFA' name='TypeFormato' class="m-2 mr-1" value="PDFA" />PDF/A</label>
                                       </div>
                                       <div class="4">
                                           <label class="font-weight-light" id='TIFC' for='TIF'>
                                               <input type='radio' id='TIF' name='TypeFormato' value="TIF" class="m-2" />TIF
                                           </label>
                                       </div>
                                   </div>
                                           
                               </div>
                           </div>
                           
                           
                       </div>
                   </div>                  
                   <div class="modal-footer  justify-content-end" id="content_boton_guarda_servidor">
                       <asp:UpdatePanel ID="UpdatePanelguarda_servidor" runat="server" UpdateMode="Conditional">
                           <ContentTemplate>
                               <asp:Button ID="Button_guardar_popup" runat="server" Text="Aceptar" ToolTip="Guardar documento en el servidor" class="btn btn-success" />
                               <asp:Button ID="Button_cancelar_popup" runat="server" Text="Cancelar" ToolTip="Cancelar" class="btn btn-secondary" />
                               <input id="Hidden0001" type="hidden" value="-1" runat="server"/>
                           </ContentTemplate>
                           <Triggers>
                           </Triggers>
                       </asp:UpdatePanel>
                   </div>
                   <div style="display: none; height: 1px">
                       <asp:Button ID="ButtonSalir_guarda_servidor" CssClass="invisible" Style="display: none" runat="server" Text="" />
                       <asp:Button ID="Button1_guarda_servidor" CssClass="invisible" Style="display: none" runat="server" Text="" Height="0px" Width="0px" />
                       <asp:Button ID="Buttoncerrarimpre_guarda_servidor" runat="Server" Text="" Style="display: none" CssClass="invisible" />
                   </div>
                   
               </div>
           </asp:Panel>

           <asp:Panel ID="Panel_adjunta_servidor" runat="server"  Style="display:block; color: White; width: auto; height: auto" CssClass="modal_content_general">
               <asp:ModalPopupExtender ID="ModalPopupExtenderimpre_adjunta_servidor" runat="Server" BackgroundCssClass="ModalBackgroud_gorund" TargetControlID="ButtonSalir_adjunta_servidor"
                   PopupControlID="Panel_adjunta_servidor" CancelControlID="Buttoncerrarimpre_adjunta_servidor">
               </asp:ModalPopupExtender>
               <div id="modal_content_adjunta_servidor" class="modal-content">
                   <div id="divcabecer2__adjunta_servidor" class="modal_title_superior_ modal-header">
                       <h6 class="modal-title d-inline ml-1">Archivar</h6>
                       <button type="button" value="Buttoncerrarimpre_adjunta_servidor" class="close da_event_captive mr-1">&times;</button>
                   </div>
                   <div id="contenido_procesa_usu_rel_solicitud" style="background-color: white; width: 100%; height: 100%; border-top: none" class="modal_content_back modal-body">    
                                   <br />
                                   <asp:Label ID="Label4" runat="server" Text="Desea añadir el archivo digitalizado al documento seleccionado?" Style="color: black; width: 200px; margin-left: 10px"></asp:Label>
                                   <br />
                                   <br />                   
                   </div>
                   <div class="modal-footer justify-content-end" id="footer_adjunta_servidor">
                       <asp:UpdatePanel ID="UpdatePaneladjunta_servidor" runat="server" UpdateMode="Conditional">
                           <ContentTemplate>
                               <asp:Button ID="Button_añadir_popup" runat="server" Text="Aceptar" ToolTip="Añadir documento digitalizado" class="btn btn-success" />
                               <asp:Button ID="Button_cancelar_popup_adjunta" runat="server" Text="Cancelar" ToolTip="Cancelar" class="btn btn-secondary" />
                               <asp:Button ID="Button_añade_documento" runat="server" Text="Aceptar" ToolTip="Añadir documento digitalizado" class="boton_azul" Style="display: none" />
                               <input id="Hidden00010" type="hidden" value="-1" runat="server"/>
                           </ContentTemplate>
                       </asp:UpdatePanel>
                   </div>
                   <div style="display: none; height: 1px">
                       <asp:Button ID="Button1_adjunta_servidor" CssClass="invisible" runat="server" Style="display: none" Text="" Height="1px" Width="1px" />
                       <asp:Button ID="ButtonSalir_adjunta_servidor" CssClass="invisible" Style="display: none" runat="server" Text="" />
                       <asp:Button ID="Buttoncerrarimpre_adjunta_servidor" runat="Server" Style="display: none" Text="" CssClass="invisible" />
                   </div>
               </div>
             </asp:Panel>
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
            <asp:UpdatePanel ID="UpdatePanel_tipo_save" runat="server" UpdateMode="Conditional"
                  >
                <ContentTemplate>
                    <input id="Hidden21" type="hidden" value="1" runat="server"/>
                    <input id="Hidden22" type="hidden" value="" runat="server"/>
                    <input id="HiddenRuta" type="hidden" value="0" runat="server"/>
                    <input id="HiddenIdFlujo" type="hidden" value="0" runat="server"/>
                </ContentTemplate>
                </asp:UpdatePanel>
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
    <div id="ImgSizeEditor" style="visibility:hidden; display:none; text-align:left;">	
        <ul>
            <li><label for="img_height"><b>New Height :</b>
                <input type="text" id="img_height" style="width:50%;" size="10"/>pixel</label></li>
            <li><label for="img_width"><b>New Width :</b>&nbsp;
                <input type="text" id="img_width" style="width:50%;" size="10"/>pixel</label></li>
            <li>Interpolation method:
                <select size="1" id="InterpolationMethod"><option value = ""></option></select></li>
            <li style="text-align:center;">
                <input type="button" value="   OK   " id="btnChangeImageSizeOK" onclick ="btnChangeImageSizeOK_onclick();"/>
                <input type="button" value=" Cancel " id="btnCancelChange" onclick ="btnCancelChange_onclick();"/></li>
        </ul>
    </div>

    <div id="Crop" style="visibility:hidden ; display:none">	
        <div style="width:50%; height:100%; float:left; text-align:left;">
            <ul>
                <li><label for="img_left"><b>left: </b>
                    <input type="text" id="img_left" style="width:50%;" size="4"/></label></li>
                <li><label for="img_top"><b>top: </b>
                    <input type="text" id="img_top" style="width:50%;" size="4"/></label></li>
                <li style="text-align:center;">
                    <input type="button" value="  OK  " id="btnCropOK" onclick ="btnCropOK_onclick()"/></li>
            </ul>
            </div>
            <div style="width:50%; height:100%; float:left; text-align:right;">
            <ul>
                <li><label for="img_right"><b>right : </b>
                    <input type="text" id="img_right" style="width:50%;" size="4"/></label></li>
                <li><label for="img_bottom"><b>bottom:</b>
                    <input type="text" id="img_bottom" style="width:50%;" size="4"/></label></li>
                <li style=" text-align:center;">
                    <input type="button" value="Cancel" id="cancelcrop" onclick ="btnCropCancel_onclick()"/></li>
            </ul>
        </div>
    </div>        
    <div id="preload" class="ModalBackgroud_gorund" style="visibility:hidden;background-color:#999999;height:22px;width:100px; z-index:400000">| | | | | | | | | | | | | | | procesando . . .</div>            
    <script type="text/javascript">
        $(function () {
            pageonload();
        });
               
</script>
    <script>
        
</script>
   </form>        
</body>
</html>
