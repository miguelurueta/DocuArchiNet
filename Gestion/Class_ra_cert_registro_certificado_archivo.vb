Imports System.IO
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.OleDb
Imports System.Data.Odbc
Imports MySql.Data.MySqlClient
Public Class class_ra_cert_registro_certificado_archivo_reponse
    Property Error_gestion As String
    Property Icono_file As String

End Class
Public Class Class_ra_cert_registro_certificado_archivo
    Function Agrega_certificado_digital_a_documento(ByVal id_certificado As Integer,
                                                    ByVal id_registro_producion As Long,
                                                    ByVal id_imagen As String,
                                                    ByVal gabinete As String,
                                                    ByVal archivo As String,
                                                    ByVal estado_registra_firma_producion As Integer,
                                                    ByVal id_registro_version As Long,
                                                    ByRef id_registro_certificado_archivo As Long) As String
        '-------------------------------------------------------------
        'Funcion : Registra la transacción del certificado digital
        'gabinete cuando se remplaza
        'Ing . Miguel Angel Urueta Miranda
        'Fecha : 2022-03-15
        '-------------------------------------------------------------
        Dim ClassGestionFechas As New ClassGestionFechas
        Dim date_fecha As String = Now
        Dim Result As String = ClassGestionFechas.Formatea_Fecha_Almacenamiento_Time(date_fecha)
        If Result <> "YES" Then
            Agrega_certificado_digital_a_documento = Result
            Exit Function
        End If
        Dim ref_archivo As String = archivo.Replace("\", "/")
        Dim Sql_insert As String = "Insert into ra_cert_registro_certificado_archivo (ra_cert_certificado_id_certificado,registro_producion_documental_ID_REGISTRO," &
                "fecha_registro,Ruta_archivo_copia,Estado_archivo_copia,id_archivo,nombre_gabinete,id_registro_version) values (" & id_certificado & "," & id_registro_producion & "," &
                "'" & date_fecha & "','" & ref_archivo & "',0," & id_imagen & ",'" & gabinete & "'," & id_registro_version & ")"
        Dim ref As New conect.Dbase_Conction_Mysql_DA
        Dim myConnection As New MySqlConnection
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Try
            '-------------------------------------------------------------
            'Registra la firma de la firma digital
            '-------------------------------------------------------------
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = Sql_insert
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Agrega_certificado_digital_a_documento = "Imposible registrar la firma digital " & Sql_insert
                myConnection.Close()
                errorM = "Imposible registrar la firma digital" & Sql_insert
                Exit Function
            End If
            If estado_registra_firma_producion = 1 Then
                '-------------------------------------------------------------
                'Actualiza estado firma digital en la producción documental
                '-------------------------------------------------------------
                Dim id_registro_firma_digital As Object = myCommand.LastInsertedId
                Dim Sql_update_producion = "update registro_producion_documental set ESTADO_FIRMA_DIGITAL=" & 1 & ",CERTIFICADO_FIRMA_DIGITAL=" & id_certificado &
                   ",FECHA_FIRMA_DIGITAL='" & date_fecha & "',ID_REGISTRO_FIRMA_DIGITAL=" & id_registro_firma_digital &
                   " where ID_REGISTRO_PRODUCION_DOCUMENTAL=" & id_registro_producion
                myCommand.CommandText = Sql_update_producion
                Switc = myCommand.ExecuteNonQuery()
                If Switc = 0 Then
                    Agrega_certificado_digital_a_documento = "Imposible actualizar el estado de firma en la produccion documental  : " & Sql_update_producion
                    myTrans.Rollback()
                    myConnection.Close()
                    errorM = "Imposible actualizar el estado de firma en la produccion documental  : " & Sql_update_producion
                    Exit Function
                End If
                '-------------------------------------------------------------
                'Actualiza estado firma digital del documento en el gabinete
                '-------------------------------------------------------------
                Dim update_gabinte As String = "update " & gabinete & " set ESTADO_FIRMA_DIGITAL=" & 1 & " where ID=" & id_imagen
                myCommand.CommandText = update_gabinte
                Switc = myCommand.ExecuteNonQuery()
                If Switc = 0 Then
                    Agrega_certificado_digital_a_documento = "Imposible actualizar el estado de firma en el gabinete  : " & Sql_update_producion
                    myTrans.Rollback()
                    myConnection.Close()
                    errorM = "Imposible actualizar el estado de firma en el gabinete  : " & Sql_update_producion
                    Exit Function
                End If
                '//-----------------Actualizar estado firma en el registro de version----////
                If id_registro_version <> 0 Then
                    Dim SQLupdate = "UPDATE ra_ver_version_documento SET ESTADO_FIRMA_DIGITAL=1 WHERE id_registro_version=" & id_registro_version
                    myCommand.CommandText = SQLupdate
                    Switc = myCommand.ExecuteNonQuery()
                    If Switc = 0 Then
                        Agrega_certificado_digital_a_documento = "Imposible actualizar el regitro de firma en el constrol de versiones  : " & SQLupdate
                        myTrans.Rollback()
                        myConnection.Close()
                        errorM = "Imposible actualizar el regitro de firma en el constrol de versiones  : " & SQLupdate
                        Exit Function
                    End If
                End If
            End If
            myTrans.Commit()
            myConnection.Close()
            Agrega_certificado_digital_a_documento = "YES"
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                myConnection.Close()
                Agrega_certificado_digital_a_documento = "An exception of type " + ex.GetType().ToString() +
                                  " was encountered while attempting to roll back the transaction."

                Exit Function
            End If
        Finally
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            If errorM <> "YES" Then
                Agrega_certificado_digital_a_documento = errorM
            End If
        End Try

    End Function
    Function Solicita_registro_certificado_archivo_imagen_gabinete(ByVal id_imagen_archivo As Integer,
                                                                   ByVal nombre_gabinete As String,
                                                                   ByRef id_certificado As Long) As String
        '--------------------------------------------------------------------------------------------------
        'Funcion : Funcion solicita el registro de certificado digital de una imagen con la identificación
        'del archivo y el nombre del gabinete
        '--------------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------------------------
        'id_imagen_archivo   : Representa la identificación de la imagen en el gabinete
        'nombre_gabinete     : Representa el nombre del gabinete
        '
        '--------------------------------------------------------------------------------------------------
        '                           RETORNO
        '--------------------------------------------------------------------------------------------------
        'id_certificado  : Retorna la identificación de certifificado
        '--------------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '--------------------------------------------------------------------------------------------------
        'Fecha                 : 2025-01-17
        'Elabora               : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------------------------------------------
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select ra_cert_certificado_id_certificado " &
                "from ra_cert_registro_certificado_archivo where id_archivo=" & id_imagen_archivo &
                " and nombre_gabinete='" & nombre_gabinete & "'" &
                " order by id_registro_certificado_archivo desc"
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_registro_certificado_archivo_imagen_gabinete = "Functión Solicita_registro_certificado_archivo_imagen_gabinete dice   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_certificado = 0
                Solicita_registro_certificado_archivo_imagen_gabinete = "YES"
                Exit Function
            Else
                id_certificado = Datset.Tables(0).Rows(0).Item(0)
                Solicita_registro_certificado_archivo_imagen_gabinete = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_registro_certificado_archivo_imagen_gabinete = "Inconsistencia general fución Solicita_registro_certificado_archivo_imagen_gabinete " & ex.Message
        End Try
    End Function

    Function Solicita_registro_certificado_archivo(ByVal id_registro_produccion As Long,
                                                   ByVal id_registro_version As Integer,
                                                   ByRef id_certificado As Long) As String
        '--------------------------------------------------------------------------------------------------
        'Funcion : Funcion solicita el registro de certificado digital con la identificción del registro
        'de producción documental
        '
        '--------------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------------------------
        'id_registro_produccion   : Representa la identificación del registro de producion
        'id_registro_version      : Representa la identificacion del registro de version del documento
        '
        '
        '--------------------------------------------------------------------------------------------------
        '                           RETORNO
        '--------------------------------------------------------------------------------------------------
        'id_certificado  : Retorna la identificación de certifificado
        '--------------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '--------------------------------------------------------------------------------------------------
        'Fecha                 : 2022-03-17
        'Elabora               : Miguel Angel Urueta Miranda
        'Modificación          : 2024-01-27
        'Elabora               : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------------------------------------------
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select ra_cert_certificado_id_certificado " &
                "from ra_cert_registro_certificado_archivo where registro_producion_documental_ID_REGISTRO=" & id_registro_produccion &
                " and id_registro_version=" & id_registro_version &
                " order by id_registro_certificado_archivo desc"
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_registro_certificado_archivo = "Functión Solicita_registro_certificado_archivo dice   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_certificado = 0
                Solicita_registro_certificado_archivo = "YES"
                Exit Function
            Else
                id_certificado = Datset.Tables(0).Rows(0).Item(0)
                Solicita_registro_certificado_archivo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_registro_certificado_archivo = "Inconsistencia general funcion Solicita_registro_certificado_archivo " & ex.Message
        End Try
    End Function
    Function Solicita_registro_certificado_registro_producion(ByVal id_registro_produccion As Long,
                                                              ByRef CountCertificados As Long) As String
        '--------------------------------------------------------------------------------------------------
        'Funcion : Funcion que solicita el numero de archivos firmados del registro de produccion documental
        '
        '
        '--------------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------------------------
        'id_registro_produccion   : Representa el registro unico de producción documental
        '--------------------------------------------------------------------------------------------------
        '                           RETORNO
        '--------------------------------------------------------------------------------------------------
        'id_certificado  : Representa el numero de versiones de un documento que se encuentran firmadas
        'digitalmente 
        '--------------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '--------------------------------------------------------------------------------------------------
        'Fecha                 : 2025-02-06
        'Elabora               : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------------------------------------------
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select ra_cert_certificado_id_certificado " &
                "from ra_cert_registro_certificado_archivo where registro_producion_documental_ID_REGISTRO=" & id_registro_produccion &
                " order by id_registro_certificado_archivo desc"
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_registro_certificado_registro_producion = "Functión Solicita_registro_certificado_registro_producion dice   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                CountCertificados = 0
                Solicita_registro_certificado_registro_producion = "YES"
                Exit Function
            Else
                CountCertificados = Datset.Tables(0).Rows.Count
                Solicita_registro_certificado_registro_producion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_registro_certificado_registro_producion = "Inconsistencia general funcion Solicita_registro_certificado_registro_producion " & ex.Message
        End Try
    End Function
End Class
