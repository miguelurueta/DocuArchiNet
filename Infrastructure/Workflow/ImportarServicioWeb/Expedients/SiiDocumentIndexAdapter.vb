Imports System
Imports System.Collections.Generic

' Frontera in-process para índices físicos SQL y XML; ambas evidencias permanecen independientes.
Public Interface ISiiDocumentIndexPhysicalGateway
    Function ActualizarCampos(ByVal contexto As ContextoImportacionServicio,
                              ByVal documento As DocumentoRelacionadoImportacion,
                              ByVal campos As IDictionary(Of String, String)) As Boolean
    Function LeerCampos(ByVal contexto As ContextoImportacionServicio,
                        ByVal documento As DocumentoRelacionadoImportacion,
                        ByVal campos As IDictionary(Of String, String)) As IDictionary(Of String, String)
    Function ExisteIndiceSql(ByVal contexto As ContextoImportacionServicio,
                             ByVal documento As DocumentoRelacionadoImportacion) As Boolean
    Function ExisteIndiceXml(ByVal contexto As ContextoImportacionServicio,
                             ByVal documento As DocumentoRelacionadoImportacion) As Boolean
End Interface

Public NotInheritable Class SiiDocumentIndexAdapter
    Implements IImportDocumentIndexUpdater
    Implements IImportElectronicIndexVerifier

    Private ReadOnly _gateway As ISiiDocumentIndexPhysicalGateway

    Public Sub New(ByVal gateway As ISiiDocumentIndexPhysicalGateway)
        If gateway Is Nothing Then Throw New ArgumentNullException("gateway")
        _gateway = gateway
    End Sub

    Public Function Actualizar(ByVal contexto As ContextoImportacionServicio,
                               ByVal documento As DocumentoRelacionadoImportacion,
                               ByVal inscripcion As InscripcionImportacion) As ResultadoEfectoExpedienteImportacion Implements IImportDocumentIndexUpdater.Actualizar
        If Not Valid(contexto, documento) OrElse inscripcion Is Nothing Then
            Return Result(EstadoEfectoExpedienteImportacion.Fallido, "DOCUMENT_INDEX_INPUT_INVALID", False)
        End If
        Dim fields = BuildFields(documento, inscripcion)
        If fields Is Nothing Then Return Result(EstadoEfectoExpedienteImportacion.Fallido, "DOCUMENT_INDEX_CABINET_NOT_SUPPORTED", False)
        Try
            If Not _gateway.ActualizarCampos(contexto, documento, fields) Then
                Return Result(EstadoEfectoExpedienteImportacion.Fallido, "DOCUMENT_INDEX_UPDATE_REJECTED", False)
            End If
            Dim persisted = _gateway.LeerCampos(contexto, documento, fields)
            Dim mismatch = MismatchedField(fields, persisted)
            If Not String.IsNullOrWhiteSpace(mismatch) Then
                Return Result(EstadoEfectoExpedienteImportacion.ResultadoIncierto, "DOCUMENT_INDEX_FIELD_NOT_CONFIRMED_" & mismatch, True)
            End If
            Return Result(EstadoEfectoExpedienteImportacion.Confirmado, "DOCUMENT_INDEX_FIELDS_CONFIRMED", False)
        Catch ex As InvalidOperationException
            If System.Text.RegularExpressions.Regex.IsMatch(If(ex.Message, String.Empty), "^DOCUMENT_INDEX_[A-Z0-9_]+$") Then
                Return Result(EstadoEfectoExpedienteImportacion.ResultadoIncierto, ex.Message, True)
            End If
            Return Result(EstadoEfectoExpedienteImportacion.ResultadoIncierto, "DOCUMENT_INDEX_UPDATE_UNKNOWN", True)
        Catch ex As Exception
            Return Result(EstadoEfectoExpedienteImportacion.ResultadoIncierto, "DOCUMENT_INDEX_UPDATE_UNKNOWN", True)
        End Try
    End Function

    Public Function Verificar(ByVal contexto As ContextoImportacionServicio,
                              ByVal documento As DocumentoRelacionadoImportacion) As EvidenciaIndiceElectronicoImportacion Implements IImportElectronicIndexVerifier.Verificar
        Dim evidence As New EvidenciaIndiceElectronicoImportacion With {
            .IdImagen = If(documento Is Nothing, 0, documento.IdImagen),
            .IdExpediente = If(documento Is Nothing OrElse Not documento.IdExpedienteEsperado.HasValue, 0, documento.IdExpedienteEsperado.Value),
            .Estado = EstadoEfectoExpedienteImportacion.Fallido
        }
        If Not Valid(contexto, documento) Then Return evidence
        Try
            evidence.SqlConfirmado = _gateway.ExisteIndiceSql(contexto, documento)
            evidence.XmlConfirmado = _gateway.ExisteIndiceXml(contexto, documento)
            evidence.Estado = If(evidence.SqlConfirmado AndAlso evidence.XmlConfirmado,
                                  EstadoEfectoExpedienteImportacion.Confirmado,
                                  EstadoEfectoExpedienteImportacion.Ausente)
            Return evidence
        Catch ex As Exception
            evidence.Estado = EstadoEfectoExpedienteImportacion.ResultadoIncierto
            Return evidence
        End Try
    End Function

    Private Shared Function BuildFields(ByVal documento As DocumentoRelacionadoImportacion,
                                        ByVal inscription As InscripcionImportacion) As IDictionary(Of String, String)
        Dim cabinet = If(documento.NombreGabinete, String.Empty).Trim().ToUpperInvariant()
        If String.IsNullOrWhiteSpace(cabinet) Then Return Nothing
        Dim enrollment = If(inscription.MatriculaNormalizada, inscription.Matricula)
        Return New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase) From {
            {"NITCEDULA", Clean(inscription.IdentificacionSujeto)},
            {"RAZONSOCIAL", Truncate(Clean(inscription.RazonSocial), 40)},
            {"MATRICULA", Clean(enrollment)}
        }
    End Function

    Private Shared Function MismatchedField(ByVal expected As IDictionary(Of String, String),
                                            ByVal actual As IDictionary(Of String, String)) As String
        If actual Is Nothing Then Return "SNAPSHOT"
        For Each pair In expected
            Dim value As String = Nothing
            If Not actual.TryGetValue(pair.Key, value) OrElse
               Not String.Equals(Clean(pair.Value), Clean(value), StringComparison.OrdinalIgnoreCase) Then Return pair.Key.ToUpperInvariant()
        Next
        Return String.Empty
    End Function

    Private Shared Function Valid(ByVal contexto As ContextoImportacionServicio,
                                  ByVal documento As DocumentoRelacionadoImportacion) As Boolean
        Return contexto IsNot Nothing AndAlso documento IsNot Nothing AndAlso contexto.IdTarea > 0 AndAlso
            contexto.IdTarea = documento.IdTarea AndAlso documento.IdImagen > 0 AndAlso
            documento.IdExpedienteEsperado.HasValue AndAlso documento.IdExpedienteEsperado.Value > 0
    End Function

    Private Shared Function Clean(ByVal value As String) As String
        Return If(value, String.Empty).Trim()
    End Function

    Private Shared Function Truncate(ByVal value As String, ByVal maximum As Integer) As String
        Return If(value.Length <= maximum, value, value.Substring(0, maximum))
    End Function

    Private Shared Function Result(ByVal state As EstadoEfectoExpedienteImportacion,
                                  ByVal code As String,
                                  ByVal retryable As Boolean) As ResultadoEfectoExpedienteImportacion
        Return New ResultadoEfectoExpedienteImportacion With {
            .Estado = state, .Codigo = code, .Reintentable = retryable
        }
    End Function
End Class
