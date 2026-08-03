Public Class Class_relacion_usu_grup
    Function Solicita_grupo_usuario_docuarchi(ByVal id_usuario As Integer, _
                                              ByRef id_grupo As Integer) As String
        Try
            Dim Parametro_Consulta As String = "select  GRUPOS_DA_Clave_Grupo  from  relacion_usu_grup " & _
                  " where USUARIOS_DA_Clave_Usuario='" & id_usuario & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("permisos_grupos_gabinetes")
            Dim Result As String = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_grupo_usuario_docuarchi = " La funcion Solicita_grupo_usuario_docuarchi dice : " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_grupo = 0
                Solicita_grupo_usuario_docuarchi = "YES"
                Exit Function
            Else
                id_grupo = Datset.Tables(0).Rows(0).Item(0)
                Solicita_grupo_usuario_docuarchi = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_grupo_usuario_docuarchi = "Inconsistencia general función Solicita_grupo_usuario_docuarchi " & ex.Message
        End Try
    End Function
    Function SolicitaGrupoRelacionadousuarioDocuarchi(ByVal IdUsuarioDocuarchi As Integer,
                                                      ByRef IdGrupoDocuarchi As Integer) As String
        '--------------------------------------------------------------
        'Funcion : Solicita los datos de los gabinetes permitidos
        'por el grupo
        'Fecha : 2015-09-09
        'Ingeniero : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select  GRUPOS_DA_Clave_Grupo  from  relacion_usu_grup " &
                   " where USUARIOS_DA_Clave_Usuario='" & IdUsuarioDocuarchi & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("relacion_usu_grup")
            Dim Result As String = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                SolicitaGrupoRelacionadousuarioDocuarchi = " La función SolicitaGrupoRelacionadousuarioDocuarchi dice : " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                IdGrupoDocuarchi = 0
                SolicitaGrupoRelacionadousuarioDocuarchi = "YES"
                Exit Function
            Else
                IdGrupoDocuarchi = Datset.Tables(0).Rows(0).Item(0)
                SolicitaGrupoRelacionadousuarioDocuarchi = "YES"
            End If
        Catch ex As Exception
            SolicitaGrupoRelacionadousuarioDocuarchi = "Inconsistencia función  SolicitaGrupoRelacionadousuarioDocuarchi " & ex.Message
        End Try
    End Function
End Class
