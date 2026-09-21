Imports System
Imports System.Collections.Generic

' Proyección de solo lectura: reutiliza el repositorio MySQL parametrizado de configuración física.
Public NotInheritable Class MySqlImportEffectConfigurationRepository
    Implements IImportEffectConfigurationRepository

    Private ReadOnly _source As IImportExpedientConfigurationRepository

    Public Sub New(ByVal source As IImportExpedientConfigurationRepository)
        If source Is Nothing Then Throw New ArgumentNullException("source")
        _source = source
    End Sub

    Public Function Obtener(ByVal contexto As ContextoImportacionServicio) As ImportEffectConfiguration Implements IImportEffectConfigurationRepository.Obtener
        Dim source = _source.Obtener(contexto)
        If source Is Nothing Then Return Nothing
        Dim result As New ImportEffectConfiguration With {
            .ExpedientRequired = source.ExpedienteObligatorio,
            .ExpedientMode = source.Modo,
            .AutomaticCreationEnabled = source.CreacionAutomaticaHabilitada,
            .MultipleExpedients = source.MultiplesExpedientes}
        If source.CamposIdentidad IsNot Nothing Then
            For Each field In source.CamposIdentidad
                If field IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(field.NombreCampo) Then result.IdentityFields.Add(field.NombreCampo.Trim())
            Next
        End If
        If source.TipologiasSecundarias IsNot Nothing Then
            For Each documentTypeId In source.TipologiasSecundarias
                If documentTypeId > 0 Then result.SecondaryDocumentTypeIds.Add(documentTypeId)
            Next
        End If
        Return result
    End Function
End Class
