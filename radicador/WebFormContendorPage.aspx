<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Masterpage/Principal.Master" CodeBehind="WebFormContendorPage.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormContendorPage" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title> Conendor </title>
     <script src="../js/ui/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
   <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
   <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <script src="../js/radicacion/WebFormContendorPage.js"></script>
    <script accesskey="javascript" type="text/javascript">
        
        setInterval('determina_cambio_tarea_seleccionada()', '<%= (0.9 * 6000)%>');
    </script>
</asp:Content>
<asp:Content ID="Content_frame" ContentPlaceHolderID="ContentPlacenter" runat="server">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"
            EnableScriptGlobalization="True" EnablePageMethods="True">
        </asp:ScriptManager>
            <div style="background-color:white; height:100%">
                 <input id="Hidden_id_tarea_selecionada" type="hidden" value="-1" runat="server"/>
                <input id="Hidden_tipo_conten" type="hidden" value="contenedor" runat="server"/>
                 <input id="Hidden_estado_actualizacion" type="hidden" value="" runat="server"/>
                  
                <asp:UpdatePanel ID="UpdatePanel_visor" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <iframe id="ifrm_ds_" runat="server"
                            style="border-style: none; background-color: white; border-color: white; border-left: solid; border-left-color: white; left: auto; width: 99.6%; height: 97%; float: left"
                            frameborder="0" scrolling="no"></iframe>
                    </ContentTemplate>
                </asp:UpdatePanel>
                
                <asp:UpdatePanel ID="UpdatePanel_actualiza" runat="server" UpdateMode="Conditional" >
                          <ContentTemplate>
                             
                            <asp:Button ID="Button_actualiza_trevie_seleccion" runat="server" Text="Aceptar" Style="background-color: white; border-color: #b0c4de; height: 30px; width: 200px; height: 25px; text-align: center; display:none" CssClass="boton"  OnClientClick="actualiza_documento_relacionado();" />

                          </ContentTemplate>
                      </asp:UpdatePanel>
                
                               
                                       
            </div>
        </form>
</asp:Content>
