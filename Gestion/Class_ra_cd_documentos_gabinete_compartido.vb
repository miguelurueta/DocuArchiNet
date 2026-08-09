Public Structure stru_docu_compartido
    Dim ID_IMAGEN As Long
    Dim NOMBRE_GABINETE As String
    Dim RUTA_DOCUMENTO As String
End Structure
Public Class Class_ra_cd_documentos_gabinete_compartido
    Function Solicita_id_registro_documento_compartido_gabinete(ByVal id_imagen As Integer,
                                                                ByVal nombre_gabinete As String,
                                                                ByRef id_registro_doc_compartido As Long) As String
        '----------------------------------------------------------------------------
        'Función : Solicita el registro de documento compartido asociado a la magen
        'o documento.
        'Fecha : 2017-12-15
        'Ing :Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------
        Try
            Dim Parametro_Consulta = "select RA_CD_DOCUMENTOS_COMPARTIDOS_ID_RA_CD_DOCUMENTOS_COMPARTIDOS " &
            " from ra_cd_doumentos_gabinete_compartidos where ID_IMAGEN=" & id_imagen &
            " and NOMBRE_GABINETE='" & nombre_gabinete & "'"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_doumentos_gabinete_compartidos")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_registro_documento_compartido_gabinete = "Funcion  Solicita_verificar_imagen_relacionada_a_documento_compartido dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_registro_doc_compartido = 0
                Solicita_id_registro_documento_compartido_gabinete = "YES"
                Exit Function
            Else
                id_registro_doc_compartido = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_registro_documento_compartido_gabinete = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Solicita_id_registro_documento_compartido_gabinete = "Inconsistencia general función Solicita_id_registro_documento_compartido_gabinete " & ex.Message
        End Try
    End Function
    Function SolicitaDatosEstructuraDocumentoCompartido(ByVal IdDocumentoCompartido As Long,
                                                        ByRef stru_documento() As stru_docu_compartido) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita estructura de documentos relaciandos al registro de un documento compartido
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdDocumentoCompartido  : Representa la identificación del registro de un documento compartido
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'stru_documento  : Retorna la estructura de un documento compartido
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-05-13
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------------------
        Try
            Dim sqlconsulta As String = "Select ID_IMAGEN,NOMBRE_GABINETE  from ra_cd_doumentos_gabinete_compartidos " &
           " where RA_CD_DOCUMENTOS_COMPARTIDOS_ID_RA_CD_DOCUMENTOS_COMPARTIDOS=" & IdDocumentoCompartido
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_doumentos_gabinete_compartidos")
            Dim result As String = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If result <> "YES" Then
                SolicitaDatosEstructuraDocumentoCompartido = "Error función SolicitaDatosEstructuraDocumentoCompartido " & result
                Exit Function
            End If
            Erase stru_documento
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaDatosEstructuraDocumentoCompartido = "No se encontraron documentos relacionados con la solicitud de aprobación número  (" & IdDocumentoCompartido & ")."
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_documento(i)
                    stru_documento(i).ID_IMAGEN = Datset.Tables(0).Rows(0).Item(0)
                    stru_documento(i).NOMBRE_GABINETE = Datset.Tables(0).Rows(0).Item(1)
                Next
                SolicitaDatosEstructuraDocumentoCompartido = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaDatosEstructuraDocumentoCompartido = "Incosistencia general función SolicitaDatosEstructuraDocumentoCompartido " & ex.Message
        End Try
    End Function
End Class
