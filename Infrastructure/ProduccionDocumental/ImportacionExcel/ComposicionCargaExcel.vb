Option Strict On
Imports System
Imports System.Web

Public Class ComposicionCargaExcel
    Private ReadOnly _recepcion As Lazy(Of ServicioRecepcionExcel)
    Private ReadOnly _preparacion As Lazy(Of ServicioPreparacionExcel)
    Public ReadOnly Property Recepcion As ServicioRecepcionExcel
        Get
            Return _recepcion.Value
        End Get
    End Property
    Public ReadOnly Property Preparacion As ServicioPreparacionExcel
        Get
            Return _preparacion.Value
        End Get
    End Property
    Public ReadOnly Property Confirmacion As ServicioConfirmacionExcel
    Public Sub New(http As HttpContext, conexionId As UInteger)
        Dim adaptador As New ImportacionExcelConexionAdapter
        Dim limites As Func(Of LimitesLecturaExcel) = Function() New LimitesLecturaExcel With {.MaxFilasDatos = adaptador.Limite("MaxFilasDatos", 10000), .MaxArchivoMB = adaptador.Limite("MaxArchivoMB", 10)}
        Dim resolver As Func(Of ContextoCargaExcel) =
            Function()
                Dim ctx = New ImportacionExcelContextAdapter().Resolver(http)
                adaptador.ValidarModulo(http, ctx)
                If http.Request.ContentType.StartsWith("multipart/form-data;", StringComparison.OrdinalIgnoreCase) Then
                    ImportacionExcelRequestGuard.ValidarMultipart(http, ctx, limites())
                Else
                    ImportacionExcelRequestGuard.Validar(http, ctx)
                End If
                Return adaptador.ContextoCarga(http, ctx, conexionId)
            End Function
        Dim c = resolver()
        Dim plantilla = adaptador.Resolver(http, c, conexionId)
        Dim politica As New PoliticaDestinoCargaExcelAdapter(http, c)
        Dim cargas As New MySqlCargaExcelRepository(adaptador.FabricaCarga(http, c), AddressOf politica.Obtener)
        Dim almacen As Func(Of IAlmacenArchivoExcel) = Function() New AlmacenArchivoExcel(http.Server.MapPath("~/App_Data/ImportacionExcel"))
        _recepcion = New Lazy(Of ServicioRecepcionExcel)(Function() New ServicioRecepcionExcel(resolver, plantilla, cargas, almacen(), New ReferenciaRecepcionExcel, limites()))
        _preparacion = New Lazy(Of ServicioPreparacionExcel)(Function() New ServicioPreparacionExcel(resolver, plantilla, cargas, almacen(), limites()))
        Confirmacion = New ServicioConfirmacionExcel(resolver, cargas, almacen, limites)
    End Sub
End Class
