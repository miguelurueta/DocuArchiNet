Imports System.IO
Imports MySql.Data
Imports System.Xml
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.OleDb
Imports System.Data.Odbc
Imports MySql.Data.MySqlClient
Imports System.IO.IsolatedStorage
Public Class ClassGaClasificacionDocumental
    
    Public Structure stru_serie_subserie
        Dim id_serie_sub_serie As Integer
        Dim Nombre_serie_subserie As String
        Dim id_area As Integer
        Dim tipo_seri_sub_serie As String
    End Structure
    Function Lista_cuadro_clasificacion_documental_drowlist(ByRef Combo As DropDownList) As String
        '******************************************************
        'Funcion : Lista cuadros de clasficación documental
        'Fecha : 2017-01-13
        'Igeniero: Miguel Angel Urueta Miranda
        '******************************************************
        Try
            Combo.Items.Clear()
            Dim Parametro_Consulta As String = "select  NOMBRE_ENTIDAD_CLASFICACION " & _
                  " from ra_de_cuadro_clasificacion "
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Dat_reader As DataSet = New DataSet("ra_de_cuadro_clasificacion")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Lista_cuadro_clasificacion_documental_drowlist = " Imposible conectar la base de datos del gestor documental " & Result
                Combo.Items.Clear()
                'update.Update()
                Exit Function
            End If
            Dim Paswuser As String = ""
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                Combo.Items.Add("")
                For i As Integer = 0 To Dat_reader.Tables(0).Rows.Count - 1
                    Combo.Items.Add(Dat_reader.Tables(0).Rows(i).Item(0).ToString)
                Next
                'update.Update()
                Lista_cuadro_clasificacion_documental_drowlist = "YES"
            Else
                Combo.Items.Clear()
                'update.Update()
                Lista_cuadro_clasificacion_documental_drowlist = "YES"
            End If

        Catch ex As Exception
            Lista_cuadro_clasificacion_documental_drowlist = "Inconsistencia General Funcion Lista_cuadro_clasificacion_documental_drowlist " & ex.Message
        End Try
    End Function
    Function Lista_cuadro_clasificacion_documental_drowlist_seleccion(ByRef Combo As DropDownList, _
                                                                      ByVal nombre_seleccion As String) As String
        '******************************************************
        'Funcion : Lista cuadros de clasficación documental
        'Fecha : 2017-01-13
        'Igeniero: Miguel Angel Urueta Miranda
        '******************************************************
        Try
            Combo.Items.Clear()
            Dim Parametro_Consulta As String = "select  NOMBRE_ENTIDAD_CLASFICACION " & _
                  " from ra_de_cuadro_clasificacion "
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Dat_reader As DataSet = New DataSet("ra_de_cuadro_clasificacion")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Lista_cuadro_clasificacion_documental_drowlist_seleccion = "Función Lista_cuadro_clasificacion_documental_drowlist_seleccion Imposible conectar la base de datos del gestor documental " & Result
                Combo.Items.Clear()
                'update.Update()
                Exit Function
            End If
            Dim Paswuser As String = ""
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                'Combo.Items.Add("")
                For i As Integer = 0 To Dat_reader.Tables(0).Rows.Count - 1
                    Combo.Items.Add(Dat_reader.Tables(0).Rows(i).Item(0).ToString)
                Next
                For i As Integer = 0 To Combo.Items.Count - 1
                    If Combo.Items(i).Value = nombre_seleccion Then
                        Combo.Text = nombre_seleccion
                        Exit For
                    End If
                Next
                Lista_cuadro_clasificacion_documental_drowlist_seleccion = "YES"
            Else
                Combo.Items.Clear()
                'update.Update()
                Lista_cuadro_clasificacion_documental_drowlist_seleccion = "YES"
            End If

        Catch ex As Exception
            Lista_cuadro_clasificacion_documental_drowlist_seleccion = "Inconsistencia General Funcion Lista_cuadro_clasificacion_documental_drowlist " & ex.Message
        End Try
    End Function
    Function Lista_niveles_de_clasificacion_documental_drowlist(ByRef Combo As DropDownList) As String
        '******************************************************
        'Funcion : Lista cuadros de clasficación documental
        'Fecha : 2017-01-13
        'Igeniero: Miguel Angel Urueta Miranda
        '******************************************************
        Try
            Combo.Items.Clear()
            Dim Parametro_Consulta As String = "select  NOMBRE_NIVEL_DESCRIPCION " & _
                  " from ra_de_niveles_descripcion where ESTADO_NIVEL_DESCRIPCION=1"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Dat_reader As DataSet = New DataSet("ra_de_niveles_descripcion")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Lista_niveles_de_clasificacion_documental_drowlist = " Imposible conectar la base de datos del gestor documental " & Result
                Combo.Items.Clear()
                'update.Update()
                Exit Function
            End If
            Dim Paswuser As String = ""
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                Combo.Items.Add("")
                For i As Integer = 0 To Dat_reader.Tables(0).Rows.Count - 1
                    Combo.Items.Add(Dat_reader.Tables(0).Rows(i).Item(0).ToString)
                Next
                'update.Update()
                Lista_niveles_de_clasificacion_documental_drowlist = "YES"
            Else
                Combo.Items.Clear()
                'update.Update()
                Lista_niveles_de_clasificacion_documental_drowlist = "YES"
            End If

        Catch ex As Exception
            Lista_niveles_de_clasificacion_documental_drowlist = "Inconsistencia General Funcion Lista_cuadro_clasificacion_documental_drowlist " & ex.Message
        End Try
    End Function
   
    Function Verifica_existencia_cuadro_clasificacion_documental(ByVal nombre_entidad_cuadro_clasificacion As String) As String
        '---------------------------------------------------------------
        'Función : Verifica la existencia del cuadro de clasificacion
        'Fecha : 2017-01-13
        'Ing Miguel Angel Urueta Miranda
        '---------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select  ID_DE_CUADRO_CLASIFICACION " & _
                      " from ra_de_cuadro_clasificacion where  NOMBRE_ENTIDAD_CLASFICACION='" & nombre_entidad_cuadro_clasificacion & "'"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Dat_reader As DataSet = New DataSet("ra_de_cuadro_clasificacion")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Verifica_existencia_cuadro_clasificacion_documental = "Función Verifica_existencia_cuadro_clasificacion_documental Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                Verifica_existencia_cuadro_clasificacion_documental = "El sistema ya registra un estructura de clasificación documental con el nombre  " & nombre_entidad_cuadro_clasificacion
                Exit Function
            Else
                Verifica_existencia_cuadro_clasificacion_documental = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_existencia_cuadro_clasificacion_documental = "Inconsistencia general función Verifica_existencia_cuadro_clasificacion_documental " & ex.Message
        End Try
    End Function
    Function Verifica_existencia_relaciones_jerarquia_cuadro_clasificacion_documental(ByVal id_cuadro_clasificacion As String) As String
        '---------------------------------------------------------------
        'Función : Verifica la existencia relaciones de la extrucutura
        'jerarquia del cuadro de clasificación documental
        'Fecha : 2017-01-14
        'Ing Miguel Angel Urueta Miranda
        '---------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select  RA_DE_CUADRO_CLASIFICACION_ID_DE_CUADRO_CLASIFICACION " & _
                      " from ra_de_registro_relaciones_jerarquia where  RA_DE_CUADRO_CLASIFICACION_ID_DE_CUADRO_CLASIFICACION='" & id_cuadro_clasificacion & "'"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Dat_reader As DataSet = New DataSet("ra_de_registro_relaciones_jerarquia")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Verifica_existencia_relaciones_jerarquia_cuadro_clasificacion_documental = "Función Verifica_existencia_relaciones_jerarquia_cuadro_clasificacion_documental Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                Verifica_existencia_relaciones_jerarquia_cuadro_clasificacion_documental = "El sistema ya registra relaciones de jerarquía en el cuadro de clasficiación  " & id_cuadro_clasificacion
                Exit Function
            Else
                Verifica_existencia_relaciones_jerarquia_cuadro_clasificacion_documental = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_existencia_relaciones_jerarquia_cuadro_clasificacion_documental = "Inconsistencia general función Verifica_existencia_relaciones_jerarquia_cuadro_clasificacion_documental " & ex.Message
        End Try
    End Function
    Function Retorna_id_cuadro_clasificacion_documental(ByVal nombre_entidad_cuadro_clasificacion As String, _
                                                        ByRef id_cuadro_clasficacion As Integer) As String
        '---------------------------------------------------------------
        'Función : retorna el id del cuadro de clasificacion
        'Fecha : 2017-01-13
        'Ing Miguel Angel Urueta Miranda
        '---------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select  ID_DE_CUADRO_CLASIFICACION " & _
                      " from ra_de_cuadro_clasificacion where  NOMBRE_ENTIDAD_CLASFICACION='" & nombre_entidad_cuadro_clasificacion & "'"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Dat_reader As DataSet = New DataSet("ra_de_cuadro_clasificacion")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Retorna_id_cuadro_clasificacion_documental = "Función Retorna_id_cuadro_clasificacion_documental Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                id_cuadro_clasficacion = Dat_reader.Tables(0).Rows(0).Item(0)
                Retorna_id_cuadro_clasificacion_documental = "YES"
                Exit Function
            Else
                Retorna_id_cuadro_clasificacion_documental = "Imposible encontrar el identificador del cuadro de clasificación " & nombre_entidad_cuadro_clasificacion
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_cuadro_clasificacion_documental = "Inconsistencia general función Retorna_id_cuadro_clasificacion_documental " & ex.Message
        End Try
    End Function
    
    Function Retorna_datos_caracterizacion_cuadro_clasificacion_por_id_cuadro(ByVal id_cuadro_clasificacion As Integer, _
                                                                              ByRef nombre_entidad_cuadro_clasificacion As String, _
                                                                              ByRef codigo_pais As String, _
                                                                              ByRef fecha_extrema_inicial As String, _
                                                                              ByRef fecha_extrema_final As String) As String
        '---------------------------------------------------------------
        'Función : Retorna datos de caracterización cuadro clasificación
        'Fecha : 2017-01-13
        'Ing Miguel Angel Urueta Miranda
        '---------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select  CODIGO_CUADRO,FECHA_INICIAL,FECHA_FINAL,NOMBRE_ENTIDAD_CLASFICACION " & _
                      " from ra_de_cuadro_clasificacion where  ID_DE_CUADRO_CLASIFICACION='" & id_cuadro_clasificacion & "'"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Dat_reader As DataSet = New DataSet("ra_de_cuadro_clasificacion")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Retorna_datos_caracterizacion_cuadro_clasificacion_por_id_cuadro = "Función Retorna_datos_caracterizacion_cuadro_clasificacion_por_id_cuadro Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                codigo_pais = Dat_reader.Tables(0).Rows(0).Item(0)
                fecha_extrema_inicial = Dat_reader.Tables(0).Rows(0).Item(1)
                fecha_extrema_final = Dat_reader.Tables(0).Rows(0).Item(2)
                nombre_entidad_cuadro_clasificacion = Dat_reader.Tables(0).Rows(0).Item(3)
                Retorna_datos_caracterizacion_cuadro_clasificacion_por_id_cuadro = "YES"
                Exit Function
            Else
                Retorna_datos_caracterizacion_cuadro_clasificacion_por_id_cuadro = "Imposible encontrar el identificador del cuadro de clasificación " & nombre_entidad_cuadro_clasificacion
                Exit Function
            End If
        Catch ex As Exception
            Retorna_datos_caracterizacion_cuadro_clasificacion_por_id_cuadro = "Inconsistencia general función Retorna_datos_caracterizacion_cuadro_clasificacion_por_id_cuadro " & ex.Message
        End Try
    End Function
    Function Retorna_registro_jerarquia_cuadro_clasificacion(ByVal id_cuadro_clasificacion As Integer, _
                                                             ByVal id_nivel_descripcion As Integer, _
                                                             ByRef id_registro_jerarquia As Integer) As String
        '-----------------------------------------------------------
        'Función : Retorna la identificacion del nivel de jerarquia
        'del cuadro de clasificación documental
        'Fecha : 2017-01-13
        'Ing : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------
        Try

            Dim Result As String = ""
            Dim Parametro_Consulta As String = "select  ID_REGISTRO_JERARQUIA " & _
                          " from ra_de_registro_jerarquia where  RA_DE_NIVELES_DESCRIPCION_ID_NIVELES_DESCRIPCION='" & id_nivel_descripcion & "' and " & _
                          " RA_DE_CUADRO_CLASIFICACION_ID_DE_CUADRO_CLASIFICACION=" & id_cuadro_clasificacion
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA

            Dim Dat_reader As DataSet = New DataSet("ra_de_cuadro_clasificacion")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Retorna_registro_jerarquia_cuadro_clasificacion = "Función Retorna_registro_jerarquia_cuadro_clasificacion Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                id_registro_jerarquia = Dat_reader.Tables(0).Rows(0).Item(0)
                Retorna_registro_jerarquia_cuadro_clasificacion = "YES"
                Exit Function
            Else
                Retorna_registro_jerarquia_cuadro_clasificacion = "Imposible encontrar el registro de jerarquía del cuadro de clasificación de la entidad " & id_cuadro_clasificacion
                Exit Function
            End If
        Catch ex As Exception
            Retorna_registro_jerarquia_cuadro_clasificacion = "Inconsistencia general función Retorna_registro_jerarquia_cuadro_clasificacion " & ex.Message
        End Try
    End Function
    Function Retorna_ayuda_restricciones_niveles_clasificacion(ByVal nombre_nivel_clasficacion As String, _
                                                               ByRef texto_ayuda_nivel_clasificacion As String) As String
        '---------------------------------------------------------------
        'Función : Retorna texto de ayuda del nivel de clasificación
        'Fecha : 2017-01-18
        'Ing Miguel Angel Urueta Miranda
        '---------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select  DESCRIPCION_NIVEL_DESCRIPCION " & _
                      " from ra_de_niveles_descripcion where  NOMBRE_NIVEL_DESCRIPCION='" & nombre_nivel_clasficacion & "'"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Dat_reader As DataSet = New DataSet("ra_de_niveles_descripcion")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Retorna_ayuda_restricciones_niveles_clasificacion = "Función Retorna_ayuda_restricciones_niveles_clasificacion Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                If Dat_reader.Tables(0).Rows(0).IsNull(0) = True Then
                    texto_ayuda_nivel_clasificacion = ""
                Else
                    texto_ayuda_nivel_clasificacion = Dat_reader.Tables(0).Rows(0).Item(0)
                End If
                Retorna_ayuda_restricciones_niveles_clasificacion = "YES"
                Exit Function
            Else
                Retorna_ayuda_restricciones_niveles_clasificacion = "Imposible encontrar la descripción del nivel de clasificación documental " & nombre_nivel_clasficacion
                Exit Function
            End If
        Catch ex As Exception
            Retorna_ayuda_restricciones_niveles_clasificacion = "Inconsistencia general función Retorna_ayuda_restricciones_niveles_clasificacion " & ex.Message
        End Try
    End Function

    Function Eliminar_cuadro_clasificacion_documental(ByVal id_registro_jerarquia As Integer, _
                                                      ByRef ref_triview As TreeView) As String
        Dim Result As String = ""
        Dim id_cuadro_clasificacion As Integer = 0
        Dim Ref_clas_registro_jerarquia As New Class_ra_de_registro_jerarquia
        Result = Ref_clas_registro_jerarquia.Retorna_id_cuadro_registro_jerarquia(id_registro_jerarquia, _
                                                                                id_cuadro_clasificacion)
        If Result <> "YES" Then
            Eliminar_cuadro_clasificacion_documental = Result
            Exit Function
        End If
        Result = Me.Verifica_existencia_relaciones_jerarquia_cuadro_clasificacion_documental(id_cuadro_clasificacion)
        If Result <> "YES" Then
            Eliminar_cuadro_clasificacion_documental = Result
            Exit Function
        End If
        '-------------------------------------------------------------------------
        'Retorna datos de caracterización cuadro clasficacion
        '-------------------------------------------------------------------------
        Dim ref_nombre_empresa_clasificacion As String = ""
        Dim ref_fecha_extrema_ini As String = ""
        Dim ref_fecha_extrema_fin As String = ""
        Dim ref_codigo_pais As String = ""
        Result = Me.Retorna_datos_caracterizacion_cuadro_clasificacion_por_id_cuadro(id_cuadro_clasificacion, ref_nombre_empresa_clasificacion, ref_codigo_pais, _
                                                                                      ref_fecha_extrema_ini, ref_fecha_extrema_fin)
        If Result <> "YES" Then
            Eliminar_cuadro_clasificacion_documental = Result
            Exit Function
        End If
        Dim date1al As String = Date.Today
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            Eliminar_cuadro_clasificacion_documental = Result
            Exit Function
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            '-----------------------------------
            ' Elimina registro nível de jerarquía
            '-----------------------------------
            Dim sqlinsertcion As String = "Delete FROM ra_de_registro_jerarquia" & " where RA_DE_CUADRO_CLASIFICACION_ID_DE_CUADRO_CLASIFICACION='" & id_cuadro_clasificacion & "'"
            myCommand.CommandText = sqlinsertcion
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Eliminar_cuadro_clasificacion_documental = "Imposible eliminar el registro de jerarquia del cuadro de clasificación : " & Left(sqlinsertcion, 30)
                myConnection.Close()
                errorM = "Imposible eliminar el registro de jerarquia del cuadro de clasificación : " & Left(sqlinsertcion, 30)
                Exit Function
            End If
            '-----------------------------------
            'Elimina cuadro clasificación
            '-----------------------------------
            sqlinsertcion = "Delete FROM ra_de_cuadro_clasificacion " & " where ID_DE_CUADRO_CLASIFICACION='" & id_cuadro_clasificacion & "'"
            myCommand.CommandText = sqlinsertcion
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Eliminar_cuadro_clasificacion_documental = "Imposible eliminar el cuadro de clasificación  : " & Left(sqlinsertcion, 30)
                myTrans.Rollback()
                myConnection.Close()
                errorM = "Imposible eliminar el cuadro de clasificación  : " & Left(sqlinsertcion, 30)
                Exit Function
            End If
            '-----------------------------------
            'Registra log
            '-----------------------------------
            Dim campos_cambio As String = "CAMPO|CONTENIDO" & vbCrLf & _
                "NOMBRE_ENTIDAD_CLASFICACION|" & ref_nombre_empresa_clasificacion & vbCrLf & _
                "CODIGO_CUADRO|" & ref_codigo_pais & vbCrLf & _
                "FECHA_INICIAL|" & ref_fecha_extrema_ini & vbCrLf & _
                "FECHA_FINAL|" & ref_fecha_extrema_fin & vbCrLf
            Dim hor As String = Now
            Dim sqlforupdate As String = "INSERT INTO ra_log_organizacion_documental (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_UNIDAD" & _
            ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values (" & _
            "'ELIMINA CUADRO CLASIFICACION','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "','" & _
            id_cuadro_clasificacion & "','" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','GESTOR WEB','" & campos_cambio & "')"
            myCommand.CommandText = sqlforupdate
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Eliminar_cuadro_clasificacion_documental = "Imposible registrar el log de transacción  : " & Left(sqlforupdate, 30)
                myTrans.Rollback()
                myConnection.Close()
                errorM = "Imposible registrar el log de transacción  : " & Left(sqlforupdate, 30)
                Exit Function
            End If
            ref_triview.Nodes.Remove(ref_triview.SelectedNode)
            myTrans.Commit()
            myConnection.Close()
            Eliminar_cuadro_clasificacion_documental = "YES"
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                'myTrans.Rollback()
                myConnection.Close()
                Eliminar_cuadro_clasificacion_documental = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Eliminar_cuadro_clasificacion_documental = errorM

        End Try
    End Function
    Function Elimina_nivel_cuadro_clasificacion_documental(ByVal id_nivel_jerarquia_padre As Integer, _
                                                           ByRef treview As TreeView, _
                                                           ByRef trenode As TreeNode) As String
        Dim Result As String = ""
        Dim matri_id_relaciones_jerarquia_hijo() As Integer = Nothing
        '----------------------------------------------------
        'Verifica existencia de relaciones jerarquias del
        'nivel de clasificación 
        '----------------------------------------------------
        Dim ref_ra_de_re_jerarquia As New Class_ra_de_registro_jerarquia
        Result = ref_ra_de_re_jerarquia.Retorna_listado_relaciones_jerarquia(id_nivel_jerarquia_padre, _
                                                                             matri_id_relaciones_jerarquia_hijo)
        If Result <> "YES" Then
            Elimina_nivel_cuadro_clasificacion_documental = Result
            Exit Function
        End If
        If Not matri_id_relaciones_jerarquia_hijo Is Nothing Then
            Elimina_nivel_cuadro_clasificacion_documental = "El sistema no puede eliminar el nivel de clasificación, elimine los  niveles inferiores relacionados a este nivel clasificación "
            Exit Function
        End If
        '--------------------------------------------------
        'Retorna datos de titulo y signatura
        '--------------------------------------------------
        Dim titulo As String = ""
        Dim signatura As String = ""
        Dim Class_ra_de_descripcion_niveles_jerarquia As New Class_ra_de_descripcion_niveles_jerarquia
        Result = Class_ra_de_descripcion_niveles_jerarquia.Retorna_datos_de_descripcion_titulo_signatura_nivel_clasificacion(id_nivel_jerarquia_padre, _
                                                                                                                             titulo, _
                                                                                                                             signatura)
        If Result <> "YES" Then
            Elimina_nivel_cuadro_clasificacion_documental = Result
            Exit Function
        End If
        '--------------------------------------------------
        'Retorna nombre nivel de clasificación
        '--------------------------------------------------
        Dim id_nivel_clasificacion As Integer = 0
        Dim nombre_nivel_clasificacion As String = ""
        Dim ref_des_niv_jerarquia As New Class_ra_de_descripcion_niveles_jerarquia
        Result = ref_des_niv_jerarquia.Retorna_id_nombre_nivel_descripcion_por_id_jerarquia(id_nivel_jerarquia_padre, _
                                                                                            id_nivel_clasificacion, _
                                                                                            nombre_nivel_clasificacion)
        If Result <> "YES" Then
            Elimina_nivel_cuadro_clasificacion_documental = Result
            Exit Function
        End If
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Dim date1al As String = Date.Today
        Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            Elimina_nivel_cuadro_clasificacion_documental = Result
            Exit Function
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Try

            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            '-----------------------------------
            ' Elimina relación nível de jerarquía
            '-----------------------------------
            Dim sqlinsertcion As String = "Delete FROM ra_de_registro_relaciones_jerarquia" & " where ID_REGISTRO_JERARQUIA_HIJO='" & id_nivel_jerarquia_padre & "'"
            myCommand.CommandText = sqlinsertcion
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Elimina_nivel_cuadro_clasificacion_documental = "Imposible eliminar relaciones de jerarquia del nivel de clasificación : " & Left(sqlinsertcion, 30)
                myConnection.Close()
                errorM = "Imposible eliminar relaciones de jerarquia del nivel de clasificación : " & Left(sqlinsertcion, 30)
                Exit Function
            End If
            '--------------------------------------
            'Elimina descripcion nivel de jerarquía
            '--------------------------------------
            sqlinsertcion = "Delete FROM ra_de_descripcion_niveles_jerarquia " & " where RA_DE_REGISTRO_JERARQUIA_ID_REGISTRO_JERARQUIA='" & id_nivel_jerarquia_padre & "'"
            myCommand.CommandText = sqlinsertcion
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Elimina_nivel_cuadro_clasificacion_documental = "Imposible eliminar la descripción del nivel de clasificación  : " & Left(sqlinsertcion, 30)
                myTrans.Rollback()
                myConnection.Close()
                errorM = "Imposible eliminar la descripción del nivel de clasificación  : " & Left(sqlinsertcion, 30)
                Exit Function
            End If
            '--------------------------------------
            'Elimina regitro de jeraquia
            '--------------------------------------
            sqlinsertcion = "Delete FROM ra_de_registro_jerarquia " & " where ID_REGISTRO_JERARQUIA='" & id_nivel_jerarquia_padre & "'"
            myCommand.CommandText = sqlinsertcion
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Elimina_nivel_cuadro_clasificacion_documental = "Imposible eliminar la jeraquía del nivel de clasificación  : " & Left(sqlinsertcion, 30)
                myTrans.Rollback()
                myConnection.Close()
                errorM = "Imposible eliminar la jeraquía del nivel de clasificación  : " & Left(sqlinsertcion, 30)
                Exit Function
            End If
            '-----------------------------------
            'Registra log
            '-----------------------------------
            Dim campos_cambio As String = "CAMPO|CONTENIDO" & vbCrLf & _
                "TITULO|" & titulo & vbCrLf & _
                "SIGNATURA|" & signatura & vbCrLf & _
                "NIVEL CALASIFICACION|" & nombre_nivel_clasificacion

            Dim hor As String = Now
            Dim sqlforupdate As String = "INSERT INTO ra_log_organizacion_documental (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_UNIDAD" & _
            ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values (" & _
            "'ELIMINA NIVEL CLASIFICACION','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "','" & _
            id_nivel_jerarquia_padre & "','" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','GESTOR WEB','" & campos_cambio & "')"
            myCommand.CommandText = sqlforupdate
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Elimina_nivel_cuadro_clasificacion_documental = "Imposible registrar el log de transacción  : " & Left(sqlforupdate, 30)
                myTrans.Rollback()
                myConnection.Close()
                errorM = "Imposible registrar el log de transacción  : " & Left(sqlforupdate, 30)
                Exit Function
            End If
            treview.Nodes.Remove(treview.SelectedNode)
            Dim sNodo As TreeNode = treview.SelectedNode
            Dim pNodo As TreeNode = sNodo.Parent
            pNodo.ChildNodes.Remove(sNodo)
            myTrans.Commit()
            myConnection.Close()
            Elimina_nivel_cuadro_clasificacion_documental = "YES"
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                'myTrans.Rollback()
                myConnection.Close()
                Elimina_nivel_cuadro_clasificacion_documental = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Elimina_nivel_cuadro_clasificacion_documental = errorM

        End Try
    End Function
    
    Function Restriciones_registro_nivel_clasificacion(ByVal id_nivel_clasificacion_nodo_padre As Integer, ByVal nombre_nivel_clasificacion_nodo_padre As String, _
        ByVal id_nivel_clasificacion_nodo_hijo As Integer, ByVal nombre_nivel_clasificacion_nodo_hijo As String) As String
        '-------------------------------------------------
        'Función : Funcion con las restricciones de cada
        'nivel de clasificación 
        'Fecha : 2017-01-19
        'Ing .Miguel Angel Urueta Miranda
        '-------------------------------------------------
        Dim Result As String = ""
        '-------------------------------------------------
        Try
            If id_nivel_clasificacion_nodo_padre = id_nivel_clasificacion_nodo_hijo Then
                Restriciones_registro_nivel_clasificacion = "El sistema no permite anidar niveles de clasificación del mismo tipo "
                Exit Function
            End If
            '----------------------------------------------
            'Restricciones Sección y subsección
            '----------------------------------------------
            If nombre_nivel_clasificacion_nodo_padre = "Sección y subsección" Then
                If nombre_nivel_clasificacion_nodo_hijo = "Fondo" Then
                    Restriciones_registro_nivel_clasificacion = "El sistema no permite anidar el nivel de clasificación (Fondo) a un nivel de descripción (Sección y subsección)"
                    Exit Function
                End If
            End If
            '----------------------------------------------
            'Restricción series y sub series
            '----------------------------------------------
            If nombre_nivel_clasificacion_nodo_padre = "Serie y subserie" Then
                If nombre_nivel_clasificacion_nodo_hijo = "Sección y subsección" Then
                    Restriciones_registro_nivel_clasificacion = "El sistema no permite anidar el nivel de clasificación (Sección y subsección) a un nivel de clasificación (Serie y subserie)"
                    Exit Function
                End If
                If nombre_nivel_clasificacion_nodo_hijo = "Fondo" Then
                    Restriciones_registro_nivel_clasificacion = "El sistema no permite anidar el nivel de clasificación (Fondo) a un nivel de descripción (Serie y subserie)"
                    Exit Function
                End If
            End If
            '---------------------------------------------
            'Unidad documental compuesta (Expediente)
            '---------------------------------------------
            If nombre_nivel_clasificacion_nodo_padre = "Unidad documental compuesta (Expediente)" Then
                If nombre_nivel_clasificacion_nodo_hijo = "Sección y subsección" Then
                    Restriciones_registro_nivel_clasificacion = "El sistema no permite anidar el nivel de clasificación (Sección y subsección) a un nivel de clasificación (Unidad documental compuesta (Expediente))"
                    Exit Function
                End If
                If nombre_nivel_clasificacion_nodo_hijo = "Fondo" Then
                    Restriciones_registro_nivel_clasificacion = "El sistema no permite anidar el nivel de clasificación (Fondo) a un nivel de descripción (Unidad documental compuesta (Expediente))"
                    Exit Function
                End If
                If nombre_nivel_clasificacion_nodo_hijo = "Serie y subserie" Then
                    Restriciones_registro_nivel_clasificacion = "El sistema no permite anidar el nivel de clasificación (Serie y subserie) a un nivel de descripción (Unidad documental compuesta (Expediente))"
                    Exit Function
                End If
                If nombre_nivel_clasificacion_nodo_hijo = "Unidad documental simple" Then
                    Restriciones_registro_nivel_clasificacion = "El sistema no permite anidar el nivel de clasificación (Unidad documental simple) a un nivel de descripción (Unidad documental compuesta (Expediente))"
                    Exit Function
                End If

            End If
            '---------------------------------------------
            'Unidad documental simple
            '---------------------------------------------
            If nombre_nivel_clasificacion_nodo_padre = "Unidad documental simple" Then
                If nombre_nivel_clasificacion_nodo_hijo = "Sección y subsección" Then
                    Restriciones_registro_nivel_clasificacion = "El sistema no permite anidar el nivel de clasificación (Sección y subsección) a un nivel de clasificación (Unidad documental simple)"
                    Exit Function
                End If
                If nombre_nivel_clasificacion_nodo_hijo = "Fondo" Then
                    Restriciones_registro_nivel_clasificacion = "El sistema no permite anidar el nivel de clasificación (Fondo) a un nivel de descripción (Unidad documental simple)"
                    Exit Function
                End If
                If nombre_nivel_clasificacion_nodo_hijo = "Serie y subserie" Then
                    Restriciones_registro_nivel_clasificacion = "El sistema no permite anidar el nivel de clasificación (Serie y subserie) a un nivel de descripción (Unidad documental simple)"
                    Exit Function
                End If
                If nombre_nivel_clasificacion_nodo_hijo = "Unidad documental compuesta (Expediente)" Then
                    Restriciones_registro_nivel_clasificacion = "El sistema no permite anidar el nivel de clasificación (Unidad documental compuesta (Expediente)) a un nivel de descripción (Unidad documental simple)"
                    Exit Function
                End If

            End If
            Restriciones_registro_nivel_clasificacion = "YES"
        Catch ex As Exception
            Restriciones_registro_nivel_clasificacion = "Inconsistencia general función Restriciones_registro_nivel_clasificacion " & ex.Message
        End Try
    End Function
    Function Asgina_datos_interface_edita_nivel(ByVal id_nivel_jerarquia_nodo_padre As Integer, _
                                                ByVal titulo_clasificacion As String, _
                                                ByVal signatura As String) As String
        Try
            Dim Result As String = ""
            Dim Refclas As New Class_ra_de_descripcion_niveles_jerarquia
            Result = Refclas.Retorna_datos_de_descripcion_titulo_signatura_nivel_clasificacion(id_nivel_jerarquia_nodo_padre, _
                                                                                               titulo_clasificacion, _
                                                                                               signatura)
            If Result <> "YES" Then
                Asgina_datos_interface_edita_nivel = Result
                Exit Function
            End If
            Asgina_datos_interface_edita_nivel = "YES"
        Catch ex As Exception
            Asgina_datos_interface_edita_nivel = "Inconsistencia general función Asgina_datos_interface_edita_nivel " & ex.Message
        End Try
    End Function
    Function Actualiza_nivel_cuadro_clasificacion_documental(ByVal id_registro_jerarquia As Integer, _
                                                             ByVal titulo_cuadro_clasificacion As String, _
                                                             ByVal signatura As String, _
                                                             ByRef ref_triview_node As TreeNode) As String

        Dim ref_des_niv_jerarquia As New Class_ra_de_descripcion_niveles_jerarquia
        Dim Result As String = ""
        Dim id_nivel_nivel_clasficacion As Integer = 0
        Dim nombre_nivel_clasificacion_jerarquia As String = ""
        Result = ref_des_niv_jerarquia.Retorna_id_nombre_nivel_descripcion_por_id_jerarquia(id_registro_jerarquia, _
                                                                                            id_nivel_nivel_clasficacion, _
                                                                                            nombre_nivel_clasificacion_jerarquia)
        If Result <> "YES" Then
            Actualiza_nivel_cuadro_clasificacion_documental = Result
            Exit Function
        End If
        Dim ref_titulo_cuadro_clasificacion As String = ""
        Dim ref_signatura As String = ""
        If titulo_cuadro_clasificacion = "" Then
            ref_titulo_cuadro_clasificacion = "Null"
        Else
            ref_titulo_cuadro_clasificacion = "'" & titulo_cuadro_clasificacion & "'"
        End If
        If signatura = "" Then
            ref_signatura = "Null"
        Else
            ref_signatura = "'" & signatura & "'"
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"

        Try

            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            Dim sqlinsertcion As String = ""
            myCommand.CommandText = sqlinsertcion
            Dim Switc As Integer = 0
            '---------------------------------------
            'Actualiza descripción jerarquía
            '---------------------------------------
            sqlinsertcion = "Update ra_de_descripcion_niveles_jerarquia set TITULO=" & ref_titulo_cuadro_clasificacion & _
                ",SIGNATURA=" & ref_signatura & " where RA_DE_REGISTRO_JERARQUIA_ID_REGISTRO_JERARQUIA=" & id_registro_jerarquia
            myCommand.CommandText = sqlinsertcion
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Actualiza_nivel_cuadro_clasificacion_documental = "Imposible Actualizar descripcion de jeraquía del nivel de jerarquía  : " & Left(sqlinsertcion, 30)
                myTrans.Rollback()
                myConnection.Close()
                errorM = "Imposible actualizar descripcion de jeraquía del nivel de jerarquía  : " & Left(sqlinsertcion, 30)
                Exit Function
            End If
            Dim Trednode As New TreeNode
            ref_triview_node.Text = UCase(titulo_cuadro_clasificacion & " /  " & signatura & " (" & "" & " - " & "" & ")" & " / " & " / " & nombre_nivel_clasificacion_jerarquia)
            myTrans.Commit()
            myConnection.Close()
            Actualiza_nivel_cuadro_clasificacion_documental = "YES"
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                'myTrans.Rollback()
                myConnection.Close()
                Actualiza_nivel_cuadro_clasificacion_documental = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Actualiza_nivel_cuadro_clasificacion_documental = errorM

        End Try
    End Function
    Function Agregar_nivel_cuadro_clasficion_documental(ByVal nombre_nivel_clasificacion_nodo_hijo As String, _
                                                        ByVal id_nivel_jerarquia_nodo_padre As Integer, _
                                                        ByVal titulo_cuadro_clasificacion As String, _
                                                        ByVal signatura As String, _
                                                        ByRef ref_triview_node As TreeNode, _
                                                        ByRef ref_treview As TreeView) As String
        '-------------------------------------------
        'Solicita nombre nivél y ide de descripción
        'del nodo padre
        '-------------------------------------------
        Dim nombre_nivel_clasificacion_nodo_padre As String = ""
        Dim id_nivel_clasificacion_nodo_padre As Integer = 0
        Dim id_cuadro_clasficacion As Integer = 0
        Dim Result As String = ""
        Dim Refclas_registro_jerarquia As New Class_ra_de_registro_jerarquia
        Result = Refclas_registro_jerarquia.Retorna_id_cuadro_registro_jerarquia(id_nivel_jerarquia_nodo_padre, _
                                                                                 id_cuadro_clasficacion)
        If Result <> "YES" Then
            Agregar_nivel_cuadro_clasficion_documental = Result
            Exit Function
        End If
        Dim ref_des_niv_jerarquia As New Class_ra_de_descripcion_niveles_jerarquia
        Result = ref_des_niv_jerarquia.Retorna_id_nombre_nivel_descripcion_por_id_jerarquia(id_nivel_jerarquia_nodo_padre, _
                                                                                            id_nivel_clasificacion_nodo_padre, _
                                                                                            nombre_nivel_clasificacion_nodo_padre)
        If Result <> "YES" Then
            Agregar_nivel_cuadro_clasficion_documental = Result
            Exit Function
        End If
        '--------------------------------------------
        'Solicita el id de descripción del nodo hijo
        '--------------------------------------------
        Dim id_nivel_clasificacion_nodo_hijo As Integer = 0
        Dim Class_ra_de_niveles_descripcion As New Class_ra_de_niveles_descripcion
        Result = Class_ra_de_niveles_descripcion.Solicita_id_nivel_de_clasificacion(nombre_nivel_clasificacion_nodo_hijo, _
                                                                                    id_nivel_clasificacion_nodo_hijo)
        If Result <> "YES" Then
            Agregar_nivel_cuadro_clasficion_documental = Result
            Exit Function
        End If
        '--------------------------------------------
        'Solicita las restricciones para el nivel de 
        'clasficación dentro de la jerarquía
        '--------------------------------------------
        Result = Me.Restriciones_registro_nivel_clasificacion(id_nivel_clasificacion_nodo_padre, _
                                                              nombre_nivel_clasificacion_nodo_padre, _
                                                              id_nivel_clasificacion_nodo_hijo, _
                                                              nombre_nivel_clasificacion_nodo_hijo)
        If Result <> "YES" Then
            Agregar_nivel_cuadro_clasficion_documental = Result
            Exit Function
        End If
        Dim ref_titulo_cuadro_clasificacion As String = ""
        Dim ref_signatura As String = ""
        If titulo_cuadro_clasificacion = "" Then
            ref_titulo_cuadro_clasificacion = "Null"
        Else
            ref_titulo_cuadro_clasificacion = "'" & titulo_cuadro_clasificacion & "'"
        End If
        If signatura = "" Then
            ref_signatura = "Null"
        Else
            ref_signatura = "'" & signatura & "'"
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"

        Try

            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            Dim sqlinsertcion As String = ""
            myCommand.CommandText = sqlinsertcion
            Dim Switc As Integer = 0
            '-----------------------------------
            'Registra nivel de jerarquia
            '-----------------------------------
            sqlinsertcion = " Insert into ra_de_registro_jerarquia (RA_DE_NIVELES_DESCRIPCION_ID_NIVELES_DESCRIPCION,RA_DE_CUADRO_CLASIFICACION_ID_DE_CUADRO_CLASIFICACION) values (" & _
                "'" & id_nivel_clasificacion_nodo_hijo & "','" & id_cuadro_clasficacion & "')"
            myCommand.CommandText = sqlinsertcion
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Agregar_nivel_cuadro_clasficion_documental = "Imposible registrar nivel de jeraquía   : " & Left(sqlinsertcion, 30)
                'myTrans.Rollback()
                myConnection.Close()
                errorM = "Imposible registrar nivel de jeraquía cuadro de clasificación documental  : " & Left(sqlinsertcion, 30)
                Exit Function
            End If
            Dim id_nivel_de_jeraquia_hijo As Object = myCommand.LastInsertedId
            '--------------------------------------
            'Registra la relaciones de jerarquia
            '--------------------------------------
            sqlinsertcion = " Insert into ra_de_registro_relaciones_jerarquia (RA_DE_CUADRO_CLASIFICACION_ID_DE_CUADRO_CLASIFICACION,ID_REGISTRO_JERARQUIA_PADRE,ID_REGISTRO_JERARQUIA_HIJO) values (" & _
                "'" & id_cuadro_clasficacion & "','" & id_nivel_jerarquia_nodo_padre & "','" & id_nivel_de_jeraquia_hijo & "')"
            myCommand.CommandText = sqlinsertcion
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Agregar_nivel_cuadro_clasficion_documental = "Imposible registrar relación de jearquía  : " & Left(sqlinsertcion, 30)
                myTrans.Rollback()
                myConnection.Close()
                errorM = "Imposible registrar relación de jearquia  : " & Left(sqlinsertcion, 30)
                Exit Function
            End If
            '---------------------------------------
            'Registra descripción jerarquía
            '---------------------------------------
            sqlinsertcion = " Insert into ra_de_descripcion_niveles_jerarquia (RA_DE_REGISTRO_JERARQUIA_ID_REGISTRO_JERARQUIA,TITULO,SIGNATURA) values (" & _
               "'" & id_nivel_de_jeraquia_hijo & "'," & ref_titulo_cuadro_clasificacion & "," & ref_signatura & ")"
            myCommand.CommandText = sqlinsertcion
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Agregar_nivel_cuadro_clasficion_documental = "Imposible registrar descripcion de jeraquía del nivel de jerarquía  : " & Left(sqlinsertcion, 30)
                myTrans.Rollback()
                myConnection.Close()
                errorM = "Imposible registrar descripcion de jeraquía del nivel de jerarquía  : " & Left(sqlinsertcion, 30)
                Exit Function
            End If
            Dim Trednode As New TreeNode
            Trednode.Text = UCase(titulo_cuadro_clasificacion & " /  " & signatura & " (" & "" & " - " & "" & ")" & " / " & " / " & nombre_nivel_clasificacion_nodo_hijo)
            Trednode.Value = id_nivel_de_jeraquia_hijo & "|" & nombre_nivel_clasificacion_nodo_hijo
            ref_treview.SelectedNode.ChildNodes.Add(Trednode)
            myTrans.Commit()
            myConnection.Close()
            Agregar_nivel_cuadro_clasficion_documental = "YES"
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                'myTrans.Rollback()
                myConnection.Close()
                Agregar_nivel_cuadro_clasficion_documental = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Agregar_nivel_cuadro_clasficion_documental = errorM

        End Try
    End Function
    Function Actualiza_cuadro_clasficacion_documental(ByVal nombre_empresa_clasificacion As String, _
                                                      ByVal codigo_pais As String, _
                                                      ByVal fecha_extrema_inicial As String, _
                                                      ByVal fecha_extrema_final As String, _
                                                      ByRef drop_list As DropDownList, _
                                                      ByRef ref_triview As TreeView, _
                                                      ByVal id_registro_jerarquia As Integer, _
                                                      ByVal id_empresa As Integer, _
                                                      ByVal id_organigrama As Integer) As String
        If nombre_empresa_clasificacion = "" Then
            Actualiza_cuadro_clasficacion_documental = "Por favor seleccione el nombre de la entidad/empresa de la estructura de clasificación "
            Exit Function
        End If
        If codigo_pais = "" Then
            Actualiza_cuadro_clasficacion_documental = "Por favor seleccione el código de la estructura de clasificación "
            Exit Function
        End If
        If fecha_extrema_inicial = "" Then
            Actualiza_cuadro_clasficacion_documental = "Por favor seleccione la fecha extrema inicial de la estructura de clasificación "
            Exit Function
        End If
        If fecha_extrema_final = "" Then
            Actualiza_cuadro_clasficacion_documental = "Por favor seleccione la fecha fecha_extrema_final de la estructura de clasificación "
            Exit Function
        End If
        If id_organigrama = 0 Then
            Actualiza_cuadro_clasficacion_documental = "Por favor seleccione la estructura organica del cuadro de clasificación "
            Exit Function
        End If
        If id_empresa = 0 Then
            Actualiza_cuadro_clasficacion_documental = "Por favor seleccione la entidad del cuadro de clasificación "
            Exit Function
        End If
        Dim Result As String = ""
        Dim Class_ra_de_registro_jerarquia As New Class_ra_de_registro_jerarquia
        Dim Class_ra_de_cuadro_clasificacion As New Class_ra_de_cuadro_clasificacion
        Dim stru_clasficacion As stru_ra_de_cuadro_clasificacion
        Dim id_cuadro_clasificacion As Integer = 0
        Result = Class_ra_de_registro_jerarquia.Retorna_id_cuadro_registro_jerarquia(id_registro_jerarquia, _
                                                                                     id_cuadro_clasificacion)
        If Result <> "YES" Then
            Actualiza_cuadro_clasficacion_documental = Result
            Exit Function
        End If
        Result = Class_ra_de_cuadro_clasificacion.Solicita_datos_estructura_cuadro(id_cuadro_clasificacion, _
                                                                                   stru_clasficacion)
        If Result <> "YES" Then
            Actualiza_cuadro_clasficacion_documental = Result
            Exit Function
        End If
        Dim existencia As String = ""
        If stru_clasficacion.registro_organigrama_ID_ORGANIGRAMA <> id_organigrama Then
            Result = Me.Solicita_existencia_cuadro_clasficacion(id_empresa, _
                                                                id_organigrama, _
                                                                existencia)
            If Result <> "YES" Then
                Actualiza_cuadro_clasficacion_documental = Result
                Exit Function
            End If
            If existencia = "YES" Then
                Actualiza_cuadro_clasficacion_documental = "Ya se enecuentra registrado un cuadro de clasificacion para la entidad y el organigrama informado"
                Exit Function
            End If
        End If  
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Dim date1al As String = Date.Today
        Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            Actualiza_cuadro_clasficacion_documental = Result
            Exit Function
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            Dim sqlinsertcion As String = " Update ra_de_cuadro_clasificacion set CODIGO_CUADRO='" & codigo_pais & "', NOMBRE_ENTIDAD_CLASFICACION='" & nombre_empresa_clasificacion & "'," & _
                "FECHA_INICIAL='" & fecha_extrema_inicial & "', FECHA_FINAL='" & fecha_extrema_final & "' where ID_DE_CUADRO_CLASIFICACION='" & id_cuadro_clasificacion & "'"
            myCommand.CommandText = sqlinsertcion
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Actualiza_cuadro_clasficacion_documental = "Imposible actualizar el cuadro de clasificación  : " & Left(sqlinsertcion, 30)
                myConnection.Close()
                errorM = "Imposible actualizar el cuadro de clasificación  : " & Left(sqlinsertcion, 30)
                Exit Function
            End If
            '-----------------------------------
            'Registra log
            '-----------------------------------
            Dim campos_cambio As String = "CAMPO|CONTENIDO ANTIGUO|CAMPO|NUEVO CONTENIDO" & vbCrLf & _
                "NOMBRE_ENTIDAD_CLASFICACION|" & stru_clasficacion.NOMBRE_ENTIDAD_CLASFICACION & "|NOMBRE_ENTIDAD_CLASFICACION|" & nombre_empresa_clasificacion & vbCrLf & _
                "CODIGO_CUADRO|" & stru_clasficacion.CODIGO_CUADRO & "|CODIGO_CUADRO|" & codigo_pais & vbCrLf & _
                "FECHA_INICIAL|" & stru_clasficacion.FECHA_INICIAL & "|FECHA_INICIAL|" & fecha_extrema_inicial & vbCrLf & _
                "FECHA_FINAL|" & stru_clasficacion.FECHA_FINAL & "|FECHA_FINAL|" & fecha_extrema_final & vbCrLf
            Dim hor As String = Now
            Dim sqlforupdate As String = "INSERT INTO ra_log_organizacion_documental (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_UNIDAD" & _
            ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values (" & _
            "'EDITA CUADRO CLASIFICACION','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "','" & _
            id_cuadro_clasificacion & "','" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','GESTOR WEB','" & campos_cambio & "')"
            myCommand.CommandText = sqlforupdate
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Actualiza_cuadro_clasficacion_documental = "Imposible registrar el log de transacción  : " & Left(sqlforupdate, 30)
                myTrans.Rollback()
                myConnection.Close()
                errorM = "Imposible registrar el log de transacción  : " & Left(sqlforupdate, 30)
                Exit Function
            End If
            ref_triview.SelectedNode.Text = UCase(codigo_pais & " /  " & nombre_empresa_clasificacion & " (" & fecha_extrema_inicial & " - " & fecha_extrema_final & ")")
            myTrans.Commit()
            myConnection.Close()
            Actualiza_cuadro_clasficacion_documental = "YES"
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                'myTrans.Rollback()
                myConnection.Close()
                Actualiza_cuadro_clasficacion_documental = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Actualiza_cuadro_clasficacion_documental = errorM

        End Try
    End Function
    Function Crear_Cuadro_de_clasficacion_documental(ByVal nombre_empresa_clasificacion As String, _
                                                     ByVal codigo_pais As String, _
                                                     ByVal fecha_extrema_inicial As String, _
                                                     ByVal fecha_extrema_final As String, _
                                                     ByVal id_empresa As Integer, _
                                                     ByVal id_organigrama As Integer, _
                                                     ByRef ref_triview As TreeView) As String
        If nombre_empresa_clasificacion = "" Then
            Crear_Cuadro_de_clasficacion_documental = "Por favor seleccione el nombre de la entidad/empresa de la estructura de clasificación "
            Exit Function
        End If
        If codigo_pais = "" Then
            Crear_Cuadro_de_clasficacion_documental = "Por favor seleccione el código de la estructura de clasificación "
            Exit Function
        End If
        If fecha_extrema_inicial = "" Then
            Crear_Cuadro_de_clasficacion_documental = "Por favor seleccione la fecha extrema inicial de la estructura de clasificación "
            Exit Function
        End If
        If fecha_extrema_final = "" Then
            Crear_Cuadro_de_clasficacion_documental = "Por favor seleccione la fecha fecha_extrema_final de la estructura de clasificación "
            Exit Function
        End If
        If id_organigrama = 0 Then
            Crear_Cuadro_de_clasficacion_documental = "Por favor seleccione la estructura organica del cuadro de clasificación "
            Exit Function
        End If
        If id_empresa = 0 Then
            Crear_Cuadro_de_clasficacion_documental = "Por favor seleccione la entidad del cuadro de clasificación "
            Exit Function
        End If
        ''-----------------------------------------------------
        ''Verifica la existencia del cuadro de clasificación
        ''-----------------------------------------------------
        Dim Result As String = ""
        Dim Existencia As String = ""
        Result = Me.Solicita_existencia_cuadro_clasficacion(id_empresa, _
                                                          id_organigrama, _
                                                          Existencia)
        If Result <> "YES" Then
            Crear_Cuadro_de_clasficacion_documental = Result
            Exit Function
        End If
        If Existencia = "YES" Then
            Crear_Cuadro_de_clasficacion_documental = "Ya se enecuentra registrado un cuadro de clasificacion para la entidad y el organigrama informado"
            Exit Function
        End If
        '--------------------------------------------------------------------------
        'Retorna el nivel de descripción del cuadro de clasificación documental
        '--------------------------------------------------------------------------
        Dim id_nivel_descripcion As Integer = 0
        Dim Class_ra_de_niveles_descripcion As New Class_ra_de_niveles_descripcion
        Result = Class_ra_de_niveles_descripcion.Solicita_id_nivel_de_clasificacion("Cuadro de Clasificación documental", _
                                                                                    id_nivel_descripcion)
        If Result <> "YES" Then
            Crear_Cuadro_de_clasficacion_documental = Result
            Exit Function
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            Dim sqlinsertcion As String = " Insert into ra_de_cuadro_clasificacion (CODIGO_CUADRO,NOMBRE_ENTIDAD_CLASFICACION,FECHA_INICIAL,FECHA_FINAL" & _
                ",empresa_gestion_documental_ID_EMPRESA,registro_organigrama_ID_ORGANIGRAMA) " & _
                " values (" & _
                "'" & codigo_pais & "','" & nombre_empresa_clasificacion & "','" & fecha_extrema_inicial & "','" & fecha_extrema_final & "'," & _
                id_empresa & "," & id_organigrama & ")"
            myCommand.CommandText = sqlinsertcion
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Crear_Cuadro_de_clasficacion_documental = "Imposible registrar el cuadro de clasficación documental  : " & Left(sqlinsertcion, 30)
                'myTrans.Rollback()
                myConnection.Close()
                errorM = "Imposible registrar unidad de conservacion  : " & sqlinsertcion
                Exit Function
            End If
            Dim id_cuadro_clasificacion As Object = myCommand.LastInsertedId
            '-----------------------------------
            'Registra nivel de jerarquia
            '-----------------------------------
            sqlinsertcion = " Insert into ra_de_registro_jerarquia (RA_DE_NIVELES_DESCRIPCION_ID_NIVELES_DESCRIPCION,RA_DE_CUADRO_CLASIFICACION_ID_DE_CUADRO_CLASIFICACION) values (" & _
                "'" & id_nivel_descripcion & "','" & id_cuadro_clasificacion & "')"
            myCommand.CommandText = sqlinsertcion
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Crear_Cuadro_de_clasficacion_documental = "Imposible registrar nivel de jeraquía cuadro de clasificación documental  : " & Left(sqlinsertcion, 30)
                myTrans.Rollback()
                myConnection.Close()
                errorM = "Imposible registrar nivel de jeraquía cuadro de clasificación documental  : " & Left(sqlinsertcion, 30)
                Exit Function
            End If
            Dim id_nivel_de_jeraquia As Object = myCommand.LastInsertedId
            '---------------------------------------
            'Registra descripción jerarquía
            '---------------------------------------
            sqlinsertcion = " Insert into ra_de_descripcion_niveles_jerarquia (RA_DE_REGISTRO_JERARQUIA_ID_REGISTRO_JERARQUIA,COD_REFERENCIA) values (" & _
               "'" & id_nivel_de_jeraquia & "','" & codigo_pais & "')"
            myCommand.CommandText = sqlinsertcion
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Crear_Cuadro_de_clasficacion_documental = "Imposible registrar descripcion de jeraquía cuadro de clasificación documental  : " & Left(sqlinsertcion, 30)
                myTrans.Rollback()
                myConnection.Close()
                errorM = "Imposible registrar descripcion de jeraquia cuadro de clasificación documental  : " & Left(sqlinsertcion, 30)
                Exit Function
            End If
            'drop_list.Items.Add(nombre_empresa_clasificacion)
            'drop_list.Text = nombre_empresa_clasificacion
            Dim attrNode As New TreeNode
            attrNode.Text = UCase(codigo_pais & " /  " & nombre_empresa_clasificacion & " (" & fecha_extrema_inicial & " - " & fecha_extrema_final & ")")
            attrNode.Value = id_nivel_de_jeraquia & "|" & "CUADRO CLASIFICACION"
            attrNode.ToolTip = "Cuadro de clasificación documental "
            attrNode.ImageUrl = "../workflow/imageneswf/rectangle-portrait-light.png"
            ref_triview.Nodes.Add(attrNode)
            myTrans.Commit()
            myConnection.Close()
            Crear_Cuadro_de_clasficacion_documental = "YES"
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                'myTrans.Rollback()
                myConnection.Close()
                Crear_Cuadro_de_clasficacion_documental = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Crear_Cuadro_de_clasficacion_documental = errorM

        End Try
    End Function
    Function Solicita_existencia_cuadro_clasficacion(ByVal id_empresa As Integer, _
                                                     ByVal id_organigrama As Integer, _
                                                     ByRef estado_existencia As String) As String
        Try
            Dim Parametro_Consulta As String = "select  ID_DE_CUADRO_CLASIFICACION " & _
                     " from ra_de_cuadro_clasificacion where empresa_gestion_documental_ID_EMPRESA=" & id_empresa & _
                     " AND registro_organigrama_ID_ORGANIGRAMA=" & id_organigrama
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Dat_reader As DataSet = New DataSet("areas_depart_radicacion")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Solicita_existencia_cuadro_clasficacion = "Error conexión función Solicita_existencia_cuadro_clasficacion " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count = 0 Then
                estado_existencia = "NO"
                Solicita_existencia_cuadro_clasficacion = "YES"
                Exit Function
            Else
                estado_existencia = "YES"
                Solicita_existencia_cuadro_clasficacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_existencia_cuadro_clasficacion = "Inconsistencia general función Solicita_existencia_cuadro_clasficacion " & ex.Message
        End Try
    End Function
    Function Asigna_datos_interface_edicion_cuadro_clasificacion(ByVal id_registro_jerarquia As Integer, _
                                                                 ByRef text_fecha_extrema_inicial As TextBox, _
                                                                 ByVal text_fecha_extrema_final As TextBox, _
                                                                 ByRef drop_list_codigo As DropDownList, _
                                                                 ByRef drop_list_nombre_cuadro As DropDownList, _
                                                                 ByRef drop_list_empresa As DropDownList, _
                                                                 ByRef drop_list_organica As DropDownList,
                                                                 ByRef UpdatePanel As UpdatePanel) As String
        '--------------------------------------------------------
        'Función : Asigna datos de caracterización del cuadro
        'de clasificación a la interface
        'Fecha : 2017-01-13
        'Ing : Miguel Angel Urueta Miranda
        '--------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim id_cuadro_clasficacion As Integer = 0
            Dim id_nivel_descripcion As Integer = 0
            Dim Class_ra_de_cuadro_clasificacion As New Class_ra_de_cuadro_clasificacion
            Dim Class_empresa_gestion As New Class_empresa_gestion_documental
            Dim Class_registro_organigrama As New Class_registro_organigrama
            Dim Class_ra_de_registro_jerarquia As New Class_ra_de_registro_jerarquia
            Dim stru_clasficacion As stru_ra_de_cuadro_clasificacion
            Result = Class_ra_de_registro_jerarquia.Retorna_id_cuadro_registro_jerarquia(id_registro_jerarquia, _
                                                                                       id_cuadro_clasficacion)
            If Result <> "YES" Then
                Asigna_datos_interface_edicion_cuadro_clasificacion = Result
                Exit Function
            End If
            Result = Class_ra_de_cuadro_clasificacion.Solicita_datos_estructura_cuadro(id_cuadro_clasficacion, _
                                                                                       stru_clasficacion)
            If Result <> "YES" Then
                Asigna_datos_interface_edicion_cuadro_clasificacion = Result
                Exit Function
            End If
            Result = Class_empresa_gestion.Solicita_listado_empresa(stru_clasficacion.empresa_gestion_documental_ID_EMPRESA, _
                                                                  drop_list_empresa, _
                                                                  UpdatePanel)
            If Result <> "YES" Then
                Asigna_datos_interface_edicion_cuadro_clasificacion = Result
                Exit Function
            End If
            Result = Class_registro_organigrama.Listar_Organigramas_Empresa_Combo_Default_Items(stru_clasficacion.empresa_gestion_documental_ID_EMPRESA, _
                                                                                                stru_clasficacion.registro_organigrama_ID_ORGANIGRAMA, _
                                                                                                drop_list_organica, _
                                                                                                UpdatePanel)
            If Result <> "YES" Then
                Asigna_datos_interface_edicion_cuadro_clasificacion = Result
                Exit Function
            End If
            Dim Ref_clas_codigo_pais_3166 As New Class_Listar_Codigo_pais_3166
            Result = Ref_clas_codigo_pais_3166.Listar_Codigo_pais_3166_seleccion(drop_list_codigo, _
                                                                                 stru_clasficacion.CODIGO_CUADRO)
            If Result <> "YES" Then
                Asigna_datos_interface_edicion_cuadro_clasificacion = Result
                Exit Function
            End If
            Dim ref_ClassGestionFechas As New ClassGestionFechas
            Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(stru_clasficacion.FECHA_INICIAL)
            If Result <> "YES" Then
                Asigna_datos_interface_edicion_cuadro_clasificacion = Result
                Exit Function
            End If
            Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(stru_clasficacion.FECHA_FINAL)
            If Result <> "YES" Then
                Asigna_datos_interface_edicion_cuadro_clasificacion = Result
                Exit Function
            End If
            text_fecha_extrema_inicial.Text = stru_clasficacion.FECHA_INICIAL
            text_fecha_extrema_final.Text = stru_clasficacion.FECHA_FINAL
            Asigna_datos_interface_edicion_cuadro_clasificacion = "YES"
        Catch ex As Exception
            Asigna_datos_interface_edicion_cuadro_clasificacion = "Inconsistencia general función Asigna_datos_interface_edicion_cuadro_clasificacion " & ex.Message
        End Try
    End Function
    Function Listar_cuadro_clasificacion_documental(ByVal id_empresa As Integer, _
                                                    ByRef treeview As TreeView, _
                                                    ByVal estado_sub_elementos As Integer) As String
        '-----------------------------------------------------------
        'Función : Lista cuadro de clasificación documental
        'Fecha : 2017-13-01
        'Ing :Miguel Angel Urueta Miranda
        '-----------------------------------------------------------
        Try
            Dim id_registro_jerarquia As Integer = 0
            Dim id_nivel_descripcion As Integer = 0
            Dim id_cuadro_clasificacion As Integer = 0
            Dim Result As String = ""
            Dim Class_ra_de_niveles_descripcion As New Class_ra_de_niveles_descripcion
            Result = Class_ra_de_niveles_descripcion.Solicita_id_nivel_de_clasificacion("Cuadro de Clasificación documental", _
                                                                                        id_nivel_descripcion)
            If Result <> "YES" Then
                Listar_cuadro_clasificacion_documental = Result
                Exit Function
            End If
            Dim Class_ra_de_cuadro_clasificacion As New Class_ra_de_cuadro_clasificacion
            Dim stru_cuadro() As stru_ra_de_cuadro_clasificacion = Nothing
            Result = Class_ra_de_cuadro_clasificacion.Solicita_matriz_estructuras_cuadro_clasficacion(id_empresa, _
                                                                                                     stru_cuadro)
            If Result <> "YES" Then
                Listar_cuadro_clasificacion_documental = Result
                Exit Function
            End If
            If stru_cuadro Is Nothing Then
                treeview.Nodes.Clear()
                Listar_cuadro_clasificacion_documental = "YES"
                Exit Function
            End If
            For i As Integer = 0 To stru_cuadro.Length - 1
                Result = Me.Retorna_registro_jerarquia_cuadro_clasificacion(stru_cuadro(i).ID_DE_CUADRO_CLASIFICACION, _
                                                                            id_nivel_descripcion, _
                                                                            id_registro_jerarquia)
                If Result <> "YES" Then
                    Listar_cuadro_clasificacion_documental = Result
                    Exit Function
                End If
                Dim Trenode_cuadro As New TreeNode
                Result = Me.Listar_cuadro_clasifciacion_documental_treview_consulta(stru_cuadro(i).NOMBRE_ENTIDAD_CLASFICACION, _
                                                                           stru_cuadro(i).CODIGO_CUADRO, _
                                                                           stru_cuadro(i).FECHA_INICIAL, _
                                                                           stru_cuadro(i).FECHA_FINAL, _
                                                                           id_registro_jerarquia, _
                                                                           treeview, _
                                                                           Trenode_cuadro)
                If Result <> "YES" Then
                    Listar_cuadro_clasificacion_documental = Result
                    Exit Function
                End If
                Dim stru() As stru_ra_de_registro_relaciones_jerarquia = Nothing
                Dim Ref_calss As New Class_ra_de_registro_relaciones_jerarquia
                If estado_sub_elementos = 1 Then
                    Result = Ref_calss.Solicita_niveles_rleacionados_a_cuadro_de_clasificacion(stru_cuadro(i).ID_DE_CUADRO_CLASIFICACION, _
                                                                                          stru)
                    If Result <> "YES" Then
                        Listar_cuadro_clasificacion_documental = Result
                        Exit Function
                    End If
                    If Not stru Is Nothing Then
                        Dim value_node As String = ""
                        For z As Integer = 0 To stru.Length - 1
                            Dim trenode_hijo As TreeNode
                            If z = 0 Then
                                Result = Listar_niveles_cuadro_clasficacion_documental_treview(stru(z).ID_REGISTRO_JERARQUIA_PADRE, _
                                                                                               stru(z).ID_REGISTRO_JERARQUIA_HIJO, _
                                                                                               stru_cuadro(i).ID_DE_CUADRO_CLASIFICACION, _
                                                                                              Trenode_cuadro, _
                                                                                              value_node)
                            Else
                                trenode_hijo = Nothing
                                trenode_hijo = treeview.FindNode(value_node)
                                If Not trenode_hijo Is Nothing Then
                                    Result = Listar_niveles_cuadro_clasficacion_documental_treview(stru(z).ID_REGISTRO_JERARQUIA_PADRE, _
                                                                                                   stru(z).ID_REGISTRO_JERARQUIA_HIJO, _
                                                                                                   stru_cuadro(i).ID_DE_CUADRO_CLASIFICACION, _
                                                                                                   trenode_hijo, _
                                                                                                   value_node)
                                End If

                            End If
                        Next
                    End If
                End If
                
            Next

            Listar_cuadro_clasificacion_documental = "YES"
        Catch ex As Exception
            Listar_cuadro_clasificacion_documental = "Inconsistencia general función Listar_cuadro_clasificacion_documental " & ex.Message
        End Try
    End Function
    Function Listar_cuadro_clasifciacion_documental_treview(ByVal nombre_empresa_clasificacion As String, _
                                                            ByVal codigo_pais As String, _
                                                            ByVal fecha_extrema_inicial As String, _
                                                            ByVal fecha_extrema_final As String, _
                                                            ByVal id_registro_jerarquia As Integer, _
                                                            ByRef Treview As TreeView) As String
        '-----------------------------------------------------------
        'Función : Lista cuadro de clasificación documental en la
        'interface treview
        'Fecha : 2017-13-01
        'Ing :Miguel Angel Urueta Miranda
        '-----------------------------------------------------------
        Try
            Dim attrNode As New TreeNode
            attrNode.Text = UCase(codigo_pais & " /  " & nombre_empresa_clasificacion & " (" & fecha_extrema_inicial & " - " & fecha_extrema_final & ")")
            attrNode.Value = id_registro_jerarquia
            attrNode.ToolTip = "Cuadro de clasificación documental "
            attrNode.ImageUrl = "../workflow/imageneswf/rectangle-portrait-light.png"
            Treview.Nodes.Add(attrNode)
            Listar_cuadro_clasifciacion_documental_treview = "YES"
        Catch ex As Exception
            Listar_cuadro_clasifciacion_documental_treview = "Inconsistencia general función Listar_cuadro_clasifciacion_documental_treview " & ex.Message
        End Try
    End Function
    Function Listar_cuadro_clasifciacion_documental_treview_consulta(ByVal nombre_empresa_clasificacion As String, _
                                                                     ByVal codigo_pais As String, _
                                                                     ByVal fecha_extrema_inicial As String, _
                                                                     ByVal fecha_extrema_final As String, _
                                                                     ByVal id_registro_jerarquia As Integer, _
                                                                     ByRef Treview As TreeView, _
                                                                     ByRef attrNode As TreeNode) As String
        '-----------------------------------------------------------
        'Función : Lista cuadro de clasificación documental en la
        'interface treview
        'Fecha : 2017-13-01
        'Ing :Miguel Angel Urueta Miranda
        '-----------------------------------------------------------
        Try
            'Treview.Nodes.Clear()
            attrNode = New TreeNode
            attrNode.Text = UCase(codigo_pais & " /  " & nombre_empresa_clasificacion & " (" & fecha_extrema_inicial & " - " & fecha_extrema_final & ")")
            attrNode.Value = id_registro_jerarquia & "|" & "CUADRO CLASIFICACION"
            attrNode.ToolTip = "Cuadro de clasificación documental "
            attrNode.ImageUrl = "../workflow/imageneswf/rectangle-portrait-light.png"
            Treview.Nodes.Add(attrNode)
            Listar_cuadro_clasifciacion_documental_treview_consulta = "YES"
        Catch ex As Exception
            Listar_cuadro_clasifciacion_documental_treview_consulta = "Inconsistencia general función Listar_cuadro_clasifciacion_documental_treview " & ex.Message
        End Try
    End Function
    Function Listar_niveles_cuadro_clasficacion_documental_treview(ByVal id_nivel_jerarquia_padre As Integer, _
                                                                   ByVal id_nivel_hijo As Integer, _
                                                                   ByVal id_cuadro_clasficacion As Integer, _
                                                                   ByRef Treview_node_padre As TreeNode, _
                                                                   ByRef value_node As String) As String
        Try
            Dim Result As String = ""
            Dim Class_ra_de_descripcion_niveles_jerarquia As New Class_ra_de_descripcion_niveles_jerarquia
            Dim Ref As New Class_ra_de_registro_jerarquia
            Dim nombre_nivel_clasificacion_jerarquia As String = ""
            Dim id_nivel_nivel_clasficacion As Integer = 0
            'Treview_node.ChildNodes.Clear()
            'Result = Ref.Retorna_id_cuadro_registro_jerarquia(id_nivel_jerarquia_padre, _
            '                                                 id_cuadro_clasficacion)
            'If Result <> "YES" Then
            '    Listar_niveles_cuadro_clasficacion_documental_treview = Result
            '    Exit Function
            'End If
            'Dim matri_id_registro_relacion_jerarquia() As Integer = Nothing
            'Dim ref_ra_de_re_jerarquia As New Class_ra_de_registro_jerarquia
            'Result = ref_ra_de_re_jerarquia.Retorna_listado_relaciones_jerarquia(id_nivel_jerarquia_padre, _
            '                                                                     matri_id_registro_relacion_jerarquia)
            'If Result <> "YES" Then
            '    Listar_niveles_cuadro_clasficacion_documental_treview = Result
            '    Exit Function
            'End If
            'If matri_id_registro_relacion_jerarquia Is Nothing Then
            '    Listar_niveles_cuadro_clasficacion_documental_treview = "YES"
            '    Exit Function
            'End If
            'For i As Integer = 0 To matri_id_registro_relacion_jerarquia.Length - 1
            Dim id_nivel_nivel_clasficacion_hijo As Integer = 0
            Dim nombre_nivel_clasificacion_jerarquia_hijo As String = ""
            Dim ref_des_niv_jerarquia As New Class_ra_de_descripcion_niveles_jerarquia
            Result = ref_des_niv_jerarquia.Retorna_id_nombre_nivel_descripcion_por_id_jerarquia(id_nivel_hijo, _
                                                                                                id_nivel_nivel_clasficacion, _
                                                                                                nombre_nivel_clasificacion_jerarquia_hijo)
            If Result <> "YES" Then
                Listar_niveles_cuadro_clasficacion_documental_treview = Result
                Exit Function
            Else
                Dim signatura As String = ""
                Dim titulo As String = ""

                Result = Class_ra_de_descripcion_niveles_jerarquia.Retorna_datos_de_descripcion_titulo_signatura_nivel_clasificacion(id_nivel_hijo, _
                                                                                                                                     titulo, _
                                                                                                                                     signatura)
                If Result <> "YES" Then
                    Listar_niveles_cuadro_clasficacion_documental_treview = Result
                    Exit Function
                End If
                Dim trenode_hijo As New TreeNode
                trenode_hijo.Text = UCase(titulo & " /  " & signatura & " (" & "" & " - " & "" & ")" & " / " & " / " & nombre_nivel_clasificacion_jerarquia_hijo)
                trenode_hijo.Value = id_nivel_hijo & "|" & nombre_nivel_clasificacion_jerarquia_hijo
                Treview_node_padre.ChildNodes.Add(trenode_hijo)
                value_node = trenode_hijo.ValuePath
                If nombre_nivel_clasificacion_jerarquia_hijo = "Sección y subsección" Then
                    trenode_hijo.ImageUrl = "../workflow/imageneswf/area-light.png"
                    trenode_hijo.ToolTip = "Representa al conjunto de áreas, dependencias o departamentos pertenecientes al cuadro de clasficación documental"
                End If
                If nombre_nivel_clasificacion_jerarquia_hijo = "Serie y subserie" Then
                    trenode_hijo.ImageUrl = "../workflow/imageneswf/list-alt-light.png"
                    trenode_hijo.ToolTip = "Representa al conjunto de asuntos o series pertenecientes al cuadro de clasficación documental"
                End If
                If nombre_nivel_clasificacion_jerarquia_hijo = "Unidad documental compuesta y simple (Expediente,Actas, decretos)" Then
                    trenode_hijo.ImageUrl = "../workflow/imageneswf/lista_sub_serie.png"
                    trenode_hijo.ToolTip = "Conjunto de carpetas o expedientes organizados por asuntos y temas especificos"
                End If
            End If

            'Next
            Listar_niveles_cuadro_clasficacion_documental_treview = "YES"
        Catch ex As Exception
            Listar_niveles_cuadro_clasficacion_documental_treview = "Inconsistencia general función Listar_niveles_cuadro_clasficacion_documental_treview " & ex.Message
        End Try
    End Function

    Function Listar_niveles_cuadro_clasficacion_documental_treview_Consulta(ByVal tag_nivel_clasficacion As String, _
                                                                            ByRef Treview_node As TreeNode, _
                                                                            ByRef HiddenEmailconsulta As Object, _
                                                                            ByRef reflabel As Label, _
                                                                            ByRef hideselecion As Object, _
                                                                            ByRef update As UpdatePanel, _
                                                                            ByRef grediview As GridView, _
                                                                            ByRef grediview_documentos As GridView, _
                                                                            ByRef reflabel_documento As Label, _
                                                                            ByRef update_documento As UpdatePanel, _
                                                                            ByRef UpdatePanel_title_expediente As UpdatePanel) As String
        Try
            Treview_node.ChildNodes.Clear()
            Dim Result As String = ""
            Dim nombre_nivel_clasificacion_jerarquia As String = ""
            Dim id_nivel_nivel_clasficacion As Integer = 0
            Dim id_cuadro_clasficacion As Integer = 0
            Dim id_nivel_jerarquia_padre As Integer = -1
            Dim split() As String = tag_nivel_clasficacion.Split("|")
            id_nivel_jerarquia_padre = Val(split(0))
            Dim ref_des_niv_jerarquia As New Class_ra_de_descripcion_niveles_jerarquia
            Dim re_Class_ra_de_registro_jerarquia As New Class_ra_de_registro_jerarquia
            Dim stru_clasficacion As stru_ra_de_cuadro_clasificacion
            Dim class_re_de_cuadro As New Class_ra_de_cuadro_clasificacion
            If id_nivel_jerarquia_padre <> -1 Then
                Result = re_Class_ra_de_registro_jerarquia.Retorna_id_cuadro_registro_jerarquia(id_nivel_jerarquia_padre, _
                                                                                                id_cuadro_clasficacion)
                If Result <> "YES" Then
                    Listar_niveles_cuadro_clasficacion_documental_treview_Consulta = Result
                    Exit Function
                End If
                Result = class_re_de_cuadro.Solicita_datos_estructura_cuadro(id_cuadro_clasficacion, _
                                                                             stru_clasficacion)
                If Result <> "YES" Then
                    Listar_niveles_cuadro_clasficacion_documental_treview_Consulta = Result
                    Exit Function
                End If
                Result = ref_des_niv_jerarquia.Retorna_id_nombre_nivel_descripcion_por_id_jerarquia(id_nivel_jerarquia_padre, _
                                                                                                    id_nivel_nivel_clasficacion, _
                                                                                                    nombre_nivel_clasificacion_jerarquia)
                If Result <> "YES" Then
                    Listar_niveles_cuadro_clasficacion_documental_treview_Consulta = Result
                    Exit Function
                End If
                ''----------------------------------------------------------
                ''Lista nodos tipo Fondo
                ''----------------------------------------------------------
                If nombre_nivel_clasificacion_jerarquia = "Fondo" Then
                    Result = Me.Lista_registro_jerarquia_fondo(id_nivel_jerarquia_padre, _
                                                               id_nivel_nivel_clasficacion, _
                                                               "Fondo", _
                                                               0, _
                                                               "", _
                                                               Treview_node)
                    If Result <> "YES" Then
                        Listar_niveles_cuadro_clasficacion_documental_treview_Consulta = Result
                        Exit Function
                    End If
                End If
                ''----------------------------------------------------------
                ''Lista nodos tipo cuadro clasificacion
                ''----------------------------------------------------------
                If nombre_nivel_clasificacion_jerarquia = "Cuadro de Clasificación documental" Then
                    Result = Me.Lista_registro_jerarquia_fondo(id_nivel_jerarquia_padre, _
                                                               id_nivel_nivel_clasficacion, _
                                                               "Cuadro de Clasificación documental", _
                                                               0, _
                                                               "", _
                                                               Treview_node)
                    If Result <> "YES" Then
                        Listar_niveles_cuadro_clasficacion_documental_treview_Consulta = Result
                        Exit Function
                    End If
                End If
                '-----------------------------------------------------------
                'Lista nodos session y sub sección 
                '-----------------------------------------------------------
                If nombre_nivel_clasificacion_jerarquia = "Sección y subsección" Then
                    Dim matri_id_registro_relacion_jerarquia() As Integer = Nothing
                    Dim ref_ra_de_re_jerarquia As New Class_ra_de_registro_jerarquia
                    Result = ref_ra_de_re_jerarquia.Retorna_listado_relaciones_jerarquia(id_nivel_jerarquia_padre, _
                                                                                         matri_id_registro_relacion_jerarquia)
                    If Result <> "YES" Then
                        Listar_niveles_cuadro_clasficacion_documental_treview_Consulta = Result
                        Exit Function
                    End If
                    Dim hijo_padre As String = ""
                    If Not matri_id_registro_relacion_jerarquia Is Nothing Then
                        For i As Integer = 0 To matri_id_registro_relacion_jerarquia.Length - 1
                            If i = 0 Then
                                hijo_padre = matri_id_registro_relacion_jerarquia(i)
                            Else
                                hijo_padre = hijo_padre & "-" & matri_id_registro_relacion_jerarquia(i)
                            End If
                        Next
                    End If
                    Result = Me.Lista_registro_jerarquia_areas("Sección", _
                                                               hijo_padre, _
                                                               id_nivel_jerarquia_padre, _
                                                               stru_clasficacion.registro_organigrama_ID_ORGANIGRAMA, _
                                                               Treview_node)
                    If Result <> "YES" Then
                        Listar_niveles_cuadro_clasficacion_documental_treview_Consulta = Result
                        Exit Function
                    End If
                End If
                '-----------------------------------------------------------
                'Lista nodos series y sub series 
                '-----------------------------------------------------------
                If nombre_nivel_clasificacion_jerarquia = "Serie y subserie" Then
                    Dim matri_id_registro_relacion_jerarquia() As Integer = Nothing
                    Dim ref_ra_de_re_jerarquia As New Class_ra_de_registro_jerarquia
                    Result = ref_ra_de_re_jerarquia.Retorna_listado_relaciones_jerarquia(id_nivel_jerarquia_padre, _
                                                                                         matri_id_registro_relacion_jerarquia)
                    If Result <> "YES" Then
                        Listar_niveles_cuadro_clasficacion_documental_treview_Consulta = Result
                        Exit Function
                    End If
                    Dim hijo_padre As String = ""
                    If Not matri_id_registro_relacion_jerarquia Is Nothing Then
                        For i As Integer = 0 To matri_id_registro_relacion_jerarquia.Length - 1
                            If i = 0 Then
                                hijo_padre = matri_id_registro_relacion_jerarquia(i)
                            Else
                                hijo_padre = hijo_padre & "-" & matri_id_registro_relacion_jerarquia(i)
                            End If
                        Next
                    End If
                    Result = Me.Lista_registro_jerarquia_series_sub_series("Serie", hijo_padre, id_nivel_jerarquia_padre, Val(split(2)), Treview_node)
                    If Result <> "YES" Then
                        Listar_niveles_cuadro_clasficacion_documental_treview_Consulta = Result
                        Exit Function
                    End If
                End If
                '-----------------------------------------------------------
                'Lista nodos series y sub series 
                '-----------------------------------------------------------
                If nombre_nivel_clasificacion_jerarquia = "Unidad documental compuesta y simple (Expediente,Actas, decretos)" Then
                    HttpContext.Current.Session.Item("SortExpression_expe_clasificacion") = "ID_EXPEDIENTE"
                    HttpContext.Current.Session.Item("SortDirection_expe_clasificacion") = "DESC"
                    Result = Me.Lista_jerarquia_expedientes_unidades_simples_por_serie_area(0, _
                                                                                            "", _
                                                                                            grediview, _
                                                                                            HiddenEmailconsulta, _
                                                                                            reflabel, _
                                                                                            hideselecion, _
                                                                                            update, _
                                                                                            grediview_documentos, _
                                                                                            reflabel_documento, _
                                                                                            update_documento, _
                                                                                            1, _
                                                                                            HttpContext.Current.Session.Item("SortExpression_expe_clasificacion"), _
                                                                                            HttpContext.Current.Session.Item("SortDirection_expe_clasificacion"), _
                                                                                            "", _
                                                                                            UpdatePanel_title_expediente)

                    If Result <> "YES" Then
                        Listar_niveles_cuadro_clasficacion_documental_treview_Consulta = Result
                        Exit Function
                    End If
                End If
            End If
            If id_nivel_jerarquia_padre = -1 Then
                If split(1) = "Sección" Then
                    Dim split_sesion() As String = split(3).Split("-")
                    If split_sesion Is Nothing Then
                        Listar_niveles_cuadro_clasficacion_documental_treview_Consulta = "YES"
                        Exit Function
                    End If
                    For i As Integer = 0 To split_sesion.Length - 1
                        Dim ref_id_nivel_nivel_clasficacion As Integer = 0
                        Dim ref_nombre_nivel_clasificacion_jerarquia As String = ""
                        Result = ref_des_niv_jerarquia.Retorna_id_nombre_nivel_descripcion_por_id_jerarquia(Val(split_sesion(i)), _
                                                                                                            ref_id_nivel_nivel_clasficacion, _
                                                                                                            ref_nombre_nivel_clasificacion_jerarquia)
                        If Result <> "YES" Then
                            Listar_niveles_cuadro_clasficacion_documental_treview_Consulta = Result
                            Exit Function
                        End If
                        If ref_nombre_nivel_clasificacion_jerarquia = "Serie y subserie" Then
                            Result = Me.Lista_registro_jerarquia_fondo(Val(split(4)), ref_id_nivel_nivel_clasficacion, ref_nombre_nivel_clasificacion_jerarquia, split(2), "0", Treview_node)
                            If Result <> "YES" Then
                                Listar_niveles_cuadro_clasficacion_documental_treview_Consulta = Result
                                Exit Function
                            End If
                        End If
                        If ref_nombre_nivel_clasificacion_jerarquia = "Unidad documental compuesta y simple (Expediente,Actas, decretos)" Then
                            'Result = Me.Lista_jerarquia_expedientes_unidades_simples_por_serie_area(Val(split(2)), split(1), grediview, HiddenEmailconsulta, reflabel, hideselecion, update, grediview_documentos, reflabel_documento, update_documento)
                            HttpContext.Current.Session.Item("SortExpression_expe_clasificacion") = "ID_EXPEDIENTE"
                            HttpContext.Current.Session.Item("SortDirection_expe_clasificacion") = "DESC"
                            Result = Me.Lista_jerarquia_expedientes_unidades_simples_por_serie_area(Val(split(2)), _
                                                                                                    split(1), _
                                                                                                    grediview, _
                                                                                                    HiddenEmailconsulta, _
                                                                                                    reflabel, _
                                                                                                    hideselecion, _
                                                                                                    update, _
                                                                                                    grediview_documentos, _
                                                                                                    reflabel_documento, _
                                                                                                    update_documento, _
                                                                                                    1, _
                                                                                                    HttpContext.Current.Session.Item("SortExpression_expe_clasificacion"), _
                                                                                                    HttpContext.Current.Session.Item("SortDirection_expe_clasificacion"), _
                                                                                                    "", _
                                                                                                    UpdatePanel_title_expediente)
                            If Result <> "YES" Then
                                Listar_niveles_cuadro_clasficacion_documental_treview_Consulta = Result
                                Exit Function
                            End If
                        End If
                    Next
                End If
                If split(1) = "Serie" Then
                    Dim split_sesion() As String = split(3).Split("-")
                    If split_sesion Is Nothing Then
                        Listar_niveles_cuadro_clasficacion_documental_treview_Consulta = "YES"
                        Exit Function
                    End If
                    For i As Integer = 0 To split_sesion.Length - 1
                        Dim ref_id_nivel_nivel_clasficacion As Integer = 0
                        Dim ref_nombre_nivel_clasificacion_jerarquia As String = ""
                        Result = ref_des_niv_jerarquia.Retorna_id_nombre_nivel_descripcion_por_id_jerarquia(Val(split_sesion(i)), _
                                                                                                            ref_id_nivel_nivel_clasficacion, _
                                                                                                            ref_nombre_nivel_clasificacion_jerarquia)
                        If Result <> "YES" Then
                            Listar_niveles_cuadro_clasficacion_documental_treview_Consulta = Result
                            Exit Function
                        End If
                        If ref_nombre_nivel_clasificacion_jerarquia = "Unidad documental compuesta y simple (Expediente,Actas, decretos)" Then
                            'Result = Me.Lista_jerarquia_expedientes_unidades_simples_por_serie_area(Val(split(2)), split(1), grediview, HiddenEmailconsulta, reflabel, hideselecion, update, grediview_documentos, reflabel_documento, update_documento)
                            HttpContext.Current.Session.Item("SortExpression_expe_clasificacion") = "ID_EXPEDIENTE"
                            HttpContext.Current.Session.Item("SortDirection_expe_clasificacion") = "DESC"
                            Result = Me.Lista_jerarquia_expedientes_unidades_simples_por_serie_area(Val(split(2)), _
                                                                                                    split(1), _
                                                                                                    grediview, _
                                                                                                    HiddenEmailconsulta, _
                                                                                                    reflabel, _
                                                                                                    hideselecion, _
                                                                                                    update, _
                                                                                                    grediview_documentos, _
                                                                                                    reflabel_documento, _
                                                                                                    update_documento, _
                                                                                                    1, _
                                                                                                    HttpContext.Current.Session.Item("SortExpression_expe_clasificacion"), _
                                                                                                    HttpContext.Current.Session.Item("SortDirection_expe_clasificacion"), _
                                                                                                    "", _
                                                                                                    UpdatePanel_title_expediente)
                            If Result <> "YES" Then
                                Listar_niveles_cuadro_clasficacion_documental_treview_Consulta = Result
                                Exit Function
                            End If
                        End If
                    Next
                End If
            End If
            Listar_niveles_cuadro_clasficacion_documental_treview_Consulta = "YES"
        Catch ex As Exception
            Listar_niveles_cuadro_clasficacion_documental_treview_Consulta = "Inconsistencia general función Listar_niveles_cuadro_clasficacion_documental_treview_Consulta " & ex.Message
        End Try
    End Function
    Function Lista_documentos_relacionados_a_unidad_documental(ByVal id_unidad_documental As Integer,
                                                               ByRef update As UpdatePanel,
                                                               ByRef hideselecion As Object,
                                                               ByRef HiddenEmailconsulta As Object,
                                                               ByRef grediview As GridView,
                                                               ByRef reflabel As Object,
                                                               ByRef ref_label_relacionado As Label,
                                                               ByRef up_date_relacionado As UpdatePanel,
                                                               ByVal valor_relacion_documento As String,
                                                               ByVal tipo_consulta As Integer,
                                                               ByRef colum_order_name As String,
                                                               ByRef order_colum As String,
                                                               ByVal valor_consulta As String,
                                                               ByRef UpdatePanel_documentos_exp_title As UpdatePanel) As String
        Try
            up_date_relacionado.Update()
            Dim sql_consulta As String = ""
            If tipo_consulta = 1 Then
                sql_consulta = "SELECT ID_DOCUMENTO_DOCUARCHI_ALMACEN AS ID_DOCUMENTO," &
                  "NOMBRE_GABINETE AS CONTENEDOR,CLASEDOCUMENTO,DESCRIPCION_TIPO_DOCUMENTO AS TIPOLOGIA,FECHA_DOCUMENTO,SEGUNDO_NOMBRE_DOCUMENTO as NOMBRE,NOMBRE_AREA_DEPARTAMENTO " &
                  "AS SECCION,SERIE_DOCUMENTO,SUBSERIE_DOCUMENTO, RADICADO_DOCUMENTO" &
                  " from registro_producion_documental where EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE=" & id_unidad_documental &
                   " order by " & colum_order_name & " " & order_colum
            End If
            If tipo_consulta = 2 Then
                sql_consulta = "SELECT ID_DOCUMENTO_DOCUARCHI_ALMACEN AS ID_DOCUMENTO," &
             "NOMBRE_GABINETE AS CONTENEDOR,CLASEDOCUMENTO,DESCRIPCION_TIPO_DOCUMENTO AS TIPOLOGIA,FECHA_DOCUMENTO,SEGUNDO_NOMBRE_DOCUMENTO as NOMBRE,NOMBRE_AREA_DEPARTAMENTO " &
             "AS SECCION,SERIE_DOCUMENTO,SUBSERIE_DOCUMENTO, RADICADO_DOCUMENTO " &
             " from registro_producion_documental where EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE=" & id_unidad_documental &
              " and (" &
               "  ID_DOCUMENTO_DOCUARCHI_ALMACEN like '%" & valor_consulta & "%'" &
               "  or  NOMBRE_GABINETE like '%" & valor_consulta & "%'" &
               "  or  CLASEDOCUMENTO like '%" & valor_consulta & "%'" &
               "  or  FECHA_DOCUMENTO like '%" & valor_consulta & "%'" &
               "  or  SEGUNDO_NOMBRE_DOCUMENTO like '%" & valor_consulta & "%'" &
               "  or  NOMBRE_AREA_DEPARTAMENTO like '%" & valor_consulta & "%'" &
               "  or  SERIE_DOCUMENTO like '%" & valor_consulta & "%'" &
               "  or  DESCRIPCION_TIPO_DOCUMENTO like '%" & valor_consulta & "%')" &
               "  order by " & colum_order_name & " " & order_colum
            End If
            If tipo_consulta = 3 Then
                sql_consulta = HttpContext.Current.Session.Item("GA_DATO_CONSULTA_doc_clasificacion")
            End If
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("registro_producion_documental")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_documentos_relacionados_a_unidad_documental = "Error listando datos " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                HiddenEmailconsulta.value = ""
                reflabel.Text = "Se encontraron 0 registro(s) de documento(s)"
                Datset.Tables(0).Rows.Add(Datset.Tables(0).NewRow)
                grediview.DataSource = Datset
                hideselecion.value = "0"
                grediview.DataBind()
                grediview.Rows(0).Visible = False
                UpdatePanel_documentos_exp_title.Update()
                update.Update()
                Lista_documentos_relacionados_a_unidad_documental = "YES"
                Exit Function
            Else
                HiddenEmailconsulta.value = sql_consulta
                reflabel.Text = "Se encontraron " & Datset.Tables(0).Rows.Count & " registro(s) de documento(s)"
                grediview.DataSource = Datset
                hideselecion.value = grediview.Rows.Count
                grediview.DataBind()
                UpdatePanel_documentos_exp_title.Update()
                update.Update()
                For i As Integer = 0 To grediview.Rows.Count - 1
                    grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                    Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                    Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-file-alt")
                    ihtml.Style.Add("color", "white")
                    Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-success btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Ver documentos")
                    ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(2).Text.ToString())
                    ahtml.Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "ver_doc")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)
                    divhtml.Style.Add("display", "inline-flex")
                    grediview.Rows(i).Cells(0).Controls.Add(divhtml)
                    For z As Integer = 0 To grediview.Rows(i).Cells.Count - 1
                        If z > 0 Then
                            grediview.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_cort_tr_flex")
                            grediview.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(event,this);")
                        End If
                    Next
                Next
                HttpContext.Current.Session.Item("Sort_matri_colum_doc_clasificacion") = {"OPCIONES", "ID_DOCUMENTO", "CONTENEDOR",
                                                                        "CLASEDOCUMENTO", "TIPOLOGIA", "FECHA_DOCUMENTO",
                                                                        "NOMBRE", "SECCION", "SERIE_DOCUMENTO", "SUBSERIE_DOCUMENTO",
                                                                         "RADICADO_DOCUMENTO"}
                HttpContext.Current.Session.Item("SortExpression_doc_clasificacion") = colum_order_name
                HttpContext.Current.Session.Item("SortDirection_doc_clasificacion") = order_colum
                HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_doc_clasificacion") = tipo_consulta
                HttpContext.Current.Session.Item("GA_DATO_CONSULTA_doc_clasificacion") = sql_consulta
                Dim Refclas As New ClassGredview
                Result = Refclas.add_clase_acender_decender(colum_order_name,
                                                            HttpContext.Current.Session.Item("Sort_matri_colum_doc_clasificacion"),
                                                            order_colum,
                                                            grediview)
                If Result <> "YES" Then
                    Lista_documentos_relacionados_a_unidad_documental = "Error add clase funcion  Lista_solictudes_compartidos_de_un_usuario " & Result
                    Exit Function
                End If
                Lista_documentos_relacionados_a_unidad_documental = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_documentos_relacionados_a_unidad_documental = "Inconsistencia general función Lista_documentos_relacionados_a_unidad_documental " & ex.Message
        End Try
    End Function
    Function Lista_documentos_relacionados_a_unidad_documental_post(ByRef update As UpdatePanel,
                                                                    ByRef hideselecion As Object,
                                                                    ByVal HiddenEmailconsulta As Object,
                                                                    ByRef grediview As GridView,
                                                                    ByRef reflabel As Object,
                                                                    ByRef UpdatePanel_documentos_exp_title As UpdatePanel) As String
        Try
            If HiddenEmailconsulta.value = "" Then
                Lista_documentos_relacionados_a_unidad_documental_post = "YES"
                Exit Function
            End If
            Dim sql_condicion As String = ""
            Dim sql_consulta As String = HiddenEmailconsulta.value
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("registro_producion_documental")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_documentos_relacionados_a_unidad_documental_post = "Error listando datos " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                HiddenEmailconsulta.value = ""
                reflabel.Text = "Se encontraron 0 registro(s) "
                grediview.DataSource = Nothing
                hideselecion.value = "-1"
                grediview.DataBind()
                UpdatePanel_documentos_exp_title.Update()
                update.Update()
                Lista_documentos_relacionados_a_unidad_documental_post = "YES"
                Exit Function
            Else
                HiddenEmailconsulta.value = sql_consulta
                reflabel.Text = "Se encontraron " & Datset.Tables(0).Rows.Count & " registro(s) "
                grediview.DataSource = Datset
                hideselecion.value = "-1"
                grediview.DataBind()
                UpdatePanel_documentos_exp_title.Update()
                update.Update()
                For i As Integer = 0 To grediview.Rows.Count - 1
                    grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(0).Text.ToString())
                Next
                Lista_documentos_relacionados_a_unidad_documental_post = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_documentos_relacionados_a_unidad_documental_post = "Inconsistencia general función Lista_documentos_relacionados_a_unidad_documental " & ex.Message
        End Try
    End Function

    Function Lista_jerarquia_expedientes_unidades_simples_por_serie_area_post(ByRef update As UpdatePanel, _
                                                                              ByRef hideselecion As Object, _
                                                                              ByRef HiddenEmailconsulta As Object, _
                                                                              ByRef grediview As GridView, _
                                                                              ByRef reflabel As Object) As String
        Try
            If HiddenEmailconsulta.value = "" Then
                Lista_jerarquia_expedientes_unidades_simples_por_serie_area_post = "YES"
                Exit Function
            End If
            Dim sql_consulta As String = HiddenEmailconsulta.value
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("radicado")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_jerarquia_expedientes_unidades_simples_por_serie_area_post = "Error listando datos " & Result
                Exit Function
            End If

            If Datset.Tables(0).Rows.Count = 0 Then
                HiddenEmailconsulta.value = ""
                reflabel.Text = "Se encontro " & Datset.Tables(0).Rows.Count & " registro(s) de expediente " &
                grediview.DataSource = Datset
                'grediview.DataKeyNames = DataKey
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                Lista_jerarquia_expedientes_unidades_simples_por_serie_area_post = "YES"
                Exit Function
            Else
                HiddenEmailconsulta.value = sql_consulta
                reflabel.Text = "Se encontro " & Datset.Tables(0).Rows.Count & " registro(s) de expediente "
                grediview.DataSource = Datset
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                For i As Integer = 0 To grediview.Rows.Count - 1
                    grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(0).Text.ToString())
                Next
                Lista_jerarquia_expedientes_unidades_simples_por_serie_area_post = "YES"
                Exit Function
            End If
            Lista_jerarquia_expedientes_unidades_simples_por_serie_area_post = "YES"
        Catch ex As Exception
            Lista_jerarquia_expedientes_unidades_simples_por_serie_area_post = "Inconsistencia funcion Lista_jerarquia_expedientes_unidades_simples_por_serie_area_post " & ex.Message
        End Try
    End Function
    Function Lista_jerarquia_expedientes_unidades_simples_por_serie_area(ByVal id_area_serie As Integer, _
                                                                         ByVal nombre_nivel As String, _
                                                                         ByRef grediview As GridView, _
                                                                         ByRef HiddenEmailconsulta As Object, _
                                                                         ByRef reflabel As Label, _
                                                                         ByRef hideselecion As Object, _
                                                                         ByRef update As UpdatePanel, _
                                                                         ByRef gred_view_documento As GridView, _
                                                                         ByRef label_documento As Label, _
                                                                         ByRef up_date_documento As UpdatePanel, _
                                                                         ByVal tipo_consulta As Integer, _
                                                                         ByRef colum_order_name As String, _
                                                                         ByRef order_colum As String, _
                                                                         ByVal valor_consulta As String, _
                                                                         ByRef UpdatePanel_title_expediente As UpdatePanel) As String
        Try
            gred_view_documento.DataSource = Nothing
            gred_view_documento.DataBind()
            label_documento.Text = ""
            Dim panel As Panel = gred_view_documento.Page.FindControl("Panel_busqueda")
            If Not panel Is Nothing Then
                panel.Visible = False
            End If
            up_date_documento.Update()
            Dim sql_condicion As String = ""
            Dim sql_consulta As String = ""
            If tipo_consulta = 1 Then
                If nombre_nivel = "Serie" Then
                    sql_condicion = " WHERE CODIGO_SERIE_TRD=" & id_area_serie '& " AND Estado_Publico_Sub_Expediente=1 "
                End If
                If nombre_nivel = "Sección" Then
                    sql_condicion = " WHERE CODIGO_AREA_TRD=" & id_area_serie & " or ID_SUB_AREA=" & id_area_serie '& " AND Estado_Publico_Sub_Expediente=1 "
                End If
                sql_consulta = "SELECT ID_EXPEDIENTE AS CODIGO,CODIGO_UNICO AS NOMBRE_UNIDAD,ALEAS_EXPEDIENTE as ALEAS," & _
                    "TEMA_EXPEDIENTE AS TEMA,TIPO_UNIDAD_CONSERVACION AS UNIDAD," & _
                    "FECHA_EXTREMA_INICIAL AS FECHA_INI,FECHA_EXTREMA_FINAL AS FECHA_FIN,rte.NOMBRE_TIPO_EXPEDIENTE AS TIPO,NOMBRE_TIPO_UNIDAD_DOCUMENTAL AS CLASE_UNIDAD,NUMERO_FOLIOS_CONTENIDOS as FOLIO_FISICO,NUMERO_ELECTRONICO_CONTENIDO" _
                    & " AS FOLIO_ELECTRONICO,NUMERO_DIGITALIZADO_CONTENIDO AS FOLIO_DIGITALIZADO,NOMBRE_SERIE_TRD,NOMBRE_SUBSERIE_TRD from expediente_archivo " & _
                    " left outer join ra_tipo_expediente as rte on (rte.ID_TIPO_EXPEDIENTE=RA_TIP_EXPE_ID_TIPO_EXPEDIENTE)" & _
                     sql_condicion & " order by " & colum_order_name & " " & order_colum

            Else
                If nombre_nivel = "Serie" Then
                    sql_condicion = " and CODIGO_SERIE_TRD=" & id_area_serie '& " AND Estado_Publico_Sub_Expediente=1 "
                End If
                If nombre_nivel = "Sección" Then
                    sql_condicion = " and CODIGO_AREA_TRD=" & id_area_serie & " or ID_SUB_AREA=" & id_area_serie '& " AND Estado_Publico_Sub_Expediente=1 "
                End If
                sql_consulta = "SELECT ID_EXPEDIENTE AS CODIGO,CODIGO_UNICO AS NOMBRE_UNIDAD,ALEAS_EXPEDIENTE as ALEAS," & _
                   "TEMA_EXPEDIENTE AS TEMA,TIPO_UNIDAD_CONSERVACION AS UNIDAD," & _
                   "FECHA_EXTREMA_INICIAL AS FECHA_INI,FECHA_EXTREMA_FINAL AS FECHA_FIN,rte.NOMBRE_TIPO_EXPEDIENTE AS TIPO,NOMBRE_TIPO_UNIDAD_DOCUMENTAL AS CLASE_UNIDAD,NUMERO_FOLIOS_CONTENIDOS as FOLIO_FISICO,NUMERO_ELECTRONICO_CONTENIDO" _
                   & " AS FOLIO_ELECTRONICO,NUMERO_DIGITALIZADO_CONTENIDO AS FOLIO_DIGITALIZADO,NOMBRE_SERIE_TRD,NOMBRE_SUBSERIE_TRD from expediente_archivo " & _
                   " left outer join ra_tipo_expediente as rte on (rte.ID_TIPO_EXPEDIENTE=RA_TIP_EXPE_ID_TIPO_EXPEDIENTE) " & _
                    " where (" & _
                    "  ID_EXPEDIENTE like '%" & valor_consulta & "%'" & _
                    "  or  CODIGO_UNICO like '%" & valor_consulta & "%'" & _
                    "  or  TEMA_EXPEDIENTE like '%" & valor_consulta & "%'" & _
                    "  or  TIPO_UNIDAD_CONSERVACION like '%" & valor_consulta & "%'" & _
                    "  or  FECHA_EXTREMA_INICIAL like '%" & valor_consulta & "%'" & _
                    "  or  FECHA_EXTREMA_FINAL like '%" & valor_consulta & "%'" & _
                    "  or  NOMBRE_TIPO_EXPEDIENTE like '%" & valor_consulta & "%'" & _
                    "  or  NOMBRE_TIPO_UNIDAD_DOCUMENTAL like '%" & valor_consulta & "%'" & _
                    "  or  NUMERO_FOLIOS_CONTENIDOS like '%" & valor_consulta & "%'" & _
                    "  or  NUMERO_ELECTRONICO_CONTENIDO like '%" & valor_consulta & "%') " & _
                     sql_condicion &
                    " order by " & colum_order_name & " " & order_colum
            End If
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("expediente_archivo")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_jerarquia_expedientes_unidades_simples_por_serie_area = "Error listando datos " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                HiddenEmailconsulta.value = ""
                reflabel.Text = "0 registro (s) de expediente (s)"
                grediview.DataSource = Nothing
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                UpdatePanel_title_expediente.Update()
                Lista_jerarquia_expedientes_unidades_simples_por_serie_area = "YES"
                Exit Function
            Else
                reflabel.Text = Datset.Tables(0).Rows.Count & " registro (s) de expediente (s)"
                grediview.DataSource = Datset
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                UpdatePanel_title_expediente.Update()
                For i As Integer = 0 To grediview.Rows.Count - 1
                    grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                    grediview.HeaderRow.Cells(i).Attributes.Add("Class", "GridviewScrollHeader_line_blanco_cort_leter")
                Next
                For i As Integer = 0 To grediview.Rows.Count - 1
                    grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                    Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                    Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                    If grediview.Rows(i).Cells(11).Text > 0 Or grediview.Rows(i).Cells(12).Text > 0 Then
                        ihtml.Attributes.Add("class", "fad fa-folder-open")
                        ihtml.Style.Add("color", "white")
                    Else
                        ihtml.Attributes.Add("class", "fad fa-folder-open")
                        ihtml.Style.Add("color", "white")
                    End If
                    Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                    If grediview.Rows(i).Cells(11).Text > 0 Or grediview.Rows(i).Cells(12).Text > 0 Then
                        ahtml.Attributes.Add("Class", "btn btn-success btn-sm")
                        ahtml.Attributes.Add("title", "Con documentos")
                    Else
                        ahtml.Attributes.Add("Class", "btn btn-success btn-sm")
                        ahtml.Attributes.Add("title", "Sin documentos")
                    End If
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "ver_doc_col")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)
                    divhtml.Style.Add("display", "inline-flex")
                    grediview.Rows(i).Cells(0).Controls.Add(divhtml)
                    For z As Integer = 0 To grediview.Rows(i).Cells.Count - 1
                        If z > 0 Then
                            grediview.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_cort_tr_flex")
                            grediview.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(event,this);")
                        End If
                    Next
                    'Dim imaga_buton As New HtmlInputImage
                    'imaga_buton.Attributes.Add("Class", "image_buton_clik_image")
                    'imaga_buton.Attributes.Add("onclick", "prevent(event,this);")
                    'imaga_buton.Attributes.Add("title", "Ver documentos relacionados")
                    'If grediview.Rows(i).Cells(11).Text > 0 Or grediview.Rows(i).Cells(12).Text > 0 Then
                    '    imaga_buton.Src = "../workflow/imageneswf/lista_sub_serie.png"
                    'Else
                    '    imaga_buton.Src = "../workflow/imageneswf/folder-open-light.png"
                    'End If
                    'imaga_buton.Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                    'imaga_buton.Attributes.Add("tip_event", "ver_doc_col")
                    'grediview.Rows(i).Cells(0).Controls.Add(imaga_buton)
                    'For z As Integer = 0 To grediview.Rows(i).Cells.Count - 1
                    '    grediview.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_corte_tr")
                    '    grediview.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(event,this);")
                    'Next
                Next
                HttpContext.Current.Session.Item("Sort_matri_colum_expe_clasificacion") = {"OPCIONES", "CODIGO", _
                                                                             "NOMBRE_UNIDAD", "ALEAS", "TEMA", "UNIDAD", _
                                                                             "FECHA_INI", "FECHA_FIN", "TIPO", _
                                                                             "CLASE_UNIDAD", "FOLIO_FISICO", "FOLIO_ELECTRONICO", _
                                                                              "FOLIO_DIGITALIZADO", "NOMBRE_SERIE_TRD", "NOMBRE_SUBSERIE_TRD"}
                HttpContext.Current.Session.Item("SortExpression_expe_clasificacion") = colum_order_name
                HttpContext.Current.Session.Item("SortDirection_expe_clasificacion") = order_colum
                HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_expe_clasificacion") = tipo_consulta
                HttpContext.Current.Session.Item("GA_DATO_CONSULTA_expe_clasificacion") = sql_consulta
                HttpContext.Current.Session.Item("nivel_expe_clasificacion") = nombre_nivel
                HttpContext.Current.Session.Item("serie_expe_clasificacion") = id_area_serie
                Dim Refclas As New ClassGredview
                Result = Refclas.add_clase_acender_decender(colum_order_name, _
                                                            HttpContext.Current.Session.Item("Sort_matri_colum_expe_clasificacion"), _
                                                            order_colum, _
                                                            grediview)
                If Result <> "YES" Then
                    Lista_jerarquia_expedientes_unidades_simples_por_serie_area = "Error add clase funcion  Lista_solictudes_compartidos_de_un_usuario " & Result
                    Exit Function
                End If
                Lista_jerarquia_expedientes_unidades_simples_por_serie_area = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_jerarquia_expedientes_unidades_simples_por_serie_area = "Inconsistencia general función Lista_jerarquia_expedientes_unidades_simples_por_serie_area " & ex.Message
        End Try
    End Function
    Function Lista_registro_jerarquia_series_sub_series(ByVal nombre_jerarquia As String, _
                                                        ByVal valor_jerarquia As String, _
                                                        ByVal id_registro_jerarquia_padre As Integer, _
                                                        ByVal codigo_area As Integer, _
                                                        ByRef Treview_node As TreeNode) As String
        Try
            Dim Result As String = ""
            Dim stru_area() As stru_serie_subserie = Nothing
            Result = Me.lista_serie_clasificacion(codigo_area, stru_area)
            If Result <> "YES" Then
                Lista_registro_jerarquia_series_sub_series = Result
                Exit Function
            End If
            If stru_area Is Nothing Then
                Lista_registro_jerarquia_series_sub_series = "YES"
                Exit Function
            End If
            For i As Integer = 0 To stru_area.Length - 1
                Dim Trednode As New TreeNode
                Trednode.Text = UCase("" & " /  " & "" & " (" & "" & " - " & "" & ")" & " / " & " / " & UCase(stru_area(i).Nombre_serie_subserie))
                Trednode.Value = "-1|" & nombre_jerarquia & "|" & stru_area(i).id_serie_sub_serie & "|" & valor_jerarquia & "|" & id_registro_jerarquia_padre
                Trednode.ToolTip = "-1|" & nombre_jerarquia & "|" & stru_area(i).id_serie_sub_serie & "|" & valor_jerarquia & "|" & id_registro_jerarquia_padre
                Trednode.ToolTip = "Conjunto de carpetas o expedientes organizados por asuntos y temas especificos"
                Trednode.ImageUrl = "../workflow/imageneswf/books-light.png"
                Treview_node.ChildNodes.Add(Trednode)
            Next
            Lista_registro_jerarquia_series_sub_series = "YES"
        Catch ex As Exception
            Lista_registro_jerarquia_series_sub_series = "Inconsistencia general función Lista_registro_jerarquia_areas " & ex.Message
        End Try
    End Function
    Function Lista_registro_jerarquia_areas(ByVal nombre_jerarquia As String, _
                                            ByVal valor_jerarquia As String, _
                                            ByVal id_registro_jerarquia_padre As Integer, _
                                            ByVal id_organigrma As Integer, _
                                            ByRef Treview_node As TreeNode) As String
        Try
            Dim Result As String = ""
            Dim stru_area() As stru_area = Nothing
            Dim Ref As New Class_areas_depart_radicacion
            Result = Ref.Lista_areas_clasificacion(id_organigrma, _
                                                   stru_area)
            If Result <> "YES" Then
                Lista_registro_jerarquia_areas = Result
                Exit Function
            End If
            If stru_area Is Nothing Then
                Lista_registro_jerarquia_areas = "YES"
                Exit Function
            End If
            For i As Integer = 0 To stru_area.Length - 1
                Dim Trednode As New TreeNode
                Trednode.Text = UCase("" & " /  " & "" & " (" & "" & " - " & "" & ")" & " / " & " / " & UCase(stru_area(i).Nombre_Area))
                Trednode.Value = "-1|" & nombre_jerarquia & "|" & stru_area(i).Codigo_Area & "|" & valor_jerarquia & "|" & id_registro_jerarquia_padre
                Trednode.ToolTip = "Representa a un área, dependencia o departamento  del cuadro clasificación documental"
                Trednode.ImageUrl = "../workflow/imageneswf/area-light.png"
                Treview_node.ChildNodes.Add(Trednode)
            Next
            Lista_registro_jerarquia_areas = "YES"
        Catch ex As Exception
            Lista_registro_jerarquia_areas = "Inconsistencia general función Lista_registro_jerarquia_areas " & ex.Message
        End Try
    End Function
    Function lista_serie_clasificacion(ByVal id_area_departamento As Integer, ByRef stru_serie_sub_serie() As stru_serie_subserie) As String
        '-------------------------------------------------------------
        'Función : Retorna las areas departamento para clasificación
        'Fecha : 2017-01-23
        'Ing : Miguel Angel Urueta Miranda
        '-------------------------------------------------------------
        Try
            Dim condicion_departamento As String = ""
            If id_area_departamento <> 0 Then
                condicion_departamento = "and Areas_Depart_Radicacion_Codigo_Area=" & id_area_departamento
            End If
            Dim Parametro_Consulta As String = "select  Id_Series,Nombre_Serie " & _
                      " from series_documentales WHERE Estado_Publico_Serie=1 " & condicion_departamento
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Dat_reader As DataSet = New DataSet("series_documentales")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                lista_serie_clasificacion = "Error conexión función lista_serie_clasificacion " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count = 0 Then
                lista_serie_clasificacion = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Dat_reader.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_serie_sub_serie(i)
                    stru_serie_sub_serie(i).id_serie_sub_serie = Dat_reader.Tables(0).Rows(i).Item(0)
                    stru_serie_sub_serie(i).Nombre_serie_subserie = Dat_reader.Tables(0).Rows(i).Item(1)
                    stru_serie_sub_serie(i).id_area = id_area_departamento
                    stru_serie_sub_serie(i).tipo_seri_sub_serie = "Serie"
                Next
                lista_serie_clasificacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            lista_serie_clasificacion = "Inconsistencia general función lista_serie_clasificacion " & ex.Message
        End Try
    End Function
    Function lista_serie_subserie_clasificacion(ByVal id_area_departamento As Integer, ByRef stru_serie_sub_serie() As stru_serie_subserie) As String
        '-------------------------------------------------------------
        'Función : Retorna las serie y sub series clasificación
        'Fecha : 2017-01-23
        'Ing : Miguel Angel Urueta Miranda
        '-------------------------------------------------------------
        Try
            Dim condicion_departamento As String = ""
            If id_area_departamento <> 0 Then
                condicion_departamento = "and Areas_Depart_Radicacion_Codigo_Area=" & id_area_departamento
            End If
            Dim Parametro_Consulta As String = "select  Id_Series,Nombre_Serie " & _
                      " from series_documentales WHERE Estado_Publico_Serie=1 " & condicion_departamento
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Dat_reader As DataSet = New DataSet("series_documentales")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                lista_serie_subserie_clasificacion = "Error conexión función lista_serie_subserie_clasificacion " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count = 0 Then
                lista_serie_subserie_clasificacion = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Dat_reader.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_serie_sub_serie(i)
                    stru_serie_sub_serie(i).id_serie_sub_serie = Dat_reader.Tables(0).Rows(i).Item(0)
                    stru_serie_sub_serie(i).Nombre_serie_subserie = Dat_reader.Tables(0).Rows(i).Item(1)
                    stru_serie_sub_serie(i).id_area = id_area_departamento
                    stru_serie_sub_serie(i).tipo_seri_sub_serie = "Sub Serie"
                Next
                lista_serie_subserie_clasificacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            lista_serie_subserie_clasificacion = "Inconsistencia general función lista_serie_subserie_clasificacion " & ex.Message
        End Try
    End Function   
    Function Lista_registro_jerarquia_fondo(ByVal id_nivel_jerarquia_padre As Integer, _
                                            ByVal id_nivel_nivel_clasficacion As Integer, _
                                            ByVal nombre_jerarquia As String, _
                                            ByVal codigo_contedor_area_id_serie As Integer, _
                                            ByVal valor_jerarquia As String, _
                                            ByRef Treview_node As TreeNode) As String
        Try
            Dim matri_id_registro_relacion_jerarquia() As Integer = Nothing
            Dim ref_ra_de_re_jerarquia As New Class_ra_de_registro_jerarquia
            Dim Result As String = ref_ra_de_re_jerarquia.Retorna_listado_relaciones_jerarquia(id_nivel_jerarquia_padre, _
                                                                                               matri_id_registro_relacion_jerarquia)
            If Result <> "YES" Then
                Lista_registro_jerarquia_fondo = Result
                Exit Function
            End If
            If matri_id_registro_relacion_jerarquia Is Nothing Then
                Lista_registro_jerarquia_fondo = "YES"
                Exit Function
            End If
            For i As Integer = 0 To matri_id_registro_relacion_jerarquia.Length - 1
                Dim id_nivel_nivel_clasficacion_hijo As Integer = 0
                Dim nombre_nivel_clasificacion_jerarquia_hijo As String = ""
                Dim ref_des_niv_jerarquia As New Class_ra_de_descripcion_niveles_jerarquia
                Result = ref_des_niv_jerarquia.Retorna_id_nombre_nivel_descripcion_por_id_jerarquia(matri_id_registro_relacion_jerarquia(i), _
                                                                                                    id_nivel_nivel_clasficacion, _
                                                                                                    nombre_nivel_clasificacion_jerarquia_hijo)
                If Result <> "YES" Then
                    Lista_registro_jerarquia_fondo = Result
                    Exit Function
                Else
                    Dim signatura As String = ""
                    Dim titulo As String = ""
                    Dim Class_ra_de_descripcion_niveles_jerarquia As New Class_ra_de_descripcion_niveles_jerarquia
                    Result = Class_ra_de_descripcion_niveles_jerarquia.Retorna_datos_de_descripcion_titulo_signatura_nivel_clasificacion(matri_id_registro_relacion_jerarquia(i), titulo, signatura)
                    If Result <> "YES" Then
                        Lista_registro_jerarquia_fondo = Result
                        Exit Function
                    End If
                    Dim texto_nivel As String = ""

                    Dim Trednode As New TreeNode
                    Trednode.Text = UCase(titulo & " /  " & signatura & " (" & "" & " - " & "" & ")" & " / " & " / " & nombre_nivel_clasificacion_jerarquia_hijo & texto_nivel)
                    Trednode.Value = matri_id_registro_relacion_jerarquia(i) & "|" & nombre_jerarquia & "|" & _
                        codigo_contedor_area_id_serie & "|" & valor_jerarquia & "|" & id_nivel_jerarquia_padre
                    If nombre_nivel_clasificacion_jerarquia_hijo = "Sección y subsección" Then
                        Trednode.ImageUrl = "../workflow/imageneswf/area-light.png"
                        Trednode.ToolTip = "Representa al conjunto de áreas, dependencias o departamentos pertenecientes al cuadro de clasficación documental"
                    End If
                    If nombre_nivel_clasificacion_jerarquia_hijo = "Serie y subserie" Then
                        Trednode.ImageUrl = "../workflow/imageneswf/list-alt-light.png"
                        Trednode.ToolTip = "Representa al conjunto de asuntos o series pertenecientes al cuadro de clasficación documental"
                    End If

                    Treview_node.ChildNodes.Add(Trednode)
                    'Treview_node.Expand()
                End If
            Next
            Lista_registro_jerarquia_fondo = "YES"
        Catch ex As Exception
            Lista_registro_jerarquia_fondo = "Inconsistencia general función Lista_registro_jerarquia_fondo " & ex.Message
        End Try
    End Function

End Class
