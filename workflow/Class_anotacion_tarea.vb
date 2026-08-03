Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.OleDb
Imports System.Data.Odbc
Imports MySql.Data.MySqlClient
Public Class class_detail_note
    Public id_anotacion As Integer
    Public fecha_anotacion As String
    Public nombre_usuario As String
    Public loguin_usuario As String
    Public cargo_usuario As String
    Public dato_anotacion As String
    Public nombre_actividad As String
    Public result As String
    Public title_anotacion As String
End Class
Public Class Class_anotacion_tarea
    Function Solicta_nota_tarea(ByVal id_anotacion As Integer,
                                ByRef data_anotacion As String) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita el contenido de la nota
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_anotacion          : Representa el identificador de la nota
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        '
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha modificado      : 2023-07-04
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Sql_consulta As String = "select DATO_ANOTACION from  ANOTACION_TAREA WHERE ID_ANOTACION=" & id_anotacion
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ANOTACION_TAREA")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicta_nota_tarea = "Funcion Solicta_nota_tarea dice (" & Result & ")"
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicta_nota_tarea = "Imposible encontrar los datos de la nota (" & id_anotacion & ")"
                Exit Function
            Else
                data_anotacion = Datset.Tables(0).Rows(0).Item(0).ToString
                Solicta_nota_tarea = "YES"
            End If
        Catch ex As Exception
            Solicta_nota_tarea = "Inconsistencia general function Solicta_nota_tarea " & ex.Message
        End Try
    End Function
    Function Listar_Anotaciones_tarea_workflow(ByRef refgrid As GridView,
                                               ByVal id_tarea_workflow As Long) As String
        '---------------------------------------------------------------------------
        'Funcion : Lista anotaciones workflow
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_tarea_workflow     : Representa la identificación del tarea workflow
        'refgrid               : Representa el objeto table de vb
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        '
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha modificado      : 2023-07-01
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim pag As Page = refgrid.Page
            Dim updat As UpdatePanel = pag.FindControl("UpdatePanelanotacion")
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT AT.ID_ANOTACION,AT.FECHA_ANOTACION," &
            "UW.NOMBRE_USUARIO as USUARIO,UW.LOGIN_USUARIO AS LOGIN,UW.CARGO_USUARIO,AT.DATO_ANOTACION AS NOTA, Fecha_Anotacion as FECHA" &
            " FROM ANOTACION_TAREA AT " &
            "INNER JOIN USUARIO_WORKFLOW AS UW ON " &
            " (UW.IDU_SUARIO=AT.ID_USUARIO) " &
            "WHERE INICIO_TAREAS_WORKFLOW_ID_TAREA=" & id_tarea_workflow &
            " AND ESTADO_TAREA=1 ORDER BY AT.FECHA_ANOTACION DESC"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ANOTACION_TAREA")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Listar_Anotaciones_tarea_workflow = " Funcion Listar_Anotaciones_tarea_workflow  dice (" & Result & ")"
                Exit Function
            End If
            Dim Dat_set_zero As DataSet = New DataSet("ANOTACION_TAREA")
            Dat_set_zero.Tables.Add("cahce_estados_tarea_workflow_zero")
            For z As Integer = 0 To Datset.Tables(0).Columns.Count - 1
                Dat_set_zero.Tables(0).Columns.Add(Datset.Tables(0).Columns(z).ColumnName, Datset.Tables(0).Columns(z).DataType)
            Next
            If Datset.Tables(0).Rows.Count = 0 Then
                Dat_set_zero.Tables(0).Rows.Add(Dat_set_zero.Tables(0).NewRow)
                refgrid.DataSource = Dat_set_zero
                refgrid.DataBind()
                refgrid.Rows(0).Visible = False
                refgrid.DataBind()
                updat.Update()
                Listar_Anotaciones_tarea_workflow = "YES"
                Exit Function
            Else
                refgrid.DataSource = Datset
                refgrid.DataBind()
                updat.Update()
                For i As Integer = 0 To refgrid.Rows.Count - 1
                    refgrid.Rows(i).Attributes.Add("id", refgrid.Rows(i).Cells(1).Text.ToString())
                    Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                    Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fas fa-sticky-note")
                    ihtml.Style.Add("color", "white")
                    Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-success btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent_event(event,this);")
                    ahtml.Attributes.Add("title", "Nota")
                    ahtml.Attributes.Add("idd", refgrid.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "ver_nota")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)
                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "far fa-trash-alt")
                    ihtml.Style.Add("color", "white")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-danger btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent_event(event,this);")
                    ahtml.Attributes.Add("title", "Eliminar nota")
                    ahtml.Attributes.Add("idd", refgrid.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "eli_nota")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)
                    divhtml.Style.Add("display", "inline-flex")
                    refgrid.Rows(i).Cells(0).Controls.Add(divhtml)
                    For z As Integer = 0 To refgrid.Rows(i).Cells.Count - 1
                        If z > 0 Then
                            refgrid.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_cort_tr_flex")
                            refgrid.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(event,this);")
                        End If
                    Next
                Next
                Listar_Anotaciones_tarea_workflow = "YES"
                Exit Function
            End If
        Catch es As Exception
            Listar_Anotaciones_tarea_workflow = "Inconsistencia general funcion Listar_Anotaciones_tarea_workflow  " + es.Message
        End Try
    End Function

    Function Eliminar_nota_service_workflow(ByVal Dat_anotacion As String,
                                            ByVal Id_anotacion As Integer,
                                            ByVal id_tarea_workflow As Long,
                                            ByVal id_usuario_workflow As Integer) As String
        '---------------------------------------------------------------------------
        'Funcion : Elimina nota workflow
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'Dat_anotacion         : Representa el contenido de la nota
        'Id_anotacion          : Representa la identificacion de la anotacion
        'id_tarea_workflow     : Representa la identificación del tarea workflow
        'id_usuario_workflow   : Representa la identificacion del usuario workflow
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        '
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-07-01
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------

        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Dim myTrans As MySqlTransaction
        myTrans = myConnection.BeginTransaction()
        myCommand.Connection = myConnection
        myCommand.Transaction = myTrans
        Dim paramter_resp As Integer = 0
        Dim Result As String = ""
        Dim estado_propietario As String = ""
        Result = Solicita_estado_usuario_propietario_nota(id_usuario_workflow,
                                                          Id_anotacion,
                                                          estado_propietario)
        If Result <> "YES" Then
            Eliminar_nota_service_workflow = Result
            Exit Function
        End If
        If estado_propietario = "NO" Then
            Eliminar_nota_service_workflow = "El usuario no es el propietario de la nota imposible eliminar la nota"
            Exit Function
        End If
        '-----------------------------
        'Formatea framework actual
        '-----------------------------
        Dim Fecha_Fromat As String = ""
        Dim DateCreate As Date = Now
        Dim ClassGestionFechas As New ClassGestionFechas
        Result = ClassGestionFechas.Formatea_fecha_time_framework(DateCreate,
                                                                  Fecha_Fromat)
        If Result <> "YES" Then
            Eliminar_nota_service_workflow = Result
            Exit Function
        End If
        Dim Parametro_Update As String = "delete from  ANOTACION_TAREA" &
            " WHERE ID_ANOTACION=" & Id_anotacion & " AND " &
            " ID_USUARIO=" & id_usuario_workflow
        Dim Parametro_Insert As String = "INSERT INTO wf_log_workflow (usuario_workflow_idU_suario,fecha_hora,operacion,ID_TAREA_WORKFLOW," &
            "datos_operacion,opcion,descripcion_opcion,ip_transacion,id_operacion) values (" & id_usuario_workflow & ",'" & Fecha_Fromat & "','Elimina'," &
            id_tarea_workflow & ",'" & Dat_anotacion & "',1,'NOTA WORKFLOW','" &
            HttpContext.Current.Session.Item("ip_host_name") & "'," & Id_anotacion & ")"
        Try
            myCommand.CommandText = Parametro_Update
            paramter_resp = myCommand.ExecuteNonQuery()
            If paramter_resp = 0 Then
                myConnection.Close()
                Eliminar_nota_service_workflow = "Imposible eliminar la nota  " & Parametro_Update
                Exit Function
            End If
            myCommand.CommandText = Parametro_Insert
            paramter_resp = myCommand.ExecuteNonQuery()
            If paramter_resp = 0 Then
                myTrans.Rollback()
                myConnection.Close()
                Eliminar_nota_service_workflow = "Imposible registrar el log de anotación para la eliminación de la nota " & Parametro_Insert
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            Eliminar_nota_service_workflow = "YES"
            Exit Function
        Catch e As Exception
            Try
                myTrans.Rollback()
                myConnection.Close()
                Eliminar_nota_service_workflow = "Error eliminando  " & Parametro_Update &
                " Insertando " & Parametro_Insert
                Exit Function
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Eliminar_nota_service_workflow = "An exception of type " & ex.GetType().ToString() &
                                      " was encountered while attempting to roll back the transaction."
                    myConnection.Close()
                    Exit Function
                Else
                    Eliminar_nota_service_workflow = "Error Actualizando FUNCION Eliminar_nota_service_workflow " & ex.Message
                    Exit Function
                End If
            End Try
        End Try
    End Function
    Function Actualizar_datos_anotacion(ByVal Dat_anotacion As String,
                                        ByVal Id_anotacion As Integer,
                                        ByVal id_tarea_workflow As Long,
                                        ByVal id_usuario_workflow As Integer) As String
        '---------------------------------------------------------------------------
        'Funcion : Actualiza el contenido de la anotación
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'Dat_anotacion         : Representa el contenido de la nota
        'Id_anotacion          : Representa la identificacion de la anotacion
        'id_tarea_workflow     : Representa la identificación del tarea workflow
        'id_usuario_workflow   : Representa la identificacion del usuario workflow
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        '
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-07-01
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Dim myTrans As MySqlTransaction
        myTrans = myConnection.BeginTransaction()
        myCommand.Connection = myConnection
        myCommand.Transaction = myTrans
        Dim paramter_resp As Integer = 0
        Dim estado_propietario As String = ""
        Dim Result As String = ""
        Result = Solicita_estado_usuario_propietario_nota(id_usuario_workflow,
                                                          Id_anotacion,
                                                          estado_propietario)
        If Result <> "YES" Then
            Actualizar_datos_anotacion = Result
            Exit Function
        End If
        If estado_propietario = "NO" Then
            Actualizar_datos_anotacion = "El usuario no es el propietario de la nota imposible editar la nota"
            Exit Function
        End If
        '-----------------------------
        'Formatea framework actual
        '-----------------------------
        Dim Fecha_Fromat As String = ""
        Dim DateCreate As Date = Now
        Dim ClassGestionFechas As New ClassGestionFechas
        Result = ClassGestionFechas.Formatea_fecha_time_framework(DateCreate,
                                                                  Fecha_Fromat)
        If Result <> "YES" Then
            Actualizar_datos_anotacion = Result
            Exit Function
        End If
        Dim Parametro_Update As String = "UPDATE ANOTACION_TAREA" &
            " SET DATO_ANOTACION='" & Dat_anotacion & "'" &
            " WHERE ID_ANOTACION=" & Id_anotacion
        Dim Parametro_Insert As String = "INSERT INTO wf_log_workflow (usuario_workflow_idU_suario,fecha_hora,operacion,ID_TAREA_WORKFLOW," &
            "datos_operacion,opcion,descripcion_opcion,ip_transacion,id_operacion) values (" & id_usuario_workflow & ",'" & Fecha_Fromat & "','Actualiza'," &
            id_tarea_workflow & ",'" & Dat_anotacion & "',1,'NOTA WORKFLOW','" &
            HttpContext.Current.Session.Item("ip_host_name") & "'," & Id_anotacion & ")"
        Try
            myCommand.CommandText = Parametro_Update
            paramter_resp = myCommand.ExecuteNonQuery()
            If paramter_resp = 0 Then
                myConnection.Close()
                Actualizar_datos_anotacion = "Imposible actualizar la nota  " & Parametro_Update
                Exit Function
            End If
            myCommand.CommandText = Parametro_Insert
            paramter_resp = myCommand.ExecuteNonQuery()
            If paramter_resp = 0 Then
                myTrans.Rollback()
                myConnection.Close()
                Actualizar_datos_anotacion = "Imposible registrar el log de anotación para la nota " & Parametro_Insert
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            Actualizar_datos_anotacion = "YES"
            Exit Function
        Catch e As Exception
            Try
                myTrans.Rollback()
                myConnection.Close()
                Actualizar_datos_anotacion = "Error Actualizando  " & Parametro_Update &
                " Insertando " & Parametro_Insert
                Exit Function
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Actualizar_datos_anotacion = "An exception of type " & ex.GetType().ToString() &
                                      " was encountered while attempting to roll back the transaction."
                    myConnection.Close()
                    Exit Function
                Else
                    Actualizar_datos_anotacion = "Error Actualizando FUNCION Actualizar_Datos_Anotacion " & ex.Message
                    Exit Function
                End If
            End Try
        End Try
    End Function
    Function Eliminar_nota_tarea_workflow(ByVal Dat_anotacion As String,
                                          ByVal Id_anotacion As Integer,
                                          ByVal id_tarea_workflow As Long,
                                          ByVal id_usuario_workflow As Integer) As String
        '---------------------------------------------------------------------------
        'Funcion : Elimina la nota de la tarea
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'Dat_anotacion         : Representa el contenido de la nota
        'Id_anotacion          : Representa la identificacion de la anotacion
        'id_tarea_workflow     : Representa la identificación del tarea workflow
        'id_usuario_workflow   : Representa la identificacion del usuario workflow
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        '
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-07-01
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        'Try
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Dim myTrans As MySqlTransaction
        myTrans = myConnection.BeginTransaction()
        myCommand.Connection = myConnection
        myCommand.Transaction = myTrans
        Dim paramter_resp As Integer = 0
        '-----------------------------
        'Formatea framework actual
        '-----------------------------
        Dim Fecha_Fromat As String = ""
        Dim DateCreate As Date = Now
        Dim ClassGestionFechas As New ClassGestionFechas
        Dim Result As String = ClassGestionFechas.Formatea_fecha_time_framework(DateCreate,
                                                                                Fecha_Fromat)
        If Result <> "YES" Then
            Eliminar_nota_tarea_workflow = Result
            Exit Function
        End If
        Dim Parametro_Update As String = "delete from  ANOTACION_TAREA" &
            " WHERE ID_ANOTACION=" & Id_anotacion & " AND " &
            " ID_USUARIO=" & id_usuario_workflow
        Dim Parametro_Insert As String = "INSERT INTO wf_log_workflow (usuario_workflow_idU_suario,fecha_hora,operacion,ID_TAREA_WORKFLOW," &
            "datos_operacion,opcion,descripcion_opcion,ip_transacion,id_operacion) values (" & id_usuario_workflow & ",'" & Fecha_Fromat & "','Eliminar'," &
            id_tarea_workflow & ",'" & Dat_anotacion & "','1,'NOTA WORKFLOW','" &
            HttpContext.Current.Session.Item("ip_host_name") & "'," & Id_anotacion & ")"
        Try
            myCommand.CommandText = Parametro_Update
            paramter_resp = myCommand.ExecuteNonQuery()
            If paramter_resp = 0 Then
                myConnection.Close()
                Eliminar_nota_tarea_workflow = "Imposible eliminar la nota " & Parametro_Update
                Exit Function
            End If
            myCommand.CommandText = Parametro_Insert
            paramter_resp = myCommand.ExecuteNonQuery()
            If paramter_resp = 0 Then
                myTrans.Rollback()
                myConnection.Close()
                Eliminar_nota_tarea_workflow = "Imposible registrar log de eliminación de la nota  " & Parametro_Insert
                Exit Function
            End If

            myTrans.Commit()
            myConnection.Close()
            Parametro_Insert = "YES"
            Exit Function
        Catch e As Exception
            Try
                myTrans.Rollback()
                myConnection.Close()
                Parametro_Insert = "Error Actualizando  " & Parametro_Update &
                " Insertando " & Parametro_Insert
                Exit Function
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Parametro_Insert = "An exception of type " & ex.GetType().ToString() &
                                      " was encountered while attempting to roll back the transaction."
                    myConnection.Close()
                    Exit Function
                Else
                    Parametro_Insert = "Error Actualizando FUNCION Actualizar_Datos_Anotacion " & ex.Message
                    Exit Function
                End If
            End Try
        End Try
    End Function
    Function Solicita_estado_usuario_propietario_nota(ByVal id_usuario_workflow As Integer,
                                                      ByVal id_nota As Integer,
                                                      ByRef estado_propietario As String) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita estado de propiedad del usuario de la tarea
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_usuario_workflow   : Representa el id usuario workflow propietario
        'id_nota               : Representa el identificador de la nota
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'estado_propietario  : Retorna la estado de propiedad de la nota
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : NA
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim sql_consulta As String = "Select ID_ANOTACION from  ANOTACION_TAREA" &
            " WHERE ID_ANOTACION=" & id_nota & " AND " &
            " ID_USUARIO=" & id_usuario_workflow
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ANOTACION_TAREA")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_estado_usuario_propietario_nota = "Función Solicita_estado_usuario_propietario_nota dice (" & Result & ")"
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_propietario = "NO"
                Solicita_estado_usuario_propietario_nota = "YES"
                Exit Function
            Else
                estado_propietario = "YES"
                Solicita_estado_usuario_propietario_nota = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estado_usuario_propietario_nota = "Inconsistencia general función Solicita_estado_usuario_propietario_nota " & ex.Message
        End Try
    End Function
    Function Create_note_workflow(ByVal value_nota As String,
                                  ByVal id_tarea_workflow As Long,
                                  ByVal id_usuario_workflow As Integer,
                                  ByVal id_grupo_workflow As Integer,
                                  ByRef id_nota As Integer,
                                  ByRef fecha_anotacion As String) As String
        '---------------------------------------------------------------------------
        'Funcion : Crea la nota de una tarea workflow
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'value_nota            : Representa el conteido de la nota
        'id_tarea_workflow     : Representa la identificacion de la tarea workflow
        'id_usuario_workflow   : Representa la identificacion del usuario workflow
        '                        propietario de la nueva nota
        'id_grupo_workflow     : Representa la identificacion del grupo workflow
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'id_nota               : Retorna la idnetificación de la nueva nota
        'fecha_anotacion       : Retorna fecha de anotación
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-07-03
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        'Try
        Dim Result As String = ""
        Dim Class_grupos_workflow As New Class_grupos_workflow
        Dim id_actividad_workflow As Integer = 0
        Result = Class_grupos_workflow.Solicita_id_actividad_grupo_workflow(id_grupo_workflow,
                                                                                 id_actividad_workflow)
        If Result <> "YES" Then
            Create_note_workflow = Result
            Exit Function
        End If

        Dim Fecha_Fromat As String = ""
        Dim DateCreate As Date = Now
        '-----------------------------
        'Formatea fecha para mysql
        '-----------------------------
        Dim ClassGestionFechas As New ClassGestionFechas
        Result = ClassGestionFechas.Formatea_fecha_time_db(DateCreate,
                                                           Fecha_Fromat)
        If Result <> "YES" Then
            Create_note_workflow = Result
            Exit Function
        End If
        fecha_anotacion = Fecha_Fromat
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Dim myTrans As MySqlTransaction
        myTrans = myConnection.BeginTransaction()
        myCommand.Connection = myConnection
        myCommand.Transaction = myTrans
        Dim paramter_resp As Integer = 0
        Dim Parametro_Insert As String = "INSERT INTO ANOTACION_TAREA " &
            "(INICIO_TAREAS_WORKFLOW_ID_TAREA," &
            "DATO_ANOTACION,ID_ACTIVIDAD,ID_USUARIO" &
            ",FECHA_ANOTACION,ESTADO_TAREA) VALUES (" &
            id_tarea_workflow & ",'" &
            value_nota & "'," &
            id_actividad_workflow & "," &
            id_usuario_workflow & ",'" &
            Fecha_Fromat & "',1)"
        Try
            myCommand.CommandText = Parametro_Insert
            paramter_resp = myCommand.ExecuteNonQuery()
            If paramter_resp = 0 Then
                myConnection.Close()
                Create_note_workflow = "Imposible registrar la nota  " & Parametro_Insert
                Exit Function
            End If
            id_nota = myCommand.LastInsertedId
            Dim Parametro_Insert_log As String = "INSERT INTO wf_log_workflow (usuario_workflow_idU_suario,fecha_hora,operacion,ID_TAREA_WORKFLOW," &
            "datos_operacion,opcion,descripcion_opcion,ip_transacion,id_operacion) values (" & id_usuario_workflow & ",'" & Fecha_Fromat & "','Agrega'," &
            id_tarea_workflow & ",'" & value_nota & "',1,'NOTA WORKFLOW','" &
            HttpContext.Current.Session.Item("ip_host_name") & "'," & myCommand.LastInsertedId & ")"
            myCommand.CommandText = Parametro_Insert_log
            paramter_resp = myCommand.ExecuteNonQuery()
            If paramter_resp = 0 Then
                myTrans.Rollback()
                myConnection.Close()
                Create_note_workflow = "Imposible registrar el log de anotación para la nueva nota " & Parametro_Insert
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            Create_note_workflow = "YES"
            Exit Function
        Catch e As Exception
            Try
                myTrans.Rollback()
                myConnection.Close()
                Create_note_workflow = "Error registrando funcion Create_note_workflow  " & Parametro_Insert &
                " Insertando " & Parametro_Insert
                Exit Function
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Create_note_workflow = "An exception of type " & ex.GetType().ToString() &
                                      " was encountered while attempting to roll back the transaction."
                    myConnection.Close()
                    Exit Function
                Else
                    Create_note_workflow = "Error registrando FUNCION Create_note_workflow " & ex.Message
                    Exit Function
                End If
            End Try
        End Try
    End Function
    Function Listar_Numero_Anotaciones(ByVal id_tarea_seleccion As Integer,
                                       ByRef numero_notas As Integer) As String
        '********************************************
        'Function : Listar_Numero_Anotaciones
        'Fecha : 2011-09-14
        'Ingeniero : Miguel Angel Urueta Miranda
        '********************************************
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT * " &
            " FROM ANOTACION_TAREA AT " &
            "WHERE INICIO_TAREAS_WORKFLOW_ID_TAREA=" & id_tarea_seleccion &
            " AND ESTADO_TAREA=1 "
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ANOTACION_TAREA")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Listar_Numero_Anotaciones = " Lista anotaciones #1 Imposible econsultar tabla " & Result
                Exit Function
            End If
            numero_notas = Datset.Tables(0).Rows.Count
            Listar_Numero_Anotaciones = "YES"
            Exit Function
        Catch ex As Exception
            Listar_Numero_Anotaciones = "Error General Funcion Listar_Numero_Anotaciones Error " & ex.Message
        End Try
    End Function
    Function Service_solicita_lista_notas_tarea_workflow(ByVal id_tarea_workflow As Long,
                                                         ByRef ilis_class_detail_note As List(Of class_detail_note)) As String
        '-----------------------------------------------------------------------------
        'Funcion : Solicita el detalle de las notas de una tarea determinada
        '          
        '-----------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------
        'id_tarea_workflo         : Representa la identificacion de la tarea workflow
        '
        '------------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'ilis_class_detail_note  : Retorna la estructura del listado del log de
        '                          notas de la tarea
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-10-31
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim ilis_class_detail_note_ As class_detail_note = Nothing
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("anotacion_tarea")
            Dim Sql_consulta As String = "select at.Id_Anotacion as Id_Anotacion,at.fecha_anotacion as fecha_anotacion,law.Nombre_Actividad," &
                "uw.Nombre_Usuario as nombre_usuario, at.Dato_Anotacion as dato_anotacion " &
                " from anotacion_tarea as at" &
                " left outer join  listado_actividades_workflow as law on (law.Id_Actividad=at.Id_Actividad)" &
                " left outer join  usuario_workflow as uw on (uw.idU_suario=at.id_usuario)" &
                " where at.Inicio_Tareas_Workflow_id_Tarea=" & id_tarea_workflow
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                ilis_class_detail_note_ = New class_detail_note
                ilis_class_detail_note_.result = Result
                ilis_class_detail_note_.id_anotacion = -1
                ilis_class_detail_note.Add(ilis_class_detail_note_)
                Service_solicita_lista_notas_tarea_workflow = "Error function Service_solicita_lista_notas_tarea_workflow " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                ilis_class_detail_note_ = New class_detail_note
                ilis_class_detail_note_.result = Result
                ilis_class_detail_note_.id_anotacion = -1
                ilis_class_detail_note.Add(ilis_class_detail_note_)
                Service_solicita_lista_notas_tarea_workflow = "Error function Service_lista_notas_tarea_workflow " & Result
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ilis_class_detail_note_ = New class_detail_note
                    ilis_class_detail_note_.result = "YES"
                    ilis_class_detail_note_.id_anotacion = Datset.Tables(0).Rows(i).Item(0)
                    ilis_class_detail_note_.fecha_anotacion = Datset.Tables(0).Rows(i).Item(1)
                    ilis_class_detail_note_.nombre_actividad = Datset.Tables(0).Rows(i).Item(2)
                    ilis_class_detail_note_.nombre_usuario = Datset.Tables(0).Rows(i).Item(3)
                    ilis_class_detail_note_.dato_anotacion = Datset.Tables(0).Rows(i).Item(4)
                    ilis_class_detail_note.Add(ilis_class_detail_note_)
                Next
                Service_solicita_lista_notas_tarea_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Service_solicita_lista_notas_tarea_workflow = "Inconsistencia general funcion Service_solicita_lista_notas_tarea_workflow " & ex.Message
            Exit Function
        End Try
    End Function
End Class
