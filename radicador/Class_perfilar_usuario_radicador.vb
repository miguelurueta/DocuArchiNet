Public Class Class_perfilar_usuario_radicador
    Function AsignaPermisosSesionUsuarioRadicador(ByVal IdUsuarioRadicacion As Integer) As String
        Dim refra As New conect.Dbase_Conction_Mysql_RA
        Dim Result As String = ""
        Dim Datset As New DataSet
        Try
            Dim Sqlstext As String = "Select * from perfilar_usuario_radicador where Usuario_Radicador_id_usuario=" &
                IdUsuarioRadicacion
            Result = refra.SELECTION_SELECT_FIELD(Sqlstext, Datset)
            If Result <> "YES" Then
                AsignaPermisosSesionUsuarioRadicador = "Funcion AsignaPermisosSesionUsuarioRadicador ClassInicioworkflow " & Result.ToString
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                AsignaPermisosSesionUsuarioRadicador = "No se encontraron los permisos generales de usuario "
                Exit Function
            Else
                HttpContext.Current.Session("RA_PERMISO_RADICADO") = Datset.Tables(0).Rows(0).Item("Permiso_Radicado")
                HttpContext.Current.Session("RA_PERMISO_ADICIONAR_DEST_INTERNO") = Datset.Tables(0).Rows(0).Item("Permiso_Adicionar_Dest_Interno")
                HttpContext.Current.Session("RA_PERMISO_CONSULTA") = Datset.Tables(0).Rows(0).Item("Permiso_Consulta")
                HttpContext.Current.Session("RA_PERMISO_EDITA_RADICADO") = Datset.Tables(0).Rows(0).Item("Permiso_Eidta_Radicado")
                HttpContext.Current.Session("RA_PERMISO_ELIMINA_RADICADO") = Datset.Tables(0).Rows(0).Item("Permiso_Elimina_Radicado")
                HttpContext.Current.Session("RA_PERMISO_GENERAR_GUIA") = Datset.Tables(0).Rows(0).Item("Permiso_Generar_Guia")
                HttpContext.Current.Session("RA_PERMISO_IMPRIMIR_GUIA") = Datset.Tables(0).Rows(0).Item("Permiso_Imprimir_Guia")
                HttpContext.Current.Session("RA_PERMISO_ELIMINAR_GUIA") = Datset.Tables(0).Rows(0).Item("Permiso_Eliminar_Guia")
                HttpContext.Current.Session("RA_PERMISO_EDITAR_GUIA") = Datset.Tables(0).Rows(0).Item("Permiso_Editar_Guia")
                If Datset.Tables(0).Columns.Count > 9 Then
                    HttpContext.Current.Session("RA_PERMISO_GESTION_RESPUESTA") = Datset.Tables(0).Rows(0).Item("Permiso_Gestion_Respuesta")
                    HttpContext.Current.Session("RA_PERMISO_GESTION_CORRESPONDENCIA") = Datset.Tables(0).Rows(0).Item("Permiso_Gestion_Correspondencia")
                    HttpContext.Current.Session("RA_PERMISO_GESTION_REPORTES") = Datset.Tables(0).Rows(0).Item("Permiso_Gestion_Reportes")
                End If
                HttpContext.Current.Session.Item("RA_PERMISO_REMISION_CORRESPONDENCIA_INTERNA") = Datset.Tables(0).Rows(0).Item("Permiso_remision_correspondencia_interna")
                HttpContext.Current.Session.Item("RA_PERMISO_GESTION_CORRESPONDENCIA_SIMPLE") = Datset.Tables(0).Rows(0).Item("Permiso_Gestion_Correspondencia_Simple")
                AsignaPermisosSesionUsuarioRadicador = "YES"
                Exit Function
            End If

        Catch ex As Exception
            AsignaPermisosSesionUsuarioRadicador = "Funcion AsignaPermisosSesionUsuarioRadicador " & ex.Message
        End Try
    End Function
End Class
