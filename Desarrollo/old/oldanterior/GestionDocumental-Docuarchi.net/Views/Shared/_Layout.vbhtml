<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <title>ViewData("Title") - Híbrido WebForms + MVC</title>
</head>
<body>
    <header>
        <nav>
            <a href="/Home/Index">Inicio MVC</a> |
            <a href="/Home/About">Acerca de</a> |
            <a href="/form/Dasboard.aspx">WebForm</a>
        </nav>
    </header>
    <hr />
    <main>
        @RenderBody()
    </main>
</body>
</html>
