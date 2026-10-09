Imports System.Data

' Doble mínimo para compilar el repositorio DOC-93 sin distribuir ni conectar MySql.Data.
Namespace Global.MySql.Data.MySqlClient
    Public NotInheritable Class MySqlParameter
        Implements IDataParameter

        Public Sub New(ByVal parameterName As String, ByVal value As Object)
            Me.ParameterName = parameterName
            Me.Value = value
        End Sub

        Public Property DbType As DbType Implements IDataParameter.DbType
        Public Property Direction As ParameterDirection Implements IDataParameter.Direction
        Public Property IsNullable As Boolean Implements IDataParameter.IsNullable
        Public Property ParameterName As String Implements IDataParameter.ParameterName
        Public Property SourceColumn As String Implements IDataParameter.SourceColumn
        Public Property SourceVersion As DataRowVersion Implements IDataParameter.SourceVersion
        Public Property Value As Object Implements IDataParameter.Value
    End Class
End Namespace
