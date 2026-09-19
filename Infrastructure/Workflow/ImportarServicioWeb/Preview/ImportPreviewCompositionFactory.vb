Imports System

Public NotInheritable Class ImportPreviewCompositionFactory
    Private Sub New()
    End Sub

    Public Shared Function CreateRepository(ByVal workflowConnectionString As String,
                                            ByVal moduleContext As ContextoModulo) As IImportPreviewDescriptorRepository
        If String.IsNullOrWhiteSpace(workflowConnectionString) Then Throw New InvalidOperationException("PREVIEW_CONTEXT_UNAVAILABLE")
        Return New ImportPreviewDescriptorRepository(New WorkflowModuleConnectionFactory(workflowConnectionString),
            New AdoNetDataExecutor(), New DbTransactionFactory(), moduleContext)
    End Function

    Public Shared Function CreateDescriptorService(ByVal workflowConnectionString As String,
                                                   ByVal moduleContext As ContextoModulo,
                                                   ByVal maximumBytes As Long,
                                                   ByVal ttl As TimeSpan) As ImportPreviewDescriptorService
        Return New ImportPreviewDescriptorService(CreateRepository(workflowConnectionString, moduleContext), maximumBytes, ttl)
    End Function

    Public Shared Function CreateContentService(ByVal workflowConnectionString As String,
                                                ByVal moduleContext As ContextoModulo) As ImportPreviewContentService
        Return New ImportPreviewContentService(CreateRepository(workflowConnectionString, moduleContext))
    End Function
End Class
