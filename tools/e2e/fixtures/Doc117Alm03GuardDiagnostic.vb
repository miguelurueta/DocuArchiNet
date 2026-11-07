Option Strict On
Option Compare Binary
Imports System
Imports System.IO
Imports System.Web
Imports System.Globalization
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Collections.Generic

' Laboratory only. The writer is never called, including when every guard accepts.
Friend NotInheritable Class Doc117Alm03GuardDiagnostic
    Private Const Site As String = "NO_SITE"
    Private Const Run As String = "NO_RUN"
    Private Const ScopeHash As String = "NO_SCOPE"
    Private Const Cabinet As String = "NO_CABINET"
    Private Const FileName As String = "NO_FILE"
    Private Shared Function Text(h As HttpContext, key As String) As String
        Return Convert.ToString(h.Session(key), CultureInfo.InvariantCulture)
    End Function
    Private Shared Function SamePath(a As String, b As String) As Boolean
        Try
            Return String.Equals(Path.GetFullPath(a).TrimEnd(Path.DirectorySeparatorChar), Path.GetFullPath(b).TrimEnd(Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase)
        Catch
            Return False
        End Try
    End Function
    Private Shared Function Equivalent(a As String, b As String) As Boolean
        Try
            Return String.Equals(Regex.Replace(a.Normalize(NormalizationForm.FormKC), "\s+", " ").Trim(), b, StringComparison.OrdinalIgnoreCase)
        Catch
            Return False
        End Try
    End Function
    Private Shared Function Code(ex As Exception) As String
        Select Case ex.Message
            Case "DOC117_ALM03_GUARD_SITE_CHANGED", "DOC117_ALM03_GUARD_CONTEXT_CHANGED", "DOC117_ALM03_GUARD_CALLER_CHANGED", "DOC117_ALM03_GUARD_INDIVIDUAL_SELECTION_CHANGED", "DOC117_ALM03_GUARD_FILE_CHANGED", "DOC117_ALM03_GUARD_INDIVIDUAL_ARGUMENTS_CHANGED", "DOC117_ALM03_GUARD_CALLER_SCOPE_CHANGED", "DOC117_ALM03_GUARD_UNBOUND_WRITER", "DOC117_ALM03_GUARD_WRITER_ARGUMENTS_CHANGED", "DOC117_ALM03_SCOPE_INVALID", "DOC117_ALM03_PATH_INVALID", "DOC117_ALM03_FRESH_JOURNAL_REQUIRED", "DOC117_ALM03_RESOURCE_INVALID", "DOC117_ALM03_RESOURCE_ALREADY_PREPARED", "DOC117_ALM03_SESSIONS_NOT_DISTINCT", "DOC117_ALM03_SESSION_CHANGED"
                Return ex.Message
            Case Else
                Return "UNCLASSIFIED_FAILURE"
        End Select
    End Function
    Private Shared Sub NoLinks(target As String)
        Dim current = target, root = Site.TrimEnd(Path.DirectorySeparatorChar)
        Do
            If (Directory.Exists(current) OrElse File.Exists(current)) AndAlso (File.GetAttributes(current) And FileAttributes.ReparsePoint) <> 0 Then Throw New IOException()
            If String.Equals(current, root, StringComparison.OrdinalIgnoreCase) Then Exit Do
            current = Path.GetDirectoryName(current)
            If current Is Nothing OrElse (Not current.StartsWith(root & Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) AndAlso Not String.Equals(current, root, StringComparison.OrdinalIgnoreCase)) Then Throw New IOException()
        Loop
    End Sub
    Friend Shared Sub MarcarPreparacion()
        Try
            Dim h = HttpContext.Current
            If Site = "NO_SITE" OrElse Not SamePath(AppDomain.CurrentDomain.BaseDirectory, Site) OrElse h Is Nothing OrElse h.Session Is Nothing OrElse Not Regex.IsMatch(ScopeHash, "\A[a-f0-9]{64}\z") Then Return
            If Text(h, "GA_IDUSUARIOGESTION") <> "141" OrElse Text(h, "GA_IDEMPRESA") <> "2" OrElse h.Request.Url.Scheme <> "https" OrElse h.Request.Url.Host <> "localhost" OrElse h.Request.Url.Port <> 443 Then Return
            If Not String.Equals(h.Request.Url.AbsolutePath, "/GestionDocumental-Docuarchi.net/webservice/WebServiceProducion.asmx/ServiceSolicitaCargarDocumentoExpediente", StringComparison.OrdinalIgnoreCase) Then Return
            h.Response.AppendHeader("X-Doc117-Guard-Diag", ScopeHash)
        Catch
            ' Without the header the client refuses the upload.
        End Try
    End Sub
    Friend Shared Function Evaluar(gabinete As String, ruta As String, nombre As String, expediente As Integer, tipologia As Integer, tipo As Integer, clase As String) As String
        Dim h = HttpContext.Current
        If Site = "NO_SITE" OrElse Not Regex.IsMatch(Run, "\A[a-f0-9]{32}\z") OrElse Not Regex.IsMatch(ScopeHash, "\A[a-f0-9]{64}\z") OrElse Not SamePath(AppDomain.CurrentDomain.BaseDirectory, Site) OrElse h Is Nothing OrElse h.Session Is Nothing OrElse h.Request Is Nothing Then Return "DOC117_ALM03_DIAGNOSTIC_CONTEXT_REJECTED"
        If Text(h, "GA_IDUSUARIOGESTION") <> "141" OrElse Text(h, "GA_IDEMPRESA") <> "2" OrElse h.Request.Url.Scheme <> "https" OrElse h.Request.Url.Host <> "localhost" OrElse h.Request.Url.Port <> 443 OrElse Not String.Equals(h.Request.Url.AbsolutePath, "/GestionDocumental-Docuarchi.net/generic_control/FileUploadHandler_.ashx", StringComparison.OrdinalIgnoreCase) Then Return "DOC117_ALM03_DIAGNOSTIC_CONTEXT_REJECTED"
        Try
            Dim parts = Text(h, "PG_SELECCION_TREVIEEW_PRODUCCION").Split("|"c)
            Dim checks As New Dictionary(Of String, Boolean) From {
                {"site", True}, {"actor", True}, {"company", True}, {"caller", True},
                {"relation", parts.Length >= 3 AndAlso parts(0) = "1072"}, {"level", parts.Length >= 3 AndAlso parts(1) = "8"},
                {"selectedExpedient", parts.Length >= 3 AndAlso parts(2) = "467" AndAlso Text(h, "PG_SELECCION_ID_EXPEIDENTE") = "467"},
                {"workflow", Text(h, "WF_TIPO_ADJUNTA") = "PRODUCCION"}, {"cabinet", gabinete = Cabinet},
                {"expedient", expediente = 467}, {"typology", tipologia = 50}, {"storageType", tipo = 2},
                {"descriptionExact", nombre = "Comprobante de Egreso"}, {"descriptionEquivalent", Equivalent(nombre, "Comprobante de Egreso")},
                {"documentClass", clase = "DOCUMENTO ELECTRONICO"}, {"sessionTempPath", SamePath(ruta, Text(h, "WF_RUTA_TEMPO_ADJUNTA"))},
                {"fileName", Path.GetFileName(ruta) = FileName}}
            Dim result = "GUARDS_ACCEPTED", failureCode = "GUARDS_ACCEPTED"
            Try
                Using caller = Doc117Alm03ClosedGuard.PrepararIndividual(gabinete, ruta, nombre, expediente, tipologia, tipo, clase)
                    ' Deliberate terminal point. Do not enter the document writer.
                End Using
            Catch ex As Exception
                result = "GUARDS_REJECTED" : failureCode = Code(ex)
            End Try
            Dim directory = Path.Combine(Site, "App_Data", "DOC117-ALM03-GUARD-DIAG-" & Run), target = Path.Combine(directory, "probe.json")
            NoLinks(target)
            System.IO.Directory.CreateDirectory(directory)
            NoLinks(target)
            Dim data = Newtonsoft.Json.JsonConvert.SerializeObject(New With {.version = 1, .protocol = "ALM03_INDIVIDUAL_GUARD_DIAGNOSTIC_NO_WRITER_V1", .runId = Run, .scopeSha256 = ScopeHash, .outcome = result, .code = failureCode, .writerInvoked = False, .checks = checks})
            Using output As New FileStream(target, FileMode.CreateNew, FileAccess.Write, FileShare.Read)
                Dim bytes = Encoding.UTF8.GetBytes(data)
                output.Write(bytes, 0, bytes.Length) : output.Flush(True)
            End Using
            Return "DOC117_ALM03_DIAGNOSTIC_WRITER_BLOCKED"
        Catch
            Return "DOC117_ALM03_DIAGNOSTIC_EVIDENCE_FAILED"
        End Try
    End Function
End Class
