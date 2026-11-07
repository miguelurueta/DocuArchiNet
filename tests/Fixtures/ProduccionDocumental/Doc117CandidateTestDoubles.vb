Option Strict On
Imports System
Imports System.IO
Imports System.Globalization
Imports System.Threading
Imports System.Threading.Tasks

' Offline doubles only. No HTTP, database, writer or runtime installation.
Public Class ContextoImportacionCarpetas
    Public UsuarioRef As String = "141", AmbitoRef As String = "2", NivelRef As String = "7"
End Class
Public Class ConfiguracionPerfilImportacion
    Public OrigenGabinete As String = "DA"
    Public GabineteId As Nullable(Of Integer) = 10
End Class
Public Class SnapshotPerfilImportacionOperativa
    Public VersionId As String = "38"
    Public Huella As String = New String("f"c, 64)
    Public Configuracion As New ConfiguracionPerfilImportacion()
End Class
Public Class RaizImportacionCarpetas
    Public LoteId As String = "900", RaizId As String = "901"
    Public IdentidadExpediente As New IdentidadExpedienteImportacion()
End Class
Public Class IdentidadExpedienteImportacion
    Public OrigenLogico As String = "RA", NivelRef As String = "7", ExpedienteId As String = "910", RelacionNivelId As String = "911"
End Class
Public Class ArchivoImportacionCarpetas
    Public RaizId As String = "901", ArchivoId As String = "902"
    Public NombreOriginal As String = "test-a.pdf", BytesPrevistos As String = "677", BytesRecibidos As String = "677", ContenidoHash As String = New String("d"c, 64)
End Class
Public Class IntentoImportacionCarpetas
    Public LoteId As String = "900", RaizId As String = "901", ArchivoId As String = "902", IntentoId As String = "903", Fase As String = "ALMACENAR"
    Public Estado As String = "RESERVADO", SolicitudHash As String = New String("a"c, 64), FinUtc As String = Nothing
    Public Resultado As ResultadoEfectoImportacion = Nothing
    Public EsNuevaReserva As Boolean = True
End Class
Public Class ComandoAlmacenamientoCarpetas
    Public EsVerificacion As Boolean = False
    Public Contexto As New ContextoImportacionCarpetas()
    Public Snapshot As New SnapshotPerfilImportacionOperativa()
    Public Raiz As New RaizImportacionCarpetas()
    Public Archivo As New ArchivoImportacionCarpetas()
    Public Intento As New IntentoImportacionCarpetas()
End Class
Public Class ResultadoEfectoImportacion
    Public Estado As String = "CONFIRMADO", DocumentoId As String = "910", InventarioId As String = "911"
End Class
Public Class PerfilImportacionException
    Inherits Exception
    Public Sub New(codigo As String, mensaje As String)
        MyBase.New(codigo)
    End Sub
End Class
Public Class ValidadorPerfilImportacion
    Public Shared Function ReferenciaValida(valor As String) As Boolean
        Dim n As UInteger
        Return UInteger.TryParse(valor, NumberStyles.None, CultureInfo.InvariantCulture, n) AndAlso n > 0 AndAlso n.ToString(CultureInfo.InvariantCulture) = valor
    End Function
End Class
Public Class PerfilImportacionJson
    Public Shared Function Escribir(valor As Object) As String
        Return Newtonsoft.Json.JsonConvert.SerializeObject(valor)
    End Function
End Class
Public Class MySqlImportacionCarpetasRepository
    Public Calls As Integer = 0
    Public ReturnDifferentIds As Boolean = False
    Public Function RegistrarResultadoDocumento(comando As ComandoAlmacenamientoCarpetas, resultado As ResultadoEfectoImportacion) As ResultadoEfectoImportacion
        Calls += 1
        If ReturnDifferentIds Then Return New ResultadoEfectoImportacion With {.DocumentoId = "999"}
        Return resultado
    End Function
