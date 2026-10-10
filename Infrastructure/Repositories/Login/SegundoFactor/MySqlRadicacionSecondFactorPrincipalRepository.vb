Public NotInheritable Class MySqlRadicacionSecondFactorPrincipalRepository
    Inherits MySqlSecondFactorPrincipalRepositoryBase

    Private Const SelectSql As String = "SELECT id_usuario,Login_usuario,Correo_Usuario FROM usuario_radicador WHERE Login_usuario=@login LIMIT 2"

    Public Sub New(ByVal connections As IModuleConnectionFactory, ByVal executor As IDataExecutor)
        MyBase.New(connections, executor, SelectSql, "id_usuario", "Login_usuario", "Correo_Usuario")
    End Sub
End Class
