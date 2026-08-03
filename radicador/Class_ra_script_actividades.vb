Public Class Class_ra_script_actividades
    Function lista_campos_Validacion_plantilla(ByVal id_plantilla As Integer, _
                                              ByRef stru_campos() As validacion_plantilla) As String
        '**************************************************************
        'Funcion : lista campos validacion de una plantilla especifica
        'con el parametro id plantilla
        'Fecha : 2014-07-23
        'Ingeniero : Miguel Angel Urueta Miranda
        '***************************************************************
        Try
            Erase stru_campos
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select NOMBRE_CAMPO,TIPO_SCRIPT,COMBINACION_TECLA,Valor_Script,Estado_Script,PLATAFORMA_SCRIPT,Id_Script " & _
            " from ra_script_actividades where system_plantilla_radicado_id_Plantilla = " & _
            id_plantilla & " AND Estado_Script=1"
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                lista_campos_Validacion_plantilla = " Error listado plantilla radicacion   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                lista_campos_Validacion_plantilla = "YES"
                Exit Function
            Else
                For Iconta As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_campos(Iconta)
                    If Datset.Tables(0).Rows(Iconta).IsNull(0) = False Then
                        stru_campos(Iconta).Campo_Plantilla = Datset.Tables(0).Rows(Iconta).Item(0).ToString
                    Else
                        stru_campos(Iconta).Campo_Plantilla = ""
                    End If
                    If Datset.Tables(0).Rows(Iconta).IsNull(1) = False Then
                        stru_campos(Iconta).TIPO_SCRIPT = Datset.Tables(0).Rows(Iconta).Item(1).ToString
                    Else
                        stru_campos(Iconta).TIPO_SCRIPT = ""
                    End If
                    If Datset.Tables(0).Rows(Iconta).IsNull(2) = False Then
                        stru_campos(Iconta).COMBINACION_TECLA = Datset.Tables(0).Rows(Iconta).Item(2).ToString
                    Else
                        stru_campos(Iconta).COMBINACION_TECLA = ""
                    End If
                    If Datset.Tables(0).Rows(Iconta).IsNull(3) = False Then
                        stru_campos(Iconta).VALOR_SCRIPT = Datset.Tables(0).Rows(Iconta).Item(3).ToString
                    Else
                        stru_campos(Iconta).VALOR_SCRIPT = ""
                    End If
                    If Datset.Tables(0).Rows(Iconta).IsNull(4) = False Then
                        stru_campos(Iconta).ESTADO_ESCRIPT = Datset.Tables(0).Rows(Iconta).Item(4).ToString
                    Else
                        stru_campos(Iconta).ESTADO_ESCRIPT = ""
                    End If
                    If Datset.Tables(0).Rows(Iconta).IsNull(5) = False Then
                        stru_campos(Iconta).PLATAFORMA_SCRIPT = Datset.Tables(0).Rows(Iconta).Item(5).ToString
                    Else
                        stru_campos(Iconta).PLATAFORMA_SCRIPT = ""
                    End If
                    If Datset.Tables(0).Rows(Iconta).IsNull(6) = False Then
                        stru_campos(Iconta).ID_SCRIPT = Datset.Tables(0).Rows(Iconta).Item(6).ToString
                    Else
                        stru_campos(Iconta).ID_SCRIPT = 0
                    End If
                Next
            End If
            lista_campos_Validacion_plantilla = "YES"
        Catch ex As Exception
            lista_campos_Validacion_plantilla = "Inconsistencia general funcion lista_campos_Validacion_plantilla " & ex.Message
        End Try
    End Function
End Class
