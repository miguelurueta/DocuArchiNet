<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="InnerPage.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.InnerPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script language="javascript" type="text/javascript">
        function okay() {
           /* window.parent.document.getElementById('Button3').click();*/
           /* __doPostBack('Button3', '');*/
            alert("ojo")
        }
        function cancel() {
            window.parent.document.getElementById('btnCancel').click();
        }
    </script>
</head>
<body style="margin: 0px; padding: 0px;">
    <form id="form1" runat="server">
     <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
       
         
    <div id="container" style=" border:thin double #000080; color:White;  background-color: #FFFFFF; 
      height:140px; width:247px">
    
        <div id="Contenido" style=" height:60%">
          
                    <asp:label ID="Text" runat="server" Text="ojo" ForeColor="Black" />
        </div>
       
        <div id="Contenidbuton" style=" height:30%">
             <asp:Button ID="btnOkay2" runat="server" Text="Aceptar "  />
            <asp:Button ID="Cancelar" runat="server" Text="Cancelar "  />
            
        </div>
    </div>
     
    </form>
</body>
</html>
