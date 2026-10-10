Public NotInheritable Class MySqlGestorSecondFactorPrincipalRepository
    Inherits MySqlSecondFactorPrincipalRepositoryBase

    Private Const SelectSql As String = "SELECT Id_Remit_Dest_Int,Login_Usuario,Correo_Electronico FROM remit_dest_interno WHERE Login_Usuario=@login LIMIT 2"

    Public Sub New(ByVal connections As IModuleConnectionFactory, ByVal executor As IDataExecutor)
        MyBase.New(connections, executor, SelectSql, "Id_Remit_Dest_Int", "Login_Usuario", "Correo_Electronico")
    End Sub
End Class
