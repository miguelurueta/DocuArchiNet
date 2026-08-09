Imports System.IO
Imports MySql.Data.MySqlClient

Public Structure ArrayItem_integracion_
    Public idliquidacion As String
    Public fecha As String
    Public tipotramite As String
    Public idmatriculabase As String
    Public idproponentebase As String
    Public identificacionbase As String
    Public nombrebase As String
    Public numerorecibo As String
    Public numerorecuperacion As String
    Public numeroradicacion As String
    Public tramitepresencial As String
    Public firmadoelectronicamente As String
    Public IMP_02_ID_CLAVE As String
    Public estado_migrado As String
    Public error_funcion As String
End Structure
Public Class Class_ra_sii_migra_imagenes
    Function Migra_documento_radicado_sii(ByVal numeroradicacion As String) As String
        Try
            Dim Result As String = ""
            Dim Class_ConsultarRadicado_sii As New Class_ConsultarRadicado_sii
            Dim stru_consulta_radicado As ConsultarRadicado_sii = Nothing
            Dim Class_tipo_doc_entrante As New Class_tipo_doc_entrante
            Dim ClassAlmacenamiento As New ClassAlmacenamiento
            Dim Class_ra_sii_migra_imagenes As New Class_ra_sii_migra_imagenes
            Dim Class_sii_migra_registro As New Class_sii_migra_registro
            Dim existencia As String = ""
            Result = Class_sii_migra_registro.Solicita_existencia_registro_codigo_sii(numeroradicacion, _
                                                                                     existencia)
            If Result <> "YES" Then
                Migra_documento_radicado_sii = Result
                Exit Function
            End If
            If existencia = "YES" Then
                Migra_documento_radicado_sii = "YES"
                Exit Function
            End If
            Result = Class_ConsultarRadicado_sii.ConSultarRadicado(numeroradicacion, _
                                                                   stru_consulta_radicado)
            If Result <> "YES" Then
                Migra_documento_radicado_sii = Result
                Exit Function
            End If
            Dim gabinete As String = ""
            Result = Class_tipo_doc_entrante.Solicita_nombre_gabinete_sii(stru_consulta_radicado.tipotramite, _
                                                                          gabinete)
            If Result <> "YES" Then
                Migra_documento_radicado_sii = Result
                Exit Function
            End If

            Dim Class_sii_migra_registro_imagen As New Class_sii_migra_registro
            Dim estado_existencia As String = ""
            Dim ruta_virtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION").ToString & "/DONWLOAD/"
            Dim ruta_fisica As String = HttpContext.Current.Server.MapPath(ruta_virtual)
            If Directory.Exists(ruta_fisica) = False Then
                Directory.CreateDirectory(ruta_fisica)
            End If
            Dim matricula As String = ""
            If stru_consulta_radicado.imagenes.Length > 0 Then
                For i As Integer = 0 To stru_consulta_radicado.imagenes.Length - 1
                    Result = Me.Solicita_existencia_registro_imagen_sii(stru_consulta_radicado.imagenes(i).idanexo, _
                                                                                                   numeroradicacion, _
                                                                                                   estado_existencia)
                    If Result <> "YES" Then
                        Migra_documento_radicado_sii = Result
                        Exit Function
                    End If
                    If estado_existencia <> "YES" Then
                        Dim ob1 = stru_consulta_radicado.imagenes(i).formato
                        Dim archivo As String = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION").ToString & "-" & numeroradicacion & "-" & stru_consulta_radicado.imagenes(i).idanexo & "_doc_adjunto_." & stru_consulta_radicado.imagenes(i).formato
                        Dim archivo_donwload As String = ruta_fisica & archivo
                        If IO.File.Exists(archivo_donwload) Then
                            Kill(archivo_donwload)
                        End If
                        Dim ob As Object
                        Dim Class_file_byte As New Class_file_byte
                        Result = Class_file_byte.DownloadFileViaRestAPI(stru_consulta_radicado.imagenes(i).url, _
                                                                        ob, _
                                                                        "MyDocumentLib", _
                                                                        archivo, _
                                                                        ruta_fisica)
                        If Result <> "YES" Then
                            Migra_documento_radicado_sii = Result
                            Exit Function
                        End If
                        HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA") = archivo_donwload
                        If HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA") = "" Then
                            Migra_documento_radicado_sii = "Imposible copiar el documento ruta sin documento"
                            Exit Function
                        End If
                        If UCase(gabinete) = "MERCANTIL" Then
                            matricula = stru_consulta_radicado.matricula
                        End If
                        If UCase(gabinete) = "RUP" Then
                            matricula = stru_consulta_radicado.proponente
                        End If
                        If UCase(gabinete) = "ESAL" Then
                            If stru_consulta_radicado.matricula <> "" Then
                                matricula = Val(stru_consulta_radicado.matricula.Replace("S", ""))
                            Else
                                matricula = ""
                            End If
                        End If
                        Dim id_imagen As Integer = 0
                        Dim observacion As String = ""
                        Dim nombre As String = ""
                        If stru_consulta_radicado.imagenes(i).observaciones <> "" Then
                            If stru_consulta_radicado.imagenes(i).observaciones.Length > 40 Then
                                observacion = Left(stru_consulta_radicado.imagenes(i).observaciones, 40)
                            Else
                                observacion = stru_consulta_radicado.imagenes(i).observaciones
                            End If
                        End If
                        If stru_consulta_radicado.nombre <> "" Then
                            If stru_consulta_radicado.nombre.Length > 40 Then
                                nombre = Left(stru_consulta_radicado.nombre, 40)
                            Else
                                nombre = stru_consulta_radicado.nombre
                            End If
                        End If
                        Result = ClassAlmacenamiento.Almacenamiento_migra_sii(gabinete, _
                                                                              matricula, _
                                                                              stru_consulta_radicado.recibo, _
                                                                              stru_consulta_radicado.radicado, _
                                                                              "", _
                                                                              "", _
                                                                              nombre, _
                                                                              observacion, _
                                                                              stru_consulta_radicado.identificacion, _
                                                                               "", _
                                                                              archivo_donwload, _
                                                                              id_imagen)
                        If Result <> "YES" Then
                            Migra_documento_radicado_sii = Result
                            Exit Function
                        End If
                        Result = Me.Registra_imagen_sii(Val(stru_consulta_radicado.imagenes(i).idanexo), _
                                                         stru_consulta_radicado.radicado, _
                                                         stru_consulta_radicado.imagenes(i).url, _
                                                         id_imagen, _
                                                         gabinete)
                        If Result <> "YES" Then
                            Migra_documento_radicado_sii = Result
                            Exit Function
                        End If
                    End If
                Next
                Result = Me.Registra_migracion_radicado_actualiza_estado(stru_consulta_radicado.radicado, _
                                                                         HttpContext.Current.Session.Item("Login_Usuario_Workfow"), _
                                                                         stru_consulta_radicado.imagenes.Length, _
                                                                         matricula, _
                                                                         stru_consulta_radicado.identificacion, _
                                                                         stru_consulta_radicado.recibo)
                If Result <> "YES" Then
                    Migra_documento_radicado_sii = Result
                    Exit Function
                End If
                Migra_documento_radicado_sii = "YES"
                Exit Function
            Else
                Result = Actualiza_estado_migrado(numeroradicacion)
                If Result <> "YES" Then
                    Migra_documento_radicado_sii = Result
                    Exit Function
                End If
                Migra_documento_radicado_sii = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Migra_documento_radicado_sii = "Inconsistencia general funcion Migra_documento_radicado_sii " & ex.Message
        End Try
    End Function
    Function Actualiza_estado_migrado(ByVal codigo_sii As String) As String
        Try
            Dim Parametro_Actualiza_System1 As String = "update imp_02_migra_sii_fecha set estado_migrado=2 where numeroradicacion='" & codigo_sii & "'"
            Dim sql_resutado As Integer = 0
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Result = ref.SELECTION_INSERT_COMMAND(Parametro_Actualiza_System1)
            If Result <> "YES" Then
                Actualiza_estado_migrado = Result
            Else
                Actualiza_estado_migrado = "YES"
            End If
        Catch ex As Exception
            Actualiza_estado_migrado = "Inconsistencia general funcion Actualiza_estado_migrado " & ex.Message
        End Try
    End Function
    Function Registra_migracion_radicado_actualiza_estado(ByVal codigo_sii As String, _
                                                          ByVal usuario_migracion As String, _
                                                          ByVal numero_registro_matri_imagenes As Integer, _
                                                          ByVal matricula As String, _
                                                          ByVal nit_identificacion As String, _
                                                          ByVal recibo_sii As String) As String
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Try
            Dim Refclas_gestion_fechas As New ClassGestionFechas
            Dim date1al As String = Date.Now
            Dim Result = Refclas_gestion_fechas.Formatea_fecha_time_framework(Date.Now, _
                                                                              date1al)
            If Result <> "YES" Then
                Registra_migracion_radicado_actualiza_estado = "Imposible formatear fecha " & Result
                Exit Function
            End If
            Dim sqlresultinsert As Integer = 0
            Dim Parametro_Actualiza_System1 As String = "update imp_02_migra_sii_fecha set estado_migrado=1 where numeroradicacion='" & codigo_sii & "'"
            Dim Parametro_Insercio As String = "insert into sii_migra_registro (codigo_sii,fecha_migracion,usuario_migracion,numero_registro_matri_imagenes," & _
                                               "matricula,nit_identificacion,recibo_sii) values (" & _
                                               "'" & codigo_sii & "','" & date1al & "','" & usuario_migracion & "','" & numero_registro_matri_imagenes & _
                                               "','" & matricula & "','" & nit_identificacion & "','" & recibo_sii & "')"
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = Parametro_Actualiza_System1
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Registra_migracion_radicado_actualiza_estado = "Imposible actualizar el estado de la lista de migracion   " & Parametro_Actualiza_System1
                myConnection.Close()
                Exit Function
            End If
            myCommand.CommandText = Parametro_Insercio
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Registra_migracion_radicado_actualiza_estado = "Imposible registrar la migración del codigo sii  " & sqlresultinsert
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            Registra_migracion_radicado_actualiza_estado = "YES"
            Exit Function
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Registra_migracion_radicado_actualiza_estado = "An exception of type " + ex.GetType().ToString() + _
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Registra_migracion_radicado_actualiza_estado = "Error General funcion Registra_migracion_radicado_actualiza_estado " & e.Message
            Exit Function
        End Try
    End Function
    Function Registra_imagen_sii(ByVal id_anexo_sii As Integer, _
                                 ByVal codigo_sii As String, _
                                 ByVal url_sii As String, _
                                 ByVal id_imagen_da As Integer, _
                                 ByVal gabiente As String) As String
        Try
            Dim date1al As String = Date.Now
            Dim Result As String = ""
            Result = ""
            Dim Refclas_gestion_fechas As New ClassGestionFechas
            Result = Refclas_gestion_fechas.Formatea_fecha_time_framework(Date.Now, _
                                                                          date1al)
            If Result <> "YES" Then
                Registra_imagen_sii = "Imposible formatear fecha " & Result
                Exit Function
            End If
            Dim sql_insert As String = "Insert into sii_migra_registro_imagen (fecha_registro,id_anexo_sii,codigo_sii,url_sii,id_imagen_docuarchi,gabinete,estado_error) values (" & _
                                                                               "'" & date1al & "'," & id_anexo_sii & ",'" & codigo_sii & "','" & url_sii & "'," & id_imagen_da & _
                                                                               ",'" & gabiente & "','YES')"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Result = Ref_Car_Conec.SELECTION_INSERT_COMMAND(sql_insert)
            If Result <> "YES" Then
                Registra_imagen_sii = "Imposible hacer el registro de la imagen migrada : " & Result
                Exit Function
            Else
                Registra_imagen_sii = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Registra_imagen_sii = "Inconsistencia general funcion Registra_imagen_sii " & ex.Message
        End Try
    End Function
    Function Solicita_existencia_registro_imagen_sii(ByVal id_anexo_sii As Integer, _
                                                    ByVal codigo_sii As String, _
                                                    ByRef estado_existencia As String) As String
        Try
            Dim Parametro_Consulta As String = " SELECT  id_migra_registro_imagen " & _
              " from sii_migra_registro_imagen where id_anexo_sii=" & _
               id_anexo_sii & " and codigo_sii='" & codigo_sii & "'"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_doc_entrante")
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_existencia_registro_imagen_sii = "Función Solicita_existencia_registro_imagen_sii dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_existencia = "NO"
                Solicita_existencia_registro_imagen_sii = "YES"
                Exit Function
            Else
                estado_existencia = "YES"
                Solicita_existencia_registro_imagen_sii = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_existencia_registro_imagen_sii = "Inconsisencia general funcion Solicita_existencia_registro_imagen_sii " & ex.Message
        End Try
    End Function
End Class
