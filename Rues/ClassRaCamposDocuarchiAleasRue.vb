Public Structure SturcturaCampoConsultaAleasRue
    Dim campo_docuarchi As String
    Dim Aleas_rue As String
End Structure
Public Class ClassRaCamposDocuarchiAleasRue
    Function SolicitaCamposGabinetesRues(ByVal struc_campos() As SturcturaCampoConsultaAleasRue,
                                         ByVal Gabinete As String,
                                         ByRef class_campos_table_bostra_table As List(Of class_campos_table_bostra_table)) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la estructura de campos tipo boot para consulta de documentos RUES
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'struc_campos           : Representa la estructura de campos RUES
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'class_campos_table_bostra_table  : Retorna la idnetificación del usuario radicador
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-05-06
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            class_campos_table_bostra_table = New List(Of class_campos_table_bostra_table)
            Dim item As New class_campos_table_bostra_table
            item = New class_campos_table_bostra_table
            item.title = "ID"
            item.field = "ID"
            item.visible = False
            item.viisble_sql = 1
            item.visible_like_sql = 1
            class_campos_table_bostra_table.Add(item)
            For i As Integer = 0 To struc_campos.Length - 1
                item = New class_campos_table_bostra_table
                item.title = struc_campos(i).Aleas_rue
                item.field = struc_campos(i).campo_docuarchi
                item.visible = True
                item.viisble_sql = 1
                class_campos_table_bostra_table.Add(item)
            Next
            item = New class_campos_table_bostra_table
            item.field = "operate"
            item.title = "Ver detalle Inscripción"
            item.checkbox = False
            item.visible = True
            item.viisble_sql = 0
            item.clickToSelect = False
            item.visible_like_sql = 0
            item.align = "center"
            item.events = "window.operateEventsrUEDocumentos"
            item.formatter = "operateFormattertablebootRueDocumentos"
            class_campos_table_bostra_table.Add(item)
            SolicitaCamposGabinetesRues = "YES"
            Exit Function
        Catch ex As Exception
            SolicitaCamposGabinetesRues = "Inconsistencia general fucntion SolicitaCamposGabinetesRues " & ex.Message
        End Try
    End Function
    Function SolicitaNombresDocuarchiAleasRue(ByVal IdGabineteRue As Integer,
                                              ByRef struc_campos() As SturcturaCampoConsultaAleasRue) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita estructura de campos consulta aleas gabinete rue
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdGabineteRue           : Representa la identificación del gabinete rue
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'struc_campos  : Retorna la estructura de campos aeas rue
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-06-05
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------------------
        Try
            Dim Parametro_consulta As String = "Select NOMBRE_CAMPOS_DOCUARCHI,NOMBRE_CAMPOS_ALEAS_RUE from ra_campos_docuarchi_aleas_rue where Ra_tiporue_gabinete_ID_RUE_GABINETE='" & IdGabineteRue & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_campos_docuarchi_aleas_rue")
            Dim Result = ref.SELECTION_SELECT_FIELDA(Parametro_consulta, Datset)
            If Result <> "YES" Then
                SolicitaNombresDocuarchiAleasRue = "Función SolicitaNombresDocuarchiAleasRue Error (" & Result & ")"
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaNombresDocuarchiAleasRue = "Imposible encontrar los campos de consulta  en la tabla relación ra_campos_docuarchi_aleas_rue"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve struc_campos(i)
                    struc_campos(i).campo_docuarchi = Datset.Tables(0).Rows(i).Item(0)
                    struc_campos(i).Aleas_rue = Datset.Tables(0).Rows(i).Item(1)
                Next
                SolicitaNombresDocuarchiAleasRue = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaNombresDocuarchiAleasRue = "Inconsistencia general función SolicitaNombresDocuarchiAleasRue " & ex.Message
        End Try
    End Function
End Class
