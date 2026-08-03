Imports MySql.Data.MySqlClient
Imports System.IO
Imports GestionDocumental_Docuarchi.net.conect.Dbase_Conction_Mysql_DA
Imports System.Xml

Public Structure Stru_config_digitalizacion
    Dim ID_RA_CONFIG As Integer
    Dim tipo_doc_entrante_id_Tipo_Doc_Entrante As Integer
    Dim tipo_doc_saliente_id_Tip_Doc_Saliente As Integer
    Dim OBLIGA_LISTA_CHEQUEO As Integer
    Dim TIPO_DIGITALIZACION As Integer
    Dim TIPO_ARCHIVO_DIGITALIZA As String
    Dim ACTIVA_OCR As Integer
End Structure

Public Class ClassWorkflowDigitalizacion
    Inherits conect.Dbase_Conction_Mysql_DA
    Function Solicita_id_imagen_en_gabinete_por_radicado(ByVal nombre_gabinete As String, _
                                                         ByVal nombre_Campo_radicado As String, _
                                                         ByVal radicado As String, _
                                                         ByRef id_imagen As Long) As String
        Try
            Dim Sql_consulta As String = "select  ID  from " & nombre_gabinete & "  where " & nombre_Campo_radicado & " ='" & _
                radicado & "' and ENLASE='" & radicado & "'"
            Dim Numero_Imagenesl As Integer = 0
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet(nombre_gabinete)
            Dim result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If result <> "YES" Then
                Solicita_id_imagen_en_gabinete_por_radicado = "Error función Solicita_id_imagen_en_gabinete_por_radicado  " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_imagen = 0
                Solicita_id_imagen_en_gabinete_por_radicado = "YES"
                Exit Function
            Else
                id_imagen = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_imagen_en_gabinete_por_radicado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_imagen_en_gabinete_por_radicado = "Incosistencia general función Solicita_id_imagen_en_gabinete_por_radicado " & ex.Message
        End Try
    End Function

    Function Actualiza_a_Documento_Principal_treview(ByRef Updadat As UpdatePanel, _
                                                     ByRef treview As TreeView, _
                                                     ByVal Nombre_Gabinete As String,
                                                     ByVal Enlace_Radic As String, _
                                                     ByVal id_ruta As Integer, _
                                                     ByVal id_tarea As Long) As String


        Dim nombre_campo As String = ""
        Dim Result As String = ""
        Dim Ref_Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
        Result = Ref_Class_DETALLE_GABIENETE.SolicitaNombreCampoRadicadoGabinete(Nombre_Gabinete,
                                                                                 nombre_campo)
        If Result <> "YES" Then
            Actualiza_a_Documento_Principal_treview = Result
            Exit Function
        End If
        If nombre_campo = "" Then
            Actualiza_a_Documento_Principal_treview = "Por favor active el campo radicado en el gabinete (" & Nombre_Gabinete & ")"
            Exit Function
        End If
        Dim Refclas_digitalizacion As New Classselecciotarea
        Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
        Dim id_imagen As Long = 0
        Result = Class_DAT_ADIC_TAR.SolicitaIdImagenRelacionadaTareaworkflowIdRuta(id_ruta,
                                                                                   id_tarea,
                                                                                   id_imagen)
        If Result <> "YES" Then
            Actualiza_a_Documento_Principal_treview = Result
            Exit Function
        End If
        If id_imagen = 0 Then
            Actualiza_a_Documento_Principal_treview = "La tarea (" & id_tarea & ") no tiene asignado un documento principal en el flujo de trabajo, imposible activar como documento principal"
            Exit Function
        End If
        If treview.SelectedNode Is Nothing Then
            Actualiza_a_Documento_Principal_treview = "Imposible encontrar documentos para actualizar"
            Exit Function
        End If
        Dim spliitem() As String = treview.SelectedNode.Value.Split("|")
        Dim Idseleccion As Long = spliitem(1)
        Dim Matri_id() As String
        Erase Matri_id
        Dim SqlUpdate As String = ""
        For i As Integer = 0 To treview.Nodes.Count - 1
            Dim spli() As String = treview.Nodes(i).Value.Split("|")
            ReDim Preserve Matri_id(i)
            Matri_id(i) = spli(0)
            If i = 0 Then
                If spli(1) = spliitem(1) Then
                    SqlUpdate = "Update " & Nombre_Gabinete & " set " & nombre_campo & "='" & Enlace_Radic & "' where id=" & spli(1)
                Else
                    SqlUpdate = "Update " & Nombre_Gabinete & " set " & nombre_campo & "=null where id=" & spli(1)
                End If
            Else
                If spli(1) = spliitem(1) Then
                    SqlUpdate = SqlUpdate & "  ;  " & vbCrLf & "Update " & Nombre_Gabinete & " set " & nombre_campo & "='" & Enlace_Radic & "' where id=" & spli(1)
                Else
                    SqlUpdate = SqlUpdate & "  ;  " & vbCrLf & "Update " & Nombre_Gabinete & " set " & nombre_campo & "=null where id=" & spli(1)
                End If

            End If
        Next

        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_DA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Try
            Dim Refclassselecion As New Classselecciotarea
            If id_imagen <> 0 Then
                Result = Class_DAT_ADIC_TAR.AcualizaIdImagenTareaWorkflowIdRUta(id_ruta,
                                                                                id_tarea,
                                                                                Idseleccion,
                                                                                Nombre_Gabinete)
                If Result <> "YES" Then
                    Actualiza_a_Documento_Principal_treview = Result
                    Exit Function
                End If
            End If
            Dim myCommand2 As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand2.Connection = myConnection
            myCommand2.Transaction = myTrans
            '*****************************
            'Actualiza registro de las 
            'imagenes en la base de datos
            '*****************************
            myCommand2.CommandText = SqlUpdate
            Dim Swicth As Integer = 0
            Swicth = myCommand2.ExecuteNonQuery()
            '************************************
            'Determina si actualizo el registro
            'con los datos
            '*************************************
            If Swicth = 0 Then
                myTrans.Rollback()
                myConnection.Close()
                Actualiza_a_Documento_Principal_treview = "Imposible actualizar el registro en el gabinete, pero se actualizo a documento principal "
                Exit Function
            End If
            For i As Integer = 0 To treview.Nodes.Count - 1
                Dim split() As String = treview.Nodes(i).Value.Split("|")
                If split(1) = spliitem(1) Then
                    treview.Nodes(i).ImageUrl = "../workflow/imageneswf/page_white_principal.png"
                Else
                    Dim ref_seleccion As New Classselecciotarea
                    ref_seleccion.Agrega_icono_image_tre_view_extension(split(3), treview.Nodes(i))
                    'treview.Nodes(i).ImageUrl = "../workflow/imageneswf/page_white.png"
                End If
            Next
            Updadat.Update()
            myTrans.Commit()
            myConnection.Close()
            Actualiza_a_Documento_Principal_treview = "YES"
        Catch e As Exception
            Try
                myTrans.Rollback()
                myConnection.Close()

            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Actualiza_a_Documento_Principal_treview = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Actualiza_a_Documento_Principal_treview = "Error General " & e.Message
            Exit Function
        End Try

    End Function
    Function Actualiza_a_Documento_Principal(ByRef Updadat As UpdatePanel, ByRef Droplisbox As DropDownList _
    , ByVal Nombre_Gabinete As String, ByVal Enlace_Radic As String, ByVal nombre_campo As String) As String

        '--------------------------------------------------
        'Consulta los documentos relacionados con el enlace
        '--------------------------------------------------
        If Droplisbox.Items.Count = 0 Then
            Actualiza_a_Documento_Principal = "Imposible encontrar documentos para actualizar"
            Exit Function
        End If
        Dim spliitem() As String = Droplisbox.Items(Droplisbox.SelectedIndex).Text.Split("|")
        Dim Idseleccion As Integer = spliitem(0)
        Dim Matri_id() As String
        Erase Matri_id
        Dim SqlUpdate As String = ""
        For i As Integer = 0 To Droplisbox.Items.Count - 1
            Dim spli() As String = Droplisbox.Items(i).Text.Split("|")
            ReDim Preserve Matri_id(i)
            Matri_id(i) = spli(0)
            If i = 0 Then
                If spli(0) = spliitem(0) Then
                    SqlUpdate = "Update " & Nombre_Gabinete & " set " & nombre_campo & "='" & Enlace_Radic & "' where id=" & spli(0)
                Else
                    SqlUpdate = "Update " & Nombre_Gabinete & " set " & nombre_campo & "=null where id=" & spli(0)
                End If
            Else
                If spli(0) = spliitem(0) Then
                    SqlUpdate = SqlUpdate & "  ;  " & vbCrLf & "Update " & Nombre_Gabinete & " set " & nombre_campo & "='" & Enlace_Radic & "' where id=" & spli(0)
                Else
                    SqlUpdate = SqlUpdate & "  ;  " & vbCrLf & "Update " & Nombre_Gabinete & " set " & nombre_campo & "=null where id=" & spli(0)
                End If

            End If
        Next

        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_DA
        ref.Returna_Conexion_Mysql(myConnection)

        Dim myTrans As MySqlTransaction
        Try

            Dim myCommand2 As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand2.Connection = myConnection
            myCommand2.Transaction = myTrans
            '*****************************
            'Actualiza registro de las 
            'imagenes en la base de datos
            '*****************************
            myCommand2.CommandText = SqlUpdate
            Dim Swicth As Integer = 0
            Swicth = myCommand2.ExecuteNonQuery()
            '************************************
            'Determina si actualizo el registro
            'con los datos
            '*************************************
            If Swicth = 0 Then
                myTrans.Rollback()
                myConnection.Close()
                Actualiza_a_Documento_Principal = "Imposible actualizar registro  "
                Exit Function
            End If
            For i As Integer = 0 To Droplisbox.Items.Count - 1
                Dim split() As String = Droplisbox.Items(i).Text.Split("|")
                If split(0) = spliitem(0) Then
                    Droplisbox.Items(i).Text = split(0) & "|" & "R" & Enlace_Radic
                Else
                    Droplisbox.Items(i).Text = split(0) & "|" & "R"
                End If
            Next
            Updadat.Update()
            myTrans.Commit()
            myConnection.Close()
            Actualiza_a_Documento_Principal = "YES"
        Catch e As Exception
            Try
                myTrans.Rollback()
                myConnection.Close()

            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Actualiza_a_Documento_Principal = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Actualiza_a_Documento_Principal = "Error General " & e.Message
            Exit Function
        End Try

    End Function
    Function Lista_Documentos_Alamacenados_Escaner(ByVal Datos_Script As String, ByRef List As DropDownList,
                                                   ByRef up_date As UpdatePanel, ByRef label As Label) As String
        '-------------------------------------------------------
        'Función : Lista los documentos alamcenados con la 
        'opcion de enlace de documentos
        'Fecha : 2014-02-26
        'Ingeniero : Miguel Angel Urueta Miranda
        '-------------------------------------------------------
        Try
            List.Items.Clear()
            '----------------------------------------------------
            'Obtiene los datos de almacenamiento  Documentos Digitalizados
            '----------------------------------------------------
            Dim datos_enlace As String = Trim(Datos_Script)
            If datos_enlace = "" Then
                Lista_Documentos_Alamacenados_Escaner = "Enlace sin datos imposible almacenar"
                Exit Function
            End If

            If InStr(datos_enlace, "POSITIVOQL_") < 1 Then
                Lista_Documentos_Alamacenados_Escaner = "Enlace sin datos correctos"
                Exit Function
            End If
            Dim dat As String = datos_enlace.Replace("POSITIVOQL_", "<!#>")
            Dim Splipositvol() As String = dat.Split("<!#>")
            Dim SpliDATOS() As String = Splipositvol(1).Split("|")
            Dim SpliCAMPOS() As String = Splipositvol(2).Split("|")
            Dim Gabinete As String = SpliDATOS(1)
            Dim Radicado As String = SpliDATOS(2)
            Dim Campo_Radicado As String = SpliCAMPOS(2)
            'Dim matri_datos() As Datos_Almacenamiento
            'ReDim Preserve matri_datos(0)
            'matri_datos(0).nombre_campo = Campo_Radicado
            ''---------------------------------------------
            ''Remplaza las R000 del numero de radicado
            ''---------------------------------------------
            'Dim RadicTemporal As String = ""
            'RadicTemporal = Radicado.Replace("R", "")
            'Radicado = Val(RadicTemporal)
            'matri_datos(0).valor_campo = Radicado
            'ReDim Preserve matri_datos(1)
            'matri_datos(1).nombre_campo = "ENLASE"
            'matri_datos(1).valor_campo = Radicado

            '----------------------------------------------
            'Lista documentos almacenados con enlace
            '----------------------------------------------
            Dim Sql_consulta As String = "select ID,DISC,PAG,DBT,IDEX," & Campo_Radicado & "  from " & Gabinete & "  where ENLASE = '" & Radicado & "'" &
                " order by ID "
            Dim Numero_Imagenesl As Integer = 0
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("Gabinete_text")
            Dim result As String = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If result <> "YES" Then
                label.Text = "Documentos relacionados " & "(0)"
                up_date.Update()
                Lista_Documentos_Alamacenados_Escaner = "Error Consultando en tabla " & Gabinete & Sql_consulta
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                label.Text = "Documentos relacionados " & "(0)"
                up_date.Update()
                Lista_Documentos_Alamacenados_Escaner = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    Dim ilist As New ListItem
                    ilist.Text = Datset.Tables(0).Rows(i).Item(0).ToString & "|R" & Datset.Tables(0).Rows(i).Item(5).ToString()
                    'ilist.Value = Datset.Tables(0).Rows(i).Item(0).ToString & "|" & _
                    '   Datset.Tables(0).Rows(i).Item(1).ToString & "|" & _
                    '  Datset.Tables(0).Rows(i).Item(2).ToString & "|" & _
                    ' Datset.Tables(0).Rows(i).Item(3).ToString & "|" & _
                    'Datset.Tables(0).Rows(i).Item(4).ToString()
                    ilist.Value = Gabinete & "|" & i.ToString
                    List.Items.Add(ilist)
                Next
                label.Text = "Documentos relacionados " & "(" & List.Items.Count & ")"
                up_date.Update()
                Lista_Documentos_Alamacenados_Escaner = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_Documentos_Alamacenados_Escaner = "Inconsistencia general listar documentos almacenados " & ex.Message
        End Try
    End Function
    Function Lista_Documentos_Almacenados_Escaner_Treview(ByVal Datos_Script As String,
                                                          ByVal id_tarea As Long,
                                                          ByRef Treview As TreeView,
                                                          ByVal id_gabinete As Integer,
                                                          ByVal nombre_gabinete As String,
                                                          ByRef up_date As UpdatePanel,
                                                          ByRef label As Label,
                                                          ByRef update_tree As UpdatePanel,
                                                          ByRef radicado_ref As String) As String
        Try
            '----------------------------------------------------------------
            'Obtiene los datos de almacenamiento  Documentos Digitalizados
            '-----------------------------------------------------------------
            Treview.Nodes.Clear()
            Dim Gabinete As String = ""
            Dim Radicado As String = ""
            Dim Campo_Radicado As String = ""
            Dim Result As String = ""
            If Datos_Script <> "" Then
                Dim datos_enlace As String = Trim(Datos_Script)
                If datos_enlace = "" Then
                    Lista_Documentos_Almacenados_Escaner_Treview = "Enlace sin datos imposible almacenar"
                    Exit Function
                End If

                If InStr(datos_enlace, "POSITIVOQL_") < 1 Then
                    Lista_Documentos_Almacenados_Escaner_Treview = "Enlace sin datos correctos"
                    Exit Function
                End If
                Dim dat As String = datos_enlace.Replace("POSITIVOQL_", "<!#>")
                Dim Splipositvol() As String = dat.Split("<!#>")
                Dim SpliDATOS() As String = Splipositvol(1).Split("|")
                Dim SpliCAMPOS() As String = Splipositvol(2).Split("|")
                Gabinete = SpliDATOS(1)
                Radicado = SpliDATOS(2)
                Campo_Radicado = SpliCAMPOS(2)
            Else
                Dim Refclas_seleccion As New Classselecciotarea
                Result = Refclas_seleccion.Solicita_radicado_id_tarea_seleccionada(HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                                                   id_tarea,
                                                                                   Radicado)
                If Result <> "YES" Then
                    Lista_Documentos_Almacenados_Escaner_Treview = Result
                    Exit Function
                End If
                Dim Ref_Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
                Result = Ref_Class_DETALLE_GABIENETE.SolicitaNombreCampoRadicadoGabinete(nombre_gabinete,
                                                                                    Campo_Radicado)
                If Result <> "YES" Then
                    Lista_Documentos_Almacenados_Escaner_Treview = Result
                    Exit Function
                End If
                '------------------------------------------------
                'Busca el nombre del gabinete por ruta workflow
                '------------------------------------------------
                If nombre_gabinete = "" Then
                    Lista_Documentos_Almacenados_Escaner_Treview = "Imposible encontrar el gabinete de almacenamiento para el tipo de trámite"
                    Exit Function
                End If
                Gabinete = nombre_gabinete
            End If
            radicado_ref = Radicado
            Dim inventario_documental As Integer = 0
            Dim aplica_trd As Integer = 0
            Dim asigna_unidad As Integer = 0
            Dim Refclas_produccion As New ClassGaProducionDocumental
            Dim Ref_Class_system1 As New Class_system1
            Result = Ref_Class_system1.Verifica_opcion_aplicar_trd_inventario_unidad_documental_gabinete(nombre_gabinete, _
                                                                                                          inventario_documental, _
                                                                                                          aplica_trd, _
                                                                                                          asigna_unidad)
            If Result <> "YES" Then
                Lista_Documentos_Almacenados_Escaner_Treview = Result
                Exit Function
            End If
            '-----------------------------------------------------------
            'Retorna el id de la imagen relacionada al flujo de trabajo
            '-----------------------------------------------------------
            Dim Refclasslecion As New Classselecciotarea
            Dim id_imagen_seleccion As Integer = 0
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = Class_DAT_ADIC_TAR.SolicitaIdImagenRelacionadaTareaworkflowIdRuta(HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                                                      id_tarea,
                                                                                      id_imagen_seleccion)
            If Result <> "YES" Then
                Lista_Documentos_Almacenados_Escaner_Treview = Result
                Exit Function
            End If
            Dim Sql_consulta As String = ""
            If aplica_trd = 0 Then
                Sql_consulta = "select ID,DISC,PAG,DBT,IDEX," & Campo_Radicado & "  from " & Gabinete & "  where ENLASE = '" & Radicado & "'" & _
               " order by ID "
            Else
                Sql_consulta = "select ID,DISC,PAG,DBT,IDEX," & Campo_Radicado & ",TIPODOCUMENTO,ID_TIPODOCUMENTO" & "  from " & Gabinete & "  where ENLASE = '" & Radicado & "'" & _
              " order by ID "
            End If
            Dim Numero_Imagenesl As Integer = 0
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet(Gabinete)
            Result = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Result <> "YES" Then
                label.Text = "Documentos relacionados " & "(0)"
                up_date.Update()
                Lista_Documentos_Almacenados_Escaner_Treview = "Error función Lista_Documentos_Almacenados_Escaner_Treview " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                label.Text = "Documentos relacionados " & "(0)"
                up_date.Update()
                Lista_Documentos_Almacenados_Escaner_Treview = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    Dim attrNodeGru1 As New TreeNode
                    attrNodeGru1.Value = Gabinete & "|" & Datset.Tables(0).Rows(i).Item(0) & "|" & Radicado & "|" & Datset.Tables(0).Rows(i).Item(3)
                    attrNodeGru1.PopulateOnDemand = False
                    If id_imagen_seleccion = Datset.Tables(0).Rows(i).Item(0) Then
                        attrNodeGru1.ImageUrl = "../workflow/imageneswf/page_white_principal.png"
                    Else
                        Dim refclas_seleccion As New Classselecciotarea
                        refclas_seleccion.Agrega_icono_image_tre_view_extension(Datset.Tables(0).Rows(i).Item(3).ToString, _
                                                                                attrNodeGru1)
                    End If
                    If aplica_trd <> 0 Then
                        If Datset.Tables(0).Rows(i).IsNull(6) = True Then
                            attrNodeGru1.Text = "Documento(" & i & ")"
                        Else
                            attrNodeGru1.Text = Datset.Tables(0).Rows(i).Item(6)
                        End If

                    Else
                        attrNodeGru1.Text = "Documento(" & i & ")"
                    End If
                    Treview.Nodes.Add(attrNodeGru1)
                Next
                label.Text = "Documentos relacionados " & "(" & Datset.Tables(0).Rows.Count & ")"
                Lista_Documentos_Almacenados_Escaner_Treview = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_Documentos_Almacenados_Escaner_Treview = "Inconsistencia general función Lista_Documentos_Almacenados_Escaner_Treview " & ex.Message
        Finally
            up_date.Update()
            update_tree.Update()
        End Try
    End Function
    Function Retorna_tipo_documento_gabinete(ByVal Gabinete As String, ByVal id_imagen As Long, _
                                             ByRef nombre_tipo_documento As String) As String
        Try
            Dim Sql_consulta As String = ""
            Sql_consulta = "select TIPODOCUMENTO  from " & Gabinete & "  where ID=" & id_imagen
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet(Gabinete)
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_tipo_documento_gabinete = "Error función Retorna_tipo_documento_gabinete " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_tipo_documento_gabinete = "Imposible encontrar el tipo documental con el identificador (" & id_imagen & ") en el gabinete (" & Gabinete & ")"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    nombre_tipo_documento = ""
                Else
                    nombre_tipo_documento = Datset.Tables(0).Rows(0).Item(0)
                End If
                Retorna_tipo_documento_gabinete = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_tipo_documento_gabinete = "Inconsistencia general función Retorna_tipo_documento_gabinete " & ex.Message
        End Try
    End Function



    Function SolicitaParametrosParaListartiposDocumentalesTareaWorkflowExterna(ByVal IdRutaWorkflow As Integer,
                                                                               ByVal IdTareaWorkflow As Long,
                                                                               ByVal NombreRutaWorkflow As String,
                                                                               ByRef TipoTramitePlantilla As String,
                                                                               ByRef IdGabinete As Integer,
                                                                               ByRef NombreGabinete As String,
                                                                               ByRef Radicado As String,
                                                                               ByRef Tramite As String) As String
        Try
            Dim Result As String = ""
            Dim nombre_campo_radicado As String = ""
            Dim Refclass_seleccion_tarea As New Classselecciotarea
            Dim Ref_class_config_listado As New Class_configuracion_listado_ruta
            Dim Class_configuracion_gabinete As New Class_configuracion_gabinete
            TipoTramitePlantilla = "RADICACION ENTRANTE"
            Result = Ref_class_config_listado.SolicitaNombreCampoRadicadoRuta(IdRutaWorkflow,
                                                                              nombre_campo_radicado)
            If Result <> "YES" Then
                SolicitaParametrosParaListartiposDocumentalesTareaWorkflowExterna = Result
                Exit Function
            End If
            Dim nombre_campo_tramite As String = ""
            Dim Ref_class_cinfig_listado_ruta As New Class_configuracion_listado_ruta
            Result = Ref_class_cinfig_listado_ruta.SolicitaNombreCampoTramiteRuta(IdRutaWorkflow,
                                                                                  nombre_campo_tramite)
            If Result <> "YES" Then
                SolicitaParametrosParaListartiposDocumentalesTareaWorkflowExterna = Result
                Exit Function
            End If
            Result = Me.Solicita_radicado_id_gabnete_id_tarea_seleccionda(nombre_campo_radicado,
                                                                          nombre_campo_tramite,
                                                                          NombreRutaWorkflow,
                                                                          IdTareaWorkflow,
                                                                          IdGabinete,
                                                                          Radicado,
                                                                          Tramite)
            If Result <> "YES" Then
                SolicitaParametrosParaListartiposDocumentalesTareaWorkflowExterna = Result
                Exit Function
            End If
            Result = Class_configuracion_gabinete.SolicitanombreGabineteWorkflow(IdGabinete,
                                                                                                   NombreGabinete)
            If Result <> "YES" Then
                SolicitaParametrosParaListartiposDocumentalesTareaWorkflowExterna = Result
                Exit Function
            End If
            SolicitaParametrosParaListartiposDocumentalesTareaWorkflowExterna = "YES"
        Catch ex As Exception
            SolicitaParametrosParaListartiposDocumentalesTareaWorkflowExterna = "Inconsistencia general función Activar_tipos_documentales_flujo_externo " & ex.Message
        End Try
    End Function
    Function SolicitaParametrosParaListartiposDocumentalesTareaWorkflowInterna(ByVal id_ruta As Integer,
                                                                               ByVal id_tarea As Long,
                                                                               ByRef tipo_plantilla_tramite As String,
                                                                               ByRef id_tipo_tramite As Integer,
                                                                               ByRef id_gabinete As Integer,
                                                                               ByRef nombre_gabinete As String,
                                                                               ByRef id_config_digitalizacion As Integer,
                                                                               ByRef Radicado As String,
                                                                               ByRef descripcion_tramite As String) As String
        '---------------------------------------------------------
        'Funcion : Activa listar tipos documentales relacionados
        'devuelve el tipo de plantilla del radicado y  la 
        'identificación del tipo de tramite relacionado
        'a la tarea seleccionada
        'con los paramtros id tarea e id ruta
        'Fecha : 2018-01-04
        'Ingeniero : Miguel Angel Urueta Miranda
        '---------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Refclas_seleccion_tarea As New Classselecciotarea
            '------------------------------------------------------------------
            'Solicita el radicado relacionado al la tarea workflow y la ruta
            '------------------------------------------------------------------
            Result = Refclas_seleccion_tarea.Solicita_radicado_id_tarea_seleccionada(id_ruta,
                                                                                     id_tarea,
                                                                                     Radicado)
            If Result <> "YES" Then
                SolicitaParametrosParaListartiposDocumentalesTareaWorkflowInterna = Result
                Exit Function
            End If
            Dim nombre_plantilla As String = ""
            Dim Refclas_radicador As New ClassRadicador
            '-----------------------------------------------------------------------
            'Retorna el nombre de la plantilla de radicación por medio del registro
            'general del radicado
            '-----------------------------------------------------------------------
            Dim Ref_Class_ra_registro_general_radicacion As New Class_ra_registro_general_radicacion
            Result = Ref_Class_ra_registro_general_radicacion.SolicitaNombrePlantillaRadicado(Radicado,
                                                                                              nombre_plantilla)
            If Result <> "YES" Then
                SolicitaParametrosParaListartiposDocumentalesTareaWorkflowInterna = Result
                Exit Function
            End If
            '-----------------------------------------------------------------------------------------------------
            'Retorna el tipo de plantilla por nombre de plantilla ("RADICACION ENTRANTE", "RADICACION SALIENTE")
            '-----------------------------------------------------------------------------------------------------
            Dim Ref_Class_system_plantilla_radicado As New Class_system_plantilla_radicado
            Result = Ref_Class_system_plantilla_radicado.Retorna_Tipo_Plantilla_nombre(nombre_plantilla,
                                                                                       tipo_plantilla_tramite)
            If Result <> "YES" Then
                SolicitaParametrosParaListartiposDocumentalesTareaWorkflowInterna = Result
                Exit Function
            End If
            'Dim descripcion_tramite As String = ""
            '-----------------------------------------------------------------------------------------------------
            'Retorna la descripción del trámite con el número de radicado y el nombre de la plantilla
            '-----------------------------------------------------------------------------------------------------
            Dim Ref_Class_plantillas_radicacion As New Class_plantillas_radicacion
            Result = Ref_Class_plantillas_radicacion.retorna_tipo_documental_radicado(Radicado,
                                                                                     nombre_plantilla,
                                                                                     descripcion_tramite)
            If Result <> "YES" Then
                SolicitaParametrosParaListartiposDocumentalesTareaWorkflowInterna = Result
                Exit Function
            End If
            Dim id_plantilla_radicado As Integer = 0
            '-------------------------------------------------------------------------------------------------------
            'Retorna la identificación o código de la plantilla
            '-------------------------------------------------------------------------------------------------------
            Result = Ref_Class_system_plantilla_radicado.SolicitaIdPlantillaRadicado(id_plantilla_radicado,
                                                                                        nombre_plantilla)
            If Result <> "YES" Then
                SolicitaParametrosParaListartiposDocumentalesTareaWorkflowInterna = Result
                Exit Function
            End If
            Dim ref_class_tipo_doc_entrante As New Class_tipo_doc_entrante
            Result = ref_class_tipo_doc_entrante.retorna_id_tipo_tramite_radicado(id_plantilla_radicado,
                                                                                  descripcion_tramite,
                                                                                  id_tipo_tramite)
            If Result <> "YES" Then
                SolicitaParametrosParaListartiposDocumentalesTareaWorkflowInterna = Result
                Exit Function
            End If

            Result = ref_class_tipo_doc_entrante.Retorna_id_nombre_gabinete_tipo_tramite(id_plantilla_radicado,
                                                                                        descripcion_tramite,
                                                                                        id_gabinete,
                                                                                        nombre_gabinete)
            If Result <> "YES" Then
                SolicitaParametrosParaListartiposDocumentalesTareaWorkflowInterna = Result
                Exit Function
            End If
            Dim Refclas_config As New Class_ra_dig_config_digitalizacion
            Result = Refclas_config.Solicita_id_configuracion_digitalizacion(id_tipo_tramite,
                                                                             tipo_plantilla_tramite,
                                                                             id_config_digitalizacion)
            If Result <> "YES" Then
                SolicitaParametrosParaListartiposDocumentalesTareaWorkflowInterna = Result
                Exit Function
            End If
            SolicitaParametrosParaListartiposDocumentalesTareaWorkflowInterna = "YES"
            Exit Function
        Catch ex As Exception
            SolicitaParametrosParaListartiposDocumentalesTareaWorkflowInterna = "Inconsistencia general función Activa_listar_tipos_documentales " & ex.Message
        End Try
    End Function
    Function Actualiza_tipo_documento_lista_chequeo(ByVal id_imagen As Long, _
                                                    ByVal id_tipo_lista_chequeo As Integer, _
                                                    ByVal nombre_gabinete As String, _
                                                    ByRef tre_node As TreeNode, _
                                                    ByVal id_tipo_configuracion_tramite As Integer, _
                                                    ByVal radicado As String, _
                                                    ByRef ref_update As UpdatePanel, _
                                                    ByRef valor_cambio As String) As String

        Dim Result As String = ""
        Dim Ref_producion As New ClassGaProducionDocumental
        Dim inventario_documental As Integer = 0
        Dim aplica_trd As Integer = 0
        Dim asigna_unidad As Integer = 0
        Dim Ref_Class_system1 As New Class_system1
        Result = Ref_Class_system1.Verifica_opcion_aplicar_trd_inventario_unidad_documental_gabinete(nombre_gabinete, _
                                                                                                    inventario_documental, _
                                                                                                    aplica_trd, _
                                                                                                    asigna_unidad)
        If Result <> "YES" Then
            Actualiza_tipo_documento_lista_chequeo = Result
            Exit Function
        End If
        If aplica_trd = 0 Then
            Actualiza_tipo_documento_lista_chequeo = "Debe activar la opción aplicar tabla de retención en el gabinete (" & nombre_gabinete & ")"
            Exit Function
        End If
        If inventario_documental = 0 Then
            Actualiza_tipo_documento_lista_chequeo = "Debe activar la opción aplicar inventario documental en el gabinete (" & nombre_gabinete & ")"
            Exit Function
        End If
        Dim stru_ As Stru_config_digitalizacion = Nothing
        Dim Class_ra_dig_config_digitalizacion As New Class_ra_dig_config_digitalizacion
        If id_tipo_configuracion_tramite <> 0 Then
            Result = Class_ra_dig_config_digitalizacion.Solicita_datos_configuracion_digitalizacion(id_tipo_configuracion_tramite,
                                                                                                    stru_)
            If Result <> "YES" Then
                Actualiza_tipo_documento_lista_chequeo = Result
                Exit Function
            End If
            '-----------------------------------------------------
            'Verifica la seleccion del documento si la lista de
            'chequeo es obligatoria
            '-----------------------------------------------------
            If stru_.OBLIGA_LISTA_CHEQUEO = 1 Then
                If id_tipo_lista_chequeo = -1 Then
                    Actualiza_tipo_documento_lista_chequeo = "Debe seleccionar el tipo documento de la lista de chequeo"
                    Exit Function
                End If
            End If
        End If

        Dim id_inventario_documental As Integer = 0
        Dim ClassGaProducionDocumental As New ClassGaProducionDocumental
        Result = ClassGaProducionDocumental.Solicita_id_inventario_documental(id_imagen,
                                                                              nombre_gabinete,
                                                                              id_inventario_documental)
        If Result <> "YES" Then
            Actualiza_tipo_documento_lista_chequeo = Result
            Exit Function
        End If
        Dim stru_lista_chequeo As stru_tipo_lista_chequeo
        Dim Ref_Class_ra_dig_tipos_docum_lista_chequeo As New ra_dig_tipos_docum_lista_chequeo
        If id_tipo_lista_chequeo <> "-1" Then
            Result = Ref_Class_ra_dig_tipos_docum_lista_chequeo.SolicitaDatosTipoDocumentalListaChequeo(id_tipo_lista_chequeo, _
                                                                                                             stru_lista_chequeo)
            If Result <> "YES" Then
                Actualiza_tipo_documento_lista_chequeo = Result
                Exit Function
            End If
            If stru_lista_chequeo.UNICO = 1 Then
                Result = Me.Verifica_existencia_del_tipo_documental_en_el_gabinete_con_enlace_radicado(radicado, _
                                                                                                     nombre_gabinete, _
                                                                                                     stru_lista_chequeo)
                If Result <> "YES" Then
                    Actualiza_tipo_documento_lista_chequeo = Result
                    Exit Function
                End If
            End If
        End If
        '------------------------------------------------------
        'Asina datos de tipo documental para la actualización
        '------------------------------------------------------
        Dim id_tipo_documento As Integer = 0
        Dim id_area As Integer = 0
        Dim id_serie As Integer = 0
        Dim id_sub_serie As Integer = 0
        Dim descripcion_tipo_documento As String = ""
        Dim nombre_area As String = ""
        Dim nombre_serie As String = ""
        Dim nombre_sub_serie As String = ""
        If id_tipo_lista_chequeo <> -1 Then
            Dim stru As stru_tipo_lista_chequeo
            Result = Ref_Class_ra_dig_tipos_docum_lista_chequeo.SolicitaDatosTipoDocumentalListaChequeo(id_tipo_lista_chequeo, _
                                                                                                             stru)
            If Result <> "YES" Then
                Actualiza_tipo_documento_lista_chequeo = Result
                Exit Function
            End If
            If stru.subseries_documentales_Id_SubSeries <> 0 Then
                id_tipo_documento = stru.tipos_doc_subseries_Id_Tipos_Doc_SubSerie
            Else
                id_tipo_documento = stru.tipo_doc_series_Id_Tipo_Doc_Series
            End If
            id_serie = stru.series_documentales_Id_Series
            id_sub_serie = stru.subseries_documentales_Id_SubSeries
            Dim ref_Class_series_documentales As New Class_series_documentales
            Result = ref_Class_series_documentales.Retorna_id_area_serie_documental(id_serie, _
                                                                                    id_area)
            If Result <> "YES" Then
                Actualiza_tipo_documento_lista_chequeo = Result
                Exit Function
            End If
            Dim ref_Class_ra_tipo_doc_series As New Class_ra_tipo_doc_series
            If id_tipo_documento <> 0 Then
                Result = ref_Class_ra_tipo_doc_series.Retorna_nombre_tipo_documental(id_serie, _
                                                                                     id_sub_serie, _
                                                                                     id_tipo_documento, _
                                                                                     descripcion_tipo_documento)
                If Result <> "YES" Then
                    Actualiza_tipo_documento_lista_chequeo = Result
                    Exit Function
                End If
            End If
            Dim ref_Class_areas_depart_radicacion As New Class_areas_depart_radicacion
            If id_area <> 0 Then
                Result = ref_Class_areas_depart_radicacion.Retorna_nombre_area_por_id_area(id_area, _
                                                                                           nombre_area)
                If Result <> "YES" Then
                    Actualiza_tipo_documento_lista_chequeo = Result
                    Exit Function
                End If
            End If

            If id_serie <> 0 Then
                Result = ref_Class_series_documentales.Retorna_nombre_serie_id_serie(id_serie,
                                                                                     nombre_serie)
                If Result <> "YES" Then
                    Actualiza_tipo_documento_lista_chequeo = Result
                    Exit Function
                End If
            End If
            Dim Class_subseries_documentales As New Class_subseries_documentales
            If id_sub_serie <> 0 Then
                Result = Class_subseries_documentales.Retorna_nombre_sub_serie(id_sub_serie,
                                                                                nombre_sub_serie)
                If Result <> "YES" Then
                    Actualiza_tipo_documento_lista_chequeo = Result
                    Exit Function
                End If
            End If

        End If
        Dim Refclas_da_gabinete As New ClassDaGabinete
        Dim id_tipo_imagen As Integer = -1
        Result = Refclas_da_gabinete.SolicitaIdTipoImagen(id_imagen, _
                                                            nombre_gabinete, _
                                                            id_tipo_imagen)
        Dim ref_descripcion_tipo_documento As String = "Null"
        If descripcion_tipo_documento <> "" Then
            ref_descripcion_tipo_documento = "'" & descripcion_tipo_documento & "'"
        End If
        Dim ref_id_tipo_documento As Object = "Null"
        If id_tipo_documento <> 0 Then
            ref_id_tipo_documento = id_tipo_documento
        End If
        Dim ref_id_area As Object = "Null"
        If id_area <> 0 Then
            ref_id_area = id_area
        End If
        Dim ref_id_serie As Object = "Null"
        If id_serie <> 0 Then
            ref_id_serie = id_serie
        End If
        Dim ref_id_sub_serie As Object = "Null"
        If id_sub_serie <> 0 Then
            ref_id_sub_serie = id_sub_serie
        End If
        Dim ref_nombre_area As String = "Null"
        If nombre_area <> "" Then
            ref_nombre_area = "'" & nombre_area & "'"
        End If
        Dim ref_nombre_serie As String = "Null"
        If nombre_serie <> "" Then
            ref_nombre_serie = "'" & nombre_serie & "'"
        End If
        Dim ref_nombre_sub_serie As String = "Null"
        If nombre_sub_serie <> "" Then
            ref_nombre_sub_serie = "'" & nombre_sub_serie & "'"
        End If
        Dim Update_gabinete As String = "update " & nombre_gabinete & " set TIPODOCUMENTO=" & ref_descripcion_tipo_documento & "," & _
            "ID_TIPODOCUMENTO=" & ref_id_tipo_documento & ",ID_AREA=" & ref_id_area & ",ID_SERIE=" & ref_id_serie & ",ID_SUB_SERIE=" & ref_id_sub_serie & _
            ",NOMBRESERIE=" & ref_nombre_serie & ",NOMBRESUBSERIE=" & ref_nombre_sub_serie & " where ID=" & id_imagen
        Dim Update_producion As String = ""
        If id_inventario_documental <> 0 Then
            Update_producion = "update registro_producion_documental set ID_TIPO_DOCUMENTO=" & ref_id_tipo_documento & ",ID_AREA_DEPARTAMENTO=" & ref_id_area & _
            ",ID_SERIE_DOCUMENTO=" & ref_id_serie & ",ID_SUBSERIE_DOCUMENTO=" & ref_id_sub_serie & ",SERIE_DOCUMENTO=" & ref_nombre_serie & _
            ",SUBSERIE_DOCUMENTO=" & ref_nombre_sub_serie & ",NOMBRE_AREA_DEPARTAMENTO=" & ref_nombre_area & ",DESCRIPCION_TIPO_DOCUMENTO=" & ref_descripcion_tipo_documento & _
            " where ID_REGISTRO_PRODUCION_DOCUMENTAL=" & id_inventario_documental
        End If
        Dim id_expediente As Integer = 0
        Dim id_cert_indice_expediente As Long = 0
        Dim class_ra_cert_indice_expediente As New Class_ra_cert_indice_expediente
        Dim stru_ruta_expediente_ As stru_ruta_expediente = Nothing
        Dim ref_ra_ruta_expediente As New Class_ra_ruta_expediente
        Dim stru_produccion_indice As stru_produccion_indice = Nothing
        Dim class_producciondocumental As New ClassGaProducionDocumental
        Dim xmlArchivo As New XmlDocument
        Dim update_indice As String = ""
        Dim Ruta_archivo_xml As String = ""
        If id_inventario_documental <> 0 Then
            Result = class_producciondocumental.Solicita_id_expediente_registro_produccion(id_inventario_documental,
                                                                                          id_expediente)
            If Result <> "YES" Then
                Actualiza_tipo_documento_lista_chequeo = Result
                Exit Function
            End If
            If id_expediente <> 0 Then
                Result = class_ra_cert_indice_expediente.Solicita_existencia_indice_produccion(id_inventario_documental, _
                                                                                               id_cert_indice_expediente)
                If Result <> "YES" Then
                    Actualiza_tipo_documento_lista_chequeo = Result
                    Exit Function
                End If
                If id_cert_indice_expediente <> 0 Then
                    Result = ref_ra_ruta_expediente.Solicita_datos_estructura_ruta_expediente(stru_ruta_expediente_)
                    If Result <> "YES" Then
                        Actualiza_tipo_documento_lista_chequeo = Result
                        Exit Function
                    End If
                    Dim disco_carpeta_ As String = stru_ruta_expediente_.DISCO
                    Dim class_zerro_fill_ As New Class_zero_fill
                    Result = class_zerro_fill_.zero_fill(disco_carpeta_, 9, "0")
                    If Result <> "YES" Then
                        Actualiza_tipo_documento_lista_chequeo = Result
                        Exit Function
                    End If
                    Dim Ruta_expediente As String = stru_ruta_expediente_.RUTA.Replace("/", "\")
                    If Directory.Exists(Ruta_expediente) = False Then
                        Actualiza_tipo_documento_lista_chequeo = "Por favor crea la siguiente ruta en el servidor " & Ruta_expediente
                        Exit Function
                    End If
                    Ruta_expediente = Ruta_expediente & disco_carpeta_
                    If Directory.Exists(Ruta_expediente) = False Then
                        Directory.CreateDirectory(Ruta_expediente)
                    End If
                    Dim expediente_zero_fil As String = id_expediente.ToString
                    Result = class_zerro_fill_.zero_fill(expediente_zero_fil, 9, "0")
                    If Result <> "YES" Then
                        Actualiza_tipo_documento_lista_chequeo = Result
                        Exit Function
                    End If
                    Ruta_archivo_xml = Ruta_expediente & "\" & expediente_zero_fil & ".xml"
                    '----------------------------------------------------------------------------
                    'Actualiza indice archivo expediente archivo
                    '-----------------------------------------------------------------------------
                    Dim classgaexpediente As New ClassGaExpediente
                    Result = classgaexpediente.Actualiza_indice_tipo_documental_xml_expediente(Ruta_archivo_xml, _
                                                                                               id_inventario_documental, _
                                                                                               descripcion_tipo_documento, _
                                                                                               xmlArchivo)
                    If Result <> "YES" Then
                        Actualiza_tipo_documento_lista_chequeo = Result
                        Exit Function
                    End If
                    update_indice = "update ra_cert_indice_expediente set Tipologia_documental='" & descripcion_tipo_documento & "'" & _
                                    " where id_cert_indice_expediente=" & id_cert_indice_expediente
                End If
            End If
        End If
        Dim datos_campo As String = ""
        Dim detalle_trans As String = ""
        Dim campos_trans As String = ""
        Dim hor2 As New System.DateTime
        hor2 = Date.Now
        Dim hora As String = hor2.Hour.ToString & ":" & hor2.Minute.ToString & ":" & hor2.Second.ToString
        detalle_trans = "CAMBIA CLASE DOCUMENTO"
        campos_trans = "CAMBIA CLASE (" & tre_node.Text & _
              ") A CLASE (" & descripcion_tipo_documento & ")"
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Dim date1al As String = Date.Today
        Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        Dim isert_datos As String = ""
        If Result <> "YES" Then
            Actualiza_tipo_documento_lista_chequeo = Result
            Exit Function
        End If
        isert_datos = isert_datos & "('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," & _
                     id_inventario_documental & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hora & "','GESTOR DOCUMENTAL','" & campos_trans & "')"

        Dim update_gestion As String = "INSERT INTO ra_log_inventario (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_REGISTRO_PRODUCCION" & _
                                    ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values " & _
                                    isert_datos

        Dim SqlTransac As String = " INSERT INTO logdocuarchi (id_tran,desc_op,USER_OPER,DATE_TRANS," _
        & "RUT_DOCU,GABINETE,CAMPOS,IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO) VALUES ( "
        SqlTransac = SqlTransac & "'" & id_imagen & "',"
        SqlTransac = SqlTransac & "'" & "EditarIndice" & "',"
        SqlTransac = SqlTransac & "'" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "',"
        SqlTransac = SqlTransac & "'" & date1al & "',"
        SqlTransac = SqlTransac & "'" & "NONE" & "',"
        SqlTransac = SqlTransac & "'" & nombre_gabinete & "',"
        SqlTransac = SqlTransac & "'" & datos_campo & "','" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hora & "','" & "GESTOR DOCUMENTAL'" & ")"
        Dim myConnection As New MySqlConnection
        Dim myConnection_da As New conect.Dbase_Conction_Mysql_DA
        myConnection_da.Returna_Conexion_Mysql(myConnection)
        Dim myCommand2 As MySqlCommand = myConnection.CreateCommand()
        Dim myTrans As MySqlTransaction
        Dim Switc As Integer = 0
        Try
            Dim refclas As New ClassAlmacenamiento
            'myConnection.Open()
            myTrans = myConnection.BeginTransaction()
            myCommand2.Connection = myConnection
            myCommand2.Transaction = myTrans
            '------------------------------------------
            'Actualiza gabinete
            '------------------------------------------
            If Update_gabinete <> "" Then
                myCommand2.CommandText = Update_gabinete
                Switc = myCommand2.ExecuteNonQuery()
                If Switc = 0 Then
                    Actualiza_tipo_documento_lista_chequeo = "Imposible actualizar la tabla gabinete cambios  : " & Update_gabinete
                    'myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            '------------------------------------------
            'Actualiza registro producción documental
            '------------------------------------------
            If Update_producion <> "" Then
                myCommand2.CommandText = Update_producion
                Switc = myCommand2.ExecuteNonQuery()
                If Switc = 0 Then
                    Actualiza_tipo_documento_lista_chequeo = "Imposible actualizar la tabla gabinete cambios  : " & Update_producion
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            '--------------------------------------------
            'Actualiza log inventario
            '--------------------------------------------
            If update_gestion <> "" Then
                myCommand2.CommandText = update_gestion
                Switc = myCommand2.ExecuteNonQuery()
                If Switc = 0 Then
                    Actualiza_tipo_documento_lista_chequeo = "Imposible actualizar la tabla log inventario cambios  : " & update_gestion
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            '--------------------------------------------
            'Actualiza indice log  docuarchi
            '--------------------------------------------
            myCommand2.CommandText = SqlTransac
            Switc = myCommand2.ExecuteNonQuery()
            If Switc = 0 Then
                Actualiza_tipo_documento_lista_chequeo = "Imposible actualizar la tabla log docuarchi cambios  : " & SqlTransac
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '--------------------------------------------
            'Actualiza indice documento 
            '--------------------------------------------
            If update_indice <> "" Then
                myCommand2.CommandText = update_indice
                Switc = myCommand2.ExecuteNonQuery()
                If Switc = 0 Then
                    Actualiza_tipo_documento_lista_chequeo = "Imposible actualizar la tabla indice expediente  : " & update_indice
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
                xmlArchivo.Save(Ruta_archivo_xml)
            End If
            myTrans.Commit()
            If descripcion_tipo_documento = "" Then
                tre_node.Text = "Documento"
            Else
                tre_node.Text = descripcion_tipo_documento
            End If
            valor_cambio = descripcion_tipo_documento
            Dim Refclas_selecion As New Classselecciotarea
            Refclas_selecion.Agrega_icono_image_tre_view_extension(id_tipo_imagen.ToString, _
                                                                   tre_node)
            ref_update.Update()
            Actualiza_tipo_documento_lista_chequeo = "YES"
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Actualiza_tipo_documento_lista_chequeo = "An exception of type " + ex.GetType().ToString() + _
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Actualiza_tipo_documento_lista_chequeo = "Error General " & e.Message
            Exit Function
        Finally
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
        End Try
    End Function
    Function Actualiza_tipo_documento_lista_chequeo(ByVal id_imagen As Long, _
                                                    ByVal id_tipo_lista_chequeo As Integer, _
                                                    ByVal nombre_gabinete As String, _
                                                    ByVal nombre_tipo_old As String, _
                                                    ByVal id_tipo_configuracion_tramite As Integer, _
                                                    ByVal radicado As String,
                                                    ByRef valor_cambio As String) As String

        Dim Result As String = ""
        Dim Ref_producion As New ClassGaProducionDocumental
        Dim inventario_documental As Integer = 0
        Dim aplica_trd As Integer = 0
        Dim asigna_unidad As Integer = 0
        Dim Ref_Class_system1 As New Class_system1
        Result = Ref_Class_system1.Verifica_opcion_aplicar_trd_inventario_unidad_documental_gabinete(nombre_gabinete, _
                                                                                                    inventario_documental, _
                                                                                                    aplica_trd, _
                                                                                                    asigna_unidad)
        If Result <> "YES" Then
            Actualiza_tipo_documento_lista_chequeo = Result
            Exit Function
        End If
        If aplica_trd = 0 Then
            Actualiza_tipo_documento_lista_chequeo = "Debe activar la opción aplicar tabla de retención en el gabinete (" & nombre_gabinete & ")"
            Exit Function
        End If
        If inventario_documental = 0 Then
            Actualiza_tipo_documento_lista_chequeo = "Debe activar la opción aplicar inventario documental en el gabinete (" & nombre_gabinete & ")"
            Exit Function
        End If
        Dim stru_ As Stru_config_digitalizacion = Nothing
        Dim Class_ra_dig_config_digitalizacion As New Class_ra_dig_config_digitalizacion
        If id_tipo_configuracion_tramite <> 0 Then
            Result = Class_ra_dig_config_digitalizacion.Solicita_datos_configuracion_digitalizacion(id_tipo_configuracion_tramite,
                                                                                                    stru_)
            If Result <> "YES" Then
                Actualiza_tipo_documento_lista_chequeo = Result
                Exit Function
            End If
            '-----------------------------------------------------
            'Verifica la seleccion del documento si la lista de
            'chequeo es obligatoria
            '-----------------------------------------------------
            If stru_.OBLIGA_LISTA_CHEQUEO = 1 Then
                If id_tipo_lista_chequeo = -1 Then
                    Actualiza_tipo_documento_lista_chequeo = "Debe seleccionar el tipo documento de la lista de chequeo"
                    Exit Function
                End If
            End If
        End If

        Dim id_inventario_documental As Integer = 0
        Dim ClassGaProducionDocumental As New ClassGaProducionDocumental
        Result = ClassGaProducionDocumental.Solicita_id_inventario_documental(id_imagen,
                                                                              nombre_gabinete,
                                                                              id_inventario_documental)
        If Result <> "YES" Then
            Actualiza_tipo_documento_lista_chequeo = Result
            Exit Function
        End If
        Dim stru_lista_chequeo As stru_tipo_lista_chequeo = Nothing
        Dim Ref_Class_ra_dig_tipos_docum_lista_chequeo As New ra_dig_tipos_docum_lista_chequeo
        If id_tipo_lista_chequeo <> "-1" Then
            Result = Ref_Class_ra_dig_tipos_docum_lista_chequeo.SolicitaDatosTipoDocumentalListaChequeo(id_tipo_lista_chequeo, _
                                                                                                             stru_lista_chequeo)
            If Result <> "YES" Then
                Actualiza_tipo_documento_lista_chequeo = Result
                Exit Function
            End If
            If stru_lista_chequeo.UNICO = 1 Then
                Result = Me.Verifica_existencia_del_tipo_documental_en_el_gabinete_con_enlace_radicado(radicado, _
                                                                                                       nombre_gabinete, _
                                                                                                       stru_lista_chequeo)
                If Result <> "YES" Then
                    Actualiza_tipo_documento_lista_chequeo = Result
                    Exit Function
                End If
            End If
        End If
        '------------------------------------------------------
        'Asina datos de tipo documental para la actualización
        '------------------------------------------------------
        Dim id_tipo_documento As Integer = 0
        Dim id_area As Integer = 0
        Dim id_serie As Integer = 0
        Dim id_sub_serie As Integer = 0
        Dim descripcion_tipo_documento As String = ""
        Dim nombre_area As String = ""
        Dim nombre_serie As String = ""
        Dim nombre_sub_serie As String = ""
        If id_tipo_lista_chequeo <> -1 Then
            Dim stru As stru_tipo_lista_chequeo = Nothing
            Result = Ref_Class_ra_dig_tipos_docum_lista_chequeo.SolicitaDatosTipoDocumentalListaChequeo(id_tipo_lista_chequeo, _
                                                                                                             stru)
            If Result <> "YES" Then
                Actualiza_tipo_documento_lista_chequeo = Result
                Exit Function
            End If
            If stru.subseries_documentales_Id_SubSeries <> 0 Then
                id_tipo_documento = stru.tipos_doc_subseries_Id_Tipos_Doc_SubSerie
            Else
                id_tipo_documento = stru.tipo_doc_series_Id_Tipo_Doc_Series
            End If
            id_serie = stru.series_documentales_Id_Series
            id_sub_serie = stru.subseries_documentales_Id_SubSeries
            Dim ref_Class_series_documentales As New Class_series_documentales
            Result = ref_Class_series_documentales.Retorna_id_area_serie_documental(id_serie, _
                                                                                    id_area)
            If Result <> "YES" Then
                Actualiza_tipo_documento_lista_chequeo = Result
                Exit Function
            End If
            Dim ref_Class_ra_tipo_doc_series As New Class_ra_tipo_doc_series
            If id_tipo_documento <> 0 Then
                Result = ref_Class_ra_tipo_doc_series.Retorna_nombre_tipo_documental(id_serie, _
                                                                                     id_sub_serie, _
                                                                                     id_tipo_documento, _
                                                                                     descripcion_tipo_documento)
                If Result <> "YES" Then
                    Actualiza_tipo_documento_lista_chequeo = Result
                    Exit Function
                End If
            End If
            Dim ref_Class_areas_depart_radicacion As New Class_areas_depart_radicacion
            If id_area <> 0 Then
                Result = ref_Class_areas_depart_radicacion.Retorna_nombre_area_por_id_area(id_area, _
                                                                                           nombre_area)
                If Result <> "YES" Then
                    Actualiza_tipo_documento_lista_chequeo = Result
                    Exit Function
                End If
            End If

            If id_serie <> 0 Then
                Result = ref_Class_series_documentales.Retorna_nombre_serie_id_serie(id_serie,
                                                                                     nombre_serie)
                If Result <> "YES" Then
                    Actualiza_tipo_documento_lista_chequeo = Result
                    Exit Function
                End If
            End If
            Dim Class_subseries_documentales As New Class_subseries_documentales
            If id_sub_serie <> 0 Then
                Result = Class_subseries_documentales.Retorna_nombre_sub_serie(id_sub_serie,
                                                                                nombre_sub_serie)
                If Result <> "YES" Then
                    Actualiza_tipo_documento_lista_chequeo = Result
                    Exit Function
                End If
            End If

        End If
        Dim id_expediente As Integer = 0
        Dim id_cert_indice_expediente As Long = 0
        Dim class_ra_cert_indice_expediente As New Class_ra_cert_indice_expediente
        Dim stru_ruta_expediente_ As stru_ruta_expediente = Nothing
        Dim ref_ra_ruta_expediente As New Class_ra_ruta_expediente
        Dim stru_produccion_indice As stru_produccion_indice = Nothing
        Dim class_producciondocumental As New ClassGaProducionDocumental
        Dim xmlArchivo As New XmlDocument
        Dim update_indice As String = ""
        Dim Ruta_archivo_xml As String = ""
        If id_inventario_documental <> 0 Then
            Result = class_producciondocumental.Solicita_id_expediente_registro_produccion(id_inventario_documental,
                                                                                           id_expediente)
            If Result <> "YES" Then
                Actualiza_tipo_documento_lista_chequeo = Result
                Exit Function
            End If
            If id_expediente <> 0 Then
                Result = class_ra_cert_indice_expediente.Solicita_existencia_indice_produccion(id_inventario_documental, _
                                                                                               id_cert_indice_expediente)
                If Result <> "YES" Then
                    Actualiza_tipo_documento_lista_chequeo = Result
                    Exit Function
                End If
                If id_cert_indice_expediente <> 0 Then
                    Result = ref_ra_ruta_expediente.Solicita_datos_estructura_ruta_expediente(stru_ruta_expediente_)
                    If Result <> "YES" Then
                        Actualiza_tipo_documento_lista_chequeo = Result
                        Exit Function
                    End If
                    Dim disco_carpeta_ As String = stru_ruta_expediente_.DISCO
                    Dim class_zerro_fill_ As New Class_zero_fill
                    Result = class_zerro_fill_.zero_fill(disco_carpeta_, 9, "0")
                    If Result <> "YES" Then
                        Actualiza_tipo_documento_lista_chequeo = Result
                        Exit Function
                    End If
                    Dim Ruta_expediente As String = stru_ruta_expediente_.RUTA.Replace("/", "\")
                    If Directory.Exists(Ruta_expediente) = False Then
                        Actualiza_tipo_documento_lista_chequeo = "Por favor crea la siguiente ruta en el servidor " & Ruta_expediente
                        Exit Function
                    End If
                    Ruta_expediente = Ruta_expediente & disco_carpeta_
                    If Directory.Exists(Ruta_expediente) = False Then
                        Directory.CreateDirectory(Ruta_expediente)
                    End If
                    Dim expediente_zero_fil As String = id_expediente.ToString
                    Result = class_zerro_fill_.zero_fill(expediente_zero_fil, 9, "0")
                    If Result <> "YES" Then
                        Actualiza_tipo_documento_lista_chequeo = Result
                        Exit Function
                    End If
                    Ruta_archivo_xml = Ruta_expediente & "\" & expediente_zero_fil & ".xml"
                    '----------------------------------------------------------------------------
                    'Actualiza indice archivo expediente archivo
                    '-----------------------------------------------------------------------------
                    Dim classgaexpediente As New ClassGaExpediente
                    Result = classgaexpediente.Actualiza_indice_tipo_documental_xml_expediente(Ruta_archivo_xml, _
                                                                                               id_inventario_documental, _
                                                                                               descripcion_tipo_documento, _
                                                                                               xmlArchivo)
                    If Result <> "YES" Then
                        Actualiza_tipo_documento_lista_chequeo = Result
                        Exit Function
                    End If
                    update_indice = "update ra_cert_indice_expediente set Tipologia_documental='" & descripcion_tipo_documento & "'" & _
                                    " where id_cert_indice_expediente=" & id_cert_indice_expediente
                End If
            End If
        End If
        Dim Refclas_da_gabinete As New ClassDaGabinete
        Dim id_tipo_imagen As Integer = -1
        Dim value_documento As String = ""
        Result = Refclas_da_gabinete.RemoveDiacritics(descripcion_tipo_documento,
                                                      descripcion_tipo_documento)

        Dim ref_descripcion_tipo_documento As String = "Null"
        If descripcion_tipo_documento <> "" Then
            ref_descripcion_tipo_documento = "'" & descripcion_tipo_documento & "'"
        End If
        Dim ref_id_tipo_documento As Object = "Null"
        If id_tipo_documento <> 0 Then
            ref_id_tipo_documento = id_tipo_documento
        End If
        Dim ref_id_area As Object = "Null"
        If id_area <> 0 Then
            ref_id_area = id_area
        End If
        Dim ref_id_serie As Object = "Null"
        If id_serie <> 0 Then
            ref_id_serie = id_serie
        End If
        Dim ref_id_sub_serie As Object = "Null"
        If id_sub_serie <> 0 Then
            ref_id_sub_serie = id_sub_serie
        End If
        Dim ref_nombre_area As String = "Null"
        If nombre_area <> "" Then
            ref_nombre_area = "'" & nombre_area & "'"
        End If
        Dim ref_nombre_serie As String = "Null"
        If nombre_serie <> "" Then
            ref_nombre_serie = "'" & nombre_serie & "'"
        End If
        Dim ref_nombre_sub_serie As String = "Null"
        If nombre_sub_serie <> "" Then
            ref_nombre_sub_serie = "'" & nombre_sub_serie & "'"
        End If
        Dim Update_gabinete As String = "update " & nombre_gabinete & " set TIPODOCUMENTO=" & ref_descripcion_tipo_documento & "," & _
            "ID_TIPODOCUMENTO=" & ref_id_tipo_documento & ",ID_AREA=" & ref_id_area & ",ID_SERIE=" & ref_id_serie & ",ID_SUB_SERIE=" & ref_id_sub_serie & _
            ",NOMBRESERIE=" & ref_nombre_serie & ",NOMBRESUBSERIE=" & ref_nombre_sub_serie & " where ID=" & id_imagen
        Dim Update_producion As String = ""
        If id_inventario_documental <> 0 Then
            Update_producion = "update registro_producion_documental set ID_TIPO_DOCUMENTO=" & ref_id_tipo_documento & ",ID_AREA_DEPARTAMENTO=" & ref_id_area & _
            ",ID_SERIE_DOCUMENTO=" & ref_id_serie & ",ID_SUBSERIE_DOCUMENTO=" & ref_id_sub_serie & ",SERIE_DOCUMENTO=" & ref_nombre_serie & _
            ",SUBSERIE_DOCUMENTO=" & ref_nombre_sub_serie & ",NOMBRE_AREA_DEPARTAMENTO=" & ref_nombre_area & ",DESCRIPCION_TIPO_DOCUMENTO=" & ref_descripcion_tipo_documento & _
            " where ID_REGISTRO_PRODUCION_DOCUMENTAL=" & id_inventario_documental
        End If
        Dim datos_campo As String = ""
        Dim detalle_trans As String = ""
        Dim campos_trans As String = ""
        Dim hor2 As New System.DateTime
        hor2 = Date.Now
        Dim hora As String = hor2.Hour.ToString & ":" & hor2.Minute.ToString & ":" & hor2.Second.ToString
        detalle_trans = "CAMBIA CLASE DOCUMENTO"
        campos_trans = "CAMBIA CLASE (" & nombre_tipo_old & _
              ") A CLASE (" & descripcion_tipo_documento & ")"
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Dim date1al As String = Date.Today
        Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        Dim isert_datos As String = ""
        If Result <> "YES" Then
            Actualiza_tipo_documento_lista_chequeo = Result
            Exit Function
        End If
        isert_datos = isert_datos & "('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," & _
                     id_inventario_documental & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hora & "','GESTOR DOCUMENTAL','" & campos_trans & "')"

        Dim update_gestion As String = "INSERT INTO ra_log_inventario (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_REGISTRO_PRODUCCION" & _
                                    ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values " & _
                                    isert_datos
        Dim SqlTransac As String = " INSERT INTO logdocuarchi (id_tran,desc_op,USER_OPER,DATE_TRANS," _
        & "RUT_DOCU,GABINETE,CAMPOS,IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO) VALUES ( "
        SqlTransac = SqlTransac & "'" & id_imagen & "',"
        SqlTransac = SqlTransac & "'" & "EditarIndice" & "',"
        SqlTransac = SqlTransac & "'" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "',"
        SqlTransac = SqlTransac & "'" & date1al & "',"
        SqlTransac = SqlTransac & "'" & "NONE" & "',"
        SqlTransac = SqlTransac & "'" & nombre_gabinete & "',"
        SqlTransac = SqlTransac & "'" & datos_campo & "','" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hora & "','" & "GESTOR DOCUMENTAL'" & ")"
        Dim myConnection As New MySqlConnection
        Dim myConnection_da As New conect.Dbase_Conction_Mysql_DA
        myConnection_da.Returna_Conexion_Mysql(myConnection)
        Dim myCommand2 As MySqlCommand = myConnection.CreateCommand()
        Dim myTrans As MySqlTransaction
        Dim Switc As Integer = 0
        Try
            Dim refclas As New ClassAlmacenamiento
            'myConnection.Open()
            myTrans = myConnection.BeginTransaction()
            myCommand2.Connection = myConnection
            myCommand2.Transaction = myTrans
            '------------------------------------------
            'Actualiza gabinete
            '------------------------------------------
            If Update_gabinete <> "" Then
                myCommand2.CommandText = Update_gabinete
                Switc = myCommand2.ExecuteNonQuery()
                If Switc = 0 Then
                    Actualiza_tipo_documento_lista_chequeo = "Imposible actualizar la tabla gabinete cambios  : " & Update_gabinete
                    'myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            '------------------------------------------
            'Actualiza registro producción documental
            '------------------------------------------
            If Update_producion <> "" Then
                myCommand2.CommandText = Update_producion
                Switc = myCommand2.ExecuteNonQuery()
                If Switc = 0 Then
                    Actualiza_tipo_documento_lista_chequeo = "Imposible actualizar la tabla gabinete cambios  : " & Update_producion
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            '--------------------------------------------
            'Actualiza log inventario
            '--------------------------------------------
            If update_gestion <> "" Then
                myCommand2.CommandText = update_gestion
                Switc = myCommand2.ExecuteNonQuery()
                If Switc = 0 Then
                    Actualiza_tipo_documento_lista_chequeo = "Imposible actualizar la tabla log inventario cambios  : " & update_gestion
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            '--------------------------------------------
            'Actualiza indice log  docuarchi
            '--------------------------------------------
            myCommand2.CommandText = SqlTransac
            Switc = myCommand2.ExecuteNonQuery()
            If Switc = 0 Then
                Actualiza_tipo_documento_lista_chequeo = "Imposible actualizar la tabla log docuarchi cambios  : " & SqlTransac
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '--------------------------------------------
            'Actualiza indice expdiente
            '--------------------------------------------
            If update_indice <> "" Then
                myCommand2.CommandText = update_indice
                Switc = myCommand2.ExecuteNonQuery()
                If Switc = 0 Then
                    Actualiza_tipo_documento_lista_chequeo = "Imposible actualizar la tabla indice expediente  : " & update_indice
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
                xmlArchivo.Save(Ruta_archivo_xml)
            End If
            myTrans.Commit()
            valor_cambio = descripcion_tipo_documento
            Actualiza_tipo_documento_lista_chequeo = "YES"
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Actualiza_tipo_documento_lista_chequeo = "An exception of type " + ex.GetType().ToString() + _
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Actualiza_tipo_documento_lista_chequeo = "Error General " & e.Message
            Exit Function
        Finally
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
        End Try
    End Function
    Function Valida_adjuntar_documento_digitalizado(ByVal id_imagen As Long,
                                                    ByVal nombre_gabinete As String,
                                                    ByRef extension_archivo As String) As String
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassDaGabinete
            Dim id_tipo_imagen As Integer = 0
            Dim extension As String = ""
            Result = Refclas.SolicitaIdTipoImagen(id_imagen,
                                                    nombre_gabinete,
                                                    id_tipo_imagen)
            If Result <> "YES" Then
                Valida_adjuntar_documento_digitalizado = Result
                Exit Function
            End If
            Result = Refclas.Solicita_extension_documento_docuarchi_segun_id_tipo_archivo(id_tipo_imagen,
                                                                                          extension)
            If Result <> "YES" Then
                Valida_adjuntar_documento_digitalizado = Result
                Exit Function
            End If
            extension_archivo = extension.Replace(".", "")
            If extension <> ".TIF" And extension <> ".PDF" Then
                Valida_adjuntar_documento_digitalizado = "El documento al que usted quiere adjuntar no esta en formato tif ni pdf, imposible añadir"
                Exit Function
            End If
            Valida_adjuntar_documento_digitalizado = "YES"
        Catch ex As Exception
            Valida_adjuntar_documento_digitalizado = "Inconsistencia general función Activa_adjuntar_documento_digitalizado " & ex.Message
        End Try
    End Function
    Function EliminaDocumentosDigigitalizados(ByVal RutaBusqueda As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Elimina documentos digitalizados en la ruta temporal
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'RutaBusqueda        : Representa la ruta de busqueda
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        '
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-01
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            If Directory.Exists(RutaBusqueda & "\") = False Then
                EliminaDocumentosDigigitalizados = "YES"
                Exit Function
            End If
            For Each Archivo In My.Computer.FileSystem.GetFiles(
                       RutaBusqueda & "\",
                        FileIO.SearchOption.SearchTopLevelOnly, "*.*")
                Kill(Archivo)
            Next
            EliminaDocumentosDigigitalizados = "YES"
            Exit Function
        Catch ex As Exception
            EliminaDocumentosDigigitalizados = "Inconsistencia general funcion EliminaDocumentosDigigitalizados " & ex.Message
        End Try
    End Function


    Function SolicitaRutaDocumentoDigitalizado(ByVal IdentificadorArchivo As Integer,
                                               ByVal RutaBusquedaArchivo As String,
                                               ByRef RutaDocumentoDgitalizado As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la ruta del documento digitalizado
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdentificadorArchivo    : Representa la identificación del archivo
        'RutaBusquedaArchivo     : Representa la ruta de busqueda de los archivos a guardar
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'RutaDocumentoDgitalizado  : Retorna la ruta del documento digitalizado
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-07-21
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Doc_Cuerpo As String = ""
            '---////Valida error en la copia del documento digitalizado en el servidor---////
            If HttpContext.Current.Session.Item("WF_RUTA_ERROR_ESCANER_FILE") <> "" Then
                SolicitaRutaDocumentoDigitalizado = HttpContext.Current.Session.Item("WF_RUTA_ERROR_ESCANER_FILE")
                Exit Function
            End If
            '-------------------------------------------------------
            'Genera cuerpo documento busqueda
            '-------------------------------------------------------
            Result = Ceros_Documentos_Digitalizados(IdentificadorArchivo,
                                                    Doc_Cuerpo)
            If Result <> "YES" Then
                SolicitaRutaDocumentoDigitalizado = Result
                Exit Function
            End If
            Dim MatrizDocumentos() As String
            Erase MatrizDocumentos
            Dim Archivo As String = ""
            Dim incre As Integer = 0
            For Each Archivo In My.Computer.FileSystem.GetFiles(
                       RutaBusquedaArchivo & "\",
                        FileIO.SearchOption.SearchTopLevelOnly, Doc_Cuerpo & "*.*")
                ReDim Preserve MatrizDocumentos(incre)
                MatrizDocumentos(incre) = Archivo
                incre = incre + 1
            Next
            If MatrizDocumentos Is Nothing Then
                SolicitaRutaDocumentoDigitalizado = "Imposible encontrar archivos digitalizados (" & Doc_Cuerpo & ") en la ruta (" & RutaBusquedaArchivo & "\)"
                Exit Function
            End If
            RutaDocumentoDgitalizado = MatrizDocumentos(0)
            SolicitaRutaDocumentoDigitalizado = "YES"
            Exit Function
        Catch ex As Exception
            SolicitaRutaDocumentoDigitalizado = "Inconsistencia general funcion SolicitaRutaDocumentoDigitalizado " & ex.Message
        End Try
    End Function
    Function SolicitaMatrizDocumentosDigitalizados(ByVal Id_entificador_archivo As Integer,
                                                   ByVal RutaBusquedaArchivo As String,
                                                   ByRef MatrizDocumentos() As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la matriz de documentos digitlizados
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'Id_entificador_archivo  : Representa la identificación del archivo
        'RutaBusquedaArchivo     : Representa la ruta de busqueda de los archivos a guardar
        'id_plantilla_radicacion : 
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'id_usuario_radicador  : Retorna la idnetificación del usuario radicador
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-02-15
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim Result As String = ""
            Dim Doc_Cuerpo As String = ""
            '---////Valida error en la copia del documento digitalizado en el servidor---////
            If HttpContext.Current.Session.Item("WF_RUTA_ERROR_ESCANER_FILE") <> "" Then
                SolicitaMatrizDocumentosDigitalizados = HttpContext.Current.Session.Item("WF_RUTA_ERROR_ESCANER_FILE")
                Exit Function
            End If
            '-------------------------------------------------------
            'Genera cuerpo documento busqueda
            '-------------------------------------------------------
            Result = Ceros_Documentos_Digitalizados(Id_entificador_archivo,
                                                    Doc_Cuerpo)
            If Result <> "YES" Then
                SolicitaMatrizDocumentosDigitalizados = Result
                Exit Function
            End If
            Dim Matri_Documentos() As String
            Erase Matri_Documentos
            Dim Archivo As String = ""
            Dim incre As Integer = 0
            For Each Archivo In My.Computer.FileSystem.GetFiles(
                       RutaBusquedaArchivo & "\",
                        FileIO.SearchOption.SearchTopLevelOnly, Doc_Cuerpo & "*.*")
                ReDim Preserve Matri_Documentos(incre)
                Matri_Documentos(incre) = Archivo
                incre = incre + 1
            Next
            If Matri_Documentos Is Nothing Then
                SolicitaMatrizDocumentosDigitalizados = "Imposible encontrar archivos digitalizados (" & Doc_Cuerpo & ") en la ruta (" & RutaBusquedaArchivo & "\)"
                Exit Function
            End If
            Dim file_info As New FileInfo(Matri_Documentos(0))
            Dim refclas_neodinamyc As New ClassNeodynamic
            If UCase(file_info.Extension) = ".TIF" Then
                Dim ruta_tempo_directroy As String = HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ESCANER") & "\TEMPO_EXTRACION"
                If Directory.Exists(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ESCANER") & "\TEMPO_EXTRACION") = False Then
                    Directory.CreateDirectory(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ESCANER") & "\TEMPO_EXTRACION")
                End If
                Result = refclas_neodinamyc.Extraer_Documento_de_Multitif_fisico(Matri_Documentos(0),
                                                                                 MatrizDocumentos,
                                                                                 ruta_tempo_directroy & "\")
                If Result <> "YES" Then
                    SolicitaMatrizDocumentosDigitalizados = Result
                    Exit Function
                Else
                    SolicitaMatrizDocumentosDigitalizados = "YES"
                    Exit Function
                End If
            Else
                Erase MatrizDocumentos
                incre = 0
                ReDim Preserve MatrizDocumentos(incre)
                MatrizDocumentos(incre) = Matri_Documentos(UBound(Matri_Documentos))
                If Matri_Documentos.Length > 1 Then
                    For z As Integer = 0 To Matri_Documentos.Length - 2
                        incre = incre + 1
                        ReDim Preserve MatrizDocumentos(incre)
                        MatrizDocumentos(incre) = Matri_Documentos(z)
                    Next
                End If
            End If
            SolicitaMatrizDocumentosDigitalizados = "YES"
        Catch ex As Exception
            SolicitaMatrizDocumentosDigitalizados = "Funcion SolicitaMatrizDocumentosDigitalizados " & ex.Message
        End Try
    End Function
    Function Ceros_Documentos_Digitalizados(ByVal numero As Integer, ByRef doc As String) As String
        '---------------------------------------------------------------
        'Funcion : Genera el cuerpo de documentos digitalizados
        'Fecha : 2014-02-24
        'Ingeniero : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------
        Try
            Dim n = numero.ToString.Length

            Select Case n

                Case 1
                    doc = "DIG000000000" + numero.ToString

                Case 2
                    doc = "DIG00000000" + numero.ToString

                Case 3
                    doc = "DIG0000000" + numero.ToString


                Case 4
                    doc = "DIG000000" + numero.ToString


                Case 5
                    doc = "DIG00000" + numero.ToString


                Case 6
                    doc = "DIG0000" + numero.ToString


                Case 7
                    doc = "DIG000" + numero.ToString


                Case 8
                    doc = "DIG00" + numero.ToString


                Case 9
                    doc = "DIG0" + numero.ToString


                Case 10
                    doc = "DIG" + numero.ToString

            End Select

            Ceros_Documentos_Digitalizados = "YES"
        Catch ex As Exception
            Ceros_Documentos_Digitalizados = "Funcion ceros " & ex.Message
        End Try
    End Function
    Function Obtiene_Valores_Campos_Documento_Enlazados(ByRef Matri_Datos_Almacen() As String, _
                                                        ByVal Nombre_Tabla As String, _
                                                        ByVal Matri_Datos_Asignado() As Datos_Almacenamiento) As String
        '******************************************************************
        'Funcion : Obtiene campos del almacenamiento del gabinete y asigna
        'los datos de almacenamiento
        'ingeniero Miguel Angel Urueta Miranda
        'Fecha : 2015-02-03
        'Modificado para web el 2016-06-29
        'Funcion extraida del workflow cliente y modificada para el modulo
        'web
        '*******************************************************************
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Numero_Colum As Integer = 0
            Dim Result As String = ""
            Dim Sql_consulta As String = "SELECT CAMPO FROM DETALLE_GABIENETE WHERE GABINETE" & _
                "='" & Nombre_Tabla & "' AND VISIBLE=1 order by IDENTI"
            Dim Datset As DataSet = New DataSet("DETALLE_GABIENETE")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Obtiene_Valores_Campos_Documento_Enlazados = "Función Obtiene_Valores_Campos_Documento_Enlazados dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Obtiene_Valores_Campos_Documento_Enlazados = "Imposible Encontrar  campos de almacenamiento"
                Exit Function
            Else
                Erase Matri_Datos_Almacen
                Dim icont As Integer = 0
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    Dim nombre_Campo As String = Datset.Tables(0).Rows(i).Item(0)
                    '        '----Asigna el valor al campo de la matriz 
                    ReDim Preserve Matri_Datos_Almacen(i)
                    Matri_Datos_Almacen(i) = ""
                    If Not Matri_Datos_Asignado Is Nothing Then
                        For z As Integer = 0 To Matri_Datos_Asignado.Length - 1
                            If UCase(nombre_Campo) = UCase(Matri_Datos_Asignado(z).nombre_campo) Then
                                Matri_Datos_Almacen(i) = Matri_Datos_Asignado(z).valor_campo
                                Exit For
                            End If
                        Next
                    End If
                Next
                Obtiene_Valores_Campos_Documento_Enlazados = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Obtiene_Valores_Campos_Documento_Enlazados = "Error General Funcion Obtiene_Valores_Campos_Documento_Enlazado Error :" & ex.Message

        End Try
    End Function



    Function Solicita_radicado_id_gabnete_id_tarea_seleccionda(ByVal nombre_campo_radicado As String, _
                                                               ByVal nombre_campo_tramite As String, _
                                                               ByVal nombre_ruta As String, _
                                                               ByVal id_tarea_seleccionada As Long, _
                                                               ByRef id_gabinete As Integer, _
                                                               ByRef radicado As String, _
                                                               ByRef tramite As String) As String
        Try
            Dim Result As String = ""
            Dim Parametro_Consulta As String = "Select " & nombre_campo_radicado & "," & nombre_campo_tramite & ",ID_GABINETE from dat_adic_tar" & nombre_ruta & _
                " where INICIO_TAREAS_WORKFLOW_ID_TAREA=" & id_tarea_seleccionada
            Dim Datset_consulta As DataSet = New DataSet(nombre_ruta)
            Dim ref As New conect.Dbase_Conction_Mysql
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset_consulta)
            If Result <> "YES" Then
                Solicita_radicado_id_gabnete_id_tarea_seleccionda = "Error Solicita_radicado_id_gabnete_id_tarea_seleccionda " & Result
                Exit Function
            End If
            If Datset_consulta.Tables(0).Rows.Count = 0 Then
                Solicita_radicado_id_gabnete_id_tarea_seleccionda = "Imposible encontrar los datos del flujo de trabajo del código de tarea (" & id_tarea_seleccionada & ")"
                Exit Function
            Else
                If Datset_consulta.Tables(0).Rows(0).IsNull(0) Then
                    radicado = ""
                Else
                    radicado = Datset_consulta.Tables(0).Rows(0).Item(0)
                End If
                If Datset_consulta.Tables(0).Rows(0).IsNull(1) Then
                    tramite = ""
                Else
                    tramite = Datset_consulta.Tables(0).Rows(0).Item(1)
                End If
                If Datset_consulta.Tables(0).Rows(0).IsNull(2) Then
                    id_gabinete = 0
                Else
                    id_gabinete = Datset_consulta.Tables(0).Rows(0).Item(2)
                End If
                Solicita_radicado_id_gabnete_id_tarea_seleccionda = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_radicado_id_gabnete_id_tarea_seleccionda = "Inconsistencia general función Solicita_radicado_id_gabnete_id_tarea_seleccionda " & ex.Message
        End Try
    End Function
    Function Activa_guardar_documento_digitalizado_relacionado_a_tramite(ByVal id_tipo_configuracion_tramite As Integer, _
                                                                         ByVal nombre_gabinete As String, _
                                                                         ByVal id_tipo_documento_lista_chequeo As Integer, _
                                                                         ByVal radicado As String) As String
        Try
            Dim stru As Stru_config_digitalizacion = Nothing
            Dim Result As String = ""
            Dim Class_ra_dig_config_digitalizacion As New Class_ra_dig_config_digitalizacion
            Result = Class_ra_dig_config_digitalizacion.Solicita_datos_configuracion_digitalizacion(id_tipo_configuracion_tramite,
                                                                                                    stru)
            If Result <> "YES" Then
                Activa_guardar_documento_digitalizado_relacionado_a_tramite = Result
                Exit Function
            End If
            '-----------------------------------------------------
            'Verfica la opcion aplicar tabla de retención en el
            'gabinete
            '-----------------------------------------------------
            Dim Ref_producion As New ClassGaProducionDocumental

            '-----------------------------------------------------
            'Verifica la seleccion del documento si la lista de
            'chequeo es obligatoria
            '-----------------------------------------------------
            If stru.OBLIGA_LISTA_CHEQUEO = 1 Then
                If id_tipo_documento_lista_chequeo = -1 Then
                    Activa_guardar_documento_digitalizado_relacionado_a_tramite = "Debe seleccionar el tipo documento de la lista de chequeo"
                    Exit Function
                End If
            End If
            Dim stru_lista_chequeo As stru_tipo_lista_chequeo = Nothing
            Dim Ref_Class_ra_dig_tipos_docum_lista_chequeo As New ra_dig_tipos_docum_lista_chequeo
            If id_tipo_documento_lista_chequeo <> "-1" Then
                Result = Ref_Class_ra_dig_tipos_docum_lista_chequeo.SolicitaDatosTipoDocumentalListaChequeo(id_tipo_documento_lista_chequeo, _
                                                                                                                 stru_lista_chequeo)
                If Result <> "YES" Then
                    Activa_guardar_documento_digitalizado_relacionado_a_tramite = Result
                    Exit Function
                End If
                If stru_lista_chequeo.UNICO = 1 Then
                    Result = Me.Verifica_existencia_del_tipo_documental_en_el_gabinete_con_enlace_radicado(radicado, _
                                                                                                         nombre_gabinete, _
                                                                                                         stru_lista_chequeo)
                    If Result <> "YES" Then
                        Activa_guardar_documento_digitalizado_relacionado_a_tramite = Result
                        Exit Function
                    End If
                End If
            End If
            Activa_guardar_documento_digitalizado_relacionado_a_tramite = "YES"
        Catch ex As Exception
            Activa_guardar_documento_digitalizado_relacionado_a_tramite = "Inconsistencia general función Activa_guardar_documento_digitalizado_relacionado_a_tramite " & ex.Message
        End Try
    End Function


    Function Verfica_existencia_tipo_documental_obligatorio_digitalizado(ByVal Radicado As String, _
                                                                         ByVal nombre_gabinete As String, _
                                                                         ByVal id_tramite As Integer) As String
        Try
            Dim Result As String = ""
            Dim stru() As stru_tipo_lista_chequeo = Nothing
            Dim Ref_Class_ra_dig_tipos_docum_lista_chequeo As New ra_dig_tipos_docum_lista_chequeo
            Result = Ref_Class_ra_dig_tipos_docum_lista_chequeo.Lista_tipos_documentales_obligatorios_tramite(id_tramite, _
                                                                                                              stru)
            If Result <> "YES" Then
                Verfica_existencia_tipo_documental_obligatorio_digitalizado = Result
                Exit Function
            End If
            Dim id_tipo_documento As Integer = 0
            Dim nombre_tipo_documento As String = ""
            Dim ref_Class_ra_tipo_doc_series As New Class_ra_tipo_doc_series
            If Not stru Is Nothing Then
                For i As Integer = 0 To stru.Length - 1
                    Result = Me.Verifica_existencia_del_tipo_documental_obligatorio_en_gabinete(Radicado, nombre_gabinete, stru(i), id_tipo_documento)
                    If Result <> "YES" Then
                        Verfica_existencia_tipo_documental_obligatorio_digitalizado = Result
                        Exit Function
                    Else
                        If id_tipo_documento <> 0 Then
                            Result = ref_Class_ra_tipo_doc_series.Retorna_nombre_tipo_documental(stru(i).series_documentales_Id_Series, _
                                                                                                 stru(i).subseries_documentales_Id_SubSeries, _
                                                                                                 id_tipo_documento, _
                                                                                                 nombre_tipo_documento)
                            If Result <> "YES" Then
                                Verfica_existencia_tipo_documental_obligatorio_digitalizado = Result
                                Exit Function
                            Else
                                Verfica_existencia_tipo_documental_obligatorio_digitalizado = "El tipo documental (" & nombre_tipo_documento & ") es obligatorio para este trámite, por favor agregue el documento"
                                Exit Function
                            End If
                        End If
                    End If
                Next
            Else
                Verfica_existencia_tipo_documental_obligatorio_digitalizado = "YES"
                Exit Function
            End If
            Verfica_existencia_tipo_documental_obligatorio_digitalizado = "YES"
            Exit Function
        Catch ex As Exception
            Verfica_existencia_tipo_documental_obligatorio_digitalizado = "Inconsistencia general función Verfica_existencia_tipo_documental_obligatorio_digitalizado " & ex.Message
        End Try
    End Function
    Function Verifica_existencia_del_tipo_documental_obligatorio_en_gabinete(ByVal radicado As String, _
                                                                            ByVal nombre_gabinete As String, _
                                                                            ByVal stru As stru_tipo_lista_chequeo, _
                                                                            ByRef id_tipo_documento As Integer) As String
        Try
            Dim Result As String = ""
            id_tipo_documento = 0
            Dim Tipo_documento As Integer = 0
            If stru.tipos_doc_subseries_Id_Tipos_Doc_SubSerie <> 0 Then
                Tipo_documento = stru.tipos_doc_subseries_Id_Tipos_Doc_SubSerie
            Else
                Tipo_documento = stru.tipo_doc_series_Id_Tipo_Doc_Series
            End If
            Dim Parametro_Consulta As String = " SELECT TIPODOCUMENTO " & _
                " from " & nombre_gabinete & " where ENLASE='" & _
                radicado & "' AND ID_TIPODOCUMENTO=" & Tipo_documento & _
                " AND ID_SERIE=" & stru.series_documentales_Id_Series
            If stru.subseries_documentales_Id_SubSeries <> 0 Then
                Parametro_Consulta = Parametro_Consulta & " AND ID_SUB_SERIE=" & stru.subseries_documentales_Id_SubSeries
            End If
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet(nombre_gabinete)
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_existencia_del_tipo_documental_obligatorio_en_gabinete = "Función Verifica_existencia_del_tipo_documental_en_el_gabinete_con_enlace_radicado dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_tipo_documento = Tipo_documento
                Verifica_existencia_del_tipo_documental_obligatorio_en_gabinete = "YES"
                Exit Function
            Else
                id_tipo_documento = 0
                Verifica_existencia_del_tipo_documental_obligatorio_en_gabinete = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_existencia_del_tipo_documental_obligatorio_en_gabinete = "Inconsistencia general función Verifica_existencia_del_tipo_documental_obligatorio_en_gabinete " & ex.Message
        End Try
    End Function
    Function Verifica_existencia_del_tipo_documental_en_el_gabinete_con_enlace_radicado(ByVal radicado As String, _
                                                                                        ByVal nombre_gabinete As String, _
                                                                                        ByVal stru As stru_tipo_lista_chequeo) As String
        Try
            Dim Result As String = ""
            Dim Tipo_documento As Integer = 0
            If stru.tipos_doc_subseries_Id_Tipos_Doc_SubSerie <> 0 Then
                Tipo_documento = stru.tipos_doc_subseries_Id_Tipos_Doc_SubSerie
            Else
                Tipo_documento = stru.tipo_doc_series_Id_Tipo_Doc_Series
            End If
            Dim Parametro_Consulta As String = " SELECT TIPODOCUMENTO " & _
                " from " & nombre_gabinete & " where ENLASE='" & _
                radicado & "' AND ID_TIPODOCUMENTO=" & Tipo_documento & _
                " AND ID_SERIE=" & stru.series_documentales_Id_Series
            If stru.subseries_documentales_Id_SubSeries <> 0 Then
                Parametro_Consulta = Parametro_Consulta & " AND ID_SUB_SERIE=" & stru.subseries_documentales_Id_SubSeries
            End If
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet(nombre_gabinete)
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_existencia_del_tipo_documental_en_el_gabinete_con_enlace_radicado = "Función Verifica_existencia_del_tipo_documental_en_el_gabinete_con_enlace_radicado dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Verifica_existencia_del_tipo_documental_en_el_gabinete_con_enlace_radicado = "YES"
                Exit Function
            Else
                Verifica_existencia_del_tipo_documental_en_el_gabinete_con_enlace_radicado = "El tipo documento (" & Datset.Tables(0).Rows(0).Item(0) & ") " & _
                    " se encuentra registrado en el gabinete"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_existencia_del_tipo_documental_en_el_gabinete_con_enlace_radicado = "Inconsistencia general función Verifica_existencia_del_tipo_documental_en_el_gabinete_con_enlace_radicado " & ex.Message
        End Try
    End Function


End Class
