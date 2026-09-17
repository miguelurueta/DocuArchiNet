Imports System
Imports System.Collections.Generic

Public NotInheritable Class ImportExpedientPlan
    Public Function Construir(ByVal intencion As IntencionImportacionServicio,
                              ByVal inscripciones As IList(Of InscripcionImportacion)) As PlanExpedienteImportacion
        Dim plan As New PlanExpedienteImportacion With {
            .IntentId = If(intencion Is Nothing, Nothing, intencion.Id),
            .Estado = EstadoEfectoExpedienteImportacion.Pendiente
        }
        If intencion Is Nothing OrElse inscripciones Is Nothing OrElse inscripciones.Count = 0 Then Return plan

        Dim byKey As New Dictionary(Of String, InscripcionImportacion)(StringComparer.Ordinal)
        For Each inscription In inscripciones
            If inscription Is Nothing OrElse String.IsNullOrWhiteSpace(inscription.ClaveInscripcion) OrElse
               Not inscription.IdExpediente.HasValue OrElse inscription.IdExpediente.Value <= 0 OrElse
               byKey.ContainsKey(inscription.ClaveInscripcion) Then Return plan
            byKey.Add(inscription.ClaveInscripcion, inscription)
            plan.Inscripciones.Add(inscription)
        Next

        For Each item In intencion.Resultados
            Dim inscription As InscripcionImportacion = Nothing
            If item Is Nothing OrElse String.IsNullOrWhiteSpace(item.ClientItemId) OrElse
               String.IsNullOrWhiteSpace(item.ClaveInscripcion) OrElse
               Not item.IdTipoDocumental.HasValue OrElse
               Not byKey.TryGetValue(item.ClaveInscripcion, inscription) Then Return plan
            plan.Destinos.Add(New DestinoLogicoExpedienteImportacion With {
                .ClaveInscripcion = inscription.ClaveInscripcion,
                .IdTipoDocumental = item.IdTipoDocumental,
                .IdExpediente = inscription.IdExpediente.Value,
                .Rol = inscription.RolExpediente
            })
        Next
        If plan.Destinos.Count <> intencion.Resultados.Count Then Return plan
        plan.Estado = EstadoEfectoExpedienteImportacion.Confirmado
        Return plan
    End Function
End Class
