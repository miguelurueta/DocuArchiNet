Imports System.Drawing
Imports System.IO
Imports MySql.Data.MySqlClient
Imports GestionDocumental_Docuarchi.net.WebServiceWorkflow

Public Structure stru_documentos_compartidos
    Dim id_imagen As Integer
    Dim nombre_gabinete As String
    Dim tipo_documento As String
    Dim ruta_documento As String
    Dim identificador As Integer
    Dim extension As String
End Structure
Public Structure stru_documentos_colaboracion
    Dim ID_IMAGEN
    Dim NOMBRE_GABINETE As String
    Dim nombre_archivo As String
End Structure
Public Structure stru_usuario_gestion_compartido
    Dim id_usuario_gestion As Integer
    Dim nombre_usuario As String
    Dim cargo_usuario As String
End Structure
Public Class CdCompartirDocunento
    Property AppError As String
    Property ResultadoEnvioCorreo As String
    Property EstadoCambioSolicitudUsuario As String
    Property EstadoResultadoAprobacion As String
    Property IdDcoumento As Integer
    Property Gabinete As String
End Class
Public Structure STRU_DOCUMENTO_COMPARTIDO_GENERAL
    Dim ID_RA_CD_DOCUMENTOS_COMPARTIDOS As Long
    Dim Remit_Dest_Interno_id_remit_dest_Int As Integer
    Dim FECHA_REGISTRO_SOLICITUD As String
    Dim FECHA_REGISTRO_APROBACION As String
    Dim ESTADO_APROBACION As Integer
    Dim TIEMPO_RESPUESTA_APROBACION As Long
    Dim ESTADO_PRIORIDAD As Integer
    Dim NOTA_SOLICITUD As String
    Dim DESCRIPCION_ESTADO_APROBACION As String
    Dim FECHA_LIMITE_RESPUESTA As String
    Dim TIPO_REGISTRO_COMPARTIDO As Integer
    Dim DESCRIPCION_TIPO_COMPARTIDO As String
    Dim ASUNTO_DOCUMENTO As String
    Dim RADICADO_RELACIONADO As String
    Dim ESTADO_CONFIRMACION_COLABORACION As Integer
    Dim ESTADO_ELIMINADO As Integer
End Structure
Public Structure STRU_DOCUMENTO_COMPARTIDO_USUARIOS
    Dim Remit_Dest_Interno_id_remit_dest_Int As Integer
    Dim ID_USUARIOS_DOCUMENTOS_COMPARTIDOS As Integer
    Dim ID_RA_CD_DOCUMENTOS_COMPARTIDOS As Integer
    Dim FECHA_REGISTRO_SOLICITUD As String
    Dim FECHA_RESPUESTA_SOLICITUD As String
    Dim ESTADO_RESPUESTA_SOLICITUD As Integer
    Dim TIEMPO_RESPUESTA_SOLICITUD As Long
    Dim ESTADO_VISTO_SOLICITANTE As Integer
    Dim DESCRIPCION_ESTADO_RESPUESTA As String
    Dim FECHA_LIMITE_RESPUESTA As String
    Dim ASUNTO_DOCUMENTO As String
    Dim RADICADO_RELACIONADO As String
    Dim ESTADO_ELIMINADO As Integer
    Dim TIPO_REGISTRO_COMPARTIDO As Integer
    Dim DESCRIPCION_TIPO_COMPARTIDO As String
    Dim ESTADO_CONFIRMACION_COLABORACION As Integer
