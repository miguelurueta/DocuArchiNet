Option Strict On
Option Compare Binary
Imports System
Imports System.Collections.Specialized
Imports System.Globalization
Imports System.Linq
Imports System.Security.Cryptography
Imports System.Text

Public NotInheritable Class PoliticaLimpiezaOriginalImportacion
    Public ReadOnly Property Habilitada As Boolean
    Public ReadOnly Property Retencion As TimeSpan
    Public Sub New(habilitada As Boolean, retencion As TimeSpan)
        Me.Habilitada = habilitada : Me.Retencion = retencion
        If habilitada Then Exigir()
    End Sub
    Public Sub Exigir()
        If Not Habilitada OrElse Retencion <= TimeSpan.Zero Then Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "La limpieza de originales no esta habilitada con una retencion valida.")
    End Sub
End Class

Public Partial Class ServicioConsultaImportacion
    Private ReadOnly _politicaLimpieza As Func(Of PoliticaLimpiezaOriginalImportacion)
    Private ReadOnly _limpieza As ILimpiezaOriginalImportacionRepository
    Private ReadOnly _verificarLimpieza As Func(Of VistaLimpiezaOriginalImportacion, Boolean)
    Private ReadOnly _stagingLimpieza As Func(Of LoteImportacionCarpetas, ArchivoImportacionCarpetas, ILimpiezaStagingImportacion)

    Public Sub New(identidad As Func(Of ContextoImportacionCarpetas), repositorio As IConsultaImportacionRepository, destino As Func(Of String, ContextoImportacionCarpetas), autorizar As Action(Of ContextoImportacionCarpetas, ConfiguracionPerfilImportacion), maximoPagina As Integer, politica As Func(Of PoliticaLimpiezaOriginalImportacion), limpieza As ILimpiezaOriginalImportacionRepository, verificar As Func(Of VistaLimpiezaOriginalImportacion, Boolean), staging As Func(Of LoteImportacionCarpetas, ArchivoImportacionCarpetas, ILimpiezaStagingImportacion), Optional relojUtc As Func(Of DateTime) = Nothing)
        Me.New(identidad, repositorio, destino, autorizar, maximoPagina, relojUtc)
        If politica Is Nothing OrElse limpieza Is Nothing OrElse verificar Is Nothing OrElse staging Is Nothing Then Throw New ArgumentNullException()
        _politicaLimpieza = politica : _limpieza = limpieza : _verificarLimpieza = verificar : _stagingLimpieza = staging
    End Sub

    'Una referencia exacta, sin escaneo de disco o borrado de historial.
    'No se expone por HTTP ni se invoca desde consultar/diagnosticar.
    Public Function LimpiarOriginal(solicitud As LimpiarOriginalImportacionSolicitud) As LimpiezaOriginalImportacionResultado
        If solicitud Is Nothing OrElse Not ValidadorPerfilImportacion.ReferenciaValida(solicitud.loteId) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(solicitud.archivoId) OrElse solicitud.revisionLoteEsperada = 0 OrElse solicitud.revisionArchivoEsperada = 0 Then Throw Invalida()
        If _politicaLimpieza Is Nothing Then Throw NoDisponible()
        Dim politica = _politicaLimpieza()
        If politica Is Nothing Then Throw NoDisponible()
        politica.Exigir()
        Dim consulta As New ConsultarLoteImportacionSolicitud With {.loteId = solicitud.loteId}
        Dim lectura = Autorizar(consulta), contexto = lectura.Lote.Contexto
        Dim vista = _limpieza.ObtenerVistaLimpieza(contexto, solicitud.loteId, solicitud.archivoId)
        ValidarVistaLimpieza(vista, politica, _ahora())
        If Not MismoDestino(contexto, vista.Inventario.Lote.Contexto) OrElse vista.ArchivoId <> solicitud.archivoId OrElse vista.Inventario.Lote.LoteId <> solicitud.loteId Then Throw Denegado()
        Dim archivo = vista.Inventario.Archivos.Single(Function(f) f.ArchivoId = vista.ArchivoId)
        If vista.Inventario.Lote.Revision <> solicitud.revisionLoteEsperada OrElse archivo.Revision <> solicitud.revisionArchivoEsperada Then Throw New PerfilImportacionException("CONFLICTO_REVISION", "El candidato de limpieza cambio.")
        Dim comprobar As Action(Of VistaLimpiezaOriginalImportacion) =
            Sub(actual)
                Dim vigente = _politicaLimpieza()
                If vigente Is Nothing Then Throw NoDisponible()
                vigente.Exigir()
                If vigente.Retencion <> politica.Retencion Then Throw NoDisponible()
                Dim fresca = Autorizar(consulta)
                If Not MismoDestino(contexto, fresca.Lote.Contexto) OrElse fresca.HayIntentosActivos OrElse fresca.HayIncertidumbre Then Throw Denegado()
                ValidarVistaLimpieza(actual, vigente, _ahora())
                If Not _verificarLimpieza(CopiarLimpieza(actual)) Then Throw NoDisponible()
            End Sub
        comprobar(vista)
        If vista.CodigoFinal IsNot Nothing Then Return ResultadoLimpieza(vista, "CONFIRMADO", vista.CodigoFinal, True)
        Try
            Dim almacen = _stagingLimpieza(CopiarLimpieza(vista.Inventario.Lote), CopiarLimpieza(archivo))
            If almacen Is Nothing Then Throw NoDisponible()
            Using retiro = almacen.AbrirRetiro(contexto)
                If retiro Is Nothing Then Throw NoDisponible()
                If Not retiro.Existe AndAlso vista.Autorizacion Is Nothing Then Return ResultadoLimpieza(vista, "CONSERVADO", "ORIGINAL_NO_VERIFICABLE", False)
                Dim preparada = _limpieza.PrepararLimpieza(CopiarLimpieza(vista), politica, comprobar)
                Return _limpieza.AplicarLimpieza(preparada, politica, comprobar, Function() retiro.Eliminar())
            End Using
        Catch ex As PerfilImportacionException When {"ACCESO_DENEGADO", "CONFLICTO_REVISION", "SOLICITUD_INVALIDA"}.Contains(ex.Codigo)
            Throw
        Catch ex As Exception
            'El diario preparado puede haber quedado confirmado o el archivo
            'ya ausente. Nunca reintentar aqui el borrado ni el efecto documental.
            Return ResultadoLimpieza(vista, "INCIERTO", "LIMPIEZA_NO_CONFIRMADA", False)
        End Try
    End Function

    Friend Shared Sub ValidarVistaLimpieza(vista As VistaLimpiezaOriginalImportacion, politica As PoliticaLimpiezaOriginalImportacion, ahora As DateTime)
        If politica Is Nothing Then Throw NoDisponible()
        politica.Exigir()
        If vista Is Nothing OrElse vista.Inventario Is Nothing OrElse vista.Inventario.Lote Is Nothing OrElse ahora.Kind <> DateTimeKind.Utc OrElse Not ValidadorPerfilImportacion.ReferenciaValida(vista.ArchivoId) Then Throw NoDisponible()
        Dim v = vista.Inventario, lote = v.Lote
        Dim agregado = CoordinadorImportacionCarpetas.EvaluarConsolidacion(v)
        If lote.Estado <> "COMPLETADO" OrElse agregado.Estado <> "COMPLETADO" OrElse agregado.HayIncertidumbre OrElse agregado.HayIntentosActivos OrElse v.Archivos.Where(Function(f) f.ArchivoId = vista.ArchivoId).Count() <> 1 Then Throw NoDisponible()
        Dim archivo = v.Archivos.Single(Function(f) f.ArchivoId = vista.ArchivoId)
        If Not EvidenciaRetencionVerificable(lote, archivo) Then Throw NoDisponible()
        Dim fecha As DateTime
        If Not DateTime.TryParseExact(lote.ActualizadoUtc, "yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal Or DateTimeStyles.AdjustToUniversal, fecha) OrElse fecha > ahora Then Throw NoDisponible()
        For Each intento In v.Intentos.Where(Function(i) i.Estado = "CONFIRMADO")
            Dim fin As DateTime
            If Not DateTime.TryParseExact(intento.FinUtc, "yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal Or DateTimeStyles.AdjustToUniversal, fin) OrElse fin > ahora Then Throw NoDisponible()
            If fin > fecha Then fecha = fin
        Next
        Try
            If fecha.Add(politica.Retencion) > ahora Then Throw NoDisponible()
        Catch ex As ArgumentOutOfRangeException
            Throw NoDisponible()
        End Try
        If vista.Autorizacion IsNot Nothing Then
            Dim a = vista.Autorizacion, preparada As DateTime
            If a.LoteId <> lote.LoteId OrElse a.ArchivoId <> archivo.ArchivoId OrElse a.HuellaInventario <> HuellaInventarioLimpieza(v) OrElse a.RetencionTicks <> politica.Retencion.Ticks OrElse Not DateTime.TryParseExact(a.PreparadaUtc, "yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal Or DateTimeStyles.AdjustToUniversal, preparada) OrElse preparada > ahora Then Throw NoDisponible()
        ElseIf vista.CodigoFinal IsNot Nothing Then
            Throw NoDisponible()
        End If
        If vista.CodigoFinal IsNot Nothing AndAlso Not {"STAGING_ELIMINADO", "STAGING_AUSENTE_AUTORIZADO"}.Contains(vista.CodigoFinal) Then Throw NoDisponible()
    End Sub
    Friend Shared Function HuellaInventarioLimpieza(vista As VistaConsolidacionImportacion) As String
        Using sha = SHA256.Create()
            Return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(PerfilImportacionJson.Escribir(vista)))).Replace("-", "").ToLowerInvariant()
        End Using
    End Function
    Friend Shared Function ResultadoLimpieza(vista As VistaLimpiezaOriginalImportacion, estado As String, codigo As String, repeticion As Boolean) As LimpiezaOriginalImportacionResultado
        Return New LimpiezaOriginalImportacionResultado With {.loteId = vista.Inventario.Lote.LoteId, .archivoId = vista.ArchivoId, .estado = estado, .codigo = codigo, .repeticion = repeticion}
    End Function
    Private Shared Function CopiarLimpieza(Of T)(valor As T) As T
        Return PerfilImportacionJson.Leer(Of T)(PerfilImportacionJson.Escribir(valor))
    End Function
End Class
