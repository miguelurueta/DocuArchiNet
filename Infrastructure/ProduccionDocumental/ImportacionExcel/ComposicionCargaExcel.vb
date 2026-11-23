Option Strict On
Imports System
Imports System.Web

Public Class ComposicionCargaExcel
    Public ReadOnly Property Recepcion As ServicioRecepcionExcel
    Public ReadOnly Property Preparacion As ServicioPreparacionExcel
    Public Sub New(http As HttpContext, conexionId As UInteger)
        Dim adaptador As New ImportacionExcelConexionAdapter
        Dim limites As New LimitesLecturaExcel With {.MaxFilasDatos = adaptador.Limite("MaxFilasDatos", 10000), .MaxArchivoMB = adaptador.Limite("MaxArchivoMB", 10)}
        Dim resolver As Func(Of ContextoCargaExcel) =
            Function()
                Dim ctx = New ImportacionExcelContextAdapter().Resolver(http)
                adaptador.ValidarModulo(http, ctx)
                If http.Request.ContentType.StartsWith("multipart/form-data;", StringComparison.OrdinalIgnoreCase) Then
                    ImportacionExcelRequestGuard.ValidarMultipart(http, ctx, limites)
                Else
                    ImportacionExcelRequestGuard.Validar(http, ctx)
                End If
                Return adaptador.ContextoCarga(http, ctx, conexionId)
            End Function
        Dim c = resolver()
        Dim plantilla = adaptador.Resolver(http, c, conexionId)
        Dim politica As New PoliticaDestinoCargaExcelAdapter(http, c)
        Dim cargas As New MySqlCargaExcelRepository(adaptador.FabricaCarga(http, c), AddressOf politica.Obtener)
        Dim almacen As New AlmacenArchivoExcel(http.Server.MapPath("~/App_Data/ImportacionExcel"))
        Recepcion = New ServicioRecepcionExcel(resolver, plantilla, cargas, almacen, New ReferenciaRecepcionExcel, limites)
        Preparacion = New ServicioPreparacionExcel(resolver, plantilla, cargas, almacen, limites)
    End Sub
End Class
