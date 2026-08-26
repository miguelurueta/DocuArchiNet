Imports System
Imports System.Globalization
Imports System.Text
Imports System.Web.Security

'Token opaco exclusivo. Compromete tarea, snapshots y contexto autenticado sin exponer el historial al cliente.
Public Class DevolverUsuarioAnteriorTokenCodec
    Implements IDevolverUsuarioAnteriorTokenCodec

    Private Const DuracionMinutos As Integer = 5

    Public Function Emitir(ByVal contexto As ContextoModuloWorkflow,
                           ByVal tarea As TareaDevolverUsuarioAnterior,
                           ByVal usuarioHistorico As UsuarioHistoricoDevolverUsuarioAnterior) As String Implements IDevolverUsuarioAnteriorTokenCodec.Emitir
        If Not EsValido(contexto, tarea, usuarioHistorico) Then Return String.Empty
        Try
            Dim vence As Long = DateTime.UtcNow.AddMinutes(DuracionMinutos).Ticks
            Dim contenido As String = String.Join(ChrW(31), New String() {
                tarea.IdTarea.ToString(CultureInfo.InvariantCulture),
                tarea.IdEstadoActual.ToString(CultureInfo.InvariantCulture),
                usuarioHistorico.IdEstadoHistorico.ToString(CultureInfo.InvariantCulture),
                contexto.IdUsuarioWorkflow.ToString(CultureInfo.InvariantCulture),
                contexto.IdGrupoWorkflow.ToString(CultureInfo.InvariantCulture),
                contexto.IdRutaWorkflow.ToString(CultureInfo.InvariantCulture),
                vence.ToString(CultureInfo.InvariantCulture)})
            Dim protegido As Byte() = MachineKey.Protect(Encoding.UTF8.GetBytes(contenido), "WorkflowDevolverUsuarioAnterior", "v1")
            If protegido Is Nothing OrElse protegido.Length = 0 Then Return String.Empty
            Return Convert.ToBase64String(protegido).TrimEnd("="c).Replace("+", "-").Replace("/", "_")
        Catch
            Return String.Empty
        End Try
    End Function

    Public Function Validar(ByVal contexto As ContextoModuloWorkflow,
                            ByVal tarea As TareaDevolverUsuarioAnterior,
                            ByVal usuarioHistorico As UsuarioHistoricoDevolverUsuarioAnterior,
                            ByVal tokenVersion As String) As Boolean Implements IDevolverUsuarioAnteriorTokenCodec.Validar
        If Not EsValido(contexto, tarea, usuarioHistorico) OrElse String.IsNullOrWhiteSpace(tokenVersion) OrElse tokenVersion.Length > 512 Then Return False
        Try
            Dim base64 As String = tokenVersion.Replace("-", "+").Replace("_", "/")
            base64 = base64.PadRight(base64.Length + ((4 - base64.Length Mod 4) Mod 4), "="c)
            Dim protegido As Byte() = Convert.FromBase64String(base64)
            Dim contenidoProtegido As Byte() = MachineKey.Unprotect(protegido, "WorkflowDevolverUsuarioAnterior", "v1")
            If contenidoProtegido Is Nothing Then Return False
            Dim partes As String() = Encoding.UTF8.GetString(contenidoProtegido).Split(New Char() {ChrW(31)})
            If partes.Length <> 7 Then Return False
            Dim idTarea As Long = 0
            Dim estadoActual As Long = 0
            Dim estadoHistorico As Long = 0
            Dim idUsuario As Integer = 0
            Dim idGrupo As Integer = 0
            Dim idRuta As Integer = 0
            Dim vence As Long = 0
            If Not Long.TryParse(partes(0), NumberStyles.None, CultureInfo.InvariantCulture, idTarea) OrElse
               Not Long.TryParse(partes(1), NumberStyles.None, CultureInfo.InvariantCulture, estadoActual) OrElse
               Not Long.TryParse(partes(2), NumberStyles.None, CultureInfo.InvariantCulture, estadoHistorico) OrElse
               Not Integer.TryParse(partes(3), NumberStyles.None, CultureInfo.InvariantCulture, idUsuario) OrElse
               Not Integer.TryParse(partes(4), NumberStyles.None, CultureInfo.InvariantCulture, idGrupo) OrElse
               Not Integer.TryParse(partes(5), NumberStyles.None, CultureInfo.InvariantCulture, idRuta) OrElse
               Not Long.TryParse(partes(6), NumberStyles.None, CultureInfo.InvariantCulture, vence) Then Return False
            If vence <= DateTime.UtcNow.Ticks Then Return False
            Return idTarea = tarea.IdTarea AndAlso estadoActual = tarea.IdEstadoActual AndAlso
                   estadoHistorico = usuarioHistorico.IdEstadoHistorico AndAlso idUsuario = contexto.IdUsuarioWorkflow AndAlso
                   idGrupo = contexto.IdGrupoWorkflow AndAlso idRuta = contexto.IdRutaWorkflow
        Catch
            Return False
        End Try
    End Function

    Private Shared Function EsValido(ByVal contexto As ContextoModuloWorkflow,
                                     ByVal tarea As TareaDevolverUsuarioAnterior,
                                     ByVal usuarioHistorico As UsuarioHistoricoDevolverUsuarioAnterior) As Boolean
        Return contexto IsNot Nothing AndAlso contexto.EsValido() AndAlso tarea IsNot Nothing AndAlso tarea.EstaActiva AndAlso
               tarea.IdTarea > 0 AndAlso tarea.IdEstadoActual > 0 AndAlso usuarioHistorico IsNot Nothing AndAlso
               usuarioHistorico.IdEstadoHistorico > 0 AndAlso usuarioHistorico.IdUsuarioWorkflow > 0
    End Function
End Class
