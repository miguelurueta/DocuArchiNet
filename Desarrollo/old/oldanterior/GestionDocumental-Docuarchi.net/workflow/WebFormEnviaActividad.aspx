<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormEnviaActividad.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormEnviaActividad" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="../js/ui/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
    <script src="../js/workflow/WebFormEnviaActividad.js"></script>
      <script src="../js/ScrollableGridViewPlugin_ASP.NetAJAXmin.js" type="text/javascript"></script>
    <script src="../Fixed-Header-Table-master/gridviewScroll.min.js"></script>
    <script src="../js/Filtrar.js"></script>
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
       <script defer src="../Awesome/js/all.js"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet">
  <link href="../Awesome/css/brands.css" rel="stylesheet">
  <link href="../Awesome/css/solid.css" rel="stylesheet">
    <script defer src="../Awesome/js/brands.js"></script>
  <script defer src="../Awesome/js/solid.js"></script>
  <script defer src="../Awesome/js/fontawesome.js"></script>
    <style type="text/css">
  
        .invisible { 
            visibility: hidden; 
        } 
    </style>
    <script  accesskey="javascript" type="text/javascript">
       
     
    </script>
</head>
<body style="">
    <form id="form1" runat="server">
    
        <asp:ScriptManager ID="ScriptManager1" runat="server"
            EnableScriptGlobalization="True" EnablePageMethods="True">
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

                 auto_zise_popup_pendinetes();
                 if (elment_postbak.id == "Buttonbuscar") {
                     busqueda_gred('hdnEmailID', 'GridViewlista', 'contenidobusqueda', 'CheckboxBusqueda');
                 }

                 progres_hiden('progres_bar');
             }

            </script>
          <div>
        <input id="hdnEmailID" type="hidden" value="0" runat="server">
        <input id="Hidden_resultado_gred" type="hidden" value="YES" runat="server">
        <div id="contenido_label" style="width:100%; height:20%; text-align:center; padding-top:5px" class="border_superior_radius">
            <i class="fas fa-users"></i>
            <asp:Label ID="Label_titulo" runat="server" Text="Grupos disponibles" style="font-family:Arial; color:black; font-size:14px"></asp:Label>
        </div>
         <div id="Lista" style="width: 100%; position: inherit; height:60%;  margin-top: 1px; border-color: #b0c4de; border-style: ridge; border-width: 1px">
            <asp:UpdatePanel ID="UpdatePanelgred" runat="server" UpdateMode="Conditional"  RenderMode="Inline">
                <ContentTemplate>
                   <asp:GridView ID="GridViewlista" runat="server" Font-Size="10pt" style="width:99.5%; font-family:Arial; font-size:13px"
                            AutoGenerateSelectButton="False" CssClass="filtrar" GridLines="None">
                              <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                <HeaderStyle CssClass="GridviewScrollHeader_line_blanco" />
                                <RowStyle CssClass="GridviewScrollItem_line" />
                                <PagerStyle CssClass="GridviewScrollPager_line" />
                            <Columns>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                           
                        </asp:GridView>

                    
                </ContentTemplate>
                <Triggers>
               
                </Triggers>
            </asp:UpdatePanel>
            </div>
            <div id="contenido_botonoes" style="width: 100%; position: inherit; left: auto; float: right; height: 20%">
                <asp:UpdatePanel ID="UpdatePanellista" runat="server" UpdateMode="Conditional"  >
                    <ContentTemplate>
                         <asp:TextBox ID="contenidobusqueda" runat="server" style="left:30px; width:250px; margin-left:2px; margin-top:3px"></asp:TextBox>
                          <asp:Button ID="Buttonbuscar" runat="server" Text="Buscar" class="boton_azul"  />
                          <asp:CheckBox ID="CheckboxBusqueda" runat="server" />
                          <label style="font-family:Arial ; font-size:10px"   >Buscar sólo palabra completa</label>
                    </ContentTemplate>
                </asp:UpdatePanel>                        
            </div>     
    </div>
      <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 200px">
            <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
            Processing ...
        </div>
    
    </form>
</body>
</html>