End Class
Public Module Doc117CandidateTests
    Private Sub Exigir(valor As Boolean)
        If Not valor Then Throw New Exception("ASSERTION_FAILED")
    End Sub
    Private Sub Rechazo(accion As Action, codigo As String)
        Dim rechazado As Boolean = False
        Try
            accion()
        Catch ex As Exception
            rechazado = ex.Message = codigo
        End Try
        Exigir(rechazado)
    End Sub
    Public Sub Main(args As String())
        Dim repo As New MySqlImportacionCarpetasRepository(), comando As New ComandoAlmacenamientoCarpetas(), resultado As New ResultadoEfectoImportacion()
        If args.Length = 1 AndAlso args(0) = "dormant" Then
            Rechazo(Sub() Doc117InternalDocumentCandidate.ExigirAlcance(Nothing), "CAPACIDAD_NO_DISPONIBLE")
            Rechazo(Sub() Doc117InternalDocumentCandidate.Registrar(comando, resultado, AddressOf repo.RegistrarResultadoDocumento), "CAPACIDAD_NO_DISPONIBLE")
            Exigir(repo.Calls = 0 AndAlso Not Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data")))
            Console.WriteLine("DOC117_DORMANT_REFUSES_BEFORE_EFFECT: PASS")
            Return
        End If
        Doc117InternalDocumentCandidate.ExigirAlcance(comando)
        Dim cambios As Action(Of ComandoAlmacenamientoCarpetas)() = {
            Sub(c) c.Contexto.UsuarioRef = "142", Sub(c) c.Contexto.AmbitoRef = "3", Sub(c) c.Contexto.NivelRef = "215",
            Sub(c) c.Snapshot.VersionId = "39", Sub(c) c.Snapshot.Configuracion.OrigenGabinete = "RA", Sub(c) c.Snapshot.Configuracion.GabineteId = 11,
            Sub(c) c.Raiz.LoteId = "18", Sub(c) c.Raiz.RaizId = "30", Sub(c) c.Archivo.ArchivoId = "999",
            Sub(c) c.Intento.IntentoId = "41", Sub(c) c.Intento.ArchivoId = "999", Sub(c) c.Intento.RaizId = "999",
            Sub(c) c.Intento.LoteId = "999", Sub(c) c.Intento.Fase = "CREAR_EXPEDIENTE", Sub(c) c.EsVerificacion = True}
        For Each cambio In cambios
            Dim ajeno As New ComandoAlmacenamientoCarpetas()
            cambio(ajeno)
            Rechazo(Sub() Doc117InternalDocumentCandidate.Iniciar(ajeno, Function() True), "DOC117_ALM02_SCOPE_INVALID")
        Next
        Exigir(repo.Calls = 0)
        Dim diario = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "DOC117-ALM02-" & New String("e"c, 32)), pin = Path.Combine(diario, "intento-archivo-902.json")
        Rechazo(Sub() Doc117InternalDocumentCandidate.Registrar(comando, resultado, AddressOf repo.RegistrarResultadoDocumento), "DOC117_ALM02_PIN_NOT_VERIFIED")
        Directory.CreateDirectory(diario)
        File.WriteAllText(pin, "{")
        Rechazo(Sub() Doc117InternalDocumentCandidate.Iniciar(comando, Function() True), "DOC117_ALM02_PIN_NOT_VERIFIED")
        File.Delete(pin) ' Only this disposable test's malformed fixture, never a live claim.
        Directory.CreateDirectory(pin)
        Rechazo(Sub() Doc117InternalDocumentCandidate.Iniciar(comando, Function() True), "DOC117_ALM02_PIN_NOT_CREATED")
        Directory.Delete(pin) ' Empty directory created above by this test.
        Dim starts As Integer = 0, denials As Integer = 0
        Dim start As Func(Of Boolean) = Function()
                                            Dim marca = Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText(pin))
                                            Exigir(CStr(marca("intentoId")) = "903" AndAlso CInt(marca("version")) = 1 AndAlso CStr(marca("solicitudHash")) = New String("a"c, 64))
                                            Interlocked.Increment(starts)
                                            Return True
                                        End Function
        Dim contend As Action = Sub()
                                    Try
                                        Doc117InternalDocumentCandidate.Iniciar(comando, start)
                                    Catch ex As InvalidOperationException
                                        Exigir(ex.Message = "DOC117_ALM02_PIN_NOT_CREATED" OrElse ex.Message = "DOC117_ALM02_PIN_NOT_VERIFIED")
                                        Interlocked.Increment(denials)
                                    End Try
                                End Sub
        Task.WaitAll(Task.Run(contend), Task.Run(contend))
        Exigir(starts = 1 AndAlso denials = 1)
        Doc117InternalDocumentCandidate.ExigirAlcance(comando, True)
        Dim pinOriginal = File.ReadAllBytes(pin)
        comando.Intento.IntentoId = "907"
        Rechazo(Sub() Doc117InternalDocumentCandidate.ExigirAlcance(comando, True), "DOC117_ALM02_PIN_NOT_VERIFIED")
        comando.Intento.IntentoId = "903"
        File.Delete(pin)
        Rechazo(Sub() Doc117InternalDocumentCandidate.ExigirAlcance(comando, True), "DOC117_ALM02_PIN_NOT_VERIFIED")
        File.WriteAllBytes(pin, pinOriginal) ' Restore this test-only fixture to test the next boundary.
        Dim incompleto As New ResultadoEfectoImportacion With {.DocumentoId = Nothing}
        Rechazo(Sub() Doc117InternalDocumentCandidate.Registrar(comando, incompleto, AddressOf repo.RegistrarResultadoDocumento), "DOC117_ALM02_RESULT_NOT_CONFIRMED")
        Dim writers As Integer = 0
        Rechazo(Sub() Doc117InternalDocumentCandidate.Registrar(comando, resultado, AddressOf repo.RegistrarResultadoDocumento), "DOC117_ALM02_WRITER_NOT_VERIFIED")
        Doc117InternalDocumentCandidate.Invocar(comando, Sub() writers += 1)
        Rechazo(Sub() Doc117InternalDocumentCandidate.Invocar(comando, Sub() writers += 1), "DOC117_ALM02_WRITER_ALREADY_OBSERVED")
        Exigir(writers = 1)
        Rechazo(Sub() Doc117InternalDocumentCandidate.Registrar(comando, resultado, AddressOf repo.RegistrarResultadoDocumento), "DOC117_ALM02_INTERNAL_FAILURE")
        Exigir(repo.Calls = 1 AndAlso resultado.DocumentoId = "910" AndAlso resultado.InventarioId = "911")
        Dim marcaA = Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText(Path.Combine(diario, "archivo-902.json")))
        Exigir(CStr(marcaA("modo")) = "AFTER_CHECKPOINT_COMMIT_BEFORE_RETURN" AndAlso CStr(marcaA("documentoId")) = "910" AndAlso CStr(marcaA("inventarioId")) = "911")
        Rechazo(Sub() Doc117InternalDocumentCandidate.Iniciar(comando, start), "DOC117_ALM02_ALREADY_OBSERVED")
        Exigir(repo.Calls = 1 AndAlso starts = 1)
        comando.Raiz.RaizId = "904" : comando.Archivo.RaizId = "904" : comando.Archivo.ArchivoId = "905"
        comando.Intento.RaizId = "904" : comando.Intento.ArchivoId = "905" : comando.Intento.IntentoId = "906"
        comando.Intento.SolicitudHash = New String("b"c, 64) : comando.Archivo.NombreOriginal = "test-b.pdf"
        comando.Raiz.IdentidadExpediente.ExpedienteId = "912" : comando.Raiz.IdentidadExpediente.RelacionNivelId = "913"
        Exigir(Doc117InternalDocumentCandidate.Iniciar(comando, Function() True))
        Doc117InternalDocumentCandidate.Invocar(comando, Sub() writers += 1)
        Rechazo(Sub() Doc117InternalDocumentCandidate.Registrar(comando, resultado, AddressOf repo.RegistrarResultadoDocumento), "DOC117_ALM02_INTERNAL_FAILURE")
        Exigir(repo.Calls = 1 AndAlso resultado.DocumentoId = "910" AndAlso resultado.InventarioId = "911")
        Dim marcaB = Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText(Path.Combine(diario, "archivo-905.json")))
        Exigir(CStr(marcaB("modo")) = "AFTER_VERIFICATION_BEFORE_CHECKPOINT" AndAlso CStr(marcaB("intentoId")) = "906")
        Rechazo(Sub() Doc117InternalDocumentCandidate.Iniciar(comando, Function() True), "DOC117_ALM02_ALREADY_OBSERVED")
        Exigir(repo.Calls = 1 AndAlso writers = 2 AndAlso Directory.GetFiles(diario).Length = 6)
        Console.WriteLine("DOC117_INTERNAL_WINDOWS_AND_SCOPE_WITH_DOUBLES: PASS")
    End Sub
End Module
