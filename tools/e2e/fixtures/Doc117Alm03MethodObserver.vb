Option Strict On
Option Compare Binary
Imports System
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading

' LAB ONLY. There is intentionally no live-scope factory, switch or endpoint.
Friend NotInheritable Class Doc117Alm03MethodObserver
    Friend Shared Function Entrar(gabinete As String, ruta As String, nombre As String, expediente As Integer, tipologia As Integer, tipo As Integer, clase As String) As Doc117Alm03Ticket
        Throw New InvalidOperationException("DOC117_ALM03_DORMANT_SCOPE")
    End Function
End Class

' Observation engine tested only with synthetic, prevalidated resources. A live
' binding must separately validate context, scope and actual writer arguments.
Friend NotInheritable Class Doc117Alm03ObservationCore
    Private ReadOnly _journal As String
    Friend ReadOnly RunId As String
    Friend ReadOnly ClockId As String = Guid.NewGuid().ToString("N")
    Friend ReadOnly Pid As String = Process.GetCurrentProcess().Id.ToString(CultureInfo.InvariantCulture)
    Friend ReadOnly DomainId As String = AppDomain.CurrentDomain.Id.ToString(CultureInfo.InvariantCulture)
    Private ReadOnly _hashes As String()
    Private ReadOnly _tickets As New Dictionary(Of String, Doc117Alm03Ticket)(StringComparer.Ordinal)
    Private ReadOnly _gate As New Object()
    Private _importSession As String
    Private _individualSession As String
    Private _faults As Integer
    Private _sealed As Integer
    Friend Shared ReadOnly Keys As String() = {"IMPORT_A", "IMPORT_B", "INDIVIDUAL"}

    Friend Sub New(journal As String, run As String, hashes As String())
        If Not Regex.IsMatch(run, "\A[a-f0-9]{32}\z") OrElse hashes Is Nothing OrElse hashes.Length <> 3 OrElse hashes.Any(Function(h) h Is Nothing OrElse Not Regex.IsMatch(h, "\A[a-f0-9]{64}\z")) OrElse hashes.Distinct().Count() <> 3 OrElse Not Stopwatch.IsHighResolution Then Throw Fallo("SCOPE_INVALID")
        Dim base = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory).TrimEnd(Path.DirectorySeparatorChar)
        _journal = Path.GetFullPath(journal)
        If Not _journal.StartsWith(base & Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) Then Throw Fallo("PATH_INVALID")
        Dim current = _journal
        Do
            If (Directory.Exists(current) OrElse File.Exists(current)) AndAlso (File.GetAttributes(current) And FileAttributes.ReparsePoint) <> 0 Then Throw Fallo("PATH_INVALID")
            If String.Equals(current, base, StringComparison.OrdinalIgnoreCase) Then Exit Do
            current = Path.GetDirectoryName(current)
        Loop
        If Directory.Exists(_journal) OrElse File.Exists(_journal) Then Throw Fallo("FRESH_JOURNAL_REQUIRED")
        Directory.CreateDirectory(_journal)
        RunId = run : _hashes = CType(hashes.Clone(), String())
    End Sub
    Friend Shared Function Fallo(code As String) As InvalidOperationException
        Return New InvalidOperationException("DOC117_ALM03_" & code)
    End Function
    Friend Sub Fault()
        Interlocked.Increment(_faults)
    End Sub
    Friend Function Preparar(key As String, sessionIdentity As String) As Doc117Alm03Ticket
        Dim index = Array.IndexOf(Keys, key)
        If index < 0 OrElse String.IsNullOrEmpty(sessionIdentity) OrElse sessionIdentity.Length > 256 Then Throw Fallo("RESOURCE_INVALID")
        SyncLock _gate
            If _sealed <> 0 OrElse _tickets.ContainsKey(key) Then Throw Fallo("RESOURCE_ALREADY_PREPARED")
            If key = "INDIVIDUAL" Then
                If _importSession IsNot Nothing AndAlso _importSession = sessionIdentity Then Throw Fallo("SESSIONS_NOT_DISTINCT")
                If _individualSession IsNot Nothing AndAlso _individualSession <> sessionIdentity Then Throw Fallo("SESSION_CHANGED")
                _individualSession = sessionIdentity
            Else
                If _individualSession IsNot Nothing AndAlso _individualSession = sessionIdentity Then Throw Fallo("SESSIONS_NOT_DISTINCT")
                If _importSession IsNot Nothing AndAlso _importSession <> sessionIdentity Then Throw Fallo("SESSION_CHANGED")
                _importSession = sessionIdentity
            End If
            Dim ticket As New Doc117Alm03Ticket(Me, key, _hashes(index))
            Write("reserved-" & key & ".json", New With {.version = 1, .runId = RunId, .resourceKey = key, .resourceSha256 = _hashes(index)})
            _tickets.Add(key, ticket)
            Return ticket
        End SyncLock
    End Function
    Friend Sub Write(name As String, data As Object)
        If (File.GetAttributes(_journal) And FileAttributes.ReparsePoint) <> 0 Then Throw Fallo("PATH_INVALID")
        Dim bytes = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(data))
        Using output As New FileStream(Path.Combine(_journal, name), FileMode.CreateNew, FileAccess.Write, FileShare.None)
            output.Write(bytes, 0, bytes.Length) : output.Flush(True)
        End Using
    End Sub
    Friend Sub ComprobarReserva(key As String, hash As String)
        Try
            Dim reservationPath = Path.Combine(_journal, "reserved-" & key & ".json")
            If (File.GetAttributes(_journal) And FileAttributes.ReparsePoint) <> 0 OrElse (File.GetAttributes(reservationPath) And FileAttributes.ReparsePoint) <> 0 Then Throw Fallo("RESERVATION_CHANGED")
            Dim expected = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(New With {.version = 1, .runId = RunId, .resourceKey = key, .resourceSha256 = hash}))
            If Not System.IO.File.ReadAllBytes(reservationPath).SequenceEqual(expected) Then Throw Fallo("RESERVATION_CHANGED")
        Catch
            Throw Fallo("RESERVATION_CHANGED")
        End Try
    End Sub
    Friend Sub TrySeal()
        'Only finalization takes this short lock; no lock spans the writer body.
        SyncLock _gate
            If _tickets.Count <> 3 OrElse _tickets.Values.Any(Function(t) Not t.Ended) Then Return
            If Interlocked.CompareExchange(_sealed, 1, 0) <> 0 Then Return
            Try
                Write("summary.json", New With {.version = 1, .runId = RunId, .clockDomainId = ClockId, .processId = Pid, .appDomainId = DomainId, .frequencyHz = Stopwatch.Frequency.ToString(CultureInfo.InvariantCulture), .expectedResources = Keys, .resourceHashes = _hashes, .sessionsDistinct = _importSession IsNot Nothing AndAlso _individualSession IsNot Nothing AndAlso _importSession <> _individualSession, .entries = Enumerable.Count(_tickets.Values, Function(t) t.Started), .exits = Enumerable.Count(_tickets.Values, Function(t) t.Ended), .observationFaults = Volatile.Read(_faults), .complete = Volatile.Read(_faults) = 0})
            Catch
                Fault()
            End Try
        End SyncLock
    End Sub
