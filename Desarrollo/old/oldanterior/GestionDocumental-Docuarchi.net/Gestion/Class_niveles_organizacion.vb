Imports MySql.Data.MySqlClient

Public Class Class_niveles_organizacion
    Function Solicita_propietario_nivel_expedientes(ByVal id_nivel As Long, _
                                                    ByRef id_usuario_propietario As Integer) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_pro_niveles")
            Dim sql_consulta As String = "Select remit_dest_interno_id_Remit_Dest_Int from ra_pro_niveles" & _
                " where id_nivel=" & id_nivel
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_propietario_nivel_expedientes = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_propietario_nivel_expedientes = "Imposible encontrar el propietario del nivel  (" & id_nivel & ")"
                Exit Function
            Else
                id_usuario_propietario = Datset.Tables(0).Rows(0).Item(0)
                Solicita_propietario_nivel_expedientes = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_propietario_nivel_expedientes = "Inconsistencia general función Solicita_propietario_nivel_expedientes " & ex.Message
        End Try
    End Function
    Function Agregar_nivel_clasificacion(ByVal id_nivel_padre As Integer, _
                                         ByVal nombre_nivel As String, _
                                         ByVal id_usuario_gestion As Integer, _
                                         ByRef tree As TreeView, _
                                         ByRef tre_node As TreeNode, _
                                         ByRef update As UpdatePanel, _
                                         ByRef last_id_nivle As Object) As String
        Try

            Dim existencia As String = ""
            Dim Result As String = ""
            Dim Ref_class_pro_nivel As New Class_ra_pro_niveles
            Dim Ref_class_permisos_usuarios As New Class_ra_pro_permisos_niveles
            Dim numero_nivel As Integer = 0
            Dim nivel_padre_raiz As Integer = 0
            Dim ref_id_nivel_padre As Integer = id_nivel_padre
            Dim sql_permisos As String = ""
            Dim stru_permiso_nivel() As stru_permiso_nivel = Nothing
            Dim Estado_propietario As String = ""
            If id_nivel_padre <> 0 Then
                Result = Ref_class_pro_nivel.Solicita_estado_nivel_propietario(id_usuario_gestion, _
                                                                             id_nivel_padre, _
                                                                             Estado_propietario)
                If Result <> "YES" Then
                    Agregar_nivel_clasificacion = Result
                    Exit Function
                End If
                If Estado_propietario = "NO" Then
                    Agregar_nivel_clasificacion = "El usuario no es propietario del nivel padre, imposible anidar otro nivel"
                    Exit Function
                End If
                Result = Ref_class_pro_nivel.Solicita_nivel_nodo(id_nivel_padre, _
                                                                 numero_nivel)
                If Result <> "YES" Then
                    Agregar_nivel_clasificacion = Result
                    Exit Function
                End If
                nivel_padre_raiz = Ref_class_pro_nivel.Solicita_id_nivel_padre_raiz(ref_id_nivel_padre)
                Result = Ref_class_permisos_usuarios.Solicita_matriz_datos_estrctura_permiso_nivel_usuario_gestion(ref_id_nivel_padre, _
                                                                                                                   stru_permiso_nivel)
                If Result <> "YES" Then
                    Agregar_nivel_clasificacion = Result
                    Exit Function
                End If

            End If
            Dim myConnection As New MySqlConnection
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            ref.Returna_Conexion_Mysql(myConnection)
            Dim myTrans As MySqlTransaction
            Dim mySqldatReader As MySqlDataReader
            Dim sqlresultinsert As Integer = 0
            Try
                'myConnection.Open()
                Dim myCommand As MySqlCommand = myConnection.CreateCommand()
                myTrans = myConnection.BeginTransaction()
                myCommand.Connection = myConnection
                myCommand.Transaction = myTrans
                Dim Parametro_Select_System1 As String = " SELECT contador_nivel FROM ra_pro_niveles " & _
                " where id_nivel=" & id_nivel_padre & " for update"
                If id_nivel_padre <> 0 Then
                    myCommand.CommandText = Parametro_Select_System1
                    mySqldatReader = myCommand.ExecuteReader()
                    If mySqldatReader Is Nothing Then
                        Agregar_nivel_clasificacion = "Imposible Encontrar Registro funcion Agregar_nivel_clasificacion Error Conexión"
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                    If mySqldatReader.HasRows = False Then
                        Agregar_nivel_clasificacion = "Imposible Encontrar el control de niveles del nivel (" & id_nivel_padre & ")"
                        myConnection.Close()
                        Exit Function
                    End If
                    mySqldatReader.Read()
                    numero_nivel = mySqldatReader.Item(0)
                    numero_nivel = numero_nivel + 1
                    mySqldatReader.Close()
                End If
                Dim estado_nivel_compartido As Integer = 0
                If Not stru_permiso_nivel Is Nothing Then
                    estado_nivel_compartido = 1
                End If
                Dim Parametro_registra_nivel As String = "insert into ra_pro_niveles  (remit_dest_interno_id_Remit_Dest_Int," & _
                    "nivel,nombre_nivel,id_nivel_padre,estado_padre,conta_expediente,estado_nivel,contador_nivel,estado_nivel_compartido) values " & _
                    "(" & id_usuario_gestion & "," & numero_nivel & ",'" & nombre_nivel & "'," & id_nivel_padre & "," & _
                    "0,0,1,0," & estado_nivel_compartido & ")"
                myCommand.CommandText = Parametro_registra_nivel
                sqlresultinsert = myCommand.ExecuteNonQuery()
                If sqlresultinsert = 0 Then
                    Agregar_nivel_clasificacion = "Imposible registrar el nivel error de comando  "
                    myConnection.Close()
                    Exit Function
                End If
                last_id_nivle = myCommand.LastInsertedId
                If id_nivel_padre <> 0 Then
                    Dim Parametro_actualiza_estado_padre As String = "Update ra_pro_niveles set estado_padre=1, contador_nivel=" & numero_nivel & _
                   " where id_nivel=" & id_nivel_padre
                    myCommand.CommandText = Parametro_actualiza_estado_padre
                    sqlresultinsert = myCommand.ExecuteNonQuery()
                    If sqlresultinsert = 0 Then
                        Agregar_nivel_clasificacion = "Imposible actualizar el nivel padre  "
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                End If
                If Not stru_permiso_nivel Is Nothing Then
                    sql_permisos = "Insert into ra_pro_permisos_niveles (remit_dest_interno_id_Remit_Dest_Int,ra_pro_niveles_id_nivel" & _
                        ",carga_archivo,descarga_archivo,compartir_archivo,elimiminar_archivo,radicar_archivo,visualizar_archivo," & _
                        "editar_expediente,eliminar_expediente,agregar_expediente,cambiar_nombre_archivo,mover_expediente,copiar_archivo) values "

                    For i As Integer = 0 To stru_permiso_nivel.Length - 1
                        If i = 0 Then
                            sql_permisos = sql_permisos & " (" & stru_permiso_nivel(i).remit_dest_interno_id_Remit_Dest_Int & "," & last_id_nivle & "," & stru_permiso_nivel(i).carga_archivo & _
                            "," & stru_permiso_nivel(i).descarga_archivo & "," & stru_permiso_nivel(i).compartir_archivo & "," & stru_permiso_nivel(i).elimiminar_archivo & _
                            "," & stru_permiso_nivel(i).radicar_archivo & "," & stru_permiso_nivel(i).visualizar_archivo & "," & stru_permiso_nivel(i).editar_expediente & _
                            "," & stru_permiso_nivel(i).eliminar_expediente & "," & stru_permiso_nivel(i).agregar_expediente & "," & stru_permiso_nivel(i).cambiar_nombre_archivo & _
                            "," & stru_permiso_nivel(i).mover_expediente & "," & stru_permiso_nivel(i).copiar_archivo & ") "
                        Else
                            sql_permisos = sql_permisos & " , (" & stru_permiso_nivel(i).remit_dest_interno_id_Remit_Dest_Int & "," & last_id_nivle & "," & stru_permiso_nivel(i).carga_archivo & _
                            "," & stru_permiso_nivel(i).descarga_archivo & "," & stru_permiso_nivel(i).compartir_archivo & "," & stru_permiso_nivel(i).elimiminar_archivo & _
                            "," & stru_permiso_nivel(i).radicar_archivo & "," & stru_permiso_nivel(i).visualizar_archivo & "," & stru_permiso_nivel(i).editar_expediente & _
                            "," & stru_permiso_nivel(i).eliminar_expediente & "," & stru_permiso_nivel(i).agregar_expediente & "," & stru_permiso_nivel(i).cambiar_nombre_archivo & _
                            "," & stru_permiso_nivel(i).mover_expediente & "," & stru_permiso_nivel(i).copiar_archivo & ") "
                        End If
                    Next
                    Dim Parametro_actualiza_estado_padre As String = sql_permisos
                    myCommand.CommandText = Parametro_actualiza_estado_padre
                    sqlresultinsert = myCommand.ExecuteNonQuery()
                    If sqlresultinsert = 0 Then
                        Agregar_nivel_clasificacion = "Imposible registrar permisos  "
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                End If
                If id_nivel_padre = 0 Then
                    Dim nod As New TreeNode
                    tre_node.Value = last_id_nivle
                    tre_node.Text = nombre_nivel
                    tre_node.ImageUrl = "../Gestion/imagenes/angle-double-right-duotone.png"
                    tree.Nodes.Add(tre_node)
                Else
                    Dim nod As New TreeNode
                    nod.Value = last_id_nivle
                    nod.Text = nombre_nivel
                    If Not stru_permiso_nivel Is Nothing Then
                        nod.ImageUrl = "../Gestion/imagenes/share-light.png"
                    Else
                        nod.ImageUrl = "../Gestion/imagenes/angle-double-right-duotone.png"
                    End If
                    tre_node.ChildNodes.Add(nod)
                End If
                myTrans.Commit()
                myConnection.Close()
                update.Update()
                Agregar_nivel_clasificacion = "YES"
            Catch e As Exception
                Try
                    myTrans.Rollback()
                Catch ex As MySqlException
                    If Not myTrans.Connection Is Nothing Then
                        Agregar_nivel_clasificacion = "An exception of type " + ex.GetType().ToString() + _
                                          " was encountered while attempting to roll back the transaction."
                        Exit Function
                    End If
                End Try
                If Not myConnection Is Nothing Then
                    myConnection.Close()
                End If
                Agregar_nivel_clasificacion = "Error General " & e.Message
                Exit Function
            End Try
        Catch ex As Exception
            Agregar_nivel_clasificacion = "Inconsistencia general función Agregar_nivel_clasificacion " & ex.Message
        End Try
    End Function
    Function Agregar_nivel_clasificacion_java(ByVal id_nivel_padre As Integer, _
                                              ByVal nombre_nivel As String, _
                                              ByVal id_usuario_gestion As Integer, _
                                              ByRef Hidden_rest_tit_0006 As Object, _
                                              ByRef Hidden_rest_val_0008 As Object, _
                                              ByRef Hidden_rest_ur_0007 As Object, _
                                              ByRef last_id_nivle As Object) As String
        Try

            Dim existencia As String = ""
            Dim Result As String = ""
            Dim Ref_class_pro_nivel As New Class_ra_pro_niveles
            Dim Ref_class_permisos_usuarios As New Class_ra_pro_permisos_niveles
            Dim numero_nivel As Integer = 0
            Dim nivel_padre_raiz As Integer = 0
            Dim ref_id_nivel_padre As Integer = id_nivel_padre
            Dim sql_permisos As String = ""
            Dim stru_permiso_nivel() As stru_permiso_nivel = Nothing
            Dim Estado_propietario As String = ""
            If id_nivel_padre <> 0 Then
                Result = Ref_class_pro_nivel.Solicita_estado_nivel_propietario(id_usuario_gestion, _
                                                                               id_nivel_padre, _
                                                                               Estado_propietario)
                If Result <> "YES" Then
                    Agregar_nivel_clasificacion_java = Result
                    Exit Function
                End If
                If Estado_propietario = "NO" Then
                    Agregar_nivel_clasificacion_java = "El usuario no es propietario del nivel padre, imposible anidar otro nivel"
                    Exit Function
                End If
                Result = Ref_class_pro_nivel.Solicita_nivel_nodo(id_nivel_padre, _
                                                                 numero_nivel)
                If Result <> "YES" Then
                    Agregar_nivel_clasificacion_java = Result
                    Exit Function
                End If
                nivel_padre_raiz = Ref_class_pro_nivel.Solicita_id_nivel_padre_raiz(ref_id_nivel_padre)
                Result = Ref_class_permisos_usuarios.Solicita_matriz_datos_estrctura_permiso_nivel_usuario_gestion(ref_id_nivel_padre, _
                                                                                                                   stru_permiso_nivel)
                If Result <> "YES" Then
                    Agregar_nivel_clasificacion_java = Result
                    Exit Function
                End If

            End If
            Dim myConnection As New MySqlConnection
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            ref.Returna_Conexion_Mysql(myConnection)
            Dim myTrans As MySqlTransaction
            Dim mySqldatReader As MySqlDataReader
            Dim sqlresultinsert As Integer = 0
            Try
                'myConnection.Open()
                Dim myCommand As MySqlCommand = myConnection.CreateCommand()
                myTrans = myConnection.BeginTransaction()
                myCommand.Connection = myConnection
                myCommand.Transaction = myTrans
                Dim Parametro_Select_System1 As String = " SELECT contador_nivel FROM ra_pro_niveles " & _
                " where id_nivel=" & id_nivel_padre & " for update"
                If id_nivel_padre <> 0 Then
                    myCommand.CommandText = Parametro_Select_System1
                    mySqldatReader = myCommand.ExecuteReader()
                    If mySqldatReader Is Nothing Then
                        Agregar_nivel_clasificacion_java = "Imposible Encontrar Registro funcion Agregar_nivel_clasificacion Error Conexión"
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                    If mySqldatReader.HasRows = False Then
                        Agregar_nivel_clasificacion_java = "Imposible Encontrar el control de niveles del nivel (" & id_nivel_padre & ")"
                        myConnection.Close()
                        Exit Function
                    End If
                    mySqldatReader.Read()
                    numero_nivel = mySqldatReader.Item(0)
                    numero_nivel = numero_nivel + 1
                    mySqldatReader.Close()
                End If
                Dim estado_nivel_compartido As Integer = 0
                If Not stru_permiso_nivel Is Nothing Then
                    estado_nivel_compartido = 1
                End If
                Dim Parametro_registra_nivel As String = "insert into ra_pro_niveles  (remit_dest_interno_id_Remit_Dest_Int," & _
                    "nivel,nombre_nivel,id_nivel_padre,estado_padre,conta_expediente,estado_nivel,contador_nivel,estado_nivel_compartido) values " & _
                    "(" & id_usuario_gestion & "," & numero_nivel & ",'" & nombre_nivel & "'," & id_nivel_padre & "," & _
                    "0,0,1,0," & estado_nivel_compartido & ")"
                myCommand.CommandText = Parametro_registra_nivel
                sqlresultinsert = myCommand.ExecuteNonQuery()
                If sqlresultinsert = 0 Then
                    Agregar_nivel_clasificacion_java = "Imposible registrar el nivel error de comando  "
                    myConnection.Close()
                    Exit Function
                End If
                last_id_nivle = myCommand.LastInsertedId
                If id_nivel_padre <> 0 Then
                    Dim Parametro_actualiza_estado_padre As String = "Update ra_pro_niveles set estado_padre=1, contador_nivel=" & numero_nivel & _
                   " where id_nivel=" & id_nivel_padre
                    myCommand.CommandText = Parametro_actualiza_estado_padre
                    sqlresultinsert = myCommand.ExecuteNonQuery()
                    If sqlresultinsert = 0 Then
                        Agregar_nivel_clasificacion_java = "Imposible actualizar el nivel padre  "
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                End If
                If Not stru_permiso_nivel Is Nothing Then
                    sql_permisos = "Insert into ra_pro_permisos_niveles (remit_dest_interno_id_Remit_Dest_Int,ra_pro_niveles_id_nivel" & _
                        ",carga_archivo,descarga_archivo,compartir_archivo,elimiminar_archivo,radicar_archivo,visualizar_archivo," & _
                        "editar_expediente,eliminar_expediente,agregar_expediente,cambiar_nombre_archivo,mover_expediente,copiar_archivo) values "

                    For i As Integer = 0 To stru_permiso_nivel.Length - 1
                        If i = 0 Then
                            sql_permisos = sql_permisos & " (" & stru_permiso_nivel(i).remit_dest_interno_id_Remit_Dest_Int & "," & last_id_nivle & "," & stru_permiso_nivel(i).carga_archivo & _
                            "," & stru_permiso_nivel(i).descarga_archivo & "," & stru_permiso_nivel(i).compartir_archivo & "," & stru_permiso_nivel(i).elimiminar_archivo & _
                            "," & stru_permiso_nivel(i).radicar_archivo & "," & stru_permiso_nivel(i).visualizar_archivo & "," & stru_permiso_nivel(i).editar_expediente & _
                            "," & stru_permiso_nivel(i).eliminar_expediente & "," & stru_permiso_nivel(i).agregar_expediente & "," & stru_permiso_nivel(i).cambiar_nombre_archivo & _
                            "," & stru_permiso_nivel(i).mover_expediente & "," & stru_permiso_nivel(i).copiar_archivo & ") "
                        Else
                            sql_permisos = sql_permisos & " , (" & stru_permiso_nivel(i).remit_dest_interno_id_Remit_Dest_Int & "," & last_id_nivle & "," & stru_permiso_nivel(i).carga_archivo & _
                            "," & stru_permiso_nivel(i).descarga_archivo & "," & stru_permiso_nivel(i).compartir_archivo & "," & stru_permiso_nivel(i).elimiminar_archivo & _
                            "," & stru_permiso_nivel(i).radicar_archivo & "," & stru_permiso_nivel(i).visualizar_archivo & "," & stru_permiso_nivel(i).editar_expediente & _
                            "," & stru_permiso_nivel(i).eliminar_expediente & "," & stru_permiso_nivel(i).agregar_expediente & "," & stru_permiso_nivel(i).cambiar_nombre_archivo & _
                            "," & stru_permiso_nivel(i).mover_expediente & "," & stru_permiso_nivel(i).copiar_archivo & ") "
                        End If
                    Next
                    Dim Parametro_actualiza_estado_padre As String = sql_permisos
                    myCommand.CommandText = Parametro_actualiza_estado_padre
                    sqlresultinsert = myCommand.ExecuteNonQuery()
                    If sqlresultinsert = 0 Then
                        Agregar_nivel_clasificacion_java = "Imposible registrar permisos  "
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                End If
                If id_nivel_padre = 0 Then
                    Hidden_rest_val_0008.Value = last_id_nivle.ToString
                    Hidden_rest_tit_0006.Value = last_id_nivle.ToString & "\\"
                    Hidden_rest_ur_0007.Value = "../Gestion/imagenes/angle-double-right-duotone.png"
                Else
                    Hidden_rest_val_0008.Value = last_id_nivle.ToString
                    Hidden_rest_tit_0006.Value = last_id_nivle.ToString & "\\"
                    If Not stru_permiso_nivel Is Nothing Then
                        Hidden_rest_ur_0007.Value = "../Gestion/imagenes/share-light.png"
                    Else
                        Hidden_rest_ur_0007.Value = "../Gestion/imagenes/angle-double-right-duotone.png"
                    End If

                End If
                myTrans.Commit()
                myConnection.Close()
                'update.Update()
                Agregar_nivel_clasificacion_java = "YES"
            Catch e As Exception
                Try
                    myTrans.Rollback()
                Catch ex As MySqlException
                    If Not myTrans.Connection Is Nothing Then
                        Agregar_nivel_clasificacion_java = "An exception of type " + ex.GetType().ToString() + _
                                          " was encountered while attempting to roll back the transaction."
                        Exit Function
                    End If
                End Try
                If Not myConnection Is Nothing Then
                    myConnection.Close()
                End If
                Agregar_nivel_clasificacion_java = "Error General " & e.Message
                Exit Function
            End Try
        Catch ex As Exception
            Agregar_nivel_clasificacion_java = "Inconsistencia general función Agregar_nivel_clasificacion_java " & ex.Message
        End Try
    End Function
    Function Registra_expedientes_version_old(ByVal id_nivel As Integer, _
                                              ByVal id_usuario_gestion As Integer, _
                                              ByRef tree As TreeView, _
                                              ByRef trenode As TreeNode, _
                                              ByRef update As UpdatePanel) As String
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGaProducionDocumental
            Dim stru() As STRU_LISTA_EXP_PRDUCION = Nothing
            Dim Ref_Car_Conec33 As New conect.Dbase_Conction_Mysql_RA
            Dim sql_insert As String = ""
            Dim last As Object = Nothing
            Result = Refclas.Solicita_lista_expedientes_producion_documental(id_usuario_gestion, _
                                                                             stru)
            If Result <> "YES" Then
                Registra_expedientes_version_old = Result
                Exit Function
            End If
            If Not stru Is Nothing Then
                For i As Integer = 0 To stru.Length - 1
                    sql_insert = "Insert into ra_pro_niveles_has_expediente_archivo (ra_pro_niveles_id_nivel,expediente_archivo_ID_EXPEDIENTE) values ( " & _
                        id_nivel & "," & stru(i).ID_EXPEDIENTE & ")"
                    Result = Ref_Car_Conec33.SELECTION_LAST_INSERT_COMMAND(sql_insert, last)
                    If Result <> "YES" Then
                        Registra_expedientes_version_old = Result
                        Exit Function
                    Else
                        Dim nod As New TreeNode
                        nod.Value = last & "|" & id_nivel & "|" & stru(i).ID_EXPEDIENTE
                        nod.Text = stru(i).NOMBRE_EXPEDIENTE
                        trenode.ChildNodes.Add(nod)
                    End If
                Next
                sql_insert = "UPDATE  ra_pro_niveles SET conta_expediente= " & stru.Length & _
                      " WHERE id_nivel=" & id_nivel
                Result = Ref_Car_Conec33.SELECTION_LAST_INSERT_COMMAND(sql_insert, last)
                If Result <> "YES" Then
                    Registra_expedientes_version_old = Result
                    Exit Function
                End If
            End If
            update.Update()
            Registra_expedientes_version_old = "YES"
            Exit Function
        Catch ex As Exception
            Registra_expedientes_version_old = "Inconsistencia general función Registra_expedientes_version_old " & ex.Message
        End Try
    End Function

    Function Lista_niveles_de_organizacion(ByVal id_usuario_gestion As Integer, _
                                           ByRef tree As TreeView, _
                                           ByRef update As UpdatePanel, _
                                           ByRef stru_nivel() As stru_niveles, _
                                           ByVal nivel_compartido As Integer) As String
        Try
            Dim Result As String = ""
            Dim Class_remit_dest_int As New Class_remit_dest_interno
            Dim Ref_class_ra_pro_nivel As New Class_ra_pro_niveles_has_expediente_archivo
            Dim stru_expediente() As stru_expediente = Nothing
            Dim nodo_padre As TreeNode = Nothing
            Dim spl() As String = Nothing
            For i As Integer = 0 To stru_nivel.Length - 1
                If stru_nivel(i).estado_nivel = 1 Then
                    nodo_padre = New TreeNode
                    nodo_padre.Value = stru_nivel(i).id_nivel
                    nodo_padre.Text = stru_nivel(i).nombre_nivel
                    nodo_padre.NavigateUrl = "javascript:OnTreeNodeClicked('" & stru_nivel(i).id_nivel & "')"
                    Dim nombre_padre As String = ""
                    Dim cargo_padre As String = ""
                    If nivel_compartido = 1 Then
                        nodo_padre.ImageUrl = "../Gestion/imagenes/hand-holding-regular.png"
                        Class_remit_dest_int.Retorna_nombre_cargo_destinatario_interno(stru_nivel(i).remit_dest_interno_id_Remit_Dest_Int, _
                                                                                       nombre_padre, _
                                                                                       cargo_padre)
                        nodo_padre.ToolTip = stru_nivel(i).id_nivel & "\\Nivel compartido por " & nombre_padre & "(" & cargo_padre & ")"
                    Else
                        If stru_nivel(i).estado_nivel_compartido > 0 Then
                            nodo_padre.ImageUrl = "../Gestion/imagenes/share-light.png"
                            nodo_padre.ToolTip = stru_nivel(i).id_nivel & "\\Nivel compartido con otros usuarios "
                        Else
                            nodo_padre.ToolTip = stru_nivel(i).id_nivel & "\\"
                            nodo_padre.ImageUrl = "../Gestion/imagenes/angle-double-right-duotone.png"
                        End If
                    End If
                    stru_expediente = Nothing
                    If stru_nivel(i).conta_expediente > 0 Then
                        Result = Ref_class_ra_pro_nivel.Solicita_expedientes_relacion_nivel(stru_nivel(i).id_nivel, _
                                                                                            stru_expediente)
                        If Result <> "YES" Then
                            Lista_niveles_de_organizacion = Result
                            Exit Function
                        End If
                        If Not stru_expediente Is Nothing Then
                            For z = 0 To stru_expediente.Length - 1
                                Dim node_expediente As New TreeNode(stru_expediente(z).expediente_archivo_ID_EXPEDIENTE)
                                node_expediente.Text = stru_expediente(z).ALEAS_EXPEDIENTE
                                node_expediente.Value = stru_expediente(z).id_registro & "|" & stru_expediente(z).ra_pro_niveles_id_nivel & _
                                    "|" & stru_expediente(z).expediente_archivo_ID_EXPEDIENTE
                                Dim value_node As String = stru_expediente(z).id_registro & "|" & stru_expediente(z).ra_pro_niveles_id_nivel & _
                                    "|" & stru_expediente(z).expediente_archivo_ID_EXPEDIENTE & "|"
                                node_expediente.ImageUrl = "../Gestion/imagenes/folder-regular.png"
                                node_expediente.NavigateUrl = "javascript:OnTreeNodeClicked('" & value_node & "')"
                                node_expediente.ToolTip = value_node
                                'node_expediente.NavigateUrl = value_node
                                nodo_padre.ChildNodes.Add(node_expediente)

                            Next

                        End If
                    End If
                    If stru_nivel(i).nivel = 0 Then
                        tree.Nodes.Add(nodo_padre)
                        stru_nivel(i).value_path = nodo_padre.ValuePath
                    Else
                        Dim tre_node As TreeNode = Nothing
                        Dim value_path As String = ""
                        Solicita_valor_path(stru_nivel, _
                                            stru_nivel(i).id_nivel_padre, _
                                            value_path)
                        tre_node = tree.FindNode(value_path)
                        If tre_node Is Nothing Then
                            tree.Nodes.Add(nodo_padre)
                            stru_nivel(i).value_path = nodo_padre.ValuePath
                        Else
                            tre_node.ChildNodes.Add(nodo_padre)
                            stru_nivel(i).value_path = nodo_padre.ValuePath
                        End If
                    End If
                End If
            Next
            update.Update()
            Lista_niveles_de_organizacion = "YES"
            Exit Function
        Catch ex As Exception
            Lista_niveles_de_organizacion = "Inconsistencia general función Lista_niveles_de_organizacion " & ex.Message
        End Try
    End Function
    Function Solicita_valor_path(ByVal stru_nivel() As stru_niveles, _
                                 ByVal id_nivel_padre As Integer, _
                                 ByRef value_path As String) As String
        Try
            For i As Integer = 0 To stru_nivel.Length - 1
                If stru_nivel(i).id_nivel = id_nivel_padre Then
                    value_path = stru_nivel(i).value_path
                    Solicita_valor_path = "YES"
                    Exit For
                End If
            Next
            Solicita_valor_path = "Imposible encontrar el valor"
        Catch ex As Exception
            Solicita_valor_path = "Inconsistencia general funcion Solicita_valor_path " & ex.Message
        End Try
    End Function

    Function Busca_nodo_padre(ByVal id_nivel_padre As Integer, _
                              ByVal tree As TreeView, _
                              ByRef nod As TreeNode) As String
        Try
            nod = tree.FindNode(id_nivel_padre.ToString)
            Busca_nodo_padre = "YES"
            Exit Function
        Catch ex As Exception
            Busca_nodo_padre = "Inconsistencia función Busca_nodo_padre " & ex.Message
        End Try
    End Function
    Function Eliminar_nivel_organizacion_expediente(ByVal id_usuario_gestion As Integer, _
                                                    ByVal id_nivel As Integer, _
                                                    ByRef tree As TreeView, _
                                                    ByRef update As UpdatePanel) As String
        Dim Result As String = ""
        Dim Rerclas As New Class_ra_pro_niveles
        Dim Estado_propietario As String = ""
        Result = Rerclas.Solicita_estado_nivel_propietario(id_usuario_gestion, _
                                                           id_nivel, _
                                                           Estado_propietario)
        If Result <> "YES" Then
            Eliminar_nivel_organizacion_expediente = Result
            Exit Function
        End If
        If Estado_propietario = "NO" Then
            Eliminar_nivel_organizacion_expediente = "Usted no es propietario de la estructura imposible eliminar"
            Exit Function
        End If
        Dim Stru_niveles() As stru_niveles = Nothing
        Result = Rerclas.Solicita_niveles_hijos(id_nivel, _
                                                Stru_niveles)
        If Result <> "YES" Then
            Eliminar_nivel_organizacion_expediente = Result
            Exit Function
        End If
        If Not Stru_niveles Is Nothing Then
            Eliminar_nivel_organizacion_expediente = "El nivel tiene niveles inferiores relacionados imposible eliminar"
            Exit Function
        End If
        Dim Ref_class_per As New Class_ra_pro_niveles_has_expediente_archivo
        Dim stru_expediente() As stru_expediente = Nothing
        Result = Ref_class_per.Solicita_expedientes_relacion_nivel(id_nivel, _
                                                                 stru_expediente)
        If Result <> "YES" Then
            Eliminar_nivel_organizacion_expediente = Result
            Exit Function
        End If
        If Not stru_expediente Is Nothing Then
            Eliminar_nivel_organizacion_expediente = "El nivel tiene expedientes  relacionados  imposible eliminar"
            Exit Function
        End If
        Dim numero_permisos As Integer = 0
        Dim Ref_class_per_nivel As New Class_ra_pro_permisos_niveles
        Result = Ref_class_per_nivel.Solicita_numero_permisos_nivel(id_nivel, _
                                                                   numero_permisos)
        If Result <> "YES" Then
            Eliminar_nivel_organizacion_expediente = Result
            Exit Function
        End If
        'If numero_permisos <> 0 Then
        '    Eliminar_nivel_organizacion_expediente = "El nivel que quiere eliminar esta compartido con otros usuarios, imposible eliminar"
        '    Exit Function
        'End If
        Dim id_nivel_padre As Integer = 0
        Dim numero_nivel As Integer = 0
        Dim estado_padre As Integer = 0
        Result = Rerclas.Solicita_id_nivel_padre(id_nivel, _
                                                id_nivel_padre)

        If Result <> "YES" Then
            Eliminar_nivel_organizacion_expediente = Result
            Exit Function
        End If
        If id_nivel_padre <> 0 Then
            Result = Rerclas.Solicita_numero_niveles_hijos(id_nivel_padre, _
                                                           numero_nivel)
            If Result <> "YES" Then
                Eliminar_nivel_organizacion_expediente = Result
                Exit Function
            End If
        End If
        If numero_nivel > 0 Then
            numero_nivel = numero_nivel - 1
        End If
        If numero_nivel = 0 Then
            estado_padre = 0
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        'Dim mySqldatReader As MySqlDataReader
        Dim sqlresultinsert As Integer = 0
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            Dim Paramentro_elimina_registro_permiso As String = "Delete from ra_pro_permisos_niveles where ra_pro_niveles_id_nivel=" & id_nivel
            myCommand.CommandText = Paramentro_elimina_registro_permiso
            If numero_permisos <> 0 Then
                sqlresultinsert = myCommand.ExecuteNonQuery()
                If sqlresultinsert = 0 Then
                    Eliminar_nivel_organizacion_expediente = "Imposible eliminar los registros de permisos  "
                    myConnection.Close()
                    Exit Function
                End If
            End If
            Dim Parametro_elimina_registro_nivel As String = "Delete from ra_pro_niveles where id_nivel=" & id_nivel
            myCommand.CommandText = Parametro_elimina_registro_nivel
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Eliminar_nivel_organizacion_expediente = "Imposible eliminar el nivel del registro  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If id_nivel_padre <> 0 Then
                Dim Parametro_actualiza_estado_padre As String = "Update ra_pro_niveles set estado_padre=" & estado_padre & _
               " where id_nivel=" & id_nivel_padre
                myCommand.CommandText = Parametro_actualiza_estado_padre
                sqlresultinsert = myCommand.ExecuteNonQuery()
                If sqlresultinsert = 0 Then
                    Eliminar_nivel_organizacion_expediente = "Imposible actualizar el nivel padre  "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            'If id_nivel_padre = 0 Then
            '    tree.Nodes.Remove(tree.SelectedNode)
            'Else
            '    'tree.Nodes.Remove(tree.SelectedNode)
            '    Dim sNodo As TreeNode = tree.SelectedNode
            '    Dim pNodo As TreeNode = sNodo.Parent
            '    pNodo.ChildNodes.Remove(sNodo)
            'End If
            myTrans.Commit()
            myConnection.Close()
            'update.Update()
            Eliminar_nivel_organizacion_expediente = "YES"
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Eliminar_nivel_organizacion_expediente = "An exception of type " + ex.GetType().ToString() + _
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Eliminar_nivel_organizacion_expediente = "Error General " & e.Message
            Exit Function
        End Try
    End Function
    Function Editar_nivel_de_organizacion(ByVal id_usuario_gestion As Integer, _
                                          ByVal id_nivel As Integer, _
                                          ByVal nombre_nivel As String, _
                                          ByRef tree As TreeView, _
                                          ByRef updat As UpdatePanel) As String
        Try
            Dim Result As String = ""
            Dim Rerclas As New Class_ra_pro_niveles
            Dim Estado_propietario As String = ""
            Result = Rerclas.Solicita_estado_nivel_propietario(id_usuario_gestion, _
                                                               id_nivel, _
                                                               Estado_propietario)
            If Result <> "YES" Then
                Editar_nivel_de_organizacion = Result
                Exit Function
            End If
            If Estado_propietario = "NO" Then
                Editar_nivel_de_organizacion = "Usted no es propietario de la estructura imposible editar"
                Exit Function
            End If
            Dim Parametro_actualiza_estado_padre As String = "Update ra_pro_niveles set nombre_nivel='" & nombre_nivel & "'" & _
            " where id_nivel=" & id_nivel
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Result = ref.SELECTION_INSERT_COMMAND(Parametro_actualiza_estado_padre)
            If Result <> "YES" Then
                Editar_nivel_de_organizacion = Result
                Exit Function
            Else
                HttpContext.Current.Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION_TEXT") = nombre_nivel
                Editar_nivel_de_organizacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Editar_nivel_de_organizacion = "Inconsistencia general función Editar_nivel_de_organizacion " & ex.Message
        End Try
    End Function
    Function Cambia_estado_nivel_organizacion_usuario_gestion(ByVal id_usuario_gestion As Integer, _
                                                              ByVal id_nivel As Integer, _
                                                              ByVal estado_nivel As Integer, _
                                                              ByRef stru_niveles_hijo_() As stru_niveles_hijo, _
                                                              ByRef numero_expedientes As Integer, _
                                                              ByRef numero_niveles As Integer) As String

        Dim Result As String = ""
        Dim Refclas As New Class_ra_pro_niveles
        Dim Refcla_class_permisos_nivel As New Class_ra_pro_permisos_niveles
        Dim Estado_propietario_nivel As String = ""
        Dim stru_permiso_nivel As stru_permiso_nivel = Nothing
        Result = Refclas.Solicita_estado_nivel_propietario(id_usuario_gestion, _
                                                           id_nivel, _
                                                           Estado_propietario_nivel)
        If Result <> "YES" Then
            Cambia_estado_nivel_organizacion_usuario_gestion = Result
            Exit Function
        End If
        If Estado_propietario_nivel = "NO" Then
            Cambia_estado_nivel_organizacion_usuario_gestion = "El usuario no es el propietario del nivel imposible compartir"
            Exit Function
        End If
        Result = Refclas.Solicita_numero_expediente_nivel(id_nivel, _
                                                          numero_expedientes)
        If Result <> "YES" Then
            Cambia_estado_nivel_organizacion_usuario_gestion = Result
            Exit Function
        End If
        Result = Refclas.Solicita_niveles_relacionados_padre_recursive(id_nivel, _
                                                                       stru_niveles_hijo_, _
                                                                       id_usuario_gestion)
        If Result <> "YES" Then
            Cambia_estado_nivel_organizacion_usuario_gestion = Result
            Exit Function
        End If
        numero_niveles = 1
        If Not stru_niveles_hijo_ Is Nothing Then
            For i As Integer = 0 To stru_niveles_hijo_.Length - 1
                numero_niveles = numero_niveles + 1
                If stru_niveles_hijo_(i).estado_repetido = 0 Then
                    numero_expedientes = numero_expedientes + stru_niveles_hijo_(i).numero_expediente
                End If
            Next
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim sqlresultinsert As Integer = 0
        Dim Parametro_actualiza_estado_nivel As String = ""
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            If stru_niveles_hijo_ Is Nothing Then
                Parametro_actualiza_estado_nivel = "update ra_pro_niveles set estado_nivel=" & estado_nivel & ", estado_nivel_oculto_padre=" & estado_nivel & _
                    " where id_nivel=" & id_nivel
            Else
                Parametro_actualiza_estado_nivel = "update ra_pro_niveles set estado_nivel=" & estado_nivel & ", estado_nivel_oculto_padre=" & estado_nivel & _
                      " where id_nivel=" & id_nivel
                For i As Integer = 0 To stru_niveles_hijo_.Length - 1
                    If stru_niveles_hijo_(i).estado_repetido = 0 Then
                        Parametro_actualiza_estado_nivel = Parametro_actualiza_estado_nivel & " ; " & " update ra_pro_niveles set estado_nivel=" & estado_nivel & _
                     " where id_nivel=" & stru_niveles_hijo_(i).id_nivel
                    End If
                Next
            End If
            myCommand.CommandText = Parametro_actualiza_estado_nivel
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Cambia_estado_nivel_organizacion_usuario_gestion = "Imposible cambiar el estado del nivel "
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            Cambia_estado_nivel_organizacion_usuario_gestion = "YES"
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Cambia_estado_nivel_organizacion_usuario_gestion = "An exception of type " + ex.GetType().ToString() + _
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Cambia_estado_nivel_organizacion_usuario_gestion = "Error General " & e.Message
            Exit Function
        End Try
    End Function
    Function Compartir_nivel_organizacion_usuario_gestion(ByVal id_usuario_gestion As Integer, _
                                                          ByVal id_usuario_gestion_compartir As Integer, _
                                                          ByVal id_nivel As Integer, _
                                                          ByVal stru_permiso_nivel As stru_permiso_nivel) As String
        Dim Result As String = ""
        Dim Refclas As New Class_ra_pro_niveles
        Dim Refcla_class_permisos_nivel As New Class_ra_pro_permisos_niveles
        Dim Estado_propietario_nivel As String = ""
        Result = Refclas.Solicita_estado_nivel_propietario(id_usuario_gestion, _
                                                           id_nivel, _
                                                           Estado_propietario_nivel)
        If Result <> "YES" Then
            Compartir_nivel_organizacion_usuario_gestion = Result
            Exit Function
        End If
        If Estado_propietario_nivel = "NO" Then
            Compartir_nivel_organizacion_usuario_gestion = "El usuario no es el propietario del nivel imposible compartir"
            Exit Function
        End If
        If id_usuario_gestion_compartir = 0 Then
            Compartir_nivel_organizacion_usuario_gestion = "Debe informar el usuario a compartir "
            Exit Function
        End If
        'Dim split_id_usuario() As String = usuario_compartir.Split("-")
        'Dim stru_user() As stru_usuario_gestion_compartido = Nothing
        'Dim refclas_compartir As New ClassGaCompartirDocumento
        'Result = refclas_compartir.Retorna_matriz_id_usuarios_gestion(usuario_compartir, _
        '                                                              stru_user)
        'If Result <> "YES" Then
        '    Compartir_nivel_organizacion_usuario_gestion = Result
        '    Exit Function
        'End If
        'If stru_user Is Nothing Then
        '    Compartir_nivel_organizacion_usuario_gestion = "Imposible encontrar la estructura del usuario " & usuario_compartir
        '    Exit Function
        'End If
        Dim Estado_nivel_compartido_usuario As String = ""
        Result = Refcla_class_permisos_nivel.Solicita_existencia_permiso_nivel(id_usuario_gestion_compartir, _
                                                                               id_nivel, _
                                                                               Estado_nivel_compartido_usuario)
        If Result <> "YES" Then
            Compartir_nivel_organizacion_usuario_gestion = Result
            Exit Function
        End If
        If Estado_nivel_compartido_usuario = "YES" Then
            Compartir_nivel_organizacion_usuario_gestion = "El usuario informado ya tiene compartido este nivel, imposible continuar"
            Exit Function
        End If
        If id_usuario_gestion_compartir = id_usuario_gestion Then
            Compartir_nivel_organizacion_usuario_gestion = "El usuario  es el propietario del nivel imposible compartir"
            Exit Function
        End If
        Dim stru_niveles_hijo_() As stru_niveles_hijo = Nothing
        Result = Refclas.Solicita_niveles_relacionados_padre_recursive(id_nivel, _
                                                                       stru_niveles_hijo_, _
                                                                       id_usuario_gestion_compartir)
        If Result <> "YES" Then
            Compartir_nivel_organizacion_usuario_gestion = Result
            Exit Function
        End If
        Dim numero_compartido As Integer = 0
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim mySqldatReader As MySqlDataReader
        Dim sqlresultinsert As Integer = 0
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            Dim Parametro_Select_System1 As String = " SELECT estado_nivel_compartido FROM ra_pro_niveles " & _
            " where id_nivel=" & id_nivel & " for update"
            myCommand.CommandText = Parametro_Select_System1
            mySqldatReader = myCommand.ExecuteReader()
            If mySqldatReader Is Nothing Then
                Compartir_nivel_organizacion_usuario_gestion = "Imposible Encontrar Registro funcion Compartir_nivel_organizacion_usuario_gestion Error Conexión"
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If mySqldatReader.HasRows = False Then
                Compartir_nivel_organizacion_usuario_gestion = "Imposible Encontrar el control de niveles del nivel (" & id_nivel & ")"
                myConnection.Close()
                Exit Function
            End If
            mySqldatReader.Read()
            numero_compartido = mySqldatReader.Item(0)
            numero_compartido = numero_compartido + 1
            mySqldatReader.Close()
            Dim Parametro_registra_nivel As String = ""
            If stru_niveles_hijo_ Is Nothing Then
                Parametro_registra_nivel = "insert into ra_pro_permisos_niveles  (remit_dest_interno_id_Remit_Dest_Int," & _
               "ra_pro_niveles_id_nivel,carga_archivo,descarga_archivo,compartir_archivo,elimiminar_archivo,radicar_archivo," & _
               "visualizar_archivo,editar_expediente,eliminar_expediente,agregar_expediente,cambiar_nombre_archivo,mover_expediente,copiar_archivo) values " & _
               "(" & id_usuario_gestion_compartir & "," & id_nivel & "," & stru_permiso_nivel.carga_archivo & "," & stru_permiso_nivel.descarga_archivo & "," & _
               stru_permiso_nivel.compartir_archivo & "," & stru_permiso_nivel.elimiminar_archivo & "," & stru_permiso_nivel.radicar_archivo & _
               "," & stru_permiso_nivel.visualizar_archivo & "," & stru_permiso_nivel.editar_expediente & "," & stru_permiso_nivel.eliminar_expediente & _
               "," & stru_permiso_nivel.agregar_expediente & "," & stru_permiso_nivel.cambiar_nombre_archivo & "," & stru_permiso_nivel.mover_expediente & _
                "," & stru_permiso_nivel.copiar_archivo & ")"
            Else
                Parametro_registra_nivel = "insert into ra_pro_permisos_niveles  (remit_dest_interno_id_Remit_Dest_Int," & _
              "ra_pro_niveles_id_nivel,carga_archivo,descarga_archivo,compartir_archivo,elimiminar_archivo,radicar_archivo," & _
              "visualizar_archivo,editar_expediente,eliminar_expediente,agregar_expediente,cambiar_nombre_archivo,mover_expediente,copiar_archivo) values "
                Parametro_registra_nivel = Parametro_registra_nivel & "(" & id_usuario_gestion_compartir & "," & id_nivel & "," & stru_permiso_nivel.carga_archivo & "," & stru_permiso_nivel.descarga_archivo & "," & _
                           stru_permiso_nivel.compartir_archivo & "," & stru_permiso_nivel.elimiminar_archivo & "," & stru_permiso_nivel.radicar_archivo & _
                           "," & stru_permiso_nivel.visualizar_archivo & "," & stru_permiso_nivel.editar_expediente & "," & stru_permiso_nivel.eliminar_expediente & _
                           "," & stru_permiso_nivel.agregar_expediente & "," & stru_permiso_nivel.cambiar_nombre_archivo & "," & stru_permiso_nivel.mover_expediente & _
                           "," & stru_permiso_nivel.copiar_archivo & ")"
                Dim detecta As Integer = 0
                For i As Integer = 0 To stru_niveles_hijo_.Length - 1
                    If stru_niveles_hijo_(i).estado_repetido = 0 Then
                        Parametro_registra_nivel = Parametro_registra_nivel & " , (" & id_usuario_gestion_compartir & "," & stru_niveles_hijo_(i).id_nivel & "," & stru_permiso_nivel.carga_archivo & "," & stru_permiso_nivel.descarga_archivo & "," & _
                        stru_permiso_nivel.compartir_archivo & "," & stru_permiso_nivel.elimiminar_archivo & "," & stru_permiso_nivel.radicar_archivo & _
                        "," & stru_permiso_nivel.visualizar_archivo & "," & stru_permiso_nivel.editar_expediente & "," & stru_permiso_nivel.eliminar_expediente & _
                        "," & stru_permiso_nivel.agregar_expediente & "," & stru_permiso_nivel.cambiar_nombre_archivo & "," & stru_permiso_nivel.mover_expediente & _
                        "," & stru_permiso_nivel.copiar_archivo & ")"
                    End If
                Next
            End If
            myCommand.CommandText = Parametro_registra_nivel
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Compartir_nivel_organizacion_usuario_gestion = "Imposible compartir el nivel "
                myConnection.Close()
                Exit Function
            End If
            Dim Parametro_actualiza_estado_padre As String = "Update ra_pro_niveles set estado_nivel_compartido=" & numero_compartido & _
           " where id_nivel=" & id_nivel
            myCommand.CommandText = Parametro_actualiza_estado_padre
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Compartir_nivel_organizacion_usuario_gestion = "Imposible actualizar numero nivel compartido  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            Compartir_nivel_organizacion_usuario_gestion = "YES"
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Compartir_nivel_organizacion_usuario_gestion = "An exception of type " + ex.GetType().ToString() + _
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Compartir_nivel_organizacion_usuario_gestion = "Error General " & e.Message
            Exit Function
        End Try

    End Function
    Function Dejar_de_compartir_nivel_organizacion_usuario_gestion(ByVal id_nivel As Integer, _
                                                                   ByVal id_registro_permiso As Integer, _
                                                                   ByRef tre_node As TreeNode, _
                                                                   ByRef update As UpdatePanel, _
                                                                   ByRef label_eatado_lista As Label, _
                                                                   ByRef update_estado As UpdatePanel, _
                                                                   ByVal id_usuario_gestion As Integer, _
                                                                   ByRef Hidden_rest_ur_permiso_elimina_0007 As Object) As String
        Dim numero_compartido As Integer = 0
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim mySqldatReader As MySqlDataReader
        Dim sqlresultinsert As Integer = 0
        Dim Refclas_ra_pro_permisos As New Class_ra_pro_permisos_niveles
        Dim Result As String = ""
        Result = Refclas_ra_pro_permisos.Solicita_numero_permisos_nivel(id_nivel, _
                                                                        numero_compartido)
        If Result <> "YES" Then
            Dejar_de_compartir_nivel_organizacion_usuario_gestion = Result
            Exit Function
        End If
        Dim id_usuario_permiso As Integer = 0
        Result = Refclas_ra_pro_permisos.Solicita_id_usuario_id_permiso(id_registro_permiso, _
                                                                       id_usuario_permiso)
        If Result <> "YES" Then
            Dejar_de_compartir_nivel_organizacion_usuario_gestion = Result
            Exit Function
        End If
        Dim Refclas As New Class_ra_pro_niveles
        Dim stru_niveles_hijo_() As stru_niveles_hijo = Nothing
        Result = Refclas.Solicita_niveles_relacionados_padre_recursive(id_nivel, _
                                                                       stru_niveles_hijo_, _
                                                                       id_usuario_permiso)
        If Result <> "YES" Then
            Dejar_de_compartir_nivel_organizacion_usuario_gestion = Result
            Exit Function
        End If
        Try
            'myConnection.Open()
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            Dim Parametro_Select_System1 As String = " SELECT estado_nivel_compartido FROM ra_pro_niveles " & _
            " where id_nivel=" & id_nivel & " for update"
            myCommand.CommandText = Parametro_Select_System1
            mySqldatReader = myCommand.ExecuteReader()
            If mySqldatReader Is Nothing Then
                Dejar_de_compartir_nivel_organizacion_usuario_gestion = "Imposible Encontrar Registro funcion Compartir_nivel_organizacion_usuario_gestion Error Conexión"
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If mySqldatReader.HasRows = False Then
                Dejar_de_compartir_nivel_organizacion_usuario_gestion = "Imposible Encontrar el control de niveles del nivel (" & id_nivel & ")"
                myConnection.Close()
                Exit Function
            End If
            mySqldatReader.Read()
            'numero_compartido = mySqldatReader.Item(0)
            If numero_compartido > 0 Then
                numero_compartido = numero_compartido - 1
            End If
            mySqldatReader.Close()
            Dim Parametro_elimina_permiso As String = "Delete from ra_pro_permisos_niveles  " & _
                " where id_permisos_niveles=" & id_registro_permiso
            Dim paremetro_elimina As String = ""
            If Not stru_niveles_hijo_ Is Nothing Then
                For i As Integer = 0 To stru_niveles_hijo_.Length - 1
                    If stru_niveles_hijo_(i).estado_repetido <> 0 Then
                        Parametro_elimina_permiso = Parametro_elimina_permiso & "; Delete from ra_pro_permisos_niveles  " & _
                          " where id_permisos_niveles=" & stru_niveles_hijo_(i).estado_repetido
                    End If
                Next
            End If
            myCommand.CommandText = Parametro_elimina_permiso
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Dejar_de_compartir_nivel_organizacion_usuario_gestion = "Imposible compartir el nivel "
                myConnection.Close()
                Exit Function
            End If
            Dim Parametro_actualiza_estado_padre As String = "Update ra_pro_niveles set estado_nivel_compartido=" & numero_compartido & _
           " where id_nivel=" & id_nivel
            myCommand.CommandText = Parametro_actualiza_estado_padre
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Dejar_de_compartir_nivel_organizacion_usuario_gestion = "Imposible actualizar numero nivel compartido  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Hidden_rest_ur_permiso_elimina_0007.value = ""
            If numero_compartido = 0 Then
                'tre_node.ImageUrl = "../Gestion/imagenes/angle-double-right-duotone.png"
                'tre_node.ToolTip = ""
                Hidden_rest_ur_permiso_elimina_0007.value = "../Gestion/imagenes/angle-double-right-duotone.png"
            End If
            label_eatado_lista.Text = "Se encontraron " & numero_compartido & "  registro(s) "
            myTrans.Commit()
            myConnection.Close()
            'update.Update()
            update_estado.Update()
            Dejar_de_compartir_nivel_organizacion_usuario_gestion = "YES"
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Dejar_de_compartir_nivel_organizacion_usuario_gestion = "An exception of type " + ex.GetType().ToString() + _
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Dejar_de_compartir_nivel_organizacion_usuario_gestion = "Error General " & e.Message
            Exit Function
        End Try
    End Function
    Function Traslado_carpeta_nivel(ByVal id_usuario_gestion As Integer, _
                                    ByVal id_nivel_fuente As Integer, _
                                    ByVal id_nivel_destino As Integer, _
                                    ByVal id_expediente As Integer, _
                                    ByVal id_registro_relacion_fuente As Integer, _
                                    ByRef Hidden_rest_expe_tit_0009 As String) As String
        Dim Result As String = ""
        Dim Ref_class As New Class_ra_pro_niveles
        Dim Estado_propietario As String = ""
        Result = Ref_class.Solicita_estado_nivel_propietario(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                             id_nivel_destino, _
                                                             Estado_propietario)
        If Result <> "YES" Then
            Traslado_carpeta_nivel = Result
            Exit Function
        End If
        If Estado_propietario = "NO" Then
            Traslado_carpeta_nivel = "Usted no es propietario del nivel para  mover el expediente seleccionado"
            Exit Function
        End If
        If id_nivel_fuente = id_nivel_destino Then
            Traslado_carpeta_nivel = "Esta intentando mover el expediente al mismo nivel"
            Exit Function
        End If
        Dim Estado_registro_nivel As String = ""
        Dim Ref_class_pro_niveles As New Class_ra_pro_niveles_has_expediente_archivo
        Result = Ref_class_pro_niveles.Solicita_relacion_nivel_expediente(id_nivel_destino, _
                                                                           id_expediente, _
                                                                           Estado_registro_nivel)
        If Result <> "YES" Then
            Traslado_carpeta_nivel = Result
            Exit Function
        End If
        If Estado_registro_nivel = "YES" Then
            Traslado_carpeta_nivel = "Esta intentando registrar el expediente al mismo nivel, por favor actualice la página "
            Exit Function
        End If
        Dim id_registro_relacion_copia As Integer = 0
        Dim sqlinsertcion As String = ""
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim consecutivo_unidad As String = 0
        Dim consecutivo_exp_nivel As Integer = 1
        Dim errorM As String = "YES"
        Dim Switc As Integer = 0
        Try

            Dim sqlforupdate As String = ""
            Dim dat_reader As MySqlDataReader
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = sqlforupdate
            sqlforupdate = "Select conta_expediente  from ra_pro_niveles where id_nivel=" & id_nivel_destino & " for update "
            myCommand.CommandText = sqlforupdate
            dat_reader = myCommand.ExecuteReader()
            If dat_reader Is Nothing Then
                Traslado_carpeta_nivel = "Imposible Encontrar el nivel del expediente error de conexión"
                errorM = "Imposible Encontrar consecutivo de expedientes del nivel"
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If dat_reader.HasRows = False Then
                Traslado_carpeta_nivel = "Imposible Encontrar consecutivo de expedientes del nivel"
                errorM = "Imposible Encontrar consecutivo de expedientes del nivel"
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If dat_reader.HasRows = True Then
                dat_reader.Read()
                consecutivo_exp_nivel = dat_reader.Item(0)
                consecutivo_exp_nivel = consecutivo_exp_nivel + 1
                dat_reader.Close()
            End If
            Dim sql_consecutivo_nivel As String = "Update ra_pro_niveles set conta_expediente=" & consecutivo_exp_nivel & "  where id_nivel=" & id_nivel_destino
            myCommand.CommandText = sql_consecutivo_nivel
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                errorM = "Imposible actualizar el consecutivo del nivel de expedientes "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Dim sql_insert_relacion As String = "Insert into ra_pro_niveles_has_expediente_archivo (ra_pro_niveles_id_nivel," & _
                "expediente_archivo_ID_EXPEDIENTE) values (" & id_nivel_destino & "," & id_expediente & ")"
            myCommand.CommandText = sql_insert_relacion
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                errorM = "Imposible registrar la relación con el nivel "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            id_registro_relacion_copia = myCommand.LastInsertedId
            Dim sql_delete_nivel As String = "delete from ra_pro_niveles_has_expediente_archivo  where id_registro=" & id_registro_relacion_fuente
            myCommand.CommandText = sql_delete_nivel
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                errorM = "Imposible elimniar la relación con el nivel "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Hidden_rest_expe_tit_0009 = id_registro_relacion_copia & "|" & id_nivel_destino & "|" & id_expediente
            myTrans.Commit()
            myConnection.Close()
            errorM = "YES"
            Traslado_carpeta_nivel = "YES"
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                'myTrans.Rollback()
                myConnection.Close()
                Traslado_carpeta_nivel = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            If errorM <> "YES" Then
                Traslado_carpeta_nivel = errorM + sqlinsertcion
            Else
                Traslado_carpeta_nivel = errorM
            End If

        End Try

    End Function
    Function Auto_expand(ByRef node_ As TreeNode) As String
        Try
            node_.Expand()
            If Not node_.Parent Is Nothing Then
                Auto_expand(node_.Parent)
            End If
            Auto_expand = "YES"
        Catch ex As Exception
            Auto_expand = "Inconsistencia función  Auto_expand " & ex.Message
        End Try

    End Function



End Class
