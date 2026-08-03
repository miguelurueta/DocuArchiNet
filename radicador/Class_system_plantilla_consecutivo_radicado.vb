Imports MySql.Data.MySqlClient

Public Class Class_system_plantilla_consecutivo_radicado
    Function Solicita_estado_registro_consecutivo_radicado(ByRef estado_consecutivo As String) As String
        Try

            Dim sqlconsulta As String = " SELECT id_consecutivo_radicado " & _
            " FROM  system_plantilla_consecutivo_radicado "
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Dat_set As New DataSet("system_plantilla_consecutivo_radicado")
            Dim Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(sqlconsulta, _
                                                              Dat_set)
            If Result <> "YES" Then
                Solicita_estado_registro_consecutivo_radicado = " función Solicita_estado_registro_consecutivo_radicado Error:   " & Result
                Exit Function
            End If
            If Dat_set.Tables(0).Rows.Count > 0 Then
                estado_consecutivo = "YES"
                Solicita_estado_registro_consecutivo_radicado = "YES"
                Exit Function
            Else
                estado_consecutivo = "NO"
                Solicita_estado_registro_consecutivo_radicado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estado_registro_consecutivo_radicado = "Inconsistencia general funcion Solicita_estado_registro_consecutivo_radicado " & ex.Message
        End Try
    End Function
    Function Inicializa_consecutivo_radicado_anual() As String
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim mySqldatReader As MySqlDataReader
        Dim sqlresultinsert As Integer = 0
        Dim anualidad As String = Now.Year
        Try
            Dim Parametro_Select_System1 As String = "SELECT  anualidad  FROM system_plantilla_consecutivo_radicado  for update"
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = Parametro_Select_System1
            mySqldatReader = myCommand.ExecuteReader()
            If mySqldatReader Is Nothing Then
                Inicializa_consecutivo_radicado_anual = "Imposible Encontrar Registro En Tabla system_plantilla_consecutivo_radicado Error Conexion"
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If mySqldatReader.HasRows = False Then
                Inicializa_consecutivo_radicado_anual = "Imposible encontrar el registro del consecutivo radicado anual"
                myConnection.Close()
                Exit Function
            End If
            mySqldatReader.Read()
            Dim anualidad_registro As String = mySqldatReader.Item(0)
            mySqldatReader.Close()
            If anualidad <> anualidad_registro Then
                Dim Parametro_Insercio As String = "update system_plantilla_consecutivo_radicado set Consecutivo_rad=0, Consecutivo_CodBarra=0, anualidad='" & anualidad & "'"
                myCommand.CommandText = Parametro_Insercio
                sqlresultinsert = myCommand.ExecuteNonQuery()
                If sqlresultinsert = 0 Then
                    myConnection.Close()
                    Inicializa_consecutivo_radicado_anual = "Imposible actualizar el consecutivo general del radicado "
                    Exit Function
                Else
                    myTrans.Commit()
                    myConnection.Close()
                    Inicializa_consecutivo_radicado_anual = "YES"
                    Exit Function
                End If
            Else
                myConnection.Close()
                Inicializa_consecutivo_radicado_anual = "YES"
                Exit Function
            End If

        Catch e As Exception
            Try
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Inicializa_consecutivo_radicado_anual = "An exception of type " + ex.GetType().ToString() + _
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Inicializa_consecutivo_radicado_anual = "Error General " & e.Message
            Exit Function
        End Try

    End Function
End Class