End Class

Friend NotInheritable Class Doc117Alm03Ticket
    Private ReadOnly _core As Doc117Alm03ObservationCore
    Private ReadOnly _key As String
    Private ReadOnly _hash As String
    Private _started As Integer
    Private _exitStarted As Integer
    Private _ended As Integer
    Private _managed As String
    Private _native As String
    <DllImport("kernel32.dll")>
    Private Shared Function GetCurrentThreadId() As UInteger
    End Function
    Friend Sub New(core As Doc117Alm03ObservationCore, key As String, hash As String)
        _core = core : _key = key : _hash = hash
    End Sub
    Friend ReadOnly Property Started As Boolean
        Get
            Return Volatile.Read(_started) = 1
        End Get
    End Property
    Friend ReadOnly Property Ended As Boolean
        Get
            Return Volatile.Read(_ended) = 1
        End Get
    End Property
    Private Function Data(phase As String, ticks As Long, status As String, documento As Integer, inventario As Integer) As Object
        Return New With {.version = 1, .runId = _core.RunId, .resourceKey = _key, .resourceSha256 = _hash, .boundary = "METHOD_BODY_ENTRY_EXIT_V1", .phase = phase, .clockDomainId = _core.ClockId, .frequencyHz = Stopwatch.Frequency.ToString(CultureInfo.InvariantCulture), .highResolution = Stopwatch.IsHighResolution, .processId = _core.Pid, .appDomainId = _core.DomainId, .threadId = _managed, .nativeThreadId = _native, .ticks = ticks.ToString(CultureInfo.InvariantCulture), .sessionSlot = If(_key = "INDIVIDUAL", "INDIVIDUAL_SESSION", "IMPORT_SESSION"), .status = status, .documentoId = If(documento > 0, documento.ToString(CultureInfo.InvariantCulture), Nothing), .inventarioId = If(inventario > 0, inventario.ToString(CultureInfo.InvariantCulture), Nothing)}
    End Function
    Friend Function Iniciar() As Doc117Alm03Ticket
        _core.ComprobarReserva(_key, _hash)
        If Interlocked.CompareExchange(_started, 1, 0) <> 0 Then Throw Doc117Alm03ObservationCore.Fallo("WRITER_ALREADY_ENTERED")
        _managed = Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture)
        _native = GetCurrentThreadId().ToString(CultureInfo.InvariantCulture)
        Dim ticks = Stopwatch.GetTimestamp()
        Try
            _core.Write("entry-" & _key & ".json", Data("ENTER", ticks, "PENDING", 0, 0))
        Catch
            _core.Fault() : Throw Doc117Alm03ObservationCore.Fallo("ENTRY_NOT_DURABLE")
        End Try
        Return Me
    End Function
    Friend Sub Salir(resultado As String, documento As Integer, inventario As Integer)
        Dim ticks = Stopwatch.GetTimestamp()
        'Observation failures must never replace a writer return or exception.
        If Interlocked.CompareExchange(_exitStarted, 1, 0) <> 0 Then
            _core.Fault() : Return
        End If
        Try
            If Not Started OrElse Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture) <> _managed OrElse GetCurrentThreadId().ToString(CultureInfo.InvariantCulture) <> _native Then Throw Doc117Alm03ObservationCore.Fallo("EXIT_NOT_CORRELATED")
            _core.Write("exit-" & _key & ".json", Data("EXIT", ticks, If(resultado = "YES" AndAlso documento > 0 AndAlso inventario > 0, "SUCCESS", "FAILURE"), documento, inventario))
        Catch
            _core.Fault()
        Finally
            Interlocked.Exchange(_ended, 1)
        End Try
        _core.TrySeal()
    End Sub
End Class
