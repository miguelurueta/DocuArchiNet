Option Strict On
Option Compare Binary
Imports System
Imports System.Globalization
Imports System.IO
Imports System.Security.Cryptography

'Una llamada documental bajo una reserva nueva. Nunca reintenta el escritor.
Public Partial Class AdaptadorDocumentoImportacion
    Implements IAlmacenamientoDocumentoImportacion
    Private ReadOnly _autorizar As Action(Of ComandoAlmacenamientoCarpetas)
    Private ReadOnly _datos As Func(Of ComandoAlmacenamientoCarpetas, DatosAlmacenamientoImportacion)
    Private ReadOnly _iniciar As Func(Of ComandoAlmacenamientoCarpetas, DatosAlmacenamientoImportacion, Boolean)
    Private ReadOnly _invocar As Action(Of ComandoAlmacenamientoCarpetas, DatosAlmacenamientoImportacion, String, SalidaAlmacenamientoLegacy)
    Private ReadOnly _verificar As Func(Of ComandoAlmacenamientoCarpetas, DatosAlmacenamientoImportacion, ResultadoEfectoImportacion, Boolean)
    Private ReadOnly _registrar As Func(Of ComandoAlmacenamientoCarpetas, ResultadoEfectoImportacion, ResultadoEfectoImportacion)
    Private ReadOnly _staging As IAlmacenStagingImportacion

    Friend Sub New(autorizar As Action(Of ComandoAlmacenamientoCarpetas), datos As Func(Of ComandoAlmacenamientoCarpetas, DatosAlmacenamientoImportacion), iniciar As Func(Of ComandoAlmacenamientoCarpetas, DatosAlmacenamientoImportacion, Boolean), invocar As Action(Of ComandoAlmacenamientoCarpetas, DatosAlmacenamientoImportacion, String, SalidaAlmacenamientoLegacy), verificar As Func(Of ComandoAlmacenamientoCarpetas, DatosAlmacenamientoImportacion, ResultadoEfectoImportacion, Boolean), registrar As Func(Of ComandoAlmacenamientoCarpetas, ResultadoEfectoImportacion, ResultadoEfectoImportacion), staging As IAlmacenStagingImportacion)
        If autorizar Is Nothing OrElse datos Is Nothing OrElse iniciar Is Nothing OrElse invocar Is Nothing OrElse verificar Is Nothing OrElse registrar Is Nothing OrElse staging Is Nothing Then Throw New ArgumentNullException()
        _autorizar = autorizar : _datos = datos : _iniciar = iniciar : _invocar = invocar : _verificar = verificar : _registrar = registrar : _staging = staging
    End Sub

    Public Function Almacenar(contexto As ContextoImportacionCarpetas, raiz As RaizImportacionCarpetas, archivo As ArchivoImportacionCarpetas, snapshot As SnapshotPerfilImportacionOperativa, intento As IntentoImportacionCarpetas) As ResultadoEfectoImportacion Implements IAlmacenamientoDocumentoImportacion.Almacenar
        Dim comando = ComandoAlmacenamientoCarpetas.Preparar(contexto, raiz, archivo, snapshot, intento)
        contexto = comando.Contexto : raiz = comando.Raiz : archivo = comando.Archivo
        _autorizar(comando)
        Dim datos = Copiar(_datos(comando))
        comando.ValidarDatos(datos)
        _autorizar(comando)
        If Not _iniciar(comando, Copiar(datos)) Then Throw New PerfilImportacionException("CONFLICTO_REVISION", "El intento ya no puede iniciar almacenamiento.")
        intento.EsNuevaReserva = False
        Dim resultado As New ResultadoEfectoImportacion With {.Estado = "INCIERTO", .Codigo = "RESULTADO_INCIERTO", .Mensaje = "El almacenamiento requiere comprobacion antes de cualquier repeticion.", .Expediente = comando.Raiz.IdentidadExpediente, .OrigenGabinete = comando.Snapshot.Configuracion.OrigenGabinete, .GabineteId = comando.Snapshot.Configuracion.GabineteId.Value.ToString(CultureInfo.InvariantCulture), .ValoresEfectivos = comando.Archivo.ValoresEfectivos}
        Dim retenido As IRetencionVerificadaImportacion = Nothing
        Dim salida As New SalidaAlmacenamientoLegacy()
        Dim concluida As Boolean = False
        Try
            Try
                _autorizar(comando)
                retenido = _staging.ConservarVerificado(contexto)
                If retenido Is Nothing OrElse retenido.Recepcion Is Nothing OrElse retenido.Recepcion.StagingRef <> archivo.StagingRef OrElse retenido.Recepcion.ContenidoHash <> archivo.ContenidoHash OrElse retenido.Recepcion.BytesRecibidos <> archivo.BytesPrevistos Then Throw New IOException()
                Dim copia = _staging.CrearCopiaTrabajo(contexto, archivo.StagingRef, archivo.NombreOriginal)
                VerificarCopia(comando, copia)
                _autorizar(comando)
                _invocar(comando, Copiar(datos), copia, salida)
                concluida = True
            Catch ex As Exception
                'Sin mensajes legacy, SQL o paths en evidencia publica.
            End Try
            If salida.DocumentoId > 0 Then resultado.DocumentoId = salida.DocumentoId.ToString(CultureInfo.InvariantCulture)
            If salida.InventarioId > 0 Then resultado.InventarioId = salida.InventarioId.ToString(CultureInfo.InvariantCulture)
            If concluida AndAlso salida.Respuesta = "YES" AndAlso salida.DocumentoId > 0 AndAlso salida.InventarioId > 0 AndAlso salida.NombreArchivo = archivo.NombreOriginal AndAlso salida.GabineteNombre = datos.GabineteNombre Then
                Try
                    _autorizar(comando)
                    If _verificar(comando, Copiar(datos), Copiar(resultado)) Then
                        resultado.Estado = "CONFIRMADO" : resultado.Codigo = "DOCUMENTO_CONFIRMADO"
                        resultado.Mensaje = "Documento, gabinete e incorporacion comprobados en el expediente confirmado."
                    End If
                Catch ex As Exception
                    'Mantener evidencia y estado incierto si no se puede verificar.
                End Try
            End If
            Try
                Dim guardado = _registrar(comando, Copiar(resultado))
                If guardado IsNot Nothing AndAlso PerfilImportacionJson.Escribir(guardado) = PerfilImportacionJson.Escribir(resultado) Then Return guardado
            Catch ex As Exception
                'Un checkpoint sin respuesta puede haber confirmado; consultar.
            End Try
            resultado.Estado = "INCIERTO" : resultado.Codigo = "RESULTADO_INCIERTO"
            resultado.Mensaje = "El almacenamiento requiere comprobacion antes de cualquier repeticion."
            Return resultado
        Finally
            If retenido IsNot Nothing Then retenido.Dispose()
        End Try
    End Function
    Private Shared Sub VerificarCopia(comando As ComandoAlmacenamientoCarpetas, ruta As String)
        If String.IsNullOrWhiteSpace(ruta) OrElse Not Path.IsPathRooted(ruta) OrElse Path.GetFileName(ruta) <> comando.Archivo.NombreOriginal Then Throw New IOException()
        Using entrada As New FileStream(ruta, FileMode.Open, FileAccess.Read, FileShare.Read), sha = SHA256.Create()
            If entrada.Length.ToString(CultureInfo.InvariantCulture) <> comando.Archivo.BytesPrevistos OrElse BitConverter.ToString(sha.ComputeHash(entrada)).Replace("-", "").ToLowerInvariant() <> comando.Archivo.ContenidoHash Then Throw New IOException()
        End Using
    End Sub
    Private Shared Function Copiar(Of T)(valor As T) As T
        Return PerfilImportacionJson.Leer(Of T)(PerfilImportacionJson.Escribir(valor))
    End Function
End Class

Public Class SalidaAlmacenamientoLegacy
    Public Property Respuesta As String
    Public Property DocumentoId As Integer
    Public Property InventarioId As Long
    Public Property NombreArchivo As String
    Public Property GabineteNombre As String
End Class
