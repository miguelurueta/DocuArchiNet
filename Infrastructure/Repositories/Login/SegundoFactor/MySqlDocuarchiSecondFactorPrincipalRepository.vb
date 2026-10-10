Public NotInheritable Class MySqlDocuarchiSecondFactorPrincipalRepository
    Inherits MySqlSecondFactorPrincipalRepositoryBase

    Private Const SelectSql As String = "SELECT Clave_Usuario,idusuario,correo FROM usuarios_da WHERE idusuario=@login LIMIT 2"

    Public Sub New(ByVal connections As IModuleConnectionFactory, ByVal executor As IDataExecutor)
        MyBase.New(connections, executor, SelectSql, "Clave_Usuario", "idusuario", "correo")
    End Sub
End Class
