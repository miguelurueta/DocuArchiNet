Imports System
Imports System.Collections.Generic

' Operaciones legacy encapsuladas en proceso. El texto del mutador no es evidencia de postcondición.
Public Interface IDocumentExpedientPhysicalGateway
    Function ConsultarExpedientes(ByVal contexto As ContextoImportacionServicio,
                                  ByVal nombreGabinete As String,
                                  ByVal idImagen As Long) As IList(Of Long)
    Function VincularDocumento(ByVal contexto As ContextoImportacionServicio,
                               ByVal nombreGabinete As String,
                               ByVal idImagen As Long,
                               ByVal idExpediente As Long,
                               ByVal radicadoSii As String) As String
End Interface

Public NotInheritable Class DocumentExpedientRelationAdapter
    Implements IImportDocumentExpedientRelationPort

    Private ReadOnly _gateway As IDocumentExpedientPhysicalGateway

    Public Sub New(ByVal gateway As IDocumentExpedientPhysicalGateway)
        If gateway Is Nothing Then Throw New ArgumentNullException("gateway")
        _gateway = gateway
    End Sub

    Public Function Consultar(ByVal contexto As ContextoImportacionServicio,
                              ByVal documento As DocumentoRelacionadoImportacion) As ResultadoEfectoExpedienteImportacion Implements IImportDocumentExpedientRelationPort.Consultar
        If Not Valid(contexto, documento) Then Return Result(EstadoEfectoExpedienteImportacion.Fallido, "DOCUMENT_RELATION_INPUT_INVALID", EstadoRelacionDocumentoExpediente.NoConsultada, False)
        Try
            Dim relations = _gateway.ConsultarExpedientes(contexto, documento.NombreGabinete, documento.IdImagen)
            If relations Is Nothing OrElse relations.Count = 0 Then
                Return Result(EstadoEfectoExpedienteImportacion.Ausente, "DOCUMENT_RELATION_ABSENT", EstadoRelacionDocumentoExpediente.Ausente, False)
            End If
            If relations.Count > 1 Then
                Return Result(EstadoEfectoExpedienteImportacion.Conflicto, "DOCUMENT_RELATION_DUPLICATED", EstadoRelacionDocumentoExpediente.Duplicada, False)
            End If
            If relations(0) <> documento.IdExpedienteEsperado.Value Then
                Return Result(EstadoEfectoExpedienteImportacion.Conflicto, "DOCUMENT_RELATION_CROSSED", EstadoRelacionDocumentoExpediente.Cruzada, False)
            End If
            Return Result(EstadoEfectoExpedienteImportacion.Confirmado, "DOCUMENT_RELATION_CONFIRMED", EstadoRelacionDocumentoExpediente.Correcta, False)
        Catch ex As Exception
            Return Result(EstadoEfectoExpedienteImportacion.ResultadoIncierto, "DOCUMENT_RELATION_READ_UNKNOWN", EstadoRelacionDocumentoExpediente.ResultadoIncierto, True)
        End Try
    End Function

    Public Function Vincular(ByVal contexto As ContextoImportacionServicio,
                             ByVal documento As DocumentoRelacionadoImportacion) As ResultadoEfectoExpedienteImportacion Implements IImportDocumentExpedientRelationPort.Vincular
        Dim before = Consultar(contexto, documento)
        If before.Estado <> EstadoEfectoExpedienteImportacion.Ausente Then Return before

        Try
            Dim respuestaMutador = _gateway.VincularDocumento(contexto, documento.NombreGabinete, documento.IdImagen,
                                                               documento.IdExpedienteEsperado.Value, documento.RadicadoSii)
            ' La respuesta, incluido "YES", se conserva solo como dato local y no confirma el efecto.
            Dim after = Consultar(contexto, documento)
            If after.Estado = EstadoEfectoExpedienteImportacion.Confirmado OrElse
               after.Estado = EstadoEfectoExpedienteImportacion.Conflicto Then Return after
            Return Result(EstadoEfectoExpedienteImportacion.ResultadoIncierto, "DOCUMENT_RELATION_WRITE_UNKNOWN", EstadoRelacionDocumentoExpediente.ResultadoIncierto, True)
        Catch ex As Exception
            Return Result(EstadoEfectoExpedienteImportacion.ResultadoIncierto, "DOCUMENT_RELATION_WRITE_UNKNOWN", EstadoRelacionDocumentoExpediente.ResultadoIncierto, True)
        End Try
    End Function

    Private Shared Function Valid(ByVal contexto As ContextoImportacionServicio,
                                  ByVal documento As DocumentoRelacionadoImportacion) As Boolean
        Return contexto IsNot Nothing AndAlso documento IsNot Nothing AndAlso
            contexto.IdTarea > 0 AndAlso documento.IdTarea = contexto.IdTarea AndAlso
            documento.IdImagen > 0 AndAlso documento.IdExpedienteEsperado.HasValue AndAlso
            documento.IdExpedienteEsperado.Value > 0 AndAlso Not String.IsNullOrWhiteSpace(documento.NombreGabinete)
    End Function

    Private Shared Function Result(ByVal state As EstadoEfectoExpedienteImportacion,
                                  ByVal code As String,
                                  ByVal relation As EstadoRelacionDocumentoExpediente,
                                  ByVal retryable As Boolean) As ResultadoEfectoExpedienteImportacion
        Return New ResultadoEfectoExpedienteImportacion With {
            .Estado = state, .Codigo = code, .Relacion = relation, .Reintentable = retryable
        }
    End Function
End Class
