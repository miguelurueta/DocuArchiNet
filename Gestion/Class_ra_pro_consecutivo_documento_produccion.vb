Imports MySql.Data.MySqlClient

Public Class Class_ra_pro_consecutivo_documento_produccion
    Function Solicita_consecutivo_produccion(ByVal id_consecutivo As Long, _
                                             ByRef zero_fil As String) As String
        Dim Class_zero_fill As New Class_zero_fill
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_DA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim mySqldatReader As MySqlDataReader
        Dim Result As String = ""
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            Dim Parametro_Select_System1 As String = " SELECT consecutivo_arhivo FROM ra_pro_consecutivo_documento_produccion " & _
                "where ra_pro_consecutivo=" & id_consecutivo & " for update"
            myCommand.CommandText = Parametro_Select_System1
            mySqldatReader = myCommand.ExecuteReader()
            If mySqldatReader Is Nothing Then
                Solicita_consecutivo_produccion = "Imposible Encontrar Registro En Tabla ra_pro_consecutivo_documento_produccion Error Conexion"
                myConnection.Close()
                Exit Function
            End If
            If mySqldatReader.HasRows = False Then
                Solicita_consecutivo_produccion = "Imposible Encontrar Registro En Tabla ra_pro_consecutivo_documento_produccion"
                myConnection.Close()
                Exit Function
            End If
            mySqldatReader.Read()
            Dim consecutivo As Long = mySqldatReader.Item(0)
            consecutivo += 1
            mySqldatReader.Close()
            Dim valor_zero_fill As String = consecutivo.ToString
            Result = Class_zero_fill.zero_fill(valor_zero_fill, 10, "0")
            If Result <> "YES" Then
                Solicita_consecutivo_produccion = "Imposible agregar zerofill " & Result
                myConnection.Close()
                Exit Function
            End If
            Dim sqlresultinsert As Integer = 0
            Dim Parametro_Actualiza_System1 As String = "update ra_pro_consecutivo_documento_produccion set consecutivo_arhivo = " & consecutivo & _
                " where ra_pro_consecutivo=" & id_consecutivo
            myCommand.CommandText = Parametro_Actualiza_System1
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Solicita_consecutivo_produccion = "Imposible actualizar el consecutivo de documentos de producción documental  "
                'mySqldatReader.Close()
                myConnection.Close()
                Exit Function
            End If
            zero_fil = valor_zero_fill
            myTrans.Commit()
            myConnection.Close()
            Solicita_consecutivo_produccion = "YES"
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Solicita_consecutivo_produccion = "An exception of type " + ex.GetType().ToString() + _
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Solicita_consecutivo_produccion = "Error General " & e.Message
            Exit Function
        End Try
    End Function
    Function Solicita_id_consectivo_usuario_producion(ByVal id_usuario_gestion As Integer, _
                                                      ByRef id_consecutivo As Long) As String
        Try
            Dim Parametro_Consulta As String = "SELECT ra_pro_consecutivo FROM  ra_pro_consecutivo_documento_produccion WHERE id_usuario_gestion=" & _
                 id_usuario_gestion
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_pro_consecutivo_documento_produccion")
            Dim result As String = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If result <> "YES" Then
                Solicita_id_consectivo_usuario_producion = "Error función Solicita_id_consectivo_usuario_producion " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_consecutivo = 0
                Solicita_id_consectivo_usuario_producion = "YES"
                Exit Function
            Else
                id_consecutivo = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_consectivo_usuario_producion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_consectivo_usuario_producion = "Incinsistencia general función Solicita_id_consectivo_usuario_producion " & ex.Message
        End Try
    End Function
    Function Registra_conecutivo_producio_usuario_gestion(ByVal id_usuario_gestion As Integer, _
                                                          ByRef id_consecutivo_producion As Long) As String
        Try
            Dim Parametro_insert As String = "INSERT INTO  ra_pro_consecutivo_documento_produccion " & _
                 "(consecutivo_arhivo,id_usuario_gestion) VALUES (0," & id_usuario_gestion & ")"
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim result As String = ref.SELECTION_LAST_INSERT_COMMAND(Parametro_insert, id_consecutivo_producion)
            If result <> "YES" Then
                Registra_conecutivo_producio_usuario_gestion = "Error función Registra_conecutivo_producio_usuario_gestion " & result
                Exit Function
            Else
                Registra_conecutivo_producio_usuario_gestion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Registra_conecutivo_producio_usuario_gestion = "Inconsistencia general función Registra_conecutivo_producio_usuario_gestion " & ex.Message
        End Try
    End Function
End Class
