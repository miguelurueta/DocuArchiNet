Imports System
Imports System.Collections.Concurrent
Imports System.Configuration
Imports System.Diagnostics
Imports System.Globalization
Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading

Public NotInheritable Class ResultadoPreparacionPdf
    Public Property RutaVisualizacion As String
    Public Property RutaVistaPreviaPrimeraPagina As String
    Public Property ModoCarga As String
End Class

' Prepara, sin modificar el original, una representación temporal apta para rangos.
' La primera solicitud prepara solo una página visible; el derivado completo queda en segundo plano.
Public NotInheritable Class PreparadorVisualizacionPdf
    Private Const ClaveHabilitado As String = "PdfLinearizedDerivativeEnabled"
    Private Const ClaveEjecutable As String = "PdfLinearizedDerivativeExecutable"
    Private Const ClaveDirectorio As String = "PdfLinearizedDerivativeCacheDirectory"
    Private Const ClaveTimeout As String = "PdfLinearizedDerivativeTimeoutSeconds"
    Private Const ClaveRetencion As String = "PdfLinearizedDerivativeRetentionHours"
    Private Const ClaveMaximoPrimeraSeccion As String = "PdfLinearizedDerivativeFirstSectionMaximumBytes"
    Private Const ClaveFraccionPrimeraSeccion As String = "PdfLinearizedDerivativeFirstSectionMaximumFraction"
    Private Shared ReadOnly EnPreparacion As New ConcurrentDictionary(Of String, Byte)(StringComparer.OrdinalIgnoreCase)

    Private Sub New()
    End Sub

    Public Shared Function Preparar(ByVal rutaOriginal As String) As ResultadoPreparacionPdf
        Dim resultado As New ResultadoPreparacionPdf With {
            .RutaVisualizacion = rutaOriginal,
            .ModoCarga = EstrategiaCargaPdf.Seleccionar(rutaOriginal)
        }
        If resultado.ModoCarga = EstrategiaCargaPdf.ModoRangos AndAlso EsDerivadoEficaz(rutaOriginal) Then Return resultado
        If Not EstaHabilitado() OrElse
           (resultado.ModoCarga <> EstrategiaCargaPdf.ModoRangos AndAlso
            Not EstrategiaCargaPdf.EsCandidatoDerivado(rutaOriginal)) Then Return resultado

        Try
            Dim original As New FileInfo(rutaOriginal)
            Dim clave As String = CrearClave(original)
            Dim rutaDerivado As String = Path.Combine(ObtenerDirectorioCache(), clave & ".pdf")
            Dim rutaVistaPrevia As String = Path.Combine(ObtenerDirectorioCache(), clave & ".first.pdf")
            Dim derivadoValido As Boolean = EsDerivadoValido(rutaDerivado)
            Dim vistaPreviaValida As Boolean = EsPdfValido(rutaVistaPrevia)
            If derivadoValido AndAlso vistaPreviaValida Then
                RegistrarAcceso(rutaDerivado)
                RegistrarAcceso(rutaVistaPrevia)
                resultado.RutaVisualizacion = rutaDerivado
                resultado.RutaVistaPreviaPrimeraPagina = rutaVistaPrevia
                resultado.ModoCarga = If(EsDerivadoEficaz(rutaDerivado),
                                         EstrategiaCargaPdf.ModoRangos,
                                         EstrategiaCargaPdf.ModoStreamingCompleto)
                Return resultado
            End If
            Try
                If File.Exists(rutaDerivado) AndAlso Not derivadoValido Then
                    If Not EsCachePropia(rutaDerivado) Then Return resultado
                    File.Delete(rutaDerivado)
                End If
                If File.Exists(rutaVistaPrevia) AndAlso Not vistaPreviaValida Then
                    If Not EsCachePropia(rutaVistaPrevia) Then Return resultado
                    File.Delete(rutaVistaPrevia)
                End If
            Catch
                Return resultado
            End Try

            If EnPreparacion.TryAdd(clave, 0) Then
                Dim capturaRuta As String = original.FullName
                Dim capturaLongitud As Long = original.Length
                Dim capturaEscritura As Long = original.LastWriteTimeUtc.Ticks
                vistaPreviaValida = vistaPreviaValida OrElse
                    GenerarVistaPrevia(capturaRuta, capturaLongitud, capturaEscritura, rutaVistaPrevia)
                If vistaPreviaValida Then resultado.RutaVistaPreviaPrimeraPagina = rutaVistaPrevia
                If derivadoValido Then
                    resultado.RutaVisualizacion = rutaDerivado
                    resultado.ModoCarga = If(EsDerivadoEficaz(rutaDerivado),
                                             EstrategiaCargaPdf.ModoRangos,
                                             EstrategiaCargaPdf.ModoStreamingCompleto)
                    Dim ignorado As Byte
                    EnPreparacion.TryRemove(clave, ignorado)
                    Return resultado
                End If
                ThreadPool.QueueUserWorkItem(
                    Sub(estado As Object)
                        Try
                            GenerarDerivado(capturaRuta, capturaLongitud, capturaEscritura, rutaDerivado)
                        Finally
                            Dim ignorado As Byte
                            EnPreparacion.TryRemove(clave, ignorado)
                        End Try
                    End Sub)
            End If
        Catch
            ' La optimización nunca impide visualizar el documento original.
        End Try
        Return resultado
    End Function

    Private Shared Sub GenerarDerivado(ByVal rutaOriginal As String,
                                       ByVal longitudOriginal As Long,
                                       ByVal escrituraOriginal As Long,
                                       ByVal rutaDerivado As String)
        Dim temporal As String = rutaDerivado & "." & Guid.NewGuid().ToString("N") & ".tmp.pdf"
        Try
            Dim ejecutable As String = ObtenerEjecutable()
            If String.IsNullOrWhiteSpace(ejecutable) Then Return
            Directory.CreateDirectory(Path.GetDirectoryName(rutaDerivado))
            LimpiarExpirados(Path.GetDirectoryName(rutaDerivado))

            Dim argumentos As String = String.Join(" ", New String() {
                "-q", "-dBATCH", "-dNOPAUSE", "-dSAFER", "-sDEVICE=pdfwrite",
                "-dFastWebView=true", "-sOutputFile=" & EntreComillas(temporal), EntreComillas(rutaOriginal)})
            If Not EjecutarLinealizador(ejecutable, argumentos) Then Return

            Dim originalActual As New FileInfo(rutaOriginal)
            If Not originalActual.Exists OrElse originalActual.Length <> longitudOriginal OrElse
               originalActual.LastWriteTimeUtc.Ticks <> escrituraOriginal OrElse
               Not EsDerivadoValido(temporal) Then Return

            Try
                File.Move(temporal, rutaDerivado)
                RegistrarAcceso(rutaDerivado, True)
            Catch ex As IOException
                ' Otro proceso IIS pudo publicar la misma versión primero.
                If Not EsDerivadoValido(rutaDerivado) Then Throw
            End Try
        Catch
            ' Fallo cerrado: la siguiente apertura continuará usando STREAM del original.
        Finally
            Try
                If File.Exists(temporal) Then File.Delete(temporal)
            Catch
            End Try
        End Try
    End Sub

    Private Shared Function GenerarVistaPrevia(ByVal rutaOriginal As String,
                                                ByVal longitudOriginal As Long,
                                                ByVal escrituraOriginal As Long,
                                                ByVal rutaVistaPrevia As String) As Boolean
        Dim temporal As String = rutaVistaPrevia & "." & Guid.NewGuid().ToString("N") & ".tmp.pdf"
        Try
            Dim ejecutable As String = ObtenerEjecutable()
            If String.IsNullOrWhiteSpace(ejecutable) Then Return False
            Directory.CreateDirectory(Path.GetDirectoryName(rutaVistaPrevia))
            Dim argumentos As String = String.Join(" ", New String() {
                "-q", "-dBATCH", "-dNOPAUSE", "-dSAFER", "-sDEVICE=pdfwrite",
                "-dFirstPage=1", "-dLastPage=1", "-dFastWebView=true",
                "-sOutputFile=" & EntreComillas(temporal), EntreComillas(rutaOriginal)})
            If Not EjecutarLinealizador(ejecutable, argumentos) Then Return False
            Dim originalActual As New FileInfo(rutaOriginal)
            If Not originalActual.Exists OrElse originalActual.Length <> longitudOriginal OrElse
               originalActual.LastWriteTimeUtc.Ticks <> escrituraOriginal OrElse
               Not EsPdfValido(temporal) Then Return False
            Try
                File.Move(temporal, rutaVistaPrevia)
                RegistrarAcceso(rutaVistaPrevia, True)
            Catch ex As IOException
                If Not EsPdfValido(rutaVistaPrevia) Then Throw
            End Try
            RegistrarAcceso(rutaVistaPrevia)
            Return True
        Catch
            Return False
        Finally
            Try
                If File.Exists(temporal) Then File.Delete(temporal)
            Catch
            End Try
        End Try
    End Function

    Private Shared Function EjecutarLinealizador(ByVal ejecutable As String, ByVal argumentos As String) As Boolean
        Dim inicio As New ProcessStartInfo With {
            .FileName = ejecutable,
            .Arguments = argumentos,
            .UseShellExecute = False,
            .CreateNoWindow = True,
            .WindowStyle = ProcessWindowStyle.Hidden,
            .WorkingDirectory = Path.GetDirectoryName(ejecutable),
            .RedirectStandardOutput = True,
            .RedirectStandardError = True
        }
        Using proceso As Process = Process.Start(inicio)
            If proceso Is Nothing Then Return False
            Dim salida = proceso.StandardOutput.ReadToEndAsync()
            Dim errorSalida = proceso.StandardError.ReadToEndAsync()
            If Not proceso.WaitForExit(ObtenerTimeoutMilisegundos()) Then
                Try
                    proceso.Kill()
                Catch
                End Try
                Return False
            End If
            If proceso.ExitCode <> 0 Then Return False
            salida.Wait()
            errorSalida.Wait()
            Return True
        End Using
    End Function

    Private Shared Function EstaHabilitado() As Boolean
        Dim habilitado As Boolean
        Return Boolean.TryParse(ConfigurationManager.AppSettings(ClaveHabilitado), habilitado) AndAlso habilitado
    End Function

    Private Shared Function ObtenerEjecutable() As String
        Try
            Dim configurado As String = ConfigurationManager.AppSettings(ClaveEjecutable)
            If String.IsNullOrWhiteSpace(configurado) OrElse Not Path.IsPathRooted(configurado) Then Return String.Empty
            Dim ruta As String = Path.GetFullPath(configurado)
            If Not File.Exists(ruta) OrElse Not String.Equals(Path.GetExtension(ruta), ".exe", StringComparison.OrdinalIgnoreCase) Then Return String.Empty
            Return ruta
        Catch
            Return String.Empty
        End Try
    End Function

    Private Shared Function ObtenerDirectorioCache() As String
        Dim configurado As String = ConfigurationManager.AppSettings(ClaveDirectorio)
        If String.IsNullOrWhiteSpace(configurado) Then
            Return Path.Combine(Path.GetTempPath(), "DocuArchiNet", "PdfLinearized")
        End If
        Return Path.GetFullPath(configurado)
    End Function

    Private Shared Function ObtenerTimeoutMilisegundos() As Integer
        Dim segundos As Integer
        If Not Integer.TryParse(ConfigurationManager.AppSettings(ClaveTimeout), NumberStyles.None, CultureInfo.InvariantCulture, segundos) OrElse segundos < 5 Then segundos = 120
        segundos = Math.Min(segundos, 600)
        Return segundos * 1000
    End Function

    Private Shared Function ObtenerRetencionHoras() As Integer
        Dim horas As Integer
        If Not Integer.TryParse(ConfigurationManager.AppSettings(ClaveRetencion), NumberStyles.None, CultureInfo.InvariantCulture, horas) OrElse horas < 1 Then horas = 24
        Return Math.Min(horas, 168)
    End Function

    Private Shared Function ObtenerMaximoPrimeraSeccion() As Long
        Dim bytes As Long
        If Not Long.TryParse(ConfigurationManager.AppSettings(ClaveMaximoPrimeraSeccion), NumberStyles.None,
                             CultureInfo.InvariantCulture, bytes) OrElse bytes < 1048576L Then bytes = 33554432L
        Return Math.Min(bytes, 536870912L)
    End Function

    Private Shared Function ObtenerFraccionPrimeraSeccion() As Double
        Dim fraccion As Double
        If Not Double.TryParse(ConfigurationManager.AppSettings(ClaveFraccionPrimeraSeccion), NumberStyles.AllowDecimalPoint,
                               CultureInfo.InvariantCulture, fraccion) OrElse fraccion <= 0 OrElse fraccion >= 1 Then fraccion = 0.5
        Return fraccion
    End Function

    Private Shared Function CrearClave(ByVal archivo As FileInfo) As String
        Dim identidad As String = archivo.FullName & ChrW(31) & archivo.Length.ToString(CultureInfo.InvariantCulture) & ChrW(31) & archivo.LastWriteTimeUtc.Ticks.ToString(CultureInfo.InvariantCulture)
        Using sha As SHA256 = SHA256.Create()
            Return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(identidad))).Replace("-", String.Empty).ToLowerInvariant()
        End Using
    End Function

    Private Shared Function EsDerivadoValido(ByVal ruta As String) As Boolean
        Return File.Exists(ruta) AndAlso EstrategiaCargaPdf.Seleccionar(ruta) = EstrategiaCargaPdf.ModoRangos
    End Function

    Private Shared Function EsDerivadoEficaz(ByVal ruta As String) As Boolean
        Try
            Dim archivo As New FileInfo(ruta)
            If Not archivo.Exists OrElse archivo.Length <= 0 Then Return False
            Dim cantidad As Integer = CInt(Math.Min(4096L, archivo.Length))
            Dim cabecera(cantidad - 1) As Byte
            Using origen As New FileStream(ruta, FileMode.Open, FileAccess.Read, FileShare.ReadWrite Or FileShare.Delete)
                If origen.Read(cabecera, 0, cantidad) <> cantidad Then Return False
            End Using
            Dim longitudDeclarada As Long
            Dim finPrimeraSeccion As Long
            If Not EstrategiaCargaPdf.TryObtenerSeccionInicialLinearizada(cabecera, longitudDeclarada, finPrimeraSeccion) Then Return False
            If longitudDeclarada <> archivo.Length Then Return False
            Return finPrimeraSeccion <= ObtenerMaximoPrimeraSeccion() AndAlso
                   CDbl(finPrimeraSeccion) / CDbl(longitudDeclarada) <= ObtenerFraccionPrimeraSeccion()
        Catch
            Return False
        End Try
    End Function

    Private Shared Function EsPdfValido(ByVal ruta As String) As Boolean
        If Not File.Exists(ruta) Then Return False
        Try
            Dim archivo As New FileInfo(ruta)
            If archivo.Length <= 0 Then Return False
            Dim cantidad As Integer = CInt(Math.Min(4096L, archivo.Length))
            Dim cabecera(cantidad - 1) As Byte
            Using origen As New FileStream(ruta, FileMode.Open, FileAccess.Read, FileShare.ReadWrite Or FileShare.Delete)
                Return origen.Read(cabecera, 0, cantidad) = cantidad AndAlso EstrategiaCargaPdf.EsCabeceraPdf(cabecera)
            End Using
        Catch
            Return False
        End Try
    End Function

    Private Shared Function EntreComillas(ByVal valor As String) As String
        Return ChrW(34) & valor & ChrW(34)
    End Function

    Private Shared Sub LimpiarExpirados(ByVal directorio As String)
        Try
            If Not EsRutaSinReparse(directorio) Then Return
            Dim limite As DateTime = DateTime.UtcNow.AddHours(-ObtenerRetencionHoras())
            For Each ruta As String In Directory.GetFiles(directorio, "*.pdf")
                Try
                    If Not EsCachePropia(ruta) Then Continue For
                    If EnPreparacion.ContainsKey(Path.GetFileName(ruta).Substring(0, 64)) Then Continue For
                    Dim marcaUso As String = ruta & ".use"
                    Dim ultimoUso As DateTime = File.GetLastWriteTimeUtc(marcaUso)
                    If ultimoUso < limite Then
                        File.Delete(ruta)
                        If File.Exists(marcaUso) Then File.Delete(marcaUso)
                    End If
                Catch
                End Try
            Next
        Catch
        End Try
    End Sub

    Private Shared Function EsRutaSinReparse(ByVal ruta As String) As Boolean
        Try
            Dim actual As String = Path.GetFullPath(ruta)
            While Not String.IsNullOrEmpty(actual)
                If (File.GetAttributes(actual) And FileAttributes.ReparsePoint) <> 0 Then Return False
                actual = Path.GetDirectoryName(actual)
            End While
            Return True
        Catch
            Return False
        End Try
    End Function

    Private Shared Function EsCachePropia(ByVal ruta As String) As Boolean
        Try
            Dim nombre As String = Path.GetFileName(ruta)
            If Not Regex.IsMatch(nombre, "\A[0-9a-f]{64}(\.first)?\.pdf\z", RegexOptions.CultureInvariant) Then Return False
            Dim marca As New FileInfo(ruta & ".use")
            If Not marca.Exists OrElse marca.Length > 256 Then Return False
            If Not EsRutaSinReparse(ruta) OrElse Not EsRutaSinReparse(marca.FullName) Then Return False
            Return String.Equals(File.ReadAllText(marca.FullName), "DocuArchiNet.PdfCache.v1:" & nombre, StringComparison.Ordinal)
        Catch
            Return False
        End Try
    End Function

    Private Shared Sub RegistrarAcceso(ByVal rutaDerivado As String, Optional ByVal publicadoPorEstaOperacion As Boolean = False)
        Try
            Dim marcaUso As String = rutaDerivado & ".use"
            If publicadoPorEstaOperacion Then
                ' Only a successful publication may create ownership; never adopt old/foreign files.
                If Not EsRutaSinReparse(rutaDerivado) Then Return
                If File.Exists(marcaUso) Then
                    If Not EsCachePropia(rutaDerivado) Then Return
                Else
                    Using marca As New FileStream(marcaUso, FileMode.CreateNew, FileAccess.Write, FileShare.None)
                        Dim contenido As Byte() = Encoding.UTF8.GetBytes("DocuArchiNet.PdfCache.v1:" & Path.GetFileName(rutaDerivado))
                        marca.Write(contenido, 0, contenido.Length)
                    End Using
                End If
            End If
            If Not EsCachePropia(rutaDerivado) Then Return
            File.SetLastWriteTimeUtc(marcaUso, DateTime.UtcNow)
        Catch
            ' La marca solo gobierna limpieza; nunca invalida un derivado correcto.
        End Try
    End Sub
End Class