End Structure
Public Class ClassGaCompartirDocumento
    Function Registra_documento_colaboracion_documento_compartido(ByVal Nombre_Gabinete As String, _
                                                                  ByVal id_documento_compartido As Integer, _
                                                                  ByVal ruta_document As String, _
                                                                  ByRef Drowplist As DropDownList, _
                                                                  ByVal id_usuario_gestion As Integer) As String
        Try
            Dim id_usuario_compartido As Integer = 0
            Dim Result As String = ""
            Result = Me.Retorna_id_usuario_documento_compartido(id_documento_compartido, _
                                                                id_usuario_compartido, _
                                                                id_usuario_gestion)
            If Result <> "YES" Then
                Registra_documento_colaboracion_documento_compartido = Result
                Exit Function
            End If
            Dim id_imagen As Integer = 0
            Result = Me.Guardar_Documento_colaboracion(id_imagen, _
                                                       Nombre_Gabinete, _
                                                       id_documento_compartido, _
                                                       ruta_document)
            If Result <> "YES" Then
                Registra_documento_colaboracion_documento_compartido = Result
                Exit Function
            End If
            Dim refclas_gestion_fechas As New ClassGestionFechas
            Dim date1al As String = Date.Now
            Result = ""
            Result = refclas_gestion_fechas.Formatea_Fecha_Almacenamiento_Time(date1al)
            If Result <> "YES" Then
                Registra_documento_colaboracion_documento_compartido = "Imposible formatear fecha " & Result
                Exit Function
            End If
            Dim file_inf As New FileInfo(ruta_document)
            Dim sql_insert As String = "Insert into ra_cd_doumentos_colaboracion_compartidos (RA_CD_DOCUMENTOS_COMPARTIDOS_ID_RA_CD_DOCUMENTOS_COMPARTIDOS," & _
                         "ID_IMAGEN,NOMBRE_GABINETE,ID_USUARIOS_DOCUMENTOS_COMPARTIDOS,FECHA_REGISTRO_COLABORACION,nombre_archivo) values (" &
                         id_documento_compartido & "," & id_imagen & ",'" & Nombre_Gabinete & "'," & id_usuario_compartido & ",'" & date1al & "','" & id_imagen & "|" & file_inf.Name & "')"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_usuarios_documentos_compartidos")
            Result = ref.SELECTION_INSERT_COMMAND(sql_insert)
            If Result <> "YES" Then
                Registra_documento_colaboracion_documento_compartido = "Error Registra_documento_colaboracion_documento_compartido " & Result
                Exit Function
            Else
                Drowplist.Items.Add(id_imagen & "|" & file_inf.Name)
                Registra_documento_colaboracion_documento_compartido = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Registra_documento_colaboracion_documento_compartido = "Inconsistencia general función Registra_documento_colaboracion_documento_compartido " & ex.Message
        End Try
    End Function
    Function Lista_documentos_de_colaboracion_usuario_colaboracion_drow_list(ByVal id_documento_compartido As Integer, _
                                                                             ByVal id_usuario_documento_compartido As Integer, _
                                                                             ByRef drow_list As DropDownList, _
                                                                             ByRef update As UpdatePanel) As String
        '----------------------------------------------------------------
        'Función : Lista los documentos de colaboración de un usuario es
        'pecifico
        'Ing :Miguel Angel Urueta Miranda
        'Fecha : 2017-06-01
        '-----------------------------------------------------------------
        Try
            drow_list.Items.Clear()
            Dim sql_consulta As String = "SELECT nombre_archivo  from ra_cd_doumentos_colaboracion_compartidos " & _
                             " where RA_CD_DOCUMENTOS_COMPARTIDOS_ID_RA_CD_DOCUMENTOS_COMPARTIDOS=" & id_documento_compartido & " and ID_USUARIOS_DOCUMENTOS_COMPARTIDOS=" & id_usuario_documento_compartido
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_usuarios_documentos_compartidos")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_documentos_de_colaboracion_usuario_colaboracion_drow_list = "Error listando Lista_documentos_de_colaboracion_usuario_colaboracion " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Lista_documentos_de_colaboracion_usuario_colaboracion_drow_list = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    drow_list.Items.Add(Datset.Tables(0).Rows(i).Item(0))
                Next
                update.Update()
                Lista_documentos_de_colaboracion_usuario_colaboracion_drow_list = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_documentos_de_colaboracion_usuario_colaboracion_drow_list = "Inconsistencia general función Lista_documentos_de_colaboracion_usuario_colaboracion_drow_list " & ex.Message
        End Try
    End Function
    Function Elimina_documento_colaboracion_documento_compartido_dorw_list(ByVal nombre_documento As String, _
                                                                           ByRef drow_list As DropDownList, _
                                                                           ByRef update As UpdatePanel _
                                                                           ) As String
        '------------------------------------------------
        'Función : Elimina el documento de colaboración
        'seleccionando de la tabla de documentos de 
        'colabración
        'Ing Miguel Angel Urueta Miranda
        'Fecha : 2017-06-07
        '-------------------------------------------------
        Try
            Dim Result As String = ""
            Dim stru() As stru_documentos_colaboracion = Nothing
            Result = Me.Lista_documentos_colaboracion_usuario_por_nombre_archivo(nombre_documento, stru)
            If Result <> "YES" Then
                Elimina_documento_colaboracion_documento_compartido_dorw_list = Result
                Exit Function
            End If
            Dim Delte_sql As String = "Delete from ra_cd_doumentos_colaboracion_compartidos where nombre_archivo='" & nombre_documento & "'"
            'Elimina_documento_colaboracion_documento_compartido_dorw_list = "YES"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Result = ref.SELECTION_INSERT_COMMAND(Delte_sql)
            If Result <> "YES" Then
                Elimina_documento_colaboracion_documento_compartido_dorw_list = Result
                Exit Function
            Else
                drow_list.Items.Remove(nombre_documento)
                update.Update()
                Dim ref_class As New ClassEliminarDocListResult
                Result = ref_class.EliminarDocumentosGabinete(stru(0).ID_IMAGEN,
                                                                    0,
                                                                    stru(0).NOMBRE_GABINETE,
                                                                    0,
                                                                    1,
                                                                    0,
                                                                    -1,
                                                                    "DOCUMENTOCOMPARTIDO")
                If Result <> "YES" Then
                    Elimina_documento_colaboracion_documento_compartido_dorw_list = "El documento se elimino de la lista pero no se del gabinete por este error " & Result
                    Exit Function
                Else
                    Elimina_documento_colaboracion_documento_compartido_dorw_list = "YES"
                    Exit Function
                End If
            End If
        Catch ex As Exception
            Elimina_documento_colaboracion_documento_compartido_dorw_list = "Inconsistencia general función Elimina_documento_colaboracion_documento_compartido_dorw_list " & ex.Message
        End Try
    End Function

    Function Registrar_nota_documento_compartido(ByVal nota_documento_compartido As String, _
                                                 ByVal id_documento_compartido As Integer, _
                                                 ByVal id_usuario_documento_compartido As Integer, _
                                                 ByRef id_nota As Integer) As String
        '---------------------------------------------------
        'Función : Registrar nota a documento compartido
        'Ing : Miguel Angel Urueta Miranda
        'Fecha : 2017-06-01
        '---------------------------------------------------
        Try
            Dim refclas_gestion_fechas As New ClassGestionFechas
            Dim date1al As String = Date.Now
            Dim Result As String = ""
            Result = refclas_gestion_fechas.Formatea_Fecha_Almacenamiento_Time(date1al)
            If Result <> "YES" Then
                Registrar_nota_documento_compartido = "Imposible formatear fecha " & Result
                Exit Function
            End If
            Dim Insert_sql As String = "INSERT INTO ra_cd_notas_usuario_documento_compartido (ID_USUARIOS_DOCUMENTOS_COMPARTIDOS,DOC_COMP_ID_RA_CD_DOCUMENTOS_COMPARTIDOS," & _
                "NOTA_DOCUMENTO_COMPARTIDO,FECHA_NOTA_SOLICITUD) VALUES (" & id_usuario_documento_compartido & "," & id_documento_compartido & ",'" & nota_documento_compartido & _
                "','" & date1al & "')"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim last_isert As Object = 0
            Result = ref.SELECTION_LAST_INSERT_COMMAND(Insert_sql, last_isert)
            If Result <> "YES" Then
                Registrar_nota_documento_compartido = Result
                Exit Function
            Else
                id_nota = last_isert
                Registrar_nota_documento_compartido = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Registrar_nota_documento_compartido = "Inconsistencia general función Registrar_nota_documento_compartido " & ex.Message
        End Try
    End Function

    Function Actualiza_nota_documento_compartido(ByVal id_nota_documento As Integer, _
                                                 ByVal nota_documento_compartido As String) As String
        '-------------------------------------------------------------
        'Función : Actualiza nota documento compartido
        'Ing : Miguel Angel Urueta Miranda
        'Fecha : 2017-06-01
        '-------------------------------------------------------------
        Try
            Dim ref_nota_documento_compartido As String = ""
            If nota_documento_compartido <> "" Then
                ref_nota_documento_compartido = "'" & nota_documento_compartido & "'"
            Else
                ref_nota_documento_compartido = "Null"
            End If
            Dim update_sql As String = "UPDATE ra_cd_notas_usuario_documento_compartido SET NOTA_DOCUMENTO_COMPARTIDO=" & ref_nota_documento_compartido & _
                " where RA_CD_NOTAS_USUARIO_DOCUMENTO_COMPARTIDO=" & id_nota_documento
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim last_isert As Object = 0
            Dim Result As String = ref.SELECTION_INSERT_COMMAND(update_sql)
            If Result <> "YES" Then
                Actualiza_nota_documento_compartido = Result
                Exit Function
            Else
                Actualiza_nota_documento_compartido = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Actualiza_nota_documento_compartido = "Inconsistencia general función Actualiza_nota_documento_compartido "
        End Try
    End Function
    Function Retorna_estado_confirmacion_documento_colaboracion_usuario(ByVal id_documento_compartido As Integer, _
                                                                        ByVal id_usuario_documento_compartido As Integer, _
                                                                        ByRef id_estado As Integer) As String
        '----------------------------------------------------
        'Función : Retorna estado confirmación colaboracion 
        'colaboración documento compartido
        'Fecha : 2017-06-02
        'Ing : Miguel Angel Urueta Miranda
        '-----------------------------------------------------
        Try
            Dim sql_consulta As String = "SELECT ESTADO_CONFIRMACION_COLABORACION  from ra_cd_usuarios_documentos_compartidos " & _
                          " where ID_RA_CD_DOCUMENTOS_COMPARTIDOS=" & id_documento_compartido & " and ID_USUARIOS_DOCUMENTOS_COMPARTIDOS=" & id_usuario_documento_compartido
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_notas_usuario_documento_compartido")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_estado_confirmacion_documento_colaboracion_usuario = "Error listando Retorna_estado_confirmacion_documento_colaboracion_usuario " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_estado_confirmacion_documento_colaboracion_usuario = "Imposible encontrar el estado colaboración del documento compartido, código documento compartido " & id_documento_compartido & " código usuario compartido " & id_usuario_documento_compartido
                Exit Function
            Else
                id_estado = Datset.Tables(0).Rows(0).Item(0)
                Retorna_estado_confirmacion_documento_colaboracion_usuario = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_estado_confirmacion_documento_colaboracion_usuario = "Inconsistencia general función Retorna_estado_confirmacion_documento_colaboracion_usuario " & ex.Message
        End Try
    End Function

    Function Retorna_contenido_nota_id_documento_compartido(ByVal id_documento_compartido As Integer, _
                                                            ByVal id_usuario_documento_compartido As Integer, _
                                                            ByRef contenido_nota_documento_compartido As String, _
                                                            ByRef id_nota_documento_compartido As Integer) As String
        '--------------------------------------------
        'Función : Retorna id usuario nota y 
        'contenido de la nota del documento compatido
        'Fecha : 2017-05-31
        'Ing : Miguel Angel Urueta Miranda
        '--------------------------------------------
        Try
            Dim sql_consulta As String = "SELECT RA_CD_NOTAS_USUARIO_DOCUMENTO_COMPARTIDO,NOTA_DOCUMENTO_COMPARTIDO  from ra_cd_notas_usuario_documento_compartido " & _
                          " where DOC_COMP_ID_RA_CD_DOCUMENTOS_COMPARTIDOS=" & id_documento_compartido & " and ID_USUARIOS_DOCUMENTOS_COMPARTIDOS=" & id_usuario_documento_compartido
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_notas_usuario_documento_compartido")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_contenido_nota_id_documento_compartido = "Error listando Retorna_contenido_nota_id_documento_compartido " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_contenido_nota_id_documento_compartido = "YES"
                Exit Function
            Else
                id_nota_documento_compartido = Datset.Tables(0).Rows(0).Item(0)
                If Datset.Tables(0).Rows(0).IsNull(1) Then
                    contenido_nota_documento_compartido = ""
                Else
                    contenido_nota_documento_compartido = Datset.Tables(0).Rows(0).Item(1)
                End If

                Retorna_contenido_nota_id_documento_compartido = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_contenido_nota_id_documento_compartido = "Inconsistencia general función Retorna_contenido_nota_id_documento_compartido " & ex.Message
        End Try
    End Function
    Function Retorna_numero_documentos_compartidos_pendientes_colaborar(ByVal id_documento_compartido_general As Integer, _
                                                                        ByRef numero_documento_por_confirmar As Integer) As String
        '---------------------------------------------------
        'Función : Retorna numero de documentos compartidos
        'pendientes por confirmar
        'Fecha 2017-06-02
        'Ing Miguel Angel Urueta Miranda
        '---------------------------------------------------
        Try
            Dim sql_consulta As String = "SELECT ESTADO_CONFIRMACION_COLABORACION  from ra_cd_usuarios_documentos_compartidos " & _
                             " where ID_RA_CD_DOCUMENTOS_COMPARTIDOS=" & id_documento_compartido_general
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_usuarios_documentos_compartidos")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_numero_documentos_compartidos_pendientes_colaborar = "Error listando Retorna_numero_documentos_compartidos_pendientes_colaborar " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_numero_documentos_compartidos_pendientes_colaborar = "Imposible encontrar solicitudes de documentos compartidos para la solicitud " & id_documento_compartido_general
                Exit Function
            Else
                Dim numero_compartido As Integer = Datset.Tables(0).Rows.Count
                Dim numero_documentos_confirmados As Integer = 0
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    If Datset.Tables(0).Rows(i).Item(0) = 1 Then
                        numero_documentos_confirmados = numero_documentos_confirmados + 1
                    End If
                Next
                numero_documento_por_confirmar = numero_compartido - numero_documentos_confirmados
                Retorna_numero_documentos_compartidos_pendientes_colaborar = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_numero_documentos_compartidos_pendientes_colaborar = "Inconsistencia general función "
        End Try
    End Function
    Function Retorna_id_documento_compartido(ByVal id_documento_compartido_general As Integer, _
                                             ByRef id_documento_compartido As Integer, _
                                             ByVal id_usuario_gestion As Integer) As String
        '--------------------------------------------
        'Función : Retorna id documento compartido
        'al documento compartido
        'Fecha : 2017-05-31
        'Ing : Miguel Angel Urueta Miranda
        '--------------------------------------------
        Try
            Dim sql_consulta As String = "SELECT ID_USUARIOS_DOCUMENTOS_COMPARTIDOS  from ra_cd_usuarios_documentos_compartidos " & _
                          " where ID_RA_CD_DOCUMENTOS_COMPARTIDOS=" & id_documento_compartido_general & " and Remit_Dest_Interno_id_Remit_Dest_Int=" & id_usuario_gestion
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_usuarios_documentos_compartidos")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_id_documento_compartido = "Error listando Retorna_id_documento_compartido " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_id_documento_compartido = "Imponsible encontrar el identificador del documento compartido para el usuario numero " & id_documento_compartido_general
                Exit Function
            Else
                id_documento_compartido = Datset.Tables(0).Rows(0).Item(0)
                Retorna_id_documento_compartido = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_documento_compartido = "Inconsistencia general función Retorna_id_documento_compartido " & ex.Message
        End Try
    End Function
    Function Retorna_id_usuario_documento_compartido(ByVal id_documento_compartido As Integer, _
                                                     ByRef id_usuario_compartido As Integer, _
                                                     ByVal id_usuario_gestion As Integer) As String
        '--------------------------------------------
        'Función : Retorna id usuario relacionado
        'al documento compartido
        'Fecha : 2017-05-31
        'Ing : Miguel Angel Urueta Miranda
        '--------------------------------------------
        Try
            Dim sql_consulta As String = "SELECT ID_USUARIOS_DOCUMENTOS_COMPARTIDOS  from ra_cd_usuarios_documentos_compartidos " & _
                          " where ID_RA_CD_DOCUMENTOS_COMPARTIDOS=" & id_documento_compartido & " and Remit_Dest_Interno_id_Remit_Dest_Int=" & id_usuario_gestion
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_usuarios_documentos_compartidos")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_id_usuario_documento_compartido = "Error listando Retorna_id_usuario_documento_compartido " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_id_usuario_documento_compartido = "Imponsible encontrar el id usuario del documento compartido"
                Exit Function
            Else
                id_usuario_compartido = Datset.Tables(0).Rows(0).Item(0)
                Retorna_id_usuario_documento_compartido = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_usuario_documento_compartido = "Inconsistencia general función Retorna_id_usuario_documento_compartido " & ex.Message
        End Try
    End Function
    Function Lista_documentos_colaboracion_usuario_por_nombre_archivo(ByVal nombre_archivo As String, _
                                                                      ByRef stru() As stru_documentos_colaboracion) As String
        '--------------------------------------------
        'Función : Retorna lista de documentos
        'de colaboración de un usuario 
        'Fecha : 2017-06-05
        'Ing : Miguel Angel Urueta Miranda
        '--------------------------------------------
        Try
            Dim sql_consulta As String = "SELECT ID_IMAGEN,NOMBRE_GABINETE,nombre_archivo  from ra_cd_doumentos_colaboracion_compartidos " & _
                          " where nombre_archivo='" & nombre_archivo & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_usuarios_documentos_compartidos")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_documentos_colaboracion_usuario_por_nombre_archivo = "Error listando Lista_documentos_colaboracion_usuario_por_nombre_archivo " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Lista_documentos_colaboracion_usuario_por_nombre_archivo = "No se encontraron documentos de colaboración "
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru(i)
                    stru(i).ID_IMAGEN = Datset.Tables(0).Rows(i).Item(0)
                    stru(i).NOMBRE_GABINETE = Datset.Tables(0).Rows(i).Item(1)
                    stru(i).nombre_archivo = Datset.Tables(0).Rows(i).Item(2)
                Next
                Lista_documentos_colaboracion_usuario_por_nombre_archivo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_documentos_colaboracion_usuario_por_nombre_archivo = "Inconsistencia general función Lista_documentos_colaboracion_usuario_por_nombre_archivo " & ex.Message
        End Try
    End Function
    Function Lista_documentos_colaboracion_usuario(ByVal id_usuario_compartido As Integer, _
                                                   ByRef stru() As stru_documentos_colaboracion, _
                                                   ByRef ref_label_estado As Label, _
                                                   ByRef update_pane As UpdatePanel) As String
        '--------------------------------------------
        'Función : Retorna lista de documentos
        'de colaboración de un usuario 
        'Fecha : 2017-06-05
        'Ing : Miguel Angel Urueta Miranda
        '--------------------------------------------
        Try
            Dim sql_consulta As String = "SELECT ID_IMAGEN,NOMBRE_GABINETE,nombre_archivo  from ra_cd_doumentos_colaboracion_compartidos " & _
                          " where ID_USUARIOS_DOCUMENTOS_COMPARTIDOS=" & id_usuario_compartido
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_usuarios_documentos_compartidos")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_documentos_colaboracion_usuario = "Error listando Lista_documentos_colaboracion_usuario " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                ref_label_estado.Text = "Numero registro(s) " & Datset.Tables(0).Rows.Count
                update_pane.Update()
                Lista_documentos_colaboracion_usuario = "No se encontraron documentos de colaboración "
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru(i)
                    stru(i).ID_IMAGEN = Datset.Tables(0).Rows(i).Item(0)
                    stru(i).NOMBRE_GABINETE = Datset.Tables(0).Rows(i).Item(1)
                    stru(i).nombre_archivo = Datset.Tables(0).Rows(i).Item(2)
                Next
                ref_label_estado.Text = "Numero registro(s) " & Datset.Tables(0).Rows.Count
                update_pane.Update()
                Lista_documentos_colaboracion_usuario = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_documentos_colaboracion_usuario = "Inconsistencia general función Lista_documentos_colaboracion_usuario " & ex.Message
        End Try
    End Function
    Function Guardar_Documento_colaboracion(
                                            ByRef Id_imagen As Integer, _
                                            ByVal Nombre_Gabinete As String, _
                                            ByVal id_documento_compartido As Integer, _
                                            ByVal ruta_document As String) As String
        Try
            Dim Refeclasaladir As New ClassAñadirDocumento
            Dim MatriDatosAlmacen() As String
            Erase MatriDatosAlmacen
            Dim Result As String = ""
            Dim Refalmacena As New ClassAlmacenamiento
            Dim option_unidad_conservacion As Integer = 0
            Dim ref_Class_system1 As New Class_system1
            Result = ref_Class_system1.Verfica_opcion_seleccion_unidad(option_unidad_conservacion, _
                                                                       Nombre_Gabinete)
            If Result <> "YES" Then
                Guardar_Documento_colaboracion = "Inconsistencia verficando opción asignación unidad y expediente codigo :  " & Result
                Exit Function
            End If
            Dim matri_datos() As Datos_Almacenamiento
            ReDim Preserve matri_datos(0)
            matri_datos(0).nombre_campo = "NUMERORADICA"
            matri_datos(0).valor_campo = ""
            ReDim Preserve matri_datos(1)
            matri_datos(1).nombre_campo = "ENLASE"
            matri_datos(1).valor_campo = ""
            ReDim Preserve matri_datos(2)
            matri_datos(2).nombre_campo = "IDREGCOLABORA"
            matri_datos(2).valor_campo = id_documento_compartido
            If option_unidad_conservacion = 1 Then
                ReDim Preserve matri_datos(3)
                matri_datos(3).nombre_campo = "CLASEDOCUMENTO"
                matri_datos(3).valor_campo = "DOCUMENTO ELECTRONICO"
                Dim date1al As String = Date.Today
                Result = ""
                Dim ref_ClassGestionFechas As New ClassGestionFechas
                Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
                If Result <> "YES" Then
                    Guardar_Documento_colaboracion = "Error formatenado fecha alamcenamiento Funcion: Formatea_Fecha_Almacenamiento " & Result
                    Exit Function
                End If
                ReDim Preserve matri_datos(3)
                matri_datos(3).nombre_campo = "CLASEDOCUMENTO"
                matri_datos(3).valor_campo = "DOCUMENTO ELECTRONICO"
            End If
            Dim RefclasAñadir As New ClassAñadirDocumento
            Dim Refclaswfdigtializado As New ClassWorkflowDigitalizacion
            Dim Matri_Datos_Almacen() As String
            Erase Matri_Datos_Almacen
            Result = Refclaswfdigtializado.Obtiene_Valores_Campos_Documento_Enlazados(Matri_Datos_Almacen, Nombre_Gabinete, matri_datos)
            If Result <> "YES" Then
                Guardar_Documento_colaboracion = "Imposible general mariz datos almacenamiento "
                Exit Function
            End If
            If Matri_Datos_Almacen Is Nothing Then
                Guardar_Documento_colaboracion = "Matriz de datos de almacenamiento es nothing "
                Exit Function
            End If
            Dim Filein As New IO.FileInfo(ruta_document)
            Result = ""
            Dim Tipo_Doc_int As Integer = -1
            Dim Refclasvisor As New Classactualizacionvisor
            Result = ""
            Dim Class_da_extension As New Class_da_extension
            Result = Class_da_extension.SolicitaTipoArchivoDocuarchiExtension(UCase(Filein.Extension), _
                                                                          Tipo_Doc_int)
            If Result <> "YES" Then
                Guardar_Documento_colaboracion = Result
                Exit Function
            End If
            '-------------------------------------------
            'Asigna datos gestion
            '-------------------------------------------
            Dim matri_gestion As estructure_gestion = Nothing
            'matri_gestion = Nothing
            matri_gestion.CLASE_DOCUMENTO = ""
            matri_gestion.EXPEDIENTE = ""
            matri_gestion.ID_AREA = 0
            matri_gestion.ID_CLASE_DOCUMENTO = 0
            matri_gestion.ID_EXPEDIENTE = 0
            matri_gestion.ID_SERIE = 0
            matri_gestion.ID_SUB_SERIE = 0
            matri_gestion.ID_TIPO_EXPEDIENTE = 0
            matri_gestion.ID_TIPO_UNIDAD_CONSERVACION = 0
            matri_gestion.ID_TIPODOCUMENTO = 0
            matri_gestion.ID_UNIDAD_CONSERVACION = 0
            matri_gestion.ID_USUARIO_GESTION = 0
            matri_gestion.TIPO_UNIDAD_DOCUMENTAL = 0
            matri_gestion.UNIDAD_CONSERVACION = ""
            matri_gestion.FECHA_ELABORACION = ""
            Id_imagen = Tipo_Doc_int
            Dim radicado As String = ""
            Dim matri_documento() As String = {ruta_document}
            Result = Refalmacena.Almacenamiento("", "", Nombre_Gabinete, 0, Matri_Datos_Almacen, _
            2, 1, Tipo_Doc_int, matri_documento, 0, Id_imagen, Tipo_Doc_int, HttpContext.Current.Session.Item("GA_IDEMPRESA"), HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), matri_gestion.ID_AREA, matri_gestion.ID_SERIE, matri_gestion.ID_SUB_SERIE, _
            matri_gestion.ID_TIPODOCUMENTO, matri_gestion.ID_EXPEDIENTE, matri_gestion.ID_TIPO_EXPEDIENTE, _
            matri_gestion.ID_UNIDAD_CONSERVACION, matri_gestion.ID_TIPO_UNIDAD_CONSERVACION, _
            matri_gestion.ID_CLASE_DOCUMENTO, matri_gestion.EXPEDIENTE, matri_gestion.NOMBRE_SERIE, _
            matri_gestion.NOMBRE_SUB_SERIE, matri_gestion.TIPODOCUMENTO, matri_gestion.UNIDAD_CONSERVACION, _
            matri_gestion.CLASE_DOCUMENTO, matri_gestion.FECHA_ELABORACION, radicado)
            If Result <> "YES" Then
                Guardar_Documento_colaboracion = "Almacenando  dice " & Result
                Exit Function
            End If

            Guardar_Documento_colaboracion = "YES"
            Exit Function
        Catch ex As Exception
            Guardar_Documento_colaboracion = "Inconsistencia función Guardar_Documento_colaboracion " & ex.Message
        End Try
    End Function


    Function Perfila_interface_compartir_documento(ByVal id_documento_compatido As Integer, _
                                                   ByVal descripcion_tipo As String, _
                                                   ByRef Page_manager As Page) As String
        Try
            Dim refclas_cd_compartidos As New Class_ra_Cd_Documentos_Compartidos
            Dim Result As String = ""
            Dim stru_documento_compartido() As stru_documentos_compartidos = HttpContext.Current.Session.Item("GA_STRU_DOCUMENTO_COMPARTIDO")
            Dim numero_documentos As Integer = 0
            If Not stru_documento_compartido Is Nothing Then
                numero_documentos = stru_documento_compartido.Length
            End If
            If descripcion_tipo = "VER DOCUMENTOS" Then
                Dim stru_compartido_general As STRU_DOCUMENTO_COMPARTIDO_GENERAL = Nothing
                Result = refclas_cd_compartidos.SolicitaEstructuraGeneraldocumentosCompartido(id_documento_compatido,
                                                                                              stru_compartido_general)
                If Result <> "YES" Then
                    Perfila_interface_compartir_documento = Result
                    Exit Function
                End If
                Dim usuarios_relacionados As String = ""
                Result = Me.Retorna_usuarios_relacionados_a_documento_compartido(id_documento_compatido, _
                                                                                 usuarios_relacionados)
                If Result <> "YES" Then
                    Perfila_interface_compartir_documento = Result
                    Exit Function
                End If
                Dim ref_Button_anexar_aportes_colaboracion As Button = Page_manager.FindControl("Button_anexar_aportes_colaboracion")
                Dim ref_Button_responder As Button = Page_manager.FindControl("Button_responder")
                Dim ref_Button_compartir_documento As Button = Page_manager.FindControl("Button_compartir_documento")
                Dim ref_DropDownList_prioridad_solicitud As DropDownList = Page_manager.FindControl("DropDownList_prioridad_solicitud")
                Dim ref_DropDownList_tipo_documento_compartir As DropDownList = Page_manager.FindControl("DropDownList_tipo_documento_compartir")
                Dim ref_Label_fecha_limite As Label = Page_manager.FindControl("Label_fecha_limite")
                Dim ref_TextBox_fecha_limite_solicitud As TextBox = Page_manager.FindControl("TextBox_fecha_limite_solicitud")
                Dim ref_TextBox_asunto_documento As TextBox = Page_manager.FindControl("TextBox_asunto_documento")
                Dim ref_TextBox_nota_documento As TextBox = Page_manager.FindControl("TextBox_nota_documento")
                Dim ref_Panel_selecion_user As Panel = Page_manager.FindControl("Panel_selecion_user")
                Dim ref_Label_title As Label = Page_manager.FindControl("Label_title")
                ref_Label_title.Text = numero_documentos & " documento(s) compartido(s) para visualización"
                ref_Panel_selecion_user.Visible = False
                ref_Button_responder.Visible = False
                ref_Button_anexar_aportes_colaboracion.Visible = False
                ref_Button_compartir_documento.Visible = False
                ref_DropDownList_prioridad_solicitud.Enabled = False
                ref_DropDownList_tipo_documento_compartir.Enabled = False
                ref_DropDownList_tipo_documento_compartir.Text = stru_compartido_general.DESCRIPCION_TIPO_COMPARTIDO
                Dim tempo_fecha As String = ""
                If stru_compartido_general.FECHA_LIMITE_RESPUESTA <> "" Then
                    tempo_fecha = stru_compartido_general.FECHA_LIMITE_RESPUESTA.Replace("/", "-")
                    Dim spli() As String = tempo_fecha.Split("-")
                    tempo_fecha = spli(2) & "-" & spli(1) & "-" & spli(0)
                End If
                ref_TextBox_fecha_limite_solicitud.Text = tempo_fecha
                ref_TextBox_asunto_documento.Text = stru_compartido_general.ASUNTO_DOCUMENTO
                ref_TextBox_nota_documento.Text = stru_compartido_general.NOTA_SOLICITUD
                ref_TextBox_nota_documento.Enabled = False
                ref_TextBox_asunto_documento.Enabled = False
                ref_TextBox_nota_documento.BackColor = Color.White
                ref_TextBox_asunto_documento.BackColor = Color.White
                ref_TextBox_fecha_limite_solicitud.Enabled = False
            End If

            If descripcion_tipo = "Para colaboración" Then
                Dim stru_compartido_general As STRU_DOCUMENTO_COMPARTIDO_GENERAL = Nothing
                Result = refclas_cd_compartidos.SolicitaEstructuraGeneraldocumentosCompartido(id_documento_compatido,
                                                                                             stru_compartido_general)
                If Result <> "YES" Then
                    Perfila_interface_compartir_documento = Result
                    Exit Function
                End If
                Dim usuarios_relacionados As String = ""
                Result = Me.Retorna_usuarios_relacionados_a_documento_compartido(id_documento_compatido, _
                                                                                 usuarios_relacionados)
                If Result <> "YES" Then
                    Perfila_interface_compartir_documento = Result
                    Exit Function
                End If
                Dim ref_Button_anexar_aportes_colaboracion As Button = Page_manager.FindControl("Button_anexar_aportes_colaboracion")
                Dim ref_Button_responder As Button = Page_manager.FindControl("Button_responder")
                Dim ref_Button_compartir_documento As Button = Page_manager.FindControl("Button_compartir_documento")
                Dim ref_DropDownList_prioridad_solicitud As DropDownList = Page_manager.FindControl("DropDownList_prioridad_solicitud")
                Dim ref_DropDownList_tipo_documento_compartir As DropDownList = Page_manager.FindControl("DropDownList_tipo_documento_compartir")
                Dim ref_Label_fecha_limite As Label = Page_manager.FindControl("Label_fecha_limite")
                Dim ref_TextBox_fecha_limite_solicitud As TextBox = Page_manager.FindControl("TextBox_fecha_limite_solicitud")
                Dim ref_TextBox_asunto_documento As TextBox = Page_manager.FindControl("TextBox_asunto_documento")
                Dim ref_TextBox_nota_documento As TextBox = Page_manager.FindControl("TextBox_nota_documento")
                Dim ref_Panel_selecion_user As Panel = Page_manager.FindControl("Panel_selecion_user")
                Dim ref_Label_title As Label = Page_manager.FindControl("Label_title")
                ref_Label_title.Text = numero_documentos & " documento(s) compartido(s) para colaboración"
                ref_Panel_selecion_user.Visible = False
                ref_Button_responder.Visible = False
                ref_Button_compartir_documento.Visible = False
                ref_DropDownList_prioridad_solicitud.Enabled = False
                ref_DropDownList_tipo_documento_compartir.Enabled = False
                ref_Button_anexar_aportes_colaboracion.Visible = True
                ref_DropDownList_tipo_documento_compartir.Text = stru_compartido_general.DESCRIPCION_TIPO_COMPARTIDO
                Dim tempo_fecha As String = ""
                If stru_compartido_general.FECHA_LIMITE_RESPUESTA <> "" Then
                    tempo_fecha = stru_compartido_general.FECHA_LIMITE_RESPUESTA.Replace("/", "-")
                    Dim spli() As String = tempo_fecha.Split("-")
                    tempo_fecha = spli(2) & "-" & spli(1) & "-" & spli(0)
                End If
                ref_TextBox_fecha_limite_solicitud.Text = tempo_fecha
                ref_TextBox_asunto_documento.Text = stru_compartido_general.ASUNTO_DOCUMENTO
                ref_TextBox_nota_documento.Text = stru_compartido_general.NOTA_SOLICITUD
                ref_TextBox_nota_documento.Enabled = False
                ref_TextBox_asunto_documento.Enabled = False
                ref_TextBox_nota_documento.BackColor = Color.White
                ref_TextBox_asunto_documento.BackColor = Color.White
                ref_TextBox_fecha_limite_solicitud.Enabled = False
            End If
            If descripcion_tipo = "Informativo" Or descripcion_tipo = "Para aprobación" Then
                Dim stru_compartido_general As STRU_DOCUMENTO_COMPARTIDO_GENERAL = Nothing
                Result = refclas_cd_compartidos.SolicitaEstructuraGeneraldocumentosCompartido(id_documento_compatido,
                                                                                              stru_compartido_general)
                If Result <> "YES" Then
                    Perfila_interface_compartir_documento = Result
                    Exit Function
                End If
                Dim usuarios_relacionados As String = ""
                Result = Me.Retorna_usuarios_relacionados_a_documento_compartido(id_documento_compatido, usuarios_relacionados)
                If Result <> "YES" Then
                    Perfila_interface_compartir_documento = Result
                    Exit Function
                End If
                Dim ref_Button_anexar_aportes_colaboracion As Button = Page_manager.FindControl("Button_anexar_aportes_colaboracion")
                Dim ref_Button_responder As Button = Page_manager.FindControl("Button_responder")
                Dim ref_Button_compartir_documento As Button = Page_manager.FindControl("Button_compartir_documento")
                Dim ref_DropDownList_prioridad_solicitud As DropDownList = Page_manager.FindControl("DropDownList_prioridad_solicitud")
                Dim ref_DropDownList_tipo_documento_compartir As DropDownList = Page_manager.FindControl("DropDownList_tipo_documento_compartir")
                Dim ref_Label_fecha_limite As Label = Page_manager.FindControl("Label_fecha_limite")
                Dim ref_TextBox_fecha_limite_solicitud As TextBox = Page_manager.FindControl("TextBox_fecha_limite_solicitud")
                Dim ref_TextBox_asunto_documento As TextBox = Page_manager.FindControl("TextBox_asunto_documento")
                Dim ref_TextBox_nota_documento As TextBox = Page_manager.FindControl("TextBox_nota_documento")
                Dim ref_Panel_selecion_user As Panel = Page_manager.FindControl("Panel_selecion_user")
                Dim ref_Label_title As Label = Page_manager.FindControl("Label_title")
                ref_Label_title.Text = numero_documentos & " documento(s) compartido(s) " & descripcion_tipo
                ref_Panel_selecion_user.Visible = False
                ref_Button_responder.Visible = False
                ref_Button_compartir_documento.Visible = False
                ref_DropDownList_prioridad_solicitud.Enabled = False
                ref_DropDownList_tipo_documento_compartir.Enabled = False
                ref_Button_anexar_aportes_colaboracion.Visible = False
                ref_DropDownList_tipo_documento_compartir.Text = stru_compartido_general.DESCRIPCION_TIPO_COMPARTIDO
                Dim tempo_fecha As String = ""
                If stru_compartido_general.FECHA_LIMITE_RESPUESTA <> "" Then
                    tempo_fecha = stru_compartido_general.FECHA_LIMITE_RESPUESTA.Replace("/", "-")
                    Dim spli() As String = tempo_fecha.Split("-")
                    tempo_fecha = spli(2) & "-" & spli(1) & "-" & spli(0)
                End If
                ref_TextBox_fecha_limite_solicitud.Text = tempo_fecha
                ref_TextBox_asunto_documento.Text = stru_compartido_general.ASUNTO_DOCUMENTO
                ref_TextBox_nota_documento.Text = stru_compartido_general.NOTA_SOLICITUD
                ref_TextBox_nota_documento.Enabled = False
                ref_TextBox_asunto_documento.Enabled = False
                ref_TextBox_nota_documento.BackColor = Color.White
                ref_TextBox_asunto_documento.BackColor = Color.White
            End If
            If descripcion_tipo = "COMPARTIR WORKFLOW" Then
                Dim ref_Button_anexar_aportes_colaboracion As Button = Page_manager.FindControl("Button_anexar_aportes_colaboracion")
                Dim ref_Button_responder As Button = Page_manager.FindControl("Button_responder")
                Dim ref_Button_compartir_documento As Button = Page_manager.FindControl("Button_compartir_documento")
                Dim ref_DropDownList_prioridad_solicitud As DropDownList = Page_manager.FindControl("DropDownList_prioridad_solicitud")
                Dim ref_DropDownList_tipo_documento_compartir As DropDownList = Page_manager.FindControl("DropDownList_tipo_documento_compartir")
                Dim ref_Label_fecha_limite As Label = Page_manager.FindControl("Label_fecha_limite")
                Dim ref_TextBox_fecha_limite_solicitud As TextBox = Page_manager.FindControl("TextBox_fecha_limite_solicitud")
                Dim ref_TextBox_asunto_documento As TextBox = Page_manager.FindControl("TextBox_asunto_documento")
                Dim ref_TextBox_nota_documento As TextBox = Page_manager.FindControl("TextBox_nota_documento")
                ref_Button_anexar_aportes_colaboracion.Visible = False
                ref_Button_responder.Visible = False
                ref_Button_compartir_documento.Visible = True
                ref_DropDownList_prioridad_solicitud.Enabled = True
                ref_DropDownList_tipo_documento_compartir.Enabled = True
            End If
            If descripcion_tipo = "COMPARTIR RESPUESTA" Then
                Dim ref_Button_anexar_aportes_colaboracion As Button = Page_manager.FindControl("Button_anexar_aportes_colaboracion")
                Dim ref_Button_responder As Button = Page_manager.FindControl("Button_responder")
                Dim ref_Button_compartir_documento As Button = Page_manager.FindControl("Button_compartir_documento")
                Dim ref_DropDownList_prioridad_solicitud As DropDownList = Page_manager.FindControl("DropDownList_prioridad_solicitud")
                Dim ref_DropDownList_tipo_documento_compartir As DropDownList = Page_manager.FindControl("DropDownList_tipo_documento_compartir")
                Dim ref_Label_fecha_limite As Label = Page_manager.FindControl("Label_fecha_limite")
                Dim ref_TextBox_fecha_limite_solicitud As TextBox = Page_manager.FindControl("TextBox_fecha_limite_solicitud")
                Dim ref_TextBox_asunto_documento As TextBox = Page_manager.FindControl("TextBox_asunto_documento")
                Dim ref_TextBox_nota_documento As TextBox = Page_manager.FindControl("TextBox_nota_documento")
                ref_Button_anexar_aportes_colaboracion.Visible = False
                ref_Button_responder.Visible = False
                ref_Button_compartir_documento.Visible = True
                ref_DropDownList_prioridad_solicitud.Enabled = True
                ref_DropDownList_tipo_documento_compartir.Enabled = True
                ref_DropDownList_tipo_documento_compartir.Items.Clear()
                ref_DropDownList_tipo_documento_compartir.Items.Add("Para colaboración")
            End If
            Perfila_interface_compartir_documento = "YES"
        Catch ex As Exception
            Perfila_interface_compartir_documento = "Inconsistencia general función Perfila_interface_compartir_documento " & ex.Message
        End Try
    End Function
    Function Lista_documentos_colaboracion_interface(ByRef Page_manager As Page, _
                                                     ByVal id_usuario_documento_compartido As Integer) As String
        '--------------------------------------------------------------
        'Función : Genera interface documentos relacionados de colabora
        'ción
        'ing : Miguel Angel Urueta Miranda
        'Fecha : 2017-06-05
        '---------------------------------------------------------------
        Try
            Dim pane As Panel = Page_manager.FindControl("Panel_seleccion_documento")
            Dim Update As UpdatePanel = Page_manager.FindControl("UpdatePanel_seleccion_documento")
            Dim Table As Table = Page_manager.FindControl("Table_seleccion_documento")
            Dim Label As Label = Page_manager.FindControl("Label_estado_doc_colaboracion")
            Dim Update_panel As UpdatePanel = Page_manager.FindControl("UpdatePanel_estado_doc_colaboracion")
            Dim objRow As TableRow
            Dim objCell As TableCell
            Dim label_nombre() As Label = {}
            Dim buton_decarga() As HtmlGenericControl = {}
            Dim Result As String = ""
            Dim stru() As stru_documentos_colaboracion = Nothing
            Result = Me.Lista_documentos_colaboracion_usuario(id_usuario_documento_compartido, stru, Label, Update_panel)
            If Result <> "YES" Then
                Lista_documentos_colaboracion_interface = Result
                Exit Function
            End If
            Dim i_conlabel As Integer = 0
            For i As Integer = 0 To stru.Length - 1
                If i = 0 Then
                    objRow = New TableRow
                    objCell = New TableCell
                    ReDim Preserve label_nombre(i_conlabel)
                    label_nombre(i_conlabel) = New Label
                    label_nombre(i_conlabel).Text = "DOCUMENTO"
                    objCell.Controls.Add(label_nombre(i_conlabel))
                    objCell.HorizontalAlign = HorizontalAlign.Center
                    'objCell.BorderWidth = 4
                    'objCell.BorderColor = Color.Blue
                    objRow.Cells.Add(objCell)
                    objCell = New TableCell
                    i_conlabel = i_conlabel + 1
                    ReDim Preserve label_nombre(i_conlabel)
                    label_nombre(i_conlabel) = New Label
                    label_nombre(i_conlabel).Text = "DESCARGA"
                    objCell.Controls.Add(label_nombre(i_conlabel))
                    objCell.HorizontalAlign = HorizontalAlign.Center
                    'objCell.BorderWidth = 4
                    'objCell.BorderColor = Color.Blue
                    objRow.Cells.Add(objCell)
                    Table.Rows.Add(objRow)
                    i_conlabel = i_conlabel + 1
                End If
                objRow = New TableRow
                objCell = New TableCell
                'objCell.HorizontalAlign = HorizontalAlign.Center
                ReDim Preserve label_nombre(i_conlabel)
                label_nombre(i_conlabel) = New Label
                label_nombre(i_conlabel).Text = stru(i).nombre_archivo
                objCell.Controls.Add(label_nombre(i_conlabel))
                objRow.Cells.Add(objCell)
                i_conlabel = i_conlabel + 1
                objCell = New TableCell
                objCell.HorizontalAlign = HorizontalAlign.Center
                ReDim Preserve buton_decarga(i)
                buton_decarga(i) = New HtmlGenericControl("input")
                buton_decarga(i).Attributes.Add("Type", "button")
                buton_decarga(i).Attributes.Add("width", "50px")
                buton_decarga(i).Attributes.Add("value", "Descarga")
                buton_decarga(i).Attributes.Add("class", "boton")
                buton_decarga(i).Attributes.Add("onclick", "dercaga_documento(this);")
                buton_decarga(i).ID = stru(i).ID_IMAGEN & "|" & stru(i).NOMBRE_GABINETE
                objCell.Controls.Add(buton_decarga(i))
                objRow.Cells.Add(objCell)
                Table.Rows.Add(objRow)
            Next
            Update.Update()
            Lista_documentos_colaboracion_interface = "YES"
        Catch ex As Exception
            Lista_documentos_colaboracion_interface = "Inconsistencia general función Lista_documentos_colaboracion_interface " & ex.Message
        End Try
    End Function
    Function Interface_dinamica_documentos_a_compartir(ByRef Page_manager As Page) As String
        Try
            Dim pane As Panel = Page_manager.FindControl("Panel_seleccion_documento")
            Dim Update As UpdatePanel = Page_manager.FindControl("UpdatePanel_seleccion_documento")
            Dim Table As Table = Page_manager.FindControl("Table_seleccion_documento")
            Dim objRow As TableRow
            Dim objCell As TableCell
            Dim m_image() As ImageMap = {}
            Dim item_chek() As HtmlGenericControl = {}
            Dim div_generic() As HtmlGenericControl = {}
            Dim Result As String = ""
            Dim stru_documento_compartido() As stru_documentos_compartidos = HttpContext.Current.Session.Item("GA_STRU_DOCUMENTO_COMPARTIDO")
            If stru_documento_compartido Is Nothing Then
                Interface_dinamica_documentos_a_compartir = "YES"
                Exit Function
            End If
            objRow = New TableRow
            Dim Ruttempo As String = HttpContext.Current.Server.MapPath(HttpContext.Current.Session.Item("GA_RUTA_TEMPO") + HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION").ToString)
            If Directory.Exists(Ruttempo) = False Then
                Directory.CreateDirectory(Ruttempo)
            End If
            Ruttempo = Ruttempo & "\tempo_extra_icono"
            If Directory.Exists(Ruttempo) = False Then
                Directory.CreateDirectory(Ruttempo)
            End If
            Dim url_icono As String = HttpContext.Current.Session.Item("GA_RUTA_TEMPO") + HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION").ToString + "/tempo_extra_icono/"
            Dim contador_div As Integer = 0
            For i As Integer = 0 To stru_documento_compartido.Length - 1
                Dim refclas_visializa As New ClassVisualisaDocumento
                Dim matri_documentos() As String = Nothing
                refclas_visializa.Genera_Matris_Documentos_Almacenados(stru_documento_compartido(i).id_imagen, stru_documento_compartido(i).nombre_gabinete, matri_documentos)
                objCell = New TableCell
                ReDim Preserve m_image(i)
                m_image(i) = New ImageMap
                Dim ref_img As Image = Nothing
                Dim files As New FileInfo(matri_documentos(1))
                stru_documento_compartido(i).tipo_documento = files.Extension
                stru_documento_compartido(i).extension = files.Extension
                extare_icono_archivo(matri_documentos(1), ref_img)
                If Not ref_img Is Nothing Then
                    If File.Exists(Ruttempo & "\" & stru_documento_compartido(i).identificador & ".jpg") = True Then
                        Kill(Ruttempo & "\" & stru_documento_compartido(i).identificador & ".jpg")
                        ref_img.Save(Ruttempo & "\" & stru_documento_compartido(i).identificador & ".jpg")
                    Else
                        ref_img.Save(Ruttempo & "\" & stru_documento_compartido(i).identificador & ".jpg")
                    End If
                    m_image(i).ImageUrl = url_icono & "/" & stru_documento_compartido(i).identificador & ".jpg"
                    m_image(i).ID = stru_documento_compartido(i).identificador & "_" & stru_documento_compartido(i).id_imagen & "_" & stru_documento_compartido(i).nombre_gabinete & "_" & stru_documento_compartido(i).tipo_documento
                End If
                ReDim Preserve div_generic(contador_div)
                div_generic(contador_div) = New HtmlGenericControl("div")
                div_generic(contador_div).Attributes.Add("class", "responsive")
                Dim ob_responsive As HtmlGenericControl = div_generic(contador_div)
                contador_div = contador_div + 1
                ReDim Preserve div_generic(contador_div)
                div_generic(contador_div) = New HtmlGenericControl("div")
                div_generic(contador_div).Attributes.Add("class", "gallery")
                div_generic(contador_div).Attributes.Add("width", "50px")
                Dim ob_gallery As HtmlGenericControl = div_generic(contador_div)
                ob_gallery.ID = stru_documento_compartido(i).identificador & "_" & stru_documento_compartido(i).id_imagen & "_" & stru_documento_compartido(i).nombre_gabinete & "_" & stru_documento_compartido(i).tipo_documento & "_copy"
                contador_div = contador_div + 1
                ReDim Preserve item_chek(i)
                item_chek(i) = New HtmlGenericControl("input")
                item_chek(i).Attributes.Add("checked", "checked")
                item_chek(i).Attributes.Add("Type", "checkbox")
                item_chek(i).InnerText = "Ver documento"
                'item_chek(i).Attributes.Add("font-size", "8px")
                item_chek(i).ID = i & "chek_" & stru_documento_compartido(i).identificador & "_" & stru_documento_compartido(i).id_imagen & "_" & stru_documento_compartido(i).nombre_gabinete & "_" & stru_documento_compartido(i).tipo_documento
                item_chek(i).Attributes.Add("class", "onclik_item_chek")
                ReDim Preserve div_generic(contador_div)
                div_generic(contador_div) = New HtmlGenericControl("div")
                div_generic(contador_div).Attributes.Add("class", "desc")
                'Dim ob_desc As HtmlGenericControl = div_generic(contador_div)
                ob_gallery.Controls.Add(item_chek(i))
                'div_generic(contador_div).Controls.Add(item_chek(i))
                ob_responsive.Controls.Add(ob_gallery)
                m_image(i).Attributes.Add("width", "50px")
                m_image(i).Attributes.Add("class", "onclik_item")
                ob_gallery.Controls.Add(m_image(i))
                'ob_gallery.Attributes.Add("class", "onclik_item")
                'ob_responsive.Controls.Add(ob_desc)
                objCell.Controls.Add(ob_gallery)
                objCell.Attributes.Add("width", "50px")
                objRow.Cells.Add(objCell)
                contador_div = contador_div + 1
            Next
            Table.Rows.Add(objRow)
            Update.Update()
            Interface_dinamica_documentos_a_compartir = "YES"
        Catch ex As Exception
            Interface_dinamica_documentos_a_compartir = "Inconsistencia general funcion Interface_dinamica_documentos_a_compartir " & ex.Message
        End Try
    End Function
    Function Numero_documentos_compartidos_relacionados_a_un_radicado(ByVal radicado_relacionado As String, _
                                                                      ByRef numero_doc_compartidos As Integer) As String
        '------------------------------------------------------
        'Función : Retorna numero de documentos relacionados
        'a un radicado por tipo de colaboración
        'Fecha : 2017-06-03
        'Ing : Miguel Angel Urueta Miranda
        '-------------------------------------------------------
        Try
            Dim sql_consulta As String = "SELECT ID_RA_CD_DOCUMENTOS_COMPARTIDOS  from ra_cd_documentos_compartidos " & _
                          " where RADICADO_RELACIONADO='" & radicado_relacionado & "' and TIPO_REGISTRO_COMPARTIDO=3"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_usuarios_documentos_compartidos")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Numero_documentos_compartidos_relacionados_a_un_radicado = "Error listando Numero_documentos_compartidos_relacionados_a_un_radicado " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                numero_doc_compartidos = 0
                Numero_documentos_compartidos_relacionados_a_un_radicado = "YES"
                Exit Function
            Else
                numero_doc_compartidos = Datset.Tables(0).Rows.Count
                Numero_documentos_compartidos_relacionados_a_un_radicado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Numero_documentos_compartidos_relacionados_a_un_radicado = "Inconsistencia general función Retorna_id_usuario_documento_compartido " & ex.Message
        End Try
    End Function
    Function extare_icono_archivo(ByVal Ruta_Image As String, ByRef img As Image) As String
        Try
            Dim icono As Icon
            Dim inp As IntPtr = New IntPtr
            Dim refile As New FileInfo(Ruta_Image)
            If File.Exists(Ruta_Image) = True Then
                icono = System.Drawing.Icon.ExtractAssociatedIcon(Ruta_Image)
                ' Convertirlo a Bitmap y asignarlo a la propiedad Image del picture
                If icono IsNot Nothing Then
                    Dim ibtm As Bitmap = icono.ToBitmap()
                    ibtm.SetResolution(27024, 27024)
                    img = ibtm
                    extare_icono_archivo = "YES"
                    Exit Function
                Else
                    extare_icono_archivo = "YES"
                    Exit Function
                    img = Nothing
                End If
            Else
                extare_icono_archivo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            extare_icono_archivo = "Inconsistencia extracción de icono"
        End Try
    End Function
    Function GetLista_usuarios_gestion(ByRef DName As String) As String
        Dim response As String = ""
        Try
            Dim refcconect As New conect.Dbase_Conction_Mysql_RA
            Dim refclas As New ClassRadicador
            Dim datset As New DataSet
            Dim sqlconsult As String = "Select id_Remit_Dest_Int,Login_Usuario,Nombre_Remitente,Cargo_Remite from remit_dest_interno where  Estado_Usuario=1 "
            response = refcconect.SELECTION_SELECT_FIELD(sqlconsult, datset)
            If response <> "YES" Then
                GetLista_usuarios_gestion = response
                Exit Function
            End If
            If datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To datset.Tables(0).Rows.Count - 1
                    Dim tempo_record As String = "<" & datset.Tables(0).Rows(i).Item(0).ToString() & "> " & datset.Tables(0).Rows(i).Item(2).ToString() & " (" & datset.Tables(0).Rows(i).Item(3).ToString() & ")"
                    tempo_record = tempo_record.Replace(",", "")
                    DName = DName & tempo_record & ","
                Next
                GetLista_usuarios_gestion = "YES"
                Exit Function
            Else
                GetLista_usuarios_gestion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            GetLista_usuarios_gestion = "Inconsistencia general función GetLista_usuarios_gestion " & ex.Message
        End Try
    End Function
    Function Confirma_colaboracion_solicitud_colaboracion(ByVal id_documento_compatido_general As Integer, _
                                                          ByVal id_nota_aprobacion As Integer, _
                                                          ByVal ref_drowlist As DropDownList, _
                                                          ByVal nota_solicitud As String, _
                                                          ByRef resultado_correo As String) As String

        If id_nota_aprobacion = 0 And ref_drowlist.Items.Count = 0 Then
            Confirma_colaboracion_solicitud_colaboracion = "Debe registrar una nota o adjuntar un danexo de colaboración "
            Exit Function
        End If
        '--------------------------------------------------------
        'Retorna datos estructura general documento compartido
        '--------------------------------------------------------
        Dim Result As String = ""
        Dim refclas_cd_compartidos As New Class_ra_Cd_Documentos_Compartidos
        Dim stru_documento_general As STRU_DOCUMENTO_COMPARTIDO_GENERAL = Nothing
        Result = refclas_cd_compartidos.SolicitaEstructuraGeneraldocumentosCompartido(id_documento_compatido_general,
                                                                                      stru_documento_general)
        If Result <> "YES" Then
            Confirma_colaboracion_solicitud_colaboracion = Result
            Exit Function
        End If
        '---------------------------------------------------------
        'Retorna correo electrónico usuario de confirmación   
        '---------------------------------------------------------
        Dim refclas_respuesta As New Classgestionrespuesta
        Dim Ref_clas_remit_dest As New Class_remit_dest_interno
        Dim correo_electronico As String = ""
        Dim nombre_usuario As String = ""
        Dim cargo_usuario As String = ""
        Result = Ref_clas_remit_dest.Retorna_datos_caracterizacion_usuario_gestion(stru_documento_general.Remit_Dest_Interno_id_remit_dest_Int, _
                                                                                   nombre_usuario, _
                                                                                   cargo_usuario, _
                                                                                   correo_electronico)
        If Result <> "YES" Then
            Confirma_colaboracion_solicitud_colaboracion = Result
            Exit Function
        End If
        Dim asunto_radicado_respuesta As String = ""
        Dim radicado_peticionario As String = ""
        Dim remitente_relacionado As String = ""
        Dim stru_envio As stru_envio = Nothing
        If stru_documento_general.RADICADO_RELACIONADO <> "" Then
            Result = refclas_respuesta.Retorna_datos_estructura_envio_por_radicado(stru_documento_general.RADICADO_RELACIONADO, _
                                                                                   stru_envio, _
                                                                                   1)
            If Result <> "YES" Then
                Confirma_colaboracion_solicitud_colaboracion = Result
                Exit Function
            End If
            If stru_envio.RADICADO <> Nothing Then
                asunto_radicado_respuesta = stru_envio.ASUNTO
                radicado_peticionario = stru_envio.RADICADO
                remitente_relacionado = stru_envio.DESTINATARIO
            End If
        End If

        Dim id_usuario_compartido As Integer = 0
        Result = Me.Retorna_id_usuario_documento_compartido(id_documento_compatido_general, _
                                                            id_usuario_compartido, _
                                                            HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
        If Result <> "YES" Then
            Confirma_colaboracion_solicitud_colaboracion = Result
            Exit Function
        End If
        Dim id_documento_compartido As Integer = 0
        Result = Me.Retorna_id_documento_compartido(id_documento_compatido_general, _
                                                    id_documento_compartido, _
                                                    HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
        If Result <> "YES" Then
            Confirma_colaboracion_solicitud_colaboracion = Result
            Exit Function
        End If
        Dim update_confirmacion_usuario As String = "Update ra_cd_usuarios_documentos_compartidos set ESTADO_CONFIRMACION_COLABORACION=1 where ID_USUARIOS_DOCUMENTOS_COMPARTIDOS=" & _
            id_documento_compartido
        Dim numero_documento_pend_confirmar As Integer = 0
        Result = Me.Retorna_numero_documentos_compartidos_pendientes_colaborar(stru_documento_general.ID_RA_CD_DOCUMENTOS_COMPARTIDOS, _
                                                                               numero_documento_pend_confirmar)
        If Result <> "YES" Then
            Confirma_colaboracion_solicitud_colaboracion = Result
            Exit Function
        End If
        Dim date1al As String = Date.Now
        Dim refclas_gestion_fechas As New ClassGestionFechas
        Result = refclas_gestion_fechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            Confirma_colaboracion_solicitud_colaboracion = Result
            Exit Function
        End If
        Dim update_confirmacion_general As String = ""
        If numero_documento_pend_confirmar = 1 Then
            update_confirmacion_general = "Update ra_cd_documentos_compartidos set ESTADO_CONFIRMACION_COLABORACION=1 where ID_RA_CD_DOCUMENTOS_COMPARTIDOS=" & id_documento_compatido_general
        End If
        Dim documentos_colaboracion As String = ""
        If ref_drowlist.Items.Count > 0 Then
            For i As Integer = 0 To ref_drowlist.Items.Count - 1
                If i = 0 Then
                    documentos_colaboracion = ref_drowlist.Items(0).Text
                Else
                    documentos_colaboracion = documentos_colaboracion & "   " & ref_drowlist.Items(0).Text
                End If
            Next
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction = Nothing
        Dim sqlresultinsert As Integer = 0
        Try
            '-------------------------------------------------
            'Registra solicitud de aprobación
            '-------------------------------------------------
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = update_confirmacion_usuario
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Confirma_colaboracion_solicitud_colaboracion = "Imposible registrar confirmación de la colaboración  "
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If update_confirmacion_general <> "" Then
                myCommand.CommandText = update_confirmacion_general
                sqlresultinsert = myCommand.ExecuteNonQuery()
                If sqlresultinsert = 0 Then
                    Confirma_colaboracion_solicitud_colaboracion = "Imposible registrar confirmación general de la colaboración  "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            Dim hor As String = Now
            Dim campos_trans As String = "Null"
            Dim isert_datos As String = ""
            campos_trans = "COLABORACION A LA SOLICITUD " & "  (" & id_documento_compatido_general & _
          ")" & " POR MEDIO DE LA SOLICTUD  " & id_documento_compartido & vbCrLf & " DOCUMENTOS DE COLABORACION " & vbCrLf & documentos_colaboracion & vbCrLf
            isert_datos = "('" & "COLABORACION DOCUMENTO COMPARTIDO" & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" &
                HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                         id_documento_compatido_general & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','GESTOR WEB','" & campos_trans & "')"
            Dim sql_insert_log_solicitud = "INSERT INTO ra_log_documentos_compartidos (desc_op,USER_OPER,Remit_Dest_Interno_id_Remit_Dest_Int,DATE_TRANS,ID_RA_CD_DOCUMENTOS_COMPARTIDOS" & _
                                                 ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values " & _
                                                 isert_datos
            myCommand.CommandText = sql_insert_log_solicitud
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Confirma_colaboracion_solicitud_colaboracion = "Imposible registrar log de colaboración  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            Result = Me.Envio_correo_electronico_confirmacion_colaboracion(correo_electronico,
                                                                           nota_solicitud,
                                                                           id_documento_compatido_general,
                                                                           asunto_radicado_respuesta,
                                                                           radicado_peticionario,
                                                                           remitente_relacionado)
            If Result <> "YES" Then
                resultado_correo = "Se notifico la comaboración, pero no su pudo enviar al correo electrónico la notificación " & Result
            End If
            Confirma_colaboracion_solicitud_colaboracion = "YES"
            Exit Function
        Catch ex As Exception
            myTrans.Rollback()
            If Not myTrans.Connection Is Nothing Then
                Confirma_colaboracion_solicitud_colaboracion = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
            If myTrans.Connection.State = ConnectionState.Open Then
                myConnection.Close()
            End If
            Confirma_colaboracion_solicitud_colaboracion = "Error General " & ex.Message
            Exit Function
        End Try
    End Function
    Function Registra_solicitud_general_documento_compartido_usuario(ByVal stru_user() As stru_usuario_gestion_compartido, _
                                                                     ByVal Asunto As String, _
                                                                     ByVal nota As String, _
                                                                     ByVal nivel_urgencia_solicitud As String, _
                                                                     ByVal tipo_solicitud As String, _
                                                                     ByRef fecha_limite As String, _
                                                                     ByVal Radicado_relacionado As String, _
                                                                     ByVal id_usuario_propietario As Integer, _
                                                                     ByVal stru_documentos() As stru_documentos_compartidos, _
                                                                     ByRef resultado_nitcorreo As String) As String
        Dim Result As String = ""
        If nivel_urgencia_solicitud = "" Then
            Registra_solicitud_general_documento_compartido_usuario = "Por favor seleccione el nivel prioridad del documento a compartir"
            Exit Function
        End If
        If tipo_solicitud = "" Then
            Registra_solicitud_general_documento_compartido_usuario = "Por favor seleccione el modo de compartir los documentos "
            Exit Function
        End If

        If Asunto = "" Then
            Registra_solicitud_general_documento_compartido_usuario = "Por favor informe el asunto de los documentos compartidos"
            Exit Function
        End If
        If stru_user Is Nothing Then
            Registra_solicitud_general_documento_compartido_usuario = "Por favor seleccione los usuarios a compartir"
            Exit Function
        End If

        '---------------------------------------------
        'Retorna datos radicado relacionado
        '---------------------------------------------
        Dim refclas_respuesta As New Classgestionrespuesta
        Dim stru_envio As stru_envio = Nothing
        If Radicado_relacionado <> "" Then
            Result = refclas_respuesta.Retorna_datos_estructura_envio_por_radicado(Radicado_relacionado, _
                                                                                   stru_envio, _
                                                                                   1)
            If Result <> "YES" Then
                Registra_solicitud_general_documento_compartido_usuario = Result
                Exit Function
            Else
                If fecha_limite = "" Then
                    If stru_envio.FECHA_VENCE <> "" Then
                        Dim split_fecha() As String = stru_envio.FECHA_VENCE.Split("/")
                        fecha_limite = split_fecha(2) & "-" & split_fecha(1) & "-" & split_fecha(0)
                    End If
                End If

            End If
        End If
        If tipo_solicitud <> "Informativo" Then
            If fecha_limite = "" Then
                Registra_solicitud_general_documento_compartido_usuario = "Por favor seleccione la fecha límite  "
                Exit Function
            End If
        End If
        '-----------------------------------------------------------------
        'Verifica que la fecha limite no sea menor a la fecha de registro
        '-----------------------------------------------------------------
        'fecha_limite = Formatear_Fecha_Mysql(fecha_limite)
        Dim date1al As String = Date.Now
        Dim refclas_gestion_fechas As New ClassGestionFechas
        Result = refclas_gestion_fechas.Formatea_Fecha_Almacenamiento_Time(date1al)
        If Result <> "YES" Then
            Registra_solicitud_general_documento_compartido_usuario = Result
            Exit Function
        End If
        'If fecha_limite <> "" Then
        '    Dim fecha_ref_limite As Date
        '    Dim bln As Boolean = DateTime.TryParse(fecha_limite, fecha_ref_limite)
        '    If (Not (bln)) Then
        '        Registra_solicitud_general_documento_compartido_usuario = "La conversión no es correcta para la fecha límite"
        '        Exit Function
        '    End If
        '    Dim fecha_hoy As Date
        '    'date1al = Formatear_Fecha_Mysql(date1al)
        '    bln = DateTime.TryParse(date1al, fecha_hoy)
        '    If (Not (bln)) Then
        '        Registra_solicitud_general_documento_compartido_usuario = "La conversión no es correcta para fecha actual"
        '        Exit Function
        '    End If
        '    If (fecha_ref_limite < fecha_hoy) Then
        '        Registra_solicitud_general_documento_compartido_usuario = "La fecha límite de respuesta no puede ser inferior a la fecha actual, seleccione la fecha limite correcta"
        '        Exit Function
        '    End If

        'End If
        Dim ref_fecha_limite As String = ""
        If fecha_limite = "" Then
            ref_fecha_limite = "Null"
        Else
            ref_fecha_limite = "'" & fecha_limite & "'"
        End If
        '-----------------------------------------------
        'Registra solicitudes de aprobación
        '-----------------------------------------------

        Dim estado_solicitud As Integer = 0
        If nivel_urgencia_solicitud = "Normal" Then
            estado_solicitud = 1
        End If
        If nivel_urgencia_solicitud = "Urgente" Then
            estado_solicitud = 2
        End If

        Dim tipo_tramite As Integer = 0
        If tipo_solicitud = "Informativo" Then
            tipo_tramite = 1
        End If
        If tipo_solicitud = "Para aprobación" Then
            tipo_tramite = 2
        End If
        If tipo_solicitud = "Para colaboración" Then
            tipo_tramite = 3
        End If

        Dim nota_solicitud_aprobacion As String = ""
        If nota = "" Then
            nota_solicitud_aprobacion = "Null"
        Else
            nota_solicitud_aprobacion = "'" & nota & "'"
        End If
        Dim ref_Radicado_relacionado As String = ""
        If Radicado_relacionado = "" Then
            ref_Radicado_relacionado = "Null"
        Else
            ref_Radicado_relacionado = "'" & Radicado_relacionado & "'"
        End If
        Dim sql_insert_registro_radicado As String = "insert into ra_cd_documentos_compartidos (" & _
            "Remit_Dest_Interno_id_remit_dest_Int,FECHA_REGISTRO_SOLICITUD,ESTADO_PRIORIDAD,NOTA_SOLICITUD,FECHA_LIMITE_RESPUESTA,RADICADO_RELACIONADO,ASUNTO_DOCUMENTO,TIPO_REGISTRO_COMPARTIDO,DESCRIPCION_TIPO_COMPARTIDO) VALUES (" & id_usuario_propietario & _
            ",'" & date1al & "'," & estado_solicitud & "," & nota_solicitud_aprobacion & "," & ref_fecha_limite & "," & ref_Radicado_relacionado & ",'" & Asunto & "'," & tipo_tramite & ",'" & tipo_solicitud & "')"

        Dim usuarios_relacionados As String = ""
        For i As Integer = 0 To stru_user.Length - 1
            usuarios_relacionados = usuarios_relacionados & stru_user(i).id_usuario_gestion & "-" & stru_user(i).nombre_usuario
        Next
        Dim documentos_compartidos As String = "ID_IMAGEN|NOMBRE_GABINETE"
        For i As Integer = 0 To stru_documentos.Length - 1
            documentos_compartidos = documentos_compartidos & stru_documentos(i).id_imagen & "-" & stru_documentos(i).nombre_gabinete & vbCrLf
        Next
        For i As Integer = 0 To stru_user.Length - 1
            If stru_user(i).id_usuario_gestion = 0 Then
                Registra_solicitud_general_documento_compartido_usuario = "El usuario " & stru_user(i).nombre_usuario & " no tiene usuario de gestion relacionado"
                Exit Function
            End If
        Next
        Dim log_rea_respuesta As String = ""
        If Radicado_relacionado <> "" Then
            Dim hor As String = Now
            Dim campos_trans_log As String = "Null"
            Dim isert_datos_log As String = ""
            If stru_envio.ID_RESPUESTA_RADICADO <> 0 Then
                campos_trans_log = "COMPARTE EL DOCUMENTO PETICIONARIO (" & stru_envio.ID_RESPUESTA_RADICADO & _
              ")" & "AL USUARIO(S) DE GESTION ID : " & usuarios_relacionados
                isert_datos_log = isert_datos_log & "('" & "COMPARTE DOCUMENTOS" & "','" & HttpContext.Current.Session.Item("Login_Usuario_Workfow") & "','" &
                    id_usuario_propietario & "','" & date1al & "'," &
                             stru_envio.ID_RESPUESTA_RADICADO & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','WORKFLOW WEB','" & campos_trans_log & "')"
                log_rea_respuesta = "INSERT INTO ra_log_respuesta_radicado (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_RESPUESTA_RADICADO" & _
                                                     ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values " & _
                                                     isert_datos_log
            End If
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction = Nothing
        Dim sqlresultinsert As Integer = 0
        Try
            '-------------------------------------------------
            'Registra solicitud de aprobación
            '-------------------------------------------------
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = sql_insert_registro_radicado
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Registra_solicitud_general_documento_compartido_usuario = "Imposible registrar la solicitud de compartir documento  "
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Dim id_solicitud_aprobacion As Object = Nothing
            id_solicitud_aprobacion = myCommand.LastInsertedId
            '----------------------------------------------------------------
            'Insertar los usuarios relacionados a la solicitud de aprobación
            '----------------------------------------------------------------
            Dim values_usuarios_solicitud As String = ""
            Dim sql_insert_registro_usuarios_solicitud As String = "insert into ra_cd_usuarios_documentos_compartidos (Remit_Dest_Interno_id_remit_dest_Int," & _
                "ID_RA_CD_DOCUMENTOS_COMPARTIDOS,FECHA_REGISTRO_SOLICITUD,FECHA_LIMITE_RESPUESTA,TIPO_REGISTRO_COMPARTIDO,DESCRIPCION_TIPO_COMPARTIDO,RADICADO_RELACIONADO) values "
            For i As Integer = 0 To stru_user.Length - 1
                If stru_user(i).id_usuario_gestion <> 0 Then
                    If i = 0 Then
                        values_usuarios_solicitud = "(" & stru_user(i).id_usuario_gestion & "," & id_solicitud_aprobacion & ",'" & date1al & "'," & ref_fecha_limite & "," & tipo_tramite & ",'" & tipo_solicitud & "'," & ref_Radicado_relacionado & ")"
                    Else
                        values_usuarios_solicitud = values_usuarios_solicitud & ",(" & stru_user(i).id_usuario_gestion & "," & id_solicitud_aprobacion & ",'" & date1al & "'," & ref_fecha_limite & "," & tipo_tramite & ",'" & tipo_solicitud & "'," & ref_Radicado_relacionado & ")"
                    End If
                End If
            Next

            sql_insert_registro_usuarios_solicitud = sql_insert_registro_usuarios_solicitud & values_usuarios_solicitud
            myCommand.CommandText = sql_insert_registro_usuarios_solicitud
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Registra_solicitud_general_documento_compartido_usuario = "Imposible registar los usuarios relacionados a la solicitud de aprobación  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If

            '-----------------------------------------------------------------
            'Insertar los registros de gabinetes documentos compartidos
            '-----------------------------------------------------------------
            values_usuarios_solicitud = ""
            Dim sql_insert_documentos As String = " insert into ra_cd_doumentos_gabinete_compartidos (RA_CD_DOCUMENTOS_COMPARTIDOS_ID_RA_CD_DOCUMENTOS_COMPARTIDOS,ID_IMAGEN,NOMBRE_GABINETE) VALUES "
            For i As Integer = 0 To stru_documentos.Length - 1
                If i = 0 Then
                    values_usuarios_solicitud = "(" & id_solicitud_aprobacion & "," & stru_documentos(i).id_imagen & ",'" & stru_documentos(i).nombre_gabinete & "')"
                Else
                    values_usuarios_solicitud = values_usuarios_solicitud & ",(" & id_solicitud_aprobacion & "," & stru_documentos(i).id_imagen & ",'" & stru_documentos(i).nombre_gabinete & "')"
                End If
            Next
            sql_insert_documentos = sql_insert_documentos & values_usuarios_solicitud
            myCommand.CommandText = sql_insert_documentos
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Registra_solicitud_general_documento_compartido_usuario = "Imposible registar los documentos de la solicitud  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Dim hor As String = Now
            Dim campos_trans As String = "Null"
            Dim isert_datos As String = ""
            campos_trans = "COMPARTE DOCUMENTO " & tipo_solicitud & "  (" & id_solicitud_aprobacion & _
          ")" & "AL USUARIO(S) DE GESTION ID : " & usuarios_relacionados & vbCrLf & " DOCUMENTOS COMPARTIDOS " & vbCrLf & documentos_compartidos & vbCrLf & "RADICADO RELACIONADO : " & Radicado_relacionado
            isert_datos = isert_datos & "('" & "COMPARTE DOCUMENTO" & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & id_usuario_propietario & "','" & date1al & "'," & _
                         id_solicitud_aprobacion & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','GESTOR WEB','" & campos_trans & "')"
            Dim sql_insert_log_solicitud = "INSERT INTO ra_log_documentos_compartidos (desc_op,USER_OPER,Remit_Dest_Interno_id_Remit_Dest_Int,DATE_TRANS,ID_RA_CD_DOCUMENTOS_COMPARTIDOS" & _
                                                 ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values " & _
                                                 isert_datos
            '-----------------------------------------------
            'Registra log respuesta solicitud
            '-----------------------------------------------
            myCommand.CommandText = sql_insert_log_solicitud
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Registra_solicitud_general_documento_compartido_usuario = "Imposible registar log de solicitud de aprobación  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '-----------------------------------------------
            'Registra log para la respuesta 
            '-----------------------------------------------
            If log_rea_respuesta <> "" Then
                myCommand.CommandText = log_rea_respuesta
                sqlresultinsert = myCommand.ExecuteNonQuery()
                If sqlresultinsert = 0 Then
                    Registra_solicitud_general_documento_compartido_usuario = "Imposible registar log documento compartido  "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            myTrans.Commit()
            myConnection.Close()
            Dim correos_relacionados As String = ""
            Result = Me.Retorna_correos_usuarios_documento_compartido(id_solicitud_aprobacion, _
                                                                      correos_relacionados)
            If Result <> "YES" Then
                resultado_nitcorreo = "Se compartieron los documentos pero no se notifico a los correos, error relacionado " & Result
            Else
                If tipo_tramite = 3 Then
                    nota = "Documento compartido para colaboración con la siguiente nota " & nota
                    Asunto = Asunto & " (para colaboración)"
                End If
                Result = Me.Envio_correo_electronico_usuarios_solicitados(correos_relacionados, _
                                                                          nota, _
                                                                          fecha_limite, _
                                                                          id_solicitud_aprobacion, _
                                                                          Asunto, _
                                                                          tipo_solicitud, _
                                                                          id_solicitud_aprobacion)
                If Result <> "YES" Then
                    resultado_nitcorreo = "Se compartieron los documentos pero no se notifico a los correos, error relacionado " & Result
                End If
            End If
            HttpContext.Current.Session.Item("GA_STRU_DOCUMENTO_NUMERO_COMPARTIDO") = HttpContext.Current.Session.Item("GA_STRU_DOCUMENTO_NUMERO_COMPARTIDO") + 1
            Registra_solicitud_general_documento_compartido_usuario = "YES"
        Catch ex As Exception
            myTrans.Rollback()
            If Not myTrans.Connection Is Nothing Then
                Registra_solicitud_general_documento_compartido_usuario = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
            If myTrans.Connection.State = ConnectionState.Open Then
                myConnection.Close()
            End If
            Registra_solicitud_general_documento_compartido_usuario = "Error General " & ex.Message
            Exit Function
        End Try

    End Function
    Function Retorna_correos_usuarios_documento_compartido(ByVal id_documento_compartido As Integer, _
                                                           ByRef correos_relacionados As String) As String
        '----------------------------------------------------
        'Función : Retorna correos electrónicos de usuarios
        'de documentos compartidos
        'Fecha : 2017-03-17
        'Ing :Miguel Angel Urueta Miranda
        '----------------------------------------------------
        Try
            Dim sql_consulta As String = "SELECT rdi.Correo_Electronico " & _
                     " from ra_cd_usuarios_documentos_compartidos as rcu " & _
                     "inner join remit_dest_interno as rdi on (rdi.id_Remit_Dest_Int=rcu.Remit_Dest_Interno_id_remit_dest_Int) where ID_RA_CD_DOCUMENTOS_COMPARTIDOS=" & id_documento_compartido
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_usuarios_documentos_compartidos")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_correos_usuarios_documento_compartido = "Error listando correos electrónicos " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_correos_usuarios_documento_compartido = "Imposible encontrar los correos electrónicos de la solicitud " & id_documento_compartido
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    If Datset.Tables(0).Rows(i).IsNull(0) = True Then
                    Else
                        If correos_relacionados = "" Then
                            correos_relacionados = Datset.Tables(0).Rows(i).Item(0)
                        Else
                            correos_relacionados = correos_relacionados & "," & Datset.Tables(0).Rows(i).Item(0)
                        End If
                    End If
                Next
                Retorna_correos_usuarios_documento_compartido = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_correos_usuarios_documento_compartido = "Inconsistencia general función Retorna_correos_usuarios_documento_compartido " & ex.Message
        End Try
    End Function
    Function Retorna_usuarios_relacionados_a_documento_compartido(ByVal id_documento_compartido As Integer, ByRef usuarios_relacionados As String) As String
        '----------------------------------------------------
        'Función : Retorna usuarios relacionados
        'a documentos compartidos
        'Fecha : 2017-03-17
        'Ing :Miguel Angel Urueta Miranda
        '----------------------------------------------------
        Try
            Dim sql_consulta As String = "SELECT rdi.id_Remit_Dest_Int,rdi.Login_Usuario,rdi.Nombre_Remitente,rdi.Cargo_Remite " & _
                    " from ra_cd_usuarios_documentos_compartidos as rcu " & _
                    "inner join remit_dest_interno as rdi on (rdi.id_Remit_Dest_Int=rcu.Remit_Dest_Interno_id_remit_dest_Int) where ID_RA_CD_DOCUMENTOS_COMPARTIDOS=" & id_documento_compartido

            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_usuarios_documentos_compartidos")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_usuarios_relacionados_a_documento_compartido = "Error listando usuarios " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_usuarios_relacionados_a_documento_compartido = "Imposible encontrar usuarios relacionados " & id_documento_compartido
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    Dim user_relacionados As String = "<" & Datset.Tables(0).Rows(i).Item(0).ToString() & "> " & Datset.Tables(0).Rows(i).Item(2).ToString() & " (" & Datset.Tables(0).Rows(i).Item(3).ToString() & ")"
                    If i = 0 Then
                        usuarios_relacionados = user_relacionados
                    Else
                        usuarios_relacionados = usuarios_relacionados & "," & user_relacionados
                    End If
                Next
                Retorna_usuarios_relacionados_a_documento_compartido = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_usuarios_relacionados_a_documento_compartido = "Inconsistencia general función Retorna_usuarios_relacionados_a_documento_compartido " & ex.Message
        End Try
    End Function
    Function Envio_correo_electronico_usuarios_solicitados(ByVal correos_relacionados As String, ByVal nota_solicitud As String, _
                                                           ByVal fecha_solicitud As String, _
                                                           ByVal numero_solictud As Integer, _
                                                           ByVal Asunto As String, _
                                                           ByVal tipo_documento As String, _
                                                           ByVal numero_documento_compartido As Integer) As String
        '----------------------------------------------------------
        'Función : Envía correo electrónico usuario de solicitud
        'Fecha : 2016-02-20
        'Ing Miguel Angel Urueta Miranda 
        '----------------------------------------------------------
        Try
            Dim refclas_gestion As New Classgestionrespuesta
            Dim Result As String = ""
            Dim nombre_area As String = ""
            Dim id_area As Integer = 0
            Dim nombre_usuario As String = ""
            Dim cargo_usuario As String = ""
            Result = refclas_gestion.Retorna_datos_asignacion_respuesta_usuario_gestion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                        id_area, _
                                                                                        nombre_area, _
                                                                                        nombre_usuario, _
                                                                                        cargo_usuario)
            If Result <> "YES" Then
                Envio_correo_electronico_usuarios_solicitados = Result
                Exit Function
            End If
            Dim spli_texto_correo() As String = {"Nuevo documento compartido numero : " & numero_solictud, _
            "Fecha límite de respuesta : " & fecha_solicitud, "Asunto :" & Asunto, "Nota Solicitud : " & nota_solicitud, _
            "Nombre de quien comparte : " & nombre_usuario, "Cargo de quien comparte : " & cargo_usuario, "Area de quien comparte : " & nombre_area, "Por favor ingrese al sistema de gestión documental a la opción (Notificaciones y solicitudes - Documentos compatidos - Documentos compartidos pendientes por mi revisión), ubique el numero " & numero_documento_compartido & " en la columna NUMERO_DOCUMENTO"}
            Dim Refclas As New ClassCorreo
            Result = Refclas.Envio_Correo_documento_compartido(spli_texto_correo, _
                                                               correos_relacionados, _
                                                               "Nuevo documento compartido por " & nombre_usuario & " Numero : " & numero_solictud)
            If Result <> "YES" Then
                Envio_correo_electronico_usuarios_solicitados = Result
                Exit Function
            Else
                Envio_correo_electronico_usuarios_solicitados = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Envio_correo_electronico_usuarios_solicitados = "Inconsistencia general función Envio_correo_electronico_usuarios_solicitados " & ex.Message
        End Try
    End Function
    Function Envio_correo_electronico_confirmacion_colaboracion(ByVal correos_relacionados As String, ByVal nota_solicitud As String, _
                                                            ByVal numero_solictud As Integer, ByVal Asunto As String, ByVal radicado_relacionado As String, _
                                                             ByVal remitente_relacionado As String) As String
        '----------------------------------------------------------
        'Función : Envía correo electrónico usuario de solicitud
        'Fecha : 2016-02-20
        'Ing Miguel Angel Urueta Miranda 
        '----------------------------------------------------------
        Try
            Dim refclas_gestion As New Classgestionrespuesta
            Dim Result As String = ""
            Dim nombre_area As String = ""
            Dim id_area As Integer = 0
            Dim nombre_usuario As String = ""
            Dim cargo_usuario As String = ""
            Result = refclas_gestion.Retorna_datos_asignacion_respuesta_usuario_gestion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), id_area, nombre_area, nombre_usuario, cargo_usuario)
            If Result <> "YES" Then
                Envio_correo_electronico_confirmacion_colaboracion = Result
                Exit Function
            End If
            Dim spli_texto_correo() As String = {"Respuesta a solicitud de colaboración numero : " & numero_solictud, _
            "El usuario " & nombre_usuario & " adjunto un documento o una nota respondiendo a su solicitud de colaboración número " & numero_solictud & _
             ", relacionada al radicado " & radicado_relacionado & " para su conocimiento y afines. ", "La nota de colaboración expone lo siguiente: ", nota_solicitud, _
             "Los datos continuación relacionan la descripción del usuario que responde a su solicitud de colaboración ", _
            "Nombre de quien colabora : " & nombre_usuario, "Cargo de quien colabora : " & cargo_usuario, "Area de quien colabora : " & nombre_area, _
            "Radicado relacionado : " & radicado_relacionado, "Asunto relacionado : " & Asunto, "Peticionario relacionado : " & remitente_relacionado, "Nota : Para revisar los aportes de colaboración entre a la opción (Trámites y documentos -> Documentos compartidos), ubique en la columna (NUMERO_DOCUMENTO) el consecutivo : " & numero_solictud}
            Dim Refclas As New ClassCorreo
            Result = Refclas.Envio_Correo_documento_compartido(spli_texto_correo, correos_relacionados, "Respuesta a solicitud de colaboración numero : " & numero_solictud & " por el usuario " & nombre_usuario)
            If Result <> "YES" Then
                Envio_correo_electronico_confirmacion_colaboracion = Result
                Exit Function
            Else
                Envio_correo_electronico_confirmacion_colaboracion = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Envio_correo_electronico_confirmacion_colaboracion = "Inconsistencia general función Envio_correo_electronico_confirmacion_colaboracion " & ex.Message
        End Try
    End Function
    Function Retorna_matriz_id_usuarios_gestion(ByVal datos_user As String, _
                                                ByRef stru_user() As stru_usuario_gestion_compartido) As String
        '-------------------------------------------------------------
        'Función : Reorna matriz usuarios de gestión con el protocolo
        'de seleccion de la interface 
        'Fecha : 2017-03-16
        'Ing Miguel Angel Urueta Miranda
        '-------------------------------------------------------------
        Try
            Dim selector_coma As String = Right(datos_user, 1)
            Dim split_coma() As String = datos_user.Split(",")
            If split_coma Is Nothing Then
                Retorna_matriz_id_usuarios_gestion = "La matriz de usuario tiene datos erroneos "
                Exit Function
            End If
            Dim matriz_tama As Integer = 0
            If selector_coma = "," Or selector_coma = " " Then
                matriz_tama = split_coma.Length - 1
            Else
                matriz_tama = split_coma.Length
            End If

            For i As Integer = 0 To matriz_tama - 1
                If InStr(split_coma(i), ">") = 0 Then
                    Retorna_matriz_id_usuarios_gestion = "La lista de usuario tiene una inconsistencia falta el separador (>) de un usuario seleccionado" & split_coma(i)
                    Exit Function
                Else
                    Dim split_mayor_q() As String = split_coma(i).Split(">")
                    Dim id As String = split_mayor_q(0).Replace("<", "")
                    ReDim Preserve stru_user(i)
                    stru_user(i).id_usuario_gestion = Val(id)
                    stru_user(i).nombre_usuario = split_coma(i)
                End If

            Next
            Retorna_matriz_id_usuarios_gestion = "YES"
        Catch ex As Exception
            Retorna_matriz_id_usuarios_gestion = "Inconsistencia general Función Retorna_matriz_id_usuarios_gestion " & ex.Message
        End Try
    End Function
    Function Lista_documentos_compartidos_general_por_tipo(ByVal id_usuario As Integer, _
                                                           ByRef grediview As GridView, _
                                                           ByRef HiddenEmailconsulta As Object, _
                                                           ByRef reflabel As Label, _
                                                           ByRef hideselecion As Object, _
                                                           ByRef update As UpdatePanel, _
                                                           ByVal estado_solicitud As String, _
                                                           ByRef update_title As UpdatePanel, _
                                                           ByVal tipo_consulta As Integer, _
                                                           ByVal valor_consulta As String, _
                                                           ByRef colum_order_name As String, _
                                                           ByRef order_colum As String) As String
        Try
            Dim estado_ As Integer = -1
            If estado_solicitud = "Informativo" Then
                estado_ = 1
            End If
            If estado_solicitud = "Para aprobación" Then
                estado_ = 2
            End If
            If estado_solicitud = "Para colaboración" Then
                estado_ = 3
            End If
            If estado_solicitud = "Eliminados" Then
                estado_ = 4
            End If
            Dim filtro As String = ""
            If estado_ <> -1 Then
                filtro = " and rcs.TIPO_REGISTRO_COMPARTIDO=" & estado_
            End If
            Dim estado_elminado As Integer = 1
            If estado_ = 4 Then
                estado_elminado = 0
                filtro = ""
            End If
            Dim sql_consulta As String = ""
            If tipo_consulta = 1 Then
                sql_consulta = "SELECT ID_RA_CD_DOCUMENTOS_COMPARTIDOS AS NUMERO,ESTADO_PRIORIDAD," & _
                   "rdi.Nombre_Remitente as COMPARTE,rdi.Cargo_Remite " _
                  & "as CARGO_COMPARTE,ASUNTO_DOCUMENTO as ASUNTO,rcs.DESCRIPCION_TIPO_COMPARTIDO AS TIPO,rcs.DESCRIPCION_ESTADO_APROBACION as ESTADO,rcs.ESTADO_ELIMINADO,rcs.RADICADO_RELACIONADO AS RADICADO,rcs.FECHA_REGISTRO_SOLICITUD " & _
                  " as FECHA_REGISTRO,rcs.FECHA_LIMITE_RESPUESTA as FECHA_LIMITE from ra_cd_documentos_compartidos AS rcs " & _
                   " INNER JOIN remit_dest_interno AS rdi on (rdi.id_Remit_Dest_Int=rcs.Remit_Dest_Interno_id_remit_dest_Int)  where rcs.Remit_Dest_Interno_id_remit_dest_Int=" & id_usuario & _
                   filtro & " and rcs.ESTADO_ELIMINADO=" & estado_elminado & " order by " & colum_order_name & " " & order_colum
            Else
                sql_consulta = "SELECT ID_RA_CD_DOCUMENTOS_COMPARTIDOS AS NUMERO,ESTADO_PRIORIDAD," & _
                   "rdi.Nombre_Remitente as COMPARTE,rdi.Cargo_Remite  " _
                  & "as CARGO_COMPARTE,ASUNTO_DOCUMENTO as ASUNTO,rcs.DESCRIPCION_TIPO_COMPARTIDO AS TIPO,rcs.DESCRIPCION_ESTADO_APROBACION as ESTADO,rcs.ESTADO_ELIMINADO,rcs.RADICADO_RELACIONADO AS RADICADO,rcs.FECHA_REGISTRO_SOLICITUD " & _
                  "as FECHA_REGISTRO,rcs.FECHA_LIMITE_RESPUESTA as FECHA_LIMITE from ra_cd_documentos_compartidos AS rcs " & _
                  " INNER JOIN remit_dest_interno AS rdi on (rdi.id_Remit_Dest_Int=rcs.Remit_Dest_Interno_id_remit_dest_Int)  where " & _
                    " (" & _
                  "  ID_RA_CD_DOCUMENTOS_COMPARTIDOS like '%" & valor_consulta & "%'" & _
                  " or rdi.Nombre_Remitente like '%" & valor_consulta & "%'" & _
                  " or rdi.Cargo_Remite like '%" & valor_consulta & "%'" & _
                  " or ASUNTO_DOCUMENTO like '%" & valor_consulta & "%'" & _
                  " or rcs.DESCRIPCION_TIPO_COMPARTIDO like '%" & valor_consulta & "%'" & _
                  " or rcs.FECHA_REGISTRO_SOLICITUD like '%" & valor_consulta & "%'" & _
                  " or rcs.FECHA_LIMITE_RESPUESTA like '%" & valor_consulta & "%'" & _
                  " or rcs.RADICADO_RELACIONADO like '%" & valor_consulta & "%' )" & _
                  " and rcs.Remit_Dest_Interno_id_remit_dest_Int=" & id_usuario & _
                  " order by " & colum_order_name & " " & order_colum
            End If
            HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_SOLICITUD_COLABORACION") = tipo_consulta
            HttpContext.Current.Session.Item("GA_DATO_CONSULTA_SOLICITUD_COLABORACION") = valor_consulta
            HttpContext.Current.Session.Item("Sort_matri_colum_colaboracion") = {"OPCIONES", "NUMERO", "ESTADO_PRIORIDAD", _
                                                                               "COMPARTE", "CARGO_COMPARTE", _
                                                                               "ASUNTO", "TIPO", "ESTADO", "ESTADO_ELIMINADO", _
                                                                                 "RADICADO", _
                                                                               "FECHA_REGISTRO", "FECHA_LIMITE"}
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_usuarios_documentos_compartidos")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_documentos_compartidos_general_por_tipo = "Error listando documentos compartidos a otros usuarios " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                HiddenEmailconsulta.value = ""
                'reflabel.Text = "Se encontraron 0 registro(s) de documentos compartidos a otros usuarios"
                If tipo_consulta = 1 Then
                    reflabel.Text = "Se encontraron 0 registro(s) de documentos compartidos   "
                Else
                    reflabel.Text = "Se encontraron 0 registro(s) de documentos compartidos   "
                End If
                grediview.DataSource = Nothing
                'grediview.DataKeyNames = DataKey
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                update_title.Update()
                Lista_documentos_compartidos_general_por_tipo = "YES"
                Exit Function
            Else
                If tipo_consulta = 1 Then
                    reflabel.Text = "Se encontraron " & Datset.Tables(0).Rows.Count & " registro(s) de documentos compartidos   "
                Else
                    reflabel.Text = "Se encontraron " & Datset.Tables(0).Rows.Count & "  registro(s) de documentos compartidos  "
                End If
                grediview.DataSource = Datset
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                update_title.Update()
                For i As Integer = 0 To grediview.Rows.Count - 1
                    grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                    grediview.Rows(i).Attributes.Add("id_estado", grediview.Rows(i).Cells(2).Text.ToString())
                    Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                    Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-folder-open fa-lg")
                    Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-success btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Documentos compartidos relacionados")
                    ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "ver_doc_comp")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)

                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-list-alt fa-lg ")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-info btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Registros de colaboración o aprobación")
                    ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "ver_reg_colab")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)

                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-arrow-down fa-lg ")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn  btn-primary  btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Descarga certificado de aprobación")
                    ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "ver_cert_colab")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)

                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-times fa-lg")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-danger btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Elimina registro de documentos compartidos")
                    ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "elimina_doc_comp")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)

                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-archive fa-lg ")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-warning btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Archiva solicitud de aprobación de documentos")
                    ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "archiva_doc_colab")
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
                Dim Refclas As New ClassGredview
                Result = Refclas.add_clase_acender_decender(colum_order_name, _
                                                            HttpContext.Current.Session.Item("Sort_matri_colum_colaboracion"), _
                                                            order_colum, _
                                                            grediview)
                If Result <> "YES" Then
                    Lista_documentos_compartidos_general_por_tipo = "Error add clase funcion  Lista_documentos_compartidos_general_por_tipo " & Result
                    Exit Function
                End If
                Lista_documentos_compartidos_general_por_tipo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_documentos_compartidos_general_por_tipo = "Inconsistencia general función Lista_documentos_compartidos_general_por_tipo " & ex.Message
        End Try
    End Function
    Function Lista_registros_de_colaboracion_id_documento_compartido(ByVal id_documento_compartido As Integer, _
                                                                     ByRef grediview As GridView, _
                                                                     ByRef reflabel As Label, _
                                                                     ByRef hideselecion As Object, _
                                                                     ByRef update As UpdatePanel, _
                                                                     ByRef update_title As UpdatePanel, _
                                                                     ByVal tipo_consulta As Integer, _
                                                                     ByVal valor_consulta As String) As String
        Try
            Dim sql_consulta As String = ""
            If tipo_consulta = 1 Then
                sql_consulta = "SELECT ID_USUARIOS_DOCUMENTOS_COMPARTIDOS,ID_RA_CD_DOCUMENTOS_COMPARTIDOS" & _
                        ",rdi.Nombre_Remitente as NOMBRE_DE_QUIEN_COLABORA,rdi.Cargo_Remite as CARGO_DE_QUIEN_COLABORA," _
                        & "rcs.FECHA_REGISTRO_SOLICITUD as FECHA_REGISTRO from ra_cd_usuarios_documentos_compartidos AS rcs " & _
                        " INNER JOIN remit_dest_interno AS rdi on (rdi.id_Remit_Dest_Int=rcs.Remit_Dest_Interno_id_remit_dest_Int) " & _
                        " where rcs.ESTADO_ELIMINADO=1 and rcs.TIPO_REGISTRO_COMPARTIDO=3 and ID_RA_CD_DOCUMENTOS_COMPARTIDOS='" & _
                        id_documento_compartido & "' order by  ID_USUARIOS_DOCUMENTOS_COMPARTIDOS desc "
            Else
                sql_consulta = "SELECT ID_USUARIOS_DOCUMENTOS_COMPARTIDOS,ID_RA_CD_DOCUMENTOS_COMPARTIDOS" & _
                       ",rdi.Nombre_Remitente as NOMBRE_DE_QUIEN_COLABORA,rdi.Cargo_Remite as CARGO_DE_QUIEN_COLABORA," _
                       & "rcs.FECHA_REGISTRO_SOLICITUD as FECHA_REGISTRO from ra_cd_usuarios_documentos_compartidos AS rcs " & _
                       " INNER JOIN remit_dest_interno AS rdi on (rdi.id_Remit_Dest_Int=rcs.Remit_Dest_Interno_id_remit_dest_Int) " & _
                       " where ( " & _
                       " rdi.Nombre_Remitente like '%" & valor_consulta & "%'" & _
                       " or rdi.Cargo_Remite like '%" & valor_consulta & "%')" & _
                       " and rcs.ESTADO_ELIMINADO=1 and rcs.TIPO_REGISTRO_COMPARTIDO=3 and ID_RA_CD_DOCUMENTOS_COMPARTIDOS='" & _
                       id_documento_compartido & "' order by  ID_USUARIOS_DOCUMENTOS_COMPARTIDOS desc "
            End If
            HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_DOC_COMPARTIDO") = tipo_consulta
            HttpContext.Current.Session.Item("GA_DATO_CONSULTA_DOC_COMPARTIDO") = valor_consulta
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_usuarios_documentos_compartidos")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_registros_de_colaboracion_id_documento_compartido = "Error listando solicitudes relacionadas a un usuario " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                reflabel.Text = "Se encontraron 0 registro(s) de colaboración"
                grediview.DataSource = Nothing
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                update_title.Update()
                Lista_registros_de_colaboracion_id_documento_compartido = "YES"
                Exit Function
            Else
                reflabel.Text = "Se encontraron " & Datset.Tables(0).Rows.Count & " registro(s) de colaboración"
                grediview.DataSource = Datset
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                update_title.Update()
                For i As Integer = 0 To grediview.Rows.Count - 1
                    grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                    grediview.Rows(i).Attributes.Add("id_doc_compatido", grediview.Rows(i).Cells(2).Text.ToString())

                    Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                    Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-folder-open fa-lg")
                    ihtml.Style.Add("color", "white")
                    Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-primary btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Ver documentos")
                    ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("id_doc", grediview.Rows(i).Cells(2).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "ver_doc_col")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)

                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-sticky-note fa-lg")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn   btn-info btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "ver nota")
                    ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("id_doc", grediview.Rows(i).Cells(2).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "ver_not_comp")
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
                Lista_registros_de_colaboracion_id_documento_compartido = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_registros_de_colaboracion_id_documento_compartido = "Inconsistencia general función Lista_registros_de_colaboracion_radicado " & ex.Message
        End Try
    End Function
    Function Lista_registros_de_colaboracion_radicado(ByVal radicado As String, _
                                                      ByRef grediview As GridView, _
                                                      ByRef reflabel As Label, _
                                                      ByRef hideselecion As Object, _
                                                      ByRef update As UpdatePanel, _
                                                      ByRef update_title As UpdatePanel, _
                                                      ByVal tipo_consulta As Integer, _
                                                      ByVal valor_consulta As String) As String

        Try
            Dim sql_consulta As String = ""
            If tipo_consulta = 1 Then
                sql_consulta = "SELECT ID_USUARIOS_DOCUMENTOS_COMPARTIDOS,ID_RA_CD_DOCUMENTOS_COMPARTIDOS" & _
                        ",rdi.Nombre_Remitente as NOMBRE_DE_QUIEN_COLABORA,rdi.Cargo_Remite as CARGO_DE_QUIEN_COLABORA," _
                        & "rcs.FECHA_REGISTRO_SOLICITUD as FECHA_REGISTRO from ra_cd_usuarios_documentos_compartidos AS rcs " & _
                        " INNER JOIN remit_dest_interno AS rdi on (rdi.id_Remit_Dest_Int=rcs.Remit_Dest_Interno_id_remit_dest_Int) " & _
                        " where rcs.ESTADO_ELIMINADO=1 and rcs.TIPO_REGISTRO_COMPARTIDO=3 and RADICADO_RELACIONADO='" & radicado & _
                        "' order by  ID_USUARIOS_DOCUMENTOS_COMPARTIDOS desc "
            Else
                sql_consulta = "SELECT ID_USUARIOS_DOCUMENTOS_COMPARTIDOS,ID_RA_CD_DOCUMENTOS_COMPARTIDOS" & _
                       ",rdi.Nombre_Remitente as NOMBRE_DE_QUIEN_COLABORA,rdi.Cargo_Remite as CARGO_DE_QUIEN_COLABORA," _
                       & "rcs.FECHA_REGISTRO_SOLICITUD as FECHA_REGISTRO from ra_cd_usuarios_documentos_compartidos AS rcs " & _
                       " INNER JOIN remit_dest_interno AS rdi on (rdi.id_Remit_Dest_Int=rcs.Remit_Dest_Interno_id_remit_dest_Int) " & _
                       " where (" & _
                       " rdi.Nombre_Remitente like '%" & valor_consulta & "%'" & _
                       " or rdi.Cargo_Remite like '%" & valor_consulta & "%')" & _
                       " and rcs.ESTADO_ELIMINADO=1 and rcs.TIPO_REGISTRO_COMPARTIDO=3 and RADICADO_RELACIONADO='" & radicado & _
                       "' order by  ID_USUARIOS_DOCUMENTOS_COMPARTIDOS desc "
            End If
            HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_DOC_COMPARTIDO") = tipo_consulta
            HttpContext.Current.Session.Item("GA_DATO_CONSULTA_DOC_COMPARTIDO") = valor_consulta
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_usuarios_documentos_compartidos")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_registros_de_colaboracion_radicado = "Error listando solicitudes relacionadas a un usuario " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                'HiddenEmailconsulta.value = ""
                reflabel.Text = "Se encontraron 0 registro(s) de colaboración"
                grediview.DataSource = Nothing
                'grediview.DataKeyNames = DataKey
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                update_title.Update()
                Lista_registros_de_colaboracion_radicado = "YES"
                Exit Function
            Else
                reflabel.Text = "Se encontraron " & Datset.Tables(0).Rows.Count & " registro(s) de colaboración"
                grediview.DataSource = Datset
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                update_title.Update()
                For i As Integer = 0 To grediview.Rows.Count - 1
                    grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                    grediview.Rows(i).Attributes.Add("id_doc_compatido", grediview.Rows(i).Cells(2).Text.ToString())

                    Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                    Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-folder-open fa-lg")
                    ihtml.Style.Add("color", "white")
                    Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-primary btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Ver documentos")
                    ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("id_doc", grediview.Rows(i).Cells(2).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "ver_doc_col")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)

                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-sticky-note fa-lg")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn   btn-info btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "ver nota")
                    ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("id_doc", grediview.Rows(i).Cells(2).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "ver_not_comp")
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
                Lista_registros_de_colaboracion_radicado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_registros_de_colaboracion_radicado = "Inconsistencia general función Lista_registros_de_colaboracion_radicado " & ex.Message
        End Try
    End Function
    Function Lista_solictudes_compartidas_de_un_usuario_por_tipos(ByVal id_usuario As Integer, _
                                                                  ByRef grediview As GridView, _
                                                                  ByRef HiddenEmailconsulta As Object, _
                                                                  ByRef reflabel As Label, _
                                                                  ByRef hideselecion As Object, _
                                                                  ByRef update As UpdatePanel, _
                                                                  ByVal estado_solicitud As String, _
                                                                  ByRef update_title As UpdatePanel, _
                                                                  ByVal tipo_consulta As Integer, _
                                                                  ByVal valor_consulta As String, _
                                                                  ByRef colum_order_name As String, _
                                                                  ByRef order_colum As String) As String
        Try

            Dim estado_ As Integer = -1
            If estado_solicitud = "Informativo" Then
                estado_ = 1
            End If
            If estado_solicitud = "Para aprobación" Then
                estado_ = 2
            End If
            If estado_solicitud = "Para colaboración" Then
                estado_ = 3
            End If
            If estado_solicitud = "Eliminados" Then
                estado_ = 4
            End If
            Dim filtro As String = ""
            If estado_ <> -1 And estado_ <> 4 Then
                filtro = " and rcs.TIPO_REGISTRO_COMPARTIDO=" & estado_
            End If
            Dim estado_elminado As Integer = 1
            If estado_ = 4 Then
                estado_elminado = 0
            End If
            Dim sql_consulta As String = ""
            If tipo_consulta = 1 Then
                sql_consulta = "SELECT ID_USUARIOS_DOCUMENTOS_COMPARTIDOS,rcsd.ESTADO_PRIORIDAD,rcs.ESTADO_VISTO_SOLICITANTE,rcs.ESTADO_ELIMINADO," & _
                    "rcsd.ID_RA_CD_DOCUMENTOS_COMPARTIDOS AS NUMERO,rdi.Nombre_Remitente as COMPARTE,rdi.Cargo_Remite as CARGO," _
                    & "rcsd.ASUNTO_DOCUMENTO AS ASUNTO,rcs.DESCRIPCION_TIPO_COMPARTIDO AS TIPO,rcs.DESCRIPCION_ESTADO_RESPUESTA as ESTADO,rcs.FECHA_LIMITE_RESPUESTA as FECHA_LIMITE,rcsd.RADICADO_RELACIONADO as RADICADO, " & _
                    " rcsd.FECHA_REGISTRO_SOLICITUD as FECHA from ra_cd_usuarios_documentos_compartidos AS rcs " & _
                    " INNER JOIN ra_cd_documentos_compartidos AS rcsd on (rcsd.ID_RA_CD_DOCUMENTOS_COMPARTIDOS=rcs.ID_RA_CD_DOCUMENTOS_COMPARTIDOS)" & _
                    " INNER JOIN remit_dest_interno AS rdi on (rdi.id_Remit_Dest_Int=rcsd.Remit_Dest_Interno_id_remit_dest_Int) " & _
                    " where rcs.Remit_Dest_Interno_id_remit_dest_Int=" & id_usuario & _
                    filtro & " and rcs.ESTADO_ELIMINADO=" & estado_elminado & " order by " & colum_order_name & " " & order_colum
            Else
                sql_consulta = "SELECT ID_USUARIOS_DOCUMENTOS_COMPARTIDOS,rcsd.ESTADO_PRIORIDAD,rcs.ESTADO_VISTO_SOLICITANTE,rcs.ESTADO_ELIMINADO," & _
                    "rcsd.ID_RA_CD_DOCUMENTOS_COMPARTIDOS AS NUMERO,rdi.Nombre_Remitente as COMPARTE,rdi.Cargo_Remite as CARGO," _
                    & "rcsd.ASUNTO_DOCUMENTO AS ASUNTO,rcs.DESCRIPCION_TIPO_COMPARTIDO AS TIPO,rcs.DESCRIPCION_ESTADO_RESPUESTA as ESTADO,rcs.FECHA_LIMITE_RESPUESTA as FECHA_LIMITE,rcsd.RADICADO_RELACIONADO as RADICADO, " & _
                    " rcsd.FECHA_REGISTRO_SOLICITUD as FECHA from ra_cd_usuarios_documentos_compartidos AS rcs " & _
                    " INNER JOIN ra_cd_documentos_compartidos AS rcsd on (rcsd.ID_RA_CD_DOCUMENTOS_COMPARTIDOS=rcs.ID_RA_CD_DOCUMENTOS_COMPARTIDOS)" & _
                    " INNER JOIN remit_dest_interno AS rdi on (rdi.id_Remit_Dest_Int=rcsd.Remit_Dest_Interno_id_remit_dest_Int) " & _
                    " where (" & _
                    "  rcsd.ID_RA_CD_DOCUMENTOS_COMPARTIDOS like '%" & valor_consulta & "%'" & _
                    " or rdi.Nombre_Remitente like '%" & valor_consulta & "%'" & _
                    " or rdi.Cargo_Remite like '%" & valor_consulta & "%'" & _
                    " or rcsd.ASUNTO_DOCUMENTO like '%" & valor_consulta & "%'" & _
                    " or rcs.DESCRIPCION_TIPO_COMPARTIDO like '%" & valor_consulta & "%'" & _
                    " or rcsd.FECHA_REGISTRO_SOLICITUD like '%" & valor_consulta & "%'" & _
                    " or rcs.FECHA_LIMITE_RESPUESTA like '%" & valor_consulta & "%'" & _
                    " or rcsd.RADICADO_RELACIONADO like '%" & valor_consulta & "%' )" & _
                    " and rcs.Remit_Dest_Interno_id_remit_dest_Int=" & id_usuario & _
                      " order by  " & colum_order_name & " " & order_colum
            End If
            HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_DOC_COMPARTIDO") = tipo_consulta
            HttpContext.Current.Session.Item("GA_DATO_CONSULTA_DOC_COMPARTIDO") = valor_consulta
            HttpContext.Current.Session.Item("Sort_matri_colum_compartido") = {"OPCIONES", "ID_USUARIOS_DOCUMENTOS_COMPARTIDOS", _
                                                                               "ESTADO_PRIORIDAD", "ESTADO_VISTO_SOLICITANTE", _
                                                                               "ESTADO_ELIMINADO", "NUMERO", "COMPARTE", "CARGO", _
                                                                               "ASUNTO", "TIPO", "ESTADO", "FECHA_LIMITE", "RADICADO", "FECHA"}

            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_usuarios_documentos_compartidos")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_solictudes_compartidas_de_un_usuario_por_tipos = "Error listando solicitudes relacionadas a un usuario " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                HiddenEmailconsulta.value = ""
                If tipo_consulta = 1 Then
                    reflabel.Text = "Se encontraron 0 registro(s) de documentos compartidos   "
                Else
                    reflabel.Text = "Se encontraron 0 registro(s) de documentos compartidos   "
                End If
                grediview.DataSource = Nothing
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                update_title.Update()
                Lista_solictudes_compartidas_de_un_usuario_por_tipos = "YES"
                Exit Function
            Else
                If tipo_consulta = 1 Then
                    reflabel.Text = "Se encontraron " & Datset.Tables(0).Rows.Count & " registro(s) de documentos compartidos   "
                Else
                    reflabel.Text = "Se encontraron " & Datset.Tables(0).Rows.Count & "  registro(s) de documentos compartidos   "
                End If

                grediview.DataSource = Datset
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                update_title.Update()
                For i As Integer = 0 To grediview.Rows.Count - 1
                    grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                    grediview.Rows(i).Attributes.Add("id_estado", grediview.Rows(i).Cells(2).Text.ToString())
                    grediview.Rows(i).Attributes.Add("id_estado_visto", grediview.Rows(i).Cells(3).Text.ToString())
                    grediview.Rows(i).Attributes.Add("id_documento_general", grediview.Rows(i).Cells(5).Text.ToString())
                    If grediview.Rows(i).Cells(7).Text.ToString <> "" Then
                        'If grediview.Rows(i).Cells(7).Text.ToString.Length > 60 Then
                        '    Dim tempo As String = grediview.Rows(i).Cells(7).Text
                        '    tempo = Left(grediview.Rows(i).Cells(7).Text, 60)
                        '    tempo = tempo & " ........"
                        '    grediview.Rows(i).Cells(7).Text = Trim(tempo)

                        'End If
                        Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                        Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                        ihtml.Style.Add("color", "white")
                        ihtml.Attributes.Add("class", "fal fa-folder-open fa-lg")
                        Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                        ahtml.Attributes.Add("Class", "btn btn-success btn-sm")
                        ahtml.Attributes.Add("onclick", "prevent(event,this);")
                        ahtml.Attributes.Add("title", "Documentos compartidos")
                        ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                        ahtml.Attributes.Add("id_estado_", grediview.Rows(i).Cells(2).Text.ToString())
                        ahtml.Attributes.Add("id_estado_visto_", grediview.Rows(i).Cells(3).Text.ToString())
                        ahtml.Attributes.Add("id_documento_general_", grediview.Rows(i).Cells(5).Text.ToString())
                        ahtml.Attributes.Add("tip_event", "ver_doc_comp")
                        ahtml.Style.Add("margin-left", "3px")
                        ahtml.Controls.Add(ihtml)
                        divhtml.Controls.Add(ahtml)

                        ihtml = New HtmlControls.HtmlGenericControl("i")
                        ihtml.Style.Add("color", "white")
                        ihtml.Attributes.Add("class", "fal fa-times fa-lg")
                        ahtml = New HtmlControls.HtmlGenericControl("a")
                        ahtml.Attributes.Add("Class", "btn btn-danger btn-sm")
                        ahtml.Attributes.Add("onclick", "prevent(event,this);")
                        ahtml.Attributes.Add("title", "Elimina registro")
                        ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                        ahtml.Attributes.Add("id_estado_", grediview.Rows(i).Cells(2).Text.ToString())
                        ahtml.Attributes.Add("id_estado_visto_", grediview.Rows(i).Cells(3).Text.ToString())
                        ahtml.Attributes.Add("id_documento_general_", grediview.Rows(i).Cells(5).Text.ToString())
                        ahtml.Attributes.Add("tip_event", "elimina_doc_comp")
                        ahtml.Style.Add("margin-left", "3px")
                        ahtml.Controls.Add(ihtml)
                        divhtml.Controls.Add(ahtml)

                        ihtml = New HtmlControls.HtmlGenericControl("i")
                        ihtml.Style.Add("color", "white")
                        ihtml.Attributes.Add("class", "fal fa-check fa-lg ")
                        ahtml = New HtmlControls.HtmlGenericControl("a")
                        ahtml.Attributes.Add("Class", "btn  btn-primary  btn-sm")
                        ahtml.Attributes.Add("onclick", "prevent(event,this);")
                        ahtml.Attributes.Add("title", "Decisión documento respuesta")
                        ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                        ahtml.Attributes.Add("id_estado_", grediview.Rows(i).Cells(2).Text.ToString())
                        ahtml.Attributes.Add("id_estado_visto_", grediview.Rows(i).Cells(3).Text.ToString())
                        ahtml.Attributes.Add("id_documento_general_", grediview.Rows(i).Cells(5).Text.ToString())
                        ahtml.Attributes.Add("tip_event", "desicion_doc_resp")
                        ahtml.Style.Add("margin-left", "3px")
                        ahtml.Controls.Add(ihtml)
                        divhtml.Controls.Add(ahtml)

                        ihtml = New HtmlControls.HtmlGenericControl("i")
                        ihtml.Style.Add("color", "white")
                        ihtml.Attributes.Add("class", "fal fa-list-alt fa-lg ")
                        ahtml = New HtmlControls.HtmlGenericControl("a")
                        ahtml.Attributes.Add("Class", "btn btn-info btn-sm")
                        ahtml.Attributes.Add("onclick", "prevent(event,this);")
                        ahtml.Attributes.Add("title", "Ver registros de  aprobación")
                        ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                        ahtml.Attributes.Add("id_estado_", grediview.Rows(i).Cells(2).Text.ToString())
                        ahtml.Attributes.Add("id_estado_visto_", grediview.Rows(i).Cells(3).Text.ToString())
                        ahtml.Attributes.Add("id_documento_general_", grediview.Rows(i).Cells(5).Text.ToString())
                        ahtml.Attributes.Add("tip_event", "ver_reg_colab")
                        ahtml.Style.Add("margin-left", "3px")
                        ahtml.Controls.Add(ihtml)
                        divhtml.Controls.Add(ahtml)
                        divhtml.Style.Add("display", "inline-flex")
                        grediview.Rows(i).Cells(0).Controls.Add(divhtml)

                    End If
                    For z As Integer = 0 To grediview.Rows(i).Cells.Count - 1
                        If z > 0 Then
                            grediview.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_cort_tr_flex")
                            grediview.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(event,this);")
                        End If

                    Next
                Next
                Dim Refclas As New ClassGredview
                Result = Refclas.add_clase_acender_decender(colum_order_name, _
                                                            HttpContext.Current.Session.Item("Sort_matri_colum_compartido"), _
                                                            order_colum, _
                                                            grediview)
                If Result <> "YES" Then
                    Lista_solictudes_compartidas_de_un_usuario_por_tipos = "Error add clase funcion  Lista_solictudes_compartidos_de_un_usuario" & Result
                    Exit Function
                End If
                Lista_solictudes_compartidas_de_un_usuario_por_tipos = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_solictudes_compartidas_de_un_usuario_por_tipos = "Inconsistencia general función Lista_solictudes_compartidos_de_un_usuario " & ex.Message
        End Try

    End Function
    Function Retorna_documentos_relacionados_a_registro_compartido(ByVal id_documento_compartido As Integer) As String
        Try
            Dim stru_documento_compartido() As stru_documentos_compartidos = Nothing
            Dim sql_consulta As String = "SELECT RA_CD_DOUMENTOS_GABINETE_COMPARTIDOS,RA_CD_DOCUMENTOS_COMPARTIDOS_ID_RA_CD_DOCUMENTOS_COMPARTIDOS," _
                                        & "ID_IMAGEN,NOMBRE_GABINETE from ra_cd_doumentos_gabinete_compartidos " & _
                       " where RA_CD_DOCUMENTOS_COMPARTIDOS_ID_RA_CD_DOCUMENTOS_COMPARTIDOS=" & id_documento_compartido
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_doumentos_gabinete_compartidos")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_documentos_relacionados_a_registro_compartido = "Error listando Retorna_documentos_relacionados_a_registro_compartido " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_documentos_relacionados_a_registro_compartido = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_documento_compartido(i)
                    stru_documento_compartido(i).identificador = i
                    stru_documento_compartido(i).id_imagen = Datset.Tables(0).Rows(i).Item(2)
                    stru_documento_compartido(i).nombre_gabinete = Datset.Tables(0).Rows(i).Item(3)
                Next
                HttpContext.Current.Session.Item("GA_STRU_DOCUMENTO_COMPARTIDO") = stru_documento_compartido
                Retorna_documentos_relacionados_a_registro_compartido = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_documentos_relacionados_a_registro_compartido = "Inconsistencia general función Lista_documentos_relacionados_a_registro_compartido " & ex.Message
        End Try
    End Function

    Function Retorna_numero_de_documentos_compartidos_de_un_usuario(ByVal id_usuario As Integer, _
                                                                    ByRef numero_solicitud As Integer) As String
        Try
            Dim sql_consulta As String = "SELECT ID_USUARIOS_DOCUMENTOS_COMPARTIDOS  from ra_cd_usuarios_documentos_compartidos AS rcs" & _
                       " where rcs.Remit_Dest_Interno_id_remit_dest_Int=" & id_usuario & _
                       " and rcs.ESTADO_ELIMINADO=1"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_usuarios_solicitudes_aprobacion")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_numero_de_documentos_compartidos_de_un_usuario = "Error listando Retorna_numero_de_documentos_compartidos_de_un_usuario " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                numero_solicitud = Datset.Tables(0).Rows.Count
                Retorna_numero_de_documentos_compartidos_de_un_usuario = "YES"
                Exit Function
            Else
                numero_solicitud = Datset.Tables(0).Rows.Count
                Retorna_numero_de_documentos_compartidos_de_un_usuario = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_numero_de_documentos_compartidos_de_un_usuario = "Inconsistencia general función Retorna_numero_de_solicitudes_aprobacion_de_un_usuario " & ex.Message
        End Try
    End Function

    Function Retorna_numero_de_documentos_compartidos_de_un_usuario_para_otros_usuarios(ByVal id_usuario As Integer, ByRef numero_solicitud As Integer) As String
        Try
            Dim sql_consulta As String = "SELECT ID_RA_CD_DOCUMENTOS_COMPARTIDOS  from ra_cd_documentos_compartidos AS rcs" & _
                       " where rcs.Remit_Dest_Interno_id_Remit_Dest_Int=" & id_usuario & _
                       " and rcs.ESTADO_ELIMINADO=1"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_documentos_compartidos")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_numero_de_documentos_compartidos_de_un_usuario_para_otros_usuarios = "Error listando Retorna_numero_de_documentos_compartidos_de_un_usuario_para_otros_usuarios " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                numero_solicitud = Datset.Tables(0).Rows.Count
                Retorna_numero_de_documentos_compartidos_de_un_usuario_para_otros_usuarios = "YES"
                Exit Function
            Else
                numero_solicitud = Datset.Tables(0).Rows.Count
                Retorna_numero_de_documentos_compartidos_de_un_usuario_para_otros_usuarios = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_numero_de_documentos_compartidos_de_un_usuario_para_otros_usuarios = "Inconsistencia general función Retorna_numero_de_documentos_compartidos_de_un_usuario_para_otros_usuarios " & ex.Message
        End Try
    End Function

    Function Elimina_registro_general_documento_compartido(ByVal id_documento_compartido As Integer) As String
        Try
            Dim stru_documento As STRU_DOCUMENTO_COMPARTIDO_USUARIOS = Nothing
            Dim result As String = ""
            Dim stru_general As STRU_DOCUMENTO_COMPARTIDO_GENERAL = Nothing
            Dim refclas_cd_compartidos As New Class_ra_Cd_Documentos_Compartidos
            result = refclas_cd_compartidos.SolicitaEstructuraGeneraldocumentosCompartido(id_documento_compartido,
                                                                                         stru_general)
            If result <> "YES" Then
                Elimina_registro_general_documento_compartido = result
                Exit Function
            Else
                If stru_documento.TIPO_REGISTRO_COMPARTIDO = 3 Then
                    If stru_general.ESTADO_CONFIRMACION_COLABORACION = 0 Then
                        Elimina_registro_general_documento_compartido = "No se puede eliminar el registro por que tiene una solicitud de aprobación que está abierta"
                        Exit Function
                    End If
                End If
                If stru_general.ESTADO_ELIMINADO = 0 Then
                    Elimina_registro_general_documento_compartido = "El registro ya se encuentra en la badeja de eliminados, imposible eliminar"
                    Exit Function
                End If
            End If
            Dim sql_consulta As String = "update  ra_cd_documentos_compartidos  set ESTADO_ELIMINADO=0" &
                      " where ID_RA_CD_DOCUMENTOS_COMPARTIDOS=" & id_documento_compartido
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_documentos_compartidos")
            result = ref.SELECTION_INSERT_COMMAND(sql_consulta)
            If result <> "YES" Then
                Elimina_registro_general_documento_compartido = "Error Elimina_registro_general_documento_compartido " & result
                Exit Function
            Else
                If HttpContext.Current.Session.Item("GA_STRU_DOCUMENTO_NUMERO_COMPARTIDO") > 0 Then
                    HttpContext.Current.Session.Item("GA_STRU_DOCUMENTO_NUMERO_COMPARTIDO") = HttpContext.Current.Session.Item("GA_STRU_DOCUMENTO_NUMERO_COMPARTIDO") - 1
                End If
            End If
            Elimina_registro_general_documento_compartido = "YES"
        Catch ex As Exception
            Elimina_registro_general_documento_compartido = "Inconsistencia general función Elimina_registro_usuario_documento_compartido " & ex.Message
        End Try
    End Function
    Function AdjuntaContanciaDecisioDocumentoCompartido(ByVal IdDocumentoCompartido As Integer,
                                                        ByVal IdUsuarioGestionDesicion As Integer,
                                                        ByVal DescripcionDesicion As String,
                                                        ByVal stru_comp_general As STRU_DOCUMENTO_COMPARTIDO_GENERAL,
                                                        ByVal EstructuraDocumentoCompartido() As stru_docu_compartido,
                                                        ByRef NumeroPaginas As Integer,
                                                        ByRef FileCopiaArchivoGabinete As String,
                                                        ByRef MatriDocumentoGabinete() As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Adjunta constancia de inscripcion al documento compartido
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdDcoumentoCompartido           : Representa la identificación de la tarea compartida
        'IdUsuarioGestionDesicion        : Representa la identificación del usuario de gestión
        'DescripcionDesicion             : Representa la descripcion de la decisión
        'stru_compartido_general         : Representa la estructura general del documento compartido
        'EstructuraDocumentoCompartido   : Representa la estructura de documentos compartidos
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'NumeroPaginas                   : Retorna el número de páginas del documento de aprobación con
        'el certifcado de aprobación.
        '                 
        'FileCopiaArchivoGabinete       : Retorna el archivo copia con el certificado de decisión
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-05-14
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim stru_parameter_image As stru_paramter_image = Nothing
            Result = ClassDaGabinete.SolicitaEtructuraImagenGabinete(EstructuraDocumentoCompartido(0).NOMBRE_GABINETE,
                                                                     EstructuraDocumentoCompartido(0).ID_IMAGEN,
                                                                     stru_parameter_image)
            If Result <> "YES" Then
                AdjuntaContanciaDecisioDocumentoCompartido = Result
                Exit Function
            End If
            Dim Class_da_extension As New Class_da_extension
            Dim ExtensionDocumento As String = ""
            Result = Class_da_extension.RetornaExtensionTipoDocumento(stru_parameter_image.DBT_TIPO_IMAGEN,
                                                                      ExtensionDocumento)
            If Result <> "YES" Then
                AdjuntaContanciaDecisioDocumentoCompartido = Result
                Exit Function
            End If
            If UCase(ExtensionDocumento) <> ".PDF" Then
                AdjuntaContanciaDecisioDocumentoCompartido = "YES"
                Exit Function
            End If

            ClassDaGabinete.Solicita_matriz_documentos_almacenados_gabinete(EstructuraDocumentoCompartido(0).ID_IMAGEN,
                                                                            EstructuraDocumentoCompartido(0).NOMBRE_GABINETE,
                                                                            MatriDocumentoGabinete)
            If Result <> "YES" Then
                AdjuntaContanciaDecisioDocumentoCompartido = Result
                Exit Function
            End If
            Dim Class_ra_cd_detalle_documento_compartido As New Class_ra_cd_detalle_documento_compartido
            Dim FileCertificado As String = ""
            Result = Class_ra_cd_detalle_documento_compartido.SolicitaCertificadoDesicionDocumentoCompartido(IdDocumentoCompartido,
                                                                                                             IdUsuarioGestionDesicion,
                                                                                                             DescripcionDesicion,
                                                                                                             stru_comp_general,
                                                                                                             EstructuraDocumentoCompartido,
                                                                                                             MatriDocumentoGabinete,
                                                                                                             FileCertificado)
            If Result <> "YES" Then
                AdjuntaContanciaDecisioDocumentoCompartido = Result
                Exit Function
            End If
            Dim Rutatemp As String = HttpContext.Current.Session.Item("GA_RUTA_TEMP_GESTION") & "\"
            FileCopiaArchivoGabinete = Rutatemp & EstructuraDocumentoCompartido(0).NOMBRE_GABINETE & "-" & EstructuraDocumentoCompartido(0).ID_IMAGEN & UCase(ExtensionDocumento)
            If File.Exists(Rutatemp) = True Then
                Kill(FileCopiaArchivoGabinete)
            End If
            File.Copy(MatriDocumentoGabinete(1), FileCopiaArchivoGabinete, True)
            Dim Class_ItexShare As New Class_ItexShare
            Result = Class_ItexShare.UnirArchivoPdf(FileCopiaArchivoGabinete,
                                                    FileCertificado,
                                                    Rutatemp,
                                                    NumeroPaginas)
            If Result <> "YES" Then
                AdjuntaContanciaDecisioDocumentoCompartido = Result
                Exit Function
            End If
            Kill(FileCertificado)
            AdjuntaContanciaDecisioDocumentoCompartido = "YES"
        Catch ex As Exception
            AdjuntaContanciaDecisioDocumentoCompartido = "Inconsistencia general funcion AdjuntaContanciaDecisioDocumentoCompartido " & ex.Message
        End Try
    End Function
    Function ConfirmaDesicionDocumentoCompartido(ByVal IdDocumentoCompartidoUsuario As Integer,
                                                 ByVal nota_registro_desicion As String,
                                                 ByVal descripcion_tipo_aprobacion As String,
                                                 ByRef resultado_envio_correo As String,
                                                 ByRef hiden_cambio_solicitud_usuario As String,
                                                 ByRef Hidden_resultado_aprobacion As String,
                                                 ByRef IdDocumento As Integer,
                                                 ByRef Gabinete As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Registra la de decisión de un documento compartido
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdDocumentoCompartidoUsuario : Representa la identificación del registro del documento compartido
        '                               al usuario 
        'NotaRegistroDecision           : Representa la nota del registro de decisión 
        'descripcion_tipo_aprobacion    : Representa la descripción de la decisicón del usuario

        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'resultado_envio_correo         : Retorna el resultado del envión de correo
        'hiden_cambio_solicitud_usuario : Retorna el cambio de estado decisión del usuario
        'Hidden_resultado_aprobacion    : Retorna el resultado de la decisión de aprobacion
        'IdDocumento                    : Retorna la identiicación del documento aprobado en el gabinete
        'Gabinete                       : Retorna el nombre del gabinete
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-05-15
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Dim Result As String = ""
        Dim Refclas As New Class_ra_cd_usuarios_documentos_compartidos
        If descripcion_tipo_aprobacion = "" Then
            ConfirmaDesicionDocumentoCompartido = "Debe informar su decisión sobre el documento compartido."
            Exit Function
        End If
        If nota_registro_desicion = "" Then
            ConfirmaDesicionDocumentoCompartido = "Debe informar la nota de su desición sobre el documento compartido."
            Exit Function
        End If
        '//-----Solicita la estructura del registro del docmento compartido-------////
        Dim stru_socitud_usuario As STRU_DOCUMENTO_COMPARTIDO_USUARIOS = Nothing
        Result = Refclas.SolicitaeEstructuraDocumentoCompartido(IdDocumentoCompartidoUsuario,
                                                                stru_socitud_usuario)
        If Result <> "YES" Then
            ConfirmaDesicionDocumentoCompartido = Result
            Exit Function
        End If
        '//-----Valida el estado de respuesta de la solicitud de aprobación del usuario------------////
        If stru_socitud_usuario.ESTADO_RESPUESTA_SOLICITUD > 0 Then
            ConfirmaDesicionDocumentoCompartido = "El usuario respondió la solicitud con la decisión (" & stru_socitud_usuario.DESCRIPCION_ESTADO_RESPUESTA & "); por lo tanto, no es posible registrar una nueva decisión."
            Exit Function
        End If
        '//---Solicita la estructura general de un documento compartido---//
        Dim stru_comp_general As New STRU_DOCUMENTO_COMPARTIDO_GENERAL
        Dim refclas_cd_compartidos As New Class_ra_Cd_Documentos_Compartidos
        Result = refclas_cd_compartidos.SolicitaEstructuraGeneraldocumentosCompartido(stru_socitud_usuario.ID_RA_CD_DOCUMENTOS_COMPARTIDOS,
                                                                                      stru_comp_general)
        If Result <> "YES" Then
            ConfirmaDesicionDocumentoCompartido = Result
            Exit Function
        End If
        '//-----Valida el estado de aprobación general de la solicitud de aprobación ------------////
        If stru_comp_general.ESTADO_APROBACION <> 0 And stru_socitud_usuario.ESTADO_RESPUESTA_SOLICITUD > 0 Then
            ConfirmaDesicionDocumentoCompartido = "La solicitud se encuentra en estado (" & stru_comp_general.DESCRIPCION_ESTADO_APROBACION & "); por lo tanto, no es posible registrar una nueva decisión. "
            Exit Function
        End If
        Dim tipo_aprobacion As Integer = 0
        Dim descripcion_estado_aprobacion As String = ""
        If descripcion_tipo_aprobacion = "Aprobado" Then
            tipo_aprobacion = 1
            descripcion_estado_aprobacion = "Aprobado"
        End If
        If descripcion_tipo_aprobacion = "Desaprobado" Then
            tipo_aprobacion = 2
            descripcion_estado_aprobacion = "Desaprobado"
        End If
        If descripcion_tipo_aprobacion = "Archivado" Then
            tipo_aprobacion = 3
            descripcion_estado_aprobacion = "Archivado"
        End If
        Dim correo_usuario_solicitante As String = ""
        Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
        Dim Reclas_remit_dest_interno As New Class_remit_dest_interno
        Result = Reclas_remit_dest_interno.Solicita_correo_usuario_gestion(stru_socitud_usuario.Remit_Dest_Interno_id_remit_dest_Int,
                                                                           correo_usuario_solicitante)
        If Result <> "YES" Then
            ConfirmaDesicionDocumentoCompartido = Result
            Exit Function
        End If
        Dim date1al As String = ""
        Dim ref_almacen As New ClassRadicador
        Dim refclas_gestion_fechas As New ClassGestionFechas
        Result = refclas_gestion_fechas.Formatea_fecha_time_framework(Date.Now,
                                                                      date1al)
        If Result <> "YES" Then
            ConfirmaDesicionDocumentoCompartido = Result
            Exit Function
        End If
        Dim stiempo As Object = Nothing
        Dim minuno As Object = Nothing
        Dim hora As Object = Nothing
        Dim dias_calendario As Object = Nothing
        Dim dias_no_habiles As Object = Nothing
        Result = refclas_gestion_fechas.Solicita_tiempo_respuesta_tramite(stru_comp_general.FECHA_REGISTRO_SOLICITUD,
                                                                          stiempo,
                                                                          hora,
                                                                          minuno,
                                                                          dias_calendario,
                                                                          dias_no_habiles)
        If Result <> "YES" Then
            ConfirmaDesicionDocumentoCompartido = Result
            Exit Function
        End If
        Dim Class_ra_cd_documentos_gabinete_compartido As New Class_ra_cd_documentos_gabinete_compartido
        Dim EstructuraDocumentoCompartido() As stru_docu_compartido = Nothing
        Result = Class_ra_cd_documentos_gabinete_compartido.SolicitaDatosEstructuraDocumentoCompartido(stru_comp_general.ID_RA_CD_DOCUMENTOS_COMPARTIDOS,
                                                                                                       EstructuraDocumentoCompartido)
        If Result <> "YES" Then
            ConfirmaDesicionDocumentoCompartido = Result
            Exit Function
        End If
        '---------------------------------------------
        'Retorna estado de solicitudes de aprobación
        '-1 Registra la solicitud
        '-2 No hay desición por que no se completo el
        'numero de aprobaciones
        '--------------------------------------------
        Dim sql_update_actualizacion_general As String = ""
        Dim Refclas_ As New Class_ra_cd_doumentos_colaboracion_compartidos
        Dim EstadoAProbacion As Integer = -3
        Dim EstadoAprobacionFinal As Integer = stru_comp_general.ESTADO_APROBACION
        Dim decripcion_estado_solicitud As String = ""
        If stru_comp_general.ESTADO_APROBACION = 0 Then
            Result = Refclas_.SolicitaEstadoSolicitudAprobacionDocumentoCompartido(stru_comp_general.ID_RA_CD_DOCUMENTOS_COMPARTIDOS,
                                                                                   EstadoAProbacion,
                                                                                   decripcion_estado_solicitud,
                                                                                   descripcion_tipo_aprobacion)
            If Result <> "YES" Then
                ConfirmaDesicionDocumentoCompartido = Result
                Exit Function
            End If
            '------------Caso única solicitud, se toma la desicion del usuario como decisión general-----------
            If EstadoAProbacion = -1 Then
                EstadoAprobacionFinal = tipo_aprobacion
                decripcion_estado_solicitud = descripcion_tipo_aprobacion
                sql_update_actualizacion_general = "Update ra_cd_documentos_compartidos set ESTADO_APROBACION=" & tipo_aprobacion & ",DESCRIPCION_ESTADO_APROBACION='" &
                    decripcion_estado_solicitud & "',FECHA_REGISTRO_APROBACION='" & date1al & "',TIEMPO_RESPUESTA_APROBACION=" & stiempo & " where ID_RA_CD_DOCUMENTOS_COMPARTIDOS=" & stru_comp_general.ID_RA_CD_DOCUMENTOS_COMPARTIDOS
            End If
            '------------Caso multiplex solicitudes, se toma la desicion según el el numero de decisiones-----------
            If EstadoAProbacion <> -1 And EstadoAProbacion <> -2 Then
                EstadoAprobacionFinal = EstadoAProbacion
                sql_update_actualizacion_general = "Update ra_cd_documentos_compartidos set ESTADO_APROBACION=" & EstadoAProbacion & ",DESCRIPCION_ESTADO_APROBACION='" &
                     decripcion_estado_solicitud & "',FECHA_REGISTRO_APROBACION='" & date1al & "',TIEMPO_RESPUESTA_APROBACION=" & stiempo & " where ID_RA_CD_DOCUMENTOS_COMPARTIDOS=" & stru_comp_general.ID_RA_CD_DOCUMENTOS_COMPARTIDOS
            End If
        End If
        Dim NumeroPaginas As Integer = 0
        Dim FileCopiaArchivoGabinete As String = ""
        Dim MatriDocumentoGabinete() As String = Nothing
        If EstadoAprobacionFinal = 1 Then
            '//----------Adjunta constancia de decisición al documento y lo copia a un archivo temporal----///
            stru_comp_general.FECHA_REGISTRO_APROBACION = date1al
            stru_comp_general.DESCRIPCION_ESTADO_APROBACION = decripcion_estado_solicitud
            stru_comp_general.ESTADO_APROBACION = EstadoAprobacionFinal
            Result = AdjuntaContanciaDecisioDocumentoCompartido(stru_comp_general.ID_RA_CD_DOCUMENTOS_COMPARTIDOS,
                                                                stru_socitud_usuario.Remit_Dest_Interno_id_remit_dest_Int,
                                                                decripcion_estado_solicitud,
                                                                stru_comp_general,
                                                                EstructuraDocumentoCompartido,
                                                                NumeroPaginas,
                                                                FileCopiaArchivoGabinete,
                                                                MatriDocumentoGabinete)
            If Result <> "YES" Then
                ConfirmaDesicionDocumentoCompartido = Result
                Exit Function
            End If

        End If
        Dim Class_ra_ver_version_documento As New Class_ra_ver_version_documento
        'Confirma_desicion_documento_compartido = "ojo"
        'Exit Function
        Dim sql_update As String = "Update ra_cd_usuarios_documentos_compartidos set ESTADO_RESPUESTA_SOLICITUD=" & tipo_aprobacion & ",DESCRIPCION_ESTADO_RESPUESTA='" & descripcion_tipo_aprobacion & "'" &
            ",FECHA_RESPUESTA_SOLICITUD='" & date1al & "',TIEMPO_RESPUESTA_SOLICITANTE=" & stiempo &
            " where ID_USUARIOS_DOCUMENTOS_COMPARTIDOS=" & IdDocumentoCompartidoUsuario
        hiden_cambio_solicitud_usuario = descripcion_tipo_aprobacion
        Dim sql_nota_aprobacion As String = ""
        If nota_registro_desicion <> "" Then
            sql_nota_aprobacion = "Insert into RA_CD_NOTAS_USUARIO_DOCUMENTO_COMPARTIDO (ID_USUARIOS_DOCUMENTOS_COMPARTIDOS,NOTA_DOCUMENTO_COMPARTIDO," &
                "FECHA_NOTA_SOLICITUD,DOC_COMP_ID_RA_CD_DOCUMENTOS_COMPARTIDOS) values (" &
            IdDocumentoCompartidoUsuario & ",'" & nota_registro_desicion & "','" & date1al & "'," & stru_comp_general.ID_RA_CD_DOCUMENTOS_COMPARTIDOS & ")"
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction = Nothing
        Dim sqlresultinsert As Integer = 0
        Try
            '-------------------------------------------------
            'Actualiza solicitud de aprobación usuario
            '-------------------------------------------------
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = sql_update
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                ConfirmaDesicionDocumentoCompartido = "Imposible actualizar la solicitud de aprobación del usuario  "
                myConnection.Close()
                Exit Function
            End If
            '------------------------------------------------
            'Actualiza solicitud general aprobacion
            '------------------------------------------------
            If sql_update_actualizacion_general <> "" Then
                myCommand.CommandText = sql_update_actualizacion_general
                sqlresultinsert = myCommand.ExecuteNonQuery()
                If sqlresultinsert = 0 Then
                    ConfirmaDesicionDocumentoCompartido = "Imposible actualizar la solicitud general de aprobación  "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            '------------------------------------------------
            'Registra nota solicitud usuario
            '------------------------------------------------
            If sql_nota_aprobacion <> "" Then
                myCommand.CommandText = sql_nota_aprobacion
                sqlresultinsert = myCommand.ExecuteNonQuery()
                If sqlresultinsert = 0 Then
                    ConfirmaDesicionDocumentoCompartido = "Imposible registar nota de solicitud de aprobación  "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            '---//Actualiza numero de documentos en el gabinete
            If EstadoAprobacionFinal = 1 And FileCopiaArchivoGabinete <> "" Then
                Dim SQLupdateGabinete As String = "Update " & EstructuraDocumentoCompartido(0).NOMBRE_GABINETE & " set PAG=" & NumeroPaginas &
                    " where ID=" & EstructuraDocumentoCompartido(0).ID_IMAGEN
                sqlresultinsert = myCommand.ExecuteNonQuery()
                If sqlresultinsert = 0 Then
                    ConfirmaDesicionDocumentoCompartido = "Imposible actualizar el número de imagenes en el gabinete  "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
                File.Copy(FileCopiaArchivoGabinete,
                          MatriDocumentoGabinete(1),
                          True)
                Kill(FileCopiaArchivoGabinete)
                IdDocumento = EstructuraDocumentoCompartido(0).ID_IMAGEN
                Gabinete = EstructuraDocumentoCompartido(0).NOMBRE_GABINETE
            End If
            myTrans.Commit()
            myConnection.Close()
            If sql_update_actualizacion_general <> "" Then
                Dim nota As String = decripcion_estado_solicitud & " respuesta solicitud de paobación  " &
                                     stru_comp_general.ID_RA_CD_DOCUMENTOS_COMPARTIDOS

                Result = Me.Envio_correo_electronico_aprobacion_documento(correo_usuario_solicitante,
                                                                          nota,
                                                                          stru_comp_general.FECHA_REGISTRO_SOLICITUD,
                                                                          stru_comp_general.ID_RA_CD_DOCUMENTOS_COMPARTIDOS,
                                                                          decripcion_estado_solicitud,
                                                                          stru_comp_general.Remit_Dest_Interno_id_remit_dest_Int)

            End If
            resultado_envio_correo = Result
            Hidden_resultado_aprobacion = "YES"
            ConfirmaDesicionDocumentoCompartido = "YES"
        Catch ex As Exception
            myTrans.Rollback()
            If Not myTrans.Connection Is Nothing Then
                ConfirmaDesicionDocumentoCompartido = "An exception of type " + ex.GetType().ToString() +
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
            If myTrans.Connection.State = ConnectionState.Open Then
                myConnection.Close()
            End If
            ConfirmaDesicionDocumentoCompartido = "Error General " & ex.Message
            Exit Function
        Finally
            Hidden_resultado_aprobacion = ConfirmaDesicionDocumentoCompartido
        End Try
    End Function
    Function Envio_correo_electronico_aprobacion_documento(ByVal correos_relacionados As String, _
                                                           ByVal nota_solicitud As String, _
                                                           ByVal fecha_solicitud As String, _
                                                           ByVal numero_solictud As Integer, _
                                                           ByVal descripcion_tipo_aprobacion As String, _
                                                           ByVal id_usuario_gestion As Integer) As String
        '----------------------------------------------------------
        'Función : Envía correo electrónico usuario de solicitud
        'Fecha : 2019-02-19
        'Ing Miguel Angel Urueta Miranda 
        '----------------------------------------------------------
        Try
            Dim refclas_gestion As New Classgestionrespuesta
            Dim Result As String = ""
            Dim nombre_area As String = ""
            Dim id_area As Integer = 0
            Dim nombre_usuario As String = ""
            Dim cargo_usuario As String = ""
            Result = refclas_gestion.Retorna_datos_asignacion_respuesta_usuario_gestion(id_usuario_gestion, _
                                                                                        id_area, _
                                                                                        nombre_area, _
                                                                                        nombre_usuario, _
                                                                                        cargo_usuario)
            If Result <> "YES" Then
                Envio_correo_electronico_aprobacion_documento = Result
                Exit Function
            End If
            Dim spli_texto_correo() As String = {"Se " & descripcion_tipo_aprobacion & "(a) la solicitud de aprobación del documento compartido número " & numero_solictud, _
            "Fecha solicitud " & fecha_solicitud, "Nota Solicitud ", nota_solicitud, _
            "Nombre solicitante : " & nombre_usuario, "Cargo Solicitante : " & cargo_usuario, "Area Solicitante : " & nombre_area, "Por favor ingrese al sistema de gestión documental a la opción (Notificaciones y solicitudes - solicitudes de aprobación) y revise la solicitud"}
            Dim Refclas As New ClassCorreo
            Dim matri_documentos() As String = Nothing
            Result = Refclas.Envio_Correo_confirmacion_solicitud_aprobacion_respuesta(spli_texto_correo, _
                                                                                      correos_relacionados, _
                                                                                      descripcion_tipo_aprobacion & _
                                                                                      " solicitud de aprobación de documento compartido " & _
                                                                                      numero_solictud, _
                                                                                      matri_documentos)
            If Result <> "YES" Then
                Envio_correo_electronico_aprobacion_documento = Result
                Exit Function
            Else
                Envio_correo_electronico_aprobacion_documento = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Envio_correo_electronico_aprobacion_documento = "Inconsistencia general función Envio_correo_electronico_aprobacion_documento " & ex.Message
        End Try
    End Function
    Function Lista_usuarios_relacionados_a_solictud_de_aprobacion_doc_compartido(ByVal id_solicitud_aprobacion As Integer, _
                                                                                 ByRef grediview As GridView, _
                                                                                 ByRef HiddenEmailconsulta As Object, _
                                                                                 ByRef reflabel As Label, _
                                                                                 ByRef hideselecion As Object, _
                                                                                 ByRef update As UpdatePanel) As String
        Try

            Dim sql_consulta As String = "SELECT ID_USUARIOS_DOCUMENTOS_COMPARTIDOS," & _
                    "DESCRIPCION_ESTADO_RESPUESTA AS ESTADO,rcd.DESCRIPCION_ESTADO_APROBACION as ESTADO_GENERAL,FECHA_RESPUESTA_SOLICITUD as " & _
                    "FECHA_RESPUESTA,rdi.Nombre_Remitente as NOMBRE," & _
                    "rdi.Cargo_Remite as CARGO from ra_cd_usuarios_documentos_compartidos AS rcs" & _
                    " INNER JOIN ra_cd_documentos_compartidos AS rcd on (rcd.ID_RA_CD_DOCUMENTOS_COMPARTIDOS=rcs.ID_RA_CD_DOCUMENTOS_COMPARTIDOS)" & _
                    " INNER JOIN remit_dest_interno AS rdi on (rdi.id_Remit_Dest_Int=rcs.Remit_Dest_Interno_id_Remit_Dest_Int)  where " & _
                    "rcs.ID_RA_CD_DOCUMENTOS_COMPARTIDOS=" & id_solicitud_aprobacion
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_usuarios_documentos_compartidos")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_usuarios_relacionados_a_solictud_de_aprobacion_doc_compartido = "Error Lista_usuarios_relacionados_a_solictud_de_aprobacion_doc_compartido función " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                HiddenEmailconsulta.value = "YES"
                reflabel.Text = "Se encontraron 0 registro(s) "
                grediview.DataSource = Nothing
                'grediview.DataKeyNames = DataKey
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                Lista_usuarios_relacionados_a_solictud_de_aprobacion_doc_compartido = "YES"
                Exit Function
            Else
                HiddenEmailconsulta.value = "YES"
                reflabel.Text = "Se encontraron " & Datset.Tables(0).Rows.Count & " registro(s) "
                'grediview.DataKeyNames = DataKey
                grediview.DataSource = Datset
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                For i As Integer = 0 To grediview.Rows.Count - 1
                    grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                Next
                Lista_usuarios_relacionados_a_solictud_de_aprobacion_doc_compartido = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_usuarios_relacionados_a_solictud_de_aprobacion_doc_compartido = "Inconsistencia general función Lista_usuarios_relacionados_a_solictud_de_aprobacion_doc_compartido " & ex.Message
        End Try

    End Function
End Class
