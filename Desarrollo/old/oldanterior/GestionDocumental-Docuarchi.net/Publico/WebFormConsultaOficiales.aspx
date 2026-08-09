<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormConsultaOficiales.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormConsultaOficiales" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
     <script src="../js/ui/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
    <link href="../ccs/themes/base/jquery.ui.all.css" rel="stylesheet" type="text/css" />
         <script  src="../Awesome/js/all.js"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet"/>
  <link href="../Awesome/css/brands.css" rel="stylesheet"/>
  <link href="../Awesome/css/solid.css" rel="stylesheet"/>
    <script  src="../Awesome/js/brands.js"></script>
  <script  src="../Awesome/js/solid.js"></script>
  <script  src="../Awesome/js/fontawesome.js"></script>
    <script src="../js/java_general/general_code_java.js"></script>
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />       
    <script src="../js/Publico/WebFormConsultaOficiales.js"></script>
 <!--===============================================================================================-->
	<link rel="stylesheet" type="text/css" href="../colorlib/vendor/bootstrap/css/bootstrap.min.css"/>
<!--===============================================================================================-->
	<link rel="stylesheet" type="text/css" href="../colorlib/fonts/font-awesome-4.7.0/css/font-awesome.min.css"/>
<!--===============================================================================================-->
	<link rel="stylesheet" type="text/css" href="../colorlib/vendor/animate/animate.css"/>
<!--===============================================================================================-->	
	<link rel="stylesheet" type="text/css" href="../colorlib/vendor/css-hamburgers/hamburgers.min.css"/>
<!--===============================================================================================-->
	<link rel="stylesheet" type="text/css" href="../colorlib/vendor/animsition/css/animsition.min.css"/>
<!--===============================================================================================-->
	<link rel="stylesheet" type="text/css" href="../colorlib/vendor/select2/select2.min.css"/>
<!--===============================================================================================-->	
	<link rel="stylesheet" type="text/css" href="../colorlib/vendor/daterangepicker/daterangepicker.css"/>
<!--===============================================================================================-->
	<link rel="stylesheet" type="text/css" href="../colorlib/css/util.css"/>
	<link rel="stylesheet" type="text/css" href="../colorlib/css/main.css"/>
<!--===============================================================================================-->
</head>
<body >
    <form id="form1" runat="server">
         <asp:ScriptManager ID="ScriptManager1" runat="server">
         </asp:ScriptManager>
        <div class="limiter"> 
            <div class="container-login100">
                <div class="wrap-login100_person">
                    <div class="validate-form p-l-35 p-r-35 p-t-17">
                        <div id="area_trabjo" class="mb-3">
                            <div class="row">
                                <div class="col-6">
                                    <h5 style="color: #57b846">CONSULTA ENTIDADES OFICIALES</h5>
                                </div>
                                <div class="col-6 ">
                                    <a href="javascript:void(0)" title="Atrás" class="float-right" onclick="activa_retroceso_principal();">
                                        <i style="color: #57b846" class="far fa-arrow-left fa-2x float-left"></i>
                                    </a>
                                </div>
                            </div> 
                            <hr />
                            <div class="row mt-3">
                                <div class="col-12">
                                     <p style="font-size: 14px; font-family: Arial; font-weight: bold">Estimado Usuario:</p>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-12">
                                    <p style="text-align: justify; font-size: 16px; font-family: Arial">
                                        Si eres un funcionario de entidad oficial, acércate a nuestra oficina principal
                                con un oficio membreteado por la entidad a la que pertenece, solicitando la consulta especial de documentos. En el oficio debe citar los siguientes datos:
                                 Dependencia a la que pertenece, cargo que ocupa, nombre completo, numero de identificación y correo electrónico institucional.
                                    </p>
                                </div>
                            </div>
                           
                        </div>
                    </div>
                </div>
            </div>
        </div>
       
    
    </form>
</body>
</html>
