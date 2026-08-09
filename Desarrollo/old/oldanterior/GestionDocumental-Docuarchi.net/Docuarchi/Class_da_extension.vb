Public Structure Extensiones_Docuarchi
    Dim ESTENSION As String
    Dim ESTADO_NORMAL As Integer
    Dim ESTADO_ADJUNTO As Integer
    Dim ESTADO_LINK As Integer
    Dim PROGRAMA As String
    Dim RUTA_PROGRAMA As String
    Dim TIPO_ARCHIVO As String
    Dim DESCRIPCION_ARCHIVO As String
End Structure
Public Class Class_da_extension
    Function SolicitaExtensionArchivoGabineteTipoImagen(ByVal IdTipoImagen As Integer,
                                                        ByRef ExtensionArchivo As String) As String
        Try
            Dim SqlConsulta As String = "select ESTENSION  from  da_extension " &
                   " where ESTADO_NORMAL='" & IdTipoImagen & "' OR ESTADO_ADJUNTO=" & IdTipoImagen & " OR ESTADO_LINK=" & IdTipoImagen
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("da_extension")
            Dim Result As String = ref.SELECTION_SELECT_FIELDA(SqlConsulta, Datset)
            If Result <> "YES" Then
                Return " La función SolicitaExtensionArchivoGabineteTipoImagen dice : " & Result
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Return "Imposible encontrar extensión de la imagen "
            Else
                ExtensionArchivo = Datset.Tables(0).Rows(0).Item(0)
                Return "YES"
            End If

        Catch ex As Exception
            SolicitaExtensionArchivoGabineteTipoImagen = "Inconsistencia función  SolicitaExtensionArchivoGabineteTipoImagen " & ex.Message
        End Try
    End Function
    Function SolicitaTipoArchivoDocuarchiExtension(ByVal EXTENSION As String,
                                                   Optional ByRef TIPO_ESTADO_NORMAL As String = "-1",
                                                   Optional ByRef TIPO_ESTADO_ADJUNTO As String = "-10",
                                                   Optional ByRef TYPO_ESTADO_LINK As String = "-11") As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita el tipo de archivo docuarchi por extension 
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'EXTENSION           : Representa la extensión del archivo ejemplo 
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2014-02-24
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT ESTADO_NORMAL,ESTADO_ADJUNTO,ESTADO_LINK,TIPO_ARCHIVO,PROGRAMA " &
            " FROM " & "DA_EXTENSION" &
            " WHERE ESTENSION='" & UCase(EXTENSION) & "'"
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("DA_EXTENSION")
            Result = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Result <> "YES" Then
                SolicitaTipoArchivoDocuarchiExtension = "Imposible encontrar CODIGO extensión para visualización " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaTipoArchivoDocuarchiExtension = "Imposible encontrar extensiones para visualización"
                Exit Function
            Else
                TIPO_ESTADO_NORMAL = Datset.Tables(0).Rows(0).Item(0)
                TIPO_ESTADO_ADJUNTO = Datset.Tables(0).Rows(0).Item(1)
                TYPO_ESTADO_LINK = Datset.Tables(0).Rows(0).Item(2)
                SolicitaTipoArchivoDocuarchiExtension = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaTipoArchivoDocuarchiExtension = "Funcion Determina_Tipo_Documento_Extensión" & ex.Message
        End Try
    End Function
    Function Determina_tipo_documento_list(ByVal Cod_Doc As Integer,
                                           ByRef Visor As String,
                                           ByRef Extension As String,
                                           ByRef Estado_Documento As String) As String
        '-------------------------------------------------
        'Funcion : Devolber el tipo de documento, exten
        'sion, el tipo visor, estado de documento tomando
        'como referencia el codigo numerico del documento
        'Fecha : 2012-08-01
        '--------------------------------------------------

        Try
            If Cod_Doc = 1 Then
                Visor = "PRINCIPAL"
                Extension = ".TIF"
                Estado_Documento = "NORMAL"
                Determina_tipo_documento_list = "YES"
                Exit Function
            End If
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT ESTENSION,ESTADO_NORMAL,ESTADO_ADJUNTO,ESTADO_LINK,TIPO_ARCHIVO,PROGRAMA " &
            " FROM " & "DA_EXTENSION" &
            " WHERE ESTADO_NORMAL=" & Cod_Doc & " or ESTADO_ADJUNTO=" & Cod_Doc & " or ESTADO_LINK=" & Cod_Doc
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("DA_EXTENSION")
            Result = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Result <> "YES" Then
                Determina_tipo_documento_list = "Imposible encontrar extensiones para visualizacion " & Result
                Exit Function
            End If
            Dim EXTENSION_DA() As Extensiones_Docuarchi
            Erase EXTENSION_DA
            ReDim Preserve EXTENSION_DA(0)
            If Datset.Tables(0).Rows.Count = 0 Then
                Determina_tipo_documento_list = "Imposible encontrar extensiones para visualizacion"
                Exit Function
            Else
                EXTENSION_DA(0).ESTENSION = Datset.Tables(0).Rows(0).Item(0)
                EXTENSION_DA(0).ESTADO_NORMAL = Datset.Tables(0).Rows(0).Item(1)
                EXTENSION_DA(0).ESTADO_ADJUNTO = Datset.Tables(0).Rows(0).Item(2)
                EXTENSION_DA(0).ESTADO_LINK = Datset.Tables(0).Rows(0).Item(3)
                EXTENSION_DA(0).TIPO_ARCHIVO = Datset.Tables(0).Rows(0).Item(4)
            End If
            For i As Integer = 0 To EXTENSION_DA.Length - 1
                If EXTENSION_DA(i).ESTADO_ADJUNTO = Cod_Doc Then
                    Visor = EXTENSION_DA(i).PROGRAMA
                    Extension = EXTENSION_DA(i).ESTENSION
                    Estado_Documento = "ADJUNTO"
                    Determina_tipo_documento_list = "YES"
                    Exit Function

                End If
                If EXTENSION_DA(i).ESTADO_LINK = Cod_Doc Then
                    Visor = EXTENSION_DA(i).PROGRAMA
                    Extension = EXTENSION_DA(i).ESTENSION
                    Estado_Documento = "LINK"
                    Determina_tipo_documento_list = "YES"
                    Exit Function

                End If
                If EXTENSION_DA(i).ESTADO_NORMAL = Cod_Doc Then
                    Visor = EXTENSION_DA(i).PROGRAMA
                    Extension = EXTENSION_DA(i).ESTENSION
                    Estado_Documento = "NORMAL"
                    Determina_tipo_documento_list = "YES"
                    Exit Function
                End If
            Next
            Determina_tipo_documento_list = "No hay considencias para la extension del documento"
        Catch ex As Exception
            Determina_tipo_documento_list = "Inconsistencia general funcion Determina_tipo_documento_list " & ex.Message
        End Try
    End Function
    Function RetornaExtensionTipoDocumento(ByVal IdTipo As Integer,
                                           ByRef exten As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la extension de un tipo de documento
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdTipo           : Representa la identificación del tipo documento
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'exten               :Retorna la extensión de un tipo de documento
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2013-08-09
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------------------
        Try
            Dim i As Integer = 0
            Dim Sql_consulta = "SELECT ESTENSION FROM " &
                 "DA_EXTENSION " &
                 "WHERE ESTADO_NORMAL=" & IdTipo & " or ESTADO_ADJUNTO=" & IdTipo & " or " &
                 " ESTADO_LINK=" & IdTipo
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("DA_EXTENSION")
            Dim result As String = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If result <> "YES" Then
                RetornaExtensionTipoDocumento = "Error funcion RetornaExtensionTipoDocumento " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                RetornaExtensionTipoDocumento = "No se pudo determinar la extensión de archivo asociada al identificador (" & IdTipo & ")"
                Exit Function
            Else
                exten = Datset.Tables(0).Rows(0).Item(0)
                RetornaExtensionTipoDocumento = "YES"
                Exit Function
            End If
        Catch ex As Exception
            RetornaExtensionTipoDocumento = "Inconsistencia general Funcion RetornaExtensionTipoDocumento " & ex.Message
        End Try
    End Function
    Function Solicita_el_tipo_documento_docuarchi_segun_extension_de_archivo(ByVal extension As String,
                                                                           ByRef tipo_archivo As Integer) As String
        Try
            Dim Parametro_Consulta = "select ESTADO_NORMAL " &
            " from da_extension Where ESTENSION='" & UCase(extension) & "'"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("da_extension")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_el_tipo_documento_docuarchi_segun_extension_de_archivo = "Funcion  Solicita_el_tipo_documento_docuarchi_segun_extension_de_archivo dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_el_tipo_documento_docuarchi_segun_extension_de_archivo = "Imposible encontrar la identificación de la extensión " & extension
                Exit Function
            Else
                tipo_archivo = Datset.Tables(0).Rows(0).Item(0)
                Solicita_el_tipo_documento_docuarchi_segun_extension_de_archivo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_el_tipo_documento_docuarchi_segun_extension_de_archivo = "Inconsistencia función Solicita_el_tipo_documento_docuarchi_segun_extension_de_archivo " & ex.Message
        End Try
    End Function
End Class
