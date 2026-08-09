Public Class class_detalle_log_procesing_workflow
    Public Id_log_docuarchi As Long
    Public desc_op As String
    Public USER_OPER As String
    Public DATE_TRANS As String
    Public RUT_DOCU As String
    Public GABINETE As String
    Public CAMPOS As String
    Public RADICADO As String
    Public HORA_REGISTRO As String
    Public result As String
    Public id_tran As Integer
    Public USER_PROPIETARIO As String
    Public TIPOLOGIA_DOCUMENTAL As String
End Class
Public Class Class_logdocuarchi
    Function Solicita_service_detalle_log_procesos_imagen_workflow(ByVal id_tarea_workflow As Long,
                                                                   ByVal id_ruta_workflow As Integer,
                                                                   ByRef ilis_cls_det_log_pro_wf As List(Of class_detalle_log_procesing_workflow)) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita el detalle de los procesos realizados a las imagenes
        '          en la gestión de una tarea en workflow
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_tarea_workflo         : Representa la identificacion de la tarea workflow
        'id_ruta_workflow         : Representa la identificacion de la ruta
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'ilis_cls_det_log_pro_wf  : Retorna la estructura del listado del log de
        '                           procesos sobre imagenes
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-07-05
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim ilis_cls_det_log_pro_wf_ As class_detalle_log_procesing_workflow = Nothing
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("logdocuarchi")
            Dim Sql_consulta As String = "select Id_log_docuarchi,desc_op,USER_OPER,DATE_TRANS,RUT_DOCU,GABINETE,CAMPOS,RADICADO,HORA_REGISTRO,id_tran,USER_PROPIETARIO,TIPOLOGIA_DOCUMENTAL from " &
                " logdocuarchi where ID_TAREA_WF=" & id_tarea_workflow & " and ID_RUTA_WF=" & id_ruta_workflow & " order by Id_log_docuarchi desc"
            Dim Result As String = ref.SELECTION_SELECT_FIELDA(Sql_consulta,
                                                               Datset)
            If Result <> "YES" Then
                ilis_cls_det_log_pro_wf_ = New class_detalle_log_procesing_workflow
                ilis_cls_det_log_pro_wf_.result = Result
                ilis_cls_det_log_pro_wf.Add(ilis_cls_det_log_pro_wf_)
                Solicita_service_detalle_log_procesos_imagen_workflow = "Error function Solicita_service_detalle_log_procesos_imagen_workflow " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                ilis_cls_det_log_pro_wf_ = New class_detalle_log_procesing_workflow
                ilis_cls_det_log_pro_wf_.result = "YES"
                ilis_cls_det_log_pro_wf_.Id_log_docuarchi = -1
                ilis_cls_det_log_pro_wf.Add(ilis_cls_det_log_pro_wf_)
                Solicita_service_detalle_log_procesos_imagen_workflow = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ilis_cls_det_log_pro_wf_ = New class_detalle_log_procesing_workflow
                    ilis_cls_det_log_pro_wf_.result = "YES"
                    ilis_cls_det_log_pro_wf_.Id_log_docuarchi = Datset.Tables(0).Rows(i).Item("Id_log_docuarchi")
                    ilis_cls_det_log_pro_wf_.desc_op = Datset.Tables(0).Rows(i).Item("desc_op")
                    If (Datset.Tables(0).Rows(i).IsNull("USER_OPER")) Then
                        ilis_cls_det_log_pro_wf_.USER_OPER = ""
                    Else
                        ilis_cls_det_log_pro_wf_.USER_OPER = Datset.Tables(0).Rows(i).Item("USER_OPER")
                    End If
                    If (Datset.Tables(0).Rows(i).IsNull("DATE_TRANS")) Then
                        ilis_cls_det_log_pro_wf_.DATE_TRANS = ""
                    Else
                        ilis_cls_det_log_pro_wf_.DATE_TRANS = Datset.Tables(0).Rows(i).Item("DATE_TRANS")
                    End If
                    If (Datset.Tables(0).Rows(i).IsNull("RUT_DOCU")) Then
                        ilis_cls_det_log_pro_wf_.RUT_DOCU = ""
                    Else
                        ilis_cls_det_log_pro_wf_.RUT_DOCU = Datset.Tables(0).Rows(i).Item("RUT_DOCU")
                    End If
                    If (Datset.Tables(0).Rows(i).IsNull("GABINETE")) Then
                        ilis_cls_det_log_pro_wf_.GABINETE = ""
                    Else
                        ilis_cls_det_log_pro_wf_.GABINETE = Datset.Tables(0).Rows(i).Item("GABINETE")
                    End If
                    If (Datset.Tables(0).Rows(i).IsNull("CAMPOS")) Then
                        ilis_cls_det_log_pro_wf_.CAMPOS = ""
                    Else
                        ilis_cls_det_log_pro_wf_.CAMPOS = Datset.Tables(0).Rows(i).Item("CAMPOS")
                    End If
                    If (Datset.Tables(0).Rows(i).IsNull("RADICADO")) Then
                        ilis_cls_det_log_pro_wf_.RADICADO = ""
                    Else
                        ilis_cls_det_log_pro_wf_.RADICADO = Datset.Tables(0).Rows(i).Item("RADICADO")
                    End If
                    ilis_cls_det_log_pro_wf_.HORA_REGISTRO = Datset.Tables(0).Rows(i).Item("HORA_REGISTRO")
                    ilis_cls_det_log_pro_wf_.id_tran = Datset.Tables(0).Rows(i).Item("id_tran")
                    If (Datset.Tables(0).Rows(i).IsNull("USER_PROPIETARIO")) Then
                        ilis_cls_det_log_pro_wf_.USER_PROPIETARIO = ""
                    Else
                        ilis_cls_det_log_pro_wf_.USER_PROPIETARIO = Datset.Tables(0).Rows(i).Item("USER_PROPIETARIO")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("TIPOLOGIA_DOCUMENTAL") Then
                        ilis_cls_det_log_pro_wf_.TIPOLOGIA_DOCUMENTAL = ""
                    Else
                        ilis_cls_det_log_pro_wf_.TIPOLOGIA_DOCUMENTAL = Datset.Tables(0).Rows(i).Item("TIPOLOGIA_DOCUMENTAL")
                    End If
                    ilis_cls_det_log_pro_wf.Add(ilis_cls_det_log_pro_wf_)
                Next
                Solicita_service_detalle_log_procesos_imagen_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_service_detalle_log_procesos_imagen_workflow = "Inconsistencia general funcion Solicita_service_detalle_log_procesos_imagen_workflow " & ex.Message
        End Try
    End Function
    Function Registra_log_procesing_image(ByVal id_Imagen As Integer,
                                          ByVal Nombre_Gabinete As String,
                                          ByVal modulo_log As String,
                                          ByVal operacion As String,
                                          ByVal id_tarea_wf As Long,
                                          ByVal radicado As String,
                                          ByVal date_campos As String) As String
        '---------------------------------------------------------------------------
        'Funcion : Registra log de procesos 
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_Imagen             : Representa la idnetificación de la imagen
        'Nombre_Gabinete       : Representa el nombre del gabinete
        'modulo_log            : Representa el modulo que realiza la operacion
        'operacion             : Representa la operación realizada por el usuario
        '                      : EditarIndice, Elimina, Visualiza, Registra
        'id_tarea_wf           : Representa la identificacion de la tarea worklflow
        'radicado              : Representa el identificador del radicado
        'date_campos           : Fepresenta los campos indices del documento
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        '
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-07-07
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim result As String = ""
            Dim date1al As String = Date.Today
            Dim ref_ClassGestionFechas As New ClassGestionFechas
            result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
            If result <> "YES" Then
                Registra_log_procesing_image = "Error formateando  Funcion: FRegistra_log_procesing_image_workflow " & result
                Exit Function
            End If
            Dim ref_radicado As String = "null"
            If radicado <> "" Then
                ref_radicado = "'" & radicado & "'"
            End If
            '----------------------------------------------------
            'Solicita ruta del documento
            '----------------------------------------------------
            Dim Route_cabinet As String = ""
            Dim Class_SYSTEM1RUT As New Class_SYSTEM1RUT
            result = Class_SYSTEM1RUT.Consulta_Ruta_Almacenamiento(Route_cabinet,
                                                                   Nombre_Gabinete)
            If result <> "YES" Then
                Registra_log_procesing_image = result
                Exit Function
            End If
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim Route_document As String = ""
            result = ClassDaGabinete.Solicita_ruta_achivo_gabinete(id_Imagen,
                                                                   Nombre_Gabinete,
                                                                   Route_cabinet,
                                                                   Route_document)
            If result <> "YES" Then
                Registra_log_procesing_image = result
                Exit Function
            End If
            Route_document = Route_document.Replace("\", "/")
            '------------------------------------------------
            'Solicita datos del documento
            '------------------------------------------------
            Dim Class_system1 As New Class_system1
            Dim inventario_documental As Integer = 0
            Dim aplica_trd As Integer = 0
            Dim asigna_unidad As Integer = 0
            result = Class_system1.Verifica_opcion_aplicar_trd_inventario_unidad_documental_gabinete(Nombre_Gabinete,
                                                                                                     inventario_documental,
                                                                                                     aplica_trd,
                                                                                                     asigna_unidad)
            If result <> "YES" Then
                Registra_log_procesing_image = result
                Exit Function
            End If
            Dim stru_paramter_image As stru_paramter_image = Nothing
            result = ClassDaGabinete.SolicitaEtructuraImagenGabinete(Nombre_Gabinete,
                                                                     id_Imagen,
                                                                     stru_paramter_image,
                                                                     aplica_trd,
                                                                     1)
            If result <> "YES" Then
                Registra_log_procesing_image = result
                Exit Function
            End If
            Dim ref_user As String = "null"
            If stru_paramter_image.USER <> "" Then
                ref_user = "'" & stru_paramter_image.USER & "'"
            End If
            Dim ref_Tipologia As String = "null"
            If stru_paramter_image.TIPODOCUMENTO <> "" Then
                ref_Tipologia = "'" & stru_paramter_image.TIPODOCUMENTO & "'"
            End If
            Dim ref_date_campos As String = "null"
            If date_campos <> "" Then
                ref_date_campos = "'" & date_campos & "'"
            End If
            Dim hor As New System.DateTime
            hor = Date.Now
            Dim hora As String = hor.Hour.ToString & ":" & hor.Minute.ToString & ":" & hor.Second.ToString
            Dim SqlTransac As String = " INSERT INTO logdocuarchi (id_tran,desc_op,USER_OPER,DATE_TRANS," _
            & "RUT_DOCU,MODULO_REGISTRO,GABINETE,CAMPOS,IP_TRANS,HORA_REGISTRO,RADICADO,ID_TAREA_WF,ID_RUTA_WF,USER_PROPIETARIO,TIPOLOGIA_DOCUMENTAL) VALUES ( "
            SqlTransac = SqlTransac & "'" & id_Imagen & "',"
            SqlTransac = SqlTransac & "'" & operacion & "',"
            SqlTransac = SqlTransac & "'" & HttpContext.Current.Session.Item("DA_Login_Usuario") & "',"
            SqlTransac = SqlTransac & "'" & date1al & "',"
            SqlTransac = SqlTransac & "'" & Route_document & "',"
            SqlTransac = SqlTransac & "'" & modulo_log & "',"
            SqlTransac = SqlTransac & "'" & Nombre_Gabinete & "',"
            SqlTransac = SqlTransac & ref_date_campos & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hora & "'," & ref_radicado & "," &
                id_tarea_wf & "," & HttpContext.Current.Session.Item("Id_Ruta_Workflow") & "," & ref_user & "," & ref_Tipologia & ")"
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            result = ref.SELECTION_INSERT_COMMAND(SqlTransac)
            Registra_log_procesing_image = result
            Exit Function
        Catch ex As Exception
            Registra_log_procesing_image = "Inconsistencia general funcion Registra_log_procesing_image " & ex.Message
        End Try
    End Function
End Class
