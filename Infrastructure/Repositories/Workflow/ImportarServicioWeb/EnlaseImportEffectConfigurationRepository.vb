Imports System

' Configuración explícita de efectos para anexos ENLASE: almacenamiento documental sin expediente.
Public NotInheritable Class EnlaseImportEffectConfigurationRepository
    Implements IImportEffectConfigurationRepository

    Public Function Obtener(ByVal contexto As ContextoImportacionServicio) As ImportEffectConfiguration Implements IImportEffectConfigurationRepository.Obtener
        If contexto Is Nothing OrElse
           Not String.Equals(contexto.Capability, SiiImportProvider.AnnexesEnlaseCapability, StringComparison.OrdinalIgnoreCase) Then
            Return Nothing
        End If
        Return New ImportEffectConfiguration With {
            .ExpedientRequired = False,
            .ExpedientMode = ModoExpedienteImportacion.SinExpediente,
            .AutomaticCreationEnabled = False,
            .MultipleExpedients = False}
    End Function
End Class