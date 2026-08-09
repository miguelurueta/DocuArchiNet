<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormRues.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormRues" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="PostFormConsultaExpedientes" runat="server" name="PostFormConsultaExpedientes" action="../rues/WebFormListadoConsultaRue.aspx" 
        method="POST" target="DoSubmit" onsubmit="DoSubmit = window.open('about:blank', 'DoSubmit', 'width=400,height=350'); ">
        <input type="hidden" name="param" value="U2FsdGVkX19j1/7t1kjj9FjHkWQh6aTQlpzwvlGm8rbVuF3phlPuizUr8kN8zUevFugLYxtjKgmbDduoby954dUjsoKbMPGfyKPJSsOC5KGST0Zkc/dtwfavBBHlnP/AHHHi2j7baO4Ku601NW6zHYO9HOz2fs64Ggo3XL468fJg/MUI0T7/jQf1GdTpu1SJi2F8zYZx5FIJOS6eO2O8SHt2GMLP4bVzNnHeYe0NVyUQXb/TSVEG9dUJidySb+co4Sj8r32mQ+hP67QueeakJOo+NNMdTs1DAN94vKN2EB1wc8+u7We1GmyUsWYQ5ExO333UzOawrxjKNRQjywdh4U3HQnApbCCKzEKq+Pf2Idk="/>
        <input type="hidden" name="codigoCamara" value="40"/>

    </form>
    <button id="VerExpediente" onclick="PostFormConsultaExpedientes.submit();" style="    
    width:auto;    
    background:#339cdf url(images/loginbuttonbg.png) repeat-x;
    color:#fff;
    padding:7px 10px 8px 10px;
    text-shadow:0px -1px #278db8;
    border:1px solid #339cdf;
    box-shadow:none;
    -moz-box-shadow:none;
    -webkit-box-shadow:none;
    margin:0 12px 0 0;
    cursor:pointer;
    *padding:7px 2px 8px 2px; /* IE7 Fix */">Ver Expediente</button>
</body>
</html>
