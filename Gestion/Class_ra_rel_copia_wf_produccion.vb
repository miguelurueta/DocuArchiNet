Public Structure ra_rel_copia_wf_produccion
    Dim id_relacion_wf_produccion As Integer
    Dim ID_REGISTRO_PRODUCION_DOCUMENTAL As Long
    Dim id_tarea_wf As Long
    Dim id_usuario_wf As Integer
    Dim id_imagen_da As Integer
    Dim nombre_gabinete As String
    Dim id_producion_wf As Long
    Dim id_expediente_destino As Integer
    Dim id_ruta_wf As Integer
    Dim estado_copia_vincula As Integer
    Dim date_registro_trans As String
End Structure
Public Class class_detail_copia_wf_production
    Public id_relacion_wf_produccion As Integer
    Public estado_copia_vincula As Integer
    Public date_registro_trans As String
    Public Nombre_Remitente As String
    Public Cargo_Remite As String
    Public DESCRIPCION_TIPO_DOCUMENTO As String
    Public ID_EXPEDIENTE As Integer
    Public codigo_unico As String
    Public result As String
End Class
Public Class Class_ra_rel_copia_wf_produccion
    Function Solicita_service_lista_copia_documento_expediente(ByVal id_tarea_workflow As Long,
                                                               ByRef ilis_cls_class_detail_copia_wf_production As List(Of class_detail_copia_wf_production)) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita la estructura del registros de documentos de copia
        'de documentos a expedientes
        '
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_tarea_workflow      : Representa la identificación de la tarea workflow
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'ilis_cls_class_detail_copia_wf_production  : Estructura del registro
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-11-24
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim ilis_cls_class_detail_copia_wf_production_ As class_detail_copia_wf_production = Nothing
            Dim sql_string As String = "Select r.id_relacion_wf_produccion, R.estado_copia_vincula as TIPO, R.date_registro_trans AS FECHA_REGISTRO" &
                ", rdi.Nombre_Remitente as USUARIO_OPERADOR,rdi.Cargo_Remite as CARGO,rpd.DESCRIPCION_TIPO_DOCUMENTO" &
                ",ea.ID_EXPEDIENTE as COD_EXPEDIENTE_DESTINO, ea.codigo_unico AS EXPEDIENTE_DESTINO " &
                " from   ra_rel_copia_wf_produccion  as r " &
                "left outer join    remit_dest_interno as rdi on (rdi.Relacion_Workflow= r.id_usuario_wf) " &
                "left outer join   registro_producion_documental as rpd on (rpd.ID_REGISTRO_PRODUCION_DOCUMENTAL=r.ID_REGISTRO_PRODUCION_DOCUMENTAL) " &
                "left outer join expediente_archivo as ea on (ea.ID_EXPEDIENTE=r.id_expediente_destino) " &
                "where r.id_tarea_wf=" & id_tarea_workflow
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_rel_copia_wf_produccion")
            Dim Result = ref2.SELECTION_SELECT_FIELD(sql_string, Datset)
            If Result <> "YES" Then
                Solicita_service_lista_copia_documento_expediente = "Función  Solicita_service_lista_copia_documento_expediente " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                ilis_cls_class_detail_copia_wf_production_ = New class_detail_copia_wf_production
                ilis_cls_class_detail_copia_wf_production_.result = "YES"
                ilis_cls_class_detail_copia_wf_production_.id_relacion_wf_produccion = -1
                ilis_cls_class_detail_copia_wf_production.Add(ilis_cls_class_detail_copia_wf_production_)
                Solicita_service_lista_copia_documento_expediente = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ilis_cls_class_detail_copia_wf_production_ = New class_detail_copia_wf_production
                    ilis_cls_class_detail_copia_wf_production_.id_relacion_wf_produccion = Datset.Tables(0).Rows(i).Item(0)
                    ilis_cls_class_detail_copia_wf_production_.estado_copia_vincula = Datset.Tables(0).Rows(i).Item(1)
                    If Datset.Tables(0).Rows(i).IsNull(2) Then
                        ilis_cls_class_detail_copia_wf_production_.date_registro_trans = ""
                    Else
                        ilis_cls_class_detail_copia_wf_production_.date_registro_trans = Datset.Tables(0).Rows(i).Item(2)
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(3) Then
                        ilis_cls_class_detail_copia_wf_production_.Nombre_Remitente = ""
                    Else
                        ilis_cls_class_detail_copia_wf_production_.Nombre_Remitente = Datset.Tables(0).Rows(i).Item(3)
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(4) Then
                        ilis_cls_class_detail_copia_wf_production_.Cargo_Remite = ""
                    Else
                        ilis_cls_class_detail_copia_wf_production_.Cargo_Remite = Datset.Tables(0).Rows(i).Item(4)
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(5) Then
                        ilis_cls_class_detail_copia_wf_production_.DESCRIPCION_TIPO_DOCUMENTO = ""
                    Else
                        ilis_cls_class_detail_copia_wf_production_.DESCRIPCION_TIPO_DOCUMENTO = Datset.Tables(0).Rows(i).Item(5)
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(6) Then
                        ilis_cls_class_detail_copia_wf_production_.ID_EXPEDIENTE = 0
                    Else
                        ilis_cls_class_detail_copia_wf_production_.ID_EXPEDIENTE = Datset.Tables(0).Rows(i).Item(6)
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(7) Then
                        ilis_cls_class_detail_copia_wf_production_.codigo_unico = ""
                    Else
                        ilis_cls_class_detail_copia_wf_production_.codigo_unico = Datset.Tables(0).Rows(i).Item(7)
                    End If
                    ilis_cls_class_detail_copia_wf_production_.result = "YES"
                    ilis_cls_class_detail_copia_wf_production.Add(ilis_cls_class_detail_copia_wf_production_)
                Next
                Solicita_service_lista_copia_documento_expediente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_service_lista_copia_documento_expediente = "Inconsistencia general funcion Solicita_service_lista_copia_documento_expediente " & ex.Message
        End Try
    End Function
    Function Solicita_estrucutura_copia_documento_expediente(ByVal id_tarea_wf As Long,
                                                             ByVal id_ruta As Integer,
                                                             ByRef _ra_rel_copia_wf_produccion() As ra_rel_copia_wf_produccion) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita la estructura del registros de documentos copiados
        'desde el modulo workflow hasta la produción documental o un expdiente
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_tarea_wf      : Representa la identificación de la tarea workflow
        'id_ruta          : Representa la ruta a la que pertence la tarea
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        '_ra_rel_copia_wf_produccion  : Estructura del registro
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-11-18
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Parametro_Consulta = "select id_relacion_wf_produccion,ID_REGISTRO_PRODUCION_DOCUMENTAL,id_tarea_wf," &
                "id_usuario_wf,id_imagen_da,nombre_gabinete,id_producion_wf,id_expediente_destino,id_ruta_wf,estado_copia_vincula,date_registro_trans " &
            " from ra_rel_copia_wf_produccion where " &
            " id_tarea_wf=" & id_tarea_wf & " and id_ruta_wf=" & id_ruta
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_rel_copia_wf_produccion")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_estrucutura_copia_documento_expediente = "Función  Solicita_estrucutura_copia_documento_expediente " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                _ra_rel_copia_wf_produccion = Nothing
                Solicita_estrucutura_copia_documento_expediente = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve _ra_rel_copia_wf_produccion(i)
                    _ra_rel_copia_wf_produccion(i).id_relacion_wf_produccion = Datset.Tables(0).Rows(i).Item("id_relacion_wf_produccion")
                    _ra_rel_copia_wf_produccion(i).ID_REGISTRO_PRODUCION_DOCUMENTAL = Datset.Tables(0).Rows(i).Item("ID_REGISTRO_PRODUCION_DOCUMENTAL")
                    _ra_rel_copia_wf_produccion(i).id_tarea_wf = Datset.Tables(0).Rows(i).Item("id_tarea_wf")
                    _ra_rel_copia_wf_produccion(i).id_usuario_wf = Datset.Tables(0).Rows(i).Item("id_usuario_wf")
                    _ra_rel_copia_wf_produccion(i).id_imagen_da = Datset.Tables(0).Rows(i).Item("id_imagen_da")
                    _ra_rel_copia_wf_produccion(i).nombre_gabinete = Datset.Tables(0).Rows(i).Item("nombre_gabinete")
                    _ra_rel_copia_wf_produccion(i).id_producion_wf = Datset.Tables(0).Rows(i).Item("id_producion_wf")
                    _ra_rel_copia_wf_produccion(i).id_expediente_destino = Datset.Tables(0).Rows(i).Item("id_expediente_destino")
                    _ra_rel_copia_wf_produccion(i).id_ruta_wf = Datset.Tables(0).Rows(i).Item("id_ruta_wf")
                    _ra_rel_copia_wf_produccion(i).estado_copia_vincula = Datset.Tables(0).Rows(i).Item("estado_copia_vincula")
                    If Datset.Tables(0).Rows(i).IsNull("date_registro_trans") = True Then
                        _ra_rel_copia_wf_produccion(i).date_registro_trans = ""
                    Else
                        _ra_rel_copia_wf_produccion(i).date_registro_trans = Datset.Tables(0).Rows(i).Item("date_registro_trans")
                    End If
                Next
                Solicita_estrucutura_copia_documento_expediente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estrucutura_copia_documento_expediente = "Inconsistencia general función Solicita_estrucutura_copia_documento_expediente " & ex.Message
        End Try
    End Function
    Function Solicita_existencia_copia_estructura_expediente_workflow(ByVal id_imagen_da As Integer,
                                                                      ByVal nombre_gabinete_da As String,
                                                                      ByVal id_tarea_wf_da_ As Long,
                                                                      ByVal id_expediente_destino_da_ As Integer,
                                                                      ByRef exitencia_copia_wf As String) As String
        Try
            Dim Parametro_Consulta = "select id_imagen_da " &
           " from ra_rel_copia_wf_produccion where id_imagen_da=" & id_imagen_da &
           " and nombre_gabinete='" & nombre_gabinete_da & "'" &
           " and id_tarea_wf=" & id_tarea_wf_da_ &
           " and id_expediente_destino=" & id_expediente_destino_da_
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_rel_copia_wf_produccion")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_existencia_copia_estructura_expediente_workflow = "Función  Solicita_existencia_copia_estructura_expediente_workflow " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                exitencia_copia_wf = "NO"
                Solicita_existencia_copia_estructura_expediente_workflow = "YES"
                Exit Function
            Else
                exitencia_copia_wf = "YES"
                Solicita_existencia_copia_estructura_expediente_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_existencia_copia_estructura_expediente_workflow = "Inconsistencia general funcion Solicita_existencia_copia_estructura_expediente_workflow " & ex.Message
        End Try
    End Function
    Function Solicita_existencia_numero_imagenes_copiadas(ByVal id_tarea_wf_da_ As Long,
                                                          ByVal id_ruta As Integer,
                                                          ByRef numero_copias_doc As Integer) As String
        Try
            Dim Parametro_Consulta = "select id_imagen_da " &
          " from ra_rel_copia_wf_produccion where " &
          "  id_tarea_wf=" & id_tarea_wf_da_ & " and id_ruta_wf=" & id_ruta
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_rel_copia_wf_produccion")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_existencia_numero_imagenes_copiadas = "Función  Solicita_existencia_numero_imagenes_copiadas " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                numero_copias_doc = 0
                Solicita_existencia_numero_imagenes_copiadas = "YES"
                Exit Function
            Else
                numero_copias_doc = Datset.Tables(0).Rows.Count
                Solicita_existencia_numero_imagenes_copiadas = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_existencia_numero_imagenes_copiadas = "Inconsistencia general funcion Solicita_existencia_numero_imagenes_copiadas " & ex.Message
        End Try
    End Function
    Function Solicita_existencia_numero_imagenes_vinculadas(ByVal id_tarea_wf_da_ As Long,
                                                            ByVal id_ruta As Integer,
                                                            ByRef numero_copias_doc As Integer) As String
        Try
            Dim Parametro_Consulta = "select id_imagen_da " &
          " from ra_rel_copia_wf_produccion where " &
          "  id_tarea_wf=" & id_tarea_wf_da_ & " and estado_copia_vincula=" & 2 & " and id_ruta_wf=" & id_ruta
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_rel_copia_wf_produccion")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_existencia_numero_imagenes_vinculadas = "Función  Solicita_existencia_numero_imagenes_vinculadas " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                numero_copias_doc = 0
                Solicita_existencia_numero_imagenes_vinculadas = "YES"
                Exit Function
            Else
                numero_copias_doc = Datset.Tables(0).Rows.Count
                Solicita_existencia_numero_imagenes_vinculadas = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_existencia_numero_imagenes_vinculadas = "Inconsistencia general funcion Solicita_existencia_numero_imagenes_vinculadas " & ex.Message
        End Try
    End Function
    Function EliminaRelacionVinculaTareaWorkflowExpedienteIdTarea(ByVal IdTareaWorkflow As Long) As String
        Try
            Dim Result As String = ""
            Dim ConexioDB As New conect.Dbase_Conction_Mysql_DA
            Dim SQLdelete As String = "Delete from ra_rel_copia_wf_produccion where id_tarea_wf=" & IdTareaWorkflow
            Result = ConexioDB.SELECTION_DELETE_COMMAND(SQLdelete)
            EliminaRelacionVinculaTareaWorkflowExpedienteIdTarea = Result
            Exit Function
        Catch ex As Exception
            EliminaRelacionVinculaTareaWorkflowExpedienteIdTarea = "Inconsistencia general funcion EliminaRelacionVinculaTareaWorkflowExpedienteIdTarea " & ex.Message
        End Try
    End Function
    Function SolicitaUltimaRelacionExpedienteIdTareaWorkflow(ByVal IdTareaWorkflow As Long,
                                                             ByVal IdRuta As Integer,
                                                             ByRef IdExpediente As Integer) As String
        Try
            Dim Parametro_Consulta = "select id_expediente_destino " &
            " from ra_rel_copia_wf_produccion where " &
            "  id_tarea_wf=" & IdTareaWorkflow & " and id_ruta_wf=" & IdRuta & " and estado_copia_vincula=2  order by id_relacion_wf_produccion desc  limit 1 "
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_rel_copia_wf_produccion")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                SolicitaUltimaRelacionExpedienteIdTareaWorkflow = "Función  SolicitaUltimaRelacionExpedienteIdTareaWorkflow " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                IdExpediente = 0
                SolicitaUltimaRelacionExpedienteIdTareaWorkflow = "YES"
                Exit Function
            Else
                IdExpediente = Datset.Tables(0).Rows(0).Item(0)
                SolicitaUltimaRelacionExpedienteIdTareaWorkflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaUltimaRelacionExpedienteIdTareaWorkflow = "Inconsistencia general funcion SolicitaUltimaRelacionExpedienteIdTareaWorkflow " & ex.Message
        End Try
    End Function
End Class
