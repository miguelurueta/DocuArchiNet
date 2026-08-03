Imports System.ComponentModel
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports Newtonsoft.Json
' Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class WebServiceReportes
    Inherits System.Web.Services.WebService
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ServiceExportaReporteExcel(ByVal CdfExportaReportes As Object)
        '---------------------------------------------------------------------------
        'Funcion : Servicio que expone la exportación de tablas a excell
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'CdfExportaReportes   : Representa la estructura de los datos de la tabla
        '
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        '
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2025-05-23
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim IListCdReportesGneral = New List(Of CdReportesGeneral)
        Dim ItemCdReportesGneral As CdReportesGeneral = New CdReportesGeneral()
        Try
            Dim Result As String = ""
            Dim _CdfExportaReportes As New List(Of CdfExportaReportes)
            _CdfExportaReportes = JsonConvert.DeserializeObject(Of List(Of CdfExportaReportes))(CdfExportaReportes)
            If _CdfExportaReportes Is Nothing Then
                ItemCdReportesGneral.AppError = "Imposible deserializar los datos de la clase : (" & CdfExportaReportes & ")"
                IListCdReportesGneral.Add(ItemCdReportesGneral)
                Return IListCdReportesGneral
            End If
            Dim ClassReportesGeneral As New ClassReportesGeneral
            ItemCdReportesGneral.AppError = ClassReportesGeneral.ExportaReporteExcel(_CdfExportaReportes(0),
                                                                                     ItemCdReportesGneral.UrlFileReporte,
                                                                                     ItemCdReportesGneral.NameFile,
                                                                                     ItemCdReportesGneral.RutaFile)
            IListCdReportesGneral.Add(ItemCdReportesGneral)
            Return IListCdReportesGneral
        Catch ex As Exception
            ItemCdReportesGneral.AppError = "Inconsistencia general funcion ServiceExportaReporteExcel " & ex.Message
            IListCdReportesGneral.Add(ItemCdReportesGneral)
            Return IListCdReportesGneral
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ServiceEliminaArchivoReport(ByVal RutaFile As Object)
        '---------------------------------------------------------------------------
        'Funcion : Servicio que expone la eliminación de un archivo de reporte
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'RutaFile              :Representa la ruta física del archivo
        '
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        '
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-05-23
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim IListCdReportesGneral = New List(Of CdReportesGeneral)
        Dim ItemCdReportesGneral As CdReportesGeneral = New CdReportesGeneral()
        Try
            Dim Result As String = ""
            Dim ClassReportesGeneral As New ClassReportesGeneral
            ItemCdReportesGneral.AppError = ClassReportesGeneral.EliminaArchivoReport(RutaFile)
            IListCdReportesGneral.Add(ItemCdReportesGneral)
            Return IListCdReportesGneral
        Catch ex As Exception
            ItemCdReportesGneral.AppError = "Inconsistencia general funcion ServiceEliminaArchivoReport " & ex.Message
            IListCdReportesGneral.Add(ItemCdReportesGneral)
            Return IListCdReportesGneral
        End Try
    End Function

End Class