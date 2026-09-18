Imports System
Imports System.Collections.Generic
Imports System.Linq

Public NotInheritable Class ImportRelatedDocumentPlan
    Public Function Construir(ByVal intencion As IntencionImportacionServicio,
                              ByVal planExpedientes As PlanExpedienteImportacion,
                              ByVal documentos As IList(Of DocumentoRelacionadoImportacion)) As PlanDocumentosRelacionadosImportacion
        Dim result As New PlanDocumentosRelacionadosImportacion With {
            .IntentId = If(intencion Is Nothing, Nothing, intencion.Id),
            .Estado = EstadoEfectoExpedienteImportacion.Pendiente
        }
        If intencion Is Nothing OrElse planExpedientes Is Nothing OrElse
           planExpedientes.Estado <> EstadoEfectoExpedienteImportacion.Confirmado OrElse documentos Is Nothing Then Return result

        Dim distinctExpedients = planExpedientes.Destinos.Select(Function(x) x.IdExpediente).Distinct().ToList()
        Dim seen As New HashSet(Of Long)()
        For Each document In documentos
            If document Is Nothing OrElse document.IdImagen <= 0 OrElse Not seen.Add(document.IdImagen) Then Continue For
            Dim destination = ResolveDestination(document, intencion, planExpedientes, distinctExpedients)
            If destination Is Nothing Then Return result
            document.IntentId = intencion.Id
            document.ClaveInscripcion = destination.ClaveInscripcion
            document.IdExpedienteEsperado = destination.IdExpediente
            document.EstadoDestino = EstadoEfectoExpedienteImportacion.Confirmado
            result.Documentos.Add(document)
        Next
        If result.Documentos.Count <> seen.Count Then Return result
        result.Estado = EstadoEfectoExpedienteImportacion.Confirmado
        Return result
    End Function

    Private Shared Function ResolveDestination(ByVal document As DocumentoRelacionadoImportacion,
                                               ByVal intent As IntencionImportacionServicio,
                                               ByVal plan As PlanExpedienteImportacion,
                                               ByVal distinctExpedients As IList(Of Long)) As DestinoLogicoExpedienteImportacion
        Dim storedItems = intent.Resultados.Where(Function(x) x.IdDocumento.HasValue AndAlso x.IdDocumento.Value = document.IdImagen).ToList()
        If storedItems.Count > 1 Then Return Nothing
        If storedItems.Count = 1 Then
            Return SingleDestination(plan, storedItems(0).ClaveInscripcion, storedItems(0).IdTipoDocumental)
        End If
        If document.IdExpedienteEsperado.HasValue Then
            Dim matches = plan.Destinos.Where(Function(x) x.IdExpediente = document.IdExpedienteEsperado.Value).ToList()
            If matches.Count = 1 Then Return matches(0)
        End If
        If document.IdTipoDocumental.HasValue Then
            Return SingleDestination(plan, Nothing, document.IdTipoDocumental)
        End If
        If distinctExpedients.Count = 1 Then Return plan.Destinos.First()
        Return Nothing
    End Function

    Private Shared Function SingleDestination(ByVal plan As PlanExpedienteImportacion,
                                              ByVal inscriptionKey As String,
                                              ByVal documentType As Nullable(Of Integer)) As DestinoLogicoExpedienteImportacion
        Dim matches = plan.Destinos.Where(Function(x)
            Return (String.IsNullOrWhiteSpace(inscriptionKey) OrElse x.ClaveInscripcion = inscriptionKey) AndAlso
                   (Not documentType.HasValue OrElse x.IdTipoDocumental = documentType)
        End Function).ToList()
        Return If(matches.Count = 1, matches(0), Nothing)
    End Function
End Class
