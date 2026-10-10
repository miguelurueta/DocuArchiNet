Public NotInheritable Class MySqlWorkflowSecondFactorPrincipalRepository
    Inherits MySqlSecondFactorPrincipalRepositoryBase

    Private Const SelectSql As String = "SELECT idU_suario,login_Usuario,Correo_Usuario FROM usuario_workflow WHERE login_Usuario=@login LIMIT 2"

    Public Sub New(ByVal connections As IModuleConnectionFactory, ByVal executor As IDataExecutor)
        MyBase.New(connections, executor, SelectSql, "idU_suario", "login_Usuario", "Correo_Usuario")
    End Sub
End Class
