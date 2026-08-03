Public Structure stru_configuracion_listado_ruta_valor_campo
    Dim nombre_campo_ruta As String
    Dim valor_campo_ruta As String
    Dim valor_campo_tramite As String
    Dim ERROR_SERVICE As String
End Structure
Public Class Class_configuracion_listado_ruta_valor_campo_
    Private m_nombre_campo_ruta As String
    Public Property nombre_campo_ruta() As String
        Get
            Return m_nombre_campo_ruta
        End Get
        Set(value As String)
            m_nombre_campo_ruta = value
        End Set
    End Property
    Private m_valor_campo_ruta As String
    Public Property valor_campo_ruta() As String
        Get
            Return m_valor_campo_ruta
        End Get
        Set(value As String)
            m_valor_campo_ruta = value
        End Set
    End Property
    Private m_valor_campo_tramite As String
    Public Property valor_campo_tramite() As String
        Get
            Return m_valor_campo_tramite
        End Get
        Set(value As String)
            m_valor_campo_tramite = value
        End Set
    End Property
    Private m_ERROR_SERVICE As String
    Public Property ERROR_SERVICE() As String
        Get
            Return m_ERROR_SERVICE
        End Get
        Set(value As String)
            m_ERROR_SERVICE = value
        End Set
    End Property
