Imports System
Imports System.Collections.Generic

Public NotInheritable Class SecondFactorPrincipalRepositoryResolver
    Implements ISecondFactorPrincipalRepositoryResolver

    Private ReadOnly _repositories As IDictionary(Of String, ISecondFactorPrincipalRepository)

    Public Sub New(ByVal docuarchi As ISecondFactorPrincipalRepository,
                   ByVal gestor As ISecondFactorPrincipalRepository,
                   ByVal radicacion As ISecondFactorPrincipalRepository,
                   ByVal workflow As ISecondFactorPrincipalRepository)
        If docuarchi Is Nothing Then Throw New ArgumentNullException(NameOf(docuarchi))
        If gestor Is Nothing Then Throw New ArgumentNullException(NameOf(gestor))
        If radicacion Is Nothing Then Throw New ArgumentNullException(NameOf(radicacion))
        If workflow Is Nothing Then Throw New ArgumentNullException(NameOf(workflow))
        _repositories = New Dictionary(Of String, ISecondFactorPrincipalRepository)(StringComparer.OrdinalIgnoreCase) From {
            {"DOCUARCHI CONTENEDOR", docuarchi},
            {"GESTOR DOCUMENTAL", gestor},
            {"RADICACION DOCUMENTAL", radicacion},
            {"WORKFLOW DOCUMENTAL", workflow}}
    End Sub

    Public Function Resolve(ByVal moduleType As String) As ISecondFactorPrincipalRepository Implements ISecondFactorPrincipalRepositoryResolver.Resolve
        If String.IsNullOrWhiteSpace(moduleType) Then Return Nothing
        Dim repository As ISecondFactorPrincipalRepository = Nothing
        If Not _repositories.TryGetValue(moduleType.Trim(), repository) Then Return Nothing
        Return repository
    End Function
End Class
