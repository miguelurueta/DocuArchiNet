<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Login.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.Login" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="UTF-8" />
    <title>Login</title>
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css" rel="stylesheet" />
    <style>
        body {
            font-family: Arial, sans-serif;
            display: flex;
            height: 100vh;
            justify-content: center;
            align-items: center;
            background: #f5f6fa;
        }
      
        .login-container {
            width: 340px;
            background: #fff;
            padding: 35px 30px;
            border-radius: 10px;
            box-shadow: 0 5px 18px rgba(0,0,0,0.15);
        }

        .input-contenedor {
            position: relative;
            margin: 20px 0;
            border-bottom: 2px solid #000;
            font-size: 1.05rem;
            padding-top: 0.8rem;
        }
        
        .input-contenedor i {
            position: absolute;
            top: 50%;
            left: 5px;
            transform: translateY(-50%);
            color: #666;
        }
        
        .input-contenedor input {
            width: 100%;
            border: none;
            outline: none;
            background: transparent;
            padding: 10px 5px 6px 28px; /* espacio para el ícono */
            font-size: 1rem;
        }

        .input-contenedor label {
            position: absolute;
            left: 28px;
            top: 50%;
            transform: translateY(-50%);
            transition: all 0.2s ease;
            pointer-events: none;
            color: #444;
            font-weight: 600;
        }

        /* efecto flotante */
        .input-contenedor input:focus ~ label,
        .input-contenedor input:not(:placeholder-shown) ~ label {
            top: -6px;
            font-size: 0.85rem;
            color: #0a66ff;
        }

        .btn-login {
            width: 100%;
            padding: 12px;
            background: #0a66ff;
            color: white;
            border: none;
            border-radius: 6px;
            font-size: 1rem;
            cursor: pointer;
            margin-top: 15px;
        }

        .btn-login:hover {
            background: #084bcc;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-container">
            <h2 style="text-align:center; margin-bottom:25px;">Iniciar Sesión</h2>

            <div class="input-contenedor">
                <i class="fas fa-user"></i>
                <input type="text" id="txtUsuario" placeholder=" " required />
                <label for="txtUsuario">Usuario</label>
            </div>

            <div class="input-contenedor">
                <i class="fas fa-lock"></i>
                <input type="password" id="txtPass" placeholder=" " required />
                <label for="txtPass">Contraseña</label>
            </div>

            <button type="submit" class="btn-login">Ingresar</button>
        </div>
    </form>
</body>
</html>