End Class
Public Class Class_configuracion_listado_ruta
    Function Solicita_valor_nombre_campo_radicado_beneficiario(ByVal id_tarea As Long,
                                                               ByVal nombre_ruta As String,
                                                               ByVal campo_radicado As String,
                                                               ByVal campo_beneficiario As String,
                                                               ByVal campo_tramite As String,
                                                               ByRef Class_configuracion_listado_ruta_valor_campo_() As Class_configuracion_listado_ruta_valor_campo_) As String
        '-----------------------------------------------------------
        'Fucion : Retorna el valor del campo beneficiario de la ruta
        'y del campo rdicado.
        'Fecha : 2022-03-26
        'Ing .Miguel Angel Urueta Miranda
        '-----------------------------------------------------------
        Try
            Dim sql_consulta As String = "Select " & campo_radicado & "," & campo_beneficiario & "," & campo_tramite & " from dat_adic_tar" & nombre_ruta & " where INICIO_TAREAS_WORKFLOW_ID_TAREA=" & id_tarea
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet(nombre_ruta)
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_valor_nombre_campo_radicado_beneficiario = "Error función Solicita_valor_nombre_campo_radicado_beneficiario " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_valor_nombre_campo_radicado_beneficiario = "Imposible encontrar el beneficiario y el radicado de la tarea (" & id_tarea & ")"
                Exit Function
            Else
                Dim ref_Class_configuracion_listado_ruta_valor_campo_ As New Class_configuracion_listado_ruta_valor_campo_
                If Datset.Tables(0).Rows(0).IsNull(0) Then
                    ref_Class_configuracion_listado_ruta_valor_campo_.ERROR_SERVICE = "YES"
                    ref_Class_configuracion_listado_ruta_valor_campo_.valor_campo_ruta = ""
                    ref_Class_configuracion_listado_ruta_valor_campo_.nombre_campo_ruta = campo_radicado
                Else
                    ref_Class_configuracion_listado_ruta_valor_campo_.ERROR_SERVICE = "YES"
                    ref_Class_configuracion_listado_ruta_valor_campo_.valor_campo_ruta = Datset.Tables(0).Rows(0).Item(0)
                    ref_Class_configuracion_listado_ruta_valor_campo_.nombre_campo_ruta = campo_radicado
                End If
                ReDim Preserve Class_configuracion_listado_ruta_valor_campo_(0)
                Class_configuracion_listado_ruta_valor_campo_(0) = ref_Class_configuracion_listado_ruta_valor_campo_
                ref_Class_configuracion_listado_ruta_valor_campo_ = New Class_configuracion_listado_ruta_valor_campo_
                If Datset.Tables(0).Rows(0).IsNull(1) Then
                    ref_Class_configuracion_listado_ruta_valor_campo_.ERROR_SERVICE = "YES"
                    ref_Class_configuracion_listado_ruta_valor_campo_.valor_campo_ruta = ""
                    ref_Class_configuracion_listado_ruta_valor_campo_.nombre_campo_ruta = campo_beneficiario
                Else
                    ref_Class_configuracion_listado_ruta_valor_campo_.ERROR_SERVICE = "YES"
                    ref_Class_configuracion_listado_ruta_valor_campo_.valor_campo_ruta = Datset.Tables(0).Rows(0).Item(1)
                    ref_Class_configuracion_listado_ruta_valor_campo_.nombre_campo_ruta = campo_beneficiario
                End If
                ReDim Preserve Class_configuracion_listado_ruta_valor_campo_(1)
                Class_configuracion_listado_ruta_valor_campo_(1) = ref_Class_configuracion_listado_ruta_valor_campo_
                ref_Class_configuracion_listado_ruta_valor_campo_ = New Class_configuracion_listado_ruta_valor_campo_
                If Datset.Tables(0).Rows(0).IsNull(2) Then
                    ref_Class_configuracion_listado_ruta_valor_campo_.ERROR_SERVICE = "YES"
                    ref_Class_configuracion_listado_ruta_valor_campo_.valor_campo_ruta = ""
                    ref_Class_configuracion_listado_ruta_valor_campo_.nombre_campo_ruta = campo_tramite
                Else
                    ref_Class_configuracion_listado_ruta_valor_campo_.ERROR_SERVICE = "YES"
                    ref_Class_configuracion_listado_ruta_valor_campo_.valor_campo_ruta = Datset.Tables(0).Rows(0).Item(2)
                    ref_Class_configuracion_listado_ruta_valor_campo_.nombre_campo_ruta = campo_tramite
                End If
                ReDim Preserve Class_configuracion_listado_ruta_valor_campo_(2)
                Class_configuracion_listado_ruta_valor_campo_(2) = ref_Class_configuracion_listado_ruta_valor_campo_
                Solicita_valor_nombre_campo_radicado_beneficiario = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_valor_nombre_campo_radicado_beneficiario = "Inconsistencia general función Solicita_valor_nombre_campo_radicado_beneficiario " & ex.Message
        End Try
    End Function
    Function SolicitaNombreCampoRadicadoRuta(ByVal IdRutaWorkflow As Integer,
                                             ByRef NombreCampoRadicado As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita nombre campo radicado ruta workflow
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdRutaWorkflow      : Representa la identificacion de la ruta workflow
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'NombreCampoRadicado   : Retorna el nombre del campo radicado
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-01
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim sqlconsulta As String = "Select Nombre_Campo from configuracion_listado_ruta where campo_radicado=1 and Rutas_Workflow_id_Ruta=" & IdRutaWorkflow
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("configuracion_listado_ruta")
            Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                SolicitaNombreCampoRadicadoRuta = "Error función NombreCampoRadicado " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaNombreCampoRadicadoRuta = "Imposible encontrar el campo que actúa como consecutivo radicado en la ruta, contacte a su administrador"
                Exit Function
            Else
                NombreCampoRadicado = Datset.Tables(0).Rows(0).Item(0)
                SolicitaNombreCampoRadicadoRuta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaNombreCampoRadicadoRuta = "Inconsistencia general función SolicitaNombreCampoRadicadoRuta " & ex.Message
        End Try
    End Function
    Function SolicitaNombreCampoCodigoBarrasRuta(ByVal IdRutaWorkflow As Integer,
                                                 ByVal ConfirmaExistencia As Integer,
                                                 ByRef NombreCampoCodigoBarras As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita campo nombre campo codigo brras SII
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdRutaWorkflow      : Representa la identificacion de la ruta workflow
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'NombreCampoCodigoBarras   : Retorna la idnetificación del usuario radicador
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-01
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim sqlconsulta As String = "Select Nombre_Campo from configuracion_listado_ruta where campo_codigo_barras=1 and Rutas_Workflow_id_Ruta=" & IdRutaWorkflow
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("configuracion_listado_ruta")
            Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                SolicitaNombreCampoCodigoBarrasRuta = "Error función SolicitaNombreCampoCodigoBarrasRuta " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                NombreCampoCodigoBarras = ""
                If ConfirmaExistencia = 1 Then
                    SolicitaNombreCampoCodigoBarrasRuta = "No fue posible encontrar el campo que actúa como consecutivo para el código de barras en la ruta especificada. Por favor, contacte al administrador del sistema."
                    Exit Function
                Else
                    SolicitaNombreCampoCodigoBarrasRuta = "YES"
                    Exit Function
                End If
            Else
                NombreCampoCodigoBarras = Datset.Tables(0).Rows(0).Item(0)
                SolicitaNombreCampoCodigoBarrasRuta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaNombreCampoCodigoBarrasRuta = "Inconsistencia general función SolicitaNombreCampoCodigoBarrasRuta " & ex.Message
        End Try
    End Function
    Function SolicitaNombreCampoTramiteRuta(ByVal id_ruta As Integer,
                                            ByRef nombre_campo_tramite As String) As String
        Try
            Dim Result As String = ""
            Dim sqlconsulta As String = "Select Nombre_Campo from configuracion_listado_ruta where campo_tramite=1 and Rutas_Workflow_id_Ruta=" & id_ruta
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("configuracion_listado_ruta")
            Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                SolicitaNombreCampoTramiteRuta = "Error funcion Retorna_nombre_campo_tramite_ruta " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaNombreCampoTramiteRuta = "Imposible encontrar el campo que actúa como registro del trámite en la ruta, contacte a su administrador"
                Exit Function
            Else
                nombre_campo_tramite = Datset.Tables(0).Rows(0).Item(0)
                SolicitaNombreCampoTramiteRuta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaNombreCampoTramiteRuta = "Inconsistencia general función SolicitaNombreCampoTramiteRuta " & ex.Message
        End Try
    End Function
    Function SolicitaNombreCampoBenificiarioRuta(ByVal IdRuta As Integer,
                                                 ByRef NombreCampoBeneficiario As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita el nombre del campo beneficiario en la ruta
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdRuta              : Representa la identificación de la ruta
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'NombreCampoBeneficiario  : Retorna el nombre del campo benefciario
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha Modifica        : 2025-05-29
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim sqlconsulta As String = "Select Nombre_Campo from configuracion_listado_ruta where Campo_beneficiario=1 and Rutas_Workflow_id_Ruta=" & IdRuta
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("configuracion_listado_ruta")
            Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                SolicitaNombreCampoBenificiarioRuta = "Error funcion SolicitaNombreCampoBenificiarioRuta " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaNombreCampoBenificiarioRuta = "YES"
                Exit Function
            Else
                NombreCampoBeneficiario = Datset.Tables(0).Rows(0).Item(0)
                SolicitaNombreCampoBenificiarioRuta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaNombreCampoBenificiarioRuta = "Inconsistencia general función SolicitaNombreCampoBenificiarioRuta " & ex.Message
        End Try
    End Function
    Function Solicita_campos_lista_tramite(ByVal id_ruta_workflow As Integer,
                                           ByRef campos_lista As String) As String
        Try
            campos_lista = ""
            Dim Result As String = ""
            Dim campo_radicado As String = ""
            Dim campo_tramite As String = ""
            Dim campo_fecha_vence As String = ""
            Dim Campo_beneficiario As String = ""
            Dim sqlconsulta As String = "Select Nombre_Campo   " &
                                        " from configuracion_listado_ruta  where  Rutas_Workflow_id_Ruta=" & id_ruta_workflow &
                                        " and Lista_gestion_tamite=1 Order by Orden_lista_gestion_tamite "
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("configuracion_listado_ruta")
            Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                Solicita_campos_lista_tramite = "Error funcion Solicita_campos_lista_tramite " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_campos_lista_tramite = "No hay campos activos para la lista de tramites pendientes, contacte a su adminstrador"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    If campos_lista = "" Then
                        campos_lista = campos_lista & Datset.Tables(0).Rows(i).Item(0)
                    Else
                        campos_lista = campos_lista & "," & Datset.Tables(0).Rows(i).Item(0)
                    End If
                Next
                Solicita_campos_lista_tramite = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_campos_lista_tramite = "Inconsistencia Solicita_campos_lista_tramite " & ex.Message
        End Try
    End Function
    Function Solicita_campos_lista_workflow(ByVal id_ruta_workflow As Integer,
                                            ByRef campos_lista As String) As String
        Try
            campos_lista = ""
            Dim Result As String = ""
            Dim campo_radicado As String = ""
            Dim campo_tramite As String = ""
            Dim campo_fecha_vence As String = ""
            Dim Campo_beneficiario As String = ""
            Dim sqlconsulta As String = "Select Nombre_Campo   " &
                                        " from configuracion_listado_ruta  where  Rutas_Workflow_id_Ruta=" & id_ruta_workflow &
                                        " AND LISTA_TAREA=1 Order by id_campo "
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("configuracion_listado_ruta")
            Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                Solicita_campos_lista_workflow = "Error funcion Solicita_campos_lista_workflow " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_campos_lista_workflow = "No hay campos activos para la lista de tareas workflow, contacte a su adminstrador"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    If campos_lista = "" Then
                        campos_lista = campos_lista & Datset.Tables(0).Rows(i).Item(0)
                    Else
                        campos_lista = campos_lista & "," & Datset.Tables(0).Rows(i).Item(0)
                    End If

                Next
                Solicita_campos_lista_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_campos_lista_workflow = "Inconsistencia Solicita_campos_lista_tramite " & ex.Message
        End Try
    End Function
    Function Lista_campos_ruta_documento(ByRef matri_campos_ruta() As String) As String
        '*******************************************************************
        'Funcion : Lista los campos de la ruta seleccionada por el usuario
        'con el parametro fijo id ruta
        'Ingeniero : Miguel Angel Urueta Miranda
        'Fecha 2014-10-31
        '*******************************************************************
        Try
            Dim Sql_consulta As String = "Select NOMBRE_CAMPO from " &
                    " CONFIGURACION_LISTADO_RUTA WHERE RUTAS_WORKFLOW_ID_RUTA =" & HttpContext.Current.Session("Id_Ruta_Workflow") &
                    " AND LISTA_TAREA=1"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("CONFIGURACION_LISTADO_RUTA")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then

                Lista_campos_ruta_documento = " Error # 01 Imposible Encontrar campos en la tabla CONFIGURACION_LISTADO_RUTA" & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then

                Lista_campos_ruta_documento = " # 01 La tabla CONFIGURACION_LISTADO_RUTA no tiene campos de consulta"
                Exit Function
            Else
                Erase matri_campos_ruta
                Dim i As Integer = 0
                For i = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve matri_campos_ruta(i)
                    matri_campos_ruta(i) = Datset.Tables(0).Rows(i).Item(0).ToString

                Next
                Lista_campos_ruta_documento = "YES"
            End If
        Catch ex As Exception
            Lista_campos_ruta_documento = "Inconsistencia general funcion Lista_campos_ruta_documento " & ex.Message
        End Try
    End Function
End Class
