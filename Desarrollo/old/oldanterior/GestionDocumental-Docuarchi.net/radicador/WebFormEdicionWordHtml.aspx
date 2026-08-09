<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormEdicionWordHtml.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormEdicionWordHtml" %>
<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <script src="../js/ui/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
    <script src="../js/radicacion/WebFormEdicionWordHtml.js"></script>
    <script src="../ckeditor/ckeditor.js"></script>
    <script src="../ckeditor/ckeditor_basic.js"></script>
    <script type="text/javascript" language="javascript">
        auto_zise_popup_edicion_word_html();
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div id="editor" style="height:80%">
            <CKEditor:CKEditorControl ID="htmlEditor" runat="server">
                
            </CKEditor:CKEditorControl>
        </div>
        <div id="vista" style="display:none; height:19%">
              <asp:Button ID="exportButton" runat="server" Text="Vista Previa" OnClick="OnExportButtonClicked"  />
            <asp:Literal ID="Literal1" Text=" to " runat="server"  />
            <asp:DropDownList ID="outputFormatList" runat="server" >
                <asp:ListItem Value="docx">DOCX</asp:ListItem>
                <asp:ListItem Value="html">HTML</asp:ListItem>
                <asp:ListItem Value="mht">MHTML</asp:ListItem>
                <asp:ListItem Value="rtf">RTF</asp:ListItem>
                <asp:ListItem Value="txt">TXT</asp:ListItem>
                <asp:ListItem Selected="True" Value="pdf">PDF</asp:ListItem>
                <asp:ListItem Value="xps">XPS</asp:ListItem>
                <asp:ListItem Value="png">PNG</asp:ListItem>
                <asp:ListItem Value="jpg">JPEG</asp:ListItem>
                <asp:ListItem Value="gif">GIF</asp:ListItem>
                <asp:ListItem Value="bmp">BMP</asp:ListItem>
                <asp:ListItem Value="tif">TIFF</asp:ListItem>
                <asp:ListItem Value="wdp">WMP</asp:ListItem>
            </asp:DropDownList>
            &nbsp &nbsp &nbsp &nbsp &nbsp
             <input id="Button_guadar_html_editor" type="button" value="Salvar cambios" onclick="cargar_win();" />
            <asp:Label ID="label_result" runat="server" Text="Label" Style="font-family:Arial; font-size:10px; float:right"></asp:Label>
            <input id="Hidden_id_respuesta" type="hidden" value="-1" runat="server">
        </div>
       
    </div>
    </form>
</body>
</html>
