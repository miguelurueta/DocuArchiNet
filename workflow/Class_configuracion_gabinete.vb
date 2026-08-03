Public Structure structure_gabinete_workflow
    Dim NOMBRE_GABINETE As String
    Dim RUTA_BUSQUEDA_IMAGEN As String
    Dim BASE_DATOS As String
    Dim MOTOR_BASE As String
    Dim ODBC_BASE As String
    Dim USUARIO_BASE As String
    Dim PASWORD_BASE As String
End Structure
Public Class Class_configuracion_gabinete
    Function SolicitaIdGabineteWorkflowPorNombre(ByVal NombreGabineteWorkflow As String,
                                                 ByRef IdGabineteWorkflow As Integer) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita identifcación del gabinete workflow con el nombre del gabinete
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'NombreGabineteWorkflow  : Representa el nombre del gabinete workflow
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'IdGabineteWorkflow      : Retorna la identificación del gabinete workflow
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-01
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim Result As String = ""
            Dim sqlconsulta As String = "Select id_Gabinete from configuracion_gabinete where Nombre_Gabinete='" & NombreGabineteWorkflow & "'"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("configuracion_gabinete")
            Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                SolicitaIdGabineteWorkflowPorNombre = "Error función SolicitaIdGabineteWorkflowPorNombre " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaIdGabineteWorkflowPorNombre = "Imposible encontrar el id del gabinete workflow con el nombre (" & NombreGabineteWorkflow & ") "
                Exit Function
            Else
                IdGabineteWorkflow = Datset.Tables(0).Rows(0).Item(0)
                SolicitaIdGabineteWorkflowPorNombre = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaIdGabineteWorkflowPorNombre = "Inconsistencia general función SolicitaIdGabineteWorkflowPorNombre " & ex.Message
        End Try
    End Function
    Function SolicitanombreGabineteWorkflow(ByVal IdGabineteWorkflow As Integer,
                                            ByRef NombreGabineteWorkflow As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita nombre campo gabinete workflow
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdGabineteWorkflow  : Representa la identificación del gabinete en la ruta workflow
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'NombreGabineteWorkflow  : Retorna el nombre del gabinete
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-01
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim sqlconsulta As String = "Select Nombre_Gabinete from configuracion_gabinete where id_Gabinete=" & IdGabineteWorkflow
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("configuracion_gabinete")
            Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                SolicitanombreGabineteWorkflow = "Error función SolicitanombreGabineteWorkflow " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitanombreGabineteWorkflow = "Imposible encontrar el nombre del gabinete con la identificación (" & IdGabineteWorkflow & ") "
                Exit Function
            Else
                NombreGabineteWorkflow = Datset.Tables(0).Rows(0).Item(0)
                SolicitanombreGabineteWorkflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitanombreGabineteWorkflow = "Inconsistencia general función SolicitanombreGabineteWorkflow " & ex.Message
        End Try
    End Function
    Function Solicita_nombre_gabinete(ByVal codigo_gabinete As String,
                                      ByRef nombre_gabinete As String) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("configuracion_gabinete")
            Dim Sql_consulta As String = "Select Nombre_Gabinete from configuracion_gabinete " &
                " where id_Gabinete=" & Val(codigo_gabinete)
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_nombre_gabinete = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_nombre_gabinete = "Imposible encontrar el nombre del gabinete workflow con el código  " & codigo_gabinete
                Exit Function
            Else
                nombre_gabinete = Datset.Tables(0).Rows(0).Item(0)
                Solicita_nombre_gabinete = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_nombre_gabinete = "Inconsistencia general función SolicitaNombreGabinetePorId " & ex.Message
        End Try
    End Function
    Function SolicitaDatosEstructuraGabineteWorkflow(ByVal IdGabineteWorkflow As Integer,
                                                     ByRef structure_gabinete_workflow As structure_gabinete_workflow) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solcitud de estructura de gabinete workflow con la identificaicón del gabinete
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdGabineteWorkflow  : Representa la identificación del gabinete workflow
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'structure_gabinete_workflow  : Retorna la estructura del gabinete workflow
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-06-03
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT NOMBRE_GABINETE,RUTA_BUSQUEDA_IMAGEN " &
            ",BASE_DATOS,MOTOR_BASE,ODBC_BASE,USUARIO_BASE,PASWORD_BASE" &
            " FROM CONFIGURACION_GABINETE" &
            " WHERE ID_GABINETE=" & IdGabineteWorkflow
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("CONFIGURACION_GABINETE")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                SolicitaDatosEstructuraGabineteWorkflow = "Funcion SolicitaDatosEstructuraGabineteWorkflow dice :  (" & Result & ")"
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaDatosEstructuraGabineteWorkflow = "Imposible encontrar la configuración  del gabiente  (" & IdGabineteWorkflow & ") en la configuración de gabinetes workflow"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    structure_gabinete_workflow.NOMBRE_GABINETE = ""
                Else
                    structure_gabinete_workflow.NOMBRE_GABINETE = Datset.Tables(0).Rows(0).Item(0)
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) = True Then
                    structure_gabinete_workflow.RUTA_BUSQUEDA_IMAGEN = ""
                Else
                    structure_gabinete_workflow.RUTA_BUSQUEDA_IMAGEN = Datset.Tables(0).Rows(0).Item(1).ToString.Replace("/", "\")
                End If
                If Datset.Tables(0).Rows(0).IsNull(2) = True Then
                    structure_gabinete_workflow.BASE_DATOS = ""
                Else
                    structure_gabinete_workflow.BASE_DATOS = Datset.Tables(0).Rows(0).Item(2)
                End If
                If Datset.Tables(0).Rows(0).IsNull(3) = True Then
                    structure_gabinete_workflow.MOTOR_BASE = ""
                Else
                    structure_gabinete_workflow.MOTOR_BASE = Datset.Tables(0).Rows(0).Item(3)
                End If
                If Datset.Tables(0).Rows(0).IsNull(4) = True Then
                    structure_gabinete_workflow.ODBC_BASE = ""
                Else
                    structure_gabinete_workflow.ODBC_BASE = Datset.Tables(0).Rows(0).Item(4)
                End If
                If Datset.Tables(0).Rows(0).IsNull(5) = True Then
                    structure_gabinete_workflow.USUARIO_BASE = ""
                Else
                    structure_gabinete_workflow.USUARIO_BASE = Datset.Tables(0).Rows(0).Item(5)
                End If
                If Datset.Tables(0).Rows(0).IsNull(6) = True Then
                    structure_gabinete_workflow.PASWORD_BASE = ""
                Else
                    structure_gabinete_workflow.PASWORD_BASE = Datset.Tables(0).Rows(0).Item(6)
                End If
                SolicitaDatosEstructuraGabineteWorkflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaDatosEstructuraGabineteWorkflow = "Inconsistencia general funcion SolicitaDatosEstructuraGabineteWorkflow " & ex.Message
        End Try
    End Function
    Function Solicita_datos_ruta_workflow_gabinete_SII(ByVal Datos_Recibo As String,
                                                       ByVal Nombre_Ruta As String,
                                                       ByRef Codigo_Barras As String,
                                                       ByRef Nombre_Gabinete As String,
                                                       ByRef Id_Tarea As Long,
                                                       ByRef Id_gabinete As Integer,
                                                       ByRef secuencia As Integer) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita datos detalle de la ruta de la tabla dat_adic_tar_ y configuración de
        'gabinete
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'Datos_Recibo           : Representa la identificación del recibo SII
        'Nombre_Ruta            : Representa el nombre de la ruta
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'Codigo_Barras   : Retorna el codigo de barras integración SII
        'Nombre_Gabinete : Retorna el nombre del gabinete
        'Id_Tarea        : Representa la identificación de la tarea
        'Id_gabinete     : Representa la identificación del gabinete
        'secuencia       : Representa la identificación de la secuencia de dpocumentos SII
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2023-04-01
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim Sql_Consulta As String = ""
            Sql_Consulta = "SELECT CG.NOMBRE_GABINETE,dat.CODIGO_BARRAS, dat.INICIO_TAREAS_WORKFLOW_ID_TAREA,dat.ID_GABINETE,dat.SECUENCIAC FROM dat_adic_tar" & Nombre_Ruta &
                 " dat INNER JOIN configuracion_gabinete cg on (cg.id_Gabinete=dat.ID_GABINETE) " &
                 " WHERE DATOS_RECIBO ='" & Datos_Recibo & "'"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("estados_tarea_workflow")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Sql_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_datos_ruta_workflow_gabinete_SII = "Error funcion Solicita_datos_ruta_workflow_gabinete_SII " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_datos_ruta_workflow_gabinete_SII = "Imposible contrar los datos del recibo (" & Datos_Recibo & ") en la tabla (dat_adic_tar_" & Nombre_Ruta & ")"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = False Then
                    Nombre_Gabinete = Datset.Tables(0).Rows(0).Item(0)
                Else
                    Nombre_Gabinete = ""
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) = False Then
                    Codigo_Barras = Datset.Tables(0).Rows(0).Item(1)
                Else
                    Codigo_Barras = ""
                End If
                If Datset.Tables(0).Rows(0).IsNull(2) = False Then
                    Id_Tarea = Datset.Tables(0).Rows(0).Item(2)
                Else
                    Id_Tarea = 0
                End If

                If Datset.Tables(0).Rows(0).IsNull(3) = False Then
                    Id_gabinete = Datset.Tables(0).Rows(0).Item(3)
                Else
                    Id_gabinete = 0
                End If
                If Datset.Tables(0).Rows(0).IsNull(4) = False Then
                    secuencia = Datset.Tables(0).Rows(0).Item(4)
                Else
                    secuencia = 0
                End If
                Solicita_datos_ruta_workflow_gabinete_SII = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_datos_ruta_workflow_gabinete_SII = "Inconistencia general función Solicita_datos_ruta_workflow_gabinete_SII " & ex.Message
        End Try
    End Function
End Class
