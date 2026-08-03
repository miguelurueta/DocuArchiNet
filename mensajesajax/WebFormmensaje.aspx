<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormmensaje.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormmensaje" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>AJAX Control Toolkit</title>
  <script type="text/javascript">
      var launch = false;
      function launchModal() {
          launch = true;
      }

      function pageLoad() {
          if (launch) {
              $find("ModalPopupExtender1").show();
          }
      }
  </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div>
      <asp:Button ID="ClientButton" runat="server" Text="Launch Modal Popup (Client)" />
      <asp:Button ID="ServerButton" runat="server" Text="Launch Modal Popup (Server)" OnClick="ServerButton_Click" />
    </div>
    <asp:Panel ID="ModalPanel" runat="server" Width="500px">
      ASP.NET AJAX is a free framework for quickly creating a new generation of more efficient,
      more interactive and highly-personalized Web experiences that work across all the
      most popular browsers.<br />
      <asp:Button ID="OKButton" runat="server" Text="Close" />
    </asp:Panel>
    
    <asp:ModalPopupExtender ID="ModalPopupExtender1" runat="server" 
        OkControlID="OKButton" PopupControlID="ModalPanel" 
        TargetControlID="ClientButton">
    </asp:ModalPopupExtender>
    </form>
</body>
</html>
