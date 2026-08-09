<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormproduc.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormproduc" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Web Api Test</title>
    <script src="http://cdnjs.cloudflare.com/ajax/libs/jquery/2.0.3/jquery.min.js"></script>
    <script type="text/javascript">
        function getProducts() {
            $.getJSON("/api/products",
                function (data) {
                    $('#products').empty(); // Clear the table body.
                                       // Loop through the list of products.
                    $.each(data, function (key, val) {
                                        // Add a table row for the product.
                        $('#products').append('<tr><td>' + val.Name + '</td><td>' + val.Price + '</td></tr>');
                    });
                });
        }
       $(document).ready(getProducts);
    </script>
</head>
<body>
    <form id="form1" runat="server">
       <h2>Products</h2>
    <table>
        <thead>
            <tr><th>Name</th><th>Price</th></tr>
        </thead>
        <tbody id="products"></tbody>
    </table>
    </form>
</body>
</html>
