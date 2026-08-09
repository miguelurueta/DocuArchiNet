Public Class Class_ra_campos_relacionados_ruta_plantilla
    Function solicita_relacion_campos_ruta_plantilla(ByVal id_relacion_ruta_plantilla As Integer, _
                                                     ByRef stru_campos_plantilla_ruta() _
                                                     As csfc_structure_relacion_campos_plantilla_ruta) As String
        '---------------------------------------------------------------------
        'Función : Lista los campos relación de las plantillas de radicación
        'y el listado de campos ruta con el codigo de de la ruta y el código
        'de la plantilla
        'Fecha : 2017-08-23
        'Ing : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------
        Try
            Dim Sql_consulta As String = "Select rcr.NOMBRE_CAMPO_PLANTILLA,rcr.TIPO_CAMPO_PLANTILLA,rcr.DIMENSION_CAMPO_PLANTILLA," & _
                "rcr.NOMBRE_CAMPO_RUTA,rcr.TIPO_CAMPO_RUTA,rcr.DIMENSION_CAMPO_RUTA " & _
                " from ra_campos_relacionados_ruta_plantilla rcr " & _
                " where RA_RELACION_RUTA_PLANTILLA_ID_RELACION_RUTA_PLANTILLA=" & id_relacion_ruta_plantilla
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("LISTADO_ACTIVIDADES_WORKFLOW")
            Dim result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If result <> "YES" Then
                solicita_relacion_campos_ruta_plantilla = "Función  solicita_relacion_campos_ruta_plantilla dice " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                solicita_relacion_campos_ruta_plantilla = "Imposible encontrar los campos relación del  plantilla radicación y rutas  workflow de la relacion (" & id_relacion_ruta_plantilla & ")"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_campos_plantilla_ruta(i)
                    stru_campos_plantilla_ruta(i).nombre_campo_plantilla = Datset.Tables(0).Rows(i).Item(0)
                    stru_campos_plantilla_ruta(i).tipo_campo_plantilla = Datset.Tables(0).Rows(i).Item(1)
                    stru_campos_plantilla_ruta(i).dimension_campo_plantilla = Datset.Tables(0).Rows(i).Item(2)
                    stru_campos_plantilla_ruta(i).nombre_campo_ruta = Datset.Tables(0).Rows(i).Item(3)
                    stru_campos_plantilla_ruta(i).tipo_campo_ruta = Datset.Tables(0).Rows(i).Item(4)
                    stru_campos_plantilla_ruta(i).dimension_campo_ruta = Datset.Tables(0).Rows(i).Item(5)
                Next
                solicita_relacion_campos_ruta_plantilla = "YES"
                Exit Function
            End If
        Catch ex As Exception
            solicita_relacion_campos_ruta_plantilla = "Inconsistencia general función solicita_relacion_campos_ruta_plantilla " & ex.Message
        End Try
    End Function
End Class
