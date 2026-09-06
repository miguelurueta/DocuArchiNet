Imports System

Public Class ValidadorContextoImportacion
    Private ReadOnly _autorizacion As IAutorizacionImportacionRepository

    Public Sub New(ByVal autorizacion As IAutorizacionImportacionRepository)
        If autorizacion Is Nothing Then
            Throw New ArgumentNullException("autorizacion")
        End If
        _autorizacion = autorizacion
    End Sub

    Public Function Validar(ByVal contexto As ContextoImportacionServicio) As ResultadoValidacionContextoImportacion
        If contexto Is Nothing OrElse contexto.IdUsuario <= 0 OrElse contexto.IdTarea <= 0 OrElse
           contexto.IdRuta <= 0 OrElse contexto.IdTramite <= 0 OrElse
           String.IsNullOrWhiteSpace(contexto.LoginUsuario) OrElse String.IsNullOrWhiteSpace(contexto.ProviderId) Then
            Return ResultadoValidacionContextoImportacion.Fallido("INVALID_CONTEXT", "El contexto de importación no es válido.")
        End If
        If Not _autorizacion.UsuarioAutenticado(contexto) OrElse Not contexto.PermiteImportar OrElse
           Not _autorizacion.PermisoVigente(contexto) Then
            Return ResultadoValidacionContextoImportacion.Fallido("FORBIDDEN", "No está autorizado para importar documentos.")
        End If
        If Not _autorizacion.TareaOperable(contexto) Then
            Return ResultadoValidacionContextoImportacion.Fallido("TASK_NOT_OPERABLE", "La tarea no está disponible para esta operación.")
        End If
        If Not _autorizacion.RutaCoincide(contexto) Then
            Return ResultadoValidacionContextoImportacion.Fallido("ROUTE_MISMATCH", "La ruta de la tarea cambió.")
        End If
        If Not _autorizacion.TramiteCoincide(contexto) Then
            Return ResultadoValidacionContextoImportacion.Fallido("PROCEDURE_MISMATCH", "El trámite de la tarea cambió.")
        End If
        If Not _autorizacion.ProveedorHabilitado(contexto) Then
            Return ResultadoValidacionContextoImportacion.Fallido("PROVIDER_NOT_SUPPORTED", "El proveedor solicitado no está disponible.")
        End If
        Return ResultadoValidacionContextoImportacion.Exitoso()
    End Function
End Class
