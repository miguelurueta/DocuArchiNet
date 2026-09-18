Imports System

' La caché legacy se usa solo como indicio; el coordinador verifica después el expediente físico.
Public NotInheritable Class LegacyImportExpedientCacheRepository
    Implements IImportExpedientCacheRepository

    Public Function Obtener(ByVal contexto As ContextoImportacionServicio,
                            ByVal inscripcion As InscripcionImportacion) As InscripcionImportacion Implements IImportExpedientCacheRepository.Obtener
        Return Nothing ' La clave hash moderna no tiene equivalencia segura en la caché global legacy.
    End Function

    Public Function RegistrarVerificado(ByVal contexto As ContextoImportacionServicio,
                                        ByVal inscripcion As InscripcionImportacion) As ResultadoEfectoExpedienteImportacion Implements IImportExpedientCacheRepository.RegistrarVerificado
        If contexto Is Nothing OrElse inscripcion Is Nothing OrElse Not inscripcion.IdExpediente.HasValue OrElse inscripcion.IdExpediente.Value <= 0 OrElse inscripcion.IdExpediente.Value > Integer.MaxValue Then Return Result(EstadoEfectoExpedienteImportacion.Fallido, "EXPEDIENT_CACHE_INPUT_INVALID")
        Dim identity = If(String.IsNullOrWhiteSpace(inscripcion.MatriculaNormalizada), If(String.IsNullOrWhiteSpace(inscripcion.MatriculaPropietario), inscripcion.Matricula, inscripcion.MatriculaPropietario), inscripcion.MatriculaNormalizada)
        identity = If(identity, String.Empty).Replace("S0", String.Empty).Trim()
        Dim legacyRepository As New ClassRaSIiCacheExpediente()
        Dim existing As New CStruSiiCahcheExpediente()
        Dim response = legacyRepository.SolicitaCacheCreacionExpedienteSII(identity, inscripcion.NombreGabinete, existing)
        If Not String.Equals(response, "YES", StringComparison.OrdinalIgnoreCase) Then Return Result(EstadoEfectoExpedienteImportacion.ResultadoIncierto, "EXPEDIENT_CACHE_READ_FAILED")
        If existing.IdExpediente > 0 Then
            If existing.IdExpediente <> CInt(inscripcion.IdExpediente.Value) Then Return Result(EstadoEfectoExpedienteImportacion.Conflicto, "EXPEDIENT_CACHE_CONFLICT")
            Return Result(EstadoEfectoExpedienteImportacion.Confirmado, "EXPEDIENT_CACHE_CONFIRMED")
        End If
        Dim entry As New CStruSiiCahcheExpediente With {.RadicadoSII = inscripcion.RadicadoSii, .CodBarras = inscripcion.RadicadoSii, .NitIdentificacion = inscripcion.IdentificacionSujeto, .Rsocial = inscripcion.RazonSocial, .NombreGabinete = inscripcion.NombreGabinete, .Matricula = identity, .IdExpediente = CInt(inscripcion.IdExpediente.Value), .EstadoPadre = If(inscripcion.RolExpediente = RolExpedienteImportacion.Secundario, 0, 1)}
        Dim cacheId As Integer = 0
        response = legacyRepository.RegistraCacheCreacionExpedienteSII(entry, cacheId)
        If Not String.Equals(response, "YES", StringComparison.OrdinalIgnoreCase) Then Return Result(EstadoEfectoExpedienteImportacion.ResultadoIncierto, "EXPEDIENT_CACHE_WRITE_UNCERTAIN")
        Dim confirmed As New CStruSiiCahcheExpediente()
        response = legacyRepository.SolicitaCacheCreacionExpedienteSII(identity, inscripcion.NombreGabinete, confirmed)
        If Not String.Equals(response, "YES", StringComparison.OrdinalIgnoreCase) OrElse confirmed.IdExpediente <> entry.IdExpediente Then Return Result(EstadoEfectoExpedienteImportacion.ResultadoIncierto, "EXPEDIENT_CACHE_WRITE_UNCERTAIN")
        Return Result(EstadoEfectoExpedienteImportacion.Confirmado, "EXPEDIENT_CACHE_CONFIRMED")
    End Function

    Private Shared Function Result(ByVal state As EstadoEfectoExpedienteImportacion, ByVal code As String) As ResultadoEfectoExpedienteImportacion
        Return New ResultadoEfectoExpedienteImportacion With {.Estado = state, .Codigo = code, .Reintentable = state = EstadoEfectoExpedienteImportacion.ResultadoIncierto}
    End Function
End Class
