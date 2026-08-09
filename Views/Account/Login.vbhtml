@inherits System.Web.Mvc.WebViewPage
@Code
    Layout = Nothing
End Code

<!DOCTYPE html>

<html>
<head>
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Iniciar Sesion</title>
    <link href="../wwwroot/stylesCSS/main.css" rel="stylesheet" />
    <script src="https://kit.fontawesome.com/5823d0daaf.js" crossorigin="anonymous" rel="stylesheet"></script>
</head>
<body>
    <div class="contenedor">
        <div class="formulario">
            <form action="#">
                <h2><img src="../wwwroot/stylesCSS/Images/contasoft.png" width="230" height="120" /></h2><!--Aqui debe estar la imagen de Docu.firma-->
                <div class="input-contenedor">
                    <!--Este es el input del login usuario-->
                    <i class="fas fa-user"></i>
                    <input type="text" placeholder=" " required />
                    <label for="#">Usuario</label>
                </div>
                <div class="input-contenedor">
                    <!--Este es el input de contraseña-->
                    <i class="fa-solid fa-lock"></i>
                    <input id="Userpasword" type="password" placeholder=" " />
                    <label for="#">Contraseña</label>
                </div>
                <div class="olvidar">
                    <label for="#">
                        <input type="checkbox" />Ver contraseña
                    </label>
                </div>
                <div class="aaa">
                    <button onclick="window.location.href='dashboard/dashboard.html';">Iniciar Sesión</button>
                </div>
                <br />
                <div class="www">
                    <p><a href="Formularioolvidecontraseña.html">¿Olvidaste tu contraseña?</a></p>
                </div>
            </form>
        </div>
    </div>
</body>
</html>
