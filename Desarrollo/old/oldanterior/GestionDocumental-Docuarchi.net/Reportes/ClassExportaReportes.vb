Imports System.IO
Public Class CdfExportaReportes
    Property NombreReporte As String
    Property UsuarioReporte As String
    Property RutaTeporte As String
    Property DateReporte As String
    Property ColumReportes As IList(Of CdColumReportes)
    Property Row As IList(Of CdRowReportes)
End Class
Public Class CdColumReportes
    Property NameColum As String
    Property AleasColum As String
    Property VisibleColum As String
End Class
Public Class CdRowReportes
    Property Cell As IList(Of CdCellReportes)
End Class
Public Class CdCellReportes
    Property CellValue As String
End Class
Public Class ClassExportaReportes
    Function ExportaReporteHtml(ByVal CdfExportaReportes As CdfExportaReportes) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Exporta los datos de una tabla a excell HTML
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'CdfExportaReportes  : Representa la estructura de los datos de importación
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        '
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-05-24
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim FileStream As New FileStream(CdfExportaReportes.RutaTeporte, FileMode.Create, FileAccess.ReadWrite)
            Dim StreamWriter As StreamWriter
            StreamWriter = New StreamWriter(FileStream)
            Dim comillas As String = Char.ConvertFromUtf32(34)
            Dim html As New StringBuilder()
            html.Append("!DOCTYPE HTML>")
            html.Append("<head>")
            html.Append("<meta http-equiv=" + comillas + "Content-Type" + comillas + "content=" + comillas + "text/html charset=utf-8" + comillas + "/>")
            html.Append("<title>Untitled Document</title>")
            html.Append("</head>")
            html.Append("<body>")
            Dim colp As Integer = CdfExportaReportes.Row.Count - 1
            Dim registro As Integer = CdfExportaReportes.Row.Count
            Dim uri_split() As String = HttpContext.Current.Request.Url.ToString.Split("/")
            Dim name_page As String = uri_split(uri_split.Length - 1)
            Dim BaseUrl As String = ""
            Dim Url As String = HttpContext.Current.Request.Url.Scheme & "://" & HttpContext.Current.Request.Url.Host & HttpContext.Current.Request.ApplicationPath
            BaseUrl = Url & "/" & "imagera/logo_trd.png"
            Dim mg = "<img src=" + BaseUrl + comillas + " alt=" + comillas + "Smiley face" + comillas + "width=" + comillas + "80" + comillas +
                      "height =" + comillas + "80" + comillas + " >"
            html.Append("<table WIDTH=730 CELLSPACING=0 CELLPADDING=10 border=1 BORDERCOLOR=" + comillas + "#333366" + comillas + " bgcolor=" + comillas + "#FFFFFF" + comillas + ">")
            html.Append(" <tr> " &
                              "<td rowspan=" + comillas + "5" + comillas + "; colspan=" + comillas + "0" + comillas + "> " & mg & " </td>" &
                         "</tr>")
            html.Append(" <tr> " &
                        "<td colspan=" + comillas + colp.ToString + comillas + " ; align=" + comillas + "middle" + comillas + "> Reporte : " & CdfExportaReportes.NombreReporte & " </td>" &
                      "</tr>")
            html.Append(" <tr> " &
                       "<td colspan=" + comillas + colp.ToString + comillas + " ; align=" + comillas + "middle" + comillas + "> Genera : " & CdfExportaReportes.UsuarioReporte & " </td>" &
                     "</tr>")
            html.Append(" <tr> " &
                      "<td colspan=" + comillas + colp.ToString + comillas + " ; align=" + comillas + "middle" + comillas + "> Fecha : " & Trim(CStr(Now.ToString("yyyy'-'MM'-'dd HH':'mm':'ss"))) & " </td>" &
                    "</tr>")
            html.Append(" <tr> " &
                     "<td colspan=" + comillas + colp.ToString + comillas + " ; align=" + comillas + "middle" + comillas + "> Registros : " & registro.ToString & " </td>" &
                   "</tr>")
            html.Append(" <tr border=0> " &
                     "<td colspan=" + comillas + (colp + 1).ToString + comillas + " ; align=" + comillas + "middle" + comillas + "> " & "" & " </td>" &
                   "</tr>")
            html.Append("<tr> <b>")
            For i As Integer = 0 To CdfExportaReportes.ColumReportes.Count - 1
                Dim item = CdfExportaReportes.ColumReportes.Item(i)
                html.Append("<th bgcolor=" + comillas + "#E7EDF5" + comillas + ">" + item.AleasColum.ToString() + "</th>")
            Next
            html.Append("</b> </tr>")
            For Zrow As Integer = 0 To CdfExportaReportes.Row.Count - 1
                html.Append("<tr>")
                For j As Integer = 0 To CdfExportaReportes.ColumReportes.Count - 1
                    html.Append("<td>" + CdfExportaReportes.Row.Item(Zrow).Cell.Item(j).CellValue + "</td>")
                Next
                html.Append("</tr>")
            Next
            html.Append("</body>")
            html.Append("</html>")
            StreamWriter.Write(html.ToString())
            StreamWriter.Close()
            ExportaReporteHtml = "YES"
        Catch ex As Exception
            ExportaReporteHtml = "Inconsistencia general funcion Export_html5 " & ex.Message
        End Try

    End Function
End Class
