Option Strict On
Option Compare Binary
Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Security.Cryptography
Imports System.Text
Imports Microsoft.Win32.SafeHandles

Public NotInheritable Partial Class AlmacenStagingImportacion
    Implements IAlmacenStagingV2Importacion

    'Conservador: otro AppDomain/worker/reinicio no puede usar esta observación.
    'No se deriva de MachineName, PID, fecha ni de una declaración del operador.
    Private Shared ReadOnly EpocaStagingV2 As String = Guid.NewGuid().ToString("N")

    Private Function ConservarOriginalV2(contexto As ContextoImportacionCarpetas) As IRetencionOriginalV2Importacion Implements IAlmacenStagingV2Importacion.ConservarOriginalV2
        Return AbrirOriginalV2(contexto, Nothing)
    End Function

    Private Function AbrirGuardiaV2(contexto As ContextoImportacionCarpetas, documento As DocumentoAlmacenamientoV2Importacion) As IGuardiaOriginalV2Importacion Implements IAlmacenStagingV2Importacion.AbrirGuardiaV2
        'El codec valida forma, no procedencia: RV-04/RV-05 deben leer y comparar
        'inicio/diario del servidor antes de usar este puerto en una recuperación.
        Dim copia = DocumentosAlmacenamientoV2Importacion.Leer(DocumentosAlmacenamientoV2Importacion.Escribir(documento))
        Autorizar(contexto)
        Dim inicio = copia.Inicio
        If inicio.LoteId <> _lote.LoteId OrElse inicio.RaizId <> _archivo.RaizId OrElse inicio.ArchivoId <> _archivo.ArchivoId OrElse inicio.IntentoId <> _archivo.IntentoActivoId OrElse inicio.UsuarioRef <> contexto.UsuarioRef OrElse inicio.AmbitoRef <> contexto.AmbitoRef OrElse inicio.NivelRef <> contexto.NivelRef OrElse inicio.RevisionLote > _lote.Revision OrElse inicio.RevisionArchivo > _archivo.Revision Then Throw Incierto()
        If inicio.ContextoHash <> HuellaStagingV2(New With {contexto.UsuarioRef, contexto.AmbitoRef, contexto.NivelRef, contexto.RutaCanonica, contexto.SelloDestino}) OrElse inicio.SnapshotHash <> _lote.Snapshot.Huella OrElse inicio.RecepcionHash <> HuellaStagingV2(New With {_archivo.StagingRef, _archivo.BytesRecibidos, _archivo.ContenidoHash}) OrElse inicio.ValoresHash <> HuellaStagingV2(_archivo.ValoresEfectivos) Then Throw Incierto()
        If inicio.Original.NodoRef <> EpocaStagingV2 Then Throw NoDisponible()
        Return AbrirOriginalV2(contexto, inicio.Original)
    End Function

    Private Function AbrirOriginalV2(contexto As ContextoImportacionCarpetas, esperado As OriginalAlmacenamientoV2Importacion) As RetencionStagingV2
        Autorizar(contexto)
        If _lote.Estado <> "EN_CURSO" OrElse _archivo.EstadoEfecto <> "EN_CURSO" OrElse Not ValidadorPerfilImportacion.ReferenciaValida(_archivo.IntentoActivoId) OrElse _archivo.EstadoRecepcion <> "RECIBIDO" OrElse _archivo.BytesRecibidos <> _archivo.BytesPrevistos OrElse Not HexadecimalValido(_archivo.StagingRef, 32) OrElse Not HexadecimalValido(_archivo.ContenidoHash, 64) OrElse _archivo.ValoresEfectivos Is Nothing Then Throw Invalida()
        Dim bytes = ExigirArchivo(), entrada As FileStream = Nothing, bloqueo As FileStream = Nothing
        Dim directorios As List(Of SafeFileHandle) = Nothing
        Try
            Dim original = Ruta(CarpetaArchivo(contexto), _archivo.StagingRef & ".bin")
            ExigirProveedorLocalV2(original)
            'OPEN_EXISTING y handles sin FILE_SHARE_DELETE fijan los ancestros;
            'no crea directorios, recepcion.lock, originales o copias de trabajo.
            directorios = FijarDirectoriosV2(Path.GetDirectoryName(original))
            Dim rutaBloqueo = Path.Combine(CarpetaLote(contexto), "recepcion.lock")
            ExigirSinEnlaces(rutaBloqueo)
            bloqueo = New FileStream(rutaBloqueo, FileMode.Open, FileAccess.Read, FileShare.None)
            ExigirRutaHandleV2(bloqueo.SafeFileHandle, rutaBloqueo)
            ExigirSinEnlaces(original)
            entrada = New FileStream(original, FileMode.Open, FileAccess.Read, If(esperado Is Nothing, FileShare.Read, FileShare.None), 4096, FileOptions.SequentialScan)
            ExigirRutaHandleV2(entrada.SafeFileHandle, original)
            Dim informacion = InformacionV2(entrada.SafeFileHandle, False)
            If CULng(entrada.Length) <> bytes Then Throw Incierto()
            Dim contenido = CopiarAcotado(entrada, Stream.Null, bytes)
            entrada.Position = 0
            If contenido <> _archivo.ContenidoHash Then Throw Incierto()
            If esperado Is Nothing Then ValidarFormato(original)
            Dim indiceRaiz = directorios.Count - 1
            Dim raiz = Path.GetDirectoryName(original)
            While Not String.Equals(raiz.TrimEnd(Path.DirectorySeparatorChar), _raiz.TrimEnd(Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase)
                raiz = Path.GetDirectoryName(raiz) : indiceRaiz -= 1
                If String.IsNullOrEmpty(raiz) OrElse indiceRaiz < 0 Then Throw NoDisponible()
            End While
            Dim prueba As New OriginalAlmacenamientoV2Importacion With {
                .ProveedorRef = HuellaStagingV2(New With {.protocolo = "NTFS_LOCAL_V2", .epoca = EpocaStagingV2, .raiz = IdentidadNativaV2(InformacionV2(directorios(indiceRaiz), True))}).Substring(0, 32),
                .NodoRef = EpocaStagingV2, .IdentidadFisicaHash = HuellaStagingV2(IdentidadNativaV2(informacion)),
                .ContenidoHash = contenido, .Bytes = bytes.ToString(CultureInfo.InvariantCulture)}
            If esperado IsNot Nothing AndAlso PerfilImportacionJson.Escribir(esperado) <> PerfilImportacionJson.Escribir(prueba) Then Throw Incierto()
            Dim retencion As New RetencionStagingV2(entrada, directorios, prueba, esperado IsNot Nothing, original, bytes)
            entrada = Nothing : directorios = Nothing
            Return retencion
        Catch ex As IOException When (ex.HResult And &HFFFF) = 32 OrElse (ex.HResult And &HFFFF) = 33
            Throw Conflicto()
        Catch ex As IOException
            Throw Incierto()
        Catch ex As UnauthorizedAccessException
            Throw NoDisponible()
        Catch ex As System.Security.SecurityException
            Throw NoDisponible()
        Catch ex As Exception When TypeOf ex Is DllNotFoundException OrElse TypeOf ex Is EntryPointNotFoundException OrElse TypeOf ex Is PlatformNotSupportedException
            Throw NoDisponible()
        Finally
            If bloqueo IsNot Nothing Then bloqueo.Dispose()
            If entrada IsNot Nothing Then entrada.Dispose()
            If directorios IsNot Nothing Then
                For Each handle In directorios
                    handle.Dispose()
                Next
            End If
        End Try
    End Function

    Private NotInheritable Class RetencionStagingV2
        Implements IRetencionOriginalV2Importacion, IGuardiaOriginalV2Importacion
        Private ReadOnly _sincronizacion As New Object()
        Private _entrada As FileStream
        Private _directorios As List(Of SafeFileHandle)
        Private ReadOnly _prueba As String, _original As String, _bytes As ULong, _exclusiva As Boolean
        Private _enOperacion As Boolean
        Public Sub New(entrada As FileStream, directorios As List(Of SafeFileHandle), prueba As OriginalAlmacenamientoV2Importacion, exclusiva As Boolean, original As String, bytes As ULong)
            _entrada = entrada : _directorios = directorios : _prueba = PerfilImportacionJson.Escribir(prueba)
            _exclusiva = exclusiva : _original = original : _bytes = bytes
        End Sub
        Public ReadOnly Property Original As OriginalAlmacenamientoV2Importacion Implements IRetencionOriginalV2Importacion.Original
            Get
                SyncLock _sincronizacion
                    Comprobar()
                    Return PerfilImportacionJson.Leer(Of OriginalAlmacenamientoV2Importacion)(_prueba)
                End SyncLock
            End Get
        End Property
        Public Sub Validar() Implements IRetencionOriginalV2Importacion.Validar
            SyncLock _sincronizacion
                Comprobar()
            End SyncLock
        End Sub
        Public Sub ConservarDurante(operacion As Action) Implements IGuardiaOriginalV2Importacion.ConservarDurante
            SyncLock _sincronizacion
                If Not _exclusiva OrElse _enOperacion OrElse operacion Is Nothing Then Throw Conflicto()
                Comprobar() : _enOperacion = True
                Try
                    operacion()
                    Comprobar()
                Finally
                    _enOperacion = False
                End Try
            End SyncLock
        End Sub
        Private Sub Comprobar()
            If _entrada Is Nothing OrElse _directorios Is Nothing Then Throw Conflicto()
            Try
                For Each handle In _directorios
                    InformacionV2(handle, True)
                Next
                ExigirRutaHandleV2(_entrada.SafeFileHandle, _original)
                Dim prueba = PerfilImportacionJson.Leer(Of OriginalAlmacenamientoV2Importacion)(_prueba)
                If HuellaStagingV2(IdentidadNativaV2(InformacionV2(_entrada.SafeFileHandle, False))) <> prueba.IdentidadFisicaHash OrElse CULng(_entrada.Length) <> _bytes Then Throw Incierto()
                _entrada.Position = 0
                Using sha = SHA256.Create()
                    Dim hash = BitConverter.ToString(sha.ComputeHash(_entrada)).Replace("-", "").ToLowerInvariant()
                    If hash <> prueba.ContenidoHash Then Throw Incierto()
                End Using
                _entrada.Position = 0
            Catch ex As IOException
                Throw Incierto()
            Catch ex As UnauthorizedAccessException
                Throw NoDisponible()
            End Try
        End Sub
        Public Sub Dispose() Implements IDisposable.Dispose
            SyncLock _sincronizacion
                'Monitor es reentrante: tampoco liberar dentro de la acción.
                If _enOperacion Then Throw Conflicto()
                Try
                    If _entrada IsNot Nothing Then _entrada.Dispose()
                Finally
                    _entrada = Nothing
                    If _directorios IsNot Nothing Then
                        For Each handle In _directorios
                            handle.Dispose()
                        Next
                    End If
                    _directorios = Nothing
                End Try
            End SyncLock
        End Sub
    End Class

    <StructLayout(LayoutKind.Sequential)>
    Private Structure InformacionArchivoV2
        Public Atributos As UInteger
        Public Creado As System.Runtime.InteropServices.ComTypes.FILETIME
        Public Accedido As System.Runtime.InteropServices.ComTypes.FILETIME
        Public Escrito As System.Runtime.InteropServices.ComTypes.FILETIME
        Public Volumen As UInteger
        Public TamanoAlto As UInteger
        Public TamanoBajo As UInteger
        Public Enlaces As UInteger
        Public IndiceAlto As UInteger
        Public IndiceBajo As UInteger
    End Structure
    <DllImport("kernel32.dll", SetLastError:=True)>
    Private Shared Function GetFileInformationByHandle(handle As SafeFileHandle, ByRef informacion As InformacionArchivoV2) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function
    <DllImport("kernel32.dll", CharSet:=CharSet.Unicode, SetLastError:=True)>
    Private Shared Function CreateFileW(nombre As String, acceso As UInteger, compartir As UInteger, seguridad As IntPtr, disposicion As UInteger, atributos As UInteger, plantilla As IntPtr) As SafeFileHandle
    End Function
    <DllImport("kernel32.dll", CharSet:=CharSet.Unicode, SetLastError:=True)>
    Private Shared Function GetFinalPathNameByHandleW(handle As SafeFileHandle, destino As StringBuilder, capacidad As UInteger, flags As UInteger) As UInteger
    End Function
    <DllImport("kernel32.dll", CharSet:=CharSet.Unicode)>
    Private Shared Function GetDriveTypeW(raiz As String) As UInteger
    End Function
    <DllImport("kernel32.dll", CharSet:=CharSet.Unicode, SetLastError:=True)>
    Private Shared Function GetVolumeInformationW(raiz As String, nombre As StringBuilder, capacidad As UInteger, ByRef serial As UInteger, ByRef componente As UInteger, ByRef flags As UInteger, sistema As StringBuilder, capacidadSistema As UInteger) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    Private Shared Sub ExigirProveedorLocalV2(original As String)
        If Environment.OSVersion.Platform <> PlatformID.Win32NT OrElse original.Length < 3 OrElse original.StartsWith("\\", StringComparison.Ordinal) OrElse original(1) <> ":"c OrElse original(2) <> "\"c OrElse original.IndexOf(":"c, 2) >= 0 Then Throw NoDisponible()
        Dim serial As UInteger, componente As UInteger, flags As UInteger, sistema As New StringBuilder(64)
        Dim raiz = Path.GetPathRoot(original)
        If GetDriveTypeW(raiz) <> 3UI OrElse Not GetVolumeInformationW(raiz, Nothing, 0, serial, componente, flags, sistema, 64) OrElse sistema.ToString() <> "NTFS" Then Throw NoDisponible()
    End Sub
    Private Shared Function FijarDirectoriosV2(carpeta As String) As List(Of SafeFileHandle)
        Dim rutas As New List(Of String)(), retenidos As New List(Of SafeFileHandle)()
        Dim actual = carpeta
        While Not String.IsNullOrEmpty(actual)
            rutas.Add(actual) : actual = Path.GetDirectoryName(actual)
        End While
        rutas.Reverse()
        Try
            For Each directorioActual As String In rutas
                'FILE_READ_ATTRIBUTES; compartir lectura/escritura, nunca borrado.
                Dim handle = CreateFileW(directorioActual, &H80UI, 3UI, IntPtr.Zero, 3UI, &H2200000UI, IntPtr.Zero)
                If handle.IsInvalid Then
                    Dim codigo = Marshal.GetLastWin32Error()
                    handle.Dispose()
                    If codigo = 32 OrElse codigo = 33 Then Throw Conflicto()
                    Throw NoDisponible()
                End If
                retenidos.Add(handle)
                InformacionV2(handle, True) : ExigirRutaHandleV2(handle, directorioActual)
            Next
            Return retenidos
        Catch
            For Each handle In retenidos
                handle.Dispose()
            Next
            Throw
        End Try
    End Function
    Private Shared Function InformacionV2(handle As SafeFileHandle, directorio As Boolean) As InformacionArchivoV2
        Dim info As InformacionArchivoV2
        If handle Is Nothing OrElse handle.IsClosed OrElse handle.IsInvalid OrElse Not GetFileInformationByHandle(handle, info) Then Throw NoDisponible()
        If (info.Atributos And &H400UI) <> 0 OrElse ((info.Atributos And &H10UI) <> 0) <> directorio OrElse (info.IndiceAlto = 0 AndAlso info.IndiceBajo = 0) OrElse (Not directorio AndAlso info.Enlaces <> 1) Then Throw NoDisponible()
        Return info
    End Function
    Private Shared Function IdentidadNativaV2(info As InformacionArchivoV2) As String
        Return info.Volumen.ToString("x8", CultureInfo.InvariantCulture) & ":" & info.IndiceAlto.ToString("x8", CultureInfo.InvariantCulture) & info.IndiceBajo.ToString("x8", CultureInfo.InvariantCulture) & ":" & info.Creado.dwHighDateTime.ToString("x8", CultureInfo.InvariantCulture) & info.Creado.dwLowDateTime.ToString("x8", CultureInfo.InvariantCulture)
    End Function
    Private Shared Sub ExigirRutaHandleV2(handle As SafeFileHandle, ruta As String)
        Dim destino As New StringBuilder(32768)
        Dim longitud = GetFinalPathNameByHandleW(handle, destino, CUInt(destino.Capacity), 0UI)
        If longitud = 0 OrElse longitud >= CUInt(destino.Capacity) Then Throw NoDisponible()
        Dim final = destino.ToString()
        If Not final.StartsWith("\\?\", StringComparison.Ordinal) Then Throw NoDisponible()
        final = final.Substring(4).TrimEnd(Path.DirectorySeparatorChar)
        If Not String.Equals(final, Path.GetFullPath(ruta).TrimEnd(Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase) Then Throw NoDisponible()
    End Sub
    Private Shared Function HuellaStagingV2(valor As Object) As String
        Using sha = SHA256.Create()
            Return BitConverter.ToString(sha.ComputeHash(Encoding.ASCII.GetBytes(PerfilImportacionJson.Escribir(valor)))).Replace("-", "").ToLowerInvariant()
        End Using
    End Function
End Class
