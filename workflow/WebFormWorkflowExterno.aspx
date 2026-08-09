<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormWorkflowExterno.aspx.vb" MasterPageFile="~/Masterpage/Principal.Master" Inherits="GestionDocumental_Docuarchi.net.WebFormWorkflowExterno" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <title> Reportes workflow</title>
     <script src="../js/ui/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
     <script src="../js/ScrollableGridPlugin.js"></script>   
    <script src="../js/ScrollableGridViewPlugin_ASP.NetAJAXmin.js" type="text/javascript"></script>
    <script src="../Fixed-Header-Table-master/gridviewScroll.min.js"></script>
   <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
   <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <script src="../js/workflow/WebFormWorkflowExterno.js"></script>
    <script  accesskey="javascript" type="text/javascript">
      
    </script>
    <script type="text/javascript">
        
    </script>

</asp:Content>

    <asp:Content ID="Content1" ContentPlaceHolderID="ContentPlacenter" runat="server">
        <form id="form1" runat="server">
            <div>
                <input id="Hidden_selecion" type="hidden" value="" runat="server"/>
                <iframe id="ifrm_ds_" runat="server"
                    style="border-style: none; border-color: white; border-left: solid; border-left-color: white; left: auto; width: 99.6%; height: 97%; float: left"
                    frameborder="0" scrolling="no" src="../workflow/WebFormReportesWorkflow.aspx">

                </iframe>
            </div>
        </form>
   </asp:Content>


