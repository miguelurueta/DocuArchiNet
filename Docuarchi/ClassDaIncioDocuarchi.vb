Public Class ClassDaIncioDocuarchi
    Function Cambiar_Contraseña_da(ByVal Pawsuno As String, ByVal paswdos As String) As String
        Try
            '*****************************************************
            'Verifica que los campos contraseña no esten vacios
            '*****************************************************
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("CONFIGURACION_GABINETE")
            Dim Result As String = ""
            If Pawsuno = "" Then
                Cambiar_Contraseña_da = "La primera contraseña debe informarse"
                Exit Function
            End If
            If paswdos = "" Then
                Cambiar_Contraseña_da = "La segunda contraseña debe informarse"
                Exit Function
            End If
            '******************************************************
            'La contraseña debe tener mas de ocho caracteres 
            '*****************************************************
            If Pawsuno.Length < 8 Then
                Cambiar_Contraseña_da = "La contraseña debe tener mínimo 8 caracteres"
                Exit Function
            End If
            '******************************************************
            'Compara las contraseña entre minusculas y mayusculas
            '******************************************************
            Dim compara As Integer = -2
            Dim Srcomuno As String = Pawsuno
            Dim Srcomdos As String = paswdos
            compara = StrComp(Srcomuno, Srcomdos, _
             CompareMethod.Binary)
            If compara = 0 Then
            Else
                Cambiar_Contraseña_da = "Las contraseñas no coinciden, tenga en cuenta que el sistema diferencia entre minúsculas y mayusculas"
                Exit Function
            End If
            '******************************************************
            'Encriptacion de contraseñas
            '******************************************************
            Dim Contraseña_Encript As String = Pawsuno
            Result = Encrip_Value(Contraseña_Encript)
            If Result <> "YES" Then
                Cambiar_Contraseña_da = "Imposible Encriptar la contraseña " & Result
                Exit Function
            End If
            '*******************************************************
            'Actualizacion de la base de datos
            '*******************************************************
            Dim Sqlupdate As String = "Update usuarios_da   " & _
                "set pasword='" & Pawsuno & "'" & _
                ", pasw_encript='" & Contraseña_Encript & "'" & _
                " where Clave_Usuario=" & HttpContext.Current.Session.Item("ID_USUARIO_DOCUARCHI")
            Dim Resultado_Insertar As String = ref.SELECTION_INSERT_COMMAND(Sqlupdate)
            If Resultado_Insertar <> "YES" Then
                Cambiar_Contraseña_da = "Funcion update Error : " & Resultado_Insertar
                Exit Function
            End If
            Cambiar_Contraseña_da = "YES"
        Catch ex As Exception
            Cambiar_Contraseña_da = "Inconsistencia General Función Cambiar_Contraseña_da " & ex.Message
        End Try
    End Function


    Function SolicitaloginUsuarioDocuarchi(ByVal id_usuario As Integer,
                                             ByRef login_usuario As String) As String
        '--------------------------------------------------------------
        'Funcion : Solicita login usuario docuarchi
        '
        'Fecha : 2015-09-24
        'Ingeniero : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select  idusuario  from  usuarios_da " &
                      " where Clave_Usuario='" & id_usuario & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("usuarios_da")
            Dim Result As String = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                SolicitaloginUsuarioDocuarchi = " La función SolicitaloginUsuarioDocuarchi dice : " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaloginUsuarioDocuarchi = " La función SolicitaloginUsuarioDocuarchi dice imposble encontrar el usuario " & id_usuario.ToString
                Exit Function
            Else
                login_usuario = UCase(Datset.Tables(0).Rows(0).Item(0))
                SolicitaloginUsuarioDocuarchi = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaloginUsuarioDocuarchi = "Inconsistencia función  SolicitaloginUsuarioDocuarchi " & ex.Message
        End Try
    End Function
    Function Retorna_id_usuario_docuarchi(ByRef id_usuario As Integer, _
                                          ByVal login_usuario As String) As String
        '--------------------------------------------------------------
        'Funcion : Solicita login usuario docuarchi
        '
        'Fecha : 2015-09-24
        'Ingeniero : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select  Clave_Usuario  from  usuarios_da " & _
                      " where idusuario='" & login_usuario & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("usuarios_da")
            Dim Result As String = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_id_usuario_docuarchi = " La función Retorna_id_usuario_docuarchi dice : " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_id_usuario_docuarchi = " La función Retorna_id_usuario_docuarchi dice imposble encontrar el usuario " & login_usuario
                Exit Function
            Else
                id_usuario = UCase(Datset.Tables(0).Rows(0).Item(0))
                Retorna_id_usuario_docuarchi = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_usuario_docuarchi = "Inconsistencia función  Retorna_id_usuario_docuarchi " & ex.Message
        End Try
    End Function
    Function RegtraLogSesionUsuarioDocuarchi(ByVal IdUsuarioDocuarchi As Integer,
                                             ByVal DirecionIpUsusario As String,
                                             ByRef CodigoTransaccion As Integer) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim refclas_gestion_fechas As New ClassGestionFechas
            Dim Result As String = ""
            Dim date1al As String = Date.Now
            Result = refclas_gestion_fechas.Formatea_Fecha_Almacenamiento_Time(date1al)
            If Result <> "YES" Then
                RegtraLogSesionUsuarioDocuarchi = "Imposible formatear fecha " & Result
                Exit Function
            End If
            Dim sql_insert As String = "Insert into log_usuario (Usuario_Workflow_idU_suario,Fecha_Inicio_Seccion,Direccion_ip_Nombre) values (" &
                IdUsuarioDocuarchi & ",'" & date1al & "','" & DirecionIpUsusario & "')"
            Result = ref.SELECTION_LAST_INSERT_COMMAND(sql_insert, CodigoTransaccion)
            If Result <> "YES" Then
                RegtraLogSesionUsuarioDocuarchi = Result
                Exit Function
            Else
                RegtraLogSesionUsuarioDocuarchi = "YES"
                Exit Function
            End If
        Catch ex As Exception
            RegtraLogSesionUsuarioDocuarchi = "Inconsistencia general función RegtraLogSesionUsuarioDocuarchi " & ex.Message
        End Try
    End Function
    Function update_log_sesion_usuario_docuarchi(ByVal id_log As Integer) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim refclas_gestion_fechas As New ClassGestionFechas
            Dim date1al As String = Date.Now
            Dim Result As String = ""
            Result = refclas_gestion_fechas.Formatea_Fecha_Almacenamiento_Time(date1al)
            If Result <> "YES" Then
                update_log_sesion_usuario_docuarchi = "Imposible formatear fecha " & Result
                Exit Function
            End If
            Dim sql_insert As String = "update log_usuario set Fecha_Fin_Seccion=" & _
                 "'" & date1al & "' where id_log=" & id_log
            Result = ref.SELECTION_INSERT_COMMAND(sql_insert)
            If Result <> "YES" Then
                update_log_sesion_usuario_docuarchi = Result
                Exit Function
            Else
                update_log_sesion_usuario_docuarchi = "YES"
                Exit Function
            End If
        Catch ex As Exception
            update_log_sesion_usuario_docuarchi = "Inconsistencia general función update_log_sesion_usuario_docuarchi " & ex.Message
        End Try
    End Function
    Function Listar_Treeview_docuarchi(ByRef Tre_v2 As TreeView, _
                                       Optional ByVal lista_gestor As Integer = 0) As String
        '*******************************************************
        'Funcion : Crea treeview con las opciones de workflow
        'Fecha : 2012-10-04
        'Ingeniero: Miguel Angel Urueta Miranda
        '*******************************************************
        Try
            Dim Tre_v As New TreeNode
            Tre_v.ChildNodes.Clear()
            Tre_v.Text = "DocuArchi Contenedor"
            Dim attrNode1Gru As New TreeNode
            attrNode1Gru.Text = "Contenedor de documentos"
            attrNode1Gru.Value = "DA-CLI-01|../docuarhi/WebFormDaPrincipal.aspx"
            attrNode1Gru.ImageUrl = "../workflow/imageneswf/books-light.png"
            Tre_v.ChildNodes.Add(attrNode1Gru)
            Dim attrNode1Client As New TreeNode
            attrNode1Client.Text = "Autenticación"
            attrNode1Client.Value = "DA-CLI-02|../Docuarhi/WebFormDaCambiarPaswordDa.aspx"
            attrNode1Client.ImageUrl = "../workflow/imageneswf/key-light.png"
            If lista_gestor = 0 Then
                Tre_v.ChildNodes.Add(attrNode1Client)
            End If
            Tre_v2.Nodes.Add(Tre_v)
            Listar_Treeview_docuarchi = "YES"
        Catch ex As Exception
            Listar_Treeview_docuarchi = "Inconsistencia generando treview función Listar_Treeview_docuarchi  " & ex.Message
        End Try
    End Function
End Class
