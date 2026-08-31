Imports System
Imports System.Globalization
Imports System.Text
Imports System.Web.Security

'Protege la continuación del listado operativo contra manipulación y reutilización entre contextos.
Public Class NotasWorkflowCursorCodec
    Implements INotasWorkflowCursorCodec

    Private Const OrdenOperativo As String = "FECHA_DESC_ID_DESC"

    Public Function Proteger(ByVal contexto As ContextoModuloWorkflow,
                             ByVal tarea As TareaWorkflow,
                             ByVal fechaCreacionUtc As DateTime,
                             ByVal idNota As Long) As String Implements INotasWorkflowCursorCodec.Proteger
        If Not EsContextoValido(contexto, tarea) OrElse idNota <= 0 Then Return String.Empty

        Dim contenido As String = String.Join(ChrW(31), New String() {
            tarea.IdTarea.ToString(CultureInfo.InvariantCulture),
            tarea.TokenVersion,
            contexto.IdUsuarioWorkflow.ToString(CultureInfo.InvariantCulture),
            contexto.IdGrupoWorkflow.ToString(CultureInfo.InvariantCulture),
            contexto.IdRutaWorkflow.ToString(CultureInfo.InvariantCulture),
            OrdenOperativo,
            NormalizarUtc(fechaCreacionUtc).Ticks.ToString(CultureInfo.InvariantCulture),
            idNota.ToString(CultureInfo.InvariantCulture)})
        Try
            Dim protegido As Byte() = MachineKey.Protect(Encoding.UTF8.GetBytes(contenido), "WorkflowNotasCursor", "v1")
            If protegido Is Nothing OrElse protegido.Length = 0 Then Return String.Empty
            Return Convert.ToBase64String(protegido).TrimEnd("="c).Replace("+", "-").Replace("/", "_")
        Catch
            Return String.Empty
        End Try
    End Function

    Public Function Validar(ByVal contexto As ContextoModuloWorkflow,
                            ByVal tarea As TareaWorkflow,
                            ByVal cursor As String,
                            ByRef fechaCreacionUtc As DateTime,
                            ByRef idNota As Long) As Boolean Implements INotasWorkflowCursorCodec.Validar
        fechaCreacionUtc = DateTime.MinValue
        idNota = 0L
        If Not EsContextoValido(contexto, tarea) OrElse String.IsNullOrWhiteSpace(cursor) Then Return False

        Try
            Dim base64 As String = cursor.Replace("-", "+").Replace("_", "/")
            base64 = base64.PadRight(base64.Length + ((4 - base64.Length Mod 4) Mod 4), "="c)
            Dim contenidoProtegido As Byte() = Convert.FromBase64String(base64)
            Dim contenido As Byte() = MachineKey.Unprotect(contenidoProtegido, "WorkflowNotasCursor", "v1")
            If contenido Is Nothing Then Return False

            Dim partes As String() = Encoding.UTF8.GetString(contenido).Split(New Char() {ChrW(31)})
            If partes.Length <> 8 Then Return False
            Dim idTarea As Long = 0L
            Dim idUsuario As Integer = 0
            Dim idGrupo As Integer = 0
            Dim idRuta As Integer = 0
            Dim ticks As Long = 0L
            If Not Long.TryParse(partes(0), NumberStyles.None, CultureInfo.InvariantCulture, idTarea) OrElse
               Not Integer.TryParse(partes(2), NumberStyles.None, CultureInfo.InvariantCulture, idUsuario) OrElse
               Not Integer.TryParse(partes(3), NumberStyles.None, CultureInfo.InvariantCulture, idGrupo) OrElse
               Not Integer.TryParse(partes(4), NumberStyles.None, CultureInfo.InvariantCulture, idRuta) OrElse
               Not Long.TryParse(partes(6), NumberStyles.None, CultureInfo.InvariantCulture, ticks) OrElse
               Not Long.TryParse(partes(7), NumberStyles.None, CultureInfo.InvariantCulture, idNota) OrElse
               idTarea <> tarea.IdTarea OrElse idUsuario <> contexto.IdUsuarioWorkflow OrElse
               idGrupo <> contexto.IdGrupoWorkflow OrElse idRuta <> contexto.IdRutaWorkflow OrElse idNota <= 0 Then
                idNota = 0L
                Return False
            End If
            fechaCreacionUtc = New DateTime(ticks, DateTimeKind.Utc)
            Return String.Equals(partes(1), tarea.TokenVersion, StringComparison.Ordinal) AndAlso
                   String.Equals(partes(5), OrdenOperativo, StringComparison.Ordinal)
        Catch
            fechaCreacionUtc = DateTime.MinValue
            idNota = 0L
            Return False
        End Try
    End Function

    Private Shared Function EsContextoValido(ByVal contexto As ContextoModuloWorkflow,
                                              ByVal tarea As TareaWorkflow) As Boolean
        Return contexto IsNot Nothing AndAlso contexto.EsValido() AndAlso tarea IsNot Nothing AndAlso tarea.EstaActiva AndAlso
               tarea.IdTarea > 0 AndAlso Not String.IsNullOrWhiteSpace(tarea.TokenVersion)
    End Function

    Private Shared Function NormalizarUtc(ByVal value As DateTime) As DateTime
        If value.Kind = DateTimeKind.Utc Then Return value
        Return DateTime.SpecifyKind(value, DateTimeKind.Utc)
    End Function
End Class
