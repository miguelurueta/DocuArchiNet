Option Strict On
Option Compare Binary
Imports System
Imports System.IO

Public NotInheritable Partial Class AlmacenStagingImportacion
    Implements ILimpiezaStagingImportacion
    Public Function AbrirRetiro(contexto As ContextoImportacionCarpetas) As IRetiroOriginalImportacion Implements ILimpiezaStagingImportacion.AbrirRetiro
        Autorizar(contexto)
        If _lote.Estado <> "COMPLETADO" OrElse _archivo.EstadoEfecto <> "COMPLETADO" OrElse _archivo.EstadoRecepcion <> "RECIBIDO" OrElse _archivo.IntentoActivoId IsNot Nothing OrElse Not HexadecimalValido(_archivo.StagingRef, 32) Then Throw Invalida()
        Dim bloqueo As FileStream = Nothing, entrada As FileStream = Nothing
        Try
            Dim carpeta = CarpetaLote(contexto)
            ExigirSinEnlaces(carpeta)
            If Not Directory.Exists(carpeta) Then Throw NoDisponible()
            bloqueo = BloquearLote(carpeta)
            Dim original = Ruta(CarpetaArchivo(contexto), _archivo.StagingRef & ".bin")
            ExigirSinEnlaces(original)
            If File.Exists(original) Then
                Dim esperado = ExigirArchivo(), prueba = VerificarCompleto(original, _archivo.StagingRef, esperado)
                If prueba.ContenidoHash <> _archivo.ContenidoHash OrElse prueba.BytesRecibidos <> _archivo.BytesRecibidos Then Throw Incierto()
                'Rechaza una retencion/lector existente; nadie puede consumir el
                'original mientras se confirma la autorizacion durable.
                entrada = New FileStream(original, FileMode.Open, FileAccess.Read, FileShare.None)
                If CULng(entrada.Length) <> esperado OrElse CopiarAcotado(entrada, Stream.Null, esperado) <> _archivo.ContenidoHash Then Throw Incierto()
            End If
            Dim retiro As New RetiroOriginal(original, bloqueo, entrada)
            bloqueo = Nothing : entrada = Nothing
            Return retiro
        Finally
            If entrada IsNot Nothing Then entrada.Dispose()
            If bloqueo IsNot Nothing Then bloqueo.Dispose()
        End Try
    End Function

    Private NotInheritable Class RetiroOriginal
        Implements IRetiroOriginalImportacion
        Private ReadOnly _original As String
        Private _bloqueo As FileStream, _entrada As FileStream
        Private _ejecutado As Boolean
        Public Sub New(original As String, bloqueo As FileStream, entrada As FileStream)
            _original = original : _bloqueo = bloqueo : _entrada = entrada
        End Sub
        Public ReadOnly Property Existe As Boolean Implements IRetiroOriginalImportacion.Existe
            Get
                If _bloqueo Is Nothing Then Throw Conflicto()
                Return _entrada IsNot Nothing
            End Get
        End Property
        Public Function Eliminar() As Boolean Implements IRetiroOriginalImportacion.Eliminar
            If _bloqueo Is Nothing OrElse _ejecutado Then Throw Conflicto()
            _ejecutado = True
            ExigirSinEnlaces(_original)
            If _entrada Is Nothing Then
                If File.Exists(_original) Then Throw Incierto()
                Return False
            End If
            _entrada.Dispose() : _entrada = Nothing
            'Solo el .bin exacto ya comprobado; nunca directorios, partes,
            'copias de trabajo, historia o referencias documentales.
            ExigirSinEnlaces(_original)
            If Not File.Exists(_original) Then Throw Incierto()
            File.Delete(_original)
            Return True
        End Function
        Public Sub Dispose() Implements IDisposable.Dispose
            Try
                If _entrada IsNot Nothing Then _entrada.Dispose()
            Finally
                _entrada = Nothing
                If _bloqueo IsNot Nothing Then _bloqueo.Dispose()
                _bloqueo = Nothing
            End Try
        End Sub
    End Class
End Class
