Option Strict On
Imports System
Imports System.IO
Imports System.Linq
Imports System.Security.Cryptography

Public Class AlmacenArchivoExcel
    Implements IAlmacenArchivoExcel
    Private ReadOnly _raiz As String
    Public Sub New(raiz As String)
        _raiz = Path.GetFullPath(raiz)
        ComprobarRuta(_raiz)
    End Sub
    Public Shared Function NombreSeguro(nombre As String) As String
        Dim partes = If(nombre, "").Replace("\", "/").Split("/"c)
        Dim limpio = partes(partes.Length - 1)
        If String.IsNullOrWhiteSpace(limpio) OrElse limpio.Length > 255 OrElse limpio.Any(Function(c) Char.IsControl(c) OrElse Char.IsSurrogate(c) OrElse Path.GetInvalidFileNameChars().Contains(c)) Then Throw ReglasCargaExcel.Fallo("NOMBRE_ARCHIVO_INVALIDO")
        Return limpio
    End Function
    Public Function Recibir(c As ContextoCargaExcel, id As String, nombre As String, formato As String, contenido As Stream, limites As LimitesLecturaExcel) As ArchivoRecibidoExcel Implements IAlmacenArchivoExcel.Recibir
        ReglasCargaExcel.Autorizar(c) : ReglasCargaExcel.Limites(limites)
        nombre = NombreSeguro(nombre)
        If contenido Is Nothing OrElse Not contenido.CanRead Then Throw ReglasCargaExcel.Fallo("ARCHIVO_INVALIDO")
        Dim relativo = Referencia(c, id), final = Ruta(relativo), parte = final & ".part", creado As Boolean = False
        Directory.CreateDirectory(Path.GetDirectoryName(final)) : ComprobarRuta(Path.GetDirectoryName(final))
        Try
            Dim cantidad As Long = 0, huella As String
            Using salida As New FileStream(parte, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None), digest = SHA256.Create()
                creado = True
                Dim buffer(65535) As Byte, leidos As Integer
                Do
                    leidos = contenido.Read(buffer, 0, buffer.Length)
                    If leidos = 0 Then Exit Do
                    cantidad += leidos
                    If cantidad > CLng(limites.MaxArchivoMB) * 1000000L Then Throw ReglasCargaExcel.Fallo("LIMITE_ARCHIVO_EXCEDIDO")
                    salida.Write(buffer, 0, leidos) : digest.TransformBlock(buffer, 0, leidos, buffer, 0)
                Loop
                If cantidad = 0 Then Throw ReglasCargaExcel.Fallo("ARCHIVO_VACIO")
                digest.TransformFinalBlock(New Byte() {}, 0, 0)
                huella = BitConverter.ToString(digest.Hash).Replace("-", "").ToLowerInvariant()
                salida.Flush(True) : salida.Position = 0
                Dim lector As New LectorArchivoExcel
                Dim inspeccion = lector.Inspeccionar(salida, formato, nombre, limites)
                Dim resultado As New ArchivoRecibidoExcel With {.Nombre = nombre, .Referencia = relativo, .Bytes = cantidad, .Hash = huella, .Formato = formato, .Hojas = inspeccion.Hojas}
                'El handle se cierra antes del rename; el lock de carga sigue retenido por el servicio.
                salida.Close()
                If File.Exists(final) Then
                    Using retenido = Retener(c, id, resultado)
                        retenido.Verificar()
                    End Using
                    File.Delete(parte) : creado = False
                Else
                    File.Move(parte, final) : creado = False
                End If
                Return resultado
            End Using
        Catch ex As IOException
            Throw ReglasCargaExcel.Fallo("RESULTADO_INCIERTO")
        Finally
            If creado Then
                Try
                    File.Delete(parte)
                Catch
                    'No se elimina el archivo completo ni se reclama un .part ajeno.
                End Try
            End If
        End Try
    End Function
    Public Function Retener(c As ContextoCargaExcel, id As String, archivo As ArchivoRecibidoExcel) As IArchivoRetenidoExcel Implements IAlmacenArchivoExcel.Retener
        ReglasCargaExcel.Autorizar(c)
        If archivo Is Nothing OrElse archivo.Referencia <> Referencia(c, id) OrElse archivo.Bytes < 1 OrElse Not ReglasCargaExcel.HashValido(archivo.Hash) Then Throw ReglasCargaExcel.Fallo("ARCHIVO_ALTERADO")
        Dim retenido As New Retencion(Ruta(archivo.Referencia), archivo)
        Try
            retenido.Verificar()
            Return retenido
        Catch
            retenido.Dispose() : Throw
        End Try
    End Function
    Private Shared Function Referencia(c As ContextoCargaExcel, id As String) As String
        If Not ReglasCargaExcel.IdValido(id) Then Throw ReglasCargaExcel.Fallo("REFERENCIA_NO_DISPONIBLE")
        Return c.EmpresaId.ToString(Globalization.CultureInfo.InvariantCulture) & "/" & c.ModuloId.ToString(Globalization.CultureInfo.InvariantCulture) & "/" & id & "/contenido.bin"
    End Function
    Private Function Ruta(relativo As String) As String
        Dim result = Path.GetFullPath(Path.Combine(_raiz, relativo.Replace("/"c, Path.DirectorySeparatorChar)))
        If Not result.StartsWith(_raiz.TrimEnd(Path.DirectorySeparatorChar) & Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) Then Throw ReglasCargaExcel.Fallo("ARCHIVO_INVALIDO")
        ComprobarRuta(result)
        Return result
    End Function
    Private Shared Sub ComprobarRuta(ruta As String)
        Dim actual = Path.GetFullPath(ruta)
        Do While Not String.IsNullOrEmpty(actual)
            If (File.Exists(actual) OrElse Directory.Exists(actual)) AndAlso (File.GetAttributes(actual) And FileAttributes.ReparsePoint) <> 0 Then Throw ReglasCargaExcel.Fallo("ALMACEN_NO_DISPONIBLE")
            actual = Path.GetDirectoryName(actual)
        Loop
    End Sub
    Private Class Retencion
        Implements IArchivoRetenidoExcel
        Private ReadOnly _stream As FileStream
        Private ReadOnly _archivo As ArchivoRecibidoExcel
        Public Sub New(ruta As String, archivo As ArchivoRecibidoExcel)
            _stream = New FileStream(ruta, FileMode.Open, FileAccess.Read, FileShare.Read)
            _archivo = archivo
        End Sub
        Public ReadOnly Property Contenido As Stream Implements IArchivoRetenidoExcel.Contenido
            Get
                Return _stream
            End Get
        End Property
        Public Sub Verificar() Implements IArchivoRetenidoExcel.Verificar
            Dim posicion = _stream.Position
            Try
                _stream.Position = 0
                Using digest = SHA256.Create()
                    If _stream.Length <> _archivo.Bytes OrElse BitConverter.ToString(digest.ComputeHash(_stream)).Replace("-", "").ToLowerInvariant() <> _archivo.Hash Then Throw ReglasCargaExcel.Fallo("ARCHIVO_ALTERADO")
                End Using
            Finally
                _stream.Position = posicion
            End Try
        End Sub
        Public Sub Dispose() Implements IDisposable.Dispose
            _stream.Dispose()
        End Sub
    End Class
End Class
