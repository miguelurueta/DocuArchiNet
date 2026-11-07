Option Strict On
Option Compare Binary
Imports System
Imports System.IO
Imports System.Globalization
Imports System.Linq
Imports System.Security.Cryptography
Imports System.Web

' LAB ONLY. Constants are rendered from a reviewed, sealed setup scope.
Friend NotInheritable Class Doc117Alm03ClosedGuard
    Private Const Site As String = "NO_SITE"
    Private Const Run As String = "NO_RUN"
    Private Const Lot As String = "NO_LOT"
    Private Const Snapshot As String = "NO_SNAPSHOT"
    Private Const Cabinet As String = "NO_CABINET"
    Private Const RootA As String = "NO_ROOT_A"
    Private Const FileA As String = "NO_FILE_A"
    Private Const ExpA As String = "NO_EXP_A"
    Private Const RelA As String = "NO_REL_A"
    Private Const NameA As String = "NO_NAME_A"
    Private Const ContentA As String = "NO_CONTENT_A"
    Private Const RequestA As String = "NO_REQUEST_A"
    Private Const DigestA As String = "NO_DIGEST_A"
    Private Const RootB As String = "NO_ROOT_B"
    Private Const FileB As String = "NO_FILE_B"
    Private Const ExpB As String = "NO_EXP_B"
    Private Const RelB As String = "NO_REL_B"
    Private Const NameB As String = "NO_NAME_B"
    Private Const ContentB As String = "NO_CONTENT_B"
    Private Const RequestB As String = "NO_REQUEST_B"
    Private Const DigestB As String = "NO_DIGEST_B"
    Private Const IndividualName As String = "NO_INDIVIDUAL_NAME"
    Private Const IndividualContent As String = "NO_INDIVIDUAL_CONTENT"
    Private Const IndividualDigest As String = "NO_INDIVIDUAL_DIGEST"
    Private Shared ReadOnly Engine As New Lazy(Of Doc117Alm03ObservationCore)(Function() New Doc117Alm03ObservationCore(Path.Combine(Site, "App_Data", "DOC117-ALM03-" & Run), Run, New String() {DigestA, DigestB, IndividualDigest}))
    <ThreadStatic()> Private Shared Current As Caller

    Private Shared Function Failure(code As String) As InvalidOperationException
        Return New InvalidOperationException("DOC117_ALM03_GUARD_" & code)
    End Function
    Private Shared Function Context(individual As Boolean) As HttpContext
        If Site = "NO_SITE" OrElse Not String.Equals(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory).TrimEnd(Path.DirectorySeparatorChar), Site.TrimEnd(Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase) Then Throw Failure("SITE_CHANGED")
        Dim h = HttpContext.Current
        If h Is Nothing OrElse h.Session Is Nothing OrElse h.Request Is Nothing OrElse h.Request.Url.Scheme <> "https" OrElse h.Request.Url.Host <> "localhost" OrElse h.Request.Url.Port <> 443 OrElse Convert.ToString(h.Session("GA_IDUSUARIOGESTION"), CultureInfo.InvariantCulture) <> "141" OrElse Convert.ToString(h.Session("GA_IDEMPRESA"), CultureInfo.InvariantCulture) <> "2" Then Throw Failure("CONTEXT_CHANGED")
        Dim expected = If(individual, "/GestionDocumental-Docuarchi.net/generic_control/FileUploadHandler_.ashx", "/GestionDocumental-Docuarchi.net/webservice/WebServiceImportacionCarpetas.asmx/AvanzarLote")
        If Not String.Equals(h.Request.Url.AbsolutePath, expected, StringComparison.OrdinalIgnoreCase) Then Throw Failure("CALLER_CHANGED")
        If individual Then
            Dim parts = Convert.ToString(h.Session("PG_SELECCION_TREVIEEW_PRODUCCION"), CultureInfo.InvariantCulture).Split("|"c)
            If parts.Length < 3 OrElse parts(0) <> "1072" OrElse parts(1) <> "8" OrElse parts(2) <> "467" OrElse Convert.ToString(h.Session("PG_SELECCION_ID_EXPEIDENTE"), CultureInfo.InvariantCulture) <> "467" OrElse Convert.ToString(h.Session("WF_TIPO_ADJUNTA"), CultureInfo.InvariantCulture) <> "PRODUCCION" Then Throw Failure("INDIVIDUAL_SELECTION_CHANGED")
        End If
        Return h
    End Function
    Private Shared Sub CheckFile(filePath As String, expectedName As String, content As String, individual As Boolean)
        Try
            Dim full = Path.GetFullPath(filePath), base = Path.Combine(Site, If(individual, "Temp_Image\upload_file\141", "App_Data\ImportacionCarpetas"))
            If Not full.StartsWith(base.TrimEnd(Path.DirectorySeparatorChar) & Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) OrElse Path.GetFileName(full) <> expectedName Then Throw Failure("FILE_CHANGED")
            Dim cursor = full
            Do
                If (File.GetAttributes(cursor) And FileAttributes.ReparsePoint) <> 0 Then Throw Failure("FILE_CHANGED")
                If String.Equals(cursor, Site.TrimEnd(Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase) Then Exit Do
                cursor = Path.GetDirectoryName(cursor)
            Loop
            Using input As New FileStream(full, FileMode.Open, FileAccess.Read, FileShare.Read), digest = SHA256.Create()
                If input.Length <> 677 OrElse BitConverter.ToString(digest.ComputeHash(input)).Replace("-", "").ToLowerInvariant() <> content Then Throw Failure("FILE_CHANGED")
            End Using
        Catch
            Throw Failure("FILE_CHANGED")
        End Try
    End Sub
    Friend Shared Function PrepararImportacion(command As ComandoAlmacenamientoCarpetas, data As DatosAlmacenamientoImportacion, copy As String) As IDisposable
        Dim h = Context(False)
        If command Is Nothing OrElse data Is Nothing OrElse command.EsVerificacion OrElse command.Contexto Is Nothing OrElse command.Snapshot Is Nothing OrElse command.Snapshot.Configuracion Is Nothing OrElse command.Raiz Is Nothing OrElse command.Archivo Is Nothing OrElse command.Intento Is Nothing OrElse command.Raiz.IdentidadExpediente Is Nothing Then Throw Failure("COMMAND_CHANGED")
        Dim a = command.Archivo, r = command.Raiz, i = command.Intento, c = command.Contexto, s = command.Snapshot
        Dim first = a.ArchivoId = FileA AndAlso r.RaizId = RootA, second = a.ArchivoId = FileB AndAlso r.RaizId = RootB
        If Not (first OrElse second) OrElse r.LoteId <> Lot OrElse a.RaizId <> r.RaizId OrElse i.LoteId <> Lot OrElse i.RaizId <> r.RaizId OrElse i.ArchivoId <> a.ArchivoId OrElse i.Fase <> "ALMACENAR" OrElse Not ValidadorPerfilImportacion.ReferenciaValida(i.IntentoId) OrElse i.Estado <> "RESERVADO" OrElse i.FinUtc IsNot Nothing OrElse i.Resultado IsNot Nothing Then Throw Failure("COMMAND_CHANGED")
        If c.UsuarioRef <> "141" OrElse c.AmbitoRef <> "2" OrElse c.NivelRef <> "7" OrElse s.VersionId <> "38" OrElse s.Huella <> Snapshot OrElse s.Configuracion.OrigenGabinete <> "DA" OrElse s.Configuracion.GabineteId <> 10 OrElse command.TipologiaId <> "79" Then Throw Failure("DESTINATION_CHANGED")
        If r.IdentidadExpediente.OrigenLogico <> "RA" OrElse r.IdentidadExpediente.NivelRef <> "7" OrElse r.IdentidadExpediente.ExpedienteId <> If(first, ExpA, ExpB) OrElse r.IdentidadExpediente.RelacionNivelId <> If(first, RelA, RelB) OrElse a.NombreOriginal <> If(first, NameA, NameB) OrElse a.BytesPrevistos <> "677" OrElse a.BytesRecibidos <> "677" OrElse a.ContenidoHash <> If(first, ContentA, ContentB) OrElse i.SolicitudHash <> If(first, RequestA, RequestB) Then Throw Failure("RESOURCE_CHANGED")
        command.ValidarDatos(data)
        CheckFile(copy, a.NombreOriginal, a.ContenidoHash, False)
        Return Prepare(h, If(first, "IMPORT_A", "IMPORT_B"), data.GabineteNombre, copy, a.NombreOriginal, Integer.Parse(r.IdentidadExpediente.ExpedienteId, CultureInfo.InvariantCulture), 79, 0, data.ClaseDocumentoNombre, i.IntentoId, i.SolicitudHash)
    End Function
    Friend Shared Function PrepararIndividual(gabinete As String, ruta As String, nombre As String, expediente As Integer, tipologia As Integer, tipo As Integer, clase As String) As IDisposable
        Dim h = Context(True)
        If expediente <> 467 OrElse tipologia <> 50 OrElse tipo <> 2 OrElse nombre <> "Comprobante de Egreso" OrElse clase <> "DOCUMENTO ELECTRONICO" OrElse Not String.Equals(Path.GetFullPath(Convert.ToString(h.Session("WF_RUTA_TEMPO_ADJUNTA"), CultureInfo.InvariantCulture)), Path.GetFullPath(ruta), StringComparison.OrdinalIgnoreCase) Then Throw Failure("INDIVIDUAL_ARGUMENTS_CHANGED")
        CheckFile(ruta, IndividualName, IndividualContent, True)
        Return Prepare(h, "INDIVIDUAL", gabinete, ruta, nombre, expediente, tipologia, tipo, clase, Nothing, Nothing)
    End Function
    Private Shared Function Prepare(h As HttpContext, key As String, gabinete As String, ruta As String, nombre As String, expediente As Integer, tipologia As Integer, tipo As Integer, clase As String, intento As String, request As String) As IDisposable
        If Current IsNot Nothing OrElse gabinete <> Cabinet OrElse String.IsNullOrWhiteSpace(nombre) OrElse String.IsNullOrWhiteSpace(clase) OrElse String.IsNullOrEmpty(h.Session.SessionID) Then Throw Failure("CALLER_SCOPE_CHANGED")
        Dim engineValue = Engine.Value
        If intento IsNot Nothing Then
            Try
                engineValue.Write("attempt-" & key & ".json", New With {.version = 1, .runId = Run, .resourceKey = key, .intentoId = intento, .solicitudHash = request})
            Catch
                Throw Failure("ATTEMPT_ALREADY_PINNED")
            End Try
        End If
        Dim value As New Caller(h, key, engineValue.Preparar(key, h.Session.SessionID), gabinete, Path.GetFullPath(ruta), nombre, expediente, tipologia, tipo, clase)
        Current = value : Return value
    End Function
    Friend Shared Function Entrar(gabinete As String, ruta As String, nombre As String, expediente As Integer, tipologia As Integer, tipo As Integer, clase As String) As Doc117Alm03Ticket
        Dim value = Current
        If value Is Nothing Then Throw Failure("UNBOUND_WRITER")
        Dim h = Context(value.Key = "INDIVIDUAL")
        If Not Object.ReferenceEquals(h, value.Http) OrElse h.Session.SessionID <> value.SessionIdentity OrElse gabinete <> value.CabinetName OrElse Path.GetFullPath(ruta) <> value.FilePath OrElse nombre <> value.Name OrElse expediente <> value.Expedient OrElse tipologia <> value.Typology OrElse tipo <> value.StorageType OrElse clase <> value.DocumentClass Then Throw Failure("WRITER_ARGUMENTS_CHANGED")
        CheckFile(ruta, If(value.Key = "INDIVIDUAL", IndividualName, If(value.Key = "IMPORT_A", NameA, NameB)), If(value.Key = "INDIVIDUAL", IndividualContent, If(value.Key = "IMPORT_A", ContentA, ContentB)), value.Key = "INDIVIDUAL")
        Return value.Ticket.Iniciar()
    End Function
    Private NotInheritable Class Caller
        Implements IDisposable
        Friend ReadOnly Http As HttpContext, SessionIdentity As String, Key As String, Ticket As Doc117Alm03Ticket
        Friend ReadOnly CabinetName As String, FilePath As String, Name As String, DocumentClass As String
        Friend ReadOnly Expedient As Integer, Typology As Integer, StorageType As Integer
        Friend Sub New(h As HttpContext, k As String, t As Doc117Alm03Ticket, g As String, f As String, n As String, e As Integer, typ As Integer, st As Integer, cl As String)
            Http = h : SessionIdentity = h.Session.SessionID : Key = k : Ticket = t : CabinetName = g : FilePath = f : Name = n : Expedient = e : Typology = typ : StorageType = st : DocumentClass = cl
        End Sub
        Public Sub Dispose() Implements IDisposable.Dispose
            If Object.ReferenceEquals(Current, Me) Then Current = Nothing
        End Sub
    End Class
End Class
