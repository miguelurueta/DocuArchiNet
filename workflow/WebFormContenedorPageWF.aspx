<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Masterpage/Principal.Master"  CodeBehind="WebFormContenedorPageWF.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormContenedorPageWF" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title> Contendor </title>
     <script src="../js/ui/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
   <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
   <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <script src="../js/workflow/WebFormContenedorPageWF.js" type="text/javascript"></script>
    <script src="../js/sesion/js_sesion_gestor.js" type="text/javascript"></script>
   
</asp:Content>
<asp:Content ID="Content_frame" ContentPlaceHolderID="ContentPlacenter" runat="server">
    <form id="form1" runat="server" style="overflow:inherit">
            <div style="background-color:white; height:100%">
                 <input id="Hidden_id_tarea_selecionada" type="hidden" value="-1" runat="server"/>
                <input id="Hidden_tipo_conten" type="hidden" value="contenedor" runat="server"/>
                <iframe id="ifrm_ds_" runat="server"
                    style="border-style: none; background-color:white; border-color: white; border-left: solid; border-left-color: white; width: 100%; height: 97%; float: left"
                    frameborder="0" scrolling="no" >

                </iframe>
            </div>
        </form>
</asp:Content>
