<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormRadicacion.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormRadicacion" EnableEventValidation="false" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, 
Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI" 
TagPrefix="asp" %> 
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <style type="text/css">
  
        .invisible { 
            visibility: hidden; 
        } 

  .header
  {
    overflow: auto;
    position:absolute; 
    background-color:White;
       
  }
  .FondoAplicacion
    {
        background-color: Gray;
        filter: alpha(opacity=70);
        opacity: 0.7;
    }
    
    
     .cabecera2{
    height : 20px;
    /*position : static;*/
    margin: 0px;
    padding: 0px;
    background: #053061;
    /*width: 100%;*/
    color:White;
    text-align:left;
	top: 0px;
	left: 0px;
     }
     .cabecera{
    height : 7%;
    /*position : static;*/
    margin: 0px;
    padding: 0px;
    background: #053061;
    width: 100%;
    color:White;
    text-align:left;
	top: 0px;
	left: 0px;
}

.cabecera3{
    height : 13%;
    /*position : static;*/
    margin: 0px;
    padding: 0px;
    background: #053061;
    width: 250px;
    color:White;
    text-align:left;
	top: 0px;
	left: 0px;
}

  .cabecera4{
    height : 5%;
    /*position : static;*/
    margin: 0px;
    padding: 0px;
    background: #053061;
    width: 99%;
    color:White;
    text-align:left;
	top: 0px;
	left: 0px;
}

 
</style>
    <title></title>
     <script src="../js/ui/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
   <script src="../js/ScrollableGrid.js" type="text/javascript"></script>
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
    <script type="text/javascript">
       
        $(function () {

            //filtra los campos solo lectura
            $(document).on('keypress', function (e) {
                var id_element = e.srcElement.id;
                
                
                if (id_element == "DIRECIONDESTINATARIO" + "|" + "Direccion_Dest" + "|" + "VARCHAR") {
                     return false;
                }

                if (id_element == "MUNICIPIO|MUNICIPIO" + "|" + "VARCHAR") {
                    return false;
                }

                if (id_element == "TELEFONO" + "|" + "Telefono_Dest" + "|" & "VARCHAR") {
                    return false;
                }
            });
            $(document).on('keydown', function (e) {
                var id_element = e.srcElement.id;


                if (id_element == "DIRECIONDESTINATARIO" + "|" + "Direccion_Dest" + "|" + "VARCHAR") {
                    if (e.which == 8 || e.which == 46) {
                        return false;
                    }
                }

                if (id_element == "MUNICIPIO|MUNICIPIO" + "|" + "VARCHAR") {
                    if (e.which == 8 || e.which == 46) {
                        return false;
                    }
                    
                }

                if (id_element == "TELEFONO" + "|" + "Telefono_Dest" + "|" & "VARCHAR") {
                    if (e.which == 8 || e.which == 46) {
                        return false;
                    }
                }
            });
            //Filtra los campos enteros para que nos escriba letra
            $(document).on('keypress', function (e) {
                var id_element = e.srcElement.id;
                var id_element_spli = id_element.split("|")
                if (id_element_spli[2] == "INT" || id_element_spli[2] == "DATE") {
                    if (e.charCode < 48 || e.charCode > 57) return false;
               }
            });

            //Formatea los campos tipo fecha
            $(document).on('keydown', function (e) {
                if (e.which == 9) {
                    var id_element = e.srcElement.id;
                    var matr_id = id_element.split("|");
                    var salidadato;
                    if (matr_id[2] == "DATE") {
                        var dato = e.srcElement.value;

                        
                        if (dato == "") {
                            
                            return false;
                        }


                        if (salidadato == "Formato fecha no cumple") {
                            alert(salidadato);
                            e.preventDefault();
                            return false;
                        }
                        var BisestA;
                        var Año_F, Mes_f, Dia_f, tip;
                        var numerocaracter = dato.length;
                        if (numerocaracter == 10 || numerocaracter == 8) {

                        }
                        else {
                            alert ("Formato fecha no cumple");
                            e.preventDefault();
                            return false;
                        }

                        if (numerocaracter == 10) {

                            Año_F = dato.substring(0, 4);
                            Mes_f = dato.substring(0, 7);
                            Mes_f = Mes_f.substring(7, 5);
                            Dia_f = dato.substring(8, 10);
                        }
                        else {
                            Año_F = dato.substring(0, 4);
                            Mes_f = dato.substring(0, 6);
                            Mes_f = Mes_f.substring(6, 4);
                            Dia_f = dato.substring(6, 8);
                        }

                        //Verifica el formato del dia
                        if (Dia_f > 31 || Dia_f == 0) {

                            alert( "ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                            e.preventDefault();
                            return false;
                        }

                        //verifica el formato del mes
                        if (Mes_f > 12 || Mes_f < 1) {
                            alert( "EM_" + Año_F + "(" + Mes_f + ")" + Dia_f);
                            e.preventDefault();
                            return false;
                        }

                        switch (Mes_f) {
                            case "01":
                                if (Dia_f > 31) {
                                    alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                                    e.preventDefault();
                                }
                                break;

                            case "02":
                                if (Dia_f % 4 == 0) {

                                    BisestA = 29;
                                }
                                else {
                                    BisestA = 28;
                                }
                                if (Dia_f > BisestA) {
                                    alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                                    e.preventDefault();
                                }
                                break;
                            case "03":
                                if (Dia_f > 31) {
                                    alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                                    e.preventDefault();
                                }
                                break;

                            case "04":
                                if (Dia_f > 30) {
                                    alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                                    e.preventDefault();
                                }
                                break;

                            case "05":
                                if (Dia_f > 31) {
                                    alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                                    e.preventDefault();
                                }
                                break;

                            case "06":
                                if (Dia_f > 30) {
                                    alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                                    e.preventDefault();
                                }
                                break;

                            case "07":
                                if (Dia_f > 31) {
                                    alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                                    e.preventDefault();
                                }
                                break;

                            case "08":
                                if (Dia_f > 31) {
                                    alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                                    e.preventDefault();
                                }
                                break;

                            case "09":
                                if (Dia_f > 30) {
                                    alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                                    e.preventDefault();
                                }
                                break;

                            case "10":
                                if (Dia_f > 31) {
                                    alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                                    e.preventDefault();
                                }
                                break;

                            case "11":
                                if (Dia_f > 30) {
                                    alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                                    e.preventDefault();
                                }
                                break;

                            case "12":
                                if (Dia_f > 31) {
                                    alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                                    e.preventDefault();
                                }
                                break;
                        }

                        if (numerocaracter == 8) {
                            salidadato =  Año_F + "/" + Mes_f + "/" + Dia_f;
                            e.srcElement.value = salidadato;
                        }

                        if (numerocaracter == 10) {
                            salidadato = Año_F + "/" + Mes_f + "/" + Dia_f;
                            e.srcElement.value = salidadato;
                        }
                       
                    }
                }
            });

            $('#<%=TableControles.ClientID%> input:text').each(function () {
                var rethis = $(this).text();
                 var tablad = "";
                 var campo = "";
                
                 $(document).on('keydown',function(e){
                     var f = e.srcElement;
                     rethis = f.value;
                     tablad = $('#HiddenPlantilla').val();
                     campo = f.id;
                     if (e.srcElement.id == "Remitente_Cor" + "|" + "Destinatario_Cor" + "|VARCHAR") {
                         tablad = "destinatario_externo";
                         campo = "Nombre_Remitente";
                     }
                     
                 });
                 $(document).on('keyup', function (e) {
                     var f = e.srcElement;
                     rethis = f.value;
                     tablad = $('#HiddenPlantilla').val();
                     campo = f.id;
                     if (e.srcElement.id == "Remitente_Cor" + "|" + "Destinatario_Cor" + "|VARCHAR") {
                         tablad = "destinatario_externo";
                         campo = "Nombre_Remitente";
                     }
                 });
                 $(this).autocomplete({
                     
                     source: function (request, response) {
                         $.ajax({
                             type: "POST",
                             contentType: "application/json; charset=utf-8",
                             url: "../webservice/WebServiceRadicacion.asmx/GetGuiaRadicacon",
                             data: "{'DName':'" + rethis + "'," + "'DAcampo':'" + campo + "'," + "'DNtable':'" + tablad + "'}",
                             dataType: "json",
                             success: function (data) {
                                 response(data.d);
                                 
                             },
                             error: function (result) {
                                 alert("Error......" + result);
                             }
                         });
                     }
                 });
             });
        });
        function limpiarCampos() {
            $('#<%=TableControles.ClientID%> input:text').each(function () {

               $(this).val("");
               
            });
            $('#<%=TableControles.ClientID%> textarea').each(function () {

                $(this).val("");

            });
        }
       
        function ConfirmMensaje(mensaje) {
            var x;
            var r = confirm(mensaje);
            if (r == true) {
                x = "0";
            }
            else {
                x = "1";
            }
            document.getElementById("HiddenPROMP").value = x;
        }

        function llenardepartamento() {
            //alert("ojo");
            var drowplist = document.getElementById("ComboBoxEditPaisDesExt");
            var idsel = document.getElementById("Hiddenselecionpais");
            idsel.value = drowplist.options[drowplist.selectedIndex].text;
            var boton = document.getElementById("Buttonllenardepartamento");
            var idsel2 = document.getElementById("Hiddenselecionciudad");
            idsel2.value = "";
            boton.click();
           
        }
        function llenarciudad() {
            //alert("ojo");
            var drowplist = document.getElementById("ComboBoxEditDepartDestExt");
            var idsel = document.getElementById("Hiddenselecionciudad");
            idsel.value = drowplist.options[drowplist.selectedIndex].text;
            var boton = document.getElementById("Buttonllenarciudad");
            boton.click();

        }

        function seleccionmuicipio() {
            var drowplist = document.getElementById("DropDownListciudad");
            var idsel = document.getElementById("Hiddenmunicipio");
            idsel.value = drowplist.options[drowplist.selectedIndex].text;
            
        }
</script>
</head>
<body>
   
    
    <form id="formrotulo" runat="server" >
        
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="True">
        </asp:ScriptManager>
       
        <div id="contenguia" class="contenguia" style="width:95%; float:left;background-color:#E7EDF5" >
            <asp:Panel ID="PanelTitulo"   BorderStyle="Solid" BackColor="#E7EDF5" ForeColor="Black" runat="server" ScrollBars="None" EnableViewState="true"  >
                 
            </asp:Panel>

            <asp:Panel ID="PanelRadicacion"  runat="server" ScrollBars="Both"  Height="500px"
                 ViewStateMode="Enabled" >
                 
                <asp:Table ID="TableTitle" runat="server" ForeColor="#E7EDF5" ViewStateMode="Enabled" >
                   
                </asp:Table>
                <asp:Table ID="TableControles"   runat="server" ForeColor="Black" BackColor="White"   ViewStateMode="Enabled" CssClass="contenguia">
                    
                </asp:Table>
                <div id="butonloftfocus" style="display:none">
                    <asp:Button ID="focusremitente" runat="server" />
                </div>
            </asp:Panel>
            <input id="HiddenPROMP" type="hidden" value="" runat="server">
            <input id="HiddenPlantilla" type="hidden" value="" runat="server">
        </div>
        <div id="estado_titulo" style="width:100%;float:left;height:3%">
             <asp:Label ID="Label_estado_transac" runat="server" Text="Label"></asp:Label>
        </div>
       
       
        <div id="cler" style="clear: both"></div>
         <div id="Destinatarioguia" >
              
             <asp:Panel ID="Paneldestinatario" runat="server" DefaultButton="ButtonDAPCERRAR" Style="display: none; color: White; width: auto; height: auto">
                 <asp:DragPanelExtender ID="DragPanelExtenderdestinatario" runat="server" TargetControlID="Paneldestinatario" />
                 <asp:ModalPopupExtender ID="ModalPopupExtenderdestinatario" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonDAPCERRAR"
                     PopupControlID="Paneldestinatario" CancelControlID="Buttoncacerrar">
                 </asp:ModalPopupExtender>
                 <div id="divcabecer" class="cabecera2">
                     <asp:Button ID="Buttond2" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                     <asp:Button ID="ButtonDAPCERRAR" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                     <asp:Label ID="Label4" runat="server" Text="Busqueda" Font-Size="10" Style="float: left">
                     </asp:Label>
                     <div id="Divcerrarbuton" style="float: right">
                         <asp:Button ID="Buttoncacerrar" runat="Server" Text="X"
                             ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />

                     </div>

                 </div>

                 <asp:UpdatePanel ID="UpdatePanelContenido" runat="server" UpdateMode="Conditional" >
                     <ContentTemplate>

                         <div id="Diupdate" style="border: thin double #000080; color: White; background-color: #FFFFFF; height: auto; width: auto">
                             <div id="Contenidopaginabusqueda" style="height: 350px; width: 650px; overflow: no-display; color: black; margin-left: 15px">
                                 <asp:Table ID="Contenido" runat="server">
                                     <asp:TableRow ID="Detinatario">
                                         <asp:TableCell ID="dest" ColumnSpan="2" HorizontalAlign="Center">
                                  <label>CARACTERIZACION DESTINATARIO INTERNO </label>
                                         </asp:TableCell>

                                     </asp:TableRow>
                                     <asp:TableRow ID="Espacio">
                                         <asp:TableCell></asp:TableCell>
                                         <asp:TableCell></asp:TableCell>
                                     </asp:TableRow>
                                     <asp:TableRow ID="nombre">
                                         <asp:TableCell>
                                   <label>Nombre Destinatario </label>
                                         </asp:TableCell>
                                         <asp:TableCell>
                                             <asp:TextBox ID="TextBoxEditNombreDestRem" runat="server" Columns="50"></asp:TextBox>

                                             <asp:Button ID="ButtonConsulta" runat="server" Text="Consulta" />
                                         </asp:TableCell>
                                     </asp:TableRow>
                                     <asp:TableRow ID="Entidadempresa">
                                         <asp:TableCell>
                                               <label> Entidad Empresa representada</label>
                                         </asp:TableCell>
                                         <asp:TableCell>
                                             <asp:TextBox ID="TextBoxEntidadempresa" runat="server" Columns="50"></asp:TextBox>
                                         </asp:TableCell>
                                     </asp:TableRow>
                                     <asp:TableRow>
                                         <asp:TableCell>
                                  <label>Direccion Destinatario </label>
                                         </asp:TableCell>
                                         <asp:TableCell>
                                             <asp:TextBox ID="TextBoxEditDirecionDestEst" runat="server" TextMode="MultiLine" Columns="50" Rows="3"></asp:TextBox>
                                         </asp:TableCell>
                                     </asp:TableRow>
                                     <asp:TableRow>
                                         <asp:TableCell>
                                  <label>Pais destinatario  </label>
                                         </asp:TableCell>
                                         <asp:TableCell>
                                             <asp:DropDownList ID="ComboBoxEditPaisDesExt" runat="server" AutoPostBack="false" onchange="llenardepartamento();"></asp:DropDownList>
                                         </asp:TableCell>
                                     </asp:TableRow>
                                     <asp:TableRow>
                                         <asp:TableCell>
                                       <label>Departamento destinatario </label>
                                         </asp:TableCell>
                                         <asp:TableCell>
                                             <asp:DropDownList ID="ComboBoxEditDepartDestExt" runat="server" AutoPostBack="false" onchange="llenarciudad();"></asp:DropDownList>
                                         </asp:TableCell>
                                     </asp:TableRow>
                                     <asp:TableRow>
                                         <asp:TableCell>
                                  <label>Ciudad y/o Municipio </label>
                                         </asp:TableCell>
                                         <asp:TableCell>
                                             <asp:DropDownList ID="DropDownListciudad" runat="server" onchange="seleccionmuicipio();"></asp:DropDownList>
                                         </asp:TableCell>
                                     </asp:TableRow>
                                     <asp:TableRow>
                                         <asp:TableCell>
                                  <label>Telefono destinatario </label>
                                         </asp:TableCell>
                                         <asp:TableCell>
                                             <asp:TextBox ID="TextBoxEditTelefonodestext" runat="server" Columns="50"></asp:TextBox>
                                         </asp:TableCell>
                                     </asp:TableRow>
                                     <asp:TableRow>
                                         <asp:TableCell>
                                      <label> Codigo postal </label>
                                         </asp:TableCell>
                                         <asp:TableCell>
                                             <asp:TextBox ID="TextBoxEditCodPostalDestExt" runat="server" Columns="50"></asp:TextBox>
                                         </asp:TableCell>
                                     </asp:TableRow>
                                     <asp:TableRow>
                                         <asp:TableCell></asp:TableCell>
                                         <asp:TableCell>
                                             <asp:Button ID="BootonAgregar" runat="server" Text="Aceptar" />
                                             <asp:Button ID="ButtonActualizar" runat="server" Text="Actualizar" />
                                             <asp:Button ID="ButtonEliminar" runat="server" Text="Eliminar" OnClientClick='ConfirmMensaje("Desea eliminar el destinatario");' />
                                         </asp:TableCell>
                                     </asp:TableRow>
                                 </asp:Table>

                             </div>
                             <div id="border" style="border-style: outset; color: white; font-size: small; background-color: #053061; width: 670px">
                                 <label>El sistema detectara si usted quiere agregar un nuevo destinatario,</label>
                                 <label>o si quiere actualizar un destinatario existente en la base de datos</label>
                                 <label>Si esta para actualizar los datos el sistema muestra el boton aceptar</label>
                                 <label>de lo contrario el sistema le muestra el boton actualizar</label>
                             </div>


                         </div>
                         <div id="Divsepara" style="height: 10px; display: none">
                             <asp:Button ID="Buttonllenardepartamento" runat="server" Text="Button" BackColor="Silver" />
                             <input id="Hiddenselecionpais" type="hidden" value="" runat="server">
                             <asp:Button ID="Buttonllenarciudad" runat="server" Text="Button" />
                             <input id="Hiddenselecionciudad" type="hidden" value="" runat="server">
                             <input id="Hiddenmunicipio" type="hidden" value="" runat="server">
                             <input id="HiddenIDdestinatario" type="hidden" value="" runat="server">
                             <input id="Hiddendatoradicacion" type="hidden" value="" runat="server">
                             <input id="Hiddenruta" type="hidden" value="" runat="server">
                         </div>
                     </ContentTemplate>
                     <Triggers>
                         
                         <asp:AsyncPostBackTrigger ControlID="ButtonDAPCERRAR" EventName="Click" />
                         
                         
                         
                         
                     </Triggers>
                 </asp:UpdatePanel>

             </asp:Panel>

             
          </div>

        <div id="ventanaimpreion">
            <asp:Panel ID="Panelimpresion" runat="server" DefaultButton="ButtonDAPCERRAR" Style="display:none; color: White; width: auto; height: auto">
                 <asp:DragPanelExtender ID="DragPanelExtenderimpre" runat="server" TargetControlID="Panelimpresion" />
                 <asp:ModalPopupExtender ID="ModalPopupExtenderimpre" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir"
                     PopupControlID="Panelimpresion" CancelControlID="Buttoncerrarimpre">
                 </asp:ModalPopupExtender>
                 <div id="divcabecer2" class="cabecera2">
                     <asp:Button ID="Button1" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                     <asp:Button ID="ButtonSalir" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                     <asp:Label ID="Label1" runat="server" Text="Menu Impresion" Font-Size="10" Style="float: left">
                     </asp:Label>
                     <div id="Divcerrarbuton2" style="float: right">
                         <asp:Button ID="Buttoncerrarimpre" runat="Server" Text="X"
                             ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />

                     </div>
                   </div>
               
                <asp:UpdatePanel ID="UpdatePaneliframe" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div id="ContenidoImpresion" style="border: thin double #000080; color: black; background-color: #FFFFFF; height: 280px; width: 500px">
                            <iframe width="100%" height="100%" id="ifimpre" runat="server" src="../radicador/WebFormImprimir.aspx" ></iframe>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                </asp:Panel>
        </div>

        <div id="Impresion_post">
            <asp:Panel ID="Panelimpresionpost" runat="server"  Style="display: none; color: White; width: auto; height: auto">
                <asp:DragPanelExtender ID="DragPanelExtenderimpre_post" runat="server" TargetControlID="Panelimpresionpost" />
                <asp:ModalPopupExtender ID="ModalPopupExtenderimpre_post" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_post"
                    PopupControlID="Panelimpresionpost" CancelControlID="Buttoncerrarimpre_post">
                </asp:ModalPopupExtender>
                <div id="divcabecer2_post" class="cabecera2">
                    <asp:Button ID="Button1_post" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_post" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Label ID="Label2" runat="server" Text="Menu Impresion" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton2_post" style="float: right">
                        <asp:Button ID="Buttoncerrarimpre_post" runat="Server" Text="X"
                            ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />

                    </div>
                </div>
                <asp:UpdatePanel ID="UpdatePaneliframe_post" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div id="ContenidoImpresion_post" style="border: thin double #000080; color: black; background-color: #FFFFFF; height: 280px; width: 500px">
                            <iframe width="100%" height="100%" id="ifimpre_post_" runat="server" src="../radicador/WebFormImprimirfiles.aspx" ></iframe>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </asp:Panel>
        </div>
        
    </form>
  
</body>
    <script type="text/javascript">
       
        $(document).ready(function () {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);
            function EndRequestHandler(sender, args) {
              
                
             //*****Busca el nombre del destinatario externo 
            $('#<%=TextBoxEditNombreDestRem.ClientID%>').autocomplete({
                source: function (request, response) {
                    $.ajax({
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        url: "../webservice/WebServiceRadicacion.asmx/GetData",
                        data: "{'DName':'" + document.getElementById('TextBoxEditNombreDestRem').value + "'}",
                        dataType: "json",
                        success: function (data) {
                            response(data.d);
                        },
                        error: function (result) {
                            //alert("Error......");
                        }
                    });
                }
            });
                //*****Busca el nombre de la empresa a la que pertnece el usuario
                $('#<%=TextBoxEntidadempresa.ClientID%>').autocomplete({
                    source: function (request, response) {
                        $.ajax({
                            type: "POST",
                            contentType: "application/json; charset=utf-8",
                            url: "../webservice/WebServiceRadicacion.asmx/GetEmpresa",
                            data: "{'DName':'" + document.getElementById('TextBoxEntidadempresa').value + "'}",
                            dataType: "json",
                            success: function (data) {
                                response(data.d);
                            },
                            error: function (result) {
                                //alert("Error......");
                            }
                        });
                    }
                });
                //**********Busca las coinsisdencia de los campos de plantilla de la guia de radicacion
                $('#<%=TableControles.ClientID%> input:text').each(function () {
                    var rethis = $(this).text();
                    var tablad = "";
                    var campo = "";

                    $(document).on('keydown', function (e) {
                        var f = e.srcElement;
                        rethis = f.value;
                        tablad = $('#HiddenPlantilla').val();
                        campo = f.id;
                        if (e.srcElement.id == "Remitente_Cor" + "|" + "Destinatario_Cor" + "|VARCHAR") {
                            tablad = "destinatario_externo";
                            campo = "Nombre_Remitente";
                        }

                    });
                    $(document).on('keyup', function (e) {
                        var f = e.srcElement;
                        rethis = f.value;
                        tablad = $('#HiddenPlantilla').val();
                        campo = f.id;
                        if (e.srcElement.id == "Remitente_Cor" + "|" + "Destinatario_Cor" + "|VARCHAR") {
                            tablad = "destinatario_externo";
                            campo = "Nombre_Remitente";
                        }
                    });
                    $(this).autocomplete({

                        source: function (request, response) {
                            $.ajax({
                                type: "POST",
                                contentType: "application/json; charset=utf-8",
                                url: "../webservice/WebServiceRadicacion.asmx/GetGuiaRadicacon",
                                data: "{'DName':'" + rethis + "'," + "'DAcampo':'" + campo + "'," + "'DNtable':'" + tablad + "'}",
                                dataType: "json",
                                success: function (data) {
                                    response(data.d);

                                },
                                error: function (result) {
                                    alert("Error......" + result);
                                }
                            });
                        }
                    });
                });
             
               

            }

        });
      </script>  
</html>
