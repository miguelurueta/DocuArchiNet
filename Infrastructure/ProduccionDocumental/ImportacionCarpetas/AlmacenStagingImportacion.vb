Option Strict On
Option Compare Binary
Imports System
Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions
Imports iTextSharp.text.pdf

'Instancia para un archivo del lote autorizado. No conoce HttpPostedFile,
'sesion mutable, SQL ni operaciones documentales. No devuelve rutas por HTTP.
Public NotInheritable Partial Class AlmacenStagingImportacion
    Implements IAlmacenStagingImportacion
    Private ReadOnly _raiz As String
    Private ReadOnly _limites As LimitesImportacionCarpetas
    Private ReadOnly _lote As LoteImportacionCarpetas
    Private ReadOnly _archivo As ArchivoImportacionCarpetas
    Private ReadOnly _maximoPaginasPdf As Integer
    Private ReadOnly _maximoObjetosPdf As Integer

    Public Sub New(raizAplicacion As String, limites As LimitesImportacionCarpetas, lote As LoteImportacionCarpetas, archivo As ArchivoImportacionCarpetas, Optional maximoPaginasPdf As Integer = Integer.MaxValue, Optional maximoObjetosPdf As Integer = Integer.MaxValue)
        LimitesImportacionCarpetas.Exigir(limites)
        If String.IsNullOrWhiteSpace(raizAplicacion) OrElse Not Path.IsPathRooted(raizAplicacion) OrElse Not Directory.Exists(raizAplicacion) OrElse maximoPaginasPdf <= 0 OrElse maximoObjetosPdf <= 0 Then Throw NoDisponible()
        If lote Is Nothing OrElse lote.Contexto Is Nothing OrElse lote.Snapshot Is Nothing OrElse lote.Snapshot.Configuracion Is Nothing OrElse archivo Is Nothing OrElse
            Not ValidadorPerfilImportacion.ReferenciaValida(lote.LoteId) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(archivo.ArchivoId) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(archivo.RaizId) Then Throw Invalida()
        _raiz = Path.GetFullPath(Path.Combine(raizAplicacion, "App_Data", "ImportacionCarpetas"))
        _limites = limites : _maximoPaginasPdf = maximoPaginasPdf : _maximoObjetosPdf = maximoObjetosPdf
        _lote = PerfilImportacionJson.Leer(Of LoteImportacionCarpetas)(PerfilImportacionJson.Escribir(lote))
        _archivo = PerfilImportacionJson.Leer(Of ArchivoImportacionCarpetas)(PerfilImportacionJson.Escribir(archivo))
        ExigirSinEnlaces(Path.GetFullPath(raizAplicacion))
    End Sub

    Public Function Recibir(contexto As ContextoImportacionCarpetas, intento As IntentoImportacionCarpetas, contenido As Stream, bytesPrevistos As String) As RecepcionVerificadaImportacion Implements IAlmacenStagingImportacion.Recibir
        Dim referencia = ReferenciaIntento(contexto, intento)
        If Not {"RESERVADO", "EN_CURSO"}.Contains(intento.Estado) OrElse _archivo.EstadoRecepcion = "RECIBIDO" Then Throw Conflicto()
        Dim esperados = ExigirArchivo()
        If bytesPrevistos <> _archivo.BytesPrevistos OrElse contenido Is Nothing OrElse Not contenido.CanRead Then Throw Invalida()
        Try
            Dim carpetaLote = Me.CarpetaLote(contexto)
            AsegurarDirectorio(carpetaLote)
            Using bloqueo = BloquearLote(carpetaLote, True)
                Dim carpeta = CarpetaArchivo(contexto)
                AsegurarDirectorio(carpeta)
                Dim parte = Ruta(carpeta, referencia & ".part"), completo = Ruta(carpeta, referencia & ".bin")
                If File.Exists(parte) OrElse File.Exists(completo) Then Throw Conflicto()
                Dim ocupados = BytesRetenidos(carpetaLote)
                If ocupados > _limites.MaximoBytesLote OrElse esperados > _limites.MaximoBytesLote - ocupados Then Throw Limite()
                ExigirEspacio(carpeta, esperados)
                Dim hash As String
                Using salida As New FileStream(parte, FileMode.CreateNew, FileAccess.Write, FileShare.None, 4096, FileOptions.SequentialScan)
                    hash = CopiarAcotado(contenido, salida, esperados)
                    salida.Flush(True)
                End Using
                ValidarFormato(parte)
                ExigirSinEnlaces(carpeta)
                File.Move(parte, completo)
                Return New RecepcionVerificadaImportacion With {.StagingRef = referencia, .BytesRecibidos = esperados.ToString(CultureInfo.InvariantCulture), .ContenidoHash = hash}
            End Using
        Catch ex As UnauthorizedAccessException
            Throw NoDisponible()
        Catch ex As IOException
            Throw NoDisponible()
        End Try
    End Function

    Public Function Verificar(contexto As ContextoImportacionCarpetas, intento As IntentoImportacionCarpetas) As RecepcionVerificadaImportacion Implements IAlmacenStagingImportacion.Verificar
        Dim referencia = ReferenciaIntento(contexto, intento)
        Return VerificarReferencia(contexto, referencia)
    End Function

    'Lectura de un RECIBIDO ya persistido, sin inventar un intento de recepcion
    'o crear una copia consumible. Misma contencion, bloqueo, hash y PDF.
    Public Function VerificarRetenido(contexto As ContextoImportacionCarpetas) As RecepcionVerificadaImportacion Implements IAlmacenStagingImportacion.VerificarRetenido
        Autorizar(contexto)
        If _archivo.EstadoRecepcion <> "RECIBIDO" OrElse _archivo.BytesRecibidos <> _archivo.BytesPrevistos OrElse Not HexadecimalValido(_archivo.StagingRef, 32) OrElse Not HexadecimalValido(_archivo.ContenidoHash, 64) Then Throw Invalida()
        Return VerificarReferencia(contexto, _archivo.StagingRef)
    End Function

    Public Function ConservarVerificado(contexto As ContextoImportacionCarpetas) As IRetencionVerificadaImportacion Implements IAlmacenStagingImportacion.ConservarVerificado
        Autorizar(contexto)
        If _archivo.EstadoRecepcion <> "RECIBIDO" OrElse _archivo.BytesRecibidos <> _archivo.BytesPrevistos OrElse Not HexadecimalValido(_archivo.StagingRef, 32) OrElse Not HexadecimalValido(_archivo.ContenidoHash, 64) Then Throw Invalida()
        Dim esperados = ExigirArchivo(), entrada As FileStream = Nothing
        Try
            Dim carpeta = CarpetaLote(contexto)
            ExigirSinEnlaces(carpeta)
            If Not Directory.Exists(carpeta) Then Throw Incierto()
            Using bloqueo = BloquearLote(carpeta)
                Dim original = Ruta(CarpetaArchivo(contexto), _archivo.StagingRef & ".bin")
                ExigirSinEnlaces(original)
                If Not File.Exists(original) Then Throw Incierto()
                entrada = New FileStream(original, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan)
                If CULng(entrada.Length) <> esperados Then Throw Incierto()
                Dim hash = CopiarAcotado(entrada, Stream.Null, esperados)
                Try
                    ValidarFormato(original)
                Catch ex As PerfilImportacionException When ex.Codigo = "ARCHIVO_RECHAZADO"
                    Throw Incierto()
                End Try
                If hash <> _archivo.ContenidoHash Then Throw Incierto()
                Dim retencion As New RetencionVerificada(entrada, New RecepcionVerificadaImportacion With {.StagingRef = _archivo.StagingRef, .BytesRecibidos = _archivo.BytesRecibidos, .ContenidoHash = hash})
                entrada = Nothing
                Return retencion
            End Using
        Catch ex As UnauthorizedAccessException
            Throw NoDisponible()
        Catch ex As IOException
            Throw Incierto()
        Finally
            If entrada IsNot Nothing Then entrada.Dispose()
        End Try
    End Function

    Private NotInheritable Class RetencionVerificada
        Implements IRetencionVerificadaImportacion
        Private _entrada As FileStream
        Private ReadOnly _recepcion As RecepcionVerificadaImportacion
        Public Sub New(entrada As FileStream, recepcion As RecepcionVerificadaImportacion)
            _entrada = entrada : _recepcion = recepcion
        End Sub
        Public ReadOnly Property Recepcion As RecepcionVerificadaImportacion Implements IRetencionVerificadaImportacion.Recepcion
            Get
                Return New RecepcionVerificadaImportacion With {.StagingRef = _recepcion.StagingRef, .BytesRecibidos = _recepcion.BytesRecibidos, .ContenidoHash = _recepcion.ContenidoHash}
            End Get
        End Property
        Public Sub Dispose() Implements IDisposable.Dispose
            Dim anterior = _entrada : _entrada = Nothing
            If anterior IsNot Nothing Then anterior.Dispose()
        End Sub
    End Class

    Private Function VerificarReferencia(contexto As ContextoImportacionCarpetas, referencia As String) As RecepcionVerificadaImportacion
        Dim esperados = ExigirArchivo()
        Try
            Dim carpeta = CarpetaLote(contexto)
            ExigirSinEnlaces(carpeta)
            If Not Directory.Exists(carpeta) Then Throw Incierto()
            Using bloqueo = BloquearLote(carpeta)
                Dim rutaCompleta = Ruta(CarpetaArchivo(contexto), referencia & ".bin")
                Dim recepcion = VerificarCompleto(rutaCompleta, referencia, esperados)
                If _archivo.EstadoRecepcion = "RECIBIDO" AndAlso (_archivo.StagingRef <> referencia OrElse _archivo.BytesRecibidos <> recepcion.BytesRecibidos OrElse _archivo.ContenidoHash <> recepcion.ContenidoHash) Then Throw Incierto()
                Return recepcion
            End Using
        Catch ex As UnauthorizedAccessException
            Throw NoDisponible()
        Catch ex As IOException
            Throw Incierto()
        End Try
    End Function

    Public Function CrearCopiaTrabajo(contexto As ContextoImportacionCarpetas, stagingRef As String, nombreOriginal As String) As String Implements IAlmacenStagingImportacion.CrearCopiaTrabajo
        Autorizar(contexto)
        Dim esperados = ExigirArchivo()
        If Not HexadecimalValido(stagingRef, 32) OrElse stagingRef <> _archivo.StagingRef OrElse _archivo.EstadoRecepcion <> "RECIBIDO" OrElse _archivo.BytesRecibidos <> _archivo.BytesPrevistos OrElse Not HexadecimalValido(_archivo.ContenidoHash, 64) OrElse nombreOriginal <> _archivo.NombreOriginal Then Throw Invalida()
        ExigirNombre(nombreOriginal)
        Try
            Dim carpetaLote = Me.CarpetaLote(contexto)
            ExigirSinEnlaces(carpetaLote)
            If Not Directory.Exists(carpetaLote) Then Throw Incierto()
            Using bloqueo = BloquearLote(carpetaLote)
                Dim original = Ruta(CarpetaArchivo(contexto), stagingRef & ".bin")
                Dim verificado = VerificarCompleto(original, stagingRef, esperados)
                If verificado.ContenidoHash <> _archivo.ContenidoHash Then Throw Incierto()
                Dim directorio = Ruta(CarpetaArchivo(contexto), "trabajo", Guid.NewGuid().ToString("N"))
                AsegurarDirectorio(directorio)
                ExigirEspacio(directorio, esperados)
                Dim copia = Ruta(directorio, nombreOriginal)
                Using entrada As New FileStream(original, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan),
                    salida As New FileStream(copia, FileMode.CreateNew, FileAccess.Write, FileShare.None, 4096, FileOptions.SequentialScan)
                    If CopiarAcotado(entrada, salida, esperados) <> _archivo.ContenidoHash Then Throw Incierto()
                    salida.Flush(True)
                End Using
                Return copia
            End Using
        Catch ex As UnauthorizedAccessException
            Throw NoDisponible()
        Catch ex As IOException
            Throw Incierto()
        End Try
    End Function

    Public Function VerificarReenvio(contexto As ContextoImportacionCarpetas, intento As IntentoImportacionCarpetas, contenido As Stream) As RecepcionVerificadaImportacion Implements IAlmacenStagingImportacion.VerificarReenvio
        Dim retenido = Verificar(contexto, intento)
        If contenido Is Nothing OrElse Not contenido.CanRead Then Throw Invalida()
        Dim hash As String
        Try
            hash = CopiarAcotado(contenido, Stream.Null, ExigirArchivo())
        Catch ex As PerfilImportacionException When ex.Codigo = "LIMITE_EXCEDIDO" OrElse ex.Codigo = "ARCHIVO_RECHAZADO"
            Throw ContenidoDistinto()
        Catch ex As IOException
            Throw NoDisponible()
        End Try
        If hash <> retenido.ContenidoHash Then Throw ContenidoDistinto()
        Return retenido
    End Function

    Private Function VerificarCompleto(archivo As String, referencia As String, esperados As ULong) As RecepcionVerificadaImportacion
        ExigirSinEnlaces(archivo)
        If Not File.Exists(archivo) OrElse CULng(New FileInfo(archivo).Length) <> esperados Then Throw Incierto()
        Dim hash As String
        Using entrada As New FileStream(archivo, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan)
            hash = CopiarAcotado(entrada, Stream.Null, esperados)
            Try
                ValidarFormato(archivo)
            Catch ex As PerfilImportacionException When ex.Codigo = "ARCHIVO_RECHAZADO"
                Throw Incierto()
            End Try
        End Using
        Return New RecepcionVerificadaImportacion With {.StagingRef = referencia, .BytesRecibidos = esperados.ToString(CultureInfo.InvariantCulture), .ContenidoHash = hash}
    End Function

    Private Function CopiarAcotado(entrada As Stream, salida As Stream, esperados As ULong) As String
        Using sha = SHA256.Create()
            Dim buffer(4095) As Byte, total As ULong = 0UL
            While True
                'Se lee como maximo un byte adicional para detectar exceso; no
                'se consume ni materializa en memoria el contenido restante.
                Dim cantidad = CInt(Math.Min(CULng(buffer.Length), esperados - total + 1UL))
                Dim leidos = entrada.Read(buffer, 0, cantidad)
                If leidos = 0 Then Exit While
                If CULng(leidos) > esperados - total Then Throw Limite()
                salida.Write(buffer, 0, leidos)
                sha.TransformBlock(buffer, 0, leidos, buffer, 0)
                total += CULng(leidos)
            End While
            If total <> esperados Then Throw Rechazado()
            sha.TransformFinalBlock(New Byte() {}, 0, 0)
            Return BitConverter.ToString(sha.Hash).Replace("-", "").ToLowerInvariant()
        End Using
    End Function

    Private Function ExigirArchivo() As ULong
        Dim esperados As ULong
        If Not ULong.TryParse(_archivo.BytesPrevistos, NumberStyles.None, CultureInfo.InvariantCulture, esperados) OrElse esperados = 0UL OrElse esperados > CULng(Long.MaxValue) OrElse esperados.ToString(CultureInfo.InvariantCulture) <> _archivo.BytesPrevistos Then Throw Invalida()
        If esperados > _limites.MaximoBytesArchivo OrElse esperados > _limites.MaximoBytesLote Then Throw Limite()
        ExigirNombre(_archivo.NombreOriginal)
        Dim configuracion = _lote.Snapshot.Configuracion
        Dim extension = Path.GetExtension(_archivo.NombreOriginal).TrimStart("."c).ToLowerInvariant()
        If configuracion.Extensiones Is Nothing OrElse (configuracion.Extensiones.Count > 0 AndAlso Not configuracion.Extensiones.Contains(extension, StringComparer.OrdinalIgnoreCase)) Then Throw Rechazado()
        If extension <> "pdf" Then Throw New PerfilImportacionException("FORMATO_NO_SOPORTADO", "Este receptor admite PDF validado; el formato solicitado aún no está caracterizado.")
        If configuracion.MaximoBytesArchivo.HasValue AndAlso (configuracion.MaximoBytesArchivo.Value <= 0 OrElse esperados > CULng(configuracion.MaximoBytesArchivo.Value)) Then Throw Limite()
        Return esperados
    End Function

    Private Sub ValidarFormato(archivo As String)
        Try
            Using entrada As New FileStream(archivo, FileMode.Open, FileAccess.Read, FileShare.Read)
                Dim cabecera(7) As Byte
                If entrada.Read(cabecera, 0, cabecera.Length) <> 8 OrElse Not Regex.IsMatch(Encoding.ASCII.GetString(cabecera), "\A%PDF-1\.[0-7]\z", RegexOptions.CultureInvariant) Then Throw Rechazado()
                Dim longitud = CInt(Math.Min(entrada.Length, 1024L)), cola(longitud - 1) As Byte
                entrada.Position = entrada.Length - longitud
                If entrada.Read(cola, 0, longitud) <> longitud OrElse Not Encoding.ASCII.GetString(cola).TrimEnd(ChrW(9), ChrW(10), ChrW(12), ChrW(13), " "c).EndsWith("%%EOF", StringComparison.Ordinal) Then Throw Rechazado()
            End Using
            'Lectura parcial sobre archivo, sin byte[] del documento completo.
            Using pdf As New PdfReader(archivo, Nothing, True)
                If pdf.IsEncrypted() OrElse pdf.IsRebuilt() OrElse pdf.NumberOfPages <= 0 OrElse pdf.NumberOfPages > _maximoPaginasPdf OrElse pdf.XrefSize > _maximoObjetosPdf Then Throw Rechazado()
                For pagina = 1 To pdf.NumberOfPages
                    If pdf.GetPageNRelease(pagina) Is Nothing Then Throw Rechazado()
                Next
            End Using
        Catch ex As PerfilImportacionException
            Throw
        Catch ex As Exception When TypeOf ex Is IOException OrElse TypeOf ex Is ArgumentException OrElse TypeOf ex Is iTextSharp.text.exceptions.BadPasswordException OrElse TypeOf ex Is InvalidOperationException OrElse TypeOf ex Is NullReferenceException
            Throw Rechazado()
        End Try
    End Sub

    Private Function ReferenciaIntento(contexto As ContextoImportacionCarpetas, intento As IntentoImportacionCarpetas) As String
        Autorizar(contexto)
        If intento Is Nothing OrElse Not ValidadorPerfilImportacion.ReferenciaValida(intento.IntentoId) OrElse intento.LoteId <> _lote.LoteId OrElse intento.ArchivoId <> _archivo.ArchivoId OrElse intento.RaizId <> _archivo.RaizId OrElse intento.Fase <> "RECEPCION" OrElse Not HexadecimalValido(intento.TokenReclamacion, 32) OrElse Not EstadosImportacionCarpetas.EsIntento(intento.Estado) Then Throw Invalida()
        Using sha = SHA256.Create()
            Return BitConverter.ToString(sha.ComputeHash(Encoding.ASCII.GetBytes(contexto.SelloDestino & ":" & contexto.AmbitoRef & ":" & contexto.UsuarioRef & ":" & intento.LoteId & ":" & intento.ArchivoId & ":" & intento.IntentoId & ":" & intento.TokenReclamacion))).Replace("-", "").ToLowerInvariant().Substring(0, 32)
        End Using
    End Function

    Private Sub Autorizar(contexto As ContextoImportacionCarpetas)
        Dim propio = _lote.Contexto
        If contexto Is Nothing OrElse Not contexto.PuedeImportar OrElse Not ValidadorPerfilImportacion.ReferenciaValida(contexto.UsuarioRef) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(contexto.AmbitoRef) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(contexto.NivelRef) OrElse Not HexadecimalValido(contexto.SelloDestino, 64) OrElse contexto.UsuarioRef <> propio.UsuarioRef OrElse contexto.AmbitoRef <> propio.AmbitoRef OrElse contexto.NivelRef <> propio.NivelRef OrElse contexto.SelloDestino <> propio.SelloDestino OrElse contexto.RutaCanonica <> propio.RutaCanonica Then Throw New PerfilImportacionException("ACCESO_DENEGADO", "El staging no está disponible para este contexto.")
    End Sub

    Private Function CarpetaLote(contexto As ContextoImportacionCarpetas) As String
        Return Ruta(_raiz, contexto.SelloDestino, contexto.AmbitoRef, contexto.UsuarioRef, _lote.LoteId)
    End Function
    Private Function CarpetaArchivo(contexto As ContextoImportacionCarpetas) As String
        Return Ruta(CarpetaLote(contexto), _archivo.ArchivoId)
    End Function
    Private Shared Function Ruta(base As String, ParamArray partes As String()) As String
        Dim destino = Path.GetFullPath(Path.Combine(New String() {base}.Concat(partes).ToArray()))
        If Not destino.StartsWith(Path.GetFullPath(base).TrimEnd(Path.DirectorySeparatorChar) & Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) Then Throw Invalida()
        Return destino
    End Function
    Private Shared Sub ExigirNombre(nombre As String)
        If String.IsNullOrWhiteSpace(nombre) OrElse nombre.Length > 160 OrElse nombre.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 OrElse nombre <> Path.GetFileName(nombre) OrElse nombre.EndsWith(".", StringComparison.Ordinal) OrElse nombre.EndsWith(" ", StringComparison.Ordinal) OrElse Regex.IsMatch(nombre, "\A(CON|PRN|AUX|NUL|CONIN\$|CONOUT\$|COM[1-9¹²³]|LPT[1-9¹²³])(\.|\z)", RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant) Then Throw Invalida()
    End Sub
    Private Shared Function HexadecimalValido(valor As String, longitud As Integer) As Boolean
        Return valor IsNot Nothing AndAlso valor.Length = longitud AndAlso valor.All(Function(c) (c >= "0"c AndAlso c <= "9"c) OrElse (c >= "a"c AndAlso c <= "f"c))
    End Function
    Private Shared Sub ExigirSinEnlaces(destino As String)
        Dim actual As String = Path.GetFullPath(destino)
        While Not String.IsNullOrEmpty(actual)
            Try
                If (File.GetAttributes(actual) And FileAttributes.ReparsePoint) <> 0 Then Throw NoDisponible()
            Catch ex As FileNotFoundException
                'Un componente nuevo se crea despues de comprobar sus padres.
            Catch ex As DirectoryNotFoundException
            End Try
            actual = Path.GetDirectoryName(actual)
        End While
    End Sub
    Private Shared Sub AsegurarDirectorio(destino As String)
        ExigirSinEnlaces(destino)
        Directory.CreateDirectory(destino)
        ExigirSinEnlaces(destino)
    End Sub
    Private Shared Function BloquearLote(carpeta As String, Optional nuevaRecepcion As Boolean = False) As FileStream
        ExigirSinEnlaces(carpeta)
        Dim archivo = Path.Combine(carpeta, "recepcion.lock")
        ExigirSinEnlaces(archivo)
        Try
            Return New FileStream(archivo, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None)
        Catch ex As IOException When (ex.HResult And &HFFFF) = 32 OrElse (ex.HResult And &HFFFF) = 33
            If nuevaRecepcion Then Throw New PerfilImportacionException("RECEPCION_OCUPADA", "El almacenamiento del lote está ocupado; este intento no inició escritura.")
            Throw Conflicto()
        End Try
    End Function
    Private Shared Function BytesRetenidos(carpeta As String) As ULong
        Dim total As ULong = 0UL
        For Each directorio In Directory.GetDirectories(carpeta)
            ExigirSinEnlaces(directorio)
            For Each archivo In Directory.GetFiles(directorio).Where(Function(p) p.EndsWith(".part", StringComparison.Ordinal) OrElse p.EndsWith(".bin", StringComparison.Ordinal))
                ExigirSinEnlaces(archivo)
                Dim longitud = CULng(New FileInfo(archivo).Length)
                If ULong.MaxValue - total < longitud Then Throw Limite()
                total += longitud
            Next
        Next
        Return total
    End Function
    Private Shared Sub ExigirEspacio(carpeta As String, requerido As ULong)
        Dim unidad As New DriveInfo(Path.GetPathRoot(carpeta))
        If Not unidad.IsReady OrElse CULng(unidad.AvailableFreeSpace) < requerido Then Throw NoDisponible()
    End Sub
    Private Shared Function Invalida() As PerfilImportacionException
        Return New PerfilImportacionException("SOLICITUD_INVALIDA", "Las referencias o metadatos del archivo no son válidos.")
    End Function
    Private Shared Function NoDisponible() As PerfilImportacionException
        Return New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "No se pudo comprobar el almacenamiento privado.")
    End Function
    Private Shared Function Conflicto() As PerfilImportacionException
        Return New PerfilImportacionException("OPERACION_EN_CURSO", "Existe una recepción o evidencia previa; consulte su estado antes de repetir.")
    End Function
    Private Shared Function Limite() As PerfilImportacionException
        Return New PerfilImportacionException("LIMITE_EXCEDIDO", "El contenido excede los bytes o la capacidad temporal permitidos.")
    End Function
    Private Shared Function Rechazado() As PerfilImportacionException
        Return New PerfilImportacionException("ARCHIVO_RECHAZADO", "El contenido está incompleto o no corresponde a un PDF admitido.")
    End Function
    Private Shared Function Incierto() As PerfilImportacionException
        Return New PerfilImportacionException("RESULTADO_INCIERTO", "No se pudo verificar el staging retenido; no se autoriza el efecto documental.")
    End Function
    Private Shared Function ContenidoDistinto() As PerfilImportacionException
        Return New PerfilImportacionException("CONTENIDO_DISTINTO", "El reenvío no corresponde al contenido retenido; no se sustituye el archivo.")
    End Function
End Class
