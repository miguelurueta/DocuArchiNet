Imports System
Imports System.Collections.Generic
Imports System.Globalization

Public NotInheritable Class ImportEffectConfiguration
    Public Sub New()
        IdentityFields = New List(Of String)()
        SecondaryDocumentTypeIds = New List(Of Integer)()
    End Sub
    Public Property ExpedientRequired As Boolean
    Public Property AutomaticCreationEnabled As Boolean
    Public Property MultipleExpedients As Boolean
    Public Property IdentityFields As IList(Of String)
    Public Property SecondaryDocumentTypeIds As IList(Of Integer)

    Public Function CanonicalValue() As String
        Dim fields As New List(Of String)(IdentityFields) : fields.Sort(StringComparer.OrdinalIgnoreCase)
        Dim types As New List(Of Integer)(SecondaryDocumentTypeIds) : types.Sort()
        Return ExpedientRequired.ToString() & "|" & AutomaticCreationEnabled.ToString() & "|" &
            MultipleExpedients.ToString() & "|" & String.Join(",", fields.ToArray()) & "|" &
            String.Join(",", types.ConvertAll(Function(value) value.ToString(CultureInfo.InvariantCulture)).ToArray())
    End Function
End Class

Public NotInheritable Class ImportEffectPlanBuilder
    Private Shared ReadOnly EffectCodes As String() = {"DOCUMENT_STORAGE", "EXPEDIENT_RESOLUTION", "DOCUMENT_LINK", "LINK_CACHE", "DOCUMENT_INDEXES"}

    Public Function Build(ByVal context As ContextoImportacionServicio,
                          ByVal items As IEnumerable(Of ImportItemSelectionDto),
                          ByVal configuration As ImportEffectConfiguration) As IList(Of ImportEffectPlanDto)
        Dim plans As New List(Of ImportEffectPlanDto)()
        If context Is Nothing OrElse items Is Nothing OrElse configuration Is Nothing Then Return plans
        For Each item In items
            Dim plan As New ImportEffectPlanDto With {
                .ClientItemId = item.ClientItemId.Trim(), .TargetTaskId = context.IdTarea,
                .DocumentTypeId = item.DocumentTypeId, .DocumentTypeName = item.DocumentTypeName.Trim(),
                .DestinationMode = If(configuration.MultipleExpedients, "Multiple", "Single"),
                .ExpedientRequired = configuration.ExpedientRequired}
            For Each code In EffectCodes
                plan.Effects.Add(New ImportPlannedEffectDto With {.Code = code, .Status = "Planned"})
            Next
            plan.Requirements.Add(New ImportRequirementDto With {.Codigo = "EFFECT_CONFIGURATION_AVAILABLE", .Satisfecho = True})
            plan.Requirements.Add(New ImportRequirementDto With {.Codigo = "EXPEDIENT_DESTINATION_PLANNED", .Satisfecho = True})
            plans.Add(plan)
        Next
        Return plans
    End Function
End Class
