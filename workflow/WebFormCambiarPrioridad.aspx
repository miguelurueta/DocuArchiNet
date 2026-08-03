<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormCambiarPrioridad.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormCambiarPrioridad" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>  
    <script src="../js/ui/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
   <link href="../Styles/Aplicaction.css" rel="stylesheet" />
   <style type="text/css">
  
        .invisible { 
            visibility: hidden; 
        } 
    </style>
</head>
<script language="javascript" type="text/javascript">

    $(document).ready(function () {
        $('#<%=Buttonaplicar.ClientID%>').click(function () {          
                var buton = $('#Buttoncabcel', window.parent.document);
                buton.click();
        });
    });
     
    </script>
<body style="">
    <form id="form1" runat="server">
     <asp:ScriptManager ID="ScriptManager1" runat="server" 
             EnableScriptGlobalization="True" EnablePageMethods="True" >
             
         </asp:ScriptManager>
         <asp:UpdatePanel ID="Updategenral" runat="Server" UpdateMode="Conditional" >
            <ContentTemplate>
               <INPUT id="hdnEmailID" type="hidden" value="0" runat="server" >
            </ContentTemplate>
            <Triggers>
              <asp:AsyncPostBackTrigger ControlID="Buttonaplicar" EventName="Click" />
            </Triggers>
         </asp:UpdatePanel>
        <div id="contenido_titulo" style="height: 20px; text-overflow:ellipsis" class="border_superior_radius">
            <asp:Label ID="Label3" runat="server" Text="Prioridad" Font-Size="11" Font-Names="arial" Style="float: left; margin-left: 10px; color:black"></asp:Label>
        </div> 
        
    <div id="contenedorgenral" style="height:auto; margin-top:1px">
        
        
      <div id="Radio" style="width:100%" >
      <asp:Panel ID="contenedor" runat="server"  style="border: 1px solid #ccc">
          <br />
         
        <asp:RadioButton ID="RadioButtonUrgente" runat="server" Text="Estado urgente" GroupName="Prioridad" ForeColor="Red" style="margin-top:10px; margin-left:5px" Font-Size="10" Font-Names="arial" />
        <br />
        <asp:RadioButton ID="RadioButtonmediourgente" runat="server" Text="Estado medio urgente" GroupName="Prioridad" ForeColor="#0033CC" Font-Size="10" Font-Names="arial" style="margin-top:10px; margin-left:5px"/>
        <br />
        <asp:RadioButton ID="RadioButtonsemiurgente" runat="server" Text="Estado semi urgente" GroupName="Prioridad" ForeColor="#009900" Font-Size="10" Font-Names="arial" style="margin-top:10px; margin-left:5px"/>
        <br />
        <asp:RadioButton ID="RadioButtonEstadonormal" runat="server" Text="Estado normal" GroupName="Prioridad" Font-Size="10" Font-Names="arial" style="margin-top:10px; margin-left:5px" />
        <br />
        <br />
       </asp:Panel>
       
       <asp:Panel ID="boton" runat="Server" Height="30" CssClass="border_inferior_radius">
       
        <asp:Button ID="Buttonaplicar" runat="server" Text="Aplicar" style=" float:right; margin-right:3px" CssClass="boton_azul"/>
        </asp:Panel>
        
        </div>
        
    </div>
    </form>
</body>
</html>
