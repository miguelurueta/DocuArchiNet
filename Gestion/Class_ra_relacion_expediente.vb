Imports MySql.Data.MySqlClient

Public Class Class_ra_relacion_expediente
    Function Solicita_listado_expedientes_volumenes_relacionados(ByVal id_expediente_padre As Long,
                                                                 ByRef reflabel As Label,
                                                                 ByRef grediview As GridView,
                                                                 ByRef hideselecion As Object,
                                                                 ByRef update As UpdatePanel,
                                                                 ByRef update_title As UpdatePanel) As String
        Try
            Dim sql_consulta As String = "Select rle.ID_EXPDIENTE_HIJO as ID_EXPEDIENTE, ea.CODIGO_UNICO AS CONSECUTIVO from  ra_relacion_expediente as rle " &
                " inner join expediente_archivo as ea on (ea.ID_EXPEDIENTE=rle.ID_EXPDIENTE_HIJO) " &
                " WHERE rle.ID_EXPEDIENTE_PADRE=" & id_expediente_padre
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_relacion_expediente")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_listado_expedientes_volumenes_relacionados = "Error función Solicita_listado_expedientes_volumenes_relacionados  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                reflabel.Text = "Se encontraron 0 registro(s) "
                grediview.DataSource = Nothing
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                update_title.Update()
                Solicita_listado_expedientes_volumenes_relacionados = "YES"
                Exit Function
            Else
                reflabel.Text = "Se encontraron " & Datset.Tables(0).Rows.Count & " registro(s) "
                'grediview.DataKeyNames = DataKey
                grediview.DataSource = Datset
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                update_title.Update()
                For i As Integer = 0 To grediview.Rows.Count - 1
                    grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                    Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                    Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-folder-times fa-lg")
                    ihtml.Style.Add("color", "white")
                    Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-primary btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Elimina relación volumen")
                    ahtml.Attributes.Add("idd_image_rel_", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("idd_expediente_rel_padre_", id_expediente_padre.ToString())
                    ahtml.Attributes.Add("tip_event", "elimina_rel_exp_")
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
                Solicita_listado_expedientes_volumenes_relacionados = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_listado_expedientes_volumenes_relacionados = "Inconsistencia general función Solicita_listado_expedientes_volumenes_relacionados " & ex.Message
        End Try
    End Function
    Function Verfica_existencia_expediente_padre_volumen(ByVal id_expediente As Integer,
                                                         ByRef existencia As String) As String
        '******************************************************************
        'Funcion : Verifica la existencia volumenes expediente
        'diente
        'Fecha : 2015-01-28
        'Ingeniero: Miguel Angel Urueta Miranda
        'Modificado para la versión web 2015-21-04, se cambia el modo de
        'conexión a la base de datos
        '******************************************************************
        Try
            Dim Parametro_Consulta As String = "Select * from ra_relacion_expediente where ID_EXPEDIENTE_PADRE='" &
            id_expediente & "'"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("ra_relacion_expediente")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verfica_existencia_expediente_padre_volumen = "Función Verfica_existencia_expediente_padre_volumen  Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                existencia = "YES"
                Verfica_existencia_expediente_padre_volumen = "YES"
            Else
                existencia = "NO"
                Verfica_existencia_expediente_padre_volumen = "YES"
            End If
        Catch ex As Exception
            Verfica_existencia_expediente_padre_volumen = "Inconsistencia función Verfica_existencia_expediente_padre_volumen " & ex.Message
        End Try
    End Function
    Function Solicita_numero_volumen_expediente_padre(ByVal id_expediente_padre As Integer,
                                                      ByRef numero_volumen As Integer) As String

        Try
            Dim Parametro_Consulta As String = "Select * from ra_relacion_expediente where ID_EXPEDIENTE_PADRE='" &
            id_expediente_padre & "'"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("ra_relacion_expediente")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_numero_volumen_expediente_padre = "Función Solicita_numero_volumen_expediente_padre  Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                numero_volumen = Datset.Tables(0).Rows.Count
                Solicita_numero_volumen_expediente_padre = "YES"
            Else
                numero_volumen = 0
                Solicita_numero_volumen_expediente_padre = "YES"
            End If
        Catch ex As Exception
            Solicita_numero_volumen_expediente_padre = "Inconsistencia función Solicita_numero_volumen_expediente_padre " & ex.Message
        End Try
    End Function
    Function Des_registrar_expediente_volumen(ByVal id_expediente_hijo As Long,
                                              ByVal hiden_ressultado As String,
                                              ByRef id_expediente_padre As Integer) As String
        Dim Result As String = ""
        Dim ref_class As New ClassGaExpediente
        Result = Me.Solicita_id_expediente_padre_volumen(id_expediente_hijo,
                                                         id_expediente_padre)
        If Result <> "YES" Then
            Des_registrar_expediente_volumen = Result
            hiden_ressultado = Result
            Exit Function
        End If
        If id_expediente_padre = 0 Then
            hiden_ressultado = Result
            Des_registrar_expediente_volumen = "Imposible encontrar el expediente padre del expediente (" & id_expediente_hijo & ") , en la relación de volúmenes"
            Exit Function
        End If
        If HttpContext.Current.Session.Item("GA_MANAGER_GESTION") <> 1 Then
            Result = ref_class.Verifica_propiedad_usuario_expediente(id_expediente_hijo,
                                                                     HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
            If Result <> "YES" Then
                hiden_ressultado = Result
                Des_registrar_expediente_volumen = Result
                Exit Function
            End If
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim consecutivo_unidad_volumen As String = 0
        Dim errorM As String = "YES"
        Try

            Dim sqlforupdate As String = "Select  CONSECUTIVO_EXPEDIENTE_2 from expediente_archivo  where ID_EXPEDIENTE=" &
            id_expediente_padre & " for update "
            'myConnection.Open()
            Dim dat_reader As MySqlDataReader
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = sqlforupdate
            dat_reader = myCommand.ExecuteReader()
            If dat_reader Is Nothing Then
                Des_registrar_expediente_volumen = "Imposible Encontrar consecutivo expediente error de conexion"
                errorM = "Imposible Encontrar consecutivo expediente error de conexion"
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If dat_reader.HasRows = False Then
                Des_registrar_expediente_volumen = "Imposible Encontrar consecutivo expediente"
                errorM = "Imposible Encontrar consecutivo expediente"
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If dat_reader.HasRows = True Then
                dat_reader.Read()
                consecutivo_unidad_volumen = dat_reader.Item(0)
                dat_reader.Close()
            End If
            consecutivo_unidad_volumen = consecutivo_unidad_volumen - 1
            '--------------------------------------------------
            'Actualiza el consecutivo volumen
            '--------------------------------------------------
            Dim Switc As Integer = 0
            Dim updatconsecutivo As String = "UPDATE expediente_archivo SET CONSECUTIVO_EXPEDIENTE_2=" &
            consecutivo_unidad_volumen & " where ID_EXPEDIENTE=" &
            id_expediente_padre
            myCommand.CommandText = updatconsecutivo
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                errorM = "Imposible actualiza consecutivo tipo unidad  : " & updatconsecutivo
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '--------------------------------------------------
            'Actualiza estado expediente padre
            '--------------------------------------------------
            Switc = 0
            updatconsecutivo = "UPDATE expediente_archivo SET EXPEDIENTE_PADRE = Null, VOLUMEN_EXPEDIENTE=1" &
              " where ID_EXPEDIENTE=" &
            id_expediente_hijo
            myCommand.CommandText = updatconsecutivo
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                errorM = "Imposible actualiza estado expediente padre de expediente hijo : " & updatconsecutivo
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '--------------------------------------------------
            'Elimina relación expediente padre
            '--------------------------------------------------
            Switc = 0
            updatconsecutivo = "Delete from  ra_relacion_expediente " &
              " where ID_EXPDIENTE_HIJO=" &
            id_expediente_hijo
            myCommand.CommandText = updatconsecutivo
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                errorM = "Imposible eliminar relación expediente padre  expediente hijo : " & updatconsecutivo
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            errorM = "YES"
            Des_registrar_expediente_volumen = "YES"
            hiden_ressultado = "YES"
            Exit Function
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                'myTrans.Rollback()
                myConnection.Close()
                Des_registrar_expediente_volumen = "An exception of type " + ex.GetType().ToString() +
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() +
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Des_registrar_expediente_volumen = errorM
            hiden_ressultado = errorM

        End Try
    End Function
    Function Solicita_id_expediente_padre_volumen(ByVal id_expediente_hijo As Long, _
                                                  ByRef id_expediente_padre As Long) As String
        '******************************************************************
        'Funcion : Solicita el expediente padre relacionado con el expedien
        'te hijo informado en la tabla de relaciones
        'Fecha : 2019-03-04
        'Ingeniero: Miguel Angel Urueta Miranda
        '******************************************************************
        Try
            Dim Parametro_Consulta As String = "Select ID_EXPEDIENTE_PADRE from ra_relacion_expediente where ID_EXPDIENTE_HIJO=" & _
             id_expediente_hijo
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("ra_relacion_expediente")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_expediente_padre_volumen = "Función Solicita_id_expediente_padre_volumen  Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                id_expediente_padre = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_expediente_padre_volumen = "YES"
            Else
                id_expediente_padre = 0
                Solicita_id_expediente_padre_volumen = "YES"
            End If
        Catch ex As Exception
            Solicita_id_expediente_padre_volumen = "Inconsistencia general función Solicita_id_expediente_padre_volumen " & ex.Message
        End Try
    End Function
    Function Valida_interface_relacion_volumen(ByVal id_expediente As Long) As String
        '----------------------------------------------------------
        'Funcion : Valida el estado del expediente para determinar
        'si es un volumen o un expediente padre
        'Fecha : 2019-03-05
        'Ing :Miguel Angel Urueta Miranda
        '---------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Refclass As New ClassGaExpediente
            Dim estado_propietario As Integer = 0
            '--------------------------------------------
            'Verifica estado propietario expediente
            '--------------------------------------------
            Result = Refclass.Verifica_propietario_expediente(id_expediente, _
                                                            estado_propietario)
            If Result <> "YES" Then
                Valida_interface_relacion_volumen = Result
                Exit Function
            End If
            If estado_propietario = 0 Then
                Valida_interface_relacion_volumen = "Usuario no propietario imposible continuar"
                Exit Function
            End If
            '------------------------------------------------
            'Determina existencia expediente padre
            '-----------------------------------------------
            Dim estado_padre As String = "YES"
            Result = Me.Verfica_existencia_expediente_padre_volumen(id_expediente, _
                                                                    estado_padre)
            If Result <> "YES" Then
                Valida_interface_relacion_volumen = Result
                Exit Function
            End If
            If estado_padre = "YES" Then
                Valida_interface_relacion_volumen = "El expediente tiene expedientes relacionados, imposible relacionar como volumen"
                Exit Function
            End If
            '------------------------------------------
            'Determina existencia expediente volumen
            '-----------------------------------------
            Dim id_expediente_padre As Long = 0
            Result = Me.Solicita_id_expediente_padre_volumen(id_expediente, _
                                                           id_expediente_padre)
            If Result <> "YES" Then
                Valida_interface_relacion_volumen = Result
                Exit Function
            End If
            If id_expediente_padre <> 0 Then
                Valida_interface_relacion_volumen = "El expediente es un volumen relacionado a un expediente, imposible relacionar como volumen"
                Exit Function
            End If
            Valida_interface_relacion_volumen = "YES"
            Exit Function
        Catch ex As Exception
            Valida_interface_relacion_volumen = "Inconsistencia general función Valida_interface_relacion_volumen " & ex.Message
        End Try
    End Function
    Function Relacionar_como_expediente_volumen(ByVal id_expediente_padre As Integer,
                                                ByVal id_expediente_volumen As Integer,
                                                ByVal hiden_ressultado As String) As String
        Dim Result As String = ""
        Dim Refclass As New ClassGaExpediente
        Dim estado_propietario As Integer = 0
        '----------------------------------------------------------
        'Verifica estado propietario expediente padre a relacionar
        '-----------------------------------------------------------
        Result = Refclass.Verifica_propietario_expediente(id_expediente_padre,
                                                          estado_propietario)
        If Result <> "YES" Then
            Relacionar_como_expediente_volumen = Result
            Exit Function
        End If
        If estado_propietario = 0 Then
            Relacionar_como_expediente_volumen = "Usuario no propietario del expediente, imposible relacionar"
            Exit Function
        End If
        '------------------------------------------------
        'Verifica existencia de relación asi mismo
        '-----------------------------------------------   
        If id_expediente_padre = id_expediente_volumen Then
            Relacionar_como_expediente_volumen = "El expediente no se puede relacionar asi mismo"
            Exit Function
        End If
        '-----------------------------------------------------
        'Determina existencia expediente padre como volumen
        '-----------------------------------------------------
        Dim id_expediente_padre_volumen As Long = 0
        Result = Me.Solicita_id_expediente_padre_volumen(id_expediente_volumen,
                                                         id_expediente_padre_volumen)
        If Result <> "YES" Then
            Relacionar_como_expediente_volumen = Result
            Exit Function
        End If
        If id_expediente_padre_volumen <> 0 Then
            Relacionar_como_expediente_volumen = "El expediente es un volumen relacionado a un expediente, imposible relacionar como padre del volumen"
            Exit Function
        End If
        '------------------------------------------------
        'Verifica expediente produción documental 
        'no se elimine desde el gestor de expedientes
        '------------------------------------------------
        Dim estado_expediente As Integer = 0
        Dim estado_publico As Integer = 0
        Result = Refclass.Retorna_estado_expediente(id_expediente_volumen,
                                                    estado_expediente,
                                                    estado_publico)
        If Result <> "YES" Then
            Relacionar_como_expediente_volumen = Result
            Exit Function
        End If
        If estado_publico = 2 Then
            Relacionar_como_expediente_volumen = "Imposible relacionar el expediente como volumen, debido a que pertenece a la producción documental de otro usuario "
            Exit Function
        End If
        Result = Refclass.Retorna_estado_expediente(id_expediente_padre,
                                                    estado_expediente,
                                                    estado_publico)
        If Result <> "YES" Then
            Relacionar_como_expediente_volumen = Result
            Exit Function
        End If
        If estado_publico = 2 Then
            Relacionar_como_expediente_volumen = "Imposible relacionar el expediente padre (" & id_expediente_padre & ") como volumen, debido a que pertenece a la producción documental de otro usuario"
            Exit Function
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim consecutivo_unidad_volumen As String = 0
        Dim errorM As String = "YES"
        Try

            Dim sqlforupdate As String = "Select  CONSECUTIVO_EXPEDIENTE_2 from expediente_archivo  where ID_EXPEDIENTE=" &
            id_expediente_padre & " for update "
            'myConnection.Open()
            Dim dat_reader As MySqlDataReader
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = sqlforupdate
            dat_reader = myCommand.ExecuteReader()
            If dat_reader Is Nothing Then
                Relacionar_como_expediente_volumen = "Imposible Encontrar consecutivo expediente error de conexion"
                errorM = "Imposible Encontrar consecutivo expediente error de conexion"
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If dat_reader.HasRows = False Then
                Relacionar_como_expediente_volumen = "Imposible Encontrar consecutivo expediente"
                errorM = "Imposible Encontrar consecutivo expediente"
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If dat_reader.HasRows = True Then
                dat_reader.Read()
                consecutivo_unidad_volumen = dat_reader.Item(0)
                dat_reader.Close()
            End If
            consecutivo_unidad_volumen = consecutivo_unidad_volumen + 1
            '--------------------------------------------------
            'Actualiza el consecutivo expediente volumen
            '--------------------------------------------------
            Dim Switc As Integer = 0
            Dim updatconsecutivo As String = "UPDATE expediente_archivo SET CONSECUTIVO_EXPEDIENTE_2=" &
            consecutivo_unidad_volumen & " where ID_EXPEDIENTE=" &
            id_expediente_padre
            myCommand.CommandText = updatconsecutivo
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                errorM = "Imposible actualiza consecutivo expedientes padre : " & updatconsecutivo
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '--------------------------------------------------
            'Actualiza estado expediente padre del volumen
            '--------------------------------------------------
            Switc = 0
            updatconsecutivo = "UPDATE expediente_archivo SET EXPEDIENTE_PADRE = " & id_expediente_padre & " , VOLUMEN_EXPEDIENTE=" & consecutivo_unidad_volumen &
              " where ID_EXPEDIENTE=" &
            id_expediente_volumen
            myCommand.CommandText = updatconsecutivo
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                errorM = "Imposible actualiza estado expediente padre de expediente hijo : " & updatconsecutivo
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '--------------------------------------------------
            'Agrega relación expediente padre
            '--------------------------------------------------
            Switc = 0
            Dim isertrelacion As String = "Insert Into ra_relacion_expediente (ID_EXPEDIENTE_PADRE,ID_EXPDIENTE_HIJO) values " &
            "(" & id_expediente_padre & "," & id_expediente_volumen & ")"
            myCommand.CommandText = isertrelacion
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                errorM = "Imposible eliminar relación expediente padre  expediente hijo : " & updatconsecutivo
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            errorM = "YES"
            Relacionar_como_expediente_volumen = "YES"
            hiden_ressultado = "YES"
            Exit Function
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                'myTrans.Rollback()
                myConnection.Close()
                Relacionar_como_expediente_volumen = "An exception of type " + ex.GetType().ToString() +
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() +
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Relacionar_como_expediente_volumen = errorM
            hiden_ressultado = errorM

        End Try
    End Function
End Class
