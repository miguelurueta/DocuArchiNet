<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Masterpage/Principal.Master" CodeBehind="Default.aspx.vb" Inherits="GestionDocumental_Docuarchi.net._Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlacenter" runat="server">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div id="contenedor" style="left: auto; width: 100%; float:left; height: 500px; background-color:White">
     <div id="Izquierdo" style="left: auto; width: 20%; float:left; height: 500px; background-color:White">
     </div>
     <div id="Central" style="left: auto; width: 60%; float:left; height: 500px; background-color:White">
         <div id="centralsuperior" style="left: auto; width: 100%; float:left; height: 20%; background-color:White">
         </div>
         <div id="centralcentral" style="left: auto; width: 100%; float:left; height: 60%; background-color:White">
             <asp:Panel ID="Panelcentral" runat="server" Height="292px" 
                 HorizontalAlign="Left">
                 <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                     <ContentTemplate>
                         <br />
                         <br />
                         <br />
                         <br />
                         <br />
                         <br />
                         <br />
                         <br />
                         <br />
                         <br />
                         <br />
                         <br />
                         &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                         <asp:Button ID="Button1" runat="server" Text="Aceptar" />
                         &nbsp;&nbsp;
                         <asp:Button ID="Button2" runat="server" Text="Button" />
                         <br />
                         <br />
                         <br />
                         <br />
                         <br />
                         <br />
                     </ContentTemplate>
                 </asp:UpdatePanel>
                 <br />
                 <br />
                 <br />
                 <br />
                 <br />
                 <br />
                 <br />
                 <br />
                 <br />
                 <br />
             </asp:Panel>
         </div>
         <div id="centralinferior" style="left: auto; width: 100%; float:left; height: 20%; background-color:White">
         </div>
     </div>
     <div id="Derecho" style="left: auto; width: 19.95%; float:right; height: 500px; background-color:White">
     </div>
     </div>
    </form>
</asp:Content>
