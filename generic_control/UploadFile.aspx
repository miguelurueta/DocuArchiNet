<%@ Page Language="vb" AutoEventWireup="true" CodeBehind="UploadFile.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.UploadFile" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <script src="../js/ui/jquery-3.4.1.min.js"></script>
    <script src="FileUploadHandler.js"></script>
    <link href="UploadFile.css" rel="stylesheet" />
      <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <script  src="../Awesome/js/all.js"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet"/>
     <link href="../Awesome/css/brands.css" rel="stylesheet"/>
     <link href="../Awesome/css/solid.css" rel="stylesheet"/>
     <script  src="../Awesome/js/brands.js"></script>
     <script  src="../Awesome/js/solid.js"></script>
     <script  src="../Awesome/js/fontawesome.js"></script> 
   
</head>
<body>
    <form id="form1" runat="server">
     
            <div class="p-2">
            <div class="row p-2">
                <div class="col-12">
                    <div class="file-select " id="src-file">
                        <input id="file_element_demo" type="file" multiple="multiple" style="width: 100px; height: 40px" name="src-file" class="p-1" contente_file="conten_file_element" aria-label="Archivo" onchange="start_UploadFile('demo');" />
                    </div>
                    <a id="save_file_element_demo" title="Guardar todos los archivos" onclick="copy_file_UploadFile();" class="btn  btn-success "><i style="color: white" class="fas fa-save "></i> Guardar </a>
                    <a id="cancel_file_element_demo" title="Cancelar carga de archivos" class="btn  btn-warning  d-none"><i style="color: white" class="fas fa-save "></i> Cancelar </a>
                    <a id="delete_file_element_demo" title="Elminar todos los archivos guardados" onclick="delete_file_all_UploadFile();" class="btn  btn-danger "><i style="color: white" class="fal fa-trash-alt "></i> Eliminar </a>
                </div>
            </div>
            <div class="p-2">
                <div style="text-align: center">
                    <asp:Label ID="Label_progres_bar_file_element_demo" runat="server" Text="" Style="font-family: Arial; text-align: center; font-size: 20px"></asp:Label>
                </div>
                <div id="pogres_file_element_contador_demo" style="text-align: center; font-family: Arial; font-size: 14px">
                </div>
                <div id="pogres_file_element_porcent_demo" style="text-align: center; font-family: Arial; font-size: 14px">
                </div>
                <div >
                    <div id="myProgress_file_element_demo">
                        <div id="myBar_file_element_demo" class="file-select-bar"></div>
                    </div>
                </div>
            </div>
            <div class="paren_element" id="conten_file_element_demo" style="overflow: auto; height: 300px">
                <table id="table_file_element_demo" class="table table-striped">
                </table>
            </div>
            <div class="row border pt-2">
                <div class="col-6  justify-content-start" >
                    <p id="count_byte_file_element_demo"></p>
                   
                </div>
                <div class="col-6 justify-content-end pt-2" >
                     <p id="count_file_element_demo" class=" font-weight-bold" style="float:right"> Estado </p>
                </div>
            </div>
        </div>
            <div class="preview_" style="display:none">
                <p>No files currently selected for upload</p>

            </div>
            <div  style="display:none">
                  <asp:FileUpload ID="file1" runat="server" AllowMultiple="true" /><br/>
                 <input type="button" value="Upload File" onclick="UploadFile()" />
  
                 <progress id="progressBar" value="0" max="100" style="width: 300px;"></progress>
                 <h3 id="status"></h3>
                 <p id="loaded_n_total"></p>
            </div>
          
        
    </form>
     <script>
         //start_UploadFile('file_element');
     </script>
</body>
</html>
