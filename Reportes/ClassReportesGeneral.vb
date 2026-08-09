Imports System.IO

Public Class CdReportesGeneral
    Property AppError As String
    Property UrlFileReporte As String
    Property NameFile As String
    Property RutaFile As String
End Class
Public Class ClassReportesGeneral
    Function EliminaArchivoReport(RutaFile) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Elimina el archivo de reporte del repositorio
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'RutaFile             : Representa la ruta física del archivo
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        '
        '
        'RutaFile             : Retorna la ruta física del archivo
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-05-24
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim RutaFileTemp As String = RutaFile.Replace("/", "\")
            If File.Exists(RutaFileTemp) Then
                Kill(RutaFileTemp)
            End If
            EliminaArchivoReport = "YES"
        Catch ex As Exception
            EliminaArchivoReport = "Inconsistencia general funcion EliminaArchivoReport " & ex.Message
        End Try
    End Function
    Function ExportaReporteExcel(ByRef CdfExportaReportes As CdfExportaReportes,
                                 ByRef UrlArchivoReporte As String,
                                 ByRef NameFile As String,
                                 ByRef RutaFile As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Exporta los datos de una tabla a excell
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'CdfExportaReportes  : Representa la estructura de los datos de importación
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'UrlArchivoReporte    : Retorna la url del archivo de descarga
        'NameFile             : Retorna el nombre del archivo
        'RutaFile             : Retorna la ruta física del archivo
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-05-24
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim FechaNow As Date = Now
            Dim FechaReporte As String = FechaNow
            FechaReporte = FechaReporte.Replace(":", "")
            FechaReporte = FechaReporte.Replace(",", "")
            FechaReporte = FechaReporte.Replace("/", "")
            FechaReporte = FechaReporte.Replace("\", "")
            FechaReporte = FechaReporte.Replace("-", "")
            FechaReporte = Left(FechaReporte, FechaReporte.Length - 6)
            Dim value As Integer = CInt(Int((100 * Rnd()) + 1))
            Dim RutaReporte As String = HttpContext.Current.Session.Item("GA_RUTA_TEMP_GESTION")
            If Directory.Exists(RutaReporte) = False Then
                Directory.CreateDirectory(RutaReporte)
            End If
            NameFile = FechaReporte & "-" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & value.ToString & ".xls"
            Dim RutaArchivoReporte As String = RutaReporte & "\" & NameFile
            RutaFile = RutaArchivoReporte.Replace("\", "/")
            CdfExportaReportes.RutaTeporte = RutaArchivoReporte.Replace("\", "/")
            CdfExportaReportes.UsuarioReporte = HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION")
            Dim uri As String = HttpContext.Current.Request.Url.Scheme & "://" & HttpContext.Current.Request.Url.Host & HttpContext.Current.Request.ApplicationPath
            UrlArchivoReporte = uri & HttpContext.Current.Session.Item("GA_RUTA_TEMP_GESTION_URL") & NameFile
            Dim ClassExportaReportes As New ClassExportaReportes
            Result = ClassExportaReportes.ExportaReporteHtml(CdfExportaReportes)
            ExportaReporteExcel = Result
            Exit Function
        Catch ex As Exception
            ExportaReporteExcel = "Inconsistencia general funcion ExportaReporteExcel " & ex.Message
        End Try
    End Function
End Class
