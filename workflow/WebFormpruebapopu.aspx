<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormpruebapopu.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormpruebapopu" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="../Styles/style.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
  .header
  {
    overflow: auto;
    position:absolute; 
    background-color:White;
       
  }
  .FondoAplicacion
    {
        background-color: white;
        filter: alpha(opacity=70);
        opacity: 0.7;
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
</style>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    
          </asp:ScriptManager>
    <div>
    <asp:Button ID="Button3" runat="server" Text="ojo" />
       
     <asp:Panel ID="PanelPopup" runat="server"  estilo = "display: none"   Width="400" Height="400" HorizontalAlign="Center"   BehaviorID="leadingloadpanel" >
               <div id="TABCPTION" class="cabecera"   >
                  
                   <asp:Label ID="Label1" runat="server" Text="Enlace web workflow Importe documentos"></asp:Label>
                   <div id="Cerrar" style=" float: right ">
                       <asp:Button ID="botonpopupshow" runat="Server"  Text="X" ForeColor="White" />
                   </div>
                   
                   </div>
               <div id="text" 
                   style=" border:thin double #000080; color:White; width:100%; height: 93%; background-color: #FFFFFF;" >
                   
                   
                  
                   
               </div>
               
           </asp:Panel> 
           <asp:DragPanelExtender ID="move" runat="server" TargetControlID="PanelPopup" />            
              <asp:UpdatePanel ID="UpdatePopup" runat="server"  UpdateMode="Conditional"  >
              <ContentTemplate>
              <asp:ModalPopupExtender ID="Modalpopoenlace" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button1" PopupControlID="PanelPopup"
                      Cancelcontrolid="Close" DropShadow="true" >
                   </asp:ModalPopupExtender>
                  
                   <div >
                        <asp:Button ID="Button1" runat="server" Text="Button" />
                        <asp:Button ID="Close" runat="Server"   Text="Cerrar" />
                        <asp:Button ID="Button2" runat="server" Text="show" />
                         
                   </div>
              </ContentTemplate> 

                  <Triggers>
                      <asp:AsyncPostBackTrigger ControlID="Button1" />
                  </Triggers>

              </asp:UpdatePanel>
   
      
    </div>
    </form>
</body>
</html>
