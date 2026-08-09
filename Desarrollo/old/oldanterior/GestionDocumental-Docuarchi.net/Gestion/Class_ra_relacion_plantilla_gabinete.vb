
Public Class Class_ra_relacion_plantilla_gabinete
    Function SolicitaCamposRelacionPlantillaGabinete(ByVal IdPlantillaRadicado As Integer,
                                                     ByVal IdGabinete As Integer,
                                                     ByRef stru_campos_plantilla_ruta() As _
                                                         csfc_structure_relacion_campos_plantilla_ruta) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion :Solicita  los campos relación de las plantillas de radicación y el gabinete
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdPlantillaRadicado : Representa la identificación de la plantilla de radicación
        'IdGabinete          : Representa la identificación del gabinete
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'stru_campos_plantilla_ruta  : Retorna la estructura de relacion de campos
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2017-08-23
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Sql_consulta As String = "Select rcr.NOMBRE_CAMPO_PLANTILLA,rcr.TIPO_CAMPO_PLANTILLA,rcr.DIMENSION_CAMPO_PLANTILLA," &
                "rcr.NOMBRE_CAMPO_GABINETE,rcr.TIPO_CAMPO_GABINETE,rcr.DIMENSION_CAMPO_GABINETE " &
                " from ra_relacion_plantilla_gabinete rrr " &
                " inner join ra_campos_relacionados_plantilla_gabinete rcr on (rcr.RA_RELACION_PLANTILLA_GABINETE_ID_RELACION_PLANTILLA_GABINETE=rrr.ID_RELACION_PLANTILLA_GABINETE)" &
                " where system_plantilla_radicado_id_Plantilla='" & IdPlantillaRadicado & "' and " &
                " system1_id_gabinete=" & IdGabinete
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("LISTADO_ACTIVIDADES_WORKFLOW")
            Dim result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If result <> "YES" Then
                SolicitaCamposRelacionPlantillaGabinete = "Función  SolicitaCamposRelacionPlantillaGabinete dice " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaCamposRelacionPlantillaGabinete = "Imposible encontrar campos relacionados entre la plantilla  " & IdPlantillaRadicado & " y el gabinete " & IdGabinete
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
                SolicitaCamposRelacionPlantillaGabinete = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaCamposRelacionPlantillaGabinete = "Inconsistencia general función SolicitaCamposRelacionPlantillaGabinete " & ex.Message
        End Try
    End Function
End Class
