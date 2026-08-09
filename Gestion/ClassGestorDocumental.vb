Imports MySql.Data.MySqlClient

Public Class ClassGestorDocumental
    Function Retorna_nombre_id_usuario_gestion(ByVal id_usuario_gestion As Integer, ByRef nombre_usuario_gestion As String) As String
        Try
            Dim Parametro_Consulta As String = "select Nombre_Remitente " &
                                       "from remit_dest_interno where id_Remit_Dest_Int=" & id_usuario_gestion
            Dim Ref_Car_Conec33 As New conect.Dbase_Conction_Mysql_RA
            Dim datset As DataSet = New DataSet("remit_dest_interno")
            Dim Result As String = ""
            Result = Ref_Car_Conec33.SELECTION_SELECT_FIELD(Parametro_Consulta, datset)
            If Result <> "YES" Then
                Retorna_nombre_id_usuario_gestion = "Función Retorna_nombre_id_usuario_gestion dice " & Result
                Exit Function
            End If
            If datset.Tables(0).Rows.Count = 0 Then
                Retorna_nombre_id_usuario_gestion = "Imposible encontrar la relacion entre el usuario de gestion de radicacion y el usuario docuarchi "
                Exit Function
            Else
                nombre_usuario_gestion = datset.Tables(0).Rows(0).Item(0)
                Retorna_nombre_id_usuario_gestion = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Retorna_nombre_id_usuario_gestion = "Inconsistencia General Funcion : Retorna_nombre_id_usuario_gestion, mensaje " & ex.Message
        End Try
    End Function
    Function SolicitaIdUsuarioDocuarchiRelacionadoUsuarioGestion(ByVal id_usuario_gestion As Integer, ByRef id_usuario_docuarchi As Integer) As String
        Try
            Dim Parametro_Consulta As String = "select RELACION_DA " &
                                       "from remit_dest_interno where id_Remit_Dest_Int=" & id_usuario_gestion
            Dim Ref_Car_Conec33 As New conect.Dbase_Conction_Mysql_RA
            Dim datset As DataSet = New DataSet("remit_dest_interno")
            Dim Result As String = ""
            Result = Ref_Car_Conec33.SELECTION_SELECT_FIELD(Parametro_Consulta, datset)
            If Result <> "YES" Then
                SolicitaIdUsuarioDocuarchiRelacionadoUsuarioGestion = "Función SolicitaIdUsuarioGestionRelacionadoUsuarioWorkflow dice " & Result
                Exit Function
            End If
            If datset.Tables(0).Rows.Count = 0 Then
                SolicitaIdUsuarioDocuarchiRelacionadoUsuarioGestion = "Imposible encontrar la relacion entre el usuario de gestion de radicacion y el usuario docuarchi " & vbCrLf &
                "Por favor cree en modulo de administración SIC el usuario de gestión y relacionelo al usuario docuarchi"
                Exit Function
            Else
                id_usuario_docuarchi = datset.Tables(0).Rows(0).Item(0)
                SolicitaIdUsuarioDocuarchiRelacionadoUsuarioGestion = "YES"
                Exit Function
            End If

        Catch ex As Exception
            SolicitaIdUsuarioDocuarchiRelacionadoUsuarioGestion = "Inconsistencia General Funcion : SolicitaIdUsuarioDocuarchiRelacionadoUsuarioGestion, mensaje " & ex.Message
        End Try
    End Function
    'Function SolicitaIdUsuarioGestionRelacionadoUsuarioWorkflow(ByVal Id_Usuario_Wf As Integer, ByRef id_user_gestor As Integer) As String
    '    Try
    '        Dim Parametro_Consulta As String = "select id_Remit_Dest_Int " & _
    '                                   "from remit_dest_interno where Relacion_Workflow=" & Id_Usuario_Wf
    '        Dim Ref_Car_Conec33 As New conect.Dbase_Conction_Mysql_RA
    '        Dim datset As DataSet = New DataSet("remit_dest_interno")
    '        Dim Result As String = ""
    '        Result = Ref_Car_Conec33.SELECTION_SELECT_FIELD(Parametro_Consulta, datset)
    '        If Result <> "YES" Then
    '            SolicitaIdUsuarioGestionRelacionadoUsuarioWorkflow = "Función SolicitaIdUsuarioGestionRelacionadoUsuarioWorkflow dice " & Result
    '            Exit Function
    '        End If
    '        If datset.Tables(0).Rows.Count = 0 Then
    '            SolicitaIdUsuarioGestionRelacionadoUsuarioWorkflow = "Imposible encontrar la relacion entre el usuario workflow y el usuario gestor " & vbCrLf & _
    '            "Por favor cree en modulo de administración SIC el usuario de gestión y relacionelo al usuario workflow"
    '            Exit Function
    '        Else
    '            id_user_gestor = datset.Tables(0).Rows(0).Item(0)
    '            SolicitaIdUsuarioGestionRelacionadoUsuarioWorkflow = "YES"
    '            Exit Function
    '        End If

    '    Catch ex As Exception
    '        SolicitaIdUsuarioGestionRelacionadoUsuarioWorkflow = "Inconsistencia General Funcion : SolicitaIdUsuarioGestionRelacionadoUsuarioWorkflow, mensaje " & ex.Message
    '    End Try
    'End Function
    Function SolicitaIdUsuarioGestionRelacionadoUsuarioWorkflow(ByVal Id_Usuario_Wf As Integer, ByRef id_user_gestor As Integer) As String
        Try
            Dim Parametro_Consulta As String = "select Relacion_Gestion " &
                                       "from usuario_workflow where idU_suario=" & Id_Usuario_Wf
            Dim Ref_Car_Conec33 As New conect.Dbase_Conction_Mysql
            Dim datset As DataSet = New DataSet("usuario_workflow")
            Dim Result As String = ""
            Result = Ref_Car_Conec33.SELECTION_SELECT_FIELD(Parametro_Consulta, datset)
            If Result <> "YES" Then
                SolicitaIdUsuarioGestionRelacionadoUsuarioWorkflow = "Función SolicitaIdUsuarioGestionRelacionadoUsuarioWorkflow dice " & Result
                Exit Function
            End If
            If datset.Tables(0).Rows.Count = 0 Then
                SolicitaIdUsuarioGestionRelacionadoUsuarioWorkflow = "Imposible encontrar la relacion entre el usuario workflow y el usuario gestor " & vbCrLf &
                "Por favor cree en modulo de administración SIC el usuario de gestión y relacionelo al usuario workflow"
                Exit Function
            Else
                id_user_gestor = datset.Tables(0).Rows(0).Item(0)
                SolicitaIdUsuarioGestionRelacionadoUsuarioWorkflow = "YES"
                Exit Function
            End If

        Catch ex As Exception
            SolicitaIdUsuarioGestionRelacionadoUsuarioWorkflow = "Inconsistencia General Funcion : SolicitaIdUsuarioGestionRelacionadoUsuarioWorkflow, mensaje " & ex.Message
        End Try
    End Function
    Function SolicitaIdUsuarioGestionRelacionUsuarioDocuarchi(ByVal IdUsuarioDocuarchi As Integer,
                                                              ByRef IdUsuarioGestion As Integer) As String
        Try
            Dim Parametro_Consulta As String = "select id_Remit_Dest_Int " &
                                       "from remit_dest_interno where RELACION_DA=" & IdUsuarioDocuarchi
            Dim Ref_Car_Conec33 As New conect.Dbase_Conction_Mysql_RA
            Dim datset As DataSet = New DataSet("remit_dest_interno")
            Dim Result As String = ""
            Result = Ref_Car_Conec33.SELECTION_SELECT_FIELD(Parametro_Consulta, datset)
            If Result <> "YES" Then
                SolicitaIdUsuarioGestionRelacionUsuarioDocuarchi = "Función SolicitaIdUsuarioGestionRelacionUsuarioDocuarchi dice " & Result
                Exit Function
            End If
            If datset.Tables(0).Rows.Count = 0 Then
                SolicitaIdUsuarioGestionRelacionUsuarioDocuarchi = "Imposible encontrar la relacion entre el usuario docuarchi.net y el usuario gestor " & vbCrLf &
                "Por favor cree en modulo de administración SIC el usuario de gestión y relacionelo al usuario docuarchi.net"
                Exit Function
            Else
                IdUsuarioGestion = datset.Tables(0).Rows(0).Item(0)
                SolicitaIdUsuarioGestionRelacionUsuarioDocuarchi = "YES"
                Exit Function
            End If

        Catch ex As Exception
            SolicitaIdUsuarioGestionRelacionUsuarioDocuarchi = "Inconsistencia General Funcion : SolicitaIdUsuarioGestionRelacionUsuarioDocuarchi, mensaje " & ex.Message
        End Try
    End Function
    Function SolicitaIdUsuarioRadicadorRelacionadoUsuarioGestion(ByVal id_usuario_gestion As Integer, ByRef id_usuario_radicacion As Integer) As String
        '********************************************************************************************
        'Funcion : Retorna relación usuario gestion usuario de radicador
        'Fecha 2016-07-07
        'Ing Miguel Angel Urueta Miranda
        '********************************************************************************************
        Try
            Dim Parametro_Consulta As String = "select RELACION_ID_USUARIO_RADICACION " &
                                       "from remit_dest_interno where id_Remit_Dest_Int=" & id_usuario_gestion
            Dim Ref_Car_Conec33 As New conect.Dbase_Conction_Mysql_RA
            Dim datset As DataSet = New DataSet("remit_dest_interno")
            Dim Result As String = ""
            Result = Ref_Car_Conec33.SELECTION_SELECT_FIELD(Parametro_Consulta, datset)
            If Result <> "YES" Then
                SolicitaIdUsuarioRadicadorRelacionadoUsuarioGestion = "Función SolicitaIdUsuarioRadicadorRelacionadoUsuarioGestion dice " & Result
                Exit Function
            End If
            If datset.Tables(0).Rows.Count = 0 Then
                SolicitaIdUsuarioRadicadorRelacionadoUsuarioGestion = "Imposible encontrar la relacion entre el usuario workflow y el usuario gestor " & vbCrLf &
                "Por favor cree en modulo de administración SIC el usuario de gestión y relacionelo al usuario workflow"
                Exit Function
            Else
                id_usuario_radicacion = datset.Tables(0).Rows(0).Item(0)
                SolicitaIdUsuarioRadicadorRelacionadoUsuarioGestion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaIdUsuarioRadicadorRelacionadoUsuarioGestion = "Inconsistencia General Funcion : SolicitaIdUsuarioRadicadorRelacionadoUsuarioGestion, mensaje " & ex.Message
        End Try
    End Function

    Function Retorna_Caracterizacion_Usuario_Gestion(ByVal Id_Usuario_Remit As Integer, ByRef Nombre_u As String, _
    ByRef Cargo_u As String, ByRef nombre_area As String) As String
        '********************************************************************************************
        'Funcion : Retorna datos de cataracterizacion usuario de gestión con la relación de workfow
        'Fecha 2015-06-19
        'Ing Miguel Angel Urueta Miranda
        '********************************************************************************************
        Try
            Dim Parametro_Consulta As String = "select rdi.Nombre_Remitente,rdi.Cargo_Remite,adr.Nombre_Area " & _
                                       "from remit_dest_interno as rdi " & _
                                       " inner join areas_depart_radicacion as adr on  (adr.Codigo_Area=rdi.Areas_Dep_Radicacion_id_Areas_Dep)" & _
                                       " where id_Remit_Dest_Int=" & Id_Usuario_Remit
            Dim Ref_Car_Conec33 As New conect.Dbase_Conction_Mysql_RA
            Dim datset As DataSet = New DataSet("remit_dest_interno")
            Dim Result As String = ""
            Result = Ref_Car_Conec33.SELECTION_SELECT_FIELD(Parametro_Consulta, datset)
            If Result <> "YES" Then
                Retorna_Caracterizacion_Usuario_Gestion = "Función Retorna_Caracterizacion_Usuario_Gestion dice " & Result
                Exit Function
            End If
            If datset.Tables(0).Rows.Count = 0 Then
                Retorna_Caracterizacion_Usuario_Gestion = "Imposible encontrar la relacion entre el usuario workflow y el usuario gestor " & vbCrLf & _
                "Por favor cree en modulo de administración SIC el usuario de gestión y relacionelo al usuario workflow"
                Exit Function
            Else
                Nombre_u = datset.Tables(0).Rows(0).Item(0)
                Cargo_u = datset.Tables(0).Rows(0).Item(1)
                If datset.Tables(0).Rows(0).IsNull(2) = True Then
                    nombre_area = ""
                Else
                    nombre_area = datset.Tables(0).Rows(0).Item(2)
                End If
                Retorna_Caracterizacion_Usuario_Gestion = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Retorna_Caracterizacion_Usuario_Gestion = "Inconsistencia General Funcion : Retorna_Caracterizacion_Usuario_Gestion, mensaje " & ex.Message
        End Try
    End Function
    Function SolicitaLoginUsuarioGestion(ByVal IdUsuarioGestion As Integer,
                                         ByRef LoginUsuarioGestion As String) As String
        '********************************************************************************************
        'Funcion : Retorna login usuario de gestion
        'Fecha 2016-07-20
        'Ing Miguel Angel Urueta Miranda
        '********************************************************************************************
        Try
            Dim Parametro_Consulta As String = "select Login_Usuario " &
                                       "from remit_dest_interno where id_Remit_Dest_Int=" & IdUsuarioGestion
            Dim Ref_Car_Conec33 As New conect.Dbase_Conction_Mysql_RA
            Dim datset As DataSet = New DataSet("remit_dest_interno")
            Dim Result As String = ""
            Result = Ref_Car_Conec33.SELECTION_SELECT_FIELD(Parametro_Consulta, datset)
            If Result <> "YES" Then
                SolicitaLoginUsuarioGestion = "Función SolicitaLoginUsuarioGestion dice " & Result
                Exit Function
            End If
            If datset.Tables(0).Rows.Count = 0 Then
                SolicitaLoginUsuarioGestion = "Imposible encontrar el login del usuario de gestion " & vbCrLf &
                "Por favor cree en modulo de administración SIC el usuario de gestión y relacionelo al usuario workflow, docuarchi y radicación"
                Exit Function
            Else
                LoginUsuarioGestion = datset.Tables(0).Rows(0).Item(0)
                SolicitaLoginUsuarioGestion = "YES"
                Exit Function
            End If

        Catch ex As Exception
            SolicitaLoginUsuarioGestion = "Inconsistencia General Funcion : SolicitaLoginUsuarioGestion, mensaje " & ex.Message
        End Try
    End Function
End Class
